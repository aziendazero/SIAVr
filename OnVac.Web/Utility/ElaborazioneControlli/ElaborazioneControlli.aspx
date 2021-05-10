<%@ Page Language="vb" AutoEventWireup="False" CodeBehind="ElaborazioneControlli.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.ElaborazioneControlli" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register Assembly="Onit.Web.UI.WebControls.DynamicPanel" Namespace="Onit.Web.UI.WebControls.DynamicPanel" TagPrefix="dyp" %>
<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Elaborazione Controlli</title>
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <script type="text/javascript" language="javascript">
        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;

            switch (button.Key) {
                case 'btnAvvia':
                    if (!confirm('Vuoi avviare i controlli?')) evnt.needPostBack = false;
                    break;
            }

            return;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Height="100%" Width="100%" TitleCssClass="Title3" Titolo="Elaborazione controlli scuole">
            <div class="title" id="divLayoutTitolo" style="width: 100%">
                <asp:Label id="LayoutTitolo" runat="server">Utility di Elaborazione dei controlli per le scuole</asp:Label>
            </div>
            <div>
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="tlbElaborazione" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                    <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
                    <Items>
				        <igtbar:TBarButton Key="btnCerca" Text="Cerca" DisabledImage="~/Images/cerca_dis.gif" Image="~/Images/cerca.gif"
                            ToolTip="Effettua la ricerca in base filtri impostati">
				        </igtbar:TBarButton>
                        <igtbar:TBSeparator></igtbar:TBSeparator>
				        <igtbar:TBarButton Key="btnAvvia" Text="Avvia controlli" DisabledImage="../../images/rotella_dis.gif" Image="../../images/rotella.gif"
                            ToolTip="Avvia la procedura di controllo per l'elemento selezionato">
                            <DefaultStyle CssClass="infratoolbar_button_default" Width="110px"></DefaultStyle>
				        </igtbar:TBarButton>
				        <igtbar:TBarButton Key="btnDettagli" Text="Dettagli" DisabledImage="~/Images/dettaglio_dis.gif" Image="~/Images/dettaglio.gif" 
                            ToolTip="Visualizza i dettagli del processo selezionato">
				        </igtbar:TBarButton>
                    </Items>
                </igtbar:UltraWebToolbar>
            </div>
            <div class="vac-sezione">Filtri di ricerca</div>
             
            <asp:Panel id="pnlFiltri" runat="server" style="padding: 10px 0 10px 0; background-color: whitesmoke;">
                <table id="tableFiltri" style="width: 99%;" cellspacing="0" cellpadding="1" border="0">
                    <colgroup>
                        <col style="width: 10%; text-align: right" />
                        <col style="width: 37%" />
                        <col style="width: 5%; text-align: right" />
                        <col style="width: 10%" />
                        <col style="width: 5%; text-align: right" />
                        <col style="width: 10%" />
                        <col style="width: 11%; text-align: right" />
                        <col style="width: 12%" />
                    </colgroup>
                    <tr>
                        <td class="label">Operatore</td>
                        <td class="label_left" >
                            <on_ofm:OnitModalList ID="omlUtenteFiltro" runat="server" Width="70%" Height="24px" CodiceWidth="29%" UseCode="True" 
                                CampoCodice="UTE_CODICE CODICE" CampoDescrizione="UTE_DESCRIZIONE DESCRIZIONE" AltriCampi="UTE_ID ID" Tabella="T_ANA_UTENTI" 
                                SetUpperCase="False" LabelWidth="-8px" PosizionamentoFacile="False" Label="Medico" Filtro="1=1 order by UTE_CODICE "></on_ofm:OnitModalList>
                        </td>
                        <td class="label">Da</td>
                        <td>
                            <on_val:OnitDatePick ID="odpDaDataCaricamentoFiltro" runat="server" DateBox="True" CssClass="TextBox_Data"></on_val:OnitDatePick>
                        </td>
                        <td class="label">A</td>
                        <td>
                            <on_val:OnitDatePick ID="odpADataCaricamentoFiltro" runat="server" DateBox="True" CssClass="TextBox_Data"></on_val:OnitDatePick>
                        </td>
                        <td class="label">Id Processo</td>
                        <td>
                            <asp:TextBox ID="txtIdProcessoFiltro" runat="server" Width="100%" CssClass="TextBox_Stringa" ></asp:TextBox>
                        </td>
                    </tr>
                </table>                    
            </asp:Panel>
                    
            <div class="vac-sezione">Risultati della ricerca</div>

            <div id="divLegenda" class="legenda-vaccinazioni">
                <span class="legenda-stato-import">
                    <img src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetStatoImportUrlIcona("S", Page) %>' alt="S" title='' />
                    <span><%= Onit.OnAssistnet.OnVac.OnVacUtility.GetStatoImportToolTip("S") %></span>
                </span>
                <span class="legenda-stato-import">
                    <img src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetStatoImportUrlIcona("F", Page) %>' alt="S" title='' />
                    <span><%= Onit.OnAssistnet.OnVac.OnVacUtility.GetStatoImportToolTip("F") %></span>
                </span>
                <span class="legenda-stato-import">
                    <img src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetStatoImportUrlIcona("R", Page) %>' alt="S" title='' />
                    <span><%= Onit.OnAssistnet.OnVac.OnVacUtility.GetStatoImportToolTip("R") %></span>
                </span>
                <span class="legenda-stato-import">
                    <img src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetStatoImportUrlIcona("W", Page) %>' alt="S" title='' />
                    <span><%= Onit.OnAssistnet.OnVac.OnVacUtility.GetStatoImportToolTip("W") %></span>
                </span>
			</div>

            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" ExpandDirection="horizontal">
                <on_dgr:OnitGrid ID="dgrElaborazioneControlli" runat="server" Width="100%" PagerVoicesBefore="-1" PagerVoicesAfter="-1"
                    AllowSelection="true" EditMode="None" AutoGenerateColumns="False" 
                     SelectionOption="rowClick" DataKeyField="IdCaricamento" AllowPaging="true" PageSize="25" AllowCustomPaging="true">
                    <HeaderStyle CssClass="header"></HeaderStyle>
                    <FooterStyle CssClass="footer"></FooterStyle>
                    <PagerStyle CssClass="pager"></PagerStyle>
                    <ItemStyle CssClass="item"></ItemStyle>
                    <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
                    <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
                    <EditItemStyle CssClass="edit"></EditItemStyle>
                    <PagerStyle CssClass="pager" Position="TopAndBottom" Mode="NumericPages" />
                    <Columns>
                        <on_dgr:SelectorColumn></on_dgr:SelectorColumn>                        
                        <on_dgr:OnitBoundColumn SortDirection="NoSort" DataField="CodiceUtenteCaricamento" HeaderText="Operatore" key="CodiceUtenteCaricamento" >
                        </on_dgr:OnitBoundColumn>
                        <on_dgr:OnitBoundColumn SortDirection="NoSort" DataField="IdCaricamento" HeaderText="Processo" key="IdCaricamento">
                        </on_dgr:OnitBoundColumn>
                        <on_dgr:OnitTemplateColumn SortDirection="NoSort" HeaderText="Data Caricamento" key="DataInizioCaricamento">
                            <ItemTemplate>
                                <asp:Label ID="lblDataCaricamento" runat="server" Text='<%# ConvertToDateString(Eval("DataInizioCaricamento")) %>'></asp:Label>
                            </ItemTemplate>
                        </on_dgr:OnitTemplateColumn>
                        <on_dgr:OnitBoundColumn SortDirection="NoSort" DataField="NomeFileOrigine" HeaderText="File" key="NomeFileOrigine"></on_dgr:OnitBoundColumn>
                        <on_dgr:OnitTemplateColumn key="StatoCaricamento" HeaderText="Caric." >
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Image ID="imgStatoCaricamento" runat="server"
                                    ImageUrl='<%# Onit.OnAssistnet.OnVac.OnVacUtility.GetStatoImportUrlIcona(Eval("StatoCaricamento").ToString(), Page) %>' ToolTip='<%# Onit.OnAssistnet.OnVac.OnVacUtility.GetStatoImportToolTip(Eval("StatoCaricamento").ToString()) %>' />
                            </ItemTemplate>
                        </on_dgr:OnitTemplateColumn>
                        <on_dgr:OnitBoundColumn SortDirection="NoSort" DataField="RigheElabScart" HeaderText="N. Record" key="RigheElabScart">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </on_dgr:OnitBoundColumn>
                        <on_dgr:OnitTemplateColumn key="StatoControllo" HeaderText="Controllo">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Image ID="imgStatoControllo" runat="server"
                                    ImageUrl='<%# Onit.OnAssistnet.OnVac.OnVacUtility.GetStatoImportUrlIcona(Eval("StatoControllo").ToString(), Page) %>' ToolTip='<%# Onit.OnAssistnet.OnVac.OnVacUtility.GetStatoImportToolTip(Eval("StatoControllo").ToString()) %>' />
                            </ItemTemplate>
                        </on_dgr:OnitTemplateColumn>
                        <on_dgr:OnitBoundColumn SortDirection="NoSort" DataField="TotaleRecord" HeaderText="Tot" key="TotaleRecord" >
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </on_dgr:OnitBoundColumn>
                        <on_dgr:OnitBoundColumn SortDirection="NoSort" DataField="TotaleControllati" HeaderText="Elab" key="TotaleControllati">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </on_dgr:OnitBoundColumn>
                        <on_dgr:OnitBoundColumn SortDirection="NoSort" DataField="TotaleErrore" HeaderText="Err" key="TotaleErrore">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </on_dgr:OnitBoundColumn>
                        <on_dgr:OnitBoundColumn SortDirection="NoSort" DataField="IdControllo" HeaderText="IdControllo" key="IdControllo" Visible="false" >
                            </on_dgr:OnitBoundColumn>
                        <on_dgr:OnitBoundColumn SortDirection="NoSort" DataField="CodiceCaricamento" HeaderText="CodiceCaricamento" key="CodiceCaricamento" Visible="false" >
                            </on_dgr:OnitBoundColumn>
                        <on_dgr:OnitBoundColumn SortDirection="NoSort" DataField="StatoCaricamento" HeaderText="StatoCaricamento" key="StatoCaricamento" Visible="false" >
                            </on_dgr:OnitBoundColumn>
                        <on_dgr:OnitBoundColumn SortDirection="NoSort" DataField="RigheElabDiff" HeaderText="RigheElabDiff" key="RigheElabDiff" Visible="false" >
                            </on_dgr:OnitBoundColumn>
                    </Columns>
                </on_dgr:OnitGrid>
            </dyp:DynamicPanel>
        </on_lay3:OnitLayout3>
    </form>
</body>
</html>
