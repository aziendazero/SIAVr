<%@ Page Language="vb" AutoEventWireup="false" Codebehind="EsitoCampagna.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.EsitoCampagna"%>

<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="uc1" TagName="SelezioneConsultori" Src="../../Common/Controls/SelezioneConsultori.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>Esito Campagne Vaccinali</title>

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
		                if (OnitDataPickGet('odpDataNascitaDa') == "" || OnitDataPickGet('odpDataNascitaA') == "") {
		                    alert("Non tutti i campi obbligatori sono impostati. Impossibile stampare il report.");
		                    evnt.needPostBack = false;
		                }
		        }
		    }
	
		    function Stampa(evt) {
		        if (evt.keyCode == 13) {
		            if (OnitDataPickGet('odpDataNascitaDa') == "" || OnitDataPickGet('odpDataNascitaA') == "") {
		                alert("Non tutti i campi obbligatori sono impostati. Impossibile stampare il report.");
		                evnt.needPostBack = false;
		            }
		            else {
		                __doPostBack("Stampa", "");
		            }
		        }
		    }		
		</script>
        <style type="text/css">
            .vac-fieldset-height-45 legend {
                margin-bottom: 5px;
            }
        </style>
	</head>
	<body>
		<form id="Form1" method="post" runat="server" onkeyup="Stampa(event)">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" height="100%" width="100%" Titolo="Statistiche - <b>Esito campagne vaccinali</b>" TitleCssClass="Title3">
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
                            <fieldset title="Data di nascita" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Data di Nascita</legend>
								<table>
                                    <tr>
                                        <td class="label">Da</td>
            							<td>
                                            <on_val:onitdatepick id="odpDataNascitaDa" tabIndex="2" runat="server" Height="20px" Width="136px" CssClass="textbox_data_obbligatorio" DateBox="True"></on_val:onitdatepick></td>
										<td class="label">A</td>
                                        <td>
                                            <on_val:onitdatepick id="odpDataNascitaA" tabIndex="3" runat="server" Height="20px" Width="136px" CssClass="textbox_data_obbligatorio" DateBox="True"></on_val:onitdatepick></td>
									</tr>
                                </table>
				            </fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset title="Data convocazione" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Data di convocazione</legend>
                                <on_val:onitdatepick id="odpData" tabIndex="4" runat="server" Height="20px" Width="136px" CssClass="textbox_data" DateBox="True"></on_val:onitdatepick>
				            </fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset title="Data di effettuazione" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Data Effettuazione</legend>
							    <table>
                                    <tr>
									    <td class="label">Da</td>
									    <td>
										    <on_val:onitdatepick id="odpDataEffettuazioneIniz" tabIndex="5" runat="server" Height="20px" Width="136px" CssClass="textbox_data" DateBox="True"></on_val:onitdatepick></td>
									    <td class="label">A</td>
									    <td>
										    <on_val:onitdatepick id="odpDataEffettuazioneFin" tabIndex="6" runat="server" Height="20px" Width="136px" CssClass="textbox_data" DateBox="True"></on_val:onitdatepick></td>
                                    </tr>
							    </table>
						    </fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <fieldset title="Note" class="fldroot">
						    <legend class="label">Note</legend>
							<asp:Label id="Label2" runat="server" Width="100%" CssClass="label_left">
                                Il conteggio dei <b>pazienti vaccinati</b> avviene in base al centro vaccinale che ha <b>generato la campagna</b> e non in base al centro vaccinale in cui il paziente si è recato a vaccinarsi.
                            </asp:Label>
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