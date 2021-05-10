<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CalendarioCup.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_CalendarioCup" %>

<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Calendario</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <style type="text/css">
        .dgr {
            width: 100%;
        }

        .dgr2 {
            border: 1px solid gray;
            width: 95%;
            border-collapse: collapse;
        }

        .dgr td {
            font-size: 12px;
            font-family: Tahoma;
        }

        .dgr2 td {
            font-size: 10px;
        }

        .r1 {
            background-color: whitesmoke;
        }

        .r2 {
            background-color: #e7e7ff;
        }

        .h1 {
            font-weight: bold;
            color: #4a3c8c;
            border-style: none;
            background-color: lightsteelblue;
        }

        .cExp {
            padding: 2px;
            color: white;
            background-color: #4a3c8c;
        }

        .tdInt {
            font-weight: bold;
            color: #4a3c8c;
            background-color: lightsteelblue;
        }
    </style>

    <script type="text/javascript" language="javascript">
        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        //controllo valore dei datepick
        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;
            switch (button.Key) {
                case 'btnCerca':

                    if ((!isValidFinestraModale('txtConsultorio', true)) || OnitDataPickGet("txtData") == '') {
                        alert("Impostare tutti i filtri di ricerca!");
                        evnt.needPostBack = false;
                    }

                    break;
            }
        }

    </script>

</head>
<body>
    <form id="Form1" method="post" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Height="100%" Width="100%" Titolo="Appuntamenti del Giorno" TitleCssClass="Title3">
                       
            <div class="title" id="PanelTitolo" runat="server">
				<asp:Label id="LayoutTitolo" runat="server"> Appuntamenti del giorno </asp:Label>
            </div>

            <div>
				<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="170px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
					<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
					<Items>
						<igtbar:TBarButton Key="btnCerca" Text="Cerca" DisabledImage="~/Images/cerca.gif"
							Image="~/Images/cerca.gif"></igtbar:TBarButton>
						<igtbar:TBarButton Key="btnStampa" Text="Stampa appuntamenti" DisabledImage="~/Images/stampa.gif"
							Image="~/Images/stampa.gif"></igtbar:TBarButton>
					</Items>
				</igtbar:UltraWebToolbar>
            </div>
			<div class="sezione" id="Panel23" runat="server">
				<asp:Label id="LayoutTitolo_sezioneRicerca" runat="server">Filtri</asp:Label>
            </div>
            <div>
				<table id="Table1" style="WIDTH: 100%" cellspacing="1" cellpadding="1" width="856" border="0">
					<tr>
						<td width="50%">
							<fieldset class="fldroot" id="fldConsultorio" title="Consultorio">
								<legend class="label">Centro Vaccinale</legend>
								<table id="tblCns" cellspacing="0" cellpadding="0" width="100%" border="0">
									<tr>
										<td valign="bottom" align="center">
											<on_ofm:onitmodallist id="txtConsultorio" runat="server" PosizionamentoFacile="True" LabelWidth="-8px"
												CodiceWidth="64px" Label="Consultorio" CampoCodice="CNS_CODICE" CampoDescrizione="CNS_DESCRIZIONE"
												Tabella="T_ANA_CONSULTORI" UseCode="True" Obbligatorio="True" SetUpperCase="True" Width="232px" Filtro="cns_data_apertura <= SYSDATE AND (cns_data_chiusura > SYSDATE OR cns_data_chiusura IS NULL) ORDER BY cns_descrizione"></on_ofm:onitmodallist></td>
									</tr>
								</table>
							</fieldset>
						</td>
						<td width="5%"></td>
						<td>
							<fieldset class="fldroot" id="fldData" title="Data" style="WIDTH: 200px">
								<legend class="label">Data</legend>
								<table id="tblData" cellspacing="0" cellpadding="0" width="100%" border="0">
									<tr>
										<td valign="bottom" align="center">
											<on_val:onitdatepick id="txtData" runat="server" Height="20px" Width="136px" CssClass="textbox_stringa_obbligatorio"
												DateBox="True"></on_val:onitdatepick></td>
									</tr>
								</table>
							</fieldset>
						</td>
					</tr>
				</table>
            </div>
			<div class="sezione" id="Div13" runat="server">
				<asp:Label id="Label2" runat="server">Risultati ricerca</asp:Label>
            </div>

            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
				<table id="TableTestateEsterna" style="table-layout: fixed" cellspacing="0" cellpadding="0" width="100%" border="0" runat="server">
					<tr style="display: block">
						<td>
							<table class="title" style="table-layout: fixed" cellspacing="0" cellpadding="0" width="100%">
								<tr>
									<td width="55"></td>
									<td width="10%">Ora</td>
									<td width="48%">Cognome e nome</td>
									<td width="15%">Data di nascita</td>
									<td width="15%">Richiesta</td>
									<td width="10%">Tipo</td>
									<td width="20"></td>
								</tr>
							</table>
						</td>
					</tr>
					<tr>
						<td>
							<asp:DataGrid id="dgrTestate" runat="server" Width="100%" CssClass="dgr" AutoGenerateColumns="False"
								ShowHeader="False" GridLines="None">
								<AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
								<ItemStyle BackColor="#E7E7FF"></ItemStyle>
								<HeaderStyle Font-Bold="True" ForeColor="White" BackColor="#4A3C8C"></HeaderStyle>
								<Columns>
									<asp:TemplateColumn>
										<ItemTemplate>
											<table id="TableTestate" style="table-layout: fixed" cellspacing="0" cellpadding="0" width="100%" border="0" runat="server">
												<tr>
													<td width="20"></td>
													<td width="15"><IMG id="imgEspandi2" style="CURSOR: hand" alt="" src="../../images/meno.gif" runat="server"></td>
													<td width="20"></td>
													<td width="10%">
														<asp:Label id="lblOrario" runat="server" Text='<%# CDate(Container.DataItem("Ora")).ToString("HH:mm") %>' Visible="true">
														</asp:Label></td>
													<td width="48%">
														<asp:LinkButton id="lblPaziente" runat="server" Text='<%# Container.DataItem("Cognome") %>' CommandName="Nome" ToolTip="Mostra vaccinazioni programmate">
														</asp:LinkButton>
														<asp:Label id="lblCodPaziente" runat="server" Text='<%# Container.DataItem("CodicePazienteAusiliario")  %>' Visible="False">
														</asp:Label></td>
													<td width="15%">
														<asp:Label id="lblDataNascita" runat="server" Text='<%# CDate(Container.DataItem("DataNascita")).ToString("dd/MM/yyyy") %>'>
														</asp:Label></td>
													<td width="15%">
														<asp:Label id="lblRichiesta" runat="server" Text='<%# Container.DataItem("NumeroRichiesta") %>'>
														</asp:Label></td>
													<td width="10%">
														<asp:Label id="lblTipoRich" runat="server" Text='<%# Container.DataItem("TipoRichiesta") %>' Visible="true">
														</asp:Label></td>
													<td width="20"></td>
												</tr>
												<tr>
													<td></td>
													<td></td>
													<td colspan="7">
														<asp:DataGrid id="dgrRecord" style="table-layout: fixed" runat="server" CssClass="dgr2" AutoGenerateColumns="False">
															<AlternatingItemStyle CssClass="r1"></AlternatingItemStyle>
															<ItemStyle CssClass="r2"></ItemStyle>
															<HeaderStyle CssClass="h1"></HeaderStyle>
															<Columns>
																<asp:TemplateColumn HeaderText="Mnemonico Prestazione">
																	<HeaderStyle Width="15%"></HeaderStyle>
																	<ItemTemplate>
																		<asp:Label runat="server" Text='<%# Container.DataItem("PrestazioneMnemonico") %>' ID="Label5">
																		</asp:Label>
																	</ItemTemplate>
																</asp:TemplateColumn>
																<asp:TemplateColumn HeaderText="Descrizione Prestazione">
																	<HeaderStyle Width="40%"></HeaderStyle>
																	<ItemTemplate>
																		<asp:Label runat="server" Text='<%#Container.DataItem("PrestazioneDescrizione") %>' ID="Label6">
																		</asp:Label>
																	</ItemTemplate>
																</asp:TemplateColumn>
																<asp:TemplateColumn HeaderText="Mnemonico Profilo">
																	<HeaderStyle Width="15%"></HeaderStyle>
																	<ItemTemplate>
																		<asp:Label runat="server" Text='<%# Container.DataItem("ProfiloMnemonico") %>' ID="Label1">
																		</asp:Label>
																	</ItemTemplate>
																</asp:TemplateColumn>
																<asp:TemplateColumn HeaderText="Descrizione Profilo">
																	<HeaderStyle Width="30%"></HeaderStyle>
																	<ItemTemplate>
																		<asp:Label runat="server" Text='<%#Container.DataItem("ProfiloDescrizione") %>' ID="Label3">
																		</asp:Label>
																	</ItemTemplate>
																</asp:TemplateColumn>
															</Columns>
														</asp:DataGrid>
													</td>
													<td></td>
												</tr>
											</table>
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
							</asp:DataGrid>
                        </td>
					</tr>
				</table> 
                <!--<iframe id="modal" src="../../../../includes/formdettaglio/FinestraModaleDettaglio.aspx?cn=&SQL=SELECT CNS_DESCRIZIONE, CNS_CODICE FROM T_ANA_CONSULTORI" style="background-color:white;BORDER-RIGHT:2px outset;BORDER-TOP:2px outset;LEFT:50px;OVERFLOW:AUTO;BORDER-LEFT:2px outset;BORDER-BOTTOM:2px outset;POSITION:absolute;TOP:50px;width:400px;height:500px">
			        </iframe>
			        <div id="modal">
			        Immetti il nome e la password <input tye=text>
			        <div>
                -->
            </dyp:DynamicPanel>
        </on_lay3:OnitLayout3>
    </form>

    <script type="text/javascript" language="javascript">

        //<!--
        //createModalDialog('Prova.htm',400,500);
        //readParameter('');
        //closeModalDialog();
        //-->

        function OnitDataPick_ClickDay(id) {
            if (id == 'txtData' && (isValidFinestraModale('txtConsultorio', true))) {
                __doPostBack('Ricerca', '');

            }
        }

        function Espandi(dgrId, imgId, tableId, rowN) {
            var img = document.getElementById(imgId);
            var dgr = document.getElementById(dgrId);
            var tbl = document.getElementById(tableId);
            var stato = img.stato;

            if (stato == 'True') {
                if (dgr != null) dgr.style.display = 'none';
                if (tableId != null)
                    tbl.rows[rowN - 1].style.display = 'none';
                img.stato = 'False';
                img.src = '../../images/piu.gif';
            }
            else {
                if (dgr != null) dgr.style.display = 'block';
                if (tableId != null)
                    tbl.rows[rowN - 1].style.display = 'block';
                img.stato = 'True';
                img.src = '../../images/meno.gif';
            }
        }

        function EspandiTutto(espandi, dgrId, imgId, tableId, rowN) {

            var img = document.getElementById(imgId);
            var dgr = document.getElementById(dgrId);
            var tbl = document.getElementById(tableId);
            var stato = img.stato;

            if (!espandi) {
                if (dgr != null) dgr.style.display = 'none';
                if (tableId != null)
                    tbl.rows[rowN - 1].style.display = 'none';
                img.stato = 'False';
                img.src = '../../images/piu.gif';
            }
            else {
                if (dgr != null) dgr.style.display = 'block';
                if (tableId != null)
                    tbl.rows[rowN - 1].style.display = 'block';
                img.stato = 'True';
                img.src = '../../images/meno.gif';
            }
        }
    </script>

</body>

</html>
