Imports System.Collections.Generic

Namespace Entities

    Public Class MergeInfo

        Public NomeTabellaPadre As String
        Public CampiIndicePadre As List(Of String)
        Public CampoFkPadre As String
        Public CampiSelectPadre As List(Of String)
        Public CampoCodicePazientePadre As String
        Public CampoCodicePazienteOldPadre As String
        Public OrdinePadre As Integer

        Public NomeTabellaFiglia As String
        Public CampiIndiceFiglia As List(Of String)
        Public CampoFkFiglia As String
        Public CampiSelectFiglia As List(Of String)
        Public CampoCodicePazienteFiglia As String
        Public CampoCodicePazienteOldFiglia As String
        Public OrdineFiglia As Integer

        Public NomeSequence As String

    End Class

End Namespace
