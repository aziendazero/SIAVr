<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Distretti.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_Distretti" %>

<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
		<title>Distretti</title>

        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/archivi.css") %>' type="text/css" rel="stylesheet" />
		
        <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("onvac.common.js")%>'></script>
        <script type="text/javascript" language="javascript">
		    function ToolBarClick(ToolBar, button, evnt)
            {
                evnt.needPostBack = true;
		    }
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" width="100%" height="100%" TitleCssClass="Title3" Titolo="Distretti">
				<ondp:OnitDataPanel id="odpDistrettiMaster" runat="server" useToolbar="False" renderOnlyChildren="True" ConfigFile="Distretti.odpDistrettiMaster.xml">
                    <div>
						<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                            <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                            <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
							<ClientSideEvents Click="ToolBarClick"></ClientSideEvents>
							<Items>
								<igtbar:TBarButton Key="btnCerca" Text="Cerca"></igtbar:TBarButton>
								<igtbar:TBSeparator></igtbar:TBSeparator>
								<igtbar:TBarButton Key="btnNew" Text="Nuovo"></igtbar:TBarButton>
								<igtbar:TBarButton Key="btnEdit" Text="Modifica"></igtbar:TBarButton>
								<igtbar:TBSeparator></igtbar:TBSeparator>
								<igtbar:TBarButton Key="btnSalva" Text="Salva"></igtbar:TBarButton>
								<igtbar:TBarButton Key="btnAnnulla" Text="Annulla"></igtbar:TBarButton>
							</Items>
						</igtbar:UltraWebToolbar>
                    </div>
					<div class="Sezione">Modulo ricerca</div>
                    <div>
						<ondp:wzFilter id="WzFilter1" runat="server" Width="100%" Height="70px" CssClass="InfraUltraWebTab2">
							<DefaultTabStyle CssClass="InfraTab_Default2"></DefaultTabStyle>
							<SelectedTabStyle CssClass="InfraTab_Selected2"></SelectedTabStyle>
							<DisabledTabStyle CssClass="InfraTab_Disabled2"></DisabledTabStyle>
							<HoverTabStyle CssClass="InfraTab_Hover2"></HoverTabStyle>
							<Tabs>
								<igtab:Tab Text="Ricerca di Base">
									<ContentTemplate>
										<table height="100%" cellspacing="10" style="table-layout:fixed" cellpadding="0" width="100%" border="0">
											<tr>
												<td align="right" width="90">
													<asp:Label id="Label1" runat="server" CssClass="label">Filtro di Ricerca</asp:Label></td>
												<td>
													<asp:TextBox id="WzFilterKeyBase" runat="server" cssclass="textbox_stringa w100p"></asp:TextBox></td>
											</tr>
										</table>
									</ContentTemplate>
								</igtab:Tab>
							</Tabs>
						</ondp:wzFilter>
                    </div>
                    <div class="Sezione">Elenco</div>

                    <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                        <ondp:wzMsDataGrid id="dgrDistretti" runat="server" Width="100%" OnitStyle="False" EditMode="None" 
                            PagerVoicesAfter="-1" PagerVoicesBefore="-1" AutoGenerateColumns="False" SelectionOption="rowClick">
                            <HeaderStyle CssClass="header"></HeaderStyle>
                            <ItemStyle CssClass="item"></ItemStyle>
                            <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
                            <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
                            <EditItemStyle CssClass="edit"></EditItemStyle>
                            <FooterStyle CssClass="footer"></FooterStyle>
                            <PagerStyle CssClass="pager"></PagerStyle>	                  
	                        <Columns>
		                        <ondp:wzBoundColumn HeaderText="Codice" Key="DIS_CODICE" SourceField="DIS_CODICE" SourceTable="T_ANA_DISTRETTI" SourceConn="connessioneMaster">
                                    <HeaderStyle width="20%" />
                                </ondp:wzBoundColumn>
		                        <ondp:wzBoundColumn HeaderText="Descrizione" Key="DIS_DESCRIZIONE" SourceField="DIS_DESCRIZIONE" SourceTable="T_ANA_DISTRETTI" SourceConn="connessioneMaster">
                                    <HeaderStyle width="60%" />
                                </ondp:wzBoundColumn>
		                        <ondp:wzBoundColumn HeaderText="Codice Esterno" Key="DIS_CODICE_ESTERNO" SourceField="DIS_CODICE_ESTERNO" SourceTable="T_ANA_DISTRETTI" SourceConn="connessioneMaster">
                                    <HeaderStyle width="20%" />
                                </ondp:wzBoundColumn>
	                        </Columns>
	                        <BindingColumns>
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="Codice" Connection="connessioneMaster"
                                    SourceTable="T_ANA_DISTRETTI" Hidden="False" SourceField="DIS_CODICE"></ondp:BindingFieldValue>
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="Descrizione" Connection="connessioneMaster"
			                        SourceTable="T_ANA_DISTRETTI" Hidden="False" SourceField="DIS_DESCRIZIONE"></ondp:BindingFieldValue>
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="Codice Esterno" Connection="connessioneMaster"
			                        SourceTable="T_ANA_DISTRETTI" Hidden="False" SourceField="DIS_CODICE_ESTERNO"></ondp:BindingFieldValue>
	                        </BindingColumns>
                        </ondp:wzMsDataGrid>
                    </dyp:DynamicPanel>
                    <div class="Sezione">Dettaglio</div>
                    <div>
						<ondp:OnitDataPanel id="odpDistrettiDetail" runat="server" useToolbar="False" renderOnlyChildren="True"
							ConfigFile="Distretti.odpDistrettiDetail.xml" BindingFields="(Insieme)" externalToolBar="ToolBar"
							externalToolBar-Length="7" dontLoadDataFirstTime="True">
							<table style="table-layout: fixed" cellspacing="3" width="100%" border="0">
								<tr align="left">
									<td class="label" width="10%">Codice</td>
									<td width="40%">
										<ondp:wzTextBox id="txtCodice" runat="server" maxlength="8" onblur="toUpper(this);"
                                            BindingField-SourceField="DIS_CODICE" BindingField-Hidden="False" BindingField-SourceTable="T_ANA_DISTRETTI"
											BindingField-Connection="connessioneSec" BindingField-Editable="onNew"
											CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p" CssStyles-CssEnabled="TextBox_Stringa w100p"
											CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"></ondp:wzTextBox></td>
									<td class="label" width="20%">Codice Esterno</td>
									<td width="30%">
										<ondp:wzTextBox id="txtCodiceEsterno" runat="server" BindingField-SourceField="DIS_CODICE_ESTERNO"
											maxlength="20" BindingField-Hidden="False" BindingField-SourceTable="T_ANA_DISTRETTI"
											BindingField-Connection="connessioneSec" BindingField-Editable="always" 
											CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p" CssStyles-CssEnabled="TextBox_Stringa w100p"
											CssStyles-CssRequired="TextBox_Stringa w100p"></ondp:wzTextBox></td>
								</tr>
								<tr>
									<td class="label">Descrizione</td>
									<td colspan="3">
										<ondp:wzTextBox id="txtDescrizione" runat="server" maxlength="35" onblur="toUpper(this);" 
                                            BindingField-SourceField="DIS_DESCRIZIONE" BindingField-Hidden="False" BindingField-SourceTable="T_ANA_DISTRETTI"
											BindingField-Connection="connessioneSec" BindingField-Editable="always" 
											CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p" CssStyles-CssEnabled="TextBox_Stringa w100p"
											CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"></ondp:wzTextBox></td>
								</tr>
								<tr>
									<td class="label">USL</td>
									<td colspan="3">
										<ondp:wzFinestraModale id="wzFmUsl" runat="server" Width="70%" BindingDescription-Connection="connessioneSec"
											BindingCode-Connection="connessioneSec" BindingDescription-SourceField="USL_DESCRIZIONE" BindingCode-SourceField="DIS_USL_CODICE"
											PosizionamentoFacile="False" LabelWidth="-1px" CodiceWidth="30%" Tabella="T_ANA_USL" Sorting="True"
											SetUpperCase="True" Paging="True" PageSize="50" MaxRecords="0" CampoCodice="USL_CODICE as CODICE" CampoDescrizione="USL_DESCRIZIONE as DESCRIZIONE"
											BindingCode-Editable="always" BindingCode-SourceTable="T_ANA_DISTRETTI" BindingCode-Hidden="False" BindingDescription-Editable="always"
											BindingDescription-SourceTable="T_ANA_USL" BindingDescription-Hidden="False" UseCode="True" UseTableLayout="True"
											Filtro=""></ondp:wzFinestraModale>
									</td>
								</tr>
							</table>
						</ondp:OnitDataPanel>
					</div>
				</ondp:OnitDataPanel>
			</on_lay3:onitlayout3>
        </form>
	</body>
</html>