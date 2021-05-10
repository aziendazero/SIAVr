<%@ Page Language="vb" AutoEventWireup="false" Codebehind="PazientiInIngresso.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.PazientiInIngresso"%>

<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>

<%@ Register TagPrefix="uc1" TagName="uscFiltriStampaEtichetteMovAusl" Src="../uscFiltriStampaEtichetteMovAusl.ascx" %>
<%@ Register TagPrefix="uc2" TagName="StatiAnagrafici" Src="../../../Common/Controls/StatiAnagrafici.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
		<title>Pazienti In Ingresso</title>
		
        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

        <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
        <script type="text/javascript" language="javascript">

        $(document).ready(function () {
                
            // Imposta l'altezza uguale nelle table dei filtri
            try 
            {
                var max = 0;
                $('[id=tblPazReg],[id=tblAcquisizione]').each(function () {
                    max = Math.max($(this).height(), max);
                }).height(max);
            } 
            catch (e) 
            {
                // 
            }
        });

		function ToolBarClick(t, button, evnt) 
        {
            evnt.needPostBack = true;

			switch (button.Key) 
			{
				case 'btnCerca':
				    if (!document.getElementById('<%=rdbPazNoReg.ClientID%>').checked) 
					{
						if (!confirm('ATTENZIONE: la ricerca dei pazienti regolarizzati potrebbe impiegare molto tempo. Continuare?')) 
						{
							evnt.needPostBack = false;
						}
					}
					break;
				
				case 'btnStampaEtichette':
					showFm('fmFiltriEtichette',true,250);
				    evnt.needPostBack = false;
				    break;
			}
		}
		
		function clickStampa()
		{
			closeFm('fmFiltriEtichette');
			document.getElementById('btnStampaEtichette').click();
        }
                
        function controlloRedirect() 
        {
            return controlloRedirectToPaziente(<%= Me.IsPageInEdit().ToString().ToLower() %>, true);
        }
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" Width="100%" Height="100%" TitleCssClass="Title3" Busy="False" Titolo="Pazienti in ingresso">
				
                <div>					
					<table border="0" cellspacing="0" cellpadding="0" width="100%">
						<tr>
							<td>
								<div style="width: 100%" id="divTitolo" class="title">&nbsp;Pazienti in ingresso</div>
							</td>
						</tr>
						<tr>
							<td>
								<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="tlbMovimenti" runat="server" ItemWidthDefault="70px" CssClass="infratoolbar">
                                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		                            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
									<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
									<Items>
										<igtbar:TBarButton Tag="" Key="btnCerca" HoverImage="" ToolTip="Effettua la ricerca degli assistiti"
											SelectedImage="" Text="Cerca" TargetURL="" DisabledImage="~/Images/cerca_dis.gif" TargetFrame=""
											Image="~/Images/cerca.gif">
										</igtbar:TBarButton>
										<igtbar:TBSeparator Tag="" Key="" Image=""></igtbar:TBSeparator>
										<igtbar:TBarButton Tag="" Key="btnStampaElenco" HoverImage="" ToolTip="Stampa l'elenco dei movimenti"
											SelectedImage="" Text="Stampa elenco" TargetURL="" DisabledImage="~/Images/stampa_dis.gif"
											TargetFrame="" Image="~/Images/stampa.gif">
											<DefaultStyle Width="110px" CssClass="infratoolbar_button_default"></DefaultStyle>
										</igtbar:TBarButton>
										<igtbar:TBarButton Tag="" Key="btnStampaElencoPerComune" HoverImage="" ToolTip="Stampa l'elenco dei movimenti per comune"
											SelectedImage="" Text="Stampa elenco per comune" TargetURL="" DisabledImage="~/Images/stampa_dis.gif"
											TargetFrame="" Image="~/Images/stampa.gif">
											<DefaultStyle Width="180px" CssClass="infratoolbar_button_default"></DefaultStyle>
										</igtbar:TBarButton>
										<igtbar:TBarButton Tag="" Key="btnStampaEtichette" HoverImage="" ToolTip="Stampa le etichette" SelectedImage=""
											Text="Stampa etichette" TargetURL="" DisabledImage="~/Images/stampa_dis.gif" TargetFrame=""
											Image="~/Images/stampa.gif">
											<DefaultStyle Width="120px" CssClass="infratoolbar_button_default"></DefaultStyle>
										</igtbar:TBarButton>
										<igtbar:TBSeparator></igtbar:TBSeparator>
								        <igtbar:TBarButton Key="btnPulisci" ToolTip="Resetta i filtri impostati ai valori di default" Text="Pulisci"
									        DisabledImage="~/Images/pulisci_dis.gif" Image="~/Images/pulisci.gif">
								        </igtbar:TBarButton>
									</Items>
								</igtbar:UltraWebToolbar>
                            </td>
						</tr>
						<tr>
							<td>
								<div class="Sezione">&nbsp;FILTRI DI RICERCA</div>
							</td>
						</tr>
						<tr>
							<td style="background-color: whitesmoke">
								<table border="0" cellspacing="0" cellpadding="2" width="100%" style="margin-bottom:5px;">
                                    <colgroup>
                                        <col width="12%" />
                                        <col width="3%" />
                                        <col width="14%" />
                                        <col width="3%" />
                                        <col width="15%" />
                                        <col width="15%"  />
                                        <col width="3%" />
                                        <col width="15%" />
                                        <col width="3%"  />
                                        <col width="14%" />
                                        <col width="2%" />
                                    </colgroup>
									<tr style="padding-top: 7px">
										<td title="Filtro intervallo data di nascita"  class="label" style="font-weight:bold">Nascita</td>
										<td title="Filtro intervallo data di nascita"  class="label">Da</td>
										<td title="Filtro intervallo data di nascita" >
											<on_val:OnitDatePick id="odpDaNascita" runat="server" DateBox="True" ToolTip="Filtro intervallo data di nascita"></on_val:OnitDatePick></td>
										<td title="Filtro intervallo data di nascita" class="label">A</td>
										<td title="Filtro intervallo data di nascita" >
											<on_val:OnitDatePick id="odpANascita" runat="server" DateBox="True" ToolTip="Filtro intervallo data di nascita"></on_val:OnitDatePick></td>
												
                                        <td title="Filtro intervallo data di immigrazione" class="label" style="font-weight:bold">Immigrazione</td>
										<td title="Filtro intervallo data di immigrazione" class="label">Da</td>
										<td title="Filtro intervallo data di immigrazione">
											<on_val:OnitDatePick id="dpkDaImm" runat="server" DateBox="True" ToolTip="Filtro intervallo data di immigrazione"></on_val:OnitDatePick></td>
										<td title="Filtro intervallo data di immigrazione" class="label">A</td>
										<td title="Filtro intervallo data di immigrazione">
											<on_val:OnitDatePick id="dpkAImm" runat="server" DateBox="True" ToolTip="Filtro intervallo data di immigrazione"></on_val:OnitDatePick></td>												
										<td></td>
                                    </tr>
									<tr>
                                        <td title="Filtro intervallo data di inizio residenza" class="label" style="font-weight:bold">Inizio Residenza</td>
                                        <td title="Filtro intervallo data di inizio residenza" class="label">Da</td>
                                        <td title="Filtro intervallo data di inizio residenza">
                                            <on_val:OnitDatePick id="dpkDaResidenza" runat="server" DateBox="True" ToolTip="Filtro intervallo data di inizio residenza"></on_val:OnitDatePick></td>
                                        <td title="Filtro intervallo data di inizio residenza" class="label">A</td>
                                        <td title="Filtro intervallo data di inizio residenza">
                                            <on_val:OnitDatePick id="dpkAResidenza" runat="server" DateBox="True" ToolTip="Filtro intervallo data di inizio residenza"></on_val:OnitDatePick></td>
                                        <td title="Filtro intervallo data di inizio domicilio" class="label" style="font-weight:bold">Inizio Domicilio</td>
                                        <td title="Filtro intervallo data di inizio domicilio" class="label">Da</td>
                                        <td title="Filtro intervallo data di inizio domicilio">
                                            <on_val:OnitDatePick id="dpkDaDomicilio" runat="server" DateBox="True" ToolTip="Filtro intervallo data di inizio domicilio"></on_val:OnitDatePick></td>
                                        <td title="Filtro intervallo data di inizio domicilio" class="label">A</td>
                                        <td title="Filtro intervallo data di inizio domicilio">
                                            <on_val:OnitDatePick id="dpkADomicilio" runat="server" DateBox="True" ToolTip="Filtro intervallo data di inizio domicilio"></on_val:OnitDatePick></td>
                                        <td></td>
									</tr>
									<tr>
                                        <td title="Filtro intervallo data di assistenza" class="label" style="font-weight:bold">Assistenza</td>
                                        <td title="Filtro intervallo data di assistenza" class="label">Da</td>
                                        <td title="Filtro intervallo data di assistenza">
                                            <on_val:OnitDatePick id="dpkDaAssistenza" runat="server" DateBox="True" ToolTip="Filtro intervallo data di assistenza"></on_val:OnitDatePick></td>
                                        <td title="Filtro intervallo data di assistenza" class="label">A</td>
                                        <td title="Filtro intervallo data di assistenza">
                                            <on_val:OnitDatePick id="dpkAAssistenza" runat="server" DateBox="True" ToolTip="Filtro intervallo data di assistenza"></on_val:OnitDatePick></td>
                                        <td title="Filtro stato anagrafico" class="label" style="font-weight:bold">Stato Anagrafico</td>
                                        <td title="Filtro stato anagrafico"></td>
                                        <td colspan="3" title="Filtro stato anagrafico">
                                            <uc2:StatiAnagrafici id="uscStatiAnagrafici" runat="server" ShowLabel="false"></uc2:StatiAnagrafici></td>
                                        <td></td>
									</tr>
									<tr style="padding-top: 7px">
										<td valign="top" class="label" style="font-weight:bold"><b>Pazienti</b></td>
										<td class="label"></td>
										<td colspan="3">
											<asp:Table ID="tblPazReg" Width="100%" BorderStyle="Solid" BackColor="#E7E7FF" BorderWidth="1px" CssClass="label_left" BorderColor="Navy" runat="server">
									            <asp:TableRow ID="TableRow8" runat="server">
									                <asp:TableCell ID="TableCell8" VerticalAlign="Top" runat="server">
									                    <asp:RadioButton ID="rdbPazReg" Text="Regolarizzati" GroupName="Reg" runat="server" />
									                </asp:TableCell>
									            </asp:TableRow>
                                                <asp:TableRow ID="TableRow9" runat="server">
									                <asp:TableCell ID="TableCell9" VerticalAlign="Top" runat="server">
									                    <asp:RadioButton ID="rdbPazNoReg" Text="Non Regolarizzati" GroupName="Reg" Checked="true" runat="server" />
									                </asp:TableCell>
									            </asp:TableRow>
                                                <asp:TableRow ID="TableRow10" runat="server">
									                <asp:TableCell ID="TableCell10" VerticalAlign="Top" runat="server">
									                    <asp:RadioButton ID="rdbIgnoraPazReg" Text="Entrambi" GroupName="Reg" runat="server" />
									                </asp:TableCell>
									            </asp:TableRow>
									        </asp:Table>
										</td>
										<td></td>
                                        <td></td>
										<td colspan="3"  class="label">
                                            <asp:Table ID="tblAcquisizione" Width="100%" BorderStyle="Solid" BackColor="#E7E7FF" BorderWidth="1px" CssClass="label_left" BorderColor="Navy" runat="server">
                                                <asp:TableRow ID="TableRow1" runat="server">
									                <asp:TableCell ID="TableCell1" VerticalAlign="Top" runat="server">
									                    <asp:RadioButton ID="rdbAcquisizione_NonEffettuata" Text="Acquisizione Non Effettuata" GroupName="Acq" runat="server" Checked="true" />
									                </asp:TableCell>
									            </asp:TableRow>
                                                <asp:TableRow ID="TableRow2" runat="server">
									                <asp:TableCell ID="TableCell2" VerticalAlign="Top" runat="server">
									                    <asp:RadioButton ID="rdbAcquisizione_NessunDatoDaAcquisire" Text="Nessun Dato Da Acquisire" GroupName="Acq" runat="server" />
									                </asp:TableCell>
									            </asp:TableRow>
                                                <asp:TableRow ID="TableRow3" runat="server">
									                <asp:TableCell ID="TableCell3" VerticalAlign="Top" runat="server">
									                    <asp:RadioButton ID="rdbAcquisizione_Parziale" Text="Acquisizione Parziale" GroupName="Acq" runat="server" />
									                </asp:TableCell>
									            </asp:TableRow>
                                                <asp:TableRow ID="TableRow4" runat="server">
									                <asp:TableCell ID="TableCell4" VerticalAlign="Top" runat="server">
                                                        <asp:RadioButton ID="rdbAcquisizione_Totale" Text="Acquisizione Totale" GroupName="Acq" runat="server" />
									                </asp:TableCell>
									            </asp:TableRow>
                                                <asp:TableRow ID="TableRow5" runat="server">
									                <asp:TableCell ID="TableCell5" VerticalAlign="Top" runat="server">
                                                        <asp:RadioButton ID="rdbAcquisizione_Tutti" Text="Tutti" GroupName="Acq" runat="server" />
									                </asp:TableCell>
									            </asp:TableRow>
									        </asp:Table>
                                        </td>
                                        <td></td>
									</tr>
								</table>
							</td>
						</tr>
						<tr>
							<td>
								<div id="divSezioneMovimenti" class="Sezione" runat="server">&nbsp;MOVIMENTI</div>
							</td>
						</tr>
					</table>
                </div>

                <dyp:DynamicPanel ID="dypScroll1" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                    <div>
					    <table border="0" cellspacing="0" cellpadding="0" width="100%" style="margin-top: 3px">
						    <tr>
							    <td width="3px"></td>
							    <td>
								    <asp:DataGrid id="dgrPazienti" runat="server" Width="100%" AutoGenerateColumns="False"
									    AllowCustomPaging="true" AllowPaging="true" PageSize="200" AllowSorting="True">
									    <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
									    <ItemStyle CssClass="item"></ItemStyle>
									    <PagerStyle  CssClass="pager" Position="TopAndBottom" Mode="NumericPages" />
									    <HeaderStyle CssClass="header"></HeaderStyle>
									    <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
									    <Columns>
										    <asp:BoundColumn Visible="False" DataField="paz_codice" HeaderText="H_idPaz"></asp:BoundColumn>
                                            <asp:TemplateColumn Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCodiceStatoAnagrafico" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("paz_stato_anagrafico") %>' ></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:Label ID="lblCodiceStatoAnagraficoEdit" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("paz_stato_anagrafico") %>' ></asp:Label>
                                                </EditItemTemplate>
                                            </asp:TemplateColumn>
                                            <asp:TemplateColumn Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblStatoAcquisizione" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("paz_stato_acquisizione") %>' ></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:Label ID="lblStatoAcquisizioneEdit" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("paz_stato_acquisizione") %>' ></asp:Label>
                                                </EditItemTemplate>
                                            </asp:TemplateColumn>
                                            <asp:TemplateColumn>
                                                <HeaderStyle Width="3%" HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                <ItemTemplate>
                                                    <asp:ImageButton ID="btnEditGrid" runat="server" CommandName="EditRowMovimenti" ImageUrl="~/Images/modifica.gif" ToolTip="Modifica" />
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:ImageButton ID="btnConfermaGrid" runat="server" CommandName="UpdateRowMovimenti" ImageUrl="~/Images/conferma.gif" ToolTip="Conferma le modifiche" />
                                                    <asp:ImageButton ID="btnAnnullaGrid" runat="server" CommandName="CancelRowMovimenti" ImageUrl="~/Images/annullaConf.gif" ToolTip="Annulla le modifiche" />
                                                </EditItemTemplate>
                                            </asp:TemplateColumn>
										    <asp:ButtonColumn Text="&lt;div align=center&gt;&lt;img style='cursor:hand' onclick=&quot;if(!controlloRedirect()){StopPreventDefault(event);}&quot; src='../../../images/utente.gif' alt='Visualizza dati paziente'/&gt;&lt;/div&gt;"
											    HeaderText="" CommandName="DatiPaziente">
											    <HeaderStyle Width="3%" HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" />
										    </asp:ButtonColumn>
										    <asp:BoundColumn DataField="paz_cognome" HeaderText="Cognome &lt;img id=&quot;imgCognome&quot; alt=&quot;&quot; src=&quot;../../../images/transparent16.gif&quot; /&gt;" 
                                                ReadOnly="true" SortExpression="paz_cognome" >
											    <HeaderStyle Width="10%" />
										    </asp:BoundColumn>
										    <asp:BoundColumn DataField="paz_nome" HeaderText="Nome &lt;img id=&quot;imgNome&quot; alt=&quot;&quot; src=&quot;../../../images/transparent16.gif&quot; /&gt;" 
                                                ReadOnly="true" SortExpression="paz_nome" >
											    <HeaderStyle Width="9%" />
										    </asp:BoundColumn>
										    <asp:BoundColumn DataField="paz_data_nascita" HeaderText="Data di Nascita &lt;img id=&quot;imgNascita&quot; alt=&quot;&quot; src=&quot;../../../images/transparent16.gif&quot; /&gt;"
                                                DataFormatString="{0:dd/MM/yyyy}" ReadOnly="true" SortExpression="paz_data_nascita" >
											    <HeaderStyle Width="9%" HorizontalAlign="Center" />
											    <ItemStyle HorizontalAlign="Center" />
										    </asp:BoundColumn>
                                            <asp:TemplateColumn HeaderText="Data e Comune<br/>Residenza &lt;img id=&quot;imgRes&quot; alt=&quot;&quot; src=&quot;../../../images/transparent16.gif&quot; /&gt;"
                                                SortExpression="res_com_descrizione, paz_data_inizio_residenza">
                                                <HeaderStyle Width="9%" HorizontalAlign="Left" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                <ItemTemplate>
                                                    <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblDataResidenza" runat="server" Text='<%# String.Format("{0:dd/MM/yyyy}", DataBinder.Eval(Container, "DataItem")("paz_data_inizio_residenza")) %>' ></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblComuneResidenza" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("res_com_descrizione") %>' ></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
                                            <asp:TemplateColumn HeaderText="Data e Comune<br/>Domicilio &lt;img id=&quot;imgDom&quot; alt=&quot;&quot; src=&quot;../../../images/transparent16.gif&quot; /&gt;"
                                                SortExpression="dom_com_descrizione, paz_data_inizio_domicilio">
											    <HeaderStyle Width="9%" HorizontalAlign="Left" />
											    <ItemStyle HorizontalAlign="Left" />  
                                                <ItemTemplate>
                                                    <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblDataDomicilio" runat="server" Text='<%# String.Format("{0:dd/MM/yyyy}", DataBinder.Eval(Container, "DataItem")("paz_data_inizio_domicilio")) %>' ></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblComuneDomicilio" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("dom_com_descrizione") %>' ></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>                                                        
                                                </ItemTemplate>                                              
                                            </asp:TemplateColumn>
                                            <asp:TemplateColumn HeaderText="Data e Comune<br/>Immigrazione &lt;img id=&quot;imgImm&quot; alt=&quot;&quot; src=&quot;../../../images/transparent16.gif&quot; /&gt;"
                                                SortExpression="proven_com_descrizione, paz_data_immigrazione">
											    <HeaderStyle Width="9%" HorizontalAlign="Left" />
											    <ItemStyle HorizontalAlign="Left" />
                                                <ItemTemplate>
                                                    <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblDataImmigrazione" runat="server" Text='<%# String.Format("{0:dd/MM/yyyy}", DataBinder.Eval(Container, "DataItem")("paz_data_immigrazione")) %>' ></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblComuneImmigrazione" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("proven_com_descrizione") %>' ></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
                                            <asp:TemplateColumn HeaderText=" &lt;img id=&quot;imgAss&quot; alt=&quot;&quot; src=&quot;../../../images/transparent16.gif&quot; /&gt;"
                                                SortExpression="usl_assistenza_descrizione, paz_data_inizio_ass">
											    <HeaderStyle Width="10%" HorizontalAlign="Left" />
											    <ItemStyle HorizontalAlign="Left" />
                                                <ItemTemplate>                                                    
                                                    <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblDataAssistenza" runat="server" Text='<%# String.Format("{0:dd/MM/yyyy}", DataBinder.Eval(Container, "DataItem")("paz_data_inizio_ass")) %>' ></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblUslAssistenza" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("usl_assistenza_descrizione") %>' ></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
                                            <asp:BoundColumn DataField="usl_provenienza_descrizione" HeaderText=" &lt;img id=&quot;imgProv&quot; alt=&quot;&quot; src=&quot;../../../images/transparent16.gif&quot; /&gt;" 
                                                ReadOnly="true" SortExpression="usl_provenienza_descrizione">
											    <HeaderStyle Width="10%" HorizontalAlign="Left" />
											    <ItemStyle HorizontalAlign="Left" />
										    </asp:BoundColumn>
                                            <asp:TemplateColumn HeaderText="Stato anagrafico &lt;img id=&quot;imgAnag&quot; alt=&quot;&quot; src=&quot;../../../images/transparent16.gif&quot; /&gt;" 
                                                SortExpression="stato_anagrafico">
											    <HeaderStyle Width="10%"></HeaderStyle>
                                                <ItemStyle Width="10%"></ItemStyle>
											    <ItemTemplate>
												    <asp:Label id="lblStatoAnagrafico" CssClass="label_left" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("stato_anagrafico") %>'>
												    </asp:Label>
											    </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:DropDownList ID="ddlStatoAnagrafico" CssClass="label_left" runat="server" Width="100%" ></asp:DropDownList>
                                                </EditItemTemplate>
										    </asp:TemplateColumn>
										    <asp:TemplateColumn>
											    <HeaderStyle Width="3%" HorizontalAlign="Center"></HeaderStyle>
											    <ItemStyle HorizontalAlign="Center" />
                                                <HeaderTemplate>
                                                    <asp:Label ID="lblHeaderRegolarizzazione" runat="server" ToolTip="Paziente Regolarizzato">Reg.</asp:Label>
                                                </HeaderTemplate>
											    <ItemTemplate>
													    <asp:ImageButton id="btnRegPaz" runat="server" CommandName="RegPaz" Visible='<%# iif(databinder.eval(Container,"DataItem")("paz_regolarizzato").toString="S",false,true) %>' ImageUrl="~/Images/conferma.gif" AlternateText="Regolarizza">
													    </asp:ImageButton>
													    <asp:Label id="lblPazReg" runat="server" CssClass="label" Visible='<%# iif(databinder.eval(Container,"DataItem")("paz_regolarizzato").toString="S",true,false) %>'>Si</asp:Label>
											    </ItemTemplate>
										    </asp:TemplateColumn>
										    <asp:TemplateColumn SortExpression="paz_stato_acquisizione">
											    <HeaderStyle Width="3%" HorizontalAlign="Center"></HeaderStyle>
											    <ItemStyle HorizontalAlign="Center" />
                                                <HeaderTemplate>
                                                    <asp:Label runat="server" ID="lblHeaderAcquisizione" ToolTip="Acquisizione">Acq.</asp:Label>
                                                </HeaderTemplate>
											    <ItemTemplate>
												    <asp:ImageButton id="btnAcquisizione" runat="server" AlternateText="Acquisisci" ImageUrl="~/images/mov_ingresso.gif" CommandName="Acquisisci">
												    </asp:ImageButton>
												    <asp:Label id="lblAcquisizione" runat="server" CssClass="label">Si</asp:Label>
											    </ItemTemplate>
										    </asp:TemplateColumn>
										    <%--<asp:TemplateColumn SortExpression="paz_note_acquisizione">
											    <HeaderStyle Width="3%" HorizontalAlign="Center"></HeaderStyle>
											    <ItemStyle HorizontalAlign="Center" />
                                                <HeaderTemplate>
                                                    <asp:Label runat="server" ID="lblHeaderNoteAcquisizione" ToolTip="Note Acquisizione">Note<br />Acq.</asp:Label>
                                                </HeaderTemplate>
											    <ItemTemplate>
												    <asp:ImageButton id="btnNoteAcquisizione" runat="server"  Visible='<%# IIf(databinder.eval(Container,"DataItem")("paz_note_acquisizione").ToString()="",false,true) %>' AlternateText="Visualizza le note di acquisizione" ImageUrl="~/images/note.png" CommandName="NoteAcquisizione">
												    </asp:ImageButton>
											    </ItemTemplate>
										    </asp:TemplateColumn>--%>
									    </Columns>
								    </asp:DataGrid>
							    </td>
							    <td width="3px"></td>
						    </tr>
					    </table>
                    </div>
                </dyp:DynamicPanel>
			</on_lay3:onitlayout3>
			
			<onitcontrols:OnitFinestraModale id="fmFiltriEtichette" title="<div style=&quot;font-family:'Microsoft Sans Serif'; font-size: 11pt;padding-bottom:2px&quot;>&nbsp;Stampa Etichette</div>"
				runat="server" width="420px" BackColor="LightGray" height="185px" NoRenderX="True" RenderModalNotVisible="True">
				<table border="0" cellspacing="0" cellpadding="0" width="100%">
					<tr>
						<td></td>
						<td colspan="3">
							<uc1:uscFiltriStampaEtichetteMovAusl id="UscFiltriEtichette" runat="server" TipoMovimento="I"></uc1:uscFiltriStampaEtichetteMovAusl></td>
						<td></td>
					</tr>
					<tr>
						<td width="1%"></td>
						<td width="45%" align="right">
                            <input type="button" style="cursor: hand" id="inputStampaEtichette" onclick="clickStampa();" value="Stampa">
							<asp:Button style="display: none" id="btnStampaEtichette" runat="server" Text="Stampa"></asp:Button>
                        </td>
						<td width="8%"></td>
						<td width="45%" align="left">
                            <input type="button" style="cursor: hand" id="btnAnnullaEtichette" onclick="closeFm('fmFiltriEtichette')" value="Annulla">
                        </td>
						<td width="1%"></td>
					</tr>
				</table>
			</onitcontrols:OnitFinestraModale>

<%--            <onitcontrols:OnitFinestraModale id="fmNoteAcquisizione" title="<div style=&quot;font-family:'Microsoft Sans Serif'; font-size: 11pt;padding-bottom:2px&quot;>&nbsp;Note Acquisizione</div>"
				runat="server" width="420px" BackColor="LightGray" height="185px" NoRenderX="False" RenderModalNotVisible="True">
                <table width="100%" cellpadding="0" cellspacing="0" style="border:1px solid navy; height:100%; margin-top:3px;" >
                    <colgroup>
                        <col width="2%" />
                        <col width="96%" />
                        <col width="2%" />
                    </colgroup>
                    <tr style="padding-top:5px; padding-bottom:3px;">
                        <td></td>
                        <td valign="top">
                            <asp:Label ID="lblNoteAcquisizione" runat="server" CssClass="label_left" style="font-family:Verdana; font-size:12px; overflow:auto;" ></asp:Label>
                        </td>
                        <td></td>
                    </tr>
                </table>
            </onitcontrols:OnitFinestraModale>--%>

		</form>
	</body>
</html>
