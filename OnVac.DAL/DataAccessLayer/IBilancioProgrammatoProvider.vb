Imports Onit.OnAssistnet.OnVac.Entities
Imports System.Collections.Generic

Public Interface IBilancioProgrammatoProvider

    Function GetFromKey(codicePaziente As Integer, numeroBilancio As Integer, codiceMalattia As String) As DataTable
    Function GetFromKey(codicePaziente As Integer, dataConvocazione As Date) As DataTable

    Function Exists(id As Integer) As Boolean

    Function CountFromKey(codicePaziente As Integer, dataConvocazione As Date) As Integer
    Function RecordCount(filter As Object) As Integer
    Function CountBilanciPaziente(codicePaziente As String) As Integer

    Function EditRecord(con As Object) As Boolean

    Function NewRecord(bilancio As BilancioProgrammato) As Boolean

    Function VerifyVisits(codiceMalattia As String, codicePaziente As Integer) As Boolean
    Function VerifyInterval(codiceMalattia As String, codicePaziente As Integer, dataPartenza As Date, numeroBilancio As Int16) As Boolean

    Function GetBiltoProgr(codicePaziente As Integer) As DataTable
    Function GetBilanciMalattiePaziente(codicePaziente As String, dataConvocazione As Date, stato As String) As List(Of KeyValuePair(Of Integer, String))

    Function ManGetBiltoProgr(codicePaziente As Integer, codiceMalattia As String) As IDataReader

    Function VaccinazioneAssociata(codicePaziente As Integer, dataConvocazione As Date) As Boolean

    Function GetBilanciDaSollecitare(codicePaziente As Integer) As List(Of Entities.BilancioDaSollecitare)

    Function GetLastBil(codicePaziente As Integer, codiceMalattia As String, stato As String) As Entities.BilancioInfo
    Function GetLastDateBilUnsolved(codicePaziente As Integer, codiceMalattia As String) As Date

    Function RetrieveIntervalByVisit(codiceMalattia As String, codicePaziente As Integer, numeroBilancio As Int16) As Integer
    Function RetrieveIntervalByDate(codiceMalattia As String, codicePaziente As Integer, numeroBilancio As Int16, dataPartenza As Date, isLastDateUS As Boolean) As Integer
    Function RetrieveIntervalByBilUnsolved(codiceMalattia As String, codicePaziente As Integer, numeroBilancio As Int16) As Integer

    Function DeleteRecord(codicePaziente As Integer, codiceMalattia As String) As Boolean

    Function Update(bilancio As BilancioProgrammato) As Boolean
    Function UpdateDataInvio(id As Integer, dataInvio As Date) As Boolean

    Function DeleteRecord(codicePaziente As Integer, numeroBilancio As Integer, codiceMalattia As String) As Boolean
    Function DeleteRecord(key As Object) As Boolean

    Function LoadBilanciMalattie(codicePaziente As Integer, dataConvocazione As Date) As DataTable
    Function GetDtBilanci(codiceMalattia As String) As DataTable

    Function GetRisposteBilancio(numeroBilancio As Integer, codiceMalattia As String, codicePaziente As Integer, dataVisita As Date) As DataTable
    Function GetRisposteBilancio(idVisita As Integer) As DataTable
    Function GetUltimaDataVisitaBilancio(numeroBilancio As Integer, codiceMalattia As String, codicePaziente As Integer) As Date

    'Function CalcolaPercentile(dataNascita As Date, sesso As String, tipo As Integer, valore As Double) As String
    Function GetListPercentili(anni As Integer, sesso As String, tipo As Integer, valore As Double) As List(Of Entities.Percentile)

    Function GetDtVisiteBilanci(codicePaziente As Integer) As DataTable
    Function GetDtVisiteBilanciById(listIdVisite As List(Of Int64)) As DataTable
    Function GetVisitaBilancio(idVisita As Integer) As Visita

    Class InfoBilancio

        Public Bilancio As BilancioProgrammato
        Public IsConvocazioneSoloBilancio As Boolean

        Public Sub New()
            Me.Bilancio = New BilancioProgrammato()
            Me.IsConvocazioneSoloBilancio = False
        End Sub

    End Class

    Function GetInfoBilanciNonEseguitiScaduti(codicePaziente As Integer) As List(Of InfoBilancio)

End Interface
