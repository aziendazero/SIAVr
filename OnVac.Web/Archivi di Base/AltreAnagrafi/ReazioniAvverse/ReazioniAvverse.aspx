<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ReazioniAvverse.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_ReazioniAvverse" %>

<%@ Register TagPrefix="on_otb" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitTable" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
		<title>Reazioni Avverse</title>

        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
		
        <style type="text/css">
		    .w100 { 
                width: 100px 
		    }
		</style>

        <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("onvac.common.js")%>'></script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" width="100%" height="100%" Titolo="Reazioni Avverse" TitleCssClass="Title3">
				<ondp:OnitDataPanel id="odpReazioniAvverseMaster" runat="server" renderOnlyChildren="True" ConfigFile="ReazioniAvverse.odpReazioniAvverseMaster.xml" useToolbar="False">
					<on_otb:onittable id="OnitTable1" runat="server" height="100%" width="100%">
						<on_otb:onitsection id="secRicerca" runat="server" width="100%" typeHeight="content">
							<on_otb:onitcell id="celRicerca" runat="server" height="100%" width="100%">
								
                                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="toolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		                            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
									<items>
										<igtbar:tbarbutton Text="Cerca" Key="btnCerca"></igtbar:tbarbutton>
										<igtbar:tbseparator></igtbar:tbseparator>
										<igtbar:tbarbutton Text="Nuovo" Key="btnNew"></igtbar:tbarbutton>
										<igtbar:tbarbutton Text="Modifica" Key="btnEdit"></igtbar:tbarbutton>
										<igtbar:tbarbutton Text="Elimina" Key="btnElimina"></igtbar:tbarbutton>
										<igtbar:tbseparator></igtbar:tbseparator>
										<igtbar:tbarbutton Text="Salva" Key="btnSalva"></igtbar:tbarbutton>
										<igtbar:tbarbutton Text="Annulla" Key="btnAnnulla"></igtbar:tbarbutton>
									</items>
								</igtbar:UltraWebToolbar>

								<div class="Sezione">Ricerca reazioni avverse</div>

								<ondp:wzFilter id="WzFilter1" runat="server" Height="70px" CssClass="InfraUltraWebTab2" Width="100%">
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

								<div class="Sezione">Elenco reazioni avverse</div>

							</on_otb:onitcell>
						</on_otb:onitsection>

						<on_otb:onitsection id="secElenco" runat="server" width="100%" typeHeight="calculate">
							<on_otb:onitcell id="celElenco" runat="server" height="100%" width="100%" typescroll="auto">
								<ondp:wzDataGrid Browser="UpLevel" id="WzDataGrid1" runat="server" Width="100%" OnitStyle="False" EditMode="None" disableActiveRowChange="False">
									<DisplayLayout ColWidthDefault="" AutoGenerateColumns="False" RowHeightDefault="26px" Version="2.00" GridLinesDefault="None" SelectTypeRowDefault="Single" ScrollBar="Never" BorderCollapseDefault="Separate" CellPaddingDefault="3" RowSelectorsDefault="No" Name="WzDataGrid1" CellClickActionDefault="RowSelect">
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
												<igtbl:UltraGridColumn HeaderText="&lt;input type=checkbox  OnClick='javascript:wzDataGrid_Master_Click(this,&amp;quot;WzDataGrid1&amp;quot;);' /&gt;" Key="check" Hidden="True" Type="CheckBox" BaseColumnName="" AllowUpdate="Yes"></igtbl:UltraGridColumn>
												<igtbl:UltraGridColumn HeaderText="Codice" Key="" Width="15%" BaseColumnName=""></igtbl:UltraGridColumn>
												<igtbl:UltraGridColumn HeaderText="Descrizione" Key="" Width="90%" BaseColumnName=""></igtbl:UltraGridColumn>
												<igtbl:UltraGridColumn HeaderText="Locale" Key="" Width="150px" BaseColumnName="">
													<CellButtonStyle HorizontalAlign="Center"></CellButtonStyle>
													<CellStyle HorizontalAlign="Center"></CellStyle>
												</igtbl:UltraGridColumn>
											</Columns>
										</igtbl:UltraGridBand>
									</Bands>
									<BindingColumns>
										<ondp:BindingFieldValue Value="" Editable="always" Description="Codice" Connection="ConnessioneMaster" SourceTable="T_ANA_REAZIONI_AVVERSE" Hidden="False" SourceField="REA_CODICE"></ondp:BindingFieldValue>
										<ondp:BindingFieldValue Value="" Editable="always" Description="Descrizione" Connection="ConnessioneMaster" SourceTable="T_ANA_REAZIONI_AVVERSE" Hidden="False" SourceField="REA_DESCRIZIONE"></ondp:BindingFieldValue>
										<ondp:BindingFieldValue Value="" Editable="always" Description="Locale" Connection="ConnessioneMaster" SourceTable="T_ANA_CODIFICHE" Hidden="False" SourceField="COD_DESCRIZIONE"></ondp:BindingFieldValue>
									</BindingColumns>
								</ondp:wzDataGrid>
							</on_otb:onitcell>
						</on_otb:onitsection>

						<on_otb:onitsection id="secDettagli" runat="server" width="100%" typeHeight="Content">
							<on_otb:onitcell id="celDettagli" runat="server" height="100%" width="100%" typescroll="Hidden">
								
                                <div class="Sezione">Dettaglio</div>

								<ondp:OnitDataPanel id="odpReazioniAvverseDetail" runat="server" renderOnlyChildren="True" ConfigFile="ReazioniAvverse.odpReazioniAvverseDetail.xml" 
                                    useToolbar="False" dontLoadDataFirstTime="True" BindingFields="(Insieme)" externalToolBar-Length="0">
									<table id="Table1" style="table-layout: fixed" cellspacing="0" cellpadding="3" width="100%">
                                        <colgroup>
                                            <col style="width:10%" />
                                            <col style="width:20%" />
                                            <col style="width:10%" />
                                            <col style="width:20%" />
                                            <col style="width:10%" />
                                            <col style="width:30%" />
                                        </colgroup>
										<tr>
											<td class="label">Codice</td>
											<td colspan="5">
												<ondp:wzTextBox id="txtCodice" runat="server" maxlength="8" onblur="toUpper(this);controlloCampoCodice(this);"
                                                    BindingField-SourceField="REA_CODICE" BindingField-Hidden="False" BindingField-SourceTable="T_ANA_REAZIONI_AVVERSE" 
                                                    BindingField-Connection="ConnessioneSec" BindingField-Editable="onNew" 
                                                    CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p" CssStyles-CssEnabled="TextBox_Stringa w100p" 
                                                    CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"></ondp:wzTextBox></td>
										</tr>
										<tr>
											<td class="label">Descrizione</td>
											<td colspan="5">
												<ondp:wzTextBox id="txtDescrizione" runat="server" CssClass="w100p" maxlength="50" onblur="toUpper(this);"
                                                    BindingField-SourceField="REA_DESCRIZIONE" BindingField-Hidden="False" BindingField-SourceTable="T_ANA_REAZIONI_AVVERSE" 
                                                    BindingField-Connection="ConnessioneSec" BindingField-Editable="always" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p" 
                                                    CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"></ondp:wzTextBox></td>
										</tr>
										<tr>
											<td class="label">Locale</td>
											<td>
												<ondp:wzDropDownList id="WzDropDownList1" runat="server" BindingField-SourceField="REA_LOCALE" BindingField-Hidden="False" 
                                                    BindingField-SourceTable="T_ANA_REAZIONI_AVVERSE" BindingField-Connection="ConnessioneSec" BindingField-Editable="always" 
                                                    CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p" CssStyles-CssEnabled="TextBox_Stringa w100p" 
                                                    CssStyles-CssRequired="TextBox_Stringa_Obbligatorio  w100p" SourceTable="T_ANA_CODIFICHE" 
                                                    TextFieldName="COD_DESCRIZIONE" KeyFieldName="COD_CODICE" DataFilter="cod_campo='REA_LOCALE'" OtherListFields="cod_campo"></ondp:wzDropDownList></td>
											<td class="label">Obsoleta</td>
                                            <td>
										        <ondp:wzDropDownList id="ddlObsoleto" runat="server" Width="50%" CssStyles-CssRequired="textbox_stringa"
											        CssStyles-CssEnabled="textbox_stringa" CssStyles-CssDisabled="textbox_stringa_disabilitato" BindingField-Editable="always" 
                                                    BindingField-Connection="ConnessioneSec" BindingField-SourceTable="T_ANA_REAZIONI_AVVERSE" BindingField-Hidden="False" 
                                                    BindingField-SourceField="REA_OBSOLETO" BindingField-Value="N">
											        <asp:ListItem Value="N" Selected="True">NO</asp:ListItem>
											        <asp:ListItem Value="S">SI</asp:ListItem>
										        </ondp:wzDropDownList></td>
                                            <td class="label">Codice Esterno</td>
											<td >
												<ondp:wzTextBox id="txtCodiceEsterno" runat="server" CssClass="w100p" maxlength="50" onblur="toUpper(this);"
                                                    BindingField-SourceField="REA_CODICE_ESTERNO" BindingField-Hidden="False" BindingField-SourceTable="T_ANA_REAZIONI_AVVERSE" 
                                                    BindingField-Connection="ConnessioneSec" BindingField-Editable="always" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p" 
                                                    CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"></ondp:wzTextBox></td>
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
