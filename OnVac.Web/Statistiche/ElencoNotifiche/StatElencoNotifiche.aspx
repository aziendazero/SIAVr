<%@ Page Language="vb" AutoEventWireup="false" Codebehind="StatElencoNotifiche.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.StatElencoNotifiche"%>

<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="uc1" TagName="SelezioneConsultori" Src="../../Common/Controls/SelezioneConsultori.ascx" %>

<%@ Import namespace="Onit.OnAssistnet.OnVac" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>Statistica Elenco Notifiche</title>

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
		                LanciaStampa(evnt);
		        }
		    }
		
		    //deve chiedere se impostare lo stato oppure stampare una copia [modifica 08/03/2006]
		    function LanciaStampa(evt) {
		        var messaggioConferma;
		        messaggioConferma = "Attenzione: si desidera impostare il valore <% = RecuperaStato(Enumerators.StatoInadempienza.CasoConcluso) %> nello stato delle inadempienze dei pazienti selezionati"
		        messaggioConferma += "\r(premendo 'Ok' la Comunicazione al Sindaco sarà stampabile solo da 'Inadempienze' di ogni paziente)?"
		        if (confirm(messaggioConferma)) {
		            evt.needPostBack = false;
		            __doPostBack('Stampa', 'ImpostaStatoS');
		        }
		        else {
		            evt.needPostBack = false;
		            __doPostBack('Stampa', 'ImpostaStatoN');
		        }
		    }
		
		    //lancio della stampa tramite pulsante invio (modifica 29/07/2004)
		    function Stampa(evt) {
		        if (evt.keyCode == 13) {
		            LanciaStampa(evt);
		        }
		    }		
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server" onkeyup="Stampa(event)">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" TitleCssClass="Title3" Titolo="Statistiche - <b>Elenco notifiche</b>" height="100%" width="100%">
                <div id="PanelTitolo" class="title" runat="server">
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
				<div id="Panel23" class="sezione" runat="server">
					<asp:Label id="LayoutTitolo_sezioneRicerca" runat="server">Filtri di stampa</asp:Label>
                </div>

                <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
					<table style="WIDTH: 100%" id="TblFld" border="0" cellspacing="1" cellpadding="1">
						<tr>
							<td style="PADDING-LEFT: 30px; PADDING-TOP: 30px">
								<fieldset class="fldroot" style="WIDTH: 50%;" id="fldCnv" title="Consultorio">
                                    <legend style="BORDER-BOTTOM: #26a 1px solid; BORDER-LEFT: #26a 1px solid; PADDING-BOTTOM: 1px; BACKGROUND-COLOR: #485d96; PADDING-LEFT: 10px; PADDING-RIGHT: 10px; COLOR: #ffffff; BORDER-TOP: #26a 1px solid; FONT-WEIGHT: bold; BORDER-RIGHT: #26a 1px solid; PADDING-TOP: 1px" class="label">Centro Vaccinale</legend>
									<table id="tblCnv" border="0" cellspacing="0" cellpadding="0" width="100%">
										<tr style="PADDING-BOTTOM: 10px; PADDING-LEFT: 10px; PADDING-RIGHT: 10px; PADDING-TOP: 10px">
											<td class="label_left" align="left">
												<uc1:SelezioneConsultori id="ucSelezioneConsultori" runat="server" Width="100%"  MaxCnsInListaSelezionati="3"></uc1:SelezioneConsultori>
											</td>

										</tr>
									</table>
								</fieldset>
							</td>
						</tr>
						<tr>
							<td style="PADDING-LEFT: 30px; PADDING-TOP: 30px">
								<fieldset class="fldroot" style="WIDTH: 80%; " id="fldStatoAnagrafico" title="Stato anagrafico">
                                    <legend style="BORDER-BOTTOM: #26a 1px solid; BORDER-LEFT: #26a 1px solid; PADDING-BOTTOM: 1px; BACKGROUND-COLOR: #485d96; PADDING-LEFT: 10px; PADDING-RIGHT: 10px; COLOR: #ffffff; BORDER-TOP: #26a 1px solid; FONT-WEIGHT: bold; BORDER-RIGHT: #26a 1px solid; PADDING-TOP: 1px" class="label">Stato anagrafico</legend>
									<table id="tblStatoAn" border="0" cellspacing="0" cellpadding="0" width="100%">
										<tr style="PADDING-BOTTOM: 10px; PADDING-LEFT: 10px; PADDING-RIGHT: 10px; PADDING-TOP: 10px">
											<td class="label_left" align="left">
												<asp:CheckBoxList id="chklStatoAnagrafico" runat="server" Height="27px" CssClass="LABEL_left" RepeatDirection="Horizontal"
													RepeatColumns="4" TextAlign="Right"></asp:CheckBoxList></td>
										</tr>
									</table>
								</fieldset>
							</td>
						</tr>
						<tr>
							<td style="PADDING-LEFT: 30px; PADDING-TOP: 30px">
								<fieldset class="fldroot" style="WIDTH: 50%; DISPLAY: none;" id="fldStatusVac" title="Status vaccinale">
                                    <legend style="BORDER-BOTTOM: #26a 1px solid; BORDER-LEFT: #26a 1px solid; PADDING-BOTTOM: 1px; BACKGROUND-COLOR: #485d96; PADDING-LEFT: 10px; PADDING-RIGHT: 10px; COLOR: #ffffff; BORDER-TOP: #26a 1px solid; FONT-WEIGHT: bold; BORDER-RIGHT: #26a 1px solid; PADDING-TOP: 1px" class="label">Status vaccinale</legend>
									<table id="tblStatusVac" border="0" cellspacing="0" cellpadding="0" width="100%">
										<tr style="PADDING-BOTTOM: 10px; PADDING-LEFT: 10px; PADDING-RIGHT: 10px; PADDING-TOP: 10px">
											<td class="label_left" align="left">
												<asp:DropDownList id="ddlStatusVaccinale" runat="server" width="100%">
													<asp:ListItem Selected="True"></asp:ListItem>
													<asp:ListItem Value="3">IN CORSO</asp:ListItem>
													<asp:ListItem Value="4">TERMINATO</asp:ListItem>
													<asp:ListItem Value="9">INADEMPIENTE TOTALE</asp:ListItem>
												</asp:DropDownList></td>
										</tr>
									</table>
								</fieldset>
							</td>
						</tr>
					</table>
                </dyp:DynamicPanel>

                <dyp:DynamicPanel ID="dypMsg" runat="server" Width="100%" Height="18px">
					<asp:Label id="Label1" runat="server" Width="100%" CssClass="label_errorMsg"></asp:Label>
                </dyp:DynamicPanel>

			</on_lay3:onitlayout3>
		</form>
	</body>
</html>
