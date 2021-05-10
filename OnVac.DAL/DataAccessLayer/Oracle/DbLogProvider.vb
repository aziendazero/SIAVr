Imports System.Collections.Generic
Imports System.Data.OracleClient

Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.Data.OracleClient
Imports Onit.OnAssistnet.OnVac.DAL


Public Class DbLogProvider
    Inherits DbProvider
    Implements ILogProvider

#Region " Costruttore "

    Public Sub New(dam As IDAM)

        MyBase.New(dam)

    End Sub

#End Region

#Region " ILogProvider "

    Public Function GetDataTableTestateOperazioniGruppo(argomento As String, listOperazioni As List(Of DataLogStructure.Operazione), dataOperazioneInizio As Date, dataOperazioneFine As Date) As DataTable Implements ILogProvider.GetDataTableTestateOperazioniGruppo

        Dim dtTestateOperazioniGruppo As New DataTable()

        Dim cmd As OracleCommand = Nothing
        Dim adp As OracleDataAdapter = Nothing
        Dim ownConnection As Boolean = False

        Try

            'N.B. il to_char è fatto perchè in modalità MULTI-USL (ex: veneto) il campo paz_codice deve essere una stringa perchè gestito dal servizio pazienti

            cmd = New System.Data.OracleClient.OracleCommand()
            cmd.Connection = Me.Connection

            cmd.Parameters.AddWithValue("argomento", argomento)
            cmd.Parameters.AddWithValue("dataOperazioneInizio", dataOperazioneInizio)
            cmd.Parameters.AddWithValue("dataOperazioneFine", dataOperazioneFine)

            Dim queryStringBuilder As New System.Text.StringBuilder()
            queryStringBuilder.Append("SELECT lot_codice, lot_utente, lot_operazione, lot_paziente, lot_automatico, NVL (ute_descrizione, '-') ute_descrizione, ")
            queryStringBuilder.Append("paz_cognome, paz_nome, paz_data_nascita, lot_gruppo, lot_stack, lot_data_operazione ")

            queryStringBuilder.Append("FROM t_log_testata ")
            queryStringBuilder.Append("LEFT OUTER JOIN t_ana_utenti ON lot_utente = ute_id ")
            queryStringBuilder.Append("LEFT OUTER JOIN t_paz_pazienti ON to_char(lot_paziente) = to_char(paz_codice) ")

            queryStringBuilder.Append("WHERE TRUNC(lot_data_operazione) >= :dataOperazioneInizio AND lot_argomento = :argomento ")

            If listOperazioni.Count > 0 Then

                queryStringBuilder.Append("AND ( ")

                For i As Int16 = 0 To listOperazioni.Count - 1

                    Dim paramName As String = String.Format("operazione{0}", i)

                    queryStringBuilder.AppendFormat("lot_operazione = :{0} OR ", paramName)

                    cmd.Parameters.AddWithValue(paramName, listOperazioni(i))

                Next

                queryStringBuilder.Remove(queryStringBuilder.Length - 4, 4)

                queryStringBuilder.Append(")")

            End If

            queryStringBuilder.Append(" AND TRUNC(lot_data_operazione) < :dataOperazioneFine AND lot_gruppo IS NOT NULL ORDER BY lot_data_operazione DESC")

            cmd.CommandText = queryStringBuilder.ToString()

            ownConnection = Me.ConditionalOpenConnection(cmd)

            adp = New OracleDataAdapter(cmd)
            adp.Fill(dtTestateOperazioniGruppo)

        Finally

            Me.ConditionalCloseConnection(ownConnection)

            If Not adp Is Nothing Then adp.Dispose()
            If Not cmd Is Nothing Then cmd.Dispose()

        End Try

        Return dtTestateOperazioniGruppo

    End Function

    Public Function GetDataTableTestateAllineamentoOperazioniGruppo(listOperazioni As List(Of DataLogStructure.Operazione), dataOperazioneInizio As DateTime, dataOperazioneFine As DateTime) As DataTable Implements ILogProvider.GetDataTableTestateAllineamentoOperazioniGruppo

        Dim dt As New DataTable()

        With _DAM.QB

            .NewQuery()

            .AddSelectFields("lot_codice, lot_utente, lot_operazione, lot_paziente, lot_automatico")
            .AddSelectFields(_DAM.QB.FC.IsNull("ute_descrizione", "-", DataTypes.Stringa) + " ute_descrizione")
            .AddSelectFields("all_cognome paz_cognome, all_nome paz_nome, all_data_nascita paz_data_nascita")
            .AddSelectFields("lot_gruppo, lot_stack, lot_data_operazione")
            .AddTables("t_log_testata, t_ana_utenti, log_allineamento_cedaf")

            .AddWhereCondition("lot_paziente", Comparatori.Uguale, "all_paz_codice_ausiliario", DataTypes.OutJoinLeft)
            .AddWhereCondition("lot_argomento", Comparatori.Uguale, DataLogStructure.TipiArgomento.ALLINEAMENTO, DataTypes.Stringa)
            .AddWhereCondition("lot_data_operazione", Comparatori.Uguale, "all_data", DataTypes.OutJoinLeft)

            .AddWhereCondition("lot_argomento", Comparatori.Uguale, DataLogStructure.TipiArgomento.ALLINEAMENTO, DataTypes.Stringa)
            .AddWhereCondition("lot_utente", Comparatori.Uguale, "ute_id", DataTypes.OutJoinLeft)
            .AddWhereCondition(.FC.Tronca("lot_data_operazione"), Comparatori.MaggioreUguale, dataOperazioneInizio, DataTypes.Data)

            Me.AddOperazioniSelezionateWhereConditions(listOperazioni)

            .AddWhereCondition(.FC.Tronca("lot_data_operazione"), Comparatori.Minore, dataOperazioneFine, DataTypes.Data)
            .AddWhereCondition("lot_gruppo", Comparatori.IsNot, "null", DataTypes.Replace)
            .AddOrderByFields("lot_data_operazione DESC")

        End With

        _DAM.BuildDataTable(dt)

        Return dt

    End Function

    Private Sub AddOperazioniSelezionateWhereConditions(listOperazioni As List(Of DataLogStructure.Operazione))

        With _DAM.QB

            If Not listOperazioni Is Nothing AndAlso listOperazioni.Count > 0 Then

                .OpenParanthesis()
                .AddWhereCondition("lot_operazione", Comparatori.Uguale, Convert.ToInt32(listOperazioni(0)), DataTypes.Numero)

                If listOperazioni.Count > 1 Then
                    For i As Int16 = 1 To listOperazioni.Count - 1
                        .AddWhereCondition("lot_operazione", Comparatori.Uguale, Convert.ToInt32(listOperazioni(i)), DataTypes.Numero)
                    Next
                End If

                .CloseParanthesis()

            End If

        End With

    End Sub

    Public Function GetLogDatiVaccinali(idLogDatiVaccinali As Int64) As Entities.LogDatiVaccinali Implements ILogProvider.GetLogDatiVaccinali

        Dim logDatiVaccinali As LogDatiVaccinali = Nothing

        Using cmd As New System.Data.OracleClient.OracleCommand()

            Dim sqlStringBuilder As System.Text.StringBuilder = Me.GetQueryLogDatiVaccinali(True)

            sqlStringBuilder.Append(" WHERE LDV_ID = :LDV_ID")

            cmd.Parameters.AddWithValue("LDV_ID", idLogDatiVaccinali)

            cmd.CommandText = sqlStringBuilder.ToString()

            cmd.Connection = Me.Connection

            Dim ownConnection As Boolean = False

            Try

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Dim logDatiVaccinaliList As List(Of LogDatiVaccinali) = Me.GetLogDatiVaccinaliList(cmd, True)

                If Not logDatiVaccinaliList Is Nothing Then
                    logDatiVaccinali = logDatiVaccinaliList.First()
                End If

            Finally

                Me.ConditionalCloseConnection(ownConnection)

            End Try

        End Using

        Return logDatiVaccinali

    End Function

    Public Function GetLogsDatiVaccinali(getLogsDatiVaccinaliQuery As GetLogsDatiVaccinaliQuery) As Entities.LogDatiVaccinali() Implements ILogProvider.GetLogsDatiVaccinali

        Dim logDatiVaccinaliList As List(Of LogDatiVaccinali) = Nothing

        Using cmd As New System.Data.OracleClient.OracleCommand()

            ' Creazione query (select e join)
            Dim sqlStringBuilder As System.Text.StringBuilder = Me.GetQueryLogDatiVaccinali(getLogsDatiVaccinaliQuery.IncludeInfos)

            ' Filtri
            Dim sqlWhereStringBuilder As System.Text.StringBuilder = Me.GetSqlWhereCondition(getLogsDatiVaccinaliQuery, cmd)

            If sqlWhereStringBuilder.Length > 0 Then
                sqlStringBuilder.AppendFormat(" WHERE {0}", sqlWhereStringBuilder.ToString())
            End If

            ' Ordinamento
            If getLogsDatiVaccinaliQuery.IncludeInfos Then
                sqlStringBuilder.AppendFormat(" ORDER BY PAZ_COGNOME, PAZ_NOME, PAZ_DATA_NASCITA, PAZ_CODICE, LDV_ID")
            Else
                sqlStringBuilder.AppendFormat(" ORDER BY LDV_ID")
            End If

            cmd.CommandText = sqlStringBuilder.ToString()

            ' Paginazione
            If Not getLogsDatiVaccinaliQuery.PagingOptions Is Nothing Then
                cmd.AddPaginatedQuery(getLogsDatiVaccinaliQuery.PagingOptions)
            End If

            cmd.Connection = Me.Connection

            Dim ownConnection As Boolean = False

            Try

                ownConnection = Me.ConditionalOpenConnection(cmd)

                logDatiVaccinaliList = Me.GetLogDatiVaccinaliList(cmd, getLogsDatiVaccinaliQuery.IncludeInfos)

            Finally

                Me.ConditionalCloseConnection(ownConnection)

            End Try

        End Using

        Return logDatiVaccinaliList.ToArray()

    End Function

    Public Function CountLogsDatiVaccinali(getLogsDatiVaccinaliQuery As GetLogsDatiVaccinaliQuery) As Integer Implements ILogProvider.CountLogsDatiVaccinali

        Dim countRisultati As Integer = 0

        Using cmd As New System.Data.OracleClient.OracleCommand()

            Dim sqlWhereStringBuilder As System.Text.StringBuilder = Me.GetSqlWhereCondition(getLogsDatiVaccinaliQuery, cmd)

            Dim sqlStringBuilder As New System.Text.StringBuilder("SELECT COUNT(*) FROM T_LOG_DATI_VACCINALI INNER JOIN T_PAZ_PAZIENTI ON LDV_PAZ_CODICE = PAZ_CODICE ")

            If sqlWhereStringBuilder.Length > 0 Then
                sqlStringBuilder.AppendFormat(" WHERE {0}", sqlWhereStringBuilder.ToString())
            End If

            cmd.CommandText = sqlStringBuilder.ToString()

            cmd.Connection = Me.Connection
            Dim ownConnection As Boolean = False

            Try

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Dim obj As Object = cmd.ExecuteScalar()

                If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then

                    countRisultati = Convert.ToInt32(obj)

                End If

            Finally

                Me.ConditionalCloseConnection(ownConnection)

            End Try

        End Using

        Return countRisultati

    End Function

    Public Function GetArgomentiLog(soloAttivi As Boolean, listCodiciArgomenti As List(Of String)) As List(Of Log.DataLogStructure.Argomento) Implements ILogProvider.GetArgomentiLog

        Dim listArgomenti As New List(Of DataLogStructure.Argomento)()

        With _DAM.QB

            .NewQuery()

            .AddSelectFields("LOA_CODICE, LOA_DESCRIZIONE, LOA_CRITICITA, LOA_ATTIVO")

            If soloAttivi Then
                .AddWhereCondition("LOA_ATTIVO", Comparatori.Uguale, "S", DataTypes.Stringa)
            End If

            If Not listCodiciArgomenti Is Nothing AndAlso listCodiciArgomenti.Count > 0 Then

                Dim filtroCodici As New System.Text.StringBuilder()

                For Each codiceArgomento As String In listCodiciArgomenti
                    filtroCodici.AppendFormat("{0},", .AddCustomParam(codiceArgomento))
                Next

                filtroCodici.Remove(filtroCodici.Length - 1, 1)

                .AddWhereCondition("LOA_CODICE", Comparatori.In, filtroCodici.ToString(), DataTypes.Replace)

            End If

            .AddTables("T_LOG_ARGOMENTI")

            .AddOrderByFields("LOA_CODICE")

        End With

        Using idr As System.Data.IDataReader = _DAM.BuildDataReader()

            If Not idr Is Nothing Then

                Dim loa_codice As Integer = idr.GetOrdinal("LOA_CODICE")
                Dim loa_descrizione As Integer = idr.GetOrdinal("LOA_DESCRIZIONE")
                Dim loa_criticita As Integer = idr.GetOrdinal("LOA_CRITICITA")
                Dim loa_attivo As Integer = idr.GetOrdinal("LOA_ATTIVO")

                Dim criticita As DataLogStructure.Criticita

                While idr.Read()

                    If idr.IsDBNull(loa_criticita) Then
                        criticita = DataLogStructure.Criticita.Log
                    Else
                        criticita = DirectCast([Enum].Parse(GetType(DataLogStructure.Criticita), idr(loa_criticita).ToString()), DataLogStructure.Criticita)
                    End If

                    listArgomenti.Add(New DataLogStructure.Argomento(idr.GetString(loa_codice), idr.GetStringOrDefault(loa_descrizione), criticita, idr.GetBooleanOrDefault(loa_attivo)))

                End While

            End If

        End Using

        Return listArgomenti

    End Function

    Public Sub InsertLogDatiVaccinali(logDatiVaccinali As Entities.LogDatiVaccinali) Implements ILogProvider.InsertLogDatiVaccinali

        Using cmd As New System.Data.OracleClient.OracleCommand()

            cmd.Connection = Me.Connection

            Dim ownConnection As Boolean = False

            Try

                ownConnection = Me.ConditionalOpenConnection(cmd)

                cmd.CommandText = "select seq_t_log_dati_vaccinali.nextval from dual"

                logDatiVaccinali.Id = cmd.ExecuteScalar()

                cmd.CommandText = "INSERT INTO T_LOG_DATI_VACCINALI(LDV_ID, LDV_PAZ_CODICE, LDV_DATA, LDV_UTE_ID, LDV_USL_CODICE, LDV_LOA_CODICE, LDV_OPERAZIONE, LDV_STATO, LDV_NOTE) VALUES (:LDV_ID, :LDV_PAZ_CODICE, :LDV_DATA, :LDV_UTE_ID, :LDV_USL_CODICE, :LDV_LOA_CODICE, :LDV_OPERAZIONE, :LDV_STATO, :LDV_NOTE)"

                cmd.Parameters.AddWithValue("LDV_ID", logDatiVaccinali.Id)
                cmd.Parameters.AddWithValue("LDV_PAZ_CODICE", logDatiVaccinali.Paziente.Paz_Codice)
                cmd.Parameters.AddWithValue("LDV_DATA", logDatiVaccinali.DataOperazione)
                cmd.Parameters.AddWithValue("LDV_UTE_ID", logDatiVaccinali.Utente.Id)
                cmd.Parameters.AddWithValue("LDV_USL_CODICE", logDatiVaccinali.Usl.Codice)
                cmd.Parameters.AddWithValue("LDV_LOA_CODICE", logDatiVaccinali.Argomento.Codice)
                cmd.Parameters.AddWithValue("LDV_OPERAZIONE", logDatiVaccinali.Operazione)
                cmd.Parameters.AddWithValue("LDV_STATO", logDatiVaccinali.Stato)
                cmd.Parameters.AddWithValue("LDV_NOTE", IIf(logDatiVaccinali.Note Is Nothing, String.Empty, logDatiVaccinali.Note))

                cmd.ExecuteNonQuery()

            Finally

                Me.ConditionalCloseConnection(ownConnection)

            End Try

        End Using

    End Sub

#End Region

#Region " Private "

    Private Function GetSqlWhereCondition(getLogsDatiVaccinaliQuery As GetLogsDatiVaccinaliQuery, cmd As System.Data.OracleClient.OracleCommand) As System.Text.StringBuilder

        Dim sqlWhereStringBuilder As New System.Text.StringBuilder()

        ' --- Filtri sull'operazione --- '

        If getLogsDatiVaccinaliQuery.DataOperazioneMinima.HasValue Then

            If sqlWhereStringBuilder.Length > 0 Then sqlWhereStringBuilder.Append(" AND")
            sqlWhereStringBuilder.Append(" LDV_DATA >= :LDV_DATA_MIN ")

            cmd.Parameters.AddWithValue("LDV_DATA_MIN", getLogsDatiVaccinaliQuery.DataOperazioneMinima.Value.Date)

        End If

        If getLogsDatiVaccinaliQuery.DataOperazioneMassima.HasValue Then

            If sqlWhereStringBuilder.Length > 0 Then sqlWhereStringBuilder.Append(" AND")
            sqlWhereStringBuilder.Append(" LDV_DATA < :LDV_DATA_MAX ")

            cmd.Parameters.AddWithValue("LDV_DATA_MAX", getLogsDatiVaccinaliQuery.DataOperazioneMassima.Value.AddDays(1).Date)

        End If

        If Not String.IsNullOrEmpty(getLogsDatiVaccinaliQuery.CodiceArgomento) Then

            If sqlWhereStringBuilder.Length > 0 Then sqlWhereStringBuilder.Append(" AND")
            sqlWhereStringBuilder.Append(" LDV_LOA_CODICE = :LDV_LOA_CODICE ")

            cmd.Parameters.AddWithValue("LDV_LOA_CODICE", getLogsDatiVaccinaliQuery.CodiceArgomento)

        End If

        If getLogsDatiVaccinaliQuery.Operazione.HasValue Then

            If sqlWhereStringBuilder.Length > 0 Then sqlWhereStringBuilder.Append(" AND")
            sqlWhereStringBuilder.Append(" LDV_OPERAZIONE = :LDV_OPERAZIONE ")

            cmd.Parameters.AddWithValue("LDV_OPERAZIONE", getLogsDatiVaccinaliQuery.Operazione.Value)

        End If

        If getLogsDatiVaccinaliQuery.StatoOperazione.HasValue Then

            If sqlWhereStringBuilder.Length > 0 Then sqlWhereStringBuilder.Append(" AND")
            sqlWhereStringBuilder.Append(" LDV_STATO = :LDV_STATO ")

            cmd.Parameters.AddWithValue("LDV_STATO", getLogsDatiVaccinaliQuery.StatoOperazione.Value)

        End If

        ' --- Filtri sul paziente --- '

        If Not String.IsNullOrEmpty(getLogsDatiVaccinaliQuery.CognomePaziente) Then

            If sqlWhereStringBuilder.Length > 0 Then sqlWhereStringBuilder.Append(" AND")
            sqlWhereStringBuilder.Append(" PAZ_COGNOME LIKE :PAZ_COGNOME ")

            cmd.Parameters.AddWithValue("PAZ_COGNOME", getLogsDatiVaccinaliQuery.CognomePaziente + "%")

        End If

        If Not String.IsNullOrEmpty(getLogsDatiVaccinaliQuery.NomePaziente) Then

            If sqlWhereStringBuilder.Length > 0 Then sqlWhereStringBuilder.Append(" AND")
            sqlWhereStringBuilder.Append(" PAZ_NOME LIKE :PAZ_NOME ")

            cmd.Parameters.AddWithValue("PAZ_NOME", getLogsDatiVaccinaliQuery.NomePaziente + "%")

        End If

        If Not String.IsNullOrEmpty(getLogsDatiVaccinaliQuery.CodiceFiscalePaziente) Then

            If sqlWhereStringBuilder.Length > 0 Then sqlWhereStringBuilder.Append(" AND")
            sqlWhereStringBuilder.Append(" PAZ_CODICE_FISCALE LIKE :PAZ_CODICE_FISCALE ")

            cmd.Parameters.AddWithValue("PAZ_CODICE_FISCALE", getLogsDatiVaccinaliQuery.CodiceFiscalePaziente + "%")

        End If

        If getLogsDatiVaccinaliQuery.DataNascitaPazienteMinima.HasValue Then

            If sqlWhereStringBuilder.Length > 0 Then sqlWhereStringBuilder.Append(" AND")
            sqlWhereStringBuilder.Append(" PAZ_DATA_NASCITA >= :PAZ_DATA_NASCITA_MIN ")

            cmd.Parameters.AddWithValue("PAZ_DATA_NASCITA_MIN", getLogsDatiVaccinaliQuery.DataNascitaPazienteMinima.Value.Date)

        End If

        If getLogsDatiVaccinaliQuery.DataNascitaPazienteMassima.HasValue Then

            If sqlWhereStringBuilder.Length > 0 Then sqlWhereStringBuilder.Append(" AND")
            sqlWhereStringBuilder.Append(" PAZ_DATA_NASCITA < :PAZ_DATA_NASCITA_MAX ")

            cmd.Parameters.AddWithValue("PAZ_DATA_NASCITA_MAX", getLogsDatiVaccinaliQuery.DataNascitaPazienteMassima.Value.AddDays(1).Date)

        End If

        If getLogsDatiVaccinaliQuery.StatiAnagraficiPaziente.Length > 0 Then

            If sqlWhereStringBuilder.Length > 0 Then sqlWhereStringBuilder.Append(" AND")

            sqlWhereStringBuilder.Append(" PAZ_STATO_ANAGRAFICO IN (")

            For statiAnagraficiPazienteIndex As Int16 = 0 To getLogsDatiVaccinaliQuery.StatiAnagraficiPaziente.Length - 1

                Dim statoAnagraficoParameterName As String = String.Format(":PAZ_STATO_ANAGRAFICO_{0}", statiAnagraficiPazienteIndex)

                sqlWhereStringBuilder.AppendFormat("{0}{1}", IIf(statiAnagraficiPazienteIndex = 0, String.Empty, ", "), statoAnagraficoParameterName)

                cmd.Parameters.AddWithValue(statoAnagraficoParameterName, getLogsDatiVaccinaliQuery.StatiAnagraficiPaziente(statiAnagraficiPazienteIndex))

            Next

            sqlWhereStringBuilder.Append(") ")

        End If

        If Not String.IsNullOrEmpty(getLogsDatiVaccinaliQuery.CodiceCentroVaccinalePaziente) Then

            If sqlWhereStringBuilder.Length > 0 Then sqlWhereStringBuilder.Append(" AND")
            sqlWhereStringBuilder.Append(" PAZ_CNS_CODICE = :PAZ_CNS_CODICE ")

            cmd.Parameters.AddWithValue("PAZ_CNS_CODICE", getLogsDatiVaccinaliQuery.CodiceCentroVaccinalePaziente)

        End If

        Return sqlWhereStringBuilder

    End Function

    Private Function GetQueryLogDatiVaccinali(includeInfos As Boolean) As System.Text.StringBuilder

        Dim sqlStringBuilder As New System.Text.StringBuilder("SELECT T_LOG_DATI_VACCINALI.* ")

        If includeInfos Then
            sqlStringBuilder.Append(", T_PAZ_PAZIENTI.PAZ_COGNOME, T_PAZ_PAZIENTI.PAZ_NOME, T_PAZ_PAZIENTI.PAZ_DATA_NASCITA ")
            sqlStringBuilder.Append(", T_PAZ_PAZIENTI.PAZ_STATO_ANAGRAFICO, T_PAZ_PAZIENTI.PAZ_CODICE_FISCALE, T_PAZ_PAZIENTI.PAZ_CNS_CODICE ")
            sqlStringBuilder.Append(", T_ANA_USL.USL_DESCRIZIONE, T_LOG_ARGOMENTI.LOA_DESCRIZIONE ")
            sqlStringBuilder.Append(", T_ANA_UTENTI.UTE_CODICE, T_ANA_UTENTI.UTE_DESCRIZIONE ")
        End If

        sqlStringBuilder.Append("FROM T_LOG_DATI_VACCINALI ")
        sqlStringBuilder.Append("INNER JOIN T_PAZ_PAZIENTI ON LDV_PAZ_CODICE = PAZ_CODICE ")

        If includeInfos Then
            sqlStringBuilder.Append("INNER JOIN T_ANA_USL ON LDV_USL_CODICE = USL_CODICE ")
            sqlStringBuilder.Append("INNER JOIN T_ANA_UTENTI ON LDV_UTE_ID = UTE_ID ")
            sqlStringBuilder.Append("INNER JOIN T_LOG_ARGOMENTI ON LDV_LOA_CODICE = LOA_CODICE ")
        End If

        Return sqlStringBuilder

    End Function

    Private Function GetLogDatiVaccinaliList(cmd As OracleCommand, includeInfos As Boolean) As List(Of LogDatiVaccinali)

        Dim logDatiVaccinaliList As New List(Of LogDatiVaccinali)()

        Using reader As OracleDataReader = cmd.ExecuteReader()

            If reader.HasRows Then

                Dim LDV_ID As Int16 = reader.GetOrdinal("LDV_ID")
                Dim LDV_PAZ_CODICE As Int16 = reader.GetOrdinal("LDV_PAZ_CODICE")
                Dim LDV_DATA As Int16 = reader.GetOrdinal("LDV_DATA")
                Dim LDV_UTE_ID As Int16 = reader.GetOrdinal("LDV_UTE_ID")
                Dim LDV_USL_CODICE As Int16 = reader.GetOrdinal("LDV_USL_CODICE")
                Dim LDV_LOA_CODICE As Int16 = reader.GetOrdinal("LDV_LOA_CODICE")
                Dim LDV_OPERAZIONE As Int16 = reader.GetOrdinal("LDV_OPERAZIONE")
                Dim LDV_STATO As Int16 = reader.GetOrdinal("LDV_STATO")
                Dim LDV_NOTE As Int16 = reader.GetOrdinal("LDV_NOTE")

                Dim PAZ_COGNOME As Int16
                Dim PAZ_NOME As Int16
                Dim PAZ_DATA_NASCITA As Int16
                Dim PAZ_STATO_ANAGRAFICO As Int16
                Dim PAZ_CODICE_FISCALE As Int16
                Dim PAZ_CNS_CODICE As Int16
                Dim USL_DESCRIZIONE As Int16
                Dim UTE_CODICE As Int16
                Dim UTE_DESCRIZIONE As Int16
                Dim LOA_DESCRIZIONE As Int16

                If includeInfos Then

                    PAZ_COGNOME = reader.GetOrdinal("PAZ_COGNOME")
                    PAZ_NOME = reader.GetOrdinal("PAZ_NOME")
                    PAZ_DATA_NASCITA = reader.GetOrdinal("PAZ_DATA_NASCITA")
                    PAZ_STATO_ANAGRAFICO = reader.GetOrdinal("PAZ_STATO_ANAGRAFICO")
                    PAZ_CODICE_FISCALE = reader.GetOrdinal("PAZ_CODICE_FISCALE")
                    PAZ_CNS_CODICE = reader.GetOrdinal("PAZ_CNS_CODICE")
                    USL_DESCRIZIONE = reader.GetOrdinal("USL_DESCRIZIONE")
                    UTE_CODICE = reader.GetOrdinal("UTE_CODICE")
                    UTE_DESCRIZIONE = reader.GetOrdinal("UTE_DESCRIZIONE")
                    LOA_DESCRIZIONE = reader.GetOrdinal("LOA_DESCRIZIONE")

                End If

                While reader.Read()

                    Dim logDatiVaccinali As New LogDatiVaccinali()

                    logDatiVaccinali.Id = reader.GetInt64(LDV_ID)
                    logDatiVaccinali.Paziente.Paz_Codice = reader.GetInt32(LDV_PAZ_CODICE)
                    logDatiVaccinali.DataOperazione = reader.GetDateTime(LDV_DATA)
                    logDatiVaccinali.Utente.Id = reader.GetInt32(LDV_UTE_ID)
                    logDatiVaccinali.Usl.Codice = reader.GetString(LDV_USL_CODICE)
                    logDatiVaccinali.Argomento.Codice = reader.GetString(LDV_LOA_CODICE)
                    logDatiVaccinali.Operazione = Convert.ToInt32(reader.GetString(LDV_OPERAZIONE))
                    logDatiVaccinali.Stato = reader.GetInt32(LDV_STATO)
                    logDatiVaccinali.Note = reader.GetStringOrDefault(LDV_NOTE)

                    If includeInfos Then

                        logDatiVaccinali.Paziente.PAZ_COGNOME = reader.GetStringOrDefault(PAZ_COGNOME)
                        logDatiVaccinali.Paziente.PAZ_NOME = reader.GetStringOrDefault(PAZ_NOME)
                        logDatiVaccinali.Paziente.Data_Nascita = reader.GetDateTimeOrDefault(PAZ_DATA_NASCITA)
                        logDatiVaccinali.Paziente.StatoAnagrafico = Convert.ToInt32(reader.GetStringOrDefault(PAZ_STATO_ANAGRAFICO))
                        logDatiVaccinali.Paziente.PAZ_CODICE_FISCALE = reader.GetStringOrDefault(PAZ_CODICE_FISCALE)
                        logDatiVaccinali.Paziente.Paz_Cns_Codice = reader.GetStringOrDefault(PAZ_CNS_CODICE)
                        logDatiVaccinali.Usl.Descrizione = reader.GetStringOrDefault(USL_DESCRIZIONE)
                        logDatiVaccinali.Utente.Codice = reader.GetStringOrDefault(UTE_CODICE)
                        logDatiVaccinali.Utente.Descrizione = reader.GetStringOrDefault(UTE_DESCRIZIONE)
                        logDatiVaccinali.Argomento.Descrizione = reader.GetStringOrDefault(LOA_DESCRIZIONE)

                    End If

                    logDatiVaccinaliList.Add(logDatiVaccinali)

                End While

            End If

        End Using

        Return logDatiVaccinaliList

    End Function

#End Region

End Class


