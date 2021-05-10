Imports System.Collections.Generic
Imports System.Data.OracleClient
Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.DataSet


Namespace DAL.Oracle

    Public Class DbAppuntamentiGiornoProvider
        Inherits DbProvider
        Implements IAppuntamentiGiornoProvider

#Region " Costruttori "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " Appuntamenti del Giorno (Con Bilanci) "

        'SELECT distinct paz_codice, PAZ_COGNOME, PAZ_NOME, PAZ_MED_CODICE_BASE, PAZ_COM_CODICE_RESIDENZA, PAZ_LIBERO_1, PAZ_LIBERO_2, PAZ_LIBERO_3, 
        '                PAZ_DATA_NASCITA, CNV_DATA, 
        '                DECODE((SELECT count(*) FROM t_ana_cittadinanze WHERE cit_codice = p1.paz_cit_codice 
        '                         and ( cit_cee = 'N' or cit_cee is null ) ),1,'', 
        '                         DECODE((SELECT COUNT(t1.paz_codice) FROM T_PAZ_PAZIENTI t1
        '                                 WHERE t1.paz_codice = p1.paz_codice and ( (  not exists (select 1 from t_ana_link_comuni_consultori
        '                                                                                          where t1.paz_com_comune_provenienza = cco_com_codice ) 
        '                                                                              and t1.paz_com_comune_provenienza not like '999%' ) 
        '                                                                            or exists (select 1 from t_ana_cittadinanze 
        '                                                                                       where t1.paz_com_comune_provenienza like '999%' 
        '                                                                                       and cit_cee = 'S' 
        '                                                                                       and substr(t1.paz_com_comune_provenienza,4) = cit_codice
        '                                                                                      ) ) and  not exists (select distinct 1
        '                                                                                                           from t_vac_eseguite
        '                                                                                                           where ves_cns_codice is not null 
        '                                                                                                           and ves_paz_codice = p1.paz_codice) 
        '                                ),1,'I','')) TIPO_IMMI_NON_EXTRA_PRIMA, 
        '                         DECODE((SELECT count(*) FROM t_ana_cittadinanze WHERE cit_codice = p1.paz_cit_codice 
        '                                 and ( cit_cee = 'N' or cit_cee is null )),1,'E','') TIPO_EXTRACOMUNITARI , 
        '                                 DECODE((SELECT distinct 1 FROM t_paz_malattie WHERE pma_paz_codice = p1.paz_codice 
        '                                         and pma_mal_codice <> '0'),1,'C','') CRONICO,
        '                CNV_PAZ_CODICE, CNV_DATA_INVIO, CNV_DATA_APPUNTAMENTO, COM_CODICE, COM_DESCRIZIONE, MED_CODICE, MED_DESCRIZIONE,
        '                VPR_PAZ_CODICE, VPR_CNV_DATA, VPR_VAC_CODICE, VPR_N_RICHIAMO, VEX_PAZ_CODICE,
        '                VES_PAZ_CODICE, (select max(ves_data_effettuazione) from t_vac_eseguite e1 
        '                                 where paz_codice = e1.ves_paz_codice) VES_DATA_EFFETTUAZIONE,
        '                VAC_DESCRIZIONE, VRA_PAZ_CODICE, VRA_VAC_CODICE, VRA_N_RICHIAMO, VRA_DATA_REAZIONE,
        '                ( Select distinct Max (cnc_n_sollecito) cnc_n_sollecito from t_cnv_cicli
        '                    where c1.cnv_paz_codice = cnc_cnv_paz_codice
        '                    and c1.cnv_data = cnc_cnv_data
        '                ) CNC_N_SOLLECITO,
        '                PRI_PAZ_CODICE, PRI_DATA_APPUNTAMENTO1, PRI_DATA_APPUNTAMENTO2, 
        '                PRI_DATA_APPUNTAMENTO3, PRI_DATA_APPUNTAMENTO4, bip_mal_codice, bip_bil_numero, bip_stato

        ' FROM T_CNV_CONVOCAZIONI c1, T_PAZ_PAZIENTI p1,  t_ana_comuni, t_ana_medici, t_vac_programmate, T_VAC_ESCLUSE,  
        '      T_VAC_ESEGUITE, T_ANA_VACCINAZIONI, T_VAC_REAZIONI_AVVERSE, T_PAZ_RITARDI, T_BIL_PROGRAMMATI

        ' WHERE cnv_cns_codice = '01' 
        ' and cnv_data_appuntamento >= to_date('10/01/2008 0.00','dd/MM/yyyy HH24.mi') 
        ' and cnv_data_appuntamento <= to_date('31/01/2008 23.59','dd/MM/yyyy HH24.mi') 
        ' and ( (  exists  (select 1 from t_vac_programmate where vpr_paz_codice = cnv_paz_codice and vpr_cnv_data = cnv_data ) and cnv_cns_codice = '01' ) 
        '       or (  not exists  (select 1 from t_vac_programmate where vpr_paz_codice = cnv_paz_codice and vpr_cnv_data = cnv_data )  
        '             and cnv_durata_appuntamento <> 0 ) ) 
        '
        ' -- Filtro pazienti avvisati (filtroPazAvvisati = Biz.BizAppuntamentiGiorno.FiltroAvvisati.SoloAvvisati)
        ' and cnv_data_invio is not null
        '
        ' -- Filtro pazienti non avvisati (filtroPazAvvisati = Biz.BizAppuntamentiGiorno.FiltroAvvisati.SoloNonAvvisati)
        ' and cnv_data_invio is null
        '
        ' -- Filtro associazioni-dosi (solo se specificato)
        'and exists (
        '  select 1
        '  from t_vac_programmate v
        '  where paz_codice = v.vpr_paz_codice
        '  and cnv_data = v.vpr_cnv_data
        '  and (
        '       (vpr_ass_codice = 'HIB' and vpr_n_richiamo = 1)
        '    or (vpr_ass_codice = 'HIB' and vpr_n_richiamo = 2)
        '    or (vpr_ass_codice = 'PCV13' and vpr_n_richiamo = 1)
        '    or (vpr_ass_codice = 'PCV13' and vpr_n_richiamo = 2)
        '  )
        ')
        '
        'and CNV_PAZ_CODICE = PAZ_CODICE
        'and paz_com_codice_residenza = com_codice (+)
        'and paz_med_codice_base = med_codice (+)
        'and cnv_paz_codice = vpr_paz_codice (+)
        'and cnv_data = vpr_cnv_data (+)
        'and vpr_paz_codice = vex_paz_codice (+)
        'and vpr_vac_codice = vex_vac_codice (+)
        'and vpr_paz_codice = ves_paz_codice (+)
        'and vpr_vac_codice = ves_vac_codice (+)
        'and vpr_n_richiamo = ves_n_richiamo (+)
        'and vpr_vac_codice = vac_codice (+)
        'and vpr_paz_codice = vra_paz_codice (+)
        'and vpr_vac_codice = vra_vac_codice (+)
        'and cnv_paz_codice = pri_paz_codice (+)
        'and cnv_data = pri_cnv_data (+)
        'and cnv_paz_codice = bip_paz_codice (+)
        'and cnv_data = bip_cnv_data (+)

        'ORDER BY cnv_data_appuntamento, paz_codice

        Private Function GetNotaPaziente(qb As AbstractQB, codiceUsl As String, codiceTipoNota As String) As String

            With qb
                .NewQuery(False, False)
                .AddSelectFields("pno_testo_note")
                .AddTables("t_ana_tipo_note, t_paz_note")
                .AddWhereCondition("tno_codice", Comparatori.Uguale, "pno_tno_codice", DataTypes.Join)
                .AddWhereCondition("pno_paz_codice", Comparatori.Uguale, "paz_codice", DataTypes.Join)
                .OpenParanthesis()
                .AddWhereCondition("pno_azi_codice", Comparatori.Uguale, codiceUsl, DataTypes.Stringa)
                .AddWhereCondition("pno_azi_codice", Comparatori.In, String.Format("select dis_codice from t_ana_distretti where dis_usl_codice = '{0}'", codiceUsl), DataTypes.Replace, "OR")
                .CloseParanthesis()
                .AddWhereCondition("pno_tno_codice", Comparatori.Uguale, codiceTipoNota, DataTypes.Stringa)
            End With
            Return qb.GetSelect()

        End Function

        Public Function BuildDataSetAppuntamentiGiornoBilanci(codiceConsultorio As String, codiceAmbulatorio As Integer, codiceUsl As String, strDataInizio As String, strDataFine As String, filtroPazientiAvvisati As OnVac.Enumerators.FiltroAvvisati, filtroAssociazioniDosi As Entities.FiltroComposto) As AppuntamentiGiornoBilanci Implements IAppuntamentiGiornoProvider.BuildDataSetAppuntamentiGiornoBilanci
            '--
            Dim dsAppuntamenti As New AppuntamentiGiornoBilanci()
            Dim dtRitardi As New DataTable()
            '--
            Try
                '--
                'query per escludere i pazienti con solo bilancio [modifica 04/02/2005]
                '--
                With _DAM.QB
                    .NewQuery()
                    .AddSelectFields("1")
                    .AddTables("T_VAC_PROGRAMMATE")
                    .AddWhereCondition("VPR_PAZ_CODICE", Comparatori.Uguale, "CNV_PAZ_CODICE", DataTypes.Join)
                    .AddWhereCondition("VPR_CNV_DATA", Comparatori.Uguale, "CNV_DATA", DataTypes.Join)
                End With
                '--
                Dim querySoloBilancio As String = _DAM.QB.GetSelect
                '--
                'Ritardi
                '--
                With _DAM.QB
                    '--
                    .NewQuery()
                    .AddTables("T_PAZ_RITARDI", "T_CNV_CONVOCAZIONI")
                    .AddSelectFields("T_PAZ_RITARDI.*")
                    .AddWhereCondition("PRI_CNV_DATA", Comparatori.Uguale, "CNV_DATA", DataTypes.Join)
                    '--
                End With
                '--
                AddConvocazioniWhereConditions(_DAM, codiceConsultorio, strDataInizio, strDataFine, codiceAmbulatorio, filtroPazientiAvvisati, querySoloBilancio)
                '--
                _DAM.BuildDataTable(dtRitardi)
                '--

                ' --------- Query Immigrati [CMR 12/02/07] --------- '
                Dim strQueryComuniUsl As String = ""
                Dim strQueryComuniStatoComunitario As String = ""
                Dim strQueryVacEseguitePaz As String = ""
                ' Filtro per non extracomunitari prima volta
                Dim strQueryImmigratiNonPrimaVolta As String = ""

                ' --- Sottoquery per comune di provenienza associato all'ausl --- '
                With _DAM.QB
                    .NewQuery()
                    .AddSelectFields("1")
                    .AddTables("t_ana_link_comuni_consultori")
                    .AddWhereCondition("t1.paz_com_comune_provenienza", Comparatori.Uguale, "cco_com_codice", DataTypes.Join)
                End With
                strQueryComuniUsl = _DAM.QB.GetSelect()
                ' --------------------------------------------------------------- '

                ' --- Sottoquery per comune di provenienza = stato comunitario --- '
                With _DAM.QB
                    .NewQuery(False, False)
                    .AddSelectFields("1")
                    .AddTables("t_ana_cittadinanze")
                    .AddWhereCondition("t1.paz_com_comune_provenienza", Comparatori.Like, "999%", DataTypes.Stringa)
                    .AddWhereCondition("cit_cee", Comparatori.Uguale, "S", DataTypes.Stringa)
                    .AddWhereCondition("SUBSTR(t1.paz_com_comune_provenienza,4)", Comparatori.Uguale, "cit_codice", DataTypes.Replace)
                End With
                strQueryComuniStatoComunitario = _DAM.QB.GetSelect
                ' ---------------------------------------------------------------- '

                ' --- Sottoquery per paziente con vaccinazioni già eseguite --- '
                With _DAM.QB
                    .NewQuery(False, False)
                    .AddSelectFields("distinct 1")
                    .AddTables("t_vac_eseguite")
                    .AddWhereCondition("ves_cns_codice", Comparatori.[IsNot], "NULL", DataTypes.Replace)
                    .AddWhereCondition("ves_paz_codice", Comparatori.Uguale, "p1.paz_codice", DataTypes.Join)
                End With
                strQueryVacEseguitePaz = _DAM.QB.GetSelect
                ' ------------------------------------------------------------- '

                With _DAM.QB
                    .NewQuery(False, False)
                    .AddSelectFields("COUNT(t1.paz_codice)")
                    .AddTables("T_PAZ_PAZIENTI t1")
                    .AddWhereCondition("t1.paz_codice", Comparatori.Uguale, "p1.paz_codice", DataTypes.Join)
                    .OpenParanthesis()
                    .OpenParanthesis()
                    .AddWhereCondition("", Comparatori.NotExist, "(" + strQueryComuniUsl + ")", DataTypes.Replace)
                    .AddWhereCondition("t1.paz_com_comune_provenienza", Comparatori.NotLike, "999%", DataTypes.Stringa)
                    .CloseParanthesis()
                    .AddWhereCondition("", Comparatori.Exist, "(" + strQueryComuniStatoComunitario + ")", DataTypes.Replace, "OR")
                    .CloseParanthesis()
                    .AddWhereCondition("", Comparatori.NotExist, "(" + strQueryVacEseguitePaz + ")", DataTypes.Replace)
                End With
                strQueryImmigratiNonPrimaVolta = _DAM.QB.GetSelect

                ' --- Sottoquery per comune provenienza paziente = stato extracomunitario --- '
                Dim strQueryComuniStatoExtracom As String = ""
                With _DAM.QB
                    .NewQuery(False, False)
                    .AddSelectFields("count(*)")
                    .AddTables("t_ana_cittadinanze")
                    .AddWhereCondition("cit_codice", Comparatori.Uguale, "p1.paz_cit_codice", DataTypes.Join)
                    .OpenParanthesis()
                    .AddWhereCondition("cit_cee", Comparatori.Uguale, "N", DataTypes.Stringa)
                    .AddWhereCondition("cit_cee", Comparatori.Is, "NULL", DataTypes.Replace, "OR")
                    .CloseParanthesis()
                End With
                strQueryComuniStatoExtracom = _DAM.QB.GetSelect()
                ' --------------------------------------------------------------------------- '

                ' Sottoquery per determinare se il paziente ha una malattia cronica
                Dim qryCronico As String
                With _DAM.QB
                    .NewQuery(False, False)
                    .AddSelectFields("distinct 1")
                    .AddTables("t_paz_malattie")
                    .AddWhereCondition("pma_paz_codice", Comparatori.Uguale, "p1.paz_codice", DataTypes.Join)
                    .AddWhereCondition("pma_mal_codice", Comparatori.Diverso, "0", DataTypes.Stringa)
                End With
                qryCronico = "(" & _DAM.QB.GetSelect() & ")"
                ' --

                ' ------------------ Sottoquery Numero Sollecito ------------------ '
                With _DAM.QB
                    .NewQuery(False, False)
                    .IsDistinct = True
                    .AddSelectFields("NVL(MAX(cnc_n_sollecito), 0) cnc_n_sollecito")
                    .AddTables("t_cnv_cicli")
                    .AddWhereCondition("c1.cnv_paz_codice", Comparatori.Uguale, "cnc_cnv_paz_codice", DataTypes.Join)
                    .AddWhereCondition("c1.cnv_data", Comparatori.Uguale, "cnc_cnv_data", DataTypes.Join)
                End With
                Dim strQueryMaxNumSollecito As String = String.Format("({0})", _DAM.QB.GetSelect)
                ' ----------------------------------------------------------------- '

                ' ------------------ Sottoquery Ultima Data Vaccinazione Eseguita ------------------ '
                ' select max(ves_data_effettuazione)
                ' from t_vac_eseguite e1
                ' where p1.paz_codice = e1.ves_paz_codice
                With _DAM.QB
                    .NewQuery(False, False)
                    .AddSelectFields("MAX(ves_data_effettuazione)")
                    .AddTables("t_vac_eseguite e1")
                    .AddWhereCondition("p1.paz_codice", Comparatori.Uguale, "e1.ves_paz_codice", DataTypes.Join)
                End With
                Dim strQueryMaxDataVac As String = String.Format("({0})", _DAM.QB.GetSelect())
                ' ---------------------------------------------------------------------------------- '

                ' TODO [postel filtro ass-dosi]

                ' ------------------ Sottoquery Filtro Associazioni Dosi ------------------ '
                Dim queryAssociazioniDosi As String = String.Empty

                If Not filtroAssociazioniDosi Is Nothing AndAlso
                   (Not filtroAssociazioniDosi.CodiceValore.IsNullOrEmpty() OrElse Not filtroAssociazioniDosi.Valori.IsNullOrEmpty()) Then

                    'and exists (
                    '  select 1
                    '  from t_vac_programmate v
                    '  where paz_codice = v.vpr_paz_codice
                    '  and cnv_data = v.vpr_cnv_data
                    '  and (
                    '       (vpr_ass_codice = 'HIB' and vpr_n_richiamo = 1)
                    '    or (vpr_ass_codice = 'HIB' and vpr_n_richiamo = 2)
                    '    or (vpr_ass_codice = 'PCV13' and vpr_n_richiamo = 1)
                    '    or (vpr_ass_codice = 'PCV13' and vpr_n_richiamo = 2)
                    '  )
                    ')
                    With _DAM.QB
                        .NewQuery(False, False)
                        .AddSelectFields("1")
                        .AddTables("t_vac_programmate v")
                        .AddWhereCondition("paz_codice", Comparatori.Uguale, "v.vpr_paz_codice", DataTypes.Join)
                        .AddWhereCondition("cnv_data", Comparatori.Uguale, "v.vpr_cnv_data", DataTypes.Join)

                        ' Associazioni-dosi
                        If Not filtroAssociazioniDosi.CodiceValore.IsNullOrEmpty() Then

                            ' Date le coppie (associazione; lista dosi), crea una lista (associazione; dose), (associazione; dose), ...
                            Dim listAssociazioneDose As List(Of KeyValuePair(Of String, String)) = CreateListAssociazioneDose(filtroAssociazioniDosi.CodiceValore)

                            .OpenParanthesis()

                            For i As Integer = 0 To listAssociazioneDose.Count - 1

                                Dim pair As KeyValuePair(Of String, String) = listAssociazioneDose(i)

                                .OpenParanthesis()

                                .AddWhereCondition("vpr_ass_codice", Comparatori.Uguale, pair.Key, DataTypes.Stringa, IIf(i = 0, "AND", "OR"))

                                If Not String.IsNullOrWhiteSpace(pair.Value) Then
                                    .AddWhereCondition("vpr_n_richiamo", Comparatori.Uguale, pair.Value, DataTypes.Numero)
                                End If

                                .CloseParanthesis()

                            Next

                            .CloseParanthesis()

                        End If

                        ' Solo dosi
                        If Not filtroAssociazioniDosi.Valori.IsNullOrEmpty() Then

                            .OpenParanthesis()

                            For i As Integer = 0 To filtroAssociazioniDosi.Valori.Count - 1

                                .OpenParanthesis()

                                Dim dose As String = filtroAssociazioniDosi.Valori(i)

                                .AddWhereCondition("vpr_n_richiamo", Comparatori.Uguale, dose, DataTypes.Numero, IIf(i = 0, "AND", "OR"))

                                .CloseParanthesis()

                            Next

                            .CloseParanthesis()

                        End If

                    End With

                    queryAssociazioniDosi = String.Format("({0})", _DAM.QB.GetSelect())

                End If
                ' ------------------------------------------------------------------------- '

                ' sottoquery per le note
                Dim queryNotaLibero1 As String = GetNotaPaziente(_DAM.QB, codiceUsl, Constants.CodiceTipoNotaPaziente.Appuntamenti)
                Dim queryNotaLibero2 As String = GetNotaPaziente(_DAM.QB, codiceUsl, Constants.CodiceTipoNotaPaziente.MalattiePregresse)
                Dim queryNotaLibero3 As String = GetNotaPaziente(_DAM.QB, codiceUsl, Constants.CodiceTipoNotaPaziente.Esclusioni)
                Dim queryNotaSolleciti As String = GetNotaPaziente(_DAM.QB, codiceUsl, Constants.CodiceTipoNotaPaziente.Solleciti)

                ' ------------------------------------------------------------------------- '

                ' Query esterna
                With _DAM.QB
                    '
                    .NewQuery(False, False)

                    ' --- Distinct --- '
                    .IsDistinct = True

                    ' --- Tabelle --- '
                    .AddTables("T_CNV_CONVOCAZIONI c1, T_PAZ_PAZIENTI p1")
                    .AddTables("T_ANA_COMUNI, T_ANA_MEDICI, T_VAC_PROGRAMMATE, T_VAC_ESCLUSE")
                    .AddTables("T_VAC_ESEGUITE, T_ANA_VACCINAZIONI, T_VAC_REAZIONI_AVVERSE")
                    .AddTables("T_CNV_CICLI")
                    .AddTables("T_BIL_PROGRAMMATI")
                    .AddTables("T_ANA_AMBULATORI")
                    .AddTables("T_ANA_ASSOCIAZIONI")

                    ' --- Select --- '
                    .AddSelectFields("PAZ_CODICE, PAZ_DATA_NASCITA, CNV_DATA")
                    .AddSelectFields("PAZ_COGNOME, PAZ_NOME, PAZ_MED_CODICE_BASE, PAZ_COM_CODICE_RESIDENZA")

                    .AddSelectFields("(" + queryNotaLibero1 + ") PAZ_LIBERO_1")
                    .AddSelectFields("(" + queryNotaLibero2 + ") PAZ_LIBERO_2")
                    .AddSelectFields("(" + queryNotaLibero3 + ") PAZ_LIBERO_3")
                    .AddSelectFields("(" + queryNotaSolleciti + ") PAZ_NOTE_SOLLECITI")

                    .AddSelectFields("DECODE((" & strQueryComuniStatoExtracom & "),1,'',DECODE((" & strQueryImmigratiNonPrimaVolta & "),1,'I','')) TIPO_IMMI_NON_EXTRA_PRIMA")
                    .AddSelectFields("DECODE((" & strQueryComuniStatoExtracom & "),1,'E','') TIPO_EXTRACOMUNITARI ")
                    .AddSelectFields("DECODE(" & qryCronico & ",1,'C','') CRONICO")
                    '
                    .AddSelectFields("CNV_PAZ_CODICE, CNV_DATA_INVIO, CNV_DATA_APPUNTAMENTO,CNV_AMB_CODICE")
                    .AddSelectFields("AMB_DESCRIZIONE")
                    .AddSelectFields("COM_CODICE, COM_DESCRIZIONE, MED_CODICE, MED_DESCRIZIONE")
                    .AddSelectFields("VPR_PAZ_CODICE, VPR_CNV_DATA, VPR_VAC_CODICE, VPR_N_RICHIAMO")
                    .AddSelectFields("VEX_PAZ_CODICE, VES_PAZ_CODICE")
                    '
                    ' Data Ultima vaccinazione
                    .AddSelectFields(strQueryMaxDataVac & " VES_DATA_EFFETTUAZIONE")
                    '
                    .AddSelectFields("VAC_DESCRIZIONE, VRA_PAZ_CODICE, VRA_VAC_CODICE, VRA_N_RICHIAMO, VRA_DATA_REAZIONE", "CNC_CIC_CODICE", "CNC_SED_N_SEDUTA")
                    '
                    ' Numero solleciti
                    .AddSelectFields(strQueryMaxNumSollecito & " MAX_SOLLECITO")
                    '
                    .AddSelectFields("BIP_MAL_CODICE, BIP_BIL_NUMERO, BIP_STATO")
                    .AddSelectFields("PAZ_TELEFONO_1, PAZ_TELEFONO_2, PAZ_TELEFONO_3")
                    '
                    .AddSelectFields("ASS_CODICE, ASS_DESCRIZIONE, ASS_STAMPA")

                    ' --- Join --- '
                    ' convocazioni - pazienti
                    .AddWhereCondition("cnv_paz_codice", Comparatori.Uguale, "paz_codice", DataTypes.Join)
                    ' pazienti - comuni
                    .AddWhereCondition("paz_com_codice_residenza", Comparatori.Uguale, "com_codice", DataTypes.OutJoinLeft)
                    ' pazienti - medici
                    .AddWhereCondition("paz_med_codice_base", Comparatori.Uguale, "med_codice", DataTypes.OutJoinLeft)
                    ' convocazioni - programmate
                    .AddWhereCondition("cnv_paz_codice", Comparatori.Uguale, "vpr_paz_codice", DataTypes.OutJoinLeft)
                    .AddWhereCondition("cnv_data", Comparatori.Uguale, "vpr_cnv_data", DataTypes.OutJoinLeft)
                    ' programmate - escluse
                    .AddWhereCondition("vpr_paz_codice", Comparatori.Uguale, "vex_paz_codice", DataTypes.OutJoinLeft)
                    .AddWhereCondition("vpr_vac_codice", Comparatori.Uguale, "vex_vac_codice", DataTypes.OutJoinLeft)
                    ' programmate - eseguite
                    .AddWhereCondition("vpr_paz_codice", Comparatori.Uguale, "ves_paz_codice", DataTypes.OutJoinLeft)
                    .AddWhereCondition("vpr_vac_codice", Comparatori.Uguale, "ves_vac_codice", DataTypes.OutJoinLeft)
                    .AddWhereCondition("vpr_n_richiamo", Comparatori.Uguale, "ves_n_richiamo", DataTypes.OutJoinLeft)
                    ' programmate - vaccinazioni
                    .AddWhereCondition("vpr_vac_codice", Comparatori.Uguale, "vac_codice", DataTypes.OutJoinLeft)
                    ' programmate - convocazioni
                    .AddWhereCondition("vpr_paz_codice", Comparatori.Uguale, "vra_paz_codice", DataTypes.OutJoinLeft)
                    .AddWhereCondition("vpr_vac_codice", Comparatori.Uguale, "vra_vac_codice", DataTypes.OutJoinLeft)
                    ' programmate - cicli
                    .AddWhereCondition("vpr_paz_codice", Comparatori.Uguale, "cnc_cnv_paz_codice", DataTypes.OutJoinLeft)
                    .AddWhereCondition("vpr_cnv_data", Comparatori.Uguale, "cnc_cnv_data", DataTypes.OutJoinLeft)
                    .AddWhereCondition("vpr_cic_codice", Comparatori.Uguale, "cnc_cic_codice", DataTypes.OutJoinLeft)
                    .AddWhereCondition("vpr_n_seduta", Comparatori.Uguale, "cnc_sed_n_seduta", DataTypes.OutJoinLeft)
                    ' convocazioni - bilanci
                    .AddWhereCondition("cnv_paz_codice", Comparatori.Uguale, "bip_paz_codice", DataTypes.OutJoinLeft)
                    .AddWhereCondition("cnv_data", Comparatori.Uguale, "bip_cnv_data", DataTypes.OutJoinLeft)
                    ' convocazioni - ambulatori
                    .AddWhereCondition("cnv_amb_codice", Comparatori.Uguale, "amb_codice", DataTypes.OutJoinLeft)
                    ' programmate - associazioni
                    .AddWhereCondition("vpr_ass_codice", Comparatori.Uguale, "ass_codice", DataTypes.OutJoinLeft)

                    ' --- Filtri --- '
                    AddConvocazioniWhereConditions(_DAM, codiceConsultorio, strDataInizio, strDataFine, codiceAmbulatorio, filtroPazientiAvvisati, querySoloBilancio)

                    '--- MGR 20/07/2009
                    '--- dopo attenta riflessione aggiungo filtro sul tipo malattia per i bilanci = 0 e bilanci in stato da eseguire
                    '--- non verranno mai stampati gli appuntamenti per bilanci malattia, x qs occorre l'elenco bilanci malattia cronica
                    .OpenParanthesis()
                    .AddWhereCondition("bip_mal_codice", Comparatori.Uguale, "0", DataTypes.Stringa)
                    .AddWhereCondition("bip_mal_codice", Comparatori.Is, "NULL", DataTypes.Replace, "Or")
                    .AddWhereCondition("vpr_vac_codice", Comparatori.IsNot, "NULL", DataTypes.Replace, "Or")
                    .CloseParanthesis()
                    '--- fine MGR

                    ' Filtro per eliminare dai risultati le vaccinazioni escluse senza scadenza (hanno vex_paz_codice nullo, perchè siamo in outerjoin)
                    ' e le escluse con una data di scadenza superiore all'appuntamento (è compreso anche il caso di esclusa senza scadenza)
                    .OpenParanthesis()
                    .AddWhereCondition("vex_paz_codice", Comparatori.Is, "NULL", DataTypes.Replace)
                    .AddWhereCondition("vex_data_scadenza", Comparatori.MinoreUguale, "cnv_data_appuntamento", DataTypes.Replace, "Or")
                    .CloseParanthesis()

                    ' Filtro associazioni-dosi (solo le cnv contenenti associazioni o dosi specificate)
                    If Not String.IsNullOrWhiteSpace(queryAssociazioniDosi) Then
                        .AddWhereCondition(String.Empty, Comparatori.Exist, queryAssociazioniDosi, DataTypes.Replace)
                    End If

                    ' --- Order by --- '
                    .AddOrderByFields("cnv_data_appuntamento, paz_codice")

                End With

                _DAM.BuildDataTable(dsAppuntamenti.APPUNTAMENTI_GIORNO_BIL)

            Catch ex As Exception

                dsAppuntamenti = Nothing
                ex.InternalPreserveStackTrace()
                Throw

            End Try
            '--
            Dim ritardiConvocazioneDictionary As New Dictionary(Of String, Dictionary(Of DateTime, String))
            '--
            For Each drAppGiornoBil As AppuntamentiGiornoBilanci.APPUNTAMENTI_GIORNO_BILRow In dsAppuntamenti.APPUNTAMENTI_GIORNO_BIL
                '--
                Dim codicePaziente As String = drAppGiornoBil.CNV_PAZ_CODICE
                Dim dataConvocazione As String = drAppGiornoBil.CNV_DATA
                '--
                Dim dateAppuntamentiSolleciti As System.Text.StringBuilder = Nothing
                '--
                If ritardiConvocazioneDictionary.ContainsKey(codicePaziente) Then
                    '--
                    Dim ritardiConvocazionePazienteDictionary As Dictionary(Of Date, String) = ritardiConvocazioneDictionary(codicePaziente)
                    '--
                    If ritardiConvocazionePazienteDictionary.ContainsKey(dataConvocazione) Then
                        '--
                        dateAppuntamentiSolleciti = New System.Text.StringBuilder(ritardiConvocazionePazienteDictionary(dataConvocazione))
                        '--
                    End If
                    '--
                End If
                '--
                If dateAppuntamentiSolleciti Is Nothing Then
                    '--
                    dateAppuntamentiSolleciti = New System.Text.StringBuilder()
                    '--
                    Dim maxSollecito As Int32 = drAppGiornoBil.MAX_SOLLECITO
                    '--
                    Dim drRitardiList As List(Of DataRow) = dtRitardi.AsEnumerable.Where(Function(drRitardo) drRitardo("PRI_PAZ_CODICE") = codicePaziente AndAlso drRitardo("PRI_CNV_DATA") = dataConvocazione).ToList ' AndAlso drRitardo("PRI_CIC_CODICE") = codiceCiclo AndAlso drRitardo("PRI_SED_N_SEDUTA") = numeroSeduta).ToList
                    '--
                    Dim dicDateApp1 As New Dictionary(Of String, List(Of String))
                    Dim dicDateApp2 As New Dictionary(Of String, List(Of String))
                    Dim dicDateApp3 As New Dictionary(Of String, List(Of String))
                    Dim dicDateApp4 As New Dictionary(Of String, List(Of String))
                    '--
                    For Each drRitardo As DataRow In drRitardiList
                        '--
                        addDateToList(drRitardo, 1, dicDateApp1)
                        addDateToList(drRitardo, 2, dicDateApp2)
                        addDateToList(drRitardo, 3, dicDateApp3)
                        addDateToList(drRitardo, 4, dicDateApp4)
                        '--
                    Next
                    '--
                    ' Label per avviso
                    '--
                    If maxSollecito >= 1 Then
                        dateAppuntamentiSolleciti.Append(BuildMessageLabelRitardo(maxSollecito, dicDateApp1))
                    End If
                    '--
                    ' Label per primo sollecito
                    '--
                    If maxSollecito >= 2 Then
                        If dateAppuntamentiSolleciti.Length > 0 Then dateAppuntamentiSolleciti.Append(Environment.NewLine)
                        dateAppuntamentiSolleciti.Append(BuildMessageLabelRitardo(maxSollecito, dicDateApp2))
                    End If
                    '--
                    ' Label per secondo sollecito
                    '--
                    If maxSollecito >= 3 Then
                        If dateAppuntamentiSolleciti.Length > 0 Then dateAppuntamentiSolleciti.Append(Environment.NewLine)
                        dateAppuntamentiSolleciti.Append(BuildMessageLabelRitardo(maxSollecito, dicDateApp3))
                    End If
                    '--
                    ' Label per terzo sollecito
                    '--
                    If maxSollecito = 4 Then
                        If dateAppuntamentiSolleciti.Length > 0 Then dateAppuntamentiSolleciti.Append(Environment.NewLine)
                        dateAppuntamentiSolleciti.Append(BuildMessageLabelRitardo(maxSollecito, dicDateApp4))
                    End If
                    '--
                    If Not ritardiConvocazioneDictionary.ContainsKey(codicePaziente) Then
                        ritardiConvocazioneDictionary.Add(codicePaziente, New Dictionary(Of Date, String))
                    End If
                    '--
                    ritardiConvocazioneDictionary(codicePaziente).Add(dataConvocazione, dateAppuntamentiSolleciti.ToString)
                    '--
                End If
                '--
                drAppGiornoBil.DATE_APPUNTAMENTO_SOLLECITI = dateAppuntamentiSolleciti.ToString
                '--
            Next
            '--
            Return dsAppuntamenti
            '--
        End Function

        Private Function CreateListAssociazioneDose(listCodiciValori As List(Of KeyValuePair(Of String, String))) As List(Of KeyValuePair(Of String, String))

            Dim list As New List(Of KeyValuePair(Of String, String))()

            For Each pair As KeyValuePair(Of String, String) In listCodiciValori

                If String.IsNullOrWhiteSpace(pair.Value) Then

                    list.Add(New KeyValuePair(Of String, String)(pair.Key, String.Empty))

                Else

                    Dim values As String() = pair.Value.Split(",")
                    For Each value As String In values

                        list.Add(New KeyValuePair(Of String, String)(pair.Key, value))

                    Next

                End If

            Next

            Return list

        End Function

        ' La data, in formato stringa, viene aggiunta all'elenco di date solo se non è già presente.
        Private Sub addDateToList(drRitardo As DataRow, dataAppIndex As Integer, dicDateApp As Dictionary(Of String, List(Of String)))

            Dim dataAppColumnName As String = String.Format("PRI_DATA_APPUNTAMENTO{0}", dataAppIndex)

            If Not drRitardo(dataAppColumnName) Is DBNull.Value Then

                Dim dateToAdd As String = DirectCast(drRitardo(dataAppColumnName), Date).ToString("dd/MM/yyyy")


                Dim item As String = (From d As String In dicDateApp.Keys
                                      Where d = dateToAdd
                                      Select d).FirstOrDefault()

                If String.IsNullOrEmpty(item) Then
                    dicDateApp.Add(dateToAdd, New List(Of String))
                End If

                Dim cicloToAdd As String = String.Format("{0} [{1}]", drRitardo("PRI_CIC_CODICE"), drRitardo("PRI_SED_N_SEDUTA"))

                item = (From c As String In dicDateApp(dateToAdd)
                        Where c = cicloToAdd
                        Select c).FirstOrDefault()

                If String.IsNullOrEmpty(item) Then
                    dicDateApp(dateToAdd).Add(cicloToAdd)
                End If

            End If

        End Sub

        Private Function BuildMessageLabelRitardo(maxSollecito As Int32, dataAppDic As Dictionary(Of String, List(Of String))) As String

            Dim msg As New System.Text.StringBuilder()

            If Not dataAppDic Is Nothing AndAlso dataAppDic.Count > 0 Then
                Dim sollecito As String
                If maxSollecito = 1 Then
                    sollecito = "(Avv.)"
                Else
                    sollecito = String.Format("( {0}° )", (maxSollecito - 1).ToString)
                End If
                For Each dataAppKeyValuePair As KeyValuePair(Of String, List(Of String)) In dataAppDic
                    If msg.Length > 0 Then msg.Append(Environment.NewLine)
                    msg.AppendFormat("{0} {1}{2}{3}", dataAppKeyValuePair.Key, sollecito, Environment.NewLine, String.Join(" - ", dataAppKeyValuePair.Value.ToArray))
                Next
            End If

            Return msg.ToString()

        End Function

        Private Sub AddConvocazioniWhereConditions(dam As IDAM, codCns As String, strDataInizio As String, strDataFine As String, codAmb As Int32, filtroPazAvvisati As OnVac.Enumerators.FiltroAvvisati, querySoloBilancio As String)

            With dam.QB

                ' --- Filtri --- '
                .AddWhereCondition("CNV_CNS_CODICE", Comparatori.Uguale, codCns, DataTypes.Stringa)
                .AddWhereCondition("cnv_data_appuntamento", Comparatori.MaggioreUguale, strDataInizio + " 0.00", DataTypes.DataOra)
                .AddWhereCondition("cnv_data_appuntamento", Comparatori.MinoreUguale, strDataFine + " 23.59", DataTypes.DataOra)
                'non deve considerare i pazienti senza bilancio [modifica 04/02/2005]
                .OpenParanthesis()
                .OpenParanthesis()
                .AddWhereCondition("", Comparatori.Exist, " (" & querySoloBilancio & ") ", DataTypes.Replace)
                .AddWhereCondition("CNV_CNS_CODICE", Comparatori.Uguale, codCns, DataTypes.Stringa)
                If Not codAmb = Nothing AndAlso codAmb <> 0 Then
                    .AddWhereCondition("CNV_AMB_CODICE", Comparatori.Uguale, codAmb, DataTypes.Numero)
                End If
                .CloseParanthesis()
                .OpenParanthesis()
                .AddWhereCondition("", Comparatori.NotExist, " (" & querySoloBilancio & ") ", DataTypes.Replace, "OR")
                .AddWhereCondition("CNV_DURATA_APPUNTAMENTO", Comparatori.Diverso, "0", DataTypes.Numero)
                .CloseParanthesis()
                .CloseParanthesis()
                '
                ' Filtro solo avvisati o solo non avvisati
                Select Case filtroPazAvvisati
                    Case OnVac.Enumerators.FiltroAvvisati.SoloAvvisati
                        ' Solo pazienti che hanno la data di invio valorizzata
                        .AddWhereCondition("cnv_data_invio", Comparatori.[IsNot], "NULL", DataTypes.Replace)
                    Case OnVac.Enumerators.FiltroAvvisati.SoloNonAvvisati
                        ' Solo pazienti che hanno la data di invio non valorizzata
                        .AddWhereCondition("cnv_data_invio", Comparatori.Is, "NULL", DataTypes.Replace)
                    Case OnVac.Enumerators.FiltroAvvisati.Tutti
                        ' Tutti i pazienti, quindi nessun filtro
                End Select

            End With

        End Sub

        Public Function BuildDataSetAppuntamentiGiornoBilanci(codiceConsultorio As String, codiceUsl As String, strDataInizio As String, strDataFine As String, filtroPazientiAvvisati As OnVac.Enumerators.FiltroAvvisati) As AppuntamentiGiornoBilanci Implements IAppuntamentiGiornoProvider.BuildDataSetAppuntamentiGiornoBilanci

            Return BuildDataSetAppuntamentiGiornoBilanci(codiceConsultorio, Nothing, codiceUsl, strDataInizio, strDataFine, filtroPazientiAvvisati, Nothing)

        End Function

#End Region

#Region " V_AVVISI - V_BILANCI "

        ''' <summary>
        ''' Controlla se un paziente è presente nella v_avvisi
        ''' </summary>
        ''' <param name="pazCodice"></param>
        ''' <param name="dataCnv"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ControllaAvviso(pazCodice As Integer, dataCnv As Date) As Boolean Implements IAppuntamentiGiornoProvider.ControllaAvviso

            Dim obj As Object

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("1")
                .AddTables("V_AVVISI")
                .AddWhereCondition("PAZ_CODICE", Comparatori.Uguale, pazCodice, DataTypes.Numero)
                .AddWhereCondition("CNV_DATA", Comparatori.Uguale, dataCnv, DataTypes.Data)
            End With

            Try
                obj = _DAM.ExecScalar()

                If Not obj Is Nothing And Not obj Is DBNull.Value Then
                    Return True
                End If

            Catch ex As Exception

                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return False

        End Function

        ''' <summary>
        ''' Controlla se un paziente è presente nella v_bilanci
        ''' </summary>
        ''' <param name="pazCodice"></param>
        ''' <param name="dataCnv"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ControllaSoloBilancio(pazCodice As Integer, dataCnv As Date) As Boolean Implements IAppuntamentiGiornoProvider.ControllaSoloBilancio

            Try
                With _DAM.QB
                    .NewQuery()
                    .AddSelectFields("1")
                    .AddTables("V_BILANCI")
                    .AddWhereCondition("PAZ_CODICE", Comparatori.Uguale, pazCodice, DataTypes.Numero)
                    .AddWhereCondition("CNV_DATA", Comparatori.Uguale, dataCnv, DataTypes.Data)
                    .AddWhereCondition("CNV_DATA_APPUNTAMENTO", Comparatori.[IsNot], "null", DataTypes.Replace)
                End With

                Dim obj As Object = _DAM.ExecScalar

                If Not obj Is Nothing And Not obj Is DBNull.Value Then
                    Return True
                End If

            Catch ex As Exception

                LogError(ex)
                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return False

        End Function

        ''' <summary>
        ''' Restituisce i codice dei pazienti, le date di convocazione e i flag relativi al ritardo, in base ai dati specificati
        ''' </summary>
        ''' <param name="command"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetListPazientiAvvisi(command As Entities.PazientiAvvisiCommand) As List(Of Entities.PazienteAvviso) Implements IAppuntamentiGiornoProvider.GetListPazientiAvvisi

            Dim tabella As String = "V_AVVISI"

            Select Case command.TipoStampaAppuntamento
                Case Constants.TipoStampaAppuntamento.Bilanci
                    tabella = "V_BILANCI"
                Case Constants.TipoStampaAppuntamento.BilanciMalattia
                    tabella = "V_BILANCI_MALATTIA"
            End Select

            With _DAM.QB
                '--
                .NewQuery()
                .AddTables(tabella)
                .AddSelectFields("DISTINCT(PAZ_CODICE), CNV_DATA")
                '--
                ' filtro se nessun filtro precedente è stato aggiunto => serve se vengono aperte le parentesi
                .AddWhereCondition("1", Comparatori.Uguale, "1", DataTypes.Replace)
                '--
                If command.TipoStampaAppuntamento = Constants.TipoStampaAppuntamento.Avvisi Or
                   command.TipoStampaAppuntamento = Constants.TipoStampaAppuntamento.CampagnaAdulti Then
                    '--
                    .AddSelectFields("SOLLECITO")
                    '--
                    .OpenParanthesis()
                    .OpenParanthesis()
                    '--
                ElseIf command.TipoStampaAppuntamento = Constants.TipoStampaAppuntamento.Bilanci Or
                       command.TipoStampaAppuntamento = Constants.TipoStampaAppuntamento.BilanciMalattia Then
                    '--
                    .AddSelectFields("NULL as SOLLECITO")
                    '--
                End If
                '--
                ' Periodo di appuntamento
                .AddWhereCondition(tabella + ".CNV_DATA_APPUNTAMENTO", Comparatori.MaggioreUguale, command.DataInizioAppuntamento, DataTypes.Data)
                .AddWhereCondition(.FC.Tronca(tabella + ".CNV_DATA_APPUNTAMENTO"), Comparatori.MinoreUguale, command.DataFineAppuntamento, DataTypes.Data)
                '--
                If command.TipoStampaAppuntamento = Constants.TipoStampaAppuntamento.Avvisi Or
                   command.TipoStampaAppuntamento = Constants.TipoStampaAppuntamento.CampagnaAdulti Then
                    .CloseParanthesis()
                    .OpenParanthesis()
                    .AddWhereCondition(tabella + ".CNV_DATA_APPUNTAMENTO", Comparatori.Uguale, New DateTime(2100, 1, 1), DataTypes.Data, "OR")
                    .AddWhereCondition(tabella + ".CNV_DATA_INVIO", Comparatori.Is, "NULL", DataTypes.Replace)
                    .AddWhereCondition(tabella + ".SEDUTA_CICLO_OBBLIGATORIA", Comparatori.Uguale, "S", DataTypes.Stringa)
                    .AddWhereCondition(tabella + ".SOLLECITO_SEDUTA_CICLO", Comparatori.Maggiore, tabella + ".NUM_SOLLECITI", DataTypes.Replace)
                    .CloseParanthesis()
                    .CloseParanthesis()
                End If

                ' Filtro sulla data di invio
                Select Case command.FiltroPazientiAvvisati
                    Case Enumerators.FiltroAvvisati.SoloAvvisati
                        .AddWhereCondition(tabella + ".CNV_DATA_INVIO", Comparatori.IsNot, "null", DataTypes.Replace)
                    Case Enumerators.FiltroAvvisati.SoloNonAvvisati
                        .AddWhereCondition(tabella + ".CNV_DATA_INVIO", Comparatori.Is, "null", DataTypes.Replace)
                End Select

                ' Filtro sul consultorio
                If Not String.IsNullOrWhiteSpace(command.CodiceConsultorio) Then
                    .AddWhereCondition(tabella + ".CNV_CNS_CODICE", Comparatori.Uguale, command.CodiceConsultorio, DataTypes.Stringa)
                End If

                ' Filtro sull'ambulatorio
                If Not String.IsNullOrWhiteSpace(command.CodiceAmbulatorio) Then
                    '--
                    Dim codiceAmbulatorio As Integer = 0
                    If Integer.TryParse(command.CodiceAmbulatorio, codiceAmbulatorio) Then
                        If codiceAmbulatorio > 0 Then
                            .AddWhereCondition(tabella + ".CNV_AMB_CODICE", Comparatori.Uguale, codiceAmbulatorio, DataTypes.Numero)
                        End If
                    End If
                    '--
                End If

            End With

            Return Me.GetListPazientiAvvisi(_DAM)

        End Function

        Public Function GetListPazientiAvvisiPostel(command As Entities.PazientiAvvisiCommand) As List(Of Entities.PazienteAvviso) Implements IAppuntamentiGiornoProvider.GetListPazientiAvvisiPostel

            With _DAM.QB
                '--
                Dim strCnsDistretti As String = String.Empty

                If Not String.IsNullOrWhiteSpace(command.Distretti) Then

                    ' Tabella consultori del distretto
                    .NewQuery(False, False)
                    .AddSelectFields("1")
                    .AddTables("T_ANA_CONSULTORI, T_ANA_LINK_UTENTI_CONSULTORI")
                    .AddWhereCondition("CNS_DIS_CODICE", Comparatori.Uguale, command.Distretti, DataTypes.Stringa)
                    .AddWhereCondition("CNV_CNS_CODICE", Comparatori.Uguale, "CNS_CODICE", DataTypes.OutJoinLeft)
                    .AddWhereCondition("LUC_CNS_CODICE", Comparatori.Uguale, "CNS_CODICE", DataTypes.Join)
                    .AddWhereCondition("LUC_UTE_ID", Comparatori.Uguale, command.userId, DataTypes.Numero)

                    strCnsDistretti = .GetSelect
                Else
                    If command.userId.HasValue Then
                        ' Tabella consultori utente
                        .NewQuery(False, False)
                        .AddSelectFields("1")
                        .AddTables("T_ANA_CONSULTORI, T_ANA_LINK_UTENTI_CONSULTORI")
                        .AddWhereCondition("LUC_UTE_ID", Comparatori.Uguale, command.userId, DataTypes.Numero)
                        .AddWhereCondition("CNV_CNS_CODICE", Comparatori.Uguale, "CNS_CODICE", DataTypes.OutJoinLeft)
                        .AddWhereCondition("LUC_CNS_CODICE", Comparatori.Uguale, "CNS_CODICE", DataTypes.Join)

                        strCnsDistretti = .GetSelect
                    End If
                End If
                .NewQuery(False, False)
                .AddTables("V_AVVISI_POSTEL")
                .AddSelectFields("DISTINCT(PAZ_CODICE), CNV_DATA, SOLLECITO")
                '--
                ' filtro se nessun filtro precedente è stato aggiunto => serve se vengono aperte le parentesi
                .AddWhereCondition("1", Comparatori.Uguale, "1", DataTypes.Replace)
                '--
                .OpenParanthesis()
                '--
                ' Periodo di appuntamento
                .OpenParanthesis()
                .AddWhereCondition("CNV_DATA_APPUNTAMENTO", Comparatori.MaggioreUguale, command.DataInizioAppuntamento, DataTypes.Data)
                .AddWhereCondition(.FC.Tronca("CNV_DATA_APPUNTAMENTO"), Comparatori.MinoreUguale, command.DataFineAppuntamento, DataTypes.Data)
                .CloseParanthesis()
                '--
                .OpenParanthesis()
                .AddWhereCondition("CNV_DATA_APPUNTAMENTO", Comparatori.Uguale, New DateTime(2100, 1, 1), DataTypes.Data, "OR")
                .AddWhereCondition("CNV_DATA_INVIO", Comparatori.Is, "NULL", DataTypes.Replace)
                .AddWhereCondition("SEDUTA_CICLO_OBBLIGATORIA", Comparatori.Uguale, "S", DataTypes.Stringa)
                .AddWhereCondition("SOLLECITO_SEDUTA_CICLO", Comparatori.Maggiore, "NUM_SOLLECITI", DataTypes.Replace)
                .CloseParanthesis()
                '--
                .CloseParanthesis()
                '--
                ' Filtro cittadinanza
                If Not String.IsNullOrEmpty(command.CodiceCittadinanza) Then
                    .AddWhereCondition("PAZ_CIT_CODICE", Comparatori.Uguale, command.CodiceCittadinanza, DataTypes.Stringa)
                End If
                '--
                ' Filtro nascita
                If command.DataInizioNascita.HasValue Then
                    .AddWhereCondition("PAZ_DATA_NASCITA", Comparatori.MaggioreUguale, command.DataInizioNascita.Value, DataTypes.Data)
                End If
                If command.DataFineNascita.HasValue Then
                    .AddWhereCondition(.FC.Tronca("PAZ_DATA_NASCITA"), Comparatori.MinoreUguale, command.DataFineNascita.Value, DataTypes.Data)
                End If
                '--
                ' Filtro sulla data di invio
                Select Case command.FiltroPazientiAvvisati
                    Case Enumerators.FiltroAvvisati.SoloAvvisati
                        .AddWhereCondition("CNV_DATA_INVIO", Comparatori.IsNot, "null", DataTypes.Replace)
                    Case Enumerators.FiltroAvvisati.SoloNonAvvisati
                        .AddWhereCondition("CNV_DATA_INVIO", Comparatori.Is, "null", DataTypes.Replace)
                End Select
                '--
                ' Filtro sul consultorio
                If Not String.IsNullOrWhiteSpace(command.CodiceConsultorio) Then
                    .AddWhereCondition("CNV_CNS_CODICE", Comparatori.Uguale, command.CodiceConsultorio, DataTypes.Stringa)
                End If
                If Not String.IsNullOrWhiteSpace(command.Distretti) AndAlso Not String.IsNullOrWhiteSpace(strCnsDistretti) Then
                    .AddWhereCondition("", Comparatori.Exist, "(" + strCnsDistretti + ")", DataTypes.Replace)
                Else
                    If command.userId.HasValue AndAlso Not String.IsNullOrWhiteSpace(strCnsDistretti) Then
                        .AddWhereCondition("", Comparatori.Exist, "(" + strCnsDistretti + ")", DataTypes.Replace)
                    End If

                End If
                '--
                ' Filtro sull'ambulatorio
                If Not String.IsNullOrWhiteSpace(command.CodiceAmbulatorio) Then
                    '--
                    Dim codiceAmbulatorio As Integer = 0
                    If Integer.TryParse(command.CodiceAmbulatorio, codiceAmbulatorio) Then
                        If codiceAmbulatorio > 0 Then
                            .AddWhereCondition("CNV_AMB_CODICE", Comparatori.Uguale, codiceAmbulatorio, DataTypes.Numero)
                        End If
                    End If
                    '--
                End If
                '--
            End With
            '--
            Return Me.GetListPazientiAvvisi(_DAM)
            '--
        End Function

        Private Function GetListPazientiAvvisi(dam As IDAM) As List(Of Entities.PazienteAvviso)

            Dim listPazientiAvvisi As List(Of Entities.PazienteAvviso) = Nothing

            Dim dataStampa As DateTime = Date.Now

            Using idr As IDataReader = dam.BuildDataReader()

                If Not idr Is Nothing Then

                    listPazientiAvvisi = New List(Of Entities.PazienteAvviso)()

                    Dim pazienteAvviso As Entities.PazienteAvviso = Nothing

                    Dim paz_codice As Integer = idr.GetOrdinal("PAZ_CODICE")
                    Dim cnv_data As Integer = idr.GetOrdinal("CNV_DATA")
                    Dim sollecito As Integer = idr.GetOrdinal("SOLLECITO")

                    While idr.Read()

                        If Not idr.IsDBNull(paz_codice) AndAlso Not idr.IsDBNull(cnv_data) Then

                            pazienteAvviso = New Entities.PazienteAvviso()
                            pazienteAvviso.CodicePaziente = idr.GetInt32OrDefault(paz_codice)
                            pazienteAvviso.DataConvocazione = idr.GetDateTime(cnv_data)
                            pazienteAvviso.IsPazienteRitardatario = (Not idr.IsDBNull(sollecito))

                            listPazientiAvvisi.Add(pazienteAvviso)

                        End If

                    End While
                End If

            End Using

            Return listPazientiAvvisi

        End Function

#End Region

#Region " Ricerca Appuntamenti "

        ' Ricerca appuntamenti del giorno specificato, per il consultorio specificato
        ' Restituisce un datatable, ordinato secondo i campi presenti in strCampiOrdinamento.
        ' In caso di errore restituisce Nothing.
        Public Function FindAppuntamentiGiorno(codCns As String, codAmb As Integer, codiceUsl As String, strData As String, strCampiOrdinamento As String) As DataTable Implements IAppuntamentiGiornoProvider.FindAppuntamentiGiorno

            Dim dt As New DataTable()

            Try
                _DAM.BeginTrans()

                ' ------------------ Query per escludere i pazienti con solo bilancio ------------------ '
                With _DAM.QB
                    .NewQuery()
                    .AddSelectFields("1")
                    .AddTables("T_VAC_PROGRAMMATE")
                    .AddWhereCondition("VPR_PAZ_CODICE", Comparatori.Uguale, "CNV_PAZ_CODICE", DataTypes.Join)
                    .AddWhereCondition("VPR_CNV_DATA", Comparatori.Uguale, "CNV_DATA", DataTypes.Join)
                End With
                Dim querySoloBilancio As String = _DAM.QB.GetSelect
                ' -------------------------------------------------------------------------------------- '

                ' ------------------ Query Immigrati ------------------ '
                '
                ' --- Sottoquery comune di provenienza associato all'ausl --- '
                With _DAM.QB
                    .NewQuery(False, False)
                    .AddSelectFields("1")
                    .AddTables("t_ana_link_comuni_consultori")
                    .AddWhereCondition("t1.paz_com_comune_provenienza", Comparatori.Uguale, "cco_com_codice", DataTypes.Join)
                End With
                Dim strQueryComuniUsl As String = _DAM.QB.GetSelect()
                ' ----------------------------------------------------------- '

                ' --- Sottoquery per comune di provenienza = stato comunitario --- '
                With _DAM.QB
                    .NewQuery(False, False)
                    .AddSelectFields("1")
                    .AddTables("t_ana_cittadinanze")
                    .AddWhereCondition("t1.paz_com_comune_provenienza", Comparatori.Like, "999%", DataTypes.Stringa)
                    .AddWhereCondition("cit_cee", Comparatori.Uguale, "S", DataTypes.Stringa)
                    .AddWhereCondition("SUBSTR(t1.paz_com_comune_provenienza,4)", Comparatori.Uguale, "cit_codice", DataTypes.Replace)
                End With
                Dim strQueryComuniStatoComunitario As String = _DAM.QB.GetSelect
                ' ---------------------------------------------------------------- '

                ' --- Sottoquery per paziente con vaccinazioni già eseguite --- '
                With _DAM.QB
                    .NewQuery(False, False)
                    .AddSelectFields("distinct 1")
                    .AddTables("t_vac_eseguite")
                    .AddWhereCondition("ves_cns_codice", Comparatori.[IsNot], "NULL", DataTypes.Replace)
                    .AddWhereCondition("ves_paz_codice", Comparatori.Uguale, "p1.paz_codice", DataTypes.Join)
                End With
                Dim strQueryVacEseguitePaz As String = _DAM.QB.GetSelect
                ' ------------------------------------------------------------- '

                ' --- Sottoquery per non extracomunitari prima volta --- '
                With _DAM.QB
                    .NewQuery(False, False)
                    .AddSelectFields("COUNT(t1.paz_codice)")
                    .AddTables("T_PAZ_PAZIENTI t1")
                    .AddWhereCondition("t1.paz_codice", Comparatori.Uguale, "p1.paz_codice", DataTypes.Join)
                    .OpenParanthesis()
                    .OpenParanthesis()
                    .AddWhereCondition("", Comparatori.NotExist, "(" + strQueryComuniUsl + ")", DataTypes.Replace)
                    .AddWhereCondition("t1.paz_com_comune_provenienza", Comparatori.NotLike, "999%", DataTypes.Stringa)
                    .CloseParanthesis()
                    .AddWhereCondition("", Comparatori.Exist, "(" + strQueryComuniStatoComunitario + ")", DataTypes.Replace, "OR")
                    .CloseParanthesis()
                    .AddWhereCondition("", Comparatori.NotExist, "(" + strQueryVacEseguitePaz + ")", DataTypes.Replace)
                End With
                Dim strQueryImmigratiNonPrimaVolta As String = _DAM.QB.GetSelect
                ' ------------------------------------------------------------------ '

                ' --- Sottoquery per comune provenienza paziente = stato extracomunitario --- '
                Dim strQueryComuniStatoExtracom As String = ""
                With _DAM.QB
                    .NewQuery(False, False)
                    .AddSelectFields("count(*)")
                    .AddTables("t_ana_cittadinanze")
                    .AddWhereCondition("cit_codice", Comparatori.Uguale, "p1.paz_cit_codice", DataTypes.Join)
                    .OpenParanthesis()
                    .AddWhereCondition("cit_cee", Comparatori.Uguale, "N", DataTypes.Stringa)
                    .AddWhereCondition("cit_cee", Comparatori.Is, "NULL", DataTypes.Replace, "OR")
                    .CloseParanthesis()
                End With
                strQueryComuniStatoExtracom = _DAM.QB.GetSelect()
                ' --------------------------------------------------------------------------- '

                ' --- Sottoquery per determinare se il paziente ha una malattia cronica --- '
                Dim qryCronico As String
                With _DAM.QB
                    .NewQuery(False, False)
                    .AddSelectFields("distinct 1")
                    .AddTables("t_paz_malattie")
                    .AddWhereCondition("pma_paz_codice", Comparatori.Uguale, "p1.paz_codice", DataTypes.Join)
                    .AddWhereCondition("pma_mal_codice", Comparatori.Diverso, "0", DataTypes.Stringa)
                End With
                qryCronico = "(" & _DAM.QB.GetSelect() & ")"
                ' ---------------------------------------------------------------------------- '

                ' sottoquery per le note
                Dim queryNotaLibero1 As String = GetNotaPaziente(_DAM.QB, codiceUsl, Constants.CodiceTipoNotaPaziente.Appuntamenti)
                Dim queryNotaLibero2 As String = GetNotaPaziente(_DAM.QB, codiceUsl, Constants.CodiceTipoNotaPaziente.MalattiePregresse)
                Dim queryNotaLibero3 As String = GetNotaPaziente(_DAM.QB, codiceUsl, Constants.CodiceTipoNotaPaziente.Esclusioni)

                ' ------------------------------------------------------------------------- '


                With _DAM.QB
                    .NewQuery(False, False)
                    '
                    .AddTables("T_CNV_CONVOCAZIONI", "T_PAZ_PAZIENTI p1", "T_ANA_AMBULATORI")
                    '
                    .AddSelectFields(.FC.Convert("CNV_DATA_APPUNTAMENTO", "CHAR", "", "HH24.MI.SS", "") & " ORA")
                    .AddSelectFields("PAZ_CODICE", "PAZ_CODICE_AUSILIARIO", .FC.Concat(.FC.Concat("PAZ_COGNOME", "' '"), "PAZ_NOME") & " NOME", "PAZ_COGNOME,PAZ_NOME,PAZ_MED_CODICE_BASE,PAZ_COM_CODICE_RESIDENZA")
                    .AddSelectFields("(" + queryNotaLibero1 + ") PAZ_LIBERO_1")
                    .AddSelectFields("(" + queryNotaLibero2 + ") PAZ_LIBERO_2")
                    .AddSelectFields("(" + queryNotaLibero3 + ") PAZ_LIBERO_3")
                    .AddSelectFields("PAZ_DATA_NASCITA", "'' VACCINAZIONI", "'' DOSE", "CNV_DURATA_APPUNTAMENTO", "CNV_DATA")
                    .AddSelectFields("DECODE((" & strQueryComuniStatoExtracom & "),1,'',DECODE((" & strQueryImmigratiNonPrimaVolta & "),1,'I','')) TIPO_IMMI_NON_EXTRA_PRIMA")
                    .AddSelectFields("DECODE((" & strQueryComuniStatoExtracom & "),1,'E','') TIPO_EXTRACOMUNITARI ")
                    .AddSelectFields("DECODE(" & qryCronico & ",1,'C','') CRONICO")
                    .AddSelectFields("CNV_AMB_CODICE", "AMB_DESCRIZIONE")
                    .AddSelectFields("CNV_ID_CONVOCAZIONE")

                    .AddWhereCondition("CNV_PAZ_CODICE", Comparatori.Uguale, "PAZ_CODICE", DataTypes.Join)
                    .AddWhereCondition("CNV_CNS_CODICE", Comparatori.Uguale, codCns, DataTypes.Stringa)
                    .AddWhereCondition("CNV_AMB_CODICE", Comparatori.Uguale, "AMB_CODICE", DataTypes.OutJoinLeft)
                    If Not codAmb = Nothing AndAlso codAmb <> 0 Then
                        .AddWhereCondition("CNV_AMB_CODICE", Comparatori.Uguale, codAmb, DataTypes.Numero)
                    End If
                    '
                    .AddWhereCondition(.FC.Tronca("CNV_DATA_APPUNTAMENTO"), Comparatori.Uguale, strData, DataTypes.Data)
                    '
                    ' Filtro per non considerare i pazienti senza bilancio
                    .OpenParanthesis()
                    .OpenParanthesis()
                    .AddWhereCondition("", Comparatori.Exist, " (" & querySoloBilancio & ") ", DataTypes.Replace)
                    .AddWhereCondition("CNV_CNS_CODICE", Comparatori.Uguale, codCns, DataTypes.Stringa)
                    .CloseParanthesis()
                    .OpenParanthesis()
                    .AddWhereCondition("", Comparatori.NotExist, " (" & querySoloBilancio & ") ", DataTypes.Replace, "OR")
                    .AddWhereCondition("CNV_DURATA_APPUNTAMENTO", Comparatori.Diverso, "0", DataTypes.Numero)
                    .CloseParanthesis()
                    .CloseParanthesis()
                    '
                    ' Ordinamento
                    If strCampiOrdinamento = String.Empty Then
                        .AddOrderByFields("ORA", "PAZ_COGNOME", "PAZ_NOME")
                    Else
                        .AddOrderByFields(strCampiOrdinamento)
                    End If

                End With

                _DAM.BuildDataTable(dt)

                _DAM.Commit()

            Catch ex As Exception

                If _DAM.ExistTra Then _DAM.Rollback()
                dt = Nothing

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return dt

        End Function

        ' Restituisce un datatable con vaccinazioni e dosi del paziente paz_codice, per la convocazione cnv_data
        Public Function GetVaccDosiPaziente(paz_codice As Integer, cnv_data As Date) As DataTable Implements IAppuntamentiGiornoProvider.GetVaccDosiPaziente

            Dim dt As New DataTable()

            Try
                With _DAM.QB
                    .NewQuery()
                    .AddSelectFields("1")
                    .AddTables("T_VAC_ESEGUITE")
                    .AddWhereCondition("VPR_PAZ_CODICE", Comparatori.Uguale, "VES_PAZ_CODICE", DataTypes.Join)
                    .AddWhereCondition("VPR_VAC_CODICE", Comparatori.Uguale, "VES_VAC_CODICE", DataTypes.Join)
                    .AddWhereCondition("VPR_N_RICHIAMO", Comparatori.Uguale, "VES_N_RICHIAMO", DataTypes.Join)
                End With
                Dim qEseguite As String = _DAM.QB.GetSelect

                With _DAM.QB
                    .NewQuery(False, False)
                    .AddSelectFields("1")
                    .AddTables("T_VAC_ESCLUSE")
                    .AddWhereCondition("VPR_PAZ_CODICE", Comparatori.Uguale, "VEX_PAZ_CODICE", DataTypes.Join)
                    .AddWhereCondition("VPR_VAC_CODICE", Comparatori.Uguale, "VEX_VAC_CODICE", DataTypes.Join)
                    .OpenParanthesis()
                    .AddWhereCondition("VEX_DATA_SCADENZA", Comparatori.Is, "NULL", DataTypes.Replace)
                    .AddWhereCondition("VEX_DATA_SCADENZA", Comparatori.Maggiore, DateTime.Today, DataTypes.Data, "OR")
                    .CloseParanthesis()
                End With
                Dim qEscluse As String = _DAM.QB.GetSelect

                With _DAM.QB
                    .NewQuery(False, False)
                    .AddSelectFields("VAC_DESCRIZIONE", "VPR_N_RICHIAMO")
                    .AddTables("T_VAC_PROGRAMMATE", "T_ANA_VACCINAZIONI")
                    .AddWhereCondition("VPR_VAC_CODICE", Comparatori.Uguale, "VAC_CODICE", DataTypes.Join)
                    .AddWhereCondition("VPR_PAZ_CODICE", Comparatori.Uguale, paz_codice, DataTypes.Numero)
                    .AddWhereCondition("VPR_CNV_DATA", Comparatori.Uguale, cnv_data, DataTypes.Data)
                    .AddWhereCondition("", Comparatori.NotExist, "(" & qEseguite & ")", DataTypes.Replace)
                    .AddWhereCondition("", Comparatori.NotExist, "(" & qEscluse & ")", DataTypes.Replace)
                End With

                _DAM.BuildDataTable(dt)

            Catch ex As Exception

                dt = Nothing
                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return dt

        End Function

#End Region

#Region " OnVac API "

        Public Function GetListAppuntamentiPazientiApi(listCodiciPazienti As List(Of Long)) As List(Of Appuntamento) Implements IAppuntamentiGiornoProvider.GetListAppuntamentiPazientiApi

            Dim listAppuntamenti As List(Of Appuntamento) = Nothing

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleCommand()

                    cmd.Connection = Connection

                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim filtro As String = String.Empty

                    If listCodiciPazienti.Count = 1 Then
                        filtro = " = :codicePaziente "
                        cmd.Parameters.AddWithValue("codicePaziente", listCodiciPazienti.First())
                    Else
                        Dim filtroPazienti As GetInFilterResult = GetInFilter(listCodiciPazienti)
                        filtro = String.Format(" IN ({0}) ", filtroPazienti.InFilter)
                        cmd.Parameters.AddRange(filtroPazienti.Parameters)
                    End If

                    cmd.CommandText = String.Format(GetQueryAppuntamentiPazienteApi(False), filtro)

                    listAppuntamenti = GetListAppuntamentiapi(cmd)

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return listAppuntamenti

        End Function

        Public Function GetListAppuntamentiPazientiByIdApi(IdAppuntamento As Long) As List(Of Appuntamento) Implements IAppuntamentiGiornoProvider.GetListAppuntamentiPazientiByIdApi

            Dim listAppuntamenti As List(Of Appuntamento) = Nothing

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleCommand()

                    cmd.Connection = Connection

                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim filtro As String = String.Empty

                    Dim s As New Text.StringBuilder()

                    s.Append("select a.*, ")
                    s.Append("       (select nvl(max(NOC_COSTO_UNITARIO), 0) ")
                    s.Append("        from T_ANA_LINK_NOC_ASSOCIAZIONI ")
                    s.Append("        join T_ANA_NOMI_COMMERCIALI ON NAL_NOC_CODICE = NOC_CODICE ")
                    s.Append("        left join T_NOC_CONDIZIONI_PAGAMENTO ON NOC_CODICE = CPG_NOC_CODICE ")
                    s.Append("        where NAL_ASS_CODICE = a.vpr_ass_codice ")
                    s.Append("        and NOC_OBSOLETO = 'N' ")
                    s.Append("        and (NOC_SESSO = 'E' OR NOC_SESSO = a.paz_sesso) ")
                    s.Append("        and (NOC_ETA_INIZIO IS NULL OR NOC_ETA_INIZIO <= a.gg_paz) ")
                    s.Append("        and (NOC_ETA_FINE IS NULL OR NOC_ETA_FINE >= a.gg_paz) ")
                    s.Append("        and (CPG_DA_ETA is null OR CPG_DA_ETA <= a.gg_paz) ")
                    s.Append("        and (CPG_A_ETA IS NULL OR CPG_A_ETA >= a.gg_paz)) importo ")
                    s.Append("from ( ")
                    s.Append("select cnv_paz_codice, cnv_data, cnv_data_appuntamento, cnv_cns_codice, cnv_id_convocazione, ")
                    s.Append("       cns_descrizione, cns_indirizzo, cns_n_telefono, cns_stampa2, ")
                    s.Append("       cnv_amb_codice, amb_descrizione, cns_com_codice, com_descrizione, ")
                    s.Append("       vpr_vac_codice, vpr_n_richiamo, vac_descrizione, ")
                    s.Append("       cnv_durata_appuntamento, paz_usl_codice_assistenza, ")
                    s.Append("       vpr_ass_codice, ")
                    s.Append("       paz_cognome, paz_nome, paz_sesso, paz_data_nascita, ass_descrizione, ")
                    s.Append("       (select trunc(sysdate) - trunc(paz_data_nascita) from dual) gg_paz ")
                    s.Append(" from t_cnv_convocazioni ")
                    s.Append("    join t_vac_programmate on cnv_paz_codice = vpr_paz_codice and cnv_data = vpr_cnv_data ")
                    s.Append("    join t_ana_vaccinazioni on vpr_vac_codice = vac_codice ")
                    s.Append("    left join t_ana_associazioni on vpr_ass_codice = ass_codice ")
                    s.Append("    join t_paz_pazienti on cnv_paz_codice = paz_codice ")
                    s.Append("    left join t_ana_consultori on cnv_cns_codice = cns_codice ")
                    s.Append("    left join t_ana_ambulatori on cnv_amb_codice = amb_codice ")
                    s.Append("    left join t_ana_comuni on cns_com_codice = com_codice ")
                    s.Append("where CNV_ID_CONVOCAZIONE = :CNV_ID_CONVOCAZIONE ")
                    s.Append("and not cnv_data_appuntamento is null ) a ")

                    cmd.Parameters.AddWithValue("CNV_ID_CONVOCAZIONE", IdAppuntamento)

                    cmd.CommandText = s.ToString()

                    listAppuntamenti = GetListAppuntamentiApi(cmd)

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return listAppuntamenti

        End Function

        Private Function GetListAppuntamentiApi(cmd As OracleCommand) As List(Of Appuntamento)

            Dim listAppuntamenti As New List(Of Appuntamento)()

            Using idr As IDataReader = cmd.ExecuteReader()

                If Not idr Is Nothing Then

                    Dim cnv_paz_codice As Integer = idr.GetOrdinal("cnv_paz_codice")
                    Dim cnv_data As Integer = idr.GetOrdinal("cnv_data")
                    Dim cnv_id_convocazione As Integer = idr.GetOrdinal("cnv_id_convocazione")
                    Dim cnv_data_appuntamento As Integer = idr.GetOrdinal("cnv_data_appuntamento")
                    Dim cnv_cns_codice As Integer = idr.GetOrdinal("cnv_cns_codice")
                    Dim cns_descrizione As Integer = idr.GetOrdinal("cns_descrizione")
                    Dim cns_indirizzo As Integer = idr.GetOrdinal("cns_indirizzo")
                    Dim com_descrizione As Integer = idr.GetOrdinal("com_descrizione")
                    Dim cns_n_telefono As Integer = idr.GetOrdinal("cns_n_telefono")
                    Dim cns_stampa2 As Integer = idr.GetOrdinal("cns_stampa2")
                    Dim cnv_amb_codice As Integer = idr.GetOrdinal("cnv_amb_codice")
                    Dim amb_descrizione As Integer = idr.GetOrdinal("amb_descrizione")
                    Dim vpr_vac_codice As Integer = idr.GetOrdinal("vpr_vac_codice")
                    Dim vac_descrizione As Integer = idr.GetOrdinal("vac_descrizione")
                    Dim cnv_durata_appuntamento As Integer = idr.GetOrdinal("cnv_durata_appuntamento")
                    Dim paz_usl_codice_assistenza As Integer = idr.GetOrdinal("paz_usl_codice_assistenza")
                    Dim vpr_n_richiamo As Integer = idr.GetOrdinal("vpr_n_richiamo")
                    Dim paz_cognome As Integer = idr.GetOrdinal("paz_cognome")
                    Dim paz_nome As Integer = idr.GetOrdinal("paz_nome")
                    Dim paz_data_nascita As Integer = idr.GetOrdinal("paz_data_nascita")
                    Dim vpr_ass_codice As Integer = idr.GetOrdinal("vpr_ass_codice")
                    Dim ass_descrizione As Integer = idr.GetOrdinal("ass_descrizione")
                    Dim importo As Integer = idr.GetOrdinal("importo")

                    While idr.Read()

                        Dim app As New Appuntamento()

                        app.DataConvocazione = idr.GetDateTime(cnv_data)
                        app.IdAppuntamento = idr.GetStringOrDefault(cnv_id_convocazione)
                        app.DataAppuntamento = idr.GetDateTime(cnv_data_appuntamento)
                        app.CodiceConsultorio = idr.GetStringOrDefault(cnv_cns_codice)
                        app.DescrizioneConsultorio = idr.GetStringOrDefault(cns_descrizione)
                        app.IndirizzoConsultorio = idr.GetStringOrDefault(cns_indirizzo)
                        app.ComuneConsultorio = idr.GetStringOrDefault(com_descrizione)
                        app.TelefonoConsultorio = idr.GetStringOrDefault(cns_n_telefono)
                        app.NoteConsultorio = idr.GetStringOrDefault(cns_stampa2)
                        app.CodiceAmbulatorio = idr.GetInt32OrDefault(cnv_amb_codice)
                        app.DescrizioneAmbulatorio = idr.GetStringOrDefault(amb_descrizione)
                        app.CodiceVaccinazione = idr.GetStringOrDefault(vpr_vac_codice)
                        app.DescrizioneVaccinazione = idr.GetStringOrDefault(vac_descrizione)
                        app.DurataAppuntamento = idr.GetInt32OrDefault(cnv_durata_appuntamento)
                        app.CodiceUslassistenza = idr.GetStringOrDefault(paz_usl_codice_assistenza)
                        app.DoseVaccinazione = idr.GetInt32OrDefault(vpr_n_richiamo)
                        app.CodiceLocalePaziente = idr.GetInt64(cnv_paz_codice)
                        app.CognomePaziente = idr.GetStringOrDefault(paz_cognome)
                        app.NomePaziente = idr.GetStringOrDefault(paz_nome)
                        app.DataNascitaPaziente = idr.GetDateTimeOrDefault(paz_data_nascita)
                        app.CodiceAssociazione = idr.GetStringOrDefault(vpr_ass_codice)
                        app.DescrizioneAssociazione = idr.GetStringOrDefault(ass_descrizione)
                        app.ImportoIndicativo = idr.GetDoubleOrDefault(importo)
                        listAppuntamenti.Add(app)

                    End While

                End If

            End Using

            Return listAppuntamenti

        End Function
        ''' <summary>
        ''' Restituisce la lista di appuntamenti per i pazienti specificati
        ''' </summary>
        ''' <param name="listCodiciPazienti"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetListAppuntamentiPazienti(listCodiciPazienti As List(Of Long)) As List(Of Appuntamento) Implements IAppuntamentiGiornoProvider.GetListAppuntamentiPazienti

            Dim listAppuntamenti As List(Of Appuntamento) = Nothing

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand()

                    cmd.Connection = Connection

                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim filtro As String = String.Empty

                    If listCodiciPazienti.Count = 1 Then
                        filtro = " = :codicePaziente "
                        cmd.Parameters.AddWithValue("codicePaziente", listCodiciPazienti.First())
                    Else
                        Dim filtroPazienti As GetInFilterResult = GetInFilter(listCodiciPazienti)
                        filtro = String.Format(" IN ({0}) ", filtroPazienti.InFilter)
                        cmd.Parameters.AddRange(filtroPazienti.Parameters)
                    End If

                    cmd.CommandText = String.Format(GetQueryAppuntamentiPaziente(False), filtro)

                    listAppuntamenti = GetListAppuntamenti(cmd)

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return listAppuntamenti

        End Function

        ''' <summary>
        ''' Restituisce la lista di appuntamenti per il paziente nella data specificata
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataAppuntamento"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetListAppuntamentiPazienteData(codicePaziente As Long, dataAppuntamento As DateTime) As List(Of Appuntamento) Implements IAppuntamentiGiornoProvider.GetListAppuntamentiPazienteData

            Dim listAppuntamenti As List(Of Appuntamento) = Nothing

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand()

                    cmd.Connection = Connection

                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.CommandText = String.Format(GetQueryAppuntamentiPaziente(True), " = :codicePaziente ")
                    cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)
                    cmd.Parameters.AddWithValue("dataAppuntamento", dataAppuntamento)

                    listAppuntamenti = GetListAppuntamenti(cmd)

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return listAppuntamenti

        End Function

        ''' <summary>
        ''' Da effettuare in centrale per recuperare tutti gli appuntamenti presenti nella v_cnv_appuntamenti, nell'intervallo di date specificato.
        ''' </summary>
        ''' <param name="dataAppuntamentoDa"></param>
        ''' <param name="dataAppuntamentoA"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetAppuntamentiNotifica(dataAppuntamentoDa As DateTime, dataAppuntamentoA As DateTime) As List(Of Entities.AppuntamentoNotifica) Implements IAppuntamentiGiornoProvider.GetAppuntamentiNotifica

            Dim list As New List(Of Entities.AppuntamentoNotifica)()

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleClient.OracleCommand("select cnv_paz_codice, cnv_data, cnv_data_appuntamento, app_id from v_cnv_appuntamenti where cnv_data_appuntamento >= :cnv_data_appuntamento_da and cnv_data_appuntamento <= :cnv_data_appuntamento_a", Me.Connection)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("cnv_data_appuntamento_da", dataAppuntamentoDa)
                    cmd.Parameters.AddWithValue("cnv_data_appuntamento_a", dataAppuntamentoA)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim cnv_paz_codice As Integer = idr.GetOrdinal("cnv_paz_codice")
                            Dim cnv_data As Integer = idr.GetOrdinal("cnv_data")
                            Dim cnv_data_appuntamento As Integer = idr.GetOrdinal("cnv_data_appuntamento")
                            Dim app_id As Integer = idr.GetOrdinal("app_id")

                            Dim item As Entities.AppuntamentoNotifica = Nothing

                            While idr.Read()

                                item = New Entities.AppuntamentoNotifica()

                                item.CodiceLocalePaziente = idr.GetInt64OrDefault(cnv_paz_codice)
                                item.AppIdAziendaLocale = idr.GetStringOrDefault(app_id)
                                item.DataConvocazione = idr.GetDateTimeOrDefault(cnv_data)
                                item.DataOraAppuntamento = idr.GetDateTimeOrDefault(cnv_data_appuntamento)

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
        ''' Restituisce true se esiste un appuntamento nell'ambulatorio e nella data e ora specificati
        ''' </summary>
        ''' <param name="dataAppuntamento"></param>
        ''' <param name="codiceAmbulatorio"></param>
        ''' <returns></returns>
        Public Function ExistsAppuntamentoAmbulatorio(dataAppuntamento As Date, codiceAmbulatorio As Integer) As Boolean Implements IAppuntamentiGiornoProvider.ExistsAppuntamentoAmbulatorio

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleCommand("select 1 from T_CNV_CONVOCAZIONI where CNV_AMB_CODICE = :CNV_AMB_CODICE and CNV_DATA_APPUNTAMENTO = :CNV_DATA_APPUNTAMENTO ", Connection)

                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("CNV_AMB_CODICE", codiceAmbulatorio)
                    cmd.Parameters.AddWithValue("CNV_DATA_APPUNTAMENTO", dataAppuntamento)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                        Return True
                    End If

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return False

        End Function


        ''' <summary>
        ''' Restituisce true se esiste un appuntamento per il paziente nella data e ora specificate
        ''' </summary>
        ''' <param name="dataAppuntamento"></param>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        Public Function ExistsAppuntamentoPaziente(dataAppuntamento As Date, codicePaziente As Long) As Boolean Implements IAppuntamentiGiornoProvider.ExistsAppuntamentoPaziente

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleCommand("select 1 from T_CNV_CONVOCAZIONI where CNV_PAZ_CODICE = :CNV_PAZ_CODICE and CNV_DATA_APPUNTAMENTO = :CNV_DATA_APPUNTAMENTO ", Connection)

                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("CNV_PAZ_CODICE", codicePaziente)
                    cmd.Parameters.AddWithValue("CNV_DATA_APPUNTAMENTO", dataAppuntamento)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                        Return True
                    End If

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return False

        End Function
        ''' <summary>
        ''' Restituisce S o N in base alla valutazione di una prenotazione, se una prenotazione è annullabile alolora la funzionalità restituisce il codice S.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataConv"></param>
        ''' <returns></returns>
        Public Function GetPrenotazioneAnnullabileEsterni(codicePaziente As Integer, dataConv As Date) As String Implements IAppuntamentiGiornoProvider.GetPrenotazioneAnnullabileEsterni

            Dim result As String = String.Empty

            Dim query As String =
                "SELECT MIN(ASS_ANNULLABILE_PRENO_ESTERNI) " +
                "FROM T_CNV_CONVOCAZIONI " +
                "LEFT JOIN T_VAC_PROGRAMMATE ON CNV_PAZ_CODICE = VPR_PAZ_CODICE AND CNV_DATA = VPR_CNV_DATA " +
                "LEFT JOIN T_ANA_ASSOCIAZIONI ON VPR_ASS_CODICE = ASS_CODICE " +
                "WHERE CNV_PAZ_CODICE = :CNV_PAZ_CODICE1 AND CNV_DATA = :CNV_DATA1 "

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleCommand(query, Connection)

                    cmd.Parameters.AddWithValue("CNV_PAZ_CODICE1", codicePaziente)
                    cmd.Parameters.AddWithValue("CNV_DATA1", dataConv)

                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                        result = obj.ToString()
                    End If

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return result

        End Function

        Public Function GetDatiPropostaVariazione(idConvocazione As String) As DTORevoca

            Dim result As DTORevoca = New DTORevoca()

            Dim ownConnection As Boolean = False

            Dim query As String = "SELECT CNV_PAZ_CODICE, CNV_DATA, CNV_DATA_APPUNTAMENTO, CNV_DURATA_APPUNTAMENTO, CNV_AMB_CODICE, CNV_CNS_CODICE " +
                                  "FROM T_CNV_CONVOCAZIONI " +
                                  "WHERE CNV_ID_CONVOCAZIONE = :CNV_ID_CONVOCAZIONE"
            Try
                Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                    cmd.Parameters.AddWithValue("CNV_ID_CONVOCAZIONE", idConvocazione)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()

                        Dim cnv_paz_codice As Integer = _context.GetOrdinal("CNV_PAZ_CODICE")
                        Dim cnv_data As Integer = _context.GetOrdinal("CNV_DATA")
                        Dim cnv_data_appuntamento As Integer = _context.GetOrdinal("CNV_DATA_APPUNTAMENTO")
                        Dim cnv_durata_appuntamento As Integer = _context.GetOrdinal("CNV_DURATA_APPUNTAMENTO")
                        Dim cnv_ambulatorio_codice As Integer = _context.GetOrdinal("CNV_AMB_CODICE")
                        Dim cnv_codice_consultorio As Integer = _context.GetOrdinal("CNV_CNS_CODICE")


                        If _context.Read() Then

                            result.CodicePaziente = _context.GetInt32OrDefault(cnv_paz_codice)
                            result.DataConvocazione = _context.GetDateTimeOrDefault(cnv_data)
                            result.DataAppuntamento = _context.GetDateTimeOrDefault(cnv_data_appuntamento)
                            result.DurataAppuntamento = _context.GetDoubleOrDefault(cnv_durata_appuntamento)
                            result.CodiceAmbulatorio = _context.GetInt32OrDefault(cnv_ambulatorio_codice)
                            result.CodiceConsultorioAppuntamento = _context.GetStringOrDefault(cnv_codice_consultorio)

                        End If
                    End Using
                End Using

            Catch ex As Exception
                ' TODO [API]: PERCHE' NASCONDI L'ECCEZIONE SENZA FARE NIENTE??? Io toglierei il catch...

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return result

        End Function
        ''' <summary>
        ''' Query per l'estrazione degli appuntamenti del paziente.
        ''' </summary>
        ''' <param name="filterByDataAppuntamento"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetQueryAppuntamentiPaziente(filterByDataAppuntamento As Boolean) As String

            Dim s As New System.Text.StringBuilder()

            s.Append("select cnv_paz_codice, cnv_data, cnv_data_appuntamento, cnv_cns_codice, cnv_id_convocazione, ")
            s.Append("       cns_descrizione, cns_indirizzo, cns_n_telefono, cns_stampa2, ")
            s.Append("       cnv_amb_codice, amb_descrizione, cns_com_codice, com_descrizione, ")
            s.Append("       vpr_vac_codice, vpr_n_richiamo, vac_descrizione, ")
            s.Append("       paz_cognome, paz_nome, paz_data_nascita, vpr_ass_codice, ass_descrizione ")
            s.Append(" from t_cnv_convocazioni ")
            s.Append("    join t_vac_programmate on cnv_paz_codice = vpr_paz_codice and cnv_data = vpr_cnv_data ")
            s.Append("    join t_ana_vaccinazioni on vpr_vac_codice = vac_codice ")
            s.Append("    join t_ana_associazioni on vpr_ass_codice = ass_codice ")
            s.Append("    join t_paz_pazienti on cnv_paz_codice = paz_codice ")
            s.Append("    left join t_ana_consultori on cnv_cns_codice = cns_codice ")
            s.Append("    left join t_ana_ambulatori on cnv_amb_codice = amb_codice ")
            s.Append("    left join t_ana_comuni on cns_com_codice = com_codice ")
            s.Append("where cnv_paz_codice {0} ")
            s.Append("and not cnv_data_appuntamento is null ")

            If filterByDataAppuntamento Then
                s.Append("and cnv_data_appuntamento = :dataAppuntamento ")
            Else
                ' TODO [OnVacApp]: filtro data appuntamento non passata
                's.Append("and cnv_data_appuntamento > sysdate - 1 ")
            End If

            s.Append("order by cnv_paz_codice, cnv_data_appuntamento ")

            Return s.ToString()

        End Function
        Private Function GetQueryAppuntamentiPazienteApi(filterByDataAppuntamento As Boolean) As String

            Dim s As New Text.StringBuilder()


            s.Append("select a.*, ")
            s.Append("       (select nvl(max(NOC_COSTO_UNITARIO), 0) ")
            s.Append("        from T_ANA_LINK_NOC_ASSOCIAZIONI ")
            s.Append("        join T_ANA_NOMI_COMMERCIALI ON NAL_NOC_CODICE = NOC_CODICE ")
            s.Append("        left join T_NOC_CONDIZIONI_PAGAMENTO ON NOC_CODICE = CPG_NOC_CODICE ")
            s.Append("        where NAL_ASS_CODICE = a.vpr_ass_codice ")
            s.Append("        and NOC_OBSOLETO = 'N' ")
            s.Append("        and (NOC_SESSO = 'E' OR NOC_SESSO = a.paz_sesso) ")
            s.Append("        and (NOC_ETA_INIZIO IS NULL OR NOC_ETA_INIZIO <= a.gg_paz) ")
            s.Append("        and (NOC_ETA_FINE IS NULL OR NOC_ETA_FINE >= a.gg_paz) ")
            s.Append("        and (CPG_DA_ETA is null OR CPG_DA_ETA <= a.gg_paz) ")
            s.Append("        and (CPG_A_ETA IS NULL OR CPG_A_ETA >= a.gg_paz)) importo ")
            s.Append("from ( ")
            s.Append("select cnv_paz_codice, cnv_data, cnv_data_appuntamento, cnv_cns_codice, cnv_id_convocazione, ")
            s.Append("       cns_descrizione, cns_indirizzo, cns_n_telefono, cns_stampa2, ")
            s.Append("       cnv_amb_codice, amb_descrizione, cns_com_codice, com_descrizione, ")
            s.Append("       vpr_vac_codice, vpr_n_richiamo, vac_descrizione, ")
            s.Append("       cnv_durata_appuntamento, paz_usl_codice_assistenza, ")
            s.Append("       vpr_ass_codice, ")
            s.Append("       paz_cognome, paz_nome, paz_sesso, paz_data_nascita, ass_descrizione, ")
            s.Append("       (select trunc(sysdate) - trunc(paz_data_nascita) from dual) gg_paz ")
            s.Append(" from t_cnv_convocazioni ")
            s.Append("    join t_vac_programmate on cnv_paz_codice = vpr_paz_codice and cnv_data = vpr_cnv_data ")
            s.Append("    join t_ana_vaccinazioni on vpr_vac_codice = vac_codice ")
            s.Append("    left join t_ana_associazioni on vpr_ass_codice = ass_codice ")
            s.Append("    join t_paz_pazienti on cnv_paz_codice = paz_codice ")
            s.Append("    left join t_ana_consultori on cnv_cns_codice = cns_codice ")
            s.Append("    left join t_ana_ambulatori on cnv_amb_codice = amb_codice ")
            s.Append("    left join t_ana_comuni on cns_com_codice = com_codice ")
            s.Append("where cnv_paz_codice {0} ")
            s.Append("and not cnv_data_appuntamento is null ) a ")

            If filterByDataAppuntamento Then
                s.Append("and cnv_data_appuntamento = :dataAppuntamento ")
            Else
                ' TODO [OnVacApp]: filtro data appuntamento non passata
                's.Append("and cnv_data_appuntamento > sysdate - 1 ")
            End If

            s.Append("order by cnv_paz_codice, cnv_data_appuntamento ")

            Return s.ToString()

        End Function
        Private Function GetListAppuntamenti(cmd As OracleClient.OracleCommand) As List(Of Appuntamento)

            Dim listAppuntamenti As New List(Of Appuntamento)()

            Using idr As IDataReader = cmd.ExecuteReader()

                If Not idr Is Nothing Then

                    Dim cnv_paz_codice As Integer = idr.GetOrdinal("cnv_paz_codice")
                    Dim cnv_data As Integer = idr.GetOrdinal("cnv_data")
                    Dim cnv_id_convocazione As Integer = idr.GetOrdinal("cnv_id_convocazione")
                    Dim cnv_data_appuntamento As Integer = idr.GetOrdinal("cnv_data_appuntamento")
                    Dim cnv_cns_codice As Integer = idr.GetOrdinal("cnv_cns_codice")
                    Dim cns_descrizione As Integer = idr.GetOrdinal("cns_descrizione")
                    Dim cns_indirizzo As Integer = idr.GetOrdinal("cns_indirizzo")
                    Dim com_descrizione As Integer = idr.GetOrdinal("com_descrizione")
                    Dim cns_n_telefono As Integer = idr.GetOrdinal("cns_n_telefono")
                    Dim cns_stampa2 As Integer = idr.GetOrdinal("cns_stampa2")
                    Dim cnv_amb_codice As Integer = idr.GetOrdinal("cnv_amb_codice")
                    Dim amb_descrizione As Integer = idr.GetOrdinal("amb_descrizione")
                    Dim vpr_vac_codice As Integer = idr.GetOrdinal("vpr_vac_codice")
                    Dim vac_descrizione As Integer = idr.GetOrdinal("vac_descrizione")
                    Dim vpr_n_richiamo As Integer = idr.GetOrdinal("vpr_n_richiamo")
                    Dim paz_cognome As Integer = idr.GetOrdinal("paz_cognome")
                    Dim paz_nome As Integer = idr.GetOrdinal("paz_nome")
                    Dim paz_data_nascita As Integer = idr.GetOrdinal("paz_data_nascita")
                    Dim vpr_ass_codice As Integer = idr.GetOrdinal("vpr_ass_codice")
                    Dim ass_descrizione As Integer = idr.GetOrdinal("ass_descrizione")

                    While idr.Read()

                        Dim app As New Appuntamento()

                        app.DataConvocazione = idr.GetDateTime(cnv_data)
                        app.IdConvocazione = idr.GetStringOrDefault(cnv_id_convocazione)
                        app.DataAppuntamento = idr.GetDateTime(cnv_data_appuntamento)
                        app.CodiceConsultorio = idr.GetStringOrDefault(cnv_cns_codice)
                        app.DescrizioneConsultorio = idr.GetStringOrDefault(cns_descrizione)
                        app.IndirizzoConsultorio = idr.GetStringOrDefault(cns_indirizzo)
                        app.ComuneConsultorio = idr.GetStringOrDefault(com_descrizione)
                        app.TelefonoConsultorio = idr.GetStringOrDefault(cns_n_telefono)
                        app.NoteConsultorio = idr.GetStringOrDefault(cns_stampa2)
                        app.CodiceAmbulatorio = idr.GetInt32OrDefault(cnv_amb_codice)
                        app.DescrizioneAmbulatorio = idr.GetStringOrDefault(amb_descrizione)
                        app.CodiceVaccinazione = idr.GetStringOrDefault(vpr_vac_codice)
                        app.DescrizioneVaccinazione = idr.GetStringOrDefault(vac_descrizione)
                        app.DoseVaccinazione = idr.GetInt32OrDefault(vpr_n_richiamo)
                        app.CodiceLocalePaziente = idr.GetInt64(cnv_paz_codice)
                        app.CognomePaziente = idr.GetStringOrDefault(paz_cognome)
                        app.NomePaziente = idr.GetStringOrDefault(paz_nome)
                        app.DataNascitaPaziente = idr.GetDateTimeOrDefault(paz_data_nascita)
                        app.CodiceAssociazione = idr.GetStringOrDefault(vpr_ass_codice)
                        app.DescrizioneAssociazione = idr.GetStringOrDefault(ass_descrizione)

                        listAppuntamenti.Add(app)

                    End While

                End If

            End Using

            Return listAppuntamenti

        End Function

        Public Function GetDatiAppuntamentoStampa(idConvocazione As String) As DTOAppuntamentoStampa Implements IAppuntamentiGiornoProvider.GetDatiAppuntamentoStampa

            Dim ownConnection As Boolean = False
            Dim app As New DTOAppuntamentoStampa()

            Try
                Using cmd As New OracleClient.OracleCommand()

                    cmd.Connection = Connection

                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.CommandText = "select CNS_AZI_CODICE, CNV_PAZ_CODICE, CNV_DATA, CNV_DATA_APPUNTAMENTO, CNV_ID_CONVOCAZIONE " +
                                      "from T_CNV_CONVOCAZIONI " +
                                      "join T_ANA_CONSULTORI on CNV_CNS_CODICE = CNS_CODICE " +
                                      "where CNV_ID_CONVOCAZIONE = :CNV_ID_CONVOCAZIONE "

                    cmd.Parameters.AddWithValue("CNV_ID_CONVOCAZIONE", idConvocazione)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim CNS_AZI_CODICE As Integer = idr.GetOrdinal("CNS_AZI_CODICE")
                            Dim CNV_PAZ_CODICE As Integer = idr.GetOrdinal("CNV_PAZ_CODICE")
                            Dim CNV_DATA As Integer = idr.GetOrdinal("CNV_DATA")
                            Dim CNV_DATA_APPUNTAMENTO As Integer = idr.GetOrdinal("CNV_DATA_APPUNTAMENTO")
                            Dim CNV_ID_CONVOCAZIONE As Integer = idr.GetOrdinal("CNV_ID_CONVOCAZIONE")

                            If idr.Read() Then
                                app.CodiceUsl = idr.GetStringOrDefault(CNS_AZI_CODICE)
                                app.CodicePaziente = idr.GetInt64OrDefault(CNV_PAZ_CODICE)
                                app.DataConvocazione = idr.GetDateTimeOrDefault(CNV_DATA)
                                app.DataAppuntamento = idr.GetDateTimeOrDefault(CNV_DATA_APPUNTAMENTO)
                                app.IdConvocazione = idr.GetStringOrDefault(CNV_ID_CONVOCAZIONE)
                            End If
                        End If

                    End Using

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return app

        End Function

        Public Function InserisciLockAppuntamento(CodiceAmb As Long, DataAppuntamento As DateTime) As Integer Implements IAppuntamentiGiornoProvider.InserisciLockAppuntamento

            Dim count As Integer = 0

            Dim ownConnection As Boolean = False

            Try
                Using cmd As New OracleCommand()

                    cmd.Connection = Connection

                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim filtro As String = String.Empty

                    Dim s As New Text.StringBuilder()

                    s.Append("insert into T_LOCK_APPUNTAMENTI (LAP_AMB_CODICE, LAP_DATA_APPUNTAMENTO) ")
                    s.Append("values (:LAP_AMB_CODICE, :LAP_DATA_APPUNTAMENTO)")


                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("LAP_AMB_CODICE", CodiceAmb)
                    cmd.Parameters.AddWithValue("LAP_DATA_APPUNTAMENTO", DataAppuntamento)

                    cmd.CommandText = s.ToString()

                    count = cmd.ExecuteNonQuery()

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return count
        End Function

        Public Function GetLockAppuntamento(DataAppuntamento As DateTime, CodiceAmbulatorio As Long, Nparametro As Integer) As LockAppuntamento Implements IAppuntamentiGiornoProvider.GetLockAppuntamento

            Dim ownConnection As Boolean = False
            Dim app As LockAppuntamento = Nothing
            Try
                Using cmd As New OracleClient.OracleCommand()

                    cmd.Connection = Connection

                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.CommandText = "select LAP_AMB_CODICE, LAP_DATA_APPUNTAMENTO " +
                                      "from T_LOCK_APPUNTAMENTI " +
                                      "where LAP_DATA_APPUNTAMENTO = :LAP_DATA_APPUNTAMENTO AND LAP_AMB_CODICE = :LAP_AMB_CODICE "

                    If Nparametro > 0 Then
                        cmd.CommandText += "AND LAP_DATA_INSERIMENTO >= sysdate - (1/1440 * :NMinuti)"
                        cmd.Parameters.AddWithValue("NMinuti", Nparametro)
                    End If
                    'NPARAMETRO
                    cmd.Parameters.AddWithValue("LAP_DATA_APPUNTAMENTO", DataAppuntamento)
                    cmd.Parameters.AddWithValue("LAP_AMB_CODICE", CodiceAmbulatorio)


                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim LAP_DATA_APPUNTAMENTO As Integer = idr.GetOrdinal("LAP_DATA_APPUNTAMENTO")
                            Dim LAP_AMB_CODICE As Integer = idr.GetOrdinal("LAP_AMB_CODICE")

                            If idr.Read() Then
                                app = New LockAppuntamento()
                                app.DataAppuntamento = idr.GetDateTimeOrDefault(LAP_DATA_APPUNTAMENTO)
                                app.CodiceAmbulatorio = idr.GetInt64OrDefault(LAP_AMB_CODICE)

                            End If
                        End If

                    End Using

                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return app
        End Function
#End Region

    End Class

End Namespace
