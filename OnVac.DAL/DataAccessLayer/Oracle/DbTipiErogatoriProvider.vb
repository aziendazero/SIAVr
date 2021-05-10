Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.Collections.Generic

Namespace DAL.Oracle
    Public Class DbTipiErogatoriProvider
        Inherits DbProvider
        Implements ITipiErogatoriProvider

#Region " Costruttore "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

        ''' <summary>
        ''' Restituisce la lista dei tipi erogatori
        ''' </summary>
        ''' <returns></returns>
        Public Function GetTipiErogatori() As List(Of TipoErogatoreVacc) Implements ITipiErogatoriProvider.GetTipiErogatori

            Dim list As New List(Of TipoErogatoreVacc)
            Dim ownConnection As Boolean = False

            Try
                Dim query As String =
                    " SELECT TER_ID, TER_CODICE, TER_DESCRIZIONE, TER_CODICE_AVN, TER_ORDINE, TER_OBSOLETO " +
                    " FROM T_ANA_TIPOLOGIE_EROGATORI " +
                    " ORDER BY TER_ORDINE "

                Using cmd As New OracleClient.OracleCommand(query, Connection)

                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If idr IsNot Nothing Then

                            Dim TER_ID As Integer = idr.GetOrdinal("TER_ID")
                            Dim TER_CODICE As Integer = idr.GetOrdinal("TER_CODICE")
                            Dim TER_DESCRIZIONE As Integer = idr.GetOrdinal("TER_DESCRIZIONE")
                            Dim TER_CODICE_AVN As Integer = idr.GetOrdinal("TER_CODICE_AVN")
                            Dim TER_ORDINE As Integer = idr.GetOrdinal("TER_ORDINE")
                            Dim TER_OBSOLETO As Integer = idr.GetOrdinal("TER_OBSOLETO")

                            While idr.Read()

                                Dim item As New TipoErogatoreVacc()

                                item.Id = idr.GetInt32OrDefault(TER_ID)
                                item.Codice = idr.GetStringOrDefault(TER_CODICE)
                                item.Descrizione = idr.GetStringOrDefault(TER_DESCRIZIONE)
                                item.CodiceAvn = idr.GetStringOrDefault(TER_CODICE_AVN)
                                item.Ordine = idr.GetNullableInt32OrDefault(TER_ORDINE)
                                item.Obsoleto = idr.GetStringOrDefault(TER_OBSOLETO)

                                list.Add(item)

                            End While

                        End If
                    End Using

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return list

        End Function

        Public Function GetDettaglioTipoErogatore(id As Integer) As List(Of TipoErogatoreVacc) Implements ITipiErogatoriProvider.GetDettaglioTipoErogatore

            Dim list As New List(Of TipoErogatoreVacc)
            Dim ownConnection As Boolean = False

            Try
                Dim query As String =
                    " SELECT TER_ID, TER_CODICE, TER_DESCRIZIONE, TER_CODICE_AVN, TER_ORDINE, TER_OBSOLETO " +
                    " FROM T_ANA_TIPOLOGIE_EROGATORI " +
                    " WHERE TER_ID = :id "

                Using cmd As New OracleClient.OracleCommand(query, Connection)

                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("id", id)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If idr IsNot Nothing Then

                            Dim TER_ID As Integer = idr.GetOrdinal("TER_ID")
                            Dim TER_CODICE As Integer = idr.GetOrdinal("TER_CODICE")
                            Dim TER_DESCRIZIONE As Integer = idr.GetOrdinal("TER_DESCRIZIONE")
                            Dim TER_CODICE_AVN As Integer = idr.GetOrdinal("TER_CODICE_AVN")
                            Dim TER_ORDINE As Integer = idr.GetOrdinal("TER_ORDINE")
                            Dim TER_OBSOLETO As Integer = idr.GetOrdinal("TER_OBSOLETO")

                            While idr.Read()

                                Dim item As New TipoErogatoreVacc()

                                item.Id = idr.GetInt32OrDefault(TER_ID)
                                item.Codice = idr.GetStringOrDefault(TER_CODICE)
                                item.Descrizione = idr.GetStringOrDefault(TER_DESCRIZIONE)
                                item.CodiceAvn = idr.GetStringOrDefault(TER_CODICE_AVN)
                                item.Ordine = idr.GetNullableInt32OrDefault(TER_ORDINE)
                                item.Obsoleto = idr.GetStringOrDefault(TER_OBSOLETO)

                                list.Add(item)

                            End While

                        End If
                    End Using

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return list

        End Function

        Public Function GetDettaglioTipoErogatoreFromCodice(codice As String) As List(Of TipoErogatoreVacc) Implements ITipiErogatoriProvider.GetDettaglioTipoErogatoreFromCodice

            Dim list As New List(Of TipoErogatoreVacc)
            Dim ownConnection As Boolean = False

            Try
                Dim query As String =
                    " SELECT TER_ID, TER_CODICE, TER_DESCRIZIONE, TER_CODICE_AVN, TER_ORDINE, TER_OBSOLETO " +
                    " FROM T_ANA_TIPOLOGIE_EROGATORI " +
                    " WHERE TER_CODICE = :codice "

                Using cmd As New OracleClient.OracleCommand(query, Connection)

                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("codice", codice)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If idr IsNot Nothing Then

                            Dim TER_ID As Integer = idr.GetOrdinal("TER_ID")
                            Dim TER_CODICE As Integer = idr.GetOrdinal("TER_CODICE")
                            Dim TER_DESCRIZIONE As Integer = idr.GetOrdinal("TER_DESCRIZIONE")
                            Dim TER_CODICE_AVN As Integer = idr.GetOrdinal("TER_CODICE_AVN")
                            Dim TER_ORDINE As Integer = idr.GetOrdinal("TER_ORDINE")
                            Dim TER_OBSOLETO As Integer = idr.GetOrdinal("TER_OBSOLETO")

                            While idr.Read()

                                Dim item As New TipoErogatoreVacc()

                                item.Id = idr.GetInt32OrDefault(TER_ID)
                                item.Codice = idr.GetStringOrDefault(TER_CODICE)
                                item.Descrizione = idr.GetStringOrDefault(TER_DESCRIZIONE)
                                item.CodiceAvn = idr.GetStringOrDefault(TER_CODICE_AVN)
                                item.Ordine = idr.GetNullableInt32OrDefault(TER_ORDINE)
                                item.Obsoleto = idr.GetStringOrDefault(TER_OBSOLETO)

                                list.Add(item)

                            End While

                        End If
                    End Using

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return list

        End Function

        Public Function InsertTipoErogatore(item As TipoErogatoreVaccCommand) As Integer Implements ITipiErogatoriProvider.InsertTipoErogatore

            Dim count As Integer = 0
            Dim ownConnection As Boolean = False

            Try
                Dim query As String =
                    " INSERT INTO T_ANA_TIPOLOGIE_EROGATORI " +
                    " (TER_CODICE, TER_DESCRIZIONE, TER_CODICE_AVN, TER_ORDINE, TER_OBSOLETO) " +
                    " VALUES (:codice, :descrizione, :codiceAvn, :ordine, :obsoleto) "

                Using cmd As New OracleClient.OracleCommand(query, Connection)

                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("codice", GetStringParam(item.Codice))
                    cmd.Parameters.AddWithValue("descrizione", GetStringParam(item.Descrizione))
                    cmd.Parameters.AddWithValue("codiceAvn", GetStringParam(item.CodiceAvn))
                    cmd.Parameters.AddWithValue("ordine", GetIntParam(item.Ordine))
                    cmd.Parameters.AddWithValue("obsoleto", GetStringParam(item.Obsoleto))

                    count = cmd.ExecuteNonQuery()

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return count

        End Function

        Public Function UpdateTipoErogatore(item As TipoErogatoreVaccCommand) As Integer Implements ITipiErogatoriProvider.UpdateTipoErogatore

            Dim count As Integer = 0
            Dim ownConnection As Boolean = False

            Try
                Dim query As String =
                    " UPDATE T_ANA_TIPOLOGIE_EROGATORI SET " +
                    " TER_CODICE = :codice ," +
                    " TER_DESCRIZIONE = :descrizione ," +
                    " TER_CODICE_AVN = :codiceAvn ," +
                    " TER_ORDINE = :ordine, " +
                    " TER_OBSOLETO = :obsoleto " +
                    " WHERE TER_ID = :id "

                Using cmd As New OracleClient.OracleCommand(query, Connection)

                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("id", GetIntParam(item.Id))
                    cmd.Parameters.AddWithValue("codice", GetStringParam(item.Codice))
                    cmd.Parameters.AddWithValue("descrizione", GetStringParam(item.Descrizione))
                    cmd.Parameters.AddWithValue("codiceAvn", GetStringParam(item.CodiceAvn))
                    cmd.Parameters.AddWithValue("ordine", GetIntParam(item.Ordine))
                    cmd.Parameters.AddWithValue("obsoleto", GetStringParam(item.Obsoleto))

                    count = cmd.ExecuteNonQuery()

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return count

        End Function

        Public Function DeleteTipoErogatore(id As Integer) As Integer Implements ITipiErogatoriProvider.DeleteTipoErogatore

            Dim count As Integer = 0
            Dim ownConnection As Boolean = False

            Try
                Dim query As String =
                    " DELETE FROM T_ANA_TIPOLOGIE_EROGATORI " +
                    " WHERE TER_ID = :id "

                Using cmd As New OracleClient.OracleCommand(query, Connection)

                    ownConnection = ConditionalOpenConnection(cmd)
                    cmd.Parameters.AddWithValue("id", GetIntParam(id))
                    count = cmd.ExecuteNonQuery()

                End Using
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return count

        End Function

#Region " Luoghi - Tipo Erogatore "

        Public Function GetTipiErogatoriFromLuogoEsecuzione(codiceLuogoEsecuzione As String) As List(Of TipoErogatoreVacc) Implements ITipiErogatoriProvider.GetTipiErogatoriFromLuogoEsecuzione

            Dim list As New List(Of TipoErogatoreVacc)
            Dim ownConnection As Boolean = False

            Try
                Dim query As String =
                    " SELECT TER_ID, TER_CODICE, TER_DESCRIZIONE, TER_CODICE_AVN, TER_ORDINE, TER_OBSOLETO " +
                    " FROM T_ANA_TIPOLOGIE_EROGATORI " +
                    " left join T_ANA_LUOGHI_TIPI_EROGATORE ON TER_ID = LTE_TER_ID " +
                    " WHERE LTE_LUO_CODICE = :codiceLuogoEsecuzione " +
                    " ORDER BY TER_ORDINE "

                Using cmd As New OracleClient.OracleCommand(query, Connection)

                    cmd.Parameters.AddWithValue("codiceLuogoEsecuzione", codiceLuogoEsecuzione)
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If idr IsNot Nothing Then

                            Dim TER_ID As Integer = idr.GetOrdinal("TER_ID")
                            Dim TER_CODICE As Integer = idr.GetOrdinal("TER_CODICE")
                            Dim TER_DESCRIZIONE As Integer = idr.GetOrdinal("TER_DESCRIZIONE")
                            Dim TER_CODICE_AVN As Integer = idr.GetOrdinal("TER_CODICE_AVN")
                            Dim TER_ORDINE As Integer = idr.GetOrdinal("TER_ORDINE")
                            Dim TER_OBSOLETO As Integer = idr.GetOrdinal("TER_OBSOLETO")

                            While idr.Read()

                                Dim item As New TipoErogatoreVacc()

                                item.Id = idr.GetInt32OrDefault(TER_ID)
                                item.Codice = idr.GetStringOrDefault(TER_CODICE)
                                item.Descrizione = idr.GetStringOrDefault(TER_DESCRIZIONE)
                                item.CodiceAvn = idr.GetStringOrDefault(TER_CODICE_AVN)
                                item.Ordine = idr.GetNullableInt32OrDefault(TER_ORDINE)
                                item.Obsoleto = idr.GetStringOrDefault(TER_OBSOLETO)

                                list.Add(item)

                            End While

                        End If
                    End Using

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return list

        End Function

        Public Function InsertLinkTipiErogatoreLuogo(idTipiErogatore As List(Of Integer), codiceLuogo As String) As Integer Implements ITipiErogatoriProvider.InsertLinkTipiErogatoreLuogo


            Dim count As Integer = 0
            Dim ownConnection As Boolean = False

            Try

                Using cmd As New OracleClient.OracleCommand()


                    Dim query As String = " INSERT ALL"

                    For i As Integer = 0 To idTipiErogatore.Count - 1
                        query += String.Format(" INTO T_ANA_LUOGHI_TIPI_EROGATORE (LTE_LUO_CODICE, LTE_TER_ID) VALUES (:codiceLuogo, :idTipoErogatore{0}) ", i)
                        cmd.Parameters.AddWithValue(String.Format("idTipoErogatore{0}", i), GetIntParam(idTipiErogatore(i)))
                        cmd.Parameters.AddWithValue("codiceLuogo", GetStringParam(codiceLuogo))
                    Next

                    query += " SELECT 1 FROM DUAL "


                    cmd.CommandText = query
                    cmd.Connection = Connection

                    ownConnection = ConditionalOpenConnection(cmd)

                    count = cmd.ExecuteNonQuery()

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return count

        End Function

        Public Function DeleteTipoErogatoreFromLuogo(codiceLuogo As String) As Integer Implements ITipiErogatoriProvider.DeleteTipoErogatoreFromLuogo

            Dim count As Integer = 0
            Dim ownConnection As Boolean = False

            Try
                Dim query As String =
                    " DELETE FROM T_ANA_LUOGHI_TIPI_EROGATORE " +
                    " WHERE LTE_LUO_CODICE = :codiceLuogo "

                Using cmd As New OracleClient.OracleCommand(query, Connection)

                    ownConnection = ConditionalOpenConnection(cmd)
                    cmd.Parameters.AddWithValue("codiceLuogo", GetStringParam(codiceLuogo))
                    count = cmd.ExecuteNonQuery()

                End Using
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return count

        End Function

#End Region

    End Class

End Namespace