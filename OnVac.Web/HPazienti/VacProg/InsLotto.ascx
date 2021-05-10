<%@ Control Language="vb" AutoEventWireup="false" Codebehind="InsLotto.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.InsLotto" %>

<%@ Register TagPrefix="uc1" TagName="InsDatiLotto" Src="../../Common/Controls/InsDatiLotto.ascx" %>

<link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
<link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

<script language="javascript" type="text/javascript" src="../../Magazzino/Magazzino.js"></script>
<script language="javascript" type="text/javascript">
		
		//Marco 11/12/03
		// Scopo: trasforma il contenuto di una text box il lettere TUTTE maiuscole
		// Parametri: obj= oggetto di tipo TextBox
		function stringToUpperCase(obj){
			obj.value=obj.value.toUpperCase();
		}
		
		function InizializzaToolBar_lotti(t)
		{
			t.PostBackButton=false;
		}
		
		function ToolBarClick_lotti(ToolBar,button,evnt){
			evnt.needPostBack=true;
			switch  (button.Key)
			{
				case 'btn_Salva':
					ControllaLotto(evnt,'<%=DatiLotto.codLottoClientID%>','<%=DatiLotto.descLottoClientID%>','<%=DatiLotto.codNCClientID%>','<%=DatiLotto.dataPreparazioneClientID%>','<%=DatiLotto.dataScadenzaClientID%>','<%=DatiLotto.fornitoreClientID%>','<%=DatiLotto.noteClientID%>','<%=DatiLotto.dosiScatolaClientID%>','<%=DatiLotto.qtaInizialeClientID%>','<%=DatiLotto.qtaMinimaClientID%>')
					break;
			    default:
					evnt.needPostBack=true;
			}
		}
</script>

<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
	<SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
	<ClientSideEvents InitializeToolbar="InizializzaToolBar_lotti" Click="ToolBarClick_lotti"></ClientSideEvents>
    <Items>
		<igtbar:TBarButton Key="btn_Salva" DisabledImage="~/Images/salva_dis.gif" Text="Salva" Image="~/Images/salva.gif"></igtbar:TBarButton>
	</Items>
</igtbar:ultrawebtoolbar>

<div class="vac-sezione" id="PanelLayoutTitolo">
    <asp:label id="LayoutTitolo_sezioneCnv" runat="server">INSERIMENTO LOTTO</asp:label>
</div>

<uc1:insdatilotto id="DatiLotto" runat="server"></uc1:insdatilotto>

<script type="text/javascript">
	<%response.write(strJS)%>
</script>
