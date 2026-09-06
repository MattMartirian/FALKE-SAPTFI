<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MenuPruebas.aspx.cs" Inherits="GUI.MenuPruebas" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Menu de pruebas - Seguridad</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Menu de pruebas - Seguridad</h2>

            <p><asp:Label ID="lblSesion" runat="server" Font-Bold="true" /></p>
            <p><asp:Label ID="lblAviso" runat="server" ForeColor="Red" Font-Bold="true" /></p>

            <ul>
                <li><a href="Login.aspx">Login</a></li>
                <li><a href="Logout.aspx">Logout</a></li>
                <li id="liRegistrarEmpresa" runat="server"><a href="RegistrarEmpresa.aspx">Registrar empresa (+ administrador) &ndash; requiere permiso</a></li>
                <li id="liRegistrarUsuario" runat="server"><a href="RegistrarUsuario.aspx">Registrar usuario &ndash; requiere permiso</a></li>
                <li><a href="CambiarContrasena.aspx">Cambiar contrasena &ndash; requiere sesion</a></li>
                <li><a href="RecuperarContrasena.aspx">Recuperar contrasena</a></li>
                <li id="liRecalcular" runat="server"><a href="RecalcularIntegridad.aspx">Recalcular integridad (DVH / DVV) &ndash; requiere permiso</a></li>
            </ul>

            <p>
                <small>
                    El alta y la recuperacion envian un mail con un enlace a
                    <b>EstablecerContrasena.aspx</b>; los mails se escriben como archivos en
                    <b>App_Data/mails/</b>. Los errores quedan en <b>App_Data/logs/errores.log</b>.
                </small>
            </p>
            <hr />
            <p><small>Acceso de emergencia (sin base de datos): usuario <b>admin</b> / contrasena <b>admin</b>.</small></p>
        </div>
    </form>
</body>
</html>
