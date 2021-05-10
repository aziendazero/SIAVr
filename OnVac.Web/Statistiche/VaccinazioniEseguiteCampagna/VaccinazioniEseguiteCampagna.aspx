<%@ Page Language="vb" AutoEventWireup="false" Codebehind="VaccinazioniEseguiteCampagna.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.VaccinazioniEseguiteCampagna" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>Statistica Vaccinazioni Eseguite Campagna</title>

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
		            alert("Non tutti i campi obbligatori sono impostati.Impossibile stampare il report!!");
		            return false;
		        }
		        return true;
		    }		
		</script>
	</head>
	<body>
		<form id="Form1" onkeyup="Stampa(event)" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" TitleCssClass="Title3" Titolo="Statistiche - <b>Vaccinazioni eseguite campagna</b>" width="100%" height="100%">
                
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
                            <fieldset title="Data convocazione" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Data di convocazione</legend>
                                <on_val:onitdatepick id="odpData" runat="server" CssClass="textbox_data" DateBox="True"></on_val:onitdatepick>
                            </fieldset> 
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset title="Data nascita" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Data di nascita</legend>
							    <table style="width: 100%">
                                    <tr>
										<td class="label">
											<asp:Label id="lblDataNascitaIniz" runat="server">Da</asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataNascitaIniz" runat="server" CssClass="textbox_data" Width="136px" DateBox="True"></on_val:onitdatepick></td>
					                    <td class="label">
						                    <asp:Label id="lblDataNascitaFin" runat="server">A</asp:Label></td>
					                    <td>
						                    <on_val:onitdatepick id="odpDataNascitaFin" runat="server" CssClass="textbox_data" Width="136px" DateBox="True"></on_val:onitdatepick></td>
				                    </tr>
                                </table>
                            </fieldset>                         
                        </div>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset title="Data di effettuazione" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Data di effettuazione</legend>
							    <table style="width: 100%">
								    <tr>
									    <td class="label">
										    <asp:Label id="lblDataEffettuazioneIniz" runat="server">Da</asp:Label></td>
									    <td>
										    <on_val:onitdatepick id="odpDataEffettuazioneIniz" runat="server" CssClass="textbox_data_obbligatorio" Width="136px" DateBox="True"></on_val:onitdatepick></td>
									    <td class="label">
										    <asp:Label id="lblDataEffettuazioneFin" runat="server">A</asp:Label></td>
									    <td>
										    <on_val:onitdatepick id="odpDataEffettuazioneFin" runat="server" CssClass="textbox_data_obbligatorio" Width="136px" DateBox="True"></on_val:onitdatepick></td>
								    </tr>
							    </table>
						    </fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset title="Tipo vaccinazioni" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Tipo vaccinazioni</legend>
							    <asp:CheckBoxList id="chklModVaccinazione" runat="server" CssClass="label" TextAlign="Right" RepeatDirection="Horizontal" Width="100%">
									<asp:ListItem Value="1">Obbligatorie</asp:ListItem>
									<asp:ListItem Value="2">Raccomandate</asp:ListItem>
									<asp:ListItem Value="3">Facoltative</asp:ListItem>
								</asp:CheckBoxList>
						    </fieldset>
                        </div>
                    </div>
                </dyp:DynamicPanel>

                <dyp:DynamicPanel ID="dypMsg" runat="server" Width="100%" Height="18px">
					<asp:Label id="Label1" runat="server" CssClass="label_errormsg" Width="100%"></asp:Label>
				</dyp:DynamicPanel>

			</on_lay3:onitlayout3>
		</form>
	</body>
</html>
