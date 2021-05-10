/*********funzione di validazione orario***********/
/*              05/02/2004 (samu)                 */
/*                                                */
/* obj(String) --> nome dell'oggetto che contie-  */
/*                 ne l'orario da validare;       */
/* sep(String) --> carattere di separazione       */
/*                 HH[sep]MM[sep]SS               */
/*                 (se stringa vuota formato di   */
/*                 input);                         */
/* focus(boolean) --> 'true' focus                */
/*                    'false' no focus;           */
/* msg (String) --> messaggio di errore: se strin-*/
/*                  ga vuota non visualizzato;    */
/**************************************************/

var busy;

function validaOrario(obj, sep, focus, msg)
{
	var ora=document.getElementById(obj).value;
	if (ora=="") return true;
	var cont = -1;
	var ok = false;
	
	// controllo della presenza dei caratteri ':' e '.'
	if (ora.search(sep) != cont)
	{
		splOra=ora.split(sep);
		var ok=true;
	} 
	else
	{			
		if (ora.search(sep) != cont)
		{
			splOra=ora.split(sep);
			var ok=true;
		}
	}
	var lung = splOra.length;
	ora0=""+splOra[0];
	ora1=""+splOra[1];
	ora2=""+splOra[2];
	
	if ((ora0=="")||(ora1==""))
	{
		if (msg!="") alert(msg);
		return false;
	}	
					
				
	// validazione dell'orario
	if (ok)
	{
		if ((splOra.length!=2)&&(splOra.length!=3))
		{
			if (msg!="") alert(msg);
			return false;
		}
		if ((isNaN(splOra[0]))||(isNaN(splOra[1])))
		{
			if (msg!="") alert(msg);
			return false;
		}
		else
			if (splOra.length==3)
				if (isNaN(splOra[2]))
				{
					if (msg!="") alert(msg);
					return false;
				}	
		if ((splOra.length==3) || (splOra.length==2))
		{
			if ((splOra[0].length>2)||(splOra[1].length>2)||(splOra[0].length<0)||(splOra[1].length<0))
			{
				if (msg!="") alert(msg);
				return false;	
			}
			else
				if (splOra.length==3)
					if ((splOra[2].length)>2||(splOra[2].length)<0)
					{
						if (msg!="") alert(msg);
						return false;
					}
				splOra[0]=parseInt(splOra[0]);
				splOra[1]=parseInt(splOra[1]);

				if ((splOra[0]<0)||(splOra[0]>23)) 
				{
					if (msg!="") alert(msg);
					return false;
				}
				if ((splOra[1]<0)||(splOra[1]>59)) 
				{
					if (msg!="") alert(msg);
					return false;
				}
				if (splOra.length==3)
				{
					splOra[2]=parseInt(splOra[2]);
					if ((splOra[2]<0)||(splOra[2]>59))
					{
						if (msg!="") alert(msg);
						return false;
					}
				}
				
			// formato dell'orario
					
			
			
			if (sep != "")
			{
				if (lung==3)
				{
					//ora0=""+splOra[0];
					//ora1=""+splOra[1];
					//ora2=""+splOra[2];
					if ((ora0.length)==1) ora0 = "0" + ora0;
					if ((ora1.length)==1) ora1 = "0" + ora1;
					if ((ora2.length)==1) ora2 = "0" + ora2;
					nuovaOra = ora0 + sep + ora1 + sep + ora2;
					document.getElementById(obj).value = nuovaOra;
					
				}
				else
				{
					//ora0=""+splOra[0];
					//ora1=""+splOra[1];
					if ((ora0.length)==1) ora0="0"+ora0; 
					if ((ora1.length)==1) ora1="0"+ora1; 
					nuovaOra = ora0 + sep + ora1;
					document.getElementById(obj).value = nuovaOra;
				}
			}
			
			// focus dell'elemento
			if (focus==true) document.getElementById(obj).focus();					
			return true;
		}
		if (msg!="") alert(msg);
		return false;
	}
	else return false;
}

function InizializzaToolBar(t)
{
	t.PostBackButton=false;
}

function ToolBarClick(ToolBar,button,evnt)
{
	evnt.needPostBack=true;

	switch(button.Key)
	{
		case 'btn_Indietro':

			if(checkMod()) 
			{ 
				evnt.needPostBack=confirm("Le modifiche effettuate andranno perse. Continuare?"); 
			} 
			else 
			{ 
				evnt.needPostBack=true; 
			} 
			break;
		
		case 'btn_Annulla':

 			if(busy=="True")
			{
				var ret;
				ret= confirm("Le modifiche effettuate andranno perse. Continuare?");
				if (ret==true)
				{
					window.location.href="Ambulatori.aspx?RicaricaDati=True";
					evnt.needPostBack=false;
				}
			}
			else
                    window.location.href ="Ambulatori.aspx?RicaricaDati=True";
			
			evnt.needPostBack=false;
			break;
		
		case 'btn_Salva':

			if(busy=="True") 
			{
				evnt.needPostBack=true; 
			}
			else 
				evnt.needPostBack=false; 
			break;
		
		case 'btn_Stampa':
		
			if (!ControllaOrari())
			{
				alert("Attenzione: il Centro Vaccinale non ha alcun orario impostato. Impossibile eseguire la stampa.");
				evnt.needPostBack=false;
			}
				
			break;
		
		case 'btn_Conferma':
			settimana=new Array ('Domenica','Lunedi', 'Martedi', 'Mercoledi', 'Giovedi', 'Venerdi', 'Sabato'); 
			//var object="or1_0";
			//var control = validaOrario(object, ".", true, "Brutto errore!");
			//alert(control);
			
			//il controllo deve essere fatto anche sugli appuntamenti (modifica 20/07/2004)
			des = new Array("","app");
			tipoGiorni = new Array(" (Orari Giornalieri)"," (Orari Appuntamenti)");
			
			for (count=0;count<des.length;count++)
			{
			
				for (i=0;i<7;i++) 
				{
					for (j=0;j<4;j++) 
					{ 
						var object="or"+i+"_"+j+des[count];
						oraValida=validaOrario(object, ".", false, "");
						if(!oraValida)
						{ 
							alert("I dati relativi a '" + settimana[i] + tipoGiorni[count] + "' non sono corretti!"); 
							document.getElementById("or"+i+"_"+j+des[count]).focus();
							evnt.needPostBack=false; 
						} 
					} 
				}
				
				//alert("eh eh eh!");
				for(i=0;i<7;i++) 
				{ 
					ctlTemp0=document.getElementById("or"+i+"_0"+des[count]);
					ctlTemp1=document.getElementById("or"+i+"_1"+des[count]); 
					ctlTemp2=document.getElementById("or"+i+"_2"+des[count]); 
					ctlTemp3=document.getElementById("or"+i+"_3"+des[count]); 
					if (ctlTemp0.value != "") 
					{ 
						if(ctlTemp1.value == "") 
						{ 
							alert("I dati relativi a '" + settimana[i] + tipoGiorni[count] + "' sono incompleti!"); 
							ctlTemp1.focus();
							evnt.needPostBack=false; 
						}
					}
					if (ctlTemp1.value != "") 
					{ 
						if(ctlTemp0.value == "") 
						{ 
							alert("I dati relativi a '" + settimana[i] + tipoGiorni[count] + "' sono incompleti!");
							ctlTemp0.focus(); 
							evnt.needPostBack=false; 
						} 
					}
					if(ctlTemp2.value != "")
					{ 
						if(ctlTemp3.value=="") 
						{
							alert("I dati relativi a '" + settimana[i] + tipoGiorni[count] + "' sono incompleti!"); 
							ctlTemp3.focus();
							evnt.needPostBack=false;
						}
					}
					if (ctlTemp3.value != "")
					{ 
						if(ctlTemp2.value=="") 
						{ 
							alert("I dati relativi a '" + settimana[i] + tipoGiorni[count] + "' sono incompleti!"); 
							ctlTemp2.focus();
							evnt.needPostBack=false; 
						}
					}
					ar0=ctlTemp0.value.split("."); 
					ar1=ctlTemp1.value.split("."); 
					d0=new Date("March 1, 1900 "+ ar0[0] +":"+ ar0[1]); 
					d1=new Date("March 1, 1900 "+ ar1[0] +":"+ ar1[1]); 
										
					if (!(ctlTemp0.value == "" && ctlTemp1.value == ""))
					{ 
						if (d0 >= d1)
						{
							alert("I dati relativi a '" + settimana[i] + tipoGiorni[count] + "' mattina non sono corretti: l'orario di apertura deve essere inferiore a quello di chiusura!");
							ctlTemp0.focus(); 
							evnt.needPostBack=false;
						}
					} 
					ar2=ctlTemp2.value.split("."); 
					ar3=ctlTemp3.value.split("."); 
					d2=new Date("March 1, 1900 "+ ar2[0] +":"+ ar2[1]);
					d3=new Date("March 1, 1900 "+ ar3[0] +":"+ ar3[1]); 
					if (!(ctlTemp2.value == "" && ctlTemp3.value == "")) 
					{ 
						if (d2 >= d3)
						{ 
							alert("I dati relativi a '" + settimana[i] + tipoGiorni[count] + "' pomeriggio non sono corretti: l'orario di apertura deve essere inferiore a quello di chiusura!"); 
							ctlTemp2.focus(); 
							evnt.needPostBack=false;
						} 
					} 
					if (!(ctlTemp1.value == "" && ctlTemp2.value == ""))
					{
						if (d1 > d2) 
						{ 
							alert("I dati relativi a '" + settimana[i] + tipoGiorni[count] + "' non sono corretti: il centro vaccinale non può aprire al pomeriggio prima che abbia chiuso alla mattina!");
							ctlTemp1.focus(); 
							evnt.needPostBack=false; 
						} 
					} 
					
					// controllo fascia oraria: mattino 07.00-14.00, pomeriggio 14.00-19.00
					ar0=ctlTemp0.value.split(".");  
					ar1=ctlTemp1.value.split("."); 
					hh0=0;
					hh1=0;
					hh0=ar0[0];
					hh1=ar1[0];
					if (((hh0>14)||(hh0<7))&&((hh0!="")&&(hh1!="")))
					{
					    alert("I dati relativi a '" + settimana[i] + tipoGiorni[count] + "' non sono corretti: l'orario del centro vaccinale si riferisce alla fascia mattutina!");
						ctlTemp0.focus(); 
						evnt.needPostBack=false; 
					} 
					if (((hh1>14)||(hh1<7))&&((hh0!="")&&(hh1!="")))
					{
					    alert("I dati relativi a '" + settimana[i] + tipoGiorni[count] + "' non sono corretti: l'orario del centro vaccinale si riferisce alla fascia mattutina!");
						ctlTemp1.focus(); 
						evnt.needPostBack=false; 
					} 
					ar2=ctlTemp2.value.split(".");  
					ar3=ctlTemp3.value.split("."); 
					hh2=0;
					hh3=0;
					hh2=ar2[0];
					hh3=ar3[0];
					if (((hh2<14)||(hh2>19))&&((hh2!="")&&(hh3!="")))
					{
					    alert("I dati relativi a '" + settimana[i] + tipoGiorni[count] + "' non sono corretti: l'orario del centro vaccinale si riferisce alla fascia pomeridiana!");
						ctlTemp2.focus(); 
						evnt.needPostBack=false; 
					} 
					if (((hh3<14)||(hh3>19))&&((hh2!="")&&(hh3!="")))
					{
					    alert("I dati relativi a '" + settimana[i] + tipoGiorni[count] + "' non sono corretti: l'orario del centro vaccinale si riferisce alla fascia pomeridiana!");
						ctlTemp3.focus(); 
						evnt.needPostBack=false; 
					} 
				}
			
			} //fine for (count)
		break;
	}
} 

//controllo presenza orari (modifica 26/08/2004)
function ControllaOrari()
{
	des = new Array("","app");
	for (count=0;count<des.length;count++)
	{
	
		for (i=0;i<7;i++) 
		{
			for (j=0;j<4;j++) 
			{ 
				var object="or"+i+"_"+j+des[count];
				var orario = document.getElementById(object).value;
				if (orario!='')
				{
					return true;
				}
			}
		}
	}
	return false;
}