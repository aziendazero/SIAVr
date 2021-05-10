<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="StampaReportPopUp.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.StampaReportPopUp" %>

<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel"  Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>Anteprima Report</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Utility.js") %>"></script>
    <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
    <script type="text/javascript">
        var IsPostBack = <%= IsPostBack.ToString().ToLower() %>;

        $(document).ready(function () {
            if (!IsPostBack) {
                try {
                    showWaitScreen(2000, true);
                } catch (e) {
                    // non si vede il waitscreen
                }
                $('form').submit();
            }
        });

        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;

            switch (button.Key) {
                case 'btnChiudi':
                    window.close();
                    evnt.needPostBack = false;
                    break;
            }
        }		
    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <div>
			<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="UltraWebToolbar1" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
	            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
				<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
				<Items>
					<igtbar:TBarButton Key="btnChiudi" Text="Chiudi" DisabledImage="~/Images/esci.gif" Image="~/Images/esci.gif"></igtbar:TBarButton>
				</Items>
			</igtbar:UltraWebToolbar>
        </div>
        <dyp:DynamicPanel runat="server" Height="100%" Width="100%" ScrollBars="None">
		    <iframe id="ReportViewer" style="WIDTH: 100%; HEIGHT: 100%" runat="server"></iframe>
	    </dyp:DynamicPanel>
    </form>
</body>
</html>
