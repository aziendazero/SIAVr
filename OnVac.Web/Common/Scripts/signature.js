
/*
firma un documento di testo con la smartcard
*/
function signText(text) {
    return signApplet(text);
}


function signApplet(text) {
    var error = null;
    var result = null;
    var errorCode = 0;
    if (OnJSign != null) {
        try {
            OnJSign.setForceExpiredCert(true);// commentare per impedire di utilizzare un certificato scaduto
            result=OnJSign.signText(text, false);
        } catch (e) {
            error = e.message
            errorCode = 1;
        }
        if (error==null){
            error = OnJSign.getLastErrorDescription();
            errorCode = OnJSign.getLastErrorCode();
        }
    }
    else {
        errorCode = 1;
        error ="Applet OnJSign per la firma elettronica non trovata";
    }

    return { data: result, errorCode : errorCode, error: error };
}

//----------------------------- firma di documenti --------------------------

function SignDocumentClass(serviceUrl, docIds, userId, appId, codiceAzienda, callBack) {

    this.serviceUrl = serviceUrl;
    this.ids = docIds;
    this.userId = userId;
    this.appId = appId;
    this.codiceAzienda = codiceAzienda;
    this.callBack = callBack;
    this.currentIndex = -1;
    this.results = [];

    var me = this;

    this.OnError = function (e,stop) {
        //punto finale con errore
        this.results[currentIndex] = e;
        if (stop || !this.signNext()) {
            if (this.callBack) callBack(this, this.results);
        }
    };

    this.OnUploadDocumentSuccess = function (data, status) {
        // punto finale esecuzione
        var docId=this.ids[currentIndex];
        var result = { Ok: data.d.Ok, Message: data.d.Message, documentId: docId};
        this.results[currentIndex] = result;

        // la callback alla fine della firma
        if (!this.signNext()) {
            if (this.callBack) this.callBack(this, this.results);
        }
    };

    this.OnGetDocumentSuccess = function (data, status) {
        // firma del documento e upload
        var signResult = null;
        
        signResult = signText(data.d.Xml);
        var signedXml = signResult.data;
        if (signResult.errorCode==1) {
            var error = "Errore durante la firma del documento:\n" + signResult.error;
            me.OnError({ Ok: false, Message: error },false);
            return null;
        } else if (signResult.errorCode>0) {
            me.OnError({ Ok: false, Message: "Operazione annullata." }, true);
            return null;
        }

        var strJson2 = JSON.stringify(
            {
                signed: { Id: data.d.Id, SignedXml: signedXml },
                idUtente: this.userId,
                idApplicazione: this.appId,
                codiceAzienda: this.codiceAzienda
            });

        $.ajax({
            type: "POST",
            url: this.serviceUrl + "/UploadSignedDocument",
            data: strJson2,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, status) { me.OnUploadDocumentSuccess(data, status) },
            error: function (request, status, error) { me.OnError({Ok:false, Message:error});}
        });
    };

    this.sign = function(index) {
        // recupero documento da server
        var strJson1 = JSON.stringify(
            {
                idDocumento: this.ids[index],
                idUtente: this.userId,
                idApplicazione: this.appId,
                codiceAzienda: this.codiceAzienda
            });

        $.ajax({
            type: "POST",
            url: this.serviceUrl + "/GetDocument",
            data: strJson1,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, status) { me.OnGetDocumentSuccess(data, status) },
            error: function (request, status, error) { me.OnError({ Ok: false, Message: error }); }
        });
    };

    this.signNext = function () {
        this.currentIndex++;
        if (this.ids != null && this.currentIndex < this.ids.length) {
            this.sign(this.currentIndex);
            return true;
        } else
            return false;
    }

    // parte con la firma del primo documento
    this.signNext();
}

function signDocument(serviceUrl, docIds, userId, appId, codiceAzienda, callBack) {
    var c = SignDocumentClass(serviceUrl, docIds, userId, appId, codiceAzienda, callBack)
}
