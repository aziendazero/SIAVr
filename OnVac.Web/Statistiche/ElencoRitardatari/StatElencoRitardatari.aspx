<%@ Page Language="vb" AutoEventWireup="false" Codebehind="StatElencoRitardatari.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.ElencoRitardatari" %>

<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="uc1" TagName="SelezioneConsultori" Src="../../Common/Controls/SelezioneConsultori.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>Statistica Elenco Ritardatari</title>
		
        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

		<script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
        <script type="text/javascript">
		    function InizializzaToolBar(t) {
		        t.PostBackButton = false;
		    }

		    function ToolBarClick(ToolBar, button, evnt) {
		        evnt.needPostBack = true;

		        if (!checkDosi()) {
		            alert("Impossibile stampare il report: il campo 'Numero Dosi' deve essere un numero valido.");
		            evnt.needPostBack = false;
		            return false;
		        }

		        switch (button.Key) {
		            case 'btnStampa':
		                if (OnitDataPickGet('odpDataConvocazioneFin') == "") {
		                    alert("Impossibile stampare il report: il campo 'Data di Convocazione A' non è valorizzato.");
		                    evnt.needPostBack = false;
		                }
		                break;
		        }
		    }

		    //lancio della stampa tramite pulsante invio (modifica 29/07/2004)
		    function Stampa(evt) {
		        if (evt.keyCode == 13) {
		            if (!checkDosi()) {
		                alert("Impossibile stampare il report: il campo 'Numero Dosi' deve essere un numero valido.");
		                evt.needPostBack = false;
		                return false;
		            }
		            if (OnitDataPickGet('odpDataConvocazioneFin') == "") {
		                alert("Impossibile stampare il report: il campo 'Data di Convocazione A' non è valorizzato.");
		                evt.needPostBack = false;
		                return false;
		            }
		            else {
		                __doPostBack("Stampa", "");
		            }
		        }
		    }

		    function checkDosi() {
		        val = document.getElementById('txtNumeroDosi').value;
		        if (isNaN(val) || (val > 100)) {
		            return false;
		        }
		        return true;
		    }            	
		</script>
        <style type="text/css">
            .margin-bottom-5 {
                margin-bottom: 5px;
            }
        </style>
	</head>
	<body>
		<form id="Form1" method="post" runat="server" onkeyup="Stampa(event)">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" height="100%" width="100%" Titolo="Statistiche - <b>Elenco ritardatari</b>" TitleCssClass="Title3">
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
                            <fieldset title="Centro Vaccinale" class="fldroot" style="height:40px">
                                <legend class="label">Centro Vaccinale</legend>
								<uc1:SelezioneConsultori id="ucSelezioneConsultori" runat="server" Width="100%"  MaxCnsInListaSelezionati="3"></uc1:SelezioneConsultori>								
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset title="Distretto" class="fldroot" style="height:40px">
                                <legend class="label">Distretto</legend>
                                <on_ofm:onitmodallist id="fmDistretto" runat="server" SetUpperCase="True" UseCode="True" Tabella="T_ANA_DISTRETTI"
									CampoDescrizione="DIS_DESCRIZIONE" CampoCodice="DIS_CODICE" Label="Distretto" CodiceWidth="28%" LabelWidth="-8px"
									PosizionamentoFacile="False" Width="70%" RaiseChangeEvent="True"></on_ofm:onitmodallist>
							</fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset title="Data Convocazione" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Data di Convocazione</legend>
							    <table style="width: 100%">
								    <tr>
									    <td class="label">
										    <asp:Label id="lblDataConvocazioneIniz" runat="server">Da</asp:Label></td>
									    <td>
										    <on_val:onitdatepick id="odpDataConvocazioneIniz" runat="server" Height="20px" Width="136px" CssClass="textbox_data" DateBox="True"></on_val:onitdatepick></td>
						                <td class="label">
							                <asp:Label id="lblDataConvocazioneFin" runat="server">A</asp:Label></td>
						                <td>
							                <on_val:onitdatepick id="odpDataConvocazioneFin" runat="server" Height="20px" Width="136px" CssClass="textbox_data_obbligatorio" DateBox="True"></on_val:onitdatepick></td>
					                </tr>
				                </table>
                            </fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset title="Numero sollecito" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Numero del Sollecito</legend>
							    <table style="width: 100%">
								    <tr>
									    <td>
										    <asp:RadioButtonList id="rdlNumeroSollecito" runat="server" CssClass="label" TextAlign="Right" RepeatDirection="Horizontal">
											    <asp:ListItem Value="1" Selected="True">Primo</asp:ListItem>
											    <asp:ListItem Value="2">Secondo</asp:ListItem>
											    <asp:ListItem Value="3">Terzo</asp:ListItem>
											    <asp:ListItem Value="4">Quarto</asp:ListItem>
										    </asp:RadioButtonList>
                                        </td>
									    <td>
										    <asp:CheckBox id="chkEsatta" runat="server" Width="100%" CssClass="label" TextAlign="right" Text="Ricerca Esatta"></asp:CheckBox>
                                        </td>
								    </tr>
							    </table>
						    </fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset title="Data di nascita" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Data di Nascita</legend>
							    <table style="width: 100%">
								    <tr>
									    <td class="label">
										    <asp:Label id="lblDataNascitaDa" runat="server">Da</asp:Label></td>
									    <td>
										    <on_val:onitdatepick id="dpkDataNascitaDa" runat="server" Height="20px" Width="136px" CssClass="textbox_data" DateBox="True"></on_val:onitdatepick></td>
						                <td class="label">
							                <asp:Label id="lblDataNascitaA" runat="server">A</asp:Label></td>
						                <td>
							                <on_val:onitdatepick id="dpkDataNascitaA" runat="server" Height="20px" Width="136px" CssClass="textbox_data" DateBox="True"></on_val:onitdatepick></td>
					                </tr>
				                </table>
                            </fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset title="Numero Dosi" class="fldroot vac-fieldset-height-45">
                                <legend class="label margin-bottom-5">Numero Dosi</legend>
							    <asp:TextBox id="txtNumeroDosi" Width="62px" runat="server" CssClass="textbox_stringa"></asp:TextBox>
                            </fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <fieldset title="Stati anagrafici" class="fldroot">
                            <legend class="label">Stati Anagrafici</legend>
                            <onit:CheckBoxList id="chklStatoAnagrafico" runat="server" CssClass="label_left" RepeatDirection="Horizontal" TextAlign="Right" RepeatColumns="5" Width="100%"></onit:CheckBoxList>
                        </fieldset>
                    </div>
                    <div class="vac-riga">
                        <fieldset id="fldVaccinazioni" title="Vaccinazioni" class="fldroot">
                            <legend class="label">Vaccinazioni</legend>
							<onit:CheckBoxList id="chkVaccinazioni" runat="server" CssClass="label_left" RepeatDirection="Horizontal" TextAlign="Right" RepeatColumns="5" Width="100%"></onit:CheckBoxList>
						</fieldset>
                    </div>
                </dyp:DynamicPanel>
            </on_lay3:onitlayout3>
		</form>
	</body>
</html>