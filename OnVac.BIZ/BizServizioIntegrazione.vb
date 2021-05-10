Imports System.Collections.Generic
Imports System.Globalization
Imports Onit.OnAssistnet.OnVac.Biz.BizVaccinazioniEseguite
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.Enumerators

Public Class BizServizioIntegrazione
    Inherits BizClass

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos)

        MyBase.New(genericprovider, settings, contextInfos, Nothing)

    End Sub

#End Region


#Region " Public "

    ''' <summary>
    ''' Restituisce gli id dei poli abilitati alla notifica dell'evento-funzionalità
    ''' </summary>
    ''' <param name="codiceEvento"></param>
    ''' <param name="codiceFunzionalita"></param>
    ''' <returns></returns>
    Public Function GetIdPoliAbilitati(codiceEvento As String, codiceFunzionalita As String) As List(Of Integer)

        Dim result As GestioneNotificheEventiResult = GestioneNotificheEventi(codiceEvento, codiceFunzionalita)

        Return result.ListaIdPoliAbilitati

    End Function

#End Region



#Region " FSE "

#End Region



#Region " Protected "

    Protected Class GestioneNotificheEventiResult

        Public Success As Boolean
        Public ListaIdPoliAbilitati As List(Of Integer)

    End Class

    ''' <summary>
    ''' Dato il codice dell'evento e il codice della funzionalità, restituisce la lista dei poli a cui notificare l'evento, 
    ''' se la coppia evento-funzionalità è associata al polo e se il polo è abilitato alle notifiche.
    ''' Se trova almeno un polo, imposta il flag Success a true, altrimenti a false
    ''' </summary>
    ''' <param name="codEvento"></param>
    ''' <param name="codFunzionalita"></param>
    ''' <returns></returns>
    Protected Function GestioneNotificheEventi(codEvento As String, codFunzionalita As String) As GestioneNotificheEventiResult

        Dim result As New GestioneNotificheEventiResult()
        result.Success = False

        Dim myEvento As EventoNotifica = GenericProvider.EventiNotifiche.GetEventoNotificaByCodice(codEvento)
        Dim myFunzionalita As FunzionalitaNotifica = Nothing
        If Not String.IsNullOrWhiteSpace(codFunzionalita) Then
            myFunzionalita = GenericProvider.EventiNotifiche.GetFunzionalitaNotificaByCodice(codFunzionalita)
        End If


        If Not myEvento Is Nothing AndAlso Not myFunzionalita Is Nothing Then

            Dim listIdPoli As List(Of Integer) = GenericProvider.EventiNotifiche.GetIdPoliAbilitatiNotifica(myEvento.IdEvento, myFunzionalita.IdFunzionalita)

            If Not listIdPoli.IsNullOrEmpty() Then
                result.Success = True
                result.ListaIdPoliAbilitati = listIdPoli
            End If

        End If

        Return result

    End Function

#End Region





End Class
