Imports System.Collections.Generic
Imports Onit.OnAssistnet.Data
Imports Onit.OnAssistnet.OnVac.Enumerators

Public Interface ILogNotificheProvider

	' Notifiche ricevute
	Function GetLogNotificaRicevuta(idMessaggio As String) As Entities.LogNotificaRicevuta
	Function InsertLogNotificaRicevuta(logNotificaRicevuta As Entities.LogNotificaRicevuta) As Integer
	Function UpdateLogNotificaRicevuta(logNotificaRicevuta As Entities.LogNotificaRicevuta) As Integer

	' Notifiche inviate
	Function InsertLogNotificaInviata(logNotificaInviata As Entities.LogNotificaInviata, codiceUslLocale As String) As Integer
	Function UpdateLogNotificaInviata(logNotificaInviata As Entities.LogNotificaInviata) As Integer
	Function GetLogNotificheInvioByStato(numeroNotificheDaRecuperare As Integer, statoNotifiche As Enumerators.StatoLogNotificaInviata) As IEnumerable(Of Entities.LogNotificaInviata)
	Function GetNextIdNotificaInviata() As Long
	Function GetLogNotificheInvioByOperazione(codicePaziente As Long, operazione As OperazioneLogNotifica) As List(Of Entities.LogNotificaInviata)
	Function GetLogNotificaInvio(idNotifica As Long) As LogNotificaInviata
	Function GetLogNotificaRicevutaAcnByIdNotifica(idNotifica As Long) As LogNotificaRicevutaACN
	Function CountLogNotificheRicevuteAcn(filtri As FiltriLogNotifiche) As Integer
	Function GetLogNotificheRicevuteAcn(filtri As FiltriLogNotifiche, campoOrdinamento As String, versoOrdinamento As String, pagingOptions As PagingOptions) As List(Of LogNotificaRicevutaACN)

    Function GetLogNotificheDaInviareFSE(maxRecord As Integer) As List(Of LogNotificaInviata)

End Interface
