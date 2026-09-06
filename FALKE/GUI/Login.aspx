<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="GUI.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Iniciar sesion</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Iniciar sesion</h2>

            <p>
                Email:<br />
                <asp:TextBox ID="txtEmail" runat="server" Width="300" />
            </p>
            <p>
                Contrasena:<br />
                <asp:TextBox ID="txtPass" runat="server" TextMode="Password" Width="300" />
            </p>
            <p>
                <asp:Button ID="btnIngresar" runat="server" Text="Ingresar" OnClick="btnIngresar_Click" />
                <asp:Button ID="Button1" runat="server" Text="Ingresar" OnClick="btn1_Click" />
            </p>

            <p><asp:Label ID="lblMsg" runat="server" ForeColor="Red" Font-Bold="true" /></p>

            <hr />
            <p>
                <a href="RecuperarContrasena.aspx">Recuperar contrasena</a> |
                <a href="CambiarContrasena.aspx">Cambiar contrasena</a> |
                <a href="MenuPruebas.aspx">Menu</a>
            </p>
            <p><small>El inicio de sesion es por <b>email</b>. Unica excepcion: acceso de emergencia
                con usuario <b>admin</b> / contrasena <b>admin</b>.</small></p>
        </div>
    </form>
</body>
</html>
