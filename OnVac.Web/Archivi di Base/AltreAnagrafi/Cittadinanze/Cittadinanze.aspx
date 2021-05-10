<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Cittadinanze.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_Cittadinanze" %>

<%@ Register TagPrefix="on_otb" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitTable" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Cittadinanze</title>
        
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
    <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("onvac.common.js")%>'></script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Titolo="Cittadinanze" TitleCssClass="Title3" Height="100%" Width="100%">
            <ondp:OnitDataPanel id="odpCittadinanzeMaster" runat="server" configfile="Cittadinanze.odpCittadinanzeMaster.xml" renderonlychildren="True" usetoolbar="False">
                <on_otb:onittable id="OnitTable1" runat="server" height="100%" width="100%">
                
                    <on_otb:onitsection id="sezRicerca" runat="server" width="100%" typeHeight="content">
					    <on_otb:onitcell id="cellaRicerca" runat="server" height="100%" Width="100%">
						    <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar" >
                                <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                                <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		                        <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
							    <Items>
								    <igtbar:TBarButton Key="btnCerca" Text="Cerca" ></igtbar:TBarButton>
								    <igtbar:TBSeparator></igtbar:TBSeparator>
								    <igtbar:TBarButton Key="btnNew" Text="Nuovo" ></igtbar:TBarButton>
								    <igtbar:TBarButton Key="btnEdit" Text="Modifica" ></igtbar:TBarButton>
								    <igtbar:TBSeparator></igtbar:TBSeparator>
								    <igtbar:TBarButton Key="btnSalva" Text="Salva" ></igtbar:TBarButton>
								    <igtbar:TBarButton Key="btnAnnulla" Text="Annulla" ></igtbar:TBarButton>
							    </Items>
						    </igtbar:UltraWebToolbar>
                            <div class="Sezione">Modulo ricerca</div>
						    <ondp:wzFilter id="WzFilter1" runat="server" Width="100%" Height="70px" CssClass="InfraUltraWebTab2" TargetUrl="" Dummy>
							    <SelectedTabStyle CssClass="InfraTab_Selected2"></SelectedTabStyle>
							    <DisabledTabStyle CssClass="InfraTab_Disabled2"></DisabledTabStyle>
							    <HoverTabStyle CssClass="InfraTab_Hover2"></HoverTabStyle>
                                <DefaultTabStyle CssClass="InfraTab_Default2"></DefaultTabStyle>
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
						    <ondp:wzDataGrid Browser="UpLevel" id="dgrCittadinanze" runat="server" Width="100%" PagerVoicesBefore="-1" PagerVoicesAfter="-1" OnitStyle="False" EditMode="None">
							    <DisplayLayout ColWidthDefault="" AutoGenerateColumns="False" RowHeightDefault="26px" Version="2.00"
								    GridLinesDefault="None" SelectTypeRowDefault="Single" ScrollBar="Never" BorderCollapseDefault="Separate"
								    CellPaddingDefault="3" RowSelectorsDefault="No" Name="dgrCittadinanze" CellClickActionDefault="RowSelect">
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
										    <igtbl:UltraGridColumn HeaderText="Istat" Width="15%">
											    <Footer Key="">
												    <RowLayoutColumnInfo OriginX="2"></RowLayoutColumnInfo>
											    </Footer>
											    <Header Key="" Caption="Istat">
												    <RowLayoutColumnInfo OriginX="2"></RowLayoutColumnInfo>
											    </Header>
										    </igtbl:UltraGridColumn>
										    <igtbl:UltraGridColumn HeaderText="UE" Width="15%">
											    <Footer Key="">
												    <RowLayoutColumnInfo OriginX="3"></RowLayoutColumnInfo>
											    </Footer>
											    <Header Key="" Caption="UE">
												    <RowLayoutColumnInfo OriginX="3"></RowLayoutColumnInfo>
											    </Header>
										    </igtbl:UltraGridColumn>
									    </Columns>
								    </igtbl:UltraGridBand>
							    </Bands>
							    <BindingColumns>
								    <ondp:BindingFieldValue Value="" Editable="always" Description="Codice" Connection="connessioneMaster" SourceTable="T_ANA_CITTADINANZE"
									    Hidden="False" SourceField="CIT_CODICE"></ondp:BindingFieldValue>
								    <ondp:BindingFieldValue Value="" Editable="always" Description="Descrizione" Connection="connessioneMaster"
									    SourceTable="T_ANA_CITTADINANZE" Hidden="False" SourceField="CIT_STATO"></ondp:BindingFieldValue>
								    <ondp:BindingFieldValue Value="" Editable="always" Description="Istat" Connection="connessioneMaster" SourceTable="T_ANA_CITTADINANZE"
									    Hidden="False" SourceField="CIT_ISTAT"></ondp:BindingFieldValue>
								    <ondp:BindingFieldValue Value="" Editable="always" Description="UE" Connection="connessioneMaster"
									    SourceTable="T_ANA_CITTADINANZE" Hidden="False" SourceField="CIT_CEE"></ondp:BindingFieldValue>
							    </BindingColumns>
						    </ondp:WzDataGrid>
					    </on_otb:onitcell>
				    </on_otb:onitsection>

				    <on_otb:onitsection id="sezDettagli" runat="server" width="100%" TypeHeight="Content">
					    <on_otb:onitcell id="cellaDettagli" runat="server" height="100%" Width="100%">
                            <ondp:OnitDataPanel id="odpCittadinanzeDetail" runat="server" ConfigFile="Cittadinanze.odpCittadinanzeDetail.xml" renderOnlyChildren="True" 
                                useToolbar="False" dontLoadDataFirstTime="True" externalToolBar-Length="7" externalToolBar="ToolBar" BindingFields="(Insieme)" >
							    <div class="Sezione">
								    <table class="Sezione" cellspacing="0" cellpadding="0" width="100%" border="0">
									    <tr>
										    <td>Dettaglio</td>
										    <td align="right" width="10%">
											    <ondp:wzCheckBox id="chkCee" runat="server" height="12" width="20%"
												    CssStyles-CssEnabled="TextBox_Stringa" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato"
												    BindingField-Editable="always" BindingField-Connection="connessioneSec" BindingField-SourceTable="T_ANA_CITTADINANZE"
												    BindingField-Hidden="False" BindingField-SourceField="CIT_CEE" BindingField-Value="N" Text="U.E."></ondp:wzCheckBox>
										    </td>
									    </tr>
								    </table>
							    </div>
							    <table style="table-layout: fixed" cellspacing="3" width="100%" border="0">
								    <colgroup>
									    <col width="10%">
									    <col width="40%">
									    <col width="10%">
									    <col width="40%">
									    <col>
								    </colgroup>
								    <tr align="left">
									    <td class="label">Codice</td>
									    <td>
										    <ondp:wzTextBox id="txtCodice" runat="server" onblur="toUpper(this);" maxlength="8" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
											    CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
											    BindingField-Editable="onNew" BindingField-Connection="connessioneSec" BindingField-SourceTable="T_ANA_CITTADINANZE" 
                                                BindingField-Hidden="False" BindingField-SourceField="CIT_CODICE"></ondp:wzTextBox></td>
									    <td class="label">Istat</td>
									    <td>
										    <ondp:wzTextBox id="txtIstat" runat="server" maxlength="3" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
											    CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
											    BindingField-Editable="always" BindingField-Connection="connessioneSec" BindingField-SourceTable="T_ANA_CITTADINANZE" 
                                                BindingField-Hidden="False" BindingField-SourceField="CIT_ISTAT"></ondp:wzTextBox></td>
								    </tr>
                                    <tr>
									    <td class="label">Descrizione</td>
									    <td>
										    <ondp:wzTextBox id="txtDescrizione" runat="server" onblur="toUpper(this);" maxlength="30" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
											    CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
											    BindingField-Editable="always" BindingField-Connection="connessioneSec" BindingField-SourceTable="T_ANA_CITTADINANZE" 
                                                BindingField-Hidden="False" BindingField-SourceField="CIT_STATO"></ondp:wzTextBox></td>
									    <td class="label">Cod. esterno</td>
									    <td>
										    <ondp:wzTextBox id="txtCodiceEsterno" runat="server" maxlength="10" BindingField-SourceField="CIT_CODICE_ESTERNO"
											    BindingField-Hidden="False" BindingField-SourceTable="T_ANA_CITTADINANZE"
											    BindingField-Connection="connessioneSec" BindingField-Editable="always" 
											    CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p" CssStyles-CssEnabled="TextBox_Stringa w100p"
											    CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"></ondp:wzTextBox></td>
								    </tr>
								    <tr>
									    <td class="label">Sigla</td>
									    <td>
										    <ondp:wzTextBox id="txtSigla" runat="server" maxlength="3" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
											    CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
											    BindingField-Editable="always" BindingField-Connection="connessioneSec"
											    BindingField-SourceTable="T_ANA_CITTADINANZE" BindingField-Hidden="False" BindingField-SourceField="CIT_SIGLA"></ondp:wzTextBox></td>
									    <td class="label">Scadenza</td>
									    <td>
										    <ondp:wzOnitDatePick id="dpkFineValidità" runat="server" Width="120px" Height="20px" CssStyles-CssRequired="textbox_data_obbligatorio"
											    CssStyles-CssEnabled="textbox_data" CssStyles-CssDisabled="textbox_data_disabilitato" BindingField-Editable="always"
											    BindingField-Connection="connessioneSec" BindingField-SourceTable="T_ANA_CITTADINANZE" BindingField-Hidden="False"
											    BindingField-SourceField="CIT_SCADENZA" target="dpkFineValidità"></ondp:wzOnitDatePick></td>
								    </tr>
							    </table>
				            </ondp:OnitDataPanel>
                        </on_otb:onitcell>
                    </on_otb:onitsection>
    
                </on_otb:onittable>

            </ondp:OnitDataPanel>
        </on_lay3:OnitLayout3>
    </form>
</body>
</html>
