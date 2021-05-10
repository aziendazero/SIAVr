<%@ Page Language="vb" AutoEventWireup="false" Codebehind="StatVaccinazioniEseguite.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.StatisticaVaccinazioni" %>

<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="uc1" TagName="SelezioneConsultori" Src="../../Common/Controls/SelezioneConsultori.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>Statistica Vaccinazioni Eseguite</title>

        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

		<script type="text/javascript">
		    function InizializzaToolBar(t) {
		        t.PostBackButton = false;
		    }

		    function ToolBarClick(ToolBar, button, evnt) {
		        evnt.needPostBack = true;

                if ((!OnitDataPickGet('odpDataNascitaIniz') == "" && OnitDataPickGet('odpDataNascitaFin') == "") || (OnitDataPickGet('odpDataNascitaIniz') == "" && !OnitDataPickGet('odpDataNascitaFin') == "")) {
                    alert("I campi 'Data Nascita' non sono impostati correttamente. Impossibile stampare il report.");
                    evnt.needPostBack = false;
                }

		        switch (button.Key) {
		            case "btnStampa":
		            case "btnStampaComune":
		            case "btnStampaCircoscriz":
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
		        return true;
		    }
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server" onkeyup="Stampa(event)">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" height="100%" width="100%" Titolo="Statistiche - <b>Vaccinazioni eseguite</b>" TitleCssClass="Title3">

				<div class="title" id="PanelTitolo" runat="server">
					<asp:Label id="LayoutTitolo" runat="server"></asp:Label>
                </div>
                <div>
					<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="Toolbar" runat="server" ItemWidthDefault="190px" CssClass="infratoolbar">
                        <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                        <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
	                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
						<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
						<Items>
							<igtbar:TBarButton Key="btnStampa" Text="Stampa Per Centro Vacc" DisabledImage="~/Images/stampa.gif"
								Image="~/Images/Stampa.gif">
							</igtbar:TBarButton>
							<igtbar:TBarButton Key="btnStampaComune" Text="Stampa Per Comune" DisabledImage="~/Images/stampa.gif"
								Image="~/Images/Stampa.gif">
								<DefaultStyle Width="140px" CssClass="infratoolbar_button_default"></DefaultStyle>
							</igtbar:TBarButton>
							<igtbar:TBarButton Key="btnStampaCircoscriz" Text="Stampa Per Circoscrizione" DisabledImage="~/Images/stampa.gif"
								Image="~/Images/Stampa.gif">
							</igtbar:TBarButton>
							<igtbar:TBarButton Key="btnStampaTotCns" Text="Stampa Totali CV" DisabledImage="~/Images/stampa.gif"
								Image="~/Images/Stampa.gif">
								<DefaultStyle Width="140px" CssClass="infratoolbar_button_default"></DefaultStyle>
							</igtbar:TBarButton>	
							<igtbar:TBarButton Key="btnStampaEseMal" Text="Stampa Esenzione Malattia" DisabledImage="~/Images/stampa.gif"
								Image="~/Images/Stampa.gif">
							</igtbar:TBarButton>	
						</Items>
					</igtbar:UltraWebToolbar>
                </div>
				<div class="sezione" id="Panel23" runat="server">
					<asp:Label id="LayoutTitolo_sezioneRicerca" runat="server">Filtri di stampa</asp:Label>
                </div>
                
                <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                    
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset title="Comune di Residenza" class="fldroot">
								<legend class="label">Comune di Residenza</legend>
								<on_ofm:OnitModalList id="fmComuneRes" runat="server" SetUpperCase="True" UseCode="True" Tabella="T_ANA_COMUNI"
									CampoDescrizione="COM_DESCRIZIONE" CampoCodice="COM_CODICE" Label="Comune" CodiceWidth="28%" LabelWidth="-8px"
									PosizionamentoFacile="False" RaiseChangeEvent="True" Width="70%"></on_ofm:OnitModalList>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset title="Circoscrizione di Residenza" class="fldroot">
                                <legend class="label">Circoscrizione di Residenza</legend>
                                    <on_ofm:OnitModalList id="fmCircoscrizione" runat="server" SetUpperCase="True" UseCode="True" Tabella="T_ANA_CIRCOSCRIZIONI"
									    CampoDescrizione="CIR_DESCRIZIONE" CampoCodice="CIR_CODICE" Label="Circoscrizione" CodiceWidth="28%"
									    LabelWidth="-8px" PosizionamentoFacile="False" RaiseChangeEvent="True" Width="70%"></on_ofm:OnitModalList>
							</fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset title="Centro Vaccinale" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Centro Vaccinale</legend>
								<uc1:SelezioneConsultori id="ucSelezioneConsultori" runat="server" Width="100%"  MaxCnsInListaSelezionati="3"></uc1:SelezioneConsultori>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset title="Distretto (relativo al Centro Vaccinale)" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Distretto (relativo al Centro Vaccinale)</legend>
								<on_ofm:onitmodallist id="fmDistretto" runat="server" SetUpperCase="True" UseCode="True" Tabella="T_ANA_DISTRETTI"
									CampoDescrizione="DIS_DESCRIZIONE" CampoCodice="DIS_CODICE" Label="Distretto" CodiceWidth="28%" LabelWidth="-8px"
									PosizionamentoFacile="False" Width="70%"></on_ofm:onitmodallist>
							</fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset title="Tipo Centro Vaccinale" class="fldroot">
                                <legend class="label">Tipo Centro Vaccinale</legend>
								<asp:CheckBoxList id="chklTipoCns" runat="server" CssClass="textbox_stringa" RepeatDirection="Horizontal" Width="100%">
									<asp:ListItem Value="1">Adulti</asp:ListItem>
									<asp:ListItem Value="2">Pediatrico</asp:ListItem>
									<asp:ListItem Value="3">Vaccinatore</asp:ListItem>
								</asp:CheckBoxList>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset title="Tipo vaccinazioni" class="fldroot">
                                <legend class="label">Tipo vaccinazioni</legend>
								<asp:CheckBoxList id="chklModVaccinazione" runat="server" CssClass="textbox_stringa" RepeatDirection="Horizontal" Width="100%">
									<asp:ListItem Value="1">Obbligatorie</asp:ListItem>
									<asp:ListItem Value="2">Raccomandate</asp:ListItem>
									<asp:ListItem Value="3">Facoltative</asp:ListItem>
								</asp:CheckBoxList>
							</fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
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
                        <div class="vac-colonna-destra">
                            <fieldset title="Data di effettuazione" class="fldroot">
                                <legend class="label">Data di effettuazione</legend>
								<table style="width: 100%">
									<tr>
										<td class="label">
											<asp:Label id="lblDataEffettuazioneIniz" runat="server">Da</asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataEffettuazioneIniz" runat="server" Height="20px" CssClass="textbox_data_obbligatorio"  Width="136px" DateBox="True"></on_val:onitdatepick></td>
										<td class="label">
											<asp:Label id="lblDataEffettuazioneFin" runat="server">A</asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataEffettuazioneFin" runat="server" Height="20px" CssClass="textbox_data_obbligatorio" Width="136px" DateBox="True"></on_val:onitdatepick></td>
									</tr>
								</table>
							</fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <fieldset title="Stato Anagrafico" class="fldroot">
                            <legend class="label">Stato Anagrafico</legend>
						    <asp:CheckBoxList id="chklStatoAnagrafico" runat="server" CssClass="textbox_stringa" RepeatDirection="Horizontal" RepeatColumns="5" Width="100%"/>
					    </fieldset>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset  id="fldStatoVac" title="Stato vaccinazione" class="fldroot vac-fieldset-height-45">
								<legend class="label">Vaccini inseriti</legend>
								<asp:CheckBoxList id="chklStatoVac" runat="server" CssClass="textbox_stringa" Width="100%" RepeatDirection="Horizontal">
									<asp:ListItem Value="1" Selected="true">Vaccini Eseguiti</asp:ListItem>
									<asp:ListItem Value="2">Vaccini Registrati</asp:ListItem>
								</asp:CheckBoxList>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset title="Esenzione Malattia" class="fldroot vac-fieldset-height-45">
								<legend class="label">Esenzione Malattia</legend>
								<asp:DropDownList ID="ddlEsenzioniMalattie" width="100%" runat="server" />
							</fieldset>
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
