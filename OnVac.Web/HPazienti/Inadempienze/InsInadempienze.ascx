<%@ Control Language="vb" AutoEventWireup="false" Codebehind="InsInadempienze.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.InsInadempienze" %>
<script type="text/javascript" language="javascript">
		
		function InizializzaToolBar(t)
		{
			t.PostBackButton=false;
		}
		
		function ToolBarClick_(ToolBar,button,evnt){
			evnt.needPostBack=true;
			switch(button.Key)
			{
				case 'btn_Conferma':
					closeFm('<%= ModaleName %>');
					break;
				case 'btn_Annulla':
					closeFm('<%= ModaleName %>');
					evnt.needPostBack=false;
					break;
				default:
					evnt.needPostBack=true;
			}
		}
	
</script>

<link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
<link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

<table cellpadding="0" cellspacing="0" border="0" width="342" height="222">
	<tr height="22">
		<td>
			<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		        <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
				<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick_"></ClientSideEvents>
				<Items>
					<igtbar:TBarButton Key="btn_Conferma" DisabledImage="~/Images/conferma_dis.gif" Text="Conferma"
						Image="~/Images/conferma.gif"></igtbar:TBarButton>
					<igtbar:TBarButton Key="btn_Annulla" DisabledImage="~/Images/annullaconf_dis.gif" Text="Annulla"
						Image="~/Images/annullaconf.gif"></igtbar:TBarButton>
				</Items>
			</igtbar:UltraWebToolbar>
		</td>
	</tr>
	<tr height="200">
		<td>
			<div style="OVERFLOW:auto;WIDTH:340px;HEIGHT:200px">
				<div id="divLayTit_Sez" class="sezione" style="WIDTH:99%">
					<asp:label id="LayoutTitolo_sezione" runat="server">ELENCO VACCINAZIONI</asp:label>
				</div>
				<asp:datagrid id="dg_vac" runat="server" CssClass="DATAGRID" Width="100%" AutoGenerateColumns="False"
					CellPadding="1" GridLines="None">
					<SelectedItemStyle Font-Bold="True" CssClass="selected"></SelectedItemStyle>
					<AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
					<ItemStyle CssClass="item"></ItemStyle>
					<HeaderStyle CssClass="header"></HeaderStyle>
					<Columns>
						<asp:TemplateColumn>
							<HeaderStyle Width="30px"></HeaderStyle>
							<ItemStyle HorizontalAlign="Center"></ItemStyle>
							<ItemTemplate>
								<asp:CheckBox id="cb" runat="server"></asp:CheckBox>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:BoundColumn DataField="vac_descrizione" HeaderText="Descrizione">
							<HeaderStyle HorizontalAlign="Center" Width="250px"></HeaderStyle>
							<ItemStyle HorizontalAlign="Left"></ItemStyle>
						</asp:BoundColumn>
						<asp:BoundColumn DataField="vac_codice" HeaderText="Codice">
							<HeaderStyle HorizontalAlign="Center" Width="100px"></HeaderStyle>
							<ItemStyle HorizontalAlign="Left"></ItemStyle>
						</asp:BoundColumn>
						<asp:BoundColumn></asp:BoundColumn>
					</Columns>
					<PagerStyle CssClass="pager" Mode="NumericPages"></PagerStyle>
				</asp:datagrid>
			</div>
		</td>
	</tr>
</table>

<script type="text/javascript" language="javascript">
	<%response.write(strJS)%>
</script>
