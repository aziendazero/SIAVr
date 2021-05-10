
//var paz_deceduto;
//var consultorioCodiceDefault;
//var consultorioDescrizioneDefault;
//var luogoConsultorio; 				// Constante che indica come luogo di vaccinazione il consultorio

////function cmbLuogo_onChange(sender) {
////    AbilitaConsultorio(sender, true);
////    AbilitaMedico(sender, true);
////    AbilitaLuogo(sender, true);
////    Replica(6, sender.selectedIndex, -1, sender);
////}

////function AbilitaConsultorio(ob, focus) {
////    el = GetElementByTag(ob, 'TR', 1, 0, false);
////    elInput = GetElementByTag(el.cells[7], 'DIV', 1, 1, false);
////    if (ob.selectedIndex == 1) {
////        elInput.childNodes[0].readOnly = false;
////        elInput.childNodes[1].readOnly = false;
////        elInput.childNodes[0].className = "TextBox_Stringa_Obbligatorio";
////        elInput.childNodes[1].className = "TextBox_Stringa_Obbligatorio";
////        if (elInput.childNodes[1].value == '') {
////            if (consultorioCodiceDefault == null) {
////                elInput.childNodes[1].value = '';
////                elInput.childNodes[0].value = '';
////            }
////            else {
////                elInput.childNodes[1].value = consultorioCodiceDefault;
////                elInput.childNodes[0].value = consultorioDescrizioneDefault;
////            }
////        }
////        if (focus)
////            elInput.childNodes[0].focus();
////    }
////    else {
////        elInput.childNodes[0].readOnly = true;
////        elInput.childNodes[1].readOnly = true;
////        elInput.childNodes[0].className = "TextBox_Stringa_Disabilitato";
////        elInput.childNodes[1].className = "TextBox_Stringa_Disabilitato";
////        elInput.childNodes[0].value = "";
////        elInput.childNodes[1].value = "";
////    }
////}

////function AbilitaMedico(ob, focus) {
////    el = GetElementByTag(ob, 'TR', 1, 0, false);
////    elInput = GetElementByTag(el.cells[8], 'DIV', 1, 1, false);
////    if (ob.selectedIndex == 4) {
////        if (focus)
////            elInput.childNodes[0].focus();
////    }
////}

////function AbilitaLuogo(ob, focus) {

////    el = GetElementByTag(ob, 'TR', 1, 0, false);
////    elInput = GetElementByTag(el.cells[9], 'DIV', 1, 1, false);

////    if ((ob.selectedIndex != 0) && (ob.selectedIndex != 1)) {
////        elInput.childNodes[0].readOnly = false;
////        elInput.childNodes[1].readOnly = false;
////        elInput.childNodes[0].className = "TextBox_Stringa";
////        elInput.childNodes[1].className = "TextBox_Stringa";
////        if (focus)
////            elInput.childNodes[0].focus();
////    }
////    else {
////        elInput.childNodes[0].readOnly = true;
////        elInput.childNodes[1].readOnly = true;
////        elInput.childNodes[0].className = "TextBox_Stringa_Disabilitato";
////        elInput.childNodes[1].className = "TextBox_Stringa_Disabilitato";
////        elInput.childNodes[0].value = "";
////        elInput.childNodes[1].value = "";
////    }
////}

//function BeforeReplicaData(evt) {
//    el = evt.srcElement
//    if (ctl_CheckData(el.id, True, "Inserire una data valida", 0, True))
//        Replica(5, el.value, el)
//    else
//        evt.returnValue = false
//}

function OnClientButtonClicking(sender, args) 
{
    if (!e) var e = window.event;

    var button = args.get_item();
    
    switch (button.get_value())	
    {
        case 'btnEsegui':
            if (paz_deceduto == 'True') 
            {
                alert('ATTENZIONE: il paziente selezionato risulta deceduto.');
                
            }
            else 
            {
                dgr = document.getElementById("dgrVaccinazioni");
                if (dgr != null && dgr.rows.length > 1) 
                {
                    if (!ControllaEsegui()) 
                    {
                        args.set_cancel(true);
                        break;
                    }
                }
                else 
                {
                    args.set_cancel(true);
                    alert("Selezionare almeno una vaccinazione da eseguire!")
                }
            }
            break;

        case 'btnElimina':
            dgr = document.getElementById("dgrVaccinazioni");
            if (dgr != null && dgr.rows.length > 1) 
            {
                if (!ControllaEsegui()) 
                {
                    args.set_cancel(true);
                    break;
                }
            }
            else 
            {
                alert("Selezionare almeno una vaccinazione da eliminare!")
                args.set_cancel(true);
            }
            break

            //controllo: il salvataggio deve andare al server se le vaccinazioni sono eseguite (modifica 08/06/2004)
        case 'btnSalva':
            if (!controllaVacEseguite()) 
            {
                alert("Eseguire almeno una vaccinazione per effettuare il salvataggio!");
                args.set_cancel(true);
            }
            break;

        case 'btnAnnulla':
            var dgrVacc = document.getElementById("dgrVaccinazioni");
            if (dgrVacc == null || dgrVacc.rows.length == 0) 
            {
                args.set_cancel(true);
            }
            break;
    }
}

//// N.B. : controlla la colonna con l'immagine della vacc eseguita -> se si aggiungono colonne va modificato l'indice!!!
function controllaVacEseguite() 
{
    dgr = document.getElementById("dgrVaccinazioni");
    if (dgr == null) return false;
    for (i = 1; i < dgr.rows.length; i++) 
    {
        objChk = GetElementByTag(dgr.rows[i].cells[13], 'IMG', 1, 1, false);
        if (objChk != null) return true;
    }
    return false;
}

//function OnitDataPick_Blur(id, e) {
//    Replica(5, OnitDataPickGet(id), null, document.getElementById(id + '_Int').parentNode);
//}

function ControllaEsegui() {
    dgr = document.getElementById("dgrVaccinazioni");

    for (i = 1; i < dgr.rows.length; i++) {

        objChk = GetElementByTag(dgr.rows[i].cells[0], 'INPUT', 1, 1, false);
        if (objChk.checked) {

            txtAssDose = dgr.rows[i].cells[3].getElementsByTagName('INPUT')[0];
            if (!IsValidValue(txtAssDose.value)) {
                alert("Numero dose associazione non ammesso!");
                return false
            }

            txtDose = dgr.rows[i].cells[4].getElementsByTagName('INPUT')[1];
            if (!IsValidValue(txtDose.value)) {
                alert("Numero dose vaccinazione non ammesso!");
                return false
            }

            return true
        }
    }
    alert("Selezionare almeno una vaccinazione!")
    return false
}


//function Replica(col, value1, value2, el) {
//    dgr = document.getElementById("dgrVaccinazioni")

//    var continua = true;
//    if (el != null) {
//        var row = GetElementByTag(el, 'TR', 1, 0, false);

//        check = row.getElementsByTagName("INPUT")[0];
//        continua = check.checked;
//    }

//    if (continua) {
//        for (i = 1; i < dgr.rows.length; i++) {
//            objChk = GetElementByTag(dgr.rows[i].cells[0], 'INPUT', 1, 1, false);
//            if (objChk.checked) {
//                switch (col) {


//                    case 2: // dose ASS
//                        txtAssDose = dgr.rows[i].cells[col].getElementsByTagName('INPUT')[0];
//                        if (!txtAssDose.disabled)
//                            txtAssDose.value = value1;
//                        break;

//                    case 5: //datepick
//                        objDataPick = GetElementByTag(dgr.rows[i].cells[col], 'TABLE', 1, 1, false);
//                        idPick = objDataPick.id;
//                        idPick = idPick.replace("_Int", "");
//                        if (OnitDataPickGet(idPick) == "")
//                            OnitDataPickSet(idPick, value1);
//                        break;

//                    case 6: //Combo Luogo
//                        objSelect = GetElementByTag(dgr.rows[i].cells[col], 'SELECT', 1, 1, false);
//                        objSelect.selectedIndex = value1;
//                        //AbilitaConsultorio(objSelect, false);
//                        //AbilitaLuogo(objSelect, false);

//                        break;

//                    case 7: //Consultorio
//                        obj = GetElementByTag(dgr.rows[i].cells[col], 'INPUT', 1, 1, false);
//                        objLuogo = GetElementByTag(dgr.rows[i].cells[6], 'SELECT', 1, 1, false);
//                        if (obj.value == "" && objLuogo.value == luogoConsultorio) {
//                            obj.value = value1;
//                            var name = obj.id;
//                            document.getElementById(name + '_Cod').value = value2;
//                        }
//                        break;

//                    //case 8: //Medico
//                    //    objFM = GetElementByTag(dgr.rows[i].cells[col], 'INPUT', 1, 1, false);
//                    //    if (objFM.value == "") {
//                    //        objFM.value = value1;
//                    //        var name = objFM.id;
//                    //        document.getElementById(name + '_Cod').value = value2;
//                    //    }
//                    //    break;

//                    //case 9: //Luogo
//                    //    objFM = GetElementByTag(dgr.rows[i].cells[col], 'INPUT', 1, 1, false);
//                    //    if (objFM.value == "") {
//                    //        objFM.value = value1;
//                    //        objFM.nextSibling.value = value2;
//                    //    }
//                    //    break;

//                }
//            }
//        }
//    }
//}

//function BeforeReplicaCons(par1, par2, par3, par4, tipoOperazione, fmName, campiImpostati) {
//    var desc;
//    var cod;
//    switch (tipoOperazione) {
//        case 0:
//            desc = campiImpostati['DESCRIZIONE'];
//            cod = campiImpostati['CODICE'];
//            Replica(7, desc, cod, document.getElementById(par1));
//            FM_OK_Click(par1, par2, par3, par4, tipoOperazione, campiImpostati);
//            break;
//        case 1:
//        case 2:
//            FM_Annulla_Click(par1, par2, '', '', tipoOperazione, campiImpostati);
//            break;
//        case 3:
//            FM_Annulla_Click(par1, par2, '', '', tipoOperazione, campiImpostati);
//            desc = ''
//            cod = ''
//            Replica(7, desc, cod, document.getElementById(par1))
//            break;
//    }
//}

////function BeforeReplicaMedico(par1, par2, par3, par4, tipoOperazione, fmName, campiImpostati) {
////    var desc;
////    var cod;
////    switch (tipoOperazione) {
////        case 0:
////            desc = campiImpostati['NOME'];
////            cod = campiImpostati['CODICE'];
////            Replica(8, desc, cod, document.getElementById(par1));
////            FM_OK_Click(par1, par2, par3, par4, tipoOperazione, campiImpostati);
////            break;
////        case 1:
////        case 2:
////            FM_Annulla_Click(par1, par2, '', '', tipoOperazione, campiImpostati);
////            break;
////        case 3:
////            FM_Annulla_Click(par1, par2, '', '', tipoOperazione, campiImpostati);
////            desc = ''
////            cod = ''
////            Replica(8, desc, cod, document.getElementById(par1))
////            break;
////    }
////}

////function BeforeReplicaLuogo(par1, par2, par3, par4, tipoOperazione, fmName, campiImpostati) {
////    var desc;
////    var cod;
////    switch (tipoOperazione) {
////        case 0:
////            desc = campiImpostati['NOME'];
////            cod = campiImpostati['CODICE'];
////            Replica(9, desc, cod, document.getElementById(par1));
////            FM_OK_Click(par1, par2, par3, par4, tipoOperazione, campiImpostati);
////            break;
////        case 1:
////        case 2:
////            FM_Annulla_Click(par1, par2, '', '', tipoOperazione, campiImpostati);
////            break;
////        case 3:
////            FM_Annulla_Click(par1, par2, '', '', tipoOperazione, campiImpostati);
////            desc = ''
////            cod = ''
////            Replica(9, desc, cod, document.getElementById(par1))
////            break;
////    }
////}

function ClearCol(col) 
{
    dgr = document.getElementById("dgrVaccinazioni");

    for (i = 1; i < dgr.rows.length; i++) 
    {
        var chk = dgr.rows[i].cells[0].getElementsByTagName('INPUT')[0];
        if (chk.checked) 
        {
            switch (col) 
            {
                case 2: //dose
                    txtDose = dgr.rows[i].cells[col].getElementsByTagName('INPUT')[0];
                    if (!txtDose.disabled)
                        txtDose.value = '';
                    break;

                case 3: //dose ASS
                    txtAssDose = dgr.rows[i].cells[col].getElementsByTagName('INPUT')[0];
                    if (!txtAssDose.disabled)
                        txtAssDose.value = '';
                    break;

                case 5: //datepick
                    objDataPick = GetElementByTag(dgr.rows[i].cells[col], 'TABLE', 1, 1, false);
                    idPick = objDataPick.id;
                    idPick = idPick.replace("_Int", "");
                    if (!document.getElementById(idPick + 1).readOnly) 
                    {
                        OnitDataPickSet(idPick, '');
                    }
                    break;

                case 6: //Combo Luogo
                    objSelect = GetElementByTag(dgr.rows[i].cells[col], 'SELECT', 1, 1, false);
                    if (!objSelect.disabled) 
                    {
                        objSelect.selectedIndex = 0;
                        //AbilitaConsultorio(objSelect, false);
                        //AbilitaMedico(objSelect, false);
                        //AbilitaLuogo(objSelect, false);
                    }
                    break;

                case 7: //Combo Luogo
                    objSelect = GetElementByTag(dgr.rows[i].cells[col], 'SELECT', 1, 1, false);
                    if (!objSelect.disabled) {
                        objSelect.selectedIndex = 0;
                        //AbilitaConsultorio(objSelect, false);
                        //AbilitaMedico(objSelect, false);
                        //AbilitaLuogo(objSelect, false);
                    }
                    break;

                case 8: //Consultorio
                    obj = GetElementByTag(dgr.rows[i].cells[col], 'INPUT', 1, 1, false);
                    objLuogo=GetElementByTag(dgr.rows[i].cells[6],'SELECT',1,1,false);
                    if (!obj.readOnly) 
                    {
                        obj.value = '';
                        obj.nextSibling.value = '';
                    }
                    break;

                //case 8: //Medico
                //    objFM = GetElementByTag(dgr.rows[i].cells[col], 'INPUT', 1, 1, false);
                //    var name = objFM.id;
                //    if (!document.getElementById(name + '_Cod').readOnly) 
                //    {
                //        objFM.value = '';
                //        document.getElementById(name + '_Cod').value = '';
                //    }
                //    break;

                //case 9: //Luogo
                //    objFM = GetElementByTag(dgr.rows[i].cells[col], 'INPUT', 1, 1, false);
                //    if (!objFM.readOnly) 
                //    {
                //        objFM.value = '';
                //        objFM.nextSibling.value = '';
                //    }
                //    break;

                case 10: //Esito
                    var obj = GetElementByTag(dgr.rows[i].cells[col], 'input', 1, 1, false);
                    obj.checked = false;
            }
        }
    }
}

//function CancellaTutto() 
//{
//    ClearCol(2);
//    ClearCol(3);
//    ClearCol(5);
//    ClearCol(6);
//    ClearCol(7);
//    ClearCol(8);
//    ClearCol(9);
//    ClearCol(10);
//}


function IsValidValue(val) {
    val = val.replace('.', ',');
    if (isNaN(val))
        return false;
    else
        return ((val - 0) > 0);
}