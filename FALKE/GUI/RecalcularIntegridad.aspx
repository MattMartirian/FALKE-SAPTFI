<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RecalcularIntegridad.aspx.cs" Inherits="GUI.RecalcularIntegridad" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Integridad - Recalcular DVH / DVV</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Integridad - Recalcular DVH / DVV</h2>
            <p>
                Recalcula el digito verificador de cada registro (DVH) y la firma de cada
                tabla controlada (DVV), y los almacena. Accion de administrador.
            </p>

            <p>
                <asp:Button ID="btnVerificar" runat="server" Text="Verificar estado actual" OnClick="btnVerificar_Click" />
                &nbsp;
                <asp:Button ID="btnRecalcular" runat="server" Text="Recalcular y almacenar" OnClick="btnRecalcular_Click" />
            </p>

            <p><asp:Label ID="lblMsg" runat="server" Font-Bold="true" /></p>

            <h3>Resumen del recalculo</h3>
            <div style="overflow-x:auto">
                <asp:GridView ID="gvResumen" runat="server" AutoGenerateColumns="true" Visible="false" />
            </div>

            <h3>Registros con problemas de integridad</h3>
            <div style="overflow-x:auto">
                <asp:GridView ID="gvInconsistencias" runat="server" AutoGenerateColumns="false" Visible="false">
                    <Columns>
                        <asp:BoundField HeaderText="Tabla" DataField="Tabla" />
                        <asp:BoundField HeaderText="Tipo" DataField="Tipo" />
                        <asp:BoundField HeaderText="Registro #" DataField="NumeroRegistro" />
                        <asp:BoundField HeaderText="Clave (PK)" DataField="ClaveRegistro" />
                        <asp:BoundField HeaderText="DVH almacenado" DataField="DvhAlmacenado" />
                        <asp:BoundField HeaderText="DVH recalculado" DataField="DvhRecalculado" />
                        <asp:BoundField HeaderText="Datos del registro" DataField="DatosLegibles" />
                        <asp:BoundField HeaderText="Detalle" DataField="Detalle" />
                    </Columns>
                </asp:GridView>
            </div>

            <hr />
            <p>
                <a href="MenuPruebas.aspx">Menu</a> |
                <a href="Logout.aspx">Cerrar sesion</a>
            </p>
        </div>
    </form>
</body>
</html>
