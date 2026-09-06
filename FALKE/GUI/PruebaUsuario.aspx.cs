using System;
using ORM;
using SERVICES;
using TE;
using TLL;

namespace GUI
{
    public partial class PruebaUsuario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnRecalcularIntegridad_Click(object sender, EventArgs e)
        {
            try
            {
                var gestorIntegridad = new GestorIntegridad_SERVICE();
                gestorIntegridad.RecalcularTodasLasTablas();

                lblResultado.Text = "Integridad recalculada correctamente para todas las tablas.";
            }
            catch (Exception ex)
            {
                lblResultado.Text = "Error al recalcular integridad: " + ex.Message;
            }
        }

        protected void btnCrearUsuario_Click(object sender, EventArgs e)
        {
            try
            {
                var Usuario_TLL = new Usuario_TLL();

                var nuevoUsuario = new Usuario_TE
                {
                    IdEmpresa = 1,
                    NombreUsuario = "Juan",
                    ApellidoUsuario = "Pérez",
                    EmailUsuario = "juan.perez." + DateTime.Now.Ticks + "@falke.com",
                    Rol = new PermisoCompuesto_TE("Usuario", true),
                    IdIdioma = 1,
                    EsCuentaEmergencia = false
                };

                string tokenActivacion = Usuario_TLL.RegistrarUsuario(nuevoUsuario);

                lblResultado.Text = "Usuario creado con id " + nuevoUsuario.IdUsuario +
                                     ", email " + nuevoUsuario.EmailUsuario +
                                     ". Token de activacion: " + tokenActivacion;
            }
            catch (Exception ex)
            {
                lblResultado.Text = "Error al crear usuario: " + ex.Message;
            }
        }

        protected void btnValidarLogin_Click(object sender, EventArgs e)
        {
            try
            {
                var Usuario_TLL = new Usuario_TLL();
                var resultado = Usuario_TLL.ValidarCredenciales("juan.perez@falke.com", "ContrasenaTemporal123");

                lblResultado.Text = resultado.Exito
                    ? "Login exitoso. Usuario: " + resultado.Usuario.NombreUsuario + " " + resultado.Usuario.ApellidoUsuario
                    : "Login fallido. Motivo: " + resultado.Motivo;
            }
            catch (Exception ex)
            {
                lblResultado.Text = "Error al validar login: " + ex.Message;
            }
        }
    }
}