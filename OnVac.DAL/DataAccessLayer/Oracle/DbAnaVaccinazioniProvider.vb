Imports System.Data
Imports System.Data.OracleClient
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Collections.Generic
Imports System.Linq

Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.Entities


Namespace DAL.Oracle

    Public Class DbAnaVaccinazioniProvider
        Inherits DbProvider
        Implements IAnaVaccinazioniProvider

#Region " Costruttori "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " Metodi di Select "

        Public Function GetVaccinazioni(obbligatorietaVaccinazioni As List(Of String)) As DataTable Implements IAnaVaccinazioniProvider.GetVaccinazioni

            With _DAM.QB

                .NewQuery()
                .AddSelectFields("*")
                .AddTables("t_ana_vaccinazioni")

                If Not obbligatorietaVaccinazioni Is Nothing AndAlso obbligatorietaVaccinazioni.Count > 0 Then

                    Dim s As New StringBuilder()

                    For i As Integer = 0 To obbligatorietaVaccinazioni.Count - 1
                        s.AppendFormat("{0},", .AddCustomParam(obbligatorietaVaccinazioni(i)))
                    Next

                    If s.Length > 0 Then s.Remove(s.Length - 1, 1)

                    .AddWhereCondition("vac_obbligatoria", Comparatori.In, s, DataTypes.Stringa)

                End If

                .AddOrderByFields("vac_ordine", "vac_codice")

            End With

            _DAM.BuildDataTable(_DT)

            Return _DT

        End Function

        Public Function GetDataTableCodiceDescrizioneVaccinazioni(campoOrdinamento As String) As DataTable Implements IAnaVaccinazioniProvider.GetDataTableCodiceDescrizioneVaccinazioni

            Dim dtVaccini As New DataTable()

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("VAC_CODICE, VAC_DESCRIZIONE")
                .AddTables("T_ANA_VACCINAZIONI")
                If Not String.IsNullOrEmpty(campoOrdinamento) Then
                    .AddOrderByFields(campoOrdinamento)
                End If
            End With

            _DAM.BuildDataTable(dtVaccini)

            Return dtVaccini

        End Function

        Public Function IsVaccinazioneObbligatoria(codiceVaccinazione As String) As Boolean Implements IAnaVaccinazioniProvider.IsVaccinazioneObbligatoria

            Dim isObbligatoria As Boolean = False

            With _DAM.QB

                .NewQuery()
                .AddTables("T_ANA_VACCINAZIONI")
                .AddSelectFields("VAC_OBBLIGATORIA")
                .AddWhereCondition("VAC_CODICE", Comparatori.Uguale, codiceVaccinazione, DataTypes.Stringa)

            End With

            Dim val As Object = _DAM.ExecScalar()

            If val Is Nothing OrElse val Is DBNull.Value Then

                isObbligatoria = False

            Else

                isObbligatoria = (val.ToString() = Constants.ObbligatorietaVaccinazione.Obbligatoria)

            End If

            Return isObbligatoria

        End Function

        Public Function GetDataTableVaccinazioniSostitute() As DataTable Implements IAnaVaccinazioniProvider.GetDataTableVaccinazioniSostitute

            Dim dt As New DataTable()

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("VAC_CODICE, VAC_DESCRIZIONE, VAC_COD_SOSTITUTA")
                .AddTables("T_ANA_VACCINAZIONI")
            End With

            _DAM.BuildDataTable(dt)

            Return dt

        End Function

        ''' <summary>
        ''' Restituisce la descrizione della vaccinazione in base al codice
        ''' </summary>
        ''' <param name="codiceVaccinazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDescrizioneVaccinazione(codiceVaccinazione As String) As String Implements IAnaVaccinazioniProvider.GetDescrizioneVaccinazione

            Dim descrizione As String = String.Empty

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand("SELECT VAC_DESCRIZIONE FROM T_ANA_VACCINAZIONI WHERE VAC_CODICE = :VAC_CODICE", Me.Connection)

                    cmd.Parameters.AddWithValue("VAC_CODICE", codiceVaccinazione)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()
                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        descrizione = obj.ToString()
                    End If

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return descrizione

        End Function

        ''' <summary>
        ''' Restituisce una lista di codice e descrizione per tutte le vaccinazioni in anagrafica
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCodiceDescrizioneVaccinazioni() As List(Of KeyValuePair(Of String, String)) Implements IAnaVaccinazioniProvider.GetCodiceDescrizioneVaccinazioni

            Return GetListCodiceDescrizioneVaccinazioni(Nothing, True, True)

        End Function

        ''' <summary>
        ''' Restituisce una lista di codice e descrizione per le vaccinazioni specificate
        ''' </summary>
        ''' <param name="codiciVaccinazioni"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCodiceDescrizioneVaccinazioni(codiciVaccinazioni As List(Of String)) As List(Of KeyValuePair(Of String, String)) Implements IAnaVaccinazioniProvider.GetCodiceDescrizioneVaccinazioni

            Return GetListCodiceDescrizioneVaccinazioni(codiciVaccinazioni, True, True)

        End Function

        ''' <summary>
        ''' Restituisce una lista di (codice, descrizione) per tutte le vaccinazioni dell'anagrafe.
        ''' In base ai parametri, vengono incluse o escluse le obsolete e quelle che hanno associato il codice di una vaccinazione sostituta.
        ''' N.B. : l'obsolescenza delle vaccinazioni è prevista ma non è gestita!
        ''' </summary>
        ''' <param name="includiObsolete"></param>
        ''' <param name="includiSostituite"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCodiceDescrizioneVaccinazioni(includiObsolete As Boolean, includiSostituite As Boolean) As List(Of KeyValuePair(Of String, String)) Implements IAnaVaccinazioniProvider.GetCodiceDescrizioneVaccinazioni

            Return GetListCodiceDescrizioneVaccinazioni(Nothing, includiObsolete, includiSostituite)

        End Function

        Private Function GetListCodiceDescrizioneVaccinazioni(codiciVaccinazioni As List(Of String), includiObsolete As Boolean, includiSostituite As Boolean) As List(Of KeyValuePair(Of String, String))

            Dim list As New List(Of KeyValuePair(Of String, String))()

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand()

                    cmd.Connection = Me.Connection

                    Dim filtroVaccinazioni As New System.Text.StringBuilder()

                    If Not codiciVaccinazioni.IsNullOrEmpty() Then

                        If codiciVaccinazioni.Count = 1 Then
                            filtroVaccinazioni.Append(" vac_codice = :vac_codice and ")
                            cmd.Parameters.AddWithValue("vac_codice", codiciVaccinazioni.Single())
                        Else
                            Dim filter As GetInFilterResult = GetInFilter(codiciVaccinazioni)
                            filtroVaccinazioni.AppendFormat(" vac_codice in ({0}) and ", filter.InFilter)
                            cmd.Parameters.AddRange(filter.Parameters)
                        End If

                    End If

                    If Not includiObsolete Then
                        'filtroVaccinazioni.Append(" (vac_obsoleto is null or vac_obsoleto = 'N') and ")
                    End If

                    If Not includiSostituite Then
                        filtroVaccinazioni.Append(" vac_cod_sostituta is null and ")
                    End If

                    If filtroVaccinazioni.Length > 0 Then
                        filtroVaccinazioni.RemoveLast(4)
                        filtroVaccinazioni.Insert(0, " where ")
                    End If

                    cmd.CommandText = String.Format("select vac_codice, vac_descrizione from t_ana_vaccinazioni {0} order by vac_ordine asc", filtroVaccinazioni.ToString())

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As OracleDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim vac_codice As Integer = idr.GetOrdinal("vac_codice")
                            Dim vac_descrizione As Integer = idr.GetOrdinal("vac_descrizione")

                            While idr.Read()

                                list.Add(New KeyValuePair(Of String, String)(
                                    idr.GetString(vac_codice),
                                    idr.GetString(vac_descrizione))
                                )

                            End While

                        End If
                    End Using
                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return list

        End Function

        'Public Function ExistsVaccinazioneInfluenzale(codiceVac As String) As Boolean Implements IAnaVaccinazioniProvider.ExistsVaccinazioneInfluenzale

        '    Dim exists As Boolean = False

        '    Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

        '        cmd.CommandText = "SELECT 1 FROM T_ANA_VACCINAZIONI WHERE VAC_FLAG_INFLUENZALE = 'S' AND VAC_CODICE = :codiceVac "

        '        cmd.Parameters.AddWithValue("codiceVac", codiceVac)

        '        Dim ownConnection As Boolean = False

        '        Try
        '            ownConnection = Me.ConditionalOpenConnection(cmd)

        '            Dim obj As Object = cmd.ExecuteScalar()

        '            exists = (obj IsNot Nothing AndAlso obj IsNot DBNull.Value)

        '        Finally
        '            Me.ConditionalCloseConnection(ownConnection)
        '        End Try

        '    End Using

        '    Return exists

        'End Function

        ''' <summary>
        ''' Restituisce una lista di oggetti BilancioVaccinazione, per tutte le vaccinazioni presenti in anagrafica
        ''' </summary>
        ''' <param name="listCodiciVaccinazioni"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetListBilancioVaccinazioneFromAnagrafica() As List(Of Entities.BilancioVaccinazione) Implements IAnaVaccinazioniProvider.GetListBilancioVaccinazioneFromAnagrafica

            Dim list As New List(Of Entities.BilancioVaccinazione)()

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand("select vac_codice, vac_descrizione, vac_ordine from t_ana_vaccinazioni order by vac_ordine asc", Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As OracleDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim vac_codice As Integer = idr.GetOrdinal("vac_codice")
                            Dim vac_descrizione As Integer = idr.GetOrdinal("vac_descrizione")
                            Dim vac_ordine As Integer = idr.GetOrdinal("vac_ordine")

                            While idr.Read()

                                Dim item As New Entities.BilancioVaccinazione()
                                item.Codice = idr.GetString(vac_codice)
                                item.Descrizione = idr.GetString(vac_descrizione)
                                item.Ordine = idr.GetInt32OrDefault(vac_ordine)

                                ' N.B. : item.Dose  => è solo per le selezionate, in anagrafica non c'è!

                                list.Add(item)

                            End While

                        End If
                    End Using
                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return list

        End Function

        ''' <summary>
        ''' Restituisce il codice Avn della vaccinazione e il tipo (A: Associziazione, V: Vaccinazione)
        ''' </summary>
        ''' <param name="codiceVac"></param>
        ''' <returns></returns>
        Public Function GetCodiceAvnVaccinazione(codiceVac As String) As Entities.VaccinazioneAVN Implements IAnaVaccinazioniProvider.GetCodiceAvnVaccinazione

            Dim result As New Entities.VaccinazioneAVN()

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand()
                    cmd.Connection = Me.Connection

                    Dim query As String = "select vac_codice_avn, vac_tipo_codice_avn from t_ana_vaccinazioni where vac_codice = :vac_codice and vac_flag_estrai_avn = 'S' "
                    cmd.Parameters.AddWithValue("vac_codice", codiceVac)

                    cmd.CommandText = query

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As OracleDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim vac_codice_avn As Integer = idr.GetOrdinal("vac_codice_avn")
                            Dim vac_tipo_codice_avn As Integer = idr.GetOrdinal("vac_tipo_codice_avn")

                            While idr.Read()

                                result.CodiceAvn = idr.GetStringOrDefault(vac_codice_avn)
                                result.TipoCodiceAvn = idr.GetStringOrDefault(vac_tipo_codice_avn)

                            End While

                        End If
                    End Using
                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return result

        End Function

#End Region

#Region " VaccinazioneInfo "

        Public Function GetVaccinazioneInfo(codiceVaccinazione As String) As Entities.VaccinazioneInfo Implements IAnaVaccinazioniProvider.GetVaccinazioneInfo

            Dim info As New Entities.VaccinazioneInfo()

            Dim ownConnection As Boolean = False

            Dim query As String =
                "select vai_id, vai_titolo, vai_descrizione, vac_codice, vac_descrizione " +
                "from t_ana_vaccinazioni " +
                "left join t_ana_vaccinazioni_info on vac_codice=vai_vac_codice " +
                "where vac_codice = :codiceVaccinazione"

            Try
                Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("codiceVaccinazione", codiceVaccinazione)

                    Using idr As OracleDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim vai_id As Integer = idr.GetOrdinal("vai_id")
                            Dim vai_titolo As Integer = idr.GetOrdinal("vai_titolo")
                            Dim vai_descrizione As Integer = idr.GetOrdinal("vai_descrizione")
                            Dim vac_codice As Integer = idr.GetOrdinal("vac_codice")
                            Dim vac_descrizione As Integer = idr.GetOrdinal("vac_descrizione")

                            If idr.Read() Then

                                info.Id = idr.GetNullableInt32OrDefault(vai_id)
                                info.CodiceVaccinazione = idr.GetStringOrDefault(vac_codice)
                                info.DescrizioneVaccinazione = idr.GetStringOrDefault(vac_descrizione)
                                info.Titolo = idr.GetStringOrDefault(vai_titolo)

                                Dim clob As OracleLob = idr.GetOracleLob(vai_descrizione)

                                If Not clob.IsNull AndAlso clob.CanRead() Then
                                    info.Descrizione = clob.Value
                                Else
                                    info.Descrizione = String.Empty
                                End If

                            End If
                        End If

                    End Using
                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return info

        End Function

        Public Function InsertVaccinazioneInfo(info As Entities.VaccinazioneInfo) As Integer Implements IAnaVaccinazioniProvider.InsertVaccinazioneInfo

            Dim count As Integer = 0

            Dim ownConnection As Boolean = False

            Dim query As String =
                "insert into t_ana_vaccinazioni_info (vai_vac_codice, vai_titolo, vai_descrizione) " +
                "values (:vai_vac_codice, :vai_titolo, :vai_descrizione)"

            Try
                Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("vai_vac_codice", info.CodiceVaccinazione)

                    If String.IsNullOrWhiteSpace(info.Titolo) Then
                        cmd.Parameters.AddWithValue("vai_titolo", System.DBNull.Value)
                    Else
                        cmd.Parameters.AddWithValue("vai_titolo", info.Titolo)
                    End If

                    If String.IsNullOrWhiteSpace(info.Descrizione) Then
                        cmd.Parameters.AddWithValue("vai_descrizione", System.DBNull.Value)
                    Else
                        cmd.Parameters.AddWithValue("vai_descrizione", info.Descrizione)
                    End If

                    count = cmd.ExecuteNonQuery()

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return count

        End Function

        Public Function UpdateVaccinazioneInfo(info As Entities.VaccinazioneInfo) As Integer Implements IAnaVaccinazioniProvider.UpdateVaccinazioneInfo

            Dim count As Integer = 0

            Dim ownConnection As Boolean = False

            Dim query As String =
                "update t_ana_vaccinazioni_info " +
                "set vai_titolo = :vai_titolo, " +
                "vai_descrizione = :vai_descrizione " +
                "where vai_id = :vai_id"

            Try
                Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    If String.IsNullOrWhiteSpace(info.Titolo) Then
                        cmd.Parameters.AddWithValue("vai_titolo", System.DBNull.Value)
                    Else
                        cmd.Parameters.AddWithValue("vai_titolo", info.Titolo)
                    End If

                    If String.IsNullOrWhiteSpace(info.Descrizione) Then
                        cmd.Parameters.AddWithValue("vai_descrizione", System.DBNull.Value)
                    Else
                        cmd.Parameters.AddWithValue("vai_descrizione", info.Descrizione)
                    End If

                    cmd.Parameters.AddWithValue("vai_id", info.Id.Value)

                    count = cmd.ExecuteNonQuery()

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return count

        End Function

        Public Function DeleteVaccinazioneInfo(idInfo As Integer) As Integer Implements IAnaVaccinazioniProvider.DeleteVaccinazioneInfo

            Dim count As Integer = 0

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand("delete from t_ana_vaccinazioni_info where vai_id = :vai_id", Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("vai_id", idInfo)
                    count = cmd.ExecuteNonQuery()

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return count

        End Function

#End Region

#Region " Codifica esterna vaccinazione - associazione "

        ''' <summary>
        ''' Restituisce le codifiche per associazione relative alla vaccinazione specificata
        ''' </summary>
        ''' <param name="codiceVaccinazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetListCodiciEsterniVaccinazioneAssociazione(codiceVaccinazione As String) As List(Of Entities.CodificaEsternaVaccinazioneAssociazione) Implements IAnaVaccinazioniProvider.GetListCodiciEsterniVaccinazioneAssociazione

            Dim list As New List(Of Entities.CodificaEsternaVaccinazioneAssociazione)()

            Dim ownConnection As Boolean = False

            Try
                Dim query As String =
                "SELECT VAL_VAC_CODICE, VAL_ASS_CODICE, ASS_DESCRIZIONE, NVL(ASS_OBSOLETO,'N') ASS_OBSOLETO, VCA_ASS_CODICE, VCA_CODICE_AVN " +
                "FROM T_ANA_LINK_ASS_VACCINAZIONI " +
                "JOIN T_ANA_ASSOCIAZIONI ON VAL_ASS_CODICE = ASS_CODICE " +
                "LEFT JOIN T_VAC_CODICE_ESTERNO_ASS ON VAL_VAC_CODICE=VCA_VAC_CODICE AND VAL_ASS_CODICE = VCA_ASS_CODICE " +
                "WHERE VAL_VAC_CODICE = :VAL_VAC_CODICE " +
                "ORDER BY VCA_ASS_CODICE NULLS LAST, ASS_OBSOLETO, VAL_ASS_CODICE "

                Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("VAL_VAC_CODICE", codiceVaccinazione)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim VAL_VAC_CODICE As Integer = idr.GetOrdinal("VAL_VAC_CODICE")
                            Dim VAL_ASS_CODICE As Integer = idr.GetOrdinal("VAL_ASS_CODICE")
                            Dim ASS_DESCRIZIONE As Integer = idr.GetOrdinal("ASS_DESCRIZIONE")
                            Dim ASS_OBSOLETO As Integer = idr.GetOrdinal("ASS_OBSOLETO")
                            Dim VCA_CODICE_AVN As Integer = idr.GetOrdinal("VCA_CODICE_AVN")

                            While idr.Read()

                                Dim item As New Entities.CodificaEsternaVaccinazioneAssociazione()
                                item.CodiceAssociazione = idr.GetStringOrDefault(VAL_ASS_CODICE)
                                item.CodiceEsterno = idr.GetStringOrDefault(VCA_CODICE_AVN)
                                item.CodiceVaccinazione = idr.GetStringOrDefault(VAL_VAC_CODICE)
                                item.DescrizioneAssociazione = idr.GetStringOrDefault(ASS_DESCRIZIONE)
                                item.IsAssociazioneObsoleta = idr.GetBooleanOrDefault(ASS_OBSOLETO)
                                list.Add(item)

                            End While

                        End If
                    End Using

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return list

        End Function

        Public Function GetCodificaEsterna(codiceVaccinazione As String, codiceAssociazione As String) As String Implements IAnaVaccinazioniProvider.GetCodificaEsterna

            Dim result As String = String.Empty

            Dim ownConnection As Boolean = False

            Try
                Dim query As String =
                "SELECT  VCA_CODICE_AVN " +
                "FROM T_VAC_CODICE_ESTERNO_ASS " +
                "WHERE VCA_VAC_CODICE = :codiceVaccinazione " +
                "AND VCA_ASS_CODICE = :codiceAssociazione "

                Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("codiceVaccinazione", codiceVaccinazione)
                    cmd.Parameters.AddWithValue("codiceAssociazione", codiceAssociazione)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim vca_codice_avn As Integer = idr.GetOrdinal("VCA_CODICE_AVN")

                            While idr.Read()

                                result = idr.GetStringOrDefault(vca_codice_avn)

                            End While

                        End If
                    End Using

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return result

        End Function

        ''' <summary>
        ''' Inserimento codifica per associazione specificata
        ''' </summary>
        ''' <param name="codifica"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsertCodificaVaccinazioneAssociazione(codifica As Entities.CodificaEsternaVaccinazioneAssociazione) As Integer Implements IAnaVaccinazioniProvider.InsertCodificaVaccinazioneAssociazione

            Dim count As Integer = 0

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand("insert into T_VAC_CODICE_ESTERNO_ASS (vca_vac_codice, vca_ass_codice, vca_codice_avn) values (:vca_vac_codice, :vca_ass_codice, :vca_codice_avn)", Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("vca_vac_codice", codifica.CodiceVaccinazione)
                    cmd.Parameters.AddWithValue("vca_ass_codice", codifica.CodiceAssociazione)
                    cmd.Parameters.AddWithValue("vca_codice_avn", codifica.CodiceEsterno)

                    count = cmd.ExecuteNonQuery()

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return count

        End Function

        ''' <summary>
        ''' Cancella tutte le codifiche relative alla vaccinazione specificata
        ''' </summary>
        ''' <param name="codiceVaccinazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteCodificheAssociazione(codiceVaccinazione As String) As Integer Implements IAnaVaccinazioniProvider.DeleteCodificheAssociazione

            Dim count As Integer = 0

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand("delete from T_VAC_CODICE_ESTERNO_ASS where vca_vac_codice = :vca_vac_codice", Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("vca_vac_codice", codiceVaccinazione)

                    count = cmd.ExecuteNonQuery()

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return count

        End Function

        Public Function ExistsVaccinazioneAntiInfluenzale(codiceVac As String) As Boolean Implements IAnaVaccinazioniProvider.ExistsVaccinazioneAntiInfluenzale

            Dim exists As Boolean = False

            Using cmd As OracleCommand = Connection.CreateCommand()

                cmd.CommandText = "SELECT 1 FROM T_ANA_VACCINAZIONI WHERE VAC_ANTI_INFLUENZALE = 'S' AND VAC_CODICE = :codiceVac "

                cmd.Parameters.AddWithValue("codiceVac", codiceVac)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    exists = obj IsNot Nothing AndAlso obj IsNot DBNull.Value

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return exists

        End Function

#End Region

#Region " OnVac APP "

        ''' <summary>
        ''' Restituisce la lista (codice-descrizione) delle vaccinazioni presenti in anagrafe, includendo solo quelle che hanno il flag di visualizzazione nella APP a "S".
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetListVaccinazioniAPP() As List(Of Entities.VaccinazioneAPP) Implements IAnaVaccinazioniProvider.GetListVaccinazioniAPP

            Dim listVaccinazioniAPP As List(Of Entities.VaccinazioneAPP) = Nothing

            Dim query As String =
                " select vac_codice, nvl(vai_titolo, vac_descrizione) vac_descrizione " +
                " from t_ana_vaccinazioni left join t_ana_vaccinazioni_info on vac_codice = vai_vac_codice " +
                " where vac_show_in_app = 'S' "

            Using cmd As New OracleCommand(query, Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    listVaccinazioniAPP = Me.GetVaccinazioniAPP(cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listVaccinazioniAPP

        End Function

        ''' <summary>
        ''' Restituisce le vaccinazioni (codice-descrizione) che compongono l'associazione specificata.
        ''' </summary>
        ''' <param name="codiceAssociazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetListVaccinazioniAPP(codiceAssociazione As String) As List(Of Entities.VaccinazioneAPP) Implements IAnaVaccinazioniProvider.GetListVaccinazioniAPP

            Dim listVaccinazioniAPP As List(Of Entities.VaccinazioneAPP) = Nothing

            Dim query As String =
                "select val_vac_codice vac_codice, vac_descrizione " +
                "from t_ana_link_ass_vaccinazioni " +
                "join t_ana_vaccinazioni on val_vac_codice = vac_codice " +
                "where val_ass_codice = :codiceAssociazione "

            Using cmd As New OracleCommand(query, Me.Connection)

                cmd.Parameters.AddWithValue("codiceAssociazione", codiceAssociazione)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    listVaccinazioniAPP = Me.GetVaccinazioniAPP(cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listVaccinazioniAPP

        End Function

        Public Function GetListAssociazioneByListVacc(listaVac As List(Of String), countList As Integer) As List(Of StatVacciniAssociatiControllo) Implements IAnaVaccinazioniProvider.GetListAssociazioneByListVacc

            Dim listAssVaccinazioni As New List(Of StatVacciniAssociatiControllo)

            Dim ownConnection As Boolean = False


            Try
                Using cmd As New OracleClient.OracleCommand()

                    cmd.Connection = Me.Connection

                    Dim filtroVaccinazioni As New System.Text.StringBuilder()

                    If Not listaVac.IsNullOrEmpty() Then

                        If listaVac.Count = 1 Then
                            filtroVaccinazioni.Append(" ass1.val_vac_codice = :par_val_vac_codice ")
                            cmd.Parameters.AddWithValue("par_val_vac_codice", listaVac.Single())
                        Else
                            Dim filter As GetInFilterResult = GetInFilter(listaVac)
                            filtroVaccinazioni.AppendFormat(" ass1.val_vac_codice in ({0}) ", filter.InFilter)
                            cmd.Parameters.AddRange(filter.Parameters)
                        End If

                    End If

                    If filtroVaccinazioni.Length > 0 Then
                        filtroVaccinazioni.Insert(0, " where ")
                    End If

                    cmd.CommandText = String.Format("SELECT COUNT(*) AS tot,val_ass_codice,ASS_DESCRIZIONE,ASS_DEF_STAMPA
FROM t_ana_link_ass_vaccinazioni
JOIN T_ANA_ASSOCIAZIONI ON val_ass_codice    = ass_codice
WHERE 
EXISTS (SELECT 1 FROM t_ana_link_ass_vaccinazioni ass1
{0}
AND t_ana_link_ass_vaccinazioni.val_ass_codice=ass1.val_ass_codice)
GROUP BY val_ass_codice,
  ASS_DESCRIZIONE,
  ASS_DEF_STAMPA
  HAVING COUNT(*) = {1}", filtroVaccinazioni.ToString(), countList.ToString())
                    'String.Format("select count(*) as tot,val_ass_codice, ASS_DESCRIZIONE, ASS_DEF_STAMPA  from t_ana_link_ass_vaccinazioni join T_ANA_ASSOCIAZIONI on val_ass_codice = ass_codice {0} group by val_ass_codice, ASS_DESCRIZIONE, ASS_DEF_STAMPA having count(*) = {1} ", filtroVaccinazioni.ToString(), countList.ToString())

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As OracleDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim val_ass_codice As Integer = idr.GetOrdinal("val_ass_codice")
                            Dim ASS_DESCRIZIONE As Integer = idr.GetOrdinal("ASS_DESCRIZIONE")
                            Dim ASS_DEF_STAMPA As Integer = idr.GetOrdinal("ASS_DEF_STAMPA")
                            Dim tot As Integer = idr.GetOrdinal("tot")

                            While idr.Read()
                                Dim associazione As New StatVacciniAssociatiControllo
                                associazione.CodiceAssociazione = idr.GetStringOrDefault(val_ass_codice)
                                associazione.DescrizioneAssociazione = idr.GetStringOrDefault(ASS_DESCRIZIONE)
                                associazione.DefaultAssociazione = idr.GetStringOrDefault(ASS_DEF_STAMPA)
                                associazione.CountVac = tot

                                listAssVaccinazioni.Add(associazione)

                            End While

                        End If
                    End Using
                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try


            Return listAssVaccinazioni

        End Function

        Private Function GetVaccinazioniAPP(cmd As OracleCommand) As List(Of Entities.VaccinazioneAPP)

            Dim listVaccinazioniAPP As New List(Of Entities.VaccinazioneAPP)()

            Using idr As IDataReader = cmd.ExecuteReader()

                If Not idr Is Nothing Then

                    Dim vac As Entities.VaccinazioneAPP = Nothing

                    Dim val_vac_codice As Integer = idr.GetOrdinal("vac_codice")
                    Dim vac_descrizione As Integer = idr.GetOrdinal("vac_descrizione")

                    While idr.Read()

                        vac = New Entities.VaccinazioneAPP()
                        vac.Codice = idr.GetString(val_vac_codice)
                        vac.Descrizione = idr.GetString(vac_descrizione)

                        listVaccinazioniAPP.Add(vac)

                    End While

                End If

            End Using

            Return listVaccinazioniAPP

        End Function

#End Region

    End Class

End Namespace