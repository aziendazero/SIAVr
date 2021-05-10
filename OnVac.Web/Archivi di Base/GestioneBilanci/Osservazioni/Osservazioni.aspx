<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Osservazioni.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_Osservazioni" %>

<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
		<title>Vaccinazioni</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

		<style type="text/css">
		    .w100 { width: 100px }
		</style>

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
		                var da = OnitDataPickGet('WzInizioValidita');
		                var a = OnitDataPickGet('WzFineValidita');
		                if (da == '' ) {
		                    alert("Salvataggio non effettuato: la data di inizio validità è obbligatoria.");
		                    evnt.needPostBack = false;
		                    return;
		                }                    
		                if (confrontaDate(da, a) == -1) {
		                    alert("Salvataggio non effettuato: la data di inizio validità deve essere inferiore a quella di fine validità.");
		                    evnt.needPostBack = false;
		                    return;
		                }
		                
		                break;
		        }
		    }
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="Onitlayout31" runat="server" name="Onitlayout31" height="100%" width="100%" Titolo="Osservazioni" TitleCssClass="Title3">
				<ondp:OnitDataPanel id="odpOsservazioni" runat="server" width="100%" useToolbar="False" maxRecord="200" maxRecord-Length="3" 
                    renderOnlyChildren="True" ConfigFile="Osservazioni.odpOsservazioni.xml" FieldBindingMode="BindControlAsColumn">
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
							    <igtbar:TBSeparator></igtbar:TBSeparator>
							    <igtbar:TBarButton Key="btnLinkRisposte" Text="Risposte" Image="~/Images/info.gif"></igtbar:TBarButton>
						    </Items>
					    </igtbar:UltraWebToolbar>
                    </div>
                    <div class="Sezione">Modulo ricerca</div>
                    <div>
						<ondp:wzFilter id="filFiltro" runat="server" Height="70px" TablesForIndexSearch="T_ANA_OSSERVAZIONI"
							Width="100%" CssClass="InfraUltraWebTab2">
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
                    
                    <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
						<ondp:wzDataGrid Browser="UpLevel" id="dgrOsservazioni" runat="server" Width="100%" EditMode="None" OnitStyle="False"
							PagerVoicesAfter="-1" PagerVoicesBefore="-1">
							<DisplayLayout ColWidthDefault="" AutoGenerateColumns="False" RowHeightDefault="26px" Version="2.00"
								GridLinesDefault="None" SelectTypeRowDefault="Single" ScrollBar="Never" BorderCollapseDefault="Separate"
								CellPaddingDefault="3" RowSelectorsDefault="No" Name="dgrOsservazioni" CellClickActionDefault="RowSelect">
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
										<igtbl:UltraGridColumn HeaderText="&lt;input type=checkbox  OnClick='javascript:wzDataGrid_Master_Click(this,&amp;quot;dgrOsservazioni&amp;quot;);' /&gt;"
											Key="check" Hidden="True" Type="CheckBox" BaseColumnName="" AllowUpdate="Yes"></igtbl:UltraGridColumn>
										<igtbl:UltraGridColumn HeaderText="Codice" Key="" Width="15%" BaseColumnName=""></igtbl:UltraGridColumn>
										<igtbl:UltraGridColumn HeaderText="Descrizione" Key="" Width="60%" BaseColumnName=""></igtbl:UltraGridColumn>
										<igtbl:UltraGridColumn HeaderText="Tipo risposta" Key="" Width="20%" BaseColumnName="">
											<CellStyle HorizontalAlign="Center"></CellStyle>
										</igtbl:UltraGridColumn>
										<igtbl:UltraGridColumn HeaderText="Sesso" Key="" Width="100px" BaseColumnName=""></igtbl:UltraGridColumn>
												
									</Columns>
								</igtbl:UltraGridBand>
							</Bands>
							<BindingColumns>
								<ondp:BindingFieldValue Value="" Editable="always" Description="Codice" Connection="OsservazioniMaster"
									SourceTable="T_ANA_OSSERVAZIONI" Hidden="False" SourceField="OSS_CODICE"></ondp:BindingFieldValue>
								<ondp:BindingFieldValue Value="" Editable="always" Description="Descrizione" Connection="OsservazioniMaster"
									SourceTable="T_ANA_OSSERVAZIONI" Hidden="False" SourceField="OSS_DESCRIZIONE"></ondp:BindingFieldValue>
								<ondp:BindingFieldValue Value="" Editable="always" Description="Tipo risposta" Connection="OsservazioniMaster"
									SourceTable="TipoRisposta" Hidden="False" SourceField="COD_DESCRIZIONE"></ondp:BindingFieldValue>
								<ondp:BindingFieldValue Value="" Editable="always" Description="Sesso" Connection="OsservazioniMaster" SourceTable="Sesso"
									Hidden="False" SourceField="COD_DESCRIZIONE"></ondp:BindingFieldValue>
							</BindingColumns>
						</ondp:wzDataGrid>
					</dyp:DynamicPanel>

                    <div class="Sezione">Dettaglio</div>
                    <div>
						<table style="table-layout: fixed" cellspacing="3" cellpadding="0" width="100%" border="0">
                            <colgroup>
                                <col width="80px" />
                                <col />
                                <col width="120px"/>
                                <col />
                                <col width="120px"/>
                                <col />
                            </colgroup>
							<tr>
								<td class="label" align="right" >Codice</td>
								<td colspan="5">
									<ondp:wzTextBox id="txtCodice" onblur="toUpper(this);controlloCampoCodice(this);" runat="server" Width="100%"
										CssStyles-CssRequired="TextBox_Stringa_Obbligatorio" CssStyles-CssEnabled="TextBox_Stringa"
										CssStyles-CssDisabled="TextBox_Stringa_Disabilitato"  BindingField-Editable="onNew"
										BindingField-Connection="OsservazioniMaster" BindingField-SourceTable="T_ANA_OSSERVAZIONI" BindingField-Hidden="False"
										BindingField-SourceField="OSS_CODICE" MaxLength="8"></ondp:wzTextBox></td>
							</tr>
							<tr>
								<td class="label" align="right" >Descrizione</td>
								<td colspan="5">
									<ondp:wzTextBox id="txtDescrizione" onblur="toUpper(this);" runat="server" Width="100%"
										CssStyles-CssRequired="TextBox_Stringa_Obbligatorio" CssStyles-CssEnabled="TextBox_Stringa"
										CssStyles-CssDisabled="TextBox_Stringa_Disabilitato"  BindingField-Editable="always"
										BindingField-Connection="OsservazioniMaster" BindingField-SourceTable="T_ANA_OSSERVAZIONI" BindingField-Hidden="False"
										BindingField-SourceField="OSS_DESCRIZIONE" MaxLength="150"></ondp:wzTextBox></td>
							</tr>
							<tr>
								<td class="label" align="right" >Tipo risposta</td>
								<td align="left">
									<ondp:wzDropDownList id="WzDropDownList1" runat="server" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio" Width="100%"
										CssStyles-CssEnabled="TextBox_Stringa" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato" 
										BindingField-Editable="always" BindingField-Connection="OsservazioniMaster" BindingField-SourceTable="T_ANA_OSSERVAZIONI"
										BindingField-Hidden="False" BindingField-SourceField="OSS_TIPO_RISPOSTA" TextFieldName="COD_DESCRIZIONE"
										KeyFieldName="COD_CODICE" SourceTable="T_ANA_CODIFICHE" DataFilter="cod_campo='OSS_TIPO_RISPOSTA'"
										OtherListFields="cod_campo"></ondp:wzDropDownList>
								</td>
                                <td class="label" align="right">Categoria</td>
								<td align="left">
									<ondp:wzDropDownList id="ddlCategoria" runat="server" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio"
										CssStyles-CssEnabled="TextBox_Stringa" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato"  Width="100%"
										BindingField-Editable="always" BindingField-Connection="OsservazioniMaster" BindingField-SourceTable="T_ANA_OSSERVAZIONI"
										BindingField-Hidden="False" BindingField-SourceField="OSS_FLAG_TIPOLOGIA" TextFieldName="COD_DESCRIZIONE"
										KeyFieldName="COD_CODICE" SourceTable="T_ANA_CODIFICHE" DataFilter="cod_campo='OSS_FLAG_TIPOLOGIA'" OtherListFields="cod_campo"></ondp:wzDropDownList>
								</td>
								<td class="label" align="right">Sesso</td>
								<td align="left">
									<ondp:wzDropDownList id="WzDropDownList2" runat="server" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio" Width="100%"
										CssStyles-CssEnabled="TextBox_Stringa" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato" 
										BindingField-Editable="always" BindingField-Connection="OsservazioniMaster" BindingField-SourceTable="T_ANA_OSSERVAZIONI"
										BindingField-Hidden="False" BindingField-SourceField="OSS_SESSO" TextFieldName="COD_DESCRIZIONE"
										KeyFieldName="COD_CODICE" SourceTable="T_ANA_CODIFICHE" DataFilter="cod_campo='OSS_SESSO'" OtherListFields="cod_campo"></ondp:wzDropDownList>
								</td>
                            </tr>
                            <tr>
                                <td align="right">
								    <asp:Label id="Label4" runat="server" CssClass="label">Inizio Validita</asp:Label>
                                </td>

								<td>
									<ondp:wzOnitDatePick id="WzInizioValidita" runat="server"
										CssStyles-CssDisabled="textbox_data_disabilitato" CssStyles-CssEnabled="textbox_data_obbligatorio" CssStyles-CssRequired="textbox_data_obbligatorio"
										BindingField-Editable="always" BindingField-Connection="cicliDettConn" BindingField-SourceTable="T_ANA_OSSERVAZIONI"
										BindingField-Hidden="False" BindingField-SourceField="OSS_DATA_INIZIO_VALIDITA"></ondp:wzOnitDatePick>
								</td>
								<td align="right">
									<asp:Label id="Label5" runat="server" CssClass="label">Fine Validita</asp:Label></td>
								<td align="left">
									<ondp:wzOnitDatePick id="WzFineValidita" runat="server"
										CssStyles-CssDisabled="textbox_data_disabilitato" CssStyles-CssEnabled="textbox_data" CssStyles-CssRequired="textbox_data_obbligatorio"
										BindingField-Editable="always" BindingField-Connection="cicliDettConn" BindingField-SourceTable="T_ANA_OSSERVAZIONI"
										BindingField-Hidden="False" BindingField-SourceField="OSS_DATA_FINE_VALIDITA"></ondp:wzOnitDatePick>
								</td>
                                <td class="label" align="right" >Tipo dati risposta</td>
								<td align="left">
									<ondp:wzDropDownList id="WzDropDownList3" runat="server" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio" Width="100%"
										CssStyles-CssEnabled="TextBox_Stringa" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato" 
										BindingField-Editable="always" BindingField-Connection="OsservazioniMaster" BindingField-SourceTable="T_ANA_OSSERVAZIONI"
										BindingField-Hidden="False" BindingField-SourceField="OSS_TIPO_DATI_RISPOSTA" TextFieldName="COD_DESCRIZIONE"
										KeyFieldName="COD_CODICE" SourceTable="T_ANA_CODIFICHE" DataFilter="cod_campo='OSS_TIPO_DATI_RISPOSTA'"
										OtherListFields="cod_campo"></ondp:wzDropDownList>
							</tr>
						</table>
					</div>
				</ondp:OnitDataPanel>
			</on_lay3:onitlayout3>
        </form>
	</body>
</html>
