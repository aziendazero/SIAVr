//
//	Codice javascript relativo alla pagina AssociaCicli.aspx
//


function InizializzaToolBar(ToolBar)
{
	ToolBar.PostBackButton=false;
}
			
function ToolBarClick(ToolBar,button,evnt){
	evnt.needPostBack=true;
	switch(button.Key)
	{
		case 'btnCerca':
							
			// Controllo aggiornamento finestre modali
			if (!isValidFinestraModale("omlConsultorio",false))
			{
			    alert("Il campo 'Centro Vaccinale' non era aggiornato.\nRiprovare.");
				evnt.needPostBack = false;
				return;
			}
			
			if (!isValidFinestraModale("omlCategorieRischio",false))
			{
				alert("Il campo 'Categoria a Rischio' non era aggiornato.\nRiprovare.");
				evnt.needPostBack = false;
				return;
			}

			if (!isValidFinestraModale("omlMalattia",false))
			{
				alert("Il campo 'Malattia' non era aggiornato.\nRiprovare.");
				evnt.needPostBack = false;
				return;
			}
			
			if (document.getElementById('omlConsultorio').value == '') 
			{
			    alert("Per effettuare la ricerca deve essere impostato il campo 'Centro Vaccinale', e almeno uno tra i seguenti campi:\n- la data di nascita;\n- la categoria a rischio;\n- la malattia.\nRicerca non effettuata.");
				document.getElementById('omlConsultorio').focus();
				evnt.needPostBack = false;
				return;
			}

			// Se, oltre al consultorio non sono presenti le date di nascita, la categoria a richio o la malattia, non effettua la ricerca
			if ( OnitDataPickGet('dpkDataNascitaDa') == '' && OnitDataPickGet('dpkDataNascitaA') == '' && document.getElementById('omlCategorieRischio').value == '' && document.getElementById('omlMalattia').value == '' )
			{
			    alert("Per effettuare la ricerca deve essere impostato il campo 'Centro Vaccinale', e almeno uno tra i seguenti campi:\n- la data di nascita;\n- la categoria a rischio;\n- la malattia.\nRicerca non effettuata.");
				evnt.needPostBack = false;
				return;
			}

			// Controllo date di nascita
			var strDataNascDa = OnitDataPickGet('dpkDataNascitaDa');
			var strDataNascA = OnitDataPickGet('dpkDataNascitaA');
			if ( strDataNascDa != '' && strDataNascA != '' )
			{
				if ( confrontaDate(strDataNascDa,strDataNascA) == -1 )
				{
					alert("Attenzione: la data di nascita iniziale deve essere inferiore a quella finale.");
					evnt.needPostBack=false;
					return;
				}
			}

			break;
			
		case 'btnAssocia':
			// Controllo aggiornamento finestra modale Ciclo
			if (!isValidFinestraModale("omlCicloCNV",false))
			{
				alert("Il campo 'Ciclo' non era aggiornato.\nRiprovare.");
				evnt.needPostBack=false;
				return;
			}
			
			// Controllo presenza Ciclo da associare
			if (document.getElementById('omlCicloCNV').value == '')
			{
				alert("Inserire un ciclo da associare!");
				evnt.needPostBack=false;
				return;
			}
			
			// Richiesta di conferma all'utente
			if (!confirm('ATTENZIONE: sta per essere effettuata la richiesta di associazione del ciclo ai pazienti selezionati. Continuare?'))
			{
				evnt.needPostBack=false;
				return;
			}
			
			break;
	}
}
		
function selezionaPazienti(chk){
	__doPostBack('selPazienti', chk);
}
			
function clickStatiAnagrafici() {
	document.getElementById("btnAggiungiStati").click();
	return true;		
}
			
function mouse(obj,tipo) {
	if (obj.src.indexOf('_dis')==-1) {
		var idx_file = obj.src.lastIndexOf('/')+1;
		var new_path = obj.src.substr(0,idx_file);
		var new_file = obj.src.substr(idx_file);
		var idx_ext = new_file.indexOf('.');
		var new_ext = new_file.substr(idx_ext);
		new_file = new_file.substr(0,idx_ext);
		
		if (tipo=='over') {
			obj.src=new_path+new_file+'_hov'+new_ext;
		} else {
			obj.src=new_path+new_file.substr(0,new_file.indexOf('_hov'))+new_ext;
		}
	}			
	return true;		
}
