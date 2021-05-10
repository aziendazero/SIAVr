<%@ Control Language="vb" AutoEventWireup="false" Codebehind="AggiungiVaccinazione.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.AggiungiVaccinazione" %>

<script type="text/javascript" >
		
		function controllaSeNumero(obj){
					
			if (isNaN(obj.value)){
				obj.value="";
				alert("Il valore della dose deve essere un numero!!");
				obj.focus();
				return false;
			}
			
			//controlla se il numero è intero, altrimenti genera errore con l'immissione del punto [modifica 10/05/2006]
			
			if (obj.value.replace(".",",").search(",") != -1){
				obj.value="";
				alert("Il valore della dose deve essere un numero intero!!");
				obj.focus();
				return false;
			}
		}
		
		function AggiungiVaccinazioneInizializzaToolBar(t)
		{
			t.PostBackButton=false;
		}
		
		function AggiungiVaccinazioneToolBarClick(ToolBar,button,evnt){
			evnt.needPostBack=true;
			switch (button.Key)			
			{
				case 'btnAnnulla':
					closeFm('<%= ModaleName %>');
					break
				case 'btnAggiungi':
					//recupero id del datagrid
					dgr="<%=dgrElencoAss.clientId%>";
					msg="un'associazione";
					
					//controllo sulla presenza di una selezione da passare al server
					if (!ControllaSelezione(dgr))
					{
						alert("Selezionare almeno " + msg + " per confermare!")
						evnt.needPostBack=false;
					}
					else
					{
						closeFm('<%= ModaleName %>');
					}
				    break;
			}
		}
		
		function ControllaSelezione(dgrId)
		{
			var control=false;
			dgr=document.getElementById(dgrId);
			for (i=1;i<dgr.rows.length;i++)
			{
				objChk=GetElementByTag(dgr.rows[i].cells[0],'INPUT',1,1,false);
				if (objChk.checked)
					control=true;
			}
			return control;
		}		
		
</script>

<link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />

<table cellpadding="0" cellspacing="0" border="0" width="100%" height="340px">
	<tr height="24">
		<td>
			<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
				<SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
				<ClientSideEvents InitializeToolbar="AggiungiVaccinazioneInizializzaToolBar" Click="AggiungiVaccinazioneToolBarClick"></ClientSideEvents>
                <Items>
					<igtbar:TBarButton Key="btnAggiungi" DisabledImage="~/Images/nuovo_dis.gif" Text="Aggiungi"
						Image="~/Images/nuovo.gif"></igtbar:TBarButton>
					<igtbar:TBarButton Key="btnAnnulla" DisabledImage="~/Images/annulla_dis.gif" Text="Annulla"
						Image="~/Images/annulla.gif"></igtbar:TBarButton>
				</Items>
			</igtbar:UltraWebToolbar>
		</td>
	</tr>
	<tr>
		<td>
			<div style="overflow:scroll;width:100%;height:315px">
				<asp:Panel id="pnlVaccinazioni" runat="server" Width="100%">
					<asp:DataGrid id="dgrElencoAss" Width="100%" runat="server" AutoGenerateColumns="False" CssClass="DataGrid">
						<SelectedItemStyle CssClass="Selected"></SelectedItemStyle>
						<EditItemStyle CssClass="Edit"></EditItemStyle>
						<AlternatingItemStyle CssClass="Alternating"></AlternatingItemStyle>
						<ItemStyle CssClass="Item"></ItemStyle>
						<HeaderStyle CssClass="Header"></HeaderStyle>
						<FooterStyle CssClass="Footer"></FooterStyle>
						<PagerStyle CssClass="Pager"></PagerStyle>
						<Columns>
							<asp:TemplateColumn>
								<HeaderStyle Width="10px"></HeaderStyle>
								<ItemTemplate>
									<asp:CheckBox id="chkSelezionaAss" runat="server"></asp:CheckBox>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:BoundColumn DataField="ASS_CODICE" HeaderText="Codice">
								<HeaderStyle Width="20%"></HeaderStyle>
							</asp:BoundColumn>
							<asp:BoundColumn DataField="ASS_DESCRIZIONE" HeaderText="Associazione">
								<HeaderStyle Width="78%"></HeaderStyle>
							</asp:BoundColumn>
							<asp:TemplateColumn HeaderText="Dosi">
								<HeaderStyle Width="1%"></HeaderStyle>
								<ItemTemplate>
									<asp:TextBox id="txt_dosi" Width="28px" onblur="controllaSeNumero(this)" runat="server" MaxLength="2"></asp:TextBox>
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
					</asp:DataGrid>
				</asp:Panel>
			</div>
		</td>
	</tr>
</table>
