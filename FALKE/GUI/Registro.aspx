<%@ Page Language="C#" MasterPageFile="~/Publico.master" AutoEventWireup="true" CodeFile="Registro.aspx.cs" Inherits="GUI.Registro" Title="[[registro_como_se_obtiene_una_cuenta]]" %>

<asp:Content ContentPlaceHolderID="cabeza" runat="server">
    <style>

        .registro-cabecera
        {
            padding: 56px 0 12px;
            max-width: 720px;
            margin: 0 auto;
        }

        .registro-cabecera p
        {
            color: var(--texto-suave);
        }

        .circuito
        {
            max-width: 720px;
            margin: 40px auto 0;
            position: relative;
            padding-left: 54px;
        }

        .circuito::before
        {
            content: "";
            position: absolute;
            left: 17px;
            top: 12px;
            bottom: 12px;
            width: 2px;
            background: var(--borde);
        }

        .circuito-paso
        {
            position: relative;
            padding-bottom: 30px;
        }

        .circuito-paso:last-child
        {
            padding-bottom: 0;
        }

        .circuito-paso .numero
        {
            position: absolute;
            left: -54px;
            top: 0;
            width: 36px;
            height: 36px;
            border-radius: 50%;
            background: var(--marca-azul);
            color: var(--marca-dorado);
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: 700;
            border: 3px solid var(--fondo);
        }

        html[data-tema="noche"] .circuito-paso .numero
        {
            background: var(--marca-dorado);
            color: var(--marca-azul);
        }

        .circuito-paso h3
        {
            margin-bottom: 4px;
        }

        .circuito-paso p
        {
            color: var(--texto-suave);
            margin-bottom: 0;
            font-size: .93rem;
        }

        .circuito-paso .quien
        {
            display: inline-block;
            margin-top: 8px;
            font-size: .84rem;
            font-weight: 600;
            color: var(--texto-suave);
        }

        .registro-cierre
        {
            max-width: 720px;
            margin: 48px auto 0;
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="cuerpo" runat="server">

    <div class="contenedor">

        <header class="registro-cabecera">
            <h1 data-i18n="registro.titulo">[[registro_como_se_obtiene_una_cuenta_en]]</h1>
            <p data-i18n="registro.bajada">
                [[registro_falke_no_tiene_registro_automatico_como]]
            </p>
        </header>

        <div class="aviso aviso-info" style="max-width: 720px; margin: 24px auto 0;">
            <svg width="20" height="20" aria-hidden="true"><use href="#i-mail" /></svg>
            <div>
                <strong data-i18n="registro.yaTeInvitaron.titulo">[[registro_ya_te_invitaron]]</strong>
                <p class="sin-margen" data-i18n="registro.yaTeInvitaron.texto">
                    [[registro_si_tu_empresa_ya_es_cliente]]
                </p>
            </div>
        </div>

        <section class="circuito">

            <article class="circuito-paso">
                <span class="numero" aria-hidden="true">1</span>
                <h3 data-i18n="registro.paso1.titulo">[[registro_nos_escribis]]</h3>
                <p data-i18n="registro.paso1.texto">
                    [[registro_eliges_el_plan_que_mejor_se]]
                </p>
                <span class="quien" data-i18n="registro.paso1.quien">[[registro_lo_hace_tu_equipo]]</span>
            </article>

            <article class="circuito-paso">
                <span class="numero" aria-hidden="true">2</span>
                <h3 data-i18n="registro.paso2.titulo">[[registro_coordinamos_una_demostracion]]</h3>
                <p data-i18n="registro.paso2.texto">
                    [[registro_un_ejecutivo_comercial_te_contacta_y]]
                </p>
                <span class="quien" data-i18n="registro.paso2.quien">[[registro_lo_hace_pattern_blue]]</span>
            </article>

            <article class="circuito-paso">
                <span class="numero" aria-hidden="true">3</span>
                <h3 data-i18n="registro.paso3.titulo">[[registro_se_firma_el_contrato]]</h3>
                <p data-i18n="registro.paso3.texto">
                    [[registro_se_define_el_plan_el_ciclo]]
                </p>
                <span class="quien" data-i18n="registro.paso3.quien">[[registro_lo_firman_las_dos_partes]]</span>
            </article>

            <article class="circuito-paso">
                <span class="numero" aria-hidden="true">4</span>
                <h3 data-i18n="registro.paso4.titulo">[[registro_damos_de_alta_tu_empresa]]</h3>
                <p data-i18n="registro.paso4.texto">
                    [[registro_un_gestor_de_pattern_blue_registra]]
                </p>
                <span class="quien" data-i18n="registro.paso4.quien">[[registro_lo_hace_pattern_blue]]</span>
            </article>

            <article class="circuito-paso">
                <span class="numero" aria-hidden="true">5</span>
                <h3 data-i18n="registro.paso5.titulo">[[registro_recibes_el_dispositivo]]</h3>
                <p data-i18n="registro.paso5.texto">
                    [[registro_el_tobii_viaja_por_courier_con]]
                </p>
                <span class="quien" data-i18n="registro.paso5.quien">[[registro_lo_hace_pattern_blue]]</span>
            </article>

            <article class="circuito-paso">
                <span class="numero" aria-hidden="true">6</span>
                <h3 data-i18n="registro.paso6.titulo">[[registro_tu_administrador_suma_al_equipo]]</h3>
                <p data-i18n="registro.paso6.texto">
                    [[registro_desde_la_seccion_de_usuarios_el]]
                </p>
                <span class="quien" data-i18n="registro.paso6.quien">[[registro_lo_hace_tu_administrador]]</span>
            </article>

        </section>

        <section class="registro-cierre">
            <h2 data-i18n="registro.cierre.titulo">[[registro_listo_para_empezar]]</h2>
            <p class="texto-suave" data-i18n="registro.cierre.texto">
                [[registro_mira_los_planes_y_mandanos_la]]
            </p>
            <div class="fila" style="justify-content: center;">
                <a class="btn btn-primario btn-grande" href="<%: ResolveUrl("~/Planes.aspx") %>" data-i18n="registro.cierre.planes">[[registro_ver_los_planes_y_contactar]]</a>
                <a class="btn btn-secundario btn-grande" href="<%: ResolveUrl("~/Faq.aspx") %>" data-i18n="registro.cierre.faq">[[registro_ir_a_preguntas_frecuentes]]</a>
            </div>
        </section>

    </div>

</asp:Content>
