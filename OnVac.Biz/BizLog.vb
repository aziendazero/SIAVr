Imports System.Collections.Generic

Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Log
Imports Onit.OnAssistnet.Data


Public Class BizLog
    Inherits Biz.BizClass

#Region " Constructors "

    Public Sub New(ByRef genericProvider As DbGenericProvider, contextInfo As BizContextInfos)

        MyBase.New(genericProvider, Nothing, contextInfo, Nothing)

    End Sub

#End Region

#Region " Public "

    ''' <summary>
    ''' Restituisce la lista di tutti gli argomenti attivi
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetListArgomentiAttivi() As List(Of DataLogStructure.Argomento)

        Return Me.GenericProvider.Log.GetArgomentiLog(True, Nothing)

    End Function

    ''' <summary>
    ''' Restituisce la lista degli argomenti in base ai codici specificati
    ''' </summary>
    ''' <param name="codiciArgomenti"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetListArgomenti(codiciArgomenti As String()) As List(Of DataLogStructure.Argomento)

        Return Me.GenericProvider.Log.GetArgomentiLog(False, codiciArgomenti.ToList())

    End Function

    ''' <summary>
    ''' Restituisce il numero di log in base ai filtri
    ''' </summary>
    ''' <param name="logDatiVaccinaliFilter"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountLogsDatiVaccinali(logDatiVaccinaliFilter As LogDatiVaccinaliFilter) As Integer

        Return Me.GenericProvider.Log.CountLogsDatiVaccinali(Me.GetLogsDatiVaccinaliQueryFromFilter(logDatiVaccinaliFilter))

    End Function

    ''' <summary>
    ''' Restituisce un array con i dati dei log, in base ai filtri specificati.
    ''' Non viene effettuata nessuna paginazione dei risultati.
    ''' </summary>
    ''' <param name="logDatiVaccinaliFilter"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetLogsDatiVaccinali(logDatiVaccinaliFilter As LogDatiVaccinaliFilter) As Entities.LogDatiVaccinali()

        Return Me.GetLogsDatiVaccinali(logDatiVaccinaliFilter, Nothing, Nothing, True)

    End Function

    ''' <summary>
    ''' Restituisce un array con i dati dei log, in base ai filtri.
    ''' La query viene paginata in base a indice e ampiezza della pagina specificati.
    ''' </summary>
    ''' <param name="logDatiVaccinaliFilter"></param>
    ''' <param name="currentPageIndex"></param>
    ''' <param name="pageSize"></param>
    ''' <returns></returns>
    ''' <remarks>currentPageIndex: 0-based index</remarks>
    Public Function GetLogsDatiVaccinali(logDatiVaccinaliFilter As LogDatiVaccinaliFilter, currentPageIndex As Integer, pageSize As Integer) As Entities.LogDatiVaccinali()

        Return Me.GetLogsDatiVaccinali(logDatiVaccinaliFilter, currentPageIndex, pageSize, True)

    End Function

    ''' <summary>
    ''' Restituisce un array con i dati dei log, in base ai filtri.
    ''' Se currentPageIndex e pageSize sono specificati, viene effettuata la paginazione; altrimenti, vengono restituiti tutti i risultati.
    ''' </summary>
    ''' <param name="logDatiVaccinaliFilter"></param>
    ''' <param name="currentPageIndex"></param>
    ''' <param name="pageSize"></param>
    ''' <param name="includeInfos"></param>
    ''' <returns></returns>
    ''' <remarks>currentPageIndex: 0-based index</remarks>
    Public Function GetLogsDatiVaccinali(logDatiVaccinaliFilter As LogDatiVaccinaliFilter, currentPageIndex As Integer?, pageSize As Integer?, includeInfos As Boolean) As Entities.LogDatiVaccinali()

        Dim getLogsDatiVaccinaliQuery As GetLogsDatiVaccinaliQuery = Me.GetLogsDatiVaccinaliQueryFromFilter(logDatiVaccinaliFilter)

        If Not currentPageIndex Is Nothing AndAlso Not pageSize Is Nothing Then
            ' StartRecordIndex e EndRecordIndex vengono calcolati in base a Index e Size della Page
            getLogsDatiVaccinaliQuery.PagingOptions.PageIndex = currentPageIndex
            getLogsDatiVaccinaliQuery.PagingOptions.PageSize = pageSize
        Else
            ' Se indice e ampiezza pagina non sono impostati, non deve essere effettuata la paginazione
            getLogsDatiVaccinaliQuery.PagingOptions = Nothing
        End If

        getLogsDatiVaccinaliQuery.IncludeInfos = includeInfos

        Return Me.GenericProvider.Log.GetLogsDatiVaccinali(getLogsDatiVaccinaliQuery)

    End Function

    ''' <summary>
    ''' Restituisce un oggetto LogDatiVaccinali con i dati del log relativo all'id specificato
    ''' </summary>
    ''' <param name="idLogDatiVaccinali"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetLogDatiVaccinali(idLogDatiVaccinali As Int64) As Entities.LogDatiVaccinali

        Return Me.GenericProvider.Log.GetLogDatiVaccinali(idLogDatiVaccinali)

    End Function

    ''' <summary>
    ''' Restituisce un datatable con le testate per argomento, periodo e operazioni specificati
    ''' </summary>
    ''' <param name="argomento"></param>
    ''' <param name="dataInizio"></param>
    ''' <param name="dataFine"></param>
    ''' <param name="listOperazioni"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDataTableTestateOperazioniGruppo(argomento As String, dataInizio As DateTime, dataFine As DateTime, listOperazioni As List(Of DataLogStructure.Operazione)) As DataTable

        Dim dt As DataTable = Nothing

        dataFine = dataFine.AddDays(1)

        If argomento = Log.DataLogStructure.TipiArgomento.ALLINEAMENTO Then

            dt = Me.GenericProvider.Log.GetDataTableTestateAllineamentoOperazioniGruppo(listOperazioni, dataInizio, dataFine)

        Else

            dt = Me.GenericProvider.Log.GetDataTableTestateOperazioniGruppo(argomento, listOperazioni, dataInizio, dataFine)

        End If

        Return dt

    End Function

#End Region

#Region " Private "

    Private Function GetLogsDatiVaccinaliQueryFromFilter(logDatiVaccinaliFilter As LogDatiVaccinaliFilter) As GetLogsDatiVaccinaliQuery

        Dim getLogsDatiVaccinaliQuery As New GetLogsDatiVaccinaliQuery()

        getLogsDatiVaccinaliQuery.CodiceArgomento = logDatiVaccinaliFilter.CodiceArgomento
        
        If logDatiVaccinaliFilter.DataOperazioneMinima > DateTime.MinValue Then getLogsDatiVaccinaliQuery.DataOperazioneMinima = logDatiVaccinaliFilter.DataOperazioneMinima
        If logDatiVaccinaliFilter.DataOperazioneMassima > DateTime.MinValue Then getLogsDatiVaccinaliQuery.DataOperazioneMassima = logDatiVaccinaliFilter.DataOperazioneMassima

        getLogsDatiVaccinaliQuery.Operazione = logDatiVaccinaliFilter.Operazione
        getLogsDatiVaccinaliQuery.StatoOperazione = logDatiVaccinaliFilter.StatoOperazione

        If Not String.IsNullOrEmpty(logDatiVaccinaliFilter.CognomePaziente) Then getLogsDatiVaccinaliQuery.CognomePaziente = logDatiVaccinaliFilter.CognomePaziente
        If Not String.IsNullOrEmpty(logDatiVaccinaliFilter.NomePaziente) Then getLogsDatiVaccinaliQuery.NomePaziente = logDatiVaccinaliFilter.NomePaziente
        If Not String.IsNullOrEmpty(logDatiVaccinaliFilter.CodiceFiscalePaziente) Then getLogsDatiVaccinaliQuery.CodiceFiscalePaziente = logDatiVaccinaliFilter.CodiceFiscalePaziente

        If logDatiVaccinaliFilter.DataNascitaPazienteMinima > DateTime.MinValue Then getLogsDatiVaccinaliQuery.DataNascitaPazienteMinima = logDatiVaccinaliFilter.DataNascitaPazienteMinima
        If logDatiVaccinaliFilter.DataNascitaPazienteMassima > DateTime.MinValue Then getLogsDatiVaccinaliQuery.DataNascitaPazienteMassima = logDatiVaccinaliFilter.DataNascitaPazienteMassima

        getLogsDatiVaccinaliQuery.CodiceCentroVaccinalePaziente = logDatiVaccinaliFilter.CodiceCentroVaccinalePaziente

        getLogsDatiVaccinaliQuery.StatiAnagraficiPaziente = logDatiVaccinaliFilter.StatiAnagraficiPaziente

        Return getLogsDatiVaccinaliQuery

    End Function

#End Region

#Region " Types "

    <Serializable()>
    Public Class LogDatiVaccinaliFilter

        Public Property DataOperazioneMinima As DateTime?
        Public Property DataOperazioneMassima As DateTime?

        Public Property CodiceArgomento As String

        Public Property Operazione As Log.DataLogStructure.Operazione?
        Public Property StatoOperazione As Enumerators.StatoLogDatiVaccinaliCentrali?

        Public Property CognomePaziente As String
        Public Property NomePaziente As String
        Public Property CodiceFiscalePaziente As String

        Public Property DataNascitaPazienteMinima As DateTime?
        Public Property DataNascitaPazienteMassima As DateTime?

        Public Property StatiAnagraficiPaziente As Enumerators.StatoAnagrafico()

        Public Property CodiceCentroVaccinalePaziente As String
        Public Property DescrizioneCentroVaccinalePaziente As String

    End Class

#End Region

End Class
