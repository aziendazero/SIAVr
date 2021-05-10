<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TipiPagamentoVac.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.TipiPagamentoVac" %>

<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>

<%@ Register TagPrefix="uc1" TagName="ElencoCondizioniPagamento" Src="../../../Common/Controls/ElencoCondizioniPagamento.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html">
<head>
    <title>TipiPagamento</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/StylesAnaTipiPagamento.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("onvac.common.js")%>'></script>
    <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>

    <script type="text/javascript">
        $(document).ready(function () {
        });

        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }
        

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;
            switch (button.Key) {
                case "btnElimina":
                    if (!confirm("Confermare eliminazione?")) {
                        evnt.needPostBack = false;
                    }
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
         <on_lay3:onitlayout3 id="OnitLayout31" runat="server" width="100%" height="100%" TitleCssClass="Title3" Titolo="Tipi Pagamento">
            <div>
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
					<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
				    <Items>
					    <igtbar:TBarButton Key="btnNuovo" Text="Nuovo" DisabledImage="~/Images/Nuovo_dis.gif" Image="~/Images/Nuovo.gif" ></igtbar:TBarButton>
					    <igtbar:TBarButton Key="btnModifica" Text="Modifica" DisabledImage="~/Images/Modifica_dis.gif" Image="~/Images/Modifica.gif" ></igtbar:TBarButton>
					    <igtbar:TBarButton Key="btnElimina" Text="Elimina" DisabledImage="~/Images/Elimina_dis.gif" Image="~/Images/Elimina.gif" ></igtbar:TBarButton>
					    <igtbar:TBSeparator></igtbar:TBSeparator>
					    <igtbar:TBarButton Key="btnSalva" Text="Salva" DisabledImage="~/Images/salva_dis.gif" Image="~/Images/salva.gif" ></igtbar:TBarButton>
					    <igtbar:TBarButton Key="btnAnnulla" Text="Annulla" DisabledImage="~/Images/annulla_dis.gif" Image="~/Images/annulla.gif" ></igtbar:TBarButton>
				    </Items>
			    </igtbar:UltraWebToolbar>
            </div>

			<div class="vac-sezione">Elenco</div>
            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                <asp:DataGrid ID="dgrTipiPagamento" runat="server" Width="100%" AutoGenerateColumns="False"
                    OnSelectedIndexChanged="dgrTipiPagamento_SelectedIndexChanged">
                    <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
                    <ItemStyle CssClass="item"></ItemStyle>
                    <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
                    <HeaderStyle CssClass="header"></HeaderStyle>
                    <PagerStyle CssClass="pager" Position="TopAndBottom" Mode="NumericPages" />

                    <Columns>
                        <on_dgr:SelectorColumn HeaderStyle-Width="4%">
                            <ItemStyle HorizontalAlign="Center"/>
                        </on_dgr:SelectorColumn>

                        <asp:BoundColumn DataField="Guid" HeaderText="Guid" Visible="false"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Descrizione" HeaderText="Descrizione" HeaderStyle-Width="64%"></asp:BoundColumn>
                        <asp:BoundColumn DataField="CodiceEsterno" HeaderText="Codice Esterno" HeaderStyle-Width="16%"></asp:BoundColumn> 
                        <asp:BoundColumn DataField="CodiceAvn" HeaderText="Codice AVN" HeaderStyle-Width="16%"></asp:BoundColumn>      
                    </Columns>
                </asp:DataGrid>
            </dyp:DynamicPanel>

            <div class="vac-sezione">Dettaglio</div>
            <div style="width: 100%">
                <table style="width:100%; padding-top:5px;">    
                    <tr>
                        <td class="label">Descrizione</td>
                        <td colspan="3">
                            <asp:TextBox runat="server" ID="txtDescrizione" CssClass="TextBox_Stringa uppercase" style="width:100%" MaxLength="50"></asp:TextBox>
                        </td>

                        <td class="label">Codice Esterno</td>
                        <td>
                            <asp:TextBox runat="server" ID="txtCodiceEsterno" CssClass="TextBox_Stringa uppercase" style="width:100%" MaxLength="8"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="label">Importo</td>
                        <td>
                            <asp:DropDownList id="ddlImporto" CssClass="dropItems" runat="server">
                              <asp:ListItem Value="0">DISABILITATO</asp:ListItem>
                              <asp:ListItem Value="1">ABILITATO</asp:ListItem>
                              <asp:ListItem Value="2">OBBLIGATORIO</asp:ListItem>
                           </asp:DropDownList>
                        </td>

                        <td class="label">Esenzione</td>
                        <td>
                            <asp:DropDownList id="ddlEsenzione" CssClass="dropItems" runat="server">
                              <asp:ListItem Value="0">DISABILITATO</asp:ListItem>
                              <asp:ListItem Value="1">ABILITATO</asp:ListItem>
                              <asp:ListItem Value="2">OBBLIGATORIO</asp:ListItem>
                           </asp:DropDownList>
                        </td>                  
                        <td class="label">Impos. Auto. Importo</td>
                        <td>

                           <asp:CheckBox ID="chkImportoAuto" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">Condizioni Pagamento</td>
                        <td>               
                            <asp:CheckBox ID="chkCondPagamento" runat="server" />
                        </td>

                        <td class="label">Codice AVN</td>
                        <td>
                            <asp:TextBox runat="server" id="txtCodiceAvn" CssClass="TextBox_Stringa" style="width:100%" MaxLength="2"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </div>
        </on_lay3:onitlayout3>
    </form>
</body>
</html>