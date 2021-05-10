Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.Data.OracleClient


Namespace DAL.Oracle

    Public Class DBVisitaCentraleProvider
        Inherits DbProvider
        Implements IVisitaCentraleProvider

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

#Region " IVisitaCentraleProvider "

#Region " Visite "

        Public Function GetVisitaCentraleById(idVisitaCentrale As Long) As Entities.VisitaCentrale Implements IVisitaCentraleProvider.GetVisitaCentraleById

            Dim visitaCentrale As Entities.VisitaCentrale

            Using cmd As New OracleClient.OracleCommand("SELECT * FROM T_VISITE_CENTRALE WHERE VIC_ID=:VIC_ID", Me.Connection)

                cmd.Parameters.AddWithValue("VIC_ID", GetLongParam(idVisitaCentrale))

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As OracleClient.OracleDataReader = cmd.ExecuteReader()

                        visitaCentrale = Me.GetVisitaCentraleListFromReader(idr).First()

                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return visitaCentrale

        End Function

        Public Function GetVisitaCentraleByUslInserimento(idVisita As Long, codiceUslInserimento As String) As Entities.VisitaCentrale Implements IVisitaCentraleProvider.GetVisitaCentraleByUslInserimento

            Dim visitaCentrale As Entities.VisitaCentrale

            Using cmd As New OracleClient.OracleCommand(Queries.Visite.OracleQueries.selVisitaCentraleByUslInserimento, Me.Connection)

                cmd.Parameters.AddWithValue("idVisita", GetLongParam(idVisita))
                cmd.Parameters.AddWithValue("codiceUslInserimento", GetStringParam(codiceUslInserimento))

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As OracleClient.OracleDataReader = cmd.ExecuteReader()

                        visitaCentrale = Me.GetVisitaCentraleListFromReader(idr).FirstOrDefault()

                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return visitaCentrale

        End Function

        Public Function GetVisitaCentraleByIdLocale(idVisitaLocale As Long, codiceUslLocale As String) As Entities.VisitaCentrale Implements IVisitaCentraleProvider.GetVisitaCentraleByIdLocale

            Dim visitaCentrale As Entities.VisitaCentrale = Nothing

            Using cmd As New OracleClient.OracleCommand(Queries.Visite.OracleQueries.selVisitaCentraleByIdLocale, Me.Connection)

                cmd.Parameters.AddWithValue("idVisita", GetLongParam(idVisitaLocale))
                cmd.Parameters.AddWithValue("codiceUsl", GetStringParam(codiceUslLocale))

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As OracleClient.OracleDataReader = cmd.ExecuteReader()

                        visitaCentrale = Me.GetVisitaCentraleListFromReader(idr).FirstOrDefault()

                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return visitaCentrale

        End Function

        Public Function GetVisitaCentraleDistribuitaByUsl(idVisita As Long, codiceUsl As String) As Entities.VisitaCentrale.VisitaDistribuita Implements IVisitaCentraleProvider.GetVisitaCentraleDistribuitaByUsl

            Dim visitaCentraleDistribuita As Entities.VisitaCentrale.VisitaDistribuita

            Using cmd As New OracleClient.OracleCommand(Queries.Visite.OracleQueries.selVisitaCentraleDistribuitaByUsl, Me.Connection)

                cmd.Parameters.AddWithValue("idVisita", GetLongParam(idVisita))
                cmd.Parameters.AddWithValue("codiceUsl", GetStringParam(codiceUsl))

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As OracleClient.OracleDataReader = cmd.ExecuteReader()

                        visitaCentraleDistribuita = Me.GetVisitaCentraleDistribuitaListFromReader(idr).FirstOrDefault()

                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return visitaCentraleDistribuita

        End Function

        Public Function GetVisitaCentraleDistribuitaByIdCentrale(idVisitaCentrale As Long, codiceUsl As String) As Entities.VisitaCentrale.VisitaDistribuita Implements IVisitaCentraleProvider.GetVisitaCentraleDistribuitaByIdCentrale

            Dim visitaDistribuita As Entities.VisitaCentrale.VisitaDistribuita

            Using cmd As New OracleClient.OracleCommand("SELECT * FROM T_VISITE_DISTRIBUITE WHERE VID_VIC_ID = :idVisitaCentrale AND VID_USL_CODICE = :codiceUsl", Me.Connection)

                cmd.Parameters.AddWithValue("idVisitaCentrale", GetLongParam(idVisitaCentrale))
                cmd.Parameters.AddWithValue("codiceUsl", GetStringParam(codiceUsl))

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As OracleClient.OracleDataReader = cmd.ExecuteReader()

                        visitaDistribuita = Me.GetVisitaCentraleDistribuitaListFromReader(idr).FirstOrDefault()

                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return visitaDistribuita

        End Function

        Public Function GetVisitaCentraleDistribuitaEnumerable(codicePazienteLocale As Int64, codiceUsl As String) As System.Collections.Generic.IEnumerable(Of Entities.VisitaCentrale.VisitaDistribuita) Implements IVisitaCentraleProvider.GetVisitaCentraleDistribuitaEnumerable

            Dim visitaCentraleDistribuitaList As List(Of Entities.VisitaCentrale.VisitaDistribuita)

            Using cmd As New OracleClient.OracleCommand("SELECT * FROM T_VISITE_DISTRIBUITE WHERE  VID_PAZ_CODICE_LOCALE = :codicePazienteLocale AND VID_USL_CODICE = :codiceUsl", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    cmd.Parameters.AddWithValue("codiceUsl", GetStringParam(codiceUsl))
                    cmd.Parameters.AddWithValue("codicePazienteLocale", GetLongParam(codicePazienteLocale))

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As OracleClient.OracleDataReader = cmd.ExecuteReader()

                        visitaCentraleDistribuitaList = Me.GetVisitaCentraleDistribuitaListFromReader(idr)

                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return visitaCentraleDistribuitaList.AsEnumerable()

        End Function

        Public Function GetVisitaCentraleEnumerable(codicePazienteCentrale As String) As System.Collections.Generic.IEnumerable(Of Entities.VisitaCentrale) Implements IVisitaCentraleProvider.GetVisitaCentraleEnumerable

            Return Me.GetVisitaCentraleEnumerable(codicePazienteCentrale, Nothing)

        End Function

        Public Function GetVisitaCentraleEnumerable(codicePazienteCentrale As String, flagVisibilita As String) As System.Collections.Generic.IEnumerable(Of Entities.VisitaCentrale) Implements IVisitaCentraleProvider.GetVisitaCentraleEnumerable

            Dim visitaCentraleList As List(Of Entities.VisitaCentrale)

            Using cmd As OracleClient.OracleCommand = New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                Dim query As String = "SELECT * FROM T_VISITE_CENTRALE WHERE VIC_PAZ_CODICE_CENTRALE = :codicePazienteCentrale"

                cmd.Parameters.AddWithValue("codicePazienteCentrale", GetStringParam(codicePazienteCentrale))

                If Not String.IsNullOrEmpty(flagVisibilita) Then
                    query += " AND VIC_VISIBILITA = :flagVisibilita"
                    cmd.Parameters.AddWithValue("flagVisibilita", flagVisibilita)
                End If

                cmd.CommandText = query

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As OracleClient.OracleDataReader = cmd.ExecuteReader()

                        visitaCentraleList = Me.GetVisitaCentraleListFromReader(idr)

                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return visitaCentraleList.AsEnumerable()

        End Function

        Public Function IsVisitaCentraleInConflitto(idVisitaCentrale As Int64) As Boolean

            Dim isConflitto As Boolean = False

            Using cmd As New OracleClient.OracleCommand("SELECT 1 FROM T_VISITE_CENTRALE WHERE VIC_ID = :VIC_ID AND VIC_ID_CONFLITTO IS NOT NULL AND VIC_DATA_RISOLUZ_CONFLITTO IS NULL", Me.Connection)

                cmd.Parameters.AddWithValue("VIC_ID", idVisitaCentrale)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    isConflitto = (Not cmd.ExecuteScalar() Is Nothing)

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return isConflitto

        End Function

        Public Function CountVisitaCentraleInConflittoByIdConflitto(idConflitto As Long) As Long Implements IVisitaCentraleProvider.CountVisitaCentraleInConflittoByIdConflitto

            Using cmd As OracleClient.OracleCommand = New OracleClient.OracleCommand("SELECT COUNT(*) FROM T_VISITE_CENTRALE WHERE VIC_ID_CONFLITTO = :VIC_ID_CONFLITTO AND VIC_DATA_RISOLUZ_CONFLITTO IS NULL", Me.Connection)

                cmd.Parameters.AddWithValue("VIC_ID_CONFLITTO", idConflitto)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Return cmd.ExecuteScalar()

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

        End Function

        Public Function CountVisitePazienteCentrale(codicePazienteCentrale As String, listVisibilita As List(Of String), noEliminate As Boolean) As Integer Implements IVisitaCentraleProvider.CountVisitePazienteCentrale

            Dim count As Integer = 0

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                Dim query As New System.Text.StringBuilder("SELECT COUNT(*) FROM T_VISITE_CENTRALE WHERE VIC_PAZ_CODICE_CENTRALE = :VIC_PAZ_CODICE_CENTRALE")

                cmd.Parameters.AddWithValue("VIC_PAZ_CODICE_CENTRALE", codicePazienteCentrale)

                ' Filtro visibilità
                query.Append(Me.GetFiltroVisibilita(listVisibilita, cmd))

                ' Filtro eliminate
                query.Append(Me.GetFiltroEliminate(noEliminate, cmd))

                cmd.CommandText = query.ToString()

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

        Public Function GetUslInserimentoVisite(codicePazienteCentrale As String, noEliminate As Boolean) As List(Of String) Implements IVisitaCentraleProvider.GetUslInserimentoVisite

            Dim listUslInserimento As New List(Of String)()

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                Dim query As New System.Text.StringBuilder("SELECT DISTINCT VIC_USL_INSERIMENTO FROM T_VISITE_CENTRALE ")

                ' Filtro paziente
                query.Append(" WHERE VIC_PAZ_CODICE_CENTRALE = :VIC_PAZ_CODICE_CENTRALE ")
                cmd.Parameters.AddWithValue("VIC_PAZ_CODICE_CENTRALE", codicePazienteCentrale)

                '' Filtro visibilità
                'query.Append(Me.GetFiltroVisibilita(listVisibilita, cmd))

                ' Filtro eliminate
                query.Append(Me.GetFiltroEliminate(noEliminate, cmd))

                cmd.CommandText = query.ToString()

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

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

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return listUslInserimento

        End Function

        ''' <summary>
        ''' Restituisce l'elenco delle usl gestite in cui sono state distribuite le visite, in base a id locali e usl inserimento
        ''' </summary>
        Public Function GetUslDistribuiteVisite(idVisiteLocali As List(Of Long), codiceUslInserimentoVisite As String) As List(Of Entities.UslDistribuitaDatoVaccinaleInfo) Implements IVisitaCentraleProvider.GetUslDistribuiteVisite

            Dim listDatiUslDistribuite As New List(Of Entities.UslDistribuitaDatoVaccinaleInfo)()

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                Dim query As String = "SELECT VIC_VIS_ID ID, VIC_USL_INSERIMENTO USL_INSERIMENTO, VID_USL_CODICE USL_DISTRIBUITA FROM T_VISITE_DISTRIBUITE JOIN T_VISITE_CENTRALE ON VID_VIC_ID = VIC_ID WHERE VIC_USL_INSERIMENTO = :USL_INSERIMENTO AND VIC_VIS_ID IN ({0}) AND VID_USL_CODICE <> VIC_USL_INSERIMENTO"

                cmd.Parameters.AddWithValue("USL_INSERIMENTO", codiceUslInserimentoVisite)

                Dim filtroId As New System.Text.StringBuilder()

                For i As Integer = 0 To idVisiteLocali.Count - 1

                    Dim parameterName As String = String.Format("p{0}", i)

                    filtroId.AppendFormat(":{0},", parameterName)

                    cmd.Parameters.AddWithValue(parameterName, idVisiteLocali(i))

                Next

                If filtroId.Length > 0 Then filtroId.Remove(filtroId.Length - 1, 1)

                cmd.CommandText = String.Format(query, filtroId)

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim id As Int16 = idr.GetOrdinal("ID")
                            Dim usl_inserimento As Int16 = idr.GetOrdinal("USL_INSERIMENTO")
                            Dim usl_distribuita As Int16 = idr.GetOrdinal("USL_DISTRIBUITA")

                            While idr.Read()

                                Dim datiUslDistribuita As New Entities.UslDistribuitaDatoVaccinaleInfo()

                                datiUslDistribuita.IdDatoVaccinale = idr.GetInt64(id)
                                datiUslDistribuita.CodiceUslInserimento = idr.GetString(usl_inserimento)
                                datiUslDistribuita.CodiceUslDistribuita = idr.GetString(usl_distribuita)

                                listDatiUslDistribuite.Add(datiUslDistribuita)

                            End While

                        End If

                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return listDatiUslDistribuite

        End Function

#Region " Conflitti "

        Public Function CountConflittiVisiteCentrale(filtriRicercaConflitti As Filters.FiltriRicercaConflittiDatiVaccinali) As Integer Implements IVisitaCentraleProvider.CountConflittiVisiteCentrale

            Dim count As Integer = 0

            Using cmd As OracleClient.OracleCommand = New OracleClient.OracleCommand()

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

        Public Function GetConflittiVisiteCentrale(filtriRicercaConflitti As Filters.FiltriRicercaConflittiDatiVaccinali, pagingOptions As OnAssistnet.Data.PagingOptions) As List(Of Entities.ConflittoVisite) Implements IVisitaCentraleProvider.GetConflittiVisiteCentrale

            Dim listConflittiVisite As New List(Of Entities.ConflittoVisite)()

            ' Ricerca vaccinazioni "principali" in conflitto
            Using cmd As OracleClient.OracleCommand = New OracleClient.OracleCommand()

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
                            Dim VIC_ID As Int16 = idr.GetOrdinal("VIC_ID")

                            While idr.Read()

                                Dim conflittoVisite As New Entities.ConflittoVisite()

                                conflittoVisite.CodicePazienteCentrale = idr.GetString(PAZ_CODICE)
                                conflittoVisite.Cognome = idr.GetStringOrDefault(PAZ_COGNOME)
                                conflittoVisite.Nome = idr.GetStringOrDefault(PAZ_NOME)
                                conflittoVisite.DataNascita = idr.GetNullableDateTimeOrDefault(PAZ_DATA_NASCITA)
                                conflittoVisite.IdVisitaCentrale = idr.GetInt64(VIC_ID)

                                listConflittiVisite.Add(conflittoVisite)

                            End While

                        End If

                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            ' Ricerca dati dei conflitti per ogni visita trovata
            For Each conflittoVisite As Entities.ConflittoVisite In listConflittiVisite

                conflittoVisite.VisiteInConflitto = Me.GetDatiVisiteInConflitto(conflittoVisite.IdVisitaCentrale)

            Next

            Return listConflittiVisite

        End Function

#End Region

#Region " Insert / Update / Delete "

        Public Sub InsertVisitaCentrale(visitaCentrale As Entities.VisitaCentrale) Implements IVisitaCentraleProvider.InsertVisitaCentrale

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try

                cmd = New OracleClient.OracleCommand("SELECT SEQ_VISITE_CENTRALE.NEXTVAL FROM DUAL", Me.Connection)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                visitaCentrale.Id = Convert.ToInt64(cmd.ExecuteScalar())

                cmd.CommandText = "INSERT INTO T_VISITE_CENTRALE(VIC_ID, VIC_TIPO, VIC_PAZ_CODICE_CENTRALE, VIC_PAZ_CODICE_LOCALE, VIC_VIS_ID, VIC_VISIBILITA, VIC_DATA_REVOCA_VISIBILITA, VIC_USL_INSERIMENTO, VIC_DATA_INSERIMENTO, VIC_UTE_ID_INSERIMENTO, VIC_DATA_VARIAZIONE, VIC_UTE_ID_VARIAZIONE, VIC_DATA_ELIMINAZIONE, VIC_UTE_ID_ELIMINAZIONE, VIC_PAZ_CODICE_ALIAS_CENTRALE, VIC_USL_CODICE_ALIAS, VIC_UTE_ID_ALIAS, VIC_DATA_ALIAS, VIC_ID_CONFLITTO, VIC_UTE_ID_RISOLUZ_CONFLITTO, VIC_DATA_RISOLUZ_CONFLITTO, VIC_UTE_ID_ULTIMA_OPERAZIONE) VALUES(:VIC_ID, :VIC_TIPO, :VIC_PAZ_CODICE_CENTRALE, :VIC_PAZ_CODICE_LOCALE, :VIC_VIS_ID, :VIC_VISIBILITA, :VIC_DATA_REVOCA_VISIBILITA, :VIC_USL_INSERIMENTO, :VIC_DATA_INSERIMENTO, :VIC_UTE_ID_INSERIMENTO, :VIC_DATA_VARIAZIONE, :VIC_UTE_ID_VARIAZIONE, :VIC_DATA_ELIMINAZIONE, :VIC_UTE_ID_ELIMINAZIONE, :VIC_PAZ_CODICE_ALIAS_CENTRALE, :VIC_USL_CODICE_ALIAS, :VIC_UTE_ID_ALIAS, :VIC_DATA_ALIAS, :VIC_ID_CONFLITTO, :VIC_UTE_ID_RISOLUZ_CONFLITTO, :VIC_DATA_RISOLUZ_CONFLITTO, :VIC_UTE_ID_ULTIMA_OPERAZIONE)"

                Me.AddVisitaCentraleInsertOrUpdateParameters(cmd, visitaCentrale)

                cmd.ExecuteNonQuery()

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

        End Sub

        Public Sub InsertVisitaCentraleDistribuita(visitaCentraleDistribuita As Entities.VisitaCentrale.VisitaDistribuita) Implements IVisitaCentraleProvider.InsertVisitaCentraleDistribuita

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try

                cmd = New OracleClient.OracleCommand("SELECT SEQ_VISITE_DISTRIBUITE.NEXTVAL FROM DUAL", Me.Connection)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                visitaCentraleDistribuita.Id = Convert.ToInt64(cmd.ExecuteScalar())

                cmd.CommandText = "INSERT INTO T_VISITE_DISTRIBUITE(VID_ID, VID_VIC_ID, VID_PAZ_CODICE_LOCALE, VID_VIS_ID, VID_USL_CODICE, VID_DATA_INS_LOCALE, VID_UTE_ID_INS_LOCALE, VID_DATA_AGG_LOCALE) VALUES (:VID_ID, :VID_VIC_ID, :VID_PAZ_CODICE_LOCALE, :VID_VIS_ID, :VID_USL_CODICE, :VID_DATA_INS_LOCALE, :VID_UTE_ID_INS_LOCALE, :VID_DATA_AGG_LOCALE)"

                Me.AddVisitaCentraleDistribuitaInsertOrUpdateParameters(cmd, visitaCentraleDistribuita)

                cmd.ExecuteNonQuery()

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

        End Sub

        Public Sub UpdateVisitaCentrale(visitaCentrale As Entities.VisitaCentrale) Implements IVisitaCentraleProvider.UpdateVisitaCentrale

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try

                cmd = New OracleClient.OracleCommand("UPDATE T_VISITE_CENTRALE SET VIC_TIPO=:VIC_TIPO, VIC_PAZ_CODICE_CENTRALE=:VIC_PAZ_CODICE_CENTRALE, VIC_PAZ_CODICE_LOCALE=:VIC_PAZ_CODICE_LOCALE, VIC_VIS_ID=:VIC_VIS_ID, VIC_VISIBILITA=:VIC_VISIBILITA, VIC_DATA_REVOCA_VISIBILITA=:VIC_DATA_REVOCA_VISIBILITA, VIC_USL_INSERIMENTO=:VIC_USL_INSERIMENTO,  VIC_DATA_INSERIMENTO=:VIC_DATA_INSERIMENTO, VIC_UTE_ID_INSERIMENTO=:VIC_UTE_ID_INSERIMENTO, VIC_DATA_VARIAZIONE=:VIC_DATA_VARIAZIONE, VIC_UTE_ID_VARIAZIONE=:VIC_UTE_ID_VARIAZIONE, VIC_DATA_ELIMINAZIONE=:VIC_DATA_ELIMINAZIONE, VIC_UTE_ID_ELIMINAZIONE=:VIC_UTE_ID_ELIMINAZIONE, VIC_PAZ_CODICE_ALIAS_CENTRALE=:VIC_PAZ_CODICE_ALIAS_CENTRALE, VIC_USL_CODICE_ALIAS=:VIC_USL_CODICE_ALIAS, VIC_UTE_ID_ALIAS=:VIC_UTE_ID_ALIAS, VIC_DATA_ALIAS=:VIC_DATA_ALIAS, VIC_ID_CONFLITTO=:VIC_ID_CONFLITTO, VIC_UTE_ID_RISOLUZ_CONFLITTO=:VIC_UTE_ID_RISOLUZ_CONFLITTO,VIC_DATA_RISOLUZ_CONFLITTO=:VIC_DATA_RISOLUZ_CONFLITTO, VIC_UTE_ID_ULTIMA_OPERAZIONE=:VIC_UTE_ID_ULTIMA_OPERAZIONE WHERE VIC_ID=:VIC_ID", Me.Connection)

                Me.AddVisitaCentraleInsertOrUpdateParameters(cmd, visitaCentrale)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                cmd.ExecuteNonQuery()

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

        End Sub

        Public Sub UpdateVisitaCentraleDistribuita(visitaCentraleDistribuita As Entities.VisitaCentrale.VisitaDistribuita) Implements IVisitaCentraleProvider.UpdateVisitaCentraleDistribuita

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try

                cmd = New OracleClient.OracleCommand("UPDATE T_VISITE_DISTRIBUITE SET VID_VIC_ID=:VID_VIC_ID, VID_PAZ_CODICE_LOCALE=:VID_PAZ_CODICE_LOCALE, VID_VIS_ID=:VID_VIS_ID, VID_USL_CODICE=:VID_USL_CODICE, VID_DATA_INS_LOCALE=:VID_DATA_INS_LOCALE, VID_UTE_ID_INS_LOCALE=:VID_UTE_ID_INS_LOCALE, VID_DATA_AGG_LOCALE=:VID_DATA_AGG_LOCALE WHERE VID_ID=:VID_ID", Me.Connection)

                Me.AddVisitaCentraleDistribuitaInsertOrUpdateParameters(cmd, visitaCentraleDistribuita)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                cmd.ExecuteNonQuery()

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

        End Sub

        Public Sub UpdateIdConflittoVisitaCentraleByIdConflitto(idConflittoCorrente As Long, idConflittoAggiornato As Long?) Implements IVisitaCentraleProvider.UpdateIdConflittoVisitaCentraleByIdConflitto

            Using cmd As OracleClient.OracleCommand = New OracleClient.OracleCommand("UPDATE T_VISITE_CENTRALE SET VIC_ID_CONFLITTO = :VIC_ID_CONFLITTO_AGG WHERE VIC_ID_CONFLITTO = :VIC_ID_CONFLITTO_CORR", Me.Connection)

                cmd.Parameters.AddWithValue("VIC_ID_CONFLITTO_CORR", idConflittoCorrente)
                cmd.Parameters.AddWithValue("VIC_ID_CONFLITTO_AGG", IIf(idConflittoAggiornato.HasValue, idConflittoAggiornato, DBNull.Value))

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.ExecuteNonQuery()

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

        End Sub

        Public Sub DeleteVisitaCentraleDistribuita(visitaCentraleDistribuita As Entities.VisitaCentrale.VisitaDistribuita) Implements IVisitaCentraleProvider.DeleteVisitaCentraleDistribuita

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try

                cmd = New OracleClient.OracleCommand("DELETE FROM T_VISITE_DISTRIBUITE WHERE VID_ID=:VID_ID", Me.Connection)

                cmd.Parameters.AddWithValue("VID_ID", visitaCentraleDistribuita.Id)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                cmd.ExecuteNonQuery()

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

        End Sub

#End Region

#End Region

#End Region

#Region " Private "

        Private Function GetVisitaCentraleListFromReader(idr As OracleClient.OracleDataReader) As List(Of Entities.VisitaCentrale)

            Dim visitaCentraleList As New List(Of Entities.VisitaCentrale)

            If idr.HasRows Then

                Dim VIC_ID_ordinal As Int16 = idr.GetOrdinal("VIC_ID")
                Dim VIC_TIPO_ordinal As Int16 = idr.GetOrdinal("VIC_TIPO")
                Dim VIC_VIS_ID_ordinal As Int16 = idr.GetOrdinal("VIC_VIS_ID")
                Dim VIC_PAZ_CODICE_CENTRALE_ordinal As Int16 = idr.GetOrdinal("VIC_PAZ_CODICE_CENTRALE")
                Dim VIC_PAZ_CODICE_LOCALE_ordinal As Int16 = idr.GetOrdinal("VIC_PAZ_CODICE_LOCALE")
                Dim VIC_VISIBILITA_ordinal As Int16 = idr.GetOrdinal("VIC_VISIBILITA")
                Dim VIC_DATA_REVOCA_VISIBILITA_ordinal As Int16 = idr.GetOrdinal("VIC_DATA_REVOCA_VISIBILITA")
                Dim VIC_USL_INSERIMENTO_ordinal As Int16 = idr.GetOrdinal("VIC_USL_INSERIMENTO")
                Dim VIC_DATA_INSERIMENTO_ordinal As Int16 = idr.GetOrdinal("VIC_DATA_INSERIMENTO")
                Dim VIC_UTE_ID_INSERIMENTO_ordinal As Int16 = idr.GetOrdinal("VIC_UTE_ID_INSERIMENTO")
                Dim VIC_DATA_VARIAZIONE_ordinal As Int16 = idr.GetOrdinal("VIC_DATA_VARIAZIONE")
                Dim VIC_UTE_ID_VARIAZIONE_ordinal As Int16 = idr.GetOrdinal("VIC_UTE_ID_VARIAZIONE")
                Dim VIC_DATA_ELIMINAZIONE_ordinal As Int16 = idr.GetOrdinal("VIC_DATA_ELIMINAZIONE")
                Dim VIC_UTE_ID_ELIMINAZIONE_ordinal As Int16 = idr.GetOrdinal("VIC_UTE_ID_ELIMINAZIONE")
                Dim VIC_PAZ_CODICE_ALIAS_CENTRALE_ordinal As Int16 = idr.GetOrdinal("VIC_PAZ_CODICE_ALIAS_CENTRALE")
                Dim VIC_USL_CODICE_ALIAS_ordinal As Int16 = idr.GetOrdinal("VIC_USL_CODICE_ALIAS")
                Dim VIC_UTE_ID_ALIAS_ordinal As Int16 = idr.GetOrdinal("VIC_UTE_ID_ALIAS")
                Dim VIC_DATA_ALIAS_ordinal As Int16 = idr.GetOrdinal("VIC_DATA_ALIAS")
                Dim VIC_ID_CONFLITTO_ordinal As Int16 = idr.GetOrdinal("VIC_ID_CONFLITTO")
                Dim VIC_DATA_RISOLUZ_CONFLITTO_ordinal As Int16 = idr.GetOrdinal("VIC_DATA_RISOLUZ_CONFLITTO")
                Dim VIC_UTE_ID_RISOLUZ_CONFLITTO_ordinal As Int16 = idr.GetOrdinal("VIC_UTE_ID_RISOLUZ_CONFLITTO")
                Dim VIC_UTE_ID_ULTIMA_OPERAZIONE_ordinal As Int16 = idr.GetOrdinal("VIC_UTE_ID_ULTIMA_OPERAZIONE")

                While idr.Read()

                    Dim visitaCentrale As New VisitaCentrale()

                    visitaCentrale.Id = idr.GetInt64(VIC_ID_ordinal)
                    visitaCentrale.TipoVisitaCentrale = idr.GetStringOrDefault(VIC_TIPO_ordinal)
                    visitaCentrale.IdVisita = idr.GetInt64(VIC_VIS_ID_ordinal)
                    visitaCentrale.CodicePazienteCentrale = idr.GetString(VIC_PAZ_CODICE_CENTRALE_ordinal)
                    visitaCentrale.CodicePaziente = idr.GetInt64(VIC_PAZ_CODICE_LOCALE_ordinal)
                    visitaCentrale.FlagVisibilitaCentrale = idr.GetString(VIC_VISIBILITA_ordinal)
                    visitaCentrale.DataRevocaVisibilita = idr.GetNullableDateTimeOrDefault(VIC_DATA_REVOCA_VISIBILITA_ordinal)
                    visitaCentrale.CodiceUslVisita = idr.GetString(VIC_USL_INSERIMENTO_ordinal)
                    visitaCentrale.DataInserimentoVisita = idr.GetDateTime(VIC_DATA_INSERIMENTO_ordinal)
                    visitaCentrale.IdUtenteInserimentoVisita = idr.GetInt64(VIC_UTE_ID_INSERIMENTO_ordinal)
                    visitaCentrale.DataModificaVisita = idr.GetNullableDateTimeOrDefault(VIC_DATA_VARIAZIONE_ordinal)
                    visitaCentrale.IdUtenteModificaVisita = idr.GetNullableInt64OrDefault(VIC_UTE_ID_VARIAZIONE_ordinal)
                    visitaCentrale.DataEliminazioneVisita = idr.GetNullableDateTimeOrDefault(VIC_DATA_ELIMINAZIONE_ordinal)
                    visitaCentrale.IdUtenteEliminazioneVisita = idr.GetNullableInt64OrDefault(VIC_UTE_ID_ELIMINAZIONE_ordinal)
                    visitaCentrale.MergeInfoCentrale.CodicePazienteCentraleAlias = idr.GetStringOrDefault(VIC_PAZ_CODICE_ALIAS_CENTRALE_ordinal)
                    visitaCentrale.MergeInfoCentrale.CodiceUslAlias = idr.GetStringOrDefault(VIC_USL_CODICE_ALIAS_ordinal)
                    visitaCentrale.MergeInfoCentrale.IdUtenteAlias = idr.GetNullableInt64OrDefault(VIC_UTE_ID_ALIAS_ordinal)
                    visitaCentrale.MergeInfoCentrale.DataAlias = idr.GetNullableDateTimeOrDefault(VIC_DATA_ALIAS_ordinal)
                    visitaCentrale.IdConflitto = idr.GetNullableInt64OrDefault(VIC_ID_CONFLITTO_ordinal)
                    visitaCentrale.DataRisoluzioneConflitto = idr.GetNullableDateTimeOrDefault(VIC_DATA_RISOLUZ_CONFLITTO_ordinal)
                    visitaCentrale.IdUtenteRisoluzioneConflitto = idr.GetNullableInt64OrDefault(VIC_UTE_ID_RISOLUZ_CONFLITTO_ordinal)
                    visitaCentrale.IdUtenteUltimaOperazione = idr.GetInt64OrDefault(VIC_UTE_ID_ULTIMA_OPERAZIONE_ordinal)

                    visitaCentraleList.Add(visitaCentrale)

                End While

            End If

            Return visitaCentraleList

        End Function

        Private Function GetVisitaCentraleDistribuitaListFromReader(idr As OracleClient.OracleDataReader) As List(Of Entities.VisitaCentrale.VisitaDistribuita)

            Dim visitaCentraleDistribuitaList As New List(Of Entities.VisitaCentrale.VisitaDistribuita)

            If idr.HasRows Then

                Dim VID_ID_ordinal As Int16 = idr.GetOrdinal("VID_ID")
                Dim VID_PAZ_CODICE_LOCALE_ordinal As Int16 = idr.GetOrdinal("VID_PAZ_CODICE_LOCALE")
                Dim VID_VIC_ID_ordinal As Int16 = idr.GetOrdinal("VID_VIC_ID")
                Dim VID_VIS_ID_ordinal As Int16 = idr.GetOrdinal("VID_VIS_ID")
                Dim VID_USL_CODICE_ordinal As Int16 = idr.GetOrdinal("VID_USL_CODICE")
                Dim VID_DATA_INS_LOCALE_ordinal As Int16 = idr.GetOrdinal("VID_DATA_INS_LOCALE")
                Dim VID_UTE_ID_INS_LOCALE_ordinal As Int16 = idr.GetOrdinal("VID_UTE_ID_INS_LOCALE")
                Dim VID_DATA_AGG_LOCALE_ordinal As Int16 = idr.GetOrdinal("VID_DATA_AGG_LOCALE")

                While idr.Read()

                    Dim visitaCentraleDistribuita As New VisitaCentrale.VisitaDistribuita()

                    visitaCentraleDistribuita.Id = idr.GetInt64(VID_ID_ordinal)
                    visitaCentraleDistribuita.IdVisitaCentrale = idr.GetInt64(VID_VIC_ID_ordinal)
                    visitaCentraleDistribuita.CodicePaziente = idr.GetInt64(VID_PAZ_CODICE_LOCALE_ordinal)
                    visitaCentraleDistribuita.IdVisita = idr.GetInt64(VID_VIS_ID_ordinal)
                    visitaCentraleDistribuita.CodiceUslVisita = idr.GetString(VID_USL_CODICE_ordinal)
                    visitaCentraleDistribuita.DataInserimentoVisita = idr.GetDateTime(VID_DATA_INS_LOCALE_ordinal)
                    visitaCentraleDistribuita.IdUtenteInserimentoVisita = idr.GetInt64(VID_UTE_ID_INS_LOCALE_ordinal)
                    visitaCentraleDistribuita.DataAggiornamentoVisita = idr.GetNullableDateTimeOrDefault(VID_DATA_AGG_LOCALE_ordinal)

                    visitaCentraleDistribuitaList.Add(visitaCentraleDistribuita)

                End While

            End If

            Return visitaCentraleDistribuitaList

        End Function

        Private Sub AddVisitaCentraleInsertOrUpdateParameters(cmd As OracleClient.OracleCommand, visitaCentrale As VisitaCentrale)

            cmd.Parameters.AddWithValue("VIC_ID", GetLongParam(visitaCentrale.Id))
            cmd.Parameters.AddWithValue("VIC_TIPO", GetStringParam(visitaCentrale.TipoVisitaCentrale))
            cmd.Parameters.AddWithValue("VIC_VIS_ID", GetLongParam(visitaCentrale.IdVisita))
            cmd.Parameters.AddWithValue("VIC_PAZ_CODICE_CENTRALE", GetStringParam(visitaCentrale.CodicePazienteCentrale))
            cmd.Parameters.AddWithValue("VIC_PAZ_CODICE_LOCALE", GetLongParam(visitaCentrale.CodicePaziente))
            cmd.Parameters.AddWithValue("VIC_VISIBILITA", GetStringParam(visitaCentrale.FlagVisibilitaCentrale))
            cmd.Parameters.AddWithValue("VIC_DATA_REVOCA_VISIBILITA", GetDateParam(visitaCentrale.DataRevocaVisibilita))
            cmd.Parameters.AddWithValue("VIC_USL_INSERIMENTO", GetStringParam(visitaCentrale.CodiceUslVisita))
            cmd.Parameters.AddWithValue("VIC_DATA_INSERIMENTO", GetDateParam(visitaCentrale.DataInserimentoVisita))
            cmd.Parameters.AddWithValue("VIC_UTE_ID_INSERIMENTO", GetLongParam(visitaCentrale.IdUtenteInserimentoVisita))
            cmd.Parameters.AddWithValue("VIC_DATA_VARIAZIONE", GetDateParam(visitaCentrale.DataModificaVisita))
            cmd.Parameters.AddWithValue("VIC_UTE_ID_VARIAZIONE", GetLongParam(visitaCentrale.IdUtenteModificaVisita))
            cmd.Parameters.AddWithValue("VIC_DATA_ELIMINAZIONE", GetDateParam(visitaCentrale.DataEliminazioneVisita))
            cmd.Parameters.AddWithValue("VIC_UTE_ID_ELIMINAZIONE", GetLongParam(visitaCentrale.IdUtenteEliminazioneVisita))
            cmd.Parameters.AddWithValue("VIC_PAZ_CODICE_ALIAS_CENTRALE", GetStringParam(visitaCentrale.MergeInfoCentrale.CodicePazienteCentraleAlias))
            cmd.Parameters.AddWithValue("VIC_USL_CODICE_ALIAS", GetStringParam(visitaCentrale.MergeInfoCentrale.CodiceUslAlias))
            cmd.Parameters.AddWithValue("VIC_UTE_ID_ALIAS", GetLongParam(visitaCentrale.MergeInfoCentrale.IdUtenteAlias))
            cmd.Parameters.AddWithValue("VIC_DATA_ALIAS", GetDateParam(visitaCentrale.MergeInfoCentrale.DataAlias))
            cmd.Parameters.AddWithValue("VIC_ID_CONFLITTO", GetLongParam(visitaCentrale.IdConflitto))
            cmd.Parameters.AddWithValue("VIC_DATA_RISOLUZ_CONFLITTO", GetDateParam(visitaCentrale.DataRisoluzioneConflitto))
            cmd.Parameters.AddWithValue("VIC_UTE_ID_RISOLUZ_CONFLITTO", GetLongParam(visitaCentrale.IdUtenteRisoluzioneConflitto))
            cmd.Parameters.AddWithValue("VIC_UTE_ID_ULTIMA_OPERAZIONE", GetLongParam(visitaCentrale.IdUtenteUltimaOperazione))

        End Sub

        Private Sub AddVisitaCentraleDistribuitaInsertOrUpdateParameters(cmd As OracleClient.OracleCommand, visitaCentraleDistribuita As VisitaCentrale.VisitaDistribuita)

            cmd.Parameters.AddWithValue("VID_ID", GetLongParam(visitaCentraleDistribuita.Id))
            cmd.Parameters.AddWithValue("VID_VIC_ID", GetLongParam(visitaCentraleDistribuita.IdVisitaCentrale))
            cmd.Parameters.AddWithValue("VID_PAZ_CODICE_LOCALE", GetLongParam(visitaCentraleDistribuita.CodicePaziente))
            cmd.Parameters.AddWithValue("VID_VIS_ID", GetLongParam(visitaCentraleDistribuita.IdVisita))
            cmd.Parameters.AddWithValue("VID_USL_CODICE", GetStringParam(visitaCentraleDistribuita.CodiceUslVisita))
            cmd.Parameters.AddWithValue("VID_DATA_INS_LOCALE", GetDateParam(visitaCentraleDistribuita.DataInserimentoVisita))
            cmd.Parameters.AddWithValue("VID_UTE_ID_INS_LOCALE", GetLongParam(visitaCentraleDistribuita.IdUtenteInserimentoVisita))
            cmd.Parameters.AddWithValue("VID_DATA_AGG_LOCALE", GetDateParam(visitaCentraleDistribuita.DataAggiornamentoVisita))

        End Sub

        Private Function GetQueryRicercaConflittiMaster(cmd As OracleClient.OracleCommand, filtriRicercaConflitti As Filters.FiltriRicercaConflittiDatiVaccinali, countQuery As Boolean) As System.Text.StringBuilder

            Dim query As New System.Text.StringBuilder()

            If countQuery Then
                query.Append("SELECT count(VIC_ID) ")
            Else
                query.Append("SELECT PAZ_CODICE, PAZ_COGNOME, PAZ_NOME, PAZ_DATA_NASCITA, VIC_ID ")
            End If

            query.Append("FROM T_PAZ_PAZIENTI JOIN T_VISITE_CENTRALE ON PAZ_CODICE = VIC_PAZ_CODICE_CENTRALE ")

            query.Append("WHERE NOT VIC_ID_CONFLITTO IS NULL ")
            query.Append("AND VIC_DATA_RISOLUZ_CONFLITTO IS NULL ")
            query.Append("AND VIC_ID_CONFLITTO = VIC_ID ")

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

        Private Function GetDatiVisiteInConflitto(idVisitaCentrale As Int64) As List(Of Entities.ConflittoVisite.DatiVisitaInConflitto)

            Dim listDatiVisiteInConflitto As New List(Of Entities.ConflittoVisite.DatiVisitaInConflitto)()

            Using cmd As OracleClient.OracleCommand = New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                Dim query As New System.Text.StringBuilder()

                query.Append("SELECT VIC_ID, VIC_ID_CONFLITTO, VIC_PAZ_CODICE_CENTRALE, VIC_PAZ_CODICE_LOCALE, VIC_VISIBILITA, VIC_TIPO, VIC_USL_INSERIMENTO, VIC_VIS_ID ")
                query.Append("FROM T_VISITE_CENTRALE ")
                query.Append("WHERE VIC_ID_CONFLITTO = :vic_id_conflitto ")
                query.Append("AND VIC_DATA_RISOLUZ_CONFLITTO IS NULL ")
                query.Append("ORDER BY VIC_ID ")

                cmd.Parameters.AddWithValue("vic_id_conflitto", idVisitaCentrale)

                cmd.CommandText = query.ToString()

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim VIC_ID As Int16 = idr.GetOrdinal("VIC_ID")
                            Dim VIC_ID_CONFLITTO As Int16 = idr.GetOrdinal("VIC_ID_CONFLITTO")
                            Dim VIC_PAZ_CODICE_CENTRALE As Int16 = idr.GetOrdinal("VIC_PAZ_CODICE_CENTRALE")
                            Dim VIC_PAZ_CODICE_LOCALE As Int16 = idr.GetOrdinal("VIC_PAZ_CODICE_LOCALE")
                            Dim VIC_VISIBILITA As Int16 = idr.GetOrdinal("VIC_VISIBILITA")
                            Dim VIC_TIPO As Int16 = idr.GetOrdinal("VIC_TIPO")
                            Dim VIC_USL_INSERIMENTO As Int16 = idr.GetOrdinal("VIC_USL_INSERIMENTO")
                            Dim VIC_VIS_ID As Int16 = idr.GetOrdinal("VIC_VIS_ID")

                            While idr.Read()

                                Dim datiVisiteInConflitto As New Entities.ConflittoVisite.DatiVisitaInConflitto()

                                ' Dati che verranno reperiti in locale:
                                'datiVisiteInConflitto.CodiceMalattia
                                'datiVisiteInConflitto.DataFineSospensione
                                'datiVisiteInConflitto.DataVisita
                                'datiVisiteInConflitto.DescrizioneBilancio
                                'datiVisiteInConflitto.DescrizioneMalattia
                                'datiVisiteInConflitto.NumeroBilancio

                                datiVisiteInConflitto.CodicePazienteCentrale = idr.GetString(VIC_PAZ_CODICE_CENTRALE)
                                datiVisiteInConflitto.CodicePaziente = idr.GetInt64(VIC_PAZ_CODICE_LOCALE)
                                datiVisiteInConflitto.CodiceUslVisita = idr.GetString(VIC_USL_INSERIMENTO)
                                datiVisiteInConflitto.FlagVisibilitaCentrale = idr.GetString(VIC_VISIBILITA)
                                datiVisiteInConflitto.Id = idr.GetInt64(VIC_ID)
                                datiVisiteInConflitto.IdVisita = idr.GetInt64(VIC_VIS_ID)
                                datiVisiteInConflitto.TipoVisitaCentrale = idr.GetStringOrDefault(VIC_TIPO)

                                listDatiVisiteInConflitto.Add(datiVisiteInConflitto)

                            End While

                        End If

                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return listDatiVisiteInConflitto

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

            query.AppendFormat(" AND VIC_VISIBILITA IN ({0}) ", filtroVisibilita)

            Return query.ToString()

        End Function

        Private Function GetFiltroEliminate(noEliminate As Boolean, cmd As OracleClient.OracleCommand) As String

            Dim query As String = String.Empty

            If noEliminate Then

                query = " AND (VIC_TIPO IS NULL OR VIC_TIPO <> :VIC_TIPO) "

                cmd.Parameters.AddWithValue("VIC_TIPO", Constants.TipoVisitaCentrale.Eliminata)

            End If

            Return query

        End Function

#End Region

    End Class

End Namespace
