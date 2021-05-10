<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Medici.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.OnVac_Medici" %>

<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_otb" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitTable" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>Medici</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

    <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
    <script type="text/javascript" src='<%= Onit.OnAssistnet.OnVac.OnVacUtility.GetScriptUrl("onvac.common.js")%>'></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#txtCodice").live('blur', function () { this.value=(this.value).toUpperCase(); });
            $("#txtCognome").live('blur', function () { this.value = (this.value).toUpperCase(); });
            $("#txtNome").live('blur', function () { this.value = (this.value).toUpperCase(); });
            $("#tbIndirizzo").live('blur', function () { this.value = (this.value).toUpperCase(); });
            $("#tbCap").live('blur', function () { this.value = (this.value).toUpperCase(); });
            $("#tbProvincia").live('blur', function () { this.value = (this.value).toUpperCase(); });
            $("#tbTel").live('blur', function () { this.value = (this.value).toUpperCase(); });
        });

        function InizializzaToolBar(t) {
            t.PostBackButton = false;
        }

        function ToolBarClick(ToolBar, button, evnt) {
            evnt.needPostBack = true;

            switch (button.Key) {

                case 'btnSalva':

                    var cod = $("#txtCodice").val();
                    var cognome = $("#txtCognome").val();
                    var nome = $("#txtNome").val();

                    if ($.trim(cod) == "" || $.trim(cognome) == "" || $.trim(nome) == "") {
                        alert('Attenzione: alcuni campi obbligatori non sono impostati correttamente!');
                        evnt.needPostBack = false;
                    }
            }
        }		
    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
    <on_lay3:OnitLayout3 ID="Onitlayout31" runat="server" Height="100%" Width="100%" Titolo="Medici" TitleCssClass="Title3">
        <ondp:onitdatapanel id="odpMedici" runat="server" width="100%" renderonlychildren="True"
            maxrecord-length="3" maxrecord="200" configfile="Medici.odpMedici.xml" usetoolbar="False">
			<on_otb:onittable id="OnitTable1" runat="server" width="100%" height="100%">
				<on_otb:onitsection id="sezRicerca" runat="server" width="100%" typeHeight="content">
					<on_otb:onitcell id="cellaRicerca" runat="server" height="100%">
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
							</Items>
						</igtbar:UltraWebToolbar>
						<div class="Sezione">Modulo ricerca</div>
						<ondp:wzFilter id="WzFilter1" runat="server" Height="70px" CssClass="InfraUltraWebTab2" Width="100%">
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
													<asp:Label id="Label1" runat="server" CssClass="LABEL">Filtro di Ricerca</asp:Label></td>
												<td>
													<asp:TextBox id="WzFilterKeyBase" runat="server" cssclass="textbox_stringa w100p"></asp:TextBox></td>
											</tr>
										</table>
									</ContentTemplate>
								</igtab:Tab>
							</Tabs>
						</ondp:wzFilter>
						<div class="Sezione">Elenco</div>
					</on_otb:onitcell>
				</on_otb:onitsection>
				<on_otb:onitsection id="sezioneRisultati" runat="server" width="100%" typeHeight="calculate">
					<on_otb:onitcell id="cellaRisultati" runat="server" height="100%" typescroll="auto">
						<ondp:wzDataGrid Browser="UpLevel" id="dgrMedici" runat="server" Width="100%" disableActiveRowChange="False" EditMode="None">
							<DisplayLayout ColWidthDefault="" AutoGenerateColumns="False" RowHeightDefault="26px" Version="2.00"
								GridLinesDefault="None" SelectTypeRowDefault="Single" ScrollBar="Never" BorderCollapseDefault="Separate"
								CellPaddingDefault="3" RowSelectorsDefault="No" Name="dgrMedici" CellClickActionDefault="RowSelect">
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
										<igtbl:UltraGridColumn HeaderText="&lt;input type=checkbox  OnClick='javascript:wzDataGrid_Master_Click(this,&amp;quot;dgrMedici&amp;quot;);' /&gt;"
											Key="check" Width="0%" Hidden="True" Type="CheckBox" BaseColumnName="" AllowUpdate="Yes"></igtbl:UltraGridColumn>
										<igtbl:UltraGridColumn HeaderText="Codice" Key="" Width="10%" BaseColumnName=""></igtbl:UltraGridColumn>
										<igtbl:UltraGridColumn HeaderText="Descrizione" Key="" Width="40%" BaseColumnName=""></igtbl:UltraGridColumn>
										<igtbl:UltraGridColumn HeaderText="Codice Regionale" Key="" Width="10%" BaseColumnName=""></igtbl:UltraGridColumn>
                                        <igtbl:UltraGridColumn HeaderText="Tipo" Key="" Width="10%" BaseColumnName=""></igtbl:UltraGridColumn>
                                        <igtbl:UltraGridColumn HeaderText="Codice Fiscale" Key="" Width="10%" BaseColumnName=""></igtbl:UltraGridColumn>
                                        <igtbl:UltraGridColumn HeaderText="Data iscrizione" Key="" Width="10%" Format="dd/MM/yyyy" BaseColumnName=""></igtbl:UltraGridColumn>
									</Columns>
								</igtbl:UltraGridBand>
							</Bands>
							<BindingColumns>
								<ondp:BindingFieldValue Value="" Editable="always" Description="Codice" Connection="connessioneMaster" SourceTable="T_ANA_MEDICI"
									Hidden="False" SourceField="MED_CODICE"></ondp:BindingFieldValue>
								<ondp:BindingFieldValue Value="" Editable="always" Description="Descrizione" Connection="connessioneMaster"
									SourceTable="T_ANA_MEDICI" Hidden="False" SourceField="MED_DESCRIZIONE"></ondp:BindingFieldValue>
                                <ondp:BindingFieldValue Value="" Editable="always" Description="Codice Regionale" Connection="connessioneMaster"
									SourceTable="T_ANA_MEDICI" Hidden="False" SourceField="MED_CODICE_REGIONALE"></ondp:BindingFieldValue>
                                <ondp:BindingFieldValue Value="" Editable="always" Description="Tipo" Connection="connessioneMaster"
									SourceTable="T_ANA_TIPI_MEDICO" Hidden="False" SourceField="TME_DESCRIZIONE"></ondp:BindingFieldValue>
                                <ondp:BindingFieldValue Value="" Editable="always" Description="Codice Fiscale" Connection="connessioneMaster"
									SourceTable="T_ANA_MEDICI" Hidden="False" SourceField="MED_CODICE_FISCALE"></ondp:BindingFieldValue>
                                <ondp:BindingFieldValue Value="" Editable="always" Description="Data iscrizione" Connection="connessioneMaster"
									SourceTable="T_ANA_MEDICI" Hidden="False" SourceField="MED_DATA_ISCRIZIONE"></ondp:BindingFieldValue>
							</BindingColumns>
						</ondp:wzDataGrid>
					</on_otb:onitcell>
				</on_otb:onitsection>
				<on_otb:onitsection id="sezDettaglio" runat="server" width="100%" TypeHeight="Content">
					<on_otb:onitcell id="cellaDettaglio" runat="server" height="100%">
						<div class="Sezione">Dettaglio</div>
							<table id="Table1" style="table-layout: fixed" cellspacing="3" cellpadding="0" width="100%" border="0">
								<tr>
									<td class="label" width="100">Codice</td>
									<td colspan="3">
										<ondp:wzTextBox id="txtCodice" onblur="toUpper(this);controlloCampoCodice(this);" runat="server"
											maxlength="16"  CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
											CssStyles-CssEnabled="TextBox_Stringa  w100p" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
											BindingField-Hidden="False" BindingField-Editable="onNew" BindingField-Connection="connessioneMaster"
											BindingField-SourceTable="T_ANA_MEDICI" BindingField-SourceField="MED_CODICE"></ondp:wzTextBox></td>
									<td class="label" width="100">Codice regionale</td>
									<td colspan="3">
										<ondp:wzTextBox id="txtCodiceRegionale" onblur="toUpper(this);" runat="server"
											maxlength="16"  CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
											CssStyles-CssEnabled="TextBox_Stringa  w100p" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
											BindingField-Hidden="False" BindingField-Editable="onNew" BindingField-Connection="connessioneMaster"
											BindingField-SourceTable="T_ANA_MEDICI" BindingField-SourceField="MED_CODICE_REGIONALE"></ondp:wzTextBox></td>
                                </tr>
                                <tr>
                                    <td class="label">Codice esterno</td>
									<td colspan="3">
										<ondp:wzTextBox id="txtCodiceEsterno" onblur="toUpper(this);" runat="server"
											Width="100%" maxlength="16"  CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
											CssStyles-CssEnabled="TextBox_Stringa  w100p" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
											BindingField-Hidden="False" BindingField-Editable="always" BindingField-Connection="connessioneMaster"
											BindingField-SourceTable="T_ANA_MEDICI" BindingField-SourceField="MED_CODICE_ESTERNO"></ondp:wzTextBox></td>
                                    <td class="label" width="100">Codice fiscale</td>
									<td>
										<ondp:wzTextBox id="txtCodiceFiscale" onblur="toUpper(this);" runat="server"
											Width="150" maxlength="16"  CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
											CssStyles-CssEnabled="TextBox_Stringa  w100p" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
											BindingField-Hidden="False" BindingField-Editable="always" BindingField-Connection="connessioneMaster"
											BindingField-SourceTable="T_ANA_MEDICI" BindingField-SourceField="MED_CODICE_FISCALE"></ondp:wzTextBox></td>
                                    <td class="label"  width="100">Consenso</td>
									<td>
                                        <ondp:wzCheckBox id="chkConsenso" runat="server" height="12" width="20"
                                        BindingField-SourceField="MED_CONSENSO" BindingField-Hidden="False" BindingField-SourceTable="T_ANA_MEDICI"
                                        BindingField-Connection="locale" BindingField-Editable="always" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato"
                                        CssStyles-CssEnabled="TextBox_Stringa" BindingField-Value="N"></ondp:wzCheckBox>
									</td>
                                </tr>
                                <tr>
                                    <td class="label">Cognome</td>
									<td colspan="3">
										<ondp:wzTextBox id="txtCognome" onblur="toUpper(this);" runat="server"
											Width="100%" maxlength="35"  CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
											CssStyles-CssEnabled="TextBox_Stringa  w100p" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
											BindingField-Hidden="False" BindingField-Editable="always" BindingField-Connection="connessioneMaster"
											BindingField-SourceTable="T_ANA_MEDICI" BindingField-SourceField="MED_COGNOME"></ondp:wzTextBox>
                                    </td>
                                    <td class="label">Nome</td>
									<td colspan="3">
										<ondp:wzTextBox id="txtNome" onblur="toUpper(this);" runat="server"
											Width="100%" maxlength="35"  CssStyles-CssDisabled="TextBox_Stringa_Disabilitato w100p"
											CssStyles-CssEnabled="TextBox_Stringa  w100p" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio w100p"
											BindingField-Hidden="False" BindingField-Editable="always" BindingField-Connection="connessioneMaster"
											BindingField-SourceTable="T_ANA_MEDICI" BindingField-SourceField="MED_NOME"></ondp:wzTextBox>
                                    </td>
								</tr>
                                <tr>
									<td class="label">Data iscrizione</td>
									<td>
										<ondp:wzOnitDatePick id="odpDataIscrizione" runat="server" height="18px" width="95px"
											CssStyles-CssDisabled="textbox_data_disabilitato" CssStyles-CssEnabled="textbox_data" 
                                            CssStyles-CssRequired="textbox_data_obbligatorio"
											BindingField-Editable="always" BindingField-Connection="connessioneMaster" BindingField-SourceTable="T_ANA_MEDICI"
											BindingField-Hidden="False" BindingField-SourceField="MED_DATA_ISCRIZIONE" ></ondp:wzOnitDatePick>
                                    </td>
                                    <td class="label" width="100">Data scadenza</td>
									<td>
										<ondp:wzOnitDatePick id="odpDataScadenza" runat="server" height="18px" width="95px"
											CssStyles-CssDisabled="textbox_data_disabilitato" CssStyles-CssEnabled="textbox_data" 
                                            CssStyles-CssRequired="textbox_data_obbligatorio"
											BindingField-Editable="always" BindingField-Connection="connessioneMaster" BindingField-SourceTable="T_ANA_MEDICI"
											BindingField-Hidden="False" BindingField-SourceField="MED_SCADENZA" ></ondp:wzOnitDatePick>
                                    </td>  
                                    <td class="label" width="100">Tipo</td>
									<td colspan="3">
										<ondp:wzDropDownList id="WzDropDownList1" runat="server" Width="100%"
											CssStyles-CssDisabled="TextBox_Data_Disabilitato w100" CssStyles-CssEnabled="TextBox_Data  w100"
											CssStyles-CssRequired="TextBox_Data_Obbligatorio w100" BindingField-Hidden="False" BindingField-Editable="always"
											BindingField-Connection="connessioneMaster" BindingField-SourceTable="T_ANA_MEDICI" BindingField-SourceField="MED_TIPO"
											TextFieldName="TME_DESCRIZIONE" KeyFieldName="TME_CODICE" SourceTable="T_ANA_TIPI_MEDICO" OtherListFields="TME_CODICE"></ondp:wzDropDownList></td>      
                                </tr>
							</table>
					</on_otb:onitcell>
				</on_otb:onitsection>
			</on_otb:onittable>
		</ondp:onitdatapanel>
    </on_lay3:OnitLayout3>
    </form>
</body>
</html>
