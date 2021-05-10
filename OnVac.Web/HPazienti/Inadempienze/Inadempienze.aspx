<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Inadempienze.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_Inadempienze" %>

<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="uc1" TagName="InsInadempienze" Src="InsInadempienze.ascx" %>
<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="onitcontrols" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>

<%@ Import namespace="Onit.OnAssistnet.OnVac" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>VacInadempiute</title>

		<script type="text/javascript" language="javascript">
		
		function InizializzaToolBar(t)
		{
			t.PostBackButton = false;
		}
		
		function InizializzaToolBar_mod(t)
		{
			t.PostBackButton = false;
		}
		
		function ToolBarClick_mod(ToolBar,button,evnt)
		{
			evnt.needPostBack = true;
		}

		function ToolBarClick(ToolBar, button, evnt)
        {
			evnt.needPostBack = true;
		
			switch(button.Key)
			{
				case 'btn_StampaComunicazioneAlSindaco':
					st(evnt);
					break;
				
				case 'btn_Annulla':
					if("<%response.write(CStr(onitlayout31.busy).toLower())%>" == "true")
					{
						evnt.needPostBack = confirm("Le modifiche effettuate andranno perse. Continuare?");
					}
					else
					{
						evnt.needPostBack = false;
					}
					break;
						
				case 'btn_Salva':
					if("<%response.write(CStr(onitlayout31.busy).toLower())%>"!="true")
					{ 
						evnt.needPostBack = false;
					}
					else
					{
						if ('<%= msgElimProg %>' != "") 
							if (!confirm('<%= msgElimProg %>')) evnt.needPostBack = false;
					}
					break;
					
				default:
					evnt.needPostBack = true;
					break;
			}
		}

		function st(evt)
        {
			var messaggioConferma;
			messaggioConferma = "Attenzione: si desidera impostare il valore <% = RecuperaStato(Enumerators.StatoInadempienza.CasoConcluso) %> nello stato delle inadempienze dei pazienti selezionati"
			messaggioConferma += "\r\n(premendo \'Ok\' la Comunicazione al Sindaco sarà stampabile solo da \'Inadempienze\' di ogni paziente)?"
			
			if (confirm(messaggioConferma))
			{
				evt.needPostBack = false;
				__doPostBack('Stampa', 'ImpostaStatoS');
			}
			else
			{
				evt.needPostBack = false;
				__doPostBack('Stampa', 'ImpostaStatoN');
			}
		}
	
		</script>
		
        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" TitleCssClass="Title3" HeightTitle="90px" width="100%" height="100%" Titolo="Inadempienze">
				
                <div class="Title" id="divLayoutTitolo" style="width: 100%">
					<asp:Label id="LayoutTitolo" runat="server" CssClass="title"></asp:Label>
                </div>

				<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" CssClass="infratoolbar" ItemWidthDefault="80px" >
					<DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover" ></HoverStyle>
                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
					<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
					<Items>
						<igtbar:TBarButton Key="btn_Salva" Text="Salva" DisabledImage="~/Images/salva_dis.gif" Image="~/Images/salva.gif"></igtbar:TBarButton>
						<igtbar:TBarButton Key="btn_Annulla" Text="Annulla" DisabledImage="~/Images/annulla_dis.gif"
							Image="~/Images/annulla.gif"></igtbar:TBarButton>
						<igtbar:TBarButton Key="btn_Inserisci" Text="Inserisci" DisabledImage="~/Images/nuovo_dis.gif"
							Image="~/Images/nuovo.gif"></igtbar:TBarButton>
						<igtbar:TBSeparator></igtbar:TBSeparator>
						<igtbar:TBarButton Key="btn_StampaTerminePerentorio" Text="Termine Perentorio" DisabledImage="~/Images/stampa_dis.gif"
							Image="~/Images/stampa.gif">
							<DefaultStyle Width="150px" CssClass="infratoolbar_button_default"></DefaultStyle>
						</igtbar:TBarButton>
						<igtbar:TBarButton Key="btn_StampaComunicazioneAlSindaco" Text="Comunicazione al Sindaco" DisabledImage="~/Images/stampa_dis.gif"
							Image="~/Images/stampa.gif">
							<DefaultStyle Width="170px" CssClass="infratoolbar_button_default"></DefaultStyle>
						</igtbar:TBarButton>
					</Items>
				</igtbar:UltraWebToolbar>
				
                <div class="sezione" id="divLayoutTitolo_sezione" style="WIDTH: 100%">
					<asp:Label id="LayoutTitolo_sezione" runat="server">ELENCO VACCINAZIONI</asp:Label>
                </div>
				
                <asp:Panel id="pan_vac" style="POSITION: relative" runat="server" Width="100%">
					<asp:DataGrid id="dg_VacInad" style="TABLE-LAYOUT: fixed" runat="server" CssClass="DATAGRID" Width="100%"
						GridLines="None" CellPadding="1" AutoGenerateColumns="False">
						<SelectedItemStyle Font-Bold="True" CssClass="selected"></SelectedItemStyle>
						<AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
						<ItemStyle CssClass="item"></ItemStyle>
						<HeaderStyle CssClass="header"></HeaderStyle>
						<PagerStyle CssClass="pager" Mode="NumericPages"></PagerStyle>
						<Columns>
							<asp:ButtonColumn Text="&lt;img  title=&quot;Elimina&quot; src=&quot;../../images/elimina.gif&quot;"
								CommandName="Delete">
								<HeaderStyle HorizontalAlign="Center" Width="16px"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center"></ItemStyle>
							</asp:ButtonColumn>
							<asp:EditCommandColumn ButtonType="LinkButton" UpdateText="&lt;img  title=&quot;Conferma&quot; src=&quot;../../images/conferma.gif&quot;&gt;"
								CancelText="&lt;img  title=&quot;Annulla&quot; src=&quot;../../images/annullaconf.gif&quot; &gt;"
								EditText="&lt;img  title=&quot;Modifica&quot; src=&quot;../../images/modifica.gif&quot;  &gt;">
								<HeaderStyle HorizontalAlign="Center" Width="16px"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center"></ItemStyle>
							</asp:EditCommandColumn>
							<asp:TemplateColumn>
								<HeaderStyle HorizontalAlign="Left" Width="25%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Left"></ItemStyle>
								<HeaderTemplate>
									<P>Descrizione&nbsp;</P>
								</HeaderTemplate>
								<ItemTemplate>
									<asp:Label id="lb_descVac" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("vac_descrizione") %>'>
									</asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<HeaderStyle Width="10%"></HeaderStyle>
								<HeaderTemplate>
									Codice
								</HeaderTemplate>
								<ItemTemplate>
									<asp:Label id="lb_codVac" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("pin_vac_codice") %>'>
									</asp:Label>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<HeaderStyle Width="20%"></HeaderStyle>
								<HeaderTemplate>
									Stato
								</HeaderTemplate>
								<ItemTemplate>
									<asp:Label id="lblCodStato" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("pin_stato") %>' Visible="False">
									</asp:Label>
									<asp:Label id="lblDescStato" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("cod_descrizione") %>'>
									</asp:Label>
								</ItemTemplate>
								<EditItemTemplate>
									<asp:DropDownList id="ddlStato" runat="server" Width="100%" DataSource="<%# Me.CollCodifiche %>" DataTextField="Descrizione" DataValueField="Codice">
									</asp:DropDownList>
								</EditItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<HeaderStyle Width="10%"></HeaderStyle>
								<HeaderTemplate>
									Stampato
								</HeaderTemplate>
								<ItemTemplate>
									<asp:Label id="lblStampato" Visible="False" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("pin_stampato") %>'>
									</asp:Label>
									<asp:Label id="Label1" runat="server" Text='<%# IIF(DataBinder.Eval(Container, "DataItem")("pin_stampato") & ""="S","SI","NO") %>'>
									</asp:Label>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<HeaderStyle Width="10%"></HeaderStyle>
								<HeaderTemplate>
									Data
								</HeaderTemplate>
								<ItemTemplate>
									<asp:Label id="lblData" runat="server" Text='<%# ctype(DataBinder.Eval(Container, "DataItem")("pin_data"),DATETIME).tostring("dd/MM/yyyy") %>'>
									</asp:Label>
								</ItemTemplate>
								<EditItemTemplate>
									<on_val:OnitDatePick id="dpkData" runat="server" Width="100%" CssClass="textbox_data" Height="20px" DateBox="True"
										NoCalendario="True"></on_val:OnitDatePick>
								</EditItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<HeaderStyle Width="20%"></HeaderStyle>
								<HeaderTemplate>
									Utente
								</HeaderTemplate>
								<ItemTemplate>
									<asp:Label id="lblCodUtente" runat="server" Visible=False Text='<%# DataBinder.Eval(Container, "DataItem")("pin_ute_id") %>'>
									</asp:Label>
									<asp:Label id="lblDescUtente" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("ute_descrizione") %>'>
									</asp:Label>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn>
								<HeaderStyle Width="55px"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center"></ItemStyle>
								<HeaderTemplate>
									Dettaglio
								</HeaderTemplate>
								<ItemTemplate>
									<asp:ImageButton id="btnDettaglio" onclick="apriDettaglio" runat="server" ToolTip="Dettaglio" ImageUrl="~/Images/dettaglio.gif" CommandArgument='<%# DataBinder.Eval(Container, "DataItem")("pin_vac_codice") %>'>
									</asp:ImageButton>
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
					</asp:DataGrid>

					<on_ofm:OnitFinestraModale id="modDettaglio" title="Dettaglio" runat="server"  width="280px"  BackColor="aliceblue" NoRenderX="true">
						
                        <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="toolbarMod" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                            <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                            <HoverStyle CssClass="infratoolbar_button_hover" ></HoverStyle>
				            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
							<ClientSideEvents InitializeToolbar="InizializzaToolBar_mod" Click="ToolBarClick_mod"></ClientSideEvents>
							<Items>
								<igtbar:TBarButton Key="btn_Chiudi" Text="Chiudi" DisabledImage="~/Images/esci_dis.gif"
									Image="~/Images/esci.gif"></igtbar:TBarButton>
							</Items>
						</igtbar:UltraWebToolbar>

						<div class="sezione" id="divLayout" style="width: 100%">
							<asp:Label id="Label6" runat="server">Date appuntamenti</asp:Label>
                        </div>

						<table width="100%">
							<tr>
								<td class="label_left" width="60%">
									<asp:Label id="Label2" runat="server" CssClass="label_letf" Width="100%">Data appunt. Avviso :</asp:Label></td>
								<td class="label_left" width="40%">
									<asp:Label id="lblData1" runat="server" CssClass="label_letf" Width="100%"></asp:Label></td>
							</tr>
							<tr>
								<td class="label_left" style="HEIGHT: 18px" width="60%">
									<asp:Label id="Label3" runat="server" CssClass="label_letf" Width="100%">Data appunt. 1° Sollecito :</asp:Label></td>
								<td class="label_left" style="HEIGHT: 18px" width="40%">
									<asp:Label id="lblData2" runat="server" CssClass="label_letf" Width="100%"></asp:Label></td>
							</tr>
							<tr>
								<td class="label_left" width="60%">
									<asp:Label id="Label4" runat="server" CssClass="label_letf" Width="100%">Data appunt. 2° Sollecito :</asp:Label></td>
								<td class="label_left" width="40%">
									<asp:Label id="lblData3" runat="server" CssClass="label_letf" Width="100%"></asp:Label></td>
							</tr>
							<tr>
								<td class="label_left" width="60%">
									<asp:Label id="Label5" runat="server" CssClass="label_letf" Width="100%">Data appunt. 3° Sollecito :</asp:Label></td>
								<td class="label_left" width="40%">
									<asp:Label id="lblData4" runat="server" CssClass="label_letf" Width="100%"></asp:Label></td>
							</tr>
							<tr>
								<td class="label_left" width="60%">
									<asp:Label id="Label7" runat="server" CssClass="label_letf" Width="100%">Data stampa Term. Per. :</asp:Label></td>
								<td class="label_left" width="40%">
									<asp:Label id="lblData5" runat="server" CssClass="label_letf" Width="100%"></asp:Label></td>
							</tr>
						</table>

					</on_ofm:OnitFinestraModale>
					
                    <on_ofm:onitfinestramodale id="modInsInadempienze" title="Inserisci inadempienze" runat="server" BackColor="LightGray" Width="350px">
						<uc1:InsInadempienze id="uscInsInadempienze" runat="server"></uc1:InsInadempienze>
					</on_ofm:onitfinestramodale>

				</asp:Panel>
			</on_lay3:onitlayout3>

		</form>
		
        <script type="text/javascript" language="javascript">
		<%response.write(strJS)%>
		</script>

		<!-- #include file="../../Common/Scripts/AggiornaStatoVaccinazioni.inc" -->
		<!-- #include file="../../Common/Scripts/ControlloDatiValidi.inc" -->

	</body>
</html>
