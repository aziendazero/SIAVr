<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Interventi.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.Interventi" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register Assembly="Onit.Web.UI.WebControls.DynamicPanel" Namespace="Onit.Web.UI.WebControls.DynamicPanel" TagPrefix="dyp" %>
<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Anagrafe Interventi</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript" language="javascript">
        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;
            switch (button.Key) {

                case 'btnElimina':
                    if (confirm("Proseguire con l'eliminazione?") == false) {
                        evnt.needPostBack = false;
                        return;
                    }
                    break;
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Height="100%" Width="100%" TitleCssClass="Title3" Titolo="Anagrafe Interventi">
            <div>
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="tlbInterventi" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                    <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
                    <Items>
                        <igtbar:TBarButton Key="btnCerca" Text="Cerca" DisabledImage="~/Images/cerca_dis.gif" Image="~/Images/cerca.gif">
                        </igtbar:TBarButton>
                        <igtbar:TBSeparator></igtbar:TBSeparator>
                        <igtbar:TBarButton Key="btnNuovo" Text="Nuovo" DisabledImage="~/Images/nuovo_dis.gif" Image="~/Images/nuovo.gif">
                        </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btnModifica" Text="Modifica" DisabledImage="~/Images/Modifica_dis.gif" Image="~/Images/Modifica.gif">
                        </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btnElimina" Text="Elimina" DisabledImage="~/Images/elimina_dis.gif" Image="~/Images/elimina.gif">
                        </igtbar:TBarButton>
                        <igtbar:TBSeparator></igtbar:TBSeparator>
                        <igtbar:TBarButton Key="btnSalva" Text="Salva" DisabledImage="~/Images/salva_dis.gif" Image="~/Images/salva.gif">
                        </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btnAnnulla" Text="Annulla" DisabledImage="~/Images/annulla_dis.gif" Image="~/Images/annulla.gif">
                        </igtbar:TBarButton>
                    </Items>
                </igtbar:UltraWebToolbar>
            </div>
            <div class="sezione">
                Modulo ricerca
            </div>
            <dyp:DynamicPanel ID="dyp1" runat="server" Width="100%" Height="50px" ExpandDirection="horizontal" BackColor="WhiteSmoke">
                <table cellspacing="10" cellpadding="0" width="100%" border="0" style="table-layout: fixed">
                    <tr>
                        <td align="right" width="90">
                            <asp:Label ID="Label1" runat="server" CssClass="LABEL">Filtro di Ricerca</asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="WzFilterKeyBase" runat="server" CssClass="textbox_stringa w100p" style="text-transform: uppercase;"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </dyp:DynamicPanel>
            <div class="sezione">
                Elenco
            </div>
            <dyp:DynamicPanel ID="dyp2" runat="server" Width="100%" Height="100%" ScrollBars="Auto" ExpandDirection="horizontal">
				<on_dgr:OnitGrid id="dgrInterventi" runat="server" Width="100%" CssClass="DataGrid" GridLines="Horizontal" AutoGenerateColumns="False" 
					SortedColumns="Matrice IGridColumn[]" SelectionOption="rowClick" CascadeStyles="True"
					AllowPaging="False" PagingMode="Auto" PageSize="100" PagerVoicesBefore="10" PagerVoicesAfter="10" PagerDropDownList="ddlPager" PagerGoToOption="False">
					<AlternatingItemStyle CssClass="Infra2Dgr_RowAlternate" />
					<ItemStyle CssClass="Infra2Dgr_Row" />
					<HeaderStyle CssClass="Infra2Dgr_Header" />
					<SelectedItemStyle CssClass="Infra2Dgr_SelectedRow" />
					<PagerStyle Visible="False" />
					<Columns>
						<on_dgr:OnitBoundColumn DataField="Codice" key="Codice" HeaderText="Codice" SortDirection="NoSort"  >
                            <HeaderStyle Width="20%" HorizontalAlign="Left" />
						</on_dgr:OnitBoundColumn>
                        <on_dgr:OnitBoundColumn DataField="Descrizione" key="Descrizione" HeaderText="Descrizione" SortDirection="NoSort">
                            <HeaderStyle Width="30%" HorizontalAlign="Left" />
                        </on_dgr:OnitBoundColumn>
						<on_dgr:OnitTemplateColumn SortDirection="NoSort" HeaderText="Tipologia">
							<HeaderStyle Width="25%" HorizontalAlign="Left"></HeaderStyle>
                            <ItemTemplate>
								<asp:Label ID="lblTipologia" runat="server"></asp:Label>
							</ItemTemplate>
						</on_dgr:OnitTemplateColumn>                        
                        <on_dgr:OnitBoundColumn DataField="Durata" key="Durata" HeaderText="Durata" SortDirection="NoSort">
                            <HeaderStyle Width="25%" HorizontalAlign="Left" />
                        </on_dgr:OnitBoundColumn>
					</Columns>
				</on_dgr:OnitGrid>
            </dyp:DynamicPanel>
            <dyp:DynamicPanel ID="dyp3" runat="server" Width="100%" Height="80px" ExpandDirection="horizontal">
                <div class="sezione">
                    Dettagli
                </div>
                <table border="0" cellpadding="2" cellspacing="0" style="width: 100%">
                    <colgroup>
                        <col style="width: 13%; text-align: right;" />
                        <col style="width: 17%;" />
                        <col style="width: 13%; text-align: right;" />
                        <col style="width: 50%;" />
                        <col style="width: 7%;" />
                    </colgroup>
                    <tr>
                        <td>
                            <asp:Label ID="lblCodice" runat="server" Text="Codice" CssClass="label"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodice" runat="server" CssClass="textbox_stringa_disabilitato" style="text-align:right;"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="lblDescrizione" runat="server" Text="Descrizione" CssClass="label"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescrizione" runat="server" CssClass="textbox_stringa_obbligatorio" Width="100%"></asp:TextBox>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblDurata" runat="server" Text="Durata media (min)" CssClass="label"></asp:Label>
                        </td>
                        <td>
                            <on_val:OnitJsValidator ID="txtDurata" runat="server" CssClass="textbox_stringa_obbligatorio" style="text-align:right; text-transform: uppercase;"
                                actionCorrect="False" actionDelete="False" actionFocus="True" actionMessage="True" actionSelect="False"
                                actionUndo="True" autoFormat="True" validationType="Validate_custom" CustomValFunction-Length="2" CustomValFunction="validaNumero"
                                SetOnChange="True" MaxLength="5"></on_val:OnitJsValidator>
                        </td>
                        <td>
                            <asp:Label ID="lblTipologia" runat="server" Text="Tipologia" CssClass="label"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlTipologia" runat="server" CssClass="textbox_stringa_obbligatorio" Width="100%"></asp:DropDownList>
                        </td>
                        <td></td>
                    </tr>
                </table>
            </dyp:DynamicPanel>
        </on_lay3:OnitLayout3>
    </form>
</body>
</html>
