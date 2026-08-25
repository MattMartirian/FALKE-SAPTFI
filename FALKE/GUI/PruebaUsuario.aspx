<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PruebaUsuario.aspx.cs" Inherits="GUI.PruebaUsuario" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Prueba Usuario / Integridad</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Prueba de Usuario + Integridad</h2>

            <p>
                <asp:Button ID="btnRecalcularIntegridad" runat="server" Text="1. Recalcular integridad (correr una sola vez al inicio)" OnClick="btnRecalcularIntegridad_Click" />
            </p>
            <p>
                <asp:Button ID="btnCrearUsuario" runat="server" Text="2. Crear usuario de prueba" OnClick="btnCrearUsuario_Click" />
            </p>
            <p>
                <asp:Button ID="btnValidarLogin" runat="server" Text="3. Probar login" OnClick="btnValidarLogin_Click" />
            </p>

            <hr />

            <p>
                <asp:Label ID="lblResultado" runat="server" Text="" Font-Bold="true" />
            </p>
        </div>
    </form>
</body>
</html>