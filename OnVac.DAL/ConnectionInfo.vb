<Serializable()>
Public Class ConnectionInfo

#Region " Properties "

    Private _provider As String
    Public ReadOnly Property Provider() As String
        Get
            Return _provider
        End Get
    End Property

    Private _connectionString As String
    Public ReadOnly Property ConnectionString() As String
        Get
            Return _connectionString
        End Get
    End Property

#End Region

    Public Sub New(provider As String, connectionString As String)
        _provider = provider
        _connectionString = connectionString
    End Sub

End Class