using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TE;
using TLL;

public partial class BitacoraTest : System.Web.UI.Page
{
    private readonly BitacoraGestor_TLL _gestor = new BitacoraGestor_TLL();

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void btnGenerarEntrada_Click(object sender, EventArgs e)
    {
        var bitacora = new Bitacora_TE
        {
            IdUsuario = 1,
            ModuloBitacora = "BitacoraTest",
            DescripcionBitacora = "Entrada generada desde la página de prueba.",
            CriticidadBitacora = CriticidadBitacora.Baja,
            FechaHoraBitacora = DateTime.Now
        };

        _gestor.Guardar(bitacora);
    }

    protected void btnLeerEntradas_Click(object sender, EventArgs e)
    {
        gvBitacora.DataSource = _gestor.ObtenerTodas();
        gvBitacora.DataBind();
    }
}