<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ModDataVacEsc.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.ModDataVacEsc" %>

<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>

<link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' rel="stylesheet" type="text/css" />
<link href='<%= ResolveClientUrl("~/css/button.css") %>' rel="stylesheet" type="text/css" />
<link href='<%= ResolveClientUrl("~/css/default/button.default.css") %>' rel="stylesheet" type="text/css" />

<script type="text/javascript" src="<%= Page.ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>

<script type="text/javascript">

    var ModaleName = '<%= ModaleName %>';

    function InizializzaToolBar_ModDataVacEsc(t) {
        t.PostBackButton = false;
    }

    function ToolBarClick_ModDataVacEsc(toolBar, button, evnt) {
        evnt.needPostBack = true;
        switch (button.Key) {
            case 'btn_Annulla':                
                evnt.needPostBack = true;
                break;
            //case 'btn_Annulla':
            //    closeFm(ModaleName);
            //    evnt.needPostBack = false;
            //    break;
        }
    }

</script>

<style type="text/css">
    .msg {
        color: darkblue;
        border: navy 1px solid;
        background-color: whitesmoke;
        font-family: verdana;
        font-size: 12px;
        margin: 10px 10px 0px 10px;
        padding: 10px 10px 10px 10px; 
    }

        .msg p {
            padding-bottom: 5px;
        }

    .msg-table {
        width: 100%;
        border-width: 0px;
        padding-bottom: 15px;
    }
</style>

<div>
    <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolBar_ModDataVacEsc" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
        <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
        <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
        <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
        <ClientSideEvents InitializeToolbar="InizializzaToolBar_ModDataVacEsc" Click="ToolBarClick_ModDataVacEsc"></ClientSideEvents>
        <Items>
            <igtbar:TBarButton Key="btn_Salva" DisabledImage="~/Images/salva_dis.gif"
                Text="Salva" Image="~/Images/salva.gif" />
            <igtbar:TBarButton Key="btn_Annulla" DisabledImage="~/Images/annullaconf_dis.gif"
                Text="Annulla" Image="~/Images/annullaconf.gif" />
        </Items>
    </igtbar:UltraWebToolbar>
</div>

<div class="msg">
    <p>Selezionare una data e premere “Salva” per impostarla come data di scadenza comune a tutte le esclusioni selezionate</p>
    <table class="msg-table">
        <tr>
            <td valign="bottom" align="center">
                <on_val:OnitDatePick ID="dpkDataScadenzaModificata" runat="server" CssClass="TextBox_Data" DateBox="True" Width="180px"></on_val:OnitDatePick>
            </td>
        </tr>
    </table>    
</div>


