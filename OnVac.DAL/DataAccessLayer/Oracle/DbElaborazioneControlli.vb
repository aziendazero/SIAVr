Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.Data.OracleClient


Namespace DAL.Oracle

    Public Class DbElaborazioneControlliProvider
        Inherits DbProvider
        Implements IElaborazioneControlliProvider

#Region " Costruttore "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " IElaborazioneControlliProvider "

#Region " Elaborazione Controlli "

        Public Function CountListaElaborazioneControlli(filtro As Entities.FiltroElaborazioneControlli) As Integer Implements IElaborazioneControlliProvider.CountListaElaborazioneControlli

            Dim count As Integer = 0

            Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

                Dim query As String = GetCountQueryVCaricamentoControlloScuole() + String.Format(" where  CCS_APP_ID = :CCS_APP_ID")
                If filtro.IdUtente.HasValue Then
                    query = query + String.Format(" and CCS_UTE_ID_CARICAMENTO = :CCS_UTE_ID_CARICAMENTO")
                End If
                If filtro.DaData.HasValue Then
                    query = query + String.Format(" and CCS_DATA_INIZIO_CARICAMENTO >= :CCS_DA_INIZIO_CARICAMENTO")
                End If
                If filtro.AData.HasValue Then
                    query = query + String.Format(" and CCS_DATA_INIZIO_CARICAMENTO  < :CCS_A_INIZIO_CARICAMENTO + 1")
                End If
                If filtro.IdProcesso.HasValue Then
                    query = query + String.Format(" and CCS_PRC_ID_CARICAMENTO = :CCS_PRC_ID_CARICAMENTO")
                End If
                cmd.CommandText = query

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)
                    cmd.Parameters.AddWithValue("CCS_APP_ID", filtro.AppId)
                    If filtro.IdUtente.HasValue Then
                        cmd.Parameters.AddWithValue("CCS_UTE_ID_CARICAMENTO", filtro.IdUtente)
                    End If
                    If filtro.DaData.HasValue Then
                        cmd.Parameters.AddWithValue("CCS_DA_INIZIO_CARICAMENTO", filtro.DaData)
                    End If
                    If filtro.AData.HasValue Then
                        cmd.Parameters.AddWithValue("CCS_A_INIZIO_CARICAMENTO", filtro.AData)
                    End If
                    If filtro.IdProcesso.HasValue Then
                        cmd.Parameters.AddWithValue("CCS_PRC_ID_CARICAMENTO", filtro.IdProcesso)
                    End If
                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        count = Convert.ToInt32(obj)
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function
        ''' <summary>
        ''' Restituisce l'anagrafica del bilancio specificato.
        ''' </summary>
        ''' <param name="numeroBilancio"></param>
        ''' <param name="codiceMalattia"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetListaElaborazioneControlli(filtro As Entities.FiltroElaborazioneControlli, campoOrdinamento As String, versoOrdinamento As String, pagingOptions As Data.PagingOptions) As List(Of Entities.ElaborazioneControlli) Implements IElaborazioneControlliProvider.GetListaElaborazioneControlli

            Dim item As List(Of Entities.ElaborazioneControlli) = Nothing

            Dim ownConnection As Boolean = False

            Try
                ' N.B. : la join con la t_ana_link_malattie_tipologia è per compatibilità con il metodo che veniva usato prima

                Dim query As String = GetSelectQueryVCaricamentoControlloScuole() +
                    " where  CCS_APP_ID = :CCS_APP_ID"
                If filtro.IdUtente.HasValue Then
                    query = query + String.Format(" and CCS_UTE_ID_CARICAMENTO = :CCS_UTE_ID_CARICAMENTO")
                End If
                If filtro.DaData.HasValue Then
                    query = query + String.Format(" and CCS_DATA_INIZIO_CARICAMENTO >= :CCS_DA_INIZIO_CARICAMENTO")
                End If
                If filtro.AData.HasValue Then
                    query = query + String.Format(" and CCS_DATA_INIZIO_CARICAMENTO  < :CCS_A_INIZIO_CARICAMENTO + 1")
                End If
                If filtro.IdProcesso.HasValue Then
                    query = query + String.Format(" and CCS_PRC_ID_CARICAMENTO = :CCS_PRC_ID_CARICAMENTO")
                End If
                query = query + " order by CCS_PRC_ID_CARICAMENTO desc"


                Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)
                    cmd.Parameters.AddWithValue("CCS_APP_ID", filtro.AppId)
                    If filtro.IdUtente.HasValue Then
                        cmd.Parameters.AddWithValue("CCS_UTE_ID_CARICAMENTO", filtro.IdUtente)
                    End If
                    If filtro.DaData.HasValue Then
                        cmd.Parameters.AddWithValue("CCS_DA_INIZIO_CARICAMENTO", filtro.DaData)
                    End If
                    If filtro.AData.HasValue Then
                        cmd.Parameters.AddWithValue("CCS_A_INIZIO_CARICAMENTO", filtro.AData)
                    End If
                    If filtro.IdProcesso.HasValue Then
                        cmd.Parameters.AddWithValue("CCS_PRC_ID_CARICAMENTO", filtro.IdProcesso)
                    End If
                    ' Paginazione
                    If Not pagingOptions Is Nothing Then
                        cmd.AddPaginatedQuery(pagingOptions)
                    End If

                    item = GetListElabControlli(cmd)



                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return item

        End Function
        Public Function GetElaborazioneControlloXCodiceProc(filtro As Entities.FiltroSingolaElaborazioneControlli) As Entities.ElaborazioneControlli Implements IElaborazioneControlliProvider.GetElaborazioneControlloXCodiceProc

            Dim item As List(Of Entities.ElaborazioneControlli) = Nothing
            Dim dett As New Entities.ElaborazioneControlli

            Dim ownConnection As Boolean = False

            Try
                ' N.B. : la join con la t_ana_link_malattie_tipologia è per compatibilità con il metodo che veniva usato prima

                Dim query As String = GetSelectQueryVCaricamentoControlloScuoleXCodiceCaricamento()

                Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)
                    cmd.Parameters.AddWithValue("CCS_APP_ID", filtro.AppId)
                    cmd.Parameters.AddWithValue("CCS_CODICE_CARICAMENTO", GetGuidParam(filtro.CodiceProcesso))

                    item = GetListElabControlli(cmd)
                    If item.Count > 0 Then
                        dett = item(0)
                    End If


                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return dett

        End Function

        Private Function GetSelectQueryVCaricamentoControlloScuole() As String

            Return "select * from V_CARICAMENTO_CONTROLLO_SCUOLE "


        End Function
        Private Function GetSelectQueryVCaricamentoControlloScuoleXCodiceCaricamento() As String

            Return "select * from V_CARICAMENTO_CONTROLLO_SCUOLE  where CCS_APP_ID = :CCS_APP_ID and CCS_CODICE_CARICAMENTO = :CCS_CODICE_CARICAMENTO "


        End Function
        Private Function GetCountQueryVCaricamentoControlloScuole() As String

            Return "select Count(CCS_PRC_ID_CARICAMENTO) from V_CARICAMENTO_CONTROLLO_SCUOLE "


        End Function

        Private Function GetListElabControlli(cmd As OracleClient.OracleCommand) As List(Of Entities.ElaborazioneControlli)

            Dim list As New List(Of Entities.ElaborazioneControlli)()

            Using idr As IDataReader = cmd.ExecuteReader()
                If Not idr Is Nothing Then

                    Dim ccs_codice_caricamento As Integer = idr.GetOrdinal("CCS_CODICE_CARICAMENTO")
                    Dim ccs_prc_id_caricamento As Integer = idr.GetOrdinal("CCS_PRC_ID_CARICAMENTO")
                    Dim ccs_ute_id_caricamento As Integer = idr.GetOrdinal("CCS_UTE_ID_CARICAMENTO")
                    Dim ccs_ute_codice_caricamento As Integer = idr.GetOrdinal("CCS_UTE_CODICE_CARICAMENTO")
                    Dim ccs_app_id As Integer = idr.GetOrdinal("CCS_APP_ID")
                    Dim ccs_argomento As Integer = idr.GetOrdinal("CCS_ARGOMENTO")
                    Dim ccs_ente_caricamento As Integer = idr.GetOrdinal("CCS_ENTE_CARICAMENTO")
                    Dim ccs_nome_file_origine As Integer = idr.GetOrdinal("CCS_NOME_FILE_ORIGINE")
                    Dim ccs_nome_file_elaborazione As Integer = idr.GetOrdinal("CCS_NOME_FILE_ELABORAZIONE")
                    Dim ccs_data_inizio_caricamento As Integer = idr.GetOrdinal("CCS_DATA_INIZIO_CARICAMENTO")
                    Dim ccs_data_fine_caricamento As Integer = idr.GetOrdinal("CCS_DATA_FINE_CARICAMENTO")
                    Dim ccs_righe_elaborate As Integer = idr.GetOrdinal("CCS_RIGHE_ELABORATE")
                    Dim ccs_righe_scartate As Integer = idr.GetOrdinal("CCS_RIGHE_SCARTATE")
                    Dim ccs_stato_caricamento As Integer = idr.GetOrdinal("CCS_STATO_CARICAMENTO")
                    Dim ccs_ute_id_controllo As Integer = idr.GetOrdinal("CCS_UTE_ID_CONTROLLO")
                    Dim ccs_ute_codice_controllo As Integer = idr.GetOrdinal("CCS_UTE_CODICE_CONTROLLO")
                    Dim ccs_prc_id_controllo As Integer = idr.GetOrdinal("CCS_PRC_ID_CONTROLLO")
                    Dim ccs_data_inizio_controllo As Integer = idr.GetOrdinal("CCS_DATA_INIZIO_CONTROLLO")
                    Dim ccs_data_fine_controllo As Integer = idr.GetOrdinal("CCS_DATA_FINE_CONTROLLO")
                    Dim ccs_stato_controllo As Integer = idr.GetOrdinal("CCS_STATO_CONTROLLO")
                    Dim ccs_totale_record As Integer = idr.GetOrdinal("CCS_TOTALE_RECORD")
                    Dim ccs_totale_controllati As Integer = idr.GetOrdinal("CCS_TOTALE_CONTROLLATI")
                    Dim ccs_totale_errori As Integer = idr.GetOrdinal("CCS_TOTALE_ERRORI")
                    Dim ccs_totale_vaccinati As Integer = idr.GetOrdinal("CCS_TOTALE_VACCINATI")
                    Dim ccs_totale_non_vaccinati As Integer = idr.GetOrdinal("CCS_TOTALE_NON_VACCINATI")
                    Dim css_copertura As Integer = idr.GetOrdinal("CSS_COPERTURA")

                    While idr.Read()

                        Dim item As New Entities.ElaborazioneControlli

                        item.CodiceCaricamento = idr.GetGuidOrDefault(ccs_codice_caricamento)
                        item.IdCaricamento = idr.GetInt32(ccs_prc_id_caricamento)
                        item.IdUtenteCaricamento = idr.GetInt32(ccs_ute_id_caricamento)
                        item.CodiceUtenteCaricamento = idr.GetStringOrDefault(ccs_ute_codice_caricamento)
                        item.AppId = idr.GetStringOrDefault(ccs_app_id)
                        item.Argomento = idr.GetStringOrDefault(ccs_argomento)
                        item.EnteCaricamento = idr.GetGuidOrDefault(ccs_ente_caricamento)
                        item.NomeFileOrigine = idr.GetStringOrDefault(ccs_nome_file_origine)
                        item.NomeFileElaborazione = idr.GetStringOrDefault(ccs_nome_file_elaborazione)
                        item.DataInizioCaricamento = idr.GetDateTimeOrDefault(ccs_data_inizio_caricamento)
                        item.DataFineCaricamento = idr.GetDateTimeOrDefault(ccs_data_fine_caricamento)
                        item.RigheElaborate = idr.GetInt32OrDefault(ccs_righe_elaborate)
                        item.RigheScartate = idr.GetInt32OrDefault(ccs_righe_scartate)
                        item.StatoCaricamento = idr.GetStringOrDefault(ccs_stato_caricamento)
                        item.IdUtenteControllo = idr.GetNullableInt32OrDefault(ccs_ute_id_controllo)
                        item.CodiceUtenteControllo = idr.GetStringOrDefault(ccs_ute_codice_controllo)
                        item.IdControllo = idr.GetInt32OrDefault(ccs_prc_id_controllo)
                        item.DataInizioControllo = idr.GetNullableDateTimeOrDefault(ccs_data_inizio_controllo)
                        item.DataFineControllo = idr.GetNullableDateTimeOrDefault(ccs_data_fine_controllo)
                        item.StatoControllo = idr.GetStringOrDefault(ccs_stato_controllo)
                        item.TotaleRecord = idr.GetNullableInt32OrDefault(ccs_totale_record)
                        item.TotaleControllati = idr.GetNullableInt32OrDefault(ccs_totale_controllati)
                        item.TotaleErrore = idr.GetNullableInt32OrDefault(ccs_totale_errori)
                        item.TotaleVaccinati = idr.GetNullableInt32OrDefault(ccs_totale_vaccinati)
                        item.TotaleNonVaccinati = idr.GetNullableInt32OrDefault(ccs_totale_non_vaccinati)
                        item.Copertura = idr.GetNullableDecimalOrDefault(css_copertura)

                        list.Add(item)

                    End While

                End If
            End Using

            Return list

        End Function

        Public Function InsertTestataControlli(controlli As Entities.InsertControlliTesta) As Integer Implements IElaborazioneControlliProvider.InsertTestataControlli

            Dim id As Long = 0

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand()

                    cmd.Connection = Connection

                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.CommandText = "select SEQ_PROC_ID_CONTROLLI.nextval from DUAL"

                    id = Convert.ToInt64(cmd.ExecuteScalar())

                    cmd.Parameters.Clear()

                    cmd.CommandText =
                        "insert into t_controlli_testata (cot_prc_id_caricamento, cot_imr_codice, cot_ute_id_controllo, cot_prc_id_controllo, cot_data_inizio_controllo, cot_stato_controllo, cot_totale_record) " +
                        "values (:cot_prc_id_caricamento, :cot_imr_codice, :cot_ute_id_controllo, :cot_prc_id_controllo, :cot_data_inizio_controllo, :cot_stato_controllo, :cot_totale_record)"

                    cmd.Parameters.AddWithValue("cot_prc_id_caricamento", controlli.IdCaricamento)
                    cmd.Parameters.AddWithValue("cot_imr_codice", GetGuidParam(controlli.CodiceCaricamento))
                    cmd.Parameters.AddWithValue("cot_ute_id_controllo", controlli.IdUtenteControllo)
                    cmd.Parameters.AddWithValue("cot_prc_id_controllo", id)
                    cmd.Parameters.AddWithValue("cot_data_inizio_controllo", Date.Today)
                    cmd.Parameters.AddWithValue("cot_stato_controllo", controlli.StatoControllo)
                    cmd.Parameters.AddWithValue("cot_totale_record", controlli.TotaleRecord)

                    cmd.ExecuteNonQuery()

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return id

        End Function

        Public Function EseguiStoredProcedureControlli(idProceduraControlli As Integer) As Integer Implements IElaborazioneControlliProvider.EseguiStoredProcedureControlli

            Dim count As Integer = 0

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand()

                    cmd.Connection = Connection
                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.CommandText = "ESEGUI_CONTROLLI_SCUOLE"
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddWithValue("PRC_ID", idProceduraControlli)

                    count = cmd.ExecuteNonQuery()

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return count

        End Function

#End Region

#End Region

    End Class

End Namespace
