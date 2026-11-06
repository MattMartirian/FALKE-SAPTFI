<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="SinPermiso.aspx.cs" Inherits="GUI.SinPermiso" Title="[[sinpermiso_sin_permiso]]" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>

        .sin-permiso
        {
            max-width: 560px;
            margin: 40px 0 0;
        }

        .sin-permiso .simbolo
        {
            margin: 0 0 14px;
            color: var(--acento-texto);
        }

        .sin-permiso h1
        {
            font-size: 1.6rem;
            margin-bottom: 10px;
        }

        .sin-permiso .acciones
        {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            margin-top: 26px;
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <section class="sin-permiso">

        <div class="simbolo" aria-hidden="true">
            <svg width="30" height="30"><use href="#i-candado" /></svg>
        </div>

        <h1 data-i18n="sinPermiso.titulo">[[sinpermiso_no_tienes_permiso_para_entrar_aqui]]</h1>

        <p class="texto-suave" data-i18n="sinPermiso.texto">
            [[sinpermiso_tu_rol_no_incluye_esta_seccion]]
        </p>

        <div class="aviso aviso-info" style="text-align:left">
            <svg width="20" height="20" aria-hidden="true"><use href="#i-info" /></svg>
            <p class="sin-margen" data-i18n="sinPermiso.registro">
                [[sinpermiso_el_intento_de_acceso_queda_registrado]]
            </p>
        </div>

        <div class="acciones">
            <a class="btn btn-primario" href="<%: ResolveUrl("~/Panel.aspx") %>" data-i18n="sinPermiso.volver">[[sinpermiso_volver_al_panel]]</a>
            <a class="btn btn-secundario" href="<%: ResolveUrl("~/Salir.aspx") %>" data-i18n="usuario.salir">[[app_cerrar_sesion]]</a>
        </div>

    </section>

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
</asp:Content>
