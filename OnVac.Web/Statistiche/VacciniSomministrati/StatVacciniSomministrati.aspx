<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="StatVacciniSomministrati.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.RilevazioneQuantitativa" %>

<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="uc1" TagName="SelezioneConsultori" Src="../../Common/Controls/SelezioneConsultori.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Statistica Vaccini Somministrati</title>
        
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <style type="text/css">
        .vac-fieldset-height-65 {
            height: 65px;
        }
    </style>
    <script type="text/javascript">
        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;

            // Controllo campi obbligatori
            if (!checkDateValorizzate()) {
                alert("Non è stata impostata nessuna data. Impossibile stampare il report.");
                evnt.needPostBack = false;
            }
        }

        function Stampa(evt) {
            if (evt.keyCode == 13) {
                if (!checkDateValorizzate()) {
                    alert("Non è stata impostata nessuna data. Impossibile stampare il report.");
                    evnt.needPostBack = false;
                }
                else {
                    __doPostBack("Stampa", "");
                }
            }
        }

        function checkDateValorizzate() {
            if ((OnitDataPickGet('odpDataNascitaIniz') == "" || OnitDataPickGet('odpDataNascitaFin') == "")
				&& (OnitDataPickGet('odpDataRegistrazioneIniz') == "" || OnitDataPickGet('odpDataRegistrazioneFin') == "")
				&& (OnitDataPickGet('odpDataEffettuazioneIniz') == "" || OnitDataPickGet('odpDataEffettuazioneFin') == "")) {
                return false;
            }

            return true;
        }	
    </script>
</head>
<body>
    <form id="Form1" onkeyup="Stampa(event)" method="post" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Titolo="Statistiche - <b>Vaccini somministrati</b>" TitleCssClass="Title3" Width="100%" Height="100%">
			<div class="title" id="PanelTitolo" runat="server">
				<asp:Label id="LayoutTitolo" runat="server"></asp:Label>
            </div>
            <div>
			    <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="Toolbar" runat="server" ItemWidthDefault="160" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                    <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
				    <Items>
					    <igtbar:TBarButton Key="btnStampa" DisabledImage="~/Images/stampa.gif" Text="Stampa" Image="~/Images/Stampa.gif">
                            <DefaultStyle Width="80px" CssClass="infratoolbar_button_default"></DefaultStyle>
                        </igtbar:TBarButton>
					    <igtbar:TBarButton Key="btnStampaAmb" DisabledImage="~/Images/stampa.gif" Text="Stampa per ambulatori" Image="~/Images/Stampa.gif">
                        </igtbar:TBarButton>
					    <igtbar:TBarButton Key="btnStampaCatRischio" DisabledImage="~/Images/stampa.gif" Text="Stampa Cat Rischio" Image="~/Images/Stampa.gif">
                        </igtbar:TBarButton>
					    <igtbar:TBarButton Key="btnStampaDose" DisabledImage="~/Images/stampa.gif" Text="Stampa Dose" Image="~/Images/Stampa.gif">
                            <DefaultStyle Width="120px" CssClass="infratoolbar_button_default"></DefaultStyle>
                        </igtbar:TBarButton>
					    <igtbar:TBarButton Key="btnStampaEseMal" Text="Stampa Esenzione Malattia" DisabledImage="~/Images/stampa.gif" Image="~/Images/Stampa.gif">
						    <DefaultStyle Width="200px" CssClass="infratoolbar_button_default"></DefaultStyle>
					    </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btnStampaNomeCommerciale" Text="Nome Commerciale" DisabledImage="~/Images/stampa.gif" Image="~/Images/Stampa.gif">
						    <DefaultStyle Width="170px" CssClass="infratoolbar_button_default"></DefaultStyle>
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
							<on_ofm:onitmodallist id="fmComuneRes" runat="server" SetUpperCase="True" UseCode="True" Tabella="T_ANA_COMUNI"
								CampoDescrizione="COM_DESCRIZIONE" CampoCodice="COM_CODICE" Label="Comune" CodiceWidth="28%" LabelWidth="-8px"
								PosizionamentoFacile="False" Width="70%" RaiseChangeEvent="True"></on_ofm:onitmodallist>
						</fieldset>
                    </div>
                    <div class="vac-colonna-destra">
                        <fieldset title="Circoscrizione di Residenza" class="fldroot">
							<legend class="label">Circoscrizione di Residenza</legend>
							<on_ofm:onitmodallist id="fmCircoscrizione" runat="server" SetUpperCase="True" UseCode="True" Tabella="T_ANA_CIRCOSCRIZIONI"
								CampoDescrizione="CIR_DESCRIZIONE" CampoCodice="CIR_CODICE" Label="Circoscrizione" CodiceWidth="28%"
								LabelWidth="-8px" PosizionamentoFacile="False" Width="70%" RaiseChangeEvent="True"></on_ofm:onitmodallist>
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
                        <fieldset title="Data effettuazione" class="fldroot vac-fieldset-height-45">
							<legend class="label">Date Effettuazione</legend>
							<table style="width: 100%">
								<tr>
									<td class="label">
                                        <asp:Label id="lblDataEffettuazioneIniz" runat="server">Da</asp:Label></td>
									<td>
										<on_val:onitdatepick id="odpDataEffettuazioneIniz" runat="server" Height="20px" CssClass="textbox_data" Width="136px" DateBox="True"></on_val:onitdatepick></td>
									<td class="label">
										<asp:Label id="lblDataEffettuazioneFin" runat="server">A</asp:Label></td>
									<td>
										<on_val:onitdatepick id="odpDataEffettuazioneFin" runat="server" Height="20px" CssClass="textbox_data" Width="136px" DateBox="True"></on_val:onitdatepick></td>
								</tr>
							</table>
						</fieldset>
                    </div>
                    <div class="vac-colonna-destra">
                        <fieldset title="Tipo centro vaccinale" class="fldroot vac-fieldset-height-45">
                            <legend class="label">Tipo Centro Vaccinale</legend>								        
							<asp:CheckBoxList id="chklTipoCns" runat="server" Width="100%" CssClass="label" TextAlign="Right" RepeatDirection="Horizontal">
								<asp:ListItem Value="1">Adulti</asp:ListItem>
								<asp:ListItem Value="2">Pediatrico</asp:ListItem>
								<asp:ListItem Value="3">Vaccinatore</asp:ListItem>
							</asp:CheckBoxList>
						</fieldset>
                    </div>
                </div>
                <div class="vac-riga">
                    <div class="vac-colonna-sinistra">
                        <fieldset title="Data di Registrazione" class="fldroot vac-fieldset-height-45" >
                            <legend class="label">Data di Registrazione</legend>
							<table style="width: 100%">
								<tr>
									<td class="label">
										<asp:Label id="lblDataRegistrazioneIniz" runat="server">Da</asp:Label></td>
									<td>
										<on_val:onitdatepick id="odpDataRegistrazioneIniz" runat="server" Height="20px" CssClass="textbox_data" Width="136px" DateBox="True"></on_val:onitdatepick></td>
									<td class="label">
										<asp:Label id="lblDataRegistrazioneFin" runat="server">A</asp:Label></td>
									<td>
										<on_val:onitdatepick id="odpDataRegistrazioneFin" runat="server" Height="20px" CssClass="textbox_data" Width="136px" DateBox="True"></on_val:onitdatepick></td>
								</tr>
							</table>
						</fieldset>
                    </div>
                    <div class="vac-colonna-destra">
                        <fieldset title="Stato vaccinazione" class="fldroot vac-fieldset-height-45">
                            <legend class="label">Vaccini Inseriti</legend>
                            <table style="width: 100%;">
                                <tr>
                                    <td>
                                        <asp:CheckBoxList id="chklStatoVac" runat="server" CssClass="label" RepeatDirection="Horizontal" Width="100%">
									        <asp:ListItem Value="1" Selected="true">Vaccini Eseguiti</asp:ListItem>
									        <asp:ListItem Value="2">Vaccini Registrati</asp:ListItem>
								        </asp:CheckBoxList>
                                    </td>
                                    <td>
                                        <asp:CheckBox id="chkFittizia" runat="server" CssClass="label" Text="Includi fittizi" />
                                    </td>
                                </tr>
                            </table>
						</fieldset>
                    </div>
                </div>
                <div class="vac-riga">
                    <div class="vac-colonna-sinistra">
                        <fieldset title="Data nascita" class="fldroot vac-fieldset-height-45">
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
                        <fieldset title="Medico in ambulatorio" class="fldroot vac-fieldset-height-45">
                            <legend class="label">Medico in ambulatorio</legend>
							<asp:RadioButtonList id="rblMedInAmb" runat="server" CssClass="label" RepeatDirection="Horizontal" Width="100%">
								<asp:ListItem Value="1">SI</asp:ListItem>
								<asp:ListItem Value="2">NO</asp:ListItem>
								<asp:ListItem Value="3" Selected="true">TUTTI</asp:ListItem>
							</asp:RadioButtonList>
						</fieldset>
                    </div>
                </div>
                <div class="vac-riga">
                    <div class="vac-colonna-sinistra">
                        <fieldset title="Categoria Rischio" class="fldroot vac-fieldset-height-45">
                            <legend class="label">Categoria rischio</legend>
							<asp:DropDownList id="cmbCatRischio" runat="server" Width="100%" DataTextField="RSC_DESCRIZIONE" DataValueField="RSC_CODICE" Enabled="true"></asp:DropDownList>
						</fieldset>
                    </div>
                    <div class="vac-colonna-destra">
                        <fieldset title="Esenzione Malattia" class="fldroot vac-fieldset-height-45">
						    <legend class="label">Esenzione Malattia</legend>
							<asp:DropDownList ID="ddlEsenzioniMalattie" width="100%" runat="server" />
						</fieldset>
                    </div>
                </div>
                <div class="vac-riga">
                    <fieldset title="Stato anagrafico" class="fldroot">
                        <legend class="label">Stato anagrafico</legend>
                        <onit:CheckBoxList id="chklStatoAnagrafico" runat="server" CssClass="textbox_stringa" RepeatDirection="Horizontal" RepeatColumns="5" Width="100%" />
					</fieldset>
                </div>
                <div class="vac-riga">
                    <fieldset title="Note" class="fldroot vac-fieldset-height-65">
                        <legend class="label">Note</legend>
						<asp:Label id="Label5" runat="server" Width="100%" CssClass="label_left">
							Se viene scelto <b>"Vaccini Registrati"</b> il raggruppamento avviene per centro vaccinale di <b>registrazione</b>, altrimenti per centro vaccinale di <b>esecuzione</b>. </br>La <b>"Stampa Dose" </b> richiede che le vaccinazioni componenti le associazioni vaccinali abbiano lo stesso numero di dose per essere conteggiate correttamente.
						</asp:Label>
					</fieldset>
                </div>
			</dyp:DynamicPanel>
            
            <dyp:DynamicPanel ID="dypMsg" runat="server" Width="100%" Height="18px">
			    <asp:Label id="lblErrorMessage" runat="server" Width="100%" CssClass="label_errormsg"></asp:Label>
            </dyp:DynamicPanel>
        </on_lay3:OnitLayout3>
    </form>
</body>
</html>
