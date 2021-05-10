<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Associazioni.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_Associazioni"%>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
		<title>Associazioni</title>

        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/archivi.css") %>' type="text/css" rel="stylesheet" />

        <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("onvac.common.js")%>'></script>
        
        <!-- patch per il click sul checkbox dell'header per la selezione di tutti i checkbox -->
        <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("patch_selectAll.js")%>'></script>

		<script type="text/javascript">
		    function InizializzaToolBar(t) {
		        t.PostBackButton = false;
		    }
			
			//controllo campo ass_ordine (modifica 03/10/2006)
		    function ToolBarClick(ToolBar, button, evnt) {
		        evnt.needPostBack = true;
		        switch (button.Key) {
		            case 'btnSalva':
		                ordine = document.getElementById('WsTbOrdAss').value;
		                if (isNaN(ordine)) {
		                    alert("Inserire un numero nel campo 'Ordine'!");
		                    evnt.needPostBack = false;
		                }
		                break;
		        }
		    }
		</script>
	</head>
	<body onload="registerCheckClick('WzDgrVaccinazioni__ctl1_ChkMultiSel');">
		<form id="Form1" method="post" runat="server">
		    <on_lay3:onitlayout3 id="LayoutAssociazioni" runat="server" height="100%" width="100%" Titolo="Associazioni" TitleCssClass="Title3">
				<ondp:OnitDataPanel id="OdpAssociazioniMaster" runat="server" width="100%" useToolbar="False" renderOnlyChildren="True" ConfigFile="Associazioni.OdpAssociazioniMaster.xml">
                    <div>
						<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBarAssociazioni" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                            <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                            <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
							<SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
							<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
							<Items>
								<igtbar:TBarButton Key="btnCerca" Text="Cerca"></igtbar:TBarButton>
								<igtbar:TBarButton Key="btnNew" Text="Nuovo"></igtbar:TBarButton>
								<igtbar:TBarButton Key="btnEdit" Text="Modifica"></igtbar:TBarButton>
								<igtbar:TBarButton Key="btnElimina" Text="Elimina"></igtbar:TBarButton>
								<igtbar:TBarButton Key="btnSalva" Text="Salva"></igtbar:TBarButton>
								<igtbar:TBarButton Key="btnAnnulla" Text="Annulla"></igtbar:TBarButton>
								<igtbar:TBarButton Key="btnVaccinazioni" Text="Vaccinazioni" Image="../../../images/vaccinazione.gif">
                                    <DefaultStyle Width="100px" CssClass="infratoolbar_button_default"></DefaultStyle>
								</igtbar:TBarButton>
								<igtbar:TBarButton Key="btnTipiCNS" Text="Tipi CV" Image="../../../images/consultorio.gif"></igtbar:TBarButton>
                                <igtbar:TBSeparator></igtbar:TBSeparator>
                                <igtbar:TBarButton Key="btnInfo" Text="Info" ></igtbar:TBarButton>
							</Items>
						</igtbar:UltraWebToolbar>
                    </div>
					<div class="Sezione">Modulo ricerca</div>
                    <div>
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
                        <ondp:wzMsDataGrid id="WzDgrVaccinazioni" runat="server" Width="100%" disableActiveRowChange="False" EditMode="None" OnitStyle="False"
	                        PagerVoicesBefore="-1" PagerVoicesAfter="-1" AutoGenerateColumns="False" SelectionOption="rowClick">
	                        <HeaderStyle CssClass="header"/>
	                        <ItemStyle CssClass="item"/>
	                        <AlternatingItemStyle CssClass="alternating"/>
	                        <EditItemStyle CssClass="edit"/>
	                        <SelectedItemStyle CssClass="selected"/>
	                        <PagerStyle CssClass="pager"/>
	                        <FooterStyle CssClass="footer"/>
	                        <Columns>
		                        <ondp:wzMultiSelColumn></ondp:wzMultiSelColumn>
		                        <ondp:wzBoundColumn HeaderText="Ordine" Key="ASS_ORDINE" SourceField="ASS_ORDINE" SourceTable="T_ANA_ASSOCIAZIONI" SourceConn="assConn">
			                        <HeaderStyle width="10%" />
		                        </ondp:wzBoundColumn>
		                        <ondp:wzBoundColumn HeaderText="Codice" Key="ASS_CODICE" SourceField="ASS_CODICE" SourceTable="T_ANA_ASSOCIAZIONI" SourceConn="assConn">
			                        <HeaderStyle width="10%" />
		                        </ondp:wzBoundColumn>
		                        <ondp:wzBoundColumn HeaderText="Descrizione" Key="ASS_DESCRIZIONE" SourceField="ASS_DESCRIZIONE" SourceTable="T_ANA_ASSOCIAZIONI" SourceConn="assConn">
			                        <HeaderStyle width="70%" />
		                        </ondp:wzBoundColumn>
		                        <ondp:wzBoundColumn HeaderText="Stampa" Key="ASS_STAMPA" SourceField="ASS_STAMPA" SourceTable="T_ANA_ASSOCIAZIONI" SourceConn="assConn">
			                        <HeaderStyle width="200px" />
		                        </ondp:wzBoundColumn>
		                        <ondp:wzBoundColumn HeaderText="Discrez." Key="ASS_DISCREZIONALE" SourceField="ASS_DISCREZIONALE" SourceTable="T_ANA_ASSOCIAZIONI" SourceConn="assConn">
			                        <HeaderStyle width="100px" cssclass="column_align_center" />
                                    <ItemStyle cssclass="column_align_center" />
		                        </ondp:wzBoundColumn>
	                        </Columns>
	                        <BindingColumns>
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="Ordine" Connection="assConn" SourceTable="T_ANA_ASSOCIAZIONI"
			                        Hidden="False" SourceField="ASS_ORDINE"></ondp:BindingFieldValue>
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="Codice" Connection="assConn" SourceTable="T_ANA_ASSOCIAZIONI"
			                        Hidden="False" SourceField="ASS_CODICE"></ondp:BindingFieldValue>
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="Descrizione" Connection="assConn" SourceTable="T_ANA_ASSOCIAZIONI"
			                        Hidden="False" SourceField="ASS_DESCRIZIONE"></ondp:BindingFieldValue>
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="Stampa" Connection="assConn" SourceTable="T_ANA_ASSOCIAZIONI"
			                        Hidden="False" SourceField="ASS_STAMPA"></ondp:BindingFieldValue>
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="Discrez." Connection="assConn" SourceTable="T_ANA_ASSOCIAZIONI"
			                        Hidden="False" SourceField="ASS_DISCREZIONALE"></ondp:BindingFieldValue>
	                        </BindingColumns>
                        </ondp:wzMsDataGrid>
                    </dyp:DynamicPanel>

					<div class="Sezione">Dettaglio</div>
                    <div>
					    <ondp:OnitDataPanel id="OdpAssociazioniDetail" runat="server" useToolbar="False" renderOnlyChildren="True"
						    ConfigFile="Associazioni.OdpAssociazioniDetail.xml" Width="100%" dontLoadDataFirstTime="True" externalToolBar="ToolBarAssociazioni"
						    externalToolBar-Length="19" BindingFields="(Insieme)">
						    <table style="table-layout: fixed" cellSpacing="3" cellPadding="0" width="100%" border="0">
                                <colgroup>
                                    <col width="9%" />
                                    <col width="24%" />
                                    <col width="10%" />
                                    <col width="3%" />
                                    <col width="7%" />
                                    <col width="3%" />
                                    <col width="6%" />
                                    <col width="3%" />
                                    <col width="7%" />
                                    <col width="5%" />
                                </colgroup>
							    <tr>
								    <td class="label">Codice</td>
								    <td>
									    <ondp:wzTextBox id="WzTbCodAss" runat="server" MaxLength="8" BindingField-SourceField="ASS_CODICE"
                                            onblur="toUpper(this);controlloCampoCodice(this);"
										    BindingField-Hidden="False" BindingField-SourceTable="T_ANA_ASSOCIAZIONI" BindingField-Connection="assConnDati"
										    BindingField-Editable="onNew" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p" CssStyles-CssEnabled="textbox_stringa w100p"
										    CssStyles-CssDisabled="textbox_stringa_disabilitato w100p"></ondp:wzTextBox>
								    </td>
								    <td class="label">Codice Esterno</td>
								    <td colSpan="5">
									    <ondp:wzTextBox id="WzTbCodAssExt" runat="server" MaxLength="10" BindingField-SourceField="ASS_CODICE_ESTERNO"
										    BindingField-Hidden="False" BindingField-SourceTable="T_ANA_ASSOCIAZIONI" BindingField-Connection="assConnDati"
										    BindingField-Editable="always" CssStyles-CssRequired="textbox_stringa w100p" CssStyles-CssEnabled="textbox_stringa w100p"
										    CssStyles-CssDisabled="textbox_stringa_disabilitato w100p"></ondp:wzTextBox></td>
                                    <td class="label">Codice AVN</td>
                                    <td>
                                        <ondp:wzTextBox id="WzTxtAvn" runat="server" Width="100%" MaxLength="2" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p"
										    CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p"
										    BindingField-Editable="always" BindingField-Connection="connessione" BindingField-SourceTable="T_ANA_ASSOCIAZIONI"
										    BindingField-Hidden="False" BindingField-SourceField="ASS_CODICE_AVN"></ondp:wzTextBox></td>
							    </tr>
							    <tr>
								    <td class="label">Descrizione</td>
								    <td>
									    <ondp:wzTextBox id="WzTbDescAss" runat="server" MaxLength="100" BindingField-SourceField="ASS_DESCRIZIONE" onblur="toUpper(this);"
										    BindingField-Hidden="False" BindingField-SourceTable="T_ANA_ASSOCIAZIONI" BindingField-Connection="assConnDati"
										    BindingField-Editable="always" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p" CssStyles-CssEnabled="textbox_stringa w100p"
										    CssStyles-CssDisabled="textbox_stringa_disabilitato w100p"></ondp:wzTextBox></td>
                                    <td class="label">Obsoleto</td>
                                    <td>
                                        <ondp:wzCheckBox id="chkObsoleto" runat="server" Height="12px" BindingField-SourceField="ASS_OBSOLETO"
										    BindingField-Hidden="False" BindingField-SourceTable="T_ANA_ASSOCIAZIONI" BindingField-Connection="assConnDati"
										    BindingField-Editable="always" CssStyles-CssEnabled="Label" CssStyles-CssDisabled="Label_Disabilitato"
										    BindingField-Value="N"></ondp:wzCheckBox></td>
                                    <td class="label">Mostra in APP</td>
                                    <td>
                                        <ondp:wzCheckBox id="chkShowInApp" runat="server" Height="12px" BindingField-SourceField="ASS_SHOW_IN_APP"
										    BindingField-Hidden="False" BindingField-SourceTable="T_ANA_ASSOCIAZIONI" BindingField-Connection="assConnDati"
										    BindingField-Editable="always" CssStyles-CssEnabled="Label" CssStyles-CssDisabled="Label_Disabilitato"
										    BindingField-Value="N"></ondp:wzCheckBox>
                                    </td>
                                    <td class="label">Discrezionale</td>
                                    <td>
                                        <ondp:wzCheckBox id="chkDiscrezionale" runat="server" Height="12px" BindingField-SourceField="ASS_DISCREZIONALE"
										    BindingField-Hidden="False" BindingField-SourceTable="T_ANA_ASSOCIAZIONI" BindingField-Connection="assConnDati"
										    BindingField-Editable="always" CssStyles-CssEnabled="Label" CssStyles-CssDisabled="Label_Disabilitato"
										    BindingField-Value="N"></ondp:wzCheckBox>
                                    </td>
                                    <td class="label" align="right">Ordine</td>
								    <td>
									    <ondp:wzTextBox id="WsTbOrdAss" runat="server" MaxLength="2" width="100%"
										    BindingField-SourceField="ASS_ORDINE" BindingField-Hidden="False" BindingField-SourceTable="T_ANA_ASSOCIAZIONI"
										    BindingField-Connection="assConnDati" BindingField-Editable="always" CssStyles-CssRequired="TextBox_Numerico_Obbligatorio"
										    CssStyles-CssEnabled="TextBox_Numerico" CssStyles-CssDisabled="TextBox_Numerico_Disabilitato"></ondp:wzTextBox>
								    </td>
							    </tr>
							    <tr>
								    <td class="label">Stampa</td>
								    <td>
									    <ondp:wzTextBox id="WzTextBox2" runat="server" MaxLength="16" BindingField-SourceField="ASS_STAMPA"
										    BindingField-Hidden="False" BindingField-SourceTable="T_ANA_ASSOCIAZIONI" BindingField-Connection="assConnDati"
										    BindingField-Editable="always" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p" CssStyles-CssEnabled="textbox_stringa w100p"
										    CssStyles-CssDisabled="textbox_stringa_disabilitato w100p"></ondp:wzTextBox>
								    </td>								    
                                    <td class="label">Def. stampa</td>
                                    <td>
                                        <ondp:wzCheckBox id="chkDefStampa" runat="server" Height="12px" BindingField-SourceField="ASS_DEF_STAMPA"
										    BindingField-Hidden="False" BindingField-SourceTable="T_ANA_ASSOCIAZIONI" BindingField-Connection="assConnDati"
										    BindingField-Editable="always" CssStyles-CssEnabled="Label" CssStyles-CssDisabled="Label_Disabilitato"
										    BindingField-Value="N"></ondp:wzCheckBox>
                                    </td>
                                    <td class="label" colspan="3">Anti-Pneumococcica</td>
                                    <td>
                                        <ondp:wzCheckBox id="chkAntiPneumo" runat="server" Height="12px" BindingField-SourceField="ASS_ANTI_PNEUMOCOCCO"
										    BindingField-Hidden="False" BindingField-SourceTable="T_ANA_ASSOCIAZIONI" BindingField-Connection="assConnDati"
										    BindingField-Editable="always" CssStyles-CssEnabled="Label" CssStyles-CssDisabled="Label_Disabilitato"
										    BindingField-Value="N"></ondp:wzCheckBox>
                                    </td>
                                    <td class="label">Anti-Influenzale</td>
                                    <td>
                                        <ondp:wzCheckBox id="chkAntiInfluenzale" runat="server" Height="12px" BindingField-SourceField="ASS_ANTI_INFLUENZALE"
										    BindingField-Hidden="False" BindingField-SourceTable="T_ANA_ASSOCIAZIONI" BindingField-Connection="assConnDati"
										    BindingField-Editable="always" CssStyles-CssEnabled="Label" CssStyles-CssDisabled="Label_Disabilitato"
										    BindingField-Value="N"></ondp:wzCheckBox>
                                    </td>
							    </tr>
                                <tr>
								    <td class="label">
									    <asp:Label id="lblSito" runat="server" >Sito Inoculazione</asp:Label></td>
                                    <td>
									    <ondp:wzFinestraModale id="fmSitoInoculazione" runat="server" Width="80%" Tabella="T_ANA_SITI_INOCULAZIONE" CampoCodice="SII_CODICE"
										    CampoDescrizione="SII_DESCRIZIONE" BindingDescription-SourceField="SII_DESCRIZIONE" BindingDescription-SourceTable="T_ANA_SITI_INOCULAZIONE"
										    BindingDescription-Connection="assConnDati" BindingDescription-Hidden="False" BindingDescription-Editable="always"
										    Obbligatorio="False" SetUpperCase="True" RaiseChangeEvent="False" UseCode="True" BindingCode-SourceField="ASS_SII_CODICE"
										    BindingCode-Hidden="False" BindingCode-SourceTable="T_ANA_ASSOCIAZIONI" BindingCode-Connection="assConnDati"
										    BindingCode-Editable="always" CodiceWidth="20%" LabelWidth="-8px" PosizionamentoFacile="False" Filtro="1=1 order by SII_DESCRIZIONE" UseTableLayout="true"></ondp:wzFinestraModale>
                                    </td>
                                    <td class="label">
                                        <asp:Label id="lblVia" runat="server" >Via Somministrazione</asp:Label></td>
                                    <td colspan="7">
									    <ondp:wzFinestraModale id="fmViaSomministrazione" runat="server" Width="80%" Tabella="T_ANA_VIE_SOMMINISTRAZIONE" CampoCodice="VII_CODICE"
										    CampoDescrizione="VII_DESCRIZIONE" BindingDescription-SourceField="VII_DESCRIZIONE" BindingDescription-SourceTable="T_ANA_VIE_SOMMINISTRAZIONE"
										    BindingDescription-Connection="assConnDati" BindingDescription-Hidden="False" BindingDescription-Editable="always"
										    Obbligatorio="False" SetUpperCase="True" RaiseChangeEvent="False" UseCode="True" BindingCode-SourceField="ASS_VII_CODICE"
										    BindingCode-Hidden="False" BindingCode-SourceTable="T_ANA_ASSOCIAZIONI" BindingCode-Connection="assConnDati"
										    BindingCode-Editable="always" CodiceWidth="20%" LabelWidth="-8px" PosizionamentoFacile="False" Filtro="1=1 order by VII_DESCRIZIONE" UseTableLayout="true"></ondp:wzFinestraModale></td>
                                </tr>
						    </table>
					    </ondp:OnitDataPanel>
                    </div>
				</ondp:OnitDataPanel>
			</on_lay3:onitlayout3>
		</form>
	</body>
</html>
