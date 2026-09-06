using ORM;
using TE;
using System;
using System.Collections.Generic;

namespace TLL
{
    public class BitacoraGestor_TLL
    {
        private readonly BitacoraRepository bitacoraRepo;

        public BitacoraGestor_TLL()
        {
            bitacoraRepo = new BitacoraRepository();
        }

        public void Guardar(Bitacora_TE bitacora)
        {
            bitacoraRepo.Alta(bitacora);
        }

        /// <summary>
        /// Registra un evento de auditoria. Un fallo al escribir la bitacora nunca interrumpe
        /// la operacion auditada (se traga la excepcion, igual que el log de errores).
        /// idUsuario &lt;= 0 se guarda como "sin usuario" (eventos de sistema / sin actor conocido).
        /// </summary>
        public void Registrar(int idUsuario, string modulo, string descripcion, CriticidadBitacora criticidad)
        {
            try
            {
                Guardar(new Bitacora_TE(idUsuario, modulo, descripcion, criticidad, DateTime.Now));
            }
            catch
            {
                // intencional: la auditoria no debe romper el flujo
            }
        }

        public List<Bitacora_TE> ObtenerTodas()
        {
            return bitacoraRepo.ObtenerTodos();
        }
    }
}
