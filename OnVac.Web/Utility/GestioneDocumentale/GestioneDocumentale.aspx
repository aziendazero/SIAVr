<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="GestioneDocumentale.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.GestioneDocumentale" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="dyp" Assembly="Onit.Web.UI.WebControls.DynamicPanel" Namespace="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="uc3" TagName="FirmaDigitaleInfo" Src="../../Common/Controls/FirmaDigitaleInfo.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Gestione Documentale</title>
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <style type="text/css">
        .label_left {
            margin-right: 50px;
        }
    </style>

    <script type="text/javascript"  src="<%= ResolveUrl("~/scripts/json/json2.js") %>"></script>
    <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
    <script type="text/javascript" src='<%= ResolveClientUrl("~/common/scripts/signature.js")  %>'></script>

    <script type="text/javascript">

        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;

            switch (button.Key) {
                case 'btnCerca':
						
                    if (<%= (String.IsNullOrWhiteSpace(dpkFiltroDataCompilazioneA.Text) Or String.IsNullOrWhiteSpace(dpkFiltroDataCompilazioneDa.Text)).ToString().ToLower()%> == 'true') 
                    {
                        alert("Inserire tutti i filtri data per proseguire con la ricerca.");
                        evnt.needPostBack = false;
                    }
                    break;
            }
        }

        function ImpostaImmagineOrdinamento(imgId, imgUrl) {
            var img = document.getElementById(imgId);
            if (img != null) {
                img.style.display = 'inline';
                img.src = imgUrl;
            }
        }

        function SelezionaTutti(chkValue){
            __doPostBack('selectAll', chkValue);
        }

        function SignatureCallBack(s, e) {
            var lnk = document.getElementById('<% =lnkPostBk.ClientID %>');
            var hd = document.getElementById('<% =txtResult.ClientID%>');
            hd.value = JSON.stringify(e);
            resetWaitScreen(true);
            lnk.click();
        }
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" TitleCssClass="Title3" Titolo="Gestione Documentale">
            
            <asp:HiddenField ID="txtResult" runat="server" />
            <asp:LinkButton ID="lnkPostBk" runat="server" style="display:none"></asp:LinkButton>

            <div class="title">
                <asp:Label ID="LayoutTitolo" runat="server" Text="Gestione Documentale"></asp:Label>
            </div>
            <div>
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolBar" runat="server" ItemWidthDefault="100px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                    <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
                    <Items>
                        <igtbar:TBarButton Key="btnCerca" Text="Cerca" ToolTip="Ricerca documenti in base ai filtri impostati"
                            DisabledImage="~/Images/cerca_dis.gif" Image="~/Images/cerca.gif">
                        </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btnFirma" Text="Firma digitale" DisabledImage="../../images/firmaDigitale_dis.png"
                            Image="../../images/firmaDigitale.png" ToolTip="Firma digitalmente i documenti selezionati">
                        </igtbar:TBarButton>
                        <igtbar:TBSeparator></igtbar:TBSeparator>
                        <igtbar:TBarButton Key="btnPulisci" Text="Pulisci" ToolTip="Resetta i filtri impostati ai valori di default"
                            DisabledImage="../../images/eraser.png" Image="../../images/eraser.png">
                            <DefaultStyle CssClass="infratoolbar_button_default" Width="70px"></DefaultStyle>
                        </igtbar:TBarButton>
                    </Items>
                </igtbar:UltraWebToolbar>
            </div>
            <div style="background-color: whitesmoke; padding-top: 5px; width: 100%">
                <fieldset class="vacFieldSet" title="Filtri ricerca documenti" style="width: 99%; margin-left: 10px;">
                    <legend class="label">Ricerca Documenti</legend>
                    <table style="border-style: none; border-width: 0px; width: 100%" cellpadding="2" cellspacing="0">
                        <colgroup>
                            <col width="15%" />
                            <col width="3%" />
                            <col width="20%" />
                            <col width="3%" />
                            <col width="56%" />
                            <col width="3%" />
                        </colgroup>
                        <tr>
                            <td class="label">Utente Registrazione</td>
                            <td colspan="4">
                                <onitcontrols:OnitModalList ID="omlUtenteRegistrazione" runat="server" AltriCampi="UTE_ID as Id"
                                    Width="70%" CampoDescrizione="UTE_DESCRIZIONE as Descrizione" CampoCodice="UTE_CODICE as Codice"
                                    Tabella="V_ANA_UTENTI" PosizionamentoFacile="False" LabelWidth="-1px" CodiceWidth="29%" Label="Titolo"
                                    UseCode="True" SetUpperCase="False" Obbligatorio="False"></onitcontrols:OnitModalList>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <td class="label">Utente Firma</td>
                            <td colspan="4">
                                <onitcontrols:OnitModalList ID="omlUtenteFirma" runat="server" AltriCampi="UTE_ID as Id"
                                    Width="70%" CampoDescrizione="UTE_DESCRIZIONE as Descrizione" CampoCodice="UTE_CODICE as Codice"
                                    Tabella="V_ANA_UTENTI" PosizionamentoFacile="False" LabelWidth="-1px" CodiceWidth="29%" Label="Titolo"
                                    UseCode="True" SetUpperCase="False" Obbligatorio="False"></onitcontrols:OnitModalList>

                            </td>
                            <td></td>
                        </tr>                        
                        <tr>
                            <td class="label">Rilevatore</td>
                            <td colspan="4">
                                <onitcontrols:OnitModalList ID="omlRilevatore" runat="server"
                                    Width="70%" CampoDescrizione="OPE_NOME Rilevatore" CampoCodice="OPE_CODICE Codice" 
                                    Tabella="T_ANA_OPERATORI,T_ANA_LINK_OPER_CONSULTORI" PosizionamentoFacile="False" LabelWidth="-1px" CodiceWidth="29%" Label="Titolo"
                                    UseCode="True" SetUpperCase="True" Obbligatorio="False"></onitcontrols:OnitModalList>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <td class="label">Stato documenti</td>
                            <td colspan="5">
                                <asp:RadioButton ID="rdbFiltroStatoDaFirmare" runat="server" Text="Solo da firmare" CssClass="label_left" GroupName="filtroStato" Checked="true" />
                                <asp:RadioButton ID="rdbFiltroStatoFirmNoArc" runat="server" Text="Firmati ma non archiviati" CssClass="label_left" GroupName="filtroStato" />
                                <asp:RadioButton ID="rdbFiltroStatoFirmArc" runat="server" Text="Firmati e archiviati" CssClass="label_left" GroupName="filtroStato" />
                                <asp:RadioButton ID="rdbFiltroStatoTutti" runat="server" Text="Tutti i documenti" CssClass="label_left" GroupName="filtroStato" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">Data compilazione</td>
                            <td class="label">Da</td>
                            <td>
                                <on_val:OnitDatePick ID="dpkFiltroDataCompilazioneDa" runat="server" CssClass="textbox_data" DateBox="True"></on_val:OnitDatePick>
                            </td>
                            <td class="label">A</td>
                            <td>
                                <on_val:OnitDatePick ID="dpkFiltroDataCompilazioneA" runat="server" CssClass="textbox_data" DateBox="True"></on_val:OnitDatePick>
                            </td>
                            <td></td>
                        </tr>
                    </table>
                </fieldset>
                <div class="vac-sezione" style="margin-left: 2px; margin-right: 2px; margin-top: 2px;">
                    <asp:Label ID="lblDocumenti" runat="server">Documenti Trovati</asp:Label>
                </div>
            </div>
            <dyp:DynamicPanel ID="dypDocumenti" runat="server" Width="100%" Height="100%" ScrollBars="Auto">
                <asp:DataGrid ID="dgrDocumenti" runat="server" Width="100%" AutoGenerateColumns="False"
                    AllowCustomPaging="true" AllowPaging="true" PageSize="25" AllowSorting="True">
                    <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
                    <ItemStyle CssClass="item"></ItemStyle>
                    <PagerStyle CssClass="pager" Position="TopAndBottom" Mode="NumericPages" />
                    <HeaderStyle CssClass="header"></HeaderStyle>
                    <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
                    <Columns>
                        <asp:TemplateColumn>
                            <HeaderStyle Width="2%" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <HeaderTemplate>
                                <input type="checkbox" id="chkSelezioneHeader" onclick="SelezionaTutti(this.checked);" title="Seleziona tutti" runat="server" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSelezioneItem" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderStyle-Width="0%">
                            <ItemTemplate>
                                <asp:HiddenField ID="hidIdVisita" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "IdVisita")%>' />
                                <asp:HiddenField ID="hidUteFirma" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "IdUtenteFirma")%>' />
                                <asp:HiddenField ID="hidUteArchiviazione" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "IdUtenteArchiviazione")%>' />
                                <asp:HiddenField ID="hidIdDocumento" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "IdDocumento")%>' />
                                <asp:HiddenField ID="hidCodAzInserimento" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "CodiceAziendaInserimento")%>' />
                                <asp:HiddenField ID="hidDataRegistrazione" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "DataRegistrazione")%>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="PazienteInfo" HeaderText="Paziente <img id='imgPaz' alt='' src='../../images/transparent16.gif' />"
                            SortExpression="PazienteInfo">
                            <HeaderStyle Width="32%" HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundColumn>
                        <asp:TemplateColumn HeaderText="Data Visita <img id='imgDat' alt='' src='../../images/transparent16.gif' />" 
                            SortExpression="DataVisita">
                            <HeaderStyle Width="10%" HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                            <ItemTemplate>
                                <asp:Label ID="lblDataVisita" CssClass="label_left" runat="server"
                                    Text='<%# CDate(DataBinder.Eval(Container.DataItem, "DataVisita")).Date.ToString("dd/MM/yyyy")%>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="UtenteVisita" HeaderText="Utente Registrazione<img id='imgUte' alt='' src='../../images/transparent16.gif' />"
                            SortExpression="UtenteVisita">
                            <HeaderStyle Width="14%" HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="UtenteFirma" HeaderText="Utente Firma <img id='imgFir' alt='' src='../../images/transparent16.gif' />"
                            SortExpression="UtenteFirma">
                            <HeaderStyle Width="14%" HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="UtenteRilevatore" HeaderText="Rilevatore <img id='imgRil' alt='' src='../../images/transparent16.gif' />"
                            SortExpression="UtenteRilevatore">
                            <HeaderStyle Width="22%" HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundColumn>
                        <asp:TemplateColumn HeaderText="Stato">
                            <HeaderStyle Width="6%" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            <ItemTemplate>
                                <asp:ImageButton ID="btnFlagFirma" runat="server" CommandName="InfoArchiviazione"
                                    ImageUrl="<%# BindFlagFirmaImageUrlValue(Container.DataItem)%>"
                                    ToolTip="<%# BindFlagFirmaToolTipValue(Container.DataItem)%>" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>

            </dyp:DynamicPanel>

            <on_ofm:onitfinestramodale id="fmInfoArchiviazione" title="Info Archiviazione" runat="server" width="500px" Height="160px" BackColor="WhiteSmoke">
                    
                <uc3:FirmaDigitaleInfo ID="ucInfoFirma" runat="server" />

            </on_ofm:onitfinestramodale>
            <applet id="OnJSign" style="width:1px; height:1px;" archive="../../Applet/OnJSignature/1.1.0.0/OnJSignature.jar" code="it.onit.OnJSign.class" width="100" height="100"/>
            
        </on_lay3:OnitLayout3>
    </form>
</body>
</html>
