using System;
using DAL;

namespace ORM
{
    /// <summary>
    /// Punto unico desde el que las capas superiores (BLL / TLL) delimitan una transaccion.
    /// La transaccion en si la maneja, centralizada, GestorBaseDeDatos_DAL; aca solo se le
    /// avisa cuando abrir, confirmar o revertir. Las capas de arriba no tocan DAL.
    /// </summary>
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

        /// <summary>
        /// Ejecuta los pasos dentro de una transaccion: si terminan sin excepcion se confirma,
        /// si tiran se revierte y se relanza la excepcion.
        /// </summary>
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

        /// <summary>Igual que Ejecutar(Action) pero devolviendo un resultado.</summary>
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
