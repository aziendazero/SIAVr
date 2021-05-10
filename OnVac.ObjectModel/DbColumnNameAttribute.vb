
Imports System.Collections.Generic

Public Class DbColumnName
    Inherits System.Attribute

    Public ReadOnly Property ColumnName As List(Of String)

    Public ReadOnly Property LastName As String
        Get
            Return ColumnName.LastOrDefault(Function(x)
                                                Return Not String.IsNullOrWhiteSpace(x)
                                            End Function)
        End Get
    End Property

    Public Sub New(ByVal ParamArray nomi As String())
        ColumnName = New List(Of String)(nomi)
    End Sub


End Class