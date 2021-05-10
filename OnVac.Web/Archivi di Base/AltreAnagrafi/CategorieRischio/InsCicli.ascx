<%@ Control Language="vb" AutoEventWireup="false" Codebehind="InsCicli.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.InsCicli"%>
<script type="text/javascript" language="javascript">
    function InizializzaToolBar(t) {
        t.PostBackButton = false;
    }
		
    function ToolBarClick(ToolBar,button,evnt){
		evnt.needPostBack=true;
		switch  (button.Key)
		{
			case 'btn_Annulla':
				closeFm('<%= ModaleName %>');
				break;
			case 'btn_Conferma':
				//recupero id del datagrid
				dgr="<%=dgrCicli.clientId%>";
				//controllo sulla presenza di una selezione da passare al server
				if (!ControllaSelezione(dgr))
				{
					alert("Selezionare almeno un ciclo per confermare!")
					evnt.needPostBack=false;
				}
				else
				{
					closeFm('<%= ModaleName %>');
				}
				break;
		}
	}

    function ControllaSelezione(dgrId) {
        var control = false;
        dgr = document.getElementById(dgrId);
        for (i = 1; i < dgr.rows.length; i++) {
            objChk = GetElementByTag(dgr.rows[i].cells[0], 'INPUT', 1, 1, false);
            if (objChk.checked)
                control = true;
        }
        return control;
    }

</script>
<table height="421" cellspacing="0" cellpadding="0" width="400" border="0">
	<tr height="20">
		<td>
            <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		        <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
				<Items>
					<igtbar:TBarButton Key="btn_Conferma" DisabledImage="~/Images/conferma.gif" Text="Conferma"
						Image="~/Images/conferma.gif"></igtbar:TBarButton>
					<igtbar:TBarButton Key="btn_Annulla" DisabledImage="~/Images/annullaconf.gif" Text="Annulla"
						Image="~/Images/annullaconf.gif"></igtbar:TBarButton>
				</Items>
			</igtbar:ultrawebtoolbar>
        </td>
	</tr>
	<tr height="380">
		<td>
			<div style="overflow:auto; width:400px; height:380px">
                <asp:datagrid id="dgrCicli" runat="server" GridLines="None" CellPadding="1" AutoGenerateColumns="False" Width="100%">
					<SelectedItemStyle Font-Bold="True" CssClass="selected"></SelectedItemStyle>
					<AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
					<ItemStyle CssClass="item"></ItemStyle>
					<HeaderStyle CssClass="header"></HeaderStyle>
					<PagerStyle CssClass="pager" Mode="NumericPages"></PagerStyle>
					<Columns>
						<asp:TemplateColumn>
							<HeaderStyle Width="20px"></HeaderStyle>
							<ItemTemplate>
								<asp:CheckBox id="cb" runat="server"></asp:CheckBox>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:BoundColumn DataField="CIC_DESCRIZIONE" HeaderText="Descrizione">
							<HeaderStyle HorizontalAlign="Center" Width="200px"></HeaderStyle>
							<ItemStyle HorizontalAlign="Left"></ItemStyle>
						</asp:BoundColumn>
						<asp:BoundColumn DataField="CIC_CODICE" HeaderText="Codice">
							<HeaderStyle HorizontalAlign="Center" Width="100px"></HeaderStyle>
							<ItemStyle HorizontalAlign="Left"></ItemStyle>
						</asp:BoundColumn>
						<asp:TemplateColumn>
							<HeaderStyle HorizontalAlign="Center" Width="30px"></HeaderStyle>
							<ItemStyle HorizontalAlign="Center"></ItemStyle>
						</asp:TemplateColumn>
						<asp:BoundColumn></asp:BoundColumn>
					</Columns>
				</asp:datagrid>
			</div>
		</td>
	</tr>
</table>
