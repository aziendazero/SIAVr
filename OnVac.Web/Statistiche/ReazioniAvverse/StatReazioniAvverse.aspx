<%@ Page Language="vb" AutoEventWireup="false" Codebehind="StatReazioniAvverse.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.StatReazioniAvverse" %>

<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="uc1" TagName="SelezioneConsultori" Src="../../Common/Controls/SelezioneConsultori.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>Statistica Reazioni Avverse</title>

        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

		<script type="text/javascript">
		    function InizializzaToolBar(t) {
		        t.PostBackButton = false;
		    }

		    function ToolBarClick(ToolBar, button, evnt) {
		        evnt.needPostBack = true;

		        switch (button.Key) {
		            case "btnStampa":
		                if (!checkCampiObbligatori()) {
		                    evnt.needPostBack = false;
		                }
		                break;
		        }
		    }

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

                var dataNascitaIniz = OnitDataPickGet('odpDataNascitaIniz');
                var dataNascitaFin = OnitDataPickGet('odpDataNascitaFin');
                if ((dataNascitaIniz == '' && dataNascitaFin != '') || (dataNascitaIniz != '' && dataNascitaFin == '')) {
                    alert("I campi 'Data Nascita' non sono impostati correttamente. Impossibile stampare il report.");
                    return false;
                }

                var odpDataReazioneIniz = OnitDataPickGet('odpDataReazioneIniz');
                var odpDataReazioneFin = OnitDataPickGet('odpDataReazioneFin');
                if ((odpDataReazioneIniz == '' && odpDataReazioneFin != '') || (odpDataReazioneIniz != '' && odpDataReazioneFin == '')) {
                    alert("I campi 'Data Reazione' non sono impostati correttamente. Impossibile stampare il report.");
                    return false;
                }

		        return true;
		    }
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server" onkeyup="Stampa(event)">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" height="100%" width="100%" Titolo="Statistiche - <b>Reazioni avverse</b>" TitleCssClass="Title3">
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
							<igtbar:TBarButton Key="btnStampa" Text="Stampa" DisabledImage="~/Images/stampa.gif" Image="~/Images/Stampa.gif"></igtbar:TBarButton>
						</Items>
					</igtbar:UltraWebToolbar>
                </div>
				<div class="sezione" id="Panel23" runat="server">
					<asp:Label id="LayoutTitolo_sezioneRicerca" runat="server">Filtri di stampa</asp:Label>
                </div>

                <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                    
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset title="Comune di Residenza" class="fldroot" >
                                <legend class="label">Comune di Residenza</legend>
                                <on_ofm:OnitModalList id="fmComuneRes" runat="server" SetUpperCase="True" UseCode="True" Tabella="T_ANA_COMUNI"
									CampoDescrizione="COM_DESCRIZIONE" CampoCodice="COM_CODICE" Label="Comune" CodiceWidth="29%" LabelWidth="-8px"
									PosizionamentoFacile="False" RaiseChangeEvent="True" Width="70%"></on_ofm:OnitModalList>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset title="Circoscrizione di Residenza" class="fldroot" >
                                <legend class="label">Circoscrizione di Residenza</legend>
                                <on_ofm:OnitModalList id="fmCircoscrizione" runat="server" SetUpperCase="True" UseCode="True" Tabella="T_ANA_CIRCOSCRIZIONI"
									CampoDescrizione="CIR_DESCRIZIONE" CampoCodice="CIR_CODICE" Label="Circoscrizione" CodiceWidth="29%"
									LabelWidth="-8px" PosizionamentoFacile="False" RaiseChangeEvent="True" Width="70%"></on_ofm:OnitModalList>
                            </fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset title="Centro Vaccinale" class="fldroot vac-fieldset-height-45" >
                                <legend class="label">Centro Vaccinale</legend>
								<uc1:SelezioneConsultori id="ucSelezioneConsultori" runat="server" Width="100%"  MaxCnsInListaSelezionati="3"></uc1:SelezioneConsultori>
                            </fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset title="Distretto (relativo al CNS Vaccinale)" class="fldroot vac-fieldset-height-45" >
                                <legend class="label">Distretto (relativo al Centro Vaccinale)</legend>
                                <on_ofm:onitmodallist id="fmDistretto" runat="server" SetUpperCase="True" UseCode="True" Tabella="T_ANA_DISTRETTI"
									CampoDescrizione="DIS_DESCRIZIONE" CampoCodice="DIS_CODICE" Label="Distretto" CodiceWidth="28%" LabelWidth="-8px"
									PosizionamentoFacile="False" Width="70%"></on_ofm:onitmodallist>
							</fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset title="Data nascita" class="fldroot">
                                <legend class="label">Data di nascita</legend>
                                <table style="width: 100%">
									<tr>
										<td class="label_right">
                                            <asp:Label id="lblDataNascitaIniz" runat="server">Da</asp:Label></td>
                                        <td>
                                            <on_val:onitdatepick id="odpDataNascitaIniz" runat="server" CssClass="textbox_data" Width="136px" DateBox="True"></on_val:onitdatepick></td>
										<td class="label_right">
											<asp:Label id="lblDataNascitaFin" runat="server">A</asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataNascitaFin" runat="server" CssClass="textbox_data" Width="136px" DateBox="True"></on_val:onitdatepick></td>
									</tr>
								</table>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset title="Data di effettuazione" class="fldroot">
                                <legend class="label">Data di effettuazione</legend>
                                <table style="width: 100%">
									<tr>
										<td class="label_right">
                                            <asp:Label id="lblDataEffettuazioneIniz" runat="server">Da</asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataEffettuazioneIniz" runat="server" CssClass="textbox_data_obbligatorio" Width="136px" DateBox="True"></on_val:onitdatepick></td>
										<td class="label_right">
											<asp:Label id="lblDataEffettuazioneFin" runat="server">A</asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataEffettuazioneFin" runat="server" CssClass="textbox_data_obbligatorio" Width="136px" DateBox="True"></on_val:onitdatepick></td>
									</tr>
								</table>
							</fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset title="Data di reazione" class="fldroot">
                                <legend class="label">Data di reazione</legend>
								<table style="width: 100%">
                                    <tr>
                                        <td class="label_right">
                                            <asp:Label id="Label2" runat="server">Da</asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataReazioneIniz" runat="server" CssClass="textbox_data" Width="136px" DateBox="True"></on_val:onitdatepick></td>
										<td class="label_right">
											<asp:Label id="Label3" runat="server">A</asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataReazioneFin" runat="server" CssClass="textbox_data" Width="136px" DateBox="True"></on_val:onitdatepick></td>
									</tr>
								</table>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">

                        </div>
                    </div>

                </dyp:DynamicPanel>

                <dyp:DynamicPanel ID="dypMsg" runat="server" Width="100%" Height="18px">
                    <asp:Label id="Label1" runat="server" Width="100%" CssClass="label_errormsg"></asp:Label>
                </dyp:DynamicPanel>

			</on_lay3:onitlayout3>
		</form>
	</body>
</html>