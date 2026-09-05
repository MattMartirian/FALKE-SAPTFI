<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EstablecerContrasena.aspx.cs" Inherits="GUI.EstablecerContrasena" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Definir contrasena</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Definir contrasena</h2>

            <asp:Panel ID="pnlForm" runat="server">
                <p>Cuenta: <asp:Label ID="lblEmail" runat="server" Font-Bold="true" /></p>

                <p>Contrasena nueva (minimo 8):<br />
                    <asp:TextBox ID="txtNueva" runat="server" TextMode="Password" Width="300" /></p>
                <p>Repetir contrasena nueva:<br />
                    <asp:TextBox ID="txtRepetir" runat="server" TextMode="Password" Width="300" /></p>

                <p>
                    <asp:Button ID="btnEstablecer" runat="server" Text="Guardar" OnClick="btnEstablecer_Click" />
                </p>
            </asp:Panel>

            <p><asp:Label ID="lblMsg" runat="server" Font-Bold="true" /></p>

            <hr />
            <p><a href="Login.aspx">Ir al login</a></p>
        </div>
    </form>
</body>
</html>
