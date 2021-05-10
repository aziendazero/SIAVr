Imports System.Collections.Generic

Public Interface IProgressiviProvider

    Sub LockProgressivo(codiceProgressivo As String, anno As Integer)

    Function SelectProgressivo(codice As String, anno As Integer) As Entities.Progressivi

    Sub UpdateProgressivo(progressivo As Integer, codice As String, anno As Integer)

    Function LoadProgressivi(codice As String) As List(Of Entities.Progressivi)

    Sub InsertProgressivo(progressivo As Entities.Progressivi)

End Interface
