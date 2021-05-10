Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager


Namespace DAL.Oracle

    Public Class DbAssociazioniProvider
        Inherits DbProvider
        Implements IAssociazioniProvider

#Region " Costruttori "

        Public Sub New(ByRef DAM As IDAM)
            MyBase.New(DAM)
        End Sub

#End Region

#Region " Public "

        ''' <summary>
        ''' Restituisce l'elenco delle vaccinazioni che compongono l'associazione specificata.
        ''' </summary>
        ''' <param name="codiceAssociazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetVaccinazioniAssociazione(codiceAssociazione As String) As System.Collections.ArrayList Implements IAssociazioniProvider.GetVaccinazioniAssociazione

            Dim listVaccinazioni As New ArrayList()

            Using cmd As New OracleClient.OracleCommand(Queries.Associazioni.OracleQueries.selVaccinazioniAssociazione, Me.Connection)

                cmd.Parameters.AddWithValue("cod_ass", codiceAssociazione)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        While idr.Read()
                            listVaccinazioni.Add(idr(0).ToString())
                        End While

                    End Using

                Catch ex As Exception

                    Throw New Exception("Errore lettura vaccinazioni non associabili", ex)

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return listVaccinazioni

        End Function

        ''' <summary>
        ''' Restituisce la lista di vaccinazioni che compongono le associazioni specificate
        ''' </summary>
        ''' <param name="codiciAssociazioni"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetVaccinazioniAssociazioni(codiciAssociazioni As List(Of String)) As List(Of Entities.VaccinazioneAssociazione) Implements IAssociazioniProvider.GetVaccinazioniAssociazioni

            Dim listVaccinazioni As New List(Of Entities.VaccinazioneAssociazione)()

            If codiciAssociazioni Is Nothing OrElse codiciAssociazioni.Count = 0 Then Return listVaccinazioni

            Dim query As String = "select val_ass_codice, val_vac_codice from t_ana_link_ass_vaccinazioni where val_ass_codice {0} order by val_ass_codice, val_vac_codice"

            Using cmd As New OracleClient.OracleCommand()

                Dim filter As New System.Text.StringBuilder()

                If codiciAssociazioni.Count = 1 Then

                    filter.Append(" = :codiceAssociazione ")
                    cmd.Parameters.AddWithValue("codiceAssociazione", codiciAssociazioni.First())

                Else

                    Dim filtroInResult As GetInFilterResult = Me.GetInFilter(codiciAssociazioni)

                    filter.AppendFormat(" in ({0}) ", filtroInResult.InFilter)
                    cmd.Parameters.AddRange(filtroInResult.Parameters)

                End If

                cmd.Connection = Me.Connection
                cmd.CommandText = String.Format(query, filter)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim ass As Integer = idr.GetOrdinal("val_ass_codice")
                            Dim vac As Integer = idr.GetOrdinal("val_vac_codice")

                            While idr.Read()

                                Dim item As New Entities.VaccinazioneAssociazione()
                                item.CodiceAssociazione = idr.GetString(ass)
                                item.CodiceVaccinazione = idr.GetString(vac)

                                listVaccinazioni.Add(item)

                            End While

                        End If

                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listVaccinazioni

        End Function


        ''' <summary>
        ''' Restituisce un datatable con i dati delle associazioni che possono essere aggiunte alla convocazioni, in base ai dati specificati.
        ''' Non restituisce le associazioni obsolete.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataConvocazione"></param>
        ''' <param name="gestioneTipiConsultori"></param>
        ''' <param name="codiceConsultorio"></param>
        ''' <param name="codiciVaccinazioniProgrammateConvocazione"></param>
        ''' <param name="getSitoInoculazioneDefault"></param>
        ''' <param name="getViaSomministrazioneDefault"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LoadAssociazioniDaInserire(codicePaziente As Integer, dataConvocazione As Date, gestioneTipiConsultori As Boolean,
                                                   codiceConsultorio As String, codiciVaccinazioniProgrammateConvocazione As String(),
                                                   getSitoInoculazioneDefault As Boolean, getViaSomministrazioneDefault As Boolean) As DataTable Implements IAssociazioniProvider.LoadAssociazioniDaInserire
            '--
            Me.RefurbishDT()
            '--
            With _DAM.QB
                '--
                .NewQuery()
                .AddTables("T_VAC_ESCLUSE, T_ANA_LINK_ASS_VACCINAZIONI")
                .AddSelectFields("1")
                .AddWhereCondition("VEX_VAC_CODICE", Comparatori.Uguale, "VAL_VAC_CODICE", DataTypes.Join)
                .AddWhereCondition("VAL_ASS_CODICE", Comparatori.Uguale, "ASS_CODICE", DataTypes.Replace)
                .AddWhereCondition("VEX_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("VEX_DATA_SCADENZA", Comparatori.Is, "Null", DataTypes.Replace)
                '--
                Dim vaccinazioniEscluseQuery As String = .GetSelect()
                '--
                .NewQuery(False, False)
                .AddTables("T_VAC_ESCLUSE, T_ANA_VACCINAZIONI, T_ANA_LINK_ASS_VACCINAZIONI")
                .AddSelectFields("1")
                .AddWhereCondition("VEX_VAC_CODICE", Comparatori.Uguale, "VAC_CODICE", DataTypes.Join)
                .AddWhereCondition("VAC_COD_SOSTITUTA", Comparatori.Uguale, "VAL_VAC_CODICE", DataTypes.Join)
                .AddWhereCondition("VAL_ASS_CODICE", Comparatori.Uguale, "ASS_CODICE", DataTypes.Replace)
                .AddWhereCondition("VEX_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("VEX_DATA_SCADENZA", Comparatori.Is, "Null", DataTypes.Replace)
                '--
                Dim vaccinazioniEscluseSostituiteQuery As String = .GetSelect()
                '--
                .NewQuery(False, False)
                .AddTables("T_VAC_PROGRAMMATE, T_ANA_LINK_ASS_VACCINAZIONI")
                .AddSelectFields("1")
                .AddWhereCondition("VPR_VAC_CODICE", Comparatori.Uguale, "VAL_VAC_CODICE", DataTypes.Join)
                .AddWhereCondition("VAL_ASS_CODICE", Comparatori.Uguale, "ASS_CODICE", DataTypes.Replace)
                .AddWhereCondition("VPR_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("VPR_CNV_DATA", Comparatori.Diverso, dataConvocazione, DataTypes.Data)
                '--
                Dim vaccinazioniProgrammateQuery As String = .GetSelect()
                '--
                .NewQuery(False, False)
                .AddTables("T_VAC_PROGRAMMATE, T_ANA_VACCINAZIONI, T_ANA_LINK_ASS_VACCINAZIONI")
                .AddSelectFields("1")
                .AddWhereCondition("VPR_VAC_CODICE", Comparatori.Uguale, "VAC_CODICE", DataTypes.Join)
                .AddWhereCondition("VAC_COD_SOSTITUTA", Comparatori.Uguale, "VAL_VAC_CODICE", DataTypes.Join)
                .AddWhereCondition("VAL_ASS_CODICE", Comparatori.Uguale, "ASS_CODICE", DataTypes.Replace)
                .AddWhereCondition("VPR_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("VPR_CNV_DATA", Comparatori.Diverso, dataConvocazione, DataTypes.Data)
                '--
                Dim vaccinazioniProgrammateSostituiteQuery As String = .GetSelect()
                '--
                .NewQuery(False, False)
                '--
                .AddTables("T_ANA_ASSOCIAZIONI, T_ANA_LINK_ASS_VACCINAZIONI")
                '--
                If gestioneTipiConsultori Then
                    .IsDistinct = True
                    .AddTables("T_ANA_ASSOCIAZIONI_TIPI_CNS, T_ANA_CONSULTORI")
                End If
                '--
                .AddSelectFields("ASS_CODICE, ASS_DESCRIZIONE, ASS_ORDINE")
                '--
                '-- escludo le associazioni obsolete
                .OpenParanthesis()
                .AddWhereCondition("ASS_OBSOLETO", Comparatori.Uguale, "N", DataTypes.Stringa)
                .AddWhereCondition("ASS_OBSOLETO", Comparatori.Is, "NULL", DataTypes.Replace, "OR")
                .CloseParanthesis()
                '-- escludo le associazioni che hanno una vaccinazione esclusa (senza scadenza) e/o sostituita
                .AddWhereCondition("", Comparatori.NotExist, "(" + vaccinazioniEscluseQuery + ")", DataTypes.Replace)
                .AddWhereCondition("", Comparatori.NotExist, "(" + vaccinazioniEscluseSostituiteQuery + ")", DataTypes.Replace)
                '-- escludo le associazioni che hanno una vaccinazione programmata e/o sostituita
                .AddWhereCondition("", Comparatori.NotExist, "(" + vaccinazioniProgrammateQuery + ")", DataTypes.Replace)
                .AddWhereCondition("", Comparatori.NotExist, "(" + vaccinazioniProgrammateSostituiteQuery + ")", DataTypes.Replace)
                '--
                If Not codiciVaccinazioniProgrammateConvocazione Is Nothing Then
                    '--
                    For Each codiceVaccinazioneProgrammata As String In codiciVaccinazioniProgrammateConvocazione
                        '-- escludo le associazioni che hanno una vaccinazione programmata e/o sostituita tra quelle elencate
                        .AddWhereCondition("", Comparatori.NotExist, String.Format("(SELECT 1 FROM T_ANA_LINK_ASS_VACCINAZIONI WHERE VAL_ASS_CODICE = ASS_CODICE AND VAL_VAC_CODICE = {0})",
                                                                                   .AddCustomParam(codiceVaccinazioneProgrammata)), DataTypes.Replace)
                        .AddWhereCondition("", Comparatori.NotExist, String.Format("(SELECT 1 FROM T_ANA_LINK_ASS_VACCINAZIONI INNER JOIN T_ANA_VACCINAZIONI ON VAL_VAC_CODICE = VAC_COD_SOSTITUTA WHERE VAL_ASS_CODICE = ASS_CODICE AND VAC_CODICE = {0})",
                                                                                   .AddCustomParam(codiceVaccinazioneProgrammata)), DataTypes.Replace)
                    Next
                    '--
                End If
                '-- escludo le associazioni che hanno una vaccinazione sostituita
                .AddWhereCondition("", Comparatori.NotExist, "(SELECT 1 FROM T_ANA_VACCINAZIONI INNER JOIN T_ANA_LINK_ASS_VACCINAZIONI ON VAC_CODICE = VAL_VAC_CODICE WHERE VAL_ASS_CODICE = ASS_CODICE AND VAC_COD_SOSTITUTA IS NOT NULL)", DataTypes.Replace)
                '-- escludo le associazioni che non hanno vaccinazioni
                .AddWhereCondition("", Comparatori.Exist, "(SELECT 1 FROM T_ANA_LINK_ASS_VACCINAZIONI WHERE VAL_ASS_CODICE = ASS_CODICE)", DataTypes.Replace)
                '--
                .AddWhereCondition("ASS_CODICE", Comparatori.Uguale, "VAL_ASS_CODICE", DataTypes.Join)
                '--
                If gestioneTipiConsultori Then
                    '--
                    .AddWhereCondition("ASS_CODICE", Comparatori.Uguale, "ATC_ASS_CODICE", DataTypes.Join)
                    .AddWhereCondition("ATC_CNS_TIPO", Comparatori.Uguale, "CNS_TIPO", DataTypes.Join)
                    .AddWhereCondition("CNS_CODICE", Comparatori.Uguale, codiceConsultorio, DataTypes.Stringa)
                    '--
                End If
                '--
                .AddGroupByFields("ASS_CODICE, ASS_DESCRIZIONE, ASS_ORDINE")
                '--
                .AddOrderByFields("ASS_ORDINE, ASS_DESCRIZIONE")
                '--
                ' Recupero del valore di default per il sito di inoculazione
                If getSitoInoculazioneDefault Then
                    .AddTables("T_ANA_SITI_INOCULAZIONE")
                    .AddSelectFields("ASS_SII_CODICE, SII_DESCRIZIONE")
                    .AddWhereCondition("ASS_SII_CODICE", Comparatori.Uguale, "SII_CODICE", DataTypes.OutJoinLeft)
                    .AddGroupByFields("ASS_SII_CODICE, SII_DESCRIZIONE")
                Else
                    .AddSelectFields("'' ASS_SII_CODICE, '' SII_DESCRIZIONE")
                End If
                '--
                ' Recupero del valore di default per la via di somministrazione
                If getViaSomministrazioneDefault Then
                    .AddTables("T_ANA_VIE_SOMMINISTRAZIONE")
                    .AddSelectFields("ASS_VII_CODICE, VII_DESCRIZIONE")
                    .AddWhereCondition("ASS_VII_CODICE", Comparatori.Uguale, "VII_CODICE", DataTypes.OutJoinLeft)
                    .AddGroupByFields("ASS_VII_CODICE, VII_DESCRIZIONE")
                Else
                    .AddSelectFields("'' ASS_VII_CODICE, '' VII_DESCRIZIONE")
                End If
                '--
            End With
            '--
            Try
                _DAM.BuildDataTable(_DT)
            Catch ex As Exception
                ex.InternalPreserveStackTrace()
                Throw
            End Try
            '--
            Return _DT.Copy()
            '--
        End Function

        ''' <summary>
        ''' Recupero delle informazioni di default relative a sito inoculazione e via di somministrazione in base all'associazione.
        ''' </summary>
        ''' <param name="codiceAssociazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetInfoSomministrazioneDefaultAssociazione(codiceAssociazione As String) As Entities.InfoSomministrazione Implements IAssociazioniProvider.GetInfoSomministrazioneDefaultAssociazione

            Dim infoSomministrazione As New Entities.InfoSomministrazione()

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand(OnVac.Queries.Associazioni.OracleQueries.selSitoViaByAssociazione, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("ass_codice", codiceAssociazione)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim ass_sii_codice As Integer = idr.GetOrdinal("ASS_SII_CODICE")
                            Dim sii_descrizione As Integer = idr.GetOrdinal("SII_DESCRIZIONE")
                            Dim ass_vii_codice As Integer = idr.GetOrdinal("ASS_VII_CODICE")
                            Dim vii_descrizione As Integer = idr.GetOrdinal("VII_DESCRIZIONE")

                            If idr.Read() Then
                                infoSomministrazione.CodiceSitoInoculazione = idr.GetStringOrDefault(ass_sii_codice)
                                infoSomministrazione.DescrizioneSitoInoculazione = idr.GetStringOrDefault(sii_descrizione)
                                infoSomministrazione.CodiceViaSomministrazione = idr.GetStringOrDefault(ass_vii_codice)
                                infoSomministrazione.DescrizioneViaSomministrazione = idr.GetStringOrDefault(vii_descrizione)
                            End If

                        End If

                    End Using

                End Using

            Finally

                Me.ConditionalCloseConnection(ownConnection)

            End Try

            Return infoSomministrazione

        End Function

        ''' <summary>
        ''' Restituisce un datatable contenente le vaccinazioni relative alle associazioni specificate
        ''' </summary>
        ''' <param name="listCodiciAssociazioni"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDtVaccinazioniAssociazioni(listCodiciAssociazioni As List(Of String)) As DataTable Implements IAssociazioniProvider.GetDtVaccinazioniAssociazioni

            Dim dt As New DataTable()

            With _DAM.QB
                '--
                .NewQuery()
                .AddTables("T_ANA_VACCINAZIONI, T_ANA_LINK_ASS_VACCINAZIONI, T_ANA_ASSOCIAZIONI")
                .AddSelectFields("VAC_CODICE, VAC_COD_SOSTITUTA, VAC_DESCRIZIONE, VAC_OBBLIGATORIA, VAL_ASS_CODICE, ASS_DESCRIZIONE")
                '--
                If listCodiciAssociazioni.Count = 1 Then
                    .AddWhereCondition("VAL_ASS_CODICE", Comparatori.Uguale, listCodiciAssociazioni(0), DataTypes.Stringa)
                Else
                    .AddWhereCondition("VAL_ASS_CODICE", Comparatori.In, "'" + String.Join("','", listCodiciAssociazioni.ToArray()) + "'", DataTypes.Replace)
                End If
                '--
                .AddWhereCondition("VAC_CODICE", Comparatori.Uguale, "VAL_VAC_CODICE", DataTypes.Join)
                .AddWhereCondition("VAL_ASS_CODICE", Comparatori.Uguale, "ASS_CODICE", DataTypes.Join)
                .AddOrderByFields("VAC_CODICE")
                '--
                _DAM.BuildDataTable(dt)
                '--
            End With

            Return dt

        End Function

        ''' <summary>
        ''' Restituisce la descrizione dell'associazione dato il codice
        ''' </summary>
        ''' <param name="codiceAssociazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDescrizioneAssociazione(codiceAssociazione As String) As String Implements IAssociazioniProvider.GetDescrizioneAssociazione

            Dim descrizione As String = String.Empty

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand("select ass_descrizione from t_ana_associazioni where ass_codice = :ass_codice", Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("ass_codice", codiceAssociazione)

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
        Public Function GetListaVacAss(codiceAssociazione As List(Of String)) As List(Of Entities.StatVacciniAssociati) Implements IAssociazioniProvider.GetListaVacAss

            Dim descrizione As String = String.Empty

            Dim ownConnection As Boolean = False
            Dim stringQuery As String = String.Empty
            Dim list As New List(Of Entities.StatVacciniAssociati)()

            Try
                Using cmd As New OracleClient.OracleCommand()
                    stringQuery = "SELECT DISTINCT ASS_CODICE, ASS_DESCRIZIONE, ASS_DEF_STAMPA, ASS_OBSOLETO from T_ANA_LINK_ASS_VACCINAZIONI inner join  T_ANA_ASSOCIAZIONI on ASS_CODICE= VAL_ASS_CODICE "
                    If Not codiceAssociazione Is Nothing Then
                        Dim filter As GetInFilterResult = GetInFilter(codiceAssociazione)
                        stringQuery = String.Format("{0} where ASS_CODICE in ({1}) ", stringQuery, filter.InFilter)
                        cmd.Parameters.AddRange(filter.Parameters)
                        '' stringQuery = stringQuery + " AND ASS_CODICE in :ASS_CODICE"
                        'cmd.Parameters.AddWithValue("ASS_CODICE", codiceAssociazione)
                    End If
                    cmd.Connection = Me.Connection
                    cmd.CommandText = stringQuery
                    ownConnection = Me.ConditionalOpenConnection(cmd)



                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim vpr_ass_codice As Integer = idr.GetOrdinal("ASS_CODICE")
                            Dim vpr_ass_def_stampa As Integer = idr.GetOrdinal("ASS_DEF_STAMPA")
                            Dim vpr_ass_descrizione_codice As Integer = idr.GetOrdinal("ASS_DESCRIZIONE")
                            Dim vpr_ass_obsoleta As Integer = idr.GetOrdinal("ASS_OBSOLETO")


                            While idr.Read()

                                Dim item As New Entities.StatVacciniAssociati()
                                item.CodiceAssociazione = idr.GetString(vpr_ass_codice)
                                item.DescrizioneAssociazione = idr.GetString(vpr_ass_descrizione_codice)
                                item.DefaultAssociazione = idr.GetString(vpr_ass_def_stampa)
                                item.Obsoleta = idr.GetString(vpr_ass_obsoleta)
                                Dim appList As List(Of String) = Nothing
                                appList = GetListaVacciniAssociazione(idr.GetString(vpr_ass_codice))
                                item.ListaVaccini = appList
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
        Private Function GetListaVacciniAssociazione(codiceAssociazione As String) As List(Of String)


            Dim ownConnection As Boolean = False
            Dim stringQuery As String = String.Empty
            Dim list As New List(Of String)()

            Try
                Using cmd As New OracleClient.OracleCommand()
                    stringQuery = "SELECT VAL_VAC_CODICE FROM T_ANA_LINK_ASS_VACCINAZIONI"
                    If codiceAssociazione <> String.Empty Then
                        stringQuery = stringQuery + " WHERE VAL_ASS_CODICE= :ASS_CODICE"
                        cmd.Parameters.AddWithValue("ASS_CODICE", codiceAssociazione)
                    End If
                    cmd.Connection = Me.Connection
                    cmd.CommandText = stringQuery
                    ownConnection = Me.ConditionalOpenConnection(cmd)



                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim vpr_val_vac_codice As Integer = idr.GetOrdinal("VAL_VAC_CODICE")

                            While idr.Read()

                                Dim item As String
                                item = idr.GetStringOrDefault(vpr_val_vac_codice)
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
#End Region

#Region " Info Associazioni "

        Public Function GetAssociazioneInfo(codiceAssociazione As String) As Entities.AssociazioneInfo Implements IAssociazioniProvider.GetAssociazioneInfo

            Dim info As New Entities.AssociazioneInfo()

            Dim ownConnection As Boolean = False

            Dim query As String =
                "select asi_id, asi_titolo, asi_descrizione, ass_codice, ass_descrizione " +
                "from t_ana_associazioni " +
                "left join t_ana_associazioni_info on ass_codice=asi_ass_codice " +
                "where ass_codice = :codiceAssociazione"

            Try
                Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("codiceAssociazione", codiceAssociazione)

                    Using idr As OracleClient.OracleDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim asi_id As Integer = idr.GetOrdinal("asi_id")
                            Dim asi_titolo As Integer = idr.GetOrdinal("asi_titolo")
                            Dim asi_descrizione As Integer = idr.GetOrdinal("asi_descrizione")
                            Dim ass_codice As Integer = idr.GetOrdinal("ass_codice")
                            Dim ass_descrizione As Integer = idr.GetOrdinal("ass_descrizione")

                            If idr.Read() Then

                                info.Id = idr.GetNullableInt32OrDefault(asi_id)
                                info.CodiceAssociazione = idr.GetStringOrDefault(ass_codice)
                                info.DescrizioneAssociazione = idr.GetStringOrDefault(ass_descrizione)
                                info.Titolo = idr.GetStringOrDefault(asi_titolo)

                                Dim clob As OracleClient.OracleLob = idr.GetOracleLob(asi_descrizione)

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

        Public Function InsertAssociazioneInfo(info As Entities.AssociazioneInfo) As Integer Implements IAssociazioniProvider.InsertAssociazioneInfo

            Dim count As Integer = 0

            Dim ownConnection As Boolean = False

            Dim query As String =
                "insert into t_ana_associazioni_info (asi_ass_codice, asi_titolo, asi_descrizione) " +
                "values (:asi_ass_codice, :asi_titolo, :asi_descrizione)"

            Try
                Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("asi_ass_codice", info.CodiceAssociazione)

                    If String.IsNullOrWhiteSpace(info.Titolo) Then
                        cmd.Parameters.AddWithValue("asi_titolo", System.DBNull.Value)
                    Else
                        cmd.Parameters.AddWithValue("asi_titolo", info.Titolo)
                    End If

                    If String.IsNullOrWhiteSpace(info.Descrizione) Then
                        cmd.Parameters.AddWithValue("asi_descrizione", System.DBNull.Value)
                    Else
                        cmd.Parameters.AddWithValue("asi_descrizione", info.Descrizione)
                    End If

                    count = cmd.ExecuteNonQuery()

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return count

        End Function

        Public Function UpdateAssociazioneInfo(info As Entities.AssociazioneInfo) As Integer Implements IAssociazioniProvider.UpdateAssociazioneInfo

            Dim count As Integer = 0

            Dim ownConnection As Boolean = False

            Dim query As String =
                "update t_ana_associazioni_info " +
                "set asi_titolo = :asi_titolo, " +
                "asi_descrizione = :asi_descrizione " +
                "where asi_id = :asi_id"

            Try
                Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    If String.IsNullOrWhiteSpace(info.Titolo) Then
                        cmd.Parameters.AddWithValue("asi_titolo", System.DBNull.Value)
                    Else
                        cmd.Parameters.AddWithValue("asi_titolo", info.Titolo)
                    End If

                    If String.IsNullOrWhiteSpace(info.Descrizione) Then
                        cmd.Parameters.AddWithValue("asi_descrizione", System.DBNull.Value)
                    Else
                        cmd.Parameters.AddWithValue("asi_descrizione", info.Descrizione)
                    End If

                    cmd.Parameters.AddWithValue("asi_id", info.Id.Value)

                    count = cmd.ExecuteNonQuery()

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return count

        End Function

        Public Function DeleteAssociazioneInfo(idInfo As Integer) As Integer Implements IAssociazioniProvider.DeleteAssociazioneInfo

            Dim count As Integer = 0

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand("delete from t_ana_associazioni_info where asi_id = :asi_id", Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("asi_id", idInfo)
                    count = cmd.ExecuteNonQuery()

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return count

        End Function


#End Region

#Region " OnVac APP "

        ''' <summary>
        ''' Restituisce la lista (codice e descrizione) delle associazioni presenti in anagrafe, includendo solo quelle che hanno il flag di visualizzazione nella APP a "S".
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetListAssociazioniAPP() As List(Of Entities.AssociazioneAPP) Implements IAssociazioniProvider.GetListAssociazioniAPP

            Dim list As List(Of Entities.AssociazioneAPP) = Nothing

            Dim query As String =
                " select ass_codice, nvl(asi_titolo, ass_descrizione) ass_descrizione " +
                " from t_ana_associazioni left join t_ana_associazioni_info on ass_codice = asi_ass_codice " +
                " where ass_show_in_app = 'S' "

            Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    list = Me.GetAssociazioniAPP(cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list

        End Function


        ''' <summary>
        ''' Restituisce la lista (codice e descrizione) delle associazioni presenti in anagrafe.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetListCodiciAssociazioni() As List(Of String) Implements IAssociazioniProvider.GetListCodiciAssociazioni

            Dim list As New List(Of String)()

            Dim query As String =
                " select ass_codice" +
                " from t_ana_associazioni " +
                " where ass_obsoleto = 'N' "
            Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim ass_codice As Integer = idr.GetOrdinal("ass_codice")

                            While idr.Read()

                                list.Add(idr.GetString(ass_codice))

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
        ''' Restituisce la lista di associazioni (codice e descrizione) relative alla vaccinazione specificata
        ''' </summary>
        ''' <param name="codiceVaccinazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetListAssociazioniAPP(codiceVaccinazione As String) As List(Of Entities.AssociazioneAPP) Implements IAssociazioniProvider.GetListAssociazioniAPP

            Dim list As List(Of Entities.AssociazioneAPP) = Nothing

            Dim query As String =
               "select val_ass_codice ass_codice, ass_descrizione " +
               "from t_ana_link_ass_vaccinazioni " +
               "join t_ana_associazioni on val_ass_codice = ass_codice " +
               "where val_vac_codice = :codiceVaccinazione "

            Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                cmd.Parameters.AddWithValue("codiceVaccinazione", codiceVaccinazione)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    list = Me.GetAssociazioniAPP(cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list

        End Function

        Private Function GetAssociazioniAPP(cmd As OracleClient.OracleCommand) As List(Of Entities.AssociazioneAPP)

            Dim list As New List(Of Entities.AssociazioneAPP)()

            Using idr As IDataReader = cmd.ExecuteReader()

                If Not idr Is Nothing Then

                    Dim item As Entities.AssociazioneAPP = Nothing

                    Dim ass_codice As Integer = idr.GetOrdinal("ass_codice")
                    Dim ass_descrizione As Integer = idr.GetOrdinal("ass_descrizione")

                    While idr.Read()

                        item = New Entities.AssociazioneAPP()
                        item.Codice = idr.GetString(ass_codice)
                        item.Descrizione = idr.GetString(ass_descrizione)

                        list.Add(item)

                    End While
                End If

            End Using

            Return list

        End Function

#End Region



#Region " Flussi ACN "


		Public Function GetAssociazioneByCodiceACN(codiceACN As String) As AssociazioneAnag Implements IAssociazioniProvider.GetAssociazioneByCodiceACN

			Dim list As New List(Of AssociazioneAnag)()

			Dim ownConnection As Boolean = False

			Try
				Using cmd As New OracleClient.OracleCommand("select * from t_ana_associazioni where ass_codice_acn = :ass_codice_acn", Connection)

					ownConnection = ConditionalOpenConnection(cmd)

					cmd.Parameters.AddWithValue("ass_codice_acn", codiceACN)

					Using idr As IDataReader = cmd.ExecuteReader()

						If Not idr Is Nothing Then

							Dim ass_codice As Integer = idr.GetOrdinal("ass_codice")
							Dim ass_descrizione As Integer = idr.GetOrdinal("ass_descrizione")
							Dim ass_stampa As Integer = idr.GetOrdinal("ass_stampa")
							Dim ass_ordine As Integer = idr.GetOrdinal("ass_ordine")
							Dim ass_discrezionale As Integer = idr.GetOrdinal("ass_discrezionale")
							Dim ass_codice_esterno As Integer = idr.GetOrdinal("ass_codice_esterno")
							Dim ass_sii_codice As Integer = idr.GetOrdinal("ass_sii_codice")
							Dim ass_vii_codice As Integer = idr.GetOrdinal("ass_vii_codice")
							Dim ass_show_in_app As Integer = idr.GetOrdinal("ass_show_in_app")
							Dim ass_obsoleto As Integer = idr.GetOrdinal("ass_obsoleto")
							Dim ass_def_stampa As Integer = idr.GetOrdinal("ass_def_stampa")
							Dim ass_codice_acn As Integer = idr.GetOrdinal("ass_codice_acn")

							While idr.Read()

								Dim item As New AssociazioneAnag()
								item.Codice = idr.GetStringOrDefault(ass_codice)
								item.Descrizione = idr.GetStringOrDefault(ass_descrizione)
								item.Stampa = idr.GetStringOrDefault(ass_def_stampa)
								item.Ordine = idr.GetNullableInt32OrDefault(ass_ordine)
								item.Discrezionale = idr.GetBooleanOrDefault(ass_discrezionale)
								item.CodiceEsterno = idr.GetStringOrDefault(ass_codice_esterno)
								item.CodiceSitoInoculazione = idr.GetStringOrDefault(ass_sii_codice)
								item.CodiceViaSomministrazione = idr.GetStringOrDefault(ass_vii_codice)
								item.ShowInApp = idr.GetBooleanOrDefault(ass_show_in_app)
								item.Obsoleto = idr.GetBooleanOrDefault(ass_obsoleto)
								item.DefStampa = idr.GetBooleanOrDefault(ass_def_stampa)
								item.CodiceACN = idr.GetStringOrDefault(ass_codice_acn)

								list.Add(item)

							End While

						End If

					End Using

				End Using

			Finally

				ConditionalCloseConnection(ownConnection)

			End Try


			If Not list Is Nothing AndAlso list.Count > 0 Then
				Return list.FirstOrDefault()
			Else
				Return Nothing
			End If

		End Function
		Public Function GetAssociazioneByCodiceACNAic(codiceACN As String, codiceAic As String) As AssociazioneAnag Implements IAssociazioniProvider.GetAssociazioneByCodiceACNAic

			Dim list As New List(Of AssociazioneAnag)()

			Dim ownConnection As Boolean = False
			Dim filtroAcn As String = String.Empty
			Dim filtroAic As String = String.Empty
			If not codiceACN.IsNullOrEmpty then
				   filtroAcn=" and D.ass_codice_acn = :ass_codice_acn"
			End If
			If Not codiceAic.IsNullOrEmpty Then
				filtroAic = " and F.NOC_CODICE_AIC = :noc_codice_aic"
			End If
			Dim query As String = String.Format("select * from t_ana_associazioni D 
LEFT OUTER JOIN T_ANA_LINK_NOC_ASSOCIAZIONI E ON D.ass_codice = E.NAL_ASS_CODICE 
LEFT OUTER JOIN T_ANA_NOMI_COMMERCIALI F ON E.NAL_NOC_CODICE = F.NOC_CODICE
where 1=1 {0} {1}
order by ass_obsoleto, ass_descrizione ", filtroAcn, filtroAic)




			Try
				Using cmd As New OracleClient.OracleCommand(query, Connection)

					ownConnection = ConditionalOpenConnection(cmd)
					If Not codiceACN.IsNullOrEmpty() Then
						cmd.Parameters.AddWithValue("ass_codice_acn", codiceACN)
					End If
					If Not codiceAic.IsNullOrEmpty() Then
						cmd.Parameters.AddWithValue("noc_codice_aic", codiceAic)
					End If


					Using idr As IDataReader = cmd.ExecuteReader()

						If Not idr Is Nothing Then

							Dim ass_codice As Integer = idr.GetOrdinal("ass_codice")
							Dim ass_descrizione As Integer = idr.GetOrdinal("ass_descrizione")
							Dim ass_stampa As Integer = idr.GetOrdinal("ass_stampa")
							Dim ass_ordine As Integer = idr.GetOrdinal("ass_ordine")
							Dim ass_discrezionale As Integer = idr.GetOrdinal("ass_discrezionale")
							Dim ass_codice_esterno As Integer = idr.GetOrdinal("ass_codice_esterno")
							Dim ass_sii_codice As Integer = idr.GetOrdinal("ass_sii_codice")
							Dim ass_vii_codice As Integer = idr.GetOrdinal("ass_vii_codice")
							Dim ass_show_in_app As Integer = idr.GetOrdinal("ass_show_in_app")
							Dim ass_obsoleto As Integer = idr.GetOrdinal("ass_obsoleto")
							Dim ass_def_stampa As Integer = idr.GetOrdinal("ass_def_stampa")
							Dim ass_codice_acn As Integer = idr.GetOrdinal("ass_codice_acn")

							While idr.Read()

								Dim item As New AssociazioneAnag()
								item.Codice = idr.GetStringOrDefault(ass_codice)
								item.Descrizione = idr.GetStringOrDefault(ass_descrizione)
								item.Stampa = idr.GetStringOrDefault(ass_def_stampa)
								item.Ordine = idr.GetNullableInt32OrDefault(ass_ordine)
								item.Discrezionale = idr.GetBooleanOrDefault(ass_discrezionale)
								item.CodiceEsterno = idr.GetStringOrDefault(ass_codice_esterno)
								item.CodiceSitoInoculazione = idr.GetStringOrDefault(ass_sii_codice)
								item.CodiceViaSomministrazione = idr.GetStringOrDefault(ass_vii_codice)
								item.ShowInApp = idr.GetBooleanOrDefault(ass_show_in_app)
								item.Obsoleto = idr.GetBooleanOrDefault(ass_obsoleto)
								item.DefStampa = idr.GetBooleanOrDefault(ass_def_stampa)
								item.CodiceACN = idr.GetStringOrDefault(ass_codice_acn)

								list.Add(item)

							End While

						End If

					End Using

				End Using

			Finally

				ConditionalCloseConnection(ownConnection)

			End Try


			If Not list Is Nothing AndAlso list.Count > 0 Then
				Return list.FirstOrDefault()
			Else
				Return Nothing
			End If

		End Function



		Public Function GetVaccByCodiceACN(codVaccinazioneACN As String, codAssociazione As String) As VaccinazioneAssociazione Implements IAssociazioniProvider.GetVaccByCodiceACN

            Dim result As VaccinazioneAssociazione = Nothing

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand("select val_ass_codice, val_vac_codice from t_ana_vaccinazioni inner join t_ana_link_ass_vaccinazioni on val_vac_codice = vac_codice where vac_codice_acn = :vac_codice_acn and val_ass_codice = :val_ass_codice ", Connection)

                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("vac_codice_acn", codVaccinazioneACN)
                    cmd.Parameters.AddWithValue("val_ass_codice", codAssociazione)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim val_ass_codice As Integer = idr.GetOrdinal("val_ass_codice")
                            Dim val_vac_codice As Integer = idr.GetOrdinal("val_vac_codice")

                            'legge solo il primo
                            If idr.Read() Then

                                Dim item As New VaccinazioneAssociazione()
                                item.CodiceAssociazione = idr.GetStringOrDefault(val_ass_codice)
                                item.CodiceVaccinazione = idr.GetStringOrDefault(val_vac_codice)

                                result = item

                            End If

                        End If

                    End Using

                End Using

            Finally

                ConditionalCloseConnection(ownConnection)

            End Try

            Return result

        End Function


#End Region
    End Class

End Namespace


