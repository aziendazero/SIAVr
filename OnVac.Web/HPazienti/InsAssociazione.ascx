<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="InsAssociazione.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.InsAssociazione" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>

<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>

<script type="text/javascript">

    function InizializzaToolBar_Ass(t) {
        t.PostBackButton = false;
    }

    function ToolBarClick_Ass(ToolBar, button, evnt) {
        evnt.needPostBack = true;
        switch (button.Key) {
            case 'btn_Conferma':
                var almenoUnaSelezionata = false

                for (i = 1; i < document.getElementById('<% = dg_ass.clientid %>').rows.length; i++) {
                    if (document.getElementById('<% = dg_ass.clientid %>').rows[i].cells[0].getElementsByTagName("input")[0].checked == true) {
                        almenoUnaSelezionata = true
                    }
                }
                if (almenoUnaSelezionata == false) {
                    alert("Selezionare almeno una associazione.");
                    evnt.needPostBack = false;
                    break;
                }

                evnt.needPostBack = true;
                break;

            default:
                evnt.needPostBack = true;
        }
    }

</script>

<link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
<style type="text/css">
    .legenda {
        border-bottom: #485d96 1px solid
    }
</style>

<on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" WindowNoFrames="true">
    <table height="400" cellspacing="0" cellpadding="0" width="610" border="0">
        <tr height="50">
            <td>
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                    <ClientSideEvents InitializeToolbar="InizializzaToolBar_Ass" Click="ToolBarClick_Ass"></ClientSideEvents>
                    <Items>
                        <igtbar:TBarButton Key="btn_Conferma" DisabledImage="~/Images/conferma_dis.gif" Text="Conferma"
                            Image="~/Images/conferma.gif">
                        </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btn_Annulla" DisabledImage="~/Images/annullaconf_dis.gif" Text="Annulla"
                            Image="~/Images/annullaconf.gif">
                        </igtbar:TBarButton>
                    </Items>
                </igtbar:UltraWebToolbar>
                <asp:Panel ID="Panel23" runat="server" CssClass="vac-sezione">
                    <asp:Label ID="LayoutTitolo_sezione" runat="server">ELENCO ASSOCIAZIONI</asp:Label>
                </asp:Panel>
            </td>
        </tr>
        <tr height="350">
            <td>
                <div style="overflow: auto; width: 610px; height: 350px">
                    <asp:DataGrid ID="dg_ass" runat="server" Width="100%" GridLines="None" CellPadding="1" AutoGenerateColumns="False">
                        <ItemStyle CssClass="item"></ItemStyle>
                        <SelectedItemStyle Font-Bold="True" CssClass="selected"></SelectedItemStyle>
                        <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
                        <HeaderStyle CssClass="header"></HeaderStyle>
                        <PagerStyle CssClass="pager" Mode="NumericPages"></PagerStyle>
                        <Columns>
                            <asp:TemplateColumn>
                                <HeaderStyle Width="1%"></HeaderStyle>
                                <ItemTemplate>
                                    <asp:CheckBox ID="cb" runat="server"></asp:CheckBox>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:BoundColumn DataField="ass_descrizione" HeaderText="Descrizione">
                                <HeaderStyle HorizontalAlign="Left" Width="30%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Left"></ItemStyle>
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="ass_codice" HeaderText="Codice">
                                <HeaderStyle HorizontalAlign="Left" Width="19%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Left"></ItemStyle>
                            </asp:BoundColumn>
                            <asp:TemplateColumn HeaderText="Via Somministrazione">
                                <HeaderStyle HorizontalAlign="Left" Width="25%"></HeaderStyle>
                                <ItemTemplate>
                                    <on_ofm:OnitModalList ID="fmVia" runat="server" Width="100%" Tabella="t_ana_vie_somministrazione" CodiceWidth="0px"
                                        CampoCodice="vii_codice CODICE" Codice='<%# DataBinder.Eval(Container, "DataItem")("ass_vii_codice")%>'
                                        CampoDescrizione="vii_descrizione DESCRIZIONE" Descrizione='<%# DataBinder.Eval(Container, "DataItem")("vii_descrizione") %>'
                                        RaiseChangeEvent="False" SetUpperCase="True" UseCode="True" Obbligatorio="False" LabelWidth="-1px"
                                        PosizionamentoFacile="False" Filtro=" 1=1 order by vii_descrizione"></on_ofm:OnitModalList>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Sito Inoculazione">
                                <HeaderStyle HorizontalAlign="Left" Width="25%"></HeaderStyle>
                                <ItemTemplate>
                                    <on_ofm:OnitModalList ID="fmSito" runat="server" Width="100%" Tabella="t_ana_siti_inoculazione" CodiceWidth="0px"
                                        CampoCodice="sii_codice CODICE" Codice='<%# DataBinder.Eval(Container, "DataItem")("ass_sii_codice")%>'
                                        CampoDescrizione="sii_descrizione DESCRIZIONE" Descrizione='<%# DataBinder.Eval(Container, "DataItem")("sii_descrizione") %>'
                                        RaiseChangeEvent="False" SetUpperCase="True" UseCode="True" Obbligatorio="False" LabelWidth="-1px"
                                        PosizionamentoFacile="False" Filtro=" 1=1 order by sii_descrizione"></on_ofm:OnitModalList>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:BoundColumn DataField="ass_vii_codice" Visible="false"></asp:BoundColumn>
                            <asp:BoundColumn DataField="ass_sii_codice" Visible="false"></asp:BoundColumn>
                        </Columns>
                    </asp:DataGrid>
                </div>
            </td>
        </tr>
    </table>
</on_lay3:OnitLayout3>

<script type="text/javascript">
    function DisAllCheck() {
        for (i = 1; i < document.getElementById('<% = dg_ass.clientid %>').rows.length; i++)
            document.getElementById('<% = dg_ass.clientid %>').rows[i].cells[0].getElementsByTagName("input")[0].enabled = false
    }
</script>
