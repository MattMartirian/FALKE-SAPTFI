<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SelectorIdioma.ascx.cs" Inherits="GUI.SelectorIdioma" %>

<label class="selector-idioma">
    <span class="solo-lectores">[[selector_idioma]]</span>
    <asp:DropDownList ID="ddlIdioma" runat="server" CssClass="entrada" AutoPostBack="true" OnSelectedIndexChanged="ddlIdioma_SelectedIndexChanged" CausesValidation="false" />
</label>
