<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RicPazPS.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.RicPazPS" %>

<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>RicPazProntoSoccorso</title>
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
    <script type="text/javascript">        
        function AvviaRicerca(evt, obj) {
            if (evt.keyCode == 13) {
                // finestraModaleNacita_id è una variabile che viene aggiunta dal code behind
                var validFm = isValidFinestraModale(finestraModaleNacita_id, false);
                if (batchValidator.exec() && validFm) {
                    __doPostBack("cerca", "");
                }
                if (!validFm) {
                    var oFm = document.getElementById();
                    if (oFm != null) {
                        oFm.blur();
                    }
                }
            }
        }
	
        <%= HideLeftFrameIfNeeded() %>
		
    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Titolo="Ricerca Paziente" TitleCssClass="title3" Width="100%" Height="100%">
            <ondp:onitdatapanel id="odpRicercaPaziente" runat="server" fieldbindingmode="bindMixed"
                width="100%" height="100%" renderonlychildren="True" configfile="RicPazPS.OnitDataPanel1.xml"
                dontloaddatafirsttime="True" detaildestpage-length="21" detaildestpanel-length="20" emptysearch="False">
                <div>
					<igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                        <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                        <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                        <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
						<Items>
							<igtbar:TBarButton Key="btnFind" Text="Cerca" Image="~/Images/cerca.gif"></igtbar:TBarButton>
							<igtbar:TBSeparator></igtbar:TBSeparator>
							<igtbar:TBarButton Key="btnConfirm" Image="~/Images/conferma.gif" Text="Conferma"></igtbar:TBarButton>
							<igtbar:TBarButton Key="btnPulisci" Image="~/Images/Pulisci.gif" Text="Pulisci"></igtbar:TBarButton>
						</Items>
						<ClientSideEvents InitializeToolbar="" Click="toolbar_click"></ClientSideEvents>
					</igtbar:UltraWebToolbar>
                </div>
				<div style="background-color: #E1EDFF">
					<ondp:wzFilter id="tabRicerca" runat="server" Width="100%" CssClass="InfraUltraWebTab2" OnitStyle="False" ThreeDEffect="False">
						<searchOptions>
							<ondp:wzFilterOption SearchControlName="txtNome" ignoreCase="False" includeNulls="False" mode="percDopo"></ondp:wzFilterOption>
							<ondp:wzFilterOption SearchControlName="txtCognome" ignoreCase="False" includeNulls="False" mode="percDopo"></ondp:wzFilterOption>
							<ondp:wzFilterOption SearchControlName="odpDataNascita" ignoreCase="False" includeNulls="False" mode="Esatta"></ondp:wzFilterOption>
						</searchOptions>
						<DefaultTabStyle Height="20px" BackColor="#E1EDFF"></DefaultTabStyle>
						<RoundedImage SelectedImage="ig_tab_blue2.gif" NormalImage="ig_tab_blue1.gif" FillStyle="LeftMergedWithCenter"></RoundedImage>
						<Tabs>
							<igtab:Tab Text="Ricerca di Base" Visible="False">
								<ContentTemplate>
									<table cellspacing="10" cellpadding="0" border="0" width="100%" height="100%">
										<tr>
											<td width="90" align="right">
												<asp:Label id="Label1" runat="server" CssClass="LABEL">Filtro di Ricerca</asp:Label>
											</td>
											<td>
												<asp:TextBox id="WzFilterKeyBase" runat="server" cssclass="textbox_stringa w100p"></asp:TextBox>
											</td>
										</tr>
									</table>
								</ContentTemplate>
							</igtab:Tab>
							<igtab:Tab Text="Criteri di ricerca">
								<Style Font-Size="10pt" Font-Names="arial" Font-Bold="True"></Style>
								<ContentTemplate>
									<table id="Table5" onkeyup="AvviaRicerca(event,this)" style="width:99%" cellspacing="0" cellpadding="1" border="0">
                                        <colgroup>
                                            <col style="width: 13%" />
                                            <col style="width: 35%" />
                                            <col style="width: 12%" />
                                            <col style="width: 8%" />
                                            <col style="width: 10%"/>
                                            <col style="width: 7%"/>
                                            <col style="width: 5%" />
                                            <col style="width: 10%"/>
                                        </colgroup>
										<tr>
											<td style="text-align:right">
												<asp:Label id="Label2" runat="server" CssClass="label">Cognome</asp:Label></td>
											<td>
												<ondp:wzOnitJsValidator id="txtCognome" runat="server" Width="100%" CustomValFunction-Length="20"
													CustomValFunction="validaCognomeNomeRic" validationType="Validate_custom"
													autoFormat="True" actionUndo="False" actionSelect="True" actionMessage="True" actionFocus="True" actionDelete="False"
													actionCorrect="False" BindingField-Editable="always" BindingField-Connection="centrale" BindingField-SourceTable="t_paz_pazienti_centrale"
													BindingField-Hidden="False" BindingField-SourceField="PAZ_COGNOME" AddToBatchValidation="True">
													<Parameters>
														<on_val:ValidationParam paramType="boolean" paramValue="true" paramOrder="0" paramName="blnUpper"></on_val:ValidationParam>
													</Parameters>
												</ondp:wzOnitJsValidator></td>
											<td style="text-align:right">
												<asp:Label id="Label3" runat="server" CssClass="label">Nome</asp:Label></td>
											<td colspan="5">
												<ondp:wzOnitJsValidator id="txtNome" runat="server" Width="100%" CustomValFunction-Length="20" 
													CustomValFunction="validaCognomeNomeRic" validationType="Validate_custom" autoFormat="True" actionUndo="False"
													actionSelect="True" actionMessage="True" actionFocus="True" actionDelete="False" actionCorrect="False"
													BindingField-Editable="always" BindingField-Connection="centrale" BindingField-SourceTable="t_paz_pazienti_centrale"
													BindingField-Hidden="False" BindingField-SourceField="PAZ_NOME" AddToBatchValidation="True">
													<Parameters>
														<on_val:ValidationParam paramType="boolean" paramValue="true" paramOrder="0" paramName="blnUpper"></on_val:ValidationParam>
													</Parameters>
												</ondp:wzOnitJsValidator></td>
										</tr>
										<tr>
											<td style="text-align:right">
												<asp:Label id="Label6" runat="server" CssClass="label">Comune di nascita</asp:Label></td>
											<td>
												<ondp:wzFinestraModale id="WzFinestraModale1" runat="server" Width="100%" CssClass="textbox_data" CssStyles-CssRequired="TextBox_Data_obbligatorio"
													CssStyles-CssEnabled="TextBox_Data" CssStyles-CssDisabled="TextBox_Data_disabilitato" BindingDescription-SourceField="PAZ_ALIAS"
													BindingDescription-SourceTable="t_paz_pazienti_centrale" BindingDescription-Connection="centrale" Obbligatorio="False"
													DataTypeDescription="Stringa" BindingDescription-Hidden="False" BindingDescription-Editable="always"
													UseCode="False" BindingCode-SourceField="PAZ_COM_CODICE_NASCITA" BindingCode-Hidden="False" BindingCode-SourceTable="t_paz_pazienti_centrale"
													BindingCode-Connection="centrale" BindingCode-Editable="always" DataTypeCode="Stringa" RaiseChangeEvent="False"
													PosizionamentoFacile="False" LabelWidth="-8px" CodiceWidth="0px" Tabella="T_ANA_COMUNI" CampoCodice="COM_CODICE as CODICE"
													CampoDescrizione="COM_DESCRIZIONE as DESCRIZIONE" SetUpperCase="True"  UseTableLayout="False" Filtro="1=1 ORDER BY DESCRIZIONE" ></ondp:wzFinestraModale></td>
											<td style="text-align:right">
												<asp:Label id="Label4" runat="server" CssClass="label">Sesso</asp:Label></td>
											<td>
												<ondp:wzDropDownList id="ddlSesso" runat="server" Width="100%" 
													BindingField-Editable="always" BindingField-Connection="centrale" BindingField-SourceTable="t_paz_pazienti_centrale"
													BindingField-Hidden="False" BindingField-SourceField="PAZ_SESSO" SourceConnection=" " IncludeNull="False"
													BindingField-Value="M">
													<asp:ListItem Selected="True"></asp:ListItem>
													<asp:ListItem Value="M">M</asp:ListItem>
													<asp:ListItem Value="F">F</asp:ListItem>
												</ondp:wzDropDownList></td>
											<td style="text-align:right">
												<asp:Label id="Label5" runat="server" CssClass="label">Data Nascita</asp:Label></td>
											<td>
												<ondp:wzOnitDatePick id="odpDataNascita" runat="server" height="22px" Width="120px"
													CssClass="TextBox_data" BindingField-Editable="always" BindingField-Connection="centrale"
													BindingField-SourceTable="t_paz_pazienti_centrale" BindingField-Hidden="False" BindingField-SourceField="PAZ_DATA_NASCITA"
													BorderColor="White"></ondp:wzOnitDatePick></td>
                                            <td colspan="2"></td>
										</tr>
										<tr>
											<td style="text-align:right">
												<asp:Label id="Label7" runat="server" CssClass="label">Codice Fiscale</asp:Label></td>
											<td>
												<ondp:wzOnitJsValidator id="txtCodFiscale" runat="server" Width="100%" CustomValFunction-Length="14" 
													CustomValFunction="validaSintaxCF" validationType="Validate_custom" autoFormat="True" actionUndo="False"
													actionSelect="True" actionMessage="True" actionFocus="True" actionDelete="False" actionCorrect="False"
													BindingField-Editable="always" BindingField-Connection="centrale" BindingField-SourceTable="t_paz_pazienti_centrale"
													BindingField-Hidden="False" BindingField-SourceField="PAZ_CODICE_FISCALE" AddToBatchValidation="True"></ondp:wzOnitJsValidator></td>
											<td style="text-align:right">
												<asp:Label id="Label8" runat="server" CssClass="label">Tessera sanitaria</asp:Label></td>
											<td colspan="5">
												<ondp:wzTextBox id="txtTesseraSan" runat="server" Width="100%"
													BindingField-Editable="always" BindingField-Connection="centrale" BindingField-SourceTable="t_paz_pazienti_centrale"
													BindingField-Hidden="False" BindingField-SourceField="PAZ_TESSERA"></ondp:wzTextBox></td>
										</tr>
										<tr id="trMedico" visible="false" runat="server">
											<td style="text-align:right">
												<asp:Label id="lblMedico" runat="server" CssClass="label">Medico</asp:Label></td>
											<td>
												<ondp:wzFinestraModale id="fmMedico" runat="server" Width="70%" CssStyles-CssRequired="TextBox_stringa_obbligatorio"
													CssStyles-CssEnabled="TextBox_stringa" CssStyles-CssDisabled="TextBox_stringa_disabilitato" BindingDescription-SourceField="PAZ_ALIAS"
													BindingDescription-SourceTable="t_paz_pazienti_centrale" BindingDescription-Connection="centrale" Obbligatorio="False"
													DataTypeDescription="Stringa" BindingDescription-Hidden="False" BindingDescription-Editable="always" IsDistinct="true"
													UseCode="True" BindingCode-SourceField="PAZ_MED_CODICE_BASE" BindingCode-Hidden="False" BindingCode-SourceTable="t_paz_pazienti_centrale"
													BindingCode-Connection="centrale" BindingCode-Editable="always" DataTypeCode="Stringa" RaiseChangeEvent="False" 
													PosizionamentoFacile="false" LabelWidth="-8px" CodiceWidth="30%" Tabella="T_ANA_MEDICI, T_MED_ABILITAZIONI_VIS_PAZ" 
													Filtro="(MED_CODICE = MAP_MED_CODICE_MEDICO AND MAP_DATA_INIZIO <= {0} AND MAP_DATA_FINE >= {0} AND MAP_MED_CODICE_ABILITATO = '{1}') UNION SELECT MED_CODICE,MED_DESCRIZIONE FROM T_ANA_MEDICI WHERE MED_CODICE = '{1}' ORDER BY MED_CODICE" 
													CampoCodice="MED_CODICE" CampoDescrizione="MED_DESCRIZIONE" SetUpperCase="True"  UseTableLayout="True" ></ondp:wzFinestraModale>
											</td>
                                            <td colspan="6"></td>
										</tr>
									</table>
								</ContentTemplate>
							</igtab:Tab>
						</Tabs>
					</ondp:wzFilter>
					<div class="sezione" id="sezioneRisultati">
						<asp:Label id="labelRisultati" runat="server">Risultati della ricerca </asp:Label>
					</div>
				</div>

                <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">

					<ondp:wzMsDataGrid id="WzMsDataGrid1" runat="server" width="100%" PagerVoicesBefore="-1" PagerVoicesAfter="-1"
						AllowSelection="False" EditMode="None" AutoGenerateColumns="False" sortAscImage="~/Images/ordAZ.gif"
						sortDescImage="~/Images/ordZA.gif" SelectionOption="clientOnly">
						<ItemStyle CssClass="item"></ItemStyle>
						<HeaderStyle CssClass="header"></HeaderStyle>
						<AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
						<Columns>
							<ondp:wzSelectorColumn></ondp:wzSelectorColumn>
							<ondp:wzMultiSelColumn></ondp:wzMultiSelColumn>
							<ondp:wzBoundColumn SourceConn="centrale" SourceField="PAZ_COGNOME" HeaderText="Cognome" SourceTable="t_paz_pazienti_centrale"></ondp:wzBoundColumn>
							<ondp:wzBoundColumn SourceConn="centrale" SourceField="PAZ_NOME" HeaderText="Nome" SourceTable="t_paz_pazienti_centrale"></ondp:wzBoundColumn>
							<ondp:wzBoundColumn SourceConn="centrale" SourceField="PAZ_SESSO" HeaderText="Sesso" SourceTable="t_paz_pazienti_centrale"></ondp:wzBoundColumn>
							<ondp:wzBoundColumn SourceConn="centrale" SourceField="PAZ_DATA_NASCITA" HeaderText="Data nascita" SourceTable="t_paz_pazienti_centrale" DataFormatString="{0:dd/MM/yyyy}"></ondp:wzBoundColumn>
							<ondp:wzBoundColumn SourceConn="locale_cnas" SourceField="COM_DESCRIZIONE" HeaderText="Comune nascita" SourceTable="T_ANA_COMUNI"></ondp:wzBoundColumn>
							<ondp:wzBoundColumn SourceConn="centrale" SourceField="PAZ_CODICE_FISCALE" HeaderText="Codice fiscale" SourceTable="t_paz_pazienti_centrale"></ondp:wzBoundColumn>
							<ondp:wzBoundColumn SourceConn="centrale" SourceField="PAZ_TESSERA" HeaderText="Tessera" SourceTable="t_paz_pazienti_centrale"></ondp:wzBoundColumn>
							<ondp:wzBoundColumn SourceConn="locale_comre" SourceField="COM_DESCRIZIONE" HeaderText="Comune res." SourceTable="T_ANA_COMUNI"></ondp:wzBoundColumn>
							<ondp:wzBoundColumn SourceConn="centrale" SourceField="PAZ_INDIRIZZO_RESIDENZA" HeaderText="Indirizzo res." SourceTable="t_paz_pazienti_centrale"></ondp:wzBoundColumn>
							<ondp:wzBoundColumn SourceConn="centrale" SourceField="PAZ_TIPO" HeaderText="paz tipo" SourceTable="t_paz_pazienti_centrale"></ondp:wzBoundColumn>
						</Columns>
						<EditItemStyle CssClass="edit"></EditItemStyle>
						<PagerStyle CssClass="pager"></PagerStyle>
						<FooterStyle CssClass="footer"></FooterStyle>
						<SelectedItemStyle CssClass="selected"></SelectedItemStyle>
						<BindingColumns>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Cognome" Connection="centrale" SourceTable="t_paz_pazienti_centrale" Hidden="False" SourceField="PAZ_COGNOME"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Nome" Connection="centrale" SourceTable="t_paz_pazienti_centrale" Hidden="False" SourceField="PAZ_NOME"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Sesso" Connection="centrale" SourceTable="t_paz_pazienti_centrale" Hidden="False" SourceField="PAZ_SESSO"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Data nascita" Connection="centrale" SourceTable="t_paz_pazienti_centrale" Hidden="False" SourceField="PAZ_DATA_NASCITA"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Comune nascita" Connection="locale_cnas" SourceTable="T_ANA_COMUNI" Hidden="False" SourceField="COM_DESCRIZIONE"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Codice fiscale" Connection="centrale" SourceTable="t_paz_pazienti_centrale" Hidden="False" SourceField="PAZ_CODICE_FISCALE"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Tessera" Connection="centrale" SourceTable="t_paz_pazienti_centrale" Hidden="False" SourceField="PAZ_TESSERA"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Comune res." Connection="locale_comre" SourceTable="T_ANA_COMUNI" Hidden="False" SourceField="COM_DESCRIZIONE"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Indirizzo res." Connection="centrale" SourceTable="t_paz_pazienti_centrale" Hidden="False" SourceField="PAZ_INDIRIZZO_RESIDENZA"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="paz tipo" Connection="centrale" SourceTable="t_paz_pazienti_centrale" Hidden="False" SourceField="PAZ_TIPO"></ondp:BindingFieldValue>
						</BindingColumns>
					</ondp:wzMsDataGrid>

                </dyp:DynamicPanel>
			</ondp:onitdatapanel>
        </on_lay3:OnitLayout3>

        <script type="text/javascript">
            var enableNextPostBack = true;
			
            function toolbar_click(oToolBar, oItem, oEvent) // per gestire il click della ToolBar Infragistics
            {
                if (oToolBar.Enabled) {
                    oEvent.needPostBack = enableNextPostBack;

                    switch (oItem.Key) {

                        // Premuto il pulsante "Annulla"
                        case 'btnPulisci':
                            pulisciCampi();
                            oEvent.needPostBack = false;
                            //oEvent.cancel=true;
                            break;

                        case 'btnFind':
                            //vb script
                            var oTable, oList, i;
                            var flag_pieni;

                            flag_pieni = 0;

                            oTable = document.getElementById("Table5");
                            oList = oTable.getElementsByTagName("input");

                            if (oList[0].value != "") flag_pieni = 1;
                            if (oList[1].value != "") flag_pieni = 1;
                            if (oList[4].value != "") flag_pieni = 1;
                            //if (oList[5].value!="") flag_pieni=1;
                            if (oList[6].value != "") flag_pieni = 1;

                            if (flag_pieni == 0) passa_val_al_server();

                            var validFm = isValidFinestraModale(finestraModaleNacita_id, false);
                            if (!validFm) {
                                var oFm = document.getElementById(finestraModaleNacita_id);
                                if (oFm != null) oFm.blur();
                                oEvent.needPostBack = false;
                            }
                            break;
                    }
                    return;
                }
            }

            function pulisciCampi() {
                var oTable, oList, i;

                oTable = document.getElementById("Table5");
                oList = oTable.getElementsByTagName("input");
                for (i = 0; i < oList.length; i++) {
                    oList[i].value = ""
                }
                oList = oTable.getElementsByTagName("select");
                for (i = 0; i < oList.length; i++) {
                    oList[i].selectedIndex = 0;
                }
            }
        </script>
    </form>
</body>
</html>
