Imports System.Collections.Generic
Imports System.Collections.ObjectModel

Public Class BizGenericResult

    Public Success As Boolean
    Public Message As String

    Public Sub New()
        Me.New(True, String.Empty)
    End Sub

    Public Sub New(success As Boolean, message As String)
        Me.Success = success
        Me.Message = message
    End Sub

End Class

Public Class BizResult

#Region " Properties "

    Private _Success As Boolean = True
    Public Property Success() As Boolean
        Get
            Return _Success
        End Get
        Set(value As Boolean)
            _Success = value
        End Set
    End Property

    Private _Messages As ResultMessageCollection
    Public Property Messages() As ResultMessageCollection
        Get
            If _Messages Is Nothing Then
                _Messages = New ResultMessageCollection()
            End If
            Return _Messages
        End Get
        Protected Set(value As ResultMessageCollection)
            _Messages = value
        End Set
    End Property

    Public Function GetResultMessageCollection(success As Boolean) As ResultMessageCollection

        Return New ResultMessageCollection(Me.Messages.Where(Function(rm) rm.Success = success))

    End Function

#End Region

#Region " Constructors "

    Public Sub New()
        Me.New(True, New String() {}, Nothing)
    End Sub

    Public Sub New(success As Boolean, messageDescription As String)
        Me.New(success, messageDescription, Nothing)
    End Sub

    Public Sub New(success As Boolean, exception As Exception)
        Me.New(success, New String() {}, exception)
    End Sub

    Public Sub New(success As Boolean, messagesDescription As String, exception As Exception)
        Me.New(success, New String() {messagesDescription}, exception)
    End Sub

    Public Sub New(success As Boolean, messagesDescription As IEnumerable(Of String), exception As Exception)

        Dim resultMessages As New List(Of ResultMessage)()

        If Not messagesDescription Is Nothing Then

            For Each messageDescription As String In messagesDescription
                resultMessages.Add(New ResultMessage(messageDescription))
            Next

        End If

        If Not exception Is Nothing Then
            resultMessages.Add(New ResultMessage(exception.ToString()))
        End If

        Me.Messages = New ResultMessageCollection(resultMessages)

        Me.Success = success

    End Sub

    Public Sub New(success As Boolean, resultMessages As IEnumerable(Of ResultMessage))

        Me.Messages = New ResultMessageCollection(resultMessages)

        Me.Success = success

    End Sub

#End Region

#Region " Classes "

    Public Class ResultMessage

        Public Description As String
        Public Success As Boolean

        Public Sub New(description As String)
            Me.New(description, True)
        End Sub

        Public Sub New(description As String, success As Boolean)
            Me.Description = description
            Me.Success = success
        End Sub

    End Class

    Public Class ResultMessageCollection
        Inherits ReadOnlyCollection(Of ResultMessage)

        Public Sub New()
            MyBase.New(New List(Of ResultMessage)())
        End Sub

        Public Sub New(resultMessages As IEnumerable(Of ResultMessage))
            MyBase.New(New List(Of ResultMessage)(resultMessages))
        End Sub

        Public Function ToHtmlString() As String
            Return Me.ToString("<br/>")
        End Function

        Public Function ToJavascriptString() As String
            Return Me.ToString("\n")
        End Function

        Public Overrides Function ToString() As String
            Return Me.ToString(Environment.NewLine)
        End Function

        Private Overloads Function ToString(breakLineString As String) As String

            If Me.Items.Count = 0 Then Return String.Empty

            Dim text As New System.Text.StringBuilder()

            If Me.Items.Count > 1 Then

                For i As Integer = 0 To Me.Items.Count - 2

                    text.Append(Me.Items(i).Description)
                    text.Append(breakLineString)

                Next

            End If

            text.Append(Me.Items(Me.Items.Count - 1).Description)

            Return text.ToString()

        End Function

        Public Function GetDescriptions() As IEnumerable(Of String)

            Dim messagesDescriptions As New List(Of String)(Me.Count)

            For Each resultMessage As BizResult.ResultMessage In Me.Items
                messagesDescriptions.Add(resultMessage.Description)
            Next

            Return messagesDescriptions

        End Function

    End Class

#End Region

End Class

