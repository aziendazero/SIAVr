<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Circoscrizioni.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_Circoscrizioni" %>

<%@ Register TagPrefix="on_otb" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitTable" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
		<title>Circoscrizioni</title>
		
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

        <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("onvac.common.js")%>'></script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" Titolo="Circoscrizioni" TitleCssClass="Title3" height="100%" width="100%">
				<ondp:OnitDataPanel id="odpCircoscrizioniMaster" runat="server" ConfigFile="Circoscrizioni.odpCircoscrizioniMaster.xml" renderOnlyChildren="True" useToolbar="False">
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
										<igtbar:TBSeparator></igtbar:TBSeparator>
										<igtbar:TBarButton Key="btnSalva" Text="Salva"></igtbar:TBarButton>
										<igtbar:TBarButton Key="btnAnnulla" Text="Annulla"></igtbar:TBarButton>
									</Items>
								</igtbar:UltraWebToolbar>

								<div class="Sezione">Modulo ricerca</div>

								<ondp:wzFilter id="WzFilter1" runat="server" Width="100%" Height="70px" CssClass="InfraUltraWebTab2" TargetUrl="" Dummy>
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
															<asp:Label id="Label1" runat="server" CssClass="LABEL">Filtro di Ricerca</asp:Label></td>
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
								<ondp:wzDataGrid Browser="UpLevel" id="dgrCircoscrizioni" runat="server" Width="100%" PagerVoicesBefore="-1" PagerVoicesAfter="-1"
									OnitStyle="False" EditMode="None">
									<DisplayLayout ColWidthDefault="" AutoGenerateColumns="False" RowHeightDefault="26px" Version="2.00"
										GridLinesDefault="None" SelectTypeRowDefault="Single" ScrollBar="Never" BorderCollapseDefault="Separate"
										CellPaddingDefault="3" RowSelectorsDefault="No" Name="dgrCircoscrizioni" CellClickActionDefault="RowSelect">
										<HeaderStyleDefault HorizontalAlign="Left" CssClass="Infra2Dgr_Header"></HeaderStyleDefault>
										<RowSelectorStyleDefault CssClass="Infra2Dgr_RowSelector"></RowSelectorStyleDefault>
										<FrameStyle Width="100%"></FrameStyle>
										<ActivationObject BorderWidth="0px" BorderColor="Navy"></ActivationObject>
										<Images></Images>
										<SelectedRowStyleDefault CssClass="Infra2Dgr_SelectedRow"></SelectedRowStyleDefault>
										<RowAlternateStyleDefault CssClass="Infra2Dgr_RowAlternate"></RowAlternateStyleDefault>
										<RowStyleDefault CssClass="Infra2Dgr_Row"></RowStyleDefault>
									</DisplayLayout>
									<Bands>
										<igtbl:UltraGridBand>
											<Columns>
												<igtbl:UltraGridColumn HeaderText="Codice" Width="10%">
													<Footer Key=""></Footer>
													<Header Key="" Caption="Codice"></Header>
												</igtbl:UltraGridColumn>
												<igtbl:UltraGridColumn HeaderText="Descrizione" Width="60%">
													<Footer Key="">
														<RowLayoutColumnInfo OriginX="1"></RowLayoutColumnInfo>
													</Footer>
													<Header Key="" Caption="Descrizione">
														<RowLayoutColumnInfo OriginX="1"></RowLayoutColumnInfo>
													</Header>
												</igtbl:UltraGridColumn>
												<igtbl:UltraGridColumn HeaderText="CodiceMnemonico" Width="15%">
													<Footer Key="">
														<RowLayoutColumnInfo OriginX="2"></RowLayoutColumnInfo>
													</Footer>
													<Header Key="" Caption="CodiceMnemonico">
														<RowLayoutColumnInfo OriginX="2"></RowLayoutColumnInfo>
													</Header>
												</igtbl:UltraGridColumn>
											</Columns>
										</igtbl:UltraGridBand>
									</Bands>
									<BindingColumns>
										<ondp:BindingFieldValue Value="" Editable="always" Description="Codice" Connection="connessioneMaster" SourceTable="T_ANA_CIRCOSCRIZIONI"
											Hidden="False" SourceField="CIR_CODICE"></ondp:BindingFieldValue>
										<ondp:BindingFieldValue Value="" Editable="always" Description="Descrizione" Connection="connessioneMaster"
											SourceTable="T_ANA_CIRCOSCRIZIONI" Hidden="False" SourceField="CIR_DESCRIZIONE"></ondp:BindingFieldValue>
										<ondp:BindingFieldValue Value="" Editable="always" Description="CodiceMnemonico" Connection="connessioneMaster" SourceTable="T_ANA_CIRCOSCRIZIONI"
											Hidden="False" SourceField="CIR_CODICE_MNEMONICO"></ondp:BindingFieldValue>
									</BindingColumns>
								</ondp:WzDataGrid>
							</on_otb:onitcell>
						</on_otb:onitsection>

						<on_otb:onitsection id="sezDettagli" runat="server" width="100%" TypeHeight="Content">
							<on_otb:onitcell id="cellaDettagli" runat="server" height="100%" Width="100%">
								<div class="Sezione">Dettaglio</div>
								<ondp:OnitDataPanel id="odpCircoscrizioniDetail" runat="server" ConfigFile="Circoscrizioni.odpCircoscrizioniDetail.xml" renderOnlyChildren="True"
									useToolbar="False" dontLoadDataFirstTime="True" externalToolBar-Length="7" externalToolBar="ToolBar" BindingFields="(Insieme)">
									<table style="table-layout: fixed" cellspacing="3" width="100%" border="0">
										<colgroup>
											<col width="10%" />
											<col width="25%" />
											<col width="25%" />
											<col width="40%" />
											<col>
										</colgroup>
										<tr align="left">
											<td class="label">Codice</td>
											<td>
												<ondp:wzTextBox id="txtCodice" onblur="toUpper(this);" runat="server" MaxLength="8"
													CssStyles-CssRequired="textbox_numerico_obbligatorio w100p" CssStyles-CssEnabled="textbox_numerico w100p"
													CssStyles-CssDisabled="textbox_numerico_disabilitato w100p" BindingField-SourceField="CIR_CODICE"
													BindingField-Hidden="False" BindingField-SourceTable="T_ANA_CIRCOSCRIZIONI" BindingField-Connection="CircoscrizioniDettaglio"
													BindingField-Editable="never"></ondp:wzTextBox></td>
											<td class="label">Codice Mnemonico</td>
											<td>
												<ondp:wzTextBox id="txtCodiceEsterno" runat="server" maxlength="8" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
													CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
													BindingField-Editable="always" BindingField-Connection="CircoscrizioniDettaglio"
													BindingField-SourceTable="T_ANA_CIRCOSCRIZIONI" BindingField-Hidden="False" 
                                                    BindingField-SourceField="CIR_CODICE_MNEMONICO"></ondp:wzTextBox></td>
										</tr>
										<tr>
											<td class="label">Descrizione</td>
											<td colspan="3">
												<ondp:wzTextBox id="txtDescrizione" onblur="toUpper(this);" runat="server" maxlength="35" 
                                                    CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p" CssStyles-CssEnabled="TextBox_Stringa w100p" 
                                                    CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p" BindingField-Editable="always" 
                                                    BindingField-Connection="CircoscrizioniDettaglio" BindingField-SourceTable="T_ANA_CIRCOSCRIZIONI" BindingField-Hidden="False" 
                                                    BindingField-SourceField="CIR_DESCRIZIONE"></ondp:wzTextBox></td>
										</tr>
										<tr>
											<td class="label">Comune</td>
											<td colspan="3">
												<ondp:wzFinestraModale id="fmComune" runat="server" Width="69%" PosizionamentoFacile="False" LabelWidth="-1px"
													CodiceWidth="30%" BindingCode-Editable="onNew" BindingCode-Connection="CircoscrizioniDettaglio" BindingCode-SourceTable="T_ANA_CIRCOSCRIZIONI"
													BindingCode-Hidden="False" BindingCode-SourceField="CIR_COM_CODICE" UseCode="True" RaiseChangeEvent="False"
													SetUpperCase="True" Obbligatorio="False" BindingDescription-Editable="always" BindingDescription-Hidden="False"
													BindingDescription-Connection="CircoscrizioniDettaglio" BindingDescription-SourceTable="T_ANA_COMUNI"
													BindingDescription-SourceField="COM_DESCRIZIONE" CampoCodice="COM_CODICE" CampoDescrizione="COM_DESCRIZIONE"
													Tabella="T_ANA_COMUNI"></ondp:wzFinestraModale></td>
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