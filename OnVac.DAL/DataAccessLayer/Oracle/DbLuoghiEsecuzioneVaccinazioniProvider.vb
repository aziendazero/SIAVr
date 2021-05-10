Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.Collections.Generic

Namespace DAL.Oracle
    Public Class DbLuoghiEsecuzioneVaccinazioniProvider
        Inherits DbProvider
        Implements ILuoghiEsecuzioneVaccinazioniProvider

#Region " Costruttore "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region
        ''' <summary>
        ''' Restituisce la lista dei luoghi di vaccinazione
        ''' </summary>
        ''' <returns></returns>
        Public Function GetLuoghiEsecuzioneVaccinazioni() As List(Of LuoghiEsecuzioneVaccinazioni) Implements ILuoghiEsecuzioneVaccinazioniProvider.GetLuoghiEsecuzioneVaccinazioni

            Dim list As List(Of LuoghiEsecuzioneVaccinazioni) = Nothing

            Dim ownConnection As Boolean = False

            Try
                Dim query As String =
                    " SELECT LUO_CODICE, LUO_DESCRIZIONE, LUO_TIPO, LUO_ORDINE, LUO_OBSOLETO, LUO_FLAG_ESTRAI_AVN, LUO_DEFAULT_CNS" +
                    " FROM T_ANA_LUOGHI_ESECUZIONE_VAC " +
                    " ORDER BY LUO_ORDINE "

                Using cmd As New OracleClient.OracleCommand(query, Connection)

                    ownConnection = ConditionalOpenConnection(cmd)

                    list = GetListLuoghiEsecuzioneVaccinazioni(cmd)

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return list

        End Function

        Public Function GetLuogoEsecuzioneVaccinazione(codice As String) As LuoghiEsecuzioneVaccinazioni Implements ILuoghiEsecuzioneVaccinazioniProvider.GetLuogoEsecuzioneVaccinazione

            Dim luogo As LuoghiEsecuzioneVaccinazioni = Nothing

            Dim ownConnection As Boolean = False

            Try
                Dim query As String =
                    " SELECT LUO_CODICE, LUO_DESCRIZIONE, LUO_TIPO, LUO_ORDINE, LUO_OBSOLETO, LUO_FLAG_ESTRAI_AVN,LUO_DEFAULT_CNS" +
                    " FROM T_ANA_LUOGHI_ESECUZIONE_VAC " +
                    " WHERE LUO_CODICE = :codice "

                Using cmd As New OracleClient.OracleCommand(query, Connection)

                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("codice", codice)

                    Dim list As List(Of LuoghiEsecuzioneVaccinazioni) = GetListLuoghiEsecuzioneVaccinazioni(cmd)

                    If Not list.IsNullOrEmpty() Then

                        luogo = list.First()

                    End If

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return luogo

        End Function

        Private Function GetListLuoghiEsecuzioneVaccinazioni(cmd As OracleClient.OracleCommand) As List(Of LuoghiEsecuzioneVaccinazioni)

            Dim list As New List(Of LuoghiEsecuzioneVaccinazioni)()

            Using idr As IDataReader = cmd.ExecuteReader()
                If idr IsNot Nothing Then

                    Dim luo_codice As Integer = idr.GetOrdinal("LUO_CODICE")
                    Dim luo_descrizione As Integer = idr.GetOrdinal("LUO_DESCRIZIONE")
                    Dim luo_tipo As Integer = idr.GetOrdinal("LUO_TIPO")
                    Dim luo_ordine As Integer = idr.GetOrdinal("LUO_ORDINE")
                    Dim luo_obsoleto As Integer = idr.GetOrdinal("LUO_OBSOLETO")
                    Dim luo_flag_estrai_avn As Integer = idr.GetOrdinal("LUO_FLAG_ESTRAI_AVN")
                    Dim luo_default_cns As Integer = idr.GetOrdinal("LUO_DEFAULT_CNS")

                    While idr.Read()

                        Dim item As New LuoghiEsecuzioneVaccinazioni()

                        item.Codice = idr.GetString(luo_codice)
                        item.Descrizione = idr.GetStringOrDefault(luo_descrizione)
                        item.Tipo = idr.GetStringOrDefault(luo_tipo)
                        item.Ordine = idr.GetNullableInt32OrDefault(luo_ordine)
                        item.Obsoleto = idr.GetStringOrDefault(luo_obsoleto)
                        item.FlagEstraiAvn = idr.GetStringOrDefault(luo_flag_estrai_avn)
                        item.IsDefaultConsultorio = idr.GetBooleanOrDefault(luo_default_cns)

                        list.Add(item)

                    End While

                End If
            End Using

            Return list

        End Function

        Public Function InsertLuoghiEsecuzioneVaccinazioni(item As LuoghiEsecuzioneVaccCommand) As Integer Implements ILuoghiEsecuzioneVaccinazioniProvider.InsertLuoghiEsecuzioneVaccinazione

            Dim count As Integer = 0

            Dim ownConnection As Boolean = False

            Try
                Dim query As String =
                    " INSERT INTO T_ANA_LUOGHI_ESECUZIONE_VAC " +
                    " (LUO_CODICE, LUO_DESCRIZIONE, LUO_TIPO, LUO_ORDINE, LUO_OBSOLETO, LUO_FLAG_ESTRAI_AVN) " +
                    " VALUES (:codice, :descrizione, :tipo, :ordine, :obsoleto, :estraiAvn) "

                Using cmd As New OracleClient.OracleCommand(query, Connection)

                    ownConnection = ConditionalOpenConnection(cmd)


                    cmd.Parameters.AddWithValue("codice", GetStringParam(item.Codice))
                    cmd.Parameters.AddWithValue("descrizione", GetStringParam(item.Descrizione))

                    If Not String.IsNullOrWhiteSpace(item.Tipo) Then
                        cmd.Parameters.AddWithValue("tipo", GetStringParam(item.Tipo))
                    Else
                        cmd.Parameters.AddWithValue("tipo", DBNull.Value)
                    End If

                    If item.Ordine.HasValue Then
                        cmd.Parameters.AddWithValue("ordine", item.Ordine.Value)
                    Else
                        cmd.Parameters.AddWithValue("ordine", DBNull.Value)
                    End If

                    cmd.Parameters.AddWithValue("obsoleto", item.Obsoleto)
                    cmd.Parameters.AddWithValue("estraiAvn", item.FlagEstraiAvn)

                    count = cmd.ExecuteNonQuery()

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return count

        End Function

        Public Function UpdateLuoghiEsecuzioneVaccinazioni(item As LuoghiEsecuzioneVaccCommand) As Integer Implements ILuoghiEsecuzioneVaccinazioniProvider.UpdateLuoghiEsecuzioneVaccinazione

            Dim count As Integer = 0

            Dim ownConnection As Boolean = False

            Try
                Dim query As String =
                    " UPDATE T_ANA_LUOGHI_ESECUZIONE_VAC SET " +
                    " LUO_DESCRIZIONE = :descrizione ," +
                    " LUO_TIPO = :tipo  ," +
                    " LUO_ORDINE = :ordine, " +
                    " LUO_OBSOLETO = :obsoleto, " +
                    " LUO_FLAG_ESTRAI_AVN = :estraiAvn " +
                    " WHERE LUO_CODICE = :codice "

                Using cmd As New OracleClient.OracleCommand(query, Connection)

                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("codice", GetStringParam(item.Codice))
                    cmd.Parameters.AddWithValue("descrizione", GetStringParam(item.Descrizione))

                    If Not String.IsNullOrWhiteSpace(item.Tipo) Then
                        cmd.Parameters.AddWithValue("tipo", GetStringParam(item.Tipo))
                    Else
                        cmd.Parameters.AddWithValue("tipo", DBNull.Value)
                    End If

                    If item.Ordine.HasValue Then
                        cmd.Parameters.AddWithValue("ordine", item.Ordine.Value)
                    Else
                        cmd.Parameters.AddWithValue("ordine", DBNull.Value)
                    End If

                    cmd.Parameters.AddWithValue("obsoleto", item.Obsoleto)
                    cmd.Parameters.AddWithValue("estraiAvn", item.FlagEstraiAvn)

                    count = cmd.ExecuteNonQuery()

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return count

        End Function

        Public Function DeleteLuoghiEsecuzioneVaccinazioni(codice As String) As Integer Implements ILuoghiEsecuzioneVaccinazioniProvider.DeleteLuoghiEsecuzioneVaccinazione

            Dim count As Integer = 0
            Dim ownConnection As Boolean = False

            Try
                Dim query As String =
                    " DELETE FROM T_ANA_LUOGHI_ESECUZIONE_VAC " +
                    " WHERE LUO_CODICE = :codice "

                Using cmd As New OracleClient.OracleCommand(query, Connection)

                    ownConnection = ConditionalOpenConnection(cmd)
                    cmd.Parameters.AddWithValue("codice", codice)
                    count = cmd.ExecuteNonQuery()

                End Using
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return count

        End Function

#Region " Metodi per Campi Obbligatori "

        Public Function IsCampoInLuogoCampiObbligatori(codLuogo As String, codCampo As String) As Boolean Implements ILuoghiEsecuzioneVaccinazioniProvider.IsCampoInLuogoCampiObbligatori

            Using cmd As New OracleClient.OracleCommand("select 1
                                                         from T_ANA_LUOGHI_CAMPI_OBBLIGATORI
                                                         where LCA_LUO_CODICE = :codLuogo and LCA_COD_CAMPO = :codCampo", Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)
                    cmd.Parameters.AddWithValue("codLuogo", codLuogo)
                    cmd.Parameters.AddWithValue("codCampo", codCampo)

                    Dim obj As Object = cmd.ExecuteScalar()
                    If obj Is Nothing OrElse obj Is DBNull.Value Then
                        Return False
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return True

        End Function

        Public Function GetDettagliCampoObbligatorioLuogo(codLuogo As String, codCampo As String) As CampoObbligLuogoVacc Implements ILuoghiEsecuzioneVaccinazioniProvider.GetDettagliCampoObbligatorioLuogo

            Dim item As New CampoObbligLuogoVacc()

            Using cmd As New OracleClient.OracleCommand(" select LCA_LUO_CODICE, LCA_COD_CAMPO, LCA_DATA_INIZIO_OBBL
                                                          from T_ANA_LUOGHI_CAMPI_OBBLIGATORI
                                                          where LCA_LUO_CODICE = :codLuogo and LCA_COD_CAMPO = :codCampo ", Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)
                    cmd.Parameters.AddWithValue("codLuogo", codLuogo)
                    cmd.Parameters.AddWithValue("codCampo", codCampo)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If idr IsNot Nothing Then

                            Dim LCA_LUO_CODICE As Integer = idr.GetOrdinal("LCA_LUO_CODICE")
                            Dim LCA_COD_CAMPO As Integer = idr.GetOrdinal("LCA_COD_CAMPO")
                            Dim LCA_DATA_INIZIO_OBBL As Integer = idr.GetOrdinal("LCA_DATA_INIZIO_OBBL")

                            While idr.Read()

                                item.CodLuogo = idr.GetString(LCA_LUO_CODICE)
                                item.CodCampo = idr.GetStringOrDefault(LCA_COD_CAMPO)
                                item.DataInizioObblig = idr.GetNullableDateTimeOrDefault(LCA_DATA_INIZIO_OBBL)

                            End While

                        End If
                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return item

        End Function

        Public Function GetCampiObbligatori() As List(Of CampoObbligLuogoVacc) Implements ILuoghiEsecuzioneVaccinazioniProvider.GetCampiObbligatori
            Dim result As New List(Of CampoObbligLuogoVacc)
            Using cmd As New OracleClient.OracleCommand(" select LCA_LUO_CODICE, LCA_COD_CAMPO, LCA_DATA_INIZIO_OBBL
                                                          from T_ANA_LUOGHI_CAMPI_OBBLIGATORI ", Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If idr IsNot Nothing Then

                            Dim LCA_LUO_CODICE As Integer = idr.GetOrdinal("LCA_LUO_CODICE")
                            Dim LCA_COD_CAMPO As Integer = idr.GetOrdinal("LCA_COD_CAMPO")
                            Dim LCA_DATA_INIZIO_OBBL As Integer = idr.GetOrdinal("LCA_DATA_INIZIO_OBBL")

                            While idr.Read()

                                Dim item As New CampoObbligLuogoVacc()

                                item.CodLuogo = idr.GetString(LCA_LUO_CODICE)
                                item.CodCampo = idr.GetStringOrDefault(LCA_COD_CAMPO)
                                item.DataInizioObblig = idr.GetNullableDateTimeOrDefault(LCA_DATA_INIZIO_OBBL)

                                result.Add(item)

                            End While

                        End If
                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return result
        End Function

        Public Function GetCampiObbligatoriByLuogo(codLuogo As String) As List(Of CampoObbligLuogoVacc) Implements ILuoghiEsecuzioneVaccinazioniProvider.GetCampiObbligatoriByLuogo
            Dim result As New List(Of CampoObbligLuogoVacc)
            Using cmd As New OracleClient.OracleCommand(" select LCA_LUO_CODICE, LCA_COD_CAMPO, LCA_DATA_INIZIO_OBBL
                                                          from T_ANA_LUOGHI_CAMPI_OBBLIGATORI
                                                          where LCA_LUO_CODICE = :codLuogo ", Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)
                    cmd.Parameters.AddWithValue("codLuogo", codLuogo)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If idr IsNot Nothing Then

                            Dim LCA_LUO_CODICE As Integer = idr.GetOrdinal("LCA_LUO_CODICE")
                            Dim LCA_COD_CAMPO As Integer = idr.GetOrdinal("LCA_COD_CAMPO")
                            Dim LCA_DATA_INIZIO_OBBL As Integer = idr.GetOrdinal("LCA_DATA_INIZIO_OBBL")

                            While idr.Read()

                                Dim item As New CampoObbligLuogoVacc()

                                item.CodLuogo = idr.GetString(LCA_LUO_CODICE)
                                item.CodCampo = idr.GetStringOrDefault(LCA_COD_CAMPO)
                                item.DataInizioObblig = idr.GetNullableDateTimeOrDefault(LCA_DATA_INIZIO_OBBL)

                                result.Add(item)

                            End While

                        End If
                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return result
        End Function

        Public Sub InsertCampoObbligatorioLuogo(item As Entities.CampoObbligLuogoVacc) Implements ILuoghiEsecuzioneVaccinazioniProvider.InsertCampoObbligatorioLuogo

            Dim ownConnection As Boolean = False

            Try
                Dim query As String =
                    " INSERT INTO T_ANA_LUOGHI_CAMPI_OBBLIGATORI " +
                    " (LCA_LUO_CODICE, LCA_COD_CAMPO, LCA_DATA_INIZIO_OBBL, LCA_UTE_ID_INSERIMENTO, LCA_DATA_INSERIMENTO) " +
                    " VALUES (:codLuogo, :codcampo, :dataInizioObbl, :idUteInserimento, :dataInserimento) "

                Using cmd As New OracleClient.OracleCommand(query, Connection)

                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("codLuogo", item.CodLuogo)
                    cmd.Parameters.AddWithValue("codcampo", item.CodCampo)

                    If item.DataInizioObblig.HasValue Then
                        cmd.Parameters.AddWithValue("dataInizioObbl", item.DataInizioObblig.Value)
                    Else
                        cmd.Parameters.AddWithValue("dataInizioObbl", DBNull.Value)
                    End If

                    cmd.Parameters.AddWithValue("idUteInserimento", item.IdUteInserimento)
                    cmd.Parameters.AddWithValue("dataInserimento", item.DataInserimento)

                    cmd.ExecuteNonQuery()
                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

        End Sub

        Public Function DeleteCampiObbligatoriLuogo(codLuogo As String) As Integer Implements ILuoghiEsecuzioneVaccinazioniProvider.DeleteCampiObbligatoriLuogo

            Dim count As Integer = 0
            Dim ownConnection As Boolean = False

            Try

                Using cmd As New OracleClient.OracleCommand(" DELETE FROM T_ANA_LUOGHI_CAMPI_OBBLIGATORI WHERE LCA_LUO_CODICE = :codLuogo ", Connection)

                    ownConnection = ConditionalOpenConnection(cmd)
                    cmd.Parameters.AddWithValue("codLuogo", codLuogo)
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