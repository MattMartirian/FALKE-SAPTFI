<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BitacoraTest.aspx.cs" Inherits="BitacoraTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Test de Bitácora</title>
</head>

<body>
    <form id="form1" runat="server">

        <div>

            <asp:Button
                ID="btnGenerarEntrada"
                runat="server"
                Text="Generar entrada"
                OnClick="btnGenerarEntrada_Click" />

            <asp:Button
                ID="btnLeerEntradas"
                runat="server"
                Text="Leer entradas"
                OnClick="btnLeerEntradas_Click" />

            <br />
            <br />

            <asp:GridView
                ID="gvBitacora"
                runat="server"
                AutoGenerateColumns="true">
            </asp:GridView>

        </div>

    </form>
</body>
</html>