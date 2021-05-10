function InizializzaToolBar(t) 
{
    t.PostBackButton = true;
}

function controlloRedirectToPaziente(pageInEdit, bloccoSuRiga) 
{
    if (pageInEdit) 
    {
        var msgUscitaEdit;

        if (bloccoSuRiga) 
        {
            msgUscitaEdit = 'Per uscire dalla modalità di modifica è necessario premere Conferma o Annulla sulla riga che si sta modificando.';
        }
        else 
        {
            msgUscitaEdit = 'Per uscire dalla modalità di modifica è necessario premere Salva o Annulla.';
        }
        alert('Impossibile visualizzare i dati anagrafici del paziente mentre la pagina è in modifica.\n' + msgUscitaEdit);
        return false;
    }

    return confirm('Visualizzare i dati anagrafici del paziente?');
}

function ImpostaImmagineOrdinamento(imgId, imgUrl) {
    var img = document.getElementById(imgId);
    if (img != null) {
        img.style.display = 'inline';
        img.src = imgUrl;
    }
}