var messaggioErrore
var modalName
var tbCodLotto
var chkPenna

function CheckLottoFromLCBBarCode(codLotto)
{

	var setUpperCase=!document.getElementById(chkPenna).checked;

	if (codLotto!="")
	{
	    var control = false;
		for (i=1;i<document.getElementById(dg_lotti).rows.length;i++)
		{
			var obj=document.getElementById(dg_lotti).rows[i].cells[2];
			var objSpan=GetElementByTag(obj,'SPAN',1,1,false);
			
			var codL=codLotto;
			var codCheck=objSpan.firstChild.nodeValue
			
			if (setUpperCase)
			{
				codL=codL.toUpperCase();
				codCheck=codCheck.toUpperCase();
			}
			
			if (codCheck==codL)
			{
				control = true;
				var obj=document.getElementById(dg_lotti).rows[i].cells[0];
				var objChkbox=GetElementByTag(obj,'INPUT',1,1,false);
				if (!objChkbox.checked)
				{
					objChkbox.defaultChecked=true;
					objChkbox.checked=true;
					if(!controllaVacAttivate(objChkbox))
					{
						CheckaLotto(objChkbox, false);
					}							
					else
					{
						arLottiPresenti=creaLottiPresenti();
						modificato=true;
					}
				}
				else
				{
					CheckaLotto(objChkbox, true);
					arLottiPresenti=creaLottiPresenti();
					modificato=true;
				}
				
				SetFocusTbCodLottoLCB()
				return;							
			}
		}
		
		//controllo sulla presenza del lotto (modifica 29/12/2004)
		if (!control)
			//se il codice lotto inserito non è nella lista deve 
			//presentare un messaggio di errore [modifica 08/07/2005]
			//alert('Attenzione: il codice lotto inserito non è presente. Impossibile selezionarlo!');
			showFm(messaggioErrore,false,5000);
			//document.getElementById('btnChiudiMessaggioErrore').focus();
			return;
		
		//SetFocusTbCodLottoLCB()
	}
	
	//per non perdere il fuoco con il codice nullo (modifica 29/12/2004)
	SetFocusTbCodLottoLCB()
	
}

function CheckLottoFromLCB(codLotto)
{
	if (codLotto!="")
	{
		for (i=1;i<document.getElementById(dg_lotti).rows.length;i++)
		{
			var obj=document.getElementById(dg_lotti).rows[i].cells[2];
			var objSpan=GetElementByTag(obj,'SPAN',1,1,false);
			if (objSpan.firstChild.nodeValue==codLotto)
			{
				var obj=document.getElementById(dg_lotti).rows[i].cells[0];
				var objChkbox=GetElementByTag(obj,'INPUT',1,1,false);
				if (!objChkbox.checked)
				{
					objChkbox.defaultChecked=true;
					objChkbox.checked=true;
					if(!controllaVacAttivate(objChkbox))
					{
						alert("Le vaccinazioni del lotto si sovrappongono a quelle di un lotto gia\' scelto. Non e\' possibile effettuare la selezione!");
						objChkbox.defaultChecked=false;
						objChkbox.checked=false;
					}							
					else
					{
						arLottiPresenti=creaLottiPresenti();
						modificato=true;
					}
				}
					
				SetFocusTbCodLottoLCB()
				return;							
			}
		}
		SetFocusTbCodLottoLCB()
	}
}

// lancia un messaggio di conferma per la selezione del lotto (modifica 28/12/2004)
function CheckaLotto(objChkbox, mode)
{
	if (!mode)
	{
		if(!confirm("Le vaccinazioni del lotto si sovrappongono a quelle di un lotto gia\' scelto. Effettuare comunque la selezione?"))
		{
			objChkbox.defaultChecked=false;
			objChkbox.checked=false;
			return;
		}
		else
		{
			objChkbox.defaultChecked=true;
			objChkbox.checked=true;
			return;
		}
	}
	else
	{
		objChkbox.defaultChecked=false;
		objChkbox.checked=false;
		return;
	}
}

function SetFocusTbCodLottoLCB()
{ 
		if(document.getElementById(modalName).style.display != 'none'){
			document.getElementById(tbCodLotto).value=""
			document.getElementById(tbCodLotto).focus()
		}
	
}

function CekkaLotti()
{
	for (i=1;i<document.getElementById(dg_lotti).rows.length;i++)
	{
		for(j=0;j<arLottiPresenti.length;j++)
		{
			
			obj=document.getElementById(dg_lotti).rows[i].cells[2];
			var objSpan=GetElementByTag(obj,'SPAN',1,1,false);
			
			var currentLotto=objSpan.firstChild.nodeValue
			var lotto=arLottiPresenti[j]
			
			if(lotto==currentLotto)
			{
				obj=document.getElementById(dg_lotti).rows[i].cells[0];
				var objChkbox=GetElementByTag(obj,'INPUT',1,1,false);
				objChkbox.defaultChecked=true;
				objChkbox.checked=true;
			}
		}
	}
}


function controlla_lotto(evt)
{	
	
	if (evt==null)
			evt=window.event;
			
	srcElement=SourceElement(evt);//FUNZIONE PER RICAVARE L'ELEMENTO CHE HA SCATENATO L'EVENT*/
					
	if(srcElement.checked)
	{
		if(!controllaVacAttivate(srcElement))
		{
			alert("Le vaccinazioni del lotto si sovrappongono a quelle di un lotto gia\' scelto. Non e\' possibile effettuare la selezione!");
			srcElement.checked=false;
		}
		else
		{
			arLottiPresenti=creaLottiPresenti();
			modificato=true;
		}
	}
	else
		arLottiPresenti=creaLottiPresenti();
	//	
	SetFocusTbCodLottoLCB()
	//
}

//la selezione manuale del lotto non deve essere possibile (modifica 29/12/2004)
function controlla_lottoBarCode(evt)
{			
	if (evt==null)
			evt=window.event;
			
	srcElement=SourceElement(evt);//FUNZIONE PER RICAVARE L'ELEMENTO CHE HA SCATENATO L'EVENT*/
	
	if(srcElement.checked)
	{
		alert('Attenzione: non e\' possibile selezionare manualmente un lotto!');
		srcElement.checked = false;
	}
	else
	{	
		alert('Attenzione: non e\' possibile eliminare manualmente un lotto!');
		srcElement.checked = true;	
	}
	//	
	SetFocusTbCodLottoLCB()
	//
}

function controllaVacAttivate(src)
{
	objTr=GetElementByTag(src,'TR',1,0,false);
	indexTr=objTr.rowIndex;				
	objSpan=GetElementByTag(objTr.cells[2],'SPAN',1,1,false);
	objName=objSpan.firstChild.nodeValue;
	nomeLotto=objName;

	for(i=0;i<arLotti.length;i++)
	{
		if(arLotti[i][0]==nomeLotto)
		{
			for (j=0;j<arLottiPresenti.length;j++)
			{
				for (k=0;k<arLotti.length;k++)
				{
					if(arLotti[k][0]==arLottiPresenti[j])
					{
						for (w=0;w<arLotti[k][1].length;w++)
						{
							for(q=0;q<arLotti[i][1].length;q++)
							{
								if(arLotti[k][1][w]==arLotti[i][1][q])
								{
									return false;
								}
							}	
						}
					}
				}
			}
			break;
		}
	}
	return true;
}

function creaLottiPresenti()
{
	j=0;
	arLottiPresenti=new Array();
	for (i=1;i<document.getElementById(dg_lotti).rows.length-1;i++)
	{
		objChk=GetElementByTag(document.getElementById(dg_lotti).rows[i].cells[0],'INPUT',1,1,false);
		objCod=GetElementByTag(document.getElementById(dg_lotti).rows[i].cells[2],'SPAN',1,1,false);
		objValCod=objCod.firstChild.nodeValue;
		if(objChk.checked)
		{
			arLottiPresenti[j]=objValCod;
			j++;
		}
	}
	return arLottiPresenti;
}

function DoBlurTbCodLottoLCB()
{
	if (document.getElementById(chkPenna).checked )
		if (document.getElementById(tbCodLotto).value!="") {document.getElementById(tbCodLotto).blur()}
}

var intID;

function checkKey()
{
	if (window.event.keyCode==13)
	{
		document.getElementById(tbCodLotto).blur();
	}
}