using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BE;
using DAL;

namespace ORM
{
    public class CategoriaRepository : RepositoryBase<Categoria_BE, int>
    {
        private static readonly Dictionary<TipoActivoCategoria, Mapeo> Mapeos = new Mapeo[]
        {
            new MapeoSoftware(),
            new MapeoAppWeb(),
            new MapeoAppMovil(),
            new MapeoVideojuego(),
            new MapeoPublicidad()
        }.ToDictionary(m => m.Tipo);

        public CategoriaRepository() : base() { }

        public TablasBD TablaEspecifica(TipoActivoCategoria tipo)
        {
            return Mapeos[tipo].TablaIntegridad;
        }

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

            c.IdCategoria = Gestor.EjecutarScalar<int>(sql,
                new SqlParameter("@idEmpresa", c.IdEmpresa),
                new SqlParameter("@nombre", c.NombreCategoria),
                new SqlParameter("@tipo", Normalizar(c.Tipo)),
                new SqlParameter("@nombreActivo", ValorONulo(c.NombreActivo)),
                new SqlParameter("@flujo", ValorONulo(c.FlujoEsperado)),
                new SqlParameter("@fechaCreacion", c.FechaCreacion),
                new SqlParameter("@activa", c.Activa));

            Mapeos[c.Tipo].Insertar(c);
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

            if (tipoAnterior != c.Tipo)
            {
                Mapeos[tipoAnterior].Eliminar(c.IdCategoria);
                Mapeos[c.Tipo].Insertar(c);
            }
            else
            {
                Mapeos[c.Tipo].Actualizar(c);
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

        public int ContarSesiones(int idCategoria)
        {
            string sql = "SELECT COUNT(1) FROM SesionGrabadaTable WHERE id_categoria = @id";
            var dt = Gestor.EjecutarQuery(sql, new SqlParameter("@id", idCategoria));
            return Convert.ToInt32(dt.Rows[0][0]);
        }

        public void Eliminar(int idCategoria)
        {
            foreach (Mapeo mapeo in Mapeos.Values) mapeo.Eliminar(idCategoria);

            Gestor.EjecutarNonQuery("DELETE FROM CategoriaTable WHERE id_categoria = @id", new SqlParameter("@id", idCategoria));
        }

        private TipoActivoCategoria ObtenerTipoActual(int idCategoria)
        {
            var dt = Gestor.EjecutarQuery("SELECT tipo_activo_digital_categoria FROM CategoriaTable WHERE id_categoria = @id", new SqlParameter("@id", idCategoria));

            if (dt.Rows.Count == 0) throw new InvalidOperationException("La categoría no existe.");

            return (TipoActivoCategoria)Enum.Parse(typeof(TipoActivoCategoria), dt.Rows[0]["tipo_activo_digital_categoria"].ToString(), true);
        }

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
            Categoria_BE c = Mapeos[Valor<TipoActivoCategoria>(dr, "tipo_activo_digital_categoria")].Leer(dr);

            c.IdCategoria = Valor<int>(dr, "id_categoria");
            c.IdEmpresa = Valor<int>(dr, "id_empresa");
            c.NombreCategoria = Valor<string>(dr, "nombre_categoria");
            c.NombreActivo = Valor<string>(dr, "nombre_activo_categoria");
            c.FlujoEsperado = Valor<string>(dr, "flujo_analizado_categoria");
            c.FechaCreacion = Valor<DateTime>(dr, "fecha_creacion_categoria");
            c.Activa = Valor<bool>(dr, "activa_categoria");
            c.DVH = Valor<string>(dr, "DVH");
            c.CantidadSesiones = Valor<int>(dr, "cant_sesiones");

            return c;
        }

        private static List<Categoria_BE> MapTodos(DataTable dt)
        {
            var lista = new List<Categoria_BE>();
            foreach (DataRow row in dt.Rows) lista.Add(Map(row));
            return lista;
        }

        private abstract class Mapeo
        {
            protected static GestorBaseDeDatos_DAL Gestor
            {
                get { return GestorBaseDeDatos_DAL.Instancia; }
            }

            public abstract TipoActivoCategoria Tipo { get; }
            public abstract string Tabla { get; }
            public abstract TablasBD TablaIntegridad { get; }
            public abstract void Insertar(Categoria_BE c);
            public abstract void Actualizar(Categoria_BE c);
            public abstract Categoria_BE Leer(DataRow dr);

            public void Eliminar(int idCategoria)
            {
                Gestor.EjecutarNonQuery("DELETE FROM " + Tabla + " WHERE id_categoria = @id", new SqlParameter("@id", idCategoria));
            }
        }

        private sealed class MapeoSoftware : Mapeo
        {
            public override TipoActivoCategoria Tipo { get { return TipoActivoCategoria.Software; } }
            public override string Tabla { get { return "CategoriaSoftwareTable"; } }
            public override TablasBD TablaIntegridad { get { return TablasBD.CategoriaSoftware; } }

            public override void Insertar(Categoria_BE c)
            {
                var s = (CategoriaSoftware_BE)c;

                Gestor.EjecutarNonQuery(
                    "INSERT INTO CategoriaSoftwareTable (id_categoria, sistema_operativo_software, version_software) VALUES (@id, @so, @version)",
                    new SqlParameter("@id", s.IdCategoria),
                    new SqlParameter("@so", ValorONulo(s.SistemaOperativoSoftware)),
                    new SqlParameter("@version", ValorONulo(s.VersionSoftware)));
            }

            public override void Actualizar(Categoria_BE c)
            {
                var s = (CategoriaSoftware_BE)c;

                Gestor.EjecutarNonQuery(
                    "UPDATE CategoriaSoftwareTable SET sistema_operativo_software = @so, version_software = @version WHERE id_categoria = @id",
                    new SqlParameter("@so", ValorONulo(s.SistemaOperativoSoftware)),
                    new SqlParameter("@version", ValorONulo(s.VersionSoftware)),
                    new SqlParameter("@id", s.IdCategoria));
            }

            public override Categoria_BE Leer(DataRow dr)
            {
                return new CategoriaSoftware_BE
                {
                    SistemaOperativoSoftware = Valor<string>(dr, "sistema_operativo_software"),
                    VersionSoftware = Valor<string>(dr, "version_software")
                };
            }
        }

        private sealed class MapeoAppWeb : Mapeo
        {
            public override TipoActivoCategoria Tipo { get { return TipoActivoCategoria.AppWeb; } }
            public override string Tabla { get { return "CategoriaAppWebTable"; } }
            public override TablasBD TablaIntegridad { get { return TablasBD.CategoriaAppWeb; } }

            public override void Insertar(Categoria_BE c)
            {
                var w = (CategoriaAppWeb_BE)c;

                Gestor.EjecutarNonQuery(
                    "INSERT INTO CategoriaAppWebTable (id_categoria, url_appweb, dispositivo_objetivo_appweb) VALUES (@id, @url, @dispositivo)",
                    new SqlParameter("@id", w.IdCategoria),
                    new SqlParameter("@url", ValorONulo(w.UrlAppWeb)),
                    new SqlParameter("@dispositivo", Normalizar(w.DispositivoAppWeb)));
            }

            public override void Actualizar(Categoria_BE c)
            {
                var w = (CategoriaAppWeb_BE)c;

                Gestor.EjecutarNonQuery(
                    "UPDATE CategoriaAppWebTable SET url_appweb = @url, dispositivo_objetivo_appweb = @dispositivo WHERE id_categoria = @id",
                    new SqlParameter("@url", ValorONulo(w.UrlAppWeb)),
                    new SqlParameter("@dispositivo", Normalizar(w.DispositivoAppWeb)),
                    new SqlParameter("@id", w.IdCategoria));
            }

            public override Categoria_BE Leer(DataRow dr)
            {
                return new CategoriaAppWeb_BE
                {
                    UrlAppWeb = Valor<string>(dr, "url_appweb"),
                    DispositivoAppWeb = Valor<DispositivoObjetivo>(dr, "dispositivo_objetivo_appweb")
                };
            }
        }

        private sealed class MapeoAppMovil : Mapeo
        {
            public override TipoActivoCategoria Tipo { get { return TipoActivoCategoria.AppMovil; } }
            public override string Tabla { get { return "CategoriaAppMovilTable"; } }
            public override TablasBD TablaIntegridad { get { return TablasBD.CategoriaAppMovil; } }

            public override void Insertar(Categoria_BE c)
            {
                var m = (CategoriaAppMovil_BE)c;

                Gestor.EjecutarNonQuery(
                    "INSERT INTO CategoriaAppMovilTable (id_categoria, sistema_operativo_appmovil, version_appmovil) VALUES (@id, @so, @version)",
                    new SqlParameter("@id", m.IdCategoria),
                    new SqlParameter("@so", Normalizar(m.SoAppMovil)),
                    new SqlParameter("@version", ValorONulo(m.VersionAppMovil)));
            }

            public override void Actualizar(Categoria_BE c)
            {
                var m = (CategoriaAppMovil_BE)c;

                Gestor.EjecutarNonQuery(
                    "UPDATE CategoriaAppMovilTable SET sistema_operativo_appmovil = @so, version_appmovil = @version WHERE id_categoria = @id",
                    new SqlParameter("@so", Normalizar(m.SoAppMovil)),
                    new SqlParameter("@version", ValorONulo(m.VersionAppMovil)),
                    new SqlParameter("@id", m.IdCategoria));
            }

            public override Categoria_BE Leer(DataRow dr)
            {
                return new CategoriaAppMovil_BE
                {
                    SoAppMovil = Valor<SistemaOperativoMovil>(dr, "sistema_operativo_appmovil"),
                    VersionAppMovil = Valor<string>(dr, "version_appmovil")
                };
            }
        }

        private sealed class MapeoVideojuego : Mapeo
        {
            public override TipoActivoCategoria Tipo { get { return TipoActivoCategoria.Videojuego; } }
            public override string Tabla { get { return "CategoriaVideojuegoTable"; } }
            public override TablasBD TablaIntegridad { get { return TablasBD.CategoriaVideojuego; } }

            public override void Insertar(Categoria_BE c)
            {
                var v = (CategoriaVideojuego_BE)c;

                Gestor.EjecutarNonQuery(
                    "INSERT INTO CategoriaVideojuegoTable (id_categoria, plataforma_videojuego, version_videojuego) VALUES (@id, @plataforma, @version)",
                    new SqlParameter("@id", v.IdCategoria),
                    new SqlParameter("@plataforma", Normalizar(v.Plataforma)),
                    new SqlParameter("@version", ValorONulo(v.VersionVideojuego)));
            }

            public override void Actualizar(Categoria_BE c)
            {
                var v = (CategoriaVideojuego_BE)c;

                Gestor.EjecutarNonQuery(
                    "UPDATE CategoriaVideojuegoTable SET plataforma_videojuego = @plataforma, version_videojuego = @version WHERE id_categoria = @id",
                    new SqlParameter("@plataforma", Normalizar(v.Plataforma)),
                    new SqlParameter("@version", ValorONulo(v.VersionVideojuego)),
                    new SqlParameter("@id", v.IdCategoria));
            }

            public override Categoria_BE Leer(DataRow dr)
            {
                return new CategoriaVideojuego_BE
                {
                    Plataforma = Valor<PlataformaVideojuego>(dr, "plataforma_videojuego"),
                    VersionVideojuego = Valor<string>(dr, "version_videojuego")
                };
            }
        }

        private sealed class MapeoPublicidad : Mapeo
        {
            public override TipoActivoCategoria Tipo { get { return TipoActivoCategoria.Publicidad; } }
            public override string Tabla { get { return "CategoriaPublicidadTable"; } }
            public override TablasBD TablaIntegridad { get { return TablasBD.CategoriaPublicidad; } }

            public override void Insertar(Categoria_BE c)
            {
                var p = (CategoriaPublicidad_BE)c;

                Gestor.EjecutarNonQuery(
                    "INSERT INTO CategoriaPublicidadTable (id_categoria, formato_publicidad, canal_distribucion_publicidad) VALUES (@id, @formato, @canal)",
                    new SqlParameter("@id", p.IdCategoria),
                    new SqlParameter("@formato", ValorONulo(p.FormatoPublicidad)),
                    new SqlParameter("@canal", ValorONulo(p.CanalPublicidad)));
            }

            public override void Actualizar(Categoria_BE c)
            {
                var p = (CategoriaPublicidad_BE)c;

                Gestor.EjecutarNonQuery(
                    "UPDATE CategoriaPublicidadTable SET formato_publicidad = @formato, canal_distribucion_publicidad = @canal WHERE id_categoria = @id",
                    new SqlParameter("@formato", ValorONulo(p.FormatoPublicidad)),
                    new SqlParameter("@canal", ValorONulo(p.CanalPublicidad)),
                    new SqlParameter("@id", p.IdCategoria));
            }

            public override Categoria_BE Leer(DataRow dr)
            {
                return new CategoriaPublicidad_BE
                {
                    FormatoPublicidad = Valor<string>(dr, "formato_publicidad"),
                    CanalPublicidad = Valor<string>(dr, "canal_distribucion_publicidad")
                };
            }
        }
    }
}
