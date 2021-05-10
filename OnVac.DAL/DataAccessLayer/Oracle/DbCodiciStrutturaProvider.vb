Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.DAL
Imports System.Text
Imports Onit.OnAssistnet.Data.OracleClient

Public Class DbCodiciStrutturaProvider
    Inherits DbProvider
    Implements ICodiciStrutturaProvider

#Region " Costruttori "

    Public Sub New(ByRef DAM As IDAM)

        MyBase.New(DAM)

    End Sub

#End Region

#Region " Methodi HSP"

    Public Function GetCodiciHsp(pagingOptions As Data.PagingOptions) As List(Of CodiceHSP) Implements ICodiciStrutturaProvider.GetCodiciHsp

        Dim list As New List(Of CodiceHSP)()

        Using cmd As New OracleClient.OracleCommand("select HSP_ID, HSP_CODICE, HSP_DESCRIZIONE, HSP_DATA_INIZIO_VALIDITA, HSP_DATA_FINE_VALIDITA,                                                    HSP_CODICE_ASL, HSP_INDIRIZZO, HSP_CODICE_COMUNE, COM_DESCRIZIONE from T_ANA_CODICI_HSP left                                                     join T_ANA_COMUNI on HSP_CODICE_COMUNE = COM_CODICE", Connection)

            If Not pagingOptions Is Nothing Then
                cmd.AddPaginatedQuery(pagingOptions)
            End If

            Dim ownConnection As Boolean = False

            Try
                ownConnection = ConditionalOpenConnection(cmd)

                Using idr As IDataReader = cmd.ExecuteReader()

                    If idr IsNot Nothing Then

                        Dim HSP_ID As Integer = idr.GetOrdinal("HSP_ID")
                        Dim HSP_CODICE As Integer = idr.GetOrdinal("HSP_CODICE")
                        Dim HSP_DESCRIZIONE As Integer = idr.GetOrdinal("HSP_DESCRIZIONE")
                        Dim HSP_DATA_INIZIO_VALIDITA As Integer = idr.GetOrdinal("HSP_DATA_INIZIO_VALIDITA")
                        Dim HSP_DATA_FINE_VALIDITA As Integer = idr.GetOrdinal("HSP_DATA_FINE_VALIDITA")
                        Dim HSP_CODICE_ASL As Integer = idr.GetOrdinal("HSP_CODICE_ASL")
                        Dim HSP_INDIRIZZO As Integer = idr.GetOrdinal("HSP_INDIRIZZO")
                        Dim HSP_COD_COMUNE As Integer = idr.GetOrdinal("HSP_CODICE_COMUNE")
                        Dim COM_DESCRIZIONE As Integer = idr.GetOrdinal("COM_DESCRIZIONE")

                        While idr.Read()

                            Dim item As New CodiceHSP()

                            item.Id = idr.GetStringOrDefault(HSP_ID)
                            item.CodiceHsp = idr.GetString(HSP_CODICE)
                            item.Descrizione = idr.GetStringOrDefault(HSP_DESCRIZIONE)
                            item.DataInizioValidita = idr.GetNullableDateTimeOrDefault(HSP_DATA_INIZIO_VALIDITA)
                            item.DataFineValidita = idr.GetNullableDateTimeOrDefault(HSP_DATA_FINE_VALIDITA)
                            item.CodiceAsl = idr.GetStringOrDefault(HSP_CODICE_ASL)
                            item.Indirizzo = idr.GetStringOrDefault(HSP_INDIRIZZO)
                            item.CodiceComune = idr.GetStringOrDefault(HSP_COD_COMUNE)
                            item.DescrizioneComune = idr.GetStringOrDefault(COM_DESCRIZIONE)

                            list.Add(item)

                        End While
                    End If

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

        End Using

        Return list

    End Function

    Public Function GetCountCodiciHsp() As Integer Implements ICodiciStrutturaProvider.GetCountCodiciHsp

        Dim ris As Integer = 0

        Using cmd As New OracleClient.OracleCommand("select count(*) from T_ANA_CODICI_HSP", Connection)

            Dim ownConnection As Boolean = False

            Try
                ownConnection = ConditionalOpenConnection(cmd)
                Dim obj As Object = cmd.ExecuteScalar()

                If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                    ris = Convert.ToInt32(obj)
                End If

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

        End Using

        Return ris

    End Function

    Public Function GetCodiciHspValidiByFiltro(filtro As String, pagingOptions As Data.PagingOptions) As List(Of CodiceHSP) Implements ICodiciStrutturaProvider.GetCodiciHspValidiByFiltro

        Dim list As New List(Of CodiceHSP)()

        Using cmd As New OracleClient.OracleCommand("select HSP_ID, HSP_CODICE, HSP_DESCRIZIONE, HSP_DATA_INIZIO_VALIDITA, HSP_DATA_FINE_VALIDITA,                                                    
                                                            HSP_CODICE_ASL, HSP_INDIRIZZO, HSP_CODICE_COMUNE, COM_DESCRIZIONE from T_ANA_CODICI_HSP 
                                                            left join T_ANA_COMUNI on HSP_CODICE_COMUNE = COM_CODICE left join T_ANA_USL on HSP_CODICE_ASL = USL_CODICE 
                                                            where (HSP_CODICE LIKE :filtro or HSP_DESCRIZIONE LIKE :filtro) and ((HSP_DATA_INIZIO_VALIDITA <= :oggi or HSP_DATA_INIZIO_VALIDITA is null) and (HSP_DATA_FINE_VALIDITA >= :oggi or HSP_DATA_FINE_VALIDITA is null))", Connection)

            cmd.Parameters.AddWithValue("oggi", DateTime.Today)
            cmd.Parameters.AddWithValue("filtro", "%" + filtro + "%")

            If Not pagingOptions Is Nothing Then
                cmd.AddPaginatedQuery(pagingOptions)
            End If

            Dim ownConnection As Boolean = False

            Try
                ownConnection = ConditionalOpenConnection(cmd)

                Using idr As IDataReader = cmd.ExecuteReader()
                    If idr IsNot Nothing Then

                        Dim HSP_ID As Integer = idr.GetOrdinal("HSP_ID")
                        Dim HSP_CODICE As Integer = idr.GetOrdinal("HSP_CODICE")
                        Dim HSP_DESCRIZIONE As Integer = idr.GetOrdinal("HSP_DESCRIZIONE")
                        Dim HSP_DATA_INIZIO_VALIDITA As Integer = idr.GetOrdinal("HSP_DATA_INIZIO_VALIDITA")
                        Dim HSP_DATA_FINE_VALIDITA As Integer = idr.GetOrdinal("HSP_DATA_FINE_VALIDITA")
                        Dim HSP_CODICE_ASL As Integer = idr.GetOrdinal("HSP_CODICE_ASL")
                        Dim HSP_INDIRIZZO As Integer = idr.GetOrdinal("HSP_INDIRIZZO")
                        Dim HSP_COD_COMUNE As Integer = idr.GetOrdinal("HSP_CODICE_COMUNE")
                        Dim COM_DESCRIZIONE As Integer = idr.GetOrdinal("COM_DESCRIZIONE")

                        While idr.Read()
                            Dim item As New CodiceHSP()

                            item.Id = idr.GetStringOrDefault(HSP_ID)
                            item.CodiceHsp = idr.GetString(HSP_CODICE)
                            item.Descrizione = idr.GetStringOrDefault(HSP_DESCRIZIONE)
                            item.DataInizioValidita = idr.GetNullableDateTimeOrDefault(HSP_DATA_INIZIO_VALIDITA)
                            item.DataFineValidita = idr.GetNullableDateTimeOrDefault(HSP_DATA_FINE_VALIDITA)
                            item.CodiceAsl = idr.GetStringOrDefault(HSP_CODICE_ASL)
                            item.Indirizzo = idr.GetStringOrDefault(HSP_INDIRIZZO)
                            item.CodiceComune = idr.GetStringOrDefault(HSP_COD_COMUNE)
                            item.DescrizioneComune = idr.GetStringOrDefault(COM_DESCRIZIONE)

                            list.Add(item)

                        End While

                    End If

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

        End Using

        Return list

    End Function

    Public Function GetCodiciHspByFiltro(filtro As String, pagingOptions As Data.PagingOptions) As List(Of CodiceHSP) Implements ICodiciStrutturaProvider.GetCodiciHspByFiltro

        Dim list As New List(Of CodiceHSP)()

        Using cmd As New OracleClient.OracleCommand("select HSP_ID, HSP_CODICE, HSP_DESCRIZIONE, HSP_DATA_INIZIO_VALIDITA, HSP_DATA_FINE_VALIDITA,                                                    HSP_CODICE_ASL, HSP_INDIRIZZO, HSP_CODICE_COMUNE, COM_DESCRIZIONE from T_ANA_CODICI_HSP left join T_ANA_COMUNI on HSP_CODICE_COMUNE = COM_CODICE left join T_ANA_USL on HSP_CODICE_ASL = USL_CODICE where HSP_CODICE LIKE :filtro or HSP_DESCRIZIONE LIKE :filtro", Connection)

            cmd.Parameters.AddWithValue("filtro", "%" + filtro + "%")

            If Not pagingOptions Is Nothing Then
                cmd.AddPaginatedQuery(pagingOptions)
            End If

            Dim ownConnection As Boolean = False

            Try
                ownConnection = ConditionalOpenConnection(cmd)

                Using idr As IDataReader = cmd.ExecuteReader()
                    If idr IsNot Nothing Then

                        Dim HSP_ID As Integer = idr.GetOrdinal("HSP_ID")
                        Dim HSP_CODICE As Integer = idr.GetOrdinal("HSP_CODICE")
                        Dim HSP_DESCRIZIONE As Integer = idr.GetOrdinal("HSP_DESCRIZIONE")
                        Dim HSP_DATA_INIZIO_VALIDITA As Integer = idr.GetOrdinal("HSP_DATA_INIZIO_VALIDITA")
                        Dim HSP_DATA_FINE_VALIDITA As Integer = idr.GetOrdinal("HSP_DATA_FINE_VALIDITA")
                        Dim HSP_CODICE_ASL As Integer = idr.GetOrdinal("HSP_CODICE_ASL")
                        Dim HSP_INDIRIZZO As Integer = idr.GetOrdinal("HSP_INDIRIZZO")
                        Dim HSP_COD_COMUNE As Integer = idr.GetOrdinal("HSP_CODICE_COMUNE")
                        Dim COM_DESCRIZIONE As Integer = idr.GetOrdinal("COM_DESCRIZIONE")

                        While idr.Read()
                            Dim item As New CodiceHSP()

                            item.Id = idr.GetStringOrDefault(HSP_ID)
                            item.CodiceHsp = idr.GetString(HSP_CODICE)
                            item.Descrizione = idr.GetStringOrDefault(HSP_DESCRIZIONE)
                            item.DataInizioValidita = idr.GetNullableDateTimeOrDefault(HSP_DATA_INIZIO_VALIDITA)
                            item.DataFineValidita = idr.GetNullableDateTimeOrDefault(HSP_DATA_FINE_VALIDITA)
                            item.CodiceAsl = idr.GetStringOrDefault(HSP_CODICE_ASL)
                            item.Indirizzo = idr.GetStringOrDefault(HSP_INDIRIZZO)
                            item.CodiceComune = idr.GetStringOrDefault(HSP_COD_COMUNE)
                            item.DescrizioneComune = idr.GetStringOrDefault(COM_DESCRIZIONE)

                            list.Add(item)

                        End While

                    End If

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

        End Using

        Return list

    End Function

    Public Function GetCountCodiciHspValidiByFiltro(filtro As String) As Integer Implements ICodiciStrutturaProvider.GetCountCodiciHspValidiByFiltro

        Dim ris As Integer = 0

        Using cmd As New OracleClient.OracleCommand("select count(*) from T_ANA_CODICI_HSP where (HSP_CODICE LIKE :filtro or HSP_DESCRIZIONE LIKE :filtro) and ((HSP_DATA_INIZIO_VALIDITA <= :oggi or HSP_DATA_INIZIO_VALIDITA is null) and (HSP_DATA_FINE_VALIDITA >= :oggi or HSP_DATA_FINE_VALIDITA is null))", Connection)

            cmd.Parameters.AddWithValue("filtro", "%" + filtro + "%")
            cmd.Parameters.AddWithValue("oggi", DateTime.Today)

            Dim ownConnection As Boolean = False

            Try
                ownConnection = ConditionalOpenConnection(cmd)
                Dim obj As Object = cmd.ExecuteScalar()

                If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                    ris = Convert.ToInt32(obj)
                End If

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

        End Using

        Return ris

    End Function


    Public Function GetCountCodiciHspByFiltro(filtro As String) As Integer Implements ICodiciStrutturaProvider.GetCountCodiciHspByFiltro

        Dim ris As Integer = 0

        Using cmd As New OracleClient.OracleCommand("select count(*) from T_ANA_CODICI_HSP where HSP_CODICE LIKE :filtro or HSP_DESCRIZIONE LIKE :filtro", Connection)

            cmd.Parameters.AddWithValue("filtro", "%" + filtro + "%")

            Dim ownConnection As Boolean = False

            Try
                ownConnection = ConditionalOpenConnection(cmd)
                Dim obj As Object = cmd.ExecuteScalar()

                If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                    ris = Convert.ToInt32(obj)
                End If

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

        End Using

        Return ris

    End Function

    Public Function GetDettaglioCodiceHspById(id As String) As List(Of CodiceHSP) Implements ICodiciStrutturaProvider.GetDettaglioCodiceHspById

        Dim list As New List(Of CodiceHSP)()

        Using cmd As New OracleClient.OracleCommand("select * from T_ANA_CODICI_HSP left join T_ANA_COMUNI on HSP_CODICE_COMUNE = COM_CODICE left join T_ANA_USL on HSP_CODICE_ASL = USL_CODICE where HSP_ID = :id", Connection)

            cmd.Parameters.AddWithValue("id", id)
            Dim ownConnection As Boolean = False

            Try
                ownConnection = ConditionalOpenConnection(cmd)

                Using idr As IDataReader = cmd.ExecuteReader()
                    If idr IsNot Nothing Then

                        Dim HSP_ID As Integer = idr.GetOrdinal("HSP_ID")
                        Dim HSP_CODICE As Integer = idr.GetOrdinal("HSP_CODICE")
                        Dim HSP_DESCRIZIONE As Integer = idr.GetOrdinal("HSP_DESCRIZIONE")
                        Dim HSP_DATA_INIZIO_VALIDITA As Integer = idr.GetOrdinal("HSP_DATA_INIZIO_VALIDITA")
                        Dim HSP_DATA_FINE_VALIDITA As Integer = idr.GetOrdinal("HSP_DATA_FINE_VALIDITA")
                        Dim HSP_CODICE_ASL As Integer = idr.GetOrdinal("HSP_CODICE_ASL")
                        Dim USL_DESCRIZIONE As Integer = idr.GetOrdinal("USL_DESCRIZIONE")
                        Dim HSP_INDIRIZZO As Integer = idr.GetOrdinal("HSP_INDIRIZZO")
                        Dim HSP_COD_COMUNE As Integer = idr.GetOrdinal("HSP_CODICE_COMUNE")
                        Dim COM_DESCRIZIONE As Integer = idr.GetOrdinal("COM_DESCRIZIONE")
                        Dim HSP_OSPEDALE_COMUNITA As Integer = idr.GetOrdinal("HSP_OSPEDALE_COMUNITA")

                        While idr.Read()

                            Dim item As New CodiceHSP()

                            item.Id = idr.GetStringOrDefault(HSP_ID)
                            item.CodiceHsp = idr.GetString(HSP_CODICE)
                            item.Descrizione = idr.GetStringOrDefault(HSP_DESCRIZIONE)
                            item.DataInizioValidita = idr.GetNullableDateTimeOrDefault(HSP_DATA_INIZIO_VALIDITA)
                            item.DataFineValidita = idr.GetNullableDateTimeOrDefault(HSP_DATA_FINE_VALIDITA)
                            item.CodiceAsl = idr.GetStringOrDefault(HSP_CODICE_ASL)
                            item.DescrizioneAsl = idr.GetStringOrDefault(USL_DESCRIZIONE)
                            item.Indirizzo = idr.GetStringOrDefault(HSP_INDIRIZZO)
                            item.CodiceComune = idr.GetStringOrDefault(HSP_COD_COMUNE)
                            item.DescrizioneComune = idr.GetStringOrDefault(COM_DESCRIZIONE)
                            item.OspedaleDiComunita = SNtoBool(idr.GetStringOrDefault(HSP_OSPEDALE_COMUNITA))
                            list.Add(item)

                        End While

                    End If

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

        End Using

        Return list

    End Function

    Public Sub UpdateCodiceHsp(command As CodiceHSPCommand) Implements ICodiciStrutturaProvider.UpdateCodiceHsp

        Using cmd As New OracleClient.OracleCommand("", Connection)

            Dim cmdText As New StringBuilder()
            cmdText.Append("update T_ANA_CODICI_HSP set ")

            If Not String.IsNullOrWhiteSpace(command.Descrizione) Then
                cmdText.Append(" HSP_DESCRIZIONE = :descrizione, ")
                cmd.Parameters.AddWithValue("descrizione", command.Descrizione)
            End If

            If command.DataInizioValidita.HasValue Then
                cmdText.Append(" HSP_DATA_INIZIO_VALIDITA = :dataInizioValidita, ")
                cmd.Parameters.AddWithValue("dataInizioValidita", command.DataInizioValidita)
            End If

            If command.DataFineValidita.HasValue Then
                cmdText.Append(" HSP_DATA_FINE_VALIDITA = :dataFineValidita, ")
                cmd.Parameters.AddWithValue("dataFineValidita", command.DataFineValidita)
            End If

            If Not String.IsNullOrWhiteSpace(command.CodiceAsl) Then
                cmdText.Append(" HSP_CODICE_ASL = :codiceAsl, ")
                cmd.Parameters.AddWithValue("codiceAsl", command.CodiceAsl)
            End If

            If Not String.IsNullOrWhiteSpace(command.Indirizzo) Then
                cmdText.Append(" HSP_INDIRIZZO = :indirizzo, ")
                cmd.Parameters.AddWithValue("indirizzo", command.Indirizzo)
            End If

            If Not String.IsNullOrWhiteSpace(command.CodiceComune) Then
                cmdText.Append(" HSP_CODICE_COMUNE = :codComune, ")
                cmd.Parameters.AddWithValue("codComune", command.CodiceComune)
            End If

            cmdText.Remove(cmdText.Length - 2, 2)
            cmdText.Append(" where HSP_ID = :id ")
            cmd.Parameters.AddWithValue("id", command.Id)

            cmd.CommandText = cmdText.ToString()

            Dim ownConnection As Boolean = False

            Try
                ownConnection = ConditionalOpenConnection(cmd)
                cmd.ExecuteNonQuery()

            Finally
                ConditionalCloseConnection(ownConnection)

            End Try

        End Using

    End Sub

    'Public Sub EliminaMalattiaRaggruppamento(codiceMalattia As String, idUtente As String) Implements ICodiciHspProvider.EliminaCodiceHsp

    '    Using cmd As New OracleClient.OracleCommand("update T_ANA_MALATTIE_RAGGRUPPAMENTO SET MRG_ID_UTENTE_ELIMINAZIONE = :idUtente, MRG_DATA_ELIMINAZIONE = :dataEliminazione 
    '                                                     where MRG_CODICE = :codiceMalattia", Connection)

    '        cmd.Parameters.AddWithValue("codiceMalattia", codiceMalattia)
    '        cmd.Parameters.AddWithValue("idUtente", idUtente)
    '        cmd.Parameters.AddWithValue("dataEliminazione", Date.Today)

    '        Dim ownConnection As Boolean = False

    '        Try
    '            ownConnection = ConditionalOpenConnection(cmd)

    '            cmd.ExecuteNonQuery()

    '        Finally
    '            ConditionalCloseConnection(ownConnection)

    '        End Try

    '    End Using

    'End Sub

    Public Sub AggiungiCodiceHsp(command As CodiceHSPCommand) Implements ICodiciStrutturaProvider.AggiungiCodiceHsp

        Using cmd As New OracleClient.OracleCommand("insert into T_ANA_CODICI_HSP (HSP_CODICE, HSP_DESCRIZIONE, HSP_DATA_INIZIO_VALIDITA,                                                            HSP_DATA_FINE_VALIDITA, HSP_CODICE_ASL, HSP_INDIRIZZO, HSP_CODICE_COMUNE) values                                                                (:codice, :descrizione, :dataInizioValidita, :dataFineValidita, :codiceAsl, :indirizzo, :codComune)", Connection)

            If Not String.IsNullOrWhiteSpace(command.Codice) Then
                cmd.Parameters.AddWithValue("codice", command.Codice)
            Else
                cmd.Parameters.AddWithValue("codice", DBNull.Value)
            End If

            If Not String.IsNullOrWhiteSpace(command.Descrizione) Then
                cmd.Parameters.AddWithValue("descrizione", command.Descrizione)
            Else
                cmd.Parameters.AddWithValue("descrizione", DBNull.Value)
            End If

            If command.DataInizioValidita.HasValue Then
                cmd.Parameters.AddWithValue("dataInizioValidita", command.DataInizioValidita.Value)
            Else
                cmd.Parameters.AddWithValue("dataInizioValidita", DBNull.Value)
            End If

            If command.DataFineValidita.HasValue Then
                cmd.Parameters.AddWithValue("dataFineValidita", command.DataFineValidita.Value)
            Else
                cmd.Parameters.AddWithValue("dataFineValidita", DBNull.Value)
            End If

            If Not String.IsNullOrWhiteSpace(command.CodiceAsl) Then
                cmd.Parameters.AddWithValue("codiceAsl", command.CodiceAsl)
            Else
                cmd.Parameters.AddWithValue("codiceAsl", DBNull.Value)
            End If

            If Not String.IsNullOrWhiteSpace(command.Indirizzo) Then
                cmd.Parameters.AddWithValue("indirizzo", command.Indirizzo)
            Else
                cmd.Parameters.AddWithValue("indirizzo", DBNull.Value)
            End If

            If Not String.IsNullOrWhiteSpace(command.CodiceComune) Then
                cmd.Parameters.AddWithValue("codComune", command.CodiceComune)
            Else
                cmd.Parameters.AddWithValue("codComune", DBNull.Value)
            End If


            Dim ownConnection As Boolean = False

            Try
                ownConnection = ConditionalOpenConnection(cmd)

                cmd.ExecuteNonQuery()

            Finally
                ConditionalCloseConnection(ownConnection)

            End Try

        End Using

    End Sub

    Public Function IsCodiceInCodiciHsp(codice As String) As Boolean Implements ICodiciStrutturaProvider.IsCodiceInCodiciHsp

        ' TODO [Malattie]: fare la query di ricerca codice nella tabella (valore restituito: bool)
        Using cmd As New OracleClient.OracleCommand("select 1
                                                     from T_ANA_CODICI_HSP
                                                     where HSP_CODICE = :codice", Connection)

            Dim ownConnection As Boolean = False

            Try
                ownConnection = ConditionalOpenConnection(cmd)
                cmd.Parameters.AddWithValue("codice", codice)

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

#End Region

#Region " Metodi STS "

    Public Function GetCodiciSts(pagingOptions As Data.PagingOptions) As List(Of CodiceSTS) Implements ICodiciStrutturaProvider.GetCodiciSTS

        Dim list As New List(Of CodiceSTS)()

        Using cmd As New OracleClient.OracleCommand("select STS_ID, STS_CODICE, STS_DESCRIZIONE, STS_DATA_INIZIO_VALIDITA, STS_DATA_FINE_VALIDITA,                                                    STS_CODICE_ASL, STS_INDIRIZZO, STS_CODICE_COMUNE, COM_DESCRIZIONE from T_ANA_CODICI_STS left                                                     join T_ANA_COMUNI on STS_CODICE_COMUNE = COM_CODICE", Connection)

            If Not pagingOptions Is Nothing Then
                cmd.AddPaginatedQuery(pagingOptions)
            End If

            Dim ownConnection As Boolean = False

            Try
                ownConnection = ConditionalOpenConnection(cmd)

                Using idr As IDataReader = cmd.ExecuteReader()

                    If idr IsNot Nothing Then

                        Dim STS_ID As Integer = idr.GetOrdinal("STS_ID")
                        Dim STS_CODICE As Integer = idr.GetOrdinal("STS_CODICE")
                        Dim STS_DESCRIZIONE As Integer = idr.GetOrdinal("STS_DESCRIZIONE")
                        Dim STS_DATA_INIZIO_VALIDITA As Integer = idr.GetOrdinal("STS_DATA_INIZIO_VALIDITA")
                        Dim STS_DATA_FINE_VALIDITA As Integer = idr.GetOrdinal("STS_DATA_FINE_VALIDITA")
                        Dim STS_CODICE_ASL As Integer = idr.GetOrdinal("STS_CODICE_ASL")
                        Dim STS_INDIRIZZO As Integer = idr.GetOrdinal("STS_INDIRIZZO")
                        Dim STS_COD_COMUNE As Integer = idr.GetOrdinal("STS_CODICE_COMUNE")
                        Dim COM_DESCRIZIONE As Integer = idr.GetOrdinal("COM_DESCRIZIONE")

                        While idr.Read()

                            Dim item As New CodiceSTS()

                            item.Id = idr.GetStringOrDefault(STS_ID)
                            item.CodiceSts = idr.GetString(STS_CODICE)
                            item.Descrizione = idr.GetStringOrDefault(STS_DESCRIZIONE)
                            item.DataInizioValidita = idr.GetNullableDateTimeOrDefault(STS_DATA_INIZIO_VALIDITA)
                            item.DataFineValidita = idr.GetNullableDateTimeOrDefault(STS_DATA_FINE_VALIDITA)
                            item.CodiceAsl = idr.GetStringOrDefault(STS_CODICE_ASL)
                            item.Indirizzo = idr.GetStringOrDefault(STS_INDIRIZZO)
                            item.CodiceComune = idr.GetStringOrDefault(STS_COD_COMUNE)
                            item.DescrizioneComune = idr.GetStringOrDefault(COM_DESCRIZIONE)

                            list.Add(item)

                        End While
                    End If

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

        End Using

        Return list

    End Function

    Public Function GetCountCodiciSts() As Integer Implements ICodiciStrutturaProvider.GetCountCodiciSTS

        Dim ris As Integer = 0

        Using cmd As New OracleClient.OracleCommand("select count(*) from T_ANA_CODICI_STS left join T_ANA_COMUNI on STS_CODICE_COMUNE = COM_CODICE", Connection)

            Dim ownConnection As Boolean = False

            Try
                ownConnection = ConditionalOpenConnection(cmd)
                Dim obj As Object = cmd.ExecuteScalar()

                If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                    ris = Convert.ToInt32(obj)
                End If

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

        End Using

        Return ris

    End Function

    Public Function GetCodiciStsByFiltro(filtro As String, pagingOptions As Data.PagingOptions) As List(Of CodiceSTS) Implements ICodiciStrutturaProvider.GetCodiciSTSByFiltro

        Dim list As New List(Of CodiceSTS)()

        Using cmd As New OracleClient.OracleCommand("select STS_ID, STS_CODICE, STS_DESCRIZIONE, STS_DATA_INIZIO_VALIDITA, STS_DATA_FINE_VALIDITA,                                                    STS_CODICE_ASL, STS_INDIRIZZO, STS_CODICE_COMUNE, COM_DESCRIZIONE from T_ANA_CODICI_STS left join T_ANA_COMUNI on STS_CODICE_COMUNE = COM_CODICE left join T_ANA_USL on STS_CODICE_ASL = USL_CODICE where STS_CODICE LIKE :filtro or STS_DESCRIZIONE LIKE :filtro", Connection)

            cmd.Parameters.AddWithValue("filtro", "%" + filtro + "%")

            If Not pagingOptions Is Nothing Then
                cmd.AddPaginatedQuery(pagingOptions)
            End If

            Dim ownConnection As Boolean = False

            Try
                ownConnection = ConditionalOpenConnection(cmd)

                Using idr As IDataReader = cmd.ExecuteReader()
                    If idr IsNot Nothing Then

                        Dim STS_ID As Integer = idr.GetOrdinal("STS_ID")
                        Dim STS_CODICE As Integer = idr.GetOrdinal("STS_CODICE")
                        Dim STS_DESCRIZIONE As Integer = idr.GetOrdinal("STS_DESCRIZIONE")
                        Dim STS_DATA_INIZIO_VALIDITA As Integer = idr.GetOrdinal("STS_DATA_INIZIO_VALIDITA")
                        Dim STS_DATA_FINE_VALIDITA As Integer = idr.GetOrdinal("STS_DATA_FINE_VALIDITA")
                        Dim STS_CODICE_ASL As Integer = idr.GetOrdinal("STS_CODICE_ASL")
                        Dim STS_INDIRIZZO As Integer = idr.GetOrdinal("STS_INDIRIZZO")
                        Dim STS_COD_COMUNE As Integer = idr.GetOrdinal("STS_CODICE_COMUNE")
                        Dim COM_DESCRIZIONE As Integer = idr.GetOrdinal("COM_DESCRIZIONE")

                        While idr.Read()
                            Dim item As New CodiceSTS()

                            item.Id = idr.GetStringOrDefault(STS_ID)
                            item.CodiceSts = idr.GetString(STS_CODICE)
                            item.Descrizione = idr.GetStringOrDefault(STS_DESCRIZIONE)
                            item.DataInizioValidita = idr.GetNullableDateTimeOrDefault(STS_DATA_INIZIO_VALIDITA)
                            item.DataFineValidita = idr.GetNullableDateTimeOrDefault(STS_DATA_FINE_VALIDITA)
                            item.CodiceAsl = idr.GetStringOrDefault(STS_CODICE_ASL)
                            item.Indirizzo = idr.GetStringOrDefault(STS_INDIRIZZO)
                            item.CodiceComune = idr.GetStringOrDefault(STS_COD_COMUNE)
                            item.DescrizioneComune = idr.GetStringOrDefault(COM_DESCRIZIONE)

                            list.Add(item)

                        End While

                    End If

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

        End Using

        Return list

    End Function

    Public Function GetCountCodiciStsByFiltro(filtro As String) As Integer Implements ICodiciStrutturaProvider.GetCountCodiciSTSByFiltro

        Dim ris As Integer = 0

        Using cmd As New OracleClient.OracleCommand("select count(*) from T_ANA_CODICI_STS where STS_CODICE LIKE :filtro or STS_DESCRIZIONE LIKE :filtro", Connection)

            cmd.Parameters.AddWithValue("filtro", "%" + filtro + "%")

            Dim ownConnection As Boolean = False

            Try
                ownConnection = ConditionalOpenConnection(cmd)
                Dim obj As Object = cmd.ExecuteScalar()

                If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                    ris = Convert.ToInt32(obj)
                End If

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

        End Using

        Return ris

    End Function

    Public Function GetDettaglioCodiceStsById(id As String) As List(Of CodiceSTS) Implements ICodiciStrutturaProvider.GetDettaglioCodiceSTSById

        Dim list As New List(Of CodiceSTS)()

        Using cmd As New OracleClient.OracleCommand("select * from T_ANA_CODICI_STS left join T_ANA_COMUNI on STS_CODICE_COMUNE = COM_CODICE left join T_ANA_USL on STS_CODICE_ASL = USL_CODICE where STS_ID = :id", Connection)

            cmd.Parameters.AddWithValue("id", id)
            Dim ownConnection As Boolean = False

            Try
                ownConnection = ConditionalOpenConnection(cmd)

                Using idr As IDataReader = cmd.ExecuteReader()
                    If idr IsNot Nothing Then

                        Dim STS_ID As Integer = idr.GetOrdinal("STS_ID")
                        Dim STS_CODICE As Integer = idr.GetOrdinal("STS_CODICE")
                        Dim STS_DESCRIZIONE As Integer = idr.GetOrdinal("STS_DESCRIZIONE")
                        Dim STS_DATA_INIZIO_VALIDITA As Integer = idr.GetOrdinal("STS_DATA_INIZIO_VALIDITA")
                        Dim STS_DATA_FINE_VALIDITA As Integer = idr.GetOrdinal("STS_DATA_FINE_VALIDITA")
                        Dim STS_CODICE_ASL As Integer = idr.GetOrdinal("STS_CODICE_ASL")
                        Dim USL_DESCRIZIONE As Integer = idr.GetOrdinal("USL_DESCRIZIONE")
                        Dim STS_INDIRIZZO As Integer = idr.GetOrdinal("STS_INDIRIZZO")
                        Dim STS_COD_COMUNE As Integer = idr.GetOrdinal("STS_CODICE_COMUNE")
                        Dim COM_DESCRIZIONE As Integer = idr.GetOrdinal("COM_DESCRIZIONE")

                        While idr.Read()

                            Dim item As New CodiceSTS()

                            item.Id = idr.GetStringOrDefault(STS_ID)
                            item.CodiceSts = idr.GetString(STS_CODICE)
                            item.Descrizione = idr.GetStringOrDefault(STS_DESCRIZIONE)
                            item.DataInizioValidita = idr.GetNullableDateTimeOrDefault(STS_DATA_INIZIO_VALIDITA)
                            item.DataFineValidita = idr.GetNullableDateTimeOrDefault(STS_DATA_FINE_VALIDITA)
                            item.CodiceAsl = idr.GetStringOrDefault(STS_CODICE_ASL)
                            item.DescrizioneAsl = idr.GetStringOrDefault(USL_DESCRIZIONE)
                            item.Indirizzo = idr.GetStringOrDefault(STS_INDIRIZZO)
                            item.CodiceComune = idr.GetStringOrDefault(STS_COD_COMUNE)
                            item.DescrizioneComune = idr.GetStringOrDefault(COM_DESCRIZIONE)

                            list.Add(item)

                        End While

                    End If

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

        End Using

        Return list

    End Function

    Public Sub UpdateCodiceSts(command As CodiceSTSCommand) Implements ICodiciStrutturaProvider.UpdateCodiceSTS

        Using cmd As New OracleClient.OracleCommand("", Connection)

            Dim cmdText As New StringBuilder()
            cmdText.Append("update T_ANA_CODICI_STS set ")

            If Not String.IsNullOrWhiteSpace(command.Descrizione) Then
                cmdText.Append(" STS_DESCRIZIONE = :descrizione, ")
                cmd.Parameters.AddWithValue("descrizione", command.Descrizione)
            End If

            If command.DataInizioValidita.HasValue Then
                cmdText.Append(" STS_DATA_INIZIO_VALIDITA = :dataInizioValidita, ")
                cmd.Parameters.AddWithValue("dataInizioValidita", command.DataInizioValidita)
            End If

            If command.DataFineValidita.HasValue Then
                cmdText.Append(" STS_DATA_FINE_VALIDITA = :dataFineValidita, ")
                cmd.Parameters.AddWithValue("dataFineValidita", command.DataFineValidita)
            End If

            If Not String.IsNullOrWhiteSpace(command.CodiceAsl) Then
                cmdText.Append(" STS_CODICE_ASL = :codiceAsl, ")
                cmd.Parameters.AddWithValue("codiceAsl", command.CodiceAsl)
            End If

            If Not String.IsNullOrWhiteSpace(command.Indirizzo) Then
                cmdText.Append(" STS_INDIRIZZO = :indirizzo, ")
                cmd.Parameters.AddWithValue("indirizzo", command.Indirizzo)
            End If

            If Not String.IsNullOrWhiteSpace(command.CodiceComune) Then
                cmdText.Append(" STS_CODICE_COMUNE = :codComune, ")
                cmd.Parameters.AddWithValue("codComune", command.CodiceComune)
            End If

            cmdText.Remove(cmdText.Length - 2, 2)
            cmdText.Append(" where STS_ID = :id ")
            cmd.Parameters.AddWithValue("id", command.Id)

            cmd.CommandText = cmdText.ToString()

            Dim ownConnection As Boolean = False

            Try
                ownConnection = ConditionalOpenConnection(cmd)
                cmd.ExecuteNonQuery()

            Finally
                ConditionalCloseConnection(ownConnection)

            End Try

        End Using

    End Sub

    'Public Sub EliminaMalattiaRaggruppamento(codiceMalattia As String, idUtente As String) Implements ICodiciSTSProvider.EliminaCodiceSTS

    '    Using cmd As New OracleClient.OracleCommand("update T_ANA_MALATTIE_RAGGRUPPAMENTO SET MRG_ID_UTENTE_ELIMINAZIONE = :idUtente, MRG_DATA_ELIMINAZIONE = :dataEliminazione 
    '                                                     where MRG_CODICE = :codiceMalattia", Connection)

    '        cmd.Parameters.AddWithValue("codiceMalattia", codiceMalattia)
    '        cmd.Parameters.AddWithValue("idUtente", idUtente)
    '        cmd.Parameters.AddWithValue("dataEliminazione", Date.Today)

    '        Dim ownConnection As Boolean = False

    '        Try
    '            ownConnection = ConditionalOpenConnection(cmd)

    '            cmd.ExecuteNonQuery()

    '        Finally
    '            ConditionalCloseConnection(ownConnection)

    '        End Try

    '    End Using

    'End Sub

    Public Sub AggiungiCodiceSTS(command As CodiceSTSCommand) Implements ICodiciStrutturaProvider.AggiungiCodiceSTS

        Using cmd As New OracleClient.OracleCommand("insert into T_ANA_CODICI_STS (STS_CODICE, STS_DESCRIZIONE, STS_DATA_INIZIO_VALIDITA,                                                            STS_DATA_FINE_VALIDITA, STS_CODICE_ASL, STS_INDIRIZZO, STS_CODICE_COMUNE) values                                                                (:codice, :descrizione, :dataInizioValidita, :dataFineValidita, :codiceAsl, :indirizzo, :codComune)", Connection)

            If Not String.IsNullOrWhiteSpace(command.Codice) Then
                cmd.Parameters.AddWithValue("codice", command.Codice)
            Else
                cmd.Parameters.AddWithValue("codice", DBNull.Value)
            End If

            If Not String.IsNullOrWhiteSpace(command.Descrizione) Then
                cmd.Parameters.AddWithValue("descrizione", command.Descrizione)
            Else
                cmd.Parameters.AddWithValue("descrizione", DBNull.Value)
            End If

            If command.DataInizioValidita.HasValue Then
                cmd.Parameters.AddWithValue("dataInizioValidita", command.DataInizioValidita.Value)
            Else
                cmd.Parameters.AddWithValue("dataInizioValidita", DBNull.Value)
            End If

            If command.DataFineValidita.HasValue Then
                cmd.Parameters.AddWithValue("dataFineValidita", command.DataFineValidita.Value)
            Else
                cmd.Parameters.AddWithValue("dataFineValidita", DBNull.Value)
            End If

            If Not String.IsNullOrWhiteSpace(command.CodiceAsl) Then
                cmd.Parameters.AddWithValue("codiceAsl", command.CodiceAsl)
            Else
                cmd.Parameters.AddWithValue("codiceAsl", DBNull.Value)
            End If

            If Not String.IsNullOrWhiteSpace(command.Indirizzo) Then
                cmd.Parameters.AddWithValue("indirizzo", command.Indirizzo)
            Else
                cmd.Parameters.AddWithValue("indirizzo", DBNull.Value)
            End If

            If Not String.IsNullOrWhiteSpace(command.CodiceComune) Then
                cmd.Parameters.AddWithValue("codComune", command.CodiceComune)
            Else
                cmd.Parameters.AddWithValue("codComune", DBNull.Value)
            End If


            Dim ownConnection As Boolean = False

            Try
                ownConnection = ConditionalOpenConnection(cmd)

                cmd.ExecuteNonQuery()

            Finally
                ConditionalCloseConnection(ownConnection)

            End Try

        End Using

    End Sub

    Public Function IsCodiceInCodiciSTS(codice As String) As Boolean Implements ICodiciStrutturaProvider.IsCodiceInCodiciSTS

        ' TODO [Malattie]: fare la query di ricerca codice nella tabella (valore restituito: bool)
        Using cmd As New OracleClient.OracleCommand("select 1
                                                     from T_ANA_CODICI_STS
                                                     where STS_CODICE = :codice", Connection)

            Dim ownConnection As Boolean = False

            Try
                ownConnection = ConditionalOpenConnection(cmd)
                cmd.Parameters.AddWithValue("codice", codice)

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

#End Region

End Class