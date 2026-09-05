<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CambiarContrasena.aspx.cs" Inherits="GUI.CambiarContrasena" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Cambiar contrasena</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Cambiar contrasena</h2>
            <p>
                Cambia la contrasena del usuario con la sesion iniciada:
                <asp:Label ID="lblUsuario" runat="server" Font-Bold="true" />.
                Requiere la contrasena actual. La cuenta de emergencia no aplica aca.
            </p>

            <p>Contrasena actual:<br /><asp:TextBox ID="txtActual" runat="server" TextMode="Password" Width="300" /></p>
            <p>Contrasena nueva (minimo 8):<br /><asp:TextBox ID="txtNueva" runat="server" TextMode="Password" Width="300" /></p>
            <p>Repetir contrasena nueva:<br /><asp:TextBox ID="txtRepetir" runat="server" TextMode="Password" Width="300" /></p>

            <p>
                <asp:Button ID="btnCambiar" runat="server" Text="Cambiar contrasena" OnClick="btnCambiar_Click" />
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
