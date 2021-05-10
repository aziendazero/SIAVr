
/*cerca il frame contenente i menu di livello zero*/
function getTopFrame() {
    var oParent, oFrame;
    var oLastParent = null;

    if (window == window.parent) return null;

    oParent = window.parent;

    oFrame = oParent.frames["TopFrame"];


    while (oParent != null && oFrame == null && oParent != oLastParent) {

        oLastParent = oParent;
        oParent = oParent.parent;
        if (oParent != null)
            oFrame = oParent.frames["TopFrame"];
    }

    return oFrame;
}


/*cerca il frame contenente la panelbar*/
function getLeftFrame(){
	var oParent,oFrame;

	 if (window==window.parent) return null;
	 
	 oParent=window.parent;
	 oFrame=oParent.frames["LeftFrame"];
	
	
	while (oParent!=null && oFrame ==null ){
		oParent=oParent.parent;
		if (oParent!=null)
			oFrame= oParent.frames["LeftFrame"];
	}
	return 	oFrame;
}

/*Setta il titolo (nome e stile)*/
function OnitLayoutSetTitle(titolo,cssClass)
{	
	var Topf=getTopFrame();
	if (Topf==null){return}
	Topf.document.getElementById("lblNomeApp").innerHTML=titolo
	Topf.document.getElementById("lblNomeApp").className=cssClass
}
/* fine  OnitLayoutSetTitle*/

/*Imposta l'action del form con la procedura "OnitLayoutSubmit" */
/*function OnitLayoutSetSubmit(msg,hyde)
{
	var oldAction=document.forms[0].action;
	var newAction="javascript:OnitLayoutSubmit(" + hyde.toString() + ",'" + msg + "','" + oldAction + "')";
	document.forms[0].action=newAction;
	window.status=window.defaultStatus;
}*/
/* fine OnitLayoutSetOnitSubmit*/

/* Gestisce il submit della pagina nascondendone il contenuto e/o scrivendo nella barra di stato un msg personalizzato*/
/*function OnitLayoutSubmit(hyde,msg,oldAction)
{
	if (msg!=null) window.status=msg;
	if (hyde) document.body.style.display="none";
	document.forms[0].action=oldAction;
	document.forms[0].submit();
}/*
/*fine OnitSubmit*/

/*function OnitLayoutEnterClick(evnt,postBackClientScript)
{
	if (evnt.keyCode==13) eval(postBackClientScript)
}*/

/* Disabilita-Abilita il TopFRame e/o il LeftFrame*/
function OnitLayoutStatoMenu(Disabilita,Cosa)
{
	var Leftf,Topf;
	var e, pnlBar;
	Leftf=getLeftFrame();
	Topf=getTopFrame();
	if (Disabilita) 
	{
		if (Cosa == null || Cosa==1)
		{		
			if (Topf!=null) Topf.document.getElementById("copri").style.visibility="visible";
		}
		if (Cosa == null || Cosa==0)
		{
			if (Leftf!=null)
			{	
				if (useOldPanelbar()){
					if (Leftf.document.getElementById("copri")!=null)
						Leftf.document.getElementById("copri").style.visibility="visible"; /*da abbandonare in futuro compatibilità con la panelbar */
				} else {
					if (existsInfraPanelbar()){
						pnlBar=Leftf.iglbar_getListbarById("UltraWebListbar");
						if (pnlBar != null)
							pnlBar.setEnabled(false);
					}
				}
			}
			
		}
	}
	else
	{
		
		
		if (Cosa == null || Cosa==1)
		{
			if (Topf!=null) Topf.document.getElementById("copri").style.visibility="hidden";
		}
		if (Cosa == null || Cosa==0)
		{
			if (Leftf!=null)
			{

				if (useOldPanelbar()){
					if (Leftf.document.getElementById("copri")!=null)
						Leftf.document.getElementById("copri").style.visibility="hidden"; /*da abbandonare in futuro compatibilità con la panelbar */
				} else {
					if (existsInfraPanelbar()){
						pnlBar=Leftf.iglbar_getListbarById("UltraWebListbar")/*.setEnabled(true)*/; 
						if ( pnlBar!=null) pnlBar.setEnabled(true);
					}
				
				}
			}
		}
		
	}

}
/* fine OnitLayoutStatoMenu*/

/* Modifica i sotto-menu del LeftFrame*/
function SetTextLeftMenu(find, text, important, disabled) {
	if (useOldPanelbar()){
			SetTextLeftMenu_Coa(find,text,important,disabled);
		} else {
			SetTextLeftMenu_Infra(find,text,important,disabled);
		}  
}

function SetTextLeftMenu_Infra(find,text,important,disabled){
	var lf, objLB, i,j, objGp, objIt, testo, str;
	lf=getLeftFrame();
	if (lf==null) {return;} 

	objLB = lf.iglbar_getListbarById("UltraWebListbar");
	/*ciclo per tutti i gruppi alla ricerca del testo*/
	objIt=null;
	for (i=0; i<objLB.Groups.length; i++){
		objGp=objLB.Groups[i];
		for (j=0; j<objGp.Items.length; j++){
			testo =objGp.Items[j].getText();
			testo=testo.toLowerCase();
			str=find.toLowerCase();
			if (testo.indexOf(str)>=0){
				objIt=objGp.Items[j];
				break;
			}
		}
		if (objIt!=null) break;
	}

	/*applico le modifiche all'elemento trovato*/
	if (objIt!=null){
		/*stile*/
		switch (important){	
			case 1:
			    objIt.DefaultStyleClassName = 'InfraListBar_ItemStyle Menu_Select';
				break;
			case 2:
			    objIt.DefaultStyleClassName = 'InfraListBar_ItemStyle Menu_Normal';
				break;
			case 3:
			    objIt.DefaultStyleClassName = 'InfraListBar_ItemStyle Menu_Info';
				break;
		}	
		objIt.Element.className=objIt.DefaultStyleClassName;
		/*testo*/
		var ht=objIt.Element.innerHTML;
		var ix=ht.toLowerCase().lastIndexOf(find.toLowerCase());
		if (ix>=0){
			ht=ht.substr(0,ix) + text;
			objIt.Element.innerHTML=ht;
		}
		/*abilitazione*/
		return;
		if (disabled){
			objIt.setEnabled(false);
		}

	}

}


/* Modifica i sotto-menu del LeftFrame*/
function SetTextLeftMenu_Coa(find,text,important,disabled)
{
	var lf;
	lf=getLeftFrame();
	if (lf==null) {return;} 
	for (i=0;i<lf.document.getElementById("CSPBIcons0").firstChild.rows.length-1;i++)
	{
	
	///////////////////////////////////////////////////////////////////////////////////////////////
	//Marco 26-01-03 ho sostituito la riga sotto commentata con il seguente blocco di codice     //
	//per rendere il tutto compatibile con Mozilla                                               //
	///////////////////////////////////////////////////////////////////////////////////////////////
	
	//	if (lf.document.getElementById("CSPBIcons0").firstChild.rows[i].innerText.substring(0,find.length)==find) {break;}
	
	elAnchor=GetElementByTag(lf.document.getElementById("CSPBIcons0").firstChild.rows[i],'A',1,1,false);
	if (elAnchor!=null){
		var testo=elAnchor.firstChild.nodeValue;
		if (testo!=null){
			var subStr=testo.substring(0,find.length);
			if (subStr==find) 
						break;
			}
		}
	//fine Marco
	
	}
	
	if (useOldPanelbar()){
		if (lf.document.getElementById("CSPBIcons0").firstChild.rows[i].cells[0].firstChild !=null)
		{
			switch (important)
			{	
				case 1:
					lf.document.getElementById("CSPBIcons0").firstChild.rows[i].cells[0].firstChild.className="Menu_Select";
					break;
				case 2:
					lf.document.getElementById("CSPBIcons0").firstChild.rows[i].cells[0].firstChild.className="Menu_Normal";
					break;
				case 3:
					lf.document.getElementById("CSPBIcons0").firstChild.rows[i].cells[0].firstChild.className="Menu_Info";
					break;
			}
			
			lf.document.getElementById("CSPBIcons0").firstChild.rows[i].cells[0].firstChild.innerHTML=text
			if (disabled)
			{
				lf.document.getElementById("CSPBIcons0").firstChild.rows[i].cells[0].firstChild.href="javascript:function (){return false}"	
			}
		}
	
	} else {

		switch (important)
		{	
			case 1:
				lf.document.getElementById("CSPBIcons0").firstChild.rows[i].cells[0].firstChild.className="Menu_Select";
				break;
			case 2:
				lf.document.getElementById("CSPBIcons0").firstChild.rows[i].cells[0].firstChild.className="Menu_Normal";
				break;
			case 3:
				lf.document.getElementById("CSPBIcons0").firstChild.rows[i].cells[0].firstChild.className="Menu_Info";
				break;
		}
		
		lf.document.getElementById("CSPBIcons0").firstChild.rows[i].cells[0].firstChild.innerHTML=text
		if (disabled)
		{
			lf.document.getElementById("CSPBIcons0").firstChild.rows[i].cells[0].firstChild.href="javascript:function (){return false}"	
		}
	} /*end useOld..*/	
		
}
/* fine SetTextLeftMenu*/
			
function useOldPanelbar(){ 
	var lef, blnRet;
	
	 lef= getLeftFrame();
	 blnRet=(lef.iglbar_getListbarById==null);
         return blnRet 
}

/*visto che il metodo iglbar_getListbarById() da errore se la panelbar non è renderizzata 
  questa funzione permette di stabilire se esiste la panelbar */
function existsInfraPanelbar(){
	var lef, blnRet;
	lef= getLeftFrame();
	blnRet=(lef.document.getElementById("UltraWebListbar")!=null);
	return blnRet; 
}


