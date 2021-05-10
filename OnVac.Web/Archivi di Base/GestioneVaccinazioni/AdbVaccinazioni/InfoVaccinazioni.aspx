<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="InfoVaccinazioni.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.InfoVaccinazioni" ValidateRequest="false" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Informazioni sulle vaccinazioni</title>

    <asp:PlaceHolder runat="server">
        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

        <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
    </asp:PlaceHolder>

    <script type="text/javascript" src="../../../Scripts/ckeditor/ckeditor.js"></script>
    <script type="text/javascript" src="../../../Scripts/ckeditor/lang/it.js"></script>
    <script type="text/javascript" src="../../../Scripts/ckeditor/config.js"></script>

    <script type="text/javascript" language="javascript">
        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;

            switch (button.Key) {
                case 'btnSalva':
                    if (!confirm('Salvare le modifiche effettuate?')) evnt.needPostBack = false;
                    break;
                case 'btnAnnulla':
                    if (!confirm('Annullare le modifiche effettuate?')) evnt.needPostBack = false;
                    break;
            }

            return;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Height="100%" Width="100%" TitleCssClass="Title3" Titolo="Informazioni sulle vaccinazioni">
            
            <asp:HiddenField ID="hidId" runat="server" />

            <asp:Panel id="pnlTitolo" runat="server" CssClass="Title">
    			<asp:Label id="LayoutTitolo" runat="server" Width="100%" ></asp:Label>
			</asp:Panel>

            <div>
			    <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBarInfo" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
				    <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
				    <Items>
					    <igtbar:TBarButton Key="btnEdit" Text="Modifica" DisabledImage="~/Images/modifica_dis.gif" Image="~/Images/modifica.gif" ></igtbar:TBarButton>
					    <igtbar:TBSeparator></igtbar:TBSeparator>
                        <igtbar:TBarButton Key="btnPulisci" Text="Cancella campi" DisabledImage="~/Images/pulisci_dis.gif" Image="~/Images/pulisci.gif" 
                                ToolTip="Cancella il contenuto di tutti i campi" >
                            <DefaultStyle Width="130px" CssClass="infratoolbar_button_default"></DefaultStyle>
					    </igtbar:TBarButton>
					    <igtbar:TBSeparator></igtbar:TBSeparator>
					    <igtbar:TBarButton Key="btnSalva" Text="Salva" DisabledImage="~/Images/salva_dis.gif" Image="~/Images/salva.gif"></igtbar:TBarButton>
					    <igtbar:TBarButton Key="btnAnnulla" Text="Annulla" DisabledImage="~/Images/annulla_dis.gif" Image="~/Images/annulla.gif"></igtbar:TBarButton>
				    </Items>
			    </igtbar:UltraWebToolbar>
            </div>

            <asp:Panel runat="server" ID="pnlTitoloInfo" CssClass="sezione">Titolo</asp:Panel>
            <div>
                <asp:TextBox runat="server" ID="txtTitolo" Width="99%" style="margin-top:10px;margin-bottom:10px;margin-left:10px;" MaxLength="100"></asp:TextBox>
            </div>        

            <asp:Panel runat="server" ID="pnlDescrizioneInfo" CssClass="sezione">Descrizione</asp:Panel>

            <dyp:DynamicPanel ID="dyp1" runat="server" Width="100%" Height="100%" ScrollBars="None">
                <asp:TextBox runat="server" ID="txtDescrizione" CssClass="ckeditor" TextMode="MultiLine"></asp:TextBox>
            </dyp:DynamicPanel>

        </on_lay3:OnitLayout3>
    </form>
</body>
</html>
