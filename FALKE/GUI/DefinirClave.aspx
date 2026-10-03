<%@ Page Language="C#" MasterPageFile="~/Publico.master" AutoEventWireup="true" CodeFile="DefinirClave.aspx.cs" Inherits="GUI.DefinirClave" Title="Definir contraseña" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>
        .definir
        {
            padding: 56px 0 80px;
        }

        .definir-caja
        {
            max-width: 520px;
            margin: 0 auto;
            background: var(--superficie);
            border: 1px solid var(--borde);
            border-radius: var(--radio-lg);
            box-shadow: var(--sombra);
            padding: 40px 40px 34px;
        }

        .definir-caja h1
        {
            font-size: 1.55rem;
            margin-bottom: 6px;
        }

        .definir-caja .bajada
        {
            color: var(--texto-suave);
            margin-bottom: 26px;
        }

        .definir-icono
        {
            color: var(--acento-texto);
            margin-bottom: 12px;
        }

        .definir-cuenta
        {
            font-family: var(--fuente-mono);
            font-size: .82rem;
            color: var(--texto-suave);
            margin-bottom: 20px;
        }

        .definir-pie
        {
            margin-top: 26px;
            padding-top: 20px;
            border-top: 1px solid var(--borde);
            text-align: center;
            font-size: .9rem;
        }

        @media (max-width: 600px)
        {
            .definir-caja
            {
                padding: 30px 22px 26px;
            }
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <section class="definir">
        <div class="contenedor">
            <div class="definir-caja">

                <div class="definir-icono" aria-hidden="true">
                    <svg width="24" height="24"><use href="#i-candado" /></svg>
                </div>

                <h1 data-i18n="definir.titulo">Definir mi contraseña</h1>
                <p class="bajada" data-i18n="definir.bajada">
                    Elegí una contraseña para tu cuenta de Falke. Con esto la cuenta queda activa.
                </p>

                <asp:Panel ID="pnlError" runat="server" CssClass="aviso aviso-peligro" Visible="false">
                    <svg width="20" height="20" aria-hidden="true"><use href="#i-alerta" /></svg>
                    <p class="sin-margen"><asp:Literal ID="litError" runat="server" /></p>
                </asp:Panel>

                <asp:Panel ID="pnlOk" runat="server" CssClass="aviso aviso-exito" role="status" Visible="false">
                    <svg width="20" height="20" aria-hidden="true"><use href="#i-tilde" /></svg>
                    <p class="sin-margen" data-i18n="definir.ok">
                        Listo: tu contraseña quedó definida y la cuenta está activa. Ya podés iniciar sesión.
                    </p>
                </asp:Panel>

                <asp:Panel ID="pnlForm" runat="server">

                    <p class="definir-cuenta">
                        <span data-i18n="definir.cuenta">Cuenta:</span>
                        <asp:Literal ID="litEmail" runat="server" />
                    </p>

                    <div class="campo">
                        <label for="txtNueva" data-i18n="definir.nueva">Contraseña nueva</label>
                        <asp:TextBox ID="txtNueva" runat="server" CssClass="entrada" TextMode="Password"
                                     ClientIDMode="Static" autocomplete="new-password" />
                        <p class="ayuda" data-i18n="definir.requisito">Al menos 8 caracteres.</p>
                    </div>

                    <div class="campo">
                        <label for="txtRepetir" data-i18n="definir.repetir">Repetir la contraseña nueva</label>
                        <asp:TextBox ID="txtRepetir" runat="server" CssClass="entrada" TextMode="Password"
                                     ClientIDMode="Static" autocomplete="new-password" />
                        <p class="ayuda" data-i18n="definir.repetir.ayuda">Las dos tienen que coincidir.</p>
                    </div>

                    <asp:Button ID="btnGuardar" runat="server" CssClass="btn btn-primario btn-grande btn-bloque"
                                OnClick="btnGuardar_Click" Text="Guardar la contraseña" data-i18n="definir.guardar" />

                </asp:Panel>

                <div class="definir-pie">
                    <p class="sin-margen">
                        <a href="<%: ResolveUrl("~/Ingresar.aspx") %>" data-i18n="definir.volver">Ir al inicio de sesión</a>
                    </p>
                    <p class="texto-chico texto-suave mt-8 sin-margen" data-i18n="definir.vencido">
                        Si el enlace venció, pedí uno nuevo desde «Recuperar mi contraseña».
                    </p>
                </div>

            </div>
        </div>
    </section>

</asp:Content>
