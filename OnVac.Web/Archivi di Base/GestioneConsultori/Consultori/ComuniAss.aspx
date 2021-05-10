<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ComuniAss.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_ComuniAss" %>

<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>OnVac</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript" language="javascript">
        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;
            //alert(button.Key);
            switch (button.Key) {
                case 'btn_Annulla':
                    if ("<%response.write(onitlayout31.busy)%>" == "True") {
			        var ret;
			        ret = confirm("Le modifiche effettuate andranno perse. Continuare?");
			        if (ret == true) {
			            window.location.href = "Consultori.aspx?RicaricaDati=True";
			            evnt.needPostBack = false;
			        }
			    }
			    else
			        window.location.href = "Consultori.aspx?RicaricaDati=True";
			    evnt.needPostBack = false;
			    break;
            case 'btn_Salva':
                if ("<%response.write(onitlayout31.busy)%>" != "True")
			        evnt.needPostBack = false;

            }
        }

        function controlla(evt) {
            el = SourceElement(evt);
            riga = el.parentNode.parentNode.parentNode;
            tab = GetElementByTag(riga, 'TABLE', 1, 0, false);
            cell = tab.rows[riga.rowIndex].cells[1];
            el = GetElementByTag(cell, 'INPUT', 1, 1, false);
            if (el.value == "") {
                alert('I campi Descrizione-Codice sono vuoti.Non è possibile aggiornare la tabella.');
                el.focus();
                StopPreventDefault(evt);
            }
        }

        function conferma(evt) {
            if (!(confirm('La riga sarà eliminata. Continuare?'))) {
                StopPreventDefault(evt);
            }
        }
    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Height="100%" Width="100%" Titolo="Consultori-Comuni" TitleCssClass="Title3">
            <div class="title" id="divLayout">
				<asp:Label id="LayoutTitolo" runat="server" CssClass="Title" Width="100%"></asp:Label>
            </div>
            <div>
				<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
					<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
					<Items>
						<igtbar:TBarButton Key="btn_Salva" Text="Salva" Image="~/Images/salva.gif"></igtbar:TBarButton>
						<igtbar:TBarButton Key="btn_Annulla" Text="Annulla" Image="~/Images/annulla.gif"></igtbar:TBarButton>
						<igtbar:TBarButton Key="btn_Inserisci" Text="Inserisci" Image="~/Images/nuovo.gif"></igtbar:TBarButton>
					</Items>
				</igtbar:UltraWebToolbar>
            </div>
			<div class="sezione" id="divLayoutTitolo">
				<asp:Label id="LayoutTitolo_sezioneCnv" runat="server">ELENCO COMUNI</asp:Label>
            </div>
            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
				<asp:Panel id="pan_com" runat="server" Width="100%">
					<asp:datagrid id="dg_comuni" runat="server" CssClass="datagrid" Width="100%" Height="0px" AutoGenerateColumns="False" CellPadding="1" GridLines="None">
						<SelectedItemStyle Font-Bold="True" CssClass="selected"></SelectedItemStyle>
						<AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
						<ItemStyle CssClass="item"></ItemStyle>
						<HeaderStyle Font-Bold="True" CssClass="header"></HeaderStyle>
						<Columns>
							<asp:EditCommandColumn ButtonType="LinkButton" UpdateText="&lt;img  title=&quot;Conferma&quot; src=&quot;../../../images/conferma.gif&quot; onclick=&quot;controlla(event)&quot;&gt;" CancelText="&lt;img  title=&quot;Annulla&quot; src=&quot;../../../images/annullaconf.gif&quot;&gt;" EditText="&lt;img  title=&quot;Elimina&quot; src=&quot;../../../images/elimina.gif&quot;  onclick=&quot;conferma(event)&quot;&gt;">
								<HeaderStyle Width="1%"></HeaderStyle>
							</asp:EditCommandColumn>
							<asp:TemplateColumn>
								<HeaderStyle HorizontalAlign="Center" Width="49%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Left"></ItemStyle>
								<HeaderTemplate>
									<table cellspacing="0" cellpadding="0" width="100%" border="0">
										<TR>
											<td width="70%" align="center">
												Descrizione
											</td>
											<td width="30%" align="center">
												Codice
											</td>
										</tr>
									</table>
								</HeaderTemplate>
								<ItemTemplate>
									<table cellspacing="0" cellpadding="0" width="100%" border="0">
										<tr>
											<td width="70%">
												<asp:Label id="tb_descCom" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("com_descrizione") %>'>
												</asp:Label></td>
											<td width="30%">
												<asp:Label id="tb_codCom" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("cco_com_codice") %>'>
												</asp:Label></td>
										</tr>
									</table>
								</ItemTemplate>
								<EditItemTemplate>
									<on_ofm:onitmodallist id="fm_com" runat="server" Width="70%" Tabella="t_ana_comuni" Connection="" CampoDescrizione="com_descrizione" CampoCodice="com_codice " Obbligatorio="True" UseCode="True" CodiceWidth="30%" LabelWidth="-1px" PosizionamentoFacile="False" SetUpperCase="True" RaiseChangeEvent="False"></on_ofm:onitmodallist>
								</EditItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<HeaderStyle Width="50%"></HeaderStyle>
							</asp:TemplateColumn>
						</Columns>
						<PagerStyle CssClass="pager" Mode="NumericPages"></PagerStyle>
					</asp:datagrid>
					<asp:Label id="lb_warning" style="padding: 5px" runat="server" Width="100%" Font-Size="12px" Font-Names="arial" BorderStyle="Solid" BackColor="#E7E7FF" Height="35px" Visible="False" BorderColor="Black" BorderWidth="1px" Font-Bold="True"></asp:Label>
				</asp:Panel>
            </dyp:DynamicPanel>
        </on_lay3:OnitLayout3>
    </form>
    <%response.write(strJS)%>
</body>
</html>
