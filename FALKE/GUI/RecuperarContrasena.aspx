<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RecuperarContrasena.aspx.cs" Inherits="GUI.RecuperarContrasena" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Recuperar contrasena</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Recuperar contrasena</h2>
            <p>
                Te enviamos por mail un enlace temporal para definir una nueva contrasena.
                Tu contrasena actual sigue siendo valida hasta que uses ese enlace.
            </p>

            <p>Email:<br /><asp:TextBox ID="txtEmail" runat="server" Width="300" /></p>

            <p>
                <asp:Button ID="btnRecuperar" runat="server" Text="Enviar enlace" OnClick="btnRecuperar_Click" />
            </p>

            <p><asp:Label ID="lblMsg" runat="server" Font-Bold="true" /></p>

            <hr />
            <p>
                <a href="Login.aspx">Ir al login</a> |
                <a href="MenuPruebas.aspx">Menu</a>
            </p>
        </div>
    </form>
</body>
</html>
