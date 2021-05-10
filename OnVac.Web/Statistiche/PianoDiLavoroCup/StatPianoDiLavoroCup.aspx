<%@ Page Language="vb" AutoEventWireup="false" Codebehind="StatPianoDiLavoroCup.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.StatPianoDiLavoroCup"%>

<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="uc1" TagName="SelezioneConsultori" Src="../../Common/Controls/SelezioneConsultori.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>Stat Piano Di Lavoro Cup</title>

        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

		<script type="text/javascript">
	
		function InizializzaToolBar(t)
		{
			t.PostBackButton = false;
		}

		function ToolBarClick(ToolBar, button, evnt)
        {
			evnt.needPostBack = true;
		
			switch(button.Key)
			{
				case 'btnStampa':
					if (OnitDataPickGet('odpDataEffettuazioneIniz') == "" || OnitDataPickGet('odpDataEffettuazioneFin') == "")
					{
						alert("Non tutti i campi obbligatori sono impostati. Impossibile stampare il report.");
						evnt.needPostBack = false;
					}
			}
		}
	
		function Stampa(evt)
		{
			if (evt.keyCode==13) 
			{
				if (OnitDataPickGet('odpDataEffettuazioneIniz') == "" || OnitDataPickGet('odpDataEffettuazioneFin') == "")
				{
					alert("Non tutti i campi obbligatori sono impostati. Impossibile stampare il report.");
					evnt.needPostBack = false;
				}
				else
				{
					__doPostBack("Stampa", "");
				}
			}
		}
		
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server" onkeyup="Stampa(event)">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" height="100%" width="100%" Titolo="Statistiche - <b>Piano di lavoro CUP</b>" TitleCssClass="Title3">
				<div class="title" id="PanelTitolo" runat="server">
					<asp:Label id="LayoutTitolo" runat="server"></asp:Label>
                </div>
				<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="Toolbar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
	                <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
					<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
					<Items>
						<igtbar:TBarButton Key="btnStampa" DisabledImage="~/Images/stampa.gif" Text="Stampa" Image="~/Images/Stampa.gif"></igtbar:TBarButton>
					</Items>
				</igtbar:UltraWebToolbar>
				<div class="sezione" id="Panel23" runat="server">
					<asp:Label id="LayoutTitolo_sezioneRicerca" runat="server">Filtri di stampa</asp:Label>
                </div>
				<table id="TblFld" style="WIDTH: 100%" cellspacing="1" cellpadding="1" border="0">
					<tr>
						<td style="PADDING-LEFT: 30px; PADDING-TOP: 30px">
							<fieldset id="fldCnv" title="Centro Vaccinale"  class="fldroot" style="width: 50%;">
                                <legend class="label">Centro Vaccinale</legend>
								<table id="tblCnv" cellspacing="0" cellpadding="0" width="100%" border="0">
									<tr style="PADDING-RIGHT: 10px; PADDING-LEFT: 10px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px">
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
							<fieldset id="fldDataEff" title="Data di effettuazione" class="fldroot" style="width: 50%;">
                                <legend class="label">Intervallo Date</legend>
								<table id="tblDataEff" cellspacing="0" cellpadding="0" width="100%" border="0">
									<tr style="PADDING-RIGHT: 10px; PADDING-LEFT: 10px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px">
										<td>
											<asp:Label id="lblDataEffettuazioneIniz" runat="server" CssClass="label">Da:   </asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataEffettuazioneIniz" tabIndex="2" runat="server" Height="20px" CssClass="textbox_data_obbligatorio"
												Width="136px" DateBox="True"></on_val:onitdatepick></td>
										<td>
											<asp:Label id="lblDataEffettuazioneFin" runat="server" CssClass="label" Width="20px">A:   </asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataEffettuazioneFin" tabIndex="3" runat="server" Height="20px" CssClass="textbox_data_obbligatorio"
												Width="136px" DateBox="True"></on_val:onitdatepick></td>
									</tr>
								</table>
							</fieldset>
						</td>
					</tr>
					<tr>
						<td style="PADDING-LEFT: 30px; PADDING-TOP: 30px">
							<fieldset id="fldDataNasc" title="DataNascita"  class="fldroot" style="width: 50%;">
                                <legend class="label">Data di Nascita</legend>
								<table id="tblDataNascita" cellspacing="0" cellpadding="0" width="100%" border="0">
									<tr style="PADDING-RIGHT: 10px; PADDING-LEFT: 10px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px">
										<td>
											<asp:Label id="lblDataNascitaDa" runat="server" CssClass="label">Da:   </asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataNascitaDa" tabIndex="5" runat="server" Height="20px" CssClass="textbox_data"
												Width="136px" DateBox="True"></on_val:onitdatepick></td>
										<td>
											<asp:Label id="lblDataNascitaA" runat="server" CssClass="label">A:   </asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataNascitaA" tabIndex="6" runat="server" Height="20px" CssClass="textbox_data"
												Width="136px" DateBox="True"></on_val:onitdatepick></td>
									</tr>
								</table>
							</fieldset>
						</td>
					</tr>
					<tr>
						<td style="PADDING-LEFT: 30px; PADDING-TOP: 30px">
							<fieldset id="fldModAccesso" title="Modalità di Accesso"  class="fldroot" style="width: 50%;">
                                <legend class="label">Modalità di accesso</legend>
								<table id="tbModAccesso" cellspacing="0" cellpadding="0" width="100%" border="0">
									<tr style="PADDING-RIGHT: 10px; PADDING-LEFT: 10px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px">
										<td>
											<asp:CheckBoxList id="chklModAccesso" tabIndex="7" runat="server" CssClass="label" Width="400px" TextAlign="Right"
												RepeatDirection="Horizontal">
												<asp:ListItem Value="PC"> Prenotati CUP </asp:ListItem>
												<asp:ListItem Value="PS"> Pronto Soccorso </asp:ListItem>
												<asp:ListItem Value="AV"> Accesso Volontario </asp:ListItem>
												<asp:ListItem Value="AO"> Appuntamento Onvac </asp:ListItem>
											</asp:CheckBoxList>
                                        </td>
									</tr>
								</table>
							</fieldset>
						</td>
					</tr>
				</table>

				<table width="100%" border="0">
					<tr>
						<td valign="bottom" height="100">
							<asp:Label id="Label1" runat="server" CssClass="label_errormsg" Width="100%"></asp:Label></td>
					</tr>
				</table>

			</on_lay3:onitlayout3>
		</form>
	</body>
</html>