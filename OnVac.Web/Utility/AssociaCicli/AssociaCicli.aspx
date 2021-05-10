<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="AssociaCicli.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.AssociaCicli" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Associa Cicli</title>
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <script type="text/javascript" language="javascript" src="./JsAssociaCicli.js"></script>

    <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
    <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("onvac.common.js")%>'></script>

</head>
<body>
    <form id="Form1" method="post" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Titolo="Associazione Cicli ai Pazienti" TitleCssClass="Title3" Width="100%" Height="100%">
			<div class="title" id="PanelTitolo" runat="server">
				<asp:Label id="LayoutTitolo" runat="server">&nbsp;Utility Associazione Cicli ai Pazienti </asp:Label>
            </div>
            <div>
				<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="130px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
					<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
					<Items>
						<igtbar:TBarButton Key="btnCerca" Text="Cerca Pazienti" DisabledImage="~/Images/cerca_dis.gif"
							Image="~/Images/cerca.gif">
						</igtbar:TBarButton>
						<igtbar:TBSeparator></igtbar:TBSeparator>
						<igtbar:TBarButton Key="btnAssocia" Text="Associa Ciclo" DisabledImage="../../images/rotella_dis.gif"
							Image="../../images/rotella.gif">
						</igtbar:TBarButton>
					</Items>
				</igtbar:UltraWebToolbar>
            </div>
            <div>
				<table width="100%" bgColor="whitesmoke">
					<tr height="2">
						<td></td>
					</tr>
					<tr>
						<td>
							<fieldset class="vacFieldset" title="Ricerca Convocati">
                                <legend class="label">Ricerca Pazienti da Associare</legend>
								<table class="label_left" style="margin-top: 2px" cellspacing="0" cellpadding="2" width="100%" border="0">
									<colgroup>
										<col width="2%" />
										<col width="15%" />
										<col width="2%" />
										<col width="15%" />
										<col width="2%" />
										<col width="15%" />
										<col width="10%" />
										<col width="37%" />
										<col width="2%" />
									</colgroup>
									<tr height="5">
										<td colspan="9"></td>
									</tr>
									<tr>
										<td></td>
										<td class="label_left">Centro Vaccinale:</Td>
										<td colspan="6">
											<onitcontrols:OnitModalList id="omlConsultorio" runat="server" Filtro="cns_data_apertura <= SYSDATE AND (cns_data_chiusura > SYSDATE OR cns_data_chiusura IS NULL) ORDER BY cns_descrizione"
												Width="70%" CampoDescrizione="CNS_DESCRIZIONE" CampoCodice="CNS_CODICE" Tabella="T_ANA_CONSULTORI" PosizionamentoFacile="False"
												LabelWidth="-1px" CodiceWidth="29%" Label="Titolo" UseCode="True" SetUpperCase="True" Obbligatorio="True"></onitcontrols:OnitModalList></TD>
										<td></td>
									</tr>
									<tr>
										<td></td>
										<td class="label_left">Data nascita:</td>
										<td class="label" align="right">Da</td>
										<td>
											<on_val:onitdatepick id="dpkDataNascitaDa" runat="server" Height="20px" Width="120px" CssClass="textbox_data"
												DateBox="True"></on_val:onitdatepick>
                                        </td>
										<td class="label" align="right">A</td>
										<td>
											<on_val:onitdatepick id="dpkDataNascitaA" runat="server" Height="20px" Width="120px" CssClass="textbox_data"
												DateBox="True"></on_val:onitdatepick></td>
										<td class="label">Sesso:</td>
										<td>
											<asp:DropDownList id="ddlSesso" Width="99%" Runat="server">
												<asp:ListItem Selected="True"></asp:ListItem>
												<asp:ListItem Value="M">Maschio</asp:ListItem>
												<asp:ListItem Value="F">Femmina</asp:ListItem>
											</asp:DropDownList>
                                        </td>
										<td></td>
									</tr>
									<tr>
										<td></td>
										<td class="label_left">Categoria rischio:</td>
										<td colspan="4">
											<onitcontrols:OnitModalList id="omlCategorieRischio" runat="server" Width="70%" CampoDescrizione="RSC_DESCRIZIONE"
												CampoCodice="RSC_CODICE" Tabella="T_ANA_RISCHIO" PosizionamentoFacile="False" LabelWidth="-1px" CodiceWidth="29%"
												Label="Titolo" SetUpperCase="True"></onitcontrols:OnitModalList></td>
										<td class="label">Malattia:</td>
										<td>
											<onitcontrols:OnitModalList id="omlMalattia" runat="server" Width="70%" CampoDescrizione="MAL_DESCRIZIONE Descrizione" CampoCodice="MAL_CODICE Codice"
												Tabella="T_ANA_MALATTIE" PosizionamentoFacile="False" LabelWidth="-1px" CodiceWidth="29%" Label="Titolo"
												SetUpperCase="True" Filtro=" MAL_OBSOLETO='N' ORDER BY MAL_CODICE"></onitcontrols:OnitModalList></td>
										<td></td>
									</tr>
									<tr>
										<td></td>
										<td class="label_left">Stati anagrafici:</td>
										<td align="center">
                                            <img id="btnImgAggiungiStati" onmouseover="mouse(this,'over');" title="Impostazione filtro stati anagrafici" alt=""
												style="cursor: hand" onclick="document.getElementById('btnAggiungiStati').click();" onmouseout="mouse(this,'out');"
												src="../../images/filtro_statianag.gif" />
											<asp:Button id="btnAggiungiStati" style="DISPLAY: none" runat="server" Text="..."></asp:Button></td>
										<td style="border-right: navy 1px solid; border-top: navy 1px solid; padding-left: 5px; border-left: navy 1px solid; border-bottom: navy 1px solid"
											valign="top" bgColor="gainsboro" colspan="5">
											<asp:Label id="lblStatoAnagrafico" style="font-size: 10px; text-transform: uppercase; font-style: italic; font-family: Verdana"
												runat="server" Width="95%" CssClass="TextBox_Stringa"></asp:Label></td>
										<td></td>
									</tr>
									<tr height="10">
										<td colspan="9"></td>
									</tr>
								</table>
							</fieldset>
						</td>
					</tr>
					<tr height="2">
						<td></td>
					</tr>
					<tr>
						<td>
							<fieldset class="vacFieldset" id="fldcicloass" title="Ciclo da assegnare ai pazienti selezionati">
                                <legend class="label">Ciclo da Associare</legend>
								<table style="margin-top: 2px" cellspacing="0" cellpadding="2" width="100%" border="0">
									<colgroup>
										<col width="2%" />
										<col width="15%" />
										<col width="79%" />
										<col width="2%" />
									</colgroup>
									<tr height="5">
										<td colspan="4"></td>
									</tr>
									<tr>
										<td></td>
										<td class="label_left">Ciclo:</td>
										<td>
											<onitcontrols:OnitModalList id="omlCicloCNV" runat="server" Width="70%" CampoDescrizione="CIC_DESCRIZIONE Descrizione" CampoCodice="CIC_CODICE Codice"
												Tabella="T_ANA_CICLI" PosizionamentoFacile="False" LabelWidth="-1px" CodiceWidth="29%" Label="Titolo" UseCode="True" Filtro=" CIC_OBSOLETO='N' ORDER BY CIC_CODICE"
												SetUpperCase="True" Obbligatorio="True" Enabled="False" UseTableLayout="True" AltriCampi="TO_CHAR(CIC_DATA_INTRODUZIONE,'dd/mm/yyyy') INTRODUZIONE, TO_CHAR(CIC_DATA_FINE,'dd/mm/yyyy') FINE, CIC_STANDARD STANDARD, CIC_SESSO SESSO"></onitcontrols:OnitModalList></td>
										<td></td>
									</tr>
									<tr height="10">
										<td colspan="5"></td>
									</tr>
								</table>
							</fieldset>
						</td>
					</tr>
					<tr height="2">
						<td></td>
					</tr>
				</table>
            </div>
			<div class="vac-sezione" style="margin: 2px">
				<asp:Label id="lblPazientiTrovati" runat="server">Pazienti trovati</asp:Label>
            </div>
            <div>
				<asp:DropDownList id="ddlPager" style="FONT-WEIGHT: normal; MARGIN: 2px" runat="server" BackColor="AliceBlue" Width="100%" Visible="False" cssclass="mysezione">
					<asp:ListItem Value="1" Selected="True">[Pagina 1]</asp:ListItem>
					<asp:ListItem Value="2">[Pagina 2]</asp:ListItem>
				</asp:DropDownList>
            </div>
            
            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
				<table height="100%" cellspacing="0" cellpadding="0" width="100%" border="0">
					<tr>
						<td width="2"></td>
						<td valign="top">
							<on_dgr:OnitGrid id="dgrPazienti" style="BORDER-RIGHT: navy 1px solid; BORDER-TOP: navy 1px solid; BORDER-LEFT: navy 1px solid; BORDER-BOTTOM: navy 1px solid"
								runat="server" Width="100%" CssClass="DataGrid" Visible="False" SortedColumns="Matrice IGridColumn[]"
								AllowPaging="True" SelectionOption="none" PagingMode="Auto" PageSize="100" GridLines="Horizontal"
								AutoGenerateColumns="False" PagerVoicesBefore="10" PagerVoicesAfter="10" CascadeStyles="True"
								PagerDropDownList="ddlPager" PagerGoToOption="False">
								<AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
								<ItemStyle CssClass="Item"></ItemStyle>
								<Columns>
									<on_dgr:OnitBoundColumn Visible="False" DataField="PAZ_CODICE" key="PAZ_CODICE" SortDirection="NoSort"></on_dgr:OnitBoundColumn>
									<on_dgr:OnitTemplateColumn key="CheckBox" SortDirection="NoSort" HeaderText="&lt;input type='checkbox' id='chkSelTutti' onclick='selezionaPazienti(this.checked);'/&gt;">
										<ItemTemplate>
											<asp:CheckBox id="chkSel" runat="server" Checked='<%# (DataBinder.Eval(Container, "DataItem")("SEL")) = "S" %>'>
											</asp:CheckBox>
										</ItemTemplate>
									</on_dgr:OnitTemplateColumn>
									<on_dgr:OnitBoundColumn DataField="PAZ_COGNOME" HeaderText="Cognome" key="PAZ_COGNOME" SortDirection="NoSort"></on_dgr:OnitBoundColumn>
									<on_dgr:OnitBoundColumn DataField="PAZ_NOME" HeaderText="Nome" key="PAZ_NOME" SortDirection="NoSort"></on_dgr:OnitBoundColumn>
									<on_dgr:OnitBoundColumn DataField="PAZ_DATA_NASCITA" HeaderText="Data Nascita" key="PAZ_DATA_NASCITA" DataFormatString="{0:dd/MM/yyyy}"
										SortDirection="NoSort"></on_dgr:OnitBoundColumn>
									<asp:BoundColumn DataField="com_descrizione_residenza" HeaderText="Comune Residenza"></asp:BoundColumn>
									<on_dgr:OnitBoundColumn DataField="PAZ_INDIRIZZO_RESIDENZA" HeaderText="Indirizzo Residenza" key="PAZ_INDIRIZZO_RESIDENZA"
										SortDirection="NoSort"></on_dgr:OnitBoundColumn>
									<asp:BoundColumn DataField="com_descrizione_domicilio" HeaderText="Comune Domicilio"></asp:BoundColumn>
									<on_dgr:OnitBoundColumn DataField="PAZ_INDIRIZZO_DOMICILIO" HeaderText="Indirizzo Domicilio" key="PAZ_INDIRIZZO_DOMICILIO"
										SortDirection="NoSort"></on_dgr:OnitBoundColumn>
								</Columns>
								<HeaderStyle CssClass="Header"></HeaderStyle>
								<SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
								<PagerStyle Visible="False"></PagerStyle>
							</on_dgr:OnitGrid>
                        </td>
						<td width="2"></td>
					</tr>
				</table>
            </dyp:DynamicPanel>

            <on_ofm:OnitFinestraModale ID="fmStatiAnagrafici" Title="<div style=&quot;font-family:'Microsoft Sans Serif';text-align:center; font-size: 11pt;padding-bottom:2px&quot;><img src='../../images/utente.gif'>&nbsp;Stati anagrafici</div>"
                runat="server"  Width="400px" BackColor="LightGray" UseDefaultTab="True" RenderModalNotVisible="True">
                <p class="textbox_data" style="FONT-SIZE: 12px; FONT-FAMILY: Verdana">
                    <b>Selezionare gli stati anagrafici su cui filtrare:</b>
                </p>
                <p class="textbox_data" >
                    <asp:CheckBoxList ID="chlStatiAnagrafici" runat="server" BorderWidth="1px" BorderStyle="Solid" Width="100%"
                        CssClass="label_left" bgcolor="whitesmoke" BorderColor="Navy" >
                    </asp:CheckBoxList>
                </p>
                <p align="center">
                    <asp:Button ID="btnConfermaSelezionaStati" runat="server" Width="100px" Text="OK"></asp:Button>
                    <asp:Button ID="btnAnnullaSelezionaStati" runat="server" Width="100px" Text="Annulla"></asp:Button>
                </p>
            </on_ofm:OnitFinestraModale>
        </on_lay3:OnitLayout3>
    </form>
</body>
</html>
