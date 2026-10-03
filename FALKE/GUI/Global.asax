<%@ Application Language="C#" %>
<%@ Import Namespace="DAL" %>
<script runat="server">

    void Application_EndRequest(object sender, EventArgs e)
    {
        GestorBaseDeDatos_DAL.Instancia.AbortarTransaccion();
    }

</script>
