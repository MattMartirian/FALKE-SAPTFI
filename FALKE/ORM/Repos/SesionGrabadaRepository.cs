using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace ORM
{
    public class SesionGrabadaRepository : RepositoryBase<SesionGrabada_BE, int>
    {
        public SesionGrabadaRepository() : base() { }

        public override void Alta(SesionGrabada_BE s)
        {
            string sql = @"
                INSERT INTO SesionGrabadaTable
                    (id_categoria, id_usuario, fecha_sesion, estado_sesion,
                     nombre_sesion, objetivo_sesion,
                     rango_edad_tester, genero_tester, familiaridad_tester,
                     duracion_seg_sesion, muestras_sesion, bytes_video_sesion, desfase_mirada_ms_sesion)
                VALUES
                    (@idCategoria, @idUsuario, @fecha, @estado,
                     @nombre, @objetivo,
                     @edad, @genero, @familiaridad,
                     @duracion, @muestras, @bytes, @desfase);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            s.IdSesion = Gestor.EjecutarScalar<int>(sql,
                new SqlParameter("@idCategoria", s.IdCategoria),
                new SqlParameter("@idUsuario", s.IdUsuario),
                new SqlParameter("@fecha", s.Fecha),
                new SqlParameter("@estado", s.Estado),
                new SqlParameter("@nombre", ValorONulo(s.Nombre)),
                new SqlParameter("@objetivo", ValorONulo(s.Objetivo)),
                new SqlParameter("@edad", ValorONulo(s.RangoEdad)),
                new SqlParameter("@genero", ValorONulo(s.Genero)),
                new SqlParameter("@familiaridad", ValorONulo(s.Familiaridad)),
                new SqlParameter("@duracion", s.DuracionSegundos),
                new SqlParameter("@muestras", s.Muestras),
                new SqlParameter("@bytes", s.BytesVideo),
                new SqlParameter("@desfase", s.DesfaseMiradaMs));
        }

        public override void Modificar(SesionGrabada_BE s)
        {
            throw new NotSupportedException();
        }

        public override SesionGrabada_BE ObtenerPorPK(int pk)
        {
            var dt = Gestor.EjecutarQuery("SELECT * FROM SesionGrabadaTable WHERE id_sesion = @id", new SqlParameter("@id", pk));
            return dt.Rows.Count == 0 ? null : Map(dt.Rows[0]);
        }

        public override List<SesionGrabada_BE> ObtenerTodos()
        {
            var lista = new List<SesionGrabada_BE>();
            foreach (DataRow row in Gestor.EjecutarQuery("SELECT * FROM SesionGrabadaTable ORDER BY fecha_sesion DESC").Rows) lista.Add(Map(row));
            return lista;
        }

        public int ContarEncadenadas(int idCategoria, string nombreBase)
        {
            var dt = Gestor.EjecutarQuery(
                "SELECT COUNT(1) FROM SesionGrabadaTable WHERE id_categoria = @c AND (nombre_sesion = @n OR nombre_sesion LIKE @patron)",
                new SqlParameter("@c", idCategoria),
                new SqlParameter("@n", nombreBase),
                new SqlParameter("@patron", nombreBase.Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]") + " - %"));
            return Convert.ToInt32(dt.Rows[0][0]);
        }

        private static SesionGrabada_BE Map(DataRow dr)
        {
            return new SesionGrabada_BE
            {
                IdSesion = Valor<int>(dr, "id_sesion"),
                IdCategoria = Valor<int>(dr, "id_categoria"),
                IdUsuario = Valor<int>(dr, "id_usuario"),
                Fecha = Valor<DateTime>(dr, "fecha_sesion"),
                Estado = Valor<string>(dr, "estado_sesion"),
                Nombre = Valor<string>(dr, "nombre_sesion"),
                Objetivo = Valor<string>(dr, "objetivo_sesion"),
                RangoEdad = Valor<string>(dr, "rango_edad_tester"),
                Genero = Valor<string>(dr, "genero_tester"),
                Familiaridad = Valor<string>(dr, "familiaridad_tester"),
                DuracionSegundos = Valor<int>(dr, "duracion_seg_sesion"),
                Muestras = Valor<int>(dr, "muestras_sesion"),
                BytesVideo = Valor<long>(dr, "bytes_video_sesion"),
                DesfaseMiradaMs = Valor<int>(dr, "desfase_mirada_ms_sesion"),
                DVH = Valor<string>(dr, "DVH")
            };
        }
    }
}
