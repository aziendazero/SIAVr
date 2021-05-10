<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Comuni3.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.Comuni3" %>

<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>
<%@ Register TagPrefix="cc1" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitTable" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>Comuni</title>
    
    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
    <script type="text/javascript">

        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;
            switch (button.Key) {
                case "btnSave":
                    if (!datiValidi()) {
                        evnt.needPostBack = false;
                        break;
                    }
                default:
                    evnt.needPostBack = true;
            }
        }

        function datiValidi() {

            if (!isValidFinestraModale('WzFmComune', true)) {
                alert("Non è possibile avere comune non valorizzato. Salvataggio non effettuato!");
                return false;
            }
            return true;
        }

    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Titolo="Comuni USL" Width="100%" Height="100%">
            <ondp:onitdatapanel id="PanComuni3" runat="server" defaultsort="connessione.T_ANA_COMUNI.COM_DESCRIZIONE"
                defaultsort-length="40" maxrecord="200" maxrecord-length="2" renderonlychildren="True" configfile="Comuni3.PanComuni3.xml">
			    <cc1:OnitTable id="OnitTable1" runat="server" height="100%" width="100%">
				    <cc1:OnitSection id="OnitSection1" runat="server" width="100%" TypeHeight="Content">
					    <cc1:OnitCell id="OnitCell1" runat="server" height="100%" width="100%" TypeScroll="Hidden">
					        <asp:Label id="LayoutTitolo" runat="server" Width="100%" CssClass="Title"></asp:Label>
                            <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
							    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                                <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                                <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                                <ClientSideEvents Click="ToolBarClick" InitializeToolbar="InizializzaToolBar"></ClientSideEvents>
							    <Items>
								    <igtbar:TBarButton Key="btnNew" Text="Nuovo" Image="~/Images/nuovo.gif"></igtbar:TBarButton>
								    <igtbar:TBarButton Key="btnDelete" Text="Elimina" Image="~/Images/elimina.gif"></igtbar:TBarButton>
								    <igtbar:TBSeparator></igtbar:TBSeparator>
								    <igtbar:TBarButton Key="btnSave" Text="Salva" Image="~/Images/salva.gif"></igtbar:TBarButton>
								    <igtbar:TBarButton Key="btnCancel" Text="Annulla" Image="~/Images/annulla.gif"></igtbar:TBarButton>
								    <igtbar:TBSeparator></igtbar:TBSeparator>
								    <igtbar:TBarButton Key="btnBack" Text="Indietro" Image="~/Images/indietro.gif"></igtbar:TBarButton>
							    </Items>
						    </igtbar:UltraWebToolbar>
						    <div class="Sezione">Elenco
							    <asp:DropDownList id="ddlPaging" runat="server" Width="80%"></asp:DropDownList></div>
					    </cc1:OnitCell>
				    </cc1:OnitSection>
				    <cc1:OnitSection id="Onitsection2" runat="server" width="100%" TypeHeight="Calculate">
					    <cc1:OnitCell id="Onitcell2" runat="server" height="100%" width="100%" TypeScroll="Auto">
						    <ondp:wzDataGrid Browser="UpLevel" id="dgrComuni3" runat="server" Width="100%" Height="100%" OnitStyle="False" PagerDropDownList="ddlPaging"
							    PagerVoicesAfter="10" PagerVoicesBefore="10" PagingMode="Auto" SortingMode="Auto" EditMode="None">
							    <Bands>
								    <igtbl:UltraGridBand>
									    <Columns>
										    <igtbl:UltraGridColumn HeaderText="CODICE" Key="" Width="15%" BaseColumnName=""></igtbl:UltraGridColumn>
										    <igtbl:UltraGridColumn HeaderText="DESCRIZIONE" Key="" Width="30%" BaseColumnName=""></igtbl:UltraGridColumn>
										    <igtbl:UltraGridColumn HeaderText="PROVINCIA" Key="" Width="15%" BaseColumnName=""></igtbl:UltraGridColumn>
										    <igtbl:UltraGridColumn HeaderText="REGIONE" Key="" Width="20%" BaseColumnName=""></igtbl:UltraGridColumn>
										    <igtbl:UltraGridColumn HeaderText="SCADENZA" Key="" Width="20%" Format="dd/MM/yyyy" BaseColumnName=""></igtbl:UltraGridColumn>
									    </Columns>
								    </igtbl:UltraGridBand>
							    </Bands>
							    <DisplayLayout AutoGenerateColumns="False" AllowSortingDefault="Yes" RowHeightDefault="26px" Version="2.00"
								    SelectTypeRowDefault="Single" ScrollBar="Never" BorderCollapseDefault="Separate" RowSelectorsDefault="No"
								    Name="dgrComuni3" CellClickActionDefault="RowSelect">
								    <HeaderStyleDefault HorizontalAlign="Left" CssClass="Infra2Dgr_Header"></HeaderStyleDefault>
								    <RowSelectorStyleDefault CssClass="Infra2Dgr_RowSelector"></RowSelectorStyleDefault>
								    <FrameStyle Width="100%"></FrameStyle>
								    <ActivationObject BorderWidth="0px" BorderColor="Navy"></ActivationObject>
								    <SelectedRowStyleDefault CssClass="Infra2Dgr_SelectedRow"></SelectedRowStyleDefault>
								    <RowAlternateStyleDefault CssClass="Infra2Dgr_RowAlternate"></RowAlternateStyleDefault>
								    <RowStyleDefault CssClass="Infra2Dgr_Row"></RowStyleDefault>
								    <ClientSideEvents InitializeLayoutHandler="getId" ></ClientSideEvents>
								    <ImageUrls SortDescending="\ig_common\images\ig_tblSortDesc_white.gif" SortAscending="\ig_common\images\ig_tblSortAsc_white.gif"></ImageUrls>
							    </DisplayLayout>
							    <BindingColumns>
								    <ondp:BindingFieldValue Value="" Editable="always" Description="CODICE" Connection="connessione" SourceTable="T_ANA_LINK_COMUNI_USL"
									    Hidden="False" SourceField="LCU_COM_CODICE"></ondp:BindingFieldValue>
								    <ondp:BindingFieldValue Value="" Editable="always" Description="DESCRIZIONE" Connection="connessione" SourceTable="T_ANA_COMUNI"
									    Hidden="False" SourceField="COM_DESCRIZIONE"></ondp:BindingFieldValue>
								    <ondp:BindingFieldValue Value="" Editable="always" Description="PROVINCIA" Connection="connessione" SourceTable="T_ANA_COMUNI"
									    Hidden="False" SourceField="COM_PROVINCIA"></ondp:BindingFieldValue>
								    <ondp:BindingFieldValue Value="" Editable="always" Description="REG_DESCRIZIONE" Connection="connessione"
									    SourceTable="T_ANA_REGIONI" Hidden="False" SourceField="REG_DESCRIZIONE"></ondp:BindingFieldValue>
								    <ondp:BindingFieldValue Value="" Editable="always" Description="DATA_SCADENZA" Connection="connessione"
									    SourceTable="T_ANA_COMUNI" Hidden="False" SourceField="COM_SCADENZA"></ondp:BindingFieldValue>
								    <ondp:BindingFieldValue Value="" Editable="always" Description="CODICE_USL" Connection="connessione" SourceTable="T_ANA_LINK_COMUNI_USL"
									    Hidden="True" SourceField="LCU_USL_CODICE"></ondp:BindingFieldValue>
							    </BindingColumns>
						    </ondp:wzDataGrid>
					    </cc1:OnitCell>
				    </cc1:OnitSection>
				    <cc1:OnitSection id="Onitsection3" runat="server" width="100%" TypeHeight="Content">
					    <cc1:OnitCell id="Onitcell3" runat="server" height="100%" width="100%" TypeScroll="Hidden">
						    <div class="Sezione">Dettaglio</div>
						    <table id="Table1" style="table-layout: fixed" cellspacing="5" cellpadding="0" width="100%" border="0">
							    <tr>
								    <td class="label right" style="width: 100px;">USL&nbsp;</td>
								    <td style="width: 300px;">
									    <ondp:wzFinestraModale id="WzFmUSL" runat="server" Width="80%" UseCode="True" BindingDescription-Hidden="False"
										    BindingDescription-SourceTable="T_ANA_USL" BindingDescription-Editable="never" BindingCode-Hidden="False"
										    BindingCode-SourceTable="T_ANA_LINK_COMUNI_USL" BindingCode-Editable="never" CampoDescrizione="USL_DESCRIZIONE as DESCRIZIONE"
										    CampoCodice="USL_CODICE as CODICE" MaxRecords="0" PageSize="50" Paging="True" SetUpperCase="True" Sorting="True"
										    Tabella="T_ANA_USL" Label="" CodiceWidth="20%" LabelWidth="-1px" PosizionamentoFacile="False" BindingCode-SourceField="LCU_USL_CODICE"
										    BindingDescription-SourceField="USL_DESCRIZIONE" BindingCode-Connection="connessione" BindingDescription-Connection="connessione" UseAllResultCodeIfEqual="true" UseAllResultDescIfEqual="true"></ondp:wzFinestraModale></td>
                                    <td>&nbsp;</td>
							    </tr>
							    <tr>
								    <td class="label right" style="width: 100px;">Comune&nbsp;</td>
								    <td style="width: 300px;">
									    <ondp:wzFinestraModale id="WzFmComune" runat="server" Width="80%" UseCode="True" BindingDescription-Hidden="False"
										    BindingDescription-SourceTable="T_ANA_COMUNI" BindingDescription-Editable="onNew" BindingCode-Hidden="False"
										    BindingCode-SourceTable="T_ANA_LINK_COMUNI_USL" BindingCode-Editable="onNew" CampoDescrizione="COM_DESCRIZIONE as DESCRIZIONE"
										    CampoCodice="COM_CODICE as CODICE" MaxRecords="0" PageSize="50" Paging="True" SetUpperCase="True" Sorting="True"
										    Tabella=" T_ANA_COMUNI" Label="" CodiceWidth="20%" LabelWidth="-1px" PosizionamentoFacile="False"
										    BindingCode-SourceField="LCU_COM_CODICE" BindingDescription-SourceField="COM_DESCRIZIONE" BindingCode-Connection="connessione"
										    BindingDescription-Connection="connessione"  UseAllResultCodeIfEqual="true" UseAllResultDescIfEqual="true"></ondp:wzFinestraModale></td>
                                    <td>&nbsp;</td>
							    </tr>
						    </table>
					    </cc1:OnitCell>
				    </cc1:OnitSection>
			    </cc1:OnitTable>
		    </ondp:onitdatapanel>
        </on_lay3:OnitLayout3>
    </form>
</body>
</html>
