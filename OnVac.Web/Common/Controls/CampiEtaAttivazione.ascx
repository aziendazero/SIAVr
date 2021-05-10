<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="CampiEtaAttivazione.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.CampiEtaAttivazione" %>
<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>

<table style="width: 100%;">
    <colgroup>
        <col style="width: 11%" />
        <col style="width: 14%" />
        <col style="width: 11%" />
        <col style="width: 14%" />
        <col style="width: 11%" />
        <col style="width: 14%" />
        <col />
    </colgroup>
    <tr>
        <td>
            <on_val:OnitJsValidator id="txtEtaAnni" runat="server" Width="100%" CssClass="textbox_numerico"
                actionCorrect="False" actionDelete="False" actionFocus="False" actionMessage="True" actionSelect="True"
                actionUndo="False" autoFormat="False" validationType="Validate_integer" PreParams-numDecDigit="0" PreParams-maxValue="199"
                PreParams-minValue="0" MaxLength="3"></on_val:OnitJsValidator></td>
        <td>
            <asp:Label ID="lblEtaAnni" runat="server" Text="anni"></asp:Label></td>
        <td>
            <on_val:OnitJsValidator id="txtEtaMesi" runat="server" Width="100%" CssClass="textbox_numerico"
                actionCorrect="False" actionDelete="False" actionFocus="False" actionMessage="True" actionSelect="True"
                actionUndo="False" autoFormat="False" validationType="Validate_integer" PreParams-numDecDigit="0" PreParams-maxValue="999"
                PreParams-minValue="0" MaxLength="3"></on_val:OnitJsValidator></td>
        <td>
            <asp:Label ID="lblEtaMesi" runat="server" Text="mesi"></asp:Label></td>
        <td>
            <on_val:OnitJsValidator id="txtEtaGiorni" runat="server" Width="100%" CssClass="textbox_numerico"
                actionCorrect="False" actionDelete="False" actionFocus="False" actionMessage="True" actionSelect="True"
                actionUndo="False" autoFormat="False" validationType="Validate_integer" PreParams-numDecDigit="0" PreParams-maxValue="999"
                PreParams-minValue="0" MaxLength="3"></on_val:OnitJsValidator></td>
        <td>
            <asp:Label ID="lblEtaGiorni" runat="server" Text="giorni"></asp:Label></td>
        <td></td>
    </tr>
</table>