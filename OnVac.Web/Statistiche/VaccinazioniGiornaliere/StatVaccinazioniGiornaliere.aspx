<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="StatVaccinazioniGiornaliere.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.StatVaccinazioniGiornaliere" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<%@ Register TagPrefix="uc2" TagName="SelezioneAmbulatorio" Src="../../Common/Controls/SelezioneAmbulatorio.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Statistica Vaccinazioni Giornaliere</title>
    
        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript">
        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;

            switch (button.Key) {
                case 'btnStampa':
                    if (!checkCampiObbligatori()) {
                        evnt.needPostBack = false;
                    }
                    break;
            }
        }

        //lancio della stampa tramite tasto invio (modifica 29/07/2004)
        function Stampa(evt) {
            if (evt.keyCode == 13) {
                if (!checkCampiObbligatori()) {
                    evnt.needPostBack = false;
                }
                else {
                    __doPostBack("Stampa", "");
                }
            }
        }

        function checkCampiObbligatori() {
            if (OnitDataPickGet('odpDataEffettuazioneIniz') == "" || OnitDataPickGet('odpDataEffettuazioneFin') == "") {
                alert("Non tutti i campi obbligatori sono impostati. Impossibile stampare il report.");
                return false;
            }
            return true;
        }		
    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server" onkeyup="Stampa(event)" style="overflow: auto">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Height="100%" Width="100%" Titolo="Statistiche - <b>Vaccinazioni giornaliere</b>" TitleCssClass="Title3">

			<div class="title" id="PanelTitolo" runat="server">
                <asp:Label ID="LayoutTitolo" runat="server"></asp:Label>
            </div>
            <div>
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="Toolbar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
	                <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                    <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
                    <Items>
                        <igtbar:TBarButton Key="btnStampa" DisabledImage="~/Images/stampa.gif"
                            Text="Stampa" Image="~/Images/Stampa.gif">
                        </igtbar:TBarButton>
                    </Items>
                </igtbar:UltraWebToolbar>
            </div>
            <div class="sezione" id="Panel23" runat="server">
                <asp:Label ID="LayoutTitolo_sezioneRicerca" runat="server">Filtri di stampa</asp:Label>
            </div>

            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                <div class="vac-riga">
                    <fieldset title="Centro Vaccinale" class="fldroot">
                        <legend class="label">Centro Vaccinale</legend>
                        <div>
                            <uc2:SelezioneAmbulatorio ID="uscScegliAmb" TuttiCns="True" Tutti="True" runat="server" />
                        </div>
                    </fieldset>
                </div>
                <div class="vac-riga">
                    <div class="vac-colonna-sinistra">
                        <fieldset title="Data di effettuazione" class="fldroot">
                            <legend class="label">Data di effettuazione</legend>
							<table style="width: 100%">
								<tr>
									<td class="label">
										<asp:Label id="lblDataEffettuazioneIniz" runat="server">Da</asp:Label></td>
									<td>
										<on_val:onitdatepick id="odpDataEffettuazioneIniz" runat="server" Height="20px" CssClass="textbox_data_obbligatorio" Width="136px" DateBox="True"></on_val:onitdatepick></td>
									<td class="label">
										<asp:Label id="lblDataEffettuazioneFin" runat="server" Width="20px">A</asp:Label></td>
									<td>
										<on_val:onitdatepick id="odpDataEffettuazioneFin" runat="server" Height="20px" CssClass="textbox_data_obbligatorio" Width="136px" DateBox="True"></on_val:onitdatepick></td>
								</tr>
							</table>
						</fieldset>
                    </div>
                    <div class="vac-colonna-destra">
                        <fieldset title="Data nascita" class="fldroot">
                            <legend class="label">Data di nascita</legend>
                            <table style="width: 100%">
								<tr>
									<td class="label">
										<asp:Label id="lblDataNascitaIniz" runat="server">Da</asp:Label></td>
									<td>
										<on_val:onitdatepick id="odpDataNascitaIniz" runat="server" Height="20px" CssClass="textbox_data" Width="136px" DateBox="True"></on_val:onitdatepick></td>
									<td class="label">
										<asp:Label id="lblDataNascitaFin" runat="server">A</asp:Label></td>
									<td>
										<on_val:onitdatepick id="odpDataNascitaFin" runat="server" Height="20px" CssClass="textbox_data" Width="136px" DateBox="True"></on_val:onitdatepick></td>
								</tr>
							</table>
						</fieldset>                            
                    </div>
                </div>
                <div class="vac-riga">
                    <fieldset title="Vaccinazioni" class="fldroot">
                        <legend class="label">Vaccinazioni</legend>
                        <div>
                            <asp:CheckBoxList ID="chklVaccinazioni" runat="server" CssClass="label_left" RepeatColumns="4" RepeatDirection="Horizontal" TextAlign="Right" width="100%"></asp:CheckBoxList>
                        </div>
                        <div style="margin-top: 8px; margin-left:3px">
                            <asp:CheckBox id="chkFittizia" runat="server" CssClass="label" Text="Includi vaccini fittizi" Font-Bold="true" width="100%" />
                        </div>
                    </fieldset>
                </div>
            </dyp:DynamicPanel>

            <dyp:DynamicPanel ID="dypMsg" runat="server" Width="100%" Height="18px">
                <asp:Label ID="Label1" runat="server" Width="100%" CssClass="label_errormsg"></asp:Label>
            </dyp:DynamicPanel>
				    
        </on_lay3:OnitLayout3>
    </form>
</body>
</html>
