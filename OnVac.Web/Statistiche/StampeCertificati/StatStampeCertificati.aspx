<%@ Page Language="vb" AutoEventWireup="false" Codebehind="StatStampeCertificati.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.StampaCertificati" %>

<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="uc1" TagName="SelezioneConsultori" Src="../../Common/Controls/SelezioneConsultori.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>Stampa Certificati</title>
		
        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

		<script type="text/javascript">
		    function InizializzaToolBar(t) {
		        t.PostBackButton = false;
		    }

		    function ToolBarClick(ToolBar, button, evnt) {
		        evnt.needPostBack = true;

		        switch (button.Key) {
		            case 'btnStampaVaccinale':
		                controllaDatiImmessi(evnt);
		                break;
		        }
		    }

		    function controllaDatiImmessi(evnt) {
		        if (OnitDataPickGet('odpDataNascitaIniz') == "" || OnitDataPickGet('odpDataNascitaFin') == "") {
		            alert("Non tutti i campi obbligatori sono impostati. Impossibile stampare il report.");
		            evnt.needPostBack = false;
		        }
		    }	
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" height="100%" width="100%" Titolo="Statistiche - <b>Stampe certificati</b>" TitleCssClass="Title3">
				<div class="title" id="PanelTitolo" runat="server">
					<asp:Label id="LayoutTitolo" runat="server"></asp:Label>
                </div>
                <div>
				    <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="Toolbar" runat="server" ItemWidthDefault="140px" CssClass="infratoolbar">
                        <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                        <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
	                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                        <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
					    <Items>
						    <igtbar:TBarButton Key="btnStampaVaccinale" DisabledImage="~/Images/stampa.gif" Text="Certificato Vaccinale"
							    Image="~/Images/Stampa.gif">
						    </igtbar:TBarButton>
						    <igtbar:TBarButton Key="btnStampaVaccinaleLotti" DisabledImage="~/Images/stampa.gif" Text="Certificato Vaccinale Lotti"
							    Image="~/Images/Stampa.gif">
							    <DefaultStyle Width="180px" CssClass="infratoolbar_button_default"></DefaultStyle>
						    </igtbar:TBarButton>
					    </Items>
				    </igtbar:UltraWebToolbar>
                </div>
				<div class="vac-sezione" id="Panel23" runat="server">
					<asp:Label id="LayoutTitolo_sezioneRicerca" runat="server">Filtri di stampa</asp:Label>
                </div>
                <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset title="Centro Vaccinale a cui appartiene il paziente" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Centro Vaccinale paziente</legend>
								<uc1:SelezioneConsultori id="ucSelezioneConsultori" runat="server" Width="100%"  MaxCnsInListaSelezionati="3"></uc1:SelezioneConsultori>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset title="Data di nascita" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Data di nascita</legend>
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
                    </div>
                    <div class="vac-riga">
                        <div class="vac-colonna-sinistra">
                            <fieldset title="Centro Vaccinale di effettuazione delle vaccinazioni" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Centro Vaccinale effettuazione</legend>
								<uc1:SelezioneConsultori id="ucSelezioneConsultoriVaccinazioni" runat="server" Width="100%"  MaxCnsInListaSelezionati="3"></uc1:SelezioneConsultori>
							</fieldset>
                        </div>
                        <div class="vac-colonna-destra">
                            <fieldset title="Data di nascita" class="fldroot vac-fieldset-height-45">
                                <legend class="label">Data di effettuazione</legend>
								<table style="width: 100%">
                                    <tr>
                                        <td class="label_right">
                                            <asp:Label id="lblDataVacIniz" runat="server">Da</asp:Label></td>
										<td>
                                            <on_val:onitdatepick id="odpDataVacIniz" runat="server" Height="20px" Width="136px" CssClass="textbox_data" DateBox="True"></on_val:onitdatepick></td>
                                        <td class="label_right">
                                            <asp:Label id="lblDataVacFin" runat="server">A</asp:Label></td>
										<td>
                                            <on_val:onitdatepick id="odpDataVacFin" runat="server" Height="20px" Width="136px" CssClass="textbox_data" DateBox="True"></on_val:onitdatepick></td>
									</tr>
								</table>
							</fieldset>
                        </div>
                        <div class="vac-riga">
                            <fieldset title="Numero dosi" class="fldroot">
                                <legend class="label">Numero dosi</legend>
								<asp:TextBox id="txtNumeroDosi" runat="server" Width="100px" CssClass="textbox_stringa"></asp:TextBox>
							</fieldset>
                        </div>
                        <div class="vac-riga">
                            <fieldset title="Vaccinazioni" class="fldroot" >
                                <legend class="label">Vaccinazioni</legend>
                                <onit:CheckBoxList id="chkVaccinazioni" runat="server" CssClass="label_left" RepeatDirection="Horizontal" TextAlign="Right" RepeatColumns="5" Width="100%"></onit:CheckBoxList>
					        </fieldset>
                        </div>
                    </div>
                </dyp:DynamicPanel>
				
                <dyp:DynamicPanel ID="dypMsg" runat="server" Width="100%" Height="18px">
					<asp:Label id="Label1" runat="server" Width="100%" CssClass="label_errorMsg"></asp:Label>
				</dyp:DynamicPanel>

			</on_lay3:onitlayout3>
		</form>
	</body>
</html>
