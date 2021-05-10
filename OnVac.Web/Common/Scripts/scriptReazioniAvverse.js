// valoreDataVac è la data in formato stringa dd/MM/yyyy (l'ora non importa, se c'è viene scartata)
function ControlloDataReazione(idDataReaz, lblDataReaz, valoreDataVac)
{
    // Data reazione obbligatoria
    if (OnitDataPickGet(idDataReaz) == '')
    {
        alert("Il campo '" + lblDataReaz + "' è obbligatorio. Non è possibile eseguire l'operazione.");
        OnitDataPickFocus(idDataReaz, 1, false);
        return false;
    }
    
    var dReaz = new Array();
    dReaz = OnitDataPickGet(idDataReaz).split("/");
    var dataReaz = new Date(dReaz[2], dReaz[1] - 1, dReaz[0]);

    // Data reazione superiore a data vaccinazione
    if (valoreDataVac != null && valoreDataVac != '')
    {
        var idx = valoreDataVac.indexOf(' ')
        if (idx != -1) valoreDataVac = valoreDataVac.substring(0, idx);

        var dVac = new Array();
        dVac = valoreDataVac.split("/");
        var dataVac = new Date(dVac[2], dVac[1] - 1, dVac[0]);

        if (dataReaz < dataVac) {
            alert("Il campo '" + lblDataReaz + "' contiene un data antecedente a quella della relativa vaccinazione. Non è possibile completare l'operazione.");
            OnitDataPickFocus(idDataReaz, 1, false);
            return false;
        }
    }
    
    // Data reazione non futura
    if (dataReaz > new Date())
    {
        alert("Il campo '" + lblDataReaz + "' contiene una data futura. Non è possibile completare l'operazione.");
        OnitDataPickFocus(idDataReaz, 1, false);
        return false;
    }

    return true;
}

function ControlloTipoReazione(idTipoReaz1, lblTipoReaz1, idTipoReaz2, lblTipoReaz2, idTipoReaz3, lblTipoReaz3, idTipoReazAltro, lblTipoReazAltro, isAltroObbligatorio, maxLengthAltro)
{
    // Obbligatorietà Tipo Reazione 1 
    var tipoReaz1 = document.getElementById(idTipoReaz1);
    var descReaz1 = tipoReaz1.value;
    var codReaz1 = tipoReaz1.nextSibling.value;

    if (tipoReaz1 == null || codReaz1 == "" || descReaz1 == "")
    {
        alert("Il campo '" + lblTipoReaz1 + "' è obbligatorio. Non è possibile eseguire l'operazione.");
        if (tipoReaz1 != null) tipoReaz1.focus();
        return false;
    }

    // Controllo campo Altra Reazione 
    if (isAltroObbligatorio)
    {
        var tipoReazAltro = document.getElementById(idTipoReazAltro);
        if (tipoReazAltro == null || tipoReazAltro.value == "")
        {
            alert("Il campo '" + lblTipoReazAltro + "' è obbligatorio. Non è possibile eseguire l'operazione.");
            if (tipoReazAltro != null) tipoReazAltro.focus();
            return false;
        }
    }

    // Controllo lunghezza campo Altra Reazione
    if (!ControlloLunghezzaCampo(idTipoReazAltro, maxLengthAltro, lblTipoReazAltro))
    {
        return false;
    }
    
    // Controllo Tipo Reazione 2
    var tipoReaz2 = document.getElementById(idTipoReaz2);
    var descReaz2 = tipoReaz2.value;
    var codReaz2 = tipoReaz2.nextSibling.value;
    		        
    if ((codReaz2 == "" && descReaz2 != "") || (codReaz2 != "" && descReaz2 == ""))
    {
        alert("Il campo '" + lblTipoReaz2 + "' non è impostato correttamente. Non è possibile eseguire l'operazione.");
        tipoReaz2.focus();
        return false;
    }

    // Controllo Tipo Reazione 3
    var tipoReaz3 = document.getElementById(idTipoReaz3);
    var descReaz3 = tipoReaz3.value;
    var codReaz3 = tipoReaz3.nextSibling.value;
	
    if ((codReaz3 == "" && descReaz3 != "") || (codReaz3 != "" && descReaz3 == ""))
    {
        alert("Il campo '" + lblTipoReaz3 + "' non è impostato correttamente. Non è possibile eseguire l'operazione.");
        tipoReaz3.focus();
        return false;
    }

    // Valorizzato campo 3 senza prima valorizzare il 2
    if ((codReaz2 == "" && descReaz2 == "") && (codReaz3 != "" && descReaz3 != ""))
    {
        alert("Non è possibile impostare il campo '" + lblTipoReaz3 + "' senza aver prima impostato il campo '" + lblTipoReaz2 + "'.");
        tipoReaz2.value = descReaz3;
        tipoReaz2.nextSibling.value = codReaz3;
        tipoReaz3.value = "";
        tipoReaz3.nextSibling.value = "";
        return false;
    }

    // Stessa reazione in più di un campo
    if (codReaz1 == codReaz2 || codReaz1 == codReaz3 || (codReaz2 != "" && codReaz2 == codReaz3))
    {
        alert("Impossibile impostare due reazioni identiche per la stessa vaccinazione");
        if (codReaz1 == codReaz2)
        {
            tipoReaz2.value = "";
            tipoReaz2.nextSibling.value = ""; 
        }
        else
        {
            tipoReaz3.value = "";
            tipoReaz3.nextSibling.value = ""; 
        }
        return false;
    }

    return true;
}

function ControlloLunghezzaCampo(idCampo, maxLengthCampo, lblCampo)
{
    var campo = document.getElementById(idCampo);

    if (campo != null && campo.value.length > maxLengthCampo)
    {
        alert("Il campo:\n'" + lblCampo + "'\nnon può contenere più di " + maxLengthCampo + " caratteri. Non è possibile completare l'operazione.");
        campo.focus();
        return false;
    }

    return true;
}

function ControlloCampoObbligatorio(idCampo, lblCampo)
{			
    var field = document.getElementById(idCampo);

    if (field != null && field.value == "")
    {
        alert("Il campo '" + lblCampo + "' è obbligatorio. Non è possibile completare l'operazione");
        field.focus();
        return false;
    }

    return true;
}

function ControlloDataEsito(idDataEsito, idCheckRisoluzione, idCheckDecesso) {

    var isRisoluzione = false;
    var isDecesso = false;

    var chkRisoluzione = document.getElementById(idCheckRisoluzione);
    if (chkRisoluzione != null) isRisoluzione = chkRisoluzione.checked;
    
    var chkDecesso = document.getElementById(idCheckDecesso);
    if (chkDecesso != null) isDecesso = chkDecesso.checked;
    
    if ((isRisoluzione || isDecesso) && OnitDataPickGet(idDataEsito) == "") {
        alert("La data dell'esito è obbligatoria. Non è possibile eseguire l'operazione.");
        OnitDataPickFocus(idDataEsito, 1, false);
        return false;
    }

    return true;
}