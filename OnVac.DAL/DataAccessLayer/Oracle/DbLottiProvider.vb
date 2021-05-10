Imports System.Collections.Generic
Imports System.Data.OracleClient
Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.Data


Namespace DAL

    Public Class DbLottiProvider
        Inherits DbProvider
        Implements ILottiProvider

#Region " Costruttori "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " Lotti "

#Region " Public "

        ''' <summary>
        ''' Restituisce un datatable contenente i lotti utilizzabili per le vaccinazioni da eseguire
        ''' </summary>
        ''' <param name="codiceConsultorio"></param>
        ''' <param name="codiceConsultorioMagazzino"></param>
        ''' <param name="gestioneTipoConsultorio"></param>
        ''' <param name="sessoPaziente"></param>
        ''' <param name="etaPaziente"></param>
        ''' <param name="dataScadenzaLotto"></param>
        ''' <param name="includiLottiFuoriEta"></param>
        ''' <param name="soloLottiAttivi"></param>
        ''' <param name="filtraEtaAttivazione">Se true, sono considerati lotti attivi solo quelli per i quali l'età del paziente rientra nell'intervallo di attivazione</param>
        ''' <param name="includiImportiDefault"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function LoadLottiUtilizzabili(codiceConsultorio As String, codiceConsultorioMagazzino As String, gestioneTipoConsultorio As Boolean, sessoPaziente As String, etaPaziente As Double, dataScadenzaLotto As Date, includiLottiFuoriEta As Boolean, soloLottiAttivi As Boolean, filtraEtaAttivazione As Boolean, includiImportiDefault As Boolean) As DataTable Implements ILottiProvider.LoadLottiUtilizzabili
            '--
            Dim dtLottiVac As New DataTable()
            '--
            With _DAM.QB
                '--
                ' Query per determinare il valore del flag Lotto Attivo
                .NewQuery()
                '--
                .AddSelectFields("lcn_attivo")
                .AddTables("t_lot_lotti_consultori att")
                .AddWhereCondition("att.lcn_lot_codice", Comparatori.Uguale, "t_lot_lotti_consultori.lcn_lot_codice", DataTypes.Replace)
                .AddWhereCondition("att.lcn_cns_codice", Comparatori.Uguale, codiceConsultorio, DataTypes.Stringa)
                '--
                If filtraEtaAttivazione Then
                    '--
                    .OpenParanthesis()
                    .AddWhereCondition("att.lcn_eta_min_attivazione", Comparatori.Is, "NULL", DataTypes.Replace)
                    .AddWhereCondition("att.lcn_eta_min_attivazione", Comparatori.MinoreUguale, etaPaziente, DataTypes.Double, "OR")
                    .CloseParanthesis()
                    .OpenParanthesis()
                    .AddWhereCondition("att.lcn_eta_max_attivazione", Comparatori.Is, "NULL", DataTypes.Replace)
                    .AddWhereCondition("att.lcn_eta_max_attivazione", Comparatori.MaggioreUguale, etaPaziente, DataTypes.Double, "OR")
                    .CloseParanthesis()
                    '--
                End If
                '--
                Dim queryLottoAttivo As String = .GetSelect()
                '--
                ' Query per ottenere l'id della configurazione da utilizzare per i dati di pagamento
                Dim queryAutoSetImporto As String = "null"
                '--
                If includiImportiDefault Then
                    '--
                    .NewQuery(False, True)
                    .AddSelectFields("max(cpg_id)")
                    .AddTables("t_noc_condizioni_pagamento")
                    .AddWhereCondition("cpg_noc_codice", Comparatori.Uguale, "noc_codice", DataTypes.Replace)
                    .AddWhereCondition("cpg_da_eta", Comparatori.MinoreUguale, etaPaziente, DataTypes.Double)
                    .OpenParanthesis()
                    .AddWhereCondition("cpg_a_eta", Comparatori.Is, "NULL", DataTypes.Replace)
                    .AddWhereCondition("cpg_a_eta", Comparatori.MaggioreUguale, etaPaziente, DataTypes.Double, "OR")
                    .CloseParanthesis()
                    '--
                    queryAutoSetImporto = "(" + .GetSelect() + ")"
                    '--
                End If
                '--
                ' QUERY INTERNA
                .NewQuery(False, True)
                '--
                .IsDistinct = True
                '--
                .AddTables("t_ana_lotti, t_lot_lotti_consultori, t_ana_nomi_commerciali, t_ana_tipi_pagamento")
                .AddTables("t_ana_link_noc_associazioni, t_ana_link_ass_vaccinazioni")
                '--
                If gestioneTipoConsultorio Then
                    .AddTables("t_ana_associazioni_tipi_cns, t_ana_consultori")
                End If
                '--
                .AddSelectFields("lot_codice, lot_descrizione, lot_data_scadenza, lot_noc_codice, t_lot_lotti_consultori.lcn_dosi_rimaste lcn_dosi_rimaste, noc_descrizione")
                .AddSelectFields("noc_eta_inizio, noc_eta_fine, val_ass_codice, val_vac_codice")
                .AddSelectFields("noc_tpa_guid_tipi_pagamento, tpa_flag_importo, tpa_flag_esenzione")           ' tpa_descrizione
                .AddSelectFields("t_lot_lotti_consultori.lcn_cns_codice lcn_cns_codice")
                .AddSelectFields("t_lot_lotti_consultori.lcn_eta_min_attivazione lcn_eta_min_attivazione")
                .AddSelectFields("t_lot_lotti_consultori.lcn_eta_max_attivazione lcn_eta_max_attivazione")
                '--
                If includiImportiDefault Then
                    .AddSelectFields(queryAutoSetImporto + " cpg_id, tpa_auto_set_importo, noc_costo_unitario")
                Else
                    .AddSelectFields("null cpg_id, null tpa_auto_set_importo, null noc_costo_unitario")
                End If
                '--
                .AddSelectFields(.FC.IsNull("(" + queryLottoAttivo + ")", "N", DataTypes.Stringa) + " lcn_attivo")
                '--
                .AddWhereCondition("lot_codice", Comparatori.Uguale, "t_lot_lotti_consultori.lcn_lot_codice", DataTypes.Join)
                '--
                .AddWhereCondition("t_lot_lotti_consultori.lcn_dosi_rimaste", Comparatori.Maggiore, 0, DataTypes.Numero)
                .AddWhereCondition("t_lot_lotti_consultori.lcn_cns_codice", Comparatori.Uguale, codiceConsultorioMagazzino, DataTypes.Stringa)
                '--
                .AddWhereCondition("lot_noc_codice", Comparatori.Uguale, "noc_codice", DataTypes.Join)
                .AddWhereCondition("noc_codice", Comparatori.Uguale, "nal_noc_codice", DataTypes.Join)
                .AddWhereCondition("nal_ass_codice", Comparatori.Uguale, "val_ass_codice", DataTypes.Join)
                .AddWhereCondition("noc_tpa_guid_tipi_pagamento", Comparatori.Uguale, "tpa_guid", DataTypes.OutJoinLeft)
                '--
                If gestioneTipoConsultorio Then
                    '--
                    .AddWhereCondition("val_ass_codice", Comparatori.Uguale, "atc_ass_codice", DataTypes.Join)
                    .AddWhereCondition("atc_cns_tipo", Comparatori.Uguale, "cns_tipo", DataTypes.Join)
                    .AddWhereCondition("cns_codice", Comparatori.Uguale, codiceConsultorio, DataTypes.Stringa)
                    '--
                End If
                '--
                .OpenParanthesis()
                .AddWhereCondition("noc_sesso", Comparatori.Uguale, sessoPaziente, DataTypes.Stringa)
                .AddWhereCondition("noc_sesso", Comparatori.Uguale, "E", DataTypes.Stringa, "OR")
                .CloseParanthesis()
                '--
                .AddWhereCondition("lot_data_scadenza", Comparatori.MaggioreUguale, dataScadenzaLotto, DataTypes.Data)
                .AddWhereCondition("lot_obsoleto", Comparatori.Uguale, "N", DataTypes.Stringa)
                '--
                If Not includiLottiFuoriEta Then
                    '--
                    .AddWhereCondition("noc_eta_inizio", Comparatori.MinoreUguale, etaPaziente, DataTypes.Double)
                    .AddWhereCondition("noc_eta_fine", Comparatori.MaggioreUguale, etaPaziente, DataTypes.Double)
                    '--
                End If
                '--
                If soloLottiAttivi Then
                    '--
                    Dim filtroQueryEtaAttivazione As String = String.Empty
                    '--
                    If filtraEtaAttivazione Then
                        filtroQueryEtaAttivazione += String.Format(" AND (t_lot_lotti_consultori_2.lcn_eta_min_attivazione is null or t_lot_lotti_consultori_2.lcn_eta_min_attivazione <= {0}) ", _DAM.QB.AddCustomParam(etaPaziente))
                        filtroQueryEtaAttivazione += String.Format(" AND (t_lot_lotti_consultori_2.lcn_eta_max_attivazione is null or t_lot_lotti_consultori_2.lcn_eta_max_attivazione >= {0}) ", _DAM.QB.AddCustomParam(etaPaziente))
                    End If
                    '--
                    .AddWhereCondition("", Comparatori.Exist, String.Format("(select 1 from t_lot_lotti_consultori t_lot_lotti_consultori_2 where t_lot_lotti_consultori_2.lcn_lot_codice = t_lot_lotti_consultori.lcn_lot_codice and t_lot_lotti_consultori_2.lcn_cns_codice = {0} and t_lot_lotti_consultori_2.lcn_attivo = {1}{2})", _DAM.QB.AddCustomParam(codiceConsultorio), _DAM.QB.AddCustomParam("S"), filtroQueryEtaAttivazione), DataTypes.Replace)
                    '--
                End If
                '--
                Dim querySelect As String = .GetSelect()
                '--
                ' QUERY ESTERNA
                .NewQuery(False, True)
                .AddSelectFields("a.*, cpg.cpg_flag_importo, cpg.cpg_flag_esenzione, cpg.cpg_auto_set_importo")
                .AddTables("(" + querySelect + ") a ")
                .AddTables("t_noc_condizioni_pagamento cpg")
                .AddWhereCondition("a.cpg_id", Comparatori.Uguale, "cpg.cpg_id", DataTypes.OutJoinLeft)
                '--
                .AddOrderByFields("lot_codice")
                '--
            End With
            '--
            _DAM.BuildDataTable(dtLottiVac)
            '--
            Return dtLottiVac
            '--
        End Function

        ''' <summary>
        ''' Caricamento lotto del consultorio di magazzino specificato, in base ai filtri impostati
        ''' </summary>
        Public Function LoadLottiMagazzino(codiceConsultorio As String, codiceConsultorioMagazzino As String, codiceLotto As String, filtriRicerca As Filters.FiltriRicercaLottiMagazzino) As List(Of Entities.LottoMagazzino) Implements ILottiProvider.LoadLottiMagazzino

            With _DAM.QB

                .EmptyStringIsNULL = True

                .NewQuery()
                .AddTables("t_lot_lotti_consultori b")
                .AddSelectFields("lcn_attivo")
                .AddWhereCondition("a.lcn_lot_codice", Comparatori.Uguale, "b.lcn_lot_codice", DataTypes.Join)
                .AddWhereCondition("b.lcn_cns_codice", Comparatori.Uguale, codiceConsultorio, DataTypes.Stringa)

                Dim attivoCnsCorrente As String = .GetSelect()

                .NewQuery(False, False)

                .AddTables("t_ana_lotti, t_lot_lotti_consultori a, t_ana_nomi_commerciali, t_ana_fornitori")

                .AddSelectFields("lot_codice, lot_descrizione, lot_data_preparazione, lot_data_scadenza, lot_ditta")
                .AddSelectFields("lot_dosi_scatola, lot_note, lot_obsoleto, lot_noc_codice, lcn_lot_codice")
                .AddSelectFields("lcn_cns_codice, lcn_dosi_rimaste, lcn_qta_minima")
                .AddSelectFields("(" + attivoCnsCorrente + ") lcn_attivo, noc_codice")
                .AddSelectFields("noc_descrizione || decode(for_descrizione, null, '', ' (' || for_descrizione || ')') noc_descrizione")
                .AddSelectFields("noc_sesso, noc_eta_inizio, noc_eta_fine, noc_ass_codice, noc_for_codice, for_descrizione")
                .AddSelectFields("lcn_ute_id_modifica_attivo, lcn_data_modifica_attivo")
                .AddSelectFields("lot_ute_id_modifica_obsoleto, lot_data_modifica_obsoleto")
                .AddSelectFields("lcn_eta_min_attivazione, lcn_eta_max_attivazione")

                .AddWhereCondition("noc_for_codice", Comparatori.Uguale, "for_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("a.lcn_lot_codice", Comparatori.Uguale, "lot_codice", DataTypes.OutJoinRight)
                .AddWhereCondition("noc_codice", Comparatori.Uguale, "lot_noc_codice", DataTypes.OutJoinRight)
                .AddWhereCondition("a.lcn_cns_codice", Comparatori.Uguale, .AddCustomParam(codiceConsultorioMagazzino), DataTypes.OutJoinRight)

                If Not filtriRicerca Is Nothing Then

                    Me.AddFiltriRicercaMagazzino(filtriRicerca, _DAM.QB, False)

                Else

                    .AddWhereCondition("lot_codice", Comparatori.Uguale, codiceLotto, DataTypes.Stringa)

                End If

            End With

            Return GetListLottiMagazzino(_DAM, False)

        End Function

        Public Function GetLotti(idRSA As String, escludiLottiGiacenzaZero As Boolean) As List(Of Lotto) Implements ILottiProvider.GetLotti

            Dim result As New List(Of Lotto)()

            Dim ownConnection As Boolean = False

            Dim query As String =
                "SELECT LOT_CODICE, LOT_DESCRIZIONE, LCN_DOSI_RIMASTE, LCN_ATTIVO, LOT_DATA_SCADENZA, " +
                "NOC_DESCRIZIONE, NOC_CODICE_AIC, " +
                "ASS_CODICE, ASS_DESCRIZIONE, ASS_ANTI_INFLUENZALE, ASS_ANTI_PNEUMOCOCCO, ASS_SII_CODICE, NOC_SII_CODICE, ASS_VII_CODICE, NOC_VII_CODICE " +
                "FROM V_ANA_RSA " +
                "JOIN T_LOT_LOTTI_CONSULTORI ON RSA_CNS_CODICE_MAGAZZINO = LCN_CNS_CODICE " +
                "JOIN T_ANA_LOTTI ON LCN_LOT_CODICE = LOT_CODICE " +
                "JOIN T_ANA_NOMI_COMMERCIALI ON LOT_NOC_CODICE = NOC_CODICE " +
                "JOIN T_ANA_LINK_NOC_ASSOCIAZIONI ON LOT_NOC_CODICE = NAL_NOC_CODICE " +
                "JOIN T_ANA_ASSOCIAZIONI ON NAL_ASS_CODICE = ASS_CODICE " +
                "WHERE RSA_ID = :RSA_ID " +
                "AND (LOT_OBSOLETO Is NULL or LOT_OBSOLETO = 'N') " +
                "AND (LOT_DATA_SCADENZA IS NULL OR LOT_DATA_SCADENZA >= SYSDATE) " +
                "AND (ASS_ANTI_INFLUENZALE = 'S' OR ASS_ANTI_PNEUMOCOCCO = 'S') " +
                "AND ASS_OBSOLETO = 'N' "

            If escludiLottiGiacenzaZero Then
                query += "AND LCN_DOSI_RIMASTE > 0 "
            End If

            Try
                Using cmd As New OracleCommand(query, Connection)

                    cmd.Parameters.AddWithValue("RSA_ID", idRSA)

                    ownConnection = ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()

                        Dim LOT_CODICE As Integer = _context.GetOrdinal("LOT_CODICE")
                        Dim LOT_DESCRIZIONE As Integer = _context.GetOrdinal("LOT_DESCRIZIONE")
                        Dim LCN_DOSI_RIMASTE As Integer = _context.GetOrdinal("LCN_DOSI_RIMASTE")
                        Dim LCN_ATTIVO As Integer = _context.GetOrdinal("LCN_ATTIVO")
                        Dim LOT_DATA_SCADENZA As Integer = _context.GetOrdinal("LOT_DATA_SCADENZA")
                        Dim NOC_DESCRIZIONE As Integer = _context.GetOrdinal("NOC_DESCRIZIONE")
                        Dim NOC_CODICE_AIC As Integer = _context.GetOrdinal("NOC_CODICE_AIC")
                        Dim ASS_CODICE As Integer = _context.GetOrdinal("ASS_CODICE")
                        Dim ASS_DESCRIZIONE As Integer = _context.GetOrdinal("ASS_DESCRIZIONE")
                        Dim ASS_ANTI_INFLUENZALE As Integer = _context.GetOrdinal("ASS_ANTI_INFLUENZALE")
                        Dim ASS_ANTI_PNEUMOCOCCO As Integer = _context.GetOrdinal("ASS_ANTI_PNEUMOCOCCO")
                        Dim ASS_SII_CODICE_DEFAULT As Integer = _context.GetOrdinal("ASS_SII_CODICE")
                        Dim NOC_SII_CODICE_DEFAULT As Integer = _context.GetOrdinal("NOC_SII_CODICE")
                        Dim ASS_VII_CODICE_DEFAULT As Integer = _context.GetOrdinal("ASS_VII_CODICE")
                        Dim NOC_VII_CODICE_DEFAULT As Integer = _context.GetOrdinal("NOC_VII_CODICE")

                        While _context.Read()

                            Dim lotto As New Lotto()

                            lotto.Codice = _context.GetStringOrDefault(LOT_CODICE)
                            lotto.Descrizione = _context.GetStringOrDefault(LOT_DESCRIZIONE)
                            lotto.DosiRimaste = _context.GetInt32OrDefault(LCN_DOSI_RIMASTE)
                            lotto.Attivo = _context.GetStringOrDefault(LCN_ATTIVO)
                            lotto.Scadenza = _context.GetDateTimeOrDefault(LOT_DATA_SCADENZA)
                            lotto.NomeFarmaco = _context.GetStringOrDefault(NOC_DESCRIZIONE)
                            lotto.AicFarmaco = _context.GetStringOrDefault(NOC_CODICE_AIC)
                            lotto.CodiceAssociazione = _context.GetStringOrDefault(ASS_CODICE)
                            lotto.DescrizioneAssociazione = _context.GetStringOrDefault(ASS_DESCRIZIONE)
                            lotto.AssAntiInfluenzale = _context.GetStringOrDefault(ASS_ANTI_INFLUENZALE)
                            lotto.AssAntiPneumococco = _context.GetStringOrDefault(ASS_ANTI_PNEUMOCOCCO)
                            lotto.CodiceSitoDefaultAssociazione = _context.GetStringOrDefault(ASS_SII_CODICE_DEFAULT)
                            lotto.CodiceSitoDefaultFarmaco = _context.GetStringOrDefault(NOC_SII_CODICE_DEFAULT)
                            lotto.CodiceViaDefaultAssociazione = _context.GetStringOrDefault(ASS_VII_CODICE_DEFAULT)
                            lotto.CodiceViaDefaultFarmaco = _context.GetStringOrDefault(NOC_VII_CODICE_DEFAULT)

                            result.Add(lotto)

                        End While
                    End Using

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return result

        End Function

        ''' <summary>
        ''' Restituisce l'anagrafica del lotto specificato
        ''' </summary>
        ''' <param name="codiceLotto"></param>
        ''' <returns></returns>
        Public Function GetLottoAnagrafe(codiceLotto As String) As LottoAnagrafe Implements ILottiProvider.GetLottoAnagrafe

            Dim lotto As New LottoAnagrafe()

            Dim ownConnection As Boolean = False

            Dim query As String =
                "SELECT LOT_CODICE, LOT_DESCRIZIONE, LOT_DATA_PREPARAZIONE, LOT_DATA_SCADENZA, LOT_DITTA, " +
                "LOT_DOSI_SCATOLA, LOT_NOTE, LOT_OBSOLETO, LOT_NOC_CODICE, LOT_UTE_ID_MODIFICA_OBSOLETO, " +
                "LOT_DATA_MODIFICA_OBSOLETO, LOT_QTA_MINIMA_COMPLESSIVA, LOT_USL_CODICE_INSERIMENTO " +
                "FROM T_ANA_LOTTI " +
                "WHERE LOT_CODICE = :LOT_CODICE "

            Try
                Using cmd As New OracleCommand(query, Connection)

                    cmd.Parameters.AddWithValue("LOT_CODICE", codiceLotto)

                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        Dim LOT_CODICE As Integer = idr.GetOrdinal("LOT_CODICE")
                        Dim LOT_DESCRIZIONE As Integer = idr.GetOrdinal("LOT_DESCRIZIONE")
                        Dim LOT_DATA_PREPARAZIONE As Integer = idr.GetOrdinal("LOT_DATA_PREPARAZIONE")
                        Dim LOT_DATA_SCADENZA As Integer = idr.GetOrdinal("LOT_DATA_SCADENZA")
                        Dim LOT_DITTA As Integer = idr.GetOrdinal("LOT_DITTA")
                        Dim LOT_DOSI_SCATOLA As Integer = idr.GetOrdinal("LOT_DOSI_SCATOLA")
                        Dim LOT_NOTE As Integer = idr.GetOrdinal("LOT_NOTE")
                        Dim LOT_OBSOLETO As Integer = idr.GetOrdinal("LOT_OBSOLETO")
                        Dim LOT_NOC_CODICE As Integer = idr.GetOrdinal("LOT_NOC_CODICE")
                        Dim LOT_UTE_ID_MODIFICA_OBSOLETO As Integer = idr.GetOrdinal("LOT_UTE_ID_MODIFICA_OBSOLETO")
                        Dim LOT_DATA_MODIFICA_OBSOLETO As Integer = idr.GetOrdinal("LOT_DATA_MODIFICA_OBSOLETO")
                        Dim LOT_QTA_MINIMA_COMPLESSIVA As Integer = idr.GetOrdinal("LOT_QTA_MINIMA_COMPLESSIVA")
                        Dim LOT_USL_CODICE_INSERIMENTO As Integer = idr.GetOrdinal("LOT_USL_CODICE_INSERIMENTO")

                        If idr.Read() Then

                            lotto.Codice = idr.GetStringOrDefault(LOT_CODICE)
                            lotto.Descrizione = idr.GetStringOrDefault(LOT_DESCRIZIONE)
                            lotto.DataPreparazione = idr.GetNullableDateTimeOrDefault(LOT_DATA_PREPARAZIONE)
                            lotto.DataScadenza = idr.GetDateTimeOrDefault(LOT_DATA_SCADENZA)
                            lotto.Ditta = idr.GetStringOrDefault(LOT_DITTA)
                            lotto.DosiScatola = idr.GetNullableInt32OrDefault(LOT_DOSI_SCATOLA)
                            lotto.Note = idr.GetStringOrDefault(LOT_NOTE)
                            lotto.Obsoleto = idr.GetBooleanOrDefault(LOT_OBSOLETO)
                            lotto.CodiceNomeCommerciale = idr.GetStringOrDefault(LOT_NOC_CODICE)
                            lotto.IdUtenteModificaObsoleto = idr.GetNullableInt64OrDefault(LOT_UTE_ID_MODIFICA_OBSOLETO)
                            lotto.DataModificaObsoleto = idr.GetNullableDateTimeOrDefault(LOT_DATA_MODIFICA_OBSOLETO)
                            lotto.QtaMinimaComplessiva = idr.GetNullableInt32OrDefault(LOT_QTA_MINIMA_COMPLESSIVA)
                            lotto.CodiceUslInserimento = idr.GetStringOrDefault(LOT_USL_CODICE_INSERIMENTO)

                        End If
                    End Using
                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return lotto

        End Function

        ''' <summary>
        ''' Caricamento del lotto specificato per il magazzino centrale.
        ''' Le dosi rimaste sono calcolate come somma delle dosi del lotto realtivamente ai singoli magazzini.
        ''' La quantità minima è presa dalla t_ana_lotti perchè è quella relativa al magazzino centrale.
        ''' </summary>
        Public Function LoadLottoMagazzinoCentrale(codiceLotto As String, idUtente As Integer, codDistr As String) As Entities.LottoMagazzino Implements ILottiProvider.LoadLottoMagazzinoCentrale

            ' Query di ricerca dei lotti relativa al magazzino centrale
            Me.CreateQueryRicercaLottiMagazzinoCentrale(_DAM, idUtente, codDistr)

            ' Aggiunta del filtro sul codice del lotto alla query
            _DAM.QB.AddWhereCondition("lot_codice", Comparatori.Uguale, codiceLotto, DataTypes.Stringa)

            Dim listLottiMagazzinoCentrale As List(Of Entities.LottoMagazzino) = Me.GetListLottiMagazzino(_DAM, True)

            If listLottiMagazzinoCentrale Is Nothing OrElse listLottiMagazzinoCentrale.Count = 0 Then

                Return Nothing

            End If

            Return listLottiMagazzinoCentrale(0)

        End Function

        ''' <summary>
        ''' Caricamento lotti per il magazzino centrale, in base ai filtri impostati.
        ''' Le dosi rimaste sono calcolate come somma delle dosi dei singoli magazzini.
        ''' La quantità minima è presa dalla t_ana_lotti perchè è quella relativa al magazzino centrale.
        ''' </summary>
        Public Function LoadLottiMagazzinoCentrale(filtriRicerca As Filters.FiltriRicercaLottiMagazzino, listDatiOrdinamento As List(Of Entities.DatiOrdinamento), pagingOptions As PagingOptions, userId As Integer) As List(Of Entities.LottoMagazzino) Implements ILottiProvider.LoadLottiMagazzinoCentrale

            ' Impostazione query di ricerca dei lotti relativa al magazzino centrale
            Me.CreateQueryRicercaLottiMagazzinoCentrale(_DAM, userId, filtriRicerca.CodiceDistretto)

            ' Aggiunta dei filtri specificati dall'utente
            If Not filtriRicerca Is Nothing Then

                Me.AddFiltriRicercaMagazzino(filtriRicerca, _DAM.QB, True)

            End If

            ' Ordinamento
            If Not listDatiOrdinamento Is Nothing AndAlso listDatiOrdinamento.Count > 0 Then

                Dim campiOrdinamento As New System.Text.StringBuilder()

                Dim campo As String = String.Empty

                ' Indica se utilizzare la funzione oracle NLS_SORT per effettuare l'ordinamento.
                ' Con questa funzione, oracle ordina i campi stringa mettendo i numeri prima delle lettere (come .net). 
                ' I campi numerici e i campi data non hanno bisogno di utilizzare questa funzione 
                ' perchè vengono già ordinati correttamente.
                Dim nlsSortValue As String = String.Empty

                For Each datiOrdinamento As Entities.DatiOrdinamento In listDatiOrdinamento

                    campo = Me.GetDbFieldFromPropertyName(datiOrdinamento.Campo)

                    nlsSortValue = Me.GetNlsSortValueFromPropertyName(datiOrdinamento.Campo)

                    If Not String.IsNullOrEmpty(campo) Then

                        Dim sortField As String = String.Empty

                        If String.IsNullOrEmpty(nlsSortValue) Then
                            sortField = campo
                        Else
                            sortField = String.Format("NLSSORT({0}, 'NLS_SORT={1}')", campo, nlsSortValue)
                        End If

                        campiOrdinamento.AppendFormat("{0} {1},", sortField, datiOrdinamento.Verso)

                    End If

                Next

                ' Aggiunta dei campi di ordinamento, se specificati
                If campiOrdinamento.Length > 0 Then

                    campiOrdinamento.Remove(campiOrdinamento.Length - 1, 1)

                    _DAM.QB.AddOrderByFields(campiOrdinamento.ToString())

                End If

            End If

            ' Paginazione query
            If Not pagingOptions Is Nothing Then

                _DAM.QB.AddPaginatedOracleQuery(pagingOptions.StartRecordIndex, pagingOptions.EndRecordIndex)

            End If

            Return Me.GetListLottiMagazzino(_DAM, True)

        End Function

        ''' <summary>
        ''' Conteggio lotti magazzino centrale
        ''' </summary>
        Public Function CountLottiMagazzinoCentrale(filtriRicerca As Filters.FiltriRicercaLottiMagazzino, idUtente As Integer) As Integer Implements ILottiProvider.CountLottiMagazzinoCentrale

            With _DAM.QB

                .EmptyStringIsNULL = True

                ' --- Query più interna con i campi e la somma delle dosi --- '
                .NewQuery()

                Me.CreateQuerySommaDosi(_DAM, idUtente, filtriRicerca.CodiceDistretto)

                .AddSelectFields("noc_descrizione")
                .AddTables("t_ana_nomi_commerciali")
                .AddWhereCondition("lot_noc_codice", Comparatori.Uguale, "noc_codice", DataTypes.OutJoinLeft)
                .AddGroupByFields("noc_descrizione")

                Dim querySommaDosi As String = .GetSelect()

                ' --- Query di select con i filtri --- '
                .NewQuery(False, False)

                .AddSelectFields("*")
                .AddTables(String.Format("({0})", querySommaDosi))

                ' Aggiunta dei filtri impostati dall'utente alla query
                If Not filtriRicerca Is Nothing Then

                    Me.AddFiltriRicercaMagazzino(filtriRicerca, _DAM.QB, True)

                End If

                Dim queryRicercaLotti As String = .GetSelect()

                ' --- Query più esterna per il count --- '
                .NewQuery(False, False)

                .AddSelectFields(.FC.Count("*"))
                .AddTables(String.Format("({0})", queryRicercaLotti))

            End With

            Dim obj As Object = _DAM.ExecScalar()

            Dim countLotti As Integer = 0

            Try
                countLotti = Convert.ToInt32(obj)
            Catch ex As Exception
                countLotti = 0
            End Try

            Return countLotti

        End Function

        ''' <summary>
        ''' Caricamento dati del lotto relativi a tutti i magazzini (magazzino e dosi rimaste)
        ''' </summary>
        Public Function LoadDettaglioDosiLotto(codiceLotto As String, noScortaNulla As Boolean, userId As Integer, codDistr As String) As List(Of Entities.LottoDettaglioMagazzino) Implements ILottiProvider.LoadDettaglioDosiLotto

            With _DAM.QB

                .EmptyStringIsNULL = True

                .NewQuery()

                .AddTables("t_lot_lotti_consultori, t_ana_consultori, t_ana_lotti,t_ana_link_utenti_consultori")

                .AddSelectFields("lot_codice, lot_descrizione, cns_codice, cns_descrizione, lcn_dosi_rimaste, lcn_qta_minima, lcn_attivo")

                .AddWhereCondition("lcn_lot_codice", Comparatori.Uguale, codiceLotto, DataTypes.Stringa)
                If Not String.IsNullOrWhiteSpace(codDistr) Then
                    .AddWhereCondition("CNS_DIS_CODICE", Comparatori.Uguale, codDistr, DataTypes.Stringa)
                End If

                If noScortaNulla Then
                    .AddWhereCondition("lcn_dosi_rimaste", Comparatori.Maggiore, 0, DataTypes.Numero)
                End If

                .OpenParanthesis()
                .AddWhereCondition("cns_cns_magazzino", Comparatori.Is, "", DataTypes.Stringa)
                .AddWhereCondition("cns_cns_magazzino", Comparatori.Uguale, "cns_codice", DataTypes.Replace, "or")
                .CloseParanthesis()

                .AddWhereCondition("lcn_cns_codice", Comparatori.Uguale, "cns_codice", DataTypes.Join)
                .AddWhereCondition("lcn_lot_codice", Comparatori.Uguale, "lot_codice", DataTypes.Join)
                .AddWhereCondition("luc_cns_codice", Comparatori.Uguale, "cns_codice", DataTypes.Join)
                .AddWhereCondition("luc_ute_id", Comparatori.Uguale, userId, DataTypes.Numero)

            End With

            Dim listLottiDettaglioMagazzino As List(Of Entities.LottoDettaglioMagazzino) = Nothing

            Using idr As System.Data.IDataReader = _DAM.BuildDataReader()

                If Not idr Is Nothing Then

                    listLottiDettaglioMagazzino = New List(Of Entities.LottoDettaglioMagazzino)

                    Dim lot_codice As Integer = idr.GetOrdinal("lot_codice")
                    Dim lot_descrizione As Integer = idr.GetOrdinal("lot_descrizione")
                    Dim cns_codice As Integer = idr.GetOrdinal("cns_codice")
                    Dim cns_descrizione As Integer = idr.GetOrdinal("cns_descrizione")
                    Dim lcn_dosi_rimaste As Integer = idr.GetOrdinal("lcn_dosi_rimaste")
                    Dim lcn_attivo As Integer = idr.GetOrdinal("lcn_attivo")
                    Dim lcn_qta_minima As Integer = idr.GetOrdinal("lcn_qta_minima")

                    Dim lottoDettaglioMagazzino As Entities.LottoDettaglioMagazzino = Nothing

                    While idr.Read()

                        lottoDettaglioMagazzino = New Entities.LottoDettaglioMagazzino()

                        lottoDettaglioMagazzino.CodiceLotto = idr.GetStringOrDefault(lot_codice)
                        lottoDettaglioMagazzino.DescrizioneLotto = idr.GetStringOrDefault(lot_descrizione)
                        lottoDettaglioMagazzino.CodiceConsultorio = idr.GetStringOrDefault(cns_codice)
                        lottoDettaglioMagazzino.DescrizioneConsultorio = idr.GetStringOrDefault(cns_descrizione)
                        lottoDettaglioMagazzino.DosiRimaste = idr.GetInt32OrDefault(lcn_dosi_rimaste)
                        lottoDettaglioMagazzino.Attivo = idr.GetBooleanOrDefault(lcn_attivo)
                        lottoDettaglioMagazzino.QuantitaMinima = idr.GetInt32OrDefault(lcn_qta_minima)

                        listLottiDettaglioMagazzino.Add(lottoDettaglioMagazzino)

                    End While

                End If

            End Using

            Return listLottiDettaglioMagazzino

        End Function

        ''' <summary>
        ''' Restituisce true se esiste il lotto è presente nel consultorio specificato
        ''' </summary>
        Public Function IsLottoInConsultorio(codiceLotto As String, codiceConsultorio As String) As Boolean Implements ILottiProvider.IsLottoInConsultorio

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("1")
                .AddTables("t_lot_lotti_consultori")
                .AddWhereCondition("lcn_lot_codice", Comparatori.Uguale, codiceLotto, DataTypes.Stringa)
                .AddWhereCondition("lcn_cns_codice", Comparatori.Uguale, codiceConsultorio, DataTypes.Stringa)
            End With

            Dim result As Object = _DAM.ExecScalar()

            Return (Not result Is Nothing AndAlso Not result Is DBNull.Value)

        End Function

        ''' <summary>
        '''  True se il lotto specificato ha data scadenza inferiore alla data corrente
        ''' </summary>
        ''' <param name="codiceLotto"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsLottoScaduto(codiceLotto As String) As Boolean Implements ILottiProvider.IsLottoScaduto

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("1")
                .AddTables("t_ana_lotti")
                .AddWhereCondition("lot_codice", Comparatori.Uguale, codiceLotto, DataTypes.Stringa)
                .AddWhereCondition("lot_data_scadenza", Comparatori.Minore, DateTime.Now.Date, DataTypes.Data)
            End With

            Dim result As Object = _DAM.ExecScalar()

            Return (Not result Is Nothing AndAlso Not result Is DBNull.Value)

        End Function

        ''' <summary>
        ''' Restituisce true se esiste, nel consultorio, un lotto attivo (diverso da quello specificato)
        ''' associato allo stesso nome commerciale.
        ''' </summary>
        Public Function IsActiveAltroLottoStessoNomeCommerciale(codiceLotto As String, codiceNomeCommerciale As String, codiceConsultorio As String) As Boolean Implements ILottiProvider.IsActiveAltroLottoStessoNomeCommerciale

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("1")
                .AddTables("t_ana_lotti, t_lot_lotti_consultori")
                .AddWhereCondition("lot_codice", Comparatori.Uguale, "lcn_lot_codice", DataTypes.Join)
                .AddWhereCondition("lot_noc_codice", Comparatori.Uguale, codiceNomeCommerciale, DataTypes.Stringa)
                .AddWhereCondition("lot_codice", Comparatori.Diverso, codiceLotto, DataTypes.Stringa)
                .AddWhereCondition("lcn_cns_codice", Comparatori.Uguale, codiceConsultorio, DataTypes.Stringa)
                .AddWhereCondition("lcn_attivo", Comparatori.Uguale, "S", DataTypes.Stringa)
            End With

            Dim result As Object = _DAM.ExecScalar()

            Return (Not result Is Nothing AndAlso Not result Is DBNull.Value)

        End Function

        ''' <summary>
        ''' Restituisce le dosi rimaste per il lotto nel consultorio specificato.
        ''' Restituisce -1 se il lotto non è nel consultorio.
        ''' </summary>
        ''' <param name="codiceLotto"></param>
        ''' <param name="codiceConsultorioMagazzino"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDosiRimaste(codiceLotto As String, codiceConsultorioMagazzino As String) As Integer Implements ILottiProvider.GetDosiRimaste

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("lcn_dosi_rimaste")
                .AddTables("t_lot_lotti_consultori")
                .AddWhereCondition("lcn_lot_codice", Comparatori.Uguale, codiceLotto, DataTypes.Stringa)
                .AddWhereCondition("lcn_cns_codice", Comparatori.Uguale, codiceConsultorioMagazzino, DataTypes.Stringa)
            End With

            Dim result As Object = _DAM.ExecScalar()

            If result Is Nothing OrElse result Is DBNull.Value Then
                Return -1
            End If

            Return Convert.ToInt32(result)

        End Function

        ''' <summary>
        ''' Restituisce le dosi rimaste per ogni lotto specificato, nel consultorio indicato.
        ''' </summary>
        ''' <param name="codiciLotti"></param>
        ''' <param name="codiceConsultorioMagazzino"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDosiRimaste(codiciLotti As List(Of String), codiceConsultorioMagazzino As String) As List(Of KeyValuePair(Of String, Integer)) Implements ILottiProvider.GetDosiRimaste

            Dim listDosi As New List(Of KeyValuePair(Of String, Integer))()

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("lcn_lot_codice, lcn_dosi_rimaste")
                .AddTables("t_lot_lotti_consultori")
                .AddWhereCondition("lcn_cns_codice", Comparatori.Uguale, codiceConsultorioMagazzino, DataTypes.Stringa)
                .AddInWhereCondition("lcn_lot_codice", codiciLotti)
            End With

            Using idr As IDataReader = _DAM.BuildDataReader()
                If Not idr Is Nothing Then

                    Dim lcn_lot_codice As Integer = idr.GetOrdinal("lcn_lot_codice")
                    Dim lcn_dosi_rimaste As Integer = idr.GetOrdinal("lcn_dosi_rimaste")

                    While idr.Read()
                        listDosi.Add(New KeyValuePair(Of String, Integer)(idr.GetString(lcn_lot_codice), idr.GetInt32OrDefault(lcn_dosi_rimaste)))
                    End While

                End If
            End Using

            Return listDosi

        End Function

        ''' <summary>
        ''' Restituisce la quantita minima per il lotto nel consultorio specificato.
        ''' Restituisce -1 se il lotto non è nel consultorio.
        ''' </summary>
        Public Function GetQuantitaMinima(codiceLotto As String, codiceConsultorioMagazzino As String) As Integer Implements ILottiProvider.GetQuantitaMinima

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("lcn_qta_minima")
                .AddTables("t_lot_lotti_consultori")
                .AddWhereCondition("lcn_lot_codice", Comparatori.Uguale, codiceLotto, DataTypes.Stringa)
                .AddWhereCondition("lcn_cns_codice", Comparatori.Uguale, codiceConsultorioMagazzino, DataTypes.Stringa)
            End With

            Dim result As Object = _DAM.ExecScalar()

            If result Is Nothing OrElse result Is DBNull.Value Then
                Return -1
            End If

            Return Convert.ToInt32(result)

        End Function

        Public Function GetEtaAttivazioneLotto(codiceLotto As String, codiceConsultorioLotto As String) As ILottiProvider.EtaAttivazioneLottoResult Implements ILottiProvider.GetEtaAttivazioneLotto

            Dim etaAttivazione As ILottiProvider.EtaAttivazioneLottoResult = Nothing

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("lcn_eta_min_attivazione, lcn_eta_max_attivazione")
                .AddTables("t_lot_lotti_consultori")
                .AddWhereCondition("lcn_lot_codice", Comparatori.Uguale, codiceLotto, DataTypes.Stringa)
                .AddWhereCondition("lcn_cns_codice", Comparatori.Uguale, codiceConsultorioLotto, DataTypes.Stringa)
            End With

            Using idr As IDataReader = _DAM.BuildDataReader()
                If Not idr Is Nothing Then
                    '--
                    Dim lcn_eta_min_attivazione As Integer = idr.GetOrdinal("lcn_eta_min_attivazione")
                    Dim lcn_eta_max_attivazione As Integer = idr.GetOrdinal("lcn_eta_max_attivazione")
                    '--
                    If idr.Read() Then
                        etaAttivazione = New ILottiProvider.EtaAttivazioneLottoResult()
                        etaAttivazione.EtaMinima = idr.GetNullableInt32OrDefault(lcn_eta_min_attivazione)
                        etaAttivazione.EtaMassima = idr.GetNullableInt32OrDefault(lcn_eta_max_attivazione)
                    End If
                    '--
                End If
            End Using

            Return etaAttivazione

        End Function

        ''' <summary>
        ''' Restituisce true se il codice è presente nell'anagrafica dei lotti.
        ''' </summary>
        Public Function ExistsCodiceLotto(codiceLotto As String) As Boolean Implements ILottiProvider.ExistsCodiceLotto

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("1")
                .AddTables("t_ana_lotti")
                .AddWhereCondition("lot_codice", Comparatori.Uguale, codiceLotto, DataTypes.Stringa)
            End With

            Dim result As Object = _DAM.ExecScalar()

            Return (Not result Is Nothing AndAlso Not result Is DBNull.Value)

        End Function

        ''' <summary>
        ''' Restituisce true se il codice del lotto è associato al consultorio specificato.
        ''' </summary>
        Public Function ExistsLottoConsultorio(codiceLotto As String, codiceConsultorio As String) As Boolean Implements ILottiProvider.ExistsLottoConsultorio

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("1")
                .AddTables("t_lot_lotti_consultori")
                .AddWhereCondition("lcn_lot_codice", Comparatori.Uguale, codiceLotto, DataTypes.Stringa)
                .AddWhereCondition("lcn_cns_codice", Comparatori.Uguale, codiceConsultorio, DataTypes.Stringa)
            End With

            Dim result As Object = _DAM.ExecScalar()

            Return (Not result Is Nothing AndAlso Not result Is DBNull.Value)

        End Function

        ''' <summary>
        ''' Restitusice una lista di associazioni relative ai lotti specificati
        ''' </summary>
        ''' <param name="listCodiciLotti"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetLottiAssociazioni(listCodiciLotti As List(Of String)) As List(Of ILottiProvider.LottiAssociazioniResult) Implements ILottiProvider.GetLottiAssociazioni
            '--
            Dim listLottiAssociazioni As New List(Of ILottiProvider.LottiAssociazioniResult)()
            '--
            With _DAM.QB
                '--
                .NewQuery()
                .AddSelectFields("lot_codice, ass_codice, ass_descrizione")
                .AddTables("t_ana_link_noc_associazioni, t_ana_lotti, t_ana_associazioni")
                .AddWhereCondition("nal_noc_codice", Comparatori.Uguale, "lot_noc_codice", DataTypes.Join)
                .AddWhereCondition("ass_codice", Comparatori.Uguale, "nal_ass_codice", DataTypes.Join)
                '--
                Dim stbCodiciLotti As New System.Text.StringBuilder()
                '--
                For Each codiceLotto As String In listCodiciLotti
                    stbCodiciLotti.AppendFormat("{0},", .AddCustomParam(codiceLotto))
                Next
                '--
                If stbCodiciLotti.Length > 0 Then stbCodiciLotti.Remove(stbCodiciLotti.Length - 1, 1)
                '--
                .AddWhereCondition("lot_codice", Comparatori.In, stbCodiciLotti.ToString(), DataTypes.Replace)
                '--
            End With

            Using idr As IDataReader = _DAM.BuildDataReader()
                '--
                If Not idr Is Nothing Then
                    '--
                    Dim codiceLottoIndex As Int32 = idr.GetOrdinal("lot_codice")
                    Dim codiceAssociazioneIndex As Int32 = idr.GetOrdinal("ass_codice")
                    Dim descrizioneAssociazioneIndex As Int32 = idr.GetOrdinal("ass_descrizione")
                    '--
                    Dim lottoAssociazione As ILottiProvider.LottiAssociazioniResult = Nothing
                    '--
                    While idr.Read()
                        '--
                        lottoAssociazione = New ILottiProvider.LottiAssociazioniResult()
                        '--
                        lottoAssociazione.CodiceLotto = idr.GetString(codiceLottoIndex)
                        lottoAssociazione.CodiceAssociazione = idr.GetString(codiceAssociazioneIndex)
                        lottoAssociazione.DescrizioneAssociazione = idr.GetString(descrizioneAssociazioneIndex)
                        '--
                        listLottiAssociazioni.Add(lottoAssociazione)
                        '--
                    End While
                    '--
                End If
                '--
            End Using
            '--
            Return listLottiAssociazioni
            '--
        End Function

        ''' <summary>
        ''' Restituisce il numero di dosi totali rimaste nel consultorio specificato.
        ''' </summary>
        Public Function CountLottiConsultorio(codiceConsultorio As String) As Integer Implements ILottiProvider.CountLottiConsultorio

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("sum(lcn_dosi_rimaste)")
                .AddTables("t_lot_lotti_consultori")
                .AddWhereCondition("lcn_cns_codice", Comparatori.Uguale, codiceConsultorio, DataTypes.Stringa)
            End With

            Dim obj As Object = _DAM.ExecScalar()
            If obj Is Nothing OrElse obj Is DBNull.Value Then
                Return 0
            End If

            Return Convert.ToInt32(obj)

        End Function

        ''' <summary>
        ''' Restituisce il codice del ciclo associato al nome commerciale relativo al lotto specificato, se presente e se il ciclo non è obsoleto
        ''' </summary>
        ''' <param name="codiceLotto"></param>
        ''' <returns></returns>
        Public Function GetCicloNomeCommercialeByLotto(codiceLotto As String) As String Implements ILottiProvider.GetCicloNomeCommercialeByLotto

            Dim codiceCiclo As String = String.Empty

            Dim ownConnection As Boolean = False

            Dim query As String =
                "SELECT CIC_CODICE
FROM T_ANA_LOTTI
JOIN T_ANA_NOMI_COMMERCIALI ON LOT_NOC_CODICE = NOC_CODICE
JOIN T_ANA_CICLI ON NOC_CIC_CODICE = CIC_CODICE
WHERE LOT_CODICE = :LOT_CODICE
AND (CIC_OBSOLETO IS NULL OR CIC_OBSOLETO = 'N')"

            Try
                Using cmd As New OracleCommand(query, Connection)

                    cmd.Parameters.AddWithValue("LOT_CODICE", codiceLotto)

                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                        codiceCiclo = obj.ToString()
                    End If

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return codiceCiclo

        End Function

#Region " RicercaLotti per WebService "

        ''' <summary>
        ''' Restitusice le informazioni sui lotti filtrando per il codice AIC 
        ''' </summary>
        ''' <param name="codiceAIC"></param>
        ''' <param name="codiceUlss"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetLottiByAIC(codiceAIC As String, codiceUlss As String) As List(Of LottoRicercaWebService) Implements ILottiProvider.GetLottiByAIC

            Dim lotti As New List(Of LottoRicercaWebService)()

            Dim ownConnection As Boolean = False

            Dim query As String =
                " select distinct lot_codice, lot_data_scadenza, noc_codice_aic, vac_codice_acn " +
                " from t_ana_lotti " +
                " join t_ana_nomi_commerciali on lot_noc_codice = noc_codice " +
                " join t_ana_link_noc_associazioni on noc_codice = nal_noc_codice " +
                " join t_ana_link_ass_vaccinazioni on nal_ass_codice = val_ass_codice " +
                " join t_ana_vaccinazioni on val_vac_codice = vac_codice " +
                " where (lot_data_scadenza is null or lot_data_scadenza > sysdate) " +
                " and noc_codice_aic = :noc_codice_aic "

            If Not String.IsNullOrWhiteSpace(codiceUlss) Then
                query += " and exists ( " +
                         "     select 1  " +
                         "     from t_lot_lotti_consultori " +
                         "     join t_ana_consultori on lcn_cns_codice = cns_codice " +
                         "     where lcn_lot_codice = lot_codice " +
                         "     and cns_azi_codice = :cns_azi_codice)  "
            End If

            query += " order by lot_data_scadenza desc, lot_codice"

            Try
                Using cmd As New OracleCommand(query, Connection)

                    cmd.Parameters.AddWithValue("noc_codice_aic", codiceAIC)

                    If Not String.IsNullOrWhiteSpace(codiceUlss) Then
                        cmd.Parameters.AddWithValue("cns_azi_codice", codiceUlss)
                    End If

                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim lot_codice As Int32 = idr.GetOrdinal("lot_codice")
                            Dim lot_data_scadenza As Int32 = idr.GetOrdinal("lot_data_scadenza")
                            Dim noc_codice_aic As Int32 = idr.GetOrdinal("noc_codice_aic")
                            Dim vac_codice_acn As Int32 = idr.GetOrdinal("vac_codice_acn")

                            While idr.Read()

                                lotti.Add(New LottoRicercaWebService With {
                                    .CodLotto = idr.GetStringOrDefault(lot_codice),
                                    .DataScadenza = idr.GetDateTimeOrDefault(lot_data_scadenza),
                                    .AIC = idr.GetStringOrDefault(noc_codice_aic),
                                    .AvnVacc = idr.GetStringOrDefault(vac_codice_acn)
                                })

                            End While

                        End If
                    End Using

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return lotti

        End Function

        ''' <summary>
        ''' Restitusice le informazioni sui lotti filtrando per il codice AIC 
        ''' </summary>
        ''' <param name="codicePrincipioVaccinale"></param>
        ''' <param name="codiceUlss"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetLottiByCodVacc(codicePrincipioVaccinale As String, codiceUlss As String) As List(Of LottoRicercaWebService) Implements ILottiProvider.GetLottiByCodVacc

            Dim lotti As New List(Of LottoRicercaWebService)()

            Dim ownConnection As Boolean = False

            Dim query As String =
                " select distinct lot_codice, lot_data_scadenza, noc_codice_aic, vac_codice_acn" +
                " from t_ana_lotti " +
                " join t_ana_nomi_commerciali on lot_noc_codice = noc_codice " +
                " join t_ana_link_noc_associazioni on noc_codice = nal_noc_codice " +
                " join t_ana_link_ass_vaccinazioni on nal_ass_codice = val_ass_codice " +
                " join t_ana_vaccinazioni on val_vac_codice = vac_codice" +
                " where (lot_data_scadenza is null or lot_data_scadenza > sysdate) " +
                " and vac_codice_acn = :vac_codice_acn "

            If Not String.IsNullOrWhiteSpace(codiceUlss) Then
                query += " and exists ( " +
                         "     select 1  " +
                         "     from t_lot_lotti_consultori " +
                         "     join t_ana_consultori on lcn_cns_codice = cns_codice " +
                         "     where lcn_lot_codice = lot_codice " +
                         "     and cns_azi_codice = :cns_azi_codice)  "
            End If

            query += " order by lot_data_scadenza desc, lot_codice"

            Try
                Using cmd As New OracleCommand(query, Connection)

                    cmd.Parameters.AddWithValue("vac_codice_acn", codicePrincipioVaccinale)

                    If Not String.IsNullOrWhiteSpace(codiceUlss) Then
                        cmd.Parameters.AddWithValue("cns_azi_codice", codiceUlss)
                    End If

                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim lot_codice As Int32 = idr.GetOrdinal("lot_codice")
                            Dim lot_data_scadenza As Int32 = idr.GetOrdinal("lot_data_scadenza")
                            Dim noc_codice_aic As Int32 = idr.GetOrdinal("noc_codice_aic")
                            Dim vac_codice_acn As Int32 = idr.GetOrdinal("vac_codice_acn")

                            While idr.Read()

                                lotti.Add(New LottoRicercaWebService With {
                                    .CodLotto = idr.GetStringOrDefault(lot_codice),
                                    .DataScadenza = idr.GetDateTimeOrDefault(lot_data_scadenza),
                                    .AIC = idr.GetStringOrDefault(noc_codice_aic),
                                    .AvnVacc = idr.GetStringOrDefault(vac_codice_acn)
                                })

                            End While

                        End If
                    End Using

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return lotti

        End Function

#End Region

#End Region

#Region " Private "

        ''' <summary>
        ''' Impostazione query di ricerca dei lotti, per il magazzino centrale
        ''' </summary>
        Private Sub CreateQueryRicercaLottiMagazzinoCentrale(dam As IDAM, idutente As Integer, codiceDistretto As String)

            With dam.QB

                .EmptyStringIsNULL = True

                .NewQuery()

                Me.CreateQuerySommaDosi(dam, idutente, codiceDistretto)

                Dim querySommaDosi As String = .GetSelect()

                .NewQuery(False, False)

                .AddSelectFields("tot.lot_codice lot_codice, tot.lot_descrizione lot_descrizione")
                .AddSelectFields("tot.lot_data_preparazione lot_data_preparazione, tot.lot_data_scadenza lot_data_scadenza, tot.lot_ditta lot_ditta")
                .AddSelectFields("tot.lot_dosi_scatola lot_dosi_scatola, tot.lot_note lot_note, tot.lot_obsoleto lot_obsoleto")
                .AddSelectFields("tot.lot_noc_codice lot_noc_codice, tot.lot_qta_minima_complessiva lot_qta_minima_complessiva")
                .AddSelectFields("tot.dosi_rimaste dosi_rimaste, noc_codice, noc_descrizione || decode(for_descrizione, null, '', ' (' || for_descrizione || ')') noc_descrizione")
                .AddSelectFields("noc_sesso, noc_eta_inizio, noc_eta_fine, noc_ass_codice, noc_for_codice, for_descrizione")
                .AddSelectFields("tot.lot_ute_id_modifica_obsoleto lot_ute_id_modifica_obsoleto, tot.lot_data_modifica_obsoleto lot_data_modifica_obsoleto")

                .AddTables("(" + querySommaDosi + ") tot, t_ana_nomi_commerciali, t_ana_fornitori")

                .AddWhereCondition("noc_for_codice", Comparatori.Uguale, "for_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("lot_noc_codice", Comparatori.Uguale, "noc_codice", DataTypes.OutJoinLeft)

            End With

        End Sub

        ''' <summary>
        ''' Impostazione query con somma dosi rimaste, per magazzino centrale
        ''' </summary>
        Private Sub CreateQuerySommaDosi(dam As IDAM, idUtente As Integer, codiceDistretto As String)

            With dam.QB

                .AddSelectFields("nvl(sum(lcn_dosi_rimaste), 0) dosi_rimaste, lot_codice, lot_descrizione")
                .AddSelectFields("lot_data_preparazione, lot_data_scadenza, lot_ditta, lot_dosi_scatola, lot_note, lot_obsoleto")
                .AddSelectFields("lot_noc_codice, lot_qta_minima_complessiva, lot_ute_id_modifica_obsoleto, lot_data_modifica_obsoleto")

                .AddTables("t_ana_lotti, t_lot_lotti_consultori, t_ana_consultori,t_ana_link_utenti_consultori")

                .AddWhereCondition("lot_codice", Comparatori.Uguale, "lcn_lot_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("lcn_cns_codice", Comparatori.Uguale, "cns_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("luc_cns_codice", Comparatori.Uguale, "cns_codice", DataTypes.Join)

                .OpenParanthesis()
                .AddWhereCondition("cns_cns_magazzino", Comparatori.Is, "", DataTypes.Stringa)
                .AddWhereCondition("cns_cns_magazzino", Comparatori.Uguale, "cns_codice", DataTypes.Replace, "or")
                .CloseParanthesis()
                If Not String.IsNullOrWhiteSpace(codiceDistretto) Then
                    .AddWhereCondition("CNS_DIS_CODICE", Comparatori.Uguale, codiceDistretto, DataTypes.Stringa)
                End If
                .AddWhereCondition("luc_ute_id", Comparatori.Uguale, idUtente, DataTypes.Numero)
                .AddGroupByFields("lot_codice, lot_descrizione, lot_data_preparazione, lot_data_scadenza, lot_ditta")
                .AddGroupByFields("lot_dosi_scatola, lot_note, lot_obsoleto, lot_noc_codice, lot_qta_minima_complessiva")
                .AddGroupByFields("lot_ute_id_modifica_obsoleto, lot_data_modifica_obsoleto")

            End With

        End Sub

        ''' <summary>
        ''' Restituisce i lotti in base alla query impostata nel dam
        ''' </summary>
        Private Function GetListLottiMagazzino(dam As IDAM, isMagazzinoCentrale As Boolean) As List(Of Entities.LottoMagazzino)

            Dim listLottiMagazzino As List(Of Entities.LottoMagazzino) = Nothing

            Using idr As System.Data.IDataReader = dam.BuildDataReader()

                If Not idr Is Nothing Then

                    ' Campi comuni a magazzino centrale e locale
                    Dim lot_codice As Integer = idr.GetOrdinal("lot_codice")
                    Dim lot_descrizione As Integer = idr.GetOrdinal("lot_descrizione")
                    Dim lot_data_preparazione As Integer = idr.GetOrdinal("lot_data_preparazione")
                    Dim lot_data_scadenza As Integer = idr.GetOrdinal("lot_data_scadenza")
                    Dim lot_ditta As Integer = idr.GetOrdinal("lot_ditta")
                    Dim lot_dosi_scatola As Integer = idr.GetOrdinal("lot_dosi_scatola")
                    Dim lot_note As Integer = idr.GetOrdinal("lot_note")
                    Dim lot_obsoleto As Integer = idr.GetOrdinal("lot_obsoleto")
                    Dim noc_codice As Integer = idr.GetOrdinal("noc_codice")
                    Dim noc_descrizione As Integer = idr.GetOrdinal("noc_descrizione")
                    Dim noc_sesso As Integer = idr.GetOrdinal("noc_sesso")
                    Dim noc_eta_inizio As Integer = idr.GetOrdinal("noc_eta_inizio")
                    Dim noc_eta_fine As Integer = idr.GetOrdinal("noc_eta_fine")
                    Dim noc_ass_codice As Integer = idr.GetOrdinal("noc_ass_codice")
                    Dim noc_for_codice As Integer = idr.GetOrdinal("noc_for_codice")
                    Dim for_descrizione As Integer = idr.GetOrdinal("for_descrizione")
                    Dim lot_ute_id_modifica_obsoleto As Integer = idr.GetOrdinal("lot_ute_id_modifica_obsoleto")
                    Dim lot_data_modifica_obsoleto As Integer = idr.GetOrdinal("lot_data_modifica_obsoleto")
                    '--
                    Dim dosi_rimaste As Integer
                    Dim qta_minima As Integer
                    Dim lcn_cns_codice As Integer
                    Dim lcn_attivo As Integer
                    Dim lcn_ute_id_modifica_attivo As Integer
                    Dim lcn_data_modifica_attivo As Integer
                    Dim lcn_eta_min_attivazione As Integer
                    Dim lcn_eta_max_attivazione As Integer

                    If isMagazzinoCentrale Then
                        '--
                        ' Campi presenti solo per il magazzino centrale
                        dosi_rimaste = idr.GetOrdinal("dosi_rimaste")
                        qta_minima = idr.GetOrdinal("lot_qta_minima_complessiva")
                        '--
                    Else
                        '--
                        ' Campi presenti solo per il magazzino locale
                        dosi_rimaste = idr.GetOrdinal("lcn_dosi_rimaste")
                        qta_minima = idr.GetOrdinal("lcn_qta_minima")
                        lcn_cns_codice = idr.GetOrdinal("lcn_cns_codice")
                        lcn_attivo = idr.GetOrdinal("lcn_attivo")
                        lcn_ute_id_modifica_attivo = idr.GetOrdinal("lcn_ute_id_modifica_attivo")
                        lcn_data_modifica_attivo = idr.GetOrdinal("lcn_data_modifica_attivo")
                        lcn_eta_min_attivazione = idr.GetOrdinal("lcn_eta_min_attivazione")
                        lcn_eta_max_attivazione = idr.GetOrdinal("lcn_eta_max_attivazione")
                        '--
                    End If

                    listLottiMagazzino = New List(Of Entities.LottoMagazzino)()

                    Dim lottoMagazzino As Entities.LottoMagazzino = Nothing

                    While idr.Read()

                        lottoMagazzino = New Entities.LottoMagazzino()

                        ' Valori comuni a centrale e locale
                        lottoMagazzino.CodiceLotto = idr.GetStringOrDefault(lot_codice)
                        lottoMagazzino.DescrizioneLotto = idr.GetStringOrDefault(lot_descrizione)
                        lottoMagazzino.DataPreparazione = idr.GetDateTimeOrDefault(lot_data_preparazione)
                        lottoMagazzino.DataScadenza = idr.GetDateTimeOrDefault(lot_data_scadenza)
                        lottoMagazzino.Ditta = idr.GetStringOrDefault(lot_ditta)
                        lottoMagazzino.DosiScatola = idr.GetInt32OrDefault(lot_dosi_scatola)
                        lottoMagazzino.Note = idr.GetStringOrDefault(lot_note)
                        lottoMagazzino.Obsoleto = idr.GetBooleanOrDefault(lot_obsoleto)
                        lottoMagazzino.CodiceNomeCommerciale = idr.GetStringOrDefault(noc_codice)
                        lottoMagazzino.DescrizioneNomeCommerciale = idr.GetStringOrDefault(noc_descrizione)
                        lottoMagazzino.Sesso = idr.GetStringOrDefault(noc_sesso)
                        lottoMagazzino.EtaInizio = idr.GetNullableInt32OrDefault(noc_eta_inizio)
                        lottoMagazzino.EtaFine = idr.GetNullableInt32OrDefault(noc_eta_fine)
                        lottoMagazzino.CodiceAssociazione = idr.GetStringOrDefault(noc_ass_codice)
                        lottoMagazzino.CodiceFornitore = idr.GetStringOrDefault(noc_for_codice)
                        lottoMagazzino.DescrizioneFornitore = idr.GetStringOrDefault(for_descrizione)
                        lottoMagazzino.IdUtenteModificaFlagObsoleto = idr.GetNullableInt32OrDefault(lot_ute_id_modifica_obsoleto)
                        lottoMagazzino.DataModificaFlagObsoleto = idr.GetNullableDateTimeOrDefault(lot_data_modifica_obsoleto)

                        ' Valori diversi per centrale e locale
                        lottoMagazzino.DosiRimaste = idr.GetInt32OrDefault(dosi_rimaste)
                        lottoMagazzino.QuantitaMinima = idr.GetInt32OrDefault(qta_minima)

                        ' Valori presenti solo per il magazzino locale
                        If isMagazzinoCentrale Then
                            lottoMagazzino.CodiceConsultorio = String.Empty
                            lottoMagazzino.Attivo = False
                            lottoMagazzino.IdUtenteModificaFlagAttivo = Nothing
                            lottoMagazzino.DataModificaFlagAttivo = Nothing
                            lottoMagazzino.EtaMinimaAttivazione = Nothing
                            lottoMagazzino.EtaMassimaAttivazione = Nothing
                        Else
                            lottoMagazzino.CodiceConsultorio = idr.GetStringOrDefault(lcn_cns_codice)
                            lottoMagazzino.Attivo = idr.GetBooleanOrDefault(lcn_attivo)
                            lottoMagazzino.IdUtenteModificaFlagAttivo = idr.GetNullableInt32OrDefault(lcn_ute_id_modifica_attivo)
                            lottoMagazzino.DataModificaFlagAttivo = idr.GetNullableDateTimeOrDefault(lcn_data_modifica_attivo)
                            lottoMagazzino.EtaMinimaAttivazione = idr.GetNullableInt32OrDefault(lcn_eta_min_attivazione)
                            lottoMagazzino.EtaMassimaAttivazione = idr.GetNullableInt32OrDefault(lcn_eta_max_attivazione)
                        End If

                        ' Campi aggiunti all'entità solo per la gestione dello user control InsDatiLotto
                        'lottoMagazzino.UnitaMisura = Enumerators.UnitaMisuraLotto.Dose
                        'lottoMagazzino.QuantitaIniziale = 0

                        listLottiMagazzino.Add(lottoMagazzino)

                    End While

                End If

            End Using

            Return listLottiMagazzino

        End Function

        ''' <summary>
        ''' Aggiunta dei filtri di ricerca alla query
        ''' </summary>
        Private Sub AddFiltriRicercaMagazzino(filtriRicerca As Filters.FiltriRicercaLottiMagazzino, qb As AbstractQB, isMagazzinoCentrale As Boolean)

            With qb

                If Not String.IsNullOrEmpty(filtriRicerca.CodiceLotto) Then
                    .AddWhereCondition("lot_codice", Comparatori.Like, filtriRicerca.CodiceLotto + "%", DataTypes.Stringa)
                End If

                If Not String.IsNullOrEmpty(filtriRicerca.DescrizioneLotto) Then
                    .AddWhereCondition("lot_descrizione", Comparatori.Like, filtriRicerca.DescrizioneLotto + "%", DataTypes.Stringa)
                End If

                If Not String.IsNullOrEmpty(filtriRicerca.CodiceNomeCommerciale) Then
                    .AddWhereCondition("lot_noc_codice", Comparatori.Like, filtriRicerca.CodiceNomeCommerciale + "%", DataTypes.Stringa)
                End If

                If Not String.IsNullOrEmpty(filtriRicerca.DescrizioneNomeCommerciale) Then
                    .AddWhereCondition("noc_descrizione", Comparatori.Like, filtriRicerca.DescrizioneNomeCommerciale + "%", DataTypes.Stringa)
                End If

                If Not filtriRicerca.SoloLottiSequestrati Then
                    .OpenParanthesis()
                    .AddWhereCondition("lot_obsoleto", Comparatori.Uguale, "N", DataTypes.Stringa)
                    .AddWhereCondition("lot_obsoleto", Comparatori.Is, "", DataTypes.Stringa, "or")
                    .CloseParanthesis()
                Else
                    .AddWhereCondition("lot_obsoleto", Comparatori.Uguale, "S", DataTypes.Stringa)
                End If

                If filtriRicerca.NoLottiScaduti Then
                    .AddWhereCondition("lot_data_scadenza", Comparatori.MaggioreUguale, Date.Now, DataTypes.Data)
                End If

                ' Filtro dosi rimaste diverso se ricerca in magazzino centrale o in magazzino locale
                If isMagazzinoCentrale Then

                    If filtriRicerca.NoLottiScortaNulla Then
                        .AddWhereCondition("dosi_rimaste", Comparatori.Maggiore, 0, DataTypes.Numero)
                    End If

                Else

                    If filtriRicerca.NoLottiScortaNulla Then
                        .AddWhereCondition("a.lcn_dosi_rimaste", Comparatori.Maggiore, 0, DataTypes.Numero)
                    End If

                End If


            End With

        End Sub

        ''' <summary>
        ''' Restituisce il nome del campo su database, a partire dal nome della proprietà specificata
        ''' </summary>
        Private Function GetDbFieldFromPropertyName(nomeCampo As String) As String

            Select Case nomeCampo

                Case "CodiceLotto"
                    Return "lot_codice"

                Case "DescrizioneLotto"
                    Return "lot_descrizione"

                Case "DescrizioneNomeCommerciale"
                    Return "noc_descrizione"

                Case "DosiRimaste"
                    Return "dosi_rimaste"   ' funziona solo per il caricamento da mag centrale, da mag locale sarebbe lcn_dosi_rimaste

                Case "DataPreparazione"
                    Return "lot_data_preparazione"

                Case "DataScadenza"
                    Return "lot_data_scadenza"

            End Select

            Return String.Empty

        End Function

        ''' <summary>
        ''' Restituisce il valore per cui impostare la funzione di ordinamento di oracle NLSSORT.
        ''' Se tale funzione non deve essere usata, restituisce la stringa vuota.
        ''' </summary>
        Private Function GetNlsSortValueFromPropertyName(nomeCampo As String) As String

            Select Case nomeCampo

                Case "CodiceLotto",
                     "DescrizioneLotto",
                     "DescrizioneNomeCommerciale"

                    Return "BINARY"

            End Select

            Return String.Empty

        End Function

#End Region

#End Region

#Region " Salvataggio Lotti "

#Region " Inserimenti "

        ''' <summary>
        ''' Inserimento lotto in anagrafica, da magazzino centrale
        ''' </summary>
        Public Function InsertLottoMagazzinoCentrale(lottoMagazzino As Entities.LottoMagazzino) As Integer Implements ILottiProvider.InsertLottoMagazzinoCentrale

            Return Me.InsertLotto(lottoMagazzino, True)

        End Function

        ''' <summary>
        ''' Inserimento lotto in anagrafica, da magazzino locale
        ''' </summary>
        ''' <param name="lottoMagazzino"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsertLotto(lottoMagazzino As Entities.LottoMagazzino) As Integer Implements ILottiProvider.InsertLotto

            Return Me.InsertLotto(lottoMagazzino, False)

        End Function

        ''' <summary>
        ''' Inserimento lotto in anagrafica
        ''' </summary>
        Private Function InsertLotto(lottoMagazzino As Entities.LottoMagazzino, isMagazzinoCentrale As Boolean) As Integer

            With _DAM.QB

                .EmptyStringIsNULL = True

                .NewQuery()

                .AddTables("t_ana_lotti")

                .AddInsertField("lot_codice", lottoMagazzino.CodiceLotto, DataTypes.Stringa)
                .AddInsertField("lot_descrizione", lottoMagazzino.DescrizioneLotto, DataTypes.Stringa)
                .AddInsertField("lot_data_scadenza", lottoMagazzino.DataScadenza, DataTypes.Data)
                .AddInsertField("lot_ditta", lottoMagazzino.Ditta, DataTypes.Stringa)
                .AddInsertField("lot_dosi_scatola", lottoMagazzino.DosiScatola, DataTypes.Numero)
                .AddInsertField("lot_note", lottoMagazzino.Note, DataTypes.Stringa)
                .AddInsertField("lot_obsoleto", IIf(lottoMagazzino.Obsoleto, "S", "N"), DataTypes.Stringa)
                .AddInsertField("lot_noc_codice", lottoMagazzino.CodiceNomeCommerciale, DataTypes.Stringa)

                ' Gestione quantità minima solo se l'inserimento avviene da magazzino centrale
                If isMagazzinoCentrale Then
                    .AddInsertField("lot_qta_minima_complessiva", lottoMagazzino.QuantitaMinima, DataTypes.Numero)
                End If

                If lottoMagazzino.DataPreparazione > DateTime.MinValue Then
                    .AddInsertField("lot_data_preparazione", lottoMagazzino.DataPreparazione, DataTypes.Data)
                End If

                If Not lottoMagazzino.IdUtenteModificaFlagObsoleto Is Nothing Then
                    .AddInsertField("lot_ute_id_modifica_obsoleto", lottoMagazzino.IdUtenteModificaFlagObsoleto, DataTypes.Numero)
                End If

                If Not lottoMagazzino.DataModificaFlagObsoleto Is Nothing AndAlso lottoMagazzino.DataModificaFlagObsoleto > DateTime.MinValue Then
                    .AddInsertField("lot_data_modifica_obsoleto", lottoMagazzino.DataModificaFlagObsoleto, DataTypes.DataOra)
                End If

            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Insert)

        End Function

        ''' <summary>
        ''' Inserimento associazione lotto-consultorio
        ''' </summary>
        Public Function InsertLottoConsultorio(lottoMagazzino As Entities.LottoMagazzino) As Integer Implements ILottiProvider.InsertLottoConsultorio

            With _DAM.QB

                .EmptyStringIsNULL = True

                .NewQuery()

                .AddTables("t_lot_lotti_consultori")

                .AddInsertField("lcn_lot_codice", lottoMagazzino.CodiceLotto, DataTypes.Stringa)
                .AddInsertField("lcn_cns_codice", lottoMagazzino.CodiceConsultorio, DataTypes.Stringa)
                .AddInsertField("lcn_dosi_rimaste", lottoMagazzino.DosiRimaste, DataTypes.Numero)
                .AddInsertField("lcn_qta_minima", lottoMagazzino.QuantitaMinima, DataTypes.Numero)
                .AddInsertField("lcn_attivo", IIf(lottoMagazzino.Attivo, "S", "N"), DataTypes.Stringa)

                If Not lottoMagazzino.IdUtenteModificaFlagAttivo Is Nothing Then
                    .AddInsertField("lcn_ute_id_modifica_attivo", lottoMagazzino.IdUtenteModificaFlagAttivo, DataTypes.Numero)
                End If

                If Not lottoMagazzino.DataModificaFlagAttivo Is Nothing AndAlso lottoMagazzino.DataModificaFlagAttivo > DateTime.MinValue Then
                    .AddInsertField("lcn_data_modifica_attivo", lottoMagazzino.DataModificaFlagAttivo, DataTypes.DataOra)
                End If

                If lottoMagazzino.EtaMinimaAttivazione.HasValue Then
                    .AddInsertField("lcn_eta_min_attivazione", lottoMagazzino.EtaMinimaAttivazione.Value, DataTypes.Numero)
                End If

                If lottoMagazzino.EtaMassimaAttivazione.HasValue Then
                    .AddInsertField("lcn_eta_max_attivazione", lottoMagazzino.EtaMassimaAttivazione.Value, DataTypes.Numero)
                End If

            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Insert)

        End Function

#End Region

#Region " Update "

        ''' <summary>
        ''' Modifica i dati del lotto in anagrafica, da magazzino centrale
        ''' </summary>
        Public Function UpdateLottoMagazzinoCentrale(lottoMagazzino As Entities.LottoMagazzino) As Integer Implements ILottiProvider.UpdateLottoMagazzinoCentrale

            Return Me.UpdateLotto(lottoMagazzino, True)

        End Function

        ''' <summary>
        ''' Modifica i dati del lotto in anagrafica, da magazzino locale
        ''' </summary>
        Public Function UpdateLotto(lottoMagazzino As Entities.LottoMagazzino) As Integer Implements ILottiProvider.UpdateLotto

            Return Me.UpdateLotto(lottoMagazzino, False)

        End Function

        ''' <summary>
        ''' Modifica i dati del lotto in anagrafica
        ''' </summary>
        Private Function UpdateLotto(lottoMagazzino As Entities.LottoMagazzino, isMagazzinoCentrale As Boolean) As Integer

            With _DAM.QB

                .EmptyStringIsNULL = True

                .NewQuery()

                .AddTables("t_ana_lotti")

                .AddWhereCondition("lot_codice", Comparatori.Uguale, lottoMagazzino.CodiceLotto, DataTypes.Stringa)

                .AddUpdateField("lot_obsoleto", IIf(lottoMagazzino.Obsoleto, "S", "N"), DataTypes.Stringa)
                .AddUpdateField("lot_descrizione", lottoMagazzino.DescrizioneLotto, DataTypes.Stringa)
                .AddUpdateField("lot_data_scadenza", lottoMagazzino.DataScadenza, DataTypes.Data)
                .AddUpdateField("lot_ditta", lottoMagazzino.Ditta, DataTypes.Stringa)
                .AddUpdateField("lot_dosi_scatola", lottoMagazzino.DosiScatola, DataTypes.Numero)
                .AddUpdateField("lot_note", lottoMagazzino.Note, DataTypes.Stringa)
                .AddUpdateField("lot_noc_codice", lottoMagazzino.CodiceNomeCommerciale, DataTypes.Stringa)

                ' Gestione quantità minima solo se la modifica avviene da magazzino centrale
                If isMagazzinoCentrale Then
                    .AddUpdateField("lot_qta_minima_complessiva", lottoMagazzino.QuantitaMinima, DataTypes.Numero)
                End If

                If lottoMagazzino.DataPreparazione > DateTime.MinValue Then
                    .AddUpdateField("lot_data_preparazione", lottoMagazzino.DataPreparazione, DataTypes.Data)
                Else
                    .AddUpdateField("lot_data_preparazione", DBNull.Value, DataTypes.Replace)
                End If

                If Not lottoMagazzino.IdUtenteModificaFlagObsoleto Is Nothing Then
                    .AddUpdateField("lot_ute_id_modifica_obsoleto", lottoMagazzino.IdUtenteModificaFlagObsoleto, DataTypes.Numero)
                End If

                If Not lottoMagazzino.DataModificaFlagObsoleto Is Nothing AndAlso lottoMagazzino.DataModificaFlagObsoleto > Date.MinValue Then
                    .AddUpdateField("lot_data_modifica_obsoleto", lottoMagazzino.DataModificaFlagObsoleto, DataTypes.DataOra)
                End If

            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Update)

        End Function

        ''' <summary>
        ''' Modifica la quantità minima per il lotto nel consultorio specificato.
        ''' </summary>
        Public Function UpdateQuantitaMinimaLottoConsultorio(codiceLotto As String, codiceConsultorio As String, quantitaMinima As Integer) As Integer Implements ILottiProvider.UpdateQuantitaMinimaLottoConsultorio

            With _DAM.QB

                .NewQuery()

                .AddTables("t_lot_lotti_consultori")

                .AddUpdateField("lcn_qta_minima", quantitaMinima, DataTypes.Numero)

                .AddWhereCondition("lcn_lot_codice", Comparatori.Uguale, codiceLotto, DataTypes.Stringa)
                .AddWhereCondition("lcn_cns_codice", Comparatori.Uguale, codiceConsultorio, DataTypes.Stringa)

            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Update)

        End Function

        ''' <summary>
        ''' Modifica le dosi rimaste per il lotto nel consultorio specificato.
        ''' </summary>
        Public Function UpdateDosiRimasteLottoConsultorio(codiceLotto As String, codiceConsultorioMagazzino As String, numeroDosi As Integer) As Integer Implements ILottiProvider.UpdateDosiRimasteLottoConsultorio

            With _DAM.QB

                .NewQuery()

                .AddTables("t_lot_lotti_consultori")

                Dim valoreDosi As String = String.Format("{0} + {1}", .FC.IsNull("lcn_dosi_rimaste", "0", DataTypes.Numero), numeroDosi.ToString())

                .AddUpdateField("lcn_dosi_rimaste", valoreDosi, DataTypes.Replace)

                .AddWhereCondition("lcn_lot_codice", Comparatori.Uguale, codiceLotto, DataTypes.Stringa)
                .AddWhereCondition("lcn_cns_codice", Comparatori.Uguale, codiceConsultorioMagazzino, DataTypes.Stringa)

            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Update)

        End Function

        ''' <summary>
        ''' Modifica il flag di attivazione del lotto per il consultorio specificato. Se specificato, modifica anche i valori di età minima ed età massima.
        ''' </summary>
        Public Function UpdateLottoAttivo(codiceLotto As String, codiceConsultorio As String, attivo As Boolean, idUtenteModificaFlagAttivo As Integer?, dataModificaFlagAttivo As DateTime?,
                                          etaMinimaAttivazione As Integer?, etaMassimaAttivazione As Integer?, updateEta As Boolean) As Integer Implements ILottiProvider.UpdateLottoAttivo

            With _DAM.QB

                .NewQuery()

                .AddTables("t_lot_lotti_consultori")

                .AddUpdateField("lcn_attivo", IIf(attivo, "S", "N"), DataTypes.Stringa)

                If Not idUtenteModificaFlagAttivo Is Nothing Then
                    .AddUpdateField("lcn_ute_id_modifica_attivo", idUtenteModificaFlagAttivo, DataTypes.Numero)
                End If

                If Not dataModificaFlagAttivo Is Nothing AndAlso dataModificaFlagAttivo > DateTime.MinValue Then
                    .AddUpdateField("lcn_data_modifica_attivo", dataModificaFlagAttivo, DataTypes.DataOra)
                End If

                If updateEta Then

                    If etaMinimaAttivazione.HasValue Then
                        .AddUpdateField("lcn_eta_min_attivazione", etaMinimaAttivazione, DataTypes.Numero)
                    Else
                        .AddUpdateField("lcn_eta_min_attivazione", "NULL", DataTypes.Replace)
                    End If

                    If etaMassimaAttivazione.HasValue Then
                        .AddUpdateField("lcn_eta_max_attivazione", etaMassimaAttivazione, DataTypes.Numero)
                    Else
                        .AddUpdateField("lcn_eta_max_attivazione", "NULL", DataTypes.Replace)
                    End If

                End If

                .AddWhereCondition("lcn_cns_codice", Comparatori.Uguale, codiceConsultorio, DataTypes.Stringa)
                .AddWhereCondition("lcn_lot_codice", Comparatori.Uguale, codiceLotto, DataTypes.Stringa)

            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Update)

        End Function

        ''' <summary>
        ''' Disattiva il lotto su tutti i consultori a cui è associato
        ''' </summary>
        Public Sub DisattivaLotto(codiceLotto As String, idUtenteModificaFlagAttivo As Integer?, dataModificaFlagAttivo As DateTime?) Implements ILottiProvider.DisattivaLotto

            With _DAM.QB

                .NewQuery()

                .AddTables("t_lot_lotti_consultori")

                .AddWhereCondition("lcn_lot_codice", Comparatori.Uguale, codiceLotto, DataTypes.Stringa)

                .AddUpdateField("lcn_attivo", "N", DataTypes.Stringa)

                If Not idUtenteModificaFlagAttivo Is Nothing Then
                    .AddUpdateField("lcn_ute_id_modifica_attivo", idUtenteModificaFlagAttivo, DataTypes.Numero)
                End If

                If Not dataModificaFlagAttivo Is Nothing AndAlso dataModificaFlagAttivo > DateTime.MinValue Then
                    .AddUpdateField("lcn_data_modifica_attivo", dataModificaFlagAttivo, DataTypes.DataOra)
                End If

            End With

            _DAM.ExecNonQuery(ExecQueryType.Update)

        End Sub


#End Region

#End Region

#Region " Movimenti Lotto "

        Public Function LoadMovimentiLotto(codiceLotto As String, codiceConsultorioMagazzino As String, startRecordIndex As Integer, endRecordIndex As Integer) As List(Of Entities.MovimentoLotto) Implements ILottiProvider.LoadMovimentiLotto

            Dim listMovimenti As List(Of Entities.MovimentoLotto) = Nothing

            With _DAM.QB

                .NewQuery()

                .AddTables("t_lot_movimenti, t_ana_consultori, t_ana_lotti")

                .AddSelectFields("mma_progressivo, mma_lot_codice, mma_n_dosi, mma_tipo, mma_ute_id, mma_data_registrazione")
                .AddSelectFields("mma_cns_codice, mma_tra_cns_codice, cns_descrizione, mma_note, mma_ves_ass_prog")

                .AddWhereCondition("mma_lot_codice", Comparatori.Uguale, codiceLotto, DataTypes.Stringa)
                .AddWhereCondition("mma_cns_codice", Comparatori.Uguale, codiceConsultorioMagazzino, DataTypes.Stringa)
                .AddWhereCondition("mma_lot_codice", Comparatori.Uguale, "lot_codice", DataTypes.Join)
                .AddWhereCondition("mma_tra_cns_codice", Comparatori.Uguale, "cns_codice", DataTypes.OutJoinLeft)

                .AddOrderByFields("mma_data_registrazione DESC")

                .AddPaginatedOracleQuery(startRecordIndex, endRecordIndex)

            End With

            Using idr As IDataReader = _DAM.BuildDataReader()

                If Not idr Is Nothing Then

                    listMovimenti = New List(Of Entities.MovimentoLotto)()

                    Dim mma_progressivo As Integer = idr.GetOrdinal("mma_progressivo")
                    Dim mma_lot_codice As Integer = idr.GetOrdinal("mma_lot_codice")
                    Dim mma_n_dosi As Integer = idr.GetOrdinal("mma_n_dosi")
                    Dim mma_tipo As Integer = idr.GetOrdinal("mma_tipo")
                    Dim mma_ute_id As Integer = idr.GetOrdinal("mma_ute_id")
                    Dim mma_data_registrazione As Integer = idr.GetOrdinal("mma_data_registrazione")
                    Dim mma_cns_codice As Integer = idr.GetOrdinal("mma_cns_codice")
                    Dim mma_tra_cns_codice As Integer = idr.GetOrdinal("mma_tra_cns_codice")
                    Dim cns_descrizione As Integer = idr.GetOrdinal("cns_descrizione")
                    Dim mma_note As Integer = idr.GetOrdinal("mma_note")
                    Dim mma_ves_ass_prog As Integer = idr.GetOrdinal("mma_ves_ass_prog")

                    Dim movimento As Entities.MovimentoLotto = Nothing

                    While idr.Read()

                        movimento = New Entities.MovimentoLotto()

                        movimento.Progressivo = idr.GetStringOrDefault(mma_progressivo)
                        movimento.CodiceLotto = idr.GetStringOrDefault(mma_lot_codice)
                        movimento.NumeroDosi = idr.GetInt32OrDefault(mma_n_dosi)
                        movimento.TipoMovimento = idr.GetStringOrDefault(mma_tipo)
                        movimento.IdUtente = idr.GetInt32OrDefault(mma_ute_id)
                        movimento.DataRegistrazione = idr.GetDateTimeOrDefault(mma_data_registrazione)
                        movimento.CodiceConsultorio = idr.GetStringOrDefault(mma_cns_codice)
                        movimento.CodiceConsultorioTrasferimento = idr.GetStringOrDefault(mma_tra_cns_codice)
                        movimento.DescrizioneConsultorio = idr.GetStringOrDefault(cns_descrizione)
                        movimento.Note = idr.GetStringOrDefault(mma_note)
                        movimento.IdEsecuzioneAssociazione = idr.GetStringOrDefault(mma_ves_ass_prog)

                        listMovimenti.Add(movimento)

                    End While

                End If

            End Using

            Return listMovimenti

        End Function

        Public Function CountMovimentiLotto(codiceLotto As String, codiceConsultorioMagazzino As String) As Integer Implements ILottiProvider.CountMovimentiLotto

            With _DAM.QB

                .NewQuery()

                .AddTables("t_lot_movimenti, t_ana_consultori, t_ana_lotti")

                .AddSelectFields(.FC.Count("*"))

                .AddWhereCondition("mma_lot_codice", Comparatori.Uguale, codiceLotto, DataTypes.Stringa)
                .AddWhereCondition("mma_cns_codice", Comparatori.Uguale, codiceConsultorioMagazzino, DataTypes.Stringa)
                .AddWhereCondition("mma_lot_codice", Comparatori.Uguale, "lot_codice", DataTypes.Join)
                .AddWhereCondition("mma_tra_cns_codice", Comparatori.Uguale, "cns_codice", DataTypes.OutJoinLeft)

            End With

            Dim obj As Object = _DAM.ExecScalar()

            If obj Is Nothing OrElse obj Is DBNull.Value Then
                Return 0
            End If

            Return Convert.ToInt32(obj)

        End Function

#End Region

#Region " Salvataggio Movimenti "

        Public Function InsertMovimento(movimentoLotto As Entities.MovimentoLotto) As Integer Implements ILottiProvider.InsertMovimento

            With _DAM.QB

                .EmptyStringIsNULL = True

                .NewQuery()

                .AddTables("t_lot_movimenti")

                .AddInsertField("mma_progressivo", movimentoLotto.Progressivo, DataTypes.Stringa)
                .AddInsertField("mma_lot_codice", movimentoLotto.CodiceLotto, DataTypes.Stringa)
                .AddInsertField("mma_n_dosi", movimentoLotto.NumeroDosi, DataTypes.Numero)
                .AddInsertField("mma_tipo", movimentoLotto.TipoMovimento, DataTypes.Stringa)
                .AddInsertField("mma_cns_codice", movimentoLotto.CodiceConsultorio, DataTypes.Stringa)
                .AddInsertField("mma_data_registrazione", movimentoLotto.DataRegistrazione, DataTypes.DataOra)

                If String.IsNullOrEmpty(movimentoLotto.CodiceConsultorioTrasferimento) Then
                    .AddInsertField("mma_tra_cns_codice", DBNull.Value, DataTypes.Replace)
                Else
                    .AddInsertField("mma_tra_cns_codice", movimentoLotto.CodiceConsultorioTrasferimento, DataTypes.Stringa)
                End If

                If movimentoLotto.IdUtente <= 0 Then
                    .AddInsertField("mma_ute_id", DBNull.Value, DataTypes.Replace)
                Else
                    .AddInsertField("mma_ute_id", movimentoLotto.IdUtente, DataTypes.Numero)
                End If

                If String.IsNullOrEmpty(movimentoLotto.Note) Then
                    .AddInsertField("mma_note", DBNull.Value, DataTypes.Replace)
                Else
                    .AddInsertField("mma_note", movimentoLotto.Note, DataTypes.Stringa)
                End If

                If String.IsNullOrEmpty(movimentoLotto.IdEsecuzioneAssociazione) Then
                    .AddInsertField("mma_ves_ass_prog", DBNull.Value, DataTypes.Replace)
                Else
                    .AddInsertField("mma_ves_ass_prog", movimentoLotto.IdEsecuzioneAssociazione, DataTypes.Stringa)
                End If

            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Insert)

        End Function

#End Region

    End Class

End Namespace
