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

        internal RepositoryBase(GestorBaseDeDatos gestor)
        {
            Gestor = gestor ?? GestorBaseDeDatos.Instancia;
        }

        public abstract void Alta(TEntity entidad);
        public abstract void Modificar(TEntity entidad);
        public abstract TEntity ObtenerPorPK(TKey pk);
        public abstract List<TEntity> ObtenerTodos();

        #region Helpers

        /// <summary>
        /// Devuelve el valor de una columna del DataRow y lo convierte al tipo especificado.
        /// Si el valor es DBNull, devuelve null para tipos nullable o el valor predeterminado para tipos no nullable.
        /// </summary>
        protected static T Valor<T>(DataRow dr, string columna)
        {
            var val = dr[columna];

            if (val == DBNull.Value)
                return default;

            var tipoNullable = Nullable.GetUnderlyingType(typeof(T));

            if (tipoNullable != null)
                return (T)Convert.ChangeType(val, tipoNullable);

            return (T)val;
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