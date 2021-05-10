<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Consultori.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_Consultori" %>

<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>Centri Vaccinali</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/archivi.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("onvac.common.js")%>'></script>
    <script type="text/javascript">
        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }
        
        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;
        }
    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Width="100%" Height="100%" Titolo="Centri Vaccinali" WindowNoFrames="False" TitleCssClass="Title3">
            <ondp:onitdatapanel id="odpConsultoriMaster" runat="server" defaultsort-length="44" width="100%" configfile="Consultori.odpConsultoriMaster.xml" 
                height="100%" externaltoolbar-length="0" renderonlychildren="True" usetoolbar="False" defaultsort="ConsultoriMaster.t_ana_consultori.cns_codice">
                <div>
				    <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                        <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                        <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		                <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                        <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
					    <Items>
						    <igtbar:TBarButton Key="btnCerca" Text="Cerca" Image="~/Images/cerca.gif" DisabledImage="~/Images/cerca_dis.gif"></igtbar:TBarButton>
						    <igtbar:TBarButton Key="btnNew" Text="Nuovo" Image="~/Images/nuovo.gif" DisabledImage="~/Images/nuovo_dis.gif" ></igtbar:TBarButton>
						    <igtbar:TBarButton Key="btnEdit" Text="Modifica" Image="~/Images/modifica.gif" DisabledImage="~/Images/modifica_dis.gif"></igtbar:TBarButton>
						    <igtbar:TBarButton Key="btnSalva" Text="Salva" Image="~/Images/salva.gif" DisabledImage="~/Images/salva_dis.gif"></igtbar:TBarButton>
						    <igtbar:TBarButton Key="btnAnnulla" Text="Annulla" Image="~/Images/annulla.gif" DisabledImage="~/Images/annulla_dis.gif"></igtbar:TBarButton>
						    <igtbar:TBarButton Key="btnLinkAssComuni" Text="Comuni" Image="../../../images/consultorio.gif"></igtbar:TBarButton>
						    <igtbar:TBarButton Key="btnLinkAssCircoscrizioni" Text="Circoscr." Image="../../../images/consultorio.gif"></igtbar:TBarButton>
						    <igtbar:TBarButton Key="btnLinkParametri" Text="Parametri" Image="../../../images/parametri.gif"></igtbar:TBarButton>
                            <igtbar:TBarButton Key="btnLinkUtentiAbilitati" Text="Utenti Abilitati" Image="../../../images/gruppi.gif">
                                <DefaultStyle CssClass="infratoolbar_button_default" Width="120px"></DefaultStyle> 
                            </igtbar:TBarButton>
					    </Items>
				    </igtbar:UltraWebToolbar>
                </div>
                <div>
				    <ondp:wzFilter id="filFiltro" runat="server" Width="100%" CssClass="InfraUltraWebTab2">
					    <SelectedTabStyle CssClass="InfraTab_Selected2"></SelectedTabStyle>
					    <HoverTabStyle CssClass="InfraTab_Hover2"></HoverTabStyle>
					    <DefaultTabStyle CssClass="InfraTab_Default2"></DefaultTabStyle>
                        <DisabledTabStyle CssClass="InfraTab_Disabled2"></DisabledTabStyle>
					    <Tabs>
						    <igtab:Tab Text="Ricerca di Base">
							    <ContentTemplate>
								    <table style="table-layout: fixed" height="100%" cellspacing="10" cellpadding="0" width="100%" border="0">
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
            
                <div class="vac-sezione">Elenco centri vaccinali</div>

                <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                    <ondp:wzMsDataGrid id="dgrConsultoriMaster" runat="server" Width="100%" OnitStyle="False" EditMode="None"
	                    PagerVoicesAfter="-1" PagerVoicesBefore="-1" AutoGenerateColumns="False" SelectionOption="rowClick">
	                    <HeaderStyle CssClass="header"></HeaderStyle>
	                    <ItemStyle CssClass="item"></ItemStyle>
	                    <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
	                    <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
	                    <EditItemStyle CssClass="edit"></EditItemStyle>
	                    <FooterStyle CssClass="footer"></FooterStyle>
	                    <PagerStyle CssClass="pager"></PagerStyle>	
	                    <Columns>
		                    <ondp:wzBoundColumn HeaderText="Codice" Key="CNS_CODICE" SourceField="CNS_CODICE" SourceTable="T_ANA_CONSULTORI" SourceConn="ConsultoriMaster">
                                <HeaderStyle width="15%" />
		                    </ondp:wzBoundColumn>
		                    <ondp:wzBoundColumn HeaderText="Descrizione" Key="CNS_DESCRIZIONE" SourceField="CNS_DESCRIZIONE" SourceTable="T_ANA_CONSULTORI" SourceConn="ConsultoriMaster">
		                        <HeaderStyle width="25%" />
                            </ondp:wzBoundColumn>
		                    <ondp:wzBoundColumn HeaderText="Comune" Key="COM_DESCRIZIONE" SourceField="COM_DESCRIZIONE" SourceTable="T_ANA_COMUNI" SourceConn="ConsultoriMaster">
                                <HeaderStyle width="20%" />
		                    </ondp:wzBoundColumn>
		                    <ondp:wzBoundColumn HeaderText="Indirizzo" Key="CNS_INDIRIZZO" SourceField="CNS_INDIRIZZO" SourceTable="T_ANA_CONSULTORI" SourceConn="ConsultoriMaster">
                                <HeaderStyle width="25%" />
		                    </ondp:wzBoundColumn>
		                    <ondp:wzBoundColumn HeaderText="Telefono" Key="CNS_N_TELEFONO" SourceField="CNS_N_TELEFONO" SourceTable="T_ANA_CONSULTORI" SourceConn="ConsultoriMaster">
                                <HeaderStyle width="15%" />
		                    </ondp:wzBoundColumn>
	                    </Columns>
	                    <BindingColumns>
		                    <ondp:BindingFieldValue Value="" Editable="onNew" Description="Codice" Connection="ConsultoriMaster" 
			                    SourceTable="T_ANA_CONSULTORI" SourceField="CNS_CODICE" Hidden="False"></ondp:BindingFieldValue>
		                    <ondp:BindingFieldValue Value="" Editable="always" Description="Descrizione" Connection="ConsultoriMaster"
			                    SourceTable="T_ANA_CONSULTORI" SourceField="CNS_DESCRIZIONE" Hidden="False"></ondp:BindingFieldValue>
		                    <ondp:BindingFieldValue Value="" Editable="always" Description="Comune" Connection="ConsultoriMaster" 
			                    SourceTable="T_ANA_COMUNI" SourceField="COM_DESCRIZIONE" Hidden="False"></ondp:BindingFieldValue>
		                    <ondp:BindingFieldValue Value="" Editable="always" Description="Indirizzo" Connection="ConsultoriMaster"
			                    SourceTable="T_ANA_CONSULTORI" SourceField="CNS_INDIRIZZO" Hidden="False"></ondp:BindingFieldValue>
		                    <ondp:BindingFieldValue Value="" Editable="always" Description="Numero telefono" Connection="ConsultoriMaster"
			                    SourceTable="T_ANA_CONSULTORI" SourceField="CNS_N_TELEFONO" Hidden="False"></ondp:BindingFieldValue>
	                    </BindingColumns>
                    </ondp:wzMsDataGrid>
                </dyp:DynamicPanel>

                <div class="vac-sezione">Dettaglio</div>
                <div>
				    <ondp:OnitDataPanel id="odpConsultoriDetail" runat="server" Width="100%" ConfigFile="Consultori.odpConsultoriDetail.xml"
					    externalToolBar-Length="7" renderOnlyChildren="True" useToolbar="False" dontLoadDataFirstTime="True"
					    externalToolBar="ToolBar">
					    <table style="table-layout: fixed" cellspacing="1" width="100%" border="0">
                            <colgroup>
                                <col width="9%" />
                                <col width="15%" />
                                <col width="11%" />
                                <col width="15%" />
                                <col width="9%" />
                                <col width="26%" />
                                <col width="12%" />
                                <col width="3%" />
                            </colgroup>
						    <tr>
							    <td class="label">Codice</td>
							    <td colspan="3">
								    <ondp:wzTextBox id="txtCnsCodice" onblur="toUpper(this);controlloCampoCodice(this);" runat="server" Width="100%" 
									    CssStyles-CssRequired="TextBox_Stringa_Obbligatorio" CssStyles-CssEnabled="TextBox_Stringa"
									    CssStyles-CssDisabled="TextBox_Stringa_Disabilitato" BindingField-SourceField="CNS_CODICE"
									    BindingField-Hidden="False" BindingField-SourceTable="T_ANA_CONSULTORI" BindingField-Connection="ConsultoriDettaglio"
									    BindingField-Editable="onNew" MaxLength="8"></ondp:wzTextBox></td>
							    <td class="label">Cod. esterno</td>
							    <td colspan="3">
								    <ondp:wzTextBox id="WzCnsCodiceEsterno" onblur="toUpper(this);"  runat="server" Width="99%" 
									    CssStyles-CssRequired="TextBox_Stringa" CssStyles-CssEnabled="TextBox_Stringa"
									    CssStyles-CssDisabled="TextBox_Stringa_Disabilitato" BindingField-SourceField="CNS_CODICE_ESTERNO"
									    BindingField-Hidden="False" BindingField-SourceTable="T_ANA_CONSULTORI" BindingField-Connection="ConsultoriDettaglio"
									    BindingField-Editable="always" MaxLength="10"></ondp:wzTextBox></td>
						    </tr>
						    <tr>
							    <td class="label">Descrizione</td>
							    <td colspan="3">
								    <ondp:wzTextBox id="WzTextBox2" onblur="toUpper(this);"  runat="server" Width="100%" 
									    CssStyles-CssRequired="TextBox_Stringa_Obbligatorio" CssStyles-CssEnabled="TextBox_Stringa"
									    CssStyles-CssDisabled="TextBox_Stringa_Disabilitato" BindingField-SourceField="CNS_DESCRIZIONE"
									    BindingField-Hidden="False" BindingField-SourceTable="T_ANA_CONSULTORI" BindingField-Connection="ConsultoriDettaglio"
									    BindingField-Editable="always" MaxLength="30"></ondp:wzTextBox></td>
							    <td class="label">Comune</td>
							    <td colspan="3">
								    <ondp:wzFinestraModale id="WzFinestraModale1" runat="server" Width="69%" PosizionamentoFacile="False" LabelWidth="-1px"
									    CodiceWidth="30%" BindingCode-Editable="always" BindingCode-Connection="ConsultoriDettaglio" BindingCode-SourceTable="T_ANA_CONSULTORI"
									    BindingCode-Hidden="False" BindingCode-SourceField="CNS_COM_CODICE" UseCode="True" RaiseChangeEvent="False"
									    SetUpperCase="True" Obbligatorio="False" BindingDescription-Editable="always" BindingDescription-Hidden="False"
									    BindingDescription-Connection="ConsultoriDettaglio" BindingDescription-SourceTable="T_ANA_COMUNI" BindingDescription-SourceField="COM_DESCRIZIONE"
									    CampoCodice="COM_CODICE" CampoDescrizione="COM_DESCRIZIONE" Tabella="T_ANA_COMUNI"></ondp:wzFinestraModale></td>
						    </tr>
						    <tr>
							    <td class="label">Indirizzo</td>
							    <td colspan="3">
								    <ondp:wzTextBox id="WzTextBox5" onblur="toUpper(this);"  runat="server" Width="100%" 
									    CssStyles-CssRequired="TextBox_Stringa_Obbligatorio" CssStyles-CssEnabled="TextBox_Stringa"
									    CssStyles-CssDisabled="TextBox_Stringa_Disabilitato" BindingField-SourceField="CNS_INDIRIZZO"
									    BindingField-Hidden="False" BindingField-SourceTable="T_ANA_CONSULTORI" BindingField-Connection="ConsultoriDettaglio"
									    BindingField-Editable="always" MS_POSITIONING="GridLayout" MaxLength="100"></ondp:wzTextBox></td>
							    <td class="label">Distretto</td>
							    <td colspan="3">
								    <ondp:wzFinestraModale id="Wzfinestramodale3" runat="server" Width="69%" PosizionamentoFacile="False" LabelWidth="-1px"
									    CodiceWidth="30%" BindingCode-Editable="always" BindingCode-Connection="ConsultoriDettaglio" BindingCode-SourceTable="T_ANA_CONSULTORI"
									    BindingCode-Hidden="False" BindingCode-SourceField="CNS_DIS_CODICE" UseCode="True" RaiseChangeEvent="False"
									    SetUpperCase="True" Obbligatorio="False" BindingDescription-Editable="always" BindingDescription-Hidden="False"
									    BindingDescription-Connection="ConsultoriDettaglio" BindingDescription-SourceTable="T_ANA_DISTRETTI"
									    BindingDescription-SourceField="DIS_DESCRIZIONE" CampoCodice="DIS_CODICE" CampoDescrizione="DIS_DESCRIZIONE"
									    Tabella="T_ANA_DISTRETTI"></ondp:wzFinestraModale></td>
						    </tr>
						    <tr>
							    <td class="label">Telefono</td>
							    <td>
								    <ondp:wzTextBox id="WzTextBox4" onblur="toUpper(this);"  runat="server" Width="91%" 
									    CssStyles-CssRequired="TextBox_Stringa_Obbligatorio" CssStyles-CssEnabled="TextBox_Stringa"
									    CssStyles-CssDisabled="TextBox_Stringa_Disabilitato" BindingField-SourceField="CNS_N_TELEFONO"
									    BindingField-Hidden="False" BindingField-SourceTable="T_ANA_CONSULTORI" BindingField-Connection="ConsultoriDettaglio"
									    BindingField-Editable="always" MaxLength="50"></ondp:wzTextBox></td>
                                <td class="label">Codice AVN</td>
                                <td>
                                    <ondp:wzTextBox id="WzTxtAvn" runat="server" Width="100%" MaxLength="8" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p"
										CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p"
										BindingField-Editable="always" BindingField-Connection="connessione" BindingField-SourceTable="T_ANA_CONSULTORI"
										BindingField-Hidden="False" BindingField-SourceField="CNS_CODICE_AVN"></ondp:wzTextBox></td>
                                <td class="label">Email</td>
                                <td colspan="3">
                                    <ondp:wzTextBox id="txtMail" runat="server" Width="99%"
									    CssStyles-CssRequired="TextBox_Stringa_Obbligatorio" CssStyles-CssEnabled="TextBox_Stringa"
									    CssStyles-CssDisabled="TextBox_Stringa_Disabilitato" BindingField-SourceField="CNS_EMAIL"
									    BindingField-Hidden="False" BindingField-SourceTable="T_ANA_CONSULTORI" BindingField-Connection="ConsultoriDettaglio"
									    BindingField-Editable="always" MaxLength="100"></ondp:wzTextBox></td>
						    </tr>
						    <tr>
							    <td class="label">Stampa 1</td>
							    <td colspan="7">
								    <ondp:wzTextBox id="txtStampa1" runat="server" Width="99.5%" 
									    CssStyles-CssRequired="TextBox_Stringa_Obbligatorio" CssStyles-CssEnabled="TextBox_Stringa"
									    CssStyles-CssDisabled="TextBox_Stringa_Disabilitato" BindingField-SourceField="CNS_STAMPA1"
									    BindingField-Hidden="False" BindingField-SourceTable="T_ANA_CONSULTORI" BindingField-Connection="ConsultoriDettaglio"
									    BindingField-Editable="always" MaxLength="256" TextMode="MultiLine"></ondp:wzTextBox></td>
						    </tr>
						    <tr>
							    <td class="label">Stampa 2</td>
							    <td colSpan="7">
								    <ondp:wzTextBox id="txtStampa2" runat="server" Width="99.5%" 
									    CssStyles-CssRequired="TextBox_Stringa_Obbligatorio" CssStyles-CssEnabled="TextBox_Stringa"
									    CssStyles-CssDisabled="TextBox_Stringa_Disabilitato" BindingField-SourceField="CNS_STAMPA2"
									    BindingField-Hidden="False" BindingField-SourceTable="T_ANA_CONSULTORI" BindingField-Connection="ConsultoriDettaglio"
									    BindingField-Editable="always" MaxLength="500" TextMode="MultiLine"></ondp:wzTextBox></td>
						    </tr>
                            <tr>
                                <td class="label">Orari Reperibilità</td>
							    <td colSpan="7">
								    <ondp:wzTextBox id="txtOrariReperibilita" runat="server" Width="99.5%" 
									    CssStyles-CssRequired="TextBox_Stringa_Obbligatorio" CssStyles-CssEnabled="TextBox_Stringa"
									    CssStyles-CssDisabled="TextBox_Stringa_Disabilitato" BindingField-SourceField="CNS_ORARI_REPERIBILITA"
									    BindingField-Hidden="False" BindingField-SourceTable="T_ANA_CONSULTORI" BindingField-Connection="ConsultoriDettaglio"
									    BindingField-Editable="always" MaxLength="1000" TextMode="MultiLine"></ondp:wzTextBox></td>
                            </tr>
						    <tr>
							    <td class="label">Paz. da età:</td>
							    <td colspan="2" align="left">
                                    <table id="Table3" cellspacing="0" cellpadding="0" width="100%" border="0">
                                        <colgroup>
                                            <col width="40px" />
                                            <col />
                                            <col width="40px" />
                                            <col />
                                            <col width="40px" />
                                            <col />
                                        </colgroup>
									    <tr>
										    <td align="left">
											    <asp:TextBox id="tbInizioAnni" runat="server" CssClass="textbox_numerico_disabilitato" MaxLength="3" Width="100%"
												    ReadOnly="True"></asp:TextBox></td>
										    <td>
											    <asp:Label id="Label14" style="padding-right: 10px; padding-left: 3px" runat="server" CssClass="label">Anni</asp:Label></td>
										    <td align="right">
											    <asp:TextBox id="tbInizioMesi" runat="server" CssClass="textbox_numerico_disabilitato" MaxLength="2" Width="100%"
												    ReadOnly="True"></asp:TextBox></td>
										    <td>
											    <asp:Label id="Label15" style="padding-right: 10px; padding-left: 3px" runat="server" CssClass="label">Mesi</asp:Label></td>
										    <td align="right">
											    <asp:TextBox id="tbInizioGiorni" runat="server" CssClass="textbox_numerico_disabilitato" MaxLength="3" Width="100%"
												    ReadOnly="True"></asp:TextBox>
											    <ondp:wzTextBox id="wzTbEtaInizio" runat="server" Width="40px" CssStyles-CssRequired="textbox_numerico_obbligatorio"
												    CssStyles-CssEnabled="textbox_numerico" BindingField-SourceField="CNS_DA_ETA" BindingField-Hidden="False"
												    BindingField-SourceTable="T_ANA_CONSULTORI" BindingField-Connection="ConsultoriDettaglio" BindingField-Editable="always"
												    Visible="False"></ondp:wzTextBox></td>
										    <td>
											    <asp:Label id="Label16" style="padding-right: 3px; padding-left: 3px" runat="server" CssClass="label">Giorni</asp:Label></td>
									    </tr>
								    </table>
							    </td>
                                <td></td>
							    <td class="label">ad età:</td>
							    <td align="left">
								    <table id="Table4" cellspacing="0" cellpadding="0" width="100%" border="0">
                                        <colgroup>
                                            <col width="40px" />
                                            <col />
                                            <col width="40px" />
                                            <col />
                                            <col width="40px" />
                                            <col />
                                        </colgroup>
									    <tr>
										    <td align="left">
											    <asp:TextBox id="tbFineAnni" runat="server" CssClass="textbox_numerico_disabilitato" MaxLength="3" width="100%"
                                                    ReadOnly="True"></asp:TextBox></td>
										    <td>
											    <asp:Label id="Label11" style="padding-right: 10px; padding-left: 3px" runat="server" CssClass="label">Anni</asp:Label></td>
										    <td align="right">
											    <asp:TextBox id="tbFineMesi" runat="server" CssClass="textbox_numerico_disabilitato" MaxLength="2" width="100%"
                                                    ReadOnly="True"></asp:TextBox></td>
										    <td>
											    <asp:Label id="Label12" style="padding-right: 10px; padding-left: 3px" runat="server" CssClass="label">Mesi</asp:Label></td>
										    <td align="right">
											    <asp:TextBox id="tbFineGiorni" runat="server" CssClass="textbox_numerico_disabilitato" MaxLength="3" width="100%"
												    ReadOnly="True"></asp:TextBox>
											    <ondp:wzTextBox id="wzTbEtaFine" runat="server" Width="40px" BindingField-SourceField="CNS_A_ETA"
												    BindingField-Hidden="False" BindingField-SourceTable="T_ANA_CONSULTORI" BindingField-Connection="ConsultoriDettaglio"
												    BindingField-Editable="always" Visible="False"></ondp:wzTextBox></td>
										    <td>
											    <asp:Label id="Label13" style="padding-right: 3px; padding-left: 3px" runat="server" CssClass="label">Giorni</asp:Label></td>
									    </tr>
								    </table>
							    </td>
                                <td class="label">Smistamento</td>
                                <td>
                                    <ondp:wzCheckBox id="chkSmistamento" runat="server" CssStyles-CssEnabled="Label" CssStyles-CssDisabled="Label_Disabilitato"
                                        BindingField-SourceField="cns_smistamento" BindingField-Hidden="False" BindingField-SourceTable="T_ANA_CONSULTORI"
									    BindingField-Connection="ConsultoriDettaglio" BindingField-Editable="always" BindingField-Value="N"></ondp:wzCheckBox></td>
						    </tr>
						    <tr>
							    <td class="label">Tipo</td>
							    <td colspan="3">
								    <ondp:wzDropDownList id="ddlTipoCns" runat="server" Width="100%" CssStyles-CssRequired="TextBox_Stringa"
									    CssStyles-CssEnabled="TextBox_Stringa" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato" BindingField-SourceField="CNS_TIPO"
									    BindingField-Hidden="False" BindingField-SourceTable="T_ANA_CONSULTORI" BindingField-Connection="ConsultoriDettaglio"
									    BindingField-Editable="always" AutoPostBack="True">
									    <asp:ListItem Selected="True"></asp:ListItem>
									    <asp:ListItem Value="A">Centro Vaccinale Adulti</asp:ListItem>
									    <asp:ListItem Value="P">Centro Vaccinale Pediatrico</asp:ListItem>
									    <asp:ListItem Value="V">Pediatra Vaccinatore</asp:ListItem>
								    </ondp:wzDropDownList></td>
							    <td align="right">
								    <asp:Label id="lblPediatraVac" style="padding-right: 3px; padding-left: 3px" runat="server"
									    CssClass="label">Pediatra vac.</asp:Label></td>
							    <td colspan="3">
								    <ondp:wzFinestraModale id="fmPediatraVac" runat="server" Width="69%" PosizionamentoFacile="False" LabelWidth="-1px"
									    CodiceWidth="30%" BindingCode-Editable="always" BindingCode-Connection="ConsultoriDettaglio" BindingCode-SourceTable="T_ANA_CONSULTORI"
									    BindingCode-Hidden="False" BindingCode-SourceField="CNS_PEDIATRA_VACCINATORE" UseCode="True" RaiseChangeEvent="False"
									    SetUpperCase="True" Obbligatorio="False" BindingDescription-Editable="always" BindingDescription-Hidden="False"
									    BindingDescription-Connection="ConsultoriDettaglio" BindingDescription-SourceTable="T_ANA_MEDICI" BindingDescription-SourceField="MED_DESCRIZIONE"
									    CampoCodice="MED_CODICE" CampoDescrizione="MED_DESCRIZIONE" Tabella="T_ANA_MEDICI"></ondp:wzFinestraModale></td>
						    </tr>
						    <tr>
							    <td class="label">Apertura</td>
                                <td>
                                    <ondp:wzOnitDatePick id="dpkApertura" runat="server" Width="100%" CssStyles-CssRequired="textbox_data_obbligatorio"
									    CssStyles-CssEnabled="textbox_data" CssStyles-CssDisabled="textbox_data_disabilitato" BindingField-SourceField="cns_data_apertura"
									    BindingField-Hidden="False" BindingField-SourceTable="T_ANA_CONSULTORI" BindingField-Connection="ConsultoriDettaglio"
									    BindingField-Editable="always" target="dpkApertura"></ondp:wzOnitDatePick></td>
                                <td class="label">Chiusura</td>
                                <td>
                                    <ondp:wzOnitDatePick id="dpkChiusura" runat="server" Width="100%" CssStyles-CssRequired="textbox_data_obbligatorio"
									    CssStyles-CssEnabled="textbox_data" CssStyles-CssDisabled="textbox_data_disabilitato" BindingField-SourceField="cns_data_chiusura"
									    BindingField-Hidden="False" BindingField-SourceTable="T_ANA_CONSULTORI" BindingField-Connection="ConsultoriDettaglio"
									    BindingField-Editable="always" target="dpkChiusura"></ondp:wzOnitDatePick></td>
                                <td class="label">Magazzino</td>
                                <td colspan="3">
                                    <ondp:wzFinestraModale id="fmMagazzinoConsultorio" runat="server" Width="69%" PosizionamentoFacile="False" LabelWidth="-1px"
									    CodiceWidth="30%" BindingCode-Editable="always" BindingCode-Connection="ConsultoriDettaglio" BindingCode-SourceTable="T_ANA_CONSULTORI"
									    BindingCode-Hidden="False" BindingCode-SourceField="CNS_CNS_MAGAZZINO" UseCode="True" RaiseChangeEvent="True"
									    SetUpperCase="True" Obbligatorio="False" BindingDescription-Editable="always" BindingDescription-Hidden="False"
									    BindingDescription-Connection="ConsultoriDettaglio" BindingDescription-SourceTable="T_ANA_CNS_MAGAZZINO"
									    BindingDescription-SourceField="CNS_DESCRIZIONE" CampoCodice="CNS_CODICE" CampoDescrizione="CNS_DESCRIZIONE"
									    Tabella="T_ANA_CONSULTORI" DataTypeCode="Stringa" IsDistinct="False" DataTypeDescription="Stringa"></ondp:wzFinestraModale></td>
						    </tr>
                            <tr>
                                <td class="label">Tipo Erogatore</td>
                                <td>
                                    <ondp:wzTextBox id="tbTipoErogatore" runat="server" Width="100%" MaxLength="2" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p"
										CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p"
										BindingField-Editable="always" BindingField-Connection="connessione" BindingField-SourceTable="T_ANA_CONSULTORI"
										BindingField-Hidden="False" BindingField-SourceField="CNS_TIPO_EROGATORE"></ondp:wzTextBox>
                                </td>
                                <td colspan="3" class="label">
                                    Richiesta consenso trattamento dati operatore
                                </td>
                                <td colspan="3">
                                    <ondp:wzCheckBox id="chkConsensoTrattDatiOper" runat="server" CssStyles-CssEnabled="Label" CssStyles-CssDisabled="Label_Disabilitato"
                                        BindingField-SourceField="CNS_CONSENSO_TRATTAM_DATI_OPER" BindingField-Hidden="False" BindingField-SourceTable="T_ANA_CONSULTORI"
									    BindingField-Connection="ConsultoriDettaglio" BindingField-Editable="always" BindingField-Value="N"></ondp:wzCheckBox></td>
                            </tr>
					    </table>
				    </ondp:OnitDataPanel>
                </div>

			</ondp:onitdatapanel>
        </on_lay3:OnitLayout3>
    </form>
</body>
</html>
