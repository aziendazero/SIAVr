Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager

Namespace DAL.Oracle

    Public Class DbBilancioProgrammatoProvider
        Inherits DbProvider
        Implements IBilancioProgrammatoProvider

#Region " Private Members "

        Private _DR As IDataReader

#End Region

#Region " Constructors "

        Public Sub New(DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " Private "

        ''' <summary>
        ''' Restituisce l'intero corrispondente all'oggetto specificato
        ''' Se l'oggetto vale Nothing o DbNull, restituisce 0
        ''' Se non riesce a convertire l'oggetto, restituisce -1
        ''' </summary>
        Private Function GetIntFromObject(obj As Object) As Integer

            Dim val As Integer = 0

            If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then

                Try
                    val = Convert.ToInt32(obj)
                Catch
                    val = -1
                End Try

            End If

            Return val

        End Function

        ''' <summary>
        ''' Restituisce true se l'oggetto vale 1
        ''' </summary>
        Private Function GetBooleanFromObject(obj As Object) As Boolean

            Dim val As Integer = 0

            If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then

                Try
                    val = Convert.ToInt32(obj)
                Catch
                    val = -1
                End Try

            End If

            Return (val = 1)

        End Function

        ''' <summary>
        ''' Restituisce la data corrispondente all'oggetto specificato
        ''' </summary>
        Private Function GetDateFromObject(obj As Object) As Date

            Dim val As Date = Date.MinValue

            If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then

                Try
                    val = Convert.ToDateTime(obj)
                Catch
                    val = Date.MinValue
                End Try

            End If

            Return val

        End Function

#End Region

#Region " Count "

        Public Function CountBilanciPaziente(codicePaziente As String) As Integer Implements IBilancioProgrammatoProvider.CountBilanciPaziente

            Dim countBilanci As Integer = 0

            With _DAM.QB

                .NewQuery()

                .IsDistinct = True

                .AddSelectFields("VOS_BIL_N_BILANCIO, VOS_MAL_CODICE")

                .AddTables("T_VIS_OSSERVAZIONI, T_ANA_BILANCI, T_ANA_MALATTIE")

                .AddWhereCondition("BIL_NUMERO", Comparatori.Uguale, "VOS_BIL_N_BILANCIO", DataTypes.Join)
                .AddWhereCondition("BIL_MAL_CODICE", Comparatori.Uguale, "VOS_MAL_CODICE", DataTypes.Join)
                .AddWhereCondition("VOS_MAL_CODICE", Comparatori.Uguale, "MAL_CODICE", DataTypes.Join)
                .AddWhereCondition("VOS_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)

                Dim queryBilanci As String = .GetSelect()

                .NewQuery(False, False)

                .AddSelectFields("COUNT(*)")
                .AddTables(String.Format("({0})", queryBilanci))

            End With

            Dim obj As Object = _DAM.ExecScalar()

            If obj Is Nothing OrElse obj Is DBNull.Value Then
                countBilanci = 0
            Else
                countBilanci = Convert.ToInt32(obj)
            End If

            Return countBilanci

        End Function

#End Region

#Region " Metodi di Select "

        Public Function CountFromKey(codicePaziente As Integer, dataConvocazione As Date) As Integer Implements IBilancioProgrammatoProvider.CountFromKey

            Dim count As Integer = 0

            With _DAM.QB
                .NewQuery()
                .AddTables("T_BIL_PROGRAMMATI")
                .AddSelectFields("nvl(count(*),0)")
                .AddWhereCondition("bip_cnv_data", Comparatori.Uguale, "to_date('" & dataConvocazione.ToString("d") & "','DD/MM/YYYY')", DataTypes.Replace)
                .AddWhereCondition("bip_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
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

        Public Function GetBiltoProgr(codicePaziente As Integer) As DataTable Implements IBilancioProgrammatoProvider.GetBiltoProgr

            Me.RefurbishDT()

            With _DAM.QB

                'select bip_mal_codice
                'FROM T_BIL_PROGRAMMATI 
                'WHERE bip_paz_codice = 46196
                'AND bip_stato='UX'
                .NewQuery()
                .AddTables("T_BIL_PROGRAMMATI")
                .AddSelectFields("bip_mal_codice")
                .AddWhereCondition("bip_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("bip_stato", Comparatori.Uguale, Constants.StatiBilancio.UNEXECUTED, DataTypes.Stringa)
                Dim qbilIn As String = "(" + .GetSelect() + ")"

                'select pma_mal_codice
                'FROM T_PAZ_MALATTIE
                'WHERE PMA_PAZ_CODICE = 46196
                'AND PMA_FOLLOW_UP = 'S'
                'AND pma_mal_codice NOT IN ( ...qBilIn... )
                .NewQuery(False, False)
                .AddTables("T_PAZ_MALATTIE")
                .AddSelectFields("pma_mal_codice")
                .AddWhereCondition("pma_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("pma_follow_up", Comparatori.Uguale, "S", DataTypes.Stringa)
                .AddWhereCondition("pma_mal_codice", Comparatori.NotIn, "(" + qbilIn + ")", DataTypes.Replace)
                Dim qMalIn As String = "(" + .GetSelect() + ")"

                'select nvl(max(a.vis_n_bilancio), 0)
                'FROM T_VIS_VISITE a
                'WHERE a.vis_paz_codice = 46196
                'AND a.vis_mal_codice = bil_mal_codice
                .NewQuery(False, False)
                .AddTables("T_VIS_VISITE a")
                .AddSelectFields("nvl(max(a.vis_n_bilancio),0)")
                .AddWhereCondition("a.vis_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("a.vis_mal_codice", Comparatori.Uguale, "bil_mal_codice", DataTypes.Join)
                Dim qVisGre As String = "(" + .GetSelect() + ")"

                'select nvl(max(bip_bil_numero), 0)
                'FROM T_BIL_PROGRAMMATI 
                'WHERE bip_paz_codice = 46196
                'AND bip_mal_codice = bil_mal_codice
                .NewQuery(False, False)
                .AddTables("T_BIL_PROGRAMMATI")
                .AddSelectFields("nvl(max(bip_bil_numero),0)")
                .AddWhereCondition("bip_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("bip_mal_codice", Comparatori.Uguale, "bil_mal_codice", DataTypes.Join)
                Dim qProgGre As String = "(" + .GetSelect() + ")"

                'select nvl(pma_n_bilancio_partenza, 1)
                'FROM T_PAZ_MALATTIE 
                'WHERE pma_paz_codice = 46196
                'AND pma_mal_codice = bil_mal_codice
                .NewQuery(False, False)
                .AddTables("T_PAZ_MALATTIE")
                .AddSelectFields("nvl(pma_n_bilancio_partenza,1)")
                .AddWhereCondition("pma_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("pma_mal_codice", Comparatori.Uguale, "bil_mal_codice", DataTypes.Join)
                Dim qNbil As String = "(" + .GetSelect() + ")"

                ' --- Query principale --- '
                .NewQuery(False, False)
                .AddTables("t_ana_bilanci, t_ana_malattie, t_paz_malattie")
                .AddSelectFields("bil_numero,bil_mal_codice,bil_eta_minima,bil_eta_massima, bil_obbligatorio,mal_flag_visita")
                .AddSelectFields("pma_nuova_diagnosi, pma_data_diagnosi, pma_data_ultima_visita")
                .AddWhereCondition("bil_mal_codice", Comparatori.In, "(" + qMalIn + ")", DataTypes.Replace)
                .AddWhereCondition("bil_numero", Comparatori.Maggiore, "(" + qVisGre + ")", DataTypes.Replace)
                .AddWhereCondition("bil_numero", Comparatori.Maggiore, "(" + qProgGre + ")", DataTypes.Replace)
                .AddWhereCondition("bil_numero", Comparatori.MaggioreUguale, "(" + qNbil + ")", DataTypes.Replace)
                .AddWhereCondition("bil_mal_codice", Comparatori.Uguale, "mal_codice", DataTypes.Join)
                .AddWhereCondition("bil_abilitato", Comparatori.Uguale, "S", DataTypes.Stringa)
                .AddWhereCondition("pma_mal_codice", Comparatori.Uguale, "bil_mal_codice", DataTypes.Join)
                .AddWhereCondition("pma_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)

                .AddOrderByFields("bil_numero,bil_mal_codice ASC")

                Try
                    _DAM.BuildDataTable(_DT)
                Catch ex As Exception
                    Me.LogError(ex)
                    ex.InternalPreserveStackTrace()
                    Throw
                End Try

            End With

            Return _DT.Copy

        End Function

        Public Function Exists(id As Integer) As Boolean Implements IBilancioProgrammatoProvider.Exists

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("1")
                .AddTables("T_BIL_PROGRAMMATI")
                .AddWhereCondition("id", Comparatori.Uguale, id, DataTypes.Numero)
            End With

            Try
                Return GetBooleanFromObject(_DAM.ExecScalar())
            Catch ex As Exception
                Me.LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return False

        End Function

        Public Function GetFromKey(codicePaziente As Integer, numeroBilancio As Integer, codiceMalattia As String) As DataTable Implements IBilancioProgrammatoProvider.GetFromKey

            Me.RefurbishDT()

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("*")
                .AddTables("T_BIL_PROGRAMMATI ")
                .AddWhereCondition("bip_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("bip_bil_numero", Comparatori.Uguale, numeroBilancio, DataTypes.Numero)
                .AddWhereCondition("bip_mal_codice", Comparatori.Uguale, codiceMalattia, DataTypes.Stringa)
            End With

            Try
                _DAM.BuildDataTable(_DT)
            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return _DT.Copy

        End Function

        Public Function GetFromKey(codicePaziente As Integer, dataConvocazione As Date) As DataTable Implements IBilancioProgrammatoProvider.GetFromKey

            RefurbishDT()

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("*")
                .AddTables("T_BIL_PROGRAMMATI ")
                .AddWhereCondition("bip_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("bip_cnv_data", Comparatori.Uguale, dataConvocazione, DataTypes.Data)
            End With

            Try
                _DAM.BuildDataTable(_DT)
            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return _DT.Copy

        End Function

        Public Function GetBilanciMalattiePaziente(codicePaziente As String, dataConvocazione As Date, stato As String) As List(Of KeyValuePair(Of Integer, String)) Implements IBilancioProgrammatoProvider.GetBilanciMalattiePaziente

            Dim bilanciMalattieList As New List(Of KeyValuePair(Of Integer, String))()

            With _DAM.QB

                .NewQuery()

                .AddSelectFields("BIP_BIL_NUMERO", "MAL_DESCRIZIONE")

                .AddTables("T_BIL_PROGRAMMATI", "T_ANA_MALATTIE")

                .AddWhereCondition("bip_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("bip_cnv_data", Comparatori.Uguale, dataConvocazione, DataTypes.Data)
                .AddWhereCondition("mal_codice", Comparatori.Uguale, "bip_mal_codice", DataTypes.Join)

                If Not String.IsNullOrEmpty(stato) Then
                    .AddWhereCondition("bip_stato", Comparatori.Uguale, stato, DataTypes.Stringa)
                End If

            End With

            Try

                Using idr As IDataReader = _DAM.BuildDataReader()

                    If Not idr Is Nothing Then

                        Dim bip_bil_numero As Integer = idr.GetOrdinal("BIP_BIL_NUMERO")
                        Dim mal_descrizione As Integer = idr.GetOrdinal("MAL_DESCRIZIONE")

                        While idr.Read()

                            If Not idr.IsDBNull(bip_bil_numero) Then

                                bilanciMalattieList.Add(New KeyValuePair(Of Integer, String)(idr.GetDecimal(bip_bil_numero), idr.GetStringOrDefault(mal_descrizione)))

                            End If

                        End While

                    End If

                End Using

            Catch ex As Exception
                Me.LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return bilanciMalattieList

        End Function

        Public Function ManGetBiltoProgr(codicePaziente As Integer, codiceMalattia As String) As IDataReader Implements IBilancioProgrammatoProvider.ManGetBiltoProgr

            With _DAM.QB

                '--
                ' SELECT BIP_BIL_NUMERO
                ' FROM T_BIL_PROGRAMMATI 
                ' WHERE bip_paz_codice = 18769
                ' AND bip_mal_codice = bil_mal_codice
                ' AND (bip_stato = 'UX' OR bip_stato = 'EX')
                '--
                .NewQuery()

                .AddSelectFields("bip_bil_numero")

                .AddTables("T_BIL_PROGRAMMATI")

                .AddWhereCondition("bip_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("bip_mal_codice", Comparatori.Uguale, "bil_mal_codice", DataTypes.Join)

                .OpenParanthesis()

                .AddWhereCondition("bip_stato", Comparatori.Uguale, Constants.StatiBilancio.UNEXECUTED, DataTypes.Stringa)
                .AddWhereCondition("bip_stato", Comparatori.Uguale, Constants.StatiBilancio.EXECUTED, DataTypes.Stringa, "OR")

                .CloseParanthesis()

                Dim qMinNBil As String = "(" + .GetSelect() + ")"

                '--
                ' SELECT bil_numero, bil_mal_codice, mal_descrizione
                ' FROM T_PAZ_MALATTIE, T_ANA_BILANCI, T_ANA_MALATTIE
                ' WHERE bil_numero >= NVL(PMA_N_BILANCIO_PARTENZA,0)
                ' AND bil_mal_codice = mal_codice 
                ' AND pma_mal_codice = bil_mal_codice
                ' AND bil_mal_codice = :codiceMalattia  -- solo se c'è il cod malattia
                ' AND pma_paz_codice = 18769
                ' AND bil_abilitato = 'S'
                ' AND bil_numero NOT IN (qMinNBil)
                ' AND PMA_FOLLOW_UP = 'S'
                ' ORDER BY bil_mal_codice, bil_numero ASC
                '--
                .NewQuery(False, False)

                .AddSelectFields("bil_numero, bil_mal_codice, mal_descrizione")

                .AddTables("T_PAZ_MALATTIE, T_ANA_BILANCI, T_ANA_MALATTIE")

                .AddWhereCondition("bil_numero", Comparatori.MaggioreUguale, "NVL(pma_n_bilancio_partenza,0)", DataTypes.Replace)
                .AddWhereCondition("bil_mal_codice", Comparatori.Uguale, "mal_codice", DataTypes.Join)
                .AddWhereCondition("pma_mal_codice", Comparatori.Uguale, "bil_mal_codice", DataTypes.Join)

                If (codiceMalattia <> String.Empty) Then
                    .AddWhereCondition("bil_mal_codice", Comparatori.Uguale, codiceMalattia, DataTypes.Stringa)
                End If

                .AddWhereCondition("pma_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("bil_abilitato", Comparatori.Uguale, "S", DataTypes.Stringa)
                .AddWhereCondition("bil_numero", Comparatori.NotIn, qMinNBil, DataTypes.Replace)
                .AddWhereCondition("pma_follow_up", Comparatori.Uguale, "S", DataTypes.Stringa)

                .AddOrderByFields("bil_mal_codice, bil_numero ASC")

            End With

            Try
                _DR = _DAM.BuildDataReader()
            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return _DR

        End Function

        Public Function RecordCount(filter As Object) As Integer Implements IBilancioProgrammatoProvider.RecordCount

            Throw New NotImplementedException()

        End Function

        Public Function VaccinazioneAssociata(codicePaziente As Integer, dataConvocazione As Date) As Boolean Implements IBilancioProgrammatoProvider.VaccinazioneAssociata

            Dim result As Boolean

            _DT.Clear()

            Try
                With _DAM.QB
                    .NewQuery()
                    .AddSelectFields("1")
                    .AddTables("T_VAC_PROGRAMMATE")
                    .AddWhereCondition("BIP_PAZ_CODICE", Comparatori.Uguale, "VPR_PAZ_CODICE", DataTypes.Join)
                    .AddWhereCondition("BIP_CNV_DATA", Comparatori.Uguale, "VPR_CNV_DATA", DataTypes.Join)
                    Dim queryVacProg As String = .GetSelect()

                    .NewQuery(False, False)
                    .AddSelectFields("BIP_PAZ_CODICE")
                    .AddTables("T_BIL_PROGRAMMATI")
                    .AddWhereCondition("BIP_BIL_NUMERO", Comparatori.IsNot, "null", DataTypes.Replace)
                    .AddWhereCondition("BIP_BIL_NUMERO", Comparatori.Diverso, "0", DataTypes.Numero)
                    .AddWhereCondition("", Comparatori.NotExist, " (" + queryVacProg + ") ", DataTypes.Replace)
                    .AddWhereCondition("BIP_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                    .AddWhereCondition("BIP_CNV_DATA", Comparatori.Uguale, dataConvocazione, DataTypes.Data)
                End With

                result = (Not _DAM.ExecScalar() Is Nothing)

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return result

        End Function

        ''' <summary>
        ''' Effettua una query sulle tabelle "t_paz_pazienti", "t_cnv_convocazioni", "t_bil_programmati", "t_bil_solleciti", "t_ana_bilanci" 
        ''' per verificare se un bilancio di un determinato paziente è finito fuori dalla fascia di età corretta.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns>Lista bilanci fuori età del paziente specificato.</returns>
        ''' <remarks> Valori di stato ammessi: EX, UX, US</remarks>
        Public Function GetBilanciDaSollecitare(codicePaziente As Integer) As List(Of Entities.BilancioDaSollecitare) Implements IBilancioProgrammatoProvider.GetBilanciDaSollecitare

            Dim list As New List(Of Entities.BilancioDaSollecitare)()

            With _DAM.QB

                .NewQuery()

                .AddSelectFields("paz_codice, paz_cognome, paz_nome, paz_data_nascita, paz_cns_codice, cnv_data, cnv_data_invio")
                .AddSelectFields("cnv_data_appuntamento, id as bip_id, bip_mal_codice, bip_bil_numero, bip_stato, bil_intervallo")
                .AddSelectFields("bil_eta_minima, bil_eta_massima, trunc(cnv_data_appuntamento - paz_data_nascita) AS eta_appuntamento")
                .AddSelectFields("bil_obbligatorio, bil_giorni_sollecito, bil_num_solleciti, bil_scadenza_dopo")
                .AddSelectFields("DECODE ( (DECODE ((SELECT COUNT (*) FROM t_bil_programmati WHERE bip_paz_codice = cnv_paz_codice AND bip_cnv_data = cnv_data),1, 'NOBI','SIBI') || (DECODE ((SELECT COUNT (*) FROM t_vac_programmate WHERE vpr_paz_codice = cnv_paz_codice AND vpr_cnv_data = cnv_data), 0, 'NOVA','SIVA'))),'NOBINOVA', 'TRUE','FALSE') AS solo_bilancio")
                .AddSelectFields("(select count(*)from t_bil_solleciti a where t_bil_programmati.ID = a.bis_bip_id(+)) as solleciti_creati")
                .AddSelectFields("NVL((SELECT b.bis_data_invio FROM t_bil_solleciti b WHERE(b.bis_bip_id = t_bil_programmati.ID) AND b.ID = (SELECT MAX (ID) FROM t_bil_solleciti c WHERE c.bis_bip_id = t_bil_programmati.ID)),bip_data_invio) AS ultima_data_invio")

                .AddTables("t_paz_pazienti", "t_cnv_convocazioni", "t_bil_programmati", "t_ana_bilanci")

                .AddWhereCondition("cnv_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)

                '---
                ' inizio join
                .AddWhereCondition("cnv_paz_codice", Comparatori.Uguale, "paz_codice", DataTypes.Join)
                .AddWhereCondition("cnv_cns_codice", Comparatori.Uguale, "paz_cns_codice", DataTypes.Join)

                .AddWhereCondition("cnv_paz_codice", Comparatori.Uguale, "bip_paz_codice", DataTypes.Join)
                .AddWhereCondition("cnv_data", Comparatori.Uguale, "bip_cnv_data", DataTypes.Join)

                .AddWhereCondition("bip_bil_numero", Comparatori.Uguale, "bil_numero", DataTypes.OutJoinLeft)
                .AddWhereCondition("bip_mal_codice", Comparatori.Uguale, "bil_mal_codice", DataTypes.OutJoinLeft)
                ' fine join
                '---

                .AddWhereCondition("bip_stato", Comparatori.Uguale, Constants.StatiBilancio.UNEXECUTED, DataTypes.Stringa)
                .AddWhereCondition("(cnv_data_appuntamento - paz_data_nascita)", Comparatori.Minore, "bil_eta_massima", DataTypes.Join)
                .AddWhereCondition("cnv_data_appuntamento + bil_giorni_sollecito", Comparatori.MinoreUguale, "(select sysdate from dual)", DataTypes.Replace)

            End With

            Using idr As IDataReader = _DAM.BuildDataReader()
                If Not idr Is Nothing Then

                    Dim paz_codice As Integer = idr.GetOrdinal("paz_codice")
                    Dim bip_id As Integer = idr.GetOrdinal("bip_id")
                    Dim solleciti_creati As Integer = idr.GetOrdinal("solleciti_creati")
                    Dim ultima_data_invio As Integer = idr.GetOrdinal("ultima_data_invio")
                    Dim bil_num_solleciti As Integer = idr.GetOrdinal("bil_num_solleciti")
                    Dim solo_bilancio As Integer = idr.GetOrdinal("solo_bilancio")
                    Dim cnv_data As Integer = idr.GetOrdinal("cnv_data")
                    Dim bip_mal_codice As Integer = idr.GetOrdinal("bip_mal_codice")
                    Dim bip_bil_numero As Integer = idr.GetOrdinal("bip_bil_numero")
                    Dim bip_stato As Integer = idr.GetOrdinal("bip_stato")

                    While idr.Read()

                        Dim item As New Entities.BilancioDaSollecitare()

                        item.CodicePaziente = idr.GetInt32OrDefault(paz_codice)
                        item.IdBilancio = idr.GetInt32OrDefault(bip_id)
                        item.DataConvocazione = idr.GetNullableDateTimeOrDefault(cnv_data)
                        item.NumeroBilancio = idr.GetInt32OrDefault(bip_bil_numero)
                        item.CodiceMalattia = idr.GetStringOrDefault(bip_mal_codice)
                        item.StatoBilancio = idr.GetStringOrDefault(bip_stato)
                        item.SollecitiCreati = idr.GetInt32OrDefault(solleciti_creati)
                        item.UltimaDataInvio = idr.GetNullableDateTimeOrDefault(ultima_data_invio)
                        item.NumeroSollecitiBilancio = idr.GetInt32OrDefault(bil_num_solleciti)

                        If Not idr.IsDBNull(solo_bilancio) Then
                            item.FlagSoloBilancio = (idr.GetStringOrDefault(solo_bilancio) = "TRUE")
                        Else
                            item.FlagSoloBilancio = False
                        End If

                        list.Add(item)

                    End While

                End If
            End Using

            Return list

        End Function

        ''' <summary>
        ''' Restituisce i bilanci non ancora eseguiti e fuori età massima, per il paziente specificato.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        Public Function GetInfoBilanciNonEseguitiScaduti(codicePaziente As Integer) As List(Of IBilancioProgrammatoProvider.InfoBilancio) Implements IBilancioProgrammatoProvider.GetInfoBilanciNonEseguitiScaduti

            Dim listInfoBilanci As New List(Of IBilancioProgrammatoProvider.InfoBilancio)()

            With _DAM.QB

                .NewQuery()

                .AddSelectFields("id bip_id, bip_bil_numero, paz_codice, bip_mal_codice, cnv_data, bip_stato")
                .AddSelectFields("DECODE ((DECODE ((SELECT COUNT (*) FROM t_bil_programmati WHERE bip_paz_codice = cnv_paz_codice AND bip_cnv_data = cnv_data),0, 'NOBI','SIBI') || (DECODE ((SELECT COUNT (*) FROM t_vac_programmate WHERE vpr_paz_codice = cnv_paz_codice AND vpr_cnv_data = cnv_data), 0, 'NOVA','SIVA'))),'NOBINOVA', 'S','N') AS solo_bilancio")

                .AddTables("t_paz_pazienti, t_cnv_convocazioni, t_bil_programmati, t_ana_bilanci")

                .AddWhereCondition("cnv_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("bip_stato", Comparatori.Uguale, Constants.StatiBilancio.UNEXECUTED, DataTypes.Stringa)
                .AddWhereCondition("((select sysdate from dual) - paz_data_nascita)", Comparatori.Maggiore, "bil_eta_massima", DataTypes.Join)

                .AddWhereCondition("paz_codice", Comparatori.Uguale, "cnv_paz_codice", DataTypes.Join)
                .AddWhereCondition("bip_paz_codice", Comparatori.Uguale, "cnv_paz_codice", DataTypes.Join)
                .AddWhereCondition("bip_cnv_data", Comparatori.Uguale, "cnv_data", DataTypes.Join)
                .AddWhereCondition("bip_bil_numero", Comparatori.Uguale, "bil_numero", DataTypes.OutJoinLeft)
                .AddWhereCondition("bip_mal_codice", Comparatori.Uguale, "bil_mal_codice", DataTypes.OutJoinLeft)

            End With

            Using idr As IDataReader = _DAM.BuildDataReader()

                If Not idr Is Nothing Then

                    Dim bip_id As Integer = idr.GetOrdinal("bip_id")
                    Dim bip_bil_numero As Integer = idr.GetOrdinal("bip_bil_numero")
                    Dim paz_codice As Integer = idr.GetOrdinal("paz_codice")
                    Dim bip_mal_codice As Integer = idr.GetOrdinal("bip_mal_codice")
                    Dim cnv_data As Integer = idr.GetOrdinal("cnv_data")
                    Dim bip_stato As Integer = idr.GetOrdinal("bip_stato")
                    Dim solo_bilancio As Integer = idr.GetOrdinal("solo_bilancio")

                    Dim infoBilancio As IBilancioProgrammatoProvider.InfoBilancio = Nothing

                    While idr.Read()

                        infoBilancio = New IBilancioProgrammatoProvider.InfoBilancio()

                        infoBilancio.Bilancio.Bil_id = idr.GetInt32(bip_id)
                        infoBilancio.Bilancio.N_bilancio = idr.GetInt32(bip_bil_numero)
                        infoBilancio.Bilancio.Paz_Codice = idr.GetInt32(paz_codice)
                        infoBilancio.Bilancio.Mal_codice = idr.GetStringOrDefault(bip_mal_codice)
                        infoBilancio.Bilancio.Data_CNV = idr.GetDateTimeOrDefault(cnv_data)
                        infoBilancio.Bilancio.Bil_stato = idr.GetString(bip_stato)

                        infoBilancio.IsConvocazioneSoloBilancio = idr.GetBooleanOrDefault(solo_bilancio)

                        listInfoBilanci.Add(infoBilancio)

                    End While

                End If

            End Using

            Return listInfoBilanci

        End Function

        Public Function VerifyInterval(codiceMalattia As String, codicePaziente As Integer, dataPartenza As Date, numeroBilancio As Int16) As Boolean Implements IBilancioProgrammatoProvider.VerifyInterval

            Dim qMxVis As String = String.Empty

            With _DAM.QB

                .NewQuery()

                If codicePaziente <> 0 Then

                    '--
                    '(sysdate - (SELECT max(v.vis_data_visita) 
                    'FROM T_VIS_VISITE v 
                    'WHERE v.vis_paz_codice = '46196'
                    'AND  v.vis_n_bilancio <> '5'
                    'AND  v.vis_mal_codice = '0')) 
                    '--
                    .AddTables("T_VIS_VISITE v")
                    .AddSelectFields("NVL (MAX (v.vis_data_visita), TO_DATE ('1801', 'yyyy'))")
                    .AddWhereCondition("v.vis_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                    .AddWhereCondition("v.vis_mal_codice", Comparatori.Uguale, codiceMalattia, DataTypes.Stringa)
                    .AddWhereCondition("v.vis_n_bilancio", Comparatori.Diverso, numeroBilancio, DataTypes.Numero)
                    qMxVis = "(" + .GetSelect() + ")"

                End If

                '--
                'SELECT '1'
                'FROM T_ANA_BILANCI
                'WHERE bil_numero = '5'
                'AND bil_mal_codice = '0' 
                'AND  ((sysdate)- (data_diagnosi/data_ultima_visita/data_nascita)) >= bil_intervallo
                '--
                .NewQuery(False, False)

                .AddTables("T_ANA_BILANCI")

                .AddSelectFields("1")

                .AddWhereCondition("bil_numero", Comparatori.Uguale, numeroBilancio, DataTypes.Numero)
                .AddWhereCondition("bil_mal_codice", Comparatori.Uguale, codiceMalattia, DataTypes.Stringa)

                If (codicePaziente <> 0) Then
                    .AddWhereCondition("( sysdate - " + qMxVis + ")", Comparatori.MaggioreUguale, "bil_intervallo", DataTypes.Replace)
                Else
                    .AddWhereCondition("" & DateDiff(DateInterval.Day, dataPartenza, Date.Now) & "", Comparatori.MaggioreUguale, "bil_intervallo", DataTypes.Replace)
                End If

                Try
                    _RET = _DAM.ExecScalar()
                Catch ex As Exception
                    LogError(ex)
                    ex.InternalPreserveStackTrace()
                    Throw
                End Try

            End With

            Return (Not _RET Is Nothing)

        End Function

        Public Function VerifyVisits(codiceMalattia As String, codicePaziente As Integer) As Boolean Implements IBilancioProgrammatoProvider.VerifyVisits

            With _DAM.QB
                .NewQuery()
                .AddTables("T_VIS_VISITE")
                .AddSelectFields("1")
                .AddWhereCondition("vis_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("vis_mal_codice", Comparatori.Uguale, codiceMalattia, DataTypes.Stringa)
            End With

            Try
                _RET = _DAM.ExecScalar()
            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return (Not _RET Is Nothing)

        End Function

        ''' <summary>
        ''' Restituisce la data di convocazione più recente tra i bilanci unsolved del paziente, relativi alla malattia specificata
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="codiceMalattia"></param>
        Public Function GetLastDateBilUnsolved(codicePaziente As Integer, codiceMalattia As String) As Date Implements IBilancioProgrammatoProvider.GetLastDateBilUnsolved

            Dim dataConvocazione As Date = Date.MinValue

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("max(bip_cnv_data)")
                .AddTables("t_bil_programmati")
                .AddWhereCondition("bip_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("bip_mal_codice", Comparatori.Uguale, codiceMalattia, DataTypes.Stringa)
                .AddWhereCondition("bip_stato", Comparatori.Uguale, Constants.StatiBilancio.UNSOLVED, DataTypes.Stringa)
            End With

            Try
                dataConvocazione = GetDateFromObject(_DAM.ExecScalar())
            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return dataConvocazione

        End Function

        ''' <summary>
        ''' Restituisce il bilancio con numero più alto per il paziente, la malattia e lo stato specificati.
        ''' Se non lo trova restituisce Nothing.
        ''' </summary>
        Public Function GetLastBil(codicePaziente As Integer, codiceMalattia As String, stato As String) As Entities.BilancioInfo Implements IBilancioProgrammatoProvider.GetLastBil

            Dim bilancioInfo As Entities.BilancioInfo = Nothing

            With _DAM.QB

                .NewQuery()

                .AddSelectFields("max(b2.bip_bil_numero)")

                .AddTables("t_bil_programmati b2")

                .AddWhereCondition("b2.bip_paz_codice", Comparatori.Uguale, "b1.bip_paz_codice", DataTypes.Join)
                .AddWhereCondition("b2.bip_mal_codice", Comparatori.Uguale, "b1.bip_mal_codice", DataTypes.Join)
                .AddWhereCondition("b2.bip_stato", Comparatori.Uguale, "b1.bip_stato", DataTypes.Join)

                Dim subQuery As String = String.Format("({0})", .GetSelect())

                .NewQuery(False, False)

                .AddSelectFields("b1.*, mal_descrizione, mal_flag_visita, cnv_data_appuntamento, cnv_data_invio")

                .AddTables("t_bil_programmati b1, t_ana_malattie, t_cnv_convocazioni")

                .AddWhereCondition("b1.bip_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("b1.bip_mal_codice", Comparatori.Uguale, codiceMalattia, DataTypes.Stringa)
                .AddWhereCondition("b1.bip_stato", Comparatori.Uguale, stato, DataTypes.Stringa)
                .AddWhereCondition("b1.bip_bil_numero", Comparatori.Uguale, subQuery, DataTypes.Replace)
                .AddWhereCondition("b1.bip_mal_codice", Comparatori.Uguale, "mal_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("b1.bip_paz_codice", Comparatori.Uguale, "cnv_paz_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("b1.bip_cnv_data", Comparatori.Uguale, "cnv_data", DataTypes.OutJoinLeft)

            End With

            Using idr As IDataReader = _DAM.BuildDataReader()

                If idr IsNot Nothing Then

                    Dim pos_paz As Integer = idr.GetOrdinal("bip_paz_codice")
                    Dim pos_mal_cod As Integer = idr.GetOrdinal("bip_mal_codice")
                    Dim pos_id As Integer = idr.GetOrdinal("id")
                    Dim pos_app As Integer = idr.GetOrdinal("cnv_data_appuntamento")
                    Dim pos_cnv As Integer = idr.GetOrdinal("bip_cnv_data")
                    Dim pos_invio As Integer = idr.GetOrdinal("cnv_data_invio")
                    Dim pos_mal_descr As Integer = idr.GetOrdinal("mal_descrizione")
                    Dim pos_visita As Integer = idr.GetOrdinal("mal_flag_visita")
                    Dim pos_num As Integer = idr.GetOrdinal("bip_bil_numero")
                    Dim pos_stato As Integer = idr.GetOrdinal("bip_stato")

                    If idr.Read() Then

                        bilancioInfo = New BilancioInfo()

                        bilancioInfo.Id = idr.GetDecimal(pos_id)
                        bilancioInfo.CodicePaziente = idr.GetDecimal(pos_paz)
                        bilancioInfo.NumeroBilancio = idr.GetDecimal(pos_num)
                        bilancioInfo.CodiceMalattia = idr.GetStringOrDefault(pos_mal_cod)
                        bilancioInfo.DescrizioneMalattia = idr.GetStringOrDefault(pos_mal_descr)
                        bilancioInfo.DataConvocazione = idr.GetDateTimeOrDefault(pos_cnv)
                        bilancioInfo.DataAppuntamento = idr.GetDateTimeOrDefault(pos_app)
                        bilancioInfo.DataInvio = idr.GetDateTimeOrDefault(pos_invio)
                        bilancioInfo.FlagVisita = idr.GetBooleanOrDefault(pos_visita)
                        bilancioInfo.Stato = idr.GetStringOrDefault(pos_stato)

                    End If

                End If

            End Using

            Return bilancioInfo

        End Function

#Region " Calcolo dell'intervallo di convocazione di un bilancio "

        ''' <summary>
        ''' Restituisce il numero di giorni per il calcolo dell'intervallo di convocazione del bilancio, 
        ''' in base alla data di visita più recente.
        ''' </summary>
        ''' <param name="codiceMalattia"></param>
        ''' <param name="codicePaziente"></param>
        ''' <param name="numeroBilancio"></param>
        Public Function RetrieveIntervalByVisit(codiceMalattia As String, codicePaziente As Integer, numeroBilancio As Int16) As Integer Implements IBilancioProgrammatoProvider.RetrieveIntervalByVisit

            Dim intervallo As Integer = 0

            With _DAM.QB

                .NewQuery()

                .AddSelectFields("round(bil_intervallo - ((sysdate) - (SELECT max(vis_data_visita) FROM T_VIS_VISITE WHERE vis_paz_codice ='" & codicePaziente _
                    & "' AND vis_n_bilancio <> '" & numeroBilancio & "' AND vis_mal_codice = '" & codiceMalattia & "'))) intervallo")
                .AddTables("T_ANA_BILANCI")
                .AddWhereCondition("bil_numero", Comparatori.Uguale, numeroBilancio, DataTypes.Numero)
                .AddWhereCondition("bil_mal_codice", Comparatori.Uguale, codiceMalattia, DataTypes.Stringa)

                Try
                    intervallo = GetIntFromObject(_DAM.ExecScalar())
                Catch ex As Exception
                    LogError(ex)
                    ex.InternalPreserveStackTrace()
                    Throw
                End Try

            End With

            Return intervallo

        End Function

        ''' <summary>
        ''' Restituisce il numero di giorni per il calcolo dell'intervallo di convocazione del bilancio, 
        ''' in base alla data di partenza specificata.
        ''' </summary>
        ''' <param name="codiceMalattia"></param>
        ''' <param name="codicePaziente"></param>
        ''' <param name="numeroBilancio"></param>
        ''' <param name="datapartenza"></param>
        ''' <param name="isLastDateUS"></param>
        Public Function RetrieveIntervalByDate(codiceMalattia As String, codicePaziente As Integer, numeroBilancio As Int16, dataPartenza As Date, isLastDateUS As Boolean) As Integer Implements IBilancioProgrammatoProvider.RetrieveIntervalByDate

            Dim intervallo As Integer = 0

            Dim campo_intervallo As String = "bil_intervallo"
            If (isLastDateUS) Then campo_intervallo = "bil_tempo_cnv_precedente"

            With _DAM.QB

                .NewQuery()

                .AddSelectFields("round((to_date('" & dataPartenza & "','dd/mm/yyyy') + " + campo_intervallo + ") - sysdate) intervallo")
                .AddTables("T_ANA_BILANCI")
                .AddWhereCondition("bil_numero", Comparatori.Uguale, numeroBilancio, DataTypes.Numero)
                .AddWhereCondition("bil_mal_codice", Comparatori.Uguale, codiceMalattia, DataTypes.Stringa)

                Try
                    intervallo = GetIntFromObject(_DAM.ExecScalar())
                Catch ex As Exception
                    LogError(ex)
                    ex.InternalPreserveStackTrace()
                    Throw
                End Try

            End With

            Return intervallo

        End Function

        ''' <summary>
        ''' Restituisce il numero di giorni per il calcolo dell'intervallo di convocazione del bilancio, 
        ''' in base alla data del bilancio unsolved più recente.
        ''' </summary>
        ''' <param name="codiceMalattia"></param>
        ''' <param name="codicePaziente"></param>
        ''' <param name="numeroBilancio"></param>
        Public Function RetrieveIntervalByBilUnsolved(codiceMalattia As String, codicePaziente As Integer, numeroBilancio As Int16) As Integer Implements IBilancioProgrammatoProvider.RetrieveIntervalByBilUnsolved

            Dim intervallo As Integer = 0

            With _DAM.QB

                .NewQuery()

                .AddSelectFields("MAX(bip_cnv_data)")
                .AddTables("t_bil_programmati")
                .AddWhereCondition("bip_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("bip_bil_numero", Comparatori.Diverso, numeroBilancio, DataTypes.Numero)
                .AddWhereCondition("bip_mal_codice", Comparatori.Uguale, codiceMalattia, DataTypes.Stringa)
                .AddWhereCondition("bip_stato", Comparatori.Uguale, Constants.StatiBilancio.UNSOLVED, DataTypes.Stringa)

                Dim subQuery As String = .GetSelect()

                .NewQuery(False)

                .AddSelectFields("round(bil_tempo_cnv_precedente - (sysdate - (" + subQuery + ")) ) intervallo")
                .AddTables("t_ana_bilanci")
                .AddWhereCondition("bil_numero", Comparatori.Uguale, numeroBilancio, DataTypes.Numero)
                .AddWhereCondition("bil_mal_codice", Comparatori.Uguale, codiceMalattia, DataTypes.Stringa)

                Try
                    intervallo = GetIntFromObject(_DAM.ExecScalar())
                Catch ex As Exception
                    LogError(ex)
                    ex.InternalPreserveStackTrace()
                    Throw
                End Try

            End With

            Return intervallo

        End Function

#End Region

        ''' <summary>
        ''' Restituisce una lista di percentili in base agli anni, al sesso, al tipo e al valore specificati
        ''' </summary>
        ''' <param name="anni"></param>
        ''' <param name="sesso"></param>
        ''' <param name="tipo"></param>
        ''' <param name="valore"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetListPercentili(anni As Integer, sesso As String, tipo As Integer, valore As Double) As List(Of Entities.Percentile) Implements IBilancioProgrammatoProvider.GetListPercentili

            Dim listPercentili As New List(Of Entities.Percentile)()

            With _DAM.QB
                .NewQuery()
                .AddTables("T_ANA_PERCENTILI")
                .AddSelectFields("PER_PERCENTILE, PER_VALORE")
                .AddWhereCondition("PER_SESSO", Comparatori.Uguale, sesso, DataTypes.Stringa)
                .AddWhereCondition("PER_TIPO", Comparatori.Uguale, tipo, DataTypes.Stringa)
                .AddWhereCondition("PER_ETA", Comparatori.MaggioreUguale, anni, DataTypes.Numero)
                .AddWhereCondition("PER_VALORE", Comparatori.MaggioreUguale, valore, DataTypes.Double)
                .AddOrderByFields("PER_ETA, PER_VALORE")
            End With

            Using idr As IDataReader = _DAM.BuildDataReader()

                If Not idr Is Nothing Then

                    Dim per_percentile As Integer = idr.GetOrdinal("PER_PERCENTILE")
                    Dim per_valore As Integer = idr.GetOrdinal("PER_VALORE")

                    Dim percentileItem As Entities.Percentile = Nothing

                    While idr.Read()

                        percentileItem = New Entities.Percentile()

                        ' N.B. : su db il campo è stringa, poi viene usato come double per cui lo tiro su come stringa e lo converto
                        Dim campoPercentile As String = idr.GetStringOrDefault(per_percentile)
                        percentileItem.Percentile = Convert.ToDouble(campoPercentile)
                        percentileItem.Valore = idr.GetDoubleOrDefault(per_valore)

                        listPercentili.Add(percentileItem)

                    End While

                End If

            End Using

            Return listPercentili

        End Function

        Public Function GetUltimaDataVisitaBilancio(numeroBilancio As Integer, codiceMalattia As String, codicePaziente As Integer) As Date Implements IBilancioProgrammatoProvider.GetUltimaDataVisitaBilancio

            Dim dataVisita As Date = Date.MinValue

            Try
                With _DAM.QB
                    .NewQuery()
                    .AddSelectFields("MAX(VIS_DATA_VISITA)")
                    .AddTables("T_VIS_VISITE")
                    .AddWhereCondition("VIS_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                    .AddWhereCondition("VIS_N_BILANCIO", Comparatori.MinoreUguale, numeroBilancio, DataTypes.Numero)
                    .AddWhereCondition("VIS_MAL_CODICE", Comparatori.Uguale, codiceMalattia, DataTypes.Stringa)
                    .AddOrderByFields("VIS_DATA_VISITA DESC")
                End With

                Dim obj As Object = _DAM.ExecScalar()
                If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                    dataVisita = Convert.ToDateTime(obj)
                End If

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return dataVisita

        End Function

        Public Function GetVisitaBilancio(idVisita As Integer) As Visita Implements IBilancioProgrammatoProvider.GetVisitaBilancio

            Dim visita As New Visita()

            Try
                With _DAM.QB

                    .NewQuery()

                    .IsDistinct = True

                    .AddSelectFields("T_VIS_VISITE.*")
                    .AddSelectFields("MAL_DESCRIZIONE, med.OPE_NOME DESCRIZIONE_MEDICO")
                    .AddSelectFields("BIL_ETA_MINIMA, BIL_DESCRIZIONE, BIL_CRANIO, BIL_ALTEZZA, BIL_PESO")
                    .AddSelectFields("ril.OPE_NOME DESCRIZIONE_RILEVATORE, CIT_STATO")

                    .AddTables("T_ANA_BILANCI, T_ANA_MALATTIE, T_VIS_VISITE, T_ANA_UTENTI")
                    .AddTables("T_ANA_OPERATORI med, T_ANA_OPERATORI ril, T_ANA_CITTADINANZE")

                    ' Join
                    .AddWhereCondition("VIS_N_BILANCIO", Comparatori.Uguale, "BIL_NUMERO", DataTypes.Join)
                    .AddWhereCondition("VIS_MAL_CODICE", Comparatori.Uguale, "BIL_MAL_CODICE", DataTypes.Join)
                    .AddWhereCondition("VIS_MAL_CODICE", Comparatori.Uguale, "MAL_CODICE", DataTypes.Join)
                    .AddWhereCondition("VIS_OPE_CODICE", Comparatori.Uguale, "med.OPE_CODICE", DataTypes.OutJoinLeft)
                    .AddWhereCondition("VIS_UTE_ID", Comparatori.Uguale, "UTE_ID", DataTypes.OutJoinLeft)
                    .AddWhereCondition("VIS_OPE_CODICE_RILEVATORE", Comparatori.Uguale, "ril.OPE_CODICE", DataTypes.OutJoinLeft)
                    .AddWhereCondition("VIS_CIT_CODICE_PAESE_VIAGGIO", Comparatori.Uguale, "CIT_CODICE", DataTypes.OutJoinLeft)

                    ' Filtro
                    .AddWhereCondition("VIS_ID", Comparatori.Uguale, idVisita, DataTypes.Numero)

                End With

                Using dr As IDataReader = _DAM.BuildDataReader()

                    If Not dr Is Nothing Then

                        Dim vis_paz_codice As Integer = dr.GetOrdinal("VIS_PAZ_CODICE")
                        Dim vis_id As Integer = dr.GetOrdinal("VIS_ID")
                        Dim vis_peso As Integer = dr.GetOrdinal("VIS_PESO")
                        Dim vis_altezza As Integer = dr.GetOrdinal("VIS_ALTEZZA")
                        Dim vis_cranio As Integer = dr.GetOrdinal("VIS_CRANIO")
                        Dim vis_percentile_peso As Integer = dr.GetOrdinal("VIS_PERCENTILE_PESO")
                        Dim vis_percentile_altezza As Integer = dr.GetOrdinal("VIS_PERCENTILE_ALTEZZA")
                        Dim vis_percentile_cranio As Integer = dr.GetOrdinal("VIS_PERCENTILE_CRANIO")
                        Dim vis_cns_codice As Integer = dr.GetOrdinal("VIS_CNS_CODICE")
                        Dim vis_vaccinabile As Integer = dr.GetOrdinal("VIS_VACCINABILE")
                        Dim vis_fine_sospensione As Integer = dr.GetOrdinal("VIS_FINE_SOSPENSIONE")
                        Dim vis_ute_id As Integer = dr.GetOrdinal("VIS_UTE_ID")
                        Dim vis_note As Integer = dr.GetOrdinal("VIS_NOTE")
                        Dim vis_patologia As Integer = dr.GetOrdinal("VIS_PATOLOGIA")
                        Dim vis_paz_codice_old As Integer = dr.GetOrdinal("VIS_PAZ_CODICE_OLD")
                        Dim vis_mos_codice As Integer = dr.GetOrdinal("VIS_MOS_CODICE")
                        Dim vis_data_variazione As Integer = dr.GetOrdinal("VIS_DATA_VARIAZIONE")
                        Dim vis_ute_id_variazione As Integer = dr.GetOrdinal("VIS_UTE_ID_VARIAZIONE")
                        Dim vis_note_acquisizione As Integer = dr.GetOrdinal("VIS_NOTE_ACQUISIZIONE")
                        Dim vis_n_bilancio As Integer = dr.GetOrdinal("VIS_N_BILANCIO")
                        Dim vis_mal_codice As Integer = dr.GetOrdinal("VIS_MAL_CODICE")
                        Dim mal_descrizione As Integer = dr.GetOrdinal("MAL_DESCRIZIONE")
                        Dim vis_data_visita As Integer = dr.GetOrdinal("VIS_DATA_VISITA")
                        Dim vis_data_registrazione As Integer = dr.GetOrdinal("VIS_DATA_REGISTRAZIONE")
                        Dim vis_firma As Integer = dr.GetOrdinal("VIS_FIRMA")
                        Dim vis_ope_codice As Integer = dr.GetOrdinal("VIS_OPE_CODICE")
                        Dim descrizione_medico As Integer = dr.GetOrdinal("DESCRIZIONE_MEDICO")
                        Dim bil_eta_minima As Integer = dr.GetOrdinal("BIL_ETA_MINIMA")
                        Dim bil_descrizione As Integer = dr.GetOrdinal("BIL_DESCRIZIONE")
                        Dim bil_peso As Integer = dr.GetOrdinal("BIL_PESO")
                        Dim bil_altezza As Integer = dr.GetOrdinal("BIL_ALTEZZA")
                        Dim bil_cranio As Integer = dr.GetOrdinal("BIL_CRANIO")
                        Dim vis_flag_visibilita As Integer = dr.GetOrdinal("VIS_FLAG_VISIBILITA")
                        Dim vis_usl_inserimento As Integer = dr.GetOrdinal("VIS_USL_INSERIMENTO")
                        Dim vis_data_inizio_viaggio As Integer = dr.GetOrdinal("VIS_DATA_INIZIO_VIAGGIO")
                        Dim vis_data_fine_viaggio As Integer = dr.GetOrdinal("VIS_DATA_FINE_VIAGGIO")
                        Dim vis_cit_codice_paese_viaggio As Integer = dr.GetOrdinal("VIS_CIT_CODICE_PAESE_VIAGGIO")
                        Dim cit_stato As Integer = dr.GetOrdinal("CIT_STATO")
                        Dim vis_ope_codice_rilevatore As Integer = dr.GetOrdinal("VIS_OPE_CODICE_RILEVATORE")
                        Dim descrizione_rilevatore As Integer = dr.GetOrdinal("DESCRIZIONE_RILEVATORE")
                        Dim vis_vaccinazioni_bilancio As Integer = dr.GetOrdinal("VIS_VACCINAZIONI_BILANCIO")
                        Dim vis_vis_id_follow_up As Integer = dr.GetOrdinal("VIS_VIS_ID_FOLLOW_UP")
                        Dim data_follow_up_previsto As Integer = dr.GetOrdinal("VIS_DATA_FOLLOW_UP_PREVISTO")
                        Dim data_follow_up_effettivo As Integer = dr.GetOrdinal("VIS_DATA_FOLLOW_UP_EFFETTIVO")

                        If dr.Read() Then
                            visita.IdVisita = dr.GetInt32OrDefault(vis_id)
                            visita.CodicePaziente = dr.GetInt32OrDefault(vis_paz_codice)
                            visita.CodicePazienteAlias = dr.GetNullableInt32OrDefault(vis_paz_codice_old)
                            visita.Peso = dr.GetDecimalOrDefault(vis_peso)
                            visita.Altezza = dr.GetDecimalOrDefault(vis_altezza)
                            visita.Cranio = dr.GetDecimalOrDefault(vis_cranio)
                            visita.PercentilePeso = dr.GetStringOrDefault(vis_percentile_peso)
                            visita.PercentileAltezza = dr.GetStringOrDefault(vis_percentile_altezza)
                            visita.PercentileCranio = dr.GetStringOrDefault(vis_percentile_cranio)
                            visita.CodiceConsultorio = dr.GetStringOrDefault(vis_cns_codice)
                            visita.Vaccinabile = dr.GetStringOrDefault(vis_vaccinabile)
                            visita.DataFineSospensione = dr.GetDateTimeOrDefault(vis_fine_sospensione)
                            visita.IdUtente = dr.GetInt32OrDefault(vis_ute_id)
                            visita.Note = dr.GetStringOrDefault(vis_note)
                            visita.FlagPatologia = dr.GetStringOrDefault(vis_patologia)
                            visita.MotivoSospensioneCodice = dr.GetStringOrDefault(vis_mos_codice)
                            visita.DataModifica = dr.GetNullableDateTimeOrDefault(vis_data_variazione)
                            visita.IdUtenteModifica = dr.GetNullableInt64OrDefault(vis_ute_id_variazione)
                            visita.NoteAcquisizioneDatiVaccinaliCentrale = dr.GetStringOrDefault(vis_note_acquisizione)
                            visita.BilancioNumero = dr.GetNullableInt64OrDefault(vis_n_bilancio)
                            visita.MalattiaCodice = dr.GetStringOrDefault(vis_mal_codice)
                            visita.MalattiaDescrizione = dr.GetStringOrDefault(mal_descrizione)
                            visita.DataVisita = dr.GetDateTimeOrDefault(vis_data_visita)
                            visita.DataRegistrazione = dr.GetDateTimeOrDefault(vis_data_registrazione)
                            visita.Firma = dr.GetStringOrDefault(vis_firma)
                            visita.MedicoCodice = dr.GetStringOrDefault(vis_ope_codice)
                            visita.MedicoDescrizione = dr.GetStringOrDefault(descrizione_medico)
                            visita.FlagVisibilitaDatiVaccinaliCentrale = dr.GetStringOrDefault(vis_flag_visibilita)
                            visita.CodiceUslInserimento = dr.GetStringOrDefault(vis_usl_inserimento)
                            visita.EtaGiorniEsecuzione = dr.GetDecimalOrDefault(bil_eta_minima)
                            visita.BilancioDescrizione = dr.GetStringOrDefault(bil_descrizione)
                            visita.RegistraPeso = dr.GetBooleanOrDefault(bil_peso)
                            visita.RegistraAltezza = dr.GetBooleanOrDefault(bil_altezza)
                            visita.RegistraCranio = dr.GetBooleanOrDefault(bil_cranio)
                            visita.DataInizioViaggio = dr.GetNullableDateTimeOrDefault(vis_data_inizio_viaggio)
                            visita.DataFineViaggio = dr.GetNullableDateTimeOrDefault(vis_data_fine_viaggio)
                            visita.PaeseViaggioCodice = dr.GetStringOrDefault(vis_cit_codice_paese_viaggio)
                            visita.PaeseViaggioDescrizione = dr.GetStringOrDefault(cit_stato)
                            visita.RilevatoreCodice = dr.GetStringOrDefault(vis_ope_codice_rilevatore)
                            visita.RilevatoreDescrizione = dr.GetStringOrDefault(descrizione_rilevatore)
                            visita.VaccinazioniBilancio = dr.GetStringOrDefault(vis_vaccinazioni_bilancio)
                            visita.FollowUpId = dr.GetNullableInt32OrDefault(vis_vis_id_follow_up)
                            visita.DataFollowUpEffettivo = dr.GetNullableDateTimeOrDefault(data_follow_up_effettivo)
                            visita.DataFollowUpPrevisto = dr.GetNullableDateTimeOrDefault(data_follow_up_previsto)
                        End If
                    End If

                End Using

            Catch ex As Exception
                Me.LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return visita

        End Function

        Public Function LoadBilanciMalattie(codicePaziente As Integer, dataConvocazione As Date) As DataTable Implements IBilancioProgrammatoProvider.LoadBilanciMalattie

            RefurbishDT()

            With _DAM.QB

                .NewQuery()

                .AddSelectFields("ID, bip_paz_codice, bip_cnv_data,bip_data_invio, mal_descrizione")
                .AddSelectFields("bip_mal_codice, bip_bil_numero, bip_stato, '' escluso, '' ritardo")

                .AddTables("t_bil_programmati", "t_ana_malattie")

                .AddWhereCondition("bip_mal_codice", Comparatori.Uguale, "mal_codice", DataTypes.Join)
                .AddWhereCondition("bip_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Join)
                .AddWhereCondition("bip_cnv_data", Comparatori.Uguale, dataConvocazione, DataTypes.Data)
                .AddWhereCondition("bip_stato", Comparatori.Uguale, Constants.StatiBilancio.UNEXECUTED, DataTypes.Stringa)

                .AddOrderByFields("bip_mal_codice, bip_bil_numero")

            End With

            _DAM.BuildDataTable(_DT)

            Return _DT.Copy

        End Function

        ' Caricamento dei bilanci dall'anagrafica
        Public Function GetDtBilanci(codiceMalattia As String) As DataTable Implements IBilancioProgrammatoProvider.GetDtBilanci

            Dim dtBilanci As New DataTable()

            With _DAM.QB

                .NewQuery()

                .AddSelectFields("BIL_NUMERO", .FC.Concat("BIL_NUMERO", "' - '", "BIL_DESCRIZIONE") & " BIL_DESCRIZIONE")
                .AddTables("T_ANA_BILANCI")

                If Not String.IsNullOrEmpty(codiceMalattia) Then
                    .AddWhereCondition("BIL_MAL_CODICE", Comparatori.Uguale, codiceMalattia, DataTypes.Stringa)
                End If

                .AddOrderByFields("BIL_NUMERO")

            End With

            _DAM.BuildDataTable(dtBilanci)

            Return dtBilanci

        End Function

#Region " Datatable visite-bilanci "

        Public Function GetDtVisiteBilanciById(listIdVisite As List(Of Int64)) As DataTable Implements IBilancioProgrammatoProvider.GetDtVisiteBilanciById

            Return Me.GetDtVisiteBilanci(Nothing, listIdVisite)

        End Function

        Public Function GetDtVisiteBilanci(codicePaziente As Integer) As DataTable Implements IBilancioProgrammatoProvider.GetDtVisiteBilanci

            Return Me.GetDtVisiteBilanci(codicePaziente.ToString(), Nothing)

        End Function

        Private Function GetDtVisiteBilanci(codicePaziente As String, listIdVisite As List(Of Int64)) As DataTable

            Dim dtElencoBilanci As New DataTable()

            Try
                With _DAM.QB

                    .NewQuery()

                    .IsDistinct = True

                    .AddSelectFields("VIS_N_BILANCIO, VIS_MAL_CODICE, VIS_DATA_VISITA, VIS_DATA_REGISTRAZIONE, VIS_ID")
                    .AddSelectFields("VIS_PAZ_CODICE, VIS_OPE_CODICE, med.OPE_NOME OPE_NOME, VIS_FIRMA, UTE_DESCRIZIONE")
                    .AddSelectFields("BIL_ETA_MINIMA, BIL_DESCRIZIONE, MAL_DESCRIZIONE")
                    .AddSelectFields("VIS_FLAG_VISIBILITA, VIS_USL_INSERIMENTO, USL_DESCRIZIONE USL_INSERIMENTO_VIS_DESCR")
                    .AddSelectFields("VIS_DOC_ID_DOCUMENTO, VIS_DATA_FIRMA, VIS_UTE_ID_FIRMA, VIS_DATA_ARCHIVIAZIONE, VIS_UTE_ID_ARCHIVIAZIONE")
                    .AddSelectFields("VIS_DATA_INIZIO_VIAGGIO, VIS_DATA_FINE_VIAGGIO, VIS_CIT_CODICE_PAESE_VIAGGIO, CIT_STATO")
                    .AddSelectFields("VIS_OPE_CODICE_RILEVATORE, ril.OPE_NOME DESCRIZIONE_RILEVATORE, VIS_VACCINAZIONI_BILANCIO, VIS_NOTE")
                    .AddSelectFields("MAL_SOLO_FOLLOW_UP, MAL_MAL_CODICE_FOLLOW_UP")
                    .AddSelectFields("VIS_VIS_ID_FOLLOW_UP, VIS_DATA_FOLLOW_UP_PREVISTO,VIS_DATA_FOLLOW_UP_EFFETTIVO")

                    .AddTables("T_ANA_BILANCI, T_ANA_MALATTIE, T_VIS_VISITE, T_ANA_OPERATORI med, T_ANA_UTENTI, T_ANA_USL")
                    .AddTables("T_ANA_OPERATORI ril, T_ANA_CITTADINANZE")

                    If Not String.IsNullOrEmpty(codicePaziente) Then
                        .AddWhereCondition("VIS_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                    End If

                    If Not listIdVisite Is Nothing AndAlso listIdVisite.Count > 0 Then
                        .AddWhereCondition("VIS_ID", Comparatori.In, String.Join(",", listIdVisite.ToArray()), DataTypes.Replace)
                    End If

                    .AddWhereCondition("VIS_N_BILANCIO", Comparatori.Uguale, "BIL_NUMERO", DataTypes.Join)
                    .AddWhereCondition("VIS_MAL_CODICE", Comparatori.Uguale, "BIL_MAL_CODICE", DataTypes.Join)
                    .AddWhereCondition("VIS_MAL_CODICE", Comparatori.Uguale, "MAL_CODICE", DataTypes.Join)
                    .AddWhereCondition("VIS_OPE_CODICE", Comparatori.Uguale, "med.OPE_CODICE", DataTypes.OutJoinLeft)
                    .AddWhereCondition("VIS_UTE_ID", Comparatori.Uguale, "UTE_ID", DataTypes.OutJoinLeft)
                    .AddWhereCondition("VIS_USL_INSERIMENTO", Comparatori.Uguale, "USL_CODICE", DataTypes.OutJoinLeft)

                    .AddWhereCondition("VIS_OPE_CODICE_RILEVATORE", Comparatori.Uguale, "ril.OPE_CODICE", DataTypes.OutJoinLeft)
                    .AddWhereCondition("VIS_CIT_CODICE_PAESE_VIAGGIO", Comparatori.Uguale, "CIT_CODICE", DataTypes.OutJoinLeft)

                End With

                _DAM.BuildDataTable(dtElencoBilanci)

                dtElencoBilanci.DefaultView.Sort = "VIS_DATA_VISITA DESC"

            Catch ex As Exception
                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return dtElencoBilanci

        End Function

#End Region

#Region " Datatable risposte-bilancio "

        Public Function GetRisposteBilancio(idVisita As Integer) As DataTable Implements IBilancioProgrammatoProvider.GetRisposteBilancio

            Dim dtRisposte As New DataTable()

            Me.SetQueryRisposteBilancio(_DAM.QB)

            ' Filtro id visita
            _DAM.QB.AddWhereCondition("VIS_ID", Comparatori.Uguale, idVisita, DataTypes.Numero)

            _DAM.BuildDataTable(dtRisposte)

            Return dtRisposte

        End Function

        Public Function GetRisposteBilancio(numeroBilancio As Integer, codiceMalattia As String, codicePaziente As Integer, dataVisita As Date) As DataTable Implements IBilancioProgrammatoProvider.GetRisposteBilancio

            Dim dtRisposte As New DataTable()

            Me.SetQueryRisposteBilancio(_DAM.QB)

            ' Filtri
            With _DAM.QB
                .AddWhereCondition("VIS_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("VIS_N_BILANCIO", Comparatori.Uguale, numeroBilancio, DataTypes.Numero)
                .AddWhereCondition("VIS_MAL_CODICE", Comparatori.Uguale, codiceMalattia, DataTypes.Stringa)
                .AddWhereCondition("VIS_DATA_VISITA", Comparatori.Uguale, dataVisita, DataTypes.Data)
            End With

            _DAM.BuildDataTable(dtRisposte)

            Return dtRisposte

        End Function

        Private Sub SetQueryRisposteBilancio(ByRef qb As AbstractQB)

            With qb

                .NewQuery()

                ' Select
                .AddSelectFields("VOS_RISPOSTA, VIS_OPE_CODICE, VIS_DATA_VISITA")
                .AddSelectFields(.FC.IsNull("UTE_DESCRIZIONE", "On.Ped", DataTypes.Stringa) + " UTE_DESCRIZIONE")
                .AddSelectFields("VIS_UTE_ID, VIS_DATA_REGISTRAZIONE, OSS_DESCRIZIONE")
                .AddSelectFields(.FC.IsNull("OSS_DESCRIZIONE", "-", DataTypes.Stringa) + " OSS_DESCRIZIONE")
                .AddSelectFields("OSB_SEZ_CODICE, VOS_OSS_CODICE, RIS_DESCRIZIONE, VOS_RIS_CODICE")
                .AddSelectFields("VOS_ID")

                ' From
                .AddTables("T_VIS_OSSERVAZIONI, T_ANA_OSSERVAZIONI, T_ANA_RISPOSTE, T_ANA_UTENTI, T_VIS_VISITE, T_ANA_LINK_OSS_BILANCI")

                ' Join
                .AddWhereCondition("UTE_ID", Comparatori.Uguale, "VIS_UTE_ID", DataTypes.OutJoinRight)
                .AddWhereCondition("OSS_CODICE", Comparatori.Uguale, "VOS_OSS_CODICE", DataTypes.Join)
                .AddWhereCondition("RIS_CODICE", Comparatori.Uguale, "VOS_RIS_CODICE", DataTypes.OutJoinRight)
                .AddWhereCondition("OSS_CODICE", Comparatori.Uguale, "OSB_OSS_CODICE", DataTypes.Join)
                .AddWhereCondition("OSB_BIL_N_BILANCIO", Comparatori.Uguale, "VOS_BIL_N_BILANCIO", DataTypes.Join)
                .AddWhereCondition("OSB_BIL_MAL_CODICE", Comparatori.Uguale, "VIS_MAL_CODICE", DataTypes.Join)
                .AddWhereCondition("VIS_ID", Comparatori.Uguale, "VOS_VIS_ID", DataTypes.Join)

            End With

        End Sub

#End Region

#End Region

#Region " Metodi di Delete "

        Public Function DeleteRecord(key As Object) As Boolean Implements IBilancioProgrammatoProvider.DeleteRecord
        End Function

        Public Function DeleteRecord(codicePaziente As Integer, codiceMalattia As String) As Boolean Implements IBilancioProgrammatoProvider.DeleteRecord

            Dim count As Integer = 0

            With _DAM.QB
                .NewQuery()
                .AddTables("T_BIL_PROGRAMMATI")
                .AddWhereCondition("bip_mal_codice", Comparatori.Uguale, codiceMalattia, DataTypes.Stringa)
                .AddWhereCondition("bip_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
            End With

            Try
                count = _DAM.ExecNonQuery(ExecQueryType.Delete)
            Catch ex As Exception
                If (_DAM.ExistTra) Then _DAM.Rollback()
                Me.LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return (count > 0)

        End Function

        Public Function DeleteRecord(codicePaziente As Integer, numeroBilancio As Integer, codiceMalattia As String) As Boolean Implements IBilancioProgrammatoProvider.DeleteRecord

            Dim count As Integer = 0

            With _DAM.QB
                .NewQuery()
                .AddTables("T_BIL_PROGRAMMATI")
                .AddWhereCondition("bip_bil_numero", Comparatori.Uguale, numeroBilancio, DataTypes.Numero)
                .AddWhereCondition("bip_mal_codice", Comparatori.Uguale, codiceMalattia, DataTypes.Stringa)
                .AddWhereCondition("bip_paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
            End With

            Try
                count = _DAM.ExecNonQuery(ExecQueryType.Delete)

            Catch ex As Exception
                If _DAM.ExistTra Then _DAM.Rollback()
                Me.LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return (count > 0)

        End Function

#End Region

#Region " Metodi di Update/Insert "

        Public Function EditRecord(con As Object) As Boolean Implements IBilancioProgrammatoProvider.EditRecord
        End Function

        Public Function Update(bilancio As BilancioProgrammato) As Boolean Implements IBilancioProgrammatoProvider.Update

            Dim count As Integer = 0

            With _DAM.QB
                .NewQuery()
                .AddUpdateField("bip_paz_codice", bilancio.Paz_Codice, DataTypes.Numero)
                .AddUpdateField("bip_bil_numero", bilancio.N_bilancio, DataTypes.Numero)
                .AddUpdateField("bip_mal_codice", bilancio.Mal_codice, DataTypes.Stringa)
                .AddUpdateField("bip_cnv_data", "to_date('" & bilancio.Data_CNV.ToString("d") & "','dd/mm/yyyy')", DataTypes.Replace)
                .AddUpdateField("bip_stato", bilancio.Bil_stato, DataTypes.Stringa)
                .AddTables("T_BIL_PROGRAMMATI ")
                .AddWhereCondition("id", Comparatori.Uguale, bilancio.Bil_id, DataTypes.Numero)
            End With

            Try
                count = _DAM.ExecNonQuery(ExecQueryType.Update)
            Catch ex As Exception
                If _DAM.ExistTra Then _DAM.Rollback()
                Me.LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return (count > 0)

        End Function

        Public Function UpdateDataInvio(id As Integer, dataInvio As Date) As Boolean Implements IBilancioProgrammatoProvider.UpdateDataInvio

            Dim count As Integer = 0

            With _DAM.QB
                .NewQuery()
                .AddTables("T_BIL_PROGRAMMATI")

                If (dataInvio = DateTime.MinValue) Then
                    .AddUpdateField("bip_data_invio", DBNull.Value, DataTypes.Data)
                Else
                    .AddUpdateField("bip_data_invio", dataInvio, DataTypes.Data)
                End If

                .AddWhereCondition("id", Comparatori.Uguale, id, DataTypes.Numero)
            End With

            Try
                count = _DAM.ExecNonQuery(ExecQueryType.Update)
            Catch ex As Exception
                If _DAM.ExistTra Then _DAM.Rollback()
                Me.LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return (count > 0)

        End Function

        Public Function NewRecord(bil As BilancioProgrammato) As Boolean Implements IBilancioProgrammatoProvider.NewRecord

            With _DAM.QB
                .NewQuery()
                .AddTables("t_bil_programmati")
                .AddInsertField("bip_bil_numero", bil.N_bilancio, DataTypes.Replace)

                ' ORACLE NON PERMETTE L'INSERIMENTO DELLA DATA MINIMA
                If (bil.Data_CNV = Date.MinValue) Then
                    bil.Data_CNV = Date.MinValue.AddYears(1901)
                End If

                .AddInsertField("bip_cnv_data", bil.Data_CNV, DataTypes.Data)
                .AddInsertField("bip_mal_codice", bil.Mal_codice, DataTypes.Stringa)
                .AddInsertField("bip_paz_codice", bil.Paz_Codice, DataTypes.Numero)
                .AddInsertField("bip_stato", bil.Bil_stato, DataTypes.Stringa)
            End With

            Try
                _DAM.ExecNonQuery(ExecQueryType.Insert)
            Catch ex As Exception
                If _DAM.ExistTra Then _DAM.Rollback()
                Me.LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return True

        End Function

#End Region

    End Class

End Namespace
