<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Operatori.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_Operatori" %>

<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="ONDP" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
		<title>Operatori</title>

        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/archivi.css") %>' type="text/css" rel="stylesheet" />
        
        <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("onvac.common.js")%>'></script>
        
        <!-- patch per il click sul checkbox dell'header per la selezione di tutti i checkbox -->
        <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("patch_selectAll.js")%>'></script>

		<script type="text/javascript" language="javascript">
		    function InizializzaToolBar(t) {
		        t.PostBackButton = false;
		    }
		
		    function ToolBarClick(ToolBar,button,evnt) {
			    evnt.needPostBack = true;
		
			    switch (button.Key) {
				    case 'btnStampa':
					    // se non ci sono righe, non deve stampare il report (modifica 27/08/2004)
					    if (<% = odpOperatori.getCurrentDataTable.Rows.Count %> == 0) {
						    alert("Attenzione: l'elenco non contiene alcun operatore. Impossibile eseguire la stampa.");
						    evnt.needPostBack = false;
					    }
					    break;
			    }
		    }
			function AssociaCentriToolBarClick(ToolBar,button,evnt) {
		        
		        evnt.needPostBack = true;

		        switch (button.Key) {

		            case 'btnSave':
		                var counter = countElementiSelezionati('<%= dgrConsultori.ClientID %>');
                        
		                if (counter == -1) {
		                    evnt.needPostBack = false;
		                    alert("Non ci sono dati da salvare.");
		                }
		                else {
		                    if (counter == 0) {
		                        if (!confirm("Tutti i centri associati verranno eliminati. Continuare?")) {
		                            evnt.needPostBack = false;
		                        }
		                    }
		                    else {
		                        if (!confirm("Salvare le modifiche effettuate?")) {
		                            evnt.needPostBack = false;
		                        }
		                    }	                    
		                }
		                break;
			        
		            case 'btnAnnulla':
			            closeFm('modAssociaCentri');
			            break;
			    }
		    }
		</script>
	</head>
	<body onload="registerCheckClick('dgrOperatoriMaster__ctl1_ChkMultiSel')">
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" width="100%" height="100%" Titolo="Operatori" TitleCssClass="Title3">
				<ondp:OnitDataPanel id="odpOperatori" runat="server" ConfigFile="Operatori.odpOperatori.xml"
					Width="100%" useToolbar="False" renderOnlyChildren="True" maxRecord="200">
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
                                <igtbar:TBarButton Key="btnAssociaCentri" Text="Associa centri" Image="../../../images/consultorio.gif">
                                    <DefaultStyle CssClass="infratoolbar_button_default" Width="120px"></DefaultStyle>
                                </igtbar:TBarButton>
								<igtbar:TBSeparator></igtbar:TBSeparator>
								<igtbar:TBarButton Key="btnStampa" Text="Stampa" Image="~/Images/stampa.gif"></igtbar:TBarButton>
							</Items>
						</igtbar:UltraWebToolbar>
                    </div>
                    <div class="Sezione">Modulo ricerca</div>
                    <div>
						<ondp:wzFilter id="WzFilter1" runat="server" Width="100%" Height="70px" CssClass="InfraUltraWebTab2">
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
					    <ondp:wzMsDataGrid id="dgrOperatoriMaster" runat="server" Width="100%" EditMode="None" OnitStyle="False"
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
				                <ondp:wzBoundColumn HeaderText="Codice" Key="OPE_CODICE" SourceField="OPE_CODICE" SourceTable="T_ANA_OPERATORI" SourceConn="OperatoriMaster"/>
				                <ondp:wzBoundColumn HeaderText="Nome" Key="OPE_NOME" SourceField="OPE_NOME" SourceTable="T_ANA_OPERATORI" SourceConn="OperatoriMaster"/>
				                <ondp:wzBoundColumn HeaderText="Qualifica" Key="COD_DESCRIZIONE" SourceField="COD_DESCRIZIONE" SourceTable="T_ANA_CODIFICHE" SourceConn="OperatoriMaster"/>
				                <ondp:wzBoundColumn HeaderText="Codice esterno" Key="OPE_CODICE_ESTERNO" SourceField="OPE_CODICE_ESTERNO" SourceTable="T_ANA_OPERATORI" SourceConn="OperatoriMaster"/>
				                <ondp:wzBoundColumn HeaderText="Obsoleto" Key="OPE_OBSOLETO" SourceField="OPE_OBSOLETO" SourceTable="T_ANA_OPERATORI" SourceConn="OperatoriMaster"/>
				                <ondp:wzBoundColumn HeaderText="Tel Studio" Key="OPE_TEL_STUDIO" SourceField="OPE_TEL_STUDIO" SourceTable="T_ANA_OPERATORI" SourceConn="OperatoriMaster"/>
				                <ondp:wzBoundColumn HeaderText="Tel Casa" Key="OPE_TEL_CASA" SourceField="OPE_TEL_CASA" SourceTable="T_ANA_OPERATORI" SourceConn="OperatoriMaster"/>
				                <ondp:wzBoundColumn HeaderText="Comune" Key="COM_DESCRIZIONE" SourceField="COM_DESCRIZIONE" SourceTable="T_ANA_COMUNI" SourceConn="OperatoriMaster"/>
				                <ondp:wzBoundColumn HeaderText="CAP" Key="OPE_CAP" SourceField="OPE_CAP" SourceTable="T_ANA_OPERATORI" SourceConn="OperatoriMaster"/>
				                <ondp:wzBoundColumn HeaderText="Indirizzo" Key="OPE_INDIRIZZO" SourceField="OPE_INDIRIZZO" SourceTable="T_ANA_OPERATORI" SourceConn="OperatoriMaster"/>
				                <ondp:wzBoundColumn HeaderText="Codice fiscale" Key="OPE_CODICE_FISCALE" SourceField="OPE_CODICE_FISCALE" SourceTable="T_ANA_OPERATORI" SourceConn="OperatoriMaster"/>
				                <ondp:wzBoundColumn HeaderText="E-mail" Key="OPE_EMAIL" SourceField="OPE_EMAIL" SourceTable="T_ANA_OPERATORI" SourceConn="OperatoriMaster"/>
			                </Columns>
		                    <BindingColumns>
		                        <ondp:BindingFieldValue Value="" Editable="onNew" Description="Codice" Connection="OperatoriMaster" 
                                    SourceTable="T_ANA_OPERATORI" SourceField="OPE_CODICE" Hidden="False" />
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="Nome" Connection="OperatoriMaster" 
                                    SourceTable="T_ANA_OPERATORI" SourceField="OPE_NOME" Hidden="False" />
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="Qualifica" Connection="OperatoriMaster"
                                    SourceTable="T_ANA_CODIFICHE" SourceField="COD_DESCRIZIONE" Hidden="False" />
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="Codice esterno" Connection="OperatoriMaster"
                                    SourceTable="T_ANA_OPERATORI" SourceField="OPE_CODICE_ESTERNO" Hidden="False" />
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="Obsoleto" Connection="OperatoriMaster" 
                                    SourceTable="T_ANA_OPERATORI" SourceField="OPE_OBSOLETO" Hidden="False" />
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="Tel Studio" Connection="OperatoriMaster"
                                    SourceTable="T_ANA_OPERATORI" SourceField="OPE_TEL_STUDIO" Hidden="False" />
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="Tel Casa" Connection="OperatoriMaster" 
                                    SourceTable="T_ANA_OPERATORI" SourceField="OPE_TEL_CASA" Hidden="False" />
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="Comune" Connection="OperatoriMaster" 
                                    SourceTable="T_ANA_COMUNI" SourceField="COM_DESCRIZIONE" Hidden="False" />
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="CAP" Connection="OperatoriMaster" 
                                    SourceTable="T_ANA_OPERATORI" SourceField="OPE_CAP" Hidden="False" />
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="Indirizzo" Connection="OperatoriMaster"
                                    SourceTable="T_ANA_OPERATORI" SourceField="OPE_INDIRIZZO" Hidden="False" />
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="Codice fiscale" Connection="OperatoriMaster"
                                    SourceTable="T_ANA_OPERATORI" SourceField="OPE_CODICE_FISCALE" Hidden="False" />
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="E-mail" Connection="OperatoriMaster" 
                                    SourceTable="T_ANA_OPERATORI" SourceField="OPE_EMAIL" Hidden="False" />
	                        </BindingColumns>
                        </ondp:wzMsDataGrid>
                    </dyp:DynamicPanel>

                    <div class="Sezione">Dettaglio</div>
                    <div>
						<table style="table-layout: fixed" cellspacing="3" width="100%" border="0">
							<tr>
								<td class="label" width="110">Codice</td>
								<td>
									<ondp:wzTextBox id="txtCodice" onblur="toUpper(this);controlloCampoCodice(this);" runat="server" MaxLength="16"
										BindingField-SourceField="OPE_CODICE" BindingField-Hidden="False" BindingField-SourceTable="T_ANA_OPERATORI"
										BindingField-Connection="OperatoriMaster" BindingField-Editable="onNew" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
										CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"></ondp:wzTextBox></td>
								<td class="label" width="110">Nome</td>
								<td>
									<ondp:wzTextBox id="txtNome" onblur="toUpper(this);" runat="server" BindingField-SourceField="OPE_NOME"
										BindingField-Hidden="False" BindingField-SourceTable="T_ANA_OPERATORI" BindingField-Connection="OperatoriMaster"
										BindingField-Editable="always" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p" CssStyles-CssEnabled="TextBox_Stringa w100p"
										CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p" MaxLength="35"></ondp:wzTextBox></td>
							</tr>
							<tr>
								<td class="label" width="110">Qualifica</td>
								<td>
									<ondp:wzDropDownList id="WzDropDownList1" runat="server" BindingField-SourceField="OPE_QUALIFICA"
										BindingField-Hidden="False" BindingField-SourceTable="T_ANA_OPERATORI" BindingField-Connection="OperatoriMaster"
										BindingField-Editable="always" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p" CssStyles-CssEnabled="TextBox_Stringa w100p"
										CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p" SourceTable="T_ANA_CODIFICHE"
										KeyFieldName="COD_CODICE" TextFieldName="COD_DESCRIZIONE" OtherListFields="cod_campo" DataFilter="cod_campo='OPE_QUALIFICA'"></ondp:wzDropDownList></td>
								<td class="label" width="110">Codice esterno</td>
								<td>
									<ondp:wzTextBox id="txtCodiceEsterno" runat="server" MaxLength="30"
										BindingField-SourceField="OPE_CODICE_ESTERNO" BindingField-Hidden="False" BindingField-SourceTable="T_ANA_OPERATORI"
										BindingField-Connection="OperatoriMaster" BindingField-Editable="always" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
										CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"></ondp:wzTextBox></td>
							</tr>
							<tr>
								<td class="label" width="110">Telefono studio</td>
								<td>
									<ondp:wzTextBox id="txtTelStudio" onblur="toUpper(this);" runat="server" MaxLength="18"
										BindingField-SourceField="OPE_TEL_STUDIO" BindingField-Hidden="False" BindingField-SourceTable="T_ANA_OPERATORI"
										BindingField-Connection="OperatoriMaster" BindingField-Editable="always" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
										CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"></ondp:wzTextBox></td>
								<td class="label" width="110">Telefono casa</td>
								<td>
									<ondp:wzTextBox id="txtTelCasa" onblur="toUpper(this);" runat="server" MaxLength="18"
										BindingField-SourceField="OPE_TEL_CASA" BindingField-Hidden="False" BindingField-SourceTable="T_ANA_OPERATORI"
										BindingField-Connection="OperatoriMaster" BindingField-Editable="always" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
										CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"></ondp:wzTextBox></td>
							</tr>
							<tr>
								<td class="label" width="110">Comune residenza</td>
								<td>
									<ondp:wzFinestraModale id="txtComuneResidenza" runat="server" Width="70%" CodiceWidth="30%" LabelWidth="-1px"
										PosizionamentoFacile="False" BindingCode-Editable="always" BindingCode-Connection="OperatoriMaster"
										BindingCode-SourceTable="T_ANA_OPERATORI" BindingCode-Hidden="False" BindingCode-SourceField="OPE_COM_RESIDENZA"
										UseCode="True" RaiseChangeEvent="False" SetUpperCase="True" Obbligatorio="False" BindingDescription-Editable="always"
										BindingDescription-Hidden="False" BindingDescription-Connection="OperatoriMaster" BindingDescription-SourceTable="T_ANA_COMUNI"
										BindingDescription-SourceField="COM_DESCRIZIONE" CampoCodice="COM_CODICE Codice" CampoDescrizione="COM_DESCRIZIONE Descrizione"
										Tabella="T_ANA_COMUNI"></ondp:wzFinestraModale></td>
								<td class="label" width="110">Cap</td>
								<td>
									<ondp:wzTextBox id="txtCAP" onblur="toUpper(this);" runat="server" BindingField-SourceField="OPE_CAP"
										BindingField-Hidden="False" BindingField-SourceTable="T_ANA_OPERATORI" BindingField-Connection="OperatoriMaster"
										BindingField-Editable="always" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p" CssStyles-CssEnabled="TextBox_Stringa w100p"
										CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p" MaxLength="5"></ondp:wzTextBox></td>
							</tr>
							<tr>
								<td class="label" width="110">Indirizzo</td>
								<td colspan="3">
									<ondp:wzTextBox id="txtIndirizzo" onblur="toUpper(this);" runat="server" MaxLength="30"
										BindingField-SourceField="OPE_INDIRIZZO" BindingField-Hidden="False" BindingField-SourceTable="T_ANA_OPERATORI"
										BindingField-Connection="OperatoriMaster" BindingField-Editable="always" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
										CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"></ondp:wzTextBox></td>
							</tr>
							<tr>
								<td class="label" width="110">Codice fiscale</td>
								<td>
									<ondp:wzTextBox id="txtCodiceFiscale" onblur="toUpper(this);" runat="server" MaxLength="16"
										BindingField-SourceField="OPE_CODICE_FISCALE" BindingField-Hidden="False" BindingField-SourceTable="T_ANA_OPERATORI"
										BindingField-Connection="OperatoriMaster" BindingField-Editable="always" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
										CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"></ondp:wzTextBox></td>
								<td class="label" width="110">Mail</td>
								<td>
									<ondp:wzTextBox id="txtMail" onblur="toUpper(this);" runat="server" BindingField-SourceField="OPE_EMAIL"
										BindingField-Hidden="False" BindingField-SourceTable="T_ANA_OPERATORI" BindingField-Connection="OperatoriMaster"
										BindingField-Editable="always" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p" CssStyles-CssEnabled="TextBox_Stringa w100p"
										CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p" MaxLength="35"></ondp:wzTextBox></td>
							</tr>
							<tr>
								<td class="label" width="110">Obsoleto</td>
								<td>
									<ondp:wzDropDownList id="Wzdropdownlist2" runat="server" width="70px" BindingField-SourceField="OPE_OBSOLETO"
										BindingField-Hidden="False" BindingField-SourceTable="T_ANA_OPERATORI" BindingField-Connection="OperatoriMaster"
										BindingField-Editable="always" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p" CssStyles-CssEnabled="TextBox_Stringa w100p"
										CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p" SourceTable="T_ANA_CODIFICHE" IncludeNull="True"
										KeyFieldName="COD_CODICE" TextFieldName="COD_DESCRIZIONE" OtherListFields="cod_campo" DataFilter="cod_campo='OPE_OBSOLETO'"></ondp:wzDropDownList></td>
							</tr>
						</table>
                    </div>
				</ondp:OnitDataPanel>
			</on_lay3:onitlayout3>
			 <onitcontrols:OnitFinestraModale ID="modAssociaCentri" Title="Associa centri all'operatore corrente" runat="server"
                Width="800px" Height="500px" BackColor="LightGray" ZIndexPosition="1" IsAnagrafe="False">
                <div>
                    <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolBarAssocia" runat="server" ItemWidthDefault="100px" CssClass="infratoolbar">
                        <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                        <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                        <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                        <ClientSideEvents InitializeToolbar="AssociaCentriInizializzaToolBar" Click="AssociaCentriToolBarClick"></ClientSideEvents>
                        <Items>
                            <igtbar:TBarButton Key="btnFind" DisabledImage="~/Images/Cerca_dis.gif" Text="Cerca"
                                Image="~/Images/Cerca.gif">
                            </igtbar:TBarButton>
                            <igtbar:TBSeparator></igtbar:TBSeparator>
                            <igtbar:TBarButton Key="btnSave" DisabledImage="~/Images/salva_dis.gif" Text="Salva"
                                Image="~/Images/salva.gif">
                            </igtbar:TBarButton>
                            <igtbar:TBarButton Key="btnAnnulla" DisabledImage="~/Images/annulla_dis.gif" Text="Annulla"
                                Image="~/Images/annulla.gif">
                            </igtbar:TBarButton>
                        </Items>
                    </igtbar:UltraWebToolbar>
                </div>
                <div class="filters">
                    <table border="0" cellpadding="0" cellspacing="2" style="width:100%">
                        <colgroup>
                            <col style="width: 10%" />
                            <col style="width: 40%" />
                            <col style="width: 10%" />
                            <col style="width: 40%" />
                        </colgroup>
                        <tr>
                            <td style="text-align: right">
                                    <asp:Label runat="server" ID="lblUls" CssClass="Label" Text="Ulss"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlUls" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlUls_SelectedIndexChanged" Width="100%"></asp:DropDownList>
                                </td>
                            <td style="text-align: right;">
                                <asp:Label runat="server" ID="lbDistretto" Text="Distretto" CssClass="Label"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlDistretto" runat="server" Width="100%" AutoPostBack="true" OnSelectedIndexChanged="ddlDistretto_SelectedIndexChanged"></asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                </div>
                <dyp:DynamicPanel ID="dypScrollRight" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                    <asp:HiddenField ID="hdIndexFlagDefault" runat="server" />
                    <asp:DataGrid ID="dgrConsultori" runat="server" Width="100%" AutoGenerateColumns="False">
                        <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
                        <ItemStyle CssClass="item"></ItemStyle>
                        <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
                        <HeaderStyle CssClass="header"></HeaderStyle>
                        <PagerStyle CssClass="pager" Position="TopAndBottom" Mode="NumericPages" />
                        <Columns>
                            <asp:TemplateColumn>
                                <HeaderStyle Width="2%" HorizontalAlign="Center" />
                                <HeaderTemplate>
                                    <input type="checkbox" id="chkSelDeselAll" onclick="selezionaTutti(this, 'dgrConsultori', 0)" />
                                </HeaderTemplate>
                                <ItemStyle Width="2%" HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkConsultorio" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:BoundColumn Visible="True" DataField="DesAmbitoDistretto" HeaderText="Distretto" HeaderStyle-Width="45%"></asp:BoundColumn>
                            <asp:BoundColumn Visible="True" DataField="DescrCentro" HeaderText="Centro" HeaderStyle-Width="33%"></asp:BoundColumn>
                            <asp:BoundColumn Visible="False" DataField="ConsultorioDefault"></asp:BoundColumn>
                            <asp:BoundColumn Visible="False" DataField="CodiceConsultorio"></asp:BoundColumn>
                        </Columns>
                    </asp:DataGrid>
                </dyp:DynamicPanel>
            </onitcontrols:OnitFinestraModale>
		</form>
	</body>
</html>
