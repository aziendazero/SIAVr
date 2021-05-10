<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="AssistitiEmigrati.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.AssistitiEmigrati" %>

<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<%@ Register TagPrefix="uc1" TagName="uscFiltriStampaEtichetteMovAusl" Src="../uscFiltriStampaEtichetteMovAusl.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Assistiti Emigrati</title>
        
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
    <script type="text/javascript" language="javascript">
        $(document).ready(function () {

            // Imposta l'altezza uguale nelle table dei filtri
            try {
                var max = 0;
                $('[id=tblCertificato],[id=tblNotifica]').each(function () {
                    max = Math.max($(this).height(), max);
                }).height(max);
            }
            catch (e) {
                // 
            }
        });

        function ToolBarClick(t, button, evnt) 
        {
            evnt.needPostBack = true;

            switch (button.Key) 
            {
                case 'btnCerca':
                    if (!document.getElementById('<%=rdbCertNonStampato.ClientID%>').checked)
                    {
                        if (!confirm('ATTENZIONE: la ricerca dei pazienti con certificato gia\' stampato potrebbe impiegare molto tempo. Continuare?')) 
                        {
                            evnt.needPostBack = false;
                        }
                    }
                    break;

                case 'btnStampaEtichette':
                    showFm('fmFiltriEtichette', true, 250);
                    evnt.needPostBack = false;
                    break;

                case 'btnCertificato':
                    st(evnt);
                    break;

                case 'btnPulisci':
                    OnitDataPickSet('<%=odpDaNascita.ClientID%>', '')
                    OnitDataPickSet('<%=odpANascita.ClientID%>', '')
                    OnitDataPickSet('<%=dpkDaEmig.ClientID%>', '')
                    OnitDataPickSet('<%=dpkAEmig.ClientID%>', '')
                    document.getElementById('<%=rdbCertNonStampato.ClientID%>').checked = true;
                    var rdbDaNotificare = document.getElementById('<%=rdbDaNotificare.ClientID%>');
                    if (rdbDaNotificare) {rdbDaNotificare.checked = true; }
                    evnt.needPostBack = false;
                    break;
            }
        }

        function clickStampa() 
        {
            closeFm('fmFiltriEtichette');
            document.getElementById('btnStampaEtichette').click();
        }

        function st(evt) 
        {
            evt.needPostBack = false;

            if (confirm("Attenzione: si desidera impostare lo stato del Certificato Vaccinale dei pazienti a \'stampato\'?"))
                __doPostBack('ImpostaStatoS');
            else
                __doPostBack('ImpostaStatoN');
        }
                
        function controlloRedirect() 
        {
            return controlloRedirectToPaziente(<%= Me.IsPageInEdit().ToString().ToLower() %>, true);
        }
    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Width="100%" Height="100%" TitleCssClass="Title3" Busy="False" Titolo="Assistiti Emigrati">
            <div>
				<table border="0" cellspacing="0" cellpadding="0" width="100%">
					<tr>
						<td>
							<div style="width: 100%" id="divTitolo" class="title">&nbsp;Assistiti emigrati</div>
                        </td>
                    </tr>
                    <tr>
						<td>
							<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="tlbMovimenti" runat="server" ItemWidthDefault="120px" CssClass="infratoolbar">
                                <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                                <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		                        <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
								<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
								<Items>
									<igtbar:TBarButton Key="btnCerca" ToolTip="Effettua la ricerca degli assistiti" Text="Cerca" DisabledImage="~/Images/cerca_dis.gif"
										Image="~/Images/cerca.gif">
										<DefaultStyle Width="70px" CssClass="infratoolbar_button_default"></DefaultStyle>
									</igtbar:TBarButton>
									<igtbar:TBSeparator></igtbar:TBSeparator>
									<igtbar:TBarButton Key="btnStampaElenco" ToolTip="Stampa l'elenco dei movimenti" Text="Stampa elenco"
										DisabledImage="~/Images/stampa_dis.gif" Image="~/Images/stampa.gif">
									</igtbar:TBarButton>
									<igtbar:TBarButton Key="btnStampaElencoPerComune" ToolTip="Stampa l'elenco dei movimenti per comune"
										Text="Stampa elenco per comune" DisabledImage="~/Images/stampa_dis.gif" Image="~/Images/stampa.gif">
										<DefaultStyle Width="180px" CssClass="infratoolbar_button_default"></DefaultStyle>
									</igtbar:TBarButton>
									<igtbar:TBarButton Key="btnStampaEtichette" ToolTip="Stampa le etichette" Text="Stampa etichette" DisabledImage="~/Images/stampa_dis.gif"
										Image="~/Images/stampa.gif">
									</igtbar:TBarButton>
									<igtbar:TBarButton Key="btnCertificato" ToolTip="Stampa il Certificato vaccinale" Text="Certif. Vaccinale"
										DisabledImage="~/Images/stampa_dis.gif" Image="~/Images/stampa.gif">
									</igtbar:TBarButton>
									<igtbar:TBSeparator></igtbar:TBSeparator>
									<igtbar:TBarButton Key="btnPulisci" ToolTip="Resetta i filtri impostati ai valori di default" Text="Pulisci"
										DisabledImage="~/Images/pulisci_dis.gif" Image="~/Images/pulisci.gif">
										<DefaultStyle Width="70px" CssClass="infratoolbar_button_default"></DefaultStyle>
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
							<table bgcolor="whitesmoke" border="0" cellpadding="2" cellspacing="0" width="100%">
								<tr height="10">
									<td colspan="6"></td>
								</tr>
								<tr>
									<td class="label" width="15%"><b>Data di nascita:</b></td>
									<td class="label" width="5%">Da</td>
                                    <td width="30%">
										<on_val:OnitDatePick ID="odpDaNascita" runat="server" DateBox="True"></on_val:OnitDatePick></td>
									<td class="label" width="5%">A</td>
									<td width="30%">
										<on_val:OnitDatePick ID="odpANascita" runat="server" DateBox="True"></on_val:OnitDatePick></td>
									<td width="20%"></td>
								</tr>
								<tr>
									<td class="label"><b>Data di emigrazione:</b></td>
									<td class="label">Da</td><td>
										<on_val:OnitDatePick ID="dpkDaEmig" runat="server" DateBox="True"></on_val:OnitDatePick></td>
									<td class="label">A</td><td>
										<on_val:OnitDatePick ID="dpkAEmig" runat="server" DateBox="True"></on_val:OnitDatePick></td>
									<td></td>
								</tr>
								<tr height="10">
									<td colspan="6"></td>
								</tr>
								<tr>
									<td class="label" style="padding-top: 7px" valign="top"><b>Pazienti:</b></td>
                                    <td></td>
									<td>
									    <asp:Table ID="tblCertificato" BackColor="#E7E7FF" BorderColor="Navy" BorderStyle="Solid" BorderWidth="1px" CssClass="label_left" Width="70%" runat="server">
									        <asp:TableRow runat="server">
									            <asp:TableCell VerticalAlign="Top" runat="server">
									                    <asp:RadioButton ID="rdbCertStampato" Text="Certificato emigrazione stampato" GroupName="Cert" runat="server" />
									            </asp:TableCell>
									        </asp:TableRow>
									            <asp:TableRow VerticalAlign="Top" runat="server">
									            <asp:TableCell runat="server">
									                    <asp:RadioButton ID="rdbCertNonStampato" Text="Certificato emigrazione non stampato" Checked="true" GroupName="Cert" runat="server" />
									            </asp:TableCell>
									        </asp:TableRow>
									        <asp:TableRow VerticalAlign="Top" runat="server">
									            <asp:TableCell runat="server">
									                    <asp:RadioButton ID="rdbCertIgnora" Text="Entrambi" GroupName="Cert"  runat="server" />
									            </asp:TableCell>
									        </asp:TableRow>
									    </asp:Table>
									</td>
									<td></td>
									<td>
                                        <asp:Table ID="tblNotifica" BackColor="#E7E7FF" BorderColor="Navy" BorderStyle="Solid" BorderWidth="1px" CssClass="label_left" Width="70%" runat="server">
									        <asp:TableRow runat="server">
									            <asp:TableCell VerticalAlign="Top" runat="server" >
									                    <asp:RadioButton ID="rdbNotificati" Text="Notificati" GroupName="Notifica"  runat="server" />
									            </asp:TableCell>
									        </asp:TableRow>
									            <asp:TableRow runat="server">
									            <asp:TableCell VerticalAlign="Top" runat="server">
									                    <asp:RadioButton ID="rdbDaNotificare" Text="Da notificare" Checked="true" GroupName="Notifica" runat="server" />
									            </asp:TableCell>
									        </asp:TableRow>
                                            <asp:TableRow runat="server">
									            <asp:TableCell VerticalAlign="Top" runat="server">
									                    <asp:RadioButton ID="rdbIgnoraNotificare" Text="Entrambi" GroupName="Notifica"  runat="server" />
									            </asp:TableCell>
									        </asp:TableRow>
									    </asp:Table>									
								    </td>
								    <td></td>
								</tr>
								<tr height="10">
									<td colspan="6"></td>
								</tr>
							</table>
						</td>
					</tr>
					<tr>
						<td>
							<div id="divSezioneMovimenti"  runat="server" class="Sezione">&#160;MOVIMENTI</div></td>
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
						            AllowCustomPaging="true" AllowPaging="true" PageSize="200">
						            <AlternatingItemStyle CssClass="alternating" />
						            <ItemStyle CssClass="item" />
						            <PagerStyle CssClass="pager" Position="TopAndBottom" Mode="NumericPages" />
                                    <HeaderStyle CssClass="header" />
						            <SelectedItemStyle CssClass="selected" />
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
										    <HeaderStyle Width="4%" HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
							            </asp:ButtonColumn>
							            <asp:BoundColumn DataField="paz_cognome" HeaderText="Cognome" ReadOnly="true" >
								            <HeaderStyle Width="17%"></HeaderStyle>
							            </asp:BoundColumn>
							            <asp:BoundColumn DataField="paz_nome" HeaderText="Nome" ReadOnly="true" >
								            <HeaderStyle Width="17%"></HeaderStyle>
							            </asp:BoundColumn>
							            <asp:BoundColumn DataField="paz_data_nascita" HeaderText="Data Nascita" DataFormatString="{0:dd/MM/yyyy}" ReadOnly="true" >
								            <HeaderStyle Width="11%" HorizontalAlign="Center" />
										    <ItemStyle HorizontalAlign="Center" />
							            </asp:BoundColumn>
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
							            <asp:BoundColumn DataField="paz_data_emigrazione" HeaderText="Data Emigrazione" DataFormatString="{0:dd/MM/yyyy}" ReadOnly="true" >
								            <HeaderStyle Width="10%" HorizontalAlign="Center" />
										    <ItemStyle HorizontalAlign="Center" />
							            </asp:BoundColumn>
							            <asp:BoundColumn DataField="emi_com_descrizione" HeaderText="Comune Emigrazione" ReadOnly="true" >
								            <HeaderStyle Width="20%"></HeaderStyle>
							            </asp:BoundColumn>
							            <asp:TemplateColumn HeaderText="Certificato Emigrazione Stampato" >
								            <HeaderStyle Width="100px" HorizontalAlign="Center"></HeaderStyle>
								            <ItemStyle HorizontalAlign="Center"></ItemStyle>
								            <ItemTemplate>
									            <asp:ImageButton id="btnCertEmi" runat="server" CommandName="CertEmi" Visible='<%# iif(databinder.eval(Container,"DataItem")("paz_sta_certificato_emi").toString="S",false,true) %>' ImageUrl="~/Images/duplica.gif" AlternateText="Certificato emigrazione">
									            </asp:ImageButton>
									            <asp:Label id="lblCertEmi" runat="server" CssClass="label" Visible='<%# iif(databinder.eval(Container,"DataItem")("paz_sta_certificato_emi").toString="S",true,false) %>'>Si</asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="Notifica">
								            <HeaderStyle Width="50px" HorizontalAlign="Center"></HeaderStyle>
								            <ItemStyle HorizontalAlign="Center"></ItemStyle>
								            <ItemTemplate>
									            <asp:ImageButton id="btnNotifica" runat="server" CommandName="Notifica" ImageUrl="~/images/mov_uscita.gif" AlternateText="Notifica">
									            </asp:ImageButton>
									            <asp:Label id="lblNotifica" runat="server" CssClass="label">Si</asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateColumn>
                                    </Columns>
					            </asp:DataGrid>
                            </td>
				            <td width="3px"></td>
			            </tr>
		            </table>
                </div>
            </dyp:DynamicPanel>
        </on_lay3:OnitLayout3>
    
        <onitcontrols:OnitFinestraModale ID="fmFiltriEtichette" Title="<div style=&quot;font-family:'Microsoft Sans Serif'; font-size: 11pt;padding-bottom:2px&quot;>&nbsp;Stampa Etichette</div>"
            runat="server" Height="185px" Width="420px" BackColor="LightGray" NoRenderX="True" RenderModalNotVisible="True">
            <table border="0" cellspacing="0" cellpadding="0" width="100%" style="background-color: LightGrey;width: 420px; height: 185px">
                <tr>
                    <td></td>
                    <td colspan="3">
                        <uc1:uscFiltriStampaEtichetteMovAusl ID="UscFiltriEtichette" runat="server" TipoMovimento="E">
                        </uc1:uscFiltriStampaEtichetteMovAusl>
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td width="1%"></td>
                    <td width="45%" align="right">
                        <input type="button" style="cursor: hand" id="inputStampaEtichette" onclick="clickStampa();" value="Stampa" />
                        <asp:Button Style="display: none" ID="btnStampaEtichette" runat="server" Text="Stampa"></asp:Button>
                    </td>
                    <td width="8%"></td>
                    <td width="45%" align="left">
                        <input type="button" style="cursor: hand" id="btnAnnullaEtichette" onclick="closeFm('fmFiltriEtichette')" value="Annulla" />
                    </td>
                    <td width="1%"></td>
                </tr>
            </table>
        </onitcontrols:OnitFinestraModale>

    </form>
</body>
</html>
