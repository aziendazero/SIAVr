<%@ Page Language="vb" AutoEventWireup="false" Codebehind="StatCoperturaVaccinale.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.CoperturaVaccinale" %>

<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="uc1" TagName="SelezioneConsultori" Src="../../Common/Controls/SelezioneConsultori.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>Copertura Vaccinale</title>
        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
		<script type="text/javascript" >
		    function InizializzaToolBar(t) {
		        t.PostBackButton = false;
		    }

		    function ToolBarClick(ToolBar, button, evnt) {
		        evnt.needPostBack = true;

		        // Controlli (comuni a tutte le stampe)
		        control = true;
		        campiValorizzati = true;

		        if (!(checkDosi() && checkGiorni())) {
		            control = false;
		        }

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

		    function checkDosi() {
		        val = document.getElementById('txtNumeroDosi').value;
		        if (isNaN(val) || (val > 100)) {
		            alert("Il 'Numero dosi' deve essere un numero valido!!!");
		            return false;
		        }
		        return true;
		    }

		    function checkGiorni() {
		        val = document.getElementById('txtGiorniVita').value;
		        if (isNaN(val)) {
		            alert("Il 'Numero giorni di vita' deve essere un numero valido!!!");
		            return false;
		        }
		        return true;
		    }	
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
		
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" width="100%" height="100%" Titolo="Statistiche - <b>Copertura vaccinale</b>" TitleCssClass="Title3">
			    <div class="title" id="PanelTitolo" runat="server">
				    <asp:Label id="LayoutTitolo" runat="server"></asp:Label>
                </div>
                <div>
					<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="Toolbar" runat="server" ItemWidthDefault="100px" CssClass="infratoolbar">
                        <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                        <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		                <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
						<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
						<Items>
							<igtbar:TBarButton Key="btnStampa" Text="Copertura Vaccinazione" ToolTip="Copertura vaccinale per vaccinazione" 
								Image="~/Images/Stampa.gif" DisabledImage="~/Images/stampa.gif">
								<DefaultStyle Width="180px" CssClass="infratoolbar_button_default"></DefaultStyle>
							</igtbar:TBarButton>
							<igtbar:TBarButton Key="btnCoperturaAss" Text="Copertura Associazione" ToolTip="Copertura vaccinale per associazione"
								Image="~/Images/Stampa.gif" DisabledImage="~/Images/stampa.gif">
								<DefaultStyle Width="180px" CssClass="infratoolbar_button_default"></DefaultStyle>
							</igtbar:TBarButton>										
							<igtbar:TBarButton Key="btnMotiviInadempienti" Text="Motivi Esclusione" ToolTip="Motivi esclusione"
								Image="~/Images/stampa.gif" DisabledImage="~/Images/stampa.gif">
								<DefaultStyle Width="130px" CssClass="infratoolbar_button_default"></DefaultStyle>
                            </igtbar:TBarButton>
                            <igtbar:TBarButton Key="btnElencoVaccinati" Text="Vaccinati" ToolTip="Elenco vaccinati"
								Image="~/Images/stampa.gif" DisabledImage="~/Images/stampa.gif"></igtbar:TBarButton>
							<igtbar:TBarButton Key="btnElencoNonVaccinati" Text="Non Vaccinati" ToolTip="Elenco non vaccinati in generale (con o senza esclusione)"
								Image="~/Images/stampa.gif" DisabledImage="~/Images/stampa.gif"></igtbar:TBarButton>
							<igtbar:TBarButton Key="btnStampaCns" Text="Copertura CV" ToolTip="Copertura vaccinale per centro"
                                Image="~/Images/Stampa.gif" DisabledImage="~/Images/stampa.gif"></igtbar:TBarButton>
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
                            <fieldset title="Consultorio" class="fldroot" style="height:40px" >
                            <legend class="label" >Centro Vaccinale</legend>
								  <uc1:SelezioneConsultori id="ucSelezioneConsultori" runat="server" Width="100%"  MaxCnsInListaSelezionati="3"></uc1:SelezioneConsultori>								
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset title="Distretto" class="fldroot" style="height:40px">
                                <legend class="label">Distretto</legend>
                                <on_ofm:onitmodallist id="fmDistretto" runat="server" UseCode="True" Tabella="T_ANA_DISTRETTI" CampoDescrizione="DIS_DESCRIZIONE"
									CampoCodice="DIS_CODICE" Label="Distretto" CodiceWidth="28%" LabelWidth="-8px" PosizionamentoFacile="False"
									Width="70%" RaiseChangeEvent="True" SetUpperCase="True"></on_ofm:onitmodallist>
							</fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
							<fieldset title="Data Nascita" class="fldroot">
                                <legend class="label">Data nascita</legend>
								<table style="width:100%">
									<tr>
										<td class="label_right">
											<asp:Label id="lblDataNascitaIniz" runat="server" CssClass="label">Da</asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataNascitaIniz" runat="server" Height="20px" Width="136px" CssClass="textbox_data_obbligatorio" DateBox="True"></on_val:onitdatepick></td>
										<td class="label_right">
											<asp:Label id="lblDataNascitaFin" runat="server" CssClass="label">A</asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataNascitaFin" runat="server" Height="20px" Width="136px" CssClass="textbox_data_obbligatorio" DateBox="True"></on_val:onitdatepick></td>
									</tr>
								</table>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
							<fieldset id="fldDataEff" title="Data effettuazione" class="fldroot" >
                                <legend class="label">Data effettuazione</legend>
								<table style="width:100%">
									<tr>
										<td class="label_right">
											<asp:Label id="lblDataEffettuazioneIniz" runat="server" CssClass="label">Da</asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataEffettuazioneIniz" runat="server" Height="20px" Width="136px" CssClass="textbox_data" DateBox="True"></on_val:onitdatepick></td>
										<td class="label_right">
											<asp:Label id="lblDataEffettuazioneFin" runat="server" CssClass="label">A</asp:Label></td>
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
								<onit:CheckBoxList id="chklModVaccinazione" runat="server" CssClass="textbox_stringa" RepeatDirection="Horizontal" TextAlign="Right" AutoPostBack="true" Width="100%">
									<asp:ListItem Value="A">Obbligatorie</asp:ListItem>
									<asp:ListItem Value="B">Raccomandate</asp:ListItem>
                                    <asp:ListItem Value="C">Facoltative</asp:ListItem>
								</onit:CheckBoxList>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
							<fieldset id="fldSesso" title="Sesso" class="fldroot" >
                                <legend class="label">Sesso</legend>
                                <onit:CheckBoxList id="chklSesso" runat="server" CssClass="textbox_stringa" RepeatDirection="Horizontal" TextAlign="Right" Width="100%">
									<asp:ListItem Value="M">Maschio</asp:ListItem>
									<asp:ListItem Value="F">Femmina</asp:ListItem>
								</onit:CheckBoxList>
							</fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <fieldset title="Stato anagrafico" class="fldroot" >
                            <legend class="label">Stato anagrafico</legend>
                            <onit:CheckBoxList id="chklStatoAnagrafico" runat="server" CssClass="label_left" RepeatDirection="Horizontal" TextAlign="Right" RepeatColumns="5" Width="100%"></onit:CheckBoxList>
						</fieldset>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset title="Numero dosi" class="fldroot">
                                <legend class="label">Numero dosi</legend>
								<asp:TextBox id="txtNumeroDosi" runat="server" Width="62px" CssClass="textbox_stringa_obbligatorio"></asp:TextBox>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset id="fldGgVita" title="Giorni di vita" class="fldroot" >
                                <legend class="label">Giorni di vita</legend>
								<asp:TextBox id="txtGiorniVita" runat="server" Width="62px" CssClass="textbox_stringa_obbligatorio"></asp:TextBox>
							</fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <fieldset title="Vaccinazioni" class="fldroot" >
                            <legend class="label">Vaccinazioni</legend>
                            <onit:CheckBoxList id="chkVaccinazioni" runat="server" CssClass="label_left" RepeatDirection="Horizontal" TextAlign="Right" RepeatColumns="5" Width="100%"></onit:CheckBoxList>
					    </fieldset>
                    </div>

                </dyp:DynamicPanel>

				<dyp:DynamicPanel ID="dypMsg" runat="server" Width="100%" Height="18px">
                    <asp:Label id="Label1" runat="server" Width="100%" CssClass="label_errorMsg"></asp:Label>
				</dyp:DynamicPanel>

			</on_lay3:onitlayout3>
			
           
			
		</form>
	</body>
</html>
