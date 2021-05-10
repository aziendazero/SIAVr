<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Cicli.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_Cicli"%>

<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
		<title>Cicli</title>

        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/archivi.css") %>' type="text/css" rel="stylesheet" />

		<style type="text/css">
            .w50PX {
	            width: 50px
            }
		</style>

        <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("onvac.common.js")%>'></script>

        <!-- patch per il click sul checkbox dell'header per la selezione di tutti i checkbox -->
        <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("patch_selectAll.js")%>'></script>

        <script type="text/javascript" language="javascript">
            function InizializzaToolBar(t) {
                t.PostBackButton = false;
            }
			
			function ToolBarClick(ToolBar,button,evnt) {

			    evnt.needPostBack = true;

				switch(button.Key) {
					case 'btn_Stampa':
						if (<% = OdpCicliMaster.getCurrentDataTable.Rows.Count %>==0) {
							alert("Attenzione: l'elenco non contiene alcun ciclo. Impossibile eseguire la stampa.");
							evnt.needPostBack=false;
						}
					break;
				}
			}
		</script>
	</head>
	<body onload="registerCheckClick('WzDgrCicli__ctl1_ChkMultiSel')">
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="LayoutCicli" runat="server" height="100%" width="100%" Titolo="Cicli" TitleCssClass="Title3">
				<ondp:OnitDataPanel id="OdpCicliMaster" runat="server" width="100%" useToolbar="False" ConfigFile="Cicli.OdpCicliMaster.xml" renderOnlyChildren="True">
                    <div>
						<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBarCicli" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
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
								<igtbar:TBarButton Key="btnSedute" Text="Sedute" Image="../../../images/inoculazione.png"></igtbar:TBarButton>
								<igtbar:TBSeparator></igtbar:TBSeparator>
								<igtbar:TBarButton Key="btn_Stampa" Text="Stampa" Image="~/Images/stampa.gif"></igtbar:TBarButton>
							</Items>
						</igtbar:UltraWebToolbar>
                    </div>
					<div class="Sezione">Modulo ricerca</div>
                    <div>
						<ondp:wzFilter id="WzFilter1" runat="server" Height="70px" Width="100%"  CssClass="InfraUltraWebTab2">
							<DefaultTabStyle CssClass="InfraTab_Default2"></DefaultTabStyle>
							<SelectedTabStyle CssClass="InfraTab_Selected2"></SelectedTabStyle>
							<DisabledTabStyle CssClass="InfraTab_Disabled2"></DisabledTabStyle>
							<HoverTabStyle CssClass="InfraTab_Hover2"></HoverTabStyle>
							<Tabs>
								<igtab:Tab Text="Ricerca di Base">
									<ContentTemplate>
										<table style="table-layout: fixed" height="100%" cellspacing="10" cellpadding="0" width="100%" border="0">
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
                        <ondp:wzMsDataGrid id="WzDgrCicli" runat="server" Width="100%" disableActiveRowChange="False" EditMode="None" OnitStyle="False"
	                        PagerVoicesBefore="-1" PagerVoicesAfter="-1" AutoGenerateColumns="False" SelectionOption="rowClick">
	                        <HeaderStyle CssClass="header"/>
	                        <ItemStyle CssClass="item"/>
	                        <AlternatingItemStyle CssClass="alternating"/>
	                        <EditItemStyle CssClass="edit"/>
	                        <SelectedItemStyle CssClass="selected"/>
	                        <PagerStyle CssClass="pager"/>
	                        <FooterStyle CssClass="footer"/>
	                        <Columns>
		                        <ondp:wzMultiSelColumn ></ondp:wzMultiSelColumn>
		                        <ondp:wzBoundColumn HeaderText="Codice" Key="CIC_CODICE" SourceField="CIC_CODICE" SourceTable="T_ANA_CICLI" SourceConn="cicliConn">
			                        <HeaderStyle width="15%" />
		                        </ondp:wzBoundColumn>
		                        <ondp:wzBoundColumn HeaderText="Descrizione" Key="CIC_DESCRIZIONE" SourceField="CIC_DESCRIZIONE" SourceTable="T_ANA_CICLI" SourceConn="cicliConn">
			                        <HeaderStyle width="70%" />
		                        </ondp:wzBoundColumn>
		                        <ondp:wzBoundColumn HeaderText="Introduzione" Key="CIC_DATA_INTRODUZIONE" DataFormatString="{0:dd/MM/yyyy}" SourceField="CIC_DATA_INTRODUZIONE" SourceTable="T_ANA_CICLI" SourceConn="cicliConn">
			                        <HeaderStyle width="120px" cssclass="column_align_center" />
                                    <ItemStyle cssclass="column_align_center" />
		                        </ondp:wzBoundColumn>
		                        <ondp:wzBoundColumn HeaderText="Fine" Key="CIC_DATA_FINE" DataFormatString="{0:dd/MM/yyyy}" SourceField="CIC_DATA_FINE" SourceTable="T_ANA_CICLI" SourceConn="cicliConn">
			                        <HeaderStyle width="120px" cssclass="column_align_center" />
                                    <ItemStyle cssclass="column_align_center" />
		                        </ondp:wzBoundColumn>
		                        <ondp:wzBoundColumn HeaderText="Standard" Key="COD_DESCRIZIONE" SourceField="COD_DESCRIZIONE" SourceTable="T_ANA_CODIFICHE" SourceConn="cicliConn">
			                        <HeaderStyle width="5%" cssclass="column_align_center" />
                                    <ItemStyle cssclass="column_align_center" />
		                        </ondp:wzBoundColumn>
		                        <ondp:wzBoundColumn HeaderText="Sesso" Key="CIC_SESSO" SourceField="CIC_SESSO" SourceTable="T_ANA_CICLI" SourceConn="cicliConn">
			                        <HeaderStyle width="100px" cssclass="column_align_center" />
                                    <ItemStyle cssclass="column_align_center" />
		                        </ondp:wzBoundColumn>
	                        </Columns>
	                        <BindingColumns>
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="Codice" Connection="cicliConn" SourceTable="T_ANA_CICLI"
			                        Hidden="False" SourceField="CIC_CODICE"></ondp:BindingFieldValue>
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="Descrizione" Connection="cicliConn" SourceTable="T_ANA_CICLI"
			                        Hidden="False" SourceField="CIC_DESCRIZIONE"></ondp:BindingFieldValue>
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="Introduzione" Connection="cicliConn" SourceTable="T_ANA_CICLI"
			                        Hidden="False" SourceField="CIC_DATA_INTRODUZIONE"></ondp:BindingFieldValue>
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="Fine" Connection="cicliConn" SourceTable="T_ANA_CICLI"
			                        Hidden="False" SourceField="CIC_DATA_FINE"></ondp:BindingFieldValue>
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="Standard" Connection="cicliConn" SourceTable="T_ANA_CODIFICHE"
			                        Hidden="False" SourceField="COD_DESCRIZIONE"></ondp:BindingFieldValue>
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="Sesso" Connection="cicliConn" SourceTable="T_ANA_CICLI"
			                        Hidden="False" SourceField="CIC_SESSO"></ondp:BindingFieldValue>
	                        </BindingColumns>
                        </ondp:wzMsDataGrid>
                    </dyp:DynamicPanel>
                    
                    <div class="Sezione">Dettaglio</div>

                    <div>
					    <ondp:OnitDataPanel id="OdpCicliDetail" runat="server" useToolbar="False" ConfigFile="Cicli.OdpCicliDetail.xml"
						    renderOnlyChildren="True" Width="100%" dontLoadDataFirstTime="True" externalToolBar-Length="0" BindingFields="(Insieme)">
						    <table style="table-layout: fixed" cellspacing="3" cellpadding="0" width="100%" border="0">
							    <colgroup>
								    <col width="9%" />
								    <col width="18%" />
								    <col width="10%" />
								    <col width="15%" />
								    <col width="8%" />
								    <col width="7%" />
								    <col width="6%" />
								    <col width="13%" />
                                    <col width="12%" />
                                    <col width="2%" />
							    </colgroup>
							    <tr>
								    <td align="right" >
									    <asp:Label id="Label2" runat="server" CssClass="label">Codice</asp:Label></td>
								    <td>
									    <ondp:wzTextBox id="WzTbCodCiclo" runat="server" onblur="toUpper(this);controlloCampoCodice(this);"
										    CssStyles-CssDisabled="textbox_stringa_disabilitato w100p" CssStyles-CssEnabled="textbox_stringa w100p"
										    CssStyles-CssRequired="textbox_stringa_obbligatorio w100p" BindingField-Editable="onNew" BindingField-Connection="cicliDettConn"
										    BindingField-SourceTable="T_ANA_CICLI" BindingField-Hidden="False" BindingField-SourceField="CIC_CODICE"
										    MaxLength="8"></ondp:wzTextBox></td>
								    <td align="right">
									    <asp:Label id="Label3" runat="server" CssClass="label">Descrizione</asp:Label></td>
								    <td colspan="5">
									    <ondp:wzTextBox id="WzTbDescCiclo" runat="server" onblur="toUpper(this);"
										    CssStyles-CssDisabled="textbox_stringa_disabilitato w100p" CssStyles-CssEnabled="textbox_stringa w100p"
										    CssStyles-CssRequired="textbox_stringa_obbligatorio w100p" BindingField-Editable="always" BindingField-Connection="cicliDettConn"
										    BindingField-SourceTable="T_ANA_CICLI" BindingField-Hidden="False" BindingField-SourceField="CIC_DESCRIZIONE"
										    MaxLength="40"></ondp:wzTextBox></td>
                                    <td class="label">Obsoleto</td>
                                    <td>
                                        <ondp:wzCheckBox id="chkObsoleto" runat="server" Height="12px" BindingField-SourceField="CIC_OBSOLETO"
										    BindingField-Hidden="False" BindingField-SourceTable="T_ANA_CICLI" BindingField-Connection="cicliDettConn"
										    BindingField-Editable="always" CssStyles-CssEnabled="Label" CssStyles-CssDisabled="Label_Disabilitato"
										    BindingField-Value="N"></ondp:wzCheckBox></td>
							    </tr>
							    <tr>
								    <td align="right">
									    <asp:Label id="Label4" runat="server" CssClass="label">Introduzione</asp:Label></td>
								    <td>
									    <ondp:wzOnitDatePick id="WzIntroduzione" runat="server" Width="120px"
										    CssStyles-CssDisabled="textbox_data_disabilitato" CssStyles-CssEnabled="textbox_data" CssStyles-CssRequired="textbox_data_obbligatorio"
										    BindingField-Editable="always" BindingField-Connection="cicliDettConn" BindingField-SourceTable="T_ANA_CICLI"
										    BindingField-Hidden="False" BindingField-SourceField="CIC_DATA_INTRODUZIONE"></ondp:wzOnitDatePick></td>
								    <td align="right">
									    <asp:Label id="Label5" runat="server" CssClass="label">Fine</asp:Label></TD>
								    <td align="left">
									    <ondp:wzOnitDatePick id="WzFine" runat="server" Width="120px"
										    CssStyles-CssDisabled="textbox_data_disabilitato" CssStyles-CssEnabled="textbox_data" CssStyles-CssRequired="textbox_data_obbligatorio"
										    BindingField-Editable="always" BindingField-Connection="cicliDettConn" BindingField-SourceTable="T_ANA_CICLI"
										    BindingField-Hidden="False" BindingField-SourceField="CIC_DATA_FINE"></ondp:wzOnitDatePick>
								    <td align="right">
									    <asp:Label id="Label8" runat="server" CssClass="label">Standard</asp:Label></td>
								    <td align="left" >
									    <ondp:wzDropDownList id="WzStandard" runat="server" CssStyles-CssDisabled="textbox_stringa_disabilitato w50PX"
										    CssStyles-CssEnabled="textbox_stringa w50PX" CssStyles-CssRequired="textbox_stringa_obbligatorio w50PX"
										    BindingField-Editable="always" BindingField-Connection="cicliDettConn" BindingField-SourceTable="T_ANA_CICLI"
										    BindingField-Hidden="False" BindingField-SourceField="CIC_STANDARD" OtherListFields="cod_campo" DataFilter="cod_campo='CIC_STANDARD'"
										    TextFieldName="COD_DESCRIZIONE" KeyFieldName="COD_CODICE" SourceTable="T_ANA_CODIFICHE"></ondp:wzDropDownList></td>
								    <td align="right" >
									    <asp:Label id="Label6" runat="server" CssClass="label">Sesso</asp:Label></td>
								    <td>
									    <ondp:wzDropDownList id="WzSesso" runat="server" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p"
										    CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p"
										    BindingField-Editable="always" BindingField-Connection="cicliDettConn" BindingField-SourceTable="T_ANA_CICLI"
										    BindingField-Hidden="False" BindingField-SourceField="CIC_SESSO" SourceConnection="cicliDettConn">
										    <asp:ListItem Value="E" Selected="True">ENTRAMBI</asp:ListItem>
										    <asp:ListItem Value="M">MASCHI</asp:ListItem>
										    <asp:ListItem Value="F">FEMMINE</asp:ListItem>
									    </ondp:wzDropDownList></td>
                                    <td class="label">Mostra in APP</td>
                                    <td>
                                        <ondp:wzCheckBox id="chkShowInApp" runat="server" Height="12px" CssStyles-CssDisabled="Label_Disabilitato"
										    CssStyles-CssEnabled="Label" BindingField-Editable="always" BindingField-Connection="vacDatiConn"
										    BindingField-SourceTable="T_ANA_CICLI" BindingField-Hidden="False" BindingField-SourceField="CIC_SHOW_IN_APP"
										    BindingField-Value="N"></ondp:wzCheckBox></td>
							    </tr>
						    </table>
					    </ondp:OnitDataPanel>
                    </div>

				</ondp:OnitDataPanel>
			</on_lay3:onitlayout3>
		</form>
	</body>
</html>
