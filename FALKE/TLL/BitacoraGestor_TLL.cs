using ORM;
using TE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public List<Bitacora_TE> ObtenerTodas()
        {
            return bitacoraRepo.ObtenerTodos();
        }
    }
}
