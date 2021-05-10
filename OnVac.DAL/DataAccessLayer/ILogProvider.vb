Imports System.Collections.Generic
Imports Onit.OnAssistnet.Data


Public Interface ILogProvider

    Function GetDataTableTestateOperazioniGruppo(argomento As String, listOperazioni As List(Of DataLogStructure.Operazione), dataOperazioneInizio As DateTime, dataOperazioneFine As DateTime) As DataTable
    Function GetDataTableTestateAllineamentoOperazioniGruppo(listOperazioni As List(Of DataLogStructure.Operazione), dataOperazioneInizio As DateTime, dataOperazioneFine As DateTime) As DataTable

    Function CountLogsDatiVaccinali(getLogsDatiVaccinaliQuery As GetLogsDatiVaccinaliQuery) As Integer

    Function GetLogsDatiVaccinali(getLogsDatiVaccinaliQuery As GetLogsDatiVaccinaliQuery) As Entities.LogDatiVaccinali()
    Function GetLogDatiVaccinali(idLogDatiVaccinali As Int64) As Entities.LogDatiVaccinali

    Function GetArgomentiLog(soloAttivi As Boolean, listCodiciArgomenti As List(Of String)) As List(Of Log.DataLogStructure.Argomento)

    Sub InsertLogDatiVaccinali(logDatiVaccinali As LogDatiVaccinali)

End Interface


Public Class GetLogsDatiVaccinaliQuery

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

    Public Property IncludeInfos As Boolean

    Public Property PagingOptions As PagingOptions

    Public Sub New()
        Me.PagingOptions = New PagingOptions()
    End Sub

End Class
