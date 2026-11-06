using ORM;
using SECURITY;
using SERVICES;
using System;
using System.Collections.Generic;
using System.Linq;
using TE;

namespace TLL
{
    // ABM de usuarios: alta por invitación, reenvío de la invitación, listado, cambio de estado, de rol y de datos, y el perfil propio.
    // Las demás responsabilidades están en Autenticacion_TLL (inicio de sesión), Contrasena_TLL (cambio y recuperación de contraseña),
    // TokenCuenta_TLL (enlaces por correo) y AccesoEmergencia_TLL (cuenta de emergencia).
    public class Usuario_TLL
    {
        public const string ROL_GESTOR = "Gestor";
        public const string ROL_WEBMASTER = "Webmaster";
        public const string ROL_ADMINISTRADOR = "Administrador";
        public const string ROL_ANALISTA = "Analista";

        private const string ACCION_ALTA_USUARIO = "bitacora_acc_alta_usuario";
        private const string ACCION_REENVIAR_INVITACION = "bitacora_acc_reenviar_invitacion";
        private const string ACCION_CAMBIAR_ESTADO = "bitacora_acc_cambiar_estado";
        private const string ACCION_CAMBIAR_ROL = "bitacora_acc_cambiar_rol";
        private const string ACCION_MODIFICAR_DATOS = "bitacora_acc_modificar_datos";

        private readonly UsuarioRepository usuarioRepo;
        private readonly Cifrador_SECURITY cifrador;
        private readonly GestorIntegridad_SERVICE gestorIntegridad;
        private readonly BitacoraGestor_TLL bitacora;
        private readonly TokenCuenta_TLL tokens;

        public Usuario_TLL()
        {
            usuarioRepo = new UsuarioRepository();
            cifrador = Cifrador_SECURITY.CifradorSingleton;
            gestorIntegridad = new GestorIntegridad_SERVICE();
            bitacora = new BitacoraGestor_TLL();
            tokens = new TokenCuenta_TLL();
        }

        public string RegistrarUsuario(ActorUsuario_TE actor, Usuario_TE usuario)
        {
            actor.Exigir(Patentes_TLL.REGISTRAR_USUARIO);

            if (!actor.Puede(Patentes_TLL.CREAR_USUARIO_OTRA_EMPRESA))
                ExigirAlcance(actor, usuario.IdEmpresa, ACCION_ALTA_USUARIO);

            if (!usuarioRepo.ExisteEmpresa(usuario.IdEmpresa)) throw new ExcepcionTraducible_TLL("usuario_empresa_no_existe");

            string rol = usuario.Rol != null ? usuario.Rol.Nombre : null;
            if (!RolesAsignables(actor).Contains(rol)) throw new ExcepcionTraducible_TLL("usuario_rol_no_asignable");

            ExigirRolCompatibleConEmpresa(rol, usuario.IdEmpresa);

            return RegistrarUsuarioInterno(usuario, actor.IdUsuario);
        }

        private string RegistrarUsuarioInterno(Usuario_TE usuario, int idActor)
        {
            usuario.EmailUsuario = TextoHelper_TLL.NormalizarEmail(usuario.EmailUsuario);

            if (!TextoHelper_TLL.EsEmailValido(usuario.EmailUsuario)) throw new ExcepcionTraducible_TLL("usuario_email_invalido");

            if (usuarioRepo.ObtenerPorEmail(usuario.EmailUsuario) != null) throw new ExcepcionTraducible_TLL("usuario_email_duplicado");

            usuario.ContrasenaHashUsuario = cifrador.Encoder(Cifrador_SECURITY.GenerarSecretoUrlSafe());
            usuario.IntentosFallidosUsuario = 0;
            usuario.Estado = EstadoUsuario.Pendiente;


            string token = Transaccion_ORM.Ejecutar(() =>
            {
                usuarioRepo.Alta(usuario);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { usuario.IdUsuario.ToString() });

                string t = tokens.EmitirActivacion(usuario.IdUsuario);

                bitacora.Auditar(idActor > 0 ? idActor : usuario.IdUsuario, "Usuarios", usuario.IdEmpresa, EventosBitacora_TLL.USUARIO_ALTA, CriticidadBitacora.Media,
                    DetalleBitacora_TLL.Armar(usuario.EmailUsuario));

                return t;
            });

            return token;
        }

        public SolicitudEnlace_TLL ReenviarInvitacion(ActorUsuario_TE actor, int idUsuario)
        {
            actor.Exigir(Patentes_TLL.REGISTRAR_USUARIO);

            var objetivo = ObtenerObjetivo(idUsuario);
            ExigirAlcance(actor, objetivo.IdEmpresa, ACCION_REENVIAR_INVITACION);
            ExigirJerarquia(actor, objetivo, ACCION_REENVIAR_INVITACION);

            if (objetivo.IdUsuario == actor.IdUsuario) throw new ExcepcionTraducible_TLL("usuario_no_reenviar_a_si_mismo");

            if (objetivo.Estado != EstadoUsuario.Pendiente) throw new ExcepcionTraducible_TLL("usuario_reenvio_solo_pendiente");

            string token = Transaccion_ORM.Ejecutar(() =>
            {
                string t = tokens.EmitirActivacion(objetivo.IdUsuario, false);

                bitacora.Auditar(actor.IdUsuario, "Usuarios", objetivo.IdEmpresa, EventosBitacora_TLL.USUARIO_INVITACION_REENVIADA, CriticidadBitacora.Media,
                    DetalleBitacora_TLL.Armar(objetivo.EmailUsuario));

                return t;
            });

            return new SolicitudEnlace_TLL { Token = token, Nombre = objetivo.NombreUsuario, Email = objetivo.EmailUsuario };
        }

        public PaginaUsuarios_TE ListarUsuarios(ActorUsuario_TE actor, FiltroUsuarios_TE filtro)
        {
            actor.Exigir(Patentes_TLL.VER_USUARIOS);

            var f = (filtro ?? new FiltroUsuarios_TE()).Copiar();

            if (!actor.VeTodasLasEmpresas())
            {
                if (actor.IdEmpresa <= 0) throw new ExcepcionNoAutorizada_TLL("comun_sesion_sin_empresa");

                f.IdEmpresa = actor.IdEmpresa;
            }

            return usuarioRepo.Listar(f);
        }

        public List<string> RolesAsignables(ActorUsuario_TE actor)
        {
            var roles = new PermisoRepository().ConstruirArbolDeRoles();

            if (actor.VeTodasLasEmpresas()) return roles.Select(r => r.Nombre).OrderBy(n => n).ToList();

            HashSet<string> propios = Permiso_TLL.ObtenerPatentes(actor.Permiso);

            return roles.Where(r => Permiso_TLL.ObtenerPatentes(r).IsSubsetOf(propios)).Select(r => r.Nombre).OrderBy(n => n).ToList();
        }

        public void CambiarEstado(ActorUsuario_TE actor, int idUsuario, EstadoUsuario nuevoEstado, string motivo)
        {
            actor.Exigir(Patentes_TLL.CAMBIAR_ESTADO_USUARIO);

            var objetivo = ObtenerObjetivo(idUsuario);
            ExigirAlcance(actor, objetivo.IdEmpresa, ACCION_CAMBIAR_ESTADO);
            ExigirJerarquia(actor, objetivo, ACCION_CAMBIAR_ESTADO);

            if (objetivo.IdUsuario == actor.IdUsuario) throw new ExcepcionTraducible_TLL("usuario_no_cambiar_propio_estado");
            if (nuevoEstado == objetivo.Estado) throw new ExcepcionTraducible_TLL("usuario_ya_en_ese_estado");

            if (nuevoEstado != EstadoUsuario.Activo && nuevoEstado != EstadoUsuario.Inactivo && nuevoEstado != EstadoUsuario.BloqueoEstricto)
                throw new ExcepcionTraducible_TLL("usuario_estado_no_manual");

            if (objetivo.Estado == EstadoUsuario.Pendiente && nuevoEstado == EstadoUsuario.Activo)
                throw new ExcepcionTraducible_TLL("usuario_cuenta_no_activada");

            string motivoLimpio = ValidarMotivo(actor, objetivo.IdEmpresa, motivo);

            if (EsAdministradorActivo(objetivo) && nuevoEstado != EstadoUsuario.Activo) ExigirOtroAdministradorActivo(objetivo);

            string evento = nuevoEstado == EstadoUsuario.Activo ? EventosBitacora_TLL.USUARIO_ACTIVACION
                : nuevoEstado == EstadoUsuario.Inactivo ? EventosBitacora_TLL.USUARIO_BAJA
                : EventosBitacora_TLL.USUARIO_BLOQUEO;
            bool restringe = nuevoEstado == EstadoUsuario.Inactivo || nuevoEstado == EstadoUsuario.BloqueoEstricto;
            Transaccion_ORM.Ejecutar(() =>
            {
                usuarioRepo.ActualizarEstado(objetivo.IdUsuario, (int)nuevoEstado);

                if (nuevoEstado == EstadoUsuario.Activo) usuarioRepo.ActualizarIntentosFallidos(objetivo.IdUsuario, 0);

                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { objetivo.IdUsuario.ToString() });

                bitacora.Auditar(actor.IdUsuario, "Usuarios", objetivo.IdEmpresa, evento, restringe ? CriticidadBitacora.Alta : CriticidadBitacora.Media,
                    DetalleBitacora_TLL.Armar(objetivo.EmailUsuario, DetalleBitacora_TLL.Motivo(motivoLimpio)));
            });
        }

        public void CambiarRol(ActorUsuario_TE actor, int idUsuario, string nuevoRol, string motivo, bool confirmado)
        {
            actor.Exigir(Patentes_TLL.CAMBIAR_ROL_USUARIO);

            var objetivo = ObtenerObjetivo(idUsuario);
            ExigirAlcance(actor, objetivo.IdEmpresa, ACCION_CAMBIAR_ROL);
            ExigirJerarquia(actor, objetivo, ACCION_CAMBIAR_ROL);

            if (objetivo.IdUsuario == actor.IdUsuario) throw new ExcepcionTraducible_TLL("usuario_no_cambiar_propio_rol");

            nuevoRol = (nuevoRol ?? string.Empty).Trim();
            if (!RolesAsignables(actor).Contains(nuevoRol)) throw new ExcepcionTraducible_TLL("usuario_rol_no_asignable");

            string anterior = objetivo.Rol != null ? objetivo.Rol.Nombre : null;
            if (anterior == nuevoRol) throw new ExcepcionTraducible_TLL("usuario_ya_tiene_rol");

            ExigirRolCompatibleConEmpresa(nuevoRol, objetivo.IdEmpresa);

            bool tocaOtraEmpresa = actor.IdEmpresa != objetivo.IdEmpresa;
            if ((tocaOtraEmpresa || EsRolDeGestion(nuevoRol)) && !confirmado)
                throw new ExcepcionTraducible_TLL("usuario_confirmar_aviso_rol");

            string motivoLimpio = ValidarMotivo(actor, objetivo.IdEmpresa, motivo);

            if (EsAdministradorActivo(objetivo) && nuevoRol != ROL_ADMINISTRADOR) ExigirOtroAdministradorActivo(objetivo);

            Transaccion_ORM.Ejecutar(() =>
            {
                usuarioRepo.ActualizarRol(objetivo.IdUsuario, nuevoRol);

                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { objetivo.IdUsuario.ToString() });

                bitacora.Auditar(actor.IdUsuario, "Usuarios", objetivo.IdEmpresa, EventosBitacora_TLL.USUARIO_CAMBIO_ROL, CriticidadBitacora.Alta,
                    DetalleBitacora_TLL.Armar(objetivo.EmailUsuario, " → ", nuevoRol, DetalleBitacora_TLL.Motivo(motivoLimpio)));
            });
        }

        private static void ExigirRolCompatibleConEmpresa(string nombreRol, int idEmpresaDelUsuario)
        {
            if (idEmpresaDelUsuario == BitacoraGestor_TLL.ID_EMPRESA_PROVEEDORA) return;

            if (EsRolDeGestion(nombreRol))
            {
                var etiquetas = new Permiso_TLL().ObtenerEtiquetas();

                throw new ExcepcionTraducible_TLL("usuario_rol_gestion_solo_pb", Permiso_TLL.Etiqueta(etiquetas, nombreRol));
            }
        }

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

            if (nombre.Length == 0 || apellido.Length == 0) throw new ExcepcionTraducible_TLL("usuario_nombre_apellido_obligatorios");
            if (nombre.Length > 100 || apellido.Length > 100) throw new ExcepcionTraducible_TLL("usuario_nombre_apellido_largos");

            var usuario = ObtenerObjetivo(idUsuario);

            Transaccion_ORM.Ejecutar(() =>
            {
                usuarioRepo.ActualizarPerfil(usuario.IdUsuario, nombre, apellido, idIdioma);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { usuario.IdUsuario.ToString() });

                bitacora.Auditar(usuario.IdUsuario, "Usuarios", usuario.IdEmpresa, EventosBitacora_TLL.USUARIO_MODIFICACION, CriticidadBitacora.Baja, DetalleBitacora_TLL.Armar(usuario.EmailUsuario));
            });
        }

        public void CambiarIdioma(int idUsuario, int idIdioma)
        {
            var usuario = ObtenerObjetivo(idUsuario);

            Transaccion_ORM.Ejecutar(() =>
            {
                usuarioRepo.ActualizarPerfil(usuario.IdUsuario, usuario.NombreUsuario, usuario.ApellidoUsuario, idIdioma);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { usuario.IdUsuario.ToString() });
            });
        }

        private void ExigirJerarquia(ActorUsuario_TE actor, Usuario_TE objetivo, string accion)
        {
            if (actor.EsEmergencia || objetivo.Rol == null) return;

            var propios = Permiso_TLL.ObtenerPatentes(actor.Permiso);

            if (Permiso_TLL.ObtenerPatentes(objetivo.Rol).Where(p => !Patentes_TLL.EsDeInfraestructura(p)).All(propios.Contains)) return;

            bitacora.Registrar(actor.IdUsuario, "Seguridad", EventosBitacora_TLL.INTENTO_USUARIO_MAS_PERMISOS, CriticidadBitacora.Alta, objetivo.IdEmpresa,
                DetalleBitacora_TLL.Armar(DetalleBitacora_TLL.Clave(accion), ": ", objetivo.EmailUsuario));
            throw new ExcepcionNoAutorizada_TLL("usuario_no_modificar_con_mas_permisos");
        }

        public string ModificarDatosUsuario(ActorUsuario_TE actor, int idUsuario, string nombre, string apellido, int idIdioma, string email, int idEmpresa, string motivo, bool confirmado)
        {
            actor.Exigir(Patentes_TLL.MODIFICAR_USUARIO);

            var objetivo = ObtenerObjetivo(idUsuario);
            ExigirAlcance(actor, objetivo.IdEmpresa, ACCION_MODIFICAR_DATOS);

            if (objetivo.IdUsuario == actor.IdUsuario) throw new ExcepcionTraducible_TLL("usuario_propios_datos_en_perfil");

            ExigirJerarquia(actor, objetivo, ACCION_MODIFICAR_DATOS);

            nombre = (nombre ?? string.Empty).Trim();
            apellido = (apellido ?? string.Empty).Trim();

            if (nombre.Length == 0 || apellido.Length == 0) throw new ExcepcionTraducible_TLL("usuario_nombre_apellido_obligatorios");
            if (nombre.Length > 100 || apellido.Length > 100) throw new ExcepcionTraducible_TLL("usuario_nombre_apellido_largos");

            string emailNuevo = TextoHelper_TLL.NormalizarEmail(email);
            bool cambiaEmail = emailNuevo != objetivo.EmailUsuario;
            bool cambiaEmpresa = idEmpresa != objetivo.IdEmpresa;
            bool sensible = cambiaEmail || cambiaEmpresa;

            if (sensible) actor.Exigir(Patentes_TLL.CAMBIAR_EMAIL_EMPRESA_USUARIO);

            if (cambiaEmail)
            {
                if (!TextoHelper_TLL.EsEmailValido(emailNuevo)) throw new ExcepcionTraducible_TLL("usuario_email_invalido");
                if (usuarioRepo.ObtenerPorEmail(emailNuevo) != null) throw new ExcepcionTraducible_TLL("usuario_email_duplicado");
            }

            if (cambiaEmpresa)
            {
                if (!usuarioRepo.ExisteEmpresa(idEmpresa)) throw new ExcepcionTraducible_TLL("usuario_empresa_no_existe");

                ExigirRolCompatibleConEmpresa(objetivo.Rol != null ? objetivo.Rol.Nombre : null, idEmpresa);

                if (EsAdministradorActivo(objetivo)) ExigirOtroAdministradorActivo(objetivo);
            }

            var cambios = new List<ParteBitacora_TLL>();
            if (nombre != objetivo.NombreUsuario) cambios.Add(DetalleBitacora_TLL.Clave("bitacora_campo_nombre"));
            if (apellido != objetivo.ApellidoUsuario) cambios.Add(DetalleBitacora_TLL.Clave("bitacora_campo_apellido"));
            if (idIdioma != objetivo.IdIdioma) cambios.Add(DetalleBitacora_TLL.Clave("bitacora_campo_idioma"));
            if (cambiaEmail) cambios.Add(DetalleBitacora_TLL.Clave("bitacora_campo_correo"));

            if (cambiaEmpresa) cambios.Add(DetalleBitacora_TLL.Clave("bitacora_campo_empresa"));

            if (cambios.Count == 0) throw new ExcepcionTraducible_TLL("comun_sin_cambios");

            bool otraEmpresa = actor.IdEmpresa != objetivo.IdEmpresa;

            if ((otraEmpresa || sensible) && !confirmado) throw new ExcepcionTraducible_TLL("usuario_confirmar_aviso_guardar");

            string motivoLimpio = ValidarMotivo(actor, objetivo.IdEmpresa, motivo);
            if (sensible && motivoLimpio.Length == 0) throw new ExcepcionTraducible_TLL("usuario_motivo_correo_empresa");

            bool reemitirActivacion = cambiaEmail && objetivo.Estado == EstadoUsuario.Pendiente;

            return Transaccion_ORM.Ejecutar(() =>
            {
                usuarioRepo.ActualizarDatos(objetivo.IdUsuario, nombre, apellido, idIdioma, emailNuevo, idEmpresa);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { objetivo.IdUsuario.ToString() });

                string token = reemitirActivacion ? tokens.EmitirActivacion(objetivo.IdUsuario) : null;

                string detalle = DetalleBitacora_TLL.Armar(objetivo.EmailUsuario, ": ", DetalleBitacora_TLL.Lista(cambios, ", "), DetalleBitacora_TLL.Motivo(motivoLimpio));

                bitacora.Auditar(actor.IdUsuario, "Usuarios", objetivo.IdEmpresa, EventosBitacora_TLL.USUARIO_MODIFICACION, sensible ? CriticidadBitacora.Alta : CriticidadBitacora.Media, detalle);

                if (cambiaEmpresa)
                    bitacora.Auditar(actor.IdUsuario, "Usuarios", idEmpresa, EventosBitacora_TLL.USUARIO_INCORPORADO, CriticidadBitacora.Alta,
                        DetalleBitacora_TLL.Armar(emailNuevo, DetalleBitacora_TLL.Motivo(motivoLimpio)));

                return token;
            });
        }

        private Usuario_TE ObtenerObjetivo(int idUsuario)
        {
            var usuario = usuarioRepo.ObtenerPorPK(idUsuario);

            if (usuario == null) throw new ExcepcionTraducible_TLL("usuario_no_existe");

            return usuario;
        }

        private void ExigirAlcance(ActorUsuario_TE actor, int idEmpresaObjetivo, string accion)
        {
            if (actor.VeTodasLasEmpresas()) return;
            if (actor.IdEmpresa > 0 && actor.IdEmpresa == idEmpresaObjetivo) return;

            bitacora.Registrar(actor.IdUsuario, "Seguridad", EventosBitacora_TLL.INTENTO_OTRA_EMPRESA, CriticidadBitacora.Alta, idEmpresaObjetivo,
                DetalleBitacora_TLL.Armar(DetalleBitacora_TLL.Clave(accion), " (", idEmpresaObjetivo, ")"));
            throw new ExcepcionNoAutorizada_TLL("usuario_sin_permiso_sobre");
        }

        private static string ValidarMotivo(ActorUsuario_TE actor, int idEmpresaObjetivo, string motivo)
        {
            motivo = (motivo ?? string.Empty).Trim();

            if (motivo.Length > 300) throw new ExcepcionTraducible_TLL("comun_motivo_largo_300");

            if (motivo.Length == 0 && actor.IdEmpresa != idEmpresaObjetivo) throw new ExcepcionTraducible_TLL("comun_motivo_obligatorio");

            return motivo;
        }

        private static bool EsAdministradorActivo(Usuario_TE usuario)
        {
            return usuario.Estado == EstadoUsuario.Activo && usuario.Rol != null && usuario.Rol.Nombre == ROL_ADMINISTRADOR;
        }

        private void ExigirOtroAdministradorActivo(Usuario_TE objetivo)
        {
            if (usuarioRepo.ContarAdministradoresActivos(objetivo.IdEmpresa, ROL_ADMINISTRADOR, objetivo.IdUsuario) == 0)
                throw new ExcepcionTraducible_TLL("usuario_empresa_sin_admin");
        }

        public Usuario_TE ObtenerPorId(int idUsuario) => usuarioRepo.ObtenerPorPK(idUsuario);
    }
}
