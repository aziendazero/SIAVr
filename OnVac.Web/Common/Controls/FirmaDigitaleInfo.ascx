<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="FirmaDigitaleInfo.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.FirmaDigitaleInfo" %>

<table style="border-style:none; width:100%; background-color: #fbf9f9;">
    <colgroup>
        <col width="27%" />
        <col width="20%" />
        <col width="29%" />
        <col width="22%" />
        <col width="2%" />
    </colgroup>
    <tr>
        <td colspan="5">
            <div class="infoLabelCenterBold" style="margin-top:10px; margin-bottom:10px;">
                <asp:Label ID="lblDescrizioneStato" runat="server"></asp:Label>
            </div>
        </td>
    </tr>
    <tr>
        <td class="infoLabelRight">Data firma&nbsp;</td>
        <td>
            <div class="infoDati">
                <asp:Label ID="lblDataFirma" runat="server"></asp:Label>
            </div>
        </td>
        <td class="infoLabelRight">Utente firma&nbsp;</td>
        <td>
            <div class="infoDati">
                <asp:Label ID="lblUtenteFirma" runat="server"></asp:Label>
            </div>
        </td>
        <td></td>
    </tr>
    <tr>
        <td class="infoLabelRight">Data archiviazione&nbsp;</td>
        <td>
            <div class="infoDati">
                <asp:Label ID="lblDataArchiviazione" runat="server"></asp:Label>
            </div>
        </td>
        <td class="infoLabelRight">Utente archiviazione&nbsp;</td>
        <td>
            <div class="infoDati">
                <asp:Label ID="lblUtenteArchiviazione" runat="server"></asp:Label>
            </div>
        </td>
        <td></td>
    </tr>
    <tr>
        <td class="infoLabelRight">Token archiviazione&nbsp;</td>
        <td colspan="3">
            <div class="infoDatiCenterBold">
                <asp:Label ID="lblTokenArchiviazione" runat="server"></asp:Label>
            </div>
        </td>
        <td></td>
    </tr>
</table>