Imports Onit.Database.DataAccessManager


Namespace DAL.Oracle

    Public Class DbCittadinanzeProvider
        Inherits DbProvider
        Implements ICittadinanzeProvider

#Region " Costruttori "

        Public Sub New(DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " ICittadinanzeProvider "

        Public Function GetCodiceCittadinanzaByCodiceIstat(codiceIstatCittadinanza As String, dataValidita As Date) As String Implements ICittadinanzeProvider.GetCodiceCittadinanzaByCodiceIstat

            Dim codiceCittadinanza As String = String.Empty

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand

                cmd.Parameters.AddWithValue("codiceIstat", GetStringParam(codiceIstatCittadinanza, False))
                cmd.Parameters.AddWithValue("dataValidita", dataValidita)

                cmd.CommandText = Queries.Cittadinanze.OracleQueries.selCodCittadinanzaByIstatStorico

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then

                        codiceCittadinanza = obj.ToString()

                    End If

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return codiceCittadinanza

        End Function

        Public Function GetCodiceCittadinanzaByCodiceIstat(codiceIstatCittadinanza As String) As String Implements ICittadinanzeProvider.GetCodiceCittadinanzaByCodiceIstat

            Dim codiceCittadinanza As String = String.Empty

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand

                cmd.Parameters.AddWithValue("codiceIstat", GetStringParam(codiceIstatCittadinanza, False))

                cmd.CommandText = Queries.Cittadinanze.OracleQueries.selCodCittadinanzeByIstat

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then

                        codiceCittadinanza = obj.ToString()

                    End If

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return codiceCittadinanza

        End Function

#End Region

    End Class

End Namespace