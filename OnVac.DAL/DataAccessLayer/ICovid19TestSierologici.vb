Imports System.Collections.Generic

Public Interface ICovid19TestSierologici
    Function GetTestSierologiciByCodiceFiscale(codiceFiscale As String) As List(Of TestRapidoScaricoScreening)
    Function InsertTestSierologico(test As TestRapidoScaricoScreening) As ResultTestSierologici
    Function InsertScaricoScreening(test As TestRapidoScaricoScreening) As ResultSetPost
    Function InsertTest(test As TestRapidoScaricoScreening) As ResultTestSierologici
    Function GetTestSierologiciPaziente(codicePaziente As Long) As List(Of TestSierologicoPaziente)

End Interface
