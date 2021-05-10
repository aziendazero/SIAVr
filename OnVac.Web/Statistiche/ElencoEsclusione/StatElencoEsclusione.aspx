<%@ Page Language="vb" AutoEventWireup="false" Codebehind="StatElencoEsclusione.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.ElencoEsclusione" %>

<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="uc1" TagName="SelezioneConsultori" Src="../../Common/Controls/SelezioneConsultori.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>Elenco Esclusione</title>

        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

		<script type="text/javascript" language="javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.StringUtility.js") %>"></script>
		<script type="text/javascript" language="javascript">
		    function InizializzaToolBar(t) {
		        t.PostBackButton = false;
		    }

		    function ToolBarClick(ToolBar, button, evnt) {
		        evnt.needPostBack = true;

		        switch (button.Key) {
		            case 'btnStampa':
		            case 'btnStampaEsclusioni':
		                if (OnitDataPickGet('odpDataNascitaIniz') == "" || OnitDataPickGet('odpDataNascitaFin') == "") {
		                    alert("Non tutti i campi obbligatori sono impostati. Impossibile stampare il report.");
		                    evnt.needPostBack = false;
		                }
		                break;
		        }
		    }
		
		    //lancio della stampa tramite tasto invio (modifica 29/07/2004)
		    function Stampa(evt) {
		        if (evt.keyCode == 13) {
		            if (OnitDataPickGet('odpDataNascitaIniz') == "" || OnitDataPickGet('odpDataNascitaFin') == "") {
		                alert("Non tutti i campi obbligatori sono impostati. Impossibile stampare il report.");
		            }
		            else {
		                document.getElementById('Label1').appendChild(document.createTextNode('Attendere: creazione del report in corso...'));
		                __doPostBack("Stampa", "");
		            }
		        }
		    }
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server" onkeyup="Stampa(event)">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" Titolo="Statistiche - <b>Elenco motivi di esclusione </b>" TitleCssClass="Title3" width="100%" height="100%">
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
							<igtbar:TBarButton Key="btnStampa" ToolTip="Stampa i pazienti con vaccinazioni escluse." Text="Stampa"
								DisabledImage="~/Images/stampa.gif" Image="~/Images/Stampa.gif"></igtbar:TBarButton>
							<igtbar:TBarButton Key="btnStampaEsclusioni" ToolTip="Stampa le vaccinazioni con pazienti esclusi."
								Text="Stampa per vaccinazioni" DisabledImage="~/Images/stampa.gif" Image="~/Images/Stampa.gif">
								<DefaultStyle Width="190px" CssClass="infratoolbar_button_default"></DefaultStyle>
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
                            <fieldset id="fldCom" title="Comune" class="fldroot" >
                                <legend class="label">Comune di Residenza</legend>
                                <on_ofm:onitmodallist id="fmComuneRes" runat="server" Width="70%" PosizionamentoFacile="False" LabelWidth="-8px"
									CodiceWidth="28%" Label="Comune" CampoCodice="COM_CODICE" CampoDescrizione="COM_DESCRIZIONE" Tabella="T_ANA_COMUNI"
									UseCode="True" SetUpperCase="True" RaiseChangeEvent="True"></on_ofm:onitmodallist>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
							<fieldset title="Circoscrizione" class="fldroot">
                                <legend class="label">Circoscrizione</legend>
								<on_ofm:onitmodallist id="fmCircoscrizione" runat="server" Width="70%" PosizionamentoFacile="False" LabelWidth="-8px"
									CodiceWidth="28%" Label="Circoscrizione" CampoCodice="CIR_CODICE" CampoDescrizione="CIR_DESCRIZIONE"
									Tabella="T_ANA_CIRCOSCRIZIONI" UseCode="True" SetUpperCase="True" RaiseChangeEvent="True"></on_ofm:onitmodallist>
							</fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset title="Consultorio" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Centro Vaccinale</legend>
								<uc1:SelezioneConsultori id="ucSelezioneConsultori" runat="server" Width="100%"  MaxCnsInListaSelezionati="3"></uc1:SelezioneConsultori>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
							<fieldset title="Distretto" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Distretto</legend>
								<on_ofm:onitmodallist id="fmDistretto" runat="server" Width="80%" PosizionamentoFacile="False" LabelWidth="-8px"
									CodiceWidth="20%" Label="Distretto" CampoCodice="DIS_CODICE" CampoDescrizione="DIS_DESCRIZIONE" Tabella="T_ANA_DISTRETTI"
									UseCode="True" SetUpperCase="True" RaiseChangeEvent="True"></on_ofm:onitmodallist>
							</fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset title="Data nascita" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Data di nascita</legend>
								<table style="width: 100%">
									<tr>
										<td class="label_right">
											<asp:Label id="lblDataEffettuazioneIniz" runat="server">Da</asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataNascitaIniz" runat="server" CssClass="textbox_data_obbligatorio" DateBox="True" Width="136px"></on_val:onitdatepick></td>
										<td class="label_right">
											<asp:Label id="lblDataEffettuazioneFin" runat="server">A</asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataNascitaFin" runat="server" CssClass="textbox_data_obbligatorio" DateBox="True"  Width="136px"></on_val:onitdatepick></td>
									</tr>
								</table>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
							<fieldset title="Vaccinazioni" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Vaccinazioni</legend>
								<asp:RadioButtonList id="radVaccinazioni" runat="server" Width="100%" CssClass="textbox_stringa" RepeatDirection="Horizontal">
									<asp:ListItem Value="A">Obbligatorie</asp:ListItem>
									<asp:ListItem Value="T" Selected="True">Tutte</asp:ListItem>
								</asp:RadioButtonList>
							</fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset title="Motivi esclusione" class="fldroot">
                                <legend class="label">Motivi di esclusione</legend>
                                <asp:DropDownList id="cmbMotiviEsclusione" runat="server" Width="100%" DataValueField="MOE_CODICE" DataTextField="MOE_DESCRIZIONE"></asp:DropDownList>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
							<fieldset title="Vaccinazioni" class="fldroot">
                                <legend class="label">Elenco vaccinazioni</legend>
								<asp:DropDownList id="cmbVaccinazioni" runat="server" Width="100%" DataValueField="VAC_CODICE" DataTextField="VAC_DESCRIZIONE"></asp:DropDownList>
							</fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <fieldset title="Stati anagrafici" class="fldroot">
                            <legend class="label">Stati Anagrafici</legend>
							<onit:CheckBoxList id="chklStatoAnagrafico" runat="server" CssClass="label_left" RepeatDirection="Horizontal" TextAlign="Right" RepeatColumns="5" Width="100%"></onit:CheckBoxList>
                        </fieldset> 
                    </div>
                </dyp:DynamicPanel>

                <dyp:DynamicPanel ID="dypMsg" runat="server" Width="100%" Height="18px">
                    <asp:Label id="Label1" runat="server" Width="100%" CssClass="label_errormsg"></asp:Label>
                </dyp:DynamicPanel>

			</on_lay3:onitlayout3>
		</form>
	</body>
</html>
