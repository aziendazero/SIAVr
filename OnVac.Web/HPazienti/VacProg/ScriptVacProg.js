
// Indici delle colonne del datagrid dg_vacProg
var idxVacProg = new function () {
    this.CheckBox = 0;
    this.BtnElimina = 1;
    this.BtnModificaConferma = 2;
    this.Associazione = 3;
    this.VacObbligatoria = 4;
    this.ReazioneAvversa = 5;
    this.Vaccinazione = 6;
    this.Dose = 7;
    this.Lotto = 8;
    this.NomeCommerciale = 9;
    this.ViaSomministrazione = 10;
    this.SitoInoculazione = 11;
    this.CondizioneSanitaria = 12;
    this.CondizioneRischio = 13;
    this.BtnPagamento = 14;
    this.Note = 15;
    this.InfoVaccinazione = 16;
    this.VacEseguitaEsclusa = 17;
    this.Ciclo = 18;                    // visible=false
    this.Seduta = 19;                   // visible=false
    this.AssociazioneDose = 20;         // visible=false
    this.Vaccinatore = 21;              // visible=false
};

var busy;

var statusVaccinale;

var doseVacProg;

var paz_deceduto;

var sceltaAccesso = false;

function InizializzaToolBar_DetAss(t) {
    t.PostBackButton = false;
}

function ToolBarClick_DetAss(ToolBar, button, evnt) {
    evnt.needPostBack = true;

    switch (button.Key) {
        case 'btn_Conferma':
            if (OnitDatePick.tb_data_detAss[0].Get() == "") {
                alert("Il campo 'Data' è obbligatorio. Non è possibile aggiornare la riga.");
                OnitDatePick.tb_data_detAss[0].Focus(1, false);
                evnt.needPostBack = false;
                break;
            }
            break;
    }
}

function InizializzaToolBar(t, evnt) {
    t.PostBackButton = false;
}

function InizializzaToolBar_SceltaAss(t, evnt) {
    t.PostBackButton = false;
}

function ToolBarClick_SceltaAss(ToolBar, button, evnt) {
    evnt.needPostBack = true;

    switch (button.Key) {
        case 'btn_Conferma':
            //salva in text box i valori selezionati nei radio button

            //panel contenente gli user-control(label-radiobutton)
            var cpag = document.getElementById("cpag");
            var i = 0;
            var assCheched;
            var assCodice;
            var assDescrizione;
            var table;
            var codLotto;

            var nDiv;

            if (cpag.childElementCount != null) {
                // Edge e Mozilla
                nDiv = cpag.childElementCount;

                for (i = 0; i < nDiv; i++) {
                    div = cpag.children[i];
                    codLotto = div.children[1].innerHTML;

                    table = div.children[3];
                    var tr = table.children[0].children[0];

                    // per ogni radio button
                    for (j = 0; j < tr.cells.length; j++) {
                        assCheched = tr.cells[j].children[0].checked;
                        if (assCheched == true) {
                            assCodice = tr.cells[j].children[0].value;
                            assDescrizione = tr.cells[j].children[1].innerHTML;
                            document.getElementById("AssociazioniScelteLatoClient").value += codLotto + "|" + assCodice + "|" + assDescrizione + "|";
                        }
                    }
                }
            }
            else {
                // fino a IE11
                nDiv = cpag.childNodes.length;

                for (i = 0; i < nDiv; i++) {
                    div = cpag.childNodes(i);
                    codLotto = div.childNodes(2).childNodes(0).nodeValue;
                    table = div.childNodes(6);

                    // per ogni radio button
                    for (j = 0; j < table.cells.length; j++) {
                        assCheched = table.childNodes(0).childNodes(0).childNodes(j).childNodes(0).checked;
                        if (assCheched == true) {
                            assCodice = table.childNodes(0).childNodes(0).childNodes(j).childNodes(0).value;
                            assDescrizione = table.childNodes(0).childNodes(0).childNodes(j).childNodes(1).innerHTML;
                            document.getElementById("AssociazioniScelteLatoClient").value += codLotto + "|" + assCodice + "|" + assDescrizione + "|";
                        }
                    }
                }
            }

            break;

        default:
            break;
    }
}

function ToolBarClick(ToolBar, button, evnt) {
    evnt.needPostBack = true;

    switch (button.Key) {
        case 'btn_Annulla':
            if (busy == "True")
                evnt.needPostBack = confirm("Le modifiche effettuate andranno perse. Continuare?");
            else
                evnt.needPostBack = false;
            break;

        case 'btn_Salva':
            if (busy != "True")
                evnt.needPostBack = false;
            break;

        case 'btn_Escludi':
            if (!isOneChecked()) {
                alert("Selezionare una vaccinazione prima di escluderla!");
                evnt.needPostBack = false;
            }
            break;

        case 'btn_Esegui':
            if (paz_deceduto == 'True') {
                alert('ATTENZIONE: il paziente selezionato risulta deceduto. Impossibile eseguire vaccinazioni.');
                evnt.needPostBack = false;
            }
            else {
                var msg = "";

                if (!isOneChecked()) {
                    msg += "Selezionare una vaccinazione prima di eseguirla!";
                }

                sceltaAccesso = checkRadioButtonSelected();
                if (!sceltaAccesso) {
                    if (msg != "") msg += "\n\n";
                    msg += "Selezionare la modalita\' di accesso del paziente!";
                }

                if (msg != "") {
                    alert(msg);
                    evnt.needPostBack = false;
                    return;
                }

                if (!controllaDataEsecuzione()) {
                    if (!confirm("ATTENZIONE: la data di esecuzione impostata e\' precedente rispetto a quella di convocazione.\n\nContinuare?")) {
                        CheckAll('dg_vacProg', false, 0, 0);
                        evnt.needPostBack = false;
                    }
                }
            }
            break;

        case 'btn_CalcolaConvocazioni':
            if (statusVaccinale == 9)
                if (!confirm('Il paziente attualmente selezionato e\' in status vaccinale "INADEMPIENTE COMPLETO": si e\' sicuri di voler creare una nuova convocazione?'))
                    evnt.needPostBack = false;
            break;

        default:
            evnt.needPostBack = true;
            break;
    }
}

// Controlla se è selezionata almeno una vaccinazione
function isOneChecked() {

    var n = document.getElementById("dg_vacProg").rows.length;

    for (i = 1; i < n; i++) {

        var chk = document.getElementById("dg_vacProg").rows[i].cells[idxVacProg.CheckBox].getElementsByTagName("input")[0];

        if (chk != null && chk.checked) return true;

    }

    return false;
}

var popUp = null;
//////////////////////
var modificato = false;
///////////////


function controllaPerDoseStessa(evt, bool) {
    elSrc = SourceElement(evt);
    riga = GetElementByTag(elSrc, 'TR', 1, 0, false)

    key = GetElementByTag(riga.cells[idxVacProg.Vaccinazione], 'SPAN', 2, 3, true).firstChild.nodeValue

    objDose = GetElementByTag(riga.cells[idxVacProg.Dose], 'INPUT', 1, 1, false);

    if (isNaN(objDose.value) || objDose.value < 1) {
        alert("Il campo 'Dose' deve essere un numero  maggiore di 0! Non e\' possibile aggiornare la riga.");
        objDose.focus();
        StopPreventDefault(evt);
        return false;
    }

    if (objDose.value.length > 2) {
        alert("Il campo 'Dose' puo\' contenere al massimo due caratteri! Non e\' possibile aggiornare la riga.");
        objDose.focus();
        StopPreventDefault(evt);
        return false;
    }

    if (typeof (arMaxDose) == "undefined") arMaxDose = null;

    if (bool) {
        if (!ControllaDoseStessa(key, objDose.value, arMaxDose, bool)) {
            objDose.focus();
            StopPreventDefault(evt);
            return false;
        }
    }
}

function AnnullaPopUp() {
    popUp.hide();
}

function ControllaDose(key, value, arMaxDose) {
    var find = false
    for (keys in arMaxDose) {
        if (keys == key) {
            find = true
            if (value - arMaxDose[key] > 1) {
                return warning_meno(value, arMaxDose[key] + 1)
            }
            else {
                if (value - arMaxDose[key] < 1) {
                    return warning_piu(arMaxDose[key])
                }
                else {
                    return true
                }
            }
        }
    }
    if (!find) {
        if (value > 1) {
            return warning_meno(value, 1)
        }
        else {
            return true
        }
    }
}
/////////////////////

function ControllaDoseStessa(key, value, arMaxDose, bool) {
    var find = false
    for (keys in arMaxDose) {
        if (keys == key) {
            find = true
            if (value - arMaxDose[key] > 1) {
                return warning_meno(value, arMaxDose[key] + 1)
            }
            else {
                if (value - arMaxDose[key] < 1) {
                    return warning_piuStessa(arMaxDose[key])
                }
                else {
                    return true
                }
            }
        }
    }
    if (!find) {
        if (value > 1) {

            return warning_meno(value, 1)
        }
        else {
            return true
        }
    }
}


/////
function warning_piuStessa(n1) {
    alert("Impossibile eseguire l'operazione.\nQuesta dose e' gia' stata programmata o eseguita!");
}

///////////////
function warning_meno(n1, n2) {
    if (n1 > n2) {
        str = "Attenzione! Le dosi n.";
        for (i = n2; i < n1; i++) {
            str += i;
            if (i != n1 - 1) { str += ","; }
        }
        str += " non sono state effettuate! Continuare?";
        return confirm(str);
    }
    else
        return true;
}

///////////////
function warning_piu(n1) {
    str = "Impossibile eseguire l'operazione.\nLa dose n." + n1 + " e' gia' stata programmata o eseguita!";
    alert(str);
}
///////////////
