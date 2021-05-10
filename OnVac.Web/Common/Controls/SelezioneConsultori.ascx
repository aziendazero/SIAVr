<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="SelezioneConsultori.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.SelezioneConsultori" %>

<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>

<table border="0" style="width: 100%; table-layout: fixed;">
	<tr>
		<td width="26px" align="right" runat="server" id="tdBtnSelezione">
			<asp:ImageButton id="btnSelezionaCns" ImageUrl="../../images/filtro_selezioneCns.gif" OnClick="btnSelezionaCns_Click"
				AlternateText="Impostazione filtro centri vaccinali" style="cursor:pointer;" runat="server" 
                onmouseover="this.src='../../images/filtro_selezioneCns_hov.gif'" onmouseout="this.src='../../images/filtro_selezioneCns.gif'" ></asp:ImageButton>
		</td>
		<td style="height: 22px; border: navy 1px solid;" valign="top" bgcolor="gainsboro" runat="server" id="tdLblConsultori">
			<asp:Label id="lblConsultori" style="font-size: 10px; text-transform: uppercase; font-style: italic; font-family: Verdana"
				runat="server" Width="98%" CssClass="TextBox_Stringa" ></asp:Label>
		</td>
	</tr>
</table>

<on_ofm:OnitFinestraModale id="fmConsultori" title="<div style=&quot;font-family:'Microsoft Sans Serif';text-align:center; font-size: 11pt;padding-bottom:2px&quot;><img src='../../images/consultorio.gif'>&nbsp;Centri Vaccinali</div>"
	runat="server" width="400px" BackColor="LightGray" UseDefaultTab="True" RenderModalNotVisible="True">

    <div style="font-size: 12px; font-family: Verdana; color:navy; margin-top:5px; text-align:center">
		<b>Selezionare i centri vaccinali su cui filtrare:</b>
    </div>
    <div style="font-size: 12px; font-family: Verdana; margin-top:10px; margin-left:5px">
        <asp:CheckBox ID="chkTutti" runat="server" AutoPostBack="true" Text="Tutti" />
    </div>
    
    <div style="border:1px solid navy; background-color:whitesmoke; height:350px; overflow:auto; vertical-align:top;"> 
        <onit:CheckBoxList id="chklConsultori" runat="server" DataTextField="Descrizione" DataValueField="Codice"
            CssClass="label_left" RepeatDirection="Vertical" TextAlign="Right" Width="90%">
        </onit:CheckBoxList>
    </div>
    
    <p style="margin-top:10px; text-align:center;">
		<asp:Button id="btnConfermaSelezione" Text="OK" runat="server" Width="100px" style="cursor:pointer"></asp:Button>
		<asp:Button id="btnAnnullaSelezione" Text="Annulla" runat="server" Width="100px" style="cursor:pointer"></asp:Button>
    </p>

</on_ofm:OnitFinestraModale>