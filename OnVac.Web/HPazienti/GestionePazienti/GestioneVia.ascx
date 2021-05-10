<%@ Control Language="vb" AutoEventWireup="false" Codebehind="GestioneVia.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.GestioneVia" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>


<table id="Table1" cellspacing="0" cellpadding="0" width="100%" border="0">
	<tr>
		<td width="100%">
            <asp:textbox id="txtVia" Width="100%" runat="server"></asp:textbox>
        </td>
		<td>
            <asp:button id="btnIndicazioniVia" runat="server" ToolTip="Definisci indicazioni via" Text="..."></asp:button>
        </td>
	</tr>
	<tr style="HEIGHT: 0px">
		<td colspan="2">
			<on_ofm:onitfinestramodale id="ofmImpostaIndirizzo" title="Impostazione Indirizzo" runat="server" Width="450px" 
                                       IsAnagrafe="False" NoRenderX="False" BackColor="LightGray"  RenderModalNotVisible="True">
				<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="uwtImpostaIndirizzo" runat="server" ItemWidthDefault="90px" CssClass="infratoolbar">
		            <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                    <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
		            <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
					<items>
						<igtbar:TBarButton Text="Conferma" Image="~/Images/conferma.gif" DisabledImage="~/Images/conferma_dis.gif"
							Key="btnConferma" Tag=""></igtbar:TBarButton>
						<igtbar:TBarButton Text="Annulla" Image="~/Images/annulla.gif" DisabledImage="~/Images/annulla_dis.gif"
							Key="btnAnnulla"></igtbar:TBarButton>
					</items>
				</igtbar:UltraWebToolbar>
				<table id="Table2" cellspacing="0" cellpadding="2" width="100%" border="0">
					<tr>
						<td class="label_left" colspan="2">
							<asp:CheckBox id="chkManuale" runat="server" Text="Gestione manuale indirizzo"></asp:CheckBox>
                        </td>
					</tr>
					<tr id="Manuale" runat="server" >
						<td class="label" style="width: 15%">Via</td>
						<td style="width: 85%">
							<asp:TextBox id="txtLibero" onblur="toUpper(this);" runat="server" Width="100%" MaxLength="100" CssClass="TextBox_Stringa"></asp:TextBox>
                        </td>
					</tr>
					<tr id="Automatica" runat="server">
						<td colspan="2">
							<table class="label" style="width:100%">
								<tr>
									<td style="width:15%">Via</td>
									<td style="width:42%">
										<on_ofm:onitmodallist id="omlVia" runat="server" Width="70%" BackColor="LightYellow" AltriCampi="VIA_CIRCOSCRIZIONE as CIRCOSCRIZIONE, CIR_DESCRIZIONE, VIA_CAP as CAP"
											UseTableLayout="True" SetUpperCase="True" UseCode="True" CodiceWidth="30%" RaiseChangeEvent="False"
											CampoCodice="VIA_CODICE" CampoDescrizione="VIA_DESCRIZIONE" Tabella="T_ANA_VIE, T_ANA_CIRCOSCRIZIONI" Obbligatorio="False"
											PosizionamentoFacile="False" LabelWidth="-8px" Connection="" LikeMode="All" IsDistinct="True"></on_ofm:onitmodallist>
                                    </td>
									<td style="width:5%">N°</td>
									<td style="width:18%; text-align:right" >
										<asp:TextBox id="txtNumeroCivico" onblur="toUpper(this);" Width="45px" MaxLength="5" CssClass="TextBox_Stringa" Runat="server"></asp:TextBox>
                                        <span class="TextBox_Stringa" style="WIDTH: 5px">/</span>
										<asp:TextBox id="txtCivicoLettera" onblur="toUpper(this);" Width="15px" MaxLength="1" CssClass="TextBox_Stringa" Runat="server"></asp:TextBox>
                                    </td>
									<td style="width:10%">Int.</td>
									<td style="width:10%">
										<asp:TextBox id="txtInterno" onblur="toUpper(this);" Width="100%" MaxLength="3" CssClass="TextBox_Numero" Runat="server"></asp:TextBox>
                                    </td>
								</tr>
								<tr>
									<td>Lotto</td>
									<td>
										<asp:TextBox id="txtLotto" onblur="toUpper(this);" runat="server" Width="100%" MaxLength="4" CssClass="TextBox_Stringa"></asp:TextBox>
                                    </td>
									<td colspan="2">Palazzina</td>
									<td colspan="2">
										<asp:TextBox id="txtPalazzina" onblur="toUpper(this);" Width="100%" MaxLength="2" CssClass="TextBox_Stringa" Runat="server"></asp:TextBox>
                                    </td>
								</tr>
								<tr>
									<td>Scala</td>
									<td>
										<asp:TextBox id="txtScala" onblur="toUpper(this);" runat="server" Width="100%" MaxLength="2" CssClass="TextBox_Stringa"></asp:TextBox>
                                    </td>
									<td colSpan="2">Piano</td>
									<td colSpan="2">
										<asp:TextBox id="txtPiano" onblur="toUpper(this);" Width="100%" MaxLength="2" CssClass="TextBox_Stringa" Runat="server"></asp:TextBox>
                                    </td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</on_ofm:onitfinestramodale>
        </td>
	</tr>
</table>
<script type="text/javascript">
document.getElementById('<%= txtVia.ClientID %>').onblur=function() {this.value=FullTrim(this.value.toUpperCase())}



if (document.getElementById('<%= chkManuale.ClientID %>')!=null)
{
	document.getElementById('<%= chkManuale.ClientID %>').onclick=function() {ControlloViaManuale<%= ofmImpostaIndirizzo.ClientID %>(this);};
	ControlloViaManuale<%= ofmImpostaIndirizzo.ClientID %>(document.getElementById('<%= chkManuale.ClientID %>'));
}

function ControlloViaManuale<%= ofmImpostaIndirizzo.ClientID %>(el)
{
	var ch=el.checked;
	if (ch)
	{
		document.getElementById('<%= Manuale.ClientID %>').style.display='';
		document.getElementById('<%= Automatica.ClientID %>').style.display='none';
	}
	else
	{
		document.getElementById('<%= Manuale.ClientID %>').style.display='none';
		document.getElementById('<%= Automatica.ClientID %>').style.display='';
	}
}

function ToolBarClickIndirizzo<%= ofmImpostaIndirizzo.ClientID %>(ToolBar,button,evnt)
{
    switch  (button.Key)	
    {
        case 'btnAnnulla':
            closeFm('<%= ofmImpostaIndirizzo.ClientID %>');
            evnt.needPostBack = false;
            break;
			
        case 'btnConferma':
            if (<%= AggiornaCirByVia%>)
            {
                var aggiornaCircoscrizionePaziente = false;

                // Circoscrizione associata alla via selezionata
                var codCircVia = document.getElementById('Ret_<%= omlVia.ClientId %>').value.split("|")[<%= IndexCampiModalListVia.CodiceCircoscrizione %>];
                var descrCircVia = document.getElementById('Ret_<%= omlVia.ClientId %>').value.split("|")[<%= IndexCampiModalListVia.DescrizioneCircoscrizione%>];
                
                if (!IsNullOrEmpty(codCircVia))
                {
                    // Circoscrizione del paziente
                    var codCircPaz = '<%= PazCircoscrizioneCodice%>';

                    // Tipologia indirizzo (residenza/domicilio)
                    var tipoVia;
                    if (<%= (IndirizzoCorrente.Tipo = Onit.OnAssistnet.OnVac.Enumerators.TipoIndirizzo.Residenza).ToString().ToLower() %>)
                    {
                        tipoVia = "residenza";
                    }
                    else
                    {
                        tipoVia = "domicilio";
                    }                    

                    if (IsNullOrEmpty(codCircPaz))
                    {
                        // Se è valorizzata solo la circoscrizione della via selezionata, la circoscrizione del paziente viene impostata in automatico
                        aggiornaCircoscrizionePaziente = true;
                    }
                    else
                    {
                        if (codCircPaz != codCircVia)
                        {
                            aggiornaCircoscrizionePaziente = confirm("ATTENZIONE:\n - Circoscrizione relativa alla via di " + tipoVia + " scelta: " + descrCircVia + "\n - Circoscrizione corrente di " + tipoVia + " del paziente: " + "<%= PazCircoscrizioneDescrizione %>" + "\n\nAggiornare la circoscrizione del paziente a quella della via selezionata (" + descrCircVia + ") ?");
                        }
                    }
                    
                    if (aggiornaCircoscrizionePaziente)
                    {
                        closeFm('<%= ofmImpostaIndirizzo.ClientID %>');
                        evnt.needPostBack = false;
                        __doPostBack("CambiaCircoscrizione_" + tipoVia,"");
                    }
                }
		    }
		    break;
	}
}

function IsNullOrEmpty(stringValue)
{
	if (stringValue == null) return true;
	if (stringValue == '') return true;
	return false;
}

</script>
