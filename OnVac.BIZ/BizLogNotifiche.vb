Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Enumerators
Imports System.Collections.Generic

Public Class BizLogNotifiche
    Inherits Biz.BizClass

#Region " Constructors "

    'TODO [BizLogNotifiche]: accorpare i due costruttori
    Public Sub New(genericProvider As DbGenericProvider, settings As Settings.Settings, contextInfo As BizContextInfos)

        MyBase.New(genericProvider, settings, contextInfo, Nothing)

    End Sub

    Public Sub New(genericProvider As DbGenericProvider, contextInfo As BizContextInfos)

        MyBase.New(genericProvider, Nothing, contextInfo, Nothing)

    End Sub

#End Region

#Region " Types "

    <Serializable>
    Public Class ManageLogNotificaRicevutaResult
        Public Property ExistsLogNotificaRicevuta As Boolean
        Public Property RisultatoElaborazione As String
        Public Property MessaggioElaborazione As String
    End Class

	Public Class RisultatoElaborazioneNotifica

		''' <summary>
		''' = "A"
		''' </summary>
		''' <remarks></remarks>
		Public Const ACK_POSITIVO As String = "A"

		''' <summary>
		''' = "E"
		''' </summary>
		''' <remarks></remarks>
		Public Const [ERROR] As String = "E"

		''' <summary>
		''' = "W"
		''' </summary>
		''' <remarks></remarks>
		Public Const WARNING As String = "W"

	End Class
	<Serializable>
	Public Class GetLogNotificheRicevuteResult

		Public ListLogNotificheRicevuteMonitor As List(Of Entities.LogNotificaRicevutaACN)
		Public CountNotificheRicevute As Integer

	End Class
	Public Class GetLogNotificheRicevuteCommand

		Public Filtri As Entities.FiltriLogNotifiche
		Public CampoOrdinamento As String
		Public VersoOrdinamento As String
		Public PageIndex As Integer
		Public PageSize As Integer

	End Class

#End Region

#Region " Public "

#Region " Notifiche ricevute "

	Public Class ManageLogNotificaRicevutaCommand
		Public Property IdMessaggio As String
		Public Property CodiceCentralePaziente As String
		Public Property CodiceCentralePazienteAlias As String
		Public Property Paziente As Entities.Paziente
		Public Property EsenzioniMalattia As IEnumerable(Of Onit.OnAssistnet.OnVac.Entities.EsenzioneMalattia)
		Public Property ServiceRequest As String
		Public Property ServiceResponse As String
		Public Property RepositoryUniqueId As String
		Public Property DocumentUniqueId As String
		Public Property EntitySerializzata As Entities.FlussoACN
	End Class

	''' <summary>
	''' Se non esiste un log per il messaggio di notifica specificato, lo inserisce.
	''' Se esiste già, incrementa il numero di ricezioni.
	''' </summary>
	''' <param name="command"></param>
	''' <remarks></remarks>
	Public Function ManageLogNotificaRicevuta(command As ManageLogNotificaRicevutaCommand) As ManageLogNotificaRicevutaResult

		Dim result As New ManageLogNotificaRicevutaResult()

		Dim logNotificaRicevuta As Entities.LogNotificaRicevuta = Me.GenericProvider.LogNotifiche.GetLogNotificaRicevuta(command.IdMessaggio)

		If logNotificaRicevuta Is Nothing Then
			'---
			' Messaggio di notifica ricevuto per la prima volta => inserimento di un log
			'---
			result.ExistsLogNotificaRicevuta = False

			logNotificaRicevuta = New Entities.LogNotificaRicevuta()

			' logNotifica.IdNotifica => automatico
			logNotificaRicevuta.IdMessaggio = command.IdMessaggio
			logNotificaRicevuta.CodiceCentralePaziente = command.CodiceCentralePaziente
			logNotificaRicevuta.CodiceCentralePazienteAlias = command.CodiceCentralePazienteAlias
			logNotificaRicevuta.NumeroRicezioni = 1
			logNotificaRicevuta.DataRicezione = DateTime.Now
			logNotificaRicevuta.EntityPazienteSerializzata = SerializeEntity(command.Paziente)
			If Not command.EsenzioniMalattia.IsNullOrEmpty() Then
				logNotificaRicevuta.Esenzioni = SerializeEntity(command.EsenzioniMalattia.ToList())
			End If
			logNotificaRicevuta.ServiceRequest = command.ServiceRequest
			logNotificaRicevuta.ServiceResponse = command.ServiceResponse
			logNotificaRicevuta.RepositoryUniqueId = command.RepositoryUniqueId
			logNotificaRicevuta.DocumentUniqueId = command.DocumentUniqueId
			logNotificaRicevuta.EntitySerializzata = SerializeEntity(command.EntitySerializzata)

			' Non valorizzati all'inserimento ma solo a fine elaborazione:
			'logNotifica.Operazione
			'logNotifica.RisultatoElaborazione
			'logNotifica.MessaggioElaborazione
			'logNotifica.DataInvioRisposta

			Me.GenericProvider.LogNotifiche.InsertLogNotificaRicevuta(logNotificaRicevuta)

		Else
			'---
			' Messaggio di notifica già ricevuto in precedenza => restituisco il risultato loggato
			'---
			result.ExistsLogNotificaRicevuta = True
			result.RisultatoElaborazione = logNotificaRicevuta.RisultatoElaborazione
			result.MessaggioElaborazione = logNotificaRicevuta.MessaggioElaborazione

			' update del log (numero ricezioni e data invio)
			logNotificaRicevuta.NumeroRicezioni += 1
			logNotificaRicevuta.DataInvioRisposta = DateTime.Now

			Me.GenericProvider.LogNotifiche.UpdateLogNotificaRicevuta(logNotificaRicevuta)

		End If

		Return result

	End Function
	Public Function GetLogNotificheRicevuteAcn(command As GetLogNotificheRicevuteCommand) As GetLogNotificheRicevuteResult

		If command Is Nothing Then Throw New ArgumentNullException()

		Dim list As List(Of Entities.LogNotificaRicevutaACN) = Nothing

		Dim count As Integer = GenericProvider.LogNotifiche.CountLogNotificheRicevuteAcn(command.Filtri)

		If count = 0 Then

			list = New List(Of Entities.LogNotificaRicevutaACN)()

		Else

			Dim pagingOptions As New Data.PagingOptions()
			pagingOptions.StartRecordIndex = command.PageIndex * command.PageSize
			pagingOptions.EndRecordIndex = pagingOptions.StartRecordIndex + command.PageSize

			list = GenericProvider.LogNotifiche.GetLogNotificheRicevuteAcn(command.Filtri, command.CampoOrdinamento, command.VersoOrdinamento, pagingOptions)

		End If

		Dim result As New GetLogNotificheRicevuteResult()
		result.CountNotificheRicevute = count
		result.ListLogNotificheRicevuteMonitor = list

		Return result

	End Function

	Public Function InsertLogNotificaRicevuta(logNotifica As ManageLogNotificaRicevutaCommand) As Integer
		Dim logNotificaRicevuta As Entities.LogNotificaRicevuta = New Entities.LogNotificaRicevuta()

		' logNotifica.IdNotifica => automatico
		logNotificaRicevuta.IdMessaggio = logNotifica.IdMessaggio
		logNotificaRicevuta.CodiceCentralePaziente = logNotifica.CodiceCentralePaziente
		logNotificaRicevuta.CodiceCentralePazienteAlias = logNotifica.CodiceCentralePazienteAlias
		logNotificaRicevuta.NumeroRicezioni = 1
		logNotificaRicevuta.DataRicezione = DateTime.Now
		logNotificaRicevuta.EntityPazienteSerializzata = SerializeEntity(logNotifica.Paziente)
		If Not logNotifica.EsenzioniMalattia.IsNullOrEmpty() Then
			logNotificaRicevuta.Esenzioni = SerializeEntity(logNotifica.EsenzioniMalattia.ToList())
		End If
		logNotificaRicevuta.ServiceRequest = logNotifica.ServiceRequest
		logNotificaRicevuta.ServiceResponse = logNotifica.ServiceResponse
		logNotificaRicevuta.RepositoryUniqueId = logNotifica.RepositoryUniqueId
		logNotificaRicevuta.DocumentUniqueId = logNotifica.DocumentUniqueId
		Return Me.GenericProvider.LogNotifiche.InsertLogNotificaRicevuta(logNotificaRicevuta)
	End Function

	''' <summary>
	''' Modifica i dati relativi all'elaborazione e all'invio della risposta, per la notifica specificata.
	''' </summary>
	''' <param name="idMessaggio"></param>
	''' <param name="operazione"></param>
	''' <param name="risultatoElaborazione"></param>
	''' <param name="messaggioElaborazione"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function UpdateDatiElaborazioneNotificaRicevuta(idMessaggio As String, operazione As Enumerators.OperazioneLogNotifica, risultatoElaborazione As String, messaggioElaborazione As String) As Integer

        Dim logNotificaRicevuta As Entities.LogNotificaRicevuta = Me.GenericProvider.LogNotifiche.GetLogNotificaRicevuta(idMessaggio)

        logNotificaRicevuta.Operazione = operazione
        logNotificaRicevuta.MessaggioElaborazione = messaggioElaborazione
        logNotificaRicevuta.RisultatoElaborazione = risultatoElaborazione
        logNotificaRicevuta.DataInvioRisposta = DateTime.Now

        Return Me.GenericProvider.LogNotifiche.UpdateLogNotificaRicevuta(logNotificaRicevuta)

    End Function

	Public Function UpdateDatiElaborazioneNotificaRicevuta(idMessaggio As String, operazione As Enumerators.OperazioneLogNotifica, successoElaborazione As Boolean, messaggioElaborazione As String, serviceResponce As String) As Integer

		Dim logNotificaRicevuta As Entities.LogNotificaRicevuta = Me.GenericProvider.LogNotifiche.GetLogNotificaRicevuta(idMessaggio)

		logNotificaRicevuta.Operazione = operazione
		logNotificaRicevuta.MessaggioElaborazione = messaggioElaborazione
		logNotificaRicevuta.DataInvioRisposta = DateTime.Now
		If Not String.IsNullOrWhiteSpace(serviceResponce) Then
			logNotificaRicevuta.ServiceResponse = serviceResponce
		End If

		If successoElaborazione Then
			logNotificaRicevuta.RisultatoElaborazione = RisultatoElaborazioneNotifica.ACK_POSITIVO
		Else
			logNotificaRicevuta.RisultatoElaborazione = RisultatoElaborazioneNotifica.ERROR
		End If

		Return Me.GenericProvider.LogNotifiche.UpdateLogNotificaRicevuta(logNotificaRicevuta)

	End Function

#End Region

#Region " Notifiche inviate "

#Region " Insert Notifica "

	Public Function InsertLogNotificaInviata(paziente As Entities.Paziente) As Entities.LogNotificaInviata

        Return InsertLogNotificaInviata(paziente, Nothing, Nothing, Nothing)

    End Function

    Public Function InsertLogNotificaInviata(paziente As Entities.Paziente, operazione As Enumerators.OperazioneLogNotifica) As Entities.LogNotificaInviata

        Return InsertLogNotificaInviata(paziente, Nothing, Nothing, operazione)

    End Function

    Public Function InsertLogNotificaInviata(paziente As Entities.Paziente, dataInvio As DateTime?) As Entities.LogNotificaInviata

        Return InsertLogNotificaInviata(paziente, Nothing, dataInvio, Nothing)

    End Function

    Public Function InsertLogNotificaInviata(paziente As Entities.Paziente, pazienteOriginale As Entities.Paziente, dataInvio As DateTime?) As Entities.LogNotificaInviata

        Return InsertLogNotificaInviata(paziente, pazienteOriginale, dataInvio, Nothing)

    End Function

    Private Function InsertLogNotificaInviata(paziente As Entities.Paziente, pazienteOriginale As Entities.Paziente, dataInvio As DateTime?, operazione? As Enumerators.OperazioneLogNotifica) As Entities.LogNotificaInviata

        Dim logNotificaInviata As New Entities.LogNotificaInviata()

        logNotificaInviata.IdNotifica = GenericProvider.LogNotifiche.GetNextIdNotificaInviata()

        logNotificaInviata.CodiceCentralePaziente = paziente.CodiceAusiliario
        logNotificaInviata.CodiceLocalePaziente = paziente.Paz_Codice

        logNotificaInviata.EntityPazienteSerializzata = SerializeEntity(paziente)

        If Not pazienteOriginale Is Nothing Then
            logNotificaInviata.EntityPazientePrecedenteSerializzata = SerializeEntity(pazienteOriginale)
        End If

        logNotificaInviata.DataInserimentoNotifica = DateTime.Now
        logNotificaInviata.IdUtenteInserimentoNotifica = ContextInfos.IDUtente

        If dataInvio.HasValue Then
            logNotificaInviata.DataInvio = dataInvio.Value
            logNotificaInviata.Stato = StatoLogNotificaInviata.Inviata
            logNotificaInviata.NumeroInvii = 1
        Else
            logNotificaInviata.Stato = StatoLogNotificaInviata.DaInviare
            logNotificaInviata.NumeroInvii = 0
        End If

        If operazione.HasValue Then
            ' Se è valorizzata, l'operazione è quella specificata
            logNotificaInviata.Operazione = operazione.Value
        Else
            ' Altrimenti, la valorizzo come inserimento o modifica in base all'assenza o alla presenza del codice ausiliario
            If String.IsNullOrWhiteSpace(paziente.CodiceAusiliario) Then
                ' Codice ausiliario non presente => notifica di inserimento 
                logNotificaInviata.Operazione = OperazioneLogNotifica.Inserimento
            Else
                ' Codice ausiliario presente => notifica di modifica
                logNotificaInviata.Operazione = OperazioneLogNotifica.Modifica
            End If
        End If

        GenericProvider.LogNotifiche.InsertLogNotificaInviata(logNotificaInviata, GetCodiceUslGestitaByIDApplicazione(ContextInfos.IDApplicazione))

        Return logNotificaInviata

    End Function

    Public Function InsertLogNotificaInviata(codiceLocalePaziente As Long, codiceCentralePaziente As String, request As String, dataInvio As DateTime?, idMessaggio As String, operazione As Enumerators.OperazioneLogNotifica, idPolo As Long?, operatore As String) As Entities.LogNotificaInviata

        Dim logNotificaInviata As New Entities.LogNotificaInviata()

		logNotificaInviata.IdNotifica = GenericProvider.LogNotifiche.GetNextIdNotificaInviata()

		logNotificaInviata.CodiceCentralePaziente = codiceCentralePaziente
        logNotificaInviata.CodiceLocalePaziente = codiceLocalePaziente

        logNotificaInviata.DataInserimentoNotifica = DateTime.Now
        logNotificaInviata.IdUtenteInserimentoNotifica = ContextInfos.IDUtente

        logNotificaInviata.Operazione = operazione
        logNotificaInviata.ServiceRequest = request

        If dataInvio.HasValue Then
            logNotificaInviata.DataInvio = dataInvio.Value
            logNotificaInviata.Stato = StatoLogNotificaInviata.Inviata
            logNotificaInviata.NumeroInvii = 1
        Else
            logNotificaInviata.Stato = StatoLogNotificaInviata.DaInviare
            logNotificaInviata.NumeroInvii = 0
        End If

        logNotificaInviata.IdMessaggio = idMessaggio

        logNotificaInviata.IdPolo = idPolo

        logNotificaInviata.Operatore = operatore

        GenericProvider.LogNotifiche.InsertLogNotificaInviata(logNotificaInviata, GetCodiceUslByIdApplicazione(ContextInfos.IDApplicazione))

        Return logNotificaInviata

    End Function


	Public Function InsertLogNotificaInviata(codiceLocalePaziente As Long, codiceCentralePaziente As String, dataInvio As DateTime?, idPolo As Long?, documentUniqueId As String, cda As String, newDocUniqId As String, reposUniqId As String, sourceIp As String, destUserId As String, operazione As OperazioneLogNotifica, stato As StatoLogNotificaInviata?) As Entities.LogNotificaInviata

		Dim logNotificaInviata As New Entities.LogNotificaInviata()
		Dim ii As Long = GenericProvider.LogNotifiche.GetNextIdNotificaInviata()

		logNotificaInviata.IdNotifica = ii

		logNotificaInviata.CodiceCentralePaziente = codiceCentralePaziente
		logNotificaInviata.CodiceLocalePaziente = codiceLocalePaziente

		logNotificaInviata.DataInserimentoNotifica = DateTime.Now
		logNotificaInviata.IdUtenteInserimentoNotifica = ContextInfos.IDUtente

		logNotificaInviata.Operazione = operazione
		logNotificaInviata.CdaFse = cda
		logNotificaInviata.NewDocUniqId = newDocUniqId
		logNotificaInviata.ReposUniqIdFse = reposUniqId
		logNotificaInviata.SourceIpFse = sourceIp
		logNotificaInviata.DestiUserIdFse = destUserId

		If dataInvio.HasValue Then
			logNotificaInviata.DataInvio = dataInvio.Value
			logNotificaInviata.Stato = StatoLogNotificaInviata.Inviata
			logNotificaInviata.NumeroInvii = 1
		Else
			logNotificaInviata.Stato = StatoLogNotificaInviata.DaInviare
			logNotificaInviata.NumeroInvii = 0
		End If
		If stato.HasValue Then
			logNotificaInviata.Stato = stato.Value
		End If

		logNotificaInviata.IdPolo = idPolo

		GenericProvider.LogNotifiche.InsertLogNotificaInviata(logNotificaInviata, GetCodiceUslByIdApplicazione(ContextInfos.IDApplicazione))

		Return logNotificaInviata

	End Function

	Public Class InsertLogNotificaInviataComunicazioneOTPCommand
        Public Property DataInvio As Date?
        Public Property DataRicezione As Date?
        Public Property IdMessaggio As String
        Public Property InvioOk As Boolean
        Public Property MessaggioRisposta As String
        Public Property ServiceRequest As String
        Public Property ServiceResponse As String
    End Class

    Public Function InsertLogNotificaInviataComunicazioneOTP(command As InsertLogNotificaInviataComunicazioneOTPCommand) As Entities.LogNotificaInviata

        If Not Settings.COV_LOG_INVIO_COMUNICAZIONE_OTP Then
            Return Nothing
        End If

        Dim logNotificaInviata As New Entities.LogNotificaInviata()

        logNotificaInviata.IdNotifica = GenericProvider.LogNotifiche.GetNextIdNotificaInviata()

        'logNotificaInviata.CodiceCentralePaziente = String.Empty
        'logNotificaInviata.CodiceLocalePaziente = Nothing

        If command.DataInvio.HasValue Then
            logNotificaInviata.DataInvio = command.DataInvio.Value
            logNotificaInviata.Stato = StatoLogNotificaInviata.Inviata
            logNotificaInviata.NumeroInvii = 1
        Else
            logNotificaInviata.Stato = StatoLogNotificaInviata.DaInviare
            logNotificaInviata.NumeroInvii = 0
        End If

        'logNotificaInviata.EntityPazienteSerializzata = String.Empty
        'logNotificaInviata.EntityPazientePrecedenteSerializzata = String.Empty

        logNotificaInviata.Operazione = OperazioneLogNotifica.ComunicazioneOTPCovid

        logNotificaInviata.DataRicezioneRisposta = command.DataRicezione
        logNotificaInviata.RisultatoRisposta = If(command.InvioOk, "S", "N")
        logNotificaInviata.MessaggioRisposta = command.MessaggioRisposta

        logNotificaInviata.ServiceRequest = command.ServiceRequest
        logNotificaInviata.ServiceResponse = command.ServiceResponse

        logNotificaInviata.DataInserimentoNotifica = Date.Now
        logNotificaInviata.IdUtenteInserimentoNotifica = ContextInfos.IDUtente
        logNotificaInviata.IdMessaggio = command.IdMessaggio

        'logNotificaInviata.IdPolo = String.Empty
        'logNotificaInviata.CdaFse = cda
        'logNotificaInviata.NewDocUniqId = newDocUniqId
        'logNotificaInviata.ReposUniqIdFse = reposUniqId
        'logNotificaInviata.SourceIpFse = sourceIp
        'logNotificaInviata.DestiUserIdFse = destUserId

        GenericProvider.LogNotifiche.InsertLogNotificaInviata(logNotificaInviata, GetCodiceUslByIdApplicazione(ContextInfos.IDApplicazione))

        Return logNotificaInviata

    End Function

    Public Class InsertLogNotificaInviataRicercaQPv2Command
        Public Property DataInvio As Date?
        Public Property DataRicezione As Date?
        Public Property RisultatoRisposta As Boolean
        Public Property MessaggioRisposta As String
        Public Property ServiceRequest As String
        Public Property ServiceResponse As String
        Public Property IdUtenteInserimento As Long?
        Public Property CodiceUslInserimento As String
    End Class

    Public Function InsertLogNotificaInviataRicercaQPv2(command As InsertLogNotificaInviataRicercaQPv2Command) As Entities.LogNotificaInviata

        Dim logNotificaInviata As New Entities.LogNotificaInviata()

        logNotificaInviata.IdNotifica = GenericProvider.LogNotifiche.GetNextIdNotificaInviata()

        'logNotificaInviata.CodiceCentralePaziente = String.Empty
        'logNotificaInviata.CodiceLocalePaziente = Nothing

        If command.DataInvio.HasValue Then
            logNotificaInviata.DataInvio = command.DataInvio.Value
            logNotificaInviata.Stato = StatoLogNotificaInviata.Inviata
            logNotificaInviata.NumeroInvii = 1
        Else
            logNotificaInviata.Stato = StatoLogNotificaInviata.DaInviare
            logNotificaInviata.NumeroInvii = 0
        End If

        'logNotificaInviata.EntityPazienteSerializzata = String.Empty
        'logNotificaInviata.EntityPazientePrecedenteSerializzata = String.Empty

        logNotificaInviata.Operazione = OperazioneLogNotifica.RicercaQPv2

        logNotificaInviata.DataRicezioneRisposta = command.DataRicezione
        logNotificaInviata.RisultatoRisposta = If(command.RisultatoRisposta, "S", "N")
        logNotificaInviata.MessaggioRisposta = command.MessaggioRisposta

        logNotificaInviata.ServiceRequest = command.ServiceRequest
        logNotificaInviata.ServiceResponse = command.ServiceResponse

        logNotificaInviata.DataInserimentoNotifica = Date.Now
        logNotificaInviata.IdUtenteInserimentoNotifica = If(command.IdUtenteInserimento.HasValue, command.IdUtenteInserimento.Value, ContextInfos.IDUtente)
        'logNotificaInviata.IdMessaggio = String.Empty

        'logNotificaInviata.IdPolo = String.Empty
        'logNotificaInviata.CdaFse = cda
        'logNotificaInviata.NewDocUniqId = newDocUniqId
        'logNotificaInviata.ReposUniqIdFse = reposUniqId
        'logNotificaInviata.SourceIpFse = sourceIp
        'logNotificaInviata.DestiUserIdFse = destUserId

        Dim codiceUsl As String = command.CodiceUslInserimento

        If String.IsNullOrWhiteSpace(codiceUsl) Then
            codiceUsl = GetCodiceUslByIdApplicazione(ContextInfos.IDApplicazione)
        End If

        GenericProvider.LogNotifiche.InsertLogNotificaInviata(logNotificaInviata, codiceUsl)

        Return logNotificaInviata

    End Function

#End Region

#Region " Update Notifica "

    Public Class UpdateLogNotificaInviataCommand
        Public LogNotificaDaModificare As Entities.LogNotificaInviata
        Public CodiceCentralePaziente As String
        Public MessaggioRisposta As String
        Public SuccessoRisposta As Boolean?
        Public RichiestaServizio As String
        Public RispostaServizio As String
        Public DataInvio As DateTime?
        Public DataRicezioneRisposta As DateTime?
        Public StatoInvio As StatoLogNotificaInviata?
        Public IncrementaInvii As Boolean
    End Class

    Public Function UpdateLogNotificaInviata(command As UpdateLogNotificaInviataCommand) As Integer

        If command.LogNotificaDaModificare Is Nothing Then Return 0

        command.LogNotificaDaModificare.CodiceCentralePaziente = command.CodiceCentralePaziente

        command.LogNotificaDaModificare.DataRicezioneRisposta = command.DataRicezioneRisposta
        command.LogNotificaDaModificare.MessaggioRisposta = command.MessaggioRisposta

        If Not command.SuccessoRisposta.HasValue Then
            command.LogNotificaDaModificare.RisultatoRisposta = String.Empty
        ElseIf command.SuccessoRisposta.Value Then
            command.LogNotificaDaModificare.RisultatoRisposta = "S"
        Else
            command.LogNotificaDaModificare.RisultatoRisposta = "N"
        End If

        command.LogNotificaDaModificare.ServiceRequest = command.RichiestaServizio
        command.LogNotificaDaModificare.ServiceResponse = command.RispostaServizio

        If command.DataInvio.HasValue AndAlso Not command.LogNotificaDaModificare.DataInvio.HasValue Then
            command.LogNotificaDaModificare.DataInvio = command.DataInvio
        End If

        If command.StatoInvio.HasValue Then
            command.LogNotificaDaModificare.Stato = command.StatoInvio
        End If

        If command.IncrementaInvii Then
            If command.LogNotificaDaModificare.NumeroInvii.HasValue Then
                command.LogNotificaDaModificare.NumeroInvii += 1
            Else
                command.LogNotificaDaModificare.NumeroInvii = 1
            End If

        End If

        ' Se la notifica rimane in stato "Da inviare", gestisco la logica dei tentativi di invio
        If command.LogNotificaDaModificare.Stato = Enumerators.StatoLogNotificaInviata.DaInviare Then

            If Settings.FSE_TENTATIVI_STOP > 0 AndAlso command.LogNotificaDaModificare.NumeroInvii >= Settings.FSE_TENTATIVI_STOP Then

                ' Se il parametro vale 0, oppure il numero di tentativi ha superato il parametro => non va ritentato l'invio
                command.LogNotificaDaModificare.Stato = StatoLogNotificaInviata.Errore
                command.LogNotificaDaModificare.MessaggioRisposta += " -- STOPPATO (numero tentativi superato) "

            End If

        End If

        Return GenericProvider.LogNotifiche.UpdateLogNotificaInviata(command.LogNotificaDaModificare)

    End Function

#End Region

#Region " Get Notifiche "

    ''' <summary>
    ''' Restituisce le notifiche in stato "Da inviare", limitate al numero specificato nel parametro. 
    ''' Se il limite viene impostato a 0, vengono restituite tutte le notifiche da inviare presenti.
    ''' </summary>
    ''' <param name="numeroNotificheDaInviare"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetLogNotificheDaInviare(numeroNotificheDaInviare As Integer) As IEnumerable(Of Entities.LogNotificaInviata)

        Return GenericProvider.LogNotifiche.GetLogNotificheInvioByStato(numeroNotificheDaInviare, Enumerators.StatoLogNotificaInviata.DaInviare)

    End Function

    ''' <summary>
    ''' Restituisce la lista di notifiche di un dato paziente per una data operazione. 
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="operazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetLogNotificheDaInviareByOperazione(codicePaziente As Long, operazione As OperazioneLogNotifica) As List(Of Entities.LogNotificaInviata)

        Return GenericProvider.LogNotifiche.GetLogNotificheInvioByOperazione(codicePaziente, operazione)

    End Function

    ''' <summary>
    ''' Restituisce l'esistenza di una notifica senza risultato risposta (ovvero ancora da processare) per un dato paziente e per una data operazione. 
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="operazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ExistLogNotificaDaInviare(codicePaziente As Long, operazione As OperazioneLogNotifica) As Boolean

        Dim exist As Boolean = False

        Dim list As List(Of Entities.LogNotificaInviata) = GetLogNotificheDaInviareByOperazione(codicePaziente, operazione)
        If Not list Is Nothing AndAlso list.Count > 0 AndAlso String.IsNullOrWhiteSpace(list.FirstOrDefault().ServiceResponse) Then
            exist = True
        End If
        Return exist

    End Function

	Public Function GetLogNotificaInvio(idNotifica As Long) As Entities.LogNotificaInviata

		Return GenericProvider.LogNotifiche.GetLogNotificaInvio(idNotifica)

	End Function
	Public Function GetLogNotificaRicevutaAcnByIdNotifica(idNotifica As Long) As Entities.LogNotificaRicevutaACN

		Return GenericProvider.LogNotifiche.GetLogNotificaRicevutaAcnByIdNotifica(idNotifica)

	End Function

#End Region

    Public Class InsertLogNotificheDaInviareCommand
        Public Property CodicePaziente As Long
        Public Property Operazione As OperazioneLogNotifica
        Public Property FunzionalitaNotifica As String
        Public Property EventoNotifica As String
        Public Property Operatore As String
    End Class

    Public Class InsertLogNotificheDaInviareResult
        Inherits BizGenericResult

        Public Property ListIdLogNotificheDaInviare As List(Of Long?)

        Public Sub New()
            ListIdLogNotificheDaInviare = New List(Of Long?)
        End Sub

    End Class

    ''' <summary>
    ''' Inserimento log notifiche
    ''' </summary>
    ''' <param name="command"></param>
    ''' <returns></returns>
    Public Function InsertLogNotificheDaInviare(command As InsertLogNotificheDaInviareCommand) As InsertLogNotificheDaInviareResult

        Dim result As New InsertLogNotificheDaInviareResult()
        result.Success = True
        result.Message = String.Empty
        result.ListIdLogNotificheDaInviare = New List(Of Long?)()

        ' Recupero i poli abilitati all'evento-funzionalità
        Dim idPoliAbilitati As List(Of Integer) = Nothing

        Using biz As New BizServizioIntegrazione(GenericProvider, Settings, ContextInfos)
            idPoliAbilitati = biz.GetIdPoliAbilitati(command.EventoNotifica, command.FunzionalitaNotifica)
        End Using

        If idPoliAbilitati.IsNullOrEmpty() Then
            result.Success = False
            result.Message = String.Format("Notifica non inviata: nessun polo abilitato per evento: {0}, funzionalità: {1}", command.EventoNotifica, command.FunzionalitaNotifica)
            Return result
        End If

        Dim now As Date = Date.Now
        Dim codiceAusiliarioPaziente As String = String.Empty

        Using bizPaziente As New BizPaziente(GenericProvider, Settings, ContextInfos, Nothing)
            codiceAusiliarioPaziente = bizPaziente.GetCodiceAusiliario(command.CodicePaziente)
        End Using

        ' Inserimento di una notifica per ogni polo abilitato
        Dim success As Boolean = True
        Dim message As New Text.StringBuilder()
        Dim newIdLogsInserted As New List(Of Long?)()

        For Each idPolo As Integer In idPoliAbilitati

            Dim insertResult As New Entities.LogNotificaInviata()

            Try
                insertResult = InsertLogNotificaInviata(command.CodicePaziente, codiceAusiliarioPaziente, String.Empty, Nothing, String.Empty, command.Operazione, CLng(idPolo), command.Operatore)
            Catch ex As Exception
                Common.Utility.EventLogHelper.EventLogWrite(ex, String.Format("Eccezione inserimento LogNotificaInviata - Polo: {0} - Paz: {1} - Operazione: {2}", idPolo.ToString(), command.CodicePaziente.ToString(), [Enum].GetName(GetType(OperazioneLogNotifica), command.Operazione).ToString()), ContextInfos.IDApplicazione)
                message.AppendLine(ex.Message)
            End Try

            If insertResult Is Nothing Then
                success = False
            Else
                newIdLogsInserted.Add(insertResult.IdNotifica)
            End If

        Next

        result.Success = success
        result.Message = message.ToString()

        If newIdLogsInserted.Count > 0 Then
            For Each idLogNotificaVac As Long In newIdLogsInserted
                result.ListIdLogNotificheDaInviare.Add(idLogNotificaVac)
            Next
        End If

        Return result

    End Function

#End Region

#End Region

End Class