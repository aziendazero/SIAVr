<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="FiltroRicercaImmediata.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.FiltroRicercaImmediata" %>
<table width="100%" border="0" cellpadding="0" cellspacing="0" >
    <colgroup>
        <col width="90%" />
        <col width="10%" />
    </colgroup>
    <tr>
        <td>
            <asp:TextBox ID="txtFiltro" runat="server" CssClass="textbox_stringa" Width="100%"></asp:TextBox>
        </td>
        <td align="center"  style="text-align: center; vertical-align: middle; cursor: pointer;" title="Cerca" onclick="document.getElementById('<%= Me.btnRicerca.ClientId %>').click();"  >
            <asp:ImageButton ID="btnRicerca" runat="server" AlternateText="Cerca" ImageAlign="AbsMiddle" />
        </td>
    </tr>
</table>
