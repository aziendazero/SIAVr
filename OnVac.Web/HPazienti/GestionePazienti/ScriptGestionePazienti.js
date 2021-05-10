// reference jquery 1.7.1

//calcola il codice fiscale se il parametro AUTO_CF è valorizzato 'S' [modifica 29/07/2005]
function CalcoloCodiceFiscale(codiceFiscale) {
    var controllo = '';
    var strNome = document.getElementById('txtNome').value;
    if (ControllaValoreCampo(strNome)) controllo = 'Nome';

    var strCognome = document.getElementById('txtCognome').value;
    if (ControllaValoreCampo(strCognome)) controllo = 'Cognome';

    var strSesso = document.getElementById('cmbSesso').value;
    if (ControllaValoreCampo(strSesso)) controllo = 'Sesso';

    var strDataNascita = OnitDataPickGet('txtDataNascita');
    if (ControllaValoreCampo(strDataNascita)) controllo = 'Data Nascita';

    var strComune = getCodiceCatastale();
    if (ControllaValoreCampo(strComune)) controllo = 'Comune Nascita non ha un codice catastale associato oppure';


    if (controllo != '') {
        alert('Impossibile effettuare il calcolo automatico del Codice Fiscale\rin quanto il campo ' + controllo + ' non e\' valorizzato!');
        return;
    }
    //richiamo la funzione del validator per il calcolo
    var codFisc = calcolaCodiceFiscale(strNome, strCognome, strSesso, strDataNascita, strComune);

    if (codiceFiscale.value != codFisc) {
        if (confirm('Il Codice Fiscale del paziente ' + codiceFiscale.value + ' non corrisponde a quello calcolato: ' + codFisc + ' \nSostituirlo?'))
            codiceFiscale.value = codFisc;
        return true;
    }
    else
        return false;
}

//controlla che il campo per il calcolo del codice fiscale sia valorizzato [modifica 29/07/2005]
function ControllaValoreCampo(campo) {
    if (campo == '')
        return true;
}

/*tipoOperazione:	0 = OK
1 = annulla
2 = non ha trovato dati
3 = valore vuoto
*/
function ValorizzaIndirizzo(par1, par2, par3, par4, tipoOperazione, fm, campiImpostati) {
    var objIndirizzo = document.getElementById('txtIndirizzoSedeVaccinale');
    var objSedeVaccinale = document.getElementById('txtSedeVaccinale');
    var objSedeVaccinaleCod = document.getElementById('txtSedeVaccinale_Cod');

    switch (tipoOperazione) {
        case 0:
            objIndirizzo.value = campiImpostati['INDIRIZZO'];
            objSedeVaccinale.value = campiImpostati['DESCRIZIONE'];
            objSedeVaccinaleCod.value = campiImpostati['CODICE'];
            FM_OK_Click(par1, par2, par3, par4, tipoOperazione, campiImpostati);
            break;
        case 1:
        case 2:
            FM_Annulla_Click(par1, par2, '', '', tipoOperazione, campiImpostati);
            break;
        case 3:
            objIndirizzo.value = '';
            FM_Annulla_Click(par1, par2, '', '', tipoOperazione, campiImpostati);
            break;
    }
}
function SetFocusToMedico() {
    document.getElementById('txtMedicoResponsabile').focus();
}

/* anty 19/1/05 - verifica se tutte le finestre modali sono allineate*/
function controllaValiditaFM() {

    if (!isValidFinestraModale("txtComuneDiNascita", false)) { alert("Il campo 'Comune di nascita' non era aggiornato.\nRiprovare."); return false; }
    //if (!isValidFinestraModale("txtComuneResidenza",true)){ alert("Il campo 'Comune di residenza' e\' obbligatorio.");return false;}
    if (!isValidFinestraModale("txtComuneDomicilio", false)) { alert("Il campo 'Comune di domicilio' non era aggiornato.\nRiprovare."); return false; }
    if (!isValidFinestraModale("txtComuneResidenza", false)) { alert("Il campo 'Comune di residenza' non era aggiornato.\nRiprovare."); return false; }
    if (!isValidFinestraModale("txtCittadinanza", false)) { alert("Il campo 'Cittadinanza' non era aggiornato.\nRiprovare."); return false; }
    if (!isValidFinestraModale("txtLuogoImmigrazione", false)) { alert("Il campo 'Luogo di immigrazione' non era aggiornato.\nRiprovare."); return false; }
    if (!isValidFinestraModale("txtLuogoEmigrazione", false)) { alert("Il campo 'Luogo di emigrazione' non era aggiornato.\nRiprovare."); return false; }
    if (!isValidFinestraModale("txtSedeVaccinale", false)) { alert("Il campo 'Sede vaccinale' non era aggiornato.\nRiprovare."); return false; }
    if (!isValidFinestraModale("txtLibero4", false)) { alert("Il campo 'Scuola' non era aggiornato.\nRiprovare."); return false; }
    return true;
}

//Brigoz 11-02-05
//Nel caso lo stato anagrafico sia impostato diverso da Domiciliato o residente deve essere
//possibile cancellare tutta la programmazione del paziente
function ControllaNuovoStato(obj, stringaStatiAnagrafici) 
{
    if (stringaStatiAnagrafici != '')
    {
        var codiciStatiAnagrafici = stringaStatiAnagrafici.split('|');

        if ($.inArray(obj.value, codiciStatiAnagrafici) != -1)
        {
            if (confirm("Lo stato anagrafico selezionato e\' uno di quelli per cui e\' prevista la CANCELLAZIONE DELLA PROGRAMMAZIONE VACCINALE relativa al paziente. L\'operazione NON E\' ANNULLABILE, le modifche verranno apportate direttamente su database! Continuare?")) 
            {
                __doPostBack('EliminaProg_cambioStatus', 'EliminaProg_cambioStatus')
            }
            return;                
        }
    }
}

//Esecuzione al load	
if (document.getElementById('btnOKCampiNoSet') != null)
    document.getElementById('btnOKCampiNoSet').onclick = function (evt) { closeFm("modCampiNoSet"); StopPreventDefault(getEvent(evt)); };


if (document.getElementById('btnChiudiInfoMedico') != null)
    document.getElementById('btnChiudiInfoMedico').onclick = function (evt) { closeFm("fmInfoMedico"); StopPreventDefault(getEvent(evt)); };

function getCodiceCatastale() {
    var altriValori = document.getElementById("Ret_txtComuneDiNascita").value.split("|");
    var retValue = document.getElementById("txtCatastaleNas").value;
    if (retValue == null || retValue == "") {
        if (altriValori.length >= 3) { retValue = altriValori[2]; } else { retValue = ""; }
    };
    return retValue;
}

function hideDettaglio(table) {
    if (document.getElementById(table).style.display == 'none')
        document.getElementById(table).style.display = 'block';
    else
        document.getElementById(table).style.display = 'none';
}



/* 
    Gestione toolbar per la stampa dei libretti vaccinali  
*/
function InizializzaToolBar(t) {
    t.PostBackButton = false;
}

function ToolBarClick(ToolBar, button, evnt) {
    evnt.needPostBack = true;

    switch (button.Key) {
        case 'btnSelectedLibrettoVaccinale':
        case 'btnSelectedLibrettoVaccinale2':
        case 'btnSelectedLibrettoVaccinale3':

            closeFm('fmSelectStampaLibrettoVaccinale');
            break;
    }
}   