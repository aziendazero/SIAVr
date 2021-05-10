Imports System.Collections.Generic
Imports System.Data.OracleClient
Imports Onit.Database.DataAccessManager

Namespace DAL.Oracle

    Public Class DBVaccinazioniEscluseProvider
        Inherits DbProvider
        Implements IVaccinazioniEscluseProvider

#Region " Constructors "

        Public Sub New(DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " Caricamento "

        ''' <summary>
        ''' Restituisce i dati della vaccinazione esclusa per il paziente e la vaccinazione specificati
        ''' </summary>
        Public Function GetVaccinazioneEsclusaPaziente(codicePaziente As Integer, codiceVaccinazione As String) As Entities.VaccinazioneEsclusa Implements IVaccinazioniEscluseProvider.GetVaccinazioneEsclusaPaziente

            Dim vaccinazioneEsclusa As Entities.VaccinazioneEsclusa = Nothing

            Using cmd As New OracleClient.OracleCommand(Queries.VaccinazioniEscluse.OracleQueries.selVaccinazioneEsclusaPaziente, Me.Connection)

                cmd.Parameters.AddWithValue("pazCodice", codicePaziente)
                cmd.Parameters.AddWithValue("vacCodice", codiceVaccinazione)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        vaccinazioneEsclusa = Me.GetListVaccinazioniEscluse(idr).FirstOrDefault()

                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return vaccinazioneEsclusa

        End Function

        Public Function GetVaccinazioneEsclusaByIdIfExists(idVaccinazioneEsclusa As Long) As Entities.VaccinazioneEsclusa Implements IVaccinazioniEscluseProvider.GetVaccinazioneEsclusaByIdIfExists

            Dim vaccinazioneEsclusa As Entities.VaccinazioneEsclusa = Nothing

            Using cmd As New OracleClient.OracleCommand(Queries.VaccinazioniEscluse.OracleQueries.selVaccinazioneEsclusaById, Me.Connection)

                cmd.Parameters.AddWithValue("vex_id", idVaccinazioneEsclusa)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        vaccinazioneEsclusa = Me.GetListVaccinazioniEscluse(idr).FirstOrDefault()

                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return vaccinazioneEsclusa

        End Function

        ''' <summary>
        ''' Restituisce i dati della vaccinazione esclusa corrispondente al vex_id specificato
        ''' </summary>
        Public Function GetVaccinazioneEsclusaById(idVaccinazioneEsclusa As Long) As VaccinazioneEsclusa Implements IVaccinazioniEscluseProvider.GetVaccinazioneEsclusaById

            Return GetVaccinazioneEsclusaByIdIfExists(idVaccinazioneEsclusa)

        End Function

        ''' <summary>
        ''' Restituisce i dati della vaccinazione esclusa eliminata corrispondente al vxe_id specificato
        ''' </summary>
        Public Function GetVaccinazioneEsclusaEliminataById(idVaccinazioneEsclusaEliminata As Int64) As VaccinazioneEsclusa Implements IVaccinazioniEscluseProvider.GetVaccinazioneEsclusaEliminataById

            Dim vaccinazioneEsclusaEliminata As VaccinazioneEsclusa = Nothing

            Using cmd As New OracleCommand(Queries.VaccinazioniEscluse.OracleQueries.selVaccinazioneEsclusaEliminataById, Connection)

                cmd.Parameters.AddWithValue("vxe_id", idVaccinazioneEsclusaEliminata)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        vaccinazioneEsclusaEliminata = GetListVaccinazioniEscluseEliminate(idr).First()

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return vaccinazioneEsclusaEliminata

        End Function

        ''' <summary>
        ''' Restituisce le vaccinazioni escluse eliminate filtrate per paziente e per vaccinazione, in modo da creare uno storico dei rinnovi della vaccinazione esclusa
        ''' </summary>
        Public Function GetVaccinazioneEsclusaEliminataByPazienteVaccinazione(pazCodice As Integer, codiceVaccinazione As String, sortColumn As String, filtraRinnovate As Boolean) As List(Of Entities.VaccinazioneEsclusaDettaglio) Implements IVaccinazioniEscluseProvider.GetVaccinazioneEsclusaEliminataByPazienteVaccinazione

            Dim vaccinazioniEscluseEliminate As List(Of Entities.VaccinazioneEsclusaDettaglio) = Nothing

            Using cmd As New OracleClient.OracleCommand()

                'cmd.CommandText = Queries.VaccinazioniEscluse.OracleQueries.selVaccinazioneEsclusaEliminataByPazienteVaccinazione
                cmd.Connection = Me.Connection
                cmd.Parameters.AddWithValue("vxe_paz_codice", pazCodice)
                cmd.Parameters.AddWithValue("vxe_vac_codice", codiceVaccinazione)

                Dim query As String = Queries.VaccinazioniEscluse.OracleQueries.selVaccinazioneEsclusaEliminataByPazienteVaccinazione

                If filtraRinnovate Then
                    query += String.Format(" and  vxe_stato_eliminazione = '{0}' ", Constants.StatoVaccinazioniEscluseEliminate.Rinnovata)
                End If

                If Not String.IsNullOrWhiteSpace(sortColumn) Then
                    query = String.Format(" {0} order by {1} ", query, sortColumn)
                Else
                    query = String.Format(" {0} order by {1} ", query, " vxe_data_visita desc , vxe_data_scadenza desc ")
                End If

                cmd.CommandText = query


                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        vaccinazioniEscluseEliminate = Me.GetListVaccinazioniEscluseEliminateDettaglio(idr)

                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return vaccinazioniEscluseEliminate

        End Function

        Public Function GetVaccinazioniEsclusePaziente(codicePaziente As Integer) As List(Of VaccinazioneEsclusa) Implements IVaccinazioniEscluseProvider.GetVaccinazioniEsclusePaziente

            Dim listVaccinazioniEscluse As List(Of VaccinazioneEsclusa)

            Using cmd As New OracleCommand()

                cmd.Connection = Connection

                Dim query As New Text.StringBuilder(Queries.VaccinazioniEscluse.OracleQueries.selVaccinazioniEsclusePaziente)

                cmd.Parameters.AddWithValue("pazCodice", codicePaziente)

                query.Append(" order by vex_data_visita, vex_vac_codice, vex_data_scadenza, moe_descrizione")

                cmd.CommandText = query.ToString()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        listVaccinazioniEscluse = GetListVaccinazioniEscluse(idr)

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listVaccinazioniEscluse

        End Function

        ''' <summary>
        ''' Creazione lista vaccinazioni
        ''' </summary>
        Private Function GetListVaccinazioniEscluse(idr As IDataReader) As List(Of VaccinazioneEsclusa)

            Dim listVaccinazioniEscluse As New List(Of VaccinazioneEsclusa)()

            If Not idr Is Nothing Then

                Dim vex_paz_codice As Integer = idr.GetOrdinal("vex_paz_codice")
                Dim vex_data_visita As Integer = idr.GetOrdinal("vex_data_visita")
                Dim vex_vac_codice As Integer = idr.GetOrdinal("vex_vac_codice")
                Dim vex_moe_codice As Integer = idr.GetOrdinal("vex_moe_codice")
                Dim moe_descrizione As Integer = idr.GetOrdinal("moe_descrizione")
                Dim vex_ope_codice As Integer = idr.GetOrdinal("vex_ope_codice")
                Dim ope_nome As Integer = idr.GetOrdinal("ope_nome")
                Dim vex_data_scadenza As Integer = idr.GetOrdinal("vex_data_scadenza")
                Dim vex_paz_codice_old As Integer = idr.GetOrdinal("vex_paz_codice_old")
                Dim vex_id As Integer = idr.GetOrdinal("vex_id")
                Dim vex_usl_inserimento As Integer = idr.GetOrdinal("vex_usl_inserimento")
                Dim vex_data_registrazione As Integer = idr.GetOrdinal("vex_data_registrazione")
                Dim vex_ute_id_registrazione As Integer = idr.GetOrdinal("vex_ute_id_registrazione")
                Dim vex_data_variazione As Integer = idr.GetOrdinal("vex_data_variazione")
                Dim vex_ute_id_variazione As Integer = idr.GetOrdinal("vex_ute_id_variazione")
                Dim vex_flag_visibilita As Integer = idr.GetOrdinal("vex_flag_visibilita")
                Dim vex_dose As Integer = idr.GetOrdinal("vex_dose")
                Dim vex_note As Integer = idr.GetOrdinal("vex_note")

                Dim vaccinazioneEsclusa As VaccinazioneEsclusa = Nothing

                While idr.Read()

                    vaccinazioneEsclusa = New VaccinazioneEsclusa()

                    vaccinazioneEsclusa.CodicePaziente = idr.GetDecimal(vex_paz_codice)
                    vaccinazioneEsclusa.DataVisita = idr.GetDateTimeOrDefault(vex_data_visita)
                    vaccinazioneEsclusa.CodiceVaccinazione = idr.GetString(vex_vac_codice)
                    vaccinazioneEsclusa.CodiceMotivoEsclusione = idr.GetStringOrDefault(vex_moe_codice)
                    vaccinazioneEsclusa.DescrizioneMotivoEsclusione = idr.GetStringOrDefault(moe_descrizione)
                    vaccinazioneEsclusa.CodiceOperatore = idr.GetStringOrDefault(vex_ope_codice)
                    vaccinazioneEsclusa.DescrizioneOperatore = idr.GetStringOrDefault(ope_nome)
                    vaccinazioneEsclusa.DataScadenza = idr.GetDateTimeOrDefault(vex_data_scadenza)
                    vaccinazioneEsclusa.CodicePazientePrecedente = idr.GetNullableInt32OrDefault(vex_paz_codice_old)
                    vaccinazioneEsclusa.Id = idr.GetDecimal(vex_id)
                    vaccinazioneEsclusa.DataRegistrazione = idr.GetDateTime(vex_data_registrazione)
                    vaccinazioneEsclusa.IdUtenteRegistrazione = idr.GetInt64(vex_ute_id_registrazione)
                    vaccinazioneEsclusa.DataModifica = idr.GetNullableDateTimeOrDefault(vex_data_variazione)
                    vaccinazioneEsclusa.IdUtenteModifica = idr.GetNullableInt64OrDefault(vex_ute_id_variazione)

                    vaccinazioneEsclusa.CodiceUslInserimento = idr.GetStringOrDefault(vex_usl_inserimento)
                    vaccinazioneEsclusa.FlagVisibilita = idr.GetStringOrDefault(vex_flag_visibilita)

                    vaccinazioneEsclusa.NumeroDose = idr.GetNullableInt32OrDefault(vex_dose)
                    vaccinazioneEsclusa.Note = idr.GetStringOrDefault(vex_note)

                    listVaccinazioniEscluse.Add(vaccinazioneEsclusa)

                End While

            End If

            Return listVaccinazioniEscluse

        End Function

        ''' <summary>
        ''' Creazione lista vaccinazioni eliminate
        ''' </summary>
        Private Function GetListVaccinazioniEscluseEliminate(idr As IDataReader) As List(Of VaccinazioneEsclusa)

            Dim listVaccinazioniEscluseEliminate As New List(Of VaccinazioneEsclusa)

            If Not idr Is Nothing Then

                Dim vxe_paz_codice As Integer = idr.GetOrdinal("vxe_paz_codice")
                Dim vxe_data_visita As Integer = idr.GetOrdinal("vxe_data_visita")
                Dim vxe_vac_codice As Integer = idr.GetOrdinal("vxe_vac_codice")
                Dim vxe_ope_codice As Integer = idr.GetOrdinal("vxe_ope_codice")
                Dim vxe_data_scadenza As Integer = idr.GetOrdinal("vxe_data_scadenza")
                Dim vxe_paz_codice_old As Integer = idr.GetOrdinal("vxe_paz_codice_old")
                Dim vxe_id As Integer = idr.GetOrdinal("vxe_id")
                Dim vxe_data_eliminazione As Integer = idr.GetOrdinal("vxe_data_eliminazione")
                Dim vxe_ute_id_eliminazione As Integer = idr.GetOrdinal("vxe_ute_id_eliminazione")
                Dim vxe_usl_inserimento As Integer = idr.GetOrdinal("vxe_usl_inserimento")
                Dim vxe_flag_visibilita As Integer = idr.GetOrdinal("vxe_flag_visibilita")
                Dim vxe_data_registrazione As Integer = idr.GetOrdinal("vxe_data_registrazione")
                Dim vxe_ute_id_registrazione As Integer = idr.GetOrdinal("vxe_ute_id_registrazione")
                Dim vxe_data_variazione As Integer = idr.GetOrdinal("vxe_data_variazione")
                Dim vxe_ute_id_variazione As Integer = idr.GetOrdinal("vxe_ute_id_variazione")
                Dim vxe_dose As Integer = idr.GetOrdinal("vxe_dose")
                Dim vxe_note As Integer = idr.GetOrdinal("vxe_note")

                Dim vaccinazioneEsclusa As VaccinazioneEsclusa = Nothing

                While idr.Read()

                    vaccinazioneEsclusa = New VaccinazioneEsclusa()

                    vaccinazioneEsclusa.CodicePaziente = idr.GetDecimal(vxe_paz_codice)
                    vaccinazioneEsclusa.DataVisita = idr.GetDateTimeOrDefault(vxe_data_visita)
                    vaccinazioneEsclusa.CodiceVaccinazione = idr.GetString(vxe_vac_codice)
                    vaccinazioneEsclusa.CodiceOperatore = idr.GetStringOrDefault(vxe_ope_codice)
                    vaccinazioneEsclusa.DataScadenza = idr.GetDateTimeOrDefault(vxe_data_scadenza)
                    vaccinazioneEsclusa.CodicePazientePrecedente = idr.GetNullableInt32OrDefault(vxe_paz_codice_old)
                    vaccinazioneEsclusa.Id = idr.GetDecimal(vxe_id)
                    vaccinazioneEsclusa.DataEliminazione = idr.GetNullableDateTimeOrDefault(vxe_data_eliminazione)
                    vaccinazioneEsclusa.IdUtenteEliminazione = idr.GetNullableInt64OrDefault(vxe_ute_id_eliminazione)
                    vaccinazioneEsclusa.CodiceUslInserimento = idr.GetStringOrDefault(vxe_usl_inserimento)
                    vaccinazioneEsclusa.FlagVisibilita = idr.GetStringOrDefault(vxe_flag_visibilita)
                    vaccinazioneEsclusa.DataRegistrazione = idr.GetDateTime(vxe_data_registrazione)
                    vaccinazioneEsclusa.IdUtenteRegistrazione = idr.GetInt64(vxe_ute_id_registrazione)
                    vaccinazioneEsclusa.DataModifica = idr.GetNullableDateTimeOrDefault(vxe_data_variazione)
                    vaccinazioneEsclusa.IdUtenteModifica = idr.GetNullableInt64OrDefault(vxe_ute_id_variazione)
                    vaccinazioneEsclusa.NumeroDose = idr.GetNullableInt32OrDefault(vxe_dose)
                    vaccinazioneEsclusa.Note = idr.GetStringOrDefault(vxe_note)

                    listVaccinazioniEscluseEliminate.Add(vaccinazioneEsclusa)

                End While

            End If

            Return listVaccinazioniEscluseEliminate

        End Function

        ''' <summary>
        ''' Creazione lista vaccinazioni eliminate con l'aggiunta delle join per ottenere le descrizioni dei campi
        ''' </summary>
        Private Function GetListVaccinazioniEscluseEliminateDettaglio(idr As IDataReader) As List(Of Entities.VaccinazioneEsclusaDettaglio)

            Dim listVaccinazioniEscluseEliminate As New List(Of Entities.VaccinazioneEsclusaDettaglio)

            If Not idr Is Nothing Then

                Dim vxe_paz_codice As Integer = idr.GetOrdinal("vxe_paz_codice")
                Dim vxe_data_visita As Integer = idr.GetOrdinal("vxe_data_visita")
                Dim vxe_vac_codice As Integer = idr.GetOrdinal("vxe_vac_codice")
                Dim vxe_ope_codice As Integer = idr.GetOrdinal("vxe_ope_codice")
                Dim vxe_data_scadenza As Integer = idr.GetOrdinal("vxe_data_scadenza")
                Dim vxe_paz_codice_old As Integer = idr.GetOrdinal("vxe_paz_codice_old")
                Dim vxe_id As Integer = idr.GetOrdinal("vxe_id")
                Dim vxe_data_eliminazione As Integer = idr.GetOrdinal("vxe_data_eliminazione")
                Dim vxe_ute_id_eliminazione As Integer = idr.GetOrdinal("vxe_ute_id_eliminazione")
                Dim vxe_usl_inserimento As Integer = idr.GetOrdinal("vxe_usl_inserimento")
                Dim vxe_flag_visibilita As Integer = idr.GetOrdinal("vxe_flag_visibilita")
                Dim vxe_data_registrazione As Integer = idr.GetOrdinal("vxe_data_registrazione")
                Dim vxe_ute_id_registrazione As Integer = idr.GetOrdinal("vxe_ute_id_registrazione")
                Dim vxe_data_variazione As Integer = idr.GetOrdinal("vxe_data_variazione")
                Dim vxe_ute_id_variazione As Integer = idr.GetOrdinal("vxe_ute_id_variazione")
                Dim vxe_stato_eliminazione As Integer = idr.GetOrdinal("vxe_stato_eliminazione")

                Dim vac_descrizione As Integer = idr.GetOrdinal("vac_descrizione")
                Dim ope_nome As Integer = idr.GetOrdinal("ope_nome")
                Dim ute_registrazione As Integer = idr.GetOrdinal("ute_registrazione")
                Dim ute_eliminazione As Integer = idr.GetOrdinal("ute_eliminazione")
                Dim ute_modifica As Integer = idr.GetOrdinal("ute_modifica")

                Dim vaccinazioneEsclusaEliminata As Entities.VaccinazioneEsclusaDettaglio = Nothing

                While idr.Read()

                    vaccinazioneEsclusaEliminata = New Entities.VaccinazioneEsclusaDettaglio()

                    vaccinazioneEsclusaEliminata.CodicePaziente = idr.GetDecimal(vxe_paz_codice)
                    vaccinazioneEsclusaEliminata.DataVisita = idr.GetDateTimeOrDefault(vxe_data_visita)
                    vaccinazioneEsclusaEliminata.CodiceVaccinazione = idr.GetString(vxe_vac_codice)
                    vaccinazioneEsclusaEliminata.CodiceOperatore = idr.GetStringOrDefault(vxe_ope_codice)
                    vaccinazioneEsclusaEliminata.DataScadenza = idr.GetDateTimeOrDefault(vxe_data_scadenza)
                    vaccinazioneEsclusaEliminata.CodicePazientePrecedente = idr.GetNullableInt32OrDefault(vxe_paz_codice_old)
                    vaccinazioneEsclusaEliminata.Id = idr.GetDecimal(vxe_id)
                    vaccinazioneEsclusaEliminata.DataEliminazione = idr.GetNullableDateTimeOrDefault(vxe_data_eliminazione)
                    vaccinazioneEsclusaEliminata.IdUtenteEliminazione = idr.GetNullableInt64OrDefault(vxe_ute_id_eliminazione)
                    vaccinazioneEsclusaEliminata.CodiceUslInserimento = idr.GetStringOrDefault(vxe_usl_inserimento)
                    vaccinazioneEsclusaEliminata.FlagVisibilita = idr.GetStringOrDefault(vxe_flag_visibilita)
                    vaccinazioneEsclusaEliminata.DataRegistrazione = idr.GetNullableDateTimeOrDefault(vxe_data_registrazione)
                    vaccinazioneEsclusaEliminata.IdUtenteRegistrazione = idr.GetInt64(vxe_ute_id_registrazione)
                    vaccinazioneEsclusaEliminata.DataModifica = idr.GetNullableDateTimeOrDefault(vxe_data_variazione)
                    vaccinazioneEsclusaEliminata.IdUtenteModifica = idr.GetNullableInt64OrDefault(vxe_ute_id_variazione)
                    vaccinazioneEsclusaEliminata.StatoEliminazione = idr.GetStringOrDefault(vxe_stato_eliminazione) 'idr.GetNullableEnumOrDefault(Of Constants.StatoVaccinazioniEscluseEliminate)(vxe_stato_eliminazione)

                    vaccinazioneEsclusaEliminata.DescrizioneVaccinazione = idr.GetStringOrDefault(vac_descrizione)
                    vaccinazioneEsclusaEliminata.DescrizioneOperatore = idr.GetStringOrDefault(ope_nome)
                    vaccinazioneEsclusaEliminata.UtenteRegistrazione = idr.GetStringOrDefault(ute_registrazione)
                    vaccinazioneEsclusaEliminata.UtenteEliminazione = idr.GetStringOrDefault(ute_eliminazione)
                    vaccinazioneEsclusaEliminata.UtenteModifica = idr.GetStringOrDefault(ute_modifica)

                    listVaccinazioniEscluseEliminate.Add(vaccinazioneEsclusaEliminata)

                End While

            End If

            Return listVaccinazioniEscluseEliminate

        End Function

        '''<summary>
        ''' Restituisce la data di esclusione per il paziente e la vaccinazione specificati, 
        ''' se l'esclusione non ha una data di scadenza passata rispetto alla convocazione. Altrimenti restituisce Date.MinValue.
        '''</summary>
        Public Function GetDataVaccinazioneEsclusaNonScaduta(codicePaziente As Integer, codiceVaccinazione As String, dataConvocazione As Date) As Date Implements IVaccinazioniEscluseProvider.GetDataVaccinazioneEsclusaNonScaduta

            Dim dataScadenzaEsclusione As Date = Date.MinValue

            Using cmd As New OracleClient.OracleCommand(Queries.VaccinazioniEscluse.OracleQueries.selDataScadenzaEsclusioneNonScaduta, Me.Connection)

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                cmd.Parameters.AddWithValue("cod_vac", GetStringParam(codiceVaccinazione, False))
                cmd.Parameters.AddWithValue("data_cnv", dataConvocazione)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        Try
                            dataScadenzaEsclusione = Convert.ToDateTime(obj)
                        Catch ex As Exception
                            dataScadenzaEsclusione = Date.MinValue
                        End Try
                    End If

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return dataScadenzaEsclusione

        End Function

        '''<summary>
        ''' Restituisce un datatable contenente i dati delle vaccinazioni escluse per il paziente specificato.
        '''</summary>
        Public Function GetDtVaccinazioniEsclusePaziente(codicePaziente As Integer) As DataTable Implements IVaccinazioniEscluseProvider.GetDtVaccinazioniEsclusePaziente

            Return Me.GetDtVaccinazioniEscluse(codicePaziente.ToString(), Nothing)

        End Function

        '''<summary>
        ''' Restituisce un datatable contenente i dati delle vaccinazioni escluse in base agli id specificati.
        '''</summary>
        Public Function GetDtVaccinazioniEscluseById(listIdVaccinazioniEscluse As List(Of Int64)) As DataTable Implements IVaccinazioniEscluseProvider.GetDtVaccinazioniEscluseById

            Return Me.GetDtVaccinazioniEscluse(Nothing, listIdVaccinazioniEscluse)

        End Function

        Private Function GetDtVaccinazioniEscluse(codicePaziente As String, listIdVaccinazioniEscluse As List(Of Int64)) As DataTable

            Dim dtVaccinazioniEscluse As New DataTable()

            With _DAM.QB

                .NewQuery()

                .AddTables("t_vac_escluse, t_ana_vaccinazioni, t_ana_operatori, t_ana_motivi_esclusione, t_ana_usl")

                .AddSelectFields("vex_id, vex_data_visita, vex_vac_codice, vac_descrizione, vex_ope_codice, ope_nome, vex_moe_codice, moe_descrizione")
                .AddSelectFields("vex_paz_codice_old, vex_data_scadenza, vex_usl_inserimento, usl_descrizione usl_inserimento_vex_descr, vex_flag_visibilita")
                .AddSelectFields("vex_data_registrazione, vex_ute_id_registrazione, vex_data_variazione, vex_ute_id_variazione, vex_dose, vex_note")

                If Not listIdVaccinazioniEscluse Is Nothing AndAlso listIdVaccinazioniEscluse.Count > 0 Then
                    .AddWhereCondition("vex_id", Comparatori.In, String.Join(",", listIdVaccinazioniEscluse.ToArray()), DataTypes.Replace)
                End If

                If Not String.IsNullOrEmpty(codicePaziente) Then
                    .AddWhereCondition("vex_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                End If

                .AddWhereCondition("vac_codice", Comparatori.Uguale, "vex_vac_codice", DataTypes.Join)
                .AddWhereCondition("ope_codice", Comparatori.Uguale, "vex_ope_codice", DataTypes.OutJoinRight)
                .AddWhereCondition("moe_codice", Comparatori.Uguale, "vex_moe_codice", DataTypes.OutJoinRight)
                .AddWhereCondition("usl_codice", Comparatori.Uguale, "vex_usl_inserimento", DataTypes.OutJoinRight)

                .AddOrderByFields("vac_ordine")

            End With

            _DAM.BuildDataTable(dtVaccinazioniEscluse)

            Return dtVaccinazioniEscluse

        End Function

        Public Function GetVaccinazioniEscluseById(listIdVaccinazioniEscluse As List(Of Int64)) As List(Of VaccinazioneEsclusa) Implements IVaccinazioniEscluseProvider.GetVaccinazioniEscluseById

            Dim listVaccinazioniEscluse As List(Of VaccinazioneEsclusa) = Nothing

            With _DAM.QB

                .NewQuery()

                .AddTables("t_vac_escluse, t_ana_motivi_esclusione")

                .AddSelectFields("vex_id, vex_data_visita, vex_vac_codice, vex_ope_codice, '' ope_nome, vex_moe_codice, moe_descrizione")
                .AddSelectFields("vex_paz_codice_old, vex_data_scadenza, vex_usl_inserimento, vex_flag_visibilita, vex_paz_codice")
                .AddSelectFields("vex_data_registrazione, vex_ute_id_registrazione, vex_data_variazione, vex_ute_id_variazione, vex_dose, vex_note")

                If Not listIdVaccinazioniEscluse Is Nothing AndAlso listIdVaccinazioniEscluse.Count > 0 Then
                    .AddWhereCondition("vex_id", Comparatori.In, String.Join(",", listIdVaccinazioniEscluse.ToArray()), DataTypes.Replace)
                End If

                .AddWhereCondition("moe_codice", Comparatori.Uguale, "vex_moe_codice", DataTypes.OutJoinRight)

                .AddOrderByFields("vex_id")

            End With

            Using idr As IDataReader = _DAM.BuildDataReader()

                listVaccinazioniEscluse = GetListVaccinazioniEscluse(idr)

            End Using

            Return listVaccinazioniEscluse

        End Function

        Public Function GetMaxDoseEseguitaVaccinazioneEsclusa(codicePaziente As String, codiceVaccinazione As String) As Integer Implements IVaccinazioniEscluseProvider.GetMaxDoseEseguitaVaccinazionaEsclusa

            Dim dose As Integer = 0

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("nvl(max(ves_n_richiamo), 0) ves_n_richiamo")
                .AddTables("t_vac_eseguite")
                .AddWhereCondition("ves_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("ves_vac_codice", Comparatori.Uguale, codiceVaccinazione, DataTypes.Stringa)
            End With

            Dim obj As Object = _DAM.ExecScalar()

            If Not IsDBNull(obj) Then
                dose = Integer.Parse(obj)
            End If

            Return dose

        End Function

#End Region

#Region " Count "

        ''' <summary>
        ''' Restituisce il numero di vaccinazioni escluse relative al paziente specificato
        ''' </summary>
        Public Function CountVaccinazioniEsclusePaziente(codicePaziente As Integer) As Integer Implements IVaccinazioniEscluseProvider.CountVaccinazioniEsclusePaziente

            Dim count As Integer = 0

            Using cmd As New OracleClient.OracleCommand(Queries.VaccinazioniEscluse.OracleQueries.countEsclusePaziente, Me.Connection)

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

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

#Region " Exists "

        ''' <summary>
        ''' Restituisce true se la vaccinazione specificata è presente tra le escluse del paziente
        ''' </summary>
        Public Function ExistsVaccinazioneEsclusa(codicePaziente As Integer, codiceVaccinazione As String) As Boolean Implements IVaccinazioniEscluseProvider.ExistsVaccinazioneEsclusa

            Dim existsEsclusa As Boolean = False

            Using cmd As New OracleClient.OracleCommand(Queries.VaccinazioniEscluse.OracleQueries.existsEsclusione, Me.Connection)

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                cmd.Parameters.AddWithValue("cod_vac", codiceVaccinazione)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    existsEsclusa = (Not obj Is Nothing AndAlso Not obj Is DBNull.Value)

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return existsEsclusa

        End Function

        '''<summary>
        ''' Restituisce true se esiste un'esclusione non scaduta rispetto alla data di convocazione, 
        ''' per il paziente e la vaccinazione specificati. Altrimenti restituisce false.
        '''</summary>
        Public Function ExistsVaccinazioneEsclusaNonScaduta(codicePaziente As Integer, codiceVaccinazione As String, dataConvocazione As Date) As Boolean Implements IVaccinazioniEscluseProvider.ExistsVaccinazioneEsclusaNonScaduta

            Dim existsEsclusa As Boolean = False

            Using cmd As New OracleClient.OracleCommand(Queries.VaccinazioniEscluse.OracleQueries.selExistsEsclusioneNonScaduta, Me.Connection)

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                cmd.Parameters.AddWithValue("cod_vac", GetStringParam(codiceVaccinazione, False))
                cmd.Parameters.AddWithValue("data_cnv", dataConvocazione)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    existsEsclusa = (Not obj Is Nothing AndAlso Not obj Is DBNull.Value)

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return existsEsclusa

        End Function

#End Region

#Region " Insert/Update/Delete "

        Friend Sub InserisciVaccinazioneEsclusa(vaccinazioneEsclusa As VaccinazioneEsclusa) Implements IVaccinazioniEscluseProvider.InserisciVaccinazioneEsclusa

            Using cmd As New OracleCommand()

                cmd.Connection = Connection

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    If vaccinazioneEsclusa.Id <= 0 Then
                        cmd.CommandText = Queries.VaccinazioniEscluse.OracleQueries.selNextSeqEsclusa
                        vaccinazioneEsclusa.Id = cmd.ExecuteScalar()
                    End If

                    cmd.CommandText = Queries.VaccinazioniEscluse.OracleQueries.insEsclusione

                    SetVaccinazioneEsclusaInsertOrUpdateParameters(cmd, vaccinazioneEsclusa, True)

                    cmd.ExecuteNonQuery()

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

        End Sub

        '''<summary>
        ''' Update esclusione per il paziente e la vaccinazione specificati. Restituisce il numero di record modificati.
        '''</summary>
        Friend Sub ModificaVaccinazioneEsclusa(vaccinazioneEsclusa As VaccinazioneEsclusa) Implements IVaccinazioniEscluseProvider.ModificaVaccinazioneEsclusa

            Using cmd As New OracleClient.OracleCommand(Queries.VaccinazioniEscluse.OracleQueries.updEsclusione, Me.Connection)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Me.SetVaccinazioneEsclusaInsertOrUpdateParameters(cmd, vaccinazioneEsclusa, False)

                    cmd.ExecuteNonQuery()

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

        End Sub

        Friend Sub DeleteVaccinazioneEsclusa(idVaccinazioneEsclusa As Long) Implements IVaccinazioniEscluseProvider.DeleteVaccinazioneEsclusa

            Using cmd As New OracleClient.OracleCommand(Queries.VaccinazioniEscluse.OracleQueries.delEsclusione, Me.Connection)

                cmd.Parameters.AddWithValue("id", idVaccinazioneEsclusa)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.ExecuteNonQuery()

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

        End Sub

        Friend Sub DeleteVaccinazioniEscluse(codicePaziente As Integer) Implements IVaccinazioniEscluseProvider.DeleteVaccinazioniEscluse

            Using cmd As New OracleClient.OracleCommand(Queries.VaccinazioniEscluse.OracleQueries.delAllEsclusione, Me.Connection)

                cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)
                    cmd.ExecuteNonQuery()

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

        End Sub

        Private Sub SetVaccinazioneEsclusaInsertOrUpdateParameters(cmd As OracleCommand, vaccinazioneEsclusa As VaccinazioneEsclusa, setCodicePazientePrecedente As Boolean)

            'TODO: vaccinazioneEsclusa.NoteAcquisizioneDatiVaccinaliCentrale ??

            cmd.Parameters.AddWithValue("id", vaccinazioneEsclusa.Id)
            cmd.Parameters.AddWithValue("cod_paz", vaccinazioneEsclusa.CodicePaziente)
            cmd.Parameters.AddWithValue("data_visita", GetDateParam(vaccinazioneEsclusa.DataVisita))
            cmd.Parameters.AddWithValue("cod_vac", vaccinazioneEsclusa.CodiceVaccinazione)
            cmd.Parameters.AddWithValue("cod_ope", GetStringParam(vaccinazioneEsclusa.CodiceOperatore))
            cmd.Parameters.AddWithValue("cod_motivo", GetStringParam(vaccinazioneEsclusa.CodiceMotivoEsclusione))
            cmd.Parameters.AddWithValue("data_scadenza", GetDateParam(vaccinazioneEsclusa.DataScadenza))
            cmd.Parameters.AddWithValue("usl_inserimento", GetStringParam(vaccinazioneEsclusa.CodiceUslInserimento))
            cmd.Parameters.AddWithValue("data_registrazione", GetDateParam(vaccinazioneEsclusa.DataRegistrazione))
            cmd.Parameters.AddWithValue("ute_registrazione", GetLongParam(vaccinazioneEsclusa.IdUtenteRegistrazione))
            cmd.Parameters.AddWithValue("data_variazione", GetDateParam(vaccinazioneEsclusa.DataModifica))
            cmd.Parameters.AddWithValue("ute_variazione", GetLongParam(vaccinazioneEsclusa.IdUtenteModifica))
            cmd.Parameters.AddWithValue("flag_visibilita", GetStringParam(vaccinazioneEsclusa.FlagVisibilita))

            If vaccinazioneEsclusa.NumeroDose.HasValue Then
                cmd.Parameters.AddWithValue("dose", vaccinazioneEsclusa.NumeroDose.Value)
            Else
                cmd.Parameters.AddWithValue("dose", DBNull.Value)
            End If

            cmd.Parameters.AddWithValue("note", GetStringParam(vaccinazioneEsclusa.Note))

            If setCodicePazientePrecedente Then

                If vaccinazioneEsclusa.CodicePazientePrecedente.HasValue AndAlso vaccinazioneEsclusa.CodicePazientePrecedente.Value > 0 Then
                    cmd.Parameters.AddWithValue("cod_paz_old", vaccinazioneEsclusa.CodicePazientePrecedente.Value)
                Else
                    cmd.Parameters.AddWithValue("cod_paz_old", DBNull.Value)
                End If

            End If

        End Sub

        ''' <summary>
        ''' Update flag di visibilità. Vengono modificati anche utente e data di variazione.
        ''' </summary>
        ''' <param name="idVaccinazioneEsclusa"></param>
        ''' <param name="flagVisibilita"></param>
        ''' <param name="idUtenteVariazione"></param>
        ''' <param name="dataVariazione"></param>
        ''' <returns></returns>
        Public Function UpdateFlagVisibilita(idVaccinazioneEsclusa As Long, flagVisibilita As String, idUtenteVariazione As Long, dataVariazione As Date) As Integer Implements IVaccinazioniEscluseProvider.UpdateFlagVisibilita

            Dim count As Integer = 0

            Using cmd As OracleCommand = Connection.CreateCommand()

                cmd.CommandText =
                    " update t_vac_escluse set " +
                    " vex_flag_visibilita = :vex_flag_visibilita, " +
                    " vex_ute_id_variazione = :vex_ute_id_variazione, " +
                    " vex_data_variazione = :vex_data_variazione " +
                    " where vex_id = :vex_id "

                cmd.Parameters.AddWithValue("vex_flag_visibilita", flagVisibilita)
                cmd.Parameters.AddWithValue("vex_ute_id_variazione", idUtenteVariazione)
                cmd.Parameters.AddWithValue("vex_data_variazione", dataVariazione)
                cmd.Parameters.AddWithValue("vex_id", idVaccinazioneEsclusa)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    count = cmd.ExecuteNonQuery()

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

#Region " Eliminata "

        Friend Sub InserisciVaccinazioneEsclusaEliminata(vaccinazioneEsclusaEliminata As VaccinazioneEsclusa) Implements IVaccinazioniEscluseProvider.InserisciVaccinazioneEsclusaEliminata
            InserisciVaccinazioneEsclusaEliminata(vaccinazioneEsclusaEliminata, Constants.StatoVaccinazioniEscluseEliminate.Eliminata)
        End Sub

        Friend Sub InserisciVaccinazioneEsclusaEliminata(vaccinazioneEsclusaEliminata As VaccinazioneEsclusa, tipoEliminazione As String) Implements IVaccinazioniEscluseProvider.InserisciVaccinazioneEsclusaEliminata

            Using cmd As New OracleClient.OracleCommand()

                Dim ownConnection As Boolean = False

                Try
                    cmd.Connection = Me.Connection

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    If vaccinazioneEsclusaEliminata.Id <= 0 Then
                        cmd.CommandText = Queries.VaccinazioniEscluse.OracleQueries.selNextSeqEsclusa
                        vaccinazioneEsclusaEliminata.Id = cmd.ExecuteScalar()
                    End If

                    cmd.CommandText = Queries.VaccinazioniEscluse.OracleQueries.insEsclusioneEliminata

                    Me.SetVaccinazioneEsclusaEliminataInsertParameters(cmd, vaccinazioneEsclusaEliminata, True)

                    If Not String.IsNullOrWhiteSpace(tipoEliminazione.ToString()) Then
                        cmd.Parameters.AddWithValue("stato_eliminazione", tipoEliminazione)
                    Else
                        cmd.Parameters.AddWithValue("stato_eliminazione", "E")
                    End If

                    cmd.ExecuteNonQuery()

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

        End Sub

        Private Sub SetVaccinazioneEsclusaEliminataInsertParameters(cmd As OracleClient.OracleCommand, vaccinazioneEsclusaEliminata As VaccinazioneEsclusa, setCodicePazientePrecedente As Boolean)

            Me.SetVaccinazioneEsclusaInsertOrUpdateParameters(cmd, vaccinazioneEsclusaEliminata, setCodicePazientePrecedente)

            cmd.Parameters.AddWithValue("ute_eliminazione", GetLongParam(vaccinazioneEsclusaEliminata.IdUtenteEliminazione))
            cmd.Parameters.AddWithValue("data_eliminazione", GetDateParam(vaccinazioneEsclusaEliminata.DataEliminazione))

        End Sub

#End Region

#End Region

#Region " OnVac API "

        ''' <summary>
        ''' Restituisce la lista delle escluse del paziente
        ''' </summary>
        ''' <param name="listCodiciPazienti"></param>
        ''' <returns></returns>
        Public Function GetListVaccinazioniEsclusePazientiAPP(listCodiciPazienti As List(Of Int64)) As List(Of Entities.VaccinazioneEsclusaAPP) Implements IVaccinazioniEscluseProvider.GetListVaccinazioniEsclusePazientiAPP

            Dim listEscluse As List(Of VaccinazioneEsclusaAPP) = Nothing

            Using cmd As New OracleCommand()

                cmd.Connection = Me.Connection

                Dim filtroPazienti As GetInFilterResult = Me.GetInFilter(listCodiciPazienti)

                cmd.CommandText = String.Format(GetQueryVaccinazioniEscluseAPP(False), filtroPazienti.InFilter)
                cmd.Parameters.AddRange(filtroPazienti.Parameters)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    listEscluse = GetListVaccinazioniEscluseAPP(cmd)

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listEscluse

        End Function

        ''' <summary>
        ''' Restituzione lista escluse per il paziente e vaccinazione specificati
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="codiceVaccinazioneCovid"></param>
        ''' <returns></returns>
        Public Function GetListVaccinazioniEsclusePazienteAPP(codicePaziente As Long, codiceVaccinazioneCovid As String) As List(Of VaccinazioneEsclusaAPP) Implements IVaccinazioniEscluseProvider.GetListVaccinazioniEsclusePazienteAPP

            Dim listEscluse As List(Of VaccinazioneEsclusaAPP) = Nothing

            Using cmd As New OracleCommand()

                cmd.Connection = Connection

                cmd.CommandText = GetQuerySelectVaccinazioniEscluseAPP() + " where vex_paz_codice = :vex_paz_codice AND vex_vac_codice = :vex_vac_codice"
                cmd.Parameters.AddWithValue("vex_paz_codice", codicePaziente)
                cmd.Parameters.AddWithValue("vex_vac_codice", codiceVaccinazioneCovid)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    listEscluse = GetListVaccinazioniEscluseAPP(cmd)

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listEscluse

        End Function

        ''' <summary>
        ''' Restituisce la vaccinazione esclusa specificata
        ''' </summary>
        ''' <param name="idEsclusa"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetVaccinazioneEsclusaPazienteAPP(idEsclusa As Int64) As VaccinazioneEsclusaAPP Implements IVaccinazioniEscluseProvider.GetVaccinazioneEsclusaPazienteAPP

            Dim esclusa As VaccinazioneEsclusaAPP = Nothing

            Using cmd As New OracleCommand(GetQueryVaccinazioniEscluseAPP(True), Connection)

                cmd.Parameters.AddWithValue("id", idEsclusa)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim listEscluse As List(Of VaccinazioneEsclusaAPP) = GetListVaccinazioniEscluseAPP(cmd)

                    If Not listEscluse Is Nothing AndAlso listEscluse.Count > 0 Then
                        esclusa = listEscluse.First()
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return esclusa

        End Function

        Private Function GetQuerySelectVaccinazioniEscluseAPP() As String

            Dim s As New Text.StringBuilder()

            s.Append("select vex_id, vex_paz_codice, vex_data_visita, vex_vac_codice, vex_moe_codice, vex_data_scadenza,  moe_descrizione, vac_descrizione, ")
            s.Append(" paz_cognome, paz_nome, paz_data_nascita ")
            s.Append("from t_vac_escluse ")
            s.Append("    join t_ana_vaccinazioni on vex_vac_codice = vac_codice ")
            s.Append("    join t_paz_pazienti on vex_paz_codice = paz_codice ")
            s.Append("    left join t_ana_motivi_esclusione on vex_moe_codice = moe_codice ")

            Return s.ToString()

        End Function

        Private Function GetQueryVaccinazioniEscluseAPP(filterById As Boolean) As String

            Dim s As New Text.StringBuilder(GetQuerySelectVaccinazioniEscluseAPP())

            If filterById Then
                s.Append("where vex_id = :id ")
            Else
                s.Append("where vex_paz_codice in ({0}) ")
            End If
            s.Append("order by vex_paz_codice, vex_data_visita desc, vex_vac_codice, vex_data_scadenza, moe_descrizione ")

            Return s.ToString()

        End Function

        Private Function GetListVaccinazioniEscluseAPP(cmd As OracleClient.OracleCommand) As List(Of Entities.VaccinazioneEsclusaAPP)

            Dim listEscluse As New List(Of Entities.VaccinazioneEsclusaAPP)()

            Using idr As IDataReader = cmd.ExecuteReader()

                If Not idr Is Nothing Then

                    Dim vex_id As Integer = idr.GetOrdinal("vex_id")
                    Dim vex_paz_codice As Integer = idr.GetOrdinal("vex_paz_codice")
                    Dim vex_data_visita As Integer = idr.GetOrdinal("vex_data_visita")
                    Dim vex_vac_codice As Integer = idr.GetOrdinal("vex_vac_codice")
                    Dim vac_descrizione As Integer = idr.GetOrdinal("vac_descrizione")
                    Dim vex_moe_codice As Integer = idr.GetOrdinal("vex_moe_codice")
                    Dim moe_descrizione As Integer = idr.GetOrdinal("moe_descrizione")
                    Dim vex_data_scadenza As Integer = idr.GetOrdinal("vex_data_scadenza")
                    Dim paz_cognome As Integer = idr.GetOrdinal("paz_cognome")
                    Dim paz_nome As Integer = idr.GetOrdinal("paz_nome")
                    Dim paz_data_nascita As Integer = idr.GetOrdinal("paz_data_nascita")

                    Dim esclusa As Entities.VaccinazioneEsclusaAPP = Nothing

                    While idr.Read()

                        esclusa = New Entities.VaccinazioneEsclusaAPP()

                        esclusa.Id = idr.GetInt64(vex_id)
                        esclusa.CodicePaziente = idr.GetInt64(vex_paz_codice)
                        esclusa.DataVisita = idr.GetDateTimeOrDefault(vex_data_visita)
                        esclusa.CodiceVaccinazione = idr.GetString(vex_vac_codice)
                        esclusa.DescrizioneVaccinazione = idr.GetString(vac_descrizione)
                        esclusa.CodiceMotivoEsclusione = idr.GetStringOrDefault(vex_moe_codice)
                        esclusa.DescrizioneMotivoEsclusione = idr.GetStringOrDefault(moe_descrizione)
                        esclusa.DataScadenza = idr.GetNullableDateTimeOrDefault(vex_data_scadenza)
                        esclusa.CognomePaziente = idr.GetStringOrDefault(paz_cognome)
                        esclusa.NomePaziente = idr.GetStringOrDefault(paz_nome)
                        esclusa.DataNascitaPaziente = idr.GetDateTimeOrDefault(paz_data_nascita)

                        listEscluse.Add(esclusa)

                    End While

                End If

            End Using

            Return listEscluse

        End Function

#Region " FSE "

        ''' <summary>
        ''' Restituisce la lista di vaccinazioni escluse del paziente specificato
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        Public Function GetListVaccinazioniEsclusePazienteFSE(codicePaziente As Long) As List(Of Entities.VaccinazioneFSE) Implements IVaccinazioniEscluseProvider.GetListVaccinazioniEsclusePazienteFSE

            Dim listEscluse As New List(Of Entities.VaccinazioneFSE)()

            Dim query As New System.Text.StringBuilder()
            query.Append("SELECT vex_id, vex_vac_codice, vac_descrizione, vex_data_visita, mal_codice, mal_descrizione, mal_codice_fse, vex_data_registrazione, moe_codice, moe_descrizione, mal_codice_esenzione, vac_codice_fse ")
            query.Append(" FROM t_vac_escluse ")
            query.Append(" INNER JOIN t_ana_vaccinazioni ON vac_codice = vex_vac_codice ")
            query.Append(" INNER JOIN t_ana_motivi_esclusione ON moe_codice = vex_moe_codice ")
            query.Append(" LEFT JOIN t_ana_malattie ON mal_codice = F_GET_CODICE_MALATTIA_VAC(vex_vac_codice) ")
            query.Append(" WHERE vex_paz_codice = :codicePaziente ")
            query.Append("  AND moe_flag_stampa_certificato = 'S' ")
            query.Append(" ORDER BY vex_data_visita desc, vex_vac_codice, vex_data_scadenza, moe_descrizione ")

            Using cmd As OracleCommand = New OracleCommand(query.ToString(), Me.Connection)

                cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim vex_id As Integer = idr.GetOrdinal("vex_id")
                            Dim vex_vac_codice As Integer = idr.GetOrdinal("vex_vac_codice")
                            Dim vac_descrizione As Integer = idr.GetOrdinal("vac_descrizione")
                            Dim vex_data_visita As Integer = idr.GetOrdinal("vex_data_visita")
                            Dim vac_codice_fse As Integer = idr.GetOrdinal("vac_codice_fse")
                            Dim mal_codice As Integer = idr.GetOrdinal("mal_codice")
                            Dim mal_descrizione As Integer = idr.GetOrdinal("mal_descrizione")
                            Dim mal_codice_fse As Integer = idr.GetOrdinal("mal_codice_fse")
                            Dim mal_codice_esenzione As Integer = idr.GetOrdinal("mal_codice_esenzione")
                            Dim vex_data_registrazione As Integer = idr.GetOrdinal("vex_data_registrazione")
                            Dim moe_codice As Integer = idr.GetOrdinal("moe_codice")
                            Dim moe_descrizione As Integer = idr.GetOrdinal("moe_descrizione")

                            While idr.Read()

                                Dim vacEsclusa As New Entities.VaccinazioneFSE()

                                vacEsclusa.Id = idr.GetInt64(vex_id)
                                vacEsclusa.CodiceVaccinazione = idr.GetStringOrDefault(vex_vac_codice)
                                vacEsclusa.DescrizioneVaccinazione = idr.GetStringOrDefault(vac_descrizione)
                                'vacEsclusa.DataEffettuazione = idr.GetNullableDateTimeOrDefault(vex_data_visita)
                                vacEsclusa.CodiceCvxVaccinazione = idr.GetStringOrDefault(vac_codice_fse)
                                vacEsclusa.CodiceMalattia = idr.GetStringOrDefault(mal_codice)
                                vacEsclusa.DescrizioneMalattia = idr.GetStringOrDefault(mal_descrizione)
                                vacEsclusa.CodiceICD9CMMalattia = idr.GetStringOrDefault(mal_codice_fse)
                                vacEsclusa.CodiceHL7Esenzione = idr.GetStringOrDefault(mal_codice_esenzione)
                                vacEsclusa.IsEseguita = False
                                vacEsclusa.DataRegistrazioneEsclusione = idr.GetNullableDateTimeOrDefault(vex_data_registrazione)
                                vacEsclusa.CodiceMotivoEsclusione = idr.GetStringOrDefault(moe_codice)
                                vacEsclusa.DescrizioneMotivoEsclusione = idr.GetStringOrDefault(moe_descrizione)

                                listEscluse.Add(vacEsclusa)

                            End While

                        End If

                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return listEscluse

        End Function


#End Region


#End Region

#Region "OnVacInadempienze"
        Public Function CreaInadempienza() As ResultSetPost Implements IVaccinazioniEscluseProvider.CreaInadempienza
            Dim result As New ResultSetPost()
            result.Success = True
            result.Message = "pippo eseguito con successo"
            Return result
        End Function
        'Public Sub InserisciInadempienzePaziente(codicePaziente As String, inadempienzePaziente As System.Collections.ObjectModel.Collection(Of Entities.MovimentoCNS.InadempienzaPaziente)) Implements IMovimentiEsterniCNSProvider.InserisciInadempienzePaziente

        '    Dim cmd As OracleClient.OracleCommand = Nothing

        '    Dim ownTransaction As Boolean = False

        '    Try
        '        cmd = New OracleClient.OracleCommand("INSERT INTO T_PAZ_INADEMPIENZE(PIN_PAZ_CODICE, PIN_PAZ_CODICE_OLD, PIN_VAC_CODICE, PIN_STAMPATO, PIN_STATO, PIN_DATA, PIN_UTE_ID, PIN_PRI_DATA_APPUNTAMENTO1, PIN_PRI_DATA_APPUNTAMENTO2, PIN_PRI_DATA_APPUNTAMENTO3, PIN_PRI_DATA_APPUNTAMENTO4, PIN_PRI_DATA_APPUNTAMENTO_TP, PIN_PRI_DATA_STAMPA_TP, PIN_UTE_ID_STAMPA_CS, PIN_PRI_DATA_STAMPA_CS) VALUES(:codicePaziente, :codicePazientePrecedente, :codiceVaccinazione, :stampato, :stato, :dataInadempienza, :idUtente, :dataAppuntamento1, :dataAppuntamento2, :dataAppuntamento3, :dataAppuntamento4, :dataAppuntamentoTP, :dataStampaTP, :idUtenteStampaCS, :dataStampaCS)", Me.Connection)

        '        If Not Me.Transaction Is Nothing Then
        '            cmd.Transaction = Me.Transaction
        '        Else
        '            ownTransaction = True
        '            Me._DAM.BeginTrans()
        '        End If

        '        For Each inadempienzaPaziente As MovimentoCNS.InadempienzaPaziente In inadempienzePaziente

        '            cmd.Parameters.Clear()

        '            cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

        '            If Not inadempienzaPaziente.CodicePazientePrecedente Is Nothing Then
        '                cmd.Parameters.AddWithValue("codicePazientePrecedente", inadempienzaPaziente.CodicePazientePrecedente)
        '            Else
        '                cmd.Parameters.AddWithValue("codicePazientePrecedente", DBNull.Value)
        '            End If

        '            cmd.Parameters.AddWithValue("codiceVaccinazione", inadempienzaPaziente.Vaccinazione.Codice)

        '            If Not inadempienzaPaziente.Stampato Is Nothing Then
        '                cmd.Parameters.AddWithValue("stampato", IIf(inadempienzaPaziente.Stampato, "S", "N"))
        '            Else
        '                cmd.Parameters.AddWithValue("stampato", DBNull.Value)
        '            End If

        '            If Not inadempienzaPaziente.Stato Is Nothing Then
        '                cmd.Parameters.AddWithValue("stato", inadempienzaPaziente.Stato)
        '            Else
        '                cmd.Parameters.AddWithValue("stato", DBNull.Value)
        '            End If

        '            If Not inadempienzaPaziente.DataInadempienza Is Nothing Then
        '                cmd.Parameters.AddWithValue("dataInadempienza", inadempienzaPaziente.DataInadempienza)
        '            Else
        '                cmd.Parameters.AddWithValue("dataInadempienza", DBNull.Value)
        '            End If

        '            If Not inadempienzaPaziente.Utente Is Nothing Then
        '                cmd.Parameters.AddWithValue("idUtente", inadempienzaPaziente.Utente.ID)
        '            Else
        '                cmd.Parameters.AddWithValue("idUtente", DBNull.Value)
        '            End If

        '            If Not inadempienzaPaziente.DataAppuntamento1 Is Nothing Then
        '                cmd.Parameters.AddWithValue("dataAppuntamento1", inadempienzaPaziente.DataAppuntamento1)
        '            Else
        '                cmd.Parameters.AddWithValue("dataAppuntamento1", DBNull.Value)
        '            End If

        '            If Not inadempienzaPaziente.DataAppuntamento2 Is Nothing Then
        '                cmd.Parameters.AddWithValue("dataAppuntamento2", inadempienzaPaziente.DataAppuntamento2)
        '            Else
        '                cmd.Parameters.AddWithValue("dataAppuntamento2", DBNull.Value)
        '            End If

        '            If Not inadempienzaPaziente.DataAppuntamento3 Is Nothing Then
        '                cmd.Parameters.AddWithValue("dataAppuntamento3", inadempienzaPaziente.DataAppuntamento3)
        '            Else
        '                cmd.Parameters.AddWithValue("dataAppuntamento3", DBNull.Value)
        '            End If

        '            If Not inadempienzaPaziente.DataAppuntamento4 Is Nothing Then
        '                cmd.Parameters.AddWithValue("dataAppuntamento4", inadempienzaPaziente.DataAppuntamento4)
        '            Else
        '                cmd.Parameters.AddWithValue("dataAppuntamento4", DBNull.Value)
        '            End If

        '            If Not inadempienzaPaziente.DataAppuntamentoTP Is Nothing Then
        '                cmd.Parameters.AddWithValue("dataAppuntamentoTP", inadempienzaPaziente.DataAppuntamentoTP)
        '            Else
        '                cmd.Parameters.AddWithValue("dataAppuntamentoTP", DBNull.Value)
        '            End If

        '            If Not inadempienzaPaziente.DataStampaTP Is Nothing Then
        '                cmd.Parameters.AddWithValue("dataStampaTP", inadempienzaPaziente.DataStampaTP)
        '            Else
        '                cmd.Parameters.AddWithValue("dataStampaTP", DBNull.Value)
        '            End If

        '            If Not inadempienzaPaziente.UtenteStampaCS Is Nothing Then
        '                cmd.Parameters.AddWithValue("idUtenteStampaCS", inadempienzaPaziente.UtenteStampaCS.ID)
        '            Else
        '                cmd.Parameters.AddWithValue("idUtenteStampaCS", DBNull.Value)
        '            End If

        '            If Not inadempienzaPaziente.DataStampaCS Is Nothing Then
        '                cmd.Parameters.AddWithValue("dataStampaCS", inadempienzaPaziente.DataStampaCS)
        '            Else
        '                cmd.Parameters.AddWithValue("dataStampaCS", DBNull.Value)
        '            End If

        '            cmd.ExecuteNonQuery()

        '        Next

        '        If ownTransaction Then
        '            Me.Transaction.Commit()
        '        End If

        '    Catch ex As Exception

        '        If ownTransaction Then
        '            Me.Transaction.Rollback()
        '        End If

        '        ex.InternalPreserveStackTrace()
        '        Throw

        '    Finally

        '        If Not Me.Transaction Is Nothing AndAlso ownTransaction Then Me.Transaction.Dispose()
        '        If Not cmd Is Nothing Then cmd.Dispose()

        '    End Try

        'End Sub
#End Region

    End Class

End Namespace
