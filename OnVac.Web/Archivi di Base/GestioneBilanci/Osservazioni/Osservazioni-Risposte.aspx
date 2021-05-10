<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Osservazioni-Risposte.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.Osservazioni_Risposte" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<%@ Import Namespace="Onit.OnAssistnet.OnVac" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Osservazioni_Risposte</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript" language="javascript">

        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;

            switch (button.Key) {
                case 'btnIndietro':
                    evnt.needPostBack = false;
                    window.location.href='Osservazioni.aspx';
                    break;
            }
        }
    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Titolo="Osservazioni-Risposte" TitleCssClass="Title3" Width="100%" Height="100%">

            <div class="title" id="titolo" runat="server"></div>
            <div>
                <asp:Label ID="lblTitolo" Style="z-index: 101; left: 0px; position: absolute; top: 32px" runat="server"
                    Height="16px" CssClass="Title1" Width="100%"></asp:Label>
            </div>
            <div>
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                    <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
                    <Items>
                        <igtbar:TBarButton Key="btnAggiungi" Text="Aggiungi" Image="~/Images/avanti.gif" ToolTip="Aggiunge le risposte selezionate alle risposte associate all'osservazione"></igtbar:TBarButton>
                        <igtbar:TBarButton Key="btnElimina" Text="Elimina" Image="~/Images/indietro.gif" ToolTip="Rimuove le risposte selezionate alle risposte associate all'osservazione"></igtbar:TBarButton>
                        <igtbar:TBarButton Key="btnSalva" Text="Salva" Image="~/Images/salva.gif"></igtbar:TBarButton>
                        <igtbar:TBarButton Key="btnAnnulla" Text="Annulla" Image="~/Images/annulla.gif"></igtbar:TBarButton>
                        <igtbar:TBSeparator Key="sep"></igtbar:TBSeparator>
                        <igtbar:TBarButton Key="btnIndietro" Text="Indietro" Image="~/Images/indietro.gif"></igtbar:TBarButton>
                        <igtbar:TBSeparator Key="sep"></igtbar:TBSeparator>
                        <igtbar:TBarButton Key="btnDefault" Text="Risposta Predefinita" Image="../../../Images/defaultPulsante.gif">
                            <DefaultStyle Width="150px" CssClass="infratoolbar_button_default"></DefaultStyle>
                        </igtbar:TBarButton>
                    </Items>
                </igtbar:UltraWebToolbar>
            </div>

            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                <div>
                    <table width="100%" style="table-layout: fixed;">
                        <colgroup>
                            <col width="45%" />
                            <col width="55%" />
                        </colgroup>
                        <tr>
                            <td>
                                <div class="vac-sezione">Risposte</div>
                            </td>
                            <td>
                                <div class="vac-sezione">Risposte associate</div>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top">
                                <asp:DataList ID="dlsRisposte" runat="server" CssClass="DataGrid" Width="100%">
                                    <SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
                                    <HeaderTemplate>
                                        <table style="table-layout: fixed;" width="100%">
                                            <tr>
                                                <td width="15">
                                                    <asp:CheckBox ID="chkSelezionaAll" language="javascript" onclick="CheckAll('dlsRisposte',this.checked,0,0);"
                                                        runat="server" ToolTip="Seleziona tutte le risposte della colonna"></asp:CheckBox></td>
                                                <td width="20%">
                                                    <asp:Label ID="lblTitolo1" runat="server">Codice</asp:Label></td>
                                                <td width="70%">
                                                    <asp:Label ID="lblTitolo2" runat="server">Descrizione</asp:Label></td>
                                            </tr>
                                        </table>
                                    </HeaderTemplate>
                                    <EditItemStyle CssClass="Edit"></EditItemStyle>
                                    <AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
                                    <ItemStyle CssClass="Item"></ItemStyle>
                                    <ItemTemplate>
                                        <table style="table-layout: fixed;" width="100%">
                                            <tr>
                                                <td width="20">
                                                    <asp:CheckBox ID="chkSeleziona" runat="server"></asp:CheckBox></td>
                                                <td width="20%">
                                                    <asp:Label ID="lblCodice" runat="server" Text='<%# Container.DataItem("RIS_CODICE") %>' Width="100%">
                                                    </asp:Label></td>
                                                <td width="70%">
                                                    <asp:Label ID="lblDescrizione" runat="server" Text='<%# Container.DataItem("RIS_DESCRIZIONE") %>'>
                                                    </asp:Label></td>
                                            </tr>
                                        </table>
                                    </ItemTemplate>
                                    <HeaderStyle CssClass="Header"></HeaderStyle>
                                </asp:DataList>
                            </td>
                            <td valign="top">
                                <asp:DataList ID="dlsRisposteAssociate" runat="server" CssClass="DataGrid" Width="100%">
                                    <SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
                                    <HeaderTemplate>
                                        <table style="table-layout: fixed;" width="100%">
                                            <tr>
                                                <td width="15">&nbsp;</td>
                                                <td width="40">
                                                    <asp:CheckBox ID="chkSelezionaAll" language="javascript" onclick="CheckAll('dlsRisposteAssociate',this.checked,0,0);"
                                                        runat="server" ToolTip="Seleziona tutte le risposte della colonna"></asp:CheckBox></td>
                                                <td width="50">
                                                    <asp:Label ID="lblTitolo1" runat="server">Codice</asp:Label></td>
                                                <td width="50%">
                                                    <asp:Label ID="lblTitolo2" runat="server">Descrizione</asp:Label></td>
                                                <td width="50">
                                                    <asp:Label ID="lblTitolo3" runat="server">Numero</asp:Label></td>
                                                <td width="40">
                                                    <asp:Label ID="lblTitolo4" runat="server">Pred.</asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </HeaderTemplate>
                                    <EditItemStyle CssClass="Edit"></EditItemStyle>
                                    <AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
                                    <ItemStyle CssClass="Item"></ItemStyle>
                                    <HeaderStyle CssClass="Header"></HeaderStyle>
                                    <ItemTemplate>
                                        <table style="table-layout: fixed;" width="100%">
                                            <tr>
                                                <td width="20">
                                                    <img alt="" language="javascript" onclick="MoveUp(this.parentNode.parentNode);" runat="server" src="~/Images/sopra.gif" /><br>
                                                    <img alt="" language="javascript" onclick="MoveDown(this.parentNode.parentNode);" runat="server" src="~/Images/sotto.gif" />
                                                </td>
                                                <td width="40">
                                                    <asp:CheckBox ID="chkSeleziona" runat="server"></asp:CheckBox></td>
                                                <td width="50" align="center">
                                                    <asp:TextBox ID="lblCodice" Style="border: 0px; background-color: transparent" runat="server" Width="100%" Text='<%# Container.DataItem("RIO_RIS_CODICE") %>'>
                                                    </asp:TextBox></td>
                                                <td width="50%">
                                                    <asp:Label ID="lblDescrizione" runat="server" Text='<%# Container.DataItem("RIS_DESCRIZIONE") %>'>
                                                    </asp:Label></td>
                                                <td width="50" align="center">
                                                    <asp:TextBox ID="txtPosizione" Style="border: 0px; background-color: transparent" runat="server" Width="100%" Text='<%# Container.DataItem("RIO_N_RISPOSTA") %>'>
                                                    </asp:TextBox></td>
                                                <td width="40">
                                                    <asp:ImageButton ID="imgDefault" Enabled="false" title="Risposta predefinita" runat="server" ImageUrl="../../../Images/default.gif" /></td>
                                            </tr>
                                        </table>
                                    </ItemTemplate>
                                </asp:DataList>
                            </td>
                        </tr>
                    </table>
                </div>
            </dyp:DynamicPanel>
        </on_lay3:OnitLayout3>
    </form>

    <script type="text/javascript" language="javascript">

        function MoveUp(el) {
            elBase = el;

            var lbl1 = GetElementByTag(GetElementByTag(el, "tr", 1, 0).cells[4], "input", 1, 3, true);
            var tab = GetElementByTag(el, "table", 2, 0);
            el = GetElementByTag(el, "tr", 2, 0);

            if (el.rowIndex == 1)
                return false;

            indiceColSel = GetElementByTag(elBase, "tr", 2, 0).rowIndex;
            indiceColSup = indiceColSel - 1;

            swapCells(elBase, indiceColSel, indiceColSup);

            OnitLayoutStatoMenu(true);
        }

        function MoveDown(el) {
            elBase = el;
            var lbl1 = GetElementByTag(GetElementByTag(el, "tr", 1, 0).cells[4], "input", 1, 3, true);

            var tab = GetElementByTag(el, "table", 2, 0);
            el = GetElementByTag(el, "tr", 2, 0);
            if (el.rowIndex >= tab.rows.length - 1)
                return false;

            indiceColSel = GetElementByTag(elBase, "tr", 2, 0).rowIndex;
            indiceColInf = indiceColSel + 1;

            swapCells(elBase, indiceColSel, indiceColInf);

            OnitLayoutStatoMenu(true);
        }

        function swapCells(elCliccato, indiceColSel, indiceColDaSost) {

            var elTmp = new Array;
            var elTmp2 = new Array;

            //alert(elBase.childNodes.length);
            //Prendo i dati dalla tabella selezionata
            for (i = 0; i < elCliccato.cells.length; i++) {
                if (i != 4) {
                    for (j = 0; j < elCliccato.cells[i].childNodes.length; j++) {
                        if (elCliccato.cells[i].childNodes[j].firstChild != null && elCliccato.cells[i].childNodes[j].type != "select-one") {
                            elTmp[i] = elCliccato.cells[i].childNodes[j].firstChild.nodeValue;
                        }
                        else
                            if (elCliccato.cells[i].childNodes[j].type == "text" || elCliccato.cells[i].childNodes[j].type == "select-one")
                                elTmp[i] = elCliccato.cells[i].childNodes[j].value;
                            else if (elCliccato.cells[i].childNodes[j].type == "image")
                                elTmp[i] = elCliccato.cells[i].childNodes[j].style.display;
                    }
                }
            }

            tabPrincipale = GetElementByTag(elCliccato, "table", 2, 0, false);
            rowSuperiore = tabPrincipale.rows[indiceColDaSost];
            tabDaModificare = GetElementByTag(rowSuperiore, "table", 1, 1, false);
            for (i = 0; i < tabDaModificare.rows[0].cells.length; i++) {
                if (i != 4) {
                    for (j = 0; j < tabDaModificare.rows[0].cells[i].childNodes.length; j++) {
                        if (tabDaModificare.rows[0].cells[i].childNodes[j].firstChild != null && tabDaModificare.rows[0].cells[i].childNodes[j].type != "select-one") {
                            elTmp2[i] = tabDaModificare.rows[0].cells[i].childNodes[j].firstChild.nodeValue;
                            tabDaModificare.rows[0].cells[i].childNodes[j].firstChild.nodeValue = elTmp[i];
                        }
                        else
                            if (elCliccato.cells[i].childNodes[j].type == "text" || elCliccato.cells[i].childNodes[j].type == "select-one") {
                                elTmp2[i] = tabDaModificare.rows[0].cells[i].childNodes[j].value;
                                tabDaModificare.rows[0].cells[i].childNodes[j].value = elTmp[i];
                            }
                            else if (elCliccato.cells[i].childNodes[j].type == "image") {
                                elTmp2[i] = tabDaModificare.rows[0].cells[i].childNodes[j].style.display;
                                tabDaModificare.rows[0].cells[i].childNodes[j].style.display = elTmp[i];
                            }
                    }
                }
            }

            for (i = 0; i < elCliccato.cells.length; i++) {
                if (i != 4) {
                    for (j = 0; j < elBase.cells[i].childNodes.length; j++) {
                        if (elBase.cells[i].childNodes[j].firstChild != null && elBase.cells[i].childNodes[j].type != "select-one") {
                            elBase.cells[i].childNodes[j].firstChild.nodeValue = elTmp2[i];
                        }
                        else
                            if (elCliccato.cells[i].childNodes[j].type == "text" || elCliccato.cells[i].childNodes[j].type == "select-one")
                                elBase.cells[i].childNodes[j].value = elTmp2[i];
                            else if (elCliccato.cells[i].childNodes[j].type == "image")
                                elBase.cells[i].childNodes[j].style.display = elTmp2[i];
                    }
                }
            }

            /* DEBUG: mostra gli array con i dati che vengono swappati			
			var debug = 'array 1:\n';
			for (i=0;i<elTmp.length;i++)
			    debug = debug + '\t' + elTmp[i];
            debug = debug + '\narray 2:\n';
			for (i=0;i<elTmp2.length;i++)
			    debug = debug + '\t' + elTmp2[i];  
			alert(debug);
			*/
        }
    </script>
</body>
</html>
