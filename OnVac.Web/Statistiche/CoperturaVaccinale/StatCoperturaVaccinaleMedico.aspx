<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="StatCoperturaVaccinaleMedico.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.StatCoperturaVaccinaleMedico" %>

<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="uc1" TagName="SelezioneConsultori" Src="../../Common/Controls/SelezioneConsultori.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head >
    <title>Copertura Vaccinale Medico</title>
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
    <script type="text/javascript">

        $(document).ready(function () {
            $('#tblVaccinazioni :checked').each(function () {
                chkVaccinazione_CheckedChanged($(this));
            });
        });

        function chkVaccinazione_CheckedChanged(sender) {
            
            // Get the current row
            var row = $(sender).closest('tr');

            // Textbox numero dosi e numero giorni
            var txtDosi = row.find('input[name*="txtNumeroDosi"]');
            var txtGiorni = row.find('input[name*="txtGiorniVita"]');

            // Impostazione cssclass
            txtDosi.toggleClass('textbox_stringa_disabilitato textbox_stringa_obbligatorio');
            txtGiorni.toggleClass('textbox_stringa_disabilitato textbox_stringa_obbligatorio');

            if (!$(sender).is(':checked')) {

                // Se checkbox non selezionato => disabilitazione e pulizia textbox
                txtDosi.val('');
                txtGiorni.val('');

                txtDosi.prop('disabled', true);
                txtGiorni.prop('disabled', true);

            } else {

                // Se checkbox selezionato => abilitazione e valorizzazione textbox (se non già valorizzati)
                if (txtGiorni.val() == '') txtGiorni.val(120 * 365);

                txtDosi.prop('disabled', false);
                txtGiorni.prop('disabled', false);

                txtDosi.focus();
            }
        }

        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;

            // Controlli (comuni a tutte le stampe)
            control = true;
            campiValorizzati = true;

            if (OnitDataPickGet('odpDataNascitaIniz') == "" || OnitDataPickGet('odpDataNascitaFin') == "" || (document.getElementById('txtNumeroDosi').value == "") || (document.getElementById('txtGiorniVita').value == "")) {
                campiValorizzati = false;
                control = false;
            }

            if (!control) {
                if (!campiValorizzati)
                    alert("Non tutti i campi obbligatori sono impostati. Impossibile stampare il report.");

                evnt.needPostBack = false;
            }

            if ((!OnitDataPickGet('odpDataEffettuazioneIniz') == "" && OnitDataPickGet('odpDataEffettuazioneFin') == "") || (OnitDataPickGet('odpDataEffettuazioneIniz') == "" && !OnitDataPickGet('odpDataEffettuazioneFin') == "")) {
                alert("Valorizzare sia la data di inizio sia di fine effettuazione per impostare il filtro. Impossibile stampare il report.");
                evnt.needPostBack = false;
            }

        }
    </script>
    <style type="text/css">
        .margin-bottom-5 {
            margin-bottom: 5px;
        }
    </style>
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <asp:MultiView ID="MultiView" runat="server" ActiveViewIndex="0">
            <asp:View ID="ViewFiltri" runat="server">
                <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Width="100%" Height="100%" Titolo="Statistiche - <b>Copertura vaccinale Medico</b>" TitleCssClass="Title3">

					<div class="title" id="PanelTitolo" runat="server">
						<asp:Label id="LayoutTitolo" runat="server"></asp:Label>
                    </div>
                    <div>
						<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="Toolbar" runat="server" ItemWidthDefault="130px" CssClass="infratoolbar">
                            <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                            <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
							<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
							<Items>
								<igtbar:TBarButton Key="btnStampa" Text="Copertura Vaccinazione" DisabledImage="~/Images/stampa.gif"
									Image="~/Images/Stampa.gif" ToolTip="Copertura vaccinale per vaccinazione" >
									<DefaultStyle Width="180px" CssClass="infratoolbar_button_default"></DefaultStyle>
								</igtbar:TBarButton>
                                <igtbar:TBarButton Key="btnElencoVaccinati" ToolTip="Elenco vaccinati"
									Text="Vaccinati" DisabledImage="~/Images/stampa.gif" Image="~/Images/stampa.gif"></igtbar:TBarButton>
								<igtbar:TBarButton Key="btnElencoNonVaccinati" ToolTip="Elenco non vaccinati in generale (con o senza esclusione)"
									Text="Non Vaccinati" DisabledImage="~/Images/stampa.gif" Image="~/Images/stampa.gif"></igtbar:TBarButton>
								<igtbar:TBarButton Key="btnElencoNonVaccinatiPaziente" ToolTip="Elenco non vaccinati in generale (con o senza esclusione)"
									Text="Non Vaccinati Paziente" DisabledImage="~/Images/stampa.gif" Image="~/Images/stampa.gif">
                                    <DefaultStyle Width="180px" CssClass="infratoolbar_button_default"></DefaultStyle>
								</igtbar:TBarButton>
							</Items>
						</igtbar:UltraWebToolbar>
                    </div>
					<div class="sezione" id="Panel23" runat="server">
						<asp:Label id="LayoutTitolo_sezioneRicerca" runat="server">Filtri di stampa</asp:Label>
                    </div>

                    <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">

                        <div class="vac-riga">
                            <div class="vac-colonna-sinistra">
							    <fieldset title="Comune" class="fldroot" >
                                    <legend class="label">Comune di Residenza</legend>
								    <on_ofm:onitmodallist id="fmComuneRes" runat="server" UseCode="True" Tabella="T_ANA_COMUNI" CampoDescrizione="COM_DESCRIZIONE"
									    CampoCodice="COM_CODICE" Label="Comune" CodiceWidth="28%" LabelWidth="-8px" PosizionamentoFacile="False"
									    Width="70%" RaiseChangeEvent="True" SetUpperCase="True"></on_ofm:onitmodallist>
							    </fieldset>
                            </div>
                            <div class="vac-colonna-destra">
                                <fieldset title="Circoscrizione" class="fldroot" >
                                    <legend class="label">Circoscrizione</legend>
								    <on_ofm:onitmodallist id="fmCircoscrizione" runat="server" UseCode="True" Tabella="T_ANA_CIRCOSCRIZIONI"
									    CampoDescrizione="CIR_DESCRIZIONE" CampoCodice="CIR_CODICE" Label="Circoscrizione" CodiceWidth="28%"
									    LabelWidth="-8px" PosizionamentoFacile="False" Width="70%" RaiseChangeEvent="True" SetUpperCase="True"></on_ofm:onitmodallist>
							    </fieldset>
                            </div>
                        </div>
                        <div class="vac-riga">
                            <div class="vac-colonna-sinistra">
								<fieldset title="Consultorio" class="fldroot" style="height:40px">
                                    <legend class="label">Centro Vaccinale</legend>
									<uc1:SelezioneConsultori id="ucSelezioneConsultori" runat="server" Width="100%"  MaxCnsInListaSelezionati="3"></uc1:SelezioneConsultori>								
								</fieldset>                            
                            </div>
                            <div class="vac-colonna-destra">
                                <fieldset title="Distretto" class="fldroot" style="height:40px">
                                    <legend class="label">Distretto</legend>
									<on_ofm:onitmodallist id="fmDistretto" runat="server" UseCode="True" Tabella="T_ANA_DISTRETTI" CampoDescrizione="DIS_DESCRIZIONE"
										CampoCodice="DIS_CODICE" Label="Distretto" CodiceWidth="15%" LabelWidth="-8px" PosizionamentoFacile="False"
										Width="80%" RaiseChangeEvent="True" SetUpperCase="True"></on_ofm:onitmodallist>
								</fieldset>
                            </div>
                        </div>
                        <div class="vac-riga">
                            <div class="vac-colonna-sinistra">
                                <fieldset title="Data Nascita" class="fldroot">
                                    <legend class="label">Data nascita</legend>
									<table style="width: 100%">
										<tr>
											<td class="label_right">
												<asp:Label id="lblDataNascitaIniz" runat="server">Da</asp:Label></td>
											<td>
												<on_val:onitdatepick id="odpDataNascitaIniz" runat="server" Height="20px" Width="136px" CssClass="textbox_data_obbligatorio" DateBox="True"></on_val:onitdatepick></td>
											<td class="label_right">
                                                <asp:Label id="lblDataNascitaFin" runat="server">A</asp:Label></td>
											<td>
												<on_val:onitdatepick id="odpDataNascitaFin" runat="server" Height="20px" Width="136px" CssClass="textbox_data_obbligatorio" DateBox="True"></on_val:onitdatepick></td>
										</tr>
									</table>
								</fieldset>                            
                            </div>
                            <div class="vac-colonna-destra">
                                <fieldset title="Data effettuazione" class="fldroot">
                                    <legend class="label">Data effettuazione</legend>
									<table style="width: 100%">
										<tr>
											<td class="label_right">
												<asp:Label id="lblDataEffettuazioneIniz" runat="server">Da</asp:Label></td>
											<td>
												<on_val:onitdatepick id="odpDataEffettuazioneIniz" runat="server" Height="20px" Width="136px" CssClass="textbox_data" DateBox="True"></on_val:onitdatepick></td>
											<td class="label_right">
												<asp:Label id="lblDataEffettuazioneFin" runat="server">A</asp:Label></td>
											<td>
												<on_val:onitdatepick id="odpDataEffettuazioneFin" runat="server" Height="20px" Width="136px" CssClass="textbox_data" DateBox="True"></on_val:onitdatepick></td>
										</tr>
									</table>
								</fieldset>
                            </div>
                        </div>
                        <div class="vac-riga">
                            <div class="vac-colonna-sinistra">
								<fieldset title="Tipo vaccinazioni" class="fldroot" >
                                    <legend class="label">Tipo vaccinazioni</legend>
									<onit:CheckBoxList id="chklModVaccinazione" runat="server" CssClass="textbox_stringa" RepeatDirection="Horizontal" TextAlign="Right" AutoPostBack="true"  Width="100%">
										<asp:ListItem Value="A">Obbligatorie</asp:ListItem>
										<asp:ListItem Value="B">Raccomandate</asp:ListItem>
                                        <asp:ListItem Value="C">Facoltative</asp:ListItem>
									</onit:CheckBoxList>
								</fieldset>                            
                            </div>
                            <div class="vac-colonna-destra">
                                <fieldset title="Sesso" class="fldroot">
                                    <legend class="label">Sesso</legend>
									<onit:CheckBoxList id="chklSesso" runat="server" CssClass="textbox_stringa" RepeatDirection="Horizontal" TextAlign="Right" Width="100%">
										<asp:ListItem Value="M">Maschio</asp:ListItem>
										<asp:ListItem Value="F">Femmina</asp:ListItem>
									</onit:CheckBoxList>
								</fieldset>                                
                            </div>
                        </div>
                        <div class="vac-riga">
                            <fieldset title="Stato anagrafico" class="fldroot">
                                <legend class="label">Stato anagrafico</legend>
							    <onit:CheckBoxList id="chklStatoAnagrafico" runat="server" CssClass="textbox_stringa" RepeatDirection="Horizontal" TextAlign="Right" RepeatColumns="5" Width="100%"></onit:CheckBoxList>
						    </fieldset>
                        </div>
                        <div class="vac-riga">
                            <div class="vac-colonna-sinistra">
                                <fieldset title="TipoMedico" class="fldroot vac-fieldset-height-45">
                                    <legend class="label">Tipo Medico</legend>
									<onit:CheckBoxList id="chkTipoMedico" runat="server" CssClass="textbox_stringa" RepeatDirection="Horizontal" TextAlign="Right" Width="100%">
										<asp:ListItem Value="1">MMG</asp:ListItem>
										<asp:ListItem Value="2">PLS</asp:ListItem>
									</onit:CheckBoxList>
								</fieldset>                            
                            </div>
                            <div class="vac-colonna-destra">
                                <fieldset title="Medico" class="fldroot vac-fieldset-height-45">
									<legend class="label margin-bottom-5">Medico di Base</legend>
									<on_ofm:onitmodallist id="omlMedicoBase" runat="server" SetUpperCase="True" UseCode="True" Tabella="T_ANA_MEDICI, T_MED_ABILITAZIONI_VIS_PAZ" IsDistinct="true"
										CampoDescrizione="MED_DESCRIZIONE" CampoCodice="MED_CODICE" CodiceWidth="30%" LabelWidth="-8px"
										PosizionamentoFacile="False" Width="70%"></on_ofm:onitmodallist>
					            </fieldset>                        
                            </div>
                        </div>
                        <div class="vac-riga">
							<fieldset id="Fieldset1" title="Vaccinazioni" class="fldroot" >
                                <legend class="label">Vaccinazioni</legend>
                                <asp:Repeater ID="rptVaccinazioni" runat="server">
                                    <HeaderTemplate>
                                        <table id="tblVaccinazioni" cellSpacing="0" cellPadding="0" width="100%" border="0">
                                            <colgroup>
                                                <col style="width: 40%" />
                                                <col style="width: 10%" />
                                                <col style="width: 50%" />
                                            </colgroup> 
                                            <thead>
                                                <td></td>
                                                <td class="label_left">N. dosi</td>
                                                <td class="label_left">GG vita</td>
                                            </thead>
                                    </HeaderTemplate>
                                    <ItemTemplate>
									        <tr>
										        <td class="label_left" align="left">
											        <asp:Label ID="lblVaccinazioneCodice" runat="server" Text ='<%# Eval("VAC_CODICE") %>' style="display: none" />
                                                    <asp:CheckBox id="chkVaccinazione" runat="server" CssClass="label_left" TextAlign="Right" Text='<%# Eval("VAC_DESCRIZIONE") %>' onclick="chkVaccinazione_CheckedChanged(this)" ></asp:CheckBox>
                                                </td>
                                                <td>
                                                    <asp:TextBox id="txtNumeroDosi" runat="server" Width="62px" CssClass="textbox_stringa_disabilitato"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:TextBox id="txtGiorniVita" runat="server" Width="62px" CssClass="textbox_stringa_disabilitato"></asp:TextBox>
                                                </td>
									        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>
							</fieldset>                            
                        </div>
                    </dyp:DynamicPanel>

					<dyp:DynamicPanel ID="dypMsg" runat="server" Width="100%" Height="18px">
						<asp:Label id="Label1" runat="server" Width="100%" CssClass="label_errorMsg"></asp:Label>
					</dyp:DynamicPanel>

                </on_lay3:OnitLayout3>
            </asp:View>

            <asp:View ID="ViewReport" runat="server">
                <asp:ImageButton ID="btnCloseViewReport" runat="server" OnClick="btnCloseViewReport_Click"
                    ImageUrl="~/Images/onassistnet/telerik_reportviewer_closereport.png" 
                    onmouseover="this.src='../../images/onassistnet/telerik_reportviewer_closereport_hover.png'"
                    onmouseout="this.src='../../images/onassistnet/telerik_reportviewer_closereport.png'" Style="position: absolute;margin-top: 3px; margin-left: 3px;" AlternateText="Chiudi"  />
                <telerik:ReportViewer ID="ReportViewer" runat="server" Height="100%" Width="100%" ShowRefreshButton="false" ShowParametersButton="false" />
            </asp:View>

        </asp:MultiView>
    </form>
</body>
</html>
