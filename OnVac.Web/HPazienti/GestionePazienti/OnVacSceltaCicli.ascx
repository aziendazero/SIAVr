<%@ Control Language="vb" AutoEventWireup="false" Codebehind="OnVacSceltaCicli.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVacSceltaCicli" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>

<link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

<table id="Table1"  cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td style="height:30px">
			<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="UltraWebToolbar1" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                <HoverStyle CssClass="infratoolbar_button_hover" ></HoverStyle>
				<SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
				<Items>
					<igtbar:TBarButton Key="btnConferma" Text="Conferma" Image="~/Images/conferma.gif"></igtbar:TBarButton>
					<igtbar:TBarButton Key="btnAnnulla" Text="Annulla" Image="~/Images/annulla.gif"></igtbar:TBarButton>
				</Items>
			</igtbar:UltraWebToolbar>
		</td>
	</tr>
	<tr>
		<td style="padding-bottom: 2px">
			<div style="overflow:auto; height:150px">
				<asp:DataGrid ID="dgrCicli" runat="server" PageSize="1000" AutoGenerateColumns="False" CssClass="DataGrid" Width="100%">
					<SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
					<AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
					<ItemStyle CssClass="Item"></ItemStyle>
					<HeaderStyle CssClass="Header"></HeaderStyle>
					<PagerStyle CssClass="Pager" Mode="NumericPages"></PagerStyle>
					<Columns>
						<asp:TemplateColumn>
							<HeaderStyle Width="5%"></HeaderStyle>
							<ItemTemplate>
								<asp:CheckBox id="chkCic_scelta" runat="server"></asp:CheckBox>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:BoundColumn DataField="CIC_CODICE" SortExpression="CIC_CODICE" HeaderText="Codice">
							<HeaderStyle Width="25%"></HeaderStyle>
						</asp:BoundColumn>
						<asp:BoundColumn DataField="CIC_DESCRIZIONE" SortExpression="CIC_DESCRIZIONE" HeaderText="Descrizione ciclo">
							<HeaderStyle Width="70%"></HeaderStyle>
						</asp:BoundColumn>
					</Columns>
				</asp:DataGrid>
			</div>
		</td>
	</tr>
	<tr>
		<td style="border-top: black thin solid; height:40px">
			<table class="label_left" id="Table2" cellspacing="0" cellpadding="0" width="100%" border="0" bgColor="#ffffcc">
				<tr>
					<td style="color:red">Ciclo</td>
					<td>Cicli non compatibili con la data di nascita</td>
				</tr>
				<tr>
					<td style="color:black">Ciclo</td>
					<td>Cicli compatibili con la data di nascita</td>
				</tr>
				<tr>
					<td style="font-weight:bold; color:black">Ciclo</td>
					<td>Cicli standard</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
