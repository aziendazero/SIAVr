<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TipiCNSAss.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_TipiCNSAss" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title></title>
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript">
        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;
            switch (button.Key) {
                case 'btn_Annulla':
                    if (confirm("Le modifiche effettuate andranno perse. Continuare?")) {
                        window.location.href = 'Associazioni.aspx';
                    }
                    evnt.needPostBack = false;
                    break;
              
            }
        }			
    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <on_lay3:onitlayout3 ID="OnitLayout31" runat="server" Height="100%" Width="100%">
			<div class="title">
                <asp:Label id="LayoutTitolo" runat="server" Width="100%" ></asp:Label>
			</div>
            <div>
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
	                <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
			        <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
			        <Items>
				        <igtbar:TBarButton Key="btn_Salva" Text="Salva" Image="~/Images/salva.gif"></igtbar:TBarButton>
				        <igtbar:TBarButton Key="btn_Annulla" Text="Annulla" Image="~/Images/annulla.gif"></igtbar:TBarButton>
			        </Items>
		        </igtbar:UltraWebToolbar>
            </div>
		    <div class="sezione">
			    <asp:Label id="LayoutTitolo_sezioneCnv" runat="server" Width="100%">ELENCO TIPI CENTRI VACCINALI</asp:Label>
		    </div>

            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto">
		        <asp:CheckBoxList ID="chklTipiCNS" CssClass="TextBox_Stringa" runat="server">
		            <asp:ListItem Value="A">Centri Vaccinale Adulti</asp:ListItem>
                    <asp:ListItem Value="P">Centri Vaccinale Pediatrico</asp:ListItem>
                    <asp:ListItem Value="V">Pediatra Vaccinatore</asp:ListItem>
		        </asp:CheckBoxList>
            </dyp:DynamicPanel>

        </on_lay3:onitlayout3>
    </form>
</body>
</html>
