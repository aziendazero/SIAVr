<%@ Page Language="vb" AutoEventWireup="false" Codebehind="StatElencoNonVaccinati.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.StatElencoNonVaccinati"%>

<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="uc1" TagName="SelezioneConsultori" Src="../../Common/Controls/SelezioneConsultori.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>ElencoNonVaccinati</title>

        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

		<script type="text/javascript" language="javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.StringUtility.js") %>"></script>
		<script type="text/javascript">
		    function InizializzaToolBar(t) {
		        t.PostBackButton = false;
		    }

		    function ToolBarClick(ToolBar, button, evnt) {
		        evnt.needPostBack = true;

		        switch (button.Key) {
		            case 'btnStampa':
		                if (!CheckCampiObbligatori()) {
		                    evnt.needPostBack = false;
		                }
		                break;
		        }
		    }
		
		    //lancio della stampa tramite tasto invio (modifica 29/07/2004)
		    function Stampa(evt) {
		        if (evt.keyCode == 13) {
		            if (!CheckCampiObbligatori()) {
		                evnt.needPostBack = false;
		            }
		            else {
		                __doPostBack("Stampa", "");
		            }
		        }
		    }
		
		    // Controllo campi obbligatori
		    function CheckCampiObbligatori() {
		        var seduta = document.getElementById('txtSeduta').value;
		        if (OnitDataPickGet('odpDataNascitaIniz') == "" || OnitDataPickGet('odpDataNascitaFin') == "" || FullTrim(seduta) == "" || isNaN(seduta)) {
		            alert("Non tutti i campi obbligatori sono impostati correttamente. Impossibile stampare il report.");
		            return false;
		        }

		        return true;
		    }	
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server" onkeyup="Stampa(event)">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" Titolo="Statistiche - <b>Elenco non vaccinati </b>" TitleCssClass="Title3" width="100%" height="100%">
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
						    <igtbar:TBarButton Key="btnStampa" ToolTip="Non considera le vaccinazioni escluse o con inadempienza. "
							    Text="Stampa" DisabledImage="~/Images/stampa.gif" Image="~/Images/Stampa.gif"></igtbar:TBarButton>
					    </Items>
				    </igtbar:UltraWebToolbar>
                </div>
				<div class="sezione" id="Panel23" runat="server">
					<asp:Label id="LayoutTitolo_sezioneRicerca" runat="server">Filtri di stampa</asp:Label>
                </div>
                
                <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
						
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
							<fieldset title="Comune" class="fldroot" >
                                <legend class="label">Comune di Residenza</legend>
								<on_ofm:onitmodallist id="fmComuneRes" runat="server" Width="70%" PosizionamentoFacile="False" LabelWidth="-8px"
									CodiceWidth="29%" Label="Comune" CampoCodice="COM_CODICE" CampoDescrizione="COM_DESCRIZIONE" Tabella="T_ANA_COMUNI"
									UseCode="True" SetUpperCase="True" RaiseChangeEvent="True"></on_ofm:onitmodallist>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset title="Circoscrizione" class="fldroot">
                                <legend class="label">Circoscrizione</legend>
								<on_ofm:onitmodallist id="fmCircoscrizione" runat="server" Width="70%" PosizionamentoFacile="False" LabelWidth="-8px"
									CodiceWidth="29%" Label="Circoscrizione" CampoCodice="CIR_CODICE" CampoDescrizione="CIR_DESCRIZIONE"
									Tabella="T_ANA_CIRCOSCRIZIONI" UseCode="True" SetUpperCase="True" RaiseChangeEvent="True"></on_ofm:onitmodallist>
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
                            <fieldset title="Distretto" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Distretto</legend>
                                <on_ofm:onitmodallist id="fmDistretto" runat="server" Width="70%" PosizionamentoFacile="False" LabelWidth="-8px"
									CodiceWidth="29%" Label="Distretto" CampoCodice="DIS_CODICE" CampoDescrizione="DIS_DESCRIZIONE" Tabella="T_ANA_DISTRETTI"
									UseCode="True" SetUpperCase="True" RaiseChangeEvent="True"></on_ofm:onitmodallist>
							</fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset id="fldDataNasc" title="Data nascita" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Data di nascita</legend>
								<table style="width: 100%">
									<tr>
										<td class="label_right">
											<asp:Label id="lblDataEffettuazioneIniz" runat="server" CssClass="label">Da</asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataNascitaIniz" runat="server" CssClass="textbox_data_obbligatorio" DateBox="True" Width="136px"></on_val:onitdatepick></td>
										<td class="label_right">
											<asp:Label id="lblDataEffettuazioneFin" runat="server">A</asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataNascitaFin" runat="server" CssClass="textbox_data_obbligatorio" DateBox="True" Width="136px"></on_val:onitdatepick></td>
									</tr>
								</table>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset id="fldVac" title="Vaccinazioni" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Vaccinazioni</legend>
								<asp:RadioButtonList id="radVaccinazioni" runat="server" Width="100%" CssClass="textbox_stringa" RepeatDirection="Horizontal">
									<asp:ListItem Value="A">Obbligatorie</asp:ListItem>
									<asp:ListItem Value="T" Selected="True">Tutte</asp:ListItem>
								</asp:RadioButtonList>
							</fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <fieldset id="fldSraroAn" title="Stato anagrafico" class="fldroot" >
                            <legend class="label">Stato anagrafico</legend>
							<asp:CheckBoxList id="chkStatoAnagrafico" runat="server" CssClass="textbox_stringa" RepeatColumns="5" RepeatDirection="Horizontal" Width="100%"></asp:CheckBoxList>
						</fieldset>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset title="Ciclo" class="fldroot">
                                <legend class="label">Ciclo</legend>
							    <asp:DropDownList id="cmbCiclo" runat="server" Width="100%"></asp:DropDownList>								
						    </fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset id="fldSeduta" title="Seduta" class="fldroot">
                                <legend class="label">Seduta</legend>
                                <asp:TextBox id="txtSeduta" runat="server" width="62px" CssClass="textbox_stringa_obbligatorio"></asp:TextBox>
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
