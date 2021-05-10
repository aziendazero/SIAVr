<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Convocazioni.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.Convocazioni" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<%@ Register TagPrefix="uc1" TagName="ModConv" Src="ModConv.ascx" %>
<%@ Register TagPrefix="uc1" TagName="NotePaziente" Src="NotePaziente.ascx" %>
<%@ Register TagPrefix="uc1" TagName="CreaCnv" Src="CreaCnv.ascx" %>
<%@ Register TagPrefix="uc1" TagName="InsAssociazione" Src="../InsAssociazione.ascx" %>
<%@ Register TagPrefix="uc1" TagName="RicercaBilancioCnv" Src="./RicercaBilancioCnv.ascx" %>

<%@ Import namespace="Onit.OnAssistnet.OnVac.OnVacUtility" %>
<%@ Import namespace="Onit.OnAssistnet.OnVac" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN"> 

<html>
	<head>
		<title>Convocazioni</title>

        <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/toolbar.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/default/toolbar.default.css") %>' type="text/css" rel="stylesheet" />

	    <script type="text/javascript" src='<%= OnVacUtility.GetScriptUrl("onvac.common.js") %>' ></script>
		<script type="text/javascript">
		    function confirmDelete(evt) 
            {
                if (!confirm("ATTENZIONE !!!\nLa convocazione verrà eliminata.\nContinuare ?")) 
                {
		            StopPropagation(evt);
		            StopPreventDefault(evt);
		        }
		    }
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
			<on_lay3:onitlayout3 id="OnitLayout31" runat="server" width="100%" height="100%" titolo="Convocazioni" TitleCssClass="Title3">

				<asp:Panel id="PanelTitolo" runat="server" CssClass="title" Width="100%">
					<asp:Label id="LayoutTitolo" runat="server"></asp:Label>
				</asp:Panel>
                <div>
                    <telerik:RadToolBar ID="ToolBar" runat="server" Width="100%" Skin="Default" EnableEmbeddedSkins="false" EnableAjaxSkinRendering="false" 
                        EnableEmbeddedBaseStylesheet="false" OnButtonClick="Toolbar_ButtonClick">
                        <Items>
                            <telerik:RadToolBarButton Value="CreaCNV" Text="Crea CNV" runat="server" ImageUrl="../../images/rotella.gif" DisabledImageUrl="../../images/rotella_dis.gif" 
                                ToolTip="Creazione convocazioni"></telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Value="btnCompilaBilancio" Text="Compila Anamnesi" runat="server" ImageUrl="../../images/bilanci_add.gif" DisabledImageUrl="../../images/bilanci_add_dis.gif" 
                                ToolTip="Compilazione anamnesi"></telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Value="btnVisioneBilanci" Text="Visione Anamnesi" runat="server" ImageUrl="../../images/bilanci.gif" DisabledImageUrl="../../images/bilanci_dis.gif" 
                                ToolTip="Visione anamnesi"></telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Value="btnRecuperaStoricoVacc" Text="Recupera" runat="server" ImageUrl="../../images/recupera.png" DisabledImageUrl="../../images/recupera_dis.png" 
                                ToolTip="Recupera lo storico vaccinale centralizzato del paziente"></telerik:RadToolBarButton>
                        </Items>
                    </telerik:RadToolBar>
                </div>
				<asp:Panel id="PanelTitolo_sezione" runat="server" CssClass="vac-sezione">
					<asp:Label id="LayoutTitolo_sezione" runat="server">ELENCO CONVOCAZIONI</asp:Label>
				</asp:Panel>

                <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="99%" ScrollBars="Auto" RememberScrollPosition="true">
					<asp:DataGrid id="dg_Cnv" runat="server" CssClass="DATAGRID" Width="100%" AutoGenerateColumns="False"
						CellPadding="1" GridLines="None">
						<SelectedItemStyle CssClass="selected"></SelectedItemStyle>
						<AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
						<ItemStyle CssClass="item"></ItemStyle>
						<HeaderStyle CssClass="header"></HeaderStyle>
						<Columns>
							<asp:TemplateColumn ItemStyle-VerticalAlign="Top">
								<ItemTemplate>
									<asp:ImageButton ID="imgSelect" 
									    AlternateText="Visualizza Vaccinazioni" 
									    ImageUrl='<%# Me.ResolveClientUrl("~/images/sel.gif") %>'
									    CommandName="Select" 
									    runat="server" />
								</ItemTemplate>
							</asp:TemplateColumn>
								<asp:TemplateColumn ItemStyle-VerticalAlign="Top">
								<ItemTemplate>
									<asp:ImageButton ID="imgEdit" 
									    AlternateText="Modifica Convocazione" 
									    ImageUrl="~/Images/modifica.gif"  
									    CommandName="Edit" 
									    runat="server" />
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn ItemStyle-VerticalAlign="Top">
								<ItemTemplate>
									<asp:ImageButton ID="imgDelete"  OnClientClick="confirmDelete(event)"
									    AlternateText="Elimina Convocazione" 
									    ImageUrl="~/Images/elimina.gif"  
									    CommandName="Delete" 
									    runat="server" />
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText="">
								<ItemTemplate>
									<asp:Image id="imgRitardo" style="margin:0 4px 0 4px"
										title="Ritardo"
										Visible='<%# Container.DataItem("SOLLECITO") > 0 %>' 
										ImageUrl="~/Images/avvertimento.gif"
										runat="server" />
								</ItemTemplate>
								<ItemStyle VerticalAlign="Top"></ItemStyle>
							</asp:TemplateColumn>
							<asp:BoundColumn DataField="cns_descrizione" HeaderText="Centro Vaccinale">
								<HeaderStyle HorizontalAlign="Left" Width="20%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Left" VerticalAlign="Top"></ItemStyle>
							</asp:BoundColumn>
							<asp:BoundColumn ItemStyle-Font-Bold="True" DataField="cnv_data" HeaderText="Data&lt;BR&gt; Convocazione"
								DataFormatString="{0:dd/MM/yyyy}">
								<HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center" VerticalAlign="Top"></ItemStyle>
							</asp:BoundColumn>
							<asp:BoundColumn DataField="cnv_data_invio" HeaderText="Data &lt;BR&gt; Invito" DataFormatString="{0:dd/MM/yyyy}">
								<HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center" VerticalAlign="Top"></ItemStyle>
							</asp:BoundColumn>
							<asp:BoundColumn DataField="cnv_data_appuntamento" HeaderText="Data &lt;BR&gt; Appuntamento" DataFormatString="{0:dd/MM/yyyy}">
								<HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center" VerticalAlign="Top"></ItemStyle>
							</asp:BoundColumn>
							<asp:BoundColumn DataField="cnv_durata_appuntamento" HeaderText="Durata &lt;BR&gt; Appuntamento">
								<HeaderStyle HorizontalAlign="Center" Width="13%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center" VerticalAlign="Top"></ItemStyle>
							</asp:BoundColumn>
							<asp:BoundColumn DataField="cic_descrizione" HeaderText="Ciclo">
								<HeaderStyle HorizontalAlign="Left" Width="20%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Left" VerticalAlign="Top"></ItemStyle>
							</asp:BoundColumn>
							<asp:BoundColumn DataField="Num_Sed_Bil" HeaderText="Numero&lt;BR&gt;Sed/Bil">
								<HeaderStyle HorizontalAlign="Center" Width="8%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center" VerticalAlign="Top"></ItemStyle>
							</asp:BoundColumn>
							<asp:BoundColumn DataField="Desc_Ass_Mal" HeaderText="Ass/Mal" >
								<ItemStyle VerticalAlign="Top" Width="8%"/>
							</asp:BoundColumn>
							<asp:BoundColumn DataField="Desc_Vac_Bil" HeaderText="Vacc/Bil">
								<ItemStyle VerticalAlign="Top" Font-Bold="True" Width="12%"/>
							</asp:BoundColumn>
							<asp:TemplateColumn>
								<ItemStyle VerticalAlign="Top" Width="10px" />
								<ItemTemplate>
									<asp:Label id="lb_tipo_cnv" Text='<%# DataBinder.Eval(Container, "DataItem")("tipo_cnv") %>' style="background-color:navy; color:white; font-weight:bold; font-size:11px; margin:2px 2px 2px 10px; padding:0 2px; display:block" runat="server"  />
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
						<PagerStyle CssClass="pager" Mode="NumericPages"></PagerStyle>
					</asp:DataGrid>
				</dyp:DynamicPanel>

				<on_ofm:OnitFinestraModale id="modNotePaziente" NoRenderX="false" title="Note del Paziente" runat="server" width="557px" BackColor="LightGray">
					<uc1:NotePaziente id="uscNotePaziente" runat="server"></uc1:NotePaziente>
				</on_ofm:OnitFinestraModale>

				<on_ofm:onitfinestramodale id="modModConv" NoRenderX="false" title="Modifica convocazione" runat="server" Width="700px" Height="430px" BackColor="LightGray" >
					<uc1:ModConv id="uscModConv" runat="server"></uc1:ModConv>
				</on_ofm:onitfinestramodale>

				<on_ofm:onitfinestramodale id="modCreaCnv" NoRenderX="false" title="Crea convocazione" runat="server" Width="400px" BackColor="LightGray">
					<uc1:CreaCnv id="uscCreaCnv" runat="server"></uc1:CreaCnv>
				</on_ofm:onitfinestramodale>

				<on_ofm:OnitFinestraModale id="modInsAssociazione" title="Inserisci associazione" runat="server" width="610px" BackColor="LightGray" NoRenderX="True">
					<uc1:InsAssociazione id="uscInsAssociazione" runat="server" ShowInfoSomministrazione="false" ></uc1:InsAssociazione>
				</on_ofm:OnitFinestraModale>

                <on_ofm:OnitFinestraModale id="modRicercaBilancioCnv" title="Rilevazione" runat="server" width="610px" BackColor="LightGray" NoRenderX="True">
					<uc1:RicercaBilancioCnv id="uscRicercaBilancioCnv" runat="server"></uc1:RicercaBilancioCnv>
				</on_ofm:OnitFinestraModale>

			</on_lay3:onitlayout3>

			<!-- #include file="../../Common/Scripts/AggiornaStatoVaccinazioni.inc" -->

		</form>
	</body>
</html>
