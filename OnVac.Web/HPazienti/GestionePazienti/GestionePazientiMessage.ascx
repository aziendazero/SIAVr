<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="GestionePazientiMessage.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.GestionePazientiMessage" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Import Namespace="Onit.OnAssistnet.OnVac" %>
<style type="text/css">
    .imgCol
    {
        width: 60px;
        text-align: center;
    }
  
    .maintable
    {
    	height: 160px; 
    	width: 99%; 
    	border: 1px solid navy; 
    	background-color: whitesmoke;
    	padding: 2px;
    	margin: 2px;
    }
    
    .maintable tr
    {
    	height: 5px; 
    }
    
    .contenttable
    {
        font-family: verdana;
        color: navy;
        font-size: 12px;
        text-align: left;
        vertical-align: middle;
    }
    
    .contenttable hr
    {
        color: navy;
        width: 90%;
        text-align: center;
        height: 1px;
    }
        
</style>
<div style="margin-left: 2px; border-bottom: #8080ff 1px solid; border-left: #8080ff 1px solid; border-top: #8080ff 1px solid;
    border-right: #8080ff 1px solid" id="lblConsole" class="alert" runat="server">
</div>
<!-- Modale warning paz. deceduto, paz non appartenente al consultorio corrente, alert consenso -->
<on_ofm:OnitFinestraModale ID="fmWarningMessage" Title="<div style=&quot;vertical-align:middle;text-align:center; font-family:'Microsoft Sans Serif'; font-size: 11pt;padding-bottom:2px&quot; height='16px' width='100%' >&nbsp;ATTENZIONE</div>"
    runat="server" Width="480px" BackColor="Gainsboro" NoRenderX="True" RenderModalNotVisible="True"> 
    <table class="maintable">
        <tr>
            <td></td>
        </tr>
        <tr style="font-family: Verdana; height: 100%; color: navy; font-size: 16px; font-weight: bold">
            <td>

                <!-- Messaggio paz cancellato -->
                <table style="display: none" class="contenttable" id="tblPazCancellato" border="0" cellspacing="0" cellpadding="2" width="100%" runat="server">
                    <colgroup>
                        <col />
                        <col />
                    </colgroup>
                    <tr>
                        <td rowspan="2" class="imgCol">
                            <img alt="Controllo bloccante" align="middle" src="../../images/errore.gif" style="padding: 5;width: 32; height: 32" />
                        </td>
                        <td>Il paziente è marcato come <b>CANCELLATO</b>.</td>
                    </tr>
                    <tr>
                        <td>Impossibile modificare i dati anagrafici.</td>
                    </tr>
                    <tr>
                        <td colspan="2" style="width:460px;">
                            <hr />
                        </td>
                    </tr>
                </table>

                <!-- Messaggio paz deceduto -->
                <table style="display: none" class="contenttable" id="tblPazDeceduto" border="0" cellspacing="0" cellpadding="2" width="100%" runat="server">
                     <colgroup>
                        <col />
                        <col />
                    </colgroup>
                    <tr>
                        <td rowspan="3" class="imgCol">
                            <img alt="Controllo non bloccante" align="middle" src="../../images/warning.gif" style="padding: 5;width: 32; height: 32" />
                        </td>
                        <td>
                            Il paziente corrente:
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <%=DescrizionePaziente%>
                        </td>
                    </tr>
                    <tr>
                        <td style="font-weight: bold;">
                            risulta deceduto
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="width:460px;">
                            <hr />
                        </td>
                    </tr>
                </table>

                <!-- Messaggio paz non appartenente al consultorio corrente -->
                <table style="display: block" class="contenttable" id="tblCnsCorrente" border="0" cellspacing="0" cellpadding="2" width="100%" runat="server">
                    <colgroup>
                        <col />
                        <col />
                    </colgroup>
                    <tr>
                        <td rowspan="3" class="imgCol">
                            <img alt="Controllo non bloccante" align="middle" src="../../images/warning.gif" width="32" height="32" />
                        </td>
                        <td style="font-weight: bold;">
                            Il paziente non appartiene al centro vaccinale corrente.
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Centro vaccinale paziente: <%=Me.CentroVaccinalePaziente%>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Centro vaccinale corrente: <%=Me.CentroVaccinaleCorrente%>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="width:460px;">
                            <hr />
                        </td>
                    </tr>
                </table>

                <!-- Messaggio assistenza scaduta -->
                <table style="display: none" class="contenttable" id="tblAssistenzaScaduta" border="0" cellspacing="0" cellpadding="2" width="100%" runat="server">
                    <colgroup>
                        <col />
                        <col />
                    </colgroup>
                    <tr>
                        <td rowspan="2" class="imgCol">
                            <img alt="Controllo non bloccante" align="middle" src="../../images/warning.gif" style="padding: 5;width: 32; height: 32" />
                        </td>
                        <td>L'assistenza sanitaria del paziente</td>
                    </tr>
                    <tr>
                        <td style="font-weight: bold;">risulta scaduta.</td>
                    </tr>
                    <tr>
                        <td colspan="2" style="width:460px;">
                            <hr />
                        </td>
                    </tr>
                </table>

                <!-- Stato del consenso del paz -->
                <table style="display: block" class="contenttable" id="tblStatoConsenso" border="0" cellspacing="0" cellpadding="2" width="100%" runat="server">
                    <colgroup>
                        <col />
                        <col />
                        <col />
                    </colgroup>
                    <tr>
                        <td rowspan="2" class="imgCol" >
                            <img alt="Controllo non bloccante" align="middle" src="../../images/warning.gif" width="32" height="32" />
                        </td>
                        <td colspan="2">Stato Consenso Paziente:</td>
                    </tr>
                    <tr>
                        <td style="width:20px">
                            <asp:Image ID="imgWarningStatoConsenso" runat="server" ImageUrl="~/Images/consensoAltro.png" />
                        </td>
                        <td style="font-weight: bold;">
                            <asp:Label ID="lblWarningStatoConsenso" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr >
                        <td colspan="3" style="width:460px;">
                            <hr  />
                        </td>
                    </tr>
                </table>
                
                <!-- Campi obbligatori non impostati -->
                <table style="display: block" class="contenttable" id="tblCampiObbligatori" border="0" cellspacing="0" cellpadding="2" width="100%" runat="server">
                    <colgroup>
                        <col />
                        <col />
                    </colgroup>
                    <tr>
                        <td rowspan="2" class="imgCol">
                            <img alt="Controllo bloccante" align="middle" src="../../images/errore.gif" width="32" height="32" />
                        </td>
                        <td>I seguenti <b>campi obbligatori</b> non sono stati impostati. Per poter proseguire è necessario
                            impostarli. Operazione annullata</td>
                    </tr>
                    <tr>
                        <td style="padding-left:50px; text-align: left;">
                            <asp:Label ID="lblCampiNoSet" runat="server" Font-Bold="True"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="width:460px;">
                            <hr />
                        </td>
                    </tr>
                </table>
                
                <!-- Campi raccomandati non impostati -->
                <table style="display: block" class="contenttable" id="tblCampiWarning" border="0" cellspacing="0" cellpadding="2" width="100%" runat="server">
                    <colgroup>
                        <col />
                        <col />
                    </colgroup>
                    <tr>
                        <td  class="imgCol" >
                            <img alt="Controllo non bloccante" align="middle" src="../../images/warning.gif" width="32" height="32" />
                        </td>
                        <td>I seguenti <b>campi raccomandati</b> non sono stati impostati:</td>
                    </tr>
                    <tr>
                        <td colspan="2" style="padding-left:50px; text-align: left;">
                            <asp:Label ID="lblCampiWarningNoSet" runat="server" Font-Bold="True"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td align="center">
                <input type="button" value="Chiudi" id="btnCloseWarning" style="font-family: verdana; cursor: hand" onclick="closeFm('<%=fmWarningMessage.ClientID%>');" />
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
        </tr>
    </table>
</on_ofm:OnitFinestraModale>
