using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using DAL;

namespace ORM
{
    public class Integridad_ORM
    {
        private readonly GestorBaseDeDatos_DAL Gestor;

        public Integridad_ORM()
        {
            Gestor = GestorBaseDeDatos_DAL.Instancia;
        }

        private static string NombreTabla(TablasBD tabla) => $"{tabla}Table";

        public List<string> ObtenerNombresColumnas(TablasBD tabla) => ObtenerColumnas(NombreTabla(tabla));

        private List<string> ObtenerColumnas(string nombreTabla)
        {
            string sql = @"
                SELECT COLUMN_NAME
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME = @tabla AND COLUMN_NAME <> 'DVH'
                ORDER BY ORDINAL_POSITION";

            var dt = Gestor.EjecutarQuery(sql, new SqlParameter("@tabla", nombreTabla));
            var columnas = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                columnas.Add(row["COLUMN_NAME"].ToString());
            }

            return columnas;
        }

        private List<string> ObtenerColumnasPrimaria(string nombreTabla)
        {
            string sql = @"
                SELECT KU.COLUMN_NAME
                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC
                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KU
                    ON TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME
                    AND TC.TABLE_NAME = KU.TABLE_NAME
                WHERE TC.CONSTRAINT_TYPE = 'PRIMARY KEY'
                    AND KU.TABLE_NAME = @tabla
                ORDER BY KU.ORDINAL_POSITION";

            var dt = Gestor.EjecutarQuery(sql, new SqlParameter("@tabla", nombreTabla));
            var columnas = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                columnas.Add(row["COLUMN_NAME"].ToString());
            }

            return columnas;
        }

        public List<string> ObtenerDVHs(TablasBD tabla)
        {
            string nombreTabla = NombreTabla(tabla);
            string sql = $"SELECT DVH FROM {nombreTabla} WHERE DVH IS NOT NULL";

            var dt = Gestor.EjecutarQuery(sql);
            var dvhs = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                dvhs.Add(row["DVH"].ToString());
            }

            return dvhs;
        }
        
                
        public List<(string[] Datos, string Dvh, string[] ClavePK)> ObtenerDatosTabla(TablasBD tabla)
        {
            string nombreTabla = NombreTabla(tabla);
            var columnasPK = ObtenerColumnasPrimaria(nombreTabla);
            var columnas = ObtenerColumnas(nombreTabla);

            var indicesPK = columnasPK.ConvertAll(pk => columnas.IndexOf(pk));

            string selectCols = string.Join(", ", columnas) + ", DVH";
            string sql = $"SELECT {selectCols} FROM {nombreTabla}";

            var dt = Gestor.EjecutarQuery(sql);
            var resultado = new List<(string[], string, string[])>();

            foreach (DataRow row in dt.Rows)
            {
                var datos = new string[columnas.Count];

                for (int i = 0; i < columnas.Count; i++)
                {
                    datos[i] = FormatearValor(row[i]);
                }

                string dvh = FormatearValor(row[columnas.Count]);
                var clave = indicesPK.ConvertAll(idx => idx >= 0 ? datos[idx] : string.Empty).ToArray();

                resultado.Add((datos, dvh, clave));
            }

            return resultado;
        }

        public string[] ObtenerDatosRegistro(TablasBD tabla, string[] valoresClave)
        {
            string nombreTabla = NombreTabla(tabla);
            var columnasPK = ObtenerColumnasPrimaria(nombreTabla);
            var columnas = ObtenerColumnas(nombreTabla);

            if (columnasPK.Count != valoresClave.Length)
            {
                //TODO: Traducir.
                throw new ArgumentException($"La tabla {nombreTabla} tiene {columnasPK.Count} columna(s) de PK, se pasaron {valoresClave.Length} valores.");
            }

            var condiciones = new List<string>();
            var parametros = new List<SqlParameter>();

            for (int i = 0; i < columnasPK.Count; i++)
            {
                string nombreParam = $"@pk{i}";
                condiciones.Add($"{columnasPK[i]} = {nombreParam}");
                parametros.Add(new SqlParameter(nombreParam, valoresClave[i]));
            }

            string sql = $"SELECT {string.Join(", ", columnas)} FROM {nombreTabla} WHERE {string.Join(" AND ", condiciones)}";
            var dt = Gestor.EjecutarQuery(sql, parametros.ToArray());

            if (dt.Rows.Count == 0) return null;

            var datos = new string[columnas.Count];
            for (int i = 0; i < columnas.Count; i++)
            {
                datos[i] = FormatearValor(dt.Rows[0][i]);
            }

            return datos;
        }

        // Formatea el valor de una celda de forma independiente de la cultura del servidor,
        // para que el DVH/DVV sea el mismo sin importar el locale donde corra la aplicacion
        // (una fecha o un decimal cambian de texto segun la cultura y romperian la firma).
        private static string FormatearValor(object valor)
        {
            if (valor == null || valor == DBNull.Value) return string.Empty;

            var formateable = valor as IFormattable;
            if (formateable != null) return formateable.ToString(null, CultureInfo.InvariantCulture);

            return valor.ToString();
        }


        public void GuardarNuevoDVH(TablasBD tabla, string[] valoresClave, string dvh)
        {
            string nombreTabla = NombreTabla(tabla);
            var columnasPK = ObtenerColumnasPrimaria(nombreTabla);

            if (columnasPK.Count == 0)
            {
                //TODO: Traducir.
                throw new InvalidOperationException($"No se encontró clave primaria para la tabla {nombreTabla}.");
            }

            if (columnasPK.Count != valoresClave.Length)
            {
                //TODO: Traducir.
                throw new ArgumentException($"La tabla {nombreTabla} tiene {columnasPK.Count} columna(s) de PK, se pasaron {valoresClave.Length} valores.");
            }

            var condiciones = new List<string>();
            var parametros = new List<SqlParameter> { new SqlParameter("@dvh", dvh) };

            for (int i = 0; i < columnasPK.Count; i++)
            {
                string nombreParam = $"@pk{i}";
                condiciones.Add($"{columnasPK[i]} = {nombreParam}");
                parametros.Add(new SqlParameter(nombreParam, valoresClave[i]));
            }

            string sql = $"UPDATE {nombreTabla} SET DVH = @dvh WHERE {string.Join(" AND ", condiciones)}";
            Gestor.EjecutarNonQuery(sql, parametros.ToArray());
        }

        public void GuardarRegistroIntegridad(TablasBD tabla, string dvv, int cantidadRegistros)
        {
            string sql = @"
        MERGE INTO IntegridadTable AS destino
        USING (SELECT @TablaIntegridad AS tabla_integridad) AS origen
        ON (destino.tabla_integridad = origen.tabla_integridad)
        WHEN MATCHED THEN
            UPDATE SET dvv_integridad = @DVV, cantidad_registros_integridad = @CR
        WHEN NOT MATCHED THEN
            INSERT (tabla_integridad, dvv_integridad, cantidad_registros_integridad)
            VALUES (@TablaIntegridad, @DVV, @CR);";

            Gestor.EjecutarNonQuery(sql,
                new SqlParameter("@TablaIntegridad", tabla.ToString()),
                new SqlParameter("@DVV", dvv),
                new SqlParameter("@CR", cantidadRegistros)
            );
        }

        public (string DVV, int CR)? LeerRegistroIntegridad(TablasBD tabla)
        {
            string sql = @"
        SELECT dvv_integridad, cantidad_registros_integridad
        FROM IntegridadTable
        WHERE tabla_integridad = @TablaIntegridad";

            var dt = Gestor.EjecutarQuery(sql, new SqlParameter("@TablaIntegridad", tabla.ToString()));

            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            if (row["dvv_integridad"] == DBNull.Value || row["cantidad_registros_integridad"] == DBNull.Value) return null;

            return (row["dvv_integridad"].ToString(), Convert.ToInt32(row["cantidad_registros_integridad"]));
        }
    }
}