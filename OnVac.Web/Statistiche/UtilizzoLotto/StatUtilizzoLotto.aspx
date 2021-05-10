<%@ Page Language="vb" AutoEventWireup="false" Codebehind="StatUtilizzoLotto.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.UtilizzoLotto" %>

<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="uc1" TagName="SelezioneConsultori" Src="../../Common/Controls/SelezioneConsultori.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>Utilizzo Lotto</title>

        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

		<script type="text/javascript">	
		    function InizializzaToolBar(t) {
		        t.PostBackButton = false;
		    }
		
		    function ToolBarClick(ToolBar, button, evnt) {
		        evnt.needPostBack = true;

		        switch (button.Key) {
		            case 'btnStampa':
		                if (document.getElementById('fmLotto').value == "") {
		                    alert("Nessun lotto inserito. Impossibile stampare il report.");
		                    evnt.needPostBack = false;
                        }
                        if ((!OnitDataPickGet('odpDataEffettuazioneIniz') == "" && OnitDataPickGet('odpDataEffettuazioneFin') == "") || (OnitDataPickGet('odpDataEffettuazioneIniz') == "" && !OnitDataPickGet('odpDataEffettuazioneFin') == "")) {
                            alert("Valorizzare sia la data di inizio sia di fine effettuazione per impostare il filtro. Impossibile stampare il report.");
                            evnt.needPostBack = false;
                        }
		                break;
		        }
		    }
		
		//lancio della stampa tramite tasto invio (modifica 29/07/2004)
		    function Stampa(evt) {
		        if (evt.keyCode == 13) {
		            if (document.getElementById('fmLotto').value == "") {
		                alert("Nessun lotto inserito. Impossibile stampare il report.");
		                evnt.needPostBack = false;
		            }
		            else {
		                __doPostBack("Stampa", "");
		            }
		        }
		    }	
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server" onkeyup="Stampa(event)">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" width="100%" height="100%" TitleCssClass="Title3" Titolo="Statistiche - <b>Utilizzo lotto</b>">
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
						    <igtbar:TBarButton Key="btnStampa" DisabledImage="~/Images/stampa.gif" Text="Stampa" Image="~/Images/Stampa.gif"></igtbar:TBarButton>
					    </Items>
				    </igtbar:UltraWebToolbar>
                </div>
				<div class="sezione" id="Panel23" runat="server">
					<asp:Label id="LayoutTitolo_sezioneRicerca" runat="server">Filtri di stampa</asp:Label>
                </div>

                <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset title="Centro Vaccinale" class="fldroot" style="height:40px">
                                <legend class="label">Centro Vaccinale</legend>
								<uc1:SelezioneConsultori id="ucSelezioneConsultori" runat="server" Width="100%"  MaxCnsInListaSelezionati="3"></uc1:SelezioneConsultori>								
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset title="Lotto" class="fldroot" style="height:40px">
                                <legend class="label">Lotto</legend>
								<on_ofm:onitmodallist id="fmLotto" runat="server" UseCode="True" Tabella="T_ANA_LOTTI" CampoDescrizione="LOT_DESCRIZIONE"
									CampoCodice="LOT_CODICE" Label="Consultorio" CodiceWidth="28%" LabelWidth="-8px" PosizionamentoFacile="False"
									CssClass="textbox_stringa" Width="70%" SetUpperCase="True" Obbligatorio="True"></on_ofm:onitmodallist>
							</fieldset>
                        </div>
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset title="Data di effettuazione" class="fldroot">
                                <legend class="label">Data di effettuazione</legend>
								<table style="width: 100%">
									<tr>
										<td class="label">
											<asp:Label id="lblDataEffettuazioneIniz" runat="server">Da</asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataEffettuazioneIniz" runat="server" Height="20px" CssClass="textbox_data" Width="136px" DateBox="True"></on_val:onitdatepick></td>
										<td class="label">
											<asp:Label id="lblDataEffettuazioneFin" runat="server">A</asp:Label></td>
										<td>
											<on_val:onitdatepick id="odpDataEffettuazioneFin" runat="server" Height="20px" CssClass="textbox_data" Width="136px" DateBox="True"></on_val:onitdatepick></td>
									</tr>
								</table>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">

                        </div>
                    </div>
                </dyp:DynamicPanel>

                <dyp:DynamicPanel ID="dypMsg" runat="server" Width="100%" Height="18px">
                    <asp:Label id="Label1" runat="server" CssClass="label_errorMsg" Width="100%"></asp:Label>
                </dyp:DynamicPanel>

			</on_lay3:onitlayout3>
		</form>
	</body>
</html>
