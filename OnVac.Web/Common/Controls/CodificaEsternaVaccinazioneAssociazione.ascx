<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="CodificaEsternaVaccinazioneAssociazione.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.CodificaEsternaVaccinazioneAssociazione" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Assembly="Onit.Web.UI.WebControls.DynamicPanel" Namespace="Onit.Web.UI.WebControls.DynamicPanel" %>

<style type="text/css">
    .obsoleto{
        font-style:italic; 
        color:darkblue;
    }
</style>

        <script type="text/javascript" src="<%= Page.ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>

<script type="text/javascript">
    function InitTlb(t) {
        t.PostBackButton = false;
    }

    function ClickTlb(t, btn, evnt) {

        evnt.needPostBack = true;
        		
        switch (btn.Key) {

            case 'btnSalva':
                evnt.needPostBack = confirm('ATTENZIONE: salvare le modifiche effettuate?');
                break;
            
            case 'btnAnnulla':
                evnt.needPostBack = confirm('ATTENZIONE: le modifiche effettuate verranno perse. Continuare?');
                break;
        }
    }

</script> 

<on_lay3:onitlayout3 id="Onitlayout31" runat="server" TitleCssClass="Title3" Titolo="Codifica esterna vaccinazioni per associazione" width="100%" height="100%" NAME="Onitlayout31">
    <div>
        <table border="0" cellpadding="0" cellspacing="0" style="background-color:whitesmoke; width:100%">
            <tr>
                <td>
                    <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="tlbCodifica" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                        <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                        <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
	                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
	                    <ClientSideEvents InitializeToolbar="InitTlb" Click="ClickTlb"></ClientSideEvents>
	                    <Items>
		                    <igtbar:TBarButton Key="btnSalva" Text="Salva" DisabledImage="~/Images/salva_dis.gif" Image="~/Images/salva.gif"
                                ToolTip="Salva le modifiche effettuate e chiude la pop-up di elenco codifiche">
		                    </igtbar:TBarButton>
		                    <igtbar:TBarButton Key="btnAnnulla" Text="Annulla" DisabledImage="~/Images/annulla_dis.gif" Image="~/Images/annulla.gif"
                                ToolTip="Annulla le modifiche effettuate e chiude la pop-up di elenco codifiche">
		                    </igtbar:TBarButton>
                        </Items>
                    </igtbar:UltraWebToolbar>
                </td>
            </tr>
            <tr>
                <td>
                    <div class="Title" id="divTitolo" style="width: 100%; text-align:center">
					    <asp:Label id="lblTitolo" runat="server" ></asp:Label>
				    </div>
                </td>
            </tr>
            <tr>
                <td style="text-align:left">
                    <asp:Panel id="pnlSezioneDettagli" runat="server" CssClass="sezione" Width="100%">
						<asp:Label id="lblSezioneDettagli" runat="server">ELENCO CODIFICHE ESTERNE VACCINAZIONE</asp:Label>
					</asp:Panel>
                </td>
            </tr>
        </table>
    </div>

    <dyp:DynamicPanel ID="dypCodifiche" runat="server" Width="100%" Height="410px" ScrollBars="Auto">
		<asp:DataGrid id="dgrCodifiche" runat="server" CssClass="datagrid" Width="100%" ShowFooter="false"
			AllowSorting="True" AutoGenerateColumns="False" CellPadding="1" GridLines="None">
			<SelectedItemStyle CssClass="selected"></SelectedItemStyle>
			<AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
			<ItemStyle CssClass="item"></ItemStyle>
			<HeaderStyle Font-Bold="True" HorizontalAlign="Left" CssClass="header"></HeaderStyle>
			<Columns>
                <asp:TemplateColumn HeaderText="Associazione" HeaderStyle-Width="50%">
                    <ItemTemplate>
                        <asp:Label ID="lblAssociazione" runat="server"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="" HeaderStyle-Width="15%">
                   <ItemTemplate>
                        <asp:Label ID="lblObsoleto" runat="server" CssClass="label_left obsoleto" Visible='<%# Eval("IsAssociazioneObsoleta") %>' Text="OBSOLETA"></asp:Label>
                   </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Codice esterno" HeaderStyle-Width="15%">
                    <ItemTemplate>
                        <asp:TextBox ID="txtCodiceEsterno" runat="server" MaxLength="2"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:BoundColumn DataField="CodiceAssociazione" HeaderText="Codice Associazione" HeaderStyle-Width="20%" Visible="false">
                </asp:BoundColumn>
            </Columns>
        </asp:DataGrid>
    </dyp:DynamicPanel>

</on_lay3:onitlayout3>
