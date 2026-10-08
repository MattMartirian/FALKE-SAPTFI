using System;
using System.Collections.Generic;
using System.Linq;
using BE;
using ORM;
using SERVICES;
using TE;
using TLL;
using static TLL.TextoHelper_TLL;

namespace BLL
{
    public class Empresa_BLL
    {
        private const int LARGO_NOMBRE = 150;
        private const int LARGO_CONTACTO = 50;
        private const int LARGO_RUBRO = 100;
        private const int LARGO_DOMICILIO = 200;

        private readonly EmpresaRepository empresaRepo;
        private readonly GestorIntegridad_SERVICE gestorIntegridad;
        private readonly BitacoraGestor_TLL bitacora;

        public Empresa_BLL()
        {
            empresaRepo = new EmpresaRepository();
            gestorIntegridad = new GestorIntegridad_SERVICE();
            bitacora = new BitacoraGestor_TLL();
        }
        public string RegistrarEmpresa(ActorUsuario_TE actor, Empresa_BE empresa, Usuario_TE adminInicial)
        {
            if (actor == null || !actor.Puede(Patentes_TLL.REGISTRAR_EMPRESA))
            {
                bitacora.Registrar(actor != null ? actor.IdUsuario : 0, "Seguridad", "Acción rechazada por falta de permiso (" + Patentes_TLL.REGISTRAR_EMPRESA + ")", CriticidadBitacora.Media);
                throw new UnauthorizedAccessException("No tenés permiso para registrar empresas.");
            }

            if (empresa == null) throw new ArgumentNullException(nameof(empresa));

            if (adminInicial == null) throw new ArgumentNullException(nameof(adminInicial));

            ValidarYNormalizar(empresa);

            empresa.FechaAlta = DateTime.Now;
            empresa.Estado = EstadoEmpresa.Activa;
            empresa.FechaRenovacion = empresa.Facturacion == CicloFacturacion.Anual ? empresa.FechaAlta.AddYears(1) : empresa.FechaAlta.AddMonths(1);

            adminInicial.Rol = new PermisoCompuesto_TE(Usuario_TLL.ROL_ADMINISTRADOR, true);
            adminInicial.EsCuentaEmergencia = false;

            string token = Transaccion_ORM.Ejecutar(() =>
            {
                empresaRepo.Alta(empresa);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.EmpresaCliente, new[] { empresa.IdEmpresa.ToString() });

                adminInicial.IdEmpresa = empresa.IdEmpresa;

                string t = new Usuario_TLL().RegistrarUsuario(actor, adminInicial);

                bitacora.Guardar(new Bitacora_TE(actor.IdUsuario, "Empresas",
                    "Alta de empresa \"" + empresa.NombreEmpresa + "\" (CUIT " + empresa.Cuit + ", plan " + empresa.PlanSuscripcion + ") con usuario administrador \"" + adminInicial.EmailUsuario + "\"",
                    CriticidadBitacora.Alta, DateTime.Now) { IdEmpresa = empresa.IdEmpresa });

                return t;
            });

            return token;
        }

        public Empresa_BE ObtenerMiEmpresa(ActorUsuario_TE actor)
        {
            if (actor == null || actor.IdEmpresa <= 0 || !actor.Puede(Patentes_TLL.VER_DATOS_EMPRESA))
                throw new UnauthorizedAccessException("No tenés permiso para ver los datos de la empresa.");

            return empresaRepo.ObtenerResumenPorId(actor.IdEmpresa);
        }

        public void ModificarDatos(ActorUsuario_TE actor, int idEmpresa, Empresa_BE nuevos, string motivo)
        {
            actor.Exigir(Patentes_TLL.MODIFICAR_EMPRESA);

            Empresa_BE actual = ObtenerExistente(idEmpresa);

            ValidarYNormalizar(nuevos, idEmpresa, actual.Cuit);

            var cambios = new List<string>();
            bool critico = false;

            critico |= Registrar(cambios, "razón social", actual.NombreEmpresa, nuevos.NombreEmpresa);
            critico |= Registrar(cambios, "CUIT", actual.Cuit, nuevos.Cuit);
            critico |= Registrar(cambios, "plan", actual.PlanSuscripcion.ToString(), nuevos.PlanSuscripcion.ToString());
            Registrar(cambios, "rubro", actual.Rubro, nuevos.Rubro);
            Registrar(cambios, "domicilio", actual.Domicilio, nuevos.Domicilio);
            Registrar(cambios, "teléfono", actual.NumContactoEmpresa, nuevos.NumContactoEmpresa);
            bool cambiaCiclo = Registrar(cambios, "facturación", actual.Facturacion.HasValue ? actual.Facturacion.Value.ToString() : null, nuevos.Facturacion.ToString());

            if (cambios.Count == 0) throw new InvalidOperationException("No hay cambios para guardar.");

            string motivoLimpio = LimpiarMotivo(motivo);
            if (critico && motivoLimpio.Length == 0) throw new InvalidOperationException("Indicá el motivo: se modifica la razón social, el CUIT o el plan.");

            actual.NombreEmpresa = nuevos.NombreEmpresa;
            actual.Cuit = nuevos.Cuit;
            actual.Rubro = nuevos.Rubro;
            actual.Domicilio = nuevos.Domicilio;
            actual.NumContactoEmpresa = nuevos.NumContactoEmpresa;
            actual.PlanSuscripcion = nuevos.PlanSuscripcion;
            actual.Facturacion = nuevos.Facturacion;

            if (cambiaCiclo) actual.FechaRenovacion = ProximaRenovacion(actual.FechaAlta, nuevos.Facturacion.Value);

            Transaccion_ORM.Ejecutar(() =>
            {
                empresaRepo.Modificar(actual);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.EmpresaCliente, new[] { actual.IdEmpresa.ToString() });

                bitacora.Auditar(actor, "Empresas", actual.IdEmpresa,
                    "Modificación de datos de la empresa \"" + actual.NombreEmpresa + "\": " + string.Join("; ", cambios) + ConMotivo(motivoLimpio),
                    critico ? CriticidadBitacora.Alta : CriticidadBitacora.Media);
            });
        }

        public void ModificarContacto(ActorUsuario_TE actor, string rubro, string domicilio, string telefono)
        {
            actor.Exigir(Patentes_TLL.MODIFICAR_CONTACTO_EMPRESA);

            if (actor.IdEmpresa <= 0) throw new UnauthorizedAccessException("La sesión no tiene una empresa asociada.");

            Empresa_BE actual = ObtenerExistente(actor.IdEmpresa);

            rubro = Recortar(rubro);
            domicilio = Recortar(domicilio);
            telefono = Recortar(telefono);

            if (Largo(rubro) > LARGO_RUBRO) throw new InvalidOperationException("El rubro no puede superar los " + LARGO_RUBRO + " caracteres.");
            if (Largo(domicilio) > LARGO_DOMICILIO) throw new InvalidOperationException("El domicilio no puede superar los " + LARGO_DOMICILIO + " caracteres.");
            if (Largo(telefono) > LARGO_CONTACTO) throw new InvalidOperationException("El número de contacto no puede superar los " + LARGO_CONTACTO + " caracteres.");

            var cambios = new List<string>();
            Registrar(cambios, "rubro", actual.Rubro, rubro);
            Registrar(cambios, "domicilio", actual.Domicilio, domicilio);
            Registrar(cambios, "teléfono", actual.NumContactoEmpresa, telefono);

            if (cambios.Count == 0) throw new InvalidOperationException("No hay cambios para guardar.");

            actual.Rubro = rubro;
            actual.Domicilio = domicilio;
            actual.NumContactoEmpresa = telefono;

            Transaccion_ORM.Ejecutar(() =>
            {
                empresaRepo.Modificar(actual);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.EmpresaCliente, new[] { actual.IdEmpresa.ToString() });

                bitacora.Auditar(actor, "Empresas", actual.IdEmpresa,
                    "Modificación de datos de contacto de la empresa \"" + actual.NombreEmpresa + "\" hecha por su administrador: " + string.Join("; ", cambios),
                    CriticidadBitacora.Media);
            });
        }

        public void CambiarEstado(ActorUsuario_TE actor, int idEmpresa, EstadoEmpresa nuevoEstado, string motivo)
        {
            actor.Exigir(Patentes_TLL.CAMBIAR_ESTADO_EMPRESA);

            if (idEmpresa == BitacoraGestor_TLL.ID_EMPRESA_PROVEEDORA) throw new InvalidOperationException("Pattern Blue no se bloquea ni se deshabilita.");
            if (!Enum.IsDefined(typeof(EstadoEmpresa), nuevoEstado)) throw new InvalidOperationException("El estado no es válido.");

            Empresa_BE actual = ObtenerExistente(idEmpresa);

            if (actual.Estado == nuevoEstado) throw new InvalidOperationException("La empresa ya está en ese estado.");

            if (actual.Estado == EstadoEmpresa.Deshabilitada && nuevoEstado != EstadoEmpresa.Activa)
                throw new InvalidOperationException("Una empresa dada de baja solo puede volver a Activa, si retoma el servicio.");

            string motivoLimpio = LimpiarMotivo(motivo);
            if (motivoLimpio.Length == 0) throw new InvalidOperationException("Indicá el motivo del cambio de estado.");

            EstadoEmpresa anterior = actual.Estado;
            actual.Estado = nuevoEstado;

            Transaccion_ORM.Ejecutar(() =>
            {
                empresaRepo.Modificar(actual);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.EmpresaCliente, new[] { actual.IdEmpresa.ToString() });

                bitacora.Auditar(actor, "Empresas", actual.IdEmpresa,
                    "Cambio de estado de la empresa \"" + actual.NombreEmpresa + "\": " + anterior + " → " + nuevoEstado + ConMotivo(motivoLimpio),
                    CriticidadBitacora.Alta);
            });
        }

        private Empresa_BE ObtenerExistente(int idEmpresa)
        {
            Empresa_BE empresa = empresaRepo.ObtenerPorPK(idEmpresa);

            if (empresa == null) throw new InvalidOperationException("La empresa no existe.");

            return empresa;
        }

        private static bool Registrar(List<string> cambios, string campo, string antes, string despues)
        {
            if (string.Equals(antes ?? string.Empty, despues ?? string.Empty, StringComparison.Ordinal)) return false;

            cambios.Add(campo + ": " + (string.IsNullOrEmpty(antes) ? "(vacío)" : antes) + " → " + (string.IsNullOrEmpty(despues) ? "(vacío)" : despues));
            return true;
        }

        private static string LimpiarMotivo(string motivo)
        {
            motivo = (motivo ?? string.Empty).Trim();

            if (motivo.Length > 300) throw new InvalidOperationException("El motivo no puede superar los 300 caracteres.");

            return motivo;
        }

        private static string ConMotivo(string motivo)
        {
            return motivo.Length == 0 ? string.Empty : ". Motivo: " + motivo;
        }

        private static DateTime ProximaRenovacion(DateTime alta, CicloFacturacion ciclo)
        {
            DateTime fecha = alta;
            int paso = 1;

            while (fecha <= DateTime.Now)
            {
                fecha = ciclo == CicloFacturacion.Anual ? alta.AddYears(paso) : alta.AddMonths(paso);
                paso++;
            }

            return fecha;
        }

        public List<Empresa_BE> ObtenerTodas() => empresaRepo.ObtenerTodos();

        public Empresa_BE ObtenerPorId(int idEmpresa) => empresaRepo.ObtenerPorPK(idEmpresa);

        public List<Empresa_BE> ObtenerParaFiltroDeBitacora(ActorUsuario_TE actor)
        {
            if (actor == null || !actor.VeBitacoraCompleta()) throw new UnauthorizedAccessException("No tenés permiso para ver la bitácora de todas las empresas.");

            return empresaRepo.ObtenerTodos().Select(e => new Empresa_BE { IdEmpresa = e.IdEmpresa, NombreEmpresa = e.NombreEmpresa }).OrderBy(e => e.NombreEmpresa).ToList();
        }

        public List<Empresa_BE> ObtenerCartera(ActorUsuario_TE actor)
        {
            if (actor == null || !actor.VeTodasLasEmpresas()) throw new UnauthorizedAccessException("No tenés permiso para ver todas las empresas.");

            return empresaRepo.ObtenerResumen();
        }

        private void ValidarYNormalizar(Empresa_BE empresa, int idEmpresaPropia = 0, string cuitActual = null)
        {
            empresa.NombreEmpresa = (empresa.NombreEmpresa ?? string.Empty).Trim();
            empresa.NumContactoEmpresa = Recortar(empresa.NumContactoEmpresa);
            empresa.Rubro = Recortar(empresa.Rubro);
            empresa.Domicilio = Recortar(empresa.Domicilio);

            if (empresa.NombreEmpresa.Length == 0) throw new InvalidOperationException("La razón social es obligatoria.");
            if (empresa.NombreEmpresa.Length > LARGO_NOMBRE) throw new InvalidOperationException("La razón social no puede superar los " + LARGO_NOMBRE + " caracteres.");
            if (Largo(empresa.NumContactoEmpresa) > LARGO_CONTACTO) throw new InvalidOperationException("El número de contacto no puede superar los " + LARGO_CONTACTO + " caracteres.");
            if (Largo(empresa.Rubro) > LARGO_RUBRO) throw new InvalidOperationException("El rubro no puede superar los " + LARGO_RUBRO + " caracteres.");
            if (Largo(empresa.Domicilio) > LARGO_DOMICILIO) throw new InvalidOperationException("El domicilio no puede superar los " + LARGO_DOMICILIO + " caracteres.");

            if (!Enum.IsDefined(typeof(PlanSuscripcion), empresa.PlanSuscripcion)) throw new InvalidOperationException("El plan de suscripción no es válido.");
            if (!empresa.Facturacion.HasValue || !Enum.IsDefined(typeof(CicloFacturacion), empresa.Facturacion.Value)) throw new InvalidOperationException("El tipo de facturación no es válido.");

            string cuitIngresado = NormalizarCuit(empresa.Cuit);
            bool cuitSinCambios = idEmpresaPropia > 0 && (cuitIngresado != null ? cuitIngresado == NormalizarCuit(cuitActual) : string.IsNullOrWhiteSpace(empresa.Cuit) && string.IsNullOrWhiteSpace(cuitActual));

            if (cuitSinCambios)
            {
                empresa.Cuit = cuitIngresado ?? cuitActual;
            }
            else
            {
                empresa.Cuit = cuitIngresado;
                if (empresa.Cuit == null) throw new InvalidOperationException("El CUIT debe tener 11 dígitos, con o sin guiones (por ejemplo 30-12345678-1).");

                string motivoInvalido = MotivoCuitInvalido(empresa.Cuit);
                if (motivoInvalido != null) throw new InvalidOperationException(motivoInvalido);
            }

            if (empresaRepo.ExisteNombre(empresa.NombreEmpresa, idEmpresaPropia)) throw new InvalidOperationException("Ya existe una empresa registrada con esa razón social.");
            if (empresa.Cuit != null && empresaRepo.ExisteCuit(empresa.Cuit, idEmpresaPropia)) throw new InvalidOperationException("Ya existe una empresa registrada con ese CUIT.");
        }

        private static readonly int[] PrefijosCuit = { 20, 23, 24, 27, 30, 33, 34 };
        private static readonly int[] PesosCuit = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };

        private static string MotivoCuitInvalido(string cuitNormalizado)
        {
            const string FORMATO = " El formato es 11 dígitos, con o sin guiones (por ejemplo 30-12345678-1).";
            string digitos = cuitNormalizado.Replace("-", string.Empty);

            if (!PrefijosCuit.Contains(int.Parse(digitos.Substring(0, 2))))
                return "El CUIT tiene que empezar con 20, 23, 24, 27, 30, 33 o 34, y el tuyo empieza con " + digitos.Substring(0, 2) + "." + FORMATO;

            return TieneDigitoVerificadorValido(cuitNormalizado)
                ? null
                : "El último dígito del CUIT no coincide con los anteriores (es un número de control): revisá que no haya un error de tipeo." + FORMATO;
        }

        private static bool TieneDigitoVerificadorValido(string cuitNormalizado)
        {
            string digitos = cuitNormalizado.Replace("-", string.Empty);

            if (digitos.Length != 11 || !PrefijosCuit.Contains(int.Parse(digitos.Substring(0, 2)))) return false;

            int suma = 0;
            for (int i = 0; i < PesosCuit.Length; i++) suma += (digitos[i] - '0') * PesosCuit[i];

            int resto = suma % 11;

            if (resto == 1) return false;

            int esperado = resto == 0 ? 0 : 11 - resto;

            return digitos[10] - '0' == esperado;
        }

        private static string NormalizarCuit(string cuit)
        {
            string digitos = new string((cuit ?? string.Empty).Where(char.IsDigit).ToArray());

            if (digitos.Length != 11) return null;

            return digitos.Substring(0, 2) + "-" + digitos.Substring(2, 8) + "-" + digitos.Substring(10, 1);
        }
    }
}
