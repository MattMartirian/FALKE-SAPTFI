using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace ORM
{
    public class TestORM
    {
        public void testear()
        {
            GestorBaseDeDatos.Instancia.EjecutarQuery(
                "INSERT INTO [FalkeDB].[dbo].[BitacoraTable] " +
                "([id_usuario], [modulo_bitacora], [descripcion_bitacora], [criticidad_bitacora], [fecha_hora_bitacora]) " +
                "VALUES (1, 'Testeo', 'Registro de prueba desde testear()', 1, GETDATE())"
            );
        }
    }
}
