<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="SinPermiso.aspx.cs" Inherits="GUI.SinPermiso" Title="Sin permiso" %>

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

        <h1 data-i18n="sinPermiso.titulo">No tienes permiso para entrar aquí</h1>

        <p class="texto-suave" data-i18n="sinPermiso.texto">
            Tu rol no incluye esta sección. Si necesitas acceder, solicítalo al administrador
            de tu empresa: es quien puede solicitar el cambio de permisos.
        </p>

        <div class="aviso aviso-info" style="text-align:left">
            <svg width="20" height="20" aria-hidden="true"><use href="#i-info" /></svg>
            <p class="sin-margen" data-i18n="sinPermiso.registro">
                El intento de acceso queda registrado en la bitácora. No es un problema:
                se anota para poder revisar después quién necesita qué permisos.
            </p>
        </div>

        <div class="acciones">
            <a class="btn btn-primario" href="<%: ResolveUrl("~/Panel.aspx") %>" data-i18n="sinPermiso.volver">Volver al panel</a>
            <a class="btn btn-secundario" href="<%: ResolveUrl("~/Salir.aspx") %>" data-i18n="usuario.salir">Cerrar sesión</a>
        </div>

    </section>

</asp:Content>

<asp:Content ContentPlaceHolderID="scripts" runat="server">
</asp:Content>
