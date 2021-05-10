<%@ Page Language="vb" AutoEventWireup="false" Codebehind="MovimentiInIngresso.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.MovimentiInIngresso"%>

<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>MovimentiInIngresso</title>
				
        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

		<script type="text/javascript" language="javascript"> 
		function ToolBarClick(t,button,evnt) 
		{
		    evnt.needPostBack = true;
		    switch (button.Key) 
            {
		        case 'btnPulisci':
		            OnitDataPickSet('<%=odpDaNascita.ClientID%>', '')
		            OnitDataPickSet('<%=odpANascita.ClientID%>', '')
		            OnitDataPickSet('<%=dpkDaConsultorio.ClientID%>', '')
		            OnitDataPickSet('<%=dpkAConsultorio.ClientID%>', '')
		            document.getElementById('<%=rdbAutoMovAdultiNo.ClientID%>').checked = true;
		            evnt.needPostBack = false;
		            break;
		    }
		}		
                
        function controlloRedirect() 
        {
            return controlloRedirectToPaziente(<%= Me.IsPageInEdit().ToString().ToLower() %>, true);
        }
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" Width="100%" Height="100%" TitleCssClass="Title3" Busy="False" Titolo="Movimenti in Ingresso">
				<div>
					<table border="0" cellspacing="0" cellpadding="0" width="100%">
						<tr>
							<td>
								<div style="WIDTH: 100%" id="divTitolo" class="title">&nbsp;Movimenti in ingresso</div>
							</td>
						</tr>
						<tr>
							<td>
								<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="tlbMovimenti" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		                            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
									<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
									<Items>
										<igtbar:TBarButton Key="btnCerca" Text="Cerca" 
                                            DisabledImage="~/Images/cerca_dis.gif" Image="~/Images/cerca.gif"></igtbar:TBarButton>
										<igtbar:TBarButton Key="btnStampaElenco" Text="Stampa" ToolTip="Stampa l'elenco dei movimenti"
                                            DisabledImage="~/Images/stampa_dis.gif" Image="~/Images/stampa.gif"></igtbar:TBarButton>
										<igtbar:TBSeparator></igtbar:TBSeparator>
								        <igtbar:TBarButton Key="btnPulisci" Text="Pulisci" ToolTip="Resetta i filtri impostati ai valori di default"
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
									<tr>
										<td class="label" width="35%"><b>Data di nascita:</b></td>
										<td class="label" width="5%">Da</td>
										<td width="15%">
											<on_val:OnitDatePick id="odpDaNascita" runat="server" DateBox="True"></on_val:OnitDatePick></td>
										<td class="label" width="5%">A</td>
										<td width="15%">
											<on_val:OnitDatePick id="odpANascita" runat="server" DateBox="True"></on_val:OnitDatePick></td>
										<td></td>
									</tr>
									<tr>
										<td class="label" width="35%"><b>Data di assegnazione del centro vaccinale:</b></td>
										<td class="label" width="5%">Da</td>
										<td width="15%">
											<on_val:OnitDatePick id="dpkDaConsultorio" runat="server" DateBox="True"></on_val:OnitDatePick></td>
										<td class="label" width="5%">A</td>
										<td width="15%">
											<on_val:OnitDatePick id="dpkAConsultorio" runat="server" DateBox="True"></on_val:OnitDatePick></td>
										<td></td>
									</tr>
									<tr height="10">
										<td colspan="6"></td>
									</tr>
									<tr>
										<td style="padding-top: 7px" class="label" valign="top"><b>Movimento automatico centro vaccinale adulti:</b></td>
										<td></td>
										<td colspan="3">
											<asp:Table ID="tblAutoMovAdulti"   runat="server" Width="80%" BorderWidth="1px" BackColor="#E7E7FF"
													BorderStyle="Solid" CssClass="label_left" BorderColor="Navy">
									            <asp:TableRow ID="TableRow1" runat="server">
									                <asp:TableCell ID="TableCell1" VerticalAlign="Top" runat="server" >
									                        <asp:RadioButton ID="rdbAutoMovAdulti" Text="Solo movimenti automatici" GroupName="Mov"  runat="server" />
									                </asp:TableCell>
									            </asp:TableRow>
									                <asp:TableRow ID="TableRow2" runat="server">
									                <asp:TableCell ID="TableCell2" VerticalAlign="Top" runat="server">
									                        <asp:RadioButton ID="rdbAutoMovAdultiNo" Text="Solo movimenti non automatici" Checked="true" GroupName="Mov" runat="server" />
									                </asp:TableCell>
									            </asp:TableRow>
									                <asp:TableRow ID="TableRow3" runat="server">
									                <asp:TableCell ID="TableCell3" VerticalAlign="Top" runat="server">
									                        <asp:RadioButton ID="rdbAutoMovAdultiIgnora" Text="Tutti" GroupName="Mov"  runat="server" />
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
                        <table border="0" cellspacing="0" cellpadding="0" width="100%" style="margin-top: 3px" >
						    <tr>
							    <td width="3px"></td>
							    <td>
								    <on_dgr:OnitGrid id="dgrPazienti" runat="server" Width="100%" SelectionOption="none" PagerVoicesAfter="-1"
									    PagerVoicesBefore="-1" SortedColumns="Matrice IGridColumn[]" AutoGenerateColumns="False">
									    <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
									    <ItemStyle CssClass="item"></ItemStyle>
									    <HeaderStyle CssClass="header"></HeaderStyle>
									    <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
									    <Columns>
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
											    HeaderText="Dati Paziente" CommandName="DatiPaziente">
											    <HeaderStyle HorizontalAlign="Center" Width="3%"></HeaderStyle>
                                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
										    </asp:ButtonColumn>
										    <on_dgr:OnitBoundColumn DataField="paz_cognome" ReadOnly="true" HeaderText="Cognome" key="Cognome" SortDirection="NoSort">
											    <HeaderStyle Width="15%"></HeaderStyle>
										    </on_dgr:OnitBoundColumn>
										    <on_dgr:OnitBoundColumn DataField="paz_nome" ReadOnly="true" HeaderText="Nome" key="Nome" SortDirection="NoSort">
											    <HeaderStyle Width="15%"></HeaderStyle>
										    </on_dgr:OnitBoundColumn>
										    <on_dgr:OnitBoundColumn DataField="paz_data_nascita" ReadOnly="true" HeaderText="Data di nascita" key="DataNascita" DataFormatString="{0:dd/MM/yyyy}"
											    SortDirection="NoSort">
											    <HeaderStyle HorizontalAlign="Center" Width="12%"></HeaderStyle>
											    <ItemStyle HorizontalAlign="Center"></ItemStyle>
										    </on_dgr:OnitBoundColumn>
                                            <asp:TemplateColumn HeaderText="Stato anagrafico">
											    <HeaderStyle Width="13%"></HeaderStyle>
                                                <ItemStyle Width="13%"></ItemStyle>
											    <ItemTemplate>
												    <asp:Label id="lblStatoAnagrafico" CssClass="label_left" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("stato_anagrafico") %>'>
												    </asp:Label>
											    </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:DropDownList ID="ddlStatoAnagrafico" CssClass="label_left" runat="server" Width="100%" ></asp:DropDownList>
                                                </EditItemTemplate>
										    </asp:TemplateColumn>
										    <on_dgr:OnitBoundColumn DataField="cns_descrizione" ReadOnly="true" HeaderText="Sede vaccinale precedente" key="Descrizione"
											    SortDirection="NoSort">
											    <HeaderStyle Width="13%"></HeaderStyle>
										    </on_dgr:OnitBoundColumn>
										    <on_dgr:OnitBoundColumn DataField="cnm_data" ReadOnly="true" HeaderText="Data assegnazione" key="DataAssegnazione" DataFormatString="{0:dd/MM/yyyy}"
											    SortDirection="NoSort">
											    <HeaderStyle HorizontalAlign="Center" Width="7%"></HeaderStyle>
											    <ItemStyle HorizontalAlign="Center"></ItemStyle>
										    </on_dgr:OnitBoundColumn>
										    <asp:TemplateColumn HeaderText="Cartella inviata">
											    <HeaderStyle Width="4%" HorizontalAlign="Center"></HeaderStyle>
											    <ItemStyle HorizontalAlign="Center"></ItemStyle>
											    <ItemTemplate>
												    <asp:Label id="lblCartellaInviata" runat="server" Text='<%# IIF(DataBinder.Eval(Container, "DataItem")("cnm_invio_cartella").toString="S", "Si","No") %>'>
												    </asp:Label>
											    </ItemTemplate>
										    </asp:TemplateColumn>
										    <asp:TemplateColumn HeaderText="Mov. Auto CV Adulti">
											    <HeaderStyle HorizontalAlign="Center" Width="8%"></HeaderStyle>
											    <ItemStyle HorizontalAlign="Center"></ItemStyle>
											    <ItemTemplate>
												    <asp:Label id="lblMovAutoAdulti" runat="server" Text='<%# IIF(DataBinder.Eval(Container, "DataItem")("cnm_auto_adulti").toString="S", "Si","No") %>'></asp:Label>
											    </ItemTemplate>
										    </asp:TemplateColumn>
                                            <asp:TemplateColumn HeaderText="Presa Visione">
											    <HeaderStyle HorizontalAlign="Center" Width="4%"></HeaderStyle>
											    <ItemStyle HorizontalAlign="Center"></ItemStyle>
											    <ItemTemplate>
												    <div align="center">
													    <asp:ImageButton id="btnPresaVisione" runat="server" AlternateText="Presa visione" ImageUrl="~/Images/conferma.gif" Visible='<%# iif(databinder.eval(Container,"DataItem")("cnm_invio_cartella").toString="S",true,false) %>' CommandName="PresaVisione" />
                                                    </div>
											    </ItemTemplate>
										    </asp:TemplateColumn>
										    <on_dgr:OnitBoundColumn Visible="False" DataField="paz_codice" HeaderText="H_idPaz" key="CodPaziente" SortDirection="NoSort"></on_dgr:OnitBoundColumn>
										    <on_dgr:OnitBoundColumn Visible="False" DataField="cnm_progressivo" HeaderText="H_progressivo" key="Progressivo" SortDirection="NoSort"></on_dgr:OnitBoundColumn>
										    <on_dgr:OnitBoundColumn Visible="False" DataField="paz_data_aggiornamento" HeaderText="H_dataAgg" key="DataAggiornamento" SortDirection="NoSort"></on_dgr:OnitBoundColumn>
										    <on_dgr:OnitBoundColumn Visible="False" DataField="cnm_invio_cartella" HeaderText="H_invioCartella" key="InvioCartella" SortDirection="NoSort"></on_dgr:OnitBoundColumn>
										    <on_dgr:OnitBoundColumn Visible="False" DataField="cnm_cns_codice_old" HeaderText="H_codConsOld" key="CodConsOld" SortDirection="NoSort"></on_dgr:OnitBoundColumn>
										    <on_dgr:OnitBoundColumn Visible="False" DataField="paz_indirizzo_residenza" HeaderText="H_paz_ind_res" key="Indirizzo" SortDirection="NoSort"></on_dgr:OnitBoundColumn>
										    <on_dgr:OnitBoundColumn Visible="False" DataField="cnm_presa_visione" HeaderText="H_presaVisione" key="PresaVisione" SortDirection="NoSort"></on_dgr:OnitBoundColumn>
                                            <asp:TemplateColumn Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCodiceStatoAnagrafico" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("paz_stato_anagrafico") %>' ></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:Label ID="lblCodiceStatoAnagraficoEdit" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("paz_stato_anagrafico") %>' ></asp:Label>
                                                </EditItemTemplate>
                                            </asp:TemplateColumn>
									    </Columns>
								    </on_dgr:OnitGrid>
                                </td>
							    <td width="3px"></td>
						    </tr>
					    </table>
                    </div>
                </dyp:DynamicPanel>
			</on_lay3:onitlayout3>
		</form>
	</body>
</html>
