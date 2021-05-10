Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.Log.DataLogStructure
Imports System.Data.OracleClient


Namespace DAL

    Public Class DbConsultoriProvider
        Inherits DbProvider
        Implements IConsultoriProvider

#Region " Enum "

        Public Enum Entity
            Ambulatorio
            Consultorio
        End Enum

#End Region

#Region " Constructors "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " Ambulatori "

        Public Function GetAmbulatoriConsultorio(codiceConsultorio As String, soloAperti As Boolean) As List(Of Entities.Ambulatorio) Implements IConsultoriProvider.GetAmbulatoriConsultorio

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("amb_codice, amb_descrizione, amb_data_apertura, amb_data_chiusura")
                .AddTables("t_ana_ambulatori")
                .AddWhereCondition("amb_cns_codice", Comparatori.Uguale, codiceConsultorio, DataTypes.Stringa)
                If soloAperti Then
                    .AddWhereCondition("amb_data_apertura", Comparatori.MinoreUguale, "SYSDATE", DataTypes.Replace)
                    .OpenParanthesis()
                    .AddWhereCondition("amb_data_chiusura", Comparatori.Maggiore, "SYSDATE", DataTypes.Replace)
                    .AddWhereCondition("amb_data_chiusura", Comparatori.Is, "NULL", DataTypes.Stringa, "or")
                    .CloseParanthesis()
                    .AddOrderByFields("amb_descrizione")
                End If
            End With

            Return GetAmbulatori(_DAM)

        End Function

        Public Function GetAmbulatorio(codiceAmbulatorio As Integer) As Entities.Ambulatorio Implements IConsultoriProvider.GetAmbulatorio

            Dim ambulatorio As Entities.Ambulatorio = Nothing

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("amb_codice, amb_descrizione, amb_data_apertura, amb_data_chiusura")
                .AddTables("t_ana_ambulatori")
                .AddWhereCondition("amb_codice", Comparatori.Uguale, codiceAmbulatorio, DataTypes.Numero)
            End With

            Dim listAmbulatori As List(Of Entities.Ambulatorio) = GetAmbulatori(_DAM)
            If Not listAmbulatori.IsNullOrEmpty() Then
                ambulatorio = listAmbulatori.FirstOrDefault()
            End If

            Return ambulatorio

        End Function

        Private Function GetAmbulatori(dam As IDAM) As List(Of Entities.Ambulatorio)

            Dim listAmbulatori As New List(Of Entities.Ambulatorio)()

            Using idr As IDataReader = dam.BuildDataReader()
                If Not idr Is Nothing Then

                    Dim amb_codice As Int16 = idr.GetOrdinal("amb_codice")
                    Dim amb_descrizione As Int16 = idr.GetOrdinal("amb_descrizione")
                    Dim amb_data_apertura As Int16 = idr.GetOrdinal("amb_data_apertura")
                    Dim amb_data_chiusura As Int16 = idr.GetOrdinal("amb_data_chiusura")

                    Dim ambulatorio As Entities.Ambulatorio = Nothing

                    While idr.Read()

                        ambulatorio = New Entities.Ambulatorio()
                        ambulatorio.Codice = idr.GetInt32OrDefault(amb_codice)
                        ambulatorio.Descrizione = idr.GetStringOrDefault(amb_descrizione)
                        ambulatorio.DataApertura = idr.GetNullableDateTimeOrDefault(amb_data_apertura)
                        ambulatorio.DataChiusura = idr.GetNullableDateTimeOrDefault(amb_data_chiusura)

                        listAmbulatori.Add(ambulatorio)

                    End While

                End If
            End Using

            Return listAmbulatori

        End Function

        Public Function GetAmbulatoriAperti(codiceConsultorio As String) As DataTable Implements IConsultoriProvider.GetAmbulatoriAperti

            Dim dt As New DataTable()

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("AMB_DESCRIZIONE as DESCRIZIONE", "AMB_CODICE as CODICE")
                .AddTables("T_ANA_AMBULATORI")
                .AddWhereCondition("amb_cns_codice", Comparatori.Uguale, codiceConsultorio, DataTypes.Stringa)
                .AddWhereCondition("amb_data_apertura", Comparatori.MinoreUguale, "SYSDATE", DataTypes.Replace)
                .OpenParanthesis()
                .AddWhereCondition("amb_data_chiusura", Comparatori.Maggiore, "SYSDATE", DataTypes.Replace)
                .AddWhereCondition("amb_data_chiusura", Comparatori.Is, "NULL", DataTypes.Stringa, "or")
                .CloseParanthesis()
                .AddOrderByFields("amb_descrizione")
            End With

            Try
                _DAM.BuildDataTable(dt)

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return dt

        End Function

        Public Function GetAmbDescrizione(codiceAmbulatorio As Integer) As String Implements IConsultoriProvider.GetAmbDescrizione

            Dim result As String = String.Empty

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("AMB_DESCRIZIONE")
                .AddTables("T_ANA_AMBULATORI")
                .AddWhereCondition("AMB_CODICE", Comparatori.Uguale, codiceAmbulatorio, DataTypes.Numero)
            End With

            Try
                Using dr As IDataReader = _DAM.BuildDataReader()
                    If dr.Read() Then

                        result = dr.GetStringOrDefault(0)

                    End If
                End Using

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return result

        End Function

        Public Function GetMedicoInAmb(codiceAmbulatorio As Integer) As Boolean Implements IConsultoriProvider.GetMedicoInAmb

            Dim result As Boolean = False

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("AMB_MEDINAMB")
                .AddTables("T_ANA_AMBULATORI")
                .AddWhereCondition("AMB_CODICE", Comparatori.Uguale, codiceAmbulatorio, DataTypes.Numero)
            End With

            Try
                Using dr As IDataReader = _DAM.BuildDataReader()
                    If Not dr Is Nothing Then

                        Dim pos_medinamb As Integer = dr.GetOrdinal("AMB_MEDINAMB")
                        While dr.Read()
                            If Not dr(pos_medinamb) Is System.DBNull.Value AndAlso dr(pos_medinamb) = "S" Then
                                result = True
                            End If
                        End While

                    End If
                End Using

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return result

        End Function

#End Region

#Region " Select "

        Public Function GetNumeroAmbulatori(codiceConsultorio As String) As Integer Implements IConsultoriProvider.GetNumeroAmbulatori

            Return Me.GetNumeroAmbulatori(codiceConsultorio, False)

        End Function

        Public Function GetNumeroAmbulatori(codiceConsultorio As String, soloAperti As Boolean) As Integer Implements IConsultoriProvider.GetNumeroAmbulatori

            Dim count As Integer

            With _DAM.QB

                .NewQuery(True)
                .AddTables("T_ANA_AMBULATORI")
                .AddSelectFields("nvl(count(*),0)")
                .AddWhereCondition("amb_cns_codice", Comparatori.Uguale, codiceConsultorio, DataTypes.Stringa)
                If soloAperti Then
                    .AddWhereCondition("amb_data_apertura", Comparatori.MinoreUguale, "SYSDATE", DataTypes.Replace)
                    .OpenParanthesis()
                    .AddWhereCondition("amb_data_chiusura", Comparatori.Maggiore, "SYSDATE", DataTypes.Replace)
                    .AddWhereCondition("amb_data_chiusura", Comparatori.Is, "NULL", DataTypes.Stringa, "or")
                    .CloseParanthesis()
                    .AddOrderByFields("amb_descrizione")
                End If

            End With

            Try
                count = _DAM.ExecScalar()

            Catch ex As Exception

                LogError(ex)

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return count

        End Function

        ''' <summary>
        ''' Restituisce un oggetto Cns con i dati del consultorio specificato. Se non lo trova restituisce Nothing.
        ''' </summary>
        Public Function GetConsultorio(codiceConsultorio As String) As Cns Implements IConsultoriProvider.GetConsultorio

            Dim cns As Entities.Cns = Nothing

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("com_descrizione, cns_descrizione, cns_tipo, cns_indirizzo, com_cap, cns_n_telefono, cns_stampa1, cns_stampa2, cns_email")
                .AddTables("t_ana_consultori, t_ana_comuni")
                .AddWhereCondition("cns_codice", Comparatori.Uguale, codiceConsultorio, DataTypes.Stringa)
                .AddWhereCondition("cns_com_codice", Comparatori.Uguale, "com_codice", DataTypes.Join)
            End With

            Try
                Using dr As IDataReader = _DAM.BuildDataReader()

                    If dr.Read() Then
                        cns = New Entities.Cns(codiceConsultorio,
                                               dr("cns_descrizione").ToString(),
                                               dr("cns_tipo").ToString(),
                                               dr("com_descrizione").ToString(),
                                               dr("cns_indirizzo").ToString(),
                                               dr("com_cap").ToString(),
                                               dr("cns_n_telefono").ToString(),
                                               dr("cns_stampa1").ToString(),
                                               dr("cns_stampa2").ToString(),
                                               dr("cns_email").ToString())
                    End If

                End Using

            Catch ex As Exception

                LogError(ex)

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return cns

        End Function

        ''' <summary>
        ''' Restituisce il codice del consultorio di magazzino per il consultorio specificato.
        ''' Se il codice del consultorio di magazzino non è specificato, restituisce la stringa vuota.
        ''' </summary>
        Public Function GetConsultorioMagazzino(codiceConsultorio As String) As Consultorio Implements IConsultoriProvider.GetConsultorioMagazzino

            Dim cnsMagazzino As New Consultorio()
            Dim dt As New DataTable()

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("MAGAZZINO.CNS_CODICE", "MAGAZZINO.CNS_DESCRIZIONE")
                .AddTables("T_ANA_CONSULTORI PADRE", "T_ANA_CONSULTORI MAGAZZINO")
                .AddWhereCondition("PADRE.CNS_CODICE", Comparatori.Uguale, codiceConsultorio, DataTypes.Stringa)
                .AddWhereCondition("PADRE.CNS_CNS_MAGAZZINO", Comparatori.Uguale, "MAGAZZINO.CNS_CODICE", DataTypes.Join)
            End With

            _DAM.BuildDataTable(dt)

            If dt.Rows.Count = 0 Then

                With _DAM.QB
                    .NewQuery(True, True)
                    .AddSelectFields("CNS_CODICE", "CNS_DESCRIZIONE")
                    .AddTables("T_ANA_CONSULTORI")
                    .AddWhereCondition("CNS_CODICE", Comparatori.Uguale, codiceConsultorio, DataTypes.Stringa)
                End With

                _DAM.BuildDataTable(dt)

            End If

            If dt.Rows.Count > 0 Then
                cnsMagazzino.Codice = dt.Rows(0).Item("CNS_CODICE")
                cnsMagazzino.Descrizione = dt.Rows(0).Item("CNS_DESCRIZIONE")
            End If

            Return cnsMagazzino

        End Function

        Public Function GetConsultoriAperti(codiceConsultorio As String, queryType As Enumerators.QueryType) As DataTable Implements IConsultoriProvider.GetConsultoriAperti

            Return GetConsultoriAperti(codiceConsultorio, String.Empty, queryType)

        End Function

        Public Function GetConsultoriAperti(codiceConsultorio As String, descrizioneConsultorio As String, queryType As Enumerators.QueryType) As DataTable Implements IConsultoriProvider.GetConsultoriAperti

            Dim dt As New DataTable()

            Dim cnsCodiceLike As String = String.Empty
            Dim cnsDescLike As String = String.Empty

            Select Case queryType

                Case Enumerators.QueryType.PostLike
                    cnsCodiceLike = String.Format("{0}%", codiceConsultorio)
                    cnsDescLike = String.Format("{0}%", descrizioneConsultorio)

                Case Enumerators.QueryType.PrePostLike
                    cnsCodiceLike = String.Format("%{0}%", codiceConsultorio)
                    cnsDescLike = String.Format("%{0}%", descrizioneConsultorio)

                Case Enumerators.QueryType.Equals
                    cnsCodiceLike = String.Format("{0}", codiceConsultorio)
                    cnsDescLike = String.Format("{0}", descrizioneConsultorio)

            End Select

			Me.CreaQueryCaricamentoConsultori(_DAM.QB, True, True, Nothing, 0, String.Empty, String.Empty)

			If codiceConsultorio <> "" Then
                _DAM.QB.AddWhereCondition("cns_codice", Comparatori.Like, cnsCodiceLike, DataTypes.Stringa)
            End If

            If descrizioneConsultorio <> "" Then
                _DAM.QB.AddWhereCondition("cns_descrizione", Comparatori.Like, cnsDescLike, DataTypes.Stringa)
            End If

            Try
                _DAM.BuildDataTable(dt)

            Catch ex As Exception

                LogError(ex)

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return dt

        End Function

        Public Function GetConsultoriAperti() As DataTable Implements IConsultoriProvider.GetConsultoriAperti

            Dim dt As New DataTable()

			Me.CreaQueryCaricamentoConsultori(_DAM.QB, True, False, Nothing, 0, String.Empty, String.Empty)

			Try

                _DAM.BuildDataTable(dt)

            Catch ex As Exception

                LogError(ex)

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return dt

        End Function

        Public Function GetListCodiceDescrizioneConsultori(soloCnsAperti As Boolean, codiceDescrizioneLikeFilter As String, idUtente As Long, filtroCodDistretto As String, filtroUsl As String) As List(Of ConsultorioAperti) Implements IConsultoriProvider.GetListCodiceDescrizioneConsultori

            Dim listConsultori As List(Of ConsultorioAperti) = Nothing

            CreaQueryCaricamentoConsultori(_DAM.QB, soloCnsAperti, False, codiceDescrizioneLikeFilter, idUtente, filtroCodDistretto, filtroUsl)

            Using dr As IDataReader = _DAM.BuildDataReader()

				If Not dr Is Nothing Then

					Dim cns_codice As Integer = dr.GetOrdinal("cns_codice")
					Dim cns_descrizione As Integer = dr.GetOrdinal("cns_descrizione")
					Dim cns_dataChiusura As Integer = dr.GetOrdinal("cns_data_chiusura")

					listConsultori = New List(Of Entities.ConsultorioAperti)()

					While dr.Read()

						Dim cns As New Entities.ConsultorioAperti()

						cns.Codice = dr.GetStringOrDefault(cns_codice)
						cns.Descrizione = dr.GetStringOrDefault(cns_descrizione)
						cns.DataChiusura = dr.GetNullableDateTimeOrDefault(cns_dataChiusura)

						listConsultori.Add(cns)

					End While

				End If

			End Using

			Return listConsultori

		End Function

        Private Sub CreaQueryCaricamentoConsultori(ByRef qb As AbstractQB, soloCnsAperti As Boolean, usaAliasCodiceDescrizione As Boolean, codiceDescrizioneLikeFilter As String, idUtente As Long, filtroCodDistretto As String, filtroUsl As String)

            With qb

                .NewQuery()

                If usaAliasCodiceDescrizione Then
                    .AddSelectFields("cns_codice codice, cns_descrizione descrizione, cns_data_chiusura")
                Else
                    .AddSelectFields("cns_codice, cns_descrizione, cns_data_chiusura")
                End If
                'Aggiungo un flag che mi indica se il consultorio è aperto o chiuso => usato per l'orderby
                If Not soloCnsAperti Then
                    .AddSelectFields("(case when cns_data_chiusura is not null and cns_data_chiusura<=sysdate then 'C' else 'A' end) cns_flag_apertura")
                End If

                .AddTables("t_ana_consultori")

                .AddWhereCondition("1", Comparatori.Uguale, "1", DataTypes.Replace)

                If soloCnsAperti Then
                    .AddWhereCondition("cns_data_apertura", Comparatori.MinoreUguale, "SYSDATE", DataTypes.Replace)
                    .OpenParanthesis()
                    .AddWhereCondition("cns_data_chiusura", Comparatori.Maggiore, "SYSDATE", DataTypes.Replace)
                    .AddWhereCondition("cns_data_chiusura", Comparatori.Is, "NULL", DataTypes.Stringa, "or")
                    .CloseParanthesis()
                End If

                If Not String.IsNullOrEmpty(codiceDescrizioneLikeFilter) Then
                    .OpenParanthesis()
                    .AddWhereCondition("cns_codice", Comparatori.Like, "%" + codiceDescrizioneLikeFilter + "%", DataTypes.Stringa)
                    .AddWhereCondition("cns_descrizione", Comparatori.Like, "%" + codiceDescrizioneLikeFilter + "%", DataTypes.Stringa, "OR")
                    .CloseParanthesis()
                End If
                If idUtente > 0 Then
                    .AddTables("T_ANA_LINK_UTENTI_CONSULTORI")
                    .AddWhereCondition("LUC_UTE_ID", Comparatori.Uguale, idUtente, DataTypes.Numero)
                    .AddWhereCondition("LUC_CNS_CODICE", Comparatori.Uguale, "CNS_CODICE", DataTypes.Join)
                End If
                If Not String.IsNullOrWhiteSpace(filtroCodDistretto) Then
                    .AddWhereCondition("CNS_DIS_CODICE", Comparatori.Uguale, filtroCodDistretto, DataTypes.Stringa)
                End If
                If Not String.IsNullOrWhiteSpace(filtroUsl) Then
                    .AddTables("T_ANA_DISTRETTI")
                    .AddWhereCondition("DIS_USL_CODICE", Comparatori.Uguale, filtroUsl, DataTypes.Stringa)
                    .AddWhereCondition("CNS_DIS_CODICE", Comparatori.Uguale, "DIS_CODICE", DataTypes.Join)
				End If
                If Not soloCnsAperti Then
                    .AddOrderByFields("cns_flag_apertura, cns_descrizione")
                Else
                    .AddOrderByFields("cns_descrizione")
                End If

            End With

		End Sub

		Public Function GetCnsDescrizione(codiceConsultorio As String) As String Implements IConsultoriProvider.GetCnsDescrizione

            Dim result As String = String.Empty

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("CNS_DESCRIZIONE")
                .AddTables("T_ANA_CONSULTORI")
                .AddWhereCondition("CNS_CODICE", Comparatori.Uguale, codiceConsultorio, DataTypes.Stringa)
            End With

            Using dr As IDataReader = _DAM.BuildDataReader()

                Try
                    If dr.Read() Then
                        result = dr("CNS_DESCRIZIONE")
                    End If

                Catch ex As Exception

                    LogError(ex)

                    ex.InternalPreserveStackTrace()
                    Throw

                End Try

            End Using

            Return result

        End Function

        Public Function GetConsultoriInCircoscrizione(codiceCircoscrizione As String) As DataTable Implements IConsultoriProvider.GetConsultoriInCircoscrizione

            Dim dt As New DataTable()

            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim adp As OracleClient.OracleDataAdapter = Nothing

            Dim dataCnv As Date = Date.MinValue

            Dim ownConnection As Boolean = False

            Try

                cmd = New OracleClient.OracleCommand(Queries.Consultori.OracleQueries.selConsultoriInCircoscrizione, Me.Connection)

                cmd.Parameters.AddWithValue("circodice", codiceCircoscrizione)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                adp = New OracleClient.OracleDataAdapter(cmd)
                adp.Fill(dt)

            Catch ex As Exception

                Me.SetErrorMsg("Errore lettura db per recupero dati consultorio")
                LogError(ex, "GetConsultoriInCircoscrizione: errore lettura db per recupero dati consultorio.")

                ex.InternalPreserveStackTrace()
                Throw

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not adp Is Nothing Then adp.Dispose()
                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return dt

        End Function

        Public Function GetConsultoriInComune(codiceComune As String) As DataTable Implements IConsultoriProvider.GetConsultoriInComune

            Dim dt As New DataTable()

            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim adp As OracleClient.OracleDataAdapter = Nothing

            Dim dataCnv As Date = Date.MinValue

            Dim ownConnection As Boolean = False

            Try
                cmd = New OracleClient.OracleCommand(Queries.Consultori.OracleQueries.selConsultoriInComune, Me.Connection)

                cmd.Parameters.AddWithValue("comcodice", codiceComune)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                adp = New OracleClient.OracleDataAdapter(cmd)
                adp.Fill(dt)

            Catch ex As Exception

                Me.SetErrorMsg("Errore lettura db per recupero dati consultorio")

                LogError(ex, "GetConsultoriInComune: errore lettura db per recupero dati consultorio.")

                ex.InternalPreserveStackTrace()
                Throw

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not adp Is Nothing Then adp.Dispose()
                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return dt

        End Function

        Public Function GetConsultoriPerGiorniNascita(giorniNascita As Integer) As DataTable Implements IConsultoriProvider.GetConsultoriPerGiorniNascita

            Dim dt As New DataTable()

            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim adp As OracleClient.OracleDataAdapter = Nothing

            Dim dataCnv As Date = Date.MinValue

            Dim ownConnection As Boolean = False

            Try
                cmd = New OracleClient.OracleCommand(Queries.Consultori.OracleQueries.selConsultoriPerGiorniNascita, Me.Connection)

                cmd.Parameters.AddWithValue("giorninascita", giorniNascita)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                adp = New OracleClient.OracleDataAdapter(cmd)
                adp.Fill(dt)

            Catch ex As Exception

                Me.SetErrorMsg("Errore lettura db per recupero dati consultorio")
                LogError(ex, "GetConsultoriPerGiorniNascita: errore lettura db per recupero dati consultorio.")

                ex.InternalPreserveStackTrace()
                Throw

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not adp Is Nothing Then adp.Dispose()
                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return dt

        End Function

        Public Function GetCircoscrizioniInConsultorio(codiceConsultorio As String) As DataTable Implements IConsultoriProvider.GetCircoscrizioniInConsultorio

            Dim dt As New DataTable()

            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim adp As OracleClient.OracleDataAdapter = Nothing

            Dim dataCnv As Date = Date.MinValue

            Dim ownConnection As Boolean = False

            Try
                cmd = New OracleClient.OracleCommand(Queries.Consultori.OracleQueries.selCircoscrizioniInConsultorio, Me.Connection)

                cmd.Parameters.AddWithValue("cnscodice", GetStringParam(codiceConsultorio, False))

                ownConnection = Me.ConditionalOpenConnection(cmd)

                adp = New OracleClient.OracleDataAdapter(cmd)
                adp.Fill(dt)

            Catch ex As Exception

                Me.SetErrorMsg("Errore lettura db per recupero dati consultorio")
                LogError(ex, "GetCircoscrizioniInConsultorio: errore lettura db per recupero dati consultorio.")

                ex.InternalPreserveStackTrace()
                Throw

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not adp Is Nothing Then adp.Dispose()
                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return dt

        End Function

        Public Function SelectDataUltimaConvocazioneCampagna(codiceConsultorio As String) As Date Implements IConsultoriProvider.SelectDataUltimaConvocazioneCampagna

            Dim dataCnv As Date = Date.MinValue

            Dim ownConnection As Boolean = False

            Using cmd As New OracleClient.OracleCommand(Queries.Consultori.OracleQueries.selDataConvocazioneCampagna, Me.Connection)

                cmd.Parameters.AddWithValue("cod_cns", GetStringParam(codiceConsultorio, False))

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then

                        Try
                            dataCnv = Convert.ToDateTime(obj)
                        Catch ex As Exception
                            dataCnv = Date.MinValue
                        End Try

                    End If

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return dataCnv

        End Function

        ''' <summary>
        ''' Restituisce un intervallo con l'età minima e massima di validità del consultorio specificato.
        ''' </summary>
        Public Function GetEtaValiditaConsultorio(codiceConsultorio As String) As Entities.EstremiIntervallo Implements IConsultoriProvider.GetEtaValiditaConsultorio

            Dim intervalloEta As Entities.EstremiIntervallo = Nothing

            Using cmd As New OracleClient.OracleCommand(Queries.Consultori.OracleQueries.selEtaValidita, Me.Connection)

                cmd.Parameters.AddWithValue("cod_cns", GetStringParam(codiceConsultorio, False))

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using dr As IDataReader = cmd.ExecuteReader()

                        If dr.Read() Then

                            Dim cns_da_eta As Int16 = dr.GetOrdinal("cns_da_eta")
                            Dim cns_a_eta As Int16 = dr.GetOrdinal("cns_a_eta")

                            intervalloEta = New Entities.EstremiIntervallo()

                            ' Gli estremi, su db, dovrebbero essere sempre impostati. 
                            ' Però non sono campi obbligatori, per cui li controllo.
                            If dr.IsDBNull(cns_da_eta) Then
                                intervalloEta.EstremoInferiore = 0
                            Else
                                intervalloEta.EstremoInferiore = dr.GetDecimal(cns_da_eta)
                            End If

                            If dr.IsDBNull(cns_a_eta) Then
                                intervalloEta.EstremoSuperiore = 0
                            Else
                                intervalloEta.EstremoSuperiore = dr.GetDecimal(cns_a_eta)
                            End If

                        End If

                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return intervalloEta

        End Function

        ''' <summary>
        ''' Restituisce il codice del comune associato al consultorio specificato.
        ''' </summary>
        Public Function GetComuneConsultorio(ByVal cnsCodice As String) As String Implements IConsultoriProvider.GetComuneConsultorio

            Dim codiceComune As String = String.Empty

            Using cmd As New OracleClient.OracleCommand(Queries.Consultori.OracleQueries.selComuneConsultorio, Me.Connection)

                cmd.Parameters.AddWithValue("cod_cns", GetStringParam(cnsCodice, False))

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If (Not obj Is Nothing AndAlso Not obj Is DBNull.Value) Then
                        codiceComune = obj.ToString()
                    End If

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return codiceComune

        End Function

        ''' <summary>
        ''' Restituisce il codice del comune associato al consultorio specificato
        ''' </summary>
        ''' <param name="cnsCodice"></param>
        ''' <returns></returns>
        Public Function GetTipoErogatoreConsultorio(cnsCodice As String) As String Implements IConsultoriProvider.GetTipoErogatoreConsultorio

            Dim tipoErogatore As String = String.Empty

            Using cmd As New OracleCommand(Queries.Consultori.OracleQueries.selTipoErogatoreConsultorio, Connection)

                cmd.Parameters.AddWithValue("cod_cns", GetStringParam(cnsCodice, False))

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        tipoErogatore = obj.ToString()
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return tipoErogatore

        End Function

        Public Function GetListCodiceDescrizioneConsultori(soloCnsAperti As Boolean, codiceDescrizioneLikeFilter As String) As List(Of Consultorio) Implements IConsultoriProvider.GetListCodiceDescrizioneConsultori

            Dim listConsultori As List(Of Consultorio) = Nothing

            CreaQueryCaricamentoConsultori(_DAM.QB, soloCnsAperti, False, codiceDescrizioneLikeFilter)

            listConsultori = ReaderListCodiceDescrizioneConsultori(_DAM)

            Return listConsultori

        End Function

        ''' <summary>
        ''' Restituisce il consultorio (aperto) in base al pediatra vaccinatore e all'età specificati
        ''' </summary>
        ''' <param name="codicePediatra"></param>
        ''' <param name="etaPaziente"></param>
        ''' <returns></returns>
        Public Function GetCnsByPediatra(codicePediatra As String, etaPaziente As Integer) As String Implements IConsultoriProvider.GetCnsByPediatra

            Dim codiceConsultorio As String = String.Empty

            Using cmd As New OracleCommand(Queries.Consultori.OracleQueries.selCnsByPediatra, Connection)

                cmd.Parameters.AddWithValue("cod_pediatra", GetStringParam(codicePediatra, False))
                cmd.Parameters.AddWithValue("eta_paziente", etaPaziente)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        codiceConsultorio = obj.ToString()
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return codiceConsultorio

        End Function

        ''' <summary>
        ''' Restituisce il consultorio (aperto) in base al comune e all'età specificati.
        ''' </summary>
        Public Function GetCnsByComune(codiceComune As String, etaPaziente As Integer) As String Implements IConsultoriProvider.GetCnsByComune

            Dim codiceConsultorio As String = String.Empty

            Using cmd As New OracleClient.OracleCommand(Queries.Consultori.OracleQueries.selCnsByComune, Me.Connection)

                cmd.Parameters.AddWithValue("eta_paziente", etaPaziente)
                cmd.Parameters.AddWithValue("cod_comune", GetStringParam(codiceComune, False))

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If (Not obj Is Nothing AndAlso Not obj Is DBNull.Value) Then
                        codiceConsultorio = obj.ToString()
                    End If

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return codiceConsultorio

        End Function

        ''' <summary>
        ''' Restituisce il consultorio (aperto) in base alla circoscrizione e all'età specificata.
        ''' </summary>
        Public Function GetCnsByCircoscrizione(codiceCircoscrizione As String, etaPaziente As Integer) As String Implements IConsultoriProvider.GetCnsByCircoscrizione

            Dim codiceConsultorio As String = String.Empty

            Using cmd As New OracleClient.OracleCommand(Queries.Consultori.OracleQueries.selCnsByCircoscrizione, Me.Connection)

                cmd.Parameters.AddWithValue("eta_paziente", etaPaziente)
                cmd.Parameters.AddWithValue("cod_circoscrizione", GetStringParam(codiceCircoscrizione, False))

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If (Not obj Is Nothing AndAlso Not obj Is DBNull.Value) Then
                        codiceConsultorio = obj.ToString()
                    End If

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return codiceConsultorio

        End Function

        ''' <summary>
        ''' Restituisce il consultorio di smistamento (aperto) in base all'età specificata.
        ''' </summary>
        Public Function GetCnsSmistamento(etaPaziente As Integer) As String Implements IConsultoriProvider.GetCnsSmistamento

            Dim codiceConsultorio As String = String.Empty

            Using cmd As New OracleClient.OracleCommand(Queries.Consultori.OracleQueries.selCnsSmistamento, Me.Connection)

                cmd.Parameters.AddWithValue("eta_paziente", etaPaziente)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If (Not obj Is Nothing AndAlso Not obj Is DBNull.Value) Then
                        codiceConsultorio = obj.ToString()
                    End If

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return codiceConsultorio

        End Function

        ''' <summary>
        ''' Restituisce true se il codice del consultorio specificato è presente in anagrafe consultori
        ''' </summary>
        Public Function ExistsConsultorio(codiceConsultorio As String) As Boolean Implements IConsultoriProvider.ExistsConsultorio

            Using cmd As New OracleClient.OracleCommand(Queries.Consultori.OracleQueries.existsConsultorio, Me.Connection)

                cmd.Parameters.AddWithValue("cns_codice", codiceConsultorio)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If (obj Is Nothing OrElse obj Is DBNull.Value) Then
                        Return False
                    End If

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return True

        End Function

        ''' <summary>
        ''' Restituisce il tipo del consultorio specificato
        ''' </summary>
        Public Function GetTipoConsultorio(codiceConsultorio As String) As String Implements IConsultoriProvider.GetTipoConsultorio

            With _DAM.QB
                .AddSelectFields("cns_tipo")
                .AddTables("t_ana_consultori")
                .AddWhereCondition("cns_codice", Comparatori.Uguale, codiceConsultorio, DataTypes.Stringa)
            End With

            Dim tipoCns As Object = _DAM.ExecScalar()

            If tipoCns Is Nothing OrElse tipoCns Is DBNull.Value Then
                Return String.Empty
            End If

            Return tipoCns.ToString()

        End Function

        ''' <summary>
        ''' Restituisce una lista con i codici dei consultori associati al distretto specificato
        ''' </summary>
        ''' <param name="codiceDistretto"></param>
        ''' <returns></returns>
        Public Function GetCodiciConsultoriDistretto(codiceDistretto As String) As List(Of String) Implements IConsultoriProvider.GetCodiciConsultoriDistretto

            Return GetCodiciConsultoriDistretto(codiceDistretto, False)

        End Function

        Public Function GetCodiciConsultoriDistretto(codiceDistretto As String, soloAperti As Boolean) As List(Of String) Implements IConsultoriProvider.GetCodiciConsultoriDistretto

            Dim listCodici As New List(Of String)()

            With _DAM.QB

                .NewQuery()

                .AddTables("T_ANA_CONSULTORI")
                .AddSelectFields("CNS_CODICE")

                .AddWhereCondition("CNS_DIS_CODICE", Comparatori.Uguale, codiceDistretto, DataTypes.Stringa)

                If soloAperti Then
                    .AddWhereCondition("cns_data_apertura", Comparatori.MinoreUguale, "SYSDATE", DataTypes.Replace)
                    .OpenParanthesis()
                    .AddWhereCondition("cns_data_chiusura", Comparatori.Maggiore, "SYSDATE", DataTypes.Replace)
                    .AddWhereCondition("cns_data_chiusura", Comparatori.Is, "NULL", DataTypes.Replace, "or")
                    .CloseParanthesis()
                End If

            End With

            Using dr As IDataReader = _DAM.BuildDataReader()

                If Not dr Is Nothing Then

                    While dr.Read()
                        listCodici.Add(dr(0))
                    End While

                End If

            End Using

            Return listCodici

        End Function

        ' TODO [App Auto]: TESTARE!!!
        Public Function GetCodiciConsultoriUsl(codiceUsl As String, soloAperti As Boolean) As List(Of String) Implements IConsultoriProvider.GetCodiciConsultoriUsl

            Dim listCodici As New List(Of String)()

            With _DAM.QB

                .NewQuery()

                .AddSelectFields("CNS_CODICE")
                .AddTables("T_ANA_DISTRETTI, T_ANA_CONSULTORI")
                .AddWhereCondition("DIS_CODICE", Comparatori.Uguale, "CNS_DIS_CODICE", DataTypes.Join)
                .AddWhereCondition("DIS_USL_CODICE", Comparatori.Uguale, codiceUsl, DataTypes.Stringa)

                If soloAperti Then
                    .AddWhereCondition("CNS_DATA_APERTURA", Comparatori.MinoreUguale, "SYSDATE", DataTypes.Replace)
                    .OpenParanthesis()
                    .AddWhereCondition("CNS_DATA_CHIUSURA", Comparatori.Maggiore, "SYSDATE", DataTypes.Replace)
                    .AddWhereCondition("CNS_DATA_CHIUSURA", Comparatori.Is, "NULL", DataTypes.Replace, "OR")
                    .CloseParanthesis()
                End If

            End With

            Using dr As IDataReader = _DAM.BuildDataReader()

                If Not dr Is Nothing Then

                    While dr.Read()
                        listCodici.Add(dr(0))
                    End While

                End If

            End Using

            Return listCodici

        End Function

        Public Function GetCampoStampa1(codiceConsultorio As String) As String Implements IConsultoriProvider.GetCampoStampa1

            Dim campoStampa1 As String = String.Empty

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("CNS_STAMPA1")
                .AddTables("T_ANA_CONSULTORI")
                .AddWhereCondition("CNS_CODICE", Comparatori.Uguale, codiceConsultorio, DataTypes.Stringa)
            End With

            Using idr As IDataReader = _DAM.BuildDataReader()

                If Not idr Is Nothing Then

                    If idr.Read() Then
                        campoStampa1 = idr.GetStringOrDefault(0)
                    End If

                End If


            End Using

            Return campoStampa1

        End Function

        Public Function GetConsultorioByMachineID(machineID As Integer) As Entities.Consultorio Implements IConsultoriProvider.GetConsultorioByMachineID

            Dim consultorio As Entities.Consultorio = Nothing

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("COG_CNS_CODICE, CNS_DESCRIZIONE")
                .AddTables("T_ANA_LINK_CONSULTORI_GP, T_ANA_POSTAZIONI, T_ANA_CONSULTORI")
                .AddWhereCondition("POS_ID", Comparatori.Uguale, machineID, DataTypes.Numero)
                .AddWhereCondition("POS_ID", Comparatori.Uguale, "COG_GP_CODICE", DataTypes.Join)
                .AddWhereCondition("COG_TIPO", Comparatori.Uguale, "2", DataTypes.Numero)
                .AddWhereCondition("CNS_CODICE", Comparatori.Uguale, "COG_CNS_CODICE", DataTypes.Join)
            End With

            Using idr As IDataReader = _DAM.BuildDataReader()

                consultorio = Me.GetConsultorioByPostazione(idr)

            End Using

            Return consultorio

        End Function

        Public Function GetConsultorioByMachineGroupID(machineGroupID As Integer) As Entities.Consultorio Implements IConsultoriProvider.GetConsultorioByMachineGroupID

            Dim consultorio As Entities.Consultorio = Nothing

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("COG_CNS_CODICE, CNS_DESCRIZIONE")
                .AddTables("T_ANA_LINK_CONSULTORI_GP, T_ANA_GRUPPI_POSTAZIONI, T_ANA_CONSULTORI")
                .AddWhereCondition("GRP_ID", Comparatori.Uguale, machineGroupID, DataTypes.Numero)
                .AddWhereCondition("GRP_ID", Comparatori.Uguale, "COG_GP_CODICE", DataTypes.Join)
                .AddWhereCondition("COG_TIPO", Comparatori.Uguale, "1", DataTypes.Numero)
                .AddWhereCondition("CNS_CODICE", Comparatori.Uguale, "COG_CNS_CODICE", DataTypes.Join)
            End With

            Using idr As IDataReader = _DAM.BuildDataReader()

                consultorio = Me.GetConsultorioByPostazione(idr)

            End Using

            Return consultorio

        End Function

        Public Function GetConsultoriAbilitatiUtente(idUtente As Long) As List(Of Consultorio) Implements IConsultoriProvider.GetConsultoriAbilitatiUtente

            Dim listConsultori As List(Of Consultorio) = Nothing

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("LUC_CNS_CODICE, CNS_DESCRIZIONE")
                .AddTables("T_ANA_LINK_UTENTI_CONSULTORI, T_ANA_CONSULTORI")
                .AddWhereCondition("LUC_UTE_ID", Comparatori.Uguale, idUtente, DataTypes.Numero)
                .AddWhereCondition("LUC_CNS_CODICE", Comparatori.Uguale, "CNS_CODICE", DataTypes.Join)
                .AddOrderByFields("CNS_DESCRIZIONE")
            End With

            Using idr As IDataReader = _DAM.BuildDataReader()

                listConsultori = GetConsultoriAbilitatiUtente(idr)

            End Using

            Return listConsultori

        End Function

        Public Function GetConsultorioDefaultUtente(idUtente As Long) As Entities.Consultorio Implements IConsultoriProvider.GetConsultorioDefaultUtente

            Dim consultorio As Entities.Consultorio = Nothing

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("LUC_CNS_CODICE, CNS_DESCRIZIONE")
                .AddTables("T_ANA_LINK_UTENTI_CONSULTORI, T_ANA_CONSULTORI")
                .AddWhereCondition("LUC_UTE_ID", Comparatori.Uguale, idUtente, DataTypes.Numero)
                .AddWhereCondition("LUC_DEFAULT", Comparatori.Uguale, "S", DataTypes.Stringa)
                .AddWhereCondition("LUC_CNS_CODICE", Comparatori.Uguale, "CNS_CODICE", DataTypes.Join)
            End With

            Using idr As IDataReader = _DAM.BuildDataReader()

                Dim listConsultori As List(Of Entities.Consultorio) = Me.GetConsultoriAbilitatiUtente(idr)

                If Not listConsultori Is Nothing AndAlso listConsultori.Count > 0 Then
                    consultorio = listConsultori(0)
                End If

            End Using

            Return consultorio

        End Function

        Public Sub VerificaAssociazioneAutomaticaConsultori(idUtente As Long, cnsDefault As String) Implements IConsultoriProvider.VerificaAssociazioneAutomaticaConsultori

            'verifico se la funzionalità è abilitata da parametro
            Dim sql As String =
                "select count(*) as c FROM T_ANA_LINK_UTENTI_CONSULTORI WHERE LUC_UTE_ID = :ute "

            Dim genera As Boolean = True
            Dim ownConnection As Boolean = False

            Using cmd As New OracleClient.OracleCommand(sql, Me.Connection)

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    ' verifico la presenza di consultori associati
                    cmd.Parameters.Add(New OracleClient.OracleParameter("ute", idUtente))
                    Dim obj As Object = cmd.ExecuteScalar()
                    genera = (obj Is Nothing OrElse obj Is DBNull.Value OrElse obj = 0)

                    If genera Then

                        cmd.CommandText =
                            "insert into T_ANA_LINK_UTENTI_CONSULTORI(LUC_UTE_ID,LUC_CNS_CODICE,LUC_DEFAULT) " +
                            "SELECT :ute,cns_codice, 'N' " +
                            "FROM T_ANA_CONSULTORI " +
                            "where(CNS_DATA_APERTURA < SYSDATE Or CNS_DATA_APERTURA Is NULL) And (SYSDATE < CNS_DATA_CHIUSURA Or CNS_DATA_CHIUSURA Is NULL) "

                        cmd.ExecuteNonQuery()

                        cmd.CommandText =
                            "update T_ANA_LINK_UTENTI_CONSULTORI set LUC_DEFAULT='S'" +
                            "where LUC_UTE_ID= :ute " +
                            "and LUC_CNS_CODICE= :cns "

                        cmd.Parameters.Add(New OracleClient.OracleParameter("cns", cnsDefault))
                        cmd.ExecuteNonQuery()

                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

        End Sub

        Public Function GetDateInfoConsultorio(codiceConsultorio As String) As Entities.ConsultorioDateInfo Implements IConsultoriProvider.GetDateInfoConsultorio

            Dim consultorioDateInfo As Entities.ConsultorioDateInfo = Nothing

            Dim query As String = "select cns_sta_dinizio, cns_sta_dfine, cns_sta_dinizio_bil, cns_sta_dfine_bil, " +
                                  "cns_crea_dcnv, cns_cerca_dcnv, cns_sta_dinizio_mag, cns_sta_dfine_mag " +
                                  "from t_ana_consultori where cns_codice = :cns_codice"

            Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                cmd.Parameters.AddWithValue("cns_codice", codiceConsultorio)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim cns_sta_dinizio As Integer = idr.GetOrdinal("cns_sta_dinizio")
                            Dim cns_sta_dfine As Integer = idr.GetOrdinal("cns_sta_dfine")
                            Dim cns_sta_dinizio_bil As Integer = idr.GetOrdinal("cns_sta_dinizio_bil")
                            Dim cns_sta_dfine_bil As Integer = idr.GetOrdinal("cns_sta_dfine_bil")
                            Dim cns_crea_dcnv As Integer = idr.GetOrdinal("cns_crea_dcnv")
                            Dim cns_cerca_dcnv As Integer = idr.GetOrdinal("cns_cerca_dcnv")
                            Dim cns_sta_dinizio_mag As Integer = idr.GetOrdinal("cns_sta_dinizio_mag")
                            Dim cns_sta_dfine_mag As Integer = idr.GetOrdinal("cns_sta_dfine_mag")

                            If idr.Read() Then

                                consultorioDateInfo = New Entities.ConsultorioDateInfo()
                                consultorioDateInfo.DataUltimaStampaAvvisoInizio = idr.GetNullableDateTimeOrDefault(cns_sta_dinizio)
                                consultorioDateInfo.DataUltimaStampaAvvisoFine = idr.GetNullableDateTimeOrDefault(cns_sta_dfine)
                                consultorioDateInfo.DataUltimaStampaBilancioInizio = idr.GetNullableDateTimeOrDefault(cns_sta_dinizio_bil)
                                consultorioDateInfo.DataUltimaStampaBilancioFine = idr.GetNullableDateTimeOrDefault(cns_sta_dfine_bil)
                                consultorioDateInfo.DataCreazioneUltimaCampagna = idr.GetNullableDateTimeOrDefault(cns_crea_dcnv)
                                consultorioDateInfo.DataUltimaRicercaConvocazioni = idr.GetNullableDateTimeOrDefault(cns_cerca_dcnv)
                                consultorioDateInfo.DataUltimaStampaMaggiorenniInizio = idr.GetNullableDateTimeOrDefault(cns_sta_dinizio_mag)
                                consultorioDateInfo.DataUltimaStampaMaggiorenniFine = idr.GetNullableDateTimeOrDefault(cns_sta_dfine_mag)

                            End If
                        End If

                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return consultorioDateInfo

        End Function

        Public Function GetStrutture(codiceStruttura As String, tipoErogatore As String) As List(Of Entities.Struttura) Implements IConsultoriProvider.GetStrutture
            Dim strutture As New List(Of Entities.Struttura)
            Dim query As String = " select * from V_ANA_STRUTTURE where 1 = 1 "
            Using cmd As New OracleClient.OracleCommand()
                If Not String.IsNullOrWhiteSpace(codiceStruttura) Then
                    query += " and ast_codice = :codiceStruttura"
                    cmd.Parameters.AddWithValue("codiceStruttura", codiceStruttura)
                End If

                If Not String.IsNullOrWhiteSpace(tipoErogatore) Then
                    query += " and ast_tipo_erogatore = :tipoErogatore"
                    cmd.Parameters.AddWithValue("tipoErogatore", tipoErogatore)
                End If

                cmd.CommandText = query
                cmd.Connection = Me.Connection

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim ast_codice As Integer = idr.GetOrdinal("ast_codice")
                            Dim ast_codice_struttura As Integer = idr.GetOrdinal("ast_codice_struttura")
                            Dim ast_descrizione As Integer = idr.GetOrdinal("ast_descrizione")
                            Dim ast_codice_comune As Integer = idr.GetOrdinal("ast_codice_comune")
                            Dim ast_comune As Integer = idr.GetOrdinal("ast_comune")
                            Dim ast_indirizzo As Integer = idr.GetOrdinal("ast_indirizzo")
                            Dim ast_data_apertura As Integer = idr.GetOrdinal("ast_data_apertura")
                            Dim ast_data_chiusura As Integer = idr.GetOrdinal("ast_data_chiusura")
                            Dim ast_regione As Integer = idr.GetOrdinal("ast_regione")
                            Dim ast_asl As Integer = idr.GetOrdinal("ast_asl")
                            Dim ast_usl_codice As Integer = idr.GetOrdinal("ast_usl_codice")
                            Dim ast_stato As Integer = idr.GetOrdinal("ast_stato")
                            Dim ast_tipo_erogatore As Integer = idr.GetOrdinal("ast_tipo_erogatore")

                            While idr.Read()
                                Dim item As New Struttura()

                                item.Codice = idr.GetStringOrDefault(ast_codice)
                                item.CodiceStruttura = idr.GetStringOrDefault(ast_codice_struttura)
                                item.Descrizione = idr.GetStringOrDefault(ast_descrizione)
                                item.CodiceComune = idr.GetStringOrDefault(ast_codice_comune)
                                item.Comune = idr.GetStringOrDefault(ast_comune)
                                item.Indirizzo = idr.GetStringOrDefault(ast_indirizzo)
                                item.DataApertura = idr.GetNullableDateTimeOrDefault(ast_data_apertura)
                                item.DataChiusura = idr.GetNullableDateTimeOrDefault(ast_data_chiusura)
                                item.Regione = idr.GetStringOrDefault(ast_regione)
                                item.Asl = idr.GetStringOrDefault(ast_asl)
                                item.CodiceAsl = idr.GetStringOrDefault(ast_usl_codice)
                                item.Stato = idr.GetStringOrDefault(ast_stato)
                                item.TipoErogatore = idr.GetStringOrDefault(ast_tipo_erogatore)

                                strutture.Add(item)

                            End While

                        End If
                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return strutture
        End Function

        ''' <summary>
        ''' Restituisce il valore del campo relativo alla richiesta di consenso al trattamento dati per l'utente
        ''' </summary>
        ''' <param name="codiceConsultorio"></param>
        ''' <returns></returns>
        Public Function GetRichiestaConsensoTrattamentoDatiUtente(codiceConsultorio As String) As String Implements IConsultoriProvider.GetRichiestaConsensoTrattamentoDatiUtente

            Dim valore As String = String.Empty

            Using cmd As New OracleCommand("SELECT CNS_CONSENSO_TRATTAM_DATI_OPER FROM T_ANA_CONSULTORI WHERE CNS_CODICE = :CNS_CODICE", Connection)

                cmd.Parameters.AddWithValue("CNS_CODICE", codiceConsultorio)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                        valore = obj.ToString()
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return valore

        End Function

        ''' <summary>
        ''' Restituisce il codice del consultorio associato all'RSA specificata
        ''' </summary>
        ''' <param name="idRSA"></param>
        ''' <returns></returns>
        Public Function GetCodiceConsultorioRSA(idRSA As String) As String Implements IConsultoriProvider.GetCodiceConsultorioRSA

            Dim cod As String = String.Empty

            Using cmd As New OracleCommand("SELECT RSA_CODICE FROM V_ANA_RSA WHERE RSA_ID = :RSA_ID", Connection)

                cmd.Parameters.AddWithValue("RSA_ID", idRSA)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                        cod = obj.ToString()
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return cod

        End Function

#Region " OnVac API "

        ''' <summary>
        ''' Restituisce l'elenco di consultori e ambulatorio aperti.
        ''' </summary>
        ''' <param name="escludiSmistamento"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetListInfoConsultori(escludiSmistamento As Boolean) As List(Of Entities.ConsultorioInfoAPP) Implements IConsultoriProvider.GetListInfoConsultori

            Dim listInfoConsultori As List(Of Entities.ConsultorioInfoAPP) = Nothing

            Dim query As New System.Text.StringBuilder(Me.GetQuerySelectInfoConsultori())
            query.Append("where cns_data_apertura <= sysdate ")
            query.Append("and (cns_data_chiusura is null or cns_data_chiusura > sysdate) ")
            If escludiSmistamento Then query.Append("and (cns_smistamento is null or cns_smistamento <> 'S') ")
            query.Append("and amb_data_apertura <= sysdate ")
            query.Append("and (amb_data_chiusura is null or amb_data_chiusura > sysdate) ")
            query.Append("order by cns_descrizione, amb_descrizione ")

            Using cmd As New OracleClient.OracleCommand(query.ToString(), Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    listInfoConsultori = Me.GetListInfoConsultori(cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listInfoConsultori

        End Function

        ''' <summary>
        ''' Restituisce le info sul consultorio specificato, inclusi ambulatori e orari.
        ''' </summary>
        ''' <param name="codiceConsultorio"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetListInfoConsultorio(codiceConsultorio As String) As List(Of Entities.ConsultorioInfoAPP) Implements IConsultoriProvider.GetListInfoConsultorio

            Dim listInfoConsultori As List(Of Entities.ConsultorioInfoAPP) = Nothing

            Dim query As New System.Text.StringBuilder(Me.GetQuerySelectInfoConsultori())
            query.Append("where cns_codice = :codiceConsultorio ")
            query.Append("and amb_data_apertura <= sysdate ")
            query.Append("and (amb_data_chiusura is null or amb_data_chiusura > sysdate) ")
            query.Append("order by cns_descrizione, amb_descrizione ")

            Using cmd As New OracleClient.OracleCommand(query.ToString(), Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("codiceConsultorio", codiceConsultorio)

                    listInfoConsultori = Me.GetListInfoConsultori(cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listInfoConsultori

        End Function

        Private Function GetListInfoConsultori(cmd As OracleCommand) As List(Of Entities.ConsultorioInfoAPP)

            Dim listInfoConsultori As New List(Of Entities.ConsultorioInfoAPP)()

            Using idr As IDataReader = cmd.ExecuteReader()

                If Not idr Is Nothing Then

                    Dim cns_codice As Integer = idr.GetOrdinal("cns_codice")
                    Dim cns_descrizione As Integer = idr.GetOrdinal("cns_descrizione")
                    Dim cns_indirizzo As Integer = idr.GetOrdinal("cns_indirizzo")
                    Dim cns_n_telefono As Integer = idr.GetOrdinal("cns_n_telefono")
                    Dim cns_com_codice As Integer = idr.GetOrdinal("cns_com_codice")
                    Dim com_descrizione As Integer = idr.GetOrdinal("com_descrizione")
                    Dim cns_stampa2 As Integer = idr.GetOrdinal("cns_stampa2")
                    Dim amb_codice As Integer = idr.GetOrdinal("amb_codice")
                    Dim amb_descrizione As Integer = idr.GetOrdinal("amb_descrizione")
                    Dim countGiorn As Integer = idr.GetOrdinal("countGiorn")

                    While idr.Read()

                        Dim infoConsultorio As New Entities.ConsultorioInfoAPP()

                        infoConsultorio.CodiceConsultorio = idr.GetString(cns_codice)
                        infoConsultorio.DescrizioneConsultorio = idr.GetStringOrDefault(cns_descrizione)
                        infoConsultorio.Indirizzo = idr.GetStringOrDefault(cns_indirizzo)
                        infoConsultorio.Telefono = idr.GetStringOrDefault(cns_n_telefono)
                        infoConsultorio.CodiceComune = idr.GetStringOrDefault(cns_com_codice)
                        infoConsultorio.DescrizioneComune = idr.GetStringOrDefault(com_descrizione)
                        infoConsultorio.Note = idr.GetStringOrDefault(cns_stampa2)
                        infoConsultorio.CodiceAmbulatorio = idr.GetInt32OrDefault(amb_codice)
                        infoConsultorio.DescrizioneAmbulatorio = idr.GetStringOrDefault(amb_descrizione)
                        infoConsultorio.CountOrariGiornalieri = idr.GetInt32OrDefault(countGiorn)

                        listInfoConsultori.Add(infoConsultorio)

                    End While
                End If

            End Using

            Return listInfoConsultori

        End Function

        Private Function GetQuerySelectInfoConsultori() As String

            Dim query As New System.Text.StringBuilder()
            query.Append("select cns_codice, cns_descrizione, cns_indirizzo, cns_n_telefono, cns_com_codice, com_descrizione, ")
            query.Append("       cns_stampa2, amb_codice, amb_descrizione, ")
            query.Append("       (select count(*) from t_ana_orari_giornalieri ")
            query.Append("        where not (org_am_inizio is null and org_am_fine is null and org_pm_inizio is null and org_pm_fine is null) ")
            query.Append("        and org_amb_codice = amb_codice) countGiorn ")
            query.Append("from t_ana_consultori ")
            query.Append("  left join t_ana_comuni on cns_com_codice = com_codice ")
            query.Append("  left join t_ana_ambulatori on amb_cns_codice = cns_codice ")

            Return query.ToString()

        End Function

        ''' <summary>
        ''' Restituisce la lista degli orari di prenotazione per gli ambulatori specificati (dalla t_ana_orari_appuntamenti)
        ''' </summary>
        ''' <param name="codiciAmbulatori"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetOrariAppuntamentiAmbulatori(codiciAmbulatori As List(Of Integer)) As List(Of Entities.OrarioInfoAPP) Implements IConsultoriProvider.GetOrariAppuntamentiAmbulatori

            Return Me.GetOrariAmbulatorio(codiciAmbulatori, True)

        End Function

        ''' <summary>
        ''' Restituisce una lista degli orari di apertura per gli ambulatori specificati (dalla t_ana_orari_giornalieri)
        ''' </summary>
        ''' <param name="codiciAmbulatori"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetOrariGiornalieriAmbulatori(codiciAmbulatori As List(Of Integer)) As List(Of Entities.OrarioInfoAPP) Implements IConsultoriProvider.GetOrariGiornalieriAmbulatori

            Return Me.GetOrariAmbulatorio(codiciAmbulatori, False)

        End Function

        Private Function GetOrariAmbulatorio(codiciAmbulatori As List(Of Integer), fromOrariAppuntamenti As Boolean) As List(Of Entities.OrarioInfoAPP)

            If codiciAmbulatori Is Nothing OrElse codiciAmbulatori.Count = 0 Then Return Nothing

            Dim prefix As String = String.Empty
            Dim table As String = String.Empty

            If fromOrariAppuntamenti Then
                prefix = "ora"
                table = "t_ana_orari_appuntamenti"
            Else
                prefix = "org"
                table = "t_ana_orari_giornalieri"
            End If

            Dim filtroInResult As DbProvider.GetInFilterResult = Me.GetInFilter(codiciAmbulatori)

            Dim query As New System.Text.StringBuilder()
            query.AppendFormat("select {0}_amb_codice amb_codice, {0}_giorno giorno, {0}_am_inizio am_inizio, {0}_am_fine am_fine, {0}_pm_inizio pm_inizio, {0}_pm_fine pm_fine ", prefix)
            query.AppendFormat("from {0} ", table)
            query.AppendFormat("where {0}_amb_codice in ({1}) ", prefix, filtroInResult.InFilter)
            query.AppendFormat("and not ({0}_am_inizio is null and {0}_am_fine is null and {0}_pm_inizio is null and {0}_pm_fine is null) ", prefix)
            query.Append("order by amb_codice, giorno, am_inizio, am_fine, pm_inizio, pm_fine ")

            Dim listOrari As New List(Of Entities.OrarioInfoAPP)()

            Using cmd As New OracleClient.OracleCommand(query.ToString(), Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddRange(filtroInResult.Parameters)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim amb_codice As Integer = idr.GetOrdinal("amb_codice")
                            Dim giorno As Integer = idr.GetOrdinal("giorno")
                            Dim am_inizio As Integer = idr.GetOrdinal("am_inizio")
                            Dim am_fine As Integer = idr.GetOrdinal("am_fine")
                            Dim pm_inizio As Integer = idr.GetOrdinal("pm_inizio")
                            Dim pm_fine As Integer = idr.GetOrdinal("pm_fine")

                            While idr.Read()

                                Dim orario As New Entities.OrarioInfoAPP()

                                orario.CodiceAmbulatorio = idr.GetInt32OrDefault(amb_codice)
                                orario.Giorno = idr.GetString(giorno)
                                orario.OraInizioAM = idr.GetNullableDateTimeOrDefault(am_inizio)
                                orario.OraFineAM = idr.GetNullableDateTimeOrDefault(am_fine)
                                orario.OraInizioPM = idr.GetNullableDateTimeOrDefault(pm_inizio)
                                orario.OraFinePM = idr.GetNullableDateTimeOrDefault(pm_fine)

                                listOrari.Add(orario)

                            End While
                        End If

                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listOrari

        End Function

#End Region

#End Region

#Region " Insert / Update "

        Public Function UpdateDataUltimaConvocazioneCampagna(codiceConsultorio As String, dataConvocazione As Date) As Integer Implements IConsultoriProvider.UpdateDataUltimaConvocazioneCampagna

            Dim count As Integer = 0

            Using cmd As New OracleClient.OracleCommand(Queries.Consultori.OracleQueries.updDataConvocazioneCampagna, Me.Connection)

                cmd.Parameters.AddWithValue("data_cnv", GetDateParam(dataConvocazione))
                cmd.Parameters.AddWithValue("cod_cns", codiceConsultorio)

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

#Region " Update date di stampa "

        ''' <summary>
        ''' Update date ultima stampa avvisi, per il consultorio selezionato
        ''' </summary>
        ''' <param name="codiceConsultorio"></param>
        ''' <param name="dataIniziale"></param>
        ''' <param name="dataFinale"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateDateUltimaStampaAvvisi(codiceConsultorio As String, dataIniziale As DateTime, dataFinale As DateTime) As Integer Implements IConsultoriProvider.UpdateDateUltimaStampaAvvisi

            With _DAM.QB
                .NewQuery()
                .AddTables("T_ANA_CONSULTORI")
                .AddUpdateField("CNS_STA_DINIZIO", dataIniziale, DataTypes.Data)
                .AddUpdateField("CNS_STA_DFINE", dataFinale, DataTypes.Data)
                .AddWhereCondition("CNS_CODICE", Comparatori.Uguale, codiceConsultorio, DataTypes.Stringa)
            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Update)

        End Function

        ''' <summary>
        ''' Update date ultima stampa avvisi, per tutti i consultori aperti
        ''' </summary>
        ''' <param name="dataIniziale"></param>
        ''' <param name="dataFinale"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateDateUltimaStampaAvvisi(dataIniziale As DateTime, dataFinale As DateTime) As Integer Implements IConsultoriProvider.UpdateDateUltimaStampaAvvisi

            With _DAM.QB
                .NewQuery()
                .AddTables("T_ANA_CONSULTORI")
                .AddUpdateField("CNS_STA_DINIZIO", dataIniziale, DataTypes.Data)
                .AddUpdateField("CNS_STA_DFINE", dataFinale, DataTypes.Data)
                .OpenParanthesis()
                .AddWhereCondition("CNS_DATA_CHIUSURA", Comparatori.Is, "NULL", DataTypes.Replace)
                .AddWhereCondition("CNS_DATA_CHIUSURA", Comparatori.Maggiore, DateTime.Now, DataTypes.Data, "OR")
                .CloseParanthesis()
            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Update)

        End Function

        ''' <summary>
        ''' Update date ultima stampa avvisi bilancio, per il consultorio selezionato
        ''' </summary>
        ''' <param name="codiceConsultorio"></param>
        ''' <param name="dataIniziale"></param>
        ''' <param name="dataFinale"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateDateUltimaStampaAvvisiBilancio(codiceConsultorio As String, dataIniziale As DateTime, dataFinale As DateTime) As Integer Implements IConsultoriProvider.UpdateDateUltimaStampaAvvisiBilancio

            With _DAM.QB
                .NewQuery()
                .AddTables("T_ANA_CONSULTORI")
                .AddUpdateField("CNS_STA_DINIZIO_BIL", dataIniziale, DataTypes.Data)
                .AddUpdateField("CNS_STA_DFINE_BIL", dataFinale, DataTypes.Data)
                If String.IsNullOrWhiteSpace(codiceConsultorio) Then
                    .OpenParanthesis()
                    .AddWhereCondition("CNS_DATA_CHIUSURA", Comparatori.Is, "NULL", DataTypes.Replace)
                    .AddWhereCondition("CNS_DATA_CHIUSURA", Comparatori.Maggiore, DateTime.Now, DataTypes.Data, "OR")
                    .CloseParanthesis()
                Else
                    .AddWhereCondition("CNS_CODICE", Comparatori.Uguale, codiceConsultorio, DataTypes.Stringa)
                End If
            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Update)

        End Function

        ''' <summary>
        ''' Update date ultima stampa avvisi bilancio, per tutti i consultori aperti
        ''' </summary>
        ''' <param name="dataIniziale"></param>
        ''' <param name="dataFinale"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateDateUltimaStampaAvvisiBilancio(dataIniziale As DateTime, dataFinale As DateTime) As Integer Implements IConsultoriProvider.UpdateDateUltimaStampaAvvisiBilancio

            With _DAM.QB
                .NewQuery()
                .AddTables("T_ANA_CONSULTORI")
                .AddUpdateField("CNS_STA_DINIZIO_BIL", dataIniziale, DataTypes.Data)
                .AddUpdateField("CNS_STA_DFINE_BIL", dataFinale, DataTypes.Data)
                .OpenParanthesis()
                .AddWhereCondition("CNS_DATA_CHIUSURA", Comparatori.Is, "NULL", DataTypes.Replace)
                .AddWhereCondition("CNS_DATA_CHIUSURA", Comparatori.Maggiore, DateTime.Now, DataTypes.Data, "OR")
                .CloseParanthesis()
            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Update)

        End Function

        ''' <summary>
        ''' Update date ultima stampa avviso maggiorenni
        ''' </summary>
        ''' <param name="codiceConsultorio"></param>
        ''' <param name="dataIniziale"></param>
        ''' <param name="dataFinale"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateDateUltimaStampaAvvisoMaggiorenni(codiceConsultorio As String, dataIniziale As DateTime, dataFinale As DateTime) As Integer Implements IConsultoriProvider.UpdateDateUltimaStampaAvvisoMaggiorenni

            With _DAM.QB
                .NewQuery()
                .AddTables("T_ANA_CONSULTORI")
                .AddUpdateField("CNS_STA_DINIZIO_MAG", dataIniziale, DataTypes.Data)
                .AddUpdateField("CNS_STA_DFINE_MAG", dataFinale, DataTypes.Data)
                .AddWhereCondition("CNS_CODICE", Comparatori.Uguale, codiceConsultorio, DataTypes.Stringa)
            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Update)

        End Function

#End Region

#End Region

#Region " Private "

		Private Function GetConsultorioByPostazione(idr As IDataReader) As Entities.Consultorio

			Dim consultorio As Entities.Consultorio = Nothing

			If Not idr Is Nothing Then

				Dim cog_cns_codice As Int16 = idr.GetOrdinal("COG_CNS_CODICE")
				Dim cns_descrizione As Int16 = idr.GetOrdinal("CNS_DESCRIZIONE")

				If idr.Read() Then

					consultorio = New Entities.Consultorio()
					consultorio.Codice = idr.GetStringOrDefault(cog_cns_codice)
					consultorio.Descrizione = idr.GetStringOrDefault(cns_descrizione)



				End If

			End If

			Return consultorio

		End Function
		Private Sub CreaQueryCaricamentoConsultori(ByRef qb As AbstractQB, soloCnsAperti As Boolean, usaAliasCodiceDescrizione As Boolean, codiceDescrizioneLikeFilter As String)

			With qb

				.NewQuery()

				If usaAliasCodiceDescrizione Then
					.AddSelectFields("cns_codice codice, cns_descrizione descrizione")
				Else
					.AddSelectFields("cns_codice, cns_descrizione")
				End If

				.AddTables("t_ana_consultori")

				.AddWhereCondition("1", Comparatori.Uguale, "1", DataTypes.Replace)

				If soloCnsAperti Then
					.AddWhereCondition("cns_data_apertura", Comparatori.MinoreUguale, "SYSDATE", DataTypes.Replace)
					.OpenParanthesis()
					.AddWhereCondition("cns_data_chiusura", Comparatori.Maggiore, "SYSDATE", DataTypes.Replace)
					.AddWhereCondition("cns_data_chiusura", Comparatori.Is, "NULL", DataTypes.Stringa, "or")
					.CloseParanthesis()
				End If

				If Not String.IsNullOrEmpty(codiceDescrizioneLikeFilter) Then
					.OpenParanthesis()
					.AddWhereCondition("cns_codice", Comparatori.Like, "%" + codiceDescrizioneLikeFilter + "%", DataTypes.Stringa)
					.AddWhereCondition("cns_descrizione", Comparatori.Like, "%" + codiceDescrizioneLikeFilter + "%", DataTypes.Stringa, "OR")
					.CloseParanthesis()
				End If

				.AddOrderByFields("cns_descrizione")

			End With

		End Sub
		Private Function ReaderListCodiceDescrizioneConsultori(dam As IDAM) As List(Of Entities.Consultorio)
			Dim listConsultori As List(Of Entities.Consultorio) = Nothing
			Using dr As IDataReader = dam.BuildDataReader()

				If Not dr Is Nothing Then

					Dim cns_codice As Integer = dr.GetOrdinal("cns_codice")
					Dim cns_descrizione As Integer = dr.GetOrdinal("cns_descrizione")

					listConsultori = New List(Of Entities.Consultorio)()

					While dr.Read()

						Dim cns As New Entities.Consultorio()

						cns.Codice = dr.GetStringOrDefault(cns_codice)
						cns.Descrizione = dr.GetStringOrDefault(cns_descrizione)

						listConsultori.Add(cns)

					End While

				End If

			End Using
			Return listConsultori
		End Function
		Private Function ReaderListCodiceDescrizioneConsultoriUlss(dam As IDAM) As List(Of Entities.ConsultorioUlss)
			Dim listConsultoriAmbito As List(Of Entities.ConsultorioUlss) = Nothing
			Using dr As IDataReader = dam.BuildDataReader()

				If Not dr Is Nothing Then

					Dim cns_codice As Integer = dr.GetOrdinal("cns_codice")
					Dim cns_descrizione As Integer = dr.GetOrdinal("cns_descrizione")
					Dim dis_codice As Integer = dr.GetOrdinal("dis_codice")
					Dim dis_descrizione As Integer = dr.GetOrdinal("dis_descrizione")
					Dim amt_codice As Integer = dr.GetOrdinal("amt_codice")
					Dim amt_descrizione As Integer = dr.GetOrdinal("amt_descrizione")

					listConsultoriAmbito = New List(Of Entities.ConsultorioUlss)()

					While dr.Read()

						Dim cns As New Entities.ConsultorioUlss()

						cns.CodiceConsultorio = dr.GetStringOrDefault(cns_codice)
						cns.DescrizioneConsultorio = dr.GetStringOrDefault(cns_descrizione)
						cns.CodiceDistretto = dr.GetStringOrDefault(dis_codice)
						cns.DescrizioneDistretto = dr.GetStringOrDefault(dis_descrizione)
						cns.CodiceUlss = dr.GetStringOrDefault(amt_codice)
						cns.DescrizioneUlss = dr.GetStringOrDefault(amt_descrizione)

						listConsultoriAmbito.Add(cns)

					End While

				End If

			End Using
			Return listConsultoriAmbito
		End Function

		Private Sub CreaQueryCaricamentoConsultoriAmbiti(ByRef qb As AbstractQB, soloCnsAperti As Boolean)

			With qb

				.NewQuery()

				.AddSelectFields("cns_codice, cns_descrizione, usl_codice, usl_descrizione, dis_codice, dis_descrizione")

				.AddTables("t_ana_consultori, t_ana_USL, t_ana_distretti")

				.AddWhereCondition("1", Comparatori.Uguale, "1", DataTypes.Replace)
				.AddWhereCondition("cns_dis_codice", Comparatori.Uguale, "dis_codice", DataTypes.OutJoinLeft)
				.AddWhereCondition("dis_usl_codice", Comparatori.Uguale, "usl_codice", DataTypes.OutJoinLeft)

				If soloCnsAperti Then
					.AddWhereCondition("cns_data_apertura", Comparatori.MinoreUguale, "SYSDATE", DataTypes.Replace)
					.OpenParanthesis()
					.AddWhereCondition("cns_data_chiusura", Comparatori.Maggiore, "SYSDATE", DataTypes.Replace)
					.AddWhereCondition("cns_data_chiusura", Comparatori.Is, "NULL", DataTypes.Stringa, "or")
					.CloseParanthesis()
				End If

				.AddOrderByFields("usl_codice, dis_descrizione, cns_descrizione")

			End With

		End Sub


		Private Function GetConsultoriAbilitatiUtente(idr As IDataReader) As List(Of Consultorio)

            Dim listConsultori As New List(Of Consultorio)()

            If Not idr Is Nothing Then

                Dim cog_cns_codice As Int16 = idr.GetOrdinal("LUC_CNS_CODICE")
                Dim cns_descrizione As Int16 = idr.GetOrdinal("CNS_DESCRIZIONE")

                While idr.Read()

                    Dim consultorio As New Consultorio()
                    consultorio.Codice = idr.GetStringOrDefault(cog_cns_codice)
                    consultorio.Descrizione = idr.GetStringOrDefault(cns_descrizione)

                    listConsultori.Add(Consultorio)

                End While

            End If

            Return listConsultori

        End Function

#End Region

    End Class

End Namespace