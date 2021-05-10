Imports System.Collections.Generic
Imports Onit.OnAssistnet.Data


Public Interface IAliasCentraleProvider

    Function CloneAlias(codicePazienteMaster As String, codicePazienteAlias As String, dataAlias As DateTime, idUtente As Integer, aziendaProvenienza As String) As Integer

End Interface
