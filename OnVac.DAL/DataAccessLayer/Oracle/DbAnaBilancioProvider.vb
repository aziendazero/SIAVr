Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.Entities


Namespace DAL.Oracle

    Public Class DbAnaBilancioProvider
        Inherits DbProvider
        Implements IAnaBilancioProvider

#Region " Costruttore "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " IAnaBilancioProvider "

#Region " Anagrafica Bilanci "

        ''' <summary>
        ''' Restituisce le anagrafiche dei bilanci relativi alla malattia specificata
        ''' </summary>
        ''' <param name="codiceMalattia"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetAnagraficaBilanciByMalattia(codiceMalattia As String) As List(Of Entities.BilancioAnagrafica) Implements IAnaBilancioProvider.GetAnagraficaBilanciByMalattia

            Dim list As List(Of Entities.BilancioAnagrafica) = Nothing

            Dim ownConnection As Boolean = False

            Try
                Dim query As String = GetSelectQueryAnagraficaBilanci() +
                    " where BIL_MAL_CODICE = :BIL_MAL_CODICE " +
                    " order by BIL_NUMERO "

                Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("BIL_MAL_CODICE", codiceMalattia)

                    list = GetListAnagraficaBilanci(cmd)

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return list

        End Function

        ''' <summary>
        ''' Restituisce le anagrafiche dei bilanci relativi alla malattia specificata, non ancora compilati per il paziente.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="codiceMalattia"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetAnagraficaBilanciNonCompilatiPazienteByMalattia(codicePaziente As Long, codiceMalattia As String) As List(Of Entities.BilancioAnagrafica) Implements IAnaBilancioProvider.GetAnagraficaBilanciNonCompilatiPazienteByMalattia

            Dim list As List(Of Entities.BilancioAnagrafica) = Nothing

            Dim ownConnection As Boolean = False

            Try
                Dim query As String = GetSelectQueryAnagraficaBilanci() +
                    " where BIL_MAL_CODICE = :BIL_MAL_CODICE " +
                    " and not exists ( " +
                    "    select 1 from T_VIS_VISITE " +
                    "    where VIS_PAZ_CODICE = :VIS_PAZ_CODICE " +
                    "    and VIS_N_BILANCIO = BIL_NUMERO " +
                    "    and VIS_MAL_CODICE = BIL_MAL_CODICE) " +
                    " order by BIL_NUMERO "

                Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("BIL_MAL_CODICE", codiceMalattia)
                    cmd.Parameters.AddWithValue("VIS_PAZ_CODICE", codicePaziente)

                    list = GetListAnagraficaBilanci(cmd)

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return list

        End Function

        ''' <summary>
        ''' Restituisce l'anagrafica del bilancio specificato.
        ''' </summary>
        ''' <param name="numeroBilancio"></param>
        ''' <param name="codiceMalattia"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetAnagraficaBilancio(numeroBilancio As Integer, codiceMalattia As String) As Entities.BilancioAnagrafica Implements IAnaBilancioProvider.GetAnagraficaBilancio

            Dim item As Entities.BilancioAnagrafica = Nothing

            Dim ownConnection As Boolean = False

            Try
                ' N.B. : la join con la t_ana_link_malattie_tipologia è per compatibilità con il metodo che veniva usato prima

                Dim query As String = GetSelectQueryAnagraficaBilanci() +
                    " join T_ANA_LINK_MALATTIE_TIPOLOGIA on mal_codice = mml_mal_codice " +
                    " where BIL_NUMERO = :BIL_NUMERO " +
                    " and BIL_MAL_CODICE = :BIL_MAL_CODICE "

                Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("BIL_NUMERO", numeroBilancio)
                    cmd.Parameters.AddWithValue("BIL_MAL_CODICE", codiceMalattia)

                    Dim list As List(Of Entities.BilancioAnagrafica) = GetListAnagraficaBilanci(cmd)

                    If Not list.IsNullOrEmpty() Then
                        item = list.First()
                    End If

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return item

        End Function

        Private Function GetSelectQueryAnagraficaBilanci() As String

            Return "select BIL_NUMERO, BIL_DESCRIZIONE, BIL_ETA_MINIMA, BIL_ETA_MASSIMA, BIL_CRANIO, BIL_ALTEZZA, BIL_PESO, BIL_OBBLIGATORIO, " +
                   " BIL_RILEVAZIONE_VACCINAZIONI, BIL_RILEVAZIONE_VIAGGI, BIL_RPT_NOME, MAL_CODICE, MAL_DESCRIZIONE " +
                   " from T_ANA_BILANCI " +
                   " join T_ANA_MALATTIE on BIL_MAL_CODICE = MAL_CODICE "

        End Function

        Private Function GetListAnagraficaBilanci(cmd As OracleClient.OracleCommand) As List(Of Entities.BilancioAnagrafica)

            Dim list As New List(Of Entities.BilancioAnagrafica)()

            Using idr As IDataReader = cmd.ExecuteReader()
                If Not idr Is Nothing Then

                    Dim bil_numero As Integer = idr.GetOrdinal("BIL_NUMERO")
                    Dim bil_descrizione As Integer = idr.GetOrdinal("BIL_DESCRIZIONE")
                    Dim mal_codice As Integer = idr.GetOrdinal("MAL_CODICE")
                    Dim mal_descrizione As Integer = idr.GetOrdinal("MAL_DESCRIZIONE")
                    Dim bil_eta_minima As Integer = idr.GetOrdinal("BIL_ETA_MINIMA")
                    Dim bil_eta_massima As Integer = idr.GetOrdinal("BIL_ETA_MASSIMA")
                    Dim bil_cranio As Integer = idr.GetOrdinal("BIL_CRANIO")
                    Dim bil_altezza As Integer = idr.GetOrdinal("BIL_ALTEZZA")
                    Dim bil_peso As Integer = idr.GetOrdinal("BIL_PESO")
                    Dim bil_obbligatorio As Integer = idr.GetOrdinal("BIL_OBBLIGATORIO")
                    Dim bil_rilevazione_vaccinazioni As Integer = idr.GetOrdinal("BIL_RILEVAZIONE_VACCINAZIONI")
                    Dim bil_rilevazione_viaggi As Integer = idr.GetOrdinal("BIL_RILEVAZIONE_VIAGGI")
                    Dim bil_rpt_nome As Integer = idr.GetOrdinal("BIL_RPT_NOME")

                    While idr.Read()

                        Dim item As New Entities.BilancioAnagrafica()
                        item.NumeroBilancio = idr.GetInt32(bil_numero)
                        item.DescrizioneBilancio = idr.GetStringOrDefault(bil_descrizione)
                        item.CodiceMalattia = idr.GetString(mal_codice)
                        item.DescrizioneMalattia = idr.GetStringOrDefault(mal_descrizione)
                        item.EtaMinima = idr.GetNullableInt32OrDefault(bil_eta_minima)
                        item.EtaMassima = idr.GetNullableInt32OrDefault(bil_eta_massima)
                        item.GestioneCranio = idr.GetBooleanOrDefault(bil_cranio)
                        item.GestioneAltezza = idr.GetBooleanOrDefault(bil_altezza)
                        item.GestionePeso = idr.GetBooleanOrDefault(bil_peso)
                        item.Obbligatorio = idr.GetBooleanOrDefault(bil_obbligatorio)
                        item.GestioneVaccinazioni = idr.GetBooleanOrDefault(bil_rilevazione_vaccinazioni)
                        item.GestioneViaggi = idr.GetBooleanOrDefault(bil_rilevazione_viaggi)
                        item.NomeReport = idr.GetStringOrDefault(bil_rpt_nome)

                        list.Add(item)

                    End While

                End If
            End Using

            Return list

        End Function

#End Region

#Region " Bilanci "

        Public Function GetBilanciPaziente(codicePaziente As Integer,
                                           tipologieMalattia As List(Of String),
                                           tipoConsegna As List(Of String)) As List(Of BilancioProgrammato) Implements IAnaBilancioProvider.GetBilanciPaziente

            Dim lst As New List(Of BilancioProgrammato)

            If tipologieMalattia.Count > 0 AndAlso tipoConsegna.Count > 0 Then

                With _DAM.QB

                    .NewQuery()
                    .AddSelectFields("1")
                    .AddTables("t_vis_visite")
                    .AddWhereCondition("vis_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                    .AddWhereCondition("vis_n_bilancio", Comparatori.Uguale, "bil_numero", DataTypes.Join)
                    .AddWhereCondition("vis_mal_codice", Comparatori.Uguale, "mal_codice", DataTypes.Join)
                    Dim Q1 As String = .GetSelect()

                    .NewQuery(False, False)
                    .AddWhereCondition("", Comparatori.NotExist, "(" & Q1 & ")", DataTypes.Replace)
                    .AddSelectFields("bil_numero, bil_descrizione, mal_codice, mal_descrizione, bil_eta_minima, bil_eta_massima, bil_intervallo")
                    .AddSelectFields("bil_cranio, bil_altezza, bil_peso, bil_obbligatorio")
                    .AddTables("t_ana_bilanci, t_ana_malattie, t_ana_link_malattie_tipologia")
                    .AddWhereCondition("bil_mal_codice", Comparatori.Uguale, "mal_codice", DataTypes.Join)
                    .AddWhereCondition("mal_codice", Comparatori.Uguale, "mml_mal_codice", DataTypes.Join)

                    ' Filtro sulle tipologie di malattia
                    Dim pString As New System.Text.StringBuilder()
                    For i As Integer = 0 To tipologieMalattia.Count - 1
                        pString.AppendFormat("{0},", .AddCustomParam(tipologieMalattia(i)))
                    Next
                    If pString.ToString.Length > 1 Then
                        pString.Remove(pString.ToString.Length - 1, 1)
                    End If
                    .AddWhereCondition("mml_mti_codice", Comparatori.In, pString.ToString, DataTypes.Replace)

                    ' Filtro sul tipo consegna
                    pString.Clear()
                    For i As Integer = 0 To tipoConsegna.Count - 1
                        pString.AppendFormat("{0},", .AddCustomParam(tipoConsegna(i)))
                    Next
                    If pString.ToString.Length > 1 Then
                        pString.Remove(pString.ToString.Length - 1, 1)
                    End If
                    .AddWhereCondition("bil_consegnato_a", Comparatori.In, pString.ToString, DataTypes.Replace)

                    ' Ordinamento
                    .AddOrderByFields("mal_descrizione", "bil_numero")

                End With

                Using dr As IDataReader = _DAM.BuildDataReader()

                    If Not dr Is Nothing Then

                        Dim MAL_CODICE As Integer = dr.GetOrdinal("MAL_CODICE")
                        Dim MAL_DESCRIZIONE As Integer = dr.GetOrdinal("MAL_DESCRIZIONE")
                        Dim BIL_NUMERO As Integer = dr.GetOrdinal("BIL_NUMERO")
                        Dim BIL_DESCRIZIONE As Integer = dr.GetOrdinal("BIL_DESCRIZIONE")
                        Dim BIL_ETA_MINIMA As Integer = dr.GetOrdinal("BIL_ETA_MINIMA")
                        Dim BIL_ETA_MASSIMA As Integer = dr.GetOrdinal("BIL_ETA_MASSIMA")
                        Dim BIL_INTERVALLO As Integer = dr.GetOrdinal("BIL_INTERVALLO")
                        Dim BIL_CRANIO As Integer = dr.GetOrdinal("BIL_CRANIO")
                        Dim BIL_ALTEZZA As Integer = dr.GetOrdinal("BIL_ALTEZZA")
                        Dim BIL_PESO As Integer = dr.GetOrdinal("BIL_PESO")
                        Dim BIL_OBBLIGATORIO As Integer = dr.GetOrdinal("BIL_OBBLIGATORIO")

                        While dr.Read()

                            Dim bilancio As New BilancioProgrammato(dr.GetString(MAL_CODICE), dr.GetDecimal(BIL_NUMERO))

                            bilancio.BIL_DESCRIZIONE = dr.GetStringOrDefault(BIL_DESCRIZIONE)
                            bilancio.Descrizione_Malattia = dr.GetStringOrDefault(MAL_DESCRIZIONE)
                            bilancio.Eta_Minima = dr.GetDecimal(BIL_ETA_MINIMA)
                            bilancio.Eta_Massima = dr.GetDecimal(BIL_ETA_MASSIMA)
                            bilancio.Intervallo = dr.GetDecimal(BIL_INTERVALLO)
                            bilancio.BIL_CRANIO = dr.GetBooleanOrDefault(BIL_CRANIO)
                            bilancio.BIL_ALTEZZA = dr.GetBooleanOrDefault(BIL_ALTEZZA)
                            bilancio.BIL_PESO = dr.GetBooleanOrDefault(BIL_PESO)
                            bilancio.BIL_OBBLIGATORIO = dr.GetBooleanOrDefault(BIL_OBBLIGATORIO)

                            lst.Add(bilancio)

                        End While

                    End If

                End Using

            End If

            Return lst

        End Function

        Public Function GetFromKey(numeroBilancio As Integer, codiceMalattia As String) As BilancioProgrammato Implements IAnaBilancioProvider.GetFromKey

            Dim bilancio As BilancioProgrammato = Nothing

            With _DAM.QB
                .NewQuery(True)
                .AddSelectFields("bil_descrizione, mal_descrizione, bil_eta_minima, bil_eta_massima, bil_cranio, bil_altezza, bil_peso, bil_obbligatorio")
                .AddTables("t_ana_bilanci, t_ana_malattie, t_ana_link_malattie_tipologia")
                .AddWhereCondition("bil_mal_codice", Comparatori.Uguale, "mal_codice", DataTypes.Join)
                .AddWhereCondition("mal_codice", Comparatori.Uguale, "mml_mal_codice", DataTypes.Join)
                ' filtro sulla malattia e sul bilancio
                .AddWhereCondition("bil_numero", Comparatori.Uguale, numeroBilancio, DataTypes.Replace)
                .AddWhereCondition("bil_mal_codice", Comparatori.Uguale, codiceMalattia, DataTypes.Stringa)
            End With

            Using dr As IDataReader = _DAM.BuildDataReader()

                If Not dr Is Nothing Then

                    Dim MAL_DESCRIZIONE As Integer = dr.GetOrdinal("MAL_DESCRIZIONE")
                    Dim BIL_DESCRIZIONE As Integer = dr.GetOrdinal("BIL_DESCRIZIONE")
                    Dim BIL_ETA_MINIMA As Integer = dr.GetOrdinal("BIL_ETA_MINIMA")
                    Dim BIL_ETA_MASSIMA As Integer = dr.GetOrdinal("BIL_ETA_MASSIMA")
                    Dim BIL_CRANIO As Integer = dr.GetOrdinal("BIL_CRANIO")
                    Dim BIL_ALTEZZA As Integer = dr.GetOrdinal("BIL_ALTEZZA")
                    Dim BIL_PESO As Integer = dr.GetOrdinal("BIL_PESO")
                    Dim BIL_OBBLIGATORIO As Integer = dr.GetOrdinal("BIL_OBBLIGATORIO")

                    If dr.Read() Then

                        bilancio = New BilancioProgrammato(codiceMalattia, numeroBilancio)

                        bilancio.BIL_DESCRIZIONE = dr.GetStringOrDefault(BIL_DESCRIZIONE)
                        bilancio.Descrizione_Malattia = dr.GetStringOrDefault(MAL_DESCRIZIONE)
                        bilancio.Eta_Minima = dr.GetDecimal(BIL_ETA_MINIMA)
                        bilancio.Eta_Massima = dr.GetDecimal(BIL_ETA_MASSIMA)
                        bilancio.BIL_CRANIO = dr.GetBooleanOrDefault(BIL_CRANIO)
                        bilancio.BIL_ALTEZZA = dr.GetBooleanOrDefault(BIL_ALTEZZA)
                        bilancio.BIL_PESO = dr.GetBooleanOrDefault(BIL_PESO)
                        bilancio.BIL_OBBLIGATORIO = dr.GetBooleanOrDefault(BIL_OBBLIGATORIO)

                    End If

                End If

            End Using

            Return bilancio

        End Function

#End Region

#Region " Sezioni "

        Public Function GetSezioni(numeroBilancio As Integer, codiceMalattia As String) As DataTable Implements IAnaBilancioProvider.GetSezioni

            Dim dta As New DataTable()

            With _DAM.QB

                .NewQuery()

                .AddSelectFields(.FC.IsNull("OSB_SEZ_CODICE", "-", DataTypes.Stringa) + " OSB_SEZ_CODICE")
                .AddSelectFields(.FC.IsNull("SEZ_DESCRIZIONE", "-", DataTypes.Stringa) + " SEZ_DESCRIZIONE")
                .AddSelectFields("SEZ_N_SEZIONE")

                .AddTables("T_ANA_OSSERVAZIONI, T_ANA_BILANCI, T_ANA_LINK_OSS_BILANCI, T_ANA_SEZIONI")

                .AddWhereCondition("BIL_NUMERO", Comparatori.Uguale, "OSB_BIL_N_BILANCIO", DataTypes.Join)
                .AddWhereCondition("BIL_MAL_CODICE", Comparatori.Uguale, "OSB_BIL_MAL_CODICE", DataTypes.Join)
                .AddWhereCondition("SEZ_CODICE", Comparatori.Uguale, "OSB_SEZ_CODICE", DataTypes.Join)
                .AddWhereCondition("OSS_CODICE", Comparatori.Uguale, "OSB_OSS_CODICE", DataTypes.Join)
                .AddWhereCondition("BIL_NUMERO", Comparatori.Uguale, numeroBilancio, DataTypes.Numero)
                .AddWhereCondition("BIL_MAL_CODICE", Comparatori.Uguale, codiceMalattia, DataTypes.Stringa)

                .AddGroupByFields("OSB_SEZ_CODICE, SEZ_DESCRIZIONE, SEZ_N_SEZIONE")

                .AddOrderByFields("SEZ_N_SEZIONE")

            End With

            _DAM.BuildDataTable(dta)

            Return dta

        End Function

        Public Function GetAnagraficaSezioni(numeroBilancio As Integer, codiceMalattia As String) As List(Of Entities.BilancioSezione) Implements IAnaBilancioProvider.GetAnagraficaSezioni

            Dim list As List(Of Entities.BilancioSezione) = Nothing

            Dim ownConnection As Boolean = False

            Try
                Dim query As String = "select SEZ_CODICE, SEZ_DESCRIZIONE, SEZ_N_SEZIONE, SEZ_BIL_NUMERO, SEZ_BIL_MAL_CODICE " +
                    " from T_ANA_SEZIONI " +
                    " where SEZ_BIL_NUMERO = :SEZ_BIL_NUMERO " +
                    " and SEZ_BIL_MAL_CODICE = :SEZ_BIL_MAL_CODICE " +
                    " order by SEZ_N_SEZIONE "

                Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("SEZ_BIL_NUMERO", numeroBilancio)
                    cmd.Parameters.AddWithValue("SEZ_BIL_MAL_CODICE", codiceMalattia)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim sez_codice As Integer = idr.GetOrdinal("SEZ_CODICE")
                            Dim sez_descrizione As Integer = idr.GetOrdinal("SEZ_DESCRIZIONE")
                            Dim sez_n_sezione As Integer = idr.GetOrdinal("SEZ_N_SEZIONE")
                            Dim sez_bil_numero As Integer = idr.GetOrdinal("SEZ_BIL_NUMERO")
                            Dim sez_bil_mal_codice As Integer = idr.GetOrdinal("SEZ_BIL_MAL_CODICE")

                            list = New List(Of Entities.BilancioSezione)()

                            While idr.Read()

                                Dim item As New Entities.BilancioSezione()
                                item.CodiceSezione = idr.GetString(sez_codice)
                                item.DescrizioneSezione = idr.GetStringOrDefault(sez_descrizione)
                                item.Ordine = idr.GetInt32OrDefault(sez_n_sezione)
                                item.NumeroBilancio = idr.GetInt32OrDefault(sez_bil_numero)
                                item.CodiceMalattia = idr.GetStringOrDefault(sez_bil_mal_codice)

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

        Public Function CountOsservazioniSezione(codiceSezione As String) As Integer Implements IAnaBilancioProvider.CountOsservazioniSezione

            Dim count As Integer = 0

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand("select COUNT(*) from T_ANA_LINK_OSS_BILANCI where OSB_SEZ_CODICE = :OSB_SEZ_CODICE", Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("OSB_SEZ_CODICE", codiceSezione)

                    Dim obj As Object = cmd.ExecuteScalar()
                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then

                        count = Convert.ToInt32(obj)

                    End If

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return count

        End Function

        Public Function InsertSezione(sezione As Entities.BilancioSezione) As Integer Implements IAnaBilancioProvider.InsertSezione

            Dim count As Integer = 0

            Dim ownConnection As Boolean = False

            Try
                Dim query As String = "insert into T_ANA_SEZIONI (SEZ_DESCRIZIONE, SEZ_BIL_NUMERO, SEZ_BIL_MAL_CODICE, SEZ_N_SEZIONE) " +
                    " values (:SEZ_DESCRIZIONE, :SEZ_BIL_NUMERO, :SEZ_BIL_MAL_CODICE, :SEZ_N_SEZIONE) "

                Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("SEZ_DESCRIZIONE", GetStringParam(sezione.DescrizioneSezione))
                    cmd.Parameters.AddWithValue("SEZ_BIL_NUMERO", sezione.NumeroBilancio)
                    cmd.Parameters.AddWithValue("SEZ_BIL_MAL_CODICE", sezione.CodiceMalattia)
                    cmd.Parameters.AddWithValue("SEZ_N_SEZIONE", sezione.Ordine)

                    count = cmd.ExecuteNonQuery()

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return count

        End Function

        Public Function UpdateSezione(sezione As Entities.BilancioSezione) As Integer Implements IAnaBilancioProvider.UpdateSezione

            Dim count As Integer = 0

            Dim ownConnection As Boolean = False

            Try
                Dim query As String = "update T_ANA_SEZIONI set SEZ_DESCRIZIONE = :SEZ_DESCRIZIONE, SEZ_N_SEZIONE = :SEZ_N_SEZIONE where SEZ_CODICE = :SEZ_CODICE "

                Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("SEZ_DESCRIZIONE", GetStringParam(sezione.DescrizioneSezione))
                    cmd.Parameters.AddWithValue("SEZ_N_SEZIONE", sezione.Ordine)
                    cmd.Parameters.AddWithValue("SEZ_CODICE", sezione.CodiceSezione)

                    count = cmd.ExecuteNonQuery()

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return count

        End Function

        Public Function DeleteSezione(codiceSezione As String) As Integer Implements IAnaBilancioProvider.DeleteSezione

            Dim count As Integer = 0

            Dim ownConnection As Boolean = False

            Try
                Dim query As String = "delete from T_ANA_SEZIONI where SEZ_CODICE = :SEZ_CODICE "

                Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("SEZ_CODICE", codiceSezione)

                    count = cmd.ExecuteNonQuery()

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return count

        End Function

#End Region

#Region " Osservazioni-Risposte-Condizioni "

        Public Function GetDomande(numeroBilancio As Integer, codiceMalattia As String, sessoPaziente As String) As DataTable Implements IAnaBilancioProvider.GetDomande
            Return GetDomande(numeroBilancio, codiceMalattia, sessoPaziente, Nothing)
        End Function

        Public Function GetDomande(numeroBilancio As Integer, codiceMalattia As String, sessoPaziente As String, dataRegistrazione As Date) As DataTable Implements IAnaBilancioProvider.GetDomande

            Dim dta As New DataTable()

            With _DAM.QB

                .NewQuery()

                .AddSelectFields("OSS_CODICE, OSS_DESCRIZIONE, BIL_NUMERO, BIL_MAL_CODICE, OSS_TIPO_RISPOSTA")
                .AddSelectFields(.FC.IsNull("SEZ_DESCRIZIONE", "-", DataTypes.Stringa) + " SEZ_DESCRIZIONE")
                .AddSelectFields("OSB_SEZ_CODICE, OSB_N_OSSERVAZIONE, OSB_OBBLIGATORIA")
                .AddSelectFields("OSS_DATA_INIZIO_VALIDITA, OSS_DATA_FINE_VALIDITA")

                .AddTables("T_ANA_OSSERVAZIONI, T_ANA_BILANCI, T_ANA_LINK_OSS_BILANCI, T_ANA_SEZIONI")

                .AddWhereCondition("BIL_NUMERO", Comparatori.Uguale, "OSB_BIL_N_BILANCIO", DataTypes.Join)
                .AddWhereCondition("BIL_MAL_CODICE", Comparatori.Uguale, "OSB_BIL_MAL_CODICE", DataTypes.Join)
                .AddWhereCondition("SEZ_CODICE", Comparatori.Uguale, "OSB_SEZ_CODICE", DataTypes.Join)
                .AddWhereCondition("OSS_CODICE", Comparatori.Uguale, "OSB_OSS_CODICE", DataTypes.Join)
                .AddWhereCondition("BIL_NUMERO", Comparatori.Uguale, numeroBilancio, DataTypes.Numero)
                .OpenParanthesis()
                .AddWhereCondition("OSS_SESSO", Comparatori.Uguale, "E", DataTypes.Stringa)
                .AddWhereCondition("OSS_SESSO", Comparatori.Uguale, sessoPaziente, DataTypes.Stringa, "OR")
                .CloseParanthesis()
                .AddWhereCondition("BIL_MAL_CODICE", Comparatori.Uguale, codiceMalattia, DataTypes.Stringa)
                If dataRegistrazione > Date.MinValue Then
                    .OpenParanthesis()
                    .AddWhereCondition("OSS_DATA_INIZIO_VALIDITA", Comparatori.MinoreUguale, dataRegistrazione.Date, DataTypes.Data)
                    .OpenParanthesis()
                    .AddWhereCondition("OSS_DATA_FINE_VALIDITA", Comparatori.MaggioreUguale, dataRegistrazione.Date, DataTypes.Data, "AND")
                    .AddWhereCondition("OSS_DATA_FINE_VALIDITA", Comparatori.Is, "null", DataTypes.Replace, "OR")
                    .CloseParanthesis()
                    .CloseParanthesis()
                End If


                .AddOrderByFields("SEZ_N_SEZIONE, OSB_N_OSSERVAZIONE")

            End With

            _DAM.BuildDataTable(dta)

            Return dta

        End Function

        Public Function GetRispostePossibili(numeroBilancio As Integer, codiceMalattia As String) As DataTable Implements IAnaBilancioProvider.GetRispostePossibili

            Dim dta As New DataTable()

            With _DAM.QB

                .NewQuery()

                .AddSelectFields("RIO_OSS_CODICE, RIS_CODICE, RIS_DESCRIZIONE, RIO_DEFAULT")

                .AddTables("T_ANA_LINK_OSS_BILANCI, T_ANA_OSSERVAZIONI, T_ANA_RISPOSTE, T_ANA_LINK_OSS_RISPOSTE")

                .AddWhereCondition("OSS_CODICE", Comparatori.Uguale, "OSB_OSS_CODICE", DataTypes.Join)
                .AddWhereCondition("RIO_OSS_CODICE", Comparatori.Uguale, "OSS_CODICE", DataTypes.Join)
                .AddWhereCondition("RIS_CODICE", Comparatori.Uguale, "RIO_RIS_CODICE", DataTypes.Join)
                .AddWhereCondition("OSB_BIL_N_BILANCIO", Comparatori.Uguale, numeroBilancio, DataTypes.Numero)
                .AddWhereCondition("OSB_BIL_MAL_CODICE", Comparatori.Uguale, codiceMalattia, DataTypes.Stringa)

                .AddOrderByFields("RIO_N_RISPOSTA")

            End With

            _DAM.BuildDataTable(dta)

            Return dta

        End Function

        Public Function GetCondizioni(numeroBilancio As Integer, codiceMalattia As String) As DataTable Implements IAnaBilancioProvider.GetCondizioni

            Return Me.GetCondizioni(numeroBilancio, codiceMalattia, Nothing)

        End Function

        Public Function GetCondizioni(numeroBilancio As Integer, codiceMalattia As String, osservazione As String) As DataTable Implements IAnaBilancioProvider.GetCondizioni

            Dim dtaCondizioni As New DataTable()

            With _DAM.QB

                .NewQuery()

                .AddSelectFields("T_ANA_LINK_BIL_OSS_RISP_OSS.*, OSS_DESCRIZIONE, RIS_DESCRIZIONE")

                .AddTables("T_ANA_LINK_BIL_OSS_RISP_OSS, T_ANA_OSSERVAZIONI, T_ANA_RISPOSTE")

                .AddWhereCondition("OSS_CODICE", Comparatori.Uguale, "LRO_OSS_CODICE", DataTypes.Join)
                .AddWhereCondition("RIS_CODICE", Comparatori.Uguale, "LRO_RIS_CODICE_COLLEGATA", DataTypes.Join)
                .AddWhereCondition("LRO_BIL_N_BILANCIO", Comparatori.Uguale, numeroBilancio, DataTypes.Numero)
                .AddWhereCondition("LRO_BIL_MAL_CODICE", Comparatori.Uguale, codiceMalattia, DataTypes.Stringa)

                If Not String.IsNullOrEmpty(osservazione) Then
                    .AddWhereCondition("LRO_OSS_CODICE_COLLEGATA", Comparatori.Uguale, osservazione, DataTypes.Stringa)
                End If

            End With

            _DAM.BuildDataTable(dtaCondizioni)

            Return dtaCondizioni

        End Function

        Public Function GetOsservazioni() As List(Of Entities.BilancioOsservazioneAnagrafica) Implements IAnaBilancioProvider.GetOsservazioni

            Dim list As New List(Of Entities.BilancioOsservazioneAnagrafica)()

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand("select OSS_CODICE, OSS_DESCRIZIONE, OSS_TIPO_RISPOSTA from T_ANA_OSSERVAZIONI order by OSS_CODICE", Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim oss_codice As Integer = idr.GetOrdinal("OSS_CODICE")
                            Dim oss_descrizione As Integer = idr.GetOrdinal("OSS_DESCRIZIONE")
                            Dim oss_tipo_risposta As Integer = idr.GetOrdinal("OSS_TIPO_RISPOSTA")

                            While idr.Read()

                                Dim item As New Entities.BilancioOsservazioneAnagrafica()
                                item.CodiceOsservazione = idr.GetString(oss_codice)
                                item.DescrizioneOsservazione = idr.GetString(oss_descrizione)
                                item.TipoRisposta = idr.GetStringOrDefault(oss_tipo_risposta)

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

        Public Function GetOsservazioniAssociate(numeroBilancio As Integer, codiceMalattia As String) As List(Of Entities.BilancioOsservazioneAssociata) Implements IAnaBilancioProvider.GetOsservazioniAssociate

            Dim list As New List(Of Entities.BilancioOsservazioneAssociata)()

            Dim ownConnection As Boolean = False

            Try
                Dim query As String = " select OSB_BIL_N_BILANCIO, OSB_OSS_CODICE, OSB_N_OSSERVAZIONE, OSB_SEZ_CODICE, OSB_BIL_MAL_CODICE, OSB_OBBLIGATORIA, OSS_DESCRIZIONE, OSS_TIPO_RISPOSTA, SEZ_N_SEZIONE " +
                    " from T_ANA_LINK_OSS_BILANCI " +
                    " join T_ANA_OSSERVAZIONI on OSB_OSS_CODICE = OSS_CODICE " +
                    " join T_ANA_SEZIONI on OSB_SEZ_CODICE = SEZ_CODICE " +
                    " where OSB_BIL_N_BILANCIO = :OSB_BIL_N_BILANCIO " +
                    " and OSB_BIL_MAL_CODICE = :OSB_BIL_MAL_CODICE " +
                    " order by SEZ_N_SEZIONE, OSB_N_OSSERVAZIONE "

                Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("OSB_BIL_N_BILANCIO", numeroBilancio)
                    cmd.Parameters.AddWithValue("OSB_BIL_MAL_CODICE", codiceMalattia)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim osb_bil_n_bilancio As Integer = idr.GetOrdinal("OSB_BIL_N_BILANCIO")
                            Dim osb_oss_codice As Integer = idr.GetOrdinal("OSB_OSS_CODICE")
                            Dim osb_n_osservazione As Integer = idr.GetOrdinal("OSB_N_OSSERVAZIONE")
                            Dim osb_sez_codice As Integer = idr.GetOrdinal("OSB_SEZ_CODICE")
                            Dim osb_bil_mal_codice As Integer = idr.GetOrdinal("OSB_BIL_MAL_CODICE")
                            Dim osb_obbligatoria As Integer = idr.GetOrdinal("OSB_OBBLIGATORIA")
                            Dim oss_descrizione As Integer = idr.GetOrdinal("OSS_DESCRIZIONE")
                            Dim oss_tipo_risposta As Integer = idr.GetOrdinal("OSS_TIPO_RISPOSTA")

                            While idr.Read()

                                Dim item As New Entities.BilancioOsservazioneAssociata()

                                item.NumeroBilancio = idr.GetInt32OrDefault(osb_bil_n_bilancio)
                                item.CodiceOsservazione = idr.GetStringOrDefault(osb_oss_codice)
                                item.NumeroOsservazione = idr.GetInt32OrDefault(osb_n_osservazione)
                                item.CodiceSezione = idr.GetString(osb_sez_codice)
                                item.CodiceMalattia = idr.GetStringOrDefault(osb_bil_mal_codice)
                                item.IsObbligatoria = idr.GetBooleanOrDefault(osb_obbligatoria)
                                item.DescrizioneOsservazione = idr.GetStringOrDefault(oss_descrizione)
                                item.TipoRisposta = idr.GetStringOrDefault(oss_tipo_risposta)

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
        ''' Cancella tutte le osservazioni associate al bilancio specificato
        ''' </summary>
        ''' <param name="numeroBilancio"></param>
        ''' <param name="codiceMalattia"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteOsservazioniAssociate(numeroBilancio As Integer, codiceMalattia As String) As Integer Implements IAnaBilancioProvider.DeleteOsservazioniAssociate

            Dim count As Integer = 0

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand("delete from t_ana_link_oss_bilanci where osb_bil_n_bilancio = :osb_bil_n_bilancio and osb_bil_mal_codice = :osb_bil_mal_codice", Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("osb_bil_n_bilancio", numeroBilancio)
                    cmd.Parameters.AddWithValue("osb_bil_mal_codice", codiceMalattia)
                    count = cmd.ExecuteNonQuery()

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return count

        End Function

        ''' <summary>
        ''' Inserimento osservazione associata al bilancio
        ''' </summary>
        ''' <param name="osservazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsertOsservazioneAssociata(osservazione As BilancioOsservazione) As Integer Implements IAnaBilancioProvider.InsertOsservazioneAssociata

            Dim count As Integer = 0

            Dim ownConnection As Boolean = False

            Dim query As String =
                "insert into t_ana_link_oss_bilanci (osb_bil_n_bilancio, osb_bil_mal_codice, osb_oss_codice, osb_n_osservazione, osb_sez_codice, osb_obbligatoria) " +
                "values (:osb_bil_n_bilancio, :osb_bil_mal_codice, :osb_oss_codice, :osb_n_osservazione, :osb_sez_codice, :osb_obbligatoria)"

            Try
                Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("osb_bil_n_bilancio", osservazione.NumeroBilancio)
                    cmd.Parameters.AddWithValue("osb_bil_mal_codice", osservazione.CodiceMalattia)
                    cmd.Parameters.AddWithValue("osb_oss_codice", osservazione.CodiceOsservazione)

                    If osservazione.NumeroOsservazione.HasValue Then
                        cmd.Parameters.AddWithValue("osb_n_osservazione", osservazione.NumeroOsservazione)
                    Else
                        cmd.Parameters.AddWithValue("osb_n_osservazione", DBNull.Value)
                    End If

                    If String.IsNullOrWhiteSpace(osservazione.CodiceSezione) Then
                        cmd.Parameters.AddWithValue("osb_sez_codice", DBNull.Value)
                    Else
                        cmd.Parameters.AddWithValue("osb_sez_codice", osservazione.CodiceSezione)
                    End If

                    If osservazione.IsObbligatoria Then
                        cmd.Parameters.AddWithValue("osb_obbligatoria", "S")
                    Else
                        cmd.Parameters.AddWithValue("osb_obbligatoria", "N")
                    End If

                    count = cmd.ExecuteNonQuery()

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return count

        End Function

        ''' <summary>
        ''' Log delle modifiche alle osservazioni relative ai bilanci.
        ''' </summary>
        ''' <param name="osservazione"></param>
        ''' <param name="operazione"></param>
        ''' <param name="dataLog"></param>
        ''' <param name="idUtente"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function WriteLogOsservazione(osservazione As Entities.BilancioOsservazioneAssociata, operazione As Integer, dataLog As DateTime, idUtente As Long) As Integer Implements IAnaBilancioProvider.WriteLogOsservazione

            Dim count As Integer = 0

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand()

                    cmd.Connection = Me.Connection
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.CommandText = "select SEQ_LOG_MOD_BILANCI.nextval from DUAL"

                    Dim obj As Object = cmd.ExecuteScalar()

                    Dim id As Long = Convert.ToInt64(obj)

                    cmd.Parameters.Clear()
                    cmd.CommandText = "insert into T_LOG_MOD_BILANCI (tlm_id, tlm_data, tlm_ute_id, tlm_oss_codice, tlm_operazione) values (:tlm_id, :tlm_data, :tlm_ute_id, :tlm_oss_codice, :tlm_operazione)"

                    cmd.Parameters.AddWithValue("tlm_id", id)
                    cmd.Parameters.AddWithValue("tlm_data", dataLog)
                    cmd.Parameters.AddWithValue("tlm_ute_id", idUtente)
                    cmd.Parameters.AddWithValue("tlm_oss_codice", osservazione.CodiceOsservazione)
                    cmd.Parameters.AddWithValue("tlm_operazione", operazione)

                    count = cmd.ExecuteNonQuery()

                    cmd.Parameters.Clear()
                    cmd.CommandText = "insert into T_LOG_LINK_MOD_BILANCI (tlb_tlm_id, tlb_bil_numero, tlb_mal_codice) values (:tlb_tlm_id, :tlb_bil_numero, :tlb_mal_codice)"

                    cmd.Parameters.AddWithValue("tlb_tlm_id", id)
                    cmd.Parameters.AddWithValue("tlb_bil_numero", osservazione.NumeroBilancio)
                    cmd.Parameters.AddWithValue("tlb_mal_codice", osservazione.CodiceMalattia)

                    cmd.ExecuteNonQuery()

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return count

        End Function

#End Region

#End Region

    End Class

End Namespace
