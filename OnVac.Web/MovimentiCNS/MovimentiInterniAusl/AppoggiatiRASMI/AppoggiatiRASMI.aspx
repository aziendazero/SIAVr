<%@ Page Language="vb" AutoEventWireup="false" Codebehind="AppoggiatiRASMI.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.AppoggiatiRASMI"%>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>Appoggiati C.V. SMI</title>
		
        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

        <script type="text/javascript" language="javascript"> 
		function ToolBarClick(t,button,evnt) 
		{
		    evnt.needPostBack = true;

			switch (button.Key) 
			{
				case 'btnSalva':
                    if (!checkModale())
                    {
                        evnt.needPostBack = false;
                    }
					else 
					{
						if (!confirm('Salvare le assegnazioni effettuate?')) 
						{
							evnt.needPostBack = false;
						}
					}
					break;
				
                case 'btnAnnulla':
					if (!confirm('Annullare le assegnazioni e ricaricare i dati?')) 
					{
						evnt.needPostBack = false;
					}
		            break;

	            case 'btnPulisci':
	                OnitDataPickSet('<%= odpDaNascita.ClientID %>', '')
	                OnitDataPickSet('<%= odpANascita.ClientID %>', '')
	                OnitDataPickSet('<%= dpkDaConsultorio.ClientID %>', '')
	                OnitDataPickSet('<%= dpkAConsultorio.ClientID %>', '')
	                evnt.needPostBack = false;
	                break;	
			}
		}

		function checkModale() 
		{
			var id_modale = '<%= txtConsultorioVacc.ClientId() %>';
			var validFm = isValidFinestraModale(id_modale,false);
			if (!validFm) 
			{
				var oFm = document.getElementById(id_modale);
				if (oFm!=null) oFm.blur();
				return false;
			} 
		
			return true;
		}
                
        function controlloRedirect() 
        {
            return controlloRedirectToPaziente(<%= Me.IsPageInEdit().ToString().ToLower() %>, false);
        }
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" Width="100%" Height="100%" TitleCssClass="Title3" Busy="False" Titolo="Movimenti Centro Smistamento">
				<div>
					<table border="0" cellspacing="0" cellpadding="0" width="100%">
						<tr>
							<td>
								<div style="width: 100%" id="divTitolo" class="title">&nbsp;Assistiti centro vaccinale smistamento</div>
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
										<igtbar:TBarButton Key="btnCerca" ToolTip="Effettua la ricerca degli assistiti" Text="Cerca" DisabledImage="~/Images/cerca_dis.gif"
											Image="~/Images/cerca.gif">
										</igtbar:TBarButton>
										<igtbar:TBarButton Key="btnSalva" ToolTip="Salva gli smistamenti effettuati" Text="Salva" DisabledImage="~/Images/salva_dis.gif"
											Image="~/Images/salva.gif">
										</igtbar:TBarButton>
										<igtbar:TBarButton Key="btnAnnulla" ToolTip="Annulla le modifiche effettuate" Text="Annulla" DisabledImage="~/Images/annulla_dis.gif"
											Image="~/Images/annulla.gif">
										</igtbar:TBarButton>
										<igtbar:TBSeparator></igtbar:TBSeparator>
										<igtbar:TBarButton Key="btnStampaElenco" ToolTip="Stampa l'elenco dei movimenti" Text="Stampa" DisabledImage="~/Images/stampa_dis.gif"
											Image="~/Images/stampa.gif">
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
										<td class="label" width="35%"><b>Data di inserimento in anagrafe:</b></td>
										<td class="label" width="5%">Da</td>
										<td width="15%">
											<on_val:OnitDatePick id="dpkDaConsultorio" runat="server" DateBox="True"></on_val:OnitDatePick></td>
										<td class="label" width="5%">A</td>
										<td width="15%">
											<on_val:OnitDatePick id="dpkAConsultorio" runat="server" DateBox="True"></on_val:OnitDatePick></td>
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
								    <asp:DataGrid id="DatagridPazienti" runat="server" Width="100%" AutoGenerateColumns="False"
									    AllowCustomPaging="true" AllowPaging="true" PageSize="200">
									    <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
									    <ItemStyle CssClass="item"></ItemStyle>
									    <PagerStyle  CssClass="pager" Position="TopAndBottom" Mode="NumericPages" />
									    <HeaderStyle CssClass="header"></HeaderStyle>
									    <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
									    <Columns>
											<asp:ButtonColumn Text="&lt;img src='../../../images/seleziona.gif' title='seleziona' onclick='if (!checkModale()) StopPreventDefault(event);'/&gt;"
												CommandName="Select">
                                                <ItemStyle Width="4%" HorizontalAlign="Center" />
                                            </asp:ButtonColumn>
										    <asp:BoundColumn DataField="paz_cognome" HeaderText="Cognome" ReadOnly="true" >
											    <HeaderStyle Width="14%" />
										    </asp:BoundColumn>
										    <asp:BoundColumn DataField="paz_nome" HeaderText="Nome" ReadOnly="true" >
											    <HeaderStyle Width="14%" />
										    </asp:BoundColumn>
										    <asp:BoundColumn DataField="paz_data_nascita" HeaderText="Data di nascita" DataFormatString="{0:dd/MM/yyyy}" ReadOnly="true" >
											    <HeaderStyle Width="11%" HorizontalAlign="Center" />
											    <ItemStyle HorizontalAlign="Center" />
										    </asp:BoundColumn>
										    <asp:BoundColumn DataField="stato_anagrafico" HeaderText="Stato anagrafico" ReadOnly="true" >
											    <HeaderStyle Width="12%" />
										    </asp:BoundColumn>
										    <asp:BoundColumn DataField="cns_descrizione" HeaderText="Nuova sede vaccinale" ReadOnly="true" >
											    <HeaderStyle Width="10%" />
										    </asp:BoundColumn>
										    <asp:BoundColumn DataField="paz_cns_data_assegnazione" HeaderText="Data assegnazione" DataFormatString="{0:dd/MM/yyyy}" ReadOnly="true" >
											    <HeaderStyle Width="7%" HorizontalAlign="Center" />
											    <ItemStyle HorizontalAlign="Center" />
										    </asp:BoundColumn>
										    <asp:BoundColumn DataField="paz_indirizzo_domicilio" HeaderText="Indirizzo domicilio" ReadOnly="true" >
											    <HeaderStyle Width="15%" />
										    </asp:BoundColumn>
										    <asp:BoundColumn DataField="com_descrizione" HeaderText="Comune domicilio" ReadOnly="true" >
											    <HeaderStyle Width="9%" />
										    </asp:BoundColumn>
											<asp:ButtonColumn Text="&lt;div align=center&gt;&lt;img style='cursor:hand' onclick=&quot;if(!controlloRedirect()){StopPreventDefault(event);}&quot; src='../../../images/utente.gif' alt='Visualizza dati paziente'/&gt;&lt;/div&gt;"
												HeaderText="&lt;div align=center&gt;Dati Paziente&lt;/div&gt;" CommandName="DatiPaziente">
												<HeaderStyle Width="4%" HorizontalAlign="Center"></HeaderStyle>
												<ItemStyle HorizontalAlign="Center"></ItemStyle>
											</asp:ButtonColumn>
										    <asp:BoundColumn DataField="paz_cns_codice" HeaderText="H_codSedeVac" ReadOnly="true" Visible="false" ></asp:BoundColumn>
										    <asp:BoundColumn DataField="paz_codice" HeaderText="H_idPaz" ReadOnly="true" Visible="false" ></asp:BoundColumn>
										    <asp:BoundColumn DataField="paz_cns_codice" HeaderText="H_codSedeVacPrec" ReadOnly="true" Visible="false" ></asp:BoundColumn>
										    <asp:BoundColumn DataField="paz_stato_anagrafico" HeaderText="H_StatoAnagrafico" ReadOnly="true" Visible="false" ></asp:BoundColumn>
										    <asp:BoundColumn DataField="paz_stato_anagrafico" HeaderText="H_StatoAnagrafico" ReadOnly="true" Visible="false" ></asp:BoundColumn>
									    </Columns>
								    </asp:DataGrid>
                                </td>
								<td width="3px"></td>
							</tr>
						</table>
                    </div>
                </dyp:DynamicPanel>

                <dyp:DynamicPanel ID="dypDettaglio" runat="server" Width="100%" Height="90px">
                    <div>
						<table border="0" cellspacing="0" cellpadding="0" width="100%">
							<tr style="height:22px">
								<td>
									<div class="Sezione">&nbsp;DETTAGLIO CENTRO VACCINALE</div>
								</td>
							</tr>
							<tr style="height:68px">
								<td>
									<table border="0" cellspacing="0" cellpadding="2" width="100%" style="height:100%">
										<colgroup>
                                            <col width="30%" />
                                            <col width="55%" />
                                            <col width="15%" />
                                        </colgroup>
                                        <tr valign="middle">
                                            <td  class="label">
                                                <asp:Label ID="lblStatoAnagrafico" runat="server" Text="Stato anagrafico paziente"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlStatoAnagrafico" CssClass="label_left" runat="server" Width="100%" AutoPostBack="true" ></asp:DropDownList>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr valign="middle">
											<td class="label">Centro vaccinale assegnato</td>
											<td>
												<onitcontrols:OnitModalList id="txtConsultorioVacc" runat="server" Width="70%" Enabled="False" UseTableLayout="True"
													UseCode="True" CampoDescrizione="CNS_DESCRIZIONE Descrizione" CampoCodice="CNS_CODICE Codice" Tabella="T_ANA_CONSULTORI"
													SetUpperCase="True" RaiseChangeEvent="True" Label="Titolo" CodiceWidth="30%" LabelWidth="-8px" PosizionamentoFacile="False"></onitcontrols:OnitModalList></td>
											<td></td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
                    </div>
                </dyp:DynamicPanel>

			</on_lay3:onitlayout3>
		</form>
	</body>
</html>
