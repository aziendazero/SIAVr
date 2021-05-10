<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="RinnovaEsc.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.RinnovaEsc" %>

<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' rel="stylesheet" type="text/css" />
<link href='<%= ResolveClientUrl("~/css/button.css") %>' rel="stylesheet" type="text/css" />
<link href='<%= ResolveClientUrl("~/css/default/button.default.css") %>' rel="stylesheet" type="text/css" />

<script type="text/javascript" src="<%= Page.ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>

<script type="text/javascript">

    var ModaleName = '<%= ModaleName %>';

    function InizializzaToolBar_ModDataVacEsc(t) {
        t.PostBackButton = false;
    }

    function ToolBarClick_ModDataVacEsc(toolBar, button, evnt) {
        evnt.needPostBack = true;
        switch (button.Key) {
            case 'btn_Annulla':                
                evnt.needPostBack = true;
                break;
            //case 'btn_Annulla':
            //    closeFm(ModaleName);
            //    evnt.needPostBack = false;
                break;
            case 'btn_Conferma':
                var data = OnitDataPickGet('dpkDataScadenzaRinnovaEsc');
                var dataOdierna = Date.now
                if (confrontaDate(data, dataOdiera) == -1) {
                    alert("Salvataggio non effettuato: la data di scadenza non può essere inferiore alla data odierna.");
                    evnt.needPostBack = false;
                    return;
                }
                evnt.needPostBack = false;
                break;
        }
    }

</script>

<style type="text/css">
    .non-rinnovabile {
        color: darkblue;
        font-weight: bold;
        font-style: italic;
    }
</style>

<table cellpadding="0"  cellspacing="0" border="0" width="100%" height="530px">
    <tr>
        <td>
            <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolBar_RinnovaEsc" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                <ClientSideEvents InitializeToolbar="InizializzaToolBar_RinnovaEsc" Click="ToolBarClick_RinnovaEsc"></ClientSideEvents>
                <Items>
                    <igtbar:TBarButton Key="btn_Salva" DisabledImage="~/Images/salva_dis.gif"
                        Text="Salva" Image="~/Images/salva.gif" />
                    <igtbar:TBarButton Key="btn_Annulla" DisabledImage="~/Images/annullaconf_dis.gif"
                        Text="Annulla" Image="~/Images/annullaconf.gif" />
                </Items>
            </igtbar:UltraWebToolbar>
        </td>
    </tr>
    <tr>
        <td>           
            <div id="divScroll" style="overflow: auto; width: 100%; height: 500px" onscroll="SaveScroll(this.scrollTop)">
               <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
			        <asp:DataGrid id="dg_vacExRinnovate" style="padding: 0px;" runat="server" Width="100%" CssClass="datagrid" AutoGenerateColumns="False" CellPadding="1" GridLines="None">
				        <SelectedItemStyle Font-Bold="True" CssClass="selected"></SelectedItemStyle>
				        <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
				        <ItemStyle CssClass="item"></ItemStyle>
				        <HeaderStyle CssClass="header"></HeaderStyle>
				        <FooterStyle ForeColor="#4A3C8C" BackColor="#B5C7DE"></FooterStyle>
				        <PagerStyle CssClass="pager" Mode="NumericPages"></PagerStyle>
				        <Columns>
                            <asp:TemplateColumn HeaderText="Vaccinazione">
						        <HeaderStyle HorizontalAlign="Left" Width="20%"></HeaderStyle>
						        <ItemStyle HorizontalAlign="Left"></ItemStyle>
						        <ItemTemplate>
							        <asp:Label id="lblVaccinazione" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("vac_descrizione") %>'>
							        </asp:Label>							    
                                    <asp:HiddenField id="hdCodVaccinazione" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem")("vex_vac_codice") %>' />
						        </ItemTemplate>
					        </asp:TemplateColumn>
                            <%--<asp:TemplateColumn HeaderText="Dose">       
                                <HeaderStyle Width="10%" />
                                <ItemTemplate>
                                    <asp:TextBox ID="txtDose" runat="server" CssClass="textboxVacEs_noEdit" Width="75%" MaxLength="2" ReadOnly="true"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateColumn>--%>
                            <asp:TemplateColumn HeaderText="Motivazione">
					            <HeaderStyle HorizontalAlign="Left" Width="20%"></HeaderStyle>
					            <ItemStyle HorizontalAlign="Left"></ItemStyle>
					            <ItemTemplate>
						            <asp:Label id="tb_motivo" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("moe_descrizione") %>'>
						            </asp:Label>
						            <asp:HiddenField id="hdCodMotivo" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem")("vex_moe_codice") %>' />
					            </ItemTemplate>
				            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Data Scadenza">
					            <HeaderStyle HorizontalAlign="Left" Width="20%"></HeaderStyle>
					            <ItemStyle HorizontalAlign="Left"></ItemStyle>
					            <ItemTemplate>
						            <asp:Label id="lblDataScadenza" runat="server" Text='<%# ConvertToDateString(DataBinder.Eval(Container.DataItem, "vex_data_scadenza"), False) %>'></asp:Label>
					            </ItemTemplate>
				            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Nuova Data Visita">
					            <HeaderStyle HorizontalAlign="Left" Width="20%"></HeaderStyle>
					            <ItemStyle HorizontalAlign="Left"></ItemStyle>
					            <ItemTemplate>
						            <asp:Label id="lblNuovaDataVisita" runat="server" Text='<%# ConvertToDateString(DataBinder.Eval(Container.DataItem, "newDataVisita"), False) %>'></asp:Label>
					            </ItemTemplate>
				            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Nuova Data Scadenza">
					            <HeaderStyle HorizontalAlign="Left" Width="20%"></HeaderStyle>
					            <ItemStyle HorizontalAlign="Left"></ItemStyle>
					            <ItemTemplate>
						            <asp:Label id="lblNuovaDataScadenza" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem")("newDataScadenza") %>'></asp:Label>
					            </ItemTemplate>
				            </asp:TemplateColumn>
                            <asp:TemplateColumn>
						        <HeaderStyle Width="1%"></HeaderStyle>
						        <ItemTemplate>
							        <asp:Label id="Scaduta" runat="server" CssClass="legenda-vaccinazioni-esclusioneScaduta" Visible='false' ToolTip="Esclusione scaduta" Text='<%# DataBinder.Eval(Container, "DataItem")("s") %>' ></asp:Label>
						        </ItemTemplate>
					        </asp:TemplateColumn>                          
                        </Columns>
                    </asp:DataGrid>
                </dyp:DynamicPanel>
            </div>
        </td>
    </tr>
</table>
