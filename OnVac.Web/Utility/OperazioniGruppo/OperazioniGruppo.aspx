<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="OperazioniGruppo.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OperazioniGruppo" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<%@ Import Namespace="Onit.OnAssistnet.OnVac.OnVacUtility" %>
<%@ Import Namespace="Onit.OnAssistnet.OnVac" %>
<%@ Import Namespace="Onit.OnAssistnet.Web" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>Operazioni di Gruppo</title>
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/scripts/jqueryui/1.8.17/jquery-ui.min.js") %>"></script>
    <style type="text/css">
        .dgr {
            width: 100%;
        }

        .dgr2 {
            border: 1px solid gray;
            width: 70%;
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
            font-weight: bold;
            color: white;
            background-color: #4a3c8c;
        }

        .tdInt {
            font-weight: bold;
            color: #4a3c8c;
            background-color: lightsteelblue;
        }
    </style>
    <script type="text/javascript">
        var appId = "<%= OnVacContext.AppId %>";
        var azienda = "<%= OnVacContext.Azienda %>";

        function changedSelectedRow(obj) {

            if (document.getElementById('tableDisplay').innerHTML != "") {
                $("#tableDisplay").hide("drop");
            }

            var datiRichiesta = '{ "lotCodice":"' + obj.getAttribute('codicelotto') + '", "operazione":"' + obj.getAttribute('operazione') + '", "appID":"' + appId + '", "aziCodice":"' + azienda + '"}';
            $.ajax({
                type: "POST",
                url: "OperazioniGruppo.aspx/CaricaCampi",
                data: datiRichiesta,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: successOperazioniGruppo,
                error: errorOperazioniGruppo
            });
        }

        function successOperazioniGruppo(data, textStatus, jqXHR) {
            if (data != null && typeof (data) == "object") {
                $("#tableDisplay").show("drop", { direction: "right" });
                //document.getElementById('tableDisplay').innerHTML = data.d;
                $("#tableDisplay").html(data.d);
            } else {
                alert("Error. [3001] " + textStatus);
            }
        }

        function errorOperazioniGruppo(jqXHR, textStatus, errorThrown) {
            alert("Error. [3001] " + textStatus);
        }

        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;

            switch (button.Key) {
                case 'btnOpGrpCerca':

                    if ((OnitDataPickGet('odpOpGrpDaData') != "") && (OnitDataPickGet('odpOpGrpAData') != "")) {
                        if (!dateCompatibili('odpOpGrpDaData', 'odpOpGrpAData', '0')) {
                            alert("Attenzione: il campo 'Da Data' deve essere inferiore o uguale al campo 'A Data'");
                            evnt.needPostBack = false;
                            break;
                        }
                    }
                    else {
                        alert('Attenzione: è necessario valorizzare entrambe le date per effettuare la ricerca!');
                        evnt.needPostBack = false;
                        break;
                    }
                    break;

                case 'btnOpGrpStampa':
                    var dgr = document.getElementById('<%= dgrArgomento.ClientId %>');
                    var control = false;
                    if (dgr == null)
                        control = true;
                    else
                        if (dgr.rows.length == 0)
                            control = true;
                    if (control) {
                        alert('Attenzione: non è presente alcun elemento da stampare!');
                        evnt.needPostBack = false;
                        break;
                    }
                    break;
            }
        }

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

        function date2date() {

        }

    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout" runat="server" Height="100%" Width="70%" Titolo="Operazioni Gruppo" WindowNoFrames="False">
            <div>
				<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="tlbOperazioniGruppo" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover" ></HoverStyle>
                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
					<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
					<Items>
						<igtbar:TBarButton Key="btnOpGrpCerca" Text="Cerca" DisabledImage="../../images/cerca_dis.gif" Image="../../images/cerca.gif">
						</igtbar:TBarButton>
						<igtbar:TBarButton Key="btnOpGrpPulisci" Text="Pulisci Filtri" DisabledImage="../../images/pulisci_dis.gif" Image="../../images/pulisci.gif">
							<DefaultStyle Width="100px" CssClass="infratoolbar_button_default"></DefaultStyle>
						</igtbar:TBarButton>
						<%--
                        <igtbar:TBSeparator></igtbar:TBSeparator>
						<igtbar:TBarButton Key="btnOpGrpStampa" Text="Stampa" DisabledImage="../../images/stampa_dis.gif" Image="../../images/stampa.gif">
							<DefaultStyle Width="70px"></DefaultStyle>
						</igtbar:TBarButton>
                        --%>
					</Items>
				</igtbar:UltraWebToolbar>
            </div>
            <div class="sezione">Filtri di ricerca</div>
            <div>
				<table class="datagrid" id="Table1" cellspacing="2" cellpadding="0" width="100%" border="0">
					<colgroup>
                        <col width="100px" />
                        <col />
                        <col width="100px" />
                        <col />
                        <col width="5px" />
                    </colgroup>
                    <tr>
						<td class="label_right" style="font-weight: bold">Da data:</td>
						<td style="padding-left: 7px">
							<on_val:OnitDatePick id="odpOpGrpDaData" runat="server" CssClass="textbox_data_obbligatorio" DateBox="True"></on_val:OnitDatePick></td>
						<td class="label_right" style="font-weight: bold">A data:</td>
						<td style="padding-left: 7px">
							<on_val:OnitDatePick id="odpOpGrpAdata" runat="server" CssClass="textbox_data_obbligatorio" DateBox="True"></on_val:OnitDatePick></td>
						<td>&nbsp;</td>
					</tr>
					<tr>
						<td class="label" style="font-weight: bold">Argomento:</td>
						<td style="border-top: gainsboro 2px solid;" colspan="3">
							<asp:RadioButtonList id="rblArgomenti" runat="server" CssClass="label_left" RepeatDirection="Horizontal"
								Width="100%" RepeatColumns="5"></asp:RadioButtonList></td>
                        <td>&nbsp;</td>
					</tr>
					<tr>
						<td class="label" style="font-weight: bold">Operazioni:</td>
						<td style="border-top: gainsboro 2px solid;" colspan="3">
							<asp:CheckBoxList id="chkOperazioni" style="table-layout: fixed" runat="server" RepeatColumns="5"
								Width="100%" CssClass="label_left"></asp:CheckBoxList></td>
                        <td>&nbsp;</td>
					</tr>								
				</table>
            </div>
            <div>
				<table class="sezione" id="Table3" cellspacing="0" cellpadding="0" width="100%" border="0">
					<tr>
						<td>Legenda:</td>
						<td align="left">
							<p>
                                <img alt="" src="../../images/op_log.gif" align="absMiddle" /> 
                                &nbsp;Log generico <asp:Label id="lblLogGenerico" runat="server"></asp:Label>&nbsp;&nbsp; 
                                <img alt="" src="../../images/op_eliminazione.gif" align="absMiddle" />
								&nbsp;Eliminazione <asp:Label id="lblEliminazione" runat="server"></asp:Label>&nbsp;&nbsp; 
                                <img alt="" src="../../images/op_inserimento.gif" align="absMiddle" />
								&nbsp;Inserimento <asp:Label id="lblInserimento" runat="server"></asp:Label>&nbsp;&nbsp; 
                                <img alt="" src="../../images/op_modifica.gif" align="absMiddle" />
								&nbsp;Modifica <asp:Label id="lblModifica" runat="server"></asp:Label>&nbsp;&nbsp; 
                                <img alt="" src="../../images/op_eccezione.gif" align="absMiddle" />
								&nbsp;Eccezione <asp:Label id="lblEccezione" runat="server"></asp:Label>
                            </p>
						</td>
						<td style="border-bottom: 1px solid" align="right">
							<asp:CheckBox id="chkEspandi" onclick="EspandiComprimi(this.checked);" runat="server" Text="Espandi" AutoPostBack="False" Visible="False"></asp:CheckBox>
							<asp:CheckBox id="chkEsteso" runat="server" Text="Modalità estesa" AutoPostBack="True" Visible="False"></asp:CheckBox>
                        </td>
					</tr>
				</table>
            </div>

            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">

				<asp:DataGrid id="dgrArgomento" runat="server" CssClass="dgr" EnableViewState="False" AutoGenerateColumns="False" ShowHeader="False" GridLines="None">
					<AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
					<ItemStyle BackColor="#E7E7FF"></ItemStyle>
					<HeaderStyle Font-Bold="True" ForeColor="White" BackColor="#4A3C8C"></HeaderStyle>
					<Columns>
						<asp:TemplateColumn>
							<ItemTemplate>
								<table id="TableTestate" style="table-layout: fixed" cellspacing="0" cellpadding="0" width="100%" border="0" runat="server">
									<tr>
										<td class="cExp">
                                            <img id="imgEspandi1" style="cursor: hand" alt="" src='<%# IIf(chkEspandi.Checked, "../../images/meno.gif", "../../images/piu.gif") %>' runat="server" />&nbsp;
											<asp:Label id="lblArgomento" runat="server" Text='<%# Container.DataItem("LOT_ARGOMENTO").ToString %>'>
											</asp:Label>
											<asp:Label id="lblGruppo" runat="server" Text='<%# Container.DataItem("LOT_GRUPPO").ToString %>'>
											</asp:Label>
											<asp:Label id="Label1" runat="server" Text='<%# Container.DataItem("LOA_DESCRIZIONE").ToString %>'>
											</asp:Label>&nbsp;( Data Operazione:
											<asp:Label id="Label2" runat="server" Text='<%# CDate(Container.DataItem("DATA_OPERAZIONE")).ToString("d") %>'>
											</asp:Label>&nbsp;<%# iif(isDbNull(Container.DataItem("LOT_MASCHERA")),"",", Maschera:") %>
											&nbsp;
											<asp:Label id="lblMaschera" runat="server" Text='<%# Container.DataItem("LOT_MASCHERA") %>'>
											</asp:Label>&nbsp;)
                                        </td>
									</tr>
									<tr>
										<td>
											<table id="TableTestateInner" style="table-layout: fixed" cellspacing="0" cellpadding="0"
												width="100%" runat="server">
												<tr>
													<td width="20"></td>
													<td width="15"></td>
													<td width="20"></td>
													<td class="tdInt" width="30%">Cognome Nome</td>
													<td class="tdInt" width="15%">Data Nascita</td>
													<td class="tdInt" width="25%">Utente</td>
													<td class="tdInt" width="10%"><%# IIf(chkEsteso.Checked, "Data Completa", "") %></td>
													<td class="tdInt" width="25%"><%# IIf(chkEsteso.Checked, "Stack", "") %></td>
													<td width="20"></td>
													<td></td>
												</tr>
												<tr>
													<td colspan="10">
														<asp:DataGrid id="dgrTestate" runat="server" Width="100%" GridLines="None" ShowHeader="False" AutoGenerateColumns="False">
															<AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
															<ItemStyle BackColor="#E7E7FF"></ItemStyle>
															<HeaderStyle Font-Bold="True" ForeColor="White" BackColor="#4A3C8C"></HeaderStyle>
															<Columns>
																<asp:TemplateColumn>
																	<ItemTemplate>
																		<table id="TableInnerRecord" style="table-layout: fixed" cellspacing="0" cellpadding="0" width="100%" border="0">
																			<tr>
																				<td width="10">
																					<asp:Label id="lblCodice" style="display: none" runat="server" Text='<%# Container.DataItem("LOT_CODICE") %>'>
																					</asp:Label></td>
																				<td width="20"><img id="imgEspandi2" style="cursor: hand" alt="" src="../../images/sel.gif" onclick='changedSelectedRow(this)' codiceLotto='<%# Container.DataItem("LOT_CODICE") %>' operazione='<%# Container.DataItem("LOT_OPERAZIONE") %>'  runat="server" /></td>
																				<td width="10"></td>
																				<td width="20">
																					<asp:Image id="imgOperazione" runat="server" AlternateText='<%# Container.DataItem("LOT_OPERAZIONE").ToString() %>' ImageUrl='<%# "../../images/" &amp; RecuperaImmagineOperazione(Container.DataItem("LOT_OPERAZIONE"))%>'>
																					</asp:Image>&nbsp;</td>
																				<td width="30%">
																					<asp:Label id="lblPaziente" runat="server" Text='<%# Container.DataItem("PAZ_COGNOME").ToString() &amp; " " &amp; Container.DataItem("PAZ_NOME").ToString() %>'>
																					</asp:Label></td>
																				<td width="15%">
																					<asp:Label id="lblDataNascita" runat="server" Text='<%# CutTime(Container.DataItem("PAZ_DATA_NASCITA")) %>'>
																					</asp:Label></td>
																				<td width="25%">
																					<asp:Label id="lblUtente" runat="server" Text='<%# Container.DataItem("UTE_DESCRIZIONE") %>'>
																					</asp:Label></td>
																				<td width="10%">
																					<asp:Label id="lblOrario" runat="server" Text='<%# CDate(Container.DataItem("LOT_DATA_OPERAZIONE")).ToString("d") %>' Visible="<%# chkEsteso.Checked %>">
																					</asp:Label></td>
																				<td width="25%">
																					<asp:Label id="lblStack" runat="server" Text='<%# Container.DataItem("LOT_STACK") %>' Visible="<%# chkEsteso.Checked %>">
																					</asp:Label></td>
																				<td width="20">
																					<asp:Image id="imgAuto" runat="server" AlternateText='<%# iif(Container.DataItem("LOT_AUTOMATICO")="S","Automatico","Manuale")%>' ImageUrl='<%# iif(Container.DataItem("LOT_AUTOMATICO")="S","../../images/operazione_automatica.gif","../../images/operazione_manuale.gif")%>'>
																					</asp:Image>&nbsp;</td>
																				<td></td>
																			</tr>
																			<tr>
																				<td colspan="10"></td>
																			</tr>
																		</table>
																	</ItemTemplate>
																</asp:TemplateColumn>
															</Columns>
														</asp:DataGrid>
													</td>
												</tr>
											</table>
										</td>
									</tr>
									<tr>
                                        <td></td>
									</tr>
								</table>
							</ItemTemplate>
						</asp:TemplateColumn>
					</Columns>
				</asp:DataGrid>

			</dyp:DynamicPanel>

            <div class="sezione">Dettaglio operazione</div>
            
            <dyp:DynamicPanel ID="dypDettaglio" runat="server" Width="100%" Height="100px" ScrollBars="Auto" RememberScrollPosition="true">
                <div id="tableDisplay"></div>
				<table id="AjaxValues" style="display: none" runat="server">
					<tr>
						<td id="AjaxValue1"></td>
						<td id="AjaxValue2"></td>
						<td id="AjaxValue3"></td>
					</tr>
				</table>
            </dyp:DynamicPanel>

        </on_lay3:OnitLayout3>
    </form>
    <script type="text/javascript">
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
