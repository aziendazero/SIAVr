Imports System.Collections.Generic


Public Interface IOperatoriProvider

    Function GetListOperatori(descrizione As String) As List(Of Entities.Operatore)
    Function GetListOperatori(descrizione As String, codiceConsultorio As String) As List(Of Entities.Operatore)
    Function GetOperatoreMedicoPaziente(codicePaziente As String) As Entities.Operatore
    Function GetListConsultoriOperatori(codiceOper As String) As List(Of ConsultorioOperatore)
    Function InsertConsultorioOperatore(consultorioOperatore As ConsultorioOperatore) As Integer
    Function DeleteConsultoriOperatore(codiceOpe As String) As Integer
    Function DeleteConsultorioOperatore(codiceConsultorio As String, codiceOpe As String) As Integer
    Function GetOperatoreByIdACN(IdACN As String) As Operatore
    Function GetOperatoreByCodiceFiscale(codiceFiscale As String) As Operatore
    Function InsertOperatore(operatore As Operatore) As Integer
    Function UpdateIdACNOperatore(codiceOperatore As String, idACN As String) As Integer
    Function GetOperatoreById(codice As String) As Operatore
    Function GetOperatoriByIdRSATipoOpe(idRSA As String, OpeCodice As String, Nome As String) As List(Of Operatore)
    Function GetOperatoriByIdRSATipoOpeDISTINCT(Tipo As String, Nome As String, uslCodice As String) As List(Of Operatore)
End Interface
