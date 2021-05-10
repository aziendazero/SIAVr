<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="RicercaBilancio.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.RicercaBilancio" %>

<!-- Utilizza il foglio di stile Style_OnVac.css -->
<igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
    <DefaultStyle CssClass="infratoolbar_button_default">
    </DefaultStyle>
    <HoverStyle CssClass="infratoolbar_button_hover">
    </HoverStyle>
    <SelectedStyle CssClass="infratoolbar_button_selected">
    </SelectedStyle>
    <ClientSideEvents InitializeToolbar="RicercaBilancio_InizializzaToolBar" Click="RicercaBilancio_ToolBarClick">
    </ClientSideEvents>
    <Items>
        <igtbar:TBarButton Key="btnConferma" DisabledImage="~/Images/conferma.gif"
            Text="Conferma" Image="~/Images/conferma.gif">
        </igtbar:TBarButton>
        <igtbar:TBarButton Key="btnAnnulla" DisabledImage="~/Images/annulla.gif"
            Text="Annulla" Image="~/Images/annulla.gif">
        </igtbar:TBarButton>
    </Items>
</igtbar:UltraWebToolbar>
<div class="Label">
    <div style="width: 100%; margin: 15px; text-align:left;">
        <table id="Table1" height="30" cellspacing="1" cellpadding="1" width="550px" border="0" >
            <tr>
                <td width="50px" class="label">
                    Malattia:
                </td>
                <td>
                    <asp:DropDownList ID="cmbMalattie" runat="server" Height="24px" Width="100%" AutoPostBack="True"
                        DataTextField="DescrizioneMalattia" DataValueField="CodiceMalattia">
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
    </div>
</div>
<div style="height: 400px; overflow: auto">
    <asp:DataGrid ID="dgrBilanci" runat="server" Width="100%" CssClass="DataGrid" AutoGenerateColumns="False" >
        <SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
        <EditItemStyle CssClass="Edit"></EditItemStyle>
        <AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
        <ItemStyle CssClass="Item"></ItemStyle>
        <HeaderStyle CssClass="Header"></HeaderStyle>
        <Columns>
            <asp:TemplateColumn>
                <HeaderStyle Width="1%"></HeaderStyle>
                <ItemTemplate>
                    <asp:ImageButton ID="btnSelect" runat="server" ImageUrl='<%# UrlIconaSelezione %>' CommandName="Select"></asp:ImageButton>
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:BoundColumn DataField="NumeroBilancio" HeaderText="Numero" >
                <HeaderStyle Width="10%"></HeaderStyle>
            </asp:BoundColumn>
            <asp:BoundColumn DataField="DescrizioneBilancio" HeaderText="Descrizione">
                <HeaderStyle Width="35%"></HeaderStyle>
            </asp:BoundColumn>
            <asp:TemplateColumn HeaderText="Et&#224; Minima">
                <HeaderStyle Width="27%" />
                <ItemTemplate>
                    <asp:Label ID="lblEtaMinima" runat="server" Text='<%# GetStringEta(Eval("EtaMinima"))%>' >
                    </asp:Label>
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn HeaderText="Et&#224; Massima">
                <HeaderStyle Width="27%" />
                <ItemTemplate>
                    <asp:Label ID="lblEtaMassima" runat="server" Text='<%# GetStringEta(Eval("EtaMassima"))%>' >
                    </asp:Label>
                </ItemTemplate>
            </asp:TemplateColumn>
        </Columns>
    </asp:DataGrid>
</div>
<script type="text/javascript">
    function RicercaBilancio_InizializzaToolBar(t) {
        t.PostBackButton = false;
    }

    function RicercaBilancio_ToolBarClick(ToolBar, button, evnt) {
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
