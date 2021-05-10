<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DettaglioLottoMagazzinoCentrale.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.DettaglioLottoMagazzinoCentrale" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="uc1" TagName="InsDatiLotto" Src="../../Common/Controls/InsDatiLotto.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <script type="text/javascript" src="../Magazzino.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <script type="text/javascript">
            function ToolBarClick(ToolBar, button, evnt) {
                evnt.needPostBack = true;
                switch (button.Key) {
                    case 'btnSalva':

                        var errorMessage = ControlloDatiLotto('<%=ucDatiLotto.codLottoClientID%>', '<%=ucDatiLotto.descLottoClientID%>', '<%=ucDatiLotto.codNCClientID%>', '<%=ucDatiLotto.dataPreparazioneClientID%>', '<%=ucDatiLotto.dataScadenzaClientID%>', '<%=ucDatiLotto.dosiScatolaClientID%>', '<%=ucDatiLotto.qtaInizialeClientID%>', '<%=ucDatiLotto.qtaMinimaClientID%>', '<%=ucDatiLotto.fornitoreClientID%>', '<%=ucDatiLotto.noteClientID%>');

                        if (errorMessage != "") {
                            alert("Salvataggio non effettuato.\n" + errorMessage);
                            evnt.needPostBack = false;
                        }
                        else {
                            // Controllo data scadenza
                            if (!ControlloScadenzaLotto('<%=ucDatiLotto.dataScadenzaClientID%>')) {
                                evnt.needPostBack = false;
                            }
                        }
                        break;

                    case 'btnAnnulla':
                        if (!confirm("Attenzione: le modifiche non salvate verranno perse. Continuare?")) {
                            evnt.needPostBack = false;
                        }
                        break;
                }
            }
        </script>

        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" TitleCssClass="Title3" Height="100%" Width="100%" Titolo="Magazzino Centrale - Dettaglio lotto">
            <div>
				<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="110px" CssClass="infratoolbar" >
					<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
					<SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                    <Items>
                        <igtbar:TBarButton Key="btnIndietro" Text="Indietro" >
                        </igtbar:TBarButton>
                        <igtbar:TBSeparator />
                        <igtbar:TBarButton Key="btnModifica" Text="Modifica" DisabledImage="~/Images/modifica_dis.gif" Image="~/Images/modifica.gif">
                        </igtbar:TBarButton>
                        <igtbar:TBSeparator />							    
                        <igtbar:TBarButton Key="btnSalva" Text="Salva" >
                        </igtbar:TBarButton>							    
                        <igtbar:TBarButton Key="btnAnnulla" Text="Annulla" >
                        </igtbar:TBarButton>
                    </Items>						
				</igtbar:UltraWebToolbar>
            </div>
			<div class="vac-sezione">
				<asp:Label id="Label1" runat="server">DATI LOTTO</asp:Label>
			</div>

            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
				<uc1:InsDatiLotto id="ucDatiLotto" runat="server"></uc1:InsDatiLotto>
			</dyp:DynamicPanel>

        </on_lay3:OnitLayout3>
    </form>
</body>
</html>
