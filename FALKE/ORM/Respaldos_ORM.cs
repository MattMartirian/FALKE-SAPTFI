using System;
using System.Data.SqlClient;
using System.IO;
using DAL;

namespace ORM
{
    // Lo que se le pide al servidor de base de datos: copiar, verificar y restaurar la base. Los archivos quedan en el servidor.
    public class Respaldos_ORM
    {
        private readonly GestorBaseDeDatos_DAL Gestor;

        public Respaldos_ORM()
        {
            Gestor = GestorBaseDeDatos_DAL.Instancia;
        }

        public string NombreBase
        {
            get { return Gestor.NombreBaseDeDatos; }
        }

        // La carpeta de copias predeterminada de la instancia: el servicio de SQL Server siempre puede escribir ahí.
        public string CarpetaDeCopias()
        {
            var dt = Gestor.EjecutarQueryEnMaster("SELECT CAST(SERVERPROPERTY('InstanceDefaultBackupPath') AS nvarchar(260))");
            string carpeta = dt.Rows.Count == 0 || dt.Rows[0][0] == DBNull.Value ? null : Convert.ToString(dt.Rows[0][0]);

            if (string.IsNullOrWhiteSpace(carpeta)) throw new InvalidOperationException("El servidor de base de datos no informó su carpeta de copias de seguridad.");

            return carpeta;
        }

        // Con numero > 1 se agrega un sufijo: dos copias en el mismo segundo no pueden pisarse el archivo.
        public string RutaNueva(DateTime fecha, int numero = 1)
        {
            return Path.Combine(CarpetaDeCopias(), NombreBase + "_" + fecha.ToString("yyyyMMdd_HHmmss") + (numero > 1 ? "_" + numero : string.Empty) + ".bak");
        }

        // Copia completa con suma de comprobación y la verifica enseguida. Devuelve el tamaño del archivo, si el servidor lo informa.
        public long? Respaldar(string ruta)
        {
            Gestor.EjecutarNonQueryEnMaster("BACKUP DATABASE " + Entre(NombreBase) + " TO DISK = @ruta WITH INIT, CHECKSUM", 0,
                new SqlParameter("@ruta", ruta));

            Verificar(ruta);

            try
            {
                var dt = Gestor.EjecutarQueryEnMaster(@"
                    SELECT TOP 1 bs.backup_size
                    FROM msdb.dbo.backupset bs
                    JOIN msdb.dbo.backupmediafamily mf ON mf.media_set_id = bs.media_set_id
                    WHERE mf.physical_device_name = @ruta
                    ORDER BY bs.backup_finish_date DESC",
                    new SqlParameter("@ruta", ruta));

                return dt.Rows.Count == 0 || dt.Rows[0][0] == DBNull.Value ? (long?)null : Convert.ToInt64(dt.Rows[0][0]);
            }
            catch
            {
                return null;
            }
        }

        // Falla si el archivo no está o está dañado.
        public void Verificar(string ruta)
        {
            Gestor.EjecutarNonQueryEnMaster("RESTORE VERIFYONLY FROM DISK = @ruta WITH CHECKSUM", 0, new SqlParameter("@ruta", ruta));
        }

        public bool ArchivoExiste(string ruta)
        {
            try
            {
                var dt = Gestor.EjecutarQueryEnMaster("SELECT file_exists FROM sys.dm_os_file_exists(@ruta)", new SqlParameter("@ruta", ruta));

                return dt.Rows.Count > 0 && Convert.ToInt32(dt.Rows[0][0]) == 1;
            }
            catch
            {
                return false;
            }
        }

        // Reemplaza toda la base por la de la copia. Desconecta a cualquiera que la esté usando. Si algo falla, la base vuelve a quedar disponible.
        public void Restaurar(string ruta)
        {
            string bd = Entre(NombreBase);

            string sql = @"
                BEGIN TRY
                    ALTER DATABASE " + bd + @" SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    RESTORE DATABASE " + bd + @" FROM DISK = @ruta WITH REPLACE, RECOVERY;
                    ALTER DATABASE " + bd + @" SET MULTI_USER;
                END TRY
                BEGIN CATCH
                    DECLARE @mensaje nvarchar(2000) = ERROR_MESSAGE();
                    BEGIN TRY
                        ALTER DATABASE " + bd + @" SET MULTI_USER;
                    END TRY
                    BEGIN CATCH
                    END CATCH;
                    RAISERROR(@mensaje, 16, 1);
                END CATCH;";

            try
            {
                Gestor.EjecutarNonQueryEnMaster(sql, 0, new SqlParameter("@ruta", ruta));
            }
            finally
            {
                Gestor.LimpiarConexiones();
            }
        }

        private static string Entre(string nombre)
        {
            return "[" + nombre.Replace("]", "]]") + "]";
        }
    }
}
