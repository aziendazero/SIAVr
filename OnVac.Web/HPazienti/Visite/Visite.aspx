<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Visite.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_Visite"%>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<%@ Import namespace="Onit.OnAssistnet.OnVac" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>Visite</title>

        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
		
        <style type="text/css">
            .w50PX {
                width: 50px;
            }
		</style>
		
        <script type="text/javascript" language="javascript">

            //Controllo che data1 sia minore di data2
            function dateCompatibili(id1, id2, mode) {
                data1 = OnitDataPickGet(id1);
                if (mode == '0') data2 = OnitDataPickGet(id2);
                if (mode == '1') data2 = id2;
                splitData1 = data1.split('/');
                splitData2 = data2.split('/');

                if (!(parseInt(splitData1[2]) > parseInt(splitData2[2]))) {
                    if (parseInt(splitData1[2]) < parseInt(splitData2[2]))
                        return true;
                    else {
                        if (!((splitData1[1]) > (splitData2[1]))) {
                            if ((splitData1[1]) < (splitData2[1])) {
                                return true;
                            }
                            else {
                                if ((splitData1[0]) <= (splitData2[0])) {
                                    return true;
                                }
                                else {
                                    return false;
                                }
                            }
                        }
                        else {
                            return false;
                        }
                    }
                }
                else {
                    return false;
                }
            }
	
            function InizializzaToolBar(t) {
                t.PostBackButton = false;
            }
		
            function ToolBarClick(ToolBar, button, evnt) {
                evnt.needPostBack = true;

                switch (button.Key) {

                    case 'btnElimina':

                        if ('<%= Me.odpVisiteMaster.getCurrentDataTable().Rows.Count = 0 %>' == 'True' && '<%= Me.odpVisiteDetail.getCurrentDataTable().Rows.Count = 0 %>' == 'True') {
                            evnt.needPostBack = false;
                            return;
                        }

                        if (!confirm('<%= GetMessaggioConfermaEliminazioneVisita()%>')) {
                            evnt.needPostBack = false;
                            return;
                        }
                        break;

                    case 'btnAnnulla':

                        if ('<%= Me.odpVisiteMaster.getCurrentDataTable().Rows.Count = 0 %>' == 'True' && '<%= Me.odpVisiteDetail.getCurrentDataTable().Rows.Count = 0 %>' == 'True') {
                            evnt.needPostBack = false;
                            return;
                        }
                        break;

                    case 'btnSalva':
                        
                        if ('<%= Me.odpVisiteMaster.getCurrentDataTable().Rows.Count = 0 %>' == 'True' && '<%= Me.odpVisiteDetail.getCurrentDataTable().Rows.Count = 0 %>' == 'True') {
                            evnt.needPostBack = false;
                            return;
                        }

                        // Controllo di avere sempre DataSospensione >= DataVisita prima di poter salvare i dati
                        if ('<%= txtDataVisita.Enabled %>' == 'True') {

                            if (OnitDataPickGet('txtDataVisita') == "") {
                                alert("Impossibile salvare senza aver immesso la data di visita!");
                                evnt.needPostBack = false;
                            }

                            if ('<%= Me.SospObbligatoria.ToLower() %>' == 'true') {

                                if (OnitDataPickGet('txtFineSospensione') == "") {
                                    alert("Impossibile salvare senza aver immesso la data di fine sospensione!");
                                    evnt.needPostBack = false;
                                }

                                if (!isValidFinestraModale('fmMotivoSospensione', true)) {
                                    alert("Impossibile salvare senza aver immesso il motivo di sospensione!");
                                    evnt.needPostBack = false;
                                }
                            }

                            dataCorrente = new Date();

                            giorno = dataCorrente.getDate().toString();
                            if (giorno.length == 1) giorno = "0" + giorno;

                            mese = (dataCorrente.getMonth() + 1).toString();
                            if (mese.length == 1) mese = "0" + mese;

                            anno = dataCorrente.getFullYear();

                            data = giorno + "/" + mese + "/" + anno;

                            if (!dateCompatibili('txtDataVisita', data, '1')) {
                                alert("La \'Data Visita\' deve essere minore o uguale a quella corrente");
                                evnt.needPostBack = false;
                            }

                            if (OnitDataPickGet('txtFineSospensione') != "") {
                                if (!dateCompatibili('txtDataVisita', 'txtFineSospensione', '0')) {
                                    alert("La data di \'Fine sospensione\' deve essere maggiore o uguale alla \'Data Visita\'");
                                    evnt.needPostBack = false;
                                }
                            }
                        }
                        break;
                }
            }
		</script>
	</head>
	<body>
		<form id="Form1" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" Titolo="Visite" TitleCssClass="Title3">
				<ondp:onitdatapanel id="odpVisiteMaster" runat="server" useToolbar="False" FieldBindingMode="BindControlAsColumn"
					Height="100%" ConfigFile="Visite.odpVisiteMaster.xml" renderOnlyChildren="True" Width="100%" >

					<div class="title" id="divTitolo">
						<asp:Label id="LayoutTitolo" runat="server" Width="100%">Label</asp:Label>
                    </div>

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
								<igtbar:TBarButton Key="btnElimina" Text="Elimina" DisabledImage="~/Images/AnnullaConf_dis.gif" 
                                    Image="~/Images/AnnullaConf.gif" ToolTip="Elimina la visita selezionata"></igtbar:TBarButton>
								<igtbar:TBSeparator></igtbar:TBSeparator>
								<igtbar:TBarButton Key="btnSalva" Text="Salva"></igtbar:TBarButton>
								<igtbar:TBarButton Key="btnAnnulla" Text="Annulla"></igtbar:TBarButton>
								<igtbar:TBSeparator></igtbar:TBSeparator>
							    <igtbar:TBarButton Key="btnRecuperaStoricoVacc" Text="Recupera" DisabledImage="../../images/recupera_dis.png"
								    Image="../../images/recupera.png" ToolTip="Recupera lo storico vaccinale centralizzato del paziente">
                                    <DefaultStyle Width="90px" CssClass="infratoolbar_button_default"></DefaultStyle>
							    </igtbar:TBarButton>
							</Items>
						</igtbar:UltraWebToolbar>
                    </div>
                    
                    <div class="Sezione">Ricerca visite</div>
                    <div>
						<ondp:wzFilter id="filFiltro" runat="server" Height="70px" Width="100%" CssClass="InfraUltraWebTab2"
							TargetUrl="" Dummy>
							<SelectedTabStyle CssClass="InfraTab_Selected2"></SelectedTabStyle>
							<DisabledTabStyle CssClass="InfraTab_Disabled2"></DisabledTabStyle>
							<HoverTabStyle CssClass="InfraTab_Hover2"></HoverTabStyle>
							<DefaultTabStyle CssClass="InfraTab_Default2"></DefaultTabStyle>
							<Tabs>
								<igtab:Tab Text="Ricerca di Base">
									<ContentTemplate>
										<table height="100%" cellspacing="10" cellpadding="0" width="100%" border="0">
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
                    
                    <div class="Sezione">Elenco visite</div>

                    <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
						
                        <ondp:wzMsDataGrid id="dgrVisiteMaster" runat="server" Width="100%" PagerVoicesBefore="-1" PagerVoicesAfter="-1" EditMode="None" OnitStyle="False" 
                            AutoGenerateColumns="False" SelectionOption="rowClick">
							<AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
							<FooterStyle CssClass="footer"></FooterStyle>
							<ItemStyle CssClass="item"></ItemStyle>
							<HeaderStyle CssClass="header"></HeaderStyle>
							<SelectedItemStyle CssClass="selected"></SelectedItemStyle>
							<PagerStyle CssClass="pager"></PagerStyle>
							<EditItemStyle CssClass="edit"></EditItemStyle>
							<Columns>
								<ondp:wzBoundColumn HeaderText="Data visita" Key="" DataFormatString="{0:dd/MM/yyyy}" SourceField="VIS_DATA_VISITA" SourceTable="T_VIS_VISITE" SourceConn="Visite" />
								<ondp:wzBoundColumn HeaderText="Bil." Key="" SourceField="VIS_N_BILANCIO" SourceTable="T_VIS_VISITE" SourceConn="Visite" />
								<ondp:wzBoundColumn HeaderText="Malattia" Key="" SourceField="MAL_DESCRIZIONE" SourceTable="T_ANA_MALATTIE" SourceConn="Visite" />
								<ondp:wzBoundColumn HeaderText="Medico" Key="" SourceField="OPE_NOME" SourceTable="T_ANA_OPERATORI" SourceConn="Visite" />
								<ondp:wzBoundColumn HeaderText="Vacc." Key="" SourceField="COD_DESCRIZIONE" SourceTable="T_ANA_CODIFICHE" SourceConn="Visite" />
								<ondp:wzBoundColumn HeaderText="Motivo sosp." Key="" SourceField="MOS_DESCRIZIONE" SourceTable="T_ANA_MOTIVI_SOSPENSIONE" SourceConn="Visite" />
								<ondp:wzBoundColumn HeaderText="Fine sosp." Key="" DataFormatString="{0:dd/MM/yyyy}" SourceField="VIS_FINE_SOSPENSIONE" SourceTable="T_VIS_VISITE" SourceConn="Visite" />
								<ondp:wzBoundColumn HeaderText="Note" Key="" SourceField="VIS_NOTE" SourceTable="T_VIS_VISITE" SourceConn="Visite" />
                                <ondp:wzBoundColumn HeaderText="<%$ Resources: Onit.OnAssistnet.OnVac.Web, DatiVaccinali.AslInserimentoVacc %>" Key="UslInserimentoVisita" SourceField="USL_DESCRIZIONE" SourceTable="T_ANA_USL" SourceConn="Visite" />
								<ondp:wzBoundColumn HeaderText="Utente" Key="" SourceField="UTE_DESCRIZIONE" SourceTable="T_ANA_UTENTI" SourceConn="Visite" />
										
                                <ondp:wzTemplateColumn Key="VIS_FLAG_VISIBILITA" HeaderText="" SourceField="VIS_FLAG_VISIBILITA" SourceTable="T_VIS_VISITE" SourceConn="Visite">
                                    <HeaderStyle HorizontalAlign="Center" />
									<ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Image ID="imgFlagVisibilita" runat="server" 
                                                    ImageUrl="<%# BindFlagVisibilitaImageUrlValue(Container.DataItem) %>" 
                                                    ToolTip="<%# BindFlagVisibilitaToolTipValue(Container.DataItem) %>" />
                                        <asp:HiddenField ID="HiddenField_FLAGVISIBILITA" runat="server" Value='<%# Container.DataItem("VIS_FLAG_VISIBILITA") %>'></asp:HiddenField>
                                    </ItemTemplate>
                                </ondp:wzTemplateColumn>
                                        
								<ondp:wzTemplateColumn Key="FLAG_FIRMA" HeaderText="" SourceField="" SourceTable="" SourceConn="">
                                    <HeaderStyle HorizontalAlign="Center" />
									<ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Image ID="imgFlagFirma" runat="server" 
                                            ImageUrl="<%# BindFlagFirmaImageUrlValue(Container.DataItem)%>" 
                                            ToolTip="<%# BindFlagFirmaToolTipValue(Container.DataItem)%>" />
                                    </ItemTemplate>
                                </ondp:wzTemplateColumn>

                                <ondp:wzBoundColumn HeaderText="CodMalattia" Key="" Hidden="True" SourceField="VIS_MAL_CODICE" SourceTable="T_VIS_VISITE" SourceConn="Visite" />
								<ondp:wzBoundColumn HeaderText="IdVisita" Key="VIS_ID" Hidden="True" SourceField="VIS_ID" SourceTable="T_VIS_VISITE" SourceConn="Visite" />
								<ondp:wzBoundColumn HeaderText="UslInserimento" Hidden="True" Key="VIS_USL_INSERIMENTO" SourceField="VIS_USL_INSERIMENTO" SourceTable="T_VIS_VISITE" SourceConn="Visite" />
								<ondp:wzBoundColumn HeaderText="DataRegistrazione" Key="VIS_DATA_REGISTRAZIONE"  Hidden="True" SourceField="VIS_DATA_REGISTRAZIONE" SourceTable="T_VIS_VISITE" SourceConn="Visite" />
                                <ondp:wzBoundColumn HeaderText="UteIdFirma" Key="VIS_UTE_ID_FIRMA"  Hidden="True" SourceField="VIS_UTE_ID_FIRMA" SourceTable="T_VIS_VISITE" SourceConn="Visite" />
                                <ondp:wzBoundColumn HeaderText="UteIdArchiviazione" Key="VIS_UTE_ID_ARCHIVIAZIONE"  Hidden="True" SourceField="VIS_UTE_ID_ARCHIVIAZIONE" SourceTable="T_VIS_VISITE" SourceConn="Visite" />
							</Columns>
							<BindingColumns>
								<ondp:BindingFieldValue Value="" Editable="always" Description="Data visita" Connection="Visite" SourceTable="T_VIS_VISITE"
									Hidden="False" SourceField="VIS_DATA_VISITA"></ondp:BindingFieldValue>
								<ondp:BindingFieldValue Value="" Editable="always" Description="" Connection="Visite" SourceTable="T_VIS_VISITE"
									Hidden="False" SourceField="VIS_N_BILANCIO"></ondp:BindingFieldValue>
								<ondp:BindingFieldValue Value="" Editable="always" Description="" Connection="Visite" SourceTable="T_ANA_MALATTIE"
									Hidden="False" SourceField="MAL_DESCRIZIONE"></ondp:BindingFieldValue>
								<ondp:BindingFieldValue Value="" Editable="always" Description="Medico" Connection="Visite" SourceTable="T_ANA_OPERATORI"
									Hidden="False" SourceField="OPE_NOME"></ondp:BindingFieldValue>
								<ondp:BindingFieldValue Value="" Editable="always" Description="Vacc." Connection="Visite" SourceTable="T_ANA_CODIFICHE"
									Hidden="False" SourceField="COD_DESCRIZIONE"></ondp:BindingFieldValue>
								<ondp:BindingFieldValue Value="" Editable="always" Description="Motivo sosp." Connection="Visite" SourceTable="T_ANA_MOTIVI_SOSPENSIONE"
									Hidden="False" SourceField="MOS_DESCRIZIONE"></ondp:BindingFieldValue>
								<ondp:BindingFieldValue Value="" Editable="always" Description="Fine sosp." Connection="Visite" SourceTable="T_VIS_VISITE"
									Hidden="False" SourceField="VIS_FINE_SOSPENSIONE"></ondp:BindingFieldValue>
								<ondp:BindingFieldValue Value="" Editable="always" Description="Note" Connection="Visite" SourceTable="T_VIS_VISITE"
									Hidden="False" SourceField="VIS_NOTE"></ondp:BindingFieldValue>
                                <ondp:BindingFieldValue Value="" Editable="always" Description="<%$ Resources: Onit.OnAssistnet.OnVac.Web, DatiVaccinali.AslInserimentoVacc %>" 
                                    Connection="Visite" SourceTable="T_ANA_USL" Hidden="False" SourceField="USL_DESCRIZIONE"></ondp:BindingFieldValue>
                                <ondp:BindingFieldValue Value="" Editable="always" Description="Utente" Connection="Visite" SourceTable="T_ANA_UTENTI"
									Hidden="False" SourceField="UTE_DESCRIZIONE"></ondp:BindingFieldValue>
                                <ondp:BindingFieldValue Value="" Editable="always" Description="Com" Connection="Visite" SourceTable="T_VIS_VISITE"
									Hidden="False" SourceField="VIS_FLAG_VISIBILITA"></ondp:BindingFieldValue>
								<ondp:BindingFieldValue Value="" Editable="never" Description="CodMalattia" Connection="Visite" SourceTable="T_VIS_VISITE"
									Hidden="True" SourceField="VIS_MAL_CODICE"></ondp:BindingFieldValue>
								<ondp:BindingFieldValue Value="" Editable="never" Description="VisId" Connection="Visite" SourceTable="T_VIS_VISITE"
									Hidden="True" SourceField="VIS_ID"></ondp:BindingFieldValue>
                                <ondp:BindingFieldValue Value="" Editable="never" Description="UslInserimento" Connection="Visite" SourceTable="T_VIS_VISITE"
									Hidden="True" SourceField="VIS_USL_INSERIMENTO"></ondp:BindingFieldValue>
                                <ondp:BindingFieldValue Value="" Editable="never" Description="DataRegistrazione" Connection="Visite" SourceTable="T_VIS_VISITE"
									Hidden="True" SourceField="VIS_DATA_REGISTRAZIONE"></ondp:BindingFieldValue>
                                <ondp:BindingFieldValue Value="" Editable="never" Description="UteIdFirma" Connection="Visite" SourceTable="T_VIS_VISITE"
									Hidden="True" SourceField="VIS_UTE_ID_FIRMA"></ondp:BindingFieldValue>
                                <ondp:BindingFieldValue Value="" Editable="never" Description="UteIdArchiviazione" Connection="Visite" SourceTable="T_VIS_VISITE"
									Hidden="True" SourceField="VIS_UTE_ID_ARCHIVIAZIONE"></ondp:BindingFieldValue>
							</BindingColumns>
						</ondp:wzMsDataGrid>                    

                    </dyp:DynamicPanel>						

                    <dyp:DynamicPanel ID="dypDettaglio" runat="server" Width="100%" Height="170px" ScrollBars="Auto" RememberScrollPosition="true">
						<ondp:OnitDataPanel id="odpVisiteDetail" runat="server" useToolbar="False" ConfigFile="Visite.odpVisiteDetail.xml"
						    renderOnlyChildren="True" externalToolBar="ToolBar" externalToolBar-Length="7" BindingFields="(Insieme)">
									
                            <div class="Sezione">Dettaglio visita</div>
							
                            <div class="label" style="text-align:left; padding:5px;"><asp:Label ID="Lbl_StatoDetail" runat="server" Text="Non disponibile" Visible="false"></asp:Label></div>

                            <table id="Table_Dettaglio" runat="server" cellspacing="3" cellpadding="0" width="100%" border="0">
                                <colgroup>
                                    <col width="8%" />
                                    <col width="12%" />
                                    <col width="24%" />
                                    <col width="2%" />
                                    <col width="15%" />
                                    <col width="23%" />
                                    <col width="5%" />
                                    <col width="10%" />
                                    <col width="1%" />
                                </colgroup>
								<tr>
									<td class="label">Data Visita</td>
									<td>
										<ondp:wzOnitDatePick id="txtDataVisita" runat="server" Width="120px"
											BindingField-Editable="always" BindingField-Connection="VisiteDetail" BindingField-SourceTable="T_VIS_VISITE"
											BindingField-Hidden="False" BindingField-SourceField="VIS_DATA_VISITA" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato"
											CssStyles-CssEnabled="TextBox_Data" CssStyles-CssRequired="TextBox_Data_Obbligatorio"></ondp:wzOnitDatePick></td>
                                    <td align="right">
                                        <asp:Label runat="server" id="lblFlagVisibilita" CssClass="label"
                                            Text="<%$ Resources: Onit.OnAssistnet.OnVac.Web, DatiVaccinali.ConsensoComunicazione %>" ></asp:Label></td>
                                    <td>
                                        <ondp:wzCheckBox id="chkFlagVisibilita" runat="server" ToolTip="Consenso alla comunicazione dei dati vaccinali da parte del paziente"
                                            CssStyles-CssEnabled="TextBox_Stringa" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato"
											BindingField-Editable="always" BindingField-Connection="VisiteDetail" BindingField-SourceTable="T_VIS_VISITE"
											BindingField-Hidden="False" BindingField-SourceField="VIS_FLAG_VISIBILITA" BindingField-Value="" ></ondp:wzCheckBox></td>
									<td class="label">Motivo sospensione</td>
									<td colspan="3">
										<ondp:wzFinestraModale id="fmMotivoSospensione" runat="server" Width="70%" Tabella="T_ANA_MOTIVI_SOSPENSIONE"
											CampoCodice="MOS_CODICE" CampoDescrizione="MOS_DESCRIZIONE" BindingDescription-SourceField="MOS_DESCRIZIONE"
											BindingDescription-SourceTable="T_ANA_MOTIVI_SOSPENSIONE" BindingDescription-Connection="VisiteDetail"
											BindingDescription-Hidden="False" BindingDescription-Editable="always" Obbligatorio="False" SetUpperCase="True"
											RaiseChangeEvent="False" UseCode="True" BindingCode-SourceField="VIS_MOS_CODICE" BindingCode-Hidden="False"
											BindingCode-SourceTable="T_VIS_VISITE" BindingCode-Connection="VisiteDetail" BindingCode-Editable="always"
											CodiceWidth="29%" LabelWidth="-8px" PosizionamentoFacile="False" Filtro="1=1 order by MOS_CODICE"></ondp:wzFinestraModale></td>
									<td></td>
								</tr>
								<tr>
									<td class="label">Medico</td>
									<td colspan="3">
										<ondp:wzFinestraModale id="fmMedico" runat="server" Width="70%" Tabella="T_ANA_OPERATORI,T_ANA_LINK_OPER_CONSULTORI" CampoCodice="OPE_CODICE Codice"
											CampoDescrizione="OPE_NOME Medico" BindingDescription-SourceField="OPE_NOME" BindingDescription-SourceTable="T_ANA_OPERATORI"
											BindingDescription-Connection="VisiteDetail" BindingDescription-Hidden="False" BindingDescription-Editable="always"
											Obbligatorio="False" SetUpperCase="True" RaiseChangeEvent="False" UseCode="True" BindingCode-SourceField="VIS_OPE_CODICE"
											BindingCode-Hidden="False" BindingCode-SourceTable="T_VIS_VISITE" BindingCode-Connection="VisiteDetail"
											BindingCode-Editable="always" CodiceWidth="29%" LabelWidth="-8px" PosizionamentoFacile="False" ></ondp:wzFinestraModale></td>
									<td class="label">Fine sospensione</td>
									<td>
										<ondp:wzOnitDatePick id="txtFineSospensione" runat="server" Width="120px"
											BindingField-Editable="always" BindingField-Connection="VisiteDetail"
											BindingField-SourceTable="T_VIS_VISITE" BindingField-Hidden="False" BindingField-SourceField="VIS_FINE_SOSPENSIONE"
											CssStyles-CssDisabled="TextBox_Stringa_Disabilitato" CssStyles-CssEnabled="TextBox_Data" CssStyles-CssRequired="TextBox_Data_Obbligatorio"></ondp:wzOnitDatePick></td>											
									<td class="label">Vaccinabile</td>
									<td>
										<ondp:wzDropDownList id="ddlVaccinabile" runat="server" Width="100%"
											BindingField-Editable="always" BindingField-Connection="VisiteDetail" BindingField-SourceTable="T_VIS_VISITE"
											BindingField-Hidden="False" BindingField-SourceField="VIS_VACCINABILE" CssStyles-CssDisabled="textbox_stringa_disabilitato w50PX"
											CssStyles-CssEnabled="textbox_stringa w50PX" CssStyles-CssRequired="textbox_stringa_obbligatorio w50PX"
											IncludeNull="True" TextFieldName="COD_DESCRIZIONE" KeyFieldName="COD_CODICE" OtherListFields="cod_campo"
											DataFilter="cod_campo='VIS_VACCINABILE'" SourceTable="T_ANA_CODIFICHE"></ondp:wzDropDownList></td>
									<td></td>
								</tr>
								<tr>
									<td class="label">Operatore</td>
									<td colspan="3">
										<ondp:wzFinestraModale id="fmRilevatore" runat="server" Width="70%" Tabella="T_ANA_OPERATORI,T_ANA_LINK_OPER_CONSULTORI" CampoCodice="OPE_CODICE Codice"
											CampoDescrizione="OPE_NOME Rilevatore" BindingDescription-SourceField="OPE_NOME" BindingDescription-SourceTable="T_ANA_OPERATORI_RILEV"
											BindingDescription-Connection="VisiteDetail" BindingDescription-Hidden="False" BindingDescription-Editable="always"
											Obbligatorio="False" SetUpperCase="True" RaiseChangeEvent="False" UseCode="True" BindingCode-SourceField="VIS_OPE_CODICE_RILEVATORE"
											BindingCode-Hidden="False" BindingCode-SourceTable="T_VIS_VISITE" BindingCode-Connection="VisiteDetail"
											BindingCode-Editable="always" CodiceWidth="29%" LabelWidth="-8px" PosizionamentoFacile="False"></ondp:wzFinestraModale></td>
                                    <td class="label">Malattia Diagnosticata</td>
									<td colspan="3">
										<ondp:wzFinestraModale id="fmMalattia" runat="server" Width="70%" Tabella="T_ANA_MALATTIE" CampoCodice="MAL_CODICE Codice"
											CampoDescrizione="MAL_DESCRIZIONE Descrizione" BindingDescription-SourceField="MAL_DESCRIZIONE" BindingDescription-SourceTable="T_ANA_MALATTIE"
											BindingDescription-Connection="VisiteDetail" BindingDescription-Hidden="False" BindingDescription-Editable="always"
											Obbligatorio="False" SetUpperCase="True" RaiseChangeEvent="False" UseCode="True" BindingCode-SourceField="VIS_MAL_CODICE"
											BindingCode-Hidden="False" BindingCode-SourceTable="T_VIS_VISITE" BindingCode-Connection="VisiteDetail"
											BindingCode-Editable="always" CodiceWidth="29%" LabelWidth="-8px" PosizionamentoFacile="False" Filtro="1=1 AND MAL_OBSOLETO='N' order by MAL_CODICE"></ondp:wzFinestraModale></td>
                                    <td></td>
								</tr>
								<tr>
									<td class="label" style="vertical-align:top">Note</td>
									<td colspan="7">
										<ondp:wzTextBox id="txtNote" runat="server" MaxLength="4000"
											BindingField-Editable="always" BindingField-Connection="VisiteDetail" BindingField-SourceTable="T_VIS_VISITE"
											BindingField-Hidden="False" BindingField-SourceField="VIS_NOTE" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
											CssStyles-CssEnabled="TextBox_Stringa w100p" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
											Rows="3" TextMode="MultiLine"></ondp:wzTextBox></td>
									<td></td>
								</tr>
							</table>

						</ondp:OnitDataPanel>
							
                    </dyp:DynamicPanel>
				</ondp:onitdatapanel>

				<on_ofm:onitfinestramodale id="fmUnisciCnv" title="Spostamento convocazione" runat="server"  width="640px" BackColor="LightGray">
					<p class="label_left">
                        <table id="Table2" cellspacing="1" cellpadding="1" width="100%" border="0">
							<tr>
								<td style="width: 47px" align="center">
                                    <img alt="" runat="server" src="~/Images/alert.gif" align="absMiddle" />
                                </td>
								<td class="label_left">Alcune convocazioni saranno spostate perchè la loro data di 
									inizio è inferiore a quella di fine sospensione. E' possibile che alcune 
									convocazioni siano unite se in data di fine sospensione si sovrappongono. 
                                    Se le convocazioni unite hanno centri vaccinali diversi, verrà utilizzato il centro 
                                    relativo alla prima convocazione (o quello della convocazione nella data di unione, se già presente).
									<br />Continuare?</td>
							</tr>
						</table>
                    </p>
					<p align="center">
						<asp:Button id="btnSpostaOK" runat="server" Width="93px" Text="OK"></asp:Button>&nbsp;
						<asp:Button id="btnSpostaAnnulla" runat="server" Width="93px" Text="Annulla"></asp:Button>
                    </p>
				</on_ofm:onitfinestramodale>

			</on_lay3:onitlayout3>
        </form>

		<!-- #include file="../../Common/Scripts/AggiornaStatoVaccinazioni.inc" -->
		<!-- #include file="../../Common/Scripts/ControlloDatiValidi.inc" --> 

	</body>
</html>
