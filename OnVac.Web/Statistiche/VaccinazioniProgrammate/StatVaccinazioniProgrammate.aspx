<%@ Page Language="vb" AutoEventWireup="false" Codebehind="StatVaccinazioniProgrammate.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.StatVaccinazioniProgrammate"%>

<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="uc1" TagName="SelezioneConsultori" Src="../../Common/Controls/SelezioneConsultori.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>Statistica Vaccinazioni Programmate</title>
				
        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

        <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
		<script type="text/javascript">
		    function InizializzaToolBar(t) {
		        t.PostBackButton = false;
		    }
		
		    function ToolBarClick(ToolBar,button,evnt) {
			    evnt.needPostBack = true;
		
			    switch (button.Key) {
				    case 'btnStampa':
					
				        if (!checkCampiObbligatori()) {
				            evnt.needPostBack = false;
				        }
				        else {
				            var numeroDose = $.trim(document.getElementById('txtNumeroDose').value);
                            
				            if (numeroDose != '' && (isNaN(numeroDose) || (numeroDose <= 0) || (numeroDose > 100))) {
				                alert("Il campo 'Numero dose' deve essere un numero valido. Impossibile stampare il report.");
				                evnt.needPostBack = false;
                            }

                            var dataNascitaIniz = OnitDataPickGet('odpDataNascitaIniz');
                            var dataNascitaFin = OnitDataPickGet('odpDataNascitaFin');

                            if ((dataNascitaIniz == '' && dataNascitaFin != '') || (dataNascitaIniz != '' && dataNascitaFin == '')) {
                                alert("I campi 'Data Nascita' non sono impostati correttamente. Impossibile stampare il report.");
                                return false;
                            }

				        }
				        break;
			        case 'btnStampaAssociazioni':

			            if (!checkCampiObbligatori()) {
			                evnt.needPostBack = false;
			            }
			            else {
			                var numeroDose = $.trim(document.getElementById('txtNumeroDose').value);

			                if (numeroDose != '' && (isNaN(numeroDose) || (numeroDose <= 0) || (numeroDose > 100))) {
			                    alert("Il campo 'Numero dose' deve essere un numero valido. Impossibile stampare il report.");
			                    evnt.needPostBack = false;
                            }

                            var dataNascitaIniz = OnitDataPickGet('odpDataNascitaIniz');
                            var dataNascitaFin = OnitDataPickGet('odpDataNascitaFin');

                            if ((dataNascitaIniz == '' && dataNascitaFin != '') || (dataNascitaIniz != '' && dataNascitaFin == '')) {
                                alert("I campi 'Data Nascita' non sono impostati correttamente. Impossibile stampare il report.");
                                return false;
                            }

			            }
			            break;
			    }
		    }
		
		    //lancio della stampa tramite pulsante invio (modifica 29/07/2004)
		    function Stampa(evt) {
		        if (evt.keyCode == 13) {
		            if (checkCampiObbligatori())
		                __doPostBack("Stampa", "");
		        }
		    }
		
		    function checkCampiObbligatori() {
		        var dataNascitaIniz = OnitDataPickGet('odpDataNascitaIniz');
		        var dataNascitaFin = OnitDataPickGet('odpDataNascitaFin');

		        if ((dataNascitaIniz == '' && dataNascitaFin != '') || (dataNascitaIniz != '' && dataNascitaFin == '')) {
		            alert("I campi 'Data Nascita' non sono impostati correttamente. Impossibile stampare il report.");
		            return false;
		        }

		        if (OnitDataPickGet('odpDataAppuntamentoIniz') == '' || OnitDataPickGet('odpDataAppuntamentoFin') == '') {
		            alert("I campi 'Data Appuntamento' sono obbligatori. Impossibile stampare il report.");
		            return false;
		        }

		        return true;
		    }		
		</script>
	</head>
	<body>
		<form id="Form1" onkeyup="Stampa(event)" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" TitleCssClass="Title3" Titolo="Statistiche - <b>Vaccinazioni programmate</b>" height="100%" width="100%">
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
                            <igtbar:TBarButton Key="btnStampaAssociazioni" Text="Stampa Associazioni" DisabledImage="~/Images/stampa.gif"  Image="~/Images/Stampa.gif">
                                <DefaultStyle CssClass="infratoolbar_button_default" Width="150px"></DefaultStyle>
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
							<fieldset title="Centro Vaccinale" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Centro Vaccinale</legend>
								<uc1:SelezioneConsultori id="ucSelezioneConsultori" runat="server" Width="100%"  MaxCnsInListaSelezionati="3"></uc1:SelezioneConsultori>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
							<fieldset title="Medico di base" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Medico di base</legend>
								<on_ofm:OnitModalList id="fmMedico" runat="server" BackColor="White" CssClass="textbox_data" Width="70%"
									UseCode="True" Tabella="T_ANA_OPERATORI" CampoDescrizione="OPE_NOME" CampoCodice="OPE_CODICE" Label="Medico"
									CodiceWidth="28%" LabelWidth="-8px" PosizionamentoFacile="false" Filtro="1=1 ORDER BY OPE_NOME" SetUpperCase="True"></on_ofm:OnitModalList>
							</fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
						    <fieldset title="Data di nascita" class="fldroot">
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
							<fieldset title="Data appuntamento" class="fldroot">
                                <legend class="label">Data appuntamento</legend>
								<table style="width: 100%">
								    <tr>
									    <td class="label">
                                            <asp:Label id="lblDataAppuntamentoIniz" runat="server">Da</asp:Label></td>
										<td>
                                            <on_val:onitdatepick id="odpDataAppuntamentoIniz" runat="server" Height="20px" CssClass="textbox_data_obbligatorio" Width="136px" DateBox="True"></on_val:onitdatepick></td>
									    <td class="label">
                                            <asp:Label id="lblDataAppuntamentoFin" runat="server">A</asp:Label></td>
						                <td>
							                <on_val:onitdatepick id="odpDataAppuntamentoFin" runat="server" Height="20px" CssClass="textbox_data_obbligatorio" Width="136px" DateBox="True"></on_val:onitdatepick></td>
                                    </tr>
                                </table>
                            </fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <fieldset title="Stato anagrafico" class="fldroot">
							<legend class="label">Stato anagrafico</legend>
							<onit:CheckBoxList id="chklStatoAnagrafico" runat="server" CssClass="label_left" RepeatDirection="Horizontal" TextAlign="Right" RepeatColumns="5" Width="100%"></onit:CheckBoxList>
						</fieldset>
                    </div>
                    <div class="vac-riga">
                        <fieldset title="Vaccinazioni" class="fldroot">
                            <legend class="label">Vaccinazioni</legend>
                            <div class="label" style="float: left; width:80px;margin-right:3px">
                                Numero dose
                            </div>
                            <div style="float:left">
                                <asp:TextBox id="txtNumeroDose" runat="server" Width="100px" CssClass="textbox_stringa"></asp:TextBox>
                            </div>
                            <div>
                                <onit:CheckBoxList id="chklVaccinazioni" runat="server" cssclass="label_left" RepeatDirection="Horizontal" TextAlign="Right" RepeatColumns="5" Width="100%"></onit:CheckBoxList>
                            </div>
						</fieldset>
                    </div>
                       
                </dyp:DynamicPanel>

                <dyp:DynamicPanel ID="dypMsg" runat="server" Width="100%" Height="18px">
                    <asp:Label id="Label1" runat="server" CssClass="label_errorMsg" Width="100%" BorderColor="#8080FF" BorderWidth="1px" BorderStyle="Solid"></asp:Label>
                </dyp:DynamicPanel>

            </on_lay3:onitlayout3>
		</form>
	</body>
</html>