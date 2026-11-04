<%@ Page Language="C#" MasterPageFile="~/App.master" AutoEventWireup="true" CodeFile="Descargas.aspx.cs" Inherits="GUI.Descargas" Title="[[descargas_titulo]]" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>

        .descarga-escena
        {
            position: relative;
            display: flex;
            align-items: center;
            justify-content: center;
            min-height: calc(100vh - var(--alto-topbar) - 56px);
        }

        .descarga-escena > .aviso
        {
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            margin: 0;
        }

        .descarga-tarjeta
        {
            width: 100%;
            max-width: 640px;
            margin: 0;
            padding: 36px 32px;
            text-align: center;
            border: 1px solid var(--borde);
            border-radius: var(--radio-lg);
            background: var(--superficie);
        }

        .descarga-icono
        {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            width: 56px;
            height: 56px;
            margin-bottom: 14px;
            border-radius: var(--radio-full);
            background: var(--marca-dorado);
            color: var(--marca-azul);
        }

        .descarga-tarjeta h1
        {
            margin: 0 0 8px;
            font-size: 1.5rem;
        }

        .descarga-boton
        {
            margin-top: 22px;
        }

        .descarga-pasos
        {
            margin-top: 28px;
            padding-top: 22px;
            border-top: 1px solid var(--borde);
            text-align: left;
        }

        .descarga-pasos[hidden]
        {
            display: none;
        }

        .descarga-pasos h2
        {
            margin: 0 0 10px;
            font-size: 1.05rem;
        }

        .descarga-pasos ol
        {
            margin: 0;
            padding-left: 20px;
        }

        .descarga-pasos li
        {
            margin-bottom: 6px;
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <div class="descarga-escena">

    <asp:Panel ID="pnlAviso" runat="server" CssClass="aviso" Visible="false" role="status">
        <svg width="20" height="20" aria-hidden="true"><use href="#i-info" /></svg>
        <p class="sin-margen"><asp:Literal ID="litAviso" runat="server" /></p>
    </asp:Panel>

    <div class="descarga-tarjeta">
        <span class="descarga-icono"><svg width="28" height="28" aria-hidden="true"><use href="#i-descarga" /></svg></span>
        <h1>[[descargas_bienvenida_titulo]]</h1>
        <p class="sin-margen">[[descargas_bienvenida_mensaje]]</p>
        <p class="texto-suave sin-margen mt-8"><asp:Literal ID="litIncluye" runat="server" /></p>
        <p class="texto-suave sin-margen">[[descargas_requisitos]]</p>

        <asp:LinkButton ID="btnTodo" runat="server" CssClass="btn btn-primario btn-grande descarga-boton" OnClick="btnTodo_Click" CausesValidation="false"
                        OnClientClick="document.getElementById('descargaPasos').hidden = false; return true;">
            <svg width="18" height="18" aria-hidden="true"><use href="#i-descarga" /></svg>
            <span>[[descargas_descargar_todo]]</span>
        </asp:LinkButton>

        <div class="descarga-pasos" id="descargaPasos" runat="server" ClientIDMode="Static" hidden="hidden">
            <h2>[[descargas_despues_de_descargar]]</h2>
            <ol>
                <li>[[descargas_paso_1]]</li>
                <li>[[descargas_paso_2]]</li>
                <li>[[descargas_paso_3]]</li>
                <li>[[descargas_paso_4]]</li>
            </ol>
        </div>
    </div>

    </div>

</asp:Content>
