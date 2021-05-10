Imports System.Collections.Generic
Imports System.Data.OracleClient

Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.Collection
Imports Onit.OnAssistnet.OnVac.Log.DataLogStructure


Namespace DAL.Oracle

    Public Class DbConvocazioneProvider
        Inherits DbProvider
        Implements IConvocazioneProvider

#Region " Costruttore "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " Calendario "

        Public Function GetCalendarioVaccinaleDS(codicePaziente As Integer) As DSCalendarioVaccinale Implements IConvocazioneProvider.GetCalendarioVaccinaleDS

            Dim ds As New DSCalendarioVaccinale()

            '-------------------------

            'SELECT cic_codice, cic_descrizione, cic_data_introduzione, nvl(a.vac_descrizione, b.vac_descrizione) vac_descrizione,
            '       nvl(a.vac_obbligatoria, b.vac_obbligatoria) vac_obbligatoria, tsd_cic_codice, tsd_n_seduta, tsd_eta_seduta, tsd_durata_seduta, 
            '       tsd_intervallo, tsd_intervallo_prossima, nvl(sed_vac_codice,sas_vac_codice) sed_vac_codice, nvl(sed_n_richiamo, sas_n_richiamo) as sed_n_richiamo
            '  FROM t_paz_cicli,
            '       t_ana_tempi_sedute,
            '       t_ana_vaccinazioni_sedute,
            '       t_ana_cicli,
            '       t_ana_associazioni_sedute,
            '       t_ana_vaccinazioni  a,
            '       t_ana_vaccinazioni  b
            ' WHERE pac_paz_codice = 1448248
            '   AND pac_cic_codice = cic_codice
            '   AND cic_codice = tsd_cic_codice

            '   AND tsd_cic_codice = sed_cic_codice(+)
            '   AND tsd_n_seduta = sed_n_seduta(+)

            '   AND tsd_cic_codice = sas_cic_codice(+)
            '   AND tsd_n_seduta = sas_n_seduta(+)

            '   AND sas_vac_codice = a.vac_codice(+)
            '   AND sed_vac_codice = b.vac_codice(+)

            'ORDER BY tsd_cic_codice ASC, tsd_n_seduta ASC

            '-------------------------
            With _DAM.QB
                .NewQuery()
                .AddSelectFields("cic_codice, cic_descrizione, cic_data_introduzione, nvl(a.vac_descrizione,b.vac_descrizione) vac_descrizione",
                                 "nvl(a.vac_obbligatoria, b.vac_obbligatoria) vac_obbligatoria, tsd_cic_codice, tsd_n_seduta, tsd_eta_seduta, tsd_durata_seduta",
                                 "tsd_intervallo, tsd_intervallo_prossima, nvl(sed_vac_codice, sas_vac_codice) sed_vac_codice, nvl(sed_n_richiamo, sas_n_richiamo) sed_n_richiamo")

                .AddTables("t_paz_cicli, t_ana_tempi_sedute, t_ana_vaccinazioni_sedute, t_ana_cicli, t_ana_associazioni_sedute, t_ana_vaccinazioni a, t_ana_vaccinazioni b")

                .AddWhereCondition("pac_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("pac_cic_codice", Comparatori.Uguale, "cic_codice", DataTypes.Join)
                .AddWhereCondition("cic_codice", Comparatori.Uguale, "tsd_cic_codice", DataTypes.Join)

                .AddWhereCondition("tsd_cic_codice", Comparatori.Uguale, "sas_cic_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("tsd_n_seduta", Comparatori.Uguale, "sas_n_seduta", DataTypes.OutJoinLeft)

                .AddWhereCondition("tsd_cic_codice", Comparatori.Uguale, "sed_cic_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("tsd_n_seduta", Comparatori.Uguale, "sed_n_seduta", DataTypes.OutJoinLeft)

                .AddWhereCondition("sas_vac_codice", Comparatori.Uguale, "a.vac_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("sed_vac_codice", Comparatori.Uguale, "b.vac_codice", DataTypes.OutJoinLeft)

                .AddOrderByFields("tsd_cic_codice ASC,tsd_n_seduta ASC")

            End With

            _DAM.BuildDataTable(ds.dtCicSedVac)

            Return ds

        End Function

#End Region

#Region " Exists "

        ''' <summary>
        ''' Restituisce true se esistono convocazioni per il paziente nella data specificata
        ''' </summary>
        Public Function Exists(codicePaziente As Integer, dataConvocazione As Date) As Boolean Implements IConvocazioneProvider.Exists

            Dim count As Int16 = Me.CountConvocazioniData(codicePaziente, dataConvocazione, Nothing)

            Return (count > 0)

        End Function

        Public Function Exists(codicePaziente As Integer) As Boolean Implements IConvocazioneProvider.Exists

            With _DAM.QB

                Me.CreateQueryCountConvocazioni(codicePaziente, _DAM.QB)

            End With

            Dim count As Int16

            Try

                Dim obj As Object = _DAM.ExecScalar()

                If obj Is Nothing OrElse obj Is DBNull.Value Then
                    count = 0
                Else
                    count = Convert.ToInt16(obj)
                End If

            Catch ex As Exception

                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return (count > 0)

        End Function

        ''' <summary>
        ''' Restituisce true se esistono convocazioni su altri consultori per il paziente nella data specificata
        ''' </summary>
        Public Function ExistsConvocazioneAltroConsultorio(codicePaziente As Integer, dataConvocazione As Date, codiceConsultorioCorrente As String) As Boolean Implements IConvocazioneProvider.ExistsConvocazioneAltroConsultorio

            Dim count As Int16 = Me.CountConvocazioniData(codicePaziente, dataConvocazione, codiceConsultorioCorrente)

            Return (count > 0)

        End Function

        ''' <summary>
        ''' Restituisce true se le date di convocazione hanno almeno una vaccinazione in comune. False altrimenti.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataConvocazione1"></param>
        ''' <param name="dataConvocazione2"></param>
        Public Function ExistsStessaVaccinazioneInConvocazioni(codicePaziente As Integer, dataConvocazione1 As Date, dataConvocazione2 As Date) As Boolean Implements IConvocazioneProvider.ExistsStessaVaccinazioneInConvocazioni

            Dim ok As Boolean = False

            Using cmd As New OracleClient.OracleCommand(Queries.Convocazioni.OracleQueries.selCountVaccProgComuni, Me.Connection)

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                cmd.Parameters.AddWithValue("data_cnv_1", dataConvocazione1)
                cmd.Parameters.AddWithValue("data_cnv_2", dataConvocazione2)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If (Not obj Is Nothing AndAlso Not obj Is DBNull.Value) Then ok = True ' ci sono risultati, quindi ci sono vac in comune

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return ok

        End Function

#End Region

#Region " Count "

        ' Restituisce il numero di convocazioni del paziente, con data precedente la data di sospensione specificata
        Public Function CountConvocazioniPrecedentiFineSospensione(codicePaziente As Integer, dataFineSospensione As DateTime) As Integer Implements IConvocazioneProvider.CountConvocazioniPrecedentiFineSospensione

            Dim numeroConvocazioni As Int32 = 0

            With _DAM.QB

                .NewQuery()
                .AddTables("T_CNV_CONVOCAZIONI")
                .AddSelectFields("COUNT(*)")
                .AddWhereCondition("CNV_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("CNV_DATA", Comparatori.Minore, dataFineSospensione, DataTypes.Data)

            End With

            Dim obj As Object = _DAM.ExecScalar()

            If obj Is Nothing OrElse obj Is DBNull.Value Then
                numeroConvocazioni = 0
            Else
                numeroConvocazioni = Convert.ToInt32(obj)
            End If

            Return numeroConvocazioni

        End Function

        Public Function CountConvocazioniConAppuntamento(codicePaziente As Integer, codiceConsultorioCorrente As String, soloConsultorioCorrente As Boolean) As Integer Implements IConvocazioneProvider.CountConvocazioniConAppuntamento

            Dim numeroConvocazioni As Int32 = 0

            With _DAM.QB

                ' Query conteggio convocazioni paziente
                Me.CreateQueryCountConvocazioni(codicePaziente, _DAM.QB)

                ' Filtro data appuntamento impostata
                .AddWhereCondition("CNV_DATA_APPUNTAMENTO", Comparatori.[IsNot], "NULL", DataTypes.Replace)

                ' Filtro su consultorio corrente
                If soloConsultorioCorrente Then
                    .AddWhereCondition("CNV_CNS_CODICE", Comparatori.Uguale, codiceConsultorioCorrente, DataTypes.Stringa)
                End If

            End With

            Dim obj As Object = _DAM.ExecScalar()

            If obj Is Nothing OrElse obj Is DBNull.Value Then
                numeroConvocazioni = 0
            Else
                numeroConvocazioni = Convert.ToInt32(obj)
            End If

            Return numeroConvocazioni

        End Function

        Public Function CountConvocazioniSenzaAppuntamento(codicePaziente As Integer, codiceConsultorioCorrente As String, soloConsultorioCorrente As Boolean) As Integer Implements IConvocazioneProvider.CountConvocazioniSenzaAppuntamento

            Dim numeroConvocazioni As Int32 = 0

            With _DAM.QB

                ' Query conteggio convocazioni paziente
                Me.CreateQueryCountConvocazioni(codicePaziente, _DAM.QB)

                ' Filtro data appuntamento non impostata
                .AddWhereCondition("CNV_DATA_APPUNTAMENTO", Comparatori.Is, "NULL", DataTypes.Replace)

                ' Filtro su consultorio corrente
                If soloConsultorioCorrente Then
                    .AddWhereCondition("CNV_CNS_CODICE", Comparatori.Uguale, codiceConsultorioCorrente, DataTypes.Stringa)
                End If

            End With

            Dim obj As Object = _DAM.ExecScalar()

            If obj Is Nothing OrElse obj Is DBNull.Value Then
                numeroConvocazioni = 0
            Else
                numeroConvocazioni = Convert.ToInt32(obj)
            End If

            Return numeroConvocazioni

        End Function

        Private Function CountConvocazioniData(codicePaziente As Integer, dataConvocazione As Date, codiceConsultorioCorrente As String) As Int16

            With _DAM.QB

                Me.CreateQueryCountConvocazioni(codicePaziente, _DAM.QB)

                .AddWhereCondition("cnv_data", Comparatori.Uguale, dataConvocazione, DataTypes.Data)

                If Not String.IsNullOrEmpty(codiceConsultorioCorrente) Then
                    .AddWhereCondition("cnv_cns_codice", Comparatori.Diverso, codiceConsultorioCorrente, DataTypes.Stringa)
                End If

            End With

            Dim count As Int16

            Try

                Dim obj As Object = _DAM.ExecScalar()

                If obj Is Nothing OrElse obj Is DBNull.Value Then
                    count = 0
                Else
                    count = Convert.ToInt16(obj)
                End If

            Catch ex As Exception

                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return count

        End Function

        Private Sub CreateQueryCountConvocazioni(codicePaziente As Integer, ByRef qb As AbstractQB)

            qb.NewQuery()

            qb.AddSelectFields("count(*)")
            qb.AddTables("t_cnv_convocazioni")
            qb.AddWhereCondition("cnv_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)

        End Sub

#End Region

#Region " Select "

        Public Function GetFromKey(codicePaziente As Integer, dataConvocazione As Date) As DataTable Implements IConvocazioneProvider.GetFromKey

            _DT.Clear()

            With _DAM.QB

                .NewQuery(True)
                .AddTables("t_cnv_convocazioni")
                .AddSelectFields("*")
                .AddWhereCondition("to_date('" & dataConvocazione.ToString("d") & "','DD/MM/YYYY')", Comparatori.Uguale, "cnv_data", DataTypes.Join)
                .AddWhereCondition("cnv_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Join)

            End With

            Try
                _DAM.BuildDataTable(_DT)

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return _DT

        End Function

        Public Function GetDtConvocazioniPaziente(codicePaziente As Integer) As DataTable Implements IConvocazioneProvider.GetDtConvocazioniPaziente

            Dim dtConvocazioni As New DataTable()

            With _DAM.QB

                .NewQuery()

                .AddSelectFields(.FC.IsNull("MAX(cnc_n_sollecito)", 0, DataTypes.Numero))
                .AddTables("t_cnv_cicli")
                .AddWhereCondition("cnc_cnv_data", Comparatori.Uguale, "CNV_DATA", DataTypes.Replace)
                .AddWhereCondition("cnc_cnv_paz_codice", Comparatori.Uguale, "CNV_PAZ_CODICE", DataTypes.Replace)

                Dim querySollecitiVaccinazioni As String = .GetSelect()

                .NewQuery(False, False)

                .AddTables("t_cnv_convocazioni", "t_ana_consultori", "t_cnv_cicli", "t_ana_cicli")
                .AddTables("t_vac_programmate", "t_ana_vaccinazioni", "t_ana_associazioni", "t_ana_ambulatori")

                .AddSelectFields("cnv_data", "cnv_data_invio", "cnv_data_appuntamento", "cnv_durata_appuntamento", "cnv_ute_id", "cnv_tipo_appuntamento")
                .AddSelectFields("cnv_cns_codice", "cns_descrizione", "vpr_n_seduta Num_Sed_Bil", "vpr_cic_codice", "cic_descrizione", "amb_descrizione")
                .AddSelectFields("vpr_vac_codice", "vac_descrizione Desc_Vac_Bil", "vpr_ass_codice", "ass_descrizione Desc_Ass_Mal", "(" + querySollecitiVaccinazioni + ") SOLLECITO", "'V' TIPO_CNV")

                .AddWhereCondition("cnv_amb_codice", Comparatori.Uguale, "amb_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("cnv_cns_codice", Comparatori.Uguale, "cns_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("cnv_paz_codice", Comparatori.Uguale, "vpr_paz_codice", DataTypes.Join)
                .AddWhereCondition("cnv_data", Comparatori.Uguale, "vpr_cnv_data", DataTypes.Join)
                .AddWhereCondition("vpr_paz_codice", Comparatori.Uguale, "cnc_cnv_paz_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("vpr_cnv_data", Comparatori.Uguale, "cnc_cnv_data", DataTypes.OutJoinLeft)
                .AddWhereCondition("vpr_vac_codice", Comparatori.Uguale, "vac_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("vpr_ass_codice", Comparatori.Uguale, "ass_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("vpr_n_seduta", Comparatori.Uguale, "cnc_sed_n_seduta", DataTypes.OutJoinLeft)
                .AddWhereCondition("vpr_cic_codice", Comparatori.Uguale, "cnc_cic_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("cnc_cic_codice", Comparatori.Uguale, "cic_codice", DataTypes.OutJoinLeft)

                .AddWhereCondition("cnv_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)

                .AddUnion(.GetSelect())

                .NewQuery(False, False)

                .AddTables("t_cnv_convocazioni", "t_ana_consultori", "t_ana_ambulatori", "t_bil_programmati", "t_ana_bilanci", "t_ana_malattie")

                .AddSelectFields("cnv_data", "cnv_data_invio", "cnv_data_appuntamento", "cnv_durata_appuntamento", "cnv_ute_id", "cnv_tipo_appuntamento")
                .AddSelectFields("cnv_cns_codice", "cns_descrizione", "bil_numero Num_Sed_Bil", "NULL vpr_cic_codice", "NULL cic_descrizione", "amb_descrizione")
                .AddSelectFields("NULL vpr_vac_codice", "bil_descrizione Desc_Vac_Bil", "NULL vpr_ass_codice", "mal_descrizione Desc_Ass_Mal", "(" + querySollecitiVaccinazioni + ") SOLLECITO", "'B' TIPO_CNV")

                .AddWhereCondition("cnv_cns_codice", Comparatori.Uguale, "cns_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("cnv_amb_codice", Comparatori.Uguale, "amb_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("cnv_data", Comparatori.Uguale, "bip_cnv_data", DataTypes.Join)
                .AddWhereCondition("cnv_paz_codice", Comparatori.Uguale, "bip_paz_codice", DataTypes.Join)
                .AddWhereCondition("bip_mal_codice", Comparatori.Uguale, "bil_mal_codice", DataTypes.Join)
                .AddWhereCondition("bil_mal_codice", Comparatori.Uguale, "mal_codice", DataTypes.Join)
                .AddWhereCondition("bip_bil_numero", Comparatori.Uguale, "bil_numero", DataTypes.Join)
                .AddWhereCondition("bip_stato", Comparatori.Uguale, Constants.StatiBilancio.UNEXECUTED, DataTypes.Stringa)

                .AddWhereCondition("cnv_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)

                .AddOrderByFields("cnv_data", "tipo_cnv desc", "vpr_cic_codice", "Num_Sed_Bil", "Desc_Ass_Mal", "Desc_Vac_Bil")

                .AddUnion(.GetSelect())

            End With

            _DAM.BuildDataTable(dtConvocazioni)

            Return dtConvocazioni

        End Function

        Public Function GetConvocazioniPaziente(codicePaziente As String, appuntamenti As Boolean?) As ICollection(Of Entities.Convocazione) Implements IConvocazioneProvider.GetConvocazioniPaziente

            Return Me.GetConvocazioniPaziente(codicePaziente, Nothing, appuntamenti)

        End Function

        Public Function GetConvocazionePaziente(codicePaziente As String, dataConvocazione As DateTime) As Entities.Convocazione Implements IConvocazioneProvider.GetConvocazionePaziente

            Return Me.GetConvocazioniPaziente(codicePaziente, dataConvocazione, Nothing).FirstOrDefault()

        End Function

        Public Function GetIdConvocazione(pazCodice As Long, dataConv As DateTime) As String Implements IConvocazioneProvider.GetIdConvocazione

            Dim ownConnection As Boolean = False
            Dim cnv_Id_conv As String = String.Empty

            Dim query As String = "SELECT CNV_ID_CONVOCAZIONE " +
                "FROM T_CNV_CONVOCAZIONI " +
                "WHERE CNV_PAZ_CODICE = :CNV_PAZ_CODICE AND CNV_DATA = :CNV_DATA "
            Try
                Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                    cmd.Parameters.AddWithValue("CNV_PAZ_CODICE", pazCodice)
                    cmd.Parameters.AddWithValue("CNV_DATA", dataConv)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()

                        Dim cnv_id_convocazione As Integer = _context.GetOrdinal("CNV_ID_CONVOCAZIONE")

                        If _context.Read() Then

                            cnv_Id_conv = _context.GetStringOrDefault(cnv_id_convocazione)

                        End If
                    End Using

                End Using
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return cnv_Id_conv
        End Function


        Private Function GetConvocazioniPaziente(codicePaziente As String, dataConvocazione As DateTime?, appuntamenti As Boolean?) As ICollection(Of Entities.Convocazione)

            Dim convocazioni As New List(Of Entities.Convocazione)()

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try
                If dataConvocazione.HasValue Then

                    cmd = New OracleClient.OracleCommand(Queries.Convocazioni.OracleQueries.selDatiCnv, Me.Connection)

                    cmd.Parameters.AddWithValue("cnv_data", dataConvocazione)

                Else

                    cmd = New OracleClient.OracleCommand(Queries.Convocazioni.OracleQueries.selConvocazioni, Me.Connection)

                    If appuntamenti.HasValue Then
                        cmd.Parameters.AddWithValue("flag_appuntamento", IIf(appuntamenti.Value, "S", "N"))
                    Else
                        cmd.Parameters.AddWithValue("flag_appuntamento", DBNull.Value)
                    End If

                End If

                cmd.Parameters.AddWithValue("cnv_paz_codice", codicePaziente)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using reader As OracleClient.OracleDataReader = cmd.ExecuteReader()

                    If Not reader Is Nothing Then

                        Dim cnv_paz_codice_ordinal As Integer = reader.GetOrdinal("cnv_paz_codice")
                        Dim cnv_paz_codice_old_ordinal As Integer = reader.GetOrdinal("cnv_paz_codice_old")
                        Dim cnv_data_ordinal As Integer = reader.GetOrdinal("cnv_data")
                        Dim cnv_cns_codice_ordinal As Integer = reader.GetOrdinal("cnv_cns_codice")
                        Dim cnv_data_appuntamento_ordinal As Integer = reader.GetOrdinal("cnv_data_appuntamento")
                        Dim cnv_tipo_appuntamento_ordinal As Integer = reader.GetOrdinal("cnv_tipo_appuntamento")
                        Dim cnv_durata_appuntamento_ordinal As Integer = reader.GetOrdinal("cnv_durata_appuntamento")
                        Dim cnv_primo_appuntamento_ordinal As Integer = reader.GetOrdinal("cnv_primo_appuntamento")
                        Dim cnv_ute_id_ordinal As Integer = reader.GetOrdinal("cnv_ute_id")
                        Dim cnv_data_invio_ordinal As Integer = reader.GetOrdinal("cnv_data_invio")
                        Dim cnv_eta_pomeriggio_ordinal As Integer = reader.GetOrdinal("cnv_eta_pomeriggio")
                        Dim cnv_rinvio_ordinal As Integer = reader.GetOrdinal("cnv_rinvio")
                        Dim cnv_campagna_ordinal As Integer = reader.GetOrdinal("cnv_campagna")
                        Dim cnv_amb_codice_ordinal As Integer = reader.GetOrdinal("cnv_amb_codice")
                        Dim cnv_data_inserimento_ordinal As Integer = reader.GetOrdinal("cnv_data_inserimento")
                        Dim cnv_ute_id_inserimento_ordinal As Integer = reader.GetOrdinal("cnv_ute_id_inserimento")

                        While reader.Read()

                            Dim convocazione As New Convocazione()

                            convocazione.Paz_codice = reader.GetInt32(cnv_paz_codice_ordinal)
                            convocazione.Paz_codice_old = reader.GetInt32OrDefault(cnv_paz_codice_old_ordinal)
                            convocazione.Data_CNV = reader.GetDateTime(cnv_data_ordinal)
                            convocazione.Cns_Codice = reader.GetString(cnv_cns_codice_ordinal)
                            convocazione.DataAppuntamento = reader.GetDateTimeOrDefault(cnv_data_appuntamento_ordinal)
                            convocazione.TipoAppuntamento = reader.GetStringOrDefault(cnv_tipo_appuntamento_ordinal)
                            convocazione.Durata_Appuntamento = reader.GetInt32OrDefault(cnv_durata_appuntamento_ordinal)
                            convocazione.DataPrimoAppuntamento = reader.GetDateTimeOrDefault(cnv_primo_appuntamento_ordinal)
                            convocazione.IdUtente = reader.GetInt32OrDefault(cnv_ute_id_ordinal)
                            convocazione.DataInvio = reader.GetDateTimeOrDefault(cnv_data_invio_ordinal)
                            convocazione.EtaPomeriggio = reader.GetStringOrDefault(cnv_eta_pomeriggio_ordinal)
                            convocazione.Rinvio = reader.GetStringOrDefault(cnv_rinvio_ordinal)
                            convocazione.CampagnaVaccinale = reader.GetStringOrDefault(cnv_campagna_ordinal)
                            convocazione.CodiceAmbulatorio = reader.GetInt32OrDefault(cnv_amb_codice_ordinal)
                            convocazione.DataInserimento = reader.GetNullableDateTimeOrDefault(cnv_data_inserimento_ordinal)
                            convocazione.IdUtenteInserimento = reader.GetNullableInt64OrDefault(cnv_ute_id_inserimento_ordinal)

                            convocazioni.Add(convocazione)

                        End While

                    End If

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
                If Not cmd Is Nothing Then cmd.Dispose()
            End Try

            Return convocazioni

        End Function

        Public Function GetConvocazioniPaziente(codicePaziente As String) As System.Collections.Generic.ICollection(Of Entities.Convocazione) Implements IConvocazioneProvider.GetConvocazioniPaziente

            Return Me.GetConvocazioniPaziente(codicePaziente, Nothing)

        End Function

        Public Function GetMaxSollecitoVaccinazioni(codicePaziente As Integer, dataConvocazione As Date) As Int32 Implements IConvocazioneProvider.GetMaxSollecitoVaccinazioni

            Dim result As Int32 = 0

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("MAX(CNC_N_SOLLECITO) SOLLECITO")
                .AddTables("T_CNV_CICLI")
                .AddWhereCondition("CNC_CNV_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("CNC_CNV_DATA", Comparatori.Uguale, dataConvocazione, DataTypes.Data)
            End With

            Try
                Using dr As IDataReader = _DAM.BuildDataReader()

                    If dr.Read() Then

                        Dim pos_sol As Integer = dr.GetOrdinal("SOLLECITO")

                        If Not dr.IsDBNull(pos_sol) Then
                            result = CType(dr(pos_sol), Int32)
                        End If

                    End If

                End Using

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return result

        End Function

        Public Function TerminePerentorio(codicePaziente As Int32, dataConvocazione As Nullable(Of Date), parametroNumSol As Integer) As Boolean Implements IConvocazioneProvider.TerminePerentorio

            Dim obj As Object
            Dim result As Boolean = False

            With _DAM.QB

                .NewQuery()
                .AddSelectFields("1")
                .AddTables("V_ANA_ASS_VACC_SEDUTE, T_ANA_VACCINAZIONI")
                .AddWhereCondition("SAS_VAC_CODICE", Comparatori.Uguale, "VAC_CODICE", DataTypes.Join)
                .AddWhereCondition("VAC_OBBLIGATORIA", Comparatori.Uguale, "A", DataTypes.Stringa)
                .AddWhereCondition("SAS_CIC_CODICE", Comparatori.Uguale, "CNC_CIC_CODICE", DataTypes.Replace)
                .AddWhereCondition("SAS_N_SEDUTA", Comparatori.Uguale, "CNC_SED_N_SEDUTA", DataTypes.Replace)

                .IsDistinct = True

                Dim queryCicloSedutaObbligatoria As String = _DAM.QB.GetSelect()

                .NewQuery(False, False)
                .AddSelectFields("1")
                .AddTables("T_CNV_CICLI, T_ANA_TEMPI_SEDUTE")
                .AddWhereCondition("CNC_CIC_CODICE", Comparatori.Uguale, "TSD_CIC_CODICE", DataTypes.Join)
                .AddWhereCondition("CNC_SED_N_SEDUTA", Comparatori.Uguale, "TSD_N_SEDUTA", DataTypes.Join)
                .AddWhereCondition("CNC_CNV_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                If Not dataConvocazione Is Nothing Then
                    .AddWhereCondition("CNC_CNV_DATA", Comparatori.Uguale, dataConvocazione, DataTypes.Data)
                End If
                .AddWhereCondition(.FC.Switch(.FC.IsNull("TSD_NUM_SOLLECITI", 0, DataTypes.Numero), 0, parametroNumSol, "TSD_NUM_SOLLECITI"), Comparatori.Minore, "CNC_N_SOLLECITO", DataTypes.Replace)
                .AddWhereCondition(.FC.IsNull("TSD_NUM_SOLLECITI_RAC", 0, DataTypes.Numero), Comparatori.Uguale, 0, DataTypes.Numero)
                .AddWhereCondition("", Comparatori.Exist, "(" + queryCicloSedutaObbligatoria + ")", DataTypes.Replace)

                .IsDistinct = True

            End With

            Try
                obj = _DAM.ExecScalar()

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            If Not obj Is Nothing And Not obj Is DBNull.Value Then
                result = True
            End If

            Return result

        End Function

        Public Function GetCnvFromInterval(codicePaziente As Integer, firstDate As Date, secondDate As Date) As DataTable Implements IConvocazioneProvider.GetCnvFromInterval

            _DT.Clear()

            With _DAM.QB

                .NewQuery(True)
                .AddTables("t_cnv_convocazioni,t_vac_programmate,t_ana_vaccinazioni")
                .AddSelectFields("DISTINCT(cnv_data),vac_obbligatoria")
                .AddWhereCondition("to_date('" & firstDate.ToString("d") & "','DD/MM/YYYY')", Comparatori.MinoreUguale, "cnv_data", DataTypes.Join)
                .AddWhereCondition("cnv_data", Comparatori.MinoreUguale, "to_date('" & secondDate.ToString("d") & "','DD/MM/YYYY')", DataTypes.Join)
                .AddWhereCondition("cnv_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Join)
                .AddWhereCondition("vpr_paz_codice", Comparatori.Uguale, "cnv_paz_codice", DataTypes.OutJoinRight)
                .AddWhereCondition("vpr_cnv_data", Comparatori.Uguale, "cnv_data", DataTypes.OutJoinRight)
                .AddWhereCondition("vac_codice", Comparatori.Uguale, "vpr_vac_codice", DataTypes.OutJoinRight)
                ' da verificare l'ordinamento dei valori
                .AddOrderByFields("vac_obbligatoria,cnv_data ASC")

            End With

            Try
                _DAM.BuildDataTable(_DT)

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return _DT

        End Function

        Public Function GetDataInvio(codicePaziente As Integer, dataConvocazione As Date) As Date Implements IConvocazioneProvider.GetDataInvio

            Dim result As Date = Date.MinValue

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("CNV_DATA_INVIO")
                .AddTables("T_CNV_CONVOCAZIONI")
                .AddWhereCondition("CNV_DATA", Comparatori.Uguale, dataConvocazione, DataTypes.Data)
                .AddWhereCondition("CNV_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
            End With

            Try

                Dim obj As Object = _DAM.ExecScalar()

                If Not obj Is DBNull.Value Then
                    result = obj
                End If

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return result

        End Function

        Public Function GetDurataAppuntamento(codicePaziente As Integer, dataConvocazione As Date) As Int32 Implements IConvocazioneProvider.GetDurataAppuntamento

            Dim result As Int32 = 0

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("CNV_DURATA_APPUNTAMENTO")
                .AddTables("T_CNV_CONVOCAZIONI")
                .AddWhereCondition("CNV_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("CNV_DATA", Comparatori.Uguale, dataConvocazione, DataTypes.Data)
            End With

            Try
                Dim obj As Object = _DAM.ExecScalar()
                If Not obj Is DBNull.Value Then
                    result = obj
                End If

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return result

        End Function

        ''' <summary>
        ''' Restituisce una lista di oggetti contenenti le informazioni su convocazioni e vaccinazioni del paziente.
        ''' </summary>
        Public Function GetConvocazioniVaccinazioniPaziente(codicePaziente As Integer) As List(Of Entities.ConvocazioneVaccinazione) Implements IConvocazioneProvider.GetConvocazioniVaccinazioniPaziente

            Dim listCnvVacPaziente As New List(Of Entities.ConvocazioneVaccinazione)

            Using cmd As OracleCommand = New OracleClient.OracleCommand(Queries.Convocazioni.OracleQueries.selConvocazioniVaccinazioniPaziente, Me.Connection)

                cmd.Parameters.AddWithValue("codPaziente", codicePaziente)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim cnv_paz_codice As Integer = idr.GetOrdinal("cnv_paz_codice")
                            Dim cnv_data As Integer = idr.GetOrdinal("cnv_data")
                            Dim cnv_cns_codice As Integer = idr.GetOrdinal("cnv_cns_codice")
                            Dim cns_descrizione As Integer = idr.GetOrdinal("cns_descrizione")
                            Dim cnv_data_appuntamento As Integer = idr.GetOrdinal("cnv_data_appuntamento")
                            Dim cnv_amb_codice As Integer = idr.GetOrdinal("cnv_amb_codice")
                            Dim amb_descrizione As Integer = idr.GetOrdinal("amb_descrizione")
                            Dim vpr_vac_codice As Integer = idr.GetOrdinal("vpr_vac_codice")
                            Dim vac_descrizione As Integer = idr.GetOrdinal("vac_descrizione")
                            Dim vpr_n_richiamo As Integer = idr.GetOrdinal("vpr_n_richiamo")
                            Dim vpr_paz_codice_old As Integer = idr.GetOrdinal("vpr_paz_codice_old")

                            Dim cnvVac As Entities.ConvocazioneVaccinazione = Nothing

                            While idr.Read()

                                cnvVac = New Entities.ConvocazioneVaccinazione()

                                cnvVac.CodicePaziente = idr.GetDecimal(cnv_paz_codice)
                                cnvVac.CodicePazienteAlias = idr.GetNullableInt32OrDefault(vpr_paz_codice_old)
                                cnvVac.DataConvocazione = idr.GetDateTimeOrDefault(cnv_data)
                                cnvVac.CodiceConsultorio = idr.GetStringOrDefault(cnv_cns_codice)
                                cnvVac.DataAppuntamento = idr.GetNullableDateTimeOrDefault(cnv_data_appuntamento)
                                cnvVac.DescrizioneConsultorio = idr.GetStringOrDefault(cns_descrizione)
                                cnvVac.CodiceAmbulatorio = idr.GetInt32OrDefault(cnv_amb_codice)
                                cnvVac.DescrizioneAmbulatorio = idr.GetStringOrDefault(amb_descrizione)
                                cnvVac.CodiceVaccinazione = idr.GetStringOrDefault(vpr_vac_codice)
                                cnvVac.DescrizioneVaccinazione = idr.GetStringOrDefault(vac_descrizione)
                                cnvVac.NumeroRichiamo = idr.GetInt32(vpr_n_richiamo)

                                listCnvVacPaziente.Add(cnvVac)

                            End While

                        End If

                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listCnvVacPaziente

        End Function

        ''' <summary>
        ''' Restituisce true se la convocazione specificata ha il flag InCampagna valorizzato a "S"
        ''' </summary>
        Public Function IsCnvCampagna(codicePaziente As Integer, dataConvocazione As Date) As Boolean Implements IConvocazioneProvider.IsCnvCampagna

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("CNV_CAMPAGNA")
                .AddTables("T_CNV_CONVOCAZIONI")
                .AddWhereCondition("CNV_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("CNV_DATA", Comparatori.Uguale, dataConvocazione, DataTypes.Data)
            End With

            Dim inCampagna As Object = _DAM.ExecScalar()

            If inCampagna Is Nothing OrElse inCampagna Is DBNull.Value Then
                Return False
            End If

            Return (inCampagna.ToString() = "S")

        End Function

        ''' <summary>
        ''' Restituisce una lista con le date di convocazione del paziente che sono precedenti rispetto a quella eventualmente specificata
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="maxDataConvocazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDateConvocazioniPaziente(codicePaziente As Int64, maxDataConvocazione As DateTime?) As IEnumerable(Of Date) Implements IConvocazioneProvider.GetDateConvocazioniPaziente

            Dim listDateConvocazione As List(Of DateTime) = Nothing

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("CNV_DATA")
                .AddTables("T_CNV_CONVOCAZIONI")
                .AddWhereCondition("CNV_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                If maxDataConvocazione.HasValue Then .AddWhereCondition("CNV_DATA", Comparatori.Minore, maxDataConvocazione, DataTypes.Data)
                .AddOrderByFields("CNV_DATA ASC")
            End With

            Using idr As IDataReader = _DAM.BuildDataReader()

                If Not idr Is Nothing Then

                    Dim dataConvocazione As DateTime = DateTime.MinValue

                    Dim cnv_data As Integer = idr.GetOrdinal("CNV_DATA")

                    listDateConvocazione = New List(Of DateTime)()

                    While idr.Read()

                        dataConvocazione = idr.GetDateTimeOrDefault(cnv_data)

                        If dataConvocazione > DateTime.MinValue Then

                            listDateConvocazione.Add(dataConvocazione)

                        End If

                    End While

                End If

            End Using

            Return listDateConvocazione.AsEnumerable()

        End Function

        Public Function GetCicliConvocazioniPaziente(codicePaziente As Integer) As IEnumerable(Of CicloConvocazione) Implements IConvocazioneProvider.GetCicliConvocazioniPaziente

            Dim list As New List(Of CicloConvocazione)()

            Using cmd As New OracleClient.OracleCommand(Queries.Convocazioni.OracleQueries.selCicliConvocazioniPaziente, Me.Connection)

                cmd.Parameters.AddWithValue("cnc_cnv_paz_codice", codicePaziente)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As OracleClient.OracleDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim dataConvocazioneOrdinal As Integer = idr.GetOrdinal("CNC_CNV_DATA")
                            Dim codiceCicloConvocazioneOrdinal As Integer = idr.GetOrdinal("CNC_CIC_CODICE")
                            Dim codicePazientePrecedenteCicloConvocazioneOrdinal As Integer = idr.GetOrdinal("CNC_CNV_PAZ_CODICE_OLD")
                            Dim numeroSedutaCicloConvocazioneOrdinal As Integer = idr.GetOrdinal("CNC_SED_N_SEDUTA")
                            Dim flagGiorniPosticipoCicloConvocazioneOrdinal As Integer = idr.GetOrdinal("CNC_FLAG_GIORNI_POSTICIPO")
                            Dim flagPosticipoSedutaCicloConvocazioneOrdinal As Integer = idr.GetOrdinal("CNC_FLAG_POSTICIPO_SEDUTA")
                            Dim numeroSollecitoCicloConvocazioneOrdinal As Integer = idr.GetOrdinal("CNC_N_SOLLECITO")
                            Dim dataInvioSollecitoCicloConvocazioneOrdinal As Integer = idr.GetOrdinal("CNC_DATA_INVIO_SOLLECITO")
                            Dim cnc_data_inserimento As Integer = idr.GetOrdinal("CNC_DATA_INSERIMENTO")
                            Dim cnc_ute_id_inserimento As Integer = idr.GetOrdinal("CNC_UTE_ID_INSERIMENTO")

                            While idr.Read()

                                Dim item As New CicloConvocazione()
                                item.CodicePaziente = codicePaziente
                                item.DataConvocazione = idr.GetDateTime(dataConvocazioneOrdinal)
                                item.CodiceCiclo = idr.GetString(codiceCicloConvocazioneOrdinal)
                                item.NumeroSeduta = idr.GetInt32(numeroSedutaCicloConvocazioneOrdinal)
                                item.CodicePazientePrecedente = idr.GetInt32OrDefault(codicePazientePrecedenteCicloConvocazioneOrdinal)
                                item.FlagGiorniPosticipo = idr.GetStringOrDefault(flagGiorniPosticipoCicloConvocazioneOrdinal)
                                item.FlagPosticipoSeduta = idr.GetStringOrDefault(flagPosticipoSedutaCicloConvocazioneOrdinal)
                                item.NumeroSollecito = idr.GetInt32OrDefault(numeroSollecitoCicloConvocazioneOrdinal)
                                item.DataInvioSollecito = idr.GetDateTimeOrDefault(dataInvioSollecitoCicloConvocazioneOrdinal)
                                item.DataInserimento = idr.GetNullableDateTimeOrDefault(cnc_data_inserimento)
                                item.IdUtenteInserimento = idr.GetNullableInt64OrDefault(cnc_ute_id_inserimento)

                                list.Add(item)

                            End While

                        End If
                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list.AsEnumerable()

        End Function

        Public Function GetRitardiCicliConvocazioniPaziente(codicePaziente As Integer) As IEnumerable(Of RitardoCicloConvocazione) Implements IConvocazioneProvider.GetRitardiCicliConvocazioniPaziente

            Dim ritardoCicloConvocazioneList As New List(Of RitardoCicloConvocazione)

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try
                cmd = New OracleClient.OracleCommand(Queries.Convocazioni.OracleQueries.selRitardiCicliConvocazioniPaziente, Me.Connection)

                cmd.Parameters.AddWithValue("pri_paz_codice", codicePaziente)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using idr As OracleClient.OracleDataReader = cmd.ExecuteReader()

                    If Not idr Is Nothing Then

                        Dim dataConvocazioneOrdinal As Integer = idr.GetOrdinal("PRI_CNV_DATA")
                        Dim numeroSedutaCicloConvocazioneOrdinal As Integer = idr.GetOrdinal("PRI_SED_N_SEDUTA")
                        Dim codiceCicloConvocazioneOrdinal As Integer = idr.GetOrdinal("PRI_CIC_CODICE")
                        Dim codicePazientePrecedenteRitardoConvocazioneOrdinal As Integer = idr.GetOrdinal("PRI_PAZ_CODICE_OLD")
                        Dim dataInvioRitardoConvocazioneOrdinal As Integer = idr.GetOrdinal("PRI_DATA_INVIO1")
                        Dim dataAppuntamento1RitardoConvocazioneOrdinal As Integer = idr.GetOrdinal("PRI_DATA_APPUNTAMENTO1")
                        Dim dataAppuntamento2RitardoConvocazioneOrdinal As Integer = idr.GetOrdinal("PRI_DATA_APPUNTAMENTO2")
                        Dim dataAppuntamento3RitardoConvocazioneOrdinal As Integer = idr.GetOrdinal("PRI_DATA_APPUNTAMENTO3")
                        Dim dataAppuntamento4RitardoConvocazioneOrdinal As Integer = idr.GetOrdinal("PRI_DATA_APPUNTAMENTO4")
                        Dim noteRitardoConvocazioneOrdinal As Integer = idr.GetOrdinal("PRI_NOTE")

                        While idr.Read()

                            Dim ritardoCicloConvocazione As New RitardoCicloConvocazione()
                            ritardoCicloConvocazione.CodicePaziente = codicePaziente
                            ritardoCicloConvocazione.DataConvocazione = idr.GetDateTime(dataConvocazioneOrdinal)
                            ritardoCicloConvocazione.CodiceCiclo = idr.GetString(codiceCicloConvocazioneOrdinal)
                            ritardoCicloConvocazione.NumeroSeduta = idr.GetInt32(numeroSedutaCicloConvocazioneOrdinal)
                            ritardoCicloConvocazione.CodicePazientePrecedente = idr.GetInt32OrDefault(codicePazientePrecedenteRitardoConvocazioneOrdinal)
                            ritardoCicloConvocazione.DataInvio = idr.GetDateTimeOrDefault(dataInvioRitardoConvocazioneOrdinal)
                            ritardoCicloConvocazione.DataAppuntamento1 = idr.GetDateTimeOrDefault(dataAppuntamento1RitardoConvocazioneOrdinal)
                            ritardoCicloConvocazione.DataAppuntamento2 = idr.GetDateTimeOrDefault(dataAppuntamento2RitardoConvocazioneOrdinal)
                            ritardoCicloConvocazione.DataAppuntamento3 = idr.GetDateTimeOrDefault(dataAppuntamento3RitardoConvocazioneOrdinal)
                            ritardoCicloConvocazione.DataAppuntamento4 = idr.GetDateTimeOrDefault(dataAppuntamento4RitardoConvocazioneOrdinal)
                            ritardoCicloConvocazione.Note = idr.GetStringOrDefault(noteRitardoConvocazioneOrdinal)

                            ritardoCicloConvocazioneList.Add(ritardoCicloConvocazione)

                        End While

                    End If

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
                If Not cmd Is Nothing Then cmd.Dispose()
            End Try

            Return ritardoCicloConvocazioneList.AsEnumerable()

        End Function

#End Region

#Region " Delete "

        ''' <summary>
        ''' Cancellazione dati sulla programmazione relativa al paziente specificato. 
        ''' Verranno eliminati, nell'ordine:
        ''' - solleciti di bilancio (se cancellaBilanciAssociati = true)
        ''' - bilanci (se cancellaBilanciAssociati = true)
        ''' - convocazioni (vaccinazione programmate e cicli in CASCADE)
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        Public Function Delete(codicePaziente As Integer, cancellaBilanciAssociati As Boolean) As Boolean Implements IConvocazioneProvider.Delete

            Return Me.Delete(codicePaziente, Nothing, cancellaBilanciAssociati)

        End Function

        ''' <summary>
        ''' Cancellazione dati sulla programmazione relativa al paziente e alla convocazione specificati. 
        ''' Verranno eliminati, nell'ordine:
        ''' - solleciti di bilancio (cancellaBilanciAssociati = true)
        ''' - bilanci (cancellaBilanciAssociati = true)
        ''' - convocazioni (vaccinazione programmate e cicli in CASCADE)
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        Public Function Delete(codicePaziente As Integer, dataConvocazione As Nullable(Of DateTime), cancellaBilanciAssociati As Boolean) As Boolean Implements IConvocazioneProvider.Delete

            Dim deleted As Boolean = False

            Try
                Dim transactionOptions As New Transactions.TransactionOptions()
                transactionOptions.IsolationLevel = Transactions.IsolationLevel.ReadCommitted

                Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, transactionOptions)

                    If cancellaBilanciAssociati Then

                        ' --- t_bil_solleciti --- '
                        If Me.DeleteSollecitiBilancioConvocazione(codicePaziente, dataConvocazione) > 0 Then
                            deleted = True
                        End If

                        ' --- t_bil_programmati --- '
                        If Me.DeleteBilancioConvocazione(codicePaziente, dataConvocazione) > 0 Then
                            deleted = True
                        End If

                    End If

                    ' --- t_cnv_convocazioni --- '
                    If Me.DeleteConvocazione(codicePaziente, dataConvocazione) > 0 Then
                        deleted = True
                    End If

                    transactionScope.Complete()

                End Using

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return deleted

        End Function

        ''' <summary>
        ''' Cancellazione dei solleciti relativi al bilancio associato alla convocazione specificata.
        ''' Se la data di convocazione è Nothing, cancella tutti i solleciti di bilancio del paziente.
        ''' Restituisce il numero dei solleciti eliminati.
        ''' </summary>
        Public Function DeleteSollecitiBilancioConvocazione(codicePaziente As Integer, dataConvocazione As DateTime?) As Integer Implements IConvocazioneProvider.DeleteSollecitiBilancioConvocazione

            With _DAM.QB

                .NewQuery()
                .AddSelectFields("id")
                .AddTables("t_bil_programmati")
                .AddWhereCondition("bip_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                If dataConvocazione.HasValue Then .AddWhereCondition("bip_cnv_data", Comparatori.Uguale, dataConvocazione.Value, DataTypes.Data)

                Dim query As String = String.Format("({0})", .GetSelect())

                .NewQuery(False, False)
                .AddTables("t_bil_solleciti")
                .AddWhereCondition("bis_bip_id", Comparatori.In, query, DataTypes.Replace)

            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Delete)

        End Function

        ''' <summary>
        ''' Cancellazione del bilancio associato alla convocazione specificata.
        ''' Se la data di convocazione è Nothing, cancella tutti i bilanci del paziente.
        ''' Restituisce il numero di bilanci eliminati.
        ''' </summary>
        Public Function DeleteBilancioConvocazione(codicePaziente As Integer, dataConvocazione As DateTime?) As Integer Implements IConvocazioneProvider.DeleteBilancioConvocazione

            With _DAM.QB
                .NewQuery()
                .AddTables("t_bil_programmati")
                .AddWhereCondition("bip_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                If dataConvocazione.HasValue Then .AddWhereCondition("bip_cnv_data", Comparatori.Uguale, dataConvocazione.Value, DataTypes.Data)
            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Delete)

        End Function

        ''' <summary>
        ''' Cancellazione della convocazione specificata.
        ''' Se la data di convocazione è Nothing, cancella tutte le convocazioni del paziente.
        ''' Restituisce il numero di convocazioni eliminate.
        ''' </summary>
        Public Function DeleteConvocazione(codicePaziente As Integer, dataConvocazione As DateTime?) As Integer Implements IConvocazioneProvider.DeleteConvocazione

            With _DAM.QB
                .NewQuery()
                .AddTables("t_cnv_convocazioni")
                .AddWhereCondition("cnv_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                If dataConvocazione.HasValue Then .AddWhereCondition("cnv_data", Comparatori.Uguale, dataConvocazione, DataTypes.Data)
            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Delete)

        End Function

        Public Function DeleteEmpty(codicePaziente As Integer, dataConvocazione As Nullable(Of DateTime)) As Boolean Implements IConvocazioneProvider.DeleteEmpty

            Dim count As Integer = 0

            With _DAM.QB

                .NewQuery(True)
                .AddTables("t_bil_programmati")
                .AddSelectFields("1")
                .AddWhereCondition("bip_paz_codice", Comparatori.Uguale, "cnv_paz_codice", DataTypes.Join)
                .AddWhereCondition("bip_cnv_data", Comparatori.Uguale, "cnv_data", DataTypes.Join)
                .AddWhereCondition("bip_stato", Comparatori.Uguale, Constants.StatiBilancio.UNEXECUTED, DataTypes.Stringa)

                Dim qrBilProgr As String = .GetSelect()

                .NewQuery(False, False)
                .AddTables("t_vac_eseguite")
                .AddSelectFields("1")
                .AddWhereCondition("vpr_paz_codice", Comparatori.Uguale, "ves_paz_codice", DataTypes.Join)
                .AddWhereCondition("vpr_cnv_data", Comparatori.Uguale, "ves_cnv_data", DataTypes.Join)
                .AddWhereCondition("vpr_vac_codice", Comparatori.Uguale, "ves_vac_codice", DataTypes.Join)
                .AddWhereCondition("vpr_n_richiamo", Comparatori.Uguale, "ves_n_richiamo", DataTypes.Join)

                Dim qrVacProgrEseguite As String = .GetSelect()

                .NewQuery(False, False)
                .AddTables("t_vac_escluse")
                .AddSelectFields("1")
                .AddWhereCondition("vpr_paz_codice", Comparatori.Uguale, "vex_paz_codice", DataTypes.Join)
                .AddWhereCondition("vpr_vac_codice", Comparatori.Uguale, "vex_vac_codice", DataTypes.Join)
                .OpenParanthesis()
                .AddWhereCondition("vex_data_scadenza", Comparatori.Is, DBNull.Value, DataTypes.Data)
                .AddWhereCondition("vex_data_scadenza", Comparatori.Maggiore, "vpr_cnv_data", DataTypes.Replace, "OR")
                .CloseParanthesis()

                Dim qrVacProgrEscluse As String = .GetSelect()

                .NewQuery(False, False)
                .AddTables("t_vac_programmate")
                .AddSelectFields("1")
                .AddWhereCondition("vpr_paz_codice", Comparatori.Uguale, "cnv_paz_codice", DataTypes.Join)
                .AddWhereCondition("vpr_cnv_data", Comparatori.Uguale, "cnv_data", DataTypes.Join)
                .AddWhereCondition("", Comparatori.NotExist, String.Format("({0})", qrVacProgrEseguite), DataTypes.Replace)
                .AddWhereCondition("", Comparatori.NotExist, String.Format("({0})", qrVacProgrEscluse), DataTypes.Replace)

                Dim qrVacProgr As String = .GetSelect()

                .NewQuery(False, False)
                .AddTables("t_cnv_convocazioni")
                .AddWhereCondition("cnv_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                If dataConvocazione.HasValue Then .AddWhereCondition("cnv_data", Comparatori.Uguale, dataConvocazione, DataTypes.Data)
                .AddWhereCondition("", Comparatori.NotExist, String.Format("({0})", qrBilProgr), DataTypes.Replace)
                .AddWhereCondition("", Comparatori.NotExist, String.Format("({0})", qrVacProgr), DataTypes.Replace)

            End With

            Try
                count = _DAM.ExecNonQuery(ExecQueryType.Delete)

            Catch ex As Exception
                LogError(ex, String.Format("DbConvocazioneProvider.DeleteEmpty [Codice Paziente: {0}]", codicePaziente))
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return (count > 0)

        End Function

        Public Function EliminaCicliSenzaVaccinazioniProgrammate(codicePaziente As Integer, dataConvocazione As Nullable(Of DateTime)) As Integer Implements IConvocazioneProvider.EliminaCicliSenzaVaccinazioniProgrammate

            With _DAM.QB

                .NewQuery()
                .AddTables("t_vac_programmate")
                .AddSelectFields("1")
                .AddWhereCondition("vpr_paz_codice", Comparatori.Uguale, "cnc_cnv_paz_codice", DataTypes.Replace)
                .AddWhereCondition("vpr_cnv_data", Comparatori.Uguale, "cnc_cnv_data", DataTypes.Replace)
                .AddWhereCondition("vpr_cic_codice", Comparatori.Uguale, "cnc_cic_codice", DataTypes.Replace)
                .AddWhereCondition("vpr_n_seduta", Comparatori.Uguale, "cnc_sed_n_seduta", DataTypes.Replace)

                Dim queryVaccinazioneProgrammataExists As String = .GetSelect()

                .NewQuery(False, False)
                .AddTables("t_cnv_cicli")
                .AddWhereCondition("cnc_cnv_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                If dataConvocazione.HasValue Then .AddWhereCondition("cnc_cnv_data", Comparatori.Uguale, dataConvocazione, DataTypes.Data)
                .AddWhereCondition("", Comparatori.NotExist, String.Format("({0})", queryVaccinazioneProgrammataExists), DataTypes.Replace)

            End With

            Try
                Return _DAM.ExecNonQuery(ExecQueryType.Delete)

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

        End Function

#End Region

#Region " Insert / Update "

        Public Function InsertConvocazione(convocazione As Entities.Convocazione) As Boolean Implements IConvocazioneProvider.InsertConvocazione

            Dim count As Integer = 0

            Try
                With _DAM.QB

                    .NewQuery()

                    .AddTables("t_cnv_convocazioni")

                    .AddInsertField("cnv_paz_codice", convocazione.Paz_codice, DataTypes.Numero)
                    If convocazione.Paz_codice_old > 0 Then
                        .AddInsertField("cnv_paz_codice_old", GetIntParam(convocazione.Paz_codice_old), DataTypes.Numero)
                    End If
                    .AddInsertField("cnv_data", convocazione.Data_CNV, DataTypes.Data)
                    .AddInsertField("cnv_cns_codice", convocazione.Cns_Codice, DataTypes.Stringa)

                    If convocazione.IdUtente > 0 Then
                        .AddInsertField("cnv_ute_id", GetIntParam(convocazione.IdUtente), DataTypes.Numero)
                    End If

                    .AddInsertField("cnv_data_invio", GetDateParam(convocazione.DataInvio), DataTypes.DataOra)
                    .AddInsertField("cnv_data_appuntamento", GetDateParam(convocazione.DataAppuntamento), DataTypes.DataOra)
                    .AddInsertField("cnv_durata_appuntamento", GetIntParam(convocazione.Durata_Appuntamento), DataTypes.Numero)
                    .AddInsertField("cnv_tipo_appuntamento", GetStringParam(convocazione.TipoAppuntamento), DataTypes.Stringa)
                    .AddInsertField("cnv_primo_appuntamento", GetDateParam(convocazione.DataPrimoAppuntamento), DataTypes.DataOra)
                    .AddInsertField("cnv_rinvio", GetStringParam(convocazione.Rinvio), DataTypes.Stringa)
                    .AddInsertField("cnv_eta_pomeriggio", GetStringParam(convocazione.EtaPomeriggio), DataTypes.Stringa)
                    .AddInsertField("cnv_campagna", GetStringParam(IIf(String.IsNullOrEmpty(convocazione.CampagnaVaccinale), "N", convocazione.CampagnaVaccinale)), DataTypes.Stringa)
                    If convocazione.CodiceAmbulatorio > 0 Then
                        .AddInsertField("cnv_amb_codice", GetIntParam(convocazione.CodiceAmbulatorio), DataTypes.Numero)
                    End If

                    If convocazione.DataInserimento.HasValue Then
                        .AddInsertField("cnv_data_inserimento", convocazione.DataInserimento.Value, DataTypes.DataOra)
                    End If

                    If convocazione.IdUtenteInserimento.HasValue Then
                        .AddInsertField("cnv_ute_id_inserimento", convocazione.IdUtenteInserimento.Value, DataTypes.Numero)
                    End If

                End With

                count = _DAM.ExecNonQuery(ExecQueryType.Insert)

            Catch ex As Exception

                Dim descErrore As String = String.Format("Inserimento Convocazione Paziente= {0} DataCNV= {1} CNS= {2} Durata= {3}",
                                                         convocazione.Paz_codice.ToString(),
                                                         convocazione.Data_CNV.ToString("d"),
                                                         convocazione.Cns_Codice,
                                                         convocazione.Durata_Appuntamento.ToString())
                LogError(ex, descErrore)
                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return (count > 0)

        End Function

        Public Function UpdateConvocazione(convocazione As Entities.Convocazione) As Boolean Implements IConvocazioneProvider.UpdateConvocazione

            Dim count As Integer = 0

            With _DAM.QB

                .NewQuery()

                .AddTables("t_cnv_convocazioni")

                .AddUpdateField("cnv_cns_codice", convocazione.Cns_Codice, DataTypes.Stringa)
                .AddUpdateField("cnv_primo_appuntamento", GetDateParam(convocazione.DataPrimoAppuntamento), DataTypes.DataOra)
                .AddUpdateField("cnv_data_appuntamento", GetDateParam(convocazione.DataAppuntamento), DataTypes.DataOra)
                .AddUpdateField("cnv_durata_appuntamento", GetIntParam(convocazione.Durata_Appuntamento), DataTypes.Numero)
                .AddUpdateField("cnv_tipo_appuntamento", GetStringParam(convocazione.TipoAppuntamento), DataTypes.Stringa)
                .AddUpdateField("cnv_paz_codice_old", GetIntParam(convocazione.Paz_codice_old), DataTypes.Numero)
                .AddUpdateField("cnv_ute_id", GetIntParam(convocazione.IdUtente), DataTypes.Numero)
                .AddUpdateField("cnv_data_invio", GetDateParam(convocazione.DataInvio), DataTypes.DataOra)
                .AddUpdateField("cnv_rinvio", GetStringParam(convocazione.Rinvio), DataTypes.Stringa)
                .AddUpdateField("cnv_eta_pomeriggio", GetStringParam(convocazione.EtaPomeriggio), DataTypes.Stringa)
                .AddUpdateField("cnv_campagna", GetStringParam(convocazione.CampagnaVaccinale), DataTypes.Stringa)
                .AddUpdateField("cnv_amb_codice", GetIntParam(convocazione.CodiceAmbulatorio), DataTypes.Numero)

                .AddWhereCondition("cnv_paz_codice", Comparatori.Uguale, convocazione.Paz_codice, DataTypes.Numero)
                .AddWhereCondition("cnv_data", Comparatori.Uguale, convocazione.Data_CNV, DataTypes.Data)

            End With

            Try
                count = _DAM.ExecNonQuery(ExecQueryType.Update)

            Catch ex As Exception

                Dim descErrore As String = String.Format("Modifica Convocazione Paziente= {0} DataCNV= {1} CNS= {2} Durata= {3}",
                                                         convocazione.Paz_codice.ToString(),
                                                         convocazione.Data_CNV.ToString("d"),
                                                         convocazione.Cns_Codice,
                                                         convocazione.Durata_Appuntamento.ToString())
                LogError(ex, descErrore)
                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return (count > 0)

        End Function

        Public Sub UpdateDataInvioSollecitoVaccinazioni(codicePaziente As Int32, dataConvocazione As Date, dataStampa As Date) Implements IConvocazioneProvider.UpdateDataInvioSollecitoVaccinazioni

            With _DAM.QB

                .NewQuery()

                .AddTables("T_CNV_CICLI")

                .AddUpdateField("CNC_DATA_INVIO_SOLLECITO", dataStampa, DataTypes.DataOra)

                .AddWhereCondition("CNC_CNV_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("CNC_CNV_DATA", Comparatori.Uguale, dataConvocazione, DataTypes.Data)
                .AddWhereCondition("CNC_DATA_INVIO_SOLLECITO", Comparatori.Is, "NULL", DataTypes.Replace)

            End With

            Try
                _DAM.ExecNonQuery(ExecQueryType.Update)

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

        End Sub

        Public Function UpdateDataInvio(codicePaziente As Integer, dataConvocazione As Date, dataInvio As Date, noteAvvisi As String) As Boolean Implements IConvocazioneProvider.UpdateDataInvio

            Dim count As Integer = 0

            With _DAM.QB

                .NewQuery()

                .AddTables("t_cnv_convocazioni")

                If dataInvio = Date.MinValue Then
                    .AddUpdateField("cnv_data_invio", DBNull.Value, DataTypes.Data)
                Else
                    .AddUpdateField("cnv_data_invio", dataInvio, DataTypes.Data)
                End If

                If Not String.IsNullOrWhiteSpace(noteAvvisi) Then
                    .AddUpdateField("cnv_note_avvisi", noteAvvisi, DataTypes.Stringa)
                Else
                    .AddUpdateField("cnv_note_avvisi", DBNull.Value, DataTypes.Replace)
                End If

                .AddWhereCondition("cnv_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("cnv_data", Comparatori.Uguale, dataConvocazione, DataTypes.Data)

            End With

            Try
                count = _DAM.ExecNonQuery(ExecQueryType.Update)

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return (count > 0)

        End Function

        ''' <summary>
        ''' Modifica la durata dell'appuntamento per la convocazione specificata.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataConvocazione"></param>
        ''' <param name="durataAppuntamento"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateDurataAppuntamento(codicePaziente As Integer, dataConvocazione As Date, durataAppuntamento As Integer) As Boolean Implements IConvocazioneProvider.UpdateDurataAppuntamento

            Dim count As Integer = 0

            With _DAM.QB
                .NewQuery(True)
                .AddTables("t_cnv_convocazioni")
                .AddUpdateField("cnv_durata_appuntamento", durataAppuntamento, DataTypes.Numero)
                .AddWhereCondition("cnv_data", Comparatori.Uguale, dataConvocazione, DataTypes.Data)
                .AddWhereCondition("cnv_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
            End With

            Try
                count = _DAM.ExecNonQuery(ExecQueryType.Update)

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return (count > 0)

        End Function

        'TODO: sistemare questa funzione per numero di campi qualsiasi => bisognerebbe spostare la logica nel BIZ!
        Public Sub Copy(codicePaziente As Integer, dataConvocazioneOld As Date, dataConvocazioneNew As Date, empyDataAppuntamento As Boolean, emptyDataInvio As Boolean) Implements IConvocazioneProvider.Copy

            Try
                With _DAM.QB

                    .NewQuery(True)
                    .AddSelectFields("CNV_PAZ_CODICE, CNV_DATA, CNV_DATA_INVIO",
                                     "CNV_DATA_APPUNTAMENTO, CNV_DURATA_APPUNTAMENTO, CNV_RINVIO, CNV_CNS_CODICE",
                                     "CNV_ETA_POMERIGGIO, CNV_UTE_ID, CNV_TIPO_APPUNTAMENTO, CNV_PAZ_CODICE_OLD",
                                     "CNV_PRIMO_APPUNTAMENTO, CNV_CAMPAGNA, CNV_DATA_INSERIMENTO, CNV_UTE_ID_INSERIMENTO")
                    .AddTables("t_cnv_convocazioni")
                    .AddWhereCondition("cnv_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                    .AddWhereCondition("cnv_data", Comparatori.Uguale, dataConvocazioneOld, DataTypes.Data)

                    Using dtr As IDataReader = _DAM.BuildDataReader()

                        If dtr.Read() Then

                            '----------- Test: lettura dei campi datareader ------------------------------------------------
                            Dim obj As Object
                            obj = dtr("CNV_PAZ_CODICE")
                            obj = dtr("CNV_DATA")
                            obj = dtr("CNV_DATA_INVIO")
                            obj = dtr("CNV_DATA_APPUNTAMENTO")
                            obj = dtr("CNV_DURATA_APPUNTAMENTO")
                            obj = dtr("CNV_RINVIO")
                            obj = dtr("CNV_CNS_CODICE")
                            obj = dtr("CNV_ETA_POMERIGGIO")
                            obj = dtr("CNV_UTE_ID")
                            obj = dtr("CNV_TIPO_APPUNTAMENTO")
                            obj = dtr("CNV_PAZ_CODICE_OLD")
                            obj = dtr("CNV_PRIMO_APPUNTAMENTO")
                            obj = dtr("CNV_CAMPAGNA")
                            '-----------------------------------------------------------------------------------------------

                            .NewQuery(True)
                            .AddTables("t_cnv_convocazioni")
                            .AddInsertField("CNV_PAZ_CODICE", dtr("CNV_PAZ_CODICE"), DataTypes.Numero)
                            .AddInsertField("CNV_DATA", dataConvocazioneNew, DataTypes.Data)

                            If emptyDataInvio Then
                                .AddInsertField("CNV_DATA_INVIO", DBNull.Value, DataTypes.Data)
                            Else
                                .AddInsertField("CNV_DATA_INVIO", dtr("CNV_DATA_INVIO"), DataTypes.Data)
                            End If

                            If empyDataAppuntamento Then
                                .AddInsertField("CNV_DATA_APPUNTAMENTO", DBNull.Value, DataTypes.Data)
                            Else
                                .AddInsertField("CNV_DATA_APPUNTAMENTO", dtr("CNV_DATA_APPUNTAMENTO"), DataTypes.Data)
                            End If

                            .AddInsertField("CNV_DURATA_APPUNTAMENTO", dtr("CNV_DURATA_APPUNTAMENTO"), DataTypes.Numero)
                            .AddInsertField("CNV_RINVIO", dtr("CNV_RINVIO"), DataTypes.Stringa)
                            .AddInsertField("CNV_CNS_CODICE", dtr("CNV_CNS_CODICE"), DataTypes.Stringa)
                            .AddInsertField("CNV_ETA_POMERIGGIO", dtr("CNV_ETA_POMERIGGIO"), DataTypes.Stringa)
                            .AddInsertField("CNV_UTE_ID", dtr("CNV_UTE_ID"), DataTypes.Numero)
                            .AddInsertField("CNV_TIPO_APPUNTAMENTO", dtr("CNV_TIPO_APPUNTAMENTO"), DataTypes.Stringa)
                            .AddInsertField("CNV_PAZ_CODICE_OLD", dtr("CNV_PAZ_CODICE_OLD"), DataTypes.Numero)
                            .AddInsertField("CNV_PRIMO_APPUNTAMENTO", dtr("CNV_PRIMO_APPUNTAMENTO"), DataTypes.Data)
                            .AddInsertField("CNV_CAMPAGNA", dtr("CNV_CAMPAGNA"), DataTypes.Stringa)

                            ' TODO [storicoAppunt]: sono da copiare o da valorizzare con data odierna e utente corrente?
                            .AddInsertField("CNV_DATA_INSERIMENTO", dtr("CNV_DATA_INSERIMENTO"), DataTypes.DataOra)
                            .AddInsertField("CNV_UTE_ID_INSERIMENTO", dtr("CNV_UTE_ID_INSERIMENTO"), DataTypes.Numero)

                            _DAM.ExecNonQuery(ExecQueryType.Insert)

                        End If
                    End Using

                End With

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

        End Sub

        '''<summary>Modifica la convocazione. Se in_campagna è true, imposta il campo a "S", altrimenti a "N".
        '''</summary>
        Sub UpdateCnvCampagna(codicePaziente As Integer, dataConvocazione As Date, inCampagna As Boolean) Implements IConvocazioneProvider.UpdateCnvCampagna

            With _DAM.QB

                .NewQuery(True)

                .AddTables("T_CNV_CONVOCAZIONI")

                If inCampagna Then
                    .AddUpdateField("cnv_campagna", "S", DataTypes.Stringa)
                Else
                    .AddUpdateField("cnv_campagna", "N", DataTypes.Stringa)
                End If

                .AddWhereCondition("CNV_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("CNV_DATA", Comparatori.Uguale, dataConvocazione, DataTypes.Data)

            End With

            Try
                _DAM.ExecNonQuery(ExecQueryType.Update)

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

        End Sub

        '''' <summary>Update della data di convocazione dei cicli associati alla cnv unita.
        ''''  Restituisce il numero di record aggiornati.</summary>
        '''' <param name="codicePaziente"></param>
        '''' <param name="oldDataConvocazione"></param>
        '''' <param name="newDataConvocazione"></param>
        Public Function InsertCicliCnvUnita(codicePaziente As Integer, oldDataConvocazione As Date, newDataConvocazione As Date) As Integer Implements IConvocazioneProvider.InsertCicliCnvUnita

            Dim count As Integer = 0

            Using cmd As New OracleCommand(Queries.Convocazioni.OracleQueries.insCnvUnita_Cicli, Me.Connection)

                cmd.Parameters.AddWithValue("new_data_cnv", newDataConvocazione)
                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                cmd.Parameters.AddWithValue("old_data_cnv", oldDataConvocazione)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteNonQuery()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        count = Convert.ToInt32(obj)
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Modifica dei dati per la convocazione solo bilancio specificata (non controlla se effettivamente è una convocazione per solo bilancio). 
        ''' La durata dell'appuntamento viene impostata pari al valore del parametro TEMPOBIL e il flag campagna è valorizzato a 'N'.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataConvocazione"></param>
        ''' <param name="durataAppuntamento"></param>
        ''' <param name="campagna"></param>
        Public Function UpdateDatiConvocazioneSoloBilancio(codicePaziente As Integer, dataConvocazione As Date, durataAppuntamento As Integer, campagna As String) As Boolean Implements IConvocazioneProvider.UpdateDatiConvocazioneSoloBilancio

            Dim count As Int16 = 0

            Using cmd As New OracleClient.OracleCommand(Queries.Convocazioni.OracleQueries.updDatiConvocazioneSoloBilancio, Me.Connection)

                cmd.Parameters.AddWithValue("durata", GetIntParam(durataAppuntamento))
                cmd.Parameters.AddWithValue("campagna", GetStringParam(campagna, True))
                cmd.Parameters.AddWithValue("paz_codice", codicePaziente)
                cmd.Parameters.AddWithValue("cnv_data", dataConvocazione)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        '''  Update della data di convocazione dei ritardi associati alla cnv unita.
        '''  Restituisce il numero di record aggiornati.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="oldDataConvocazione"></param>
        ''' <param name="newDataConvocazione"></param>
        Public Function UpdateRitardiCnvUnita(codicePaziente As Integer, oldDataConvocazione As Date, newDataConvocazione As Date) As Integer Implements IConvocazioneProvider.UpdateRitardiCnvUnita

            Dim count As Integer = 0

            Using cmd As New OracleClient.OracleCommand(Queries.Convocazioni.OracleQueries.updCnvUnita_Ritardi, Me.Connection)

                cmd.Parameters.AddWithValue("new_data_cnv", newDataConvocazione)
                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                cmd.Parameters.AddWithValue("old_data_cnv", oldDataConvocazione)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteNonQuery()
                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        count = Convert.ToInt32(obj)
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Cancellazione cicli della convocazione unita.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="oldDataConvocazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteCicliCnvUnita(codicePaziente As Integer, oldDataConvocazione As Date) As Integer Implements IConvocazioneProvider.DeleteCicliCnvUnita

            Return Me.EliminaCicliConvocazione(codicePaziente, oldDataConvocazione)

        End Function

        Public Sub InsertCicloConvocazione(cicloConvocazione As Entities.CicloConvocazione) Implements IConvocazioneProvider.InsertCicloConvocazione

            With _DAM.QB

                .NewQuery()

                .AddTables("t_cnv_cicli")

                .AddInsertField("cnc_cnv_paz_codice", cicloConvocazione.CodicePaziente, DataTypes.Numero)
                .AddInsertField("cnc_cnv_data", cicloConvocazione.DataConvocazione, DataTypes.Data)
                .AddInsertField("cnc_cic_codice", cicloConvocazione.CodiceCiclo, DataTypes.Stringa)
                .AddInsertField("cnc_sed_n_seduta", cicloConvocazione.NumeroSeduta, DataTypes.Numero)
                .AddInsertField("cnc_cnv_paz_codice_old", cicloConvocazione.CodicePazientePrecedente, DataTypes.Numero)
                .AddInsertField("cnc_flag_giorni_posticipo", cicloConvocazione.FlagGiorniPosticipo, DataTypes.Stringa)
                .AddInsertField("cnc_flag_posticipo_seduta", cicloConvocazione.FlagPosticipoSeduta, DataTypes.Stringa)
                .AddInsertField("cnc_n_sollecito", cicloConvocazione.NumeroSollecito, DataTypes.Numero)
                .AddInsertField("cnc_data_invio_sollecito", cicloConvocazione.DataInvioSollecito, DataTypes.DataOra)

                If cicloConvocazione.DataInserimento.HasValue Then
                    .AddInsertField("cnc_data_inserimento", cicloConvocazione.DataInserimento.Value, DataTypes.DataOra)
                End If

                If cicloConvocazione.IdUtenteInserimento.HasValue Then
                    .AddInsertField("cnc_ute_id_inserimento", cicloConvocazione.IdUtenteInserimento.Value, DataTypes.Numero)
                End If

            End With

            _DAM.ExecNonQuery(ExecQueryType.Insert)

        End Sub

        Public Sub InsertRitardoCicloConvocazione(ritardoCicloConvocazione As Entities.RitardoCicloConvocazione) Implements IConvocazioneProvider.InsertRitardoCicloConvocazione

            With _DAM.QB
                .NewQuery()
                .AddTables("t_paz_ritardi")
                .AddInsertField("pri_paz_codice", ritardoCicloConvocazione.CodicePaziente, DataTypes.Numero)
                .AddInsertField("pri_cnv_data", ritardoCicloConvocazione.DataConvocazione, DataTypes.Data)
                .AddInsertField("pri_cic_codice", ritardoCicloConvocazione.CodiceCiclo, DataTypes.Stringa)
                .AddInsertField("pri_sed_n_seduta", ritardoCicloConvocazione.NumeroSeduta, DataTypes.Numero)
                .AddInsertField("pri_paz_codice_old", ritardoCicloConvocazione.CodicePazientePrecedente, DataTypes.Numero)
                .AddInsertField("pri_data_invio1", ritardoCicloConvocazione.DataInvio, DataTypes.DataOra)
                .AddInsertField("pri_data_appuntamento1", ritardoCicloConvocazione.DataAppuntamento1, DataTypes.DataOra)
                .AddInsertField("pri_data_appuntamento2", ritardoCicloConvocazione.DataAppuntamento2, DataTypes.DataOra)
                .AddInsertField("pri_data_appuntamento3", ritardoCicloConvocazione.DataAppuntamento3, DataTypes.DataOra)
                .AddInsertField("pri_data_appuntamento4", ritardoCicloConvocazione.DataAppuntamento4, DataTypes.DataOra)
                .AddInsertField("pri_note", ritardoCicloConvocazione.Note, DataTypes.Stringa)
            End With

            _DAM.ExecNonQuery(ExecQueryType.Insert)

        End Sub

        ''' <summary>
        ''' Cancellazione cicli della convocazione specificata.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataConvocazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EliminaCicliConvocazione(codicePaziente As Integer, dataConvocazione As DateTime) As Integer Implements IConvocazioneProvider.EliminaCicliConvocazione

            Dim count As Integer = 0

            Using cmd As New OracleClient.OracleCommand(Queries.Convocazioni.OracleQueries.delCnv_Cicli, Me.Connection)

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                cmd.Parameters.AddWithValue("cnv_data", dataConvocazione)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteNonQuery()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        count = Convert.ToInt32(obj)
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

#End Region

    End Class

End Namespace
