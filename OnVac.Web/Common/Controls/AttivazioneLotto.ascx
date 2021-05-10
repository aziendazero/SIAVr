<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="AttivazioneLotto.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.AttivazioneLotto" %>

<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>

<style type="text/css">
    .saveAlertLabel
    {
        font-family: Verdana;
        font-size: 12px;
        font-weight: bold;
        color: Navy;
    }
    .saveInfoLabel
    {
        font-family: Verdana;
        font-size: 11px;
        font-weight: normal;
        color: Navy;
    }
    .saveButton
    {
        width: 80px;
        cursor: pointer;
    }
</style>

<div style="width: 20px; height: 20px">
    <asp:ImageButton ID="btnAttivaDisattivaLotto" runat="server" />
</div>

<onitcontrols:OnitFinestraModale ID="modAttivazioneLotto" Title="Attivazione Lotto" runat="server" Width="400px" Height="200px" 
    BackColor="LightGray" NoRenderX="true" RenderModalNotVisible="false" ClientEventProcs-OnShow="setFocusIfNedeed()">
    <table border="0" cellpadding="2" cellspacing="0" width="100%">
        <colgroup>
            <col width="40%" />
            <col width="20%" />
            <col width="40%" />
        </colgroup>
        <tr>
            <td colspan="3" style="text-align:center">
                <div style="border:1px solid navy; background-color:#f5f5f5; width:95%; margin-top:10px; margin-bottom:15px;">
                    <table border="0" cellpadding="0" cellspacing="0" width="100%" style="padding-right:3px">
                        <colgroup>
                            <col width="25%" />
                            <col width="11%" />
                            <col width="14%" />
                            <col width="11%" />
                            <col width="14%" />
                            <col width="11%" />
                            <col width="14%" />
                        </colgroup>
                        <tr>
                            <td style="vertical-align:middle; padding-top:5px; text-align:center;" rowspan="3">
                                <img src="\On.Health\On.Assistnet\Icone\disk_blue_48.gif" alt="Salvataggio" style="text-align:center; " />
                            </td>
                            <td colspan="6" style="vertical-align:middle; padding-top:5px;">
                                <asp:Label ID="lblWarningOperazione" runat="server" CssClass="saveAlertLabel"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6" style="vertical-align:top;">
                                <asp:Label ID="lblCodiceLotto" runat="server" CssClass="saveInfoLabel" Width="100%" ></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="6" style="vertical-align:top;">
                                <asp:Label ID="lblDescrizioneLotto" runat="server" CssClass="saveInfoLabel" Width="100%" ></asp:Label>
                            </td>
                        </tr>
                        <tr style="padding-top:10px">
                            <td style="text-align:right">
                                <asp:Label ID="lblEtaMin" runat="server" CssClass="saveInfoLabel" Text="Eta minima"></asp:Label></td>
                            <td>
                                <on_val:OnitJsValidator id="txtEtaMinAnni" runat="server" Width="100%" CssClass="textbox_stringa"
                                    actionCorrect="False" actionDelete="False" actionFocus="False" actionMessage="True" actionSelect="True"
                                    actionUndo="False" autoFormat="False" validationType="Validate_integer" PreParams-numDecDigit="0" PreParams-maxValue="199"
                                    PreParams-minValue="0" MaxLength="3"></on_val:OnitJsValidator></td>
                            <td>
                                <asp:Label ID="lblEtaMinAnni" runat="server" CssClass="saveInfoLabel" Text="anni"></asp:Label></td>
                            <td>
                                <on_val:OnitJsValidator id="txtEtaMinMesi" runat="server" Width="100%" CssClass="textbox_stringa"
                                    actionCorrect="False" actionDelete="False" actionFocus="False" actionMessage="True" actionSelect="True"
                                    actionUndo="False" autoFormat="False" validationType="Validate_integer" PreParams-numDecDigit="0" PreParams-maxValue="999"
                                    PreParams-minValue="0" MaxLength="3"></on_val:OnitJsValidator></td>
                            <td>
                                <asp:Label ID="lblEtaMinMesi" runat="server" CssClass="saveInfoLabel" Text="mesi"></asp:Label></td>
                            <td>
                                <on_val:OnitJsValidator id="txtEtaMinGiorni" runat="server" Width="100%" CssClass="textbox_stringa"
                                    actionCorrect="False" actionDelete="False" actionFocus="False" actionMessage="True" actionSelect="True"
                                    actionUndo="False" autoFormat="False" validationType="Validate_integer" PreParams-numDecDigit="0" PreParams-maxValue="999"
                                    PreParams-minValue="0" MaxLength="3"></on_val:OnitJsValidator></td>
                            <td>
                                <asp:Label ID="lblEtaMinGiorni" runat="server" CssClass="saveInfoLabel" Text="giorni"></asp:Label></td>
                        </tr>
                        <tr>
                            <td style="text-align:right">
                                <asp:Label ID="lblEtaMax" runat="server" CssClass="saveInfoLabel" Text="Eta massima"></asp:Label></td>
                            <td>
                                <on_val:OnitJsValidator id="txtEtaMaxAnni" runat="server" Width="50px" CssClass="textbox_stringa"
                                    actionCorrect="False" actionDelete="False" actionFocus="False" actionMessage="True" actionSelect="True"
                                    actionUndo="False" autoFormat="False" validationType="Validate_integer" PreParams-numDecDigit="0" PreParams-maxValue="199"
                                    PreParams-minValue="0" MaxLength="3"></on_val:OnitJsValidator></td>
                            <td>
                                <asp:Label ID="lblEtaMaxAnni" runat="server" CssClass="saveInfoLabel" Text="anni"></asp:Label></td>
                            <td>
                                <on_val:OnitJsValidator id="txtEtaMaxMesi" runat="server" Width="50px" CssClass="textbox_stringa"
                                    actionCorrect="False" actionDelete="False" actionFocus="False" actionMessage="True" actionSelect="True"
                                    actionUndo="False" autoFormat="False" validationType="Validate_integer" PreParams-numDecDigit="0" PreParams-maxValue="999"
                                    PreParams-minValue="0" MaxLength="3"></on_val:OnitJsValidator></td>                                
                            <td>
                                <asp:Label ID="lblEtaMaxMesi" runat="server" CssClass="saveInfoLabel" Text="mesi"></asp:Label></td>
                            <td>
                                <on_val:OnitJsValidator id="txtEtaMaxGiorni" runat="server" Width="50px" CssClass="textbox_stringa"
                                    actionCorrect="False" actionDelete="False" actionFocus="False" actionMessage="True" actionSelect="True"
                                    actionUndo="False" autoFormat="False" validationType="Validate_integer" PreParams-numDecDigit="0" PreParams-maxValue="999"
                                    PreParams-minValue="0" MaxLength="3"></on_val:OnitJsValidator>
                            </td>
                            <td>
                                <asp:Label ID="lblEtaMaxGiorni" runat="server" CssClass="saveInfoLabel" Text="giorni"></asp:Label></td>
                        </tr>
                        <tr>
                            <td colspan="7" class="saveAlertLabel" style="text-align:center; vertical-align:middle; padding-top:10px; padding-bottom:10px">
                                L'operazione non potrà essere annullata:<br \>continuare con il salvataggio su database?
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Button ID="btnSalvaAttivazioneLotto" runat="server" Text="Salva" CssClass="saveButton" />
            </td>
            <td></td>
            <td>
                <asp:Button ID="btnAnnullaAttivazioneLotto" runat="server" Text="Annulla" CssClass="saveButton" />
            </td>
        </tr>
        <tr style="height:10px">
            <td colspan="3"></td>
        </tr>
    </table>
    <script type="text/javascript">
        function setFocusIfNedeed() {
            var idTxtEtaMinAnni = '<%= Me.txtEtaMinAnni.ClientId %>';
            if (document.getElementById(idTxtEtaMinAnni) != null) document.getElementById(idTxtEtaMinAnni).focus();
        }
    </script>
</onitcontrols:OnitFinestraModale>