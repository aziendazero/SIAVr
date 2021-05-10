<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="StoricoAppuntamentiPaziente.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.StoricoAppuntamentiPaziente" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="uc1" TagName="StoricoAppuntamenti" Src="../../Common/Controls/ElencoStoricoAppuntamenti.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Storico Appuntamenti Paziente</title>
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

</head>
<body>
    <form id="form1" runat="server">

        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" TitleCssClass="Title3" Titolo="Storico Appuntamenti Paziente">
            <div class="title">
                <asp:Label ID="LayoutTitolo" runat="server" Width="100%"></asp:Label>
            </div>

            <uc1:StoricoAppuntamenti id="ucStoricoAppuntamenti" runat="server" Visualizzazione="Completa"></uc1:StoricoAppuntamenti>

        </on_lay3:OnitLayout3>

    </form>
</body>
</html>
