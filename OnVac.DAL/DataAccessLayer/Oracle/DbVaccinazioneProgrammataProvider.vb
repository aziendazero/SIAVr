Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.Log


Namespace DAL.Oracle

    Public Class DbVaccinazioneProgrammataProvider
        Inherits DbProvider
        Implements IVaccinazioneProgrammataProvider

#Region " Constructors "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " Metodi di Select "

        '''<summary>
        ''' Restituisce il numero di vaccinazioni programmate per il paziente specificato
        '''</summary>
        Public Function CountProgrammatePaziente(codicePaziente As Integer) As Integer Implements IVaccinazioneProgrammataProvider.CountProgrammatePaziente

            Dim numeroVaccinazioniProgrammate As Integer = 0

            Using cmd As New OracleClient.OracleCommand("select count(*) from t_vac_programmate where vpr_paz_codice = :cod_paz", Me.Connection)

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        numeroVaccinazioniProgrammate = Convert.ToInt32(obj)
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return numeroVaccinazioniProgrammate

        End Function

        Public Function CountFromKey(codicePaziente As Integer, dataConvocazione As Date) As Integer Implements IVaccinazioneProgrammataProvider.CountFromKey

            Dim count As Integer = 0

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try
                cmd = New OracleClient.OracleCommand(Queries.VaccinazioniProgrammate.OracleQueries.selCountVaccProgPaziente, Me.Connection)

                cmd.Parameters.AddWithValue("data_cnv", dataConvocazione)
                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Dim obj As Object = cmd.ExecuteScalar()

                If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                    Try
                        count = CInt(obj)
                    Catch ex As Exception
                        count = 0
                    End Try
                End If

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            Finally
                Me.ConditionalCloseConnection(ownConnection)
                If Not cmd Is Nothing Then cmd.Dispose()
            End Try

            Return count

        End Function

        '''<summary>
        ''' Restituisce un datatable con le vaccinazioni programmate per il paziente
        '''</summary>
        Public Function GetVacProgNotEseguiteAndNotEscluse(codicePaziente As Integer, dataConvocazione As Date) As DataTable Implements IVaccinazioneProgrammataProvider.GetVacProgNotEseguiteAndNotEscluse

            Dim dt As New DataTable()

            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim adp As OracleClient.OracleDataAdapter = Nothing

            Dim ownConnection As Boolean = False

            Try
                cmd = New OracleClient.OracleCommand(Queries.VaccinazioniProgrammate.OracleQueries.selVaccinazioniProgrammatePazienteNotEscluseAndNotEseguite, Me.Connection)

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                cmd.Parameters.AddWithValue("data_cnv", dataConvocazione)

                adp = New OracleClient.OracleDataAdapter(cmd)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                adp.Fill(dt)

            Catch ex As Exception

                Dim msg As New System.Text.StringBuilder()

                msg.AppendFormat("DAL: Errore ricerca vaccinazioni (VaccinazioneProg.GetVaccinazioniDt){0}", vbNewLine)
                msg.AppendFormat("Paziente:{0}{1}", codicePaziente, vbNewLine)
                msg.AppendFormat("Data Convocazione:{0:dd/MM/yyyy}", dataConvocazione)

                Throw New Exception(msg.ToString(), ex)

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not adp Is Nothing Then adp.Dispose()
                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return dt

        End Function

        ''' <summary>
        ''' Restituisce una lista di codice e dose delle vaccinazioni programmate del paziente nella data di convocazione specificata.
        ''' Nella lista restituita non ci sono quelle già eseguite o escluse.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataConvocazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetVacProgNotEseguiteAndNotEscluseListCodiceDose(codicePaziente As Long, dataConvocazione As Date) As List(Of KeyValuePair(Of String, Integer)) Implements IVaccinazioneProgrammataProvider.GetVacProgNotEseguiteAndNotEscluseListCodiceDose

            Dim list As New List(Of KeyValuePair(Of String, Integer))()

            Using cmd As New OracleClient.OracleCommand(Queries.VaccinazioniProgrammate.OracleQueries.selVaccinazioniProgrammatePazienteNotEscluseAndNotEseguite, Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                    cmd.Parameters.AddWithValue("data_cnv", dataConvocazione)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim codice As Integer = idr.GetOrdinal("VPR_VAC_CODICE")
                            Dim dose As Integer = idr.GetOrdinal("VPR_N_RICHIAMO")

                            While idr.Read()

                                list.Add(New KeyValuePair(Of String, Integer)(idr.GetString(codice), idr.GetInt32(dose)))

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
        ''' Restituisce una lista di oggetti BilancioVaccinazione con le vaccinazioni programmate del paziente nella data di convocazione specificata.
        ''' Nella lista restituita non ci sono quelle già eseguite o escluse.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataConvocazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetVacProgNotEseguiteAndNotEscluseListBilancioVaccinazioni(codicePaziente As Long, dataConvocazione As Date) As List(Of Entities.BilancioVaccinazione) Implements IVaccinazioneProgrammataProvider.GetVacProgNotEseguiteAndNotEscluseListBilancioVaccinazioni

            Dim list As New List(Of Entities.BilancioVaccinazione)()

            Using cmd As New OracleClient.OracleCommand(Queries.VaccinazioniProgrammate.OracleQueries.selVaccinazioniProgrammatePazienteNotEscluseAndNotEseguite, Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                    cmd.Parameters.AddWithValue("data_cnv", dataConvocazione)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim vpr_vac_codice As Integer = idr.GetOrdinal("VPR_VAC_CODICE")
                            Dim vac_descrizione As Integer = idr.GetOrdinal("VAC_DESCRIZIONE")
                            Dim vpr_n_richiamo As Integer = idr.GetOrdinal("VPR_N_RICHIAMO")
                            Dim vac_ordine As Integer = idr.GetOrdinal("VAC_ORDINE")

                            While idr.Read()

                                Dim item As New Entities.BilancioVaccinazione()
                                item.Codice = idr.GetString(vpr_vac_codice)
                                item.Descrizione = idr.GetString(vac_descrizione)
                                item.Dose = idr.GetInt32(vpr_n_richiamo)
                                item.Ordine = idr.GetInt32OrDefault(vac_ordine)

                                list.Add(item)

                            End While

                        End If
                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list

        End Function

        '''<summary>
        ''' Restituisce le vaccinazioni programmate per il paziente nella data specificata
        '''</summary>
        Public Function GetCodiceVacProgrammatePazienteByData(codicePaziente As Integer, dataConvocazione As DateTime) As ArrayList Implements IVaccinazioneProgrammataProvider.GetCodiceVacProgrammatePazienteByData

            Dim listVaccinazioniProgrammate As ArrayList = Nothing

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try
                cmd = New OracleClient.OracleCommand(Queries.VaccinazioniProgrammate.OracleQueries.selVaccinazioniProgrammatePazienteByData, Me.Connection)

                cmd.Parameters.AddWithValue("paz_cod", codicePaziente)
                cmd.Parameters.AddWithValue("data_cnv", dataConvocazione)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using idr As IDataReader = cmd.ExecuteReader()

                    If Not idr Is Nothing Then

                        Dim pos_vac As Integer = idr.GetOrdinal("vpr_vac_codice")

                        listVaccinazioniProgrammate = New ArrayList()

                        While idr.Read()
                            listVaccinazioniProgrammate.Add(idr(pos_vac).ToString())
                        End While

                    End If

                End Using

            Catch ex As Exception

                Dim msg As New System.Text.StringBuilder()

                msg.AppendFormat("DAL: Errore ricerca vaccinazioni (VaccinazioneProg.GetVaccinazioniProgrammatePazienteByData){0}", vbNewLine)
                msg.AppendFormat("Paziente:{0}{1}", codicePaziente, vbNewLine)
                msg.AppendFormat("Data Convocazione:{0:dd/MM/yyyy}", dataConvocazione)

                Throw New Exception(msg.ToString(), ex)

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return listVaccinazioniProgrammate

        End Function

        '''<summary>
        ''' Restituisce le vaccinazioni programmate per il paziente nella data specificata
        '''</summary>
        '''<remarks>
        ''' Le vaccinazioni devono far riferimento alle esclusioni anche nel
        ''' caso in cui non ne abbiano di associate, ma queste ultime siano
        ''' escluse tramite la vaccinazione sostituta
        '''</remarks>
        Public Function GetVacProgrammatePazienteByData(codicePaziente As Integer, dataConvocazione As DateTime) As DataTable Implements IVaccinazioneProgrammataProvider.GetVacProgrammatePazienteByData

            Dim dt As New DataTable()

            Using cmd As New OracleClient.OracleCommand(Queries.VaccinazioniProgrammate.OracleQueries.selVaccinazioniProgrammatePazienteByData2, Me.Connection)
                
                Dim ownConnection As Boolean = False

                Try
                    cmd.Parameters.AddWithValue("paz_cod", codicePaziente)
                    cmd.Parameters.AddWithValue("data_cnv", dataConvocazione)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using adp As New OracleClient.OracleDataAdapter(cmd)

                        adp.Fill(dt)

                    End Using

                Catch ex As Exception

                    Dim msg As New System.Text.StringBuilder()
                    msg.AppendFormat("DAL: Errore ricerca vaccinazioni (VaccinazioneProg.GetVaccinazioniProgrammatePazienteByData){0}", vbNewLine)
                    msg.AppendFormat("Paziente:{0}{1}", codicePaziente, vbNewLine)
                    msg.AppendFormat("Data Convocazione:{0:dd/MM/yyyy}", dataConvocazione)

                    Throw New Exception(msg.ToString(), ex)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return dt

        End Function

        '''<summary>
        ''' Restituisce le vaccinazioni programmate per il paziente nella data specificata
        '''</summary>
        Public Function GetDataByVaccinazione(codicePaziente As Integer, codiceVaccinazione As String) As Object Implements IVaccinazioneProgrammataProvider.GetDataByVaccinazione

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try
                cmd = New OracleClient.OracleCommand(Queries.VaccinazioniProgrammate.OracleQueries.selDataVaccinazioneProgrammata, Me.Connection)

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                cmd.Parameters.AddWithValue("cod_vac", codiceVaccinazione)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Return cmd.ExecuteScalar()

            Catch ex As Exception

                Dim msg As New System.Text.StringBuilder()

                msg.AppendFormat("DAL: Errore ricerca vaccinazioni (VaccinazioneProg.GetDataByVaccinazione){0}", vbNewLine)
                msg.AppendFormat("Paziente:{0}{1}", codicePaziente, vbNewLine)
                msg.AppendFormat("Codice Vaccinazione:{0}", codiceVaccinazione)

                Throw New Exception(msg.ToString(), ex)

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

        End Function

        '''<summary>
        ''' Restituisce le vaccinazioni programmate per il paziente nella data specificata
        '''</summary>
        Public Function ExistsVaccinazioneProgrammataByRichiamo(codicePaziente As Integer, codiceVaccinazione As String, richiamo As Int16) As Boolean Implements IVaccinazioneProgrammataProvider.ExistsVaccinazioneProgrammataByRichiamo

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try
                cmd = New OracleClient.OracleCommand(Queries.VaccinazioniProgrammate.OracleQueries.selExistsVaccinazioneRichiamo, Connection)

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                cmd.Parameters.AddWithValue("cod_vac", codiceVaccinazione)
                cmd.Parameters.AddWithValue("vpr_n_richiamo", richiamo)

                ownConnection = ConditionalOpenConnection(cmd)

                Dim obj As Object = cmd.ExecuteScalar()

                If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                    Return True
                Else
                    Return False
                End If

            Catch ex As Exception

                Dim msg As New System.Text.StringBuilder()
                msg.AppendFormat("DAL: Errore ricerca vaccinazioni (VaccinazioneProg.GetDataByVaccinazione){0}", vbNewLine)
                msg.AppendFormat("Paziente:{0}{1}", codicePaziente, vbNewLine)
                msg.AppendFormat("Codice Vaccinazione:{0}", codiceVaccinazione)

                Throw New Exception(msg.ToString(), ex)

            Finally

                ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

        End Function

        Public Function ExistsVaccinazioneProgrammataByConvocazione(codicePaziente As Integer, codiceVaccinazione As String, dataConvocazione As DateTime) As Boolean Implements IVaccinazioneProgrammataProvider.ExistsVaccinazioneProgrammataByConvocazione

            Using cmd As New OracleClient.OracleCommand("SELECT 1 FROM T_VAC_PROGRAMMATE WHERE VPR_PAZ_CODICE = :codicePaziente AND VPR_VAC_CODICE = :codiceVaccinazione AND VPR_CNV_DATA = :dataConvocazione", Connection)

                cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)
                cmd.Parameters.AddWithValue("codiceVaccinazione", codiceVaccinazione)
                cmd.Parameters.AddWithValue("dataConvocazione", dataConvocazione)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Return Not cmd.ExecuteScalar() Is Nothing

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

        End Function

        Function GetVaccinazioneProgrammata(codicePaziente As Integer, codiceVaccinazione As String, dataConvocazione As DateTime?) As List(Of VaccinazioneProgrammata) Implements IVaccinazioneProgrammataProvider.GetVaccinazioneProgrammata

            Dim list As List(Of VaccinazioneProgrammata) = Nothing

            Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

                cmd.Parameters.AddWithValue("VPR_PAZ_CODICE", codicePaziente)
                cmd.Parameters.AddWithValue("VPR_VAC_CODICE", codiceVaccinazione)

                If dataConvocazione.HasValue Then
                    cmd.Parameters.AddWithValue("VPR_CNV_DATA", dataConvocazione)
                    cmd.CommandText = Queries.VaccinazioniProgrammate.OracleQueries.selVaccinazioneConvocazione
                Else
                    cmd.CommandText = Queries.VaccinazioniProgrammate.OracleQueries.selVaccinazione
                End If

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    list = GetListVaccinazioniProgrammate(cmd)

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list

        End Function

        Public Function GetVaccinazioniProgrammatePaziente(codicePaziente As Integer) As IEnumerable(Of VaccinazioneProgrammata) Implements IVaccinazioneProgrammataProvider.GetVaccinazioniProgrammatePaziente

            Dim list As List(Of VaccinazioneProgrammata) = Nothing

            Using cmd As OracleClient.OracleCommand = New OracleClient.OracleCommand("SELECT * FROM T_VAC_PROGRAMMATE WHERE VPR_PAZ_CODICE = :VPR_PAZ_CODICE", Connection)

                cmd.Parameters.AddWithValue("VPR_PAZ_CODICE", codicePaziente)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    list = GetListVaccinazioniProgrammate(cmd)

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list.AsEnumerable()

        End Function

        ''' <summary>
        ''' Restituisce le programmate relative alla convocazione.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataConvocazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetVaccinazioniProgrammateByCnv(codicePaziente As Long, dataConvocazione As DateTime) As List(Of VaccinazioneProgrammata) Implements IVaccinazioneProgrammataProvider.GetVaccinazioniProgrammateByCnv

            Dim list As List(Of VaccinazioneProgrammata) = Nothing

            Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

                cmd.CommandText = "select * from t_vac_programmate where vpr_paz_codice = :vpr_paz_codice and vpr_cnv_data = :vpr_cnv_data "

                cmd.Parameters.AddWithValue("vpr_paz_codice", codicePaziente)
                cmd.Parameters.AddWithValue("vpr_cnv_data", dataConvocazione)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    list = GetListVaccinazioniProgrammate(cmd)

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list

        End Function

        Private Function GetListVaccinazioniProgrammate(cmd As OracleClient.OracleCommand) As List(Of VaccinazioneProgrammata)

            Dim list As New List(Of VaccinazioneProgrammata)()

            Using idr As IDataReader = cmd.ExecuteReader()

                If Not idr Is Nothing Then

                    Dim vpr_paz_codice As Integer = idr.GetOrdinal("VPR_PAZ_CODICE")
                    Dim vpr_cnv_data As Integer = idr.GetOrdinal("VPR_CNV_DATA")
                    Dim vpr_vac_codice As Integer = idr.GetOrdinal("VPR_VAC_CODICE")
                    Dim vpr_n_richiamo As Integer = idr.GetOrdinal("VPR_N_RICHIAMO")
                    Dim vpr_cic_codice As Integer = idr.GetOrdinal("VPR_CIC_CODICE")
                    Dim vpr_n_seduta As Integer = idr.GetOrdinal("VPR_N_SEDUTA")
                    Dim vpr_paz_codice_old As Integer = idr.GetOrdinal("VPR_PAZ_CODICE_OLD")
                    Dim vpr_ass_codice As Integer = idr.GetOrdinal("VPR_ASS_CODICE")
                    Dim vpr_data_inserimento As Integer = idr.GetOrdinal("VPR_DATA_INSERIMENTO")
                    Dim vpr_ute_id_inserimento As Integer = idr.GetOrdinal("VPR_UTE_ID_INSERIMENTO")

                    While idr.Read()

                        Dim item As New VaccinazioneProgrammata()
                        item.CodicePaziente = idr.GetInt64(vpr_paz_codice)
                        item.DataConvocazione = idr.GetDateTime(vpr_cnv_data)
                        item.CodiceVaccinazione = idr.GetString(vpr_vac_codice)
                        item.NumeroRichiamo = idr.GetNullableInt32OrDefault(vpr_n_richiamo)
                        item.CodiceCiclo = idr.GetStringOrDefault(vpr_cic_codice)
                        item.NumeroSeduta = idr.GetNullableInt32OrDefault(vpr_n_seduta)
                        item.CodicePazientePrecedente = idr.GetNullableInt64OrDefault(vpr_paz_codice_old)
                        item.CodiceAssociazione = idr.GetStringOrDefault(vpr_ass_codice)
                        item.DataInserimento = idr.GetNullableDateTimeOrDefault(vpr_data_inserimento)
                        item.IdUtenteInserimento = idr.GetNullableInt64OrDefault(vpr_ute_id_inserimento)

                        list.Add(item)

                    End While

                End If

            End Using

            Return list

        End Function

        Public Function GetVaccinazioniProgrammateAssociate(dataInizioApp As DateTime, dataFineApp As DateTime, dataInizioNascita As DateTime?, dataFineNascita As DateTime?, codiceMedico As String, codiceStatoAnagrafico As String, codiceVaccini As String, numeroDose As Integer?, codiceConsultorio As List(Of String)) As List(Of StatVaccinazioneProgrammateAssociate) Implements IVaccinazioneProgrammataProvider.GetVaccinazioniProgrammateAssociate

            Dim list As New List(Of StatVaccinazioneProgrammateAssociate)()

            Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

                Dim selectText As String =
                    " SELECT PAZ_CODICE,T_CNV_CONVOCAZIONI.CNV_DATA,T_CNV_CONVOCAZIONI.CNV_DATA_APPUNTAMENTO,T_CNV_CONVOCAZIONI.CNV_CNS_CODICE " +
                    " FROM T_CNV_CONVOCAZIONI " +
                    " INNER JOIN T_PAZ_PAZIENTI ON T_PAZ_PAZIENTI.PAZ_CODICE = CNV_PAZ_CODICE " +
                    " WHERE T_CNV_CONVOCAZIONI.CNV_DATA_APPUNTAMENTO >= :DATA_INIZIO_APP " +
                    " AND T_CNV_CONVOCAZIONI.CNV_DATA_APPUNTAMENTO < :DATA_FINE_APP "

                cmd.Parameters.AddWithValue("DATA_INIZIO_APP", dataInizioApp.Date)
                cmd.Parameters.AddWithValue("DATA_FINE_APP", dataFineApp.Date.AddDays(1))

                If dataInizioNascita > DateTime.MinValue And dataFineNascita > DateTime.MinValue Then
                    selectText += " AND T_PAZ_PAZIENTI.PAZ_DATA_NASCITA >= :DATA_INIZIO AND T_PAZ_PAZIENTI.PAZ_DATA_NASCITA <=:DATA_FINE "
                    cmd.Parameters.AddWithValue("DATA_INIZIO", dataInizioNascita.Value)
                    cmd.Parameters.AddWithValue("DATA_FINE", dataFineNascita.Value)
                End If

                If Not codiceConsultorio.IsNullOrEmpty() Then
                    Dim res As GetInFilterResult = GetInFilter(codiceConsultorio)
                    selectText += " AND T_CNV_CONVOCAZIONI.CNV_CNS_CODICE IN (" + res.InFilter + ") "
                    cmd.Parameters.AddRange(res.Parameters)
                End If

                If Not String.IsNullOrWhiteSpace(codiceMedico) Then
                    selectText += " AND T_PAZ_PAZIENTI.PAZ_MED_CODICE_BASE = :COD_MEDICO "
                    cmd.Parameters.AddWithValue("COD_MEDICO", codiceMedico)
                End If

                If Not String.IsNullOrWhiteSpace(codiceStatoAnagrafico) Then
                    selectText += " AND T_PAZ_PAZIENTI.PAZ_STATO_ANAGRAFICO IN (" + codiceStatoAnagrafico + ") "
                End If

                If Not String.IsNullOrWhiteSpace(codiceVaccini) OrElse (numeroDose.HasValue AndAlso numeroDose > 0) Then

                    Dim filtroVac As String = String.Empty
                    If Not String.IsNullOrWhiteSpace(codiceVaccini) Then
                        filtroVac = " AND T.VPR_VAC_CODICE IN (" + codiceVaccini + ")"
                    End If

                    Dim filtroDose As String = String.Empty
                    If numeroDose.HasValue AndAlso numeroDose > 0 Then
                        filtroDose = " AND T.VPR_N_RICHIAMO = :DOSE"
                        cmd.Parameters.AddWithValue("DOSE", numeroDose.Value)
                    End If

                    selectText += String.Format(" AND EXISTS (SELECT 1 FROM T_VAC_PROGRAMMATE T WHERE T.VPR_PAZ_CODICE = T_CNV_CONVOCAZIONI.CNV_PAZ_CODICE AND T.VPR_CNV_DATA=T_CNV_CONVOCAZIONI.CNV_DATA {0} {1})", filtroVac, filtroDose)

                End If

                cmd.CommandText = selectText

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim vpr_paz_codice As Integer = idr.GetOrdinal("PAZ_CODICE")
                            Dim vpr_cnv_data As Integer = idr.GetOrdinal("CNV_DATA")
                            Dim vpr_cnv_cns_codice As Integer = idr.GetOrdinal("CNV_CNS_CODICE")

                            While idr.Read()

                                Dim item As New StatVaccinazioneProgrammateAssociate()
                                item.CodicePaziente = idr.GetInt64(vpr_paz_codice)
                                item.DataConvocazione = idr.GetDateTime(vpr_cnv_data)
                                item.CodiceConsultorio = idr.GetString(vpr_cnv_cns_codice)

                                Dim appList As List(Of VaccinazioneProgrammata) =
                                    GetVaccinazioniProgrammateByCnv(item.CodicePaziente, item.DataConvocazione).ToList()

                                item.ListaVacProgrammate = appList.Select(Function(p) p.CodiceVaccinazione).ToList()
                                item.ListaAssProgrammate = appList.Select(Function(p) p.CodiceAssociazione).Distinct().ToList()
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

#End Region

#Region " Metodi di Delete "

        ''' <summary>
        ''' Conteggio convocazioni da eliminare per il paziente, in base ai parametri impostati
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataConvocazione"></param>
        ''' <param name="eliminaAppuntamenti"></param>
        ''' <param name="eliminaSollecitiBilancio"></param>
        ''' <param name="eliminaBilanci"></param>
        ''' <returns></returns>
        Public Function CountProgrammazioneDaEliminare(codicePaziente As Integer, dataConvocazione As Date?, eliminaAppuntamenti As Boolean, eliminaSollecitiBilancio As Boolean, eliminaBilanci As Boolean) As Integer Implements IVaccinazioneProgrammataProvider.CountProgrammazioneDaEliminare

            Dim countProg As Integer = 0

            ' Creazione, in base ai parametri specificati, della query da utilizzare 
            ' per selezionare la programmazione che verrà eliminata
            Using cmd As New OracleClient.OracleCommand()

                Dim ownConnection As Boolean = False

                Try
                    ' Creazione della query di conteggio della programmazione da eliminare
                    SetQueryProgrammazioneDaEliminare(cmd, codicePaziente, dataConvocazione, eliminaAppuntamenti, eliminaSollecitiBilancio, eliminaBilanci, Nothing, True)

                    cmd.Connection = Connection

                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If obj Is Nothing OrElse obj Is DBNull.Value Then
                        countProg = 0
                    Else
                        countProg = Convert.ToInt32(obj)
                    End If

                Catch ex As Exception

                    Dim msg As New Text.StringBuilder()

                    msg.AppendFormat("DAL: Errore conteggio programmazione da eliminare (VaccinazioneProg.CountProgrammazioneDaEliminare){0}", vbNewLine)
                    msg.AppendFormat("Paziente:{0}{1}", codicePaziente, vbNewLine)
                    msg.AppendFormat("Eliminazione Appuntamenti:{0}{1}", eliminaAppuntamenti, vbNewLine)
                    msg.AppendFormat("Eliminazione Solleciti:{0}{1}", eliminaSollecitiBilancio, vbNewLine)
                    msg.AppendFormat("Eliminazione Bilanci:{0}", eliminaBilanci)

                    Throw New Exception(msg.ToString(), ex)

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return countProg

        End Function

        ''' <summary>
        ''' Restituisce una lista di convocazioni da eliminare per il paziente, in base ai parametri impostati
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataConvocazione"></param>
        ''' <param name="eliminaAppuntamenti"></param>
        ''' <param name="eliminaSollecitiBilancio"></param>
        ''' <param name="eliminaBilanci"></param>
        ''' <param name="codiciCicli"></param>
        ''' <returns></returns>
        Public Function GetProgrammazioneDaEliminare(codicePaziente As Integer, dataConvocazione As Date?, eliminaAppuntamenti As Boolean, eliminaSollecitiBilancio As Boolean, eliminaBilanci As Boolean, codiciCicli As List(Of String)) As List(Of ProgrammazioneDaEliminare) Implements IVaccinazioneProgrammataProvider.GetProgrammazioneDaEliminare

            Dim listProgrammazioniDaEliminare As List(Of ProgrammazioneDaEliminare) = Nothing

            ' Creazione, in base ai parametri specificati, della query da utilizzare 
            ' per selezionare la programmazione che verrà eliminata
            Using cmd As New OracleClient.OracleCommand()

                Dim ownConnection As Boolean = False

                Try
                    ' Creazione della query di ricerca della programmazione da eliminare
                    SetQueryProgrammazioneDaEliminare(cmd, codicePaziente, dataConvocazione, eliminaAppuntamenti, eliminaSollecitiBilancio, eliminaBilanci, codiciCicli, False)

                    cmd.Connection = Connection

                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            listProgrammazioniDaEliminare = New List(Of ProgrammazioneDaEliminare)()

                            Dim programmazioneDaEliminare As ProgrammazioneDaEliminare = Nothing

                            Dim pos_paz As Integer = idr.GetOrdinal("cnv_paz_codice")
                            Dim pos_cnv As Integer = idr.GetOrdinal("cnv_data")

                            While idr.Read()

                                programmazioneDaEliminare = New ProgrammazioneDaEliminare() With
                                {
                                    .CodicePaziente = idr.GetDecimal(pos_paz),
                                    .DataConvocazione = idr.GetDateTime(pos_cnv)
                                }

                                listProgrammazioniDaEliminare.Add(programmazioneDaEliminare)

                            End While

                        End If

                    End Using

                Catch ex As Exception

                    Dim msg As New Text.StringBuilder()

                    msg.AppendFormat("DAL: Errore ricerca programmazione da eliminare (VaccinazioneProg.GetProgrammazioneDaEliminare){0}", vbNewLine)
                    msg.AppendFormat("Paziente:{0}{1}", codicePaziente, vbNewLine)
                    msg.AppendFormat("Eliminazione Appuntamenti:{0}{1}", eliminaAppuntamenti, vbNewLine)
                    msg.AppendFormat("Eliminazione Solleciti:{0}{1}", eliminaSollecitiBilancio, vbNewLine)
                    msg.AppendFormat("Eliminazione Bilanci:{0}", eliminaBilanci)

                    Throw New Exception(msg.ToString(), ex)

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listProgrammazioniDaEliminare

        End Function

        ''' <summary>
        ''' Elimina la vaccinazione programmata del paziente in data specificata. 
        ''' Se la data non è valorizzata, elimina tutte le programmate del paziente con codice della vaccinazione specificato.
        ''' Restituisce il numero di record eliminati.
        ''' </summary>
        Function EliminaVaccinazioneProgrammata(codicePaziente As Integer, codiceVaccinazione As String, dataConvocazione As Nullable(Of DateTime)) As Integer Implements IVaccinazioneProgrammataProvider.EliminaVaccinazioneProgrammata
            '--
            Dim cmd As OracleClient.OracleCommand = Nothing
            '--
            Dim ownConnection As Boolean = False
            '--
            Try
                '--
                cmd = Me.Connection.CreateCommand()
                '--
                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                cmd.Parameters.AddWithValue("cod_vac", codiceVaccinazione)
                '--
                If dataConvocazione.HasValue Then
                    '--
                    cmd.Parameters.AddWithValue("dat_cnv", dataConvocazione)
                    '--
                    cmd.CommandText = Queries.VaccinazioniProgrammate.OracleQueries.delVaccinazioneConvocazione
                    '--
                Else
                    '--
                    cmd.CommandText = Queries.VaccinazioniProgrammate.OracleQueries.delVaccinazione
                    '--
                End If
                '--
                ownConnection = Me.ConditionalOpenConnection(cmd)
                '--
                Return cmd.ExecuteNonQuery()
                '--
            Finally
                '--
                Me.ConditionalCloseConnection(ownConnection)
                '--
                If Not cmd Is Nothing Then cmd.Dispose()
                '--
            End Try
            '--
        End Function

        Function EliminaVaccinazioneProgrammataByRichiamo(codicePaziente As Integer, codiceVaccinazione As String, numeroRichiamo As Int32) As Int32 Implements IVaccinazioneProgrammataProvider.EliminaVaccinazioneProgrammataByRichiamo
            '--
            Dim cmd As OracleClient.OracleCommand = Nothing
            '--
            Dim ownConnection As Boolean = False
            '--
            Try
                '--
                cmd = Me.Connection.CreateCommand()
                '--
                cmd.CommandText = Queries.VaccinazioniProgrammate.OracleQueries.delVaccinazioneByRichiamo
                '--
                cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)
                cmd.Parameters.AddWithValue("codiceVaccinazione", codiceVaccinazione)
                cmd.Parameters.AddWithValue("numeroRichiamo", numeroRichiamo)
                '--
                ownConnection = Me.ConditionalOpenConnection(cmd)
                '--
                Return cmd.ExecuteNonQuery()
                '--
            Finally
                '--
                Me.ConditionalCloseConnection(ownConnection)
                '--
                If Not cmd Is Nothing Then cmd.Dispose()
                '--
            End Try
            '--
        End Function

        Private Sub SetQueryProgrammazioneDaEliminare(ByRef cmd As OracleClient.OracleCommand, codicePaziente As Integer, dataConvocazione As DateTime?, eliminaAppuntamenti As Boolean, eliminaSolleciti As Boolean, eliminaBilanci As Boolean, codiciCicli As List(Of String), count As Boolean)

            ' Creazione della query di ricerca della programmazione da eliminare
            Dim query As New Text.StringBuilder()

            ' Query principale
            If count Then
                query.Append(Queries.VaccinazioniProgrammate.OracleQueries.countProgrammazioneDaEliminare)
            Else
                query.Append(Queries.VaccinazioniProgrammate.OracleQueries.selProgrammazioneDaEliminare)
            End If

            cmd.Parameters.AddWithValue("cod_paz", codicePaziente)

            ' Filtro: cancellazione della sola convocazione in data specificata
            If dataConvocazione > Date.MinValue Then
                query.Append(" and cnv_data = :cnv_data")
                cmd.Parameters.AddWithValue("cnv_data", dataConvocazione)
            End If

            ' Filtro: cancellazione programmazione senza appuntamenti assegnati
            If Not eliminaAppuntamenti Then
                query.Append(" and cnv_data_appuntamento is null and cnv_data_invio is null")
            End If

            ' Filtro controllo solleciti: cancellazione programmazione senza solleciti associati
            If Not eliminaSolleciti Then
                query.AppendFormat(" and not exists ({0})", Queries.VaccinazioniProgrammate.OracleQueries.selProgrElim_subQuerySolleciti)
            End If

            ' Filtro controllo bilanci: cancellazione programmazione senza bilanci associati
            If Not eliminaBilanci Then
                query.AppendFormat(" and not exists ({0})", Queries.VaccinazioniProgrammate.OracleQueries.selProgrElim_subQueryBilanci)
            End If

            ' Filtro cicli: cancellazione programmazione solo relativa ai cicli specificati
            If Not codiciCicli.IsNullOrEmpty() Then

                Dim queryCicli As String

                If codiciCicli.Count = 1 Then

                    queryCicli = String.Format(Queries.VaccinazioniProgrammate.OracleQueries.selProgElim_subQueryCicli, " = :cic_codice ")
                    cmd.Parameters.AddWithValue("cic_codice", codiciCicli.Single())

                Else

                    Dim filtroCicli As GetInFilterResult = GetInFilter(codiciCicli)

                    queryCicli = String.Format(Queries.VaccinazioniProgrammate.OracleQueries.selProgElim_subQueryCicli, " IN ( " + filtroCicli.InFilter + ") ")
                    cmd.Parameters.AddRange(filtroCicli.Parameters)

                End If

                query.AppendFormat(" and exists ({0})", queryCicli)

            End If

            cmd.CommandText = query.ToString()

        End Sub

#End Region

#Region " Insert / Update "

        Public Function InsertVaccinazioneProgrammata(codicePaziente As Integer, dataConvocazione As DateTime, codiceVaccinazione As String, codiceAssociazione As String, numeroRichiamo As Integer, dataInserimento As DateTime, idUtenteInserimento As Long) As Integer Implements IVaccinazioneProgrammataProvider.InsertVaccinazioneProgrammata

            With _DAM.QB

                .NewQuery()

                .AddTables("T_VAC_PROGRAMMATE")

                .AddInsertField("VPR_PAZ_CODICE", codicePaziente, DataTypes.Numero)
                .AddInsertField("VPR_CNV_DATA", dataConvocazione, DataTypes.Data)
                .AddInsertField("VPR_VAC_CODICE", codiceVaccinazione, DataTypes.Stringa)
                .AddInsertField("VPR_ASS_CODICE", GetStringParam(codiceAssociazione), DataTypes.Stringa)
                .AddInsertField("VPR_N_RICHIAMO", numeroRichiamo, DataTypes.Numero)
                .AddInsertField("VPR_DATA_INSERIMENTO", dataInserimento, DataTypes.DataOra)
                .AddInsertField("VPR_UTE_ID_INSERIMENTO", idUtenteInserimento, DataTypes.Numero)

            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Insert)

        End Function

        Public Sub InsertVaccinazioneProgrammata(vaccinazioneProgrammata As Entities.VaccinazioneProgrammata) Implements IVaccinazioneProgrammataProvider.InsertVaccinazioneProgrammata

            With _DAM.QB

                .NewQuery()

                .AddTables("T_VAC_PROGRAMMATE")

                .AddInsertField("VPR_PAZ_CODICE", vaccinazioneProgrammata.CodicePaziente, DataTypes.Numero)
                .AddInsertField("VPR_CNV_DATA", vaccinazioneProgrammata.DataConvocazione, DataTypes.Data)
                .AddInsertField("VPR_VAC_CODICE", vaccinazioneProgrammata.CodiceVaccinazione, DataTypes.Stringa)
                .AddInsertField("VPR_CIC_CODICE", GetStringParam(vaccinazioneProgrammata.CodiceCiclo), DataTypes.Stringa)

                If vaccinazioneProgrammata.NumeroSeduta.HasValue Then
                    .AddInsertField("VPR_N_SEDUTA", vaccinazioneProgrammata.NumeroSeduta, DataTypes.Stringa)
                Else
                    .AddInsertField("VPR_N_SEDUTA", DBNull.Value, DataTypes.Replace)
                End If

                .AddInsertField("VPR_ASS_CODICE", GetStringParam(vaccinazioneProgrammata.CodiceAssociazione), DataTypes.Stringa)
                .AddInsertField("VPR_N_RICHIAMO", GetIntParam(vaccinazioneProgrammata.NumeroRichiamo), DataTypes.Numero)

                .AddInsertField("VPR_DATA_INSERIMENTO", GetDateParam(vaccinazioneProgrammata.DataInserimento), DataTypes.DataOra)
                .AddInsertField("VPR_UTE_ID_INSERIMENTO", GetLongParam(vaccinazioneProgrammata.IdUtenteInserimento), DataTypes.Numero)

            End With

            _DAM.ExecNonQuery(ExecQueryType.Insert)

        End Sub

        Public Function UpdateVaccinazioneProgrammata(vaccinazioneProgrammata As Entities.VaccinazioneProgrammata) As Boolean Implements IVaccinazioneProgrammataProvider.UpdateVaccinazioneProgrammata

            With _DAM.QB

                .NewQuery()

                .AddTables("T_VAC_PROGRAMMATE")

                .AddUpdateField("VPR_CIC_CODICE", GetStringParam(vaccinazioneProgrammata.CodiceCiclo), DataTypes.Stringa)

                If vaccinazioneProgrammata.NumeroSeduta.HasValue Then
                    .AddUpdateField("VPR_N_SEDUTA", vaccinazioneProgrammata.NumeroSeduta, DataTypes.Stringa)
                Else
                    .AddUpdateField("VPR_N_SEDUTA", DBNull.Value, DataTypes.Replace)
                End If

                .AddUpdateField("VPR_ASS_CODICE", GetStringParam(vaccinazioneProgrammata.CodiceAssociazione), DataTypes.Stringa)
                .AddUpdateField("VPR_N_RICHIAMO", GetIntParam(vaccinazioneProgrammata.NumeroRichiamo), DataTypes.Numero)

                .AddWhereCondition("VPR_PAZ_CODICE", Comparatori.Uguale, vaccinazioneProgrammata.CodicePaziente, DataTypes.Numero)
                .AddWhereCondition("VPR_CNV_DATA", Comparatori.Uguale, vaccinazioneProgrammata.DataConvocazione, DataTypes.Data)
                .AddWhereCondition("VPR_VAC_CODICE", Comparatori.Uguale, vaccinazioneProgrammata.CodiceVaccinazione, DataTypes.Stringa)

            End With

            Return (_DAM.ExecNonQuery(ExecQueryType.Update) > 0)

        End Function

        ''' <summary>Update della data di convocazione delle vacc prog associate alla cnv unita.
        '''  Restituisce il numero di record aggiornati.</summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="oldDataConvocazione"></param>
        ''' <param name="newDataConvocazione"></param>
        Public Function UpdateDataCnv(codicePaziente As Integer, oldDataConvocazione As Date, newDataConvocazione As Date) As Integer Implements IVaccinazioneProgrammataProvider.UpdateDataCnv

            Dim count As Integer = 0

            Dim ownConnection As Boolean = False

            Using cmd As New OracleClient.OracleCommand(Queries.Convocazioni.OracleQueries.updCnvUnita_VaccProg, Me.Connection)

                cmd.Parameters.AddWithValue("new_data_cnv", newDataConvocazione)
                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                cmd.Parameters.AddWithValue("old_data_cnv", oldDataConvocazione)

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
