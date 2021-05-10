<%@ Control Language="vb" AutoEventWireup="false" Codebehind="SelezioneAmbulatorio.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.Common.Controls.SelezioneAmbulatorio" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>

<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="uc2" TagName="AjaxList" Src="./AjaxList.ascx" %>

<div>
<table id="ScegliAmbulatorio_MainWindow" cellspacing="0" cellpadding="1" border="0">
	<tr>
		<td class="label_left" valign="middle" align="center" width="19%" runat="server" id="tdLabelCentroVaccinale">
            Centro Vaccinale
        </td>
		<td valign="middle" align="center" width="80%" runat="server" id="tdValoreCentroVaccinale">
            <div style="width: 100%;">
			    <asp:textbox id="txtParent_Desc" runat="server" CssClass="TextBox_Stringa_Disabilitato" Width="80%" ReadOnly="True"></asp:textbox>
    			<asp:textbox id="txtParent_Cod" runat="server" CssClass="TextBox_Stringa_Disabilitato" Width="18%" ReadOnly="True"></asp:textbox>
            </div>
        </td>
		<td style="vertical-align:middle; text-align: center; width: 20px" rowspan="2">
			<asp:imagebutton id="btnOpenFM" style="cursor: pointer" ImageUrl="~/Images/modifica.gif" runat="server" />
            <asp:ImageButton ID="btnClean" ImageUrl="~/images/eraser.png" runat="server" style="margin-top:5px" />
        </td>
	</tr>
	<tr>
		<td class="label_left" valign="middle" align="center" width="19%" runat="server" id="tdLabelAmbulatorio">
            Ambulatorio
        </td>
		<td valign="middle" align="center" width="80%" runat="server" id="tdValoreAmbulatorio">
            <div style="width: 100%;">
			    <asp:textbox id="txtChild_Desc" runat="server" CssClass="TextBox_Stringa_Disabilitato" Width="80%" ReadOnly="True"></asp:textbox>
                <asp:textbox id="txtChild_Cod" runat="server" CssClass="TextBox_Stringa_Disabilitato" Width="18%" ReadOnly="True"></asp:textbox>
            </div>
        </td>
	</tr>
</table>
</div>
										
<!-- Finestra Modale per la scelta del consultorio e dell'ambulatorio -->
<onitcontrols:onitfinestramodale id="fmSelect" title="<div style=&quot;font-family:'Microsoft Sans Serif'; font-size: 11pt;padding-bottom:2px&quot;>&nbsp;Selezione Ambulatorio</div>" runat="server" ZIndexPosition="101" BackColor="LightGray" RenderModalNotVisible="True" NoRenderX="true">
    <div style="overflow: auto; width: 490px;  margin-top: 3px">
        <uc2:AjaxList id="alsConsultorio" Label="Centro Vaccinale" CampoCodice="cns_codice" CampoDescrizione="cns_descrizione" Tabella="t_ana_consultori" PostBackOnSelect="True" Tutti="False" MieiCNS="True" runat="server"></uc2:AjaxList>
        <br/>
        <asp:Label ID="lblCnsSelezionato" runat="server" Text="" CssClass="Label" Font-Italic="true" />
        <uc2:AjaxList id="alsAmbulatorio" Label="Ambulatorio" CampoCodice="amb_codice" CampoDescrizione="amb_descrizione" Tabella="t_ana_ambulatori" PostBackOnSelect="True" Tutti="True" MieiCNS="false" runat="server"></uc2:AjaxList>
    </div> 
</onitcontrols:onitfinestramodale>