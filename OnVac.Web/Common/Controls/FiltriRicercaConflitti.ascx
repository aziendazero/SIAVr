<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="FiltriRicercaConflitti.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.FiltriRicercaConflitti" %>

<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>

<div style="width: 100%; background-color: #F5F5F5; padding: 5px;">
	<fieldset class="vacFieldset" title="Filtri di ricerca">
		<legend style="width:100px; text-align: center; font-family: Arial; font-size: 12px;">Filtri di ricerca</legend>
        <table style="width: 100%; background-color: #F5F5F5" border="0">
            <colgroup>
                <col style="width: 15%" />
                <col style="width: 3%" />
                <col style="width: 35%" />
                <col style="width: 3%" />
                <col style="width: 35%" />
                <col style="width: 9%" />
            </colgroup>
	        <tr>
		        <td class="label">
                    <asp:Label ID="lblCognome" runat="server" Text="Cognome"></asp:Label>
                </td>
                <td></td>
                <td colspan="3">
                    <asp:TextBox ID="txtCognome" runat="server" CssClass="textbox_stringa" Width="100%"></asp:TextBox>
                </td>
                <td></td>
	        </tr>
	        <tr>
		        <td class="label">
                    <asp:Label ID="lblNome" runat="server" Text="Nome"></asp:Label>
                </td>
                <td></td>
                <td colspan="3">
                    <asp:TextBox ID="txtNome" runat="server" CssClass="textbox_stringa" Width="100%"></asp:TextBox>
                </td>
                <td></td>
	        </tr>
	        <tr>
		        <td class="label">
                    <asp:Label ID="lblDataNascita" runat="server" Text="Data Nascita"></asp:Label>
                </td>
                <td class="label">
                    <asp:Label ID="lblDa" runat="server" Text="Da"></asp:Label>
                </td>
                <td>
                    <on_val:onitdatepick ID="dpkDataNascitaDa" runat="server" CssClass="textbox_data" DateBox="True" Height="20px" Width="120px"></on_val:onitdatepick>
                </td>
                <td class="label">
                    <asp:Label ID="lblA" runat="server" Text="A"></asp:Label>
                </td>
                <td>
                    <on_val:onitdatepick ID="dpkDataNascitaA" runat="server" CssClass="textbox_data" DateBox="True" Height="20px" Width="120px"></on_val:onitdatepick>
                </td>
                <td></td>
	        </tr>
        </table>
    </fieldset>
</div>
