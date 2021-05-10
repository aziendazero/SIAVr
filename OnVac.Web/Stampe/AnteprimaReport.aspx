<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="AnteprimaReport.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.AnteprimaReport" %>

<%@ Register TagPrefix="on_otb" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitTable" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Anteprima Report</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript"  src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Utility.js") %>"></script>
    <script type="text/javascript">
        function InizializzaToolBar(t) 
        {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) 
        {
            evnt.needPostBack = true;
        }
    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
    <on_otb:OnitTable ID="OnitTable1" runat="server" Height="100%" Width="100%">
		
        <on_otb:onitsection id="OnitSection2" runat="server" width="100%" typeHeight="Manual" Height="26px">
			<on_otb:onitcell id="OnitCell2" runat="server" width="100%" height="100%">
				<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="UltraWebToolbar1" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
	                <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                    <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
					<Items>
						<igtbar:TBarButton Key="btnChiudi" DisabledImage="~/Images/esci.gif" Text="Chiudi" Image="~/Images/esci.gif"></igtbar:TBarButton>
					</Items>
				</igtbar:UltraWebToolbar>
			</on_otb:onitcell>
		</on_otb:onitsection>

		<on_otb:onitsection id="OnitSection1" runat="server" width="100%" typeHeight="Calculate">
			<on_otb:onitcell id="OnitCell1" runat="server" width="100%" height="100%" typescroll="Hidden">
				<iframe id="ReportViewer" style="width: 100%; height: 100%" runat="server" ></iframe>
			</on_otb:onitcell>
		</on_otb:onitsection>

    </on_otb:OnitTable>
    </form>
</body>
</html>
