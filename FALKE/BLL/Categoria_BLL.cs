using System;
using System.Collections.Generic;
using System.Linq;
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

            if (actor.IdEmpresa <= 0) throw new ExcepcionNoAutorizada_TLL("comun_sesion_sin_empresa");

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
            if (actor.IdEmpresa <= 0) throw new ExcepcionNoAutorizada_TLL("comun_sesion_sin_empresa");

            categoria.IdEmpresa = actor.IdEmpresa;

            ValidarYNormalizar(categoria);

            categoria.FechaCreacion = DateTime.Now;
            categoria.Activa = true;

            Transaccion_ORM.Ejecutar(() =>
            {
                categoriaRepo.Alta(categoria);

                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Categoria, new[] { categoria.IdCategoria.ToString() });
                gestorIntegridad.ActualizarDVHRegistro(categoriaRepo.TablaEspecifica(categoria.Tipo), new[] { categoria.IdCategoria.ToString() });

                bitacora.Auditar(actor, "Categorías", actor.IdEmpresa, EventosBitacora_TLL.CATEGORIA_ALTA, CriticidadBitacora.Media, DetalleBitacora_TLL.Armar(categoria.NombreCategoria));
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

                bitacora.Auditar(actor, "Categorías", actor.IdEmpresa, EventosBitacora_TLL.CATEGORIA_MODIFICACION, CriticidadBitacora.Media, DetalleBitacora_TLL.Armar(nuevos.NombreCategoria));
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

                    bitacora.Auditar(actor, "Categorías", actor.IdEmpresa, EventosBitacora_TLL.CATEGORIA_BAJA, CriticidadBitacora.Alta, DetalleBitacora_TLL.Armar(actual.NombreCategoria));
                });

                return;
            }

            if (!actual.Activa) throw new ExcepcionTraducible_TLL("categoria_ya_desactivada");

            actual.Activa = false;

            Transaccion_ORM.Ejecutar(() =>
            {
                categoriaRepo.Modificar(actual);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Categoria, new[] { idCategoria.ToString() });

                bitacora.Auditar(actor, "Categorías", actor.IdEmpresa, EventosBitacora_TLL.CATEGORIA_DESACTIVADA, CriticidadBitacora.Alta,
                        DetalleBitacora_TLL.Armar(actual.NombreCategoria));
            });
        }

        private Categoria_BE ObtenerPropia(ActorUsuario_TE actor, int idCategoria)
        {
            Categoria_BE categoria = categoriaRepo.ObtenerPorPK(idCategoria);

            if (categoria == null) throw new ExcepcionTraducible_TLL("categoria_no_existe");

            if (!actor.EsEmergencia && categoria.IdEmpresa != actor.IdEmpresa)
                throw new ExcepcionNoAutorizada_TLL("categoria_otra_empresa");

            return categoria;
        }

        private void ValidarYNormalizar(Categoria_BE c, int idCategoriaPropia = 0)
        {
            c.NombreCategoria = (c.NombreCategoria ?? string.Empty).Trim();
            c.NombreActivo = Recortar(c.NombreActivo);
            c.FlujoEsperado = Recortar(c.FlujoEsperado);

            if (c.NombreCategoria.Length == 0) throw new ExcepcionTraducible_TLL("categoria_nombre_obligatorio");
            if (c.NombreCategoria.Length > LARGO_NOMBRE) throw new ExcepcionTraducible_TLL("categoria_nombre_largo", LARGO_NOMBRE);
            if (Largo(c.NombreActivo) > LARGO_NOMBRE_ACTIVO) throw new ExcepcionTraducible_TLL("categoria_activo_largo", LARGO_NOMBRE_ACTIVO);
            if (Largo(c.FlujoEsperado) > LARGO_FLUJO) throw new ExcepcionTraducible_TLL("categoria_flujo_largo", LARGO_FLUJO);

            if (categoriaRepo.ExisteNombre(c.IdEmpresa, c.NombreCategoria, idCategoriaPropia))
                throw new ExcepcionTraducible_TLL("categoria_duplicada");

            ErrorValidacion_BE errorEspecifico = c.NormalizarYValidar();

            if (errorEspecifico != null)
                throw new ExcepcionTraducible_TLL(errorEspecifico.Codigo, errorEspecifico.Parametros.Select(p => p is string ? new ClaveTexto_TLL((string)p) : p).ToArray());
        }
    }
}
