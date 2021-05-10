<%@ Control Language="vb" AutoEventWireup="false" Codebehind="ReazAvverseDetail.ascx.vb" Inherits="Onit.OnAssistnet.OnVac.ReazAvverseDetail" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>

<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="uc1" TagName="ReazAvverseFarmaco" Src="ReazAvverseFarmaco.ascx" %>

<%@ Import namespace="Onit.OnAssistnet.OnVac" %>

<link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
<link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
<style type="text/css">
    .rb_large {
        font-size: 12px;
        font-family: Verdana;
    }

    .rb_large_bold {
        font-weight: bold;
        font-size: 12px;
        font-family: Verdana;
    }

    .rb_small {
        font-size: 10px;
        font-family: Verdana;
    }

    .textarea {
        width: 100%;
    }

    .w100px {
        width: 100px;
    }

    .w200px {
        width: 200px;
    }

    .title_1 {
        width: 100%;
        font-family: Verdana;
        font-size: 14px;
        font-weight: bold;
        color: white;
        padding-left: 15px;
        margin-top: 3px;
        padding-top: 2px;
        padding-bottom: 2px;
        background-color: steelblue;
        border: 1px solid navy;
    }

    .section_1 {        
        border: white 5px solid; 
        table-layout: fixed; 
        width: 100%; 
        background-color: whitesmoke;
    }

        .section_1 .label, .label_left, .label_right,
        .TextBox_Stringa, .TextBox_Stringa_Disabilitato, .TextBox_Stringa_Obbligatorio,
        .TextBox_Data, .TextBox_Data_Disabilitato, .TextBox_Data_Obbligatorio,
        .TextBox_Numerico, .TextBox_Numerico_Disabilitato, .TextBox_Numerico_Obbligatorio {
            font-family: Verdana;
        }

    .riquadro {
        border: black 1px solid; 
        background-color: whitesmoke;
        width: 92%;
        margin-top:3px;
    }

        .riquadro .rb_large_bold {
            padding-left: 5px;
        }

    .indented1 {
        width: 100%; 
        padding-left: 20px;
    }

    .button_reazioni {
        width: 100%;
        height: 20px;
        font-family: Verdana;
        font-size: 10px;
        cursor: pointer;
    }

    .separatoreFarmaci {
        width: 100%;
        text-align: center;
        margin-top: 2px;
        margin-bottom: 2px;
        border: 1px solid navy;
    }
</style>

<script type="text/javascript" src='<%= OnVacUtility.GetScriptUrl("onvac.common.js")%>' ></script>
<script type="text/javascript">
    function clickAmbitoOsservazione(isStudioOsservazionale)
    {
        enableTextBox('<%=txtAmbitoStudio_Titolo.ClientID%>', isStudioOsservazionale);
        enableTextBox('<%=txtAmbitoStudio_Tipologia.ClientID%>', isStudioOsservazionale);
        enableTextBox('<%=txtAmbitoStudio_Numero.ClientID%>', isStudioOsservazionale);
    }

    function enableTextBox(id, enable)
    {
        if (enable) {
            document.getElementById(id).disabled = false;
            document.getElementById(id).className = "Textbox_Stringa";
        }
        else {
            document.getElementById(id).disabled = true;
            document.getElementById(id).value = "";
            document.getElementById(id).className = "Textbox_Stringa_Disabilitato";
        }
    }
</script>

<dyp:DynamicPanel ID="dypElencoDate" runat="server" Height="100%" Width="100%" ScrollBars="Auto" RememberScrollPosition="true">

    <div class="title_1" id="LayoutTitolo1" runat="server">Informazioni sulla reazione avversa</div>
    <asp:HiddenField runat="server" ID="txtIdReazione" />
    <asp:HiddenField runat="server" ID="txtCodiceUslAppartenenzaReazione" />

    <dyp:DynamicPanel ID="dypInfoReazione" runat="server" Width="100%" ScrollBars="None" ExpandDirection="horizontal">
        
        <dyp:DynamicPanel ID="DynamicPanel1" runat="server" Width="67%">
            <table id="Table1" cellspacing="0" cellpadding="2" class="section_1">
                <colgroup>
                    <col style="width: 23%" />
                    <col style="width: 22%" />
                    <col style="width: 13%" />
                    <col style="width: 12%" />
                    <col style="width: 16%" />
                    <col style="width: 14%" />
                </colgroup>
                <tr>
		            <td class="label">
                        <!-- Vaccinazione / Associazione (visualizza solo una delle due informazioni) -->
		                <asp:label runat="server" cssClass="label" id="lblVac">Vaccinazione</asp:label>
		                <asp:label runat="server" cssClass="label" id="lblAss">Associazioni</asp:label>
		            </td>
		            <td colspan="4">
			            <!-- Descrizione Vaccinazione / Associazione -->
			            <asp:textbox id="tb_vacDesc" runat="server" CssClass="TextBox_Stringa_Disabilitato" ReadOnly="True"	Width="100%"></asp:textbox>
                        <asp:textbox id="tb_assDesc" runat="server" CssClass="TextBox_Stringa_Disabilitato" ReadOnly="True"	Width="100%"></asp:textbox>
                    </td>
                    <td>
                        <!-- Codice Vaccinazione / Associazione -->
			            <asp:textbox id="tb_vacCod" runat="server" CssClass="TextBox_Stringa_Disabilitato" ReadOnly="True" Width="100%"></asp:textbox>
			            <asp:textbox id="tb_assCod" runat="server" CssClass="TextBox_Stringa_Disabilitato" ReadOnly="True" Width="100%"></asp:textbox>
		            </td>
                </tr>
                <tr>
                    <td class="label">
                        <asp:Label ID="lblDataReazione" runat="server" Text="Data reazione"></asp:Label></td>
                    <td>
                        <on_val:onitdatepick id="dpkDataReazione" runat="server" CssClass="TextBox_Data_Obbligatorio"
                            DateBox="True" Width="100%"></on_val:onitdatepick>
                    </td>
                    <td class="label">
                        <asp:Label ID="lblPeso" runat="server" Text="Peso (kg)"></asp:Label></td>
                    <td>
                        <asp:TextBox id="txtPeso" runat="server" CssClass="TextBox_Numerico" Width="100%" MaxLength="7" onblur="controlloNumero(this)"></asp:TextBox>
                    </td>
                    <td class="label">
                        <asp:Label ID="lblAltezza" runat="server" Text="Altezza (cm)"></asp:Label></td>
                    <td>
						<on_val:OnitJsValidator id="txtAltezza" runat="server" CssClass="TextBox_Numerico" Width="100%"
							actionCorrect="False" actionDelete="False" actionFocus="True" actionMessage="True" actionSelect="False"
							actionUndo="True" autoFormat="True" validationType="Validate_integer" MaxLength="3"
                            PreParams-numDecDigit="0" PreParams-maxValue="999" PreParams-minValue="1"></on_val:OnitJsValidator>
                    </td>
                </tr>
                <tr>
                    <td class="label">Ultima mestruazione</td>
                    <td>
                        <on_val:onitdatepick id="dpkUltimaMestruazione" runat="server" CssClass="TextBox_Data"
                            DateBox="True" Width="100%"></on_val:onitdatepick></td>
                    <td class="label">Origine Etnica</td>
                    <td>
                        <asp:DropDownList ID="ddlOrigineEtnica" CssClass="TextBox_Stringa" Runat="server" Width="100%" DataValueField="OET_CODICE" DataTextField="OET_DESCRIZIONE">
                        </asp:DropDownList>
                        </td>
                    <td  class="label">Allattamento</td>
                    <td>
                        <asp:dropdownlist id="ddlAllattamento" CssClass="TextBox_Stringa" Runat="server" Width="100%">
				            <asp:ListItem Value=""></asp:ListItem>
				            <asp:ListItem Value="N">NO</asp:ListItem>
				            <asp:ListItem Value="S">SI</asp:ListItem>
			            </asp:dropdownlist>
                    </td>
                </tr>
                <tr>
                    <td class="Label">Gravidanza</td>
                    <td colspan="5">
                        <asp:radiobuttonlist id="rblGravidanza" CssClass="TextBox_Stringa" Runat="server" RepeatDirection="Horizontal" Width="100%" TextAlign="right">
				            <asp:ListItem Value="N">No</asp:ListItem>
				            <asp:ListItem Value="1">1° trimestre</asp:ListItem>
				            <asp:ListItem Value="2">2° trimestre</asp:ListItem>
				            <asp:ListItem Value="3">3° trimestre</asp:ListItem>
				            <asp:ListItem Value="0">Sconosciuta</asp:ListItem>
			            </asp:radiobuttonlist>
                    </td>
                </tr>
                <tr>
		            <td class="label">
                        <asp:Label ID="lblTipoReaz" runat="server" Text="Tipologia reazione 1"></asp:Label>
		            </td>
		            <td colspan="5">
                        <on_ofm:onitmodallist id="fm_tipoReaz" runat="server" Width="82%" RaiseChangeEvent="True"
				            Obbligatorio="True" PosizionamentoFacile="False" LabelWidth="-1px" Tabella="t_ana_reazioni_avverse" Connection=""
				            CampoDescrizione="rea_descrizione" CampoCodice="rea_codice" CodiceWidth="17%" UseCode="True" SetUpperCase="True"></on_ofm:onitmodallist></td>                    
                </tr>
                <tr>
		            <td class="label">
                        <asp:Label ID="lblTipoReaz2" runat="server" Text="Tipologia reazione 2"></asp:Label>
		            </td>
		            <td colspan="5">
                        <on_ofm:onitmodallist id="fm_tipoReaz1" runat="server" Width="82%" RaiseChangeEvent="True"
				            Obbligatorio="False" PosizionamentoFacile="False" LabelWidth="-1px" Tabella="t_ana_reazioni_avverse" Connection=""
				            CampoDescrizione="rea_descrizione" CampoCodice="rea_codice" CodiceWidth="17%" UseCode="True" SetUpperCase="True"></on_ofm:onitmodallist></td>
                </tr>
                <tr>
                    <td class="label">
                        <asp:Label ID="lblTipoReaz3" runat="server" Text="Tipologia reazione 3"></asp:Label>
		            </td>
		            <td colspan="5">
                        <on_ofm:onitmodallist id="fm_tipoReaz2" runat="server" Width="82%" RaiseChangeEvent="True"
				            Obbligatorio="False" PosizionamentoFacile="False" LabelWidth="-1px" Tabella="t_ana_reazioni_avverse" Connection=""
				            CampoDescrizione="rea_descrizione" CampoCodice="rea_codice" CodiceWidth="17%" UseCode="True" SetUpperCase="True"></on_ofm:onitmodallist></td>
                </tr>
                <tr>
		            <td class="label">
                        <asp:Label ID="lblTipoReazAltro" runat="server" Text="Specificare Altro"></asp:Label></td>
		            <td colspan="5">
                        <asp:textbox id="tb_altroReaz" maxlength="240" runat="server" Width="99%" onblur="toUpper(this)"
                            TextMode="MultiLine"></asp:textbox></td>
                </tr>
                <tr>
		            <td colspan="6">
                        <div style="margin-top:10px">
                            <asp:Label ID="lblCausa" runat="server" CssClass="rb_large_bold" Text="La reazione osservata deriva da"></asp:Label>
                        </div>
		            </td>
                </tr>
                <tr>
                    <td colspan="6">
                        <asp:radiobuttonlist id="rblCausa" CssClass="TextBox_Stringa" Runat="server" RepeatDirection="Horizontal" RepeatColumns="4" Width="100%" TextAlign="right">
				            <asp:ListItem Value="N">Nessuna</asp:ListItem>
				            <asp:ListItem Value="INT">Interazione</asp:ListItem>
				            <asp:ListItem Value="ERR">Errore Terapeutico</asp:ListItem>
				            <asp:ListItem Value="ABS">Abuso</asp:ListItem>
				            <asp:ListItem Value="MIS">Misuso</asp:ListItem>
				            <asp:ListItem Value="OFF">Off Label</asp:ListItem>
				            <asp:ListItem Value="OVR">Overdose</asp:ListItem>
				            <asp:ListItem Value="ESP">Esposizione Professionale</asp:ListItem>
			            </asp:radiobuttonlist>
                    </td>
                </tr>
                <tr>
		            <td colspan="6">
                        <div style="margin-top:10px">
                            <asp:Label ID="lblVisRic" runat="server" CssClass="rb_large_bold" Text="Eventuali esami di laboratorio rilevanti per ADR"></asp:Label>
                        </div>
		            </td>
                </tr>
                <tr>
                    <td colspan="6" style="vertical-align:top">
                        <asp:textbox id="tb_VisRic" maxlength="240" runat="server" CssClass="Textbox_Stringa" width="99%"
				            TextMode="MultiLine" onblur="toUpper(this)"></asp:textbox>
                    </td>
                </tr>
                <tr>
		            <td colspan="6">
                        <div style="margin-top:10px">
                            <asp:label ID="lblTerapie" runat="server" CssClass="rb_large_bold" Text="Azioni intraprese (specificare)"></asp:label>
                        </div>
		            </td>
                </tr>
                <tr>
                    <td colspan="6" style="vertical-align:top">
                        <asp:textbox id="tb_Terapie" maxlength="240" runat="server" CssClass="Textbox_Stringa" width="99%"
				            TextMode="MultiLine" onblur="toUpper(this)"></asp:textbox>
                    </td>
                </tr>
            </table>
        </dyp:DynamicPanel>

        <dyp:DynamicPanel ID="DynamicPanel2" runat="server" Width="33%">
			<div class="riquadro">
				<table style="width: 99%;">
					<tr>
						<td>
                            <label class="rb_large_bold">Gravità reazione</label>
                        </td>
					</tr>
					<tr>
						<td>
							<asp:radiobutton id="rb_grave" runat="server" CssClass="rb_large" width="100%" Text="GRAVE" GroupName="grave"
								AutoPostBack="True"></asp:radiobutton>
					</tr>
					<tr>
						<td class="indented1">
							<table>
								<tr>
									<td>
                                        <asp:radiobutton id="rb_decesso" runat="server" CssClass="rb_small" width="100%" Text="Decesso" 
                                            GroupName="grave2"></asp:radiobutton></td>
								</tr>
								<tr>
									<td>
                                        <asp:radiobutton id="rb_ospedalizz" runat="server" CssClass="rb_small" width="100%" Text="Ospedalizzazione o prolungamento osp."
											GroupName="grave2"></asp:radiobutton></td>
								</tr>
								<tr>
									<td>
                                        <asp:radiobutton id="rb_invalidita" runat="server" CssClass="rb_small" width="100%" Text="Invalidità grave o permanente"
											GroupName="grave2"></asp:radiobutton></td>
								</tr>
								<tr>
									<td>
                                        <asp:radiobutton id="rb_pericolo" runat="server" CssClass="rb_small" width="100%" Text="Ha messo in pericolo di vita"
											GroupName="grave2"></asp:radiobutton></td>
								</tr>
								<tr>
									<td>
                                        <asp:radiobutton id="rb_anomalie" runat="server" CssClass="rb_small" width="100%" Text="Anomalie congenite/deficit nel neonato"
											GroupName="grave2"></asp:radiobutton></td>
								</tr>
								<tr>
									<td>
                                        <asp:radiobutton id="rb_altro" runat="server" CssClass="rb_small" width="100%" Text="Altra condizione clinicamente rilevante"
											GroupName="grave2"></asp:radiobutton></td>
								</tr>
							</table>
						</td>
					</tr>
					<tr>
						<td>
                            <asp:radiobutton id="rb_noGrave" runat="server" CssClass="rb_large" width="100%" Text="NON GRAVE"
								GroupName="grave" AutoPostBack="True"></asp:radiobutton>
                        </td>
					</tr>
				</table>
			</div>
			<div class="riquadro">
				<table>
					<colgroup>
						<col style="width: 25%" />
						<col style="width: 20%" />
						<col style="width: 55%" />
					</colgroup>
					<tr>
						<td>
                            <label class="rb_large_bold">Esito</label>
						</td>
                        <td class="Label">Data</td>
                        <td>
                            <on_val:onitdatepick id="dpkDataEsito" runat="server" DateBox="True"></on_val:onitdatepick>                            
                        </td>
					</tr>
					<tr>
						<td colspan="3">
                            <asp:radiobutton id="rb_risCompleta" runat="server" CssClass="rb_large" width="100%" Text="RISOLUZIONE COMPLETA"
								GroupName="esito" AutoPostBack="True"></asp:radiobutton></td>
					</tr>
					<tr>
						<td colspan="3">
                            <asp:radiobutton id="rb_risPostumi" runat="server" CssClass="rb_large" width="100%" Text="RISOLUZIONE CON POSTUMI"
								GroupName="esito" AutoPostBack="True"></asp:radiobutton></td>
					</tr>
					<tr>
						<td colspan="3">
                            <asp:radiobutton id="rb_miglioramento" runat="server" CssClass="rb_large" width="100%" Text="MIGLIORAMENTO"
								GroupName="esito" AutoPostBack="True"></asp:radiobutton></td>
					</tr>
					<tr>
						<td colspan="3">
                            <asp:radiobutton id="rb_reazioneInvariata" runat="server" CssClass="rb_large" width="100%" Text="REAZIONE INVARIATA O PEGGIORATA"
								GroupName="esito" AutoPostBack="True"></asp:radiobutton></td>
					</tr>
					<tr>
						<td colspan="3">
                            <asp:radiobutton id="rb_EsitoDecesso" runat="server" CssClass="rb_large" width="100%" Text="DECESSO"
								GroupName="esito" AutoPostBack="True"></asp:radiobutton></td>
					</tr>
					<tr>
						<td colspan="3" class="indented1">
							<table style="width: 100%">
								<tr>
									<td>
                                        <asp:radiobutton id="rb_reazAvversa" runat="server" CssClass="rb_small" width="100%" Text="Dovuto alla reazione avversa"
											GroupName="esito2"></asp:radiobutton></td>
								</tr>
								<tr>
									<td>
                                        <asp:radiobutton id="rb_farmaco" runat="server" CssClass="rb_small" width="100%" Text="Il farmaco può avere contribuito"
											GroupName="esito2"></asp:radiobutton></td>
								</tr>
								<tr>
									<td>
                                        <asp:radiobutton id="rb_noFarmaco" runat="server" CssClass="rb_small" width="100%" Text="Non dovuto al farmaco"
											GroupName="esito2"></asp:radiobutton></td>
								</tr>
								<tr>
									<td>
                                        <asp:radiobutton id="rb_sconosciuta" runat="server" CssClass="rb_small" width="100%" Text="Causa sconosciuta"
											GroupName="esito2"></asp:radiobutton></td>
								</tr>
							</table>
						</td>
					</tr>
					<tr>
						<td colspan="3">
                            <asp:radiobutton id="rb_noDisponibile" runat="server" CssClass="rb_large" width="100%" Text="NON DISPONIBILE"
								GroupName="esito" AutoPostBack="True"></asp:radiobutton></td>
					</tr>
				</table>
			</div>
        </dyp:DynamicPanel>

    </dyp:DynamicPanel>

    <div class="title_1" id="LayoutTitle2" runat="server">Informazioni sui farmaci</div>
    
    <div id="divFarmaciSospetti" style="width:100%">
        <uc1:ReazAvverseFarmaco ID="ucFarmacoSospetto1" runat="server" TipoFarmaco="Sospetto" />
        <uc1:ReazAvverseFarmaco ID="ucFarmacoSospetto2" runat="server" TipoFarmaco="Sospetto" />
        <uc1:ReazAvverseFarmaco ID="ucFarmacoSospetto3" runat="server" TipoFarmaco="Sospetto" />
    </div>
    
    <hr class="separatoreFarmaci" />

    <table cellspacing="2" cellpadding="2" class="section_1">
        <colgroup>
		    <col style="width: 18%" />
		    <col style="width: 7%" />
		    <col style="width: 20%" />
		    <col style="width: 55%" />
        </colgroup>
	    <tr>
		    <td class="rb_large_bold">
                <asp:Label ID="lblFarmacoConcDdl" runat="server" Text="Farmaci concomitanti"></asp:Label>
		    </td>
            <td>
			    <asp:dropdownlist id="ddlFarmaciConcomitanti" Runat="server" CssClass="TextBox_Stringa_Obbligatorio" width="100%" 
                    AutoPostBack="true" OnSelectedIndexChanged="ddlFarmaciConcomitanti_SelectedIndexChanged">
				    <asp:ListItem Value=""></asp:ListItem>
				    <asp:ListItem Value="N">NO</asp:ListItem>
				    <asp:ListItem Value="S">SI</asp:ListItem>
			    </asp:dropdownlist>
            </td>
            <td style="text-align:right;">
                <asp:Button ID="btnRecuperaConcomitanti" runat="server" CssClass="button_reazioni" Text="Recupera" Width="100px" ToolTip="Recupera i dati dei farmaci concomitanti" />
            </td>
            <td></td>
	    </tr>
    </table>

    <div id="divFarmaciConcomitanti" style="width:100%">
        <uc1:ReazAvverseFarmaco ID="ucFarmacoConcomitante1" runat="server" TipoFarmaco="Concomitante" />
        <uc1:ReazAvverseFarmaco ID="ucFarmacoConcomitante2" runat="server" TipoFarmaco="Concomitante" />
    </div>

    <hr class="separatoreFarmaci" />

    <table cellspacing="2" cellpadding="2" class="section_1">
	    <tr>
		    <td class="rb_large_bold">
                <asp:Label ID="lblAltriFarmaci" runat="server" Text="Uso concomitante di altri prodotti a base di piante officinali, omeopatici, integratori alimentari"></asp:Label>
		    </td>
	    </tr>
	    <tr>
		    <td>
                <asp:textbox id="tb_altriFarmaci" maxlength="240" Width="100%" TextMode="MultiLine" Runat="server" onblur="toUpper(this)" CssClass="TextBox_Stringa"></asp:textbox>
		    </td>
	    </tr>
	    <tr>
		    <td class="rb_large_bold">
                <asp:Label ID="lblCondConcomitanti" runat="server" Text="Condizioni concomitanti predisponenti"></asp:Label>
		    </td>
	    </tr>
	    <tr>
		    <td>
                <asp:textbox id="tb_condConcomitanti" maxlength="240" Width="100%" TextMode="MultiLine" Runat="server" onblur="toUpper(this)" CssClass="TextBox_Stringa"></asp:textbox>
		    </td>
	    </tr>
        <tr>
            <td class="rb_large_bold">
                <asp:Label ID="lblAltreInfo" runat="server" Text="Altre Informazioni"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:textbox id="txtAltreInfo" maxlength="240" Width="100%" TextMode="MultiLine" Runat="server" onblur="toUpper(this)" CssClass="TextBox_Stringa"></asp:textbox>
            </td>
        </tr>
    </table>

    <div class="title_1" id="LayoutTitle3" runat="server">Informazioni sulla segnalazione e sul segnalatore</div>
    <table cellpadding="2" cellspacing="0" class="section_1">
        <colgroup>
            <col style="width: 20%" />
            <col style="width: 13%" />
            <col style="width: 33%" />
            <col style="width: 34%" />
        </colgroup>
        <tr>
            <td colspan="4">
                <label class="rb_large_bold">La reazione è stata osservata nell'ambito di</label>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:radiobutton id="rdbAmbito_NonOsservata" runat="server" CssClass="rb_large" Text="Non Osservata" 
                    GroupName="ambito" onclick="clickAmbitoOsservazione(false);" />
            </td>
            <td>
                <asp:RadioButton ID="rdbAmbito_Farmacovigilanza" runat="server" CssClass="rb_large" Text="Progetto di Farmacovigilanza Attiva" 
                    GroupName="ambito" onclick="clickAmbitoOsservazione(false);" />
            </td>
            <td>
                <asp:radiobutton id="rdbAmbito_RegistroFarmaci" runat="server" CssClass="rb_large" Text="Registro Farmaci" 
                    GroupName="ambito" onclick="clickAmbitoOsservazione(false);" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:radiobutton id="rdbAmbito_Studio" runat="server" CssClass="rb_large" width="100%" Text="Studio Osservazionale" 
                    GroupName="ambito" onclick="clickAmbitoOsservazione(true);" />
            </td>
            <td colspan="3">
                <table style="width:100%">
                    <colgroup>
                        <col style="width: 15%" />
                        <col style="width: 20%" />
                        <col style="width: 15%" />
                        <col style="width: 20%" />
                        <col style="width: 15%" />
                        <col style="width: 15%" />
                    </colgroup>
                    <tr>
                        <td class="label">Titolo studio</td>
                        <td>
                            <asp:TextBox ID="txtAmbitoStudio_Titolo" runat="server" CssClass="TextBox_Stringa" Width="100%" MaxLength="100"
                                onblur="toUpper(this)" style="text-transform: uppercase"></asp:TextBox>
                        </td>
                        <td class="label">Tipologia</td>
                        <td>
                            <asp:TextBox ID="txtAmbitoStudio_Tipologia" runat="server" CssClass="TextBox_Stringa" Width="100%" MaxLength="100"
                                onblur="toUpper(this)" style="text-transform: uppercase"></asp:TextBox>
                        </td>
                        <td class="label">Numero</td>
                        <td>
                            <asp:TextBox ID="txtAmbitoStudio_Numero" runat="server" CssClass="TextBox_Stringa" Width="100%" MaxLength="100" 
                                onblur="toUpper(this)" style="text-transform: uppercase"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

    <table cellpadding="2" cellspacing="0" class="section_1">
	    <colgroup>
		    <col style="width: 7%" />
		    <col style="width: 23%" />
		    <col style="width: 7%" />
		    <col style="width: 23%" />
		    <col style="width: 40%" />
	    </colgroup>
	    <tr>
		    <td class="rb_large_bold" colspan="4"><label>Dati del segnalatore</label></td>
		    <td rowspan="6" >
			    <div class="riquadro" style="width:97%">
                    <label class="rb_large_bold">Qualifica del segnalatore</label>
				    <asp:radiobuttonlist id="rbl_qualifica" AutoPostBack="True" CssClass="TextBox_Stringa" Runat="server" RepeatColumns="2">
                        <asp:ListItem Value="PEDIATRA">PEDIATRA DI LIBERA SCELTA</asp:ListItem>
					    <asp:ListItem Value="MEDICO DISTRETTO">MEDICO DISTRETTO</asp:ListItem>
					    <asp:ListItem Value="MEDICO OSPEDALIERO">MEDICO OSPEDALIERO</asp:ListItem>
					    <asp:ListItem Value="MMG">MEDICO MEDICINA GENERALE</asp:ListItem>
					    <asp:ListItem Value="ALTRO">ALTRO</asp:ListItem>
					    <asp:ListItem Value="FARMACISTA">FARMACISTA</asp:ListItem>
					    <asp:ListItem Value="SPECIALISTA">SPECIALISTA</asp:ListItem>
					    <asp:ListItem Value="CAV">CAV</asp:ListItem>
					    <asp:ListItem Value="INF">INFERMIERA/E</asp:ListItem>
				    </asp:radiobuttonlist>
                    <div style="width:100%; text-align:center;">
				        <asp:dropdownlist id="ddl_altroSeg" Runat="server" style="margin-bottom: 3px; width: 90%;" CssClass="TextBox_Stringa">
					        <asp:ListItem Value=""></asp:ListItem>
                            <asp:ListItem Value="MSV">MEDICO SERVIZI VACCINALI</asp:ListItem>
					        <asp:ListItem Value="ASS">ASSISTENTE SANITARIA/O</asp:ListItem>
					        <asp:ListItem Value="OST">OSTETRICA/O</asp:ListItem>
				        </asp:dropdownlist>
                    </div>
                </div>
		    </td>
	    </tr>
	    <tr>
		    <td class="label">Nome</td>
		    <td>
                <asp:textbox id="txt_nomeSeg" maxlength="50" width="100%" Runat="server" onblur="toUpper(this)"></asp:textbox></td>
		    <td class="label">Cognome</td>
		    <td>
                <asp:textbox id="txt_CognomeSeg" maxlength="50" width="100%" Runat="server" onblur="toUpper(this)"></asp:textbox></td>
	    </tr>
	    <tr>
		    <td class="label">Indirizzo</td>
		    <td colspan="3">
                <asp:textbox id="txt_indirizzoSeg" maxlength="100" width="100%" Runat="server" onblur="toUpper(this)"></asp:textbox></td>
	    </tr>
	    <tr>
		    <td class="label">Tel.</td>
		    <td>
                <asp:textbox id="txt_telSeg" maxlength="25" width="100%" Runat="server" onblur="toUpper(this)"></asp:textbox></td>
		    <td class="label">Fax.</td>
		    <td>
                <asp:textbox id="txt_faxSeg" maxlength="25" width="100%" Runat="server" onblur="toUpper(this)"></asp:textbox></td>
	    </tr>
	    <tr>
		    <td class="label">E-mail</td>
		    <td colspan="3">
                <asp:textbox id="txt_emailSeg" maxlength="100" width="100%" Runat="server"></asp:textbox></td>
	    </tr>
        <tr>
            <td class="label">Firma</td>
		    <td colspan="3">
                <asp:textbox id="txtFirmaSegnalatore" maxlength="100" width="100%" Runat="server" onblur="toUpper(this)"></asp:textbox></td>
        </tr>
    </table>

</dyp:DynamicPanel>

<on_ofm:OnitFinestraModale ID="fmRecuperaConcomitanti" Title="Recupero Farmaci Concomitanti" runat="server" Width="600px" BackColor="LightGray" NoRenderX="false">
    <igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="tlbRecupero" runat="server" ItemWidthDefault="80px" CssClass="infratoolbar">
        <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
        <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
	    <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
        <Items>
		    <igtbar:TBarButton Key="btnConfermaRecupero" DisabledImage="~/Images/conferma_dis.gif" Text="Conferma" Image="~/Images/conferma.gif"></igtbar:TBarButton>
	    </Items>
    </igtbar:UltraWebToolbar>
    <div style="width:100%; text-align: center; margin-top:3px;">
        <div class="labelAttenzione">
            ATTENZIONE: premendo il pulsante Conferma, i dati attuali dei farmaci concomitanti verranno sovrascritti con quelli selezionati.
        </div>
    </div>
    <div style="width:100%">
        <asp:Label ID="lblMesiRecupero" runat="server" CssClass="label_left" Text="Mesi precedenti:"></asp:Label>
		<on_val:OnitJsValidator id="txtMesiRecupero" runat="server" CssClass="TextBox_Numerico" Width="100px"
			actionCorrect="False" actionDelete="False" actionFocus="True" actionMessage="True" actionSelect="False"
			actionUndo="True" autoFormat="True" validationType="Validate_integer" MaxLength="3"
            PreParams-numDecDigit="0" PreParams-maxValue="999" PreParams-minValue="1"></on_val:OnitJsValidator>
        <asp:ImageButton ID="btnRefreshRecupero" runat="server" AlternateText="Aggiorna elenco farmaci concomitanti" />

    </div>
    <div style="width:100%; height:350px; overflow:auto; margin-top:3px; border-top:1px solid navy;">
        <asp:DataGrid id="dgrRecupero" runat="server" CssClass="dgr" AutoGenerateColumns="False" BorderWidth="0px" BorderColor="Black"	
            AllowSorting="False" CellPadding="1" ShowHeader="True" >
		    <SelectedItemStyle Font-Bold="True" Wrap="False" CssClass="selected"></SelectedItemStyle>
		    <EditItemStyle Wrap="False"></EditItemStyle>
            <AlternatingItemStyle Wrap="False" CssClass="alternating"></AlternatingItemStyle>
            <ItemStyle Wrap="False" CssClass="item"></ItemStyle>
            <HeaderStyle Font-Bold="True" CssClass="header"></HeaderStyle>
	        <Columns>
		        <asp:TemplateColumn>
			        <ItemStyle HorizontalAlign="Center" Width="25px" ></ItemStyle>
                    <HeaderStyle Width="25px" />
			        <ItemTemplate>
                        <div style="text-align:center;">
					        <asp:checkBox id="cb" runat="server" class="margin_btn" ></asp:checkBox>
                        </div>
			        </ItemTemplate>
		        </asp:TemplateColumn>
                <asp:BoundColumn DataField="DataOraEsecuzioneVaccinazione" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy HH:mm}" HeaderStyle-Width="20%"></asp:BoundColumn>
                <asp:BoundColumn DataField="DescrizioneAssociazione" HeaderText="Associazione" HeaderStyle-Width="40%"></asp:BoundColumn>
                <asp:BoundColumn DataField="DoseAssociazione" HeaderText="Dose" HeaderStyle-Width="15%"></asp:BoundColumn>
                <asp:BoundColumn DataField="DescrizioneNomeCommerciale" HeaderText="Nome Commerciale" HeaderStyle-Width="25%"></asp:BoundColumn>
                <asp:BoundColumn DataField="CodiceAssociazione" Visible="false"></asp:BoundColumn>
                <asp:BoundColumn DataField="CodiceLotto" Visible="false"></asp:BoundColumn>
                <asp:BoundColumn DataField="CodiceSitoInoculazione" Visible="false"></asp:BoundColumn>
                <asp:BoundColumn DataField="DescrizioneSitoInoculazione" Visible="false"></asp:BoundColumn>
                <asp:BoundColumn DataField="CodiceViaSomministrazione" Visible="false"></asp:BoundColumn>
                <asp:BoundColumn DataField="DescrizioneViaSomministrazione" Visible="false"></asp:BoundColumn>
                <asp:BoundColumn DataField="VesId" Visible="false"></asp:BoundColumn>
                <asp:BoundColumn DataField="DataScadenzaLotto" Visible="false"  DataFormatString="{0:dd/MM/yyyy}"></asp:BoundColumn>
                <asp:BoundColumn DataField="CodiceNomeCommerciale" Visible="false" HeaderText="Codice Nome Commerciale" ></asp:BoundColumn>
            </Columns>
        </asp:DataGrid>
        <asp:Label ID="lblNoResult" runat="server" CssClass="label" style="width:100%; text-align:center; font-weight:bold; margin-top: 30px" Text="Nessun risultato ottenuto nell'intervallo specificato"></asp:Label>
    </div>
</on_ofm:OnitFinestraModale>

