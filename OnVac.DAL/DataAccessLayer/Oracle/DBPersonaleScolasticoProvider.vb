Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager


Namespace DAL.Oracle

    Public Class DBPersonaleScolasticoProvider
        Inherits DbProvider
        Implements IPersonaleScolastico

#Region " Constructors "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region "Settings"
        Public Function GetDescrizioneSettingScolastico(id As Long) As String Implements IPersonaleScolastico.GetDescrizioneSettingScolastico

            Dim descrizione As String = String.Empty
            Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()
                cmd.CommandText = Queries.SettingScolastico.OracleQueries.selDescrizione
                cmd.Parameters.AddWithValue("SPS_ID", GetStringParam(id.ToString(), False))
                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If obj Is Nothing OrElse obj Is DBNull.Value Then
                        descrizione = String.Empty
                    Else
                        descrizione = obj.ToString()
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return descrizione

        End Function
#End Region
#Region "TipoPersonale"
        Public Function GetDescrizioneTipoPersonaleScolastico(id As Long) As String Implements IPersonaleScolastico.GetDescrizioneTipoPersonaleScolastico

            Dim descrizione As String = String.Empty
            Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()
                cmd.CommandText = Queries.TipoScolastico.OracleQueries.selDescrizione
                cmd.Parameters.AddWithValue("TPS_ID", GetStringParam(id.ToString(), False))
                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If obj Is Nothing OrElse obj Is DBNull.Value Then
                        descrizione = String.Empty
                    Else
                        descrizione = obj.ToString()
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return descrizione

        End Function
#End Region
    End Class
End Namespace

