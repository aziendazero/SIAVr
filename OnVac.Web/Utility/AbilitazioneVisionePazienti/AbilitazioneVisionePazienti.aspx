<%@ Page Language="vb" AutoEventWireup="false" Codebehind="AbilitazioneVisionePazienti.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_AbilitazioneVisionePazienti" %>

<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>
<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_otb" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitTable" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>Abilitazione Visione Pazienti</title>

        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

		<script type="text/javascript" >
			function InizializzaToolBar(t)
			{
				t.PostBackButton=false;
			}
			
			function ToolBarClick(ToolBar,button,evnt){
				
				evnt.needPostBack=true;
				
				switch(button.Key)
				{
					case 'btnSalva':
						
						var medicoClientID = '<%=wzfmMedico.ClientID%>';
						var medicoAbilitatoClientID = '<%=wzfmMedAbilitato.ClientID%>';
						var dataInizioClientID = '<%=wzodpInizio.ClientID%>';
						var dataFineClientID = '<%=wzodpFine.ClientID%>';
						
						if (!isValidFinestraModale(medicoClientID ,false)) 
						{ 
							alert("Il campo 'Medico' non era aggiornato.\nRiprovare.");
							document.getElementById(medicoClientID).focus();
							evnt.needPostBack = false;
						}
						
						if (!isValidFinestraModale(medicoAbilitatoClientID, false)) 
						{ 
							alert("Il campo 'Medico Abilitato' non era aggiornato.\nRiprovare.");
							if (evnt.needPostBack)
							{
								document.getElementById(medicoAbilitatoClientID).focus();
								evnt.needPostBack = false;
							}
						}
						
						var medicoElement = document.getElementById(medicoClientID);
						if (medicoElement.value == "")
						{
							alert("Il campo 'Medico' è obbligatorio.");
							if (evnt.needPostBack)
							{
								medicoElement.focus();
								evnt.needPostBack = false;
							}
						}
						
						var medicoAbilitatoElement = document.getElementById(medicoAbilitatoClientID);
						if (medicoAbilitatoElement.value == "")
						{
							alert("Il campo 'Medico Abilitato' è obbligatorio.");
							if (evnt.needPostBack)
							{
								medicoAbilitatoElement.focus();
								evnt.needPostBack = false;
							}
						}
						
						var dataInizio = OnitDataPickGet(dataInizioClientID);
						if (dataInizio=="")
						{
							alert("Il campo 'Data Inizio' è obbligatorio.");
							if (evnt.needPostBack)
							{
								OnitDataPickFocus(dataInizioClientID,1,true);
								evnt.needPostBack = false;
							}
						}
						
						var dataFine = OnitDataPickGet(dataFineClientID);
						if (dataFine=="")
						{
							alert("Il campo 'Data Scadenza' è obbligatorio.");
							if (evnt.needPostBack)
							{
								OnitDataPickFocus(dataFineClientID,1,true);
								evnt.needPostBack = false;
							}							
						}
						
						if (OnitDate_parse(dataInizio)>OnitDate_parse(dataFine))
						{	
							alert("La Data di Scadenza deve essere maggiore a quella di Decorrenza!");
							if (evnt.needPostBack)
							{
								OnitDataPickFocus(dataInizioClientID,1,true);
								evnt.needPostBack = false;
							}
						}
										
						break;
				}
			}
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" TitleCssClass="Title3" Titolo="Abilitazione Visione Pazienti" width="100%" height="100%">
				<ondp:OnitDataPanel id="OdpAbilVisPazMaster" runat="server" ConfigFile="AbilVisPaz.OdpAbilVisPazMaster.xml" Width="100%" Height="100%" renderOnlyChildren="True" useToolbar="False">
					<on_otb:onittable style="z-index: 0" id="OnitTable1" runat="server" height="100%" width="100%">

						<on_otb:onitsection id="OnitSection2" runat="server" width="100%" typeHeight="content">
							<on_otb:onitcell id="OnitCell2" runat="server" height="100%">
								<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBarAbilVisPaz" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                                    <HoverStyle CssClass="infratoolbar_button_hover" ></HoverStyle>
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
								<div class="Sezione">Modulo ricerca</div>
								<ondp:wzFilter style="z-index: 0" id="WzFilter1" runat="server" Width="100%" Height="70px" CssClass="InfraUltraWebTab2" TargetUrl="" Dummy>
									<SelectedTabStyle CssClass="InfraTab_Selected2"></SelectedTabStyle>
									<DisabledTabStyle CssClass="InfraTab_Disabled2"></DisabledTabStyle>
									<HoverTabStyle CssClass="InfraTab_Hover2"></HoverTabStyle>
                                    <DefaultTabStyle CssClass="InfraTab_Default2"></DefaultTabStyle>
									<Tabs>
										<igtab:Tab Text="Ricerca di Base">
											<ContentTemplate>
												<table height="100%" cellSpacing="10" cellPadding="0" width="100%" border="0" style="table-layout:fixed">
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
								<div class="Sezione">Elenco</div>
							</on_otb:onitcell>
						</on_otb:onitsection>

						<on_otb:onitsection id="OnitSection1" runat="server" width="100%" typeHeight="calculate">
							<on_otb:onitcell id="OnitCell1" runat="server" height="100%" typescroll="auto">
								<ondp:wzDataGrid Browser="UpLevel" style="Z-INDEX: 0" id="WzDgrAbilVisPaz" runat="server" Width="100%" ImageDirectory="/ig_common/Images/"
									EditMode="None" OnitStyle="False" PagerVoicesAfter="-1" PagerVoicesBefore="-1">
									<DisplayLayout ColWidthDefault="" AutoGenerateColumns="False" RowHeightDefault="26px" Version="2.00"
										GridLinesDefault="None" SelectTypeRowDefault="Single" ScrollBar="Never" BorderCollapseDefault="Separate"
										CellPaddingDefault="3" RowSelectorsDefault="No" Name="WzDgrAbilVisPaz" CellClickActionDefault="RowSelect">
										<HeaderStyleDefault CssClass="Infra2Dgr_Header"></HeaderStyleDefault>
										<RowSelectorStyleDefault CssClass="Infra2Dgr_RowSelector"></RowSelectorStyleDefault>
										<FrameStyle Width="100%"></FrameStyle>
										<ActivationObject BorderWidth="0px" BorderColor="Navy"></ActivationObject>
										<Images></Images>
										<SelectedRowStyleDefault CssClass="Infra2Dgr_SelectedRow"></SelectedRowStyleDefault>
										<RowAlternateStyleDefault CssClass="Infra2Dgr_RowAlternate"></RowAlternateStyleDefault>
										<RowStyleDefault CssClass="Infra2Dgr_Row"></RowStyleDefault>
									</DisplayLayout>
									<Bands>
										<igtbl:UltraGridBand>
											<Columns>
												<igtbl:UltraGridColumn HeaderText="&lt;input type=checkbox  OnClick='javascript:wzDataGrid_Master_Click(this,&amp;quot;WzDgrAbilVisPaz&amp;quot;);' /&gt;"
													Key="check" Width="0%" AllowUpdate="Yes" Hidden="True" Type="CheckBox">
													<Footer Key="check"></Footer>
													<Header Key="check" Caption="&lt;input type=checkbox  OnClick='javascript:wzDataGrid_Master_Click(this,&amp;quot;WzDgrAbilVisPaz&amp;quot;);' /&gt;"></Header>
												</igtbl:UltraGridColumn>
												<igtbl:UltraGridColumn HeaderText="Medico" Width="35%">
													<Footer Key="">
														<RowLayoutColumnInfo OriginX="1"></RowLayoutColumnInfo>
													</Footer>
													<HeaderStyle HorizontalAlign="Left"></HeaderStyle>
													<Header Key="" Caption="Medico">
														<RowLayoutColumnInfo OriginX="1"></RowLayoutColumnInfo>
													</Header>
												</igtbl:UltraGridColumn>
												<igtbl:UltraGridColumn HeaderText="Medico Abilitato" Width="35%">
													<Footer Key="">
														<RowLayoutColumnInfo OriginX="2"></RowLayoutColumnInfo>
													</Footer>
													<HeaderStyle HorizontalAlign="Left"></HeaderStyle>
													<Header Key="" Caption="Medico Abilitato">
														<RowLayoutColumnInfo OriginX="2"></RowLayoutColumnInfo>
													</Header>
												</igtbl:UltraGridColumn>
												<igtbl:UltraGridColumn HeaderText="Data Decorrenza" Width="150px" Format="dd/MM/yyyy">
													<CellStyle HorizontalAlign="Center"></CellStyle>
													<Footer Key="">
														<RowLayoutColumnInfo OriginX="3"></RowLayoutColumnInfo>
													</Footer>
													<Header Key="" Caption="Data Decorrenza">
														<RowLayoutColumnInfo OriginX="3"></RowLayoutColumnInfo>
													</Header>
												</igtbl:UltraGridColumn>
												<igtbl:UltraGridColumn HeaderText="Data Scadenza" Width="150px" Format="dd/MM/yyyy">
													<CellStyle HorizontalAlign="Center"></CellStyle>
													<Footer Key="">
														<RowLayoutColumnInfo OriginX="4"></RowLayoutColumnInfo>
													</Footer>
													<Header Key="" Caption="Data Scadenza">
														<RowLayoutColumnInfo OriginX="4"></RowLayoutColumnInfo>
													</Header>
												</igtbl:UltraGridColumn>
												<igtbl:UltraGridColumn HeaderText="Utente" Width="30%">
													<Footer Key="">
														<RowLayoutColumnInfo OriginX="5"></RowLayoutColumnInfo>
													</Footer>
													<HeaderStyle HorizontalAlign="Left"></HeaderStyle>
													<Header Key="" Caption="Utente">
														<RowLayoutColumnInfo OriginX="5"></RowLayoutColumnInfo>
													</Header>
												</igtbl:UltraGridColumn>
												<igtbl:UltraGridColumn HeaderText="Data Registrazione" Width="150px" Format="dd/MM/yyyy">
													<CellStyle HorizontalAlign="Center"></CellStyle>
													<Footer Key="">
														<RowLayoutColumnInfo OriginX="6"></RowLayoutColumnInfo>
													</Footer>
													<Header Key="" Caption="Data Registrazione">
														<RowLayoutColumnInfo OriginX="6"></RowLayoutColumnInfo>
													</Header>
												</igtbl:UltraGridColumn>
											</Columns>
										</igtbl:UltraGridBand>
									</Bands>
									<BindingColumns>
										<ondp:BindingFieldValue Value="" Editable="never" Description="Medico" Connection="AbilVisPazMaster" SourceTable="T_ANA_MEDICI"
											Hidden="False" SourceField="MED_DESCRIZIONE"></ondp:BindingFieldValue>
										<ondp:BindingFieldValue Value="" Editable="always" Description="Medico Abilitato" Connection="AbilVisPazMaster"
											SourceTable="MediciAbilitati" Hidden="False" SourceField="MED_DESCRIZIONE"></ondp:BindingFieldValue>
										<ondp:BindingFieldValue Value="" Editable="never" Description="Data Decorrenza" Connection="AbilVisPazMaster"
											SourceTable="T_MED_ABILITAZIONI_VIS_PAZ" Hidden="False" SourceField="MAP_DATA_INIZIO"></ondp:BindingFieldValue>
										<ondp:BindingFieldValue Value="" Editable="never" Description="Data Scadenza" Connection="AbilVisPazMaster"
											SourceTable="T_MED_ABILITAZIONI_VIS_PAZ" Hidden="False" SourceField="MAP_DATA_FINE"></ondp:BindingFieldValue>
										<ondp:BindingFieldValue Value="" Editable="never" Description="Utente" Connection="AbilVisPazMaster" SourceTable="T_ANA_UTENTI"
											Hidden="False" SourceField="UTE_DESCRIZIONE"></ondp:BindingFieldValue>
										<ondp:BindingFieldValue Value="" Editable="never" Description="Data Registrazione" Connection="AbilVisPazMaster"
											SourceTable="T_MED_ABILITAZIONI_VIS_PAZ" Hidden="False" SourceField="MAP_DATA_REGISTRAZIONE"></ondp:BindingFieldValue>
									</BindingColumns>
								</ondp:wzDataGrid>
							</on_otb:onitcell>
						</on_otb:onitsection>

						<on_otb:onitsection id="OnitSection3" runat="server" width="100%" TypeHeight="Content">
							<on_otb:onitcell id="OnitCell3" runat="server" height="100%">
								<div class="Sezione">Dettaglio</div>
								<ondp:OnitDataPanel id="OdpAbilVisPazDetail" runat="server" ConfigFile="AbilVisPaz.OdpAbilVisPazDetail.xml"
									Width="100%" useToolbar="False" BindingFields="(Insieme)" externalToolBar-Length="0" dontLoadDataFirstTime="True">
									<table style="table-layout: fixed" id="Table1" border="0" cellspacing="3" cellpadding="0" width="100%">
										
                                        <colgroup>
											<col width="15%" />
											<col width="50%" />
											<col width="15%" />
											<col width="15%" />
											<col width="5%" />
										</colgroup>
										<tr>
											<td class="label">Medico</td>
											<td colspan="3">
												<ondp:wzFinestraModale id="wzfmMedico" runat="server" Width="70%" UseTableLayout="True" CssStyles-CssRequired="textbox_stringa_obbligatorio"
													CssStyles-CssEnabled="textbox_stringa" CssStyles-CssDisabled="textbox_stringa_disabilitato" Obbligatorio="true"
													DataTypeDescription="Stringa" BindingDescription-Hidden="False" BindingDescription-Editable="onNew"
													UseCode="true" BindingCode-SourceField="MAP_MED_CODICE_MEDICO" BindingCode-Hidden="False" BindingCode-SourceTable="T_MED_ABILITAZIONI_VIS_PAZ"
													BindingCode-Connection="AbilVisPazDetail" BindingCode-Editable="onNew" DataTypeCode="Stringa" RaiseChangeEvent="False"
													PosizionamentoFacile="False" LabelWidth="-8px" CodiceWidth="30%" Tabella="T_ANA_MEDICI" CampoCodice="MED_CODICE"
													CampoDescrizione="MED_DESCRIZIONE" SetUpperCase="True" BindingDescription-Connection="AbilVisPazDetail"
													BindingDescription-SourceTable="T_ANA_MEDICI" BindingDescription-SourceField="MED_DESCRIZIONE"></ondp:wzFinestraModale></td>
											<td></td>
										</tr>
										<tr>
											<td class="label">Medico Abilitato</td>
											<td colspan="3">
												<ondp:wzFinestraModale id="wzfmMedAbilitato" runat="server" Width="70%" UseTableLayout="True" CssStyles-CssRequired="textbox_stringa_obbligatorio"
													CssStyles-CssEnabled="textbox_stringa" CssStyles-CssDisabled="textbox_stringa_disabilitato" Obbligatorio="true"
													DataTypeDescription="Stringa" BindingDescription-Hidden="False" BindingDescription-Editable="always"
													UseCode="true" BindingCode-SourceField="MAP_MED_CODICE_ABILITATO" BindingCode-Hidden="False" BindingCode-SourceTable="T_MED_ABILITAZIONI_VIS_PAZ"
													BindingCode-Connection="AbilVisPazDetail" BindingCode-Editable="always" DataTypeCode="Stringa" RaiseChangeEvent="False"
													PosizionamentoFacile="False" LabelWidth="-8px" CodiceWidth="30%" Tabella="T_ANA_MEDICI" CampoCodice="MED_CODICE"
													CampoDescrizione="MED_DESCRIZIONE" SetUpperCase="True" BindingDescription-Connection="AbilVisPazDetail"
													BindingDescription-SourceTable="MediciAbilitati" BindingDescription-SourceField="MED_DESCRIZIONE"></ondp:wzFinestraModale></td>
											<td></td>
										</tr>
										<tr>
											<td class="label" noWrap>Data Decorrenza</td>
											<td>
												<ondp:wzOnitDatePick id="wzodpInizio" runat="server" height="22px" Width="130px" CssStyles-CssRequired="textbox_data_obbligatorio"
													CssStyles-CssEnabled="textbox_data" CssStyles-CssDisabled="textbox_data_disabilitato" BindingField-SourceField="MAP_DATA_INIZIO"
													BindingField-Hidden="False" BindingField-SourceTable="T_MED_ABILITAZIONI_VIS_PAZ" BindingField-Connection="AbilVisPazDetail"
													BindingField-Editable="always" BorderColor="White"></ondp:wzOnitDatePick></td>
											<td class="label" noWrap>Data Scadenza</td>
											<td>
												<ondp:wzOnitDatePick id="wzodpFine" runat="server" height="22px" Width="130px" CssStyles-CssRequired="textbox_data_obbligatorio"
													CssStyles-CssEnabled="textbox_data" CssStyles-CssDisabled="textbox_data_disabilitato" BindingField-SourceField="MAP_DATA_FINE"
													BindingField-Hidden="False" BindingField-SourceTable="T_MED_ABILITAZIONI_VIS_PAZ" BindingField-Connection="AbilVisPazDetail"
													BindingField-Editable="always" BorderColor="White"></ondp:wzOnitDatePick></td>
											<td></td>
										</tr>
										<tr>
											<td class="label">Utente</td>
											<td>
												<ondp:wzFinestraModale id="wzfmUtente" runat="server" Width="100%" AllowBindingDescription="False" autoRefreshDataBind="True"
													UseTableLayout="True" CssStyles-CssRequired="textbox_stringa_obbligatorio" CssStyles-CssEnabled="textbox_stringa"
													CssStyles-CssDisabled="textbox_stringa_disabilitato" Obbligatorio="False" DataTypeDescription="Stringa"
													BindingDescription-Hidden="True" BindingDescription-Editable="never" UseCode="false" BindingCode-SourceField="MAP_UTE_ID_UTENTE"
													BindingCode-Hidden="False" BindingCode-SourceTable="T_MED_ABILITAZIONI_VIS_PAZ" BindingCode-Connection="AbilVisPazDetail"
													BindingCode-Editable="never" DataTypeCode="Numero" RaiseChangeEvent="False" PosizionamentoFacile="False"
													LabelWidth="-8px" CodiceWidth="0%" Tabella="T_ANA_UTENTI" CampoCodice="UTE_ID" CampoDescrizione="UTE_DESCRIZIONE"
													SetUpperCase="True"></ondp:wzFinestraModale></td>
											<td class="label" noWrap>Data Registrazione</td>
											<td>
												<ondp:wzOnitDatePick id="wzodpReg" runat="server" height="22px" Width="130px" CssStyles-CssRequired="textbox_data_obbligatorio"
													CssStyles-CssEnabled="textbox_data" CssStyles-CssDisabled="textbox_data_disabilitato" BindingField-SourceField="MAP_DATA_REGISTRAZIONE"
													BindingField-Hidden="False" BindingField-SourceTable="T_MED_ABILITAZIONI_VIS_PAZ" BindingField-Connection="AbilVisPazDetail"
													BindingField-Editable="never" BorderColor="White"></ondp:wzOnitDatePick></td>
											<td></td>
										</tr>
									</table>
								</ondp:OnitDataPanel>
							</on_otb:onitcell>
						</on_otb:onitsection>

					</on_otb:onittable>
				</ondp:OnitDataPanel>
			</on_lay3:onitlayout3>
		</form>
	</body>
</html>
