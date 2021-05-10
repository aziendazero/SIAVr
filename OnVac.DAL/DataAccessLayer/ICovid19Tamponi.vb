Imports System.Collections.Generic

Public Interface ICovid19Tamponi
    Function GetTamponiById(CF As Long) As List(Of TamponeDiFrontiera)
    Function GetTamponiFrontiera() As List(Of TamponeDiFrontiera)
    Function UpdateError(errore As String, IdTampone As Integer, pazCodice As Long, dataElaborazione As Date)
    Function UpdateStatoElab(IdTampone As Integer, statoElab As Integer, pazCodice As Long, dataElab As Date) As Object
    Function UpdateStatoElab(IdTampone As Integer, statoElab As Integer, dataGuarigione As Date, messaggio As String, pazCodice As Long, dataElab As Date) As Object
    Function UpdateStatoElabSDG(IdTampone As Integer, statoElab As Integer, messaggio As String, pazCodice As Long, dataElab As Date) As Object
    Function UpdateStatoElab2(IdTampone As Integer, statoElab As Integer, messaggio As String, pazCodice As Long, dataElab As Date) As Object
    Function GetAllTamponiById(Id As Long) As List(Of TamponeDatiRidotti)
    Function GetTamponiOrfani(ricerca As RicercaTamponiOrfani) As List(Of TamponeOrfano)
    Sub UpdateTamponeOrfano(idTampone As Long, codiceFiscale As String, pazCodice As Long, dataModifica As DateTime, note As String, ulss As String)
    Sub UpdateUlssTamponeOrfanp(idTampone As Long, ulss As String)
    Function ElencoEsitiOrfani(q As String) As IEnumerable(Of String)
    Function GetTamponiFrontieraPerPazientiNonIdentificati() As List(Of TamponeDiFrontiera)
    Function ContaTamponiPaziente(id As Long) As Integer
    Function UpdateErrorNoPaz(errore As String, IdTampone As Integer, dataElaborazione As Date) As Object
End Interface
