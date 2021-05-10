Imports System.Collections.Generic


Public Interface IProcedureProvider

    Function GetErrorMsg() As String

    Function GetProcedure() As DataTable

    Function FillTable(procedureId As Integer, jobId As Integer, patients As ArrayList, strParameters As String, uteId As Integer) As Boolean

    Function StoreParameters(procedureId As Integer, parameters As Hashtable) As Boolean

    Function GetProcedureNoStampaRisultati() As List(Of Integer)

    Function GetPazienteElaborazione(progressivo As Long) As Entities.PazienteElaborazione

    Function InsertPazientiElaborazioni(pazienteElaborazione As Entities.PazienteElaborazione) As Integer
    Function InsertPazientiElaborazioni(convocazioniDaElaborare As List(Of Entities.ConvocazionePK), procedureId As Integer, jobId As Integer, userId As Integer) As Integer
    Function UpdatePazientiElaborazioni(progressivo As Long, codiceErrore As Integer?, descrizioneErrore As String, dataEsecuzione As DateTime, statoElaborazione As String) As Integer

    Function InsertParametriReport(parametriReport As List(Of KeyValuePair(Of String, String)), jobId As Integer) As Integer

End Interface
