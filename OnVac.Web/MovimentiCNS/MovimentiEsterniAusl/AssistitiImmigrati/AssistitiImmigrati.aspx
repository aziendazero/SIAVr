<%@ Page Language="vb" AutoEventWireup="false" Codebehind="AssistitiImmigrati.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.AssistitiImmigrati"%>

<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>

<%@ Register TagPrefix="uc1" TagName="uscFiltriStampaEtichetteMovAusl" Src="../uscFiltriStampaEtichetteMovAusl.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
		<title>Assistiti Immigrati</title>
				
        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
		
        <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
        <script type="text/javascript" language="javascript">
        $(document).ready(function () {
                
            // Imposta l'altezza uguale nelle table dei filtri
            try 
            {
                var max = 0;
                $('[id=tblPazReg],[id=tblPazRich],[id=tblAcquisizione]').each(function () {
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

				case 'btnPulisci':
				    OnitDataPickSet('<%= odpDaNascita.ClientID %>', '')
				    OnitDataPickSet('<%= odpANascita.ClientID %>', '')
				    OnitDataPickSet('<%= dpkDaImm.ClientID %>', '')
				    OnitDataPickSet('<%= dpkAImm.ClientID %>', '')
				    document.getElementById('<%= rdbPazNoReg.ClientID %>').checked = true;
				    document.getElementById('<%= rdbCertNonRich.ClientID %>').checked = true;
				    var rdbDaAcquisire = document.getElementById('<%= rdbDaAcquisire.ClientID %>');
				    if (rdbDaAcquisire) { rdbDaAcquisire.checked = true; }
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
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" Width="100%" Height="100%" TitleCssClass="Title3" Busy="False" Titolo="Assistiti immigrati">
                <div>
					<table border="0" cellspacing="0" cellpadding="0" width="100%">
						<tr>
							<td>
								<div style="width: 100%" id="divTitolo" class="title">&nbsp;Assistiti immigrati</div>
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
							<td>
								<table border="0" cellspacing="0" cellpadding="2" width="100%" bgcolor="whitesmoke">
									<tr height="10">
										<td colspan="8"></td>
									</tr>
									<tr>
										<td class="label" width="15%"><b>Data di nascita:</b></td>
										<td class="label" width="5%">Da</td>
										<td width="20%">
											<on_val:OnitDatePick id="odpDaNascita" runat="server" DateBox="True"></on_val:OnitDatePick></td>
										<td class="label" width="5%">A</td>
										<td width="20%">
											<on_val:OnitDatePick id="odpANascita" runat="server" DateBox="True"></on_val:OnitDatePick></td>
										<td width="5%"></td>
										<td width="20%"></td>
										<td width="5%"></td>
									</tr>
									<tr>
										<td class="label"><b>Data di immigrazione:</b></td>
										<td class="label">Da</td>
										<td>
											<on_val:OnitDatePick id="dpkDaImm" runat="server" DateBox="True"></on_val:OnitDatePick></td>
										<td class="label">A</td>
										<td>
											<on_val:OnitDatePick id="dpkAImm" runat="server" DateBox="True"></on_val:OnitDatePick></td>
										<td></td>
										<td></td>
										<td></td>
									</tr>
									<tr height="10">
										<td colspan="8"></td>
									</tr>
									<tr>
										<td style="padding-top: 7px" class="label" valign="top"><b>Pazienti:</b></td>
										<td></td>
										<td>
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
										<td>
											<asp:Table ID="tblPazRich" Width="100%" BorderStyle="Solid" BackColor="#E7E7FF" BorderWidth="1px" CssClass="label_left" BorderColor="Navy" runat="server">
									            <asp:TableRow ID="TableRow1" runat="server">
									                <asp:TableCell ID="TableCell1" VerticalAlign="Top" runat="server">
									                    <asp:RadioButton ID="rdbCertRich" Text="Con certificato richiesto" GroupName="Cert" runat="server" />
									                </asp:TableCell>
									            </asp:TableRow>
                                                <asp:TableRow ID="TableRow2" VerticalAlign="Top" runat="server">
									                <asp:TableCell ID="TableCell2" runat="server">
									                    <asp:RadioButton ID="rdbCertNonRich" Text="Senza certificato richiesto" GroupName="Cert" Checked="true" runat="server" />
									                </asp:TableCell>
									            </asp:TableRow>
                                                <asp:TableRow ID="TableRow3" VerticalAlign="Top" runat="server">
									                <asp:TableCell ID="TableCell3" runat="server">
									                    <asp:RadioButton ID="rdbCertIgnora" Text="Entrambi" GroupName="Cert"  runat="server" />
									                </asp:TableCell>
									            </asp:TableRow>
									        </asp:Table>
										</td>
										<td></td>
										<td>
                                            <asp:Table ID="tblAcquisizione" Width="100%" BorderStyle="Solid" BackColor="#E7E7FF" BorderWidth="1px" CssClass="label_left" BorderColor="Navy" runat="server">
									            <asp:TableRow ID="TableRow4" runat="server">
									                <asp:TableCell ID="TableCell4" VerticalAlign="Top" runat="server">
									                    <asp:RadioButton ID="rdbAcquisiti" Text="Acquisiti" GroupName="Acq" runat="server" />
									                </asp:TableCell>
									            </asp:TableRow>
                                                <asp:TableRow ID="TableRow5" runat="server">
									                <asp:TableCell ID="TableCell5" VerticalAlign="Top" runat="server">
									                    <asp:RadioButton ID="rdbAcquisitiErrore" Text="Acquisiti con errore" GroupName="Acq" runat="server" />
									                </asp:TableCell>
									            </asp:TableRow>
                                                <asp:TableRow ID="TableRow6" runat="server">
									                <asp:TableCell ID="TableCell6" VerticalAlign="Top" runat="server">
									                    <asp:RadioButton ID="rdbDaAcquisire" Text="Da acquisire" GroupName="Acq"  Checked="true" runat="server" />
									                </asp:TableCell>
									            </asp:TableRow>
                                                <asp:TableRow ID="TableRow7" runat="server">
									                <asp:TableCell ID="TableCell7" VerticalAlign="Top" runat="server">
									                    <asp:RadioButton ID="rdbIgnoraAcquisizione" Text="Tutti" GroupName="Acq"  runat="server" />
									                </asp:TableCell>
									            </asp:TableRow>
									        </asp:Table>
										</td>
										<td></td>
									</tr>
									<tr height="10">
										<td colspan="8"></td>
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
					<table border="0" cellspacing="0" cellpadding="0" width="100%" style="margin-top: 3px">
						<tr>
							<td width="3px"></td>
							<td>
								<asp:DataGrid id="dgrPazienti" runat="server" Width="100%" AutoGenerateColumns="False"
									AllowCustomPaging="true" AllowPaging="true" PageSize="200">
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
                                        <asp:TemplateColumn>
                                            <HeaderStyle Width="5%" HorizontalAlign="Center" />
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
											HeaderText="&lt;div align=center&gt;Dati Paziente&lt;/div&gt;" CommandName="DatiPaziente">
											<HeaderStyle Width="3%" HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
										</asp:ButtonColumn>
										<asp:BoundColumn DataField="paz_cognome" HeaderText="Cognome" ReadOnly="true" >
											<HeaderStyle Width="14%" />
										</asp:BoundColumn>
										<asp:BoundColumn DataField="paz_nome" HeaderText="Nome" ReadOnly="true" >
											<HeaderStyle Width="14%" />
										</asp:BoundColumn>
										<asp:BoundColumn DataField="paz_data_nascita" HeaderText="Data di nascita" DataFormatString="{0:dd/MM/yyyy}" ReadOnly="true" >
											<HeaderStyle Width="10%" HorizontalAlign="Center" />
											<ItemStyle HorizontalAlign="Center" />
										</asp:BoundColumn>
                                        <asp:TemplateColumn HeaderText="Stato anagrafico">
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
										<asp:BoundColumn DataField="paz_data_immigrazione" HeaderText="Data immigrazione" DataFormatString="{0:dd/MM/yyyy}" ReadOnly="true" >
											<HeaderStyle Width="10%" HorizontalAlign="Center" />
											<ItemStyle HorizontalAlign="Center" />
										</asp:BoundColumn>
										<asp:BoundColumn DataField="proven_com_descrizione" HeaderText="Provenienza" ReadOnly="true" >
											<HeaderStyle Width="20%" />
										</asp:BoundColumn>
										<asp:TemplateColumn HeaderText="Acquisizione">
											<HeaderStyle Width="70px" HorizontalAlign="Center"></HeaderStyle>
											<ItemStyle HorizontalAlign="Center" />
											<ItemTemplate>
												<asp:ImageButton id="btnAcquisizione" runat="server" AlternateText="Acquisisci" ImageUrl="~/images/mov_ingresso.gif" CommandName="Acquisisci">
												</asp:ImageButton>
												<asp:Image id="imgAcquisizione" AlternateText="Acquisito con errore" ImageUrl="~/images/annulla.gif"  runat="server">
												</asp:Image>
												<asp:Label id="lblAcquisizione" runat="server" CssClass="label">Si</asp:Label>
											</ItemTemplate>
										</asp:TemplateColumn>
										<asp:TemplateColumn HeaderText="Richiesto Certificato">
											<HeaderStyle Width="100px" HorizontalAlign="Center"></HeaderStyle>
											<ItemStyle HorizontalAlign="Center" />
											<ItemTemplate>													
												<asp:ImageButton id="btnRichiestaCertificato" runat="server" CommandName="RichiestaCertificato" Visible='<%# iif(databinder.eval(Container,"DataItem")("PAZ_RICHIESTA_CERTIFICATO").toString="S",false,true) %>' ImageUrl="~/Images/incolla.gif" AlternateText="Richiesto certificato">
												</asp:ImageButton>
												<asp:Label id="lblRichCart" runat="server" CssClass="label" Visible='<%# iif(databinder.eval(Container,"DataItem")("PAZ_RICHIESTA_CERTIFICATO").toString="S",true,false) %>'>Si</asp:Label>
											</ItemTemplate>
										</asp:TemplateColumn>
										<asp:TemplateColumn HeaderText="Paziente Regolarizzato">
											<HeaderStyle Width="100px" HorizontalAlign="Center"></HeaderStyle>
											<ItemStyle HorizontalAlign="Center" />
											<ItemTemplate>
													<asp:ImageButton id="btnRegPaz" runat="server" CommandName="RegPaz" Visible='<%# iif(databinder.eval(Container,"DataItem")("paz_regolarizzato").toString="S",false,true) %>' ImageUrl="~/Images/conferma.gif" AlternateText="Regolarizza">
													</asp:ImageButton>
													<asp:Label id="lblPazReg" runat="server" CssClass="label" Visible='<%# iif(databinder.eval(Container,"DataItem")("paz_regolarizzato").toString="S",true,false) %>'>Si</asp:Label>
											</ItemTemplate>
										</asp:TemplateColumn>												
									</Columns>
								</asp:DataGrid>
							</td>
							<td width="3px"></td>
						</tr>
					</table>
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
		</form>
	</body>
</html>