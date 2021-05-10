<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="InsVacEsc.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.InsVacEsc" %>

<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>

<link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
<link href='<%= ResolveClientUrl("~/css/button.css") %>' type="text/css" rel="stylesheet" />
<link href='<%= ResolveClientUrl("~/css/default/button.default.css") %>' type="text/css" rel="stylesheet" />

<script type="text/javascript" src="<%= Page.ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
<script type="text/javascript">

    var scrollClientID = '<%= scroll.ClientID %>';
    var ModaleName = '<%= ModaleName %>';

    $(document).ready(function () {
        document.getElementById('divScroll').scrollTop = document.getElementById(scrollClientID).value;
    });

    function SaveScroll(value) {
        document.getElementById(scrollClientID).value = value;
    }

    function InizializzaToolBar_InsVacEsc(t) {
        t.PostBackButton = false;
    }

    function ToolBarClick_InsVacEsc(toolBar, button, evnt) {
        evnt.needPostBack = true;
        switch (button.Key) {
            case 'btn_Annulla':
                closeFm(ModaleName);
                evnt.needPostBack = false;
                break;
        }
    }
</script>

<style type="text/css">
    .mt {
        height: 18px;
        width: 40px;
        margin-top: 4px;
        text-decoration: none;
    }

    .stileDatiBold {
        font-family: Arial;
        font-size: 12px;
        font-weight: bold;
    }
</style>

<table cellpadding="0" cellspacing="0" border="0" width="100%" height="580">
    <tr style="height: 30px">
        <td>
            <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                <ClientSideEvents InitializeToolbar="InizializzaToolBar_InsVacEsc" Click="ToolBarClick_InsVacEsc"></ClientSideEvents>
                <Items>
                    <igtbar:TBarButton Key="btn_Conferma" DisabledImage="~/Images/conferma_dis.gif"
                        Text="Conferma" Image="~/Images/conferma.gif" />
                    <igtbar:TBarButton Key="btn_Annulla" DisabledImage="~/Images/annullaconf_dis.gif"
                        Text="Annulla" Image="~/Images/annullaconf.gif" />
                </Items>
            </igtbar:UltraWebToolbar>
        </td>
    </tr>
    <tr style="height: 550px">
        <td>
            <input type="hidden" id="scroll" runat="server" />
            <div id="divScroll" style="overflow: auto; width: 100%; height: 550px" onscroll="SaveScroll(this.scrollTop)">
                <asp:DataList ID="dtlVacEsc" runat="server" Width="100%">
                    <HeaderStyle CssClass="header"></HeaderStyle>
                    <ItemStyle CssClass="item"></ItemStyle>
                    <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
                    <SelectedItemStyle Font-Bold="True" CssClass="selected"></SelectedItemStyle>
                    <HeaderTemplate>
                        <div>Selezionare le vaccinazioni da escludere</div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <table id="itemTable" cellpadding="0" cellspacing="0" border="0" style="width: 100%">
                            <tr>
                                <td>
                                    <table style="width: 100%; table-layout: fixed;">
                                        <colgroup>
                                            <col style="width: 3%" />
                                            <col style="width: 5%" />
                                            <col style="width: 8%" />
                                            <col style="width: 15%" />
                                            <col style="width: 6%" />
                                            <col style="width: 5%" />
                                            <col style="width: 7%" />
                                            <col style="width: 13%" />
                                            <col style="width: 2%" />
                                            <col style="width: 5%" />
                                            <col style="width: 31%" />
                                        </colgroup>
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="chkVaccinazione" runat="server" />
                                            </td>
                                            <td colspan="3">
                                                <asp:Label ID="lblDescrizione" runat="server" CssClass="stileDatiBold"
                                                    Text='<%# String.Format("{0} ({1})", Eval("Descrizione"), Eval("Codice"))%>'></asp:Label>
                                                <asp:Label ID="lblVacCodice" runat="server" Text='<%# Eval("Codice")%>' Style="display: none;"></asp:Label>
                                                <asp:Label ID="lblVacDescrizione" runat="server" Text='<%# Eval("Descrizione")%>' Style="display: none;"></asp:Label>
                                            </td>
                                            <td style="text-align: right">
                                                <asp:Label ID="lblDose" runat="server" CssClass="label" Text="Dose"></asp:Label>
                                            </td>
                                            <td>
                                                <on_val:OnitJsValidator ID="txtDose" runat="server" CssClass="textbox_numerico_obbligatorio" Width="100%"
                                                    actionCorrect="False" actionDelete="False" actionFocus="True" actionMessage="True" actionSelect="False"
                                                    actionUndo="True" autoFormat="True" validationType="Validate_custom" CustomValFunction-Length="2" CustomValFunction="validaNumero"
                                                    SetOnChange="True" MaxLength="2"></on_val:OnitJsValidator>
                                            </td>
                                            <td style="text-align: right;">
                                                <asp:Label ID="lblVisita" runat="server" CssClass="label" Text="Visita"></asp:Label>
                                            </td>
                                            <td colspan="2">
                                                <on_val:OnitDatePick ID="dpkDataVisita" runat="server" CssClass="textbox_data_obbligatorio" DateBox="True" Width="100%" />
                                            </td>
                                            <td style="text-align: right;">
                                                <asp:Label ID="lblMotivo" runat="server" CssClass="label" Text="Motivo"></asp:Label>
                                            </td>
                                            <td>
                                                <on_ofm:OnitModalList ID="fmMotivo" runat="server" Width="79%" UseCode="true" CodiceWidth="20%"
                                                    LabelWidth="-1px" PosizionamentoFacile="False" Obbligatorio="True" SetUpperCase="True" CssClass="textbox_stringa_obbligatorio"
                                                    Tabella="t_ana_motivi_esclusione" Filtro="moe_obsoleto = 'N' order by moe_descrizione ASC"
                                                    CampoDescrizione="moe_descrizione Descrizione_Motivo_Rifiuto" CampoCodice="moe_codice Codice_Motivo_RIfiuto"
                                                    RaiseChangeEvent="True" OnChange="fmMotivo_Change" />
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td>
                                                <asp:ImageButton ID="btnCopia" runat="server" ImageUrl="~/Images/duplica.gif" OnClick="btnCopia_Click" 
                                                    AlternateText="Replica" ToolTip="Replica i dati di esclusione per tutte le vaccinazioni selezionate" />
                                            </td>
                                            <td style="text-align: right">
                                                <asp:Label ID="lblScadenza" runat="server" CssClass="label" Text="Scadenza"></asp:Label>
                                            </td>
                                            <td>
                                                <on_val:OnitDatePick ID="dpkDataScadenza" runat="server" CssClass="TextBox_Data" DateBox="True" Width="100%"></on_val:OnitDatePick>
                                            </td>
                                            <td style="text-align: right">
                                                <asp:Label ID="lblMedico" runat="server" CssClass="label" Text="Medico"></asp:Label>
                                            </td>
                                            <td colspan="3">
                                                <on_ofm:OnitModalList ID="fm_medico" runat="server" Width="79%" UseCode="true" CodiceWidth="20%" LabelWidth="-1px" Obbligatorio="False"
                                                    PosizionamentoFacile="False" SetUpperCase="True" RaiseChangeEvent="False" CampoDescrizione="ope_nome" CampoCodice="ope_codice"
                                                    Tabella="t_ana_operatori" Filtro="(OPE_OBSOLETO IS NULL OR OPE_OBSOLETO='N') order by ope_nome ASC" />
                                            </td>
                                            <td colspan="2" style="text-align: right">
                                                <asp:Label ID="lblNote" runat="server" CssClass="label" Text="Note"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtNote" runat="server" CssClass="TextBox_Stringa" MaxLength="1000" Width="100%"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </ItemTemplate>
                </asp:DataList>
            </div>
        </td>
    </tr>
</table>
