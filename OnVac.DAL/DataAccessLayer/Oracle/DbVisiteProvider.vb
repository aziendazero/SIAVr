Imports System.Collections.Generic
Imports System.Data.OracleClient
Imports Onit.Database.DataAccessManager

Namespace DAL

    Public Class DbVisiteProvider
        Inherits DbProvider
        Implements IVisiteProvider

#Region " Constructors "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " Public Methods "

#Region " Select "

#Region " Id visita "

        ''' <summary>
        ''' Restituisce l'id della visita del paziente nella data specificata (senza considerare malattia nè bilancio)
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataVisita"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetIdVisitaPazienteByData(codicePaziente As Integer, dataVisita As Date) As Long? Implements IVisiteProvider.GetIdVisitaPazienteByData

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("VIS_ID")
                .AddTables("T_VIS_VISITE")
                .AddWhereCondition("VIS_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("VIS_DATA_VISITA", Comparatori.Uguale, dataVisita, DataTypes.Data)
            End With

            Dim visId As Object = _DAM.ExecScalar()

            If visId Is Nothing Then
                Return Nothing
            Else
                Return Convert.ToInt64(visId)
            End If

        End Function

        ''' <summary>
        ''' Restituisce l'id della visita per il paziente nella data specificata e con malattia nulla
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataVisita"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetIdVisitaPazienteSenzaMalattiaByData(codicePaziente As Int64, dataVisita As DateTime) As Int64? Implements IVisiteProvider.GetIdVisitaPazienteSenzaMalattiaByData

            Return GetIdVisita(codicePaziente, dataVisita, Nothing, Nothing)

        End Function

        ''' <summary>
        ''' Restituisce l'id della visita del paziente in base a data e malattia specificate
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataVisita"></param>
        ''' <param name="codiceMalattia"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetIdVisitaPazienteByDataAndMalattia(codicePaziente As Int64, dataVisita As DateTime, codiceMalattia As String) As Int64? Implements IVisiteProvider.GetIdVisitaPazienteByDataAndMalattia

            Return GetIdVisita(codicePaziente, dataVisita, Nothing, codiceMalattia)

        End Function

        ''' <summary>
        ''' Restituisce l'id della visita del paziente in base a bilancio e malattia specificati
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="numeroBilancio"></param>
        ''' <param name="codiceMalattia"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetIdVisitaPazienteByBilancioAndMalattia(codicePaziente As Int64, numeroBilancio As Int64, codiceMalattia As String) As Int64? Implements IVisiteProvider.GetIdVisitaPazienteByBilancioAndMalattia

            Return GetIdVisita(codicePaziente, Nothing, numeroBilancio, codiceMalattia)

        End Function

        Private Function GetIdVisita(codicePaziente As Int64, dataVisita As DateTime?, numeroBilancio As Int64?, codiceMalattia As String) As Int64?

            Dim idVisita As Int64?

            Using cmd As OracleCommand = Connection.CreateCommand()

                Dim query As New Text.StringBuilder("select vis_id from t_vis_visite where vis_paz_codice = :vis_paz_codice ")

                cmd.Parameters.AddWithValue("vis_paz_codice", codicePaziente)

                If dataVisita.HasValue Then
                    query.Append(" and vis_data_visita = :vis_data_visita ")
                    cmd.Parameters.AddWithValue("vis_data_visita", dataVisita.Value)
                End If

                If numeroBilancio.HasValue Then
                    query.Append(" and vis_n_bilancio = :vis_n_bilancio ")
                    cmd.Parameters.AddWithValue("vis_n_bilancio", numeroBilancio.Value)
                End If

                If String.IsNullOrWhiteSpace(codiceMalattia) Then
                    query.Append(" and vis_mal_codice is null ")
                Else
                    query.Append(" and vis_mal_codice = :vis_mal_codice ")
                    cmd.Parameters.AddWithValue("vis_mal_codice", codiceMalattia)
                End If

                cmd.CommandText = query.ToString()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If obj Is Nothing Then
                        idVisita = Nothing
                    Else
                        idVisita = Convert.ToInt64(obj)
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return idVisita

        End Function

#End Region

#Region " Visite "

        ''' <summary>
        ''' Restituisce la visita eliminata corrispondente al vie_id specificato
        ''' </summary>
        ''' <param name="idVisitaEliminata"></param>
        ''' <returns></returns>
        Public Function GetVisitaEliminataById(idVisitaEliminata As Int64) As Visita Implements IVisiteProvider.GetVisitaEliminataById

            Using cmd As OracleCommand = Connection.CreateCommand()

                cmd.CommandText = Queries.Visite.OracleQueries.selVisitaEliminataById
                cmd.Parameters.AddWithValue("vie_id", idVisitaEliminata)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        Return GetVisiteEliminateListFromReader(idr)(0)

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

        End Function

        ''' <summary>
        ''' Restituisce la visita corrispondente al vis_id specificato
        ''' </summary>
        ''' <param name="idVisita"></param>
        ''' <returns></returns>
        Public Function GetVisitaById(idVisita As Int64) As Visita Implements IVisiteProvider.GetVisitaById

            Return GetVisitaByIdIfExists(idVisita)

        End Function

        Public Function GetPadreFollowup(idFollowUp As Integer) As Integer Implements IVisiteProvider.GetPadreFollowup
            Dim count As Integer = 0

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                cmd.CommandText = OnVac.Queries.Visite.OracleQueries.cntVisitePadreFollowuUp


                cmd.Parameters.AddWithValue("vis_vis_id_follow_up", idFollowUp)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If (Not obj Is Nothing AndAlso Not obj Is DBNull.Value) Then

                        Try
                            count = Convert.ToInt32(obj)
                        Catch ex As Exception
                            count = 0
                        End Try

                    End If

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return count
        End Function
        Public Function GetVisitaByIdIfExists(idVisita As Long) As Visita Implements IVisiteProvider.GetVisitaByIdIfExists

            Using cmd As OracleCommand = Connection.CreateCommand()

                Dim ownConnection As Boolean = False

                Try
                    cmd.CommandText = Queries.Visite.OracleQueries.selVisitaById

                    cmd.Parameters.AddWithValue("vis_id", idVisita)

                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        Return GetVisiteListFromReader(idr).FirstOrDefault()
                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

        End Function
        Public Function GetListViaggiVisitaById(idVisita As Long) As List(Of ViaggioVisita) Implements IVisiteProvider.GetListViaggiVisitaById

            Using cmd As OracleCommand = Connection.CreateCommand()

                Dim ownConnection As Boolean = False

                Try
                    cmd.CommandText = string.Format(Queries.Visite.OracleQueries.selViaggiVisita,idVisita.ToString)

                    'cmd.Parameters.AddWithValue("VVG_VIS_ID", idVisita)

                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        Return GetViaggiVisiteListFromReader(idr)
                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

        End Function

        Public Function GetVisiteById(listIdVisite As List(Of Int64)) As List(Of Visita) Implements IVisiteProvider.GetVisiteById

            Dim listVisite As List(Of Visita) = Nothing

            Using cmd As New OracleCommand()

                Dim filtroId As New Text.StringBuilder()

                For i As Int16 = 0 To listIdVisite.Count - 1

                    Dim paramName As String = String.Format(":id{0}", i.ToString())

                    cmd.Parameters.AddWithValue(paramName, listIdVisite(i))
                    filtroId.AppendFormat("{0},", paramName)

                Next

                If filtroId.Length > 0 Then filtroId.Remove(filtroId.Length - 1, 1)

                cmd.Connection = Connection
                cmd.CommandText = String.Format(Queries.Visite.OracleQueries.selVisiteById, filtroId)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        listVisite = GetVisiteListFromReader(idr)

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listVisite

        End Function

        Public Function GetVisitePaziente(codicePaziente As Integer) As List(Of Visita) Implements IVisiteProvider.GetVisitePaziente

            Dim listVisite As New List(Of Visita)()

            Using cmd As OracleCommand = Connection.CreateCommand()

                Dim query As New Text.StringBuilder(Queries.Visite.OracleQueries.selVisitePaziente)

                cmd.Parameters.AddWithValue("codPaziente", codicePaziente)

                query.Append(" ORDER BY VIS_DATA_VISITA DESC ")

                cmd.CommandText = query.ToString()

                Dim ownConnection As Boolean = ConditionalOpenConnection(cmd)

                Try
                    Using idr As IDataReader = cmd.ExecuteReader()

                        listVisite = GetVisiteListFromReader(idr)

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listVisite

        End Function

        ''' <summary>
        ''' Restituisce le informazioni su firma digitale e archiviazione sostitutiva relative alla visita specificata.
        ''' </summary>
        ''' <param name="idVisita"></param>
        ''' <param name="idApplicazioneFirma">appId relativo all'azienda in cui è stata effettuata la firma digitale (può essere diversa da quella corrente)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetInfoFirmaArchiviazioneVisita(idVisita As Long, idApplicazioneFirma As String) As ArchiviazioneDIRV.InfoFirmaArchiviazioneVisita Implements IVisiteProvider.GetInfoFirmaArchiviazioneVisita

            Dim info As ArchiviazioneDIRV.InfoFirmaArchiviazioneVisita = Nothing

            Using cmd As OracleCommand = Connection.CreateCommand()

                cmd.CommandText =
                    "select vis_id, vis_doc_id_documento, vis_data_firma, vis_ute_id_firma, v_ute_firma.ute_codice ute_codice_firma, " +
                    "vis_data_archiviazione, vis_ute_id_archiviazione, v_ute_archiviazione.ute_codice ute_codice_archiviazione " +
                    "from t_vis_visite " +
                    "  left join v_ana_utenti v_ute_firma on v_ute_firma.ute_id = vis_ute_id_firma and v_ute_firma.ute_app_id = :app_id " +
                    "  left join v_ana_utenti v_ute_archiviazione on v_ute_archiviazione.ute_id = vis_ute_id_archiviazione and v_ute_archiviazione.ute_app_id = :app_id " +
                    "where vis_id = :vis_id "

                cmd.Parameters.AddWithValue("app_id", idApplicazioneFirma)
                cmd.Parameters.AddWithValue("vis_id", idVisita)

                Dim ownConnection As Boolean = ConditionalOpenConnection(cmd)

                Try
                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            If idr.Read() Then

                                Dim vis_id As Integer = idr.GetOrdinal("vis_id")
                                Dim vis_doc_id_documento As Integer = idr.GetOrdinal("vis_doc_id_documento")
                                Dim vis_data_firma As Integer = idr.GetOrdinal("vis_data_firma")
                                Dim ute_codice_firma As Integer = idr.GetOrdinal("ute_codice_firma")
                                Dim vis_data_archiviazione As Integer = idr.GetOrdinal("vis_data_archiviazione")
                                Dim ute_codice_archiviazione As Integer = idr.GetOrdinal("ute_codice_archiviazione")

                                info = New ArchiviazioneDIRV.InfoFirmaArchiviazioneVisita()
                                info.IdVisita = idr.GetInt64(vis_id)
                                info.IdDocumento = idr.GetNullableInt64OrDefault(vis_doc_id_documento)
                                info.DataFirma = idr.GetNullableDateTimeOrDefault(vis_data_firma)
                                info.UtenteFirma = idr.GetStringOrDefault(ute_codice_firma)
                                info.DataArchiviazione = idr.GetNullableDateTimeOrDefault(vis_data_archiviazione)
                                info.UtenteArchiviazione = idr.GetStringOrDefault(ute_codice_archiviazione)

                            End If

                        End If
                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return info

        End Function

        ''' <summary>
        ''' Restituisce la data di fine sospensione massima per il paziente specificato, minvalue altrimenti.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetMaxDataFineSospensione(codicePaziente As Integer) As DateTime Implements IVisiteProvider.GetMaxDataFineSospensione

            Dim dataMax As DateTime = DateTime.MinValue

            Using cmd As OracleCommand = Connection.CreateCommand()

                cmd.CommandText = "select max(vis_fine_sospensione) from t_vis_visite where vis_paz_codice = :vis_paz_codice"

                cmd.Parameters.AddWithValue("vis_paz_codice", codicePaziente)

                Dim ownConnection As Boolean = Me.ConditionalOpenConnection(cmd)

                Try
                    Dim obj As Object = cmd.ExecuteScalar()
                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        dataMax = Convert.ToDateTime(obj)
                    End If
                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return dataMax

        End Function

        ''' <summary>
        ''' Restituisce la data massima di fine sospensione del paziente (successiva alla data di esecuzione specificata).
        ''' Se non presente, restituisce null.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataEsecuzione"></param>
        ''' <returns></returns>
        Public Function GetMaxDataFineSospensione(codicePaziente As Integer, dataEsecuzione As DateTime) As DateTime? Implements IVisiteProvider.GetMaxDataFineSospensione

            Dim dataFineSospensione As DateTime? = Nothing

            Using cmd As OracleCommand = Connection.CreateCommand()

                Dim ownConnection As Boolean = False

                Try
                    cmd.CommandText = "SELECT MAX(VIS_FINE_SOSPENSIONE) VIS_FINE_SOSPENSIONE FROM T_VIS_VISITE WHERE VIS_PAZ_CODICE = :codicePaziente AND VIS_FINE_SOSPENSIONE > :dataEsecuzione"

                    cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)
                    cmd.Parameters.AddWithValue("dataEsecuzione", dataEsecuzione)

                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then

                        dataFineSospensione = DirectCast(obj, DateTime)

                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return dataFineSospensione

        End Function

#Region " Exists "

        Public Function ExistsVisitaById(idVisita As Int64) As Boolean Implements IVisiteProvider.ExistsVisitaById

            Dim existsVisita As Boolean = False

            Using cmd As OracleCommand = Connection.CreateCommand()

                cmd.CommandText = "SELECT 1 FROM T_VIS_VISITE WHERE VIS_ID = :VIS_ID"

                cmd.Parameters.AddWithValue("VIS_ID", idVisita)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    existsVisita = (Not cmd.ExecuteScalar() Is Nothing)

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return existsVisita

        End Function

        Public Function ExistsVisita(codicePaziente As Integer, dataVisita As DateTime) As Boolean Implements IVisiteProvider.ExistsVisita

            Dim idVisitaEsistente As Int64? = GetIdVisitaPazienteByData(codicePaziente, dataVisita)

            Return idVisitaEsistente.HasValue

        End Function

        Public Function ExistsVisita(codicePaziente As Integer, dataVisita As DateTime, codiceMalattia As String) As Boolean Implements IVisiteProvider.ExistsVisita

            With _DAM.QB

                .NewQuery()

                .AddSelectFields("1")
                .AddTables("T_VIS_VISITE")
                .AddWhereCondition("VIS_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("VIS_DATA_VISITA", Comparatori.Uguale, dataVisita, DataTypes.Data)
                If String.IsNullOrEmpty(codiceMalattia) Then
                    .AddWhereCondition("VIS_MAL_CODICE", Comparatori.Uguale, "NULL", DataTypes.Replace)
                Else
                    .AddWhereCondition("VIS_MAL_CODICE", Comparatori.Uguale, codiceMalattia, DataTypes.Stringa)
                End If

            End With

            Dim obj As Object = _DAM.ExecScalar()

            If obj Is Nothing OrElse obj Is DBNull.Value Then Return False

            Return True

        End Function

#End Region

#End Region

#Region " Osservazioni "

        Public Function GetOsservazioniPaziente(codicePaziente As Integer) As List(Of Osservazione) Implements IVisiteProvider.GetOsservazioniPaziente

            Dim osservazioneList As New List(Of Osservazione)()

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("*")
                .AddTables("T_VIS_OSSERVAZIONI")
                .AddWhereCondition("VOS_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
            End With

            Using idr As IDataReader = _DAM.BuildDataReader()
                osservazioneList = GetOsservazioneListFromDataReader(idr, False)
            End Using

            Return osservazioneList

        End Function

        Public Function GetOsservazioniByVisita(idVisita As Long) As Osservazione() Implements IVisiteProvider.GetOsservazioniByVisita

            Dim osservazioneList As New List(Of Osservazione)()

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("*")
                .AddTables("T_VIS_OSSERVAZIONI")
                .AddWhereCondition("VOS_VIS_ID", Comparatori.Uguale, idVisita, DataTypes.Numero)
            End With

            Using idr As IDataReader = _DAM.BuildDataReader()
                osservazioneList = GetOsservazioneListFromDataReader(idr, False)
            End Using

            Return osservazioneList.ToArray()

        End Function

        Public Function GetOsservazioneByIdIfExists(idOsservazione As Long) As Osservazione Implements IVisiteProvider.GetOsservazioneByIdIfExists

            Dim osservazione As Osservazione

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("*")
                .AddTables("T_VIS_OSSERVAZIONI")
                .AddWhereCondition("VOS_ID", Comparatori.Uguale, idOsservazione, DataTypes.Numero)
            End With

            Using idr As IDataReader = _DAM.BuildDataReader()
                osservazione = GetOsservazioneListFromDataReader(idr, False).FirstOrDefault()
            End Using

            Return osservazione

        End Function

        Public Function GetOsservazioneById(idOsservazione As Long) As Osservazione Implements IVisiteProvider.GetOsservazioneById

            Return GetOsservazioneByIdIfExists(idOsservazione)

        End Function

        Public Function GetOsservazioniEliminateByIdVisitaEliminata(idVisitaEliminata As Long) As Osservazione() Implements IVisiteProvider.GetOsservazioniEliminateByIdVisitaEliminata

            Dim osservazioniEliminate As Osservazione()

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("*")
                .AddTables("T_VIS_OSS_ELIMINATE")
                .AddWhereCondition("VOE_VIS_ID", Comparatori.Uguale, idVisitaEliminata, DataTypes.Numero)
            End With

            Using idr As IDataReader = _DAM.BuildDataReader()
                osservazioniEliminate = GetOsservazioneListFromDataReader(idr, True).ToArray()
            End Using

            Return osservazioniEliminate

        End Function

        Function GetOsservazioneEliminataById(idOsservazioneEliminata As Int64) As Osservazione Implements IVisiteProvider.GetOsservazioneEliminataById

            Dim osservazione As Osservazione

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("*")
                .AddTables("T_VIS_OSS_ELIMINATE")
                .AddWhereCondition("VOE_ID", Comparatori.Uguale, idOsservazioneEliminata, DataTypes.Numero)
            End With

            Using idr As IDataReader = _DAM.BuildDataReader()
                osservazione = GetOsservazioneListFromDataReader(idr, True).First()
            End Using

            Return osservazione

        End Function

        ''' <summary>
        ''' Restituisce l'osservazione relativa ai dati specificati.
        ''' Restituisce Nothing se non la trova. 
        ''' </summary>
        Public Function GetOsservazione(codicePaziente As Integer, codiceOsservazione As String, numeroBilancio As Integer, codiceMalattia As String) As Osservazione Implements IVisiteProvider.GetOsservazione

            Dim osservazioni As List(Of Osservazione) = GetOsservazioniEnumerable(codicePaziente, codiceOsservazione, numeroBilancio, codiceMalattia).ToList()

            If osservazioni.Count > 0 Then Return osservazioni(0)

            Return Nothing

        End Function

        ''' <summary>
        ''' Restituisce la lista di osservazioni legate alla visita specificata, comprensive di relative sezioni e risposte
        ''' </summary>
        ''' <param name="idVisita"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetInfoOsservazioniSezioniRisposteByVisita(idVisita As Int64) As List(Of Osservazione) Implements IVisiteProvider.GetInfoOsservazioniSezioniRisposteByVisita

            Dim listOsservazioni As New List(Of Osservazione)()

            Dim ownConnection As Boolean = False

            Using cmd As OracleCommand = Connection.CreateCommand()

                ownConnection = ConditionalOpenConnection(cmd)

                cmd.CommandText = "select vos_id, vos_oss_codice, vos_ris_codice, vos_risposta, osb_n_osservazione, osb_sez_codice, " +
                    "sez_descrizione, sez_n_sezione, oss_descrizione, ris_descrizione " +
                    "from t_vis_osservazioni " +
                    "join t_ana_link_oss_bilanci on osb_oss_codice = vos_oss_codice and osb_bil_n_bilancio = vos_bil_n_bilancio and osb_bil_mal_codice = vos_mal_codice " +
                    "join t_ana_osservazioni on oss_codice = osb_oss_codice " +
                    "join t_ana_sezioni on sez_codice = osb_sez_codice " +
                    "left join t_ana_risposte on ris_codice = vos_ris_codice " +
                    "where vos_vis_id = :vos_vis_id " +
                    "order by sez_n_sezione, osb_n_osservazione "

                cmd.Parameters.AddWithValue("vos_vis_id", idVisita)

                Try
                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim vos_id As Integer = idr.GetOrdinal("vos_id")
                            Dim vos_oss_codice As Integer = idr.GetOrdinal("vos_oss_codice")
                            Dim oss_descrizione As Integer = idr.GetOrdinal("oss_descrizione")
                            Dim osb_n_osservazione As Integer = idr.GetOrdinal("osb_n_osservazione")
                            Dim vos_ris_codice As Integer = idr.GetOrdinal("vos_ris_codice")
                            Dim ris_descrizione As Integer = idr.GetOrdinal("ris_descrizione")
                            Dim vos_risposta As Integer = idr.GetOrdinal("vos_risposta")
                            Dim osb_sez_codice As Integer = idr.GetOrdinal("osb_sez_codice")
                            Dim sez_descrizione As Integer = idr.GetOrdinal("sez_descrizione")
                            Dim sez_n_sezione As Integer = idr.GetOrdinal("sez_n_sezione")

                            While idr.Read()

                                Dim osservazione As New Osservazione()
                                osservazione.Id = idr.GetInt64(vos_id)
                                osservazione.IdVisita = idVisita
                                osservazione.OsservazioneCodice = idr.GetStringOrDefault(vos_oss_codice)
                                osservazione.OsservazioneDescrizione = idr.GetStringOrDefault(oss_descrizione)

                                If idr.IsDBNull(osb_n_osservazione) Then
                                    osservazione.OsservazioneNumero = 0
                                Else
                                    osservazione.OsservazioneNumero = idr.GetInt32(osb_n_osservazione)
                                End If

                                osservazione.RispostaCodice = idr.GetStringOrDefault(vos_ris_codice)
                                osservazione.RispostaDescrizione = idr.GetStringOrDefault(ris_descrizione)
                                osservazione.RispostaTesto = idr.GetStringOrDefault(vos_risposta)
                                osservazione.SezioneCodice = idr.GetStringOrDefault(osb_sez_codice)
                                osservazione.SezioneDescrizione = idr.GetStringOrDefault(sez_descrizione)
                                osservazione.SezioneNumero = idr.GetInt32OrDefault(sez_n_sezione)

                                listOsservazioni.Add(osservazione)

                            End While

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listOsservazioni

        End Function

        ''' <summary>
        ''' Restituisce le osservazioni relative alla visita specificata, con TUTTI i campi valorizzati in base al valore presente su db.
        ''' </summary>
        ''' <param name="idVisita"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetListOsservazioniByVisita(idVisita As Long) As List(Of Osservazione) Implements IVisiteProvider.GetListOsservazioniByVisita

            Dim listOsservazioni As New List(Of Osservazione)()

            Dim ownConnection As Boolean = False

            Using cmd As OracleCommand = Connection.CreateCommand()

                ownConnection = ConditionalOpenConnection(cmd)

                cmd.CommandText = "select vos_id, vos_oss_codice, vos_ris_codice, vos_risposta, osb_n_osservazione, osb_sez_codice, sez_descrizione, sez_n_sezione, " +
                    "oss_descrizione, ris_descrizione, vos_mal_codice, vos_paz_codice, vos_paz_codice_old, vos_data_visita, vos_bil_n_bilancio, " +
                    "vos_data_registrazione, vos_ute_id_registrazione, vos_data_variazione, vos_ute_id_variazione, vos_note_acquisizione " +
                    "from t_vis_osservazioni " +
                    "join t_ana_link_oss_bilanci on osb_oss_codice = vos_oss_codice and osb_bil_n_bilancio = vos_bil_n_bilancio and osb_bil_mal_codice = vos_mal_codice " +
                    "join t_ana_osservazioni on oss_codice = osb_oss_codice " +
                    "join t_ana_sezioni on sez_codice = osb_sez_codice " +
                    "left join t_ana_risposte on ris_codice = vos_ris_codice " +
                    "where vos_vis_id = :vos_vis_id "

                cmd.Parameters.AddWithValue("vos_vis_id", idVisita)

                Try
                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim vos_id As Integer = idr.GetOrdinal("vos_id")
                            Dim vos_oss_codice As Integer = idr.GetOrdinal("vos_oss_codice")
                            Dim oss_descrizione As Integer = idr.GetOrdinal("oss_descrizione")
                            Dim osb_n_osservazione As Integer = idr.GetOrdinal("osb_n_osservazione")
                            Dim vos_ris_codice As Integer = idr.GetOrdinal("vos_ris_codice")
                            Dim ris_descrizione As Integer = idr.GetOrdinal("ris_descrizione")
                            Dim vos_risposta As Integer = idr.GetOrdinal("vos_risposta")
                            Dim osb_sez_codice As Integer = idr.GetOrdinal("osb_sez_codice")
                            Dim sez_descrizione As Integer = idr.GetOrdinal("sez_descrizione")
                            Dim sez_n_sezione As Integer = idr.GetOrdinal("sez_n_sezione")
                            Dim vos_mal_codice As Integer = idr.GetOrdinal("vos_mal_codice")
                            Dim vos_paz_codice As Integer = idr.GetOrdinal("vos_paz_codice")
                            Dim vos_paz_codice_old As Integer = idr.GetOrdinal("vos_paz_codice_old")
                            Dim vos_data_visita As Integer = idr.GetOrdinal("vos_data_visita")
                            Dim vos_bil_n_bilancio As Integer = idr.GetOrdinal("vos_bil_n_bilancio")
                            Dim vos_data_registrazione As Integer = idr.GetOrdinal("vos_data_registrazione")
                            Dim vos_ute_id_registrazione As Integer = idr.GetOrdinal("vos_ute_id_registrazione")
                            Dim vos_data_variazione As Integer = idr.GetOrdinal("vos_data_variazione")
                            Dim vos_ute_id_variazione As Integer = idr.GetOrdinal("vos_ute_id_variazione")
                            Dim vos_note_acquisizione As Integer = idr.GetOrdinal("vos_note_acquisizione")

                            While idr.Read()

                                Dim osservazione As New Osservazione()

                                osservazione.Id = idr.GetInt64(vos_id)
                                osservazione.IdVisita = idVisita
                                osservazione.OsservazioneCodice = idr.GetStringOrDefault(vos_oss_codice)
                                osservazione.OsservazioneDescrizione = idr.GetStringOrDefault(oss_descrizione)

                                If idr.IsDBNull(osb_n_osservazione) Then
                                    osservazione.OsservazioneNumero = 0
                                Else
                                    osservazione.OsservazioneNumero = idr.GetInt32(osb_n_osservazione)
                                End If

                                osservazione.RispostaCodice = idr.GetStringOrDefault(vos_ris_codice)
                                If String.IsNullOrEmpty(osservazione.RispostaCodice) OrElse Not osservazione.RispostaCodice.Contains("|") Then
                                    osservazione.RispostaDescrizione = idr.GetStringOrDefault(ris_descrizione)
                                Else
                                    osservazione.RispostaDescrizione = ""
                                End If
                                osservazione.RispostaTesto = idr.GetStringOrDefault(vos_risposta)
                                osservazione.SezioneCodice = idr.GetStringOrDefault(osb_sez_codice)
                                osservazione.SezioneDescrizione = idr.GetStringOrDefault(sez_descrizione)
                                osservazione.SezioneNumero = idr.GetInt32OrDefault(sez_n_sezione)
                                osservazione.CodiceMalattia = idr.GetStringOrDefault(vos_mal_codice)
                                osservazione.CodicePaziente = idr.GetInt32OrDefault(vos_paz_codice)
                                osservazione.CodicePazienteOld = idr.GetInt32OrDefault(vos_paz_codice_old)
                                osservazione.DataVisita = idr.GetNullableDateTimeOrDefault(vos_data_visita)
                                osservazione.NumeroBilancio = idr.GetInt32(vos_bil_n_bilancio)
                                osservazione.DataRegistrazione = idr.GetNullableDateTimeOrDefault(vos_data_registrazione)
                                osservazione.IdUtenteRegistrazione = idr.GetNullableInt64OrDefault(vos_ute_id_registrazione)
                                osservazione.DataVariazione = idr.GetNullableDateTimeOrDefault(vos_data_variazione)
                                osservazione.IdUtenteVariazione = idr.GetNullableInt64OrDefault(vos_ute_id_variazione)
                                osservazione.NoteAcquisizioneDatiVaccinaliCentrale = idr.GetStringOrDefault(vos_note_acquisizione)

                                ' ?
                                'osservazione.OsservazioneDisabilitata

                                ' Mancanti:
                                'osservazione.DataEliminazione
                                'osservazione.IdUtenteEliminazione

                                listOsservazioni.Add(osservazione)

                            End While

                        End If

                    End Using

                    'Ciclo per sistemare descrizione dei codici multipli
                    For Each lista As Osservazione In listOsservazioni
                        If lista.RispostaCodice.Contains("|") Then
                            lista.RispostaDescrizione = DescrizionRisposteMultiplaPrivate(lista.RispostaCodice, Connection)
                        End If
                    Next

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listOsservazioni

        End Function

        Private Function DescrizionRisposteMultiplaPrivate(codiceRisposta As String, connection As IDbConnection) As String

            Dim descrizioneRisposte As String = ""
            Dim errore As String = ""

            If Not String.IsNullOrEmpty(codiceRisposta) Then

                Using cmds As OracleCommand = connection.CreateCommand()

                    Dim s As String = codiceRisposta.Replace("|", "' , '")

                    cmds.CommandText = String.Format("select ris_codice,ris_descrizione from t_ana_risposte where ris_codice in ('{0}')", s)

                    Using idrs As IDataReader = cmds.ExecuteReader()

                        If Not idrs Is Nothing Then

                            Dim ris_cod As Integer = idrs.GetOrdinal("ris_codice")
                            Dim ris_desc As Integer = idrs.GetOrdinal("ris_descrizione")

                            While idrs.Read()
                                descrizioneRisposte = descrizioneRisposte + idrs.GetStringOrDefault(ris_desc) + ", "
                            End While

                        End If

                    End Using

                End Using

            End If

            If Not descrizioneRisposte.IsNullOrEmpty Then
                descrizioneRisposte = descrizioneRisposte.Substring(0, descrizioneRisposte.Length - 2)
            Else
                descrizioneRisposte = ""
            End If

            Return descrizioneRisposte

        End Function

#End Region

#End Region

#Region " Delete "

        Public Function EliminaOsservazione(idOsservazione As Int64) As Boolean Implements IVisiteProvider.EliminaOsservazione

            With _DAM.QB
                .NewQuery()
                .AddTables("T_VIS_OSSERVAZIONI")
                .AddWhereCondition("VOS_ID", Comparatori.Uguale, idOsservazione, DataTypes.Numero)
            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Delete) > 0

        End Function

        Public Function EliminaOsservazioni(idVisita As Integer) As Integer Implements IVisiteProvider.EliminaOsservazioni

            With _DAM.QB
                .NewQuery()
                .AddTables("T_VIS_OSSERVAZIONI")
                .AddWhereCondition("VOS_VIS_ID", Comparatori.Uguale, idVisita, DataTypes.Numero)
            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Delete)

        End Function

        Public Function EliminaVisita(idVisita As Integer) As Boolean Implements IVisiteProvider.EliminaVisita

            With _DAM.QB
                .NewQuery()
                .AddTables("T_VIS_VISITE")
                .AddWhereCondition("VIS_ID", Comparatori.Uguale, idVisita, DataTypes.Numero)
            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Delete) > 0

        End Function
        Public Function EliminaViaggio(idViaggio As Integer) As Boolean Implements IVisiteProvider.EliminaViaggio

            With _DAM.QB
                .NewQuery()
                .AddTables("T_VIS_VIAGGI")
                .AddWhereCondition("VVG_ID", Comparatori.Uguale, idViaggio, DataTypes.Numero)
            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Delete) > 0

        End Function

#End Region

#Region " Insert "

        ''' <summary>
        ''' Inserimento visita. I dati non centralizzati non vengono presi in considerazione.
        ''' </summary>
        ''' <param name="visita"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsertVisita(visita As Visita) As Boolean Implements IVisiteProvider.InsertVisita

            Return InsertVisita(visita, False)

        End Function

        ''' <summary>
        ''' Inserimento della visita tra le eliminate. Non vengono copiati i dati da non centralizzare.
        ''' </summary>
        ''' <param name="visitaEliminata"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsertVisitaEliminata(visitaEliminata As Visita) As Boolean Implements IVisiteProvider.InsertVisitaEliminata

            Return InsertVisita(visitaEliminata, True)

        End Function

        Private Function InsertVisita(visita As Visita, insertInEliminate As Boolean) As Boolean

            Dim prefix As String = String.Empty

            If visita.IdVisita <= 0 Then
                visita.IdVisita = GetNextIdVisita()
            End If

            With _DAM.QB

                .NewQuery()

                If insertInEliminate Then
                    .AddTables("t_vis_visite_eliminate")
                    prefix = "vie_"
                Else
                    .AddTables("t_vis_visite")
                    prefix = "vis_"
                End If

                If visita.IdVisita > 0 Then
                    .AddInsertField(String.Format("{0}id", prefix), visita.IdVisita, DataTypes.Numero)
                End If

                .AddInsertField(String.Format("{0}paz_codice", prefix), visita.CodicePaziente, DataTypes.Numero)
                .AddInsertField(String.Format("{0}paz_codice_old", prefix), GetLongParam(visita.CodicePazienteAlias), DataTypes.Numero)
                .AddInsertField(String.Format("{0}data_visita", prefix), visita.DataVisita, DataTypes.Data)
                .AddInsertField(String.Format("{0}ope_codice", prefix), GetStringParam(visita.MedicoCodice), DataTypes.Stringa)
                .AddInsertField(String.Format("{0}cns_codice", prefix), GetStringParam(visita.CodiceConsultorio), DataTypes.Stringa)
                .AddInsertField(String.Format("{0}mal_codice", prefix), GetStringParam(visita.MalattiaCodice), DataTypes.Stringa)
                .AddInsertField(String.Format("{0}n_bilancio", prefix), GetLongParam(visita.BilancioNumero), DataTypes.Numero)
                .AddInsertField(String.Format("{0}ute_id", prefix), visita.IdUtente, DataTypes.Numero)
                .AddInsertField(String.Format("{0}data_registrazione", prefix), visita.DataRegistrazione, DataTypes.DataOra)

                If Not String.IsNullOrEmpty(visita.Vaccinabile) Then .AddInsertField(String.Format("{0}vaccinabile", prefix), visita.Vaccinabile, DataTypes.Stringa)

                .AddInsertField(String.Format("{0}mos_codice", prefix), GetStringParam(visita.MotivoSospensioneCodice), DataTypes.Stringa)
                If visita.DataFineSospensione > DateTime.MinValue Then .AddInsertField(String.Format("{0}fine_sospensione", prefix), visita.DataFineSospensione, DataTypes.Data)

                If visita.Peso.HasValue AndAlso visita.Peso > 0 Then .AddInsertField(String.Format("{0}peso", prefix), visita.Peso, DataTypes.Float)
                If visita.Altezza.HasValue AndAlso visita.Altezza > 0 Then .AddInsertField(String.Format("{0}altezza", prefix), visita.Altezza, DataTypes.Float)
                If visita.Cranio.HasValue AndAlso visita.Cranio > 0 Then .AddInsertField(String.Format("{0}cranio", prefix), visita.Cranio, DataTypes.Float)

                If Not String.IsNullOrEmpty(visita.PercentilePeso) Then .AddInsertField(String.Format("{0}percentile_peso", prefix), visita.PercentilePeso, DataTypes.Stringa)
                If Not String.IsNullOrEmpty(visita.PercentileAltezza) Then .AddInsertField(String.Format("{0}percentile_altezza", prefix), visita.PercentileAltezza, DataTypes.Stringa)
                If Not String.IsNullOrEmpty(visita.PercentileCranio) Then .AddInsertField(String.Format("{0}percentile_cranio", prefix), visita.PercentileCranio, DataTypes.Stringa)

                If Not String.IsNullOrEmpty(visita.Firma) Then .AddInsertField(String.Format("{0}firma", prefix), visita.Firma, DataTypes.Stringa)
                If Not String.IsNullOrEmpty(visita.FlagPatologia) Then .AddInsertField(String.Format("{0}patologia", prefix), visita.FlagPatologia, DataTypes.Stringa)
                If Not String.IsNullOrEmpty(visita.Note) Then .AddInsertField(String.Format("{0}note", prefix), visita.Note, DataTypes.Stringa)

                If Not String.IsNullOrEmpty(visita.CodiceUslInserimento) Then .AddInsertField(String.Format("{0}usl_inserimento", prefix), visita.CodiceUslInserimento, DataTypes.Stringa)
                If Not String.IsNullOrEmpty(visita.FlagVisibilitaDatiVaccinaliCentrale) Then .AddInsertField(String.Format("{0}flag_visibilita", prefix), visita.FlagVisibilitaDatiVaccinaliCentrale, DataTypes.Stringa)
                If Not String.IsNullOrEmpty(visita.NoteAcquisizioneDatiVaccinaliCentrale) Then .AddInsertField(String.Format("{0}note_acquisizione", prefix), visita.NoteAcquisizioneDatiVaccinaliCentrale, DataTypes.Stringa)

                .AddInsertField(String.Format("{0}data_variazione", prefix), GetDateParam(visita.DataModifica), DataTypes.DataOra)
                .AddInsertField(String.Format("{0}ute_id_variazione", prefix), GetLongParam(visita.IdUtenteModifica), DataTypes.Numero)

                .AddInsertField(String.Format("{0}doc_id_documento", prefix), GetLongParam(visita.IdDocumento), DataTypes.Numero)
                .AddInsertField(String.Format("{0}data_firma", prefix), GetDateParam(visita.DataFirmaDigitale), DataTypes.DataOra)
                .AddInsertField(String.Format("{0}ute_id_firma", prefix), GetLongParam(visita.IdUtenteFirmaDigitale), DataTypes.Numero)
                .AddInsertField(String.Format("{0}data_archiviazione", prefix), GetDateParam(visita.DataArchiviazione), DataTypes.DataOra)
                .AddInsertField(String.Format("{0}ute_id_archiviazione", prefix), GetLongParam(visita.IdUtenteArchiviazione), DataTypes.Numero)

                .AddInsertField(String.Format("{0}data_inizio_viaggio", prefix), GetDateParam(visita.DataInizioViaggio), DataTypes.Data)
                .AddInsertField(String.Format("{0}data_fine_viaggio", prefix), GetDateParam(visita.DataFineViaggio), DataTypes.Data)
                .AddInsertField(String.Format("{0}cit_codice_paese_viaggio", prefix), GetStringParam(visita.PaeseViaggioCodice), DataTypes.Stringa)

                .AddInsertField(String.Format("{0}ope_codice_rilevatore", prefix), GetStringParam(visita.RilevatoreCodice), DataTypes.Stringa)
                .AddInsertField(String.Format("{0}vaccinazioni_bilancio", prefix), GetStringParam(visita.VaccinazioniBilancio), DataTypes.Stringa)

                If visita.FollowUpId.HasValue Then
                    .AddInsertField(String.Format("{0}vis_id_follow_up", prefix), visita.FollowUpId, DataTypes.Numero)
                End If
                If visita.DataFollowUpPrevisto.HasValue Then
                    .AddInsertField(String.Format("{0}data_follow_up_previsto", prefix), GetDateParam(visita.DataFollowUpPrevisto), DataTypes.Data)
                End If
                If visita.DataFollowUpEffettivo.HasValue Then
                    .AddInsertField(String.Format("{0}data_follow_up_effettivo", prefix), GetDateParam(visita.DataFollowUpEffettivo), DataTypes.Data)
                End If


                If insertInEliminate Then

                    If visita.DataEliminazione.HasValue AndAlso visita.DataEliminazione > DateTime.MinValue Then
                        .AddInsertField(String.Format("{0}data_eliminazione", prefix), visita.DataEliminazione, DataTypes.DataOra)
                    End If

                    If visita.IdUtenteEliminazione.HasValue AndAlso visita.IdUtenteEliminazione > 0 Then
                        .AddInsertField(String.Format("{0}ute_id_eliminazione", prefix), visita.IdUtenteEliminazione, DataTypes.Numero)
                    End If

                End If

            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Insert) > 0

        End Function

        Public Function InsertOsservazione(osservazione As Osservazione) As Boolean Implements IVisiteProvider.InsertOsservazione

            Return InsertOsservazione(osservazione, False)

        End Function

        Public Function InsertOsservazioneEliminata(osservazione As Osservazione) As Boolean Implements IVisiteProvider.InsertOsservazioneEliminata

            Return InsertOsservazione(osservazione, True)

        End Function

        Private Function InsertOsservazione(osservazione As Osservazione, eliminata As Boolean) As Boolean

            If osservazione.Id <= 0 Then
                osservazione.Id = GetNextIdOsservazione()
            End If

            Dim table As String = IIf(eliminata, "T_VIS_OSS_ELIMINATE", "T_VIS_OSSERVAZIONI")
            Dim fieldPrefix As String = IIf(eliminata, "VOE", "VOS")

            With _DAM.QB

                .NewQuery()

                .AddTables(table)

                .AddInsertField(fieldPrefix + "_ID", osservazione.Id, DataTypes.Numero)
                .AddInsertField(fieldPrefix + "_VIS_ID", osservazione.IdVisita, DataTypes.Numero)
                .AddInsertField(fieldPrefix + "_PAZ_CODICE", osservazione.CodicePaziente, DataTypes.Numero)
                .AddInsertField(fieldPrefix + "_BIL_N_BILANCIO", osservazione.NumeroBilancio, DataTypes.Numero)
                .AddInsertField(fieldPrefix + "_OSS_CODICE", osservazione.OsservazioneCodice, DataTypes.Stringa)
                .AddInsertField(fieldPrefix + "_MAL_CODICE", osservazione.CodiceMalattia, DataTypes.Stringa)

                If osservazione.DataVisita.HasValue AndAlso osservazione.DataVisita > DateTime.MinValue Then
                    .AddInsertField(fieldPrefix + "_DATA_VISITA", osservazione.DataVisita, DataTypes.Data)
                End If

                .AddInsertField(fieldPrefix + "_RISPOSTA", osservazione.RispostaTesto, DataTypes.Stringa)

                If osservazione.CodicePazienteOld.HasValue AndAlso osservazione.CodicePazienteOld.Value > 0 Then
                    .AddInsertField(fieldPrefix + "_PAZ_CODICE_OLD", osservazione.CodicePazienteOld.Value, DataTypes.Numero)
                End If

                If Not String.IsNullOrEmpty(osservazione.RispostaCodice) Then
                    .AddInsertField(fieldPrefix + "_RIS_CODICE", osservazione.RispostaCodice, DataTypes.Stringa)
                End If

                .AddInsertField(fieldPrefix + "_DATA_REGISTRAZIONE", osservazione.DataRegistrazione, DataTypes.DataOra)
                .AddInsertField(fieldPrefix + "_UTE_ID_REGISTRAZIONE", osservazione.IdUtenteRegistrazione, DataTypes.Numero)

                If osservazione.DataVariazione.HasValue AndAlso osservazione.DataVariazione > DateTime.MinValue Then
                    .AddInsertField(fieldPrefix + "_DATA_VARIAZIONE", osservazione.DataVariazione, DataTypes.DataOra)
                End If

                If osservazione.IdUtenteVariazione.HasValue AndAlso osservazione.IdUtenteVariazione > 0 Then
                    .AddInsertField(fieldPrefix + "_UTE_ID_VARIAZIONE", osservazione.IdUtenteVariazione, DataTypes.Numero)
                End If

                If Not String.IsNullOrEmpty(osservazione.NoteAcquisizioneDatiVaccinaliCentrale) Then
                    .AddInsertField(fieldPrefix + "_NOTE_ACQUISIZIONE", osservazione.NoteAcquisizioneDatiVaccinaliCentrale, DataTypes.Stringa)
                End If

                If osservazione.DataEliminazione.HasValue AndAlso osservazione.DataEliminazione > DateTime.MinValue Then
                    .AddInsertField(fieldPrefix + "_DATA_ELIMINAZIONE", osservazione.DataEliminazione, DataTypes.DataOra)
                End If

                If osservazione.IdUtenteEliminazione.HasValue AndAlso osservazione.IdUtenteEliminazione > 0 Then
                    .AddInsertField(fieldPrefix + "_UTE_ID_ELIMINAZIONE", osservazione.IdUtenteEliminazione, DataTypes.Numero)
                End If

            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Insert) > 0

        End Function
        ''' <summary>
        ''' Insert di un viaggio di una visita
        ''' </summary>
        ''' <param name="viaggio"></param>
        ''' <returns></returns>
        Public Function InsertViaggio(viaggio As ViaggioVisita) As Boolean

            If viaggio.Id <= 0 Then
                viaggio.Id = GetNextIdViaggio()
            End If

            With _DAM.QB

                .NewQuery()

                .AddTables("T_VIS_VIAGGI")

                .AddInsertField("VVG_ID", viaggio.Id, DataTypes.Numero)
                .AddInsertField("VVG_VIS_ID", viaggio.IdVisita, DataTypes.Numero)
                .AddInsertField("VVG_CIT_CODICE_PAESE_VIAGGIO", viaggio.CodicePaese, DataTypes.Stringa)
                .AddInsertField("VVG_DATA_INIZIO", viaggio.DataInizioViaggio, DataTypes.Data)
                .AddInsertField("VVG_DATA_FINE", viaggio.DataFineViaggio, DataTypes.Data)

            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Insert) > 0

        End Function

#End Region

#Region " Update "

        Public Function UpdateOsservazione(osservazione As Osservazione) As Boolean Implements IVisiteProvider.UpdateOsservazione

            With _DAM.QB

                .NewQuery()
                .AddTables("T_VIS_OSSERVAZIONI")
                .AddUpdateField("VOS_VIS_ID", osservazione.IdVisita, DataTypes.Numero)
                .AddUpdateField("VOS_PAZ_CODICE", osservazione.CodicePaziente, DataTypes.Numero)
                .AddUpdateField("VOS_BIL_N_BILANCIO", osservazione.NumeroBilancio, DataTypes.Numero)
                .AddUpdateField("VOS_MAL_CODICE", GetStringParam(osservazione.CodiceMalattia), DataTypes.Stringa)
                .AddUpdateField("VOS_OSS_CODICE", osservazione.OsservazioneCodice, DataTypes.Stringa)
                .AddUpdateField("VOS_RIS_CODICE", GetStringParam(osservazione.RispostaCodice), DataTypes.Stringa)
                .AddUpdateField("VOS_DATA_VISITA", GetDateParam(osservazione.DataVisita), DataTypes.Data)
                .AddUpdateField("VOS_RISPOSTA", GetStringParam(osservazione.RispostaTesto), DataTypes.Stringa)
                If osservazione.CodicePazienteOld.HasValue AndAlso osservazione.CodicePazienteOld.Value > 0 Then
                    .AddUpdateField("VOS_PAZ_CODICE_OLD", GetIntParam(osservazione.CodicePazienteOld), DataTypes.Numero)
                Else
                    .AddUpdateField("VOS_PAZ_CODICE_OLD", DBNull.Value, DataTypes.Numero)
                End If
                .AddUpdateField("VOS_DATA_VARIAZIONE", GetDateParam(osservazione.DataVariazione), DataTypes.DataOra)
                .AddUpdateField("VOS_UTE_ID_VARIAZIONE", GetLongParam(osservazione.IdUtenteVariazione), DataTypes.Numero)
                .AddUpdateField("VOS_NOTE_ACQUISIZIONE", GetStringParam(osservazione.NoteAcquisizioneDatiVaccinaliCentrale), DataTypes.Stringa)
                .AddUpdateField("VOS_DATA_REGISTRAZIONE", osservazione.DataRegistrazione, DataTypes.DataOra)
                .AddUpdateField("VOS_UTE_ID_REGISTRAZIONE", osservazione.IdUtenteRegistrazione, DataTypes.Numero)

                .AddWhereCondition("VOS_ID", Comparatori.Uguale, osservazione.Id, DataTypes.Numero)

            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Update) > 0

        End Function
        ''' <summary>
        ''' Update del viaggio di una visita
        ''' </summary>
        ''' <param name="viaggio"></param>
        ''' <returns></returns>
        Public Function UpdateViaggio(viaggio As ViaggioVisita) As Boolean


            With _DAM.QB

                .NewQuery()

                .AddTables("T_VIS_VIAGGI")


                .AddUpdateField("VVG_VIS_ID", viaggio.IdVisita, DataTypes.Numero)
                .AddUpdateField("VVG_CIT_CODICE_PAESE_VIAGGIO", viaggio.CodicePaese, DataTypes.Stringa)
                .AddUpdateField("VVG_DATA_INIZIO", viaggio.DataInizioViaggio, DataTypes.Data)
                .AddUpdateField("VVG_DATA_FINE", viaggio.DataFineViaggio, DataTypes.Data)
                .AddWhereCondition("VVG_ID", Comparatori.Uguale, viaggio.Id, DataTypes.Numero)
            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Update) > 0

        End Function


        ''' <summary>
        ''' Update dati della visita. I dati relativi a firma/archiviazione non vengono modificati se non sono valorizzati.
        ''' </summary>
        ''' <param name="visita"></param>
        ''' <returns></returns>
        Public Function UpdateVisita(visita As Visita, fromUpdateBilanci As Boolean) As Boolean Implements IVisiteProvider.UpdateVisita

            With _DAM.QB

                .NewQuery()

                .AddTables("T_VIS_VISITE")

                '28/09/18: NON eseguo l'update del vis_paz_codice quando proviene dall'update delle osservazioni (perchè in teoria la visita dovrebbe già averlo)
                'If Not fromUpdateBilanci Then .AddUpdateField("vis_paz_codice", visita.CodicePaziente, DataTypes.Numero)

                If visita.CodicePazienteAlias.HasValue AndAlso visita.CodicePazienteAlias.Value > 0 Then
                    .AddUpdateField("vis_paz_codice_old", GetLongParam(visita.CodicePazienteAlias), DataTypes.Numero)
                Else
                    .AddUpdateField("vis_paz_codice_old", DBNull.Value, DataTypes.Numero)
                End If
                .AddUpdateField("vis_cns_codice", GetStringParam(visita.CodiceConsultorio), DataTypes.Stringa)
                .AddUpdateField("vis_mal_codice", GetStringParam(visita.MalattiaCodice), DataTypes.Stringa)
                .AddUpdateField("vis_n_bilancio", GetLongParam(visita.BilancioNumero), DataTypes.Numero)
                .AddUpdateField("vis_ute_id", visita.IdUtente, DataTypes.Numero)
                .AddUpdateField("vis_data_registrazione", visita.DataRegistrazione, DataTypes.DataOra)

                .AddUpdateField("VIS_DATA_VISITA", visita.DataVisita, DataTypes.Data)
                .AddUpdateField("VIS_OPE_CODICE", visita.MedicoCodice, DataTypes.Stringa)
                .AddUpdateField("VIS_FIRMA", visita.Firma, DataTypes.Stringa)

                If Not String.IsNullOrEmpty(visita.Vaccinabile) Then
                    .AddUpdateField("vis_vaccinabile", visita.Vaccinabile, DataTypes.Stringa)
                Else
                    .AddUpdateField("vis_vaccinabile", DBNull.Value, DataTypes.Stringa)
                End If

                .AddUpdateField("vis_fine_sospensione", GetDateParam(visita.DataFineSospensione), DataTypes.Data)
                .AddUpdateField("vis_mos_codice", GetStringParam(visita.MotivoSospensioneCodice), DataTypes.Stringa)

                If visita.Peso.HasValue Then
                    .AddUpdateField("vis_peso", IIf(visita.Peso > 0, visita.Peso, DBNull.Value), DataTypes.Float)
                Else
                    .AddUpdateField("vis_peso", DBNull.Value, DataTypes.Float)
                End If

                If visita.Altezza.HasValue Then
                    .AddUpdateField("vis_altezza", IIf(visita.Altezza > 0, visita.Altezza, DBNull.Value), DataTypes.Float)
                Else
                    .AddUpdateField("vis_altezza", DBNull.Value, DataTypes.Float)
                End If

                If visita.Cranio.HasValue Then
                    .AddUpdateField("vis_cranio", IIf(visita.Cranio > 0, visita.Cranio, DBNull.Value), DataTypes.Float)
                Else
                    .AddUpdateField("vis_cranio", DBNull.Value, DataTypes.Float)
                End If

                .AddUpdateField("vis_percentile_peso", GetStringParam(visita.PercentilePeso), DataTypes.Stringa)
                .AddUpdateField("vis_percentile_altezza", GetStringParam(visita.PercentileAltezza), DataTypes.Stringa)
                .AddUpdateField("vis_percentile_cranio", GetStringParam(visita.PercentileCranio), DataTypes.Stringa)
                .AddUpdateField("vis_patologia", GetStringParam(visita.FlagPatologia), DataTypes.Stringa)
                .AddUpdateField("vis_note", GetStringParam(visita.Note), DataTypes.Stringa)

                .AddUpdateField("vis_usl_inserimento", GetStringParam(visita.CodiceUslInserimento), DataTypes.Stringa)
                .AddUpdateField("vis_flag_visibilita", visita.FlagVisibilitaDatiVaccinaliCentrale, DataTypes.Stringa)
                .AddUpdateField("vis_note_acquisizione", GetStringParam(visita.NoteAcquisizioneDatiVaccinaliCentrale), DataTypes.Stringa)

                .AddUpdateField("vis_data_variazione", GetDateParam(visita.DataModifica), DataTypes.DataOra)
                .AddUpdateField("vis_ute_id_variazione", GetLongParam(visita.IdUtenteModifica), DataTypes.Numero)

                If visita.IdDocumento.HasValue Then
                    .AddUpdateField("vis_doc_id_documento", visita.IdDocumento.Value, DataTypes.Numero)
                End If

                If visita.DataFirmaDigitale.HasValue Then
                    .AddUpdateField("vis_data_firma", visita.DataFirmaDigitale.Value, DataTypes.DataOra)
                End If

                If visita.IdUtenteFirmaDigitale.HasValue Then
                    .AddUpdateField("vis_ute_id_firma", visita.IdUtenteFirmaDigitale.Value, DataTypes.Numero)
                End If

                If visita.DataArchiviazione.HasValue Then
                    .AddUpdateField("vis_data_archiviazione", visita.DataArchiviazione.Value, DataTypes.DataOra)
                End If

                If visita.IdUtenteArchiviazione.HasValue Then
                    .AddUpdateField("vis_ute_id_archiviazione", visita.IdUtenteArchiviazione.Value, DataTypes.Numero)
                End If

                If visita.DataInizioViaggio.HasValue Then
                    .AddUpdateField("vis_data_inizio_viaggio", visita.DataInizioViaggio.Value, DataTypes.Data)
                Else
                    .AddUpdateField("vis_data_inizio_viaggio", DBNull.Value, DataTypes.Data)
                End If

                If visita.DataFineViaggio.HasValue Then
                    .AddUpdateField("vis_data_fine_viaggio", visita.DataFineViaggio.Value, DataTypes.Data)
                Else
                    .AddUpdateField("vis_data_fine_viaggio", DBNull.Value, DataTypes.Data)
                End If

                If String.IsNullOrWhiteSpace(visita.PaeseViaggioCodice) Then
                    .AddUpdateField("vis_cit_codice_paese_viaggio", DBNull.Value, DataTypes.Stringa)
                Else
                    .AddUpdateField("vis_cit_codice_paese_viaggio", visita.PaeseViaggioCodice, DataTypes.Stringa)
                End If

                If String.IsNullOrWhiteSpace(visita.RilevatoreCodice) Then
                    .AddUpdateField("vis_ope_codice_rilevatore", DBNull.Value, DataTypes.Stringa)
                Else
                    .AddUpdateField("vis_ope_codice_rilevatore", visita.RilevatoreCodice, DataTypes.Stringa)
                End If

                If String.IsNullOrWhiteSpace(visita.VaccinazioniBilancio) Then
                    .AddUpdateField("vis_vaccinazioni_bilancio", DBNull.Value, DataTypes.Stringa)
                Else
                    .AddUpdateField("vis_vaccinazioni_bilancio", visita.VaccinazioniBilancio, DataTypes.Stringa)
                End If
                If visita.FollowUpId.HasValue Then
                    .AddUpdateField("vis_vis_id_follow_up", visita.FollowUpId, DataTypes.Numero)
                End If
                If visita.DataFollowUpEffettivo.HasValue Then
                    .AddUpdateField("vis_data_follow_up_effettivo", visita.DataFollowUpEffettivo.Value, DataTypes.Data)
                End If
                If visita.DataFollowUpPrevisto.HasValue Then
                    .AddUpdateField("vis_data_follow_up_previsto", visita.DataFollowUpPrevisto.Value, DataTypes.Data)
                End If


                .AddWhereCondition("vis_id", Comparatori.Uguale, visita.IdVisita, DataTypes.Numero)

            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Update) > 0

        End Function

        ''' <summary>
        ''' Update flag di visibilità. Vengono modificati anche utente e data di variazione.
        ''' </summary>
        ''' <param name="idVisita"></param>
        ''' <param name="flagVisibilita"></param>
        ''' <param name="idUtenteVariazione"></param>
        ''' <param name="dataVariazione"></param>
        ''' <returns></returns>
        Public Function UpdateFlagVisibilita(idVisita As Long, flagVisibilita As String, idUtenteVariazione As Long, dataVariazione As Date) As Integer Implements IVisiteProvider.UpdateFlagVisibilita

            Dim count As Integer = 0

            Using cmd As OracleCommand = Connection.CreateCommand()

                cmd.CommandText =
                    " update t_vis_visite set " +
                    " vis_flag_visibilita = :vis_flag_visibilita, " +
                    " vis_ute_id_variazione = :vis_ute_id_variazione, " +
                    " vis_data_variazione = :vis_data_variazione " +
                    " where vis_id = :vis_id "

                cmd.Parameters.AddWithValue("vis_flag_visibilita", flagVisibilita)
                cmd.Parameters.AddWithValue("vis_ute_id_variazione", idUtenteVariazione)
                cmd.Parameters.AddWithValue("vis_data_variazione", dataVariazione)
                cmd.Parameters.AddWithValue("vis_id", idVisita)

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

        Public Function UpdateIdFollowUp(idVisita As Long, idFollowUp As Long, dataFollowUpEffettiva As Date?) As Integer Implements IVisiteProvider.UpdateIdFollowUp

            Dim count As Integer = 0

            Using cmd As OracleCommand = Connection.CreateCommand()

                cmd.CommandText =
                    " update t_vis_visite set " +
                    " vis_vis_id_follow_up = :vis_vis_id_follow_up, " +
                    " vis_data_follow_up_effettivo = :vis_data_follow_up_effettivo " +
                    " where vis_id = :vis_id "

                cmd.Parameters.AddWithValue("vis_vis_id_follow_up", idFollowUp)
                If dataFollowUpEffettiva.HasValue Then
                    cmd.Parameters.AddWithValue("vis_data_follow_up_effettivo", dataFollowUpEffettiva)
                Else
                    cmd.Parameters.AddWithValue("vis_data_follow_up_effettivo", DBNull.Value)
                End If

                cmd.Parameters.AddWithValue("vis_id", idVisita)

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

        Public Function UpdateCancellaLegameFollowUp(idFollowUp As Long) As Integer Implements IVisiteProvider.UpdateCancellaLegameFollowUp

            Dim count As Integer = 0

            Using cmd As OracleCommand = Connection.CreateCommand()

                cmd.CommandText =
                    " update t_vis_visite set " +
                    " vis_vis_id_follow_up = :vis_vis_id_follow_up, " +
                    " vis_data_follow_up_effettivo = :vis_data_follow_up_effettivo " +
                    " where vis_vis_id_follow_up = :idFollowUp "

                cmd.Parameters.AddWithValue("vis_vis_id_follow_up", DBNull.Value)
                cmd.Parameters.AddWithValue("vis_data_follow_up_effettivo", DBNull.Value)
                cmd.Parameters.AddWithValue("idFollowUp", idFollowUp)

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

#End Region

#End Region

#Region " Firma Digitale - Archiviazione Sostitutiva "

        ''' <summary>
        ''' Aggiorna i campi della visita relativi alla firma digitale con i valori specificati
        ''' </summary>
        ''' <param name="updateCommand"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateInfoFirmaDigitaleVisita(updateCommand As ArchiviazioneDIRV.FirmaDigitaleInfo) As Integer Implements IVisiteProvider.UpdateInfoFirmaDigitaleVisita

            Dim count As Integer = 0

            Using cmd As OracleCommand = Connection.CreateCommand()

                Dim query As New System.Text.StringBuilder("update t_vis_visite set ")

                query.Append(" vis_doc_id_documento = :vis_doc_id_documento, ")
                cmd.Parameters.AddWithValue("vis_doc_id_documento", updateCommand.IdDocumento)

                If updateCommand.DataFirma.HasValue Then
                    query.Append(" vis_data_firma = :vis_data_firma, ")
                    cmd.Parameters.AddWithValue("vis_data_firma", updateCommand.DataFirma.Value)
                End If

                If updateCommand.IdUtenteFirma.HasValue Then
                    query.Append(" vis_ute_id_firma = :vis_ute_id_firma, ")
                    cmd.Parameters.AddWithValue("vis_ute_id_firma", updateCommand.IdUtenteFirma.Value)
                End If

                query.Remove(query.Length - 2, 2)

                query.Append(" where vis_id = :vis_id ")
                cmd.Parameters.AddWithValue("vis_id", updateCommand.IdVisita)

                cmd.CommandText = query.ToString()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteNonQuery()

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
        ''' Aggiorna i campi della visita relativi all'archiviazione sostitutiva con i valori specificati
        ''' </summary>
        ''' <param name="updateCommand"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateInfoArchiviazioneSostitutivaVisita(updateCommand As ArchiviazioneDIRV.FirmaDigitaleInfo) As Integer Implements IVisiteProvider.UpdateInfoArchiviazioneSostitutivaVisita

            Dim count As Integer = 0

            Using cmd As OracleCommand = Connection.CreateCommand()

                Dim query As New Text.StringBuilder("update t_vis_visite set ")

                If updateCommand.DataArchiviazione.HasValue Then
                    query.Append(" vis_data_archiviazione = :vis_data_archiviazione, ")
                    cmd.Parameters.AddWithValue("vis_data_archiviazione", updateCommand.DataArchiviazione.Value)
                End If

                If updateCommand.IdUtenteArchiviazione.HasValue Then
                    query.Append(" vis_ute_id_archiviazione = :vis_ute_id_archiviazione, ")
                    cmd.Parameters.AddWithValue("vis_ute_id_archiviazione", updateCommand.IdUtenteArchiviazione.Value)
                End If

                query.Remove(query.Length - 2, 2)

                query.Append(" where vis_id = :vis_id ")
                cmd.Parameters.AddWithValue("vis_id", updateCommand.IdVisita)

                cmd.CommandText = query.ToString()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteNonQuery()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        count = Convert.ToInt32(obj)
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

#End Region

#Region " Private "

        Private Function GetVisiteListFromReader(idr As IDataReader) As List(Of Visita)

            Dim visiteList As New List(Of Visita)()

            If Not idr Is Nothing Then

                Dim vis_id As Integer = idr.GetOrdinal("vis_id")
                Dim vis_paz_codice As Integer = idr.GetOrdinal("vis_paz_codice")
                Dim vis_paz_codice_old As Integer = idr.GetOrdinal("vis_paz_codice_old")
                Dim vis_data_visita As Integer = idr.GetOrdinal("vis_data_visita")
                Dim vis_ope_codice As Integer = idr.GetOrdinal("vis_ope_codice")
                Dim descrizione_medico As Integer = idr.GetOrdinal("descrizione_medico")
                Dim vis_mal_codice As Integer = idr.GetOrdinal("vis_mal_codice")
                Dim mal_descrizione As Integer = idr.GetOrdinal("mal_descrizione")
                Dim vis_n_bilancio As Integer = idr.GetOrdinal("vis_n_bilancio")
                Dim vis_ute_id As Integer = idr.GetOrdinal("vis_ute_id")
                Dim vis_data_registrazione As Integer = idr.GetOrdinal("vis_data_registrazione")
                Dim vis_ute_id_variazione As Integer = idr.GetOrdinal("vis_ute_id_variazione")
                Dim vis_data_variazione As Integer = idr.GetOrdinal("vis_data_variazione")
                Dim vis_fine_sospensione As Integer = idr.GetOrdinal("vis_fine_sospensione")
                Dim vis_mos_codice As Integer = idr.GetOrdinal("vis_mos_codice")
                Dim mos_descrizione As Integer = idr.GetOrdinal("mos_descrizione")
                Dim vis_cns_codice As Integer = idr.GetOrdinal("vis_cns_codice")
                Dim cns_descrizione As Integer = idr.GetOrdinal("cns_descrizione")
                Dim vis_patologia As Integer = idr.GetOrdinal("vis_patologia")
                Dim vis_peso As Integer = idr.GetOrdinal("vis_peso")
                Dim vis_altezza As Integer = idr.GetOrdinal("vis_altezza")
                Dim vis_cranio As Integer = idr.GetOrdinal("vis_cranio")
                Dim vis_percentile_peso As Integer = idr.GetOrdinal("vis_percentile_peso")
                Dim vis_percentile_altezza As Integer = idr.GetOrdinal("vis_percentile_altezza")
                Dim vis_percentile_cranio As Integer = idr.GetOrdinal("vis_percentile_cranio")
                Dim vis_firma As Integer = idr.GetOrdinal("vis_firma")
                Dim vis_vaccinabile As Integer = idr.GetOrdinal("vis_vaccinabile")
                Dim vis_note As Integer = idr.GetOrdinal("vis_note")
                Dim vis_usl_inserimento As Integer = idr.GetOrdinal("vis_usl_inserimento")
                Dim vis_flag_visibilita As Integer = idr.GetOrdinal("vis_flag_visibilita")
                Dim vis_note_acquisizione As Integer = idr.GetOrdinal("vis_note_acquisizione")
                Dim bil_descrizione As Integer = idr.GetOrdinal("bil_descrizione")
                Dim vis_doc_id_documento As Integer = idr.GetOrdinal("vis_doc_id_documento")
                Dim vis_data_firma As Integer = idr.GetOrdinal("vis_data_firma")
                Dim vis_ute_id_firma As Integer = idr.GetOrdinal("vis_ute_id_firma")
                Dim vis_data_archiviazione As Integer = idr.GetOrdinal("vis_data_archiviazione")
                Dim vis_ute_id_archiviazione As Integer = idr.GetOrdinal("vis_ute_id_archiviazione")
                Dim vis_data_inizio_viaggio As Integer = idr.GetOrdinal("vis_data_inizio_viaggio")
                Dim vis_data_fine_viaggio As Integer = idr.GetOrdinal("vis_data_fine_viaggio")
                Dim vis_cit_codice_paese_viaggio As Integer = idr.GetOrdinal("vis_cit_codice_paese_viaggio")
                Dim cit_stato As Integer = idr.GetOrdinal("cit_stato")
                Dim vis_ope_codice_rilevatore As Integer = idr.GetOrdinal("vis_ope_codice_rilevatore")
                Dim descrizione_rilevatore As Integer = idr.GetOrdinal("descrizione_rilevatore")
                Dim vis_vaccinazioni_bilancio As Integer = idr.GetOrdinal("vis_vaccinazioni_bilancio")
                Dim vis_data_follow_up_previsto As Integer = idr.GetOrdinal("vis_data_follow_up_previsto")
                Dim vis_data_follow_up_effettivo As Integer = idr.GetOrdinal("vis_data_follow_up_effettivo")
                Dim vis_vis_id_follow_up As Integer = idr.GetOrdinal("vis_vis_id_follow_up")

                Dim visita As Visita = Nothing

                While idr.Read()

                    visita = New Visita()

                    visita.IdVisita = idr.GetInt32OrDefault(vis_id)
                    visita.CodicePaziente = idr.GetDecimal(vis_paz_codice)
                    visita.CodicePazienteAlias = idr.GetNullableInt32OrDefault(vis_paz_codice_old)
                    visita.DataVisita = idr.GetDateTimeOrDefault(vis_data_visita)
                    visita.MedicoCodice = idr.GetStringOrDefault(vis_ope_codice)
                    visita.MedicoDescrizione = idr.GetStringOrDefault(descrizione_medico)
                    visita.MalattiaCodice = idr.GetStringOrDefault(vis_mal_codice)
                    visita.MalattiaDescrizione = idr.GetStringOrDefault(mal_descrizione)
                    visita.BilancioNumero = idr.GetNullableInt64OrDefault(vis_n_bilancio)
                    visita.IdUtente = idr.GetInt32OrDefault(vis_ute_id)
                    visita.DataRegistrazione = idr.GetDateTimeOrDefault(vis_data_registrazione)
                    visita.IdUtenteModifica = idr.GetNullableInt64OrDefault(vis_ute_id_variazione)
                    visita.DataModifica = idr.GetNullableDateTimeOrDefault(vis_data_variazione)
                    visita.DataFineSospensione = idr.GetDateTimeOrDefault(vis_fine_sospensione)
                    visita.MotivoSospensioneCodice = idr.GetStringOrDefault(vis_mos_codice)
                    visita.MotivoSospensioneDescrizione = idr.GetStringOrDefault(mos_descrizione)
                    visita.CodiceConsultorio = idr.GetStringOrDefault(vis_cns_codice)
                    visita.DescrizioneConsultorio = idr.GetStringOrDefault(cns_descrizione)
                    visita.FlagPatologia = idr.GetStringOrDefault(vis_patologia)
                    visita.Peso = idr.GetNullableDoubleOrDefault(vis_peso)
                    visita.Altezza = idr.GetNullableDoubleOrDefault(vis_altezza)
                    visita.Cranio = idr.GetNullableDoubleOrDefault(vis_cranio)
                    visita.PercentilePeso = idr.GetStringOrDefault(vis_percentile_peso)
                    visita.PercentileAltezza = idr.GetStringOrDefault(vis_percentile_altezza)
                    visita.PercentileCranio = idr.GetStringOrDefault(vis_percentile_cranio)
                    visita.Firma = idr.GetStringOrDefault(vis_firma)
                    visita.Vaccinabile = idr.GetStringOrDefault(vis_vaccinabile)
                    visita.Note = idr.GetStringOrDefault(vis_note)
                    visita.CodiceUslInserimento = idr.GetStringOrDefault(vis_usl_inserimento)
                    visita.FlagVisibilitaDatiVaccinaliCentrale = idr.GetStringOrDefault(vis_flag_visibilita)
                    visita.NoteAcquisizioneDatiVaccinaliCentrale = idr.GetStringOrDefault(vis_note_acquisizione)
                    visita.BilancioDescrizione = idr.GetStringOrDefault(bil_descrizione)
                    visita.IdDocumento = idr.GetNullableInt64OrDefault(vis_doc_id_documento)
                    visita.DataFirmaDigitale = idr.GetNullableDateTimeOrDefault(vis_data_firma)
                    visita.IdUtenteFirmaDigitale = idr.GetNullableInt64OrDefault(vis_ute_id_firma)
                    visita.DataArchiviazione = idr.GetNullableDateTimeOrDefault(vis_data_archiviazione)
                    visita.IdUtenteArchiviazione = idr.GetNullableInt64OrDefault(vis_ute_id_archiviazione)
                    visita.DataInizioViaggio = idr.GetNullableDateTimeOrDefault(vis_data_inizio_viaggio)
                    visita.DataFineViaggio = idr.GetNullableDateTimeOrDefault(vis_data_fine_viaggio)
                    visita.PaeseViaggioCodice = idr.GetStringOrDefault(vis_cit_codice_paese_viaggio)
                    visita.PaeseViaggioDescrizione = idr.GetStringOrDefault(cit_stato)
                    visita.RilevatoreCodice = idr.GetStringOrDefault(vis_ope_codice_rilevatore)
                    visita.RilevatoreDescrizione = idr.GetStringOrDefault(descrizione_rilevatore)
                    visita.VaccinazioniBilancio = idr.GetStringOrDefault(vis_vaccinazioni_bilancio)
                    visita.DataFollowUpEffettivo = idr.GetNullableDateTimeOrDefault(vis_data_follow_up_effettivo)
                    visita.DataFollowUpPrevisto = idr.GetNullableDateTimeOrDefault(vis_data_follow_up_previsto)
                    visita.FollowUpId = idr.GetNullableInt32OrDefault(vis_vis_id_follow_up)

                    visiteList.Add(visita)

                End While

            End If

            Return visiteList

        End Function
        Private Function GetViaggiVisiteListFromReader(idr As IDataReader) As List(Of ViaggioVisita)

            Dim visiteList As New List(Of ViaggioVisita)()

            If Not idr Is Nothing Then

                Dim vvg_id As Integer = idr.GetOrdinal("VVG_ID")
                Dim vvg_vis_id As Integer = idr.GetOrdinal("VVG_VIS_ID")
                Dim vvg_data_inizio_viaggio As Integer = idr.GetOrdinal("VVG_DATA_INIZIO")
                Dim vvg_data_fine_viaggio As Integer = idr.GetOrdinal("VVG_DATA_FINE")
                Dim vvg_cit_codice_paese_viaggio As Integer = idr.GetOrdinal("VVG_CIT_CODICE_PAESE_VIAGGIO")
                Dim cit_stato As Integer = idr.GetOrdinal("CIT_STATO")



                While idr.Read()

                    Dim viaggioVisita As New ViaggioVisita()

                    viaggioVisita.Id = idr.GetInt32(vvg_id)
                    viaggioVisita.IdVisita = idr.GetInt32(vvg_vis_id)
                    viaggioVisita.DataInizioViaggio = idr.GetNullableDateTimeOrDefault(vvg_data_inizio_viaggio)
                    viaggioVisita.DataFineViaggio = idr.GetNullableDateTimeOrDefault(vvg_data_fine_viaggio)
                    viaggioVisita.CodicePaese = idr.GetStringOrDefault(vvg_cit_codice_paese_viaggio)
                    viaggioVisita.DescPaese = idr.GetStringOrDefault(cit_stato)


                    visiteList.Add(viaggioVisita)

                End While

            End If

            Return visiteList

        End Function

        Private Function GetVisiteEliminateListFromReader(idr As IDataReader) As List(Of Visita)

            Dim visiteEliminateList As New List(Of Visita)

            If Not idr Is Nothing Then

                Dim vie_paz_codice As Integer = idr.GetOrdinal("vie_paz_codice")
                Dim vie_paz_codice_old As Integer = idr.GetOrdinal("vie_paz_codice_old")
                Dim vie_data_visita As Integer = idr.GetOrdinal("vie_data_visita")
                Dim vie_ope_codice As Integer = idr.GetOrdinal("vie_ope_codice")
                Dim descrizione_medico As Integer = idr.GetOrdinal("descrizione_medico")
                Dim vie_mal_codice As Integer = idr.GetOrdinal("vie_mal_codice")
                Dim mal_descrizione As Integer = idr.GetOrdinal("mal_descrizione")
                Dim vie_n_bilancio As Integer = idr.GetOrdinal("vie_n_bilancio")
                Dim vie_fine_sospensione As Integer = idr.GetOrdinal("vie_fine_sospensione")
                Dim vie_data_registrazione As Integer = idr.GetOrdinal("vie_data_registrazione")
                Dim vie_mos_codice As Integer = idr.GetOrdinal("vie_mos_codice")
                Dim mos_descrizione As Integer = idr.GetOrdinal("mos_descrizione")
                Dim vie_id As Integer = idr.GetOrdinal("vie_id")
                Dim vie_cns_codice As Integer = idr.GetOrdinal("vie_cns_codice")
                Dim cns_descrizione As Integer = idr.GetOrdinal("cns_descrizione")
                Dim vie_patologia As Integer = idr.GetOrdinal("vie_patologia")
                Dim vie_peso As Integer = idr.GetOrdinal("vie_peso")
                Dim vie_altezza As Integer = idr.GetOrdinal("vie_altezza")
                Dim vie_cranio As Integer = idr.GetOrdinal("vie_cranio")
                Dim vie_percentile_peso As Integer = idr.GetOrdinal("vie_percentile_peso")
                Dim vie_percentile_altezza As Integer = idr.GetOrdinal("vie_percentile_altezza")
                Dim vie_percentile_cranio As Integer = idr.GetOrdinal("vie_percentile_cranio")
                Dim vie_firma As Integer = idr.GetOrdinal("vie_firma")
                Dim vie_vaccinabile As Integer = idr.GetOrdinal("vie_vaccinabile")
                Dim vie_ute_id As Integer = idr.GetOrdinal("vie_ute_id")
                Dim vie_note As Integer = idr.GetOrdinal("vie_note")
                Dim vie_usl_inserimento As Integer = idr.GetOrdinal("vie_usl_inserimento")
                Dim vie_flag_visibilita As Integer = idr.GetOrdinal("vie_flag_visibilita")
                Dim vie_note_acquisizione As Integer = idr.GetOrdinal("vie_note_acquisizione")
                Dim vie_data_variazione As Integer = idr.GetOrdinal("vie_data_variazione")
                Dim vie_ute_id_variazione As Integer = idr.GetOrdinal("vie_ute_id_variazione")
                Dim vie_data_eliminazione As Integer = idr.GetOrdinal("vie_data_eliminazione")
                Dim vie_ute_id_eliminazione As Integer = idr.GetOrdinal("vie_ute_id_eliminazione")
                Dim vie_doc_id_documento As Integer = idr.GetOrdinal("vie_doc_id_documento")
                Dim vie_data_firma As Integer = idr.GetOrdinal("vie_data_firma")
                Dim vie_ute_id_firma As Integer = idr.GetOrdinal("vie_ute_id_firma")
                Dim vie_data_archiviazione As Integer = idr.GetOrdinal("vie_data_archiviazione")
                Dim vie_ute_id_archiviazione As Integer = idr.GetOrdinal("vie_ute_id_archiviazione")
                Dim vie_data_inizio_viaggio As Integer = idr.GetOrdinal("vie_data_inizio_viaggio")
                Dim vie_data_fine_viaggio As Integer = idr.GetOrdinal("vie_data_fine_viaggio")
                Dim vie_cit_codice_paese_viaggio As Integer = idr.GetOrdinal("vie_cit_codice_paese_viaggio")
                Dim cit_stato As Integer = idr.GetOrdinal("cit_stato")
                Dim vie_ope_codice_rilevatore As Integer = idr.GetOrdinal("vie_ope_codice_rilevatore")
                Dim descrizione_rilevatore As Integer = idr.GetOrdinal("descrizione_rilevatore")
                Dim vie_vaccinazioni_bilancio As Integer = idr.GetOrdinal("vie_vaccinazioni_bilancio")

                Dim visitaEliminata As Visita = Nothing

                While idr.Read()

                    visitaEliminata = New Visita()

                    visitaEliminata.IdVisita = idr.GetInt32OrDefault(vie_id)
                    visitaEliminata.CodicePaziente = idr.GetDecimal(vie_paz_codice)
                    visitaEliminata.CodicePazienteAlias = idr.GetInt32OrDefault(vie_paz_codice_old)
                    visitaEliminata.DataVisita = idr.GetDateTimeOrDefault(vie_data_visita)
                    visitaEliminata.MedicoCodice = idr.GetStringOrDefault(vie_ope_codice)
                    visitaEliminata.MedicoDescrizione = idr.GetStringOrDefault(descrizione_medico)
                    visitaEliminata.MalattiaCodice = idr.GetStringOrDefault(vie_mal_codice)
                    visitaEliminata.MalattiaDescrizione = idr.GetStringOrDefault(mal_descrizione)
                    visitaEliminata.BilancioNumero = idr.GetNullableInt64OrDefault(vie_n_bilancio)
                    visitaEliminata.DataFineSospensione = idr.GetDateTimeOrDefault(vie_fine_sospensione)
                    visitaEliminata.DataRegistrazione = idr.GetDateTimeOrDefault(vie_data_registrazione)
                    visitaEliminata.MotivoSospensioneCodice = idr.GetStringOrDefault(vie_mos_codice)
                    visitaEliminata.MotivoSospensioneDescrizione = idr.GetStringOrDefault(mos_descrizione)
                    visitaEliminata.CodiceConsultorio = idr.GetStringOrDefault(vie_cns_codice)
                    visitaEliminata.DescrizioneConsultorio = idr.GetStringOrDefault(cns_descrizione)
                    visitaEliminata.FlagPatologia = idr.GetStringOrDefault(vie_patologia)
                    visitaEliminata.Peso = idr.GetNullableDoubleOrDefault(vie_peso)
                    visitaEliminata.Altezza = idr.GetNullableDoubleOrDefault(vie_altezza)
                    visitaEliminata.Cranio = idr.GetNullableDoubleOrDefault(vie_cranio)
                    visitaEliminata.PercentilePeso = idr.GetStringOrDefault(vie_percentile_peso)
                    visitaEliminata.PercentileAltezza = idr.GetStringOrDefault(vie_percentile_altezza)
                    visitaEliminata.PercentileCranio = idr.GetStringOrDefault(vie_percentile_cranio)
                    visitaEliminata.Firma = idr.GetStringOrDefault(vie_firma)
                    visitaEliminata.Vaccinabile = idr.GetStringOrDefault(vie_vaccinabile)
                    visitaEliminata.IdUtente = idr.GetInt32OrDefault(vie_ute_id)
                    visitaEliminata.Note = idr.GetStringOrDefault(vie_note)
                    visitaEliminata.CodiceUslInserimento = idr.GetStringOrDefault(vie_usl_inserimento)
                    visitaEliminata.FlagVisibilitaDatiVaccinaliCentrale = idr.GetStringOrDefault(vie_flag_visibilita)
                    visitaEliminata.DataModifica = idr.GetNullableDateTimeOrDefault(vie_data_variazione)
                    visitaEliminata.IdUtenteModifica = idr.GetNullableInt32OrDefault(vie_ute_id_variazione)
                    visitaEliminata.DataEliminazione = idr.GetNullableDateTimeOrDefault(vie_data_eliminazione)
                    visitaEliminata.IdUtenteEliminazione = idr.GetNullableInt32OrDefault(vie_ute_id_eliminazione)
                    visitaEliminata.NoteAcquisizioneDatiVaccinaliCentrale = idr.GetStringOrDefault(vie_note_acquisizione)
                    visitaEliminata.IdDocumento = idr.GetNullableInt64OrDefault(vie_doc_id_documento)
                    visitaEliminata.DataFirmaDigitale = idr.GetNullableDateTimeOrDefault(vie_data_firma)
                    visitaEliminata.IdUtenteFirmaDigitale = idr.GetNullableInt64OrDefault(vie_ute_id_firma)
                    visitaEliminata.DataArchiviazione = idr.GetNullableDateTimeOrDefault(vie_data_archiviazione)
                    visitaEliminata.IdUtenteArchiviazione = idr.GetNullableInt64OrDefault(vie_ute_id_archiviazione)
                    visitaEliminata.DataInizioViaggio = idr.GetNullableDateTimeOrDefault(vie_data_inizio_viaggio)
                    visitaEliminata.DataFineViaggio = idr.GetNullableDateTimeOrDefault(vie_data_fine_viaggio)
                    visitaEliminata.PaeseViaggioCodice = idr.GetStringOrDefault(vie_cit_codice_paese_viaggio)
                    visitaEliminata.PaeseViaggioDescrizione = idr.GetStringOrDefault(cit_stato)
                    visitaEliminata.RilevatoreCodice = idr.GetStringOrDefault(vie_ope_codice_rilevatore)
                    visitaEliminata.RilevatoreDescrizione = idr.GetStringOrDefault(descrizione_rilevatore)
                    visitaEliminata.VaccinazioniBilancio = idr.GetStringOrDefault(vie_vaccinazioni_bilancio)

                    visiteEliminateList.Add(visitaEliminata)

                End While

            End If

            Return visiteEliminateList

        End Function

        Private Function GetOsservazioniEnumerable(codicePaziente As Int64, codiceOsservazione As String, numeroBilancio As Int16, codiceMalattia As String) As IEnumerable(Of Osservazione)

            Dim osservazioneList As List(Of Osservazione)

            With _DAM.QB

                .NewQuery()
                .AddSelectFields("*")
                .AddTables("T_VIS_OSSERVAZIONI")
                .AddWhereCondition("VOS_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("VOS_BIL_N_BILANCIO", Comparatori.Uguale, numeroBilancio, DataTypes.Numero)
                .AddWhereCondition("VOS_MAL_CODICE", Comparatori.Uguale, codiceMalattia, DataTypes.Stringa)

                If Not String.IsNullOrEmpty(codiceOsservazione) Then
                    .AddWhereCondition("VOS_OSS_CODICE", Comparatori.Uguale, codiceOsservazione, DataTypes.Stringa)
                End If

            End With

            Using idr As IDataReader = _DAM.BuildDataReader()
                osservazioneList = GetOsservazioneListFromDataReader(idr, False)
            End Using

            Return osservazioneList.AsEnumerable()

        End Function

        Private Function GetOsservazioneListFromDataReader(idr As IDataReader, eliminate As Boolean) As List(Of Osservazione)

            Dim osservazioneList As New List(Of Osservazione)()

            Dim fieldPrefix As String

            If eliminate Then
                fieldPrefix = "voe"
            Else
                fieldPrefix = "vos"
            End If

            If Not idr Is Nothing Then

                Dim vos_id As Integer = idr.GetOrdinal(fieldPrefix + "_id")
                Dim vos_mal_codice As Integer = idr.GetOrdinal(fieldPrefix + "_mal_codice")
                Dim vos_paz_codice As Integer = idr.GetOrdinal(fieldPrefix + "_paz_codice")
                Dim vos_paz_codice_old As Integer = idr.GetOrdinal(fieldPrefix + "_paz_codice_old")
                Dim vos_data_visita As Integer = idr.GetOrdinal(fieldPrefix + "_data_visita")
                Dim vos_vis_id As Integer = idr.GetOrdinal(fieldPrefix + "_vis_id")
                Dim vos_bil_n_bilancio As Integer = idr.GetOrdinal(fieldPrefix + "_bil_n_bilancio")
                Dim vos_oss_codice As Integer = idr.GetOrdinal(fieldPrefix + "_oss_codice")
                Dim vos_ris_codice As Integer = idr.GetOrdinal(fieldPrefix + "_ris_codice")
                Dim vos_risposta As Integer = idr.GetOrdinal(fieldPrefix + "_risposta")
                Dim vos_data_variazione As Integer = idr.GetOrdinal(fieldPrefix + "_data_variazione")
                Dim vos_ute_id_variazione As Integer = idr.GetOrdinal(fieldPrefix + "_ute_id_variazione")
                Dim vos_note_acquisizione As Integer = idr.GetOrdinal(fieldPrefix + "_note_acquisizione")
                Dim vos_data_registrazione As Integer = idr.GetOrdinal(fieldPrefix + "_data_registrazione")
                Dim vos_ute_id_registrazione As Integer = idr.GetOrdinal(fieldPrefix + "_ute_id_registrazione")

                Dim vos_data_eliminazione As Integer
                Dim vos_ute_id_eliminazione As Integer

                If eliminate Then
                    vos_data_eliminazione = idr.GetOrdinal(fieldPrefix + "_data_eliminazione")
                    vos_ute_id_eliminazione = idr.GetOrdinal(fieldPrefix + "_ute_id_eliminazione")
                End If

                While idr.Read()

                    Dim osservazione As New Osservazione()

                    osservazione.Id = idr.GetInt64(vos_id)
                    osservazione.CodiceMalattia = idr.GetStringOrDefault(vos_mal_codice)
                    osservazione.CodicePaziente = idr.GetInt32OrDefault(vos_paz_codice)
                    osservazione.CodicePazienteOld = idr.GetInt32OrDefault(vos_paz_codice_old)
                    osservazione.DataVisita = idr.GetDateTimeOrDefault(vos_data_visita)
                    osservazione.IdVisita = idr.GetInt32OrDefault(vos_vis_id)
                    osservazione.NumeroBilancio = idr.GetInt32OrDefault(vos_bil_n_bilancio)
                    osservazione.OsservazioneCodice = idr.GetStringOrDefault(vos_oss_codice)
                    osservazione.RispostaCodice = idr.GetStringOrDefault(vos_ris_codice)
                    osservazione.RispostaTesto = idr.GetStringOrDefault(vos_risposta)

                    osservazione.NoteAcquisizioneDatiVaccinaliCentrale = idr.GetStringOrDefault(vos_note_acquisizione)

                    osservazione.DataVariazione = idr.GetNullableDateTimeOrDefault(vos_data_variazione)
                    osservazione.IdUtenteVariazione = idr.GetNullableInt64OrDefault(vos_ute_id_variazione)

                    osservazione.DataRegistrazione = idr.GetDateTimeOrDefault(vos_data_registrazione)
                    osservazione.IdUtenteRegistrazione = idr.GetInt64OrDefault(vos_ute_id_registrazione)

                    If eliminate Then
                        osservazione.DataEliminazione = idr.GetNullableDateTimeOrDefault(vos_data_eliminazione)
                        osservazione.IdUtenteEliminazione = idr.GetNullableInt64OrDefault(vos_ute_id_eliminazione)
                    End If

                    osservazioneList.Add(osservazione)

                End While

            End If

            Return osservazioneList

        End Function

        ''' <summary>
        ''' Restituisce il valore dell'id della visita prendendolo dalla sequence
        ''' </summary>
        ''' <returns></returns>
        Private Function GetNextIdVisita() As Integer

            Using cmd As OracleCommand = Connection.CreateCommand()

                cmd.CommandText = OnVac.Queries.Visite.OracleQueries.selNextSeqVisite

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Return Convert.ToInt32(cmd.ExecuteScalar())

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

        End Function
        Private Function GetNextIdViaggio() As Integer

            Using cmd As OracleCommand = Connection.CreateCommand()

                cmd.CommandText = OnVac.Queries.Visite.OracleQueries.selNextSeqViaggioVisite

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Return Convert.ToInt32(cmd.ExecuteScalar())

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

        End Function

        ''' <summary>
        ''' Restituisce il valore dell'id della Osservazione prendendolo dalla sequence
        ''' </summary>
        Private Function GetNextIdOsservazione() As Integer

            Using cmd As OracleCommand = Connection.CreateCommand()

                cmd.CommandText = OnVac.Queries.Visite.OracleQueries.selNextSeqOsservazioni

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Return Convert.ToInt32(cmd.ExecuteScalar())

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

        End Function

#End Region

    End Class

End Namespace
