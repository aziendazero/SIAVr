<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="StatElencoNonVaccinatiPediatra.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.StatElencoNonVaccinatiPediatra" %>

<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="uc1" TagName="SelezioneConsultori" Src="../../Common/Controls/SelezioneConsultori.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>ElencoNonVaccinatiPediatra</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript" language="javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.StringUtility.js") %>"></script>
    <script type="text/javascript">
        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;

            switch (button.Key) {
                case 'btnStampa':
                    if (!CheckCampiObbligatori()) {
                        evnt.needPostBack = false;
                    }
                    break;
                case 'btnStampaConNote':
                    if (!CheckCampiObbligatori()) {
                        evnt.needPostBack = false;
                    }
                    break;
            }
        }

        //lancio della stampa tramite tasto invio (modifica 29/07/2004)
        function Stampa(evt) {
            if (evt.keyCode == 13) {
                if (!CheckCampiObbligatori()) {
                    evt.needPostBack = false;
                }
                else {
                    __doPostBack("Stampa", "");
                }
            }
        }

        // Controllo campi obbligatori
        function CheckCampiObbligatori() {
            var seduta = document.getElementById('txtSeduta').value;

            if (OnitDataPickGet('odpDataNascitaIniz') == "" || OnitDataPickGet('odpDataNascitaFin') == "" || FullTrim(seduta) == "" || isNaN(seduta)) {
                alert("Non tutti i campi obbligatori sono impostati correttamente. Impossibile stampare il report.");
                return false;
            }

            return true;
        }
    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server" onkeyup="Stampa(event)">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Titolo="Statistiche - <b>Elenco non vaccinati pediatra</>" TitleCssClass="Title3" Width="100%" Height="100%">
			<div class="title" id="PanelTitolo" runat="server">
				<asp:Label id="LayoutTitolo" runat="server"></asp:Label>
            </div>
            <div>
			    <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="Toolbar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
	                <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
				    <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
				    <Items>
					    <igtbar:TBarButton Key="btnStampa" ToolTip="Non considera le vaccinazioni con esclusione o inadempienza."
						    Text="Stampa" DisabledImage="~/Images/stampa.gif" Image="~/Images/Stampa.gif"></igtbar:TBarButton>
                        <igtbar:TBarButton Key="btnStampaConNote" ToolTip="Non considera le vaccinazioni con esclusione o inadempienza. Visualizza anche le note del paziente"
						    Text="Stampa con note" DisabledImage="~/Images/stampa.gif" Image="~/Images/Stampa.gif"></igtbar:TBarButton>
				    </Items>
			    </igtbar:UltraWebToolbar>
            </div>
            <div class="sezione" id="Panel23" runat="server">
                <asp:Label id="LayoutTitolo_sezioneRicerca" runat="server">Filtri di stampa</asp:Label>
            </div>

            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                
                <div class="vac-riga">
                    <div class="vac-colonna-sinistra">
						<fieldset title="Comune" class="fldroot">
							<legend class="label">Comune di Residenza</legend>
							<on_ofm:onitmodallist id="fmComuneRes" runat="server" RaiseChangeEvent="True" SetUpperCase="True" UseCode="True"
								Tabella="T_ANA_COMUNI" CampoDescrizione="COM_DESCRIZIONE" CampoCodice="COM_CODICE" Label="Comune"
								CodiceWidth="30%" LabelWidth="-8px" PosizionamentoFacile="False" Width="70%"></on_ofm:onitmodallist>

						</fieldset>
                    </div>
                    <div class="vac-colonna-destra">
                        <fieldset title="Circoscrizione" class="fldroot">
							<legend class="label">Circoscrizione</legend>
							<on_ofm:onitmodallist id="fmCircoscrizione" runat="server" RaiseChangeEvent="True" SetUpperCase="True"
								UseCode="True" Tabella="T_ANA_CIRCOSCRIZIONI" CampoDescrizione="CIR_DESCRIZIONE" CampoCodice="CIR_CODICE"
								Label="Circoscrizione" CodiceWidth="30%" LabelWidth="-8px" PosizionamentoFacile="False" Width="70%"></on_ofm:onitmodallist>
						</fieldset>
                    </div>
                </div>
                <div class="vac-riga">
                    <div class="vac-colonna-sinistra">
						<fieldset id="fldCnv" title="Centro Vaccinale" class="fldroot vac-fieldset-height-45">
							<legend class="label">Centro Vaccinale</legend>
							<uc1:SelezioneConsultori id="ucSelezioneConsultori" runat="server" Width="100%"  MaxCnsInListaSelezionati="3"></uc1:SelezioneConsultori>
						</fieldset>
                    </div>
                    <div class="vac-colonna-destra">
                        <fieldset id="fldDis" title="Distretto" class="fldroot vac-fieldset-height-45">
							<legend class="label">Distretto</legend>
							<on_ofm:onitmodallist id="fmDistretto" runat="server" RaiseChangeEvent="True" SetUpperCase="True" UseCode="True"
								Tabella="T_ANA_DISTRETTI" CampoDescrizione="DIS_DESCRIZIONE" CampoCodice="DIS_CODICE" Label="Distretto"
								CodiceWidth="30%" LabelWidth="-8px" PosizionamentoFacile="False" Width="70%"></on_ofm:onitmodallist>
						</fieldset>
                    </div>
                </div>
                <div class="vac-riga">
                    <div class="vac-colonna-sinistra">
                        <fieldset title="Data di nascita" class="fldroot vac-fieldset-height-45">
							<legend class="label">Data di nascita</legend>
							<table style="width: 100%">
								<tr>
									<td class="label">
										<asp:Label id="lblDataEffettuazioneIniz" runat="server">Da</asp:Label></td>
									<td>
										<on_val:onitdatepick id="odpDataNascitaIniz" runat="server" CssClass="textbox_data_obbligatorio" DateBox="True" Width="136px"></on_val:onitdatepick></td>
									<td class="label">
										<asp:Label id="lblDataEffettuazioneFin" runat="server">A</asp:Label></td>
									<td>
										<on_val:onitdatepick id="odpDataNascitaFin" runat="server" CssClass="textbox_data_obbligatorio" DateBox="True" Width="136px"></on_val:onitdatepick></td>
								</tr>
							</table>
						</fieldset>
                    </div>
                    <div class="vac-colonna-destra">
                        <fieldset title="Sesso" class="fldroot vac-fieldset-height-45">
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
						<asp:CheckBoxList id="chkStatoAnagrafico" runat="server" CssClass="textbox_stringa" RepeatDirection="Horizontal" RepeatColumns="5" Width="100%"></asp:CheckBoxList>
					</fieldset>
                </div>
                <div class="vac-riga">
                    <div class="vac-colonna-sinistra">
                        <fieldset title="Vaccinazioni" class="fldroot vac-fieldset-height-45">
							<legend class="label">Vaccinazioni</legend>
							<asp:RadioButtonList id="radVaccinazioni" runat="server" Width="100%" CssClass="textbox_stringa" RepeatDirection="Horizontal">
								<asp:ListItem Value="A">Obbligatorie</asp:ListItem>
								<asp:ListItem Value="T" Selected="True">Tutte</asp:ListItem>
							</asp:RadioButtonList>
						</fieldset>
                    </div>
                    <div class="vac-colonna-destra">
                        <fieldset title="Distretto" class="fldroot vac-fieldset-height-45">
							<legend class="label">Medico di Base</legend>
							<on_ofm:onitmodallist id="omlMedicoBase" runat="server" SetUpperCase="True" UseCode="True" Tabella="T_ANA_MEDICI"
								CampoDescrizione="MED_DESCRIZIONE" CampoCodice="MED_CODICE" Label="Distretto" CodiceWidth="30%" LabelWidth="-8px"
								PosizionamentoFacile="False" Width="70%" Filtro="1=1 ORDER BY MED_DESCRIZIONE"></on_ofm:onitmodallist>
					    </fieldset>
                    </div>
                </div>
                <div class="vac-riga">
                    <div class="vac-colonna-sinistra">
                        <fieldset title="Ciclo" class="fldroot">
                            <legend class="label">Ciclo</legend>
                            <asp:DropDownList id="cmbCiclo" runat="server" Width="100%" DataTextField="CIC_DESCRIZIONE" DataValueField="CIC_CODICE"></asp:DropDownList>
						</fieldset>
                    </div>
                    <div class="vac-colonna-destra">
                        <fieldset title="Seduta" class="fldroot">
                            <legend class="label">Seduta</legend>
                            <asp:TextBox id="txtSeduta" runat="server" Width="62px" CssClass="textbox_stringa_obbligatorio"></asp:TextBox>
						</fieldset>
                    </div>
                </div>
            </dyp:DynamicPanel>

            <dyp:DynamicPanel ID="dypMsg" runat="server" Width="100%" Height="18px">
                <asp:Label id="Label1" runat="server" Width="100%" CssClass="label_errormsg"></asp:Label>
            </dyp:DynamicPanel>
				
        </on_lay3:OnitLayout3>
    </form>
</body>
</html>
