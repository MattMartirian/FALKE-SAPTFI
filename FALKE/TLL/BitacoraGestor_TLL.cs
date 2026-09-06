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
