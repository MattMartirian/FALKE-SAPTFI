<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RegistrarUsuario.aspx.cs" Inherits="GUI.RegistrarUsuario" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Registrar usuario (administrador)</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Registrar usuario (administrador)</h2>
            <p>
                En este sistema los usuarios no se registran solos: un administrador da el alta
                y el usuario recibe un mail con un enlace para activar la cuenta y definir su
                contrasena. La cuenta queda en estado <b>Pendiente</b> hasta que la active.
            </p>

            <p>Id empresa (el Gestor puede elegirla; el administrador de una empresa queda fijado a la suya):<br />
                <asp:TextBox ID="txtEmpresa" runat="server" Text="1" /></p>
            <p>Nombre:<br /><asp:TextBox ID="txtNombre" runat="server" Width="300" /></p>
            <p>Apellido:<br /><asp:TextBox ID="txtApellido" runat="server" Width="300" /></p>
            <p>Email:<br /><asp:TextBox ID="txtEmail" runat="server" Width="300" /></p>
            <p>Rol (debe existir en PermisoTable, p. ej. Usuario o Administrador):<br />
                <asp:TextBox ID="txtRol" runat="server" Text="Usuario" /></p>
            <p>Id idioma:<br /><asp:TextBox ID="txtIdioma" runat="server" Text="1" /></p>

            <p>
                <asp:Button ID="btnRegistrar" runat="server" Text="Registrar y enviar activacion" OnClick="btnRegistrar_Click" />
            </p>

            <p><asp:Label ID="lblMsg" runat="server" Font-Bold="true" /></p>

            <hr />
            <p>
                <a href="MenuPruebas.aspx">Menu</a> |
                <a href="Logout.aspx">Cerrar sesion</a>
            </p>
        </div>
    </form>
</body>
</html>
