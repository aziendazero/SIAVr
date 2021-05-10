<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="GestioneScadenze.aspx.vb"
    Inherits="Onit.OnAssistnet.OnVac.GestioneScadenze" %>
<%@ Import Namespace="Onit.OnAssistnet.OnVac" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_otb" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitTable" %>
<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="uc1" TagName="SelezioneVaccinazioni" Src="../../../Common/Controls/SelezioneVaccinazioni.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>MotiviEsclusione-GestioneScadenze</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript">
		
		function confermaEliminazione()
		{
			if (!confirm("Si desidera eliminare l'associazione selezionata?"))
				StopPreventDefault(getEvent());
		}
		
    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
    <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Width="100%" Height="100%"
        TitleCssClass="Title3" Titolo="Motivi Esclusione - Gestione Scadenze">
        <on_otb:OnitTable ID="OnitTable1" runat="server" Height="100%" Width="100%">
			<on_otb:onitsection id="sezRicerca" runat="server" width="100%" typeHeight="content">
				<on_otb:onitcell id="cellaRicerca" runat="server" height="100%" Width="100%">
					<div class="title" id="divLayTitolo">
					<asp:Label id="LayoutTitolo" runat="server" CssClass="title" Width="100%" BorderStyle="None">Motivi Esclusione - Gestione Scadenze</asp:Label>
                    </div>
                    <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                        <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                        <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                        <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
						<Items>
                            <igtbar:TBarButton Key="btnIndietro" Text="Indietro" DisabledImage="~/Images/indietro_dis.gif" Image="~/Images/indietro.gif" />
							<igtbar:TBSeparator></igtbar:TBSeparator>
                            <%--
                            <igtbar:TBarButton Key="btnSalva" Text="Salva" DisabledImage="~/Images/salva_dis.gif" Image="~/Images/salva.gif" />
							<igtbar:TBarButton Key="btnAnnulla" Text="Annulla" DisabledImage="~/Images/annulla_dis.gif" Image="~/Images/annulla.gif" />
                            <igtbar:TBSeparator></igtbar:TBSeparator>
                            --%>
							<igtbar:TBarButton Key="btnNew" Text="Nuova scadenza"  DisabledImage="~/Images/nuovo_dis.gif" Image="~/Images/nuovo.gif" >
                                <DefaultStyle CssClass="infratoolbar_button_default" Width="130px"></DefaultStyle>
                            </igtbar:TBarButton>
						</Items>
					</igtbar:UltraWebToolbar>
					<div class="Sezione">Elenco scadenze</div>
				</on_otb:onitcell>
			</on_otb:onitsection>
			<on_otb:onitsection id="sezDgr" runat="server" width="100%" typeHeight="calculate">
				<on_otb:onitcell id="cellaDgr" runat="server" height="100%" Width="100%" typescroll="auto">
                    <asp:DataGrid id="dgrGestioneScadenze" style="table-layout: fixed" runat="server" Width="100%" CssClass="DataGrid" AutoGenerateColumns="False">
						<SelectedItemStyle Font-Bold="True" CssClass="Selected"></SelectedItemStyle>
						<AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
						<ItemStyle BackColor="#e7e7ff" HorizontalAlign="Center"></ItemStyle>
						<HeaderStyle CssClass="Header" HorizontalAlign="Center"></HeaderStyle>
						<Columns>
                            <asp:TemplateColumn>
                                <HeaderStyle  Width="1px" />
                                <ItemTemplate>
                                    <asp:HiddenField id="hidKey" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem")("MOS_ID") %>' />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:ButtonColumn Text="&lt;div class=&quot;margin_btn&quot;&gt; &lt;img onclick='confermaEliminazione();' title=&quot;Elimina&quot; src=&quot;../../../images/elimina.gif&quot; &gt; &lt;/div&gt;" CommandName="Delete">
                                <HeaderStyle  Width="20px" />
						    </asp:ButtonColumn>
						    <asp:EditCommandColumn ButtonType="LinkButton"  
UpdateText="&lt;div class=&quot;margin_btn&quot;&gt; &lt;img  title=&quot;Aggiorna&quot; src=&quot;../../../images/conferma.gif&quot; &gt; &lt;/div&gt;" CancelText="&lt;img  title=&quot;Annulla&quot; src=&quot;../../../images/annullaconf.gif&quot;&gt;" EditText="&lt;div class=&quot;margin_btn&quot;&gt; &lt;img  title=&quot;Modifica&quot; src=&quot;../../../images/modifica.gif&quot; &gt; &lt;/div&gt;">
                                <HeaderStyle  Width="20px" />
						    </asp:EditCommandColumn>
							<asp:TemplateColumn HeaderText="Mesi">
								<HeaderStyle Width="20%" />
                                <ItemTemplate>
                                    <asp:Label ID="lblMesi" runat="server" CssClass="Label" Text='<%# DataBinder.Eval(Container, "DataItem")("MOS_MESI") %>' />
                                </ItemTemplate>
								<EditItemTemplate>
                                    <on_val:OnitJsValidator id="txtMesi" runat="server" Width="50px" CssClass="TextBox_Numerico" ReadOnly="false" actionCorrect="False" actionDelete="False" actionFocus="False" actionMessage="True" actionSelect="True" actionUndo="False" autoFormat="False" validationType="Validate_integer" PreParams-numDecDigit="0" PreParams-maxValue="11" PreParams-minValue="0" Text='<%# DataBinder.Eval(Container, "DataItem")("MOS_MESI") %>' ></on_val:OnitJsValidator>
                                </EditItemTemplate>
							</asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Anni">
                                <HeaderStyle Width="20%" />
								<ItemTemplate>
                                    <asp:Label ID="lblAnni" runat="server" CssClass="Label" Text='<%# DataBinder.Eval(Container, "DataItem")("MOS_ANNI") %>' />
                                </ItemTemplate>
								<EditItemTemplate>
                                    <on_val:OnitJsValidator id="txtAnni" runat="server" Width="50px" CssClass="TextBox_Numerico" ReadOnly="false" actionCorrect="False" actionDelete="False" actionFocus="False" actionMessage="True" actionSelect="True" actionUndo="False" autoFormat="False" validationType="Validate_integer" PreParams-numDecDigit="0" PreParams-maxValue="120" PreParams-minValue="0" Text='<%# DataBinder.Eval(Container, "DataItem")("MOS_ANNI") %>' ></on_val:OnitJsValidator>
                                </EditItemTemplate>
							</asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Vaccinazioni">
                                <HeaderStyle Width="60%" />
                                <ItemTemplate>
                                    <uc1:SelezioneVaccinazioni ID="ucSelezioneVaccinazioni" runat="server" Enabled="false" ImageUrl='<%# OnVacUtility.GetIconUrl("filtro_associazioni.gif") %>' ImageUrlDisabled='<%# OnVacUtility.GetIconUrl("filtro_associazioni_dis.gif") %>' ImageUrlHovered='<%# OnVacUtility.GetIconUrl("filtro_associazioni_hov.gif") %>' CodiciSelezionatiVisibiliMax="10" CodiciSelezionati='<%# DataBinder.Eval(Container, "DataItem")("MOS_LST_VAC_CODICE") %>' />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <uc1:SelezioneVaccinazioni ID="ucSelezioneVaccinazioniEdit" runat="server" Enabled="true" ImageUrl='<%# OnVacUtility.GetIconUrl("filtro_associazioni.gif") %>' ImageUrlDisabled='<%# OnVacUtility.GetIconUrl("filtro_associazioni_dis.gif") %>' ImageUrlHovered='<%# OnVacUtility.GetIconUrl("filtro_associazioni_hov.gif") %>' CodiciSelezionatiVisibiliMax="10" CodiciSelezionati='<%# DataBinder.Eval(Container, "DataItem")("MOS_LST_VAC_CODICE") %>' />
                                </EditItemTemplate>
                            </asp:TemplateColumn>
						</Columns>
					</asp:DataGrid>
				</on_otb:onitcell>
			</on_otb:onitsection>
        </on_otb:OnitTable>
    </on_lay3:OnitLayout3>
    </form>
</body>
</html>
