using System;
using System.Collections.Generic;
using BE;
using ORM;
using SERVICES;
using TE;
using TLL;

namespace BLL
{
    public class Categoria_BLL
    {
        private const int LARGO_NOMBRE = 100;
        private const int LARGO_NOMBRE_ACTIVO = 100;
        private const int LARGO_FLUJO = 500;
        private const int LARGO_CAMPO_ESPECIFICO = 50;
        private const int LARGO_URL = 300;

        private readonly CategoriaRepository categoriaRepo;
        private readonly GestorIntegridad_SERVICE gestorIntegridad;
        private readonly BitacoraGestor_TLL bitacora;

        public Categoria_BLL()
        {
            categoriaRepo = new CategoriaRepository();
            gestorIntegridad = new GestorIntegridad_SERVICE();
            bitacora = new BitacoraGestor_TLL();
        }

        public List<Categoria_BE> ObtenerPorEmpresa(ActorUsuario_TE actor)
        {
            ExigirPatente(actor, Patentes_TLL.OPERAR_ANALISIS);

            if (actor.IdEmpresa <= 0) throw new UnauthorizedAccessException("La sesión no tiene una empresa asociada.");

            return categoriaRepo.ObtenerPorEmpresa(actor.IdEmpresa);
        }

        public Categoria_BE ObtenerPorId(ActorUsuario_TE actor, int idCategoria)
        {
            ExigirPatente(actor, Patentes_TLL.OPERAR_ANALISIS);

            return ObtenerPropia(actor, idCategoria);
        }

        public void Crear(ActorUsuario_TE actor, Categoria_BE categoria)
        {
            ExigirPatente(actor, Patentes_TLL.GESTIONAR_CATEGORIAS);

            if (categoria == null) throw new ArgumentNullException(nameof(categoria));
            if (actor.IdEmpresa <= 0) throw new UnauthorizedAccessException("La sesión no tiene una empresa asociada.");

            categoria.IdEmpresa = actor.IdEmpresa;

            ValidarYNormalizar(categoria);

            categoria.FechaCreacion = DateTime.Now;
            categoria.Activa = true;

            Transaccion_ORM.Ejecutar(() =>
            {
                categoriaRepo.Alta(categoria);

                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Categoria, new[] { categoria.IdCategoria.ToString() });
                gestorIntegridad.ActualizarDVHRegistro(TablaEspecifica(categoria.Tipo), new[] { categoria.IdCategoria.ToString() });

                Auditar(actor, "Alta de la categoría \"" + categoria.NombreCategoria + "\" (" + categoria.Tipo + ").", CriticidadBitacora.Media);
            });
        }

        public void Modificar(ActorUsuario_TE actor, int idCategoria, Categoria_BE nuevos)
        {
            ExigirPatente(actor, Patentes_TLL.GESTIONAR_CATEGORIAS);

            if (nuevos == null) throw new ArgumentNullException(nameof(nuevos));

            Categoria_BE actual = ObtenerPropia(actor, idCategoria);

            nuevos.IdEmpresa = actual.IdEmpresa;
            ValidarYNormalizar(nuevos, idCategoria);

            TipoActivoCategoria tipoAnterior = actual.Tipo;

            nuevos.IdCategoria = actual.IdCategoria;
            nuevos.FechaCreacion = actual.FechaCreacion;
            nuevos.Activa = actual.Activa;

            Transaccion_ORM.Ejecutar(() =>
            {
                categoriaRepo.Modificar(nuevos);

                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Categoria, new[] { idCategoria.ToString() });
                gestorIntegridad.ActualizarDVHRegistro(TablaEspecifica(nuevos.Tipo), new[] { idCategoria.ToString() });

                if (tipoAnterior != nuevos.Tipo) gestorIntegridad.GuardarIntegridadTabla(TablaEspecifica(tipoAnterior));

                Auditar(actor, "Modificación de la categoría \"" + nuevos.NombreCategoria + "\".", CriticidadBitacora.Media);
            });
        }

        public void Eliminar(ActorUsuario_TE actor, int idCategoria)
        {
            ExigirPatente(actor, Patentes_TLL.GESTIONAR_CATEGORIAS);

            Categoria_BE actual = ObtenerPropia(actor, idCategoria);

            int sesiones = categoriaRepo.ContarSesiones(idCategoria);

            if (sesiones == 0)
            {
                Transaccion_ORM.Ejecutar(() =>
                {
                    categoriaRepo.Eliminar(idCategoria);

                    gestorIntegridad.GuardarIntegridadTabla(TablasBD.Categoria);
                    gestorIntegridad.GuardarIntegridadTabla(TablaEspecifica(actual.Tipo));

                    Auditar(actor, "Baja de la categoría \"" + actual.NombreCategoria + "\".", CriticidadBitacora.Alta);
                });

                return;
            }

            if (!actual.Activa) throw new InvalidOperationException("La categoría ya está desactivada.");

            actual.Activa = false;

            Transaccion_ORM.Ejecutar(() =>
            {
                categoriaRepo.Modificar(actual);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Categoria, new[] { idCategoria.ToString() });

                Auditar(actor, "La categoría \"" + actual.NombreCategoria + "\" tiene " + sesiones + " sesión(es) grabada(s): se desactivó en lugar de eliminarla.", CriticidadBitacora.Alta);
            });
        }

        private Categoria_BE ObtenerPropia(ActorUsuario_TE actor, int idCategoria)
        {
            Categoria_BE categoria = categoriaRepo.ObtenerPorPK(idCategoria);

            if (categoria == null) throw new InvalidOperationException("La categoría no existe.");

            if (!actor.EsEmergencia && categoria.IdEmpresa != actor.IdEmpresa)
                throw new UnauthorizedAccessException("La categoría no pertenece a tu empresa.");

            return categoria;
        }

        private void ExigirPatente(ActorUsuario_TE actor, string patente)
        {
            if (actor != null && actor.Puede(patente)) return;

            bitacora.Registrar(actor != null ? actor.IdUsuario : 0, "Seguridad", "Acción rechazada por falta de permiso (" + patente + ")", CriticidadBitacora.Media);
            throw new UnauthorizedAccessException("No tenés permiso para realizar esta acción.");
        }

        private void Auditar(ActorUsuario_TE actor, string descripcion, CriticidadBitacora criticidad)
        {
            bitacora.Guardar(new Bitacora_TE(actor.IdUsuario, "Categorías", descripcion, criticidad, DateTime.Now) { IdEmpresa = actor.IdEmpresa });
        }

        private static TablasBD TablaEspecifica(TipoActivoCategoria tipo)
        {
            switch (tipo)
            {
                case TipoActivoCategoria.Software: return TablasBD.CategoriaSoftware;
                case TipoActivoCategoria.AppWeb: return TablasBD.CategoriaAppWeb;
                case TipoActivoCategoria.AppMovil: return TablasBD.CategoriaAppMovil;
                case TipoActivoCategoria.Videojuego: return TablasBD.CategoriaVideojuego;
                default: return TablasBD.CategoriaPublicidad;
            }
        }

        private void ValidarYNormalizar(Categoria_BE c, int idCategoriaPropia = 0)
        {
            c.NombreCategoria = (c.NombreCategoria ?? string.Empty).Trim();
            c.NombreActivo = Recortar(c.NombreActivo);
            c.FlujoEsperado = Recortar(c.FlujoEsperado);

            if (c.NombreCategoria.Length == 0) throw new InvalidOperationException("El nombre de la categoría es obligatorio.");
            if (c.NombreCategoria.Length > LARGO_NOMBRE) throw new InvalidOperationException("El nombre de la categoría no puede superar los " + LARGO_NOMBRE + " caracteres.");
            if (Largo(c.NombreActivo) > LARGO_NOMBRE_ACTIVO) throw new InvalidOperationException("El nombre del activo no puede superar los " + LARGO_NOMBRE_ACTIVO + " caracteres.");
            if (Largo(c.FlujoEsperado) > LARGO_FLUJO) throw new InvalidOperationException("El flujo a analizar no puede superar los " + LARGO_FLUJO + " caracteres.");

            if (!Enum.IsDefined(typeof(TipoActivoCategoria), c.Tipo)) throw new InvalidOperationException("El tipo de activo no es válido.");

            if (categoriaRepo.ExisteNombre(c.IdEmpresa, c.NombreCategoria, idCategoriaPropia))
                throw new InvalidOperationException("Ya existe una categoría con ese nombre en tu empresa.");

            switch (c.Tipo)
            {
                case TipoActivoCategoria.Software:
                    c.SistemaOperativoSoftware = Recortar(c.SistemaOperativoSoftware);
                    c.VersionSoftware = Recortar(c.VersionSoftware);
                    ValidarLargoEspecifico(c.SistemaOperativoSoftware, "sistema operativo");
                    ValidarLargoEspecifico(c.VersionSoftware, "versión del software");
                    break;

                case TipoActivoCategoria.AppWeb:
                    c.UrlAppWeb = Recortar(c.UrlAppWeb);
                    ValidarLargoEspecifico(c.UrlAppWeb, "URL", LARGO_URL);
                    if (!Enum.IsDefined(typeof(DispositivoObjetivo), c.DispositivoAppWeb)) throw new InvalidOperationException("El dispositivo objetivo no es válido.");
                    break;

                case TipoActivoCategoria.AppMovil:
                    c.VersionAppMovil = Recortar(c.VersionAppMovil);
                    ValidarLargoEspecifico(c.VersionAppMovil, "versión de la aplicación");
                    if (!Enum.IsDefined(typeof(SistemaOperativoMovil), c.SoAppMovil)) throw new InvalidOperationException("El sistema operativo objetivo no es válido.");
                    break;

                case TipoActivoCategoria.Videojuego:
                    c.VersionVideojuego = Recortar(c.VersionVideojuego);
                    ValidarLargoEspecifico(c.VersionVideojuego, "versión del juego");
                    if (!Enum.IsDefined(typeof(PlataformaVideojuego), c.Plataforma)) throw new InvalidOperationException("La plataforma no es válida.");
                    break;

                case TipoActivoCategoria.Publicidad:
                    c.FormatoPublicidad = Recortar(c.FormatoPublicidad);
                    c.CanalPublicidad = Recortar(c.CanalPublicidad);
                    ValidarLargoEspecifico(c.FormatoPublicidad, "formato");
                    ValidarLargoEspecifico(c.CanalPublicidad, "canal de distribución");
                    break;
            }
        }

        private static void ValidarLargoEspecifico(string valor, string nombreCampo, int maximo = LARGO_CAMPO_ESPECIFICO)
        {
            if (Largo(valor) > maximo) throw new InvalidOperationException("El campo \"" + nombreCampo + "\" no puede superar los " + maximo + " caracteres.");
        }

        private static string Recortar(string texto)
        {
            string limpio = (texto ?? string.Empty).Trim();
            return limpio.Length == 0 ? null : limpio;
        }

        private static int Largo(string texto)
        {
            return texto == null ? 0 : texto.Length;
        }
    }
}
