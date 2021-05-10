// Indici delle colonne della tabella che viene ripetuta in ogni item del datalist
var idxTableOsservazioni = new function () {
    this.Codice = 0;
    this.Descrizione = 1;
    this.Risposta = 2;
}

var disEls;

function getDisabledEls() {

    disEls = new Object();

    var inc = 0;
    var cmbs = document.getElementById('Questionario_dlsQuestionarioCompleto').getElementsByTagName('select');
    var dis;

    for (i = 0; i < cmbs.length; i++) {

        var str = cmbs[i].parentNode.getElementsByTagName('input')[0].value;

        if (cmbs[i].selectedIndex > 0 && str != '') {
            var condizioni = str.split(':');

            for (t = 0; t < condizioni.length; t++) {
                var risposta = condizioni[t].split('|')[0];
                var oss = condizioni[t].split('|')[1];
                var disabilita = condizioni[t].split('|')[2];

                if (cmbs[i].value == risposta && disabilita == 'S') {
                    disEls[oss.toString()] = 1;
                    inc++;
                }
            }
        }
    }

    return inc;
}

function GestisciRisposta() {

    var num = getDisabledEls();

    ClearRisposte();

    var tableUp = document.getElementById('Questionario_dlsQuestionarioCompleto');

    for (r = 0; r < tableUp.rows.length; r++) {

        var tableDown = tableUp.rows[r].cells[0].getElementsByTagName('table')[1];

        for (l = 1; l < tableDown.rows.length; l++) {

            var codSpan = tableDown.rows[l].getElementsByTagName('table')[0].getElementsByTagName('span')[0];
            var cod = codSpan.innerText != null ? codSpan.innerText : codSpan.textContent;

            if (disEls[cod] != null) {

                // Riga da disabilitare
                var riga = tableDown.rows[l].getElementsByTagName('table')[0];

                var rigaDisabilitata = tableDown.rows[l].getElementsByTagName('input')[0];
                rigaDisabilitata.value = '1';

                riga.style.color = 'gray';
                riga.style.fontStyle = 'italic';
                riga.style.textDecoration = 'line-through';

                // Disabilitazione combobox
                var cmbAb = riga.rows[0].cells[idxTableOsservazioni.Risposta].getElementsByTagName('select');
                if (cmbAb.length > 0) {
                    cmbAb[0].style.display = 'none';
                    cmbAb[0].selectedIndex = 0;
                }

                // Disabilitazione textbox (input type="text")
                var txtAb = riga.rows[0].cells[idxTableOsservazioni.Risposta].getElementsByTagName('input');
                if (txtAb.length > 1) {
                    txtAb[1].value = '';
                }
                if (txtAb.length > 2) {
                    txtAb[2].style.display = 'none';
                    txtAb[2].value = '';
                }

                // Disabilitazione textarea
                var txaAb = riga.rows[0].cells[idxTableOsservazioni.Risposta].getElementsByTagName('textarea');
                if (txaAb.length > 0) {
                    txaAb[0].style.display = 'none';
                    txaAb[0].value = '';
                }
            }
        }
    }
}

function ClearRisposte() {

    var tableUp = document.getElementById('Questionario_dlsQuestionarioCompleto');

    for (r = 0; r < tableUp.rows.length; r++) {

        var tableDown = tableUp.rows[r].cells[0].getElementsByTagName('table')[1];

        for (l = 1; l < tableDown.rows.length; l++) {

            var spn = tableDown.rows[l].getElementsByTagName('TABLE')[0].getElementsByTagName('SPAN')[0];
            var cod = spn.innerText != null ? spn.innerText : spn.textContent;
            var riga = tableDown.rows[l].getElementsByTagName('TABLE')[0];

            var rigaDisabilitata = tableDown.rows[l].getElementsByTagName('INPUT')[0];
            rigaDisabilitata.value = '';

            riga.style.color = '';
            riga.style.fontStyle = '';
            riga.style.textDecoration = '';

            // Combobox
            var cmbAb = riga.rows[0].cells[idxTableOsservazioni.Risposta].getElementsByTagName('SELECT');
            if (cmbAb.length > 0)
                cmbAb[0].style.display = 'inline';

            // Textbox 
            var txtAb = riga.rows[0].cells[idxTableOsservazioni.Risposta].getElementsByTagName('INPUT');
            if (txtAb.length > 1)
                txtAb[1].value = cod;
            if (txtAb.length > 2)
                txtAb[2].style.display = 'inline';

            // Textarea
            var txaAb = riga.rows[0].cells[idxTableOsservazioni.Risposta].getElementsByTagName('textarea');
            if (txaAb.length > 0)
                txaAb[0].style.display = 'inline';
        }
    }
}

function controllaStato() {

    if (document.getElementById('Questionario_dlsQuestionarioCompleto') != null) {
        GestisciRisposta();
    }
}