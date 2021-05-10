<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Categorie.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_Categorie" %>

<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>Categorie</title>
    
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/archivi.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("onvac.common.js")%>'></script>
    
    <!-- patch per il click sul checkbox dell'header per la selezione di tutti i checkbox -->
    <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("patch_selectAll.js")%>'></script>
</head>
<body onload="registerCheckClick('dgrMaster__ctl1_ChkMultiSel');">
    <form id="Form1" method="post" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Width="100%" Height="100%" TitleCssClass="Title3" Titolo="Categorie">
            <ondp:onitdatapanel id="odpCategorieMaster" runat="server" usetoolbar="False" configfile="Categorie.odpCategorieMaster.xml" renderonlychildren="True">
				<div>
					<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                        <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                        <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
	                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
						<Items>
							<igtbar:TBarButton Key="btnCerca" Text="Cerca"></igtbar:TBarButton>
							<igtbar:TBSeparator></igtbar:TBSeparator>
							<igtbar:TBarButton Key="btnNew" Text="Nuovo"></igtbar:TBarButton>
							<igtbar:TBarButton Key="btnEdit" Text="Modifica"></igtbar:TBarButton>
							<igtbar:TBarButton Key="btnElimina" Text="Elimina"></igtbar:TBarButton>
							<igtbar:TBSeparator></igtbar:TBSeparator>
							<igtbar:TBarButton Key="btnSalva" Text="Salva"></igtbar:TBarButton>
							<igtbar:TBarButton Key="btnAnnulla" Text="Annulla"></igtbar:TBarButton>
						</Items>
					</igtbar:UltraWebToolbar>
                </div>
                <div class="Sezione">Ricerca categoria</div>
                <div>
					<ondp:wzFilter id="filFiltro" runat="server" Height="70px" Width="100%" CssClass="InfraUltraWebTab2">
						<DefaultTabStyle CssClass="InfraTab_Default2"></DefaultTabStyle>
						<SelectedTabStyle CssClass="InfraTab_Selected2"></SelectedTabStyle>
						<DisabledTabStyle CssClass="InfraTab_Disabled2"></DisabledTabStyle>
						<HoverTabStyle CssClass="InfraTab_Hover2"></HoverTabStyle>
						<Tabs>
							<igtab:Tab Text="Ricerca di Base">
								<ContentTemplate>
									<table height="100%" cellspacing="10" cellpadding="0" width="100%" border="0" style="table-layout: fixed">
										<tr>
											<td align="right" width="90">
												<asp:Label id="Label1" runat="server" CssClass="label">Filtro di Ricerca</asp:Label></td>
											<td>
												<asp:TextBox id="WzFilterKeyBase" runat="server" cssclass="textbox_stringa w100p"></asp:TextBox></td>
										</tr>
									</table>
								</ContentTemplate>
							</igtab:Tab>
						</Tabs>
					</ondp:wzFilter>
                </div>
                <div class="Sezione">Elenco categorie</div>

                <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                    <ondp:wzMsDataGrid id="dgrMaster" runat="server" Width="100%" OnitStyle="False" EditMode="None" 
                        PagerVoicesAfter="-1" PagerVoicesBefore="-1" AutoGenerateColumns="False" SelectionOption="rowClick">
                        <HeaderStyle CssClass="header"></HeaderStyle>
                        <ItemStyle CssClass="item"></ItemStyle>
                        <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
                        <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
                        <EditItemStyle CssClass="edit"></EditItemStyle>
                        <FooterStyle CssClass="footer"></FooterStyle>
                        <PagerStyle CssClass="pager"></PagerStyle>
                        <Columns>
                            <ondp:wzMultiSelColumn></ondp:wzMultiSelColumn>
                            <ondp:wzBoundColumn HeaderText="Codice" Key="CAT_CODICE" SourceField="CAT_CODICE" SourceTable="T_ANA_CATEGORIE1" SourceConn="Categorie">
                                <HeaderStyle width="10%" />
                            </ondp:wzBoundColumn>
                            <ondp:wzBoundColumn HeaderText="Descrizione" Key="CAT_DESCRIZIONE" SourceField="CAT_DESCRIZIONE" SourceTable="T_ANA_CATEGORIE1" SourceConn="Categorie">
                                <HeaderStyle width="90%" />
                            </ondp:wzBoundColumn>
                        </Columns>
						<BindingColumns>
							<ondp:BindingFieldValue Value="" Editable="onNew" Description="Codice" Connection="Categorie" SourceTable="T_ANA_CATEGORIE1"
								Hidden="False" SourceField="CAT_CODICE"></ondp:BindingFieldValue>
							<ondp:BindingFieldValue Value="" Editable="always" Description="Descrizione" Connection="Categorie" SourceTable="T_ANA_CATEGORIE1"
								Hidden="False" SourceField="CAT_DESCRIZIONE"></ondp:BindingFieldValue>
						</BindingColumns>
					</ondp:wzMsDataGrid>
                </dyp:DynamicPanel>

                <div class="Sezione">Dettaglio</div>
                <div>
					<ondp:OnitDataPanel id="odpCategorieDetail" runat="server" useToolbar="False" ConfigFile="Categorie.odpCategorieDetail.xml"
						renderOnlyChildren="True" Width="100%" dontLoadDataFirstTime="True" externalToolBar-Length="0" BindingFields="(Insieme)">
						<table id="Table1" style="table-layout: fixed" cellspacing="3" cellpadding="0" width="100%" border="0">
							<colgroup>
								<col width="15%" />
								<col width="85%" />
							</colgroup>
							<tr>
								<td class="label">Codice</td>
								<td>
									<ondp:wzTextBox id="txtCodice" runat="server" onblur="toUpper(this);" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
										CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
										maxlength="8" BindingField-SourceField="CAT_CODICE" BindingField-Hidden="False" BindingField-SourceTable="T_ANA_CATEGORIE1"
										BindingField-Connection="CategorieDetail" BindingField-Editable="onNew"></ondp:wzTextBox></td>
							</tr>
							<tr>
								<td class="label">Descrizione</td>
								<td>
									<ondp:wzTextBox id="txtDescrizione" runat="server" onblur="toUpper(this);" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
										CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
										maxlength="35" BindingField-SourceField="CAT_DESCRIZIONE" BindingField-Hidden="False" BindingField-SourceTable="T_ANA_CATEGORIE1"
										BindingField-Connection="CategorieDetail" BindingField-Editable="always" TextMode="MultiLine"></ondp:wzTextBox></td>
							</tr>
						</table>
					</ondp:OnitDataPanel>
                </div>
			</ondp:onitdatapanel>
        </on_lay3:OnitLayout3>
    </form>
</body>
</html>
