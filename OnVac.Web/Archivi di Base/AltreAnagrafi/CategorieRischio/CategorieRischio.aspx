<%@ Page Language="vb" AutoEventWireup="false" Codebehind="CategorieRischio.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_CategorieRischio" %>

<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
	<title>Categorie Rischio</title>
		
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/archivi.css") %>' type="text/css" rel="stylesheet" />
		
        <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("onvac.common.js")%>'></script>
        <!-- patch per il click sul checkbox dell'header per la selezione di tutti i checkbox -->
        <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("patch_selectAll.js")%>'></script>

        <script type="text/javascript">
		    function ToolBarClick(ToolBar, button, evnt) {
			    evnt.needPostBack=true;
			    switch(button.Key) {
				    case 'btnSalva':
					    cod = document.getElementById('txtCodiceEsterno').value;
					    if (isNaN(cod)) {
						    alert("Inserire un numero nel campo 'Codice Esterno' !");
						    evnt.needPostBack=false;
					    }
					    break;
			    }
            }

            function ClickTlb(t, btn, evnt) {
                evnt.needPostBack = true;
        		
                switch (btn.Key) {
                
                    case 'btnSalvaVaccinazioni':
                        evnt.needPostBack = confirm('ATTENZIONE: salvare le modifiche effettuate?');
                        break;
            
                    case 'btnAnnullaVaccinazioni':
                        evnt.needPostBack = confirm('ATTENZIONE: le modifiche effettuate verranno perse. Continuare?');
                        break;
                }
            }

            function SelezionaTutti(chkValue) {
                __doPostBack('selectAll', chkValue);
            }
		</script>
	</head>
	<body onload="registerCheckClick('dgrCatRischio__ctl1_ChkMultiSel');">
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" Titolo="Categorie Rischio" TitleCssClass="Title3" width="100%" height="100%">
				<ondp:OnitDataPanel id="odpRischioMaster" runat="server" ConfigFile="CategorieRischio.odpRischioMaster.xml"
					renderOnlyChildren="True" useToolbar="False">
                    <div>
                        <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                            <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                            <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
							<ClientSideEvents Click="ToolBarClick"></ClientSideEvents>
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
								<igtbar:TBarButton Key="btnCicli" Text="Cicli" Image="../../../images/aggiorna_elenco.gif"></igtbar:TBarButton>
                                <igtbar:TBarButton Key="btnVaccinazioni" Text="Vaccinazioni" Image="../../../images/vaccinazione.gif">
                                    <DefaultStyle CssClass="infratoolbar_button_default" Width="100px"></DefaultStyle>
                                </igtbar:TBarButton>
							</Items>
						</igtbar:UltraWebToolbar>
                    </div>
					<div class="Sezione">Modulo ricerca</div>
                    <div>
						<ondp:wzFilter id="WzFilter1" runat="server" Width="100%" Height="70px" CssClass="InfraUltraWebTab2">
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
					
                    <div class="Sezione">Elenco</div>

                    <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                        <ondp:wzMsDataGrid id="dgrCatRischio" runat="server" Width="100%" OnitStyle="False" EditMode="None" 
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
                                <ondp:wzBoundColumn HeaderText="Codice" Key="RSC_CODICE" SourceField="RSC_CODICE" SourceTable="T_ANA_RISCHIO" SourceConn="connessioneMaster">
                                    <HeaderStyle width="10%" />
                                </ondp:wzBoundColumn>
                                <ondp:wzBoundColumn HeaderText="Descrizione" Key="RSC_DESCRIZIONE" SourceField="RSC_DESCRIZIONE" SourceTable="T_ANA_RISCHIO" SourceConn="connessioneMaster">
                                    <HeaderStyle width="80%" />
								</ondp:wzBoundColumn>
								<ondp:wzBoundColumn HeaderText="Obsoleta" Key="RSC_OBSOLETO" SourceField="RSC_OBSOLETO" SourceTable="T_ANA_RISCHIO" SourceConn="connessioneMaster">
									<HeaderStyle width="10%" HorizontalAlign="Center"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
								</ondp:wzBoundColumn>
                            </Columns>
							<BindingColumns>
								<ondp:BindingFieldValue Value="" Editable="onNew" Description="Codice" Connection="connessioneMaster" SourceTable="T_ANA_RISCHIO"
									Hidden="False" SourceField="RSC_CODICE"></ondp:BindingFieldValue>
								<ondp:BindingFieldValue Value="" Editable="always" Description="Descrizione" Connection="connessioneMaster" SourceTable="T_ANA_RISCHIO" 
                                    Hidden="False" SourceField="RSC_DESCRIZIONE"></ondp:BindingFieldValue>
								<ondp:BindingFieldValue Value="" Editable="always" Description="Obsoleto" Connection="connessioneMaster" SourceTable="T_ANA_RISCHIO" 
                                    Hidden="False" SourceField="RSC_OBSOLETO"></ondp:BindingFieldValue>
							</BindingColumns>
						</ondp:wzMsDataGrid>
					</dyp:DynamicPanel>

                    <div class="Sezione">Dettaglio</div>
                    <div>
					    <ondp:OnitDataPanel id="odpRischioDetail" runat="server" ConfigFile="CategorieRischio.odpRischioDetail.xml"
							renderOnlyChildren="True" useToolbar="False" dontLoadDataFirstTime="True" BindingFields="(Insieme)"
							Width="100%" externalToolBar-Length="0">
							<table style="table-layout: fixed" border="0" cellspacing="3" cellpadding="0" width="100%">
                                <colgroup>
                                    <col width="10%" />
                                    <col width="20%" />
                                    <col width="10%" />
                                    <col width="10%" />
									<col width="10%" />
									<col width="2%" />
									<col width="10%" />
									<col />
                                </colgroup>
								<tr>
									<td class="label">Codice</td>
									<td>
										<ondp:wzTextBox id="txtCodice" runat="server" BindingField-Editable="onNew"
                                            onblur="toUpper(this);controlloCampoCodice(this);"
											BindingField-Connection="connessioneSec" BindingField-SourceTable="T_ANA_RISCHIO" BindingField-Hidden="False"
											BindingField-SourceField="RSC_CODICE" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
											CssStyles-CssEnabled="TextBox_Stringa  w100p" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
											maxlength="8"></ondp:wzTextBox></td>

									<td class="label">Descrizione</td>
									<td colspan="5">
										<ondp:wzTextBox id="txtDescrizione" runat="server" BindingField-Editable="always" onblur="toUpper(this);"
											BindingField-Connection="connessioneSec" BindingField-SourceTable="T_ANA_RISCHIO" BindingField-Hidden="False"
											BindingField-SourceField="RSC_DESCRIZIONE" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
											CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
											maxlength="35"></ondp:wzTextBox></td>
								</tr>
								<tr>
									<td class="label">Codice Esterno</td>
									<td>
										<ondp:wzTextBox id="txtCodiceEsterno" runat="server" BindingField-Editable="always"
											BindingField-Connection="connessioneSec" BindingField-SourceTable="T_ANA_RISCHIO" BindingField-Hidden="False"
											BindingField-SourceField="RSC_CODICE_ESTERNO" CssStyles-CssDisabled="textbox_numerico_Disabilitato w100p"
											CssStyles-CssEnabled="textbox_numerico w100p"
											maxlength="4"></ondp:wzTextBox></td>

                                    <td class="label">Codice AVN</td>
                                    <td>
                                        <ondp:wzTextBox id="txtAvn" runat="server" Width="100%" MaxLength="2" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p"
										    CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p"
										    BindingField-Editable="always" BindingField-Connection="connessione" BindingField-SourceTable="T_ANA_RISCHIO"
										    BindingField-Hidden="False" BindingField-SourceField="RSC_CODICE_AVN"></ondp:wzTextBox></td>

									<td class="label">Obsoleta</td>
									<td>
										 <ondp:wzCheckBox id="chkObsoleto" runat="server" height="12" width="20%"
											CssStyles-CssEnabled="TextBox_Stringa" CssStyles-CssDisabled="TextBox_Stringa"
											BindingField-Editable="always" BindingField-Connection="connessione" BindingField-SourceTable="T_ANA_RISCHIO"
											BindingField-Hidden="False" BindingField-SourceField="RSC_OBSOLETO" BindingField-Value="N"></ondp:wzCheckBox></td>	
									
									<td class="label">Macro Categoria</td>
									<td>
										<ondp:wzDropDownList id="ddlMacrocategoria" runat="server" Width="100%"
											CssStyles-CssDisabled="TextBox_Data_Disabilitato w100" CssStyles-CssEnabled="TextBox_Data w100"
											CssStyles-CssRequired="TextBox_Data_Obbligatorio w100" BindingField-Hidden="False" BindingField-Editable="always"
											BindingField-Connection="connessioneSec" BindingField-SourceTable="T_ANA_RISCHIO" BindingField-SourceField="RSC_MCR_CODICE"
											TextFieldName="MCR_DESCRIZIONE" KeyFieldName="MCR_CODICE" SourceTable="T_ANA_RISCHIO_MACROCATEGORIE" IncludeNull="true"></ondp:wzDropDownList></td>   
                                </tr>
							</table>
						</ondp:OnitDataPanel>
					</div>
				</ondp:OnitDataPanel>
			</on_lay3:onitlayout3>

            <on_ofm:OnitFinestraModale ID="fmVaccinazioni" runat="server" BackColor="LightGray" NoRenderX="true" Height="100%" Width="100%">    
            <div>
                <table border="0" cellpadding="0" cellspacing="0" style="background-color:whitesmoke; width:100%">
                    <tr>
                        <td>
                            <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="tlbVaccinazioni" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                                <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                                <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
	                            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
	                            <ClientSideEvents InitializeToolbar="InitTlb" Click="ClickTlb"></ClientSideEvents>
	                            <Items>
		                            <igtbar:TBarButton Key="btnSalvaVaccinazioni" Text="Salva" DisabledImage="~/Images/salva_dis.gif" Image="~/Images/salva.gif"
                                        ToolTip="Salva le modifiche effettuate e chiude la pop-up delle vaccinazioni">
		                            </igtbar:TBarButton>
		                            <igtbar:TBarButton Key="btnAnnullaVaccinazioni" Text="Annulla" DisabledImage="~/Images/annulla_dis.gif" Image="~/Images/annulla.gif"
                                        ToolTip="Annulla le modifiche effettuate e chiude la pop-up delle vaccinazioni">
		                            </igtbar:TBarButton>
                                </Items>
                            </igtbar:UltraWebToolbar>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div class="Title" id="divTitolo" style="width: 100%; text-align:center">
					            <asp:Label id="lblTitolo" runat="server" ></asp:Label>
				            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align:left">
                            <asp:Panel id="pnlSezioneDettagli" runat="server" CssClass="vac-sezione">
						        <asp:Label id="lblSezioneDettagli" runat="server">VACCINAZIONI</asp:Label>
					        </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>

            <dyp:DynamicPanel ID="dypVaccinazioni" runat="server" DynamicHeight="100%" DynamicWidth="100%" CssClass="dypScroll" ScrollBars="auto">
		        <asp:DataGrid id="dgrVaccinazioni" runat="server" CssClass="datagrid" Width="100%" ShowFooter="false" 
			        AllowSorting="True" AutoGenerateColumns="False" CellPadding="1" GridLines="None" DataKeyField="CodiceVac" >
			        <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
			        <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
			        <ItemStyle CssClass="item"></ItemStyle>
			        <HeaderStyle Font-Bold="True" HorizontalAlign="Left" CssClass="header"></HeaderStyle>
			        <Columns>
                        <asp:TemplateColumn HeaderText="" HeaderStyle-Width="4%">
                             <HeaderTemplate>
                                <input type="checkbox" id="chkSelezioneHeader" onclick="SelezionaTutti(this.checked);" title="Seleziona tutti" runat="server" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSelezioneItem" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="CodiceVac" HeaderText="Codice" HeaderStyle-Width="46%">
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="DescrizioneVac" HeaderText="Descrizione" HeaderStyle-Width="50%">
                        </asp:BoundColumn>
                    </Columns>
                </asp:DataGrid>
            </dyp:DynamicPanel>
        </on_ofm:OnitFinestraModale>
		</form>
	</body>
</html>