using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace ORM
{
    public class CategoriaRepository : RepositoryBase<Categoria_BE, int>
    {
        public CategoriaRepository() : base() { }

        public override void Alta(Categoria_BE c)
        {
            string sql = @"
                INSERT INTO CategoriaTable
                    (id_empresa, nombre_categoria, tipo_activo_digital_categoria, nombre_activo_categoria,
                     flujo_analizado_categoria, fecha_creacion_categoria, activa_categoria)
                VALUES
                    (@idEmpresa, @nombre, @tipo, @nombreActivo,
                     @flujo, @fechaCreacion, @activa);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            var idGenerado = Gestor.EjecutarScalar<int>(sql,
                new SqlParameter("@idEmpresa", c.IdEmpresa),
                new SqlParameter("@nombre", c.NombreCategoria),
                new SqlParameter("@tipo", Normalizar(c.Tipo)),
                new SqlParameter("@nombreActivo", ValorONulo(c.NombreActivo)),
                new SqlParameter("@flujo", ValorONulo(c.FlujoEsperado)),
                new SqlParameter("@fechaCreacion", c.FechaCreacion),
                new SqlParameter("@activa", c.Activa));

            c.IdCategoria = idGenerado;

            InsertarEspecifico(c);
        }

        public override void Modificar(Categoria_BE c)
        {
            TipoActivoCategoria tipoAnterior = ObtenerTipoActual(c.IdCategoria);

            string sql = @"
                UPDATE CategoriaTable SET
                    nombre_categoria = @nombre,
                    tipo_activo_digital_categoria = @tipo,
                    nombre_activo_categoria = @nombreActivo,
                    flujo_analizado_categoria = @flujo,
                    activa_categoria = @activa
                WHERE id_categoria = @id";

            Gestor.EjecutarNonQuery(sql,
                new SqlParameter("@nombre", c.NombreCategoria),
                new SqlParameter("@tipo", Normalizar(c.Tipo)),
                new SqlParameter("@nombreActivo", ValorONulo(c.NombreActivo)),
                new SqlParameter("@flujo", ValorONulo(c.FlujoEsperado)),
                new SqlParameter("@activa", c.Activa),
                new SqlParameter("@id", c.IdCategoria));

            // Si cambia el tipo de activo, la fila específica anterior ya no corresponde: se reemplaza por una nueva.
            if (tipoAnterior != c.Tipo)
            {
                EliminarEspecifico(c.IdCategoria, tipoAnterior);
                InsertarEspecifico(c);
            }
            else
            {
                ActualizarEspecifico(c);
            }
        }

        public override Categoria_BE ObtenerPorPK(int pk)
        {
            var dt = Gestor.EjecutarQuery(SqlSeleccionCompleta() + " WHERE c.id_categoria = @id", new SqlParameter("@id", pk));
            return dt.Rows.Count == 0 ? null : Map(dt.Rows[0]);
        }

        public override List<Categoria_BE> ObtenerTodos()
        {
            return MapTodos(Gestor.EjecutarQuery(SqlSeleccionCompleta() + " ORDER BY c.nombre_categoria"));
        }

        public List<Categoria_BE> ObtenerPorEmpresa(int idEmpresa)
        {
            var dt = Gestor.EjecutarQuery(SqlSeleccionCompleta() + " WHERE c.id_empresa = @idEmpresa ORDER BY c.nombre_categoria",
                new SqlParameter("@idEmpresa", idEmpresa));
            return MapTodos(dt);
        }

        public bool ExisteNombre(int idEmpresa, string nombre, int excluirIdCategoria = 0)
        {
            string sql = "SELECT COUNT(1) FROM CategoriaTable WHERE id_empresa = @idEmpresa AND nombre_categoria = @nombre AND id_categoria <> @excluir";
            var dt = Gestor.EjecutarQuery(sql,
                new SqlParameter("@idEmpresa", idEmpresa),
                new SqlParameter("@nombre", nombre),
                new SqlParameter("@excluir", excluirIdCategoria));
            return Convert.ToInt32(dt.Rows[0][0]) > 0;
        }

        // Una categoría con sesiones grabadas no se puede eliminar físicamente (ver Categoria_BLL.Eliminar).
        public int ContarSesiones(int idCategoria)
        {
            string sql = "SELECT COUNT(1) FROM SesionGrabadaTable WHERE id_categoria = @id";
            var dt = Gestor.EjecutarQuery(sql, new SqlParameter("@id", idCategoria));
            return Convert.ToInt32(dt.Rows[0][0]);
        }

        // Las tablas específicas no tienen ON DELETE CASCADE: primero se borra la fila del tipo y después la base.
        public void Eliminar(int idCategoria)
        {
            foreach (TipoActivoCategoria tipo in Enum.GetValues(typeof(TipoActivoCategoria)))
                EliminarEspecifico(idCategoria, tipo);

            Gestor.EjecutarNonQuery("DELETE FROM CategoriaTable WHERE id_categoria = @id", new SqlParameter("@id", idCategoria));
        }

        #region Tabla específica por tipo de activo

        private void InsertarEspecifico(Categoria_BE c)
        {
            switch (c.Tipo)
            {
                case TipoActivoCategoria.Software:
                    Gestor.EjecutarNonQuery(
                        "INSERT INTO CategoriaSoftwareTable (id_categoria, sistema_operativo_software, version_software) VALUES (@id, @so, @version)",
                        new SqlParameter("@id", c.IdCategoria),
                        new SqlParameter("@so", ValorONulo(c.SistemaOperativoSoftware)),
                        new SqlParameter("@version", ValorONulo(c.VersionSoftware)));
                    break;

                case TipoActivoCategoria.AppWeb:
                    Gestor.EjecutarNonQuery(
                        "INSERT INTO CategoriaAppWebTable (id_categoria, url_appweb, dispositivo_objetivo_appweb) VALUES (@id, @url, @dispositivo)",
                        new SqlParameter("@id", c.IdCategoria),
                        new SqlParameter("@url", ValorONulo(c.UrlAppWeb)),
                        new SqlParameter("@dispositivo", Normalizar(c.DispositivoAppWeb)));
                    break;

                case TipoActivoCategoria.AppMovil:
                    Gestor.EjecutarNonQuery(
                        "INSERT INTO CategoriaAppMovilTable (id_categoria, sistema_operativo_appmovil, version_appmovil) VALUES (@id, @so, @version)",
                        new SqlParameter("@id", c.IdCategoria),
                        new SqlParameter("@so", Normalizar(c.SoAppMovil)),
                        new SqlParameter("@version", ValorONulo(c.VersionAppMovil)));
                    break;

                case TipoActivoCategoria.Videojuego:
                    Gestor.EjecutarNonQuery(
                        "INSERT INTO CategoriaVideojuegoTable (id_categoria, plataforma_videojuego, version_videojuego) VALUES (@id, @plataforma, @version)",
                        new SqlParameter("@id", c.IdCategoria),
                        new SqlParameter("@plataforma", Normalizar(c.Plataforma)),
                        new SqlParameter("@version", ValorONulo(c.VersionVideojuego)));
                    break;

                case TipoActivoCategoria.Publicidad:
                    Gestor.EjecutarNonQuery(
                        "INSERT INTO CategoriaPublicidadTable (id_categoria, formato_publicidad, canal_distribucion_publicidad) VALUES (@id, @formato, @canal)",
                        new SqlParameter("@id", c.IdCategoria),
                        new SqlParameter("@formato", ValorONulo(c.FormatoPublicidad)),
                        new SqlParameter("@canal", ValorONulo(c.CanalPublicidad)));
                    break;
            }
        }

        private void ActualizarEspecifico(Categoria_BE c)
        {
            switch (c.Tipo)
            {
                case TipoActivoCategoria.Software:
                    Gestor.EjecutarNonQuery(
                        "UPDATE CategoriaSoftwareTable SET sistema_operativo_software = @so, version_software = @version WHERE id_categoria = @id",
                        new SqlParameter("@so", ValorONulo(c.SistemaOperativoSoftware)),
                        new SqlParameter("@version", ValorONulo(c.VersionSoftware)),
                        new SqlParameter("@id", c.IdCategoria));
                    break;

                case TipoActivoCategoria.AppWeb:
                    Gestor.EjecutarNonQuery(
                        "UPDATE CategoriaAppWebTable SET url_appweb = @url, dispositivo_objetivo_appweb = @dispositivo WHERE id_categoria = @id",
                        new SqlParameter("@url", ValorONulo(c.UrlAppWeb)),
                        new SqlParameter("@dispositivo", Normalizar(c.DispositivoAppWeb)),
                        new SqlParameter("@id", c.IdCategoria));
                    break;

                case TipoActivoCategoria.AppMovil:
                    Gestor.EjecutarNonQuery(
                        "UPDATE CategoriaAppMovilTable SET sistema_operativo_appmovil = @so, version_appmovil = @version WHERE id_categoria = @id",
                        new SqlParameter("@so", Normalizar(c.SoAppMovil)),
                        new SqlParameter("@version", ValorONulo(c.VersionAppMovil)),
                        new SqlParameter("@id", c.IdCategoria));
                    break;

                case TipoActivoCategoria.Videojuego:
                    Gestor.EjecutarNonQuery(
                        "UPDATE CategoriaVideojuegoTable SET plataforma_videojuego = @plataforma, version_videojuego = @version WHERE id_categoria = @id",
                        new SqlParameter("@plataforma", Normalizar(c.Plataforma)),
                        new SqlParameter("@version", ValorONulo(c.VersionVideojuego)),
                        new SqlParameter("@id", c.IdCategoria));
                    break;

                case TipoActivoCategoria.Publicidad:
                    Gestor.EjecutarNonQuery(
                        "UPDATE CategoriaPublicidadTable SET formato_publicidad = @formato, canal_distribucion_publicidad = @canal WHERE id_categoria = @id",
                        new SqlParameter("@formato", ValorONulo(c.FormatoPublicidad)),
                        new SqlParameter("@canal", ValorONulo(c.CanalPublicidad)),
                        new SqlParameter("@id", c.IdCategoria));
                    break;
            }
        }

        private void EliminarEspecifico(int idCategoria, TipoActivoCategoria tipo)
        {
            Gestor.EjecutarNonQuery($"DELETE FROM {TablaEspecifica(tipo)} WHERE id_categoria = @id", new SqlParameter("@id", idCategoria));
        }

        private static string TablaEspecifica(TipoActivoCategoria tipo)
        {
            switch (tipo)
            {
                case TipoActivoCategoria.Software: return "CategoriaSoftwareTable";
                case TipoActivoCategoria.AppWeb: return "CategoriaAppWebTable";
                case TipoActivoCategoria.AppMovil: return "CategoriaAppMovilTable";
                case TipoActivoCategoria.Videojuego: return "CategoriaVideojuegoTable";
                default: return "CategoriaPublicidadTable";
            }
        }

        private TipoActivoCategoria ObtenerTipoActual(int idCategoria)
        {
            var dt = Gestor.EjecutarQuery("SELECT tipo_activo_digital_categoria FROM CategoriaTable WHERE id_categoria = @id", new SqlParameter("@id", idCategoria));

            if (dt.Rows.Count == 0) throw new InvalidOperationException("La categoría no existe.");

            return (TipoActivoCategoria)Enum.Parse(typeof(TipoActivoCategoria), dt.Rows[0]["tipo_activo_digital_categoria"].ToString(), true);
        }

        #endregion

        #region Mapping

        private static string Normalizar(Enum valor)
        {
            return valor.ToString().ToLowerInvariant();
        }

        private static string SqlSeleccionCompleta()
        {
            return @"
                SELECT c.*,
                       sw.sistema_operativo_software, sw.version_software,
                       aw.url_appweb, aw.dispositivo_objetivo_appweb,
                       am.sistema_operativo_appmovil, am.version_appmovil,
                       vj.plataforma_videojuego, vj.version_videojuego,
                       pb.formato_publicidad, pb.canal_distribucion_publicidad,
                       (SELECT COUNT(1) FROM SesionGrabadaTable s WHERE s.id_categoria = c.id_categoria) AS cant_sesiones
                FROM CategoriaTable c
                LEFT JOIN CategoriaSoftwareTable sw ON sw.id_categoria = c.id_categoria
                LEFT JOIN CategoriaAppWebTable aw ON aw.id_categoria = c.id_categoria
                LEFT JOIN CategoriaAppMovilTable am ON am.id_categoria = c.id_categoria
                LEFT JOIN CategoriaVideojuegoTable vj ON vj.id_categoria = c.id_categoria
                LEFT JOIN CategoriaPublicidadTable pb ON pb.id_categoria = c.id_categoria";
        }

        private static Categoria_BE Map(DataRow dr)
        {
            var tipo = Valor<TipoActivoCategoria>(dr, "tipo_activo_digital_categoria");

            var c = new Categoria_BE
            {
                IdCategoria = Valor<int>(dr, "id_categoria"),
                IdEmpresa = Valor<int>(dr, "id_empresa"),
                NombreCategoria = Valor<string>(dr, "nombre_categoria"),
                Tipo = tipo,
                NombreActivo = Valor<string>(dr, "nombre_activo_categoria"),
                FlujoEsperado = Valor<string>(dr, "flujo_analizado_categoria"),
                FechaCreacion = Valor<DateTime>(dr, "fecha_creacion_categoria"),
                Activa = Valor<bool>(dr, "activa_categoria"),
                DVH = Valor<string>(dr, "DVH"),
                CantidadSesiones = Valor<int>(dr, "cant_sesiones")
            };

            switch (tipo)
            {
                case TipoActivoCategoria.Software:
                    c.SistemaOperativoSoftware = Valor<string>(dr, "sistema_operativo_software");
                    c.VersionSoftware = Valor<string>(dr, "version_software");
                    break;

                case TipoActivoCategoria.AppWeb:
                    c.UrlAppWeb = Valor<string>(dr, "url_appweb");
                    c.DispositivoAppWeb = Valor<DispositivoObjetivo>(dr, "dispositivo_objetivo_appweb");
                    break;

                case TipoActivoCategoria.AppMovil:
                    c.SoAppMovil = Valor<SistemaOperativoMovil>(dr, "sistema_operativo_appmovil");
                    c.VersionAppMovil = Valor<string>(dr, "version_appmovil");
                    break;

                case TipoActivoCategoria.Videojuego:
                    c.Plataforma = Valor<PlataformaVideojuego>(dr, "plataforma_videojuego");
                    c.VersionVideojuego = Valor<string>(dr, "version_videojuego");
                    break;

                case TipoActivoCategoria.Publicidad:
                    c.FormatoPublicidad = Valor<string>(dr, "formato_publicidad");
                    c.CanalPublicidad = Valor<string>(dr, "canal_distribucion_publicidad");
                    break;
            }

            return c;
        }

        private static List<Categoria_BE> MapTodos(DataTable dt)
        {
            var lista = new List<Categoria_BE>();
            foreach (DataRow row in dt.Rows) lista.Add(Map(row));
            return lista;
        }

        #endregion
    }
}
