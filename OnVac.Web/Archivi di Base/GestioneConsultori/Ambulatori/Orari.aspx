<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Orari.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_Orari" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
		<title>Disponibilita</title>

        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

		<script type="text/javascript" language="javascript" src="ScriptOrari.js" ></script>
		<script type="text/javascript" language="javascript">
			busy='<%response.write(onitlayout31.busy)%>';
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server" style="overflow:auto">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" width="100%" height="100%">
				<div class="Title" id="Div1" runat="server">
					<asp:Label id="LayoutTitolo" runat="server" CssClass="TITLE" Width="100%"></asp:Label>
                </div>
				<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                    <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
					<Items>
						<igtbar:TBarButton Key="btn_Salva" DisabledImage="~/Images/salva.gif" Text="Salva" Image="~/Images/salva.gif"></igtbar:TBarButton>
						<igtbar:TBarButton Key="btn_Annulla" DisabledImage="~/Images/annulla.gif" Text="Annulla" Image="~/Images/annulla.gif"></igtbar:TBarButton>
						<igtbar:TBarButton Key="btn_Modifica" DisabledImage="~/Images/modifica.gif" Text="Modifica" Image="~/Images/modifica.gif"></igtbar:TBarButton>
						<igtbar:TBSeparator></igtbar:TBSeparator>
						<igtbar:TBarButton Key="btn_Stampa" DisabledImage="~/Images/stampa.gif" Text="Stampa" Image="~/Images/stampa.gif"></igtbar:TBarButton>
					</Items>
				</igtbar:UltraWebToolbar>
				<div class="sezione" id="Div2" runat="server">
					<asp:Label id="LayoutTitolo_sezioneCnv" runat="server">TABELLA ORARI GIORNALIERI</asp:Label>
                </div>
				<asp:Panel id="pan_orari" runat="server" Width="100%">
					<table id="TABLE1" style="BORDER-RIGHT: 0px; BORDER-TOP: 0px; FONT-SIZE: 12px; BORDER-LEFT: 0px; COLOR: navy; BORDER-BOTTOM: 0px; FONT-FAMILY: Arial; BACKGROUND-COLOR: #ffffff"
						cellSpacing="0" width="100%" runat="server">
						<tr style="FONT-WEIGHT: bold; COLOR: #ffffff; BACKGROUND-COLOR: #4a3c8c" height="20">
							<td width="8%"></td>
							<td align="center" width="46%" colSpan="2">Mattino
							</td>
							<td align="center" width="46%" colSpan="2">Pomeriggio
							</td>
						</tr>
						<tr style="COLOR: #ffffff; BACKGROUND-COLOR: #4a3c8c" height="20">
							<td style="FONT-WEIGHT: bold" align="right" width="8%">Giorno
							</td>
							<td align="center" width="23%">Apertura
							</td>
							<td align="center" width="23%">Chiusura
							</td>
							<td align="center" width="23%">Apertura
							</td>
							<td align="center" width="23%">Chiusura
							</td>
						</tr>
						<tr style="BACKGROUND-COLOR: #e7e7ff" height="25">
							<td align="right" width="8%">Lunedì
							</td>
							<td align="center" width="23%">
								<asp:TextBox id="or1_0" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or1_1" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or1_2" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or1_3" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
						</tr>
						<tr style="BACKGROUND-COLOR: whitesmoke" height="25">
							<td align="right" width="8%">Martedì
							</td>
							<td align="center" width="23%">
								<asp:TextBox id="or2_0" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or2_1" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or2_2" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or2_3" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
						</tr>
						<tr style="BACKGROUND-COLOR: #e7e7ff" height="25">
							<td align="right" width="8%">Mercoledì
							</td>
							<td align="center" width="23%">
								<asp:TextBox id="or3_0" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or3_1" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or3_2" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or3_3" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
						</tr>
						<tr style="BACKGROUND-COLOR: whitesmoke" height="25">
							<td align="right" width="8%">Giovedì
							</td>
							<td align="center" width="23%">
								<asp:TextBox id="or4_0" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or4_1" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or4_2" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or4_3" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
						</tr>
						<tr style="BACKGROUND-COLOR: #e7e7ff" height="25">
							<td align="right" width="8%">Venerdì
							</td>
							<td align="center" width="23%">
								<asp:TextBox id="or5_0" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or5_1" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or5_2" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or5_3" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
						</tr>
						<tr style="BACKGROUND-COLOR: whitesmoke" height="25">
							<td align="right" width="8%">Sabato
							</td>
							<td align="center" width="23%">
								<asp:TextBox id="or6_0" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or6_1" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or6_2" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or6_3" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
						</tr>
						<tr style="BACKGROUND-COLOR: #e7e7ff" height="25">
							<td align="right" width="8%">Domenica
							</td>
							<td align="center" width="23%">
								<asp:TextBox id="or0_0" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or0_1" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or0_2" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or0_3" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
						</tr>
					</table>
				</asp:Panel>
				<div class="sezione" id="Div3" runat="server">
					<asp:Label id="LayoutTitolo_sezioneCnvApp" runat="server">TABELLA ORARI APPUNTAMENTI</asp:Label>
                </div>
				<asp:Panel id="pan_orari_app" runat="server" Width="100%">
					<table id="Table2" style="BORDER-RIGHT: 0px; BORDER-TOP: 0px; FONT-SIZE: 12px; BORDER-LEFT: 0px; COLOR: navy; BORDER-BOTTOM: 0px; FONT-FAMILY: Arial; BACKGROUND-COLOR: #ffffff"
						cellSpacing="0" width="100%" runat="server">
						<tr style="FONT-WEIGHT: bold; COLOR: #ffffff; BACKGROUND-COLOR: #4a3c8c" height="20">
							<td width="8%"></td>
							<td align="center" width="46%" colSpan="2">Mattino
							</td>
							<td align="center" width="46%" colSpan="2">Pomeriggio
							</td>
						</tr>
						<tr style="COLOR: #ffffff; BACKGROUND-COLOR: #4a3c8c" height="20">
							<td style="FONT-WEIGHT: bold" align="right" width="8%">Giorno
							</td>
							<td align="center" width="23%">Apertura
							</td>
							<td align="center" width="23%">Chiusura
							</td>
							<td align="center" width="23%">Apertura
							</td>
							<td align="center" width="23%">Chiusura
							</td>
						</tr>
						<tr style="BACKGROUND-COLOR: #e7e7ff" height="25">
							<td align="right" width="8%">Lunedì
							</td>
							<td align="center" width="23%">
								<asp:TextBox id="or1_0app" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or1_1app" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or1_2app" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or1_3app" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
						</tr>
						<tr style="BACKGROUND-COLOR: whitesmoke" height="25">
							<td align="right" width="8%">Martedì
							</td>
							<td align="center" width="23%">
								<asp:TextBox id="or2_0app" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or2_1app" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or2_2app" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or2_3app" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
						</tr>
						<tr style="BACKGROUND-COLOR: #e7e7ff" height="25">
							<td align="right" width="8%">Mercoledì
							</td>
							<td align="center" width="23%">
								<asp:TextBox id="or3_0app" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or3_1app" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or3_2app" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or3_3app" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
						</tr>
						<tr style="background-color: whitesmoke" height="25">
							<td align="right" width="8%">Giovedì
							</td>
							<td align="center" width="23%">
								<asp:TextBox id="or4_0app" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or4_1app" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or4_2app" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or4_3app" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
						</tr>
						<tr style="BACKGROUND-COLOR: #e7e7ff" height="25">
							<td align="right" width="8%">Venerdì
							</td>
							<td align="center" width="23%">
								<asp:TextBox id="or5_0app" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or5_1app" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or5_2app" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or5_3app" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
						</tr>
						<tr style="background-color: whitesmoke" height="25">
							<td align="right" width="8%">Sabato
							</td>
							<td align="center" width="23%">
								<asp:TextBox id="or6_0app" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or6_1app" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or6_2app" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or6_3app" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
						</tr>
						<tr style="background-color: #e7e7ff" height="25">
							<td align="right" width="8%">Domenica
							</td>
							<td align="center" width="23%">
								<asp:TextBox id="or0_0app" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or0_1app" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or0_2app" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
							<td align="center" width="23%">
								<asp:TextBox id="or0_3app" runat="server" CssClass="label_center" Width="80%"></asp:TextBox></td>
						</tr>
					</table>
					<asp:Label id="lb_warning" runat="server" Width="100%" Font-Names="arial" Font-Size="12px"
						BorderStyle="Solid" BackColor="#E7E7FF" Height="35px" BorderColor="Black" BorderWidth="1px"
						Visible="False" Font-Bold="True"></asp:Label>
				</asp:Panel>
			</on_lay3:onitlayout3>
		</form>
		<script type="text/javascript" language="javascript"> 
            <% Response.Write(strJS) %>
        </script>
	</body>
</html>
