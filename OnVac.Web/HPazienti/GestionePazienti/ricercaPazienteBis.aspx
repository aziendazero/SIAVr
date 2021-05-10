<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ricercaPazienteBis.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.ricercaPazienteBis" ValidateRequest="false" %>

<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="uc1" TagName="OnVacAlias" Src="OnVacAlias.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ConsensoUtente" Src="../../Common/Controls/ConsensoTrattamentoDatiUtente.ascx" %>

<%@ Import Namespace="Onit.OnAssistnet.OnVac" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>ricercaPazienteBis</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript" language="javascript">
        function AvviaRicerca(evt, obj) {
            if (evt.keyCode == 13) {
                /* idFmNascita e idFmResidenza sono dichiarati lato server. Se la modale di residenza è nascosta, idFmResidenza == ''. */
                var isValidFmNascita = isValidFinestraModale(idFmNascita, false);
                var isValidFmResidenza = true;

                if (idFmResidenza != '') isValidFmResidenza = isValidFinestraModale(idFmResidenza, false);

                if (batchValidator.exec() && isValidFmNascita && isValidFmResidenza) {
                    __doPostBack("cerca", "");
                }

                var fm = null;

                if (!isValidFmNascita) {
                    fm = document.getElementById(idFmNascita);
                } else if (!isValidFmResidenza) {
                    fm = document.getElementById(idFmResidenza);
                }

                if (fm != null) fm.blur();
            }
        }

        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function popitup(url) {
            newwindow = window.open(url, 'Consenso', 'top=0,left=0,width=750,height=550,menubar=0,resizable=1,scrollbars=1');
            if (window.focus) {newwindow.focus()}
            return false;
        }

        function RefreshFromPopup() {
            __doPostBack("RefreshFromPopup", "");
        }        
    </script>
</head>
<body>
    <script type="text/javascript" language="javascript">
        <%= HideLeftFrameIfNeeded() %>
    </script>
    <form id="Form1" method="post" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" TitleCssClass="title3" Titolo="Ricerca paziente" Width="100%" Height="100%">
            <ondp:onitdatapanel id="odpRicercaPaziente" runat="server" fieldbindingmode="bindMixed"
                width="100%" height="100%" renderonlychildren="True" configfile="ricercaPaziente.OnitDataPanel1.xml"
                dontloaddatafirsttime="True" detaildestpage-length="21" detaildestpage="GestionePazienti.aspx"
                detaildestpanel-length="20" detaildestpanel="odpDettaglioPaziente" emptysearch="False">
                <div>
                    <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="90px" CssClass="infratoolbar" >
                        <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                        <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		                <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
						<ClientSideEvents InitializeToolbar="InizializzaToolBar" MouseOver="toolbar_OnMouseOver" Click="toolbar_click" MouseOut="toolbar_OnMouseOut" Move="toolbar_Move"></ClientSideEvents>
						<Items>
							<igtbar:TBarButton Key="btnFind" Text="Cerca" Image="~/Images/cerca.gif"></igtbar:TBarButton>
							<igtbar:TBSeparator></igtbar:TBSeparator>
							<igtbar:TBarButton Key="btnEdit2" Image="~/Images/conferma.gif" Text="Conferma"></igtbar:TBarButton>
							<igtbar:TBarButton Key="btnPulisci" Image="~/Images/Pulisci.gif" Text="Pulisci"></igtbar:TBarButton>
							<igtbar:TBarButton Key="btnNew" Image="~/Images/nuovo.gif" Text="Inserisci" DisabledImage="~/Images/nuovo_dis.gif"></igtbar:TBarButton>
                            <igtbar:TBSeparator Key="sepRicercheRapide"></igtbar:TBSeparator>
                            <igtbar:TBarButton Key="btnUltimoPaz" Text="Ultimo Paziente" Image="~/Images/paziente.gif" ToolTip="Effettua la ricerca dell'ultimo paziente selezionato">
                                <DefaultStyle CssClass="infratoolbar_button_default" Width="120px"></DefaultStyle>
                            </igtbar:TBarButton>
                            <igtbar:TBarButton Key="btnUltimaRicerca" Text="Ultima Ricerca" Image="~/Images/rieseguiRicerca.gif" ToolTip="Riesegue l'ultima ricerca effettuata">
                                <DefaultStyle CssClass="infratoolbar_button_default" Width="120px"></DefaultStyle>
                            </igtbar:TBarButton>
							<igtbar:TBarButton Key="btnAlias" Image="../../images/alias.gif" Text="Alias"></igtbar:TBarButton>
							<igtbar:TBSeparator Key="sepConsenso"></igtbar:TBSeparator>
							<igtbar:TBarButton Key="btnConsenso" Image="~/Images/Consensi.gif" Text="Consensi" DisabledImage="~/Images/Consensi_dis.gif" 
                                ToolTip="Apertura programma di rilevazione del consenso" ></igtbar:TBarButton>
						</Items>
					</igtbar:UltraWebToolbar>
                </div>
				<div style="background-color: #e1edff">
					<ondp:wzFilter id="tabRicerca" runat="server" Width="100%" CssClass="InfraUltraWebTab2" OnitStyle="False" ThreeDEffect="False">
						<searchOptions>
							<ondp:wzFilterOption SearchControlName="txtNome" ignoreCase="False" includeNulls="False" mode="percDopo"></ondp:wzFilterOption>
							<ondp:wzFilterOption SearchControlName="txtCognome" ignoreCase="False" includeNulls="False" mode="percDopo"></ondp:wzFilterOption>
							<ondp:wzFilterOption SearchControlName="odpDataNascita" ignoreCase="False" includeNulls="False" mode="Esatta"></ondp:wzFilterOption>
							<%-- 
								Il campo del codice fiscale ha una validazione "validaSintaxCF" per cui la ricerca può avvenire con mode="Esatta", ovvero senza like nella query
							--%>
							<ondp:wzFilterOption SearchControlName="txtCodFiscale" ignoreCase="False" includeNulls="False" mode="Esatta"></ondp:wzFilterOption>
						</searchOptions>
						<DefaultTabStyle Height="20px" BackColor="#E1EDFF" ></DefaultTabStyle>
						<RoundedImage SelectedImage="ig_tab_blue2.gif" NormalImage="ig_tab_blue1.gif" FillStyle="LeftMergedWithCenter"></RoundedImage>
						<Tabs>
							<igtab:Tab Visible="False">
								<ContentTemplate>
									<table cellspacing="10" cellpadding="0" border="0" width="100%" height="100%">
										<tr>
											<td width="90" align="right" class="Label">
												<asp:Label id="Label1" runat="server" >Filtro di Ricerca</asp:Label>
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
									<table id="Table5" onkeyup="AvviaRicerca(event,this)" style="width: 99%;" cellSpacing="0" cellPadding="1" border="0">
                                        <colgroup>
                                            <col width="13%" align="right" />
                                            <col width="35%" />
                                            <col width="12%" align="right" />
                                            <col width="8%" />
                                            <col width="10%" align="right"/>
                                            <col width="7%"/>
                                            <col width="5%" align="right" />
                                            <col width="10%"/>
                                        </colgroup>
										<tr id="trCodicePaziente" runat="server">
											<td class="Label">
												<asp:Label id="lblPazCodice" runat="server" >Codice</asp:Label></td>
											<td colspan="7">
												<ondp:wzTextBox id="txtPazCodice" runat="server" BindingField-SourceField="PAZ_CODICE" BindingField-Hidden="False" BindingField-SourceTable="T_PAZ_PAZIENTI"
													BindingField-Connection="locale" BindingField-Editable="always" Width="130px"></ondp:wzTextBox></td>
										</tr>
										<tr>
											<td class="Label">
												<asp:Label id="Label2" runat="server" >Cognome</asp:Label></td>
											<td>
												<ondp:wzOnitJsValidator id="txtCognome" runat="server" Width="100%"
													CustomValFunction-Length="20" AddToBatchValidation="True" BindingField-SourceField="PAZ_COGNOME" BindingField-Hidden="False"
													BindingField-SourceTable="t_paz_pazienti_centrale" BindingField-Connection="centrale" BindingField-Editable="always"
													actionCorrect="False" actionDelete="False" actionFocus="True" actionMessage="True" actionSelect="True"
													actionUndo="False" autoFormat="True" validationType="Validate_custom" CustomValFunction="validaCognomeNomeRic">
													<Parameters>
														<on_val:ValidationParam paramType="boolean" paramValue="true" paramOrder="0" paramName="blnUpper"></on_val:ValidationParam>
													</Parameters>
												</ondp:wzOnitJsValidator></td>
											<td class="Label">
												<asp:Label id="Label3" runat="server" >Nome</asp:Label></td>
											<td colspan="5">
												<ondp:wzOnitJsValidator id="txtNome" runat="server" CustomValFunction-Length="20" Width="100%"
													AddToBatchValidation="True" BindingField-SourceField="PAZ_NOME" BindingField-Hidden="False" BindingField-SourceTable="t_paz_pazienti_centrale"
													BindingField-Connection="centrale" BindingField-Editable="always" actionCorrect="False" actionDelete="False"
													actionFocus="True" actionMessage="True" actionSelect="True" actionUndo="False" autoFormat="True" validationType="Validate_custom"
													CustomValFunction="validaCognomeNomeRic">
													<Parameters>
														<on_val:ValidationParam paramType="boolean" paramValue="true" paramOrder="0" paramName="blnUpper"></on_val:ValidationParam>
													</Parameters>
												</ondp:wzOnitJsValidator></td>
										</tr>
										<tr>
											<td class="Label">
												<asp:Label id="Label6" runat="server" >Comune di nascita</asp:Label></td>
											<td>
												<ondp:wzFinestraModale id="WzFinestraModale1" runat="server" Width="100%" CssClass="textbox_data" SetUpperCase="True"
													CampoDescrizione="COM_DESCRIZIONE as DESCRIZIONE" CampoCodice="COM_CODICE as CODICE" Tabella="T_ANA_COMUNI" CodiceWidth="0px"
													LabelWidth="-8px" PosizionamentoFacile="True" RaiseChangeEvent="False" DataTypeCode="Stringa" BindingCode-Editable="always"
													BindingCode-Connection="centrale" BindingCode-SourceTable="t_paz_pazienti_centrale" BindingCode-Hidden="False"
													BindingCode-SourceField="PAZ_COM_CODICE_NASCITA" UseCode="False" BindingDescription-Editable="always"
													BindingDescription-Hidden="False" DataTypeDescription="Stringa" Obbligatorio="False" BindingDescription-Connection="centrale"
													BindingDescription-SourceTable="t_paz_pazienti_centrale" BindingDescription-SourceField="PAZ_ALIAS"
													CssStyles-CssDisabled="TextBox_Data_disabilitato" CssStyles-CssEnabled="TextBox_Data" CssStyles-CssRequired="TextBox_Data_obbligatorio"
                                                    Filtro="1=1 ORDER BY DESCRIZIONE"></ondp:wzFinestraModale></td>
											<td class="Label">
												<asp:Label id="Label4" runat="server" >Sesso</asp:Label></td>
											<td>
												<ondp:wzDropDownList id="ddlSesso" runat="server" Width="100%" 
													BindingField-SourceField="PAZ_SESSO" BindingField-Hidden="False" BindingField-SourceTable="t_paz_pazienti_centrale"
													BindingField-Connection="centrale" BindingField-Editable="always" BindingField-Value="M" IncludeNull="False"
													SourceConnection=" ">
													<asp:ListItem Selected="True"></asp:ListItem>
													<asp:ListItem Value="M">M</asp:ListItem>
													<asp:ListItem Value="F">F</asp:ListItem>
												</ondp:wzDropDownList></td>
											<td  class="Label">
												<asp:Label id="Label5" runat="server" >Data Nascita</asp:Label></td>
											<td>
												<ondp:wzOnitDatePick id="odpDataNascita" runat="server" height="22px" Width="120px"
													CssClass="TextBox_data"  BindingField-SourceField="PAZ_DATA_NASCITA" BindingField-Hidden="False"
													BindingField-SourceTable="t_paz_pazienti_centrale" BindingField-Connection="centrale" BindingField-Editable="always"
													BorderColor="White"></ondp:wzOnitDatePick></td>
											<td class="Label">
                                                <asp:Label id="lblAnnoNascita" runat="server" >Anno</asp:Label></td>
                                            <td>
                                                <on_val:OnitJsValidator id="txtAnnoNascita" runat="server" Width="100%" CssClass="textbox_stringa"
                                                    actionCorrect="False" actionDelete="False" actionFocus="False" actionMessage="True" actionSelect="True"
                                                    actionUndo="False" autoFormat="False" validationType="Validate_integer" PreParams-numDecDigit="0" PreParams-maxValue="3999"
                                                    PreParams-minValue="1800" MaxLength="4"></on_val:OnitJsValidator></td>
										</tr>
										<tr>
											<td class="Label">
												<asp:Label id="Label7" runat="server" >Codice Fiscale</asp:Label></td>
											<td>
												<ondp:wzOnitJsValidator id="txtCodFiscale" runat="server"  CustomValFunction-Length="14" Width="100%"
													AddToBatchValidation="True" BindingField-SourceField="PAZ_CODICE_FISCALE" BindingField-Hidden="False"
													BindingField-SourceTable="t_paz_pazienti_centrale" BindingField-Connection="centrale" BindingField-Editable="always"
													actionCorrect="False" actionDelete="False" actionFocus="True" actionMessage="True" actionSelect="True"
													actionUndo="False" autoFormat="True" validationType="Validate_custom" CustomValFunction="validaSintaxCF"></ondp:wzOnitJsValidator></td>
											<td class="Label">
												<asp:Label id="Label8" runat="server" >Tessera Sanitaria</asp:Label></td>
											<td colspan="5">
												<ondp:wzTextBox id="txtTesseraSan" runat="server" Width="100%" 
													BindingField-SourceField="PAZ_TESSERA" BindingField-Hidden="False" BindingField-SourceTable="t_paz_pazienti_centrale"
													BindingField-Connection="centrale" BindingField-Editable="always"></ondp:wzTextBox></td>
										</tr>
										<tr>
										    <td class="Label">
											    <asp:Label id="lblComuneResidenza" runat="server" >Comune Residenza</asp:Label></td>
                                            <td>
                                                <ondp:wzFinestraModale id="fmComuneResidenza" runat="server" Width="100%" CssClass="textbox_data" SetUpperCase="True"
												    CampoDescrizione="COM_DESCRIZIONE as DESCRIZIONE" CampoCodice="COM_CODICE as CODICE" Tabella="T_ANA_COMUNI" CodiceWidth="0px"
												    LabelWidth="-8px" PosizionamentoFacile="True" RaiseChangeEvent="False" DataTypeCode="Stringa" BindingCode-Editable="always"
												    BindingCode-Connection="centrale" BindingCode-SourceTable="t_paz_pazienti_centrale" BindingCode-Hidden="False"
												    BindingCode-SourceField="PAZ_COM_CODICE_RESIDENZA" UseCode="False" BindingDescription-Editable="always"
												    BindingDescription-Hidden="False" DataTypeDescription="Stringa" Obbligatorio="False" BindingDescription-Connection="centrale"
												    BindingDescription-SourceTable="t_paz_pazienti_centrale" BindingDescription-SourceField="PAZ_ALIAS"
												    CssStyles-CssDisabled="TextBox_Data_disabilitato" CssStyles-CssEnabled="TextBox_Data" CssStyles-CssRequired="TextBox_Data_obbligatorio"
                                                    Filtro="1=1 ORDER BY DESCRIZIONE"></ondp:wzFinestraModale></td>
											<td class="Label">
												<asp:Label id="lblFiltroCns" runat="server" >Centro Vaccinale</asp:Label></td>
											<td colspan="5">
												<ondp:wzFinestraModale id="omlConsultorio" runat="server" Width="75%" CssClass="textbox_data" SetUpperCase="True"
													CampoDescrizione="CNS_DESCRIZIONE" CampoCodice="CNS_CODICE" Tabella="T_ANA_CONSULTORI" CodiceWidth="80px"
													LabelWidth="-8px" PosizionamentoFacile="True" RaiseChangeEvent="False" DataTypeCode="Stringa" BindingCode-Editable="always"
													BindingCode-Connection="locale" BindingCode-SourceTable="T_PAZ_PAZIENTI" BindingCode-Hidden="False"
													BindingCode-SourceField="PAZ_CNS_CODICE" UseCode="True" BindingDescription-Editable="always" BindingDescription-Hidden="False"
													DataTypeDescription="Stringa" Obbligatorio="False" CssStyles-CssDisabled="TextBox_Data_disabilitato"
													CssStyles-CssEnabled="TextBox_Data" CssStyles-CssRequired="TextBox_Data_obbligatorio" autoRefreshDataBind="True"
													AllowBindingDescription="False"></ondp:wzFinestraModale></td>
										</tr>
									</table>
								</ContentTemplate>
							</igtab:Tab>
						</Tabs>
					</ondp:wzFilter>
					<div class="vac-sezione" id="sezioneRisultati">
						<asp:Label id="labelRisultati" runat="server">Risultati della ricerca </asp:Label>
                    </div>
				</div>
                <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
					<ondp:wzMsDataGrid id="WzMsDataGrid1" runat="server" width="100%" PagerVoicesBefore="-1" PagerVoicesAfter="-1"
						AllowSelection="False" EditMode="None" AutoGenerateColumns="False" sortAscImage="~/Images/ordAZ.gif"
						sortDescImage="~/Images/ordZA.gif" SelectionOption="clientOnly">
						<AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
						<FooterStyle CssClass="footer"></FooterStyle>
						<ItemStyle CssClass="item"></ItemStyle>
						<Columns>
							<ondp:wzSelectorColumn></ondp:wzSelectorColumn>
							<ondp:wzMultiSelColumn></ondp:wzMultiSelColumn>
							<ondp:wzTemplateColumn Key="StatoConsenso" HeaderText="Cons." >
								<HeaderStyle HorizontalAlign="Center" />
								<ItemStyle HorizontalAlign="Center" />
								<ItemTemplate>
									<asp:Image id="imgStatoConsenso" runat="server" />
								</ItemTemplate>		
							</ondp:wzTemplateColumn>										
							<ondp:wzBoundColumn SortDirection="NoSort" SourceTable="t_paz_pazienti" SourceField="PAZ_CODICE"
								HeaderText="Codice" SourceConn="locale" Key="Codice"></ondp:wzBoundColumn>
                            <ondp:wzBoundColumn SortDirection="NoSort" SourceTable="t_paz_pazienti_centrale" SourceField="PAZ_CODICE"
                                HeaderText="Cod. Ausil." SourceConn="centrale" Key="Ausiliario"></ondp:wzBoundColumn>
                            <ondp:wzBoundColumn SortDirection="NoSort" SourceTable="t_paz_pazienti_centrale" SourceField="PAZ_COGNOME"
								HeaderText="Cognome" SourceConn="centrale"></ondp:wzBoundColumn>
							<ondp:wzBoundColumn SortDirection="NoSort" SourceTable="t_paz_pazienti_centrale" SourceField="PAZ_NOME"
								HeaderText="Nome" SourceConn="centrale"></ondp:wzBoundColumn>
							<ondp:wzBoundColumn SortDirection="NoSort" SourceTable="t_paz_pazienti_centrale" SourceField="PAZ_SESSO"
								HeaderText="Sesso" SourceConn="centrale"></ondp:wzBoundColumn>
							<ondp:wzBoundColumn SortDirection="NoSort" SourceTable="t_paz_pazienti_centrale" SourceField="PAZ_DATA_NASCITA"
								HeaderText="Data nascita" SourceConn="centrale" DataFormatString="{0:dd/MM/yyyy}"></ondp:wzBoundColumn>
							<ondp:wzBoundColumn SortDirection="NoSort" SourceTable="T_ANA_COMUNI" SourceField="COM_DESCRIZIONE"
								HeaderText="Comune nascita" SourceConn="locale_cnas"></ondp:wzBoundColumn>
							<ondp:wzBoundColumn SortDirection="NoSort" SourceTable="t_paz_pazienti_centrale" SourceField="PAZ_CODICE_FISCALE"
								HeaderText="Codice fiscale" SourceConn="centrale"></ondp:wzBoundColumn>
							<ondp:wzBoundColumn SortDirection="NoSort" SourceTable="t_paz_pazienti_centrale" SourceField="PAZ_TESSERA"
								HeaderText="Tessera" SourceConn="centrale"></ondp:wzBoundColumn>
							<ondp:wzBoundColumn SortDirection="NoSort" SourceTable="T_ANA_COMUNI" SourceField="COM_DESCRIZIONE"
								HeaderText="Comune res." SourceConn="locale_comre"></ondp:wzBoundColumn>
							<ondp:wzBoundColumn SortDirection="NoSort" SourceTable="t_paz_pazienti_centrale" SourceField="PAZ_INDIRIZZO_RESIDENZA"
								HeaderText="Indirizzo res." SourceConn="centrale"></ondp:wzBoundColumn>
							<ondp:wzBoundColumn SortDirection="NoSort" SourceTable="T_ANA_CONSULTORI" SourceField="CNS_CODICE" 
                                HeaderText="Centro Vaccinale" SourceConn="locale"></ondp:wzBoundColumn>
                            <ondp:wzBoundColumn SortDirection="NoSort" SourceTable="t_paz_pazienti" SourceField="PAZ_STATO_ANAGRAFICO"
								HeaderText="Stato Anag." SourceConn="locale"></ondp:wzBoundColumn>
							<ondp:wzBoundColumn SortDirection="NoSort" SourceTable="t_paz_pazienti_centrale" SourceField="PAZ_USL_CODICE_ASSISTENZA"
								HeaderText="ULLS" SourceConn="centrale" Key="ULSS"></ondp:wzBoundColumn>
							<ondp:wzBoundColumn SortDirection="NoSort" SourceTable="t_paz_pazienti_centrale" SourceField="PAZ_USL_CODICE_DOMICILIO"
								HeaderText="ULLSD" SourceConn="centrale" Key="ULSSD" ></ondp:wzBoundColumn>
							<ondp:wzBoundColumn SortDirection="NoSort" SourceTable="t_paz_pazienti_centrale" SourceField="PAZ_TIPO"
								HeaderText="Paz. tipo" SourceConn="centrale"></ondp:wzBoundColumn>
                            <ondp:wzBoundColumn SortDirection="NoSort" SourceTable="t_paz_pazienti" SourceField="PAZ_CANCELLATO"
								HeaderText="Paz. Cancellato" SourceConn="locale" Visible="False"></ondp:wzBoundColumn>
						</Columns>
						<HeaderStyle CssClass="header"></HeaderStyle>
						<SelectedItemStyle CssClass="selected"></SelectedItemStyle>
						<PagerStyle CssClass="pager"></PagerStyle>
						<EditItemStyle CssClass="edit"></EditItemStyle>
						<BindingColumns>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Codice" Connection="locale"	SourceTable="t_paz_pazienti" SourceField="PAZ_CODICE"></ondp:BindingFieldValue>
                            <ondp:BindingFieldValue Value="" Editable="always" Description="Ausiliario" Connection="centrale" SourceTable="t_paz_pazienti_centrale" Hidden="False" SourceField="PAZ_CODICE"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Cognome" Connection="centrale" SourceTable="t_paz_pazienti_centrale" Hidden="False" SourceField="PAZ_COGNOME"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Nome" Connection="centrale" SourceTable="t_paz_pazienti_centrale" Hidden="False" SourceField="PAZ_NOME"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Sesso" Connection="centrale" SourceTable="t_paz_pazienti_centrale" Hidden="False" SourceField="PAZ_SESSO"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Data nascita" Connection="centrale" SourceTable="t_paz_pazienti_centrale" Hidden="False" SourceField="PAZ_DATA_NASCITA"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Comune nascita" Connection="locale_cnas" SourceTable="T_ANA_COMUNI" Hidden="False" SourceField="COM_DESCRIZIONE"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Codice fiscale" Connection="centrale" SourceTable="t_paz_pazienti_centrale" Hidden="False" SourceField="PAZ_CODICE_FISCALE"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Tessera" Connection="centrale" SourceTable="t_paz_pazienti_centrale" Hidden="False" SourceField="PAZ_TESSERA"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Comune res." Connection="locale_comre" SourceTable="T_ANA_COMUNI" Hidden="False" SourceField="COM_DESCRIZIONE"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Indirizzo res." Connection="centrale" SourceTable="t_paz_pazienti_centrale" Hidden="False" SourceField="PAZ_INDIRIZZO_RESIDENZA"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Consultorio" Connection="locale" SourceTable="T_ANA_CONSULTORI" Hidden="False" SourceField="CNS_CODICE"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Stato Anag." Connection="locale" SourceTable="t_paz_pazienti" Hidden="False" SourceField="PAZ_STATO_ANAGRAFICO"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="ULLS" Connection="centrale" SourceTable="t_paz_pazienti_centrale" Hidden="False" SourceField="PAZ_USL_CODICE_ASSISTENZA"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="ULLSD" Connection="centrale" SourceTable="t_paz_pazienti_centrale" Hidden="False" SourceField="PAZ_USL_CODICE_DOMICILIO"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="paz tipo" Connection="centrale" SourceTable="t_paz_pazienti_centrale" Hidden="False" SourceField="PAZ_TIPO"></ondp:BindingFieldValue>
                            <ondp:BindingFieldValue Value="" Editable="never" Description="Paz. Cancellato" Connection="locale" SourceTable="t_paz_pazienti" Hidden="False" SourceField="PAZ_CANCELLATO"></ondp:BindingFieldValue>
						</BindingColumns>
					</ondp:wzMsDataGrid>
                </dyp:DynamicPanel>
			</ondp:onitdatapanel>
        </on_lay3:OnitLayout3>   
        
        <!-- modale alias -->
        <on_ofm:OnitFinestraModale ID="fmOnVacAlias" Title="Scelta dell'Anagrafica Corretta" runat="server" Width="850px" Height="300px" BackColor="LightGray" NoRenderX="False">
            <uc1:OnVacAlias ID="OnVacAlias1" runat="server"></uc1:OnVacAlias>
        </on_ofm:OnitFinestraModale>

        <!-- modale consenso -->
        <on_ofm:OnitFinestraModale ID="modConsenso" Title="Consenso" runat="server" Width="880px" Height="600px" BackColor="LightGray" ClientEventProcs-OnClose="RefreshFromPopup()" >
            <iframe id="frameConsenso" runat="server" class="frameConsensoStyle">
                <div>
                    Caricamento in corso. Attendere...
                </div>
            </iframe>
        </on_ofm:OnitFinestraModale>
        
        <!-- consenso trattamento dati utente -->
        <on_ofm:OnitFinestraModale ID="fmConsensoUtente" Title="Consenso al trattamento dati per l'utente" runat="server" Width="400px" Height="250px" BackColor="LightGoldenrodYellow">
            <uc1:ConsensoUtente runat="server" id="ucConsensoUtente"></uc1:ConsensoUtente>
        </on_ofm:OnitFinestraModale>

    </form>
    <script type="text/javascript" language="javascript">

        var enableNextPostBack = true;

        function toolbar_click(oToolBar, oItem, oEvent) // per gestire il click della ToolBar Infragistics
        {
            if (oToolBar.Enabled) {
                oEvent.needPostBack = enableNextPostBack;
                switch (oItem.Key) {
                    case 'btnPulisci':
                        pulisciCampi();
                        oEvent.needPostBack = false;
                        //oEvent.cancel=true;
                        break;

                    case 'btnEdit2':
                        //var grid = get_wzMsDatagrid('WzMsDataGrid1');
                        //if (grid != null)
                        //{
                        var row_ = get_wzMsDatagrid('WzMsDataGrid1').getSelectedItem();
                        if (row_ == null) {
                            alert("Effettuare una ricerca e selezionare un paziente dall'elenco per continuare.");
                            oEvent.needPostBack = false;
                        }
                        else {
                            wzDataGrid_resetCheck("WzMsDataGrid1");
                            enableNextPostBack = false;
                        }
                        //}
                        break;

                    case 'btnFind':
                        /*idFmNascita e idFmResidenza sono dichiarati lato server. Se la modale di residenza è nascosta, idFmResidenza == ''.*/
                        var isValidFmNascita = isValidFinestraModale(idFmNascita, false);
                        var isValidFmResidenza = true;

                        if (idFmResidenza != '') isValidFmResidenza = isValidFinestraModale(idFmResidenza, false);

                        if (!isValidFmNascita || !isValidFmResidenza) {

                            var fm = null;

                            if (!isValidFmNascita) {
                                fm = document.getElementById(idFmNascita);
                            } else if (!isValidFmResidenza) {
                                fm = document.getElementById(idFmResidenza);
                            }

                            if (fm != null) fm.blur();

                            oEvent.needPostBack = false;
                        }

                        break;

                    case 'btnNew':
                        enableNextPostBack = false;
                        break;

                    case 'btnConsenso':
                        
                        var row_ = get_wzMsDatagrid('WzMsDataGrid1').getSelectedItem();
                        if (row_ == null) {
                            oEvent.needPostBack = false;
                            alert("Effettuare una ricerca e selezionare un paziente dall'elenco per continuare.");
                        }
//                        else {
//                            ausiliario = get_wzMsDatagrid('WzMsDataGrid1').getCellText(row_.rowIndex - 1, 'Ausiliario');
//                            var uteCod = '<%= OnVacContext.UserCode%>';
//                            if (ausiliario != null) {
//                                popitup('../../OpenConsensi.aspx?user=' + uteCod + '&paziente=' + ausiliario);
//                            }
//                            else {
//                                alert('<%= Me.Settings.CONSENSO_MSG_NO_COD_CENTRALE %>');
//                            }
//                        }
//                        break;
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

        /*toglie tutti i segni di spunta prima di fare conferma*/
        function wzDataGrid_resetCheck(datagrid) {

            var grid = document.getElementById(datagrid); //igtbl_getGridById(datagrid);

            /*	var i, cella, riga; 
            for (i=0; i < grid.Rows.length; i++){
            riga = grid.Rows.getRow(i);
            cella = riga.getCellFromKey("check");
            if (cella!=null){ cella.setValue("false");}
            }*/
        }

    </script>
</body>
</html>
