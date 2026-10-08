using ORM;
using SECURITY;
using SERVICES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Web;
using TE;

namespace TLL
{
    public class Usuario_TLL
    {
        private const int MAX_INTENTOS_FALLIDOS = 5;
        private const int LARGO_MINIMO_CONTRASENA = 8;

        // Lo que se le dice al usuario cuando la contraseña no cumple. Es el mismo texto en todas las pantallas.
        public const string POLITICA_CONTRASENA = "La contraseña debe tener al menos 8 caracteres, con una mayúscula, una minúscula, un número y un carácter especial (por ejemplo # ! @ $ %).";

        public const string TOKEN_ACTIVACION = "activacion";
        public const string TOKEN_RECUPERACION = "recuperacion";

        public const string MOTIVO_EMPRESA_BLOQUEADA = "EMPRESA_BLOQUEADA";
        public const string MOTIVO_EMPRESA_DESHABILITADA = "EMPRESA_DESHABILITADA";

        public const string ROL_GESTOR = "Gestor";
        public const string ROL_WEBMASTER = "Webmaster";
        public const string ROL_ADMINISTRADOR = "Administrador";
        public const string ROL_ANALISTA = "Analista";

        private static readonly TimeSpan VIGENCIA_ACTIVACION = TimeSpan.FromHours(48);
        private static readonly TimeSpan VIGENCIA_RECUPERACION = TimeSpan.FromHours(2);

        private readonly UsuarioRepository usuarioRepo;
        private readonly TokenRepository tokenRepo;
        private readonly Cifrador_SECURITY cifrador;
        private readonly GestorIntegridad_SERVICE gestorIntegridad;
        private readonly BitacoraGestor_TLL bitacora;

        public Usuario_TLL()
        {
            usuarioRepo = new UsuarioRepository();
            tokenRepo = new TokenRepository();
            cifrador = Cifrador_SECURITY.CifradorSingleton;
            gestorIntegridad = new GestorIntegridad_SERVICE();
            bitacora = new BitacoraGestor_TLL();
        }

        public ResultadoLogin_TLL ValidarCredenciales(string email, string contrasenaPlana)
        {
            if (EsCredencialDeEmergencia(email, contrasenaPlana))
            {
                var usuarioEmergencia = ConstruirUsuarioEmergenciaEnMemoria(email);
                LoguearAccesoEmergenciaAArchivo(email);
                bitacora.Registrar(0, "Seguridad", "Acceso de emergencia (break-glass) con identificador '" + email + "'", CriticidadBitacora.Alta);
                return ResultadoLogin_TLL.Exitoso(usuarioEmergencia, HayInconsistenciasDeIntegridad());
            }

            email = NormalizarEmail(email);

            var usuario = usuarioRepo.ObtenerPorEmail(email);

            if (usuario == null) return ResultadoLogin_TLL.CredencialesInvalidas();

            if (usuario.Estado == EstadoUsuario.BloqueadoPorIntentos)
            {
                bitacora.Registrar(usuario.IdUsuario, "Seguridad", "Intento de inicio de sesión sobre una cuenta bloqueada por intentos fallidos", CriticidadBitacora.Media);
                return ResultadoLogin_TLL.BloqueadoPorIntentos();
            }

            if (usuario.Estado == EstadoUsuario.Pendiente)
            {
                bitacora.Registrar(usuario.IdUsuario, "Seguridad", "Intento de inicio de sesión sobre una cuenta pendiente de activación", CriticidadBitacora.Baja);
                return ResultadoLogin_TLL.UsuarioPendienteActivacion();
            }

            if (usuario.Estado == EstadoUsuario.Inactivo)
            {
                bitacora.Registrar(usuario.IdUsuario, "Seguridad", "Intento de inicio de sesión sobre una cuenta inactiva (dada de baja)", CriticidadBitacora.Baja, usuario.IdEmpresa);
                return ResultadoLogin_TLL.Inactivo();
            }

            if (usuario.Estado == EstadoUsuario.BloqueoEstricto)
            {
                bitacora.Registrar(usuario.IdUsuario, "Seguridad", "Intento de inicio de sesión sobre una cuenta con bloqueo estricto", CriticidadBitacora.Media, usuario.IdEmpresa);
                return ResultadoLogin_TLL.BloqueoEstricto();
            }

            if (!VerificarContrasena(contrasenaPlana, usuario.ContrasenaHashUsuario))
            {
                RegistrarIntentoFallido(usuario);

                // El intento que agota el cupo bloquea la cuenta en ese momento: se le avisa enseguida.
                if (usuario.Estado == EstadoUsuario.BloqueadoPorIntentos) return ResultadoLogin_TLL.BloqueadoPorIntentos();

                return ResultadoLogin_TLL.CredencialesInvalidas(Math.Max(0, MAX_INTENTOS_FALLIDOS - usuario.IntentosFallidosUsuario));
            }

            string motivoEmpresa = MotivoEmpresaSinIngreso(usuario.IdEmpresa);
            if (motivoEmpresa != null)
            {
                bitacora.Registrar(usuario.IdUsuario, "Seguridad",
                    "Inicio de sesión rechazado: la empresa del usuario está " + (motivoEmpresa == MOTIVO_EMPRESA_DESHABILITADA ? "dada de baja" : "bloqueada"),
                    CriticidadBitacora.Media, usuario.IdEmpresa);
                return ResultadoLogin_TLL.EmpresaNoActiva(motivoEmpresa);
            }

            // Con la integridad comprometida nadie entra, salvo quien puede repararla (recalcular el dígito verificador): ese entra y se lo lleva directo
            // a la pantalla de Dígito verificador.
            var inconsistencias = gestorIntegridad.VerificarIntegridadTodasLasTablas();
            bool revisarIntegridad = inconsistencias.Count > 0;

            if (revisarIntegridad)
            {
                if (!Permiso_TLL.ComprobarPermiso(Patentes_TLL.RECALCULAR_INTEGRIDAD, usuario.Rol))
                {
                    bitacora.Registrar(usuario.IdUsuario, "Integridad", "Inicio de sesión rechazado: la integridad de los datos está comprometida (" + inconsistencias.Count + " inconsistencia/s)", CriticidadBitacora.Alta);
                    return ResultadoLogin_TLL.IntegridadComprometida();
                }

                bitacora.Registrar(usuario.IdUsuario, "Integridad", "Inicio de sesión con la integridad de los datos comprometida (" + inconsistencias.Count + " inconsistencia/s): se lo lleva a Dígito verificador para revisarla", CriticidadBitacora.Alta);
            }

            Transaccion_ORM.Ejecutar(() =>
            {
                ResetearIntentosFallidos(usuario);
                bitacora.Registrar(usuario.IdUsuario, "Seguridad", "Inicio de sesión exitoso", CriticidadBitacora.Baja);
            });

            return ResultadoLogin_TLL.Exitoso(usuario, revisarIntegridad);
        }

        public string RegistrarUsuario(ActorUsuario_TE actor, Usuario_TE usuario)
        {
            ExigirPatente(actor, Patentes_TLL.REGISTRAR_USUARIO);

            if (!actor.Puede(Patentes_TLL.CREAR_USUARIO_OTRA_EMPRESA))
                ExigirAlcance(actor, usuario.IdEmpresa, "dar de alta un usuario");

            if (!usuarioRepo.ExisteEmpresa(usuario.IdEmpresa)) throw new InvalidOperationException("La empresa indicada no existe.");

            string rol = usuario.Rol != null ? usuario.Rol.Nombre : null;
            if (!RolesAsignables(actor).Contains(rol)) throw new InvalidOperationException("Ese rol no se puede asignar desde tu cuenta.");

            ExigirRolCompatibleConEmpresa(rol, usuario.IdEmpresa);

            return RegistrarUsuarioInterno(usuario, actor.IdUsuario);
        }

        private string RegistrarUsuarioInterno(Usuario_TE usuario, int idActor)
        {
            usuario.EmailUsuario = NormalizarEmail(usuario.EmailUsuario);

            if (!EsEmailValido(usuario.EmailUsuario)) throw new InvalidOperationException("El email no tiene un formato valido.");

            if (usuarioRepo.ObtenerPorEmail(usuario.EmailUsuario) != null) throw new InvalidOperationException("Ya existe un usuario registrado con ese email.");

            usuario.ContrasenaHashUsuario = cifrador.Encoder(Cifrador_SECURITY.GenerarSecretoUrlSafe());
            usuario.IntentosFallidosUsuario = 0;
            usuario.Estado = EstadoUsuario.Pendiente;

            string rolNombre = usuario.Rol != null ? usuario.Rol.Nombre : "(sin rol)";

            string token = Transaccion_ORM.Ejecutar(() =>
            {
                usuarioRepo.Alta(usuario);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { usuario.IdUsuario.ToString() });

                string t = EmitirToken(usuario.IdUsuario, TOKEN_ACTIVACION, VIGENCIA_ACTIVACION);

                Auditar(idActor > 0 ? idActor : usuario.IdUsuario, usuario.IdEmpresa,
                    "Alta de usuario '" + usuario.EmailUsuario + "' (rol " + rolNombre + "); queda pendiente de activación", CriticidadBitacora.Media);

                return t;
            });

            return token;
        }

        // Para una cuenta que sigue pendiente de activación (el enlace venció, se perdió o el correo estaba mal): emite un enlace nuevo. El
        // anterior no se anula (sigue valiendo hasta que venza o se use). Lo hace quien puede dar de alta usuarios, solo dentro de su alcance.
        // El correo lo manda quien llama.
        public SolicitudEnlace_TLL ReenviarInvitacion(ActorUsuario_TE actor, int idUsuario)
        {
            ExigirPatente(actor, Patentes_TLL.REGISTRAR_USUARIO);

            var objetivo = ObtenerObjetivo(idUsuario);
            ExigirAlcance(actor, objetivo.IdEmpresa, "reenviar una invitación");
            ExigirJerarquia(actor, objetivo, "reenviar la invitación a");

            if (objetivo.IdUsuario == actor.IdUsuario) throw new InvalidOperationException("No podés reenviarte una invitación a vos mismo.");

            if (objetivo.Estado != EstadoUsuario.Pendiente) throw new InvalidOperationException("La invitación solo se reenvía a una cuenta que sigue pendiente de activación.");

            string empresa = usuarioRepo.ObtenerNombreEmpresa(objetivo.IdEmpresa);

            string token = Transaccion_ORM.Ejecutar(() =>
            {
                string t = EmitirToken(objetivo.IdUsuario, TOKEN_ACTIVACION, VIGENCIA_ACTIVACION, false);

                Auditar(actor.IdUsuario, objetivo.IdEmpresa,
                    "Reenvío de la invitación al usuario '" + objetivo.EmailUsuario + "' (empresa " + empresa + "): se emitió un enlace de activación nuevo",
                    CriticidadBitacora.Media);

                return t;
            });

            return new SolicitudEnlace_TLL { Token = token, Nombre = objetivo.NombreUsuario, Email = objetivo.EmailUsuario };
        }

        public PaginaUsuarios_TE ListarUsuarios(ActorUsuario_TE actor, FiltroUsuarios_TE filtro)
        {
            ExigirPatente(actor, Patentes_TLL.VER_USUARIOS);

            var f = (filtro ?? new FiltroUsuarios_TE()).Copiar();

            if (!actor.VeTodasLasEmpresas())
            {
                if (actor.IdEmpresa <= 0) throw new UnauthorizedAccessException("La sesión no tiene una empresa asociada.");

                f.IdEmpresa = actor.IdEmpresa;
            }

            return usuarioRepo.Listar(f);
        }

        // Quien ve todas las empresas asigna cualquier rol. Los demás, solo los que no tienen más permisos que ellos
        // (así nadie puede darse ni dar un permiso que no tiene).
        public List<string> RolesAsignables(ActorUsuario_TE actor)
        {
            var roles = new PermisoRepository().ConstruirArbolDeRoles();

            if (actor.VeTodasLasEmpresas()) return roles.Select(r => r.Nombre).OrderBy(n => n).ToList();

            HashSet<string> propios = Permiso_TLL.ObtenerPatentes(actor.Permiso);

            return roles.Where(r => Permiso_TLL.ObtenerPatentes(r).IsSubsetOf(propios)).Select(r => r.Nombre).OrderBy(n => n).ToList();
        }

        public void CambiarEstado(ActorUsuario_TE actor, int idUsuario, EstadoUsuario nuevoEstado, string motivo)
        {
            ExigirPatente(actor, Patentes_TLL.CAMBIAR_ESTADO_USUARIO);

            var objetivo = ObtenerObjetivo(idUsuario);
            ExigirAlcance(actor, objetivo.IdEmpresa, "cambiar el estado de un usuario");
            ExigirJerarquia(actor, objetivo, "cambiar el estado de");

            if (objetivo.IdUsuario == actor.IdUsuario) throw new InvalidOperationException("No podés cambiar tu propio estado.");
            if (nuevoEstado == objetivo.Estado) throw new InvalidOperationException("El usuario ya está en ese estado.");

            if (nuevoEstado != EstadoUsuario.Activo && nuevoEstado != EstadoUsuario.Inactivo && nuevoEstado != EstadoUsuario.BloqueoEstricto)
                throw new InvalidOperationException("Ese estado no se asigna manualmente.");

            if (objetivo.Estado == EstadoUsuario.Pendiente && nuevoEstado == EstadoUsuario.Activo)
                throw new InvalidOperationException("La cuenta todavía no fue activada: la activa el propio usuario con el enlace que recibió.");

            string motivoLimpio = ValidarMotivo(actor, objetivo.IdEmpresa, motivo);

            if (EsAdministradorActivo(objetivo) && nuevoEstado != EstadoUsuario.Activo) ExigirOtroAdministradorActivo(objetivo);

            string anterior = NombreEstado(objetivo.Estado);
            string nuevo = NombreEstado(nuevoEstado);
            bool restringe = nuevoEstado == EstadoUsuario.Inactivo || nuevoEstado == EstadoUsuario.BloqueoEstricto;
            string empresa = usuarioRepo.ObtenerNombreEmpresa(objetivo.IdEmpresa);

            Transaccion_ORM.Ejecutar(() =>
            {
                usuarioRepo.ActualizarEstado(objetivo.IdUsuario, (int)nuevoEstado);

                if (nuevoEstado == EstadoUsuario.Activo) usuarioRepo.ActualizarIntentosFallidos(objetivo.IdUsuario, 0);

                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { objetivo.IdUsuario.ToString() });

                Auditar(actor.IdUsuario, objetivo.IdEmpresa,
                    "Cambio de estado del usuario '" + objetivo.EmailUsuario + "' (empresa " + empresa + "): " + anterior + " → " + nuevo + ConMotivo(motivoLimpio),
                    restringe ? CriticidadBitacora.Alta : CriticidadBitacora.Media);
            });
        }

        public void CambiarRol(ActorUsuario_TE actor, int idUsuario, string nuevoRol, string motivo, bool confirmado)
        {
            ExigirPatente(actor, Patentes_TLL.CAMBIAR_ROL_USUARIO);

            var objetivo = ObtenerObjetivo(idUsuario);
            ExigirAlcance(actor, objetivo.IdEmpresa, "cambiar el rol de un usuario");
            ExigirJerarquia(actor, objetivo, "cambiar el rol de");

            if (objetivo.IdUsuario == actor.IdUsuario) throw new InvalidOperationException("No podés cambiar tu propio rol.");

            nuevoRol = (nuevoRol ?? string.Empty).Trim();
            if (!RolesAsignables(actor).Contains(nuevoRol)) throw new InvalidOperationException("Ese rol no se puede asignar desde tu cuenta.");

            string anterior = objetivo.Rol != null ? objetivo.Rol.Nombre : "(sin rol)";
            if (anterior == nuevoRol) throw new InvalidOperationException("El usuario ya tiene ese rol.");

            ExigirRolCompatibleConEmpresa(nuevoRol, objetivo.IdEmpresa);

            bool tocaOtraEmpresa = actor.IdEmpresa != objetivo.IdEmpresa;
            if ((tocaOtraEmpresa || EsRolDeGestion(nuevoRol)) && !confirmado)
                throw new InvalidOperationException("Tenés que confirmar el aviso antes de cambiar el rol.");

            string motivoLimpio = ValidarMotivo(actor, objetivo.IdEmpresa, motivo);

            if (EsAdministradorActivo(objetivo) && nuevoRol != ROL_ADMINISTRADOR) ExigirOtroAdministradorActivo(objetivo);

            string empresa = usuarioRepo.ObtenerNombreEmpresa(objetivo.IdEmpresa);

            Transaccion_ORM.Ejecutar(() =>
            {
                usuarioRepo.ActualizarRol(objetivo.IdUsuario, nuevoRol);

                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { objetivo.IdUsuario.ToString() });

                Auditar(actor.IdUsuario, objetivo.IdEmpresa,
                    "Cambio de rol del usuario '" + objetivo.EmailUsuario + "' (empresa " + empresa + "): " + anterior + " → " + nuevoRol + ConMotivo(motivoLimpio),
                    CriticidadBitacora.Alta);
            });
        }

        // Un rol de gestión es del personal de Pattern Blue: no se asigna a usuarios de una empresa cliente.
        private static void ExigirRolCompatibleConEmpresa(string nombreRol, int idEmpresaDelUsuario)
        {
            if (idEmpresaDelUsuario == BitacoraGestor_TLL.ID_EMPRESA_PROVEEDORA) return;

            if (EsRolDeGestion(nombreRol))
            {
                var etiquetas = new Permiso_TLL().ObtenerEtiquetas();

                throw new InvalidOperationException("El rol \"" + Permiso_TLL.Etiqueta(etiquetas, nombreRol) + "\" es un rol de gestión, solo para usuarios de Pattern Blue: no se puede asignar a usuarios de empresas cliente.");
            }
        }

        // Los roles de gestión (solo para usuarios de Pattern Blue).
        public HashSet<string> RolesDeGestion()
        {
            return new HashSet<string>(new PermisoRepository().ConstruirArbolDeRoles().Where(r => r.EsDeGestion).Select(r => r.Nombre));
        }

        private static bool EsRolDeGestion(string nombreRol)
        {
            var rol = new PermisoRepository().ConstruirArbolDeRoles().FirstOrDefault(r => r.Nombre == nombreRol);

            return rol != null && rol.EsDeGestion;
        }

        public void ActualizarPerfil(int idUsuario, string nombre, string apellido, int idIdioma)
        {
            nombre = (nombre ?? string.Empty).Trim();
            apellido = (apellido ?? string.Empty).Trim();

            if (nombre.Length == 0 || apellido.Length == 0) throw new InvalidOperationException("El nombre y el apellido son obligatorios.");
            if (nombre.Length > 100 || apellido.Length > 100) throw new InvalidOperationException("El nombre y el apellido no pueden superar los 100 caracteres.");

            var usuario = ObtenerObjetivo(idUsuario);

            Transaccion_ORM.Ejecutar(() =>
            {
                usuarioRepo.ActualizarPerfil(usuario.IdUsuario, nombre, apellido, idIdioma);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { usuario.IdUsuario.ToString() });

                Auditar(usuario.IdUsuario, usuario.IdEmpresa, "Modificación de los datos personales del usuario '" + usuario.EmailUsuario + "'", CriticidadBitacora.Baja);
            });
        }

        // Nadie modifica a alguien con más permisos que él: un administrador no puede bloquear ni degradar al Gestor.
        // Los permisos de infraestructura (integridad y respaldos, del Webmaster) no cuentan: el Gestor administra también a los Webmasters.
        private void ExigirJerarquia(ActorUsuario_TE actor, Usuario_TE objetivo, string accion)
        {
            if (actor.EsEmergencia || objetivo.Rol == null) return;

            var propios = Permiso_TLL.ObtenerPatentes(actor.Permiso);

            if (Permiso_TLL.ObtenerPatentes(objetivo.Rol).Where(p => !Patentes_TLL.EsDeInfraestructura(p)).All(propios.Contains)) return;

            bitacora.Registrar(actor.IdUsuario, "Seguridad", "Intento de " + accion + " un usuario con más permisos que el suyo (usuario " + objetivo.IdUsuario + ")", CriticidadBitacora.Alta, objetivo.IdEmpresa);
            throw new UnauthorizedAccessException("No podés modificar a un usuario con más permisos que vos.");
        }

        // Datos de otro usuario. El administrador cambia nombre, apellido e idioma de la gente de su empresa.
        // Cambiar el correo o la empresa es solo de quien tenga CAMBIAR_EMAIL_EMPRESA_USUARIO (el Gestor).
        // Devuelve un token de activación nuevo cuando el usuario sigue pendiente y cambió su correo (hay que mandárselo a la dirección nueva).
        public string ModificarDatosUsuario(ActorUsuario_TE actor, int idUsuario, string nombre, string apellido, int idIdioma, string email, int idEmpresa, string motivo, bool confirmado)
        {
            ExigirPatente(actor, Patentes_TLL.MODIFICAR_USUARIO);

            var objetivo = ObtenerObjetivo(idUsuario);
            ExigirAlcance(actor, objetivo.IdEmpresa, "modificar los datos de un usuario");

            if (objetivo.IdUsuario == actor.IdUsuario) throw new InvalidOperationException("Tus propios datos se cambian desde «Mi perfil».");

            ExigirJerarquia(actor, objetivo, "modificar los datos de");

            nombre = (nombre ?? string.Empty).Trim();
            apellido = (apellido ?? string.Empty).Trim();

            if (nombre.Length == 0 || apellido.Length == 0) throw new InvalidOperationException("El nombre y el apellido son obligatorios.");
            if (nombre.Length > 100 || apellido.Length > 100) throw new InvalidOperationException("El nombre y el apellido no pueden superar los 100 caracteres.");

            string emailNuevo = NormalizarEmail(email);
            bool cambiaEmail = emailNuevo != objetivo.EmailUsuario;
            bool cambiaEmpresa = idEmpresa != objetivo.IdEmpresa;
            bool sensible = cambiaEmail || cambiaEmpresa;

            if (sensible) ExigirPatente(actor, Patentes_TLL.CAMBIAR_EMAIL_EMPRESA_USUARIO);

            if (cambiaEmail)
            {
                if (!EsEmailValido(emailNuevo)) throw new InvalidOperationException("El email no tiene un formato valido.");
                if (usuarioRepo.ObtenerPorEmail(emailNuevo) != null) throw new InvalidOperationException("Ya existe un usuario registrado con ese email.");
            }

            if (cambiaEmpresa)
            {
                if (!usuarioRepo.ExisteEmpresa(idEmpresa)) throw new InvalidOperationException("La empresa indicada no existe.");

                ExigirRolCompatibleConEmpresa(objetivo.Rol != null ? objetivo.Rol.Nombre : null, idEmpresa);

                if (EsAdministradorActivo(objetivo)) ExigirOtroAdministradorActivo(objetivo);
            }

            var cambios = new List<string>();
            if (nombre != objetivo.NombreUsuario) cambios.Add("nombre: " + objetivo.NombreUsuario + " → " + nombre);
            if (apellido != objetivo.ApellidoUsuario) cambios.Add("apellido: " + objetivo.ApellidoUsuario + " → " + apellido);
            if (idIdioma != objetivo.IdIdioma) cambios.Add("idioma: " + objetivo.IdIdioma + " → " + idIdioma);
            if (cambiaEmail) cambios.Add("correo: " + objetivo.EmailUsuario + " → " + emailNuevo);

            string empresaAnterior = usuarioRepo.ObtenerNombreEmpresa(objetivo.IdEmpresa);
            string empresaNueva = cambiaEmpresa ? usuarioRepo.ObtenerNombreEmpresa(idEmpresa) : empresaAnterior;
            if (cambiaEmpresa) cambios.Add("empresa: " + empresaAnterior + " → " + empresaNueva);

            if (cambios.Count == 0) throw new InvalidOperationException("No hay cambios para guardar.");

            bool otraEmpresa = actor.IdEmpresa != objetivo.IdEmpresa;

            if ((otraEmpresa || sensible) && !confirmado) throw new InvalidOperationException("Tenés que confirmar el aviso antes de guardar los cambios.");

            string motivoLimpio = ValidarMotivo(actor, objetivo.IdEmpresa, motivo);
            if (sensible && motivoLimpio.Length == 0) throw new InvalidOperationException("Indicá el motivo del cambio de correo o de empresa.");

            bool reemitirActivacion = cambiaEmail && objetivo.Estado == EstadoUsuario.Pendiente;

            return Transaccion_ORM.Ejecutar(() =>
            {
                usuarioRepo.ActualizarDatos(objetivo.IdUsuario, nombre, apellido, idIdioma, emailNuevo, idEmpresa);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { objetivo.IdUsuario.ToString() });

                string token = reemitirActivacion ? EmitirToken(objetivo.IdUsuario, TOKEN_ACTIVACION, VIGENCIA_ACTIVACION) : null;

                string detalle = "Modificación de datos del usuario '" + objetivo.EmailUsuario + "' (empresa " + empresaAnterior + "): " + string.Join("; ", cambios) + ConMotivo(motivoLimpio);

                Auditar(actor.IdUsuario, objetivo.IdEmpresa, detalle, sensible ? CriticidadBitacora.Alta : CriticidadBitacora.Media);

                if (cambiaEmpresa)
                    Auditar(actor.IdUsuario, idEmpresa, "Se incorporó a la empresa el usuario '" + emailNuevo + "' (venía de " + empresaAnterior + ")" + ConMotivo(motivoLimpio), CriticidadBitacora.Alta);

                return token;
            });
        }

        private Usuario_TE ObtenerObjetivo(int idUsuario)
        {
            var usuario = usuarioRepo.ObtenerPorPK(idUsuario);

            if (usuario == null) throw new InvalidOperationException("El usuario no existe.");

            return usuario;
        }

        private void ExigirPatente(ActorUsuario_TE actor, string patente)
        {
            if (actor == null || !actor.Puede(patente))
            {
                bitacora.Registrar(actor != null ? actor.IdUsuario : 0, "Seguridad", "Acción rechazada por falta de permiso (" + patente + ")", CriticidadBitacora.Media);
                throw new UnauthorizedAccessException("No tenés permiso para realizar esta acción.");
            }
        }

        private void ExigirAlcance(ActorUsuario_TE actor, int idEmpresaObjetivo, string accion)
        {
            if (actor.VeTodasLasEmpresas()) return;
            if (actor.IdEmpresa > 0 && actor.IdEmpresa == idEmpresaObjetivo) return;

            bitacora.Registrar(actor.IdUsuario, "Seguridad", "Intento de " + accion + " en otra empresa (empresa " + idEmpresaObjetivo + ")", CriticidadBitacora.Alta, idEmpresaObjetivo);
            throw new UnauthorizedAccessException("No tenés permiso sobre ese usuario.");
        }

        private static string ValidarMotivo(ActorUsuario_TE actor, int idEmpresaObjetivo, string motivo)
        {
            motivo = (motivo ?? string.Empty).Trim();

            if (motivo.Length > 300) throw new InvalidOperationException("El motivo no puede superar los 300 caracteres.");

            if (motivo.Length == 0 && actor.IdEmpresa != idEmpresaObjetivo) throw new InvalidOperationException("Indicá el motivo del cambio.");

            return motivo;
        }

        private static string ConMotivo(string motivo)
        {
            return motivo.Length == 0 ? string.Empty : ". Motivo: " + motivo;
        }

        private static bool EsAdministradorActivo(Usuario_TE usuario)
        {
            return usuario.Estado == EstadoUsuario.Activo && usuario.Rol != null && usuario.Rol.Nombre == ROL_ADMINISTRADOR;
        }

        private void ExigirOtroAdministradorActivo(Usuario_TE objetivo)
        {
            if (usuarioRepo.ContarAdministradoresActivos(objetivo.IdEmpresa, ROL_ADMINISTRADOR, objetivo.IdUsuario) == 0)
                throw new InvalidOperationException("No se puede: la empresa quedaría sin un administrador activo.");
        }

        private static string NombreEstado(EstadoUsuario estado)
        {
            switch (estado)
            {
                case EstadoUsuario.Pendiente: return "Pendiente de activación";
                case EstadoUsuario.Activo: return "Activo";
                case EstadoUsuario.BloqueadoPorIntentos: return "Bloqueado";
                case EstadoUsuario.Inactivo: return "Inactivo";
                case EstadoUsuario.BloqueoEstricto: return "Bloqueo estricto";
                default: return estado.ToString();
            }
        }

        // Estos eventos son de auditoría: si no se pueden guardar, el cambio completo se revierte.
        private void Auditar(int idActor, int idEmpresa, string descripcion, CriticidadBitacora criticidad)
        {
            bitacora.Guardar(new Bitacora_TE(idActor, "Usuarios", descripcion, criticidad, DateTime.Now) { IdEmpresa = idEmpresa });
        }

        // Marca que cambia cuando cambia la contraseña: las sesiones y las cookies de "recordarme" que la llevan dejan de valer.
        public string HuellaDeAcceso(Usuario_TE usuario)
        {
            return cifrador.Encoder(usuario.ContrasenaHashUsuario ?? string.Empty).Substring(0, 16);
        }

        public Usuario_TE ObtenerPorId(int idUsuario) => usuarioRepo.ObtenerPorPK(idUsuario);

        public Usuario_TE ObtenerPorEmail(string email) => usuarioRepo.ObtenerPorEmail(NormalizarEmail(email));

        public bool CambiarContrasena(string email, string contrasenaActual, string contrasenaNueva, out string error)
        {
            error = null;
            email = NormalizarEmail(email);

            var usuario = usuarioRepo.ObtenerPorEmail(email);

            if (usuario == null|| EstaBloqueado(usuario)|| string.IsNullOrEmpty(contrasenaActual)|| !VerificarContrasena(contrasenaActual, usuario.ContrasenaHashUsuario))
            {
                if (usuario != null && !EstaBloqueado(usuario))
                {
                    RegistrarIntentoFallido(usuario, "cambio de contraseña");
                }

                error = "No se pudo cambiar la contraseña. Verificá los datos ingresados.";
                return false;
            }

            if (!EsContrasenaAceptable(contrasenaNueva))
            {
                error = POLITICA_CONTRASENA;
                return false;
            }

            if (contrasenaNueva == contrasenaActual)
            {
                error = "La nueva contraseña no puede ser igual a la actual.";
                return false;
            }

            usuario.ContrasenaHashUsuario = cifrador.Encoder(contrasenaNueva);
            usuario.IntentosFallidosUsuario = 0;

            if (usuario.Estado == EstadoUsuario.Pendiente)
                usuario.Estado = EstadoUsuario.Activo;

            Transaccion_ORM.Ejecutar(() =>
            {
                usuarioRepo.Modificar(usuario);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { usuario.IdUsuario.ToString() });

                bitacora.Guardar(new Bitacora_TE(usuario.IdUsuario, "Seguridad", "Cambio de contraseña", CriticidadBitacora.Media, DateTime.Now));
            });

            return true;
        }

        public SolicitudEnlace_TLL SolicitarEnlace(string email)
        {
            var usuario = usuarioRepo.ObtenerPorEmail(NormalizarEmail(email));
            if (usuario == null) return null;

            if (usuario.Estado == EstadoUsuario.BloqueoEstricto || usuario.Estado == EstadoUsuario.Inactivo) return null;

            if (!EmpresaPermiteIngreso(usuario.IdEmpresa)) return null;

            string token = null;

            Transaccion_ORM.Ejecutar(() =>
            {
                token = EmitirToken(usuario.IdUsuario, TOKEN_RECUPERACION, VIGENCIA_RECUPERACION);
                bitacora.Registrar(usuario.IdUsuario, "Seguridad", "Solicitud de recuperación de contraseña", CriticidadBitacora.Baja);
            });

            return new SolicitudEnlace_TLL { Token = token, Nombre = usuario.NombreUsuario };
        }

        // Pattern Blue (la empresa proveedora) nunca se bloquea: desde ahí se administra todo.
        public bool EmpresaPermiteIngreso(int idEmpresa)
        {
            return MotivoEmpresaSinIngreso(idEmpresa) == null;
        }

        // null si la empresa deja entrar; si no, por qué no: bloqueada (sigue siendo cliente) o deshabilitada (dada de baja).
        public string MotivoEmpresaSinIngreso(int idEmpresa)
        {
            if (idEmpresa <= 0 || idEmpresa == BitacoraGestor_TLL.ID_EMPRESA_PROVEEDORA) return null;

            string estado = usuarioRepo.ObtenerEstadoEmpresa(idEmpresa);

            if (estado == null || string.Equals(estado, "activa", StringComparison.OrdinalIgnoreCase)) return null;

            return string.Equals(estado, "deshabilitada", StringComparison.OrdinalIgnoreCase) ? MOTIVO_EMPRESA_DESHABILITADA : MOTIVO_EMPRESA_BLOQUEADA;
        }

        private static bool EstaBloqueado(Usuario_TE usuario)
        {
            return usuario.Estado == EstadoUsuario.BloqueadoPorIntentos || usuario.Estado == EstadoUsuario.BloqueoEstricto;
        }

        public ResultadoToken_TLL ValidarTokenContrasena(string token)
        {
            return EvaluarToken(LeerToken(token));
        }

        public ResultadoToken_TLL EstablecerContrasenaConToken(string token, string contrasenaNueva)
        {
            var info = LeerToken(token);
            var validacion = EvaluarToken(info);
            if (!validacion.Exito) return validacion;

            if (!EsContrasenaAceptable(contrasenaNueva)) return ResultadoToken_TLL.Falla("CONTRASENA_DEBIL", validacion.Email);

            var usuario = usuarioRepo.ObtenerPorPK(info.IdUsuario);

            usuario.ContrasenaHashUsuario = cifrador.Encoder(contrasenaNueva);
            usuario.IntentosFallidosUsuario = 0;

            if (usuario.Estado == EstadoUsuario.Pendiente || usuario.Estado == EstadoUsuario.BloqueadoPorIntentos)
                usuario.Estado = EstadoUsuario.Activo;

            string via = info.Tipo == TOKEN_ACTIVACION ? "activación" : "recuperación";

            Transaccion_ORM.Ejecutar(() =>
            {
                usuarioRepo.Modificar(usuario);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { usuario.IdUsuario.ToString() });

                tokenRepo.MarcarUsado(info.IdToken);
                tokenRepo.InvalidarPendientes(usuario.IdUsuario, info.Tipo);
                gestorIntegridad.RecalcularTabla(TablasBD.Token);

                bitacora.Guardar(new Bitacora_TE(usuario.IdUsuario, "Seguridad", "Contraseña establecida mediante token de " + via + "; la cuenta queda activa", CriticidadBitacora.Media, DateTime.Now));
            });

            return ResultadoToken_TLL.Ok(usuario.EmailUsuario);
        }

        private TokenInfo LeerToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;
            return tokenRepo.ObtenerPorToken(token);
        }

        private ResultadoToken_TLL EvaluarToken(TokenInfo info)
        {
            if (info == null || (info.Tipo != TOKEN_ACTIVACION && info.Tipo != TOKEN_RECUPERACION))
            {
                return ResultadoToken_TLL.Falla("TOKEN_INVALIDO");
            }

            if (info.Usado)
            {
                return ResultadoToken_TLL.Falla("TOKEN_USADO");
            }

            if (info.FechaExpiracion.HasValue && info.FechaExpiracion.Value < DateTime.Now)
            {
                return ResultadoToken_TLL.Falla("TOKEN_EXPIRADO");
            }

            var usuario = usuarioRepo.ObtenerPorPK(info.IdUsuario);
            if (usuario == null) return ResultadoToken_TLL.Falla("TOKEN_INVALIDO");

            // Un enlace que se mandó antes de dar de baja o bloquear la cuenta ya no sirve.
            if (usuario.Estado == EstadoUsuario.Inactivo || usuario.Estado == EstadoUsuario.BloqueoEstricto) return ResultadoToken_TLL.Falla("TOKEN_INVALIDO");

            return ResultadoToken_TLL.Ok(usuario.EmailUsuario);
        }

        private string EmitirToken(int idUsuario, string tipo, TimeSpan vigencia, bool invalidarAnteriores = true)
        {
            string token = Cifrador_SECURITY.GenerarSecretoUrlSafe();
            var ahora = DateTime.Now;

            Transaccion_ORM.Ejecutar(() =>
            {
                if (invalidarAnteriores) tokenRepo.InvalidarPendientes(idUsuario, tipo);
                tokenRepo.Crear(idUsuario, token, tipo, ahora, ahora.Add(vigencia));
                gestorIntegridad.RecalcularTabla(TablasBD.Token);
            });

            return token;
        }

        // Mínimo 8 caracteres, con mayúscula, minúscula, número y un carácter especial (ni letra, ni número, ni espacio).
        public static bool EsContrasenaAceptable(string contrasena)
        {
            return !string.IsNullOrWhiteSpace(contrasena)
                && contrasena.Length >= LARGO_MINIMO_CONTRASENA
                && contrasena.Any(char.IsUpper)
                && contrasena.Any(char.IsLower)
                && contrasena.Any(char.IsDigit)
                && contrasena.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c));
        }

        private static string NormalizarEmail(string email)
        {
            return email == null ? null : email.Trim().ToLowerInvariant();
        }

        private static bool EsEmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            int arroba = email.IndexOf('@');
            if (arroba <= 0 || arroba != email.LastIndexOf('@')) return false;

            int punto = email.IndexOf('.', arroba);
            return punto > arroba + 1 && punto < email.Length - 1;
        }

        private void RegistrarIntentoFallido(Usuario_TE usuario, string contexto = "inicio de sesión")
        {
            usuario.IntentosFallidosUsuario++;

            bool seBloqueo = usuario.IntentosFallidosUsuario >= MAX_INTENTOS_FALLIDOS;
            if (seBloqueo) usuario.Estado = EstadoUsuario.BloqueadoPorIntentos;

            Transaccion_ORM.Ejecutar(() =>
            {
                if (seBloqueo) usuarioRepo.ActualizarEstado(usuario.IdUsuario, (int)usuario.Estado);

                usuarioRepo.ActualizarIntentosFallidos(usuario.IdUsuario, usuario.IntentosFallidosUsuario);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { usuario.IdUsuario.ToString() });

                if (seBloqueo)
                {
                    bitacora.Guardar(new Bitacora_TE(usuario.IdUsuario, "Seguridad", "Cuenta bloqueada por superar el máximo de intentos fallidos", CriticidadBitacora.Alta, DateTime.Now));
                }
                else
                {
                    bitacora.Guardar(new Bitacora_TE(usuario.IdUsuario, "Seguridad", "Intento fallido de " + contexto + " (" + usuario.IntentosFallidosUsuario + " de " + MAX_INTENTOS_FALLIDOS + ")", CriticidadBitacora.Baja, DateTime.Now));
                }
            });
        }

        private void ResetearIntentosFallidos(Usuario_TE usuario)
        {
            if (usuario.IntentosFallidosUsuario == 0) return;

            usuario.IntentosFallidosUsuario = 0;

            Transaccion_ORM.Ejecutar(() =>
            {
                usuarioRepo.ActualizarIntentosFallidos(usuario.IdUsuario, 0);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { usuario.IdUsuario.ToString() });
            });
        }

        private bool VerificarContrasena(string contrasenaPlana, string hashAlmacenado)
        {
            return cifrador.Encoder(contrasenaPlana) == hashAlmacenado;
        }

        // La cuenta de emergencia siempre entra; si la integridad está comprometida, también se la lleva a Dígito verificador.
        private bool HayInconsistenciasDeIntegridad()
        {
            try
            {
                return gestorIntegridad.VerificarIntegridadTodasLasTablas().Count > 0;
            }
            catch
            {
                return true;
            }
        }

        private bool EsCredencialDeEmergencia(string identificador, string contrasenaPlana)
        {
            string usuarioConfigurado = ConfigurationManager.AppSettings["FALKE_EMERGENCY_USER"];
            string hashConfigurado = ConfigurationManager.AppSettings["FALKE_EMERGENCY_HASH"];

            if (string.IsNullOrEmpty(usuarioConfigurado) || string.IsNullOrEmpty(hashConfigurado)) return false;

            if (identificador != usuarioConfigurado) return false;

            return cifrador.Encoder(contrasenaPlana) == hashConfigurado;
        }

        private Usuario_TE ConstruirUsuarioEmergenciaEnMemoria(string identificador)
        {
            return new Usuario_TE
            {
                IdUsuario = -1,
                NombreUsuario = "EMERGENCIA",
                ApellidoUsuario = string.Empty,
                EmailUsuario = identificador,
                Estado = EstadoUsuario.Activo,
                EsCuentaEmergencia = true
            };
        }

        private void LoguearAccesoEmergenciaAArchivo(string identificador)
        {
            try
            {
                string linea = $"{DateTime.Now:o} | ACCESO DE EMERGENCIA | {identificador}";
                string ruta = HttpContext.Current != null ? HttpContext.Current.Server.MapPath("~/App_Data/emergencia.log") : "emergencia.log";

                File.AppendAllText(ruta, linea + Environment.NewLine);
            }
            catch
            {
            }
        }
    }
}
