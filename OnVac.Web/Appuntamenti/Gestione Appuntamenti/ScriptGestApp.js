// reference jquery 1.7.1

 function StampaLogClient() {
    if (confirm("Stampare solamente gli appuntamenti falliti?")) {
        __doPostBack("StampaLog", "S");
    } else __doPostBack("StampaLog", "N");
    return false;
}

function espandiMessaggi(img) {
    var espanso = document.getElementById('txtEspanso').value;
    var dyp = getDynamicPanelById('dypCalendario');

    if (espanso == 'true') {
        dyp.show(true);
        img.src = '../../images/espandi_risultati.gif';
        img.title = 'Espande il riquadro dei risultati e chiude i messaggi';
        document.getElementById('txtEspanso').value = 'false';
    }
    else {
        dyp.show(false);
        img.src = '../../images/chiudi_risultati.gif';
        img.title = 'Chiude il riquadro dei risultati e visualizza i messaggi';
        document.getElementById('txtEspanso').value = 'true';
    }
}

function espandiFiltri(img) {
    var Visfiltro = document.getElementById('txtFiltri').value;
    var dyp = getDynamicPanelById('dypFiltri');

    if (Visfiltro == 'true') {
        dyp.show(true);
        img.src = '../../images/chiudi_filtri.gif';
        img.title = 'Espande il riquadro dei filtri';
        document.getElementById('txtFiltri').value = 'false';
    }
    else {
        dyp.show(false);
        img.src = '../../images/espandi_filtri.gif';
        img.title = 'Chiude il riquadro dei filtri';
        document.getElementById('txtFiltri').value = 'true';
    }
}

function gestisciInvio(evt, ojb) {
    if (evt.keyCode == 13) {
        VaiAData();
        StopPropagation(evt);
    }

}

function strToUpperCase(obj) {
    obj.value = obj.value.toUpperCase();
}

function dataValida(data) {
    if (data != "") {//visto che la data non è obbligatoria, se il campo è vuoto non eseguo controlli
        if (OnitDate_isDate(data))
            return true;
        else {
            alert("La data specificata non è valida!");
            return false;
        }
    }
}


/////// MARCO 05/01/2004  /////////////////////////////////////////////////////
// Procedura richiamata ogni qual volta                                      //
// l'utente cambia la Tab da visualizzare                                    // 
// La funzione è associata all'evento ClientSide beforeSelectedTabChange     //
// della TAB INFRAGISTICS                                                    // 
///////////////////////////////////////////////////////////////////////////////

function TabChange(owner, tabItem, evnt) {
    //Controllo su quale TAB ho swicciato

    if (tabItem.getText().toString() == "Opzioni") {
        owner.element.style.height = "220px"
    }
    //else{
    //		modificaTab(owner)
    //}			
}



///////////////////MARCO 31/12/03///////////////////
// Valida un orario nella forma HH:MM:SS o HH:MM  //
// Parametri:   sOra:la stringa da validare       //
// Valori di ritorno:                             //
//                    true: orario valido         //
//                    false: orario non valido    //
////////////////////////////////////////////////////

function orarioValido(sOra) {

    var splOra = sOra.split(":");
    searchDot = sOra.split(".");
    if (searchDot.length != 1)//cerco l'eventuale presenza di punti nella stringa
        return false;
    if ((isNaN(splOra[0])) || (isNaN(splOra[1])))
        return false;
    else
        if (splOra.length == 3)
            if (isNaN(splOra[2]))
                return false;

    if ((splOra.length == 3) || (splOra.length == 2)) {
        if ((splOra[0].length > 2) || (splOra[1].length > 2) || (splOra[0].length < 0) || (splOra[1].length < 0)) {
            return false;
        }
        else
            if (splOra.length == 3)
                if ((splOra[2].length) > 2 || (splOra[2].length) < 0)
                    return false;

        splOra[0] = parseInt(splOra[0]);
        splOra[1] = parseInt(splOra[1]);

        if ((splOra[0] < 0) || (splOra[0] > 23)) {
            return false;
        }
        if ((splOra[1] < 0) || (splOra[1] > 59)) {
            return false;
        }
        if (splOra.length == 3) {
            splOra[2] = parseInt(splOra[2]);
            if ((splOra[2] < 0) || (splOra[2] > 59))
                return false;
        }
        return true;
    }
    return false;
}


function AnnullaCambiamenti() {
    OnitLayoutStatoMenu(true);
    __doPostBack("AnnullaCambiamenti", "");
}

function AppuntamentoAutomatico() {
    var strPrenota = '';
    if (igtab_getTabById("Tab1").Tabs[1].findControl('txtDurata').value != '')
        strPrenota += "I pazienti selezionati verranno prenotati con durata appuntamento [" + igtab_getTabById("Tab1").Tabs[1].findControl('txtDurata').value + "] minuti.\n";

    if (igtab_getTabById("Tab1").Tabs[1].findControl('txtNumNuoviPazientiAlGiorno').value != '')
        strPrenota += "Verranno Prenotati [" + igtab_getTabById('Tab1').Tabs[1].findControl('txtNumNuoviPazientiAlGiorno').value + "] nuovi pazienti al giorno, come da voi impostato.\n";

    if (igtab_getTabById("Tab1").Tabs[1].findControl('txtNumPazientiAlGiorno').value != '') {
        strPrenota += "Verranno Prenotati al massimo [" + igtab_getTabById('Tab1').Tabs[1].findControl('txtNumPazientiAlGiorno').value + "] pazienti al giorno, come da voi impostato.";
        strPrenota += "\n\nLa procedura potrebbe durare alcuni minuti\n\nPremere OK per continuare";
        if (confirm(strPrenota)) {
            OnitLayoutStatoMenu(true);
            __doPostBack("AppuntamentoAutomatico", "");
        }
    } else {
        OnitLayoutStatoMenu(true);
        __doPostBack("AppuntamentoAutomatico", "");
    }
}

//controllo che le date impostate siano effettivamente valide [modifica 09/08/2005]
function OnitDataPick_Blur(id, e) {
    var tipoId = '';
    if (id.search('txtDaDataNascita') > -1) tipoId = 'Data Nascita Iniziale';
    if (id.search('txtADataNascita') > -1) tipoId = 'Data Nascita Finale';
    if (id.search('txtFinoAData') > -1) tipoId = 'Data Massima Convocazione';
    if (id.search('odpDataInizPrenotazioni') > -1) tipoId = 'Data Inizio Prenotazione';
    if (id.search('odpDataFinePrenotazioni') > -1) tipoId = 'Data Fine Prenotazione';

    if (!(ControllaData(OnitDataPickGet(id)))) {
        alert("Attenzione: il campo '" + tipoId + "' ha un formato data non valido!");
        OnitDataPickFocus(id, 1, true);
    }
}

function ControllaData(field) {
    var checkstr = "0123456789";
    var DateField = field;
    var Datevalue = "";
    var DateTemp = "";
    var seperator = ".";
    var day;
    var month;
    var year;
    var leap = 0;
    var err = 0;
    var i;
    err = 0;
    DateValue = field;
    for (i = 0; i < DateValue.length; i++) {
        if (checkstr.indexOf(DateValue.substr(i, 1)) >= 0) {
            DateTemp = DateTemp + DateValue.substr(i, 1);
        }
    }
    DateValue = DateTemp;
    if (DateValue.length == 6) {
        DateValue = DateValue.substr(0, 4) + '20' + DateValue.substr(4, 2);
    }
    if (DateValue.length != 8) {
        err = 19;
    }
    year = DateValue.substr(4, 4);
    if (year == 0) {
        err = 20;
    }
    month = DateValue.substr(2, 2);
    if ((month < 1) || (month > 12)) {
        err = 21;
    }
    day = DateValue.substr(0, 2);
    if (day < 1) {
        err = 22;
    }
    if ((year % 4 == 0) || (year % 100 == 0) || (year % 400 == 0)) {
        leap = 1;
    }
    if ((month == 2) && (leap == 1) && (day > 29)) {
        err = 23;
    }
    if ((month == 2) && (leap != 1) && (day > 28)) {
        err = 24;
    }
    if ((day > 31) && ((month == "01") || (month == "03") || (month == "05") || (month == "07") || (month == "08") || (month == "10") || (month == "12"))) {
        err = 25;
    }
    if ((day > 30) && ((month == "04") || (month == "06") || (month == "09") || (month == "11"))) {
        err = 26;
    }
    if ((day == 0) && (month == 0) && (year == 00)) {
        err = 0; day = ""; month = ""; year = ""; seperator = "";
    }
    if (err == 0) {
        return true;
    }
    else {
        return false;
    }
}

function PrenotaManuale(dove) {
    __doPostBack("AggiuntaManuale", dove);
}

function VaiAData() {
    data = document.getElementById("txtVaiAData").value;
    if (OnitDate_isDate(data))
        __doPostBack("CambiaData", "");
    else
        alert("Data non valida!!");
}

function CheckCheckBox(evt) {
    if (this.checked)
        if ((this.parentNode.parentNode.parentNode.firstchild.firstchild) != null)
            if (!confirm("Sono stati selezionati pazienti a cui è stato già inviato l'avviso. Premere Ok per continuare, Annulla per deselezionare questi ultimi."))
                this.checked = false;
}

/*
Checkka la lista dei pazienti	
*/
function chkSelDeselDtaRicercaConvocati_OnClick(oggetto) {
    //SelRicercaConvocati(oggetto.checked);
    //la selezione deve avvenire dal server [modifica 26/04/2005]
    __doPostBack('SelezionaTutti', oggetto.checked)
}

function SelRicercaConvocati(sel) {
    if (document.getElementById("dlsRicercaConvocati") != null) {
        c = document.getElementById("dlsRicercaConvocati");
        for (i = 0; i < c.childNodes.length; i++) {
            if (c.childNodes[i].childNodes.length > 0) {
                var obj = GetElementByTag(c.childNodes[i], 'TABLE', 1, 1, false);
                var objCheck = GetElementByTag(obj.rows[0].cells[0], 'INPUT', 1, 1, false);

                var obj = GetElementByTag(c.childNodes[i], 'TABLE', 1, 1, false);
                var objCheck = GetElementByTag(obj.rows[0].cells[0], 'INPUT', 1, 1, false);

                //non deve selezionare i termini perentori [modifica 22/04/2005]
                var cella = objCheck.parentNode.parentNode; 		//TD
                var imgAlert = cella.getElementsByTagName('IMG')[0];
                if (!(imgAlert == null))
                    var idImg = imgAlert.getAttribute('Id');
                // ricerca dell'immagine del termine perentorio
                if (!(idImg.search('Image2') > 0))
                    objCheck.checked = sel;
            }
        }
    }
}

function chkSelDeselMattina() {
    SelMatPom("lsMattino", this.checked);
}

function chkSelDeselPomeriggio() {
    SelMatPom("lsPomeriggio", this.checked);
}

function SelMatPom(matpom, sel) {
    var showmessage;
    var tab = igtab_getTabById("Tab2").Tabs[0];
    var matpomobj = tab.findControl(matpom);
    if (matpomobj != null) {
        for (i = 0; i < matpomobj.childNodes.length; i++) {
            if ((matpomobj.childNodes[i].childNodes.length > 0)) {
                var obj = GetElementByTag(matpomobj.childNodes[i], 'TABLE', 1, 1, false);
                var objCheck = GetElementByTag(obj.rows[0].cells[1], 'INPUT', 1, 1, false);
                var objImg = GetElementByTag(obj.rows[0].cells[0], 'IMG', 1, 1, false);
                if (sel == true)
                    if (objImg != null) {
                        if (objImg.getAttribute('nomeImmagine') == '0')
                            showmessage = true;
                    }
                objCheck.checked = sel;
            }
        }
    }

    if (showmessage) {
        if (!confirm("Attenzione: per alcuni pazienti e' gia' stato stampato l'avviso: si desidera comunque selezionarli?")) {
            for (i = 0; i < matpomobj.childNodes.length; i++) {
                if (matpomobj.childNodes[i].childNodes.length > 0) {
                    var obj = GetElementByTag(matpomobj.childNodes[i], 'TABLE', 1, 1, false);
                    var objCheck = GetElementByTag(obj.rows[0].cells[1], 'INPUT', 1, 1, false);
                    var objImg = GetElementByTag(obj.rows[0].cells[0], 'IMG', 1, 1, false);
                    if (objImg != null)
                        objCheck.checked = false;
                }
            }
        }
    }
}

function showHideDatalist(obj, stato) {
    if (obj != null)
        obj.style.display = stato;
}

function showHideEl(el, visible) {
    var vis;
    if (visible)
        vis = '';
    else
        vis = 'none';
    el.style.display = vis;
}

function btnRicApplica() {
    var dlsRicercaConvocati = document.getElementById("dlsRicercaConvocati");
    var tab = igtab_getTabById("Tab1").Tabs[1];
    if (dlsRicercaConvocati != null) {
        for (i = 0; i < dlsRicercaConvocati.childNodes.length; i++) {
            if (dlsRicercaConvocati.childNodes[i].nodeName == "BR")
                dlsRicercaConvocati.childNodes[i].style.display = "none";
            if (dlsRicercaConvocati.childNodes[i].childNodes.length > 0) {
                var objTab = GetElementByTag(dlsRicercaConvocati.childNodes[i], 'TABLE', 1, 1, false);
                showHideEl(objTab.rows[0].cells[2], tab.findControl("chkRicConvocazioni").checked);
                showHideEl(objTab.rows[1].cells[1], tab.findControl("chkRicBilancio").checked);
                showHideEl(objTab.rows[1].cells[2], tab.findControl("chkRicMedico").checked);
                showHideEl(objTab.rows[1].cells[0], tab.findControl("chkRicCiclo").checked);
            }
        } //fine for
        return false;
    }

    // controlla il valore di txtNumPazientiAlGiorno
    if (tab.findControl("txtNumPazientiAlGiorno").value != "") {
        if ((!(isNaN(tab.findControl("txtNumPazientiAlGiorno").value))) && (parseInt(tab.findControl("txtNumPazientiAlGiorno").value) > 0) && (parseInt(tab.findControl("txtNumPazientiAlGiorno").value) <= 50)) {
            return true;
        } else {
            alert('Valore non corretto!');
            tab.findControl("txtNumPazientiAlGiorno").value = "";
            tab.findControl("txtNumPazientiAlGiorno").focus();
        }
    }
    return false;

} //fine funzione

function btnAppApplica() {

    var tab = igtab_getTabById("Tab2").Tabs[0];
    var tab1 = igtab_getTabById("Tab2").Tabs[1];

    for (t = 0; t < 2; t++) {
        if (t == 0)
            var matpomobj = tab.findControl("lsMattino");
        else
            var matpomobj = tab.findControl("lsPomeriggio");

        if (matpomobj != null) {
            for (i = 0; i < matpomobj.childNodes.length; i++) {
                if (matpomobj.childNodes[i].nodeName == "BR" || matpomobj.childNodes[i].nodeName == "br")
                    matpomobj.childNodes[i].style.display = "none";
                if (matpomobj.childNodes[i].childNodes.length > 0) {
                    var objTab = GetElementByTag(matpomobj.childNodes[i], 'TABLE', 1, 1, false);
                    showHideEl(objTab.rows[1].cells[1], tab1.findControl("chkAppConvocazione").checked);
                    showHideEl(objTab.rows[1].cells[2], tab1.findControl("chkAppDurata").checked);
                    showHideEl(objTab.rows[1].cells[3], tab1.findControl("chkAppMedico").checked);
                    //Brigoz 23-11-04
                    showHideEl(objTab.rows[2].cells[1], tab1.findControl("chkAppVaccinazioni").checked);
                }
            }
        }
    }
    return false;
}

function btnPulisci_onclick() {
    document.getElementById("txtRicercaCognome").value = "";
    document.getElementById("txtRicercaNome").value = "";
    document.getElementById("txtRicercaDataNascita").value = "";
}


function InizializzaToolBar(t) {
    t.PostBackButton = false;
}

function ToolBarClick(ToolBar, button, evnt) {
    evnt.needPostBack = true;

    switch (button.Key) {
        case 'AnnullaCambiamenti':
            if (confirm("Procedendo verranno annullati tutti i cambiamenti effettuati!")) {
                AnnullaCambiamenti();
            }
            evnt.needPostBack = false;
            break;

        case 'PrenotaAutomatico':
            AppuntamentoAutomatico();
            evnt.needPostBack = false;
            break;

        case 'Cerca':
            evnt.needPostBack = CercaConvocati();
            break;
    }
}

// Gestisce il click e il rollover del pulsante di stampa convocati selezionati (visualizzato dopo la ricerca)
function btn_stampa_sel_mouse(div_btn, id_btn, tipo) {
    var btn = document.getElementById(id_btn);
    if (!btn.disabled) {
        switch (tipo) {
            case 'over':
                div_btn.style.textDecoration = 'underline';
                div_btn.style.fontSize = '10px';
                break;
            case 'out':
                div_btn.style.textDecoration = 'none';
                div_btn.style.fontSize = '10px';
                break;
            case 'click':
                btn.click();
                break;
        }
    }
    return false;
}

function CaricaExpandList() {
    var tab = igtab_getTabById("Tab2").Tabs[0];
    var pomobj = tab.findControl("lsPomeriggio");
    var matobj = tab.findControl("lsMattino");
    var imgmatobj = tab.findControl("imgls_Mattino");
    var imgpomobj = tab.findControl("imgls_Pomeriggio");

    if ((state0 == false) && (state1 == false)) {

        showHideDatalist(matobj, "none");
        showHideDatalist(pomobj, "none");
        imgmatobj.src = imagesPath + "piu.gif";
        imgpomobj.src = imagesPath + "piu.gif";
    }
    else if ((state0 == true) && (state1 == false)) {

        showHideDatalist(matobj, "none");
        showHideDatalist(pomobj, "inline");
        imgmatobj.src = imagesPath + "piu.gif";
        imgpomobj.src = imagesPath + "meno.gif";
    }
    else if ((state0 == false) && (state1 == true)) {

        showHideDatalist(matobj, "inline");
        showHideDatalist(pomobj, "none");
        imgmatobj.src = imagesPath + "meno.gif";
        imgpomobj.src = imagesPath + "piu.gif";
    }
    else if ((state0 == true) && (state1 == true)) {

        showHideDatalist(matobj, "inline");
        showHideDatalist(pomobj, "inline");
        imgmatobj.src = imagesPath + "meno.gif";
        imgpomobj.src = imagesPath + "meno.gif";
    }
}

// Controllo filtri di ricerca
function CercaConvocati() {
    // Data CNV
    if (OnitDataPickGet(txtFinoADataClientId) == "") {
        // La ricerca non può essere avviata
        alert("E' obbligatorio valorizzare il campo 'Data CNV'. Ricerca non effettuata.");
        return false;
    }

    // Intervallo di nascita
    if ((OnitDataPickGet(txtDaDataNascitaClientId) == "") && (OnitDataPickGet(txtADataNascitaClientId) == "")) {
        // Se entrambi gli estremi sono vuoti, controlla il check Data Singola
        if (!document.getElementById(chkDataSingolaClientId).checked) {
            // Richiesta di continuare anche se non sono state impostate le date di nascita
            if (!confirm("Non sono stati impostati gli estremi per la data di nascita. Il calcolo potrebbe richiedere molto tempo. Continuare?"))
            // In caso negativo, non va al server
                return false;
        }
    }

    OnitLayoutStatoMenu(true);
    return true;
}

function dateCompatibili(id1, id2, mode) {
    data1 = OnitDataPickGet(id1);
    if (mode == '0') data2 = OnitDataPickGet(id2);
    if (mode == '1') data2 = id2;
    splitData1 = data1.split('/');
    splitData2 = data2.split('/');

    if (!(parseInt(splitData1[2]) > parseInt(splitData2[2]))) {
        if (parseInt(splitData1[2]) < parseInt(splitData2[2]))
            return true;
        else {
            if (!((splitData1[1]) > (splitData2[1]))) {
                if ((splitData1[1]) < (splitData2[1])) {
                    return true;
                }
                else {
                    if ((splitData1[0]) <= (splitData2[0])) {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
            }
            else {
                return false;
            }
        }
    }
    else {
        return false;
    }
}

function mouse(obj, tipo) {
    if (obj.src.indexOf('_dis') == -1) {
        var idx_file = obj.src.lastIndexOf('/') + 1;
        var new_path = obj.src.substr(0, idx_file);
        var new_file = obj.src.substr(idx_file);
        var idx_ext = new_file.indexOf('.');
        var new_ext = new_file.substr(idx_ext);
        new_file = new_file.substr(0, idx_ext);

        if (tipo == 'over') {
            obj.src = new_path + new_file + '_hov' + new_ext;
        } else {
            obj.src = new_path + new_file.substr(0, new_file.indexOf('_hov')) + new_ext;
        }
    }
    return true;
}

function clickOrariPers() {
    document.getElementById(btnOrariPersClientId).click();
    return true;
}

function mouseOrariPers(obj, tipo) {
    if (tipo == 'over')
        obj.style.backgroundImage = 'url(../../images/bg_oraripers_hov.gif)';
    else
        obj.style.backgroundImage = 'url(../../images/bg_oraripers.gif)';
    return true;
}


function onLoad() {

    // Con il multiview vengono inseriti nella pagina solo i controlli della ActiveView
    if (mltViewActiveViewID === viewGestioneAppuntamenti) {

        //per mantenere il riquadro dei risultati espanso [modifica 26/04/2005]
        if (!IsPostBack) {
            document.getElementById('txtEspanso').value = "true";
            document.getElementById('txtFiltri').value = "false";
        }

        if (document.getElementById('txtEspanso').value == 'true') {
            getDynamicPanelById('dypCalendario').show(false);
        }
        else {
            getDynamicPanelById('dypCalendario').show(true);
        }
        
        if (document.getElementById('txtFiltri').value == 'true') {
            getDynamicPanelById('dypFiltri').show(false);
        }
        else {
            getDynamicPanelById('dypFiltri').show(true);
        }

        if (document.getElementById('btnStampaLog') != null)
            document.getElementById('btnStampaLog').onclick = function () { StampaLogClient() };

        document.getElementById('chkSelDeselPomeriggio').onclick = chkSelDeselPomeriggio;
        document.getElementById('chkSelDeselMattina').onclick = chkSelDeselMattina;
        // igtab_getTabById("Tab1").Tabs[1].findControl('btnRicApplica').onclick = btnRicApplica;

        igtab_getTabById("Tab2").Tabs[1].findControl('btnAppApplica').onclick = btnAppApplica;
        CaricaExpandList();

        btnRicApplica();
        btnAppApplica();

        if (modSalvataggio) {
            AppuntamentoAutomatico();
        }
    }
}

//igtab_getTabById("Tab1").Tabs[2].findControl('btnModificaDurate').onclick=confermaModificheDurata;
function confermaModificheDurata() {
    if ((isNaN(igtab_getTabById("Tab1").Tabs[1].findControl('txtDurata').value)) || (igtab_getTabById("Tab1").Tabs[1].findControl('txtDurata').value < 0) || (igtab_getTabById("Tab1").Tabs[1].findControl('txtDurata').value == 0)) {
        alert("La durata dell'appuntamento deve essere un valore numerico valido");
        return false;
    }
    if (!confirm('Continuando si andranno a modificare in maniera permanente le durate degli appuntamenti relativi le convocazioni selezionate.Si desidera proseguire?'))
        return false;
}

function checkFormatoOrario(obj) {
    if (!validazioneOrario(obj.id)) {
        alert('L\'orario inserito non e\' corretto');
        obj.focus();
        return false;
    }
    return true;
}

function validazioneOrario(idTxtOrario) {
    var s = document.getElementById(idTxtOrario).value;
    var strRE = '^[0-1]?[0-9][.:]?$|^2[0-3][.:]?$|^[0-1]?[0-9][.:][0-5][0-9]?$|^2[0-3][.:][0-5][0-9]?$|^$';
    var re = new RegExp(strRE, "g");
    if (re.test(s)) {
        if (s != '') document.getElementById(idTxtOrario).value = formattaOra(s);
        return true;
    } else {
        document.getElementById(idTxtOrario).value = '';
        return false;
    }
}

// formato restituito: hh.mm
function formattaOra(s) {
    var tmp = s;
    var hh = '';
    var mm = '00';

    var strRE = '[.:]'
    var re = new RegExp(strRE, "g");
    if (re.test(tmp)) { // Separatore dei minuti presente
        var idxSep = tmp.indexOf('.');
        if (idxSep == -1) idxSep = tmp.indexOf(':');
        hh = tmp.substr(0, idxSep);
        tmp = tmp.substr(idxSep + 1);
        if (tmp.length == 1) mm = tmp + '0';
        else if (tmp.length == 2) mm = tmp;
    }
    if (hh == '') hh = tmp;
    if (hh.length == 1) hh = '0' + hh;
    return hh + '.' + mm;
}



// Controllo che ci sia almeno una fascia oraria personalizzata (testando il valore del 
// campodi testo nascosto hid_txt_num_orari_pers, valorizzato lato server).
// Il controllo scatta al click del checkbox chkOrariPers.
function checkNumOrariPers() {
    var txt = document.getElementById(hid_txt_num_orari_persClientId);
    var chk = document.getElementById(chkOrariPersClientId);

    if (txt.value == '0') {
        chk.checked = false;
        alert('Impossibile utilizzare gli orari personalizzati: nessun orario personalizzato impostato!');
    }

    return;
}

// Controllo campo numerico
function checkFormatoNumerico(obj) {
    var s = obj.value;
    var strRE = '^\\d*$|^$';
    var re = new RegExp(strRE, "g");

    if (!re.test(obj.value)) {
        obj.value = '';
        alert('Il valore inserito non e\' corretto: deve essere un numero intero.');
        obj.focus();
        return false;
    }
    return true;
}

function clickAvviaRicerca(msg) {
    OnitLayoutStatoMenu(true);
    return true;
}

function pulisciFiltri() {
    document.getElementById(btnPulisciFiltri).click();
    return true;
}