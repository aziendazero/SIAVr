<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="FiltriRicercaMagazzino.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.FiltriRicercaMagazzino" %>

<table class="datagrid" width="100%" cellpadding="2" cellspacing="0" width="100%" border="0">
    <colgroup>
        <col style="width: 130px" />
        <col />
        <col style="width: 150px" />
        <col />
        <col style="width: 110px" />
        <col style="width: 135px" />
    </colgroup>
    <tr>
        <td>Codice Lotto</td>
        <td>
            <asp:TextBox id="txtCodiceLotto" onblur="stringToUpperCase(this)" runat="server" CssClass="textbox_stringa" Width="95%" MaxLength="12"></asp:TextBox>
        </td>
        <td>Descrizione Lotto</td>
        <td>
            <asp:TextBox id="txtDescrizioneLotto" onblur="stringToUpperCase(this)" runat="server" CssClass="textbox_stringa" Width="95%" MaxLength="25"></asp:TextBox>			            
        </td>
        <td colspan="2">
            <asp:CheckBox id="chkLottiSequestrati" runat="server" CssClass="label" Text="Solo lotti sequestrati" ToolTip="Visualizza solo i lotti sequestrati"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td title="Codice Nome Commerciale">Codice Nome Comm.</td>
        <td>
            <asp:TextBox id="txtCodiceNomeCommerciale" onblur="stringToUpperCase(this)" runat="server" CssClass="textbox_stringa" Width="95%" MaxLength="12"></asp:TextBox>
        </td>
        <td title="Descrizione Nome Commerciale">Descrizione Nome Comm.</td>
        <td>
            <asp:TextBox id="txtDescrizioneNomeCommerciale" onblur="stringToUpperCase(this)" runat="server" CssClass="textbox_stringa" Width="95%" MaxLength="25"></asp:TextBox>
        </td>
        <td>
            <asp:CheckBox id="chkLottiScaduti" runat="server" CssClass="label" Text="No lotti scaduti" ToolTip="Non visualizza i lotti scaduti"></asp:CheckBox>
        </td>
        <td>
            <asp:CheckBox id="chkLottiScortaNulla" runat="server" CssClass="label" Text="No lotti scorta nulla" ToolTip="Non visualizza i lotti a scorta nulla"></asp:CheckBox>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label runat="server" ID="lblDistrietti" CssClass="Label" Text="Distretti"></asp:Label>
        </td>
        <td>
            <asp:DropDownList ID="ddlDistretti" runat="server" CssClass="textbox_stringa" Width="95%">
            </asp:DropDownList>
        </td>
    </tr>
</table>

<script type="text/javascript">

    var txt = document.getElementById('<%= txtCodiceLotto.ClientID %>');
    if (txt != null) txt.focus();
    
</script>