<%@ Page Language="vb" AutoEventWireup="false" Codebehind="StatElencoSospesi.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.StatElencoSospesi"%>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="uc1" TagName="SelezioneConsultori" Src="../../Common/Controls/SelezioneConsultori.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>Statistica Elenco Sospesi</title>

        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

		<script type="text/javascript" language="javascript">
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
		
		    //lancio della stampa tramite pulsante invio (modifica 29/07/2004)
		    function Stampa(evt) {
		        if (evt.keyCode == 13) {
		            if (!CheckCampiObbligatori()) {
		                evt.needPostBack = false;
		            }
		            else {
		                __doPostBack("Stampa", "");
		            }
		        }
		    }
		
		    function CheckCampiObbligatori() {
		        var dataNascitaIniz = OnitDataPickGet('odpDataNascitaIniz');
		        var dataNascitaFin = OnitDataPickGet('odpDataNascitaFin');
		        var dataSospensioneIniz = OnitDataPickGet('odpDataSospensioneIniz');
		        var dataSospensioneFin = OnitDataPickGet('odpDataSospensioneFin');

		        if ((dataNascitaIniz == '' && dataNascitaFin != '') || (dataNascitaIniz != '' && dataNascitaFin == '')) {
		            alert("I campi 'Data Nascita' non sono impostati correttamente. Impossibile stampare il report.");
		            return false;
		        }
		        else {
		            if ((dataSospensioneIniz == '' && dataSospensioneFin != '') || (dataSospensioneIniz != '' && dataSospensioneFin == '')) {
		                alert("I campi 'Data Sospensione' non sono impostati correttamente. Impossibile stampare il report.");
		                return false;
		            }
		        }

		        return true;
		    }
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server" onkeyup="Stampa(event)">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" TitleCssClass="Title3" Titolo="Statistiche - <b>Elenco sospesi</b>" height="100%" width="100%">
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
                            <fieldset title="Centro Vaccinale" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Centro Vaccinale</legend>
								<uc1:SelezioneConsultori id="ucSelezioneConsultori" runat="server" Width="100%"  MaxCnsInListaSelezionati="3"></uc1:SelezioneConsultori>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset title="Distretto" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Distretto</legend>
                                <on_ofm:OnitModalList id="fmDistretto" runat="server" RaiseChangeEvent="True" PosizionamentoFacile="False"
								    LabelWidth="-8px" CodiceWidth="28%" Label="Distretto" CampoCodice="DIS_CODICE" CampoDescrizione="DIS_DESCRIZIONE"
								    Tabella="T_ANA_DISTRETTI" UseCode="True" Width="70%" SetUpperCase="True"></on_ofm:OnitModalList>
							</fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset title="Data di nascita" class="fldroot">
                                <legend class="label">Data di nascita</legend>
								<table>
									<tr>
										<td class="label">Da</td>
										<td>
											<on_val:onitdatepick id="odpDataNascitaIniz" runat="server" Height="20px" Width="136px" CssClass="textbox_data" DateBox="True"></on_val:onitdatepick></td>
						                <td class="label">A</td>
						                <td>
							                <on_val:onitdatepick id="odpDataNascitaFin" runat="server" Height="20px" Width="136px" CssClass="textbox_data" DateBox="True"></on_val:onitdatepick></td>
                                    </tr>
                                </table>
                            </fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset title="Data di sospensione" class="fldroot">
                                <legend class="label">Data di sospensione</legend>
							    <table>
                                    <tr>
                                        <td class="label">Da</td>
										<td>
											<on_val:onitdatepick id="odpDataSospensioneIniz" runat="server" Height="20px" Width="136px" CssClass="textbox_data" DateBox="True"></on_val:onitdatepick></td>
                                        <td class="label">A</td>
                                        <td>
						                    <on_val:onitdatepick id="odpDataSospensioneFin" runat="server" Height="20px" Width="136px" CssClass="textbox_data" DateBox="True"></on_val:onitdatepick></td>
                                    </tr>
                                </table>
                            </fieldset>
                        </div>
                    </div>
                </dyp:DynamicPanel>

                <dyp:DynamicPanel ID="dypMsg" runat="server" Width="100%" Height="18px">
                    <asp:Label id="Label1" runat="server" Width="100%" CssClass="label_errorMsg"></asp:Label>
                </dyp:DynamicPanel>

            </on_lay3:onitlayout3>
        </form>
	</body>
</html>