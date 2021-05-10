Public Class WarningMessage

    Enum MessageType
        ErrorMessage = 1
        InfoMessage = 2
        AlertMessage = 3
    End Enum

    Private Shared _objWarning As System.Web.UI.HtmlControls.HtmlGenericControl
    Private Shared _filled As Boolean

    Public ReadOnly Property Filled() As Boolean
        Get
            Return _filled
        End Get
    End Property

    Sub New(ByVal obj As System.Web.UI.HtmlControls.HtmlGenericControl)
        _objWarning = obj
        _objWarning.InnerText = ""
    End Sub

    Public Sub StatusMessage(ByVal message As String)
        StatusMessage(message, MessageType.InfoMessage)
    End Sub

    Public Sub StatusMessage(ByVal message As String, ByVal Type As MessageType)

        If Not message Is Nothing AndAlso message <> String.Empty Then

            Dim _current As New Label

            Select Case Type
                Case MessageType.AlertMessage
                    _current.Style.Add("color", "blue")
                Case MessageType.ErrorMessage
                    _current.Style.Add("color", "red")
                Case MessageType.InfoMessage
                    _current.Style.Add("color", "darkgreen")
            End Select
            _current.Text = message
            _objWarning.Controls.Add(_current)
            _filled = True
        End If

    End Sub

End Class
