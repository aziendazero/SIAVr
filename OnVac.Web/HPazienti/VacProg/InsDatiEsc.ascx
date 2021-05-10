<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="InsDatiEsc.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_InsDatiEsc" %>

<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>

<igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
    <ClientSideEvents InitializeToolbar="InizializzaToolBar_datiEsc" Click="ToolBarClick_datiEsc"></ClientSideEvents>
    <Items>
        <igtbar:TBarButton Key="btn_Conferma" DisabledImage="~/Images/conferma_dis.gif" Text="Conferma" Image="~/Images/conferma.gif"></igtbar:TBarButton>
        <igtbar:TBarButton Key="btn_Annulla" DisabledImage="~/Images/annullaconf_dis.gif" Text="Annulla" Image="~/Images/annullaconf.gif"></igtbar:TBarButton>
    </Items>
</igtbar:UltraWebToolbar>

<div style="background-color: whitesmoke; width: 100%; text-align: center;">

    <asp:Panel ID="PanelLayoutTitolo_sezione" runat="server" CssClass="vac-sezione" style="text-align: center;">
        <asp:Label ID="LayoutTitolo_sezione" runat="server"></asp:Label>
    </asp:Panel>

    <asp:Panel runat="server" ID="panelDatiIntestazione" CssClass="panelDati">

        <div class="label_center" style="font-weight: bold;">
            <asp:Label runat="server" ID="lblAvvisoEsclusa"></asp:Label>
        </div>

        <asp:Panel runat="server" ID="panelDatiEsclusione" Width="100%">
            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                <colgroup>
                    <col style="width: 25%" />
                    <col style="width: 25%" />
                    <col style="width: 8%" />
                    <col style="width: 7%" />
                    <col style="width: 15%" />
                    <col style="width: 20%" />
                </colgroup>
                <tr>
                    <td style="text-align: right">
                        <asp:Label runat="server" ID="Label6" CssClass="label">Data Visita:&nbsp;</asp:Label></td>
                    <td>
                        <asp:Label runat="server" ID="lblEsclusioneDataVisita" CssClass="labelPanelDati"></asp:Label></td>
                    <td style="text-align: right">
                        <asp:Label runat="server" ID="Label11" CssClass="label">Dose:&nbsp;</asp:Label></td>
                    <td>
                        <asp:Label runat="server" ID="lblEsclusioneDose" CssClass="labelPanelDati"></asp:Label></td>
                    <td style="text-align: right">
                        <asp:Label runat="server" ID="Label8" CssClass="label">Scadenza:&nbsp;</asp:Label></td>
                    <td>
                        <asp:Label runat="server" ID="lblEsclusioneDataScadenza" CssClass="labelPanelDati"></asp:Label></td>
                </tr>
                <tr>
                    <td style="text-align: right">
                        <asp:Label runat="server" ID="Label4" CssClass="label">Motivo:&nbsp;</asp:Label></td>
                    <td colspan="5">
                        <asp:Label runat="server" ID="lblEsclusioneMotivo" CssClass="labelPanelDati"></asp:Label></td>
                </tr>
                <tr>
                    <td style="text-align: right">
                        <asp:Label runat="server" ID="Label7" CssClass="label">Medico:&nbsp;</asp:Label></td>
                    <td colspan="5">
                        <asp:Label runat="server" ID="lblEsclusioneMedico" CssClass="labelPanelDati"></asp:Label>
                    </td>
                </tr>
            </table>
        </asp:Panel>

    </asp:Panel>

    <asp:Panel runat="server" ID="panelDatiInserimento" CssClass="panelInsert" Width="100%">
        <table width="100%">
            <colgroup>
                <col style="width: 25%" />
                <col style="width: 55%" />
                <col style="width: 10%" />
                <col style="width: 10%" />
            </colgroup>
            <tr>
                <td style="text-align: right">
                    <asp:Label ID="Label5" runat="server" Font-Names="Arial" CssClass="label">Data della visita</asp:Label></td>
                <td>
                    <on_val:OnitDatePick ID="tb_data_visita" runat="server" CssClass="textbox_data_obbligatorio w100" DateBox="True" target="tb_data_visita" Width="130px"></on_val:OnitDatePick>
                </td>
                <td style="text-align: right">
                    <asp:Label ID="Label9" runat="server" Font-Names="Arial" CssClass="label">Dose</asp:Label></td>
                <td>
                    <on_val:OnitJsValidator ID="txtDose" runat="server" CssClass="textbox_numerico_obbligatorio w100p"
                        actionCorrect="False" actionDelete="False" actionFocus="True" actionMessage="True" actionSelect="False"
                        actionUndo="True" autoFormat="True" validationType="Validate_custom" CustomValFunction-Length="2" CustomValFunction="validaNumero"
                        SetOnChange="True" MaxLength="2"></on_val:OnitJsValidator>
                </td>
            </tr>
            <tr>
                <td style="text-align: right">
                    <asp:Label ID="Label3" runat="server" Font-Names="Arial" CssClass="label">Motivo di esclusione</asp:Label></td>
                <td colspan="3">
                    <on_ofm:OnitModalList ID="fm_motivo" runat="server" Font-Names="Arial" Font-Size="12px" Width="69%" RaiseChangeEvent="True" LabelWidth="-1px" PosizionamentoFacile="False" Obbligatorio="True" SetUpperCase="True" UseCode="True" Tabella="t_ana_motivi_esclusione" Filtro="'true'='true' order by moe_descrizione ASC" CampoDescrizione="moe_descrizione" CampoCodice="moe_codice" CodiceWidth="30%"></on_ofm:OnitModalList>
                </td>
            </tr>
            <tr>
                <td style="text-align: right">
                    <asp:Label ID="Label2" runat="server" Font-Names="Arial" CssClass="label">Medico</asp:Label></td>
                <td colspan="3">
                    <on_ofm:OnitModalList ID="fm_medico" runat="server" Width="69%" RaiseChangeEvent="False" LabelWidth="-1px" PosizionamentoFacile="False" Obbligatorio="False" SetUpperCase="True" UseCode="True" Tabella="t_ana_operatori" Filtro="'true'='true' order by ope_nome ASC" CampoDescrizione="ope_nome" CampoCodice="ope_codice" CodiceWidth="30%"></on_ofm:OnitModalList>
                </td>
            </tr>
            <tr>
                <td style="text-align: right">
                    <asp:Label ID="Label1" runat="server" Font-Names="Arial" CssClass="label">Data di scadenza</asp:Label></td>
                <td colspan="3">
                    <on_val:OnitDatePick ID="tb_data_scadenza" runat="server" DateBox="True" target="tb_data_scadenza" CssClass="TextBox_Data w100" Width="130px"></on_val:OnitDatePick>
                </td>
            </tr>
            <tr>
                <td style="text-align: right; vertical-align: text-top;">
                    <asp:Label ID="Label10" runat="server" Font-Names="Arial" CssClass="label">Note</asp:Label></td>
                <td colspan="3">
                    <asp:TextBox ID="txtNote" runat="server" CssClass="TextBox_Stringa" MaxLength="1000" TextMode="MultiLine" Rows="3" Style="overflow-y: auto" Width="99%"></asp:TextBox>
                </td>
            </tr>
        </table>
    </asp:Panel>
</div>

<script type="text/javascript">

    var fm_motivo ='<%= fm_motivo.clientid%>';
    var txtDose = '<%= txtDose.ClientID%>';

    function InizializzaToolBar_datiEsc(t) {
        t.PostBackButton = false;
    }

    function ToolBarClick_datiEsc(ToolBar, button, evnt) {
        evnt.needPostBack = true;

        switch (button.Key) {
            case 'btn_Conferma':

                if (OnitDatePick.tb_data_visita[0].Get() == "") {
                    alert("La data della visita non può essere vuota.\nNon è possibile aggiornare la riga.");
                    OnitDatePick.tb_data_visita[0].Focus(1, true);
                    evnt.needPostBack = false;
                    break;
                }

                if (document.getElementById(fm_motivo).value == "") {
                    alert("Il motivo di esclusione è un dato obligatorio.\nNon è possibile aggiornare la riga.");
                    document.getElementById(fm_motivo).focus();
                    evnt.needPostBack = false;
                    break;
                }

                if (document.getElementById(txtDose).value == "") {
                    alert("La dose è un dato obligatorio.\nNon è possibile aggiornare la riga.");
                    document.getElementById(txtDose).focus();
                    evnt.needPostBack = false;
                    break;
                }

                dVArr = OnitDatePick.tb_data_visita[0].Get().split("/");

                if (new Date(dVArr[2], dVArr[1] - 1, dVArr[0]) > new Date()) {
                    alert("La data della visita non può essere futura.\nNon è possibile aggiornare la riga.");
                    OnitDatePick.tb_data_visita[0].Focus(1, true);
                    evnt.needPostBack = false;
                    break;
                }

                dSArr = OnitDatePick.tb_data_scadenza[0].Get().split("/");

                if (new Date(dSArr[2], dSArr[1] - 1, dSArr[0]) < new Date(dVArr[2], dVArr[1] - 1, dVArr[0])) {
                    alert("La data di scadenza non può essere precedente a quella della visita.\nNon è possibile aggiornare la riga.");
                    OnitDatePick.tb_data_scadenza[0].Focus(1, true);
                    evnt.needPostBack = false;
                    break;
                }

                if ((document.getElementById(fm_motivo).value != '') && (document.getElementById(fm_motivo).nextSibling.value == '')) {
                    alert('È necessario valorizzare correttamente il campo Motivo di Rifiuto!');
                    document.getElementById(fm_motivo).focus();
                    evnt.needPostBack = false;
                    break;
                }

                if ((document.getElementById(fm_medico).value != '') && (document.getElementById(fm_medico).nextSibling.value == '')) {
                    alert('È necessario valorizzare correttamente il campo Medico!');
                    document.getElementById(fm_medico).focus();
                    evnt.needPostBack = false;
                    break;
                }

                break;

        } //end switch
    } //end ToolBarClick	

</script>

<style type="text/css">
    .panelDati {
        border-left: 1px solid navy;
        border-right: 1px solid navy;
        padding-top: 5px;
        padding-bottom: 5px;
    }

    .panelInsert {
        padding-top: 5px;
        border-top: 1px solid navy;
        background-color: LightGray; /* whitesmoke; */
    }

    .redLabel {
        color: Red;
    }

    .blueLabel {
        color: Blue;
    }

    .labelPanelDati {
        font-family: Arial;
        font-size: 11px;
        text-align: left;
        font-weight: bold;
        color: black;
    }
</style>
