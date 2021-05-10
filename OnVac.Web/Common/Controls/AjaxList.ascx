<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="AjaxList.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.Common.Controls.AjaxList"
    TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
<div style=" padding-bottom:3px" >
    <fieldset class="fldroot_clone"   >
        <legend class="label" style="padding-right: 10px; padding-left: 10px; font-weight: bold;
            padding-top: 1px;" >
            <%= Label %></legend>
        <div style="margin-left: 5px; margin-top: 2px; margin-bottom: 5px;">
            <asp:TextBox ID="parentDesc" runat="server" CssClass="TextBox_Stringa" Width="70%" style='text-transform:uppercase'></asp:TextBox>
            <asp:TextBox ID="parentCod" runat="server" CssClass="TextBox_Stringa" Width="20%" style='text-transform:uppercase'></asp:TextBox>
            <img ID="btnImgPulisciFiltri" runat="server" src="~/Images/pulisci.gif"
                width="16" alt="Pulisce i filtri" style="cursor: hand;"  />
        </div>
    </fieldset>
</div>
<div style="height: 20px" >
    <table cellspacing="0px" cellpadding="2px" width="100%" border="0">
        <tr class="header">
            <td width="10%">
                &nbsp;
            </td>
            <td width="70%">
                DESCRIZIONE
            </td>
            <td width="20%">
                CODICE
            </td>
        </tr>
    </table>
</div>
<asp:Panel ID="Display" Style="overflow: auto;height: 120px; " runat="server">
    <!-- Qui viene riempito da metodo ajax -->
</asp:Panel>
