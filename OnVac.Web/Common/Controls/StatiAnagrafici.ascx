<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Control Language="vb" AutoEventWireup="false" Codebehind="StatiAnagrafici.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.Common.Controls.StatiAnagrafici" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>

<table width="100%" border="0" style="table-layout: fixed;">
	<tr>
		<td id="tdLabel" class="label_left" runat="server" width="100px">Stati anagrafici:</td>
		<td width="26px" align="right">
			<asp:ImageButton id="btnAggiungiStati" runat="server" OnClick="btnAggiungiStati_Click"
				AlternateText="Impostazione filtro stati anagrafici" style="cursor: pointer" ></asp:ImageButton>
		</td>
		<td style="height: 22px; border: navy 1px solid; padding-left: 5px; background-color: #dcdcdc; vertical-align: top" >
			<asp:Label id="lblStatoAnagrafico" style="font-size: 10px; text-transform: uppercase; font-style: italic; font-family: Verdana"
				runat="server" Width="98%" CssClass="TextBox_Stringa" ></asp:Label>
		</td>
	</tr>
</table>

<on_ofm:OnitFinestraModale id="fmStatiAnagrafici" title="<div style=&quot;font-family:'Calibri'; font-size: 14px;padding:2px 0px 3px 2px;&quot;>Stati anagrafici</div>"
	runat="server" width="400px" BackColor="LightGray" UseDefaultTab="True" RenderModalNotVisible="True">
    <table style="border-width:0px; width:100%">
        <colgroup>
            <col style="width:5%" />
            <col style="width:90%" />
            <col style="width:5%" />
        </colgroup>
        <tr>
            <td colspan="3" style="text-align:center; font-weight:bold;">
                <asp:Label ID="lblHeader" runat="server" Text="Selezionare gli stati anagrafici su cui filtrare:" CssClass="label_center"></asp:Label>
            </td>
        </tr>
        <tr>
            <td></td>
            <td>
                <onit:CheckBoxList id="chlStatiAnagrafici" runat="server" CssClass="label_left" Width="100%" BorderColor="Navy"
			bgcolor="whitesmoke" BorderStyle="Solid" BorderWidth="1px"></onit:CheckBoxList>
            </td>
            <td></td>
        </tr>
    </table>
	<div style="text-align: center">
		<asp:Button id="btnConfermaSelezionaStati" OnClick="btnConfermaSelezionaStati_Click" Text="OK" runat="server" style="width:100px; cursor:pointer"></asp:Button>
		<asp:Button id="btnAnnullaSelezionaStati" OnClick="btnAnnullaSelezionaStati_Click" Text="Annulla" runat="server" style="width:100px; cursor:pointer"></asp:Button>
    </div>
</on_ofm:OnitFinestraModale>
