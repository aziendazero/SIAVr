<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Comuni.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_Comuni" %>

<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Comuni</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/archivi.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("onvac.common.js")%>'></script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Titolo="Comuni" TitleCssClass="Title3" Height="100%" Width="100%">
            <ondp:onitdatapanel id="odpComuni" runat="server" configfile="Comuni.odpComuni.xml" renderonlychildren="True" usetoolbar="False" maxrecord="200">
                <div>
					<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                        <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                        <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		                <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
						<Items>
							<igtbar:TBarButton Key="btnCerca" Text="Cerca"></igtbar:TBarButton>
							<igtbar:TBSeparator></igtbar:TBSeparator>
							<igtbar:TBarButton Key="btnNew" Text="Nuovo" ></igtbar:TBarButton>
							<igtbar:TBarButton Key="btnEdit" Text="Modifica" ></igtbar:TBarButton>
							<igtbar:TBSeparator></igtbar:TBSeparator>
							<igtbar:TBarButton Key="btnSalva" Text="Salva" ></igtbar:TBarButton>
							<igtbar:TBarButton Key="btnAnnulla" Text="Annulla" ></igtbar:TBarButton>
						</Items>
					</igtbar:UltraWebToolbar>
                </div>
                <div class="Sezione">Modulo ricerca</div>
                <div>
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
                    <ondp:wzMsDataGrid id="dgrComuni" runat="server" Width="100%" OnitStyle="False" EditMode="None" 
                        PagerVoicesAfter="-1" PagerVoicesBefore="-1" AutoGenerateColumns="False" SelectionOption="rowClick"> 
                        <HeaderStyle CssClass="header"></HeaderStyle>
                        <ItemStyle CssClass="item"></ItemStyle>
                        <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
                        <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
                        <EditItemStyle CssClass="edit"></EditItemStyle>
                        <FooterStyle CssClass="footer"></FooterStyle>
                        <PagerStyle CssClass="pager"></PagerStyle>
                        <Columns>
                            <ondp:wzBoundColumn HeaderText="Codice" Key="COM_CODICE" SourceField="COM_CODICE" SourceTable="T_ANA_COMUNI" SourceConn="connessioneMaster">
                                <HeaderStyle width="10%" />
                            </ondp:wzBoundColumn>
                            <ondp:wzBoundColumn HeaderText="Descrizione" Key="COM_DESCRIZIONE" SourceField="COM_DESCRIZIONE" SourceTable="T_ANA_COMUNI" SourceConn="connessioneMaster">
                                <HeaderStyle width="60%" />
                            </ondp:wzBoundColumn>
                            <ondp:wzBoundColumn HeaderText="Istat" Key="COM_ISTAT" SourceField="COM_ISTAT" SourceTable="T_ANA_COMUNI" SourceConn="connessioneMaster">
                                <HeaderStyle width="15%" />
                            </ondp:wzBoundColumn>
                            <ondp:wzBoundColumn HeaderText="Catastale" Key="COM_CATASTALE" SourceField="COM_CATASTALE" SourceTable="T_ANA_COMUNI" SourceConn="connessioneMaster">
                                <HeaderStyle width="15%" />
                            </ondp:wzBoundColumn>
                        </Columns>
                        <BindingColumns>
                            <ondp:BindingFieldValue Value="" Editable="always" Description="Codice" Connection="connessioneMaster" 
                            SourceTable="T_ANA_COMUNI" Hidden="False" SourceField="COM_CODICE"></ondp:BindingFieldValue>
                            <ondp:BindingFieldValue Value="" Editable="always" Description="Descrizione" Connection="connessioneMaster"
                            SourceTable="T_ANA_COMUNI" Hidden="False" SourceField="COM_DESCRIZIONE"></ondp:BindingFieldValue>
                            <ondp:BindingFieldValue Value="" Editable="always" Description="Istat" Connection="connessioneMaster" 
                            SourceTable="T_ANA_COMUNI" Hidden="False" SourceField="COM_ISTAT"></ondp:BindingFieldValue>
                            <ondp:BindingFieldValue Value="" Editable="always" Description="Catastale" Connection="connessioneMaster"
                            SourceTable="T_ANA_COMUNI" Hidden="False" SourceField="COM_CATASTALE"></ondp:BindingFieldValue>
                        </BindingColumns>
                    </ondp:wzMsDataGrid>
                </dyp:DynamicPanel>
                <div>
					<table class="Sezione" cellspacing="0" cellpadding="0" width="100%" border="0">
						<tr>
							<td>Dettaglio</td>
							<td align="right" width="10%">
								<ondp:wzCheckBox id="chkScaduto" runat="server" height="12" width="20%"
									CssStyles-CssEnabled="TextBox_Stringa" CssStyles-CssDisabled="TextBox_Stringa" 
									BindingField-Editable="always" BindingField-Connection="connessioneMaster" BindingField-SourceTable="T_ANA_COMUNI"
									BindingField-Hidden="False" BindingField-SourceField="COM_SCADENZA" BindingField-Value="N" Text="Scaduto"></ondp:wzCheckBox>
							</td>
							<td align="right" width="10%">
								<ondp:wzCheckBox id="chkObsoleto" runat="server" height="12" width="20%"
									CssStyles-CssEnabled="TextBox_Stringa" CssStyles-CssDisabled="TextBox_Stringa" 
									BindingField-Editable="always" BindingField-Connection="connessioneMaster" BindingField-SourceTable="T_ANA_COMUNI"
									BindingField-Hidden="False" BindingField-SourceField="COM_OBSOLETO" BindingField-Value="N" Text="Obsoleto"></ondp:wzCheckBox>
							</td>
						</tr>
					</table>
					<table style="table-layout: fixed" cellspacing="3" width="100%" border="0">
						<colgroup>
							<col width="10%" />
							<col width="15%" />
							<col width="10%" />
							<col width="15%" />
							<col width="10%" />
							<col width="15%" />
							<col width="10%" />
                            <col width="5%" />
							<col />
						</colgroup>
						<tr align="left">
							<td class="label">Codice</td>
							<td>
								<ondp:wzTextBox id="txtCodice" runat="server" onblur="toUpper(this);" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
									CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
									BindingField-Editable="onNew" BindingField-Connection="connessioneMaster"
									BindingField-SourceTable="T_ANA_COMUNI" BindingField-Hidden="False" maxlength="8" BindingField-SourceField="COM_CODICE"></ondp:wzTextBox></td>
							<td class="label">Descrizione</td>
							<td colspan="7">
								<ondp:wzTextBox id="txtDescrizione" runat="server" onblur="toUpper(this);" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
									CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
									BindingField-Editable="always" BindingField-Connection="connessioneMaster"
									BindingField-SourceTable="T_ANA_COMUNI" BindingField-Hidden="False" maxlength="50" BindingField-SourceField="COM_DESCRIZIONE"></ondp:wzTextBox>
                            </td>
						</tr>
						<tr>
							<td class="label">Istat</td>
							<td>
								<ondp:wzTextBox id="txtIstat" runat="server" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
									CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
									BindingField-Editable="always" BindingField-Connection="connessioneMaster"
									BindingField-SourceTable="T_ANA_COMUNI" BindingField-Hidden="False" maxlength="8" BindingField-SourceField="COM_ISTAT"></ondp:wzTextBox></td>
							<td class="label">Catastale</td>
							<td>
								<ondp:wzTextBox id="txtCatastale" runat="server" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
									CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
									BindingField-Editable="always" BindingField-Connection="connessioneMaster"
									BindingField-SourceTable="T_ANA_COMUNI" BindingField-Hidden="False" maxlength="4" BindingField-SourceField="COM_CATASTALE"></ondp:wzTextBox></td>
							<td class="label">CAP</td>
							<td>
								<ondp:wzTextBox id="txtCAP" runat="server" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
									CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
									BindingField-Editable="always" BindingField-Connection="connessioneMaster"
									BindingField-SourceTable="T_ANA_COMUNI" BindingField-Hidden="False" maxlength="5" BindingField-SourceField="COM_CAP"></ondp:wzTextBox></td>
							<td class="label">Prov.</td>
							<td colspan="3">
								<ondp:wzTextBox id="txtProvincia" runat="server" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
									CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
									BindingField-Editable="always" BindingField-Connection="connessioneMaster"
									BindingField-SourceTable="T_ANA_COMUNI" BindingField-Hidden="False" maxlength="2" BindingField-SourceField="COM_PROVINCIA"></ondp:wzTextBox></td>
						</tr>
						<tr>
							<td class="label">Cod. distretto</td>
							<td colspan="3">
								<ondp:wzFinestraModale id="wzFmDistretto" runat="server" Width="70%" BindingDescription-Connection="connessioneMaster"
									BindingCode-Connection="connessioneMaster" BindingDescription-SourceField="DIS_DESCRIZIONE" BindingCode-SourceField="COM_DIS_CODICE"
									PosizionamentoFacile="False" LabelWidth="-1px" CodiceWidth="30%" Tabella="T_ANA_DISTRETTI" Sorting="True"
									SetUpperCase="True" Paging="True" PageSize="50" MaxRecords="0" CampoCodice="DIS_CODICE as CODICE" CampoDescrizione="DIS_DESCRIZIONE as DESCRIZIONE"
									BindingCode-Editable="always" BindingCode-SourceTable="T_ANA_COMUNI" BindingCode-Hidden="False" BindingDescription-Editable="always"
									BindingDescription-SourceTable="T_ANA_DISTRETTI" BindingDescription-Hidden="False" UseCode="True" UseTableLayout="True"
									Filtro=""></ondp:wzFinestraModale></td>
				            <td class="label">Cod. regione</td>
							<td colspan="5">
								<ondp:wzFinestraModale id="WzFmRegione" runat="server" Width="70%" Obbligatorio="False" BindingDescription-Connection="connessioneMaster"
									BindingCode-Connection="connessioneMaster" BindingDescription-SourceField="REG_DESCRIZIONE" BindingCode-SourceField="COM_REG_CODICE"
									PosizionamentoFacile="False" LabelWidth="-1px" CodiceWidth="30%" Tabella="T_ANA_REGIONI" Sorting="True"
									SetUpperCase="True" Paging="True" PageSize="50" MaxRecords="0" CampoCodice="REG_CODICE as CODICE" CampoDescrizione="REG_DESCRIZIONE as DESCRIZIONE"
									BindingCode-Editable="always" BindingCode-SourceTable="T_ANA_COMUNI" BindingCode-Hidden="False" BindingDescription-Editable="always"
									BindingDescription-SourceTable="T_ANA_REGIONI" BindingDescription-Hidden="False" UseCode="True" UseTableLayout="True"></ondp:wzFinestraModale>
																		
								<ondp:wzTextBox id="txtCodiceRegione" style="POSITION: relative" runat="server" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
									CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
									BindingField-Editable="always" BindingField-Connection="connessioneMaster"
									BindingField-SourceTable="T_ANA_COMUNI" BindingField-Hidden="False" maxlength="10" BindingField-SourceField="COM_REG_CODICE"></ondp:wzTextBox>
									
							</td>								
						</tr>
						<tr>								
							<td class="label">Inizio Val.</td>
							<td colspan="1">
								<ondp:wzOnitDatePick id="dpkInizioValidita" runat="server" Width="120px" Height="20px" CssStyles-CssRequired="textbox_data_obbligatorio"
									CssStyles-CssEnabled="textbox_data" CssStyles-CssDisabled="textbox_data_disabilitato" BindingField-Editable="always"
									BindingField-Connection="connessioneMaster" BindingField-SourceTable="T_ANA_COMUNI" BindingField-Hidden="False"
									BindingField-SourceField="COM_DATA_INIZIO_VALIDITA" target="dpkInizioValidita"></ondp:wzOnitDatePick></td>
							<td class="label">Fine Val.</td>
							<td colspan="1">
								<ondp:wzOnitDatePick id="dpkFineValidità" runat="server" Width="120px" Height="20px" CssStyles-CssRequired="textbox_data_obbligatorio"
									CssStyles-CssEnabled="textbox_data" CssStyles-CssDisabled="textbox_data_disabilitato" BindingField-Editable="always"
									BindingField-Connection="connessioneMaster" BindingField-SourceTable="T_ANA_COMUNI" BindingField-Hidden="False"
									BindingField-SourceField="COM_DATA_FINE_VALIDITA" target="dpkFineValidità"></ondp:wzOnitDatePick>
                            </td>										
							<td class="label">Cod. esterno</td>
							<td>
								<ondp:wzTextBox id="txtCodiceEsterno" runat="server" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
									CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
									BindingField-Editable="always" BindingField-Connection="connessioneMaster"
									BindingField-SourceTable="T_ANA_COMUNI" BindingField-Hidden="False" MaxLength="10" BindingField-SourceField="COM_CODICE_ESTERNO"></ondp:wzTextBox>
                            </td>
                            <td class="label">Codice AVN</td>
                            <td>
                                <ondp:wzTextBox id="txtCodiceAvn" runat="server" Width="100%" MaxLength="6" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p"
									CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p"
									BindingField-Editable="always" BindingField-Connection="connessioneMaster" BindingField-SourceTable="T_ANA_COMUNI"
									BindingField-Hidden="False" BindingField-SourceField="COM_CODICE_AVN"></ondp:wzTextBox></td>
                            <td class="label">Stato AVN</td>
                            <td>
                                <ondp:wzTextBox id="txtStatoAvn" runat="server" Width="100%" MaxLength="2" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p"
									CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p"
									BindingField-Editable="always" BindingField-Connection="connessioneMaster" BindingField-SourceTable="T_ANA_COMUNI"
									BindingField-Hidden="False" BindingField-SourceField="COM_STATO_AVN"></ondp:wzTextBox></td>
						</tr>
					</table>
                </div>
		    </ondp:onitdatapanel>
        </on_lay3:OnitLayout3>
    </form>
</body>
</html>
