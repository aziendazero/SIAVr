<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RegistroProcessi.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.RegistroProcessi" %>

<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_ocb" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitCombo" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>Registro Processi</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript" language="javascript" src="../../common/scripts/onvac.common.js"></script>
        
        <script type="text/javascript" language="javascript">
            function InizializzaToolBar(ToolBar) {
                ToolBar.PostBackButton = false;
            }

            function ToolBarClick(ToolBar, button, evnt) {
                evnt.needPostBack = true;
                switch (button.Key) {
                    case 'btnCerca':
                        // Controllo date di esecuzione
                        var strDataExecDa = OnitDataPickGet('dpkDataEsecuzioneDa');
                        var strDataExecA = OnitDataPickGet('dpkDataEsecuzioneA');
                        if (strDataExecDa != '' && strDataExecA != '') {
                            if (confrontaDate(strDataExecDa, strDataExecA) == -1) {
                                alert("Attenzione: la data di esecuzione iniziale deve essere inferiore a quella finale.");
                                evnt.needPostBack = false;
                                return;
                            }
                        }

                        break;
                }
            }

            function showReport(idProcedura, idProcesso, evt) {
                var proceduraReportHandler = null;

                var proceduraCustomReportMessage = null;
                var proceduraCustomReportHandler = null;

                if (idProcedura == "6") {
                    proceduraCustomReportMessage = "Report non disponibile !!!\n\nConsultare i RISULTATI dalla funzionalita' IMPORT DATI.";
                }
                // ex per le procedure che hanno un report personalizzato:
                //else if (idProcedura == "666") {
                //    proceduraCustomReportHandler = "666.ashx" 
                //}

                if (proceduraCustomReportMessage != null || proceduraCustomReportHandler != null) {
                    if (proceduraCustomReportMessage != null) {
                        alert(proceduraCustomReportMessage);
                    }
                    else if (proceduraCustomReportHandler != null) {
                        OpenPopUp(proceduraCustomReportHandler, 'RegistroProcessi');
                    }
                    StopPreventDefault(evt);
                }

                return false;
            }
        </script>
    </head>
    <body>
        <form id="Form1" method="post" runat="server">
            <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Height="100%" Width="100%"  Titolo="Registro Processi" >
				<div class="title" id="PanelTitolo" runat="server">
					<asp:Label id="LayoutTitolo" runat="server">&nbsp;Registro dei Processi </asp:Label>
                </div>
                <div>
					<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="90px" CssClass="infratoolbar">
                        <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                        <HoverStyle CssClass="infratoolbar_button_hover" ></HoverStyle>
                        <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                        <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
						<Items>
							<igtbar:TBarButton Key="btnCerca" Text="Cerca" DisabledImage="~/Images/cerca_dis.gif" Image="~/Images/cerca.gif">
							</igtbar:TBarButton>
							<igtbar:TBSeparator></igtbar:TBSeparator>
							<igtbar:TBarButton Key="btnAggiorna" Text="Aggiorna" DisabledImage="../../images/aggiorna_elenco_dis.gif" Image="../../images/aggiorna_elenco.gif" > 
							</igtbar:TBarButton>
						</Items>
					</igtbar:UltraWebToolbar>
                </div>
                <div>
					<table width="100%" bgcolor="whitesmoke">
						<tr>
							<td width="100%">
								<table height="100%" cellspacing="0" cellpadding="0" width="100%" border="0">
									<tr height="2">
										<td colspan="2"></td>
									</tr>
									<tr>
										<td colspan="2">
											<fieldset class="vacFieldSet" title="Ricerca Processi">
                                                <legend class="label">Ricerca Processi</legend>
												<table class="label_left" style="margin-top: 2px" cellspacing="0" cellpadding="2" width="100%" border="0">
													<colgroup>
														<col width="3%" />
														<col width="15%" />
														<col width="2%" />
														<col width="20%" />
														<col width="2%" />
														<col width="55%" />
														<col width="3%" />
													</colgroup>
													<tr>
														<td></td>
														<td class="label_left">Operatore:</td>
														<td colspan="4">
															<onitcontrols:OnitModalList id="omlOperatore" runat="server" AltriCampi="UTE_ID as Id" 
                                                                Width="70%" CampoDescrizione="UTE_DESCRIZIONE as Descrizione" CampoCodice="UTE_CODICE as Codice" 
                                                                Tabella="V_ANA_UTENTI" PosizionamentoFacile="False" LabelWidth="-1px" CodiceWidth="29%" Label="Titolo" 
                                                                UseCode="True" SetUpperCase="False" Obbligatorio="False"></onitcontrols:OnitModalList></td>
														<td></td>
													</tr>
													<tr>
														<td></td>
														<td class="label_left">Processo:</td>
														<td colspan="4">
															<on_ocb:OnitCombo id="cmbProcesso" runat="server" Width="99%" IncludeNull="True" CssClass="textbox_stringa"></on_ocb:OnitCombo></td>
														<td></td>
													</tr>
													<tr>
														<td></td>
														<td class="label_left">Data esecuzione:</td>
														<td class="label_right">Da</td>
														<td>
															<on_val:onitdatepick id="dpkDataEsecuzioneDa" runat="server" Height="20px" Width="120px" cssclass="textbox_data"
																DateBox="True"></on_val:onitdatepick></td>
														<td class="label_right">A</td>
														<td>
															<on_val:onitdatepick id="dpkDataEsecuzioneA" runat="server" Height="20px" Width="120px" DateBox="True"
																cssClass="textbox_data"></on_val:onitdatepick></td>
														<td></td>
													</tr>
												</table>
											</fieldset>
										</td>
									</tr>
									<tr height="2">
										<td colspan="2"></td>
									</tr>
								</table>
							</td>
						</tr>
					</table>
                </div>
				<div class="vac-sezione" style="margin: 2px">
					<asp:Label id="lblProcessi" runat="server">Processi Trovati</asp:Label>
                </div>

                <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
					<on_dgr:OnitGrid id="dgrProcessi" runat="server" Width="100%" CssClass="datagrid" SortedColumns="Matrice IGridColumn[]"
						PagerVoicesBefore="-1" PagerVoicesAfter="-1" SelectionOption="none"  AutoGenerateColumns="false">
						<AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
						<ItemStyle CssClass="item"></ItemStyle>
						<HeaderStyle CssClass="header"></HeaderStyle>
						<Columns>
							<asp:TemplateColumn>
								<ItemTemplate>
    								<asp:ImageButton OnClientClick='<%# string.format("showReport(""{0}"", ""{1}"", event)", Eval("IDPROC"), Eval("PROCESSO")) %>'  
    								    ImageUrl='../../images/stampa.gif' ToolTip="Visualizza report elaborazione"  style='cursor:pointer'  
    								    CommandName="Stampa" runat="server" ID="btnStampa" />
								</ItemTemplate>
								<ItemStyle HorizontalAlign="Center" />
							</asp:TemplateColumn>
							<asp:BoundColumn HeaderText="OPERATORE" DataField="OPERATORE"></asp:BoundColumn>
							<asp:BoundColumn HeaderText="PROCEDURA" DataField="PROCEDURA"></asp:BoundColumn>
							<asp:BoundColumn HeaderText="PROCESSO"  DataField="PROCESSO"></asp:BoundColumn>
							<asp:BoundColumn HeaderText="RICHIESTA"  DataField="RICHIESTA"></asp:BoundColumn>
							<asp:BoundColumn HeaderText="INIZIO"  DataField="INIZIO"></asp:BoundColumn>
							<asp:BoundColumn HeaderText="FINE"  DataField="FINE"></asp:BoundColumn>
							<asp:BoundColumn HeaderText="TOT"  DataField="TOT"></asp:BoundColumn>
							<asp:BoundColumn HeaderText="ELAB"  DataField="ELAB"></asp:BoundColumn>
							<asp:BoundColumn HeaderText="ERR"  DataField="ERR"></asp:BoundColumn>
							<asp:BoundColumn HeaderText="STATO" DataField="STATO"></asp:BoundColumn>
							<asp:BoundColumn HeaderText="IDPROC" DataField="IDPROC" Visible="false"></asp:BoundColumn>
						</Columns>					
					</on_dgr:OnitGrid>
                </dyp:DynamicPanel>

            </on_lay3:OnitLayout3>
        </form>
    </body>

</html>
