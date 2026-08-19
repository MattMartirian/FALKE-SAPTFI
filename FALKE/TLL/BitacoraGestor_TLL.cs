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
        private readonly BitacoraRepository _repository;

        public BitacoraGestor_TLL()
        {
            _repository = new BitacoraRepository();
        }

        public void Guardar(Bitacora_TE bitacora)
        {
            _repository.Alta(bitacora);
        }

        public List<Bitacora_TE> ObtenerTodas()
        {
            return _repository.ObtenerTodos();
        }
    }
}
