using System;
using System.Collections.Generic;
using BE;
using ORM;
using SERVICES;
using TE;
using TLL;
using static TLL.TextoHelper_TLL;

namespace BLL
{
    public class Categoria_BLL
    {
        private const int LARGO_NOMBRE = 100;
        private const int LARGO_NOMBRE_ACTIVO = 100;
        private const int LARGO_FLUJO = 500;

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
            actor.Exigir(Patentes_TLL.OPERAR_ANALISIS);

            if (actor.IdEmpresa <= 0) throw new UnauthorizedAccessException("La sesión no tiene una empresa asociada.");

            return categoriaRepo.ObtenerPorEmpresa(actor.IdEmpresa);
        }

        public Categoria_BE ObtenerPorId(ActorUsuario_TE actor, int idCategoria)
        {
            actor.Exigir(Patentes_TLL.OPERAR_ANALISIS);

            return ObtenerPropia(actor, idCategoria);
        }

        public void Crear(ActorUsuario_TE actor, Categoria_BE categoria)
        {
            actor.Exigir(Patentes_TLL.GESTIONAR_CATEGORIAS);

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
                gestorIntegridad.ActualizarDVHRegistro(categoriaRepo.TablaEspecifica(categoria.Tipo), new[] { categoria.IdCategoria.ToString() });

                bitacora.Auditar(actor, "Categorías", actor.IdEmpresa, "Alta de la categoría \"" + categoria.NombreCategoria + "\" (" + categoria.Tipo + ").", CriticidadBitacora.Media);
            });
        }

        public void Modificar(ActorUsuario_TE actor, int idCategoria, Categoria_BE nuevos)
        {
            actor.Exigir(Patentes_TLL.GESTIONAR_CATEGORIAS);

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
                gestorIntegridad.ActualizarDVHRegistro(categoriaRepo.TablaEspecifica(nuevos.Tipo), new[] { idCategoria.ToString() });

                if (tipoAnterior != nuevos.Tipo) gestorIntegridad.GuardarIntegridadTabla(categoriaRepo.TablaEspecifica(tipoAnterior));

                bitacora.Auditar(actor, "Categorías", actor.IdEmpresa, "Modificación de la categoría \"" + nuevos.NombreCategoria + "\".", CriticidadBitacora.Media);
            });
        }

        public void Eliminar(ActorUsuario_TE actor, int idCategoria)
        {
            actor.Exigir(Patentes_TLL.GESTIONAR_CATEGORIAS);

            Categoria_BE actual = ObtenerPropia(actor, idCategoria);

            int sesiones = categoriaRepo.ContarSesiones(idCategoria);

            if (sesiones == 0)
            {
                Transaccion_ORM.Ejecutar(() =>
                {
                    categoriaRepo.Eliminar(idCategoria);

                    gestorIntegridad.GuardarIntegridadTabla(TablasBD.Categoria);
                    gestorIntegridad.GuardarIntegridadTabla(categoriaRepo.TablaEspecifica(actual.Tipo));

                    bitacora.Auditar(actor, "Categorías", actor.IdEmpresa, "Baja de la categoría \"" + actual.NombreCategoria + "\".", CriticidadBitacora.Alta);
                });

                return;
            }

            if (!actual.Activa) throw new InvalidOperationException("La categoría ya está desactivada.");

            actual.Activa = false;

            Transaccion_ORM.Ejecutar(() =>
            {
                categoriaRepo.Modificar(actual);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Categoria, new[] { idCategoria.ToString() });

                bitacora.Auditar(actor, "Categorías", actor.IdEmpresa, "La categoría \"" + actual.NombreCategoria + "\" tiene " + sesiones + " sesión(es) grabada(s): se desactivó en lugar de eliminarla.", CriticidadBitacora.Alta);
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

        private void ValidarYNormalizar(Categoria_BE c, int idCategoriaPropia = 0)
        {
            c.NombreCategoria = (c.NombreCategoria ?? string.Empty).Trim();
            c.NombreActivo = Recortar(c.NombreActivo);
            c.FlujoEsperado = Recortar(c.FlujoEsperado);

            if (c.NombreCategoria.Length == 0) throw new InvalidOperationException("El nombre de la categoría es obligatorio.");
            if (c.NombreCategoria.Length > LARGO_NOMBRE) throw new InvalidOperationException("El nombre de la categoría no puede superar los " + LARGO_NOMBRE + " caracteres.");
            if (Largo(c.NombreActivo) > LARGO_NOMBRE_ACTIVO) throw new InvalidOperationException("El nombre del activo no puede superar los " + LARGO_NOMBRE_ACTIVO + " caracteres.");
            if (Largo(c.FlujoEsperado) > LARGO_FLUJO) throw new InvalidOperationException("El flujo a analizar no puede superar los " + LARGO_FLUJO + " caracteres.");

            if (categoriaRepo.ExisteNombre(c.IdEmpresa, c.NombreCategoria, idCategoriaPropia))
                throw new InvalidOperationException("Ya existe una categoría con ese nombre en tu empresa.");

            string errorEspecifico = c.NormalizarYValidar();

            if (errorEspecifico != null) throw new InvalidOperationException(errorEspecifico);
        }
    }
}
