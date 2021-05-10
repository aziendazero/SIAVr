Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.Data.OracleClient

Namespace DAL.Oracle

    Public Class DBVaccinazioneEseguitaCentraleProvider
        Inherits DbProvider
        Implements IVaccinazioneEseguitaCentraleProvider

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

#Region " IVaccinazioniEseguiteCentraleProvider "

#Region " Get VaccinazioneEseguitaCentrale "

        Public Function GetVaccinazioneEseguitaCentraleById(idVaccinazioneEseguitaCentrale As Long) As Entities.VaccinazioneEseguitaCentrale Implements IVaccinazioneEseguitaCentraleProvider.GetVaccinazioneEseguitaCentraleById

            Dim vaccinazioneEseguitaCentrale As VaccinazioneEseguitaCentrale

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try
                cmd = New OracleClient.OracleCommand("SELECT * FROM T_VACCINAZIONI_CENTRALE WHERE VCC_ID = :VCC_ID", Me.Connection)

                cmd.Parameters.AddWithValue("VCC_ID", idVaccinazioneEseguitaCentrale)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using idr As OracleClient.OracleDataReader = cmd.ExecuteReader()

                    vaccinazioneEseguitaCentrale = Me.GetVaccinazioneEseguitaCentraleListFromReader(idr).First()

                End Using

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return vaccinazioneEseguitaCentrale

        End Function

        Public Function GetVaccinazioneEseguitaCentraleByUslInserimento(idVaccinazioneEseguita As Long, codiceUslInserimento As String) As Entities.VaccinazioneEseguitaCentrale Implements IVaccinazioneEseguitaCentraleProvider.GetVaccinazioneEseguitaCentraleByUslInserimento

            Dim vaccinazioneEseguitaCentrale As VaccinazioneEseguitaCentrale

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try
                cmd = New OracleClient.OracleCommand("SELECT * FROM T_VACCINAZIONI_CENTRALE WHERE VCC_VES_ID = :idVaccinazioneEseguita AND VCC_USL_INSERIMENTO = :codiceUslInserimento", Me.Connection)

                cmd.Parameters.AddWithValue("idVaccinazioneEseguita", idVaccinazioneEseguita)
                cmd.Parameters.AddWithValue("codiceUslInserimento", codiceUslInserimento)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using idr As OracleClient.OracleDataReader = cmd.ExecuteReader()

                    vaccinazioneEseguitaCentrale = Me.GetVaccinazioneEseguitaCentraleListFromReader(idr).FirstOrDefault()

                End Using

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return vaccinazioneEseguitaCentrale

        End Function

        Public Function GetVaccinazioneEseguitaCentraleByUslInserimentoReazioneAvversa(idReazioneAvversa As Long, codiceUslInserimentoReazioneAvversa As String) As Entities.VaccinazioneEseguitaCentrale Implements IVaccinazioneEseguitaCentraleProvider.GetVaccinazioneEseguitaCentraleByUslInserimentoReazioneAvversa

            Dim vaccinazioneEseguitaCentrale As VaccinazioneEseguitaCentrale

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try
                cmd = New OracleClient.OracleCommand("SELECT * FROM T_VACCINAZIONI_CENTRALE WHERE VCC_VRA_ID = :idReazioneAvversa AND VCC_USL_INSERIMENTO_REA = :codiceUslInserimento", Me.Connection)

                cmd.Parameters.AddWithValue("idReazioneAvversa", idReazioneAvversa)
                cmd.Parameters.AddWithValue("codiceUslInserimento", codiceUslInserimentoReazioneAvversa)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using idr As OracleClient.OracleDataReader = cmd.ExecuteReader()
                    vaccinazioneEseguitaCentrale = Me.GetVaccinazioneEseguitaCentraleListFromReader(idr).First()
                End Using

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return vaccinazioneEseguitaCentrale

        End Function

        Public Function GetVaccinazioneEseguitaCentraleByIdLocale(idVaccinazioneEseguitaLocale As Long, codiceUslLocale As String) As Entities.VaccinazioneEseguitaCentrale Implements IVaccinazioneEseguitaCentraleProvider.GetVaccinazioneEseguitaCentraleByIdLocale

            Dim vaccinazioneEseguitaCentrale As Entities.VaccinazioneEseguitaCentrale = Nothing

            Using cmd As New OracleClient.OracleCommand(Queries.VaccinazioniEseguite.OracleQueries.selVaccinazioneEseguitaCentraleByIdLocale, Me.Connection)

                cmd.Parameters.AddWithValue("idVaccinazioneEseguita", GetLongParam(idVaccinazioneEseguitaLocale))
                cmd.Parameters.AddWithValue("codiceUsl", GetStringParam(codiceUslLocale))

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As OracleClient.OracleDataReader = cmd.ExecuteReader()
                        vaccinazioneEseguitaCentrale = Me.GetVaccinazioneEseguitaCentraleListFromReader(idr).FirstOrDefault()
                    End Using

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return vaccinazioneEseguitaCentrale

        End Function

#End Region

#Region " Get VaccinazioneEseguitaCentraleDistribuita "

        Public Function GetVaccinazioneEseguitaCentraleDistribuitaByIdCentrale(idVaccinazioneEseguitaCentrale As Long, codiceUsl As String) As Entities.VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita Implements IVaccinazioneEseguitaCentraleProvider.GetVaccinazioneEseguitaCentraleDistribuitaByIdCentrale

            Dim vaccinazioneEseguitaDistribuita As Entities.VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try
                cmd = New OracleClient.OracleCommand("SELECT * FROM T_VACCINAZIONI_DISTRIBUITE WHERE  VCD_VCC_ID = :idVaccinazioneEseguitaCentrale AND VCD_USL_CODICE = :codiceUsl", Me.Connection)

                cmd.Parameters.AddWithValue("idVaccinazioneEseguitaCentrale", GetLongParam(idVaccinazioneEseguitaCentrale))
                cmd.Parameters.AddWithValue("codiceUsl", GetStringParam(codiceUsl))

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using idr As OracleClient.OracleDataReader = cmd.ExecuteReader()

                    vaccinazioneEseguitaDistribuita = Me.GetVaccinazioneEseguitaCentraleDistribuitaListFromReader(idr).FirstOrDefault()

                End Using

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return vaccinazioneEseguitaDistribuita

        End Function

        Public Function GetVaccinazioneEseguitaCentraleDistribuitaByUsl(idVaccinazioneEseguita As Long, codiceUsl As String) As Entities.VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita Implements IVaccinazioneEseguitaCentraleProvider.GetVaccinazioneEseguitaCentraleDistribuitaByUsl

            Dim vaccinazioneEseguitaDistribuita As Entities.VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try
                cmd = New OracleClient.OracleCommand("SELECT * FROM T_VACCINAZIONI_DISTRIBUITE WHERE  VCD_VES_ID = :idVaccinazioneEseguita AND VCD_USL_CODICE = :codiceUsl", Me.Connection)

                cmd.Parameters.AddWithValue("idVaccinazioneEseguita", GetLongParam(idVaccinazioneEseguita))
                cmd.Parameters.AddWithValue("codiceUsl", GetStringParam(codiceUsl))

                ownConnection = Me.ConditionalOpenConnection(cmd)

                Using idr As OracleClient.OracleDataReader = cmd.ExecuteReader()

                    vaccinazioneEseguitaDistribuita = Me.GetVaccinazioneEseguitaCentraleDistribuitaListFromReader(idr).FirstOrDefault()

                End Using

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return vaccinazioneEseguitaDistribuita

        End Function

#End Region

#Region " GetVaccinazioneEseguitaCentraleEnumerable "

        Public Function GetVaccinazioneEseguitaCentraleEnumerable(codicePazienteCentrale As String) As System.Collections.Generic.IEnumerable(Of Entities.VaccinazioneEseguitaCentrale) Implements IVaccinazioneEseguitaCentraleProvider.GetVaccinazioneEseguitaCentraleEnumerable

            Return GetVaccinazioneEseguitaCentraleEnumerable(codicePazienteCentrale, Nothing)

        End Function

        Public Function GetVaccinazioneEseguitaCentraleEnumerable(codicePazienteCentrale As String, flagVisibilita As String) As IEnumerable(Of VaccinazioneEseguitaCentrale) Implements IVaccinazioneEseguitaCentraleProvider.GetVaccinazioneEseguitaCentraleEnumerable

            Dim vaccinazioneEseguitaCentraleList As List(Of VaccinazioneEseguitaCentrale)

            Dim query As String = "SELECT * FROM T_VACCINAZIONI_CENTRALE WHERE VCC_PAZ_CODICE_CENTRALE = :codicePazienteCentrale"

            If Not String.IsNullOrEmpty(flagVisibilita) Then
                query += " AND VCC_VISIBILITA = :flagVisibilita"
            End If

            Dim cmd As New OracleClient.OracleCommand(query, Connection)

            cmd.Parameters.AddWithValue("codicePazienteCentrale", GetStringParam(codicePazienteCentrale))

            If Not String.IsNullOrEmpty(flagVisibilita) Then
                cmd.Parameters.AddWithValue("flagVisibilita", flagVisibilita)
            End If

            Dim ownConnection As Boolean = False

            Try
                ownConnection = ConditionalOpenConnection(cmd)

                Using idr As OracleClient.OracleDataReader = cmd.ExecuteReader()

                    vaccinazioneEseguitaCentraleList = GetVaccinazioneEseguitaCentraleListFromReader(idr)

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
                If Not cmd Is Nothing Then cmd.Dispose()
            End Try

            Return vaccinazioneEseguitaCentraleList.AsEnumerable()

        End Function

#End Region

#Region " GetVaccinazioneEseguitaCentraleDistribuitaEnumerable "

        ' Restituisce le eseguite distribuite in base agli id centrali delle vaccinazioni e alla usl
        Public Function GetVaccinazioneEseguitaCentraleDistribuitaEnumerable(listIdVaccinazioniEseguiteCentrali As List(Of Int64), codiceUsl As String) As IEnumerable(Of Entities.VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita) Implements IVaccinazioneEseguitaCentraleProvider.GetVaccinazioneEseguitaCentraleDistribuitaEnumerable

            Return Me.GetVaccinazioneEseguitaCentraleDistribuitaEnumerable(Nothing, listIdVaccinazioniEseguiteCentrali, codiceUsl)

        End Function

        ' Restituisce le eseguite distribuite in base al codice locale del paziente e alla usl
        Public Function GetVaccinazioneEseguitaCentraleDistribuitaEnumerable(codicePazienteLocale As Int64, codiceUsl As String) As System.Collections.Generic.IEnumerable(Of Entities.VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita) Implements IVaccinazioneEseguitaCentraleProvider.GetVaccinazioneEseguitaCentraleDistribuitaEnumerable

            Return Me.GetVaccinazioneEseguitaCentraleDistribuitaEnumerable(codicePazienteLocale, Nothing, codiceUsl)

        End Function

        Private Function GetVaccinazioneEseguitaCentraleDistribuitaEnumerable(codicePazienteLocale As Int64, listIdVaccinazioniEseguiteCentrali As List(Of Int64), codiceUsl As String) As System.Collections.Generic.IEnumerable(Of Entities.VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita)

            Dim vaccinazioneEseguitaDistribuitaList As List(Of Entities.VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita) = Nothing

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                If listIdVaccinazioniEseguiteCentrali Is Nothing Then

                    cmd.CommandText = "SELECT * FROM T_VACCINAZIONI_DISTRIBUITE WHERE VCD_PAZ_CODICE_LOCALE = :codicePazienteLocale AND VCD_USL_CODICE = :codiceUsl"

                    cmd.Parameters.AddWithValue("codicePazienteLocale", GetLongParam(codicePazienteLocale))

                Else

                    Dim filtroId As New System.Text.StringBuilder()

                    For i As Int16 = 0 To listIdVaccinazioniEseguiteCentrali.Count - 1

                        Dim paramName As String = String.Format("p{0}", i.ToString())

                        filtroId.AppendFormat(":{0},", paramName)

                        cmd.Parameters.AddWithValue(paramName, listIdVaccinazioniEseguiteCentrali(i))

                    Next

                    If filtroId.Length > 0 Then filtroId.Remove(filtroId.Length - 1, 1)

                    cmd.CommandText = String.Format("SELECT * FROM T_VACCINAZIONI_DISTRIBUITE WHERE VCD_VCC_ID IN ({0}) AND VCD_USL_CODICE = :codiceUsl", filtroId.ToString())

                End If

                cmd.Parameters.AddWithValue("codiceUsl", GetStringParam(codiceUsl))

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As OracleClient.OracleDataReader = cmd.ExecuteReader()
                        vaccinazioneEseguitaDistribuitaList = Me.GetVaccinazioneEseguitaCentraleDistribuitaListFromReader(idr)
                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return vaccinazioneEseguitaDistribuitaList.AsEnumerable()

        End Function

#End Region

#Region " Reazione Avversa "

        ''' <summary>
        ''' Restituisce l'id locale della vaccinazione relativo alla usl di inserimento della reazione, 
        ''' in base all'id locale della vaccinazione e alla usl di inserimento della vaccinazione stessa.
        ''' </summary>
        Public Function GetIdLocaleVaccinazioneUslReazioneByUslVaccinazione(idVaccinazioneEseguitaLocale As Long, codiceUslVaccinazione As String, codiceUslReazione As String) As Long? Implements IVaccinazioneEseguitaCentraleProvider.GetIdLocaleVaccinazioneUslReazioneByUslVaccinazione

            Dim idVaccinazioneUslReazione As Long? = Nothing

            Using cmd As New OracleClient.OracleCommand("SELECT VCD_VES_ID FROM T_VACCINAZIONI_DISTRIBUITE WHERE VCD_USL_CODICE = :USL_CODICE_REA AND VCD_VCC_ID = (SELECT VCD_VCC_ID FROM T_VACCINAZIONI_DISTRIBUITE WHERE VCD_VES_ID = :VES_ID_VAC AND VCD_USL_CODICE = :USL_CODICE_VAC)", Me.Connection)

                cmd.Parameters.AddWithValue("USL_CODICE_REA", codiceUslReazione)
                cmd.Parameters.AddWithValue("VES_ID_VAC", idVaccinazioneEseguitaLocale)
                cmd.Parameters.AddWithValue("USL_CODICE_VAC", codiceUslVaccinazione)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        idVaccinazioneUslReazione = Convert.ToInt64(obj)
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return idVaccinazioneUslReazione

        End Function

#End Region

#Region " Count "

        Public Function CountVaccinazioniEseguitePazienteCentrale(codicePazienteCentrale As String, listVisibilita As List(Of String), noEliminate As Boolean) As Integer Implements IVaccinazioneEseguitaCentraleProvider.CountVaccinazioniEseguitePazienteCentrale

            Return Me.CountVaccinazioniReazioni(codicePazienteCentrale, listVisibilita, noEliminate, False)

        End Function

        Public Function CountReazioniAvversePazienteCentrale(codicePazienteCentrale As String, listVisibilita As List(Of String), noEliminate As Boolean) As Integer Implements IVaccinazioneEseguitaCentraleProvider.CountReazioniAvversePazienteCentrale

            Return Me.CountVaccinazioniReazioni(codicePazienteCentrale, listVisibilita, noEliminate, True)

        End Function

        Private Function CountVaccinazioniReazioni(codicePazienteCentrale As String, listVisibilita As List(Of String), noEliminate As Boolean, countReazioni As Boolean) As Integer

            Dim count As Integer = 0

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                Dim query As New System.Text.StringBuilder("SELECT COUNT(*) FROM T_VACCINAZIONI_CENTRALE WHERE VCC_PAZ_CODICE_CENTRALE = :VCC_PAZ_CODICE_CENTRALE ")

                cmd.Parameters.AddWithValue("VCC_PAZ_CODICE_CENTRALE", codicePazienteCentrale)

                ' Filtro per conteggio reazioni avverse
                If countReazioni Then
                    query.Append(" AND VCC_VRA_ID IS NOT NULL ")
                End If

                ' Filtro visibilità
                query.Append(Me.GetFiltroVisibilita(listVisibilita, cmd))

                ' Filtro eliminate
                If countReazioni Then
                    query.Append(Me.GetFiltroReazioniEliminate(noEliminate, cmd))
                Else
                    query.Append(Me.GetFiltroVaccinazioniEliminate(noEliminate, cmd))
                End If

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

#End Region

#Region " Usl Gestite "

        Public Function GetUslInserimentoVaccinazioniEseguite(codicePazienteCentrale As String, noEliminate As Boolean) As List(Of String) Implements IVaccinazioneEseguitaCentraleProvider.GetUslInserimentoVaccinazioniEseguite

            Return Me.GetUslInserimentoVaccinazioniReazioni(codicePazienteCentrale, noEliminate, False)

        End Function

        Public Function GetUslInserimentoReazioniAvverse(codicePazienteCentrale As String, noEliminate As Boolean) As List(Of String) Implements IVaccinazioneEseguitaCentraleProvider.GetUslInserimentoReazioniAvverse

            Return Me.GetUslInserimentoVaccinazioniReazioni(codicePazienteCentrale, noEliminate, True)

        End Function

        Private Function GetUslInserimentoVaccinazioniReazioni(codicePazienteCentrale As String, noEliminate As Boolean, ricercaUslReazioni As Boolean) As List(Of String)

            Dim listUslInserimento As New List(Of String)()

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                Dim query As New System.Text.StringBuilder()

                query.AppendFormat("SELECT DISTINCT VCC_USL_INSERIMENTO{0} FROM T_VACCINAZIONI_CENTRALE ", IIf(ricercaUslReazioni, "_REA", String.Empty))

                ' Filtro paziente
                query.Append(" WHERE VCC_PAZ_CODICE_CENTRALE = :VCC_PAZ_CODICE_CENTRALE ")
                cmd.Parameters.AddWithValue("VCC_PAZ_CODICE_CENTRALE", codicePazienteCentrale)

                '' Filtro visibilità
                'query.Append(Me.GetFiltroVisibilita(listVisibilita, cmd))

                If ricercaUslReazioni Then

                    ' Filtro reazione presente nella vaccinazione
                    query.Append(" AND NOT VCC_VRA_ID IS NULL ")

                    ' Filtro reazioni eliminate
                    query.Append(Me.GetFiltroReazioniEliminate(noEliminate, cmd))

                Else

                    ' Filtro vaccinazioni eliminate
                    query.Append(Me.GetFiltroVaccinazioniEliminate(noEliminate, cmd))

                End If

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
        ''' Restituisce l'elenco delle usl gestite in cui sono state distribuite la vaccinazioni specificate, in base a id locali e usl inserimento
        ''' </summary>
        Public Function GetUslDistribuiteVaccinazioniEseguite(idVaccinazioniEseguiteLocali As List(Of Long), codiceUslInserimentoVaccinazioni As String) As List(Of Entities.UslDistribuitaDatoVaccinaleInfo) Implements IVaccinazioneEseguitaCentraleProvider.GetUslDistribuiteVaccinazioniEseguite

            Dim listUslDistribuite As List(Of Entities.UslDistribuitaDatoVaccinaleInfo) = Nothing

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    listUslDistribuite = Me.GetUslDistribuite("SELECT VCC_VES_ID ID, VCC_USL_INSERIMENTO USL_INSERIMENTO, VCD_USL_CODICE USL_DISTRIBUITA FROM T_VACCINAZIONI_DISTRIBUITE JOIN T_VACCINAZIONI_CENTRALE ON VCD_VCC_ID = VCC_ID WHERE VCC_USL_INSERIMENTO = :USL_INSERIMENTO AND VCC_VES_ID IN ({0}) AND VCD_USL_CODICE <> VCC_USL_INSERIMENTO",
                                                              idVaccinazioniEseguiteLocali, codiceUslInserimentoVaccinazioni, cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listUslDistribuite

        End Function

        ''' <summary>
        ''' Restituisce l'elenco delle usl gestite in cui sono state distribuite le reazioni specificate, in base a id locali e usl inserimento
        ''' </summary>
        Public Function GetUslDistribuiteReazioniAvverse(idReazioniAvverseLocali As List(Of Long), codiceUslInserimentoReazioni As String) As List(Of Entities.UslDistribuitaDatoVaccinaleInfo) Implements IVaccinazioneEseguitaCentraleProvider.GetUslDistribuiteReazioniAvverse

            Dim listUslDistribuite As List(Of Entities.UslDistribuitaDatoVaccinaleInfo) = Nothing

            Using cmd As New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    listUslDistribuite = Me.GetUslDistribuite("SELECT VCC_VRA_ID ID, VCC_USL_INSERIMENTO_REA USL_INSERIMENTO, VCD_USL_CODICE USL_DISTRIBUITA FROM T_VACCINAZIONI_DISTRIBUITE JOIN T_VACCINAZIONI_CENTRALE ON VCD_VCC_ID = VCC_ID WHERE VCC_USL_INSERIMENTO_REA = :USL_INSERIMENTO AND VCC_VRA_ID IN ({0}) AND VCD_USL_CODICE <> VCC_USL_INSERIMENTO_REA",
                                                              idReazioniAvverseLocali, codiceUslInserimentoReazioni, cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listUslDistribuite

        End Function

        Private Function GetUslDistribuite(query As String, idLocali As List(Of Int64), codiceUslInserimento As String, cmd As OracleClient.OracleCommand) As List(Of Entities.UslDistribuitaDatoVaccinaleInfo)

            Dim listDatiUslDistribuite As New List(Of Entities.UslDistribuitaDatoVaccinaleInfo)()

            cmd.Parameters.AddWithValue("USL_INSERIMENTO", codiceUslInserimento)

            Dim filtroId As New System.Text.StringBuilder()

            For i As Integer = 0 To idLocali.Count - 1

                Dim parameterName As String = String.Format("p{0}", i)

                filtroId.AppendFormat(":{0},", parameterName)

                cmd.Parameters.AddWithValue(parameterName, idLocali(i))

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

            Return listDatiUslDistribuite

        End Function

#End Region

#Region " Conflitti "

        Public Function IsVaccinazioneEseguitaCentraleInConflitto(idVaccinazioneEseguitaCentrale As Int64) As Boolean

            Dim isConflitto As Boolean = False

            Using cmd As New OracleClient.OracleCommand("SELECT 1 FROM T_VACCINAZIONI_CENTRALE WHERE VCC_ID = :VCC_ID AND VCC_ID_CONFLITTO IS NOT NULL AND VCC_DATA_RISOLUZ_CONFLITTO IS NULL", Me.Connection)

                cmd.Parameters.AddWithValue("VCC_ID", idVaccinazioneEseguitaCentrale)

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

        Public Function CountVaccinazioneEseguitaCentraleInConflittoByIdConflitto(idConflitto As Long) As Long Implements IVaccinazioneEseguitaCentraleProvider.CountVaccinazioneEseguitaCentraleInConflittoByIdConflitto

            Using cmd As New OracleClient.OracleCommand("SELECT COUNT(*) FROM T_VACCINAZIONI_CENTRALE WHERE VCC_ID_CONFLITTO = :VCC_ID_CONFLITTO AND VCC_DATA_RISOLUZ_CONFLITTO IS NULL", Me.Connection)

                cmd.Parameters.AddWithValue("VCC_ID_CONFLITTO", idConflitto)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Return cmd.ExecuteScalar()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

        End Function

        Public Sub UpdateIdConflittoVaccinazioneEseguitaCentraleByIdConflitto(idConflittoCorrente As Long, idConflittoAggiornato As Long?) Implements IVaccinazioneEseguitaCentraleProvider.UpdateIdConflittoVaccinazioneEseguitaCentraleByIdConflitto

            Using cmd As OracleClient.OracleCommand = New OracleClient.OracleCommand("UPDATE T_VACCINAZIONI_CENTRALE SET VCC_ID_CONFLITTO = :VCC_ID_CONFLITTO_AGG WHERE VCC_ID_CONFLITTO = :VCC_ID_CONFLITTO_CORR", Me.Connection)

                cmd.Parameters.AddWithValue("VCC_ID_CONFLITTO_CORR", idConflittoCorrente)
                cmd.Parameters.AddWithValue("VCC_ID_CONFLITTO_AGG", IIf(idConflittoAggiornato.HasValue, idConflittoAggiornato, DBNull.Value))

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

        End Sub

        ''' <summary>
        ''' Conteggio conflitti in base ai filtri specificati
        ''' </summary>
        ''' <param name="filtriRicercaConflitti"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CountConflittiVaccinazioniEseguiteCentrale(filtriRicercaConflitti As Filters.FiltriRicercaConflittiDatiVaccinali) As Integer Implements IVaccinazioneEseguitaCentraleProvider.CountConflittiVaccinazioniEseguiteCentrale

            Dim count As Integer = 0

            Using cmd As OracleClient.OracleCommand = New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                Dim query As System.Text.StringBuilder = Me.GetQueryRicercaConflittiMaster(cmd, filtriRicercaConflitti, True)

                cmd.CommandText = query.ToString()

                Dim ownConnection As Boolean = False

                Dim obj As Object

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    obj = cmd.ExecuteScalar()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

                If obj Is Nothing OrElse obj Is DBNull.Value Then Return 0

                count = Convert.ToInt32(obj)

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Restituisce i conflitti relativi ai pazienti specificati.
        ''' </summary>
        ''' <param name="codiciCentraliPazienti"></param>
        ''' <param name="risolviConflittiGiaTentati"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetConflittiVaccinazioniEseguiteCentrale(codiciCentraliPazienti As List(Of String), risolviConflittiGiaTentati As Boolean) As List(Of Entities.ConflittoVaccinazioniEseguite) Implements IVaccinazioneEseguitaCentraleProvider.GetConflittiVaccinazioniEseguiteCentrale

            Return Me.GetConflittiVaccinazioniEseguiteCentrale(Nothing, Nothing, codiciCentraliPazienti, risolviConflittiGiaTentati)

        End Function

        ''' <summary>
        ''' Restituisce i conflitti in base ai filtri specificati.
        ''' </summary>
        ''' <param name="filtriRicercaConflitti"></param>
        ''' <param name="pagingOptions">Se valorizzato, effettua la paginazione su db.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetConflittiVaccinazioniEseguiteCentrale(filtriRicercaConflitti As Filters.FiltriRicercaConflittiDatiVaccinali, pagingOptions As OnAssistnet.Data.PagingOptions) As List(Of Entities.ConflittoVaccinazioniEseguite) Implements IVaccinazioneEseguitaCentraleProvider.GetConflittiVaccinazioniEseguiteCentrale

            Return Me.GetConflittiVaccinazioniEseguiteCentrale(filtriRicercaConflitti, pagingOptions, Nothing, True)

        End Function

        Private Function GetConflittiVaccinazioniEseguiteCentrale(filtriRicercaConflitti As Filters.FiltriRicercaConflittiDatiVaccinali, pagingOptions As OnAssistnet.Data.PagingOptions, codiciCentraliPazienti As List(Of String), risolviConflittiGiaTentati As Boolean) As List(Of Entities.ConflittoVaccinazioniEseguite)

            Dim listConflittiVaccinazioniEseguite As New List(Of Entities.ConflittoVaccinazioniEseguite)()

            ' Ricerca vaccinazioni "principali" in conflitto
            Using cmd As OracleClient.OracleCommand = New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                Dim query As System.Text.StringBuilder = Me.GetQueryRicercaConflittiMaster(cmd, filtriRicercaConflitti, False)

                If Not codiciCentraliPazienti.IsNullOrEmpty() Then

                    If codiciCentraliPazienti.Count = 1 Then
                        query.Append(" AND PAZ_CODICE = :paz_codice ")
                        cmd.Parameters.AddWithValue("paz_codice", codiciCentraliPazienti.First())
                    Else
                        Dim filtro As GetInFilterResult = Me.GetInFilter(codiciCentraliPazienti)
                        query.AppendFormat(" AND PAZ_CODICE IN ({0}) ", filtro.InFilter)
                        cmd.Parameters.AddRange(filtro.Parameters)
                    End If

                End If

                If Not risolviConflittiGiaTentati Then
                    query.Append(" AND NOT EXISTS (SELECT 1 FROM T_CONFLITTI_RISOLUZIONE WHERE CRS_VCC_ID = VCC_ID ) ")
                End If

                query.Append(" ORDER BY PAZ_COGNOME, PAZ_NOME, PAZ_DATA_NASCITA, PAZ_CODICE, VCC_ID")

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
                            Dim VCC_ID As Int16 = idr.GetOrdinal("VCC_ID")

                            While idr.Read()

                                Dim conflittoVaccinazioniEseguite As New Entities.ConflittoVaccinazioniEseguite()

                                conflittoVaccinazioniEseguite.IdVaccinazioneEseguitaCentrale = idr.GetInt64(VCC_ID)
                                conflittoVaccinazioniEseguite.CodicePazienteCentrale = idr.GetString(PAZ_CODICE)
                                conflittoVaccinazioniEseguite.Cognome = idr.GetStringOrDefault(PAZ_COGNOME)
                                conflittoVaccinazioniEseguite.Nome = idr.GetStringOrDefault(PAZ_NOME)
                                conflittoVaccinazioniEseguite.DataNascita = idr.GetNullableDateTimeOrDefault(PAZ_DATA_NASCITA)

                                listConflittiVaccinazioniEseguite.Add(conflittoVaccinazioniEseguite)

                            End While

                        End If

                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            ' Ricerca dati dei conflitti per ogni vaccinazione trovata
            For Each conflittoVaccinazioniEseguite As Entities.ConflittoVaccinazioniEseguite In listConflittiVaccinazioniEseguite

                conflittoVaccinazioniEseguite.VaccinazioniEseguiteInConflitto =
                    Me.GetDatiVaccinazioniInConflitto(conflittoVaccinazioniEseguite.IdVaccinazioneEseguitaCentrale)

            Next

            Return listConflittiVaccinazioniEseguite

        End Function

        Private Function GetQueryRicercaConflittiMaster(cmd As OracleClient.OracleCommand, filtriRicercaConflitti As Filters.FiltriRicercaConflittiDatiVaccinali, countQuery As Boolean) As System.Text.StringBuilder

            Dim query As New System.Text.StringBuilder()

            If countQuery Then
                query.Append("SELECT count(VCC_ID) ")
            Else
                query.Append("SELECT PAZ_CODICE, PAZ_COGNOME, PAZ_NOME, PAZ_DATA_NASCITA, VCC_ID ")
            End If

            query.Append("FROM T_PAZ_PAZIENTI JOIN T_VACCINAZIONI_CENTRALE ON PAZ_CODICE = VCC_PAZ_CODICE_CENTRALE ")

            query.Append("WHERE NOT VCC_ID_CONFLITTO IS NULL ")
            query.Append("AND VCC_DATA_RISOLUZ_CONFLITTO IS NULL ")
            query.Append("AND VCC_ID_CONFLITTO = VCC_ID ")

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

        Private Function GetDatiVaccinazioniInConflitto(idVaccinazioneEseguitaCentrale As Int64) As List(Of Entities.ConflittoVaccinazioniEseguite.DatiVaccinazioneInConflitto)

            Dim listDatiVaccinazioniInConflitto As New List(Of Entities.ConflittoVaccinazioniEseguite.DatiVaccinazioneInConflitto)

            Using cmd As OracleClient.OracleCommand = New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                Dim query As New System.Text.StringBuilder()

                query.Append("SELECT VCC_ID, VCC_ID_CONFLITTO, VCC_PAZ_CODICE_CENTRALE, VCC_PAZ_CODICE_LOCALE, VCC_VISIBILITA, VCC_TIPO, ")
                query.Append("       VCC_USL_INSERIMENTO, VCC_VES_ID, VCC_VRA_ID, VCC_TIPO_REA, VCC_DATA_INSERIMENTO ")
                query.Append("FROM T_VACCINAZIONI_CENTRALE ")
                query.Append("WHERE VCC_ID_CONFLITTO = :vcc_id_conflitto ")
                query.Append("AND VCC_DATA_RISOLUZ_CONFLITTO IS NULL ")
                query.Append("ORDER BY VCC_VISIBILITA DESC")

                cmd.Parameters.AddWithValue("vcc_id_conflitto", idVaccinazioneEseguitaCentrale)

                cmd.CommandText = query.ToString()

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim VCC_ID As Int16 = idr.GetOrdinal("VCC_ID")
                            Dim VCC_ID_CONFLITTO As Int16 = idr.GetOrdinal("VCC_ID_CONFLITTO")
                            Dim VCC_PAZ_CODICE_CENTRALE As Int16 = idr.GetOrdinal("VCC_PAZ_CODICE_CENTRALE")
                            Dim VCC_PAZ_CODICE_LOCALE As Int16 = idr.GetOrdinal("VCC_PAZ_CODICE_LOCALE")
                            Dim VCC_VISIBILITA As Int16 = idr.GetOrdinal("VCC_VISIBILITA")
                            Dim VCC_TIPO As Int16 = idr.GetOrdinal("VCC_TIPO")
                            Dim VCC_USL_INSERIMENTO As Int16 = idr.GetOrdinal("VCC_USL_INSERIMENTO")
                            Dim VCC_VES_ID As Int16 = idr.GetOrdinal("VCC_VES_ID")
                            Dim VCC_VRA_ID As Int16 = idr.GetOrdinal("VCC_VRA_ID")
                            Dim VCC_TIPO_REA As Int16 = idr.GetOrdinal("VCC_TIPO_REA")
                            Dim VCC_DATA_INSERIMENTO As Int16 = idr.GetOrdinal("VCC_DATA_INSERIMENTO")

                            While idr.Read()

                                Dim datiVaccinazioniInConflitto As New Entities.ConflittoVaccinazioniEseguite.DatiVaccinazioneInConflitto()

                                ' Dati che verranno reperiti in locale:
                                'datiVaccinazioniInConflitto.CodiceAssociazione
                                'datiVaccinazioniInConflitto.CodiceLotto
                                'datiVaccinazioniInConflitto.CodiceNomeCommerciale
                                'datiVaccinazioniInConflitto.CodiceVaccinazione
                                'datiVaccinazioniInConflitto.DataEffettuazione
                                'datiVaccinazioniInConflitto.DoseAssociazione
                                'datiVaccinazioniInConflitto.DoseVaccinazione
                                'datiVaccinazioniInConflitto.DataRegistrazione
                                'datiVaccinazioniInConflitto.Stato

                                datiVaccinazioniInConflitto.CodicePazienteCentrale = idr.GetString(VCC_PAZ_CODICE_CENTRALE)
                                datiVaccinazioniInConflitto.CodicePaziente = idr.GetInt64(VCC_PAZ_CODICE_LOCALE)
                                datiVaccinazioniInConflitto.CodiceUslVaccinazioneEseguita = idr.GetString(VCC_USL_INSERIMENTO)
                                datiVaccinazioniInConflitto.FlagVisibilitaCentrale = idr.GetString(VCC_VISIBILITA)
                                datiVaccinazioniInConflitto.Id = idr.GetInt64(VCC_ID)
                                datiVaccinazioniInConflitto.IdVaccinazioneEseguita = idr.GetInt64(VCC_VES_ID)
                                datiVaccinazioniInConflitto.TipoVaccinazioneEseguitaCentrale = idr.GetString(VCC_TIPO)
                                datiVaccinazioniInConflitto.IdReazioneAvversa = idr.GetNullableInt64OrDefault(VCC_VRA_ID)
                                datiVaccinazioniInConflitto.TipoReazioneAvversa = idr.GetStringOrDefault(VCC_TIPO_REA)
                                datiVaccinazioniInConflitto.DataInserimentoCentrale = idr.GetDateTime(VCC_DATA_INSERIMENTO)

                                listDatiVaccinazioniInConflitto.Add(datiVaccinazioniInConflitto)

                            End While

                        End If

                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listDatiVaccinazioniInConflitto

        End Function

        Public Function InsertConflittoEseguiteRisoluzione(conflittoRisoluzione As Entities.ConflittoEseguiteRisoluzione) As Integer Implements IVaccinazioneEseguitaCentraleProvider.InsertConflittoEseguiteRisoluzione

            Dim count As Integer = 0

            Using cmd As OracleClient.OracleCommand = New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.CommandText = "INSERT INTO T_CONFLITTI_RISOLUZIONE (CRS_PRC_ID, CRS_VCC_ID, CRS_VCC_ID_CONFLITTO, CRS_VCC_PAZ_CODICE_CENTRALE, CRS_VCC_PAZ_CODICE_LOCALE, CRS_VCC_VES_ID, CRS_VCC_USL_INSERIMENTO, CRS_STATO, CRS_MESSAGE, CRS_ID_CONFLITTO_RISOLUZIONE) values (:CRS_PRC_ID, :CRS_VCC_ID, :CRS_VCC_ID_CONFLITTO, :CRS_VCC_PAZ_CODICE_CENTRALE, :CRS_VCC_PAZ_CODICE_LOCALE, :CRS_VCC_VES_ID, :CRS_VCC_USL_INSERIMENTO, :CRS_STATO, :CRS_MESSAGE, :CRS_ID_CONFLITTO_RISOLUZIONE)"

                    cmd.Parameters.AddWithValue("CRS_PRC_ID", conflittoRisoluzione.IdProcessoBatch)
                    cmd.Parameters.AddWithValue("CRS_VCC_ID", conflittoRisoluzione.IdVaccinazioneEseguitaCentrale)
                    cmd.Parameters.AddWithValue("CRS_VCC_ID_CONFLITTO", conflittoRisoluzione.IdConflitto)
                    cmd.Parameters.AddWithValue("CRS_VCC_PAZ_CODICE_CENTRALE", conflittoRisoluzione.CodicePazienteCentrale)
                    cmd.Parameters.AddWithValue("CRS_VCC_PAZ_CODICE_LOCALE", conflittoRisoluzione.CodicePazienteLocale)
                    cmd.Parameters.AddWithValue("CRS_VCC_VES_ID", conflittoRisoluzione.IdVaccinazioneEseguitaLocale)
                    cmd.Parameters.AddWithValue("CRS_VCC_USL_INSERIMENTO", conflittoRisoluzione.CodiceUslInserimento)

                    If String.IsNullOrWhiteSpace(conflittoRisoluzione.StatoRisoluzioneConflitto) Then
                        cmd.Parameters.AddWithValue("CRS_STATO", System.DBNull.Value)
                    Else
                        cmd.Parameters.AddWithValue("CRS_STATO", conflittoRisoluzione.StatoRisoluzioneConflitto)
                    End If

                    If String.IsNullOrWhiteSpace(conflittoRisoluzione.Message) Then
                        cmd.Parameters.AddWithValue("CRS_MESSAGE", System.DBNull.Value)
                    Else
                        cmd.Parameters.AddWithValue("CRS_MESSAGE", conflittoRisoluzione.Message)
                    End If

                    If conflittoRisoluzione.IdConflittoRisoluzione.HasValue Then
                        cmd.Parameters.AddWithValue("CRS_ID_CONFLITTO_RISOLUZIONE", conflittoRisoluzione.IdConflittoRisoluzione.Value)
                    Else
                        cmd.Parameters.AddWithValue("CRS_ID_CONFLITTO_RISOLUZIONE", System.DBNull.Value)
                    End If

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Update t_conflitti_risoluzione con i dati relativi al risultato dell'elaborazione (stato, messaggio ed eventuale id del conflitto scelto come principale).
        ''' </summary>
        ''' <param name="conflittoRisoluzione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateConflittoEseguiteRisoluzione(conflittoRisoluzione As Entities.ConflittoEseguiteRisoluzione) As Integer Implements IVaccinazioneEseguitaCentraleProvider.UpdateConflittoEseguiteRisoluzione

            Dim count As Integer = 0

            Using cmd As OracleClient.OracleCommand = New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.CommandText = "UPDATE T_CONFLITTI_RISOLUZIONE SET CRS_STATO = :CRS_STATO, CRS_MESSAGE = :CRS_MESSAGE, CRS_ID_CONFLITTO_RISOLUZIONE = :CRS_ID_CONFLITTO_RISOLUZIONE WHERE CRS_PRC_ID = :CRS_PRC_ID AND CRS_VCC_ID = :CRS_VCC_ID"

                    If String.IsNullOrWhiteSpace(conflittoRisoluzione.StatoRisoluzioneConflitto) Then
                        cmd.Parameters.AddWithValue("CRS_STATO", System.DBNull.Value)
                    Else
                        cmd.Parameters.AddWithValue("CRS_STATO", conflittoRisoluzione.StatoRisoluzioneConflitto)
                    End If

                    If String.IsNullOrWhiteSpace(conflittoRisoluzione.Message) Then
                        cmd.Parameters.AddWithValue("CRS_MESSAGE", System.DBNull.Value)
                    Else
                        cmd.Parameters.AddWithValue("CRS_MESSAGE", conflittoRisoluzione.Message)
                    End If

                    If conflittoRisoluzione.IdConflittoRisoluzione.HasValue Then
                        cmd.Parameters.AddWithValue("CRS_ID_CONFLITTO_RISOLUZIONE", conflittoRisoluzione.IdConflittoRisoluzione.Value)
                    Else
                        cmd.Parameters.AddWithValue("CRS_ID_CONFLITTO_RISOLUZIONE", System.DBNull.Value)
                    End If

                    cmd.Parameters.AddWithValue("CRS_PRC_ID", conflittoRisoluzione.IdProcessoBatch)
                    cmd.Parameters.AddWithValue("CRS_VCC_ID", conflittoRisoluzione.IdVaccinazioneEseguitaCentrale)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Aggiorna i campi utente e data di risoluzione del conflitto per la vaccinazione specificata.
        ''' </summary>
        ''' <param name="idVaccinazioneCentrale"></param>
        ''' <param name="idUtenteRisoluzione"></param>
        ''' <param name="dataRisoluzione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateConflittoEseguitaCentrale(idVaccinazioneCentrale As Long, idUtenteRisoluzione As Long, dataRisoluzione As DateTime) As Integer Implements IVaccinazioneEseguitaCentraleProvider.UpdateConflittoEseguitaCentrale

            Dim count As Integer = 0

            Using cmd As OracleClient.OracleCommand = New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.CommandText = "UPDATE T_VACCINAZIONI_CENTRALE SET VCC_DATA_RISOLUZ_CONFLITTO = :VCC_DATA_RISOLUZ_CONFLITTO, VCC_UTE_ID_RISOLUZ_CONFLITTO = :VCC_UTE_ID_RISOLUZ_CONFLITTO WHERE VCC_ID = :VCC_ID"

                    cmd.Parameters.AddWithValue("VCC_DATA_RISOLUZ_CONFLITTO", dataRisoluzione)
                    cmd.Parameters.AddWithValue("VCC_UTE_ID_RISOLUZ_CONFLITTO", idUtenteRisoluzione)
                    cmd.Parameters.AddWithValue("VCC_ID", idVaccinazioneCentrale)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Cancellazione campo id conflitto per la vaccinazione centrale specificata.
        ''' </summary>
        ''' <param name="idVaccinazioneCentrale"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CancellaIdConflittoVaccinazioneEseguitaCentrale(idVaccinazioneCentrale As Long) As Integer Implements IVaccinazioneEseguitaCentraleProvider.CancellaIdConflittoVaccinazioneEseguitaCentrale

            Dim count As Integer = 0

            Using cmd As OracleClient.OracleCommand = New OracleClient.OracleCommand()

                cmd.Connection = Me.Connection

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.CommandText = "UPDATE T_VACCINAZIONI_CENTRALE SET VCC_ID_CONFLITTO = null WHERE VCC_ID = :VCC_ID"
                    cmd.Parameters.AddWithValue("VCC_ID", idVaccinazioneCentrale)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

#End Region

#Region " Insert / Update / Delete "

        Public Sub InsertVaccinazioneEseguitaCentrale(vaccinazioneEseguitaCentrale As Entities.VaccinazioneEseguitaCentrale) Implements IVaccinazioneEseguitaCentraleProvider.InsertVaccinazioneEseguitaCentrale

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try
                ' Calcolo ID
                cmd = New OracleClient.OracleCommand("SELECT SEQ_VACCINAZIONI_CENTRALE.NEXTVAL FROM DUAL", Me.Connection)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                vaccinazioneEseguitaCentrale.Id = Convert.ToInt64(cmd.ExecuteScalar())

                ' Inserimento
                cmd.CommandText = "INSERT INTO T_VACCINAZIONI_CENTRALE(VCC_ID, VCC_PAZ_CODICE_CENTRALE, VCC_PAZ_CODICE_LOCALE, VCC_VES_ID, VCC_TIPO, VCC_VISIBILITA, VCC_DATA_REVOCA_VISIBILITA, VCC_USL_INSERIMENTO, VCC_DATA_INSERIMENTO, VCC_UTE_ID_INSERIMENTO, VCC_UTE_ID_VARIAZIONE, VCC_DATA_VARIAZIONE, VCC_DATA_ELIMINAZIONE, VCC_UTE_ID_ELIMINAZIONE, VCC_DATA_SCADENZA, VCC_UTE_ID_SCADENZA, VCC_USL_SCADENZA, VCC_DATA_RIPRISTINO, VCC_UTE_ID_RIPRISTINO, VCC_VRA_ID, VCC_USL_INSERIMENTO_REA, VCC_DATA_INSERIMENTO_REA, VCC_UTE_ID_INSERIMENTO_REA, VCC_DATA_VARIAZIONE_REA, VCC_UTE_ID_VARIAZIONE_REA, VCC_DATA_ELIMINAZIONE_REA, VCC_UTE_ID_ELIMINAZIONE_REA, VCC_TIPO_REA, VCC_PAZ_CODICE_ALIAS_CENTRALE, VCC_USL_CODICE_ALIAS, VCC_UTE_ID_ALIAS, VCC_DATA_ALIAS, VCC_ID_CONFLITTO, VCC_UTE_ID_RISOLUZ_CONFLITTO, VCC_DATA_RISOLUZ_CONFLITTO, VCC_USL_ULTIMA_OPERAZIONE, VCC_UTE_ID_ULTIMA_OPERAZIONE, VCC_USL_ULTIMA_OPERAZ_REA, VCC_UTE_ID_ULTIMA_OPERAZ_REA) VALUES(:VCC_ID, :VCC_PAZ_CODICE_CENTRALE, :VCC_PAZ_CODICE_LOCALE, :VCC_VES_ID, :VCC_TIPO, :VCC_VISIBILITA, :VCC_DATA_REVOCA_VISIBILITA, :VCC_USL_INSERIMENTO, :VCC_DATA_INSERIMENTO, :VCC_UTE_ID_INSERIMENTO, :VCC_UTE_ID_VARIAZIONE, :VCC_DATA_VARIAZIONE, :VCC_DATA_ELIMINAZIONE, :VCC_UTE_ID_ELIMINAZIONE, :VCC_DATA_SCADENZA, :VCC_UTE_ID_SCADENZA, :VCC_USL_SCADENZA, :VCC_DATA_RIPRISTINO, :VCC_UTE_ID_RIPRISTINO, :VCC_VRA_ID, :VCC_USL_INSERIMENTO_REA, :VCC_DATA_INSERIMENTO_REA, :VCC_UTE_ID_INSERIMENTO_REA, :VCC_DATA_VARIAZIONE_REA, :VCC_UTE_ID_VARIAZIONE_REA, :VCC_DATA_ELIMINAZIONE_REA, :VCC_UTE_ID_ELIMINAZIONE_REA, :VCC_TIPO_REA, :VCC_PAZ_CODICE_ALIAS_CENTRALE, :VCC_USL_CODICE_ALIAS, :VCC_UTE_ID_ALIAS, :VCC_DATA_ALIAS, :VCC_ID_CONFLITTO, :VCC_UTE_ID_RISOLUZ_CONFLITTO,:VCC_DATA_RISOLUZ_CONFLITTO, :VCC_USL_ULTIMA_OPERAZIONE, :VCC_UTE_ID_ULTIMA_OPERAZIONE, :VCC_USL_ULTIMA_OPERAZ_REA, :VCC_UTE_ID_ULTIMA_OPERAZ_REA)"

                Me.AddVaccinazioneEseguitaCentraleInsertOrUpdateParameters(cmd, vaccinazioneEseguitaCentrale)

                cmd.ExecuteNonQuery()

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

        End Sub

        Public Sub InsertVaccinazioneEseguitaCentraleDistribuita(vaccinazioneEseguitaCentraleDistribuita As Entities.VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita) Implements IVaccinazioneEseguitaCentraleProvider.InsertVaccinazioneEseguitaCentraleDistribuita

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try
                ' Calcolo ID
                cmd = New OracleClient.OracleCommand("SELECT SEQ_VACCINAZIONI_DISTRIBUITE.NEXTVAL FROM DUAL", Me.Connection)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                vaccinazioneEseguitaCentraleDistribuita.Id = Convert.ToInt64(cmd.ExecuteScalar())

                ' Inserimento
                cmd.CommandText = "INSERT INTO T_VACCINAZIONI_DISTRIBUITE(VCD_ID, VCD_PAZ_CODICE_LOCALE, VCD_VCC_ID, VCD_VES_ID, VCD_VRA_ID, VCD_VCC_VRA_ID, VCD_VCC_USL_INSERIMENTO_REA, VCD_USL_CODICE, VCD_DATA_INS_LOCALE, VCD_UTE_ID_INS_LOCALE, VCD_DATA_AGG_LOCALE) VALUES(:VCD_ID, :VCD_PAZ_CODICE_LOCALE, :VCD_VCC_ID, :VCD_VES_ID, :VCD_VRA_ID, :VCD_VCC_VRA_ID, :VCD_VCC_USL_INSERIMENTO_REA, :VCD_USL_CODICE, :VCD_DATA_INS_LOCALE, :VCD_UTE_ID_INS_LOCALE, :VCD_DATA_AGG_LOCALE)"

                Me.AddVaccinazioneEseguitaCentraleDistribuitaInsertOrUpdateParameters(cmd, vaccinazioneEseguitaCentraleDistribuita)

                cmd.ExecuteNonQuery()

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

        End Sub

        Public Sub UpdateVaccinazioneEseguitaCentrale(vaccinazioneEseguitaCentrale As Entities.VaccinazioneEseguitaCentrale) Implements IVaccinazioneEseguitaCentraleProvider.UpdateVaccinazioneEseguitaCentrale

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try
                cmd = New OracleClient.OracleCommand("UPDATE T_VACCINAZIONI_CENTRALE SET VCC_PAZ_CODICE_CENTRALE=:VCC_PAZ_CODICE_CENTRALE, VCC_PAZ_CODICE_LOCALE=:VCC_PAZ_CODICE_LOCALE, VCC_VES_ID=:VCC_VES_ID, VCC_TIPO=:VCC_TIPO, VCC_VISIBILITA=:VCC_VISIBILITA, VCC_DATA_REVOCA_VISIBILITA=:VCC_DATA_REVOCA_VISIBILITA, VCC_USL_INSERIMENTO=:VCC_USL_INSERIMENTO, VCC_DATA_INSERIMENTO=:VCC_DATA_INSERIMENTO, VCC_UTE_ID_INSERIMENTO =:VCC_UTE_ID_INSERIMENTO, VCC_UTE_ID_VARIAZIONE = :VCC_UTE_ID_VARIAZIONE, VCC_DATA_VARIAZIONE = :VCC_DATA_VARIAZIONE, VCC_DATA_ELIMINAZIONE =:VCC_DATA_ELIMINAZIONE, VCC_UTE_ID_ELIMINAZIONE=:VCC_UTE_ID_ELIMINAZIONE, VCC_DATA_SCADENZA=:VCC_DATA_SCADENZA, VCC_UTE_ID_SCADENZA=:VCC_UTE_ID_SCADENZA, VCC_USL_SCADENZA=:VCC_USL_SCADENZA, VCC_DATA_RIPRISTINO=:VCC_DATA_RIPRISTINO, VCC_UTE_ID_RIPRISTINO=:VCC_UTE_ID_RIPRISTINO, VCC_VRA_ID=:VCC_VRA_ID, VCC_USL_INSERIMENTO_REA=:VCC_USL_INSERIMENTO_REA, VCC_DATA_INSERIMENTO_REA=:VCC_DATA_INSERIMENTO_REA, VCC_UTE_ID_INSERIMENTO_REA=:VCC_UTE_ID_INSERIMENTO_REA, VCC_DATA_VARIAZIONE_REA=:VCC_DATA_VARIAZIONE_REA, VCC_UTE_ID_VARIAZIONE_REA=:VCC_UTE_ID_VARIAZIONE_REA, VCC_DATA_ELIMINAZIONE_REA=:VCC_DATA_ELIMINAZIONE_REA, VCC_UTE_ID_ELIMINAZIONE_REA=:VCC_UTE_ID_ELIMINAZIONE_REA, VCC_TIPO_REA=:VCC_TIPO_REA, VCC_PAZ_CODICE_ALIAS_CENTRALE=:VCC_PAZ_CODICE_ALIAS_CENTRALE, VCC_USL_CODICE_ALIAS=:VCC_USL_CODICE_ALIAS, VCC_UTE_ID_ALIAS=:VCC_UTE_ID_ALIAS, VCC_DATA_ALIAS =:VCC_DATA_ALIAS, VCC_ID_CONFLITTO=:VCC_ID_CONFLITTO, VCC_UTE_ID_RISOLUZ_CONFLITTO=:VCC_UTE_ID_RISOLUZ_CONFLITTO, VCC_DATA_RISOLUZ_CONFLITTO=:VCC_DATA_RISOLUZ_CONFLITTO, VCC_USL_ULTIMA_OPERAZIONE=:VCC_USL_ULTIMA_OPERAZIONE, VCC_UTE_ID_ULTIMA_OPERAZIONE=:VCC_UTE_ID_ULTIMA_OPERAZIONE, VCC_USL_ULTIMA_OPERAZ_REA=:VCC_USL_ULTIMA_OPERAZ_REA, VCC_UTE_ID_ULTIMA_OPERAZ_REA=:VCC_UTE_ID_ULTIMA_OPERAZ_REA  WHERE VCC_ID=:VCC_ID", Me.Connection)

                Me.AddVaccinazioneEseguitaCentraleInsertOrUpdateParameters(cmd, vaccinazioneEseguitaCentrale)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                cmd.ExecuteNonQuery()

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

        End Sub

        Public Sub UpdateVaccinazioneEseguitaCentraleDistribuita(vaccinazioneEseguitaCentraleDistribuita As Entities.VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita) Implements IVaccinazioneEseguitaCentraleProvider.UpdateVaccinazioneEseguitaCentraleDistribuita

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try
                cmd = New OracleClient.OracleCommand("UPDATE T_VACCINAZIONI_DISTRIBUITE SET VCD_PAZ_CODICE_LOCALE=:VCD_PAZ_CODICE_LOCALE, VCD_VCC_ID=:VCD_VCC_ID, VCD_VES_ID=:VCD_VES_ID, VCD_VRA_ID=:VCD_VRA_ID, VCD_VCC_VRA_ID=:VCD_VCC_VRA_ID, VCD_VCC_USL_INSERIMENTO_REA=:VCD_VCC_USL_INSERIMENTO_REA, VCD_USL_CODICE=:VCD_USL_CODICE, VCD_DATA_INS_LOCALE=:VCD_DATA_INS_LOCALE, VCD_UTE_ID_INS_LOCALE=:VCD_UTE_ID_INS_LOCALE, VCD_DATA_AGG_LOCALE=:VCD_DATA_AGG_LOCALE  WHERE VCD_ID=:VCD_ID", Me.Connection)

                Me.AddVaccinazioneEseguitaCentraleDistribuitaInsertOrUpdateParameters(cmd, vaccinazioneEseguitaCentraleDistribuita)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                cmd.ExecuteNonQuery()

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

        End Sub

        Public Sub DeleteVaccinazioneEseguitaCentraleDistribuita(vaccinazioneEseguitaCentraleDistribuita As Entities.VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita) Implements IVaccinazioneEseguitaCentraleProvider.DeleteVaccinazioneEseguitaCentraleDistribuita

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim ownConnection As Boolean = False

            Try
                cmd = New OracleClient.OracleCommand("DELETE FROM T_VACCINAZIONI_DISTRIBUITE WHERE VCD_ID=:VCD_ID", Me.Connection)

                cmd.Parameters.AddWithValue("VCD_ID", vaccinazioneEseguitaCentraleDistribuita.Id)

                ownConnection = Me.ConditionalOpenConnection(cmd)

                cmd.ExecuteNonQuery()

            Finally

                Me.ConditionalCloseConnection(ownConnection)

                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

        End Sub

#End Region

#End Region

#Region " Private "

        Private Sub AddVaccinazioneEseguitaCentraleInsertOrUpdateParameters(cmd As OracleClient.OracleCommand, vaccinazioneEseguitaCentrale As VaccinazioneEseguitaCentrale)

            cmd.Parameters.AddWithValue("VCC_ID", GetLongParam(vaccinazioneEseguitaCentrale.Id))
            cmd.Parameters.AddWithValue("VCC_PAZ_CODICE_CENTRALE", GetStringParam(vaccinazioneEseguitaCentrale.CodicePazienteCentrale))
            cmd.Parameters.AddWithValue("VCC_PAZ_CODICE_LOCALE", GetLongParam(vaccinazioneEseguitaCentrale.CodicePaziente))
            cmd.Parameters.AddWithValue("VCC_VES_ID", GetLongParam(vaccinazioneEseguitaCentrale.IdVaccinazioneEseguita))
            cmd.Parameters.AddWithValue("VCC_TIPO", GetStringParam(vaccinazioneEseguitaCentrale.TipoVaccinazioneEseguitaCentrale))
            cmd.Parameters.AddWithValue("VCC_VISIBILITA", GetStringParam(vaccinazioneEseguitaCentrale.FlagVisibilitaCentrale))
            cmd.Parameters.AddWithValue("VCC_DATA_REVOCA_VISIBILITA", GetDateParam(vaccinazioneEseguitaCentrale.DataRevocaVisibilita))
            cmd.Parameters.AddWithValue("VCC_USL_INSERIMENTO", GetStringParam(vaccinazioneEseguitaCentrale.CodiceUslVaccinazioneEseguita))
            cmd.Parameters.AddWithValue("VCC_DATA_INSERIMENTO", GetDateParam(vaccinazioneEseguitaCentrale.DataInserimentoVaccinazioneEseguita))
            cmd.Parameters.AddWithValue("VCC_UTE_ID_INSERIMENTO", GetLongParam(vaccinazioneEseguitaCentrale.IdUtenteInserimentoVaccinazioneEseguita))
            cmd.Parameters.AddWithValue("VCC_UTE_ID_VARIAZIONE", GetLongParam(vaccinazioneEseguitaCentrale.IdUtenteModificaVaccinazioneEseguita))
            cmd.Parameters.AddWithValue("VCC_DATA_VARIAZIONE", GetDateParam(vaccinazioneEseguitaCentrale.DataModificaVaccinazioneEseguita))
            cmd.Parameters.AddWithValue("VCC_DATA_ELIMINAZIONE", GetDateParam(vaccinazioneEseguitaCentrale.DataEliminazioneVaccinazioneEseguita))
            cmd.Parameters.AddWithValue("VCC_UTE_ID_ELIMINAZIONE", GetLongParam(vaccinazioneEseguitaCentrale.IdUtenteEliminazioneVaccinazioneEseguita))
            cmd.Parameters.AddWithValue("VCC_DATA_SCADENZA", GetDateParam(vaccinazioneEseguitaCentrale.DataScadenzaVaccinazioneEseguita))
            cmd.Parameters.AddWithValue("VCC_UTE_ID_SCADENZA", GetLongParam(vaccinazioneEseguitaCentrale.IdUtenteScadenzaVaccinazioneEseguita))
            cmd.Parameters.AddWithValue("VCC_USL_SCADENZA", GetStringParam(vaccinazioneEseguitaCentrale.CodiceUslScadenzaVaccinazioneEseguita))
            cmd.Parameters.AddWithValue("VCC_DATA_RIPRISTINO", GetDateParam(vaccinazioneEseguitaCentrale.DataRipristinoVaccinazioneEseguita))
            cmd.Parameters.AddWithValue("VCC_UTE_ID_RIPRISTINO", GetLongParam(vaccinazioneEseguitaCentrale.IdUtenteRipristinoVaccinazioneEseguita))
            cmd.Parameters.AddWithValue("VCC_VRA_ID", GetLongParam(vaccinazioneEseguitaCentrale.IdReazioneAvversa))
            cmd.Parameters.AddWithValue("VCC_USL_INSERIMENTO_REA", GetStringParam(vaccinazioneEseguitaCentrale.CodiceUslReazioneAvversa))
            cmd.Parameters.AddWithValue("VCC_DATA_INSERIMENTO_REA", GetDateParam(vaccinazioneEseguitaCentrale.DataInserimentoReazioneAvversa))
            cmd.Parameters.AddWithValue("VCC_UTE_ID_INSERIMENTO_REA", GetLongParam(vaccinazioneEseguitaCentrale.IdUtenteInserimentoReazioneAvversa))
            cmd.Parameters.AddWithValue("VCC_DATA_VARIAZIONE_REA", GetDateParam(vaccinazioneEseguitaCentrale.DataModificaReazioneAvversa))
            cmd.Parameters.AddWithValue("VCC_UTE_ID_VARIAZIONE_REA", GetLongParam(vaccinazioneEseguitaCentrale.IdUtenteModificaReazioneAvversa))
            cmd.Parameters.AddWithValue("VCC_DATA_ELIMINAZIONE_REA", GetDateParam(vaccinazioneEseguitaCentrale.DataEliminazioneReazioneAvversa))
            cmd.Parameters.AddWithValue("VCC_UTE_ID_ELIMINAZIONE_REA", GetLongParam(vaccinazioneEseguitaCentrale.IdUtenteEliminazioneReazioneAvversa))
            cmd.Parameters.AddWithValue("VCC_TIPO_REA", GetStringParam(vaccinazioneEseguitaCentrale.TipoReazioneAvversaCentrale))
            cmd.Parameters.AddWithValue("VCC_PAZ_CODICE_ALIAS_CENTRALE", GetStringParam(vaccinazioneEseguitaCentrale.MergeInfoCentrale.CodicePazienteCentraleAlias))
            cmd.Parameters.AddWithValue("VCC_USL_CODICE_ALIAS", GetStringParam(vaccinazioneEseguitaCentrale.MergeInfoCentrale.CodiceUslAlias))
            cmd.Parameters.AddWithValue("VCC_UTE_ID_ALIAS", GetLongParam(vaccinazioneEseguitaCentrale.MergeInfoCentrale.IdUtenteAlias))
            cmd.Parameters.AddWithValue("VCC_DATA_ALIAS", GetDateParam(vaccinazioneEseguitaCentrale.MergeInfoCentrale.DataAlias))
            cmd.Parameters.AddWithValue("VCC_ID_CONFLITTO", GetLongParam(vaccinazioneEseguitaCentrale.IdConflitto))
            cmd.Parameters.AddWithValue("VCC_DATA_RISOLUZ_CONFLITTO", GetDateParam(vaccinazioneEseguitaCentrale.DataRisoluzioneConflitto))
            cmd.Parameters.AddWithValue("VCC_UTE_ID_RISOLUZ_CONFLITTO", GetLongParam(vaccinazioneEseguitaCentrale.IdUtenteRisoluzioneConflitto))
            cmd.Parameters.AddWithValue("VCC_USL_ULTIMA_OPERAZIONE", GetStringParam(vaccinazioneEseguitaCentrale.CodiceUslUltimaOperazioneVaccinazioneEseguita))
            cmd.Parameters.AddWithValue("VCC_UTE_ID_ULTIMA_OPERAZIONE", GetLongParam(vaccinazioneEseguitaCentrale.IdUtenteUltimaOperazioneVaccinazioneEseguita))
            cmd.Parameters.AddWithValue("VCC_USL_ULTIMA_OPERAZ_REA", GetStringParam(vaccinazioneEseguitaCentrale.CodiceUslUltimaOperazioneReazioneAvversa))
            cmd.Parameters.AddWithValue("VCC_UTE_ID_ULTIMA_OPERAZ_REA", GetLongParam(vaccinazioneEseguitaCentrale.IdUtenteUltimaOperazioneReazioneAvversa))

        End Sub

        Private Sub AddVaccinazioneEseguitaCentraleDistribuitaInsertOrUpdateParameters(cmd As OracleClient.OracleCommand, vaccinazioneEseguitaDistribuitaCentrale As VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita)

            cmd.Parameters.AddWithValue("VCD_ID", GetLongParam(vaccinazioneEseguitaDistribuitaCentrale.Id))
            cmd.Parameters.AddWithValue("VCD_PAZ_CODICE_LOCALE", GetLongParam(vaccinazioneEseguitaDistribuitaCentrale.CodicePaziente))
            cmd.Parameters.AddWithValue("VCD_VCC_ID", GetLongParam(vaccinazioneEseguitaDistribuitaCentrale.IdVaccinazioneEseguitaCentrale))
            cmd.Parameters.AddWithValue("VCD_VES_ID", GetLongParam(vaccinazioneEseguitaDistribuitaCentrale.IdVaccinazioneEseguita))
            cmd.Parameters.AddWithValue("VCD_VRA_ID", GetLongParam(vaccinazioneEseguitaDistribuitaCentrale.IdReazioneAvversa))
            cmd.Parameters.AddWithValue("VCD_USL_CODICE", GetStringParam(vaccinazioneEseguitaDistribuitaCentrale.CodiceUslVaccinazioneEseguita))
            cmd.Parameters.AddWithValue("VCD_VCC_VRA_ID", GetLongParam(vaccinazioneEseguitaDistribuitaCentrale.IdReazioneAvversaVaccinazioneEseguitaCentrale))
            cmd.Parameters.AddWithValue("VCD_VCC_USL_INSERIMENTO_REA", GetStringParam(vaccinazioneEseguitaDistribuitaCentrale.CodiceUslReazioneAvversaVaccinazioneEseguitaCentrale))
            cmd.Parameters.AddWithValue("VCD_DATA_INS_LOCALE", GetDateParam(vaccinazioneEseguitaDistribuitaCentrale.DataInserimentoVaccinazioneEseguita))
            cmd.Parameters.AddWithValue("VCD_UTE_ID_INS_LOCALE", GetLongParam(vaccinazioneEseguitaDistribuitaCentrale.IdUtenteInserimentoVaccinazioneEseguita))
            cmd.Parameters.AddWithValue("VCD_DATA_AGG_LOCALE", GetDateParam(vaccinazioneEseguitaDistribuitaCentrale.DataAggiornamentoVaccinazioneEseguita))

        End Sub

        Private Function GetVaccinazioneEseguitaCentraleListFromReader(idr As OracleClient.OracleDataReader) As List(Of VaccinazioneEseguitaCentrale)

            Dim vaccinazioneEseguitaCentraleList As New List(Of VaccinazioneEseguitaCentrale)

            If idr.HasRows Then

                Dim VCC_ID_ordinal As Int16 = idr.GetOrdinal("VCC_ID")
                Dim VCC_PAZ_CODICE_CENTRALE_ordinal As Int16 = idr.GetOrdinal("VCC_PAZ_CODICE_CENTRALE")
                Dim VCC_PAZ_CODICE_LOCALE_ordinal As Int16 = idr.GetOrdinal("VCC_PAZ_CODICE_LOCALE")
                Dim VCC_VES_ID_ordinal As Int16 = idr.GetOrdinal("VCC_VES_ID")
                Dim VCC_TIPO_ordinal As Int16 = idr.GetOrdinal("VCC_TIPO")
                Dim VCC_VISIBILITA_ordinal As Int16 = idr.GetOrdinal("VCC_VISIBILITA")
                Dim VCC_DATA_REVOCA_VISIBILITA_ordinal As Int16 = idr.GetOrdinal("VCC_DATA_REVOCA_VISIBILITA")
                Dim VCC_USL_INSERIMENTO_ordinal As Int16 = idr.GetOrdinal("VCC_USL_INSERIMENTO")
                Dim VCC_DATA_INSERIMENTO_ordinal As Int16 = idr.GetOrdinal("VCC_DATA_INSERIMENTO")
                Dim VCC_UTE_ID_INSERIMENTO_ordinal As Int16 = idr.GetOrdinal("VCC_UTE_ID_INSERIMENTO")
                Dim VCC_UTE_ID_VARIAZIONE_ordinal As Int16 = idr.GetOrdinal("VCC_UTE_ID_VARIAZIONE")
                Dim VCC_DATA_VARIAZIONE_ordinal As Int16 = idr.GetOrdinal("VCC_DATA_VARIAZIONE")
                Dim VCC_DATA_ELIMINAZIONE_ordinal As Int16 = idr.GetOrdinal("VCC_DATA_ELIMINAZIONE")
                Dim VCC_UTE_ID_ELIMINAZIONE_ordinal As Int16 = idr.GetOrdinal("VCC_UTE_ID_ELIMINAZIONE")
                Dim VCC_DATA_SCADENZA_ordinal As Int16 = idr.GetOrdinal("VCC_DATA_SCADENZA")
                Dim VCC_UTE_ID_SCADENZA_ordinal As Int16 = idr.GetOrdinal("VCC_UTE_ID_SCADENZA")
                Dim VCC_USL_SCADENZA_ordinal As Int16 = idr.GetOrdinal("VCC_USL_SCADENZA")
                Dim VCC_DATA_RIPRISTINO_ordinal As Int16 = idr.GetOrdinal("VCC_DATA_RIPRISTINO")
                Dim VCC_UTE_ID_RIPRISTINO_ordinal As Int16 = idr.GetOrdinal("VCC_UTE_ID_RIPRISTINO")
                Dim VCC_VRA_ID_ordinal As Int16 = idr.GetOrdinal("VCC_VRA_ID")
                Dim VCC_TIPO_REA_ordinal As Int16 = idr.GetOrdinal("VCC_TIPO_REA")
                Dim VCC_USL_INSERIMENTO_REA_ordinal As Int16 = idr.GetOrdinal("VCC_USL_INSERIMENTO_REA")
                Dim VCC_DATA_INSERIMENTO_REA_ordinal As Int16 = idr.GetOrdinal("VCC_DATA_INSERIMENTO_REA")
                Dim VCC_UTE_ID_INSERIMENTO_REA_ordinal As Int16 = idr.GetOrdinal("VCC_UTE_ID_INSERIMENTO_REA")
                Dim VCC_DATA_VARIAZIONE_REA_ordinal As Int16 = idr.GetOrdinal("VCC_DATA_VARIAZIONE_REA")
                Dim VCC_UTE_ID_VARIAZIONE_REA_ordinal As Int16 = idr.GetOrdinal("VCC_UTE_ID_VARIAZIONE_REA")
                Dim VCC_DATA_ELIMINAZIONE_REA_ordinal As Int16 = idr.GetOrdinal("VCC_DATA_ELIMINAZIONE_REA")
                Dim VCC_UTE_ID_ELIMINAZIONE_REA_ordinal As Int16 = idr.GetOrdinal("VCC_UTE_ID_ELIMINAZIONE_REA")
                Dim VCC_PAZ_CODICE_ALIAS_CENTRALE_ordinal As Int16 = idr.GetOrdinal("VCC_PAZ_CODICE_ALIAS_CENTRALE")
                Dim VCC_USL_CODICE_ALIAS_ordinal As Int16 = idr.GetOrdinal("VCC_USL_CODICE_ALIAS")
                Dim VCC_UTE_ID_ALIAS_ordinal As Int16 = idr.GetOrdinal("VCC_UTE_ID_ALIAS")
                Dim VCC_DATA_ALIAS_ordinal As Int16 = idr.GetOrdinal("VCC_DATA_ALIAS")
                Dim VCC_ID_CONFLITTO_ordinal As Int16 = idr.GetOrdinal("VCC_ID_CONFLITTO")
                Dim VCC_DATA_RISOLUZ_CONFLITTO_ordinal As Int16 = idr.GetOrdinal("VCC_DATA_RISOLUZ_CONFLITTO")
                Dim VCC_UTE_ID_RISOLUZ_CONFLITTO_ordinal As Int16 = idr.GetOrdinal("VCC_UTE_ID_RISOLUZ_CONFLITTO")
                Dim VCC_USL_ULTIMA_OPERAZIONE_ordinal As Int16 = idr.GetOrdinal("VCC_USL_ULTIMA_OPERAZIONE")
                Dim VCC_UTE_ID_ULTIMA_OPERAZIONE_ordinal As Int16 = idr.GetOrdinal("VCC_UTE_ID_ULTIMA_OPERAZIONE")
                Dim VCC_USL_ULTIMA_OPERAZ_REA_ordinal As Int16 = idr.GetOrdinal("VCC_USL_ULTIMA_OPERAZ_REA")
                Dim VCC_UTE_ID_ULTIMA_OPERAZ_REA_ordinal As Int16 = idr.GetOrdinal("VCC_UTE_ID_ULTIMA_OPERAZ_REA")

                While idr.Read()

                    Dim vaccinazioneEseguitaCentrale As New VaccinazioneEseguitaCentrale()

                    vaccinazioneEseguitaCentrale.Id = idr.GetInt64(VCC_ID_ordinal)
                    vaccinazioneEseguitaCentrale.CodicePazienteCentrale = idr.GetString(VCC_PAZ_CODICE_CENTRALE_ordinal)
                    vaccinazioneEseguitaCentrale.CodicePaziente = idr.GetInt64(VCC_PAZ_CODICE_LOCALE_ordinal)
                    vaccinazioneEseguitaCentrale.IdVaccinazioneEseguita = idr.GetInt64(VCC_VES_ID_ordinal)
                    vaccinazioneEseguitaCentrale.TipoVaccinazioneEseguitaCentrale = idr.GetStringOrDefault(VCC_TIPO_ordinal)
                    vaccinazioneEseguitaCentrale.FlagVisibilitaCentrale = idr.GetString(VCC_VISIBILITA_ordinal)
                    vaccinazioneEseguitaCentrale.DataRevocaVisibilita = idr.GetDateTimeOrDefault(VCC_DATA_REVOCA_VISIBILITA_ordinal)
                    vaccinazioneEseguitaCentrale.CodiceUslVaccinazioneEseguita = idr.GetString(VCC_USL_INSERIMENTO_ordinal)
                    vaccinazioneEseguitaCentrale.DataInserimentoVaccinazioneEseguita = idr.GetDateTime(VCC_DATA_INSERIMENTO_ordinal)
                    vaccinazioneEseguitaCentrale.IdUtenteInserimentoVaccinazioneEseguita = idr.GetInt64(VCC_UTE_ID_INSERIMENTO_ordinal)
                    vaccinazioneEseguitaCentrale.DataModificaVaccinazioneEseguita = idr.GetNullableDateTimeOrDefault(VCC_DATA_VARIAZIONE_ordinal)
                    vaccinazioneEseguitaCentrale.IdUtenteModificaVaccinazioneEseguita = idr.GetNullableInt64OrDefault(VCC_UTE_ID_VARIAZIONE_ordinal)
                    vaccinazioneEseguitaCentrale.DataEliminazioneVaccinazioneEseguita = idr.GetNullableDateTimeOrDefault(VCC_DATA_ELIMINAZIONE_ordinal)
                    vaccinazioneEseguitaCentrale.IdUtenteEliminazioneVaccinazioneEseguita = idr.GetNullableInt64OrDefault(VCC_UTE_ID_ELIMINAZIONE_ordinal)
                    vaccinazioneEseguitaCentrale.DataScadenzaVaccinazioneEseguita = idr.GetNullableDateTimeOrDefault(VCC_DATA_SCADENZA_ordinal)
                    vaccinazioneEseguitaCentrale.IdUtenteScadenzaVaccinazioneEseguita = idr.GetNullableInt64OrDefault(VCC_UTE_ID_SCADENZA_ordinal)
                    vaccinazioneEseguitaCentrale.CodiceUslScadenzaVaccinazioneEseguita = idr.GetStringOrDefault(VCC_USL_SCADENZA_ordinal)
                    vaccinazioneEseguitaCentrale.DataRipristinoVaccinazioneEseguita = idr.GetNullableDateTimeOrDefault(VCC_DATA_RIPRISTINO_ordinal)
                    vaccinazioneEseguitaCentrale.IdUtenteRipristinoVaccinazioneEseguita = idr.GetNullableInt64OrDefault(VCC_UTE_ID_RIPRISTINO_ordinal)
                    vaccinazioneEseguitaCentrale.IdReazioneAvversa = idr.GetNullableInt64OrDefault(VCC_VRA_ID_ordinal)
                    vaccinazioneEseguitaCentrale.TipoReazioneAvversaCentrale = idr.GetStringOrDefault(VCC_TIPO_REA_ordinal)
                    vaccinazioneEseguitaCentrale.CodiceUslReazioneAvversa = idr.GetStringOrDefault(VCC_USL_INSERIMENTO_REA_ordinal)
                    vaccinazioneEseguitaCentrale.DataInserimentoReazioneAvversa = idr.GetNullableDateTimeOrDefault(VCC_DATA_INSERIMENTO_REA_ordinal)
                    vaccinazioneEseguitaCentrale.IdUtenteInserimentoReazioneAvversa = idr.GetNullableInt64OrDefault(VCC_UTE_ID_INSERIMENTO_REA_ordinal)
                    vaccinazioneEseguitaCentrale.DataModificaReazioneAvversa = idr.GetNullableDateTimeOrDefault(VCC_DATA_VARIAZIONE_REA_ordinal)
                    vaccinazioneEseguitaCentrale.IdUtenteModificaReazioneAvversa = idr.GetNullableInt64OrDefault(VCC_UTE_ID_VARIAZIONE_REA_ordinal)
                    vaccinazioneEseguitaCentrale.DataEliminazioneReazioneAvversa = idr.GetNullableDateTimeOrDefault(VCC_DATA_ELIMINAZIONE_REA_ordinal)
                    vaccinazioneEseguitaCentrale.IdUtenteEliminazioneReazioneAvversa = idr.GetNullableInt64OrDefault(VCC_UTE_ID_ELIMINAZIONE_REA_ordinal)
                    vaccinazioneEseguitaCentrale.MergeInfoCentrale.CodicePazienteCentraleAlias = idr.GetStringOrDefault(VCC_PAZ_CODICE_ALIAS_CENTRALE_ordinal)
                    vaccinazioneEseguitaCentrale.MergeInfoCentrale.CodiceUslAlias = idr.GetStringOrDefault(VCC_USL_CODICE_ALIAS_ordinal)
                    vaccinazioneEseguitaCentrale.MergeInfoCentrale.IdUtenteAlias = idr.GetNullableInt64OrDefault(VCC_UTE_ID_ALIAS_ordinal)
                    vaccinazioneEseguitaCentrale.MergeInfoCentrale.DataAlias = idr.GetNullableDateTimeOrDefault(VCC_DATA_ALIAS_ordinal)
                    vaccinazioneEseguitaCentrale.IdConflitto = idr.GetNullableInt64OrDefault(VCC_ID_CONFLITTO_ordinal)
                    vaccinazioneEseguitaCentrale.DataRisoluzioneConflitto = idr.GetNullableDateTimeOrDefault(VCC_DATA_RISOLUZ_CONFLITTO_ordinal)
                    vaccinazioneEseguitaCentrale.IdUtenteRisoluzioneConflitto = idr.GetNullableInt64OrDefault(VCC_UTE_ID_RISOLUZ_CONFLITTO_ordinal)
                    vaccinazioneEseguitaCentrale.CodiceUslUltimaOperazioneVaccinazioneEseguita = idr.GetStringOrDefault(VCC_USL_ULTIMA_OPERAZIONE_ordinal)
                    vaccinazioneEseguitaCentrale.IdUtenteUltimaOperazioneVaccinazioneEseguita = idr.GetInt64OrDefault(VCC_UTE_ID_ULTIMA_OPERAZIONE_ordinal)
                    vaccinazioneEseguitaCentrale.CodiceUslUltimaOperazioneReazioneAvversa = idr.GetStringOrDefault(VCC_USL_ULTIMA_OPERAZ_REA_ordinal)
                    vaccinazioneEseguitaCentrale.IdUtenteUltimaOperazioneReazioneAvversa = idr.GetNullableInt64OrDefault(VCC_UTE_ID_ULTIMA_OPERAZ_REA_ordinal)

                    vaccinazioneEseguitaCentraleList.Add(vaccinazioneEseguitaCentrale)

                End While

            End If

            Return vaccinazioneEseguitaCentraleList

        End Function



        Private Function GetVaccinazioneEseguitaCentraleDistribuitaListFromReader(idr As OracleClient.OracleDataReader) As List(Of VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita)

            Dim vaccinazioneEseguitaDistribuitaList As New List(Of VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita)

            If idr.HasRows Then

                Dim vcd_id_ordinal As Int16 = idr.GetOrdinal("VCD_ID")
                Dim vcd_paz_codice_locale_ordinal As Int16 = idr.GetOrdinal("VCD_PAZ_CODICE_LOCALE")
                Dim vcd_vcc_id_ordinal As Int16 = idr.GetOrdinal("VCD_VCC_ID")
                Dim vcd_ves_id_ordinal As Int16 = idr.GetOrdinal("VCD_VES_ID")
                Dim vcd_vra_id_ordinal As Int16 = idr.GetOrdinal("VCD_VRA_ID")
                Dim vcd_usl_codice_ordinal As Int16 = idr.GetOrdinal("VCD_USL_CODICE")
                Dim vcd_vcc_vra_id_ordinal As Int16 = idr.GetOrdinal("VCD_VCC_VRA_ID")
                Dim vcd_vcc_usl_inserimento_rea_ordinal As Int16 = idr.GetOrdinal("VCD_VCC_USL_INSERIMENTO_REA")
                Dim vcd_data_ins_locale_ordinal As Int16 = idr.GetOrdinal("VCD_DATA_INS_LOCALE")
                Dim vcd_ute_id_ins_locale_ordinal As Int16 = idr.GetOrdinal("VCD_UTE_ID_INS_LOCALE")
                Dim vcd_data_agg_locale_ordinal As Int16 = idr.GetOrdinal("VCD_DATA_AGG_LOCALE")

                While idr.Read()

                    Dim vaccinazioneEseguitaDistribuita As New VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita()

                    vaccinazioneEseguitaDistribuita.Id = idr.GetInt64(vcd_id_ordinal)
                    vaccinazioneEseguitaDistribuita.IdVaccinazioneEseguitaCentrale = idr.GetInt64(vcd_vcc_id_ordinal)
                    vaccinazioneEseguitaDistribuita.CodicePaziente = idr.GetInt64(vcd_paz_codice_locale_ordinal)
                    vaccinazioneEseguitaDistribuita.IdVaccinazioneEseguita = idr.GetInt64(vcd_ves_id_ordinal)
                    vaccinazioneEseguitaDistribuita.IdReazioneAvversa = idr.GetNullableInt64OrDefault(vcd_vra_id_ordinal)
                    vaccinazioneEseguitaDistribuita.CodiceUslVaccinazioneEseguita = idr.GetString(vcd_usl_codice_ordinal)
                    vaccinazioneEseguitaDistribuita.IdReazioneAvversaVaccinazioneEseguitaCentrale = idr.GetNullableInt64OrDefault(vcd_vcc_vra_id_ordinal)
                    vaccinazioneEseguitaDistribuita.CodiceUslReazioneAvversaVaccinazioneEseguitaCentrale = idr.GetStringOrDefault(vcd_vcc_usl_inserimento_rea_ordinal)
                    vaccinazioneEseguitaDistribuita.DataInserimentoVaccinazioneEseguita = idr.GetDateTime(vcd_data_ins_locale_ordinal)
                    vaccinazioneEseguitaDistribuita.IdUtenteInserimentoVaccinazioneEseguita = idr.GetInt64(vcd_ute_id_ins_locale_ordinal)
                    vaccinazioneEseguitaDistribuita.DataAggiornamentoVaccinazioneEseguita = idr.GetNullableDateTimeOrDefault(vcd_data_agg_locale_ordinal)

                    vaccinazioneEseguitaDistribuitaList.Add(vaccinazioneEseguitaDistribuita)

                End While

            End If

            Return vaccinazioneEseguitaDistribuitaList

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

            query.AppendFormat(" AND VCC_VISIBILITA IN ({0}) ", filtroVisibilita)

            Return query.ToString()

        End Function

        Private Function GetFiltroVaccinazioniEliminate(noEliminate As Boolean, cmd As OracleClient.OracleCommand) As String

            Dim query As String = String.Empty

            If noEliminate Then

                query = " AND (VCC_TIPO IS NULL OR (VCC_TIPO <> :VCC_TIPO_ESEGUITA_ELIMINATA AND VCC_TIPO <> :VCC_TIPO_SCADUTA_ELIMINATA)) "

                cmd.Parameters.AddWithValue("VCC_TIPO_ESEGUITA_ELIMINATA", Constants.TipoVaccinazioneEseguitaCentrale.Eliminata)
                cmd.Parameters.AddWithValue("VCC_TIPO_SCADUTA_ELIMINATA", Constants.TipoVaccinazioneEseguitaCentrale.ScadutaEliminata)

            End If

            Return query

        End Function

        Private Function GetFiltroReazioniEliminate(noEliminate As Boolean, cmd As OracleClient.OracleCommand) As String

            Dim query As String = String.Empty

            If noEliminate Then

                query = " AND (VCC_TIPO_REA IS NULL OR VCC_TIPO_REA <> :VCC_TIPO_REA) "

                cmd.Parameters.AddWithValue("VCC_TIPO_REA", Constants.TipoReazioneAvversaCentrale.Eliminata)

            End If

            Return query

        End Function

#End Region

    End Class

End Namespace
