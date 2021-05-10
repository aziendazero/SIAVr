/*

  Per utilizzare NavigationUtility e' necessario importare anche Utility.js

*/
//Disabilitazione pressione tasto destro del mouse

function clickIE() {
    if (document.all) {
        return false;
    }
}

function clickNS(e) {
    if (document.layers || (document.getElementById && !document.all)) {
        if (e.which == 2 || e.which == 3) {
            return false;
        }
    }
}

if (document.layers) {
    document.captureEvents(Event.MOUSEDOWN);
    document.onmousedown = clickNS;
}
else {
    document.onmouseup = clickNS;
    document.oncontextmenu = clickIE;
}

document.oncontextmenu = new Function("return false")

//Disabilitazione pressione backspace, alt-indietro
if (typeof window.event == 'undefined') {
    // Codice per i browser "standard" (notare target)
    window.addEventListener('keypress', function(e) {
        var test_var = e.target.nodeName.toUpperCase();
        if (e.target.type) var test_type = e.target.type.toUpperCase();
        if ((test_var == 'INPUT' && test_type == 'TEXT') || test_var == 'TEXTAREA') {
            return e.keyCode;
        } else if (e.keyCode == 8 || e.keyCode == 37) {
            e.preventDefault();
        }
    }
          , true);
} else {
    // Codice per Internet Explorer (notare l'srcElement)
    addProcToEvent('window.document.onkeydown', 'disabledKeysOnKeyDown()', 'disabledKeysOnKeyDown');
}


function disabledKeysOnKeyDown() {
    var test_var = event.srcElement.tagName.toUpperCase();
    if (event.srcElement.type) var test_type = event.srcElement.type.toUpperCase();
    if ((test_var == 'INPUT' && test_type == 'TEXT') || test_var == 'TEXTAREA') {
        return event.keyCode;
    } else if (event.keyCode == 8 || event.keyCode == 37) {
        event.returnValue = false;
    }
}
