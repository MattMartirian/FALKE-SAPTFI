<%@ Page Language="C#" MasterPageFile="~/Publico.master" AutoEventWireup="true" CodeFile="Registro.aspx.cs" Inherits="GUI.Registro" Title="Cómo se obtiene una cuenta" %>

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
            <h1 data-i18n="registro.titulo">Cómo se obtiene una cuenta en Falke</h1>
            <p data-i18n="registro.bajada">
                Falke no tiene registro automático. Como el servicio incluye un dispositivo
                de seguimiento ocular en préstamo, cada cuenta se abre después de un acuerdo
                comercial. Es corto, y aquí está todo el circuito.
            </p>
        </header>

        <div class="aviso aviso-info" style="max-width: 720px; margin: 24px auto 0;">
            <svg width="20" height="20" aria-hidden="true"><use href="#i-mail" /></svg>
            <div>
                <strong data-i18n="registro.yaTeInvitaron.titulo">¿Ya te invitaron?</strong>
                <p class="sin-margen" data-i18n="registro.yaTeInvitaron.texto">
                    Si tu empresa ya es cliente, vas a recibir un correo con un enlace para definir
                    tu contraseña. Con eso tu cuenta queda activa y ya puedes iniciar sesión.
                </p>
            </div>
        </div>

        <section class="circuito">

            <article class="circuito-paso">
                <span class="numero" aria-hidden="true">1</span>
                <h3 data-i18n="registro.paso1.titulo">Nos escribis</h3>
                <p data-i18n="registro.paso1.texto">
                    Eliges el plan que mejor se acerca a lo que necesitas y nos mandas una
                    consulta desde la pantalla de planes con los datos de tu empresa.
                </p>
                <span class="quien" data-i18n="registro.paso1.quien">Lo hace tu equipo</span>
            </article>

            <article class="circuito-paso">
                <span class="numero" aria-hidden="true">2</span>
                <h3 data-i18n="registro.paso2.titulo">Coordinamos una demostración</h3>
                <p data-i18n="registro.paso2.texto">
                    Un ejecutivo comercial te contacta y arma una demostración de unos 45 minutos,
                    presencial o remota, sobre un activo digital tuyo. Ahí se ve en vivo como el
                    dispositivo captura la mirada y como quedan las visualizaciones.
                </p>
                <span class="quien" data-i18n="registro.paso2.quien">Lo hace Pattern Blue</span>
            </article>

            <article class="circuito-paso">
                <span class="numero" aria-hidden="true">3</span>
                <h3 data-i18n="registro.paso3.titulo">Se firma el contrato</h3>
                <p data-i18n="registro.paso3.texto">
                    Se define el plan, el ciclo de facturación (mensual o anual) y la cantidad de
                    dispositivos. El contrato incluye las condiciones de préstamo y devolución del
                    hardware.
                </p>
                <span class="quien" data-i18n="registro.paso3.quien">Lo firman las dos partes</span>
            </article>

            <article class="circuito-paso">
                <span class="numero" aria-hidden="true">4</span>
                <h3 data-i18n="registro.paso4.titulo">Damos de alta tu empresa</h3>
                <p data-i18n="registro.paso4.texto">
                    Un gestor de Pattern Blue registra la empresa en la plataforma y crea el usuario
                    administrador que tu designes. Esa persona recibe un correo con un enlace para
                    definir su contraseña y activar la cuenta.
                </p>
                <span class="quien" data-i18n="registro.paso4.quien">Lo hace Pattern Blue</span>
            </article>

            <article class="circuito-paso">
                <span class="numero" aria-hidden="true">5</span>
                <h3 data-i18n="registro.paso5.titulo">Recibes el dispositivo</h3>
                <p data-i18n="registro.paso5.texto">
                    El Tobii viaja por courier con seguimiento, ya reacondicionado y calibrado, junto
                    con la guia de instalación impresa. El plazo comprometido es de diez días desde la
                    firma.
                </p>
                <span class="quien" data-i18n="registro.paso5.quien">Lo hace Pattern Blue</span>
            </article>

            <article class="circuito-paso">
                <span class="numero" aria-hidden="true">6</span>
                <h3 data-i18n="registro.paso6.titulo">Tu administrador suma al equipo</h3>
                <p data-i18n="registro.paso6.texto">
                    Desde la sección de usuarios, el administrador invita a los analistas de la
                    empresa con el mismo mecanismo: cada uno recibe su enlace de activación y define
                    su propia contraseña.
                </p>
                <span class="quien" data-i18n="registro.paso6.quien">Lo hace tu administrador</span>
            </article>

        </section>

        <section class="registro-cierre">
            <h2 data-i18n="registro.cierre.titulo">¿Listo para empezar?</h2>
            <p class="texto-suave" data-i18n="registro.cierre.texto">
                Mira los planes y mandanos la consulta desde ahí, o revisa primero las preguntas
                frecuentes si te quedan dudas del circuito.
            </p>
            <div class="fila" style="justify-content: center;">
                <a class="btn btn-primario btn-grande" href="<%: ResolveUrl("~/Planes.aspx") %>" data-i18n="registro.cierre.planes">Ver los planes y contactar</a>
                <a class="btn btn-secundario btn-grande" href="<%: ResolveUrl("~/Faq.aspx") %>" data-i18n="registro.cierre.faq">Ir a preguntas frecuentes</a>
            </div>
        </section>

    </div>

</asp:Content>
