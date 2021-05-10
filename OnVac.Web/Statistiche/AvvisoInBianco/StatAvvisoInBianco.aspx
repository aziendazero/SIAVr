<%@ Page Language="vb" AutoEventWireup="false" Codebehind="StatAvvisoInBianco.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.AvvisoInBianco" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel"  %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>AvvisoInBianco</title>
		
        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
		
        <script type="text/javascript" language="javascript">
            function InizializzaToolBar(t) {
                t.PostBackButton = false;
            }
		
		    function ToolBarClick(ToolBar, button, evnt) {
		        evnt.needPostBack = true;
		    }
		
		    //lancio della stampa tramite tasto invio (modifica 29/07/2004)
		    function Stampa(evt) {
		        __doPostBack("Stampa", "");
		    }
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server" onkeyup="Stampa(event)">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" height="100%" width="100%" TitleCssClass="Title3" Titolo="Statistiche -<b> Avviso in bianco</b>">
				<div class="title" id="PanelTitolo" runat="server">
					<asp:Label id="LayoutTitolo" runat="server"></asp:Label>
                </div>
                <div>
				    <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="Toolbar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                        <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                        <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		                <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
					    <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
					    <Items>
						    <igtbar:TBarButton Key="btnStampa" DisabledImage="~/Images/stampa.gif" Text="Stampa" Image="~/Images/Stampa.gif"></igtbar:TBarButton>
					    </Items>
				    </igtbar:UltraWebToolbar>
                </div>
				<div class="sezione" id="Panel23" runat="server">
					<asp:Label id="LayoutTitolo_sezioneRicerca" runat="server">Filtri di stampa</asp:Label>
                </div>
                <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
							<fieldset id="fldSollecito" title="Sollecito" class="fldroot vac-fieldset-height-45">
							    <legend class="label">Sollecito</legend>
                                <asp:RadioButtonList id="rdlSollecito" runat="server" RepeatDirection="Horizontal" TextAlign="Right" CssClass="label" Width="100%">
								    <asp:ListItem Value="0" Selected="True">Nessuno</asp:ListItem>
								    <asp:ListItem Value="1">Primo</asp:ListItem>
								    <asp:ListItem Value="2">Secondo</asp:ListItem>
								    <asp:ListItem Value="3">Terzo</asp:ListItem>
								    <asp:ListItem Value="4">Quarto</asp:ListItem>
							    </asp:RadioButtonList>
							</fieldset>                            
                        </div>
                        <div class="vac-colonna-destra">
							<fieldset id="fldEtaPaziente" title="Età paziente" class="fldroot vac-fieldset-height-45">
							    <legend class="label">Età paziente</legend>
                                <asp:RadioButtonList id="rdlEta" runat="server" RepeatDirection="Horizontal" TextAlign="Right" CssClass="textbox_stringa" Width="100%">
									<asp:ListItem Value="1" Selected="True">Minorenne</asp:ListItem>
									<asp:ListItem Value="0">Maggiorenne</asp:ListItem>
								</asp:RadioButtonList>
							</fieldset>
                        </div>
                    </div>
                </dyp:DynamicPanel>
			</on_lay3:onitlayout3>
		</form>
	</body>
</html>
