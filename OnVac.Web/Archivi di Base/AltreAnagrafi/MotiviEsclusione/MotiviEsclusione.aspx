<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="MotiviEsclusione.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_MotiviEsclusione" %>

<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_otb" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitTable" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>Motivi Esclusione</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("onvac.common.js")%>'></script>
    <style type="text/css">
        .Infra2Dgr_Header {
            text-align: left;
        }
    </style>
</head>
<body>
    <form id="Form1" method="post" runat="server">
    <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Width="100%" Height="100%" TitleCssClass="Title3" Titolo="Motivi Esclusione">

        <ondp:onitdatapanel id="odpMotivoEsclusioneMaster" runat="server" configfile="MotiviEsclusione.odpMotivoEsclusioneMaster.xml" renderonlychildren="True" usetoolbar="False">
			<on_otb:onittable id="OnitTable1" runat="server" height="100%" width="100%">

				<on_otb:onitsection id="sezRicerca" runat="server" width="100%" typeHeight="content">
					<on_otb:onitcell id="cellaRicerca" runat="server" height="100%" Width="100%">

						<igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
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
                                <igtbar:TBSeparator></igtbar:TBSeparator>
								<igtbar:TBarButton Key="btnGestioneScadenza" Text="Gestione Scadenze" DisabledImage="../../../Images/scadenza_dis.gif" Image="../../../Images/scadenza.gif" ToolTip="Apre la gestione delle scadenze">
                                    <DefaultStyle CssClass="infratoolbar_button_default" Width="140px" ></DefaultStyle>
                                </igtbar:TBarButton>
							</Items>
						</igtbar:UltraWebToolbar>

						<div class="Sezione">Modulo ricerca</div>

						<ondp:wzFilter id="TabFiltri" runat="server" Width="100%" Height="70px" CssClass="InfraUltraWebTab2">
							<DefaultTabStyle CssClass="InfraTab_Default2"></DefaultTabStyle>
							<SelectedTabStyle CssClass="InfraTab_Selected2"></SelectedTabStyle>
							<DisabledTabStyle CssClass="InfraTab_Disabled2"></DisabledTabStyle>
							<HoverTabStyle CssClass="InfraTab_Hover2"></HoverTabStyle>
							<Tabs>
								<igtab:Tab Text="Ricerca di Base">
									<ContentTemplate>
										<table height="100%" cellSpacing="10" style="table-layout:fixed" cellPadding="0" width="100%" border="0">
											<tr>
												<td align="right" width="90">
													<asp:Label id="Label1" runat="server" CssClass="LABEL">Filtro di Ricerca</asp:Label></td>
												<TD>
													<asp:TextBox id="WzFilterKeyBase" runat="server" cssclass="textbox_stringa w100p"></asp:TextBox></TD>
											</tr>
										</table>
									</ContentTemplate>
								</igtab:Tab>
							</Tabs>
						</ondp:wzFilter>

						<div class="Sezione">Elenco</div>

					</on_otb:onitcell>
				</on_otb:onitsection>

				<on_otb:onitsection id="sezDgr" runat="server" width="100%" typeHeight="calculate">
					<on_otb:onitcell id="cellaDgr" runat="server" height="100%" Width="100%" typescroll="auto">

						<ondp:wzDataGrid Browser="UpLevel" id="dgrEsclusioni" runat="server" Width="100%" PagerVoicesBefore="-1" PagerVoicesAfter="-1" OnitStyle="False" EditMode="None">
							<Bands>
								<igtbl:UltraGridBand>
									<Columns>
										<igtbl:UltraGridColumn HeaderText="Codice" Key="" Width="5%" BaseColumnName="">
											<Footer Key=""></Footer>
											<Header Key="" Caption="Codice"></Header>
										</igtbl:UltraGridColumn>
										<igtbl:UltraGridColumn HeaderText="Descrizione" Key="" Width="45%" BaseColumnName="">
											<Footer Key=""></Footer>
											<Header Key="" Caption="Descrizione"></Header>
										</igtbl:UltraGridColumn>
										<igtbl:UltraGridColumn HeaderText="Descrizione Internazionale" Key="" Width="35%" BaseColumnName="">
											<Footer Key=""></Footer>
											<Header Key="" Caption="Descrizione Internazionale"></Header>
										</igtbl:UltraGridColumn>
										<igtbl:UltraGridColumn HeaderText="Codice AVN" Key="" Width="10%" BaseColumnName="">
											<Footer Key=""></Footer>
											<Header Key="" Caption="Codice AVN"></Header>
										</igtbl:UltraGridColumn>
										<igtbl:UltraGridColumn HeaderText="Obsoleto" Key="" Width="5%" BaseColumnName="">
											<Footer Key=""></Footer>
											<Header Key="" Caption="Obsoleto"></Header>
										</igtbl:UltraGridColumn>
									</Columns>
								</igtbl:UltraGridBand>
							</Bands>
							<DisplayLayout ColWidthDefault="" AutoGenerateColumns="False" RowHeightDefault="26px" Version="2.00"
								GridLinesDefault="None" SelectTypeRowDefault="Single" ScrollBar="Never" BorderCollapseDefault="Separate"
								CellPaddingDefault="3" RowSelectorsDefault="No" Name="dgrEsclusioni" CellClickActionDefault="RowSelect">
								<HeaderStyleDefault CssClass="Infra2Dgr_Header"></HeaderStyleDefault>
								<RowSelectorStyleDefault CssClass="Infra2Dgr_RowSelector"></RowSelectorStyleDefault>
								<FrameStyle Width="100%"></FrameStyle>
								<ActivationObject BorderWidth="0px" BorderColor="Navy"></ActivationObject>
								<SelectedRowStyleDefault CssClass="Infra2Dgr_SelectedRow"></SelectedRowStyleDefault>
								<RowAlternateStyleDefault CssClass="Infra2Dgr_RowAlternate"></RowAlternateStyleDefault>
								<RowStyleDefault CssClass="Infra2Dgr_Row"></RowStyleDefault>
							</DisplayLayout>
							<BindingColumns>
								<ondp:BindingFieldValue Value="" Editable="always" Description="Codice" Connection="connessioneMaster" SourceTable="T_ANA_MOTIVI_ESCLUSIONE"
									Hidden="False" SourceField="MOE_CODICE"></ondp:BindingFieldValue>
								<ondp:BindingFieldValue Value="" Editable="always" Description="Descrizione" Connection="connessioneMaster"
									SourceTable="T_ANA_MOTIVI_ESCLUSIONE" Hidden="False" SourceField="MOE_DESCRIZIONE"></ondp:BindingFieldValue>
								<ondp:BindingFieldValue Value="" Editable="always" Description="Descrizione Internazionale" Connection="connessioneMaster"
									SourceTable="T_ANA_MOTIVI_ESCLUSIONE" Hidden="False" SourceField="MOE_DESCRIZIONE_INTERNAZIONALE"></ondp:BindingFieldValue>
								<ondp:BindingFieldValue Value="" Editable="always" Description="Codice AVN" Connection="connessioneMaster"
									SourceTable="T_ANA_MOTIVI_ESCLUSIONE" Hidden="False" SourceField="MOE_CODICE_AVN"></ondp:BindingFieldValue>
								<ondp:BindingFieldValue Value="" Editable="always" Description="Obsoleto" Connection="connessioneMaster"
									SourceTable="T_ANA_MOTIVI_ESCLUSIONE" Hidden="False" SourceField="MOE_OBSOLETO"></ondp:BindingFieldValue>
							</BindingColumns>
						</ondp:wzdatagrid>

					</on_otb:onitcell>
				</on_otb:onitsection>

				<on_otb:onitsection id="sezDettagli" runat="server" width="100%" TypeHeight="Content">
					<on_otb:onitcell id="cellaDettagli" runat="server" height="100%" Width="100%">

						<div class="Sezione">Dettaglio</div>

						<ondp:onitdatapanel id="odpMotivoEsclusioneDetail" runat="server" ConfigFile="MotiviEsclusione.odpMotivoEsclusioneDetail.xml"
							renderOnlyChildren="True" useToolbar="False" dontLoadDataFirstTime="True" externalToolBar-Length="7"
							externalToolBar="ToolBar" BindingFields="(Insieme)">
							<table style="table-layout: fixed" cellspacing="3" width="100%" border="0">
								<colgroup>
									<col width="10%" />
									<col width="10%" />
                                    <col width="10%" />
                                    <col width="32%" />
									<col width="10%" />
									<col width="10%" />
									<col width="8%" />
									<col width="10%" />
								</colgroup>
								<tr align="left">
									<td class="label" >Codice</td>
									<td colspan="3">
										<ondp:wzTextBox id="txtCodice" runat="server" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
                                            onblur="toUpper(this);controlloCampoCodice(this);"
											CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
											BindingField-Editable="onNew" BindingField-Connection="connessioneSec"
											BindingField-SourceTable="T_ANA_MOTIVI_ESCLUSIONE" BindingField-Hidden="False" maxlength="4"
											BindingField-SourceField="MOE_CODICE"></ondp:wzTextBox></td>
									<td class="label">Cod. Esterno</td>
									<td colspan="3">
										<ondp:wzTextBox id="txtCodiceEsterno" runat="server" onblur="toUpper(this);"
											CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
											BindingField-Editable="always" BindingField-Connection="connessioneSec"
											BindingField-SourceTable="T_ANA_MOTIVI_ESCLUSIONE" BindingField-Hidden="False" maxlength="10"
											BindingField-SourceField="MOE_CODICE_ESTERNO"></ondp:wzTextBox></td>
								</tr>
								<tr>
									<td class="label">Descrizione</td>
									<td colSpan="3">
										<ondp:wzTextBox id="txtDescrizione" runat="server" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
                                            onblur="toUpper(this);" maxlength="100"
											CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
											BindingField-Editable="always" BindingField-Connection="connessioneSec"
											BindingField-SourceTable="T_ANA_MOTIVI_ESCLUSIONE" BindingField-Hidden="False"
											BindingField-SourceField="MOE_DESCRIZIONE"></ondp:wzTextBox></td>
									<td class="label">Descrizione Internazionale</td>
									<td colSpan="3">
										<ondp:wzTextBox id="txtDescInternazionale" runat="server" CssStyles-CssRequired="TextBox_Stringa w100p"
                                            onblur="toUpper(this);" maxlength="100"
											CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
											BindingField-Editable="always" BindingField-Connection="connessioneSec"
											BindingField-SourceTable="T_ANA_MOTIVI_ESCLUSIONE" BindingField-Hidden="False"
											BindingField-SourceField="MOE_DESCRIZIONE_INTERNAZIONALE"></ondp:wzTextBox></td>
								</tr>
								<tr>
									<td class="label">Genera inadempienza</td>
									<td>
										<ondp:wzDropDownList id="ddlGeneraInadempienza" runat="server" Width="100%" CssStyles-CssRequired="textbox_stringa"
											CssStyles-CssEnabled="textbox_stringa" CssStyles-CssDisabled="textbox_stringa_disabilitato" BindingField-Editable="always"
											BindingField-Connection="connessioneSec" BindingField-SourceTable="T_ANA_MOTIVI_ESCLUSIONE" BindingField-Hidden="False"
											BindingField-SourceField="MOE_GENERA_INAD" BindingField-Value="N">
											<asp:ListItem Value="N" Selected="True">NO</asp:ListItem>
											<asp:ListItem Value="S">SI</asp:ListItem>
										</ondp:wzDropDownList>
                                    </td>
                                    <td class="label">
                                        <asp:Label ID="lblCalcoloCoperture" runat="server" Text="Calcolo Coperture" />
                                    </td>
									<td>
                                        <ondp:wzRadioButtonList ID="rblCalcoloCoperture" runat="server"  RepeatDirection="Horizontal" CssStyles-CssRequired="textbox_stringa" CssStyles-CssEnabled="textbox_stringa" CssStyles-CssDisabled="textbox_stringa" BindingField-Editable="always" BindingField-Connection="connessioneSec" BindingField-SourceTable="T_ANA_MOTIVI_ESCLUSIONE" BindingField-Hidden="False" BindingField-SourceField="MOE_CALCOLO_COPERTURA" BindingField-Value="N" IncludeNull="false">
                                            <asp:ListItem Text="Normale" Value="N" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="Immunità" Value="I"></asp:ListItem>
                                            <asp:ListItem Text="Non Vaccinabilità" Value="V"></asp:ListItem>
                                        </ondp:wzRadioButtonList> 
                                    </td>
									<td class="label">Default per inadempienza</td>
									<td>
										<ondp:wzDropDownList id="ddlDefaultInadempienza" runat="server" Width="100%" CssStyles-CssRequired="textbox_stringa"
											CssStyles-CssEnabled="textbox_stringa" CssStyles-CssDisabled="textbox_stringa_disabilitato" BindingField-Editable="always" 
                                            BindingField-Connection="connessioneSec" BindingField-SourceTable="T_ANA_MOTIVI_ESCLUSIONE" BindingField-Hidden="False" 
                                            BindingField-SourceField="MOE_DEFAULT_INAD" BindingField-Value="N">
											<asp:ListItem Value="N" Selected="True">NO</asp:ListItem>
											<asp:ListItem Value="S">SI</asp:ListItem>
										</ondp:wzDropDownList></td>
                                    <td class="label">Stampa certificato</td>
                                    <td>
										<ondp:wzDropDownList id="ddlStampaCertificato" runat="server" Width="100%" CssStyles-CssRequired="textbox_stringa"
											CssStyles-CssEnabled="textbox_stringa" CssStyles-CssDisabled="textbox_stringa_disabilitato" BindingField-Editable="always"
											BindingField-Connection="connessioneSec" BindingField-SourceTable="T_ANA_MOTIVI_ESCLUSIONE" BindingField-Hidden="False"
											BindingField-SourceField="MOE_FLAG_STAMPA_CERTIFICATO" BindingField-Value="N">
											<asp:ListItem Value="N" Selected="True">NO</asp:ListItem>
											<asp:ListItem Value="S">SI</asp:ListItem>
										</ondp:wzDropDownList>  
                                    </td>
								</tr>
                                <tr>
                                    <td class="label">Centralizzazione</td>
                                    <td>
										<ondp:wzDropDownList id="ddlFlagCentralizzato" runat="server" Width="100%" CssStyles-CssRequired="textbox_stringa"
											CssStyles-CssEnabled="textbox_stringa" CssStyles-CssDisabled="textbox_stringa_disabilitato" BindingField-Editable="always"
											BindingField-Connection="connessioneSec" BindingField-SourceTable="T_ANA_MOTIVI_ESCLUSIONE" BindingField-Hidden="False"
											BindingField-SourceField="MOE_FLAG_CENTRALIZZATO" BindingField-Value="N">
											<asp:ListItem Value="N" Selected="True">NO</asp:ListItem>
											<asp:ListItem Value="S">SI</asp:ListItem>
										</ondp:wzDropDownList>                                    
                                    </td>
                                    <td class="label">
                                         <asp:Label ID="lblCalcoloScadenza" runat="server" Text="Calcolo scadenza" />
                                    </td>
                                    <td>
                                        <ondp:wzDropDownList id="ddlCalcoloScadenza" runat="server" Width="100%" CssStyles-CssRequired="textbox_stringa" CssStyles-CssEnabled="textbox_stringa" CssStyles-CssDisabled="textbox_stringa_disabilitato" BindingField-Editable="always" BindingField-Connection="connessioneSec" BindingField-SourceTable="T_ANA_MOTIVI_ESCLUSIONE" BindingField-Hidden="False" BindingField-SourceField="MOE_CALCOLO_SCADENZA" BindingField-Value="N">
											<asp:ListItem Value="0" Selected="True">Nessun calcolo</asp:ListItem>
                                            <asp:ListItem Value="1">Nascita</asp:ListItem>
											<asp:ListItem Value="2">Registrazione</asp:ListItem>
                                            <asp:ListItem Value="3">Visita</asp:ListItem>
										</ondp:wzDropDownList>
                                    </td>
                                    <td class="label">Obsoleto</td>
                                    <td>
										<ondp:wzDropDownList id="ddlObsoleto" runat="server" Width="100%" CssStyles-CssRequired="textbox_stringa"
											CssStyles-CssEnabled="textbox_stringa" CssStyles-CssDisabled="textbox_stringa_disabilitato" BindingField-Editable="always" 
                                            BindingField-Connection="connessioneSec" BindingField-SourceTable="T_ANA_MOTIVI_ESCLUSIONE" BindingField-Hidden="False" 
                                            BindingField-SourceField="MOE_OBSOLETO" BindingField-Value="N">
											<asp:ListItem Value="N" Selected="True">NO</asp:ListItem>
											<asp:ListItem Value="S">SI</asp:ListItem>
										</ondp:wzDropDownList></td>
                                    <td class="label">Codice AVN</td>
                                    <td>
                                        <ondp:wzTextBox id="txtAvn" runat="server" Width="100%" MaxLength="2" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p"
										    CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p"
										    BindingField-Editable="always" BindingField-Connection="connessione" BindingField-SourceTable="T_ANA_MOTIVI_ESCLUSIONE"
										    BindingField-Hidden="False" BindingField-SourceField="MOE_CODICE_AVN"></ondp:wzTextBox></td>
                                </tr>
							</table>
						</ondp:onitdatapanel>

					</on_otb:onitcell>
				</on_otb:onitsection>

			</on_otb:onittable>
		</ondp:onitdatapanel>
    </on_lay3:OnitLayout3>
    </form>
</body>
</html>
