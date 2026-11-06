using System;
using DAL;

namespace ORM
{

    public static class Transaccion_ORM
    {
        public static void Iniciar()
        {
            GestorBaseDeDatos_DAL.Instancia.IniciarTransaccion();
        }

        public static void Confirmar()
        {
            GestorBaseDeDatos_DAL.Instancia.Confirmar();
        }

        public static void Revertir()
        {
            GestorBaseDeDatos_DAL.Instancia.Revertir();
        }

        public static void Ejecutar(Action pasos)
        {
            if (pasos == null) throw new ArgumentNullException(nameof(pasos));

            Iniciar();

            try
            {
                pasos();
                Confirmar();
            }
            catch
            {
                Revertir();
                throw;
            }
        }


        public static T Ejecutar<T>(Func<T> pasos)
        {
            if (pasos == null) throw new ArgumentNullException(nameof(pasos));

            Iniciar();

            try
            {
                T resultado = pasos();
                Confirmar();
                return resultado;
            }
            catch
            {
                Revertir();
                throw;
            }
        }
    }
}
