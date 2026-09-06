using System;
using System.Collections.Generic;
using BE;
using ORM;
using SERVICES;
using TE;
using TLL;

namespace BLL
{
    public class Empresa_BLL
    {
        private readonly EmpresaRepository empresaRepo;
        private readonly GestorIntegridad_SERVICE gestorIntegridad;
        private readonly BitacoraGestor_TLL bitacora;

        public Empresa_BLL()
        {
            empresaRepo = new EmpresaRepository();
            gestorIntegridad = new GestorIntegridad_SERVICE();
            bitacora = new BitacoraGestor_TLL();
        }

        public string RegistrarEmpresa(Empresa_BE empresa, Usuario_TE adminInicial)
        {
            if (empresa == null) throw new ArgumentNullException(nameof(empresa));

            if (adminInicial == null) throw new ArgumentNullException(nameof(adminInicial));

            //TODO: Traducir.
            if (string.IsNullOrWhiteSpace(empresa.NombreEmpresa)) throw new InvalidOperationException("El nombre de la empresa es obligatorio.");

            //TODO: Traducir.
            if (empresaRepo.ExisteNombre(empresa.NombreEmpresa)) throw new InvalidOperationException("Ya existe una empresa registrada con ese nombre.");

            adminInicial.Rol = adminInicial.Rol ?? new PermisoCompuesto_TE(Usuario_TLL.ROL_ADMINISTRADOR, true);

            // Regla de negocio: una empresa nueva nace activa.
            empresa.Estado = EstadoEmpresa.Activa;

            string token = Transaccion_ORM.Ejecutar(() =>
            {
                empresaRepo.Alta(empresa);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.EmpresaCliente, new[] { empresa.IdEmpresa.ToString() });

                adminInicial.IdEmpresa = empresa.IdEmpresa;

                string t = new Usuario_TLL().RegistrarUsuario(adminInicial);

                //TODO: Traducir.
                bitacora.Registrar(adminInicial.IdUsuario, "Empresas", "Alta de empresa \"" + empresa.NombreEmpresa + "\" (id " + empresa.IdEmpresa + ") con usuario administrador \"" + adminInicial.EmailUsuario + "\"", CriticidadBitacora.Media);

                return t;
            });

            return token;
        }

        public List<Empresa_BE> ObtenerTodas() => empresaRepo.ObtenerTodos();

        public Empresa_BE ObtenerPorId(int idEmpresa) => empresaRepo.ObtenerPorPK(idEmpresa);
    }
}
