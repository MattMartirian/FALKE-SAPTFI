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
                ExigirAlcance(actor, usuario.IdEmpresa, "dar de alta un usuario");

            if (!usuarioRepo.ExisteEmpresa(usuario.IdEmpresa)) throw new InvalidOperationException("La empresa indicada no existe.");

            string rol = usuario.Rol != null ? usuario.Rol.Nombre : null;
            if (!RolesAsignables(actor).Contains(rol)) throw new InvalidOperationException("Ese rol no se puede asignar desde tu cuenta.");

            ExigirRolCompatibleConEmpresa(rol, usuario.IdEmpresa);

            return RegistrarUsuarioInterno(usuario, actor.IdUsuario);
        }

        private string RegistrarUsuarioInterno(Usuario_TE usuario, int idActor)
        {
            usuario.EmailUsuario = TextoHelper_TLL.NormalizarEmail(usuario.EmailUsuario);

            if (!TextoHelper_TLL.EsEmailValido(usuario.EmailUsuario)) throw new InvalidOperationException("El email no tiene un formato valido.");

            if (usuarioRepo.ObtenerPorEmail(usuario.EmailUsuario) != null) throw new InvalidOperationException("Ya existe un usuario registrado con ese email.");

            usuario.ContrasenaHashUsuario = cifrador.Encoder(Cifrador_SECURITY.GenerarSecretoUrlSafe());
            usuario.IntentosFallidosUsuario = 0;
            usuario.Estado = EstadoUsuario.Pendiente;

            string rolNombre = usuario.Rol != null ? usuario.Rol.Nombre : "(sin rol)";

            string token = Transaccion_ORM.Ejecutar(() =>
            {
                usuarioRepo.Alta(usuario);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { usuario.IdUsuario.ToString() });

                string t = tokens.EmitirActivacion(usuario.IdUsuario);

                bitacora.Auditar(idActor > 0 ? idActor : usuario.IdUsuario, "Usuarios", usuario.IdEmpresa,
                    "Alta de usuario '" + usuario.EmailUsuario + "' (rol " + rolNombre + "); queda pendiente de activación", CriticidadBitacora.Media);

                return t;
            });

            return token;
        }

        public SolicitudEnlace_TLL ReenviarInvitacion(ActorUsuario_TE actor, int idUsuario)
        {
            actor.Exigir(Patentes_TLL.REGISTRAR_USUARIO);

            var objetivo = ObtenerObjetivo(idUsuario);
            ExigirAlcance(actor, objetivo.IdEmpresa, "reenviar una invitación");
            ExigirJerarquia(actor, objetivo, "reenviar la invitación a");

            if (objetivo.IdUsuario == actor.IdUsuario) throw new InvalidOperationException("No podés reenviarte una invitación a vos mismo.");

            if (objetivo.Estado != EstadoUsuario.Pendiente) throw new InvalidOperationException("La invitación solo se reenvía a una cuenta que sigue pendiente de activación.");

            string empresa = usuarioRepo.ObtenerNombreEmpresa(objetivo.IdEmpresa);

            string token = Transaccion_ORM.Ejecutar(() =>
            {
                string t = tokens.EmitirActivacion(objetivo.IdUsuario, false);

                bitacora.Auditar(actor.IdUsuario, "Usuarios", objetivo.IdEmpresa,
                    "Reenvío de la invitación al usuario '" + objetivo.EmailUsuario + "' (empresa " + empresa + "): se emitió un enlace de activación nuevo",
                    CriticidadBitacora.Media);

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
                if (actor.IdEmpresa <= 0) throw new UnauthorizedAccessException("La sesión no tiene una empresa asociada.");

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

                bitacora.Auditar(actor.IdUsuario, "Usuarios", objetivo.IdEmpresa,
                    "Cambio de estado del usuario '" + objetivo.EmailUsuario + "' (empresa " + empresa + "): " + anterior + " → " + nuevo + ConMotivo(motivoLimpio),
                    restringe ? CriticidadBitacora.Alta : CriticidadBitacora.Media);
            });
        }

        public void CambiarRol(ActorUsuario_TE actor, int idUsuario, string nuevoRol, string motivo, bool confirmado)
        {
            actor.Exigir(Patentes_TLL.CAMBIAR_ROL_USUARIO);

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

                bitacora.Auditar(actor.IdUsuario, "Usuarios", objetivo.IdEmpresa,
                    "Cambio de rol del usuario '" + objetivo.EmailUsuario + "' (empresa " + empresa + "): " + anterior + " → " + nuevoRol + ConMotivo(motivoLimpio),
                    CriticidadBitacora.Alta);
            });
        }

        private static void ExigirRolCompatibleConEmpresa(string nombreRol, int idEmpresaDelUsuario)
        {
            if (idEmpresaDelUsuario == BitacoraGestor_TLL.ID_EMPRESA_PROVEEDORA) return;

            if (EsRolDeGestion(nombreRol))
            {
                var etiquetas = new Permiso_TLL().ObtenerEtiquetas();

                throw new InvalidOperationException("El rol \"" + Permiso_TLL.Etiqueta(etiquetas, nombreRol) + "\" es un rol de gestión, solo para usuarios de Pattern Blue: no se puede asignar a usuarios de empresas cliente.");
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

            if (nombre.Length == 0 || apellido.Length == 0) throw new InvalidOperationException("El nombre y el apellido son obligatorios.");
            if (nombre.Length > 100 || apellido.Length > 100) throw new InvalidOperationException("El nombre y el apellido no pueden superar los 100 caracteres.");

            var usuario = ObtenerObjetivo(idUsuario);

            Transaccion_ORM.Ejecutar(() =>
            {
                usuarioRepo.ActualizarPerfil(usuario.IdUsuario, nombre, apellido, idIdioma);
                gestorIntegridad.ActualizarDVHRegistro(TablasBD.Usuario, new[] { usuario.IdUsuario.ToString() });

                bitacora.Auditar(usuario.IdUsuario, "Usuarios", usuario.IdEmpresa, "Modificación de los datos personales del usuario '" + usuario.EmailUsuario + "'", CriticidadBitacora.Baja);
            });
        }

        private void ExigirJerarquia(ActorUsuario_TE actor, Usuario_TE objetivo, string accion)
        {
            if (actor.EsEmergencia || objetivo.Rol == null) return;

            var propios = Permiso_TLL.ObtenerPatentes(actor.Permiso);

            if (Permiso_TLL.ObtenerPatentes(objetivo.Rol).Where(p => !Patentes_TLL.EsDeInfraestructura(p)).All(propios.Contains)) return;

            bitacora.Registrar(actor.IdUsuario, "Seguridad", "Intento de " + accion + " un usuario con más permisos que el suyo (usuario " + objetivo.IdUsuario + ")", CriticidadBitacora.Alta, objetivo.IdEmpresa);
            throw new UnauthorizedAccessException("No podés modificar a un usuario con más permisos que vos.");
        }

        public string ModificarDatosUsuario(ActorUsuario_TE actor, int idUsuario, string nombre, string apellido, int idIdioma, string email, int idEmpresa, string motivo, bool confirmado)
        {
            actor.Exigir(Patentes_TLL.MODIFICAR_USUARIO);

            var objetivo = ObtenerObjetivo(idUsuario);
            ExigirAlcance(actor, objetivo.IdEmpresa, "modificar los datos de un usuario");

            if (objetivo.IdUsuario == actor.IdUsuario) throw new InvalidOperationException("Tus propios datos se cambian desde «Mi perfil».");

            ExigirJerarquia(actor, objetivo, "modificar los datos de");

            nombre = (nombre ?? string.Empty).Trim();
            apellido = (apellido ?? string.Empty).Trim();

            if (nombre.Length == 0 || apellido.Length == 0) throw new InvalidOperationException("El nombre y el apellido son obligatorios.");
            if (nombre.Length > 100 || apellido.Length > 100) throw new InvalidOperationException("El nombre y el apellido no pueden superar los 100 caracteres.");

            string emailNuevo = TextoHelper_TLL.NormalizarEmail(email);
            bool cambiaEmail = emailNuevo != objetivo.EmailUsuario;
            bool cambiaEmpresa = idEmpresa != objetivo.IdEmpresa;
            bool sensible = cambiaEmail || cambiaEmpresa;

            if (sensible) actor.Exigir(Patentes_TLL.CAMBIAR_EMAIL_EMPRESA_USUARIO);

            if (cambiaEmail)
            {
                if (!TextoHelper_TLL.EsEmailValido(emailNuevo)) throw new InvalidOperationException("El email no tiene un formato valido.");
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

                string token = reemitirActivacion ? tokens.EmitirActivacion(objetivo.IdUsuario) : null;

                string detalle = "Modificación de datos del usuario '" + objetivo.EmailUsuario + "' (empresa " + empresaAnterior + "): " + string.Join("; ", cambios) + ConMotivo(motivoLimpio);

                bitacora.Auditar(actor.IdUsuario, "Usuarios", objetivo.IdEmpresa, detalle, sensible ? CriticidadBitacora.Alta : CriticidadBitacora.Media);

                if (cambiaEmpresa)
                    bitacora.Auditar(actor.IdUsuario, "Usuarios", idEmpresa, "Se incorporó a la empresa el usuario '" + emailNuevo + "' (venía de " + empresaAnterior + ")" + ConMotivo(motivoLimpio), CriticidadBitacora.Alta);

                return token;
            });
        }

        private Usuario_TE ObtenerObjetivo(int idUsuario)
        {
            var usuario = usuarioRepo.ObtenerPorPK(idUsuario);

            if (usuario == null) throw new InvalidOperationException("El usuario no existe.");

            return usuario;
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

        public Usuario_TE ObtenerPorId(int idUsuario) => usuarioRepo.ObtenerPorPK(idUsuario);
    }
}
