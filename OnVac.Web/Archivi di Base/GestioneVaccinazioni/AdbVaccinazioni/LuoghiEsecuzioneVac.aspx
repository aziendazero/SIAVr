<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="LuoghiEsecuzioneVac.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.LuoghiEsecuzioneVac" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title></title>
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <script type="text/javascript">
        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;
            switch (button.Key) {
                case "btnElimina":
                    if (!confirm("Confermare eliminazione (tutti i campi obbligatori associati al luogo corrente saranno eliminati)?")) {
                        evnt.needPostBack = false;
                    }

                case "btnSalva":
                    ordine = document.getElementById('txtOrdine').value;
                    if (isNaN(ordine)) {
                        alert("Inserire un numero nel campo 'Ordine'!");
                        evnt.needPostBack = false;
                    }
            }
        }

        function ClickTlb(t, btn, evnt) {
            evnt.needPostBack = true;
        		
            switch (btn.Key) {
                
                case 'btnSalvaCampiObbligatori':
                    evnt.needPostBack = confirm('ATTENZIONE: salvare le modifiche effettuate?');
                    break;
            
                case 'btnAnnullaCampiObbligatori':
                    evnt.needPostBack = confirm('ATTENZIONE: le modifiche effettuate verranno perse. Continuare?');
                    break;
            }
        }
	</script>
    
    <style type="text/css">
        .dataPick_Disabilitato {
    background-color: #EBEBE4;
}
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <on_lay3:onitlayout3 id="OnitLayout31" runat="server" width="100%" height="100%" TitleCssClass="Title3" Titolo="Luoghi Esecuzione Vaccinazioni">
            <div>
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
					<ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
				    <Items>
					    <igtbar:TBarButton Key="btnCerca" Text="Cerca" DisabledImage="~/Images/cerca_dis.gif" Image="~/Images/cerca.gif"></igtbar:TBarButton>
					    <igtbar:TBSeparator></igtbar:TBSeparator>
					    <igtbar:TBarButton Key="btnNuovo" Text="Nuovo" DisabledImage="~/Images/Nuovo_dis.gif" Image="~/Images/Nuovo.gif" ></igtbar:TBarButton>
					    <igtbar:TBarButton Key="btnModifica" Text="Modifica" DisabledImage="~/Images/Modifica_dis.gif" Image="~/Images/Modifica.gif" ></igtbar:TBarButton>
					    <igtbar:TBarButton Key="btnElimina" Text="Elimina" DisabledImage="~/Images/Elimina_dis.gif" Image="~/Images/Elimina.gif" ></igtbar:TBarButton>
					    <igtbar:TBSeparator></igtbar:TBSeparator>
					    <igtbar:TBarButton Key="btnSalva" Text="Salva" DisabledImage="~/Images/salva_dis.gif" Image="~/Images/salva.gif" ></igtbar:TBarButton>
					    <igtbar:TBarButton Key="btnAnnulla" Text="Annulla" DisabledImage="~/Images/annulla_dis.gif" Image="~/Images/annulla.gif" ></igtbar:TBarButton>
                        <igtbar:TBSeparator />
                        <igtbar:TBarButton Key="btnCampiObbligatori" Text="Campi Obbligatori" DisabledImage="~/Images/annotazioni.gif" Image="~/Images/annotazioni.gif">
                            <DefaultStyle CssClass="infratoolbar_button_default" Width="140px"></DefaultStyle>
                        </igtbar:TBarButton>
                        <igtbar:TBSeparator />
                        <igtbar:TBarButton Key="btnTipiErogatore" Text="Tipi Erogatore" DisabledImage="~/Images/annotazioni.gif" Image="~/Images/annotazioni.gif">
                            <DefaultStyle CssClass="infratoolbar_button_default" Width="140px"></DefaultStyle>
                        </igtbar:TBarButton>
				    </Items>
			    </igtbar:UltraWebToolbar>
            </div>
            <div class="vac-sezione">Risultati di ricerca</div>
            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
                <asp:DataGrid ID="dgrLuoghiEsecuzioneVac" runat="server" Width="100%" AutoGenerateColumns="False" OnSelectedIndexChanged="dgrLuoghiEsecuzioneVac_SelectedIndexChanged">
                    <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
                    <ItemStyle CssClass="item"></ItemStyle>
                    <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
                    <HeaderStyle CssClass="header"></HeaderStyle>
                    <PagerStyle CssClass="pager" Position="TopAndBottom" Mode="NumericPages" />
                    <Columns>
                        <on_dgr:SelectorColumn HeaderStyle-Width="3%">
                            <ItemStyle HorizontalAlign="Center"/>
                        </on_dgr:SelectorColumn>
                        <asp:BoundColumn DataField="Codice" HeaderText="Codice" HeaderStyle-Width="12%"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Descrizione" HeaderText="Descrizione" HeaderStyle-Width="50%"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Tipo" HeaderText="Tipo" HeaderStyle-Width="15%"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Ordine" HeaderText="Ordine" HeaderStyle-Width="5%"></asp:BoundColumn>
                        <asp:TemplateColumn HeaderText="Obsoleto" HeaderStyle-Width="5%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblObsoleto" Text='<%# GetStringBoolean(Eval("Obsoleto")) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Estrai AVN" HeaderStyle-Width="10%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblEstraiAvn" Text='<%# GetStringBoolean(Eval("FlagEstraiAvn")) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>
            </dyp:DynamicPanel>
            <div class="vac-sezione">Dettaglio</div>
            <div>
                <table style="width:100%; padding-top:5px;">
                    <colgroup>
                        <col style="width: 15%" />
                        <col style="width: 20%" />
                        <col style="width: 15%" />
                        <col style="width: 15%" />
                        <col style="width: 10%" />
                        <col style="width: 5%" />
                        <col style="width: 10%" />
                        <col style="width: 10%" />
                    </colgroup>
                    <tr>
                        <td class="label">Codice</td>
                        <td>
                            <asp:TextBox runat="server" ID="txtCodice" CssClass="TextBox_Stringa uppercase" style="width:100%" MaxLength="2"></asp:TextBox>
                        </td>
                        <td class="label">Descrizione</td>
                        <td colspan="5">
                            <asp:TextBox runat="server" ID="txtDescrizione" CssClass="TextBox_Stringa uppercase" style="width:100%" MaxLength="20"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="label">Tipo</td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlTipo" CssClass="TextBox_Stringa uppercase" style="width:100%">
                            </asp:DropDownList>
                        </td>
                        <td class="label">Ordine</td>
                        <td>
                            <asp:TextBox runat="server" ID="txtOrdine" CssClass="TextBox_Stringa uppercase" style="width:100%" MaxLength="2"></asp:TextBox>
                        </td>                                                
                        <td class="label">Obsoleto</td>
                        <td>
                            <asp:CheckBox runat="server" ID="chkObsoleto" />
                        </td>                    
					    <td class="label">Estrai AVN</td>
                        <td>
                            <asp:CheckBox runat="server" ID="chkEstraiAvn" />
                        </td>
                    </tr>
                </table>
            </div>
        </on_lay3:onitlayout3>

        <on_ofm:OnitFinestraModale ID="fmCampiObbligatori" runat="server" BackColor="LightGray" NoRenderX="true" Height="400" Width="400">    
            
            <div>
                <table border="0" cellpadding="0" cellspacing="0" style="background-color:whitesmoke; width:100%">
                    <tr>
                        <td>
                            <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="tlbCampiObbligatori" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                                <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                                <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
	                            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
	                            <ClientSideEvents InitializeToolbar="InitTlb" Click="ClickTlb"></ClientSideEvents>
	                            <Items>
		                            <igtbar:TBarButton Key="btnSalvaCampiObbligatori" Text="Salva" DisabledImage="~/Images/salva_dis.gif" Image="~/Images/salva.gif"
                                        ToolTip="Salva le modifiche effettuate e chiude la pop-up dei campi obbligatori">
		                            </igtbar:TBarButton>
		                            <igtbar:TBarButton Key="btnAnnullaCampiObbligatori" Text="Annulla" DisabledImage="~/Images/annulla_dis.gif" Image="~/Images/annulla.gif"
                                        ToolTip="Annulla le modifiche effettuate e chiude la pop-up dei campi obbligatori">
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
                            <asp:Panel id="pnlSezioneDettagli" runat="server" CssClass="vac-sezione">
						        <asp:Label id="lblSezioneDettagli" runat="server">CAMPI OBBLIGATORI</asp:Label>
					        </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>

            <dyp:DynamicPanel ID="dypCampiObbligatori" runat="server" DynamicWidth="100%" DynamicHeight="50%" CssClass="dypScroll" ScrollBars="Auto">
		        <asp:DataGrid id="dgrCampiObbligatori" runat="server" CssClass="datagrid" Width="100%" ShowFooter="false" 
			        AllowSorting="True" AutoGenerateColumns="False" CellPadding="1" GridLines="None" >
			        <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
			        <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
			        <ItemStyle CssClass="item"></ItemStyle>
			        <HeaderStyle Font-Bold="True" HorizontalAlign="Left" CssClass="header"></HeaderStyle>
			        <Columns>
                        <asp:TemplateColumn HeaderText="" HeaderStyle-Width="5%">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkObblPerLuogo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="CodCampo" HeaderText="Codice" Visible="false">
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="DescrizCampo" HeaderText="Descrizione" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="75%">
                        </asp:BoundColumn>
                        <asp:TemplateColumn HeaderText="Data Inizio Validità" HeaderStyle-Width="20%">
                            <ItemTemplate>
								<on_val:onitdatepick id="dpkDataInizioValidita" runat="server" Height="20px" Width="120px" CssClass="textbox_data" DateBox="True"></on_val:onitdatepick>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>
            </dyp:DynamicPanel>

        </on_ofm:OnitFinestraModale>

        <on_ofm:OnitFinestraModale ID="fmTipoErogatore" runat="server" BackColor="LightGray" NoRenderX="true" Height="400" Width="400">    
            <div>
                <table border="0" cellpadding="0" cellspacing="0" style="background-color:whitesmoke; width:100%">
                    <tr>
                        <td>
                            <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="tlbTipoErogatore" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                                <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                                <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
	                            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
	                            <ClientSideEvents InitializeToolbar="InitTlb" Click="ClickTlb"></ClientSideEvents>
	                            <Items>
		                            <igtbar:TBarButton Key="btnSalvaTipoErogatore" Text="Salva" DisabledImage="~/Images/salva_dis.gif" Image="~/Images/salva.gif"
                                        ToolTip="Salva le modifiche effettuate e chiude la pop-up dei tipi erogatore">
		                            </igtbar:TBarButton>
		                            <igtbar:TBarButton Key="btnAnnullaTipoErogatore" Text="Annulla" DisabledImage="~/Images/annulla_dis.gif" Image="~/Images/annulla.gif"
                                        ToolTip="Annulla le modifiche effettuate e chiude la pop-up dei tipo erogatore">
		                            </igtbar:TBarButton>
                                </Items>
                            </igtbar:UltraWebToolbar>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div class="Title" id="divTitoloTipoErogatore" style="width: 100%; text-align:center">
					            <asp:Label id="Label1" runat="server" ></asp:Label>
				            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align:left">
                            <asp:Panel id="Panel1" runat="server" CssClass="vac-sezione">
						        <asp:Label id="Label2" runat="server">TIPI EROGATORE</asp:Label>
					        </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>
            <dyp:DynamicPanel ID="dypTipoEgogatore" runat="server" DynamicWidth="100%" DynamicHeight="50%" CssClass="dypScroll" ScrollBars="Auto">
		        <asp:DataGrid id="dgrTipoErogatore" runat="server" CssClass="datagrid" Width="100%" ShowFooter="false" 
			        AllowSorting="True" AutoGenerateColumns="False" CellPadding="1" GridLines="None" >
			        <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
			        <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
			        <ItemStyle CssClass="item"></ItemStyle>
			        <HeaderStyle Font-Bold="True" HorizontalAlign="Left" CssClass="header"></HeaderStyle>
			        <Columns>
                        <asp:TemplateColumn HeaderText="" HeaderStyle-Width="5%">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkTipoErogatorePerLuogo" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="Id" HeaderText="" Visible="false">
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="Codice" HeaderText="Codice" Visible="true">
                        </asp:BoundColumn>
                        <asp:BoundColumn DataField="Descrizione" HeaderText="Descrizione" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="75%">
                        </asp:BoundColumn>                        
                    </Columns>
                </asp:DataGrid>
            </dyp:DynamicPanel>
        </on_ofm:OnitFinestraModale>
    </form>
</body>
</html>