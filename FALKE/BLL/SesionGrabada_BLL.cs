using System;
using System.Collections.Generic;
using System.IO;
using BE;
using ORM;
using SERVICES;
using TE;
using TLL;

namespace BLL
{
    public class SesionGrabada_BLL
    {
        public const long MAX_BYTES_VIDEO = 1500L * 1024 * 1024;
        public const long MAX_BYTES_MIRADA = 100L * 1024 * 1024;
        private const int LARGO_NOMBRE = 100;
        private const int LARGO_OBJETIVO = 500;
        private const int MAX_SEGUNDOS = 4 * 3600;

        private readonly SesionGrabadaRepository sesionRepo;
        private readonly CategoriaRepository categoriaRepo;
        private readonly GestorIntegridad_SERVICE gestorIntegridad;
        private readonly BitacoraGestor_TLL bitacora;

        public SesionGrabada_BLL()
        {
            sesionRepo = new SesionGrabadaRepository();
            categoriaRepo = new CategoriaRepository();
            gestorIntegridad = new GestorIntegridad_SERVICE();
            bitacora = new BitacoraGestor_TLL();
        }

        /// <summary>
        /// Sesiones de la empresa del usuario con la sesion iniciada. La empresa sale del ACTOR (sesion del servidor),
        /// nunca de un parametro: no hay forma de pedir las de otra empresa.
        /// </summary>
        public List<SesionGrabada_BE> Listar(ActorUsuario_TE actor)
        {
            actor.Exigir(Patentes_TLL.OPERAR_ANALISIS);

            if (actor.IdEmpresa <= 0) throw new ExcepcionNoAutorizada_TLL("comun_sesion_sin_empresa");

            return sesionRepo.ObtenerPorEmpresa(actor.IdEmpresa);
        }

        /// <summary>
        /// Una sesion, solo si es de la empresa del actor. Si no existe o es de otra empresa, el resultado es el mismo
        /// (sesion_no_existe) para no revelar que el ID existe.
        /// </summary>
        public SesionGrabada_BE ObtenerPropia(ActorUsuario_TE actor, int idSesion)
        {
            actor.Exigir(Patentes_TLL.OPERAR_ANALISIS);

            if (actor.IdEmpresa <= 0) throw new ExcepcionNoAutorizada_TLL("comun_sesion_sin_empresa");

            SesionGrabada_BE sesion = sesionRepo.ObtenerDeEmpresa(actor.IdEmpresa, idSesion);

            if (sesion == null) throw new ExcepcionTraducible_TLL("sesion_no_existe");

            return sesion;
        }

        public static string NombreVideo(int idSesion) { return idSesion + ".webm"; }
        public static string NombreMirada(int idSesion) { return idSesion + ".json"; }

        /// <summary>
        /// Guarda la sesion y sus dos archivos (video y mirada) bajo el mismo ID.
        /// Todo o nada: si falla algo, no queda fila ni archivos.
        /// </summary>
        public SesionGrabada_BE Guardar(ActorUsuario_TE actor, SesionGrabada_BE sesion, Stream video, Stream mirada, string carpeta)
        {
            actor.Exigir(Patentes_TLL.OPERAR_ANALISIS);

            if (sesion == null) throw new ArgumentNullException(nameof(sesion));
            if (video == null || video.Length == 0) throw new ExcepcionTraducible_TLL("sesion_sin_video");
            if (video.Length > MAX_BYTES_VIDEO) throw new ExcepcionTraducible_TLL("sesion_video_muy_grande");
            if (mirada != null && mirada.Length > MAX_BYTES_MIRADA) throw new ExcepcionTraducible_TLL("sesion_mirada_muy_grande");
            if (actor.IdEmpresa <= 0 && !actor.EsEmergencia) throw new ExcepcionNoAutorizada_TLL("comun_sesion_sin_empresa");

            Categoria_BE categoria = categoriaRepo.ObtenerPorPK(sesion.IdCategoria);
            if (categoria == null) throw new ExcepcionTraducible_TLL("categoria_no_existe");
            if (!actor.EsEmergencia && categoria.IdEmpresa != actor.IdEmpresa) throw new ExcepcionNoAutorizada_TLL("categoria_otra_empresa");
            if (!categoria.Activa) throw new ExcepcionTraducible_TLL("sesion_categoria_inactiva");

            sesion.Nombre = (sesion.Nombre ?? string.Empty).Trim();
            sesion.Objetivo = (sesion.Objetivo ?? string.Empty).Trim();
            if (sesion.Nombre.Length == 0) throw new ExcepcionTraducible_TLL("sesion_nombre_obligatorio");
            if (sesion.Nombre.Length > LARGO_NOMBRE) throw new ExcepcionTraducible_TLL("sesion_nombre_largo", LARGO_NOMBRE);
            if (sesion.Objetivo.Length > LARGO_OBJETIVO) throw new ExcepcionTraducible_TLL("sesion_objetivo_largo", LARGO_OBJETIVO);
            if (!PerfilTester_BE.EsValido(sesion.RangoEdad, sesion.Genero, sesion.Familiaridad)) throw new ExcepcionTraducible_TLL("sesion_perfil_invalido");

            if (!EsWebm(video)) throw new ExcepcionTraducible_TLL("sesion_video_invalido");

            sesion.IdUsuario = actor.IdUsuario;
            sesion.Fecha = DateTime.Now;
            sesion.Estado = SesionGrabada_BE.ESTADO_FINALIZADA;
            sesion.DuracionSegundos = Math.Max(0, Math.Min(sesion.DuracionSegundos, MAX_SEGUNDOS));
            sesion.BytesVideo = video.Length;
            sesion.Muestras = mirada == null ? 0 : ContarMuestras(mirada);

            Directory.CreateDirectory(carpeta);

            string rutaVideo = null;
            string rutaMirada = null;

            try
            {
                Transaccion_ORM.Ejecutar(() =>
                {
                    sesionRepo.Alta(sesion);

                    rutaVideo = Path.Combine(carpeta, NombreVideo(sesion.IdSesion));
                    Copiar(video, rutaVideo);

                    if (mirada != null && sesion.Muestras > 0)
                    {
                        rutaMirada = Path.Combine(carpeta, NombreMirada(sesion.IdSesion));
                        Copiar(mirada, rutaMirada);
                    }

                    gestorIntegridad.ActualizarDVHRegistro(TablasBD.SesionGrabada, new[] { sesion.IdSesion.ToString() });

                    bitacora.Auditar(actor, "Grabaciones", categoria.IdEmpresa, EventosBitacora_TLL.SESION_GRABADA, CriticidadBitacora.Media,
                        DetalleBitacora_TLL.Armar(sesion.Nombre));
                });
            }
            catch
            {
                Borrar(rutaVideo);
                Borrar(rutaMirada);
                throw;
            }

            return sesion;
        }

        /// <summary>
        /// Nombre de la siguiente parte: "Menu" -> "Menu - 2", "Menu - 2" -> "Menu - 3".
        /// </summary>
        public string ProponerNombreSiguiente(ActorUsuario_TE actor, int idCategoria, string nombre)
        {
            actor.Exigir(Patentes_TLL.OPERAR_ANALISIS);

            string baseNombre = NombreBase(nombre);
            int n = sesionRepo.ContarEncadenadas(idCategoria, baseNombre) + 1;
            string propuesto = baseNombre + " - " + n;

            return propuesto.Length > LARGO_NOMBRE ? propuesto.Substring(propuesto.Length - LARGO_NOMBRE) : propuesto;
        }

        public static string NombreBase(string nombre)
        {
            string t = (nombre ?? string.Empty).Trim();
            int i = t.LastIndexOf(" - ", StringComparison.Ordinal);

            if (i > 0)
            {
                int n;
                if (int.TryParse(t.Substring(i + 3), out n) && n >= 2) return t.Substring(0, i);
            }

            return t;
        }

        private static bool EsWebm(Stream s)
        {
            byte[] firma = new byte[4];
            s.Position = 0;
            int leidos = s.Read(firma, 0, 4);
            s.Position = 0;
            return leidos == 4 && firma[0] == 0x1A && firma[1] == 0x45 && firma[2] == 0xDF && firma[3] == 0xA3;
        }

        private static int ContarMuestras(Stream s)
        {
            s.Position = 0;
            string texto;
            using (var lector = new StreamReader(s, System.Text.Encoding.UTF8, false, 4096, true))
            {
                texto = lector.ReadToEnd();
            }
            s.Position = 0;

            if (!texto.TrimStart().StartsWith("{", StringComparison.Ordinal)) throw new ExcepcionTraducible_TLL("sesion_error_guardado");

            int cuenta = 0;
            int pos = 0;
            while ((pos = texto.IndexOf("\"u\":", pos, StringComparison.Ordinal)) >= 0)
            {
                cuenta++;
                pos += 4;
            }

            return cuenta;
        }

        private static void Copiar(Stream origen, string destino)
        {
            origen.Position = 0;
            using (var archivo = new FileStream(destino, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                origen.CopyTo(archivo, 81920);
            }
        }

        private static void Borrar(string ruta)
        {
            try { if (ruta != null && File.Exists(ruta)) File.Delete(ruta); } catch { }
        }
    }
}
