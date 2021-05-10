<%@ Control Language="vb" AutoEventWireup="false" Codebehind="uscFiltriStampaEtichetteMovAusl.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.uscFiltriStampaEtichetteMovAusl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table border="0" cellpadding="2" cellspacing="0" width="100%">
	<tr height="5">
		<td colspan="3"></td>
	</tr>
	<tr>
		<td colspan="3" class="textbox_data" align="center"><B>Tipologie di stampa delle 
				etichette:</B></td>
	</tr>
	<tr height="10">
		<td colspan="3"></td>
	</tr>
	<tr>
		<td runat="server" id="primoRadioButton">
			<asp:RadioButtonList id="rdbTipoStampa" runat="server" Width="100%" BorderWidth="1px" BackColor="#E7E7FF"
				BorderStyle="Solid" BorderColor="Navy" CssClass="label_left">
				<asp:ListItem Value="E" Selected="True">Esterne</asp:ListItem>
				<asp:ListItem Value="I">Interne</asp:ListItem>
				<asp:ListItem Value="C">Comune</asp:ListItem>
			</asp:RadioButtonList>
		</td>
		<td runat="server" id="spaziatura"></td>
		<td runat="server" id="secondoRadioButton">
			<asp:RadioButtonList id="rdbTipoReport" runat="server" Width="100%" BorderWidth="1px" BackColor="#E7E7FF"
				BorderStyle="Solid" BorderColor="Navy" CssClass="label_left">
				<asp:ListItem Value="N" Selected="True">Non ancora stampate</asp:ListItem>
				<asp:ListItem Value="R">Ristampa</asp:ListItem>
				<asp:ListItem Value="T">Tutte</asp:ListItem>
			</asp:RadioButtonList>
		</td>
	</tr>
	<tr height="5">
		<td colspan="3"></td>
	</tr>
</table>
