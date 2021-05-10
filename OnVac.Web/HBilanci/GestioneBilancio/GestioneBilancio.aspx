<%@ Page Language="vb" AutoEventWireup="false" Codebehind="GestioneBilancio.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.GestioneBilancio" %>

<%@ Import namespace="Onit.OnAssistnet.OnVac" %>

<%@ Register TagPrefix="uc1" TagName="RicercaBilancio" Src="../../Common/Controls/RicercaBilancio.ascx" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Assembly="Onit.Web.UI.WebControls.Validators" Namespace="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>
<%@ Register TagPrefix="dyp" Assembly="Onit.Web.UI.WebControls.DynamicPanel" Namespace="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="uc2" TagName="QuestionarioBilancio" Src="../QuestionarioBilancio.ascx" %>
<%@ Register TagPrefix="uc1" TagName="FirmaDigitale" Src="../../Common/Controls/FirmaDigitaleArchiviazioneSostitutiva.ascx" %>
<%@ Register TagPrefix="uc4" TagName="DatiOpzionaliBilancio" Src="../DatiOpzionaliBilancio.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
	<head>
		<title>Gestione Anamnesi</title>

		<link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
        <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />

        <!-- #include file="../../Common/Scripts/AggiornaStatoVaccinazioni.inc"-->
		
        <script type="text/javascript" src='<%= OnVacUtility.GetScriptUrl("onvac.common.js")%>'></script>
        <script type="text/javascript">
		
            function InizializzaToolBar(t)
            {
                t.PostBackButton=false;
            }

            function ToolBarClick(ToolBar, button, evnt)
            {
                evnt.needPostBack=true;
			
                switch(button.Key)
                {
                    case 'btnAnnulla':

                        if (!confirm("Si desidera annullare le modifiche?"))
                            evnt.needPostBack=false;
                        break;

                    case 'btnSalva':
                    case 'btnSalvaFirma':
                    case 'btnSalvaStampa':

                        var message = GetMessaggioErroreCampiObbligatori();

                        if (message != '')
                        {
                            alert(message);
                            evnt.needPostBack=false;
                        }
                        break;

                    default:
                        evnt.needPostBack=true;
                        break;
                }
            }
            
            function GetMessaggioErroreCampiObbligatori()
            {
                var alertMessage = "";

                if (document.getElementById('txtMedico').value == '')
                {
                    alertMessage += ' - Medico\n'
                }

                if (document.getElementById('fmRilevatore').value == '')
                {
                    alertMessage += ' - Operatore\n'
                }

                if (OnitDataPickGet('txtData') == '')
                {
                    alertMessage += ' - Data\n';
                }

                if (alertMessage != '') 
                {
                    return "Salvataggio non effettuato. I seguenti campi obbligatori non sono stati valorizzati:\n" + alertMessage;
                }

                return '';
            }
			
            // abilita/disabilita il campo di fine sospensione [modifica 04/04/2005]
            function GestisciFineSospensione()
            {
                var vaccinabile = document.getElementById('ddlVaccinabile').value;
                if (vaccinabile == 'N')
                {
                    OnitDataPickEnable('odpFineSospensione',true);
                    var dataCorrente = new Date();
                    var giorniData = dataCorrente.getDay().toString();
                    if (giorniData.length == 1)
                        giorniData = '0' + giorniData;
                    var mesiData = (dataCorrente.getMonth() + 1).toString();
                    if (mesiData.length == 1)
                        mesiData = '0' + mesiData;
                    var dataCorrenteFormat = giorniData + '/' + mesiData + '/' + dataCorrente.getFullYear();
                    OnitDataPickSet('odpFineSospensione',dataCorrenteFormat);	
                }	
                else
                {
                    OnitDataPickEnable('odpFineSospensione',false);
                    OnitDataPickSet('odpFineSospensione','');	
                }
            }
			
            //controlla che il valore sia numerico [modifica 05/04/2005]
            function ControllaValore(oggetto, tipo, altroCampo, interi, decimali)
            {
                var valore = oggetto.value;
                if (valore != '')
                {
                    valore = valore.replace(',','.');
					
                    if (isNaN(valore))
                    {
                        AssegnaValoreFormattato(oggetto,valore);
                        alert('Attenzione: il valore inserito non è numerico!');
                        oggetto.focus();
                        return;
                    }
					
                    valore = valore.replace('.',',');
                    if (valore.search(',') != -1)
                    {
                        AssegnaValoreFormattato(oggetto,valore);
                        //controllo che il formato interi/decimali venga rispettato per il campo [modifica 13/07/2005]
                        if ((interi != null) && (decimali != null))
                        {
                            var valoreIntero = valore.split(',')[0];
                            var valoreDecimale = valore.split(',')[1];
                            if ((valoreIntero.length > interi) || (valoreDecimale.length > decimali))
                            {
                                MessaggioErrore(interi,decimali);
                                oggetto.focus();
                            }
                        }
                        //Maurizio 23-05-05
                        return;
                    }
                    else
                    {
                        if (valore.length > interi)
                        {
                            MessaggioErrore(interi,decimali);
                            oggetto.focus();
                        }	
                    }
											
                    if (parseFloat(valore) == 0)
                    {
                        alert("Attenzione: il valore del campo '" + tipo + "' non può essere nullo!");
                        oggetto.focus();
                        return;
                    }
                    //Maurizio 23-05-05
                    //if ((altroCampo != ''))
                    //{
                    //if (tipo == 'peso')
                    //CalcolaPercentile(valore,altroCampo);
                    //else
                    //CalcolaPercentile(altroCampo,valore);			
                    //}
                    //else
                    //document.getElementById('txtPercentile').value = '';
										
                }
                //else
                //document.getElementById('txtPercentile').value = '';
            }
			
            //genera il messaggio con il formato del numero relativo [modifica 13/07/2005]
            function MessaggioErrore(interi, decimali)
            {
                var formatoIntero = '';
                var formatoDecimale = '';
                for (i=0;i<interi;i++)
                {
                    formatoIntero += '0';
                }
                for (i=0;i<decimali;i++)
                {
                    formatoDecimale += '0';
                }
                alert('Attenzione: il formato del numero deve essere [' + formatoIntero + ',' + formatoDecimale + '].');
            }
			
            //assegna il valore corretto al campo selezionato [modifica 05/04/2005]
            function AssegnaValoreFormattato(oggetto, valore)
            {
                valore = valore.replace(',','.');
                oggetto.value = valore;
            }
			
            /*------------------------------------------------------------------
			calcola in automatico il valore del percentile [modifica 05/04/2005]
			(il calcolo deve essere ancora stabilito) 
			------------------------------------------------------------------*/
            /*function CalcolaPercentile(peso, altezza)
			{
				//esempio di calcolo (da modificare successivamente)
				//alert('peso --> ' + peso + '\raltezza -->' + altezza);
				var percentile = peso/altezza;
				
				//controllo sui decimali del percentile
				var valorePercentile = percentile.toString().replace('.',',');
				if (valorePercentile.search(',') != -1)
				{
					if (valorePercentile.split(',')[1].length > 2)
					{
						if (valorePercentile.split(',')[1].charAt(2) <= 5)
							percentile = valorePercentile.split(',')[0] + '.' + valorePercentile.split(',')[1].substr(0,2);
						else
							percentile = (parseFloat(valorePercentile.split(',')[0] + '.' + valorePercentile.split(',')[1].substr(0,2)) + 0.01);
					}
				}
			
				document.getElementById('txtPercentile').value = percentile;
				
			}*/
		</script>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
			<on_lay3:onitlayout3 id="OnitLayout31" Titolo="Gestione Anamnesi" runat="server" width="100%" height="100%" HeightTitle="90px">
                <asp:MultiView ID="multiViewMain" runat="server" ActiveViewIndex="0" >

                    <asp:View id="viewDati" runat="server" >
							
                        <div class="Title" id="divLayoutTitolo" style="width: 100%">
							<asp:Label id="LayoutTitolo" runat="server" Width="100%" CssClass="title" BorderStyle="None"></asp:Label>
                        </div>

						<igtbar:UltraWebToolbar BrowserTarget="UpLevel" id="ToolBar" runat="server" ItemWidthDefault="90px" CssClass="infratoolbar">
                            <DefaultStyle CssClass="infratoolbar_button_default"></DefaultStyle>
                            <HoverStyle CssClass="infratoolbar_button_hover"></HoverStyle>
	                        <SelectedStyle CssClass="infratoolbar_button_selected"></SelectedStyle>
                            <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
							<Items>
								<igtbar:TBarButton Key="btnBilancio" DisabledImage="../../images/bilanci_dis.gif" Text="Scegli Bilancio"
									Image="../../images/bilanci.gif" ToolTip="Seleziona l'anamnesi da compilare" >
                                    <DefaultStyle Width="120px" CssClass="infratoolbar_button_default"></DefaultStyle>
								</igtbar:TBarButton>
								<igtbar:TBarButton Key="btnCompilaBilancio" DisabledImage="../../Images/compilaBilancio_dis.gif" Text="Compila"
									Image="../../Images/compilaBilancio.gif" ToolTip="Compilazione anamnesi con i valori di default"></igtbar:TBarButton>
								<igtbar:TBSeparator></igtbar:TBSeparator>
								<igtbar:TBarButton Key="btnSalva" DisabledImage="../../images/salva_dis.png" Text="Salva" 
                                    Image="../../images/salva.png" ToolTip="Salva i dati di anamnesi inseriti">
                                    <DefaultStyle Width="70px" CssClass="infratoolbar_button_default"></DefaultStyle>
								</igtbar:TBarButton>
								<igtbar:TBarButton Key="btnSalvaFirma" Text="Salva e firma" DisabledImage="../../images/salvaFirma_dis.png" 
                                    Image="../../images/salvaFirma.png" ToolTip="Salva il documento e lo firma digitalmente">
									<DefaultStyle Width="100px" CssClass="infratoolbar_button_default"></DefaultStyle>
								</igtbar:TBarButton>
								<igtbar:TBarButton Key="btnSalvaStampa" Text="Salva e stampa" DisabledImage="../../images/salvaStampa_dis.png" 
                                    Image="../../images/salvaStampa.png" ToolTip="Salva l'anamnesi e apre l'anteprima di stampa">
									<DefaultStyle Width="110px" CssClass="infratoolbar_button_default"></DefaultStyle>
								</igtbar:TBarButton>
								<igtbar:TBarButton Key="btnAnnulla" DisabledImage="../../images/annulla_dis.gif" Text="Annulla"
									Image="../../images/annulla.gif" ToolTip="Annulla l'inserimento dell'anamnesi selezionata">
                                    <DefaultStyle Width="70px" CssClass="infratoolbar_button_default"></DefaultStyle>
								</igtbar:TBarButton>
                                <igtbar:TBSeparator></igtbar:TBSeparator>
                                <igtbar:TBarButton Key="btnRecuperaStoricoVacc" Text="Recupera" DisabledImage="../../images/recupera_dis.png"
								    Image="../../images/recupera.png" ToolTip="Recupera lo storico vaccinale centralizzato del paziente">
							    </igtbar:TBarButton>
							</Items>
						</igtbar:UltraWebToolbar>

						<div class="Sezione">Testata Visita</div>

                        <dyp:DynamicPanel ID="DynamicPanel1" runat="server" Width="100%" Height="180px" ScrollBars="None">
						    <table id="table_testata_visiva" style="table-layout:fixed;" cellspacing="0" cellpadding="2" width="100%" border="0">
                                <colgroup>
                                    <col width="10%" />
                                    <col width="18%" />
                                    <col width="18%" />
                                    <col width="4%" />
                                    <col width="9%" />
                                    <col width="12%" />
                                    <col width="12%" />
                                    <col width="16%" />
                                    <col width="1%" />
                                </colgroup>
							    <tr>
								    <td class="label">Malattia</td>
								    <td class="label_left" colspan="5">
									    <on_ofm:onitmodallist id="txtMalattia" runat="server" Width="70%" Height="24px" CodiceWidth="29%" CampoCodice="MAL_CODICE" CampoDescrizione="MAL_DESCRIZIONE" Tabella="T_ANA_MALATTIE" UseCode="False" SetUpperCase="False" LabelWidth="-8px" PosizionamentoFacile="False" Label="Malattia" Enabled="False" Obbligatorio="False"></on_ofm:onitmodallist></td>
								    <td class="label">Numero</td>
								    <td class="label_left">
									    <on_val:OnitJsValidator id="txtNumero" runat="server" Width="100%" CssClass="TextBox_Numerico_Disabilitato" ReadOnly="True" actionCorrect="False" actionDelete="False" actionFocus="False" actionMessage="True" actionSelect="True" actionUndo="False" autoFormat="False" validationType="Validate_integer" PreParams-numDecDigit="0" PreParams-maxValue="null" PreParams-minValue="0"></on_val:OnitJsValidator></td>
                                    <td></td>
							    </tr>
							    <tr>
								    <td class="label">Medico</td>
								    <td class="label_left" colspan="5">
									    <on_ofm:onitmodallist id="txtMedico" runat="server" Width="70%" Height="24px" CodiceWidth="29%" CampoCodice="OPE_CODICE" CampoDescrizione="OPE_NOME" Tabella="T_ANA_OPERATORI,T_ANA_LINK_OPER_CONSULTORI" UseCode="True" SetUpperCase="True" LabelWidth="-8px" PosizionamentoFacile="False" Label="Medico" Obbligatorio="True"></on_ofm:onitmodallist></td>
								    <td class="label">Firmato da</td>
								    <td class="label_left">
									    <asp:TextBox id="txtFirmaBil" runat="server" Width="100%" CssClass="TextBox_Stringa_Disabilitato" ReadOnly="True"></asp:TextBox></td>
                                    <td></td>
							    </tr>
                                <tr>
								    <td class="label">Operatore</td>
								    <td class="label_left" colspan="5">
                                        <on_ofm:onitmodallist id="fmRilevatore" runat="server" Width="70%" Height="24px" CodiceWidth="29%" CampoCodice="OPE_CODICE" CampoDescrizione="OPE_NOME" Tabella="T_ANA_OPERATORI,T_ANA_LINK_OPER_CONSULTORI" UseCode="True" SetUpperCase="True" LabelWidth="-8px" PosizionamentoFacile="False" Label="Operatore" Obbligatorio="True"></on_ofm:onitmodallist></td>
                                    <td colspan="3"></td>
                                </tr>
							    <tr>
								    <td class="label">Data</td>
								    <td class="label_left">
                                        <on_val:onitdatepick id="txtData" runat="server" Width="130px" CssClass="TextBox_Data_Obbligatorio" DateBox="True" BorderColor="White"></on_val:onitdatepick>
								    </td>
                                    <td class="label">
                                        <asp:Label ID="lblFlagVisibilita" runat="server" CssClass="label" Text="<%$ Resources: Onit.OnAssistnet.OnVac.Web, DatiVaccinali.ConsensoComunicazione %>"></asp:Label>
                                    </td>
                                    <td class="label_left">
                                        <asp:CheckBox id="chkFlagVisibilita" CssClass="Label" Runat="server" TextAlign="Left" Text="" AutoPostBack="false" ToolTip="Consenso alla comunicazione dei dati vaccinali da parte del paziente" ></asp:CheckBox>
                                    </td>
								    <td class="label">Vaccinabile</td>
								    <td class="label_left">
									    <asp:DropDownList id="ddlVaccinabile" runat="server" Width="100%" DataValueField="Codice" DataTextField="Descrizione"></asp:DropDownList></td>
								    <td class="label" >Fine Sospensione</td>
								    <td class="label_left">
									    <on_val:onitdatepick id="odpFineSospensione" runat="server" Width="130px" CssClass="TextBox_Data" Height="22px" DateBox="True" BorderColor="White"></on_val:onitdatepick></td>
                                    <td></td>
							    </tr>
                                <tr>
								    <td class="label">Note</td>
								    <td class="label_left" colspan="7">
                                        <asp:TextBox ID="txtNote" runat="server" CssClass="TextBox_Stringa" Width="100%" Rows="3" MaxLength="2000" TextMode="MultiLine"></asp:TextBox></td>
                                    <td></td>
                                </tr>
						    </table>
                        </dyp:DynamicPanel>

                        <uc4:DatiOpzionaliBilancio id="ucDatiOpzionaliBilancio" runat="server" Visible="true" UseUpperCaseCaption="false"></uc4:DatiOpzionaliBilancio>

						<div class="Sezione">Dettaglio Bilancio</div>

                        <dyp:DynamicPanel ID="DynamicPanel2" runat="server" Width="100%" Height="100%" ScrollBars="Auto">
                            <div id="divMisure" runat="server" >
							    <table id="Table_Dettaglio_Bilancio" cellspacing="1" cellpadding="0" width="100%" border="0">
								    <tr style="HEIGHT: 29px">
									    <td class="label" width="10%">
										    <asp:Label id="lblPeso" runat="server" Width="100%" CssClass="label_right">Peso(kg)</asp:Label></td>
									    <td width="60%">
										    <table id="tabBilancio" height="100%" cellspacing="1" cellpadding="0" width="100%" border="0">
											    <tr>
												    <td width="15%">
													    <asp:TextBox id="txtPeso" runat="server" Width="60px" CssClass="TextBox_Numerico"></asp:TextBox></td>
												    <td class="label" width="17%">
													    <asp:Label id="lblAltezza" runat="server" Width="100%" CssClass="label_right">Altezza(cm)</asp:Label></td>
												    <td width="12%">
													    <asp:TextBox id="txtAltezza" runat="server" Width="60px" CssClass="TextBox_Numerico"></asp:TextBox></td>
												    <td class="label" width="27%">
													    <asp:Label id="lblCranio" runat="server" Width="100%" CssClass="label_right">Circonferenza cranica(cm)</asp:Label></td>
												    <td width="15%">
													    <asp:TextBox id="txtCranio" runat="server" Width="60px" CssClass="TextBox_Numerico"></asp:TextBox></td>
											    </tr>
										    </table>
									    </td>
								    </tr>
							    </table>
                            </div>
						    <div id="divPercentili" runat="server" >
							    <table id="table_Percentili" cellspacing="0" cellpadding="0" width="100%" border="0">
								    <tr style="HEIGHT: 29px">
									    <td class="label" width="10%">
										    <asp:Label id="lblPercentilePeso" runat="server" Width="100%" CssClass="label_right">Percentile</asp:Label></td>
									    <td width="60%">
										    <table id="tabBilancio2" height="100%" cellspacing="1" cellpadding="0" width="100%" border="0">
											    <tr>
												    <td width="15%">
													    <asp:TextBox id="txtPercentilePeso" runat="server" Width="60px" CssClass="TextBox_Numerico_Disabilitato"
														    ReadOnly="True"></asp:TextBox></td>
												    <td class="label" width="17%">
													    <asp:Label id="lblPercentileAltezza" runat="server" Width="100%" CssClass="label_right">Percentile</asp:Label></td>
												    <td width="12%">
													    <asp:TextBox id="txtPercentileAltezza" runat="server" Width="60px" CssClass="TextBox_Numerico_Disabilitato"
														    ReadOnly="True"></asp:TextBox></td>
												    <td class="label" width="27%">
													    <asp:Label id="lblPercentileCranio" runat="server" Width="100%" CssClass="label_right">Percentile</asp:Label></td>
												    <td width="15%">
													    <asp:TextBox id="txtPercentileCranio" runat="server" Width="60px" CssClass="TextBox_Numerico_Disabilitato"
														    ReadOnly="True"></asp:TextBox></td>
											    </tr>
										    </table>
									    </td>
								    </tr>
							    </table>
						    </div>

                            <div>
                                <uc2:QuestionarioBilancio id="Questionario" runat="server" Width="100%" Enabled="true" visible="true" />
                            </div>
                        </dyp:DynamicPanel>

                    </asp:View>

                    <asp:View ID="viewFirma" runat="server">
                        <dyp:DynamicPanel ID="dypFirma" runat="server" Width="100%" Height="100%" ExpandDirection="vertical" ScrollBars="None">
                            <div style="width: 100%" id="divLayoutTitoloViewFirma" class="Title">
					            <asp:Label id="LayoutTitoloViewFirma" runat="server" BorderStyle="None" Width="100%" CssClass="title"></asp:Label>
				            </div>                        
                            <uc1:FirmaDigitale ID="ucFirma" runat="server" MostraPulsanteIndietro="false" />
                        </dyp:DynamicPanel>
                    </asp:View>

                </asp:MultiView>
			</on_lay3:onitlayout3>
			
            <on_ofm:onitfinestramodale id="modRicBil" title="Selezione anamnesi da compilare" runat="server" width="617px" BackColor="LightGray">
				<uc1:RicercaBilancio id="uscRicBil" runat="server" IncludiNessunaMalattia="True" SoloTipologieMalattiaPerCompilazioneBilanci="true"></uc1:RicercaBilancio>
			</on_ofm:onitfinestramodale>

			<on_ofm:onitfinestramodale id="fmErrore" title="Spostamento convocazione" runat="server"  width="640px" BackColor="LightGray" NoRenderX="true">
				<p class="label">
					<asp:Label id="lblErrorMessage" runat="server" CssClass="Label_ErrorMsg" Width="100%" Height="40px" text="Salvataggio annullato!" ></asp:Label>
				</p> 
				<p align="center">
					<input type="button"  style="width:93px;" value="OK" onclick="closeFm('fmErrore');" />
				</p>
			</on_ofm:onitfinestramodale>

			<on_ofm:onitfinestramodale id="fmUnisciCnv" title="Spostamento convocazione" runat="server"  width="640px" BackColor="LightGray" NoRenderX="true" >
				<p class="label_left">
					<table id="Table2" cellspacing="1" cellpadding="1" width="100%" border="0">
						<tr>
							<td style="width: 47px" align="center">
                                <img alt="" runat="server" src="~/Images/alert.gif" align="absMiddle" />
                            </td>
                            <td class="label_left">Alcune convocazioni saranno spostate perchè la loro data di 
								inizio è inferiore a quella di fine sospensione. E' possibile che alcune 
								convocazioni siano unite se in data di fine sospensione si sovrappongono. 
                                Se le convocazioni unite hanno centri vaccinali diversi, verrà utilizzato il centro 
                                relativo alla prima convocazione (o quello della convocazione nella data di unione, se già presente).
								<br />Continuare?</td>
						</tr>
					</table>
				</p>
				<p align="center">
					<asp:Button id="btnSpostaOK" runat="server" Width="93px" Text="OK"></asp:Button>&nbsp;
					<asp:Button id="btnSpostaAnnulla" runat="server" Width="93px" Text="Annulla"></asp:Button>
				</p>
			</on_ofm:onitfinestramodale>

		</form>

		<script type="text/javascript">

		    document.getElementById("ddlVaccinabile").onchange = function() {GestisciFineSospensione();};
				
		    if (document.getElementById("ddlVaccinabile").value != 'N') OnitDataPickEnable('odpFineSospensione',false);
			
		    if (<%= Me.txtPeso.Visible.ToString().ToLower() %>) 
					document.getElementById("txtPeso").onblur = function() {ControllaValore(this, 'peso', document.getElementById('txtAltezza').value, 3, 2);};
				if (<%= Me.txtAltezza.Visible.ToString().ToLower() %>) 
                document.getElementById("txtAltezza").onblur = function() {ControllaValore(this, 'altezza', document.getElementById('txtPeso').value, 3, 1);};
		    if (<%= Me.txtCranio.Visible.ToString().ToLower() %>) 
                document.getElementById("txtCranio").onblur = function() {ControllaValore(this, 'cranio', document.getElementById('txtCranio').value, 2, 1);};

		</script>

	</body>

</html>
