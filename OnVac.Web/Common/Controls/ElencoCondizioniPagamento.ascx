<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ElencoCondizioniPagamento.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.ElencoCondizioniPagamento" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<%@ Register TagPrefix="uc1" TagName="CampiEtaAttivazione" Src="./CampiEtaAttivazione.ascx" %>

<script type="text/javascript">
    function InitTlbCondizioni(t) {
        t.PostBackButton = false;
    }

    function ClickTlbCondizioni(t, btn, evnt) {

        evnt.needPostBack = true;
        		
        switch (btn.Key) {

            case 'btnElimina':
                evnt.needPostBack = confirm('ATTENZIONE: l\'elemento selezionato verrà eliminato. L\'operazione è definitiva e non potrà essere annullata.\nContinuare con l\'eliminazione?');
                break;

            case 'btnAnnulla':
                evnt.needPostBack = confirm('ATTENZIONE: le modifiche effettuate verranno perse. Continuare?');
                break;
        }
    }
</script>  

<on_lay3:onitlayout3 id="Onitlayout31" runat="server" TitleCssClass="Title3" Titolo="Condizioni Pagamento" width="100%" height="100%" NAME="Onitlayout31">
    <div>
        <table border="0" cellpadding="2" cellspacing="0" style="background-color:whitesmoke; width:100%">
            <colgroup>
                <col style="width:13%; text-align:right;" />
                <col style="width:37%;" />
                <col style="width:13%; text-align:right;" />
                <col style="width:37%;" />
            </colgroup>
            <tr>
                <td colspan="4">
                    <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="tlbCondizioni" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                        <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                        <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
	                    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
	                    <ClientSideEvents InitializeToolbar="InitTlbCondizioni" Click="ClickTlbCondizioni"></ClientSideEvents>
	                    <Items>
		                    <igtbar:TBarButton Key="btnChiudi" Text="Chiudi" DisabledImage="~/Images/esci_dis.gif" Image="~/Images/esci.gif">
		                    </igtbar:TBarButton>
		                    <igtbar:TBSeparator></igtbar:TBSeparator>
		                    <igtbar:TBarButton Key="btnNuovo" Text="Nuovo" DisabledImage="~/Images/nuovo_dis.gif" Image="~/Images/nuovo.gif">
		                    </igtbar:TBarButton>
                            <igtbar:TBarButton Key="btnModifica" Text="Modifica" DisabledImage="~/Images/Modifica_dis.gif" Image="~/Images/Modifica.gif">
		                    </igtbar:TBarButton>
                            <igtbar:TBarButton Key="btnElimina" Text="Elimina" DisabledImage="~/Images/elimina_dis.gif" Image="~/Images/elimina.gif">
		                    </igtbar:TBarButton>
                            <igtbar:TBSeparator></igtbar:TBSeparator>
		                    <igtbar:TBarButton Key="btnSalva" Text="Salva" DisabledImage="~/Images/salva_dis.gif" Image="~/Images/salva.gif">
		                    </igtbar:TBarButton>
		                    <igtbar:TBarButton Key="btnAnnulla" Text="Annulla" DisabledImage="~/Images/annulla_dis.gif" Image="~/Images/annulla.gif">
		                    </igtbar:TBarButton>
                        </Items>
                    </igtbar:UltraWebToolbar>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <div class="Title" id="divTitolo" style="width: 100%; text-align:center">
					    <asp:Label id="lblTitolo" runat="server" ></asp:Label>
				    </div>
                </td>
            </tr>
            <tr>
                <td colspan="4" style="text-align:left">
                    <asp:Panel id="pnlSezioneDettagli" runat="server" CssClass="vac-sezione">
						<asp:Label id="lblSezioneDettagli" runat="server">DETTAGLIO</asp:Label>
					</asp:Panel>
                    <asp:HiddenField ID="hidIdCondizione" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="label">
                    <asp:Label ID="lblEtaInizioDett" runat="server" Text="Età inizio"></asp:Label>
                </td>
                <td>
                    <uc1:CampiEtaAttivazione ID="ucEtaInizio" runat="server" LabelCssClass="label_left" TextBoxCssClass="TextBox_Numerico" TextBoxDisabledCssClass="TextBox_Numerico_Disabilitato" />
                </td>
                <td class="label">
                    <asp:Label ID="lblEtaFineDett" runat="server" Text="Età fine"></asp:Label>
                </td>
                <td>
                    <uc1:CampiEtaAttivazione ID="ucEtaFine" runat="server" LabelCssClass="label_left" TextBoxCssClass="TextBox_Numerico" TextBoxDisabledCssClass="TextBox_Numerico_Disabilitato"/>
                </td>
            </tr>
            <tr>
                <td class="label">
                    <asp:Label ID="lblDettEsenzione" runat="server" Text="Cod. Esenzione" ></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlEsenzione" runat="server" CssClass="TextBox_Stringa" Width="100%"></asp:DropDownList>
                </td>
                <td class="label">
                    <asp:Label ID="lblDettImporto" runat="server" Text="Importo"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlImporto" runat="server" CssClass="TextBox_Stringa" Width="100%"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td></td>
                <td colspan="3" class="label_left">
                    <asp:CheckBox ID="chkAutoImporto" runat="server" Text="Impostazione automatica dell'importo in fase di esecuzione delle vaccinazioni" TextAlign="Right" 
                        style="position: relative; left: -4px;"/>
                </td>
            </tr>
            <tr>
                <td colspan="4" style="text-align:left">
                    <asp:Panel id="pnlSezioneCondizioni" runat="server" CssClass="vac-sezione">
				        <asp:Label id="lblSezioneCondizioni" runat="server">ELENCO CONDIZIONI DI PAGAMENTO</asp:Label>
			        </asp:Panel>
                </td>
            </tr>
        </table>
    </div>
    
    <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">

		<asp:DataGrid id="dgrCondizioni" runat="server" CssClass="datagrid" Width="100%" ShowFooter="false"
			AllowSorting="True" AutoGenerateColumns="False" CellPadding="1" GridLines="None">
			<SelectedItemStyle CssClass="selected"></SelectedItemStyle>
			<AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
			<ItemStyle CssClass="item"></ItemStyle>
			<HeaderStyle Font-Bold="True" CssClass="header"></HeaderStyle>
            <Columns>
                <asp:TemplateColumn>
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CommandName="Select">
                            <img runat="server" src='~/Images/seleziona.gif' title='seleziona' />
                        </asp:LinkButton>
                    </ItemTemplate>
                    <ItemStyle Width="3%" HorizontalAlign="Center" />
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Eta inizio">
                    <ItemStyle Width="22%" />
                    <ItemTemplate>
                        <asp:Label ID="lblEtaInizio" runat="server"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Eta fine">
                    <ItemStyle Width="22%" />
                    <ItemTemplate>
                        <asp:Label ID="lblEtaFine" runat="server"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Cod. esenzione">
                    <ItemStyle Width="16%" />
                    <ItemTemplate>
                        <asp:Label ID="lblCodEsenzione" runat="server"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Importo">
                    <ItemStyle Width="16%" />
                    <ItemTemplate>
                        <asp:Label ID="lblImporto" runat="server"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Caricamento automatico importo">
                    <ItemStyle Width="21%" />
                    <ItemTemplate>
                        <asp:Label ID="lblAutoImporto" runat="server"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:BoundColumn Visible="False" DataField="IdCondizione" ></asp:BoundColumn>
            </Columns>
        </asp:DataGrid>

    </dyp:DynamicPanel>
</on_lay3:onitlayout3>