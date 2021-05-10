<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="UtentiAbilitati.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.UtentiAbilitati" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>

<%@ Register Src="../../../Common/Controls/FiltroRicercaImmediata.ascx" TagName="FiltroRicercaImmediata" TagPrefix="uc1" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Utenti Abilitati al Centro Vaccinale Corrente</title>
    
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <style type="text/css">
        .filtro_ricerca_utenti {
            width: 98%;
            margin-top: 4px;
            margin-left: 2px;
            margin-right: 2px;
            margin-bottom: 2px;
            background-color: whitesmoke;
            border: 1px solid navy;
            padding: 2px;
        }

        .vac-sezione {
            margin: 2px;
        }
    </style>

    <script type="text/javascript">

        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }
        
        function ToolBarClick(t, button, evnt) {

            evnt.needPostBack = true;

            switch (button.Key) {

                case 'btnIndietro':
                    window.location.href = "Consultori.aspx?RicaricaDati=True";
                    evnt.needPostBack = false;
                    break;
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Height="100%" Width="100%" TitleCssClass="Title3" Titolo="Utenti abilitati al Centro Vaccinale" >

			<div class="title" id="PanelTitolo" runat="server" style="width: 100%;">
			    <asp:Label id="LayoutTitolo" runat="server"></asp:Label>
			</div>

            <div>
				<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="90px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
					<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
					<Items>
						<igtbar:TBarButton Key="btnIndietro" Text="Indietro" Image="../../../images/prev.gif"></igtbar:TBarButton>
					</Items>
				</igtbar:UltraWebToolbar>
            </div>
                        
            <div class="vac-sezione">RICERCA UTENTI</div>

            <div class="filtro_ricerca_utenti">
                <uc1:FiltroRicercaImmediata id="ucFiltroRicercaUtenti" runat="server"></uc1:FiltroRicercaImmediata>
            </div>

            <dyp:DynamicPanel ID="dypScroll1" runat="server" Width="100%" Height="60%" ScrollBars="Auto" RememberScrollPosition="true">
                                                                                
                <asp:DataGrid id="dgrUtentiRicerca" runat="server" Width="100%" AutoGenerateColumns="False" 
                    AllowCustomPaging="true" AllowPaging="true" PageSize="25">
					<AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
					<ItemStyle CssClass="item"></ItemStyle>
					<SelectedItemStyle CssClass="selected"></SelectedItemStyle>
                    <HeaderStyle CssClass="header"></HeaderStyle>
					<PagerStyle CssClass="pager" Position="TopAndBottom" Mode="NumericPages" />
					<Columns>
					    <on_dgr:SelectorColumn>
					        <ItemStyle HorizontalAlign="Center" Width="2%" />
					    </on_dgr:SelectorColumn>
					    <asp:BoundColumn Visible="False" DataField="Id" HeaderText="idUtente"></asp:BoundColumn>
                        <asp:BoundColumn Visible="True" DataField="Codice" HeaderText="Codice" HeaderStyle-Width="20%"></asp:BoundColumn>
                        <asp:BoundColumn Visible="True" DataField="Descrizione" HeaderText="Descrizione" HeaderStyle-Width="28%"></asp:BoundColumn>
                        <asp:BoundColumn Visible="True" DataField="Cognome" HeaderText="Cognome" HeaderStyle-Width="25%"></asp:BoundColumn>
                        <asp:BoundColumn Visible="True" DataField="Nome" HeaderText="Nome" HeaderStyle-Width="25%"></asp:BoundColumn>
                    </Columns>
                </asp:DataGrid> 

            </dyp:DynamicPanel>

            <div class="vac-sezione">
                <asp:Label ID="lblUtentiAbilitati" runat="server" Text="UTENTI ABILITATI AL CENTRO VACCINALE SELEZIONATO"></asp:Label>
            </div>

            <dyp:DynamicPanel ID="dypScroll2" runat="server" Width="100%" Height="40%" ScrollBars="Auto" RememberScrollPosition="true">
                                                
                <asp:DataGrid id="dgrUtentiAssociati" runat="server" Width="100%" AutoGenerateColumns="False" AllowCustomPaging="true" AllowPaging="true" PageSize="25">
					<AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
					<ItemStyle CssClass="item"></ItemStyle>
					<SelectedItemStyle CssClass="selected"></SelectedItemStyle>
                    <HeaderStyle CssClass="header"></HeaderStyle>
					<PagerStyle CssClass="pager" Position="TopAndBottom" Mode="NumericPages" />
					<Columns>
					    <on_dgr:SelectorColumn>
					        <ItemStyle HorizontalAlign="Center" Width="2%" />
					    </on_dgr:SelectorColumn>
					    <asp:BoundColumn Visible="False" DataField="Id" HeaderText="idUtente"></asp:BoundColumn>
                        <asp:BoundColumn Visible="True" DataField="Codice" HeaderText="Codice" HeaderStyle-Width="20%"></asp:BoundColumn>
                        <asp:BoundColumn Visible="True" DataField="Descrizione" HeaderText="Descrizione" HeaderStyle-Width="28%"></asp:BoundColumn>
                        <asp:BoundColumn Visible="True" DataField="Cognome" HeaderText="Cognome" HeaderStyle-Width="25%"></asp:BoundColumn>
                        <asp:BoundColumn Visible="True" DataField="Nome" HeaderText="Nome" HeaderStyle-Width="25%"></asp:BoundColumn>
                    </Columns>
                </asp:DataGrid> 

            </dyp:DynamicPanel>

        </on_lay3:OnitLayout3>
    </form>
</body>
</html>
