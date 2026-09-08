<%@ Application Language="C#" %>
<%@ Import Namespace="DAL" %>
<script runat="server">

    void Application_EndRequest(object sender, EventArgs e)
    {
        // Red de seguridad: si el request termina con una transaccion todavia abierta
        // (error no controlado, camino que se salteo Confirmar/Revertir), se revierte
        // aca para que el hilo vuelva limpio al pool y no arrastre la conexion ni la
        // transaccion al proximo request que lo reutilice.
        GestorBaseDeDatos_DAL.Instancia.AbortarTransaccion();
    }

</script>
