<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Ambulatori.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_Ambulatori" %>

<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>Ambulatori</title>

        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/archivi.css") %>' type="text/css" rel="stylesheet" />

		<style type="text/css">
		    .w40PX { width: 40px }
		</style>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" width="100%" height="100%" Titolo="Ambulatori" WindowNoFrames="False" TitleCssClass="Title3">
				<ondp:OnitDataPanel id="odpAmbulatoriMaster" runat="server" defaultSort-Length="44" Width="100%" ConfigFile="Ambulatori.odpAmbulatoriMaster.xml"
					dontLoadDataFirstTime="true" Height="100%" externalToolBar-Length="0" renderOnlyChildren="True"
					useToolbar="False" defaultSort="AmbulatoriMaster.t_ana_Ambulatori.amb_codice">
                    <div>
                        <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar" >
                            <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                            <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
							<Items>
								<igtbar:TBarButton Key="btnCerca" Text="Cerca" Image="~/Images/cerca.gif" DisabledImage="~/Images/cerca_dis.gif"></igtbar:TBarButton>
								<igtbar:TBarButton Key="btnNew" Text="Nuovo" Image="~/Images/nuovo.gif" DisabledImage="~/Images/nuovo_dis.gif"></igtbar:TBarButton>
								<igtbar:TBarButton Key="btnEdit" Text="Modifica" Image="~/Images/modifica.gif" DisabledImage="~/Images/modifica_dis.gif"></igtbar:TBarButton>
								<igtbar:TBarButton Key="btnSalvaCheck" Text="Salva" Image="~/Images/salva.gif" DisabledImage="~/Images/salva_dis.gif"></igtbar:TBarButton>
								<igtbar:TBarButton Key="btnAnnulla" Text="Annulla" DisabledImage="~/Images/annulla_dis.gif" Image="~/Images/annulla.gif"></igtbar:TBarButton>
								<igtbar:TBarButton Key="btnLinkIndisponibilita" Text="Indisp." Image="../../../images/indisponibilita.gif"></igtbar:TBarButton>
								<igtbar:TBarButton Key="btnLinkOrari" Text="Orari" Image="../../../images/appuntamenti.gif"></igtbar:TBarButton>
							</Items>
						</igtbar:UltraWebToolbar>
                    </div>
                    
                    <div class="Sezione">Modulo ricerca</div>

                    <div>
						<ondp:wzFilter id="filFiltro" runat="server" Width="100%" Height="70px" CssClass="InfraUltraWebTab2">
							<SelectedTabStyle CssClass="InfraTab_Selected2"></SelectedTabStyle>
							<HoverTabStyle CssClass="InfraTab_Hover2"></HoverTabStyle>
							<DefaultTabStyle CssClass="InfraTab_Default2"></DefaultTabStyle>
							<DisabledTabStyle CssClass="InfraTab_Disabled2"></DisabledTabStyle>
							<Tabs>
								<igtab:Tab Text="Ricerca di Base">
									<ContentTemplate>
										<table style="table-layout: fixed" height="100%" cellspacing="10" cellpadding="0" width="100%"
											border="0">
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

                    <div class="Sezione">Elenco Ambulatori</div>

                    <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
						<ondp:wzMsDataGrid id="dgrAmbulatoriMaster" runat="server" Width="100%" OnitStyle="False" EditMode="None" 
                            PagerVoicesAfter="-1" PagerVoicesBefore="-1" AutoGenerateColumns="False" SelectionOption="rowClick">
			                <HeaderStyle CssClass="header"></HeaderStyle>
                            <ItemStyle CssClass="item"></ItemStyle>
                            <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
							<SelectedItemStyle CssClass="selected"></SelectedItemStyle>
							<EditItemStyle CssClass="edit"></EditItemStyle>
                            <FooterStyle CssClass="footer"></FooterStyle>
                            <PagerStyle CssClass="pager"></PagerStyle>
			                <Columns>
				                <ondp:wzBoundColumn HeaderText="Codice" Key="AMB_CODICE" SourceField="AMB_CODICE" SourceTable="T_ANA_AMBULATORI" SourceConn="AmbulatoriMaster">
                                    <HeaderStyle width="10%" />
				                </ondp:wzBoundColumn>
				                <ondp:wzBoundColumn HeaderText="Centro Vaccinale" Key="AMB_CNS_CODICE" SourceField="AMB_CNS_CODICE" SourceTable="T_ANA_AMBULATORI" SourceConn="AmbulatoriMaster">
                                    <HeaderStyle width="20%" />
                                </ondp:wzBoundColumn>
				                <ondp:wzBoundColumn HeaderText="Descrizione"  SourceField="AMB_DESCRIZIONE" SourceTable="T_ANA_AMBULATORI" SourceConn="AmbulatoriMaster">
                                    <HeaderStyle width="70%" />
				                </ondp:wzBoundColumn>
			                </Columns>
	                        <BindingColumns>
		                        <ondp:BindingFieldValue Value="" Editable="onNew" Description="Codice" Connection="AmbulatoriMaster"
                                    SourceTable="T_ANA_AMBULATORI" SourceField="AMB_CODICE" Hidden="False" />
		                        <ondp:BindingFieldValue Value="" Editable="onNew" Description="Centro Vaccinale" Connection="AmbulatoriMaster"
                                    SourceTable="T_ANA_AMBULATORI" SourceField="AMB_CNS_CODICE" Hidden="False" />
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="Descrizione" Connection="AmbulatoriMaster"
                                    SourceTable="T_ANA_AMBULATORI" SourceField="AMB_DESCRIZIONE" Hidden="False" />
	                        </BindingColumns>
	                    </ondp:wzMsDataGrid>
                    </dyp:DynamicPanel>
                    
                    <div class="Sezione">Dettaglio</div> 
                    
                    <div>
                        <!-- DETAIL DATAPANEL -->
						<ondp:OnitDataPanel id="odpAmbulatoriDetail" runat="server" Width="100%" ConfigFile="Ambulatori.odpAmbulatoriDetail.xml"
							dontLoadDataFirstTime="True" Height="100px" externalToolBar-Length="7" renderOnlyChildren="True"
							useToolbar="False" externalToolBar="ToolBar">
							<table style="table-layout: fixed" cellspacing="3" width="100%" border="0">
								<colgroup>
									<col width="12%" />
									<col width="6%" />
									<col width="8%" />
									<col width="12%" />
									<col width="12%" />
									<col width="12%" />
									<col width="38%" />
								</colgroup>
								<tr>
									<td class="label">Codice</td>
									<td>
										<ondp:wzTextBox id="WzTextBox1" onblur="toUpper(this);" style="POSITION: relative" runat="server"
											CssStyles-CssRequired="textbox_numerico_obbligatorio w100p" CssStyles-CssEnabled="textbox_numerico w100p"
											CssStyles-CssDisabled="textbox_numerico_disabilitato w100p" BindingField-SourceField="AMB_CODICE"
											BindingField-Hidden="False" BindingField-SourceTable="T_ANA_AMBULATORI" BindingField-Connection="AmbulatoriDettaglio"
											BindingField-Editable="never" MS_POSITIONING="GridLayout" MaxLength="8"></ondp:wzTextBox></td>
									<td class="label">Descrizione</td>
									<td colspan="2">
										<ondp:wzTextBox id="WzTextBox2" onblur="toUpper(this);" style="POSITION: relative" runat="server"
											CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p" CssStyles-CssEnabled="TextBox_Stringa w100p"
											CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p" BindingField-SourceField="AMB_DESCRIZIONE"
											BindingField-Hidden="False" BindingField-SourceTable="T_ANA_AMBULATORI" BindingField-Connection="AmbulatoriDettaglio"
											BindingField-Editable="always" ms_positioning="GridLayout" MaxLength="30"></ondp:wzTextBox></td>
									<td class="label">Centro Vaccinale</td>
									<td>
										<ondp:wzFinestraModale id="fmConsultorio" runat="server" Width="69%" PosizionamentoFacile="False" LabelWidth="-1px"
											CodiceWidth="30%" BindingCode-Editable="onNew" BindingCode-Connection="AmbulatoriDettaglio" BindingCode-SourceTable="T_ANA_AMBULATORI"
											BindingCode-Hidden="False" BindingCode-SourceField="AMB_CNS_CODICE" UseCode="True" RaiseChangeEvent="False"
											SetUpperCase="True" Obbligatorio="False" BindingDescription-Editable="onNew" BindingDescription-Hidden="False"
											BindingDescription-Connection="AmbulatoriDettaglio" BindingDescription-SourceTable="T_ANA_CONSULTORI"
											BindingDescription-SourceField="CNS_DESCRIZIONE" CampoCodice="CNS_CODICE" CampoDescrizione="CNS_DESCRIZIONE"
											Tabella="T_ANA_CONSULTORI"></ondp:wzFinestraModale></td>
								</tr>
								<tr>
									<td align="right">
										<asp:Label id="Label2" style="padding-right: 3px; padding-left: 3px" runat="server" CssClass="label">Apertura</asp:Label></td>
									<td colspan="2">
										<ondp:wzOnitDatePick id="dpkApertura" runat="server" Width="120px" Height="20px" CssStyles-CssRequired="textbox_data_obbligatorio"
											CssStyles-CssEnabled="textbox_data" CssStyles-CssDisabled="textbox_data_disabilitato" BindingField-SourceField="amb_data_apertura"
											BindingField-Hidden="False" BindingField-SourceTable="T_ANA_AMBULATORI" BindingField-Connection="AmbulatoriDettaglio"
											BindingField-Editable="always" target="dpkApertura"></ondp:wzOnitDatePick></td>
									<td align="right">
										<asp:Label id="Label3" style="padding-right: 3px; padding-left: 3px" runat="server" CssClass="label">Chiusura</asp:Label></td>
									<td>
										<ondp:wzOnitDatePick id="dpkChiusura" runat="server" Width="120px" Height="20px" CssStyles-CssRequired="textbox_data_obbligatorio"
											CssStyles-CssEnabled="textbox_data" CssStyles-CssDisabled="textbox_data_disabilitato" BindingField-SourceField="amb_data_chiusura"
											BindingField-Hidden="False" BindingField-SourceTable="T_ANA_AMBULATORI" BindingField-Connection="AmbulatoriDettaglio"
											BindingField-Editable="always" target="dpkChiusura"></ondp:wzOnitDatePick></td>
									<td align="right">
										<asp:Label id="lblPediatraVac" style="PADDING-RIGHT: 3px; PADDING-LEFT: 3px" runat="server"
											CssClass="label">Medico in amb.</asp:Label></td>
									<td>
										<ondp:wzCheckBox id="chkMedicoInAmbulatorio" style="POSITION: relative" runat="server" Height="12px"
											CssStyles-CssEnabled="Label" CssStyles-CssDisabled="Label_Disabilitato" BindingField-SourceField="AMB_MEDINAMB"
											BindingField-Hidden="False" BindingField-SourceTable="T_ANA_AMBULATORI" BindingField-Connection="AmbulatoriDettaglio"
											BindingField-Editable="always" MS_POSITIONING="GridLayout" BindingField-Value="N"></ondp:wzCheckBox></td>
								</tr>
							</table>
						</ondp:OnitDataPanel>
                    </div>
				</ondp:OnitDataPanel>
			</on_lay3:onitlayout3>
        </form>
	</body>
</html>
