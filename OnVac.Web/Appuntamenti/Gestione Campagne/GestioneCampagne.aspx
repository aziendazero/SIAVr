<%@ Page Language="vb" AutoEventWireup="false" Codebehind="GestioneCampagne.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.GestioneCampagne"%>

<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel"  %>
<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>

<%@ Register TagPrefix="uc1" TagName="MotivoEsc" Src="../../Common/Controls/MotivoEsc.ascx" %>
<%@ Register TagPrefix="uc2" TagName="StatiAnagrafici" Src="../../Common/Controls/StatiAnagrafici.ascx" %>
<%@ Register TagPrefix="uc3" TagName="SelezioneConsultori" Src="../../Common/Controls/SelezioneConsultori.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>Gestione Campagne Vaccinali</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

        <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
		<script type="text/javascript" language="javascript">

		$(document).ready(function () {
		   
		    // Imposta l'altezza uguale nelle table nei due fieldset di ricerca
		    try {
		        var max = 0;
		        $('[id=tblDataNascita],[id=tblcwr]').each(function () {
		            max = Math.max($(this).height(), max);
		        }).height(max);
		    } catch (e) {
		        // 
		    }
		});

		function InizializzaToolBar(t)
		{
			t.PostBackButton=false;
		}

		function ToolBarClick(ToolBar, button, evnt)
        {
			evnt.needPostBack=true;
			switch(button.Key)
			{
			    case 'btnCerca':
			        var dataNascitaIniz = OnitDataPickGet('odpDataNascitaIniz');
			        var dataNascitaFin = OnitDataPickGet('odpDataNascitaFin');
                    
			        var msg = '';

			        if ('<%= Me.ucSelezioneConsultori.GetConsultoriSelezionati(False) %>' == '') {
			            msg += '\n- selezionare almeno un centro vaccinale;';
			        }

			        if (((dataNascitaIniz == '') || (dataNascitaFin == '')) && (isValidFinestraModale('omlCategorieRischio', true) == false) && (isValidFinestraModale('omlMalattia', true) == false)) {
			            msg += "\n- inserire gli estremi della data di nascita, la categoria a rischio o la malattia;";
			        }

			        if (msg != '') {
			            alert('Ricerca non effettuata:\n' + msg + '\n\nCompletare i campi indicati e riprovare.');
			            evnt.needPostBack = false;
			        }

			        break;

			    case 'btnCrea':
			        if ((isValidFinestraModale('omlAss', true) == false) || (OnitDataPickGet('odpDataCNV') == '')) {
			            alert("Inserire l'associazione e la data in cui creare la convocazione!");
			            evnt.needPostBack = false;
			            return;
			        }
			        else {
			            if (confirm("Procedendo verranno create le convocazioni. Continuare?") == false) {
			                evnt.needPostBack = false;
			                return;
			            }
			        }
			        break;
			}
		}

		function selezionaPazienti(chk)
        {
			__doPostBack('selPazienti', chk);
		}
		
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" height="100%" width="100%" Titolo="Gestione Campagne Vaccinali" TitleCssClass="Title3">

				<div id="PanelTitolo" class="title" runat="server">
					<asp:Label id="LayoutTitolo" runat="server"> Gestione Campagne Vaccinali </asp:Label>
                </div>
                <div>
					<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
					    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                        <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
					    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
						<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
                        <Items>
							<igtbar:TBarButton Key="btnCerca" Text="Cerca Pazienti" DisabledImage="~/Images/cerca_dis.gif"
								Image="~/Images/cerca.gif">
								<DefaultStyle Width="120px" CssClass="infratoolbar_button_default"></DefaultStyle>
							</igtbar:TBarButton>
							<igtbar:TBSeparator></igtbar:TBSeparator>
							<igtbar:TBarButton Key="btnCrea" Text="Crea Convocazioni" DisabledImage="../../images/rotella_dis.gif"
								Image="../../images/rotella.gif">
								<DefaultStyle Width="140px" CssClass="infratoolbar_button_default"></DefaultStyle>
							</igtbar:TBarButton>
						</Items>
					</igtbar:UltraWebToolbar>
                </div>
				<div id="Panel23" class="sezione" runat="server">
					<asp:Label id="lblTitoloRicerca" runat="server"></asp:Label>
                </div>
                <div>
					<table width="100%">
						<tr>
							<td width="60%">
								<table id="tblext" border="0" cellspacing="0" cellpadding="0" width="100%" height="100%">
									<tr height="2">
										<td></td>
										<td></td>
									</tr>
									<tr>
										<td colspan="2">
											<fieldset class="fldroot" >
							                    <legend class="label">Ricerca Convocati</legend>
												<table style="margin-top: 2px" id="tblDataNascita" class="label_left" border="0" cellspacing="0" cellpadding="2" width="100%">
													<colgroup>
														<col width="2%" />
														<col width="3%" />
														<col width="21%" />
														<col width="3%" />
														<col width="21%" />
														<col width="1%" />
														<col width="15%" />
														<col width="1%" />
														<col width="15%" />
														<col width="1%" />
														<col width="15%" />
														<col width="2%" />
													</colgroup>
													<tr>
														<td></td>
														<td class="sezione" colspan="4">Centri Vaccinali di ricerca</td>
														<td></td>
														<td class="sezione" colspan="5">Comune</td>
														<td></td>
													</tr>
													<tr>
														<td></td>
														<td class="label_right" colspan="4">
                                                            <uc3:SelezioneConsultori ID="ucSelezioneConsultori" runat="server" MaxCnsInListaSelezionati="3" ImpostaCnsCorrente="true" />
                                                        </td>
														<td></td>
														<td colspan="5">
															<on_ofm:onitmodallist id="fmComune" runat="server" SetUpperCase="True" UseCode="True" UseTableLayout="True"
																Label="Comune" CodiceWidth="28%" LabelWidth="-8px" PosizionamentoFacile="False" Tabella="T_ANA_COMUNI"
																CampoCodice="COM_CODICE" CampoDescrizione="COM_DESCRIZIONE" Width="70%" RaiseChangeEvent="True"></on_ofm:onitmodallist></td>
														<td></td>
													</tr>
													<tr>
														<td></td>
														<td class="sezione" colspan="4">Data di nascita</td>
														<td></td>
														<td class="sezione" colspan="5">Sesso</td>
														<td></td>
													</tr>
													<tr>
														<td></td>
														<td class="label">Da:</td>
														<td>
															<on_val:onitdatepick id="odpDataNascitaIniz" runat="server" Height="20px" Width="120px" DateBox="True"
																CssClass="textbox_data"></on_val:onitdatepick></td>
														<td class="label">A:</td>
														<td>
															<on_val:onitdatepick id="odpDataNascitaFin" runat="server" Height="20px" Width="120px" DateBox="True"
																CssClass="textbox_data"></on_val:onitdatepick></td>
														<td></td>
														<td colspan="5">
															<asp:DropDownList id="ddlSesso" Width="50%" Runat="server">
																<asp:ListItem Selected="True"></asp:ListItem>
																<asp:ListItem Value="M">Maschio</asp:ListItem>
																<asp:ListItem Value="F">Femmina</asp:ListItem>
															</asp:DropDownList></td>
														<td></td>
													</tr>
													<tr>
														<td></td>
														<td class="sezione" colspan="4">Categoria a rischio</td>
														<td></td>
														<td class="sezione" colspan="5">Malattia</td>
														<td></td>
													</tr>
													<tr>
														<td></td>
														<td class="label" colspan="4">
															<on_ofm:OnitModalList id="omlCategorieRischio" runat="server" SetUpperCase="True" UseTableLayout="True"
																CodiceWidth="29%" LabelWidth="-1px" PosizionamentoFacile="False" Tabella="T_ANA_RISCHIO" CampoCodice="RSC_CODICE"
																CampoDescrizione="RSC_DESCRIZIONE" Width="70%" DESIGNTIMEDRAGDROP="311"></on_ofm:OnitModalList></td>
														<td></td>
														<td class="label_right" colspan="5">
															<on_ofm:OnitModalList id="omlMalattia" runat="server" SetUpperCase="True" UseTableLayout="True" Label="Titolo"
																CodiceWidth="29%" LabelWidth="-1px" PosizionamentoFacile="False" Tabella="T_ANA_MALATTIE" CampoCodice="MAL_CODICE Codice"
																CampoDescrizione="MAL_DESCRIZIONE Descrizione" Width="70%" DESIGNTIMEDRAGDROP="311" Filtro=" MAL_OBSOLETO='N' ORDER BY MAL_CODICE"></on_ofm:OnitModalList></td>
														<td></td>
													</tr>
													<tr>
														<td></td>
														<td class="sezione" colspan="4">Associazione non eseguita</td>
														<td></td>
														<td class="sezione">Dose</td>
														<td></td>
														<td class="sezione">Mesi</td>
														<td></td>
														<td class="sezione">Anni</td>
														<td></td>
													</tr>
													<tr>
														<td></td>
														<td class="label" colspan="4">
															<on_ofm:OnitModalList id="omlAssNonEs" runat="server" Obbligatorio="False" SetUpperCase="True" UseCode="True"
																UseTableLayout="True" Label="Titolo" CodiceWidth="29%" LabelWidth="-1px" PosizionamentoFacile="False" Width="70%"																		
																RaiseChangeEvent="True"></on_ofm:OnitModalList>
                                                        </td>
														<td></td>
														<td class="label_right">
															<on_val:OnitJsValidator id="txtDose" runat="server" CssClass="textbox_numerico_disabilitato w100p" ReadOnly="True"
																actionCorrect="False" actionDelete="False" actionFocus="True" actionMessage="True" actionSelect="False"
																actionUndo="True" autoFormat="True" validationType="Validate_custom" CustomValFunction-Length="2" CustomValFunction="validaNumero"
																SetOnChange="True" MaxLength="2"></on_val:OnitJsValidator>
                                                        </td>
														<td></td>
														<td class="label_right">
															<on_val:OnitJsValidator id="txtMesi" runat="server" CssClass="textbox_numerico_disabilitato w100p" ReadOnly="True"
																actionCorrect="False" actionDelete="False" actionFocus="True" actionMessage="True" actionSelect="False"
																actionUndo="True" autoFormat="True" validationType="Validate_custom" CustomValFunction-Length="2" CustomValFunction="validaNumero"
																SetOnChange="True" MaxLength="2"></on_val:OnitJsValidator>
                                                        </td>
														<td></td>
														<td class="label_right">
															<on_val:OnitJsValidator id="txtAnni" runat="server" CssClass="textbox_numerico_disabilitato w100p" ReadOnly="True"
																actionCorrect="False" actionDelete="False" actionFocus="True" actionMessage="True" actionSelect="False"
																actionUndo="True" autoFormat="True" validationType="Validate_custom" CustomValFunction-Length="4" CustomValFunction="validaNumero"
																SetOnChange="True" MaxLength="4"></on_val:OnitJsValidator>
                                                        </td>
														<td></td>
													</tr>
													<tr>
														<td></td>
														<td class="sezione" colspan="4">Motivi Esclusione</td>
														<td></td>
														<td class="sezione" colspan="5">Stati Anagrafici</td>
														<td></td>
													</tr>
													<tr>
														<td></td>
														<TD colspan="4">
															<table border="0" cellspacing="0" cellpadding="0" width="100%">
																<tr>
																	<td width="90%">
																		<asp:TextBox id="txtMotEsc" runat="server" CssClass="textbox_stringa_disabilitato w100p" ReadOnly="True"></asp:TextBox></td>
																	<td width="10%" align="right">
																		<asp:Button id="btnMotEsc" runat="server" Width="100%" Text="..." Enabled="false"></asp:Button></td>
																</tr>
															</table>
														</td>
														<td></td>
														<td colspan="5">
															<uc2:StatiAnagrafici id="ucStatiAnagrafici" runat="server" ShowLabel="false"></uc2:StatiAnagrafici></td>
														<td></td>
													</tr>
												</table>
											</fieldset>
										</td>
									</tr>
									<tr height="2">
										<td></td>
										<td></td>
									</tr>
								</table>
							</td>
							<td width="40%">
								<table id="tblext1" border="0" cellspacing="0" cellpadding="0" width="100%" height="100%">
									<tr height="2">
										<td></td>
										<td></td>
										<td></td>
									</tr>
									<tr>
										<td colspan="3">
											<fieldset class="fldroot">
							                    <legend class="label">Crea Convocazione</legend>
												<table style="MARGIN-TOP: 2px" id="tblcwr" class="label_left" border="0" cellspacing="0" cellpadding="2" width="100%">
													<colgroup>
														<col width="2%" />
														<col width="47%" />
														<col width="2%" />
														<col width="47%" />
														<col width="2%" />
													</colgroup>
													<tr>
														<td></td>
														<td class="sezione" colspan="3">Associazione da inserire nelle programmate</td>
														<td></td>
													</tr>
													<tr>
														<td></td>
														<td colspan="3">
															<on_ofm:OnitModalList id="omlAss" runat="server" Obbligatorio="True" SetUpperCase="True" UseCode="True"
																UseTableLayout="True" CodiceWidth="29%" LabelWidth="-1px" PosizionamentoFacile="False"
																Width="70%"	Enabled="False"></on_ofm:OnitModalList></td>
														<td></td>
													</tr>
													<tr>
														<td></td>
														<td class="sezione">Data CNV</td>
														<td></td>
														<td class="sezione">Ultima Data</td>
														<td></td>
													</tr>
													<tr>
														<td></td>
														<td>
															<on_val:onitdatepick id="odpDataCNV" runat="server" Height="20px" Width="120px" DateBox="True" CssClass="textbox_data_obbligatorio"
																Enabled="False"></on_val:onitdatepick></td>
														<td></td>
														<td>
															<asp:TextBox id="txtUltimaData" runat="server" Width="100%" CssClass="TextBox_Stringa_Disabilitato"
																ReadOnly="True" Enabled="False"></asp:TextBox></td>
														<td></td>
													</tr>
													<tr>
														<td></td>
														<TD class="sezione">Forza CNV
														</td>
														<td></td>
														<td class="sezione">Includi Ritardari
														</td>
														<td></td>
													</tr>
													<tr>
														<td></td>
														<td>
															<asp:CheckBox id="chkForzaCNV" Runat="server"></asp:CheckBox></td>
														<td></td>
														<td>
															<asp:CheckBox id="chkIncRit" Runat="server"></asp:CheckBox></td>
														<td></td>
													</tr>
													<tr>
														<td></td>
														<td class="sezione" colspan="3">Ciclo da associare al paziente</td>
														<td></td>
													</tr>
													<tr>
														<td></td>
														<td colspan="3">
															<on_ofm:OnitModalList id="omlCicloCNV" runat="server" Obbligatorio="False" SetUpperCase="True" UseCode="True"
																UseTableLayout="True" Label="Titolo" CodiceWidth="29%" LabelWidth="-1px" PosizionamentoFacile="False" Filtro=" CIC_OBSOLETO='N' ORDER BY CIC_CODICE"
																Tabella="T_ANA_CICLI" CampoCodice="CIC_CODICE Codice" CampoDescrizione="CIC_DESCRIZIONE Descrizione" Width="70%" Enabled="False"></on_ofm:OnitModalList></td>
														<td></td>
													</tr>
                                                    <tr>
                                                        <td></td>
                                                        <td class="sezione" colspan="3">Centro Vaccinale di Convocazione</td>
                                                        <td></td>
                                                    </tr>
                                                    <tr>
                                                        <td></td>
                                                        <td colspan="3">
                                                            <asp:TextBox ID="txtConsultorioPrenotazione" runat="server" Width="100%" CssClass="TextBox_Stringa_Disabilitato"
																ReadOnly="True"></asp:TextBox>
                                                            </td>
                                                        <td></td>
                                                    </tr>
												</table>
											</fieldset>
										</td>
									</tr>
									<tr height="2">
										<td></td>
										<td></td>
										<td></td>
									</tr>
								</table>
							</td>
						</tr>
					</table> 
                </div>

				<div id="Div1" class="sezione" runat="server">&nbsp;
					<asp:Label id="lblPazientiTrovati" runat="server">Pazienti trovati</asp:Label>
                    <asp:DropDownList style="font-weight: normal" id="ddlPager" runat="server" Width="100%" Visible="False" cssclass="sezione"></asp:DropDownList>
                </div>
                
                <dyp:DynamicPanel ID="dypScroll1" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
					<on_dgr:OnitGrid id="ogPazienti" runat="server" Width="100%" CssClass="DataGrid" Visible="False" Height="100%"
						SortedColumns="Matrice IGridColumn[]" AllowPaging="True" SelectionOption="none" PagingMode="Auto"
						PageSize="100" GridLines="Horizontal" AutoGenerateColumns="False" PagerVoicesBefore="10" PagerVoicesAfter="10"
						CascadeStyles="True" PagerDropDownList="ddlPager" PagerGoToOption="False">
						<AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
						<ItemStyle CssClass="Item"></ItemStyle>                            
						<HeaderStyle CssClass="Header"></HeaderStyle>
						<SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
						<PagerStyle Visible="False"></PagerStyle>
						<Columns>
							<on_dgr:OnitBoundColumn Visible="False" DataField="PAZ_CODICE" key="PAZ_CODICE" SortDirection="NoSort"></on_dgr:OnitBoundColumn>
							<on_dgr:OnitTemplateColumn SortDirection="NoSort" HeaderText="&lt;input type='checkbox' id='chkSelTutti' onclick='selezionaPazienti(this.checked);'/&gt;">
								<ItemTemplate>
									<asp:CheckBox id="chkSel" runat="server" Checked='<%# (DataBinder.Eval(Container, "DataItem")("SEL")) = "S" %>'>
									</asp:CheckBox>
								</ItemTemplate>
							</on_dgr:OnitTemplateColumn>
							<on_dgr:OnitBoundColumn DataField="PAZ_COGNOME" HeaderText="Cognome" key="PAZ_COGNOME" SortDirection="NoSort"></on_dgr:OnitBoundColumn>
							<on_dgr:OnitBoundColumn DataField="PAZ_NOME" HeaderText="Nome" key="PAZ_NOME" SortDirection="NoSort"></on_dgr:OnitBoundColumn>
							<on_dgr:OnitBoundColumn DataField="PAZ_DATA_NASCITA" HeaderText="Data Nascita" key="PAZ_DATA_NASCITA" DataFormatString="{0:dd/MM/yyyy}"
								SortDirection="NoSort"></on_dgr:OnitBoundColumn>
							<on_dgr:OnitBoundColumn DataField="COM_DESCRIZIONE_RESIDENZA" HeaderText="Comune Residenza" key="COM_DESCRIZIONE_RESIDENZA"
								SortDirection="NoSort"></on_dgr:OnitBoundColumn>
							<on_dgr:OnitBoundColumn DataField="PAZ_INDIRIZZO_RESIDENZA" HeaderText="Indirizzo Residenza" key="PAZ_INDIRIZZO_RESIDENZA"
								SortDirection="NoSort"></on_dgr:OnitBoundColumn>
							<on_dgr:OnitBoundColumn DataField="COM_DESCRIZIONE_DOMICILIO" HeaderText="Comune Domicilio" key="COM_DESCRIZIONE_DOMICILIO"
								SortDirection="NoSort"></on_dgr:OnitBoundColumn>
							<on_dgr:OnitBoundColumn DataField="PAZ_INDIRIZZO_DOMICILIO" HeaderText="Indirizzo Domicilio" key="PAZ_INDIRIZZO_DOMICILIO"
								SortDirection="NoSort"></on_dgr:OnitBoundColumn>
							<on_dgr:OnitBoundColumn DataField="PAZ_CNS_CODICE" HeaderText="Centro Vaccinale" key="PAZ_CNS_CODICE"
								SortDirection="NoSort"></on_dgr:OnitBoundColumn>
						</Columns>
					</on_dgr:OnitGrid>
                </dyp:DynamicPanel>
                
			</on_lay3:onitlayout3>
            
			<onitcontrols:OnitFinestraModale id="ofmMotivoEsc" title="Inserisci Esclusione" runat="server" BackColor="LightGray" NoRenderX="True">
				<uc1:MotivoEsc id="ucMotivoEsc" runat="server"></uc1:MotivoEsc>
			</onitcontrols:OnitFinestraModale>

        </form>
	</body>
</html>
