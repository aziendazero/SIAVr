<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="StoricoVaccinazioniEscluse.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.StoricoVaccinazioniEscluse" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_otb" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitTable" %>
<%@ Register TagPrefix="uc1" TagName="StoricoRinnovi" Src="../VacEscluse/StoricoRinnovi.ascx" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>



<%@ Import Namespace="Onit.OnAssistnet.OnVac" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Log Notifiche Vac</title>
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/toolbar.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/default/toolbar.default.css") %>' type="text/css" rel="stylesheet" />
    <script type='text/javascript' src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Utility.js") %>"></script>
    <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
<%--    <script type="text/javascript">
    function ImpostaImmagineOrdinamento(imgId, imgUrl) {
        var img = document.getElementById(imgId);
        if (img != null) {
            img.style.display = 'inline';
            img.src = imgUrl;
        }
    }
    function OnClientButtonClicking(sender, args) {
        if (!e) var e = window.event;

        return true;
    }
    </script>

    <style type="text/css">
        .message {
            width: 100%;
            text-align: center;
            font-weight: bold;
            margin-top: 20px;
        }
    </style>--%>

</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />

        <on_lay3:onitlayout3 id="OnitLayout31" runat="server" titlecssclass="Title3" titolo="Storico Rinnovi">
                                 
            <div class="title">
                <asp:Label ID="LayoutTitolo" runat="server" Width="100%"></asp:Label>
            </div>

            <uc1:StoricoRinnovi id="StoricoRinnovi" runat="server" SoloRinnovati="false" Height="100%"></uc1:StoricoRinnovi>

        </on_lay3:onitlayout3>
    </form>
</body>
</html>
