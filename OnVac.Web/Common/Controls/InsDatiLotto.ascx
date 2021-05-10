<%@ Control Language="vb" AutoEventWireup="false" Codebehind="InsDatiLotto.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.InsDatiLotto" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>

<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>

<%@ Register TagPrefix="ucEtaAttivazione" TagName="CampiEtaAttivazione" Src="../../Common/Controls/CampiEtaAttivazione.ascx" %>

<link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
<link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

<script type="text/javascript">
    function CheckCodeFormat(text) {
        if (!text.readOnly) {
            text.value = text.value.replace(/\n/g, ',').replace(/\s/g, '').replace(/  ,/g, ',');
            stringToUpperCase(text);
        }
    }
</script>

<table id="Table1" cellspacing="5" cellpadding="0" width="100%" border="0">
    <colgroup>
        <col style="width:150px" />
        <col style="width:25%" />
        <col style="width:20%" />
        <col style="width:15%" />
        <col style="width:20%" />
        <col />
    </colgroup>
	<tr>
		<td class="label">
			<asp:label id="Label9" runat="server">Lotto Codice</asp:label></td>
		<td colspan="4">
			<asp:textbox id="tb_codLotto" onblur="CheckCodeFormat(this)" runat="server" CssClass="TextBox_Stringa_Obbligatorio" MaxLength="15" Width="100%" ></asp:textbox></td>
        <td></td>
	</tr>
	<tr>
		<td class="label">
			<asp:label id="Label10" runat="server">Nome Commerciale</asp:label></td>
		<td colspan="4">
			<on_ofm:OnitModalList id="fm_nomeCom" runat="server" AltriCampi="noc_for_codice, for_descrizione"
				RaiseChangeEvent="True" width="70%" Filtro="noc_for_codice = for_codice (+)  AND NOC_OBSOLETO<>'S' ORDER BY NOC_DESCRIZIONE" SetUpperCase="True"
				UseCode="True" Tabella="t_ana_nomi_commerciali, t_ana_fornitori" CampoDescrizione="noc_descrizione" CampoCodice="noc_codice" CodiceWidth="30%"
				Obbligatorio="True" PosizionamentoFacile="False" LabelWidth="-1px"></on_ofm:OnitModalList></td>
        <td></td>
	</tr>
	<tr>
		<td class="label">
			<asp:label id="Label11" runat="server">Lotto Descrizione</asp:label></td>
		<td colspan="4">
			<asp:textbox id="tb_descLotto" onblur="stringToUpperCase(this)" runat="server" CssClass="Textbox_Stringa_Obbligatorio" Width="100%" Enabled="True"></asp:textbox></td>
        <td></td>
	</tr>
	<tr>
		<td class="label">
			<asp:label id="Label3" runat="server">Data Preparazione</asp:label></td>
		<td>
			<on_val:onitdatepick id="tb_dataPrep" runat="server" CssClass="textbox_data" Width="120px" DateBox="True"></on_val:onitdatepick></td>
		<td colspan="2" class="label">
			<asp:label id="Label14" runat="server">Quantità Iniziale</asp:label></td>
		<td>
			<asp:textbox id="tb_quantitaIniz" runat="server" CssClass="TextBox_Numerico" Width="100%"></asp:textbox></td>
        <td></td>
	</tr>
	<tr>
		<td class="label">
			<asp:label id="Label8" runat="server">Data Scadenza</asp:label></td>
		<td>
			<on_val:onitdatepick id="tb_dataScad" runat="server" CssClass="textbox_data_obbligatorio" Width="120px" DateBox="True"></on_val:onitdatepick></td>
		<td colspan="2" class="label">
			<asp:label id="Label1" runat="server">Quantità Minima</asp:label></td>
		<td>
			<asp:textbox id="tb_quantitaMin" runat="server" CssClass="TextBox_Numerico_Obbligatorio" Width="100%"></asp:textbox></td>
        <td></td>
	</tr>
	<tr>
        <td class="label">
			<asp:label id="Label2" runat="server">Stato</asp:label></td>
		<td>
			<asp:checkbox id="cb_Annullato" runat="server" CssClass="label" Text="Sequestrato"></asp:checkbox>
			<asp:checkbox id="cb_Attivo" runat="server" CssClass="label" Text="Attivo"></asp:checkbox></td>
		<td colspan="2" class="label">
			<asp:label id="Label12" runat="server">Dosi Rimaste</asp:label></td>
		<td>
			<asp:textbox id="tb_dosiRimaste" runat="server" CssClass="TextBox_Numerico_Disabilitato" Width="100%" ReadOnly="True"></asp:textbox></td>
        <td></td>
	</tr>
	<tr>
        <td class="label">
			<asp:label id="lblDosiScatola" runat="server">Dosi per Scatola</asp:label></td>
		<td>
			<asp:textbox id="tb_dosiScat" runat="server" CssClass="TextBox_Numerico" Width="120px"></asp:textbox>
		<td colspan="2" class="label">
			<asp:label id="lbUnitaMisura" runat="server">Unità di Misura</asp:label></td>
		<td>
			<asp:radiobutton id="rb_scat" runat="server" CssClass="label" Text="Scatola" TextAlign="Right"
				GroupName="unitMis"></asp:radiobutton>
			<asp:radiobutton id="rb_dose" runat="server" CssClass="label" Height="10px" Text="Dose" TextAlign="Right" GroupName="unitMis" Checked="True"></asp:radiobutton></td>
        <td></td>
	</tr>
    <tr>
        <td class="label">
            <asp:label id="lblEtaMinAttivazione" runat="server">Eta min. attivazione</asp:label></td>
        <td colspan="4">
            <ucEtaAttivazione:CampiEtaAttivazione ID="ucEtaMinAttivazione" runat="server"
                LabelCssClass="label_left" TextBoxCssClass="TextBox_Numerico" TextBoxDisabledCssClass="TextBox_Numerico_Disabilitato" />
        </td>
        <td></td>
    </tr>
    <tr>
        <td class="label">
            <asp:label id="lblEtaMaxAttivazione" runat="server">Eta max. attivazione</asp:label></td>
        <td colspan="4">
            <ucEtaAttivazione:CampiEtaAttivazione ID="ucEtaMaxAttivazione" runat="server"
                LabelCssClass="label_left" TextBoxCssClass="TextBox_Numerico" TextBoxDisabledCssClass="TextBox_Numerico_Disabilitato" />
        </td>
        <td></td>
    </tr>
	<tr>
		<td class="label">
			<asp:label id="Label6" runat="server">Fornitore</asp:label></td>
		<td colSpan="4">
			<asp:textbox id="tb_fornitore" onblur="stringToUpperCase(this)" style="display: none" runat="server" CssClass="TextBox_Stringa" Width="99%"></asp:textbox>
			<on_ofm:onitmodallist id="fmFornitore" runat="server" RaiseChangeEvent="True" width="70%"
				Filtro="'TRUE'='TRUE' ORDER BY FOR_DESCRIZIONE" SetUpperCase="True" UseCode="True" Tabella="t_ana_fornitori" CampoDescrizione="for_descrizione" CampoCodice="for_codice"
				CodiceWidth="30%" Obbligatorio="False" PosizionamentoFacile="False" LabelWidth="-1px" Enabled="False"></on_ofm:onitmodallist></td>
        <td></td>
	</tr>
	<tr>
		<td valign="top" class="label">
			<asp:label id="Label5" runat="server" CssClass="label">Note</asp:label></td>
		<td colspan="4">
			<asp:textbox id="tb_note" runat="server" CssClass="TextBox_Stringa" Height="40px" Width="100%" Rows="3" TextMode="MultiLine"></asp:textbox></td>
        <td></td>
	</tr>
</table>

<script type="text/javascript">
	//assegna il nome commerciale alla descrizione del lotto (modifica 28/07/2004)	
	function ValorizzaDescrizione(par1,par2,par3,par4,tipoOperazione,fm,campiImpostati) 
	{
		var descLotto = document.getElementById('<%= tb_descLotto.ClientID %>');
		var nomeComm = document.getElementById('<%= fm_nomeCom.ClientID %>');
		var fornitore = document.getElementById('<%= fmFornitore.ClientID %>');
		switch(tipoOperazione)
		{
		case 0:
			nomeComm.value = campiImpostati['NOC_DESCRIZIONE'];
			nomeComm.nextSibling.value = campiImpostati['NOC_CODICE'];
			descLotto.value = campiImpostati['NOC_DESCRIZIONE'];
			
			fornitore.value = campiImpostati['FOR_DESCRIZIONE'];
			fornitore.nextSibling.value = campiImpostati['NOC_FOR_CODICE'];
			
			FM_OK_Click(par1,par2,par3,par4,tipoOperazione,campiImpostati);
			break;
		case 1:
		case 2:
			FM_Annulla_Click(par1,par2,'','',tipoOperazione,campiImpostati);
			break;
		case 3:
			FM_Annulla_Click(par1,par2,'','',tipoOperazione,campiImpostati);
			nomeComm.value = '';
			nomeComm.nextSibling.value = '';
			descLotto.value = '';
			fornitore.value = '';
			fornitore.nextSibling.value = '';
			break;
		}
	}					
</script>
