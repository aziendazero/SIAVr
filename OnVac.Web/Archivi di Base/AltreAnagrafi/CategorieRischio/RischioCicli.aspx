<%@ Page Language="vb" AutoEventWireup="false" Codebehind="RischioCicli.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.RischioCicli"%>

<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>
<%@ Register TagPrefix="uc1" TagName="InsCicli" Src="InsCicli.ascx" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_otb" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitTable" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>

<%@ Import namespace="Onit.OnAssistnet.OnVac" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
		<title>RischioCicli</title>
		
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
		
        <script type="text/javascript" language="javascript">
	
		function InizializzaToolBar(t)
		{
			t.PostBackButton=false;
		}

		function ToolBarClick(ToolBar, button, evnt)
        {
			evnt.needPostBack=true;
		
			switch(button.Key)
			{
				case 'btnAnnulla':
		
					if("<%response.Write(OnitLayout31.Busy)%>"=="True") 
						evnt.needPostBack= confirm("Le modifiche effettuate andranno perse. Continuare?");
					else
						evnt.needPostBack=false;
					break;
			}
		}
		
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" height="100%" width="100%" TitleCssClass="Title3" Titolo="Categorie Rischio - Cicli">
				
                <div class="title" id="divLayoutTitolo_sezione1" style="width: 100%">
					<asp:label id="LayoutTitolo" runat="server" CssClass="title" Width="100%"></asp:label>
                </div>

				<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
					<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
					<Items>
						<igtbar:TBarButton Key="btnSalva" Text="Salva" Image="~/Images/salva.gif"></igtbar:TBarButton>
						<igtbar:TBarButton Key="btnAnnulla" Text="Annulla" Image="~/Images/annulla.gif"></igtbar:TBarButton>
						<igtbar:TBarButton Key="btnInserisci" Text="Inserisci" Image="~/Images/nuovo.gif"></igtbar:TBarButton>
						<igtbar:TBSeparator></igtbar:TBSeparator>
						<igtbar:TBarButton Key="btnIndietro" Text="Indietro" Image="~/Images/indietro.gif"></igtbar:TBarButton>
					</Items>
				</igtbar:UltraWebToolbar>
				
                <div class="Sezione">Elenco Cicli</div>

				<asp:Panel id="pan_Vac" runat="server" Width="100%">
					<asp:DataGrid id="dgrRischioCicli" style="PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; PADDING-TOP: 0px"
						runat="server" CssClass="datagrid" Width="100%" AutoGenerateColumns="False" CellPadding="1"
						GridLines="None">
						<SelectedItemStyle Font-Bold="True" CssClass="selected"></SelectedItemStyle>
						<AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
						<ItemStyle CssClass="item"></ItemStyle>
						<HeaderStyle CssClass="header"></HeaderStyle>
						<FooterStyle ForeColor="#4A3C8C" BackColor="#B5C7DE"></FooterStyle>
						<Columns>
							<asp:ButtonColumn Text="&lt;img  title=&quot;Elimina&quot; src=&quot;../../../images/elimina.gif&quot;&gt;"
								CommandName="Delete">
								<HeaderStyle HorizontalAlign="Center" Width="1%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center"></ItemStyle>
							</asp:ButtonColumn>
							<asp:TemplateColumn HeaderText="Codice">
								<HeaderStyle HorizontalAlign="Center" Width="9%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Left"></ItemStyle>
								<ItemTemplate>
									<asp:Label id="lblCodRischioCiclo" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("cic_codice") %>'>
									</asp:Label>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText="Descrizione">
								<HeaderStyle HorizontalAlign="Center" Width="50%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Left"></ItemStyle>
								<ItemTemplate>
									<asp:Label id="lblDescRischioCiclo" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("cic_descrizione") %>'>
									</asp:Label>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText="Introduzione">
								<HeaderStyle HorizontalAlign="Center" Width="15%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center"></ItemStyle>
								<ItemTemplate>
									<asp:Label id="lblIntroRischioCiclo" runat="server" Text='<%# OnVacUtility.GetFormattedValue(Container.DataItem("cic_data_introduzione"), "System.DateTime", "dd/MM/yyyy") %>'>
									</asp:Label>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText="Fine">
								<HeaderStyle HorizontalAlign="Center" Width="15%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center"></ItemStyle>
								<ItemTemplate>
									<asp:Label id="lblFineRischioCiclo" runat="server" Text='<%# OnVacUtility.GetFormattedValue(Container.DataItem("cic_data_fine"), "System.DateTime", "dd/MM/yyyy") %>'>
									</asp:Label>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText="Standard">
								<HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center"></ItemStyle>
								<ItemTemplate>
									<asp:Label id="lblStandardRischioCiclo" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("cod_descrizione")  %>'>
									</asp:Label>
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
					</asp:DataGrid>
				</asp:Panel>

				<on_ofm:OnitFinestraModale id="fmCicliRischio" title="Inserimento Cicli" runat="server"  width="410px" BackColor="Gainsboro" NoRenderX="True">
					<uc1:InsCicli id="InsCicli1" runat="server"></uc1:InsCicli>
				</on_ofm:OnitFinestraModale>

			</on_lay3:onitlayout3>
        </form>
	</body>
</html>