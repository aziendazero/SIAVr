Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities

Public Class BizServiceCovid19
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
    Public Function GetTamponiFrontiera() As List(Of TamponeDiFrontiera)
        Using BizRilevazioniCovid19 As New BizRilevazioniCovid19(GenericProvider, Settings, ContextInfos)
            Return BizRilevazioniCovid19.GetTamponiFrontiera()
        End Using
    End Function
    Public Function GetTamponiFrontieraPerPazientiNonIdentificati() As List(Of TamponeDiFrontiera)
        Using BizRilevazioniCovid19 As New BizRilevazioniCovid19(GenericProvider, Settings, ContextInfos)
            Return BizRilevazioniCovid19.GetTamponiFrontieraPerPazientiNonIdentificati()
        End Using
    End Function
    Public Function InserimentoPazienteCentraleLocale(pazienteAura As BizCovid19Tamponi.PazienteAura) As BizGenericResult
        Using BizCovid19Tamponi As New BizCovid19Tamponi(GenericProvider, Settings, ContextInfos)
            Return BizCovid19Tamponi.InserimentoPazienteCentraleLocale(pazienteAura)
        End Using
    End Function
    Public Function GetTipologieTamponi() As IEnumerable(Of Entities.TipologiaTampone)
        Using BizAnagrafiche As New BizAnagrafiche(GenericProvider, Settings, ContextInfos)
            Return BizAnagrafiche.GetTipologieTamponi()
        End Using
    End Function
    Public Function GetCodiciPazientiByCodiceFiscale(codiceFiscale As String) As IEnumerable(Of String)
        Using BizPaziente As New BizPaziente(GenericProvider, Settings, ContextInfos, LogOptions)
            Return BizPaziente.GetCodiciPazientiByCodiceFiscale(codiceFiscale)
        End Using
    End Function
    Public Function GetCodicePazientiByCodiceRegionale(codice As String) As List(Of String)
        Using BizPaziente As New BizPaziente(GenericProvider, Settings, ContextInfos, LogOptions)
            Return BizPaziente.GetCodicePazienteByCodiceRegionale(codice)
        End Using
    End Function
    Public Function TipiDiCodifica() As List(Of Codifica)
        Using BizCodifiche As New BizCodifiche(GenericProvider, Settings, ContextInfos, LogOptions)
            Return BizCodifiche.GetCodifiche("PET_ESITO").ToList()
        End Using
    End Function
    Public Function GetResultEsiti() As List(Of ResultEsiti)
        Using BizRilevazioniCovid19 As New BizRilevazioniCovid19(GenericProvider, Settings, ContextInfos)
            Return BizRilevazioniCovid19.GetResultEsiti()
        End Using
    End Function
    Public Function GetStatiEpisodio() As List(Of StatiEpisodio)
        Using BizRilevazioniCovid19 As New BizRilevazioniCovid19(GenericProvider, Settings, ContextInfos)
            Return BizRilevazioniCovid19.GetStatiEpisodio()
        End Using
    End Function
    Public Function EpisodiApertiPerPaziente(codicePaziente As Long) As List(Of Long)
        Using BizRilevazioniCovid19 As New BizRilevazioniCovid19(GenericProvider, Settings, ContextInfos)
            Return BizRilevazioniCovid19.EpisodiApertiPerPaziente(codicePaziente)
        End Using
    End Function
    Public Function GetEpisodiByPaziente(codicePaziente As Long) As List(Of EpisodioPaziente)
        Using BizRilevazioniCovid19 As New BizRilevazioniCovid19(GenericProvider, Settings, ContextInfos)
            Return BizRilevazioniCovid19.GetEpisodiByPaziente(codicePaziente)
        End Using
    End Function
    Public Function GetIdEpisodiByPaziente(codicePaziente As Long) As List(Of Long)
        Using BizRilevazioniCovid19 As New BizRilevazioniCovid19(GenericProvider, Settings, ContextInfos)
            Return BizRilevazioniCovid19.GetIdEpisodiByPaziente(codicePaziente)
        End Using
    End Function
    Public Function GetEpisodioById(idEpisodio As Long) As EpisodioPaziente
        Using BizRilevazioniCovid19 As New BizRilevazioniCovid19(GenericProvider, Settings, ContextInfos)
            Return BizRilevazioniCovid19.GetEpisodio(idEpisodio)
        End Using
    End Function
    Public Function SalvaNuovoEpisodio(episodio As EpisodioPaziente) As Long
        Using BizRilevazioniCovid19 As New BizRilevazioniCovid19(GenericProvider, Settings, ContextInfos)
            Dim ricoveriDelete As New List(Of Long)
            Dim tamponiDelete As New List(Of Long)
            Dim diariaDelete As New List(Of Long)
            Dim contattiDelete As New List(Of Long)
            Return BizRilevazioniCovid19.SalvaEpisodio(episodio, ricoveriDelete, tamponiDelete, diariaDelete, contattiDelete)
        End Using
    End Function
    Public Function UpdateStatoElab(idTampone As Integer, statoElab As Integer, pazCodice As Long, dataElaborazione As DateTime)
        Using BizRilevazioniCovid19 As New BizRilevazioniCovid19(GenericProvider, Settings, ContextInfos)
            BizRilevazioniCovid19.UpdateStatoElab(idTampone, statoElab, pazCodice, dataElaborazione)
        End Using
    End Function
    Public Function UpdateStatoElab(idTampone As Integer, statoElab As Integer, data As DateTime, messaggio As String, pazCodice As Long, dataElaborazione As DateTime)
        Using BizRilevazioniCovid19 As New BizRilevazioniCovid19(GenericProvider, Settings, ContextInfos)
            BizRilevazioniCovid19.UpdateStatoElab(idTampone, statoElab, data, messaggio, pazCodice, dataElaborazione)
        End Using
    End Function
    Public Function UpdateStatoElabSDG(idTampone As Integer, statoElab As Integer, messaggio As String, pazCodice As Long, dataElaborazione As DateTime)
        Using BizRilevazioniCovid19 As New BizRilevazioniCovid19(GenericProvider, Settings, ContextInfos)
            BizRilevazioniCovid19.UpdateStatoElabSDG(idTampone, statoElab, messaggio, pazCodice, dataElaborazione)
        End Using
    End Function
    Public Function UpdateStatoElab(idTampone As Integer, statoElab As Integer, messaggio As String, pazCodice As Long, dataElaborazione As DateTime)
        Using BizRilevazioniCovid19 As New BizRilevazioniCovid19(GenericProvider, Settings, ContextInfos)
            BizRilevazioniCovid19.UpdateStatoElab(idTampone, statoElab, messaggio, pazCodice, dataElaborazione)
        End Using
    End Function
    Public Function UpdateUtenteInserimento(idTampone As Long, utenteInserimento As Long)
        Using BizRilevazioniCovid19 As New BizRilevazioniCovid19(GenericProvider, Settings, ContextInfos)
            BizRilevazioniCovid19.UpdateUtenteInserimento(idTampone, utenteInserimento)
        End Using
    End Function


    Public Function GetEpisodio(idTampone As Long) As EpisodioPaziente
        Using BizRilevazioniCovid19 As New BizRilevazioniCovid19(GenericProvider, Settings, ContextInfos)
            Return BizRilevazioniCovid19.GetEpisodio(idTampone)
        End Using
    End Function
    Public Function UpdateError(errore As String, IdTampone As Integer, pazCodice As Long, dataElaborazione As DateTime)
        Using BizRilevazioniCovid19 As New BizRilevazioniCovid19(GenericProvider, Settings, ContextInfos)
            BizRilevazioniCovid19.UpdateError(errore, IdTampone, pazCodice, dataElaborazione)
        End Using
    End Function
    Public Function UpdateErrorNoPaz(errore As String, IdTampone As Integer, dataElaborazione As DateTime)
        Using BizRilevazioniCovid19 As New BizRilevazioniCovid19(GenericProvider, Settings, ContextInfos)
            BizRilevazioniCovid19.UpdateErrorNoPaz(errore, IdTampone, dataElaborazione)
        End Using
    End Function

#End Region
#Region "Paziente"
    Public Function InserisciPaziente(inserisciPazienteCommand As BizPaziente.InserisciPazienteCommand) As BizResult
        Using BizPaziente As New BizPaziente(GenericProvider, Settings, ContextInfos, Nothing)
            Return BizPaziente.InserisciPaziente(inserisciPazienteCommand)
        End Using
    End Function
#End Region
End Class
