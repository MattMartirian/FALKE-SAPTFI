using DAL;
using System;
using System.Collections.Generic;
using System.Data;

namespace ORM
{
    public abstract class RepositoryBase<TEntity, TKey>
    {
        internal readonly GestorBaseDeDatos Gestor;

        protected RepositoryBase()
        {
            Gestor = GestorBaseDeDatos.Instancia;
        }

        public abstract void Alta(TEntity entidad);
        public abstract void Modificar(TEntity entidad);
        public abstract TEntity ObtenerPorPK(TKey pk);
        public abstract List<TEntity> ObtenerTodos();

        #region Helpers

        /// <summary>
        /// Devuelve el valor de una columna del DataRow y lo convierte al tipo especificado.
        /// </summary>
        protected static T Valor<T>(DataRow dr, string columna)
        {
            var val = dr[columna];

            if (val == DBNull.Value) return default;

            var tipoDestino = typeof(T);
            var tipoNullable = Nullable.GetUnderlyingType(tipoDestino);
            var tipoReal = tipoNullable ?? tipoDestino;

            if (tipoReal.IsEnum) return (T)Enum.ToObject(tipoReal, val);

            return (T)Convert.ChangeType(val, tipoReal);
        }

        /// <summary>
        /// Convierte un valor null de C# en DBNull.Value para su uso como parámetro SQL.
        /// </summary>
        protected static object ValorONulo(object valor)
        {
            return valor ?? DBNull.Value;
        }

        #endregion
    }
}