var askConfirmReazioniAvverse = false;

function showAllDettagliVac(img) 
{
    var showDettaglio = (img.src.indexOf('piu.gif') != -1);

    // immagine dell'header
    toggleImage(img, showDettaglio);

    if (showDettaglio) 
    {
        img.alt = 'Nascondi dettagli di tutti i conflitti';
    }
    else 
    {
        img.alt = 'Mostra dettagli di tutti i conflitti';
    }

    var divs = document.getElementsByName('divDettaglio');

    if (divs != null) 
    {
        var imgs = document.getElementsByName('imgDettaglio');

        for (var i = 0; i < divs.length; i++) 
        {
            // immagine della riga
            toggleImage(imgs[i], showDettaglio);

            // div della riga
            showDivDettaglio(divs[i], showDettaglio)
        }
    }

    return showDettaglio;
}

function showDettaglioVac(img, dgrId) 
{
    var showDettaglio = (img.src.indexOf('piu.gif') != -1);

    toggleImage(img, showDettaglio);

    showDivDettaglio(document.getElementById(dgrId).parentNode, showDettaglio);
}

function toggleImage(img, showDettaglio)
{
    var showDetails = false;

    // Cambio l'immagine del pulsante
    if (showDettaglio)
    {
        img.src = imagesPath + 'meno.gif';
        img.alt = 'Nascondi dettagli conflitto';
    }
    else
    {
        img.src = imagesPath + 'piu.gif';
        img.alt = 'Mostra dettagli conflitto';
    }

    return showDettaglio;
}

function showDivDettaglio(div, showDettaglio)
{
    if (div == null) return false;

    // Mostro o nascondo il dettaglio delle vaccinazioni
    if (showDettaglio)
    {
        div.style.display = 'block';
    }
    else
    {
        div.style.display = 'none';
    }

    return showDettaglio;
}

function flagVisibilitaChanged(clientIdHdIndexFlagVisibilita, rowSelectedIndex)
{
    var hdIndexFlagVisibilita = document.getElementById(clientIdHdIndexFlagVisibilita);

    if (hdIndexFlagVisibilita != null) hdIndexFlagVisibilita.value = rowSelectedIndex;

    return true;
}

function selezionaTutti(chk, columnIndexCheck) 
{    
    var checkAll = true;

    if (chk.checked)
    {
        chk.checked = true;
        checkAll = true;
    }
    else
    {
        chk.checked = false;
        checkAll = false;
    }

    var dgrConflitti = document.getElementById("dgrConflitti");

    for (i = 0; i < dgrConflitti.rows.length; i++)
    {
        var arrayTags = dgrConflitti.rows[i].cells[columnIndexCheck].getElementsByTagName('INPUT');
        if (arrayTags != null && arrayTags.length > 0)
        {
            arrayTags[0].checked = checkAll;
        }
    }

    return true;
}

function InizializzaToolBar(t) 
{
    t.PostBackButton = true;
}

function ToolBarClick(t, button, evnt) 
{
    evnt.needPostBack = true;

    switch (button.Key) 
    {
        case 'btnRisolviConflitti':

            if (controllaCheckDatagrid()) 
            {
                evnt.needPostBack = true;

                if (askConfirmReazioniAvverse && !confirm("ATTENZIONE: procedendo con la risoluzione, eventuali reazioni avverse relative a vaccinazioni in conflitto (se non selezionate per la centralizzazione) potrebbero essere perse. Continuare?"))
                    evnt.needPostBack = false;
            }
            else 
            {
                alert('Risoluzione conflitti non effettuata: nessun conflitto selezionato.');
                evnt.needPostBack = false;
            }

            break;
    }
}

function controllaCheckDatagrid() 
{
    var dgrConflitti = document.getElementById("dgrConflitti");

    for (i = 0; i < dgrConflitti.rows.length; i++) 
    {
        // N.B. : l'indice 0 corrisponde alla cella contenente il checkbox
        var arrayTags = dgrConflitti.rows[i].cells[0].getElementsByTagName('INPUT');

        if (arrayTags != null && arrayTags.length > 0) 
        {
            if (arrayTags[0].checked) return true;
        }
    }

    return false;
}
   