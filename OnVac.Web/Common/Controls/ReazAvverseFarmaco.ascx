<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ReazAvverseFarmaco.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.ReazAvverseFarmaco" %>

<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="uc1" TagName="Anagrafica" Src="Anagrafica.ascx" %>

<table cellspacing="2" cellpadding="2" class="section_1" onkeydown="AvviaRicerca(event,this)">
	<colgroup>
		<col style="width: 18%" />
		<col style="width: 7%" />
		<col style="width: 14%" />
		<col style="width: 6%" />
		<col style="width: 11%" />
		<col style="width: 7%" />
		<col style="width: 14%" />
		<col style="width: 10%" />
		<col style="width: 13%" />
	</colgroup>
    <tr>
        <td>
            <asp:HiddenField ID="hidVesId" runat="server" />
            <asp:Label ID="lblId" runat="server" CssClass="rb_large_bold">Farmaco Sospetto/Concomitante X</asp:Label>
        </td>
        <td>
            <asp:Button ID="btnReplica" runat="server" Text="Replica" CssClass="button_reazioni" ToolTip="Replica i dati del primo farmaco nei successivi, se selezionati. Replica solo i dati relativi alla reazione." Width="100%" />
            <asp:CheckBox ID="chkReplica" runat="server" ToolTip="Selezionare se si desidera che i dati del primo farmaco vengano replicati anche in questo." />
        </td>
        <td colspan="7"></td>
    </tr>
	<tr>
		<td class="label">
            <asp:Label ID="lblNomeCommerciale" Runat="server" Text="Farmaco"></asp:Label>
		</td>
		<td colspan="3">
            <asp:textbox id="txtNomeCommerciale" Runat="server" onblur="toUpper(this)" width="100%" MaxLength="64" Visible="false"></asp:textbox>
             <uc1:Anagrafica ID="farmaco" runat="server" TipiAnagrafica="Farmaco" ></uc1:Anagrafica>
		</td>
		<td class="label">
            <asp:Label ID="lblLotto" Runat="server" Text="Lotto"></asp:Label>
		</td>
		<td colspan="2">
            <asp:textbox id="txtLotto" Runat="server" onblur="toUpper(this)" width="100%" MaxLength="15"></asp:textbox>
		</td>
        <td class="label">
            <asp:label id="lblScadenza" runat="server" Text="Scadenza"></asp:label></td>
		<td class="label_left" >
            <on_val:OnitDatePick ID="dpkDataScadenza" CssClass="TextBox_Data" Width="96%" runat="server" />
        </td>
	</tr>
	<tr>
		<td class="label">Dose</td>
		<td class="label_left">
            <on_val:OnitJsValidator id="txtDose" runat="server" CssClass="TextBox_Numerico" Width="100%"
				actionCorrect="False" actionDelete="False" actionFocus="True" actionMessage="True" actionSelect="False"
				actionUndo="True" autoFormat="True" validationType="Validate_integer" MaxLength="2"
                PreParams-numDecDigit="0" PreParams-maxValue="99" PreParams-minValue="1"></on_val:OnitJsValidator>
		</td>
		<td class="label" colspan="3">
            <asp:Label ID="lblSomministrazione" runat="server" Text="Via di somministrazione"></asp:Label></td>
		<td class="label_left" colspan="4">
			<on_ofm:OnitModalList id="fmViaSomministrazione" runat="server" Width="72%" RaiseChangeEvent="True" PosizionamentoFacile="False"
				LabelWidth="-1px" Tabella="t_ana_vie_somministrazione" CampoDescrizione="vii_descrizione" CampoCodice="vii_codice"
				CodiceWidth="27%" UseCode="True" SetUpperCase="True"></on_ofm:OnitModalList>
		</td>
	</tr>
	<tr>
		<td class="label">
            <asp:label id="lblDataOraEsecuzioneVac" runat="server" Text="Data-Ora vaccinazione"></asp:label></td>
		<td class="label_left" colspan="2">
			<asp:TextBox id="txtDataOraEsecuzioneVac" CssClass="TextBox_Stringa_Disabilitato" width="100%" Runat="server" ReadOnly="True" ></asp:textbox>
            <on_val:OnitDatePick ID="dpkDataEsecuzioneVac" CssClass="TextBox_Data" Width="100%" runat="server" />
        </td>
        <td class="label_left">
            <asp:TextBox id="txtOraEsecuzioneVac" CssClass="TextBox_Stringa" width="100%" Runat="server" onblur="formattaOrario(this);" MaxLength="5" ></asp:textbox>
        </td>
		<td class="label">Sito di inoculo</td>
		<td class="label_left" colspan="4">
			<on_ofm:OnitModalList id="fmSitoInoculo" runat="server" Width="72%" RaiseChangeEvent="True" PosizionamentoFacile="False"
				LabelWidth="-1px" Tabella="t_ana_siti_inoculazione" CampoDescrizione="sii_descrizione" CampoCodice="sii_codice"
				CodiceWidth="27%" UseCode="True" SetUpperCase="True" ></on_ofm:OnitModalList>
		</td>
    </tr>
    <tr>
        <td class="label">
            <asp:Label ID="lblSospeso" runat="server" Text="Farmaco sospeso?"></asp:Label></td>
        <td>
            <asp:dropdownlist id="ddlSospeso" Runat="server" onchange="" Width="100%">
				<asp:ListItem Value=""></asp:ListItem>
				<asp:ListItem Value="N">NO</asp:ListItem>
				<asp:ListItem Value="S">SI</asp:ListItem>
			</asp:dropdownlist>
        </td>
        <td class="label" colspan="3">Reazione migliorata?</td>
        <td>
            <asp:dropdownlist id="ddlMiglioramento" Runat="server" Width="100%" CssClass="TextBox_Stringa">
				<asp:ListItem Value=""></asp:ListItem>
				<asp:ListItem Value="N">NO</asp:ListItem>
				<asp:ListItem Value="S">SI</asp:ListItem>
			</asp:dropdownlist>
        </td>
        <td colspan="3"></td>
    </tr>
    <tr>
        <td class="label">Farmaco ripreso?</td>
        <td>
            <asp:dropdownlist id="ddlRipreso" Runat="server" Width="100%" CssClass="TextBox_Stringa">
				<asp:ListItem Value=""></asp:ListItem>
				<asp:ListItem Value="N">NO</asp:ListItem>
				<asp:ListItem Value="S">SI</asp:ListItem>
			</asp:dropdownlist>
        </td>
		<td class="label" colspan="3">Ricomparsi sintomi?</td>
        <td>
            <asp:dropdownlist id="ddlRicomparsa" Runat="server" Width="100%" CssClass="TextBox_Stringa">
				<asp:ListItem Value=""></asp:ListItem>
				<asp:ListItem Value="N">NO</asp:ListItem>
				<asp:ListItem Value="S">SI</asp:ListItem>
			</asp:dropdownlist>
        </td>
        <td colspan="3"></td>
    </tr>
    <tr>
		<td class="label">Indicazioni per cui il farmaco è stato usato</td>
		<td colspan="3">
            <asp:textbox id="txtIndicazioni" maxlength="240" Width="100%" Runat="server" onblur="toUpper(this)" Visible="false"></asp:textbox>
            <uc1:Anagrafica ID="Indicazioni" runat="server" TipiAnagrafica="IndicazioniFarmaco" ></uc1:Anagrafica>
            
		</td>
		<td class="label">Dosaggio</td>
		<td class="label_left">
			<asp:textbox id="txtDosaggio" onblur="controlloNumero(this)" Width="100%" Runat="server" Enabled="True" MaxLength="5"></asp:textbox> 
		</td>
        <td class="label_left">unità posologiche</td>
		<td class="label">Richiamo</td>
		<td>
            <asp:textbox id="txtRichiamo" onblur="controlloNumero(this)" Width="95%" Runat="server" MaxLength="2"></asp:textbox>
		</td>
    </tr>
</table>

<script type="text/javascript">
    if (<%= Me.Enabled.ToString().ToLower()%>) <%= Me.ClickSospesoFunctionName%>(document.getElementById('<%=Me.ddlSospeso.ClientID%>'), 'S');
   
     
    
</script>