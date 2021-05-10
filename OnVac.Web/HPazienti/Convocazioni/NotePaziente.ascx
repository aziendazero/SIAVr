<%@ Control Language="vb" AutoEventWireup="false" Codebehind="NotePaziente.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.NotePaziente"%>
<link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
<div id="ReazAvverse" runat="server" Visible="False" style="BACKGROUND-COLOR:LemonChiffon;font-family:Tahoma">
	<table width="100%">
		<tr>
			<td width="10%" align="center">
				<IMG alt="" src="../../images/warning.gif">
			</td>
			<td width="90%">
				<b>Attenzione, sono presenti delle Reazioni Avverse</b>
			</td>
		</tr>
	</table>
</div>
<table width="100%" border="0">
    <asp:Repeater ID="rptNote" runat="server">
        <ItemTemplate>
            <tr>
                <td class="textbox_data" vAlign="middle" width="10%" height="70">
                    <asp:Label ID="lblDescrTipoNote" runat="server" Width="100%" CssClass="label" Font-Bold="True"><%# Eval("DescrizioneNota")%></asp:Label>
                </td>
                <td class="textbox_data" vAlign="bottom" width="90%" height="70">
                    <asp:TextBox id="txtTestoNote" onblur="toUpper(this)" style="overflow-y: auto" runat="server" Width="100%" CssClass="textbox_stringa_disabilitato" 
                        TextMode="MultiLine" Height="100%" ReadOnly="True" Text='<%# Eval("TestoNota") %>'></asp:TextBox>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
</table>
