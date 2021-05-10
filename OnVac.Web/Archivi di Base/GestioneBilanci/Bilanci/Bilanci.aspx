<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Bilanci.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_Bilanci" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>Bilanci</title>
    
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

        .timetable {
            width: 90%;
            border: none;
        }
    </style>
    <script type="text/javascript">
        function toUpper(x) {
            x.value = x.value.toUpperCase()
        }

        function rollover(btn, stato) {

            if (btn.disabled == false) {
                if (stato == 'over') {
                    btn.src = btn.src.split(".png")[0] + "_over.png";
                }
                else
                    if (stato == 'out') {
                        btn.src = btn.src.split("_over.png")[0] + ".png";
                    }
            }
        }
    </script>
</head>
<body>
    <form id="Form2" method="post" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" TitleCssClass="title3" Titolo="Bilanci" Width="100%" Height="100%">
            <ondp:onitdatapanel id="OdpBilanciMaster" runat="server" usetoolbar="False" renderonlychildren="True" height="100%" width="100%" configfile="Bilanci.OdpBilanciMaster.xml">
                <div>
					<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBarBilanci" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
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
							<igtbar:TBSeparator></igtbar:TBSeparator>
							<igtbar:TBarButton Key="btnSezioni" Text="Sezioni" Image="../../../images/sezioni.gif"></igtbar:TBarButton>
							<igtbar:TBarButton Key="btnLinkOsservazioni" Text="Osservazioni" Image="../../../images/osservazioni.gif">
								<DefaultStyle Width="110px" CssClass="infratoolbar_button_default"></DefaultStyle>
							</igtbar:TBarButton>
						</Items>
					</igtbar:UltraWebToolbar>
                </div>
			    <div class="Sezione">Modulo ricerca</div>
                <div>
					<ondp:wzFilter id="WzFilter1" runat="server" Height="70px" Width="100%"  CssClass="InfraUltraWebTab2">
						<defaulttabstyle CssClass="InfraTab_Default2"></defaulttabstyle>
						<selectedtabstyle CssClass="InfraTab_Selected2"></selectedtabstyle>
						<disabledtabstyle CssClass="InfraTab_Disabled2"></disabledtabstyle>
						<hovertabstyle CssClass="InfraTab_Hover2"></hovertabstyle>
						<tabs>
							<igtab:tab Text="Ricerca di Base">
								<contenttemplate>
									<table height="100%" cellspacing="10" cellpadding="0" width="100%" border="0" style="table-layout:fixed">
										<tr>
											<td align="right" width="90">
												<asp:Label id="Label1" runat="server" CssClass="label">Filtro di Ricerca</asp:Label></td>
											<td>
												<asp:TextBox id="WzFilterKeyBase" runat="server" cssclass="textbox_stringa w100p"></asp:TextBox></td>
										</tr>
									</table>
								</contenttemplate>
							</igtab:tab>
						</tabs>
					</ondp:wzFilter>
                </div>
				<div class="Sezione">Elenco</div>
                
                <dyp:DynamicPanel ID="DynamicPanel1" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
					<ondp:wzDataGrid Browser="UpLevel" id="WzDgrBilanci" runat="server" Width="100%" PagerVoicesBefore="-1" PagerVoicesAfter="-1" OnitStyle="False" EditMode="None">
						<DisplayLayout ColWidthDefault="" AutoGenerateColumns="False" RowHeightDefault="20px" Version="2.00"
							GridLinesDefault="None" SelectTypeRowDefault="Single" ScrollBar="Never" BorderCollapseDefault="Separate"
							CellPaddingDefault="1" RowSelectorsDefault="No" Name="WzDgrBilanci" CellClickActionDefault="RowSelect">
							<HeaderStyleDefault CssClass="Infra2Dgr_Header"></HeaderStyleDefault>
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
									<igtbl:UltraGridColumn HeaderText="&lt;input type=checkbox  OnClick='javascript:wzDataGrid_Master_Click(this,&amp;quot;WzDgrBilanci&amp;quot;);' /&gt;"
										Key="check" Width="0%" AllowUpdate="Yes" Hidden="True" Type="CheckBox">
										<Footer Key="check"></Footer>
										<Header Key="check" Caption="&lt;input type=checkbox  OnClick='javascript:wzDataGrid_Master_Click(this,&amp;quot;WzDgrBilanci&amp;quot;);' /&gt;"></Header>
									</igtbl:UltraGridColumn>
									<igtbl:UltraGridColumn HeaderText="Numero" Width="70px">
										<CellStyle HorizontalAlign="Center"></CellStyle>
										<Footer Key="">
											<RowLayoutColumnInfo OriginX="1"></RowLayoutColumnInfo>
										</Footer>
										<Header Key="" Caption="Numero">
											<RowLayoutColumnInfo OriginX="1"></RowLayoutColumnInfo>
										</Header>
									</igtbl:UltraGridColumn>
									<igtbl:UltraGridColumn HeaderText="Descrizione" Width="24%">
										<CellStyle Wrap="True"></CellStyle>
										<Footer Key="">
											<RowLayoutColumnInfo OriginX="2"></RowLayoutColumnInfo>
										</Footer>
										<Header Key="" Caption="Descrizione">
											<RowLayoutColumnInfo OriginX="2"></RowLayoutColumnInfo>
										</Header>
									</igtbl:UltraGridColumn>
									<igtbl:UltraGridColumn HeaderText="Malattia" Width="23%">
                                        <CellStyle Wrap="True"></CellStyle>
										<Footer Key="">
											<RowLayoutColumnInfo OriginX="3"></RowLayoutColumnInfo>
										</Footer>
										<Header Key="" Caption="Malattia">
											<RowLayoutColumnInfo OriginX="3"></RowLayoutColumnInfo>
										</Header>
									</igtbl:UltraGridColumn>
									<igtbl:UltraGridColumn HeaderText="Et&#224; minima" Width="0%" Hidden="True">
										<Footer Key="">
											<RowLayoutColumnInfo OriginX="4"></RowLayoutColumnInfo>
										</Footer>
										<Header Key="" Caption="Et&#224; minima">
											<RowLayoutColumnInfo OriginX="4"></RowLayoutColumnInfo>
										</Header>
									</igtbl:UltraGridColumn>
									<igtbl:UltraGridColumn HeaderText="Et&#224; esecuzione" Width="0%" Hidden="True">
										<Footer Key="">
											<RowLayoutColumnInfo OriginX="5"></RowLayoutColumnInfo>
										</Footer>
										<Header Key="" Caption="Et&#224; esecuzione">
											<RowLayoutColumnInfo OriginX="5"></RowLayoutColumnInfo>
										</Header>
									</igtbl:UltraGridColumn>
									<igtbl:UltraGridColumn HeaderText="Et&#224; massima" Width="0%" Hidden="True">
										<Footer Key="">
											<RowLayoutColumnInfo OriginX="6"></RowLayoutColumnInfo>
										</Footer>
										<Header Key="" Caption="Et&#224; massima">
											<RowLayoutColumnInfo OriginX="6"></RowLayoutColumnInfo>
										</Header>
									</igtbl:UltraGridColumn>
									<igtbl:UltraGridColumn HeaderText="Bil. Succ." Width="0%" Hidden="True">
										<Footer Key="">
											<RowLayoutColumnInfo OriginX="7"></RowLayoutColumnInfo>
										</Footer>
										<Header Key="" Caption="Bil. Succ.">
											<RowLayoutColumnInfo OriginX="7"></RowLayoutColumnInfo>
										</Header>
									</igtbl:UltraGridColumn>
									<igtbl:UltraGridColumn HeaderText="Sollecito" Width="0%" Hidden="True">
										<Footer Key="">
											<RowLayoutColumnInfo OriginX="8"></RowLayoutColumnInfo>
										</Footer>
										<Header Key="" Caption="Sollecito">
											<RowLayoutColumnInfo OriginX="8"></RowLayoutColumnInfo>
										</Header>
									</igtbl:UltraGridColumn>
									<igtbl:UltraGridColumn HeaderText="Obbligatorio" Width="15%">
										<CellStyle HorizontalAlign="Center"></CellStyle>
										<Footer Key="">
											<RowLayoutColumnInfo OriginX="9"></RowLayoutColumnInfo>
										</Footer>
										<Header Key="" Caption="Obbligatorio">
											<RowLayoutColumnInfo OriginX="9"></RowLayoutColumnInfo>
										</Header>
									</igtbl:UltraGridColumn>
									<igtbl:UltraGridColumn HeaderText="Gest.&lt;br&gt; Cranio" Width="10%">
										<CellStyle HorizontalAlign="Center"></CellStyle>
										<Footer Key="">
											<RowLayoutColumnInfo OriginX="10"></RowLayoutColumnInfo>
										</Footer>
										<Header Key="" Caption="Gest.&lt;br&gt; Cranio">
											<RowLayoutColumnInfo OriginX="10"></RowLayoutColumnInfo>
										</Header>
									</igtbl:UltraGridColumn>
									<igtbl:UltraGridColumn HeaderText="Gest.&lt;br&gt; Altezza" Width="10%">
										<CellStyle HorizontalAlign="Center"></CellStyle>
										<Footer Key="">
											<RowLayoutColumnInfo OriginX="11"></RowLayoutColumnInfo>
										</Footer>
										<Header Key="" Caption="Gest.&lt;br&gt; Altezza">
											<RowLayoutColumnInfo OriginX="11"></RowLayoutColumnInfo>
										</Header>
									</igtbl:UltraGridColumn>
									<igtbl:UltraGridColumn HeaderText="Gest.&lt;br&gt; Peso" Width="10%">
										<CellStyle HorizontalAlign="Center"></CellStyle>
										<Footer Key="">
											<RowLayoutColumnInfo OriginX="12"></RowLayoutColumnInfo>
										</Footer>
										<Header Key="" Caption="Gest.&lt;br&gt; Peso">
											<RowLayoutColumnInfo OriginX="12"></RowLayoutColumnInfo>
										</Header>
									</igtbl:UltraGridColumn>
									<igtbl:UltraGridColumn HeaderText="Attivo" Width="10%">
										<CellStyle HorizontalAlign="Center"></CellStyle>
										<Footer Key="">
											<RowLayoutColumnInfo OriginX="13"></RowLayoutColumnInfo>
										</Footer>
										<Header Key="" Caption="Attivo">
											<RowLayoutColumnInfo OriginX="13"></RowLayoutColumnInfo>
										</Header>
									</igtbl:UltraGridColumn>
									<igtbl:TemplatedColumn Width="19%" Key="ETAMINIMA">
										<HeaderTemplate>
											<div><span>Età Minima</span></div>
											<table id="Table6" style="table-layout: fixed" cellspacing="0" cellpadding="0" width="120"
												border="0">
												<tr>
													<td class="Infra2Dgr_Header" width="33%">Anni</td>
													<td class="Infra2Dgr_Header" width="33%">Mesi</td>
													<td class="Infra2Dgr_Header" width="34%">Giorni</td>
												</tr>
											</table>
										</HeaderTemplate>
										<Footer Key="ETAMINIMA">
											<RowLayoutColumnInfo OriginX="14"></RowLayoutColumnInfo>
										</Footer>
										<Header Key="ETAMINIMA">
											<RowLayoutColumnInfo OriginX="14"></RowLayoutColumnInfo>
										</Header>
										<CellTemplate>
											<table id="Table9" style="table-layout: fixed" cellspacing="0" cellpadding="0" width="120" border="0">
												<tr>
													<td align="center">
														<asp:Label id="minAnni" runat="server" cssclass="label"></asp:Label></td>
													<td align="center">
														<asp:Label id="minMesi" runat="server" cssclass="label"></asp:Label></td>
													<td align="center">
														<asp:Label id="minGiorni" runat="server" cssclass="label"></asp:Label></td>
												</tr>
											</table>
										</CellTemplate>
									</igtbl:TemplatedColumn>
									<igtbl:TemplatedColumn Width="19%" Key="ETAMASSIMA">
										<HeaderTemplate>
											<div><span>Età Massima</span></div>
											<table id="Table7" style="table-layout: fixed" cellspacing="0" cellpadding="0" width="120"
												border="0">
												<tr>
													<td class="Infra2Dgr_Header" width="33%">Anni</td>
													<td class="Infra2Dgr_Header" width="33%">Mesi</td>
													<td class="Infra2Dgr_Header" width="34%">Giorni</td>
												</tr>
											</table>
										</HeaderTemplate>
										<Footer Key="ETAMASSIMA">
											<RowLayoutColumnInfo OriginX="15"></RowLayoutColumnInfo>
										</Footer>
										<Header Key="ETAMASSIMA">
											<RowLayoutColumnInfo OriginX="15"></RowLayoutColumnInfo>
										</Header>
										<CellTemplate>
											<table id="Table9" style="table-layout: fixed" cellspacing="0" cellpadding="0" width="120"
												border="0">
												<tr>
													<td align="center">
														<asp:Label id="maxAnni" runat="server" cssclass="label"></asp:Label></td>
													<td align="center">
														<asp:Label id="maxMesi" runat="server" cssclass="label"></asp:Label></td>
													<td align="center">
														<asp:Label id="maxGiorni" runat="server" cssclass="label"></asp:Label></td>
												</tr>
											</table>
										</CellTemplate>
									</igtbl:TemplatedColumn>
								</Columns>
							</igtbl:UltraGridBand>
						</Bands>
						<BindingColumns>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Numero" Connection="vacConn" SourceTable="T_ANA_BILANCI"
								Hidden="False" SourceField="BIL_NUMERO"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Descrizione" Connection="ConnessioneMaster"
								SourceTable="T_ANA_BILANCI" Hidden="False" SourceField="BIL_DESCRIZIONE"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Malattia" Connection="ConnessioneMaster"
								SourceTable="T_ANA_MALATTIE" Hidden="False" SourceField="MAL_DESCRIZIONE"></ondp:BindingFieldValue>
                            <ondp:BindingFieldValue Value="" Editable="always" Description="Obbligatorio" Connection="ConnessioneMaster"
								SourceTable="T_ANA_BILANCI" Hidden="False" SourceField="BIL_OBBLIGATORIO"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Et&#224; minima" Connection="vacConn" SourceTable="T_ANA_BILANCI"
								Hidden="True" SourceField="BIL_ETA_MINIMA"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Et&#224; massima" Connection="vacConn" SourceTable="T_ANA_BILANCI"
								Hidden="True" SourceField="BIL_ETA_MASSIMA"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Bil_Succ" Connection="vacConn" SourceTable="T_ANA_BILANCI"
								Hidden="True" SourceField="BIL_INTERVALLO"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Sollecito" Connection="vacConn" SourceTable="T_ANA_BILANCI"
								Hidden="True" SourceField="BIL_GIORNI_SOLLECITO"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Obbligatoriet&#224;" Connection="ConnessioneMaster"
								SourceTable="T_ANA_CODIFICHE" Hidden="False" SourceField="COD_DESCRIZIONE"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Gest. Cranio" Connection="ConnessioneMaster"
								SourceTable="T_ANA_BILANCI" Hidden="False" SourceField="BIL_CRANIO"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Gest. Altezza" Connection="ConnessioneMaster"
								SourceTable="T_ANA_BILANCI" Hidden="False" SourceField="BIL_ALTEZZA"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Gest. Peso" Connection="ConnessioneMaster"
								SourceTable="T_ANA_BILANCI" Hidden="False" SourceField="BIL_PESO"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Attivo" Connection="ConnessioneMaster"
								SourceTable="T_ANA_BILANCI" Hidden="False" SourceField="BIL_ABILITATO"></ondp:BindingFieldValue>
						</BindingColumns>
					</ondp:wzDataGrid>
                </dyp:DynamicPanel>
                
                <div class="Sezione">Dettaglio</div>
                <div>
					<ondp:OnitDataPanel id="OdpBilanciDetail" runat="server" useToolbar="False" Width="100%" ConfigFile="Bilanci.OdpBilanciDetail.xml"
						dontLoadDataFirstTime="True" externalToolBar-Length="14" BindingFields="(Insieme)">
						<table id="Table" style="table-layout:fixed" cellspacing="3" cellpadding="0" width="100%" border="0">
							<colgroup>
								<col width="10%" />
								<col width="16%" />
								<col width="8%" />
								<col width="8%" />
								<col width="23%"/>
								<col width="10%" />
								<col width="25%"/>
							</colgroup>
							<tr>
								<td class="label">Numero</td>
								<td>
									<ondp:wzTextBox id="WzTxtNumero" runat="server" CssStyles-CssRequired="textbox_numerico_obbligatorio w40px"
										CssStyles-CssEnabled="textbox_numerico w40px" CssStyles-CssDisabled="textbox_numerico_disabilitato w40px"
										BindingField-SourceField="BIL_NUMERO" BindingField-Hidden="False"
										BindingField-SourceTable="T_ANA_BILANCI" BindingField-Connection="ConnessioneDetail" BindingField-Editable="onNew"
										MaxLength="2"></ondp:wzTextBox></td>
								<td class="label">Malattia</td>
								<td colspan="4">
									<ondp:wzFinestraModale id="WzTxtMalattie" runat="server" Width="70%" CampoDescrizione="MAL_DESCRIZIONE"
										CampoCodice="MAL_CODICE" Obbligatorio="False" SetUpperCase="True" RaiseChangeEvent="False" PosizionamentoFacile="False"
										LabelWidth="-2px" CodiceWidth="30%" Tabella="T_ANA_MALATTIE" BindingDescription-Editable="always" BindingDescription-SourceTable="T_ANA_MALATTIE"
										BindingDescription-Hidden="False" BindingDescription-SourceField="MAL_DESCRIZIONE" BindingCode-Editable="always"
										BindingCode-SourceTable="T_ANA_BILANCI" BindingCode-Hidden="False" BindingCode-SourceField="BIL_MAL_CODICE"
										BindingDescription-Connection="ConnessioneDetail" BindingCode-Connection="ConnessioneDetail" UseCode="True" UseTableLayout="True" Filtro=" MAL_OBSOLETO ='N' " ></ondp:wzFinestraModale></td>
							</tr>
							<tr>
								<td class="label">Consegna</td>
								<td>
									<ondp:wzDropDownList id="WzDropDownList1" runat="server" width="100%" CssStyles-CssRequired="TextBox_Stringa" CssStyles-CssEnabled="TextBox_Stringa" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato" BindingField-SourceField="BIL_CONSEGNATO_A" BindingField-Hidden="False" BindingField-SourceTable="T_ANA_BILANCI" BindingField-Connection="ConnessioneDetail" BindingField-Editable="always" OtherListFields="COD_CAMPO" DataFilter="COD_CAMPO='BIL_CONSEGNATO_A'" KeyFieldName="COD_CODICE" IncludeNull="True" TextFieldName="COD_DESCRIZIONE" SourceTable="T_ANA_CODIFICHE"></ondp:wzDropDownList>
                                </td>
                                <td class="label">Descrizione</td>
								<td colspan="4">
									<ondp:wzTextBox id="Wztextbox1" onblur="toUpper(this);" runat="server"
										Width="100%" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio" CssStyles-CssEnabled="TextBox_Stringa"
										CssStyles-CssDisabled="TextBox_Stringa_Disabilitato" BindingField-SourceField="BIL_DESCRIZIONE"
										BindingField-Hidden="False" BindingField-SourceTable="T_ANA_BILANCI" BindingField-Connection="ConnessioneDetail"
										BindingField-Editable="always"></ondp:wzTextBox>
                                </td>
							</tr>
							<tr>
								<td class="label">Età Minima</td>
								<td colspan="2">
									<table id="Table2" class="timetable" cellspacing="0" cellpadding="0">
										<tr>
											<td align="right" width="16%">
												<asp:TextBox id="tbMinAnni" runat="server" Width="100%" CssClass="textbox_numerico_disabilitato"
													ReadOnly="True"></asp:TextBox></td>
											<td class="label_left" width="17%">
												<asp:Label id="Label10" runat="server" Width="100%" CssClass="label_left">Anni</asp:Label></td>
											<td align="right" width="16%">
												<asp:TextBox id="tbMinMesi" runat="server" Width="100%" CssClass="textbox_numerico_disabilitato"
													ReadOnly="True"></asp:TextBox></td>
											<td class="label_left" width="17%">
												<asp:Label id="Label9" runat="server" Width="100%" CssClass="label_left">Mesi</asp:Label></td>
											<td width="16%">
												<asp:TextBox id="tbMinGiorni" runat="server" Width="100%" CssClass="textbox_numerico_disabilitato w40PX"
													ReadOnly="True"></asp:TextBox>
												<ondp:wzTextBox id="tbMinEta" runat="server" BindingField-SourceField="BIL_ETA_MINIMA" BindingField-Hidden="False" BindingField-SourceTable="T_ANA_BILANCI" BindingField-Connection="ConnessioneDetail" BindingField-Editable="always" MaxLength="5" Visible="False"></ondp:wzTextBox>
                                            </td>
											<td class="label_left" width="18%">
												<asp:Label id="Label8" runat="server" Width="100%" CssClass="label_left">Giorni</asp:Label></td>
										</tr>
									</table>
								</td>
								<td class="label">Tempo CNV precedente</td>
								<td>
									<table class="timetable" cellspacing="0" cellpadding="0">
										<tr>
											<td align="right" width="16%">
												<asp:TextBox id="tbTempoCnvPrecAnni" runat="server" Width="100%" CssClass="textbox_numerico_disabilitato"
													ReadOnly="True"></asp:TextBox></td>
											<td class="label_left" width="17%">
												<asp:Label id="lblTempoCnvPrecAnni" runat="server" Width="100%" CssClass="label_left">Anni</asp:Label></td>
											<td align="right" width="16%">
												<asp:TextBox id="tbTempoCnvPrecMesi" runat="server" Width="100%" CssClass="textbox_numerico_disabilitato"
													ReadOnly="True"></asp:TextBox></td>
											<td class="label_left" width="17%">
												<asp:Label id="lblTempoCnvPrecMesi" runat="server" Width="100%" CssClass="label_left">Mesi</asp:Label></td>
											<td align="right" width="16%">
												<asp:TextBox id="tbTempoCnvPrecGiorni" runat="server" Width="100%" CssClass="textbox_numerico_disabilitato"
													ReadOnly="True"></asp:TextBox>
												<ondp:wzTextBox id="wzTbTempoCnvPrec" runat="server" BindingField-SourceField="BIL_TEMPO_CNV_PRECEDENTE" BindingField-Hidden="False" BindingField-SourceTable="T_ANA_BILANCI"
													BindingField-Connection="ConnessioneDetail" BindingField-Editable="always" MaxLength="5" Visible="False"></ondp:wzTextBox></td>
											<td width="18%">
												<asp:Label id="lblTempoCnvPrecGiorni" runat="server" Width="100%" CssClass="label_left">Giorni</asp:Label></td>
										</tr>
									</table>
								</td>
								<td class="label">Scadenza Dopo</td>
								<td>
									<table class="timetable" cellspacing="0" cellpadding="0">
										<tr>
											<td align="right" width="16%">
												<asp:TextBox id="tbScadDopoAnni" runat="server" Width="100%" CssClass="textbox_numerico_disabilitato"
													ReadOnly="True"></asp:TextBox></td>
											<td class="label_left" width="17%">
												<asp:Label id="lblScadDopoAnni" runat="server" Width="100%" CssClass="label_left">Anni</asp:Label></td>
											<td align="right" width="16%">
												<asp:TextBox id="tbScadDopoMesi" runat="server" Width="100%" CssClass="textbox_numerico_disabilitato"
													ReadOnly="True"></asp:TextBox></td>
											<td class="label_left" width="17%">
												<asp:Label id="lblScadDopoMesi" runat="server" Width="100%" CssClass="label_left">Mesi</asp:Label></td>
											<td align="right" width="16%">
												<asp:TextBox id="tbScadDopoGiorni" runat="server" Width="100%" CssClass="textbox_numerico_disabilitato"
													ReadOnly="True"></asp:TextBox>
												<ondp:wzTextBox id="wzTbScadDopo" runat="server" BindingField-SourceField="BIL_SCADENZA_DOPO" BindingField-Hidden="False" BindingField-SourceTable="T_ANA_BILANCI" BindingField-Connection="ConnessioneDetail" BindingField-Editable="always" MaxLength="5" Visible="False"></ondp:wzTextBox></td>
											<td width="18%">
												<asp:Label id="lblScadDopoGiorni" runat="server" Width="100%" CssClass="label_left">Giorni</asp:Label></td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td class="label">Età Massima</td>
								<td colspan="2">
									<table id="Table3" class="timetable" cellspacing="0" cellpadding="0">
										<tr>
											<td align="right" width="16%">
												<asp:TextBox id="tbMaxAnni" runat="server" Width="100%" CssClass="textbox_numerico_disabilitato"
													ReadOnly="True"></asp:TextBox></td>
											<td class="label_left" width="17%">
												<asp:Label id="Label16" runat="server" Width="100%" CssClass="label_left">Anni</asp:Label></td>
											<td align="right" width="16%">
												<asp:TextBox id="tbMaxMesi" runat="server" Width="100%" CssClass="textbox_numerico_disabilitato"
													ReadOnly="True"></asp:TextBox></td>
											<td class="label_left" width="17%">
												<asp:Label id="Label15" runat="server" Width="100%" CssClass="label_left">Mesi</asp:Label></td>
											<td align="right" width="16%">
												<asp:TextBox id="tbMaxGiorni" runat="server" Width="100%" CssClass="textbox_numerico_disabilitato"
													ReadOnly="True"></asp:TextBox>
												<ondp:wzTextBox id="tbMaxEta" runat="server" BindingField-SourceField="BIL_ETA_MASSIMA" BindingField-Hidden="False" BindingField-SourceTable="T_ANA_BILANCI"
													BindingField-Connection="ConnessioneDetail" BindingField-Editable="always" MaxLength="5" Visible="False"></ondp:wzTextBox></td>
											<td class="label_left" width="18%">
												<asp:Label id="Label14" runat="server" Width="100%" CssClass="label_left">Giorni</asp:Label></td>
										</tr>
									</table>
								</td>
								<td class="label">Obbligatorio</td>
								<td>
									<ondp:wzDropDownList id="WzDdlObbligatorio" runat="server" Width="100%" 
                                        CssStyles-CssRequired="textbox_stringa_obbligatorio  w40px" CssStyles-CssEnabled="textbox_stringa w40px" 
                                        CssStyles-CssDisabled="textbox_stringa_disabilitato w40px" BindingField-SourceField="BIL_OBBLIGATORIO" 
                                        BindingField-Hidden="False" BindingField-SourceTable="T_ANA_BILANCI" BindingField-Connection="ConnessioneDetail" 
                                        BindingField-Editable="always" OtherListFields="cod_campo" DataFilter="cod_campo='BIL_OBBLIGATORIO'" 
                                        KeyFieldName="COD_CODICE" TextFieldName="COD_DESCRIZIONE" SourceTable="T_ANA_CODIFICHE"></ondp:wzDropDownList>
								</td>
								<td class="label">Gest. Cranio</td>
								<td>
									<table class="timetable" cellspacing="0" cellpadding="0">
										<tr>
											<td width="3%">
												<ondp:wzCheckBox id="chkCranio" runat="server" Height="12px" CssStyles-CssEnabled="Label"
													CssStyles-CssDisabled="Label_Disabilitato" BindingField-SourceField="BIL_CRANIO"
													BindingField-Hidden="False" BindingField-SourceTable="T_ANA_BILANCI" BindingField-Connection="ConnessioneDetail"
													BindingField-Editable="always" BindingField-Value="N"></ondp:wzCheckBox></td>
											<td align="right" width="32%">
												<asp:Label id="Label19" runat="server" CssClass="label">Gest. Altezza</asp:Label></td>
											<td width="3%">
												<ondp:wzCheckBox id="chkAltezza" runat="server" Height="12px" CssStyles-CssEnabled="Label"
													CssStyles-CssDisabled="Label_Disabilitato" BindingField-SourceField="BIL_ALTEZZA"
													BindingField-Hidden="False" BindingField-SourceTable="T_ANA_BILANCI" BindingField-Connection="ConnessioneDetail"
													BindingField-Editable="always" BindingField-Value="N"></ondp:wzCheckBox></td>
											<td align="right" width="26%">
												<asp:Label id="Label20" runat="server" CssClass="label">Gest. Peso</asp:Label></td>
											<td width="3%">
												<ondp:wzCheckBox id="chkPeso" runat="server" Height="12px" CssStyles-CssEnabled="Label"
													CssStyles-CssDisabled="Label_Disabilitato" BindingField-SourceField="BIL_PESO"
													BindingField-Hidden="False" BindingField-SourceTable="T_ANA_BILANCI" BindingField-Connection="ConnessioneDetail"
													BindingField-Editable="always" BindingField-Value="N"></ondp:wzCheckBox></td>
											<td align="right" width="27%">
												<asp:Label id="Label2" runat="server" CssClass="label">Attivo</asp:Label></td>
											<td width="3%">
												<ondp:wzCheckBox id="chkAbilitato" runat="server" Height="12px" CssStyles-CssEnabled="Label"
													CssStyles-CssDisabled="Label_Disabilitato" BindingField-SourceField="BIL_ABILITATO"
													BindingField-Hidden="False" BindingField-SourceTable="T_ANA_BILANCI" BindingField-Connection="ConnessioneDetail"
													BindingField-Editable="always" BindingField-Value="N"></ondp:wzCheckBox></td>
											<td width="3%"></td>
										</tr>
									</table>
								</td>
							</tr>
							<tr vAlign="middle">
								<td class="label">Tempo dalla visita precedente</td>
								<td colspan="2">
									<table class="timetable" cellspacing="0" cellpadding="0">
										<tr>
											<td align="right" width="16%">
												<asp:TextBox id="tbBilSuccAnni" runat="server" Width="100%" CssClass="textbox_numerico_disabilitato"
													ReadOnly="True"></asp:TextBox></td>
											<td class="label_left" width="17%">
												<asp:Label id="lblBilSuccAnni" runat="server" Width="100%" CssClass="label_left">Anni</asp:Label></td>
											<td align="right" width="16%">
												<asp:TextBox id="tbBilSuccMesi" runat="server" Width="100%" CssClass="textbox_numerico_disabilitato"
													ReadOnly="True"></asp:TextBox></td>
											<td class="label_left" width="17%">
												<asp:Label id="lblBilSuccMesi" runat="server" Width="100%" CssClass="label_left">Mesi</asp:Label></td>
											<td align="right" width="16%">
												<asp:TextBox id="tbBilSuccGiorni" runat="server" Width="100%" CssClass="textbox_numerico_disabilitato"
													ReadOnly="True"></asp:TextBox>
												<ondp:wzTextBox id="wzTbBilSucc" runat="server"
													BindingField-SourceField="BIL_INTERVALLO" BindingField-Hidden="False" BindingField-SourceTable="T_ANA_BILANCI"
													BindingField-Connection="ConnessioneDetail" BindingField-Editable="always" MaxLength="5" Visible="False"></ondp:wzTextBox></td>
											<td width="18%">
												<asp:Label id="lblBilSuccGiorni" runat="server" Width="100%" CssClass="label_left">Giorni</asp:Label></td>
										</tr>
									</table>
								</td>
								<td class="label">Sollecito tra</td>
								<td>
									<table class="timetable" cellspacing="0" cellpadding="0">
										<tr>
											<td align="right" width="16%">
												<asp:TextBox id="tbTempoSollecitoAnni" runat="server" Width="100%" CssClass="textbox_numerico_disabilitato"
													ReadOnly="True"></asp:TextBox></td>
											<td class="label_left" width="17%">
												<asp:Label id="lblTempoSollecitoAnni" runat="server" Width="100%" CssClass="label_left">Anni</asp:Label></td>
											<td align="right" width="16%">
												<asp:TextBox id="tbTempoSollecitoMesi" runat="server" Width="100%" CssClass="textbox_numerico_disabilitato"
													ReadOnly="True"></asp:TextBox></td>
											<td class="label_left" width="17%">
												<asp:Label id="lblTempoSollecitoMesi" runat="server" Width="100%" CssClass="label_left">Mesi</asp:Label></td>
											<td align="right" width="16%">
												<asp:TextBox id="tbTempoSollecitoGiorni" runat="server" Width="100%" CssClass="textbox_numerico_disabilitato"
													ReadOnly="True"></asp:TextBox>
												<ondp:wzTextBox id="wzTbTempoSollecito" runat="server"
													BindingField-SourceField="BIL_GIORNI_SOLLECITO" BindingField-Hidden="False" BindingField-SourceTable="T_ANA_BILANCI"
													BindingField-Connection="ConnessioneDetail" BindingField-Editable="always" MaxLength="5" Visible="False"></ondp:wzTextBox></td>
											<td width="18%">
												<asp:Label id="lblTempoSollecitoGiorni" runat="server" Width="100%" CssClass="label_left">Giorni</asp:Label></td>
										</tr>
									</table>
								</td>
								<td class="label">N° solleciti</td>
								<td>
                                    <table class="timetable" cellspacing="0" cellpadding="0">
										<tr>
											<td width="19%">
										        <ondp:wzTextBox id="WzNumeroSolleciti" runat="server" MaxLength="2" BindingField-Editable="always" BindingField-Connection="ConnessioneDetail" 
                                                    BindingField-SourceTable="T_ANA_BILANCI" BindingField-Hidden="False" BindingField-SourceField="BIL_NUM_SOLLECITI" 
                                                    CssStyles-CssDisabled="textbox_numerico_disabilitato" CssStyles-CssEnabled="textbox_numerico" CssStyles-CssRequired="textbox_numerico_obbligatorio"
                                                    width="100%"></ondp:wzTextBox></td>
											<td align="right" width="49%">
												<asp:Label id="Label3" runat="server" CssClass="label">Vaccinazioni</asp:Label></td>
											<td width="3%">
												<ondp:wzCheckBox id="chkVaccinazioni" runat="server" Height="12px" CssStyles-CssEnabled="Label"
													CssStyles-CssDisabled="Label_Disabilitato" BindingField-SourceField="BIL_RILEVAZIONE_VACCINAZIONI"
													BindingField-Hidden="False" BindingField-SourceTable="T_ANA_BILANCI" BindingField-Connection="ConnessioneDetail"
													BindingField-Editable="always" BindingField-Value="N"></ondp:wzCheckBox></td>
											<td align="right" width="24%">
												<asp:Label id="Label5" runat="server" CssClass="label">Viaggi</asp:Label></td>
											<td width="2%">
												<ondp:wzCheckBox id="chkViaggi" runat="server" Height="12px" CssStyles-CssEnabled="Label"
													CssStyles-CssDisabled="Label_Disabilitato" BindingField-SourceField="BIL_RILEVAZIONE_VIAGGI"
													BindingField-Hidden="False" BindingField-SourceTable="T_ANA_BILANCI" BindingField-Connection="ConnessioneDetail"
													BindingField-Editable="always" BindingField-Value="N"></ondp:wzCheckBox></td>
											<td width="3%"></td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td class="label">Note</td>
								<td colspan="4">
									<ondp:wzTextBox id="txtNote" runat="server" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
										CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
										BindingField-SourceField="BIL_NOTE" BindingField-Hidden="False"
										BindingField-SourceTable="T_ANA_BILANCI" BindingField-Connection="ConnessioneDetail" BindingField-Editable="always"
										Rows="2" TextMode="MultiLine"></ondp:wzTextBox></td>
                                <td class="label">Nome report</td>
                                <td>
                                    <ondp:wzTextBox id="txtReport" runat="server" MaxLength="250" BindingField-Editable="always" BindingField-Connection="ConnessioneDetail" 
                                        BindingField-SourceTable="T_ANA_BILANCI" BindingField-Hidden="False" BindingField-SourceField="BIL_RPT_NOME" 
                                        CssStyles-CssDisabled="textbox_stringa_disabilitato w100p" CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p">
									</ondp:wzTextBox>
                                </td>
							</tr>
						</table>
					</ondp:OnitDataPanel>
                </div>
		    </ondp:onitdatapanel>
        </on_lay3:OnitLayout3>

        <onitcontrols:OnitFinestraModale id="fmSezioni" title="Sezioni" runat="server" height="300px" width="600px" BackColor="LightGray">
	        
            <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="tlbSezioni" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		        <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
		        <Items>
			        <igtbar:TBarButton Key="btnNew" Text="Nuovo" Image="~/Images/Nuovo.gif"></igtbar:TBarButton>
			        <igtbar:TBarButton Key="btnElimina" Text="Elimina" Image="~/Images/Elimina.gif"></igtbar:TBarButton>
			        <igtbar:TBSeparator></igtbar:TBSeparator>
			        <igtbar:TBarButton Key="btnSalva" Text="Salva" Image="~/Images/Salva.gif"></igtbar:TBarButton>
			        <igtbar:TBarButton Key="btnAnnulla" Text="Annulla" Image="~/Images/Annulla.gif"></igtbar:TBarButton>
		        </Items>
	        </igtbar:UltraWebToolbar>

            <asp:HiddenField ID="hidNumeroBilancioSelezionato" runat="server" />
            <asp:HiddenField ID="hidCodiceMalattiaSelezionata" runat="server" />

            <dyp:DynamicPanel ID="DynamicPanel2" runat="server" Width="100%" Height="280px" ScrollBars="Auto">
                <asp:DataGrid id="dgrSezioni" runat="server" Width="100%" CssClass="DataGrid" AutoGenerateColumns="False">
                    <HeaderStyle CssClass="Header"></HeaderStyle>
                    <ItemStyle CssClass="Item"></ItemStyle>
                    <AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
                    <SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
                    <EditItemStyle CssClass="Edit"></EditItemStyle>
                    <Columns>
                        <asp:TemplateColumn>
                            <HeaderStyle Width="5%" HorizontalAlign="Center"></HeaderStyle>
                            <ItemTemplate>
                                <asp:CheckBox id="chkSelezione" runat="server"></asp:CheckBox>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn>
                            <HeaderStyle Width="5%" HorizontalAlign="Center"></HeaderStyle>
                            <ItemTemplate>
                                <asp:ImageButton ID="btnUp" runat="server" ImageUrl="../../../images/arrow_blue12_up.png" CommandName="MoveUp" ToolTip="Sposta sopra"
                                    onmouseover="rollover(this, 'over');" onmouseout="rollover(this, 'out');"></asp:ImageButton>
                                <br />
                                <asp:ImageButton ID="btnDown" runat="server" ImageUrl="../../../images/arrow_blue12_down.png" CommandName="MoveDown" ToolTip="Sposta sotto"
                                    onmouseover="rollover(this, 'over');" onmouseout="rollover(this, 'out');"></asp:ImageButton>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn>
                            <HeaderStyle Width="90%"></HeaderStyle>
                            <ItemTemplate>
                                <asp:TextBox id="txtDescrizione" runat="server" Width="99%" Text='<%# Eval("DescrizioneSezione") %>' CssClass="TextBox_Stringa"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="CodiceSezione" Visible="false"></asp:BoundColumn>
                    </Columns>
                </asp:DataGrid>
            </dyp:DynamicPanel>

        </onitcontrols:OnitFinestraModale>

    </form>
</body>
</html>
