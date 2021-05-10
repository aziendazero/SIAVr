<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ElencoVaccinazioniEseguite.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.ElencoVaccinazioniEseguite" %>

<%@ Register TagPrefix="onitFM" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>

<script type="text/javascript" src="<%= Page.ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
<script type="text/javascript">

    var scrollVacEsegClientID = '<%= hidScrollVacEseg.ClientID %>';

    $(document).ready(function () {
        var hidScroll = document.getElementById(scrollVacEsegClientID);
        if (hidScroll != null) {
            var divScroll = document.getElementById('divScrollVacEseg');
            if (divScroll != null) {
                divScroll.scrollTop = hidScroll.value;
            }
        }
    });

    function SaveScroll(scrollValue) {
        var hidScroll = document.getElementById(scrollVacEsegClientID);
        if (hidScroll != null) {
            hidScroll.value = scrollValue;
        }
    }

    function InizializzaToolBar_vacEscl(t) {
        t.PostBackButton = false;
    }

    function ToolBarClick_vacEscl(ToolBar, button, evnt) {
        evnt.needPostBack = true;
        switch (button.Key) {
            case 'btn_Annulla':
                if (!confirm('Annullare le modifiche?')) {
                    evnt.needPostBack = false;
                }
                break;
                break;
            default:
                evnt.needPostBack = true;
        }
    }

    function controllaSeNumero(obj) {
        if (isNaN(obj.value) || obj.value <= 0 || obj.value == "") {
            alert("Il numero di dose deve essere un valore intero positivo");
            obj.value = "";
            obj.focus();
        }

    }

</script>

<link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

<div style="width: 100%">
    <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="Toolbar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
        <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
        <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
        <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
        <ClientSideEvents InitializeToolbar="InizializzaToolBar_vacEscl" Click="ToolBarClick_vacEscl"></ClientSideEvents>
        <Items>
            <igtbar:TBarButton Key="btn_Modifica" DisabledImage="~/Images/Modifica_dis.gif"
                Text="Modifica" Image="~/Images/Modifica.gif">
            </igtbar:TBarButton>
        </Items>
        <Items>
            <igtbar:TBarButton Key="btn_Salva" Enabled="false" DisabledImage="~/Images/salva_dis.gif"
                Text="Salva" Image="~/Images/salva.gif">
            </igtbar:TBarButton>
        </Items>
        <Items>
            <igtbar:TBarButton Key="btn_Annulla" DisabledImage="~/Images/annulla_dis.gif"
                Text="Annulla" Image="~/Images/annulla.gif">
            </igtbar:TBarButton>
        </Items>
    </igtbar:UltraWebToolbar>
            
    <%--<asp:Panel ID="PanelLayoutTitolo_sezione" runat="server" CssClass="vac-sezione" Width="100%">
        <asp:Label ID="LayoutTitolo_sezioneCnv" runat="server">ELENCO VACCINAZIONI</asp:Label>
    </asp:Panel>--%>

    <div id="divLegenda" class="legenda-vaccinazioni" style="padding-bottom: 6px;">
        <span class="legenda-vaccinazioni-reazione">R</span>
        <span>Reazione avversa</span>
        <span class="legenda-vaccinazioni-scaduta">S</span>
        <span>Vaccinazione scaduta</span>
    </div>
     
    <input type="hidden" id="hidScrollVacEseg" runat="server" />
    <div id="divScrollVacEseg" style="overflow: auto; width: 100%; height: 250px" onscroll="SaveScroll(this.scrollTop)">
        <asp:DataGrid ID="dg_vacEff" runat="server" CssClass="datagrid" Width="100%" GridLines="None"
            CellPadding="3" AutoGenerateColumns="False" AllowSorting="True">
            <HeaderStyle Font-Bold="True" CssClass="header"></HeaderStyle>
            <PagerStyle CssClass="pager" Mode="NumericPages"></PagerStyle>
            <SelectedItemStyle Font-Bold="True" CssClass="selected"></SelectedItemStyle>
            <EditItemStyle CssClass="edit"></EditItemStyle>
            <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
            <ItemStyle CssClass="item"></ItemStyle>
            <Columns>
                <asp:TemplateColumn SortExpression="vac_descrizione" HeaderText="Descrizione &lt;IMG id=&quot;ordDesc&quot; alt=&quot;&quot; src=&quot;&quot;&gt;">
                    <HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
                    <ItemTemplate>
                        <asp:Label ID="lblVacDesc" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("vac_descrizione") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn SortExpression="ves_vac_codice" HeaderText="Codice &lt;IMG id=&quot;ordCod&quot; alt=&quot;&quot; src=&quot;&quot;&gt;">
                    <HeaderStyle HorizontalAlign="Center" Width="9%"></HeaderStyle>
                    <ItemTemplate>
                        <asp:Label ID="lblVacCodice" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("ves_vac_codice") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn SortExpression="ves_n_richiamo" HeaderText="Dosi &lt;IMG id=&quot;ordDosi&quot; alt=&quot;&quot; src=&quot;&quot;&gt;">
                    <HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                    <ItemTemplate>
                        <asp:TextBox ID="txt_dose" MaxLength="2" onblur="controllaSeNumero(this)" runat="server"
                            Width="100%" ReadOnly="True" CssClass="textboxVacEs_noEdit" Text='<%# DataBinder.Eval(Container, "DataItem")("ves_n_richiamo") %>'>
                        </asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn SortExpression="ves_mal_codice_cond_sanitaria" HeaderText="Cond.&lt;br&gt;Sanit. &lt;IMG id=&quot;ordCondSan&quot; alt=&quot;&quot; src=&quot;&quot;&gt;">
                    <HeaderStyle HorizontalAlign="Center" Width="8%" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                            <asp:Label ID="lblCondSanitaria" runat="server" CssClass="label">
                        </asp:Label>
                        <onitFM:OnitModalList ID="omlCondSanitaria" runat="server" Width="0%" LabelWidth="-8px" SetUpperCase="True"
                            PosizionamentoFacile="False" Obbligatorio="False" Visible="false" CssClass="textboxVacEs_edit"
                            OnSetUpFiletr="omlCondSanitaria_SetUpFiletr" OnChange="omlCondSanitaria_Change"
                            AltriCampi="VCS_PAZ_MAL_CODICE Paziente, VCS_COND_SANITARIA_DEFAULT Is_Default"
                            Tabella="V_CONDIZIONI_SANITARIE" CampoCodice="VCS_MAL_CODICE Codice" CampoDescrizione="VCS_MAL_DESCRIZIONE Descrizione"
                            RaiseChangeEvent="True" CodiceWidth="99%" UseCode="True" IsDistinct="true" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn SortExpression="ves_rsc_codice" HeaderText="Cond.&lt;br&gt;Rischio &lt;IMG id=&quot;ordCondRischio&quot; alt=&quot;&quot; src=&quot;&quot;&gt;">
                    <HeaderStyle HorizontalAlign="Center" Width="8%" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>                              
                        <asp:Label ID="lblCondRischio" runat="server" CssClass="label">
                        </asp:Label>
                        <onitFM:OnitModalList ID="omlCondRischio" runat="server" Width="0%" LabelWidth="-8px" SetUpperCase="True"
                            PosizionamentoFacile="False" Obbligatorio="False" Visible="false" CssClass="textboxVacEs_edit"
                            OnSetUpFiletr="omlCondRischio_SetUpFiletr" OnChange="omlCondRischio_Change"
                            AltriCampi="VCR_PAZ_RSC_CODICE Paziente, VCR_RISCHIO_DEFAULT Is_Default"
                            Tabella="V_CONDIZIONI_RISCHIO" CampoCodice="VCR_CODICE_RISCHIO Codice" CampoDescrizione="VCR_DESCRIZIONE_RISCHIO Descrizione"
                            RaiseChangeEvent="True" CodiceWidth="99%" UseCode="True" IsDistinct="true" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn SortExpression="ves_data_effettuazione" HeaderText="Data &lt;IMG id=&quot;ordData&quot; alt=&quot;&quot; src=&quot;&quot;&gt;">
                    <HeaderStyle HorizontalAlign="Center" Width="9%"></HeaderStyle>
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                    <ItemTemplate>
                        <asp:Label ID="lblDataEffettuazione" runat="server" CssClass="label" Text='<%# ctype(DataBinder.Eval(Container, "DataItem")("ves_data_effettuazione"),datetime).tostring("dd/MM/yyyy") %>' />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn SortExpression="ass_descrizione" HeaderText="Assoc. &lt;IMG id=&quot;ordAss&quot; alt=&quot;&quot; src=&quot;&quot;&gt;">
                    <HeaderStyle Width="10%"></HeaderStyle>
                    <ItemTemplate>
                        <asp:Label ID="tb_ass_desc" runat="server" Text='<%# String.Format("{0} [{1}]", DataBinder.Eval(Container, "DataItem")("ass_descrizione"), DataBinder.Eval(Container, "DataItem")("ves_ass_n_dose"))%>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn SortExpression="ves_lot_codice" HeaderText="Lotto &lt;IMG id=&quot;ordLotto&quot; alt=&quot;&quot; src=&quot;&quot;&gt;">
                    <HeaderStyle Width="9%"></HeaderStyle>
                    <ItemTemplate>
                        <asp:Label ID="Label7" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("ves_lot_codice") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn SortExpression="noc_descrizione" HeaderText="Nome&lt;br&gt;Commerciale &lt;IMG id=&quot;ordNC&quot; alt=&quot;&quot; src=&quot;&quot;&gt;">
                    <HeaderStyle HorizontalAlign="Center" Width="9%"></HeaderStyle>
                    <ItemTemplate>
                        <asp:Label ID="Label8" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("noc_descrizione") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn SortExpression="ope_nome" HeaderText="Medico &lt;IMG id=&quot;ordOp&quot; alt=&quot;&quot; src=&quot;&quot;&gt;">
                    <HeaderStyle HorizontalAlign="Center" Width="9%"></HeaderStyle>
                    <ItemTemplate>
                        <asp:Label ID="Label6" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("ope_nome") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn SortExpression="cns_descrizione" HeaderText="Centro&lt;br&gt;Vaccinale &lt;IMG id=&quot;ordCNS&quot; alt=&quot;&quot; src=&quot;&quot;&gt;">
                    <HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
                    <ItemTemplate>
                        <asp:Label ID="Label9" runat="server" CssClass="label" Text='<%# DataBinder.Eval(Container, "DataItem")("cns_descrizione") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn>
                    <HeaderStyle HorizontalAlign="Center" Width="2%"></HeaderStyle>
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" CssClass="legenda-vaccinazioni-reazione" ToolTip="Reazione avversa"
                            Visible='<%# If(Eval("vra_data_reazione") Is DBNull.Value, False, True) %>'>R</asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn>
                    <HeaderStyle HorizontalAlign="Center" Width="2%"></HeaderStyle>
                    <ItemTemplate>
                        <asp:Label ID="Label11" runat="server" CssClass="legenda-vaccinazioni-scaduta" ToolTip="Associazione scaduta"
                            Visible='<%# If(Eval("scaduta").ToString() = "S", True, False) %>'>S</asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
        </asp:DataGrid>
    </div>
</div>

<onitFM:OnitFinestraModale CssClass="messaggiRiepilogo" ID="modRiepilogo" Title="Vaccinazioni non modificabili" runat="server" Width="380px" BackColor="LightGray" NoRenderX="false">

    <asp:Label runat="server" ID="lblMessaggioAggiornamentoCentrale" CssClass="messaggioAggiornamentoCentrale" Width="100%" Visible="false"></asp:Label>
    <asp:Label runat="server" ID="lblIntestazioneRiepilogo" CssClass="intestazioneErrori" Width="100%"></asp:Label>
    <asp:Label runat="server" ID="lblMessaggioRiepilogo" CssClass="messaggioRiepilogo" Width="100%"></asp:Label>

</onitFM:OnitFinestraModale>

<style type="text/css">
    .messaggioAggiornamentoCentrale {
        font-family: Verdana;
        font-size: 12px;
        font-weight: bold;
        text-align: center;
        border: 1px solid navy;
        background-color: #ffffcc;
        margin: 5px;
        padding: 5px;
    }

    .intestazioneErrori {
        font-family: Verdana;
        font-size: 12px;
        font-weight: bold;
        text-align: center;
        margin-top: 5px;
        margin-bottom: 5px;
    }

    .messaggioRiepilogo {
        font-family: Verdana;
        font-size: 12px;
    }
</style>

<script type="text/javascript">
    document.getElementById("ordOp").style.display = "none"
    document.getElementById("ordCod").style.display = "none"
    document.getElementById("ordDosi").style.display = "none"
    document.getElementById("ordData").style.display = "none"
    document.getElementById("ordDesc").style.display = "none"
    document.getElementById("ordAss").style.display = "none"
    document.getElementById("ordLotto").style.display = "none"
    document.getElementById("ordNC").style.display = "none"
    document.getElementById("ordCNS").style.display = "none"
    document.getElementById("ordCondSan").style.display = "none"
    document.getElementById("ordCondRischio").style.display = "none"

    <%Response.Write(strJS)%>
</script>

