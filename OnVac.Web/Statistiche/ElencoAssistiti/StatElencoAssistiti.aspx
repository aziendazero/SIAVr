<%@ Page Language="vb" AutoEventWireup="false" Codebehind="StatElencoAssistiti.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.StatElencoAssistiti"%>

<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="uc1" TagName="SelezioneConsultori" Src="../../Common/Controls/SelezioneConsultori.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>StatElencoAssistiti</title>

        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
        
		<script type="text/javascript">
		    function InizializzaToolBar(t) {
		        t.PostBackButton = false;
		    }
		
		    function ToolBarClick(ToolBar, button, evnt) {
		        evnt.needPostBack = true;

		        var dataNascitaIniz = OnitDataPickGet('odpDataNascitaIniz');
		        var dataNascitaFin = OnitDataPickGet('odpDataNascitaFin');

		        if ((dataNascitaIniz == '' && dataNascitaFin != '') || (dataNascitaIniz != '' && dataNascitaFin == '')) {
		            alert("I campi 'Data Nascita' non sono impostati correttamente. Impossibile stampare il report!!");
		            evnt.needPostBack = false;
		        }
		    }
        
		    function changeFocus(side) {
		        var comboStatusVacc = document.getElementById('ddlStatusVaccinale');
		        var comboMalCronica = document.getElementById('cmbMalCronica');

		        // Abilitazione di una delle due combo
		        if (side) {
		            comboStatusVacc.selectedIndex = -1;
		        } else {
		            comboMalCronica.selectedIndex = -1;
		        }

		        // Valorizzazione campo di testo nascosto
		        if (side) {
		            document.getElementById('txtStatusVaccEnabled').value = 'False';
		        }
		        else {
		            document.getElementById('txtStatusVaccEnabled').value = 'True';
		        }
		    }

            // In base al valore del campo txtStatusVaccEnabled, abilita una delle due combo ddlStatusVaccinale o cmbMalCronica.
            // Viene richiamato all'onload per abilitare il controllo, per mantenere abilitato il controllo anche dopo la stampa.
		    function enableStatusVacc() {
		        if (document.getElementById('txtStatusVaccEnabled').value == 'True') {
		            changeFocus(false);
		        }
		        else {
		            changeFocus(true);
		        }
		    }
		</script>
        <style type="text/css">
            .margin-bottom-5 {
                margin-bottom: 5px;
            }
        </style>
	</head>
	<body onload="enableStatusVacc();">
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" TitleCssClass="Title3" Titolo="Statistiche - <b>Elenco assistiti</b>" height="100%" width="100%">
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
							<igtbar:TBarButton Key="btnStampa" DisabledImage="~/Images/stampa.gif" Text="Stampa Assistiti" Image="~/Images/Stampa.gif"></igtbar:TBarButton>
							<igtbar:TBarButton Key="btnStampaEtichette" DisabledImage="~/Images/stampa_dis.gif" Text="Stampa Etichette" Image="~/Images/Stampa.gif"></igtbar:TBarButton>
							<igtbar:TBarButton Key="btnStampaEtichetteSpedizione" DisabledImage="~/Images/stampa_dis.gif" Text="Stampa Etichette Spedizione" Image="~/Images/Stampa.gif">
                                <DefaultStyle Width="200px" CssClass="infratoolbar_button_default"></DefaultStyle>
                            </igtbar:TBarButton>
							<igtbar:TBarButton Key="btnStampaDocumentazione" DisabledImage="~/Images/stampa.gif" Text="Stampa Documentazione" Image="~/Images/Stampa.gif">
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
                            <fieldset title="Comune" class="fldroot">
							    <legend class="label">Comune di Residenza</legend>
								<on_ofm:onitmodallist id="fmComuneRes" runat="server" SetUpperCase="True" UseCode="True" Tabella="T_ANA_COMUNI"
									CampoDescrizione="COM_DESCRIZIONE" CampoCodice="COM_CODICE" Label="Comune" CodiceWidth="28%" LabelWidth="-8px"
									PosizionamentoFacile="False" Width="70%" RaiseChangeEvent="True"></on_ofm:onitmodallist>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset title="Circoscrizione" class="fldroot">
								<legend class="label">Circoscrizione</legend>
								<on_ofm:onitmodallist id="fmCircoscrizione" runat="server" SetUpperCase="True" UseCode="True" Tabella="T_ANA_CIRCOSCRIZIONI"
									CampoDescrizione="CIR_DESCRIZIONE" CampoCodice="CIR_CODICE" Label="Circoscrizione" CodiceWidth="28%"
									LabelWidth="-8px" PosizionamentoFacile="False" Width="70%" RaiseChangeEvent="True"></on_ofm:onitmodallist>
							</fieldset>                                
                        </div>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset title="Centro Vaccinale" class="fldroot vac-fieldset-height-45">
								<legend class="label margin-bottom-5">Centro Vaccinale</legend>
								<uc1:SelezioneConsultori id="ucSelezioneConsultori" runat="server" Width="100%"  MaxCnsInListaSelezionati="3"></uc1:SelezioneConsultori>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset title="Data nascita" class="fldroot vac-fieldset-height-45">
							    <legend class="label">Data nascita</legend>
								<table style="width: 100%">
									<tr>
										<td class="label_right">
											<asp:Label id="lblDataNascitaIniz" runat="server">Da</asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataNascitaIniz" runat="server" Height="20px" Width="136px" CssClass="textbox_data" DateBox="True"></on_val:onitdatepick></td>
										<td class="label_right">
											<asp:Label id="lblDataNascitaFin" runat="server">A</asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataNascitaFin" runat="server" Height="20px" Width="136px" CssClass="textbox_data" DateBox="True"></on_val:onitdatepick></td>
									</tr>
								</table>
							</fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <fieldset id="fldStatoAnag" title="Stato anagrafico" class="fldroot">
							<legend class="label">Stato anagrafico</legend>
							<asp:CheckBoxList id="chklStatoAnagrafico" runat="server" CssClass="label_left" RepeatDirection="Horizontal" RepeatColumns="5" TextAlign="Right" Width="100%"></asp:CheckBoxList>
						</fieldset>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset id="fldStatoVac" title="Status vaccinale" class="fldroot">
	                            <legend class="label">Status vaccinale</legend>
								<asp:DropDownList id="ddlStatusVaccinale" runat="server" Width="100%" onchange="changeFocus(false);" >
									<asp:ListItem Selected="True"></asp:ListItem>
									<asp:ListItem Value="3">IN CORSO</asp:ListItem>
									<asp:ListItem Value="4">TERMINATO</asp:ListItem>
									<asp:ListItem Value="9">INADEMPIENTE TOTALE</asp:ListItem>
								</asp:DropDownList>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset title="Motivi esclusione" class="fldroot">
							    <legend class="label">Elenco malattia cronica</legend>
							    <asp:DropDownList id="cmbMalCronica" runat="server" Width="100%" DataTextField="MAL_DESCRIZIONE" DataValueField="MAL_CODICE" onchange="changeFocus(true);"></asp:DropDownList>
						    </fieldset>                                
                        </div>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset title="Regolarizzazione" class="fldroot">
	                            <legend class="label">Regolarizzazione</legend>
								<asp:DropDownList id="ddlRegolarizzazione" runat="server" Width="100%">
									<asp:ListItem Selected="True" />
									<asp:ListItem Value="S">REGOLARIZZATO</asp:ListItem>
									<asp:ListItem Value="N">DA REGOLARIZZARE</asp:ListItem>
								</asp:DropDownList>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
							<fieldset title="Sesso" class="fldroot">
	                            <legend class="label">Sesso</legend>
								<asp:DropDownList id="ddlSesso" runat="server" Width="100%">
									<asp:ListItem Selected="True" />
									<asp:ListItem Value="M">MASCHIO</asp:ListItem>
									<asp:ListItem Value="F">FEMMINA</asp:ListItem>
								</asp:DropDownList>
							</fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset id="fldCategoriaRischio" title="Categoria Rischio" class="fldroot">
                                <legend class="label">Categoria rischio</legend>
								<asp:DropDownList id="cmbCatRischio" runat="server" Width="100%" DataTextField="RSC_DESCRIZIONE" DataValueField="RSC_CODICE" Enabled="true"></asp:DropDownList>
							</fieldset>                                
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset title="Paziente locale" class="fldroot">
                                <legend class="label">Paziente locale</legend>
                                <asp:DropDownList id="ddlPazienteLocale" runat="server" Width="100%">
                                    <asp:ListItem Selected="True"></asp:ListItem>
                                    <asp:ListItem Value="S">S</asp:ListItem>
                                    <asp:ListItem Value="N">N</asp:ListItem>
                                </asp:DropDownList>
                            </fieldset>                                
                        </div>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset title="Gestione manuale pazienti" class="fldroot">
                                <legend class="label">Gestione manuale pazienti</legend>
                                <asp:DropDownList id="ddlGestioneManuale" runat="server" width="100%">
                                    <asp:ListItem Selected="True"></asp:ListItem>
                                    <asp:ListItem Value="S">S</asp:ListItem>
                                    <asp:ListItem Value="N">N</asp:ListItem>
                                </asp:DropDownList>
                            </fieldset>                                
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset runat="server" id="fldStatoAcquisizione" title="Stato Acquisizione" class="fldroot" >
                                <legend class="label">Stato Acquisizione</legend>
								<asp:DropDownList id="ddlStatoAcquisizione" runat="server" Width="100%">
									<asp:ListItem Selected="True" />
                                    <asp:ListItem Value="-1">ACQUISIZIONE NON EFFETTUATA</asp:ListItem>
									<asp:ListItem Value="0">NESSUN DATO DA ACQUISIRE</asp:ListItem>
									<asp:ListItem Value="1">ACQUISIZIONE PARZIALE</asp:ListItem>
									<asp:ListItem Value="2">ACQUISIZIONE TOTALE</asp:ListItem>
								</asp:DropDownList>
                            </fieldset>                                
                        </div>
                    </div>
                    <asp:TextBox ID="txtStatusVaccEnabled" runat="server" Value="True" style="display:none" />
                </dyp:DynamicPanel>

                <dyp:DynamicPanel ID="dypMsg" runat="server" Width="100%" Height="18px">
                    <div class="label_errorMsg" style="width:100%;">
					    <asp:Label id="Label1" runat="server" BorderColor="#8080FF" BorderWidth="1px" BorderStyle="Solid"></asp:Label>
                    </div>
                </dyp:DynamicPanel>

			</on_lay3:onitlayout3>
		</form>
	</body>
</html>