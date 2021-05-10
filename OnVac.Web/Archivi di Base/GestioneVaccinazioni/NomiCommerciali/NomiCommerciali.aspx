<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="NomiCommerciali.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_NomiCommerciali" %>

<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<%@ Register TagPrefix="uc1" TagName="ElencoCondizioniPagamento" Src="../../../Common/Controls/ElencoCondizioniPagamento.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>NomiCommerciali</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <style type="text/css">
        .w50PX {
            width: 50px;
        }

        .w40PX {
            width: 40px;
        }

        /* patch per template column */
        .Infra2Dgr_Header {
            background-color: #4A3C8C;
            color: #ffffff;
            font-weight: bold;
            text-decoration: none;
            text-align: center;
            border-bottom: 1px solid #4A3C8C;
            height: 25px;
        }
    </style>

    <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("onvac.common.js")%>'></script>
    <script language="javascript" type="text/javascript">
        function removeNL(s) {
            return s.replace(/\n/g, ',').replace(/\s/g, '').replace(/  ,/g, ',');
        }

        function Count(text, long) {
            if (!text.readOnly) {
                text.value = removeNL(text.value);
                var maxlength = new Number(long); // Change number to your max length.
                if (text.value.length > maxlength) {
                    text.value = text.value.substring(0, maxlength);
                    alert(" Raggiunto il numero massimo di caratteri ammessi per il campo");
                }
            }
        }

        function Count(text) {
            if (!text.readOnly) {
                text.value = removeNL(text.value);
                var maxlength = new Number(256); // Change number to your max length.
                if (text.value.length > maxlength) {
                    text.value = text.value.substring(0, maxlength);
                    alert(" Raggiunto il numero massimo di caratteri ammessi per il campo");
                }
            }
        }
    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <on_lay3:OnitLayout3 ID="Onitlayout31" runat="server" TitleCssClass="Title3" Titolo="Nomi Commerciali" Width="100%" Height="100%" NAME="Onitlayout31">
            <ondp:onitdatapanel id="OdpNomiCommercialiMaster" runat="server" width="100%" usetoolbar="False" configfile="NomiCommerciali.OdpNomiCommercialiMaster.xml"
                renderonlychildren="True" defaultsort="nomiComConn.T_ANA_NOMI_COMMERCIALI.noc_descrizione">
                <div>
					<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBarNomiCom" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
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
							<igtbar:TBSeparator></igtbar:TBSeparator>
							<igtbar:TBarButton Key="btnAssociazioni" Text="Associazioni" Image="../../../Images/Associazioni.gif">
								<DefaultStyle Width="120px" CssClass="infratoolbar_button_default"></DefaultStyle>
							</igtbar:TBarButton>
                            <igtbar:TBarButton Key="btnPagamento" Text="Condizioni Pagamento" Image="../../../Images/euro_blu.png">
                                <DefaultStyle Width="160px" CssClass="infratoolbar_button_default"></DefaultStyle>
                            </igtbar:TBarButton>
						</Items>
					</igtbar:UltraWebToolbar>
                </div>
				<div class="Sezione">Modulo ricerca</div>
                <div>
					<ondp:wzFilter id="WzFilter1" runat="server" Height="70px" Width="100%" CssClass="InfraUltraWebTab2">
                        <DefaultTabStyle CssClass="InfraTab_Default2"></DefaultTabStyle>
						<SelectedTabStyle CssClass="InfraTab_Selected2"></SelectedTabStyle>
						<DisabledTabStyle CssClass="InfraTab_Disabled2"></DisabledTabStyle>
						<HoverTabStyle CssClass="InfraTab_Hover2"></HoverTabStyle>
						<Tabs>
							<igtab:Tab Text="Ricerca di Base">
								<ContentTemplate>
									<table height="100%" cellspacing="10" cellpadding="0" width="100%" border="0" style="table-layout:fixed">
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
                </div>
				<div class="Sezione">Elenco</div>

                <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
					<ondp:wzDataGrid Browser="UpLevel" id="WzDgrNomiCom" runat="server" Width="100%" EditMode="None">
						<DisplayLayout ColWidthDefault="" AutoGenerateColumns="False" RowHeightDefault="26px" Version="2.00"
							GridLinesDefault="None" SelectTypeRowDefault="Single" ScrollBar="Never" BorderCollapseDefault="Separate"
							CellPaddingDefault="3" RowSelectorsDefault="No" Name="WzDgrNomiCom" CellClickActionDefault="RowSelect">
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
									<igtbl:UltraGridColumn HeaderText="&lt;input type=checkbox OnClick='javascript:wzDataGrid_Master_Click(this,&amp;quot;WzDgrVaccinazioni&amp;quot;);' /&gt;"
										Key="check" AllowUpdate="Yes" Hidden="True" Type="CheckBox">
										<Footer Key="check"></Footer>
										<Header Key="check" Caption="&lt;input type=checkbox OnClick='javascript:wzDataGrid_Master_Click(this,&amp;quot;WzDgrVaccinazioni&amp;quot;);' /&gt;"></Header>
									</igtbl:UltraGridColumn>
									<igtbl:UltraGridColumn HeaderText="Codice" Width="10%">
										<Footer Key="">
											<RowLayoutColumnInfo OriginX="1"></RowLayoutColumnInfo>
										</Footer>
										<Header Key="" Caption="Codice">
											<RowLayoutColumnInfo OriginX="1"></RowLayoutColumnInfo>
										</Header>
									</igtbl:UltraGridColumn>
									<igtbl:UltraGridColumn HeaderText="Descrizione" Width="45%">
										<Footer Key="">
											<RowLayoutColumnInfo OriginX="2"></RowLayoutColumnInfo>
										</Footer>
										<Header Key="" Caption="Descrizione">
											<RowLayoutColumnInfo OriginX="2"></RowLayoutColumnInfo>
										</Header>
									</igtbl:UltraGridColumn>
									<igtbl:UltraGridColumn HeaderText="Sesso" Width="100px">
										<CellStyle HorizontalAlign="Center"></CellStyle>
										<Footer Key="">
											<RowLayoutColumnInfo OriginX="3"></RowLayoutColumnInfo>
										</Footer>
										<Header Key="" Caption="Sesso">
											<RowLayoutColumnInfo OriginX="3"></RowLayoutColumnInfo>
										</Header>
									</igtbl:UltraGridColumn>
									<igtbl:UltraGridColumn HeaderText="Associazione" Width="20%" Hidden="True">
										<Footer Key="">
											<RowLayoutColumnInfo OriginX="4"></RowLayoutColumnInfo>
										</Footer>
										<Header Key="" Caption="Associazione">
											<RowLayoutColumnInfo OriginX="4"></RowLayoutColumnInfo>
										</Header>
									</igtbl:UltraGridColumn>
									<igtbl:UltraGridColumn HeaderText="Fornitore" Width="15%">
										<Footer Key="">
											<RowLayoutColumnInfo OriginX="5"></RowLayoutColumnInfo>
										</Footer>
										<Header Key="" Caption="Fornitore">
											<RowLayoutColumnInfo OriginX="5"></RowLayoutColumnInfo>
										</Header>
									</igtbl:UltraGridColumn>
									<igtbl:UltraGridColumn HeaderText="NOC_ETA_INIZIO" Hidden="True">
										<Footer Key="">
											<RowLayoutColumnInfo OriginX="6"></RowLayoutColumnInfo>
										</Footer>
										<Header Key="" Caption="NOC_ETA_INIZIO">
											<RowLayoutColumnInfo OriginX="6"></RowLayoutColumnInfo>
										</Header>
									</igtbl:UltraGridColumn>
									<igtbl:UltraGridColumn HeaderText="NOC_ETA_FINE" Hidden="True">
										<Footer Key="">
											<RowLayoutColumnInfo OriginX="7"></RowLayoutColumnInfo>
										</Footer>
										<Header Key="" Caption="NOC_ETA_FINE">
											<RowLayoutColumnInfo OriginX="7"></RowLayoutColumnInfo>
										</Header>
									</igtbl:UltraGridColumn>
									<igtbl:TemplatedColumn Width="15%" Key="ETAINIZIALE">
										<HeaderTemplate>
                                            <div style="width:130px; text-align:center;"><span>Inizio Validità</span></div>
											<table id="Table5" cellspacing="0" cellpadding="0" width="130" border="0" style="table-layout:fixed">
												<tr>
													<td width="33%" class="Infra2Dgr_Header">Anni</td>
													<td width="33%" class="Infra2Dgr_Header">Mesi</td>
													<td width="34%" class="Infra2Dgr_Header">Giorni</td>
												</tr>
											</table>
										</HeaderTemplate>
										<Footer Key="ETAINIZIALE">
											<RowLayoutColumnInfo OriginX="8"></RowLayoutColumnInfo>
										</Footer>
										<Header Key="ETAINIZIALE">
											<RowLayoutColumnInfo OriginX="8"></RowLayoutColumnInfo>
										</Header>
										<CellTemplate>
											<table id="Table8" cellspacing="0" cellpadding="0" width="130" border="0" style="table-layout:fixed">
												<tr>
													<td align="center">
														<asp:Label id="inizioAnni" runat="server" cssclass="label"></asp:Label></td>
													<td align="center">
														<asp:Label id="inizioMesi" runat="server" cssclass="label"></asp:Label></td>
													<td align="center">
														<asp:Label id="inizioGiorni" runat="server" cssclass="label"></asp:Label></td>
												</tr>
											</table>
										</CellTemplate>
									</igtbl:TemplatedColumn>
									<igtbl:TemplatedColumn Width="15%" Key="ETAFINE">
										<HeaderTemplate>
											<div style="width:130px; text-align:center;"><span>Fine Validità</span></div>
											<table id="Table6" cellspacing="0" cellpadding="0" width="130px" border="0" style="table-layout:fixed">
												<tr>
													<td width="33%" class="Infra2Dgr_Header">Anni</td>
													<td width="33%" class="Infra2Dgr_Header">Mesi</td>
													<td width="34%" class="Infra2Dgr_Header">Giorni</td>
												</tr>
											</table>
										</HeaderTemplate>
										<Footer Key="ETAFINE">
											<RowLayoutColumnInfo OriginX="9"></RowLayoutColumnInfo>
										</Footer>
										<Header Key="ETAFINE">
											<RowLayoutColumnInfo OriginX="9"></RowLayoutColumnInfo>
										</Header>
										<CellTemplate>
											<table id="Table9" cellspacing="0" cellpadding="0" width="130px" border="0" style="table-layout:fixed">
												<tr>
													<td align="center">
														<asp:Label id="fineAnni" runat="server" cssclass="label"></asp:Label></td>
													<td align="center">
														<asp:Label id="fineMesi" runat="server" cssclass="label"></asp:Label></td>
													<td align="center">
														<asp:Label id="fineGiorni" runat="server" cssclass="label"></asp:Label></td>
												</tr>
											</table>
										</CellTemplate>
									</igtbl:TemplatedColumn>
								</Columns>
							</igtbl:UltraGridBand>
						</Bands>
						<BindingColumns>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Codice" Connection="nomiComConn" SourceTable="T_ANA_NOMI_COMMERCIALI"
								Hidden="False" SourceField="NOC_CODICE"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Descrizione" Connection="nomiComConn" SourceTable="T_ANA_NOMI_COMMERCIALI"
								Hidden="False" SourceField="NOC_DESCRIZIONE"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="COD_DESCRIZIONE" Connection="nomiComConn"
								SourceTable="T_ANA_CODIFICHE" Hidden="False" SourceField="COD_DESCRIZIONE"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="ASS_DESCRIZIONE" Connection="nomiComConn"
								SourceTable="T_ANA_ASSOCIAZIONI" Hidden="False" SourceField="ASS_DESCRIZIONE"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="FOR_DESCRIZIONE" Connection="nomiComConn"
								SourceTable="T_ANA_FORNITORI" Hidden="False" SourceField="FOR_DESCRIZIONE"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="NOC_ETA_INIZIO" Connection="nomiComConn"
								SourceTable="T_ANA_NOMI_COMMERCIALI" Hidden="True" SourceField="NOC_ETA_INIZIO"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="NOC_ETA_FINE" Connection="nomiComConn" SourceTable="T_ANA_NOMI_COMMERCIALI"
								Hidden="True" SourceField="NOC_ETA_FINE"></ondp:BindingFieldValue>
						</BindingColumns>
					</ondp:wzDataGrid>
                </dyp:DynamicPanel>
                
                <div class="Sezione">Dettaglio</div>
                <div>
					<ondp:OnitDataPanel id="OdpNomiCommercialiDetail" runat="server" useToolbar="False" ConfigFile="NomiCommerciali.OdpNomiCommercialiDetail.xml"
						renderOnlyChildren="True" Width="100%" externalToolBar="ToolBarNomiCom" externalToolBar-Length="14" dontLoadDataFirstTime="True">
						<table id="Table1" style="table-layout: fixed" cellspacing="3" cellpadding="0" width="100%" border="0">
							<colgroup>
								<col align="right" width="14%" />
								<col width="13%" />
								<col align="right" width="10%" />
								<col width="10%" />
								<col align="right" width="12%" />
								<col width="10%" />
								<col width="13%" />
								<col class="label" align="right" width="8%" />
								<col width="9%" />
							</colgroup>
							<tr>
								<td class="label">Codice</td>
								<td>
									<ondp:wzTextBox id="WzCodNomeCom" runat="server" MaxLength="12" 
                                        onblur="toUpper(this);controlloCampoCodice(this);"
										CssStyles-CssRequired="textbox_stringa_obbligatorio w100p" CssStyles-CssEnabled="textbox_stringa  w100p"
										CssStyles-CssDisabled="textbox_stringa_disabilitato w100p" BindingField-Editable="onNew" BindingField-Connection="nomiComDatiConn"
										BindingField-SourceTable="T_ANA_NOMI_COMMERCIALI" BindingField-Hidden="False" BindingField-SourceField="NOC_CODICE"></ondp:wzTextBox></td>
								<td class="label">Cod. Esterno</td>
								<td>
									<ondp:wzTextBox id="WzCodExtNomeCom" runat="server" MaxLength="20" onblur="toUpper(this);"
										CssStyles-CssRequired="textbox_stringa_obbligatorio w100p" CssStyles-CssEnabled="textbox_stringa  w100p"
										CssStyles-CssDisabled="textbox_stringa_disabilitato w100p" BindingField-Editable="always" BindingField-Connection="nomiComDatiConn"
										BindingField-SourceTable="T_ANA_NOMI_COMMERCIALI" BindingField-Hidden="False" BindingField-SourceField="NOC_CODICE_ESTERNO"></ondp:wzTextBox></td>
								<td class="label">Descrizione</td>
								<td colspan="4">
									<ondp:wzTextBox id="WzDescNomeCom" runat="server" MaxLength="64" onblur="toUpper(this);"
										CssStyles-CssRequired="textbox_stringa_obbligatorio w100p" CssStyles-CssEnabled="textbox_stringa  w100p"
										CssStyles-CssDisabled="textbox_stringa_disabilitato w100p" BindingField-Editable="always" BindingField-Connection="nomiComDatiConn"
										BindingField-SourceTable="T_ANA_NOMI_COMMERCIALI" BindingField-Hidden="False" BindingField-SourceField="NOC_DESCRIZIONE"></ondp:wzTextBox></td>
							</tr>
							<tr>
								<td class="label">Sesso</td>
								<td>
									<ondp:wzDropDownList id="WzDropDownList1" runat="server" 
										CssStyles-CssRequired="textbox_data_obbligatorio  w100p" CssStyles-CssEnabled="textbox_data  w100p"
										CssStyles-CssDisabled="textbox_data_disabilitato w100p" BindingField-Editable="always" BindingField-Connection="nomiComDatiConn"
										BindingField-SourceTable="T_ANA_NOMI_COMMERCIALI" BindingField-Hidden="False" BindingField-SourceField="NOC_SESSO"
										TextFieldName="COD_DESCRIZIONE" KeyFieldName="COD_CODICE" SourceTable="T_ANA_CODIFICHE" DataFilter="COD_CAMPO='NOC_SESSO'"
										OtherListFields="COD_CAMPO"></ondp:wzDropDownList></td>
								<td class="label">Obsoleto</td>
								<td>
									<ondp:wzDropDownList id="Wzdropdownlist2" runat="server" 
										CssStyles-CssRequired="textbox_data_obbligatorio  w100p" CssStyles-CssEnabled="textbox_data  w100p"
										CssStyles-CssDisabled="textbox_data_disabilitato w100p" BindingField-Editable="always" BindingField-Connection="nomiComDatiConn"
										BindingField-SourceTable="T_ANA_NOMI_COMMERCIALI" BindingField-Hidden="False" BindingField-SourceField="NOC_OBSOLETO"
										TextFieldName="COD_DESCRIZIONE" KeyFieldName="COD_CODICE" SourceTable="T_ANA_CODIFICHE" DataFilter="COD_CAMPO='NOC_OBSOLETO'"
										OtherListFields="COD_CAMPO"></ondp:wzDropDownList></td>
								<td class="label">Fornitore</td>
								<td colspan="4">
									<ondp:wzFinestraModale id="fmFornitore" runat="server" Width="80%" Tabella="T_ANA_FORNITORI" CampoCodice="FOR_CODICE"
										CampoDescrizione="FOR_DESCRIZIONE" BindingDescription-SourceField="FOR_DESCRIZIONE" BindingDescription-SourceTable="T_ANA_FORNITORI"
										BindingDescription-Connection="VisiteDetail" BindingDescription-Hidden="False" BindingDescription-Editable="always"
										Obbligatorio="False" SetUpperCase="True" RaiseChangeEvent="False" UseCode="True" BindingCode-SourceField="NOC_FOR_CODICE"
										BindingCode-Hidden="False" BindingCode-SourceTable="T_ANA_NOMI_COMMERCIALI" BindingCode-Connection="VisiteDetail"
										BindingCode-Editable="always" CodiceWidth="20%" LabelWidth="-8px" PosizionamentoFacile="False" Filtro="1=1 order by FOR_DESCRIZIONE" UseTableLayout="true"></ondp:wzFinestraModale></td>
							</tr>
							<tr>
								<td class="label">Tipo Pagamento</td>
								<td colspan="3">
									<asp:DropDownList id="ddlTipPag"  Width="100%" runat="server" />
								</td>
								<td class="label">Costo Unitario €</td>
								<td>
									<ondp:wzOnitJsValidator ID="valCosto" runat="server" Width="100%" 
                                        actionCorrect="False" actionCustom="" actionDelete="False" actionFocus="True" 
                                        actionMessage="True" actionSelect="True" actionUndo="True" autoFormat="False" 
                                        CustomValFunction="validaNumero" validationType="Validate_custom" MaxLength="10"
                                        CssStyles-CssRequired="textbox_numerico_obbligatorio  w100p" CssStyles-CssEnabled="textbox_numerico w100p"
										CssStyles-CssDisabled="textbox_numerico_disabilitato w100p" BindingField-Editable="always" BindingField-Connection="nomiComDatiConn"
										BindingField-SourceTable="T_ANA_NOMI_COMMERCIALI" BindingField-Hidden="False" BindingField-SourceField="NOC_COSTO_UNITARIO">
										<Parameters>
                                            <on_val:ValidationParam paramName="numCifreIntere" paramOrder="0" 
                                                paramType="number" paramValue="7" />
                                            <on_val:ValidationParam paramName="numCifreDecimali" paramOrder="1" 
                                                paramType="number" paramValue="2" />
                                            <on_val:ValidationParam paramName="minValore" paramOrder="2" paramType="number" 
                                                paramValue="0" />
                                            <on_val:ValidationParam paramName="maxValore" paramOrder="3" paramType="number" 
                                                paramValue="9999999" />
                                            <on_val:ValidationParam paramName="blnCommaSeparator" paramOrder="4" 
                                                paramType="boolean" paramValue="true" />
                                        </Parameters>
                                    </ondp:wzOnitJsValidator>  
								</td>
                                <td colspan="3"></td>
							</tr>
							<tr>
								<td class="label">Inizio Validità</td>
								<td align="center" colSpan="3">
									<table id="Table3" cellspacing="0" cellpadding="0" width="250" border="0">
										<tr>
											<td>
												<asp:TextBox id="tbInizioAnni" runat="server" CssClass="textbox_numerico_disabilitato w40PX"
													ReadOnly="True"></asp:TextBox></td>
											<td align="right">
												<asp:Label id="Label14" style="PADDING-RIGHT: 10px; PADDING-LEFT: 3px" runat="server" CssClass="label">Anni</asp:Label></td>
											<td>
												<asp:TextBox id="tbInizioMesi" runat="server" CssClass="textbox_numerico_disabilitato w40PX"
													ReadOnly="True"></asp:TextBox></TD>
											<td align="right">
												<asp:Label id="Label15" style="PADDING-RIGHT: 10px; PADDING-LEFT: 3px" runat="server" CssClass="label">Mesi</asp:Label></td>
											<td>
												<asp:TextBox id="tbInizioGiorni" runat="server" CssClass="textbox_numerico_disabilitato w40PX"
													ReadOnly="True"></asp:TextBox>
												<ondp:wzTextBox id="wzTbEtaInizio"  runat="server" Width="40px" 
													CssStyles-CssRequired="textbox_numerico_obbligatorio w40PX" CssStyles-CssEnabled="textbox_numerico w40PX"
													BindingField-Editable="always" BindingField-Connection="nomiComDatiConn" BindingField-SourceTable="T_ANA_NOMI_COMMERCIALI"
													BindingField-Hidden="False" BindingField-SourceField="NOC_ETA_INIZIO" Visible="False"></ondp:wzTextBox></td>
											<td align="right" class="label">
												<asp:Label id="Label16" style="PADDING-RIGHT: 3px; PADDING-LEFT: 3px" runat="server" CssClass="label">Giorni</asp:Label></td>
										</tr>
									</table>
								</td>
								<td class="label">Fine Validità</td>
								<td align="center" colSpan="3">
									<table id="Table4" cellspacing="0" cellpadding="0" width="250" border="0">
										<tr>
											<td align="right" class="label">
												<asp:TextBox id="tbFineAnni" runat="server" CssClass="textbox_numerico_disabilitato w40PX" ReadOnly="True"></asp:TextBox></td>
											<td>
												<asp:Label id="Label11" style="PADDING-RIGHT: 10px; PADDING-LEFT: 3px" runat="server" CssClass="label">Anni</asp:Label></td>
											<td align="right">
												<asp:TextBox id="tbFineMesi" runat="server" CssClass="textbox_numerico_disabilitato w40PX" ReadOnly="True"></asp:TextBox></td>
											<td class="label">
												<asp:Label id="Label12" style="PADDING-RIGHT: 10px; PADDING-LEFT: 3px" runat="server" CssClass="label">Mesi</asp:Label></td>
											<td align="right">
												<asp:TextBox id="tbFineGiorni" runat="server" CssClass="textbox_numerico_disabilitato w40PX"
													ReadOnly="True"></asp:TextBox>
												<ondp:wzTextBox id="wzTbEtaFine" runat="server" Width="40px" 
													BindingField-Editable="always" BindingField-Connection="nomiComDatiConn" BindingField-SourceTable="T_ANA_NOMI_COMMERCIALI"
													BindingField-Hidden="False" BindingField-SourceField="NOC_ETA_FINE" Visible="False"></ondp:wzTextBox></td>
											<td class="label">
												<asp:Label id="Label13" style="PADDING-RIGHT: 3px; PADDING-LEFT: 3px" runat="server">Giorni</asp:Label></td>
										</tr>
									</table>
								</td>
								<td>&nbsp;</td>
							</tr>
                            <tr>
								<td class="label">
									<asp:Label id="lblSito" runat="server">Sito Inoculazione</asp:Label></td>
                                <td colspan="3">
									<ondp:wzFinestraModale id="fmSitoInoculazione" runat="server" Width="80%" Tabella="T_ANA_SITI_INOCULAZIONE" CampoCodice="SII_CODICE"
										CampoDescrizione="SII_DESCRIZIONE" BindingDescription-SourceField="SII_DESCRIZIONE" BindingDescription-SourceTable="T_ANA_SITI_INOCULAZIONE"
										BindingDescription-Connection="nomiComDatiConn" BindingDescription-Hidden="False" BindingDescription-Editable="always"
										Obbligatorio="False" SetUpperCase="True" RaiseChangeEvent="False" UseCode="True" BindingCode-SourceField="NOC_SII_CODICE"
										BindingCode-Hidden="False" BindingCode-SourceTable="T_ANA_NOMI_COMMERCIALI" BindingCode-Connection="nomiComDatiConn"
										BindingCode-Editable="always" CodiceWidth="20%" LabelWidth="-8px" PosizionamentoFacile="False" 
                                        Filtro="1=1 order by SII_DESCRIZIONE" UseTableLayout="true"></ondp:wzFinestraModale>
                                </td>
                                <td class="label">
                                    <asp:Label id="lblVia" runat="server">Via Somministr.</asp:Label></td>
                                <td colspan="4">
									<ondp:wzFinestraModale id="fmViaSomministrazione" runat="server" Width="80%" Tabella="T_ANA_VIE_SOMMINISTRAZIONE" CampoCodice="VII_CODICE"
										CampoDescrizione="VII_DESCRIZIONE" BindingDescription-SourceField="VII_DESCRIZIONE" BindingDescription-SourceTable="T_ANA_VIE_SOMMINISTRAZIONE"
										BindingDescription-Connection="nomiComDatiConn" BindingDescription-Hidden="False" BindingDescription-Editable="always"
										Obbligatorio="False" SetUpperCase="True" RaiseChangeEvent="False" UseCode="True" BindingCode-SourceField="NOC_VII_CODICE"
										BindingCode-Hidden="False" BindingCode-SourceTable="T_ANA_NOMI_COMMERCIALI" BindingCode-Connection="nomiComDatiConn"
										BindingCode-Editable="always" CodiceWidth="20%" LabelWidth="-8px" PosizionamentoFacile="False" 
                                        Filtro="1=1 order by VII_DESCRIZIONE" UseTableLayout="true"></ondp:wzFinestraModale></td>
                            </tr>
							<tr>
								<td class="label">Caratt. biologiche</td>
								<td colspan="3">
									<ondp:wzTextBox id="wzTbCaratteristicheBiologiche" style="overflow: hidden;" runat="server" MaxLength="256"
										CssStyles-CssRequired="textbox_stringa_obbligatorio w100p" CssStyles-CssEnabled="textbox_stringa  w100p"
										CssStyles-CssDisabled="textbox_stringa_disabilitato w100p" BindingField-Editable="always" BindingField-Connection="nomiComDatiConn"
										BindingField-SourceTable="T_ANA_NOMI_COMMERCIALI" BindingField-Hidden="False" BindingField-SourceField="NOC_CAR_BIOLOGICHE"
										TextMode="MultiLine" Rows="2" onKeyUp="Count(this)" onChange="Count(this)" ></ondp:wzTextBox></td>
								<td class="label">Conservanti</td>
								<td colspan="4">
									<ondp:wzTextBox id="wzTbConservanti" style="overflow: hidden;" runat="server" MaxLength="256" 
										CssStyles-CssRequired="textbox_stringa_obbligatorio w100p" CssStyles-CssEnabled="textbox_stringa  w100p"
										CssStyles-CssDisabled="textbox_stringa_disabilitato w100p" BindingField-Editable="always" BindingField-Connection="nomiComDatiConn"
										BindingField-SourceTable="T_ANA_NOMI_COMMERCIALI" BindingField-Hidden="False" BindingField-SourceField="NOC_CONSERVANTI"
										TextMode="MultiLine" Rows="2" onKeyUp="Count(this)" onChange="Count(this)" ></ondp:wzTextBox></td>
							</tr>
							<tr>
								<td class="label">Stabilizzanti</td>
								<td colspan="3">
									<ondp:wzTextBox id="wzTbStabilizzanti" style="overflow: hidden;" runat="server" MaxLength="256"
											CssStyles-CssRequired="textbox_stringa_obbligatorio w100p" CssStyles-CssEnabled="textbox_stringa  w100p"
										CssStyles-CssDisabled="textbox_stringa_disabilitato w100p" BindingField-Editable="always" BindingField-Connection="nomiComDatiConn"
										BindingField-SourceTable="T_ANA_NOMI_COMMERCIALI" BindingField-Hidden="False" BindingField-SourceField="NOC_STABILIZZANTI"
										TextMode="MultiLine" Rows="2" onKeyUp="Count(this)" onChange="Count(this)" ></ondp:wzTextBox></td>
								<td class="label">Antibiotici</td>
								<td colspan="4">
									<ondp:wzTextBox id="wzTbAntibiotici" style="overflow: hidden;" runat="server" MaxLength="256" 
										CssStyles-CssRequired="textbox_stringa_obbligatorio w100p" CssStyles-CssEnabled="textbox_stringa  w100p"
										CssStyles-CssDisabled="textbox_stringa_disabilitato w100p" BindingField-Editable="always" BindingField-Connection="nomiComDatiConn"
										BindingField-SourceTable="T_ANA_NOMI_COMMERCIALI" BindingField-Hidden="False" BindingField-SourceField="NOC_ANTIBIOTICI"
										TextMode="MultiLine" Rows="2" onKeyUp="Count(this)" onChange="Count(this)" ></ondp:wzTextBox></td>
							</tr>
							<tr>
								<td class="label">Adiuvanti</td>
								<td colspan="3">
									<ondp:wzTextBox id="wzTbAdiuvanti" style="overflow: hidden;" runat="server" MaxLength="256" 
										CssStyles-CssRequired="textbox_stringa_obbligatorio w100p" CssStyles-CssEnabled="textbox_stringa  w100p"
										CssStyles-CssDisabled="textbox_stringa_disabilitato w100p" BindingField-Editable="always" BindingField-Connection="nomiComDatiConn"
										BindingField-SourceTable="T_ANA_NOMI_COMMERCIALI" BindingField-Hidden="False" BindingField-SourceField="NOC_ADIUVANTI"
										TextMode="MultiLine" Rows="2" onKeyUp="Count(this)" onChange="Count(this)" ></ondp:wzTextBox></td>
                                <td class="label" >
                                    <table id="table" runat="server" width="100%" >
                                        <tr >
                                            <td class="Label" >Codice AIC</td>
                                        </tr>
                                        <tr >
                                            <td class="Label">Codice ATC</td>
                                        </tr>
                                    </table>
                                    
                                </td>
                                <td colspan="4">
                                    <table id="table1">
                                        <tr>
									<ondp:wzTextBox id="WzTextBox1" runat="server" MaxLength="9" onblur="toUpper(this);"
										CssStyles-CssRequired="textbox_stringa_obbligatorio w100p" CssStyles-CssEnabled="textbox_stringa  w100p"
										CssStyles-CssDisabled="textbox_stringa_disabilitato w100p" BindingField-Editable="always" BindingField-Connection="nomiComDatiConn"
										BindingField-SourceTable="T_ANA_NOMI_COMMERCIALI" BindingField-Hidden="False" BindingField-SourceField="NOC_CODICE_AIC"></ondp:wzTextBox></td>
                                        </tr>
                                        <tr>
									<ondp:wzTextBox id="WzTextBox2" runat="server" MaxLength="7" onblur="toUpper(this);"
										CssStyles-CssRequired="textbox_stringa_obbligatorio w100p" CssStyles-CssEnabled="textbox_stringa  w100p"
										CssStyles-CssDisabled="textbox_stringa_disabilitato w100p" BindingField-Editable="always" BindingField-Connection="nomiComDatiConn"
										BindingField-SourceTable="T_ANA_NOMI_COMMERCIALI" BindingField-Hidden="False" BindingField-SourceField="NOC_CODICE_ATC"></ondp:wzTextBox></td>
                                        </tr>
                                    </table>
                                </td>
                                
                                
							</tr>
						</table>
					</ondp:OnitDataPanel>
                </div>
			</ondp:onitdatapanel>
        </on_lay3:OnitLayout3>

        <on_ofm:OnitFinestraModale ID="fmCondizioniPagamento" Title="Condizioni di Pagamento" runat="server" Width="750px" BackColor="LightGray" NoRenderX="true" Height="500px">
            <uc1:ElencoCondizioniPagamento ID="ucElencoCondizioniPagamento" runat="server" />
        </on_ofm:OnitFinestraModale>
    </form>

</body>
</html>
