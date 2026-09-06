<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RegistrarEmpresa.aspx.cs" Inherits="GUI.RegistrarEmpresa" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Registrar empresa</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Registrar empresa</h2>
            <p>
                Al crear la empresa se registra tambien su <b>usuario administrador</b> (estado
                <b>Pendiente</b>): recibe un mail con un enlace para activar la cuenta y definir su
                contrasena. Ese administrador es quien despues da de alta a los empleados de la empresa.
            </p>

            <h3>Datos de la empresa</h3>
            <p>Nombre:<br /><asp:TextBox ID="txtNombreEmpresa" runat="server" Width="300" /></p>
            <p>Numero de contacto:<br /><asp:TextBox ID="txtContacto" runat="server" Width="300" /></p>
            <p>Plan de suscripcion:<br /><asp:TextBox ID="txtPlan" runat="server" Width="300" /></p>

            <h3>Usuario administrador de la empresa</h3>
            <p>Nombre:<br /><asp:TextBox ID="txtAdminNombre" runat="server" Width="300" /></p>
            <p>Apellido:<br /><asp:TextBox ID="txtAdminApellido" runat="server" Width="300" /></p>
            <p>Email:<br /><asp:TextBox ID="txtAdminEmail" runat="server" TextMode="Email" Width="300" /></p>
            <p>Id idioma:<br /><asp:TextBox ID="txtAdminIdioma" runat="server" Text="1" /></p>

            <p>
                <asp:Button ID="btnCrear" runat="server" Text="Crear empresa y administrador" OnClick="btnCrear_Click" />
            </p>

            <p><asp:Label ID="lblMsg" runat="server" Font-Bold="true" /></p>

            <hr />
            <h3>Empresas registradas</h3>
            <asp:GridView ID="gvEmpresas" runat="server" AutoGenerateColumns="false">
                <Columns>
                    <asp:BoundField DataField="IdEmpresa" HeaderText="Id" />
                    <asp:BoundField DataField="NombreEmpresa" HeaderText="Nombre" />
                    <asp:BoundField DataField="NumContactoEmpresa" HeaderText="Contacto" />
                    <asp:BoundField DataField="PlanSuscripcion" HeaderText="Plan" />
                    <asp:BoundField DataField="FechaAlta" HeaderText="Alta" />
                    <asp:BoundField DataField="Estado" HeaderText="Estado" />
                </Columns>
            </asp:GridView>

            <hr />
            <p>
                <a href="MenuPruebas.aspx">Menu</a> |
                <a href="RegistrarUsuario.aspx">Registrar usuario</a> |
                <a href="Logout.aspx">Cerrar sesion</a>
            </p>
        </div>
    </form>
</body>
</html>
