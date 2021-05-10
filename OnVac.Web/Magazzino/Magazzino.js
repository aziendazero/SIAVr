
function InizializzaToolBar(t)
{
	t.PostBackButton=false;
}

function AbilitaTrasferimento(evnt,el)
{
	var cella = el.parentNode.cellIndex;
	elDIV=GetElementByTag(el.parentNode.parentNode.cells[cella+1],'DIV',1,1,false);
	if (el.selectedIndex==3)
		elDIV.style.display='inline';
	else
		elDIV.style.display='none';
}

//Marco 11/12/03
// Scopo: trasforma il contenuto di una text box il lettere TUTTE maiuscole
// Parametri: obj= oggetto di tipo TextBox
function stringToUpperCase(obj){
	obj.value=obj.value.toUpperCase();
}

function mostraAttesa()
{
//	showFm('fmAttesa',true,250);
}

function ControllaLotto(evnt,codLotto,descLotto,NC,dataPrep,dataScadenza,fornitore,note,dosiScatola,qtaIniz,qtaMin)
{
	if(document.getElementById(codLotto).value=="")
	{
		alert("Non e' possibile salvare le modifiche apportate. Il dato 'Lotto Codice' e' obbligatorio.");
		document.getElementById(codLotto).focus();
		evnt.needPostBack=false;
		return false;
	}
	if(document.getElementById(codLotto).value.length>15)
	{
		alert("Non e' possibile salvare le modifiche apportate. Il campo 'Lotto Codice' non puo' contenere piu' di 15 caratteri.");
		document.getElementById(codLotto).focus();
		evnt.needPostBack=false;
		return false;
	}
		
	//controllo su entrambi i campi del nome commerciale (modifica 13/01/2005)	
	var codiceNC = document.getElementById(NC).nextSibling.value;
	var descrizioneNC = document.getElementById(NC).value;
	
	if(codiceNC=="")
	{
		alert("Non e' possibile salvare le modifiche apportate. Il campo codice 'Nome Commerciale' e' obbligatorio.");
		document.getElementById(NC).focus();
		evnt.needPostBack=false;
		return false;
	}
	
	if(descrizioneNC=="")
	{
		alert("Non e' possibile salvare le modifiche apportate. Il campo descrizione 'Nome Commerciale' e' obbligatorio.");
		document.getElementById(NC).focus();
		evnt.needPostBack=false;
		return false;
	}
	
	if(document.getElementById(descLotto).value=="")
	{
		alert("Non e' possibile salvare le modifiche apportate. Il dato 'Lotto Descrizione' e' obbligatorio.");
		document.getElementById(descLotto).focus();
		evnt.needPostBack=false;
		return false;
	}
	
	if(document.getElementById(descLotto).value.length>45)
	{
		alert("Non e' possibile salvare le modifiche apportate. Il campo 'Lotto Descrizione' non puo' contenere piu' di 45 caratteri.");
		document.getElementById(descLotto).focus();
		evnt.needPostBack=false;
		return false;
	}
	
	if(OnitDataPickGet(dataPrep)!='')
	{
		arTemp=new Array();
		arTemp=OnitDataPickGet(dataPrep).split("/");
		if(new Date(arTemp[2],arTemp[1]-1,arTemp[0],0,0,0)>new Date())
		{
			alert("Non e' possibile inserire una data di preparazione futura!");
			OnitDataPickFocus(dataPrep,1,true)
			evnt.needPostBack=false;
			return false;
		}
	}
	if(OnitDataPickGet(dataScadenza)=='')
	{
		alert("Non e' possibile salvare le modifiche apportate. La Data di Scadenza e' obbligatoria.");
		OnitDataPickFocus(dataScadenza,1,true)
		evnt.needPostBack=false;
		return false;
	}
	else
	{ 
		arTemp=new Array();
		arTemp=OnitDataPickGet(dataScadenza).split("/");
		if(new Date(arTemp[2],arTemp[1]-1,arTemp[0],0,0,0)<=new Date())
		{
			if(!confirm("Attenzione !!\nIl lotto e' gia' scaduto. Continuare?"))
			{
				OnitDataPickFocus(dataScadenza,1,true)
				evnt.needPostBack=false;
				return false;
			}
		}
	}
	
	
	if(document.getElementById(fornitore).value.length>45)
	{
		alert("Non e' possibile salvare le modifiche apportate. Il campo 'Fornitore' non puo' contenere piu' di 45 caratteri.");
		document.getElementById(fornitore).focus();
		evnt.needPostBack=false;
		return false;
	}
	if(document.getElementById(note).value.length>45)
	{
		alert("Non e' possibile salvare le modifiche apportate. Il campo 'Note' non puo' contenere piu' di 45 caratteri.");
		document.getElementById(note).focus();
		evnt.needPostBack=false;
		return false;
	}
	if(document.getElementById(dosiScatola).value=="")
	{
		document.getElementById(dosiScatola).value="1";
	}
	else
	{		
			if(isNaN(document.getElementById(dosiScatola).value) || parseInt(document.getElementById(dosiScatola).value)!=document.getElementById(dosiScatola).value ||  parseInt(document.getElementById(dosiScatola).value) < 1)
			{
				alert("Non e' possibile salvare le modifiche apportate.\nIl campo 'Dosi Scatola' deve essere un numero positivo maggiore di zero.");
				document.getElementById(dosiScatola).focus();
				evnt.needPostBack=false;
				return false;
			}
			if(document.getElementById(dosiScatola).value.length>3)
			{
				alert("Non e' possibile salvare le modifiche apportate. Il campo 'Dosi Scatola' non puo' contenere piu' di 3 caratteri.");
				document.getElementById(dosiScatola).focus();
				evnt.needPostBack=false;
				return false;
			}
	}
	if(document.getElementById(qtaIniz).value=="")
	{	
		document.getElementById(qtaIniz).value="0";
	}
	else
	{
			if(isNaN(document.getElementById(qtaIniz).value) || parseInt(document.getElementById(qtaIniz).value)!=document.getElementById(qtaIniz).value ||  parseInt(document.getElementById(qtaIniz).value) < 0)
			{
				alert("Non e' possibile salvare le modifiche apportate.\nIl campo 'Quantita' Iniziale' non puo' essere un numero negativo.");
				document.getElementById(qtaIniz).focus();
				evnt.needPostBack=false;
				return false;
			}
			if(document.getElementById(qtaIniz).value.length>7)
			{
				alert("Non e' possibile salvare le modifiche apportate. Il campo 'Quantita' Iniziale' non può contenere piu' di 7 caratteri.");
				document.getElementById(qtaIniz).focus();
				evnt.needPostBack=false;
				return false;
			}
	}
	if(document.getElementById(qtaMin).value=="")
	{
		document.getElementById(qtaMin).value="0";
	}
	else
	{
		if(isNaN(document.getElementById(qtaMin).value)|| parseInt(document.getElementById(qtaMin).value)!=document.getElementById(qtaMin).value ||  parseInt(document.getElementById(qtaMin).value) < 0)
		{
			alert("Non e' possibile aggiornare il lotto. Il campo 'Quantita' Minima' deve essere un numero positivo.");
			document.getElementById(qtaMin).focus();
			evnt.needPostBack=false;
			return false;
		}
		if(document.getElementById(qtaMin).value.length>5)
		{
			alert("Non e' possibile aggiornare il lotto. Il campo 'Quantita' Minima' non puo' contenere piu' di 5 caratteri.");
			document.getElementById(qtaMin).focus();
			evnt.needPostBack=false;
		}
	}	
	return true;
}


function ControlloDatiLotto(idCodLotto, idDescLotto, idNomeCommerciale, idDataPreparazione, idDataScadenza, idDosiScatola, idQtaIniziale, idQtaMinima, idFornitore, idNote) 
{
    // --- Controllo campi obbligatori ---

    var messageCampiObbligatori = "";

    // Controllo Codice Lotto
    var fieldCodLotto = document.getElementById(idCodLotto);

    if (fieldCodLotto != null && fieldCodLotto.value == "")
    {
        fieldCodLotto.focus();
        messageCampiObbligatori += "- Codice del lotto\n";
    }

    // Controllo Codice e Descrizione Nome Commerciale
    var fieldCodiceNC = document.getElementById(idNomeCommerciale).nextSibling;
    var fieldDescrizioneNC = document.getElementById(idNomeCommerciale);

    if ((fieldCodiceNC != null && fieldCodiceNC.value == "") || (fieldDescrizioneNC != null && fieldDescrizioneNC.value == ""))
    {
        fieldDescrizioneNC.focus();
        messageCampiObbligatori += "- Nome Commerciale\n";
    }

    // Controllo Descrizione Lotto
    var fieldDescrizioneLotto = document.getElementById(idDescLotto);

    if (fieldDescrizioneLotto != null && fieldDescrizioneLotto.value == "")
    {
        fieldDescrizioneLotto.focus();
        messageCampiObbligatori += "- Descrizione del lotto\n";
    }

    // Controllo Data Scadenza
    var dataScadenza = OnitDataPickGet(idDataScadenza);
    if (dataScadenza == "")
    {
        OnitDataPickFocus(idDataScadenza, 1, true);
        messageCampiObbligatori += "- Data di scadenza\n";
    }

    if (messageCampiObbligatori != "")
    {
        messageCampiObbligatori = "\nI seguenti campi obbligatori non sono stati valorizzati:\n" + messageCampiObbligatori;
    }

    // --- Controllo lunghezza campi
    var messageLunghezza = "";

    if (fieldCodLotto != null && fieldCodLotto.value.length > 15)
    {
        fieldCodLotto.focus();
        messageLunghezza += "- il Codice del lotto non deve superare i 15 caratteri\n";
    }

    if (fieldDescrizioneLotto != null && fieldDescrizioneLotto.value.length > 45)
    {
        messageLunghezza += "- la Descrizione del lotto non deve superare i 45 caratteri\n";
        fieldDescrizioneLotto.focus();
    }

    var fieldFornitore = document.getElementById(idFornitore);
    if (fieldFornitore != null && fieldFornitore.value.length > 45)
    {
        messageLunghezza += "- il campo 'Fornitore' non deve superare i 45 caratteri\n";
        fieldFornitore.focus();
    }

    var fieldNote = document.getElementById(idNote);
    if (fieldNote != null && fieldNote.value.length > 45)
    {
        messageLunghezza += "- il campo 'Note' non deve superare i 45 caratteri\n";
        fieldNote.focus();
    }

    var fieldDosiScatola = document.getElementById(idDosiScatola);
    if (fieldDosiScatola != null && fieldDosiScatola.value.length > 3) 
    {
        messageLunghezza += "- il campo 'Dosi Scatola' non deve superare i 3 caratteri\n";
        fieldDosiScatola.focus();
    }

    var fieldQuantitaIniziale = document.getElementById(idQtaIniziale);
    if (fieldQuantitaIniziale != null && fieldQuantitaIniziale.value.length > 7) 
    {
        messageLunghezza += "- La Quantita\' Iniziale non deve superare i 7 caratteri\n";
        fieldQuantitaIniziale.focus();
    }

    var fieldQuantitaMinima = document.getElementById(idQtaMinima);
    if (fieldQuantitaMinima != null && fieldQuantitaMinima.value.length > 5) 
    {
        messageLunghezza += "- la Quantita\' Minima non deve superare i 5 caratteri\n";
        fieldQuantitaMinima.focus();
    }

    if (messageLunghezza != "") 
    {
        messageLunghezza = "\nI valori contenuti nei seguenti campi superano la lunghezza massima prevista:\n" + messageLunghezza;
    }


    // --- Altri Controlli 

    var messageAltriControlli = "";

    // Controllo Data Preparazione
    var dataPreparazione = OnitDataPickGet(idDataPreparazione);

    if (dataPreparazione != "") 
    {
        arTemp = new Array();
        arTemp = dataPreparazione.split("/");
        if (new Date(arTemp[2], arTemp[1] - 1, arTemp[0], 0, 0, 0) > new Date())
        {
            OnitDataPickFocus(idDataPreparazione, 1, true);
            messageAltriControlli += "- non e' possibile inserire una Data di Preparazione futura\n";
        }
    }

    // Controllo Dosi Scatola
    if (fieldDosiScatola != null)
    {
        if (fieldDosiScatola.value == "") 
        {
            fieldDosiScatola.value = "1";
        }
        else
        {
            if (isNaN(fieldDosiScatola.value) || parseInt(fieldDosiScatola.value) != fieldDosiScatola.value || parseInt(fieldDosiScatola.value) < 1)
            {
                fieldDosiScatola.focus();
                messageAltriControlli += "- il Numero di Dosi deve essere un valore numerico maggiore di zero\n";
            }
        }
    }

    // Controllo Quantità Iniziale
    if (fieldQuantitaIniziale != null)
    {
        if (fieldQuantitaIniziale.value == "")
        {
            fieldQuantitaIniziale.value = "0";
        }
        else
        {
            if (isNaN(fieldQuantitaIniziale.value) || parseInt(fieldQuantitaIniziale.value) != fieldQuantitaIniziale.value || parseInt(fieldQuantitaIniziale.value) < 0) 
            {
                fieldQuantitaIniziale.focus();
                messageAltriControlli += "- la Quantita\' Iniziale' deve essere un valore numerico non negativo\n";
            }
        }
    }

    // Controllo Quantità Minima
    if (fieldQuantitaMinima != null)
    {
        if (fieldQuantitaMinima.value == "")
        {
            fieldQuantitaMinima.value = "0";
        }
        else 
        {
            if (isNaN(fieldQuantitaMinima.value) || parseInt(fieldQuantitaMinima.value) != fieldQuantitaMinima.value || parseInt(fieldQuantitaMinima.value) < 0) 
            {
                fieldQuantitaMinima.focus();
                messageAltriControlli += "- la Quantita\' Minima deve essere un valore numerico non negativo\n";
            }
        }
    }

    if (messageAltriControlli != "") 
    {
        messageAltriControlli = "\nSono stati rilevati i seguenti errori:\n" + messageAltriControlli;
    }

    return messageCampiObbligatori + messageLunghezza + messageAltriControlli;
}

function ControlloScadenzaLotto(idDataScadenza) 
{
    var dataScadenza = OnitDataPickGet(idDataScadenza);

    if (dataScadenza != "") 
    {
        arTemp = new Array();
        arTemp = dataScadenza.split("/");
        if (new Date(arTemp[2], arTemp[1] - 1, arTemp[0], 0, 0, 0) <= new Date()) 
        {
            if (!confirm("Attenzione: il lotto e' gia' scaduto.\nContinuare?")) 
            {
                OnitDataPickFocus(idDataScadenza, 1, true)
                return false;
            }
        }
    }

    return true;
}