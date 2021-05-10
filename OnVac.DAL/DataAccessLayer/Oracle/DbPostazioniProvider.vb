Imports Onit.Database.DataAccessManager

Namespace DAL.Oracle

    Public Class DbPostazioniProvider
        Inherits DbProvider
        Implements IPostazioniProvider

#Region " Costruttori "

        Public Sub New(ByRef DAM As IDAM)
            MyBase.New(DAM)
        End Sub

#End Region

#Region " IPostazioniProvider "

        ''' <summary>
        ''' Restituisce il numero di postazioni presenti nella t_ana_postazioni
        ''' </summary>
        Public Function CountPostazioni() As Integer Implements IPostazioniProvider.CountPostazioni

            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim count As Integer = 0

            Dim ownConnection As Boolean = False

            Try
                cmd = Me.Connection.CreateCommand()

                cmd.CommandText = OnVac.Queries.Postazioni.OracleQueries.cntPostazioni

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Dim obj As Object = cmd.ExecuteScalar()

                If (Not obj Is Nothing AndAlso Not obj Is DBNull.Value) Then
                    count = System.Convert.ToInt32(obj)
                Else
                    count = 0
                End If

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return count

        End Function

#End Region

    End Class

End Namespace
