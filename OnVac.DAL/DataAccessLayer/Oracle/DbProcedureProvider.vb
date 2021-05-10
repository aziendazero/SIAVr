Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager


Namespace DAL.Oracle

    Public Class DbProcedureProvider
        Inherits DbProvider
        Implements IProcedureProvider

#Region " Constructors "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " IProcedureProvider "

        ''' <summary>
        ''' Restituisce tutte le procedure presenti in tabella
        ''' </summary>
        ''' <remarks> In caso di errore scrive nel log
        ''' </remarks>
        Public Function GetProcedure() As DataTable Implements IProcedureProvider.GetProcedure

            Dim ownTransaction As Boolean = False

            With _DAM.QB

                'NB: TRANSAZIONE NECESSARIA PER DB LINK 
                If Not _DAM.ExistTra Then
                    _DAM.BeginTrans()
                    ownTransaction = True
                End If

                .NewQuery()
                .AddSelectFields("*")
                .AddTables("t_ana_procedure")

            End With

            Try
                _DAM.BuildDataTable(_DT)

                'NB: TRANSAZIONE NECESARIA PER DB LINK 
                If ownTransaction Then
                    _DAM.Commit()
                End If

            Catch ex As Exception

                'NB: TRANSAZIONE NECESARIA PER DB LINK 
                If ownTransaction Then
                    _DAM.Rollback()
                End If

                LogError(ex)
                SetErrorMsg("Errore durante la lettura delle procedure da database.")
                _DT = Nothing

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return _DT

        End Function

        ''' <summary>
        ''' Riempie la tabella dei pazienti da elaborare 
        ''' </summary>
        ''' <param name="procedureId"></param>
        ''' <param name="jobId"></param>
        ''' <param name="patients"></param>
        ''' <param name="strParameters"></param>
        ''' <param name="uteId"></param>
        ''' <returns></returns>
        Public Function FillTable(procedureId As Integer, jobId As Integer, patients As ArrayList, strParameters As String, uteId As Integer) As Boolean Implements IProcedureProvider.FillTable

            Dim result As Boolean = True
            Dim data As Date = Date.Now

            _DAM.BeginTrans()

            Try

                For i As Integer = 0 To patients.Count - 1

                    With _DAM.QB
                        .NewQuery()
                        .AddInsertField("plb_prd_codice", procedureId, DataTypes.Numero)
                        .AddInsertField("plb_id", jobId, DataTypes.Numero)
                        .AddInsertField("plb_paz_codice", patients(i), DataTypes.Stringa)
                        .AddInsertField("plb_parametri", strParameters, DataTypes.Stringa)
                        .AddInsertField("plb_data_richiesta", data, DataTypes.DataOra)
                        .AddInsertField("plb_ute_id_richiesta", uteId, DataTypes.Numero)
                        .AddTables("t_paz_elaborazioni")
                    End With

                    _DAM.ExecNonQuery(ExecQueryType.Insert)
                Next

                _DAM.Commit()

            Catch ex As Exception

                _DAM.Rollback()
                LogError(ex)

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return result

        End Function

        Public Function StoreParameters(procedureId As Integer, parameters As Hashtable) As Boolean Implements IProcedureProvider.StoreParameters

            Dim result As Boolean = True

            _DAM.BeginTrans()

            Try

                Dim param As IDictionaryEnumerator = parameters.GetEnumerator()

                While param.MoveNext()

                    With _DAM.QB
                        .NewQuery()
                        .AddInsertField("prp_id", procedureId, DataTypes.Numero)
                        .AddInsertField("prp_nome", param.Key, DataTypes.Stringa)
                        .AddInsertField("prp_valore", param.Value, DataTypes.Stringa)
                        .AddTables("t_prc_parametri")
                    End With

                    _DAM.ExecNonQuery(ExecQueryType.Insert)

                End While

            Catch ex As Exception

                _DAM.Rollback()
                LogError(ex)

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            _DAM.Commit()

            Return result

        End Function

        Public Function GetErrorMsg() As String Implements IProcedureProvider.GetErrorMsg

            Return ERROR_MSG

        End Function

        Public Function GetProcedureNoStampaRisultati() As List(Of Integer) Implements IProcedureProvider.GetProcedureNoStampaRisultati

            Dim ownTransaction As Boolean = False

            Dim listProcedure As New List(Of Integer)

            With _DAM.QB

                'NB: TRANSAZIONE NECESSARIA PER DB LINK 
                If Not _DAM.ExistTra Then
                    _DAM.BeginTrans()
                    ownTransaction = True
                End If

                .NewQuery()
                .AddSelectFields("PRD_CODICE")
                .AddTables("T_ANA_PROCEDURE")
                .AddWhereCondition("PRD_STAMPA_RISULTATI", Comparatori.Uguale, "N", DataTypes.Stringa)

            End With

            Try
                Using idr As IDataReader = _DAM.BuildDataReader()

                    If Not idr Is Nothing Then

                        Dim prd_codice As Integer = idr.GetOrdinal("PRD_CODICE")

                        While idr.Read()
                            listProcedure.Add(idr.GetInt32OrDefault(prd_codice))
                        End While

                    End If

                End Using

                'NB: TRANSAZIONE NECESARIA PER DB LINK 
                If ownTransaction Then _DAM.Commit()

            Catch ex As Exception

                'NB: TRANSAZIONE NECESARIA PER DB LINK 
                If ownTransaction Then _DAM.Rollback()

                SetErrorMsg("Errore durante la lettura da database delle procedure per cui non stampare i risultati.")

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return listProcedure

        End Function

        ''' <summary>
        ''' Restituisce l'entity PazienteElaborazione in base al progressivo specificato
        ''' </summary>
        ''' <param name="progressivo"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetPazienteElaborazione(progressivo As Long) As Entities.PazienteElaborazione Implements IProcedureProvider.GetPazienteElaborazione

            Dim item As Entities.PazienteElaborazione = Nothing

            Using cmd As New System.Data.OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.CommandText = "select plb_progressivo, plb_prd_codice, plb_id, plb_paz_codice, plb_codice_errore, plb_descrizione_errore, plb_data_esecuzione, " +
                        " plb_ute_id_richiesta, plb_stato, plb_parametri, plb_data_richiesta, plb_data_convocazione, plb_paz_codice_alias " +
                        " from t_paz_elaborazioni where plb_progressivo = :plb_progressivo"

                    cmd.Parameters.AddWithValue("plb_progressivo", progressivo)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If idr IsNot Nothing Then

                            Dim plb_progressivo As Integer = idr.GetOrdinal("plb_progressivo")
                            Dim plb_prd_codice As Integer = idr.GetOrdinal("plb_prd_codice")
                            Dim plb_id As Integer = idr.GetOrdinal("plb_id")
                            Dim plb_paz_codice As Integer = idr.GetOrdinal("plb_paz_codice")
                            Dim plb_codice_errore As Integer = idr.GetOrdinal("plb_codice_errore")
                            Dim plb_descrizione_errore As Integer = idr.GetOrdinal("plb_descrizione_errore")
                            Dim plb_data_esecuzione As Integer = idr.GetOrdinal("plb_data_esecuzione")
                            Dim plb_ute_id_richiesta As Integer = idr.GetOrdinal("plb_ute_id_richiesta")
                            Dim plb_stato As Integer = idr.GetOrdinal("plb_stato")
                            Dim plb_parametri As Integer = idr.GetOrdinal("plb_parametri")
                            Dim plb_data_richiesta As Integer = idr.GetOrdinal("plb_data_richiesta")
                            Dim plb_data_convocazione As Integer = idr.GetOrdinal("plb_data_convocazione")
                            Dim plb_paz_codice_alias As Integer = idr.GetOrdinal("plb_paz_codice_alias")

                            If idr.Read() Then

                                item = New PazienteElaborazione()

                                item.Progressivo = idr.GetInt64(plb_progressivo)
                                item.CodiceProcedura = idr.GetInt32OrDefault(plb_prd_codice)
                                item.IdJob = idr.GetNullableInt64OrDefault(plb_id)
                                item.CodicePaziente = idr.GetStringOrDefault(plb_paz_codice)
                                item.CodiceErrore = idr.GetNullableInt32OrDefault(plb_codice_errore)
                                item.DescrizioneErrore = idr.GetStringOrDefault(plb_descrizione_errore)
                                item.DataEsecuzione = idr.GetNullableDateTimeOrDefault(plb_data_esecuzione)
                                item.IdUtenteRichiesta = idr.GetNullableInt64OrDefault(plb_ute_id_richiesta)
                                item.Stato = idr.GetStringOrDefault(plb_stato)
                                item.Parametri = idr.GetStringOrDefault(plb_parametri)
                                item.DataRichiesta = idr.GetNullableDateTimeOrDefault(plb_data_richiesta)
                                item.DataConvocazione = idr.GetNullableDateTimeOrDefault(plb_data_convocazione)
                                item.CodicePazienteAlias = idr.GetStringOrDefault(plb_paz_codice_alias)

                            End If

                        End If
                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return item

        End Function

        ''' <summary>
        ''' Inserimento massivo dei dati da elaborare nella tabella t_paz_elaborazioni.
        ''' </summary>
        ''' <param name="convocazioniDaElaborare"></param>
        ''' <param name="procedureId"></param>
        ''' <param name="jobId"></param>
        ''' <param name="userId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsertPazientiElaborazioni(convocazioniDaElaborare As List(Of Entities.ConvocazionePK), procedureId As Integer, jobId As Integer, userId As Integer) As Integer Implements IProcedureProvider.InsertPazientiElaborazioni

            Dim now As DateTime = Date.Now

            _DAM.BeginTrans()

            Try
                For Each cnv As Entities.ConvocazionePK In convocazioniDaElaborare

                    With _DAM.QB
                        .NewQuery()
                        .AddTables("t_paz_elaborazioni")
                        .AddInsertField("plb_prd_codice", procedureId, DataTypes.Numero)
                        .AddInsertField("plb_id", jobId, DataTypes.Numero)
                        .AddInsertField("plb_paz_codice", cnv.IdPaziente, DataTypes.Stringa)
                        .AddInsertField("plb_data_convocazione", cnv.Data, DataTypes.DataOra)
                        .AddInsertField("plb_data_richiesta", now, DataTypes.DataOra)
                        .AddInsertField("plb_ute_id_richiesta", userId, DataTypes.Numero)
                    End With

                    _DAM.ExecNonQuery(ExecQueryType.Insert)

                Next

                _DAM.Commit()

            Catch ex As Exception

                _DAM.Rollback()
                LogError(ex)

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return True

        End Function

        ''' <summary>
        ''' Inserimento nella t_paz_elaborazioni.
        ''' </summary>
        ''' <param name="pazienteElaborazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsertPazientiElaborazioni(pazienteElaborazione As Entities.PazienteElaborazione) As Integer Implements IProcedureProvider.InsertPazientiElaborazioni

            Dim count As Integer = 0

            Using cmd As New System.Data.OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    ' Sequence x progressivo
                    cmd.CommandText = "select seq_elaborazioni.nextval from dual"

                    pazienteElaborazione.Progressivo = cmd.ExecuteScalar()

                    ' Inserimento in t_paz_elaborazioni
                    cmd.CommandText = "insert into t_paz_elaborazioni " +
                        " (plb_progressivo, plb_prd_codice, plb_id, plb_paz_codice, plb_codice_errore, plb_descrizione_errore, plb_data_esecuzione, plb_ute_id_richiesta, " +
                        "  plb_stato, plb_parametri, plb_data_richiesta, plb_data_convocazione, plb_paz_codice_alias) values " +
                        " (:plb_progressivo, :plb_prd_codice, :plb_id, :plb_paz_codice, :plb_codice_errore, :plb_descrizione_errore, :plb_data_esecuzione, :plb_ute_id_richiesta, " +
                        "  :plb_stato, :plb_parametri, :plb_data_richiesta, :plb_data_convocazione, :plb_paz_codice_alias)"

                    cmd.Parameters.AddWithValue("plb_progressivo", pazienteElaborazione.Progressivo)
                    cmd.Parameters.AddWithValue("plb_prd_codice", pazienteElaborazione.CodiceProcedura)
                    cmd.Parameters.AddWithValue("plb_id", pazienteElaborazione.IdJob)
                    cmd.Parameters.AddWithValue("plb_paz_codice", pazienteElaborazione.CodicePaziente)
                    cmd.Parameters.AddWithValue("plb_codice_errore", GetIntParam(pazienteElaborazione.CodiceErrore))
                    cmd.Parameters.AddWithValue("plb_descrizione_errore", GetStringParam(pazienteElaborazione.DescrizioneErrore))
                    cmd.Parameters.AddWithValue("plb_data_esecuzione", GetDateParam(pazienteElaborazione.DataEsecuzione))
                    cmd.Parameters.AddWithValue("plb_ute_id_richiesta", GetLongParam(pazienteElaborazione.IdUtenteRichiesta))
                    cmd.Parameters.AddWithValue("plb_stato", GetStringParam(pazienteElaborazione.Stato))
                    cmd.Parameters.AddWithValue("plb_parametri", GetStringParam(pazienteElaborazione.Parametri))
                    cmd.Parameters.AddWithValue("plb_data_richiesta", GetDateParam(pazienteElaborazione.DataRichiesta))
                    cmd.Parameters.AddWithValue("plb_data_convocazione", GetDateParam(pazienteElaborazione.DataConvocazione))
                    cmd.Parameters.AddWithValue("plb_paz_codice_alias", GetStringParam(pazienteElaborazione.CodicePazienteAlias))

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Aggiornamento dati della singola elaborazione.
        ''' </summary>
        ''' <param name="progressivo"></param>
        ''' <param name="codiceErrore"></param>
        ''' <param name="descrizioneErrore"></param>
        ''' <param name="dataEsecuzione"></param>
        ''' <param name="statoElaborazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdatePazienteElaborazione(progressivo As Long, codiceErrore As Integer?, descrizioneErrore As String, dataEsecuzione As DateTime, statoElaborazione As String) As Integer Implements IProcedureProvider.UpdatePazientiElaborazioni

            Dim count As Integer = 0

            Using cmd As New OracleClient.OracleCommand()

                cmd.CommandText =
                    "update t_paz_elaborazioni " +
                    "set plb_codice_errore = :codiceErrore, " +
                    "plb_descrizione_errore = :descrizioneErrore, " +
                    "plb_data_esecuzione = :dataEsecuzione, " +
                    "plb_stato = :statoElaborazione " +
                    "where plb_progressivo = :progressivo"

                cmd.Connection = Me.Connection

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    If codiceErrore.HasValue Then
                        cmd.Parameters.AddWithValue("codiceErrore", codiceErrore.Value)
                    Else
                        cmd.Parameters.AddWithValue("codiceErrore", DBNull.Value)
                    End If

                    If String.IsNullOrWhiteSpace(descrizioneErrore) Then
                        cmd.Parameters.AddWithValue("descrizioneErrore", DBNull.Value)
                    Else
                        cmd.Parameters.AddWithValue("descrizioneErrore", descrizioneErrore)
                    End If

                    cmd.Parameters.AddWithValue("dataEsecuzione", dataEsecuzione)

                    If String.IsNullOrWhiteSpace(statoElaborazione) Then
                        cmd.Parameters.AddWithValue("statoElaborazione", DBNull.Value)
                    Else
                        cmd.Parameters.AddWithValue("statoElaborazione", statoElaborazione)
                    End If

                    cmd.Parameters.AddWithValue("progressivo", progressivo)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Inserimento parametri stampabili nel report in t_prc_parametri
        ''' </summary>
        ''' <param name="parametriReport"></param>
        ''' <param name="jobId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsertParametriReport(parametriReport As List(Of KeyValuePair(Of String, String)), jobId As Integer) As Integer Implements IProcedureProvider.InsertParametriReport

            Dim count As Integer = 0

            _DAM.BeginTrans()

            Try
                For Each parametro As KeyValuePair(Of String, String) In parametriReport

                    With _DAM.QB
                        .NewQuery()
                        .AddInsertField("prp_id", jobId, DataTypes.Numero)
                        .AddInsertField("prp_nome", parametro.Key, DataTypes.Stringa)
                        .AddInsertField("prp_valore", parametro.Value, DataTypes.Stringa)
                        .AddTables("t_prc_parametri")
                    End With

                    _DAM.ExecNonQuery(ExecQueryType.Insert)

                Next

                _DAM.Commit()

            Catch ex As Exception

                _DAM.Rollback()
                LogError(ex)

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return count

        End Function

#End Region

    End Class

End Namespace
