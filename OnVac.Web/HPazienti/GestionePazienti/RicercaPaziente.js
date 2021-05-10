var enableNextPostBack = true;

function AvviaRicerca(evt, obj) {
    if (evt.keyCode == 13) {
        /* 
            idFmNascita e idFmResidenza sono dichiarati lato server. Se la modale di residenza è nascosta, idFmResidenza == ''.
        */
        var isValidFmNascita = isValidFinestraModale(idFmNascita, false);
        var isValidFmResidenza = true;

        if (idFmResidenza != '') isValidFmResidenza = isValidFinestraModale(idFmResidenza, false);

        if (batchValidator.exec() && isValidFmNascita && isValidFmResidenza) {
            __doPostBack("cerca", "");
        }

        var fm = null;

        if (!isValidFmNascita) {
            fm = document.getElementById(idFmNascita);
        } else if (!isValidFmResidenza) {
            fm = document.getElementById(idFmResidenza);
        }

        if (fm != null) fm.blur();
    }
}

function RefreshFromPopup() {
    __doPostBack("RefreshFromPopup", "");
}

function toolbar_click(sender, args) {
    if (!e) var e = window.event;

    var button = args.get_item();
    switch (button.get_value()) {

        case 'btnPulisci':
            pulisciCampi();
            args.set_cancel(true);
            break;

        case 'btnSeleziona':
            // TODO [RicPaz]: funzione lato client per determinare se il datagrid è selezionato???
            //var grid = get_OnitGrid('<%= dgrPazienti.ClientId %>');

            //if (grid != null && grid.SelectedIndex >= 0) {

            //    resetCheck("dgrPazienti");

            //    // Questo serve per abilitare il New solo dopo aver effettuato una ricerca
            //    enableNextPostBack = false;

            //} else {
            //    alert("Effettuare una ricerca e selezionare un paziente dall'elenco per continuare.");
            //    args.set_cancel(true);
            //}
            break;

        case 'btnFind':
            // N.B. : idFmNascita e idFmResidenza sono dichiarati lato server. Se la modale di residenza è nascosta, idFmResidenza == ''
            var isValidFmNascita = isValidFinestraModale(idFmNascita, false);
            var isValidFmResidenza = true;

            if (idFmResidenza != '') isValidFmResidenza = isValidFinestraModale(idFmResidenza, false);

            if (!isValidFmNascita || !isValidFmResidenza) {

                var fm = null;
                if (!isValidFmNascita) {
                    fm = document.getElementById(idFmNascita);
                } else if (!isValidFmResidenza) {
                    fm = document.getElementById(idFmResidenza);
                }

                if (fm != null) fm.blur();
                args.set_cancel(true);
            }
            break;

        case 'btnNew':
            enableNextPostBack = false;
            break;

        case 'btnConsenso':
            // TODO [PARMA-APC]: funzione lato client per determinare se il datagrid è selezionato???
            //var grid = get_OnitGrid('<%= dgrPazienti.ClientId %>');
            //if (grid != null && grid.SelectedIndex < 0) {
            //    args.set_cancel(true);
            //    alert("Effettuare una ricerca e selezionare un paziente dall'elenco per continuare.");
            //}
            break;
    }
}

function pulisciCampi() {
    var oTable, oList, i;

    oTable = document.getElementById("tableFiltri");
    oList = oTable.getElementsByTagName("input");
    for (i = 0; i < oList.length; i++) {
        oList[i].value = ""
    }
    oList = oTable.getElementsByTagName("select");
    for (i = 0; i < oList.length; i++) {
        oList[i].selectedIndex = 0;
    }
}

// toglie tutti i segni di spunta prima di clickare "Seleziona"
function resetCheck(datagridId) {

    var grid = document.getElementById(datagridId);

    if (grid != null) {
        for (i = 0; i < grid.rows.length; i++) {
            var arrayTags = grid.rows[i].cells[columnIndexCheck].getElementsByTagName('INPUT');
            if (arrayTags != null && arrayTags.length > 0) {
                arrayTags[0].checked = checkAll;
            }
        }
    }
}


