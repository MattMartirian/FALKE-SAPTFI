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
                bitacora.Registrar(actor != null ? actor.IdUsuario : 0, "Seguridad", EventosBitacora_TLL.ACCION_RECHAZADA_PERMISO, CriticidadBitacora.Media, null, DetalleBitacora_TLL.Armar(Patentes_TLL.REGISTRAR_EMPRESA));
                throw new ExcepcionNoAutorizada_TLL("empresa_sin_permiso_registrar");
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

                bitacora.Guardar(new Bitacora_TE(actor.IdUsuario, "Empresas", EventosBitacora_TLL.EMPRESA_ALTA, CriticidadBitacora.Alta, DateTime.Now, 0,
                    DetalleBitacora_TLL.Armar(empresa.NombreEmpresa)) { IdEmpresa = empresa.IdEmpresa });

                return t;
            });

            return token;
        }

        public Empresa_BE ObtenerMiEmpresa(ActorUsuario_TE actor)
        {
            if (actor == null || actor.IdEmpresa <= 0 || !actor.Puede(Patentes_TLL.VER_DATOS_EMPRESA))
                throw new ExcepcionNoAutorizada_TLL("empresa_sin_permiso_ver_datos");

            return empresaRepo.ObtenerResumenPorId(actor.IdEmpresa);
        }

        public void ModificarDatos(ActorUsuario_TE actor, int idEmpresa, Empresa_BE nuevos, string motivo)
        {
            actor.Exigir(Patentes_TLL.MODIFICAR_EMPRESA);

            Empresa_BE actual = ObtenerExistente(idEmpresa);

            ValidarYNormalizar(nuevos, idEmpresa, actual.Cuit);

            var cambios = new List<ParteBitacora_TLL>();
            bool critico = false;

            critico |= Registrar(cambios, "bitacora_campo_razon_social", actual.NombreEmpresa, nuevos.NombreEmpresa);
            critico |= Registrar(cambios, "bitacora_campo_cuit", actual.Cuit, nuevos.Cuit);
            critico |= Registrar(cambios, "bitacora_campo_plan", actual.PlanSuscripcion.ToString(), nuevos.PlanSuscripcion.ToString());
            Registrar(cambios, "bitacora_campo_rubro", actual.Rubro, nuevos.Rubro);
            Registrar(cambios, "bitacora_campo_domicilio", actual.Domicilio, nuevos.Domicilio);
            Registrar(cambios, "bitacora_campo_telefono", actual.NumContactoEmpresa, nuevos.NumContactoEmpresa);
            bool cambiaCiclo = Registrar(cambios, "bitacora_campo_facturacion", actual.Facturacion.HasValue ? actual.Facturacion.Value.ToString() : null, nuevos.Facturacion.ToString());

            if (cambios.Count == 0) throw new ExcepcionTraducible_TLL("comun_sin_cambios");

            string motivoLimpio = LimpiarMotivo(motivo);
            if (critico && motivoLimpio.Length == 0) throw new ExcepcionTraducible_TLL("empresa_motivo_requerido");

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

                bitacora.Auditar(actor, "Empresas", actual.IdEmpresa, EventosBitacora_TLL.EMPRESA_MODIFICACION, critico ? CriticidadBitacora.Alta : CriticidadBitacora.Media,
                    DetalleBitacora_TLL.Armar(actual.NombreEmpresa, ": ", DetalleBitacora_TLL.Lista(cambios, ", "), DetalleBitacora_TLL.Motivo(motivoLimpio)));
            });
        }

        public void ModificarContacto(ActorUsuario_TE actor, string rubro, string domicilio, string telefono)
        {
            actor.Exigir(Patentes_TLL.MODIFICAR_CONTACTO_EMPRESA);

            if (actor.IdEmpresa <= 0) throw new ExcepcionNoAutorizada_TLL("comun_sesion_sin_empresa");

            Empresa_BE actual = ObtenerExistente(actor.IdEmpresa);

            rubro = Recortar(rubro);
            domicilio = Recortar(domicilio);
            telefono = Recortar(telefono);

            if (Largo(rubro) > LARGO_RUBRO) throw new ExcepcionTraducible_TLL("empresa_rubro_largo", LARGO_RUBRO);
            if (Largo(domicilio) > LARGO_DOMICILIO) throw new ExcepcionTraducible_TLL("empresa_domicilio_largo", LARGO_DOMICILIO);
            if (Largo(telefono) > LARGO_CONTACTO) throw new ExcepcionTraducible_TLL("empresa_contacto_largo", LARGO_CONTACTO);

            var cambios = new List<ParteBitacora_TLL>();
            Registrar(cambios, "bitacora_campo_rubro", actual.Rubro, rubro);
            Registrar(cambios, "bitacora_campo_domicilio", actual.Domicilio, domicilio);
            Registrar(cambios, "bitacora_campo_telefono", actual.NumContactoEmpresa, telefono);

            if (cambios.Count == 0) throw new ExcepcionTraducible_TLL("comun_sin_cambios");

            actual.Rubro = rubro;
            actual.Domicilio = domicilio;
            actual.NumContactoEmpresa = telefono;

            Transaccion_ORM.Ejecutar(() =>
            {
                empresaRepo.Modificar(actual);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.EmpresaCliente, new[] { actual.IdEmpresa.ToString() });

                bitacora.Auditar(actor, "Empresas", actual.IdEmpresa, EventosBitacora_TLL.EMPRESA_MODIFICACION_CONTACTO, CriticidadBitacora.Media,
                    DetalleBitacora_TLL.Armar(actual.NombreEmpresa, ": ", DetalleBitacora_TLL.Lista(cambios, ", ")));
            });
        }

        public void CambiarEstado(ActorUsuario_TE actor, int idEmpresa, EstadoEmpresa nuevoEstado, string motivo)
        {
            actor.Exigir(Patentes_TLL.CAMBIAR_ESTADO_EMPRESA);

            if (idEmpresa == BitacoraGestor_TLL.ID_EMPRESA_PROVEEDORA) throw new ExcepcionTraducible_TLL("empresa_pattern_blue_no_se_bloquea");
            if (!Enum.IsDefined(typeof(EstadoEmpresa), nuevoEstado)) throw new ExcepcionTraducible_TLL("empresa_estado_invalido");

            Empresa_BE actual = ObtenerExistente(idEmpresa);

            if (actual.Estado == nuevoEstado) throw new ExcepcionTraducible_TLL("empresa_ya_en_ese_estado");

            if (actual.Estado == EstadoEmpresa.Deshabilitada && nuevoEstado != EstadoEmpresa.Activa)
                throw new ExcepcionTraducible_TLL("empresa_baja_solo_activa");

            string motivoLimpio = LimpiarMotivo(motivo);
            if (motivoLimpio.Length == 0) throw new ExcepcionTraducible_TLL("empresa_motivo_cambio_estado");

            string evento = nuevoEstado == EstadoEmpresa.Activa ? EventosBitacora_TLL.EMPRESA_REACTIVACION
                : nuevoEstado == EstadoEmpresa.Bloqueada ? EventosBitacora_TLL.EMPRESA_BLOQUEO
                : EventosBitacora_TLL.EMPRESA_BAJA;
            actual.Estado = nuevoEstado;

            Transaccion_ORM.Ejecutar(() =>
            {
                empresaRepo.Modificar(actual);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.EmpresaCliente, new[] { actual.IdEmpresa.ToString() });

                bitacora.Auditar(actor, "Empresas", actual.IdEmpresa, evento, CriticidadBitacora.Alta,
                    DetalleBitacora_TLL.Armar(actual.NombreEmpresa, DetalleBitacora_TLL.Motivo(motivoLimpio)));
            });
        }

        private Empresa_BE ObtenerExistente(int idEmpresa)
        {
            Empresa_BE empresa = empresaRepo.ObtenerPorPK(idEmpresa);

            if (empresa == null) throw new ExcepcionTraducible_TLL("disp_empresa_no_existe");

            return empresa;
        }

        private static bool Registrar(List<ParteBitacora_TLL> cambios, string campo, string antes, string despues)
        {
            if (string.Equals(antes ?? string.Empty, despues ?? string.Empty, StringComparison.Ordinal)) return false;

            cambios.Add(DetalleBitacora_TLL.Clave(campo));
            return true;
        }


        private static string LimpiarMotivo(string motivo)
        {
            motivo = (motivo ?? string.Empty).Trim();

            if (motivo.Length > 300) throw new ExcepcionTraducible_TLL("comun_motivo_largo_300");

            return motivo;
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
            if (actor == null || !actor.VeBitacoraCompleta()) throw new ExcepcionNoAutorizada_TLL("empresa_sin_permiso_bitacora_todas");

            return empresaRepo.ObtenerTodos().Select(e => new Empresa_BE { IdEmpresa = e.IdEmpresa, NombreEmpresa = e.NombreEmpresa }).OrderBy(e => e.NombreEmpresa).ToList();
        }

        public List<Empresa_BE> ObtenerCartera(ActorUsuario_TE actor)
        {
            if (actor == null || !actor.VeTodasLasEmpresas()) throw new ExcepcionNoAutorizada_TLL("empresa_sin_permiso_ver_todas");

            return empresaRepo.ObtenerResumen();
        }

        private void ValidarYNormalizar(Empresa_BE empresa, int idEmpresaPropia = 0, string cuitActual = null)
        {
            empresa.NombreEmpresa = (empresa.NombreEmpresa ?? string.Empty).Trim();
            empresa.NumContactoEmpresa = Recortar(empresa.NumContactoEmpresa);
            empresa.Rubro = Recortar(empresa.Rubro);
            empresa.Domicilio = Recortar(empresa.Domicilio);

            if (empresa.NombreEmpresa.Length == 0) throw new ExcepcionTraducible_TLL("empresa_razon_obligatoria");
            if (empresa.NombreEmpresa.Length > LARGO_NOMBRE) throw new ExcepcionTraducible_TLL("empresa_razon_larga", LARGO_NOMBRE);
            if (Largo(empresa.NumContactoEmpresa) > LARGO_CONTACTO) throw new ExcepcionTraducible_TLL("empresa_contacto_largo", LARGO_CONTACTO);
            if (Largo(empresa.Rubro) > LARGO_RUBRO) throw new ExcepcionTraducible_TLL("empresa_rubro_largo", LARGO_RUBRO);
            if (Largo(empresa.Domicilio) > LARGO_DOMICILIO) throw new ExcepcionTraducible_TLL("empresa_domicilio_largo", LARGO_DOMICILIO);

            if (!Enum.IsDefined(typeof(PlanSuscripcion), empresa.PlanSuscripcion)) throw new ExcepcionTraducible_TLL("empresa_plan_invalido");
            if (!empresa.Facturacion.HasValue || !Enum.IsDefined(typeof(CicloFacturacion), empresa.Facturacion.Value)) throw new ExcepcionTraducible_TLL("empresa_facturacion_invalida");

            string cuitIngresado = NormalizarCuit(empresa.Cuit);
            bool cuitSinCambios = idEmpresaPropia > 0 && (cuitIngresado != null ? cuitIngresado == NormalizarCuit(cuitActual) : string.IsNullOrWhiteSpace(empresa.Cuit) && string.IsNullOrWhiteSpace(cuitActual));

            if (cuitSinCambios)
            {
                empresa.Cuit = cuitIngresado ?? cuitActual;
            }
            else
            {
                empresa.Cuit = cuitIngresado;
                if (empresa.Cuit == null) throw new ExcepcionTraducible_TLL("empresa_cuit_formato");

                ValidarDigitosCuit(empresa.Cuit);
            }

            if (empresaRepo.ExisteNombre(empresa.NombreEmpresa, idEmpresaPropia)) throw new ExcepcionTraducible_TLL("empresa_razon_duplicada");
            if (empresa.Cuit != null && empresaRepo.ExisteCuit(empresa.Cuit, idEmpresaPropia)) throw new ExcepcionTraducible_TLL("empresa_cuit_duplicado");
        }

        private static readonly int[] PrefijosCuit = { 20, 23, 24, 27, 30, 33, 34 };
        private static readonly int[] PesosCuit = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };

        private static void ValidarDigitosCuit(string cuitNormalizado)
        {
            string digitos = cuitNormalizado.Replace("-", string.Empty);

            if (!PrefijosCuit.Contains(int.Parse(digitos.Substring(0, 2))))
                throw new ExcepcionTraducible_TLL("empresa_cuit_prefijo", digitos.Substring(0, 2));

            if (!TieneDigitoVerificadorValido(cuitNormalizado))
                throw new ExcepcionTraducible_TLL("empresa_cuit_digito");
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
