<%@ Control Language="vb" AutoEventWireup="false" Codebehind="UscFiltroPrenotazioneSelezioneMultipla.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.UscFiltroPrenotazioneSelezioneMultipla" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>

<table border="0" cellpadding="0" cellspacing="5px" width="100%">
	<tr runat="server" id="trRdbFilter">
		<td style="width: 1%; height: 19px"></td>
		<td class="label_left" style="width: 64%; height: 19px">
			<asp:RadioButton id="rdbFiltro1" CssClass="label" Font-Bold="True" runat="server" GroupName="filtro"
				Checked="True" AutoPostBack="True"></asp:RadioButton></td>
		<td class="label_left" style="width: 24%; height: 19px">
			<asp:RadioButton id="rdbFiltro2" CssClass="label" Font-Bold="True" runat="server" GroupName="filtro"
				AutoPostBack="True"></asp:RadioButton></td>
	</tr>
	<tr style="vertical-align: top;">
		<td></td>
		<td>
			<div style="border: 1px solid navy; overflow: auto; height: 400px">
				<on_dgr:OnitGrid id="dgrValori" runat="server" CssClass="datagrid" AutoGenerateColumns="False" Width="100%"
					SelectionOption="none" PagerVoicesAfter="-1" PagerVoicesBefore="-1" SortedColumns="Matrice IGridColumn[]">
					<HeaderStyle CssClass="header"></HeaderStyle>
                    <ItemStyle CssClass="item"></ItemStyle>
					<AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
					<Columns>
						<asp:TemplateColumn>
							<HeaderStyle Width="3%"></HeaderStyle>
							<ItemTemplate>
								<asp:CheckBox id="chk" runat="server"></asp:CheckBox>
							</ItemTemplate>
						</asp:TemplateColumn>
						<on_dgr:OnitBoundColumn DataField="Codice" HeaderText="Codice" key="Codice" SortDirection="NoSort">
							<HeaderStyle Width="15%"></HeaderStyle>
						</on_dgr:OnitBoundColumn>
						<on_dgr:OnitBoundColumn DataField="Descrizione" HeaderText="Descrizione" key="Descrizione" SortDirection="NoSort">
							<HeaderStyle Width="55%"></HeaderStyle>
						</on_dgr:OnitBoundColumn>
						<asp:TemplateColumn HeaderText="Valori">
							<ItemTemplate>
								<asp:TextBox id="txtValori" CssClass="textbox_stringa" runat="server" onblur="checkValori(this);" 
                                    style="width:80px" ToolTip="Specificare le dosi separate dalla virgola"></asp:TextBox>
							</ItemTemplate>
						</asp:TemplateColumn>
					</Columns>
				</on_dgr:OnitGrid>
			</div>
		</td>
		<td runat="server" id="tdCheckList">
			<div style="BORDER: 1px solid navy; overflow: auto; height: 400px">
				<asp:CheckBoxList id="chkList" runat="server" CssClass="textbox_stringa">
					<asp:ListItem Value="1">1</asp:ListItem>
					<asp:ListItem Value="2">2</asp:ListItem>
					<asp:ListItem Value="3">3</asp:ListItem>
					<asp:ListItem Value="4">4</asp:ListItem>
					<asp:ListItem Value="5">5</asp:ListItem>
					<asp:ListItem Value="6">6</asp:ListItem>
					<asp:ListItem Value="7">7</asp:ListItem>
					<asp:ListItem Value="8">8</asp:ListItem>
					<asp:ListItem Value="9">9</asp:ListItem>
					<asp:ListItem Value="10">10</asp:ListItem>
					<asp:ListItem Value="11">11</asp:ListItem>
					<asp:ListItem Value="12">12</asp:ListItem>
					<asp:ListItem Value="13">13</asp:ListItem>
					<asp:ListItem Value="14">14</asp:ListItem>
					<asp:ListItem Value="15">15</asp:ListItem>
				</asp:CheckBoxList>
			</div>
		</td>		
	</tr>
</table>
<script type="text/javascript">
	function checkValori(obj) {
		if (validazioneValore(obj.id)) {
			return true;
		} else {
			alert('I valori inseriti non sono corretti.'); 
			obj.focus(); 
			return false;
		} 
	}
				
	function validazioneValore(idTxt) {
		var s = document.getElementById(idTxt).value;
		var strRE = '^$|^([1-9][0-9]{0,2},(\\s)?)*[1-9][0-9]{0,2}(\\s)?$'
		var re = new RegExp(strRE, "g");
		if (!re.test(s)) {
			document.getElementById(idTxt).value='';
			return false;
		}
		return true;
	}
</script>


