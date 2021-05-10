<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="USL.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.USL" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="cc1" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitTable" %>
<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>USL</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    
    <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
    <script type="text/javascript">

        $(document).ready(function () {
            $("#tbCodice").live('blur', function () { $(this).val($(this).val().toUpperCase()); });
            $("#tbDesc").live('blur', function () { $(this).val($(this).val().toUpperCase()); });
            $("#tbIndirizzo").live('blur', function () { this.value = (this.value).toUpperCase(); });
            $("#tbCap").live('blur', function () { $(this).val($(this).val().toUpperCase()); });
            $("#tbProvincia").live('blur', function () { $(this).val($(this).val().toUpperCase()); });
            $("#tbTel").live('blur', function () { $(this).val($(this).val().toUpperCase()); });
        });

        function ToolBarClick(ToolBar, button, evnt) {
            switch (button.Key) {
                case "btnSave":
                    evnt.needPostBack = datiValidi();
                    break;
            }
        }

        function datiValidi() {

            var cod = $("#tbCodice").val();
            var desc = $("#tbDesc").val();

            if ($.trim(cod) == "" || $.trim(desc) == "") {
                alert("Non è possibile avere Codice o Descrizione non valorizzati. Salvataggio non effettuato!");
                return false;
            }

            if (!isValidFinestraModale('WzFmRegione', false)) {
                return false;
            }

            return true;
        }
    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" Titolo="USL" Width="100%" Height="100%">
            <ondp:onitdatapanel id="PanUSL" runat="server" defaultsort="connessione.T_ANA_USL.USL_DESCRIZIONE"
                defaultsort-length="37" maxrecord="100" maxrecord-length="2" renderonlychildren="True"
                configfile="USL.PanUSL.xml" dontloaddatafirsttime="True">
			    <cc1:OnitTable id="OnitTable1" runat="server" height="100%" width="100%">
				    <cc1:OnitSection id="OnitSection1" runat="server" width="100%" TypeHeight="Content">
					    <cc1:OnitCell id="OnitCell1" runat="server" height="100%" width="100%">
						    <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
							    <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                                <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
                                <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                                <ClientSideEvents Click="ToolBarClick"></ClientSideEvents>
							    <Items>
								    <igtbar:TBarButton Key="btnFind" Text="Cerca" Image="~/Images/cerca.gif"></igtbar:TBarButton>
								    <igtbar:TBSeparator></igtbar:TBSeparator>
								    <igtbar:TBarButton Key="btnNew" Text="Nuovo" Image="~/Images/nuovo.gif"></igtbar:TBarButton>
								    <igtbar:TBarButton Key="btnEdit" Text="Modifica" Image="~/Images/Modifica.gif"></igtbar:TBarButton>
								    <igtbar:TBSeparator></igtbar:TBSeparator>
								    <igtbar:TBarButton Key="btnSave" Text="Salva" Image="~/Images/salva.gif"></igtbar:TBarButton>
								    <igtbar:TBarButton Key="btnCancel" Text="Annulla" Image="~/Images/annulla.gif"></igtbar:TBarButton>
								    <igtbar:TBSeparator></igtbar:TBSeparator>
								    <igtbar:TBarButton Key="btnLinkComuni" Text="Comuni" Image="../../../images/comuniSmall.gif">
									    <DefaultStyle Width="100px" CssClass="infratoolbar_button_default"></DefaultStyle>
								    </igtbar:TBarButton>
							    </Items>
						    </igtbar:UltraWebToolbar>
						    <div class="Sezione">Modulo ricerca</div>
						    <ondp:wzFilter id="tabRicerca" runat="server" Width="100%" height="70px" TablesForIndexSearch="T_ANA_USL" CssClass="InfraUltraWebTab2"  SearchOnlyFields="connessione.T_ANA_USL.USL_CODICE,connessione.T_ANA_USL.USL_DESCRIZIONE">
							    <SelectedTabStyle CssClass="InfraTab_Selected2"></SelectedTabStyle>
							    <HoverTabStyle CssClass="InfraTab_Hover2"></HoverTabStyle>
							    <DefaultTabStyle CssClass="InfraTab_Default2"></DefaultTabStyle>
                                <DisabledTabStyle CssClass="InfraTab_Disabled2"></DisabledTabStyle>
							    <Tabs>
								    <igtab:Tab Text="Ricerca">
									    <ContentTemplate>
										    <table cellSpacing="10" cellPadding="0" style="table-layout:fixed" width="100%" border="0">
											    <tr>
												    <td align="right" style="width: 90px;">
													    <asp:Label id="Label1" runat="server" CssClass="LABEL">Filtro di Ricerca</asp:Label></td>
												    <td>
													    <asp:TextBox id="WzFilterKeyBase" runat="server" Width="100%" cssclass="textbox_stringa"></asp:TextBox></td>
											    </tr>
										    </table>
									    </ContentTemplate>
								    </igtab:Tab>
							    </Tabs>
						    </ondp:wzFilter>
						    <div class="Sezione">Elenco</div>
					    </cc1:OnitCell>
				    </cc1:OnitSection>
				    <cc1:OnitSection id="Onitsection2" runat="server" width="100%" TypeHeight="Calculate">
					    <cc1:OnitCell id="Onitcell2" runat="server" height="100%" width="100%" TypeScroll="Auto">
						    <ondp:wzDataGrid Browser="UpLevel" id="dgrUSL" runat="server" Width="100%" PagerVoicesBefore="-1" PagerVoicesAfter="-1" OnitStyle="False" EditMode="None">
                                <DisplayLayout ColWidthDefault="" AutoGenerateColumns="False" RowHeightDefault="26px" Version="2.00"
								    GridLinesDefault="None" SelectTypeRowDefault="Single" ScrollBar="Never" BorderCollapseDefault="Separate"
								    CellPaddingDefault="3" RowSelectorsDefault="No" Name="dgrComuni" CellClickActionDefault="RowSelect">
								    <HeaderStyleDefault HorizontalAlign="Left" CssClass="Infra2Dgr_Header"></HeaderStyleDefault>
								    <RowSelectorStyleDefault CssClass="Infra2Dgr_RowSelector"></RowSelectorStyleDefault>
								    <FrameStyle Width="100%"></FrameStyle>
								    <ActivationObject BorderWidth="0px" BorderColor="Navy"></ActivationObject>
								    <Images></Images>
								    <SelectedRowStyleDefault CssClass="Infra2Dgr_SelectedRow"></SelectedRowStyleDefault>
								    <RowAlternateStyleDefault CssClass="Infra2Dgr_RowAlternate"></RowAlternateStyleDefault>
								    <RowStyleDefault CssClass="Infra2Dgr_Row"></RowStyleDefault>
							    </DisplayLayout>
							    <Bands>
								    <igtbl:UltraGridBand>
									    <Columns>
										    <igtbl:UltraGridColumn HeaderText="Codice" Key="" BaseColumnName=""></igtbl:UltraGridColumn>
										    <igtbl:UltraGridColumn HeaderText="Descrizione" Key="" BaseColumnName=""></igtbl:UltraGridColumn>
										    <igtbl:UltraGridColumn HeaderText="Indirizzo" Key="" BaseColumnName=""></igtbl:UltraGridColumn>
										    <igtbl:UltraGridColumn HeaderText="CAP" Key="" BaseColumnName=""></igtbl:UltraGridColumn>
										    <igtbl:UltraGridColumn HeaderText="Comune" Key="" BaseColumnName=""></igtbl:UltraGridColumn>
										    <igtbl:UltraGridColumn HeaderText="Provincia" Key="" BaseColumnName=""></igtbl:UltraGridColumn>
									    </Columns>
								    </igtbl:UltraGridBand>
							    </Bands>
							    <BindingColumns>
								    <ondp:BindingFieldValue Value="" Editable="always" Description="CODICE" Connection="connessione" SourceTable="T_ANA_USL"
									    Hidden="False" SourceField="USL_CODICE"></ondp:BindingFieldValue>
								    <ondp:BindingFieldValue Value="" Editable="always" Description="DESCRIZIONE" Connection="connessione" SourceTable="T_ANA_USL"
									    Hidden="False" SourceField="USL_DESCRIZIONE"></ondp:BindingFieldValue>
								    <ondp:BindingFieldValue Value="" Editable="always" Description="INDIRIZZO" Connection="connessione" SourceTable="T_ANA_USL"
									    Hidden="False" SourceField="USL_INDIRIZZO"></ondp:BindingFieldValue>
								    <ondp:BindingFieldValue Value="" Editable="always" Description="CAP" Connection="connessione" SourceTable="T_ANA_USL"
									    Hidden="False" SourceField="USL_CAP"></ondp:BindingFieldValue>
								    <ondp:BindingFieldValue Value="" Editable="always" Description="COMUNE" Connection="connessione" SourceTable="T_ANA_USL" 
                                        Hidden="False" SourceField="USL_CITTA"></ondp:BindingFieldValue>
								    <ondp:BindingFieldValue Value="" Editable="always" Description="PROVINCIA" Connection="connessione" SourceTable="T_ANA_USL"
									    Hidden="False" SourceField="USL_PROVINCIA"></ondp:BindingFieldValue>
							    </BindingColumns>
						    </ondp:wzDataGrid>
					    </cc1:OnitCell>
				    </cc1:OnitSection>
				    <cc1:OnitSection id="Onitsection3" runat="server" width="100%" TypeHeight="Content">
					    <cc1:OnitCell id="Onitcell3" runat="server" height="100%" width="100%" TypeScroll="Hidden">
					        <div class="Sezione">Dettaglio</div>
						    <table id="Table1" style="table-layout: fixed" cellspacing="1" cellpadding="1" width="100%" border="0">
							    <colgroup>
								    <col width="100" />
								    <col width="100" />
								    <col width="100" />
								    <col width="100" />
								    <col width="100" />
                                    <col>
                                    <col width="100" />
								    <col>
							    </colgroup>
							    <tr>
								    <td class="label">Codice</td>
								    <td>
									    <ondp:wzTextBox id="tbCodice" runat="server" width="100%" MaxLength="8" CssStyles-CssDisabled="textbox_stringa_disabilitato"
										    CssStyles-CssEnabled="textbox_stringa" CssStyles-CssRequired="textbox_stringa_obbligatorio"
										    BindingField-Editable="onNew" BindingField-Connection="connessione" BindingField-SourceTable="T_ANA_USL"
										    BindingField-Hidden="False" BindingField-SourceField="USL_CODICE"></ondp:wzTextBox></td>
								    <td class="label">Descrizione</td>
								    <td colspan="5">
									    <ondp:wzTextBox id="tbDesc" runat="server" Width="100%" MaxLength="40" CssStyles-CssDisabled="textbox_stringa_disabilitato"
										    CssStyles-CssEnabled="textbox_stringa" CssStyles-CssRequired="textbox_stringa_obbligatorio"
										    BindingField-Editable="always" BindingField-Connection="connessione" BindingField-SourceTable="T_ANA_USL"
										    BindingField-Hidden="False" BindingField-SourceField="USL_DESCRIZIONE"></ondp:wzTextBox></td>
							    </tr>
							    <tr>
								    <td class="label">Fine Validità</td>
								    <td>
									    <ondp:wzOnitDatePick id="odpFineValidita" runat="server" height="18px" width="100%"
										    CssStyles-CssDisabled="textbox_data_disabilitato" CssStyles-CssEnabled="textbox_data" CssStyles-CssRequired="textbox_data_obbligatorio"
										    BindingField-Editable="always" BindingField-Connection="connessione" BindingField-SourceTable="T_ANA_USL"
										    BindingField-Hidden="False" BindingField-SourceField="USL_SCADENZA" MS_POSITIONING="GridLayout"></ondp:wzOnitDatePick></td>
								    <td class="label">Codice Esterno</td>
								    <td>
									    <ondp:wzTextBox id="tbCodiceEsterno" runat="server" Width="100%" MaxLength="10" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p"
										    CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p"
										    BindingField-Editable="always" BindingField-Connection="connessione" BindingField-SourceTable="T_ANA_USL"
										    BindingField-Hidden="False" BindingField-SourceField="USL_CODICE_ESTERNO"></ondp:wzTextBox></td>
                                    <td class="label">Telefono</td>
								    <td>
									    <ondp:wzTextBox id="tbTel" runat="server" Width="100%" MaxLength="18" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p"
										    CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p"
										    BindingField-Editable="always" BindingField-Connection="connessione" BindingField-SourceTable="T_ANA_USL"
										    BindingField-Hidden="False" BindingField-SourceField="USL_TELEFONO"></ondp:wzTextBox></td>
                                    <td class="label">Codice AVN</td>
                                    <td>
                                        <ondp:wzTextBox id="tbAvn" runat="server" Width="100%" MaxLength="3" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p"
										    CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p"
										    BindingField-Editable="always" BindingField-Connection="connessione" BindingField-SourceTable="T_ANA_USL"
										    BindingField-Hidden="False" BindingField-SourceField="USL_CODICE_AVN"></ondp:wzTextBox></td>
							    </tr>
							    <tr>
								    <td class="label">Indirizzo</td>
								    <td colspan="5">
									    <ondp:wzTextBox id="tbIndirizzo" runat="server" Width="100%" MaxLength="40" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p"
										    CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p"
										    BindingField-Editable="always" BindingField-Connection="connessione" BindingField-SourceTable="T_ANA_USL"
										    BindingField-Hidden="False" BindingField-SourceField="USL_INDIRIZZO"></ondp:wzTextBox></td>
                                    <td class="label">Citta</td>
								    <td>
									    <ondp:wzTextBox id="WzFmComune" runat="server" Width="100%" MaxLength="2" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p"
										    CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p"
										    BindingField-Editable="always" BindingField-Connection="connessione" BindingField-SourceTable="T_ANA_USL"
										    BindingField-Hidden="False" BindingField-SourceField="USL_CITTA"></ondp:wzTextBox></td>
							    </tr>
							    <tr>
								    <td class="label">CAP</td>
								    <td>
									    <ondp:wzTextBox id="tbCap" runat="server" Width="100%" MaxLength="5" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p"
										    CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p"
										    BindingField-Editable="always" BindingField-Connection="connessione" BindingField-SourceTable="T_ANA_USL"
										    BindingField-Hidden="False" BindingField-SourceField="USL_CAP"></ondp:wzTextBox></td>
								    <td class="label">Provincia</td>
								    <td>
									    <ondp:wzTextBox id="tbProvincia" runat="server" Width="100%" MaxLength="2" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p"
										    CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p"
										    BindingField-Editable="always" BindingField-Connection="connessione" BindingField-SourceTable="T_ANA_USL"
										    BindingField-Hidden="False" BindingField-SourceField="USL_PROVINCIA"></ondp:wzTextBox></td>
								    <td class="label">Regione</td>
								    <td colspan="3">
									    <ondp:wzFinestraModale id="WzFmRegione" runat="server" Width="70%" UseTableLayout="True" BindingDescription-SourceField="REG_DESCRIZIONE"
										    BindingDescription-SourceTable="T_ANA_REGIONI" BindingDescription-Connection="connessione" BindingDescription-Hidden="False"
										    BindingDescription-Editable="always" UseCode="True" DataTypeDescription="Stringa" IsDistinct="False"
										    DataTypeCode="Stringa" RaiseChangeEvent="False" Obbligatorio="False" BindingCode-SourceField="USL_REG_CODICE"
										    BindingCode-Hidden="False" BindingCode-SourceTable="T_ANA_USL" BindingCode-Connection="connessione"
										    BindingCode-Editable="always" CampoDescrizione="REG_DESCRIZIONE as Descrizione" CampoCodice="REG_CODICE as Codice"
										    MaxRecords="0" PageSize="50" Paging="True" SetUpperCase="True" Sorting="True" Tabella="T_ANA_REGIONI"
										    CodiceWidth="30%" LabelWidth="-1px" PosizionamentoFacile="False" UseAllResultCodeIfEqual="true" UseAllResultDescIfEqual="true"></ondp:wzFinestraModale></td>
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
