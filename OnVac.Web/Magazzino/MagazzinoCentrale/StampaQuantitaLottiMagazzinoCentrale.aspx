<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="StampaQuantitaLottiMagazzinoCentrale.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.StampaQuantitaLottiMagazzinoCentrale" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Stampa Quantitativi Movimentati</title>
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" TitleCssClass="Title3" Height="100%" Width="100%" Titolo="Magazzino Centrale - Stampa Quantitativi Movimentati">
			<div class="title" id="PanelTitolo" runat="server">
				<asp:Label id="LayoutTitolo" runat="server" Text="Stampa Quantitativi Movimentati"></asp:Label>
			</div>
            <div>
				<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="120px" CssClass="infratoolbar">
					<DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
					<SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
					<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
                    <Items>
                        <igtbar:TBarButton Key="btnStampaLotto" Text="Stampa per lotto" Image="~/Images/stampa.gif" ToolTip="Stampa quantitativi movimentati, raggruppati per lotto">
                        </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btnStampaConsultorio" Text="Stampa per centro" Image="~/Images/stampa.gif" ToolTip="Stampa quantitativi movimentati, raggruppati per centro vaccinale" >
                            <DefaultStyle Width="140px" CssClass="infratoolbar_button_default"></DefaultStyle>
                        </igtbar:TBarButton>
                        <igtbar:TBarButton Key="btnStampaElencoMovimenti" Text="Stampa elenco movimenti" Image="~/Images/stampa.gif" ToolTip="Stampa elenco movimenti" >
                            <DefaultStyle Width="190px" CssClass="infratoolbar_button_default"></DefaultStyle>
                        </igtbar:TBarButton>
                        <igtbar:TBSeparator />
                        <igtbar:TBarButton Key="btnPulisci" Text="Pulisci" Image="~/Images/pulisci.gif" ToolTip="Cancellazione campi di filtro">
                            <DefaultStyle Width="80px" CssClass="infratoolbar_button_default"></DefaultStyle>
                        </igtbar:TBarButton>
                    </Items>
				</igtbar:UltraWebToolbar>
            </div>
			<div class="sezione" style="margin-bottom: 3px">
				<asp:Label id="LayoutTitolo_sezioneFiltri" runat="server">FILTRI DI STAMPA</asp:Label>
			</div>
            
            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                <table cellpadding="2" cellspacing="0" border="0" width="100%">
                    <colgroup>
                        <col width="20%" />
                        <col width="5%" />
                        <col width="12%" />
                        <col width="5%" />
                        <col width="53%" />
                        <col width="5%" />
                    </colgroup>
                    <tr>
                        <td class="Label">Data di registrazione:</td>
                        <td class="Label">da</td>
                        <td>
                            <on_val:onitdatepick id="dpkDataRegistrazioneDa" runat="server" 
                                Height="20px" Width="120px" cssclass="textbox_data" DateBox="True"></on_val:onitdatepick>
                        </td>
                        <td class="Label">a</td>
                        <td>
                            <on_val:onitdatepick id="dpkDataRegistrazioneA" runat="server" 
                                Height="20px" Width="120px" cssclass="textbox_data" DateBox="True"></on_val:onitdatepick>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td class="label">Operatore:</td>
                        <td></td>
                        <td colspan="3">
                            <onitcontrols:OnitModalList id="fmUtente" runat="server" AltriCampi="UTE_ID as Id" 
                                Width="70%" PosizionamentoFacile="False" LabelWidth="-1px" SetUpperCase="False"
                                UseCode="True" Tabella="V_ANA_UTENTI" CampoDescrizione="UTE_DESCRIZIONE as Descrizione" CampoCodice="UTE_CODICE as Codice" 
                                CodiceWidth="29%" Label="Titolo" Obbligatorio="False"></onitcontrols:OnitModalList>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td class="label">Magazzino:</td>
                        <td></td>
                        <td colspan="3">
                            <onitcontrols:onitmodallist id="fmMagazzino" runat="server"
                                Width="70%" PosizionamentoFacile="False" LabelWidth="-1px" SetUpperCase="True"
                                UseCode="True" Tabella="T_ANA_CONSULTORI" CampoDescrizione="CNS_DESCRIZIONE Descrizione" CampoCodice="CNS_CODICE Codice"
								CodiceWidth="29%" Label="Titolo" Obbligatorio="False"
                                Filtro="(cns_cns_magazzino is null or cns_cns_magazzino = cns_codice) and cns_data_chiusura is null order by cns_descrizione"></onitcontrols:onitmodallist>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td class="label">Lotto:</td>
                        <td></td>
                        <td colspan="3">
                            <onitcontrols:OnitModalList id="fmLotto" runat="server" 
                                Width="70%" PosizionamentoFacile="False" LabelWidth="-1px" SetUpperCase="True"
                                UseCode="True" Tabella="T_ANA_LOTTI" CampoDescrizione="LOT_DESCRIZIONE as Descrizione" CampoCodice="LOT_CODICE as Codice" 
                                CodiceWidth="29%" Label="Titolo" Obbligatorio="False"
                                Filtro="1=1 order by lot_codice"></onitcontrols:OnitModalList>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td class="label">Tipo di movimento:</td>
                        <td></td>
                        <td colspan="3">
                            <asp:DropDownList ID="ddlTipoMovimento" runat="server" cssclass="textbox_Stringa" Width="100%"></asp:DropDownList>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td class="label">Quantità:</td>
                        <td></td>
                        <td>
                            <asp:RadioButtonList id="rdblOperatoreConfrontoQuantita" runat="server" CssClass="TextBox_Stringa" 
                                RepeatDirection="Horizontal" TextAlign="Left" CellPadding="7">
                                <asp:ListItem Text="<" Value="<"></asp:ListItem>
                                <asp:ListItem Text="=" Value="="></asp:ListItem>
                                <asp:ListItem Text=">" Value=">" Selected="True"></asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                        <td></td>
                        <td>
                            <on_val:onitJsValidator id="txtQuantita" runat="server" Width="100px" CssClass="textbox_stringa"
                                actionCorrect="False" actionDelete="False" actionFocus="False" actionMessage="True" actionSelect="True"
                                actionUndo="False" autoFormat="False" validationType="Validate_integer" PreParams-numDecDigit="0" PreParams-maxValue="99999999"
                                PreParams-minValue="0" MaxLength="8"></on_val:onitJsValidator>
                        </td>
                        <td></td>
                    </tr>
                </table>
            </dyp:DynamicPanel>

        </on_lay3:OnitLayout3>
    </form>
</body>
</html>
