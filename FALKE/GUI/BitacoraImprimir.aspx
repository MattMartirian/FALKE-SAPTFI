<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BitacoraImprimir.aspx.cs" Inherits="GUI.BitacoraImprimir" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Bitácora de Falke</title>
    <style>
        body
        {
            margin: 0;
            padding: 28px 32px;
            font: 12px/1.45 "Segoe UI", Arial, sans-serif;
            color: #111;
            background: #fff;
        }

        h1
        {
            margin: 0 0 4px;
            font-size: 20px;
        }

        .meta
        {
            margin: 0 0 14px;
            color: #444;
        }

        .barra
        {
            margin-bottom: 16px;
            padding: 10px 12px;
            border: 1px solid #ccc;
            border-radius: 4px;
            background: #f6f6f6;
        }

        .barra button
        {
            margin-left: 12px;
            padding: 5px 12px;
            font: inherit;
            cursor: pointer;
        }

        table
        {
            width: 100%;
            border-collapse: collapse;
        }

        th, td
        {
            padding: 5px 7px;
            border-bottom: 1px solid #ddd;
            text-align: left;
            vertical-align: top;
        }

        th
        {
            border-bottom: 2px solid #111;
            font-size: 11px;
            text-transform: uppercase;
            letter-spacing: .03em;
        }

        td.fecha
        {
            white-space: nowrap;
        }

        .error
        {
            padding: 12px;
            border: 1px solid #b00020;
            color: #b00020;
        }

        @media print
        {
            body { padding: 0; }
            .barra { display: none; }
            tr { page-break-inside: avoid; }
            thead { display: table-header-group; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <asp:PlaceHolder ID="phError" runat="server" Visible="false">
            <p class="error"><asp:Literal ID="litError" runat="server" /></p>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="phInforme" runat="server">

            <div class="barra">
                Elegí «Guardar como PDF» como destino en la ventana de impresión.
                <button type="button" onclick="window.print()">Imprimir / guardar PDF</button>
            </div>

            <h1>Bitácora de Falke</h1>
            <p class="meta">
                <asp:Literal ID="litMeta" runat="server" />
            </p>

            <table>
                <thead>
                    <tr>
                        <th>Fecha y hora</th>
                        <th>Usuario</th>
                        <th>Empresa</th>
                        <th>Módulo</th>
                        <th>Descripción</th>
                        <th>Criticidad</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptRegistros" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td class="fecha"><%#: ((DateTime)Eval("FechaHoraBitacora")).ToString("dd/MM/yyyy HH:mm:ss") %></td>
                                <td><%#: Eval("Actor") %></td>
                                <td><%#: Eval("NombreEmpresa") %></td>
                                <td><%#: Eval("ModuloBitacora") %></td>
                                <td><%#: Eval("DescripcionBitacora") %></td>
                                <td><%#: Eval("CriticidadBitacora") %></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>

        </asp:PlaceHolder>

    </form>
</body>
</html>
