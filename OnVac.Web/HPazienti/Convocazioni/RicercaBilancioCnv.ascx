<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="RicercaBilancioCnv.ascx.vb"
    Inherits="Onit.OnAssistnet.OnVac.RicercaBilancioCnv" %>
<!-- Utilizza il foglio di stile Style_OnVac.css -->
<igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
    <DefaultStyle CssClass="infratoolbar_button_default" />
    <HoverStyle CssClass="infratoolbar_button_hover" />
    <SelectedStyle CssClass="infratoolbar_button_selected" />
    <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
    <Items>
        <igtbar:TBarButton Key="btnConferma" DisabledImage="~/Images/conferma.gif"
            Text="Compila" Image="~/Images/conferma.gif">
        </igtbar:TBarButton>
        <igtbar:TBarButton Key="btnAnnulla" DisabledImage="~/Images/annulla.gif"
            Text="Visualizza vaccinazioni" Image="~/Images/annulla.gif">
            <DefaultStyle CssClass="infratoolbar_button_default" Width="160px" />
        </igtbar:TBarButton>
    </Items>
</igtbar:UltraWebToolbar>
<p class="label" style="margin: 10px; font-weight: bold; text-align: center; font-size: 14px">
    E' prevista la compilazione dei seguenti questionari.<br />
    Procedere?
</p>
<div style="height: 400px; overflow: auto">
    <asp:GridView ID="grvBilanci" runat="server" Width="100%" CssClass="DataGrid" AutoGenerateColumns="False"
        DataKeyNames="N_BILANCIO,MAL_CODICE">
        <SelectedRowStyle CssClass="Selected" />
        <EditRowStyle CssClass="Edit" />
        <AlternatingRowStyle CssClass="Alternating" />
        <RowStyle CssClass="Item" />
        <HeaderStyle CssClass="Header" />
        <Columns>
            <asp:CommandField ItemStyle-Width="2%" ItemStyle-HorizontalAlign="Center" ShowSelectButton="true" ButtonType="Image" SelectImageUrl="~/Images/seleziona.gif" />
            <asp:BoundField DataField="N_BILANCIO" HeaderText="Numero">
                <HeaderStyle Width="10%" HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="BIL_DESCRIZIONE" HeaderText="Descrizione">
                <HeaderStyle Width="30%" HorizontalAlign="Left" />
                <ItemStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Et&#224; Minima">
                <HeaderStyle Width="28%" HorizontalAlign="Left" />
                <ItemStyle HorizontalAlign="Left" />
                <ItemTemplate>
                    <asp:Label ID="lblEtàMinima" runat="server" Text="" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Et&#224; Massima">
                <HeaderStyle Width="28%" HorizontalAlign="Left" />
                <ItemStyle HorizontalAlign="Left" />
                <ItemTemplate>
                    <asp:Label ID="lblEtàMassima" runat="server" Text="" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="">
                <HeaderStyle Width="2%" HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
                <ItemTemplate>
                    <asp:Image ID="imgObbligatorio" runat="server" Visible="false" ImageUrl="~/Images/avvertimento.gif" AlternateText="Compilazione obbligatoria" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</div>
<script type="text/javascript">
    function RicercaBilancioCnv_InizializzaToolBar(t) {
        t.PostBackButton = false;
    }

    function RicercaBilancioCnv_ToolBarClick(ToolBar, button, evnt) {
        evnt.needPostBack = true;
        //alert(button.Key);
        switch (button.Key) {

            case 'btnConferma':
                //returnValue=document.all('txtRitorno').value; MARCO : DA GESTIRE
                closeFm('<%= ModaleName %>');
                break;

            case 'btnAnnulla':
                closeFm('<%= ModaleName %>');
                break;

            default:
                evnt.needPostBack = true;
        }
    }
</script>
