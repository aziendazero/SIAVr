<%@ Page Language="vb" AutoEventWireup="false" Codebehind="EsitiTerapie.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_EsitiTerapie" %>

<%@ Register TagPrefix="on_otb" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitTable" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
		<title>EsitiTerapie</title>
		
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

        <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("onvac.common.js")%>'></script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" Titolo="Esiti Terapie" TitleCssClass="Title3" height="100%" width="100%">
				<ondp:OnitDataPanel id="odpEsitiTerapieMaster" runat="server" ConfigFile="EsitiTerapie.odpEsitiTerapieMaster.xml" renderOnlyChildren="True" Width="100%" useToolBar="False">
					<on_otb:onittable id="OnitTable1" runat="server" width="100%" height="100%">
						<on_otb:onitsection id="sezRicerca" runat="server" width="100%" typeHeight="content">
							<on_otb:onitcell id="cellaRicerca" runat="server" height="100%">
								<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		                            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
									<Items>
										<igtbar:TBarButton Key="btnCerca" Text="Cerca"></igtbar:TBarButton>
										<igtbar:TBSeparator></igtbar:TBSeparator>
										<igtbar:TBarButton Key="btnNew" Text="Nuovo"></igtbar:TBarButton>
										<igtbar:TBarButton Key="btnEdit" Text="Modifica"></igtbar:TBarButton>
										<igtbar:TBarButton Key="btnElimina" Text="Elimina"></igtbar:TBarButton>
										<igtbar:TBSeparator></igtbar:TBSeparator>
										<igtbar:TBarButton Key="btnSalva" Text="Salva"></igtbar:TBarButton>
										<igtbar:TBarButton Key="btnAnnulla" Text="Annulla"></igtbar:TBarButton>
									</Items>
								</igtbar:UltraWebToolbar>
								<div class="Sezione">Modulo ricerca</div>
								<ondp:wzFilter id="WzFilter1" runat="server" Width="100%" Height="70px" CssClass="InfraUltraWebTab2">
									<DefaultTabStyle CssClass="InfraTab_Default2"></DefaultTabStyle>
									<SelectedTabStyle CssClass="InfraTab_Selected2"></SelectedTabStyle>
									<DisabledTabStyle CssClass="InfraTab_Disabled2"></DisabledTabStyle>
									<HoverTabStyle CssClass="InfraTab_Hover2"></HoverTabStyle>
									<Tabs>
										<igtab:Tab Text="Ricerca di Base">
											<ContentTemplate>
												<table height="100%" cellSpacing="10" cellPadding="0" width="100%" border="0" style="table-layout:fixed">
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
								<div class="Sezione">Elenco</div>
							</on_otb:onitcell>
						</on_otb:onitsection>
						<on_otb:onitsection id="sezRisultati" runat="server" width="100%" typeHeight="calculate">
							<on_otb:onitcell id="cellaRisultati" runat="server" height="100%" typescroll="auto">
								<ondp:wzDataGrid Browser="UpLevel" id="dgrRicerca" runat="server" Width="100%" OnitStyle="False" EditMode="None">
									<DisplayLayout ColWidthDefault="" AutoGenerateColumns="False" RowHeightDefault="26px" Version="2.00" GridLinesDefault="None" SelectTypeRowDefault="Single" ScrollBar="Never" BorderCollapseDefault="Separate" CellPaddingDefault="3" RowSelectorsDefault="No" Name="dgrRicerca" CellClickActionDefault="RowSelect">
										<HeaderStyleDefault CssClass="Infra2Dgr_Header"></HeaderStyleDefault>
										<RowSelectorStyleDefault CssClass="Infra2Dgr_RowSelector"></RowSelectorStyleDefault>
										<FrameStyle Width="100%"></FrameStyle>
										<ActivationObject BorderWidth="0px" BorderColor="Navy"></ActivationObject>
										<SelectedRowStyleDefault CssClass="Infra2Dgr_SelectedRow"></SelectedRowStyleDefault>
										<RowAlternateStyleDefault CssClass="Infra2Dgr_RowAlternate"></RowAlternateStyleDefault>
										<RowStyleDefault CssClass="Infra2Dgr_Row"></RowStyleDefault>
									</DisplayLayout>
									<Bands>
										<igtbl:UltraGridBand>
											<Columns>
												<igtbl:UltraGridColumn HeaderText="&lt;input type=checkbox  OnClick='javascript:wzDataGrid_Master_Click(this,&amp;quot;dgrRicerca&amp;quot;);' /&gt;" Key="check" Width="0%" Hidden="True" Type="CheckBox" BaseColumnName="" AllowUpdate="Yes"></igtbl:UltraGridColumn>
												<igtbl:UltraGridColumn HeaderText="Codice" Key="" Width="100px" BaseColumnName=""></igtbl:UltraGridColumn>
												<igtbl:UltraGridColumn HeaderText="Descrizione" Key="" Width="100%" BaseColumnName=""></igtbl:UltraGridColumn>
											</Columns>
										</igtbl:UltraGridBand>
									</Bands>
									<BindingColumns>
										<ondp:BindingFieldValue Value="" Editable="always" Description="Codice" Connection="connessioneMaster" SourceTable="T_ANA_ESITI_TERAPIE" Hidden="False" SourceField="ESI_CODICE"></ondp:BindingFieldValue>
										<ondp:BindingFieldValue Value="" Editable="always" Description="Descrizione" Connection="connessioneMaster" SourceTable="T_ANA_ESITI_TERAPIE" Hidden="False" SourceField="ESI_DESCRIZIONE"></ondp:BindingFieldValue>
									</BindingColumns>
								</ondp:wzDataGrid>
							</on_otb:onitcell>
						</on_otb:onitsection>
						<on_otb:onitsection id="sezDettaglio" runat="server" width="100%" height="80px" TypeHeight="manual">
							<on_otb:onitcell id="cellaDettaglio" runat="server" height="100%">
								<div class="Sezione">Dettaglio</div>
								<ondp:OnitDataPanel id="odpEsitiTerapieDetail" runat="server" ConfigFile="EsitiTerapie.odpEsitiTerapieDetail.xml" renderOnlyChildren="True" Width="100%" Height="100px" useToolbar="False" dontLoadDataFirstTime="True" BindingFields="(Insieme)" externalToolBar="ToolBar" externalToolBar-Length="7">
									<table style="table-layout: fixed" cellspacing="3" width="100%" align="left" border="0">
										<tr>
											<td class="label" width="80">Codice</td>
											<td>
												<ondp:wzTextBox id="txtCodice" runat="server" onblur="toUpper(this);" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p" 
                                                    CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p" 
                                                    BindingField-Editable="onNew" BindingField-Connection="connessioneSec" BindingField-SourceTable="T_ANA_ESITI_TERAPIE" 
                                                    BindingField-Hidden="False" maxlength="8" BindingField-SourceField="ESI_CODICE"></ondp:wzTextBox></td>
										</tr>
										<tr>
											<td class="label" width="80">Descrizione</td>
											<td>
												<ondp:wzTextBox id="txtDescrizione" runat="server" onblur="toUpper(this);" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p" 
                                                    CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p" 
                                                    BindingField-Editable="always" BindingField-Connection="connessioneSec" BindingField-SourceTable="T_ANA_ESITI_TERAPIE" 
                                                    BindingField-Hidden="False" maxlength="35" BindingField-SourceField="ESI_DESCRIZIONE"></ondp:wzTextBox></td>
										</tr>
									</table>
								</ondp:OnitDataPanel>
							</on_otb:onitcell>
						</on_otb:onitsection>
					</on_otb:onittable>
				</ondp:OnitDataPanel>
			</on_lay3:onitlayout3>
		</form>
	</body>
</html>
