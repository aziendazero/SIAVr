<%@ Page Language="vb" AutoEventWireup="false" Codebehind="StatAvvisoMantoux.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.StatAvvisoMantoux"%>

<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="uc1" TagName="SelezioneConsultori" Src="../../Common/Controls/SelezioneConsultori.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>Avviso Mantoux</title>
        
        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

        <style type="text/css">
            fieldset legend {
                margin-bottom: 5px;
            }
            
            fieldset span label {
                margin-right: 30px;
            }
        </style>
		<script type="text/javascript">
		
		    // inizializzazione della toolbar
		    function InizializzaToolBar(t) {
		        t.PostBackButton = false;
		    }
		
		    //controllo valore dei datepick
		    function ToolBarClick(ToolBar, button, evnt) {
		        evnt.needPostBack = true;

		        if (OnitDataPickGet('odpDataMantouxDa') == "" || OnitDataPickGet('odpDataMantouxA') == "") {

		            alert("Non tutti i campi obbligatori sono impostati. Impossibile stampare il report.");
		            evnt.needPostBack = false;
		        }

		        if ((OnitDataPickGet('odpDataInvioDa') == "" && OnitDataPickGet('odpDataInvioA') != "") ||
                    (OnitDataPickGet('odpDataInvioDa') != "" && OnitDataPickGet('odpDataInvioA') == "")) {

		            alert("Se si vuole specificare un intervallo di invio, le date di inizio e di fine devono essere compilate entrambe. Impossibile stampare il report.");
		            evnt.needPostBack = false;
		        }

		        if ((OnitDataPickGet('odpDataNascitaDa') == "" && OnitDataPickGet('odpDataNascitaA') != "") ||
                    (OnitDataPickGet('odpDataNascitaDa') != "" && OnitDataPickGet('odpDataNascitaA') == "")) {

		            alert("Se si vuole specificare un intervallo di nascita, le date di inizio e di fine devono essere compilate entrambe. Impossibile stampare il report.");
		            evnt.needPostBack = false;
		        }
		    }		
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" Titolo="Statistiche - <b>Avviso Mantoux</b>" TitleCssClass="Title3" width="100%" height="100%">

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
						    <igtbar:TBarButton Key="btnStampaAvviso" DisabledImage="~/Images/stampa.gif" Text="Avviso mantoux non lette"
							    Image="~/Images/Stampa.gif" ToolTip="Stampa l'avviso mantoux">
                                <DefaultStyle Width="180px" CssClass="infratoolbar_button_default"></DefaultStyle>
						    </igtbar:TBarButton>
						    <igtbar:TBarButton Key="btnStampaElenco" DisabledImage="~/Images/stampa.gif" Text="Elenco mantoux non lette"
							    Image="~/Images/Stampa.gif" ToolTip="Stampa l'elenco mantoux">
                                <DefaultStyle Width="180px" CssClass="infratoolbar_button_default"></DefaultStyle>
						    </igtbar:TBarButton>
						    <igtbar:TBarButton Key="btnStampaElencoCompleto" DisabledImage="~/Images/stampa.gif" Text="Elenco completo"
							    Image="~/Images/Stampa.gif" ToolTip="Stampa l'elenco completo delle mantoux">
                                <DefaultStyle Width="130px" CssClass="infratoolbar_button_default"></DefaultStyle>
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
                            <fieldset title="Eseguita" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Eseguita</legend>
                                <asp:RadioButtonList ID="rblEseguita" runat="server" CssClass="TextBox_Stringa" RepeatDirection="Horizontal" RepeatLayout="Flow" Width="100%">
                                    <asp:ListItem Text="Si" Value="S" title="Solo le mantoux eseguite"></asp:ListItem>
                                    <asp:ListItem Text="No" Value="N" title="Solo le mantoux non eseguite"></asp:ListItem>
                                    <asp:ListItem Text="Entrambi" Value="" Selected="True" title="Sia le mantoux eseguite che le non eseguite"></asp:ListItem>
                                </asp:RadioButtonList>
                            </fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
							<fieldset title="Data Mantoux" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Data Mantoux</legend>
								<table style="width:100%">
									<tr>
										<td class="label_right">
											<asp:Label id="lblDataEffettuazioneDa" runat="server" CssClass="label">Da</asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataMantouxDa" runat="server" Height="20px" Width="136px" CssClass="textbox_data_obbligatorio" DateBox="True"></on_val:onitdatepick>
										<td class="label_right">
											<asp:Label id="lblDataEffettuazioneA" runat="server" CssClass="label">A</asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataMantouxA" runat="server" Height="20px" Width="136px" CssClass="textbox_data_obbligatorio" DateBox="True"></on_val:onitdatepick></td>
									</tr>
								</table>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset title="" class="fldroot vac-fieldset-height-45" runat="server" id="fldDataInvio">
                                <legend class="label"><asp:Label ID="lblDataInvio" runat="server" Text="<%$ Resources: Onit.OnAssistnet.OnVac.Web, DatiPaziente.MantouxDataInvio%>"></asp:Label></legend>
								<table style="width:100%">
									<tr>
										<td class="label_right">
											<asp:Label id="lblDataInvioDa" runat="server" CssClass="label">Da</asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataInvioDa" runat="server" Height="20px" Width="136px" CssClass="textbox_data" DateBox="True"></on_val:onitdatepick>
										<td class="label_right">
											<asp:Label id="lblDataInvioA" runat="server" CssClass="label">A</asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataInvioA" runat="server" Height="20px" Width="136px" CssClass="textbox_data" DateBox="True"></on_val:onitdatepick></td>
									</tr>
								</table>
                            </fieldset>                            
                        </div>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset title="Data Nascita" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Data Nascita</legend>
								<table style="width: 100%">
									<tr>
										<td class="label_right">
											<asp:Label id="lblDataNascitaDa" runat="server" CssClass="label">Da</asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataNascitaDa" runat="server" Height="20px" Width="136px" CssClass="textbox_data" DateBox="True"></on_val:onitdatepick>
										<td class="label_right">
											<asp:Label id="lblDataNascitaA" runat="server" CssClass="label">A</asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataNascitaA" runat="server" Height="20px" Width="136px" CssClass="textbox_data" DateBox="True"></on_val:onitdatepick></td>
									</tr>
								</table>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset title="Positiva" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Positiva</legend>
                                <asp:RadioButtonList ID="rblPositiva" runat="server" CssClass="TextBox_Stringa" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                    <asp:ListItem Text="Si" Value="S" title="Solo le mantoux positive"></asp:ListItem>
                                    <asp:ListItem Text="No" Value="N" title="Solo le mantoux negative"></asp:ListItem>
                                    <asp:ListItem Text="Entrambi" Value="" Selected="True" title="Sia le mantoux positive che le negative"></asp:ListItem>
                                </asp:RadioButtonList>
                            </fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <fieldset id="fldStatoAnag" title="Stato anagrafico" class="fldroot" >
                            <legend class="label">Stato anagrafico</legend>
							<onit:CheckBoxList id="chklStatoAnagrafico" runat="server" CssClass="label_left" RepeatDirection="Horizontal" 
								TextAlign="Right" RepeatColumns="4"></onit:CheckBoxList>
						</fieldset>
                    </div>
                </dyp:DynamicPanel>
                
			</on_lay3:onitlayout3>
		</form>
	</body>
</html>
