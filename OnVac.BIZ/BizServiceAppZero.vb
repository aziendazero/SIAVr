Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities
Public Class BizServiceAppZero
    Inherits BizClass
    ''' <summary>
    ''' Biz centralizzato delle funzionalità utilizzate per il ServiceCOVID19
    ''' </summary>
    ''' <param name="genericprovider"></param>
    ''' <param name="settings"></param>
    ''' <param name="contextInfos"></param>

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos)

        MyBase.New(genericprovider, settings, contextInfos, Nothing)

    End Sub

#End Region
#Region "Funzionalità per il service"
    Public Function GetPazientiDaEpisodiConSorveglianzaAttiva() As List(Of AppZeroDatiEpisodio)
        Return GenericProvider.RilevazioniCovid19.GetPazientiDaEpisodiConSorveglianzaAttiva()
    End Function
    Public Function SintomiApp() As List(Of SintomiApp)
        Return GenericProvider.RilevazioniCovid19.SintomiApp()
    End Function
    Public Sub AggiungiDiaria(codiceEpisodio As Long, codicePaziente As Long, sintomi As List(Of Integer), note As String)
        GenericProvider.RilevazioniCovid19.InsertDiariaApp(codiceEpisodio, codicePaziente, sintomi, note, Settings.CODICE_UTENTE_PRENOTAZIONE_WEB)
    End Sub
#End Region
End Class
