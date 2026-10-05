<%@ Page Language="C#" MasterPageFile="~/Publico.master" AutoEventWireup="true" CodeFile="Salir.aspx.cs" Inherits="GUI.Salir" Title="Cerrar sesión" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>
        /* El mensaje queda en el medio de la página, entre la barra de arriba y el pie. */
        #contenido-principal
        {
            display: flex;
            flex-direction: column;
            justify-content: center;
        }

        #contenido-principal > .contenedor
        {
            width: 100%;
        }

        /* Sin separación entre el mensaje y el pie: el centrado se mide contra la zona real. */
        .pie
        {
            margin-top: 0;
        }

        .salida
        {
            max-width: 520px;
            margin: 0 auto;
            padding: 56px 0;
            text-align: center;
        }

        .salida-icono
        {
            color: var(--exito);
            display: block;
            margin-bottom: 14px;
        }

        .salida-icono.pregunta
        {
            color: var(--acento-texto);
        }

        .salida .acciones
        {
            display: grid;
            gap: 10px;
            margin-top: 28px;
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <div class="contenedor">
        <div class="salida">

            <asp:Panel ID="pnlConfirmar" runat="server" Visible="false">
                <span class="salida-icono pregunta">
                    <svg width="30" height="30" aria-hidden="true"><use href="#i-salir" /></svg>
                </span>

                <h1 data-i18n="salir.confirmar.titulo">¿Cerrás tu sesión?</h1>

                <p class="texto-suave">
                    <asp:Literal ID="litConfirmar" runat="server" />
                </p>

                <div class="acciones">
                    <asp:Button ID="btnCerrar" runat="server" CssClass="btn btn-primario btn-grande"
                                OnClick="btnCerrar_Click" Text="Cerrar sesión" data-i18n="salir.confirmar.boton" />
                    <a class="btn btn-secundario" href="<%: ResolveUrl("~/Panel.aspx") %>" data-i18n="salir.confirmar.seguir">Seguir en Falke</a>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlHecho" runat="server" Visible="false">
                <span class="salida-icono">
                    <svg width="30" height="30" aria-hidden="true"><use href="#i-tilde" /></svg>
                </span>

                <h1 data-i18n="salir.titulo">Cerraste tu sesión</h1>

                <p class="texto-suave">
                    <asp:Literal ID="litDetalle" runat="server" />
                </p>

                <div class="acciones">
                    <a class="btn btn-primario btn-grande" href="<%: ResolveUrl("~/Ingresar.aspx") %>" data-i18n="salir.volverEntrar">Volver a entrar</a>
                    <a class="btn btn-secundario" href="<%: ResolveUrl("~/Default.aspx") %>" data-i18n="salir.irInicio">Ir a la página de inicio</a>
                </div>

                <p class="texto-chico texto-suave mt-24 sin-margen" data-i18n="salir.aviso">
                    Por seguridad, cerrá también la ventana del navegador si estás en un equipo compartido.
                </p>
            </asp:Panel>

        </div>
    </div>

</asp:Content>
