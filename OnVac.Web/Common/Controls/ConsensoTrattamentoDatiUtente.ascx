<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ConsensoTrattamentoDatiUtente.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.ConsensoTrattamentoDatiUtente" %>
<style type="text/css">
    .consenso-contenitore {
        width: 100%;
        padding: 2px;
    }

    .consenso-testo {
        width: 99%;
        height: 130px;
        text-align: center;
        font-family: Calibri;
        font-size: 16px;
        padding: 10px 2px 20px 2px;
    }

    .consenso-pulsante {
        cursor: pointer;
        width: 150px;
        height: 40px;
        vertical-align: middle;
        text-align: center;
        font-family: Calibri;
        font-size: 16px;
        border: 1px solid #058;
        border-radius: 4px;
        background-color: steelblue;
        color: white;
    }

        .consenso-pulsante:hover {
            background-color: lightskyblue;
            color: #058;
        }
</style>
<div class="consenso-contenitore">
    <table style="width: 100%; border: 0px;" cellpadding="0" cellspacing="0">
        <colgroup>
            <col style="width: 50%" />
            <col style="width: 50%" />
        </colgroup>
        <tr>
            <td colspan="2">
                <div class="consenso-testo">Sta accedendo a categorie particolari di dati personali di un soggetto per il quale ha l'autorizzazione da parte del titolare istituzionale al trattamento per finalità di medicina preventiva, diagnosi, assistenza o terapia sanitaria o sociale nel rispetto dell'art. 9, par. 2, lett. h) e lettera i) e par. 3 del GDPR?</div>
            </td>
        </tr>
        <tr>
            <td style="text-align: center">
                <asp:Button runat="server" ID="btnOk" CssClass="consenso-pulsante" Text="<%$ Resources: Onit.OnAssistnet.OnVac.Web, ConsensoTrattamentoDatiUtente.Accetta %>" />
            </td>
            <td style="text-align: center">
                <asp:Button runat="server" ID="btnAnnulla" CssClass="consenso-pulsante" Text="<%$ Resources: Onit.OnAssistnet.OnVac.Web, ConsensoTrattamentoDatiUtente.Rifiuta %>" />
            </td>
        </tr>
    </table>
</div>
