function parentTable_OnTextChanged(divId, codObj, descObj, PostBackOnSelect, query, label, addTutti, appID, aziCodice) {
    var cod = document.getElementById(codObj);
    var desc = document.getElementById(descObj);
    var panel = document.getElementById(divId);

    var str = AjaxList.getTable(cod.value, desc.value, codObj, descObj, divId, PostBackOnSelect, query, label, addTutti, appID, aziCodice);

    if (str != null && typeof (str) == "object") {
        panel.innerHTML = str.value;
    } else {
        alert("Error. [3001] " + str.request.responseText);
    }
}

function getElementsByClass(searchClass, node, tag) {
	var classElements = new Array();
	if ( node == null )
		node = document;
	if ( tag == null )
		tag = '*';	
	var els = node.getElementsByTagName(tag);
	var elsLen = els.length;
	var pattern = new RegExp('(^|\\\\s)'+searchClass+'(\\\\s|$)');
	for (i = 0, j = 0; i < elsLen; i++) {
		if ( pattern.test(els[i].className) ) {
			classElements[j] = els[i];
			j++;
		}
	}
	return classElements;
}

var oldSel;
var tbl;

function rowSelected(row, node, codClientId, descClientId) {

    sel = getElementsByClass('selected', node, 'tr');
    var selLen = sel.length;
    for (i = 0; i < selLen; i++) {
        if ('className' in sel[i]) {
            sel[i].className = sel[i].oldClass;
        }
    }
    row.oldClass = row.className;
    row.className = 'selected';

    var codice = row.cells[row.cells.length - 1].innerHTML;
    var descrizione = row.cells[row.cells.length - 2].innerHTML;

    var o = document.getElementById(codClientId);
    if (o != null) o.value = codice;

    o = document.getElementById(descClientId);
    if (o != null) o.value = descrizione;
}

function pulisciFiltri(row, node, codClientId, descClientId) {

    var o = document.getElementById(codClientId);
    if (o != null) o.value = '';

    o = document.getElementById(descClientId);
    if (o != null) o.value = '';
}