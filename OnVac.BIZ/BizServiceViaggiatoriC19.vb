Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities
Public Class BizServiceViaggiatoriC19
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
    Public Function GetComuni() As List(Of Comune)
        Using BizComuni As New BizComuni(GenericProvider, Settings, ContextInfos, Nothing)
            Return BizComuni.GetComuni()
        End Using
    End Function

    Public Function GetViaggiatori() As List(Of Viaggiatore)
        Return GenericProvider.Covid19Viaggiatori.GetViaggiatori()
    End Function


    Public Function GetStati(acronimo As String) As List(Of Stato)
        Using BizStatiAnagrafici As New BizStatiAnagrafici(GenericProvider, Settings, ContextInfos)
            Return BizStatiAnagrafici.GetStati(acronimo)
        End Using
    End Function
    ''' <summary>
    ''' Metodo che permette di andare a cambiare lo stato della riga del viaggiatore selezionato
    ''' </summary>
    ''' <param name="statoElab"> numero che identifica lo stato, id della tabella T_STATI</param>
    ''' <param name="dataElab">data di elaborazione del record</param>
    ''' <param name="noteElaborazione">note relative al record</param>
    ''' <param name="codiceFiscale">codice fiscale del viaggiatore</param>
    ''' <param name="dataRientro">Data di rientro del viaggiatore</param>
    ''' <param name="data">data del viaggiatore</param>
    ''' <returns></returns>
    Public Function SalvaElaborazioneViaggiatore(statoElab As Integer, dataElab As Date, noteElaborazione As String, codiceFiscale As String, dataRientro As Date, data As Date)
        GenericProvider.Covid19Viaggiatori.SalvaElaborazioneViaggiatore(statoElab, dataElab, noteElaborazione, codiceFiscale, dataRientro, data)
    End Function

    Public Function GetPazienteByCF(CF As String) As List(Of InfoAssistito)
        Using BizPaziente As New BizPaziente(GenericProvider, Settings, ContextInfos, Nothing)
            Return BizPaziente.GetPazientiByCF(CF)
        End Using
    End Function

    Public Function NuovoEpisodio(episodio As EpisodioPaziente) As Long
        Using BizServiceCOVID19 As New BizServiceCovid19(GenericProvider, Settings, ContextInfos)
            Return BizServiceCOVID19.SalvaNuovoEpisodio(episodio)
        End Using
    End Function

    Public Function GetEpisodiByIdPaz(id As Long) As List(Of EpisodioPaziente)
        Using BizServiceCOVID19 As New BizServiceCovid19(GenericProvider, Settings, ContextInfos)
            Return BizServiceCOVID19.GetEpisodiByPaziente(id)
        End Using
    End Function
    Public Function Tags(elementi As IEnumerable(Of Long)) As IEnumerable(Of Tag)
        Using BizTag As New BizTag(GenericProvider, Settings, ContextInfos, Nothing)
            Return BizTag.GetTags(elementi)
        End Using
    End Function

    Public Function CercaTag(gruppo As String, filtro As String) As IEnumerable(Of Tag)
        Using BizTag As New BizTag(GenericProvider, Settings, ContextInfos, Nothing)
            Return BizTag.CercaTag(gruppo, filtro)
        End Using
    End Function
    Public Function GetStatiEpisodio() As List(Of StatiEpisodio)
        Using BizRilevazioniCovid19 As New BizRilevazioniCovid19(GenericProvider, Settings, ContextInfos)
            Return BizRilevazioniCovid19.GetStatiEpisodio()
        End Using
    End Function

    Public Function GetCodifiche(campo As String) As Collection.CodificheCollection
        Using BizCodifiche As New BizCodifiche(GenericProvider, Settings, ContextInfos, Nothing)
            Return BizCodifiche.GetCodifiche(campo)
        End Using
    End Function

    Public Function GetViaggiatoriExtraEU() As List(Of Viaggiatore)
        Return GenericProvider.Covid19Viaggiatori.GetViaggiatoriExtraEU()
    End Function
    Public Function SalvaElaborazioneViaggiatoreExtraEU(statoElab As Integer, dataElab As Date, noteElaborazione As String, codiceFiscale As String, dataRientro As Date, data As Date)
        GenericProvider.Covid19Viaggiatori.SalvaElaborazioneViaggiatoreExtraEU(statoElab, dataElab, noteElaborazione, codiceFiscale, dataRientro, data)
    End Function
End Class
