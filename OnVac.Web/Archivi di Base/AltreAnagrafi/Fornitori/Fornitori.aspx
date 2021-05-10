<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Fornitori.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_Fornitori" %>

<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_otb" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitTable" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
		<title>Fornitori</title>

        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

        <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("onvac.common.js")%>'></script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" width="100%" height="100%" TitleCssClass="Title3" Titolo="Fornitori">
				<ondp:OnitDataPanel id="odpFornitoriMaster" runat="server" useToolbar="False" renderOnlyChildren="True" ConfigFile="Fornitori.odpFornitoriMaster.xml">
					<on_otb:onittable id="OnitTable1" runat="server" height="100%" width="100%">
						<on_otb:onitsection id="sezRicerca" runat="server" width="100%" typeHeight="content">
							<on_otb:onitcell id="cellaRicerca" runat="server" height="100%" Width="100%">
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
								<ondp:wzFilter id="TabFiltri" runat="server" Width="100%" Height="70px" CssClass="InfraUltraWebTab2">
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
								<div class="Sezione">Elenco</div>
							</on_otb:onitcell>
						</on_otb:onitsection>
						<on_otb:onitsection id="sezDgr" runat="server" width="100%" typeHeight="calculate">
							<on_otb:onitcell id="cellaDgr" runat="server" height="100%" Width="100%" typescroll="auto">
								<ondp:wzDataGrid Browser="UpLevel" id="dgrFornitori" runat="server" Width="100%" EditMode="None" OnitStyle="False" PagerVoicesAfter="-1" PagerVoicesBefore="-1">
									<Bands>
										<igtbl:UltraGridBand>
											<Columns>
												<igtbl:UltraGridColumn HeaderText="Codice" Key="" Width="100px" BaseColumnName="">
													<Footer Key=""></Footer>
													<Header Key="" Caption="Codice"></Header>
												</igtbl:UltraGridColumn>
												<igtbl:UltraGridColumn HeaderText="Descrizione" Key="" Width="70%" BaseColumnName="">
													<Footer Key=""></Footer>
													<Header Key="" Caption="Descrizione"></Header>
												</igtbl:UltraGridColumn>
											</Columns>
										</igtbl:UltraGridBand>
									</Bands>
									<DisplayLayout ColWidthDefault="" AutoGenerateColumns="False" RowHeightDefault="26px" Version="2.00"
										GridLinesDefault="None" SelectTypeRowDefault="Single" ScrollBar="Never" BorderCollapseDefault="Separate"
										CellPaddingDefault="3" RowSelectorsDefault="No" Name="dgrFornitori" CellClickActionDefault="RowSelect">
										<HeaderStyleDefault CssClass="Infra2Dgr_Header" horizontalAlign="left"></HeaderStyleDefault>
										<RowSelectorStyleDefault CssClass="Infra2Dgr_RowSelector"></RowSelectorStyleDefault>
										<FrameStyle Width="100%"></FrameStyle>
										<ActivationObject BorderWidth="0px" BorderColor="Navy"></ActivationObject>
										<SelectedRowStyleDefault CssClass="Infra2Dgr_SelectedRow"></SelectedRowStyleDefault>
										<RowAlternateStyleDefault CssClass="Infra2Dgr_RowAlternate"></RowAlternateStyleDefault>
										<RowStyleDefault CssClass="Infra2Dgr_Row"></RowStyleDefault>
									</DisplayLayout>
									<BindingColumns>
										<ondp:BindingFieldValue Value="" Editable="always" Description="Codice" Connection="connessioneMaster" SourceTable="T_ANA_FORNITORI"
											Hidden="False" SourceField="FOR_CODICE"></ondp:BindingFieldValue>
										<ondp:BindingFieldValue Value="" Editable="always" Description="Descrizione" Connection="connessioneMaster"
											SourceTable="T_ANA_FORNITORI" Hidden="False" SourceField="FOR_DESCRIZIONE"></ondp:BindingFieldValue>
									</BindingColumns>
								</ondp:WzDataGrid>
							</on_otb:onitcell>
						</on_otb:onitsection>
						<on_otb:onitsection id="sezDettagli" runat="server" width="100%" TypeHeight="Content">
							<on_otb:onitcell id="cellaDettagli" runat="server" height="100%" Width="100%">
								<div class="Sezione">Dettaglio</div>
								<ondp:OnitdataPanel id="odpFornitoriDetail" runat="server" useToolbar="False" renderOnlyChildren="True"
									ConfigFile="Fornitori.odpFornitoriDetail.xml" BindingFields="(Insieme)" externalToolBar="ToolBar"
									externalToolBar-Length="7" dontLoadDataFirstTime="True">
									<table style="table-layout: fixed" cellspacing="3" width="100%" border="0">
										<tr align="left">
											<td class="label_left" width="150">Codice</td>
											<td colspan="3">
												<ondp:wzTextBox id="txtCodice" runat="server" onblur="toUpper(this);" BindingField-SourceField="FOR_CODICE"
													maxlength="8" BindingField-Hidden="False" BindingField-SourceTable="T_ANA_FORNITORI" BindingField-Connection="connessioneSec"
													BindingField-Editable="onNew" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
													CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"></ondp:wzTextBox></td>
										</tr>
										<tr>
											<td class="label_left" width="150">Descrizione</td>
											<td colspan="3">
												<ondp:wzTextBox id="txtDescrizione" runat="server" onblur="toUpper(this);" BindingField-SourceField="FOR_DESCRIZIONE"
													maxlength="100" BindingField-Hidden="False" BindingField-SourceTable="T_ANA_FORNITORI" BindingField-Connection="connessioneSec"
													BindingField-Editable="always" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
													CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"></ondp:wzTextBox></td>
										</tr>
										<tr align="left">
											<td class="label_left" width="150">Codice Esterno</td>
											<td colspan="3">
												<ondp:wzTextBox id="txtCodiceEsterno" runat="server" onblur="toUpper(this);" BindingField-SourceField="FOR_CODICE_ESTERNO"
													maxlength="10" BindingField-Hidden="False" BindingField-SourceTable="T_ANA_FORNITORI" BindingField-Connection="connessioneSec"
													BindingField-Editable="always" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
													CssStyles-CssEnabled="TextBox_Stringa w100p" ></ondp:wzTextBox></td>
										</tr>
									</table>
								</ondp:OnitdataPanel>
							</on_otb:onitcell>
						</on_otb:onitsection>
					</on_otb:onittable>
				</ondp:OnitDataPanel>
			</on_lay3:onitlayout3>
		</form>
	</body>
</html>
