<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="PagamentoVaccinazione.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.PagamentoVaccinazione" %>

<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>

<table cellspacing="0" cellpadding="2" width="100%" border="0" style="margin-top: 5px">
    <colgroup>
        <col width="15%" />
        <col width="85%" />
    </colgroup>
    <tr>
        <td class="label_right">Tipologia</td>
        <td class="label_left">
            <asp:DropDownList ID="ddlTipiPagVac" CssClass="Textbox_Stringa" runat="server" Width="100%" AutoPostBack="true" OnSelectedIndexChanged="ddlTipiPagVac_SelectedIndexChanged"></asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="label_right">Esenzione</td>
        <td class="label_left">
            <asp:DropDownList ID="ddlEseMalPagVac" Width="100%" runat="server"></asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="label_right">Importo&nbsp;€</td>
        <td class="label_left">
            <on_val:OnitJsValidator ID="valImpPagVac" CssClass="textbox_stringa_numerico" runat="server" Width="100%" actionCorrect="False" actionCustom="" actionDelete="False"
                actionFocus="True" actionMessage="True" actionSelect="True" actionUndo="True" autoFormat="False" CustomValFunction="validaNumero"
                validationType="Validate_custom" MaxLength="10">
            <Parameters>
                <on_val:ValidationParam paramName="numCifreIntere" paramOrder="0" 
                    paramType="number" paramValue="5" />
                <on_val:ValidationParam paramName="numCifreDecimali" paramOrder="1" 
                    paramType="number" paramValue="2" />
                <on_val:ValidationParam paramName="minValore" paramOrder="2" paramType="number" 
                    paramValue="0" />
                <on_val:ValidationParam paramName="maxValore" paramOrder="3" paramType="number" 
                    paramValue="99999.99" />
                <on_val:ValidationParam paramName="blnCommaSeparator" paramOrder="4" 
                    paramType="boolean" paramValue="true" />
            </Parameters>
            </on_val:OnitJsValidator>
        </td>
    </tr>
   <%-- <tr>
        <td align="center" colspan="2">
            <asp:Button ID="btnPagVacOk" Width="75px" runat="server" Text="Ok" Style="cursor: auto;" OnClick="btnPagVacOk_Click"></asp:Button>
            <asp:Button ID="btnPagVacAnnulla" Width="75px" runat="server" Text="Annulla" Style="cursor: auto;" OnClick="btnPagVacAnnulla_Click"></asp:Button>
        </td>
    </tr>    --%>
</table>
<script type="text/javascript">
<%--    function clearEseMalPag() {
        var ddl = document.getElementById('<%= Me.ddlEseMalPagVac.ClientID %>');
        if (ddl != null) ddl.selectedIndex = 0;
    }--%>
</script>

<style type="text/css">
    .Textbox_numerico_obbligatorio{
        text-align:left
    }
    .Textbox_numerico{
        text-align:left
    }
</style>
