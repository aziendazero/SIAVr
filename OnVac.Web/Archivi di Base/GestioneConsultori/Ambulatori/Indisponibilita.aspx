<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Indisponibilita.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_Indisponibilita" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>Indisponibilita' Ambulatorio</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/toolbar.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/default/toolbar.default.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    
    <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
    <script language="javascript" type="text/javascript">

        function ToolBarClick_OnClientButtonClicking(sender, args) {

            var button = args.get_item();
            switch (button.get_value()) {

                case 'btn_Annulla':
                    
                    args.set_cancel(true);

                    if ("<%Response.Write(OnitLayout31.Busy)%>" == "True") {

                        if (confirm("Le modifiche effettuate andranno perse. Continuare?")) {
                            window.location.href = "Ambulatori.aspx?RicaricaDati=True";
                        }
                    }
                    else {
                        window.location.href = "Ambulatori.aspx?RicaricaDati=True";
                    }
                    break;

                case 'btn_Salva':
                    if ("<%Response.Write(OnitLayout31.Busy)%>" != "True") args.set_cancel(true);
                    break;
            }
        }

        function controlla(evt) {
            el = SourceElement(evt);
            riga = el.parentNode.parentNode.parentNode;
            el = OnitDataPickGet("<%= tb_data_edit_ClientId %>");
            if (el == "") {
                alert("Il campo 'Data' è vuoto. Non è possibile aggiornare la tabella.")
                StopPreventDefault(evt);
                return false;
            }
            else {
                cb = riga.childNodes[4].firstChild;
                arTemp = new Array()
                arTemp = el.split("/")
                if (cb.checked == false && new Date(arTemp[2], arTemp[1], arTemp[0], 0, 0, 0) < new Date()) {
                    alert("Il campo 'data' contiene un giorno già passato. Non è possibile aggiornare la tabella.")
                    StopPreventDefault(evt);
                    return false;
                }
            }
            if ((!validaOrario('<%= tb_oraInizio_edit_ClientId %>', '.', false, ''))) {
                alert("L'orario di inizio non è valido o non è nel formato corretto (hh.mm). Non è possibile aggiornare la tabella.")
                document.getElementById('<%= tb_oraInizio_edit_ClientId %>').focus();
                StopPreventDefault(evt);
                return false;
            }
            if ((!validaOrario('<%= tb_oraFine_edit_ClientId %>', '.', false, ''))) {
                alert("L'orario di fine non è valido o non è nel formato corretto (hh.mm). Non è possibile aggiornare la tabella.")
                document.getElementById('<%= tb_oraFine_edit_ClientId %>').focus();
                StopPreventDefault(evt);
                return false;
            }

            el = SourceElement(evt);
            riga = el.parentNode.parentNode.parentNode;
            tab = GetElementByTag(riga, 'TABLE', 1, 0, false);
            cell = tab.rows[riga.rowIndex].cells[5];
            el = GetElementByTag(cell, 'INPUT', 1, 1, false);
            if (el.value.length > 30) {
                alert("Il campo 'descrizione' non può contenere più di 30 caratteri. Non è possibile aggiornare la tabella.")
                el.focus();
                StopPreventDefault(evt);
                return false;
            }
            cell1 = tab.rows[riga.rowIndex].cells[6];
            cell2 = tab.rows[riga.rowIndex].cells[7];
            el1 = GetElementByTag(cell1, 'INPUT', 1, 1, false);
            el2 = GetElementByTag(cell2, 'INPUT', 1, 1, false);
            if (el1.value == "") {
                el1.value = "0.00"
            }
            if (el2.value == "") {
                el2.value = "23.59"
            }
            ar1 = el1.value.split(".")
            ar2 = el2.value.split(".")
            if (new Date("March 1, 1900 " + ar1[0] + ":" + ar1[1]) >= new Date("March 1, 1900 " + ar2[0] + ":" + ar2[1])) {
                alert("I dati non sono corretti: l'ora iniziale deve essere inferiore a quella finale!")
                el1.focus();
                StopPreventDefault(evt);
                return false;
            }
            return true;
        }

    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Height="100%" Width="100%">
            <div class="title">
				<asp:Label id="LayoutTitolo" runat="server" Width="100%"></asp:Label>
            </div>
            <div>
                <telerik:RadToolBar ID="ToolBar" runat="server"  Width="100%" Skin="Default" EnableEmbeddedSkins="false" EnableAjaxSkinRendering="false" EnableEmbeddedBaseStylesheet="false" OnButtonClick="ToolBar_ButtonClick" OnClientButtonClicking="ToolBarClick_OnClientButtonClicking">
                    <Items>
                        <telerik:RadToolBarButton runat="server" value="btn_Salva" Text="Salva" ImageUrl="~/Images/salva.gif"></telerik:RadToolBarButton>
					    <telerik:RadToolBarButton runat="server" value="btn_Annulla" Text="Annulla" ImageUrl="~/Images/annulla.gif"></telerik:RadToolBarButton>
					    <telerik:RadToolBarButton runat="server" value="btn_Inserisci" Text="Inserisci" ImageUrl="~/Images/nuovo.gif"></telerik:RadToolBarButton>
                        <telerik:RadToolBarButton runat="server" value="btnIndisponibilitaPeriodiche" Text="Indisponibilità periodiche" ImageUrl="~/Images/calendario.gif"></telerik:RadToolBarButton>
				    </Items>
				</telerik:RadToolBar>
            </div>
			<div class="sezione">
				<asp:Label id="LayoutTitolo_sezioneCnv" runat="server">ELENCO INDISPONIBILITA'</asp:Label>
            </div>
            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                <asp:Panel id="pan_ind" runat="server" Width="100%">
					<asp:datagrid id="dg_indisp" runat="server" Width="100%" CssClass="datagrid" AutoGenerateColumns="False" CellPadding="1" GridLines="None" DataKeyField="key"  >
						<SelectedItemStyle Font-Bold="True" CssClass="Selected"></SelectedItemStyle>
						<EditItemStyle CssClass="edit"></EditItemStyle>
						<AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
						<ItemStyle CssClass="item"></ItemStyle>
						<HeaderStyle Font-Bold="True" CssClass="header"></HeaderStyle>
						<PagerStyle CssClass="pager" Mode="NumericPages"></PagerStyle>
						<Columns>
							<asp:ButtonColumn Text="&lt;img  title=&quot;Elimina&quot; src=&quot;../../../images/elimina.gif&quot; onclick=&quot;if(!confirm('La riga sar&#224; eliminata. Continuare?')){event.returnValue=false}&quot;&gt;" CommandName="Delete">
								<HeaderStyle HorizontalAlign="Center" Width="1%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center"></ItemStyle>
							</asp:ButtonColumn>
							<asp:EditCommandColumn ButtonType="LinkButton" UpdateText="&lt;img  title=&quot;Conferma&quot; src=&quot;../../../images/conferma.gif&quot; onclick=&quot;controlla(event)&quot;&gt;" CancelText="&lt;img  title=&quot;Annulla&quot; src=&quot;../../../images/annullaconf.gif&quot;&gt;" EditText="&lt;img  title=&quot;Modifica&quot; src=&quot;../../../images/modifica.gif&quot;  &gt;">
								<HeaderStyle HorizontalAlign="Center" Width="1%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center"></ItemStyle>
							</asp:EditCommandColumn>
							<asp:TemplateColumn>
								<HeaderStyle HorizontalAlign="Center" Width="16%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center"></ItemStyle>
								<HeaderTemplate>
									Data
								</HeaderTemplate>
								<ItemTemplate>
									<asp:Label id="tb_data" runat="server" Text='<%# formattaData(   DataBinder.Eval(Container, "DataItem")("fas_data")   ) %>'>
									</asp:Label>
								</ItemTemplate>
								<EditItemTemplate>
									<on_val:onitdatepick id="tb_data_edit" runat="server" Height="22px" CssClass="textbox_data" Text='<%# DataBinder.Eval(Container, "DataItem")("fas_data") %>' Focus="False" FormatoData="GeneralDate" DateBox="True" NoCalendario="False" Formatta="False" CalendarioPopUp="True" indice="-1" Hidden="False" ControlloTemporale="False">
									</on_val:onitdatepick>
								</EditItemTemplate>
							</asp:TemplateColumn>
                            <asp:TemplateColumn>
								<HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center"></ItemStyle>
								<HeaderTemplate>
									Festività
								</HeaderTemplate>
								<ItemTemplate>
									<asp:Image  id="imgFestivita" runat="server" Enabled="False" Visible='<%# DataBinder.Eval(Container, "DataItem")("festivita")%>' ImageUrl="~/Images/conferma.png"></asp:Image>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center"></ItemStyle>
								<HeaderTemplate>
									Ogni Anno
								</HeaderTemplate>
								<ItemTemplate>
									<asp:Image  id="imgRicorrente" runat="server" Enabled="False" Visible='<%# DataBinder.Eval(Container, "DataItem")("ric") %>' ImageUrl="~/Images/conferma.png"></asp:Image>
								</ItemTemplate>
								<EditItemTemplate>
									<asp:CheckBox id="cb_ric_edit" runat="server" Checked='<%# DataBinder.Eval(Container, "DataItem")("ric") %>'>
									</asp:CheckBox>
								</EditItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<HeaderStyle HorizontalAlign="Left" Width="30%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Left"></ItemStyle>
								<HeaderTemplate>
									Descrizione
								</HeaderTemplate>
								<ItemTemplate>
									<asp:Label id="tb_desc" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("fas_descrizione") %>'>
									</asp:Label>
								</ItemTemplate>
								<EditItemTemplate>
									<asp:TextBox id="tb_desc_edit" runat="server" CssClass="textbox_stringa" Text='<%# DataBinder.Eval(Container, "DataItem")("fas_descrizione") %>'>
									</asp:TextBox>
								</EditItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<HeaderStyle HorizontalAlign="Center" Width="16%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center"></ItemStyle>
								<HeaderTemplate>
									Ora Inizio
								</HeaderTemplate>
								<ItemTemplate>
									<asp:Label id="tb_orainizio" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("fas_ora_inizio") %>'>
									</asp:Label>
								</ItemTemplate>
								<EditItemTemplate>
									<asp:TextBox id="tb_orainizio_edit" runat="server" CssClass="textbox_data" Text='<%# DataBinder.Eval(Container, "DataItem")("fas_ora_inizio") %>'>
									</asp:TextBox>
								</EditItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<HeaderStyle HorizontalAlign="Center" Width="16%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center"></ItemStyle>
								<HeaderTemplate>
									Ora Fine
								</HeaderTemplate>
								<ItemTemplate>
									<asp:Label id="tb_orafine" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("fas_ora_fine") %>'>
									</asp:Label>
								</ItemTemplate>
								<EditItemTemplate>
									<asp:TextBox id="tb_orafine_edit" runat="server" CssClass="textbox_data" Text='<%# DataBinder.Eval(Container, "DataItem")("fas_ora_fine") %>'>
									</asp:TextBox>
								</EditItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<HeaderStyle Width="10%"></HeaderStyle>
							</asp:TemplateColumn>
						</Columns>
					</asp:datagrid>
				</asp:Panel>
                <div style="border: #8080ff 1px solid; background-color:lemonchiffon; padding: 2px;">
                    <asp:Label id="lb_warning" runat="server" CssClass="label" Font-Bold="True" />
                </div>
            </dyp:DynamicPanel>
        </on_lay3:OnitLayout3>

        <on_ofm:OnitFinestraModale ID="fmIndisponibilitaPeriodiche" Title="Indisponibilità periodiche" runat="server"
            Width="400px"  BackColor="LightGray" NoRenderX="true" RenderModalNotVisible="false">
            <telerik:RadToolBar ID="TolbarIndisponibilitaPeriodiche" runat="server" Width="100%" Skin="Default" EnableEmbeddedSkins="false" EnableAjaxSkinRendering="false" EnableEmbeddedBaseStylesheet="false" OnButtonClick="TolbarIndisponibilitaPeriodiche_ButtonClick">
                <Items>
                    <telerik:RadToolBarButton runat="server" Value="btnConferma" Text="Conferma" ImageUrl="~/Images/conferma.gif" />
                    <telerik:RadToolBarButton runat="server" Value="btnAnnulla" Text="Annulla" ImageUrl="~/Images/annulla.gif" />
                </Items>
            </telerik:RadToolBar>
            <table style="width: 100%">
                <colgroup>
                    <col align="right" />
                    <col />
                    <col />
                    <col />
                    <col />
                </colgroup>
                <tr>
                    <td>
                        <asp:Label ID="Label1" runat="server" CssClass="label" Text="Periodo"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="Label2" runat="server" CssClass="label" Text="da"></asp:Label>
                    </td>
                    <td>
                        <on_val:OnitDatePick ID="odpDataInizio" runat="server" Height="22px" CssClass="textbox_data_obbligatorio" Text="" Focus="False" FormatoData="GeneralDate" DateBox="True" NoCalendario="False" Formatta="False" CalendarioPopUp="True" indice="-1" Hidden="False" ControlloTemporale="False"></on_val:OnitDatePick>
                    </td>
                    <td>
                        <asp:Label ID="Label5" runat="server" CssClass="label" Text="a"></asp:Label>
                    </td>
                    <td>
                        <on_val:OnitDatePick ID="odpDataFine" runat="server" Height="22px" CssClass="textbox_data_obbligatorio" Text="" Focus="False" FormatoData="GeneralDate" DateBox="True" NoCalendario="False" Formatta="False" CalendarioPopUp="True" indice="-1" Hidden="False" ControlloTemporale="False"></on_val:OnitDatePick>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label7" runat="server" CssClass="label" Text="Ora"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="Label6" runat="server" CssClass="label" Text="da"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtOraInizio" runat="server" CssClass="textbox_stringa" Text=""></asp:TextBox>
                    </td>
                    <td>
                        <asp:Label ID="Label8" runat="server" CssClass="label" Text="a"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtOraFine" runat="server" CssClass="textbox_stringa" Text=""></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="Label3" runat="server" CssClass="label" Text="Motivo"></asp:Label>
                    <td colspan="3">
                        <asp:TextBox ID="txtMotivo" runat="server" Width="100%" CssClass="textbox_stringa" Text=""></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="vertical-align: top;" colspan="2">
                        <asp:Label ID="Label4" runat="server" CssClass="label" Text="Giorni"></asp:Label>
                    <td colspan="3">
                        <onit:CheckBoxList ID="chkGioniSettimana" runat="server" CssClass="label_left" RepeatDirection="Vertical"
                            TextAlign="Right" RepeatColumns="1">
                            <asp:ListItem Value="1">Luned&#236;</asp:ListItem>
                            <asp:ListItem Value="2">Marted&#236;</asp:ListItem>
                            <asp:ListItem Value="3">Mercoled&#236;</asp:ListItem>
                            <asp:ListItem Value="4">Gioved&#236;</asp:ListItem>
                            <asp:ListItem Value="5">Venerd&#236;</asp:ListItem>
                            <asp:ListItem Value="6">Sabato</asp:ListItem>
                            <asp:ListItem Value="0">Domenica</asp:ListItem>
                        </onit:CheckBoxList>
                    </td>
                </tr>
            </table>
        </on_ofm:OnitFinestraModale>
    </form>

    <%Response.Write(strJS)%>

    <script type="text/javascript">
        /*********funzione di validazione orario***********/
        /*              05/02/2004 (samu)                 */
        /*                                                */
        /* obj(String) --> nome dell'oggetto che contie-  */
        /*                 ne l'orario da validare;       */
        /* sep(String) --> carattere di separazione       */
        /*                 HH[sep]MM[sep]SS               */
        /*                 (se stringa vuota formato di   */
        /*                 input);                         */
        /* focus(boolean) --> 'true' focus                */
        /*                    'false' no focus;           */
        /* msg (String) --> messaggio di errore: se strin-*/
        /*                  ga vuota non visualizzato;    */
        /**************************************************/

        function validaOrario(obj, sep, focus, msg) {
            var ora = document.getElementById(obj).value;
            if (ora == "") return true;
            var cont = -1;
            var ok = false;
            // controllo della presenza dei caratteri ':' e '.'
            if (ora.search(sep) != cont) {
                splOra = ora.split(sep);
                var ok = true;
                if (splOra[1] == null) {
                    var splOra = new Array();
                    splOra[0] = ora;
                    splOra[1] = "00";
                    splOra[2] = "00";
                    var ok = true;
                }
            }
            var lung = splOra.length;
            ora0 = "" + splOra[0];
            ora1 = "" + splOra[1];
            ora2 = "" + splOra[2];
            // validazione dell'orario
            if (ok) {
                if ((isNaN(splOra[0])) || (isNaN(splOra[1]))) {
                    if (msg != "") alert(msg);
                    return false;
                }
                else
                    if (splOra.length == 3)
                        if (isNaN(splOra[2])) {
                            if (msg != "") alert(msg);
                            return false;
                        }
                if ((splOra.length == 3) || (splOra.length == 2)) {
                    if ((splOra[0].length > 2) || (splOra[1].length > 2) || (splOra[0].length < 0) || (splOra[1].length < 0)) {
                        if (msg != "") alert(msg);
                        return false;
                    }
                    else
                        if (splOra.length == 3)
                            if ((splOra[2].length) > 2 || (splOra[2].length) < 0) {
                                if (msg != "") alert(msg);
                                return false;
                            }
                    splOra[0] = parseInt(splOra[0]);
                    splOra[1] = parseInt(splOra[1]);

                    if ((splOra[0] < 0) || (splOra[0] > 23)) {
                        if (msg != "") alert(msg);
                        return false;
                    }
                    if ((splOra[1] < 0) || (splOra[1] > 59)) {
                        if (msg != "") alert(msg);
                        return false;
                    }
                    if (splOra.length == 3) {
                        splOra[2] = parseInt(splOra[2]);
                        if ((splOra[2] < 0) || (splOra[2] > 59)) {
                            if (msg != "") alert(msg);
                            return false;
                        }
                    }
                    // formato dell'orario			
                    if (sep != "") {
                        // if ((ora0.length)==1) ora0="0"+ora0; 
                        if ((ora1.length) == 1) ora1 = "0" + ora1;
                        nuovaOra = ora0 + sep + ora1;
                        document.getElementById(obj).value = nuovaOra;
                    }
                    // focus dell'elemento
                    if (focus == true) document.getElementById(obj).focus();
                    return true;
                }
                if (msg != "") alert(msg);
                return false;
            }
            else return false;
        }
    </script>
</body>
</html>
