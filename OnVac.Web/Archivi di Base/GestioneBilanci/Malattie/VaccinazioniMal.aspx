<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="VaccinazioniMal.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.VaccinazioniMal" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Assembly="Onit.Web.UI.WebControls.DynamicPanel" Namespace="Onit.Web.UI.WebControls.DynamicPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Vaccinazioni-Malattie</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript">
        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;

            switch (button.Key) {
                case 'btnIndietro':
                    if ("<%response.write(onitlayout31.busy)%>" == "True") {
                        var ret = confirm("Le modifiche effettuate andranno perse. Continuare?");
                        if (ret == true) {
                            window.location.href = 'Malattie.aspx';
                            evnt.needPostBack = false;
                        }
                    }
                    else
                        window.location.href = 'Malattie.aspx';
                    evnt.needPostBack = false;
                    break;

                case 'btnSalva':
                    if ("<%response.write(onitlayout31.busy)%>" != "True")
                        evnt.needPostBack = false;
                    break;
            }
        }

        function SelezionaTutti(chkValue) {
            __doPostBack('selectAll', chkValue);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">

        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" TitleCssClass="Title3" Titolo="Malattia - Vaccinazioni">
            <div class="title">
                <asp:Label ID="LayoutTitolo" runat="server" Width="100%"></asp:Label>
            </div>
            <div>
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolBar" runat="server" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                    <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
                    <Items>
                        <igtbar:TBarButton Key="btnIndietro" Text="Indietro" DisabledImage="~/Images/indietro_dis.gif" Image="~/Images/indietro.gif" />
                        <igtbar:TBSeparator />
                        <igtbar:TBarButton Key="btnSalva" Text="Salva" DisabledImage="~/Images/salva_dis.gif" Image="~/Images/salva.gif" />
                    </Items>
                </igtbar:UltraWebToolbar>
            </div>
            <div class="sezione">
                <asp:Label ID="Label1" runat="server">Elenco Vaccinazioni</asp:Label>
            </div>
            <dyp:DynamicPanel ID="dypVaccinazioni" runat="server" Width="100%" Height="100%" ScrollBars="Auto">
                <asp:DataGrid ID="dgrVaccinazioni" runat="server" Width="100%" AutoGenerateColumns="False"
                    AllowCustomPaging="false" AllowPaging="false" PageSize="25" AllowSorting="false" DataKeyField="CodiceVac">
                    <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
                    <ItemStyle CssClass="item"></ItemStyle>
                    <PagerStyle CssClass="pager" Position="TopAndBottom" Mode="NumericPages" />
                    <HeaderStyle CssClass="header"></HeaderStyle>
                    <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
                    <Columns>
                        <asp:TemplateColumn>
                            <HeaderStyle Width="1%" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <HeaderTemplate>
                                <input type="checkbox" id="chkSelezioneHeader" onclick="SelezionaTutti(this.checked);" title="Seleziona tutti" runat="server" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSelezioneItem" runat="server" AutoPostBack="true" OnCheckedChanged="chkSelezioneItem_CheckedChanged" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="CodiceVac" HeaderText="Codice">
                            <HeaderStyle Width="15%" HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="DescrizioneVac" HeaderText="Descrizione">
                            <HeaderStyle Width="15%" HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundColumn>
                    </Columns>
                </asp:DataGrid>

            </dyp:DynamicPanel>
        </on_lay3:OnitLayout3>


    </form>
</body>
</html>
