// reference jquery 1.7.1

function controlloNumero(obj) {
    obj.value = obj.value.replace(",", ".");
    if (isNaN(obj.value) || obj.value < 0) {
        alert("Inserire un numero valido!");
        obj.value = "";
        obj.focus();
    }
    obj.value = obj.value.replace(".", ",");
}

// Controlla che il testo presente nel textbox specificato sia formato solo da lettere, numeri e dai caratteri ".", "_" e "-".
// Il controllo del contenuto è effettuato solo se il campo è abilitato e non è vuoto.
function controlloCampoCodice(txt) {
    if (txt.disabled) return true;
    if (txt.readOnly != null && txt.readOnly) return true; // L'attributo "readOnly" è inserito se il campo è gestito dal pannello
    if (txt.value == '') return true;

    var pattern = '^[\\.\\-\\w]*$';
    var re = new RegExp(pattern, "g");

    if (!re.test(txt.value)) {
        alert('Codice non valido.');
        txt.focus();
        return false;
    }

    return true;
}

function toUpper(objCampo) {
    objCampo.value = objCampo.value.toUpperCase();
}

// Controllo della validità dell'orario di forzatura
function formattaOrario(obj) {

    var s = obj.value;
    var strRE = '^[0-1]?[0-9][.:]?$|^2[0-3][.:]?$|^[0-1]?[0-9][.:][0-5][0-9]?$|^2[0-3][.:][0-5][0-9]?$|^$';
    var re = new RegExp(strRE, "g");
    if (re.test(s)) {
        if (s != '') obj.value = formattaOra(s);
        return true;
    } else {
        alert("Inserire un orario valido!");
        obj.value = '';
        return false;
    }
}
       
function formattaOra(s) {
    ///	<returns>formato restituito: HH:mm</returns>
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
    //if (hh.length == 1) hh = '0' + hh;
    return hh + ':' + mm;
}

function confrontaDate(strDa, strA) {
    /// <param name="strDa">date in formato stringa gg/MM/yyyy</param>
    /// <param name="strA">date in formato stringa gg/MM/yyyy</param>
    ///	<returns> 0 se strDa = strA, 1 se strA maggiore di strDa, -1 se strDa maggiore di strA</returns>

    var arDa = strDa.split('/');
    var arA = strA.split('/');

    var dDa = new Date();
    dDa.setFullYear(arDa[2],arDa[1]-1,arDa[0]);	// tolgo 1 perchè il mese in javascript è rappresentato con un intero 0..11 e non 1..12

    var dA = new Date();
    dA.setFullYear(arA[2],arA[1]-1,arA[0]);

    if ( dDa > dA ) return -1;
    if ( dA > dDa ) return 1;
	
    // dDa = dA
    return 0;
}

// startDate: data di partenza in formato stringa "dd/MM/yyyy"
// days: numero di giorni da aggiungere
// Restituisce la data risultante (startDate + days) in formato stringa "dd/MM/yyyy"
function addDays(startDate, days) {
    if (startDate == null || startDate == "" || days == null) return "";

    if (isNaN(days)) return startDate;

    var dmy = startDate.split("/");
    var joindate = new Date(dmy[2], dmy[1] - 1, dmy[0]);

    joindate.setDate(joindate.getDate() + days);

    var endDate = ("0" + joindate.getDate()).slice(-2) + "/" +
                  ("0" + (joindate.getMonth() + 1)).slice(-2) + "/" +
                  joindate.getFullYear();

    return endDate;
}

/* Calcola il numero di giorni tra le due date, passate entrambe in formato stringa "dd/MM/yyyy" */
function dateDiffDays(dataInizio, dataFine, includiEstremi) {

    if (dataInizio == null || dataInizio == "" || dataFine == null || dataFine == "") return -1;
    
    var dmy = dataInizio.split("/");
    var date1 = new Date(dmy[2], dmy[1] - 1, dmy[0]);

    dmy = dataFine.split("/");
    var date2 = new Date(dmy[2], dmy[1] - 1, dmy[0]);

    if (date1 > date2) return -1;

    // Convert both dates to milliseconds
    var date1_ms = date1.getTime()
    var date2_ms = date2.getTime()
    
    // The number of milliseconds in one day
    var ONE_DAY = 1000 * 60 * 60 * 24

    // Calculate the difference in milliseconds
    var difference_ms = Math.abs(date1_ms - date2_ms)

    // Convert back to days and return
    var difference_days = Math.round(difference_ms / ONE_DAY);
    
    if (includiEstremi) difference_days++;

    return difference_days;
}

// Effettua la selezione/deselezione dei checkbox di un datagrid, in base al click del checkbox presente nell'intestazione di colonna.
// chk: oggetto checkBox che viene cliccato
// dgrId: id dell'asp:datagrid in cui è presente il checkbox
// columnIndexCheck: indice di colonna in cui sono presenti i checkbox nel datagrid
function selezionaTutti(chk, dgrId, columnIndexCheck) {
    var checkAll = true;

    if (chk.checked) {
        chk.checked = true;
        checkAll = true;
    } else {
        chk.checked = false;
        checkAll = false;
    }

    var dgr = document.getElementById(dgrId);

    for (i = 0; i < dgr.rows.length; i++) {
        var arrayTags = dgr.rows[i].cells[columnIndexCheck].getElementsByTagName('INPUT');
        if (arrayTags != null && arrayTags.length > 0) {
            arrayTags[0].checked = checkAll;
        }
    }

    return true;
}

/*
    Gestisce il mouseover e il mouseout andando a sostituire l'immagine "_dis" con l'immagine "_hov". 
*/
function mouseRollOver(obj, tipo) {
    if (obj != null && obj.src.indexOf('_dis') == -1) {
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

/*
    Metodi utilizzati per il caricamento di un modulo esterno da una pagina di redirect (es. OpenConsensi.aspx, OpenMVC.aspx)
*/
function encodeUrl(val) {
    var newStr = "";
    var len = val.length;
    var strTmp;

    for (var i = 0; i < len; i++) {
        if (val.substring(i, i + 1).charCodeAt(0) < 255)  // hack to eliminate the rest of unicode from this
        {
            if (isUnsafe(val.substring(i, i + 1)) == false) {
                newStr = newStr + val.substring(i, i + 1);
            }
            else {
                strTmp = val.substring(i, i + 1);
                strTmp = "%" + decToHex(strTmp.charCodeAt(0), 16);
                newStr = newStr + strTmp;
            }
        }
    }

    return newStr;
}

function isUnsafe(compareChar) {
    var unsafeString = "\"<>%\\^[]`\+\$\,";

    if (unsafeString.indexOf(compareChar) == -1 && compareChar.charCodeAt(0) > 32 && compareChar.charCodeAt(0) < 123) {
        return false;
    }
    else {
        return true;
    }
}

function decToHex(num, radix) {
    var hexString = "";
    var hexVals = new Array("0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F");

    while (num >= radix) {
        temp = num % radix;
        num = Math.floor(num / radix);
        hexString += hexVals[temp];
    }
    hexString += hexVals[num];

    return reversal(hexString);
}

function reversal(s) {
    var len = s.length;
    var trans = "";
    for (i = 0; i < len; i++) {
        trans = trans + s.substring(len - i - 1, len - i);
    }
    s = trans;

    return s;
}
/*
    ------------------------------------
*/