Imports System.Configuration

Public Class ConfigHelper

    ''' <summary>
    ''' Restituisce una stringa contenente il valore del parametro specificato nel web.config.
    ''' Se il parametro non esiste, restituisce la stringa vuota
    ''' </summary>
    ''' <param name="paramName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetParameterValue(paramName As String) As String

        Dim paramValue As String = String.Empty

        Dim appReader As New AppSettingsReader()

        Try
            paramValue = appReader.GetValue(paramName, GetType(String)).ToString()
        Catch ex As Exception
            paramValue = String.Empty
        End Try

        Return paramValue

    End Function

End Class
