<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Logout.aspx.cs" Inherits="GUI.Logout" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Cerrar sesion</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Cerrar sesion</h2>

            <p><asp:Label ID="lblEstado" runat="server" Font-Bold="true" /></p>

            <p>
                <asp:Button ID="btnLogout" runat="server" Text="Cerrar sesion" OnClick="btnLogout_Click" />
            </p>

            <hr />
            <p>
                <a href="Login.aspx">Ir al login</a> |
                <a href="MenuPruebas.aspx">Menu</a>
            </p>
        </div>
    </form>
</body>
</html>
