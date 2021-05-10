<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ModConv.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.ModConv" %>

<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>

<link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
<link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

<on_lay3:OnitLayout3 ID="OnitLayout31" runat="server" WindowNoFrames="True">
    <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="ToolBar" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
        <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
        <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
	    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
        <ClientSideEvents InitializeToolbar="InitToolBar_modConv" Click="ClickToolBar_modConv"></ClientSideEvents>
        <Items>
            <igtbar:TBarButton Key="btnSalva" Text="Salva" DisabledImage="~/Images/salva_dis.gif" Image="~/Images/salva.gif">
            </igtbar:TBarButton>
            <igtbar:TBarButton Key="btnAnnulla" Text="Chiudi" DisabledImage="~/Images/esci_dis.gif" Image="~/Images/esci.gif">
            </igtbar:TBarButton>
        </Items>
    </igtbar:UltraWebToolbar>
    <asp:Panel ID="pnlDatiConvocazione" runat="server" Width="100%">
        <div class="sezione" id="Panel23" runat="server">
            <asp:Label ID="LayoutTitolo_sezioneRicerca" runat="server">Informazioni sulla convocazione</asp:Label></div>
        <table class="label" id="Table1" style="width: 99%; height: 128px" cellspacing="2" cellpadding="2" border="0">
            <tr>
                <td>
                    <fieldset class="fldroot" id="fldDatiCnv" title="Dati convocazione">
                        <legend class="label">Dati convocazione</legend>
                        <table id="tblDatiCnv" cellspacing="2" cellpadding="2" width="100%" border="0">
                            <tr>
                                <td class="label" width="15%">
                                    <label id="lblCnvp">
                                        Centro Vaccinale</label>
                                </td>
                                <td width="30%">
                                    <on_ofm:OnitModalList ID="txtConsultorio" Height="52px" runat="server" Width="79%" Enabled="False" CodiceWidth="20%" PosizionamentoFacile="False"
                                        LabelWidth="-8px" CampoCodice="CNS_CODICE" CampoDescrizione="CNS_DESCRIZIONE" Tabella="T_ANA_CONSULTORI" UseCode="False"
                                        Obbligatorio="False" SetUpperCase="False"></on_ofm:OnitModalList>
                                </td>
                                <td class="label" width="15%">
                                    <label id="lblODataCnv" class="label">Data convocazione</label>
                                </td>
                                <td width="30%">
                                    <on_val:OnitDatePick ID="txtDataConvocazione" runat="server" Width="130px" CssClass="textbox_data_obbligatorio" DateBox="True">
                                    </on_val:OnitDatePick>
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </td>
            </tr>
            <tr>
                <td>
                    <fieldset class="fldroot" id="fldDatiApp" title="Dati appuntamento">
                        <legend class="label">Dati appuntamento</legend>
                        <table id="tblCns" cellspacing="2" cellpadding="2" width="100%" border="0">
                            <tr>
                                <td class="label" width="15%">
                                    <label id="lblDataApp">Data appuntamento</label>
                                </td>
                                <td width="30%">
                                    <on_val:OnitDatePick ID="txtDataAppuntamento" runat="server" Width="140px" Enabled="False" CssClass="textbox_data_disabilitato"
                                        DateBox="True" NoCalendario="True"></on_val:OnitDatePick>
                                    <asp:TextBox ID="txtTipoAppuntamento" runat="server" Width="80px" CssClass="textbox_stringa_disabilitato" Visible="False"
                                        ReadOnly="True"></asp:TextBox>
                                    <asp:TextBox ID="txtUtenteAppuntamento" runat="server" Width="80px" CssClass="textbox_stringa_disabilitato" Visible="False"
                                        ReadOnly="True"></asp:TextBox>
                                </td>
                                <td class="label" width="15%">
                                    <label id="lblOraApp" class="label">Ora appuntamento</label>
                                </td>
                                <td width="30%">
                                    <asp:TextBox ID="txtOraAppuntamento" runat="server" Width="80px" CssClass="textbox_stringa_disabilitato" ReadOnly="True"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="label" width="15%">
                                    <label id="lblDurataApp" class="label">Durata</label>
                                </td>
                                <td width="30%">
                                    <on_val:OnitJsValidator ID="txtDurataAppuntamento" runat="server" Width="40px" CssClass="textbox_numerico" PreParams-minValue="0"
                                        PreParams-maxValue="null" PreParams-numDecDigit="0" validationType="Validate_integer" autoFormat="False" actionUndo="False"
                                        actionSelect="True" actionMessage="True" actionFocus="False" actionDelete="False" actionCorrect="False"></on_val:OnitJsValidator>
                                </td>
                                <td class="label" width="15%">
                                    <label id="lblDataInvio" class="label">Data invio</label>
                                </td>
                                <td valign="middle" width="30%">
                                    <on_val:OnitDatePick ID="txtDataInvio" runat="server" Width="140px" Enabled="False" CssClass="TextBox_Data_Disabilitato"
                                        DateBox="True" NoCalendario="True" CalendarioPopUp="True"></on_val:OnitDatePick>
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </td>
            </tr>
            <tr>
                <td>
                    <fieldset class="fldroot" id="fldDatiPaziente" title="Dati paziente">
                        <legend class="label">Dati paziente</legend>
                        <table id="tblDatiPaz" cellspacing="2" cellpadding="2" width="100%" border="0">
                            <tr>
                                <td class="label" width="15%">
                                    <label id="lblDataNascita">Data Nascita</label>
                                </td>
                                <td width="30%">
                                    <on_val:OnitDatePick ID="odpDataNascita" runat="server" Width="130" Enabled="False" CssClass="textbox_data_disabilitato"
                                        DateBox="True" NoCalendario="True"></on_val:OnitDatePick>
                                </td>
                                <td class="label" width="15%">
                                    <label id="lblEta" class="label">Età alla conv.</label>
                                </td>
                                <td width="30%">
                                    <asp:TextBox ID="txtEtaPaziente" Width="100%" CssClass="textbox_stringa_disabilitato" ReadOnly="true" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
        </table>
        <div class="sezione" id="Div1" runat="server">
            <asp:Label ID="Label1" runat="server">Area messaggi</asp:Label></div>
        <div style="background-color: #e6e6e6">
            <asp:Label ID="lblErrore" Height="75px" runat="server" Width="100%" CssClass="Label_ErrorMsg"></asp:Label></div>
    </asp:Panel>
    <asp:Panel ID="pnlSceltaBilancio" Style="display: none" runat="server" Width="100%" CssClass="LabelLeft" HorizontalAlign="Center">
    </asp:Panel>
    <asp:PlaceHolder ID="cntEventi" runat="server"></asp:PlaceHolder>
</on_lay3:OnitLayout3>

<script type="text/javascript" language="javascript">	
	function InitToolBar_modConv(toolBar,evnt)
	{
		toolBar.PostBackButton=false;
	}
	
	function ClickToolBar_modConv(toolBar,element,evnt)
	{
		if (element.Key=='btnAnnulla') 
		{
			closeFm('<%= ModaleName %>');
			evnt.needPostBack=false;
		}
		else
		{
			if (element.Key=='btnSalva') {
				dataTmp=OnitDataPickGet('<%= txtDataConvocazione.ClientID %>');
				if (dataTmp==""){
					alert("Inserire una data di convocazione!!")
					evnt.needPostBack=false;
				}
				else
					evnt.needPostBack=true;	
			}
		}
	}
</script>

