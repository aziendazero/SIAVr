Imports System.Collections.Generic

Public Interface IDbRicoveriProvider
    Function SalvaRicovero(command As SalvaRicovero, codiceUtente As Long) As RicoveroPaziente
    Function GetRicoveriPaziente(codicePaziente As Long) As IEnumerable(Of RicoveroPaziente)
    Sub EliminaRicovero(codiceGruppo As String, codiceUtente As Long)
    Function CercaRicoveri(filtri As FiltriRicoveri) As IEnumerable(Of TestataRicovero)
End Interface
