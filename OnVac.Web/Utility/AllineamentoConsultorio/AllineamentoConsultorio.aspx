<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="AllineamentoConsultorio.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.AllineamentoConsultorio" %>

<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<%@ Register TagPrefix="uc2" TagName="StatiAnagrafici" Src="../../Common/Controls/StatiAnagrafici.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>Allineamento Centro Vaccinale</title>
    
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
    <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("onvac.common.js")%>'></script>
  
    <style type="text/css">
        .testata {
            border-right: navy 0 solid;
            padding-right: 10px;
            border-top: navy 0 solid;
            padding-left: 10px;
            font-weight: bold;
            font-size: 12px;
            padding-bottom: 1px;
            border-left: navy 0 solid;
            color: navy;
            padding-top: 1px;
            border-bottom: navy 1px solid;
            font-family: Arial,Tahoma,Verdana;
            background-color: aliceblue;
            text-align: right;
        }
    </style>
</head>
<body>
    <form id="Form1" method="post" runat="server">
    <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Height="100%" Width="100%" TitleCssClass="Title3" Titolo="Allineamento Centro Vaccinale">
		<div class="title" id="PanelTitolo" runat="server">
			<asp:Label id="LayoutTitolo" runat="server">&nbsp;Utility di allineamento Centro Vaccinale </asp:Label>
        </div>
        <div>
			<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="140px" CssClass="infratoolbar">
                <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		        <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
				<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
				<Items>
					<igtbar:TBarButton Key="btnStart" Text="Esegui" DisabledImage="../../images/rotella_dis.gif" Image="../../images/rotella.gif">
					</igtbar:TBarButton>
				</Items>
			</igtbar:UltraWebToolbar>
        </div>
        <div>
			<table width="100%" bgcolor="whitesmoke" border="0">
				<tr height="2">
					<td></td>
				</tr>
				<tr align="center">
					<td>
						<fieldset class="vacFieldset" title="Filtri comuni" style="text-align:left">
							<legend class="label">Filtri comuni</legend>
							<table class="label_left" style="margin-top: 2px" cellspacing="0" cellpadding="2" width="100%" border="0">
								<colgroup>
									<col width="2%" />
									<col width="16%" />
									<col width="80%" />
									<col width="2%" />
								</colgroup>
								<tr height="3">
									<td colspan="4"></td>
								</tr>
								<tr>
									<td>&nbsp;</td>
									<td class="label_left">Centro Vaccinale:</td>
									<td>
										<onitcontrols:OnitModalList id="omlConsultorio" runat="server" Filtro="cns_data_apertura <= SYSDATE AND (cns_data_chiusura > SYSDATE OR cns_data_chiusura IS NULL) ORDER BY cns_descrizione"
											Width="70%" CampoDescrizione="CNS_DESCRIZIONE" CampoCodice="CNS_CODICE" Tabella="T_ANA_CONSULTORI" PosizionamentoFacile="False"
											LabelWidth="-1px" CodiceWidth="29%" Label="Titolo" UseCode="True" SetUpperCase="True" Obbligatorio="True"></onitcontrols:OnitModalList></td>
									<td>&nbsp;</td>
								</tr>
								<tr height="3">
									<td colspan="4"></td>
								</tr>
							</table>
						</fieldset>
					</td>
				</tr>
			</table>
        </div>
		<div class="vac-sezione">
			<asp:Label id="lblFunzionalita" runat="server">Funzionalita'</asp:Label>
        </div>

        <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
			<table height="100%" cellspacing="0" cellpadding="0" width="100%" border="0">
				<tr height="5%">
					<td></td>
				</tr>
				<tr align="center">
					<td>
						<div class="vacFieldset" title="Pulizia convocazioni ai deceduti">
							<table class="testata" height="5" cellspacing="2" cellpadding="2" width="100%" border="0" style="border-bottom:0px;">
								<colgroup>
									<col width="80%" />
									<col width="10%" />
									<col width="10%" />
								</colgroup>
								<tr>
									<td align="left">Pulizia convocazioni ai deceduti</td>
									<td>
										<asp:CheckBox id="chkElaboraDeceduti" Runat="server" Checked="true" Text=""></asp:CheckBox></td>
									<td class="label_left">Attiva</td>
								</tr>
							</table>
						</div>
					</td>
				</tr>
				<tr height="5%">
					<td></td>
				</tr>
				<tr align="center">
					<td>
						<div class="vacFieldset" title="Cambio centro vaccinale ai pazienti fuori età">
							<table class="testata" height="5" cellspacing="2" cellpadding="2" width="100%" border="0">
								<colgroup>
									<col width="80%" />
									<col width="10%" />
									<col width="10%" />
								</colgroup>
								<tr>
									<td align="left">Cambio centro vaccinale ai pazienti fuori età</td>
									<td>
										<asp:CheckBox id="chkElaboraCambioConsultorio" Runat="server" Checked="true"></asp:CheckBox></td>
									<td class="label_left">Attiva</td>
								</tr>
							</table>
							<table class="label_left" style="margin-top: 2px" cellspacing="0" cellpadding="2" width="100%" border="0">
								<colgroup>
									<col width="2%" />
									<col width="86%" />
									<col width="2%" />
								</colgroup>
								<tr>
									<td>&nbsp;</td>
									<td align="right">
										<uc2:StatiAnagrafici id="staStatiAnagrafici" runat="server"></uc2:StatiAnagrafici></td>
									<td>&nbsp;</td>
								</tr>
								<tr>
									<td>&nbsp;</td>
									<td align="right">
										<table width="100%" border="0">
											<tr>
												<td width="25%">
													<fieldset id="fldNascita" title="Data nascita" class="fldnode">
														<legend class="label">Data nascita</legend>
														<table id="Table_nascita" height="48" cellspacing="0" cellpadding="0" width="100%" border="0">
															<tr>
																<td class="label" width="20%">Da :</td>
																<td align="center">
																	<on_val:onitdatepick id="odpDaDataNascita" runat="server" Height="20px" Width="120px" CssClass="textbox_data"
																		DateBox="True"></on_val:onitdatepick></td>
															</tr>
															<tr>
																<td class="label">A :</td>
																<td align="center">
																	<on_val:onitdatepick id="odpADataNascita" runat="server" Height="20px" Width="120px" CssClass="textbox_data"
																		DateBox="True"></on_val:onitdatepick></td>
															</tr>
														</table>
													</fieldset>
												</td>
												<td width="1%"></td>
												<td width="69%">
													<fieldset id="fldOpzioni" title="Opzioni Ricerca" class="fldnode">
														<legend class="label">Opzioni</legend>
														<table id="Table_pzioni" height="48" cellspacing="0" cellpadding="0" width="100%" border="0">
															<colgroup>
																<col width="47%" />
																<col width="1%" />
																<col width="13%" />
																<col width="3%" />
																<col width="10%" />
																<col width="21%" />
																<col width="3%" />
															</colgroup>
															<tr>
																<td class="label">Aggiorna anche pazienti con appuntamento:</td>
																<td>&nbsp;</td>
																<td align="left">
																	<asp:DropDownList id="ddlAggiornaAncheCnvConApp" runat="server" Width="95%">
																		<asp:ListItem Value="N" Selected="True">NO</asp:ListItem>
																		<asp:ListItem Value="S">SI</asp:ListItem>
																	</asp:DropDownList></td>
																<td>&nbsp;</td>
																<td class="label">Sesso:</td>
																<td align="right">
																	<asp:DropDownList id="ddlSesso" Width="95%" Runat="server">
																		<asp:ListItem Selected="True"></asp:ListItem>
																		<asp:ListItem Value="M">Maschio</asp:ListItem>
																		<asp:ListItem Value="F">Femmina</asp:ListItem>
																	</asp:DropDownList></td>
																<td>&nbsp;</td>
															</tr>
															<tr>
																<td class="label">Criterio di associazione:</td>
																<td>&nbsp;</td>
																<td align="right" colSpan="4">
																	<asp:DropDownList id="ddlCriterioAssociazione" Width="100%" Runat="server">
																		<asp:ListItem Value="C|D|R">Circoscrizione, Domicilio, Residenza</asp:ListItem>
																		<asp:ListItem Value="C|R|D">Circoscrizione, Residenza, Domicilio</asp:ListItem>
																		<asp:ListItem Value="D|C|R">Domicilio, Circoscrizione, Residenza</asp:ListItem>
																		<asp:ListItem Value="D|R|C">Domicilio, Residenza, Circoscrizione</asp:ListItem>
																		<asp:ListItem Value="R|D|C">Residenza, Domicilio, Circoscrizione</asp:ListItem>
																		<asp:ListItem Value="R|C|D">Residenza, Circoscrizione, Domicilio</asp:ListItem>
																	</asp:DropDownList></td>
																<td>&nbsp;</td>
															</tr>
														</table>
													</fieldset>
												</td>
											</tr>
										</table>
									</td>
									<td>&nbsp;</td>
								</tr>
								<tr height="10">
									<td colSpan="3">&nbsp;</td>
								</tr>
							</table>
						</div>
					</td>
				</tr>
				<tr height="5%">
					<td></td>
				</tr>
				<tr align="center">
					<td>
						<div class="vacFieldset" title="Calcolo automatico convocazioni">
							<table class="testata" height="5" cellspacing="2" cellpadding="2" width="100%" border="0">
								<colgroup>
									<col width="80%" />
									<col width="10%" />
									<col width="10%" />
								</colgroup>
								<tr>
									<td class="label_left">Calcolo automatico convocazioni</td>
									<td>
										<asp:CheckBox id="chkElaboraConvocazioni" Runat="server" Checked="true" Text=""></asp:CheckBox></td>
									<td class="label_left">Attiva</td>
								</tr>
							</table>
							<table class="label_left" style="MARGIN-TOP: 2px" cellspacing="0" cellpadding="2" width="100%" border="0">
								<colgroup>
									<col width="2%" />
									<col align="right" width="86%" />
									<col width="2%" />
								</colgroup>
								<tr>
									<td>&nbsp;</td>
									<td>
										<uc2:StatiAnagrafici id="uscCnvStatiAnagrafici" runat="server"></uc2:StatiAnagrafici></td>
									<td>&nbsp;</td>
								</tr>
								<tr>
									<td>&nbsp;</td>
									<td>
										<table width="100%" border="0">
											<tr>
												<td width="25%">
													<fieldset title="Data nascita"class="fldnode">
														<legend class="label">Data nascita</legend>
														<table height="48" cellspacing="0" cellpadding="0" width="100%" border="0">
															<tr>
																<td class="label" width="20%">Da:&nbsp;</td>
																<td align="center">
																	<on_val:onitdatepick id="dpkCnvDaNascita" runat="server" Height="20px" Width="120px" CssClass="textbox_data"
																		DateBox="True"></on_val:onitdatepick></td>
															</tr>
															<tr>
																<td class="label">A:&nbsp;</td>
																<td align="center">
																	<on_val:onitdatepick id="dpkCnvANascita" runat="server" Height="20px" Width="120px" CssClass="textbox_data"
																		DateBox="True"></on_val:onitdatepick></td>
															</tr>
														</table>
													</fieldset>
												</td>
												<td width="1%"></td>
												<td width="69%">
													<fieldset title="Opzioni Ricerca" class="fldnode">
														<legend class="label">Opzioni</legend>
														<table height="48" cellspacing="0" cellpadding="0" width="100%" border="0">
															<colgroup>
																<col width="15%" />
																<col width="48%" />
																<col width="3%" />
																<col width="10%" />
																<col width="21%" />
																<col width="3%" />
															</colgroup>
															<tr>
																<td class="label">Malattia:&nbsp;</td>
																<td>
																	<onitcontrols:OnitModalList id="omlCnvMalattia" runat="server" Width="70%" CampoDescrizione="MAL_DESCRIZIONE Descrizione"
																		CampoCodice="MAL_CODICE Codice" Tabella="T_ANA_MALATTIE" PosizionamentoFacile="False" LabelWidth="-1px" CodiceWidth="29%"
																		Label="Titolo" UseCode="True" SetUpperCase="True" Filtro=" MAL_OBSOLETO='N' ORDER BY MAL_CODICE"></onitcontrols:OnitModalList></td>
																<td>&nbsp;</td>
																<td class="label">Sesso:</td>
																<td align="right">
																	<asp:DropDownList id="ddlCnvSesso" Width="95%" Runat="server">
																		<asp:ListItem Value="" Selected="True"></asp:ListItem>
																		<asp:ListItem Value="M">Maschio</asp:ListItem>
																		<asp:ListItem Value="F">Femmina</asp:ListItem>
																	</asp:DropDownList></td>
																<td>&nbsp;</td>
															</tr>
															<tr>
																<td class="label">Categoria rischio:&nbsp;</td>
																<td align="left">
																	<onitcontrols:OnitModalList id="omlCnvCategorieRischio" runat="server" Width="70%" CampoDescrizione="RSC_DESCRIZIONE"
																		CampoCodice="RSC_CODICE" Tabella="T_ANA_RISCHIO" PosizionamentoFacile="False" LabelWidth="-1px" CodiceWidth="29%"
																		Label="Titolo" UseCode="True" SetUpperCase="True"></onitcontrols:OnitModalList></td>
																<td align="right" colspan="4">&nbsp;</td>
															</tr>
														</table>
													</fieldset>
												</td>
											</tr>
										</table>
									</td>
									<td>&nbsp;</td>
								</tr>
								<tr height="10">
									<td colspan="3">&nbsp;</td>
								</tr>
							</table>
						</div>
					</td>
				</tr>
				<tr align="center" height="85%">
					<td>&nbsp;</td>
				</tr>
			</table>
        </dyp:DynamicPanel>
    </on_lay3:OnitLayout3>
    </form>
</body>
</html>