Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.Data.OracleClient


Namespace DAL.Oracle

    Public Class DBVaccinazioneEsclusaCentraleProvider
        Inherits DbProvider
        Implements IVaccinazioneEsclusaCentraleProvider

#Region " Private variables "

        Private ci As System.Globalization.CultureInfo = System.Globalization.CultureInfo.InvariantCulture
        Private ArgomentoLog As String

#End Region

#Region " Constructors "

        Public Sub New(DAM As IDAM)

            MyBase.New(DAM)

            Me.ArgomentoLog = Log.DataLogStructure.TipiArgomento.VAC_ESEGUITE_CENTRALE

        End Sub

#End Region

#Region " IVaccinazioneEsclusaCentraleProvider "

        Public Function IsVaccinazioneEsclusaCentraleEliminata(idVaccinazioneEsclusaCentrale As Int64) As Boolean

            Dim isEliminata As Boolean = False

            Using cmd As New OracleClient.OracleCommand("SELECT 1 FROM T_ESCLUSE_CENTRALE WHERE EXC_ID = :EXC_ID AND EXC_TIPO = :EXC_TIPO", Me.Connection)

                cmd.Parameters.AddWithValue("EXC_ID", idVaccinazioneEsclusaCentrale)
                cmd.Parameters.AddWithValue("EXC_TIPO", Constants.TipoVaccinazioneEsclusaCentrale.Eliminata)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    isEliminata = (Not cmd.ExecuteScalar() Is Nothing)

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return isEliminata

        End Function

        Public Function GetVaccinazioneEsclusaCentraleById(idVaccinazioneEsclusaCentrale As Long) As Entities.VaccinazioneEsclusaCentrale Implements IVaccinazioneEsclusaCentraleProvider.GetVaccinazioneEsclusaCentraleById

            Dim vaccinazioneEsclusaCentrale As VaccinazioneEsclusaCentrale

            Using cmd As New OracleClient.OracleCommand("SELECT * FROM T_ESCLUSE_CENTRALE WHERE EXC_ID = :EXC_ID", Me.Connection)

                cmd.Parameters.AddWithValue("EXC_ID", idVaccinazioneEsclusaCentrale)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As OracleClient.OracleDataReader = cmd.ExecuteReader()

                        vaccinazioneEsclusaCentrale = Me.GetVaccinazioneEsclusaCentraleListFromReader(idr).First()

                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return vaccinazioneEsclusaCentrale

        End Function

        Public Function GetVaccinazioneEsclusaCentraleByUslInserimento(idVaccinazioneEsclusa As Long, codiceUslInserimento As String) As VaccinazioneEsclusaCentrale Implements IVaccinazioneEsclusaCentraleProvider.GetVaccinazioneEsclusaCentraleByUslInserimento

            Dim vaccinazioneEsclusaCentrale As VaccinazioneEsclusaCentrale

            Using cmd As New OracleClient.OracleCommand("SELECT * FROM T_ESCLUSE_CENTRALE WHERE EXC_VEX_ID = :idVaccinazioneEsclusa AND EXC_USL_INSERIMENTO = :codiceUslInserimento", Me.Connection)

                cmd.Parameters.AddWithValue("idVaccinazioneEsclusa", idVaccinazioneEsclusa)
                cmd.Parameters.AddWithValue("codiceUslInserimento", codiceUslInserimento)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As OracleClient.OracleDataReader = cmd.ExecuteReader()

                        vaccinazioneEsclusaCentrale = GetVaccinazioneEsclusaCentraleListFromReader(idr).FirstOrDefault()

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return vaccinazioneEsclusaCentrale

        End Function

        Public Function GetVaccinazioneEsclusaCentraleDistribuitaByUsl(idVaccinazioneEsclusa As Long, codiceUsl As String) As VaccinazioneEsclusaCentrale.VaccinazioneEsclusaDistribuita Implements IVaccinazioneEsclusaCentraleProvider.GetVaccinazioneEsclusaCentraleDistribuitaByUsl

            Dim vaccinazioneEsclusaCentraleDistribuita As VaccinazioneEsclusaCentrale.VaccinazioneEsclusaDistribuita

            Using cmd As New OracleClient.OracleCommand("SELECT * FROM T_ESCLUSE_DISTRIBUITE WHERE EXD_VEX_ID = :idVaccinazioneEsclusa AND EXD_USL_CODICE = :codiceUsl", Me.Connection)

                cmd.Parameters.AddWithValue("idVaccinazioneEsclusa", idVaccinazioneEsclusa)
                cmd.Parameters.AddWithValue("codiceUsl", codiceUsl)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As OracleClient.OracleDataReader = cmd.ExecuteReader()

                        vaccinazioneEsclusaCentraleDistribuita = GetVaccinazioneEsclusaCentraleDistribuitaListFromReader(idr).FirstOrDefault()

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return vaccinazioneEsclusaCentraleDistribuita

        End Function

        Public Function GetVaccinazioneEsclusaCentraleDistribuitaByIdCentrale(idVaccinazioneEsclusaCentrale As Long, codiceUsl As String) As VaccinazioneEsclusaCentrale.VaccinazioneEsclusaDistribuita Implements IVaccinazioneEsclusaCentraleProvider.GetVaccinazioneEsclusaCentraleDistribuitaByIdCentrale

            Dim vaccinazioneEsclusaDistribuita As VaccinazioneEsclusaCentrale.VaccinazioneEsclusaDistribuita

            Using cmd As New OracleClient.OracleCommand("SELECT * FROM T_ESCLUSE_DISTRIBUITE WHERE EXD_EXC_ID = :idVaccinazioneEsclusaCentrale AND EXD_USL_CODICE = :codiceUsl", Me.Connection)

                cmd.Parameters.AddWithValue("idVaccinazioneEsclusaCentrale", GetLongParam(idVaccinazioneEsclusaCentrale))
                cmd.Parameters.AddWithValue("codiceUsl", GetStringParam(codiceUsl))

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As OracleClient.OracleDataReader = cmd.ExecuteReader()

                        vaccinazioneEsclusaDistribuita = GetVaccinazioneEsclusaCentraleDistribuitaListFromReader(idr).FirstOrDefault()

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return vaccinazioneEsclusaDistribuita

        End Function

        Public Function GetVaccinazioneEsclusaCentraleByIdLocale(idVaccinazioneEsclusaLocale As Long, codiceUslLocale As String) As VaccinazioneEsclusaCentrale Implements IVaccinazioneEsclusaCentraleProvider.GetVaccinazioneEsclusaCentraleByIdLocale

            Dim vaccinazioneEsclusaCentrale As VaccinazioneEsclusaCentrale = Nothing

            Using cmd As New OracleClient.OracleCommand(Queries.VaccinazioniEscluse.OracleQueries.selVaccinazioneEsclusaCentraleByIdLocale, Connection)

                cmd.Parameters.AddWithValue("idVaccinazioneEsclusa", GetLongParam(idVaccinazioneEsclusaLocale))
                cmd.Parameters.AddWithValue("codiceUsl", GetStringParam(codiceUslLocale))

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As OracleClient.OracleDataReader = cmd.ExecuteReader()

                        vaccinazioneEsclusaCentrale = GetVaccinazioneEsclusaCentraleListFromReader(idr).FirstOrDefault()

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return vaccinazioneEsclusaCentrale

        End Function

        Public Function GetVaccinazioneEsclusaCentraleDistribuitaEnumerable(codicePazienteLocale As Int64, codiceUsl As String) As IEnumerable(Of VaccinazioneEsclusaCentrale.VaccinazioneEsclusaDistribuita) Implements IVaccinazioneEsclusaCentraleProvider.GetVaccinazioneEsclusaCentraleDistribuitaEnumerable

            Dim vaccinazioneEsclusaDistribuitaList As List(Of VaccinazioneEsclusaCentrale.VaccinazioneEsclusaDistribuita)

            Using cmd As New OracleClient.OracleCommand("SELECT * FROM T_ESCLUSE_DISTRIBUITE WHERE  EXD_PAZ_CODICE_LOCALE = :codicePazienteLocale AND EXD_USL_CODICE = :codiceUsl", Me.Connection)

                Dim ownConnection As Boolean = False

                cmd.Parameters.AddWithValue("codiceUsl", GetStringParam(codiceUsl))
                cmd.Parameters.AddWithValue("codicePazienteLocale", GetLongParam(codicePazienteLocale))

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As OracleClient.OracleDataReader = cmd.ExecuteReader()

                        vaccinazioneEsclusaDistribuitaList = GetVaccinazioneEsclusaCentraleDistribuitaListFromReader(idr)

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return vaccinazioneEsclusaDistribuitaList.AsEnumerable()

        End Function

        Public Function GetVaccinazioneEsclusaCentraleEnumerable(codicePazienteCentrale As String) As IEnumerable(Of VaccinazioneEsclusaCentrale) Implements IVaccinazioneEsclusaCentraleProvider.GetVaccinazioneEsclusaCentraleEnumerable

            Return GetVaccinazioneEsclusaCentraleEnumerable(codicePazienteCentrale, Nothing)

        End Function

        Public Function GetVaccinazioneEsclusaCentraleEnumerable(codicePazienteCentrale As String, flagVisibilita As String) As IEnumerable(Of VaccinazioneEsclusaCentrale) Implements IVaccinazioneEsclusaCentraleProvider.GetVaccinazioneEsclusaCentraleEnumerable

            Dim vaccinazioneEsclusaCentraleList As List(Of VaccinazioneEsclusaCentrale)

            Using cmd As New OracleClient.OracleCommand()

                Dim ownConnection As Boolean = False

                cmd.Connection = Connection

                Dim query As String = "SELECT * FROM T_ESCLUSE_CENTRALE WHERE EXC_PAZ_CODICE_CENTRALE = :codicePazienteCentrale"

                cmd.Parameters.AddWithValue("codicePazienteCentrale", GetStringParam(codicePazienteCentrale))

                If Not String.IsNullOrEmpty(flagVisibilita) Then
                    query += " AND EXC_VISIBILITA = :flagVisibilita"
                    cmd.Parameters.AddWithValue("flagVisibilita", flagVisibilita)
                End If

                cmd.CommandText = query

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As OracleClient.OracleDataReader = cmd.ExecuteReader()

                        vaccinazioneEsclusaCentraleList = GetVaccinazioneEsclusaCentraleListFromReader(idr)

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return vaccinazioneEsclusaCentraleList.AsEnumerable()

        End Function

        Public Function CountVaccinazioniEsclusePazienteCentrale(codicePazienteCentrale As String, listVisibilita As List(Of String), noEliminate As Boolean) As Integer Implements IVaccinazioneEsclusaCentraleProvider.CountVaccinazioniEsclusePazienteCentrale

            Dim count As Integer = 0

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Connection

                Dim query As New System.Text.StringBuilder("SELECT COUNT(*) FROM T_ESCLUSE_CENTRALE WHERE EXC_PAZ_CODICE_CENTRALE = :EXC_PAZ_CODICE_CENTRALE")

                cmd.Parameters.AddWithValue("EXC_PAZ_CODICE_CENTRALE", codicePazienteCentrale)

                ' Filtro visibilità
                query.Append(GetFiltroVisibilita(listVisibilita, cmd))

                ' Filtro eliminate
                query.Append(GetFiltroEliminate(noEliminate, cmd))

                cmd.CommandText = query.ToString()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

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

        Public Function GetUslInserimentoEscluse(codicePazienteCentrale As String, noEliminate As Boolean) As List(Of String) Implements IVaccinazioneEsclusaCentraleProvider.GetUslInserimentoEscluse

            Dim listUslInserimento As New List(Of String)()

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Connection

                Dim query As New System.Text.StringBuilder("SELECT DISTINCT EXC_USL_INSERIMENTO FROM T_ESCLUSE_CENTRALE ")

                ' Filtro paziente
                query.Append(" WHERE EXC_PAZ_CODICE_CENTRALE = :EXC_PAZ_CODICE_CENTRALE ")
                cmd.Parameters.AddWithValue("EXC_PAZ_CODICE_CENTRALE", codicePazienteCentrale)

                '' Filtro visibilità
                'query.Append(Me.GetFiltroVisibilita(listVisibilita, cmd))

                ' Filtro eliminate
                query.Append(GetFiltroEliminate(noEliminate, cmd))

                cmd.CommandText = query.ToString()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            While idr.Read()

                                If Not idr.IsDBNull(0) Then
                                    listUslInserimento.Add(idr.GetString(0))
                                End If

                            End While

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listUslInserimento

        End Function

        ''' <summary>
        ''' Restituisce l'elenco delle usl gestite in cui sono state distribuite le vaccinazioni escluse, in base a id locali e usl inserimento
        ''' </summary>
        ''' <param name="idVaccinazioniEscluseLocali"></param>
        ''' <param name="codiceUslInserimentoEscluse"></param>
        ''' <returns></returns>
        Public Function GetUslDistribuiteVaccinazioniEscluse(idVaccinazioniEscluseLocali As List(Of Long), codiceUslInserimentoEscluse As String) As List(Of UslDistribuitaDatoVaccinaleInfo) Implements IVaccinazioneEsclusaCentraleProvider.GetUslDistribuiteVaccinazioniEscluse

            Dim listDatiUslDistribuite As New List(Of UslDistribuitaDatoVaccinaleInfo)()

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Connection

                Dim query As String = "SELECT EXC_VEX_ID ID, EXC_USL_INSERIMENTO USL_INSERIMENTO, EXD_USL_CODICE USL_DISTRIBUITA FROM T_ESCLUSE_DISTRIBUITE JOIN T_ESCLUSE_CENTRALE ON EXD_EXC_ID = EXC_ID WHERE EXC_USL_INSERIMENTO = :USL_INSERIMENTO AND EXC_VEX_ID IN ({0}) AND EXD_USL_CODICE <> EXC_USL_INSERIMENTO"

                cmd.Parameters.AddWithValue("USL_INSERIMENTO", codiceUslInserimentoEscluse)

                Dim filtroId As New System.Text.StringBuilder()

                For i As Integer = 0 To idVaccinazioniEscluseLocali.Count - 1

                    Dim parameterName As String = String.Format("p{0}", i)

                    filtroId.AppendFormat(":{0},", parameterName)

                    cmd.Parameters.AddWithValue(parameterName, idVaccinazioniEscluseLocali(i))

                Next

                If filtroId.Length > 0 Then filtroId.Remove(filtroId.Length - 1, 1)

                cmd.CommandText = String.Format(query, filtroId)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim id As Int16 = idr.GetOrdinal("ID")
                            Dim usl_inserimento As Int16 = idr.GetOrdinal("USL_INSERIMENTO")
                            Dim usl_distribuita As Int16 = idr.GetOrdinal("USL_DISTRIBUITA")

                            While idr.Read()

                                Dim datiUslDistribuita As New UslDistribuitaDatoVaccinaleInfo()

                                datiUslDistribuita.IdDatoVaccinale = idr.GetInt64(id)
                                datiUslDistribuita.CodiceUslInserimento = idr.GetString(usl_inserimento)
                                datiUslDistribuita.CodiceUslDistribuita = idr.GetString(usl_distribuita)

                                listDatiUslDistribuite.Add(datiUslDistribuita)

                            End While

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listDatiUslDistribuite

        End Function

#Region " Conflitti "

        Public Function CountVaccinazioneEsclusaCentraleInConflittoByIdConflitto(idConflitto As Long) As Long Implements IVaccinazioneEsclusaCentraleProvider.CountVaccinazioneEsclusaCentraleInConflittoByIdConflitto

            Using cmd As New OracleClient.OracleCommand("SELECT COUNT(*) FROM T_ESCLUSE_CENTRALE WHERE EXC_ID_CONFLITTO = :EXC_ID_CONFLITTO AND EXC_DATA_RISOLUZ_CONFLITTO IS NULL", Me.Connection)

                cmd.Parameters.AddWithValue("EXC_ID_CONFLITTO", idConflitto)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Return cmd.ExecuteScalar()

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

        End Function

        Public Function CountConflittiVaccinazioniEscluseCentrale(filtriRicercaConflitti As Filters.FiltriRicercaConflittiDatiVaccinali) As Integer Implements IVaccinazioneEsclusaCentraleProvider.CountConflittiVaccinazioniEscluseCentrale

            Dim count As Integer = 0

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                Dim query As System.Text.StringBuilder = Me.GetQueryRicercaConflittiMaster(cmd, filtriRicercaConflitti, True)

                cmd.CommandText = query.ToString()

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If obj Is Nothing OrElse obj Is DBNull.Value Then Return 0

                    count = Convert.ToInt32(obj)

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return count

        End Function

        Public Function GetConflittiVaccinazioniEscluseCentrale(filtriRicercaConflitti As Filters.FiltriRicercaConflittiDatiVaccinali, pagingOptions As OnAssistnet.Data.PagingOptions) As List(Of Entities.ConflittoVaccinazioniEscluse) Implements IVaccinazioneEsclusaCentraleProvider.GetConflittiVaccinazioniEscluseCentrale

            Dim listConflittiVaccinazioniEscluse As New List(Of Entities.ConflittoVaccinazioniEscluse)()

            ' Ricerca esclusioni "principali" in conflitto
            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                Dim query As System.Text.StringBuilder = Me.GetQueryRicercaConflittiMaster(cmd, filtriRicercaConflitti, False)

                query.Append(" ORDER BY PAZ_COGNOME, PAZ_NOME, PAZ_DATA_NASCITA, PAZ_CODICE")

                cmd.CommandText = query.ToString()

                If Not pagingOptions Is Nothing Then
                    cmd.AddPaginatedQuery(pagingOptions)
                End If

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As OracleClient.OracleDataReader = cmd.ExecuteReader()

                        If idr.HasRows Then

                            Dim PAZ_CODICE As Int16 = idr.GetOrdinal("PAZ_CODICE")
                            Dim PAZ_COGNOME As Int16 = idr.GetOrdinal("PAZ_COGNOME")
                            Dim PAZ_NOME As Int16 = idr.GetOrdinal("PAZ_NOME")
                            Dim PAZ_DATA_NASCITA As Int16 = idr.GetOrdinal("PAZ_DATA_NASCITA")
                            Dim EXC_ID As Int16 = idr.GetOrdinal("EXC_ID")

                            While idr.Read()

                                Dim conflittoVaccinazioniEscluse As New Entities.ConflittoVaccinazioniEscluse()

                                conflittoVaccinazioniEscluse.CodicePazienteCentrale = idr.GetString(PAZ_CODICE)
                                conflittoVaccinazioniEscluse.Cognome = idr.GetStringOrDefault(PAZ_COGNOME)
                                conflittoVaccinazioniEscluse.Nome = idr.GetStringOrDefault(PAZ_NOME)
                                conflittoVaccinazioniEscluse.DataNascita = idr.GetNullableDateTimeOrDefault(PAZ_DATA_NASCITA)
                                conflittoVaccinazioniEscluse.IdVaccinazioneEsclusaCentrale = idr.GetInt64(EXC_ID)

                                listConflittiVaccinazioniEscluse.Add(conflittoVaccinazioniEscluse)

                            End While

                        End If

                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            ' Ricerca dati dei conflitti per ogni esclusione trovata
            For Each conflittoVaccinazioniEscluse As Entities.ConflittoVaccinazioniEscluse In listConflittiVaccinazioniEscluse

                conflittoVaccinazioniEscluse.VaccinazioniEscluseInConflitto =
                    Me.GetDatiVaccinazioniInConflitto(conflittoVaccinazioniEscluse.IdVaccinazioneEsclusaCentrale)

            Next

            Return listConflittiVaccinazioniEscluse

        End Function

#End Region

#Region " Insert / Update / Delete "

        Public Sub InsertVaccinazioneEsclusaCentrale(vaccinazioneEsclusaCentrale As Entities.VaccinazioneEsclusaCentrale) Implements IVaccinazioneEsclusaCentraleProvider.InsertVaccinazioneEsclusaCentrale

            Using cmd As New OracleClient.OracleCommand("SELECT SEQ_ESCLUSE_CENTRALE.NEXTVAL FROM DUAL", Me.Connection)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    vaccinazioneEsclusaCentrale.Id = Convert.ToInt64(cmd.ExecuteScalar())

                    cmd.CommandText = "INSERT INTO T_ESCLUSE_CENTRALE(EXC_ID, EXC_TIPO, EXC_PAZ_CODICE_CENTRALE, EXC_PAZ_CODICE_LOCALE, EXC_VEX_ID, EXC_VISIBILITA, EXC_DATA_REVOCA_VISIBILITA, EXC_USL_INSERIMENTO, EXC_UTE_ID_INSERIMENTO, EXC_DATA_INSERIMENTO, EXC_UTE_ID_VARIAZIONE, EXC_DATA_VARIAZIONE, EXC_UTE_ID_ELIMINAZIONE, EXC_DATA_ELIMINAZIONE, EXC_PAZ_CODICE_ALIAS_CENTRALE, EXC_USL_CODICE_ALIAS, EXC_UTE_ID_ALIAS, EXC_DATA_ALIAS, EXC_ID_CONFLITTO, EXC_UTE_ID_RISOLUZ_CONFLITTO, EXC_DATA_RISOLUZ_CONFLITTO, EXC_UTE_ID_ULTIMA_OPERAZIONE) VALUES(:EXC_ID, :EXC_TIPO, :EXC_PAZ_CODICE_CENTRALE, :EXC_PAZ_CODICE_LOCALE, :EXC_VEX_ID, :EXC_VISIBILITA, :EXC_DATA_REVOCA_VISIBILITA, :EXC_USL_INSERIMENTO, :EXC_UTE_ID_INSERIMENTO, :EXC_DATA_INSERIMENTO, :EXC_UTE_ID_VARIAZIONE, :EXC_DATA_VARIAZIONE, :EXC_UTE_ID_ELIMINAZIONE, :EXC_DATA_ELIMINAZIONE, :EXC_PAZ_CODICE_ALIAS_CENTRALE, :EXC_USL_CODICE_ALIAS, :EXC_UTE_ID_ALIAS, :EXC_DATA_ALIAS, :EXC_ID_CONFLITTO, :EXC_UTE_ID_RISOLUZ_CONFLITTO, :EXC_DATA_RISOLUZ_CONFLITTO, :EXC_UTE_ID_ULTIMA_OPERAZIONE)"

                    Me.AddVaccinazioneEsclusaCentraleInsertOrUpdateParameters(cmd, vaccinazioneEsclusaCentrale)

                    cmd.ExecuteNonQuery()

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

        End Sub

        Public Sub InsertVaccinazioneEsclusaCentraleDistribuita(vaccinazioneEsclusaCentraleDistribuita As Entities.VaccinazioneEsclusaCentrale.VaccinazioneEsclusaDistribuita) Implements IVaccinazioneEsclusaCentraleProvider.InsertVaccinazioneEsclusaCentraleDistribuita

            Using cmd As New OracleClient.OracleCommand("SELECT SEQ_ESCLUSE_DISTRIBUITE.NEXTVAL FROM DUAL", Me.Connection)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    vaccinazioneEsclusaCentraleDistribuita.Id = Convert.ToInt64(cmd.ExecuteScalar())

                    cmd.CommandText = "INSERT INTO T_ESCLUSE_DISTRIBUITE(EXD_ID, EXD_EXC_ID, EXD_PAZ_CODICE_LOCALE, EXD_VEX_ID, EXD_USL_CODICE, EXD_UTE_ID_INS_LOCALE, EXD_DATA_INS_LOCALE, EXD_DATA_AGG_LOCALE) VALUES(:EXD_ID, :EXD_EXC_ID, :EXD_PAZ_CODICE_LOCALE, :EXD_VEX_ID, :EXD_USL_CODICE, :EXD_UTE_ID_INS_LOCALE, :EXD_DATA_INS_LOCALE, :EXD_DATA_AGG_LOCALE)"

                    Me.AddVaccinazioneEsclusaCentraleDistribuitaInsertOrUpdateParameters(cmd, vaccinazioneEsclusaCentraleDistribuita)

                    cmd.ExecuteNonQuery()

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

        End Sub

        Public Sub UpdateVaccinazioneEsclusaCentrale(vaccinazioneEsclusaCentrale As Entities.VaccinazioneEsclusaCentrale) Implements IVaccinazioneEsclusaCentraleProvider.UpdateVaccinazioneEsclusaCentrale

            Using cmd As New OracleClient.OracleCommand("UPDATE T_ESCLUSE_CENTRALE SET EXC_TIPO = :EXC_TIPO, EXC_PAZ_CODICE_CENTRALE=:EXC_PAZ_CODICE_CENTRALE, EXC_PAZ_CODICE_LOCALE=:EXC_PAZ_CODICE_LOCALE, EXC_VEX_ID=:EXC_VEX_ID, EXC_VISIBILITA=:EXC_VISIBILITA, EXC_DATA_REVOCA_VISIBILITA=:EXC_DATA_REVOCA_VISIBILITA, EXC_USL_INSERIMENTO=:EXC_USL_INSERIMENTO, EXC_UTE_ID_INSERIMENTO=:EXC_UTE_ID_INSERIMENTO, EXC_DATA_INSERIMENTO=:EXC_DATA_INSERIMENTO, EXC_UTE_ID_VARIAZIONE=:EXC_UTE_ID_VARIAZIONE, EXC_DATA_VARIAZIONE=:EXC_DATA_VARIAZIONE, EXC_UTE_ID_ELIMINAZIONE=:EXC_UTE_ID_ELIMINAZIONE, EXC_DATA_ELIMINAZIONE=:EXC_DATA_ELIMINAZIONE, EXC_PAZ_CODICE_ALIAS_CENTRALE=:EXC_PAZ_CODICE_ALIAS_CENTRALE, EXC_USL_CODICE_ALIAS=:EXC_USL_CODICE_ALIAS, EXC_UTE_ID_ALIAS=:EXC_UTE_ID_ALIAS, EXC_DATA_ALIAS =:EXC_DATA_ALIAS, EXC_ID_CONFLITTO=:EXC_ID_CONFLITTO, EXC_UTE_ID_RISOLUZ_CONFLITTO=:EXC_UTE_ID_RISOLUZ_CONFLITTO, EXC_DATA_RISOLUZ_CONFLITTO=:EXC_DATA_RISOLUZ_CONFLITTO, EXC_UTE_ID_ULTIMA_OPERAZIONE=:EXC_UTE_ID_ULTIMA_OPERAZIONE WHERE EXC_ID=:EXC_ID", Me.Connection)

                Me.AddVaccinazioneEsclusaCentraleInsertOrUpdateParameters(cmd, vaccinazioneEsclusaCentrale)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.ExecuteNonQuery()

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

        End Sub

        Public Sub UpdateVaccinazioneEsclusaCentraleDistribuita(vaccinazioneEsclusaCentraleDistribuita As Entities.VaccinazioneEsclusaCentrale.VaccinazioneEsclusaDistribuita) Implements IVaccinazioneEsclusaCentraleProvider.UpdateVaccinazioneEsclusaCentraleDistribuita

            Using cmd As New OracleClient.OracleCommand("UPDATE T_ESCLUSE_DISTRIBUITE SET EXD_EXC_ID=:EXD_EXC_ID, EXD_PAZ_CODICE_LOCALE=:EXD_PAZ_CODICE_LOCALE, EXD_VEX_ID=:EXD_VEX_ID, EXD_USL_CODICE=:EXD_USL_CODICE, EXD_UTE_ID_INS_LOCALE=:EXD_UTE_ID_INS_LOCALE, EXD_DATA_INS_LOCALE=:EXD_DATA_INS_LOCALE, EXD_DATA_AGG_LOCALE=:EXD_DATA_AGG_LOCALE WHERE EXD_ID=:EXD_ID", Me.Connection)

                Dim ownConnection As Boolean = False

                Me.AddVaccinazioneEsclusaCentraleDistribuitaInsertOrUpdateParameters(cmd, vaccinazioneEsclusaCentraleDistribuita)

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.ExecuteNonQuery()

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

        End Sub

        Public Sub UpdateIdConflittoVaccinazioneEsclusaCentraleByIdConflitto(idConflittoCorrente As Long, idConflittoAggiornato As Long?) Implements IVaccinazioneEsclusaCentraleProvider.UpdateIdConflittoVaccinazioneEsclusaCentraleByIdConflitto

            Using cmd As New OracleClient.OracleCommand("UPDATE SET EXC_ID_CONFLITTO = :EXC_ID_CONFLITTO_AGG WHERE EXC_ID_CONFLITTO = :EXC_ID_CONFLITTO_CORR", Me.Connection)

                cmd.Parameters.AddWithValue("EXC_ID_CONFLITTO_CORR", idConflittoCorrente)
                cmd.Parameters.AddWithValue("EXC_ID_CONFLITTO_AGG", IIf(idConflittoAggiornato.HasValue, idConflittoAggiornato, DBNull.Value))

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.ExecuteNonQuery()

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

        End Sub

        Public Sub DeleteVaccinazioneEsclusaCentraleDistribuita(vaccinazioneEsclusaCentraleDistribuita As Entities.VaccinazioneEsclusaCentrale.VaccinazioneEsclusaDistribuita) Implements IVaccinazioneEsclusaCentraleProvider.DeleteVaccinazioneEsclusaCentraleDistribuita

            Using cmd As New OracleClient.OracleCommand("DELETE FROM T_ESCLUSE_DISTRIBUITE WHERE EXD_ID=:EXD_ID", Me.Connection)

                cmd.Parameters.AddWithValue("EXD_ID", vaccinazioneEsclusaCentraleDistribuita.Id)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.ExecuteNonQuery()

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

        End Sub

#End Region

#End Region

#Region " Private "

        Private Function GetVaccinazioneEsclusaCentraleListFromReader(idr As OracleClient.OracleDataReader) As List(Of Entities.VaccinazioneEsclusaCentrale)

            Dim vaccinazioneEsclusaCentraleList As New List(Of Entities.VaccinazioneEsclusaCentrale)

            If idr.HasRows Then

                Dim EXC_ID_ordinal As Int16 = idr.GetOrdinal("EXC_ID")
                Dim EXC_TIPO_ordinal As Int16 = idr.GetOrdinal("EXC_TIPO")
                Dim EXC_PAZ_CODICE_CENTRALE_ordinal As Int16 = idr.GetOrdinal("EXC_PAZ_CODICE_CENTRALE")
                Dim EXC_PAZ_CODICE_LOCALE_ordinal As Int16 = idr.GetOrdinal("EXC_PAZ_CODICE_LOCALE")
                Dim EXC_VEX_ID_ordinal As Int16 = idr.GetOrdinal("EXC_VEX_ID")
                Dim EXC_VISIBILITA_ordinal As Int16 = idr.GetOrdinal("EXC_VISIBILITA")
                Dim EXC_DATA_REVOCA_VISIBILITA_ordinal As Int16 = idr.GetOrdinal("EXC_DATA_REVOCA_VISIBILITA")
                Dim EXC_USL_INSERIMENTO_ordinal As Int16 = idr.GetOrdinal("EXC_USL_INSERIMENTO")
                Dim EXC_UTE_ID_INSERIMENTO_ordinal As Int16 = idr.GetOrdinal("EXC_UTE_ID_INSERIMENTO")
                Dim EXC_DATA_INSERIMENTO_ordinal As Int16 = idr.GetOrdinal("EXC_DATA_INSERIMENTO")
                Dim EXC_UTE_ID_VARIAZIONE_ordinal As Int16 = idr.GetOrdinal("EXC_UTE_ID_VARIAZIONE")
                Dim EXC_DATA_VARIAZIONE_ordinal As Int16 = idr.GetOrdinal("EXC_DATA_VARIAZIONE")
                Dim EXC_UTE_ID_ELIMINAZIONE_ordinal As Int16 = idr.GetOrdinal("EXC_UTE_ID_ELIMINAZIONE")
                Dim EXC_DATA_ELIMINAZIONE_ordinal As Int16 = idr.GetOrdinal("EXC_DATA_ELIMINAZIONE")
                Dim EXC_PAZ_CODICE_ALIAS_CENTRALE_ordinal As Int16 = idr.GetOrdinal("EXC_PAZ_CODICE_ALIAS_CENTRALE")
                Dim EXC_USL_CODICE_ALIAS_ordinal As Int16 = idr.GetOrdinal("EXC_USL_CODICE_ALIAS")
                Dim EXC_UTE_ID_ALIAS_ordinal As Int16 = idr.GetOrdinal("EXC_UTE_ID_ALIAS")
                Dim EXC_DATA_ALIAS_ordinal As Int16 = idr.GetOrdinal("EXC_DATA_ALIAS")
                Dim EXC_ID_CONFLITTO_ordinal As Int16 = idr.GetOrdinal("EXC_ID_CONFLITTO")
                Dim EXC_DATA_RISOLUZ_CONFLITTO_ordinal As Int16 = idr.GetOrdinal("EXC_DATA_RISOLUZ_CONFLITTO")
                Dim EXC_UTE_ID_RISOLUZ_CONFLITTO_ordinal As Int16 = idr.GetOrdinal("EXC_UTE_ID_RISOLUZ_CONFLITTO")
                Dim EXC_UTE_ID_ULTIMA_OPERAZIONE_ordinal As Int16 = idr.GetOrdinal("EXC_UTE_ID_ULTIMA_OPERAZIONE")

                While idr.Read()

                    Dim vaccinazioneEsclusaCentrale As New VaccinazioneEsclusaCentrale()

                    vaccinazioneEsclusaCentrale.Id = idr.GetInt64(EXC_ID_ordinal)
                    vaccinazioneEsclusaCentrale.TipoVaccinazioneEsclusaCentrale = idr.GetStringOrDefault(EXC_TIPO_ordinal)
                    vaccinazioneEsclusaCentrale.IdVaccinazioneEsclusa = idr.GetInt64(EXC_VEX_ID_ordinal)
                    vaccinazioneEsclusaCentrale.CodicePazienteCentrale = idr.GetString(EXC_PAZ_CODICE_CENTRALE_ordinal)
                    vaccinazioneEsclusaCentrale.CodicePaziente = idr.GetInt64(EXC_PAZ_CODICE_LOCALE_ordinal)
                    vaccinazioneEsclusaCentrale.FlagVisibilitaCentrale = idr.GetString(EXC_VISIBILITA_ordinal)
                    vaccinazioneEsclusaCentrale.DataRevocaVisibilita = idr.GetDateTimeOrDefault(EXC_DATA_REVOCA_VISIBILITA_ordinal)
                    vaccinazioneEsclusaCentrale.CodiceUslVaccinazioneEsclusa = idr.GetString(EXC_USL_INSERIMENTO_ordinal)
                    vaccinazioneEsclusaCentrale.IdUtenteInserimentoVaccinazioneEsclusa = idr.GetInt64(EXC_UTE_ID_INSERIMENTO_ordinal)
                    vaccinazioneEsclusaCentrale.DataInserimentoVaccinazioneEsclusa = idr.GetDateTime(EXC_DATA_INSERIMENTO_ordinal)
                    vaccinazioneEsclusaCentrale.IdUtenteModificaVaccinazioneEsclusa = idr.GetNullableInt64OrDefault(EXC_UTE_ID_VARIAZIONE_ordinal)
                    vaccinazioneEsclusaCentrale.DataModificaVaccinazioneEsclusa = idr.GetNullableDateTimeOrDefault(EXC_DATA_VARIAZIONE_ordinal)
                    vaccinazioneEsclusaCentrale.IdUtenteEliminazioneVaccinazioneEsclusa = idr.GetNullableInt64OrDefault(EXC_UTE_ID_ELIMINAZIONE_ordinal)
                    vaccinazioneEsclusaCentrale.DataEliminazioneVaccinazioneEsclusa = idr.GetNullableDateTimeOrDefault(EXC_DATA_ELIMINAZIONE_ordinal)
                    vaccinazioneEsclusaCentrale.MergeInfoCentrale.CodicePazienteCentraleAlias = idr.GetStringOrDefault(EXC_PAZ_CODICE_ALIAS_CENTRALE_ordinal)
                    vaccinazioneEsclusaCentrale.MergeInfoCentrale.CodiceUslAlias = idr.GetStringOrDefault(EXC_USL_CODICE_ALIAS_ordinal)
                    vaccinazioneEsclusaCentrale.MergeInfoCentrale.IdUtenteAlias = idr.GetNullableInt64OrDefault(EXC_UTE_ID_ALIAS_ordinal)
                    vaccinazioneEsclusaCentrale.MergeInfoCentrale.DataAlias = idr.GetNullableDateTimeOrDefault(EXC_DATA_ALIAS_ordinal)
                    vaccinazioneEsclusaCentrale.IdConflitto = idr.GetNullableInt64OrDefault(EXC_ID_CONFLITTO_ordinal)
                    vaccinazioneEsclusaCentrale.DataRisoluzioneConflitto = idr.GetNullableDateTimeOrDefault(EXC_DATA_RISOLUZ_CONFLITTO_ordinal)
                    vaccinazioneEsclusaCentrale.IdUtenteRisoluzioneConflitto = idr.GetNullableInt64OrDefault(EXC_UTE_ID_RISOLUZ_CONFLITTO_ordinal)
                    vaccinazioneEsclusaCentrale.IdUtenteUltimaOperazione = idr.GetInt64OrDefault(EXC_UTE_ID_ULTIMA_OPERAZIONE_ordinal)

                    vaccinazioneEsclusaCentraleList.Add(vaccinazioneEsclusaCentrale)

                End While

            End If

            Return vaccinazioneEsclusaCentraleList

        End Function

        Private Function GetVaccinazioneEsclusaCentraleDistribuitaListFromReader(idr As OracleClient.OracleDataReader) As List(Of VaccinazioneEsclusaCentrale.VaccinazioneEsclusaDistribuita)

            Dim vaccinazioneEsclusaDistribuitaList As New List(Of VaccinazioneEsclusaCentrale.VaccinazioneEsclusaDistribuita)

            If idr.HasRows Then

                Dim exd_id_ordinal As Int16 = idr.GetOrdinal("EXD_ID")
                Dim exd_exc_id_ordinal As Int16 = idr.GetOrdinal("EXD_EXC_ID")
                Dim exd_vex_id_ordinal As Int16 = idr.GetOrdinal("EXD_VEX_ID")
                Dim exd_paz_codice_locale_ordinal As Int16 = idr.GetOrdinal("EXD_PAZ_CODICE_LOCALE")
                Dim exd_usl_codice_ordinal As Int16 = idr.GetOrdinal("EXD_USL_CODICE")
                Dim exd_ute_id_ins_locale_ordinal As Int16 = idr.GetOrdinal("EXD_UTE_ID_INS_LOCALE")
                Dim exd_data_ins_locale_ordinal As Int16 = idr.GetOrdinal("EXD_DATA_INS_LOCALE")
                Dim exd_data_agg_locale_ordinal As Int16 = idr.GetOrdinal("EXD_DATA_AGG_LOCALE")

                While idr.Read()

                    Dim vaccinazioneEsclusaDistribuita As New VaccinazioneEsclusaCentrale.VaccinazioneEsclusaDistribuita()

                    vaccinazioneEsclusaDistribuita.Id = idr.GetInt64(exd_id_ordinal)
                    vaccinazioneEsclusaDistribuita.IdVaccinazioneEsclusaCentrale = idr.GetInt64(exd_exc_id_ordinal)
                    vaccinazioneEsclusaDistribuita.IdVaccinazioneEsclusa = idr.GetInt64(exd_vex_id_ordinal)
                    vaccinazioneEsclusaDistribuita.CodicePaziente = idr.GetInt64(exd_paz_codice_locale_ordinal)
                    vaccinazioneEsclusaDistribuita.CodiceUslVaccinazioneEsclusa = idr.GetString(exd_usl_codice_ordinal)
                    vaccinazioneEsclusaDistribuita.IdUtenteInserimentoVaccinazioneEsclusa = idr.GetInt64(exd_ute_id_ins_locale_ordinal)
                    vaccinazioneEsclusaDistribuita.DataInserimentoVaccinazioneEsclusa = idr.GetDateTime(exd_data_ins_locale_ordinal)
                    vaccinazioneEsclusaDistribuita.DataAggiornamentoVaccinazioneEsclusa = idr.GetNullableDateTimeOrDefault(exd_data_agg_locale_ordinal)

                    vaccinazioneEsclusaDistribuitaList.Add(vaccinazioneEsclusaDistribuita)

                End While

            End If

            Return vaccinazioneEsclusaDistribuitaList

        End Function

        Private Sub AddVaccinazioneEsclusaCentraleInsertOrUpdateParameters(cmd As OracleClient.OracleCommand, vaccinazioneEsclusaCentrale As VaccinazioneEsclusaCentrale)
            cmd.Parameters.AddWithValue("EXC_ID", GetLongParam(vaccinazioneEsclusaCentrale.Id))
            cmd.Parameters.AddWithValue("EXC_TIPO", GetStringParam(vaccinazioneEsclusaCentrale.TipoVaccinazioneEsclusaCentrale))
            cmd.Parameters.AddWithValue("EXC_PAZ_CODICE_CENTRALE", GetStringParam(vaccinazioneEsclusaCentrale.CodicePazienteCentrale))
            cmd.Parameters.AddWithValue("EXC_PAZ_CODICE_LOCALE", GetLongParam(vaccinazioneEsclusaCentrale.CodicePaziente))
            cmd.Parameters.AddWithValue("EXC_VEX_ID", GetLongParam(vaccinazioneEsclusaCentrale.IdVaccinazioneEsclusa))
            cmd.Parameters.AddWithValue("EXC_VISIBILITA", GetStringParam(vaccinazioneEsclusaCentrale.FlagVisibilitaCentrale))
            cmd.Parameters.AddWithValue("EXC_DATA_REVOCA_VISIBILITA", GetDateParam(vaccinazioneEsclusaCentrale.DataRevocaVisibilita))
            cmd.Parameters.AddWithValue("EXC_USL_INSERIMENTO", GetStringParam(vaccinazioneEsclusaCentrale.CodiceUslVaccinazioneEsclusa))
            cmd.Parameters.AddWithValue("EXC_UTE_ID_INSERIMENTO", GetLongParam(vaccinazioneEsclusaCentrale.IdUtenteInserimentoVaccinazioneEsclusa))
            cmd.Parameters.AddWithValue("EXC_DATA_INSERIMENTO", GetDateParam(vaccinazioneEsclusaCentrale.DataInserimentoVaccinazioneEsclusa))
            cmd.Parameters.AddWithValue("EXC_UTE_ID_VARIAZIONE", GetLongParam(vaccinazioneEsclusaCentrale.IdUtenteModificaVaccinazioneEsclusa))
            cmd.Parameters.AddWithValue("EXC_DATA_VARIAZIONE", GetDateParam(vaccinazioneEsclusaCentrale.DataModificaVaccinazioneEsclusa))
            cmd.Parameters.AddWithValue("EXC_UTE_ID_ELIMINAZIONE", GetLongParam(vaccinazioneEsclusaCentrale.IdUtenteEliminazioneVaccinazioneEsclusa))
            cmd.Parameters.AddWithValue("EXC_DATA_ELIMINAZIONE", GetDateParam(vaccinazioneEsclusaCentrale.DataEliminazioneVaccinazioneEsclusa))
            cmd.Parameters.AddWithValue("EXC_PAZ_CODICE_ALIAS_CENTRALE", GetStringParam(vaccinazioneEsclusaCentrale.MergeInfoCentrale.CodicePazienteCentraleAlias))
            cmd.Parameters.AddWithValue("EXC_USL_CODICE_ALIAS", GetStringParam(vaccinazioneEsclusaCentrale.MergeInfoCentrale.CodiceUslAlias))
            cmd.Parameters.AddWithValue("EXC_UTE_ID_ALIAS", GetLongParam(vaccinazioneEsclusaCentrale.MergeInfoCentrale.IdUtenteAlias))
            cmd.Parameters.AddWithValue("EXC_DATA_ALIAS", GetDateParam(vaccinazioneEsclusaCentrale.MergeInfoCentrale.DataAlias))
            cmd.Parameters.AddWithValue("EXC_ID_CONFLITTO", GetLongParam(vaccinazioneEsclusaCentrale.IdConflitto))
            cmd.Parameters.AddWithValue("EXC_DATA_RISOLUZ_CONFLITTO", GetDateParam(vaccinazioneEsclusaCentrale.DataRisoluzioneConflitto))
            cmd.Parameters.AddWithValue("EXC_UTE_ID_RISOLUZ_CONFLITTO", GetLongParam(vaccinazioneEsclusaCentrale.IdUtenteRisoluzioneConflitto))
            cmd.Parameters.AddWithValue("EXC_UTE_ID_ULTIMA_OPERAZIONE", GetLongParam(vaccinazioneEsclusaCentrale.IdUtenteUltimaOperazione))
        End Sub

        Private Sub AddVaccinazioneEsclusaCentraleDistribuitaInsertOrUpdateParameters(cmd As OracleClient.OracleCommand, vaccinazioneEsclusaDistribuitaCentrale As VaccinazioneEsclusaCentrale.VaccinazioneEsclusaDistribuita)
            cmd.Parameters.AddWithValue("EXD_ID", GetLongParam(vaccinazioneEsclusaDistribuitaCentrale.Id))
            cmd.Parameters.AddWithValue("EXD_EXC_ID", GetLongParam(vaccinazioneEsclusaDistribuitaCentrale.IdVaccinazioneEsclusaCentrale))
            cmd.Parameters.AddWithValue("EXD_PAZ_CODICE_LOCALE", GetLongParam(vaccinazioneEsclusaDistribuitaCentrale.CodicePaziente))
            cmd.Parameters.AddWithValue("EXD_VEX_ID", GetLongParam(vaccinazioneEsclusaDistribuitaCentrale.IdVaccinazioneEsclusa))
            cmd.Parameters.AddWithValue("EXD_USL_CODICE", GetStringParam(vaccinazioneEsclusaDistribuitaCentrale.CodiceUslVaccinazioneEsclusa))
            cmd.Parameters.AddWithValue("EXD_UTE_ID_INS_LOCALE", GetLongParam(vaccinazioneEsclusaDistribuitaCentrale.IdUtenteInserimentoVaccinazioneEsclusa))
            cmd.Parameters.AddWithValue("EXD_DATA_INS_LOCALE", GetDateParam(vaccinazioneEsclusaDistribuitaCentrale.DataInserimentoVaccinazioneEsclusa))
            cmd.Parameters.AddWithValue("EXD_DATA_AGG_LOCALE", GetDateParam(vaccinazioneEsclusaDistribuitaCentrale.DataAggiornamentoVaccinazioneEsclusa))
        End Sub

        Private Function GetQueryRicercaConflittiMaster(cmd As OracleClient.OracleCommand, filtriRicercaConflitti As Filters.FiltriRicercaConflittiDatiVaccinali, countQuery As Boolean) As System.Text.StringBuilder

            Dim query As New System.Text.StringBuilder()

            If countQuery Then
                query.Append("SELECT count(EXC_ID) ")
            Else
                query.Append("SELECT PAZ_CODICE, PAZ_COGNOME, PAZ_NOME, PAZ_DATA_NASCITA, EXC_ID ")
            End If

            query.Append("FROM T_PAZ_PAZIENTI JOIN T_ESCLUSE_CENTRALE ON PAZ_CODICE = EXC_PAZ_CODICE_CENTRALE ")

            query.Append("WHERE NOT EXC_ID_CONFLITTO IS NULL ")
            query.Append("AND EXC_DATA_RISOLUZ_CONFLITTO IS NULL ")
            query.Append("AND EXC_ID_CONFLITTO = EXC_ID ")

            If Not filtriRicercaConflitti Is Nothing Then

                If Not String.IsNullOrEmpty(filtriRicercaConflitti.CognomePaziente) Then
                    query.Append("AND PAZ_COGNOME like :paz_cognome ")
                    cmd.Parameters.AddWithValue("paz_cognome", filtriRicercaConflitti.CognomePaziente + "%")
                End If

                If Not String.IsNullOrEmpty(filtriRicercaConflitti.NomePaziente) Then
                    query.Append("AND PAZ_NOME like :paz_nome ")
                    cmd.Parameters.AddWithValue("paz_nome", filtriRicercaConflitti.NomePaziente + "%")
                End If

                If filtriRicercaConflitti.DataNascitaMinima.HasValue Then
                    query.Append("AND PAZ_DATA_NASCITA >= :paz_data_nascita_minima ")
                    cmd.Parameters.AddWithValue("paz_data_nascita_minima", filtriRicercaConflitti.DataNascitaMinima.Value)
                End If

                If filtriRicercaConflitti.DataNascitaMassima.HasValue Then
                    query.Append("AND PAZ_DATA_NASCITA < :paz_data_nascita_massima ")
                    cmd.Parameters.AddWithValue("paz_data_nascita_massima", filtriRicercaConflitti.DataNascitaMassima.Value.AddDays(1))
                End If

            End If

            Return query

        End Function

        Private Function GetDatiVaccinazioniInConflitto(idVaccinazioneEsclusaCentrale As Int64) As List(Of Entities.ConflittoVaccinazioniEscluse.DatiEsclusioneInConflitto)

            Dim listDatiEsclusioniInConflitto As New List(Of Entities.ConflittoVaccinazioniEscluse.DatiEsclusioneInConflitto)()

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                Dim query As New System.Text.StringBuilder()

                query.Append("SELECT EXC_ID, EXC_ID_CONFLITTO, EXC_PAZ_CODICE_CENTRALE, EXC_PAZ_CODICE_LOCALE, EXC_VISIBILITA, EXC_TIPO, EXC_USL_INSERIMENTO, EXC_VEX_ID ")
                query.Append("FROM T_ESCLUSE_CENTRALE ")
                query.Append("WHERE EXC_ID_CONFLITTO = :exc_id_conflitto ")
                query.Append("AND EXC_DATA_RISOLUZ_CONFLITTO IS NULL ")
                query.Append("ORDER BY EXC_ID ")

                cmd.Parameters.AddWithValue("exc_id_conflitto", idVaccinazioneEsclusaCentrale)

                cmd.CommandText = query.ToString()

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim EXC_ID As Int16 = idr.GetOrdinal("EXC_ID")
                            Dim EXC_ID_CONFLITTO As Int16 = idr.GetOrdinal("EXC_ID_CONFLITTO")
                            Dim EXC_PAZ_CODICE_CENTRALE As Int16 = idr.GetOrdinal("EXC_PAZ_CODICE_CENTRALE")
                            Dim EXC_PAZ_CODICE_LOCALE As Int16 = idr.GetOrdinal("EXC_PAZ_CODICE_LOCALE")
                            Dim EXC_VISIBILITA As Int16 = idr.GetOrdinal("EXC_VISIBILITA")
                            Dim EXC_TIPO As Int16 = idr.GetOrdinal("EXC_TIPO")
                            Dim EXC_USL_INSERIMENTO As Int16 = idr.GetOrdinal("EXC_USL_INSERIMENTO")
                            Dim EXC_VEX_ID As Int16 = idr.GetOrdinal("EXC_VEX_ID")

                            While idr.Read()

                                Dim datiEsclusioniInConflitto As New Entities.ConflittoVaccinazioniEscluse.DatiEsclusioneInConflitto()

                                ' Dati che verranno reperiti in locale:
                                'datiEsclusioniInConflitto.CodiceMotivoEsclusione
                                'datiEsclusioniInConflitto.CodiceVaccinazione
                                'datiEsclusioniInConflitto.DataEsclusione
                                'datiEsclusioniInConflitto.DataScadenza
                                'datiEsclusioniInConflitto.DescrizioneMotivoEsclusione

                                datiEsclusioniInConflitto.CodicePazienteCentrale = idr.GetString(EXC_PAZ_CODICE_CENTRALE)
                                datiEsclusioniInConflitto.CodicePaziente = idr.GetInt64(EXC_PAZ_CODICE_LOCALE)
                                datiEsclusioniInConflitto.CodiceUslVaccinazioneEsclusa = idr.GetString(EXC_USL_INSERIMENTO)
                                datiEsclusioniInConflitto.FlagVisibilitaCentrale = idr.GetString(EXC_VISIBILITA)
                                datiEsclusioniInConflitto.Id = idr.GetInt64(EXC_ID)
                                datiEsclusioniInConflitto.IdVaccinazioneEsclusa = idr.GetInt64(EXC_VEX_ID)
                                datiEsclusioniInConflitto.TipoVaccinazioneEsclusaCentrale = idr.GetStringOrDefault(EXC_TIPO)

                                listDatiEsclusioniInConflitto.Add(datiEsclusioniInConflitto)

                            End While

                        End If

                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return listDatiEsclusioniInConflitto

        End Function

        Private Function GetFiltroVisibilita(listVisibilita As List(Of String), cmd As OracleClient.OracleCommand) As String

            If listVisibilita Is Nothing OrElse listVisibilita.Count = 0 Then Return String.Empty

            Dim query As New System.Text.StringBuilder()

            Dim filtroVisibilita As New System.Text.StringBuilder()

            For i As Int16 = 0 To listVisibilita.Count - 1

                Dim paramName As String = String.Format("p{0}", i)

                filtroVisibilita.AppendFormat(":{0},", paramName)

                cmd.Parameters.AddWithValue(paramName, listVisibilita(i))

            Next

            filtroVisibilita.Remove(filtroVisibilita.Length - 1, 1)

            query.AppendFormat(" AND EXC_VISIBILITA IN ({0}) ", filtroVisibilita)

            Return query.ToString()

        End Function

        Private Function GetFiltroEliminate(noEliminate As Boolean, cmd As OracleClient.OracleCommand) As String

            Dim query As String = String.Empty

            If noEliminate Then

                query = " AND (EXC_TIPO IS NULL OR EXC_TIPO <> :EXC_TIPO) "

                cmd.Parameters.AddWithValue("EXC_TIPO", Constants.TipoVaccinazioneEsclusaCentrale.Eliminata)

            End If

            Return query

        End Function

#End Region

    End Class

End Namespace
