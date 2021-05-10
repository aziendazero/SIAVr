<%@ Control Language="vb" AutoEventWireup="false" Codebehind="MotivoEsc.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.MotivoEsc" %>
<table cellpadding="0" cellspacing="0" border="0" width="610" height="380">
	<tr height="20">
		<td>
			<igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                <DefaultStyle CssClass="infratoolbar_button_default">
                </DefaultStyle>
                <HoverStyle CssClass="infratoolbar_button_hover">
                </HoverStyle>
                <SelectedStyle CssClass="infratoolbar_button_selected">
                </SelectedStyle>
				<Items>
					<igtbar:TBarButton Key="btn_Conferma" DisabledImage="~/Images/conferma_dis.gif" Text="Conferma"
						Image="~/Images/conferma.gif"></igtbar:TBarButton>
					<igtbar:TBarButton Key="btn_Annulla" DisabledImage="~/Images/annullaconf_dis.gif" Text="Annulla"
						Image="~/Images/annullaconf.gif"></igtbar:TBarButton>
				</Items>
			</igtbar:UltraWebToolbar>
		</td>
	</tr>
	<tr height="380">
		<td>
			<div style="WIDTH:610px;HEIGHT:380px;OVERFLOW:auto">
				<asp:datagrid id="dg_moe_esc" runat="server" Width="100%" AutoGenerateColumns="False" CellPadding="1"
					GridLines="None">
					<SelectedItemStyle Font-Bold="True" CssClass="selected"></SelectedItemStyle>
					<AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
					<ItemStyle CssClass="item"></ItemStyle>
					<HeaderStyle CssClass="header"></HeaderStyle>
					<Columns>
						<asp:TemplateColumn>
							<HeaderStyle Width="20px"></HeaderStyle>
							<ItemTemplate>
								<asp:CheckBox id="cb" runat="server"></asp:CheckBox>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:BoundColumn DataField="moe_descrizione" HeaderText="Descrizione">
							<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
							<ItemStyle HorizontalAlign="Left"></ItemStyle>
						</asp:BoundColumn>
						<asp:BoundColumn DataField="moe_codice" HeaderText="Codice">
							<HeaderStyle HorizontalAlign="Center" Width="80px"></HeaderStyle>
							<ItemStyle HorizontalAlign="Left"></ItemStyle>
						</asp:BoundColumn>
						<asp:TemplateColumn>
							<HeaderStyle HorizontalAlign="Center" Width="20px"></HeaderStyle>
							<ItemStyle HorizontalAlign="Center"></ItemStyle>
						</asp:TemplateColumn>
						<asp:BoundColumn></asp:BoundColumn>
					</Columns>
					<PagerStyle CssClass="pager" Mode="NumericPages"></PagerStyle>
				</asp:datagrid>
			</div>
		</td>
	</tr>
</table>
