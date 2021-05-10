Imports System.Configuration
Imports System.Collections.Generic

Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.DIRV.ArchiviazioneSostitutiva


Public Class BizFirmaDigitale
    Inherits Biz.BizClass

#Region " Constants "

    Public Const ID_PROCESSO_ARCHIVIAZIONE_SOSTITUTIVA As Integer = 16

#End Region


#Region " Parametri Servizio Archiviazione "

    Private Property ParametriArchiviazioneDIRV As ArchiviazioneDIRVConfigurationSettings

    Private Class ArchiviazioneDIRVConfigurationSettings

#Region " Private "

        Private _Mittente As String
        Private _Destinatario As String
        Private _UserId As String
        Private _Password As String
        Private _Ente As String
        Private _AreaOrg As String
        Private _EndUserId As String
        Private _IdClassificazione As String
        Private _NomeFileIntestazione As String
        Private _NomeFileMetadatiRichiesta As String
        Private _NomeFileMetadatiRisposta As String
        Private _UrlServizioArchiviazione As String

#End Region

#Region " Properties "

        ''' <summary>
        ''' Il richiedente del servizio. Obbligatorio.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Mittente As String
            Get
                Return Me._Mittente
            End Get
        End Property

        ''' <summary>
        ''' Destinatario (o la lista di destinatari) del servizio se diverso dal richiedente. Non obbligatorio.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Destinatario As String
            Get
                Return Me._Destinatario
            End Get
        End Property

        ''' <summary>
        ''' Obbligatorio solo per i servizi che richiedono questo tipo di autenticazione. In questo caso la comunicazione avverrà in Https.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property UserId As String
            Get
                Return Me._UserId
            End Get
        End Property

        ''' <summary>
        ''' Obbligatorio solo per i servizi che richiedono questo tipo di autenticazione. In questo caso la comunicazione avverrà in Https.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Password As String
            Get
                Return Me._Password
            End Get
        End Property

        ''' <summary>
        ''' Ente
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Ente As String
            Get
                Return Me._Ente
            End Get
        End Property

        ''' <summary>
        ''' Area organizzativa. Può valere: A_RGVT01_01_01 per l’area dei Decreti, A_RGVT01_01_02 per l’area dei Mandati e A_RGVT01_01_03 per l’area delle Delibere
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property AreaOrg As String
            Get
                Return Me._AreaOrg
            End Get
        End Property

        ''' <summary>
        ''' Identificativo dell’utente che effettua l’operazione di archiviazione/conservazione. 
        ''' Tale campo è libero e non vengono eseguiti controlli. Viene usato per facilitare la tracciatura dell’operazione.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property EndUserId As String
            Get
                Return Me._EndUserId
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IdClassificazione As String
            Get
                Return Me._IdClassificazione
            End Get
        End Property

        ''' <summary>
        ''' Nome del file di intestazione
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property NomeFileIntestazione As String
            Get
                If String.IsNullOrWhiteSpace(Me._NomeFileIntestazione) Then
                    Me._NomeFileIntestazione = "XML_MESSAGE"
                End If
                Return Me._NomeFileIntestazione
            End Get
        End Property

        ''' <summary>
        ''' Nome del file dei metadati di richiesta applicativa
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property NomeFileMetadatiRichiesta As String
            Get
                If String.IsNullOrWhiteSpace(Me._NomeFileMetadatiRichiesta) Then
                    Me._NomeFileMetadatiRichiesta = "metadati.xml"
                End If
                Return Me._NomeFileMetadatiRichiesta
            End Get
        End Property

        ''' <summary>
        ''' Nome del file dei metadati di risposta applicativa
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property NomeFileMetadatiRisposta As String
            Get
                If String.IsNullOrWhiteSpace(Me._NomeFileMetadatiRisposta) Then
                    Me._NomeFileMetadatiRisposta = "metadatiRisposta.xml"
                End If
                Return Me._NomeFileMetadatiRisposta
            End Get
        End Property

        ''' <summary>
        ''' Url del servizio di archiviazione-conservazione documenti
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property UrlServizioArchiviazione As String
            Get
                Return Me._UrlServizioArchiviazione
            End Get
        End Property

#End Region

        Public Sub New()
            Me._Mittente = ConfigurationManager.AppSettings("ArchiviazioneDIRV_Mittente")
            Me._Destinatario = ConfigurationManager.AppSettings("ArchiviazioneDIRV_Destinatario")
            Me._UserId = ConfigurationManager.AppSettings("ArchiviazioneDIRV_UserId")
            Me._Password = ConfigurationManager.AppSettings("ArchiviazioneDIRV_Password")
            Me._Ente = ConfigurationManager.AppSettings("ArchiviazioneDIRV_Ente")
            Me._AreaOrg = ConfigurationManager.AppSettings("ArchiviazioneDIRV_AreaOrg")
            Me._EndUserId = ConfigurationManager.AppSettings("ArchiviazioneDIRV_EndUserId")
            Me._IdClassificazione = ConfigurationManager.AppSettings("ArchiviazioneDIRV_IdClassificazione")
            Me._NomeFileIntestazione = ConfigurationManager.AppSettings("ArchiviazioneDIRV__NomeFileIntestazione")
            Me._NomeFileMetadatiRichiesta = ConfigurationManager.AppSettings("ArchiviazioneDIRV__NomeFileMetadatiRichiesta")
            Me._NomeFileMetadatiRisposta = ConfigurationManager.AppSettings("ArchiviazioneDIRV__NomeFileMetadatiRisposta")
            Me._UrlServizioArchiviazione = ConfigurationManager.AppSettings("ArchiviazioneDIRV_UrlServizioArchiviazione")
        End Sub

    End Class

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Costruttore classe BizFirmaDigitale 
    ''' </summary>
    ''' <param name="genericprovider"></param>
    ''' <param name="settings"></param>
    ''' <param name="contextInfos"></param>
    ''' <remarks></remarks>
    Public Sub New(genericProvider As DbGenericProvider, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos)

        MyBase.New(genericProvider, settings, contextInfos, Nothing)

        Me.ParametriArchiviazioneDIRV = New ArchiviazioneDIRVConfigurationSettings()

    End Sub

    Public Sub New(dbGenericProviderFactory As DbGenericProviderFactory, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)

        MyBase.New(dbGenericProviderFactory, settings, Nothing, contextInfos, logOptions)

        Me.ParametriArchiviazioneDIRV = New ArchiviazioneDIRVConfigurationSettings()

    End Sub

#End Region

#Region " Types "

    Public Class CreaDocumentoResult
        Public ResultType As CreaDocumentoResultType
        Public Message As String
        Public IdDocumento As Long
        Public Documento As String
    End Class

    Public Enum CreaDocumentoResultType
        Success = 0
        ErroreRecuperoDati = 1
        ErroreCreazioneXML = 2
        ErroreCreazioneHTML = 3
        ErroreDocumentoFirmato = 4
    End Enum

    Public Class ArchiviazioneSostitutivaResult
        Public Success As Boolean
        Public Message As String
        Public CodiceErrore As String
        Public DescrizioneErrore As String
        Public TokenArchiviazione As String
    End Class

    Public Class Messages
        Public Const NO_EDIT_ANAMNESI_FIRMATA As String = "L'anamnesi selezionata è stata firmata digitalmente, non è possibile modificarla."
        Public Const CONFIRM_FIRMA As String = "Dopo la firma, il documento non sarà più modificabile.\nProcedere con la firma digitale del documento?"
        Public Const CONFIRM_FIRMA_MULTIPLA As String = "Dopo la firma, i documenti selezionati non saranno più modificabili.\nProcedere con la firma digitale dei documenti?"
        Public Const NO_DOCUMENTI_SELEZIONATI As String = "Nessun documento selezionato per la firma digitale."
        Public Const TUTTE_VISITE_FIRMATE As String = "Tutte le visite selezionate sono già state firmate."
        Public Const TUTTE_VISITE_SCARTATE As String = "Tutte le visite selezionate sono state scartate dai controlli."
    End Class

    Public Class StringWriterWithEncoding
        Inherits System.IO.StringWriter

        Private CustomEncoding As System.Text.Encoding

        Public Sub New(builder As System.Text.StringBuilder, encoding As System.Text.Encoding)
            MyBase.New(builder)
            Me.CustomEncoding = encoding
        End Sub

        Public Overrides ReadOnly Property Encoding As Text.Encoding
            Get
                Return Me.CustomEncoding
            End Get
        End Property

    End Class

#End Region

#Region " Public "

    'Me.ContextInfos.IDUtente => 4
    'Me.ContextInfos.IDApplicazione => OnVac_Veneto106
    'Me.ContextInfos.CodiceAzienda => 050000

    ''' <summary>
    ''' Crea il documento XML, lo inserisce in centrale e restituisce l'id
    ''' </summary>
    ''' <param name="idVisita"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreaDocumentoXMLAnamnesi(idVisita As Long) As CreaDocumentoResult

        Dim codiceUslCorrente As String = GetCodiceUslByIdApplicazione(ContextInfos.IDApplicazione)

        ' Dati che verranno inseriti in centrale
        Dim firmaDigitaleInfo As New ArchiviazioneDIRV.FirmaDigitaleInfo()
        firmaDigitaleInfo.IdVisita = idVisita
        firmaDigitaleInfo.TipoDocumento = Constants.TipoDocumentoFirmaDigitale.Visita

        firmaDigitaleInfo.IdUtenteInserimento = ContextInfos.IDUtente
        firmaDigitaleInfo.DataInserimento = DateTime.Now
        firmaDigitaleInfo.CodiceAziendaInserimento = codiceUslCorrente

        firmaDigitaleInfo.IdUtenteFirma = Nothing
        firmaDigitaleInfo.DataFirma = Nothing
        firmaDigitaleInfo.CodiceAziendaFirma = Nothing

        firmaDigitaleInfo.IdUtenteArchiviazione = Nothing
        firmaDigitaleInfo.DataArchiviazione = Nothing
        firmaDigitaleInfo.TokenArchiviazione = Nothing

        ' TODO [Unificazione Ulss]: GenericProviderCentrale andrà sostituito con il provider verso OnVac_050? La sostituzione non è immediata, ad esempio qui la t_doc_firma non è nello schema nuovo

        ' Ricerca in t_documenti_firma se è già presente un documento per la visita specificata:
        '  - se non c'è, lo creo;
        '  - se c'è ed è firmato => errore (non sovrascrivo un doc già firmato);
        '  - se c'è e non è firmato lo sovrascrivo con il nuovo documento (perchè i dati della visita potrebbero essere cambiati) e riutilizzo l'id.
        Dim documentoFirma As ArchiviazioneDIRV.DocumentoFirma =
            GenericProviderCentrale.FirmaDigitale.GetDocumentoByVisita(idVisita, codiceUslCorrente)

        If documentoFirma Is Nothing Then

            ' Documento non presente => calcolo nuovo id
            firmaDigitaleInfo.IdDocumento = GenericProviderCentrale.FirmaDigitale.GetNewIdDocumento()

        Else

            ' Documento già presente e firmato => errore
            If documentoFirma.IsDocumentoFirmato Then
                Return New CreaDocumentoResult() With
                {
                    .ResultType = CreaDocumentoResultType.ErroreDocumentoFirmato,
                    .Message = "Errore: il documento selezionato è già stato firmato.",
                    .IdDocumento = 0,
                    .Documento = Nothing
                }
            End If

            ' Documento già presente e non firmato => cancello il testo e riutilizzo l'id
            Me.GenericProviderCentrale.FirmaDigitale.DeleteDocumento(documentoFirma.IdDocumento)
            firmaDigitaleInfo.IdDocumento = documentoFirma.IdDocumento

        End If

        ' Recupero (da locale) i dati di visita, paziente e osservazioni (stessi dati della stampa del bilancio)
        Dim anamnesi As Anamnesi.Anamnesi = Nothing
        Using bizVisite As New BizVisite(Me.GenericProvider, Me.Settings, Me.ContextInfos, Me.LogOptions)
            anamnesi = bizVisite.GetAnamnesiByVisita(idVisita)
        End Using

        If anamnesi Is Nothing Then
            Return New CreaDocumentoResult() With
            {
                .ResultType = CreaDocumentoResultType.ErroreRecuperoDati,
                .Message = "Errore durante il recupero dei dati di anamnesi per la firma digitale.",
                .IdDocumento = 0,
                .Documento = Nothing
            }
        End If

        ' --- Creazione documento XML da firmare --- '
        Dim documentoXML As String = Me.CreateXMLDocument(anamnesi)

        If String.IsNullOrWhiteSpace(documentoXML) Then
            Return New CreaDocumentoResult() With
            {
                .ResultType = CreaDocumentoResultType.ErroreCreazioneXML,
                .Message = "Errore durante la creazione del file xml con i dati di anamnesi per la firma digitale.",
                .IdDocumento = 0,
                .Documento = Nothing
            }
        End If

        ' --- Inserimento in tabella centrale --- '
        Dim visita As Entities.Visita = Me.GenericProvider.Visite.GetVisitaById(idVisita)
        firmaDigitaleInfo.NomeFile = String.Format("anamnesi_{0:yyyyMMdd}_{1}.xml", visita.DataVisita, firmaDigitaleInfo.IdUtenteInserimento.Value.ToString())
        firmaDigitaleInfo.Documento = documentoXML

        Me.GenericProviderCentrale.FirmaDigitale.InsertDocumento(firmaDigitaleInfo, 0)

        ' N.B. : non faccio l'update della visita per memorizzare l'id del documento, lo faccio solo quando il documento verrà firmato.

        ' Tutto ok => restituisce id e documento
        Return New CreaDocumentoResult() With
        {
            .ResultType = CreaDocumentoResultType.Success,
            .Message = String.Empty,
            .IdDocumento = firmaDigitaleInfo.IdDocumento,
            .Documento = firmaDigitaleInfo.Documento
        }

    End Function

    ''' <summary>
    ''' Crea il documento di anamnesi relativo alla visita specificata, che dovrà essere firmato. 
    ''' Inserisce in centrale il documento in formato XML.
    ''' </summary>
    ''' <param name="idDocumento"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreaDocumentoHTMLAnamnesi(idDocumento As Long) As CreaDocumentoResult

        ' Controllo se documento già firmato
        If Me.IsDocumentoFirmato(idDocumento) Then
            Return New CreaDocumentoResult() With
            {
                .ResultType = CreaDocumentoResultType.ErroreDocumentoFirmato,
                .Message = "Errore: il documento selezionato è già stato firmato.",
                .IdDocumento = 0,
                .Documento = Nothing
            }
        End If

        ' Recupero xml da database
        Dim documentoXML As String = Me.GenericProviderCentrale.FirmaDigitale.GetDocumentoById(idDocumento)

        ' --- Trasformazione XML in HTML --- '
        Dim documentoHTML As String = Me.CreateHTMLDocument(documentoXML)

        If String.IsNullOrWhiteSpace(documentoHTML) Then
            Return New CreaDocumentoResult() With
            {
                .ResultType = CreaDocumentoResultType.ErroreCreazioneHTML,
                .Message = "Errore durante la creazione del file html con i dati di anamnesi per la firma digitale.",
                .IdDocumento = 0,
                .Documento = Nothing
            }
        End If

        ' Tutto ok => restituisce id e documento
        Return New CreaDocumentoResult() With
        {
            .ResultType = CreaDocumentoResultType.Success,
            .Message = String.Empty,
            .IdDocumento = idDocumento,
            .Documento = documentoHTML
        }

    End Function

    ''' <summary>
    ''' Restituisce il file xml da firmare in base all'id specificato.
    ''' Nel file verrà incluso lo stylesheet di default per la visualizzazione.
    ''' </summary>
    ''' <param name="idDocumento"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDocumentoDaFirmare(idDocumento As Long) As String

        Dim documentoXML As String = String.Empty

        Try
            ' XSLT
            Dim rm As New System.Resources.ResourceManager("Onit.OnAssistnet.OnVac.Biz.BizResources", System.Reflection.Assembly.GetExecutingAssembly())
            Dim firmaDigitaleAnamnesiXslt As String = rm.GetString("FirmaDigitaleAnamnesiXSLT")

            Dim xsltDocument As New Xml.XmlDocument()
            xsltDocument.LoadXml(firmaDigitaleAnamnesiXslt)

            ' XML
            Dim documentoXMLOriginale As String = Me.GenericProviderCentrale.FirmaDigitale.GetDocumentoById(idDocumento)

            Dim xmlDocument As New Xml.XmlDocument()
            xmlDocument.LoadXml(documentoXMLOriginale)

            ' Inserimento xslt in xml
            Dim xsltNode As Xml.XmlNode = xmlDocument.ImportNode(xsltDocument.ChildNodes(1), True)
            xmlDocument.ChildNodes(1).InsertBefore(xsltNode, xmlDocument.ChildNodes(1).FirstChild)

            Dim stylesheetId As String = xsltDocument.ChildNodes(1).Attributes("id").Value
            Dim data As String = String.Format("type=""text/xml"" href=""#{0}""", stylesheetId)

            ' Aggiunta del tag <?xml-stylesheet type="text/xml" href="#styleAnamnesi" ?>
            xmlDocument.InsertAfter(xmlDocument.CreateProcessingInstruction("xml-stylesheet", data), xmlDocument.FirstChild)

            documentoXML = xmlDocument.OuterXml

        Catch ex As Exception
            documentoXML = String.Empty
        End Try

        Return documentoXML

    End Function

    ''' <summary>
    ''' Restituisce il token di archiviazione del documento, rilasciato dal servizio di archiviazione-conservazione.
    ''' </summary>
    ''' <param name="idDocumento"></param>
    ''' <returns></returns>
    ''' <remarks>SOLO CENTRALE</remarks>
    Public Function GetTokenArchiviazioneDocumento(idDocumento As Long) As String

        Return Me.GenericProviderCentrale.FirmaDigitale.GetTokenArchiviazioneDocumento(idDocumento)

    End Function

    Public Class GetInfoFirmaArchiviazioneVisitaResult
        Public InfoFirmaArchiviazioneVisita As Entities.ArchiviazioneDIRV.InfoFirmaArchiviazioneVisita
        Public TokenArchiviazione As String
    End Class

    ''' <summary>
    ''' Restituisce le informazioni su firma digitale e archiviazione sostitutiva per la visita specificata, compreso il token di archiviazione.
    ''' </summary>
    ''' <param name="idVisita"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetInfoFirmaArchiviazioneVisita(idVisita As Long, codiceAziendaInserimento As String) As GetInfoFirmaArchiviazioneVisitaResult

        Dim result As New GetInfoFirmaArchiviazioneVisitaResult()

        ' AppId relativo alla usl di inserimento specificata
        Dim appIdAziendaInserimento As String = GetIdApplicazioneByCodiceUsl(codiceAziendaInserimento)

        result.InfoFirmaArchiviazioneVisita = GenericProvider.Visite.GetInfoFirmaArchiviazioneVisita(idVisita, appIdAziendaInserimento)

        If result.InfoFirmaArchiviazioneVisita Is Nothing Then
            result.InfoFirmaArchiviazioneVisita = New ArchiviazioneDIRV.InfoFirmaArchiviazioneVisita()
        Else
            If result.InfoFirmaArchiviazioneVisita.IdDocumento.HasValue Then
                result.TokenArchiviazione = GetTokenArchiviazioneDocumento(result.InfoFirmaArchiviazioneVisita.IdDocumento.Value)
            End If
        End If

        Return result

    End Function

    ''' <summary>
    ''' Modifiche ai dati relativi alla firma digitale del documento (documento firmato, utente che ha firmato, data di firma) 
    ''' sia nella tabella centrale dei documenti che nella tabella locale della visita relativa.
    ''' </summary>
    ''' <param name="idDocumento"></param>
    ''' <param name="documentoFirmato"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateDocumentoFirmato(idDocumento As Long, documentoFirmato As String) As Boolean

        Dim ownTransaction As Boolean = False

        ' Update t_documenti_firma (centrale) con i dati di firma (no dati di archiviazione)
        ' Update t_vis_visite (locale) per i dati di firma (no dati di archiviazione)
        Try
            Dim firmaDigitaleInfo As New ArchiviazioneDIRV.FirmaDigitaleInfo()

            firmaDigitaleInfo.IdDocumento = idDocumento
            firmaDigitaleInfo.Documento = documentoFirmato
            firmaDigitaleInfo.IdVisita = GenericProviderCentrale.FirmaDigitale.GetIdVisitaByIdDocumento(idDocumento)

            firmaDigitaleInfo.IdUtenteFirma = ContextInfos.IDUtente
            firmaDigitaleInfo.DataFirma = DateTime.Now
            firmaDigitaleInfo.CodiceAziendaFirma = GetCodiceUslByIdApplicazione(ContextInfos.IDApplicazione)

            firmaDigitaleInfo.IdUtenteArchiviazione = Nothing
            firmaDigitaleInfo.DataArchiviazione = Nothing
            firmaDigitaleInfo.TokenArchiviazione = Nothing

            If GenericProviderCentrale.Transaction Is Nothing AndAlso GenericProvider.Transaction Is Nothing Then
                GenericProviderCentrale.BeginTransaction()
                GenericProvider.BeginTransaction()
                ownTransaction = True
            End If

            ' Modifiche alla t_documenti_firma (centrale)
            GenericProviderCentrale.FirmaDigitale.UpdateDatiFirmaDocumento(firmaDigitaleInfo)

            ' Modifiche alla t_vis_visite (locale)
            GenericProvider.Visite.UpdateInfoFirmaDigitaleVisita(firmaDigitaleInfo)

            If ownTransaction Then
                GenericProviderCentrale.Commit()
                GenericProvider.Commit()
            End If

        Catch ex As Exception

            If ownTransaction Then
                GenericProviderCentrale.Rollback()
                GenericProvider.Rollback()
            End If

            ex.InternalPreserveStackTrace()
            Throw

        End Try

        Return True

    End Function

    ''' <summary>
    ''' Cancellazione documenti relativi alle visite specificate.
    ''' </summary>
    ''' <param name="listIdVisita"></param>
    ''' <returns></returns>
    ''' <remarks>SOLO CENTRALE</remarks>
    Public Function DeleteDocumentiVisite(listIdVisita As List(Of Long)) As Integer

        Return GenericProviderCentrale.FirmaDigitale.DeleteDocumentiVisite(listIdVisita)

    End Function

    ''' <summary>
    ''' Invio documento in archiviazione sostitutiva e aggiornamento dati relativi all'archiviazione sia in centrale che in locale
    ''' </summary>
    ''' <param name="idDocumentoDaArchiviare"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function InvioAnamnesiArchiviazioneSostitutiva(idDocumentoDaArchiviare As Long) As ArchiviazioneSostitutivaResult

        Dim documentoDaArchiviare As String = Me.GenericProviderCentrale.FirmaDigitale.GetDocumentoById(idDocumentoDaArchiviare)
        Dim nomeFileDaArchiviare As String = Me.GenericProviderCentrale.FirmaDigitale.GetNomeFileByIdDocumento(idDocumentoDaArchiviare)

        ' N.B.: isContentCrypted = true se il contenuto è criptato, false se è in chiaro.
        '                          Per test senza smart card => false
        Dim archiviazioneResult As ArchiviazioneSostitutivaResult =
            InvioDocumentoArchiviazioneSostitutiva(documentoDaArchiviare, nomeFileDaArchiviare, True)

        ' [Firma Digitale]: il servizio di archiviazione è stato dismesso
        archiviazioneResult.Success = False
        archiviazioneResult.Message = "Servizio archiviazione non presente"
        archiviazioneResult.DescrizioneErrore = "Servizio archiviazione non presente"

        UpdateDocumentoArchiviato(idDocumentoDaArchiviare, archiviazioneResult, ContextInfos.IDUtente, archiviazioneResult.Success, Now)

        Return archiviazioneResult

    End Function

#Region " Firma massiva "

    Public Class ControlliFirmaMassivaResult

        Public Success As Boolean
        Public Message As String

        Public CountVisiteTotali As Integer
        Public CountVisiteScartateGiaFirmate As Integer
        Public CountVisiteScartateUslInserimentoDiversa As Integer
        Public CountVisiteScartateErroreCreazioneDocumenti As Integer

        Public IdDocumentiDaFirmare As Long()

        Public Sub New()
            Me.IdDocumentiDaFirmare = New Long() {}
        End Sub

    End Class

    Private Function GetResultMessage(result As ControlliFirmaMassivaResult) As String

        Dim message As New System.Text.StringBuilder()

        message.AppendFormat("Visite selezionate: {0}", result.CountVisiteTotali.ToString())
        message.AppendLine()

        message.AppendFormat("Visite già firmate: {0}", result.CountVisiteScartateGiaFirmate.ToString())
        message.AppendLine()

        If result.CountVisiteScartateErroreCreazioneDocumenti > 0 Then
            message.AppendFormat("Visite non firmate (errori durante la creazione del documento da firmare): {0}", result.CountVisiteScartateErroreCreazioneDocumenti.ToString())
            message.AppendLine()
        End If

        Dim visiteDaFirmare As Integer = result.CountVisiteTotali - (result.CountVisiteScartateErroreCreazioneDocumenti + result.CountVisiteScartateGiaFirmate + result.CountVisiteScartateUslInserimentoDiversa)
        If visiteDaFirmare > 0 Then
            message.AppendLine()
            message.AppendFormat("Visite da firmare: {0}", visiteDaFirmare.ToString())
            message.AppendLine()
        End If

        Return message.ToString()

    End Function

    ''' <summary>
    ''' Metodo per firma massiva. Effettua i controlli sulle visite specificate, 
    ''' per scartare eventuali documenti già firmati o creati da altre ULSS.
    ''' Per le visite rimaste, crea i documenti XML relativi e li inserisce in centrale. 
    ''' Restituisce un array contenente gli id dei documenti creati, che dovranno essere firmati.
    ''' </summary>
    ''' <param name="listIdVisite"></param>
    ''' <param name="utenteLoggatoIsMedico"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreaDocumentiXMLAnamnesi(listIdVisite As List(Of Long), utenteLoggatoIsMedico As Boolean) As ControlliFirmaMassivaResult

        Dim result As New ControlliFirmaMassivaResult()
        result.CountVisiteScartateUslInserimentoDiversa = 0
        result.CountVisiteScartateErroreCreazioneDocumenti = 0

        If listIdVisite Is Nothing OrElse listIdVisite.Count = 0 Then
            result.Success = False
            result.Message = Messages.NO_DOCUMENTI_SELEZIONATI
            Return result
        End If

        Dim listVisite As List(Of Visita) = GenericProvider.Visite.GetVisiteById(listIdVisite)

        If listVisite Is Nothing OrElse listVisite.Count = 0 Then
            result.Success = False
            result.Message = Messages.NO_DOCUMENTI_SELEZIONATI
            Return result
        End If

        ' Visite totali selezionate
        result.CountVisiteTotali = listVisite.Count

        ' Visite da firmare
        Dim listVisiteDaControllare As List(Of Visita) = listVisite.Where(Function(p) Not p.IdUtenteFirmaDigitale.HasValue).ToList()

        If listVisiteDaControllare Is Nothing OrElse listVisiteDaControllare.Count = 0 Then
            result.CountVisiteScartateGiaFirmate = result.CountVisiteTotali
            result.Success = False
            result.Message = Messages.TUTTE_VISITE_FIRMATE
            Return result
        End If

        ' Visite già firmate
        result.CountVisiteScartateGiaFirmate = result.CountVisiteTotali - listVisiteDaControllare.Count

        Dim listVisiteDaFirmare As New List(Of Visita)()

        For Each visita As Visita In listVisiteDaControllare

            ' ------------------------------------ '
            ' [Unificazione Ulss]: Eliminato controllo usl inserimento
            ' ------------------------------------ '
            '' Controllo usl inserimento visita == usl corrente
            'If flagAbilitazioneVaccUslCorrente Then

            '    If Not String.IsNullOrEmpty(visita.CodiceUslInserimento) AndAlso Me.Settings.CODICE_ASL <> visita.CodiceUslInserimento Then
            '        result.CountVisiteScartateUslInserimentoDiversa += 1
            '        Continue For
            '    End If

            'End If
            ' ------------------------------------ '

            listVisiteDaFirmare.Add(visita.Clone)

        Next

        If listVisiteDaFirmare.Count = 0 Then
            result.Success = False
            result.Message = Messages.TUTTE_VISITE_SCARTATE + Environment.NewLine + Me.GetResultMessage(result)
            Return result
        End If

        Dim listIdDocumenti As New List(Of Long)()

        ' Creazione documenti XML per ogni visita e inserimento documenti da firmare in centrale
        For Each visitaDaFirmare As Visita In listVisiteDaFirmare

            Dim creaDocumentoResult As CreaDocumentoResult = CreaDocumentoXMLAnamnesi(visitaDaFirmare.IdVisita)

            If creaDocumentoResult.ResultType = CreaDocumentoResultType.Success Then
                listIdDocumenti.Add(creaDocumentoResult.IdDocumento)
            Else
                result.CountVisiteScartateErroreCreazioneDocumenti += 1
            End If

        Next

        If listIdDocumenti.Count = 0 Then
            result.Success = False
        Else
            result.Success = True
            result.IdDocumentiDaFirmare = listIdDocumenti.ToArray()
        End If

        result.Message = GetResultMessage(result)

        Return result

    End Function

#End Region

#Region " Batch archiviazione sostitutiva documenti "

    Public Class BatchArchiviazioneSostitutiva

        Public Class ParameterName
            Public Const IdApplicazioneLocale As String = "IdApplicazioneLocale"
            Public Const IdApplicazioneCentrale As String = "IdApplicazioneCentrale"
            Public Const IdUtenteArchiviazione As String = "IdUtenteArchiviazione"
        End Class

    End Class

    ''' <summary>
    ''' Avvia il processo batch di invio dei documenti in attesa al servizio di archiviazione sostitutiva.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function StartBatchArchiviazioneSostitutiva() As BizGenericResult

        Dim appIdLocale As String = ContextInfos.IDApplicazione
        Dim appIdCentrale As String = Settings.APP_ID_CENTRALE

        Dim command As New BizBatch.StartBatchProcedureCommand()
        command.ProcedureId = ID_PROCESSO_ARCHIVIAZIONE_SOSTITUTIVA
        command.ProcedureDescription = "Processo di invio documenti al servizio di archiviazione sostitutiva"
        command.StartingAppId = appIdLocale
        command.StartingCodiceAzienda = ContextInfos.CodiceAzienda
        command.StartingUserId = ContextInfos.IDUtente

        command.ListAppIdConnections.Add(appIdLocale)
        command.ListAppIdConnections.Add(appIdCentrale)

        command.ListParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(BatchArchiviazioneSostitutiva.ParameterName.IdApplicazioneLocale, appIdLocale))
        command.ListParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(BatchArchiviazioneSostitutiva.ParameterName.IdApplicazioneCentrale, appIdCentrale))
        command.ListParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(BatchArchiviazioneSostitutiva.ParameterName.IdUtenteArchiviazione, ContextInfos.IDUtente))

        Dim bizBatch As New BizBatch(GenericProvider)

        Return bizBatch.StartBatchProcedure(command)

    End Function

    Public Event RefreshTotaleDocumentiDaArchiviare(e As RefreshTotaleDocumentiDaArchiviareEventArgs)
    Public Event RefreshDocumentiArchiviati(e As RefreshDocumentiArchiviatiEventArgs)

    Public Class RefreshTotaleDocumentiDaArchiviareEventArgs
        Inherits EventArgs

        Public ReadOnly Property TotaleDocumentiDaArchiviare As Integer

        Public Sub New(totaleDocumentiDaArchiviare As Integer)
            _TotaleDocumentiDaArchiviare = totaleDocumentiDaArchiviare
        End Sub

    End Class

    Public Class RefreshDocumentiArchiviatiEventArgs
        Inherits EventArgs

        Public ReadOnly Property NumeroDocumentiArchiviati As Integer
        Public ReadOnly Property NumeroErrori As Integer

        Public Sub New(numeroDocumentiArchiviati As Integer, numeroErrori As Integer)
            _NumeroDocumentiArchiviati = numeroDocumentiArchiviati
            _NumeroErrori = numeroErrori
        End Sub

    End Class

    ''' <summary>
    ''' Richiama il servizio di archiviazione sostitutiva per tutti i documenti di anamnesi non ancora archiviati
    ''' </summary>
    ''' <param name="millisecondsSleepTime"></param>
    ''' <remarks></remarks>
    Public Sub ArchiviazioneSostitutivaAnamnesi(millisecondsSleepTime As Integer)

        Dim now As DateTime = DateTime.Now
        Dim codiceAzienda As String = GetCodiceUslByIdApplicazione(ContextInfos.IDApplicazione)

        Dim numeroDocumentiArchiviati As Integer = 0
        Dim numeroErrori As Integer = 0

        Try
            ' Ricerca in centrale delle anamnesi da archiviare (già firmate ma non archiviate)
            Dim listIdDocumenti As List(Of Long) = GenericProviderCentrale.FirmaDigitale.GetListIdDocumentiDaArchiviare(codiceAzienda)

            RaiseEvent RefreshTotaleDocumentiDaArchiviare(New RefreshTotaleDocumentiDaArchiviareEventArgs(listIdDocumenti.Count))

            If listIdDocumenti.Count > 0 Then

                Dim refreshStatusAfter As Integer = (listIdDocumenti.Count * 20 \ 100) + 1

                For i As Integer = 0 To listIdDocumenti.Count - 1

                    Dim documentoDaArchiviare As String = GenericProviderCentrale.FirmaDigitale.GetDocumentoById(listIdDocumenti(i))

                    If String.IsNullOrWhiteSpace(documentoDaArchiviare) Then

                        ' Documento vuoto => lo considero errore
                        numeroErrori += 1

                    Else

                        Try
                            Dim result As ArchiviazioneSostitutivaResult = InvioAnamnesiArchiviazioneSostitutiva(listIdDocumenti(i))

                            If result.Success Then
                                numeroDocumentiArchiviati += 1
                            Else
                                numeroErrori += 1
                            End If

                        Catch ex As Exception
                            numeroErrori += 1
                        End Try

                    End If

                    If (i + 1) Mod refreshStatusAfter = 0 Then
                        RaiseEvent RefreshDocumentiArchiviati(New RefreshDocumentiArchiviatiEventArgs(numeroDocumentiArchiviati, numeroErrori))
                    End If

                    If millisecondsSleepTime > 0 Then System.Threading.Thread.Sleep(millisecondsSleepTime)

                Next
            End If

        Finally
            RaiseEvent RefreshDocumentiArchiviati(New RefreshDocumentiArchiviatiEventArgs(numeroDocumentiArchiviati, numeroErrori))
        End Try

    End Sub

#End Region

#Region " Gestione documentale "

    Public Class ParametriGetDocumentiVisiteBiz

        Public Filtri As ArchiviazioneDIRV.FiltriDocumentiVisite
        Public CampoOrdinamento As String
        Public VersoOrdinamento As String
        Public PageIndex As Integer
        Public PageSize As Integer

    End Class

    <Serializable>
    Public Structure GetDocumentiVisiteResult
        Public DocumentiVisite As List(Of ArchiviazioneDIRV.DocumentoVisita)
        Public CountDocumenti As Integer
    End Structure

    Public Function GetDocumentiVisite(param As ParametriGetDocumentiVisiteBiz) As GetDocumentiVisiteResult

        If param Is Nothing Then Throw New ArgumentNullException()

        Dim paramToPass As New ParametriGetDocumentiVisite()
        paramToPass.Filtri = param.Filtri
        paramToPass.PagingOpts = New Data.PagingOptions()
        paramToPass.PagingOpts.StartRecordIndex = param.PageIndex * param.PageSize
        paramToPass.PagingOpts.EndRecordIndex = paramToPass.PagingOpts.StartRecordIndex + param.PageSize

        paramToPass.OrderBy = GetOrderByDocumentiVisite(param.CampoOrdinamento, param.VersoOrdinamento)

        'Recupero gli elementi filtrati
        Dim lstDocumentiVisite As List(Of ArchiviazioneDIRV.DocumentoVisita) =
            GenericProvider.FirmaDigitale.GetDocumentiVisite(paramToPass)

        'Recupero il count degli elementi
        Dim countDocumentiVisite As Integer =
            GenericProvider.FirmaDigitale.CountDocumentiVisite(param.Filtri)

        Dim getDocumentiVisiteResult As New GetDocumentiVisiteResult()
        getDocumentiVisiteResult.DocumentiVisite = lstDocumentiVisite
        getDocumentiVisiteResult.CountDocumenti = countDocumentiVisite

        Return getDocumentiVisiteResult

    End Function

    ''' <summary>
    ''' Restituisce gli id delle visite in base ai filtri specificati
    ''' </summary>
    ''' <param name="filtri"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetIdVisiteDocumenti(filtri As ArchiviazioneDIRV.FiltriDocumentiVisite) As List(Of Long)

        ' Filtri obbligatori non impostati => restituisco una lista vuota
        If filtri Is Nothing OrElse filtri.DataDa = DateTime.MinValue OrElse filtri.DataA = DateTime.MinValue OrElse String.IsNullOrWhiteSpace(filtri.UslCorrente) Then
            Return New List(Of Long)()
        End If

        Return GenericProvider.FirmaDigitale.GetIdVisiteDocumenti(filtri)

    End Function

    ''' <summary>
    ''' Restituisce true se il documento specificato è firmato digitalmente. Effettua il controllo nella tabella centrale dei documenti.
    ''' </summary>
    ''' <param name="idDocumento"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsDocumentoFirmato(idDocumento As Long) As Boolean

        Dim idUtenteFirma As Long? = GenericProviderCentrale.FirmaDigitale.GetIdUtenteFirma(idDocumento)

        Return idUtenteFirma.HasValue

    End Function

#End Region

#End Region

#Region " Private "

    ''' <summary>
    ''' Crea il documento XML in base ai dati di anamnesi
    ''' </summary>
    ''' <param name="anamnesi"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreateXMLDocument(anamnesi As Anamnesi.Anamnesi) As String

        Dim documentoXML As String = String.Empty

        Try
            ' Creazione XML con dati di anamnesi
            Dim documentoXmlAnamnesi As New Text.StringBuilder()

            Dim xmlSerializer As New Xml.Serialization.XmlSerializer(GetType(Anamnesi.Anamnesi))

            Using writerXML As New StringWriterWithEncoding(documentoXmlAnamnesi, Text.Encoding.UTF8)

                xmlSerializer.Serialize(writerXML, anamnesi)

            End Using

            documentoXML = documentoXmlAnamnesi.ToString()

        Catch ex As Exception
            Common.Utility.EventLogHelper.EventLogWrite(ex, "Eccezione in CreateXMLDocument per firma digitale", EventLogEntryType.Information, ContextInfos.IDApplicazione)
            documentoXML = String.Empty
        End Try

        Return documentoXML

    End Function

    ''' <summary>
    ''' Crea il documento HTML ottenuto trasformando l'XML specificato
    ''' </summary>
    ''' <param name="documentoXML"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreateHTMLDocument(documentoXML As String) As String

        Dim documentoHTML As String = String.Empty

        Try
            ' Trasformazione XML + XSLT => HTML
            Using stream As New IO.MemoryStream(System.Text.UTF8Encoding.Default.GetBytes(documentoXML))

                Dim document As New System.Xml.XPath.XPathDocument(stream)

                Using writerHTML As New System.IO.StringWriter()

                    Dim rm As New System.Resources.ResourceManager("Onit.OnAssistnet.OnVac.Biz.BizResources", System.Reflection.Assembly.GetExecutingAssembly())
                    Dim firmaDigitaleAnamnesiXSLT As String = rm.GetString("FirmaDigitaleAnamnesiXSLT")

                    Using stringReader As New System.IO.StringReader(firmaDigitaleAnamnesiXSLT)
                        Using xmlReader As System.Xml.XmlReader = System.Xml.XmlReader.Create(stringReader)

                            Dim transform As New System.Xml.Xsl.XslCompiledTransform()
                            transform.Load(xmlReader)
                            transform.Transform(document, Nothing, writerHTML)

                            documentoHTML = writerHTML.ToString()

                        End Using
                    End Using
                End Using

            End Using

        Catch ex As Exception
            Common.Utility.EventLogHelper.EventLogWrite(ex, " CreateHTMLDocument ", ContextInfos.IDApplicazione)
            documentoHTML = String.Empty
        End Try

        Return documentoHTML

    End Function

    ''' <summary>
    ''' Il documento specificato viene inviato al servizio di archiviazione sostitutiva.
    ''' Se tutto ok, viene restituito un oggetto contenente il token di archiviazione.
    ''' </summary>
    ''' <param name="documentoDaArchiviare"></param>
    ''' <param name="nomeFileDaArchiviare"></param>
    ''' <param name="isContentCrypted"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function InvioDocumentoArchiviazioneSostitutiva(documentoDaArchiviare As String, nomeFileDaArchiviare As String, isContentCrypted As Boolean) As ArchiviazioneSostitutivaResult

        ' [Firma Digitale]: il servizio di archiviazione è stato dismesso

        Return New ArchiviazioneSostitutivaResult() With {
            .Success = True,
            .Message = String.Empty
        }

        'Dim nomeFileFirmato As String = nomeFileDaArchiviare
        'If isContentCrypted Then nomeFileFirmato += ".p7m"

        'Dim msg As New DIRV.ArchiviazioneSostitutiva.ArchiviazioneSostitutivaMessage()

        '' Dati intestazione
        'msg.Intestazione.Mittente = ParametriArchiviazioneDIRV.Mittente
        'msg.Intestazione.Destinatario = ParametriArchiviazioneDIRV.Destinatario
        'msg.Intestazione.UserId = ParametriArchiviazioneDIRV.UserId
        'msg.Intestazione.Password = ParametriArchiviazioneDIRV.Password

        '' Caricamento del documento firmato
        'Dim documentoFirmato As New Onit.OnAssistnet.OnVac.DIRV.ArchiviazioneSostitutiva.DocumentoFirmato(nomeFileFirmato, documentoDaArchiviare, isContentCrypted)
        'msg.Documenti.Add(documentoFirmato)

        '' Metadati
        'msg.Metadati.ente = ParametriArchiviazioneDIRV.Ente
        'msg.Metadati.areaOrg = ParametriArchiviazioneDIRV.AreaOrg

        'If String.IsNullOrWhiteSpace(ParametriArchiviazioneDIRV.EndUserId) Then
        '    msg.Metadati.endUserId = String.Format("{0}_{1}", ContextInfos.IDApplicazione, ContextInfos.IDUtente)
        'End If

        'Dim fileItem As New fileItem()
        'fileItem.fileName = nomeFileFirmato             ' nome file originale
        'fileItem.fileHashType = "SHA-1"                 ' algoritmo di calcolo dell’impronta del file (attualmente supportato solo SHA-1).  
        'fileItem.fileHash = documentoFirmato.Hash       ' impronta codificata esadecimale
        'fileItem.fileMime = String.Format("{0}|{1}", Constants.MIMEContentType.APPLICATION_PKCS7, Constants.MIMEContentType.TEXT_XML)    ' mime type del file
        'fileItem.fileMimeVersion = "NA|NA"              ' versione del mime type del file o NA (Not Allowed) se il mime non ha una versione significativa.
        'fileItem.fileType = fileType.D                  ' tipologia del file: D = file del documento X = file dei metadati del documento

        'msg.Metadati.documents = New document() {
        '    New document() With
        '    {
        '        .idClassificazione = ParametriArchiviazioneDIRV.IdClassificazione,
        '        .fileVector = New fileItem() {fileItem}
        '    }
        '}

        '' Invio del messaggio
        'Dim result As New ArchiviazioneSostitutivaResult()

        'Try
        '    Dim response As ArchiviazioneSostitutivaResponse =
        '        DirvClient.Send(ParametriArchiviazioneDIRV.UrlServizioArchiviazione, msg, ParametriArchiviazioneDIRV.NomeFileIntestazione,
        '                        ParametriArchiviazioneDIRV.NomeFileMetadatiRichiesta, ParametriArchiviazioneDIRV.NomeFileMetadatiRisposta)

        '    If Not response Is Nothing AndAlso Not response.MetadatiRisposta Is Nothing AndAlso response.MetadatiRisposta.Items.Count > 0 Then

        '        ' N.B. : viene considerato solo il primo messaggio di risposta
        '        Dim rispostaArchiviazione As risposta_archiviazione_conservazioneDocumentsDocumentRisposta =
        '            response.MetadatiRisposta.Items.First().document.First().risposta(0)

        '        If rispostaArchiviazione.codiceErrore = CodiceErroreRispostaServizio.NESSUN_ERRORE Then
        '            result.Success = True
        '            result.Message = rispostaArchiviazione.descrizioneErrore
        '            result.TokenArchiviazione = rispostaArchiviazione.tokenRV
        '        Else
        '            result.Success = False
        '            result.Message = rispostaArchiviazione.descrizioneErrore
        '            result.CodiceErrore = rispostaArchiviazione.codiceErrore
        '            result.DescrizioneErrore = rispostaArchiviazione.descrizioneErrore
        '        End If

        '    Else
        '        result.Success = False
        '        result.Message = "Archiviazione documento non effettuata: errore durante l'invio al servizio."
        '        result.CodiceErrore = CodiceErroreRispostaServizio.ERRORE_INVIO
        '        result.DescrizioneErrore = response.SwaResponse.SoapEnvelope
        '    End If

        'Catch ex As Exception
        '    result.Success = False
        '    result.Message = "Archiviazione documento non effettuata: " + ex.Message
        '    result.CodiceErrore = CodiceErroreRispostaServizio.ECCEZIONE
        '    result.DescrizioneErrore = ex.ToString()
        'End Try

        'Return result

    End Function

    ''' <summary>
    ''' Modifiche ai dati di archiviazione 
    ''' </summary>
    ''' <param name="idDocumento"></param>
    ''' <param name="archiviazioneResult"></param>
    ''' <param name="idUtenteArchiviazione"></param>
    ''' <param name="dataArchiviazione"></param>
    ''' <param name="eliminaDocumentoFirmato"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdateDocumentoArchiviato(idDocumento As Long, archiviazioneResult As ArchiviazioneSostitutivaResult, idUtenteArchiviazione As Long, eliminaDocumentoFirmato As Boolean, dataArchiviazione As DateTime) As Boolean

        Dim ownTransaction As Boolean = False

        Try
            Dim firmaDigitaleInfo As New ArchiviazioneDIRV.FirmaDigitaleInfo()

            firmaDigitaleInfo.IdDocumento = idDocumento
            firmaDigitaleInfo.IdVisita = GenericProviderCentrale.FirmaDigitale.GetIdVisitaByIdDocumento(idDocumento)

            firmaDigitaleInfo.Documento = Nothing
            firmaDigitaleInfo.IdUtenteFirma = Nothing
            firmaDigitaleInfo.DataFirma = Nothing
            firmaDigitaleInfo.CodiceAziendaFirma = Nothing

            ' Se l'archiviazione ha avuto successo, salvo i dati di archiviazione (utente, data, token) e sbianco i due campi codice e descrizione dell'errore.
            ' Se non è andata a buon fine, invece, salvo codice e descrizione dell'errore (se indicati).
            If archiviazioneResult.Success Then
                firmaDigitaleInfo.IdUtenteArchiviazione = idUtenteArchiviazione
                firmaDigitaleInfo.DataArchiviazione = dataArchiviazione
                firmaDigitaleInfo.TokenArchiviazione = archiviazioneResult.TokenArchiviazione
                firmaDigitaleInfo.CodiceErroreDIRV = Nothing
                firmaDigitaleInfo.DescrizioneErroreDIRV = Nothing
            Else
                firmaDigitaleInfo.CodiceErroreDIRV = archiviazioneResult.CodiceErrore
                If archiviazioneResult.DescrizioneErrore.Length > 4000 Then
                    firmaDigitaleInfo.DescrizioneErroreDIRV = archiviazioneResult.DescrizioneErrore.Substring(0, 4000)
                Else
                    firmaDigitaleInfo.DescrizioneErroreDIRV = archiviazioneResult.DescrizioneErrore
                End If
            End If

            If GenericProviderCentrale.Transaction Is Nothing AndAlso GenericProvider.Transaction Is Nothing Then
                GenericProviderCentrale.BeginTransaction()
                GenericProvider.BeginTransaction()
                ownTransaction = True
            End If

            ' Modifiche alla t_documenti_firma (centrale)
            GenericProviderCentrale.FirmaDigitale.UpdateDatiArchiviazioneDocumento(firmaDigitaleInfo, True, eliminaDocumentoFirmato)

            If archiviazioneResult.Success Then
                ' Modifiche alla t_vis_visite (locale)
                GenericProvider.Visite.UpdateInfoArchiviazioneSostitutivaVisita(firmaDigitaleInfo)
            End If

            If ownTransaction Then
                GenericProviderCentrale.Commit()
                GenericProvider.Commit()
            End If

        Catch ex As Exception

            If ownTransaction Then
                GenericProviderCentrale.Rollback()
                GenericProvider.Rollback()
            End If

            ex.InternalPreserveStackTrace()
            Throw

        End Try

        Return True

    End Function

    Private Function GetOrderByDocumentiVisite(campoOrdinamento As String, versoOrdinamento As String) As String

        'Ordinamento di default
        Dim orderBy As String = "vis_data_visita DESC, paz_cognome ASC, paz_nome ASC, paz_data_nascita ASC "

        'Ordinamento manuale
        If Not String.IsNullOrWhiteSpace(campoOrdinamento) Then

            'Aggiornamento campoOrdinamento
            Select Case campoOrdinamento
                Case "PazienteInfo"
                    orderBy = String.Format("paz_cognome {0}, paz_nome {0}, paz_data_nascita {0}", versoOrdinamento)
                Case "DataVisita"
                    orderBy = String.Format("vis_data_visita {0}", versoOrdinamento)
                Case "UtenteVisita"
                    orderBy = String.Format("ute_visita {0}", versoOrdinamento)
                Case "UtenteFirma"
                    orderBy = String.Format("ute_firma {0}", versoOrdinamento)
                Case "UtenteRilevatore"
                    orderBy = String.Format("ute_rilevatore {0}", versoOrdinamento)
                Case Else
            End Select

        End If

        Return orderBy

    End Function

#End Region

End Class
