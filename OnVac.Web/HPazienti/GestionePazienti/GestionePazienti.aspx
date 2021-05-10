<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="GestionePazienti.aspx.vb" Inherits="Onit.OnAssistnet.OnVac.GestionePazienti" ValidateRequest="false" %>

<%@ Register TagPrefix="ondp" Namespace="Onit.Controls.OnitDataPanel" Assembly="OnitDataPanel" %>
<%@ Register TagPrefix="dyp" Namespace="Onit.Web.UI.WebControls.DynamicPanel" Assembly="Onit.Web.UI.WebControls.DynamicPanel" %>
<%@ Register TagPrefix="on_lay3" Namespace="Onit.Controls.PagesLayout" Assembly="Onit.Controls.PagesLayout.OnitLayout3" %>
<%@ Register TagPrefix="on_val" Namespace="Onit.Web.UI.WebControls.Validators" Assembly="Onit.Web.UI.WebControls.Validators" %>
<%@ Register TagPrefix="on_ofm" Namespace="Onit.Controls" Assembly="Onit.Controls.OnitFinestraModale" %>

<%@ Register TagPrefix="uc1" TagName="OnVacSceltaCicli" Src="OnVacSceltaCicli.ascx" %>
<%@ Register TagPrefix="uc1" TagName="GestioneVia" Src="GestioneVia.ascx" %>
<%@ Register TagPrefix="uc1" TagName="GestionePazientiMessage" Src="GestionePazientiMessage.ascx" %>
<%@ Register TagPrefix="uc1" TagName="GestionePazientiDatiSanitari" Src="GestionePazientiDatiSanitari.ascx" %>
<%@ Register TagPrefix="uc1" TagName="CalVac" Src="CalVac.ascx" %>

<%@ Import Namespace="Onit.OnAssistnet.OnVac" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>
<head>
    <title>GestionePazienti</title>

    <link href='<%= ResolveClientUrl("~/css/StylesPortale.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/Style_OnVac.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/toolbar.css") %>' type="text/css" rel="stylesheet" />
    <link href='<%= ResolveClientUrl("~/css/default/toolbar.default.css") %>' type="text/css" rel="stylesheet" />

    <style type="text/css">
        .textarea {
            width: 100%;
        }

        .w100px {
            width: 100px;
        }

        .w200px {
            width: 200px;
        }

        .blocco_dati {
            border: 1px solid steelblue;
            border-radius: 3px;
            background-color: whitesmoke;
        }

        /*.paddingNote {
            padding: 3px;
        }*/

        .toolbar-libretti-default {
            border: 1px solid aliceblue;
            border-radius: 5px;
            color: white;
        }

        .toolbar-libretti-hover {
            border: 1px solid #058;
            border-radius: 5px;
            background-color: lightblue;
            color: #058;
            cursor: pointer;
        }

        .toolbar-libretti-selected {
            border: 1px solid lightgreen;
            border-radius: 5px;
            color: lightgreen;
            background-color: teal;
        }
    </style>

    <script type="text/javascript" src="ScriptGestionePazienti.js"></script>
    <script type='text/javascript' src='<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Utility.js") %>'></script>
    <script type="text/javascript" src="<%= ClientScript.GetWebResourceUrl(GetType(Onit.Shared.Web.Static.WebResources), "Onit.Shared.Web.Static.Scripts.Jquery171.Min.js") %>"></script>
    <script type="text/javascript">

        var CODICI_STATI_ANAG_CANC_PROG = '<%= Me.StatiAnagraficiCancellazioneProgrammazione %>';
    
    function OnClientButtonClicking(sender, args) 
    {
        if (!e) var e = window.event;

        var button = args.get_item();
        switch (button.get_value())	
        { 
            case 'btnLibrettoVaccinale':
                args.set_cancel(true);
                showFm('fmSelectStampaLibrettoVaccinale',true,250);
                break;

            case 'btnSalva':
                // se è stata modificata la sede vaccinale corrente, chiede se si vuole continuare
                document.getElementById('txtControlloConfermaSalva').value='True';
                
                if (document.getElementById('txtSedeVaccinale_Cod').value != '<%= CodConsVaccinale %>') {
			        if (!("<%= CodConsVaccinale %>"=="")) {
			            if (!confirm('ATTENZIONE: e\' stata modificata la sede vaccinale del paziente.\nEffettuando il salvataggio, verra\' MODIFICATA automaticamente la sede vaccinale di tutte le convocazioni del paziente relative alla sede precedente.\nVerranno inoltre ELIMINATI eventuali appuntamenti sulla sede precedente.\n\nContinuare?')) {
			                // Deve andare al server per reimpostare data assegnazione, sede vaccinale corrente e precedente.
			                document.getElementById('txtControlloConfermaSalva').value = 'False';
			            }
			        }
			    }
	    			
                // calcolo automatico del codice fiscale [modifica 29/07/2005]
                if (<%= CanCalculateCodiceFiscale.ToString().ToLower() %>) {
				    var codiceFiscale = document.getElementById('txtCodiceFiscale');	
                    CalcoloCodiceFiscale(codiceFiscale);
                }
	    			
                // verifica dell'allineamento delle modali
                if (!controllaValiditaFM()) {
                    args.set_cancel(true);
                }
	    										
                // controllo sullo stato della Categoria Rischio
                var categoriaRischio = document.getElementById("txtCategorieRischio_Cod").value;
                if ('<%= CategoriaRischio %>'!= categoriaRischio) {
			        var msgCategoriaRischio = 'al paziente verrà associata la categoria di rischio selezionata.';

			        if (categoriaRischio == '') msgCategoriaRischio = 'al paziente verrà eliminata la categoria di rischio.';

			        if (confirm("Attenzione: confermando, " + msgCategoriaRischio + "\rQuesto implica la CANCELLAZIONE DELLA PROGRAMMAZIONE VACCINALE relativa al paziente, ad esclusione delle convocazioni con avviso di appuntamento già spedito e/o solleciti (per queste ultime sarà necessaria la gestione manuale).\rSi desidera continuare?")) {
			            // eliminazione
			            if (categoriaRischio=='')
			                __doPostBack('EliminazioneProgrammazioneRischio','');
			            else
			                __doPostBack('EliminazioneProgrammazioneRischio',categoriaRischio);
			            args.set_cancel(true);
			        }
			        else
			            args.set_cancel(true);
			    }
	    			
                var gestioneCorrente = document.getElementById("chkDaCompletare");

                // controllo sullo stato della Gestione Manuale
                if (gestioneCorrente != null) {
                    if ((gestioneCorrente.checked)&&('<%= GestioneManuale %>'=="N")) {
                        // conferma eliminazione della programmazione
                        if (confirm("Si desidera eliminare la programmazione associata al paziente?")) {
                            // eliminazione
                            __doPostBack('EliminazioneProgrammazione','dgrCicli');
                            args.set_cancel(true);
                        }
                        else {
                            // controllo sull'eventuale presenza di cicli da eliminare
                            if (<%= (gestionePazientiDatiSanitari.dtaCicliEliminati.Rows.Count <> 0).ToString().ToLower() %>) { 
                                if (!controllaCicliEliminati()) args.set_cancel(true);
                            }
                        }
                    }
                    else {
                        // controllo sull'eventuale presenza di cicli da eliminare
                        if (<%= (gestionePazientiDatiSanitari.dtaCicliEliminati.Rows.Count <> 0).ToString().ToLower() %>) {
                            if (!controllaCicliEliminati()) args.set_cancel(true);
                        }
                    }
                }
                break;
            }
        }

        function controllaCicliEliminati() {
            if (!confirm('Attenzione: confermando verranno eliminati i seguenti cicli <%= gestionePazientiDatiSanitari.CicliEliminatiStrJS %> \ne le relative programmazioni!')) {
                return false;
            }
            else {
                if (<%= gestionePazientiDatiSanitari.ControllaConvocazioneEliminaCicli.ToString.ToLower %>) {
			        if (!confirm('Attenzione: alcune convocazioni presenti nella programmazione da eliminare\rcontengono appuntamenti e/o solleciti associati.\rSi desidera proseguire?')) {
	                    return false;
	                }
	                else {
	                    // controlla se è stato eliminato il ciclo della mantoux e visualizza un messaggio per ricordarlo [modifica 15/12/2006]
	                    if('<%= gestionePazientiDatiSanitari.CicliEliminatiMessaggioStrJS %>'!="") {
		                    alert("Attenzione: hai eliminato i cicli <%= gestionePazientiDatiSanitari.CicliEliminatiMessaggioStrJS %> \nricordati di reinserirli dopo il salvataggio!");
				        }
                    }
                }
                else {
                    // controlla se è stato eliminato il ciclo della mantoux e visualizza un messaggio per ricordarlo [modifica 15/12/2006]
			        if('<%= gestionePazientiDatiSanitari.CicliEliminatiMessaggioStrJS %>'!="") {
                        alert("Attenzione: hai eliminato i cicli <%= gestionePazientiDatiSanitari.CicliEliminatiMessaggioStrJS %>! \nRicordati di reinserirli dopo il salvataggio.");
			        }
                }
            }
        
            return true;
        }

        // Refresh dopo la chiusura della popup di rilevazione del consenso
        function RefreshFromPopup() {
            __doPostBack("RefreshFromPopup", "");
        }

    </script>
</head>
<body>
    <script type="text/javascript" language="javascript">
        <%= HideLeftFrameIfNeeded() %>
    </script>
    <form id="Form1" method="post" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
        <on_lay3:OnitLayout3 ID="OnitLayout21" runat="server" Titolo="Gestione Dati Paziente" Busy="False" Height="100%" Width="100%" TitleCssClass="Title3">

            <div>
                <div id="LayoutTitolo" class="Title" runat="server">Dati anagrafici paziente</div>
                <telerik:RadToolBar ID="MainToolbar" runat="server" Width="100%" Skin="Default" EnableEmbeddedSkins="false" EnableAjaxSkinRendering="false" EnableEmbeddedBaseStylesheet="false" OnButtonClick="MainToolbar_ButtonClick" OnClientButtonClicking="OnClientButtonClicking">
                    <Items>
                        <telerik:RadToolBarButton runat="server" Text="Salva" Value="btnSalva" ImageUrl="~/Images/salva.gif" DisabledImageUrl="~/Images/salva_dis.gif" ToolTip="Salva le modifiche"></telerik:RadToolBarButton>
                        <telerik:RadToolBarButton runat="server" Text="Annulla" Value="btnAnnulla" ImageUrl="~/Images/annulla.gif" DisabledImageUrl="~/Images/annulla_dis.gif" ToolTip="Annulla le modifiche"></telerik:RadToolBarButton>
                        <telerik:RadToolBarButton runat="server" Text="Modifica" Value="btnModifica" ImageUrl="~/Images/Modifica.gif" DisabledImageUrl="~/Images/Modifica_dis.gif" ToolTip="Modifica i dati del paziente"></telerik:RadToolBarButton>
                        <telerik:RadToolBarButton runat="server" Text="Calendario&nbsp;Vaccinale" Value="btnCalVac" ImageUrl="~/Images/calendario.gif" DisabledImageUrl="~/Images/calendario_dis.gif" ToolTip=""></telerik:RadToolBarButton>
                        <telerik:RadToolBarButton runat="server" Text="Certif. Vaccinale" Value="btnCertificato" ImageUrl="~/Images/stampa.gif" DisabledImageUrl="~/Images/stampa_dis.gif" ToolTip=""></telerik:RadToolBarButton>
                        <telerik:RadToolBarDropDown Text="Altri certificati" ImageUrl="~/Images/stampa.gif">
                            <Buttons>
                                <telerik:RadToolBarButton runat="server" Text="Certif.&nbsp;Vaccinale&nbsp;Valido" Value="btnCertificatoVaccinaleValido" ToolTip=""></telerik:RadToolBarButton>
                                <telerik:RadToolBarButton runat="server" Text="Certif.&nbsp;Eseguite&nbsp;Scadute" Value="btnCertificatoEseguiteScadute" ToolTip=""></telerik:RadToolBarButton>
                                <telerik:RadToolBarButton runat="server" Text="Certif.&nbsp;Discrezionale" Value="btnCertificatoDiscrezionale" ToolTip=""></telerik:RadToolBarButton>
                                <telerik:RadToolBarButton runat="server" Text="Certif.&nbsp;Mantoux" Value="btnCertificatoMantoux" ToolTip=""></telerik:RadToolBarButton>
                                <telerik:RadToolBarButton runat="server" Text="Certif.&nbsp;solo&nbsp;Mantoux" Value="btnCertificatoSoloMantoux" ToolTip=""></telerik:RadToolBarButton>
                                <telerik:RadToolBarButton runat="server" Text="Certif.&nbsp;Vaccinale&nbsp;Lotti" Value="btnCertificatoVaccinaleLotti" ToolTip=""></telerik:RadToolBarButton>
                                <telerik:RadToolBarButton runat="server" Text="Certif.&nbsp;Frequenza" Value="btnCertificatoFrequenza" ToolTip="Certificato vaccinale con esito controlli"></telerik:RadToolBarButton>
                                <telerik:RadToolBarButton runat="server" Text="Certif.&nbsp;Vaccinale Covid" Value="btnCertificatoVaccinaleCovid" ToolTip="Certificato vaccinale con le sole vaccinazioni anti-covid19"></telerik:RadToolBarButton>
                            </Buttons>
                        </telerik:RadToolBarDropDown>
                        <telerik:RadToolBarButton runat="server" Text="Libret. Vaccinale" Value="btnLibrettoVaccinale" ImageUrl="~/Images/stampa.gif" DisabledImageUrl="~/Images/stampa_dis.gif" ToolTip=""></telerik:RadToolBarButton>
                    </Items>
                </telerik:RadToolBar>
            </div>

            <dyp:DynamicPanel ID="dypScroll" runat="server" Width="100%" Height="99%" ScrollBars="Auto" RememberScrollPosition="true">
                <ondp:onitdatapanel id="odpDettaglioPaziente" runat="server" fieldbindingmode="bindMixed" configfile="dettaglioPaziente.odpDettaglioPaziente.xml" renderonlychildren="True" dontloaddatafirsttime="True" preinsertfields="*.*.PAZ_COGNOME,*.*.PAZ_NOME,*.*.PAZ_SESSO,*.*.PAZ_DATA_NASCITA,*.*.PAZ_COM_CODICE_NASCITA,*.*.COM_DESCRIZIONE,*.*.PAZ_CODICE_FISCALE">
                            
                    <div id="secConsenso" runat="server">
                        <div class="vac-sezione" onclick="hideDettaglio('tableConsenso')" title="Click per mostrare/nascondere la sezione" style="cursor:pointer;">&nbsp;CONSENSO</div>
                        <table id="tableConsenso" cellpadding="0" cellspacing="0" style="padding:5px; margin-bottom:2px; background-color:#F8F8FF; display:block;" width="100%" >
                            <tr>
                                <td width="10%" class="label" style="text-align:right">
                                    <asp:Button runat="server" ID="btnConsenso" Text="Consensi" ToolTip="Apertura programma di rilevazione del consenso" style="cursor:pointer" />
                                </td>
                                <td width="10%" class="label" style="text-align:right">
                                    <asp:Button runat="server" ID="btnAutoRilevazioneConsensi" Text="Consenso Auto" ToolTip="Rilevazione automatica consensi" style="cursor:pointer" />
                                </td>
                                <td width="10%" class="label" style="padding:5px; text-align:right">
                                    <asp:Label ID="lblStatoConsensoPaz" runat="server">Stato:</asp:Label>
                                </td>
                                <td width="3%" style="width:20px; padding:5px; padding-left:5px; text-align:left;">
                                    <asp:Image id="imgStatoConsensoPaz" AlternateText="Consenso" runat="server" ToolTip="Stato del consenso del paziente" ImageUrl="~/Images/consensoAltro.png" />
                                </td>
                                <td width="67%" class="label_left" style="padding:5px; text-align:left;">
                                    <asp:Label ID="lblDescrStatoConsensoPaz" runat="server" Text="Non rilevato"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </div>

                    <div class="vac-sezione" >&nbsp;DATI ANAGRAFICI</div>
                    <div>
                        <table style="table-layout: fixed; margin-left: 2px" id="Table1" border="0" cellspacing="0" cellpadding="2" width="99%">
                            <tr height="10">
                                <td width="15%"></td>
                                <td width="35%"></td>
                                <td width="15%"></td>
                                <td width="35%"></td>
                            </tr>
                            <tr>
                                <td class="blocco_dati" colspan="4">
                                    <table border="0" cellspacing="0" cellpadding="2" width="100%">
                                        <tr height="10">
                                            <td width="15%"></td>
                                            <td width="35%"></td>
                                            <td width="15%"></td>
                                            <td width="35%"></td>
                                        </tr>
                                        <tr>
                                            <td colspan="4">
                                                <table border="0" cellspacing="0" cellpadding="2" width="100%">
                                                    <tr>
                                                        <td class="label" width="15%">
                                                            <asp:Label id="lblDataInserimento" runat="server">Data inserimento</asp:Label>
                                                        </td>
                                                        <td>
                                                            <ondp:wzOnitDatePick id="txtDataInserimento" runat="server" Width="130px" Height="20px" target="txtDataInserimento" CssStyles-CssDisabled="textbox_data_disabilitato" CssStyles-CssEnabled="textbox_data" CssStyles-CssRequired="textbox_data_obbligatorio" BindingField-Editable="never" BindingField-Connection="locale" BindingField-SourceTable="t_paz_pazienti" BindingField-Hidden="False" BindingField-SourceField="PAZ_DATA_INSERIMENTO" LabelAssociata="lblDataInserimento"></ondp:wzOnitDatePick>
                                                        </td>
                                                        <td class="label" width="15%">
                                                            <asp:Label id="lblDaCompletare" runat="server">Gestione Manuale</asp:Label>
                                                        </td>
                                                        <td>
                                                            <ondp:wzCheckBox id="chkDaCompletare" runat="server" Height="12px" CssStyles-CssDisabled="Label_Disabilitato" CssStyles-CssEnabled="Label" BindingField-Editable="always" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="False" BindingField-SourceField="PAZ_COMPLETARE" LabelAssociata="lblDaCompletare" BindingField-Value="N" ></ondp:wzCheckBox>
                                                        </td>
                                                        <td class="label" width="15%">
                                                            <asp:Label id="lblLocale" runat="server">Paziente locale</asp:Label>
                                                        </td>
                                                        <td>
                                                            <ondp:wzCheckBox id="chkLocale" runat="server" Height="12px" CssStyles-CssDisabled="Label_Disabilitato" CssStyles-CssEnabled="Label" BindingField-Editable="always" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="False" BindingField-SourceField="PAZ_LOCALE" LabelAssociata="lblLocale" BindingField-Value="N" StateChecked="S" StateUnChecked="N"></ondp:wzCheckBox>
                                                        </td>
                                                    </tr>
                                                    <tr id="rowCodiceAusiliario" runat="server">
                                                        <td class="label">
                                                            <asp:Label id="lblLivCertificazione" runat="server">Livello Certificazione</asp:Label>
                                                        </td>
                                                        <td>
                                                            <ondp:wzTextBox style="position: relative; width:100px" id="txtLivCertificazione" runat="server" CssStyles-CssDisabled="textbox_stringa_disabilitato" CssStyles-CssEnabled="textbox_stringa" CssStyles-CssRequired="textbox_stringa_obbligatorio" BindingField-Editable="never" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="False" BindingField-SourceField="PAZ_LIVELLO_CERTIFICAZIONE" LabelAssociata="lblLivCertificazione" MaxLength="2" Enabled="False"></ondp:wzTextBox>
                                                        </td>
                                                        <td class="label">
                                                            <asp:Label id="lblCodiceRegionale" runat="server">Codice regionale</asp:Label>
                                                        </td>
                                                        <td>
                                                            <ondp:wzTextBox id="txtCodiceRegionale" runat="server" CssStyles-CssDisabled="textbox_stringa_disabilitato" CssStyles-CssEnabled="textbox_stringa" CssStyles-CssRequired="textbox_stringa_obbligatorio w200px" BindingField-Editable="never" BindingField-Connection="centrale" BindingField-SourceTable="t_paz_pazienti_centrale" BindingField-Hidden="False" BindingField-SourceField="PAZ_CODICE_REGIONALE" LabelAssociata="lblCodiceRegionale" MaxLength="20"></ondp:wzTextBox>
                                                        </td>
                                                        <td class="label">
                                                            <asp:Label id="lblCodiceAusiliario" runat="server">Codice centrale</asp:Label>
                                                        </td>
                                                        <td>
                                                            <ondp:wzTextBox id="txtCodiceAusiliario" runat="server" CssStyles-CssDisabled="textbox_stringa_disabilitato w200px" CssStyles-CssEnabled="textbox_stringa w200px" CssStyles-CssRequired="textbox_stringa_obbligatorio w200px" BindingField-Editable="always" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="False" BindingField-SourceField="PAZ_CODICE_AUSILIARIO" LabelAssociata="lblCodiceAusiliario" MaxLength="20"></ondp:wzTextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="label">
                                                            <asp:Label id="lblDataAggiornamento" runat="server">Data aggiornamento</asp:Label>
                                                        </td>
                                                        <td>
                                                            <ondp:wzOnitDatePick id="txtDataAggiornamento" runat="server" Width="130px" Height="20px" target="txtDataAggiornamento" CssStyles-CssDisabled="textbox_data_disabilitato" CssStyles-CssEnabled="textbox_data" CssStyles-CssRequired="textbox_data_obbligatorio" BindingField-Editable="always" BindingField-Connection="locale" BindingField-SourceTable="t_paz_pazienti" BindingField-Hidden="False" BindingField-SourceField="PAZ_DATA_AGGIORNAMENTO" LabelAssociata="lblDataAggiornamento"></ondp:wzOnitDatePick>
                                                        </td>
                                                        <td class="label">
                                                            <asp:Label id="lblDataAggDaAnag" runat="server">Aggiornamento da anagrafe assistiti</asp:Label>
                                                        </td>
                                                        <td>
                                                            <ondp:wzOnitDatePick id="txtDataAggDaAnag" runat="server" Width="130px" Height="20px" target="txtDataAggDaAnag" CssStyles-CssDisabled="textbox_data_disabilitato" CssStyles-CssEnabled="textbox_data" CssStyles-CssRequired="textbox_data" BindingField-Editable="always" BindingField-Connection="locale" BindingField-SourceTable="t_paz_pazienti" BindingField-Hidden="False" BindingField-SourceField="PAZ_DATA_AGG_DA_ANAG" LabelAssociata="lblDataAggDaAnag"></ondp:wzOnitDatePick>
                                                        </td>
                                                        <td class="label">
                                                            <asp:Label id="lblDataAggDaComune" runat="server">Aggiornamento da comune</asp:Label>
                                                        </td>
                                                        <td>
                                                            <ondp:wzOnitDatePick id="txtDataAggDaComune" runat="server" Width="130px" Height="20px" target="txtDataAggDaComune" CssStyles-CssDisabled="textbox_data_disabilitato" CssStyles-CssEnabled="textbox_data" CssStyles-CssRequired="textbox_data" BindingField-Editable="always" BindingField-Connection="locale" BindingField-SourceTable="t_paz_pazienti" BindingField-Hidden="False" BindingField-SourceField="PAZ_DATA_AGG_DA_COMUNE" LabelAssociata="lblDataAggDaComune"></ondp:wzOnitDatePick>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr height="10">
                                            <td colspan="4"></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr height="10">
                                <td colspan="4"></td>
                            </tr>
                            <tr>
                                <td class="blocco_dati" colspan="4">
                                    <table border="0" cellspacing="0" cellpadding="2" width="100%">
                                        <tr height="1">
                                            <td width="15%"></td>
                                            <td width="35%"></td>
                                            <td width="15%"></td>
                                            <td width="35%"></td>
                                        </tr>
                                        <tr height="10">
                                            <td colspan="4"></td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblCognome" runat="server">Cognome</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzOnitJsValidator id="txtCognome" runat="server" Width="100%" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p" CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p" BindingField-Editable="always" BindingField-Connection="centrale" BindingField-SourceTable="t_paz_pazienti_centrale" BindingField-Hidden="False" BindingField-SourceField="PAZ_COGNOME" LabelAssociata="lblCognome" MaxLength="50" actionCorrect="False" actionDelete="False" actionFocus="True" actionMessage="True" actionSelect="False" actionUndo="True" autoFormat="True" validationType="Validate_custom" CustomValFunction-Length="17" CustomValFunction="validaCognomeNome" SetOnBlur="False" SetOnChange="True">
                                                    <Parameters>
                                                        <on_val:ValidationParam paramValue="true" paramOrder="0" paramType="boolean" paramName="blnUpper"></on_val:ValidationParam>
                                                    </Parameters>
                                                </ondp:wzOnitJsValidator>
                                            </td>
                                            <td class="label">
                                                <asp:Label id="lblNome" runat="server">Nome</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzOnitJsValidator id="txtNome" runat="server" Width="100%" CssStyles-CssDisabled="textbox_stringa_disabilitato" CssStyles-CssEnabled="textbox_stringa" CssStyles-CssRequired="textbox_stringa_obbligatorio" BindingField-Editable="always" BindingField-Connection="centrale" BindingField-SourceTable="t_paz_pazienti_centrale" BindingField-Hidden="False" BindingField-SourceField="PAZ_NOME" LabelAssociata="lblNome" MaxLength="50" actionCorrect="False" actionDelete="False" actionFocus="True" actionMessage="True" actionSelect="False" actionUndo="True" autoFormat="True" validationType="Validate_custom" CustomValFunction-Length="17" CustomValFunction="validaCognomeNome" SetOnBlur="False" SetOnChange="True">
                                                    <Parameters>
                                                        <on_val:ValidationParam paramValue="true" paramOrder="0" paramType="boolean" paramName="blnUpper"></on_val:ValidationParam>
                                                    </Parameters>
                                                </ondp:wzOnitJsValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblSesso" runat="server">Sesso</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzDropDownList id="cmbSesso" runat="server" Width="130px" Height="24px" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato" CssStyles-CssEnabled="TextBox_Stringa_Obbligatorio" CssStyles-CssRequired="TextBox_Stringa_Obbligatorio" BindingField-Editable="always" BindingField-Connection="centrale" BindingField-SourceTable="t_paz_pazienti_centrale" BindingField-Hidden="False" BindingField-SourceField="PAZ_SESSO" LabelAssociata="lblSesso">
                                                    <asp:ListItem Selected="True"></asp:ListItem>
                                                    <asp:ListItem Value="M">Maschio</asp:ListItem>
                                                    <asp:ListItem Value="F">Femmina</asp:ListItem>
                                                </ondp:wzDropDownList>
                                            </td>
                                            <td class="label">
                                                <asp:Label id="lblDataNascita" runat="server">Nato il</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzOnitDatePick id="txtDataNascita" runat="server" Width="130px" CssStyles-CssDisabled="TextBox_Data_Disabilitato" CssStyles-CssEnabled="TextBox_Data" CssStyles-CssRequired="TextBox_Data_Obbligatorio" BindingField-Editable="always" BindingField-Connection="centrale" BindingField-SourceTable="t_paz_pazienti_centrale" BindingField-Hidden="False" BindingField-SourceField="PAZ_DATA_NASCITA" LabelAssociata="lblDataNascita"></ondp:wzOnitDatePick>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblComuneDiNascita" runat="server">Comune di nascita</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzFinestraModale id="txtComuneDiNascita" runat="server" Width="70%" LabelAssociata="lblComuneDiNascita" 
                                                UseTableLayout="True" Label="Nato a" SetUpperCase="True" UseCode="True" CodiceWidth="30%" RaiseChangeEvent="True" CampoCodice="COM_CODICE Codice"
                                                    CampoDescrizione="COM_DESCRIZIONE Descrizione" Tabella="T_ANA_COMUNI" AltriCampi="COM_CATASTALE Catastale, COM_SCADENZA Scadenza" Obbligatorio="False"
                                                    PosizionamentoFacile="False" LabelWidth="-8px" DataTypeCode="Stringa" BindingCode-Editable="always" BindingCode-Connection="centrale" 
                                                    BindingCode-SourceTable="t_paz_pazienti_centrale" BindingCode-Hidden="False" BindingCode-SourceField="PAZ_COM_CODICE_NASCITA" BindingDescription-Editable="always"
                                                    BindingDescription-Hidden="False" DataTypeDescription="Stringa" BindingDescription-Connection="locale_cnas" BindingDescription-SourceTable="T_ANA_COMUNI" 
                                                    BindingDescription-SourceField="COM_DESCRIZIONE"></ondp:wzFinestraModale>

                                                <ondp:wzTextBox style="display: none" id="txtCatastaleNas" runat="server" BindingField-Editable="always" BindingField-Connection="locale_cnas" BindingField-SourceTable="T_ANA_COMUNI" BindingField-Hidden="False" BindingField-SourceField="COM_CATASTALE"></ondp:wzTextBox>
                                            </td>
                                            <td class="label">
                                                <asp:Label id="lblCittadinanza" runat="server">Cittadinanza</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzFinestraModale id="txtCittadinanza" runat="server" Width="70%" LabelAssociata="lblCittadinanza" UseTableLayout="True" SetUpperCase="True" UseCode="True" CodiceWidth="30%" RaiseChangeEvent="False" CampoCodice="CIT_CODICE Codice" CampoDescrizione="CIT_STATO Stato" Tabella="T_ANA_CITTADINANZE" AltriCampi="to_char(CIT_SCADENZA, 'dd/MM/yyyy') Scadenza" Obbligatorio="False" PosizionamentoFacile="False" LabelWidth="-8px" DataTypeCode="Stringa" BindingCode-Editable="always" BindingCode-Connection="centrale" BindingCode-SourceTable="t_paz_pazienti_centrale" BindingCode-Hidden="False" BindingCode-SourceField="PAZ_CIT_CODICE" BindingDescription-Editable="always" BindingDescription-Hidden="False" DataTypeDescription="Stringa" BindingDescription-Connection="locale_citt" BindingDescription-SourceTable="T_ANA_CITTADINANZE" BindingDescription-SourceField="CIT_STATO" Filtro="1=1 ORDER BY Stato, Scadenza desc"></ondp:wzFinestraModale>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblTesseraSanitaria" runat="server">Tessera sanitaria</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzOnitJsValidator id="txtTesseraSanitaria" runat="server" Width="100%" CssStyles-CssDisabled="textbox_stringa_Disabilitato w200px" CssStyles-CssEnabled="textbox_stringa w200px" CssStyles-CssRequired="textbox_stringa_Obbligatorio w200px" BindingField-Editable="always" BindingField-Connection="centrale" BindingField-SourceTable="t_paz_pazienti_centrale" BindingField-Hidden="False" BindingField-SourceField="PAZ_TESSERA" LabelAssociata="lblTesseraSanitaria" MaxLength="16" actionCorrect="False" actionDelete="False" actionFocus="False" actionMessage="False" actionSelect="False" actionUndo="False" autoFormat="False" validationType="none" SetOnBlur="False"></ondp:wzOnitJsValidator>
                                            </td>
                                            <td class="label">
                                                <asp:Label id="lblCodiceFiscale" runat="server">Codice fiscale</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzOnitJsValidator id="txtCodiceFiscale" runat="server" Width="100%" 
                                                    CssStyles-CssDisabled="textbox_stringa_disabilitato w200px" CssStyles-CssEnabled="textbox_stringa w200px" CssStyles-CssRequired="textbox_stringa_obbligatorio w200px" 
                                                    BindingField-Editable="always" BindingField-Connection="centrale" BindingField-SourceTable="t_paz_pazienti_centrale" 
                                                    BindingField-Hidden="False" BindingField-SourceField="PAZ_CODICE_FISCALE" 
                                                    LabelAssociata="lblCodiceFiscale" MaxLength="16" SetOnBlur="False" SetOnChange="True"
                                                    actionCorrect="False" actionDelete="False" actionFocus="True" actionMessage="False" 
                                                    actionSelect="False" actionUndo="True" autoFormat="True" validationType="Validate_custom" CustomValFunction="validaCodiceFiscale">
                                                    <Parameters>
                                                        <on_val:ValidationParam paramValue="eval(document.getElementById(\'txtNome\').value)" paramOrder="0" paramType="string" paramName="strNome"></on_val:ValidationParam>
                                                        <on_val:ValidationParam paramValue="eval(document.getElementById(\'txtCognome\').value)" paramOrder="1" paramType="string" paramName="strCognome"></on_val:ValidationParam>
                                                        <on_val:ValidationParam paramValue="eval(document.getElementById(\'cmbSesso\').value)" paramOrder="2" paramType="string" paramName="strSesso"></on_val:ValidationParam>
                                                        <on_val:ValidationParam paramValue="eval(OnitDataPickGet(\'txtDataNascita\'))" paramOrder="3" paramType="string" paramName="strDataNascita"></on_val:ValidationParam>
                                                        <on_val:ValidationParam paramValue="eval( getCodiceCatastale() )" paramOrder="4" paramType="string" paramName="strComune"></on_val:ValidationParam>
                                                        <on_val:ValidationParam paramValue="true" paramOrder="5" paramType="boolean" paramName="blnStepByStep"></on_val:ValidationParam>
                                                    </Parameters>
                                                </ondp:wzOnitJsValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblScadenzaSSN" runat="server">Scadenza SSN</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzOnitDatePick id="txtDataScadenzaSSN" runat="server" Width="130px" Height="20px" target="txtDataScadenzaSSN" CssStyles-CssDisabled="textbox_data_disabilitato" CssStyles-CssEnabled="textbox_data" CssStyles-CssRequired="textbox_data_obbligatorio" BindingField-Editable="always" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="False" BindingField-SourceField="PAZ_DATA_SCADENZA_SSN" LabelAssociata="lblScadenzaSSN"></ondp:wzOnitDatePick>
                                            </td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr height="10">
                                            <td colspan="4"></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr height="10">
                                <td colspan="4"></td>
                            </tr>
                            <tr>
                                <td class="blocco_dati" colspan="4">
                                    <table border="0" cellspacing="0" cellpadding="2" width="100%">
                                        <tr height="1">
                                            <td width="15%"></td>
                                            <td width="35%"></td>
                                            <td width="15%"></td>
                                            <td width="35%"></td>
                                        </tr>
                                        <tr height="10">
                                            <td colspan="4"></td>
                                        </tr>
                                        <tr>
                                            <td class="label" width="15%">
                                                <asp:Label id="lblViaResidenza" runat="server">Indirizzo residenza</asp:Label>
                                            </td>
                                            <td colspan="3">
                                                <uc1:GestioneVia id="viaResidenza" runat="server" LabelAssociata="lblViaResidenza"></uc1:GestioneVia>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblComuneResidenza" runat="server">Comune di residenza</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzFinestraModale id="txtComuneResidenza" runat="server" Width="70%" LabelAssociata="lblComuneResidenza" UseTableLayout="True" SetUpperCase="True" UseCode="True" CodiceWidth="30%" RaiseChangeEvent="True" CampoCodice="COM_CODICE Codice" CampoDescrizione="COM_DESCRIZIONE Descrizione" Tabella="T_ANA_COMUNI" Obbligatorio="True" PosizionamentoFacile="False" LabelWidth="-8px" DataTypeCode="Stringa" BindingCode-Editable="always" BindingCode-Connection="centrale" BindingCode-SourceTable="t_paz_pazienti_centrale" BindingCode-Hidden="False" BindingCode-SourceField="PAZ_COM_CODICE_RESIDENZA" BindingDescription-Editable="always" BindingDescription-Hidden="False" DataTypeDescription="Stringa" BindingDescription-Connection="locale_comre" BindingDescription-SourceTable="T_ANA_COMUNI" BindingDescription-SourceField="COM_DESCRIZIONE"></ondp:wzFinestraModale>
                                            </td>
                                            <td class="label">
                                                <asp:Label id="lblCapResidenza" runat="server">CAP</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzOnitJsValidator id="txtCapResidenza" runat="server" CssStyles-CssDisabled="textbox_stringa_disabilitato" CssStyles-CssEnabled="textbox_stringa" CssStyles-CssRequired="textbox_stringa_obbligatorio" BindingField-Editable="always" BindingField-Connection="centrale" BindingField-SourceTable="t_paz_pazienti_centrale" BindingField-Hidden="False" BindingField-SourceField="PAZ_CAP_RESIDENZA" LabelAssociata="lblCapResidenza" MaxLength="5" actionCorrect="False" actionDelete="False" actionFocus="True" actionMessage="True" actionSelect="False" actionUndo="True" autoFormat="True" validationType="Validate_custom" CustomValFunction-Length="9" CustomValFunction="validaCAP" SetOnBlur="False" SetOnChange="True" width="130px"></ondp:wzOnitJsValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblCircoscrizione" runat="server" Font-Bold="True">Circoscrizione</asp:Label>
                                            </td>
                                            <td colspan="3">
                                                <ondp:wzFinestraModale id="txtCircoscrizione" runat="server" Width="70%" LabelAssociata="lblCircoscrizione" UseTableLayout="True" SetUpperCase="True" UseCode="True" CodiceWidth="30%" RaiseChangeEvent="False" CampoCodice="CIR_CODICE Codice" CampoDescrizione="CIR_DESCRIZIONE Descrizione" Tabella="T_ANA_CIRCOSCRIZIONI" Obbligatorio="False" PosizionamentoFacile="False" LabelWidth="-8px" DataTypeCode="Stringa" BindingCode-Editable="always" BindingCode-Connection="centrale" BindingCode-SourceTable="t_paz_pazienti_centrale" BindingCode-Hidden="False" BindingCode-SourceField="PAZ_LOCALITA_RESIDENZA" BindingDescription-Editable="always" BindingDescription-Hidden="False" DataTypeDescription="Stringa" BindingDescription-Connection="locale_cir" BindingDescription-SourceTable="T_ANA_CIRCOSCRIZIONI" BindingDescription-SourceField="CIR_DESCRIZIONE" IsDistinct="False" UseAllResultCodeIfEqual="False" Paging="False" UseAllResultDescIfEqual="False" Sorting="False" LikeMode="Right" Filtro="1=1 ORDER BY Descrizione"></ondp:wzFinestraModale>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblDataInizioResidenza" runat="server">Data inizio residenza</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzOnitDatePick id="odpDataInizioResidenza" runat="server" Width="130px" Height="20px" CssStyles-CssDisabled="textbox_data_disabilitato" CssStyles-CssEnabled="textbox_data" CssStyles-CssRequired="textbox_data_obbligatorio" BindingField-Editable="always" BindingField-Connection="centrale" BindingField-SourceTable="t_paz_pazienti_centrale" BindingField-Hidden="False" BindingField-SourceField="PAZ_DATA_INIZIO_RESIDENZA" LabelAssociata="lblDataInizioResidenza"></ondp:wzOnitDatePick>
                                            </td>
                                            <td class="label">
                                                <asp:Label id="lblDataFineResidenza" runat="server">Data fine residenza</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzOnitDatePick id="odpDataFineResidenza" runat="server" Width="130px" Height="20px" CssStyles-CssDisabled="textbox_data_disabilitato" CssStyles-CssEnabled="textbox_data" CssStyles-CssRequired="textbox_data_obbligatorio" BindingField-Editable="always" BindingField-Connection="centrale" BindingField-SourceTable="t_paz_pazienti_centrale" BindingField-Hidden="False" BindingField-SourceField="PAZ_DATA_FINE_RESIDENZA" LabelAssociata="lblDataFineResidenza"></ondp:wzOnitDatePick>
                                            </td> 
                                        </tr>
                                        <tr height="10">
                                            <td colspan="4"></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr height="10">
                                <td colspan="4"></td>
                            </tr>
                            <tr>
                                <td class="blocco_dati" colspan="4">
                                    <table border="0" cellspacing="0" cellpadding="2" width="100%">
                                        <tr height="1">
                                            <td width="15%"></td>
                                            <td width="35%"></td>
                                            <td width="15%"></td>
                                            <td width="35%"></td>
                                        </tr>
                                        <tr height="10">
                                            <td colspan="4"></td>
                                        </tr>
                                        <tr>
                                            <td class="label" width="15%">
                                                <asp:Label id="lblViaDomicilio" runat="server">Indirizzo domicilio</asp:Label>
                                            </td>
                                            <td colspan="3">
                                                <uc1:GestioneVia id="viaDomicilio" runat="server"></uc1:GestioneVia>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblComuneDomicilio" runat="server">Comune di domicilio</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzFinestraModale id="txtComuneDomicilio" runat="server" Width="70%" LabelAssociata="lblComuneDomicilio" UseTableLayout="True" SetUpperCase="True" UseCode="True" CodiceWidth="30%" RaiseChangeEvent="True" CampoCodice="COM_CODICE Codice" CampoDescrizione="COM_DESCRIZIONE Descrizione" Tabella="T_ANA_COMUNI" Obbligatorio="False" PosizionamentoFacile="False" LabelWidth="-8px" DataTypeCode="Stringa" BindingCode-Editable="always" BindingCode-Connection="centrale" BindingCode-SourceTable="t_paz_pazienti_centrale" BindingCode-Hidden="False" BindingCode-SourceField="PAZ_COM_CODICE_DOMICILIO" BindingDescription-Editable="always" BindingDescription-Hidden="False" DataTypeDescription="Stringa" BindingDescription-Connection="locale_comdo" BindingDescription-SourceTable="T_ANA_COMUNI" BindingDescription-SourceField="COM_DESCRIZIONE"></ondp:wzFinestraModale>
                                            </td>
                                            <td class="label">
                                                <asp:Label id="lblCapDomicilio" runat="server">CAP</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzOnitJsValidator id="txtCapDomicilio" runat="server" CssStyles-CssDisabled="textbox_stringa_disabilitato" CssStyles-CssEnabled="textbox_stringa" CssStyles-CssRequired="textbox_stringa_obbligatorio" BindingField-Editable="always" BindingField-Connection="centrale" BindingField-SourceTable="t_paz_pazienti_centrale" BindingField-Hidden="False" BindingField-SourceField="PAZ_CAP_DOMICILIO" LabelAssociata="lblCapDomicilio" MaxLength="5" actionCorrect="False" actionDelete="False" actionFocus="True" actionMessage="True" actionSelect="False" actionUndo="True" autoFormat="True" validationType="Validate_custom" CustomValFunction-Length="9" CustomValFunction="validaCAP" SetOnBlur="False" SetOnChange="True" width="130px"></ondp:wzOnitJsValidator>
                                            </td>
                                        </tr>                                                    
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblCircoscrizione2" runat="server" Font-Bold="True">Circoscrizione</asp:Label>
                                            </td>
                                            <td colspan="3">
                                                <ondp:wzFinestraModale id="txtCircoscrizione2" runat="server" Width="70%" LabelAssociata="lblCircoscrizione2" UseTableLayout="True"  SetUpperCase="True" UseCode="True" CodiceWidth="30%" RaiseChangeEvent="False" CampoCodice="CIR_CODICE Codice"  CampoDescrizione="CIR_DESCRIZIONE Descrizione" Tabella="T_ANA_CIRCOSCRIZIONI" Obbligatorio="False"  PosizionamentoFacile="False" LabelWidth="-8px" DataTypeCode="Stringa" BindingCode-Editable="always"  BindingCode-Connection="centrale" BindingCode-SourceTable="t_paz_pazienti_centrale" BindingCode-Hidden="False"  BindingCode-SourceField="PAZ_LOCALITA_DOMICILIO" BindingDescription-Editable="always" BindingDescription-Hidden="False"  DataTypeDescription="Stringa" BindingDescription-Connection="locale_cir_2" BindingDescription-SourceTable="T_ANA_CIRCOSCRIZIONI"  BindingDescription-SourceField="CIR_DESCRIZIONE" IsDistinct="False" UseAllResultCodeIfEqual="False" Paging="False"  UseAllResultDescIfEqual="False" Sorting="False" LikeMode="Right" Filtro="1=1 ORDER BY Descrizione"></ondp:wzFinestraModale>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblDataInizioDomicilio" runat="server">Data inizio domicilio</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzOnitDatePick id="odpDataInizioDomicilio" runat="server" Width="130px" Height="20px" CssStyles-CssDisabled="textbox_data_disabilitato" CssStyles-CssEnabled="textbox_data" CssStyles-CssRequired="textbox_data_obbligatorio" BindingField-Editable="always" BindingField-Connection="centrale" BindingField-SourceTable="t_paz_pazienti_centrale" BindingField-Hidden="False" BindingField-SourceField="PAZ_DATA_INIZIO_DOMICILIO" LabelAssociata="lblDataInizioDomicilio"></ondp:wzOnitDatePick>
                                            </td>
                                            <td class="label">
                                                <asp:Label id="lblDataFineDomicilio" runat="server">Data fine domicilio</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzOnitDatePick id="odpDataFineDomicilio" runat="server" Width="130px" Height="20px" CssStyles-CssDisabled="textbox_data_disabilitato" CssStyles-CssEnabled="textbox_data" CssStyles-CssRequired="textbox_data_obbligatorio" BindingField-Editable="always" BindingField-Connection="centrale" BindingField-SourceTable="t_paz_pazienti_centrale" BindingField-Hidden="False" BindingField-SourceField="PAZ_DATA_FINE_DOMICILIO" LabelAssociata="lblDataFineDomicilio"></ondp:wzOnitDatePick>
                                            </td> 
                                        </tr>
                                        <tr height="10">
                                            <td colspan="4"></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr height="10">
                                <td colspan="4"></td>
                            </tr>
                            <tr>
                                <td class="blocco_dati" colspan="4">
                                    <table border="0" cellspacing="0" cellpadding="2" width="100%">
                                        <tr height="1">
                                            <td width="15%"></td>
                                            <td width="35%"></td>
                                            <td width="15%"></td>
                                            <td width="35%"></td>
                                        </tr>
                                        <tr height="10">
                                            <td colspan="4"></td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblTelefono1" runat="server" Text="<%$ Resources: Onit.OnAssistnet.OnVac.Web, DatiPaziente.Telefono1%>"></asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzTextBox id="txtTelefono1" runat="server" CssStyles-CssDisabled="textbox_stringa_disabilitato w200px" CssStyles-CssEnabled="textbox_stringa w200px" CssStyles-CssRequired="textbox_stringa_obbligatorio w200px" BindingField-Editable="always" BindingField-Connection="centrale" BindingField-SourceTable="t_paz_pazienti_centrale" BindingField-Hidden="False" BindingField-SourceField="PAZ_TELEFONO_1" LabelAssociata="lblTelefono1" MaxLength="20"></ondp:wzTextBox>
                                            </td>
                                            <td class="label">
                                                <asp:Label id="lblTelefono2" runat="server" Text="<%$ Resources: Onit.OnAssistnet.OnVac.Web, DatiPaziente.Telefono2%>"></asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzTextBox id="txtTelefono2" runat="server" CssStyles-CssDisabled="textbox_stringa_disabilitato w200px" CssStyles-CssEnabled="textbox_stringa w200px" CssStyles-CssRequired="textbox_stringa_obbligatorio w200px" BindingField-Editable="always" BindingField-Connection="centrale" BindingField-SourceTable="t_paz_pazienti_centrale" BindingField-Hidden="False" BindingField-SourceField="PAZ_TELEFONO_2" LabelAssociata="lblTelefono2" MaxLength="20"></ondp:wzTextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblTelefono3" runat="server" Text="<%$ Resources: Onit.OnAssistnet.OnVac.Web, DatiPaziente.Telefono3%>"></asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzTextBox id="txtTelefono3" runat="server" CssStyles-CssDisabled="textbox_stringa_disabilitato w200px" CssStyles-CssEnabled="textbox_stringa w200px" CssStyles-CssRequired="textbox_stringa_obbligatorio w200px" BindingField-Editable="always" BindingField-Connection="centrale" BindingField-SourceTable="t_paz_pazienti_centrale" BindingField-Hidden="False" BindingField-SourceField="PAZ_TELEFONO_3" LabelAssociata="lblTelefono3" MaxLength="20"></ondp:wzTextBox>
                                            </td>
                                            <td>&nbsp;</td>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblCategoriaCittadino" runat="server">Categoria cittadino</asp:Label>
                                            </td>
                                             <td>
                                                <ondp:wzTextBox id="txtCategoriaCittadino" runat="server" CssStyles-CssDisabled="textbox_stringa_disabilitato w200px" CssStyles-CssEnabled="textbox_stringa w200px" CssStyles-CssRequired="textbox_stringa_obbligatorio w200px" BindingField-Editable="always" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="true" BindingField-SourceField="PAZ_CATEGORIA_CITTADINO" LabelAssociata="lblCategoriaCittadino" MaxLength="5"></ondp:wzTextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblStatoAnagrafico" runat="server" Font-Bold="True">Stato Anagrafico</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzDropDownList id="cmbStatoAnagrafico" runat="server" Width="100%" CssStyles-CssDisabled="textbox_stringa_disabilitato" CssStyles-CssEnabled="textbox_stringa" CssStyles-CssRequired="textbox_stringa_obbligatorio" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="False" BindingField-SourceField="PAZ_STATO_ANAGRAFICO" LabelAssociata="lblStatoAnagrafico" BindingField-Value="1" SetOnChange="True" onChange="ControllaNuovoStato(this, CODICI_STATI_ANAG_CANC_PROG)" SourceConnection="locale"></ondp:wzDropDownList>
                                            </td>
                                            <td class="label">
                                                <asp:Label id="lblStatoAnagraficoDettagliato" runat="server" Font-Bold="True">Dettaglio stato anag.</asp:Label>
                                            </td>
                                            <td width="35%">
                                                <ondp:wzDropDownList id="cmbStatoAnagraficoDettagliato" runat="server" Width="100%" CssStyles-CssDisabled="textbox_stringa_disabilitato" CssStyles-CssEnabled="textbox_stringa" CssStyles-CssRequired="textbox_stringa_obbligatorio" BindingField-Editable="always" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="False" BindingField-SourceField="PAZ_STATO_ANAGRAFICO_DETT" LabelAssociata="lblStatoAnagraficoDettagliato" SourceConnection="locale"></ondp:wzDropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblPadre" runat="server">Padre</asp:Label>
                                            </td>
                                            <td colspan="3">
                                                <ondp:wzOnitJsValidator id="txtPadre" runat="server" Width="100%" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p" CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p" BindingField-Editable="always" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="False" BindingField-SourceField="PAZ_PADRE" LabelAssociata="lblPadre" MaxLength="51" actionCorrect="False" actionDelete="False" actionFocus="True" actionMessage="True" actionSelect="False" actionUndo="True" autoFormat="True" validationType="Validate_custom" CustomValFunction-Length="17" CustomValFunction="validaCognomeNome" SetOnBlur="False" SetOnChange="True">
                                                    <Parameters>
                                                        <on_val:ValidationParam paramValue="true" paramOrder="0" paramType="boolean" paramName="blnUpper"></on_val:ValidationParam>
                                                    </Parameters>
                                                </ondp:wzOnitJsValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblMadre" runat="server">Madre</asp:Label>
                                            </td>
                                            <td colspan="3">
                                                <ondp:wzOnitJsValidator id="txtMadre" runat="server" Width="100%" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p" CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p" BindingField-Editable="always" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="False" BindingField-SourceField="PAZ_MADRE" LabelAssociata="lblMadre" MaxLength="51" actionCorrect="False" actionDelete="False" actionFocus="True" actionMessage="True" actionSelect="False" actionUndo="True" autoFormat="True" validationType="Validate_custom" CustomValFunction-Length="17" CustomValFunction="validaCognomeNome" SetOnBlur="False" SetOnChange="True">
                                                    <Parameters>
                                                        <on_val:ValidationParam paramValue="true" paramOrder="0" paramType="boolean" paramName="blnUpper"></on_val:ValidationParam>
                                                    </Parameters>
                                                </ondp:wzOnitJsValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblDataDecesso" runat="server">Data di decesso</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzOnitDatePick id="txtDataDecesso" runat="server" Width="130px" target="txtDataDecesso" CssStyles-CssDisabled="TextBox_Data_disabilitato" CssStyles-CssEnabled="TextBox_Data" CssStyles-CssRequired="TextBox_Data_obbligatorio" BindingField-Editable="always" BindingField-Connection="centrale" BindingField-SourceTable="t_paz_pazienti_centrale" BindingField-Hidden="False" BindingField-SourceField="PAZ_DATA_DECESSO" LabelAssociata="lblDataDecesso" BorderColor="White"></ondp:wzOnitDatePick>
                                            </td>
                                            <td>&nbsp;</td>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblLuogoImmigrazione" runat="server" Text="<%$ Resources: Onit.OnAssistnet.OnVac.Web, DatiPaziente.LuogoImmigrazione%>"></asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzFinestraModale id="txtLuogoImmigrazione" runat="server" Width="70%" LabelAssociata="lblLuogoImmigrazione" UseTableLayout="True" SetUpperCase="True" UseCode="True" CodiceWidth="30%" RaiseChangeEvent="False" CampoCodice="COM_CODICE Codice" CampoDescrizione="COM_DESCRIZIONE Descrizione" Tabella="T_ANA_COMUNI" Obbligatorio="False" PosizionamentoFacile="False" LabelWidth="-8px" DataTypeCode="Stringa" BindingCode-Editable="always" BindingCode-Connection="centrale" BindingCode-SourceTable="t_paz_pazienti_centrale" BindingCode-Hidden="False" BindingCode-SourceField="PAZ_COM_CODICE_IMMIGRAZIONE" BindingDescription-Editable="always" BindingDescription-Hidden="False" DataTypeDescription="Stringa" BindingDescription-Connection="locale" BindingDescription-SourceTable="T_ANA_COM_PROVEN" BindingDescription-SourceField="COM_DESCRIZIONE"></ondp:wzFinestraModale>
                                            </td>
                                            <td class="label">
                                                <asp:Label id="lblDataImmigrazione" runat="server">Data</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzOnitDatePick id="txtDataImmigrazione" runat="server" Width="130px" CssStyles-CssDisabled="TextBox_Data_disabilitato" CssStyles-CssEnabled="TextBox_Data" CssStyles-CssRequired="TextBox_Data_obbligatorio" BindingField-Editable="always" BindingField-Connection="centrale" BindingField-SourceTable="t_paz_pazienti_centrale" BindingField-Hidden="False" BindingField-SourceField="PAZ_DATA_IMMIGRAZIONE" LabelAssociata="lblDataImmigrazione" BorderColor="White"></ondp:wzOnitDatePick>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblLuogoEmigrazione" runat="server" Text="<%$ Resources: Onit.OnAssistnet.OnVac.Web, DatiPaziente.LuogoEmigrazione%>"></asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzFinestraModale id="txtLuogoEmigrazione" runat="server" Width="70%" LabelAssociata="lblLuogoEmigrazione" UseTableLayout="True" SetUpperCase="True" UseCode="True" CodiceWidth="30%" RaiseChangeEvent="False" CampoCodice="COM_CODICE Codice" CampoDescrizione="COM_DESCRIZIONE Descrizione" Tabella="T_ANA_COMUNI" Obbligatorio="False" PosizionamentoFacile="False" LabelWidth="-8px" DataTypeCode="Stringa" BindingCode-Editable="always" BindingCode-Connection="centrale" BindingCode-SourceTable="t_paz_pazienti_centrale" BindingCode-Hidden="False" BindingCode-SourceField="PAZ_COM_CODICE_EMIGRAZIONE" BindingDescription-Editable="always" BindingDescription-Hidden="False" DataTypeDescription="Stringa" BindingDescription-Connection="locale" BindingDescription-SourceTable="T_ANA_COM_EMIGRA" BindingDescription-SourceField="COM_DESCRIZIONE"></ondp:wzFinestraModale>
                                            </td>
                                            <td class="label">
                                                <asp:Label id="lblDataEmigrazione" runat="server">Data</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzOnitDatePick id="txtDataEmigrazione" runat="server" Width="130px" CssStyles-CssDisabled="TextBox_Data_disabilitato" CssStyles-CssEnabled="TextBox_Data" CssStyles-CssRequired="TextBox_Data_obbligatorio" BindingField-Editable="always" BindingField-Connection="centrale" BindingField-SourceTable="t_paz_pazienti_centrale" BindingField-Hidden="False" BindingField-SourceField="PAZ_DATA_EMIGRAZIONE" LabelAssociata="lblDataEmigrazione" BorderColor="White"></ondp:wzOnitDatePick>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblAire" runat="server">Paziente AIRE</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzCheckBox id="chkAire" runat="server" Height="12px" CssStyles-CssDisabled="Label_Disabilitato" CssStyles-CssEnabled="Label" BindingField-Editable="always" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="False" BindingField-SourceField="PAZ_AIRE" LabelAssociata="lblAire" BindingField-Value="N" ></ondp:wzCheckBox>
                                            </td>
                                            <td class="label">
                                                <asp:Label id="lblDataAire" runat="server">Data AIRE</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzOnitDatePick id="txtDataAire" runat="server" Width="130px" CssStyles-CssDisabled="TextBox_Data_disabilitato" CssStyles-CssEnabled="TextBox_Data" CssStyles-CssRequired="TextBox_Data_obbligatorio" BindingField-Editable="always" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="False" BindingField-SourceField="PAZ_DATA_AIRE" LabelAssociata="lblDataAire" BorderColor="White"></ondp:wzOnitDatePick>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblIrreperibile" runat="server">Paziente irreperibile</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzCheckBox id="chkIrreperibile" runat="server" Height="12px" CssStyles-CssDisabled="Label_Disabilitato" CssStyles-CssEnabled="Label" BindingField-Editable="always" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="False" BindingField-SourceField="PAZ_IRREPERIBILE" LabelAssociata="lblIrreperibile" BindingField-Value="N" AutoPostBack="true"></ondp:wzCheckBox>
                                            </td>
                                            <td class="label">
                                                <asp:Label id="lblDataIrreperibilta" runat="server">Data irreperibilità</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzOnitDatePick id="txtDataIrreperibilta" runat="server" Width="130px" CssStyles-CssDisabled="TextBox_Data_disabilitato" CssStyles-CssEnabled="TextBox_Data" CssStyles-CssRequired="TextBox_Data_obbligatorio" BindingField-Editable="always" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="False" BindingField-SourceField="PAZ_DATA_IRREPERIBILITA" LabelAssociata="lblDataIrreperibilta" BorderColor="White"></ondp:wzOnitDatePick>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblCancellato" runat="server">Paziente cancellato</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzCheckBox id="chkCancellato" runat="server" Height="12px" CssStyles-CssDisabled="Label_Disabilitato" CssStyles-CssEnabled="Label" BindingField-Editable="always" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="False" BindingField-SourceField="PAZ_CANCELLATO" LabelAssociata="lblCancellato" BindingField-Value="N" AutoPostBack="true"></ondp:wzCheckBox>
                                            </td>
                                            <td class="label">
                                                <asp:Label id="lblDataCancellazione" runat="server">Data cancellazione</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzOnitDatePick id="txtDataCancellazione" runat="server" Width="130px" CssStyles-CssDisabled="TextBox_Data_disabilitato" CssStyles-CssEnabled="TextBox_Data" CssStyles-CssRequired="TextBox_Data_obbligatorio" BindingField-Editable="always" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="False" BindingField-SourceField="PAZ_DATA_CANCELLAZIONE" LabelAssociata="lblDataCancellazione" BorderColor="White"></ondp:wzOnitDatePick>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblOccasionale" runat="server">Paziente occasionale</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzCheckBox id="chkOccasionale" runat="server" Height="12px" CssStyles-CssDisabled="Label_Disabilitato" CssStyles-CssEnabled="Label" BindingField-Editable="always" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="False" BindingField-SourceField="PAZ_OCCASIONALE" LabelAssociata="lblOccasionale" BindingField-Value="N" AutoPostBack="true"></ondp:wzCheckBox>
                                            </td>
                                            <td class="label">
                                                <asp:Label id="lblTipoOccasionalita" runat="server">Tipo occasionalità</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzDropDownList id="cmbTipoOccasionalita" runat="server" Width="100%" CssStyles-CssDisabled="TextBox_Stringa_Disabilitato" CssStyles-CssEnabled="TextBox_Stringa" CssStyles-CssRequired="TextBox_Stringa" BindingField-Editable="always" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="False" BindingField-SourceField="PAZ_TIPO_OCCASIONALITA" LabelAssociata="lblTipoOccasionalita" BindingField-Value="1" onChange="ControllaNuovoStato(this, CODICI_STATI_ANAG_CANC_PROG)" SourceConnection="locale"></ondp:wzDropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4">&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td colspan="4">
                                                <table border="0" cellspacing="0" cellpadding="2" width="100%">
                                                    <tr>
                                                        <td class="label" width="15%">
                                                            <asp:Label id="lblCartellaInviata" runat="server">Richiesto Certificato</asp:Label>
                                                        </td>
                                                        <td>
                                                            <ondp:wzCheckBox id="chkCartellaInviata" runat="server" Height="12px" CssStyles-CssDisabled="Label_Disabilitato" CssStyles-CssEnabled="Label" BindingField-Editable="always" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="False" BindingField-SourceField="PAZ_RICHIESTA_CERTIFICATO" LabelAssociata="lblCartellaInviata" BindingField-Value="N" ></ondp:wzCheckBox>
                                                        </td>
                                                        <td class="label">
                                                            <asp:Label id="lblRegolarizzato" runat="server">Paziente regolarizzato</asp:Label>
                                                        </td>
                                                        <td>
                                                            <ondp:wzCheckBox id="chkRegolarizzato" runat="server" Height="12px" CssStyles-CssDisabled="Label_Disabilitato" CssStyles-CssEnabled="Label" BindingField-Editable="always" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="False" BindingField-SourceField="PAZ_REGOLARIZZATO" LabelAssociata="lblRegolarizzato" BindingField-Value="N" ></ondp:wzCheckBox>
                                                        </td>
                                                        <td class="label">
                                                            <asp:Label id="lblStaCertificatoEmi" runat="server">Stampato certificato emigrazione</asp:Label>
                                                        </td>
                                                        <td>
                                                            <ondp:wzCheckBox id="chkStaCertificatoEmi" runat="server" Height="12px" CssStyles-CssDisabled="Label_Disabilitato" CssStyles-CssEnabled="Label" BindingField-Editable="always" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="False" BindingField-SourceField="PAZ_STA_CERTIFICATO_EMI" LabelAssociata="lblStaCertificatoEmi" BindingField-Value="N" ></ondp:wzCheckBox>
                                                        </td>
                                                        <td class="label">
                                                            <asp:Label id="lblPosVaccinaleOk" runat="server">Posizione vaccinale ok</asp:Label>
                                                        </td>
                                                        <td>
                                                            <ondp:wzCheckBox id="chkPosVaccinaleOk" runat="server" Height="12px" CssStyles-CssDisabled="Label_Disabilitato" CssStyles-CssEnabled="Label" BindingField-Editable="always" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="False" BindingField-SourceField="PAZ_POSIZIONE_VACCINALE_OK" LabelAssociata="lblPosVaccinaleOk" BindingField-Value="N" ></ondp:wzCheckBox>
                                                        </td>
                                                    </tr>
                                                    <tr height="10">
                                                        <td colspan="4"></td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr height="10">
                                            <td colspan="4"></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr height="30">
                                <td colspan="4"></td>
                            </tr>
                        </table>
                    </div>
                                
                    <div class="vac-sezione">&nbsp;DATI SANITARI</div>
                    <div>
                        <table style="table-layout: fixed; margin-left: 2px" id="Table2" border="0" cellspacing="0" cellpadding="2" width="99%">
                            <tr height="10">
                                <td width="15%"></td>
                                <td width="85%"></td>
                            </tr>
                            <tr>
                                <td class="blocco_dati" colspan="2">
                                    <table border="0" cellspacing="0" cellpadding="2" width="100%">
                                        <colgroup>
                                            <col style="width:15%" />
                                            <col style="width:85%" />
                                        </colgroup>
                                        <tr height="10">
                                            <td colspan="2"></td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <table style="width:100%">
                                                    <td class="label" style="width:15%">
                                                        <asp:Label id="lblStatusVaccinale" runat="server">Status vaccinale</asp:Label>
                                                    </td>
                                                    <td style="width:85%">
                                                        <ondp:wzDropDownList id="cmbStatusVaccinale" runat="server" Width="50%" CssStyles-CssDisabled="textbox_stringa_disabilitato" CssStyles-CssEnabled="textbox_stringa" CssStyles-CssRequired="textbox_stringa_obbligatorio" BindingField-Editable="never" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="False" BindingField-SourceField="PAZ_STATO" LabelAssociata="lblStatusVaccinale" BindingField-Value="1" Enabled="False">
                                                            <asp:ListItem Value="3" Selected="True">IN CORSO</asp:ListItem>
                                                            <asp:ListItem Value="4">TERMINATO</asp:ListItem>
                                                            <asp:ListItem Value="9">INADEMPIENTE TOTALE</asp:ListItem>
                                                        </ondp:wzDropDownList>
                                                    </td>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label" colspan="2">
                                                <uc1:GestionePazientiDatiSanitari id="gestionePazientiDatiSanitari" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr height="10">
                                <td colspan="2"></td>
                            </tr>
                            <tr>
                                <td class="blocco_dati" colspan="2">
                                    <table border="0" cellspacing="0" cellpadding="2" width="100%">
                                        <tr height="10">
                                            <td width="15%"></td>
                                            <td width="85%"></td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <table border="0" cellspacing="0" cellpadding="2" width="100%">
                                                    <tr height=30>
                                                        <td class="label">
                                                            <asp:Label id="lblSedeTerritoriale" runat="server">Centro Vacc. territoriale</asp:Label>
                                                        </td>
                                                        <td>
                                                            <ondp:wzFinestraModale id="txtSedeTerritoriale" runat="server" Width="70%" LabelAssociata="lblSedeTerritoriale" UseTableLayout="True" SetUpperCase="True" UseCode="True" CodiceWidth="30%" CampoCodice="CNS_CODICE Codice" CampoDescrizione="CNS_DESCRIZIONE Descrizione" Tabella="T_ANA_CONSULTORI" Obbligatorio="False" PosizionamentoFacile="False" LabelWidth="-8px" DataTypeCode="Stringa" BindingCode-Editable="always" BindingCode-Connection="locale" BindingCode-SourceTable="T_PAZ_PAZIENTI" BindingCode-Hidden="False" BindingCode-SourceField="PAZ_CNS_TERR_CODICE" BindingDescription-Editable="always" BindingDescription-Hidden="False" DataTypeDescription="Stringa" BindingDescription-Connection="locale" BindingDescription-SourceTable="T_ANA_CONS_TERR" BindingDescription-SourceField="CNS_DESCRIZIONE" Filtro="cns_data_apertura <= SYSDATE AND (cns_data_chiusura >SYSDATE OR cns_data_chiusura IS NULL) ORDER BY Descrizione"></ondp:wzFinestraModale>
                                                        </td>
                                                        <td></td>
                                                        <td></td>
                                                    </tr>
                                                    <tr height="30">
                                                        <td class="label" width="15%">
                                                            <asp:Label id="lblSedeVaccinale" runat="server" Font-Bold="True">Centro vaccinale</asp:Label>
                                                        </td>
                                                        <td width="50%">
                                                            <ondp:wzFinestraModale id="txtSedeVaccinale" runat="server" Width="70%" LabelAssociata="lblSedeVaccinale" UseTableLayout="True" SetUpperCase="True" UseCode="True" CodiceWidth="30%" RaiseChangeEvent="True" CampoCodice="CNS_CODICE Codice" CampoDescrizione="CNS_DESCRIZIONE Descrizione" Tabella="T_ANA_CONSULTORI" AltriCampi="CNS_INDIRIZZO Indirizzo" Obbligatorio="True" PosizionamentoFacile="False" LabelWidth="-8px" DataTypeCode="Stringa" BindingCode-Editable="always" BindingCode-Connection="locale" BindingCode-SourceTable="T_PAZ_PAZIENTI" BindingCode-Hidden="False" BindingCode-SourceField="PAZ_CNS_CODICE" BindingDescription-Editable="always" BindingDescription-Hidden="False" DataTypeDescription="Stringa" BindingDescription-Connection="locale" BindingDescription-SourceTable="T_ANA_CONSULTORI" BindingDescription-SourceField="CNS_DESCRIZIONE" Filtro="cns_data_apertura <= SYSDATE AND (cns_data_chiusura >SYSDATE OR cns_data_chiusura IS NULL) ORDER BY Descrizione" CustomProcBeforeSend="ValorizzaIndirizzo"></ondp:wzFinestraModale>
                                                        </td>
                                                        <td class="label" width="15%">
                                                            <asp:Label id="lblDataAssegnazione" runat="server">Data assegnazione</asp:Label>
                                                        </td>
                                                        <td width="20%">
                                                            <ondp:wzOnitDatePick id="txtDataAssegnazione" runat="server" Width="130px" CssStyles-CssDisabled="TextBox_Data_disabilitato" CssStyles-CssEnabled="TextBox_Data" CssStyles-CssRequired="TextBox_Data_obbligatorio" BindingField-Editable="always" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="False" BindingField-SourceField="PAZ_CNS_DATA_ASSEGNAZIONE" LabelAssociata="lblDataAssegnazione" BorderColor="White"></ondp:wzOnitDatePick>
                                                        </td>
                                                    </tr>
                                                    <tr height="30">
                                                        <td class="label">
                                                            <asp:Label id="lblIndirizzoSedeVaccinale" runat="server">Indirizzo</asp:Label>
                                                        </td>
                                                        <td>
                                                            <ondp:wzTextBox id="txtIndirizzoSedeVaccinale" runat="server" Width="100%" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p" CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p" BindingField-Editable="never" BindingField-Connection="locale" BindingField-SourceTable="T_ANA_CONSULTORI" BindingField-Hidden="False" BindingField-SourceField="CNS_INDIRIZZO" LabelAssociata="lblIndirizzoSedeVaccinale" MaxLength="50" ReadOnly="True"></ondp:wzTextBox>
                                                        </td>
                                                        <td>&nbsp;</td>
                                                        <td>&nbsp;</td>
                                                    </tr>
                                                    <tr height="40" valign="middle">
                                                        <td class="label" width="15%">
                                                            <asp:Label id="lblPreferenza" runat="server">Preferenza</asp:Label>
                                                        </td>
                                                        <td align="center" colspan="3">
                                                            <ondp:wzTextBox id="txtGiorno" runat="server" Width="100%" CssStyles-CssDisabled="textbox_stringa_disabilitato w100p" CssStyles-CssEnabled="textbox_stringa w100p" CssStyles-CssRequired="textbox_stringa_obbligatorio w100p" BindingField-Editable="never" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="False" BindingField-SourceField="PAZ_GIORNO"  MaxLength="50" ReadOnly="True" Visible="false"></ondp:wzTextBox>
                                                            <div class="label_left" style="width: 100%; text-align: left; vertical-align: middle;">
                                                                <asp:CheckBox id="chkLunedi" runat="server" Width="13%" CssClass="label" Text="Lunedi" TextAlign="Left"></asp:CheckBox>
                                                                <asp:CheckBox id="chkMartedi" runat="server" Width="13%" CssClass="label" Text="Martedi" TextAlign="Left"></asp:CheckBox>
                                                                <asp:CheckBox id="chkMercoledi" runat="server" Width="13%" CssClass="label" Text="Mercoledi" TextAlign="Left"></asp:CheckBox>
                                                                <asp:CheckBox id="chkGiovedi" runat="server" Width="13%" CssClass="label" Text="Giovedi" TextAlign="Left"></asp:CheckBox>
                                                                <asp:CheckBox id="chkVenerdi" runat="server" Width="13%" CssClass="label" Text="Venerdi" TextAlign="Left"></asp:CheckBox>
                                                                <asp:CheckBox id="chkSabato" runat="server" Width="13%" CssClass="label" Text="Sabato" TextAlign="Left"></asp:CheckBox>
                                                                <asp:CheckBox id="chkDomenica" runat="server" Width="13%" CssClass="label" Text="Domenica" TextAlign="Left"></asp:CheckBox>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr height="30">
                                                        <td class="label">
                                                            <asp:Label id="lblSedeVaccinalePrec" runat="server">Centro vaccinale precedente</asp:Label>
                                                        </td>
                                                        <td>
                                                            <ondp:wzFinestraModale id="txtSedeVaccinalePrec" runat="server" Width="70%" LabelAssociata="lblSedeVaccinalePrec" UseTableLayout="True" SetUpperCase="True" UseCode="True" CodiceWidth="30%" CampoCodice="CNS_CODICE Codice" CampoDescrizione="CNS_DESCRIZIONE Descrizione" Tabella="T_ANA_CONSULTORI" Obbligatorio="False" PosizionamentoFacile="False" LabelWidth="-8px" DataTypeCode="Stringa" BindingCode-Editable="always" BindingCode-Connection="locale" BindingCode-SourceTable="T_PAZ_PAZIENTI" BindingCode-Hidden="False" BindingCode-SourceField="PAZ_CNS_CODICE_OLD" BindingDescription-Editable="always" BindingDescription-Hidden="False" DataTypeDescription="Stringa" BindingDescription-Connection="locale" BindingDescription-SourceTable="T_ANA_CONS_OLD" BindingDescription-SourceField="CNS_DESCRIZIONE" Filtro="1=1 ORDER BY Descrizione"></ondp:wzFinestraModale>
                                                        </td>
                                                        <td>&nbsp;</td>
                                                        <td>&nbsp;</td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr height="10">
                                <td colspan="2"></td>
                            </tr>
                            <tr>
                                <td class="blocco_dati" colspan="2">
                                    <table border="0" cellspacing="0" cellpadding="2" width="100%">
                                        <tr height="10">
                                            <td style="width: 15%"></td>
                                            <td style="width: 15%"></td>
                                            <td style="width: 20%"></td>
                                            <td style="width: 15%"></td>
                                            <td style="width: 20%"></td>
                                            <td style="width: 5%"></td>
                                            <td style="width: 10%"></td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblUSLDiResidenza" runat="server">Usl di Residenza</asp:Label>
                                            </td>
                                            <td colspan="6">
                                                <ondp:wzFinestraModale id="txtUSLDiResidenza" runat="server" Width="70%" LabelAssociata="lblUSLDiResidenza" UseTableLayout="True" SetUpperCase="True" UseCode="True" CodiceWidth="30%" RaiseChangeEvent="False" CampoCodice="USL_CODICE Codice" CampoDescrizione="USL_DESCRIZIONE Descrizione" Tabella="T_ANA_USL" Obbligatorio="False" PosizionamentoFacile="False" LabelWidth="-8px" DataTypeCode="Stringa" BindingCode-Editable="always" BindingCode-Connection="centrale" BindingCode-SourceTable="t_paz_pazienti_centrale" BindingCode-Hidden="False" BindingCode-SourceField="PAZ_USL_CODICE_RESIDENZA" BindingDescription-Editable="always" BindingDescription-Hidden="False" DataTypeDescription="Stringa" BindingDescription-Connection="locale_ausl" BindingDescription-SourceTable="T_ANA_USL" BindingDescription-SourceField="USL_DESCRIZIONE" Filtro=" USL_SCADENZA IS NULL ORDER BY Descrizione"></ondp:wzFinestraModale>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblMedicoDiBase" runat="server">Medico di base</asp:Label>
                                            </td>
                                            <td colspan="6">
                                                <ondp:wzFinestraModale id="txtMedicoDiBase" runat="server" Width="70%" LabelAssociata="lblMedicoDiBase" UseTableLayout="True" SetUpperCase="True" UseCode="True" CodiceWidth="30%" RaiseChangeEvent="False" CampoCodice="MED_CODICE Codice" CampoDescrizione="MED_DESCRIZIONE Descrizione" Tabella="T_ANA_MEDICI" Obbligatorio="False" PosizionamentoFacile="False" LabelWidth="-8px" DataTypeCode="Stringa" BindingCode-Editable="always" BindingCode-Connection="centrale" BindingCode-SourceTable="t_paz_pazienti_centrale" BindingCode-Hidden="False" BindingCode-SourceField="PAZ_MED_CODICE_BASE" BindingDescription-Editable="always" BindingDescription-Hidden="False" DataTypeDescription="Stringa" BindingDescription-Connection="locale_med" BindingDescription-SourceTable="t_ana_medici" BindingDescription-SourceField="MED_DESCRIZIONE" Filtro="1=1 ORDER BY Descrizione"></ondp:wzFinestraModale>
                                            </td>
                                        </tr>
                                        <tr id="rowMedicoDiBaseDettagli">
                                            <td class="label">
                                                <asp:Label id="lblMedicoDiBaseDecorrenza" runat="server">Decorrenza Medico</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzOnitDatePick id="txtMedicoDiBaseDecorrenza" runat="server" Width="130px" Height="20px" target="txtMedicoDiBaseDecorrenza" CssStyles-CssDisabled="textbox_data_disabilitato" CssStyles-CssEnabled="textbox_data" CssStyles-CssRequired="textbox_data_obbligatorio" BindingField-Editable="always" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="False" BindingField-SourceField="PAZ_DATA_DECORRENZA_MED" LabelAssociata="lblMedicoDiBaseDecorrenza"></ondp:wzOnitDatePick>
                                            </td>
                                            <td class="label">
                                                <asp:Label id="lblMedicoDiBaseScadenza" runat="server">Scadenza Medico</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzOnitDatePick id="txtMedicoDiBaseScadenza" runat="server" Width="130px" Height="20px" target="txtMedicoDiBaseScadenza" CssStyles-CssDisabled="textbox_data_disabilitato" CssStyles-CssEnabled="textbox_data" CssStyles-CssRequired="textbox_data_obbligatorio" BindingField-Editable="always" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="False" BindingField-SourceField="PAZ_DATA_SCADENZA_MED" LabelAssociata="lblMedicoDiBaseScadenza"></ondp:wzOnitDatePick>
                                            </td>
                                            <td class="label">
                                                <asp:Label id="lblMedicoDiBaseRevoca" runat="server">Revoca Medico</asp:Label>
                                            </td>
                                            <td colspan="2">
                                                <ondp:wzOnitDatePick id="txtMedicoDiBaseRevoca" runat="server" Width="130px" Height="20px" target="txtMedicoDiBaseRevoca" CssStyles-CssDisabled="textbox_data_disabilitato" CssStyles-CssEnabled="textbox_data" CssStyles-CssRequired="textbox_data_obbligatorio" BindingField-Editable="always" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="False" BindingField-SourceField="PAZ_DATA_REVOCA_MED" LabelAssociata="lblMedicoDiBaseRevoca"></ondp:wzOnitDatePick>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblUSLDiAssistenza" runat="server">Usl di Assistenza</asp:Label>
                                            </td>
                                            <td colspan="6">
                                                <ondp:wzFinestraModale id="txtUSLDiAssistenza" runat="server" Width="70%" LabelAssociata="lblUSLDiAssistenza" UseTableLayout="True" SetUpperCase="True" UseCode="True" CodiceWidth="30%" RaiseChangeEvent="False" CampoCodice="USL_CODICE Codice" CampoDescrizione="USL_DESCRIZIONE Descrizione" Tabella="T_ANA_USL" Obbligatorio="False" PosizionamentoFacile="False" LabelWidth="-8px" DataTypeCode="Stringa" BindingCode-Editable="always" BindingCode-Connection="centrale" BindingCode-SourceTable="t_paz_pazienti_centrale" BindingCode-Hidden="False" BindingCode-SourceField="PAZ_USL_CODICE_ASSISTENZA" BindingDescription-Editable="always" BindingDescription-Hidden="False" DataTypeDescription="Stringa" BindingDescription-Connection="locale_ausl_ass" BindingDescription-SourceTable="T_ANA_USL" BindingDescription-SourceField="USL_DESCRIZIONE" Filtro=" USL_SCADENZA IS NULL ORDER BY Descrizione"></ondp:wzFinestraModale>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                    <asp:Label id="lblDataInizioAssistenza" runat="server">Inizio assistenza</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzOnitDatePick id="odpDataInizioAssistenza" runat="server" Width="130px" Height="20px" 
                                                    CssStyles-CssDisabled="textbox_data_disabilitato" CssStyles-CssEnabled="textbox_data" CssStyles-CssRequired="textbox_data_obbligatorio" 
                                                    BindingField-Editable="always" BindingField-Connection="centrale" BindingField-SourceTable="t_paz_pazienti_centrale" BindingField-Hidden="False" BindingField-SourceField="PAZ_DATA_INIZIO_ASS" 
                                                    LabelAssociata="lblDataInizioAssistenza">
                                                </ondp:wzOnitDatePick>
                                            </td>
                                            <td class="label">
                                                <asp:Label id="lblDataCessazioneAssistenza" runat="server">Fine assistenza</asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzOnitDatePick id="odpDataCessazioneAssistenza" runat="server" Width="130px" Height="20px" 
                                                    CssStyles-CssDisabled="textbox_data_disabilitato" CssStyles-CssEnabled="textbox_data" CssStyles-CssRequired="textbox_data_obbligatorio" 
                                                    BindingField-Editable="always" BindingField-Connection="centrale" BindingField-SourceTable="t_paz_pazienti_centrale" BindingField-Hidden="False" BindingField-SourceField="PAZ_DATA_CESSAZIONE_ASS" 
                                                    LabelAssociata="lblDataCessazioneAssistenza">
                                                </ondp:wzOnitDatePick>
                                            </td>
                                            <td class="label" colspan="2">
                                                <asp:Label id="lblFlagCessato" runat="server">Cessato</asp:Label>
                                            </td> 
                                            <td>
                                                <ondp:wzCheckBox id="chkFlagCessato" runat="server" Height="12px" CssStyles-CssDisabled="Label_Disabilitato" CssStyles-CssEnabled="Label" BindingField-Editable="always" BindingField-Connection="locale" BindingField-SourceTable="T_PAZ_PAZIENTI" BindingField-Hidden="False" BindingField-SourceField="PAZ_FLAG_CESSATO" LabelAssociata="lblFlagCessato" BindingField-Value="N" ></ondp:wzCheckBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblComuneDomicilioSanitario" runat="server">Comune di domicilio sanitario</asp:Label>
                                            </td>
                                            <td colspan="6">
                                                <ondp:wzFinestraModale id="txtComuneDomicilioSanitario" runat="server" Width="70%" LabelAssociata="lblComuneDomicilioSanitario" UseTableLayout="True" SetUpperCase="True" UseCode="True" CodiceWidth="30%" RaiseChangeEvent="False" CampoCodice="COM_CODICE Codice" CampoDescrizione="COM_DESCRIZIONE Descrizione" Tabella="T_ANA_COMUNI" Obbligatorio="False" PosizionamentoFacile="False" LabelWidth="-8px" DataTypeCode="Stringa" BindingCode-Editable="always" BindingCode-Connection="centrale" BindingCode-SourceTable="t_paz_pazienti_centrale" BindingCode-Hidden="False" BindingCode-SourceField="PAZ_COM_CODICE_DOMICILIO_SAN" BindingDescription-Editable="always" BindingDescription-Hidden="False" DataTypeDescription="Stringa" BindingDescription-Connection="locale_comdosan" BindingDescription-SourceTable="T_ANA_COMUNI" BindingDescription-SourceField="COM_DESCRIZIONE"></ondp:wzFinestraModale>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblUSLDiAssistenzaPrecedente" runat="server">Usl di Assistenza Precedente</asp:Label>
                                            </td>
                                            <td colspan="6">
                                                <ondp:wzFinestraModale id="txtUSLDiAssistenzaPrecedente" runat="server" Width="70%" LabelAssociata="lblUSLDiAssistenzaPrecedente" UseTableLayout="True" SetUpperCase="True" UseCode="True" CodiceWidth="30%" RaiseChangeEvent="False" CampoCodice="USL_CODICE Codice" CampoDescrizione="USL_DESCRIZIONE Descrizione" Tabella="T_ANA_USL" Obbligatorio="False" PosizionamentoFacile="False" LabelWidth="-8px" DataTypeCode="Stringa" BindingCode-Editable="always" BindingCode-Connection="centrale" BindingCode-SourceTable="t_paz_pazienti_centrale" BindingCode-Hidden="False" BindingCode-SourceField="PAZ_USL_PROVENIENZA" BindingDescription-Editable="always" BindingDescription-Hidden="False" DataTypeDescription="Stringa" BindingDescription-Connection="locale_ausl_ass_precedente" BindingDescription-SourceTable="T_ANA_USL" BindingDescription-SourceField="USL_DESCRIZIONE" Filtro=" USL_SCADENZA IS NULL ORDER BY Descrizione"></ondp:wzFinestraModale>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblDistretto" runat="server">Distretto</asp:Label>
                                            </td>
                                            <td colspan="6">
                                                <ondp:wzFinestraModale id="txtDistretto" runat="server" Width="70%" LabelAssociata="lblDistretto" UseTableLayout="True" SetUpperCase="True" UseCode="True" CodiceWidth="30%" RaiseChangeEvent="False" CampoCodice="DIS_CODICE Codice" CampoDescrizione="DIS_DESCRIZIONE Descrizione" Tabella="T_ANA_DISTRETTI" Obbligatorio="False" PosizionamentoFacile="False" LabelWidth="-8px" DataTypeCode="Stringa" BindingCode-Editable="always" BindingCode-Connection="centrale" BindingCode-SourceTable="t_paz_pazienti_centrale" BindingCode-Hidden="False" BindingCode-SourceField="PAZ_DIS_CODICE" BindingDescription-Editable="always" BindingDescription-Hidden="False" DataTypeDescription="Stringa" BindingDescription-Connection="locale_dis" BindingDescription-SourceTable="T_ANA_DISTRETTI" BindingDescription-SourceField="DIS_DESCRIZIONE" Filtro="1=1 ORDER BY Descrizione"></ondp:wzFinestraModale>
                                            </td>
                                        </tr>
                                        <tr></tr>
                                        <tr>
                                            <td class="label">Filtro Macro Categoria Rischio</td>
									        <td colspan="6">
                                                <asp:DropDownList id="ddlMacrocategorieRischio" runat="server" DataValueField="MCR_CODICE" DataTextField="MCR_DESCRIZIONE" Width="100%"></asp:DropDownList>
									        </td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblCategorieRischio" runat="server">Categoria di rischio</asp:Label>
                                            </td>
                                            <td colspan="6">
                                                <ondp:wzFinestraModale id="txtCategorieRischio" runat="server" Width="70%" LabelAssociata="lblCategorieRischio" UseTableLayout="True" Label="Categorie rischio" SetUpperCase="True" UseCode="True" CodiceWidth="30%" RaiseChangeEvent="False" CampoCodice="RSC_CODICE Codice" CampoDescrizione="RSC_DESCRIZIONE Descrizione" Tabella="T_ANA_RISCHIO" Obbligatorio="False" PosizionamentoFacile="False" LabelWidth="-8px" DataTypeCode="Stringa" BindingCode-Editable="always" BindingCode-Connection="centrale" BindingCode-SourceTable="t_paz_pazienti_centrale" BindingCode-Hidden="False" BindingCode-SourceField="PAZ_RSC_CODICE" BindingDescription-Editable="always" BindingDescription-Hidden="False" DataTypeDescription="Stringa" BindingDescription-Connection="locale_ris" BindingDescription-SourceTable="T_ANA_RISCHIO" BindingDescription-SourceField="RSC_DESCRIZIONE"></ondp:wzFinestraModale>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr height="10">
                                <td colspan="2"></td>
                            </tr>
                        </table>
                    </div>
                        
                    <div>
                        <table style="table-layout: fixed; margin-left: 2px" id="Table3" border="0" cellspacing="0" cellpadding="2" width="99%">
                            <tr>
                                <td class="blocco_dati" colspan="2">
                                    <table border="0" cellspacing="0" cellpadding="2" width="100%">
                                        <tr height="10">
                                            <td width="15%"></td>
                                            <td width="85%"></td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblLibero4" runat="server" Text='Testo dal binding'></asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzFinestraModale id="txtLibero4" runat="server" Width="70%" LabelAssociata="lblLibero4" UseTableLayout="True" SetUpperCase="True" UseCode="True" CodiceWidth="30%" RaiseChangeEvent="False" CampoCodice="CAT_CODICE Codice" CampoDescrizione="CAT_DESCRIZIONE Descrizione" Tabella="T_ANA_CATEGORIE1" Obbligatorio="False" PosizionamentoFacile="False" LabelWidth="-8px" DataTypeCode="Stringa" BindingCode-Editable="always" BindingCode-Connection="locale" BindingCode-SourceTable="T_PAZ_PAZIENTI" BindingCode-Hidden="False" BindingCode-SourceField="PAZ_CAT_CODICE" BindingDescription-Editable="always" BindingDescription-Hidden="False" DataTypeDescription="Stringa" BindingDescription-Connection="locale" BindingDescription-SourceTable="T_ANA_CATEGORIE1" BindingDescription-SourceField="CAT_DESCRIZIONE" Filtro="1=1 ORDER BY Descrizione"></ondp:wzFinestraModale>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label">
                                                <asp:Label id="lblLibero5" runat="server" Text='Testo dal binding'></asp:Label>
                                            </td>
                                            <td>
                                                <ondp:wzFinestraModale id="txtLibero5" runat="server" Width="70%" LabelAssociata="lblLibero5" UseTableLayout="True" SetUpperCase="True" UseCode="True" CodiceWidth="30%" RaiseChangeEvent="False" CampoCodice="CAG_CODICE Codice" CampoDescrizione="CAG_DESCRIZIONE Descrizione" Tabella="T_ANA_CATEGORIE2" Obbligatorio="False" PosizionamentoFacile="False" LabelWidth="-8px" DataTypeCode="Stringa" BindingCode-Editable="always" BindingCode-Connection="locale" BindingCode-SourceTable="T_PAZ_PAZIENTI" BindingCode-Hidden="False" BindingCode-SourceField="PAZ_CAG_CODICE" BindingDescription-Editable="always" BindingDescription-Hidden="False" DataTypeDescription="Stringa" BindingDescription-Connection="locale" BindingDescription-SourceTable="T_ANA_CATEGORIE2" BindingDescription-SourceField="CAG_DESCRIZIONE" Filtro="1=1 ORDER BY Descrizione"></ondp:wzFinestraModale>
                                            </td>
                                        </tr>
                                        <tr height="10">
                                            <td colspan="2"></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr height="20">
                                <td colspan="2"></td>
                            </tr>
                        </table>
                    </div>

                </ondp:onitdatapanel>

                <asp:TextBox Style="display: none" ID="txtControlloConfermaSalva" runat="server">True</asp:TextBox>

            </dyp:DynamicPanel>

            <dyp:DynamicPanel ID="dypErrorMessage" runat="server" Width="100%">
                <uc1:GestionePazientiMessage ID="GestionePazientiMessage" runat="server"></uc1:GestionePazientiMessage>
            </dyp:DynamicPanel>

        </on_lay3:OnitLayout3>

        <on_ofm:OnitFinestraModale ID="modConsenso" Title="Consenso" runat="server" Width="800px" Height="600px" BackColor="LightGray" ClientEventProcs-OnClose="RefreshFromPopup()">
            <iframe id="frameConsenso" runat="server" class="frameConsensoStyle">
                <div>
                    Caricamento in corso. Attendere...
                </div>
            </iframe>
        </on_ofm:OnitFinestraModale>

        <on_ofm:OnitFinestraModale ID="modCalendarioVaccinale" Title="Calendario vaccinale"
            runat="server" Width="642px" BackColor="LightGray">
            <uc1:CalVac ID="uscCalVac" runat="server"></uc1:CalVac>
        </on_ofm:OnitFinestraModale>

        <on_ofm:OnitFinestraModale ID="fmSelectStampaLibrettoVaccinale" Title="Selezione della stampa" runat="server"
          Width="565px" Height="200px" BackColor="Steelblue" NoRenderX="False" RenderModalNotVisible="True">
            <div style="padding-top:5px; background-color: steelblue; font-family: Calibri; font-size: 12px; text-align: center">
                <igtbar:UltraWebToolbar BrowserTarget="UpLevel" ID="SelectStampaLibrettoVaccinaleToolBar" runat="server"
                    BackColor="Steelblue" BorderWidth="0px" BorderStyle="None" Width="100%"
                    ItemSpacing="1" MovableImage="/ig_common/images/ig_tb_move03.gif" ButtonStyle-Padding-Top="15px" ButtonStyle-Padding-Bottom="15px"
                    ItemWidthDefault="180px">
                    <DefaultStyle CssClass="toolbar-libretti-default"></DefaultStyle>
                    <HoverStyle CssClass="toolbar-libretti-hover"></HoverStyle>
                    <SelectedStyle CssClass="toolbar-libretti-selected"></SelectedStyle>
                    <ClientSideEvents InitializeToolbar="InizializzaToolBar" Click="ToolBarClick"></ClientSideEvents>
                    <Items>
                        <igtbar:TBarButton Key="btnSelectedLibrettoVaccinale" Text="<br/><br/>Singola pagina"
                            DisabledImage="~/Images/salva_dis.gif" Image="../../images/singolapagina.gif">
                        </igtbar:TBarButton>
                        <igtbar:TBSeparator Image="~/Images/transparent16.gif" />
                        <igtbar:TBarButton Key="btnSelectedLibrettoVaccinale2" Text="<br/><br/>Multipagina"
                            DisabledImage="~/Images/salva_dis.gif" Image="../../images/multipagina.gif">
                        </igtbar:TBarButton>
                        <igtbar:TBSeparator Image="~/Images/transparent16.gif" />
                        <igtbar:TBarButton Key="btnSelectedLibrettoVaccinale3" Text="<br/><br/>Etichette"
                            DisabledImage="~/Images/salva_dis.gif" Image="../../images/etichettepagina.gif">
                        </igtbar:TBarButton>
                    </Items>
            </igtbar:UltraWebToolbar>
            </div>
        </on_ofm:OnitFinestraModale>

        <on_ofm:OnitFinestraModale ID="confermaDatiPaziente" Title="Inserimento password" runat="server"
            Width="300px" BackColor="LightGray" NoRenderX="true" RenderModalNotVisible="true">
            <table>
                <tr>
                    <td>
                        <asp:Label ID="confermaDatiPaziente_label" runat="server" CssClass="label" Text="Alcuni campi fondamentali sono stati modificati. Introdurre la propria password per il salvataggio in centrale." />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox ID="confermaDatiPaziente_txt" runat="server" TextMode="Password" Text="Password"
                            ToolTip="Immettere la propria password" />
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:Button ID="confermaDatiPaziente_btnOk" runat="server" Text="OK" Width="100px" />
                        <asp:Button ID="confermaDatiPaziente_btnCancel" runat="server" Text="Annulla" Width="100px" />
                    </td>
                </tr>
            </table>
        </on_ofm:OnitFinestraModale>

        <onit:wwDataBinder ID="WwDataBinder1" runat="server">
            <DataBindingItems>
                <onit:wwDataBindingItem ID="WwDataBindingItem1" runat="server" BindingSource="Settings" BindingSourceMember="DESLIB1" ControlId="lblLibero1">
                </onit:wwDataBindingItem>
                <onit:wwDataBindingItem ID="WwDataBindingItem2" runat="server" BindingSource="Settings" BindingSourceMember="DESLIB2" ControlId="lblLibero2">
                </onit:wwDataBindingItem>
                <onit:wwDataBindingItem ID="WwDataBindingItem3" runat="server" BindingSource="Settings" BindingSourceMember="DESLIB3" ControlId="lblLibero3">
                </onit:wwDataBindingItem>
                <onit:wwDataBindingItem ID="WwDataBindingItem4" runat="server" BindingSource="Settings" BindingSourceMember="DESCAT1" ControlId="lblLibero4">
                </onit:wwDataBindingItem>
                <onit:wwDataBindingItem ID="WwDataBindingItem5" runat="server" BindingSource="Settings" BindingSourceMember="DESCAT2" ControlId="lblLibero5">
                </onit:wwDataBindingItem>
            </DataBindingItems>
        </onit:wwDataBinder>

    </form>

    <!-- #include file="../../Common/Scripts/AggiornaStatoVaccinazioni.inc" -->
    <!-- #include file="../../Common/Scripts/ControlloDatiValidi.inc" -->

</body>
</html>
