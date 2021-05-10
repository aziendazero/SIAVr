Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.Entities

Public Interface IAnaBilancioProvider

    Function GetFromKey(numeroBilancio As Integer, codiceMalattia As String) As BilancioProgrammato
    Function GetBilanciPaziente(codicePaziente As Integer, tipologieMalattia As List(Of String), tipoConsegna As List(Of String)) As List(Of BilancioProgrammato)

    Function GetSezioni(numeroBilancio As Integer, codiceMalattia As String) As DataTable
    Function GetDomande(numeroBilancio As Integer, codiceMalattia As String, sessoPaziente As String) As DataTable
    Function GetDomande(numeroBilancio As Integer, codiceMalattia As String, sessoPaziente As String, dataRegistrazione As Date) As DataTable

    Function GetCondizioni(numeroBilancio As Integer, codiceMalattia As String) As DataTable
    Function GetCondizioni(numeroBilancio As Integer, codiceMalattia As String, osservazione As String) As DataTable
    Function GetRispostePossibili(numeroBilancio As Integer, codiceMalattia As String) As DataTable

    Function DeleteOsservazioniAssociate(numeroBilancio As Integer, codiceMalattia As String) As Integer
    Function InsertOsservazioneAssociata(osservazione As BilancioOsservazione) As Integer

    Function GetAnagraficaBilanciNonCompilatiPazienteByMalattia(codicePaziente As Long, codiceMalattia As String) As List(Of Entities.BilancioAnagrafica)
    Function GetAnagraficaBilanciByMalattia(codiceMalattia As String) As List(Of Entities.BilancioAnagrafica)
    Function GetAnagraficaBilancio(numeroBilancio As Integer, codiceMalattia As String) As Entities.BilancioAnagrafica

    Function GetAnagraficaSezioni(numeroBilancio As Integer, codiceMalattia As String) As List(Of Entities.BilancioSezione)
    Function CountOsservazioniSezione(codiceSezione As String) As Integer
    Function InsertSezione(sezione As Entities.BilancioSezione) As Integer
    Function UpdateSezione(sezione As Entities.BilancioSezione) As Integer
    Function DeleteSezione(codiceSezione As String) As Integer

    Function GetOsservazioni() As List(Of Entities.BilancioOsservazioneAnagrafica)
    Function GetOsservazioniAssociate(numeroBilancio As Integer, codiceMalattia As String) As List(Of Entities.BilancioOsservazioneAssociata)

    Function WriteLogOsservazione(osservazione As Entities.BilancioOsservazioneAssociata, operazione As Integer, dataLog As DateTime, idUtente As Long) As Integer

End Interface