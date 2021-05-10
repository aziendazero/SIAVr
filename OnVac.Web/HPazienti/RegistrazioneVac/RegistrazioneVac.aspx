<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RegistrazioneVac.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.RegistrazioneVac" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_ocb" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitCombo" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>

<%@ Register TagPrefix="uc1" TagName="AggiungiVaccinazione" Src="AggiungiVaccinazione.ascx" %>
<%@ Register Src="~/Common/Controls/PagamentoVaccinazione.ascx" TagPrefix="uc1" TagName="PagamentoVaccinazione" %>

<%@ Import Namespace="Onit.OnAssistnet.OnVac" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Registrazione Vaccinazioni</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/toolbar.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/default/toolbar.default.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript" src="RegistrazioneVacScript.js"></script>	
</head>
<body>
    <form id="Form1" method="post" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
    <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" TitleCssClass="Title3" Width="100%" Height="100%" Titolo="Registrazione Vaccinazioni">

		<div class="title" id="divLayTitolo">
			<asp:Label id="LayoutTitolo" runat="server" CssClass="title" Width="100%" BorderStyle="None"></asp:Label>
        </div>
        
        <div>
            <telerik:RadToolBar ID="Toolbar" runat="server" Width="100%" Skin="Default" EnableEmbeddedSkins="false" EnableAjaxSkinRendering="false" 
                EnableEmbeddedBaseStylesheet="false" OnButtonClick="Toolbar_ButtonClick" OnClientButtonClicking="OnClientButtonClicking">
                <Items>
                    <telerik:RadToolBarButton runat="server" Text="Salva" Value="btnSalva" ImageUrl="~/Images/salva.gif" DisabledImageUrl="~/Images/salva_dis.gif"></telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Text="Annulla" Value="btnAnnulla" ImageUrl="~/Images/annulla.gif" DisabledImageUrl="~/Images/annulla_dis.gif"></telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Text="Elimina" Value="btnElimina" ImageUrl="~/Images/annullaconf.gif" DisabledImageUrl="~/Images/annullaconf_dis.gif"></telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Text="Cal seduta" Value="btnCalcolaSeduta" ImageUrl="../../images/rotella.gif" DisabledImageUrl="../../images/rotella_dis.gif"></telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Text="Ins. assoc" Value="btnAggiungiAssAssociate" ImageUrl="~/Images/nuovo.gif" DisabledImageUrl="~/Images/nuovo_dis.gif"></telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Text="Esegui" Value="btnEsegui" ImageUrl="../../images/esegui.gif" DisabledImageUrl="../../images/esegui_dis.gif"></telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Text="Recupera" Value="btnRecuperaStoricoVacc" ImageUrl="../../images/recupera.png" DisabledImageUrl="../../images/recupera_dis.png" ToolTip="Recupera lo storico vaccinale centralizzato del paziente"></telerik:RadToolBarButton>
                </Items>
            </telerik:RadToolBar>
        </div>
        
        <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
		    <asp:DataGrid id="dgrVaccinazioni" width="100%" style="padding-top:3px;" runat="server" CssClass="DataGrid" AutoGenerateColumns="False">
				<SelectedItemStyle Font-Bold="True" CssClass="Selected"></SelectedItemStyle>
				<AlternatingItemStyle BackColor="WhiteSmoke" VerticalAlign="Top"></AlternatingItemStyle>
				<ItemStyle BackColor="#e7e7ff" VerticalAlign="Top"></ItemStyle>
				<HeaderStyle CssClass="Header"></HeaderStyle>
				<Columns>
					<asp:TemplateColumn>
						<HeaderStyle Width="50px"></HeaderStyle>
						<ItemStyle HorizontalAlign="Left" />
						<HeaderTemplate>
							<table id="table2" cellspacing="0" cellpadding="0" style="width:100%; border: none;">
								<tr>
									<td align="left">
										<asp:CheckBox language="javascript" id="chkSelDesel" onclick="CheckAll('dgrVaccinazioni',this.checked,0,0);" runat="server" >
                                        </asp:CheckBox></td>
<%--									<td>
										<img alt="" src="..\..\images\spugna.gif" title="Cancella tutto" onclick="CancellaTutto();" />
									</td>--%>
								</tr>
							</table>
						</HeaderTemplate>
						<ItemTemplate>
								<asp:CheckBox id="chkSelezionaItem" runat="server" style="margin-top: 4px;" Checked='<%# OnVacUtility.GetFormattedValue(Container.DataItem("Sel"), "System.Boolean", "", "False") %>' />
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn ItemStyle-Width="30px">
						<ItemTemplate>
							<asp:ImageButton ID="btnReplica" runat="server" ImageUrl="~/Images/duplica.gif" OnClick="btnReplica_Click" 
                                AlternateText="Replica" ToolTip="Replica Data di Esecuzione, Luogo, Tipo erogatore e Centro Vaccinale su tutte le Associazioni selezionate" />
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Associaz.">
						<HeaderStyle HorizontalAlign="Left" Width="10%"></HeaderStyle>
						<ItemTemplate>
							<asp:Label id="lblAssociazioneItem" runat="server" Width="100%" ToolTip='<%# GetAssociazioniTooltip(DataBinder.Eval(Container, "DataItem"))%>' style="margin-top: 6px" Text='<%# Container.DataItem("ves_ass_codice") %>' />
                            <asp:Label id="lblAssociazioneDesc" runat="server" Width="100%" style="margin-top: 6px" Text='<%# " - " + Container.DataItem("ass_descrizione")%>' Visible='<%# IIf(AssociazioneCodiceDescrizione, False, True)%>' />
							<asp:HiddenField id="hdfAssociazioneDose" runat="server" Value='<%# OnVacUtility.GetFormattedValue(Container.DataItem("ves_ass_n_dose"), "System.Int16", "", "1") %>' />
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Dose">
						<HeaderStyle HorizontalAlign="Left" Width="40px"></HeaderStyle>
						<ItemTemplate>
							<asp:TextBox id="txtAssDoseItem" style="text-align: center;padding-top:3px;  width:30px"  
								runat="server" CssClass='<%# IIf(Container.DataItem("ESEGUITA")="False", "TextBox_Stringa_Obbligatorio", "TextBox_Stringa_Disabilitato")%>' 
								ReadOnly='<%# Container.DataItem("ESEGUITA")="True" %>' maxlength="2"
								Text='<%# OnVacUtility.GetFormattedValue(Container.DataItem("ves_ass_n_dose"), "System.Int16", "", "1") %>' >
							</asp:TextBox>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle HorizontalAlign="Left" Width="30%"></HeaderStyle>
						<HeaderTemplate>
							<table cellpadding="0px" cellspacing="0px" style="width: 100%">
								<colgroup>
									<col style="width: 50% "/>
									<col />
									<col />
									<col />
								</colgroup>
								<tr>
									<td>Vaccinazione</td>
                                    <td>Dose</td>
                                    <td><div title="Condizione Sanitaria">Cond.<br />San.</div></td>
                                    <td><div title="Condizione di Rischio">Cond.<br />Risc.</div></td>
                                </tr>
                            </table>
						</HeaderTemplate>
						<ItemStyle Width="30%" />
						<ItemTemplate>
							<asp:DataGrid id="dgrDettaglio" runat="server" width="100%" CssClass="dgr2" AutoGenerateColumns="False" ShowHeader="false" OnItemDataBound="dgrDettaglio_ItemDataBound">
								<AlternatingItemStyle CssClass="r1"></AlternatingItemStyle>
								<ItemStyle CssClass="r2"></ItemStyle>
								<HeaderStyle CssClass="h1"></HeaderStyle>
								<Columns>
									<asp:TemplateColumn HeaderText="Dose">
										<ItemTemplate>		
									<table cellpadding="0px" cellspacing="0px" style="width: 100%">
												<colgroup>
													<col style="width: 50% "/>
													<col />
													<col />
													<col />
												</colgroup>
												<tr>
													<td>
														<asp:HiddenField id="hdfVaccinazioneCodice" runat="server" Value='<%# Container.DataItem("ves_vac_codice") %>' />
														<asp:Label id="lblVacDesc" runat="server" Text='<%# Container.DataItem("vac_descrizione") %>' Width="100%" />
													</td>
													<td>
														<asp:TextBox id="txtDettagliDoseVac" style="text-align: center; width: 100%" runat="server" 
                											CssClass='<%#IIf(Container.DataItem("ESEGUITA") = "False", "TextBox_Stringa_Obbligatorio", "TextBox_Stringa_Disabilitato")%>' 
				                							ReadOnly='<%# Container.DataItem("ESEGUITA") = "True" %>' maxlength="2" Width="30px"
								                			Text='<%# OnVacUtility.GetFormattedValue(Container.DataItem("ves_n_richiamo"), "System.Int16", "", "1") %>' />
													</td>
													<td>
														<on_ofm:OnitModalList ID="omlCondSanitaria" runat="server" Width="0%" LabelWidth="-8px" SetUpperCase="True"
															Enabled='<%# Container.DataItem("ESEGUITA") <> "True"%>'
															PosizionamentoFacile="False" Obbligatorio="False" Font-Size="11px"
															OnSetUpFiletr="omlCondSanitaria_SetUpFilter" OnChange="omlCondSanitaria_Change"
															AltriCampi="VCS_PAZ_MAL_CODICE Paz, VCS_COND_SANITARIA_DEFAULT Def"
															Tabella="V_CONDIZIONI_SANITARIE" CampoCodice="VCS_MAL_CODICE Codice" CampoDescrizione="VCS_MAL_DESCRIZIONE Descrizione"
															RaiseChangeEvent="True" CodiceWidth="99%" UseCode="True" IsDistinct="true" />
													</td>
													<td>
														<on_ofm:OnitModalList ID="omlCondRischio" runat="server" Width="0%" LabelWidth="-8px" SetUpperCase="True"
															Enabled='<%# Container.DataItem("ESEGUITA") <> "True"%>'
															PosizionamentoFacile="False" Obbligatorio="False" Font-Size="11px"
															OnSetUpFiletr="omlCondRischio_SetUpFilter" OnChange="omlCondRischio_Change"
															AltriCampi="VCR_PAZ_RSC_CODICE Paz, VCR_RISCHIO_DEFAULT Def"
															Tabella="V_CONDIZIONI_RISCHIO" CampoCodice="VCR_CODICE_RISCHIO Codice" CampoDescrizione="VCR_DESCRIZIONE_RISCHIO Descrizione"
															RaiseChangeEvent="True" CodiceWidth="99%" UseCode="True" IsDistinct="true" />
													</td>
												</tr>
											</table>		
																	        
										</ItemTemplate>							
									</asp:TemplateColumn>
								</Columns>
							</asp:DataGrid>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="" >
						<HeaderStyle HorizontalAlign="Left" Width="10px"></HeaderStyle>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Data">
						<HeaderStyle HorizontalAlign="Left" Width="90px"></HeaderStyle>
						<HeaderTemplate>
							<table cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td width="100%">Data</td>
								</tr>
							</table>
						</HeaderTemplate>
						<ItemTemplate>
							<on_val:onitdatepick Enabled='<%# Container.DataItem("ESEGUITA") = "False"%>' id="txtDataItem" style="padding-top:3px; "
								runat="server" Width="90px" CssClass='<%# iif(Container.DataItem("ESEGUITA") = "False", "TextBox_Data_Obbligatorio", "TextBox_Data_Disabilitato")%>' 
								Height="22px" Text='<%# Container.DataItem("ves_data_effettuazione") %>' Focus="False" 
								FormatoData="GeneralDate" DateBox="True" ControlloTemporale="False" Hidden="False" CalendarioPopUp="True" 
								Formatta="False" NoCalendario="True" >
							</on_val:onitdatepick>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Luogo">
						<HeaderStyle HorizontalAlign="Left" Width="140px"></HeaderStyle>
						<HeaderTemplate>
							<table cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td width="100%">Luogo</td>
<%--									<td width="20px"><img title="Cancella colonna" onclick="ClearCol(6)" alt="" src="..\..\images\spugna.gif" /></td>--%>
								</tr>
							</table>
						</HeaderTemplate>
						<ItemTemplate>
								<on_ocb:onitcombo id="cmbLuogo" runat="server" Width="100%" style="margin-top: 3px;"
									CssClass='<%# iif(Container.DataItem("ESEGUITA")="False", "TextBox_Stringa_Obbligatorio", "TextBox_Stringa_Disabilitato")%>' 
									BoundText='<%# Container.DataItem("ves_luogo") %>' IncludeNull="True" OnSelectedIndexChanged="cmbLuogo_SelectedIndexChanged"
									Enabled='<%# Container.DataItem("ESEGUITA")="False"%>' AutoPostBack="true">
								</on_ocb:onitcombo>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="TipoErogatore">
						<HeaderStyle HorizontalAlign="Left" Width="200px"></HeaderStyle>
						<HeaderTemplate>
							<table cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td width="100%">Tipo Erogatore</td>
									<%--<td width="20px"><img title="Cancella colonna" onclick="ClearCol(7)" alt="" src="..\..\images\spugna.gif" /></td>--%>
								</tr>
							</table>
						</HeaderTemplate>
						<ItemTemplate>
								<on_ocb:onitcombo id="cmbTipoErogatore" runat="server" Width="100%" style="margin-top: 3px;" 
									DataValueField="Codice" DataTextField="Descrizione"
									IncludeNull="false" OnSelectedIndexChanged="cmbTipoErogatore_SelectedIndexChanged"									
									AutoPostBack="true">
								</on_ocb:onitcombo>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Centro Vaccinale">
						<HeaderStyle HorizontalAlign="Left" Width="15%"></HeaderStyle>
						<HeaderTemplate>
							<table cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td width="100%">Centro Vaccinale</td>
<%--									<td width="20px"><img alt="" src="..\..\images\spugna.gif" onclick="ClearCol(8)" title="Cancella colonna" /></td>--%>
								</tr>
							</table>
						</HeaderTemplate>
						<ItemTemplate>
							<on_ofm:onitmodallist id="txtConsultorioItem" runat="server" Width="68%" style="margin-top: 3px;"								
								CodiceWidth="30%" CampoCodice="CNS_CODICE Codice" 
								CampoDescrizione="CNS_DESCRIZIONE Descrizione" Tabella="T_ANA_CONSULTORI" UseCode="True" 
								SetUpperCase="True" RaiseChangeEvent="False" 
								Codice='<%# Container.DataItem("ves_cns_codice") %>' LabelWidth="-1px" 
								PosizionamentoFacile="False" Obbligatorio="False" >
							</on_ofm:onitmodallist>							
						</ItemTemplate>
					</asp:TemplateColumn>				
					<asp:TemplateColumn>
                        <HeaderStyle HorizontalAlign="Center" Width="3%" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkPagVac" runat="server" OnClick="lnkPagVac_Click" Text="€" Style="font-weight: bold"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <%-- SortExpression è usata per la gestione parametrica del campo  --%>
                    <asp:TemplateColumn HeaderText="Fittizia" SortExpression="ves_flag_fittizia">
						<HeaderStyle HorizontalAlign="Left" Width="20px"></HeaderStyle>
						<ItemTemplate>
							<asp:CheckBox id="chkFittizia" runat="server" style="margin-top: 4px;" Checked='<%# Container.DataItem("ves_flag_fittizia") = "S" %>' Enabled='<%# Container.DataItem("ESEGUITA")="False"%>' ToolTip="Indicatore di vaccinazione fittizia" />
						</ItemTemplate>
					</asp:TemplateColumn>
                    <%-- SortExpression è usata per la gestione parametrica del campo  --%>
                    <asp:TemplateColumn HeaderText="Consenso" SortExpression="ves_flag_visibilita">
						<HeaderStyle HorizontalAlign="Left" Width="20px"></HeaderStyle>
						<ItemTemplate>
							<asp:CheckBox id="chkFlagVisibilita" runat="server" style="margin-top: 4px;" 
                                Checked='<%# Container.DataItem("ves_flag_visibilita").ToString() = Onit.OnAssistnet.OnVac.Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente %>' 
                                ToolTip="Consenso alla comunicazione dei dati vaccinali da parte del paziente" AutoPostBack="false" />
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle HorizontalAlign="Left" Width="20px"></HeaderStyle>
						<ItemTemplate>
							<asp:Image id="imgEseguita" style="margin-top: 4px;" runat="server" ImageUrl="../../images/esegui.gif" Visible='<%# Container.DataItem("ESEGUITA") %>'/>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle HorizontalAlign="Left" Width="20px"></HeaderStyle>
						<ItemTemplate>
							<asp:ImageButton ID="btnDatiVac" OnClick="btnDatiVac_Click" Runat="server"
								ImageUrl='<%# GetDatiVacImageUrl(DataBinder.Eval(Container, "DataItem"), True)%>' 
								ToolTip='<%# GetDatiVacToolTip(DataBinder.Eval(Container, "DataItem"))%>'
								style="margin-top: 4px; cursor: pointer; padding-right:2px;" ></asp:ImageButton>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn DataField="ves_note" Visible="false"></asp:BoundColumn>
                    <asp:BoundColumn DataField="ves_ope_codice" Visible="false"></asp:BoundColumn>
                    <asp:BoundColumn DataField="ves_med_vaccinante" Visible="false"></asp:BoundColumn>
                    <asp:BoundColumn DataField="ves_comune_o_stato" Visible="false"></asp:BoundColumn>
                    <asp:BoundColumn DataField="ves_vii_codice" Visible="false"></asp:BoundColumn>
                    <asp:BoundColumn DataField="ves_sii_codice" Visible="false"></asp:BoundColumn>
                    <asp:BoundColumn DataField="ves_noc_codice" Visible="false"></asp:BoundColumn>
                    <asp:BoundColumn DataField="ves_lot_codice" Visible="false"></asp:BoundColumn>
					<asp:BoundColumn DataField="ves_lot_data_scadenza" Visible="false"></asp:BoundColumn>
                    <asp:BoundColumn DataField="tipo_comune_o_stato" Visible="false"></asp:BoundColumn>
					<asp:BoundColumn DataField="ves_tpa_guid_tipi_pagamento" Visible="false"></asp:BoundColumn>
					<asp:BoundColumn DataField="ves_mal_codice_malattia" Visible="false"></asp:BoundColumn>
					<asp:BoundColumn DataField="ves_codice_esenzione" Visible="false"></asp:BoundColumn>
					<asp:BoundColumn DataField="ves_importo" Visible="false"></asp:BoundColumn>
					<asp:BoundColumn DataField="struttura" Visible="false"></asp:BoundColumn>
					<asp:BoundColumn DataField="ves_usl_cod_somministrazione" Visible="false"></asp:BoundColumn>
				</Columns>
			</asp:DataGrid>
		</dyp:DynamicPanel>
    </on_lay3:OnitLayout3>
    
    <on_ofm:OnitFinestraModale ID="modAggAss" Title="Aggiungi" runat="server" Width="508px" BackColor="LightGray">
        <uc1:AggiungiVaccinazione ID="uscAggAss" runat="server"></uc1:AggiungiVaccinazione>
    </on_ofm:OnitFinestraModale>

    <!-- Modale inserimento/modifica note di esecuzione  -->
    <on_ofm:OnitFinestraModale ID="modDatiVac" Title="Note Vaccinazione" runat="server" Width="600px" BackColor="LightGray" NoRenderX="True" ZIndexPosition="1" IsAnagrafe="False">		

        <asp:HiddenField ID="hidRowIndex" runat="server" />
        <asp:HiddenField ID="hidCodiceAssociazione" runat="server" />
        <asp:HiddenField ID="hidTipoComuneStato" runat="server" />

        <table cellspacing="0" cellpadding="2" width="100%" border="0">
			<colgroup>
                <col style="width: 25%" />
                <col style="width: 25%" />
				<col style="width: 25%" />
                <col style="width: 25%" />
            </colgroup>
			 <tr>
                <td align="center" colspan="4">
                    <%--<div style="border: navy 2px solid; padding: 4px; width: 100%; color: blue; background-color: whitesmoke;
                        font-family: Verdana; font-weight: bold; font-size: 12px;">
                        ATTENZIONE: le note inserite verranno ripetute per tutte le vaccinazioni nella stessa
                        data. Note precedenti saranno sovrascritte.
                    </div>--%>
					<div style="border: navy 2px solid; padding: 4px; width: 95%; color: blue; background-color: whitesmoke;
                        font-family: Verdana; font-weight: bold; font-size: 12px;">
                        ATTENZIONE: i dati precedenti verranno sovrascritti.
                    </div>
                </td>
            </tr>
			<tr>
                <td class="label">
                    <asp:Label ID="lblNoc" runat="server" Text="Nome Commerciale"></asp:Label></td>
                <td colspan="3">
                    <on_ofm:OnitModalList ID="omlNomeCommerciale" runat="server" Width="80%" CodiceWidth="20%" UseCode="True"
                        AltriCampi="noc_for_codice Codice_Fornitore, for_descrizione Fornitore " 
                        CampoCodice="noc_codice Codice" CampoDescrizione="noc_descrizione Descrizione" Tabella="t_ana_nomi_commerciali, t_ana_fornitori, t_ana_link_noc_associazioni" SetUpperCase="True"
                        RaiseChangeEvent="False" LabelWidth="-8px" PosizionamentoFacile="False" Obbligatorio="False" Label="Titolo" LikeMode="All" />
                </td>
            </tr>
			<tr>
                <td class="label">
                    <asp:Label ID="lblLotto" runat="server" Text="Codice Lotto"></asp:Label></td>
                <td>
                    <asp:TextBox ID="txtLotto" runat="server" CssClass="TextBox_Stringa" Width="100%"></asp:TextBox>
                </td>
				<td class="label">
                    <asp:Label ID="lblDataScadenzaLotto" runat="server" Text="Data Scadenza"></asp:Label></td>
                <td>
					<on_val:onitdatepick id="dpkDataScadenzaLotto" style="padding-top:3px; " runat="server" Width="90px" Height="22px"  Focus="False" FormatoData="GeneralDate" DateBox="True"
						CssClass='<%#IIf(dpkDataScadenzaLotto.Enabled, "TextBox_Data_Obbligatorio", "TextBox_Data_Disabilitato")%>' 
						ControlloTemporale="False" Hidden="False" CalendarioPopUp="True"  Formatta="False" NoCalendario="True" >
					</on_val:onitdatepick>
				</td>
            </tr>
			<tr>
                <td class="label">
                    <asp:Label ID="lblVii" runat="server" Text="Via Somministrazione"></asp:Label></td>
                <td colspan="3">
                    <on_ocb:OnitCombo ID="ddlVii" runat="server" IncludeNull="true" CssClass="TextBox_Stringa" Width="100%"></on_ocb:OnitCombo></td>
            </tr>
			<tr>
                <td class="label">
                    <asp:Label ID="lblSii" runat="server" Text="Sito Inoculazione"></asp:Label></td>
                <td colspan="3">
                    <on_ocb:OnitCombo ID="ddlSii" runat="server" IncludeNull="true" CssClass="TextBox_Stringa" Width="100%"></on_ocb:OnitCombo></td>
            </tr>
			<tr>
                <td class="label">
                    <asp:Label ID="lblMedicoResp" runat="server" Text="Medico Responsabile"></asp:Label></td>
                <td colspan="3">
                    <on_ofm:OnitModalList ID="omlMedicoResp" runat="server" Width="80%" CodiceWidth="20%" UseCode="True"
                        Filtro="OPE_QUALIFICA = 'C' AND (OPE_OBSOLETO='N' OR OPE_OBSOLETO IS NULL) order by OPE_NOME"
                        CampoCodice="OPE_CODICE Codice" CampoDescrizione="OPE_NOME Nome" Tabella="T_ANA_OPERATORI" SetUpperCase="True"
                        RaiseChangeEvent="False" LabelWidth="-8px" PosizionamentoFacile="False" Obbligatorio="False" Label="Titolo" LikeMode="All" />
                </td>
            </tr>
            <tr>
                <td class="label">
                    <asp:Label ID="lblVaccinatore" runat="server" Text="Vaccinatore"></asp:Label></td>
                <td colspan="3">
                    <on_ofm:OnitModalList ID="omlVaccinatore" runat="server" Width="80%" CodiceWidth="20%" UseCode="True"
                        Filtro="OPE_QUALIFICA in ('C','D') AND (OPE_OBSOLETO='N' OR OPE_OBSOLETO IS NULL) order by OPE_NOME"
                        CampoCodice="OPE_CODICE Codice" CampoDescrizione="OPE_NOME Nome" Tabella="T_ANA_OPERATORI" SetUpperCase="True"
                        RaiseChangeEvent="False" LabelWidth="-8px" PosizionamentoFacile="False" Obbligatorio="False" Label="Titolo" LikeMode="All" />
                </td>
            </tr>
			<tr>
				<td class="label">
                    <asp:Label ID="lblCodiceStruttura" runat="server" Text="Struttura"></asp:Label></td>                
				<td colspan="3">
                    <on_ofm:OnitModalList ID="omlStrutture" runat="server" Width="80%" CodiceWidth="20%" UseCode="True"
                        Filtro=" 1 = 1 order by AST_DESCRIZIONE" AltriCampi="AST_COMUNE Comune, AST_INDIRIZZO Indirizzo, AST_DATA_APERTURA Apertura, AST_DATA_CHIUSURA Chiusura, AST_ASL Asl, AST_REGIONE Regione, AST_CODICE_COMUNE CodiceComune"
                        CampoCodice="AST_CODICE Codice" CampoDescrizione="AST_DESCRIZIONE Nome" Tabella="V_ANA_STRUTTURE" SetUpperCase="True"  OnChange="omlStrutture_Change"
                        RaiseChangeEvent="true" LabelWidth="-8px" PosizionamentoFacile="False" Obbligatorio="False" Label="Titolo" LikeMode="All" />
                </td>
			</tr>
            <tr>
                <td class="label">
                    <asp:Label ID="lblComuneStato" runat="server" Text="Luogo (Comune o Stato)"></asp:Label></td>
                <td colspan="3">
                    <on_ofm:OnitModalList ID="omlComuneStato" runat="server" Width="80%" CodiceWidth="20%" UseCode="True" 
                        Filtro="1=1 order by COM_DESCRIZIONE" AltriCampi="COM_PROVINCIA Provincia, COM_SCADENZA Scaduto" OnChange="omlComuneStato_Change"
                        CampoCodice="COM_CODICE Codice" CampoDescrizione="COM_DESCRIZIONE Nome" Tabella="T_ANA_COMUNI" SetUpperCase="true"
                        RaiseChangeEvent="true" LabelWidth="-8px" PosizionamentoFacile="False" Obbligatorio="False" Label="Titolo" LikeMode="All" />
                </td>
            </tr>
			<tr>
				<td class="label">
                    <asp:Label ID="lblAsl" runat="server" Text="ASL"></asp:Label></td>
                <td colspan="3">
                    <on_ofm:OnitModalList ID="omlUsl" runat="server" Width="80%" CodiceWidth="20%" UseCode="True"
                        Filtro="1=1 AND LCU_USL_CODICE = USL_CODICE order by USL_DESCRIZIONE" AltriCampi="USL_COM_CODICE ComuneCodice, TO_CHAR(USL_DATA_INIZIO_VALIDITA,'dd/MM/yyyy') Inizio, TO_CHAR(USL_SCADENZA,'dd/MM/yyyy') Fine"
                        CampoCodice="USL_CODICE Codice" CampoDescrizione="USL_DESCRIZIONE Nome" Tabella="T_ANA_USL, T_ANA_LINK_COMUNI_USL" SetUpperCase="True" IsDistinct="true"
                        RaiseChangeEvent="False" LabelWidth="-8px" PosizionamentoFacile="False" Obbligatorio="False" Label="Titolo" LikeMode="All" />
                </td>
			</tr>
            <tr height="100">
				<td class="label">
                    <asp:Label ID="lblNote" runat="server" Text="Note"></asp:Label>
				</td>
                <td colspan="3">					
                    <asp:TextBox ID="txtNoteVacRowIndex" style="display: none" Text="" runat="server"></asp:TextBox>
                    <asp:TextBox ID="txtNoteVac" style="overflow-y: auto; font-family: Verdana;" Width="100%"
                        Height="100%" runat="server" MaxLength="200" TextMode="MultiLine" Rows="5"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="center" width="40%" colspan="2">
                    <asp:Button ID="btnDatiVacOk" Width="75px" runat="server" Text="Ok" Style="cursor: hand">
                    </asp:Button>
                </td>

                <td align="center" width="40%" colspan="2">
                    <asp:Button ID="btnDatiVacAnnulla" Width="75px" runat="server" Text="Annulla" Style="cursor: hand">
                    </asp:Button>
                </td>
            </tr>           
        </table>
    </on_ofm:OnitFinestraModale>

	<on_ofm:OnitFinestraModale ID="modPagVac" Title="Pagamento Vaccinazione" runat="server" Width="600px" BackColor="LightGray" NoRenderX="True" ZIndexPosition="1" IsAnagrafe="False">
		<asp:HiddenField ID="hdRowIndexPagVac" runat="server" />
		<uc1:PagamentoVaccinazione runat="server" id="ucPagamentoVaccinazione" />
		<div>
			<asp:Button ID="btnPagVacOk" Width="75px" runat="server" Text="Ok" Style="cursor: auto;" OnClick="btnPagVacOk_Click"></asp:Button>
			<asp:Button ID="btnPagVacAnnulla" Width="75px" runat="server" Text="Annulla" Style="cursor: auto;" OnClick="btnPagVacAnnulla_Click"></asp:Button>
		</div>
			
	</on_ofm:OnitFinestraModale>


    <!-- #include file="../../Common/Scripts/AggiornaStatoVaccinazioni.inc" -->
    <!-- #include file="../../Common/Scripts/ControlloDatiValidi.inc" -->

    <script type="text/javascript">
		
		<% response.write(strJS) %>
		
		paz_deceduto = "<%= PazDeceduto %>";
		consultorioCodiceDefault = "<%= OnVacUtility.Variabili.CNS.Codice.Trim() %>";
        consultorioDescrizioneDefault = "<%= OnVacUtility.Variabili.CNS.Descrizione.Trim() %>";
        luogoConsultorio = "<%= Onit.OnAssistnet.OnVac.Constants.CodiceLuogoVaccinazione.CentroVaccinale %>";

		dgr=document.getElementById("dgrVaccinazioni");	
		if (dgr != null)
        {
            for (i=1;i<dgr.rows.length;i++)
			{
				obj=dgr.rows[i].getElementsByTagName('SELECT')[0];
				if (obj != null  && !obj.disabled)
				{
						//AbilitaConsultorio(obj,true);
						//AbilitaConsultorio(obj,true);
						AbilitaLuogo(obj,true);
				}
			}
		}		
    </script>

    </form>
</body>
</html>
