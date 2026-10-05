using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using BE;
using BLL;
using SERVICES;
using TLL;

namespace GUI
{
    public partial class Dispositivos : System.Web.UI.Page
    {
        private const string CLAVE_AVISO = "dispositivos.aviso";
        private const string CLAVE_AVISO_MODELOS = "dispositivos.aviso.modelos";
        private static readonly CultureInfo Cultura = new CultureInfo("es-AR");

        private ActorUsuario_TLL actor;
        private Dictionary<int, List<Prestamo_BE>> historiales = new Dictionary<int, List<Prestamo_BE>>();
        private int idAbierto;

        // Entrar y consultar es VER_DISPOSITIVOS; cada acción pide además el suyo
        // (alta e inventario, asignación a empresas, o gestión del catálogo de modelos).
        protected bool PuedeAlta { get; private set; }
        protected bool PuedeAsignar { get; private set; }
        protected bool PuedeModelos { get; private set; }

        // El dispositivo sobre el que actúa la página: al entrar, el que llegó en la dirección (?id=); después, el que eligió
        // el botón del detalle (campo oculto). Un envío que no viene de un dispositivo (por ejemplo, Buscar) no lo conserva.
        private int Seleccionado
        {
            get
            {
                int id;

                string texto = IsPostBack ? hfDispositivo.Value : Request.QueryString["id"];

                return int.TryParse(texto, out id) && id > 0 ? id : 0;
            }
        }

        // Las listas se arman en Init: así reciben lo que se eligió antes de que corra el evento del botón.
        protected void Page_Init(object sender, EventArgs e)
        {
            actor = SesionActual_GUI.ObtenerActor();

            if (actor == null) return;

            if (!actor.Puede(Patentes_TLL.VER_DISPOSITIVOS))
            {
                Response.Redirect("SinPermiso.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                actor = null;
                return;
            }

            PuedeAlta = actor.Puede(Patentes_TLL.ALTA_DISPOSITIVO);
            PuedeAsignar = actor.Puede(Patentes_TLL.ASIGNAR_DISPOSITIVO);
            PuedeModelos = actor.Puede(Patentes_TLL.GESTIONAR_MODELOS_DISPOSITIVO);

            phBotonModelos.Visible = PuedeModelos;
            phBotonAlta.Visible = PuedeAlta;
            phBotonAsignar.Visible = PuedeAsignar;
            phModalModelos.Visible = PuedeModelos;
            phModalAlta.Visible = PuedeAlta;
            phModalEditar.Visible = PuedeAlta;
            phModalBaja.Visible = PuedeAlta;
            phAccionesOcultas.Visible = PuedeAlta;
            phModalAsignar.Visible = PuedeAsignar;
            phModalDevolver.Visible = PuedeAsignar;

            try
            {
                CargarListas();
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Dispositivos.Listas", ex);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (actor == null)
            {
                SesionActual_GUI.Exigir();
                return;
            }

            if (IsPostBack) return;

            string aviso = Session[CLAVE_AVISO] as string;
            Session.Remove(CLAVE_AVISO);

            if (!string.IsNullOrEmpty(aviso)) Avisar("aviso-exito", aviso);

            // Después de agregar, renombrar o dar de baja un modelo se vuelve a la ventana de modelos con el resultado.
            if (PuedeModelos && Request.QueryString["modelos"] == "1")
            {
                string avisoModelos = Session[CLAVE_AVISO_MODELOS] as string;
                Session.Remove(CLAVE_AVISO_MODELOS);

                if (!string.IsNullOrEmpty(avisoModelos)) AvisarModelos("aviso-exito", avisoModelos);

                AbrirModal("modalModelos");
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (actor == null) return;

            try
            {
                var bll = new Dispositivo_BLL();
                List<Dispositivo_BE> todos = bll.Listar(actor);

                litTotal.Text = todos.Count(d => d.Estado != EstadoDispositivo.DeBaja).ToString(CultureInfo.InvariantCulture);
                litPrestados.Text = todos.Count(d => d.Estado == EstadoDispositivo.EnUso).ToString(CultureInfo.InvariantCulture);
                litDisponibles.Text = todos.Count(d => d.Estado == EstadoDispositivo.Disponible).ToString(CultureInfo.InvariantCulture);
                litTaller.Text = todos.Count(d => d.Estado == EstadoDispositivo.EnMantenimiento).ToString(CultureInfo.InvariantCulture);

                List<Dispositivo_BE> filtrados = bll.Listar(actor, txtBuscar.Text, EstadoFiltro(), ModeloFiltro());

                historiales = bll.ObtenerHistorialCompleto(actor).GroupBy(p => p.IdDispositivo).ToDictionary(g => g.Key, g => g.ToList());
                idAbierto = Seleccionado;

                if (idAbierto > 0 && !todos.Any(d => d.IdDispositivo == idAbierto) && !pnlAviso.Visible)
                {
                    Avisar("aviso-peligro", "El dispositivo no existe.");
                    idAbierto = 0;
                }

                rptDispositivos.DataSource = filtrados;
                rptDispositivos.DataBind();
                phVacio.Visible = filtrados.Count == 0;

                if (PuedeModelos)
                {
                    List<ModeloDispositivo_BE> modelos = new ModeloDispositivo_BLL().Listar(actor);

                    rptModelos.DataSource = modelos;
                    rptModelos.DataBind();
                    phSinModelos.Visible = modelos.Count == 0;
                }
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar("Dispositivos.Listar", ex);
                Avisar("aviso-peligro", "No se pudo cargar el inventario de dispositivos.");
            }
        }

        private void CargarListas()
        {
            var bllListas = new Dispositivo_BLL();
            List<Dispositivo_BE> todos = bllListas.Listar(actor);
            List<ModeloDispositivo_BE> modelos = new ModeloDispositivo_BLL().Listar(actor);

            ddlFiltroEstado.Items.Clear();
            ddlFiltroEstado.Items.Add(new ListItem("Todos", string.Empty));
            foreach (EstadoDispositivo estado in Enum.GetValues(typeof(EstadoDispositivo)))
                ddlFiltroEstado.Items.Add(new ListItem(Etiqueta(estado), ((int)estado).ToString(CultureInfo.InvariantCulture)));

            ddlFiltroModelo.Items.Clear();
            ddlFiltroModelo.Items.Add(new ListItem("Todos", string.Empty));
            foreach (ModeloDispositivo_BE modelo in modelos)
                ddlFiltroModelo.Items.Add(new ListItem(modelo.Nombre, modelo.IdModelo.ToString(CultureInfo.InvariantCulture)));

            if (PuedeAlta)
            {
                // Un dispositivo nuevo solo puede ser de un modelo activo.
                ddlAltaModelo.Items.Clear();
                ddlAltaModelo.Items.Add(new ListItem(modelos.Any(m => m.Activo) ? "Elegí un modelo" : "No hay modelos cargados", "0"));
                foreach (ModeloDispositivo_BE modelo in modelos.Where(m => m.Activo))
                    ddlAltaModelo.Items.Add(new ListItem(modelo.Nombre, modelo.IdModelo.ToString(CultureInfo.InvariantCulture)));

                // Al editar también aparecen los dados de baja: un dispositivo puede conservar el suyo.
                ddlEdModelo.Items.Clear();
                foreach (ModeloDispositivo_BE modelo in modelos)
                    ddlEdModelo.Items.Add(new ListItem(modelo.Activo ? modelo.Nombre : modelo.Nombre + " (de baja)", modelo.IdModelo.ToString(CultureInfo.InvariantCulture)));
            }

            if (!PuedeAsignar) return;

            ddlAsigDispositivo.Items.Clear();
            foreach (var d in todos.Where(x => x.Estado == EstadoDispositivo.Disponible))
                ddlAsigDispositivo.Items.Add(new ListItem(d.NumeroSerie + " — " + d.Modelo, d.IdDispositivo.ToString(CultureInfo.InvariantCulture)));
            if (ddlAsigDispositivo.Items.Count == 0) ddlAsigDispositivo.Items.Add(new ListItem("No hay dispositivos disponibles", "0"));

            ddlAsigEmpresa.Items.Clear();
            foreach (Empresa_BE empresa in bllListas.EmpresasParaPrestar(actor))
            {
                int? limite = Dispositivo_BLL.LimiteDelPlan(empresa.PlanSuscripcion);
                string uso = limite.HasValue ? empresa.DispositivosPrestados + " de " + limite.Value : empresa.DispositivosPrestados + " en préstamo";

                ddlAsigEmpresa.Items.Add(new ListItem(empresa.NombreEmpresa + " — plan " + empresa.PlanSuscripcion + " (" + uso + ")", empresa.IdEmpresa.ToString(CultureInfo.InvariantCulture)));
            }
            if (ddlAsigEmpresa.Items.Count == 0) ddlAsigEmpresa.Items.Add(new ListItem("No hay empresas activas", "0"));
        }

        private EstadoDispositivo? EstadoFiltro()
        {
            int estado;

            if (int.TryParse(ddlFiltroEstado.SelectedValue, out estado) && Enum.IsDefined(typeof(EstadoDispositivo), estado))
                return (EstadoDispositivo)estado;

            return null;
        }

        private int? ModeloFiltro()
        {
            int modelo;

            return int.TryParse(ddlFiltroModelo.SelectedValue, out modelo) && modelo > 0 ? modelo : (int?)null;
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            // Buscar no es una acción sobre un dispositivo: no se vuelve a abrir el último que se tocó.
            hfDispositivo.Value = string.Empty;
        }

        // ---- Inventario

        protected void btnCrear_Click(object sender, EventArgs e)
        {
            if (actor == null) return;

            string serie = txtAltaSerie.Text.Trim();
            int idModelo;
            int.TryParse(ddlAltaModelo.SelectedValue, out idModelo);

            var dispositivo = new Dispositivo_BE
            {
                NumeroSerie = serie,
                IdModelo = idModelo,
                Firmware = txtAltaFirmware.Text,
                DriverRequerido = txtAltaDriver.Text
            };

            if (!Ejecutar(() => new Dispositivo_BLL().Crear(actor, dispositivo), "Dispositivos.Alta", "modalAltaDispositivo")) return;

            Dispositivo_BE creado = new Dispositivo_BLL().Listar(actor, serie).FirstOrDefault(d => d.NumeroSerie == serie);
            IrAlDetalle(creado != null ? creado.IdDispositivo : 0, "Se dio de alta el dispositivo " + serie + ".");
        }

        protected void btnAsignar_Click(object sender, EventArgs e)
        {
            if (actor == null) return;

            int idDispositivo, idEmpresa;

            if (!int.TryParse(ddlAsigDispositivo.SelectedValue, out idDispositivo) || idDispositivo <= 0 ||
                !int.TryParse(ddlAsigEmpresa.SelectedValue, out idEmpresa) || idEmpresa <= 0)
            {
                Fallar("Elegí un dispositivo disponible y una empresa.", "modalAsignar");
                return;
            }

            string empresa = ddlAsigEmpresa.SelectedItem.Text.Split(new[] { " — " }, StringSplitOptions.None)[0];

            if (!Ejecutar(() => new Dispositivo_BLL().AsignarAEmpresa(actor, idDispositivo, idEmpresa), "Dispositivos.Asignar", "modalAsignar")) return;

            IrAlDetalle(idDispositivo, "El dispositivo se prestó a «" + empresa + "».");
        }

        protected void btnDevolver_Click(object sender, EventArgs e)
        {
            if (actor == null) return;

            bool mantenimiento = chkRequiereMantenimiento.Checked;

            if (Ejecutar(() => new Dispositivo_BLL().RegistrarDevolucion(actor, Seleccionado, mantenimiento), "Dispositivos.Devolver", "modalDevolver"))
                Avisar("aviso-exito", mantenimiento ? "Se registró la devolución y el dispositivo pasó a mantenimiento." : "Se registró la devolución: el dispositivo está disponible.");
        }

        protected void btnMantenimiento_Click(object sender, EventArgs e)
        {
            if (actor == null) return;

            if (Ejecutar(() => new Dispositivo_BLL().EnviarAMantenimiento(actor, Seleccionado), "Dispositivos.Mantenimiento", null))
                Avisar("aviso-exito", "El dispositivo pasó a mantenimiento.");
        }

        protected void btnTerminar_Click(object sender, EventArgs e)
        {
            if (actor == null) return;

            if (Ejecutar(() => new Dispositivo_BLL().TerminarMantenimiento(actor, Seleccionado), "Dispositivos.TerminarMantenimiento", null))
                Avisar("aviso-exito", "El dispositivo terminó el mantenimiento y está disponible.");
        }

        protected void btnBaja_Click(object sender, EventArgs e)
        {
            if (actor == null) return;

            string motivo = txtBajaMotivo.Text;

            if (Ejecutar(() => new Dispositivo_BLL().DarDeBaja(actor, Seleccionado, motivo), "Dispositivos.Baja", "modalBaja"))
            {
                txtBajaMotivo.Text = string.Empty;
                Avisar("aviso-exito", "El dispositivo se dio de baja.");
            }
        }

        protected void btnGuardarEdicion_Click(object sender, EventArgs e)
        {
            if (actor == null) return;

            int idModelo;
            int.TryParse(ddlEdModelo.SelectedValue, out idModelo);

            var nuevos = new Dispositivo_BE
            {
                IdModelo = idModelo,
                Firmware = txtEdFirmware.Text,
                DriverRequerido = txtEdDriver.Text
            };

            if (Ejecutar(() => new Dispositivo_BLL().Modificar(actor, Seleccionado, nuevos), "Dispositivos.Modificar", "modalEditar"))
                Avisar("aviso-exito", "Los datos del dispositivo se actualizaron.");
        }

        // ---- Modelos

        protected void btnAgregarModelo_Click(object sender, EventArgs e)
        {
            if (actor == null || !PuedeModelos) return;

            string nombre = txtNuevoModelo.Text;

            EjecutarModelos(() => new ModeloDispositivo_BLL().Crear(actor, nombre), "Dispositivos.ModeloAlta", "Se agregó el modelo «" + nombre.Trim() + "».");
        }

        protected void rptModelos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (actor == null || !PuedeModelos) return;

            int idModelo;
            if (!int.TryParse(Convert.ToString(e.CommandArgument, CultureInfo.InvariantCulture), out idModelo)) return;

            var bll = new ModeloDispositivo_BLL();

            switch (e.CommandName)
            {
                case "renombrar":
                    string nombre = ((TextBox)e.Item.FindControl("txtNombreModelo")).Text;
                    EjecutarModelos(() => bll.Renombrar(actor, idModelo, nombre), "Dispositivos.ModeloRenombrar", "Se guardó el nuevo nombre del modelo.");
                    break;

                case "baja":
                    EjecutarModelos(() => bll.DarDeBaja(actor, idModelo), "Dispositivos.ModeloBaja", "El modelo se dio de baja: ya no se ofrece para dispositivos nuevos.");
                    break;

                case "reactivar":
                    EjecutarModelos(() => bll.Reactivar(actor, idModelo), "Dispositivos.ModeloReactivar", "El modelo se reactivó.");
                    break;
            }
        }

        // Si sale bien se vuelve a cargar la página (así las listas de modelos se arman de nuevo); si no, se muestra el motivo en la misma ventana.
        private void EjecutarModelos(Action accion, string origen, string exito)
        {
            try
            {
                accion();

                Session[CLAVE_AVISO_MODELOS] = exito;
                Response.Redirect("Dispositivos.aspx?modelos=1", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
            catch (UnauthorizedAccessException ex)
            {
                AvisarModelos("aviso-peligro", ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                AvisarModelos("aviso-peligro", ex.Message);
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar(origen, ex);
                AvisarModelos("aviso-peligro", "No se pudo completar la operación. Volvé a intentarlo.");
            }

            AbrirModal("modalModelos");
        }

        // ---- Auxiliares

        // Devuelve true si salió bien. Si falla, muestra el motivo y reabre el modal para no perder lo escrito.
        private bool Ejecutar(Action accion, string origen, string modal)
        {
            try
            {
                accion();
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                Fallar(ex.Message, modal);
            }
            catch (InvalidOperationException ex)
            {
                Fallar(ex.Message, modal);
            }
            catch (Exception ex)
            {
                LogErrores_SERVICE.Registrar(origen, ex);
                Fallar("No se pudo completar la operación. Volvé a intentarlo.", modal);
            }

            return false;
        }

        private void Fallar(string mensaje, string modal)
        {
            Avisar("aviso-peligro", mensaje);

            if (modal != null) AbrirModal(modal);
        }

        private void AbrirModal(string modal)
        {
            ClientScript.RegisterStartupScript(GetType(), "abrir" + modal,
                "window.addEventListener('load',function(){try{window.Falke.abrirModal('" + modal + "');}catch(e){}});", true);
        }

        private void IrAlDetalle(int idDispositivo, string aviso)
        {
            Session[CLAVE_AVISO] = aviso;
            Response.Redirect(idDispositivo > 0 ? "Dispositivos.aspx?id=" + idDispositivo : "Dispositivos.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void Avisar(string variante, string texto)
        {
            pnlAviso.CssClass = "aviso " + variante;
            litAviso.Text = Server.HtmlEncode(texto);
            pnlAviso.Visible = true;
        }

        private void AvisarModelos(string variante, string texto)
        {
            pnlAvisoModelos.CssClass = "aviso " + variante;
            litAvisoModelos.Text = Server.HtmlEncode(texto);
            pnlAvisoModelos.Visible = true;
        }

        // ---- Para las plantillas

        protected static string Fecha(DateTime fecha)
        {
            return fecha.ToString("d MMM yyyy", Cultura);
        }

        protected static string Fecha(DateTime? fecha)
        {
            return fecha.HasValue ? Fecha(fecha.Value) : "—";
        }

        protected static string Dato(string valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? "—" : valor;
        }

        protected static string Hasta(object item)
        {
            var p = (Prestamo_BE)item;

            return p.Abierto ? "En curso" : Fecha(p.FechaDevolucion);
        }

        protected static string Etiqueta(EstadoDispositivo estado)
        {
            return Dispositivo_BLL.NombreEstado(estado);
        }

        protected static string ClasePunto(EstadoDispositivo estado)
        {
            switch (estado)
            {
                case EstadoDispositivo.EnUso: return "enUso";
                case EstadoDispositivo.EnMantenimiento: return "mantenimiento";
                case EstadoDispositivo.DeBaja: return "baja";
                default: return "disponible";
            }
        }

        // Los dispositivos dados de baja se ven atenuados.
        protected static string ClaseFila(object item)
        {
            return ((Dispositivo_BE)item).Estado == EstadoDispositivo.DeBaja ? "fila-baja-dispositivo" : string.Empty;
        }

        protected bool EstaAbierto(object item)
        {
            return ((Dispositivo_BE)item).IdDispositivo == idAbierto;
        }

        protected List<Prestamo_BE> HistorialDe(object item)
        {
            List<Prestamo_BE> lista;

            return historiales.TryGetValue(((Dispositivo_BE)item).IdDispositivo, out lista) ? lista : new List<Prestamo_BE>();
        }

        protected bool MostrarAsignar(object item)
        {
            return PuedeAsignar && ((Dispositivo_BE)item).Estado == EstadoDispositivo.Disponible;
        }

        protected bool MostrarDevolver(object item)
        {
            return PuedeAsignar && ((Dispositivo_BE)item).Estado == EstadoDispositivo.EnUso;
        }

        protected bool MostrarMantenimiento(object item)
        {
            return PuedeAlta && ((Dispositivo_BE)item).Estado == EstadoDispositivo.Disponible;
        }

        protected bool MostrarTerminar(object item)
        {
            return PuedeAlta && ((Dispositivo_BE)item).Estado == EstadoDispositivo.EnMantenimiento;
        }

        protected bool MostrarEditar(object item)
        {
            return PuedeAlta && ((Dispositivo_BE)item).Estado != EstadoDispositivo.DeBaja;
        }

        protected bool MostrarBaja(object item)
        {
            EstadoDispositivo estado = ((Dispositivo_BE)item).Estado;

            return PuedeAlta && (estado == EstadoDispositivo.Disponible || estado == EstadoDispositivo.EnMantenimiento);
        }

        protected bool HayAcciones(object item)
        {
            return MostrarAsignar(item) || MostrarDevolver(item) || MostrarMantenimiento(item) || MostrarTerminar(item) || MostrarEditar(item) || MostrarBaja(item);
        }
    }
}
