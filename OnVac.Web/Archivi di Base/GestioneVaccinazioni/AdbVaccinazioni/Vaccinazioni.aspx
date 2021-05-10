<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Vaccinazioni.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_Vaccinazioni"%>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<%@ Register TagPrefix="uc1" TagName="MotivoEsc" Src="../../../Common/Controls/MotivoEsc.ascx" %>
<%@ Register TagPrefix="uc1" TagName="CodificaAssociazione" Src="../../../Common/Controls/CodificaEsternaVaccinazioneAssociazione.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>Vaccinazioni</title>
        
        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/archivi.css") %>' type="text/css" rel="stylesheet" />

        <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("onvac.common.js")%>'></script>
        
        <!-- patch per il click sul checkbox dell'header per la selezione di tutti i checkbox -->
        <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("patch_selectAll.js")%>'></script>

		<script type="text/javascript">
            function InizializzaToolBar(t) {
                t.PostBackButton = false;
            }

            //controllo campo vac_ordine (modifica 30/06/2004)
            function ToolBarClick(ToolBar, button, evnt) {
                evnt.needPostBack = true;
                switch (button.Key) {
                    case 'btnSalva':
                        ordine = document.getElementById('Wztextbox1').value;
                        if (isNaN(ordine)) {
                            alert("Inserire un numero nel campo 'Ordine'!");
                            evnt.needPostBack = false;
                        }

                        if (document.getElementById('chkControllo18anni').checked) {
                            minimoDosi = document.getElementById('txtMinimoDosi').value;
                            if (minimoDosi == '' || isNaN(minimoDosi)) {
                                alert("Inserire un numero nel campo 'Minimo Dosi'!");
                                evnt.needPostBack = false;
                            }
                        }

                        break;
                }
            }
		</script>
	</head>
	<body onload="registerCheckClick('WzDgrVaccinazioni__ctl1_ChkMultiSel');">
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" height="100%" width="100%" Titolo="Vaccinazioni" TitleCssClass="Title3">
				<ondp:OnitDataPanel id="OdpVaccinazioniMaster" runat="server" ConfigFile="Vaccinazioni.OdpVaccinazioniMaster.xml "
					Width="100%" Height="100%" renderOnlyChildren="True" useToolbar="False">
                    <div>
						<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBarVaccinazioni" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                            <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                            <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
							<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
							<Items>
								<igtbar:TBarButton Key="btnCerca" Text="Cerca"></igtbar:TBarButton>
								<igtbar:TBSeparator></igtbar:TBSeparator>
								<igtbar:TBarButton Key="btnNew" Text="Nuovo"></igtbar:TBarButton>
								<igtbar:TBarButton Key="btnEdit" Text="Modifica" ></igtbar:TBarButton>
								<igtbar:TBarButton Key="btnElimina" Text="Elimina" ></igtbar:TBarButton>
								<igtbar:TBSeparator></igtbar:TBSeparator>
								<igtbar:TBarButton Key="btnSalva" Text="Salva" ></igtbar:TBarButton>
								<igtbar:TBarButton Key="btnAnnulla" Text="Annulla" ></igtbar:TBarButton>
                                <igtbar:TBSeparator></igtbar:TBSeparator>
                                <igtbar:TBarButton Key="btnCodificaAssociazione" Text="Codifica Associazione" DisabledImage="../../../images/codificaAssociazioni_dis.png" Image="../../../images/codificaAssociazioni.png" >
                                    <DefaultStyle CssClass="infratoolbar_button_default" Width="150px"></DefaultStyle>
                                </igtbar:TBarButton>
                                <igtbar:TBSeparator></igtbar:TBSeparator>
                                <igtbar:TBarButton Key="btnInfo" Text="Info"></igtbar:TBarButton>
                              
							</Items>
						</igtbar:UltraWebToolbar>
                    </div>
					<div class="vac-sezione">Modulo ricerca</div>
                    <div>
						<ondp:wzFilter style="Z-INDEX: 0" id="WzFilter1" runat="server" Width="100%" Height="70px" CssClass="InfraUltraWebTab2">
							<SelectedTabStyle CssClass="InfraTab_Selected2"></SelectedTabStyle>
							<DisabledTabStyle CssClass="InfraTab_Disabled2"></DisabledTabStyle>
							<HoverTabStyle CssClass="InfraTab_Hover2"></HoverTabStyle>
							<DefaultTabStyle CssClass="InfraTab_Default2"></DefaultTabStyle>
							<Tabs>
								<igtab:Tab Text="Ricerca di Base">
									<ContentTemplate>
										<table height="100%" cellspacing="10" cellpadding="0" width="100%" border="0" style="table-layout:fixed">
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
					<div class="vac-sezione">Elenco</div>

                    <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                        <ondp:wzMsDataGrid id="WzDgrVaccinazioni" runat="server" Width="100%" EditMode="None" OnitStyle="False"
	                        PagerVoicesBefore="-1" PagerVoicesAfter="-1" AutoGenerateColumns="False" SelectionOption="rowClick">
	                        <HeaderStyle CssClass="header"/>
	                        <ItemStyle CssClass="item"/>
	                        <AlternatingItemStyle CssClass="alternating"/>
	                        <EditItemStyle CssClass="edit"/>
	                        <SelectedItemStyle CssClass="selected"/>
	                        <PagerStyle CssClass="pager"/>
	                        <FooterStyle CssClass="footer"/>
	                        <Columns>
		                        <ondp:wzMultiSelColumn></ondp:wzMultiSelColumn>
		                        <ondp:wzBoundColumn HeaderText="Ordine" Key="VAC_ORDINE" SourceField="VAC_ORDINE" SourceTable="T_ANA_VACCINAZIONI" SourceConn="vacConn">
			                        <HeaderStyle width="5%" />
		                        </ondp:wzBoundColumn>
		                        <ondp:wzBoundColumn HeaderText="Codice" Key="VAC_CODICE" SourceField="VAC_CODICE" SourceTable="T_ANA_VACCINAZIONI" SourceConn="vacConn">
			                        <HeaderStyle width="10%" />
		                        </ondp:wzBoundColumn>
		                        <ondp:wzBoundColumn HeaderText="Descrizione" Key="VAC_DESCRIZIONE" SourceField="VAC_DESCRIZIONE" SourceTable="T_ANA_VACCINAZIONI" SourceConn="vacConn">
			                        <HeaderStyle width="35%" />
		                        </ondp:wzBoundColumn>
		                        <ondp:wzBoundColumn HeaderText="Descrizione Internazionale" Key="VAC_DESCRIZIONE_INTERNAZIONALE" SourceField="VAC_DESCRIZIONE_INTERNAZIONALE" SourceTable="T_ANA_VACCINAZIONI" SourceConn="vacConn">
			                        <HeaderStyle width="30%" />
 		                        </ondp:wzBoundColumn>
		                        <ondp:wzBoundColumn HeaderText="Obbligatoriet&#224" Key="COD_DESCRIZIONE" SourceField="COD_DESCRIZIONE" SourceTable="T_ANA_CODIFICHE" SourceConn="vacConn">
			                        <HeaderStyle width="15%" />
		                        </ondp:wzBoundColumn>
		                        <ondp:wzBoundColumn HeaderText="Codice AVN" Key="VAC_CODICE_AVN" SourceField="VAC_CODICE_AVN" SourceTable="T_ANA_VACCINAZIONI" SourceConn="vacConn">
			                        <HeaderStyle width="5%" />
 		                        </ondp:wzBoundColumn>
	                        </Columns>
	                        <BindingColumns>
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="Ordine" Connection="vacConn" SourceTable="T_ANA_VACCINAZIONI"
			                        Hidden="False" SourceField="VAC_ORDINE"></ondp:BindingFieldValue>
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="Codice" Connection="vacConn" SourceTable="T_ANA_VACCINAZIONI"
			                        Hidden="False" SourceField="VAC_CODICE"></ondp:BindingFieldValue>
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="Descrizione" Connection="vacConn" SourceTable="T_ANA_VACCINAZIONI"
			                        Hidden="False" SourceField="VAC_DESCRIZIONE"></ondp:BindingFieldValue>
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="DescrInt" Connection="vacConn" SourceTable="T_ANA_VACCINAZIONI"
			                        Hidden="False" SourceField="VAC_DESCRIZIONE_INTERNAZIONALE"></ondp:BindingFieldValue>
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="Obbligatoriet&#224;" Connection="vacConn"
			                        SourceTable="T_ANA_CODIFICHE" Hidden="False" SourceField="COD_DESCRIZIONE"></ondp:BindingFieldValue>
		                        <ondp:BindingFieldValue Value="" Editable="always" Description="codAVN" Connection="vacConn" SourceTable="T_ANA_VACCINAZIONI"
			                        Hidden="False" SourceField="VAC_CODICE_AVN"></ondp:BindingFieldValue>
	                        </BindingColumns>
                        </ondp:wzMsDataGrid>
					</dyp:DynamicPanel>

					<div class="vac-sezione">Dettaglio</div>
                    <div>
						<ondp:OnitDataPanel id="OdpVaccinazioniDetail" runat="server" useToolbar="False" renderOnlyChildren="True"
							ConfigFile="Vaccinazioni.OdpVaccinazioniDetail.xml" Width="100%"  dontLoadDataFirstTime="True" externalToolBar="ToolBarVaccinazioni"
                            externalToolBar-Length="19" BindingFields="(Insieme)" >
							<table style="table-layout: fixed" id="Table1" border="0" cellspacing="3" cellpadding="0" width="100%">
                                <colgroup>
                                    <col style="width: 12%" />
                                    <col style="width: 2%" />
                                    <col style="width: 15%" />
                                    <col style="width: 2%" />
                                    <col style="width: 12%" />
                                    <col style="width: 10%" />
                                    <col style="width: 11%" />
                                    <col style="width: 12%" />
                                    <col style="width: 11%" />
                                    <col style="width: 13%" />
                                </colgroup>
								<tr>
									<td class="label">Codice</td>
									<td colspan="3">
										<ondp:wzTextBox onblur="toUpper(this);controlloCampoCodice(this)" style="position: relative" id="WzTbCodVac" runat="server"
											MaxLength="8" ms_positioning="GridLayout" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p"
											CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p"
											BindingField-Editable="onNew" BindingField-Connection="vacDatiConn" BindingField-SourceTable="T_ANA_VACCINAZIONI"
											BindingField-Hidden="False" BindingField-SourceField="VAC_CODICE"></ondp:wzTextBox></td>
									<td class="label" >Codice Esterno</td>
									<td>
										<ondp:wzTextBox onblur="toUpper(this);" style="position: relative" id="txtCodiceEsterno" runat="server"
											Width="100%" MaxLength="10" ms_positioning="GridLayout" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p"
											CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa w100p" BindingField-Editable="always"
											BindingField-Connection="vacDatiConn" BindingField-SourceTable="T_ANA_VACCINAZIONI" BindingField-Hidden="False"
											BindingField-SourceField="VAC_CODICE_ESTERNO"></ondp:wzTextBox></td>
									<td class="label">Ordine</td>
									<td>
										<ondp:wzTextBox onblur="toUpper(this);" style="position: relative" id="Wztextbox1" runat="server"
											Width="100%" MaxLength="2" ms_positioning="GridLayout" CssStyles-CssDisabled="textbox_stringa_disabilitato"
											CssStyles-CssEnabled="textbox_stringa" CssStyles-CssRequired="textbox_stringa" BindingField-Editable="always"
											BindingField-Connection="vacDatiConn" BindingField-SourceTable="T_ANA_VACCINAZIONI" BindingField-Hidden="False"
											BindingField-SourceField="VAC_ORDINE"></ondp:wzTextBox></td>
									<td class="label" align="right">Obbligatorietà</td>
									<td>
										<ondp:wzDropDownList style="position: relative" id="WzDropDownList1" runat="server" Width="100%" ms_positioning="GridLayout"
											CssStyles-CssDisabled="textbox_stringa_disabilitato w100p" CssStyles-CssEnabled="textbox_stringa w100p"
											CssStyles-CssRequired="textbox_stringa_obbligatorio w100p" BindingField-Editable="always" BindingField-Connection="vacDatiConn"
											BindingField-SourceTable="T_ANA_VACCINAZIONI" BindingField-Hidden="False" BindingField-SourceField="VAC_OBBLIGATORIA"
											SourceConnection="vacDatiConn" TextFieldName="COD_DESCRIZIONE" KeyFieldName="COD_CODICE" SourceTable="T_ANA_CODIFICHE"
											DataFilter="cod_campo='VAC_OBBLIGATORIA'" OtherListFields="cod_campo"></ondp:wzDropDownList></td>
								</tr>
								<tr>
									<td class="label">Descrizione</td>
									<td colspan="5">
										<ondp:wzTextBox onblur="toUpper(this);" style="position: relative" id="WzTbDescVac" runat="server"
											Width="100%" MaxLength="30" ms_positioning="GridLayout" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p"
											CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p"
											BindingField-Editable="always" BindingField-Connection="vacDatiConn" BindingField-SourceTable="T_ANA_VACCINAZIONI"
											BindingField-Hidden="False" BindingField-SourceField="VAC_DESCRIZIONE"></ondp:wzTextBox></td>
									<td class="label">Descrizione Internazionale</td>
									<td colspan="4">
										<ondp:wzTextBox style="position: relative" id="WzTbDescVacInt" runat="server"
											Width="100%" MaxLength="50" ms_positioning="GridLayout" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p"
											CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa w100p"
											BindingField-Editable="always" BindingField-Connection="vacDatiConn" BindingField-SourceTable="T_ANA_VACCINAZIONI"
											BindingField-Hidden="False" BindingField-SourceField="VAC_DESCRIZIONE_INTERNAZIONALE"></ondp:wzTextBox></td>
								</tr>
								<tr>
									<td></td>
									<td colspan="5">
										<table border="0" cellspacing="0" cellpadding="0" width="100%">
                                            <colgroup>
                                                <col />
                                                <col style="width:130px" />
                                            </colgroup>
											<tr>
												<td>
													<ondp:wzTextBox style="position: relative" id="WzTbCodMotEscl" runat="server" MaxLength="8" ms_positioning="GridLayout"
														CssStyles-CssDisabled="textbox_stringa_disabilitato w100p" CssStyles-CssEnabled="textbox_stringa w100p"
														CssStyles-CssRequired="textbox_stringa_obbligatorio w100p" BindingField-Editable="never" BindingField-Connection="vacDatiConn"
														BindingField-SourceTable="T_ANA_VACCINAZIONI" BindingField-Hidden="False" BindingField-SourceField="VAC_MOT_ESCLUSIONE"></ondp:wzTextBox></td>
												<td>
													<asp:Button id="btnMotiviEsclusione" runat="server" Text="Motivi Esclusione" Width="130px"></asp:Button></td>
											</tr>
										</table>
									</td>
									<td align="right">
										<asp:Label id="Label12" runat="server" CssClass="label">Sesso</asp:Label></td>
									<td align="left" colspan="3">
										<ondp:wzDropDownList id="ddlSceltaSesso" runat="server" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p"
											CssStyles-CssEnabled="textbox_stringa w100p" BindingField-Editable="always" BindingField-Connection="vacDatiConn"
											BindingField-SourceTable="T_ANA_VACCINAZIONI" BindingField-Hidden="False" BindingField-SourceField="VAC_SESSO">
											<asp:ListItem Value="E" Selected="True">ENTRAMBI</asp:ListItem>
											<asp:ListItem Value="M">MASCHI</asp:ListItem>
											<asp:ListItem Value="F">FEMMINE</asp:ListItem>
										</ondp:wzDropDownList></td>
								</tr>
								<tr>											
									<td class="label">Controllo 18 anni</td>
									<td>
										<ondp:wzCheckBox id="chkControllo18anni" runat="server" Height="12px" CssStyles-CssDisabled="Label_Disabilitato"
											CssStyles-CssEnabled="Label" BindingField-Editable="always" BindingField-Connection="vacDatiConn"
											BindingField-SourceTable="T_ANA_VACCINAZIONI" BindingField-Hidden="False" BindingField-SourceField="VAC_CONTROLLO_MAGGIORENNI"
											BindingField-Value="N" AutoPostBack="True"></ondp:wzCheckBox></td>
                                    <td class="label">Discrezionale</td>
                                    <td>
                                        <ondp:wzCheckBox id="chkDiscrezionale" runat="server" Height="12px" CssStyles-CssDisabled="Label_Disabilitato"
											CssStyles-CssEnabled="Label" BindingField-Editable="always" BindingField-Connection="vacDatiConn"
											BindingField-SourceTable="T_ANA_VACCINAZIONI" BindingField-Hidden="False" BindingField-SourceField="VAC_DISCREZIONALE"
											BindingField-Value="N"></ondp:wzCheckBox></td>
									<td class="label">Mostra in APP</td>
                                    <td>
                                        <ondp:wzCheckBox id="chkShowInApp" runat="server" Height="12px" CssStyles-CssDisabled="Label_Disabilitato"
											CssStyles-CssEnabled="Label" BindingField-Editable="always" BindingField-Connection="vacDatiConn"
											BindingField-SourceTable="T_ANA_VACCINAZIONI" BindingField-Hidden="False" BindingField-SourceField="VAC_SHOW_IN_APP"
											BindingField-Value="N"></ondp:wzCheckBox></td>
									<td class="label">N. minimo dosi</td>
									<td>
										<ondp:wzOnitJsValidator style="position: relative" id="txtMinimoDosi" runat="server" Width="100%" MaxLength="2"
											ms_positioning="GridLayout" CssStyles-CssDisabled="textbox_stringa_disabilitato" CssStyles-CssEnabled="textbox_stringa" CssStyles-CssRequired="textbox_stringa_obbligatorio"
											BindingField-Editable="always" BindingField-Connection="vacDatiConn" BindingField-SourceTable="T_ANA_VACCINAZIONI"
											BindingField-Hidden="False" BindingField-SourceField="VAC_MINIMO_DOSI" SetOnChange="True" CustomValFunction="validaNumero"
											CustomValFunction-Length="2" validationType="Validate_custom" autoFormat="True" actionUndo="True" actionSelect="False"
											actionMessage="True" actionFocus="True" actionDelete="False" actionCorrect="False">
											<Parameters>
												<on_val:ValidationParam paramValue="2" paramOrder="0" paramType="number" paramName="numCifreIntere"></on_val:ValidationParam>
												<on_val:ValidationParam paramValue="0" paramOrder="1" paramType="number" paramName="numCifreDecimali"></on_val:ValidationParam>
												<on_val:ValidationParam paramValue="0" paramOrder="2" paramType="number" paramName="minValore"></on_val:ValidationParam>
												<on_val:ValidationParam paramValue="99" paramOrder="3" paramType="number" paramName="maxValore"></on_val:ValidationParam>
												<on_val:ValidationParam paramValue="true" paramOrder="4" paramType="boolean" paramName="blnCommaSeparator"></on_val:ValidationParam>
											</Parameters>
										</ondp:wzOnitJsValidator></td>                                   
                                    <td class="label">
                                        <asp:Label ID="LabelCodiceCvx" runat="server" Text="<%$ Resources: Onit.OnAssistnet.OnVac.Web, Vaccinazioni.LabelCodiceCvx%>"></asp:Label>
                                    </td>
                                    <td>
                                        <ondp:wzTextBox id="WzTxtCvx" runat="server" Width="100%" MaxLength="10" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p"
										    CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p"
										    BindingField-Editable="always" BindingField-Connection="connessione" BindingField-SourceTable="T_ANA_VACCINAZIONI"
										    BindingField-Hidden="False" BindingField-SourceField="VAC_CODICE_FSE"></ondp:wzTextBox></td>
                                    <td></td>
    							</tr>
								<tr style="display:none">
									<td class="label">Sostituta</td>
									<td colspan="5">
										<ondp:wzDropDownList style="position: relative" id="Wzdropdownlist2" runat="server" Width="100%" ms_positioning="GridLayout"
											CssStyles-CssDisabled="textbox_data_disabilitato w100p" CssStyles-CssEnabled="textbox_data w100p"
											CssStyles-CssRequired="textbox_data w100p" BindingField-Editable="always" BindingField-Connection="vacDatiConn"
											BindingField-SourceTable="T_ANA_VACCINAZIONI" BindingField-Hidden="False" BindingField-SourceField="VAC_COD_SOSTITUTA"
											SourceConnection="vacDatiConn" TextFieldName="VAC_DESCRIZIONE" KeyFieldName="VAC_CODICE" SourceTable="T_ANA_VACCINAZIONI"
											IncludeNull="True"></ondp:wzDropDownList></td>
									<td colspan="4"></td>
								</tr>
                                <tr>
                                    <td class="label">Tipo Codifica</td>
                                    <td colspan="3">
                                          <ondp:wzDropDownList style="position: relative" id="ddlTipoCodifica" runat="server" Width="100%" ms_positioning="GridLayout"
													CssStyles-CssDisabled="textbox_stringa_disabilitato w100p" CssStyles-CssEnabled="textbox_stringa w100p"
													CssStyles-CssRequired="textbox_stringa_obbligatorio w100p" BindingField-Editable="always" BindingField-Connection="vacDatiConn" IncludeNull="True"
													BindingField-SourceTable="T_ANA_VACCINAZIONI" BindingField-Hidden="False" BindingField-SourceField="VAC_TIPO_CODICE_AVN"
													SourceConnection="vacDatiConn" TextFieldName="COD_DESCRIZIONE" KeyFieldName="COD_CODICE" SourceTable="T_ANA_CODIFICHE"
													DataFilter="cod_campo='VAC_TIPO_CODICE_AVN'" OtherListFields="cod_campo"></ondp:wzDropDownList>
                                    </td>
                                    <td class="label">Estrai AVN</td>
                                    <td>
                                          <ondp:wzCheckBox id="chkEstraiAvn" runat="server" Height="12px" CssStyles-CssDisabled="Label_Disabilitato"
											    CssStyles-CssEnabled="Label" BindingField-Editable="always" BindingField-Connection="vacDatiConn"
											    BindingField-SourceTable="T_ANA_VACCINAZIONI" BindingField-Hidden="False" BindingField-SourceField="VAC_FLAG_ESTRAI_AVN"
											    BindingField-Value="N"></ondp:wzCheckBox></td>
                                    <td class="label">Default AVN</td>
                                    <td>
                                          <ondp:wzCheckBox id="chkDefaultAvn" runat="server" Height="12px" CssStyles-CssDisabled="Label_Disabilitato"
											    CssStyles-CssEnabled="Label" BindingField-Editable="always" BindingField-Connection="vacDatiConn"
											    BindingField-SourceTable="T_ANA_VACCINAZIONI" BindingField-Hidden="False" BindingField-SourceField="VAC_FLAG_DEFAULT_AVN"
											    BindingField-Value="S"></ondp:wzCheckBox></td>
                                    <td class="label">Codice AVN</td>
                                    <td>
                                        <ondp:wzTextBox id="WzTxtAvn" onblur="toUpper(this);" runat="server" Width="100%" MaxLength="4" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p"
										    CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p"
										    BindingField-Editable="always" BindingField-Connection="connessione" BindingField-SourceTable="T_ANA_VACCINAZIONI"
										    BindingField-Hidden="False" BindingField-SourceField="VAC_CODICE_AVN"></ondp:wzTextBox></td>
                                </tr>
							</table>
						</ondp:OnitDataPanel>
                    </div>

					<on_ofm:OnitFinestraModale id="modInsMotivoEsc" title="Inserisci Esclusione" runat="server" width="618px"  BackColor="LightGray" NoRenderX="True">
						<uc1:MotivoEsc id="InsMotivoEsc1" runat="server"></uc1:MotivoEsc>
					</on_ofm:OnitFinestraModale>

				</ondp:OnitDataPanel>
			</on_lay3:onitlayout3>
            
            <on_ofm:OnitFinestraModale ID="fmCodificaAssociazioni" Title="Codifica esterna vaccinazioni per associazione" runat="server" Width="750px" Height="500px" BackColor="LightGray" NoRenderX="true">
                <uc1:CodificaAssociazione ID="ucCodificaAssociazione" runat="server" />
            </on_ofm:OnitFinestraModale>

        </form>
	</body>
</html>
