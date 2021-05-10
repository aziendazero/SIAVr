<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ElencoBilanciMalattia.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.ElencoBilanciMalattia" %>

<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<%@ Register TagPrefix="uc1" TagName="RicercaBilancio" Src="../../Common/Controls/RicercaBilancio.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>ElencoBilanciMalattia</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <style type="text/css">
        .vacRowPad {
            padding: 10px;
        }
    </style>

    <script type="text/javascript">
		var oldSel = 'EA';
		
		// inizializzazione della toolbar
		function InizializzaToolBar(t) {
		    t.PostBackButton = false;
		}
		
		//controllo valore dei datepick
		function ToolBarClick(ToolBar,button,evnt) {
		    evnt.needPostBack = true;
		}
    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
    <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Height="100%" Width="100%" Titolo="Elenco Bilanci Malattia">
	    <div id="PanelTitolo" class="title" runat="server">
		    <asp:Label id="LayoutTitolo" runat="server">Elenco Bilanci Malattia</asp:Label>
        </div>
        <div>
			<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="Toolbar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
	            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
				<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
				<Items>
					<igtbar:TBarButton Key="btnBilancio" HoverImage="" ToolTip="seleziona bilancio" SelectedImage="" Text="Scegli Bilancio"
						DisabledImage="../../images/bilanci_dis.gif" Image="../../images/bilanci.gif">
						<DefaultStyle Width="130px" CssClass="infratoolbar_button_default"></DefaultStyle>
					</igtbar:TBarButton>
					<igtbar:TBarButton Key="btnStampa" HoverImage="" SelectedImage="" Text="Stampa" DisabledImage="~/Images/stampa.gif"
						Image="~/Images/Stampa.gif">
					</igtbar:TBarButton>
					<igtbar:TBCustom Width="">
						<System.Web.UI.HtmlControls.HtmlGenericControl Width="20px" TextAlign="Right"></System.Web.UI.HtmlControls.HtmlGenericControl>
					</igtbar:TBCustom>
				</Items>
			</igtbar:UltraWebToolbar>
        </div>
		<div id="Panel23" class="sezione" runat="server">
			<asp:Label id="LayoutTitolo_sezioneRicerca" runat="server">Malattia Selezionata</asp:Label>
        </div>
        <div>
			<table id="TblFld" border="0" cellspacing="1" cellpadding="20" width="100%">
				<tr>
					<td class="label" width="10%">Malattia</td>
					<td width="60%" colspan="2">
						<on_ofm:onitmodallist id="txtMalattia" runat="server" Height="24px" Width="70%" Obbligatorio="False" Enabled="False" Label="Malattia" PosizionamentoFacile="False" LabelWidth="-8px" SetUpperCase="False" UseCode="False" Tabella="T_ANA_MALATTIE" CampoDescrizione="MAL_DESCRIZIONE" CampoCodice="MAL_CODICE" CodiceWidth="30%"></on_ofm:onitmodallist>
                    </td>
					<td class="label" width="10%">&nbsp;</td>
					<td class="label" width="10%">Numero</td>
					<td width="10%">
						<on_val:OnitJsValidator id="txtNumero" runat="server" Width="100%" PreParams-minValue="0" PreParams-maxValue="null"
							PreParams-numDecDigit="0" validationType="Validate_integer" autoFormat="False" actionUndo="False" actionSelect="True"
							actionMessage="True" actionFocus="False" actionDelete="False" actionCorrect="False" ReadOnly="True"
							CssClass="TextBox_Numerico_Disabilitato"></on_val:OnitJsValidator></td>
				</tr>
			</table>
        </div>
		<div class="Sezione">Filtri di Stampa</div>
        <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">

			<table id="TblFld2" border="0" cellspacing="1" cellpadding="10" width="100%">
				<tr>
					<td>
						<fieldset id="fldCom" title="Comune di Residenza" class="fldroot"  style="width: 100%; margin: 0 px;">
							<legend class="label">Comune di Residenza</legend>
							<table id="tblCom" border="0" cellspacing="0" cellpadding="0" width="100%">
								<tr class="vacRowPad">
									<td class="label_left" align="left">
										<on_ofm:onitmodallist id="fmComuneRes" runat="server" Width="70%" Label="Comune" PosizionamentoFacile="False"
											LabelWidth="-8px" SetUpperCase="True" UseCode="True" Tabella="T_ANA_COMUNI" CampoDescrizione="COM_DESCRIZIONE"
											CampoCodice="COM_CODICE" CodiceWidth="28%" RaiseChangeEvent="True"></on_ofm:onitmodallist></td>
								</tr>
							</table>
						</fieldset>
					</td>
					<td>
						<fieldset id="fldCnv" title="Consultorio Vaccinale" class="fldroot" style="width: 100%; margin: 0 px;">
							<legend class="label">Centro Vaccinale</legend>
							<table id="tblCnv" border="0" cellspacing="0" cellpadding="0" width="100%">
								<tr class="vacRowPad">
									<td class="label_left" align="left">
										<on_ofm:onitmodallist id="fmConsultorio" runat="server" Width="70%" Label="Consultorio" PosizionamentoFacile="False"
											LabelWidth="-8px" SetUpperCase="True" UseCode="True" Tabella="T_ANA_CONSULTORI" CampoDescrizione="CNS_DESCRIZIONE"
											CampoCodice="CNS_CODICE" CodiceWidth="28%"></on_ofm:onitmodallist></td>
								</tr>
							</table>
						</fieldset>
					</td>
				</tr>
				<tr>
					<td>
						<fieldset id="fldData" title="Data effettuazione" class="fldroot" style="width: 100%; margin: 0 px;">
							<LEGEND class="label">Date effettuazione</legend>
							<TABLE id="tblDate" border="0" cellspacing="0" cellpadding="0" width="100%">
								<TR class="vacRowPad">
									<td>
										<asp:Label id="lblDataEffettuazioneIniz" runat="server" CssClass="label">Da :   </asp:Label></td>
									<td>
										<on_val:onitdatepick id="odpDataEffettuazioneIniz" runat="server" Height="20px" Width="136px" CssClass="textbox_data"
											DateBox="True"></on_val:onitdatepick></td>
									<td>
										<asp:Label id="lblDataEffettuazioneFin" runat="server" Width="46px" CssClass="label">A :   </asp:Label></td>
									<td>
										<on_val:onitdatepick id="odpDataEffettuazioneFin" runat="server" Height="20px" Width="136px" CssClass="textbox_data"
											DateBox="True"></on_val:onitdatepick></td>
								</tr>
							</table>
						</fieldset>
					</td>
					<td>
						<fieldset id="fldDataNascita" title="Data nascita" class="fldroot"  style="width: 100%; margin: 0 px;">
							<legend class="label">Data di nascita</legend>
							<table id="tblDataNascita" border="0" cellspacing="0" cellpadding="0" width="100%">
								<tr class="vacRowPad">
									<td>
										<asp:Label id="lblDataNascitaIniz" runat="server" CssClass="label">Da :   </asp:Label></td>
									<td>
										<on_val:onitdatepick id="odpDataNascitaIniz" runat="server" Height="20px" Width="136px" CssClass="textbox_data"
											DateBox="True"></on_val:onitdatepick></td>
									<td>
										<asp:Label id="lblDataNascitaFin" runat="server" Width="46px" CssClass="label">A :   </asp:Label></td>
									<td>
										<on_val:onitdatepick id="odpDataNascitaFin" runat="server" Height="20px" Width="136px" CssClass="textbox_data"
											DateBox="True"></on_val:onitdatepick></td>
								</tr>
							</table>
						</fieldset>
					</td>
				</tr>
			</table>
        </dyp:DynamicPanel>
        
        <dyp:DynamicPanel ID="dypMsg" runat="server" Width="100%" Height="18px">
            <asp:Label id="Label1" runat="server" Width="100%" CssClass="label_errormsg"></asp:Label>
        </dyp:DynamicPanel>

    </on_lay3:OnitLayout3>

    <on_ofm:OnitFinestraModale ID="modRicBil" Title="Ricerca bilancio" runat="server" Width="617px" BackColor="LightGray">
        <uc1:RicercaBilancio ID="uscRicBil" runat="server" IncludiNessunaMalattia="true"></uc1:RicercaBilancio>
    </on_ofm:OnitFinestraModale>

    <script language="javascript" type="text/javascript">
		
			if (<%= (Not IsPostBack).ToString.ToLower%>)
				OnitDataPickFocus('odpDataIniz',1,false);
						
    </script>

    </form>
</body>
</html>
