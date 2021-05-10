Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities

Public Class BizRilevazioniCovid19
    Inherits BizClass

#Region " Constructors "
    Public Sub New(genericProvider As DbGenericProvider, settings As Settings.Settings, contextInfo As BizContextInfos)

        MyBase.New(genericProvider, settings, contextInfo, Nothing)

    End Sub
#End Region

#Region " Public "

    Public Function GetDatiPaziente(codicePaziente As Long) As DatiPaziente
        Return GenericProvider.RilevazioniCovid19.GetDatiPaziente(codicePaziente)
    End Function
    Public Function GetDatiPazienti(codicePaziente As IEnumerable(Of Long)) As List(Of DatiPaziente)
        If codicePaziente Is Nothing Then
            Return New List(Of DatiPaziente)
        End If
        If codicePaziente.Any() Then
            Return GenericProvider.RilevazioniCovid19.GetDatiPazienti(codicePaziente.Distinct().ToList())
        Else
            Return New List(Of DatiPaziente)
        End If
    End Function

    Public Function SalvaElaborazioniContatti(dati As IEnumerable(Of ImportazioneContatti)) As String
        If IsNothing(dati) Then
            Return Nothing
        Else
            Return GenericProvider.RilevazioniCovid19.SalvaElaborazioniContatti(dati.ToList())
        End If
    End Function

    Public Function GetPossibiliContatti(codiciFiscali As IEnumerable(Of String)) As IEnumerable(Of InfoPossibileContatto)
        Dim codici As IEnumerable(Of String)
        If codiciFiscali Is Nothing Then
            codici = New List(Of String)
        Else
            codici = codiciFiscali.Where(Function(x)
                                             Return Not String.IsNullOrWhiteSpace(x)
                                         End Function).Select(Function(x) x.ToUpper()).Distinct().ToList()
        End If
        Return GenericProvider.RilevazioniCovid19.GetPossibiliContatti(codici)
    End Function

    Public Function GetUltimaDiaria(codicePaziente As Long) As Diaria
        Return GenericProvider.RilevazioniCovid19.GetUltimaDiaria(codicePaziente)
    End Function

    Public Function GetSintomi() As Dictionary(Of Integer, String)
        Return GenericProvider.Anagrafiche.GetListSintomi("").OrderBy(Function(x) x.Ordine).ToDictionary(Function(x)
                                                                                                             Return x.Codice
                                                                                                         End Function, Function(x)
                                                                                                                           Return x.Descrizione
                                                                                                                       End Function)
    End Function

    Public Function GetEpisodiByPaziente(codicePaziente As Long) As List(Of EpisodioPaziente)
        Return GenericProvider.RilevazioniCovid19.GetEpisodiByPaziente(codicePaziente)
    End Function
    Public Function GetIdEpisodiByPaziente(codicePaziente As Long) As List(Of Long)
        Return GenericProvider.RilevazioniCovid19.GetIdEpisodiByPaziente(codicePaziente)
    End Function
    Public Function GetEpisodio(idEpisodio As Long) As EpisodioPaziente
        Return GenericProvider.RilevazioniCovid19.GetEpisodio(idEpisodio)
    End Function

    Public Function CercaEpisodi_V2(ricerca As RicercaEpisodi) As List(Of InfoEpisodioCovid)
        Return GenericProvider.RilevazioniCovid19.RicercaEpisodiWorkList(ricerca)
    End Function

    Public Function CercaEpisodi(ricerca As RicercaEpisodi) As List(Of EpisodioPaziente)
        Return GenericProvider.RilevazioniCovid19.RicercaEpisodi(ricerca)
    End Function

    Public Function CercaCasiIndice(idEpisodi As List(Of Long)) As List(Of CasoIndice)
        Return GenericProvider.RilevazioniCovid19.CercaCasiIndice(idEpisodi)
    End Function
    'esegue il salvataggio dell'episodio ne restituisce l'id
    Public Function SalvaEpisodio(episodio As EpisodioPaziente, ricoveriDelete As List(Of Long), tamponiDelete As List(Of Long), diariaDelete As List(Of Long), contattiDelete As List(Of Long)) As Long
        If episodio.Dettaglio.GuaritoClinicamente AndAlso episodio.Dettaglio.DataGuaritoClinicamente.HasValue Then
            Dim codice As Integer? = GenericProvider.RilevazioniCovid19.GetCodiceGuaritoClinicamente()
            If codice.HasValue Then
                episodio.Testata.CodiceStato = codice
            End If
        ElseIf episodio.Dettaglio.Guarito Then
            Dim codice As Integer? = GenericProvider.RilevazioniCovid19.GetCodiceGuarito()
            If (codice.HasValue) Then
                episodio.Testata.CodiceStato = codice
            End If
        End If

        If Not episodio.Dettaglio.GuaritoClinicamente Then
            episodio.Dettaglio.DataGuaritoClinicamente = Nothing
        End If

        If Not episodio.Dettaglio.Guarito Then
            episodio.Dettaglio.DataGuarigioneVirol = Nothing
        End If

        If (episodio.Testata.IdEpisodio = 0) Then
            Return GenericProvider.RilevazioniCovid19.InsertEpisodio(episodio)
        Else
            If (ricoveriDelete IsNot Nothing AndAlso ricoveriDelete.Count > 0) Then
                GenericProvider.RilevazioniCovid19.DeleteRicoveri(ricoveriDelete)
            End If

            If (tamponiDelete IsNot Nothing AndAlso tamponiDelete.Count > 0) Then
                GenericProvider.RilevazioniCovid19.DeleteTamponi(tamponiDelete)
            End If

            If (diariaDelete IsNot Nothing AndAlso diariaDelete.Count > 0) Then
                GenericProvider.RilevazioniCovid19.DeleteDiaria(diariaDelete)
            End If

            If (contattiDelete IsNot Nothing AndAlso contattiDelete.Count > 0) Then
                GenericProvider.RilevazioniCovid19.DeleteContatti(contattiDelete)
            End If

            'prima di ogni salvataggio, i tag dell'episodio vengono "resettati"
            GenericProvider.RilevazioniCovid19.DeleteTags(episodio.Testata.IdEpisodio)
            GenericProvider.RilevazioniCovid19.UpdateEpisodio(episodio)
            Return episodio.Testata.IdEpisodio
        End If
    End Function

    'Restituisce l'elenco dei pazienti che hanno evidenziato il paziente (passato nel parametro) come contatto
    Public Function RecuperaPazientiCreatoriDelContatto(codicePazienteEpisodio As Long) As List(Of DatiPazienteCreatoreContatto)
        Return GenericProvider.RilevazioniCovid19.RecuperaPazientiCreatoriDelContatto(codicePazienteEpisodio)
    End Function

    Public Function RicercaTags(ricerca As RicercaTags) As Dictionary(Of String, String)
        Return GenericProvider.RilevazioniCovid19.RicercaTags(ricerca)
    End Function

    Public Sub DeleteEpisodio(idEpisodio As Long, utenteEliminazione As Long, uslInserimentoEliminazione As String)
        Dim episodioToDelete As EpisodioPaziente = GenericProvider.RilevazioniCovid19.GetEpisodio(idEpisodio)
        If (Not episodioToDelete Is Nothing) Then
            'prima di eliminare l'episodio, elimino tutti i contatti che hanno come riferimento questo episodio. 
            'Ex: se l'episodio A indica come contatto l'episodio B, quando viene cancellato B allora verrà cancellato anche il contatto di A verso B.

            episodioToDelete.Dettaglio.UtenteEliminazione = utenteEliminazione
            episodioToDelete.Dettaglio.UslEliminazione = uslInserimentoEliminazione
            episodioToDelete.Dettaglio.DataEliminazione = DateTime.Now

            GenericProvider.RilevazioniCovid19.DeleteContattiRiferimento(idEpisodio)
            GenericProvider.RilevazioniCovid19.DeleteEpisodio(episodioToDelete)
        End If
    End Sub

    Public Sub AggiornaDiaria(codiceDiaria As Long, sintomi As List(Of Integer), note As String)
        Dim situazioneAttuale As SituazioneDiaria = GenericProvider.RilevazioniCovid19.GetSituazioneDiaria(codiceDiaria)
        Dim sintomiDaAggiungere As IEnumerable(Of Integer) = sintomi.Where(Function(x)
                                                                               Return Not situazioneAttuale.SintomiEsistenti.Contains(x)
                                                                           End Function).ToArray()
        Dim sintomiDaEliminare As IEnumerable(Of Integer) = situazioneAttuale.SintomiEsistenti.Where(Function(x)
                                                                                                         Return Not sintomi.Contains(x)
                                                                                                     End Function)


        Dim codiciLinkDaEliminare As IEnumerable(Of Long) = sintomiDaEliminare.SelectMany(Function(x)
                                                                                              Return situazioneAttuale.sintomi.Where(Function(s)
                                                                                                                                         Return s.codiceSintomo = x
                                                                                                                                     End Function).Select(Function(l) l.codiceLink)
                                                                                          End Function).ToList()

        GenericProvider.RilevazioniCovid19.AggiornaDiaria(codiceDiaria, note, sintomiDaAggiungere, codiciLinkDaEliminare, Settings.CODICE_UTENTE_PRENOTAZIONE_WEB)

    End Sub

    Public Sub AggiungiDiaria(codiceEpisodio As Long, codicePaziente As Long, sintomi As List(Of Integer), note As String)
        GenericProvider.RilevazioniCovid19.InsertDiariaApp(codiceEpisodio, codicePaziente, sintomi, note, Settings.CODICE_UTENTE_PRENOTAZIONE_WEB)
    End Sub

    Public Sub MarcaPazienteInvioCredenzialiApp(pazCodice As Long)
        GenericProvider.RilevazioniCovid19.MarcaPazienteInvioCredenzialiApp(pazCodice)
    End Sub

    ''' <summary>
    ''' Imposta il flag di lettura dell'informativa ad "S"
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    Public Function MarcaPazienteInformativaCovid(codicePaziente As Long) As Integer
        Return GenericProvider.RilevazioniCovid19.UpdateFlagLetturaInformativaCovid(codicePaziente, True)
    End Function

    ''' <summary>
    ''' Restituisce true se il flag di lettura dell'informativa vale "S"
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    Public Function GetFlagLetturaInformativaCovid(codicePaziente As Long) As Boolean
        Return GenericProvider.RilevazioniCovid19.GetFlagLetturaInformativaCovid(codicePaziente) = "S"
    End Function

    Public Function GetSettingScolastico() As List(Of SettingScolastico)
        Return GenericProvider.RilevazioniCovid19.GetSettingScolastisco()
    End Function
    Public Function GetTipoScolastico() As List(Of TipoScolastico)
        Return GenericProvider.RilevazioniCovid19.GetTipoScolastico()
    End Function

    Public Function GetTipiEventiRicovero() As List(Of TipoEventoRicovero)
        Return GenericProvider.RilevazioniCovid19.GetTipiEventiRicovero()
    End Function

#End Region

#Region " FunzionalitàDiFrontiera "

    Public Function GetResultEsiti() As List(Of ResultEsiti)
        Return GenericProvider.RilevazioniCovid19.GetResultEsiti()
    End Function
    Public Function GetStatiEpisodio() As List(Of StatiEpisodio)
        Return GenericProvider.RilevazioniCovid19.GetStatiEpisodio()
    End Function
    Public Function GetTamponiFrontiera() As List(Of TamponeDiFrontiera)
        Using BizCovid19Tamponi As New BizCovid19Tamponi(GenericProvider, Settings, ContextInfos)
            Return BizCovid19Tamponi.GetTamponiFrontiera()
        End Using
    End Function
    Public Function GetTamponiFrontieraPerPazientiNonIdentificati() As List(Of TamponeDiFrontiera)
        Using BizCovid19Tamponi As New BizCovid19Tamponi(GenericProvider, Settings, ContextInfos)
            Return BizCovid19Tamponi.GetTamponiFrontieraPerPazientiNonIdentificati()
        End Using
    End Function
    Public Function UpdateError(errore As String, IdTampone As Integer, pazCodice As Long, dataElaborazione As DateTime)
        Using BizCovid19Tamponi As New BizCovid19Tamponi(GenericProvider, Settings, ContextInfos)
            BizCovid19Tamponi.UpdateError(errore, IdTampone, pazCodice, dataElaborazione)
        End Using
    End Function
    Public Function UpdateErrorNoPaz(errore As String, IdTampone As Integer, dataElaborazione As DateTime)
        Using BizCovid19Tamponi As New BizCovid19Tamponi(GenericProvider, Settings, ContextInfos)
            BizCovid19Tamponi.UpdateErrorNoPaz(errore, IdTampone, dataElaborazione)
        End Using
    End Function
    Public Function EpisodiApertiPerPaziente(codicePaziente As Long) As List(Of Long)
        Return GenericProvider.RilevazioniCovid19.EpisodiApertiPerPaziente(codicePaziente)
    End Function
    Public Function UpdateStatoElab(idTampone As Integer, statoElab As Integer, pazCodice As Long, dataElaborazione As DateTime)
        Using BizCovid19Tamponi As New BizCovid19Tamponi(GenericProvider, Settings, ContextInfos)
            BizCovid19Tamponi.UpdateStatoElab(idTampone, statoElab, pazCodice, dataElaborazione)
        End Using
    End Function
    Public Function UpdateStatoElab(idTampone As Integer, statoElab As Integer, data As DateTime, messaggio As String, pazCodice As Long, dataElaborazione As DateTime)
        Using BizCovid19Tamponi As New BizCovid19Tamponi(GenericProvider, Settings, ContextInfos)
            BizCovid19Tamponi.UpdateStatoElab(idTampone, statoElab, data, messaggio, pazCodice, dataElaborazione)
        End Using
    End Function
    Public Function UpdateStatoElabSDG(idTampone As Integer, statoElab As Integer, messaggio As String, pazCodice As Long, dataElaborazione As DateTime)
        Using BizCovid19Tamponi As New BizCovid19Tamponi(GenericProvider, Settings, ContextInfos)
            BizCovid19Tamponi.UpdateStatoElabSDG(idTampone, statoElab, messaggio, pazCodice, dataElaborazione)
        End Using
    End Function
    Public Function UpdateStatoElab(idTampone As Integer, statoElab As Integer, messaggio As String, pazCodice As Long, dataElaborazione As DateTime)
        Using BizCovid19Tamponi As New BizCovid19Tamponi(GenericProvider, Settings, ContextInfos)
            BizCovid19Tamponi.UpdateStatoElab(idTampone, statoElab, messaggio, pazCodice, dataElaborazione)
        End Using
    End Function

    'Permette di modificare l'utente d'inserimento del tampone.
    Public Sub UpdateUtenteInserimento(idTampone As Long, utenteInserimento As Long)
        GenericProvider.RilevazioniCovid19.UpdateUtenteInserimento(idTampone, utenteInserimento)
    End Sub
#End Region

#Region " TestRapido "

    Public Function GetTestRapidobyId(idTest As String, ulssRichiedente As String) As TestRapido
        Return GenericProvider.RilevazioniCovid19.GetTestRapidoById(idTest, ulssRichiedente)
    End Function
    Public Function GetTestRapidoTarbyId(idTest As String, ulssRichiedente As String) As TestRapido
        Return GenericProvider.RilevazioniCovid19.GetTestRapidoTarById(idTest, ulssRichiedente)
    End Function
    Public Function GetTestRapidobyCodiceFiscale(codiceFiscale As String, UlssRichiedente As String) As List(Of TestRapido)
        Return GenericProvider.RilevazioniCovid19.GetTestRapidoByCodiceFiscale(codiceFiscale, UlssRichiedente)
    End Function

    Public Function GetTestRapidoTarbyCodiceFiscale(codiceFiscale As String, UlssRichiedente As String) As List(Of TestRapido)
        Return GenericProvider.RilevazioniCovid19.GetTestRapidoTarByCodiceFiscale(codiceFiscale, UlssRichiedente)
    End Function
    Public Function GetTestRapidobyCFeId(idTest As String, codiceFiscale As String, ulssRichiedente As String) As TestRapido
        Return GenericProvider.RilevazioniCovid19.GetTestRapidoByCFeId(idTest, codiceFiscale, ulssRichiedente)
    End Function
    Public Function GetTestRapidoTarbyCFeId(idTest As String, codiceFiscale As String, ulssRichiedente As String) As TestRapido
        Return GenericProvider.RilevazioniCovid19.GetTestRapidoTarByCFeId(idTest, codiceFiscale, ulssRichiedente)
    End Function
    Public Function GetCentriByGruppo(gruppo As String) As List(Of String)
        Return GenericProvider.RilevazioniCovid19.GetCentriByGruppo(gruppo)
    End Function

    Public Function GetTestRapidoByCFeIdeGr(idTest As String, codiceFiscale As String, Gruppo As String) As TestRapido
        Return GenericProvider.RilevazioniCovid19.GetTestRapidoByCFeIdeGr(idTest, codiceFiscale, Gruppo)
    End Function
    Public Function GetTestRapidoTarByCFeIdeGr(idTest As String, codiceFiscale As String, Gruppo As String) As TestRapido
        Return GenericProvider.RilevazioniCovid19.GetTestRapidoTarByCFeIdeGr(idTest, codiceFiscale, Gruppo)
    End Function
    Public Function GetTestRapidobyIdGr(idTest As String, Gruppo As String) As TestRapido
        Return GenericProvider.RilevazioniCovid19.GetTestRapidoByIdGr(idTest, Gruppo)
    End Function
    Public Function GetTestRapidoTarByIdGr(idTest As String, Gruppo As String) As TestRapido
        Return GenericProvider.RilevazioniCovid19.GetTestRapidoTarByIdGr(idTest, Gruppo)
    End Function
    Public Function GetTestRapidobyCodiceFiscaleGr(codiceFiscale As String, Gruppo As String) As List(Of TestRapido)
        Return GenericProvider.RilevazioniCovid19.GetTestRapidoByCodiceFiscaleGr(codiceFiscale, Gruppo)
    End Function
    Public Function GetTestRapidoTarbyCodiceFiscaleGr(codiceFiscale As String, Gruppo As String) As List(Of TestRapido)
        Return GenericProvider.RilevazioniCovid19.GetTestRapidoTarByCodiceFiscaleGr(codiceFiscale, Gruppo)
    End Function
    Public Function SalvaDettaglioTestRapido(dettaglio As DettaglioTestRapido) As ResultSetPost
        Return GenericProvider.RilevazioniCovid19.SalvaDettaglio(dettaglio)
    End Function
    Public Function SalvaDettaglioTestRapidoTar(dettaglio As DettaglioAntigeneTestRapido) As ResultSetPost
        Return GenericProvider.RilevazioniCovid19.SalvaDettaglioTar(dettaglio)
    End Function
#End Region

#Region " Comunicazione OTP a servizio SAR - NON PIU' UTILIZZATO "

    ' N.B.
    ' Questa versione della chiamata al servizio del SAR è stata sostituita utilizzando HttpClient, 
    ' perchè con i metodi della classe autogenerata dal LORO wsdl non si riusciva a deserializzare la risposta

    Public Class ComunicaOTPCommand
        Public Property IdUtente As Long
        Public Property OTP As String
        Public Property DataInizioSintomi As Date
    End Class

    Public Function ComunicaOTP(command As ComunicaOTPCommand) As BizGenericResult

        Dim codiceFiscaleUtente As String = GenericProvider.Utenti.GetCodiceFiscaleUtente(command.IdUtente)

        If String.IsNullOrWhiteSpace(codiceFiscaleUtente) Then
            Return New BizGenericResult() With {.Success = False, .Message = "Codice fiscale dell'utente non presente"}
        End If

        If String.IsNullOrWhiteSpace(command.OTP) Then
            Return New BizGenericResult() With {.Success = False, .Message = "OTP non presente"}
        End If

        If command.DataInizioSintomi = Date.MinValue Then
            Return New BizGenericResult() With {.Success = False, .Message = "Data inizio sintomi non presente"}
        End If

        Dim success As Boolean = False
        Dim message As String = String.Empty
        Dim logMessage As String = String.Empty
        Dim serviceResponse As SistemaTsImmuni.SistemaTsImmuniRicevuta = Nothing

        Dim dataInvio As Date?
        Dim dataRicezione As Date?

        Dim serviceRequest As New SistemaTsImmuni.SistemaTsImmuniRichiesta() With
        {
            .idUtenteRichiedente = codiceFiscaleUtente,
            .codiceAnonimoTemporaneo = command.OTP,
            .dataInizioSintomi = command.DataInizioSintomi.ToString("yyyy/MM/dd"),
            .OpzioniRequest = New List(Of SistemaTsImmuni.opzioniType) From {
                New SistemaTsImmuni.opzioniType() With {.chiave = "TIPO_AUTH_UTENTE_REGIONALE", .valore = "FA2"}
            }.ToArray()
        }

        Try
            Using client As New SistemaTsImmuni.sistemaTsImmuniPTClient()

                dataInvio = Date.Now

                ' TODO [COV - OTP]: serve la open?
                'client.Open()

                serviceResponse = client.sistemaTsImmuni(serviceRequest)

            End Using

            If serviceResponse Is Nothing Then
                success = False
                message = "Nessuna risposta ricevuta dal servizio SAR"
            Else
                dataRicezione = Date.Now
                success = serviceResponse.esito = "0" OrElse serviceResponse.esito = "00"
                message = String.Format("{0} [{1}]", serviceResponse.descrizioneEsito, serviceResponse.esito)
            End If

            logMessage = message

        Catch ex As Exception
            success = False
            message = "Errore durante la comunicazione con il servizio SAR"
            logMessage = ex.ToString()
        End Try

        If Settings.COV_LOG_INVIO_COMUNICAZIONE_OTP Then

            ' N.B. : nel log della comunicazione OTP a SAR, l'id messaggio non c'è. Viene valorizzato con il loro id transazione che restituiscono se va a buon fine l'invio, altrimenti resta vuoto
            Try
                Using bizLogNotifiche As New BizLogNotifiche(GenericProvider, Settings, ContextInfos)

                    Dim logNotifica As LogNotificaInviata = bizLogNotifiche.InsertLogNotificaInviataComunicazioneOTP(
                    New BizLogNotifiche.InsertLogNotificaInviataComunicazioneOTPCommand() With {
                        .DataInvio = dataInvio,
                        .DataRicezione = dataRicezione,
                        .IdMessaggio = serviceResponse?.idTransazione,
                        .InvioOk = success,
                        .MessaggioRisposta = logMessage,
                        .ServiceRequest = SerializeEntity(serviceRequest),
                        .ServiceResponse = If(serviceResponse Is Nothing, String.Empty, SerializeEntity(serviceResponse))
                    }
                )

                End Using

            Catch ex As Exception
                Common.Utility.EventLogHelper.EventLogWrite(ex, String.Format("Errore scrittura log notifica OTP per l'utente {0} [{1}]", command.IdUtente.ToString(), codiceFiscaleUtente), EventLogEntryType.Information, ContextInfos.IDApplicazione)
            End Try

        End If

        Return New BizGenericResult() With {.Success = success, .Message = message}

    End Function

    Public Function GetTestataEpisodio(codiceEpisodio As Long) As TestataEpisodioCovid
        Dim elenco As New List(Of Long)
        elenco.Add(codiceEpisodio)
        Return GetTestataEpisodi(elenco).FirstOrDefault()
    End Function

    Public Function GetTestataEpisodi(codiceEpisodio As IEnumerable(Of Long)) As IEnumerable(Of TestataEpisodioCovid)
        Return GenericProvider.RilevazioniCovid19.GetTestataEpisodi(codiceEpisodio)
    End Function

    Public Sub SalvaTestataEpisodio(dati As SalvaTestataEpisodioCovid)
        Dim elenco As New List(Of SalvaTestataEpisodioCovid)
        elenco.Add(dati)
        SalvaTestateEpisodi(elenco)
    End Sub

    Public Sub SalvaTestateEpisodi(dati As IEnumerable(Of SalvaTestataEpisodioCovid))
        Dim tmp As List(Of SalvaTestataEpisodioCovid)
        If Not IsNothing(dati) Then
            tmp = dati.ToList()
        Else
            tmp = New List(Of SalvaTestataEpisodioCovid)()
        End If
        GenericProvider.RilevazioniCovid19.SalvaTestateEpisodi(tmp)
    End Sub


#End Region
    Public Function GetCodiciLaboratorio(q As String) As IEnumerable(Of Entities.CodiceLaboratorioTampone)
        Return GenericProvider.RilevazioniCovid19.GetCodiciLaboratorio(q)
    End Function

    Public Sub AbbinaFocolaioEpisodio(codiceEpisodio As Long, codiceFocolaio As Integer)
        GenericProvider.RilevazioniCovid19.AbbinaFocolaioEpisodio(codiceEpisodio, codiceFocolaio)
    End Sub

    Public Function GetContattiPositiviLiberi(codiceEpisodio As Long) As IEnumerable(Of ContattoLiberoFocolaio)
        Return GenericProvider.RilevazioniCovid19.GetContattiPositiviLiberi(codiceEpisodio)
    End Function
    Public Function GetInformazioniFocolaio(codiceFocolaio As Integer) As DettagliFocolaio
        Return GenericProvider.RilevazioniCovid19.GetInformazioniFocolaio(codiceFocolaio)
    End Function

    Public Function PossibiliFocolaiEpisodio(codiceEpisodio As Long) As IEnumerable(Of Entities.InfoFocolaio)
        Return GenericProvider.RilevazioniCovid19.PossibiliFocolaiEpisodio(codiceEpisodio).ToArray()
    End Function

    Public Function FocolaiEpisodio(codiceEpisodio As Long) As IEnumerable(Of InfoFocolaio)
        Return GenericProvider.RilevazioniCovid19.FocolaiEpisodio(codiceEpisodio).ToArray()
    End Function

    Public Function CreaFocolaio(model As CreaFocolaio) As Long
        Return GenericProvider.RilevazioniCovid19.CreaFocolaio(model)
    End Function

End Class
