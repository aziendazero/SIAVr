Imports System.Collections.Generic
Imports System.Data.OracleClient
Imports Onit.Database.DataAccessManager

Namespace DAL

    Public Class DbMalattieProvider
        Inherits DbProvider
        Implements IMalattieProvider

#Region " Constructors "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " Public "

        Public Function LoadMalattiePaziente(pazCodice As Integer) As dsMalattie.MalattieDataTable Implements IMalattieProvider.LoadMalattiePaziente

            Dim dtaMalattie As New dsMalattie.MalattieDataTable()

            _DAM.QB.NewQuery()
            _DAM.QB.AddSelectFields("PMA_MAL_CODICE, PMA_N_MALATTIA, MAL_DESCRIZIONE, PMA_FOLLOW_UP, PMA_NUOVA_DIAGNOSI, PMA_DATA_DIAGNOSI, PMA_DATA_ULTIMA_VISITA, PMA_N_BILANCIO_PARTENZA")
            _DAM.QB.AddTables("T_PAZ_MALATTIE, T_ANA_MALATTIE")
            _DAM.QB.AddWhereCondition("MAL_CODICE", Comparatori.Uguale, "PMA_MAL_CODICE", DataTypes.Join)
            _DAM.QB.AddWhereCondition("PMA_PAZ_CODICE", Comparatori.Uguale, pazCodice, DataTypes.Numero)
            _DAM.QB.AddOrderByFields("PMA_N_MALATTIA")

            _DAM.BuildDataTable(dtaMalattie)

            Return dtaMalattie

        End Function

        Public Sub SalvaMalattiePaziente(codicePaziente As Integer, dtaMalattie As dsMalattie.MalattieDataTable) Implements IMalattieProvider.SalvaMalattiePaziente

            _DAM.QB.NewQuery()
            _DAM.QB.AddTables("T_PAZ_MALATTIE")
            _DAM.QB.AddWhereCondition("PMA_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
            _DAM.ExecNonQuery(ExecQueryType.Delete)

            Dim dataFittizia As New Date(1900, 1, 1)

            For i As Integer = 0 To dtaMalattie.Rows.Count - 1

                _DAM.QB.NewQuery()
                _DAM.QB.AddTables("T_PAZ_MALATTIE")
                _DAM.QB.AddInsertField("PMA_PAZ_CODICE", codicePaziente, DataTypes.Numero)
                _DAM.QB.AddInsertField("PMA_MAL_CODICE", " ", DataTypes.Stringa)
                _DAM.QB.AddInsertField("PMA_N_MALATTIA", "0", DataTypes.Numero)
                _DAM.QB.AddInsertField("PMA_FOLLOW_UP", "S", DataTypes.Stringa)
                _DAM.QB.AddInsertField("PMA_N_BILANCIO_PARTENZA", "0", DataTypes.Numero)
                _DAM.QB.AddInsertField("PMA_NUOVA_DIAGNOSI", "S", DataTypes.Stringa)
                _DAM.QB.AddInsertField("PMA_DATA_DIAGNOSI", dataFittizia, DataTypes.Data)
                _DAM.QB.AddInsertField("PMA_DATA_ULTIMA_VISITA", dataFittizia, DataTypes.Data)

                _DAM.QB.ChangeValue(1, ExecQueryType.Insert, dtaMalattie.Rows(i).Item("PMA_MAL_CODICE"))
                _DAM.QB.ChangeValue(2, ExecQueryType.Insert, Val(dtaMalattie.Rows(i).Item("PMA_N_MALATTIA")))
                _DAM.QB.ChangeValue(3, ExecQueryType.Insert, dtaMalattie.Rows(i).Item("PMA_FOLLOW_UP").ToString)
                _DAM.QB.ChangeValue(4, ExecQueryType.Insert, Val(IIf(dtaMalattie.Rows(i).Item("PMA_N_BILANCIO_PARTENZA") Is DBNull.Value, "0", dtaMalattie.Rows(i).Item("PMA_N_BILANCIO_PARTENZA"))))
                _DAM.QB.ChangeValue(5, ExecQueryType.Insert, dtaMalattie.Rows(i).Item("PMA_NUOVA_DIAGNOSI").ToString)
                _DAM.QB.ChangeValue(6, ExecQueryType.Insert, IIf(dtaMalattie.Rows(i).Item("PMA_DATA_DIAGNOSI") Is DBNull.Value, "", dtaMalattie.Rows(i).Item("PMA_DATA_DIAGNOSI")))
                _DAM.QB.ChangeValue(7, ExecQueryType.Insert, IIf(dtaMalattie.Rows(i).Item("PMA_DATA_ULTIMA_VISITA") Is DBNull.Value, "", dtaMalattie.Rows(i).Item("PMA_DATA_ULTIMA_VISITA")))
                _DAM.ExecNonQuery(ExecQueryType.Insert)

            Next

        End Sub

        Public Function LoadMalattieEsenzione() As List(Of Malattia) Implements IMalattieProvider.LoadMalattieEsenzione

            Dim listMalattie As List(Of Malattia) = Nothing

            With _DAM.QB
                .NewQuery()
                .AddTables("T_ANA_MALATTIE")
                .AddSelectFields("MAL_CODICE, MAL_DESCRIZIONE, MAL_FLAG_VISITA, MAL_CODICE_ESENZIONE")
                .AddWhereCondition("MAL_CODICE_ESENZIONE", Comparatori.IsNot, DBNull.Value, DataTypes.Stringa)
            End With

            Using idr As IDataReader = _DAM.BuildDataReader()

                listMalattie = GetListMalattie(idr)

            End Using

            Return listMalattie

        End Function

        Public Function LoadDescrizioneMalattia(codiceMalattia As String) As String Implements IMalattieProvider.LoadDescrizioneMalattia

            _DAM.QB.NewQuery()
            _DAM.QB.AddSelectFields("MAL_DESCRIZIONE")
            _DAM.QB.AddTables("T_ANA_MALATTIE")
            _DAM.QB.AddWhereCondition("MAL_CODICE", Comparatori.Uguale, codiceMalattia, DataTypes.Stringa)

            Dim obj As Object = _DAM.ExecScalar()

            If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                Return obj.ToString()
            End If

            Return String.Empty

        End Function

        Public Function LoadMalattia(codiceMalattia As String) As Entities.Malattia Implements IMalattieProvider.LoadMalattia

            Dim malattia As Entities.Malattia = Nothing

            With _DAM.QB
                .NewQuery()
                .AddTables("T_ANA_MALATTIE")
                .AddSelectFields("MAL_CODICE, MAL_DESCRIZIONE, MAL_FLAG_VISITA, MAL_CODICE_ESENZIONE")
                .AddWhereCondition("MAL_CODICE", Comparatori.Uguale, codiceMalattia, DataTypes.Stringa)
            End With

            Using idr As IDataReader = _DAM.BuildDataReader()
                malattia = Me.GetListMalattie(idr).FirstOrDefault()
            End Using

            If Not malattia Is Nothing Then

                With _DAM.QB
                    .NewQuery()
                    .AddTables("T_ANA_LINK_MALATTIE_TIPOLOGIA, T_ANA_MALATTIE_TIPOLOGIA")
                    .AddSelectFields("MTI_CODICE, MTI_DESCRIZIONE")
                    .AddWhereCondition("MML_MTI_CODICE", Comparatori.Uguale, "MTI_CODICE", DataTypes.Join)
                    .AddWhereCondition("MML_MAL_CODICE", Comparatori.Uguale, codiceMalattia, DataTypes.Stringa)
                End With

                Using idr As IDataReader = _DAM.BuildDataReader()

                    Dim MTI_CODICE As Integer = idr.GetOrdinal("MTI_CODICE")
                    Dim MTI_DESCRIZIONE As Integer = idr.GetOrdinal("MTI_DESCRIZIONE")

                    While idr.Read()

                        Dim tipo As New Malattia.TipologiaMalattia()
                        tipo.Codice = idr.GetStringOrDefault(MTI_CODICE)
                        tipo.Descrizione = idr.GetStringOrDefault(MTI_DESCRIZIONE)

                        malattia.Tipologia.Add(tipo)

                    End While

                End Using

            End If

            Return malattia

        End Function

        Public Function GetTipologiaMalattie() As List(Of Entities.Malattia.TipologiaMalattia) Implements IMalattieProvider.GetTipologiaMalattie

            Dim listTipologieMalattia As New List(Of Entities.Malattia.TipologiaMalattia)

            With _DAM.QB
                .NewQuery()
                .AddTables("T_ANA_MALATTIE_TIPOLOGIA")
                .AddSelectFields("MTI_CODICE, MTI_DESCRIZIONE, MTI_DEFAULT")
            End With

            Using idr As IDataReader = _DAM.BuildDataReader()

                If Not idr Is Nothing Then

                    Dim MTI_CODICE As Integer = idr.GetOrdinal("MTI_CODICE")
                    Dim MTI_DESCRIZIONE As Integer = idr.GetOrdinal("MTI_DESCRIZIONE")
                    Dim MTI_DEFAULT As Integer = idr.GetOrdinal("MTI_DEFAULT")

                    While idr.Read()

                        Dim tipo As New Entities.Malattia.TipologiaMalattia()
                        tipo.Codice = idr.GetStringOrDefault(MTI_CODICE)
                        tipo.Descrizione = idr.GetStringOrDefault(MTI_DESCRIZIONE)
                        tipo.FlagDefault = idr.GetBooleanOrDefault(MTI_DEFAULT)

                        listTipologieMalattia.Add(tipo)

                    End While

                End If

            End Using

            Return listTipologieMalattia

        End Function

        Public Function DeleteTipologiaMalattia(codiceMalattia As String) As Integer Implements IMalattieProvider.DeleteTipologiaMalattia

            Dim cmd As OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try
                cmd = Me.Connection.CreateCommand()

                ownConnection = Me.ConditionalOpenConnection(cmd)

                cmd.CommandText = " delete from t_ana_link_malattie_tipologia where mml_mal_codice = :mal_codice "
                cmd.Parameters.AddWithValue("mal_codice", GetStringParam(codiceMalattia, False))

                Return cmd.ExecuteNonQuery()

            Finally
                Me.ConditionalCloseConnection(ownConnection)
                If Not cmd Is Nothing Then cmd.Dispose()
            End Try

        End Function

        Public Function InsertTipologiaMalattia(codiceMalattia As String, listaTipologie As List(Of String)) As Integer Implements IMalattieProvider.InsertTipologiaMalattia

            Dim cmd As OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try
                cmd = Me.Connection.CreateCommand()

                ownConnection = Me.ConditionalOpenConnection(cmd)

                For i As Integer = 0 To listaTipologie.Count - 1

                    cmd.CommandText = " insert into t_ana_link_malattie_tipologia (mml_mal_codice, mml_mti_codice) values (:mal_codice, :mti_codice) "
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("mal_codice", GetStringParam(codiceMalattia, False))
                    cmd.Parameters.AddWithValue("mti_codice", GetStringParam(listaTipologie(i), False))
                    cmd.ExecuteNonQuery()

                Next

            Finally
                Me.ConditionalCloseConnection(ownConnection)
                If Not cmd Is Nothing Then cmd.Dispose()
            End Try

            Return listaTipologie.Count

        End Function

        Public Function GetFlagVisita(codiceMalattia As String) As String Implements IMalattieProvider.GetFlagVisita

            Dim flagVisita As String = String.Empty

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("MAL_FLAG_VISITA")
                .AddTables("T_ANA_MALATTIE")
                .AddWhereCondition("MAL_CODICE", Comparatori.Uguale, codiceMalattia, DataTypes.Stringa)
            End With

            Dim obj As Object = _DAM.ExecScalar()

            If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then flagVisita = obj.ToString()

            Return flagVisita

        End Function

        ''' <summary>
        ''' Restituisce un datatable contenente tutte le malattie, esclusa la 0, ordinate per descrizione.
        ''' </summary>
        ''' <param name="codiceEsclusioneNessunaMalattia"></param>
        ''' <returns></returns>
        Public Function GetMalattie(codiceEsclusioneNessunaMalattia As String) As DataTable Implements IMalattieProvider.GetMalattie

            RefurbishDT()

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("MAL_CODICE, MAL_DESCRIZIONE")
                .AddTables("T_ANA_MALATTIE")

                If Not String.IsNullOrEmpty(codiceEsclusioneNessunaMalattia) Then
                    .AddWhereCondition("MAL_CODICE", Comparatori.Diverso, codiceEsclusioneNessunaMalattia, DataTypes.Stringa)
                End If

                .AddOrderByFields("MAL_DESCRIZIONE")
            End With

            _DAM.BuildDataTable(_DT)

            Return _DT.Copy()

        End Function

        Public Function GetMalattieByCodiciACN(listaCodiciACN As List(Of String)) As List(Of Malattia) Implements IMalattieProvider.GetMalattieByCodiciACN

            Dim listMalattie As List(Of Malattia) = Nothing

            Dim ownConnection As Boolean = False

            If Not listaCodiciACN Is Nothing AndAlso listaCodiciACN.Count > 0 Then

                Try

                    Using cmd As New OracleClient.OracleCommand()

                        cmd.Connection = Me.Connection
                        Dim query As New System.Text.StringBuilder()
                        query.Append(" select MAL_CODICE, MAL_DESCRIZIONE, MAL_FLAG_VISITA, MAL_CODICE_ESENZIONE from T_ANA_MALATTIE ")

                        If listaCodiciACN.Count = 1 Then
                            query.Append(" WHERE MAL_CODICE_ACN = :MAL_CODICE_ACN ")
                            cmd.Parameters.AddWithValue("MAL_CODICE_ACN", listaCodiciACN.First())
                        Else
                            Dim filtroCodiciAcn As GetInFilterResult = GetInFilter(listaCodiciACN)
                            query.AppendFormat(" WHERE MAL_CODICE_ACN in ({0}) ", filtroCodiciAcn.InFilter)
                            cmd.Parameters.AddRange(filtroCodiciAcn.Parameters)
                        End If


                        cmd.CommandText = query.ToString()

                        ownConnection = Me.ConditionalOpenConnection(cmd)

                        Using idr As IDataReader = cmd.ExecuteReader()
                            If Not idr Is Nothing Then

                                listMalattie = GetListMalattie(idr)

                            End If
                        End Using

                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End If

            Return listMalattie

        End Function


        ''' <summary>
        ''' Restituisce una lista di codici e descrizioni delle malattie in anagrafica, in base ai filtri.
        ''' </summary>
        ''' <param name="filtraObsoleti"></param>
        ''' <param name="codiciTipologieMalattia"></param>
        ''' <returns></returns>
        Public Function GetCodiceDescrizioneMalattie(filtraObsoleti As Boolean, codiciTipologieMalattia As List(Of String)) As List(Of KeyValuePair(Of String, String)) Implements IMalattieProvider.GetCodiceDescrizioneMalattie

            Dim list As New List(Of KeyValuePair(Of String, String))()

            Using cmd As OracleCommand = Me.Connection.CreateCommand()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim join As String = String.Empty
                    Dim filter As New System.Text.StringBuilder()

                    If filtraObsoleti Then
                        filter.Append(" MAL_OBSOLETO = 'N' and ")
                    End If

                    If Not codiciTipologieMalattia.IsNullOrEmpty() Then

                        join = " join T_ANA_LINK_MALATTIE_TIPOLOGIA on MAL_CODICE = MML_MAL_CODICE "

                        If codiciTipologieMalattia.Count = 1 Then

                            filter.Append(" MML_MTI_CODICE = :mml_mti_codice and ")
                            cmd.Parameters.AddWithValue("MML_MTI_CODICE", codiciTipologieMalattia.First())

                        Else

                            Dim filtroInResult As GetInFilterResult = GetInFilter(codiciTipologieMalattia)

                            filter.AppendFormat(" MML_MTI_CODICE in ({0}) and ", filtroInResult.InFilter)
                            cmd.Parameters.AddRange(filtroInResult.Parameters)

                        End If

                    End If

                    If filter.Length > 0 Then
                        filter.RemoveLast(4)
                        filter.Insert(0, " where ")
                    End If

                    cmd.CommandText = String.Format("select MAL_CODICE, MAL_DESCRIZIONE from T_ANA_MALATTIE {0} {1} order by MAL_DESCRIZIONE ", join, filter.ToString())

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim mal_codice As Integer = idr.GetOrdinal("MAL_CODICE")
                            Dim mal_descrizione As Integer = idr.GetOrdinal("MAL_DESCRIZIONE")

                            While idr.Read()
                                list.Add(New KeyValuePair(Of String, String)(idr.GetString(mal_codice), idr.GetStringOrDefault(mal_descrizione)))
                            End While

                        End If

                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list

        End Function

        ''' <summary>
        ''' Restituisce le tipologie associate alla malattia specificata
        ''' </summary>
        ''' <param name="codiceMalattia"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCodiceTipologieByMalattia(codiceMalattia As String) As List(Of String) Implements IMalattieProvider.GetCodiceTipologieByMalattia

            Dim list As New List(Of String)()

            Using cmd As New OracleCommand("select MML_MTI_CODICE from T_ANA_LINK_MALATTIE_TIPOLOGIA where MML_MAL_CODICE = :MML_MAL_CODICE", Connection)

                cmd.Parameters.AddWithValue("MML_MAL_CODICE", codiceMalattia)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim mml_mti_codice As Integer = idr.GetOrdinal("MML_MTI_CODICE")

                            While idr.Read()
                                list.Add(idr.GetString(mml_mti_codice))
                            End While

                        End If
                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list

        End Function

        ''' <summary>
        ''' Restituisce le esenzioni malattia del paziente
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEsenzioniMalattiaPaziente(codicePaziente As Integer) As List(Of EsenzioneMalattia) Implements IMalattieProvider.GetEsenzioniMalattiaPaziente

            Dim list As List(Of EsenzioneMalattia) = Nothing

            Using cmd As OracleCommand = Connection.CreateCommand()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.CommandText = GetQuerySelectEsenzioniMalattiaPaziente()

                    cmd.Parameters.AddWithValue("PMA_PAZ_CODICE", codicePaziente)

                    list = GetListEsenzioniMalattia(cmd)

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list

        End Function

        ''' <summary>
        ''' Restituisce l'esenzione malattia del paziente in base al codice interno specificato.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="codiceMalattia"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEsenzioneMalattiaPazienteByCodiceMalattia(codicePaziente As Integer, codiceMalattia As String) As EsenzioneMalattia Implements IMalattieProvider.GetEsenzioneMalattiaPazienteByCodiceMalattia

            Dim item As EsenzioneMalattia = Nothing

            Using cmd As OracleCommand = Connection.CreateCommand()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.CommandText = GetQuerySelectEsenzioniMalattiaPaziente() + " and PMA_MAL_CODICE = :PMA_MAL_CODICE "

                    cmd.Parameters.AddWithValue("PMA_PAZ_CODICE", codicePaziente)
                    cmd.Parameters.AddWithValue("PMA_MAL_CODICE", codiceMalattia)

                    Dim list As List(Of EsenzioneMalattia) = GetListEsenzioniMalattia(cmd)
                    If Not list.IsNullOrEmpty() Then
                        item = list.First()
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return item

        End Function

        ''' <summary>
        ''' Dato un codice di esenzione restituisce un codice malattia
        ''' </summary>
        ''' <param name="codiceEsenzione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCodiceMalattiaByCodiceEsenzione(codiceEsenzione As String) As String Implements IMalattieProvider.GetCodiceMalattiaByCodiceEsenzione

            Dim codice As String = String.Empty

            Using cmd As OracleCommand = Connection.CreateCommand()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.CommandText = "select MAL_CODICE from T_ANA_MALATTIE where MAL_CODICE_ESENZIONE = :MAL_CODICE_ESENZIONE"
                    cmd.Parameters.AddWithValue("MAL_CODICE_ESENZIONE", codiceEsenzione)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim mal_codice As Integer = idr.GetOrdinal("MAL_CODICE")

                            If idr.Read() Then
                                codice = idr.GetStringOrDefault(mal_codice)
                            End If

                        End If
                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return codice

        End Function

        ''' <summary>
        '''  Restituisce il massimo numero malattia per il paziente
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function MaxNumeroMalattiaPaziente(codicePaziente As Integer) As Integer Implements IMalattieProvider.MaxNumeroMalattiaPaziente

            Dim max As Integer = 0

            Using cmd As OracleCommand = Connection.CreateCommand()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.CommandText = "select max(PMA_N_MALATTIA) from T_PAZ_MALATTIE where PMA_PAZ_CODICE = :PMA_PAZ_CODICE"

                    cmd.Parameters.AddWithValue("PMA_PAZ_CODICE", codicePaziente)

                    Dim obj As Object = cmd.ExecuteScalar()
                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        max = Convert.ToInt32(obj)
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return max

        End Function

        ''' <summary>
        ''' Inserimento malattia per il paziente
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="esenzioneMalattia"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsertEsenzioneMalattiaPaziente(codicePaziente As Integer, esenzioneMalattia As EsenzioneMalattia) As Integer Implements IMalattieProvider.InsertEsenzioneMalattiaPaziente

            Dim count As Integer = 0

            Using cmd As New OracleCommand("insert into t_paz_malattie (PMA_PAZ_CODICE, PMA_MAL_CODICE, PMA_N_MALATTIA, PMA_DATA_DIAGNOSI, PMA_DATA_ULTIMA_VISITA) values (:PMA_PAZ_CODICE, :PMA_MAL_CODICE, :PMA_N_MALATTIA, :PMA_DATA_DIAGNOSI, :PMA_DATA_ULTIMA_VISITA)", Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("PMA_PAZ_CODICE", codicePaziente)
                    cmd.Parameters.AddWithValue("PMA_MAL_CODICE", esenzioneMalattia.Codice)

                    If esenzioneMalattia.Numero.HasValue Then
                        cmd.Parameters.AddWithValue("PMA_N_MALATTIA", esenzioneMalattia.Numero.Value)
                    Else
                        cmd.Parameters.AddWithValue("PMA_N_MALATTIA", DBNull.Value)
                    End If

                    If esenzioneMalattia.DataInizioValidita.HasValue Then
                        cmd.Parameters.AddWithValue("PMA_DATA_DIAGNOSI", esenzioneMalattia.DataInizioValidita.Value)
                    Else
                        cmd.Parameters.AddWithValue("PMA_DATA_DIAGNOSI", DBNull.Value)
                    End If

                    If esenzioneMalattia.DataFineValidita.HasValue Then
                        cmd.Parameters.AddWithValue("PMA_DATA_ULTIMA_VISITA", esenzioneMalattia.DataFineValidita.Value)
                    Else
                        cmd.Parameters.AddWithValue("PMA_DATA_ULTIMA_VISITA", DBNull.Value)
                    End If

                    count = cmd.ExecuteNonQuery()

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Eliminazione malattie specificate per il paziente
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="codiciMalattie"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteMalattiePaziente(codicePaziente As Integer, codiciMalattie As List(Of String)) As Integer Implements IMalattieProvider.DeleteMalattiePaziente

            Dim count As Integer = 0

            Using cmd As OracleCommand = Connection.CreateCommand()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim query As String = "delete from t_paz_malattie where PMA_PAZ_CODICE = :PMA_PAZ_CODICE {0}"

                    cmd.Parameters.AddWithValue("PMA_PAZ_CODICE", codicePaziente)

                    Dim filtro As String = String.Empty

                    If codiciMalattie.Count = 1 Then

                        filtro = " and PMA_MAL_CODICE = :PMA_MAL_CODICE "
                        cmd.Parameters.AddWithValue("PMA_MAL_CODICE", codiciMalattie.First())

                    Else

                        Dim filtroInResult As GetInFilterResult = GetInFilter(codiciMalattie.ToList())

                        filtro = String.Format(" and PMA_MAL_CODICE in ({0}) ", filtroInResult.InFilter)
                        cmd.Parameters.AddRange(filtroInResult.Parameters)

                    End If

                    cmd.CommandText = String.Format(query, filtro)

                    count = cmd.ExecuteNonQuery()

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Restituisce una lista (codice malattia, descrizione malattia, codice vaccinazione), per le malattie associate al paziente e relative alle tipologie specificate.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="codiciTipologiaMalattia"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCondizioniSanitariePaziente(codicePaziente As Integer, codiciTipologiaMalattia As List(Of String)) As List(Of CondizioneSanitaria) Implements IMalattieProvider.GetCondizioniSanitariePaziente

            Dim list As New List(Of CondizioneSanitaria)()

            Using cmd As OracleCommand = Connection.CreateCommand()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim query As String = "select distinct PMA_MAL_CODICE, MAL_DESCRIZIONE, LMV_VAC_CODICE from T_PAZ_MALATTIE " +
                                          "join T_ANA_LINK_MALATTIE_TIPOLOGIA on PMA_MAL_CODICE = MML_MAL_CODICE " +
                                          "join T_ANA_LINK_MALATTIE_VAC on PMA_MAL_CODICE = LMV_MAL_CODICE " +
                                          "join T_ANA_MALATTIE on PMA_MAL_CODICE = MAL_CODICE " +
                                          "where PMA_PAZ_CODICE = :pma_paz_codice " +
                                          "and MML_MTI_CODICE {0}"

                    cmd.Parameters.AddWithValue("pma_paz_codice", codicePaziente)

                    Dim filter As New System.Text.StringBuilder()

                    If Not codiciTipologiaMalattia Is Nothing AndAlso codiciTipologiaMalattia.Count > 0 Then

                        If codiciTipologiaMalattia.Count = 1 Then

                            filter.Append(" = :mml_mti_codice ")
                            cmd.Parameters.AddWithValue("mml_mti_codice", codiciTipologiaMalattia.First())

                        Else

                            Dim filtroInResult As GetInFilterResult = Me.GetInFilter(codiciTipologiaMalattia)

                            filter.AppendFormat(" in ({0}) ", filtroInResult.InFilter)
                            cmd.Parameters.AddRange(filtroInResult.Parameters)

                        End If

                    End If

                    cmd.CommandText = String.Format(query, filter.ToString())

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim pma_mal_codice As Integer = idr.GetOrdinal("PMA_MAL_CODICE")
                            Dim mal_descrizione As Integer = idr.GetOrdinal("MAL_DESCRIZIONE")
                            Dim lmv_vac_codice As Integer = idr.GetOrdinal("LMV_VAC_CODICE")

                            While idr.Read()

                                Dim item As New CondizioneSanitaria()
                                item.CodiceMalattia = idr.GetString(pma_mal_codice)
                                item.DescrizioneMalattia = idr.GetString(mal_descrizione)
                                item.CodiceVaccinazione = idr.GetString(lmv_vac_codice)

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


        ''' <summary>
        ''' Restituisce le condizioni sanitarie del paziente per la vaccinazione specificata
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="codiceVaccinazione"></param>
        ''' <remarks>Utilizza la vista V_CONDIZIONI_SANITARIE</remarks>
        ''' <returns></returns>
        Public Function GetCondizioniSanitarie(codicePaziente As Long, codiceVaccinazione As String) As List(Of PazienteCondizioneSanitaria) Implements IMalattieProvider.GetCondizioniSanitarie

            Dim list As New List(Of PazienteCondizioneSanitaria)()

            Using cmd As OracleCommand = New OracleCommand("select * from V_CONDIZIONI_SANITARIE where VCS_PAZ_CODICE = :paz and VCS_VAC_CODICE = :vac", Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("paz", codicePaziente)
                    cmd.Parameters.AddWithValue("vac", codiceVaccinazione)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim vcs_mal_codice As Integer = idr.GetOrdinal("VCS_MAL_CODICE")
                            Dim vcs_mal_descrizione As Integer = idr.GetOrdinal("VCS_MAL_DESCRIZIONE")
                            Dim vcs_vac_codice As Integer = idr.GetOrdinal("VCS_VAC_CODICE")
                            Dim vcs_paz_codice As Integer = idr.GetOrdinal("VCS_PAZ_CODICE")
                            Dim vcs_paz_mal_codice As Integer = idr.GetOrdinal("VCS_PAZ_MAL_CODICE")
                            Dim vcs_cond_sanitaria_default As Integer = idr.GetOrdinal("VCS_COND_SANITARIA_DEFAULT")

                            While idr.Read()

                                Dim item As New PazienteCondizioneSanitaria()

                                item.CodiceMalattia = idr.GetStringOrDefault(vcs_mal_codice)
                                item.CodicePaziente = idr.GetInt64OrDefault(vcs_paz_codice)
                                item.CodiceVaccinazione = idr.GetStringOrDefault(vcs_vac_codice)
                                item.DescrizioneMalattia = idr.GetStringOrDefault(vcs_mal_descrizione)
                                item.FlagCondizioneSanitariaDefault = idr.GetStringOrDefault(vcs_cond_sanitaria_default)
                                item.FlagMalattiaPaziente = idr.GetStringOrDefault(vcs_paz_mal_codice)

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

        Public Function GetCondizioniSanitarieByAssociazione(codicePaziente As Long, codiceAssociazione As String) As List(Of PazienteCondizioneSanitaria) Implements IMalattieProvider.GetCondizioniSanitarieByAssociazione

            Dim list As New List(Of PazienteCondizioneSanitaria)()

            Dim query As String = "SELECT V_CONDIZIONI_SANITARIE.* " +
                "FROM T_ANA_LINK_ASS_VACCINAZIONI " +
                "JOIN V_CONDIZIONI_SANITARIE ON VAL_VAC_CODICE = VCS_VAC_CODICE " +
                "WHERE VAL_ASS_CODICE = :ass " +
                "AND VCS_PAZ_CODICE = :paz "
            Using cmd As New OracleClient.OracleCommand(query, Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("paz", codicePaziente)
                    cmd.Parameters.AddWithValue("ass", codiceAssociazione)

                    list = GetListCondizioniSanitarie(cmd)

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list

        End Function
        Private Function GetListCondizioniSanitarie(cmd As OracleClient.OracleCommand) As List(Of PazienteCondizioneSanitaria)

            Dim list As New List(Of PazienteCondizioneSanitaria)()
            Using idr As IDataReader = cmd.ExecuteReader()
                If Not idr Is Nothing Then

                    Dim vcs_mal_codice As Integer = idr.GetOrdinal("VCS_MAL_CODICE")
                    Dim vcs_mal_descrizione As Integer = idr.GetOrdinal("VCS_MAL_DESCRIZIONE")
                    Dim vcs_vac_codice As Integer = idr.GetOrdinal("VCS_VAC_CODICE")
                    Dim vcs_paz_codice As Integer = idr.GetOrdinal("VCS_PAZ_CODICE")
                    Dim vcs_paz_mal_codice As Integer = idr.GetOrdinal("VCS_PAZ_MAL_CODICE")
                    Dim vcs_cond_sanitaria_default As Integer = idr.GetOrdinal("VCS_COND_SANITARIA_DEFAULT")

                    While idr.Read()

                        Dim item As New PazienteCondizioneSanitaria()

                        item.CodiceMalattia = idr.GetStringOrDefault(vcs_mal_codice)
                        item.CodicePaziente = idr.GetInt64OrDefault(vcs_paz_codice)
                        item.CodiceVaccinazione = idr.GetStringOrDefault(vcs_vac_codice)
                        item.DescrizioneMalattia = idr.GetStringOrDefault(vcs_mal_descrizione)
                        item.FlagCondizioneSanitariaDefault = idr.GetStringOrDefault(vcs_cond_sanitaria_default)
                        item.FlagMalattiaPaziente = idr.GetStringOrDefault(vcs_paz_mal_codice)

                        list.Add(item)

                    End While

                End If
            End Using
            Return list
        End Function
        ''' <summary>
        ''' Restituisce una lista di coppie codice-descrizione per le malattie specificate.
        ''' </summary>
        ''' <param name="codiciMalattie"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCodiceDescrizioneMalattie(codiciMalattie As List(Of String)) As List(Of KeyValuePair(Of String, String)) Implements IMalattieProvider.GetCodiceDescrizioneMalattie

            Return GetCodiceDescrizioneMalattie(codiciMalattie, Nothing)

        End Function

        ''' <summary>
        ''' Restituisce una lista di coppie codice-descrizione per le malattie aventi le tipologie specificate.
        ''' </summary>
        ''' <param name="codiciMalattie"></param>
        ''' <param name="codiciTipologieMalattia"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCodiceDescrizioneMalattie(codiciMalattie As List(Of String), codiciTipologieMalattia As List(Of String)) As List(Of KeyValuePair(Of String, String)) Implements IMalattieProvider.GetCodiceDescrizioneMalattie

            Dim list As New List(Of KeyValuePair(Of String, String))()

            Using cmd As OracleCommand = Connection.CreateCommand()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim join As String = String.Empty
                    Dim filter As New System.Text.StringBuilder()

                    If Not codiciMalattie.IsNullOrEmpty() Then

                        If codiciMalattie.Count = 1 Then

                            filter.Append(" MAL_CODICE = :mal_codice and ")
                            cmd.Parameters.AddWithValue("mal_codice", codiciMalattie.First())

                        Else

                            Dim filtroInResult As GetInFilterResult = Me.GetInFilter(codiciMalattie)

                            filter.AppendFormat(" MAL_CODICE in ({0}) and ", filtroInResult.InFilter)
                            cmd.Parameters.AddRange(filtroInResult.Parameters)

                        End If

                    End If

                    If Not codiciTipologieMalattia.IsNullOrEmpty() Then

                        join = " join T_ANA_LINK_MALATTIE_TIPOLOGIA on MAL_CODICE = MML_MAL_CODICE "

                        If codiciTipologieMalattia.Count = 1 Then

                            filter.Append(" MML_MTI_CODICE = :mml_mti_codice and ")
                            cmd.Parameters.AddWithValue("MML_MTI_CODICE", codiciTipologieMalattia.First())

                        Else

                            Dim filtroInResult As GetInFilterResult = Me.GetInFilter(codiciTipologieMalattia)

                            filter.AppendFormat(" MML_MTI_CODICE in ({0}) and ", filtroInResult.InFilter)
                            cmd.Parameters.AddRange(filtroInResult.Parameters)

                        End If

                    End If

                    If filter.Length > 0 Then
                        filter.RemoveLast(4)
                        filter.Insert(0, " where ")
                    End If

                    cmd.CommandText = String.Format("select MAL_CODICE, MAL_DESCRIZIONE from T_ANA_MALATTIE {0} {1} order by MAL_DESCRIZIONE ", join, filter.ToString())

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim mal_codice As Integer = idr.GetOrdinal("MAL_CODICE")
                            Dim mal_descrizione As Integer = idr.GetOrdinal("MAL_DESCRIZIONE")

                            While idr.Read()
                                list.Add(New KeyValuePair(Of String, String)(idr.GetString(mal_codice), idr.GetString(mal_descrizione)))
                            End While

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list

        End Function

#End Region

#Region " Private "

        Private Function GetQuerySelectEsenzioniMalattiaPaziente() As String

            Return " select PMA_MAL_CODICE, PMA_N_MALATTIA, PMA_DATA_DIAGNOSI, PMA_DATA_ULTIMA_VISITA, MAL_CODICE_ESENZIONE " +
                   " from T_PAZ_MALATTIE " +
                   " join T_ANA_MALATTIE on PMA_MAL_CODICE = MAL_CODICE " +
                   " where PMA_PAZ_CODICE = :PMA_PAZ_CODICE "

        End Function

        Private Function GetListEsenzioniMalattia(cmd As OracleCommand) As List(Of EsenzioneMalattia)

            Dim list As New List(Of EsenzioneMalattia)()

            Using idr As IDataReader = cmd.ExecuteReader()

                If Not idr Is Nothing Then

                    Dim pma_mal_codice As Integer = idr.GetOrdinal("PMA_MAL_CODICE")
                    Dim pma_n_malattia As Integer = idr.GetOrdinal("PMA_N_MALATTIA")
                    Dim pma_data_diagnosi As Integer = idr.GetOrdinal("PMA_DATA_DIAGNOSI")
                    Dim pma_data_ultima_visita As Integer = idr.GetOrdinal("PMA_DATA_ULTIMA_VISITA")
                    Dim mal_codice_esenzione As Integer = idr.GetOrdinal("MAL_CODICE_ESENZIONE")

                    While idr.Read()

                        Dim item As New EsenzioneMalattia()
                        item.Codice = idr.GetString(pma_mal_codice)
                        item.Numero = idr.GetNullableInt32OrDefault(pma_n_malattia)
                        item.CodiceEsenzione = idr.GetStringOrDefault(mal_codice_esenzione)
                        item.DataInizioValidita = idr.GetNullableDateTimeOrDefault(pma_data_diagnosi)
                        item.DataFineValidita = idr.GetNullableDateTimeOrDefault(pma_data_ultima_visita)

                        list.Add(item)

                    End While
                End If

            End Using

            Return list

        End Function

        Private Function GetListMalattie(idr As IDataReader) As List(Of Malattia)

            Dim listMalattie As New List(Of Malattia)()

            If Not idr Is Nothing Then

                Dim malattia As Malattia = Nothing

                Dim mal_codice As Integer = idr.GetOrdinal("MAL_CODICE")
                Dim mal_descrizione As Integer = idr.GetOrdinal("MAL_DESCRIZIONE")
                Dim mal_flag_visita As Integer = idr.GetOrdinal("MAL_FLAG_VISITA")
                Dim mal_codice_esenzione As Integer = idr.GetOrdinal("MAL_CODICE_ESENZIONE")

                Dim codiceMalattia As String = String.Empty
                Dim descrizioneMalattia As String = String.Empty
                Dim flagVisita As Boolean
                Dim codiceEsenzione As String = String.Empty

                While idr.Read()

                    codiceMalattia = idr.GetStringOrDefault(mal_codice)
                    descrizioneMalattia = idr.GetStringOrDefault(mal_descrizione)
                    flagVisita = idr.GetBooleanOrDefault(mal_flag_visita)
                    codiceEsenzione = idr.GetStringOrDefault(mal_codice_esenzione)

                    malattia =
                        New Malattia() With {
                            .CodiceMalattia = codiceMalattia,
                            .DescrizioneMalattia = descrizioneMalattia,
                            .FlagVisita = flagVisita,
                            .CodiceEsenzione = codiceEsenzione
                        }

                    listMalattie.Add(malattia)

                End While

            End If

            Return listMalattie

        End Function

#End Region

    End Class

End Namespace