<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Vie.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_Vie"%>

<%@ Register TagPrefix="on_otb" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitTable" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
		<title>Vie</title>
		
        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
		
        <script type="text/javascript" language="javascript">
	
		function InizializzaToolBar(t)
		{
			t.PostBackButton=false;
		}
		
		function ToolBarClick(ToolBar,button,evnt){
			evnt.needPostBack=true;
			
			switch(button.Key)
			{
		
				case 'btnSalva':
					codice = document.getElementById('WzTextBox1').value;
					if (codice=="")
					{
						alert('Attenzione: alcuni campi obbligatori non sono impostati correttamente!');
						evnt.needPostBack=false;
					};
				case 'btnCerca':
					document.getElementById('WzFilterKeyBase').value = '%' + document.getElementById('WzFilterKeyBase').value;
					
			}
		}		
		
		function ControllaNumero(txt)
		{
			var numero;
			numero = txt.value;
			
			if (numero != '')
			{
				if ((numero.search('1') == -1) && (numero.search('2') == -1) && (numero.search('3') == -1) && (numero.search('4') == -1) && (numero.search('5') == -1) && (numero.search('6') == -1) && (numero.search('7') == -1) && (numero.search('8') == -1) && (numero.search('9') == -1))
				{
					alert('Attenzione: immettere un numero valido!');
					txt.focus();
				}
			}
			
			toUpper(txt);
		}
				
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="Onitlayout31" runat="server" TitleCssClass="Title3" Titolo="Vie" width="100%" height="100%">
				<ondp:OnitDataPanel id="odpVie" runat="server" width="100%" useToolbar="False" ConfigFile="Vie.odpVie.xml"
					renderOnlyChildren="True" maxRecord-Length="3" maxRecord="200">
					<on_otb:onittable id="OnitTable" runat="server" height="100%" width="100%">
						<on_otb:onitsection id="secRicerca" runat="server" width="100%" typeHeight="content">
							<on_otb:onitcell id="celRicerca" runat="server" height="100%">
								<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		                            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
									<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
									<Items>
										<igtbar:TBarButton Key="btnCerca" Text="Cerca"></igtbar:TBarButton>
										<igtbar:TBSeparator></igtbar:TBSeparator>
										<igtbar:TBarButton Key="btnNew" Text="Nuovo"></igtbar:TBarButton>
										<igtbar:TBarButton Key="btnCopy" Text="Duplica"></igtbar:TBarButton>
										<igtbar:TBarButton Key="btnEdit" Text="Modifica"></igtbar:TBarButton>
										<igtbar:TBarButton Key="btnElimina" Text="Elimina"></igtbar:TBarButton>
										<igtbar:TBSeparator></igtbar:TBSeparator>
										<igtbar:TBarButton Key="btnSalva" Text="Salva"></igtbar:TBarButton>
										<igtbar:TBarButton Key="btnAnnulla" Text="Annulla"></igtbar:TBarButton>
									</Items>
								</igtbar:UltraWebToolbar>
								<div class="Sezione">Modulo ricerca</div>
								<ondp:wzFilter id="filFiltro" runat="server" Height="70px" Width="100%" CssClass="InfraUltraWebTab2" 
								    SearchOnlyFields="VieMaster.T_ANA_VIE.VIA_CODICE, VieMaster.T_ANA_VIE.VIA_DESCRIZIONE, VieMaster.T_ANA_VIE.VIA_CIRCOSCRIZIONE, VieMaster.T_ANA_VIE.VIA_DIS_CODICE, VieMaster.T_ANA_VIE.VIA_CAP, VieMaster.T_ANA_CIRCOSCRIZIONI.CIR_DESCRIZIONE, VieMaster.T_ANA_DISTRETTI.DIS_DESCRIZIONE, VieMaster.T_ANA_COMUNI.COM_DESCRIZIONE">
									<DefaultTabStyle CssClass="InfraTab_Default2"></DefaultTabStyle>
									<SelectedTabStyle CssClass="InfraTab_Selected2"></SelectedTabStyle>
									<DisabledTabStyle CssClass="InfraTab_Disabled2"></DisabledTabStyle>
									<HoverTabStyle CssClass="InfraTab_Hover2"></HoverTabStyle>
									<Tabs>
										<igtab:Tab Text="Ricerca di Base">
											<ContentTemplate>
												<table height="100%" cellspacing="10" cellpadding="0" width="100%" border="0" style="table-layout:fixed">
                                                    <colgroup>
                                                        <col align="right" width="90" />
                                                        <col />
                                                        <col width="100px"/>
                                                    </colgroup>
													<tr>
														<td>
															<asp:Label id="Label1" runat="server" CssClass="LABEL">Filtro di Ricerca</asp:Label>
                                                        </td>
														<td>
															<asp:TextBox id="WzFilterKeyBase" runat="server" cssclass="textbox_stringa w100p"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:CheckBox id="WzFilterFraseIntera" runat="server" Text="Frase intera" cssclass="label" TextAlign="Left"></asp:CheckBox>
                                                        </td>
													</tr>
												</table>
											</ContentTemplate>
										</igtab:Tab>
									</Tabs>
								</ondp:wzFilter>
								<div class="Sezione">Elenco</div>
							</on_otb:onitcell>
						</on_otb:onitsection>
						<on_otb:onitsection id="secElenco" runat="server" width="100%" typeHeight="calculate">
							<on_otb:onitcell id="celElenco" runat="server" height="100%" typescroll="auto">
								<ondp:wzDataGrid Browser="UpLevel" id="dgrVie" runat="server" Width="100%" PagerVoicesBefore="-1" PagerVoicesAfter="-1"
									OnitStyle="False" disableActiveRowChange="False" EditMode="None">
									<Bands>
										<igtbl:UltraGridBand>
											<Columns>
												<igtbl:UltraGridColumn HeaderText="&lt;input type=checkbox  OnClick='javascript:wzDataGrid_Master_Click(this,&amp;quot;dgrVie&amp;quot;);' /&gt;"
													Key="check" Width="0%" Hidden="True" Type="CheckBox" BaseColumnName="" AllowUpdate="Yes">
													<Footer Key="check"></Footer>
													<Header Key="check" Caption="&lt;input type=checkbox  OnClick='javascript:wzDataGrid_Master_Click(this,&amp;quot;dgrVie&amp;quot;);' /&gt;"></Header>
												</igtbl:UltraGridColumn>
												<igtbl:UltraGridColumn HeaderText="" Key="VIA_PROGRESSIVO" Width="0px" BaseColumnName="" Hidden="True">
													<Footer Key="VIA_PROGRESSIVO"></Footer>
												</igtbl:UltraGridColumn>
												<igtbl:UltraGridColumn HeaderText="Codice" Key="VIA_CODICE" Width="200px" BaseColumnName="">
													<Footer Key="VIA_CODICE"></Footer>
												</igtbl:UltraGridColumn>
												<igtbl:UltraGridColumn HeaderText="Descrizione" Key="VIA_DESCRIZIONE" Width="60%" BaseColumnName="">
													<Footer Key="VIA_DESCRIZIONE"></Footer>
													<HeaderStyle HorizontalAlign="Left"></HeaderStyle>
												</igtbl:UltraGridColumn>
												<igtbl:UltraGridColumn HeaderText="Comune" Key="COM_DESCRIZIONE" Width="20%" BaseColumnName="">
													<Footer Key="COM_DESCRIZIONE"></Footer>
													<HeaderStyle HorizontalAlign="Left"></HeaderStyle>
												</igtbl:UltraGridColumn>
												<igtbl:UltraGridColumn HeaderText="Cap" Key="VIA_CAP" Width="100px" BaseColumnName="">
													<Footer Key="VIA_CAP"></Footer>
													<HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                                    <CellStyle HorizontalAlign="Left"></CellStyle>
												</igtbl:UltraGridColumn>
												<igtbl:UltraGridColumn HeaderText="Distretto" Key="VIA_DIS_CODICE" Width="100px" BaseColumnName="">
													<Footer Key="VIA_DIS_CODICE"></Footer>
													<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                                    <CellStyle HorizontalAlign="Center"></CellStyle>
												</igtbl:UltraGridColumn>
												<igtbl:UltraGridColumn HeaderText="Circoscrizione" Key="VIA_CIRCOSCRIZIONE" Width="100px" BaseColumnName="">
													<CellStyle HorizontalAlign="Center"></CellStyle>
													<Footer Key="VIA_CIRCOSCRIZIONE"></Footer>
													<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
												</igtbl:UltraGridColumn>
												<igtbl:UltraGridColumn HeaderText="Da" Key="VIA_CIVICO_DA" Width="10%" BaseColumnName="">
													<CellStyle HorizontalAlign="Right"></CellStyle>
													<Footer Key="VIA_CIVICO_DA"></Footer>
													<HeaderStyle HorizontalAlign="Right"></HeaderStyle>
												</igtbl:UltraGridColumn>
												<igtbl:UltraGridColumn HeaderText="A" Key="VIA_CIVICO_A" Width="10%" BaseColumnName="">
													<CellStyle HorizontalAlign="Right"></CellStyle>
													<Footer Key="VIA_CIVICO_A"></Footer>
                                                    <HeaderStyle HorizontalAlign="Right"></HeaderStyle>
												</igtbl:UltraGridColumn>
												<igtbl:UltraGridColumn HeaderText="Tipo" Key="VIA_TIPO_NUMERO" Width="50px" BaseColumnName="">
													<CellStyle HorizontalAlign="Center"></CellStyle>
													<Footer Key="VIA_TIPO_NUMERO"></Footer>
													<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
												</igtbl:UltraGridColumn>
											</Columns>
										</igtbl:UltraGridBand>
									</Bands>
									<DisplayLayout ColWidthDefault="" AutoGenerateColumns="False" RowHeightDefault="26px" Version="2.00"
										GridLinesDefault="None" SelectTypeRowDefault="Single" ScrollBar="Never" BorderCollapseDefault="Separate"
										CellPaddingDefault="3" RowSelectorsDefault="No" Name="dgrVie" CellClickActionDefault="RowSelect">
										<HeaderStyleDefault CssClass="Infra2Dgr_Header"></HeaderStyleDefault>
										<RowSelectorStyleDefault CssClass="Infra2Dgr_RowSelector"></RowSelectorStyleDefault>
										<FrameStyle Width="100%"></FrameStyle>
										<ActivationObject BorderWidth="0px" BorderColor="Navy"></ActivationObject>
										<SelectedRowStyleDefault CssClass="Infra2Dgr_SelectedRow"></SelectedRowStyleDefault>
										<RowAlternateStyleDefault CssClass="Infra2Dgr_RowAlternate"></RowAlternateStyleDefault>
										<RowStyleDefault CssClass="Infra2Dgr_Row"></RowStyleDefault>
									</DisplayLayout>
									<BindingColumns>
										<ondp:BindingFieldValue Value="" Editable="always" Description="Progressivo" Connection="VieMaster" SourceTable="T_ANA_VIE"
											Hidden="True" SourceField="VIA_PROGRESSIVO"></ondp:BindingFieldValue>
										<ondp:BindingFieldValue Value="" Editable="always" Description="Codice" Connection="VieMaster" SourceTable="T_ANA_VIE"
											Hidden="False" SourceField="VIA_CODICE"></ondp:BindingFieldValue>
										<ondp:BindingFieldValue Value="" Editable="always" Description="Descrizione" Connection="VieMaster" SourceTable="T_ANA_VIE"
											Hidden="False" SourceField="VIA_DESCRIZIONE"></ondp:BindingFieldValue>
										<ondp:BindingFieldValue Value="" Editable="always" Description="Comune" Connection="VieMaster" SourceTable="T_ANA_COMUNI"
											Hidden="False" SourceField="COM_DESCRIZIONE"></ondp:BindingFieldValue>
										<ondp:BindingFieldValue Value="" Editable="always" Description="Cap" Connection="VieMaster" SourceTable="T_ANA_VIE"
											Hidden="False" SourceField="VIA_CAP"></ondp:BindingFieldValue>
										<ondp:BindingFieldValue Value="" Editable="always" Description="Distretto" Connection="VieMaster" SourceTable="T_ANA_VIE"
											Hidden="False" SourceField="VIA_DIS_CODICE"></ondp:BindingFieldValue>
										<ondp:BindingFieldValue Value="" Editable="always" Description="Circoscrizione" Connection="VieMaster" SourceTable="T_ANA_VIE"
											Hidden="False" SourceField="VIA_CIRCOSCRIZIONE"></ondp:BindingFieldValue>
										<ondp:BindingFieldValue Value="" Editable="always" Description="Da num" Connection="VieMaster" SourceTable="T_ANA_VIE"
											Hidden="False" SourceField="VIA_CIVICO_DA"></ondp:BindingFieldValue>
										<ondp:BindingFieldValue Value="" Editable="always" Description="A num" Connection="VieMaster" SourceTable="T_ANA_VIE"
											Hidden="False" SourceField="VIA_CIVICO_A"></ondp:BindingFieldValue>
										<ondp:BindingFieldValue Value="" Editable="always" Description="Tipo" Connection="VieMaster" SourceTable="T_ANA_VIE"
											Hidden="False" SourceField="VIA_TIPO_NUMERO"></ondp:BindingFieldValue>
									</BindingColumns>
								</ondp:wzDataGrid>
							</on_otb:onitcell>
						</on_otb:onitsection>
						<on_otb:onitsection id="secDettaglio" runat="server" width="100%" TypeHeight="Content">
							<on_otb:onitcell id="celdettaglio" runat="server" height="100%">
								<div class="Sezione">
                                    <table border="0" cellpadding="0" cellspacing="0" width="100%" class="Sezione">
                                        <colgroup>
                                            <col width="80%" />
                                            <col width="20%" align="right" />
                                        </colgroup>
                                        <tr>
                                            <td>&nbsp;Dettaglio</td>
                                            <td style="text-align:right;">
                                                <ondp:wzCheckBox id="chkDefault" runat="server" CssStyles-CssEnabled="TextBox_Stringa" CssStyles-CssDisabled="TextBox_Stringa" 
										            BindingField-Editable="always" BindingField-Connection="VieMaster" BindingField-SourceTable="T_ANA_VIE"
										            BindingField-Hidden="False" BindingField-SourceField="VIA_DEFAULT" BindingField-Value="N" Text="Default&nbsp;&nbsp;"></ondp:wzCheckBox>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
								<table style="table-layout: fixed" cellspacing="3" cellpadding="0" width="100%" border="0">
                                    <colgroup>
                                        <col width="7%" align="right" />
                                        <col width="25%" />
                                        <col width="11%" align="right" />
                                        <col width="9%" />
                                        <col width="8%" align="right" />
                                        <col width="8%" align="right" />
                                        <col width="8%" align="right" />
                                        <col width="24%" />
                                    </colgroup>
									<tr>
										<td class="label">Codice</td>
										<td>
											<ondp:wzTextBox id="WzTextBox1" onblur="ControllaNumero(this);" runat="server"  
                                                CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p" CssStyles-CssEnabled="TextBox_Stringa w100p"
												CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p" BindingField-Editable="onNew" BindingField-Connection="VieMaster"
												BindingField-SourceTable="T_ANA_VIE" BindingField-Hidden="False" BindingField-SourceField="VIA_CODICE"
												MaxLength="6"></ondp:wzTextBox>
                                        </td>
										<td class="label">Descrizione</td>
										<td colspan="5">
											<ondp:wzTextBox id="WzTextBox2" onblur="toUpper(this);" runat="server" 
                                                CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p" CssStyles-CssEnabled="TextBox_Stringa w100p"
												CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p" BindingField-Editable="always" BindingField-Connection="VieMaster"
												BindingField-SourceTable="T_ANA_VIE" BindingField-Hidden="False" BindingField-SourceField="VIA_DESCRIZIONE"
												MaxLength="35"></ondp:wzTextBox>
                                        </td>
									</tr>
                                    <tr>
                                        <td class="label">Comune</td>
                                        <td>
                                            <ondp:wzFinestraModale id="fmComuni" runat="server" Width="70%" BindingDescription-Connection="VieMaster"
										        BindingCode-Connection="VieMaster" BindingDescription-SourceField="COM_DESCRIZIONE" BindingCode-SourceField="VIA_COM_CODICE"
										        PosizionamentoFacile="False" LabelWidth="-1px" CodiceWidth="30%" Tabella="T_ANA_COMUNI" Sorting="True"
										        SetUpperCase="True" Paging="True" PageSize="50" MaxRecords="0" CampoCodice="COM_CODICE as CODICE" CampoDescrizione="COM_DESCRIZIONE as DESCRIZIONE"
										        BindingCode-Editable="onNew" BindingCode-SourceTable="T_ANA_VIE" BindingCode-Hidden="False" BindingDescription-Editable="onNew"
										        BindingDescription-SourceTable="T_ANA_COMUNI" BindingDescription-Hidden="False" UseCode="True" UseTableLayout="True"
										        Filtro=""></ondp:wzFinestraModale>
                                        </td>
										<td class="label">Da n&ordm;</td>
										<td>
											<ondp:wzTextBox id="Wztextbox4" onblur="ControllaNumero(this);" runat="server" width="100%" 
                                                CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p" CssStyles-CssEnabled="TextBox_Stringa w100p"
												CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p" BindingField-Editable="always" BindingField-Connection="VieMaster"
												BindingField-SourceTable="T_ANA_VIE" BindingField-Hidden="False" BindingField-SourceField="VIA_CIVICO_DA"
												MaxLength="6"></ondp:wzTextBox>
                                        </td>
										<td class="label">A n&ordm;</td>
										<td>
											<ondp:wzTextBox id="Wztextbox5" onblur="toUpper(this);" runat="server" width="100%" 
                                                CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p" CssStyles-CssEnabled="TextBox_Stringa w100p"
												CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p" BindingField-Editable="always" BindingField-Connection="VieMaster"
												BindingField-SourceTable="T_ANA_VIE" BindingField-Hidden="False" BindingField-SourceField="VIA_CIVICO_A"
												MaxLength="6"></ondp:wzTextBox>
                                        </td>
										<td class="label">Numeri</td>
										<td>
											<ondp:wzDropDownList id="WzDropDownList1" runat="server" width="100%" 
                                                CssStyles-CssRequired="TextBox_Stringa" CssStyles-CssEnabled="TextBox_Stringa" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato"
												BindingField-Editable="always" BindingField-Connection="VieMaster" BindingField-SourceTable="T_ANA_VIE"
												BindingField-Hidden="False" BindingField-SourceField="VIA_TIPO_NUMERO" SourceTable="T_ANA_CODIFICHE"
												TextFieldName="COD_DESCRIZIONE" IncludeNull="False" KeyFieldName="COD_CODICE" DataFilter="cod_campo='VIA_TIPO_NUMERO'"
												OtherListFields="COD_CAMPO"></ondp:wzDropDownList>
                                        </td>
                                    </tr>
									<tr>
										<td class="label">CAP</td>
										<td>
											<ondp:wzTextBox id="txtCap" runat="server" width="100%" 
                                                CssStyles-CssRequired="TextBox_Stringa" CssStyles-CssEnabled="TextBox_Stringa" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato" 
                                                BindingField-Editable="always" BindingField-Connection="VieMaster" BindingField-SourceTable="T_ANA_VIE" 
                                                BindingField-Hidden="False" BindingField-SourceField="VIA_CAP" MaxLength="5"></ondp:wzTextBox>
                                        </td>
										<td class="label">Circoscrizione</td>
										<td colspan="3">
                                            <ondp:wzFinestraModale id="fmCircoscrizioni" runat="server" Width="70%" BindingDescription-Connection="VieMaster"
										        BindingCode-Connection="VieMaster" BindingDescription-SourceField="CIR_DESCRIZIONE" BindingCode-SourceField="VIA_CIRCOSCRIZIONE"
										        PosizionamentoFacile="False" LabelWidth="-1px" CodiceWidth="30%" Tabella="T_ANA_CIRCOSCRIZIONI" Sorting="True"
										        SetUpperCase="True" Paging="True" PageSize="50" MaxRecords="0" CampoCodice="CIR_CODICE as CODICE" CampoDescrizione="CIR_DESCRIZIONE as DESCRIZIONE"
										        BindingCode-Editable="always" BindingCode-SourceTable="T_ANA_VIE" BindingCode-Hidden="False" BindingDescription-Editable="always"
										        BindingDescription-SourceTable="T_ANA_CIRCOSCRIZIONI" BindingDescription-Hidden="False" UseCode="True" UseTableLayout="True"
										        Filtro=""></ondp:wzFinestraModale>
                                        </td>
										<td class="label">Distretto</td>
										<td>
                                            <ondp:wzFinestraModale id="fmDistretti" runat="server" Width="70%" BindingDescription-Connection="VieMaster"
										        BindingCode-Connection="VieMaster" BindingDescription-SourceField="DIS_DESCRIZIONE" BindingCode-SourceField="VIA_DIS_CODICE"
										        PosizionamentoFacile="False" LabelWidth="-1px" CodiceWidth="30%" Tabella="T_ANA_DISTRETTI" Sorting="True"
										        SetUpperCase="True" Paging="True" PageSize="50" MaxRecords="0" CampoCodice="DIS_CODICE as CODICE" CampoDescrizione="DIS_DESCRIZIONE as DESCRIZIONE"
										        BindingCode-Editable="always" BindingCode-SourceTable="T_ANA_VIE" BindingCode-Hidden="False" BindingDescription-Editable="always"
										        BindingDescription-SourceTable="T_ANA_DISTRETTI" BindingDescription-Hidden="False" UseCode="True" UseTableLayout="True"
										        Filtro=""></ondp:wzFinestraModale>
                                        </td>
									</tr>
								</table>
							</on_otb:onitcell>
						</on_otb:onitsection>
					</on_otb:onittable>
				</ondp:OnitDataPanel>
			</on_lay3:onitlayout3>
		</form>
	</body>
</html>
