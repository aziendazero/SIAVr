Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Enumerators
Imports Onit.OnAssistnet.OnVac.Entities

Public Class BizFSE
    Inherits BizClass

#Region " Constructors "

    Public Sub New(genericProvider As DbGenericProvider, settings As Settings.Settings, contextInfo As BizContextInfos)

        MyBase.New(genericProvider, settings, contextInfo, Nothing)

    End Sub

#End Region

#Region " Log Notifiche "

    ''' <summary>
    ''' Recupera le notifiche da inviare ad FSE.
    ''' </summary>
    ''' <param name="maxRecord">0 per non impostare nessun limite</param>
    ''' <returns></returns>
    Public Function GetLogNotificheDaInviareFSE(maxRecord As Integer?) As List(Of LogNotificaInviata)

        Return GenericProvider.LogNotifiche.GetLogNotificheDaInviareFSE(If(maxRecord, 0))

    End Function

#End Region

#Region " Calcolo dati da inviare "

    Public Function GetDocumentUniqueId(codicePaziente As Long, codiceUslCorrente As String) As String

        Dim codiceUsl As String = codiceUslCorrente

		If String.IsNullOrWhiteSpace(codiceUsl) Then
			codiceUsl = ContextInfos.CodiceUsl
		End If
		If Not String.IsNullOrWhiteSpace(codiceUsl) Then
			codiceUsl = codiceUsl.Substring(1)
		End If


		Return String.Format("{0}^{1}.{2}",
                             String.Format(Settings.FSE_OID_DOCUMENTI, codiceUsl),
                             Settings.FSE_TIPO_DOC_CERTIFICATO_VACCINALE,
                             codicePaziente.ToString())

    End Function

    Public Function GetRepositoryUniqueId(codiceUslCorrente As String) As String

        Dim codiceUsl As String = codiceUslCorrente

        If String.IsNullOrWhiteSpace(codiceUsl) Then
            codiceUsl = ContextInfos.CodiceUsl
        End If

        Dim uslUnificata As UslUnificata = GenericProvider.Usl.GetUslUnificataByCodiceUsl(codiceUslCorrente)

        If Not uslUnificata Is Nothing Then
            Return uslUnificata.RepositoryUniqueId
        End If

        Return String.Empty

    End Function

    Public Function GetPazienteCDA(codicePaziente As Long) As FSErIndicizzazione.pazienteCDA

        Dim pazFSE As New FSErIndicizzazione.pazienteCDA()
        Dim pazienti As Collection.PazienteCollection = GenericProvider.Paziente.GetPazienti(codicePaziente.ToString())

        If pazienti Is Nothing OrElse pazienti.Count() = 0 Then
            Return pazFSE
        End If

        Dim pazVac As Paziente = pazienti(0)

        pazFSE.codiceCittadinanza = pazVac.Cittadinanza_Codice
        pazFSE.codiceCns = pazVac.Paz_Cns_Codice
        pazFSE.codiceFiscale = pazVac.PAZ_CODICE_FISCALE
        pazFSE.codicePaziente = pazVac.Paz_Codice
        pazFSE.codiceRegionale = pazVac.PAZ_CODICE_REGIONALE
        pazFSE.codiceAusiliario = pazVac.CodiceAusiliario
        pazFSE.cognome = pazVac.PAZ_COGNOME
        pazFSE.nome = pazVac.PAZ_NOME

        If String.IsNullOrWhiteSpace(pazVac.ComuneDomicilio_Codice) Then

            pazFSE.comuneDomicilio = New FSErIndicizzazione.comuneCDA()

        Else

            Dim comuneDom As Comune = GenericProvider.Comuni.GetComuneByCodice(pazVac.ComuneDomicilio_Codice)

            pazFSE.comuneDomicilio = New FSErIndicizzazione.comuneCDA() With
            {
                .cap = pazVac.ComuneDomicilio_Cap,
                .codice = pazVac.ComuneDomicilio_Codice,
                .descrizione = pazVac.ComuneDomicilio_Descrizione,
                .provincia = pazVac.ComuneDomicilio_Provincia,
                .indirizzo = pazVac.IndirizzoDomicilio,
                .codiceIstat = comuneDom.Istat
            }

        End If

        If String.IsNullOrWhiteSpace(pazVac.ComuneNascita_Codice) Then

            pazFSE.comuneNascita = New FSErIndicizzazione.comuneCDA()

        Else

            Dim comNascita As Comune = GenericProvider.Comuni.GetComuneByCodice(pazVac.ComuneNascita_Codice)

            ' N.B. : valorizzare il codice stato a vuoto altrimenti il servizio dà errore!
            pazFSE.comuneNascita = New FSErIndicizzazione.comuneCDA() With
            {
                .cap = comNascita.Cap,
                .codice = comNascita.Codice,
                .codiceIstat = comNascita.Istat,
                .descrizione = comNascita.Descrizione,
                .provincia = comNascita.Provincia,
                .codiceStato = pazVac.Cittadinanza_Codice
            }

        End If

        If String.IsNullOrWhiteSpace(pazVac.ComuneResidenza_Codice) Then

            pazFSE.comuneResidenza = New FSErIndicizzazione.comuneCDA()

        Else

            Dim comRes As Comune = GenericProvider.Comuni.GetComuneByCodice(pazVac.ComuneResidenza_Codice)

            pazFSE.comuneResidenza = New FSErIndicizzazione.comuneCDA() With
            {
                .cap = comRes.Cap,
                .codice = comRes.Codice,
                .codiceIstat = comRes.Istat,
                .descrizione = comRes.Descrizione,
                .provincia = comRes.Provincia,
                .indirizzo = pazVac.IndirizzoResidenza
            }

        End If

        pazFSE.dataNascita = pazVac.Data_Nascita.ToString("yyyy-MM-dd")
        pazFSE.sesso = pazVac.Sesso
        pazFSE.telefono1 = pazVac.Telefono1
        pazFSE.telefono2 = pazVac.Telefono2
        pazFSE.telefono3 = pazVac.Telefono3
        pazFSE.tesseraSanitaria = pazVac.Tessera
        pazFSE.tipoPaziente = pazVac.Tipo

        pazFSE.uslCorrente = New FSErIndicizzazione.struttura()

        Return pazFSE

    End Function

    Public Function InsertDocumentUniqueIdFSE(documentUniqueId As String, codicePaziente As Long) As Integer

        Return GenericProvider.Paziente.InsertDocumentUniqueId(documentUniqueId, codicePaziente, Constants.TipoDocumentoFSE.CertificatoVaccinale, Date.Now)

    End Function

#End Region

End Class
