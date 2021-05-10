<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Risposte.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_Risposte" %>

<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
		<title>Risposte</title>

        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

        <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("onvac.common.js")%>'></script>
		<script type="text/javascript" language="javascript">
		    function InizializzaToolBar(t) {
		        t.PostBackButton = false;
		    }

		    function ToolBarClick(ToolBar, button, evnt) {
		        evnt.needPostBack = true;

		        switch (button.Key) {

		            case 'btnSalva':
		                codice = document.getElementById('txtCodice').value;
		                descrizione = document.getElementById('txtDescrizione').value;
		                if ((codice == "") || (descrizione == "")) {
		                    alert('Attenzione: alcuni campi obbligatori non sono impostati correttamente!');
		                    evnt.needPostBack = false;
		                }
		        }
		    }
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="Onitlayout31" runat="server" height="100%" width="100%" Titolo="Risposte" TitleCssClass="Title3">
				<ondp:OnitDataPanel id="odpRisposte" runat="server" width="100%" renderOnlyChildren="True" maxRecord-Length="3"
					maxRecord="200" ConfigFile="Risposte.odpRisposte.xml" useToolbar="False">
                    <div>
						<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                            <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                            <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
							<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
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
					<div class="Sezione">Modulo ricerca</div>
                    <div>
						<ondp:wzFilter id="WzFilter1" runat="server" Height="70px" CssClass="InfraUltraWebTab2" Width="100%">
							<DefaultTabStyle CssClass="InfraTab_Default2"></DefaultTabStyle>
							<SelectedTabStyle CssClass="InfraTab_Selected2"></SelectedTabStyle>
							<DisabledTabStyle CssClass="InfraTab_Disabled2"></DisabledTabStyle>
							<HoverTabStyle CssClass="InfraTab_Hover2"></HoverTabStyle>
							<Tabs>
								<igtab:Tab Text="Ricerca di Base">
									<ContentTemplate>
										<table height="100%" cellspacing="10" cellpadding="0" width="100%" border="0" style="table-layout:fixed">
											<tr>
												<td align="right" width="90">
													<asp:Label id="Label1" runat="server" CssClass="LABEL">Filtro di Ricerca</asp:Label></td>
												<td>
													<asp:TextBox id="WzFilterKeyBase" runat="server" cssclass="textbox_stringa w100p"></asp:TextBox></td>
											</tr>
										</table>
									</ContentTemplate>
								</igtab:Tab>
							</Tabs>
						</ondp:wzFilter>
					</div>
                    <div class="Sezione">Elenco</div>

                    <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="99%" ScrollBars="Auto" RememberScrollPosition="true">
						<ondp:wzDataGrid Browser="UpLevel" id="dgrRisposte" runat="server" Width="100%" disableActiveRowChange="False" EditMode="None">
							<DisplayLayout ColWidthDefault="" AutoGenerateColumns="False" RowHeightDefault="26px" Version="2.00"
								GridLinesDefault="None" SelectTypeRowDefault="Single" ScrollBar="Never" BorderCollapseDefault="Separate"
								CellPaddingDefault="3" RowSelectorsDefault="No" Name="dgrRisposte" CellClickActionDefault="RowSelect">
								<HeaderStyleDefault CssClass="Infra2Dgr_Header"></HeaderStyleDefault>
								<RowSelectorStyleDefault CssClass="Infra2Dgr_RowSelector"></RowSelectorStyleDefault>
								<FrameStyle Width="100%"></FrameStyle>
								<ActivationObject BorderWidth="0px" BorderColor="Navy"></ActivationObject>
								<SelectedRowStyleDefault CssClass="Infra2Dgr_SelectedRow"></SelectedRowStyleDefault>
								<RowAlternateStyleDefault CssClass="Infra2Dgr_RowAlternate"></RowAlternateStyleDefault>
								<RowStyleDefault CssClass="Infra2Dgr_Row"></RowStyleDefault>
							</DisplayLayout>
							<Bands>
								<igtbl:UltraGridBand>
									<Columns>
										<igtbl:UltraGridColumn HeaderText="&lt;input type=checkbox  OnClick='javascript:wzDataGrid_Master_Click(this,&amp;quot;dgrRisposte&amp;quot;);' /&gt;"
											Key="check" Width="0%" Hidden="True" Type="CheckBox" BaseColumnName="" AllowUpdate="Yes"></igtbl:UltraGridColumn>
										<igtbl:UltraGridColumn HeaderText="Codice" Key="" Width="100px" BaseColumnName=""></igtbl:UltraGridColumn>
										<igtbl:UltraGridColumn HeaderText="Descrizione" Key="" Width="90%" BaseColumnName=""></igtbl:UltraGridColumn>
										<igtbl:UltraGridColumn HeaderText="Obsoleto" Key="" Width="10%" BaseColumnName="">
											<CellButtonStyle HorizontalAlign="Center"></CellButtonStyle>
										</igtbl:UltraGridColumn>
									</Columns>
								</igtbl:UltraGridBand>
							</Bands>
							<BindingColumns>
								<ondp:BindingFieldValue Value="" Editable="always" Description="Codice" Connection="connessioneMaster" SourceTable="T_ANA_RISPOSTE"
									Hidden="False" SourceField="RIS_CODICE"></ondp:BindingFieldValue>
								<ondp:BindingFieldValue Value="" Editable="always" Description="Descrizione" Connection="connessioneMaster"
									SourceTable="T_ANA_RISPOSTE" Hidden="False" SourceField="RIS_DESCRIZIONE"></ondp:BindingFieldValue>
								<ondp:BindingFieldValue Value="" Editable="always" Description="Obsoleto" Connection="connessioneMaster"
									SourceTable="T_ANA_CODIFICHE" Hidden="False" SourceField="COD_DESCRIZIONE"></ondp:BindingFieldValue>
							</BindingColumns>
						</ondp:wzDataGrid>
                    </dyp:DynamicPanel>

					<div class="Sezione">Dettaglio</div>
                    <div>
						<table id="Table1" style="table-layout: fixed" cellspacing="3" cellpadding="0" width="100%" border="0">
							<tr>
								<td class="label" width="80">Codice</td>
								<td>
									<ondp:wzTextBox id="txtCodice" onblur="toUpper(this);controlloCampoCodice(this);" runat="server"
										maxlength="12" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
										CssStyles-CssEnabled="TextBox_Stringa  w100p" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
										BindingField-Hidden="False" BindingField-Editable="onNew" BindingField-Connection="connessioneMaster"
										BindingField-SourceTable="T_ANA_RISPOSTE" BindingField-SourceField="RIS_CODICE"></ondp:wzTextBox></td>
							</tr>
							<tr>
								<td class="label" width="80">Descrizione</td>
								<td>
									<ondp:wzTextBox id="txtDescrizione" onblur="toUpper(this);" runat="server"
										maxlength="200" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
										CssStyles-CssEnabled="TextBox_Stringa  w100p" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
										BindingField-Hidden="False" BindingField-Editable="always" BindingField-Connection="connessioneMaster"
										BindingField-SourceTable="T_ANA_RISPOSTE" BindingField-SourceField="RIS_DESCRIZIONE"></ondp:wzTextBox></td>
							</tr>
							<tr>
								<td class="label" width="80">Obsoleto</td>
								<td>
									<ondp:wzDropDownList id="WzDropDownList1" runat="server" 
										CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100" CssStyles-CssEnabled="TextBox_Stringa  w100"
										CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100" BindingField-Hidden="False" BindingField-Editable="always"
										BindingField-Connection="connessioneMaster" BindingField-SourceTable="T_ANA_RISPOSTE" BindingField-SourceField="RIS_OBSOLETO"
										TextFieldName="COD_DESCRIZIONE" KeyFieldName="COD_CODICE" SourceTable="T_ANA_CODIFICHE" OtherListFields="cod_campo"
										DataFilter="cod_campo='RIS_OBSOLETO'"></ondp:wzDropDownList></td>
							</tr>
						</table>
                    </div>
				</ondp:OnitDataPanel>
			</on_lay3:onitlayout3>
        </form>
	</body>
</html>
