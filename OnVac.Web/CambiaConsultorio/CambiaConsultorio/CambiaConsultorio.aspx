<%@ Page Language="vb" AutoEventWireup="false" Codebehind="CambiaConsultorio.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.CambiaConsultorio"%>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>CambiaCentroVaccinale</title>
		
        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

        <style type="text/css">
            .msg {
                color: blue;
                border: navy 1px solid;
                background-color: whitesmoke;
                font-family: verdana;
                font-size: 12px;
                margin-bottom: 30px;
                padding-top: 20px; 
                padding-left: 20px;
                padding-bottom: 20px; 
            }
        </style>

		<script type="text/javascript">
		    function InizializzaToolBar(t) {
		        t.PostBackButton = false;
		    }
		
		    function ToolBarClick(ToolBar, button, evnt) {
		        evnt.needPostBack = true;

		        switch (button.Key) {
		            case 'btnConferma':
		                consultorio = document.getElementById("txtConsultorio").value;

		                if (!confirm("Attenzione: procedere con il cambio del centro vaccinale in\n" + consultorio + "?")) {
		                    evnt.needPostBack = false;
		                } else {
		                    evnt.needPostBack = true;
		                }

		                break;

		            default:
		                evnt.needPostBack = true;
		        }
		    }
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout" runat="server" Width="912px" Height="528px" Titolo="Cambio Centro Vaccinale" TitleCssClass="Title3">
				<div class="title" id="PanelTitolo" runat="server">
					<asp:Label id="LayoutTitolo" runat="server"> CAMBIO CENTRO VACCINALE </asp:Label>
                </div>
				<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
					<DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
					<SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
					<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
					<Items>
						<igtbar:TBarButton Key="btnConferma" DisabledImage="~/Images/conferma_dis.gif" Text="Conferma"
							Image="~/Images/conferma.gif"></igtbar:TBarButton>
						<igtbar:TBarButton Key="btnAnnulla" DisabledImage="~/Images/annullaConf_dis.gif" Text="Annulla"
							Image="~/Images/annullaConf.gif"></igtbar:TBarButton>
					</Items>
				</igtbar:UltraWebToolbar>
				<div class="msg">
                    Questa modifica riguarderà solamente la sessione corrente, non influenzerà l'associazione del centro vaccinale per la macchina in uso.
				</div>
                <div style="margin-left:10px">
					<fieldset id="fldConsultorio" title="Centro Vaccinale" class="fldroot" style="width: 90%;">
						<legend class="label">Centro Vaccinale di lavoro</legend>
						<table id="tblCns" cellspacing="0" cellpadding="0" width="100%" border="0">
							<tr>
								<td style="padding: 15px">
									<on_ofm:onitmodallist id="txtConsultorio" runat="server" Width="70%" PosizionamentoFacile="False" LabelWidth="-8px"
										CodiceWidth="30%" CampoCodice="CNS_CODICE" CampoDescrizione="CNS_DESCRIZIONE" Tabella="T_ANA_CONSULTORI"
										UseCode="True" Obbligatorio="False" SetUpperCase="True" RaiseChangeEvent="True"></on_ofm:onitmodallist></td>
							</tr>
							<tr>
								<td style="padding-right: 15px; padding-left: 15px; padding-bottom: 15px; text-align: center;">
									<asp:Label id="lblRisultato" style="font-size: 15px; font-family: Arial,Tahoma;  "
										Width="100%" ForeColor="#ff0066" Runat="server"></asp:Label></td>
							</tr>
						</table>
					</fieldset>
                </div>
			</on_lay3:onitlayout3>
        </form>
	</body>
</html>
