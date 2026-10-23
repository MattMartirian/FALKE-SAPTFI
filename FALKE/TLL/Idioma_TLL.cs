using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ORM;
using TE;

namespace TLL
{
    public class Idioma_TLL
    {
        public const string MODULO = "Idiomas";

        private static readonly Regex formatoCodigo = new Regex(@"^[A-Za-z]{2,3}(-[A-Za-z0-9]{2,8})?$", RegexOptions.Compiled);
        private static readonly Regex marcadores = new Regex(@"\{\d+\}", RegexOptions.Compiled);

        private readonly IdiomaRepository idiomaRepo;
        private readonly TraduccionRepository traduccionRepo;
        private readonly ITraductorAutomatico_TLL traductor;
        private readonly BitacoraGestor_TLL bitacora;

        public Idioma_TLL() : this(new TraductorResx_TLL()) { }

        public Idioma_TLL(ITraductorAutomatico_TLL traductor)
        {
            idiomaRepo = new IdiomaRepository();
            traduccionRepo = new TraduccionRepository();
            this.traductor = traductor;
            bitacora = new BitacoraGestor_TLL();
        }

        public List<Idioma_TE> ListarActivos()
        {
            return idiomaRepo.ObtenerActivos();
        }

        public Idioma_TE ObtenerPorCodigo(string codigo)
        {
            return idiomaRepo.ObtenerPorCodigo(NormalizarCodigo(codigo));
        }

        public Idioma_TE ObtenerPorId(int idIdioma)
        {
            return idiomaRepo.ObtenerPorPK(idIdioma);
        }

        public List<Idioma_TE> Listar(ActorUsuario_TE actor)
        {
            actor.Exigir(Patentes_TLL.GESTIONAR_IDIOMAS);

            return idiomaRepo.ObtenerTodos();
        }

        public List<Idioma_TE> ListarDisponibles(ActorUsuario_TE actor)
        {
            actor.Exigir(Patentes_TLL.GESTIONAR_IDIOMAS);

            var disponibles = new List<Idioma_TE>();

            foreach (string codigo in traductor.ObtenerCodigosDisponibles())
            {
                if (idiomaRepo.ExisteCodigo(codigo)) continue;

                disponibles.Add(new Idioma_TE { CodigoIdioma = codigo, NombreIdioma = NombreNativo(codigo) });
            }

            return disponibles;
        }

        public Idioma_TE Agregar(ActorUsuario_TE actor, string codigo)
        {
            actor.Exigir(Patentes_TLL.GESTIONAR_IDIOMAS);

            codigo = NormalizarCodigo(codigo);

            if (!formatoCodigo.IsMatch(codigo)) throw new ExcepcionTraducible_TLL("idioma_codigo_invalido");
            if (idiomaRepo.ExisteCodigo(codigo)) throw new ExcepcionTraducible_TLL("idioma_ya_existe", codigo);
            if (!traductor.Disponible(codigo)) throw new ExcepcionTraducible_TLL("idioma_no_disponible", codigo);

            var idioma = new Idioma_TE
            {
                CodigoIdioma = codigo,
                NombreIdioma = NombreNativo(codigo),
                Activo = true,
                EsBase = false
            };

            Transaccion_ORM.Ejecutar(() =>
            {
                idiomaRepo.Alta(idioma);
                traduccionRepo.AltaMasiva(Armar(idioma.IdIdioma, codigo));

                bitacora.Auditar(actor, MODULO, null, EventosBitacora_TLL.IDIOMA_ALTA, CriticidadBitacora.Media, DetalleBitacora_TLL.Armar(idioma.NombreIdioma));
            });

            Traductor_TLL.Invalidar();

            return idiomaRepo.ObtenerPorPK(idioma.IdIdioma);
        }

        public int Sincronizar(ActorUsuario_TE actor, int idIdioma)
        {
            actor.Exigir(Patentes_TLL.GESTIONAR_IDIOMAS);

            Idioma_TE idioma = Obtener(idIdioma);

            if (!traductor.Disponible(idioma.CodigoIdioma)) throw new ExcepcionTraducible_TLL("idioma_no_disponible", idioma.CodigoIdioma);

            List<Traduccion_TE> nuevas = Faltantes(idioma);

            if (nuevas.Count == 0) return 0;

            Transaccion_ORM.Ejecutar(() =>
            {
                traduccionRepo.AltaMasiva(nuevas);

                bitacora.Auditar(actor, MODULO, null, EventosBitacora_TLL.IDIOMA_SINCRONIZACION, CriticidadBitacora.Baja,
                    DetalleBitacora_TLL.Armar(idioma.NombreIdioma));
            });

            Traductor_TLL.Invalidar();

            return nuevas.Count;
        }

        private List<Traduccion_TE> Faltantes(Idioma_TE idioma)
        {
            HashSet<string> guardadas = new HashSet<string>(traduccionRepo.ObtenerPorIdioma(idioma.IdIdioma).Select(t => t.CodigoClave), StringComparer.Ordinal);

            return Armar(idioma.IdIdioma, idioma.CodigoIdioma).Where(t => !guardadas.Contains(t.CodigoClave)).ToList();
        }

        public void CambiarEstado(ActorUsuario_TE actor, int idIdioma, bool activo)
        {
            actor.Exigir(Patentes_TLL.GESTIONAR_IDIOMAS);

            Idioma_TE idioma = Obtener(idIdioma);

            if (idioma.Activo == activo) throw new ExcepcionTraducible_TLL(activo ? "idioma_ya_habilitado" : "idioma_ya_deshabilitado");

            if (!activo)
            {
                if (idioma.EsBase) throw new ExcepcionTraducible_TLL("idioma_base_inmutable");
                if (idioma.CantidadUsuarios > 0) throw new ExcepcionTraducible_TLL("idioma_en_uso", idioma.NombreIdioma, idioma.CantidadUsuarios);
            }

            idioma.Activo = activo;

            Transaccion_ORM.Ejecutar(() =>
            {
                idiomaRepo.Modificar(idioma);

                bitacora.Auditar(actor, MODULO, null, activo ? EventosBitacora_TLL.IDIOMA_HABILITADO : EventosBitacora_TLL.IDIOMA_DESHABILITADO, CriticidadBitacora.Media, DetalleBitacora_TLL.Armar(idioma.NombreIdioma));
            });
        }

        public void Eliminar(ActorUsuario_TE actor, int idIdioma)
        {
            actor.Exigir(Patentes_TLL.GESTIONAR_IDIOMAS);

            Idioma_TE idioma = Obtener(idIdioma);

            if (idioma.EsBase) throw new ExcepcionTraducible_TLL("idioma_base_inmutable");
            if (idioma.CantidadUsuarios > 0) throw new ExcepcionTraducible_TLL("idioma_en_uso", idioma.NombreIdioma, idioma.CantidadUsuarios);

            Transaccion_ORM.Ejecutar(() =>
            {
                traduccionRepo.EliminarPorIdioma(idIdioma);
                idiomaRepo.Eliminar(idIdioma);

                bitacora.Auditar(actor, MODULO, null, EventosBitacora_TLL.IDIOMA_BAJA, CriticidadBitacora.Alta, DetalleBitacora_TLL.Armar(idioma.NombreIdioma));
            });

            Traductor_TLL.Invalidar();
        }

        public List<Traduccion_TE> ListarTraducciones(ActorUsuario_TE actor, int idIdioma, bool soloPendientes)
        {
            actor.Exigir(Patentes_TLL.GESTIONAR_IDIOMAS);

            Obtener(idIdioma);

            return soloPendientes ? traduccionRepo.ObtenerPendientes(idIdioma) : traduccionRepo.ObtenerPorIdioma(idIdioma);
        }

        public void GuardarTraduccion(ActorUsuario_TE actor, int idIdioma, string codigoClave, string texto)
        {
            actor.Exigir(Patentes_TLL.GESTIONAR_IDIOMAS);

            Idioma_TE idioma = Obtener(idIdioma);
            Traduccion_TE actual = traduccionRepo.ObtenerPorPK(new TraduccionClave_TE(codigoClave, idIdioma));

            if (actual == null) throw new ExcepcionTraducible_TLL("traduccion_no_existe");

            texto = string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();

            if (texto != null) ExigirMarcadores(codigoClave, texto);

            actual.Texto = texto;
            actual.Pendiente = texto == null;
            actual.Origen = OrigenTraduccion.Manual;

            Transaccion_ORM.Ejecutar(() =>
            {
                traduccionRepo.Modificar(actual);

                bitacora.Auditar(actor, MODULO, null, EventosBitacora_TLL.IDIOMA_TRADUCCION, CriticidadBitacora.Baja, DetalleBitacora_TLL.Armar(codigoClave, " (", idioma.NombreIdioma, ")"));
            });

            Traductor_TLL.Invalidar();
        }

        private void ExigirMarcadores(string codigoClave, string texto)
        {
            Idioma_TE idiomaBase = idiomaRepo.ObtenerBase();
            if (idiomaBase == null) return;

            Traduccion_TE original = traduccionRepo.ObtenerPorPK(new TraduccionClave_TE(codigoClave, idiomaBase.IdIdioma));
            if (original == null || original.Texto == null) return;

            List<string> esperados = ExtraerMarcadores(original.Texto);

            if (!esperados.SequenceEqual(ExtraerMarcadores(texto))) throw new ExcepcionTraducible_TLL("traduccion_marcadores", esperados.Count == 0 ? "-" : string.Join(", ", esperados));
        }

        private static List<string> ExtraerMarcadores(string texto)
        {
            return marcadores.Matches(texto).Cast<Match>().Select(m => m.Value).Distinct().OrderBy(m => m, StringComparer.Ordinal).ToList();
        }

        private List<Traduccion_TE> Armar(int idIdioma, string codigoIdioma)
        {
            Dictionary<string, string> baseTextos = traductor.ObtenerTraducciones(Traductor_TLL.CODIGO_BASE);
            Dictionary<string, string> paquete = traductor.ObtenerTraducciones(codigoIdioma);
            var lista = new List<Traduccion_TE>();

            foreach (string clave in baseTextos.Keys.OrderBy(k => k, StringComparer.Ordinal))
            {
                string texto;

                if (paquete.TryGetValue(clave, out texto) && !string.IsNullOrWhiteSpace(texto)) lista.Add(new Traduccion_TE(clave, idIdioma, texto, false, OrigenTraduccion.Resx));
                else lista.Add(new Traduccion_TE(clave, idIdioma, null, true, OrigenTraduccion.Manual));
            }

            return lista;
        }

        private Idioma_TE Obtener(int idIdioma)
        {
            Idioma_TE idioma = idiomaRepo.ObtenerPorPK(idIdioma);

            if (idioma == null) throw new ExcepcionTraducible_TLL("idioma_no_existe");

            return idioma;
        }

        private static string NormalizarCodigo(string codigo)
        {
            codigo = (codigo ?? string.Empty).Trim();

            int guion = codigo.IndexOf('-');
            if (guion < 0) return codigo.ToLowerInvariant();

            string region = codigo.Substring(guion + 1);

            return codigo.Substring(0, guion).ToLowerInvariant() + "-" + (region.Length == 2 ? region.ToUpperInvariant() : region);
        }

        private static string NombreNativo(string codigo)
        {
            try
            {
                CultureInfo cultura = CultureInfo.GetCultureInfo(codigo);
                string nombre = cultura.NativeName;

                return char.ToUpper(nombre[0], cultura) + nombre.Substring(1);
            }
            catch (CultureNotFoundException)
            {
                return codigo;
            }
        }
    }
}
