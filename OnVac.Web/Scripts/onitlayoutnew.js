
/*Setta il titolo (nome e stile)*/
function OnitLayoutSetTitle(titolo, cssClass) {
    var Topf = window.parent.frames["TopFrame"]
    if (Topf == null) { return }
    Topf.document.getElementById("lblNomeApp").innerHTML = titolo
    Topf.document.getElementById("lblNomeApp").className = cssClass
}
/* fine  OnitLayoutSetTitle*/

/*Imposta il submit del form con l'OnitSubmit*/
function OnitLayoutSetSubmit() {
	/*document.forms[0].action="javascript:OnitLayoutSubmit()"
   	window.status=window.defaultStatus*/
}
/* fine OnitLayoutSetOnitSubmit*/

/* Gestisce il submit della pagina nascondendone il contenuto e scrivendo nella barra di stato un msg personalizzato*/
function OnitLayoutSubmit() {
    document.body.style.visibility = "hidden"
    window.status = "Elaborazione in corso..."
    document.forms[0].action = document.location.pathname
    document.forms[0].submit()
}
/*fine OnitSubmit*/



/* Disabilita-Abilita il TopFRame e/o il LeftFrame*/
function OnitLayoutStatoMenu(Disabilita, Cosa) {
    var Leftf, Topf;
    var e;
    Leftf = window.parent.frames["LeftFrame"];
    Topf = window.parent.frames["TopFrame"];
    if (Disabilita) {
        if (Cosa == null || Cosa == 1) {
            Topf.document.getElementById("copri").style.visibility = "visible";
        }
        if (Cosa == null || Cosa == 0) {
            //Leftf.document.getElementById("copri").style.visibility="visible";
            Leftf.iglbar_getListbarById("UltraWebListbar").setEnabled(false);

        }
    }
    else {
        if (Cosa == null || Cosa == 1) {
            Topf.document.getElementById("copri").style.visibility = "hidden";
        }
        if (Cosa == null || Cosa == 0) {
            //Leftf.document.getElementById("copri").style.visibility="hidden";
            Leftf.iglbar_getListbarById("UltraWebListbar").setEnabled(true);
        }

    }
}
/* fine OnitLayoutStatoMenu*/

/* Modifica i sotto-menu del LeftFrame*/
function SetTextLeftMenu(find, text, important, disabled) {
    var lf;
    lf = window.parent.frames["LeftFrame"];
    if (lf == null) { return; }

    var lgth = lf.document.getElementById("CSPBIcons0");
    if (lgth == null) { return; }

    for (i = 0; i < lf.document.getElementById("CSPBIcons0").firstChild.rows.length - 1; i++) {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        //Marco 26-01-03 ho sostituito la riga sotto commentata con il seguente blocco di codice     //
        //per rendere il tutto compatibile con Mozilla                                               //
        ///////////////////////////////////////////////////////////////////////////////////////////////

        //	if (lf.document.getElementById("CSPBIcons0").firstChild.rows[i].innerText.substring(0,find.length)==find) {break;}

        elAnchor = GetElementByTag(lf.document.getElementById("CSPBIcons0").firstChild.rows[i], 'A', 1, 1, false);
        if (elAnchor != null) {
            var testo = elAnchor.firstChild.nodeValue;
            if (testo != null) {
                var subStr = testo.substring(0, find.length);
                if (subStr == find)
                    break;
            }
        }
        //fine Marco

    }

    switch (important) {
        case 1:
            lf.document.getElementById("CSPBIcons0").firstChild.rows[i].cells[0].firstChild.className = "Menu_Select";
            break;
        case 2:
            lf.document.getElementById("CSPBIcons0").firstChild.rows[i].cells[0].firstChild.className = "Menu_Normal";
            break;
        case 3:
            lf.document.getElementById("CSPBIcons0").firstChild.rows[i].cells[0].firstChild.className = "Menu_Info";
            break;
    }

    lf.document.getElementById("CSPBIcons0").firstChild.rows[i].cells[0].firstChild.innerHTML = text
    if (disabled) {
        lf.document.getElementById("CSPBIcons0").firstChild.rows[i].cells[0].firstChild.href = "javascript:function (){return false}"
    }

}
/* fine SetTextLeftMenu*/


