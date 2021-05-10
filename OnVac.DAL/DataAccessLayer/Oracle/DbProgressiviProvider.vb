Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager


Namespace DAL.Oracle

    Public Class DbProgressiviProvider
        Inherits DbProvider
        Implements IProgressiviProvider

#Region " Costruttori "

        Public Sub New(ByRef dam As IDAM)
            MyBase.New(dam)
        End Sub

#End Region

#Region " Lock progressivi "

        ''' <summary>
        ''' Effettua una "finta" query di update per fare il lock della tabella
        ''' </summary>
        ''' <param name="codice"></param>
        ''' <param name="anno"></param>
        ''' <remarks></remarks>
        Public Sub LockProgressivo(codice As String, anno As Integer) Implements IProgressiviProvider.LockProgressivo

            Dim ownConnection As Boolean = False

            Using cmd As New OracleClient.OracleCommand(Queries.Progressivi.OracleQueries.lockProgressivo, Me.Connection)

                cmd.Parameters.AddWithValue("codice", codice)
                cmd.Parameters.AddWithValue("anno", anno)

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

        End Sub

#End Region

#Region " Lettura progressivi "

        ''' <summary>
        ''' Restituisce una lista con tutti i progressivi relativi al codice specificato, ordinati per anno descrescente.
        ''' </summary>
        ''' <param name="codice"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LoadProgressivi(codice As String) As List(Of Entities.Progressivi) Implements IProgressiviProvider.LoadProgressivi

            Dim listProgressivi As List(Of Entities.Progressivi) = Nothing

            Dim ownConnection As Boolean = False

            Using cmd As New OracleClient.OracleCommand(Queries.Progressivi.OracleQueries.selProgressivi, Me.Connection)

                cmd.Parameters.AddWithValue("codice", codice)

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        listProgressivi = Me.GetProgressivi(idr)

                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listProgressivi

        End Function

        ''' <summary>
        ''' Restituisce il progressivo relativo a codice e anno specificati.
        ''' </summary>
        ''' <param name="codice"></param>
        ''' <param name="anno"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SelectProgressivo(codice As String, anno As Integer) As Entities.Progressivi Implements IProgressiviProvider.SelectProgressivo

            Dim progressivo As Entities.Progressivi = Nothing

            Dim ownConnection As Boolean = False

            Using cmd As New OracleClient.OracleCommand(Queries.Progressivi.OracleQueries.selProgressivo, Me.Connection)

                cmd.Parameters.AddWithValue("codice", codice)
                cmd.Parameters.AddWithValue("anno", anno)

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        Dim listProgressivi As List(Of Entities.Progressivi) = Me.GetProgressivi(idr)

                        If Not listProgressivi Is Nothing Then

                            progressivo = listProgressivi.FirstOrDefault()

                        End If

                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return progressivo

        End Function

        ''' <summary>
        ''' Restituisce la lista di progressivi letta in base al datareader.
        ''' </summary>
        ''' <param name="idr"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetProgressivi(idr As IDataReader) As List(Of Entities.Progressivi)

            Dim listProgressivi As New List(Of Entities.Progressivi)

            If Not idr Is Nothing Then

                Dim progressivo As Entities.Progressivi = Nothing

                Dim prg_codice As Integer = idr.GetOrdinal("prg_codice")
                Dim prg_numero As Integer = idr.GetOrdinal("prg_numero")
                Dim prg_anno As Integer = idr.GetOrdinal("prg_anno")
                Dim prg_prefisso As Integer = idr.GetOrdinal("prg_prefisso")
                Dim prg_lunghezza As Integer = idr.GetOrdinal("prg_lunghezza")
                Dim prg_max As Integer = idr.GetOrdinal("prg_max")
                Dim prg_azi_codice As Integer = idr.GetOrdinal("prg_azi_codice")

                While idr.Read()

                    progressivo = New Entities.Progressivi()

                    progressivo.Codice = idr.GetStringOrDefault(prg_codice)
                    progressivo.Progressivo = idr.GetInt64OrDefault(prg_numero)
                    progressivo.Anno = idr.GetInt32OrDefault(prg_anno)
                    progressivo.Prefisso = idr.GetStringOrDefault(prg_prefisso)
                    progressivo.Lunghezza = idr.GetInt32OrDefault(prg_lunghezza)
                    progressivo.Max = idr.GetInt64OrDefault(prg_max)
                    progressivo.CodiceAzienda = idr.GetStringOrDefault(prg_azi_codice)

                    listProgressivi.Add(progressivo)

                End While

            End If

            Return listProgressivi

        End Function

#End Region

#Region " Scrittura progressivi "

        ''' <summary>
        ''' Inserimento del progressivo specificato
        ''' </summary>
        ''' <param name="progressivo"></param>
        ''' <remarks></remarks>
        Public Sub InsertProgressivo(progressivo As Entities.Progressivi) Implements IProgressiviProvider.InsertProgressivo

            Dim ownConnection As Boolean = False

            Using cmd As New OracleClient.OracleCommand(OnVac.Queries.Progressivi.OracleQueries.insProgressivo, Me.Connection)

                cmd.Parameters.AddWithValue("codice", progressivo.Codice)
                cmd.Parameters.AddWithValue("progressivo", progressivo.Progressivo)
                cmd.Parameters.AddWithValue("anno", progressivo.Anno)
                cmd.Parameters.AddWithValue("prefisso", GetStringParam(progressivo.Prefisso, False))
                cmd.Parameters.AddWithValue("lunghezza", GetIntParam(progressivo.Lunghezza))
                cmd.Parameters.AddWithValue("max", GetLongParam(progressivo.Max))
                cmd.Parameters.AddWithValue("azienda", GetStringParam(progressivo.CodiceAzienda))

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

        End Sub

        ''' <summary>
        ''' Modifica del progressivo per codice e anno specificati
        ''' </summary>
        ''' <param name="progressivo"></param>
        ''' <param name="codice"></param>
        ''' <param name="anno"></param>
        ''' <remarks></remarks>
        Public Sub UpdateProgressivo(progressivo As Integer, codice As String, anno As Integer) Implements IProgressiviProvider.UpdateProgressivo

            Dim ownConnection As Boolean = False

            Using cmd As New OracleClient.OracleCommand(OnVac.Queries.Progressivi.OracleQueries.updProgressivo, Me.Connection)

                cmd.Parameters.AddWithValue("progressivo", progressivo)
                cmd.Parameters.AddWithValue("codice", codice)
                cmd.Parameters.AddWithValue("anno", anno)

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

        End Sub

#End Region

    End Class

End Namespace