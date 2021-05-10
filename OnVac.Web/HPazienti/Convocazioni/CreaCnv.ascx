<%@ Control Language="vb" AutoEventWireup="false" Codebehind="CreaCnv.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.CreaCnv"%>
<link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
	<table width="100%" border="0">
		<TBODY>
			<tr>
				<td class="textbox_data" valign="bottom" colspan="2">
					<span style="FONT-WEIGHT:bold;TEXT-DECORATION:underline">Centro Vaccinale</span></td>
			</tr>
			<tr height="25">
				<td valign="middle" align="center" colspan="2" width="100%">
				    <asp:label runat="server" id="lbSameCNS" CssClass="textbox_stringa"></asp:label>
				    <table ID="tblCNS" runat="server">
				        <tr>
				            <td>
				                <asp:RadioButton ID="rbCNSLavoro" GroupName="CNS" CssClass="textbox_stringa" AutoPostBack="true" runat="server" />
				            </td>
				        </tr>
				        <tr>
				            <td>
				                <asp:RadioButton ID="rbCNSPaziente" GroupName="CNS" CssClass="textbox_stringa" AutoPostBack="true" runat="server" />
				            </td>
				        </tr>
				    </table>
				</td>
			</tr>
			<tr>
				<td class="textbox_data" valign="bottom" colspan="2">
					<span style="FONT-WEIGHT:bold;TEXT-DECORATION:underline">Modalità</span></td>
			</tr>
			<tr>
			<tr>
				<td align="center" colspan="2">
				    <table>
				        <tr>
				            <td>
				                <asp:RadioButton ID="rbCNVAutomatica" GroupName="CNV" Text="Automatica" CssClass="textbox_stringa"  runat="server" />
				            </td>
				        </tr>
				        <tr>
				            <td>
				                <asp:RadioButton ID="rbCNVOdierna" GroupName="CNV" Text="Odierna" CssClass="textbox_stringa" runat="server" />
				            </td>
				        </tr>
				        <tr>
				            <td>
				                <asp:RadioButton ID="rbCNVFutura" GroupName="CNV" Text="Futura" CssClass="textbox_stringa" runat="server" />
				            </td>
				        </tr>
				    </table>
				</td>
			</tr>
			<tr>
				<td align="center"><asp:button id="btnCrea" Width="96px" runat="server" Text="Crea"></asp:button></td>
			</tr>
		</TBODY>
	</table>
	<script language="javascript">
	    //
		document.getElementById('<% = btnCrea.Clientid %>').onclick = function() { closeFm('<%= ModaleName %>'); };
		//
	</script>
</TR></TBODY></TABLE>
