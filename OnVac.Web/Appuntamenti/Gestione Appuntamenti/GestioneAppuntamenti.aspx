<%@ Page Language="vb" Debug="true" AutoEventWireup="false" CodeBehind="GestioneAppuntamenti.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.GestioneAppuntamenti" %>

<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel"  %>

<%@ Register TagPrefix="uc1" TagName="UscFiltroPrenotazioneSelezioneMultipla" Src="UscFiltroPrenotazioneSelezioneMultipla.ascx" %>
<%@ Register TagPrefix="uc2" TagName="AjaxList" Src="../../Common/Controls/AjaxList.ascx" %>
<%@ Register TagPrefix="uc2" TagName="StatiAnagrafici" Src="../../Common/Controls/StatiAnagrafici.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SelezioneConsultori" Src="../../Common/Controls/SelezioneConsultori.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ConsensoUtente" Src="../../Common/Controls/ConsensoTrattamentoDatiUtente.ascx" %>

<%@ Import Namespace="Onit.OnAssistnet.OnVac.OnVacUtility" %>
<%@ Import Namespace="Onit.OnAssistnet.OnVac" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>Gestione Appuntamenti</title>
    <style type="text/css">
        .Trovato {
            font-style: italic;
        }

        .Sort {
            font-family: arial;
            font-size: 11px;
            cursor: pointer;
        }

            .Sort:hover {
                font-size: 11px;
            }

        .Festa {
            background-color: gainsboro;
        }

        .Indisponibile {
            background-color: #eeeaea;
        }

        .Prenotato {
            color: red;
            font-weight: bold;
        }

        .w70 {
            width: 70px;
        }

        .NumeroPagina {
            color: white;
            font-size: 11px;
            cursor: pointer;
            text-decoration: none;
        }

            .NumeroPagina .NumeroPaginaSel {
                color: red;
            }

        .Btn_Filtro {
            border: 1px outset;
            background-color: beige;
            cursor: pointer;
        }

        .btn_stampa_sel {
            border: 0;
            margin: 3px;
            width: 150px;
            font-family: Arial;
            height: 24px;
            color: #3333cc;
            font-size: 10px;
            cursor: pointer;
            padding-top: 2px;
        }

        .btn_pulisci_filtri {
            font-family: Arial;
            color: #3333cc;
            font-size: 10px;
            cursor: pointer;
            padding-top: 2px;
        }

        .divRiepilogoOverflow {
            text-align: center;
            overflow: auto;
            vertical-align: top;
        }

        .divRiepilogoBorder {
            border: 1px solid navy;
        }

        fieldset {
            padding: 0;
        }
        
    </style>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript"> window.imagesPath = "<%= ResolveUrl("~/Images/") %>"; </script>
    <script type="text/javascript" src="ScriptGestApp.js"></script>
    <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
    <script type="text/javascript">
        
        function toggleCheck(idChk) {
            
            var chk = document.getElementById(idChk);
            if (chk == null) return;

            chk.checked = !chk.checked;

            return;
        }

        $(document).ready(function () {
            
            // Con il multiview vengono inseriti nella pagina solo i controlli della ActiveView
            if (mltViewActiveViewID === viewGestioneAppuntamenti) {
            
                document.getElementById("btnAvviaRicerca_SoloRicerca").onclick = function () { clickAvviaRicerca('Ricerca convocazioni in corso...'); };
                document.getElementById("btnAvviaRicerca_OperazioniERicerca").onclick = function () { clickAvviaRicerca('Esecuzione operazioni e ricerca convocazioni in corso...'); };

                // Imposta l'altezza uguale nelle table nei due fieldset di ricerca
                try {
                    var max = 0;
                    $('[id=Tablecicliext],[id=TableFPr]').each(function () {
                        max = Math.max($(this).height(), max);
                    }).height(max);
                } catch (e) {
                    // 
                }
            }
        });

        var state0 = <%=CStr(State(0)).ToLower()%>;
        var state1 = <%=CStr(State(1)).ToLower()%>;
        var txtFinoADataClientId = '<%= DirectCast(Tab1.FindControl("txtFinoAData"), OnitDatePick).ClientID%>';
        var txtDaDataNascitaClientId = '<%= DirectCast(Tab1.FindControl("txtDaDataNascita"), OnitDatePick).ClientID%>';
        var txtADataNascitaClientId = '<%= DirectCast(Tab1.FindControl("txtADataNascita"), OnitDatePick).ClientID%>';
        var chkDataSingolaClientId = '<%= DirectCast(Tab1.FindControl("chkDataSingola"), System.Web.UI.WebControls.CheckBox).ClientID%>';
        var btnOrariPersClientId = '<%= directcast(tab1.FindControl("btnOrariPers"),Button).ClientId %>';
        var IsPostBack = <%= IsPostBack().ToString().ToLower() %>;
        var modSalvataggio = <%=CStr(modSalvataggio).ToLower() %>;
        var hid_txt_num_orari_persClientId = '<%= Tab1.FindControl("hid_txt_num_orari_pers").ClientId %>';
        var chkOrariPersClientId = '<%= Tab1.FindControl("chkOrariPers").ClientId %>';
        var btnPulisciFiltri = '<%= btnPulisciFiltri.ClientId %>';
        var mltViewActiveViewID = '<%= mltView.GetActiveView().ID %>';
        var viewGestioneAppuntamenti = '<%= viewGestioneAppuntamenti.ID %>';

    </script>
</head>
<body onload="onLoad();" style="margin:0; padding:0; border:0;">
    <form id="Form1" method="post" runat="server" style="margin:0; padding:0; border:0;">
    
    <asp:MultiView ID="mltView" runat="server" ActiveViewIndex="0" >
    
        <asp:View ID="viewGestioneAppuntamenti" runat="server">

            <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Titolo="Gestione Appuntamenti" TitleCssClass="Title3" Height="100%" Width="100%">

                <div id="LayoutTitolo" class="Title" runat="server">Gestione appuntamenti</div>
                
                <div>
                    <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" CssClass="infratoolbar" ItemWidthDefault="80px" >
					    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                        <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
					    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
					    <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
					    <Items>
						    <igtbar:TBarButton Key="Salva" Text="Salva" DisabledImage="~/Images/salva_dis.gif" Image="~/Images/salva.gif">
							    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
						    </igtbar:TBarButton>
						    <igtbar:TBarButton Key="AnnullaCambiamenti" Text="Annulla" DisabledImage="~/Images/annulla_dis.gif" Image="~/Images/annulla.gif">
							    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
						    </igtbar:TBarButton>
						    <igtbar:TBSeparator></igtbar:TBSeparator>
						    <igtbar:TBarButton Key="Cerca" Text="Cerca" DisabledImage="~/Images/cerca_dis.gif" Image="~/Images/cerca.gif">
							    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
						    </igtbar:TBarButton>
						    <igtbar:TBSeparator></igtbar:TBSeparator>
						    <igtbar:TBarButton Key="PrenotaAutomatico" Text="Prenota" DisabledImage="~/Images/avanti.gif" Image="~/Images/avanti.gif">
							    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
						    </igtbar:TBarButton>
						    <igtbar:TBarButton Key="SpostaPrenotazioni" Text="Sposta Pren." DisabledImage="../../images/AppSposta_dis.gif" Image="../../images/AppSposta.gif">
							    <DefaultStyle CssClass="infratoolbar_button_default" Width="100px"></DefaultStyle>
						    </igtbar:TBarButton>
						    <igtbar:TBarButton Key="Sprenota" Text="Elimina Pren." DisabledImage="~/Images/indietro.gif" Image="~/Images/indietro.gif"
                                ToolTip="I pazienti a cui sta per essere eliminato l'appuntamento non verranno visualizzati nell'elenco dei convocati fino a che non sar&#224; rieseguito il 'Cerca'!">
							    <DefaultStyle CssClass="infratoolbar_button_default" Width="100px"></DefaultStyle>
						    </igtbar:TBarButton>
						    <igtbar:TBSeparator></igtbar:TBSeparator>
						    <igtbar:TBarButton Key="StampaElenco" Text="Stampa" DisabledImage="~/Images/stampa_dis.gif" Image="~/Images/stampa.gif" 
							    ToolTip="Stampa l'elenco dei convocati">
							    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
						    </igtbar:TBarButton>
						    <igtbar:TBarButton Key="VisualizzaLog" Text="Log" DisabledImage="~/Images/anteprima_dis.gif" Image="~/Images/anteprima.gif">
							    <DefaultStyle CssClass="infratoolbar_button_default" Width="60px"></DefaultStyle>
						    </igtbar:TBarButton>
						    <igtbar:TBSeparator></igtbar:TBSeparator>
						    <igtbar:TBarButton Key="Pazienti" Text="Pazienti" DisabledImage="../../images/gruppi.gif" Image="../../images/gruppi.GIF"
							    ToolTip="Apre il dettaglio del primo paziente selezionato nella ricerca convocati oppure nel calendario appuntamenti del giorno">
							    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
						    </igtbar:TBarButton>
					    </Items>
				    </igtbar:UltraWebToolbar>
                </div>
                
                <div style="width: 100%" id="PanelRicerca" class="sezione" runat="server">
					<asp:Label id="lblTitoloRicerca" runat="server"></asp:Label>
                </div>
               
                <dyp:DynamicPanel ID="dypFiltri" runat="server" Width="100%" Height="280px">
                    <table id="TabellaFiltri" width="100%">
						<tr>
							<td>
                                <div id="divPin" style="position: absolute; right: 0; width: 80px" onclick="pulisciFiltri();">
                                    <table class="btn_pulisci_filtri" onmouseover="btn_stampa_sel_mouse(this,'btnPulisciFiltri','over')"
                                        title="Pulisci filtri" onmouseout="btn_stampa_sel_mouse(this,'btnPulisciFiltri','out')"
                                        onclick="btn_stampa_sel_mouse(this,'btnPulisciFiltri','click')" border="0" cellspacing="0"
                                        cellpadding="0" width="100%" id="table1" runat="Server">
										<tr>
                                            <td valign="middle">
                                                <asp:ImageButton id="btnPulisciFiltri" runat="server" title="Pulisci i filtri" style="cursor: pointer;" ImageUrl="../../images/pulisci.gif" />
                                            </td>
											<td valign="middle">Pulisci filtri</td>
                                        </tr>
                                    </table>
                                </div>
								<igtab:UltraWebTab BrowserTarget="UpLevel" id="Tab1" runat="server" BorderStyle="Solid" Width="100%" ThreeDEffect="False" BorderColor="#949878" BorderWidth="1px">
									<DefaultTabStyle Height="22px" Font-Size="8pt" Font-Names="Microsoft Sans Serif" ForeColor="Black" BackColor="#FEFCFD">
										<Padding Top="2px"></Padding>
									</DefaultTabStyle>
									<RoundedImage LeftSideWidth="7" RightSideWidth="6" ShiftOfImages="2" SelectedImage="ig_tab_winXP1.gif"
										NormalImage="ig_tab_winXP3.gif" HoverImage="ig_tab_winXP2.gif" FillStyle="LeftMergedWithCenter"></RoundedImage>
									<SelectedTabStyle>
										<Padding Bottom="2px"></Padding>
									</SelectedTabStyle>
									<Tabs>
										<igtab:Tab Text="Appuntamenti">
											<ContentTemplate>
												<table id="TableExt" cellspacing="0" cellpadding="0" width="100%" border="0">
													<tr vAlign="top">
														<td width="70%">
															<fieldset id="fldCerca" title="Cerca" class="fldroot" >
															    <legend class="label">Ricerca convocati</legend>
                                                                <table id="Tablecicliext" style="table-layout: fixed" cellspacing="0" cellpadding="0" width="100%" border="0">
																	<tr>
																		<td>
																			<table id="Tablecicli" style="table-layout: fixed" cellspacing="0" cellpadding="0" width="100%" border="0">
																				<colgroup>
																					<col width="1%">
																					<col width="18%">
																					<col width="35%">
                                                                                    <col width="10%">
                                                                                    <col width="35%">
																					<col width="1%">
																				</colgroup>
																				<tr>
																					<td>&nbsp;</td>
																					<td>
																						<asp:Label id="lblFiltroCicSed" runat="server" CssClass="textbox_stringa">Cicli-Sedute:</asp:Label>
                                                                                    </td>
                                                                                    <td colspan="3">
                                                                                        <table width="100%" border="0" style="table-layout: fixed;">
	                                                                                        <tr>
		                                                                                        <td width="26px" align="right">
                                                                                                    <asp:ImageButton id="btnImgCicliSedute" runat="server" onmouseover="mouse(this,'over');" title="Impostazione filtro cicli-sedute" style="cursor: pointer;" onmouseout="mouse(this,'out');" ImageUrl="../../images/filtro_cicli.gif" />
                                                                                                </td>
		                                                                                        <td style="height: 22px; border: navy 1px solid; padding-left: 5px; background: gainsboro" >
                                                                                                    <asp:Label id="lblCicliSedute" style="height: 18px; font-size: 10px; text-transform: uppercase; font-style: italic; font-family: verdana" runat="server" Text=""></asp:Label>
                                                                                                </td>
	                                                                                        </tr>
                                                                                        </table>
                                                                                    </td>
                                                                                    <td>&nbsp;</td>
																				</tr>
																				<tr>
																					<td>&nbsp;</td>
																					<td class="label_left">Associazioni-Dosi:</td>
                                                                                    <td colspan="3">
                                                                                        <table width="100%" border="0" style="table-layout: fixed;">
	                                                                                        <tr>
		                                                                                        <td width="26px" align="right">
                                                                                                    <asp:ImageButton id="btnImgAssociazioniDosi" runat="server" onmouseover="mouse(this,'over');" title="Impostazione filtro associazioni-dosi" style="cursor: pointer" onmouseout="mouse(this,'out');" ImageUrl="../../images/filtro_associazioni.gif" />
                                                                                                </td>
		                                                                                        <td style="height: 22px; border: navy 1px solid; padding-left: 5px; background: gainsboro" >
                                                                                                    <asp:Label id="lblAssociazioniDosi" style="height: 18px; font-size: 10px; text-transform: uppercase; font-style: italic; font-family: verdana" runat="server" Text=""></asp:Label>
                                                                                                </td>
	                                                                                        </tr>
                                                                                        </table>
                                                                                    </td>
                                                                                    <td>&nbsp;</td>
																				</tr>
																				<tr>
																					<td>&nbsp;</td>
																					<td class="label_left">Comune:</td>
                                                                                    <td>
																						<on_ofm:OnitModalList ID="fmComune" runat="server" CampoCodice="COM_CODICE" CampoDescrizione="COM_DESCRIZIONE" CodiceWidth="28%" Label="Comune" LabelWidth="-8px" PosizionamentoFacile="False" RaiseChangeEvent="True" SetUpperCase="True" Tabella="T_ANA_COMUNI" UseCode="True" Width="70%"></on_ofm:OnitModalList>
																					</td>
                                                                                    <td class="label">Centro:&nbsp;</td>
																					<td class="Label">
                                                                                        <uc1:SelezioneConsultori id="ucSelezioneConsultori" runat="server" Width="100%"  MaxCnsInListaSelezionati="3"></uc1:SelezioneConsultori>
                                                                                    </td>
                                                                                    <td>&nbsp;</td>
																				</tr>
																				<tr>
																					<td>&nbsp;</td>
																					<td class="label_left">Medico:</td>
                                                                                    <td>
																						<on_ofm:OnitModalList id="txtMedico" runat="server" Width="98%" SetUpperCase="True" CodiceWidth="0px" CampoCodice="MED_CODICE Codice" CampoDescrizione="MED_DESCRIZIONE Nome" Tabella="T_ANA_MEDICI" LabelWidth="-8px" PosizionamentoFacile="False" Label="Titolo" Filtro="1=1 ORDER BY Nome" IsDistinct="True"></on_ofm:OnitModalList>
                                                                                    </td>
																					<td class="label">Sesso:&nbsp;</td>
                                                                                    <td class="label_left" >
                                                                                        <table style="table-layout: fixed" cellspacing="0" cellpadding="0" width="100%" border="0">
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:DropDownList Runat="server" ID="ddlSesso">
																						                <asp:ListItem Selected="True" Value=""></asp:ListItem>
                                                                                                        <asp:ListItem Value="M">Maschio</asp:ListItem>
                                                                                                        <asp:ListItem Value="F">Femmina</asp:ListItem>
                                                                                                    </asp:DropDownList>        
                                                                                                </td>
                                                                                                <td align="right">
                                                                                                    <asp:CheckBox ID="chkCronici" runat="server" CssClass="TextBox_Stringa" Text="Cronici" TextAlign="Right"></asp:CheckBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>
                                                                                    <td>&nbsp;</td>
																				</tr>
																				<tr>
																					<td>&nbsp;</td>
																					<td class="label_left">Categoria a rischio:</td>
                                                                                    <td>
                                                                                        <on_ofm:OnitModalList ID="omlCategorieRischio" runat="server" CampoCodice="RSC_CODICE" CampoDescrizione="RSC_DESCRIZIONE" CodiceWidth="28%" Label="Titolo" LabelWidth="-8px" PosizionamentoFacile="False" SetUpperCase="True"  Tabella="T_ANA_RISCHIO" Width="70%"></on_ofm:OnitModalList>
                                                                                    </td>
                                                                                    <td class="label">Malattia:&#160;</td>
                                                                                    <td>
                                                                                        <on_ofm:OnitModalList ID="omlMalattia" runat="server" CampoCodice="MAL_CODICE Codice" CampoDescrizione="MAL_DESCRIZIONE Descrizione" CodiceWidth="19%" Label="Titolo" LabelWidth="-1px" PosizionamentoFacile="False" SetUpperCase="True" Tabella="T_ANA_MALATTIE" Width="80%" Filtro=" MAL_OBSOLETO='N' ORDER BY MAL_CODICE"></on_ofm:OnitModalList>
																					</td>
																					<td>&nbsp;</td>
																				</tr>
																				<tr>
																					<td>&nbsp;</td>
																					<td class="label_left">Cittadinanza:</td>
                                                                                    <td colspan="3">
																						<table cellSpacing="0" cellPadding="0" width="100%" border="0">
																							<tr>
																								<td width="60%">
																									<asp:CheckBox id="chkImmigratiNonExtracomPrimaVolta" runat="server" CssClass="TextBox_Stringa"
																										Text="Immigrati non extracomunitari prima volta" TextAlign="Right"></asp:CheckBox></td>
																								<td width="40%">
																									<asp:CheckBox id="chkImmigratiExtracom" runat="server" CssClass="TextBox_Stringa" Text="Extracomunitari"
																										TextAlign="Right"></asp:CheckBox></td>
																							</tr>
																						</table>
																					</td>
																					<td>&nbsp;</td>
																				</tr>
																			</table>
																			<table id="TableFiltri" cellspacing="0" cellpadding="0" width="100%" border="0">
                                                                                <tr height="2">
																					<td colspan="7"></td>
																				</tr>
																				<tr>
                                                                                    <td width="1%"></td>
																					<td width="32%">
																						<fieldset id="fldNascita" title="Data nascita" class="fldnode" >
																						    <legend class="label">Data nascita</legend><table id="Table_nascita" height="48" cellspacing="0" cellpadding="0" width="100%" border="0">
																								<tr>
																									<td class="label" width="20%">Da :</td><td align="center">
																										<on_val:onitdatepick id="txtDaDataNascita" runat="server" Height="20px" Width="120px" CssClass="textbox_data" DateBox="True"></on_val:onitdatepick>
                                                                                                    </td>
																								</tr>
																								<tr>
																									<td class="label">A :</td><td align="center">
																										<on_val:onitdatepick id="txtADataNascita" runat="server" Height="20px" Width="120px" CssClass="textbox_data" DateBox="True"></on_val:onitdatepick>
                                                                                                    </td>
																								</tr>
																							</table>
																						</fieldset>
																					    </td>
                                                                                        <td width="1%"></td>
                                                                                        <td width="32%">
																						<fieldset id="fldOpzioni" title="Opzioni Ricerca" class="fldnode"   >
																						<legend class="label">Opzioni</legend><div style="height:44;overflow:auto; margin:2px">
																						        <table id="Table_opzioni" cellspacing="0" cellpadding="0" width="100%" border="0">
																							        <tr>
																								        <td align="left">
																									        <asp:CheckBox id="chkSoloRitardatari" runat="server" CssClass="TextBox_Stringa" Text="Cerca solo ritardatari"
																										        TextAlign="Right"></asp:CheckBox></td>
																							        </tr>
																							        <tr>
																								        <td align="left">
																									        <asp:CheckBox id="chkDataSingola" runat="server" Height="23px" CssClass="TextBox_Stringa" Text="CNV in Data Singola"
																										        TextAlign="Right"></asp:CheckBox></td>
																							        </tr>
																							        <tr>
																								        <td align="left">
																								            <asp:CheckBox ID="chkEscluse" TextAlign="Right" Text="Mostra se solo ESCLUSE" 
																								                ToolTip="Includi anche programmazioni vaccinali con solo vaccinazioni escluse"
																								                    CssClass="TextBox_Stringa"  runat="server" />
																								        </td>
																							        </tr>
																						        </table>
																							</div>
																						</fieldset>
																					    </td><td width="1%"></td>
																					<td width="32%">
																						<fieldset id="fldp" title="Data Convocazione" class="fldnode" >
																						    <legend class="label">Data CNV</legend><table id="Table_ppzioni" height="46" cellspacing="0" cellpadding="0" width="100%" border="0">
																								<tr>
																									<td align="center" height="45%">
																										<asp:Label id="lblUltimaData" runat="server" Font-Size="9" CssClass="TextBox_Stringa" Text='<%# "Ultima data: " &amp; UltimaDataConsultorio()  %>'>
																										</asp:Label></td></tr><tr>
																									<td align="center" height="55%">
																										<on_val:onitdatepick id="txtFinoAData" runat="server" Height="20px" Width="120px" CssClass="textbox_data_obbligatorio" DateBox="True"></on_val:onitdatepick>
                                                                                                    </td>
																								</tr>
																							</table>
																						</fieldset>
																					    </td>
                                                                                        <td width="1%"></td>
																				</tr>
																				<tr height="2">
																					<td colSpan="7"></td>
																				</tr>
																			</table>
																		</td>
																	</tr>
																</table>
															</fieldset>
                                                        </td>
                                                        <td width="1px"></td>
														<td width="30%">
															<fieldset id="fldPrenota" title="Prenota" class="fldroot">
															    <legend class="label">Prenota convocati</legend>
                                                                <table id="TableFPr" style="table-layout: fixed" cellspacing="0" cellpadding="0" width="100%" border="0">
																	<tr>
																		<td width="3%"></td>
																		<td></td>
																		<td width="3%"></td>
                                                                    </tr>
																	<tr>
																		<td>&nbsp;</td>
																		<td>
																			<table id="Table_mx" cellspacing="0" cellpadding="0" width="100%" border="0">
																				<tr>
																					<td>
																						<fieldset id="fldPeriodo" title="Periodo appuntamento" class="fldnode">
																						    <legend class="label">Periodo appuntamento</legend><table id="Table_m" height="48" cellspacing="0" cellpadding="0" width="100%" border="0">
																								<tr>
																									<td class="label_right" style="WIDTH: 25%; PADDING-TOP: 5px" align="right">
																										<asp:Label id="lblDataIniz" runat="server" >Da :</asp:Label>
                                                                                                    </td>
                                                                                                    <td>
																										<on_val:onitdatepick id="odpDataInizPrenotazioni" runat="server" Height="20px" Width="120px" CssClass="TextBox_Data"
																											DateBox="True"></on_val:onitdatepick>
                                                                                                    </td>
																									<td width="10%"></td>
																								</tr>
																								<tr>
																									<td class="label_right" style="WIDTH: 25%" align="right">
																										<asp:Label id="lblDataFin" runat="server" >A :</asp:Label>
                                                                                                    </td>
                                                                                                    <td>
																										<on_val:onitdatepick id="odpDataFinePrenotazioni" runat="server" Height="20px" Width="120px" CssClass="TextBox_Data"
																											DateBox="True"></on_val:onitdatepick>
                                                                                                    </td>
																									<td width="10%"></td>
																								</tr>
																							</table>
																						</fieldset>
																					    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </td>
                                                                        <td>&nbsp;</td>
																	</tr>
																	<tr>
																		<td colspan="3" height="10"></td>
																	</tr>
																	<tr>
																		<td>&nbsp;</td>
																		<td>
																			<fieldset id="fldOrariPers" title="Orari Personalizzati" class="fldnode">
																				<legend class="label">Orari Personalizzati</legend><table id="Table_orari_pres" cellSpacing="0" cellPadding="4" width="100%" border="0">
																					<tr>
																						<td align="center">
																							<div class="textbox_data" onmouseover="mouseOrariPers(this,'over')" title="Imposta orari personalizzati"
																								style="padding-left: 10px; background-image: url(../../images/bg_oraripers.gif); vertical-align: middle; width: 150px; cursor: pointer; padding-top: 2px; background-repeat: no-repeat; height: 20px"
																								onclick="clickOrariPers()" onmouseout="mouseOrariPers(this,'out')" align="center">Orari personalizzati</div>
                                                                                            <asp:Button id="btnOrariPers" style="display: none" runat="server" Height="20px" Width="90%"
                                                                                                CssClass="Btn_Filtro" Text="Imposta orari personalizzati" ToolTip="Imposta gli orari di prenotazione dell'ambulatorio"></asp:Button>
                                                                                        </td>
																					</tr>
																					<tr>
																						<td align="center">
																							<asp:CheckBox id="chkOrariPers" onclick="checkNumOrariPers()" runat="server" Height="23px" CssClass="TextBox_Stringa"
																								Text="Utilizza orari personalizzati" TextAlign="Right"></asp:CheckBox>
																							<asp:TextBox id="hid_txt_num_orari_pers" style="display: none" runat="server"></asp:TextBox>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </fieldset> 
                                                                        </td>
                                                                        <td>&nbsp;</td>
																	</tr>
																	<tr>
																		<td colspan="3" height="10"></td>
																	</tr>
																	<tr>
																		<td>&nbsp;</td>
																		<td>
																			<fieldset id="fldOpz" title="Opzioni Prenota"  class="fldnode">
																				<legend class="label">Opzioni</legend>
                                                                                <table id="Table_opzioni" cellspacing="0" cellpadding="0" width="100%" border="0">
																					<tr>
																						<td width="10%"></td>
																						<td align="left">
																							<asp:CheckBox id="chkSovrapponiRit" runat="server" Height="23px" CssClass="TextBox_Stringa" Text="Overbooking Ritardatari"
																								TextAlign="Right"></asp:CheckBox>
                                                                                        </td>
																					</tr>
																					<tr>
																						<td width="10%"></td>
																						<td align="left">
																							<asp:CheckBox id="chkFiltroPomeriggioObbligatorio" runat="server" Height="23px" CssClass="TextBox_Stringa"
																								Text="Pomeriggio Obbligatorio" TextAlign="Right"></asp:CheckBox>
                                                                                        </td>
																					</tr>
																				</table>
																			</fieldset>
																		    </td>
                                                                            <td>&nbsp;</td>
																	</tr>
																	<tr>
																		<td colspan="3"></td>
																	</tr>
																</table>
															</fieldset>
														    </td>
                                                        </tr>
                                                </table>
                                            </ContentTemplate>
                                        </igtab:Tab>
                                        <igtab:Tab Text="Opzioni">
											<ContentTemplate>
												<table id="Table9" style="width:100%" cellspacing="0" cellpadding="0" border="0">
													<tr valign="top">
														<td width="70%">
															<fieldset title="Ricerca convocati" class="fldroot">
																<legend class="label">Ricerca convocati</legend>
                                                                <table id="tblOpzioniRicercaConvocati" style="table-layout: fixed; height: 130px; width: 100%" cellspacing="0" cellpadding="0" border="0">
																    <colgroup>
																		<col width="1%" />
																		<col width="22%" />
																		<col width="33%" />
                                                                        <col width="10%" />
                                                                        <col width="33%" />
																		<col width="1%" />
																	</colgroup>
                                                                    <tr>
																		<td>&nbsp;</td>
																		<td class="label_left">Stati anagrafici</td>
                                                                        <td colspan="3">
                                                                            <uc2:StatiAnagrafici id="ucStatiAnagrafici" runat="server" ShowLabel="false"></uc2:StatiAnagrafici>
                                                                        </td>
                                                                        <td>&nbsp;</td>
																	</tr>
                                                                    <tr>
																		<td>&nbsp;</td>
																		<td class="label_left">Tipo vaccinazione</td>
                                                                        <td colspan="3">
																			<table style="table-layout: fixed;" cellSpacing="0" cellPadding="0" width="100%" border="0">
																				<tr>
																					<td width="30%">
																						<asp:CheckBox id="chkTipoVaccObbligatoria" runat="server" CssClass="TextBox_Stringa" Text="Obbligatoria" TextAlign="Right"></asp:CheckBox></td>
																					<td width="30%">
																						<asp:CheckBox id="chkTipoVaccFacoltativa" runat="server" CssClass="TextBox_Stringa" Text="Facoltativa" TextAlign="Right"></asp:CheckBox></td>
																					<td width="30%">
																						<asp:CheckBox id="chkTipoVaccRaccomandata" runat="server" CssClass="TextBox_Stringa" Text="Raccomandata" TextAlign="Right"></asp:CheckBox></td>
                                                                                    <td>&nbsp;</td>
																				</tr>
																			</table>
																		</td>
																		<td>&nbsp;</td>
																	</tr>
                                                                    <tr title="Dati che verranno visualizzati">
																		<td>&nbsp;</td>
                                                                        <td class="label_left">Visualizzazione</td>
																		<td colspan="3">
                                                                            <table style="table-layout: fixed;" cellSpacing="0" cellPadding="0" width="100%" border="0">
                                                                                <tr>
                                                                                    <td width="30%">
                                                                                        <onit:CheckBox id="chkRicConvocazioni" runat="server" Height="16px" Width="110px" Text="Convocazione" CssClass="TextBox_Stringa" Checked="True" CommandName="OpzioniVisualizzazione" CommandArgument="C" onclick="btnRicApplica()" />
                                                                                    </td>
																		            <td width="30%">
																			            <onit:CheckBox id="chkRicCiclo" runat="server" Height="16px" Width="110px" Text="Vaccinazioni" CssClass="TextBox_Stringa" CommandName="OpzioniVisualizzazione" CommandArgument="V" onclick="btnRicApplica()" /></td>
																		            <td width="30%">
																			            <onit:CheckBox id="chkRicMedico" runat="server" Height="16px" Width="100px" Text="Medico" CssClass="TextBox_Stringa" CommandName="OpzioniVisualizzazione" CommandArgument="M" onclick="btnRicApplica()" /></td>
																		            <td>
																			            <onit:CheckBox id="chkRicBilancio" style="display: none; visibility: hidden" runat="server" Height="16px" Width="85px" Text="Bilancio" CssClass="TextBox_Stringa" CommandName="OpzioniVisualizzazione" CommandArgument="B" onclick="btnRicApplica()" />
                                                                           		        <asp:Button id="btnRicApplica" runat="server" Height="24px" Width="70px" Text="Applica" title="Applica le opzioni di visualizzazione selezionate" style="cursor:pointer" Visible="false"></asp:Button>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </td>
																		<td>&nbsp;</td>
																	</tr>
                                                                    <tr title="Se selezionato, i risultati della ricerca saranno ordinati per data convocazione, cognome, nome e data di nascita. Altrimenti, solo per data di convocazione e data di nascita">
                                                                        <td>&nbsp;</td>
                                                                        <td class="label_left" onclick="toggleCheck('<%= chkOrdineAlfabeticoRicerca.ClientId %>')">Ordina alfabeticamente</td>
                                                                        <td colspan="3">
                                                                            <asp:CheckBox ID="chkOrdineAlfabeticoRicerca" runat="server" Checked="true" />
                                                                        </td>
                                                                        <td>&nbsp;</td>
                                                                    </tr>

																</table>
															</fieldset>
														</td>
                                                        <td width="1px"></td>
														<td width="30%">
															<fieldset title="Opzioni di prenotazione" class="fldroot">
																<legend class="label">Prenota convocati</legend>
                                                                    <table id="tblOpzioniPrenotazione" style="table-layout: fixed; height: 130px; width: 100%" cellspacing="0" cellpadding="0" border="0">
                                                                    <colgroup>
                                                                        <col  width="59%" />
                                                                        <col  width="1%" />
                                                                        <col  width="40%" />
                                                                    </colgroup>
																	<tr>
																		<td class="label">N° max pazienti al giorno</td>
                                                                        <td>&nbsp;</td>
                                                                        <td>
                                                                            <asp:TextBox id="txtNumPazientiAlGiorno" runat="server" Width="30px" CssClass="TextBox_Stringa"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="label">Nuova durata sedute</td>
                                                                        <td>&nbsp;</td>
                                                                        <td>
                                                                            <asp:textbox id="txtDurata" runat="server" Width="30px" CssClass="TextBox_Stringa"></asp:textbox>
                                                                        </td>
																	</tr>
																	<tr>
																		<td class="label">N° nuovi pazienti al giorno</td>
                                                                        <td>&nbsp;</td>
                                                                        <td>
                                                                            <asp:textbox id="txtNumNuoviPazientiAlGiorno" runat="server" width="30px" cssClass="TextBox_Stringa"></asp:textbox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr title="Indica se, a parità di convocazione, prenotare gli assistiti seguendo l'ordine alfabetico">
                                                                        <td class="label" onclick="toggleCheck('<%= chkOrdineAlfabeticoPrenotazione.ClientId %>')">Ordina alfabeticamente</td>
                                                                        <td>&nbsp;</td>
                                                                        <td>
                                                                            <asp:CheckBox ID="chkOrdineAlfabeticoPrenotazione" runat="server" Checked="false" />
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </fieldset> 
                                                        </td>
													</tr>
												</table>
											</ContentTemplate>
										</igtab:Tab>
									</Tabs>
								</igtab:UltraWebTab>
                            </td>
						</tr>
					</table>
                </dyp:DynamicPanel>

                <div id="DivRis">
				    <table id="Table7sadf" class="sezione" cellspacing="0" cellpadding="0" width="100%">
					    <tr>
						    <td width="100%">
							    <asp:Label id="lblRis" runat="server" width="100%" CssClass="label_left" Font-Bold="True">RISULTATI RICERCA</asp:Label>
                            </td>
                            <td>&nbsp;</td>
						    <td>
                                <img style="cursor: pointer" id="imgFiltriRicerca" title="Chiude il riquadro dei filtri" 
                                    onclick="espandiFiltri(this);" alt="" src="../../images/chiudi_filtri.gif"/>
						    </td>
					    </tr>
				    </table>
                </div>

                <dyp:DynamicPanel ID="dypScroll1" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                    
				    <asp:Panel id="panelOrdinamenti" Runat="server">
					    <table border="0" cellspacing="0" cellpadding="0" width="100%">
						    <tr>
							    <td width="80%">
								    <div class="TextBox_Stringa">
                                        Ordina per: <a id="sortName" class="sort" onclick="__doPostBack('Sort','PAZ_COGNOME, PAZ_NOME')" href="#">Nome</a>
                                        &nbsp;- <a id="sortDataNascita" class="sort" onclick="__doPostBack('Sort','PAZ_DATA_NASCITA')" href="#">Data nascita</a>
                                        &nbsp;- <a id="sortDataConvocazione" class="sort" onclick="__doPostBack('Sort','CNV_DATA')" href="#">Data conv.</a>
                                        &nbsp;&nbsp;- <a id="sortMedico" class="sort" onclick="__doPostBack('Sort','MED_DESCRIZIONE')" href="#"> Medico</a>
                                        &nbsp;- <a id="sortVacc" class="sort" onclick="__doPostBack('Sort','VACCINAZIONI')" href="#"> Vaccinazione</a>
                                    </div>
                                </td>
                                <td>
								    <table class="btn_stampa_sel" onmouseover="btn_stampa_sel_mouse(this,'btnStampaSelezionati','over')"
									    title="Stampa solo i convocati selezionati" onmouseout="btn_stampa_sel_mouse(this,'btnStampaSelezionati','out')"
									    onclick="btn_stampa_sel_mouse(this,'btnStampaSelezionati','click')" border="0" cellspacing="0"
									    cellpadding="0" width="100%" id="tableStampaConvocati" runat="Server">
									    <tr>
										    <td valign="middle"><img alt="Stampa solo i convocati selezionati" runat="server" src="~/Images/stampa.gif" /></td>
										    <td valign="middle">Stampa Selezionati </td>
                                        </tr>
                                    </table>
                                    <asp:button style="display: none" id="btnStampaSelezionati" runat="server" value="Stampa selezionati"></asp:button>
                                </td>
						    </tr>
					    </table>
					    <div style="background-color: darkblue">
                            <asp:CheckBox  id="chkSelDeselDtaRicercaConvocati" onclick="chkSelDeselDtaRicercaConvocati_OnClick(this)" runat="server" /> 
                            <label style="color: white" id="labChkDeChk" class="label">Seleziona tutti</label>
                            <asp:Label id="lblNumeroPagine" runat="server" Width="85%" CssClass="label_right" Font-Bold="True" ForeColor="White" Text=""></asp:Label>
                        </div>
                    </asp:Panel>

                    <asp:DataList id="dlsRicercaConvocati" runat="server" Width="100%" CssClass="TextBox_Stringa" RepeatLayout="Flow">
						<ItemTemplate>
							<table class="TextBox_Stringa" id="Table6" cellspacing="0" cellpadding="0" width="100%">
								<tr>
									<td style="border-top: lightgrey 1px solid" valign="bottom" width="44%">
										<asp:CheckBox id="chkSelezione" runat="server" Height="0px" CssClass="label" align="absmiddle" Checked='<%# IIf(Not DataBinder.Eval(Container,"DataItem")("SEL") Is Nothing AndAlso DataBinder.Eval(Container,"DataItem")("SEL") = "S",True,False) %>'>
										</asp:CheckBox>
										<asp:Image id="Image1" title='<%# Container.DataItem("SOLLECITO") & "° Ritardo"%>' runat="server" ImageUrl="~/Images/avvertimento.gif" ImageAlign="AbsMiddle" Visible='<%# Container.DataItem("SOLLECITO") > 0 andalso not boolean.Parse(Container.DataItem("TP")) %>'>
										</asp:Image>
										<asp:Image id="Image2" title="Termine Perentorio" runat="server" ImageUrl="../../Images/TermPer.gif" ImageAlign="AbsMiddle" Visible='<%# boolean.Parse(Container.DataItem("TP")) %>'>
										</asp:Image>
										<asp:Label id="lblCognomeNome" runat="server" CssClass="label" Text='<%# databinder.eval(Container.dataitem,"PAZ_COGNOME") &amp; " " &amp; databinder.eval(Container.dataitem,"PAZ_NOME") %>' Font-Bold="True">
										</asp:Label>
                                        <asp:Label id=lblPazCodice style="display: none" runat="server" CssClass="label" Text='<%# databinder.eval(Container.dataitem,"PAZ_CODICE") %>'>
										</asp:Label>
                                    </td>
                                    <td  class="Label_left" style="border-top: lightgrey 1px solid" valign="bottom" align="left" width="25%">Data 
										di nascita: <asp:Label id="Label8" runat="server" Text='<%# cdate(databinder.eval(Container.dataitem,"PAZ_DATA_NASCITA")).ToString("dd/MM/yyyy") %>' Font-Bold="True">
										</asp:Label>
                                    </td>
                                    <td class="Label_left" style="border-top: lightgrey 1px solid" valign="bottom" align="left" width="25%">Conv: <asp:Label id="lblDataConvocazione" runat="server" Text='<%# cdate(databinder.eval(Container.dataitem,"CNV_DATA")).tostring("dd/MM/yyyy") %>' Font-Bold="True">
										</asp:Label>
                                    </td>
                                    <td  class="Label" style="border-top: lightgrey 1px solid" valign="bottom" align="left" width="2%">
										<asp:Label id="Label2" runat="server" Text='<%# IIF(databinder.eval(Container.dataitem,"tipo_extracomunitari").ToString()="0","&nbsp;","E") %>' Font-Bold="True">
										</asp:Label></td>
                                    <td  class="Label" style="border-top: lightgrey 1px solid" valign="bottom" align="left" width="2%">
										<asp:Label id="Label4" runat="server" Text='<%# IIF(databinder.eval(Container.dataitem,"tipo_immi_non_extra_prima").ToString()="0","&nbsp;","I") %>' Font-Bold="True">
										</asp:Label></td>
                                    <td class="Label" style="border-top: lightgrey 1px solid" valign="bottom" align="left" width="2%">
										<asp:Label id="Label5" runat="server" Text='<%# databinder.eval(Container.dataitem,"cronico").ToString() %>' Font-Bold="True">
										</asp:Label>
                                    </td>
                                </tr>
                                <tr>
									<td colspan="2"  class="Label_left">Vaccinazioni:&nbsp; <asp:Label id="Label16" runat="server"  Text='<%# IIf(databinder.eval(Container.dataitem,"VACCINAZIONI").toString()<>"",databinder.eval(Container.dataitem,"VACCINAZIONI"),"SOLO BILANCIO") %>' Font-Bold="True" /></td>
									<td colspan="2">Bilancio:&nbsp; </td>
                                    <td colspan="2"  class="Label_left">Medico:&#160; 
                                        <asp:Label  ID="Label23" runat="server"  Font-Bold="True" 
                                            Text='<%# databinder.eval(Container.dataitem,"MED_DESCRIZIONE") %>'>
										</asp:Label>
                                    </td>
								</tr>
							</table>
						</ItemTemplate>
					</asp:DataList>

				</dyp:DynamicPanel>

				<div id="DivCal" runat="server">
					<table id="Table6sadf" class="sezione" cellspacing="0" cellpadding="0">
						<tr>
							<td>
								<asp:Label id="LabelCal" runat="server">CALENDARIO</asp:Label>
                            </td>
                            <td width="100%"></td>
							<td>
                                <img style="cursor: pointer" id="imgEspandi" title="Chiude il riquadro dei risultati e visualizza i messaggi"
									onclick="espandiMessaggi(this);" alt="" src="../../images/chiudi_risultati.gif" />
							</td>
						</tr>
					</table>
				</div>
                
                <dyp:DynamicPanel ID="dypCalendario" runat="server" Width="100%" Height="222px" >

                    <table id="TabellaCalendario" border="0" cellspacing="0" cellpadding="0" width="100%">
                        <tr>
                            <td style="width: 226px; height: 1px" valign="top" width="226" align="left">
                                <table id="Table2" border="0" cellspacing="0" cellpadding="0" width="100%">
                                    <tr>
                                        <td style="width: 75px;" class="label_center">Vai a Data</td>
                                        <td style="width: 100px">
                                            <on_val:onitdatepick id="txtVaiAData" runat="server" width="100px" height="20px" cssclass="textbox_data" datebox="True" calendariopopup="True"
                                                nocalendario="True"></on_val:onitdatepick>
                                        </td>
                                        <td style="text-align:center">
                                            <asp:ImageButton ID="btnAssegna" runat="server" ImageUrl="~/Images/goto.gif" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:Calendar ID="Calendario" runat="server" Height="200px" UseAccessibleHeader="false" Width="200px" CssClass="DataGrid">
                                    <DayStyle CssClass="Alternating"></DayStyle>
                                    <NextPrevStyle ForeColor="White"></NextPrevStyle>
                                    <DayHeaderStyle CssClass="Header"></DayHeaderStyle>
                                    <SelectedDayStyle BorderWidth="1px" ForeColor="Black" BorderStyle="Solid" BorderColor="Blue" BackColor="#FFFFC0"></SelectedDayStyle>
                                    <TitleStyle ForeColor="White" CssClass="Header" BackColor="Navy"></TitleStyle>
                                    <WeekendDayStyle CssClass="Alternating" BackColor="Lavender"></WeekendDayStyle>
                                    <OtherMonthDayStyle ForeColor="Silver"></OtherMonthDayStyle>
                                </asp:Calendar>
                            </td>
                            <td valign="top" width="100%">
                                <div id="divTabellaCalendario" style="width: 100%; height: 220px; overflow: auto" >
                                    <igtab:UltraWebTab BrowserTarget="UpLevel" ID="Tab2" runat="server" Height="100%" Width="100%">
                                        <ClientSideEvents AfterSelectedTabChange="TabChange"></ClientSideEvents>
                                        <DefaultTabStyle Height="22px" Font-Size="8pt" Font-Names="Microsoft Sans Serif" ForeColor="Black" BackColor="#FEFCFD">
                                            <Padding Top="2px"></Padding>
                                        </DefaultTabStyle>
                                        <RoundedImage LeftSideWidth="7" RightSideWidth="6" ShiftOfImages="2" SelectedImage="ig_tab_winXP1.gif" NormalImage="ig_tab_winXP3.gif"
                                            HoverImage="ig_tab_winXP2.gif" FillStyle="LeftMergedWithCenter"></RoundedImage>
                                        <SelectedTabStyle>
                                            <Padding Bottom="2px"></Padding>
                                        </SelectedTabStyle>
                                        <Tabs>
                                            <igtab:TabSeparator Tag="preset">
                                                <Style Width="2px" />
                                            </igtab:TabSeparator>
                                            <igtab:Tab Key="Appuntamenti" Text="Appuntamenti">
                                                <ContentTemplate>
                                                    <table class="TextBox_Stringa" id="Table5" cellspacing="0" cellpadding="0" width="100%">
                                                        <tr>
                                                            <td style="background-color: darkblue">
                                                                <a href="javascript:__doPostBack('EspandilsMattino','true')">
                                                                    <img id="imgls_Mattino" style="cursor: pointer" runat="server" src="~/Images/meno.gif" align="middle" alt="" />
                                                                </a>&nbsp;
                                                                <span style="color: white"><%=oralmat%> </span>
                                                            </td>
                                                            <td style="background-color: darkblue" align="right">
                                                                <a href="javascript:__doPostBack('NuovoOrario','Mat')">
                                                                    <img style="cursor: pointer" alt="Conferma le modifiche apportate agli orari del mattino" runat="server" src="~/Images/conferma.gif">
                                                                </a>&nbsp; 
                                                                <input id="chkSelDeselMattina" type="checkbox">&nbsp;
                                                                <a href="javascript:PrenotaManuale('lsMattino')"> 
                                                                    <img style="cursor: pointer" alt="Prenota manualmente i pazienti selezionati" runat="server" src="~/Images/nuovo.gif" />
                                                                </a>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <asp:DataList ID="lsMattino" runat="server" CssClass="TextBox_Stringa" RepeatLayout="Flow" Style="overflow: auto">
                                                        <ItemTemplate>
                                                            <%#IIf(Not InRange(CDate(DataBinder.Eval(Container.DataItem, "CNV_DATA_APPUNTAMENTO")).TimeOfDay, oraMinAM.TimeOfDay, oraMaxAM.TimeOfDay, False), "<table  id=""Table15"" cellSpacing=""0""  cellPadding=""0"" width=""100%"" class=""textbox_stringa"" style=""BORDER-BOTTOM: navy 1px solid;background-color:lightyellow"">", "<table  id=""Table15"" cellSpacing=""0""  cellPadding=""0"" width=""100%"" class=""textbox_stringa"" style=""BORDER-BOTTOM: navy 1px solid;background-color:trasparent"">")%> <tr>
                                                                <td width="50px">
                                                                    <table width="100%">
                                                                        <tr>
                                                                            <td style="width:50%"><%#GetImageAvviso(DataBinder.Eval(Container.DataItem, "CNV_DATA_INVIO").ToString())%></td>
                                                                            <td style="width:50%"><%#GetImageAgenda(DataBinder.Eval(Container.DataItem, "SOLLECITO").ToString(), DataBinder.Eval(Container.DataItem, "TP").ToString(), DataBinder.Eval(Container.DataItem, "ESEGUITA").ToString(), DataBinder.Eval(Container.DataItem, "ESCLUSA").ToString())%></td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                                <td width="300px">
                                                                    <asp:CheckBox ID="chkSelezione" runat="server" 
                                                                        align="absmiddle" onclick="CheckCheckBox(event)" />
                                                                    <asp:Label ID="Label27" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"PAZ_COGNOME") &amp; " " &amp; DataBinder.Eval(Container.DataItem,"PAZ_NOME") %>'></asp:Label>
                                                                    <asp:Label ID="lblCodice" runat="server" CssClass="label" Style="display: none" Text='<%# DataBinder.Eval(Container.DataItem,"PAZ_CODICE") %>'></asp:Label>
                                                                </td>
                                                                <td width="100px">
                                                                    <asp:Label ID="Label29" runat="server" Text='<%# Cdate(DataBinder.Eval(Container.DataItem,"PAZ_DATA_NASCITA")).ToString("dd/MM/yyyy") %>'></asp:Label>
                                                                    <asp:Label ID="lblCnvData" runat="server" Style="display: none" Text='<%# Cdate(DataBinder.Eval(Container.DataItem,"CNV_DATA")).ToString("dd/MM/yyyy") %>'></asp:Label>
                                                                </td>
                                                                <td>
                                                                    Ore:&#160; <asp:TextBox ID="txtOrario" runat="server" CssClass="TextBox_Data" 
                                                                        onblur="if (!(orarioValido(this.parentNode.childNodes[1].value))) {alert('Orario non valido!'); this.focus();}" 
                                                                        Style="width: 50px" Text='<%# CDate(DataBinder.Eval(Container.DataItem,"CNV_DATA_APPUNTAMENTO")).ToString("HH\:mm")%>'>
                                                                    </asp:TextBox>
                                                                </td>
                                                                <td width="30px">
                                                                    <img ID="imgTipoApp" 
                                                                        alt='<%# IIf((DataBinder.Eval(Container.DataItem,"CNV_TIPO_APPUNTAMENTO")).ToString = "A", "Appuntamento assegnato automaticamente", "Appuntamento assegnato manualmente") %>' 
                                                                        src='<%# "../../Images/" & IIf((DataBinder.Eval(Container.DataItem,"CNV_TIPO_APPUNTAMENTO")).ToString = "A", "appuntamenti.gif", "annotazioni.gif") %>' />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td></td>
                                                                <td>
                                                                    Conv:&#160; <asp:Label ID="lblData" runat="server" Text='<%# CDate(DataBinder.Eval(Container.DataItem,"CNV_DATA")).Date.ToString("dd/MM/yyyy") %>'></asp:Label>
                                                                </td>
                                                                <td>
                                                                    Durata:&#160; <asp:Label ID="Label31" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"CNV_DURATA_APPUNTAMENTO") %>'></asp:Label>
                                                                </td>
                                                                <td>
                                                                    Medico:&#160; <asp:Label ID="Label32" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"MED_DESCRIZIONE") %>'></asp:Label>
                                                                </td>
                                                                <td></td>
                                                            </tr>
                                                            <tr>
                                                                <td></td>
                                                                <td colspan="4">
                                                                    Vaccinazioni:&#160; <asp:Label ID="Label1" runat="server" Text='<%# GetLabelVaccinazioniAgenda(DataBinder.Eval(Container.DataItem,"VACCINAZIONI").ToString(),DataBinder.Eval(Container.DataItem,"BILANCI").ToString(),DataBinder.Eval(Container.DataItem,"ESEGUITA").ToString(),DataBinder.Eval(Container.DataItem,"ESCLUSA").ToString()) %>' />
                                                                </td>
                                                            </tr>
                                                            </table>
                                                        </ItemTemplate>
                                                    </asp:DataList>
                                                    <table class="TextBox_Stringa" style="border-top:1px solid white;" id="Table7" cellspacing="0" cellpadding="0" width="100%">
                                                        <tr>
                                                            <td style="background-color: darkblue">
                                                                <a href="javascript:__doPostBack('EspandilsPomeriggio','true')">
                                                                    <img id="imgls_Pomeriggio" style="cursor: pointer" runat="server" src="~/Images/meno.gif" align="middle" alt="" />
                                                                </a>
                                                                &nbsp; 
                                                                <span style="color: white"><%=oralpom%> </span>
                                                            </td>
                                                            <td style="background-color: darkblue" align="right">
                                                                <a href="javascript:__doPostBack('NuovoOrario','Pom')">
                                                                    <img style="cursor: pointer" alt="Conferma le modifiche apportate agli orari del pomeriggio" runat="server" src="~/Images/conferma.gif" />
                                                                </a>&nbsp; 
                                                                <input id="chkSelDeselPomeriggio" type="checkbox">
                                                                &nbsp; 
                                                                <a href="javascript:PrenotaManuale('lsPomeriggio')">
                                                                    <img style="cursor: pointer" alt="Prenota manualmente i pazienti selezionati" runat="server" src="~/Images/nuovo.gif" />
                                                                </a>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <asp:DataList ID="lsPomeriggio" Style="left: 0; overflow: auto" runat="server" CssClass="TextBox_Stringa" RepeatLayout="Flow">
                                                        <ItemTemplate>
                                                            <%#IIf(Not InRange(CDate(DataBinder.Eval(Container.DataItem, "CNV_DATA_APPUNTAMENTO")).TimeOfDay, oraMinPM.TimeOfDay, oraMaxPM.TimeOfDay, False), "<table  id=""tableLS"" cellSpacing=""0"" cellPadding=""0"" width=""100%"" class=""textbox_stringa"" style=""BORDER-BOTTOM: navy 1px solid;background-color: lightyellow"">", "<table  id=""tableLS"" cellSpacing=""0"" cellPadding=""0"" width=""100%"" class=""textbox_stringa"" style= ""BORDER-BOTTOM: navy 1px solid;background-color: trasparent"">")%> <tr>
                                                                <td width="50px">
                                                                    <table width="100%">
                                                                        <tr>
                                                                            <td style="width:50%"><%#GetImageAvviso(DataBinder.Eval(Container.DataItem, "CNV_DATA_INVIO").ToString())%></td>
                                                                            <td style="width:50%"><%#GetImageAgenda(DataBinder.Eval(Container.DataItem, "SOLLECITO").ToString(), DataBinder.Eval(Container.DataItem, "TP").ToString(), DataBinder.Eval(Container.DataItem, "ESEGUITA").ToString(), DataBinder.Eval(Container.DataItem, "ESCLUSA").ToString())%></td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                                <td width="300px">
                                                                    <asp:CheckBox ID="chkSelezione" runat="server"></asp:CheckBox>
                                                                    <asp:Label ID="Label18" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"PAZ_COGNOME") &amp; " " &amp; DataBinder.Eval(Container.DataItem,"PAZ_NOME") %>'></asp:Label>
                                                                    <asp:Label ID="lblCodice" Style="display: none" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container.DataItem,"PAZ_CODICE") %>'></asp:Label>
                                                                </td>
                                                                <td width="100px">
                                                                    <asp:Label ID="lblDataNascita" runat="server" Text='<%# Cdate(DataBinder.Eval(Container.DataItem,"PAZ_DATA_NASCITA")).ToString("dd/MM/yyyy") %>'></asp:Label>
                                                                    <asp:Label ID="lblCnvData" runat="server" Style="display: none" Text='<%# Cdate(DataBinder.Eval(Container.DataItem,"CNV_DATA")).ToString("dd/MM/yyyy") %>'></asp:Label>
                                                                </td>
                                                                <td>
                                                                    Ore:&nbsp; <asp:TextBox ID="txtOrario" runat="server" Style="width: 50px" CssClass="TextBox_Data" Text='<%# CDate(DataBinder.Eval(Container.DataItem,"CNV_DATA_APPUNTAMENTO")).ToString("HH\:mm") %>'
                                                                        onblur="if (!(orarioValido(this.parentNode.childNodes[1].value))) {alert('Orario non valido!'); this.focus();}"></asp:TextBox>
                                                                </td>
                                                                <td width="30px">
                                                                    <img id="imgTipoApp" src='<%# "../../Images/" & IIf((DataBinder.Eval(Container.DataItem,"CNV_TIPO_APPUNTAMENTO")).ToString = "A", "appuntamenti.gif", "annotazioni.gif") %>'
                                                                        alt='<%# IIf((DataBinder.Eval(Container.DataItem,"CNV_TIPO_APPUNTAMENTO")).ToString = "A", "Appuntamento assegnato automaticamente", "Appuntamento assegnato manualmente") %>' />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td></td>
                                                                <td>
                                                                    Conv:&#160; <asp:Label ID="lblData" runat="server" Text='<%# CDate(DataBinder.Eval(Container.DataItem,"cnv_Data")).Date.ToString("dd/MM/yyyy") %>'></asp:Label>
                                                                </td>
                                                                <td>
                                                                    Durata:&#160; <asp:Label ID="Label25" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"cnv_durata_appuntamento") %>'></asp:Label>
                                                                </td>
                                                                <td>
                                                                    Medico:&#160; <asp:Label ID="Label26" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"med_descrizione") %>'></asp:Label></td>
                                                                <td></td>
                                                            </tr>
                                                            <tr>
                                                                <td></td>
                                                                <td colspan="4">
                                                                    Vaccinazioni:&#160; <asp:Label ID="Label3" runat="server" Text='<%# GetLabelVaccinazioniAgenda(DataBinder.Eval(Container.DataItem,"VACCINAZIONI").ToString(),DataBinder.Eval(Container.DataItem,"BILANCI").ToString(),DataBinder.Eval(Container.DataItem,"ESEGUITA").ToString(),DataBinder.Eval(Container.DataItem,"ESCLUSA").ToString()) %>'></asp:Label>
                                                                </td>
                                                            </tr>
                                                            </table>
                                                        </ItemTemplate>
                                                    </asp:DataList>
                                                </ContentTemplate>
                                            </igtab:Tab>
                                            <igtab:Tab Key="Opzioni" Text="Opzioni">
                                                <ContentTemplate>
                                                    <asp:Panel ID="pnlOpzioniAppuntamenti" runat="server" BorderStyle="None" Height="110px">
                                                        <table cellspacing="0" cellpadding="0" width="100%">
                                                            <tr>
                                                                <td>
                                                                    <div class="TextBox_Stringa" style="width: 100%; border-bottom: navy 1px solid; height: 16px" >
                                                                        Visualizza</div>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <table cellspacing="0" cellpadding="0" border="0">
                                                                        <tr>
                                                                            <td style="width:128px">
                                                                                <asp:CheckBox ID="chkAppConvocazione" runat="server" CssClass="TextBox_Stringa" Text="Convocazioni"></asp:CheckBox>
                                                                            </td>
                                                                            <td style="width:136px">
                                                                                <asp:CheckBox ID="chkAppDurata" runat="server"  CssClass="TextBox_Stringa" Text="Durata"></asp:CheckBox>
                                                                            </td>
                                                                            <td valign="middle" align="center" rowspan="2">
                                                                                <asp:Button ID="btnAppApplica" runat="server" Height="24px" Width="82px" Text="Applica"></asp:Button>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="width:128px">
                                                                                <asp:CheckBox ID="chkAppMedico" runat="server" CssClass="TextBox_Stringa" Text="Medico"></asp:CheckBox>
                                                                            </td>
                                                                            <td style="width:136px">
                                                                                <asp:CheckBox ID="chkAppVaccinazioni" runat="server" CssClass="TextBox_Stringa" Text="Vaccinazioni">
                                                                                </asp:CheckBox>
                                                                            </td>
                                                                            <td>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td height="10">
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <div class="TextBox_Stringa" style="width: 100%; border-bottom: navy 1px solid; height: 16px">
                                                                        Opzioni</div>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:CheckBox ID="chkAppInBilancio" runat="server" Height="16px" Width="300px" CssClass="TextBox_Stringa" Text="Visualizza anche elementi con solo bilancio"
                                                                        AutoPostBack="True"></asp:CheckBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </asp:Panel>
                                                </ContentTemplate>
                                            </igtab:Tab>
                                        </Tabs>
                                    </igtab:UltraWebTab>
                                </div>
                            </td>
                        </tr>
                    </table>

                    <asp:textbox style="display: none" id="txtEspanso" runat="server" CssClass="label"></asp:textbox>
                    <asp:textbox style="display: none" id="txtFiltri" runat="server" CssClass="label"></asp:textbox>

                </dyp:DynamicPanel>

                <dyp:DynamicPanel ID="dypMsg" runat="server" Width="100%" Height="18px">
                    <div style="height: 18px; width: 100%; border: 1px solid #8080FF;" class="alert">
					    <asp:Label id="lblMessaggi" runat="server" ></asp:Label>
                    </div>
                </dyp:DynamicPanel>

            </on_lay3:OnitLayout3>

            <on_ofm:OnitFinestraModale ID="fmPopUp" runat="server" Title="Errore"  Width="480px" BackColor="LightGray" NoRenderX="True">
                <table border="0" width="100%">
                    <tr height="40">
                        <td valign="middle">
                            <asp:Label ID="lblMessaggio" runat="server" Height="40px" Width="460px" CssClass="label_center" Font-Bold="True"></asp:Label>
                        </td>
                    </tr>
                    <tr height="30">
                        <td valign="middle">
                            <div align="center">
                                <asp:Button ID="btnOrariApertura" runat="server" Text="Chiudi"></asp:Button>
                            </div>
                        </td>
                    </tr>
                </table>
            </on_ofm:OnitFinestraModale>

            <on_ofm:OnitFinestraModale ID="fmLog" Title="Log delle Operazioni" runat="server" Width="600px"  BackColor="LightGray" NoRenderX="True">
                <table border="0" cellspacing="0" cellpadding="0" width="100%" height="100%">
                    <tr>
                        <td height="90%" width="100%">
                            <div style="width: 100%; height: 450px; overflow: auto" align="center">
                                <asp:Label ID="lblLog" runat="server" CssClass="label_left" EnableViewState="False"></asp:Label>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td height="10%" width="100%" align="center">
                            <asp:Button ID="btnChiudiLog" runat="server" Text="Chiudi"></asp:Button>&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnStampaLog" runat="server" Text="Stampa"></asp:Button>
                        </td>
                    </tr>
                </table>
            </on_ofm:OnitFinestraModale>

            <on_ofm:OnitFinestraModale ID="fmFiltroAssociazioniDosi" Title="<div style=&quot;font-family:'Microsoft Sans Serif';text-align:center; font-size: 11pt;padding-bottom:2px&quot;>Seleziona le associazioni e le dosi per cui filtrare</div>"
                runat="server" Width="532px" BackColor="LightGray" RenderModalNotVisible="True" UseDefaultTab="True" NoRenderX="true">
                <table border="0" cellspacing="0" cellpadding="0" width="100%" style="background-color: LightGrey; width: 532px; ">
                    <colgroup>
                        <col width="1%" />
                        <col width="45%" />
                        <col width="8%" />
                        <col width="45%" />
                        <col width="1%" />
                    </colgroup>
                    <tr>
                        <td></td>
                        <td colspan="3">
                            <uc1:UscFiltroPrenotazioneSelezioneMultipla ID="UscFiltroAssociazioniDosi" runat="server" Tipo="Associazioni_Dosi" EscludiObsoleti="true"></uc1:UscFiltroPrenotazioneSelezioneMultipla>
                        </td>
                        <td></td>
                    </tr>
                    <tr height="10">
                        <td colspan="5"></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td align="right">
                            <asp:Button Style="cursor: pointer" ID="btnOk_FiltroAssociazioniDosi" runat="server"
                                Width="100px" Text="OK"></asp:Button>
                        </td>
                        <td></td>
                        <td>
                            <asp:Button Style="cursor: pointer" ID="btnAnnulla_FiltroAssociazioniDosi" runat="server"
                                Width="100px" Text="Annulla"></asp:Button>
                        </td>
                        <td></td>
                    </tr>
                    <tr height="10">
                        <td colspan="5"></td>
                    </tr>
                </table>
            </on_ofm:OnitFinestraModale>

            <on_ofm:OnitFinestraModale ID="fmFiltroCicliSedute" Title="<div style=&quot;font-family:'Microsoft Sans Serif';text-align:center; font-size: 11pt;padding-bottom:2px&quot;>Seleziona i cicli e le sedute per cui filtrare</div>"
                runat="server" Width="532px" BackColor="LightGray" RenderModalNotVisible="True" UseDefaultTab="True">
                <table border="0" cellspacing="0" cellpadding="0" width="100%" style="background-color: LightGrey;
                    width: 532px; height: 60px">
                    <colgroup>
                        <col width="1%" />
                        <col width="45%" />
                        <col width="8%" />
                        <col width="45%" />
                        <col width="1%" />
                    </colgroup>
                    <tr>
                        <td>&nbsp;</td>
                        <td colspan="3">
                            <uc1:UscFiltroPrenotazioneSelezioneMultipla ID="UscFiltroCicliSedute" runat="server" Tipo="Cicli_Sedute" EscludiObsoleti="true"></uc1:UscFiltroPrenotazioneSelezioneMultipla>
                        </td>
                        <td>&nbsp;</td>
                    </tr>
                    <tr height="10">
                        <td colspan="5">&nbsp;</td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td align="right">
                            <asp:Button Style="cursor: pointer" ID="btnOk_FiltroCicliSedute" runat="server" Width="100px" Text="OK"></asp:Button>
                        </td>
                        <td>&nbsp;</td>
                        <td>
                            <asp:Button Style="cursor: pointer" ID="btnAnnulla_FiltroCicliSedute" runat="server" Width="100px" Text="Annulla"></asp:Button>
                        </td>
                        <td>&nbsp;</td>
                    </tr>
                    <tr height="10">
                        <td colspan="5">&nbsp;</td>
                    </tr>
                </table>
            </on_ofm:OnitFinestraModale>

            <on_ofm:OnitFinestraModale ID="fmOrariPersonalizzati" Title="<div style=&quot;font-family:'Microsoft Sans Serif';text-align:center; font-size: 11pt;padding-bottom:2px&quot;><img src='../../images/appuntamenti.gif'>Orari di prenotazione</div>"
                runat="server" Width="540px" BackColor="LightGray" NoRenderX="True" RenderModalNotVisible="True">
                <table border="0" cellspacing="0" cellpadding="0" width="100%">
                    <tr height="3">
                        <td colspan="5"></td>
                    </tr>
                    <tr height="22">
                        <td width="1%"></td>
                        <td>
                            <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="tlbOrariPersonalizzati" runat="server" ItemWidthDefault="90px"
                                CssClass="infratoolbar">
                                <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                                <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                                <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                                <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
                                <Items>
                                    <igtbar:TBarButton Key="btnConfermaOrariPers" ToolTip="Conferma gli orari di prenotazione inseriti"
                                        Text="Conferma" DisabledImage="~/Images/Conferma_dis.gif" Image="~/Images/Conferma.gif">
                                    </igtbar:TBarButton>
                                    <igtbar:TBarButton Key="btnAnnullaOrariPers" ToolTip="Annulla le modifiche agli orari di prenotazione"
                                        Text="Annulla" DisabledImage="~/Images/annullaConf_dis.gif" Image="~/Images/annulla.gif">
                                    </igtbar:TBarButton>
                                    <igtbar:TBSeparator></igtbar:TBSeparator>
                                    <igtbar:TBarButton Key="btnAggiungiOrariPers" ToolTip="Aggiunge una fascia oraria di prenotazione"
                                        Text="Aggiungi orari" DisabledImage="~/Images/nuovo_dis.gif" Image="~/Images/nuovo.gif">
                                        <DefaultStyle Width="110px" CssClass="infratoolbar_button_default">
                                        </DefaultStyle>
                                    </igtbar:TBarButton>
                                </Items>
                            </igtbar:UltraWebToolbar>
                        </td>
                        <td width="1%"></td>
                    </tr>
                    <tr height="5">
                        <td colspan="3"></td>
                    </tr>
                    <tr height="323">
                        <td></td>
                        <td>
                            <div style="border: navy 1px solid; height: 320px; overflow: auto;">
                                <on_dgr:OnitGrid Style="table-layout: fixed" ID="dgrOrariPersonalizzati" runat="server"
                                    Width="100%" CssClass="datagrid" AutoGenerateColumns="False" SortedColumns="Matrice IGridColumn[]"
                                    PagerVoicesBefore="-1" PagerVoicesAfter="-1" SelectionOption="none">
                                    <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
                                    <ItemStyle CssClass="item"></ItemStyle>
                                    <HeaderStyle CssClass="header"></HeaderStyle>
                                    <Columns>
                                        <asp:ButtonColumn Text="&lt;img src='../../images/elimina.gif' title='Elimina fascia oraria'&gt;" CommandName="Delete">
                                            <HeaderStyle Width="4%"></HeaderStyle>
                                        </asp:ButtonColumn>
                                        <asp:TemplateColumn HeaderText="Giorno">
                                            <HeaderStyle Width="30%"></HeaderStyle>
                                            <ItemTemplate>
                                                <asp:DropDownList ID="ddlGiorno" runat="server" Width="90%">
                                                    <asp:ListItem Value="LUN">Luned&#236;</asp:ListItem>
                                                    <asp:ListItem Value="MAR">Marted&#236;</asp:ListItem>
                                                    <asp:ListItem Value="MER">Mercoled&#236;</asp:ListItem>
                                                    <asp:ListItem Value="GIO">Gioved&#236;</asp:ListItem>
                                                    <asp:ListItem Value="VEN">Venerd&#236;</asp:ListItem>
                                                    <asp:ListItem Value="SAB">Sabato</asp:ListItem>
                                                    <asp:ListItem Value="DOM">Domenica</asp:ListItem>
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="Ora Inizio">
                                            <HeaderStyle HorizontalAlign="Center" Width="18%"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtOraInizio" onblur="checkFormatoOrario(this)" runat="server" Width="85%"
                                                    CssClass="textbox_data" Text='<%# IIf(DataBinder.Eval(Container.DataItem,"ORP_ORA_INIZIO").ToString="","",string.format("{0:HH.mm}",DataBinder.Eval(Container.DataItem,"ORP_ORA_INIZIO"))) %>'
                                                    MaxLength="5">
                                                </asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="Ora Fine">
                                            <HeaderStyle HorizontalAlign="Center" Width="18%"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtOraFine" onblur="checkFormatoOrario(this)" runat="server" Width="85%"
                                                    CssClass="textbox_data" Text='<%# IIf(DataBinder.Eval(Container.DataItem,"ORP_ORA_FINE").ToString()="","",string.format("{0:HH.mm}",DataBinder.Eval(Container.DataItem,"ORP_ORA_FINE"))) %>'
                                                    MaxLength="5">
                                                </asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="Pazienti">
                                            <HeaderStyle Width="15%" HorizontalAlign="Center"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtNumPaz" onblur="checkFormatoNumerico(this)" runat="server" Width="85%"
                                                    CssClass="textbox_data" Text='<%# DataBinder.Eval(Container.DataItem,"ORP_NUM_PAZIENTI").ToString() %>'
                                                    MaxLength="3">
                                                </asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="Durata">
                                            <HeaderStyle Width="15%" HorizontalAlign="Center"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtDurataPers" onblur="checkFormatoNumerico(this)" Width="85%" runat="server"
                                                    CssClass="textbox_data" Text='<%# DataBinder.Eval(Container.DataItem,"ORP_DURATA").ToString() %>'
                                                    MaxLength="3">
                                                </asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                        <on_dgr:OnitBoundColumn Visible="False" DataField="ORP_CODICE" HeaderText="H_Codice" key="Codice" SortDirection="NoSort"></on_dgr:OnitBoundColumn>
                                    </Columns>
                                </on_dgr:OnitGrid>
                            </div>
                        </td>
                        <td></td>
                    </tr>
                    <tr height="60">
                        <td></td>
                        <td style="border-bottom: navy 1px solid; border-left: navy 1px solid; background-color: whitesmoke;
                            font-family: verdana; color: blue; font-size: 12px; border-top: navy 1px solid;
                            font-weight: bold; border-right: navy 1px solid" class="textbox_data">
                            Impostando gli orari personalizzati, la prenotazione degli appuntamenti non terrà
                            conto degli orari standard dell'ambulatorio
                        </td>
                        <td></td>
                    </tr>
                    <tr height="5">
                        <td colspan="3">
                        </td>
                    </tr>
                </table>
            </on_ofm:OnitFinestraModale>

            <!-- Modale per lo spostamento dell'appuntamento -->
            <on_ofm:OnitFinestraModale ID="fmSpostaPrenotazioni" Title="Sposta prenotazioni" runat="server" Width="500px" 
                BackColor="LightGray" RenderModalNotVisible="True" UseDefaultTab="True">
                <fieldset class="fldroot" id="fldSpostaPrenotazione" runat="server">
                    <legend id="Legend1" class="label" runat="server">Selezionare la data di spostamento</legend>
                    <table border="0" cellspacing="5" cellpadding="5" width="100%">
                        <tr>
                            <td align="center">
                                <on_val:OnitDatePick ID="dpkDataSpostamento" runat="server" Height="20px" Width="120px"
                                    CssClass="textbox_data" DateBox="True"></on_val:OnitDatePick>
                            </td>
                        </tr>
                    </table>
                </fieldset>
                <uc2:AjaxList ID="SpostaSceltaAmb" runat="server" Label="Nuovo Ambulatorio" CampoCodice="amb_codice"
                    CampoDescrizione="amb_descrizione" Tabella="t_ana_ambulatori" PostBackOnSelect="False">
                </uc2:AjaxList>
                <table border="0" cellspacing="5" cellpadding="5" width="100%">
                    <tr>
                        <td align="center">
                            <asp:Button Style="cursor: pointer" ID="btnOkSposta" runat="server" Width="100px" Text="Sposta">
                            </asp:Button>
                        </td>
                        <td align="center">
                            <asp:Button Style="cursor: pointer" ID="btnAnnullaSposta" runat="server" Width="100px"
                                Text="Annulla"></asp:Button>
                        </td>
                    </tr>
                </table>
            </on_ofm:OnitFinestraModale>
    
            <!-- Modale di selezione dell'ambulatorio -->
            <on_ofm:OnitFinestraModale ID="fmSceltaAmb" Title="Selezione Ambulatorio" runat="server" NoRenderX="True" 
                Width="500px" BackColor="LightGray" RenderModalNotVisible="True" UseDefaultTab="True">
                <uc2:AjaxList ID="ambAjaxlist" runat="server" Label="Ambulatorio" CampoCodice="amb_codice"
                    CampoDescrizione="amb_descrizione" Tabella="t_ana_ambulatori" PostBackOnSelect="True">
                </uc2:AjaxList>
            </on_ofm:OnitFinestraModale>
    
            <!-- Modale di conferma avvio ricerca convocati -->
            <on_ofm:OnitFinestraModale ID="fmAvviaRicerca" Title="Ricerca convocati" runat="server" NoRenderX="False" Width="300px" 
                BackColor="LightGray" RenderModalNotVisible="True" UseDefaultTab="True">
                <table border="0" cellspacing="5" cellpadding="0" width="100%">
                    <colgroup>
                        <col width="5%" />
                        <col align="center" width="90%" />
                        <col width="5%" />
                    </colgroup>
                    <tr>
                        <td colspan="3"></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td>
                            <fieldset style="border: navy 1px solid; width: 100%;">
                                <legend style="border: navy 1px solid; padding: 3px; background-color: aliceblue; color: navy; font-weight: bold;" class="label">Operazioni da effettuare</legend>
                                <table border="0" cellspacing="3" cellpadding="0" width="100%">
                                    <colgroup>
                                        <col width="5%" />
                                        <col style="color: navy" width="90%" />
                                        <col width="5%" />
                                    </colgroup>
                                    <tr>
                                        <td colspan="3"></td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td>
                                            <asp:Label ID="lblAvviaRicerca_Solleciti" runat="server" CssClass="label_left">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
													        - Controllo Nuovi Solleciti</asp:Label>
                                        </td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td class="label_left">
                                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; - Ricerca convocati
                                        </td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td colspan="3"></td>
                                    </tr>
                                </table>
                            </fieldset>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td colspan="3"></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td align="center">
                            <asp:Button Style="cursor: pointer" ID="btnAvviaRicerca_OperazioniERicerca" runat="server"
                                Width="150px" Text="Operazioni + Ricerca"></asp:Button>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td align="center">
                            <asp:Button Style="cursor: pointer" ID="btnAvviaRicerca_SoloRicerca" runat="server"
                                Width="150px" Text="Avvia solo la ricerca"></asp:Button>
                        </td>
                        <td></td>
                    </tr>
                </table>
            </on_ofm:OnitFinestraModale>

            <!-- consenso trattamento dati utente -->
            <on_ofm:OnitFinestraModale ID="fmConsensoUtente" Title="Consenso al trattamento dati per l'utente" runat="server" Width="400px" Height="250px" BackColor="LightGoldenrodYellow">
                <uc1:ConsensoUtente runat="server" id="ucConsensoUtente"></uc1:ConsensoUtente>
            </on_ofm:OnitFinestraModale>

        </asp:View>

        <asp:View ID="viewGestioneSolleciti" runat="server">

           <div>
               <div id="Div1" class="Title" runat="server">Gestione appuntamenti - Solleciti</div>
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBarGestioneSolleciti" runat="server" CssClass="infratoolbar" ItemWidthDefault="80px" >
			        <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
			        <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
			        <Items>
				        <igtbar:TBarButton Key="Procedi" Text="Procedi" DisabledImage="~/Images/genera_dis.gif" Image="~/Images/genera.gif" />
				        <igtbar:TBarButton Key="Annulla" Text="Annulla" DisabledImage="~/Images/annulla_dis.gif" Image="~/Images/annulla.gif" />
				        <igtbar:TBSeparator />
				        <igtbar:TBarButton Key="Stampa" Text="Stampa" DisabledImage="~/Images/stampa_dis.gif" Image="~/Images/stampa.gif" />          
                    </Items>
		        </igtbar:UltraWebToolbar>
                <p class="label" style="margin: 10px; font-weight: bold; text-align: center; font-size: 14px">
                    I pazienti elencati non si sono presentati all'appuntamento e saranno processati automaticamente.<br/>
                    Le modifiche non saranno annullabili. Premere "Procedi" per continuare. <br/>
                </p>
           </div>

           <dyp:DynamicPanel ID="dypTab" runat="server" Width="100%" Height="100%" ScrollBars="Auto" OnResizeClientHandler="resizeTab" >
                    
                <igtab:UltraWebTab BrowserTarget="UpLevel" id="ultraTabRiepilogo" runat="server" BorderStyle="Solid" Width="98%"  ThreeDEffect="False" BorderColor="#949878" BorderWidth="1px">
			        <DefaultTabStyle Height="22px" Font-Size="8pt" Font-Names="Microsoft Sans Serif" ForeColor="Black" BackColor="#FEFCFD">
				        <Padding Top="2px"></Padding>
			        </DefaultTabStyle>
			        <RoundedImage LeftSideWidth="7" RightSideWidth="6" ShiftOfImages="2" SelectedImage="ig_tab_winXP1.gif"
				        NormalImage="ig_tab_winXP3.gif" HoverImage="ig_tab_winXP2.gif" FillStyle="LeftMergedWithCenter">
                    </RoundedImage>
			        <SelectedTabStyle>
				        <Padding Bottom="2px"></Padding>
			        </SelectedTabStyle>
			        <Tabs>
				        <igtab:Tab Text="Pazienti in sollecito" Key="tabPazientiDaSollecitare" >
					        <ContentTemplate>
                                    <div id="tab1Fixed">
                                        <asp:Image ID="imgPazientiDaSollecitare" runat="server" ImageUrl="~/Images/avanti.gif" />&nbsp;
                                        <asp:Label ID="lblPazientiDaSollecitare" runat="server" CssClass="label_left">Pazienti a cui verrà creato il sollecito indicato (i pazienti domiciliati con massimo numero di solleciti verranno resi inadempienti).</asp:Label>
                                        <asp:Label ID="lblNessunPazienteDaSollecitare" runat="server" CssClass="label_left" Font-Bold="True" Visible="False">Nessun Paziente</asp:Label>
                                        <asp:Label ID="lblPazSollLimitato" runat="server" CssClass="label_left" Font-Bold="True" ForeColor="Red" Visible="False">Numero pazienti limitato a </asp:Label>
                                    </div>
                                    <div id="tab1Scroll"  class="divRiepilogoOverflow" >
                                        <div class="divRiepilogoBorder" id="divPazientiDaSollecitare" runat="server" style="width:99%">
                                            <asp:DataGrid ID="dgrPazientiDaSollecitare" runat="server" BorderStyle="None" Width="100%" 
                                                BorderWidth="0px" CssClass="DataGrid" GridLines="None" AutoGenerateColumns="False" EnableViewState="false">
                                                <SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
                                                <EditItemStyle CssClass="Edit"></EditItemStyle>
                                                <AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
                                                <ItemStyle CssClass="Item"></ItemStyle>
                                                <HeaderStyle CssClass="Header"></HeaderStyle>
                                                <PagerStyle CssClass="Pager"></PagerStyle>
                                                <Columns>
                                                    <asp:BoundColumn DataField="Cognome" HeaderText="Cognome"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="Nome" HeaderText="Nome"></asp:BoundColumn>
                                                    <asp:BoundColumn HeaderText="Sollecito"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="DataNascita" HeaderText="Data Nascita" DataFormatString="{0:dd/MM/yyyy}"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="DataConvocazione" HeaderText="Data Cnv." DataFormatString="{0:dd/MM/yyyy}"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="DataAppuntamento" HeaderText="Data App." DataFormatString="{0:dd/MM/yyyy HH:mm}"></asp:BoundColumn>
                                                    <asp:BoundColumn DataField="DescStatoAnagrafico" HeaderText="Stato Anagr."></asp:BoundColumn>
                                                    <asp:TemplateColumn HeaderText="Stato">
                                                        <ItemTemplate>
                                                            <asp:Image ID="imgDescrizioneStatoPazientiDaSollecitare" runat="server" Style="cursor: pointer;" />
                                                        </ItemTemplate>
                                                    </asp:TemplateColumn>
                                                </Columns>
                                            </asp:DataGrid>
                                        </div>
                                    </div>
                            </ContentTemplate>
                        </igtab:Tab>
                        <igtab:Tab Text="Pazienti inadempienti" Key="tabPazientiInTerminePerentorio" >
					        <ContentTemplate>
                                <div  id="tab2Fixed">
                                    <asp:Image ID="imgPazientiInTerminePerentorio" runat="server" ImageUrl="~/Images/avanti.gif" />&nbsp;
                                    <asp:Label ID="lblPazientiInTerminePerentorio" runat="server" CssClass="label_left">Pazienti in Termine Perentorio che diventeranno inadempienti (stato Comunicazione al Sindaco).</asp:Label>
                                    <asp:Label ID="lblNessunPazienteTerminePerentorio" runat="server" CssClass="label_left" Font-Bold="True" DESIGNTIMEDRAGDROP="232" Visible="False">Nessun Paziente</asp:Label>
                                    <asp:Label ID="lblPazTPLimitato" runat="server" CssClass="label_left" Font-Bold="True" ForeColor="Red" Visible="False">Numero pazienti limitato a </asp:Label>
                                </div>
                                <div class="divRiepilogoOverflow"  id="tab2Scroll">
                                    <div class="divRiepilogoBorder" id="divPazientiInTerminePerentorio" runat="server"  style="width:99%">
                                        <asp:DataGrid ID="dgrPazientiInTerminePerentorio" runat="server" BorderStyle="None"
                                            Width="100%" BorderWidth="0px" CssClass="DataGrid" GridLines="None" AutoGenerateColumns="False" EnableViewState="false">
                                            <SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
                                            <EditItemStyle CssClass="Edit"></EditItemStyle>
                                            <AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
                                            <ItemStyle CssClass="Item"></ItemStyle>
                                            <HeaderStyle CssClass="Header"></HeaderStyle>
                                            <PagerStyle CssClass="Pager"></PagerStyle>
                                            <Columns>
                                                <asp:BoundColumn DataField="Cognome" HeaderText="Cognome"></asp:BoundColumn>
                                                <asp:BoundColumn DataField="Nome" HeaderText="Nome"></asp:BoundColumn>
                                                <asp:BoundColumn DataField="DataNascita" HeaderText="Data Nascita" DataFormatString="{0:dd/MM/yyyy}"></asp:BoundColumn>
                                                <asp:BoundColumn DataField="DataConvocazione" HeaderText="Data Convocazione" DataFormatString="{0:dd/MM/yyyy}"></asp:BoundColumn>
                                                <asp:BoundColumn DataField="DataInvio" HeaderText="Data Invio T.P." DataFormatString="{0:dd/MM/yyyy}"></asp:BoundColumn>
                                                <asp:TemplateColumn HeaderText="Stato">
                                                    <ItemTemplate>
                                                        <asp:Image ID="imgDescrizioneStatoTerminePerentorio" runat="server" Style="cursor: pointer;" />
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                            </Columns>
                                        </asp:DataGrid>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </igtab:Tab>
                        <igtab:Tab Text="Vaccinazioni escluse" Key="tabPazientiElimProgRicalcolaConv" >
					        <ContentTemplate>
                                <div  id="tab3Fixed">
                                    <asp:Image ID="imgPazientiElimProgRicalcolaConv" runat="server" ImageUrl="~/Images/avanti.gif" />&nbsp;
                                    <asp:Label ID="lblPazientiElimProgRicalcolaConv" runat="server" CssClass="label_left">Pazienti a cui verranno escluse le vaccinazioni con il parametro sulla seduta dei solleciti raccomandati valorizzato.</asp:Label>
                                    <asp:Label ID="lblNessunPazienteElimProgRicalcolaConv" runat="server" CssClass="label_left" Font-Bold="True" DESIGNTIMEDRAGDROP="246" Visible="False">Nessun Paziente</asp:Label>
                                    <asp:Label ID="lblPazElimProgLimitato" runat="server" CssClass="label_left" Font-Bold="True" ForeColor="Red" Visible="False">Numero pazienti limitato a </asp:Label>
                                </div>
                                <div class="divRiepilogoOverflow" id="tab3Scroll">
                                    <div class="divRiepilogoBorder" id="DivPazientiElimProgRicalcolaConv" runat="server" style="width:99%">
                                        <asp:DataGrid ID="dgrPazientiVaccinazioniRaccomandate" runat="server" BorderStyle="None"
                                            Width="100%" BorderWidth="0px" CssClass="DataGrid" GridLines="None" AutoGenerateColumns="False" EnableViewState="false">
                                            <SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
                                            <EditItemStyle CssClass="Edit"></EditItemStyle>
                                            <AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
                                            <ItemStyle CssClass="Item"></ItemStyle>
                                            <HeaderStyle CssClass="Header"></HeaderStyle>
                                            <PagerStyle CssClass="Pager"></PagerStyle>
                                            <Columns>
                                                <asp:BoundColumn DataField="Cognome" HeaderText="Cognome"></asp:BoundColumn>
                                                <asp:BoundColumn DataField="Nome" HeaderText="Nome"></asp:BoundColumn>
                                                <asp:BoundColumn DataField="DataNascita" HeaderText="Data Nascita" DataFormatString="{0:dd/MM/yyyy}"></asp:BoundColumn>
                                                <asp:BoundColumn DataField="DataConvocazione" HeaderText="Data Convocazione" DataFormatString="{0:dd/MM/yyyy}"></asp:BoundColumn>
                                                <asp:TemplateColumn HeaderText="Stato">
                                                    <ItemTemplate>
                                                        <asp:Image ID="imgDescrizioneStatoVaccinazioniRaccomandate" runat="server" Style="cursor: pointer;" />
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                            </Columns>
                                        </asp:DataGrid>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </igtab:Tab>
                        <igtab:Tab Text="Vaccinazioni non obbligatorie" Key="tabPazientiPosticipaVaccNonObbl">
					        <ContentTemplate>
                                <div id="tab4Fixed">
                                    <asp:Image ID="imgPazientiPosticipaVaccNonObbl" runat="server" ImageUrl="~/Images/avanti.gif" />&nbsp;
                                    <asp:Label ID="lblPazientiPosticipaVaccNonObbl" runat="server" CssClass="label_left">Pazienti con vaccinazioni non obbligatorie.</asp:Label>
                                    <asp:Label ID="lblNessunPazientePosticipaVaccNonObbl" runat="server" CssClass="label_left" Font-Bold="True" Visible="False">Nessun Paziente</asp:Label>
                                    <asp:Label ID="lblPazPostLimitato" runat="server" CssClass="label_left" Font-Bold="True" ForeColor="Red" Visible="False">Numero pazienti limitato a </asp:Label>
                                </div>
                                <div class="divRiepilogoOverflow" id="tab4Scroll">
                                    <div class="divRiepilogoBorder" id="divPazientiPosticipaVaccNonObbl" runat="server"  style="width:99%">
                                        <asp:DataGrid ID="dgrPazientiVaccinazioniNonObbligatorie" runat="server" BorderStyle="None"
                                            Width="100%" BorderWidth="0px" CssClass="DataGrid" GridLines="None" AutoGenerateColumns="False" EnableViewState="false">
                                            <SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
                                            <EditItemStyle CssClass="Edit"></EditItemStyle>
                                            <AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
                                            <ItemStyle CssClass="Item"></ItemStyle>
                                            <HeaderStyle CssClass="Header"></HeaderStyle>
                                            <PagerStyle CssClass="Pager"></PagerStyle>
                                            <Columns>
                                                <asp:BoundColumn DataField="Cognome" HeaderText="Cognome"></asp:BoundColumn>
                                                <asp:BoundColumn DataField="Nome" HeaderText="Nome"></asp:BoundColumn>
                                                <asp:BoundColumn DataField="DataNascita" HeaderText="Data Nascita" DataFormatString="{0:dd/MM/yyyy}"></asp:BoundColumn>
                                                <asp:BoundColumn DataField="DataConvocazione" HeaderText="Data Convocazione" DataFormatString="{0:dd/MM/yyyy}"></asp:BoundColumn>
                                                <asp:TemplateColumn HeaderText="Stato">
                                                    <ItemTemplate>
                                                        <asp:Image ID="imgDescrizioneStatoVaccinazioniNonObbligatorie" runat="server" Style="cursor: pointer;" />
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                            </Columns>
                                        </asp:DataGrid>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </igtab:Tab>
                        <igtab:Tab Text="Vaccinazioni non associate a ciclo" Key="tabPazientiNoCiclo">
					        <ContentTemplate>
                                <div id="tab5Fixed">
                                    <asp:Image ID="imgPazientiNoCiclo" runat="server" ImageUrl="~/Images/avanti.gif" />&nbsp;
                                    <asp:Label ID="lblPazientiNoCiclo" runat="server" CssClass="label_left">Pazienti con vaccinazioni non associate a nessun ciclo.</asp:Label>
                                    <asp:Label ID="lblNessunPazienteNoCiclo" runat="server" CssClass="label_left" Font-Bold="True" Visible="False">Nessun Paziente</asp:Label>
                                    <asp:Label ID="lblPazNoCicloLimitato" runat="server" CssClass="label_left" Font-Bold="True" ForeColor="Red" Visible="False">Numero pazienti limitato a </asp:Label>
                                </div>
                                <div class="divRiepilogoOverflow" id="tab5Scroll">
                                    <div class="divRiepilogoBorder" id="divPazientiNoCiclo" runat="server" style="width:99%">
                                        <asp:DataGrid ID="dgrPazientiNoCiclo" runat="server" BorderStyle="None" Width="100%" BorderWidth="0px" CssClass="DataGrid" GridLines="None" AutoGenerateColumns="False" EnableViewState="false">
                                            <SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
                                            <EditItemStyle CssClass="Edit"></EditItemStyle>
                                            <AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
                                            <ItemStyle CssClass="Item"></ItemStyle>
                                            <HeaderStyle CssClass="Header"></HeaderStyle>
                                            <PagerStyle CssClass="Pager"></PagerStyle>
                                            <Columns>
                                                <asp:BoundColumn DataField="Cognome" HeaderText="Cognome"></asp:BoundColumn>
                                                <asp:BoundColumn DataField="Nome" HeaderText="Nome"></asp:BoundColumn>
                                                <asp:BoundColumn DataField="DataNascita" HeaderText="Data Nascita" DataFormatString="{0:dd/MM/yyyy}">
                                                </asp:BoundColumn>
                                                <asp:BoundColumn DataField="DataConvocazione" HeaderText="Data Convocazione" DataFormatString="{0:dd/MM/yyyy}">
                                                </asp:BoundColumn>
                                                <asp:TemplateColumn HeaderText="Stato">
                                                    <ItemTemplate>
                                                        <asp:Image ID="imgDescrizioneStatoNoCiclo" runat="server" Style="cursor: pointer;" />
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                            </Columns>
                                        </asp:DataGrid>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </igtab:Tab>
                        <igtab:Tab Text="Pazienti disallineati" Key="tabPazientiDisallineati" >
					        <ContentTemplate>
                                <div id="tab6Fixed">
                                    <asp:Image ID="Image3" runat="server" ImageUrl="~/Images/avanti.gif" />&nbsp;
                                    <asp:Label ID="lblPazientiDisallineati" runat="server" CssClass="label_left">Pazienti in stato di acquisizione incompleto.</asp:Label>
                                    <asp:Label ID="lblNessunPazienteDisallineati" runat="server" CssClass="label_left" Font-Bold="True" Visible="False">Nessun Paziente</asp:Label>
                                    <asp:Label ID="lblPazientiLimitatiDisallineati" runat="server" CssClass="label_left" Font-Bold="True" ForeColor="Red" Visible="False">Numero pazienti limitato a </asp:Label>
                                </div>
                                <div class="divRiepilogoOverflow" id="tab6Scroll">
                                    <div class="divRiepilogoBorder" id="divPazientiDisallineati" runat="server" style="width:99%">
                                        <asp:DataGrid ID="dgrDisallineati" runat="server" BorderStyle="None" Width="100%" BorderWidth="0px" CssClass="DataGrid" GridLines="None" AutoGenerateColumns="False" EnableViewState="false">
                                            <SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
                                            <EditItemStyle CssClass="Edit"></EditItemStyle>
                                            <AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
                                            <ItemStyle CssClass="Item"></ItemStyle>
                                            <HeaderStyle CssClass="Header"></HeaderStyle>
                                            <PagerStyle CssClass="Pager"></PagerStyle>
                                            <Columns>
                                                <asp:BoundColumn DataField="Cognome" HeaderText="Cognome"></asp:BoundColumn>
                                                <asp:BoundColumn DataField="Nome" HeaderText="Nome"></asp:BoundColumn>
                                                <asp:BoundColumn DataField="DataNascita" HeaderText="Data Nascita" DataFormatString="{0:dd/MM/yyyy}">
                                                </asp:BoundColumn>
                                                <asp:BoundColumn DataField="DataConvocazione" HeaderText="Data Convocazione" DataFormatString="{0:dd/MM/yyyy}">
                                                </asp:BoundColumn>
                                                <asp:TemplateColumn HeaderText="Stato">
                                                    <ItemTemplate>
                                                        <asp:Image ID="imgDescrizioneStatoDisallineati" runat="server" Style="cursor: pointer;" />
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                            </Columns>
                                        </asp:DataGrid>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </igtab:Tab>
                    </Tabs>
                </igtab:UltraWebTab>
            </dyp:DynamicPanel>

            <div style="height:15px; width:100%;"></div>

        </asp:View>

    </asp:MultiView>

    </form>

    <script type="text/javascript">

        function  resizeTab(s,e){ 
            for (var i=1; i<6; i++) {
                var tab1s = document.getElementById("tab"+i+"Scroll");
                var tab1f = document.getElementById("tab"+i+"Fixed");
                
                if (tab1s != null && tab1f != null) {
                    tab1s.style.height = e.height > 60 ? e.height - tab1f.offsetHeight - 60 : 0;
                    //tab1s.style.width=e.width;
                }
            }
        }

    </script>
</body>
</html>
