<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ElaborazioniVaccinazioniPazienti.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.ElaborazioniVaccinazioniPazienti" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>

<%@ Import Namespace="Onit.OnAssistnet.OnVac" %>
<%@ Import Namespace="Onit.OnAssistnet.OnVac.Entities" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>Elaborazioni Vaccinazioni Pazienti</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <style type="text/css">
        .toUpper {
            text-transform: uppercase;
        }

        .legendFiltri {
            text-align: left;
            font-family: Arial;
            font-size: 12px;
            font-weight: bold;
            color: Navy;
        }
    </style>

    <script type="text/javascript" language="javascript">
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
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Height="100%" Width="100%" TitleCssClass="Title3" Titolo="Elaborazioni Vaccinazioni Pazienti">
            <div>
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="130px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover" ></HoverStyle>
				    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                    <ClientSideEvents Click="ToolBarClick" InitializeToolbar="InizializzaToolBar"></ClientSideEvents>
		            <Items>
                        <igtbar:TBarButton Key="btnCerca" Text="Cerca" ToolTip="Effettua la ricerca in base ai filtri impostati"
                            Image="../../images/cerca.gif" >
                            <DefaultStyle Width="80px" CssClass="infratoolbar_button_default"></DefaultStyle>
		                </igtbar:TBarButton>		 
                        <igtbar:TBSeparator />
                        <igtbar:TBarButton Key="btnAcquisisci" Text="Avvia Acquisizione" ToolTip="Avvia l'acquisizione delle elaborazioni" 
                            Image="../../images/rotella.gif" >
		                </igtbar:TBarButton>	
                        <igtbar:TBSeparator />
                        <igtbar:TBarButton Key="btnCambiaVista" Text="Vista Completa" DisabledImage="~/Images/dettaglio_dis.gif"
							Image="~/Images/dettaglio.gif">
                        </igtbar:TBarButton>
                    </Items>
                </igtbar:UltraWebToolbar>
            </div>
            <div class="vac-sezione" style="width: 100%;">Filtri di ricerca </div>
            <div>
                <table style="background-color: #F5F5F5; margin:2px; border:0; width:100%">
                    <tr>
                        <td style="text-align:center">
                            <fieldset class="vacFieldset" title="Acquisizione">
                                <legend><span class="legendFiltri">Acquisizione</span></legend>
                                <table style="margin:4px 0px 2px 0px; border:0px; width:99%">
                                    <colgroup>
                                        <col width="7%" />
                                        <col width="53%" />
                                        <col  width="6%" />
                                        <col width="120px" />
                                        <col  width="4%" />
                                        <col width="120px" />
                                        <col  width="14%" />
                                        <col width="14%" />
                                        <col width="2%" />
                                    </colgroup>
                                    <tr>
                                        <td class="label">Stato</td>
                                        <td>
                                            <asp:DropDownList ID="ddlFtrStatiAcq" Runat="server" Width="100%" /></td>                                            
                                        <td nowrap="nowrap" class="label">Da</td>
                                        <td>
                                            <on_val:onitdatepick ID="dpkFltAcqDa" runat="server" CssClass="textbox_data" DateBox="True" Height="20px" Width="120px"></on_val:onitdatepick></td> 
                                        <td nowrap="nowrap" class="label">A</td>
                                        <td>
                                            <on_val:onitdatepick ID="dpkFltAcqA" runat="server" CssClass="textbox_data" DateBox="True" Height="20px" Width="120px"></on_val:onitdatepick></td> 
                                        <td nowrap="nowrap" class="label">Id Processo</td>
                                        <td>
                                            <on_val:OnitJsValidator id="txtFltIdPrcAcq" runat="server" CssClass="textbox_numerico w100p"
												actionCorrect="False" actionDelete="False" actionFocus="True" actionMessage="True" actionSelect="False"
												actionUndo="True" autoFormat="True" validationType="Validate_custom" CustomValFunction-Length="2" CustomValFunction="validaNumero"
												SetOnChange="True" MaxLength="10"></on_val:OnitJsValidator></td>
                                        <td></td>
                                    </tr>
			                    </table>
		                    </fieldset>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="vac-sezione">
                <asp:Label id="lblLegenda" runat="server">Legenda</asp:Label>
            </div>
            <div>
                <table style="background-color: #F5F5F5; margin:2px; width:100%;">
                    <tr>
                        <td style="text-align:center">
                            <fieldset class="vacFieldset" title="Stati Acquisizione">
                                <legend><span class="legendFiltri">Stati Acquisizione</span></legend>
                                <asp:DataList ID="dlsLegStatiAcq" RepeatDirection="Horizontal" RepeatLayout="Table" RepeatColumns="3" 
                                    style="margin:4px 0px 2px 0px; width:99%; border:0px;" runat="server">
                                    <ItemTemplate>
                                        <table style="display:inline" cellspacing="0" cellpadding="0">
                                            <tr>
                                                <td>
                                                    <div id="divLegStatiAcq" class="<%# Me.GetCssClassStatoAcquisizione(DirectCast([Enum].Parse(GetType(Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente), DirectCast(Container.DataItem, ListItem).Value), Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente))%>" runat="server">&nbsp;</div>
                                                </td>
                                                <td nowrap="nowrap" class="Label" style="padding:0px 3px">
                                                    <asp:Label id="lblLegStatiAcq" Text="<%# DirectCast(Container.DataItem, ListItem).Text %>" runat="server" /></td>
                                            </tr>
                                        </table>
                                    </ItemTemplate>
                                </asp:DataList>   
                            </fieldset>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="vac-sezione">
                <asp:Label id="lblRisultati" runat="server">Risultati della ricerca</asp:Label>
            </div>

            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">

                <asp:DataGrid id="dgrElabVacPaz" runat="server" Width="100%" AutoGenerateColumns="False" AllowCustomPaging="true" AllowPaging="true" PageSize="25" >
                    <HeaderStyle CssClass="header"></HeaderStyle>
                    <ItemStyle CssClass="item"></ItemStyle>
                    <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
                    <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
                    <PagerStyle CssClass="pager" Mode="NumericPages" Position="TopAndBottom" />
                    <Columns>
                        <asp:BoundColumn DataField="Id" Visible="false"></asp:BoundColumn>
                        <asp:TemplateColumn>
                            <HeaderStyle HorizontalAlign="Center" ></HeaderStyle>
                            <ItemTemplate>
                                <div runat="server" class="<%# Me.GetCssClassStatoAcquisizione(DirectCast(Container.DataItem, ElaborazioneVaccinazionePaziente).StatoAcquisizione)%>" 
                                    style="width:10px; height:14px; padding:0px; "></div>
                            </ItemTemplate>                                
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="CodiceFiscalePaziente" HeaderText="Codice Fiscale">
                            <HeaderStyle Width="130px" Wrap="false"></HeaderStyle>
						</asp:BoundColumn>
                        <asp:BoundColumn DataField="CognomePaziente" HeaderText="Cognome"></asp:BoundColumn>
                        <asp:BoundColumn DataField="NomePaziente" HeaderText="Nome"></asp:BoundColumn>
                        <asp:BoundColumn DataField="SessoPaziente" HeaderText="Sesso">
                            <HeaderStyle Width="50px"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="DataNascitaPaziente" DataFormatString="{0:d}" HeaderText="Data&lt;br/&gt;Nascita">
                            <HeaderStyle HorizontalAlign="Center" Width="80px"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
						</asp:BoundColumn>
					    <asp:BoundColumn DataField="DescrizioneComuneNascitaPaziente" HeaderText="Comune Nascita">
                            <HeaderStyle Wrap="false"></HeaderStyle>
						</asp:BoundColumn>
                        <asp:BoundColumn DataField="DataEffettuazione" DataFormatString="{0:d}" HeaderText="Data&lt;br/&gt;Effettuazione">
                            <HeaderStyle HorizontalAlign="Center" Width="80px"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
						</asp:BoundColumn>
                        <asp:BoundColumn DataField="DescrizioneAssociazione" HeaderText="Associazione"></asp:BoundColumn>
                        <asp:BoundColumn DataField="DescrizioneVaccinazione" HeaderText="Vaccinazione"></asp:BoundColumn>  
                        <asp:BoundColumn DataField="CodicePaziente" HeaderText="Codice" Visible="false"></asp:BoundColumn>                          
                        <asp:BoundColumn DataField="CodiceRegionalePaziente" HeaderText="Codice Regionale" Visible="false"></asp:BoundColumn>   
                        <asp:BoundColumn DataField="TesseraSanitariaPaziente" HeaderText="Tessera Sanitaria" Visible="false"></asp:BoundColumn>      
                        <asp:BoundColumn DataField="NomeOperatore" HeaderText="Operatore" Visible="false"></asp:BoundColumn>     
                        <asp:BoundColumn DataField="DataAcquisizione" DataFormatString="{0:G}" HeaderText="Data&lt;br/&gt;Acquisizione" Visible="false"></asp:BoundColumn>    
                        <asp:BoundColumn DataField="CodicePazienteAcquisizione" HeaderText="Codice Paziente Acquisizione" Visible="false"></asp:BoundColumn>     
                        <asp:BoundColumn DataField="IdProcessoAcquisizione" HeaderText="Processo Acquisizione" Visible="false"></asp:BoundColumn>     
                        <asp:TemplateColumn>
                            <HeaderStyle HorizontalAlign="Center" Width="18px"></HeaderStyle>
                            <ItemTemplate>
                                <asp:ImageButton ID="ibtMsqAcq" runat="server" CommandName="showMsqAcq" ImageUrl='<%# Page.ResolveClientUrl("~/Images/info.png")%>'
                                    Visible="<%# not string.isnullorempty(DirectCast(Container.DataItem, ElaborazioneVaccinazionePaziente).MessaggioAcquisizione)%>" />
                            </ItemTemplate>                              
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>

            </dyp:DynamicPanel>

            <onitcontrols:OnitFinestraModale ID="modAcqInfo" Title="Messaggio Acquisizione" runat="server" Width="600px" BackColor="LightGray">
                <asp:TextBox ID="txtAcqInfo" TextMode="MultiLine" Rows="15" Width="100%" runat="server" />
            </onitcontrols:OnitFinestraModale>
        
        </on_lay3:OnitLayout3>
    </form>
</body>
</html>
