<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Malattie.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_Malattie" %>

<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>Vaccinazioni</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/combobox.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/default/combobox.default.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("onvac.common.js")%>'></script>
    <script type="text/javascript">
        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;

            switch (button.Key) {

                case 'btnSalva':
                    codice = document.getElementById('txtCodice').value;
                    if (codice == "") {
                        alert('Attenzione: alcuni campi obbligatori non sono impostati correttamente!');
                        evnt.needPostBack = false;
                    }
            }
        }
    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
        <on_lay3:OnitLayout3 ID="Onitlayout31" runat="server" TitleCssClass="Title3" Titolo="Malattie" Width="100%" Height="100%">
            <ondp:onitdatapanel id="odpMalattieMaster" runat="server" width="100%" renderonlychildren="True" configfile="Malattie.odpMalattieMaster.xml" usetoolbar="False">
                <div>
				    <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
                        <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                        <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		                <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
					    <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
					    <Items>
						    <igtbar:TBarButton Key="btnCerca" Text="Cerca"></igtbar:TBarButton>
						    <igtbar:TBSeparator></igtbar:TBSeparator>
						    <igtbar:TBarButton Key="btnNew" Text="Nuovo"></igtbar:TBarButton>
						    <igtbar:TBarButton Key="btnEdit" Text="Modifica"></igtbar:TBarButton>
						    <igtbar:TBarButton Key="btnElimina" Text="Elimina"></igtbar:TBarButton>
						    <igtbar:TBSeparator></igtbar:TBSeparator>
						    <igtbar:TBarButton Key="btnSalva" Text="Salva"></igtbar:TBarButton>
						    <igtbar:TBarButton Key="btnAnnulla" Text="Annulla"></igtbar:TBarButton>
							<igtbar:TBSeparator></igtbar:TBSeparator>
							<igtbar:TBarButton Key="btnVaccinazioni" Text="Vaccinazioni" Image="~/Images/vaccinazione.gif">
                                <DefaultStyle Width="100px" CssClass="infratoolbar_button_default"></DefaultStyle>
							</igtbar:TBarButton>
					    </Items>
				    </igtbar:UltraWebToolbar>
                </div>
			    <div class="Sezione">Modulo ricerca</div>
                <div>
				    <ondp:wzFilter id="filFiltro" runat="server" Height="70px" Width="100%" CssClass="InfraUltraWebTab2">
					    <DefaultTabStyle CssClass="InfraTab_Default2"></DefaultTabStyle>
					    <SelectedTabStyle CssClass="InfraTab_Selected2"></SelectedTabStyle>
					    <DisabledTabStyle CssClass="InfraTab_Disabled2"></DisabledTabStyle>
					    <HoverTabStyle CssClass="InfraTab_Hover2"></HoverTabStyle>
					    <Tabs>
						    <igtab:Tab Text="Ricerca di Base">
							    <ContentTemplate>
								    <table height="100%" cellspacing="10" cellpadding="0" width="100%" border="0" style="table-layout:fixed">
									    <tr>
										    <td align="right" width="90">
											    <asp:Label id="Label1" runat="server" CssClass="label">Filtro di Ricerca</asp:Label></td>
										    <td>
											    <asp:TextBox id="WzFilterKeyBase" runat="server" cssclass="textbox_stringa w100p"></asp:TextBox></td>
									    </tr>
								    </table>
							    </ContentTemplate>
						    </igtab:Tab>
					    </Tabs>
				    </ondp:wzFilter>
                </div>
                <div class="Sezione">Elenco</div>

                <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="100%" ScrollBars="Auto" RememberScrollPosition="true">
				    <ondp:wzDataGrid Browser="UpLevel" id="dgrMalattie" runat="server" Width="100%" disableActiveRowChange="False" EditMode="None">
					    <DisplayLayout ColWidthDefault="" AutoGenerateColumns="False" RowHeightDefault="26px" Version="2.00"
						    GridLinesDefault="None" SelectTypeRowDefault="Single" ScrollBar="Never" BorderCollapseDefault="Separate"
						    CellPaddingDefault="3" RowSelectorsDefault="No" Name="dgrMalattie" CellClickActionDefault="RowSelect">
						    <HeaderStyleDefault CssClass="Infra2Dgr_Header"></HeaderStyleDefault>
						    <RowSelectorStyleDefault CssClass="Infra2Dgr_RowSelector"></RowSelectorStyleDefault>
						    <FrameStyle Width="100%"></FrameStyle>
						    <ActivationObject BorderWidth="0px" BorderColor="Navy"></ActivationObject>
						    <SelectedRowStyleDefault CssClass="Infra2Dgr_SelectedRow"></SelectedRowStyleDefault>
						    <RowAlternateStyleDefault CssClass="Infra2Dgr_RowAlternate"></RowAlternateStyleDefault>
						    <RowStyleDefault CssClass="Infra2Dgr_Row"></RowStyleDefault>
					    </DisplayLayout>
					    <Bands>
						    <igtbl:UltraGridBand>
							    <Columns>
								    <igtbl:UltraGridColumn HeaderText="&lt;input type='checkbox' onclick='javascript:wzDataGrid_Master_Click(this,&amp;quot;dgrMalattie&amp;quot;);' /&gt;"
									    Key="check" Width="0%" Hidden="True" Type="CheckBox" BaseColumnName="" AllowUpdate="Yes"></igtbl:UltraGridColumn>
								    <igtbl:UltraGridColumn HeaderText="Codice" Key="MAL_CODICE" Width="200px" BaseColumnName=""></igtbl:UltraGridColumn>
								    <igtbl:UltraGridColumn HeaderText="Descrizione" Key="MAL_DESCRIZIONE" Width="100%" BaseColumnName=""></igtbl:UltraGridColumn>
								    <igtbl:UltraGridColumn HeaderText="Flag Visita" Key="MAL_FLAG_VISITA" Width="50px" BaseColumnName=""></igtbl:UltraGridColumn>
							    </Columns>
						    </igtbl:UltraGridBand>
					    </Bands>
					    <BindingColumns>
						    <ondp:BindingFieldValue Value="" Editable="always" Description="Codice" Connection="MalattieMaster" SourceTable="T_ANA_MALATTIE"
							    Hidden="False" SourceField="MAL_CODICE"></ondp:BindingFieldValue>
						    <ondp:BindingFieldValue Value="" Editable="always" Description="Descrizione" Connection="MalattieMaster"
							    SourceTable="T_ANA_MALATTIE" Hidden="False" SourceField="MAL_DESCRIZIONE"></ondp:BindingFieldValue>
						    <ondp:BindingFieldValue Value="" Editable="always" Description="Flag Visita" Connection="MalattieMaster"
							    SourceTable="T_ANA_MALATTIE" Hidden="False" SourceField="MAL_FLAG_VISITA"></ondp:BindingFieldValue>
					    </BindingColumns>
				    </ondp:wzDataGrid>
                </dyp:DynamicPanel>
            
			    <div class="Sezione">Dettaglio</div>

                <div>
				    <table style="table-layout: fixed" cellspacing="3" cellpadding="0" width="100%" border="0">
                        <colgroup>
                            <col style="text-align:right; width:80px" />
                            <col />
                            <col style="text-align:right; width:100px" />
                            <col />
                            <col style="text-align:right; width:150px" />
                            <col />
                            <col style="text-align:right; width:50px" />
                            <col style="width:30px"/>
                        </colgroup>
					    <tr>
						    <td class="label">Codice</td>
						    <td>
							    <ondp:wzTextBox id="txtCodice" onblur="toUpper(this);controlloCampoCodice(this);" runat="server" MaxLength="8"
                                    CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p" CssStyles-CssEnabled="TextBox_Stringa w100p" 
                                    CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p" BindingField-Editable="onNew" BindingField-Connection="MalattieMaster" 
                                    BindingField-SourceTable="T_ANA_MALATTIE" BindingField-Hidden="False" BindingField-SourceField="MAL_CODICE"></ondp:wzTextBox></td>
						    <td class="label">Cod. Esenzione</td>
						    <td >
							    <ondp:wzTextBox id="txtEse" onblur="toUpper(this);" runat="server" 
                                    CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p" CssStyles-CssEnabled="TextBox_Stringa w100p" MaxLength="8"
                                    CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p" BindingField-Editable="always" BindingField-Connection="MalattieMaster" 
                                    BindingField-SourceTable="T_ANA_MALATTIE" BindingField-Hidden="False" BindingField-SourceField="MAL_CODICE_ESENZIONE"></ondp:wzTextBox></td>
						    <td class="label">Bilancio da data diagnosi</td>
						    <td>
							    <ondp:wzDropDownList id="ddlFLagVisita" runat="server" Width="100%" 
                                    CssStyles-CssDisabled="TextBox_Stringa_Disabilitato" CssStyles-CssEnabled="TextBox_Stringa" 
                                    CssStyles-CssRequired="TextBox_Stringa_Obbligatorio" BindingField-Editable="always" BindingField-Connection="MalattieMaster" 
                                    BindingField-SourceTable="T_ANA_MALATTIE" BindingField-Hidden="False" BindingField-SourceField="MAL_FLAG_VISITA" 
                                    BindingField-Value="S" IncludeNull="False" SourceConnection="">
								    <asp:ListItem Selected="True"></asp:ListItem>
								    <asp:ListItem Value="S">SI</asp:ListItem>
								    <asp:ListItem Value="N">NO</asp:ListItem>
							    </ondp:wzDropDownList>
						    </td>
                            <td class="label">Obsoleto</td>
                            <td>
                                <ondp:wzCheckBox id="chkObsoleto" runat="server" Height="12px" BindingField-SourceField="MAL_OBSOLETO"
								    BindingField-Hidden="False" BindingField-SourceTable="T_ANA_MALATTIE" BindingField-Connection="MalattieMaster"
								    BindingField-Editable="always" CssStyles-CssEnabled="Label" CssStyles-CssDisabled="Label_Disabilitato"
								    BindingField-Value="N"></ondp:wzCheckBox></td>
					    </tr>
					    <tr>
						    <td class="label">Descrizione</td>
						    <td colspan="3">
							    <ondp:wzTextBox id="WzTextBox2" onblur="toUpper(this);" runat="server" MaxLength="35"
                                    CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p" CssStyles-CssEnabled="TextBox_Stringa w100p" 
                                    CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p" BindingField-Editable="always" BindingField-Connection="MalattieMaster" 
                                    BindingField-SourceTable="T_ANA_MALATTIE" BindingField-Hidden="False" BindingField-SourceField="MAL_DESCRIZIONE"></ondp:wzTextBox></td>
                            <td class="label">Tipologia</td>
						    <td>
                                <telerik:RadComboBox ID="ddlTipologia" runat="server" CheckBoxes="true" Localization-ItemsCheckedString="tipologie selezionate" 
                                    Localization-AllItemsCheckedString="Tutte" Width="100%" DataTextField="Descrizione" DataValueField="Codice" 
                                    EnableEmbeddedSkins="false" Skin="Default" Enabled="false"/></td>
                            <td class="label">Follow Up</td>
                            <td>
                                <ondp:wzCheckBox id="chkFollowUp" runat="server" Height="12px" BindingField-SourceField="MAL_SOLO_FOLLOW_UP"
								    BindingField-Hidden="False" BindingField-SourceTable="T_ANA_MALATTIE" BindingField-Connection="MalattieMaster"
								    BindingField-Editable="always" CssStyles-CssEnabled="Label" CssStyles-CssDisabled="Label_Disabilitato"
								    BindingField-Value="N"></ondp:wzCheckBox></td>
					    </tr>
                        <tr>
                            <td class="label">Follow up</td>
							<td>
								<ondp:wzTextBox id="WzTextBox1" onblur="toUpper(this);" runat="server" MaxLength="35"
                                    CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p" CssStyles-CssEnabled="TextBox_Stringa w100p" 
                                    CssStyles-CssRequired="TextBox_StringA w100p" BindingField-Editable="always" BindingField-Connection="MalattieMaster" 
                                    BindingField-SourceTable="T_ANA_MALATTIE" BindingField-Hidden="False" BindingField-SourceField="MAL_MAL_CODICE_FOLLOW_UP"></ondp:wzTextBox></td>
                             <td class="label">Codice AVN</td>
                             <td>
                                <ondp:wzTextBox id="txtAvn" runat="server" Width="100%" MaxLength="2" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p"
									CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p"
									BindingField-Editable="always" BindingField-Connection="MalattieMaster" BindingField-SourceTable="T_ANA_MALATTIE"
									BindingField-Hidden="False" BindingField-SourceField="MAL_CODICE_AVN"></ondp:wzTextBox></td>
                            <td class="label">
                                <asp:Label ID="lblCodiceICD9CM" runat="server" Text="<%$ Resources: Onit.OnAssistnet.OnVac.Web, Malattie.LabelCodiceICD9CM %>" />
                            </td>
                            <td colspan="3">
                                <ondp:wzTextBox id="WzTextBox3" runat="server" Width="100%" MaxLength="20" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p"
									CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p"
									BindingField-Editable="always" BindingField-Connection="MalattieMaster" BindingField-SourceTable="T_ANA_MALATTIE"
									BindingField-Hidden="False" BindingField-SourceField="MAL_CODICE_FSE"></ondp:wzTextBox></td>
                        </tr>
				    </table>
                </div>
		    </ondp:onitdatapanel>
        </on_lay3:OnitLayout3>
    </form>
</body>
</html>
