<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CodiciSTS.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.CodiciSTS" %>

<!-- inclusione registri griglie -->
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_dgr" Namespace="Onit.Controls.OnitGrid" Assembly="Onit.Controls.OnitGrid" %>
<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>
<%@ Register TagPrefix="on_otb" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitTable" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title></title>
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>

    <script type="text/javascript">
        $(document).ready(function () {
        });

        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;
            switch (button.Key) {
                //case "btnElimina":
                //    if (!confirm("Confermare eliminazione?")) {
                //        evnt.needPostBack = false;
                //    }

                case "btnSalva":
                    codice = document.getElementById('txtCodice').value;
                    descrizione = document.getElementById('txtDescrizione').value;

                    if (codice == "" || descrizione == "") {
                        alert('Attenzione: alcuni campi obbligatori non sono impostati correttamente!');
                        evnt.needPostBack = false;
                    }

                case "btnCerca":
                    filtro = document.getElementById("txtFiltro").textContent

                    if (filtro = "") {
                        alert('Attenzione: il filtro di ricerca è vuoto!');
                        evnt.needPostBack = false;
                    }
            }
        }
 
        function setVisibility() {
            var div = document.getElementById('divFiltri');
            var hid = document.getElementById('hidFiltriVisibility');

            if (div != null && hid != null) {
                div.style.display = hid.value;
            }

            resizeAllDynamicPanels();
        }

        function toggleVisibility(idDiv, idHid) {
            var div = document.getElementById(idDiv);
            var hid = document.getElementById(idHid);

            if (div != null && hid != null) {
                if (hid.value == 'block') {
                    hid.value = 'none';
                }
                else {
                    hid.value = 'block';
                }

                div.style.display = hid.value;
            }

            resizeAllDynamicPanels();
        }
    </script>

    <style type="text/css">
        .dettaglio {
            position: absolute;
            bottom: 0;
            width: 100%;
        }

         .clickable {
            cursor: pointer;
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">
       <on_lay3:onitlayout3 id="OnitLayout31" runat="server" width="100%" height="100%" TitleCssClass="Title3" Titolo="Codici STS">
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
    					<igtbar:TBarButton Key="btnElimina" Text="Elimina" DisabledImage="~/Images/Elimina_dis.gif" Image="~/Images/Elimina.gif" Visible="false"></igtbar:TBarButton>
					    <igtbar:TBSeparator></igtbar:TBSeparator>
					    <igtbar:TBarButton Key="btnSalva" Text="Salva" DisabledImage="~/Images/salva_dis.gif" Image="~/Images/salva.gif" ></igtbar:TBarButton>
					    <igtbar:TBarButton Key="btnAnnulla" Text="Annulla" DisabledImage="~/Images/annulla_dis.gif" Image="~/Images/annulla.gif" ></igtbar:TBarButton>
				    </Items>
			    </igtbar:UltraWebToolbar>
            </div>

            <div class="vac-sezione clickable" onclick="toggleVisibility('divFiltri', 'hidFiltriVisibility');" title="Mostra/nasconde i filtri di ricerca">Filtri di ricerca</div>
           <div id="divFiltri" style="display: block; width: 100%">
                <asp:HiddenField ID="hidFiltriVisibility" runat="server" Value="block" />
                <table cellspacing="10" cellpadding="0" width="100%" border="0" style="table-layout:fixed">
			        <tr>
				        <td class="label" width="80" style="text-align: left !important">
                            <asp:Label id="lblFiltro" runat="server" Text="<%$ Resources: Onit.OnAssistnet.OnVac.Web, FiltroDiRicerca %>"></asp:Label></td>
				        <td>
					        <asp:TextBox id="txtFiltro" runat="server" width="350" CssClass="TextBox_Stringa"></asp:TextBox>
				        </td>                 
                   </tr>
		        </table>
            </div>

			<div class="vac-sezione">Elenco</div>
            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="30%" ScrollBars="Auto" RememberScrollPosition="true" DynamicHeight="58%">
                <asp:DataGrid ID="dgrCodiciSts" runat="server" Width="100%" AutoGenerateColumns="False" 
                    AllowCustomPaging="true" AllowPaging="true" PageSize="25">
                    <AlternatingItemStyle CssClass="alternating"></AlternatingItemStyle>
                    <ItemStyle CssClass="item"></ItemStyle>
                    <SelectedItemStyle CssClass="selected"></SelectedItemStyle>
                    <HeaderStyle CssClass="header"></HeaderStyle>
                    <PagerStyle CssClass="pager" Position="TopAndBottom" Mode="NumericPages" />

                    <Columns>
                        <on_dgr:SelectorColumn HeaderStyle-Width="4%">
                            <ItemStyle HorizontalAlign="Center"/>
                        </on_dgr:SelectorColumn>
                        
                        <asp:BoundColumn DataField="Id" Visible="false" />
                        <asp:BoundColumn DataField="CodiceSts" HeaderText="Codice" HeaderStyle-Width="12%"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Descrizione" HeaderText="Descrizione" HeaderStyle-Width="30%"></asp:BoundColumn> 
                        <asp:TemplateColumn HeaderText="Data Inizio Validità"
                            SortExpression="DataInizioValidita">
                            <ItemTemplate>
                                <asp:Label ID="lblDataInizioValidita" CssClass="label_left" runat="server"
                                    Text='<%# DateTimeToString(Eval("DataInizioValidita")) %>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Data Fine Validità"
                            SortExpression="DataFineValidita">
                            <ItemTemplate>
                                <asp:Label ID="lblDataFineValidita" CssClass="label_left" runat="server"
                                    Text='<%# DateTimeToString(Eval("DataFineValidita")) %>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="Indirizzo" HeaderText="Indirizzo" HeaderStyle-Width="16%"></asp:BoundColumn>      
                        <asp:BoundColumn DataField="CodiceAsl" HeaderText="Codice ASL" HeaderStyle-Width="8%"></asp:BoundColumn>  
<%--                        <asp:BoundColumn DataField="DescrizioneAsl" HeaderText="Descrizione ASL" HeaderStyle-Width="8%" Visible="false"></asp:BoundColumn> --%>
                        <asp:BoundColumn DataField="CodiceComune" HeaderText="Codice Comune" HeaderStyle-Width="8%" Visible="false"></asp:BoundColumn>      
                        <asp:BoundColumn DataField="DescrizioneComune" HeaderText="Comune" HeaderStyle-Width="8%"></asp:BoundColumn>      
                    </Columns>
                </asp:DataGrid>
            </dyp:DynamicPanel>

            <div class="vac-sezione">Dettaglio</div>
            <div style="width:100%">
                <table style="width:100%; padding-top:5px;">
                    <colgroup>
                        <col width="12%" />
                        <col width="35%" />
                        <col width="12%" />
                        <col width="44%" />
                    </colgroup>
                    <tr>
                        <td class="label">Codice</td>
                        <td>
                            <asp:TextBox runat="server" ID="txtCodice" CssClass="TextBox_Stringa uppercase" style="width:100%" MaxLength="8" ></asp:TextBox>
                        </td>
                        <td class="label">Descrizione</td>
                        <td>
                            <asp:TextBox runat="server" ID="txtDescrizione" CssClass="TextBox_Stringa uppercase" style="width:100%" MaxLength="200"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="label">Indirizzo</td>
                        <td colspan="3">
                            <asp:TextBox runat="server" ID="txtIndirizzo" CssClass="TextBox_Stringa uppercase" style="width:100%" MaxLength="50"></asp:TextBox>
                        </td>                      
                    </tr>
                    <tr>
                        <td class="label">ASL</td>
                        <td>
                            <on_ofm:OnitModalList id="omlAsl" runat="server" PosizionamentoFacile="False" LabelWidth="-1px"
							    CodiceWidth="30%" Label="ASL" CampoCodice="USL_CODICE" CampoDescrizione="USL_DESCRIZIONE"
							    Tabella="T_ANA_USL" UseCode="True" Obbligatorio="False" SetUpperCase="True" Width="70%"></on_ofm:OnitModalList>        
                        </td>
                        <td class="label">Comune</td>
                        <td>
						    <on_ofm:OnitModalList id="omlComune" runat="server" PosizionamentoFacile="False" LabelWidth="-8px"
							    CodiceWidth="30%" Label="Comune" CampoCodice="COM_CODICE" CampoDescrizione="COM_DESCRIZIONE"
							    Tabella="T_ANA_COMUNI" UseCode="True" Obbligatorio="False" SetUpperCase="True" Width="70%"></on_ofm:OnitModalList>
                        </td>                     
                    </tr>
                    <tr>
                        <td class="label">Inizio Validità</td>
                        <td>
                            <on_val:onitdatepick id="odpDataInizioValidita" runat="server" Height="20px" Width="150px" CssClass="textbox_data" DateBox="True"></on_val:onitdatepick>
                        </td>
                        <td class="label">Fine Validità</td>
                        <td>
                            <on_val:onitdatepick id="odpDataFineValidita" runat="server" Height="20px" Width="150px" CssClass="textbox_data" DateBox="True"></on_val:onitdatepick>                                        
                        </td>
                    </tr>
                </table>
            </div>
        </on_lay3:onitlayout3>
    </form>
</body>
</html>