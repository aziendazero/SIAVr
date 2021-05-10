Imports System.Collections.Generic
Imports System.Data.OracleClient
Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.Data.OracleClient
Imports Onit.OnAssistnet.OnVac.Filters

Namespace DAL.Oracle

    Public Class DbPazienteProvider
        Inherits DbProvider
        Implements IPazienteProvider

#Region " Constructors "

        Public Sub New(ByRef DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " Metodi di Select "

#Region " CollectionPazienti "

        ''' <summary>
        ''' Restituisce i dati dei pazienti specificati nel parametro (stringa, separata da virgole, contenente i codici dei pazienti da restituire). 
        ''' </summary>
        ''' <param name="elencoCodiciPazienti">Codici paziente per la query di in</param>
        ''' <remarks>
        ''' </remarks>
        Public Function GetPazienti(elencoCodiciPazienti As String) As PazienteCollection Implements IPazienteProvider.GetPazienti

            Return GetPazienti(elencoCodiciPazienti, String.Empty)

        End Function


        ' [Unificazione Ulss]: qui il codiceUsl passato deve esserci se no scoppia quando viene fatto il BuildDataReader() del DAM!
        Public Function GetPazienti(elencoCodiciPazienti As String, codiceUsl As String) As PazienteCollection Implements IPazienteProvider.GetPazienti

            RefurbishDT()   ' pulisce il datatable della classe base, anche se poi non lo usa

            Dim collPaz As PazienteCollection

            With _DAM.QB

                .NewQuery()

                SetQueryRicercaPazienti(_DAM.QB, codiceUsl)

                If elencoCodiciPazienti.Contains(",") Then
                    ' elencoCodiciPazienti contiene più codici --> filtro di "IN" con codici interi (nessun problema con la In del Dam)
                    .AddWhereCondition("paz_codice", Comparatori.In, elencoCodiciPazienti, DataTypes.Replace)
                Else
                    ' elencoCodiciPazienti contiene un codice solo --> filtro di "=" anzichè di "IN"
                    .AddWhereCondition("paz_codice", Comparatori.Uguale, elencoCodiciPazienti, DataTypes.Numero)
                End If

            End With

            Try
                Using dr As IDataReader = _DAM.BuildDataReader()
                    collPaz = GetCollectionPazienti(dr)
                End Using

            Catch ex As Exception
                SetErrorMsg("Errore durante il recupero dei pazienti selezionati")
                ex.InternalPreserveStackTrace()
                Throw
            End Try

            Return collPaz

        End Function

        ''' <summary>
        ''' Restituisce una collection di pazienti effettuando la ricerca in base ai filtri specificati.
        ''' Vengono restituiti i pazienti che corrispondono ai filtri impostati ed aventi codice ausiliario nullo.
        ''' I filtri lasciati nulli non verranno considerati nella query.
        ''' </summary>
        ''' <param name="filtriRiconduzione"></param>
        ''' <param name="codiceUsl"></param>
        ''' <returns></returns>
        Public Function GetPazientiRiconduzione(filtriRiconduzione As FiltriRiconduzionePazienti, codiceUsl As String) As PazienteCollection Implements IPazienteProvider.GetPazientiRiconduzione

            RefurbishDT()   ' pulisce il datatable della classe base, anche se poi non lo usa

            Dim collPaz As PazienteCollection

            With _DAM.QB

                .NewQuery()

                SetQueryRicercaPazienti(_DAM.QB, codiceUsl)

                If filtriRiconduzione.FlagCodiceAusiliarioNullo Then
                    .AddWhereCondition("PAZ_CODICE_AUSILIARIO", Comparatori.Is, "null", DataTypes.Replace)
                End If

                If Not String.IsNullOrEmpty(filtriRiconduzione.Cognome) Then
                    .AddWhereCondition("PAZ_COGNOME", Comparatori.Uguale, filtriRiconduzione.Cognome, DataTypes.Stringa)
                End If
                If Not String.IsNullOrEmpty(filtriRiconduzione.Nome) Then
                    .AddWhereCondition("PAZ_NOME", Comparatori.Uguale, filtriRiconduzione.Nome, DataTypes.Stringa)
                End If
                If Not String.IsNullOrEmpty(filtriRiconduzione.CodiceFiscale) Then
                    .AddWhereCondition("PAZ_CODICE_FISCALE", Comparatori.Uguale, filtriRiconduzione.CodiceFiscale, DataTypes.Stringa)
                End If
                If Not String.IsNullOrEmpty(filtriRiconduzione.Tessera) Then
                    .AddWhereCondition("PAZ_TESSERA", Comparatori.Uguale, filtriRiconduzione.Tessera, DataTypes.Stringa)
                End If
                If Not filtriRiconduzione.DataNascita Is Nothing AndAlso filtriRiconduzione.DataNascita > Date.MinValue Then
                    .AddWhereCondition("PAZ_DATA_NASCITA", Comparatori.Uguale, filtriRiconduzione.DataNascita, DataTypes.Data)
                End If
                If Not String.IsNullOrEmpty(filtriRiconduzione.CodiceComuneNascita) Then
                    .AddWhereCondition("PAZ_COM_CODICE_NASCITA", Comparatori.Uguale, filtriRiconduzione.CodiceComuneNascita, DataTypes.Stringa)
                End If
                If Not String.IsNullOrEmpty(filtriRiconduzione.Sesso) Then
                    .AddWhereCondition("PAZ_SESSO", Comparatori.Uguale, filtriRiconduzione.Sesso, DataTypes.Stringa)
                End If
                If Not String.IsNullOrEmpty(filtriRiconduzione.CodiceComuneResidenza) Then
                    .AddWhereCondition("PAZ_COM_CODICE_RESIDENZA", Comparatori.Uguale, filtriRiconduzione.CodiceComuneResidenza, DataTypes.Stringa)
                End If
                If Not String.IsNullOrEmpty(filtriRiconduzione.CodiceComuneDomicilio) Then
                    .AddWhereCondition("PAZ_COM_CODICE_DOMICILIO", Comparatori.Uguale, filtriRiconduzione.CodiceComuneDomicilio, DataTypes.Stringa)
                End If
                If Not String.IsNullOrEmpty(filtriRiconduzione.CodiceCittadinanza) Then
                    .AddWhereCondition("PAZ_CIT_CODICE", Comparatori.Uguale, filtriRiconduzione.CodiceCittadinanza, DataTypes.Stringa)
                End If

            End With

            Try
                Using dr As IDataReader = _DAM.BuildDataReader()
                    collPaz = GetCollectionPazienti(dr)
                End Using

            Catch ex As Exception

                SetErrorMsg("Errore durante il recupero dei pazienti per la riconduzione")

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return collPaz

        End Function

        Private Function GetNotaPaziente(qb As AbstractQB, codiceUsl As String, codiceTipoNota As String) As String

            If String.IsNullOrWhiteSpace(codiceUsl) Then
                Return "NULL"
            End If

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

        Private Sub SetQueryRicercaPazienti(qb As AbstractQB, codiceUsl As String)

            Dim queryNotaSolleciti As String = GetNotaPaziente(qb, codiceUsl, Constants.CodiceTipoNotaPaziente.Solleciti)
            Dim queryNotaLibero1 As String = GetNotaPaziente(qb, codiceUsl, Constants.CodiceTipoNotaPaziente.Appuntamenti)
            Dim queryNotaLibero2 As String = GetNotaPaziente(qb, codiceUsl, Constants.CodiceTipoNotaPaziente.MalattiePregresse)
            Dim queryNotaLibero3 As String = GetNotaPaziente(qb, codiceUsl, Constants.CodiceTipoNotaPaziente.Esclusioni)
            Dim queryNotaCertificato As String = GetNotaPaziente(qb, codiceUsl, Constants.CodiceTipoNotaPaziente.Certificato)
            Dim queryNotaAnnotazioni As String = GetNotaPaziente(qb, codiceUsl, Constants.CodiceTipoNotaPaziente.Annotazioni)

            With qb
                .NewQuery(False, False)

                .AddSelectFields("paz_codice, paz_cognome, paz_nome, paz_data_nascita, paz_codice_fiscale")
                .AddSelectFields("paz_indirizzo_residenza, paz_indirizzo_domicilio, paz_sesso")
                .AddSelectFields("paz_med_codice_base, med_descrizione, paz_cns_codice")
                .AddSelectFields("res.com_codice res_codice, res.com_descrizione res_descrizione, res.com_provincia res_prv")
                ' Cap di residenza preso dai dati del paziente oppure, se nullo, dalla t_ana_comuni
                .AddSelectFields("(" + .FC.Switch("paz_cap_residenza", "null", "res.com_cap", "paz_cap_residenza") + ") res_cap")
                .AddSelectFields("dom.com_codice dom_codice, dom.com_descrizione dom_descrizione, dom.com_provincia dom_prv")
                ' Cap di domicilio preso dai dati del paziente oppure, se nullo, dalla t_ana_comuni
                .AddSelectFields("(" + .FC.Switch("paz_cap_domicilio", "null", "dom.com_cap", "paz_cap_domicilio") + ") dom_cap")

                .AddSelectFields("paz_flag_cessato, paz_aire, paz_cir_codice, paz_cir_codice_2, paz_cit_codice, cit_stato")
                .AddSelectFields("paz_cns_codice_old, paz_cns_data_assegnazione")
                .AddSelectFields("paz_cns_terr_codice, paz_codice_ausiliario, paz_codice_regionale, paz_com_codice_nascita")
                .AddSelectFields("paz_com_comune_emigrazione, paz_com_comune_provenienza, paz_data_agg_da_anag")
                .AddSelectFields("paz_data_aire, paz_data_decesso, paz_data_decorrenza_med, paz_data_emigrazione")
                .AddSelectFields("paz_data_immigrazione, paz_data_inserimento, paz_data_irreperibilita, paz_data_revoca_med")
                .AddSelectFields("paz_data_scadenza_med, paz_dis_codice, paz_irreperibile, paz_locale, paz_regolarizzato")
                .AddSelectFields("paz_stato, paz_stato_anagrafico, paz_telefono_1, paz_telefono_2, paz_telefono_3, paz_email")
                .AddSelectFields("paz_tessera, paz_usl_codice_assistenza, paz_usl_codice_residenza")
                .AddSelectFields("paz_anonimo, paz_azi_codice, paz_cat_codice, paz_cag_codice, paz_cancellato")
                .AddSelectFields("paz_codice_demografico, paz_completare, paz_data_aggiornamento, paz_data_agg_da_comune")
                .AddSelectFields("paz_data_cancellazione, paz_flag_stampa_maggiorenni, paz_giorno, paz_ind_codice_res")
                .AddSelectFields("paz_ind_codice_dom")
                .AddSelectFields("(" + queryNotaLibero1 + ") paz_libero_1")
                .AddSelectFields("(" + queryNotaLibero2 + ") paz_libero_2")
                .AddSelectFields("(" + queryNotaLibero3 + ") paz_libero_3")
                .AddSelectFields("paz_padre, paz_madre")
                .AddSelectFields("(" + queryNotaAnnotazioni + ") paz_note")
                .AddSelectFields("(" + queryNotaCertificato + ") paz_note_certificato")
                .AddSelectFields("paz_occasionale, paz_plb_id, paz_posizione_vaccinale_ok")
                .AddSelectFields("paz_reg_assistenza, paz_richiesta_cartella, paz_richiesta_certificato, paz_rsc_codice")
                .AddSelectFields("paz_stato_acquisizione_imi, paz_stato_anagrafico_dett, paz_stato_notifica_emi")
                .AddSelectFields("paz_sta_certificato_emi, paz_tipo, paz_tipo_occasionalita, paz_livello_certificazione")
                .AddSelectFields("paz_codice_esterno, paz_data_scadenza_ssn, PAZ_STATO_ACQUISIZIONE, paz_categoria_cittadino")
                .AddSelectFields("paz_id_acn, paz_motivo_cessazione_assist")
                .AddSelectFields("(" + queryNotaSolleciti + ") PAZ_NOTE_SOLLECITI")

                .AddTables("t_paz_pazienti, t_ana_medici, t_ana_comuni res, t_ana_comuni dom, t_ana_cittadinanze")

                .AddWhereCondition("paz_med_codice_base", Comparatori.Uguale, "med_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("paz_com_codice_residenza", Comparatori.Uguale, "res.com_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("paz_com_codice_domicilio", Comparatori.Uguale, "dom.com_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("paz_cit_codice", Comparatori.Uguale, "cit_codice", DataTypes.OutJoinLeft)

            End With

        End Sub

        Private Function GetCollectionPazienti(dr As IDataReader) As PazienteCollection

            Dim collPaz As New PazienteCollection()

            If Not dr Is Nothing Then

                Dim paz_codice As Integer = dr.GetOrdinal("paz_codice")
                Dim paz_cognome As Integer = dr.GetOrdinal("paz_cognome")
                Dim paz_nome As Integer = dr.GetOrdinal("paz_nome")
                Dim paz_data_nascita As Integer = dr.GetOrdinal("paz_data_nascita")
                Dim paz_codice_fiscale As Integer = dr.GetOrdinal("paz_codice_fiscale")
                Dim paz_indirizzo_residenza As Integer = dr.GetOrdinal("paz_indirizzo_residenza")
                Dim paz_indirizzo_domicilio As Integer = dr.GetOrdinal("paz_indirizzo_domicilio")
                Dim paz_sesso As Integer = dr.GetOrdinal("paz_sesso")
                Dim paz_med_codice_base As Integer = dr.GetOrdinal("paz_med_codice_base")
                Dim med_descrizione As Integer = dr.GetOrdinal("med_descrizione")
                Dim paz_cns_codice As Integer = dr.GetOrdinal("paz_cns_codice")
                Dim res_codice As Integer = dr.GetOrdinal("res_codice")
                Dim res_descrizione As Integer = dr.GetOrdinal("res_descrizione")
                Dim res_prv As Integer = dr.GetOrdinal("res_prv")
                Dim res_cap As Integer = dr.GetOrdinal("res_cap")
                Dim dom_codice As Integer = dr.GetOrdinal("dom_codice")
                Dim dom_descrizione As Integer = dr.GetOrdinal("dom_descrizione")
                Dim dom_prv As Integer = dr.GetOrdinal("dom_prv")
                Dim dom_cap As Integer = dr.GetOrdinal("dom_cap")
                '  
                Dim paz_flag_cessato As Integer = dr.GetOrdinal("paz_flag_cessato")
                Dim paz_aire As Integer = dr.GetOrdinal("paz_aire")
                Dim paz_cir_codice As Integer = dr.GetOrdinal("paz_cir_codice")
                Dim paz_cir_codice_2 As Integer = dr.GetOrdinal("paz_cir_codice_2")
                Dim paz_cit_codice As Integer = dr.GetOrdinal("paz_cit_codice")
                Dim cit_stato As Integer = dr.GetOrdinal("cit_stato")
                Dim paz_cns_codice_old As Integer = dr.GetOrdinal("paz_cns_codice_old")
                Dim paz_cns_data_assegnazione As Integer = dr.GetOrdinal("paz_cns_data_assegnazione")
                Dim paz_cns_terr_codice As Integer = dr.GetOrdinal("paz_cns_terr_codice")
                Dim paz_codice_ausiliario As Integer = dr.GetOrdinal("paz_codice_ausiliario")
                Dim paz_codice_regionale As Integer = dr.GetOrdinal("paz_codice_regionale")
                Dim paz_com_codice_nascita As Integer = dr.GetOrdinal("paz_com_codice_nascita")
                Dim paz_com_comune_emigrazione As Integer = dr.GetOrdinal("paz_com_comune_emigrazione")
                Dim paz_com_comune_provenienza As Integer = dr.GetOrdinal("paz_com_comune_provenienza")
                Dim paz_data_agg_da_anag As Integer = dr.GetOrdinal("paz_data_agg_da_anag")
                Dim paz_data_aire As Integer = dr.GetOrdinal("paz_data_aire")
                Dim paz_data_decesso As Integer = dr.GetOrdinal("paz_data_decesso")
                Dim paz_data_decorrenza_med As Integer = dr.GetOrdinal("paz_data_decorrenza_med")
                Dim paz_data_emigrazione As Integer = dr.GetOrdinal("paz_data_emigrazione")
                Dim paz_data_immigrazione As Integer = dr.GetOrdinal("paz_data_immigrazione")
                Dim paz_data_inserimento As Integer = dr.GetOrdinal("paz_data_inserimento")
                Dim paz_data_irreperibilita As Integer = dr.GetOrdinal("paz_data_irreperibilita")
                Dim paz_data_revoca_med As Integer = dr.GetOrdinal("paz_data_revoca_med")
                Dim paz_data_scadenza_med As Integer = dr.GetOrdinal("paz_data_scadenza_med")
                Dim paz_dis_codice As Integer = dr.GetOrdinal("paz_dis_codice")
                Dim paz_irreperibile As Integer = dr.GetOrdinal("paz_irreperibile")
                Dim paz_locale As Integer = dr.GetOrdinal("paz_locale")
                Dim paz_regolarizzato As Integer = dr.GetOrdinal("paz_regolarizzato")
                Dim paz_stato As Integer = dr.GetOrdinal("paz_stato")
                Dim paz_stato_anagrafico As Integer = dr.GetOrdinal("paz_stato_anagrafico")
                Dim paz_telefono_1 As Integer = dr.GetOrdinal("paz_telefono_1")
                Dim paz_telefono_2 As Integer = dr.GetOrdinal("paz_telefono_2")
                Dim paz_telefono_3 As Integer = dr.GetOrdinal("paz_telefono_3")
                Dim paz_email As Integer = dr.GetOrdinal("paz_email")
                Dim paz_tessera As Integer = dr.GetOrdinal("paz_tessera")
                Dim paz_usl_codice_assistenza As Integer = dr.GetOrdinal("paz_usl_codice_assistenza")
                Dim paz_usl_codice_residenza As Integer = dr.GetOrdinal("paz_usl_codice_residenza")
                Dim paz_anonimo As Integer = dr.GetOrdinal("paz_anonimo")
                Dim paz_azi_codice As Integer = dr.GetOrdinal("paz_azi_codice")
                Dim paz_cat_codice As Integer = dr.GetOrdinal("paz_cat_codice")
                Dim paz_cag_codice As Integer = dr.GetOrdinal("paz_cag_codice")
                Dim paz_cancellato As Integer = dr.GetOrdinal("paz_cancellato")
                Dim paz_codice_demografico As Integer = dr.GetOrdinal("paz_codice_demografico")
                Dim paz_completare As Integer = dr.GetOrdinal("paz_completare")
                Dim paz_data_aggiornamento As Integer = dr.GetOrdinal("paz_data_aggiornamento")
                Dim paz_data_agg_da_comune As Integer = dr.GetOrdinal("paz_data_agg_da_comune")
                Dim paz_data_cancellazione As Integer = dr.GetOrdinal("paz_data_cancellazione")
                Dim paz_flag_stampa_maggiorenni As Integer = dr.GetOrdinal("paz_flag_stampa_maggiorenni")
                Dim paz_giorno As Integer = dr.GetOrdinal("paz_giorno")
                Dim paz_ind_codice_res As Integer = dr.GetOrdinal("paz_ind_codice_res")
                Dim paz_ind_codice_dom As Integer = dr.GetOrdinal("paz_ind_codice_dom")
                Dim paz_padre As Integer = dr.GetOrdinal("paz_padre")
                Dim paz_madre As Integer = dr.GetOrdinal("paz_madre")
                Dim paz_occasionale As Integer = dr.GetOrdinal("paz_occasionale")
                Dim paz_plb_id As Integer = dr.GetOrdinal("paz_plb_id")
                Dim paz_posizione_vaccinale_ok As Integer = dr.GetOrdinal("paz_posizione_vaccinale_ok")
                Dim paz_reg_assistenza As Integer = dr.GetOrdinal("paz_reg_assistenza")
                Dim paz_richiesta_cartella As Integer = dr.GetOrdinal("paz_richiesta_cartella")
                Dim paz_richiesta_certificato As Integer = dr.GetOrdinal("paz_richiesta_certificato")
                Dim paz_rsc_codice As Integer = dr.GetOrdinal("paz_rsc_codice")
                Dim paz_stato_acquisizione_imi As Integer = dr.GetOrdinal("paz_stato_acquisizione_imi")
                Dim paz_stato_anagrafico_dett As Integer = dr.GetOrdinal("paz_stato_anagrafico_dett")
                Dim paz_stato_notifica_emi As Integer = dr.GetOrdinal("paz_stato_notifica_emi")
                Dim paz_sta_certificato_emi As Integer = dr.GetOrdinal("paz_sta_certificato_emi")
                Dim paz_tipo As Integer = dr.GetOrdinal("paz_tipo")
                Dim paz_tipo_occasionalita As Integer = dr.GetOrdinal("paz_tipo_occasionalita")
                Dim paz_livello_certificazione As Integer = dr.GetOrdinal("paz_livello_certificazione")
                Dim paz_codice_esterno As Integer = dr.GetOrdinal("paz_codice_esterno")
                Dim paz_data_scadenza_ssn As Integer = dr.GetOrdinal("paz_data_scadenza_ssn")
                Dim PAZ_STATO_ACQUISIZIONE As Integer = dr.GetOrdinal("PAZ_STATO_ACQUISIZIONE")
                Dim paz_id_acn As Integer = dr.GetOrdinal("paz_id_acn")
                Dim paz_categoria_cittadino As Integer = dr.GetOrdinal("paz_categoria_cittadino")
                Dim paz_motivo_cessazione_assist As Integer = dr.GetOrdinal("paz_motivo_cessazione_assist")

                Dim p As Paziente

                While dr.Read()

                    p = New Paziente(dr.GetDecimal(paz_codice))

                    p.PAZ_COGNOME = dr(paz_cognome).ToString()
                    p.PAZ_NOME = dr(paz_nome).ToString()
                    p.Data_Nascita = dr.GetDateTimeOrDefault(paz_data_nascita)
                    p.PAZ_CODICE_FISCALE = dr(paz_codice_fiscale).ToString()
                    p.Sesso = dr(paz_sesso).ToString()

                    p.IndirizzoResidenza = dr(paz_indirizzo_residenza).ToString()
                    p.ComuneResidenza_Codice = dr(res_codice).ToString()
                    p.ComuneResidenza_Descrizione = dr(res_descrizione).ToString()
                    p.ComuneResidenza_Provincia = dr(res_prv).ToString()
                    p.ComuneResidenza_Cap = dr(res_cap).ToString()

                    p.IndirizzoDomicilio = dr(paz_indirizzo_domicilio).ToString()
                    p.ComuneDomicilio_Codice = dr(dom_codice).ToString()
                    p.ComuneDomicilio_Descrizione = dr(dom_descrizione).ToString()
                    p.ComuneDomicilio_Provincia = dr(dom_prv).ToString()
                    p.ComuneDomicilio_Cap = dr(dom_cap).ToString()

                    p.MedicoBase_Codice = dr(paz_med_codice_base).ToString()
                    p.MedicoBase_Descrizione = dr(med_descrizione).ToString()
                    p.Paz_Cns_Codice = dr(paz_cns_codice).ToString()

                    p.FlagCessato = dr.GetStringOrDefault(paz_flag_cessato)
                    p.FlagAire = dr.GetStringOrDefault(paz_aire)
                    p.Circoscrizione_Codice = dr.GetStringOrDefault(paz_cir_codice)
                    p.Circoscrizione2_Codice = dr.GetStringOrDefault(paz_cir_codice_2)

                    p.Cittadinanza_Codice = dr.GetStringOrDefault(paz_cit_codice)
                    p.Cittadinanza_Descrizione = dr.GetStringOrDefault(cit_stato)

                    p.Paz_Cns_Codice_Old = dr.GetStringOrDefault(paz_cns_codice_old)
                    p.Paz_Cns_Data_Assegnazione = dr.GetNullableDateTimeOrDefault(paz_cns_data_assegnazione)
                    p.Paz_Cns_Terr_Codice = dr.GetStringOrDefault(paz_cns_terr_codice)
                    p.CodiceAusiliario = dr.GetStringOrDefault(paz_codice_ausiliario)
                    p.PAZ_CODICE_REGIONALE = dr.GetStringOrDefault(paz_codice_regionale)
                    p.ComuneNascita_Codice = dr.GetStringOrDefault(paz_com_codice_nascita)
                    p.ComuneEmigrazione_Codice = dr.GetStringOrDefault(paz_com_comune_emigrazione)
                    p.ComuneProvenienza_Codice = dr.GetStringOrDefault(paz_com_comune_provenienza)
                    p.DataAggiornamentoDaAnagrafe = dr.GetNullableDateTimeOrDefault(paz_data_agg_da_anag)

                    p.DataAire = dr.GetNullableDateTimeOrDefault(paz_data_aire)
                    p.DataDecesso = dr.GetDateTimeOrDefault(paz_data_decesso)
                    p.MedicoBase_DataDecorrenza = dr.GetDateTimeOrDefault(paz_data_decorrenza_med)
                    p.DataEmigrazione = dr.GetDateTimeOrDefault(paz_data_emigrazione)
                    p.DataImmigrazione = dr.GetDateTimeOrDefault(paz_data_immigrazione)
                    p.DataInserimento = dr.GetDateTimeOrDefault(paz_data_inserimento)
                    p.DataIrreperibilita = dr.GetNullableDateTimeOrDefault(paz_data_irreperibilita)
                    p.MedicoBase_DataRevoca = dr.GetDateTimeOrDefault(paz_data_revoca_med)
                    p.MedicoBase_DataScadenza = dr.GetDateTimeOrDefault(paz_data_scadenza_med)
                    p.Distretto_Codice = dr.GetStringOrDefault(paz_dis_codice)
                    p.FlagIrreperibilita = dr.GetStringOrDefault(paz_irreperibile)
                    p.FlagLocale = dr.GetStringOrDefault(paz_locale)
                    p.FlagRegolarizzato = dr.GetStringOrDefault(paz_regolarizzato)
                    p.Stato = dr.GetNullableEnumOrDefault(Of Enumerators.StatiVaccinali)(paz_stato)
                    p.StatoAnagrafico = dr.GetNullableEnumOrDefault(Of Enumerators.StatoAnagrafico)(paz_stato_anagrafico)
                    p.Telefono1 = dr.GetStringOrDefault(paz_telefono_1)
                    p.Telefono2 = dr.GetStringOrDefault(paz_telefono_2)
                    p.Telefono3 = dr.GetStringOrDefault(paz_telefono_3)
                    p.Email = dr.GetStringOrDefault(paz_email)
                    p.Tessera = dr.GetStringOrDefault(paz_tessera)
                    p.UslAssistenza_Codice = dr.GetStringOrDefault(paz_usl_codice_assistenza)
                    p.UslResidenza_Codice = dr.GetStringOrDefault(paz_usl_codice_residenza)
                    p.FlagAnonimo = dr.GetStringOrDefault(paz_anonimo)
                    p.CodiceAzienda = dr.GetStringOrDefault(paz_azi_codice)
                    p.CodiceCategoria1 = dr.GetStringOrDefault(paz_cat_codice)
                    p.CodiceCategoria2 = dr.GetStringOrDefault(paz_cag_codice)
                    p.FlagCancellato = dr.GetStringOrDefault(paz_cancellato)
                    p.CodiceDemografico = dr.GetStringOrDefault(paz_codice_demografico)
                    p.FlagCompletare = dr.GetStringOrDefault(paz_completare)
                    p.DataAggiornamento = dr.GetDateTimeOrDefault(paz_data_aggiornamento)
                    p.DataAggiornamentoDaComune = dr.GetDateTimeOrDefault(paz_data_agg_da_comune)
                    p.DataCancellazione = dr.GetDateTimeOrDefault(paz_data_cancellazione)
                    p.FlagStampaMaggiorenni = dr.GetStringOrDefault(paz_flag_stampa_maggiorenni)
                    p.Giorno = dr.GetStringOrDefault(paz_giorno)
                    p.CodiceIndirizzoResidenza = dr.GetNullableInt32OrDefault(paz_ind_codice_res)
                    p.CodiceIndirizzoDomicilio = dr.GetNullableInt32OrDefault(paz_ind_codice_dom)
                    p.Padre = dr.GetStringOrDefault(paz_padre)
                    p.Madre = dr.GetStringOrDefault(paz_madre)
                    p.FlagOccasionale = dr.GetStringOrDefault(paz_occasionale)
                    p.IdElaborazione = dr.GetNullableInt32OrDefault(paz_plb_id)
                    p.FlagPosizioneVaccinale = dr.GetStringOrDefault(paz_posizione_vaccinale_ok)
                    p.RegAssistenza = dr.GetStringOrDefault(paz_reg_assistenza)
                    p.FlagRichiestaCartella = dr.GetStringOrDefault(paz_richiesta_cartella)
                    p.FlagRichiestaCertificato = dr.GetStringOrDefault(paz_richiesta_certificato)
                    p.CodiceCategoriaRischio = dr.GetStringOrDefault(paz_rsc_codice)
                    p.StatoAcquisizioneImmigrazione = dr.GetStringOrDefault(paz_stato_acquisizione_imi)
                    p.StatoAnagraficoDettaglio = dr.GetStringOrDefault(paz_stato_anagrafico_dett)
                    p.StatoNotificaEmigrazione = dr.GetStringOrDefault(paz_stato_notifica_emi)
                    p.FlagStampaCertificatoEmigrazione = dr.GetStringOrDefault(paz_sta_certificato_emi)
                    p.Tipo = dr.GetStringOrDefault(paz_tipo)
                    p.TipoOccasionalita = dr.GetStringOrDefault(paz_tipo_occasionalita)
                    p.LivelloCertificazione = dr.GetInt32OrDefault(paz_livello_certificazione)
                    p.CodiceEsterno = dr.GetStringOrDefault(paz_codice_esterno)
                    p.DataScadenzaSSN = dr.GetNullableDateTimeOrDefault(paz_data_scadenza_ssn)
                    p.IdACN = dr.GetStringOrDefault(paz_id_acn)
                    p.CategoriaCittadino = dr.GetStringOrDefault(paz_categoria_cittadino)
                    p.MotivoCessazioneAssistenza = dr.GetStringOrDefault(paz_motivo_cessazione_assist)

                    If Not dr.IsDBNull(PAZ_STATO_ACQUISIZIONE) Then
                        p.StatoAcquisizioneDatiVaccinaliCentrale = dr.GetInt32(PAZ_STATO_ACQUISIZIONE)
                    End If

                    collPaz.Add(p)

                End While

            End If

            Return collPaz

        End Function

        ''' <summary>
        ''' Restituisce il valore da inserire come codice del paziente, preso dalla sequence su db
        ''' </summary>
        ''' <returns></returns>
        Private Function GetCodicePazienteDaInserire() As Integer

            Dim codPaziente As Integer = -1

            Using cmd As OracleCommand = Connection.CreateCommand()

                cmd.CommandText = OnVac.Queries.Pazienti.OracleQueries.nextValSeqPazienti

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        codPaziente = Convert.ToInt32(obj)
                    Else
                        codPaziente = -1
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return codPaziente

        End Function

#End Region

        Public Function GetCodicePazientiByCodiceAusiliario(codiceAusiliario As String) As ICollection(Of String) Implements IPazienteProvider.GetCodicePazientiByCodiceAusiliario

            Dim listCodPaz As New List(Of String)()

            Using cmd As OracleCommand = New OracleCommand(Queries.Pazienti.OracleQueries.selCodicePazientiByCodiceAusiliario, Connection)

                cmd.Parameters.AddWithValue("paz_codice_ausiliario", GetStringParam(codiceAusiliario, False))

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using dr As IDataReader = cmd.ExecuteReader()

                        If Not dr Is Nothing Then

                            Dim pos_paz_cod As Integer = dr.GetOrdinal("paz_codice")

                            While dr.Read()

                                listCodPaz.Add(dr.GetValue(pos_paz_cod).ToString())

                            End While

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listCodPaz

        End Function

        Public Function GetCodicePazientiByCodiceFiscale(codiceFiscale As String) As ICollection(Of String) Implements IPazienteProvider.GetCodicePazientiByCodiceFiscale

            Dim listCodPaz As New List(Of String)()

            Using cmd As OracleCommand = New OracleCommand(Queries.Pazienti.OracleQueries.selCodicePazientiByCodiceFiscale, Connection)

                cmd.Parameters.AddWithValue("paz_codice_fiscale", GetStringParam(codiceFiscale, False))

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using dr As IDataReader = cmd.ExecuteReader()

                        If Not dr Is Nothing Then

                            Dim pos_paz_cod As Integer = dr.GetOrdinal("paz_codice")

                            While dr.Read()

                                listCodPaz.Add(dr.GetValue(pos_paz_cod).ToString())

                            End While

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listCodPaz

        End Function

        Public Function GetCodicePazientiByCodiceRegionale(codiceRegionale As String) As ICollection(Of String) Implements IPazienteProvider.GetCodicePazientiByCodiceRegionale

            Dim listCodPaz As New List(Of String)()

            Using cmd As OracleCommand = New OracleCommand(Queries.Pazienti.OracleQueries.selCodicePazientiByCodiceRegionale, Connection)

                cmd.Parameters.AddWithValue("paz_codice_regionale", GetStringParam(codiceRegionale, False))

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using dr As IDataReader = cmd.ExecuteReader()

                        If Not dr Is Nothing Then

                            Dim pos_paz_cod As Integer = dr.GetOrdinal("paz_codice")

                            While dr.Read()

                                listCodPaz.Add(dr.GetValue(pos_paz_cod).ToString())

                            End While

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listCodPaz

        End Function

        Public Function GetCodicePazientiByTessera(tessera As String) As ICollection(Of String) Implements IPazienteProvider.GetCodicePazientiByTessera

            Dim listCodPaz As New List(Of String)()

            Using cmd As OracleCommand = New OracleCommand(Queries.Pazienti.OracleQueries.selCodicePazientiByTessera, Connection)

                cmd.Parameters.AddWithValue("paz_tessera", GetStringParam(tessera, False))

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using dr As IDataReader = cmd.ExecuteReader()

                        If Not dr Is Nothing Then

                            Dim pos_paz_cod As Integer = dr.GetOrdinal("paz_codice")

                            While dr.Read()

                                listCodPaz.Add(dr.GetValue(pos_paz_cod).ToString())

                            End While

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listCodPaz

        End Function

        Public Function GetCodicePazientiByComponentiCodiceFiscale(nome As String, cognome As String, sesso As String, dataNascita As Date?, codiceComuneNascita As String) As ICollection(Of String) Implements IPazienteProvider.GetCodicePazientiByComponentiCodiceFiscale

            Dim listCodPaz As New List(Of String)()

            Using cmd As OracleCommand = New OracleCommand(Queries.Pazienti.OracleQueries.selCodicePazientiByComponentiCodiceFiscale, Me.Connection)

                cmd.Parameters.AddWithValue("paz_nome", GetStringParam(nome, False))
                cmd.Parameters.AddWithValue("paz_cognome", GetStringParam(cognome, False))
                cmd.Parameters.AddWithValue("paz_data_nascita", GetDateParam(dataNascita))
                cmd.Parameters.AddWithValue("paz_sesso", GetStringParam(sesso, False))
                cmd.Parameters.AddWithValue("paz_com_codice_nascita", GetStringParam(codiceComuneNascita, False))

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using dr As IDataReader = cmd.ExecuteReader()

                        If Not dr Is Nothing Then

                            Dim pos_paz_cod As Integer = dr.GetOrdinal("paz_codice")

                            While dr.Read()

                                listCodPaz.Add(dr.GetValue(pos_paz_cod).ToString())

                            End While

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listCodPaz

        End Function

        ''' <summary>
        ''' Restituisce un datatable con i pazienti corrispondenti ai filtri inseriti
        ''' </summary>
        ''' <param name="filtri"></param>
        ''' <param name="joinComuni"></param>
        ''' <returns></returns>
        Public Function GetPazienti(filtri As FiltriRicercaPaziente, joinComuni As Boolean) As DataTable Implements IPazienteProvider.GetPazienti

            RefurbishDT()

            With _DAM.QB

                .NewQuery()

                .AddTables("t_paz_pazienti")
                .AddSelectFields("t_paz_pazienti.*")

                ' Filtri
                If Not filtri.IsNullValue(filtri.Codice) Then
                    .AddWhereCondition("paz_codice", Comparatori.Uguale, filtri.Codice, DataTypes.Numero)
                End If

                If Not filtri.IsNullValue(filtri.DataNascita_Da) Then
                    .AddWhereCondition("paz_data_nascita", Comparatori.MaggioreUguale, filtri.DataNascita_Da, DataTypes.Data)
                End If

                If Not filtri.IsNullValue(filtri.DataNascita_A) Then
                    .AddWhereCondition("paz_data_nascita", Comparatori.MinoreUguale, filtri.DataNascita_A, DataTypes.Data)
                End If

                If Not filtri.IsNullValue(filtri.Sesso) Then
                    .AddWhereCondition("paz_sesso", Comparatori.Uguale, filtri.Sesso, DataTypes.Stringa)
                End If

                If Not filtri.IsNullValue(filtri.Consultorio) Then
                    .AddWhereCondition("paz_cns_codice", Comparatori.Uguale, filtri.Consultorio, DataTypes.Stringa)
                End If

                If Not filtri.IsNullValue(filtri.Malattia) Then
                    .AddTables("t_paz_malattie")
                    .AddWhereCondition("pma_mal_codice", Comparatori.Uguale, filtri.Malattia, DataTypes.Stringa)
                    .AddWhereCondition("paz_codice", Comparatori.Uguale, "pma_paz_codice", DataTypes.Join)
                End If

                If Not filtri.IsNullValue(filtri.CategoriaRischio) Then
                    .AddWhereCondition("paz_rsc_codice", Comparatori.Uguale, filtri.CategoriaRischio, DataTypes.Stringa)
                End If

                If Not filtri.IsNullValue(filtri.StatoAnagrafico) Then
                    .AddWhereCondition("paz_stato_anagrafico", Comparatori.In, String.Join(",", filtri.StatoAnagrafico), DataTypes.Replace)
                End If

                ' Join con comune nascita, residenza e domicilio
                If joinComuni Then
                    .AddTables("t_ana_comuni n, t_ana_comuni r, t_ana_comuni d")
                    .AddSelectFields("n.com_descrizione com_descrizione_nascita, r.com_descrizione com_descrizione_residenza, d.com_descrizione com_descrizione_domicilio")
                    .AddWhereCondition("paz_com_codice_nascita", Comparatori.Uguale, "n.com_codice", DataTypes.OutJoinLeft)
                    .AddWhereCondition("paz_com_codice_residenza", Comparatori.Uguale, "r.com_codice", DataTypes.OutJoinLeft)
                    .AddWhereCondition("paz_com_codice_domicilio", Comparatori.Uguale, "d.com_codice", DataTypes.OutJoinLeft)
                End If

                ' Ordinamento
                .AddOrderByFields("paz_cognome, paz_nome, paz_data_nascita")

            End With

            _DAM.BuildDataTable(_DT)

            'Catch ex As Exception --> Lascio che l'eccezione venga gestita al livello superiore. E' necessaria la genericProvider.Dispose()

            Return _DT.Copy

        End Function

        Public Function GetPazientiByCF(codiceFiscale As String) As List(Of InfoAssistito)

            Dim result As New List(Of InfoAssistito)()

            Dim ownConnection As Boolean = False

            Dim query As String = "SELECT PAZ_CODICE, PAZ_NOME, PAZ_COGNOME, PAZ_DATA_NASCITA, PAZ_SESSO, PAZ_CODICE_FISCALE, PAZ_DATA_DECESSO " +
                                  "FROM T_PAZ_PAZIENTI " +
                                  "WHERE PAZ_CODICE_FISCALE = :PAZ_CODICE_FISCALE"
            Try
                Using cmd As OracleCommand = New OracleCommand(query, Connection)

                    cmd.Parameters.AddWithValue("PAZ_CODICE_FISCALE", codiceFiscale)

                    ownConnection = ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()

                        Dim paz_codice As Integer = _context.GetOrdinal("PAZ_CODICE")
                        Dim paz_nome As Integer = _context.GetOrdinal("PAZ_NOME")
                        Dim paz_cognome As Integer = _context.GetOrdinal("PAZ_COGNOME")
                        Dim paz_data_di_nascita As Integer = _context.GetOrdinal("PAZ_DATA_NASCITA")
                        Dim paz_sesso As Integer = _context.GetOrdinal("PAZ_SESSO")
                        Dim paz_codice_fiscale As Integer = _context.GetOrdinal("PAZ_CODICE_FISCALE")
                        Dim paz_data_decesso As Integer = _context.GetOrdinal("PAZ_DATA_DECESSO")

                        If _context.Read() Then

                            Dim paz As New InfoAssistito()

                            paz.Codice = _context.GetInt32OrDefault(paz_codice)
                            paz.Nome = _context.GetStringOrDefault(paz_nome)
                            paz.Cognome = _context.GetStringOrDefault(paz_cognome)
                            paz.DataDiNascita = _context.GetDateTimeOrDefault(paz_data_di_nascita)
                            paz.Sesso = _context.GetStringOrDefault(paz_sesso)
                            paz.Codice_Fiscale = _context.GetStringOrDefault(paz_codice_fiscale)
                            paz.DataDecesso = _context.GetNullableDateTimeOrDefault(paz_data_decesso)

                            result.Add(paz)

                        End If

                    End Using
                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return result

        End Function

        ''' <summary>
        ''' Restituisce un datatable con i dati del paziente, in base al codice specificato
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        Public Function GetFromKey(codicePaziente As Integer) As DataTable Implements IPazienteProvider.GetFromKey

            RefurbishDT()

            With _DAM.QB

                .NewQuery(True)
                .AddTables("t_paz_pazienti")
                .AddSelectFields("*")
                .AddWhereCondition("paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)

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

#Region " Dati Anagrafici Paziente "

        ''' <summary>
        ''' Restituisce un oggetto PazienteDatiAnagrafici contenente nome, cognome, data di nascita, sesso,
        ''' codice fiscale, indirizzo di residenza e di domicilio del paziente specificato
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        Public Function GetDatiAnagraficiPaziente(codicePaziente As Integer) As PazienteDatiAnagrafici Implements IPazienteProvider.GetDatiAnagraficiPaziente

            Return GetDatiAnagraficiPaziente(codicePaziente, False)

        End Function

        ''' <summary>
        ''' Restituisce un oggetto PazienteDatiAnagrafici contenente nome, cognome, data di nascita, sesso, 
        ''' codice fiscale, indirizzo di residenza e di domicilio del paziente specificato, effettuando la query in anagrafe centrale.
        ''' </summary>
        ''' <param name="codicePazienteCentrale"></param>
        ''' <returns></returns>
        Public Function GetDatiAnagraficiPazienteCentrale(codicePazienteCentrale As String) As PazienteDatiAnagrafici Implements IPazienteProvider.GetDatiAnagraficiPazienteCentrale

            Return GetDatiAnagraficiPaziente(codicePazienteCentrale, True)

        End Function

        Private Function GetDatiAnagraficiPaziente(codicePaziente As String, isCentrale As Boolean) As PazienteDatiAnagrafici

            Dim paziente As PazienteDatiAnagrafici = Nothing

            With _DAM.QB
                .NewQuery()
                .AddTables("t_paz_pazienti")
                .AddSelectFields("paz_cognome, paz_nome, paz_data_nascita, paz_sesso, paz_codice_fiscale, paz_indirizzo_residenza, paz_indirizzo_domicilio, paz_data_decesso")
                .AddWhereCondition("paz_codice", Comparatori.Uguale, codicePaziente, IIf(isCentrale, DataTypes.Stringa, DataTypes.Numero))
            End With

            Using idr As IDataReader = _DAM.BuildDataReader()

                If Not idr Is Nothing Then

                    If idr.Read() Then

                        Dim paz_cognome As Integer = idr.GetOrdinal("paz_cognome")
                        Dim paz_nome As Integer = idr.GetOrdinal("paz_nome")
                        Dim paz_data_nascita As Integer = idr.GetOrdinal("paz_data_nascita")
                        Dim paz_sesso As Integer = idr.GetOrdinal("paz_sesso")
                        Dim paz_codice_fiscale As Integer = idr.GetOrdinal("paz_codice_fiscale")
                        Dim paz_indirizzo_residenza As Integer = idr.GetOrdinal("paz_indirizzo_residenza")
                        Dim paz_indirizzo_domicilio As Integer = idr.GetOrdinal("paz_indirizzo_domicilio")
                        Dim paz_data_decesso As Integer = idr.GetOrdinal("paz_data_decesso")

                        paziente = New PazienteDatiAnagrafici()

                        paziente.CodicePaziente = codicePaziente
                        paziente.Cognome = idr.GetStringOrDefault(paz_cognome)
                        paziente.Nome = idr.GetStringOrDefault(paz_nome)
                        paziente.DataNascita = idr.GetDateTimeOrDefault(paz_data_nascita)
                        paziente.Sesso = idr.GetStringOrDefault(paz_sesso)
                        paziente.CodiceFiscale = idr.GetStringOrDefault(paz_codice_fiscale)
                        paziente.IndirizzoResidenza = idr.GetStringOrDefault(paz_indirizzo_residenza)
                        paziente.IndirizzoDomicilio = idr.GetStringOrDefault(paz_indirizzo_domicilio)
                        paziente.DataDecesso = idr.GetNullableDateTimeOrDefault(paz_data_decesso)

                    End If

                End If

            End Using

            Return paziente

        End Function

        ''' <summary>
        ''' Restituisce un oggetto PazienteDatiAnagraficiIntestazioneBilancio contenente i dati del paziente da inserire nell'intestazione del report di bilancio.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDatiAnagraficiPazienteIntestazioneBilancio(codicePaziente As Long) As PazienteDatiAnagraficiIntestazioneBilancio Implements IPazienteProvider.GetDatiAnagraficiPazienteIntestazioneBilancio

            Return GetDatiAnagraficiPazienteCentraleIntestazioneBilancio(codicePaziente.ToString(), False)

        End Function

        ''' <summary>
        ''' Restituisce un oggetto PazienteDatiAnagraficiIntestazioneBilancio contenente i dati del paziente da inserire nell'intestazione del report di bilancio,
        ''' effettuando la query in anagrafe centrale.
        ''' </summary>
        ''' <param name="codicePazienteCentrale"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDatiAnagraficiPazienteCentraleIntestazioneBilancio(codicePazienteCentrale As String) As PazienteDatiAnagraficiIntestazioneBilancio Implements IPazienteProvider.GetDatiAnagraficiPazienteCentraleIntestazioneBilancio

            Return GetDatiAnagraficiPazienteCentraleIntestazioneBilancio(codicePazienteCentrale, True)

        End Function

        Private Function GetDatiAnagraficiPazienteCentraleIntestazioneBilancio(codicePaziente As String, isCentrale As Boolean) As PazienteDatiAnagraficiIntestazioneBilancio

            Dim paziente As PazienteDatiAnagraficiIntestazioneBilancio = Nothing

            Using cmd As OracleCommand = Connection.CreateCommand()

                Dim query As String =
                    " select paz_cognome, paz_nome, paz_data_nascita, paz_sesso, paz_codice_fiscale, paz_tessera, paz_padre, paz_madre, " +
                    " nas.com_codice nas_codice, nas.com_descrizione nas_descrizione, nas.com_provincia nas_prv, nas.com_cap nas_cap, " +
                    " paz_indirizzo_residenza, res.com_codice res_codice, res.com_descrizione res_descrizione, res.com_provincia res_prv, res.com_cap res_cap, " +
                    " paz_indirizzo_domicilio, dom.com_codice dom_codice, dom.com_descrizione dom_descrizione, dom.com_provincia dom_prv, dom.com_cap dom_cap, " +
                    " paz_med_codice_base, med_cognome, med_nome, med_codice_regionale, med_codice_fiscale, " +
                    " paz_cit_codice, cit_stato " +
                    " from t_paz_pazienti " +
                    "  left join t_ana_comuni nas on paz_com_codice_nascita = nas.com_codice " +
                    "  left join t_ana_comuni res on paz_com_codice_residenza = res.com_codice " +
                    "  left join t_ana_comuni dom on paz_com_codice_domicilio = dom.com_codice " +
                    "  left join t_ana_medici on paz_med_codice_base = med_codice " +
                    "  left join t_ana_cittadinanze on paz_cit_codice = cit_codice " +
                    " where paz_codice = :paz_codice "

                cmd.Parameters.AddWithValue("paz_codice", codicePaziente)
                cmd.CommandText = query

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim paz_cognome As Integer = idr.GetOrdinal("paz_cognome")
                            Dim paz_nome As Integer = idr.GetOrdinal("paz_nome")
                            Dim paz_sesso As Integer = idr.GetOrdinal("paz_sesso")
                            Dim paz_codice_fiscale As Integer = idr.GetOrdinal("paz_codice_fiscale")
                            Dim paz_tessera As Integer = idr.GetOrdinal("paz_tessera")
                            Dim paz_data_nascita As Integer = idr.GetOrdinal("paz_data_nascita")

                            Dim nas_codice As Integer = idr.GetOrdinal("nas_codice")
                            Dim nas_descrizione As Integer = idr.GetOrdinal("nas_descrizione")
                            Dim nas_prv As Integer = idr.GetOrdinal("nas_prv")
                            Dim nas_cap As Integer = idr.GetOrdinal("nas_cap")

                            Dim paz_indirizzo_residenza As Integer = idr.GetOrdinal("paz_indirizzo_residenza")
                            Dim res_codice As Integer = idr.GetOrdinal("res_codice")
                            Dim res_descrizione As Integer = idr.GetOrdinal("res_descrizione")
                            Dim res_prv As Integer = idr.GetOrdinal("res_prv")
                            Dim res_cap As Integer = idr.GetOrdinal("res_cap")

                            Dim paz_indirizzo_domicilio As Integer = idr.GetOrdinal("paz_indirizzo_domicilio")
                            Dim dom_codice As Integer = idr.GetOrdinal("dom_codice")
                            Dim dom_descrizione As Integer = idr.GetOrdinal("dom_descrizione")
                            Dim dom_prv As Integer = idr.GetOrdinal("dom_prv")
                            Dim dom_cap As Integer = idr.GetOrdinal("dom_cap")

                            Dim paz_med_codice_base As Integer = idr.GetOrdinal("paz_med_codice_base")
                            Dim med_cognome As Integer = idr.GetOrdinal("med_cognome")
                            Dim med_nome As Integer = idr.GetOrdinal("med_nome")
                            Dim med_codice_regionale As Integer = idr.GetOrdinal("med_codice_regionale")
                            Dim med_codice_fiscale As Integer = idr.GetOrdinal("med_codice_fiscale")

                            Dim paz_cit_codice As Integer = idr.GetOrdinal("paz_cit_codice")
                            Dim cit_stato As Integer = idr.GetOrdinal("cit_stato")

                            Dim paz_padre As Integer = idr.GetOrdinal("paz_padre")
                            Dim paz_madre As Integer = idr.GetOrdinal("paz_madre")

                            If idr.Read() Then

                                paziente = New PazienteDatiAnagraficiIntestazioneBilancio()

                                paziente.CodicePaziente = codicePaziente
                                paziente.Cognome = idr.GetStringOrDefault(paz_cognome)
                                paziente.Nome = idr.GetStringOrDefault(paz_nome)
                                paziente.Sesso = idr.GetStringOrDefault(paz_sesso)
                                paziente.CodiceFiscale = idr.GetStringOrDefault(paz_codice_fiscale)
                                paziente.TesseraSanitaria = idr.GetStringOrDefault(paz_tessera)
                                paziente.DataNascita = idr.GetDateTimeOrDefault(paz_data_nascita)

                                paziente.CodiceComuneNascita = idr.GetStringOrDefault(nas_codice)
                                paziente.DescrizioneComuneNascita = idr.GetStringOrDefault(nas_descrizione)
                                paziente.ProvinciaNascita = idr.GetStringOrDefault(nas_prv)
                                paziente.CapNascita = idr.GetStringOrDefault(nas_cap)

                                paziente.IndirizzoResidenza = idr.GetStringOrDefault(paz_indirizzo_residenza)
                                paziente.CodiceComuneResidenza = idr.GetStringOrDefault(res_codice)
                                paziente.DescrizioneComuneResidenza = idr.GetStringOrDefault(res_descrizione)
                                paziente.ProvinciaResidenza = idr.GetStringOrDefault(res_prv)
                                paziente.CapResidenza = idr.GetStringOrDefault(res_cap)

                                paziente.IndirizzoDomicilio = idr.GetStringOrDefault(paz_indirizzo_domicilio)
                                paziente.CodiceComuneDomicilio = idr.GetStringOrDefault(dom_codice)
                                paziente.DescrizioneComuneDomicilio = idr.GetStringOrDefault(dom_descrizione)
                                paziente.ProvinciaDomicilio = idr.GetStringOrDefault(dom_prv)
                                paziente.CapDomicilio = idr.GetStringOrDefault(dom_cap)

                                paziente.CodiceMedico = idr.GetStringOrDefault(paz_med_codice_base)
                                paziente.CognomeMedico = idr.GetStringOrDefault(med_cognome)
                                paziente.NomeMedico = idr.GetStringOrDefault(med_nome)
                                paziente.CodiceFiscaleMedico = idr.GetStringOrDefault(med_codice_fiscale)
                                paziente.CodiceRegionaleMedico = idr.GetStringOrDefault(med_codice_regionale)

                                paziente.CodiceCittadinanza = idr.GetStringOrDefault(paz_cit_codice)
                                paziente.StatoCittadinanza = idr.GetStringOrDefault(cit_stato)

                                paziente.Padre = idr.GetStringOrDefault(paz_padre)
                                paziente.Madre = idr.GetStringOrDefault(paz_madre)

                            End If

                        End If
                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return paziente

        End Function

        Public Function GetPazientiByRSA(idRSA As String, Nome As String, Cognome As String, CF As String) As List(Of InfoAssistito) Implements IPazienteProvider.GetPazientiByRSA

            Dim result As New List(Of InfoAssistito)()

            Dim ownConnection As Boolean = False
            Dim query As String = "select paz_codice, paz_cognome, paz_nome, paz_data_nascita, paz_sesso, paz_codice_fiscale, paz_data_decesso " +
                                  "from t_paz_rsa " +
                                  "join t_paz_pazienti on prs_paz_codice = paz_codice " +
                                  "where prs_rsa_id = :prs_rsa_id"
            Try
                Using cmd As New OracleCommand(query, Connection)

                    cmd.Parameters.AddWithValue("prs_rsa_id", idRSA)

                    If Not String.IsNullOrWhiteSpace(Nome) Then
                        query += " AND paz_nome LIKE :Nome"
                        cmd.Parameters.AddWithValue("Nome", Nome + "%")
                    End If

                    If Not String.IsNullOrWhiteSpace(Cognome) Then
                        query += " AND paz_cognome LIKE :Cognome"
                        cmd.Parameters.AddWithValue("Cognome", Cognome + "%")
                    End If

                    If Not String.IsNullOrWhiteSpace(CF) Then
                        query += " AND paz_codice_fiscale = :CF"
                        cmd.Parameters.AddWithValue("CF", CF)
                    End If

                    cmd.CommandText = query
                    cmd.Connection = Me.Connection

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()
                        If Not _context Is Nothing Then

                            Dim paz_codice As Integer = _context.GetOrdinal("paz_codice")
                            Dim paz_cognome As Integer = _context.GetOrdinal("paz_cognome")
                            Dim paz_nome As Integer = _context.GetOrdinal("paz_nome")
                            Dim paz_data_nascita As Integer = _context.GetOrdinal("paz_data_nascita")
                            Dim paz_sesso As Integer = _context.GetOrdinal("paz_sesso")
                            Dim paz_codice_fiscale As Integer = _context.GetOrdinal("paz_codice_fiscale")
                            Dim paz_data_decesso As Integer = _context.GetOrdinal("paz_data_decesso")

                            While _context.Read()

                                Dim paziente As New InfoAssistito()
                                paziente.Codice = _context.GetInt32OrDefault(paz_codice)
                                paziente.Cognome = _context.GetStringOrDefault(paz_cognome)
                                paziente.Nome = _context.GetStringOrDefault(paz_nome)
                                paziente.DataDiNascita = _context.GetDateTimeOrDefault(paz_data_nascita)
                                paziente.Sesso = _context.GetStringOrDefault(paz_sesso)
                                paziente.Codice_Fiscale = _context.GetStringOrDefault(paz_codice_fiscale)
                                paziente.DataDecesso = _context.GetNullableDateTimeOrDefault(paz_data_decesso)

                                result.Add(paziente)

                            End While
                        End If

                    End Using
                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return result

        End Function

#End Region

        ''' <summary>
        ''' Restituisce true se esiste il paziente specificato
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        Public Function Exists(codicePaziente As Integer) As Boolean Implements IPazienteProvider.Exists

            Dim result As Boolean = False

            Using cmd As OracleCommand = Connection.CreateCommand()

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                cmd.CommandText = OnVac.Queries.Pazienti.OracleQueries.selExists

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()
                    If (Not obj Is Nothing AndAlso Not obj Is DBNull.Value) Then
                        result = True
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return result

        End Function

        Public Function ExistsNotaPaziente(codicePaziente As Long, codiceUsl As String, codiceTipoNota As String) As Integer? Implements IPazienteProvider.ExistsNotaPaziente

            Dim result As Boolean = False

            Using cmd As OracleCommand = Connection.CreateCommand()

                cmd.Parameters.AddWithValue("pno_paz_codice", codicePaziente)
                cmd.Parameters.AddWithValue("pno_azi_codice", codiceUsl)
                cmd.Parameters.AddWithValue("pno_tno_codice", codiceTipoNota)
                cmd.CommandText = OnVac.Queries.NotePazienti.OracleQueries.selExistsNota

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()
                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        result = True
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return result

        End Function

        Public Function IsPazienteInRSA(codicePaziente As Integer) As Boolean Implements IPazienteProvider.IsPazienteInRSA

            Dim result As Boolean = False

            Using cmd As OracleCommand = Connection.CreateCommand()

                cmd.CommandText = "select 1 from T_PAZ_RSA where PRS_PAZ_CODICE = :PRS_PAZ_CODICE"

                cmd.Parameters.AddWithValue("PRS_PAZ_CODICE", codicePaziente)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        result = True
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return result

        End Function

        ''' <summary>
        '''
        ''' </summary>
        Public Function IsSenzaPediatra(codicePaziente As Integer) As Boolean Implements IPazienteProvider.IsSenzaPediatra

            Dim obj As Int16

            With _DAM.QB

                .NewQuery()
                '     SQL DECODE SYNTAX
                '     decode (expression, search_1, result_1,[ search_2, result_2,] default)
                '     decode (expression, search_1, result_1, default)
                ' MED_TIPO = NULL,1 (non pediatra); 2 (pediatra)
                .AddSelectFields("decode(med_tipo,2,0,1) senzaPediatra")
                .AddTables("T_ANA_MEDICI", "T_PAZ_PAZIENTI")
                .AddWhereCondition("PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("MED_CODICE", Comparatori.Uguale, "PAZ_MED_CODICE_BASE", DataTypes.OutJoinRight)

            End With

            Try

                obj = _DAM.ExecScalar()

            Catch ex As Exception

                If _DAM.ExistTra Then
                    _DAM.Rollback()
                End If

                LogError(ex)

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            If obj <> 0 Then Return False

            Return True

        End Function

        ''' <summary>
        ''' Restituisce True se il paziente è deceduto (data di decesso valorizzata o stato anagrafico = "DECEDUTO"). False altrimenti.
        ''' </summary>
        Public Function PazienteDeceduto(codicePaziente As Integer) As Boolean Implements IPazienteProvider.PazienteDeceduto

            Dim obj As Object

            ' Query per controllare se il paziente è deceduto
            With _DAM.QB
                .NewQuery()
                .AddTables("T_PAZ_PAZIENTI")
                .AddSelectFields("PAZ_CODICE")
                .AddWhereCondition("PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .OpenParanthesis()
                .AddWhereCondition("PAZ_STATO_ANAGRAFICO", Comparatori.Uguale, Enumerators.StatoAnagrafico.DECEDUTO, DataTypes.Numero)
                .AddWhereCondition("PAZ_DATA_DECESSO", Comparatori.[IsNot], "NULL", DataTypes.Replace, "OR")
                .CloseParanthesis()
            End With

            Try

                obj = _DAM.ExecScalar()

            Catch ex As Exception

                If _DAM.ExistTra Then
                    _DAM.Rollback()
                End If

                LogError(ex)

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            ' se il paziente è deceduto, obj = nothing
            Return (Not obj Is Nothing)

        End Function

        ''' <summary>
        '''
        ''' </summary>
        Public Function pazienteChiamabile(codicePaziente As Integer) As String Implements IPazienteProvider.PazienteChiamabile

            ' Query per controllare se il paziente è chiamabile in base al suo stato anagrafico
            Dim chiamata As String

            With _DAM.QB
                .NewQuery()
                .AddTables("T_PAZ_PAZIENTI", "T_ANA_STATI_ANAGRAFICI")
                .AddSelectFields("SAN_CHIAMATA")
                .AddWhereCondition("PAZ_STATO_ANAGRAFICO", Comparatori.Uguale, "SAN_CODICE", DataTypes.Join)
                .AddWhereCondition("PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
            End With

            Try

                chiamata = _DAM.ExecScalar()

            Catch ex As Exception

                If (_DAM.ExistTra) Then
                    _DAM.Rollback()
                End If

                LogError(ex)

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return chiamata

        End Function

        ''' <summary>
        ''' Restituisce la descrizione dello stato anagrafico del paziente.
        ''' </summary>
        Public Function statoAnag(codicePaziente As Integer) As String Implements IPazienteProvider.StatoAnag

            ' Query per controllare lo stato anagrafico del paziente
            Dim stato As String

            With _DAM.QB
                .NewQuery()
                .AddTables("T_PAZ_PAZIENTI", "T_ANA_STATI_ANAGRAFICI")
                .AddSelectFields("SAN_DESCRIZIONE")
                .AddWhereCondition("PAZ_STATO_ANAGRAFICO", Comparatori.Uguale, "SAN_CODICE", DataTypes.Join)
                .AddWhereCondition("PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
            End With

            Try

                stato = _DAM.ExecScalar()

            Catch ex As Exception

                If _DAM.ExistTra Then
                    _DAM.Rollback()
                End If

                LogError(ex)

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return stato

        End Function

        ''' <summary>
        ''' Restituisce il codice dello stato anagrafico del paziente.
        ''' </summary>
        Public Function GetCodiceStatoAnag(codicePaziente As Integer) As String Implements IPazienteProvider.GetCodiceStatoAnag

            Dim codStato As String = String.Empty

            With _DAM.QB
                .NewQuery()
                .AddTables("T_PAZ_PAZIENTI")
                .AddSelectFields("PAZ_STATO_ANAGRAFICO")
                .AddWhereCondition("PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
            End With

            Try

                codStato = _DAM.ExecScalar()

            Catch ex As Exception

                If _DAM.ExistTra Then
                    _DAM.Rollback()
                End If

                LogError(ex)

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return codStato

        End Function

        '''' <summary>
        '''' Restituisce le note di acquisizione del paziente.
        '''' </summary>
        'Public Function GetNoteAcquisizione(codicePaziente As Integer) As String Implements IPazienteProvider.GetNoteAcquisizione

        '    With _DAM.QB
        '        .NewQuery()
        '        .AddTables("T_PAZ_PAZIENTI")
        '        .AddSelectFields("PAZ_NOTE_ACQUISIZIONE")
        '        .AddWhereCondition("PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
        '    End With

        '    Dim noteAcquisizione As Object = _DAM.ExecScalar()

        '    If noteAcquisizione Is Nothing OrElse noteAcquisizione Is DBNull.Value Then Return String.Empty

        '    Return noteAcquisizione.ToString()

        'End Function

        ''' <summary>
        '''
        ''' </summary>
        Public Function GetCampiAnagraficiObbligatori(codicePaziente As Integer, bloccanti As Boolean, cittadinanza As String) As DataTable Implements IPazienteProvider.GetCampiAnagraficiObbligatori

            RefurbishDT()

            With _DAM.QB

                .NewQuery()
                .AddSelectFields("distinct can_campo")
                .AddTables("t_ana_campi_anagrafici_obbl")

                If cittadinanza <> "" Then
                    .OpenParanthesis()
                    .AddWhereCondition("can_cit_codice", Comparatori.Uguale, cittadinanza, DataTypes.Stringa)
                    .AddWhereCondition("can_cit_codice", Comparatori.Is, "null", DataTypes.Stringa, "OR")
                    .CloseParanthesis()
                ElseIf codicePaziente <> Nothing AndAlso codicePaziente > 0 Then
                    .AddTables("t_paz_pazienti")
                    .AddWhereCondition("paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                    .OpenParanthesis()
                    .AddWhereCondition("can_cit_codice", Comparatori.Uguale, "paz_cit_codice", DataTypes.Join)
                    .AddWhereCondition("can_cit_codice", Comparatori.Is, "null", DataTypes.Stringa, "OR")
                    .CloseParanthesis()
                Else
                    .AddWhereCondition("can_cit_codice", Comparatori.Is, "null", DataTypes.Stringa)
                End If

                If bloccanti Then
                    .AddWhereCondition("can_bloccante", Comparatori.Uguale, "S", DataTypes.Stringa)
                Else
                    .AddWhereCondition("can_bloccante", Comparatori.Uguale, "N", DataTypes.Stringa)
                End If

            End With

            _DAM.BuildDataTable(_DT)

            Return _DT.Copy

        End Function

        ''' <summary>
        ''' Query che restituisce i pazienti fuori età che devono essere aggiornati automaticamente.
        ''' Il nuovo consultorio è determinato dalla circoscrizione del paziente o, se non presente, dal comune di residenza.
        ''' Esistono due tabelle di link t_ana_link_circoscrizioni_cns e t_ana_link_comuni_consultori che 
        ''' indicano quale consultorio nuovo deve essere determinato. Il max nella query è stato inserito per
        ''' evitare il caso in cui ci siano più consultori associati alla stessa circoscrizione/comune.
        ''' L'ordine della union è fondamentale perchè per prima va testata la presenza del cons 
        ''' associato alla circ. La querazza schifosa è la seguente (probabilmente si può fare meglio, ma non 
        ''' mi è venuta...):
        ''' </summary>
        ''' <returns>Restituisce un datatable con codice del paziente, consultorio vecchio, consultorio nuovo e indicazione
        ''' della tabella utilizzata per determinarlo (cioè, in base a circoscrizione o a comune)</returns>
        ''' <remarks></remarks>
        Public Function CercaPazientiFuoriEtaCns(codiceConsultorio As String) As Integer() Implements IPazienteProvider.CercaPazientiFuoriEtaCns

            Dim arl As New ArrayList()
            Dim result As Integer()

            Try
                _DAM.ClearParam()

                Dim query As String = Queries.Pazienti.OracleQueries.selPazientiFuoriEtaCns
                query = query.Replace(":cnscodice", String.Format("'{0}'", codiceConsultorio))

                Using idr As System.Data.IDataReader = _DAM.BuildDataReader(query)

                    If Not idr Is Nothing Then

                        Dim PAZ_CODICE As Integer = idr.GetOrdinal("PAZ_CODICE")

                        While idr.Read()

                            If (Not idr(PAZ_CODICE) Is Nothing AndAlso Not idr.IsDBNull(PAZ_CODICE)) Then
                                arl.Add(idr.GetInt32(PAZ_CODICE))
                            End If

                        End While

                    End If

                End Using

                result = arl.ToArray(GetType(Integer))

            Catch ex As Exception

                result = New Integer(0) {}

                Me.SetErrorMsg("Errore durante il recupero dei pazienti")

                LogError(ex, "CercaPazientiFuoriEtaCns: errore lettura db per recupero dati pazienti.")

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return result

        End Function

        ''' <summary>
        ''' Ricerca pazienti fuori età in base ai filtri di ricerca impostati.
        ''' </summary>
        Public Function CercaPazientiFuoriEtaCns(filtri As Filters.FiltriRicercaPaziente, aggiornaAnchePazientiConAppuntamenti As Boolean) As Integer() Implements IPazienteProvider.CercaPazientiFuoriEtaCns

            Dim result As Integer()
            Dim sb As New System.Text.StringBuilder()

            Dim count As Integer = 0

            Try

                Dim arl As New ArrayList()

                Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                    sb.Append(Queries.Pazienti.OracleQueries.selPazientiFuoriEtaCns)
                    cmd.Parameters.AddWithValue("cnscodice", filtri.Consultorio)

                    'per non considerare i pazienti che hanno una convocazione con appuntamento
                    If (Not aggiornaAnchePazientiConAppuntamenti) Then
                        sb.Append(Queries.Pazienti.OracleQueries.selPazientiFuoriEtaCns_Condition1())
                    End If

                    ' Filtro sullo stato anagrafico: se è vuoto non deve filtrare per stato anagrafico
                    If (Not filtri.IsNullValue(filtri.StatoAnagrafico) AndAlso filtri.StatoAnagrafico.Length > 0) Then
                        Dim strFiltroStatoAnag As String = String.Join(",", filtri.StatoAnagrafico)
                        If (strFiltroStatoAnag <> String.Empty) Then
                            sb.Append(Queries.Pazienti.OracleQueries.selPazientiFuoriEtaCns_Condition2)
                            sb.Replace(":statoanagrafico", String.Join(",", filtri.StatoAnagrafico))
                        End If
                    End If

                    If (Not filtri.IsNullValue(filtri.Sesso)) Then
                        sb.Append(Queries.Pazienti.OracleQueries.selPazientiFuoriEtaCns_Condition3)
                        cmd.Parameters.AddWithValue("sesso", filtri.Sesso)
                    End If

                    If (Not filtri.IsNullValue(filtri.DataNascita_Da)) Then
                        sb.Append(Queries.Pazienti.OracleQueries.selPazientiFuoriEtaCns_Condition4)
                        cmd.Parameters.AddWithValue("datanascitada", filtri.DataNascita_Da)
                    End If

                    If (Not filtri.IsNullValue(filtri.DataNascita_A)) Then
                        sb.Append(Queries.Pazienti.OracleQueries.selPazientiFuoriEtaCns_Condition5)
                        cmd.Parameters.AddWithValue("datanascitaa", filtri.DataNascita_A)
                    End If

                    cmd.CommandText = sb.ToString()

                    Dim ownConnection As Boolean = False

                    Try

                        ownConnection = Me.ConditionalOpenConnection(cmd)

                        Using idr As System.Data.IDataReader = cmd.ExecuteReader()

                            If Not idr Is Nothing Then

                                Dim codicePaziente As Integer = idr.GetOrdinal("PAZ_CODICE")

                                While (idr.Read())
                                    If (Not idr(codicePaziente) Is Nothing AndAlso Not idr.IsDBNull(codicePaziente)) Then
                                        arl.Add(idr.GetInt32(codicePaziente))
                                    End If
                                End While

                            End If

                        End Using

                    Finally

                        Me.ConditionalCloseConnection(ownConnection)

                    End Try

                End Using

                result = arl.ToArray(GetType(Integer))

            Catch ex As Exception

                result = New Integer(0) {}
                Me.SetErrorMsg("Errore durante il recupero dei pazienti")

                LogError(ex, "CercaPazientiFuoriEtaCns: errore lettura db per recupero dati pazienti.")

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return result

        End Function

        Public Function CercaNuovoCns(codicePaziente As Integer) As Hashtable Implements IPazienteProvider.CercaNuovoCns

            Dim ht As New Hashtable()

            Try
                _DAM.ClearParam()

                Dim query As String = Queries.Pazienti.OracleQueries.selNuovoConsultorio
                query = query.Replace(":pazcodice", String.Format("'{0}'", codicePaziente))

                Using idr As IDataReader = _DAM.BuildDataReader(query)

                    If Not idr Is Nothing Then

                        Dim cnsNew As Integer = idr.GetOrdinal("CNS_NEW")
                        Dim tipo As Integer = idr.GetOrdinal("TIPO")

                        Dim key, value As String

                        While idr.Read()

                            If Not idr(cnsNew) Is Nothing AndAlso Not idr.IsDBNull(cnsNew) Then

                                If Not idr(tipo) Is Nothing AndAlso Not idr.IsDBNull(tipo) Then

                                    key = idr.GetString(tipo)
                                    value = idr.GetString(cnsNew)

                                    If Not ht.ContainsKey(key) Then
                                        ht.Add(key, value)
                                    End If

                                End If

                            End If

                        End While

                    End If

                End Using

            Catch ex As Exception

                Me.SetErrorMsg("Errore durante il recupero dei pazienti")

                LogError(ex, "CercaNuovoCns: errore lettura db per recupero dati pazienti.")

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return ht

        End Function

        ''' <summary> Cerco i pazienti deceduti</summary>
        Public Function CercaPazientiDeceduti(codiceConsultorio As String) As Integer() Implements IPazienteProvider.CercaPazientiDeceduti

            Dim result As Integer()

            Try
                With _DAM.QB
                    .NewQuery()
                    .AddSelectFields("paz_codice")
                    .AddTables("t_paz_pazienti")
                    .AddWhereCondition("paz_cns_codice", Comparatori.Uguale, codiceConsultorio, DataTypes.Stringa)
                    .OpenParanthesis()
                    .AddWhereCondition("paz_stato_anagrafico", Comparatori.Uguale, "9", DataTypes.Numero)
                    .AddWhereCondition("paz_data_decesso", Comparatori.[IsNot], "NULL", DataTypes.Data, "OR")
                    .CloseParanthesis()
                End With

                Dim arl As New ArrayList()

                Using idr As IDataReader = _DAM.BuildDataReader()

                    If Not idr Is Nothing Then

                        Dim codicePaziente As Integer = idr.GetOrdinal("PAZ_CODICE")

                        While idr.Read()
                            If Not idr(codicePaziente) Is Nothing AndAlso Not idr.IsDBNull(codicePaziente) Then
                                arl.Add(idr.GetInt32(codicePaziente))
                            End If
                        End While

                    End If

                End Using

                result = arl.ToArray(GetType(Integer))

            Catch ex As Exception

                result = New Integer(0) {}

                Me.SetErrorMsg("Errore durante il recupero dei pazienti deceduti")

                LogError(ex, "CercaPazientiDeceduti: errore lettura db per recupero dati pazienti.")

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return result

        End Function

        ''' <summary> Cerco i pazienti deceduti</summary>
        Public Function CercaPazientiDecedutiConConvocazioni(codiceConsultorio As String) As Integer() Implements IPazienteProvider.CercaPazientiDecedutiConConvocazioni

            Dim result As Integer()

            Try

                With _DAM.QB
                    .NewQuery()
                    .AddSelectFields("distinct paz_codice")
                    .AddTables("t_paz_pazienti", "t_cnv_convocazioni")
                    .AddWhereCondition("paz_cns_codice", Comparatori.Uguale, codiceConsultorio, DataTypes.Stringa)
                    .AddWhereCondition("paz_codice", Comparatori.Uguale, "cnv_paz_codice", DataTypes.Join)
                    .OpenParanthesis()
                    .AddWhereCondition("paz_stato_anagrafico", Comparatori.Uguale, "9", DataTypes.Numero)
                    .AddWhereCondition("paz_data_decesso", Comparatori.[IsNot], "NULL", DataTypes.Data, "OR")
                    .CloseParanthesis()
                End With

                Dim arl As New ArrayList()

                Using idr As IDataReader = _DAM.BuildDataReader()

                    If Not idr Is Nothing Then

                        Dim codicePaziente As Integer = idr.GetOrdinal("PAZ_CODICE")

                        While idr.Read()
                            If Not idr(codicePaziente) Is Nothing AndAlso Not idr.IsDBNull(codicePaziente) Then
                                arl.Add(idr.GetInt32(codicePaziente))
                            End If
                        End While

                    End If

                End Using

                result = arl.ToArray(GetType(Integer))

            Catch ex As Exception

                result = New Integer(0) {}

                Me.SetErrorMsg("Errore durante il recupero dei pazienti deceduti")

                LogError(ex, "CercaPazientiDecedutiConConvocazioni: errore lettura db per recupero dati pazienti.")

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return result

        End Function

        Public Function CercaPazientiAdultiDaAggiornare(codiceConsultorio As String, consideraCirc As Boolean, consideraCom As Boolean) As DataTable Implements IPazienteProvider.CercaPazientiAdultiDaAggiornare

            RefurbishDT()

            Dim sb As New System.Text.StringBuilder()

            Try
                _DAM.ClearParam()

                sb.Append(Queries.Pazienti.OracleQueries.selPazientiAdultiCambioConsultorio)
                sb.Replace(":cnscodice", String.Format("'{0}'", codiceConsultorio))
                If consideraCirc Then
                    'per non considerare i pazienti che hanno il consultorio già correttamente impostato tramite circoscrizione
                    sb.Append(Queries.Pazienti.OracleQueries.selPazientiAdultiCambioConsultorio_Condition1)
                End If
                If consideraCom Then
                    'per non considerare i pazienti che hanno il consultorio già correttamente impostato tramite comune
                    sb.Append(Queries.Pazienti.OracleQueries.selPazientiAdultiCambioConsultorio_Condition2)
                End If

                _DAM.BuildDataTable(sb.ToString, _DT)

            Catch ex As Exception

                _DT = Nothing

                Me.SetErrorMsg("Errore durante il recupero dei pazienti")

                LogError(ex, "CercaPazientiAdultiDaAggiornare: errore lettura db per recupero dati pazienti.")

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return _DT

        End Function

        ''' <summary>
        ''' Restituisce il codice del consultorio associato al paziente.
        ''' </summary>
        Public Function GetCodiceConsultorio(codicePaziente As Integer) As String Implements IPazienteProvider.GetCodiceConsultorio

            Dim cod_cns As String = String.Empty

            Try

                Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand

                    cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                    cmd.CommandText = OnVac.Queries.Pazienti.OracleQueries.selCodiceCns

                    Dim ownConnection As Boolean = False

                    Try

                        ownConnection = Me.ConditionalOpenConnection(cmd)

                        Dim obj As Object = cmd.ExecuteScalar()

                        If (Not obj Is Nothing AndAlso Not obj Is DBNull.Value) Then
                            cod_cns = obj.ToString()
                        Else
                            cod_cns = String.Empty
                        End If

                    Finally

                        Me.ConditionalCloseConnection(ownConnection)

                    End Try

                End Using

            Catch ex As Exception

                LogError(ex)

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return cod_cns

        End Function

        ''' <summary>
        ''' Restituisce il codice ausiliario del paziente specificato.
        ''' </summary>
        Public Function GetCodiceAusiliario(codicePaziente As Integer) As String Implements IPazienteProvider.GetCodiceAusiliario

            Return Me.GetValoreCampo(codicePaziente, OnVac.Queries.Pazienti.OracleQueries.selCodiceAusiliario)

        End Function

        ''' <summary>
        ''' Restituisce il codice regionale del paziente specificato.
        ''' </summary>
        Public Function GetCodiceRegionale(codicePaziente As Integer) As String Implements IPazienteProvider.GetCodiceRegionale

            Return Me.GetValoreCampo(codicePaziente, OnVac.Queries.Pazienti.OracleQueries.selCodiceRegionale)

        End Function

        Private Function GetValoreCampo(codicePaziente As Integer, query As String) As String

            Dim valoreCampo As String = String.Empty

            Dim ownConnection As Boolean = False

            Try

                Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                    cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                    cmd.CommandText = query

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If (Not obj Is Nothing AndAlso Not obj Is DBNull.Value) Then
                        valoreCampo = obj.ToString()
                    Else
                        valoreCampo = String.Empty
                    End If

                End Using

            Catch ex As Exception

                LogError(ex)

                ex.InternalPreserveStackTrace()
                Throw

            Finally

                Me.ConditionalCloseConnection(ownConnection)

            End Try

            Return valoreCampo

        End Function

        ''' <summary>
        ''' Restituisce il valore dell'enumerazione corrispondente allo stato vaccinale del paziente.
        ''' Se il valore su db non corrisponde a nessuno dei valori enumerati, restituisce nothing.
        ''' </summary>
        Public Function GetCodiceStatoVaccinale(codicePaziente As Integer) As Enumerators.StatiVaccinali Implements IPazienteProvider.GetCodiceStatoVaccinale

            Dim obj As Object
            Dim statoVaccinale As Enumerators.StatiVaccinali

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                cmd.CommandText = OnVac.Queries.Pazienti.OracleQueries.selStatoVaccPaz

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    ' Query lettura stato vaccinale
                    obj = cmd.ExecuteScalar()

                    If obj Is Nothing OrElse obj Is DBNull.Value OrElse obj.ToString() = String.Empty Then
                        Return Nothing
                    End If

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            ' Parse del valore dello stato vaccinale nell'enumerazione corrispondente
            Try
                statoVaccinale = System.Enum.Parse(GetType(Enumerators.StatiVaccinali), obj.ToString())
            Catch
                Return Nothing
            End Try

            Return statoVaccinale

        End Function

        Public Function GetCodicePazienteByCodiceRegionale(codiceRegionale As String) As List(Of String)

            Dim result As List(Of String) = New List(Of String)
            Dim ownConnection As Boolean = False
            Dim query As String = "SELECT PAZ_CODICE " +
                                    "FROM T_PAZ_PAZIENTI " +
                                    "WHERE PAZ_CODICE_REGIONALE = :PAZ_CODICE_REGIONALE"
            Try

                Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                    cmd.Parameters.AddWithValue("PAZ_CODICE_REGIONALE", codiceRegionale)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()
                        Dim paz_codice As Integer = _context.GetOrdinal("PAZ_CODICE")

                        While _context.Read()
                            Dim codice As String
                            codice = Convert.ToString(_context.GetValue(paz_codice))
                            result.Add(codice)
                        End While

                    End Using
                End Using
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return result
        End Function

        ''' <summary>
        ''' Restituisce la data massima di fine sospensione del paziente.
        ''' Se il paziente è sospeso senza data fine o se non è sospeso, restituisce Date.MinValue.
        ''' </summary>
        Public Function GetMaxDataFineSospensione(codicePaziente As Integer) As Date Implements IPazienteProvider.GetMaxDataFineSospensione

            Dim _max As Date = Date.MinValue

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("MAX(VIS_FINE_SOSPENSIONE)")
                .AddTables("T_VIS_VISITE")
                .AddWhereCondition("VIS_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
            End With

            Try
                Using idr As IDataReader = _DAM.BuildDataReader
                    If idr.Read() Then
                        If idr.IsDBNull(0) Then
                            _max = Date.MinValue
                        Else
                            _max = idr.GetDateTime(0)
                        End If
                    End If
                End Using

            Catch ex As Exception

                If (_DAM.ExistTra) Then _DAM.Rollback()

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return _max

        End Function


#Region " Flussi ACN "

        Public Function GetCodicePazientiByIdACN(IdACN As String) As List(Of String) Implements IPazienteProvider.GetCodicePazientiByIdACN

            Dim listCodPaz As New List(Of String)()

            Using cmd As OracleCommand = New OracleCommand("select paz_codice from t_paz_pazienti where paz_id_acn = :paz_id_acn", Connection)

                cmd.Parameters.AddWithValue("paz_id_acn", GetStringParam(IdACN, False))

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = ConditionalOpenConnection(cmd)

                    Using dr As IDataReader = cmd.ExecuteReader()

                        If Not dr Is Nothing Then

                            Dim pos_paz_cod As Integer = dr.GetOrdinal("paz_codice")

                            While dr.Read()

                                listCodPaz.Add(dr.GetValue(pos_paz_cod).ToString())

                            End While

                        End If

                    End Using

                Finally

                    ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return listCodPaz

        End Function


#End Region


#Region " Ritardi "

        ''' <summary>
        ''' Restituisce un datatable contenente i dati sui ritardi del paziente per la convocazione specificata.
        ''' </summary>
        Public Function GetRitardi(codicePaziente As Integer, dataConvocazione As Date) As DataTable Implements IPazienteProvider.GetRitardi

            RefurbishDT()

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("*")
                .AddTables("T_PAZ_RITARDI")
                .AddWhereCondition("PRI_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("PRI_CNV_DATA", Comparatori.Uguale, dataConvocazione, DataTypes.Data)
            End With

            _DAM.BuildDataTable(_DT)

            Return _DT.Copy

        End Function

        Public Function GetDateAppuntamentiRitardi(codicePaziente As Integer, dataConvocazione As Date, codiceCiclo As String, numeroSeduta As Integer) As Entities.DateAppuntamentiRitardi Implements IPazienteProvider.GetDateAppuntamentiRitardi

            Dim dateAppuntamentiRitardi As Entities.DateAppuntamentiRitardi = Nothing

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("PRI_DATA_APPUNTAMENTO1, PRI_DATA_APPUNTAMENTO2, PRI_DATA_APPUNTAMENTO3, PRI_DATA_APPUNTAMENTO4")
                .AddTables("T_PAZ_RITARDI")
                .AddWhereCondition("PRI_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("PRI_CNV_DATA", Comparatori.Uguale, dataConvocazione, DataTypes.Data)
                .AddWhereCondition("PRI_CIC_CODICE", Comparatori.Uguale, codiceCiclo, DataTypes.Stringa)
                .AddWhereCondition("PRI_SED_N_SEDUTA", Comparatori.Uguale, numeroSeduta, DataTypes.Numero)
            End With

            Using idr As IDataReader = _DAM.BuildDataReader()
                If Not idr Is Nothing Then

                    Dim dataAppuntamento1 As Integer = idr.GetOrdinal("PRI_DATA_APPUNTAMENTO1")
                    Dim dataAppuntamento2 As Integer = idr.GetOrdinal("PRI_DATA_APPUNTAMENTO2")
                    Dim dataAppuntamento3 As Integer = idr.GetOrdinal("PRI_DATA_APPUNTAMENTO3")
                    Dim dataAppuntamento4 As Integer = idr.GetOrdinal("PRI_DATA_APPUNTAMENTO4")

                    If idr.Read() Then

                        dateAppuntamentiRitardi = New Entities.DateAppuntamentiRitardi()
                        dateAppuntamentiRitardi.DataAppuntamento1 = idr.GetNullableDateTimeOrDefault(dataAppuntamento1)
                        dateAppuntamentiRitardi.DataAppuntamento2 = idr.GetNullableDateTimeOrDefault(dataAppuntamento2)
                        dateAppuntamentiRitardi.DataAppuntamento3 = idr.GetNullableDateTimeOrDefault(dataAppuntamento3)
                        dateAppuntamentiRitardi.DataAppuntamento4 = idr.GetNullableDateTimeOrDefault(dataAppuntamento4)

                    End If

                End If
            End Using

            Return dateAppuntamentiRitardi

        End Function

#End Region

#Region " Circoscrizione "

        ''' <summary>
        ''' Restituisce il codice della circoscrizione di residenza del paziente. 
        ''' </summary>
        Public Function GetCircoscrizioneResidenza(codicePaziente As Integer) As String Implements IPazienteProvider.GetCircoscrizioneResidenza

            Return Me.GetCircoscrizione(codicePaziente, True)

        End Function

        ''' <summary>
        ''' Restituisce il codice della circoscrizione di domicilio del paziente. 
        ''' </summary>
        Public Function GetCircoscrizioneDomicilio(codicePaziente As Integer) As String Implements IPazienteProvider.GetCircoscrizioneDomicilio

            Return Me.GetCircoscrizione(codicePaziente, False)

        End Function

        Private Function GetCircoscrizione(codicePaziente As Integer, isCircoscrizioneResidenza As Boolean) As String

            Dim codiceCircoscrizione As String = String.Empty

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)

                If isCircoscrizioneResidenza Then
                    cmd.CommandText = OnVac.Queries.Pazienti.OracleQueries.selCircoscrizioneResidenza
                Else
                    cmd.CommandText = OnVac.Queries.Pazienti.OracleQueries.selCircoscrizioneDomicilio
                End If

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If (Not obj Is Nothing AndAlso Not obj Is DBNull.Value) Then
                        codiceCircoscrizione = obj.ToString()
                    End If

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return codiceCircoscrizione

        End Function

#End Region

#Region " Indirizzi Paziente "

        Public Function GetCodiceIndirizzoResidenzaPaziente(codicePaziente As Integer) As Integer Implements IPazienteProvider.GetCodiceIndirizzoResidenzaPaziente
            Return Me.GetCodiceIndirizzoPaziente(codicePaziente, Queries.Pazienti.OracleQueries.selCodiceIndirizzoResidenza)
        End Function

        Public Function GetCodiceIndirizzoDomicilioPaziente(codicePaziente As Integer) As Integer Implements IPazienteProvider.GetCodiceIndirizzoDomicilioPaziente
            Return Me.GetCodiceIndirizzoPaziente(codicePaziente, Queries.Pazienti.OracleQueries.selCodiceIndirizzoDomicilio)
        End Function

        Private Function GetCodiceIndirizzoPaziente(codicePaziente As Integer, querySelectCodiceIndirizzoPaziente As String) As Integer

            Dim codiceIndirizzo As Integer = 0

            Using cmd As New OracleClient.OracleCommand(querySelectCodiceIndirizzoPaziente, Me.Connection)

                cmd.Parameters.AddWithValue("paz_codice", codicePaziente)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        codiceIndirizzo = Convert.ToInt32(obj)
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return codiceIndirizzo

        End Function

#End Region

#Region " Codice comune "

        ''' <summary>
        ''' Restituisce il codice del comune di residenza del paziente. Se non è valorizzato, restituisce la stringa vuota
        ''' </summary>
        Public Function GetCodiceComuneResidenza(codicePaziente As Integer) As String Implements IPazienteProvider.GetCodiceComuneResidenza

            Dim codiceComuneResidenza As String = String.Empty

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                cmd.CommandText = OnVac.Queries.Pazienti.OracleQueries.selCodiceComuneResidenza

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If (Not obj Is Nothing AndAlso Not obj Is DBNull.Value) Then
                        codiceComuneResidenza = obj.ToString()
                    Else
                        codiceComuneResidenza = String.Empty
                    End If

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return codiceComuneResidenza

        End Function

        ''' <summary>
        ''' Restituisce il codice del comune di domicilio del paziente. Se non è valorizzato, restituisce la stringa vuota
        ''' </summary>
        Public Function GetCodiceComuneDomicilio(codicePaziente As Integer) As String Implements IPazienteProvider.GetCodiceComuneDomicilio

            Dim codiceComuneDomicilio As String = String.Empty

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                cmd.CommandText = OnVac.Queries.Pazienti.OracleQueries.selCodiceComuneDomicilio

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If (Not obj Is Nothing AndAlso Not obj Is DBNull.Value) Then
                        codiceComuneDomicilio = obj.ToString
                    Else
                        codiceComuneDomicilio = String.Empty
                    End If

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return codiceComuneDomicilio

        End Function

        ''' <summary>
        ''' Restituisce il codice del comune di emigrazione del paziente. Se non è valorizzato, restituisce la stringa vuota
        ''' </summary>
        Public Function GetCodiceComuneEmigrazione(codicePaziente As Integer) As String Implements IPazienteProvider.GetCodiceComuneEmigrazione

            Dim codiceComuneEmigrazione As String = String.Empty

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                cmd.CommandText = OnVac.Queries.Pazienti.OracleQueries.selCodiceComuneEmigrazione

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If (Not obj Is Nothing AndAlso Not obj Is DBNull.Value) Then
                        codiceComuneEmigrazione = obj.ToString
                    Else
                        codiceComuneEmigrazione = String.Empty
                    End If

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return codiceComuneEmigrazione

        End Function

        ''' <summary>
        ''' Restituisce il codice del comune di immigrazione del paziente. Se non è valorizzato, restituisce la stringa vuota
        ''' </summary>
        Public Function GetCodiceComuneImmigrazione(codicePaziente As Integer) As String Implements IPazienteProvider.GetCodiceComuneImmigrazione

            Dim codiceComuneImmigrazione As String = String.Empty

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                cmd.CommandText = OnVac.Queries.Pazienti.OracleQueries.selCodiceComuneImmigrazione

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If (Not obj Is Nothing AndAlso Not obj Is DBNull.Value) Then
                        codiceComuneImmigrazione = obj.ToString()
                    Else
                        codiceComuneImmigrazione = String.Empty
                    End If

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return codiceComuneImmigrazione

        End Function

#End Region

        ''' <summary>
        ''' Dato un paziente ed il suo codice fiscale, restituisce il codice di un altro paziente 
        ''' avente stesso codice fiscale del paziente specificato. Se non lo trova, restituisce -1.
        ''' </summary>
        Public Function GetAltroPazienteStessoCodFiscale(codicePaziente As Integer, codiceFiscale As String) As Integer Implements IPazienteProvider.GetAltroPazienteStessoCodFiscale

            Dim _cod_paz As Integer = -1

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                cmd.Parameters.AddWithValue("cod_fisc", GetStringParam(codiceFiscale, False))
                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                cmd.CommandText = OnVac.Queries.Pazienti.OracleQueries.selCodFiscaleDuplicato

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If (Not obj Is Nothing AndAlso Not obj Is DBNull.Value) Then
                        _cod_paz = System.Convert.ToInt32(obj)
                    Else
                        _cod_paz = -1
                    End If

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return _cod_paz

        End Function

        ''' <summary>
        ''' Restituisce la data di immigrazione del paziente. Se non è valorizzata, restituisce Date.Minvalue
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        Public Function GetDataImmigrazione(codicePaziente As Integer) As Date Implements IPazienteProvider.GetDataImmigrazione

            Dim dataImmigrazione As Date = Date.MinValue

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                cmd.CommandText = OnVac.Queries.Pazienti.OracleQueries.selDataImmigrazione

                Dim ownConnection As Boolean = False

                Try

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()
                    If (Not obj Is Nothing AndAlso Not obj Is DBNull.Value) Then
                        dataImmigrazione = Convert.ToDateTime(obj)
                    Else
                        dataImmigrazione = Date.MinValue
                    End If

                Finally

                    Me.ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return dataImmigrazione

        End Function

        ''' <summary>
        ''' Restituisce la data di immigrazione del paziente. Se non è valorizzata, restituisce Date.Minvalue.
        ''' Effettua la ricerca in centrale.
        ''' </summary>
        ''' <param name="codicePazienteCentrale"></param>
        ''' <returns></returns>
        Public Function GetDataNascitaCentrale(codicePazienteCentrale As String) As Date Implements IPazienteProvider.GetDataNascitaCentrale

            Return Me.GetDataNascita(codicePazienteCentrale, True)

        End Function

        ''' <summary>
        ''' Restituisce la data di nascita del paziente. Se non è valorizzata, restituisce Date.Minvalue
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDataNascita(codicePaziente As Integer) As Date Implements IPazienteProvider.GetDataNascita

            Return Me.GetDataNascita(codicePaziente.ToString(), False)

        End Function

        Private Function GetDataNascita(codicePaziente As String, isCentrale As Boolean) As Date

            Dim dataNascita As Date = Date.MinValue

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                cmd.Parameters.Add(CreateOracleParameterCodicePaziente("cod_paz", codicePaziente, isCentrale))

                cmd.CommandText = OnVac.Queries.Pazienti.OracleQueries.selDataNascita

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        dataNascita = Convert.ToDateTime(obj)
                    Else
                        dataNascita = Date.MinValue
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return dataNascita

        End Function

        ''' <summary>
        ''' Restituisce la data di decesso del paziente. Se non è valorizzata, restituisce Date.Minvalue
        ''' Effettua la ricerca in centrale.
        ''' </summary>
        ''' <param name="codicePazienteCentrale"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDataDecessoCentrale(codicePazienteCentrale As String) As DateTime Implements IPazienteProvider.GetDataDecessoCentrale

            Return Me.GetDataDecesso(codicePazienteCentrale, True)

        End Function

        ''' <summary>
        ''' Restituisce la data di decesso del paziente. Se non è valorizzata, restituisce Date.Minvalue
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDataDecesso(codicePaziente As Integer) As DateTime Implements IPazienteProvider.GetDataDecesso

            Return Me.GetDataDecesso(codicePaziente, False)

        End Function

        Private Function GetDataDecesso(codicePaziente As String, isCentrale As Boolean) As DateTime

            Dim dataDecesso As Date = Date.MinValue

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                cmd.Parameters.Add(CreateOracleParameterCodicePaziente("cod_paz", codicePaziente, isCentrale))

                cmd.CommandText = OnVac.Queries.Pazienti.OracleQueries.selDataDecesso

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        dataDecesso = Convert.ToDateTime(obj)
                    Else
                        dataDecesso = Date.MinValue
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return dataDecesso

        End Function

        ''' <summary>
        ''' Restituisce il sesso del paziente. Se non è valorizzat0, restituisce la stringa vuota.
        ''' Effettua la ricerca in centrale.
        ''' </summary>
        ''' <param name="codicePazienteCentrale"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSessoPazienteCentrale(codicePazienteCentrale As String) As String Implements IPazienteProvider.GetSessoPazienteCentrale

            Return GetSessoPaziente(codicePazienteCentrale, True)

        End Function

        ''' <summary>
        ''' Restituisce il sesso del paziente. Se non è valorizzat0, restituisce la stringa vuota.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSessoPaziente(codicePaziente As Integer) As String Implements IPazienteProvider.GetSessoPaziente

            Return GetSessoPaziente(codicePaziente, False)

        End Function

        Private Function GetSessoPaziente(codicePaziente As String, isCentrale As Boolean) As String

            Dim sesso As String = String.Empty

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                cmd.Parameters.Add(CreateOracleParameterCodicePaziente("paz_codice", codicePaziente, isCentrale))

                cmd.CommandText = "select paz_sesso from t_paz_pazienti where paz_codice = :paz_codice"

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        sesso = obj.ToString()
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return sesso

        End Function


        ''' <summary>
        ''' Restituisce True se uno dei campi note del paziente è valorizzato
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="soloCampiShowCnv"></param>
        ''' <returns></returns>
        Public Function CountNote(codicePaziente As String, codiceUsl As String, soloCampiShowCnv As Boolean) As Integer Implements IPazienteProvider.CountNote

            Dim count As Integer = 0

            Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

                cmd.CommandText = "SELECT COUNT(pno_id) FROM t_ana_tipo_note LEFT JOIN t_paz_note ON tno_codice = pno_tno_codice " +
                    " WHERE pno_paz_codice = :pno_paz_codice " +
                    " AND ( pno_azi_codice = :pno_azi_codice OR pno_azi_codice IN ( SELECT dis_codice FROM t_ana_distretti WHERE dis_usl_codice = :pno_azi_codice ) ) " +
                    " AND pno_testo_note IS NOT NULL "
                If soloCampiShowCnv Then
                    cmd.CommandText += " AND tno_show_convocazioni = 'S' "
                End If
                cmd.Parameters.AddWithValue("pno_paz_codice", codicePaziente)
                cmd.Parameters.AddWithValue("pno_azi_codice", codiceUsl)

                Dim ownConnection As Boolean = False
                Try

                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If (Not obj Is Nothing AndAlso Not obj Is DBNull.Value) Then
                        Try
                            count = Convert.ToInt32(obj)
                        Catch ex As Exception
                            count = 0
                        End Try
                    End If

                Finally

                    ConditionalCloseConnection(ownConnection)

                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Restituisce true se il paziente specificato ha ritardi associati
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        Public Function HasRitardi(codicePaziente As String) As Boolean Implements IPazienteProvider.HasRitardi

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("1")
                .AddTables("T_PAZ_RITARDI, T_CNV_CONVOCAZIONI")
                .AddWhereCondition("PRI_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                'Brigoz 01-12-04
                'Devo considerare solo i ritardi per cui il paziente non si è mai presentato
                .AddWhereCondition("PRI_PAZ_CODICE", Comparatori.Uguale, "CNV_PAZ_CODICE", DataTypes.OutJoinLeft)
                .AddWhereCondition("PRI_CNV_DATA", Comparatori.Uguale, "CNV_DATA", DataTypes.OutJoinLeft)
            End With

            Dim obj As Object = _DAM.ExecScalar()

            If obj Is Nothing OrElse obj Is DBNull.Value Then
                Return False
            End If

            Return Convert.ToInt32(obj) > 0

        End Function

#Region " Conteggio Dati Vaccinali Paziente "

        ''' <summary>
        ''' Restituisce il numero di vaccinazioni eseguite per il paziente specificato
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        Public Function CountVaccinazioniEseguite(codicePaziente As Integer) As Integer Implements IPazienteProvider.CountVaccinazioniEseguite

            Dim count As Integer = 0

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                cmd.CommandText = OnVac.Queries.Pazienti.OracleQueries.cntVaccinazioniEseguitePaziente

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)

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

        ''' <summary>
        ''' Restituisce il numero di vaccinazioni scadute per il paziente specificato
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        Public Function CountVaccinazioniScadute(codicePaziente As Integer) As Integer Implements IPazienteProvider.CountVaccinazioniScadute

            Dim count As Integer = 0

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                cmd.CommandText = OnVac.Queries.Pazienti.OracleQueries.cntVaccinazioniScadutePaziente

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)

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

        ''' <summary>
        ''' Restituisce il numero di vaccinazioni escluse per il paziente specificato
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        Public Function CountVaccinazioniEscluse(codicePaziente As Integer) As Integer Implements IPazienteProvider.CountVaccinazioniEscluse

            Dim count As Integer = 0

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                cmd.CommandText = OnVac.Queries.Pazienti.OracleQueries.cntVaccinazioniEsclusePaziente

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)

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

        ''' <summary>
        ''' Restituisce il numero di visite con sospensione per il paziente specificato
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        Public Function CountSospensioni(codicePaziente As Integer) As Integer Implements IPazienteProvider.CountSospensioni

            Dim count As Integer = 0

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                cmd.CommandText = OnVac.Queries.Pazienti.OracleQueries.cntVisiteSospensionePaziente

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)

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

        ''' <summary>
        ''' Restituisce il numero di osservazioni per le visite del paziente specificato
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        Public Function CountOsservazioni(codicePaziente As Integer) As Integer Implements IPazienteProvider.CountOsservazioni

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("COUNT(*)")
                .AddTables("T_VIS_OSSERVAZIONI")
                .AddWhereCondition("VOS_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
            End With

            Dim obj As Object = _DAM.ExecScalar()

            If obj Is Nothing OrElse obj Is DBNull.Value Then
                Return 0
            End If

            Return Convert.ToInt32(obj)

        End Function

        ''' <summary>
        ''' Restituisce il numero di documenti del paziente specificato
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        Public Function CountDocumenti(codicePaziente As String) As Integer Implements IPazienteProvider.CountDocumenti

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("COUNT(*)")
                .AddTables("T_PAZ_DOCUMENTI")
                .AddWhereCondition("PDO_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
            End With

            Dim obj As Object = _DAM.ExecScalar()

            If obj Is Nothing OrElse obj Is DBNull.Value Then
                Return 0
            End If

            Return Convert.ToInt32(obj)

        End Function

        ''' <summary>
        ''' Restituisce il numero di rifiuti del paziente specificato
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        Public Function CountRifiuti(codicePaziente As String) As Integer Implements IPazienteProvider.CountRifiuti

            With _DAM.QB

                .IsDistinct = True

                .NewQuery()

                .AddSelectFields("COUNT(*)")
                .AddTables("T_PAZ_RIFIUTI")
                .AddWhereCondition("PRF_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)

            End With

            Dim obj As Object = _DAM.ExecScalar()

            If obj Is Nothing OrElse obj Is DBNull.Value Then
                Return 0
            End If

            Return Convert.ToInt32(obj)

        End Function

        ''' <summary>
        ''' Restituisce il numero di visite del paziente specificato
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        Public Function CountVisite(codicePaziente As String) As Integer Implements IPazienteProvider.CountVisite

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("COUNT(*)")
                .AddTables("T_VIS_VISITE")
                .AddWhereCondition("VIS_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
            End With

            Dim obj As Object = _DAM.ExecScalar()

            If obj Is Nothing OrElse obj Is DBNull.Value Then
                Return 0
            End If

            Return Convert.ToInt32(obj)

        End Function

        ''' <summary>
        ''' Restituisce il numero di inadempienze del paziente specificato
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CountInadempienze(codicePaziente As Integer) As Integer Implements IPazienteProvider.CountInadempienze

            Return CountInadempienzePaziente(codicePaziente, String.Empty)

        End Function

        ''' <summary>
        ''' Restituisce il numero di inadempienze del paziente, per la vaccinazione specificata
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="codiceVaccinazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CountInadempienze(codicePaziente As Integer, codiceVaccinazione As String) As Integer Implements IPazienteProvider.CountInadempienze

            Return CountInadempienzePaziente(codicePaziente, codiceVaccinazione)

        End Function

        Private Function CountInadempienzePaziente(codicePaziente As Long, codiceVaccinazione As String) As Integer

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("COUNT(*)")
                .AddTables("T_PAZ_INADEMPIENZE")
                .AddWhereCondition("PIN_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                If Not String.IsNullOrWhiteSpace(codiceVaccinazione) Then
                    .AddWhereCondition("PIN_VAC_CODICE", Comparatori.Uguale, codiceVaccinazione, DataTypes.Stringa)
                End If

            End With

            Dim obj As Object = _DAM.ExecScalar()

            If obj Is Nothing OrElse obj Is DBNull.Value Then
                Return 0
            End If

            Return Convert.ToInt32(obj)

        End Function

#End Region

        Public Function GetValoreCampiPaziente(codicePaziente As String, listNomiCampi As List(Of String)) As Hashtable Implements IPazienteProvider.GetValoreCampiPaziente

            Dim nomeCampo As String

            Dim ht As New Hashtable()

            _DAM.QB.NewQuery()
            _DAM.QB.AddTables("T_PAZ_PAZIENTI")
            For Each nomeCampo In listNomiCampi
                _DAM.QB.AddSelectFields(nomeCampo)
            Next
            _DAM.QB.AddWhereCondition("PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)

            Using idr As IDataReader = _DAM.BuildDataReader()

                If idr.Read() Then

                    For Each nomeCampo In listNomiCampi
                        ht.Add(nomeCampo, idr(nomeCampo))
                    Next

                End If

            End Using

            Return ht

        End Function

        Public Function GetDtMantoux(codicePaziente As Long) As DataTable Implements IPazienteProvider.GetDtMantoux

            Dim dt As New DataTable()

            With _DAM.QB
                .NewQuery()
                .AddSelectFields("T_PAZ_MANTOUX.MAN_DATA, T_PAZ_MANTOUX.MAN_DESCRIZIONE, T_PAZ_MANTOUX.MAN_MM, T_PAZ_MANTOUX.MAN_OPE_CODICE, T_ANA_OPERATORI.OPE_NOME, T_PAZ_MANTOUX.MAN_SINO, T_PAZ_MANTOUX.MAN_DATA_INVIO, T_PAZ_MANTOUX.MAN_POSITIVA")
                .AddTables("T_PAZ_MANTOUX", "T_ANA_OPERATORI")
                .AddWhereCondition("T_PAZ_MANTOUX.MAN_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("T_PAZ_MANTOUX.MAN_OPE_CODICE", Comparatori.Uguale, "T_ANA_OPERATORI.OPE_CODICE", DataTypes.OutJoinLeft)
                .AddOrderByFields("MAN_DATA")
            End With

            _DAM.BuildDataTable(dt)

            Return dt

        End Function

        ''' <summary>
        ''' Restituisce il numero di record di mantoux presenti, per il paziente specificato.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CountMantoux(codicePaziente As Long) As Integer Implements IPazienteProvider.CountMantoux

            Dim count As Integer = 0

            Using cmd As OracleCommand = Connection.CreateCommand()

                cmd.CommandText = "select count(*) from t_paz_mantoux where man_paz_codice = :man_paz_codice"

                cmd.Parameters.AddWithValue("man_paz_codice", codicePaziente)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                        count = Convert.ToInt32(obj)
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Restituisce il numero di episodi di sorveglianza COVID-19 per il paziente
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        Public Function CountEpisodiCovid(codicePaziente As Long) As Integer Implements IPazienteProvider.CountEpisodiCovid

            Dim count As Integer = 0

            Using cmd As OracleCommand = Connection.CreateCommand()

                cmd.CommandText = "SELECT COUNT(*) FROM T_PAZ_EPISODI WHERE PES_PAZ_CODICE = :PES_PAZ_CODICE"

                cmd.Parameters.AddWithValue("PES_PAZ_CODICE", codicePaziente)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                        count = Convert.ToInt32(obj)
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Restituisce il numero di tamponi COVID-19 per il paziente
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        Public Function CountTamponiCovid(codicePaziente As Long) As Integer Implements IPazienteProvider.CountTamponiCovid

            Dim count As Integer = 0

            Using cmd As OracleCommand = Connection.CreateCommand()

                cmd.CommandText = "SELECT COUNT(*) FROM T_ANA_TAMPONI_COVID WHERE ATC_PAZ_CODICE = :ATC_PAZ_CODICE"

                cmd.Parameters.AddWithValue("ATC_PAZ_CODICE", codicePaziente)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                        count = Convert.ToInt32(obj)
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Restituisce il numero di test sierologici COVID-19 per il paziente
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        Public Function CountTestSierologiciCovid(codicePaziente As Long) As Integer Implements IPazienteProvider.CountTestSierologiciCovid

            Dim count As Integer = 0

            Using cmd As OracleCommand = Connection.CreateCommand()

                cmd.CommandText = "SELECT COUNT(*) FROM V_PAZ_TEST_SIEROLOGICI WHERE TSS_PAZ_CODICE = :TSS_PAZ_CODICE"

                cmd.Parameters.AddWithValue("TSS_PAZ_CODICE", codicePaziente)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                        count = Convert.ToInt32(obj)
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Restituisce il numero di ricoveri COVID-19 per il paziente
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        Public Function CountRicoveriCovid(codicePaziente As Long) As Integer Implements IPazienteProvider.CountRicoveriCovid
            Return DoCommand(Function(cmd)
                                 cmd.CommandText = "SELECT count(*) as numero FROM (
	                                                    SELECT DISTINCT RIP_CODICE_GRUPPO FROM T_RICOVERI_PAZIENTE 
	                                                    WHERE 
	                                                    RIP_PAZ_CODICE = ?paz AND RIP_DATA_ELIMINAZIONE IS NULL
                                                    ) tab"
                                 cmd.AddParameter("paz", codicePaziente)
                                 Return cmd.First(Of Integer)
                             End Function)
        End Function

        Public Function GetDescrizioneNotePaziente(codiceTipoNota As String) As String Implements IPazienteProvider.GetDescrizioneNotePaziente

            Dim descrizione As String = String.Empty

            Using cmd As OracleCommand = Connection.CreateCommand()

                cmd.CommandText = "select distinct TNO_DESCRIZIONE from t_ana_tipo_note where TNO_CODICE = :TNO_CODICE"

                cmd.Parameters.AddWithValue("TNO_CODICE", codiceTipoNota)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If obj IsNot Nothing AndAlso obj IsNot DBNull.Value Then
                        descrizione = obj.ToString()
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return descrizione

        End Function

        ''' <summary>
        ''' Function che restituisce le varie tipologie di note di un paziente per un dato codice Usl
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="codiceUsl"></param>
        ''' <returns></returns>
        Public Function GetNotePaziente(codicePaziente As Long, codiceUsl As String) As List(Of PazienteNote) Implements IPazienteProvider.GetNotePaziente

            Dim listPazienteNote As New List(Of PazienteNote)()

            Using cmd As OracleCommand = Connection.CreateCommand()

                cmd.CommandText = OnVac.Queries.NotePazienti.OracleQueries.selNotePaziente

                cmd.Parameters.AddWithValue("pno_paz_codice", codicePaziente)
                cmd.Parameters.AddWithValue("pno_azi_codice", codiceUsl)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim tno_codice As Integer = idr.GetOrdinal("tno_codice")
                            Dim tno_descrizione As Integer = idr.GetOrdinal("tno_descrizione")
                            Dim tno_ordine As Integer = idr.GetOrdinal("tno_ordine")
                            Dim tno_modificabile As Integer = idr.GetOrdinal("tno_modificabile")
                            Dim tno_show_convocazioni As Integer = idr.GetOrdinal("tno_show_convocazioni")
                            Dim pno_id As Integer = idr.GetOrdinal("pno_id")
                            Dim pno_paz_codice As Integer = idr.GetOrdinal("pno_paz_codice")
                            Dim pno_testo_note As Integer = idr.GetOrdinal("pno_testo_note")
                            Dim pno_azi_codice As Integer = idr.GetOrdinal("pno_azi_codice")
                            Dim pno_ute_id_ultima_modifica As Integer = idr.GetOrdinal("pno_ute_id_ultima_modifica")
                            Dim pno_data_ultima_modifica As Integer = idr.GetOrdinal("pno_data_ultima_modifica")

                            While idr.Read()

                                Dim pazienteNote As New PazienteNote()

                                pazienteNote.CodiceNota = idr.GetStringOrDefault(tno_codice)
                                pazienteNote.DescrizioneNota = idr.GetStringOrDefault(tno_descrizione)
                                pazienteNote.OrdineNota = idr.GetInt32OrDefault(tno_ordine)
                                pazienteNote.FlagNotaModificabile = idr.GetBooleanOrDefault(tno_modificabile)
                                pazienteNote.FlagNotaVisibileConvocazioni = idr.GetBooleanOrDefault(tno_show_convocazioni)
                                pazienteNote.IdNota = idr.GetNullableInt32OrDefault(pno_id)
                                pazienteNote.CodicePaziente = idr.GetInt32OrDefault(pno_paz_codice)
                                pazienteNote.TestoNota = idr.GetStringOrDefault(pno_testo_note)
                                pazienteNote.CodiceAzienda = idr.GetStringOrDefault(pno_azi_codice)
                                pazienteNote.IdUtenteUltimaModifica = idr.GetNullableInt32OrDefault(pno_ute_id_ultima_modifica)
                                pazienteNote.DataUltimaModifica = idr.GetNullableDateTimeOrDefault(pno_data_ultima_modifica)

                                listPazienteNote.Add(pazienteNote)

                            End While

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listPazienteNote

        End Function

#End Region

#Region " Metodi di Update/Insert "

        ''' <summary>
        ''' Inserimento dati paziente
        ''' </summary>
        ''' <param name="paziente"></param>
        Public Sub InserisciPaziente(paziente As Paziente) Implements IPazienteProvider.InserisciPaziente

            Using cmd As New OracleCommand(Queries.Pazienti.OracleQueries.insertPaziente, Connection)

                Dim ownConnection As Boolean = False

                Try
                    If paziente.Paz_Codice <= 0 Then
                        ' Codice interno: calcolato in base alla sequence del db.
                        paziente.Paz_Codice = GetCodicePazienteDaInserire()
                    End If

                    SetInsertUpdateParameters(paziente, cmd)

                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.ExecuteNonQuery()

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

        End Sub

        ''' <summary>
        ''' Modifica dati paziente
        ''' </summary>
        ''' <param name="paziente"></param>
        Public Sub ModificaPaziente(paziente As Paziente) Implements IPazienteProvider.ModificaPaziente

            Using cmd As New OracleCommand(Queries.Pazienti.OracleQueries.updatePaziente, Connection)

                Dim ownConnection As Boolean = False

                Try
                    SetInsertUpdateParameters(paziente, cmd)

                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.ExecuteNonQuery()

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

        End Sub

        ''' <summary>
        ''' Impostazione parametri per le query di insert e update del paziente.
        ''' </summary>
        ''' <param name="paziente"></param>
        ''' <param name="cmd"></param>
        Private Sub SetInsertUpdateParameters(paziente As Paziente, cmd As OracleCommand)

            cmd.Parameters.AddWithValue("paz_codice", paziente.Paz_Codice)
            cmd.Parameters.AddWithValue("paz_cognome", GetStringParam(paziente.PAZ_COGNOME))
            cmd.Parameters.AddWithValue("paz_nome", GetStringParam(paziente.PAZ_NOME))
            cmd.Parameters.AddWithValue("paz_data_nascita", GetDateParam(paziente.Data_Nascita))
            cmd.Parameters.AddWithValue("paz_codice_fiscale", GetStringParam(paziente.PAZ_CODICE_FISCALE))
            cmd.Parameters.AddWithValue("paz_indirizzo_residenza", GetStringParam(paziente.IndirizzoResidenza))
            cmd.Parameters.AddWithValue("paz_indirizzo_domicilio", GetStringParam(paziente.IndirizzoDomicilio))
            cmd.Parameters.AddWithValue("paz_sesso", GetStringParam(paziente.Sesso))
            cmd.Parameters.AddWithValue("paz_med_codice_base", GetStringParam(paziente.MedicoBase_Codice))
            cmd.Parameters.AddWithValue("paz_cns_codice", GetStringParam(paziente.Paz_Cns_Codice))
            cmd.Parameters.AddWithValue("paz_com_codice_residenza", GetStringParam(paziente.ComuneResidenza_Codice))
            cmd.Parameters.AddWithValue("paz_cap_residenza", GetStringParam(paziente.ComuneResidenza_Cap))
            cmd.Parameters.AddWithValue("paz_data_inizio_residenza", GetDateParam(paziente.ComuneResidenza_DataInizio))
            cmd.Parameters.AddWithValue("paz_data_fine_residenza", GetDateParam(paziente.ComuneResidenza_DataFine))
            cmd.Parameters.AddWithValue("paz_com_codice_domicilio", GetStringParam(paziente.ComuneDomicilio_Codice))
            cmd.Parameters.AddWithValue("paz_cap_domicilio", GetStringParam(paziente.ComuneDomicilio_Cap))
            cmd.Parameters.AddWithValue("paz_data_inizio_domicilio", GetDateParam(paziente.ComuneDomicilio_DataInizio))
            cmd.Parameters.AddWithValue("paz_data_fine_domicilio", GetDateParam(paziente.ComuneDomicilio_DataFine))
            cmd.Parameters.AddWithValue("paz_flag_cessato", GetStringParam(paziente.FlagCessato))
            cmd.Parameters.AddWithValue("paz_aire", GetStringParam(paziente.FlagAire))
            cmd.Parameters.AddWithValue("paz_cir_codice", GetStringParam(paziente.Circoscrizione_Codice))
            cmd.Parameters.AddWithValue("paz_cir_codice_2", GetStringParam(paziente.Circoscrizione2_Codice))
            cmd.Parameters.AddWithValue("paz_cit_codice", GetStringParam(paziente.Cittadinanza_Codice))
            cmd.Parameters.AddWithValue("paz_cns_codice_old", GetStringParam(paziente.Paz_Cns_Codice_Old))
            cmd.Parameters.AddWithValue("paz_cns_data_assegnazione", GetDateParam(paziente.Paz_Cns_Data_Assegnazione))
            cmd.Parameters.AddWithValue("paz_cns_terr_codice", GetStringParam(paziente.Paz_Cns_Terr_Codice))
            cmd.Parameters.AddWithValue("paz_codice_ausiliario", GetStringParam(paziente.CodiceAusiliario))
            cmd.Parameters.AddWithValue("paz_codice_regionale", GetStringParam(paziente.PAZ_CODICE_REGIONALE))
            cmd.Parameters.AddWithValue("paz_com_codice_nascita", GetStringParam(paziente.ComuneNascita_Codice))
            cmd.Parameters.AddWithValue("paz_com_comune_emigrazione", GetStringParam(paziente.ComuneEmigrazione_Codice))
            cmd.Parameters.AddWithValue("paz_com_comune_provenienza", GetStringParam(paziente.ComuneProvenienza_Codice))
            cmd.Parameters.AddWithValue("paz_data_agg_da_anag", GetDateParam(paziente.DataAggiornamentoDaAnagrafe))
            cmd.Parameters.AddWithValue("paz_data_aire", GetDateParam(paziente.DataAire))
            cmd.Parameters.AddWithValue("paz_data_decesso", GetDateParam(paziente.DataDecesso))
            cmd.Parameters.AddWithValue("paz_data_decorrenza_med", GetDateParam(paziente.MedicoBase_DataDecorrenza))
            cmd.Parameters.AddWithValue("paz_data_emigrazione", GetDateParam(paziente.DataEmigrazione))
            cmd.Parameters.AddWithValue("paz_data_immigrazione", GetDateParam(paziente.DataImmigrazione))
            cmd.Parameters.AddWithValue("paz_data_inserimento", GetDateParam(paziente.DataInserimento))
            cmd.Parameters.AddWithValue("paz_data_irreperibilita", GetDateParam(paziente.DataIrreperibilita))
            cmd.Parameters.AddWithValue("paz_data_revoca_med", GetDateParam(paziente.MedicoBase_DataRevoca))
            cmd.Parameters.AddWithValue("paz_data_scadenza_med", GetDateParam(paziente.MedicoBase_DataScadenza))
            cmd.Parameters.AddWithValue("paz_dis_codice", GetStringParam(paziente.Distretto_Codice))
            cmd.Parameters.AddWithValue("paz_irreperibile", GetStringParam(paziente.FlagIrreperibilita))
            cmd.Parameters.AddWithValue("paz_locale", GetStringParam(paziente.FlagLocale))
            cmd.Parameters.AddWithValue("paz_regolarizzato", GetStringParam(paziente.FlagRegolarizzato))
            cmd.Parameters.AddWithValue("paz_stato", GetEnumParam(Of Enumerators.StatiVaccinali)(paziente.Stato, "d", False))
            cmd.Parameters.AddWithValue("paz_stato_anagrafico", GetEnumParam(Of Enumerators.StatoAnagrafico)(paziente.StatoAnagrafico, "d", False))
            cmd.Parameters.AddWithValue("paz_telefono_1", GetStringParam(paziente.Telefono1))
            cmd.Parameters.AddWithValue("paz_telefono_2", GetStringParam(paziente.Telefono2))
            cmd.Parameters.AddWithValue("paz_telefono_3", GetStringParam(paziente.Telefono3))
            cmd.Parameters.AddWithValue("paz_email", GetStringParam(paziente.Email))
            cmd.Parameters.AddWithValue("paz_tessera", GetStringParam(paziente.Tessera))
            cmd.Parameters.AddWithValue("paz_usl_codice_assistenza", GetStringParam(paziente.UslAssistenza_Codice))
            cmd.Parameters.AddWithValue("paz_data_inizio_ass", GetDateParam(paziente.UslAssistenza_DataInizio))
            cmd.Parameters.AddWithValue("paz_data_cessazione_ass", GetDateParam(paziente.UslAssistenza_DataCessazione))
            cmd.Parameters.AddWithValue("paz_usl_codice_residenza", GetStringParam(paziente.UslResidenza_Codice))
            cmd.Parameters.AddWithValue("paz_usl_provenienza", GetStringParam(paziente.UslAssistenzaPrecedente_Codice))
            cmd.Parameters.AddWithValue("paz_anonimo", GetStringParam(paziente.FlagAnonimo))
            cmd.Parameters.AddWithValue("paz_cat_codice", GetStringParam(paziente.CodiceCategoria1))
            cmd.Parameters.AddWithValue("paz_cag_codice", GetStringParam(paziente.CodiceCategoria2))
            cmd.Parameters.AddWithValue("paz_cancellato", GetStringParam(paziente.FlagCancellato))
            cmd.Parameters.AddWithValue("paz_codice_demografico", GetStringParam(paziente.CodiceDemografico))
            cmd.Parameters.AddWithValue("paz_completare", GetStringParam(paziente.FlagCompletare))
            cmd.Parameters.AddWithValue("paz_data_aggiornamento", GetDateParam(paziente.DataAggiornamento))
            cmd.Parameters.AddWithValue("paz_data_agg_da_comune", GetDateParam(paziente.DataAggiornamentoDaComune))
            cmd.Parameters.AddWithValue("paz_data_cancellazione", GetDateParam(paziente.DataCancellazione))
            cmd.Parameters.AddWithValue("paz_flag_stampa_maggiorenni", GetStringParam(paziente.FlagStampaMaggiorenni))
            cmd.Parameters.AddWithValue("paz_giorno", GetStringParam(paziente.Giorno))
            cmd.Parameters.AddWithValue("paz_ind_codice_res", GetIntParam(paziente.CodiceIndirizzoResidenza))
            cmd.Parameters.AddWithValue("paz_ind_codice_dom", GetIntParam(paziente.CodiceIndirizzoDomicilio))
            cmd.Parameters.AddWithValue("paz_padre", GetStringParam(paziente.Padre))
            cmd.Parameters.AddWithValue("paz_madre", GetStringParam(paziente.Madre))
            cmd.Parameters.AddWithValue("paz_occasionale", GetStringParam(paziente.FlagOccasionale))
            cmd.Parameters.AddWithValue("paz_plb_id", GetIntParam(paziente.IdElaborazione))
            cmd.Parameters.AddWithValue("paz_posizione_vaccinale_ok", GetStringParam(paziente.FlagPosizioneVaccinale))
            cmd.Parameters.AddWithValue("paz_reg_assistenza", GetStringParam(paziente.RegAssistenza))
            cmd.Parameters.AddWithValue("paz_richiesta_cartella", GetStringParam(paziente.FlagRichiestaCartella))
            cmd.Parameters.AddWithValue("paz_richiesta_certificato", GetStringParam(paziente.FlagRichiestaCertificato))
            cmd.Parameters.AddWithValue("paz_rsc_codice", GetStringParam(paziente.CodiceCategoriaRischio))
            cmd.Parameters.AddWithValue("paz_stato_acquisizione_imi", GetStringParam(paziente.StatoAcquisizioneImmigrazione))
            cmd.Parameters.AddWithValue("paz_stato_anagrafico_dett", GetStringParam(paziente.StatoAnagraficoDettaglio))
            cmd.Parameters.AddWithValue("paz_stato_notifica_emi", GetStringParam(paziente.StatoNotificaEmigrazione))
            cmd.Parameters.AddWithValue("paz_sta_certificato_emi", GetStringParam(paziente.FlagStampaCertificatoEmigrazione))
            cmd.Parameters.AddWithValue("paz_tipo", GetStringParam(paziente.Tipo))
            cmd.Parameters.AddWithValue("paz_tipo_occasionalita", GetStringParam(paziente.TipoOccasionalita))
            cmd.Parameters.AddWithValue("paz_livello_certificazione", paziente.LivelloCertificazione)
            cmd.Parameters.AddWithValue("paz_codice_esterno", GetStringParam(paziente.CodiceEsterno))
            cmd.Parameters.AddWithValue("paz_data_scadenza_ssn", GetDateParam(paziente.DataScadenzaSSN))
            cmd.Parameters.AddWithValue("PAZ_STATO_ACQUISIZIONE", GetIntParam(paziente.StatoAcquisizioneDatiVaccinaliCentrale))
            cmd.Parameters.AddWithValue("paz_id_acn", GetStringParam(paziente.IdACN))
            cmd.Parameters.AddWithValue("paz_categoria_cittadino", GetStringParam(paziente.CategoriaCittadino))
            cmd.Parameters.AddWithValue("paz_motivo_cessazione_assist", GetStringParam(paziente.MotivoCessazioneAssistenza))

        End Sub

        ''' <summary>
        ''' Update dello stato anagrafico del paziente. Restituisce il numero di record aggiornati (0 o 1).
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="statoAnagrafico"></param>
        ''' <returns></returns>
        Public Function UpdateStatoAnagrafico(codicePaziente As Integer, statoAnagrafico As Enumerators.StatoAnagrafico) As Integer Implements IPazienteProvider.UpdateStatoAnagrafico

            Dim cnt As Integer = 0

            Try
                With _DAM.QB
                    .NewQuery()
                    .AddTables("T_PAZ_PAZIENTI")
                    .AddWhereCondition("PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                    .AddUpdateField("PAZ_STATO_ANAGRAFICO", CShort(statoAnagrafico), DataTypes.Numero)
                End With

                cnt = _DAM.ExecNonQuery(ExecQueryType.Update)

            Catch ex As Exception

                SetErrorMsg("Errore durante la modifica dello stato anagrafico per il paziente.")

                LogError(ex, "UpdateStatoAnagrafico - errore modifica stato vaccinale paziente: " & codicePaziente)

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            Return cnt

        End Function

#Region " Circoscrizione "

        ''' <summary>
        ''' Update della circoscrizione di residenza del paziente. Restituisce il numero di record aggiornati (0 o 1).
        ''' </summary>
        Public Function UpdateCircoscrizioneResidenza(codicePaziente As Integer, codiceCircoscrizione As String) As Integer Implements IPazienteProvider.UpdateCircoscrizioneResidenza

            Return Me.UpdateCircoscrizione(codicePaziente, codiceCircoscrizione, True)

        End Function

        ''' <summary>
        ''' Update della circoscrizione di domicilio del paziente. Restituisce il numero di record aggiornati (0 o 1).
        ''' </summary>
        Public Function UpdateCircoscrizioneDomicilio(codicePaziente As Integer, codiceCircoscrizione As String) As Integer Implements IPazienteProvider.UpdateCircoscrizioneDomicilio

            Return Me.UpdateCircoscrizione(codicePaziente, codiceCircoscrizione, False)

        End Function

        Private Function UpdateCircoscrizione(codicePaziente As Integer, codiceCircoscrizione As String, isCircoscrizioneResidenza As Boolean) As Integer

            Dim count As Integer = 0

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                If isCircoscrizioneResidenza Then
                    cmd.CommandText = Queries.Pazienti.OracleQueries.updCircoscrizioneResidenza
                Else
                    cmd.CommandText = Queries.Pazienti.OracleQueries.updCircoscrizioneDomicilio
                End If

                cmd.Parameters.AddWithValue("cod_circ", GetStringParam(codiceCircoscrizione, False))

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

#End Region

#Region " Stato Vaccinale "

        ''' <summary>
        ''' Imposta lo stato vaccinale del paziente a quello specificato
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="newStatoVaccinale"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateStatoVaccinalePaziente(codicePaziente As Integer, newStatoVaccinale As String) As Integer Implements IPazienteProvider.UpdateStatoVaccinalePaziente

            Return UpdateStatoVaccinalePaziente(codicePaziente, String.Empty, newStatoVaccinale)

        End Function

        ''' <summary>
        ''' Esegue la modifica dello stato vaccinale del paziente specificato.
        ''' Se il parametro old_stato_vaccinale è valorizzato, controlla che lo stato sia oldStatoVaccinale prima di modificarlo a
        ''' newStatoVaccinale. Se è Nothing, esegue l'update senza controlli.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="oldStatoVaccinale"></param>
        ''' <param name="newStatoVaccinale"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateStatoVaccinalePaziente(codicePaziente As Integer, oldStatoVaccinale As String, newStatoVaccinale As String) As Integer Implements IPazienteProvider.UpdateStatoVaccinalePaziente

            Dim count As Integer = 0

            Dim query As String = String.Empty

            Using cmd As New OracleClient.OracleCommand()

                If String.IsNullOrEmpty(oldStatoVaccinale) Then
                    '---
                    ' Update stato vaccinale senza controllare lo stato vaccinale precedente
                    '---
                    query = OnVac.Queries.Pazienti.OracleQueries.updStatoVaccPaz
                    cmd.Parameters.AddWithValue("new_stato_vacc", GetStringParam(newStatoVaccinale, False))
                    cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                Else
                    '---
                    ' Update stato vaccinale con controllo dello stato vaccinale precedente
                    '---
                    query = OnVac.Queries.Pazienti.OracleQueries.updStatoVaccPazFromOldToNew
                    cmd.Parameters.AddWithValue("new_stato_vacc", GetStringParam(newStatoVaccinale, False))
                    cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                    cmd.Parameters.AddWithValue("old_stato_vacc", GetStringParam(oldStatoVaccinale, False))
                End If

                cmd.CommandText = query
                cmd.Connection = Me.Connection

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

#End Region

        ''' <summary>
        ''' Update del codice ausiliario del paziente specificato. Restituisce il numero di record aggiornati (0 o 1).
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="codiceAusiliario"></param>
        ''' <returns></returns>
        Public Function UpdateCodiceAusiliario(codicePaziente As Integer, codiceAusiliario As String) As Integer Implements IPazienteProvider.UpdateCodiceAusiliario

            Dim count As Integer = 0

            Using cmd As New OracleCommand(Queries.Pazienti.OracleQueries.updCodiceAusiliario, Connection)

                cmd.Parameters.AddWithValue("cod_aux", GetStringParam(codiceAusiliario, False))
                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)

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

        ''' <summary>
        ''' Imposta il flag Regolarizzato del paziente al valore specificato.
        ''' L'operazione non include la scrittura del log.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="flagRegolarizzato"></param>
        Public Function SetFlagRegolarizzato(codicePaziente As Integer, flagRegolarizzato As Boolean) As Integer Implements IPazienteProvider.SetFlagRegolarizzato

            With _DAM.QB
                .NewQuery()
                .AddUpdateField("paz_regolarizzato", IIf(flagRegolarizzato, "S", "N"), DataTypes.Stringa)
                .AddTables("t_paz_pazienti")
                .AddWhereCondition("paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Update)

        End Function

        ''' <summary>
        ''' Imposta il flag Richiesta Certificato del paziente al valore specificato.
        ''' L'operazione non include la scrittura del log.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="flagRichiestaCertificato"></param>
        Public Function SetFlagRichiestaCertificato(codicePaziente As Integer, flagRichiestaCertificato As Boolean) As Integer Implements IPazienteProvider.SetFlagRichiestaCertificato

            With _DAM.QB
                .NewQuery()
                .AddUpdateField("paz_richiesta_certificato", IIf(flagRichiestaCertificato, "S", "N"), DataTypes.Stringa)
                .AddTables("t_paz_pazienti")
                .AddWhereCondition("paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Update)

        End Function

        ''' <summary>
        ''' Se il flag è true imposta, per il paziente specificato, il campo paz_regolarizzato a "S", altrimenti a "N".
        ''' La modifica (compresa la scrittura del Log) avviene solo se il campo non aveva già lo stesso valore.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="regolarizzato"></param>
        Public Function RegolarizzaPaziente(codicePaziente As Integer, regolarizzato As Boolean) As Boolean Implements IPazienteProvider.RegolarizzaPaziente

            Dim count As Integer = 0

            Dim old_reg, new_reg As String
            If (regolarizzato) Then
                old_reg = "N"
                new_reg = "S"
            Else
                old_reg = "S"
                new_reg = "N"
            End If

            Using cmd As New OracleClient.OracleCommand(OnVac.Queries.Pazienti.OracleQueries.updRegolarizzaPaz, Me.Connection)

                cmd.Parameters.AddWithValue("new_reg", new_reg)
                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                cmd.Parameters.AddWithValue("old_reg", old_reg)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            ' Se c'è stata una modifica, effettuo il Log
            If (count > 0) Then
                '******** LOG
                Dim t As New DataLogStructure.Testata(DataLogStructure.TipiArgomento.VAR_ANA_AUTO, DataLogStructure.Operazione.Modifica, True)
                Dim r As New DataLogStructure.Record()

                r.Campi.Add(New DataLogStructure.Campo("PAZ_REGOLARIZZATO", old_reg, new_reg))
                t.Records.Add(r)

                LogBox.WriteData(t)
                '************
            End If

            Return (count > 0)

        End Function

        ''' <summary>
        ''' Modifica i consultori vaccinali (precedente e corrente) del paziente e la data di assegnazione.
        ''' </summary>
        Public Function UpdateConsultori(codicePaziente As Integer, consultorioOld As String, consultorioNew As String, dataAssegnazione As DateTime, updateCnsTerritoriale As Boolean) As Integer Implements IPazienteProvider.UpdateConsultori

            With _DAM.QB
                .NewQuery()
                .AddTables("t_paz_pazienti")
                .AddUpdateField("PAZ_CNS_CODICE_OLD", consultorioOld, DataTypes.Stringa)
                .AddUpdateField("PAZ_CNS_CODICE", consultorioNew, DataTypes.Stringa)
                .AddUpdateField("PAZ_CNS_DATA_ASSEGNAZIONE", dataAssegnazione, DataTypes.DataOra)
                If updateCnsTerritoriale Then .AddUpdateField("PAZ_CNS_TERR_CODICE", consultorioNew, DataTypes.Stringa)
                .AddWhereCondition("paz_codice", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Update)

        End Function

        ''' <summary>
        ''' Imposta il valore del flag cancellato per il paziente specificato.
        ''' </summary>
        Public Function SetFlagCancellato(codicePaziente As Integer, flagCancellato As Boolean) As Integer Implements IPazienteProvider.SetFlagCancellato

            Dim count As Integer = 0

            Using transactionScope As New System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions())

                Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                    cmd.CommandText = OnVac.Queries.Pazienti.OracleQueries.updFlagCancellato

                    cmd.Parameters.AddWithValue("paz_cancellato", IIf(flagCancellato, "S", "N"))
                    cmd.Parameters.AddWithValue("paz_codice", codicePaziente)

                    Dim ownConnection As Boolean = False

                    Try
                        ownConnection = Me.ConditionalOpenConnection(cmd)

                        count = cmd.ExecuteNonQuery()

                    Finally
                        Me.ConditionalCloseConnection(ownConnection)
                    End Try

                End Using

                transactionScope.Complete()

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Concatena le note a quelle eventualmente già presenti nel campo delle note solleciti.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="notePaziente"></param>
        ''' <param name="codiceUsl"></param>
        ''' <param name="idUtente"></param>
        ''' <returns></returns>
        Public Function UpdateNotePazienteSollecito(codicePaziente As Integer, notePaziente As String, codiceUsl As String, idUtente As Integer) As Integer Implements IPazienteProvider.UpdateNotePazienteSollecito

            Dim maxLengthNoteSolleciti As Integer =
                GetDimensioneColonnaDaCatalogo("T_PAZ_NOTE", "PNO_TESTO_NOTE")

            Dim idNotaSollecito As Long?
            Dim notePaz As List(Of Entities.PazienteNote) = GetNotePaziente(codicePaziente, codiceUsl)
            Dim noteSollecito As Entities.PazienteNote
            Dim testoNote As String = notePaziente
            If Not notePaz Is Nothing Then
                noteSollecito = notePaz.Where(Function(x) x.CodiceNota = Constants.CodiceTipoNotaPaziente.Solleciti).FirstOrDefault()
                If Not noteSollecito Is Nothing AndAlso noteSollecito.IdNota.HasValue Then
                    idNotaSollecito = noteSollecito.IdNota
                    If Not String.IsNullOrWhiteSpace(noteSollecito.TestoNota) Then testoNote = notePaziente + " " + noteSollecito.TestoNota
                End If
            End If
            If testoNote.Length > maxLengthNoteSolleciti Then testoNote = testoNote.Substring(0, maxLengthNoteSolleciti)

            If idNotaSollecito.HasValue Then
                Return UpdateNotePaziente(idNotaSollecito, codicePaziente, codiceUsl, Constants.CodiceTipoNotaPaziente.Solleciti, testoNote, idUtente, Date.Now)
            Else
                Return InsertNotePaziente(codicePaziente, codiceUsl, Constants.CodiceTipoNotaPaziente.Solleciti, testoNote, idUtente, Date.Now)
            End If

        End Function

        ''' <summary>
        ''' Salvataggio flag avviso maggiorenni nella t_paz_pazienti, per tutti i pazienti che soddisfano i filtri specificati
        ''' </summary>
        Public Function UpdateFlagStampaAvvisoMaggiorenni(codiceConsultorio As String, codiceComuneResidenza As String, dataNascitaIniziale As DateTime, dataNascitaFinale As DateTime, flagAvvisoMaggiorenni As String) As Integer Implements IPazienteProvider.UpdateFlagStampaAvvisoMaggiorenni

            With _DAM.QB

                .NewQuery()
                .AddTables("V_ELENCO_NON_VACCINATI_MAGG")
                .AddSelectFields("DISTINCT V_ELENCO_NON_VACCINATI_MAGG.PAZ_CODICE")
                .AddWhereCondition("V_ELENCO_NON_VACCINATI_MAGG.PAZ_DATA_NASCITA", Comparatori.MaggioreUguale, dataNascitaIniziale, DataTypes.Data)
                .AddWhereCondition("V_ELENCO_NON_VACCINATI_MAGG.PAZ_DATA_NASCITA", Comparatori.MinoreUguale, dataNascitaFinale, DataTypes.Data)
                If Not String.IsNullOrEmpty(codiceConsultorio) Then
                    .AddWhereCondition("V_ELENCO_NON_VACCINATI_MAGG.PAZ_CNS_CODICE", Comparatori.Uguale, codiceConsultorio, DataTypes.Stringa)
                End If
                If Not String.IsNullOrEmpty(codiceComuneResidenza) Then
                    .AddWhereCondition("V_ELENCO_NON_VACCINATI_MAGG.PAZ_COM_CODICE_RESIDENZA", Comparatori.Uguale, codiceComuneResidenza, DataTypes.Stringa)
                End If
                .AddWhereCondition("V_ELENCO_NON_VACCINATI_MAGG.PAZ_FLAG_STAMPA_MAGGIORENNI", Comparatori.Is, "NULL", DataTypes.Replace)

                Dim queryCodiciPazienti As String = _DAM.QB.GetSelect()

                .NewQuery(False, False)
                .AddTables("T_PAZ_PAZIENTI")
                .AddUpdateField("PAZ_FLAG_STAMPA_MAGGIORENNI", flagAvvisoMaggiorenni, DataTypes.Stringa)
                .AddWhereCondition("PAZ_CODICE", Comparatori.In, queryCodiciPazienti, DataTypes.Replace)

            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Update)

        End Function

        ''' <summary>
        ''' Update del codice regionale del paziente specificato. Restituisce il numero di record aggiornati (0 o 1).
        ''' </summary>
        Public Function UpdateCodiceRegionale(codicePaziente As Integer, codiceRegionale As String) As Integer Implements IPazienteProvider.UpdateCodiceRegionale

            Dim count As Integer = 0

            Using cmd As New OracleClient.OracleCommand(Queries.Pazienti.OracleQueries.updCodiceRegionale, Me.Connection)

                cmd.Parameters.AddWithValue("cod_reg", GetStringParam(codiceRegionale, False))
                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

#Region " Inadempienze "

        ''' <summary>
        ''' Inserimento inadempienza per il paziente e la vaccinazione specificati.
        ''' </summary>
        ''' <param name="command"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsertInadempienza(command As InsertInadempienzaCommand) As Integer Implements IPazienteProvider.InsertInadempienza

            With _DAM.QB

                .NewQuery(True, True)
                .AddTables("T_PAZ_INADEMPIENZE")
                .AddInsertField("PIN_PAZ_CODICE", command.CodicePaziente, DataTypes.Numero)
                .AddInsertField("PIN_VAC_CODICE", command.CodiceVaccinazione, DataTypes.Stringa)
                .AddInsertField("PIN_STAMPATO", command.FlagStampato, DataTypes.Stringa)
                .AddInsertField("PIN_STATO", command.StatoInadempienza, DataTypes.Stringa)
                .AddInsertField("PIN_UTE_ID", command.IdUtenteInserimento, DataTypes.Numero)
                .AddInsertField("PIN_DATA", command.DataInserimento, DataTypes.Data)

                If Not command.DateAppuntamentiRitardiPaziente Is Nothing Then

                    If command.DateAppuntamentiRitardiPaziente.DataAppuntamento1.HasValue Then
                        .AddInsertField("PIN_PRI_DATA_APPUNTAMENTO1", command.DateAppuntamentiRitardiPaziente.DataAppuntamento1.Value, DataTypes.Data)
                    End If

                    If command.DateAppuntamentiRitardiPaziente.DataAppuntamento2.HasValue Then
                        .AddInsertField("PIN_PRI_DATA_APPUNTAMENTO2", command.DateAppuntamentiRitardiPaziente.DataAppuntamento2.Value, DataTypes.Data)
                    End If

                    If command.DateAppuntamentiRitardiPaziente.DataAppuntamento3.HasValue Then
                        .AddInsertField("PIN_PRI_DATA_APPUNTAMENTO3", command.DateAppuntamentiRitardiPaziente.DataAppuntamento3.Value, DataTypes.Data)
                    End If

                    If command.DateAppuntamentiRitardiPaziente.DataAppuntamento4.HasValue Then
                        .AddInsertField("PIN_PRI_DATA_APPUNTAMENTO4", command.DateAppuntamentiRitardiPaziente.DataAppuntamento4.Value, DataTypes.Data)
                    End If

                End If

                If command.DataAppuntamento.HasValue Then
                    .AddInsertField("PIN_PRI_DATA_APPUNTAMENTO_TP", command.DataAppuntamento.Value, DataTypes.Data)
                Else
                    .AddInsertField("PIN_PRI_DATA_APPUNTAMENTO_TP", "NULL", DataTypes.Replace)
                End If

                If command.DataInvioSollecito.HasValue Then
                    .AddInsertField("PIN_PRI_DATA_STAMPA_TP", command.DataInvioSollecito.Value, DataTypes.Data)
                Else
                    .AddInsertField("PIN_PRI_DATA_STAMPA_TP", "NULL", DataTypes.Replace)
                End If

            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Insert)

        End Function

        ''' <summary>
        ''' Aggiorna i dati relativi alla stampa delle inadempienze (utente, data di stampa e flag stampato).
        ''' Se il flag setCasoConcluso= True, aggiorna il paz_stato a "Caso concluso" per tutti i pazienti in stato "Comunicazione al sindaco"
        ''' </summary>
        Public Function UpdateInadempienze(codiceConsultorio As String, listStatiAnagrafici As List(Of String), idUtente As Integer, setCasoConcluso As Boolean) As Integer Implements IPazienteProvider.UpdateInadempienze

            With _DAM.QB

                .NewQuery()

                ' Filtro sul consultorio
                If Not String.IsNullOrEmpty(codiceConsultorio) Then
                    .AddWhereCondition("PAZ_CNS_CODICE", Comparatori.Uguale, codiceConsultorio, DataTypes.Stringa)
                End If

                ' Filtro sugli stati anagrafici
                If Not listStatiAnagrafici Is Nothing AndAlso listStatiAnagrafici.Count > 0 Then

                    .OpenParanthesis()

                    .AddWhereCondition("PAZ_STATO_ANAGRAFICO", Comparatori.Uguale, listStatiAnagrafici(0), DataTypes.Stringa)
                    If listStatiAnagrafici.Count > 1 Then
                        For i As Integer = 1 To listStatiAnagrafici.Count - 1
                            .AddWhereCondition("PAZ_STATO_ANAGRAFICO", Comparatori.Uguale, listStatiAnagrafici(i), DataTypes.Stringa, "OR")
                        Next
                    End If

                    .CloseParanthesis()

                End If

                .AddTables("T_PAZ_INADEMPIENZE", "T_PAZ_PAZIENTI")
                .AddWhereCondition("PAZ_CODICE", Comparatori.Uguale, "PIN_PAZ_CODICE", DataTypes.Join)
                .AddWhereCondition("PIN_STATO", Comparatori.Uguale, Enumerators.StatoInadempienza.ComunicazioneAlSindaco, DataTypes.Stringa)
                .AddSelectFields("PAZ_CODICE")
                .IsDistinct = True

                Dim inadempientiDaAggiornare As String = .GetSelect()

                ' Aggiorna la data di stampa della CS, il campo pin_stampato = 'S' e l'utente che l'ha stampato
                ' Se il flag setCasoConcluso vale True, aggiorna anche lo stato dell'indempienza
                .NewQuery(False, False)

                .AddUpdateField("PIN_UTE_ID_STAMPA_CS", idUtente, DataTypes.Numero)
                .AddUpdateField("PIN_PRI_DATA_STAMPA_CS", Date.Now, DataTypes.DataOra)
                .AddUpdateField("PIN_STAMPATO", "S", DataTypes.Stringa)

                If setCasoConcluso Then
                    .AddUpdateField("PIN_STATO", Enumerators.StatoInadempienza.CasoConcluso, DataTypes.Stringa)
                End If

                .AddTables("T_PAZ_INADEMPIENZE")
                .AddWhereCondition("PIN_PAZ_CODICE", Comparatori.In, " (" + inadempientiDaAggiornare + ") ", DataTypes.Replace)

            End With

            Dim count As Integer = _DAM.ExecNonQuery(ExecQueryType.Update)

            Return count

        End Function

#End Region

#Region " Indirizzi "

        ''' <summary>
        ''' Update dei campi relativi alla residenza (codice e indirizzo) del paziente
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="codiceIndirizzoResidenza"></param>
        ''' <param name="descrizioneIndirizzoResidenza"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateCodiceEIndirizzoResidenzaPaziente(codicePaziente As Integer, codiceIndirizzoResidenza As String, descrizioneIndirizzoResidenza As String) As Integer Implements IPazienteProvider.UpdateCodiceEIndirizzoResidenzaPaziente

            Return Me.UpdateDatiIndirizzoPaziente(codicePaziente, codiceIndirizzoResidenza, descrizioneIndirizzoResidenza,
                                                  Queries.Pazienti.OracleQueries.updCodiceEIndirizzoResidenzaPaziente, True)

        End Function

        ''' <summary>
        ''' Update dei campi relativi al domicilio (codice e indirizzo) del paziente
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="codiceIndirizzoDomicilio"></param>
        ''' <param name="descrizioneIndirizzoDomicilio"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateCodiceEIndirizzoDomicilioPaziente(codicePaziente As Integer, codiceIndirizzoDomicilio As String, descrizioneIndirizzoDomicilio As String) As Integer Implements IPazienteProvider.UpdateCodiceEIndirizzoDomicilioPaziente

            Return Me.UpdateDatiIndirizzoPaziente(codicePaziente, codiceIndirizzoDomicilio, descrizioneIndirizzoDomicilio,
                                                  Queries.Pazienti.OracleQueries.updCodiceEIndirizzoDomicilioPaziente, True)

        End Function

        ''' <summary>
        ''' Update del solo campo indirizzo residenza del paziente
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="descrizioneIndirizzoResidenza"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateIndirizzoResidenzaPaziente(codicePaziente As Integer, descrizioneIndirizzoResidenza As String) As Integer Implements IPazienteProvider.UpdateIndirizzoResidenzaPaziente

            Return Me.UpdateDatiIndirizzoPaziente(codicePaziente, Nothing, descrizioneIndirizzoResidenza,
                                                  Queries.Pazienti.OracleQueries.updIndirizzoResidenzaPaziente, False)

        End Function

        ''' <summary>
        ''' Update del solo campo indirizzo domicilio del paziente
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="descrizioneIndirizzoDomicilio"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateIndirizzoDomicilioPaziente(codicePaziente As Integer, descrizioneIndirizzoDomicilio As String) As Integer Implements IPazienteProvider.UpdateIndirizzoDomicilioPaziente

            Return Me.UpdateDatiIndirizzoPaziente(codicePaziente, Nothing, descrizioneIndirizzoDomicilio,
                                                  Queries.Pazienti.OracleQueries.updIndirizzoDomicilioPaziente, False)

        End Function

        Private Function UpdateDatiIndirizzoPaziente(codicePaziente As Integer, codiceIndirizzo As String, descrizioneIndirizzo As String, queryUpdateIndirizzoPaziente As String, updateCodiceIndirizzo As Boolean) As Integer

            Dim count As Integer = 0

            Using cmd As New OracleClient.OracleCommand(queryUpdateIndirizzoPaziente, Me.Connection)

                If updateCodiceIndirizzo Then cmd.Parameters.AddWithValue("codiceIndirizzo", GetStringParam(codiceIndirizzo))
                cmd.Parameters.AddWithValue("descrizioneIndirizzo", GetStringParam(descrizioneIndirizzo))
                cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

#End Region


#Region " Note "

        Public Function InsertNotePaziente(codicePaziente As Long, codiceUsl As String, codiceTipoNote As String, testoNote As String, idUtenteInserimento As Integer, dataInserimento As DateTime) As Integer Implements IPazienteProvider.InsertNotePaziente
            Dim num As Integer = 0

            Dim ownConnection As Boolean = False

            Try
                Using cmd As OracleClient.OracleCommand = Connection.CreateCommand()

                    cmd.CommandText = OnVac.Queries.NotePazienti.OracleQueries.insNotePaziente

                    cmd.Parameters.AddWithValue("pno_paz_codice", codicePaziente)
                    cmd.Parameters.AddWithValue("pno_tno_codice", GetStringParam(codiceTipoNote, False))
                    cmd.Parameters.AddWithValue("pno_testo_note", GetStringParam(testoNote, False))
                    cmd.Parameters.AddWithValue("pno_paz_codice_old", DBNull.Value)
                    cmd.Parameters.AddWithValue("pno_azi_codice", GetStringParam(codiceUsl, False))
                    cmd.Parameters.AddWithValue("pno_ute_id_ultima_modifica", idUtenteInserimento)
                    cmd.Parameters.AddWithValue("pno_data_ultima_modifica", dataInserimento)

                    ownConnection = ConditionalOpenConnection(cmd)

                    num = cmd.ExecuteNonQuery()

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return num

        End Function

        Public Function UpdateNotePaziente(idNote As Long, codicePaziente As Long, codiceUsl As String, codiceTipoNote As String, testoNote As String, idUtenteInserimento As Integer, dataInserimento As DateTime) As Integer Implements IPazienteProvider.UpdateNotePaziente

            Dim count As Integer = 0

            Using cmd As New OracleClient.OracleCommand(Queries.NotePazienti.OracleQueries.updNotePaziente, Connection)

                cmd.Parameters.AddWithValue("pno_testo_note", GetStringParam(testoNote))
                cmd.Parameters.AddWithValue("pno_azi_codice", GetStringParam(codiceUsl))
                cmd.Parameters.AddWithValue("pno_ute_id_ultima_modifica", idUtenteInserimento)
                cmd.Parameters.AddWithValue("pno_data_ultima_modifica", dataInserimento)
                cmd.Parameters.AddWithValue("pno_paz_codice", codicePaziente)
                cmd.Parameters.AddWithValue("pno_tno_codice", GetStringParam(codiceTipoNote))
                cmd.Parameters.AddWithValue("pno_id", idNote)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

#End Region

        ''' <summary>
        ''' Update del solo campo id ACN del paziente
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="idACN"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateIdACNPaziente(codicePaziente As Long, idACN As String) As Integer Implements IPazienteProvider.UpdateIdACNPaziente

            Dim count As Integer = 0

            Using cmd As New OracleClient.OracleCommand(Queries.Pazienti.OracleQueries.updIdAcnPaziente, Me.Connection)

                cmd.Parameters.AddWithValue("idACN", GetStringParam(idACN, False))
                cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function


#End Region

#Region " Metodi di Delete "

        ''' <summary>
        ''' Cancellazione ritardi del paziente specificato. Restituisce il numero di ritardi cancellati.
        ''' </summary>
        Public Function EliminaRitardiPaziente(codicePaziente As Integer) As Integer Implements IPazienteProvider.EliminaRitardiPaziente

            Dim count As Integer = 0

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                cmd.CommandText = OnVac.Queries.Pazienti.OracleQueries.delRitardiPaziente

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Cancellazione cicli del paziente specificato. Restituisce il numero di cicli cancellati dalla t_paz_cicli.
        ''' </summary>
        Public Function EliminaCicliPaziente(codicePaziente As Integer) As Integer Implements IPazienteProvider.EliminaCicliPaziente

            Dim count As Integer = 0

            Using cmd As OracleClient.OracleCommand = Me.Connection.CreateCommand()

                cmd.CommandText = OnVac.Queries.Pazienti.OracleQueries.delCicliPaziente

                cmd.Parameters.AddWithValue("cod_paz", codicePaziente)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Cancellazione inadempienza del paziente relativa alla vaccinazione specificata. Restituisce il numero di record cancellati.
        ''' </summary>
        Public Function DeleteInadempienza(codicePaziente As Integer, codiceVaccinazione As String) As Integer Implements IPazienteProvider.DeleteInadempienza

            With _DAM.QB

                .NewQuery()

                .AddTables("T_PAZ_INADEMPIENZE")

                .AddWhereCondition("PIN_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("PIN_VAC_CODICE", Comparatori.Uguale, codiceVaccinazione, DataTypes.Stringa)

            End With

            Return _DAM.ExecNonQuery(ExecQueryType.Delete)

        End Function

#End Region

#Region " Controllo campi paziente "

        ''' <summary>
        ''' Restituisce una lista con i dati dei controlli da eseguire sui campi del paziente
        ''' </summary>
        Public Function GetControlliCampiPaziente(funzione As Constants.FunzioniControlloCampiPaziente) As List(Of Entities.ControlloCampiPaziente) Implements IPazienteProvider.GetControlliCampiPaziente

            Dim listControlli As New List(Of Entities.ControlloCampiPaziente)

            Using cmd As OracleCommand = New OracleCommand(Queries.Pazienti.OracleQueries.selControlliCampiPaziente, Me.Connection)

                cmd.Parameters.AddWithValue("funzione", funzione)
                cmd.Parameters.AddWithValue("tipo", Constants.TipoControlloCampiPaziente.NessunControllo)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using dr As IDataReader = cmd.ExecuteReader()

                        If Not dr Is Nothing Then

                            Dim controllo As Entities.ControlloCampiPaziente

                            Dim cdp_funzione As Integer = dr.GetOrdinal("cdp_funzione")
                            Dim cdp_ordine As Integer = dr.GetOrdinal("cdp_ordine")
                            Dim cdp_tipo As Integer = dr.GetOrdinal("cdp_tipo")
                            Dim cdp_campi As Integer = dr.GetOrdinal("cdp_campi")
                            Dim cdp_messaggio As Integer = dr.GetOrdinal("cdp_messaggio")

                            While dr.Read()

                                controllo = New Entities.ControlloCampiPaziente()

                                controllo.Funzione = dr.GetStringOrDefault(cdp_funzione)
                                controllo.Ordine = dr.GetInt32OrDefault(cdp_ordine)
                                controllo.Tipo = dr.GetStringOrDefault(cdp_tipo)
                                controllo.Campi = dr.GetStringOrDefault(cdp_campi)
                                controllo.Messaggio = dr.GetStringOrDefault(cdp_messaggio)

                                listControlli.Add(controllo)

                            End While

                        End If

                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listControlli

        End Function

#End Region

        Public Function GetReadCommittedTransactionOptions() As Transactions.TransactionOptions

            Dim transactionOptions As New Transactions.TransactionOptions()
            transactionOptions.IsolationLevel = Transactions.IsolationLevel.ReadCommitted

            Return transactionOptions

        End Function

        Public Function GetStatoAcquisizioneDatiVaccinaliCentrale(codicePaziente As Long) As Enumerators.StatoAcquisizioneDatiVaccinaliCentrale? Implements IPazienteProvider.GetStatoAcquisizioneDatiVaccinaliCentrale

            Dim statoAcquisizioneDatiVaccinaliCentrale As Enumerators.StatoAcquisizioneDatiVaccinaliCentrale? = Nothing

            Using cmd As OracleCommand = New OracleCommand("SELECT PAZ_STATO_ACQUISIZIONE FROM T_PAZ_PAZIENTI WHERE PAZ_CODICE=:PAZ_CODICE", Me.Connection)

                cmd.Parameters.AddWithValue("PAZ_CODICE", codicePaziente)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim statoAcquisizione As String = cmd.ExecuteScalar().ToString()

                    If Not String.IsNullOrEmpty(statoAcquisizione) Then
                        statoAcquisizioneDatiVaccinaliCentrale = [Enum].Parse(GetType(Enumerators.StatoAcquisizioneDatiVaccinaliCentrale), [Enum].GetName(GetType(Enumerators.StatoAcquisizioneDatiVaccinaliCentrale), Convert.ToInt32(statoAcquisizione)))
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return statoAcquisizioneDatiVaccinaliCentrale

        End Function

        Public Function GetPazienteDistribuito(codiceAusiliarioPaziente As String, codiceUsl As String) As Entities.PazienteDistribuito Implements IPazienteProvider.GetPazienteDistribuito

            Dim pazienteDistribuito As Entities.PazienteDistribuito = Nothing

            Using cmd As OracleCommand = New OracleCommand("SELECT PAZ_CODICE FROM T_PAZ_PAZIENTI_DISTRIBUITI WHERE PAZ_CODICE_AUSILIARIO=:PAZ_CODICE_AUSILIARIO AND ULSS=:ULSS", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("PAZ_CODICE_AUSILIARIO", codiceAusiliarioPaziente)
                    cmd.Parameters.AddWithValue("ULSS", codiceUsl.Substring(3))

                    Using reader As OracleClient.OracleDataReader = cmd.ExecuteReader()

                        If reader.Read() Then

                            pazienteDistribuito = New Entities.PazienteDistribuito()
                            pazienteDistribuito.CodiceAusiliarioPaziente = codiceAusiliarioPaziente
                            pazienteDistribuito.CodiceUsl = codiceUsl

                            pazienteDistribuito.CodicePaziente = reader.GetInt64(0)

                        End If

                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return pazienteDistribuito

        End Function

        ''' <summary>
        ''' Restituisce tutti i codici locali e le usl in cui sono distribuiti i pazienti centrali specificati
        ''' </summary>
        ''' <param name="listCodiciCentraliPazienti"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetInfoDistribuzioneCompletaPazienti(listCodiciCentraliPazienti As List(Of String)) As List(Of Entities.PazienteInfoDistribuzione) Implements IPazienteProvider.GetInfoDistribuzioneCompletaPazienti

            Dim listInfoPazienti As List(Of Entities.PazienteInfoDistribuzione) = Nothing

            Using cmd As OracleCommand = New OracleCommand()

                cmd.Connection = Connection

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Dim query As String = "select paz_codice_ausiliario codice_centrale, paz_codice codice_locale, ugs_app_id app_id" +
                        " from t_paz_pazienti_distribuiti " +
                        " join t_usl_gestite on ugs_usl_codice = '050' || ulss  " +
                        " where {0} "

                    Dim filtroQueryPazienti As String = String.Empty

                    If listCodiciCentraliPazienti.Count = 1 Then

                        filtroQueryPazienti = " paz_codice_ausiliario = :codicePaziente "
                        cmd.Parameters.AddWithValue("codicePaziente", listCodiciCentraliPazienti.First())

                    Else

                        Dim filtroPazientiIn As GetInFilterResult = Me.GetInFilter(listCodiciCentraliPazienti)
                        filtroQueryPazienti = String.Format(" paz_codice_ausiliario IN ({0}) ", filtroPazientiIn.InFilter)

                        cmd.Parameters.AddRange(filtroPazientiIn.Parameters)

                    End If

                    cmd.CommandText = String.Format(query, filtroQueryPazienti)

                    listInfoPazienti = GetListPazienteInfoDistribuzione(cmd)

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listInfoPazienti

        End Function

        ''' <summary>
        ''' Restituisce true se il paziente è totalmente inadempiente per le vaccinazioni obbligatorie.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsTotalmenteInadempiente(codicePaziente As Long) As Boolean Implements IPazienteProvider.IsTotalmenteInadempiente

            With _DAM.QB

                .NewQuery()
                .AddTables("T_PAZ_INADEMPIENZE")
                .AddSelectFields("VAC_CODICE")
                .AddWhereCondition("PIN_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Numero)
                .AddWhereCondition("VAC_CODICE", Comparatori.Uguale, "PIN_VAC_CODICE", DataTypes.Join)

                Dim q1 As String = .GetSelect()

                .NewQuery(False, False)
                .AddTables("T_ANA_VACCINAZIONI")
                .AddSelectFields("1")
                .AddWhereCondition("VAC_OBBLIGATORIA", Comparatori.Uguale, Constants.ObbligatorietaVaccinazione.Obbligatoria, DataTypes.Stringa)
                .AddWhereCondition("VAC_COD_SOSTITUTA", Comparatori.Is, "NULL", DataTypes.Replace)
                .AddWhereCondition("", Comparatori.NotExist, "(" + q1 + ")", DataTypes.Replace)

            End With

            Dim obj As Object = _DAM.ExecScalar()

            Return (obj Is Nothing OrElse obj Is DBNull.Value)

        End Function

        ''' <summary>
        ''' Restituisce il testo dell'esito della situazione vaccinale del paziente
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        Public Function GetEsitoControlloSituazioneVaccinale(codicePaziente As Long) As String Implements IPazienteProvider.GetEsitoControlloSituazioneVaccinale

            Dim esito As String = String.Empty

            Using cmd As OracleCommand = New OracleCommand("select pec_testo from v_paz_esito_controlli where pec_paz_codice = :pec_paz_codice", Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("pec_paz_codice", codicePaziente)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        esito = obj.ToString()
                    End If

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return esito

        End Function

#Region " Consenso "

        ''' <summary>
        ''' Restituisce la lista con tutti i consensi registrati (del tipo specificato) per il paziente. 
        ''' Restituisce i consensi centralizzati e quelli non centralizzati registrati dall'azienda specificata.
        ''' Non restituisce i consensi non rilevati (perchè non legge dalla v_paz_ultimo_consenso ma dalla v_paz_consensi).
        ''' </summary>
        ''' <param name="codiceCentralePaziente"></param>
        ''' <param name="idConsensi"></param>
        ''' <param name="codiceAziendaRegistrazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetConsensiRegistratiPazienteAzienda(codiceCentralePaziente As String, idConsensi As List(Of Integer), codiceAziendaRegistrazione As String) As List(Of Entities.Consenso.ConsensoRegistrato) Implements IPazienteProvider.GetConsensiRegistratiPazienteAzienda

            Dim listUltimiConsensi As List(Of Entities.Consenso.ConsensoRegistrato) = Nothing

            Using cmd As OracleCommand = New OracleCommand()

                cmd.Connection = Me.Connection

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim filtroId As GetInFilterResult = Me.GetInFilter(idConsensi)

                    cmd.CommandText = Me.GetQuerySelectUltimoConsenso("v_paz_consensi") +
                        " WHERE con_paz_codice = :codiceCentralePaziente " +
                        " AND (con_centralizzato = 'S' or (con_centralizzato = 'N' and con_azi_codice = :codiceAzienda) ) " +
                        String.Format(" AND con_id_consenso in ({0}) ", filtroId.InFilter)

                    cmd.Parameters.AddWithValue("codiceCentralePaziente", codiceCentralePaziente)
                    cmd.Parameters.AddWithValue("codiceAzienda", codiceAziendaRegistrazione)
                    cmd.Parameters.AddRange(filtroId.Parameters)

                    listUltimiConsensi = Me.GetListConsensiRegistrati(cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listUltimiConsensi

        End Function

        ''' <summary>
        ''' Inserimento del consenso specificato
        ''' </summary>
        ''' <param name="consenso"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InsertConsenso(consenso As Entities.Consenso.ConsensoPaziente) As Integer Implements IPazienteProvider.InsertConsenso

            Dim count As Integer = 0

            Dim query As String =
                "INSERT INTO t_paz_consensi" +
                "(con_progressivo, con_paz_codice, con_data_evento, con_data_scadenza, con_lco_id, con_azienda, " +
                " con_eco_id, con_addetto, con_data_registrazione, con_applicativo, con_azi_codice, " +
                " con_custom1, con_custom2, con_custom3) " +
                " VALUES " +
                "(:con_progressivo, :con_paz_codice, :con_data_evento, :con_data_scadenza, :con_lco_id, :con_azienda, " +
                " :con_eco_id, :con_addetto, :con_data_registrazione, :con_applicativo, :con_azi_codice, " +
                " :con_custom1, :con_custom2, :con_custom3) "

            Using cmd As New OracleClient.OracleCommand(query, Me.Connection)

                cmd.Parameters.AddWithValue("con_progressivo", consenso.ProgressivoConsenso)
                cmd.Parameters.AddWithValue("con_paz_codice", consenso.CodicePaziente)
                cmd.Parameters.AddWithValue("con_data_evento", consenso.DataEvento)

                If consenso.DataScadenza.HasValue Then
                    cmd.Parameters.AddWithValue("con_data_scadenza", consenso.DataScadenza.Value)
                Else
                    cmd.Parameters.AddWithValue("con_data_scadenza", DBNull.Value)
                End If

                cmd.Parameters.AddWithValue("con_lco_id", consenso.IdLivello)

                If String.IsNullOrWhiteSpace(consenso.Azienda) Then
                    cmd.Parameters.AddWithValue("con_azienda", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("con_azienda", consenso.Azienda)
                End If

                cmd.Parameters.AddWithValue("con_eco_id", consenso.IdEnte)

                If String.IsNullOrWhiteSpace(consenso.Addetto) Then
                    cmd.Parameters.AddWithValue("con_addetto", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("con_addetto", consenso.Addetto)
                End If

                cmd.Parameters.AddWithValue("con_data_registrazione", consenso.DataRegistrazione)

                If String.IsNullOrWhiteSpace(consenso.Applicativo) Then
                    cmd.Parameters.AddWithValue("con_applicativo", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("con_applicativo", consenso.Applicativo)
                End If

                cmd.Parameters.AddWithValue("con_azi_codice", consenso.CodiceAzienda)

                If String.IsNullOrWhiteSpace(consenso.CampoCustom1) Then
                    cmd.Parameters.AddWithValue("con_custom1", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("con_custom1", consenso.CampoCustom1)
                End If

                If String.IsNullOrWhiteSpace(consenso.CampoCustom2) Then
                    cmd.Parameters.AddWithValue("con_custom2", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("con_custom2", consenso.CampoCustom2)
                End If

                If String.IsNullOrWhiteSpace(consenso.CampoCustom3) Then
                    cmd.Parameters.AddWithValue("con_custom3", DBNull.Value)
                Else
                    cmd.Parameters.AddWithValue("con_custom3", consenso.CampoCustom3)
                End If

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    count = cmd.ExecuteNonQuery()

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return count

        End Function

        ''' <summary>
        ''' Restituisce l'id dell'ente di default in base al consenso specificato
        ''' </summary>
        ''' <param name="idConsenso"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetIdEnteDefaultConsenso(idConsenso As Integer) As Integer Implements IPazienteProvider.GetIdEnteDefaultConsenso

            Dim idEnte As Integer = 0

            Using cmd As OracleCommand = New OracleCommand("SELECT eco_id FROM t_ana_enti_consenso WHERE eco_csn_id = :eco_csn_id AND eco_default = 'S'", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("eco_csn_id", idConsenso)

                    Dim obj As Object = cmd.ExecuteScalar()
                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        idEnte = Convert.ToInt32(obj)
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return idEnte

        End Function

        ''' <summary>
        ''' Restituisce la descrizione del consenso, in base all'id specificato
        ''' </summary>
        ''' <param name="idConsenso"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDescrizioneConsensoByIdConsenso(idConsenso As Integer) As String Implements IPazienteProvider.GetDescrizioneConsensoByIdConsenso

            Dim descrizione As String = String.Empty

            Using cmd As OracleCommand = New OracleCommand("SELECT csn_descrizione FROM t_ana_consensi WHERE csn_id = :csn_id", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("csn_id", idConsenso)

                    Dim obj As Object = cmd.ExecuteScalar()
                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        descrizione = obj.ToString()
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return descrizione

        End Function

        ''' <summary>
        ''' Restituisce la descrizione del livello, in base all'id specificato
        ''' </summary>
        ''' <param name="idLivelloConsenso"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDescrizioneLivelloConsensoByIdLivello(idLivelloConsenso As Integer) As String Implements IPazienteProvider.GetDescrizioneLivelloConsensoByIdLivello

            Dim descrizione As String = String.Empty

            Using cmd As OracleCommand = New OracleCommand("SELECT lco_descrizione FROM t_ana_livelli_consenso WHERE lco_id = :lco_id", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("lco_id", idLivelloConsenso)

                    Dim obj As Object = cmd.ExecuteScalar()
                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        descrizione = obj.ToString()
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return descrizione

        End Function

        ''' <summary>
        ''' Restituisce il tipo di evento in base all'id del livello specificato
        ''' </summary>
        ''' <param name="idLivelloConsenso"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetTipoEventoByIdLivelloConsenso(idLivelloConsenso As Integer) As String Implements IPazienteProvider.GetTipoEventoByIdLivelloConsenso

            Dim tipoEvento As String = String.Empty

            Using cmd As OracleCommand = New OracleCommand("SELECT lco_evento FROM t_ana_livelli_consenso WHERE lco_id = :lco_id", Me.Connection)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.Parameters.AddWithValue("lco_id", idLivelloConsenso)

                    Dim obj As Object = cmd.ExecuteScalar()
                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        tipoEvento = obj.ToString()
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return tipoEvento

        End Function

        ''' <summary>
        ''' Restituisce gli ultimi consensi registrati per i pazienti
        ''' </summary>
        ''' <param name="listCodiciPazienti"></param>
        ''' <param name="filtroIdConsenso"></param>
        ''' <param name="filtroFlagCalcoloStatoGlobale"></param>
        ''' <param name="codiceAziendaRegistrazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetUltimiConsensiRegistrati(listCodiciPazienti As List(Of String), filtroIdConsenso As Integer?, filtroFlagCalcoloStatoGlobale As Boolean?, codiceAziendaRegistrazione As String) As List(Of Entities.Consenso.ConsensoRegistrato) Implements IPazienteProvider.GetUltimiConsensiRegistrati

            Dim listConsensi As New List(Of Entities.Consenso.ConsensoRegistrato)()

            If listCodiciPazienti Is Nothing OrElse listCodiciPazienti.Count = 0 Then
                Throw New ApplicationException("GetUltimiConsensiRegistrati non supporta l'invocazione senza almeno un codice paziente")
            End If

            Using cmd As OracleCommand = New OracleCommand()

                cmd.Connection = Me.Connection

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim query As String =
                        "SELECT con_paz_codice, con_tipo_consenso, con_id_consenso, con_consenso, con_tipo_evento, con_data_evento, " +
                        " con_data_scadenza, con_id_livello, con_livello, con_lco_descrizione, con_azienda, con_id_ente, con_ente, con_addetto, con_data_registrazione, " +
                        " con_applicativo, con_lco_flag_controllo, con_lco_san_codice_mnemonico, con_lco_scn_id_stato_consenso, con_scn_descrizione, con_scn_url_icona, " +
                        " con_scn_ordinamento, con_obsoleto, con_centralizzato, con_ordinamento, con_calcolo_stato_globale, con_blocco_accessi_esterni, " +
                        " con_custom1, con_custom1_flag_gestione, con_custom1_etaminrilevazione, " +
                        " con_custom2, con_custom2_flag_gestione, con_custom2_etaminrilevazione, " +
                        " con_custom3, con_custom3_flag_gestione, con_custom3_etaminrilevazione, " +
                        " con_progressivo, con_azi_codice " +
                        " FROM   v_paz_consensi a " +
                        " WHERE   a.con_paz_codice in ({0}) " +
                        " AND (con_obsoleto is null OR con_obsoleto = 'N') " +
                        " AND (con_centralizzato = 'S' OR (con_centralizzato = 'N' and a.con_azi_codice = :codiceAzienda)) " +
                        " AND NOT EXISTS " +
                        " ( SELECT 1 " +
                        "   FROM   t_paz_consensi b, t_ana_livelli_consenso c, t_ana_consensi d " +
                        "   WHERE b.con_lco_id = c.lco_id " +
                        "   AND c.lco_csn_id = d.csn_id " +
                        "   AND b.con_paz_codice = a.con_paz_codice " +
                        "   AND c.lco_csn_id = a.con_id_consenso " +
                        "   AND ((b.con_azi_codice = a.con_azi_codice and d.csn_centralizzato = 'N') or d.csn_centralizzato = 'S') " +
                        "   AND b.con_progressivo > a.con_progressivo) "

                    cmd.Parameters.AddWithValue("codiceAzienda", codiceAziendaRegistrazione)

                    If filtroIdConsenso.HasValue Then
                        query += " AND a.con_id_consenso = :idConsenso "
                        cmd.Parameters.AddWithValue("idConsenso", filtroIdConsenso.Value)
                    End If

                    If filtroFlagCalcoloStatoGlobale.HasValue Then

                        If filtroFlagCalcoloStatoGlobale.Value Then
                            query += " AND con_calcolo_stato_globale = 'S' "
                        Else
                            query += " AND (con_calcolo_stato_globale = 'N' or con_calcolo_stato_globale is null) "
                        End If

                    End If

                    Dim filtroCodiciPazienti As GetInFilterResult = Me.GetInFilter(listCodiciPazienti)
                    cmd.Parameters.AddRange(filtroCodiciPazienti.Parameters)

                    cmd.CommandText = String.Format(query, filtroCodiciPazienti.InFilter)

                    listConsensi = Me.GetListConsensiRegistrati(cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listConsensi

        End Function

        ''' <summary>
        ''' Restituisce tutti i consensi dell'azienda specificata con i valori di default per il caso "non rilevato".
        ''' </summary>
        ''' <param name="codiceAziendaCentrale"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetConsensiLivelliDefaultNonRilevato(codiceAziendaCentrale As String) As List(Of Entities.Consenso.ConsensoRegistrato) Implements IPazienteProvider.GetConsensiLivelliDefaultNonRilevato

            Dim listConsensi As New List(Of Entities.Consenso.ConsensoRegistrato)()

            Using cmd As OracleCommand = New OracleCommand()

                cmd.Connection = Me.Connection

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim query As String =
                        "SELECT tcn_descrizione con_tipo_consenso, csn_id con_id_consenso, csn_descrizione con_consenso, lco_evento con_tipo_evento, " +
                        "   lco_id con_id_livello, lco_codice_mnemonico con_livello, lco_descrizione con_lco_descrizione, lco_flag_controllo con_lco_flag_controllo, " +
                        "   lco_san_codice_mnemonico con_lco_san_codice_mnemonico, lco_scn_id_stato_consenso con_lco_scn_id_stato_consenso, " +
                        "   scn_descrizione con_scn_descrizione, scn_url_icona con_scn_url_icona, scn_ordinamento con_scn_ordinamento, " +
                        "   SN_OR (tcn_obsoleto, csn_obsoleto) con_obsoleto, csn_centralizzato con_centralizzato, csn_ordinamento con_ordinamento, " +
                        "   csn_calcolo_stato_globale con_calcolo_stato_globale, lco_blocco_accessi_esterni con_blocco_accessi_esterni, " +
                        "   tcn_azi_codice con_azi_codice, null con_progressivo, null con_paz_codice, null con_data_evento, null con_data_scadenza, " +
                        "   null con_custom1, lco_custom1_flag_gestione con_custom1_flag_gestione, lco_custom1_etaminrilevazione con_custom1_etaminrilevazione, " +
                        "   null con_custom2, lco_custom2_flag_gestione con_custom2_flag_gestione, lco_custom2_etaminrilevazione con_custom2_etaminrilevazione, " +
                        "   null con_custom3, lco_custom3_flag_gestione con_custom3_flag_gestione, lco_custom3_etaminrilevazione con_custom3_etaminrilevazione, " +
                        "   null con_azienda, null con_id_ente, null con_ente, null con_addetto, null con_data_registrazione, null con_applicativo " +
                        "FROM t_ana_tipi_consenso, t_ana_consensi a, t_ana_livelli_consenso b, t_ana_stati_consenso " +
                        "WHERE tcn_id = a.csn_tcn_id AND tcn_azi_codice = a.csn_azi_codice " +
                        " AND a.csn_id = lco_csn_id AND a.csn_azi_codice = lco_azi_codice " +
                        " AND scn_id = lco_scn_id_stato_consenso " +
                        " AND lco_flag_default_non_rilevato = 'S' " +
                        " AND (con_obsoleto is null OR con_obsoleto = 'N') " +
                        " AND csn_azi_codice = :codiceAzienda "

                    cmd.Parameters.AddWithValue("codiceAzienda", codiceAziendaCentrale)

                    cmd.CommandText = query

                    listConsensi = Me.GetListConsensiRegistrati(cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listConsensi

        End Function

#End Region

#Region " OnVac API "

        ''' <summary>
        ''' Restituisce l'ultimo consenso registrato (del tipo indicato) per il paziente specificato.
        ''' </summary>
        ''' <param name="codiceCentralePaziente"></param>
        ''' <param name="idConsenso"></param>
        ''' <param name="codiceAziendaRegistrazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetUltimoConsensoRegistratoPaziente(codiceCentralePaziente As String, idConsenso As Integer, codiceAziendaRegistrazione As String) As Entities.Consenso.ConsensoRegistrato Implements IPazienteProvider.GetUltimoConsensoRegistratoPaziente

            Dim ultimoConsenso As Entities.Consenso.ConsensoRegistrato = Nothing

            Using cmd As OracleCommand = New OracleCommand()

                cmd.Connection = Me.Connection

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.CommandText = Me.GetQuerySelectUltimoConsenso("v_paz_ultimo_consenso") +
                        " WHERE con_paz_codice = :codiceCentralePaziente " +
                        " AND con_id_consenso = :idConsenso " +
                        " AND (con_centralizzato = 'S' or (con_centralizzato = 'N' and con_azi_codice = :codiceAzienda) or con_progressivo is null) "

                    cmd.Parameters.AddWithValue("codiceCentralePaziente", codiceCentralePaziente)
                    cmd.Parameters.AddWithValue("idConsenso", idConsenso)
                    cmd.Parameters.AddWithValue("codiceAzienda", codiceAziendaRegistrazione)

                    Dim listConsensi As List(Of Entities.Consenso.ConsensoRegistrato) = Me.GetListConsensiRegistrati(cmd)

                    If Not listConsensi Is Nothing AndAlso listConsensi.Count > 0 Then
                        ultimoConsenso = listConsensi.First()
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return ultimoConsenso

        End Function

        ''' <summary>
        ''' Restituisce i codici di tutti i pazienti collegati al referente specificato.
        ''' </summary>
        ''' <param name="codiceCentralePazienteReferente"></param>
        ''' <param name="idConsenso"></param>
        ''' <param name="codiceAzienda"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetUltimoConsensoRegistratoPazientiCollegati(codiceCentralePazienteReferente As String, idConsenso As Integer, codiceAzienda As String) As List(Of Entities.Consenso.ConsensoRegistrato) Implements IPazienteProvider.GetUltimoConsensoRegistratoPazientiCollegati

            Dim consensi As List(Of Entities.Consenso.ConsensoRegistrato) = Nothing

            Using cmd As OracleCommand = New OracleCommand()

                cmd.Connection = Me.Connection

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.CommandText = Me.GetQuerySelectUltimoConsenso("v_paz_ultimo_consenso") +
                        " JOIN t_paz_consensi_collegati ON ccl_con_paz_codice_paziente = con_paz_codice AND ccl_csn_id_consenso = con_id_consenso " +
                        " WHERE con_id_consenso = :idConsenso " +
                        " AND ccl_con_paz_codice_referente = :codiceCentraleReferente " +
                        " AND (con_centralizzato = 'S' or (con_centralizzato = 'N' and con_azi_codice = :codiceAzienda) or con_progressivo is null) "

                    cmd.Parameters.AddWithValue("idConsenso", idConsenso)
                    cmd.Parameters.AddWithValue("codiceCentraleReferente", codiceCentralePazienteReferente)
                    cmd.Parameters.AddWithValue("codiceAzienda", codiceAzienda)

                    consensi = Me.GetListConsensiRegistrati(cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return consensi

        End Function

        ''' <summary>
        ''' Restituisce codice centrale, codice locale e ulss in cui è presente ogni paziente specificato in base al codice centrale
        ''' </summary>
        ''' <param name="listCodiciCentraliPazienti"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetListInfoDistribuzionePazienti(listCodiciCentraliPazienti As List(Of String)) As List(Of Entities.PazienteInfoDistribuzione) Implements IPazienteProvider.GetListInfoDistribuzionePazienti

            Dim listInfoPazienti As List(Of Entities.PazienteInfoDistribuzione) = Nothing

            Using cmd As OracleCommand = New OracleCommand()

                cmd.Connection = Me.Connection

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Dim query As String = "SELECT b.codice_centrale, u.paz_codice codice_locale, USU_APP_ID app_id" +
                        " FROM ( " +
                        "   SELECT a.paz_codice codice_centrale, DECODE(a.usl_dom, null, a.usl_res, a.usl_dom) usl " +
                        "   FROM ( " +
                        "       SELECT paz_codice, paz_com_codice_domicilio, paz_com_codice_residenza, dom.lcu_usl_codice usl_dom, res.lcu_usl_codice usl_res " +
                        "       FROM t_paz_pazienti " +
                        "           LEFT JOIN t_ana_link_comuni_usl dom ON paz_com_codice_domicilio = dom.lcu_com_codice " +
                        "           LEFT JOIN t_ana_link_comuni_usl res ON paz_com_codice_residenza = res.lcu_com_codice " +
                        "       {0} ) a " +
                        " ) b " +
                        " JOIN ( " +
                        "   SELECT USU_CODICE, USU_APP_ID, '050500' AZI " +
                        "   FROM T_USL_UNIFICATE " +
                        "   UNION SELECT UGS_USL_CODICE, UGS_APP_ID USU_APP_ID, UGS_USL_CODICE AZI " +
                        "   FROM T_USL_GESTITE " +
                        " ) v ON v.USU_CODICE = b.usl " +
                        " JOIN v_paz_ulss_app u ON u.paz_codice_ausiliario = b.codice_centrale AND u.USL_CODICE = v.AZI "

                    Dim filtroQueryPazienti As String = String.Empty

                    If listCodiciCentraliPazienti.Count = 1 Then

                        filtroQueryPazienti = " WHERE paz_codice = :codicePaziente "
                        cmd.Parameters.AddWithValue("codicePaziente", listCodiciCentraliPazienti.First())

                    Else

                        Dim filtroPazientiIn As GetInFilterResult = Me.GetInFilter(listCodiciCentraliPazienti)
                        filtroQueryPazienti = String.Format(" WHERE paz_codice IN ({0}) ", filtroPazientiIn.InFilter)

                        cmd.Parameters.AddRange(filtroPazientiIn.Parameters)

                    End If

                    cmd.CommandText = String.Format(query, filtroQueryPazienti)

                    listInfoPazienti = GetListPazienteInfoDistribuzione(cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listInfoPazienti

        End Function

        ''' <summary>
        ''' Restituisce la collection dei pazienti con stato anagrafico attivo tra quelli indicati.
        ''' Lo stato anagrafico è attivo se il campo san_chiamata vale "S"
        ''' </summary>
        ''' <param name="listCodiciLocaliPazienti"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetPazientiStatoAnagraficoAttivo(listCodiciLocaliPazienti As List(Of Int64)) As List(Of Entities.DatiPazienteAPP) Implements IPazienteProvider.GetPazientiStatoAnagraficoAttivo

            Dim listPaz As List(Of Entities.DatiPazienteAPP) = Nothing

            Using cmd As OracleCommand = New OracleCommand()

                cmd.Connection = Me.Connection

                Dim ownConnection As Boolean = False

                Dim query As String = "select paz_codice, paz_cognome, paz_nome, paz_data_nascita, paz_sesso, paz_codice_fiscale " +
                    " from t_paz_pazienti " +
                    " join t_ana_stati_anagrafici on paz_stato_anagrafico = san_codice " +
                    " where san_chiamata = 'S' " +
                    " {0} "

                Dim filtroQueryPazienti As String = String.Empty

                If listCodiciLocaliPazienti.Count = 1 Then

                    filtroQueryPazienti = " and paz_codice = :codicePaziente "
                    cmd.Parameters.AddWithValue("codicePaziente", listCodiciLocaliPazienti.First())

                Else

                    Dim filtroPazientiIn As GetInFilterResult = Me.GetInFilter(listCodiciLocaliPazienti)
                    filtroQueryPazienti = String.Format(" and paz_codice in ({0}) ", filtroPazientiIn.InFilter)

                    cmd.Parameters.AddRange(filtroPazientiIn.Parameters)

                End If

                cmd.CommandText = String.Format(query, filtroQueryPazienti)

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    listPaz = Me.GetListDatiPazienteAPP(cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listPaz

        End Function

        ''' <summary>
        ''' Restituisce i dati dei pazienti specificati
        ''' </summary>
        ''' <param name="listCodiciLocaliPazienti"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDatiPazientiAPP(listCodiciLocaliPazienti As List(Of Long)) As List(Of Entities.DatiPazienteAPP) Implements IPazienteProvider.GetDatiPazientiAPP

            Dim listPaz As List(Of Entities.DatiPazienteAPP) = Nothing

            Using cmd As OracleCommand = New OracleCommand()

                cmd.Connection = Me.Connection

                Dim ownConnection As Boolean = False

                Dim query As String =
                    "select paz_codice, paz_cognome, paz_nome, paz_data_nascita, paz_sesso, paz_codice_fiscale from t_paz_pazienti where "

                If listCodiciLocaliPazienti.Count = 1 Then

                    cmd.CommandText = query + " paz_codice = :codicePaziente"
                    cmd.Parameters.AddWithValue("codicePaziente", listCodiciLocaliPazienti.First())

                Else

                    query += " paz_codice in ({0}) "

                    Dim filtroPazientiIn As GetInFilterResult = Me.GetInFilter(listCodiciLocaliPazienti)

                    cmd.CommandText = String.Format(query, filtroPazientiIn.InFilter)
                    cmd.Parameters.AddRange(filtroPazientiIn.Parameters)

                End If

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    listPaz = Me.GetListDatiPazienteAPP(cmd)

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return listPaz

        End Function

        Private Function GetListDatiPazienteAPP(cmd As OracleCommand)

            Dim list As New List(Of Entities.DatiPazienteAPP)()

            Using dr As IDataReader = cmd.ExecuteReader()

                If Not dr Is Nothing Then

                    Dim p As Entities.DatiPazienteAPP = Nothing

                    Dim paz_codice As Integer = dr.GetOrdinal("paz_codice")
                    Dim paz_cognome As Integer = dr.GetOrdinal("paz_cognome")
                    Dim paz_nome As Integer = dr.GetOrdinal("paz_nome")
                    Dim paz_data_nascita As Integer = dr.GetOrdinal("paz_data_nascita")
                    Dim paz_sesso As Integer = dr.GetOrdinal("paz_sesso")
                    Dim paz_codice_fiscale As Integer = dr.GetOrdinal("paz_codice_fiscale")

                    While dr.Read()

                        p = New Entities.DatiPazienteAPP()
                        p.CodiceLocalePaziente = dr.GetInt64(paz_codice)
                        p.Cognome = dr.GetStringOrDefault(paz_cognome)
                        p.Nome = dr.GetStringOrDefault(paz_nome)
                        p.DataNascita = dr.GetNullableDateTimeOrDefault(paz_data_nascita)
                        p.Sesso = dr.GetStringOrDefault(paz_sesso)
                        p.CodiceFiscale = dr.GetStringOrDefault(paz_codice_fiscale)

                        list.Add(p)

                    End While

                End If

            End Using

            Return list

        End Function

        Private Function GetQuerySelectUltimoConsenso(tabella As String) As String

            Return String.Format(
                " SELECT con_progressivo, con_paz_codice, con_tipo_consenso, con_id_consenso, con_consenso, con_tipo_evento, con_data_evento, con_data_scadenza, " +
                " con_id_livello, con_livello, con_lco_descrizione, con_azienda, con_id_ente, con_ente, con_addetto, con_data_registrazione, con_applicativo, " +
                " con_lco_flag_controllo, con_lco_san_codice_mnemonico, con_lco_scn_id_stato_consenso, con_scn_descrizione, con_scn_url_icona, " +
                " con_scn_ordinamento, con_obsoleto, con_centralizzato, con_ordinamento, con_calcolo_stato_globale, con_blocco_accessi_esterni, " +
                " con_custom1, con_custom2, con_custom3, con_azi_codice " +
                " FROM {0} ", tabella)

        End Function

        Private Function GetListConsensiRegistrati(cmd As OracleCommand) As List(Of Entities.Consenso.ConsensoRegistrato)

            Dim list As New List(Of Entities.Consenso.ConsensoRegistrato)()

            Using dr As IDataReader = cmd.ExecuteReader()

                If Not dr Is Nothing Then

                    Dim CON_PROGRESSIVO As Integer = dr.GetOrdinal("CON_PROGRESSIVO")
                    Dim CON_PAZ_CODICE As Integer = dr.GetOrdinal("CON_PAZ_CODICE")
                    Dim CON_TIPO_CONSENSO As Integer = dr.GetOrdinal("CON_TIPO_CONSENSO")
                    Dim CON_ID_CONSENSO As Integer = dr.GetOrdinal("CON_ID_CONSENSO")
                    Dim CON_CONSENSO As Integer = dr.GetOrdinal("CON_CONSENSO")
                    Dim CON_TIPO_EVENTO As Integer = dr.GetOrdinal("CON_TIPO_EVENTO")
                    Dim CON_DATA_EVENTO As Integer = dr.GetOrdinal("CON_DATA_EVENTO")
                    Dim CON_DATA_SCADENZA As Integer = dr.GetOrdinal("CON_DATA_SCADENZA")
                    Dim CON_ID_LIVELLO As Integer = dr.GetOrdinal("CON_ID_LIVELLO")
                    Dim CON_LIVELLO As Integer = dr.GetOrdinal("CON_LIVELLO")
                    Dim CON_LCO_DESCRIZIONE As Integer = dr.GetOrdinal("CON_LCO_DESCRIZIONE")
                    Dim CON_AZIENDA As Integer = dr.GetOrdinal("CON_AZIENDA")
                    Dim CON_ID_ENTE As Integer = dr.GetOrdinal("CON_ID_ENTE")
                    Dim CON_ENTE As Integer = dr.GetOrdinal("CON_ENTE")
                    Dim CON_ADDETTO As Integer = dr.GetOrdinal("CON_ADDETTO")
                    Dim CON_DATA_REGISTRAZIONE As Integer = dr.GetOrdinal("CON_DATA_REGISTRAZIONE")
                    Dim CON_APPLICATIVO As Integer = dr.GetOrdinal("CON_APPLICATIVO")
                    Dim CON_LCO_FLAG_CONTROLLO As Integer = dr.GetOrdinal("CON_LCO_FLAG_CONTROLLO")
                    Dim CON_LCO_SAN_CODICE_MNEMONICO As Integer = dr.GetOrdinal("CON_LCO_SAN_CODICE_MNEMONICO")
                    Dim CON_LCO_SCN_ID_STATO_CONSENSO As Integer = dr.GetOrdinal("CON_LCO_SCN_ID_STATO_CONSENSO")
                    Dim CON_SCN_DESCRIZIONE As Integer = dr.GetOrdinal("CON_SCN_DESCRIZIONE")
                    Dim CON_SCN_URL_ICONA As Integer = dr.GetOrdinal("CON_SCN_URL_ICONA")
                    Dim CON_SCN_ORDINAMENTO As Integer = dr.GetOrdinal("CON_SCN_ORDINAMENTO")
                    Dim CON_OBSOLETO As Integer = dr.GetOrdinal("CON_OBSOLETO")
                    Dim CON_CENTRALIZZATO As Integer = dr.GetOrdinal("CON_CENTRALIZZATO")
                    Dim CON_ORDINAMENTO As Integer = dr.GetOrdinal("CON_ORDINAMENTO")
                    Dim CON_CALCOLO_STATO_GLOBALE As Integer = dr.GetOrdinal("CON_CALCOLO_STATO_GLOBALE")
                    Dim CON_BLOCCO_ACCESSI_ESTERNI As Integer = dr.GetOrdinal("CON_BLOCCO_ACCESSI_ESTERNI")
                    Dim CON_AZI_CODICE As Integer = dr.GetOrdinal("CON_AZI_CODICE")
                    Dim CON_CUSTOM1 As Integer = dr.GetOrdinal("CON_CUSTOM1")
                    Dim CON_CUSTOM2 As Integer = dr.GetOrdinal("CON_CUSTOM2")
                    Dim CON_CUSTOM3 As Integer = dr.GetOrdinal("CON_CUSTOM3")

                    While dr.Read()

                        Dim consenso As New Entities.Consenso.ConsensoRegistrato()
                        consenso.Progressivo = dr.GetInt64OrDefault(CON_PROGRESSIVO)
                        consenso.CodicePaziente = dr.GetStringOrDefault(CON_PAZ_CODICE)
                        consenso.TipoConsenso = dr.GetStringOrDefault(CON_TIPO_CONSENSO)
                        consenso.ConsensoId = dr.GetInt32OrDefault(CON_ID_CONSENSO)
                        consenso.Consenso = dr.GetStringOrDefault(CON_CONSENSO)
                        consenso.TipoEvento = dr.GetStringOrDefault(CON_TIPO_EVENTO)
                        consenso.DataEvento = dr.GetDateTimeOrDefault(CON_DATA_EVENTO)
                        consenso.DataScadenza = dr.GetDateTimeOrDefault(CON_DATA_SCADENZA)
                        consenso.LivelloId = dr.GetInt32OrDefault(CON_ID_LIVELLO)
                        consenso.Livello = dr.GetStringOrDefault(CON_LIVELLO)
                        consenso.Descrizione = dr.GetStringOrDefault(CON_LCO_DESCRIZIONE)
                        consenso.Azienda = dr.GetStringOrDefault(CON_AZIENDA)
                        consenso.EnteId = dr.GetInt32OrDefault(CON_ID_ENTE)
                        consenso.Ente = dr.GetStringOrDefault(CON_ENTE)
                        consenso.Addetto = dr.GetStringOrDefault(CON_ADDETTO)
                        consenso.DataRegistrazione = dr.GetDateTimeOrDefault(CON_DATA_REGISTRAZIONE)
                        consenso.Applicativo = dr.GetStringOrDefault(CON_APPLICATIVO)
                        consenso.FlagControllo = dr.GetStringOrDefault(CON_LCO_FLAG_CONTROLLO)
                        consenso.StatoAnagraficoCodiceMnemonico = dr.GetStringOrDefault(CON_LCO_SAN_CODICE_MNEMONICO)
                        consenso.StatoIDIcona = dr.GetStringOrDefault(CON_LCO_SCN_ID_STATO_CONSENSO)
                        consenso.StatoDescrizione = dr.GetStringOrDefault(CON_SCN_DESCRIZIONE)
                        consenso.StatoUrlIcona = dr.GetStringOrDefault(CON_SCN_URL_ICONA)
                        consenso.StatoOrdinamento = dr.GetInt32OrDefault(CON_SCN_ORDINAMENTO)
                        consenso.Obsoleto = dr.GetBooleanOrDefault(CON_OBSOLETO)
                        consenso.Centralizzato = dr.GetBooleanOrDefault(CON_CENTRALIZZATO)
                        consenso.ConsensoOrdinamento = dr.GetInt32OrDefault(CON_ORDINAMENTO)
                        consenso.UsaPerCalcoloStatoGlobale = dr.GetBooleanOrDefault(CON_CALCOLO_STATO_GLOBALE)
                        consenso.BloccoAccessiEsterni = dr.GetBooleanOrDefault(CON_BLOCCO_ACCESSI_ESTERNI)
                        consenso.CodiceAziendaRegistrazione = dr.GetStringOrDefault(CON_AZI_CODICE)
                        consenso.CampoCustom1 = dr.GetStringOrDefault(CON_CUSTOM1)
                        consenso.CampoCustom2 = dr.GetStringOrDefault(CON_CUSTOM2)
                        consenso.CampoCustom3 = dr.GetStringOrDefault(CON_CUSTOM3)

                        list.Add(consenso)

                    End While
                End If

            End Using

            Return list

        End Function

        ''' <summary>
        ''' Restituisce i pazienti aventi il codice fiscale indicato, nelle varie ULSS (non unificate + unificata)
        ''' </summary>
        ''' <param name="codiceFiscale"></param>
        ''' <param name="statiAnagraficiAttivi"></param>
        ''' <returns></returns>
        Public Function GetPazientiUlssByCodiceFiscale(codiceFiscale As String) As IEnumerable(Of PazienteUlss) Implements IPazienteProvider.GetPazientiUlssByCodiceFiscale

            Dim list As New List(Of PazienteUlss)()

            Using cmd As OracleCommand = Connection.CreateCommand()

                Dim query As New Text.StringBuilder()
                query.Append(" select paz_codice, paz_codice_ausiliario, paz_stato_anagrafico, usl_codice ")
                query.Append(" from v_paz_ulss_app ")
                query.Append(" where paz_codice_fiscale = :paz_codice_fiscale ")
                query.Append(" order by usl_codice ")

                cmd.CommandText = query.ToString()
                cmd.Parameters.AddWithValue("paz_codice_fiscale", codiceFiscale)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim paz_codice As Integer = idr.GetOrdinal("paz_codice")
                            Dim paz_codice_ausiliario As Integer = idr.GetOrdinal("paz_codice_ausiliario")
                            Dim paz_stato_anagrafico As Integer = idr.GetOrdinal("paz_stato_anagrafico")
                            Dim usl_codice As Integer = idr.GetOrdinal("usl_codice")

                            While idr.Read()

                                Dim p As New PazienteUlss()

                                p.CodiceLocalePaziente = idr.GetInt64OrDefault(paz_codice)
                                p.CodiceCentralePaziente = idr.GetStringOrDefault(paz_codice_ausiliario)
                                p.StatoAnagrafico = idr.GetNullableEnumOrDefault(Of Enumerators.StatoAnagrafico)(paz_stato_anagrafico)
                                p.CodiceUsl = idr.GetStringOrDefault(usl_codice)

                                list.Add(p)

                            End While

                        End If

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list.AsEnumerable()

        End Function

        Public Function GetPazientiUlssByCellulare(cellulare As String, idConsenso As Integer, codiceAziendaRegistrazione As String) As Entities.Consenso.ConsensoRegistrato Implements IPazienteProvider.GetPazientiUlssByCellulare

            Dim ultimoConsenso As Entities.Consenso.ConsensoRegistrato = Nothing

            Using cmd As OracleCommand = New OracleCommand()

                cmd.Connection = Me.Connection

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    cmd.CommandText = Me.GetQuerySelectUltimoConsenso("v_paz_ultimo_consenso") +
                        " WHERE CON_CUSTOM1 = :cellulare " +
                        " AND con_id_consenso = :idConsenso " +
                        " AND (con_centralizzato = 'S' or (con_centralizzato = 'N' and con_azi_codice = :codiceAzienda) or con_progressivo is null) "

                    cmd.Parameters.AddWithValue("cellulare", cellulare)
                    cmd.Parameters.AddWithValue("idConsenso", idConsenso)
                    cmd.Parameters.AddWithValue("codiceAzienda", codiceAziendaRegistrazione)

                    Dim listConsensi As List(Of Entities.Consenso.ConsensoRegistrato) = Me.GetListConsensiRegistrati(cmd)

                    If Not listConsensi Is Nothing AndAlso listConsensi.Count > 0 Then
                        ultimoConsenso = listConsensi.First()
                    End If

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return ultimoConsenso

        End Function

        Public Function GetInfoAssistito(CF As String) As InfoAssistito Implements IPazienteProvider.GetInfoAssistito

            Dim result As InfoAssistito = New InfoAssistito()

            Dim ownConnection As Boolean = False

            Dim query As String = "SELECT PAZ_CODICE, PAZ_DATA_INSERIMENTO, PAZ_STATO, PAZ_TELEFONO_1, PAZ_INDIRIZZO_DOMICILIO, PAZ_COM_CODICE_DOMICILIO, PAZ_INDIRIZZO_RESIDENZA, PAZ_COM_CODICE_RESIDENZA, PAZ_CIT_CODICE, PAZ_DATA_NASCITA, PAZ_TESSERA, PAZ_CODICE_FISCALE, PAZ_SESSO, PAZ_COGNOME, PAZ_NOME, " +
                                  " NAS.COM_DESCRIZIONE NAS_DESCRIZIONE, " +
                                  "ACD.CIT_STATO ACD_STATO, " +
                                  "RES.COM_DESCRIZIONE RES_COM_DESCRIZIONE,RES.COM_PROVINCIA RES_COM_PROVINCIA, RES.COM_CAP RES_COM_CAP, RES.COM_DATA_INIZIO_VALIDITA RES_COM_DATA_INIZIO_VALIDITA,RES.COM_DATA_FINE_VALIDITA RES_COM_DATA_FINE_VALIDITA, " +
                                  "DOM.COM_DESCRIZIONE DOM_COM_DESCRIZIONE,DOM.COM_PROVINCIA DOM_COM_PROVINCIA, DOM.COM_CAP DOM_COM_CAP, DOM.COM_DATA_INIZIO_VALIDITA DOM_COM_DATA_INIZIO_VALIDITA,DOM.COM_DATA_FINE_VALIDITA DOM_COM_DATA_FINE_VALIDITA, " +
                                  "USL.USL_CODICE USL_CODICE, USL.USL_DESCRIZIONE USL_DESCRIZIONE " +
                                  "FROM T_PAZ_PAZIENTI  " +
                                  "LEFT JOIN T_ANA_COMUNI NAS ON PAZ_COM_CODICE_NASCITA = NAS.COM_CODICE " +
                                  "LEFT JOIN T_ANA_CITTADINANZE ACD ON PAZ_CIT_CODICE = ACD.CIT_CODICE " +
                                  "LEFT JOIN T_ANA_COMUNI RES ON PAZ_COM_CODICE_RESIDENZA = RES.COM_CODICE " +
                                  "LEFT JOIN T_ANA_COMUNI DOM ON PAZ_COM_CODICE_DOMICILIO = DOM.COM_CODICE " +
                                  "LEFT JOIN T_ANA_USL USL ON PAZ_USL_CODICE_RESIDENZA = USL.USL_CODICE " +
                                  "WHERE PAZ_CODICE_FISCALE=:PAZ_CODICE_FISCALE"

            Try
                Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                    cmd.Parameters.AddWithValue("PAZ_CODICE_FISCALE", CF)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()

                        Dim paz_codice As Integer = _context.GetOrdinal("PAZ_CODICE")
                        Dim paz_nome As Integer = _context.GetOrdinal("PAZ_NOME")
                        Dim paz_cognome As Integer = _context.GetOrdinal("PAZ_COGNOME")
                        Dim paz_sesso As Integer = _context.GetOrdinal("PAZ_SESSO")
                        Dim paz_codiceF As Integer = _context.GetOrdinal("PAZ_CODICE_FISCALE")
                        Dim paz_tessera As Integer = _context.GetOrdinal("PAZ_TESSERA")
                        Dim paz_dataDiNascita As Integer = _context.GetOrdinal("PAZ_DATA_NASCITA")
                        Dim paz_comuneDiNascita As Integer = _context.GetOrdinal("NAS_DESCRIZIONE")
                        Dim paz_cittadinanzaCodice As Integer = _context.GetOrdinal("PAZ_CIT_CODICE")
                        Dim paz_cittadinanzaDescrizione As Integer = _context.GetOrdinal("ACD_STATO")
                        Dim paz_resCod As Integer = _context.GetOrdinal("PAZ_COM_CODICE_RESIDENZA")
                        Dim paz_resDes As Integer = _context.GetOrdinal("RES_COM_DESCRIZIONE")
                        Dim paz_resProv As Integer = _context.GetOrdinal("RES_COM_PROVINCIA")
                        Dim paz_resCap As Integer = _context.GetOrdinal("RES_COM_CAP")
                        Dim paz_resDI As Integer = _context.GetOrdinal("RES_COM_DATA_INIZIO_VALIDITA")
                        Dim paz_resDF As Integer = _context.GetOrdinal("RES_COM_DATA_FINE_VALIDITA")
                        Dim paz_indirizzoResidenza As Integer = _context.GetOrdinal("PAZ_INDIRIZZO_RESIDENZA")
                        Dim paz_domCod As Integer = _context.GetOrdinal("PAZ_COM_CODICE_DOMICILIO")
                        Dim paz_domDes As Integer = _context.GetOrdinal("DOM_COM_DESCRIZIONE")
                        Dim paz_domPro As Integer = _context.GetOrdinal("DOM_COM_PROVINCIA")
                        Dim paz_domCap As Integer = _context.GetOrdinal("DOM_COM_CAP")
                        Dim paz_domDI As Integer = _context.GetOrdinal("DOM_COM_DATA_INIZIO_VALIDITA")
                        Dim paz_domDF As Integer = _context.GetOrdinal("DOM_COM_DATA_FINE_VALIDITA")
                        Dim paz_indirizzoDomicilio As Integer = _context.GetOrdinal("PAZ_INDIRIZZO_DOMICILIO")
                        Dim paz_telefono As Integer = _context.GetOrdinal("PAZ_TELEFONO_1")
                        Dim paz_uslResidenzaCodice As Integer = _context.GetOrdinal("USL_CODICE")
                        Dim paz_uslResidenzaDescrizione As Integer = _context.GetOrdinal("USL_DESCRIZIONE")
                        Dim paz_stato As Integer = _context.GetOrdinal("PAZ_STATO")
                        Dim paz_dataInizio As Integer = _context.GetOrdinal("PAZ_DATA_INSERIMENTO")

                        If _context.Read() Then

                            result.Codice = _context.GetInt64OrDefault(paz_codice)
                            result.Nome = _context.GetStringOrDefault(paz_nome)
                            result.Cognome = _context.GetStringOrDefault(paz_cognome)
                            result.Sesso = _context.GetStringOrDefault(paz_sesso)
                            result.Codice_Fiscale = _context.GetStringOrDefault(paz_codiceF)
                            result.Tessera = _context.GetStringOrDefault(paz_tessera)
                            result.DataDiNascita = _context.GetDateTimeOrDefault(paz_dataDiNascita)
                            result.ComuneDiNascita = _context.GetStringOrDefault(paz_comuneDiNascita)
                            result.CittadinanzaCodice = _context.GetStringOrDefault(paz_cittadinanzaCodice)
                            result.CittadinanzaDescrizione = _context.GetStringOrDefault(paz_cittadinanzaDescrizione)
                            result.ComuneResidenza.Codice = _context.GetStringOrDefault(paz_resCod)
                            result.ComuneResidenza.Descrizione = _context.GetStringOrDefault(paz_resDes)
                            result.ComuneResidenza.Provincia = _context.GetStringOrDefault(paz_resProv)
                            result.ComuneResidenza.Cap = _context.GetStringOrDefault(paz_resCap)
                            result.ComuneResidenza.DataInizio = _context.GetDateTimeOrDefault(paz_resDI)
                            result.ComuneResidenza.DataFine = _context.GetDateTimeOrDefault(paz_resDF)
                            result.IndirizzoResidenza = _context.GetStringOrDefault(paz_indirizzoResidenza)
                            result.ComuneDomicilio.Codice = _context.GetStringOrDefault(paz_domCod)
                            result.ComuneDomicilio.Descrizione = _context.GetStringOrDefault(paz_domDes)
                            result.ComuneDomicilio.Provincia = _context.GetStringOrDefault(paz_domPro)
                            result.ComuneDomicilio.Cap = _context.GetStringOrDefault(paz_domCap)
                            result.ComuneDomicilio.DataInizio = _context.GetDateTimeOrDefault(paz_domDI)
                            result.ComuneDomicilio.DataFine = _context.GetDateTimeOrDefault(paz_domDF)
                            result.IndirizzoDomicilio = _context.GetStringOrDefault(paz_indirizzoDomicilio)
                            result.Telefono = _context.GetStringOrDefault(paz_telefono)
                            result.UslResidenzaCodice = _context.GetStringOrDefault(paz_uslResidenzaCodice)
                            result.UslResidenzaDescrizione = _context.GetStringOrDefault(paz_uslResidenzaDescrizione)
                            result.Stato = _context.GetStringOrDefault(paz_stato)
                            result.DataInserimento = _context.GetDateTimeOrDefault(paz_dataInizio)

                        End If

                    End Using
                End Using



            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return result

        End Function

        Public Function GetContattiAssistito(CF As String) As ContattiAssistito Implements IPazienteProvider.GetContattiAssistito

            Dim result As ContattiAssistito = New ContattiAssistito()

            Dim ownConnection As Boolean = False

            Dim query As String = "SELECT PAZ_TELEFONO_1, PAZ_TELEFONO_2, PAZ_TELEFONO_3, PAZ_EMAIL " +
                                   "FROM T_PAZ_PAZIENTI " +
                                    "WHERE PAZ_CODICE_FISCALE=:PAZ_CODICE_FISCALE"
            Try
                Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                    cmd.Parameters.AddWithValue("PAZ_CODICE_FISCALE", CF)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()

                        Dim paz_telefono1 As Integer = _context.GetOrdinal("PAZ_TELEFONO_1")
                        Dim paz_telefono2 As Integer = _context.GetOrdinal("PAZ_TELEFONO_2")
                        Dim paz_telefono3 As Integer = _context.GetOrdinal("PAZ_TELEFONO_3")
                        Dim paz_email As Integer = _context.GetOrdinal("PAZ_EMAIL")


                        If _context.Read() Then
                            result.Telefono = _context.GetStringOrDefault(paz_telefono1)
                            result.Telefono2 = _context.GetStringOrDefault(paz_telefono2)
                            result.Telefono3 = _context.GetStringOrDefault(paz_telefono3)
                            result.EMail = _context.GetStringOrDefault(paz_email)

                        End If

                    End Using
                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return result
        End Function
        Public Function GetDocumentiAssistitoByCodice(codice As String) As List(Of DtoDocumento) Implements IPazienteProvider.GetDocumentiAssistitoByCodice
            Dim list As List(Of DtoDocumento) = New List(Of DtoDocumento)()
            Dim ownConnection As Boolean = False

            Dim query As String = "SELECT PDO_ID, PDO_DATA_ARCHIVIAZIONE, PDO_NOTE, PDO_DESCRIZIONE, PDO_NOME, PDO_DOCUMENTO, " +
                                   "ST.ST_DESCRIZIONE ST_DESCRIZIONE, ST.ST_ID ST_ID " +
                                   "FROM T_PAZ_DOCUMENTI " +
                                   "LEFT JOIN T_STATI ST ON PDO_ST_ID = ST.ST_ID " +
                                   "WHERE PDO_PAZ_CODICE=:PDO_PAZ_CODICE"
            Try
                Using cmd As OracleCommand = New OracleCommand(query, Me.Connection)

                    cmd.Parameters.AddWithValue("PDO_PAZ_CODICE", codice)

                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using _context As IDataReader = cmd.ExecuteReader()

                        Dim doc_paz_id As Integer = _context.GetOrdinal("PDO_ID")
                        Dim doc_paz_dataA As Integer = _context.GetOrdinal("PDO_DATA_ARCHIVIAZIONE")
                        Dim doc_paz_note As Integer = _context.GetOrdinal("PDO_NOTE")
                        Dim doc_paz_des As Integer = _context.GetOrdinal("PDO_DESCRIZIONE")
                        Dim doc_paz_nome As Integer = _context.GetOrdinal("PDO_NOME")
                        Dim doc_paz_st_des As Integer = _context.GetOrdinal("ST_DESCRIZIONE")
                        Dim doc_paz_st_id As Integer = _context.GetOrdinal("ST_ID")
                        Dim doc_paz_64 As Integer = _context.GetOrdinal("PDO_DOCUMENTO")

                        While _context.Read()
                            Dim documento As New DtoDocumento()

                            documento.ID = _context.GetInt64OrDefault(doc_paz_id)
                            documento.DataArchiviazione = _context.GetDateTimeOrDefault(doc_paz_dataA)
                            documento.Descrizione = _context.GetStringOrDefault(doc_paz_note)
                            documento.Tipologia = _context.GetStringOrDefault(doc_paz_des)
                            documento.NomeDocumento = _context.GetStringOrDefault(doc_paz_nome)
                            documento.StatoDocumentoDescrizione = _context.GetStringOrDefault(doc_paz_st_des)
                            documento.StatoDocumentoId = _context.GetInt64OrDefault(doc_paz_st_id)
                            documento.Documento64 = _context.GetStringOrDefault(doc_paz_64)

                            list.Add(documento)
                        End While

                    End Using
                End Using

            Finally
                ConditionalCloseConnection(ownConnection)
            End Try

            Return list.AsEnumerable()
        End Function

        Public Function SetContattoAssistito(Contatto As DTOSetContatto) As SetContattoResult

            Dim result As SetContattoResult = New SetContattoResult()
            result.Success = True
            result.Message = "Update eseguito con successo"
            Dim ownConnection As Boolean = False
            Dim query As String = "UPDATE T_PAZ_PAZIENTI " +
                                  "SET PAZ_TELEFONO_1 =:PAZ_TELEFONO_1 ,PAZ_TELEFONO_2 =:PAZ_TELEFONO_2, PAZ_TELEFONO_3 =:PAZ_TELEFONO_3, PAZ_EMAIL =:PAZ_EMAIL " +
                                    "WHERE PAZ_CODICE_FISCALE =:PAZ_CODICE_FISCALE"

            Try
                Using cmd As OracleCommand = New OracleCommand(query, Connection)

                    cmd.Parameters.AddWithValueOrDefault("PAZ_TELEFONO_1", Contatto.Telefono)
                    cmd.Parameters.AddWithValueOrDefault("PAZ_TELEFONO_2", Contatto.Telefono2)
                    cmd.Parameters.AddWithValueOrDefault("PAZ_TELEFONO_3", Contatto.Telefono3)
                    cmd.Parameters.AddWithValueOrDefault("PAZ_EMAIL", Contatto.EMail)

                    cmd.Parameters.AddWithValue("PAZ_CODICE_FISCALE", Contatto.CodiceFiscale)

                    ownConnection = ConditionalOpenConnection(cmd)
                    cmd.ExecuteNonQuery()


                End Using
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return result

        End Function

        Public Function SetIndirizzoTemporaneo(indirizzoTemporaneo As DTOSetIndirizzoTemporaneo) As ResultSetPost
            Dim result As ResultSetPost = New ResultSetPost()
            result.Success = True
            result.Message = "Update eseguito con successo"
            Dim ownConnection As Boolean = False
            Dim query As String = "UPDATE T_PAZ_PAZIENTI " +
                                 "SET PAZ_INDIRIZZO_TEMPORANEO =:PAZ_INDIRIZZO_TEMPORANEO ,PAZ_INIZIO_IT =:PAZ_INIZIO_IT, PAZ_FINE_IT =:PAZ_FINE_IT " +
                                   "WHERE PAZ_CODICE_FISCALE =:PAZ_CODICE_FISCALE"

            Try
                Using cmd As OracleCommand = New OracleCommand(query, Connection)

                    cmd.Parameters.AddWithValueOrDefault("PAZ_INDIRIZZO_TEMPORANEO", indirizzoTemporaneo.IndirizzoTemporaneo)
                    cmd.Parameters.AddWithValueOrDefault("PAZ_INIZIO_IT", indirizzoTemporaneo.InizioIT)
                    cmd.Parameters.AddWithValueOrDefault("PAZ_FINE_IT", indirizzoTemporaneo.FineIT)
                    cmd.Parameters.AddWithValue("PAZ_CODICE_FISCALE", indirizzoTemporaneo.CodiceFiscale)

                    ownConnection = ConditionalOpenConnection(cmd)
                    cmd.ExecuteNonQuery()

                End Using
            Finally
                ConditionalCloseConnection(ownConnection)
            End Try
            Return result
        End Function

#Region "FSE"

        Public Function GetCodicePazienteByDocumentUniqueId(documentUniqueId As String, tipoDocumento As String) As Integer? Implements IPazienteProvider.GetCodicePazienteByDocumentUniqueId

            Dim codicePaziente As Integer?

            Using cmd As OracleCommand = Connection.CreateCommand()
                documentUniqueId = documentUniqueId.Replace("^", ".")
                Dim query As New Text.StringBuilder()
                query.Append(" select pui_paz_codice ")
                query.Append(" from t_paz_fse_unique_id ")
                query.Append(" where replace(pui_document_unique_id,'^','.') = :pui_document_unique_id ")
                query.Append(" and pui_tipo_documento = :pui_tipo_documento ")
                query.Append(" order by pui_id desc ")

                cmd.CommandText = query.ToString()
                cmd.Parameters.AddWithValue("pui_document_unique_id", documentUniqueId)
                cmd.Parameters.AddWithValue("pui_tipo_documento", tipoDocumento)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim pui_paz_codice As Integer = idr.GetOrdinal("pui_paz_codice")

                            If idr.Read() Then

                                codicePaziente = idr.GetInt32OrDefault(pui_paz_codice)

                            End If

                        End If
                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return codicePaziente

        End Function

        Public Function GetDocumentUniqueIdByCodicePaziente(codicePaziente As Long, tipoDocumento As String) As String Implements IPazienteProvider.GetDocumentUniqueIdByCodicePaziente

            Dim documentUniqueId As String = String.Empty

            Using cmd As OracleCommand = Connection.CreateCommand()

                Dim query As New Text.StringBuilder()
                query.Append(" select pui_document_unique_id ")
                query.Append(" from t_paz_fse_unique_id ")
                query.Append(" where pui_paz_codice = :pui_paz_codice ")
                query.Append(" and pui_tipo_documento = :pui_tipo_documento ")
                query.Append(" order by pui_id desc ")

                cmd.CommandText = query.ToString()
                cmd.Parameters.AddWithValue("pui_paz_codice", codicePaziente)
                cmd.Parameters.AddWithValue("pui_tipo_documento", tipoDocumento)

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim pui_document_unique_id As Integer = idr.GetOrdinal("pui_document_unique_id")

                            If idr.Read() Then

                                documentUniqueId = idr.GetStringOrDefault(pui_document_unique_id)

                            End If

                        End If
                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return documentUniqueId

        End Function

        Public Function GetDatiPazienteFSE(codicePaziente As Integer) As PazienteFSE Implements IPazienteProvider.GetDatiPazienteFSE

            Dim paziente As PazienteFSE = Nothing

            Using cmd As OracleCommand = Connection.CreateCommand()

                Dim query As New Text.StringBuilder()
                query.Append(" SELECT paz_codice, paz_cognome, paz_nome, paz_data_nascita, paz_codice_fiscale, paz_sesso, paz_cns_codice, cns_azi_codice paz_azi_codice, usl_descrizione paz_azi_descrizione, paz_tessera, ")
                query.Append(" paz_indirizzo_residenza, res.com_codice res_codice, res.com_descrizione res_descrizione, res.com_provincia res_prv, paz_cap_residenza res_cap, res.com_stato_avn res_stato, ")
                query.Append(" (CASE WHEN res.com_istat LIKE '999%' THEN SUBSTR(res.com_istat,4) ELSE res.com_istat END) res_codice_istat, ")
                query.Append(" nas.com_codice nas_codice, nas.com_descrizione nas_descrizione, nas.com_provincia nas_prv, nas.com_stato_avn nas_stato, ")
                query.Append(" (CASE WHEN nas.com_istat LIKE '999%' THEN SUBSTR(nas.com_istat,4) ELSE nas.com_istat END) nas_codice_istat, ")
                query.Append(" paz_indirizzo_domicilio, dom.com_codice dom_codice, dom.com_descrizione dom_descrizione, dom.com_provincia dom_prv, paz_cap_domicilio dom_cap, dom.com_stato_avn dom_stato, ")
                query.Append(" (CASE WHEN dom.com_istat LIKE '999%' THEN SUBSTR(dom.com_istat,4) ELSE dom.com_istat END) dom_codice_istat, ")
                query.Append(" paz_cit_codice, cit_stato, cit_sigla, paz_codice_ausiliario, paz_codice_regionale, paz_padre, paz_madre, paz_id_acn, ")
                query.Append(" paz_stato_anagrafico, paz_categoria_cittadino, paz_telefono_1, paz_telefono_2, paz_telefono_3, paz_usl_codice_assistenza, paz_usl_codice_residenza, paz_reg_assistenza, ")
                query.Append(" GET_TIPO_ID(paz_codice_fiscale, paz_tessera) tipo_paziente ")
                query.Append(" FROM t_paz_pazienti ")
                query.Append(" left join t_ana_comuni res on res.com_codice = paz_com_codice_residenza ")
                query.Append(" left join t_ana_comuni dom on dom.com_codice = paz_com_codice_domicilio ")
                query.Append(" left join t_ana_comuni nas on nas.com_codice = paz_com_codice_nascita ")
                query.Append(" left join t_ana_cittadinanze on cit_codice = paz_cit_codice ")
                query.Append(" left join t_ana_consultori on cns_codice = paz_cns_codice ")
                query.Append(" left join t_ana_usl on usl_codice = cns_azi_codice ")
                query.Append(" WHERE paz_codice = :paz_codice ")

                cmd.CommandText = query.ToString()
                cmd.Parameters.AddWithValue("paz_codice", codicePaziente.ToString())

                Dim ownConnection As Boolean = False

                Try
                    ownConnection = Me.ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim paz_codice As Integer = idr.GetOrdinal("paz_codice")
                            Dim paz_cognome As Integer = idr.GetOrdinal("paz_cognome")
                            Dim paz_nome As Integer = idr.GetOrdinal("paz_nome")
                            Dim paz_data_nascita As Integer = idr.GetOrdinal("paz_data_nascita")
                            Dim paz_codice_fiscale As Integer = idr.GetOrdinal("paz_codice_fiscale")
                            Dim paz_sesso As Integer = idr.GetOrdinal("paz_sesso")
                            Dim paz_cns_codice As Integer = idr.GetOrdinal("paz_cns_codice")
                            Dim paz_azi_codice As Integer = idr.GetOrdinal("paz_azi_codice")
                            Dim paz_azi_descrizione As Integer = idr.GetOrdinal("paz_azi_descrizione")
                            Dim paz_tessera As Integer = idr.GetOrdinal("paz_tessera")

                            Dim paz_indirizzo_residenza As Integer = idr.GetOrdinal("paz_indirizzo_residenza")
                            Dim res_codice As Integer = idr.GetOrdinal("res_codice")
                            Dim res_descrizione As Integer = idr.GetOrdinal("res_descrizione")
                            Dim res_prv As Integer = idr.GetOrdinal("res_prv")
                            Dim res_cap As Integer = idr.GetOrdinal("res_cap")
                            Dim res_codice_istat As Integer = idr.GetOrdinal("res_codice_istat")
                            Dim res_stato As Integer = idr.GetOrdinal("res_stato")

                            Dim nas_codice As Integer = idr.GetOrdinal("nas_codice")
                            Dim nas_descrizione As Integer = idr.GetOrdinal("nas_descrizione")
                            Dim nas_prv As Integer = idr.GetOrdinal("nas_prv")
                            Dim nas_codice_istat As Integer = idr.GetOrdinal("nas_codice_istat")
                            Dim nas_stato As Integer = idr.GetOrdinal("nas_stato")

                            Dim paz_indirizzo_domicilio As Integer = idr.GetOrdinal("paz_indirizzo_domicilio")
                            Dim dom_codice As Integer = idr.GetOrdinal("dom_codice")
                            Dim dom_descrizione As Integer = idr.GetOrdinal("dom_descrizione")
                            Dim dom_prv As Integer = idr.GetOrdinal("dom_prv")
                            Dim dom_cap As Integer = idr.GetOrdinal("dom_cap")
                            Dim dom_codice_istat As Integer = idr.GetOrdinal("dom_codice_istat")
                            Dim dom_stato As Integer = idr.GetOrdinal("dom_stato")

                            Dim paz_cit_codice As Integer = idr.GetOrdinal("paz_cit_codice")
                            Dim cit_stato As Integer = idr.GetOrdinal("cit_stato")
                            Dim cit_sigla As Integer = idr.GetOrdinal("cit_sigla")
                            Dim paz_codice_ausiliario As Integer = idr.GetOrdinal("paz_codice_ausiliario")
                            Dim paz_codice_regionale As Integer = idr.GetOrdinal("paz_codice_regionale")
                            Dim paz_padre As Integer = idr.GetOrdinal("paz_padre")
                            Dim paz_madre As Integer = idr.GetOrdinal("paz_madre")
                            Dim paz_id_acn As Integer = idr.GetOrdinal("paz_id_acn")

                            Dim paz_stato_anagrafico As Integer = idr.GetOrdinal("paz_stato_anagrafico")
                            Dim paz_categoria_cittadino As Integer = idr.GetOrdinal("paz_categoria_cittadino")
                            Dim paz_telefono_1 As Integer = idr.GetOrdinal("paz_telefono_1")
                            Dim paz_telefono_2 As Integer = idr.GetOrdinal("paz_telefono_2")
                            Dim paz_telefono_3 As Integer = idr.GetOrdinal("paz_telefono_3")
                            Dim paz_usl_codice_assistenza As Integer = idr.GetOrdinal("paz_usl_codice_assistenza")
                            Dim paz_usl_codice_residenza As Integer = idr.GetOrdinal("paz_usl_codice_residenza")
                            Dim paz_reg_assistenza As Integer = idr.GetOrdinal("paz_reg_assistenza")
                            Dim tipo_paziente As Integer = idr.GetOrdinal("tipo_paziente")

                            If idr.Read() Then

                                paziente = New PazienteFSE()

                                paziente.CodicePaziente = codicePaziente
                                paziente.CodiceRegionale = idr.GetStringOrDefault(paz_codice_regionale)
                                paziente.CodiceAusiliario = idr.GetStringOrDefault(paz_codice_ausiliario)
                                paziente.Cognome = idr.GetStringOrDefault(paz_cognome)
                                paziente.Nome = idr.GetStringOrDefault(paz_nome)
                                paziente.Sesso = idr.GetStringOrDefault(paz_sesso)
                                paziente.CodiceFiscale = idr.GetStringOrDefault(paz_codice_fiscale)
                                paziente.TesseraSanitaria = idr.GetStringOrDefault(paz_tessera)
                                paziente.CodiceCnsCorrente = idr.GetStringOrDefault(paz_cns_codice)
                                paziente.CodiceUslCorrente = idr.GetStringOrDefault(paz_azi_codice)
                                paziente.DescrizioneUslCorrente = idr.GetStringOrDefault(paz_azi_descrizione)

                                paziente.DataNascita = idr.GetDateTimeOrDefault(paz_data_nascita)
                                paziente.CodiceComuneNascita = idr.GetStringOrDefault(nas_codice)
                                paziente.DescrizioneComuneNascita = idr.GetStringOrDefault(nas_descrizione)
                                paziente.ProvinciaNascita = idr.GetStringOrDefault(nas_prv)
                                paziente.CodiceIstatComuneNascita = idr.GetStringOrDefault(nas_codice_istat)
                                paziente.CodiceStatoNascita = idr.GetStringOrDefault(nas_stato)

                                paziente.IndirizzoResidenza = idr.GetStringOrDefault(paz_indirizzo_residenza)
                                paziente.CodiceComuneResidenza = idr.GetStringOrDefault(res_codice)
                                paziente.DescrizioneComuneResidenza = idr.GetStringOrDefault(res_descrizione)
                                paziente.ProvinciaResidenza = idr.GetStringOrDefault(res_prv)
                                paziente.CapResidenza = idr.GetStringOrDefault(res_cap)
                                paziente.CodiceIstatComuneResidenza = idr.GetStringOrDefault(res_codice_istat)
                                paziente.CodiceStatoResidenza = idr.GetStringOrDefault(res_stato)

                                paziente.IndirizzoDomicilio = idr.GetStringOrDefault(paz_indirizzo_domicilio)
                                paziente.CodiceComuneDomicilio = idr.GetStringOrDefault(dom_codice)
                                paziente.DescrizioneComuneDomicilio = idr.GetStringOrDefault(dom_descrizione)
                                paziente.ProvinciaDomicilio = idr.GetStringOrDefault(dom_prv)
                                paziente.CapDomicilio = idr.GetStringOrDefault(dom_cap)
                                paziente.CodiceIstatComuneDomicilio = idr.GetStringOrDefault(dom_codice_istat)
                                paziente.CodiceStatoDomicilio = idr.GetStringOrDefault(dom_stato)

                                paziente.CodiceCittadinanza = idr.GetStringOrDefault(paz_cit_codice)
                                paziente.StatoCittadinanza = idr.GetStringOrDefault(cit_stato)
                                paziente.CodiceIsoCittadinanza = idr.GetStringOrDefault(cit_sigla)
                                paziente.Padre = idr.GetStringOrDefault(paz_padre)
                                paziente.Madre = idr.GetStringOrDefault(paz_madre)
                                Dim sidAcn As String = idr.GetStringOrDefault(paz_id_acn)
                                If Not String.IsNullOrWhiteSpace(sidAcn) Then
                                    paziente.IdAcn = Convert.ToInt32(sidAcn)
                                End If


                                paziente.StatoAnagrafico = idr.GetNullableEnumOrDefault(Of Enumerators.StatoAnagrafico)(paz_stato_anagrafico)
                                paziente.CategoriaCittadino = idr.GetStringOrDefault(paz_categoria_cittadino)
                                paziente.Telefono1 = idr.GetStringOrDefault(paz_telefono_1)
                                paziente.Telefono2 = idr.GetStringOrDefault(paz_telefono_2)
                                paziente.Telefono3 = idr.GetStringOrDefault(paz_telefono_3)
                                paziente.UslAssistenza_Codice = idr.GetStringOrDefault(paz_usl_codice_assistenza)
                                paziente.UslResidenza_Codice = idr.GetStringOrDefault(paz_usl_codice_residenza)
                                paziente.TipoPaziente = idr.GetNullableEnumOrDefault(Of Enumerators.TipoPaziente)(tipo_paziente)

                            End If

                        End If
                    End Using

                Finally
                    Me.ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return paziente

        End Function

        Public Function InsertDocumentUniqueId(documentUniqueId As String, codicePaziente As Long, tipoDocumento As String, dataInserimento As Date) As Integer Implements IPazienteProvider.InsertDocumentUniqueId

            Dim count As Integer = 0

            Dim query As String =
                "INSERT INTO T_PAZ_FSE_UNIQUE_ID " +
                "(PUI_PAZ_CODICE, PUI_TIPO_DOCUMENTO, PUI_DOCUMENT_UNIQUE_ID, PUI_DATA_INSERIMENTO) " +
                "VALUES " +
                "(:PUI_PAZ_CODICE, :PUI_TIPO_DOCUMENTO, :PUI_DOCUMENT_UNIQUE_ID, :PUI_DATA_INSERIMENTO)"

            Using cmd As New OracleCommand(query, Connection)

                Dim ownConnection As Boolean = False

                cmd.Parameters.AddWithValue("PUI_PAZ_CODICE", codicePaziente)
                cmd.Parameters.AddWithValue("PUI_TIPO_DOCUMENTO", tipoDocumento)
                cmd.Parameters.AddWithValue("PUI_DOCUMENT_UNIQUE_ID", documentUniqueId)
                cmd.Parameters.AddWithValue("PUI_DATA_INSERIMENTO", dataInserimento)

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

#Region " Private "

        Private Function GetListPazienteInfoDistribuzione(cmd As OracleCommand) As List(Of Entities.PazienteInfoDistribuzione)

            Dim listInfoPazienti As New List(Of Entities.PazienteInfoDistribuzione)()

            Using dr As OracleClient.OracleDataReader = cmd.ExecuteReader()

                If Not dr Is Nothing Then

                    Dim infoPaziente As Entities.PazienteInfoDistribuzione = Nothing

                    Dim codice_centrale As Integer = dr.GetOrdinal("codice_centrale")
                    Dim codice_locale As Integer = dr.GetOrdinal("codice_locale")
                    Dim app_id As Integer = dr.GetOrdinal("app_id")

                    While dr.Read()

                        infoPaziente = New Entities.PazienteInfoDistribuzione()
                        infoPaziente.CodiceCentralePaziente = dr.GetString(codice_centrale)
                        infoPaziente.CodiceLocalePaziente = dr.GetInt64(codice_locale)
                        infoPaziente.AppIdUsl = dr.GetString(app_id)

                        listInfoPazienti.Add(infoPaziente)

                    End While
                End If

            End Using

            Return listInfoPazienti

        End Function

        Private Function CreateOracleParameterCodicePaziente(parameterName As String, parameterValue As String, isCentrale As Boolean) As OracleParameter

            Dim parameter As New OracleParameter()

            parameter.ParameterName = parameterName

            If isCentrale Then
                parameter.Value = parameterValue
                parameter.OracleType = OracleType.VarChar
            Else
                parameter.Value = Convert.ToInt32(parameterValue)
                parameter.OracleType = OracleType.Number
            End If

            Return parameter

        End Function

#End Region

#Region " Ricerca Pazienti "


        Friend Function RicercaPazientiLocale(filtro As FiltroRicercaPaziente, ordinamento As String) As List(Of PazienteTrovato) Implements IPazienteProvider.RicercaPazientiLocale

            Dim list As New List(Of PazienteTrovato)()

            Dim Sql As Text.StringBuilder = New Text.StringBuilder("select 
                PAZ_CODICE, PAZ_CODICE_AUSILIARIO, PAZ_CODICE_FISCALE, PAZ_COGNOME,PAZ_CANCELLATO, 
                PAZ_DATA_NASCITA, PAZ_INDIRIZZO_RESIDENZA, PAZ_NOME, 
                PAZ_SESSO, PAZ_STATO_ANAGRAFICO, PAZ_TESSERA, 
                cn.COM_DESCRIZIONE as COMUNE_NASCITA, cr.COM_DESCRIZIONE as COMUNE_RESIDENZA, CNS_CODICE, PAZ_CODICE_REGIONALE, PAZ_TIPO
                from T_PAZ_PAZIENTI
                left join T_ANA_COMUNI cn on (PAZ_COM_CODICE_NASCITA = cn.COM_CODICE)
                left join T_ANA_COMUNI cr on (PAZ_COM_CODICE_RESIDENZA = cr.COM_CODICE)
                left join T_ANA_CONSULTORI on (PAZ_CNS_CODICE = CNS_CODICE)
            ")

            Using cmd As OracleCommand = New OracleCommand(Nothing, Connection)

                Dim ownConnection As Boolean = False
                Dim op As String = "WHERE"

                Try
                    If filtro.CodiceLocale Then
                        Sql.AppendLine(op + " PAZ_CODICE=:CODICE")
                        cmd.Parameters.AddWithValue("CODICE", filtro.CodiceLocale)
                        op = "AND"
                    End If

                    If Not String.IsNullOrWhiteSpace(filtro.CodiceCentrale) Then
                        Sql.AppendLine(op + " PAZ_CODICE_AUSILIARIO=:CODICE_AUSILIARIO")
                        cmd.Parameters.AddWithValue("CODICE_AUSILIARIO", filtro.CodiceCentrale)
                        op = "AND"
                    End If

                    If Not String.IsNullOrWhiteSpace(filtro.CognomeNome) Then
                        Sql.AppendLine(op + " PAZ_COGNOME_NOME like :COGNOMENOME")
                        cmd.Parameters.AddWithValue("COGNOMENOME", filtro.CognomeNome + "%")
                        op = "AND"
                    End If

                    If Not String.IsNullOrWhiteSpace(filtro.Cognome) Then
                        Sql.AppendLine(op + " PAZ_COGNOME like :COGNOME")
                        cmd.Parameters.AddWithValue("COGNOME", filtro.Cognome + "%")
                        op = "AND"
                    End If

                    If Not String.IsNullOrWhiteSpace(filtro.Nome) Then
                        Sql.AppendLine(op + " PAZ_NOME like :NOME")
                        cmd.Parameters.AddWithValue("NOME", filtro.Nome + "%")
                        op = "AND"
                    End If

                    If Not String.IsNullOrWhiteSpace(filtro.CodiceComuneNascita) Then
                        Sql.AppendLine(op + " PAZ_COM_CODICE_NASCITA=:COM_CODICE_NASCITA")
                        cmd.Parameters.AddWithValue("COM_CODICE_NASCITA", filtro.CodiceComuneNascita)
                        op = "AND"
                    End If

                    If Not String.IsNullOrWhiteSpace(filtro.Sesso) Then
                        Sql.AppendLine(op + " PAZ_SESSO=:SESSO")
                        cmd.Parameters.AddWithValue("SESSO", filtro.Sesso)
                        op = "AND"
                    End If

                    If filtro.DataNascita.HasValue Then
                        Sql.AppendLine(op + " PAZ_DATA_NASCITA=:DATA_NASCITA")
                        cmd.Parameters.AddWithValue("DATA_NASCITA", filtro.DataNascita.Value)
                        op = "AND"
                    End If

                    If filtro.AnnoNascita.HasValue Then
                        Sql.AppendLine(op + " PAZ_DATA_NASCITA>=:DATA_NASCITA_DA AND PAZ_DATA_NASCITA < :DATA_NASCITA_A")
                        cmd.Parameters.AddWithValue("DATA_NASCITA_DA", New Date(filtro.AnnoNascita.Value, 1, 1))
                        cmd.Parameters.AddWithValue("DATA_NASCITA_A", New Date(filtro.AnnoNascita.Value + 1, 1, 1))
                        op = "AND"
                    End If

                    If Not String.IsNullOrWhiteSpace(filtro.CodiceFiscale) Then
                        Sql.AppendLine(op + " PAZ_CODICE_FISCALE=:CODICE_FISCALE")
                        cmd.Parameters.AddWithValue("CODICE_FISCALE", filtro.CodiceFiscale)
                        op = "AND"
                    End If

                    If Not String.IsNullOrWhiteSpace(filtro.CodiceTesseraSanitaria) Then
                        Sql.AppendLine(op + " PAZ_TESSERA=:TESSERA")
                        cmd.Parameters.AddWithValue("TESSERA", filtro.CodiceTesseraSanitaria)
                        op = "AND"
                    End If

                    If Not String.IsNullOrWhiteSpace(filtro.CodiceComuneResidenza) Then
                        Sql.AppendLine(op + " PAZ_COM_CODICE_RESIDENZA=:COM_CODICE_RESIDENZA")
                        cmd.Parameters.AddWithValue("COM_CODICE_RESIDENZA", filtro.CodiceComuneResidenza)
                        op = "AND"
                    End If

                    If Not String.IsNullOrWhiteSpace(filtro.CodiceCentroVaccinale) Then
                        Sql.AppendLine(op + " PAZ_CNS_CODICE=:CNS_CODICE")
                        cmd.Parameters.AddWithValue("CNS_CODICE", filtro.CodiceCentroVaccinale)
                        op = "AND"
                    End If

                    If Not String.IsNullOrWhiteSpace(filtro.CodiceRegionale) Then
                        Sql.AppendLine(op + " PAZ_CODICE_REGIONALE=:PAZ_CODICE_REGIONALE")
                        cmd.Parameters.AddWithValue("PAZ_CODICE_REGIONALE", filtro.CodiceRegionale)
                        op = "AND"
                    End If

                    If Not String.IsNullOrWhiteSpace(ordinamento) Then
                        Sql.Append("order by ")
                        Sql.AppendLine(ordinamento)
                    End If

                    cmd.CommandText = Sql.ToString()

                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        Dim paz_codice As Integer = idr.GetOrdinal("PAZ_CODICE")
                        Dim paz_codice_ausiliario As Integer = idr.GetOrdinal("PAZ_CODICE_AUSILIARIO")
                        Dim paz_codice_fiscale As Integer = idr.GetOrdinal("PAZ_CODICE_FISCALE")
                        Dim paz_cognome As Integer = idr.GetOrdinal("PAZ_COGNOME")
                        Dim paz_data_nascita As Integer = idr.GetOrdinal("PAZ_DATA_NASCITA")
                        Dim paz_indirizzo_residenza As Integer = idr.GetOrdinal("PAZ_INDIRIZZO_RESIDENZA")
                        Dim paz_nome As Integer = idr.GetOrdinal("PAZ_NOME")
                        Dim paz_sesso As Integer = idr.GetOrdinal("PAZ_SESSO")
                        Dim paz_stato_anagrafico As Integer = idr.GetOrdinal("PAZ_STATO_ANAGRAFICO")
                        Dim paz_tessera As Integer = idr.GetOrdinal("PAZ_TESSERA")
                        Dim comune_nascita As Integer = idr.GetOrdinal("COMUNE_NASCITA")
                        Dim comune_residenza As Integer = idr.GetOrdinal("COMUNE_RESIDENZA")
                        Dim cns_codice As Integer = idr.GetOrdinal("CNS_CODICE")
                        Dim paz_cancellato As Integer = idr.GetOrdinal("PAZ_CANCELLATO")
                        Dim paz_codice_regionale As Integer = idr.GetOrdinal("PAZ_CODICE_REGIONALE")
                        Dim paz_tipo As Integer = idr.GetOrdinal("PAZ_TIPO")

                        While idr.Read() And (Not filtro.MaxRecords.HasValue OrElse filtro.MaxRecords.Value > list.Count())

                            Dim item As New PazienteTrovato()
                            item.CodiceLocale = idr.GetNullableInt32OrDefault(paz_codice)
                            item.CodiceCentrale = idr.GetStringOrDefault(paz_codice_ausiliario)
                            item.CodiceFiscale = idr.GetStringOrDefault(paz_codice_fiscale)
                            item.Cognome = idr.GetStringOrDefault(paz_cognome)
                            item.DataNascita = idr.GetNullableDateTimeOrDefault(paz_data_nascita)
                            item.IndirizzoResidenza = idr.GetStringOrDefault(paz_indirizzo_residenza)
                            item.Nome = idr.GetStringOrDefault(paz_nome)
                            item.Sesso = idr.GetStringOrDefault(paz_sesso)
                            item.StatoAnagrafico = idr.GetStringOrDefault(paz_stato_anagrafico)
                            item.Tessera = idr.GetStringOrDefault(paz_tessera)
                            item.ComuneNascita = idr.GetStringOrDefault(comune_nascita)
                            item.ComuneResidenza = idr.GetStringOrDefault(comune_residenza)
                            item.CodiceCentroVaccinale = idr.GetStringOrDefault(cns_codice)
                            item.Cancellato = idr.GetBooleanOrDefault(paz_cancellato)
                            item.CodiceRegionale = idr.GetStringOrDefault(paz_codice_regionale)
                            item.PazTipo = idr.GetStringOrDefault(paz_tipo)
                            item.Fonte = Enumerators.FonteAnagrafica.AnagrafeLocale

                            list.Add(item)

                        End While

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list

        End Function

        Friend Function RicercaPazientiCentrale(filtro As FiltroRicercaPaziente, ordinamento As String) As List(Of PazienteTrovato) Implements IPazienteProvider.RicercaPazientiCentrale

            Dim list As New List(Of PazienteTrovato)()

            Dim query As New Text.StringBuilder("select PAZ_CODICE, PAZ_CODICE_LOCALE, PAZ_CODICE_FISCALE, PAZ_COGNOME, PAZ_CANCELLATO, 
                PAZ_DATA_NASCITA, PAZ_INDIRIZZO_RESIDENZA, PAZ_NOME, 
                PAZ_SESSO, PAZ_STATO_ANAGRAFICO, PAZ_TESSERA, CNS_CODICE, PAZ_CODICE_REGIONALE, PAZ_TIPO, 
                PAZ_USL_CODICE_ASSISTENZA, PAZ_USL_CODICE_DOMICILIO,
                cn.COM_DESCRIZIONE as COMUNE_NASCITA, cr.COM_DESCRIZIONE as COMUNE_RESIDENZA
                from T_PAZ_PAZIENTI
                left join T_ANA_COMUNI cn on (PAZ_COM_CODICE_NASCITA = cn.COM_CODICE)
                left join T_ANA_COMUNI cr on (PAZ_COM_CODICE_RESIDENZA = cr.COM_CODICE)
                left join T_ANA_CONSULTORI on (PAZ_CNS_CODICE = CNS_CODICE)")

            Using cmd As OracleCommand = New OracleCommand(Nothing, Connection)

                Dim ownConnection As Boolean = False
                Dim op As String = " WHERE "

                Try
                    If filtro.CodiceLocale.HasValue AndAlso filtro.CodiceLocale.Value > 0 Then
                        query.AppendLine(op + " PAZ_CODICE_LOCALE = :PAZ_CODICE_LOCALE")
                        cmd.Parameters.AddWithValue("PAZ_CODICE_LOCALE", filtro.CodiceLocale.Value)
                        op = "AND"
                    End If

                    If Not String.IsNullOrWhiteSpace(filtro.CodiceCentrale) Then
                        query.AppendLine(op + " PAZ_CODICE = :CODICE_CENTRALE")
                        cmd.Parameters.AddWithValue("CODICE_CENTRALE", filtro.CodiceCentrale)
                        op = "AND"
                    End If

                    If Not String.IsNullOrWhiteSpace(filtro.CognomeNome) Then
                        query.AppendLine(op + " PAZ_COGNOME_NOME like :COGNOMENOME")
                        cmd.Parameters.AddWithValue("COGNOMENOME", filtro.CognomeNome + "%")
                        op = "AND"
                    End If

                    If Not String.IsNullOrWhiteSpace(filtro.Cognome) Then
                        query.AppendLine(op + " PAZ_COGNOME like :COGNOME")
                        cmd.Parameters.AddWithValue("COGNOME", filtro.Cognome + "%")
                        op = "AND"
                    End If

                    If Not String.IsNullOrWhiteSpace(filtro.Nome) Then
                        query.AppendLine(op + " PAZ_NOME like :NOME")
                        cmd.Parameters.AddWithValue("NOME", filtro.Nome + "%")
                        op = "AND"
                    End If

                    If Not String.IsNullOrWhiteSpace(filtro.CodiceComuneNascita) Then
                        query.AppendLine(op + " PAZ_COM_CODICE_NASCITA=:COM_CODICE_NASCITA")
                        cmd.Parameters.AddWithValue("COM_CODICE_NASCITA", filtro.CodiceComuneNascita)
                        op = "AND"
                    End If

                    If Not String.IsNullOrWhiteSpace(filtro.Sesso) Then
                        query.AppendLine(op + " PAZ_SESSO=:SESSO")
                        cmd.Parameters.AddWithValue("SESSO", filtro.Sesso)
                        op = "AND"
                    End If

                    If filtro.DataNascita.HasValue Then
                        query.AppendLine(op + " PAZ_DATA_NASCITA=:DATA_NASCITA")
                        cmd.Parameters.AddWithValue("DATA_NASCITA", filtro.DataNascita.Value)
                        op = "AND"
                    End If

                    If filtro.AnnoNascita.HasValue Then
                        query.AppendLine(op + " PAZ_DATA_NASCITA >= :DATA_NASCITA_DA AND PAZ_DATA_NASCITA < :DATA_NASCITA_A")
                        cmd.Parameters.AddWithValue("DATA_NASCITA_DA", New Date(filtro.AnnoNascita.Value, 1, 1))
                        cmd.Parameters.AddWithValue("DATA_NASCITA_A", New Date(filtro.AnnoNascita.Value + 1, 1, 1))
                        op = "AND"
                    End If

                    If Not String.IsNullOrWhiteSpace(filtro.CodiceFiscale) Then
                        query.AppendLine(op + " PAZ_CODICE_FISCALE=:CODICE_FISCALE")
                        cmd.Parameters.AddWithValue("CODICE_FISCALE", filtro.CodiceFiscale)
                        op = "AND"
                    End If

                    If Not String.IsNullOrWhiteSpace(filtro.CodiceTesseraSanitaria) Then
                        query.AppendLine(op + " PAZ_TESSERA=:TESSERA")
                        cmd.Parameters.AddWithValue("TESSERA", filtro.CodiceTesseraSanitaria)
                        op = "AND"
                    End If

                    If Not String.IsNullOrWhiteSpace(filtro.CodiceComuneResidenza) Then
                        query.AppendLine(op + " PAZ_COM_CODICE_RESIDENZA=:COM_CODICE_RESIDENZA")
                        cmd.Parameters.AddWithValue("COM_CODICE_RESIDENZA", filtro.CodiceComuneResidenza)
                        op = "AND"
                    End If

                    If Not String.IsNullOrWhiteSpace(filtro.CodiceCentroVaccinale) Then
                        query.AppendLine(op + " PAZ_CNS_CODICE=:CNS_CODICE")
                        cmd.Parameters.AddWithValue("CNS_CODICE", filtro.CodiceCentroVaccinale)
                        op = "AND"
                    End If

                    If Not String.IsNullOrWhiteSpace(filtro.CodiceRegionale) Then
                        query.AppendLine(op + " PAZ_CODICE_REGIONALE=:PAZ_CODICE_REGIONALE")
                        cmd.Parameters.AddWithValue("PAZ_CODICE_REGIONALE", filtro.CodiceRegionale)
                        op = "AND"
                    End If

                    If String.IsNullOrWhiteSpace(ordinamento) Then
                        query.AppendLine(" ORDER BY PAZ_COGNOME, PAZ_NOME, PAZ_DATA_NASCITA")
                    Else
                        query.AppendFormat(" ORDER BY {0}", ordinamento).AppendLine()
                    End If

                    If filtro.MaxRecords.HasValue AndAlso filtro.MaxRecords > 0 Then

                        query.Insert(0, "select ROWNUM ROW_NUM, A.* from (")
                        query.AppendLine(") A where ROWNUM <= :maxrecord")

                        cmd.Parameters.AddWithValue("maxrecord", filtro.MaxRecords.Value)

                    End If

                    cmd.CommandText = query.ToString()

                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        Dim paz_codice As Integer = idr.GetOrdinal("PAZ_CODICE")
                        Dim paz_codice_locale As Integer = idr.GetOrdinal("PAZ_CODICE_LOCALE")
                        Dim paz_codice_fiscale As Integer = idr.GetOrdinal("PAZ_CODICE_FISCALE")
                        Dim paz_cognome As Integer = idr.GetOrdinal("PAZ_COGNOME")
                        Dim paz_data_nascita As Integer = idr.GetOrdinal("PAZ_DATA_NASCITA")
                        Dim paz_indirizzo_residenza As Integer = idr.GetOrdinal("PAZ_INDIRIZZO_RESIDENZA")
                        Dim paz_nome As Integer = idr.GetOrdinal("PAZ_NOME")
                        Dim paz_sesso As Integer = idr.GetOrdinal("PAZ_SESSO")
                        Dim paz_stato_anagrafico As Integer = idr.GetOrdinal("PAZ_STATO_ANAGRAFICO")
                        Dim paz_tessera As Integer = idr.GetOrdinal("PAZ_TESSERA")
                        Dim comune_nascita As Integer = idr.GetOrdinal("COMUNE_NASCITA")
                        Dim comune_residenza As Integer = idr.GetOrdinal("COMUNE_RESIDENZA")
                        Dim cns_codice As Integer = idr.GetOrdinal("CNS_CODICE")
                        Dim paz_cancellato As Integer = idr.GetOrdinal("PAZ_CANCELLATO")
                        Dim paz_codice_regionale As Integer = idr.GetOrdinal("PAZ_CODICE_REGIONALE")
                        Dim paz_tipo As Integer = idr.GetOrdinal("PAZ_TIPO")
                        Dim paz_usl_codice_assistenza As Integer = idr.GetOrdinal("PAZ_USL_CODICE_ASSISTENZA")
                        Dim paz_usl_codice_domicilio As Integer = idr.GetOrdinal("PAZ_USL_CODICE_DOMICILIO")

                        While idr.Read() And (Not filtro.MaxRecords.HasValue OrElse filtro.MaxRecords.Value > list.Count())

                            Dim item As New PazienteTrovato()

                            item.Fonte = Enumerators.FonteAnagrafica.AnagrafeCentrale

                            item.CodiceLocale = idr.GetNullableInt32OrDefault(paz_codice_locale)
                            item.CodiceCentrale = idr.GetStringOrDefault(paz_codice)
                            item.CodiceFiscale = idr.GetStringOrDefault(paz_codice_fiscale)
                            item.Cognome = idr.GetStringOrDefault(paz_cognome)
                            item.DataNascita = idr.GetNullableDateTimeOrDefault(paz_data_nascita)
                            item.IndirizzoResidenza = idr.GetStringOrDefault(paz_indirizzo_residenza)
                            item.Nome = idr.GetStringOrDefault(paz_nome)
                            item.Sesso = idr.GetStringOrDefault(paz_sesso)
                            item.StatoAnagrafico = idr.GetStringOrDefault(paz_stato_anagrafico)
                            item.Tessera = idr.GetStringOrDefault(paz_tessera)
                            item.ComuneNascita = idr.GetStringOrDefault(comune_nascita)
                            item.ComuneResidenza = idr.GetStringOrDefault(comune_residenza)
                            item.CodiceCentroVaccinale = idr.GetStringOrDefault(cns_codice)
                            item.Cancellato = idr.GetBooleanOrDefault(paz_cancellato)
                            item.CodiceRegionale = idr.GetStringOrDefault(paz_codice_regionale)
                            item.PazTipo = idr.GetStringOrDefault(paz_tipo)
                            item.CodiceUslAssistenza = idr.GetStringOrDefault(paz_usl_codice_assistenza)
                            item.CodiceUslDomicilio = idr.GetStringOrDefault(paz_usl_codice_domicilio)

                            list.Add(item)

                        End While

                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return list

        End Function

        Friend Function RicercaPazienteCentrale(codiceAusiliarioPaziente As String) As Paziente

            Dim p As Paziente = Nothing

            Dim query As String =
                "SELECT PAZ_CODICE_LOCALE, PAZ_STATO_ANAGRAFICO, PAZ_CNS_CODICE, PAZ_CANCELLATO,
                 PAZ_CODICE, PAZ_COGNOME, PAZ_NOME, PAZ_DATA_NASCITA, PAZ_SESSO, PAZ_CODICE_FISCALE, PAZ_COM_CODICE_NASCITA, PAZ_TESSERA, 
                 PAZ_COM_CODICE_RESIDENZA, PAZ_INDIRIZZO_RESIDENZA, PAZ_CAP_RESIDENZA, PAZ_COM_CODICE_DOMICILIO, PAZ_INDIRIZZO_DOMICILIO, PAZ_CAP_DOMICILIO,
                 PAZ_AZI_CODICE, PAZ_CIT_CODICE, PAZ_CODICE_DEMOGRAFICO, PAZ_CODICE_REGIONALE,   
                 PAZ_COM_CODICE_EMIGRAZIONE, PAZ_COM_CODICE_IMMIGRAZIONE, PAZ_DATA_AGG_DA_ANAG, PAZ_DATA_DECESSO, PAZ_DATA_EMIGRAZIONE, 
                 PAZ_DATA_FINE_DOMICILIO, PAZ_DATA_FINE_RESIDENZA, PAZ_DATA_IMMIGRAZIONE, PAZ_DATA_INIZIO_ASS, PAZ_DATA_CESSAZIONE_ASS, PAZ_DATA_INSERIMENTO,   
                 PAZ_DATA_INIZIO_DOMICILIO, PAZ_DATA_INIZIO_RESIDENZA, PAZ_DIS_CODICE, PAZ_LOCALE, PAZ_MED_CODICE_BASE, PAZ_MOTIVO_CESSAZIONE_ASS, PAZ_RSC_CODICE, 
                 PAZ_TELEFONO_1, PAZ_TELEFONO_2, PAZ_TELEFONO_3, PAZ_USL_CODICE_ASSISTENZA, PAZ_USL_CODICE_RESIDENZA, PAZ_USL_PROVENIENZA
                 FROM T_PAZ_PAZIENTI 
                 WHERE PAZ_CODICE = :PAZ_CODICE_AUSILIARIO"

            Using cmd As New OracleCommand(query, Connection)

                Dim ownConnection As Boolean = False

                cmd.Parameters.AddWithValue("PAZ_CODICE_AUSILIARIO", codiceAusiliarioPaziente)

                Try
                    ownConnection = ConditionalOpenConnection(cmd)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim paz_codice_locale As Integer = idr.GetOrdinal("PAZ_CODICE_LOCALE")                          ' paz_codice
                            Dim paz_stato_anagrafico As Integer = idr.GetOrdinal("PAZ_STATO_ANAGRAFICO")
                            Dim paz_cns_codice As Integer = idr.GetOrdinal("PAZ_CNS_CODICE")
                            Dim paz_cancellato As Integer = idr.GetOrdinal("PAZ_CANCELLATO")
                            Dim paz_codice As Integer = idr.GetOrdinal("PAZ_CODICE")                                        ' ausiliario
                            Dim paz_cognome As Integer = idr.GetOrdinal("PAZ_COGNOME")
                            Dim paz_nome As Integer = idr.GetOrdinal("PAZ_NOME")
                            Dim paz_data_nascita As Integer = idr.GetOrdinal("PAZ_DATA_NASCITA")
                            Dim paz_sesso As Integer = idr.GetOrdinal("PAZ_SESSO")
                            Dim paz_codice_fiscale As Integer = idr.GetOrdinal("PAZ_CODICE_FISCALE")
                            Dim paz_com_codice_nascita As Integer = idr.GetOrdinal("PAZ_COM_CODICE_NASCITA")
                            Dim paz_tessera As Integer = idr.GetOrdinal("PAZ_TESSERA")
                            Dim paz_com_codice_residenza As Integer = idr.GetOrdinal("PAZ_COM_CODICE_RESIDENZA")
                            Dim paz_indirizzo_residenza As Integer = idr.GetOrdinal("PAZ_INDIRIZZO_RESIDENZA")
                            Dim paz_cap_residenza As Integer = idr.GetOrdinal("PAZ_CAP_RESIDENZA")
                            Dim paz_com_codice_domicilio As Integer = idr.GetOrdinal("PAZ_COM_CODICE_DOMICILIO")
                            Dim paz_indirizzo_domicilio As Integer = idr.GetOrdinal("PAZ_INDIRIZZO_DOMICILIO")
                            Dim paz_cap_domicilio As Integer = idr.GetOrdinal("PAZ_CAP_DOMICILIO")
                            Dim paz_azi_codice As Integer = idr.GetOrdinal("PAZ_AZI_CODICE")
                            Dim paz_cit_codice As Integer = idr.GetOrdinal("PAZ_CIT_CODICE")
                            Dim paz_codice_demografico As Integer = idr.GetOrdinal("PAZ_CODICE_DEMOGRAFICO")
                            Dim paz_codice_regionale As Integer = idr.GetOrdinal("PAZ_CODICE_REGIONALE")
                            Dim paz_com_codice_emigrazione As Integer = idr.GetOrdinal("PAZ_COM_CODICE_EMIGRAZIONE")
                            Dim paz_com_codice_immigrazione As Integer = idr.GetOrdinal("PAZ_COM_CODICE_IMMIGRAZIONE")      ' provenienza
                            Dim paz_data_agg_da_anag As Integer = idr.GetOrdinal("PAZ_DATA_AGG_DA_ANAG")
                            Dim paz_data_decesso As Integer = idr.GetOrdinal("PAZ_DATA_DECESSO")
                            Dim paz_data_emigrazione As Integer = idr.GetOrdinal("PAZ_DATA_EMIGRAZIONE")
                            Dim paz_data_fine_domicilio As Integer = idr.GetOrdinal("PAZ_DATA_FINE_DOMICILIO")
                            Dim paz_data_fine_residenza As Integer = idr.GetOrdinal("PAZ_DATA_FINE_RESIDENZA")
                            Dim paz_data_immigrazione As Integer = idr.GetOrdinal("PAZ_DATA_IMMIGRAZIONE")
                            Dim paz_data_inizio_ass As Integer = idr.GetOrdinal("PAZ_DATA_INIZIO_ASS")
                            Dim paz_data_cessazione_ass As Integer = idr.GetOrdinal("PAZ_DATA_CESSAZIONE_ASS")
                            Dim paz_data_inizio_domicilio As Integer = idr.GetOrdinal("PAZ_DATA_INIZIO_DOMICILIO")
                            Dim paz_data_inizio_residenza As Integer = idr.GetOrdinal("PAZ_DATA_INIZIO_RESIDENZA")
                            Dim paz_data_inserimento As Integer = idr.GetOrdinal("PAZ_DATA_INSERIMENTO")
                            Dim paz_dis_codice As Integer = idr.GetOrdinal("PAZ_DIS_CODICE")
                            Dim paz_locale As Integer = idr.GetOrdinal("PAZ_LOCALE")
                            Dim paz_med_codice_base As Integer = idr.GetOrdinal("PAZ_MED_CODICE_BASE")
                            Dim paz_motivo_cessazione_ass As Integer = idr.GetOrdinal("PAZ_MOTIVO_CESSAZIONE_ASS")
                            Dim paz_rsc_codice As Integer = idr.GetOrdinal("PAZ_RSC_CODICE")
                            Dim paz_telefono_1 As Integer = idr.GetOrdinal("PAZ_TELEFONO_1")
                            Dim paz_telefono_2 As Integer = idr.GetOrdinal("PAZ_TELEFONO_2")
                            Dim paz_telefono_3 As Integer = idr.GetOrdinal("PAZ_TELEFONO_3")
                            Dim paz_usl_codice_assistenza As Integer = idr.GetOrdinal("PAZ_USL_CODICE_ASSISTENZA")
                            Dim paz_usl_codice_residenza As Integer = idr.GetOrdinal("PAZ_USL_CODICE_RESIDENZA")
                            Dim paz_usl_provenienza As Integer = idr.GetOrdinal("PAZ_USL_PROVENIENZA")

                            p = New Paziente()

                            If idr.Read() Then

                                p.Paz_Codice = idr.GetInt32OrDefault(paz_codice_locale)
                                p.StatoAnagrafico = idr.GetNullableEnumOrDefault(Of Enumerators.StatoAnagrafico)(paz_stato_anagrafico)
                                p.Paz_Cns_Codice = idr.GetStringOrDefault(paz_cns_codice)
                                p.FlagCancellato = idr.GetStringOrDefault(paz_cancellato)

                                p.CodiceAusiliario = idr.GetStringOrDefault(paz_codice)
                                p.PAZ_COGNOME = idr.GetStringOrDefault(paz_cognome)
                                p.PAZ_NOME = idr.GetStringOrDefault(paz_nome)
                                p.Data_Nascita = idr.GetDateTimeOrDefault(paz_data_nascita)
                                p.Sesso = idr.GetStringOrDefault(paz_sesso)
                                p.PAZ_CODICE_FISCALE = idr.GetStringOrDefault(paz_codice_fiscale)
                                p.ComuneNascita_Codice = idr.GetStringOrDefault(paz_com_codice_nascita)
                                p.Tessera = idr.GetStringOrDefault(paz_tessera)
                                p.ComuneResidenza_Codice = idr.GetStringOrDefault(paz_com_codice_residenza)
                                p.IndirizzoResidenza = idr.GetStringOrDefault(paz_indirizzo_residenza)
                                p.ComuneResidenza_Cap = idr.GetStringOrDefault(paz_cap_residenza)
                                p.ComuneDomicilio_Codice = idr.GetStringOrDefault(paz_com_codice_domicilio)
                                p.IndirizzoDomicilio = idr.GetStringOrDefault(paz_indirizzo_domicilio)
                                p.ComuneDomicilio_Cap = idr.GetStringOrDefault(paz_cap_domicilio)
                                p.CodiceAzienda = idr.GetStringOrDefault(paz_azi_codice)
                                p.Cittadinanza_Codice = idr.GetStringOrDefault(paz_cit_codice)
                                p.CodiceDemografico = idr.GetStringOrDefault(paz_codice_demografico)
                                p.PAZ_CODICE_REGIONALE = idr.GetStringOrDefault(paz_codice_regionale)
                                p.ComuneEmigrazione_Codice = idr.GetStringOrDefault(paz_com_codice_emigrazione)
                                p.ComuneProvenienza_Codice = idr.GetStringOrDefault(paz_com_codice_immigrazione)
                                p.DataAggiornamentoDaAnagrafe = idr.GetNullableDateTimeOrDefault(paz_data_agg_da_anag)
                                p.DataDecesso = idr.GetDateTimeOrDefault(paz_data_decesso)
                                p.DataEmigrazione = idr.GetDateTimeOrDefault(paz_data_emigrazione)
                                p.ComuneDomicilio_DataFine = idr.GetDateTimeOrDefault(paz_data_fine_domicilio)
                                p.ComuneResidenza_DataFine = idr.GetDateTimeOrDefault(paz_data_fine_residenza)
                                p.DataImmigrazione = idr.GetDateTimeOrDefault(paz_data_immigrazione)
                                p.UslAssistenza_DataInizio = idr.GetDateTimeOrDefault(paz_data_inizio_ass)
                                p.UslAssistenza_DataCessazione = idr.GetDateTimeOrDefault(paz_data_cessazione_ass)
                                p.ComuneDomicilio_DataInizio = idr.GetDateTimeOrDefault(paz_data_inizio_domicilio)
                                p.ComuneResidenza_DataInizio = idr.GetDateTimeOrDefault(paz_data_inizio_residenza)
                                p.DataInserimento = idr.GetDateTimeOrDefault(paz_data_inserimento)
                                p.Distretto_Codice = idr.GetStringOrDefault(paz_dis_codice)
                                p.FlagLocale = idr.GetStringOrDefault(paz_locale)
                                p.MedicoBase_Codice = idr.GetStringOrDefault(paz_med_codice_base)
                                p.MotivoCessazioneAssistenza = idr.GetStringOrDefault(paz_motivo_cessazione_ass)
                                p.CodiceCategoriaRischio = idr.GetStringOrDefault(paz_rsc_codice)
                                p.Telefono1 = idr.GetStringOrDefault(paz_telefono_1)
                                p.Telefono2 = idr.GetStringOrDefault(paz_telefono_2)
                                p.Telefono3 = idr.GetStringOrDefault(paz_telefono_3)
                                p.UslAssistenza_Codice = idr.GetStringOrDefault(paz_usl_codice_assistenza)
                                p.UslResidenza_Codice = idr.GetStringOrDefault(paz_usl_codice_residenza)
                                p.UslAssistenzaPrecedente_Codice = idr.GetStringOrDefault(paz_usl_provenienza)

                            End If

                        End If
                    End Using

                Finally
                    ConditionalCloseConnection(ownConnection)
                End Try

            End Using

            Return p

        End Function

#End Region

#Region " AVN "

        Public Function InsertTrasferimentoAVN(codiceLocalePaziente As Integer, codiceRegionale As String, codiceComuneEmigrazione As String, dataEmigrazione As Date) As Integer Implements IPazienteProvider.InsertTrasferimentoAVN

            Dim count As Integer = 0

            Dim query As String = "INSERT INTO T_AVN_TR
(AVR_PAZ_CODICE, AVR_PAZ_CODICE_REGIONALE, AVR_DATA_EMIGRAZIONE, AVR_COM_CODICE_EMIGRAZIONE)
VALUES
(:AVR_PAZ_CODICE, :AVR_PAZ_CODICE_REGIONALE, :AVR_DATA_EMIGRAZIONE, :AVR_COM_CODICE_EMIGRAZIONE)"

            Using cmd As New OracleCommand(query, Connection)

                Dim ownConnection As Boolean = False

                If codiceLocalePaziente > 0 Then
                    cmd.Parameters.AddWithValue("AVR_PAZ_CODICE", codiceLocalePaziente)
                Else
                    cmd.Parameters.AddWithValue("AVR_PAZ_CODICE", DBNull.Value)
                End If

                cmd.Parameters.AddWithValue("AVR_PAZ_CODICE_REGIONALE", codiceRegionale)
                cmd.Parameters.AddWithValue("AVR_DATA_EMIGRAZIONE", dataEmigrazione)
                cmd.Parameters.AddWithValue("AVR_COM_CODICE_EMIGRAZIONE", codiceComuneEmigrazione)

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

    End Class

End Namespace
