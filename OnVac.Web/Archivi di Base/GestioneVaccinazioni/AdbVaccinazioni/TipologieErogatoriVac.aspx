<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TipologieErogatoriVac.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.TipologieErogatoriVac" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title></title>
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <script type="text/javascript">
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

                //case "btnSalva":
                //    ordine = document.getElementById('txtOrdine').value;
                //    if (isNaN(ordine)) {
                //        alert("Inserire un numero nel campo 'Ordine'!");
                //        evnt.needPostBack = false;
                //    }
            }
        }

        function ClickTlb(t, btn, evnt) {
            evnt.needPostBack = true;
        		
            switch (btn.Key) {
                
                case 'btnSalvaCampiObbligatori':
                    evnt.needPostBack = confirm('ATTENZIONE: salvare le modifiche effettuate?');
                    break;
            
                case 'btnAnnullaCampiObbligatori':
                    evnt.needPostBack = confirm('ATTENZIONE: le modifiche effettuate verranno perse. Continuare?');
                    break;
            }
        }
	</script>
    
    <style type="text/css">
        .dataPick_Disabilitato {
            background-color: #EBEBE4;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <on_lay3:onitlayout3 id="OnitLayout31" runat="server" width="100%" height="100%" TitleCssClass="Title3" Titolo="Tipo Erogatore">
            <div>
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
					<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
				    <Items>
					    <igtbar:TBarButton Key="btnCerca" Text="Cerca" DisabledImage="~/Images/cerca_dis.gif" Image="~/Images/cerca.gif"></igtbar:TBarButton>
					    <igtbar:TBSeparator></igtbar:TBSeparator>
					    <igtbar:TBarButton Key="btnNuovo" Text="Nuovo" DisabledImage="~/Images/Nuovo_dis.gif" Image="~/Images/Nuovo.gif" ></igtbar:TBarButton>
					    <igtbar:TBarButton Key="btnModifica" Text="Modifica" DisabledImage="~/Images/Modifica_dis.gif" Image="~/Images/Modifica.gif" ></igtbar:TBarButton>
					    <igtbar:TBarButton Key="btnElimina" Text="Elimina" DisabledImage="~/Images/Elimina_dis.gif" Image="~/Images/Elimina.gif" ></igtbar:TBarButton>
					    <igtbar:TBSeparator></igtbar:TBSeparator>
					    <igtbar:TBarButton Key="btnSalva" Text="Salva" DisabledImage="~/Images/salva_dis.gif" Image="~/Images/salva.gif" ></igtbar:TBarButton>
					    <igtbar:TBarButton Key="btnAnnulla" Text="Annulla" DisabledImage="~/Images/annulla_dis.gif" Image="~/Images/annulla.gif" ></igtbar:TBarButton>
				    </Items>
			    </igtbar:UltraWebToolbar>
            </div>
            <div class="vac-sezione">Risultati di ricerca</div>
            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                <asp:DataGrid ID="dgrTipiErogatoriVac" runat="server" Width="100%" AutoGenerateColumns="False">
                    <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
                    <ItemStyle CssClass="item"></ItemStyle>
                    <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
                    <HeaderStyle CssClass="header"></HeaderStyle>
                    <PagerStyle CssClass="pager" Position="TopAndBottom" Mode="NumericPages" />
                    <Columns>
                        <on_dgr:SelectorColumn HeaderStyle-Width="3%">
                            <ItemStyle HorizontalAlign="Center"/>
                        </on_dgr:SelectorColumn>
                        <asp:BoundColumn DataField="Id" Visible="false"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Codice" HeaderText="Codice" HeaderStyle-Width="12%"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Descrizione" HeaderText="Descrizione" HeaderStyle-Width="50%"></asp:BoundColumn>
                        <asp:BoundColumn DataField="CodiceAvn" HeaderText="Codice AVN" HeaderStyle-Width="12%"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Ordine" HeaderText="Ordine" HeaderStyle-Width="8%"></asp:BoundColumn>
                        <asp:TemplateColumn HeaderText="Obsoleto" HeaderStyle-Width="5%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblObsoleto" Text='<%# GetStringBoolean(Eval("Obsoleto")) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>
            </dyp:DynamicPanel>
            <div class="vac-sezione">Dettaglio</div>
            <div>
                <table style="width:100%; padding-top:5px;">
                    <colgroup>
                        <col style="width: 15%" />
                        <col style="width: 15%" />
                        <col style="width: 15%" />
                        <col style="width: 15%" />
                        <col style="width: 15%" />
                        <col style="width: 25%" />
                    </colgroup>
                    <tr>
                        <td class="label">Codice</td>
                        <td>
                            <asp:TextBox runat="server" ID="txtCodice" CssClass="TextBox_Stringa uppercase" style="width:100%" MaxLength="2"></asp:TextBox>
                        </td>
                        <td class="label">Descrizione</td>
                        <td colspan="3">
                            <asp:TextBox runat="server" ID="txtDescrizione" CssClass="TextBox_Stringa uppercase" style="width:100%" MaxLength="200"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>                     
                        <td class="label">Codice AVN</td>
                        <td>
                            <asp:TextBox runat="server" ID="txtCodiceAvn" CssClass="TextBox_Stringa" style="width:100%" MaxLength="2"></asp:TextBox>
                        </td>
                        <td class="label">Ordine</td>
                        <td>
                            <asp:TextBox runat="server" ID="txtOrdine" CssClass="TextBox_Stringa uppercase" style="width:100%" MaxLength="2"></asp:TextBox>
                        </td> 
                        <td class="label">Obsoleto</td>
                        <td>
                            <asp:CheckBox runat="server" ID="chkObsoleto" />
                        </td>                    					  
                    </tr>
                </table>
            </div>
        </on_lay3:onitlayout3>
    </form>
</body>
</html>