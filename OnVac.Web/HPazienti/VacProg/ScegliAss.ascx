<%@ Control Language="vb" AutoEventWireup="false" Codebehind="ScegliAss.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.ScegliAss" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<div style="BORDER-RIGHT: black 1px solid; MARGIN-TOP: 2px; BORDER-LEFT: black 1px solid; BORDER-BOTTOM: black 1px solid; BACKGROUND-COLOR: #e7e7ff">
	<asp:label id="Label1" Font-Bold="True" CssClass="label_left" runat="server">&nbsp;Lotto:&nbsp;</asp:label>
	<asp:label id="lblLotto" Font-Bold="True" CssClass="label_left" runat="server"></asp:label>
	<asp:label id="lblNomeCommerciale" Font-Bold="True" CssClass="label_left" runat="server"></asp:label>
	<asp:radiobuttonlist id="rblAssociazioni" CssClass="label_left" runat="server" RepeatDirection="Horizontal"></asp:radiobuttonlist>
</div>
