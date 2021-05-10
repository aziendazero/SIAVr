Imports Onit.OnAssistnet.OnVac.Entities

Public Class TypesHelper

    Public Shared Function GetIntFromString(value As String) As Integer?

        Dim conv As Integer? = Nothing

        If Not String.IsNullOrWhiteSpace(value) Then
            conv = Integer.Parse(value)
        End If

        Return conv

    End Function

    Public Shared Function GetStringFromNullableInt(val As Integer?) As String

        If val.HasValue Then
            Return val.ToString()
        Else
            Return String.Empty
        End If

    End Function

#Region " Si possono usare per la gestione dei value delle checkbox "

    Public Shared Function GetStringFromBoolean(value As Boolean) As String

        Dim conv As String

        If value Then
            conv = "S"
        Else
            conv = "N"
        End If

        Return conv

    End Function

    Public Shared Function GetBooleanFromString(value As String) As Boolean

        If value = "S" Then
            Return True
        Else
            Return False
        End If

    End Function

#End Region

End Class
