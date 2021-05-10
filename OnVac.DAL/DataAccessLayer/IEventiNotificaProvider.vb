Imports System.Collections.Generic

Public Interface IEventiNotificaProvider
    Function GetEventoNotificaByCodice(codiceEvento As String) As EventoNotifica
    Function GetFunzionalitaNotificaByCodice(codiceFunzionalita As String) As FunzionalitaNotifica
    Function GetIdPoliAbilitatiNotifica(idEvento As Integer, idFunzionalita As Integer) As List(Of Integer)
End Interface
