<%@ Page Language="vb" AutoEventWireup="false" Codebehind="StatVaccinazioniRifiutate.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.StatVaccinazioniRifiutate"%>

<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>Statistica Vaccinazioni Rifiutate</title>
		
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
		                if (!checkCampiObbligatori()) {
		                    evnt.needPostBack = false;
		                }
		                break;
		        }
		    }
		
		    //lancio della stampa tramite tasto invio (modifica 29/07/2004)
		    function Stampa(evt) {
		        if (evt.keyCode == 13) {
		            if (checkCampiObbligatori())
		                __doPostBack("Stampa", "");
		        }
		    }
		
		    function checkCampiObbligatori() {
		        if (OnitDataPickGet('odpDataNascitaIniz') == "" || OnitDataPickGet('odpDataNascitaFin') == "") {
		            alert("Non tutti i campi obbligatori sono impostati. Impossibile stampare il report.");
		            return false;
		        }
		        return true;
		    }	
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server" onkeyup="Stampa(event)">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" Titolo="Statistiche - <b>Vaccinazioni Rifiutate</b>" TitleCssClass="Title3" width="100%" height="100%">
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
							<igtbar:TBarButton Key="btnStampa" ToolTip="Stampa i pazienti con vaccinazioni escluse." Text="Stampa"
								DisabledImage="~/Images/stampa.gif" Image="~/Images/Stampa.gif"></igtbar:TBarButton>
						</Items>
					</igtbar:UltraWebToolbar>
                </div>
				<div class="sezione" id="Panel23" runat="server">
					<asp:Label id="LayoutTitolo_sezioneRicerca" runat="server">Filtri di stampa</asp:Label>
                </div>
                
                <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
					<table id="TblFld" style="WIDTH: 100%" cellspacing="1" cellpadding="1" border="0">
						<tr>
							<td style="PADDING-LEFT: 30px; PADDING-TOP: 30px">
								<fieldset id="fldCom" title="Comune" class="fldroot" >
                                    <legend class="label">Comune di Residenza</legend>
									<table id="tblCom" cellspacing="0" cellpadding="0" width="100%" border="0">
										<tr style="PADDING-RIGHT: 10px; PADDING-LEFT: 10px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px">
											<td class="label_left" align="left">
												<on_ofm:onitmodallist id="fmComuneRes" runat="server" Width="70%" RaiseChangeEvent="True" SetUpperCase="True"
													UseCode="True" Tabella="T_ANA_COMUNI" CampoDescrizione="COM_DESCRIZIONE" CampoCodice="COM_CODICE"
													Label="Comune" CodiceWidth="28%" LabelWidth="-8px" PosizionamentoFacile="False"></on_ofm:onitmodallist></td>
										</tr>
									</table>
								</fieldset>
							</td>
							<td style="PADDING-LEFT: 30px; PADDING-TOP: 30px">
								<fieldset id="fldCir" title="Circoscrizione" class="fldroot" >
                                    <legend class="label">Circoscrizione</legend>
									<table id="tblCir" cellspacing="0" cellpadding="0" width="100%" border="0">
										<tr style="PADDING-RIGHT: 10px; PADDING-LEFT: 10px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px">
											<td class="label_left" align="left">
												<on_ofm:onitmodallist id="fmCircoscrizione" runat="server" Width="70%" RaiseChangeEvent="True" SetUpperCase="True"
													UseCode="True" Tabella="T_ANA_CIRCOSCRIZIONI" CampoDescrizione="CIR_DESCRIZIONE" CampoCodice="CIR_CODICE"
													Label="Circoscrizione" CodiceWidth="28%" LabelWidth="-8px" PosizionamentoFacile="False"></on_ofm:onitmodallist></td>
										</tr>
									</table>
								</fieldset>
							</td>
						</tr>
						<tr>
							<td style="PADDING-LEFT: 30px; WIDTH: 50%; PADDING-TOP: 15px">
								<fieldset id="fldCnv" title="Consultorio" class="fldroot" >
                                    <legend class="label">Centro Vaccinale</legend>
									<table id="tblCnv" cellspacing="0" cellpadding="0" width="100%" border="0">
										<tr style="PADDING-RIGHT: 10px; PADDING-LEFT: 10px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px">
											<td class="label_left" align="left">
												<on_ofm:onitmodallist id="fmConsultorio" runat="server" Width="80%" RaiseChangeEvent="True" SetUpperCase="True"
													UseCode="True" Tabella="T_ANA_CONSULTORI" CampoDescrizione="CNS_DESCRIZIONE" CampoCodice="CNS_CODICE"
													Label="Consultorio" CodiceWidth="20%" LabelWidth="-8px" PosizionamentoFacile="False"></on_ofm:onitmodallist></td>
										</tr>
									</table>
								</fieldset>
							</td>
							<td style="PADDING-LEFT: 30px; PADDING-TOP: 15px">
								<fieldset id="fldDistretto" title="Distretto" class="fldroot" >
                                    <legend class="label">Distretto</legend>
									<table id="tblDistretto" cellspacing="0" cellpadding="0" width="100%" border="0">
										<tr style="PADDING-RIGHT: 10px; PADDING-LEFT: 10px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px">
											<td class="label_left" align="left">
												<on_ofm:onitmodallist id="fmDistretto" runat="server" Width="80%" RaiseChangeEvent="True" SetUpperCase="True"
													UseCode="True" Tabella="T_ANA_DISTRETTI" CampoDescrizione="DIS_DESCRIZIONE" CampoCodice="DIS_CODICE"
													Label="Distretto" CodiceWidth="20%" LabelWidth="-8px" PosizionamentoFacile="False"></on_ofm:onitmodallist></td>
										</tr>
									</table>
								</fieldset>
							</td>
						</tr>
						<tr>
							<td style="PADDING-LEFT: 30px; PADDING-TOP: 15px">
								<fieldset id="fldDataNasc" title="Data nascita" class="fldroot" style="WIDTH: 100%; HEIGHT: 80px">
                                    <legend class="label">Data di nascita</legend>
									<table id="tblDataNasc" cellspacing="0" cellpadding="0" width="100%" border="0">
										<tr style="PADDING-RIGHT: 10px; PADDING-LEFT: 10px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px">
											<td class="label_right">
												<asp:Label id="lblDataEffettuazioneIniz" runat="server" CssClass="label">   Da :   </asp:Label></td>
											<td>
												<on_val:onitdatepick id="odpDataNascitaIniz" runat="server" CssClass="textbox_data_obbligatorio" DateBox="True"></on_val:onitdatepick></td>
											<td>
												<asp:Label id="lblDataEffettuazioneFin" runat="server" Width="46px" CssClass="label">A :   </asp:Label></td>
											<td>
												<on_val:onitdatepick id="odpDataNascitaFin" runat="server" CssClass="textbox_data_obbligatorio" DateBox="True"></on_val:onitdatepick></td>
										</tr>
									</table>
								</fieldset>
							</td>
							<td style="PADDING-LEFT: 30px; PADDING-TOP: 15px">
								<fieldset id="fldVac" title="Vaccinazioni" class="fldroot" style="WIDTH: 100%; HEIGHT: 80px">
                                    <legend class="label">Vaccinazioni</legend>
									<table id="tblVac" cellspacing="0" cellpadding="0" width="100%" border="0">
										<tr style="PADDING-RIGHT: 10px; PADDING-LEFT: 10px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px">
											<td>
												<asp:RadioButtonList id="radVaccinazioni" runat="server" Width="50%" CssClass="textbox_stringa" RepeatDirection="Horizontal">
													<asp:ListItem Value="A">Obbligatorie</asp:ListItem>
													<asp:ListItem Value="T" Selected="True">Tutte</asp:ListItem>
												</asp:RadioButtonList></td>
										</tr>
									</table>
								</fieldset>
							</td>
						</tr>
						<tr>
							<td style="PADDING-LEFT: 30px; PADDING-TOP: 15px">
								<fieldset id="fldCiclo" title="Vaccinazioni" class="fldroot" style="WIDTH: 100%; HEIGHT: 80px">
                                    <legend class="label">Elenco vaccinazioni</legend>
									<table id="tblCiclo" cellSpacing="0" cellPadding="0" width="100%" border="0">
										<tr style="PADDING-RIGHT: 10px; PADDING-LEFT: 10px; PADDING-BOTTOM: 10px; PADDING-TOP: 10px">
											<td>
												<asp:DropDownList id="cmbVaccinazioni" runat="server" Width="100%" DataTextField="VAC_DESCRIZIONE"
													DataValueField="VAC_CODICE"></asp:DropDownList></td>
										</tr>
									</table>
								</fieldset>
							</td>
							<td style="PADDING-LEFT: 30px; PADDING-TOP: 15px">&nbsp;</td>
						</tr>
					</table>
                </dyp:DynamicPanel>
                
                <dyp:DynamicPanel ID="dypMsg" runat="server" Width="100%" Height="18px">
                    <asp:Label id="Label1" runat="server" Width="100%" CssClass="label_errormsg"></asp:Label>
                </dyp:DynamicPanel>

			</on_lay3:onitlayout3>
			
		</form>
	</body>
</html>
