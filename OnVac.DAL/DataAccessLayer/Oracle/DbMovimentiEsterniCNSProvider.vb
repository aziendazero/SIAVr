Imports System.Collections.ObjectModel
Imports Onit.Database.DataAccessManager


Namespace DAL.Oracle

    Public Class DbMovimentiEsterniCNSProvider
        Inherits DbProvider
        Implements IMovimentiEsterniCNSProvider

#Region " Constructors "

        Public Sub New(DAM As IDAM)

            MyBase.New(DAM)

        End Sub

#End Region

#Region " Paziente "

        Public Function LoadPazientiInIngresso(pazientiInIngressoFilter As MovimentiCNSPazientiInIngressoFilter, orderBy As String) As DstMovimentiEsterni Implements IMovimentiEsterniCNSProvider.LoadPazientiInIngresso

            Return Me.LoadPazientiInIngresso(pazientiInIngressoFilter, orderBy, Nothing)

        End Function

        Public Function LoadPazientiInIngresso(pazientiInIngressoFilter As MovimentiCNSPazientiInIngressoFilter, orderBy As String, pagingOptions As MovimentiCNSPagingOptions?) As DstMovimentiEsterni Implements IMovimentiEsterniCNSProvider.LoadPazientiInIngresso

            Dim dstPazientiInIngresso As New DstMovimentiEsterni()

            With _DAM.QB

                .NewQuery()
                .IsDistinct = True

                .AddSelectFields("paz_codice, paz_cognome, paz_nome, paz_data_nascita, paz_indirizzo_domicilio, paz_com_codice_nascita")
                .AddSelectFields("paz_com_codice_domicilio, paz_com_comune_provenienza, paz_com_comune_emigrazione, paz_richiesta_certificato")
                .AddSelectFields("paz_sta_certificato_emi, paz_data_emigrazione, paz_data_immigrazione, paz_com_codice_residenza, paz_indirizzo_residenza")
                .AddSelectFields("paz_regolarizzato, paz_stato_acquisizione")
                '--
                ' TODO: servono?
                .AddSelectFields("paz_stato_notifica_emi, paz_stato_acquisizione_imi")

                .AddSelectFields("t_ana_comuni_nascita.com_descrizione nascita_com_descrizione, t_ana_comuni_nascita.com_provincia nascita_com_provincia, t_ana_comuni_nascita.com_cap nascita_com_cap")
                .AddSelectFields("t_ana_comuni_domicilio.com_descrizione dom_com_descrizione, t_ana_comuni_domicilio.com_provincia dom_com_provincia, t_ana_comuni_domicilio.com_cap dom_com_cap")
                .AddSelectFields("t_ana_comuni_provenienza.com_descrizione proven_com_descrizione, t_ana_comuni_provenienza.com_provincia proven_com_provincia, t_ana_comuni_provenienza.com_cap proven_com_cap")
                .AddSelectFields("t_ana_comuni_residenza.com_descrizione res_com_descrizione, t_ana_comuni_residenza.com_provincia res_com_provincia", .FC.IsNull("paz_cap_residenza", "t_ana_comuni_residenza.com_cap", DataTypes.Replace) & " res_com_cap")
                .AddSelectFields("paz_stato_anagrafico, san_descrizione stato_anagrafico")
                .AddSelectFields("paz_data_inizio_residenza, paz_data_inizio_domicilio, paz_data_inizio_ass")
                .AddSelectFields("paz_usl_provenienza, t_ana_usl_provenienza.usl_descrizione usl_provenienza_descrizione")
                .AddSelectFields("paz_usl_codice_assistenza, t_ana_usl_assistenza.usl_descrizione usl_assistenza_descrizione")

                .AddTables("t_paz_pazienti")
                .AddTables("t_ana_stati_anagrafici")
                .AddTables("t_ana_comuni t_ana_comuni_nascita")
                .AddTables("t_ana_comuni t_ana_comuni_domicilio")
                .AddTables("t_ana_comuni t_ana_comuni_provenienza")
                .AddTables("t_ana_comuni t_ana_comuni_residenza")
                .AddTables("t_ana_usl t_ana_usl_provenienza")
                .AddTables("t_ana_usl t_ana_usl_assistenza")

                .AddWhereCondition("paz_stato_anagrafico", Comparatori.Uguale, "san_codice", DataTypes.OutJoinLeft)

                .AddWhereCondition("paz_com_codice_nascita", Comparatori.Uguale, "t_ana_comuni_nascita.com_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("paz_com_codice_residenza", Comparatori.Uguale, "t_ana_comuni_residenza.com_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("paz_com_comune_provenienza", Comparatori.Uguale, "t_ana_comuni_provenienza.com_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("paz_com_codice_domicilio", Comparatori.Uguale, "t_ana_comuni_domicilio.com_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("paz_usl_provenienza", Comparatori.Uguale, "t_ana_usl_provenienza.usl_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("paz_usl_codice_assistenza", Comparatori.Uguale, "t_ana_usl_assistenza.usl_codice", DataTypes.OutJoinLeft)

                AddPazientiInIngressoWhereCondition(_DAM.QB, pazientiInIngressoFilter)

                .AddOrderByFields(orderBy)

            End With

            If Not pagingOptions Is Nothing Then
                _DAM.QB.AddPaginatedOracleQuery(pagingOptions.Value.StartRecordIndex, pagingOptions.Value.EndRecordIndex)
            End If

            Try
                _DAM.BuildDataTable(dstPazientiInIngresso.MovimentiEsterni)
            Finally
                _DAM.Dispose(False)
            End Try

            Return dstPazientiInIngresso

        End Function

        Public Function CountPazientiInIngresso(pazientiInIngressoFilter As MovimentiCNSPazientiInIngressoFilter) As Integer Implements IMovimentiEsterniCNSProvider.CountPazientiInIngresso

            With _DAM.QB

                .NewQuery()
                .AddSelectFields("COUNT(distinct paz_codice)")
                .AddTables("t_paz_pazienti")

                AddPazientiInIngressoWhereCondition(_DAM.QB, pazientiInIngressoFilter)

            End With

            Try
                Return _DAM.ExecScalar()
            Finally
                _DAM.Dispose(False)
            End Try

        End Function

        Public Function LoadImmigrati(immigratiFilter As MovimentiCNSImmigratiFilter, pagingOptions As MovimentiCNSPagingOptions?) As DstMovimentiEsterni Implements IMovimentiEsterniCNSProvider.LoadImmigrati
            '--
            Dim dstImmigrati As New DstMovimentiEsterni
            '--
            With _DAM.QB

                .NewQuery()

                ' Sottoquery per escludere i pazienti che hanno comune di provenienza facente parte dell'asl corrente
                .AddSelectFields("1")
                .AddTables("t_ana_link_comuni_usl")
                .AddWhereCondition("lcu_com_codice", Comparatori.Uguale, "paz_com_comune_provenienza", DataTypes.Join)
                .AddWhereCondition("lcu_usl_codice", Comparatori.Uguale, immigratiFilter.CodiceAsl, DataTypes.Join)

                Dim subQueryImmigrazioneAslCorrente As String = .GetSelect()

                .NewQuery(False, False)
                '--
                .IsDistinct = True
                '--
                .AddSelectFields("paz_codice, paz_cognome, paz_nome, paz_data_nascita, paz_indirizzo_domicilio, paz_com_codice_nascita, paz_com_codice_domicilio, paz_com_comune_provenienza, paz_richiesta_certificato, paz_sta_certificato_emi, paz_data_emigrazione, paz_data_immigrazione, paz_com_codice_residenza, paz_indirizzo_residenza, paz_regolarizzato, paz_stato_notifica_emi, paz_stato_acquisizione_imi")
                .AddSelectFields("t_ana_comuni_nascita.com_descrizione nascita_com_descrizione, t_ana_comuni_nascita.com_provincia nascita_com_provincia, t_ana_comuni_nascita.com_cap nascita_com_cap")
                .AddSelectFields("t_ana_comuni_domicilio.com_descrizione dom_com_descrizione, t_ana_comuni_domicilio.com_provincia dom_com_provincia, t_ana_comuni_domicilio.com_cap dom_com_cap")
                .AddSelectFields("t_ana_comuni_provenienza.com_descrizione proven_com_descrizione, t_ana_comuni_provenienza.com_provincia proven_com_provincia, t_ana_comuni_provenienza.com_cap proven_com_cap")
                .AddSelectFields("t_ana_comuni_residenza.com_descrizione res_com_descrizione, t_ana_comuni_residenza.com_provincia res_com_provincia", .FC.IsNull("paz_cap_residenza", "t_ana_comuni_residenza.com_cap", DataTypes.Replace) & " res_com_cap")
                .AddSelectFields("ugs_app_id ugs_app_id_imi, paz_stato_anagrafico, san_descrizione stato_anagrafico")
                '--
                .AddTables("t_paz_pazienti, t_ana_comuni t_ana_comuni_nascita, t_ana_comuni t_ana_comuni_domicilio")
                .AddTables("t_ana_comuni t_ana_comuni_provenienza, t_ana_comuni t_ana_comuni_residenza")
                .AddTables("t_ana_link_comuni_usl, t_usl_gestite, t_ana_stati_anagrafici")
                '--
                .AddWhereCondition("paz_com_codice_nascita", Comparatori.Uguale, "t_ana_comuni_nascita.com_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("paz_com_codice_residenza", Comparatori.Uguale, "t_ana_comuni_residenza.com_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("paz_com_comune_provenienza", Comparatori.Uguale, "t_ana_comuni_provenienza.com_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("paz_com_codice_domicilio", Comparatori.Uguale, "t_ana_comuni_domicilio.com_codice", DataTypes.OutJoinLeft)
                '--
                .AddWhereCondition("paz_com_comune_provenienza", Comparatori.Uguale, "lcu_com_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("lcu_usl_codice", Comparatori.Uguale, "ugs_usl_codice", DataTypes.OutJoinLeft)
                .AddWhereCondition("paz_stato_anagrafico", Comparatori.Uguale, "san_codice", DataTypes.OutJoinLeft)
                '--
                Me.AddLoadImmigratiWhereCondition(_DAM.QB, immigratiFilter, subQueryImmigrazioneAslCorrente)
                '--
                .AddOrderByFields("paz_cognome, paz_nome, paz_data_nascita, paz_codice")
                '--
            End With
            '--
            If Not pagingOptions Is Nothing Then
                '--
                _DAM.QB.AddPaginatedOracleQuery(pagingOptions.Value.StartRecordIndex, pagingOptions.Value.EndRecordIndex)
                '--
            End If
            '--
            Try
                _DAM.BuildDataTable(dstImmigrati.MovimentiEsterni)
            Finally
                _DAM.Dispose(False)
            End Try
            '--
            Return dstImmigrati
            '--
        End Function

        Public Function CountImmigrati(immigratiFilter As MovimentiCNSImmigratiFilter) As Int32 Implements IMovimentiEsterniCNSProvider.CountImmigrati

            With _DAM.QB

                .NewQuery()

                ' Sottoquery per escludere i pazienti che hanno comune di provenienza facente parte dell'asl corrente
                .AddSelectFields("1")
                .AddTables("t_ana_link_comuni_usl")
                .AddWhereCondition("lcu_com_codice", Comparatori.Uguale, "paz_com_comune_provenienza", DataTypes.Join)
                .AddWhereCondition("lcu_usl_codice", Comparatori.Uguale, immigratiFilter.CodiceAsl, DataTypes.Join)

                Dim subQueryImmigrazioneAslCorrente As String = .GetSelect()

                .NewQuery(False, False)
                .AddSelectFields("COUNT(distinct paz_codice)")
                .AddTables("t_paz_pazienti, t_ana_link_comuni_usl")
                .AddWhereCondition("paz_com_comune_provenienza", Comparatori.Uguale, "lcu_com_codice", DataTypes.OutJoinLeft)

                AddLoadImmigratiWhereCondition(_DAM.QB, immigratiFilter, subQueryImmigrazioneAslCorrente)

            End With

            Try
                Return _DAM.ExecScalar()
            Finally
                _DAM.Dispose(False)
            End Try

        End Function

        Public Function LoadEmigrati(emigratiFilter As MovimentiCNSEmigratiFilter, pagingOptions As MovimentiCNSPagingOptions?, codiceAslCorrente As String) As DstMovimentiEsterni Implements IMovimentiEsterniCNSProvider.LoadEmigrati

            Dim dstMovimentiEsterni As New DstMovimentiEsterni()

            With _DAM.QB

                CreateQueryLoadEmigrati(_DAM.QB, emigratiFilter, codiceAslCorrente, False)

                .AddOrderByFields("paz_cognome, paz_nome, paz_data_nascita, paz_codice")

            End With

            If Not pagingOptions Is Nothing Then _DAM.QB.AddPaginatedOracleQuery(pagingOptions.Value.StartRecordIndex, pagingOptions.Value.EndRecordIndex)

            Try
                _DAM.BuildDataTable(dstMovimentiEsterni.MovimentiEsterni)
            Finally
                _DAM.Dispose(False)
            End Try

            Return dstMovimentiEsterni

        End Function

        Public Function CountEmigrati(emigratiFilter As MovimentiCNSEmigratiFilter, codiceAslCorrente As String) As Int32 Implements IMovimentiEsterniCNSProvider.CountEmigrati

            With _DAM.QB
                CreateQueryLoadEmigrati(_DAM.QB, emigratiFilter, codiceAslCorrente, True)
            End With

            Try
                Return _DAM.ExecScalar()
            Finally
                _DAM.Dispose(False)
            End Try

        End Function

#Region " Private "

        Private Sub AddPazientiInIngressoWhereCondition(abstractQB As AbstractQB, immigratiFilter As MovimentiCNSPazientiInIngressoFilter)
            '--
            If immigratiFilter Is Nothing Then Return
            '--
            With abstractQB
                '--
                .AddWhereCondition("1", Comparatori.Uguale, "1", DataTypes.Numero)
                '--
                ' Filtro data di nascita
                '--
                If immigratiFilter.DataNascitaInizio.HasValue Then
                    .AddWhereCondition("paz_data_nascita", Comparatori.MaggioreUguale, immigratiFilter.DataNascitaInizio.Value, DataTypes.Data)
                End If
                '--
                If immigratiFilter.DataNascitaFine.HasValue Then
                    .AddWhereCondition("paz_data_nascita", Comparatori.Minore, immigratiFilter.DataNascitaFine.Value.AddDays(1), DataTypes.Data)
                End If
                '--
                ' Filtro data di immigrazione
                '--
                If immigratiFilter.DataImmigrazioneInizio.HasValue Then
                    .AddWhereCondition("paz_data_immigrazione", Comparatori.MaggioreUguale, immigratiFilter.DataImmigrazioneInizio.Value, DataTypes.Data)
                End If
                '--
                If immigratiFilter.DataImmigrazioneFine.HasValue Then
                    .AddWhereCondition("paz_data_immigrazione", Comparatori.Minore, immigratiFilter.DataImmigrazioneFine.Value.AddDays(1), DataTypes.Data)
                End If
                '--
                ' Filtro data di inizio residenza
                '--
                If immigratiFilter.DataResidenzaInizio.HasValue Then
                    .AddWhereCondition("paz_data_inizio_residenza", Comparatori.MaggioreUguale, immigratiFilter.DataResidenzaInizio.Value, DataTypes.Data)
                End If
                '--
                If immigratiFilter.DataResidenzaFine.HasValue Then
                    .AddWhereCondition("paz_data_inizio_residenza", Comparatori.Minore, immigratiFilter.DataResidenzaFine.Value.AddDays(1), DataTypes.Data)
                End If
                '--
                ' Filtro data di inizio domicilio
                '--
                If immigratiFilter.DataDomicilioInizio.HasValue Then
                    .AddWhereCondition("paz_data_inizio_domicilio", Comparatori.MaggioreUguale, immigratiFilter.DataDomicilioInizio.Value, DataTypes.Data)
                End If
                '--
                If immigratiFilter.DataDomicilioFine.HasValue Then
                    .AddWhereCondition("paz_data_inizio_domicilio", Comparatori.Minore, immigratiFilter.DataDomicilioFine.Value.AddDays(1), DataTypes.Data)
                End If
                '--
                ' Filtro data di assistenza
                '--
                If immigratiFilter.DataAssistenzaInizio.HasValue Then
                    .AddWhereCondition("paz_data_inizio_ass", Comparatori.MaggioreUguale, immigratiFilter.DataAssistenzaInizio.Value, DataTypes.Data)
                End If
                '--
                If immigratiFilter.DataAssistenzaFine.HasValue Then
                    .AddWhereCondition("paz_data_inizio_ass", Comparatori.Minore, immigratiFilter.DataAssistenzaFine.Value.AddDays(1), DataTypes.Data)
                End If
                '--
                ' Filtro flag regolarizzazione
                '--
                If Not immigratiFilter.Regolarizzato Is Nothing Then
                    '--
                    If immigratiFilter.Regolarizzato.Value Then
                        .AddWhereCondition("paz_regolarizzato", Comparatori.Uguale, "S", DataTypes.Stringa)
                    Else
                        .OpenParanthesis()
                        .AddWhereCondition("paz_regolarizzato", Comparatori.Uguale, "N", DataTypes.Stringa)
                        .AddWhereCondition("paz_regolarizzato", Comparatori.Is, "NULL", DataTypes.Replace, "OR")
                        .CloseParanthesis()
                    End If
                    '--
                End If
                '--
                ' Filtro stato acquisizione
                '--
                If Not immigratiFilter.StatoAcquisizioneDatiVaccinaliCentrale Is Nothing Then
                    '--
                    If immigratiFilter.StatoAcquisizioneDatiVaccinaliCentrale = MovimentiCNSPazientiInIngressoFilter.ValoriFiltroStatoAcquisizione.AcquisizioneNonEffettuata Then
                        .AddWhereCondition("paz_stato_acquisizione", Comparatori.Is, "NULL", DataTypes.Replace)
                    Else
                        .AddWhereCondition("paz_stato_acquisizione", Comparatori.Uguale, immigratiFilter.StatoAcquisizioneDatiVaccinaliCentrale, DataTypes.Numero)
                    End If
                    '--
                End If
                '--
                ' Filtro stati anagrafici
                '--
                If Not immigratiFilter.StatiAnagrafici Is Nothing AndAlso immigratiFilter.StatiAnagrafici.Count > 0 Then
                    '--
                    If immigratiFilter.StatiAnagrafici.Count > 1 Then .OpenParanthesis()
                    '--
                    .AddWhereCondition("paz_stato_anagrafico", Comparatori.Uguale, immigratiFilter.StatiAnagrafici(0), DataTypes.Stringa)
                    '--
                    For i As Integer = 1 To immigratiFilter.StatiAnagrafici.Count - 1
                        .AddWhereCondition("paz_stato_anagrafico", Comparatori.Uguale, immigratiFilter.StatiAnagrafici(i), DataTypes.Stringa, "OR")
                    Next
                    '--
                    If immigratiFilter.StatiAnagrafici.Count > 1 Then .CloseParanthesis()
                    '--
                End If
                '--
                If Not String.IsNullOrEmpty(immigratiFilter.CodiceConsultorio) Then
                    .AddWhereCondition("paz_cns_codice", Comparatori.Uguale, immigratiFilter.CodiceConsultorio, DataTypes.Stringa)
                End If

            End With
            '--
        End Sub

        Private Sub AddLoadImmigratiWhereCondition(abstractQB As AbstractQB, immigratiFilter As MovimentiCNSImmigratiFilter, subQueryImmigrazioneAslCorrente As String)
            '--
            With abstractQB
                '--
                ' Escludo dall'elenco i pazienti che hanno come comune di immigrazione un comune facente parte dell'asl corrente
                'OLD:
                '.OpenParanthesis()
                '.AddWhereCondition("lcu_usl_codice", Comparatori.Diverso, immigratiFilter.CodiceAsl, DataTypes.Stringa)
                '.AddWhereCondition("lcu_usl_codice", Comparatori.Is, "NULL", DataTypes.Replace, "OR")
                '.CloseParanthesis()
                .AddWhereCondition("", Comparatori.NotExist, String.Format("({0})", subQueryImmigrazioneAslCorrente), DataTypes.Replace)
                '--
                .AddWhereCondition("paz_cns_codice", Comparatori.Uguale, immigratiFilter.CodiceConsultorio, DataTypes.Stringa)
                .OpenParanthesis()
                .AddWhereCondition("paz_com_comune_provenienza", Comparatori.[IsNot], "NULL", DataTypes.Replace)
                .AddWhereCondition("paz_data_immigrazione", Comparatori.[IsNot], "NULL", DataTypes.Replace, "OR")
                .CloseParanthesis()
                '--
                If Not immigratiFilter Is Nothing Then
                    '--
                    If Not immigratiFilter.DataNascitaInizio Is Nothing Then
                        .AddWhereCondition("paz_data_nascita", Comparatori.MaggioreUguale, immigratiFilter.DataNascitaInizio, DataTypes.Data)
                    End If
                    '--
                    If Not immigratiFilter.DataNascitaFine Is Nothing Then
                        .AddWhereCondition("paz_data_nascita", Comparatori.Minore, immigratiFilter.DataNascitaFine.Value.AddDays(1), DataTypes.Data)
                    End If
                    '--
                    If Not immigratiFilter.DataImmigrazioneInizio Is Nothing OrElse Not immigratiFilter.DataImmigrazioneFine Is Nothing Then
                        .OpenParanthesis()
                        .OpenParanthesis()
                        If Not immigratiFilter.DataImmigrazioneInizio Is Nothing Then
                            .AddWhereCondition("paz_data_immigrazione", Comparatori.MaggioreUguale, immigratiFilter.DataImmigrazioneInizio, DataTypes.Data)
                        End If
                        If Not immigratiFilter.DataImmigrazioneFine Is Nothing Then
                            .AddWhereCondition("paz_data_immigrazione", Comparatori.Minore, immigratiFilter.DataImmigrazioneFine.Value.AddDays(1), DataTypes.Data)
                        End If
                        .CloseParanthesis()
                        .AddWhereCondition("paz_data_immigrazione", Comparatori.Is, "NULL", DataTypes.Replace, "OR")
                        .CloseParanthesis()
                    End If
                    '--
                    If Not immigratiFilter.Regolarizzato Is Nothing Then
                        If immigratiFilter.Regolarizzato Then
                            .AddWhereCondition("paz_regolarizzato", Comparatori.Uguale, "S", DataTypes.Stringa)
                        Else
                            .OpenParanthesis()
                            .AddWhereCondition("paz_regolarizzato", Comparatori.Uguale, "N", DataTypes.Stringa)
                            .AddWhereCondition("paz_regolarizzato", Comparatori.Is, "NULL", DataTypes.Replace, "OR")
                            .CloseParanthesis()
                        End If
                    End If
                    '--
                    If Not immigratiFilter.CertificatoRichiesto Is Nothing Then
                        If immigratiFilter.CertificatoRichiesto Then
                            .AddWhereCondition("PAZ_RICHIESTA_CERTIFICATO", Comparatori.Uguale, "S", DataTypes.Stringa)
                        Else
                            .OpenParanthesis()
                            .AddWhereCondition("PAZ_RICHIESTA_CERTIFICATO", Comparatori.Uguale, "N", DataTypes.Stringa)
                            .AddWhereCondition("PAZ_RICHIESTA_CERTIFICATO", Comparatori.Is, "NULL", DataTypes.Replace, "OR")
                            .CloseParanthesis()
                        End If
                    End If
                    '--
                    If Not immigratiFilter.StatiAcquisizioneImmigrazione Is Nothing Then
                        '--
                        If immigratiFilter.StatiAcquisizioneImmigrazione.Length > 1 Then
                            .OpenParanthesis()
                        End If
                        '--
                        For i As Integer = 0 To immigratiFilter.StatiAcquisizioneImmigrazione.Length - 1
                            If immigratiFilter.StatiAcquisizioneImmigrazione(i) <> Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Nessuno Then
                                .AddWhereCondition("PAZ_STATO_ACQUISIZIONE_IMI", Comparatori.Uguale, immigratiFilter.StatiAcquisizioneImmigrazione(i).ToString("d"), DataTypes.Stringa, IIf(i > 0, "OR", "AND"))
                            Else
                                .AddWhereCondition("PAZ_STATO_ACQUISIZIONE_IMI", Comparatori.Is, "NULL", DataTypes.Replace, IIf(i > 0, "OR", "AND"))
                            End If
                        Next
                        '--
                        If immigratiFilter.StatiAcquisizioneImmigrazione.Length > 1 Then
                            .CloseParanthesis()
                        End If
                        '--
                    End If
                    '--
                End If
                '--
            End With
            '--
        End Sub

        Private Sub CreateQueryLoadEmigrati(abstractQB As AbstractQB, emigratiFilter As MovimentiCNSEmigratiFilter, uslCorrente As String, isQueryCountPazienti As Boolean)
            '--
            With abstractQB

                .NewQuery()

                ' Sottoquery per escludere i pazienti che hanno comune di emigrazione facente parte dell'asl corrente
                .AddSelectFields("1")
                .AddTables("t_ana_link_comuni_usl")
                .AddWhereCondition("lcu_com_codice", Comparatori.Uguale, "paz_com_comune_emigrazione", DataTypes.Join)
                .AddWhereCondition("lcu_usl_codice", Comparatori.Uguale, uslCorrente, DataTypes.Join)

                Dim subQueryEmigrazioneAslCorrente As String = .GetSelect()

                .NewQuery(False, False)

                If isQueryCountPazienti Then

                    ' Query di count dei pazienti emigrati
                    .AddTables("t_paz_pazienti")
                    .AddSelectFields("COUNT(*)")

                Else

                    ' Query di select dei pazienti emigrati
                    .IsDistinct = True

                    .AddSelectFields("paz_codice, paz_cognome, paz_nome, paz_data_nascita, paz_indirizzo_domicilio")
                    .AddSelectFields("paz_com_codice_nascita, paz_com_codice_domicilio, paz_com_comune_provenienza")
                    .AddSelectFields("paz_richiesta_certificato, paz_sta_certificato_emi, paz_data_emigrazione")
                    .AddSelectFields("paz_data_immigrazione, paz_com_codice_residenza, paz_indirizzo_residenza")
                    .AddSelectFields("paz_regolarizzato, paz_stato_notifica_emi, paz_stato_acquisizione_imi")
                    .AddSelectFields("paz_stato_anagrafico, san_descrizione stato_anagrafico")
                    .AddSelectFields("t_ana_comuni_nascita.com_descrizione nascita_com_descrizione, t_ana_comuni_nascita.com_provincia nascita_com_provincia, t_ana_comuni_nascita.com_cap nascita_com_cap")
                    .AddSelectFields("t_ana_comuni_domicilio.com_descrizione dom_com_descrizione, t_ana_comuni_domicilio.com_provincia dom_com_provincia, t_ana_comuni_domicilio.com_cap dom_com_cap")
                    .AddSelectFields("t_ana_comuni_emigrazione.com_descrizione emi_com_descrizione, t_ana_comuni_emigrazione.com_provincia emi_com_provincia, t_ana_comuni_emigrazione.com_cap emi_com_cap")
                    .AddSelectFields("t_ana_comuni_residenza.com_descrizione res_com_descrizione, t_ana_comuni_residenza.com_provincia res_com_provincia", .FC.IsNull("paz_cap_residenza", "t_ana_comuni_residenza.com_cap", DataTypes.Replace) & " res_com_cap")
                    .AddSelectFields("ugs_app_id ugs_app_id_emi")

                    .AddTables("t_paz_pazienti, t_ana_comuni t_ana_comuni_nascita, t_ana_comuni t_ana_comuni_domicilio")
                    .AddTables("t_ana_comuni t_ana_comuni_emigrazione, t_ana_comuni t_ana_comuni_residenza")
                    .AddTables("t_ana_link_comuni_usl, t_usl_gestite, t_ana_stati_anagrafici")

                    .AddWhereCondition("paz_com_comune_emigrazione", Comparatori.Uguale, "t_ana_comuni_emigrazione.com_codice", DataTypes.OutJoinLeft)
                    .AddWhereCondition("paz_com_codice_nascita", Comparatori.Uguale, "t_ana_comuni_nascita.com_codice", DataTypes.OutJoinLeft)
                    .AddWhereCondition("paz_com_codice_residenza", Comparatori.Uguale, "t_ana_comuni_residenza.com_codice", DataTypes.OutJoinLeft)
                    .AddWhereCondition("paz_com_codice_domicilio", Comparatori.Uguale, "t_ana_comuni_domicilio.com_codice", DataTypes.OutJoinLeft)

                    .AddWhereCondition("paz_com_comune_emigrazione", Comparatori.Uguale, "lcu_com_codice", DataTypes.OutJoinLeft)
                    .AddWhereCondition("lcu_usl_codice", Comparatori.Uguale, "ugs_usl_codice", DataTypes.OutJoinLeft)

                    .AddWhereCondition("paz_stato_anagrafico", Comparatori.Uguale, "san_codice", DataTypes.OutJoinLeft)
                End If

                Me.AddLoadEmigratiWhereCondition(abstractQB, emigratiFilter, subQueryEmigrazioneAslCorrente)

            End With

        End Sub

        Private Sub AddLoadEmigratiWhereCondition(abstractQB As AbstractQB, emigratiFilter As MovimentiCNSEmigratiFilter, subQueryEmigrazioneAslCorrente As String)
            '--
            With abstractQB
                '--
                .AddWhereCondition("paz_cns_codice", Comparatori.Uguale, emigratiFilter.CodiceConsultorio, DataTypes.Stringa)
                .OpenParanthesis()
                .AddWhereCondition("paz_com_comune_emigrazione", Comparatori.[IsNot], "NULL", DataTypes.Replace)
                .AddWhereCondition("paz_data_emigrazione", Comparatori.[IsNot], "NULL", DataTypes.Replace, "OR")
                .CloseParanthesis()
                '--
                ' Escludo dall'elenco i pazienti che hanno come comune di emigrazione un comune facente parte dell'asl corrente
                .AddWhereCondition("", Comparatori.NotExist, String.Format("({0})", subQueryEmigrazioneAslCorrente), DataTypes.Replace)
                '--
                If Not emigratiFilter Is Nothing Then
                    '--
                    If Not emigratiFilter.DataNascitaInizio Is Nothing Then
                        .AddWhereCondition("paz_data_nascita", Comparatori.MaggioreUguale, emigratiFilter.DataNascitaInizio, DataTypes.Data)
                    End If
                    '--
                    If Not emigratiFilter.DataNascitaFine Is Nothing Then
                        .AddWhereCondition("paz_data_nascita", Comparatori.Minore, emigratiFilter.DataNascitaFine.Value.AddDays(1), DataTypes.Data)
                    End If
                    '--
                    If Not emigratiFilter.DataEmigrazioneInizio Is Nothing OrElse Not emigratiFilter.DataEmigrazioneFine Is Nothing Then
                        .OpenParanthesis()
                        .OpenParanthesis()
                        If Not emigratiFilter.DataEmigrazioneInizio Is Nothing Then
                            .AddWhereCondition("paz_data_emigrazione", Comparatori.MaggioreUguale, emigratiFilter.DataEmigrazioneInizio, DataTypes.Data)
                        End If
                        If Not emigratiFilter.DataEmigrazioneFine Is Nothing Then
                            .AddWhereCondition("paz_data_emigrazione", Comparatori.Minore, emigratiFilter.DataEmigrazioneFine.Value.AddDays(1), DataTypes.Data)
                        End If
                        .CloseParanthesis()
                        .AddWhereCondition("paz_data_emigrazione", Comparatori.Is, "NULL", DataTypes.Replace, "OR")
                        .CloseParanthesis()
                    End If
                    '--
                    If Not emigratiFilter.CertificatoRichiesto Is Nothing Then
                        If emigratiFilter.CertificatoRichiesto Then
                            .AddWhereCondition("paz_sta_certificato_emi", Comparatori.Uguale, "S", DataTypes.Stringa)
                        Else
                            .OpenParanthesis()
                            .AddWhereCondition("paz_sta_certificato_emi", Comparatori.Uguale, "N", DataTypes.Stringa)
                            .AddWhereCondition("paz_sta_certificato_emi", Comparatori.Is, "NULL", DataTypes.Replace, "OR")
                            .CloseParanthesis()
                        End If
                    End If
                    '--
                    If Not emigratiFilter.StatiNotificaEmigrazione Is Nothing Then
                        '--
                        If emigratiFilter.StatiNotificaEmigrazione.Length > 1 Then
                            .OpenParanthesis()
                        End If
                        '--
                        For i As Integer = 0 To emigratiFilter.StatiNotificaEmigrazione.Length - 1
                            If emigratiFilter.StatiNotificaEmigrazione(i) <> Enumerators.MovimentiCNS.StatoNotificaEmigrazione.Nessuno Then
                                .AddWhereCondition("PAZ_STATO_NOTIFICA_EMI", Comparatori.Uguale, emigratiFilter.StatiNotificaEmigrazione(i).ToString("d"), DataTypes.Stringa, IIf(i > 0, "OR", "AND"))
                            Else
                                .AddWhereCondition("PAZ_STATO_NOTIFICA_EMI", Comparatori.Is, "NULL", DataTypes.Replace, IIf(i > 0, "OR", "AND"))
                            End If
                        Next
                        '--
                        If emigratiFilter.StatiNotificaEmigrazione.Length > 1 Then
                            .CloseParanthesis()
                        End If
                    End If
                    '--
                End If
                '--
            End With
            '--
        End Sub

#End Region

#End Region

        '#Region " Malattie "

        '        Public Function LoadMalattiePaziente(codicePaziente As String) As Collection(Of MovimentoCNS.MalattiaPaziente) Implements IMovimentiEsterniCNSProvider.LoadMalattiePaziente

        '            Dim malattiaPazienteCollection As New Collection(Of MovimentoCNS.MalattiaPaziente)

        '            Dim cmd As OracleClient.OracleCommand = Nothing
        '            Dim reader As IDataReader = Nothing

        '            Try
        '                cmd = New OracleClient.OracleCommand("SELECT * FROM T_PAZ_MALATTIE WHERE PMA_PAZ_CODICE = :codice", Me.Connection)

        '                cmd.Parameters.AddWithValue("codice", codicePaziente)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    Me.conditionalOpenConnection()
        '                End If

        '                reader = cmd.ExecuteReader()

        '                If Not reader Is Nothing Then

        '                    '--
        '                    Dim codicePazientePrecedenteOrdinal As Integer = reader.GetOrdinal("PMA_PAZ_CODICE_OLD")
        '                    Dim codiceMalattiaOrdinal As Integer = reader.GetOrdinal("PMA_MAL_CODICE")
        '                    Dim dataDiagnosiOrdinal As Integer = reader.GetOrdinal("PMA_DATA_DIAGNOSI")
        '                    Dim dataUltimaVisitaOrdinal As Integer = reader.GetOrdinal("PMA_DATA_ULTIMA_VISITA")
        '                    Dim followUpOrdinal As Integer = reader.GetOrdinal("PMA_FOLLOW_UP")
        '                    Dim nuovaDiagnosiOrdinal As Integer = reader.GetOrdinal("PMA_NUOVA_DIAGNOSI")
        '                    Dim numeroBilancioPartenzaOrdinal As Integer = reader.GetOrdinal("PMA_N_BILANCIO_PARTENZA")
        '                    Dim numeroMalattiaOrdinal As Integer = reader.GetOrdinal("PMA_N_MALATTIA")
        '                    '--
        '                    While reader.Read
        '                        '--
        '                        Dim malattiaPaziente As New MovimentoCNS.MalattiaPaziente
        '                        '--
        '                        If Not reader.IsDBNull(codicePazientePrecedenteOrdinal) Then malattiaPaziente.CodicePazientePrecedente = reader.GetInt32(codicePazientePrecedenteOrdinal)
        '                        '--
        '                        malattiaPaziente.Malattia = New MovimentoCNS.MalattiaInfo()
        '                        malattiaPaziente.Malattia.Codice = reader.GetString(codiceMalattiaOrdinal)
        '                        '--
        '                        If Not reader.IsDBNull(dataDiagnosiOrdinal) Then malattiaPaziente.DataDiagnosi = reader.GetDateTime(dataDiagnosiOrdinal)
        '                        If Not reader.IsDBNull(dataUltimaVisitaOrdinal) Then malattiaPaziente.DataUltimaVisita = reader.GetDateTime(dataUltimaVisitaOrdinal)
        '                        If Not reader.IsDBNull(followUpOrdinal) Then malattiaPaziente.FollowUp = reader.GetString(followUpOrdinal) = "S"
        '                        If Not reader.IsDBNull(nuovaDiagnosiOrdinal) Then malattiaPaziente.NuovaDiagnosi = reader.GetString(nuovaDiagnosiOrdinal) = "S"
        '                        If Not reader.IsDBNull(numeroBilancioPartenzaOrdinal) Then malattiaPaziente.NumeroBilancioPartenza = reader.GetInt32(numeroBilancioPartenzaOrdinal)
        '                        If Not reader.IsDBNull(numeroMalattiaOrdinal) Then malattiaPaziente.NumeroMalattia = reader.GetInt32(numeroMalattiaOrdinal)
        '                        '--
        '                        malattiaPazienteCollection.Add(malattiaPaziente)
        '                        '--
        '                    End While
        '                    '--

        '                End If

        '            Finally

        '                If Not reader Is Nothing Then reader.Close()
        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '            Return malattiaPazienteCollection

        '        End Function

        '        Public Function CountMalattiePaziente(codicePaziente As String) As Integer Implements IMovimentiEsterniCNSProvider.CountMalattiePaziente

        '            Dim cmd As OracleClient.OracleCommand = Nothing

        '            Try
        '                cmd = New OracleClient.OracleCommand("SELECT COUNT(*) FROM T_PAZ_MALATTIE WHERE PMA_PAZ_CODICE = :codice", Me.Connection)

        '                cmd.Parameters.AddWithValue("codice", codicePaziente)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    Me.conditionalOpenConnection()
        '                End If

        '                Return Convert.ToInt32(cmd.ExecuteScalar())

        '            Finally

        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '        End Function

        '        Public Sub EliminaMalattiePaziente(codicePaziente As String) Implements IMovimentiEsterniCNSProvider.EliminaMalattiePaziente

        '            Dim cmd As OracleClient.OracleCommand = Nothing

        '            Try
        '                cmd = New OracleClient.OracleCommand("DELETE FROM T_PAZ_MALATTIE WHERE PMA_PAZ_CODICE = :codice", Me.Connection)

        '                cmd.Parameters.AddWithValue("codice", codicePaziente)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    Me.conditionalOpenConnection()
        '                End If

        '                cmd.ExecuteNonQuery()

        '            Finally

        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '        End Sub

        '        Public Sub InserisciMalattiePaziente(codicePaziente As String, ByVal malattiePaziente As Collection(Of Entities.MovimentoCNS.MalattiaPaziente)) Implements IMovimentiEsterniCNSProvider.InserisciMalattiePaziente

        '            Dim cmd As OracleClient.OracleCommand = Nothing

        '            Dim ownTransaction As Boolean = False

        '            Try
        '                cmd = New OracleClient.OracleCommand("INSERT INTO T_PAZ_MALATTIE(PMA_PAZ_CODICE, PMA_PAZ_CODICE_OLD, PMA_DATA_DIAGNOSI, PMA_DATA_ULTIMA_VISITA, PMA_FOLLOW_UP, PMA_MAL_CODICE, PMA_NUOVA_DIAGNOSI, PMA_N_BILANCIO_PARTENZA, PMA_N_MALATTIA) VALUES(:codicePaziente, :codicePazientePrecedente, :dataDiagnosi, :dataUltimaVisita, :followUp, :codiceMalattia, :nuovaDiagnosi, :numeroBilancioPartenza, :numeroMalattia)", Me.Connection)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    ownTransaction = True
        '                    Me._DAM.BeginTrans()
        '                End If

        '                For Each malattiaPaziente As MovimentoCNS.MalattiaPaziente In malattiePaziente

        '                    cmd.Parameters.Clear()

        '                    cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

        '                    If Not malattiaPaziente.CodicePazientePrecedente Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codicePazientePrecedente", malattiaPaziente.CodicePazientePrecedente)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codicePazientePrecedente", DBNull.Value)
        '                    End If

        '                    cmd.Parameters.AddWithValue("codiceMalattia", malattiaPaziente.Malattia.Codice)

        '                    If Not malattiaPaziente.DataDiagnosi Is Nothing Then
        '                        cmd.Parameters.AddWithValue("dataDiagnosi", malattiaPaziente.DataDiagnosi)
        '                    Else
        '                        cmd.Parameters.AddWithValue("dataDiagnosi", DBNull.Value)
        '                    End If

        '                    If Not malattiaPaziente.DataUltimaVisita Is Nothing Then
        '                        cmd.Parameters.AddWithValue("dataUltimaVisita", malattiaPaziente.DataUltimaVisita)
        '                    Else
        '                        cmd.Parameters.AddWithValue("dataUltimaVisita", DBNull.Value)
        '                    End If

        '                    If Not malattiaPaziente.FollowUp Is Nothing Then
        '                        cmd.Parameters.AddWithValue("followUp", IIf(malattiaPaziente.FollowUp, "S", "N"))
        '                    Else
        '                        cmd.Parameters.AddWithValue("followUp", DBNull.Value)
        '                    End If

        '                    If Not malattiaPaziente.NuovaDiagnosi Is Nothing Then
        '                        cmd.Parameters.AddWithValue("nuovaDiagnosi", IIf(malattiaPaziente.NuovaDiagnosi, "S", "N"))
        '                    Else
        '                        cmd.Parameters.AddWithValue("nuovaDiagnosi", DBNull.Value)
        '                    End If

        '                    If Not malattiaPaziente.NumeroBilancioPartenza Is Nothing Then
        '                        cmd.Parameters.AddWithValue("numeroBilancioPartenza", malattiaPaziente.NumeroBilancioPartenza)
        '                    Else
        '                        cmd.Parameters.AddWithValue("numeroBilancioPartenza", DBNull.Value)
        '                    End If

        '                    If Not malattiaPaziente.NumeroMalattia Is Nothing Then
        '                        cmd.Parameters.AddWithValue("numeroMalattia", malattiaPaziente.NumeroMalattia)
        '                    Else
        '                        cmd.Parameters.AddWithValue("numeroMalattia", DBNull.Value)
        '                    End If

        '                    cmd.ExecuteNonQuery()

        '                Next

        '                If ownTransaction Then
        '                    Me.Transaction.Commit()
        '                End If

        '            Catch ex As Exception

        '                If ownTransaction Then
        '                    Me.Transaction.Rollback()
        '                End If

        '                ex.InternalPreserveStackTrace()
        '                Throw

        '            Finally

        '                If Not Me.Transaction Is Nothing AndAlso ownTransaction Then Me.Transaction.Dispose()
        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '        End Sub

        '#End Region

        '#Region " Visite "

        '        Public Function LoadVisitePaziente(codicePaziente As String) As System.Collections.ObjectModel.Collection(Of Entities.MovimentoCNS.VisitaPaziente) Implements IMovimentiEsterniCNSProvider.LoadVisitePaziente

        '            Dim visitePazienteCollection As New Collection(Of MovimentoCNS.VisitaPaziente)

        '            Dim cmd As OracleClient.OracleCommand = Nothing
        '            Dim reader As IDataReader = Nothing

        '            Try
        '                cmd = New OracleClient.OracleCommand("SELECT * FROM T_VIS_VISITE LEFT OUTER JOIN T_VIS_OSSERVAZIONI ON VIS_PAZ_CODICE = VOS_PAZ_CODICE AND VIS_N_BILANCIO = VOS_BIL_N_BILANCIO AND VIS_MAL_CODICE = VOS_MAL_CODICE WHERE VIS_PAZ_CODICE = :codicePaziente ORDER BY VIS_DATA_VISITA", Me.Connection)

        '                cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    Me.conditionalOpenConnection()
        '                End If

        '                reader = cmd.ExecuteReader()

        '                If Not reader Is Nothing Then
        '                    '--
        '                    Dim codicePazientePrecedenteVisitaOrdinal As Integer = reader.GetOrdinal("VIS_PAZ_CODICE_OLD")
        '                    Dim dataVisitaOrdinal As Integer = reader.GetOrdinal("VIS_DATA_VISITA")
        '                    Dim codiceOperatoreOrdinal As Integer = reader.GetOrdinal("VIS_OPE_CODICE")
        '                    Dim codiceConsultorioOrdinal As Integer = reader.GetOrdinal("VIS_CNS_CODICE")
        '                    Dim codiceMalattiaOrdinal As Integer = reader.GetOrdinal("VIS_MAL_CODICE")
        '                    Dim numeroBilancioOrdinal As Integer = reader.GetOrdinal("VIS_N_BILANCIO")
        '                    Dim vaccinabileOrdinal As Integer = reader.GetOrdinal("VIS_VACCINABILE")
        '                    Dim dataFineSospensioneOrdinal As Integer = reader.GetOrdinal("VIS_FINE_SOSPENSIONE")
        '                    Dim idUtenteOrdinal As Integer = reader.GetOrdinal("VIS_UTE_ID")
        '                    Dim noteOrdinal As Integer = reader.GetOrdinal("VIS_NOTE")
        '                    Dim patologiaOrdinal As Integer = reader.GetOrdinal("VIS_PATOLOGIA")
        '                    Dim dataRegistrazioneOrdinal As Integer = reader.GetOrdinal("VIS_DATA_REGISTRAZIONE")
        '                    Dim pesoOrdinal As Integer = reader.GetOrdinal("VIS_PESO")
        '                    Dim altezzaOrdinal As Integer = reader.GetOrdinal("VIS_ALTEZZA")
        '                    Dim cranioOrdinal As Integer = reader.GetOrdinal("VIS_CRANIO")
        '                    Dim percentilePesoOrdinal As Integer = reader.GetOrdinal("VIS_PERCENTILE_PESO")
        '                    Dim percentileAltezzaOrdinal As Integer = reader.GetOrdinal("VIS_PERCENTILE_ALTEZZA")
        '                    Dim percentileCranioOrdinal As Integer = reader.GetOrdinal("VIS_PERCENTILE_CRANIO")
        '                    Dim firmaOrdinal As Integer = reader.GetOrdinal("VIS_FIRMA")
        '                    Dim codiceMotivoSospensioneOrdinal As Integer = reader.GetOrdinal("VIS_MOS_CODICE")
        '                    Dim codicePazientePrecedenteOsservazioneVisitaOrdinal As Integer = reader.GetOrdinal("VOS_PAZ_CODICE_OLD")
        '                    Dim codiceOsservazioneBilancioOrdinal As Integer = reader.GetOrdinal("VOS_OSS_CODICE")
        '                    Dim codiceRispostaBilancioOrdinal As Integer = reader.GetOrdinal("VOS_RIS_CODICE")
        '                    Dim rispostaOrdinal As Integer = reader.GetOrdinal("VOS_RISPOSTA")
        '                    '--
        '                    Dim visitaPaziente As MovimentoCNS.VisitaPaziente = Nothing
        '                    '--
        '                    While reader.Read()
        '                        '--
        '                        Dim dataVisita As DateTime = reader.GetDateTime(dataVisitaOrdinal)
        '                        '--
        '                        If visitaPaziente Is Nothing OrElse visitaPaziente.DataVisita <> dataVisita Then
        '                            '--
        '                            visitaPaziente = New MovimentoCNS.VisitaPaziente()
        '                            '--
        '                            visitaPaziente.DataVisita = dataVisita
        '                            '--
        '                            If Not reader.IsDBNull(codiceMalattiaOrdinal) Then
        '                                visitaPaziente.Malattia = New MovimentoCNS.MalattiaInfo
        '                                visitaPaziente.Malattia.Codice = reader.GetString(codiceMalattiaOrdinal)
        '                            End If
        '                            '--
        '                            If Not reader.IsDBNull(numeroBilancioOrdinal) Then visitaPaziente.NumeroBilancio = reader.GetInt32(numeroBilancioOrdinal)
        '                            '--
        '                            If Not reader.IsDBNull(codicePazientePrecedenteVisitaOrdinal) Then visitaPaziente.CodicePazientePrecedente = reader.GetInt32(codicePazientePrecedenteVisitaOrdinal)
        '                            '--
        '                            If Not reader.IsDBNull(codiceOperatoreOrdinal) Then
        '                                visitaPaziente.Operatore = New MovimentoCNS.OperatoreInfo()
        '                                visitaPaziente.Operatore.Codice = reader.GetString(codiceOperatoreOrdinal)
        '                            End If
        '                            If Not reader.IsDBNull(codiceConsultorioOrdinal) Then
        '                                visitaPaziente.Consultorio = New MovimentoCNS.ConsultorioInfo()
        '                                visitaPaziente.Consultorio.Codice = reader.GetString(codiceConsultorioOrdinal)
        '                            End If
        '                            If Not reader.IsDBNull(vaccinabileOrdinal) Then visitaPaziente.Vaccinabile = reader.GetString(vaccinabileOrdinal) = "S"
        '                            If Not reader.IsDBNull(dataFineSospensioneOrdinal) Then visitaPaziente.DataFineSospensione = reader.GetDateTime(dataFineSospensioneOrdinal)
        '                            If Not reader.IsDBNull(idUtenteOrdinal) Then
        '                                visitaPaziente.Utente = New MovimentoCNS.UtenteInfo
        '                                visitaPaziente.Utente.ID = reader.GetInt32(idUtenteOrdinal)
        '                            End If
        '                            If Not reader.IsDBNull(noteOrdinal) Then visitaPaziente.Note = reader.GetString(noteOrdinal)
        '                            If Not reader.IsDBNull(patologiaOrdinal) Then visitaPaziente.Patologia = reader.GetString(patologiaOrdinal) = "S"
        '                            If Not reader.IsDBNull(dataRegistrazioneOrdinal) Then visitaPaziente.DataRegistrazione = reader.GetDateTime(dataRegistrazioneOrdinal)
        '                            If Not reader.IsDBNull(pesoOrdinal) Then visitaPaziente.Peso = reader.GetDouble(pesoOrdinal)
        '                            If Not reader.IsDBNull(altezzaOrdinal) Then visitaPaziente.Altezza = reader.GetDouble(altezzaOrdinal)
        '                            If Not reader.IsDBNull(cranioOrdinal) Then visitaPaziente.Cranio = reader.GetDouble(cranioOrdinal)
        '                            If Not reader.IsDBNull(percentilePesoOrdinal) Then visitaPaziente.PercentilePeso = reader.GetString(percentilePesoOrdinal)
        '                            If Not reader.IsDBNull(percentileAltezzaOrdinal) Then visitaPaziente.PercentileAltezza = reader.GetString(percentileAltezzaOrdinal)
        '                            If Not reader.IsDBNull(percentileCranioOrdinal) Then visitaPaziente.PercentileCranio = reader.GetString(percentileCranioOrdinal)
        '                            If Not reader.IsDBNull(firmaOrdinal) Then visitaPaziente.Firma = reader.GetString(firmaOrdinal)
        '                            If Not reader.IsDBNull(codiceMotivoSospensioneOrdinal) Then
        '                                visitaPaziente.MotivoSospensione = New MovimentoCNS.MotivoSospensioneInfo
        '                                visitaPaziente.MotivoSospensione.Codice = reader.GetString(codiceMotivoSospensioneOrdinal)
        '                            End If
        '                            '--
        '                            visitePazienteCollection.Add(visitaPaziente)
        '                            '--
        '                        End If
        '                        '--
        '                        If Not reader.IsDBNull(codiceOsservazioneBilancioOrdinal) Then
        '                            '--
        '                            Dim osservazioneVisitaPaziente As New MovimentoCNS.OsservazioneVisitaPaziente
        '                            '--
        '                            If Not reader.IsDBNull(codicePazientePrecedenteOsservazioneVisitaOrdinal) Then osservazioneVisitaPaziente.CodicePazientePrecedente = reader.GetInt32(codicePazientePrecedenteOsservazioneVisitaOrdinal)
        '                            '--
        '                            osservazioneVisitaPaziente.OsservazioneBilancio = New MovimentoCNS.OsservazioneBilancioInfo
        '                            osservazioneVisitaPaziente.OsservazioneBilancio.Codice = reader.GetString(codiceOsservazioneBilancioOrdinal)
        '                            '--
        '                            If Not reader.IsDBNull(codiceRispostaBilancioOrdinal) Then
        '                                osservazioneVisitaPaziente.RispostaBilancio = New MovimentoCNS.RispostaBilancioInfo
        '                                osservazioneVisitaPaziente.RispostaBilancio.Codice = reader.GetString(codiceRispostaBilancioOrdinal)
        '                            End If
        '                            '--
        '                            If Not reader.IsDBNull(codiceOsservazioneBilancioOrdinal) Then
        '                                osservazioneVisitaPaziente.OsservazioneBilancio = New MovimentoCNS.OsservazioneBilancioInfo
        '                                osservazioneVisitaPaziente.OsservazioneBilancio.Codice = reader.GetString(codiceOsservazioneBilancioOrdinal)
        '                            End If
        '                            '--
        '                            If Not reader.IsDBNull(rispostaOrdinal) Then osservazioneVisitaPaziente.Risposta = reader.GetString(rispostaOrdinal)
        '                            '--
        '                            visitaPaziente.Osservazioni.Add(osservazioneVisitaPaziente)
        '                            '--
        '                        End If
        '                        '--
        '                    End While
        '                    '--

        '                End If

        '            Finally

        '                If Not reader Is Nothing Then reader.Close()
        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '            Return visitePazienteCollection

        '        End Function

        '        Public Function CountVisitePaziente(codicePaziente As String) As Integer Implements IMovimentiEsterniCNSProvider.CountVisitePaziente

        '            Dim cmd As OracleClient.OracleCommand = Nothing
        '            Dim reader As IDataReader = Nothing

        '            Try
        '                cmd = New OracleClient.OracleCommand("SELECT COUNT(*) FROM T_VIS_VISITE WHERE VIS_PAZ_CODICE = :codicePaziente", Me.Connection)

        '                cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    Me.conditionalOpenConnection()
        '                End If

        '                Return Convert.ToInt32(cmd.ExecuteScalar())

        '            Finally

        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '        End Function

        '        Public Sub EliminaVisitePaziente(codicePaziente As String) Implements IMovimentiEsterniCNSProvider.EliminaVisitePaziente

        '            Dim cmd As OracleClient.OracleCommand = Nothing

        '            Dim ownTransaction As Boolean = False

        '            Try
        '                cmd = New OracleClient.OracleCommand("DELETE FROM T_VIS_VISITE WHERE VIS_PAZ_CODICE = :codice", Me.Connection)

        '                cmd.Parameters.AddWithValue("codice", codicePaziente)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    ownTransaction = True
        '                    Me._DAM.BeginTrans()
        '                End If

        '                cmd.ExecuteNonQuery()

        '                cmd.CommandText = "DELETE FROM T_VIS_OSSERVAZIONI WHERE VOS_PAZ_CODICE = :codice"

        '                cmd.ExecuteNonQuery()

        '                If ownTransaction Then
        '                    Me.Transaction.Commit()
        '                End If

        '            Catch ex As Exception

        '                If ownTransaction Then
        '                    Me.Transaction.Rollback()
        '                End If

        '                ex.InternalPreserveStackTrace()
        '                Throw

        '            Finally

        '                If Not Me.Transaction Is Nothing AndAlso ownTransaction Then Me.Transaction.Dispose()
        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '        End Sub

        '        Public Sub InserisciVisitePaziente(codicePaziente As String, ByVal visitePaziente As System.Collections.ObjectModel.Collection(Of Entities.MovimentoCNS.VisitaPaziente)) Implements IMovimentiEsterniCNSProvider.InserisciVisitePaziente

        '            Dim cmd As OracleClient.OracleCommand = Nothing

        '            Dim ownTransaction As Boolean = False

        '            Try
        '                cmd = New OracleClient.OracleCommand()

        '                cmd.Connection = Me.Connection

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    ownTransaction = True
        '                    Me._DAM.BeginTrans()
        '                End If

        '                For Each visitaPaziente As MovimentoCNS.VisitaPaziente In visitePaziente

        '                    cmd.CommandText = "INSERT INTO T_VIS_VISITE(VIS_PAZ_CODICE, VIS_PAZ_CODICE_OLD, VIS_DATA_VISITA, VIS_OPE_CODICE, VIS_CNS_CODICE, VIS_MAL_CODICE, VIS_N_BILANCIO, VIS_VACCINABILE, VIS_FINE_SOSPENSIONE, VIS_UTE_ID, VIS_NOTE, VIS_PATOLOGIA, VIS_DATA_REGISTRAZIONE, VIS_PESO, VIS_ALTEZZA, VIS_CRANIO, VIS_PERCENTILE_PESO, VIS_PERCENTILE_ALTEZZA, VIS_PERCENTILE_CRANIO, VIS_FIRMA, VIS_MOS_CODICE) VALUES(:codicePaziente, :codicePazientePrecedente, :dataVisita, :codiceOperatore, :codiceConsultorio, :codiceMalattia, :numeroBilancio, :vaccinabile, :fineSospensione, :idUtente, :note, :patologia, :dataRegistrazione, :peso, :altezza, :cranio, :percentilePeso, :percentileAltezza, :percentileCranio, :firma, :codiceMotivoSospensione)"

        '                    cmd.Parameters.Clear()

        '                    cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

        '                    If Not visitaPaziente.DataVisita Is Nothing Then
        '                        cmd.Parameters.AddWithValue("dataVisita", visitaPaziente.DataVisita)
        '                    Else
        '                        cmd.Parameters.AddWithValue("dataVisita", DBNull.Value)
        '                    End If

        '                    If Not visitaPaziente.CodicePazientePrecedente Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codicePazientePrecedente", visitaPaziente.CodicePazientePrecedente)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codicePazientePrecedente", DBNull.Value)
        '                    End If

        '                    If Not visitaPaziente.Operatore Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codiceOperatore", visitaPaziente.Operatore.Codice)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codiceOperatore", DBNull.Value)
        '                    End If

        '                    If Not visitaPaziente.Consultorio Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codiceConsultorio", visitaPaziente.Consultorio.Codice)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codiceConsultorio", DBNull.Value)
        '                    End If

        '                    If Not visitaPaziente.Malattia Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codiceMalattia", visitaPaziente.Malattia.Codice)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codiceMalattia", DBNull.Value)
        '                    End If

        '                    If Not visitaPaziente.NumeroBilancio Is Nothing Then
        '                        cmd.Parameters.AddWithValue("numeroBilancio", visitaPaziente.NumeroBilancio)
        '                    Else
        '                        cmd.Parameters.AddWithValue("numeroBilancio", DBNull.Value)
        '                    End If

        '                    If Not visitaPaziente.Vaccinabile Is Nothing Then
        '                        cmd.Parameters.AddWithValue("vaccinabile", IIf(visitaPaziente.Vaccinabile, "S", "N"))
        '                    Else
        '                        cmd.Parameters.AddWithValue("vaccinabile", DBNull.Value)
        '                    End If

        '                    If Not visitaPaziente.DataFineSospensione Is Nothing Then
        '                        cmd.Parameters.AddWithValue("fineSospensione", visitaPaziente.DataFineSospensione)
        '                    Else
        '                        cmd.Parameters.AddWithValue("fineSospensione", DBNull.Value)
        '                    End If

        '                    If Not visitaPaziente.Utente Is Nothing Then
        '                        cmd.Parameters.AddWithValue("idUtente", visitaPaziente.Utente.ID)
        '                    Else
        '                        cmd.Parameters.AddWithValue("idUtente", DBNull.Value)
        '                    End If

        '                    If Not visitaPaziente.Note Is Nothing Then
        '                        cmd.Parameters.AddWithValue("note", visitaPaziente.Note)
        '                    Else
        '                        cmd.Parameters.AddWithValue("note", DBNull.Value)
        '                    End If

        '                    If Not visitaPaziente.Patologia Is Nothing Then
        '                        cmd.Parameters.AddWithValue("patologia", IIf(visitaPaziente.Patologia, "S", "N"))
        '                    Else
        '                        cmd.Parameters.AddWithValue("patologia", DBNull.Value)
        '                    End If

        '                    If Not visitaPaziente.DataRegistrazione Is Nothing Then
        '                        cmd.Parameters.AddWithValue("dataRegistrazione", visitaPaziente.DataRegistrazione)
        '                    Else
        '                        cmd.Parameters.AddWithValue("dataRegistrazione", DBNull.Value)
        '                    End If

        '                    If Not visitaPaziente.Peso Is Nothing Then
        '                        cmd.Parameters.AddWithValue("peso", visitaPaziente.Peso)
        '                    Else
        '                        cmd.Parameters.AddWithValue("peso", DBNull.Value)
        '                    End If

        '                    If Not visitaPaziente.Altezza Is Nothing Then
        '                        cmd.Parameters.AddWithValue("altezza", visitaPaziente.Altezza)
        '                    Else
        '                        cmd.Parameters.AddWithValue("altezza", DBNull.Value)
        '                    End If

        '                    If Not visitaPaziente.Cranio Is Nothing Then
        '                        cmd.Parameters.AddWithValue("cranio", visitaPaziente.Cranio)
        '                    Else
        '                        cmd.Parameters.AddWithValue("cranio", DBNull.Value)
        '                    End If

        '                    If Not visitaPaziente.PercentilePeso Is Nothing Then
        '                        cmd.Parameters.AddWithValue("percentilePeso", visitaPaziente.PercentilePeso)
        '                    Else
        '                        cmd.Parameters.AddWithValue("percentilePeso", DBNull.Value)
        '                    End If

        '                    If Not visitaPaziente.PercentileAltezza Is Nothing Then
        '                        cmd.Parameters.AddWithValue("percentileAltezza", visitaPaziente.PercentileAltezza)
        '                    Else
        '                        cmd.Parameters.AddWithValue("percentileAltezza", DBNull.Value)
        '                    End If

        '                    If Not visitaPaziente.PercentileCranio Is Nothing Then
        '                        cmd.Parameters.AddWithValue("percentileCranio", visitaPaziente.PercentileCranio)
        '                    Else
        '                        cmd.Parameters.AddWithValue("percentileCranio", DBNull.Value)
        '                    End If

        '                    If Not visitaPaziente.Firma Is Nothing Then
        '                        cmd.Parameters.AddWithValue("firma", visitaPaziente.Firma)
        '                    Else
        '                        cmd.Parameters.AddWithValue("firma", DBNull.Value)
        '                    End If

        '                    If Not visitaPaziente.MotivoSospensione Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codiceMotivoSospensione", visitaPaziente.MotivoSospensione.Codice)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codiceMotivoSospensione", DBNull.Value)
        '                    End If

        '                    cmd.ExecuteNonQuery()

        '                    For Each osservazioneVisitaPaziente As MovimentoCNS.OsservazioneVisitaPaziente In visitaPaziente.Osservazioni

        '                        cmd.CommandText = "INSERT INTO T_VIS_OSSERVAZIONI(VOS_PAZ_CODICE, VOS_PAZ_CODICE_OLD, VOS_BIL_N_BILANCIO, VOS_OSS_CODICE, VOS_MAL_CODICE, VOS_RIS_CODICE, VOS_DATA_VISITA, VOS_RISPOSTA) VALUES(:codicePaziente, :codicePazientePrecedente, :numeroBilancio, :codiceOsservazione, :codiceMalattia, :codiceRisposta, :dataVisita, :risposta)"

        '                        cmd.Parameters.Clear()

        '                        cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

        '                        If Not osservazioneVisitaPaziente.CodicePazientePrecedente Is Nothing Then
        '                            cmd.Parameters.AddWithValue("codicePazientePrecedente", osservazioneVisitaPaziente.CodicePazientePrecedente)
        '                        Else
        '                            cmd.Parameters.AddWithValue("codicePazientePrecedente", DBNull.Value)
        '                        End If

        '                        If Not visitaPaziente.NumeroBilancio Is Nothing Then
        '                            cmd.Parameters.AddWithValue("numeroBilancio", visitaPaziente.NumeroBilancio)
        '                        Else
        '                            cmd.Parameters.AddWithValue("numeroBilancio", DBNull.Value)
        '                        End If

        '                        If Not visitaPaziente.DataVisita Is Nothing Then
        '                            cmd.Parameters.AddWithValue("dataVisita", visitaPaziente.DataVisita)
        '                        Else
        '                            cmd.Parameters.AddWithValue("dataVisita", DBNull.Value)
        '                        End If

        '                        If Not visitaPaziente.Malattia Is Nothing Then
        '                            cmd.Parameters.AddWithValue("codiceMalattia", visitaPaziente.Malattia.Codice)
        '                        Else
        '                            cmd.Parameters.AddWithValue("codiceMalattia", DBNull.Value)
        '                        End If

        '                        If Not osservazioneVisitaPaziente.OsservazioneBilancio.Codice Is Nothing Then
        '                            cmd.Parameters.AddWithValue("codiceOsservazione", osservazioneVisitaPaziente.OsservazioneBilancio.Codice)
        '                        Else
        '                            cmd.Parameters.AddWithValue("codiceOsservazione", DBNull.Value)
        '                        End If

        '                        If Not osservazioneVisitaPaziente.RispostaBilancio Is Nothing Then
        '                            cmd.Parameters.AddWithValue("codiceRisposta", osservazioneVisitaPaziente.RispostaBilancio.Codice)
        '                        Else
        '                            cmd.Parameters.AddWithValue("codiceRisposta", DBNull.Value)
        '                        End If

        '                        If Not osservazioneVisitaPaziente.Risposta Is Nothing Then
        '                            cmd.Parameters.AddWithValue("risposta", osservazioneVisitaPaziente.Risposta)
        '                        Else
        '                            cmd.Parameters.AddWithValue("risposta", DBNull.Value)
        '                        End If

        '                        cmd.ExecuteNonQuery()

        '                    Next

        '                Next

        '                If ownTransaction Then
        '                    Me.Transaction.Commit()
        '                End If

        '            Catch ex As Exception

        '                If ownTransaction Then
        '                    Me.Transaction.Rollback()
        '                End If

        '                ex.InternalPreserveStackTrace()
        '                Throw

        '            Finally

        '                If Not Me.Transaction Is Nothing AndAlso ownTransaction Then Me.Transaction.Dispose()
        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try


        '        End Sub

        '#End Region

        '#Region " VaccinazioniEscluse "

        '        Public Function LoadVaccinazioniEsclusePaziente(codicePaziente As String) As System.Collections.ObjectModel.Collection(Of Entities.MovimentoCNS.VaccinazioneEsclusaPaziente) Implements IMovimentiEsterniCNSProvider.LoadVaccinazioniEsclusePaziente

        '            Dim vaccinazioniEsclusePazienteCollection As New Collection(Of MovimentoCNS.VaccinazioneEsclusaPaziente)

        '            Dim cmd As OracleClient.OracleCommand = Nothing
        '            Dim reader As IDataReader = Nothing

        '            Try
        '                cmd = New OracleClient.OracleCommand("SELECT * FROM T_VAC_ESCLUSE WHERE VEX_PAZ_CODICE = :codicePaziente", Me.Connection)

        '                cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    Me.conditionalOpenConnection()
        '                End If

        '                reader = cmd.ExecuteReader()

        '                If Not reader Is Nothing Then

        '                    '--
        '                    Dim codicePazientePrecedenteOrdinal As Integer = reader.GetOrdinal("VEX_PAZ_CODICE_OLD")
        '                    Dim codiceVaccinazioneOrdinal As Integer = reader.GetOrdinal("VEX_VAC_CODICE")
        '                    Dim dataVisitaOrdinal As Integer = reader.GetOrdinal("VEX_DATA_VISITA")
        '                    Dim dataScadenzaOrdinal As Integer = reader.GetOrdinal("VEX_DATA_SCADENZA")
        '                    Dim codiceMotivoEsclusioneOrdinal As Integer = reader.GetOrdinal("VEX_MOE_CODICE")
        '                    Dim codiceOperatoreOrdinal As Integer = reader.GetOrdinal("VEX_OPE_CODICE")
        '                    '--
        '                    While reader.Read
        '                        '--
        '                        Dim vaccinazioneEsclusaPaziente As New MovimentoCNS.VaccinazioneEsclusaPaziente
        '                        '--
        '                        If Not reader.IsDBNull(codicePazientePrecedenteOrdinal) Then vaccinazioneEsclusaPaziente.CodicePazientePrecedente = reader.GetInt32(codicePazientePrecedenteOrdinal)
        '                        '--
        '                        vaccinazioneEsclusaPaziente.Vaccinazione = New MovimentoCNS.VaccinazioneInfo
        '                        vaccinazioneEsclusaPaziente.Vaccinazione.Codice = reader.GetString(codiceVaccinazioneOrdinal)
        '                        '--
        '                        If Not reader.IsDBNull(dataVisitaOrdinal) Then vaccinazioneEsclusaPaziente.DataVisita = reader.GetDateTime(dataVisitaOrdinal)
        '                        If Not reader.IsDBNull(dataScadenzaOrdinal) Then vaccinazioneEsclusaPaziente.DataScadenza = reader.GetDateTime(dataScadenzaOrdinal)
        '                        If Not reader.IsDBNull(codiceMotivoEsclusioneOrdinal) Then
        '                            vaccinazioneEsclusaPaziente.MotivoEsclusione = New MovimentoCNS.MotivoEsclusioneInfo
        '                            vaccinazioneEsclusaPaziente.MotivoEsclusione.Codice = reader.GetString(codiceMotivoEsclusioneOrdinal)
        '                        End If
        '                        If Not reader.IsDBNull(codiceOperatoreOrdinal) Then
        '                            vaccinazioneEsclusaPaziente.Operatore = New MovimentoCNS.OperatoreInfo
        '                            vaccinazioneEsclusaPaziente.Operatore.Codice = reader.GetString(codiceOperatoreOrdinal)
        '                        End If
        '                        '--
        '                        vaccinazioniEsclusePazienteCollection.Add(vaccinazioneEsclusaPaziente)
        '                        '--
        '                    End While
        '                    '--

        '                End If

        '            Finally

        '                If Not reader Is Nothing Then reader.Close()
        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '            Return vaccinazioniEsclusePazienteCollection

        '        End Function

        '        Public Function CountVaccinazioniEsclusePaziente(codicePaziente As String) As Integer Implements IMovimentiEsterniCNSProvider.CountVaccinazioniEsclusePaziente

        '            Dim cmd As OracleClient.OracleCommand = Nothing

        '            Try
        '                cmd = New OracleClient.OracleCommand("SELECT COUNT(*) FROM T_VAC_ESCLUSE WHERE VEX_PAZ_CODICE = :codicePaziente", Me.Connection)

        '                cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    Me.conditionalOpenConnection()
        '                End If

        '                Return Convert.ToInt32(cmd.ExecuteScalar())

        '            Finally

        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '        End Function

        '        Public Sub EliminaVaccinazioniEsclusePaziente(codicePaziente As String) Implements IMovimentiEsterniCNSProvider.EliminaVaccinazioniEsclusePaziente

        '            Dim cmd As OracleClient.OracleCommand = Nothing

        '            Try
        '                cmd = New OracleClient.OracleCommand("DELETE FROM T_VAC_ESCLUSE WHERE VEX_PAZ_CODICE = :codicePaziente", Me.Connection)

        '                cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    Me.conditionalOpenConnection()
        '                End If

        '                cmd.ExecuteNonQuery()

        '            Finally

        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '        End Sub

        '        Public Sub InserisciVaccinazioniEsclusePaziente(codicePaziente As String, vaccinazioniEsclusePaziente As System.Collections.ObjectModel.Collection(Of Entities.MovimentoCNS.VaccinazioneEsclusaPaziente)) Implements IMovimentiEsterniCNSProvider.InserisciVaccinazioniEsclusePaziente

        '            Dim cmd As OracleClient.OracleCommand = Nothing

        '            Dim ownTransaction As Boolean = False

        '            Try
        '                cmd = New OracleClient.OracleCommand("INSERT INTO T_VAC_ESCLUSE(VEX_PAZ_CODICE, VEX_PAZ_CODICE_OLD, VEX_VAC_CODICE, VEX_DATA_VISITA, VEX_DATA_SCADENZA, VEX_MOE_CODICE, VEX_OPE_CODICE) VALUES(:codicePaziente, :codicePazientePrecedente, :codiceVaccinazione, :dataVisita, :dataScadenza, :codiceMotivoEsclusione, :codiceOperatore)", Me.Connection)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    ownTransaction = True
        '                    Me._DAM.BeginTrans()
        '                End If

        '                For Each vaccinazioneEsclusaPaziente As MovimentoCNS.VaccinazioneEsclusaPaziente In vaccinazioniEsclusePaziente

        '                    cmd.Parameters.Clear()

        '                    cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

        '                    If Not vaccinazioneEsclusaPaziente.CodicePazientePrecedente Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codicePazientePrecedente", vaccinazioneEsclusaPaziente.CodicePazientePrecedente)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codicePazientePrecedente", DBNull.Value)
        '                    End If

        '                    cmd.Parameters.AddWithValue("codiceVaccinazione", vaccinazioneEsclusaPaziente.Vaccinazione.Codice)

        '                    If Not vaccinazioneEsclusaPaziente.DataVisita Is Nothing Then
        '                        cmd.Parameters.AddWithValue("dataVisita", vaccinazioneEsclusaPaziente.DataVisita)
        '                    Else
        '                        cmd.Parameters.AddWithValue("dataVisita", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEsclusaPaziente.DataScadenza Is Nothing Then
        '                        cmd.Parameters.AddWithValue("dataScadenza", vaccinazioneEsclusaPaziente.DataScadenza)
        '                    Else
        '                        cmd.Parameters.AddWithValue("dataScadenza", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEsclusaPaziente.MotivoEsclusione Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codiceMotivoEsclusione", vaccinazioneEsclusaPaziente.MotivoEsclusione.Codice)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codiceMotivoEsclusione", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEsclusaPaziente.Operatore Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codiceOperatore", vaccinazioneEsclusaPaziente.Operatore.Codice)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codiceOperatore", DBNull.Value)
        '                    End If

        '                    cmd.ExecuteNonQuery()

        '                Next

        '                If ownTransaction Then
        '                    Me.Transaction.Commit()
        '                End If

        '            Catch ex As Exception

        '                If ownTransaction Then
        '                    Me.Transaction.Rollback()
        '                End If

        '                ex.InternalPreserveStackTrace()
        '                Throw

        '            Finally

        '                If Not Me.Transaction Is Nothing AndAlso ownTransaction Then Me.Transaction.Dispose()
        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '        End Sub

        '#End Region

        '#Region " Rifiuti "

        '        Public Function LoadRifiutiPaziente(codicePaziente As String) As System.Collections.ObjectModel.Collection(Of Entities.MovimentoCNS.RifiutoPaziente) Implements IMovimentiEsterniCNSProvider.LoadRifiutiPaziente

        '            Dim rifiutiPazienteCollection As New Collection(Of MovimentoCNS.RifiutoPaziente)

        '            Dim cmd As OracleClient.OracleCommand = Nothing
        '            Dim reader As IDataReader = Nothing

        '            Try
        '                cmd = New OracleClient.OracleCommand("SELECT * FROM T_PAZ_RIFIUTI WHERE PRF_PAZ_CODICE = :codicePaziente", Me.Connection)

        '                cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    Me.conditionalOpenConnection()
        '                End If

        '                reader = cmd.ExecuteReader()

        '                If Not reader Is Nothing Then

        '                    '--
        '                    Dim codicePazientePrecedenteOrdinal As Integer = reader.GetOrdinal("PRF_PAZ_CODICE_OLD")
        '                    Dim codiceVaccinazioneOrdinal As Integer = reader.GetOrdinal("PRF_VAC_CODICE")
        '                    Dim dataOrdinal As Integer = reader.GetOrdinal("PRF_DATA_RIFIUTO")
        '                    Dim noteOrdinal As Integer = reader.GetOrdinal("PRF_NOTE_RIFIUTO")
        '                    Dim idUtenteOrdinal As Integer = reader.GetOrdinal("PRF_UTE_ID")
        '                    Dim genitoreOrdinal As Integer = reader.GetOrdinal("PRF_GENITORE")
        '                    Dim numeroRichiamoOrdinal As Integer = reader.GetOrdinal("PRF_N_RICHIAMO")
        '                    '--
        '                    While reader.Read
        '                        '--
        '                        Dim rifiutoPaziente As New MovimentoCNS.RifiutoPaziente
        '                        '--
        '                        If Not reader.IsDBNull(codicePazientePrecedenteOrdinal) Then rifiutoPaziente.CodicePazientePrecedente = reader.GetInt32(codicePazientePrecedenteOrdinal)
        '                        '--
        '                        rifiutoPaziente.Vaccinazione = New MovimentoCNS.VaccinazioneInfo
        '                        rifiutoPaziente.Vaccinazione.Codice = reader.GetString(codiceVaccinazioneOrdinal)
        '                        '--
        '                        If Not reader.IsDBNull(dataOrdinal) Then rifiutoPaziente.DataRifiuto = reader.GetDateTime(dataOrdinal)
        '                        If Not reader.IsDBNull(noteOrdinal) Then rifiutoPaziente.Note = reader.GetString(noteOrdinal)
        '                        If Not reader.IsDBNull(idUtenteOrdinal) Then
        '                            rifiutoPaziente.Utente = New MovimentoCNS.UtenteInfo
        '                            rifiutoPaziente.Utente.ID = reader.GetInt32(idUtenteOrdinal)
        '                        End If
        '                        If Not reader.IsDBNull(genitoreOrdinal) Then rifiutoPaziente.Genitore = reader.GetString(genitoreOrdinal)
        '                        If Not reader.IsDBNull(numeroRichiamoOrdinal) Then rifiutoPaziente.NumeroRichiamo = reader.GetInt32(numeroRichiamoOrdinal)
        '                        '--
        '                        rifiutiPazienteCollection.Add(rifiutoPaziente)
        '                        '--
        '                    End While
        '                    '--

        '                End If

        '            Finally

        '                If Not reader Is Nothing Then reader.Close()
        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '            Return rifiutiPazienteCollection

        '        End Function

        '        Public Function CountRifiutiPaziente(codicePaziente As String) As Integer Implements IMovimentiEsterniCNSProvider.CountRifiutiPaziente

        '            Dim cmd As OracleClient.OracleCommand = Nothing

        '            Try
        '                cmd = New OracleClient.OracleCommand("SELECT COUNT(*) FROM T_PAZ_RIFIUTI WHERE PRF_PAZ_CODICE = :codicePaziente", Me.Connection)

        '                cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    Me.conditionalOpenConnection()
        '                End If

        '                Return Convert.ToInt32(cmd.ExecuteScalar())

        '            Finally

        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '        End Function

        '        Public Sub EliminaRifiutiPaziente(codicePaziente As String) Implements IMovimentiEsterniCNSProvider.EliminaRifiutiPaziente

        '            Dim cmd As OracleClient.OracleCommand = Nothing

        '            Try
        '                cmd = New OracleClient.OracleCommand("DELETE FROM T_PAZ_RIFIUTI WHERE PRF_PAZ_CODICE = :codicePaziente", Me.Connection)

        '                cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    Me.conditionalOpenConnection()
        '                End If

        '                cmd.ExecuteNonQuery()

        '            Finally

        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '        End Sub

        '        Public Sub InserisciRifiutiPaziente(codicePaziente As String, rifiutiPaziente As System.Collections.ObjectModel.Collection(Of Entities.MovimentoCNS.RifiutoPaziente)) Implements IMovimentiEsterniCNSProvider.InserisciRifiutiPaziente

        '            Dim cmd As OracleClient.OracleCommand = Nothing

        '            Dim ownTransaction As Boolean = False

        '            Try
        '                cmd = New OracleClient.OracleCommand("INSERT INTO T_PAZ_RIFIUTI(PRF_PAZ_CODICE, PRF_PAZ_CODICE_OLD, PRF_VAC_CODICE, PRF_DATA_RIFIUTO, PRF_NOTE_RIFIUTO, PRF_UTE_ID, PRF_GENITORE, PRF_N_RICHIAMO) VALUES(:codicePaziente, :codicePazientePrecedente, :codiceVaccinazione, :dataRifiuto, :note, :idUtente, :genitore, :numeroRichiamo)", Me.Connection)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    ownTransaction = True
        '                    Me._DAM.BeginTrans()
        '                End If

        '                For Each rifiutoPaziente As MovimentoCNS.RifiutoPaziente In rifiutiPaziente

        '                    cmd.Parameters.Clear()

        '                    cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

        '                    If Not rifiutoPaziente.CodicePazientePrecedente Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codicePazientePrecedente", rifiutoPaziente.CodicePazientePrecedente)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codicePazientePrecedente", DBNull.Value)
        '                    End If

        '                    cmd.Parameters.AddWithValue("codiceVaccinazione", rifiutoPaziente.Vaccinazione.Codice)

        '                    If Not rifiutoPaziente.DataRifiuto Is Nothing Then
        '                        cmd.Parameters.AddWithValue("dataRifiuto", rifiutoPaziente.DataRifiuto)
        '                    Else
        '                        cmd.Parameters.AddWithValue("dataRifiuto", DBNull.Value)
        '                    End If

        '                    If Not rifiutoPaziente.Note Is Nothing Then
        '                        cmd.Parameters.AddWithValue("note", rifiutoPaziente.Note)
        '                    Else
        '                        cmd.Parameters.AddWithValue("note", DBNull.Value)
        '                    End If

        '                    If Not rifiutoPaziente.Utente Is Nothing Then
        '                        cmd.Parameters.AddWithValue("idUtente", rifiutoPaziente.Utente.ID)
        '                    Else
        '                        cmd.Parameters.AddWithValue("idUtente", DBNull.Value)
        '                    End If

        '                    If Not rifiutoPaziente.Genitore Is Nothing Then
        '                        cmd.Parameters.AddWithValue("genitore", rifiutoPaziente.Genitore)
        '                    Else
        '                        cmd.Parameters.AddWithValue("genitore", DBNull.Value)
        '                    End If

        '                    If Not rifiutoPaziente.NumeroRichiamo Is Nothing Then
        '                        cmd.Parameters.AddWithValue("numeroRichiamo", rifiutoPaziente.NumeroRichiamo)
        '                    Else
        '                        cmd.Parameters.AddWithValue("numeroRichiamo", DBNull.Value)
        '                    End If

        '                    cmd.ExecuteNonQuery()

        '                Next

        '                If ownTransaction Then
        '                    Me.Transaction.Commit()
        '                End If

        '            Catch ex As Exception

        '                If ownTransaction Then
        '                    Me.Transaction.Rollback()
        '                End If

        '                ex.InternalPreserveStackTrace()
        '                Throw

        '            Finally

        '                If Not Me.Transaction Is Nothing AndAlso ownTransaction Then Me.Transaction.Dispose()
        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '        End Sub

        '#End Region

        '#Region " Inadempienze "

        '        Public Function LoadInadempienzePaziente(codicePaziente As String) As System.Collections.ObjectModel.Collection(Of Entities.MovimentoCNS.InadempienzaPaziente) Implements IMovimentiEsterniCNSProvider.LoadInadempienzePaziente

        '            Dim inadempienzaPazienteCollection As New Collection(Of MovimentoCNS.InadempienzaPaziente)

        '            Dim cmd As OracleClient.OracleCommand = Nothing
        '            Dim reader As IDataReader = Nothing

        '            Try
        '                cmd = New OracleClient.OracleCommand("SELECT * FROM T_PAZ_INADEMPIENZE WHERE PIN_PAZ_CODICE = :codicePaziente", Me.Connection)

        '                cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    Me.conditionalOpenConnection()
        '                End If

        '                reader = cmd.ExecuteReader()

        '                If Not reader Is Nothing Then

        '                    '--
        '                    Dim codicePazientePrecedenteOrdinal As Integer = reader.GetOrdinal("PIN_PAZ_CODICE_OLD")
        '                    Dim codiceVaccinazioneOrdinal As Integer = reader.GetOrdinal("PIN_VAC_CODICE")
        '                    Dim stampatoOrdinal As Integer = reader.GetOrdinal("PIN_STAMPATO")
        '                    Dim statoOrdinal As Integer = reader.GetOrdinal("PIN_STATO")
        '                    Dim dataOrdinal As Integer = reader.GetOrdinal("PIN_DATA")
        '                    Dim idUtenteOrdinal As Integer = reader.GetOrdinal("PIN_UTE_ID")
        '                    Dim dataAppuntamento1Ordinal As Integer = reader.GetOrdinal("PIN_PRI_DATA_APPUNTAMENTO1")
        '                    Dim dataAppuntamento2Ordinal As Integer = reader.GetOrdinal("PIN_PRI_DATA_APPUNTAMENTO2")
        '                    Dim dataAppuntamento3Ordinal As Integer = reader.GetOrdinal("PIN_PRI_DATA_APPUNTAMENTO3")
        '                    Dim dataAppuntamento4Ordinal As Integer = reader.GetOrdinal("PIN_PRI_DATA_APPUNTAMENTO4")
        '                    Dim dataAppuntamentoTPOrdinal As Integer = reader.GetOrdinal("PIN_PRI_DATA_APPUNTAMENTO_TP")
        '                    Dim dataStampaTPOrdinal As Integer = reader.GetOrdinal("PIN_PRI_DATA_STAMPA_TP")
        '                    Dim idUtenteStampaCSOrdinal As Integer = reader.GetOrdinal("PIN_UTE_ID_STAMPA_CS")
        '                    Dim dataStampaCSOrdinal As Integer = reader.GetOrdinal("PIN_PRI_DATA_STAMPA_CS")
        '                    '--
        '                    While reader.Read
        '                        '--
        '                        Dim inadempienzaPaziente As New MovimentoCNS.InadempienzaPaziente
        '                        '--
        '                        If Not reader.IsDBNull(codicePazientePrecedenteOrdinal) Then inadempienzaPaziente.CodicePazientePrecedente = reader.GetInt32(codicePazientePrecedenteOrdinal)
        '                        '--
        '                        inadempienzaPaziente.Vaccinazione = New MovimentoCNS.VaccinazioneInfo
        '                        inadempienzaPaziente.Vaccinazione.Codice = reader.GetString(codiceVaccinazioneOrdinal)
        '                        '--
        '                        If Not reader.IsDBNull(stampatoOrdinal) Then inadempienzaPaziente.Stampato = reader.GetString(stampatoOrdinal) = "S"
        '                        If Not reader.IsDBNull(statoOrdinal) Then inadempienzaPaziente.Stato = reader.GetString(statoOrdinal)
        '                        If Not reader.IsDBNull(dataOrdinal) Then inadempienzaPaziente.DataInadempienza = reader.GetDateTime(dataOrdinal)
        '                        If Not reader.IsDBNull(idUtenteOrdinal) Then
        '                            inadempienzaPaziente.Utente = New MovimentoCNS.UtenteInfo
        '                            inadempienzaPaziente.Utente.ID = reader.GetInt32(idUtenteOrdinal)
        '                        End If
        '                        If Not reader.IsDBNull(dataAppuntamento1Ordinal) Then inadempienzaPaziente.DataAppuntamento1 = reader.GetDateTime(dataAppuntamento1Ordinal)
        '                        If Not reader.IsDBNull(dataAppuntamento2Ordinal) Then inadempienzaPaziente.DataAppuntamento2 = reader.GetDateTime(dataAppuntamento2Ordinal)
        '                        If Not reader.IsDBNull(dataAppuntamento3Ordinal) Then inadempienzaPaziente.DataAppuntamento3 = reader.GetDateTime(dataAppuntamento3Ordinal)
        '                        If Not reader.IsDBNull(dataAppuntamento4Ordinal) Then inadempienzaPaziente.DataAppuntamento4 = reader.GetDateTime(dataAppuntamento4Ordinal)
        '                        If Not reader.IsDBNull(dataAppuntamentoTPOrdinal) Then inadempienzaPaziente.DataAppuntamentoTP = reader.GetDateTime(dataAppuntamentoTPOrdinal)
        '                        If Not reader.IsDBNull(dataStampaTPOrdinal) Then inadempienzaPaziente.DataStampaTP = reader.GetDateTime(dataStampaTPOrdinal)
        '                        If Not reader.IsDBNull(idUtenteStampaCSOrdinal) Then
        '                            inadempienzaPaziente.UtenteStampaCS = New MovimentoCNS.UtenteInfo
        '                            inadempienzaPaziente.UtenteStampaCS.ID = reader.GetInt32(idUtenteStampaCSOrdinal)
        '                        End If
        '                        If Not reader.IsDBNull(dataStampaCSOrdinal) Then inadempienzaPaziente.DataStampaCS = reader.GetDateTime(dataStampaCSOrdinal)
        '                        '--
        '                        inadempienzaPazienteCollection.Add(inadempienzaPaziente)
        '                        '--
        '                    End While
        '                    '--

        '                End If

        '            Finally

        '                If Not reader Is Nothing Then reader.Close()
        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '            Return inadempienzaPazienteCollection

        '        End Function

        '        Public Function CountInadempienzePaziente(codicePaziente As String) As Integer Implements IMovimentiEsterniCNSProvider.CountInadempienzePaziente

        '            Dim cmd As OracleClient.OracleCommand = Nothing

        '            Try
        '                cmd = New OracleClient.OracleCommand("SELECT COUNT(*) FROM T_PAZ_INADEMPIENZE WHERE PIN_PAZ_CODICE = :codicePaziente", Me.Connection)

        '                cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    Me.conditionalOpenConnection()
        '                End If

        '                Return Convert.ToInt32(cmd.ExecuteScalar())

        '            Finally

        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '        End Function

        '        Public Sub EliminaInadempienzePaziente(codicePaziente As String) Implements IMovimentiEsterniCNSProvider.EliminaInadempienzePaziente

        '            Dim cmd As OracleClient.OracleCommand = Nothing

        '            Try
        '                cmd = New OracleClient.OracleCommand("DELETE FROM T_PAZ_INADEMPIENZE WHERE PIN_PAZ_CODICE = :codicePaziente", Me.Connection)

        '                cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    Me.conditionalOpenConnection()
        '                End If

        '                cmd.ExecuteNonQuery()

        '            Finally

        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '        End Sub

        '        Public Sub InserisciInadempienzePaziente(codicePaziente As String, inadempienzePaziente As System.Collections.ObjectModel.Collection(Of Entities.MovimentoCNS.InadempienzaPaziente)) Implements IMovimentiEsterniCNSProvider.InserisciInadempienzePaziente

        '            Dim cmd As OracleClient.OracleCommand = Nothing

        '            Dim ownTransaction As Boolean = False

        '            Try
        '                cmd = New OracleClient.OracleCommand("INSERT INTO T_PAZ_INADEMPIENZE(PIN_PAZ_CODICE, PIN_PAZ_CODICE_OLD, PIN_VAC_CODICE, PIN_STAMPATO, PIN_STATO, PIN_DATA, PIN_UTE_ID, PIN_PRI_DATA_APPUNTAMENTO1, PIN_PRI_DATA_APPUNTAMENTO2, PIN_PRI_DATA_APPUNTAMENTO3, PIN_PRI_DATA_APPUNTAMENTO4, PIN_PRI_DATA_APPUNTAMENTO_TP, PIN_PRI_DATA_STAMPA_TP, PIN_UTE_ID_STAMPA_CS, PIN_PRI_DATA_STAMPA_CS) VALUES(:codicePaziente, :codicePazientePrecedente, :codiceVaccinazione, :stampato, :stato, :dataInadempienza, :idUtente, :dataAppuntamento1, :dataAppuntamento2, :dataAppuntamento3, :dataAppuntamento4, :dataAppuntamentoTP, :dataStampaTP, :idUtenteStampaCS, :dataStampaCS)", Me.Connection)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    ownTransaction = True
        '                    Me._DAM.BeginTrans()
        '                End If

        '                For Each inadempienzaPaziente As MovimentoCNS.InadempienzaPaziente In inadempienzePaziente

        '                    cmd.Parameters.Clear()

        '                    cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

        '                    If Not inadempienzaPaziente.CodicePazientePrecedente Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codicePazientePrecedente", inadempienzaPaziente.CodicePazientePrecedente)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codicePazientePrecedente", DBNull.Value)
        '                    End If

        '                    cmd.Parameters.AddWithValue("codiceVaccinazione", inadempienzaPaziente.Vaccinazione.Codice)

        '                    If Not inadempienzaPaziente.Stampato Is Nothing Then
        '                        cmd.Parameters.AddWithValue("stampato", IIf(inadempienzaPaziente.Stampato, "S", "N"))
        '                    Else
        '                        cmd.Parameters.AddWithValue("stampato", DBNull.Value)
        '                    End If

        '                    If Not inadempienzaPaziente.Stato Is Nothing Then
        '                        cmd.Parameters.AddWithValue("stato", inadempienzaPaziente.Stato)
        '                    Else
        '                        cmd.Parameters.AddWithValue("stato", DBNull.Value)
        '                    End If

        '                    If Not inadempienzaPaziente.DataInadempienza Is Nothing Then
        '                        cmd.Parameters.AddWithValue("dataInadempienza", inadempienzaPaziente.DataInadempienza)
        '                    Else
        '                        cmd.Parameters.AddWithValue("dataInadempienza", DBNull.Value)
        '                    End If

        '                    If Not inadempienzaPaziente.Utente Is Nothing Then
        '                        cmd.Parameters.AddWithValue("idUtente", inadempienzaPaziente.Utente.ID)
        '                    Else
        '                        cmd.Parameters.AddWithValue("idUtente", DBNull.Value)
        '                    End If

        '                    If Not inadempienzaPaziente.DataAppuntamento1 Is Nothing Then
        '                        cmd.Parameters.AddWithValue("dataAppuntamento1", inadempienzaPaziente.DataAppuntamento1)
        '                    Else
        '                        cmd.Parameters.AddWithValue("dataAppuntamento1", DBNull.Value)
        '                    End If

        '                    If Not inadempienzaPaziente.DataAppuntamento2 Is Nothing Then
        '                        cmd.Parameters.AddWithValue("dataAppuntamento2", inadempienzaPaziente.DataAppuntamento2)
        '                    Else
        '                        cmd.Parameters.AddWithValue("dataAppuntamento2", DBNull.Value)
        '                    End If

        '                    If Not inadempienzaPaziente.DataAppuntamento3 Is Nothing Then
        '                        cmd.Parameters.AddWithValue("dataAppuntamento3", inadempienzaPaziente.DataAppuntamento3)
        '                    Else
        '                        cmd.Parameters.AddWithValue("dataAppuntamento3", DBNull.Value)
        '                    End If

        '                    If Not inadempienzaPaziente.DataAppuntamento4 Is Nothing Then
        '                        cmd.Parameters.AddWithValue("dataAppuntamento4", inadempienzaPaziente.DataAppuntamento4)
        '                    Else
        '                        cmd.Parameters.AddWithValue("dataAppuntamento4", DBNull.Value)
        '                    End If

        '                    If Not inadempienzaPaziente.DataAppuntamentoTP Is Nothing Then
        '                        cmd.Parameters.AddWithValue("dataAppuntamentoTP", inadempienzaPaziente.DataAppuntamentoTP)
        '                    Else
        '                        cmd.Parameters.AddWithValue("dataAppuntamentoTP", DBNull.Value)
        '                    End If

        '                    If Not inadempienzaPaziente.DataStampaTP Is Nothing Then
        '                        cmd.Parameters.AddWithValue("dataStampaTP", inadempienzaPaziente.DataStampaTP)
        '                    Else
        '                        cmd.Parameters.AddWithValue("dataStampaTP", DBNull.Value)
        '                    End If

        '                    If Not inadempienzaPaziente.UtenteStampaCS Is Nothing Then
        '                        cmd.Parameters.AddWithValue("idUtenteStampaCS", inadempienzaPaziente.UtenteStampaCS.ID)
        '                    Else
        '                        cmd.Parameters.AddWithValue("idUtenteStampaCS", DBNull.Value)
        '                    End If

        '                    If Not inadempienzaPaziente.DataStampaCS Is Nothing Then
        '                        cmd.Parameters.AddWithValue("dataStampaCS", inadempienzaPaziente.DataStampaCS)
        '                    Else
        '                        cmd.Parameters.AddWithValue("dataStampaCS", DBNull.Value)
        '                    End If

        '                    cmd.ExecuteNonQuery()

        '                Next

        '                If ownTransaction Then
        '                    Me.Transaction.Commit()
        '                End If

        '            Catch ex As Exception

        '                If ownTransaction Then
        '                    Me.Transaction.Rollback()
        '                End If

        '                ex.InternalPreserveStackTrace()
        '                Throw

        '            Finally

        '                If Not Me.Transaction Is Nothing AndAlso ownTransaction Then Me.Transaction.Dispose()
        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '        End Sub

        '#End Region

        '#Region " Cicli "

        '        Public Function LoadCicliPaziente(codicePaziente As String) As System.Collections.ObjectModel.Collection(Of Entities.MovimentoCNS.CicloPaziente) Implements IMovimentiEsterniCNSProvider.LoadCicliPaziente

        '            Dim cicloPazienteCollection As New Collection(Of MovimentoCNS.CicloPaziente)

        '            Dim cmd As OracleClient.OracleCommand = Nothing
        '            Dim reader As IDataReader = Nothing

        '            Try
        '                cmd = New OracleClient.OracleCommand("SELECT * FROM T_PAZ_CICLI WHERE PAC_PAZ_CODICE = :codicePaziente", Me.Connection)

        '                cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    Me.conditionalOpenConnection()
        '                End If

        '                reader = cmd.ExecuteReader()

        '                If Not reader Is Nothing Then

        '                    '--
        '                    Dim codicePazientePrecedenteOrdinal As Integer = reader.GetOrdinal("PAC_PAZ_CODICE_OLD")
        '                    Dim codiceCicloOrdinal As Integer = reader.GetOrdinal("PAC_CIC_CODICE")
        '                    '--
        '                    While reader.Read
        '                        '-
        '                        Dim cicloPaziente As New MovimentoCNS.CicloPaziente
        '                        '--
        '                        If Not reader.IsDBNull(codicePazientePrecedenteOrdinal) Then cicloPaziente.CodicePazientePrecedente = reader.GetInt32(codicePazientePrecedenteOrdinal)
        '                        '--
        '                        cicloPaziente.Ciclo = New MovimentoCNS.CicloInfo
        '                        cicloPaziente.Ciclo.Codice = reader.GetString(codiceCicloOrdinal)
        '                        '--
        '                        cicloPazienteCollection.Add(cicloPaziente)
        '                        '--
        '                    End While
        '                    '--

        '                End If

        '            Finally

        '                If Not reader Is Nothing Then reader.Close()
        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '            Return cicloPazienteCollection

        '        End Function

        '        Public Function CountCicliPaziente(codicePaziente As String) As Integer Implements IMovimentiEsterniCNSProvider.CountCicliPaziente

        '            Dim cmd As OracleClient.OracleCommand = Nothing

        '            Try
        '                cmd = New OracleClient.OracleCommand("SELECT COUNT(*) FROM T_PAZ_CICLI WHERE PAC_PAZ_CODICE = :codicePaziente", Me.Connection)

        '                cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    Me.conditionalOpenConnection()
        '                End If

        '                Return Convert.ToInt32(cmd.ExecuteScalar())

        '            Finally

        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '        End Function

        '        Public Sub EliminaCicliPaziente(codicePaziente As String) Implements IMovimentiEsterniCNSProvider.EliminaCicliPaziente

        '            Dim cmd As OracleClient.OracleCommand = Nothing

        '            Try
        '                cmd = New OracleClient.OracleCommand("DELETE FROM T_PAZ_CICLI WHERE PAC_PAZ_CODICE = :codicePaziente", Me.Connection)

        '                cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    Me.conditionalOpenConnection()
        '                End If

        '                cmd.ExecuteNonQuery()

        '            Finally

        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '        End Sub

        '        Public Sub InserisciCicliPaziente(codicePaziente As String, cicliPaziente As System.Collections.ObjectModel.Collection(Of Entities.MovimentoCNS.CicloPaziente)) Implements IMovimentiEsterniCNSProvider.InserisciCicliPaziente

        '            Dim cmd As OracleClient.OracleCommand = Nothing

        '            Dim ownTransaction As Boolean = False

        '            Try
        '                cmd = New OracleClient.OracleCommand("INSERT INTO T_PAZ_CICLI(PAC_PAZ_CODICE, PAC_PAZ_CODICE_OLD, PAC_CIC_CODICE) VALUES(:codicePaziente, :codicePazientePrecedente, :codiceCiclo)", Me.Connection)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    ownTransaction = True
        '                    Me._DAM.BeginTrans()
        '                End If

        '                For Each cicloPaziente As MovimentoCNS.CicloPaziente In cicliPaziente

        '                    cmd.Parameters.Clear()

        '                    cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

        '                    If Not cicloPaziente.CodicePazientePrecedente Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codicePazientePrecedente", cicloPaziente.CodicePazientePrecedente)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codicePazientePrecedente", DBNull.Value)
        '                    End If

        '                    cmd.Parameters.AddWithValue("codiceCiclo", cicloPaziente.Ciclo.Codice)

        '                    cmd.ExecuteNonQuery()

        '                Next

        '                If ownTransaction Then
        '                    Me.Transaction.Commit()
        '                End If

        '            Catch ex As Exception

        '                If ownTransaction Then
        '                    Me.Transaction.Rollback()
        '                End If

        '                ex.InternalPreserveStackTrace()
        '                Throw

        '            Finally

        '                If Not Me.Transaction Is Nothing AndAlso ownTransaction Then Me.Transaction.Dispose()
        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '        End Sub

        '#End Region

        '#Region " Vaccinazioni Eseguite "

        '        Public Function LoadVaccinazioniEseguitePaziente(codicePaziente As String) As System.Collections.ObjectModel.Collection(Of Entities.MovimentoCNS.VaccinazioneEseguitaPaziente) Implements IMovimentiEsterniCNSProvider.LoadVaccinazioniEseguitePaziente

        '            Dim vaccinazioneEseguitaPazienteCollection As New Collection(Of MovimentoCNS.VaccinazioneEseguitaPaziente)

        '            Dim cmd As OracleClient.OracleCommand = Nothing
        '            Dim reader As IDataReader = Nothing

        '            Try

        '                cmd = New OracleClient.OracleCommand(Queries.MovimentiEsterniCNS.OracleQueries.selVacEseguitePaziente, Me.Connection)

        '                cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    Me.conditionalOpenConnection()
        '                End If

        '                reader = cmd.ExecuteReader()

        '                If Not reader Is Nothing Then

        '                    '--
        '                    Dim IDOrdinal As Integer = reader.GetOrdinal("VES_ID")
        '                    Dim codicePazientePrecedenteOrdinal As Integer = reader.GetOrdinal("VES_PAZ_CODICE_OLD")
        '                    Dim codiceVaccinazioneOrdinal As Integer = reader.GetOrdinal("VES_VAC_CODICE")
        '                    Dim numeroRichiamoOrdinal As Integer = reader.GetOrdinal("VES_N_RICHIAMO")
        '                    Dim dataEffettuazioneOrdinal As Integer = reader.GetOrdinal("VES_DATA_EFFETTUAZIONE")
        '                    Dim dataOraEffettuazioneOrdinal As Integer = reader.GetOrdinal("VES_DATAORA_EFFETTUAZIONE")
        '                    Dim codiceConsultorioOrdinal As Integer = reader.GetOrdinal("VES_CNS_CODICE")
        '                    Dim statoOrdinal As Integer = reader.GetOrdinal("VES_STATO")
        '                    Dim codiceCicloOrdinal As Integer = reader.GetOrdinal("VES_CIC_CODICE")
        '                    Dim numeroSedutaOrdinal As Integer = reader.GetOrdinal("VES_N_SEDUTA")
        '                    Dim dataRegistrazioneOrdinal As Integer = reader.GetOrdinal("VES_DATA_REGISTRAZIONE")
        '                    Dim codiceLottoOrdinal As Integer = reader.GetOrdinal("VES_LOT_CODICE")
        '                    Dim codiceOperatoreOrdinal As Integer = reader.GetOrdinal("VES_OPE_CODICE")
        '                    Dim idUtenteOrdinal As Integer = reader.GetOrdinal("VES_UTE_ID")
        '                    Dim codiceSitoInoculazioneOrdinal As Integer = reader.GetOrdinal("VES_SII_CODICE")
        '                    Dim codiceNomeCommercialeOrdinal As Integer = reader.GetOrdinal("VES_NOC_CODICE")
        '                    Dim codiceAssociazioneOrdinal As Integer = reader.GetOrdinal("VES_ASS_CODICE")
        '                    Dim luogoOrdinal As Integer = reader.GetOrdinal("VES_LUOGO")
        '                    Dim codiceComuneOrdinal As Integer = reader.GetOrdinal("VES_COMUNE_O_STATO")
        '                    Dim codiceMedicoVaccinanteOrdinal As Integer = reader.GetOrdinal("VES_MED_VACCINANTE")
        '                    Dim dataConvocazioneOrdinal As Integer = reader.GetOrdinal("VES_CNV_DATA")
        '                    Dim dataPrimoAppuntamentoConvocazioneOrdinal As Integer = reader.GetOrdinal("VES_CNV_DATA_PRIMO_APP")
        '                    Dim inCampagnaOrdinal As Integer = reader.GetOrdinal("VES_IN_CAMPAGNA")
        '                    Dim operatoreInAmbulatorioOrdinal As Integer = reader.GetOrdinal("VES_OPE_IN_AMBULATORIO")
        '                    Dim esitoOrdinal As Integer = reader.GetOrdinal("VES_ESITO")
        '                    Dim fittiziaOrdinal As Integer = reader.GetOrdinal("VES_FLAG_FITTIZIA")
        '                    Dim noteOrdinal As Integer = reader.GetOrdinal("VES_NOTE")
        '                    Dim codiceConsultorioRegistrazioneOrdinal As Integer = reader.GetOrdinal("VES_CNS_REGISTRAZIONE")
        '                    Dim accessoOrdinal As Integer = reader.GetOrdinal("VES_ACCESSO")
        '                    Dim codiceAmbulatorioOrdinal As Integer = reader.GetOrdinal("VES_AMB_CODICE")
        '                    Dim codiceViaSomministrazioneOrdinal As Integer = reader.GetOrdinal("VES_VII_CODICE")
        '                    Dim codiceMalattiaOrdinal As Integer = reader.GetOrdinal("VES_MAL_CODICE_MALATTIA")
        '                    Dim codiceEsenzioneOrdinal As Integer = reader.GetOrdinal("VES_CODICE_ESENZIONE")
        '                    Dim importoOrdinal As Integer = reader.GetOrdinal("VES_IMPORTO")
        '                    Dim scadutaOrdinal As Integer = reader.GetOrdinal("VES_SCADUTA")
        '                    Dim associazioneDoseOrdinal As Integer = reader.GetOrdinal("VES_ASS_N_DOSE")
        '                    Dim nominativoMedicoOrdinal As Integer = reader.GetOrdinal("MED_DESCRIZIONE")
        '                    Dim nominativoOperatoreOrdinal As Integer = reader.GetOrdinal("OPE_NOME")
        '                    '--
        '                    While reader.Read
        '                        '--
        '                        Dim vaccinazioneEseguitaPaziente As New MovimentoCNS.VaccinazioneEseguitaPaziente

        '                        vaccinazioneEseguitaPaziente.ID = reader.GetInt32(IDOrdinal)

        '                        If Not reader.IsDBNull(codicePazientePrecedenteOrdinal) Then vaccinazioneEseguitaPaziente.CodicePazientePrecedente = reader.GetInt32(codicePazientePrecedenteOrdinal)
        '                        '--
        '                        vaccinazioneEseguitaPaziente.Vaccinazione = New MovimentoCNS.VaccinazioneInfo
        '                        vaccinazioneEseguitaPaziente.Vaccinazione.Codice = reader.GetString(codiceVaccinazioneOrdinal)
        '                        '--
        '                        If Not reader.IsDBNull(numeroRichiamoOrdinal) Then vaccinazioneEseguitaPaziente.NumeroRichiamo = reader.GetInt32(numeroRichiamoOrdinal)
        '                        If Not reader.IsDBNull(codiceConsultorioOrdinal) Then
        '                            vaccinazioneEseguitaPaziente.Consultorio = New MovimentoCNS.ConsultorioInfo
        '                            vaccinazioneEseguitaPaziente.Consultorio.Codice = reader.GetString(codiceConsultorioOrdinal)
        '                        End If
        '                        If Not reader.IsDBNull(dataOraEffettuazioneOrdinal) Then vaccinazioneEseguitaPaziente.DataOraEffettuazione = reader.GetDateTime(dataOraEffettuazioneOrdinal)
        '                        '--
        '                        If Not reader.IsDBNull(dataEffettuazioneOrdinal) Then vaccinazioneEseguitaPaziente.DataEffettuazione = reader.GetDateTime(dataEffettuazioneOrdinal)
        '                        If Not reader.IsDBNull(statoOrdinal) Then vaccinazioneEseguitaPaziente.Stato = reader.GetString(statoOrdinal)
        '                        If Not reader.IsDBNull(codiceCicloOrdinal) Then
        '                            vaccinazioneEseguitaPaziente.Ciclo = New MovimentoCNS.CicloInfo
        '                            vaccinazioneEseguitaPaziente.Ciclo.Codice = reader.GetString(codiceCicloOrdinal)
        '                        End If
        '                        If Not reader.IsDBNull(numeroSedutaOrdinal) Then vaccinazioneEseguitaPaziente.NumeroSeduta = reader.GetInt32(numeroSedutaOrdinal)
        '                        If Not reader.IsDBNull(dataRegistrazioneOrdinal) Then vaccinazioneEseguitaPaziente.DataRegistrazione = reader.GetDateTime(dataRegistrazioneOrdinal)
        '                        If Not reader.IsDBNull(codiceLottoOrdinal) Then
        '                            vaccinazioneEseguitaPaziente.Lotto = New MovimentoCNS.LottoInfo
        '                            vaccinazioneEseguitaPaziente.Lotto.Codice = reader.GetString(codiceLottoOrdinal)
        '                        End If
        '                        If Not reader.IsDBNull(codiceOperatoreOrdinal) Then
        '                            vaccinazioneEseguitaPaziente.Operatore = New MovimentoCNS.OperatoreInfo
        '                            vaccinazioneEseguitaPaziente.Operatore.Codice = reader.GetString(codiceOperatoreOrdinal)
        '                            vaccinazioneEseguitaPaziente.Operatore.Nominativo = reader.GetStringOrDefault(nominativoOperatoreOrdinal)
        '                        End If
        '                        If Not reader.IsDBNull(idUtenteOrdinal) Then
        '                            vaccinazioneEseguitaPaziente.Utente = New MovimentoCNS.UtenteInfo
        '                            vaccinazioneEseguitaPaziente.Utente.ID = reader.GetInt32(idUtenteOrdinal)
        '                        End If
        '                        If Not reader.IsDBNull(codiceSitoInoculazioneOrdinal) Then
        '                            vaccinazioneEseguitaPaziente.SitoInoculazione = New MovimentoCNS.SitoInoculazioneInfo
        '                            vaccinazioneEseguitaPaziente.SitoInoculazione.Codice = reader.GetString(codiceSitoInoculazioneOrdinal)
        '                        End If
        '                        If Not reader.IsDBNull(codiceNomeCommercialeOrdinal) Then
        '                            vaccinazioneEseguitaPaziente.NomeCommerciale = New MovimentoCNS.NomeCommercialeInfo
        '                            vaccinazioneEseguitaPaziente.NomeCommerciale.Codice = reader.GetString(codiceNomeCommercialeOrdinal)
        '                        End If

        '                        If Not reader.IsDBNull(codiceAssociazioneOrdinal) Then
        '                            vaccinazioneEseguitaPaziente.Associazione = New MovimentoCNS.AssociazioneInfo
        '                            vaccinazioneEseguitaPaziente.Associazione.Codice = reader.GetString(codiceAssociazioneOrdinal)
        '                            If Not reader.IsDBNull(associazioneDoseOrdinal) Then
        '                                vaccinazioneEseguitaPaziente.Associazione.Dose = reader.GetInt32(associazioneDoseOrdinal)
        '                            End If
        '                        End If

        '                        If Not reader.IsDBNull(luogoOrdinal) Then vaccinazioneEseguitaPaziente.Luogo = reader.GetString(luogoOrdinal)
        '                        If Not reader.IsDBNull(codiceComuneOrdinal) Then
        '                            vaccinazioneEseguitaPaziente.Comune = New MovimentoCNS.ComuneInfo
        '                            vaccinazioneEseguitaPaziente.Comune.Codice = reader.GetString(codiceComuneOrdinal)
        '                        End If
        '                        If Not reader.IsDBNull(dataConvocazioneOrdinal) Then vaccinazioneEseguitaPaziente.DataConvocazione = reader.GetDateTime(dataConvocazioneOrdinal)
        '                        If Not reader.IsDBNull(dataPrimoAppuntamentoConvocazioneOrdinal) Then vaccinazioneEseguitaPaziente.DataPrimoAppuntamentoConvocazione = reader.GetDateTime(dataPrimoAppuntamentoConvocazioneOrdinal)
        '                        If Not reader.IsDBNull(inCampagnaOrdinal) Then vaccinazioneEseguitaPaziente.InCampagna = reader.GetString(inCampagnaOrdinal) = "S"
        '                        If Not reader.IsDBNull(operatoreInAmbulatorioOrdinal) Then vaccinazioneEseguitaPaziente.OperatoreInAmbulatorio = reader.GetString(operatoreInAmbulatorioOrdinal) = "S"
        '                        If Not reader.IsDBNull(codiceMedicoVaccinanteOrdinal) Then
        '                            vaccinazioneEseguitaPaziente.MedicoVaccinante = New MovimentoCNS.MedicoInfo
        '                            vaccinazioneEseguitaPaziente.MedicoVaccinante.Codice = reader.GetString(codiceMedicoVaccinanteOrdinal)
        '                            vaccinazioneEseguitaPaziente.MedicoVaccinante.Nominativo = reader.GetStringOrDefault(nominativoMedicoOrdinal)
        '                        End If
        '                        If Not reader.IsDBNull(esitoOrdinal) Then vaccinazioneEseguitaPaziente.Esito = reader.GetString(esitoOrdinal)
        '                        If Not reader.IsDBNull(fittiziaOrdinal) Then vaccinazioneEseguitaPaziente.Fittizia = (reader.GetString(fittiziaOrdinal) = "S")
        '                        If Not reader.IsDBNull(noteOrdinal) Then vaccinazioneEseguitaPaziente.Note = reader.GetString(noteOrdinal)
        '                        If Not reader.IsDBNull(codiceConsultorioRegistrazioneOrdinal) Then
        '                            vaccinazioneEseguitaPaziente.ConsultorioRegistrazione = New MovimentoCNS.ConsultorioInfo
        '                            vaccinazioneEseguitaPaziente.ConsultorioRegistrazione.Codice = reader.GetString(codiceConsultorioRegistrazioneOrdinal)
        '                        End If
        '                        If Not reader.IsDBNull(accessoOrdinal) Then vaccinazioneEseguitaPaziente.Accesso = reader.GetString(accessoOrdinal)
        '                        If Not reader.IsDBNull(codiceAmbulatorioOrdinal) Then
        '                            vaccinazioneEseguitaPaziente.Ambulatorio = New MovimentoCNS.AmbulatorioInfo
        '                            vaccinazioneEseguitaPaziente.Ambulatorio.Codice = reader.GetInt32(codiceAmbulatorioOrdinal)
        '                        End If
        '                        If Not reader.IsDBNull(codiceViaSomministrazioneOrdinal) Then
        '                            vaccinazioneEseguitaPaziente.ViaSomministrazione = New MovimentoCNS.ViaSomministrazioneInfo
        '                            vaccinazioneEseguitaPaziente.ViaSomministrazione.Codice = reader.GetString(codiceViaSomministrazioneOrdinal)
        '                        End If
        '                        If Not reader.IsDBNull(codiceMalattiaOrdinal) Then
        '                            vaccinazioneEseguitaPaziente.Malattia = New MovimentoCNS.MalattiaInfo
        '                            vaccinazioneEseguitaPaziente.Malattia.Codice = reader.GetString(codiceMalattiaOrdinal)
        '                        End If
        '                        If Not reader.IsDBNull(codiceEsenzioneOrdinal) Then vaccinazioneEseguitaPaziente.CodiceEsenzione = reader.GetString(codiceEsenzioneOrdinal)
        '                        If Not reader.IsDBNull(importoOrdinal) Then vaccinazioneEseguitaPaziente.Importo = reader.GetDouble(importoOrdinal)

        '                        vaccinazioneEseguitaPaziente.Scaduta = reader.GetString(scadutaOrdinal) = "S"
        '                        '--
        '                        vaccinazioneEseguitaPazienteCollection.Add(vaccinazioneEseguitaPaziente)
        '                        '--
        '                    End While
        '                    '--

        '                End If

        '            Finally

        '                If Not reader Is Nothing Then reader.Close()
        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '            Return vaccinazioneEseguitaPazienteCollection


        '        End Function

        '        Public Sub InserisciVaccinazioniEseguitePaziente(codicePaziente As String, vaccinazioneEseguitePaziente As System.Collections.ObjectModel.Collection(Of Entities.MovimentoCNS.VaccinazioneEseguitaPaziente)) Implements IMovimentiEsterniCNSProvider.InserisciVaccinazioniEseguitePaziente

        '            Dim cmd As OracleClient.OracleCommand = Nothing

        '            Dim ownTransaction As Boolean = False

        '            Try
        '                cmd = New OracleClient.OracleCommand()
        '                cmd.Connection = Me.Connection

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    ownTransaction = True
        '                    Me._DAM.BeginTrans()
        '                End If

        '                For Each vaccinazioneEseguitaPaziente As MovimentoCNS.VaccinazioneEseguitaPaziente In vaccinazioneEseguitePaziente

        '                    If vaccinazioneEseguitaPaziente.Scaduta Then
        '                        cmd.CommandText = Queries.MovimentiEsterniCNS.OracleQueries.insVacScaduta
        '                    Else
        '                        cmd.CommandText = Queries.MovimentiEsterniCNS.OracleQueries.insVacEseguita
        '                    End If

        '                    cmd.Parameters.Clear()

        '                    If vaccinazioneEseguitaPaziente.ID > 0 Then
        '                        cmd.Parameters.AddWithValue("id", vaccinazioneEseguitaPaziente.ID)
        '                    Else
        '                        cmd.Parameters.AddWithValue("id", DBNull.Value)
        '                    End If

        '                    cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)
        '                    cmd.Parameters.AddWithValue("codiceVaccinazione", vaccinazioneEseguitaPaziente.Vaccinazione.Codice)
        '                    cmd.Parameters.AddWithValue("numeroRichiamo", vaccinazioneEseguitaPaziente.NumeroRichiamo)
        '                    cmd.Parameters.AddWithValue("dataEffettuazione", vaccinazioneEseguitaPaziente.DataEffettuazione)
        '                    cmd.Parameters.AddWithValue("dataOraEffettuazione", vaccinazioneEseguitaPaziente.DataOraEffettuazione)

        '                    If Not vaccinazioneEseguitaPaziente.CodicePazientePrecedente Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codicePazientePrecedente", vaccinazioneEseguitaPaziente.CodicePazientePrecedente)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codicePazientePrecedente", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEseguitaPaziente.Consultorio Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codiceConsultorio", vaccinazioneEseguitaPaziente.Consultorio.Codice)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codiceConsultorio", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEseguitaPaziente.Stato Is Nothing Then
        '                        cmd.Parameters.AddWithValue("stato", vaccinazioneEseguitaPaziente.Stato)
        '                    Else
        '                        cmd.Parameters.AddWithValue("stato", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEseguitaPaziente.Ciclo Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codiceCiclo", vaccinazioneEseguitaPaziente.Ciclo.Codice)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codiceCiclo", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEseguitaPaziente.NumeroSeduta Is Nothing Then
        '                        cmd.Parameters.AddWithValue("numeroSeduta", vaccinazioneEseguitaPaziente.NumeroSeduta)
        '                    Else
        '                        cmd.Parameters.AddWithValue("numeroSeduta", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEseguitaPaziente.DataRegistrazione Is Nothing Then
        '                        cmd.Parameters.AddWithValue("dataRegistrazione", vaccinazioneEseguitaPaziente.DataRegistrazione)
        '                    Else
        '                        cmd.Parameters.AddWithValue("dataRegistrazione", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEseguitaPaziente.Lotto Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codiceLotto", vaccinazioneEseguitaPaziente.Lotto.Codice)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codiceLotto", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEseguitaPaziente.Operatore Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codiceOperatore", vaccinazioneEseguitaPaziente.Operatore.Codice)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codiceOperatore", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEseguitaPaziente.Utente Is Nothing Then
        '                        cmd.Parameters.AddWithValue("idUtente", vaccinazioneEseguitaPaziente.Utente.ID)
        '                    Else
        '                        cmd.Parameters.AddWithValue("idUtente", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEseguitaPaziente.SitoInoculazione Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codiceSitoInoculazione", vaccinazioneEseguitaPaziente.SitoInoculazione.Codice)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codiceSitoInoculazione", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEseguitaPaziente.NomeCommerciale Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codiceNomeCommerciale", vaccinazioneEseguitaPaziente.NomeCommerciale.Codice)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codiceNomeCommerciale", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEseguitaPaziente.Associazione Is Nothing Then
        '                        cmd.Parameters.AddWithValue("ass_n_dose", vaccinazioneEseguitaPaziente.Associazione.Dose)
        '                        cmd.Parameters.AddWithValue("codiceAssociazione", vaccinazioneEseguitaPaziente.Associazione.Codice)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codiceAssociazione", DBNull.Value)
        '                        cmd.Parameters.AddWithValue("ass_n_dose", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEseguitaPaziente.Luogo Is Nothing Then
        '                        cmd.Parameters.AddWithValue("luogo", vaccinazioneEseguitaPaziente.Luogo)
        '                    Else
        '                        cmd.Parameters.AddWithValue("luogo", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEseguitaPaziente.Comune Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codiceComune", vaccinazioneEseguitaPaziente.Comune.Codice)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codiceComune", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEseguitaPaziente.MedicoVaccinante Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codiceMedicoVaccinante", vaccinazioneEseguitaPaziente.MedicoVaccinante.Codice)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codiceMedicoVaccinante", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEseguitaPaziente.DataConvocazione Is Nothing Then
        '                        cmd.Parameters.AddWithValue("dataConvocazione", vaccinazioneEseguitaPaziente.DataConvocazione)
        '                    Else
        '                        cmd.Parameters.AddWithValue("dataConvocazione", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEseguitaPaziente.DataPrimoAppuntamentoConvocazione Is Nothing Then
        '                        cmd.Parameters.AddWithValue("dataPrimoAppConvocazione", vaccinazioneEseguitaPaziente.DataPrimoAppuntamentoConvocazione)
        '                    Else
        '                        cmd.Parameters.AddWithValue("dataPrimoAppConvocazione", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEseguitaPaziente.InCampagna Is Nothing Then
        '                        cmd.Parameters.AddWithValue("inCampagna", IIf(vaccinazioneEseguitaPaziente.InCampagna, "S", "N"))
        '                    Else
        '                        cmd.Parameters.AddWithValue("inCampagna", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEseguitaPaziente.OperatoreInAmbulatorio Is Nothing Then
        '                        cmd.Parameters.AddWithValue("operatoreInAmbulatorio", IIf(vaccinazioneEseguitaPaziente.OperatoreInAmbulatorio, "S", "N"))
        '                    Else
        '                        cmd.Parameters.AddWithValue("operatoreInAmbulatorio", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEseguitaPaziente.Esito Is Nothing Then
        '                        cmd.Parameters.AddWithValue("esito", vaccinazioneEseguitaPaziente.Esito)
        '                    Else
        '                        cmd.Parameters.AddWithValue("esito", DBNull.Value)
        '                    End If

        '                    cmd.Parameters.AddWithValue("fittizia", IIf(vaccinazioneEseguitaPaziente.Fittizia, "S", "N"))

        '                    If Not vaccinazioneEseguitaPaziente.Note Is Nothing Then
        '                        cmd.Parameters.AddWithValue("note", vaccinazioneEseguitaPaziente.Note)
        '                    Else
        '                        cmd.Parameters.AddWithValue("note", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEseguitaPaziente.ConsultorioRegistrazione Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codiceConsultorioRegistrazione", vaccinazioneEseguitaPaziente.ConsultorioRegistrazione.Codice)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codiceConsultorioRegistrazione", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEseguitaPaziente.Accesso Is Nothing Then
        '                        cmd.Parameters.AddWithValue("accesso", vaccinazioneEseguitaPaziente.Accesso)
        '                    Else
        '                        cmd.Parameters.AddWithValue("accesso", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEseguitaPaziente.Ambulatorio Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codiceAmbulatorio", vaccinazioneEseguitaPaziente.Ambulatorio.Codice)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codiceAmbulatorio", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEseguitaPaziente.ViaSomministrazione Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codiceViaSomministrazione", vaccinazioneEseguitaPaziente.ViaSomministrazione.Codice)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codiceViaSomministrazione", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEseguitaPaziente.Malattia Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codiceMalattia", vaccinazioneEseguitaPaziente.Malattia.Codice)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codiceMalattia", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEseguitaPaziente.CodiceEsenzione Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codiceEsenzione", vaccinazioneEseguitaPaziente.CodiceEsenzione)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codiceEsenzione", DBNull.Value)
        '                    End If

        '                    If Not vaccinazioneEseguitaPaziente.Importo Is Nothing Then
        '                        cmd.Parameters.AddWithValue("importo", vaccinazioneEseguitaPaziente.Importo.Value)
        '                    Else
        '                        cmd.Parameters.AddWithValue("importo", DBNull.Value)
        '                    End If

        '                    cmd.ExecuteNonQuery()

        '                Next

        '                If ownTransaction Then
        '                    Me.Transaction.Commit()
        '                End If

        '            Catch ex As Exception

        '                If ownTransaction Then
        '                    Me.Transaction.Rollback()
        '                End If

        '                ex.InternalPreserveStackTrace()
        '                Throw

        '            Finally

        '                If Not Me.Transaction Is Nothing AndAlso ownTransaction Then Me.Transaction.Dispose()
        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '        End Sub

        '#End Region

        '#Region " Reazioni Avverse "

        '        Public Function LoadReazioniAvversePaziente(codicePaziente As String) As System.Collections.ObjectModel.Collection(Of Entities.MovimentoCNS.ReazioneAvversaPaziente) Implements IMovimentiEsterniCNSProvider.LoadReazioniAvversePaziente

        '            Dim reazioneAvversaPazienteCollection As New Collection(Of MovimentoCNS.ReazioneAvversaPaziente)

        '            Dim cmd As OracleClient.OracleCommand = Nothing
        '            Dim reader As IDataReader = Nothing

        '            Try

        '                cmd = New OracleClient.OracleCommand("SELECT VRA_VAC_CODICE, VRA_N_RICHIAMO,  VRA_RES_DATA_EFFETTUAZIONE,  VRA_REA_CODICE, VRA_DATA_REAZIONE,  VRA_VISITA, VRA_TERAPIA,  VRA_ESI_CODICE, VRA_RE1_CODICE,  VRA_RE2_CODICE, VRA_PAZ_CODICE_OLD,  VRA_NOC_DESCRIZIONE, VRA_LOT_CODICE,  VRA_LOT_DATA_SCADENZA, VRA_DOSAGGIO,  VRA_SOMMINISTRAZIONE,  VRA_ORA_EFFETTUAZIONE,  VRA_SII_CODICE, VRA_SOSPESO,  VRA_MIGLIORATA, VRA_RIPRESO,  VRA_RICOMPARSA, VRA_INDICAZIONI,  VRA_RICHIAMO, VRA_LUOGO,  VRA_FARMACO_CONCOMITANTE,  VRA_FARMACO_DESCRIZIONE,  VRA_USO_CONCOMITANTE,  VRA_CONDIZIONI_CONCOMITANTI,  VRA_QUALIFICA, VRA_ALTRA_QUALIFICA,  VRA_ALTRO_LUOGO,  VRA_COGNOME_SEGNALATORE,  VRA_NOME_SEGNALATORE,  VRA_INDIRIZZO_SEGNALATORE,  VRA_TEL_SEGNALATORE,  VRA_FAX_SEGNALATORE,  VRA_MAIL_SEGNALATORE,  VRA_DATA_ESITO,  VRA_DATA_COMPILAZIONE,  VRA_REA_ALTRO, VRA_GRAVITA_REAZIONE,  VRA_GRAVE, VRA_ESITO,  VRA_MOTIVO_DECESSO, 'N' VRA_SCADUTA FROM T_VAC_REAZIONI_AVVERSE WHERE VRA_PAZ_CODICE = :codicePaziente UNION SELECT VRS_VAC_CODICE VRA_VAC_CODICE, VRS_N_RICHIAMO VRA_N_RICHIAMO, VRS_RES_DATA_EFFETTUAZIONE VRA_RES_DATA_EFFETTUAZIONE, VRS_REA_CODICE VRA_REA_CODICE , VRS_DATA_REAZIONE VRA_DATA_REAZIONE , VRS_VISITA VRA_VISITA, VRS_TERAPIA VRA_TERAPIA, VRS_ESI_CODICE VRA_ESI_CODICE, VRS_RE1_CODICE VRA_RE1_CODICE, VRS_RE2_CODICE VRA_RE2_CODICE, VRS_PAZ_CODICE_OLD VRA_PAZ_CODICE_OLD , VRS_NOC_DESCRIZIONE VRA_NOC_DESCRIZIONE , VRS_LOT_CODICE VRA_LOT_CODICE , VRS_LOT_DATA_SCADENZA VRA_LOT_DATA_SCADENZA , VRS_DOSAGGIO VRA_DOSAGGIO , VRS_SOMMINISTRAZIONE VRA_SOMMINISTRAZIONE , VRS_ORA_EFFETTUAZIONE VRA_ORA_EFFETTUAZIONE , VRS_SII_CODICE VRA_SII_CODICE , VRS_SOSPESO VRA_SOSPESO , VRS_MIGLIORATA VRA_MIGLIORATA, VRS_RIPRESO VRA_RIPRESO , VRS_RICOMPARSA VRA_RICOMPARSA , VRS_INDICAZIONI VRA_INDICAZIONI , VRS_RICHIAMO VRA_RICHIAMO , VRS_LUOGO VRA_LUOGO , VRS_FARMACO_CONCOMITANTE VRA_FARMACO_CONCOMITANTE , VRS_FARMACO_DESCRIZIONE VRA_FARMACO_DESCRIZIONE , VRS_USO_CONCOMITANTE VRA_USO_CONCOMITANTE , VRS_CONDIZIONI_CONCOMITANTI VRA_CONDIZIONI_CONCOMITANTI , VRS_QUALIFICA VRA_QUALIFICA , VRS_ALTRA_QUALIFICA VRA_ALTRA_QUALIFICA , VRS_ALTRO_LUOGO VRA_ALTRO_LUOGO ,  VRS_COGNOME_SEGNALATORE VRA_COGNOME_SEGNALATORE , VRS_NOME_SEGNALATORE VRA_NOME_SEGNALATORE ,  VRS_INDIRIZZO_SEGNALATORE VRA_INDIRIZZO_SEGNALATORE , VRS_TEL_SEGNALATORE VRA_TEL_SEGNALATORE, VRS_FAX_SEGNALATORE VRA_FAX_SEGNALATORE , VRS_MAIL_SEGNALATORE VRA_MAIL_SEGNALATORE , VRS_DATA_ESITO VRA_DATA_ESITO , VRS_DATA_COMPILAZIONE VRA_DATA_COMPILAZIONE , VRS_REA_ALTRO VRA_REA_ALTRO , VRS_GRAVITA_REAZIONE VRA_GRAVITA_REAZIONE , VRS_GRAVE VRA_GRAVE , VRS_ESITO VRA_ESITO , VRS_MOTIVO_DECESSO VRA_MOTIVO_DECESSO , 'S' VRS_SCADUTA FROM T_VAC_REAZIONI_SCADUTE WHERE VRS_PAZ_CODICE = :codicePaziente", Me.Connection)

        '                cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    Me.conditionalOpenConnection()
        '                End If

        '                reader = cmd.ExecuteReader()

        '                If Not reader Is Nothing Then

        '                    '--
        '                    Dim codicePazientePrecedenteOrdinal As Integer = reader.GetOrdinal("VRA_PAZ_CODICE_OLD")
        '                    Dim codiceVaccinazioneOrdinal As Integer = reader.GetOrdinal("VRA_VAC_CODICE")
        '                    Dim numeroRichiamoOrdinal As Integer = reader.GetOrdinal("VRA_N_RICHIAMO")
        '                    Dim dataEffettuazioneOrdinal As Integer = reader.GetOrdinal("VRA_RES_DATA_EFFETTUAZIONE")
        '                    Dim oraEffettuazioneOrdinal As Integer = reader.GetOrdinal("VRA_ORA_EFFETTUAZIONE")
        '                    Dim dataReazioneOrdinal As Integer = reader.GetOrdinal("VRA_DATA_REAZIONE")
        '                    Dim codiceReazioneOrdinal As Integer = reader.GetOrdinal("VRA_REA_CODICE")
        '                    Dim codiceReazione1Ordinal As Integer = reader.GetOrdinal("VRA_RE1_CODICE")
        '                    Dim codiceReazione2Ordinal As Integer = reader.GetOrdinal("VRA_RE2_CODICE")
        '                    Dim reazioneAltroOrdinal As Integer = reader.GetOrdinal("VRA_REA_ALTRO")
        '                    Dim gravitaReazioneOrdinal As Integer = reader.GetOrdinal("VRA_GRAVITA_REAZIONE")
        '                    Dim visitaOrdinal As Integer = reader.GetOrdinal("VRA_VISITA")
        '                    Dim terapiaOrdinal As Integer = reader.GetOrdinal("VRA_TERAPIA")
        '                    Dim codiceEsitoTerapiaOrdinal As Integer = reader.GetOrdinal("VRA_ESI_CODICE")
        '                    Dim descrizioneNomeCommercialeOrdinal As Integer = reader.GetOrdinal("VRA_NOC_DESCRIZIONE")
        '                    Dim codiceLottoOrdinal As Integer = reader.GetOrdinal("VRA_LOT_CODICE")
        '                    Dim dataScadenzaLottoOrdinal As Integer = reader.GetOrdinal("VRA_LOT_DATA_SCADENZA")
        '                    Dim dosaggioOrdinal As Integer = reader.GetOrdinal("VRA_DOSAGGIO")
        '                    Dim somministrazioneOrdinal As Integer = reader.GetOrdinal("VRA_SOMMINISTRAZIONE")
        '                    Dim codiceSitoInoculazioneOrdinal As Integer = reader.GetOrdinal("VRA_SII_CODICE")
        '                    Dim sospesoOrdinal As Integer = reader.GetOrdinal("VRA_SOSPESO")
        '                    Dim migliorataOrdinal As Integer = reader.GetOrdinal("VRA_MIGLIORATA")
        '                    Dim ripresoOrdinal As Integer = reader.GetOrdinal("VRA_RIPRESO")
        '                    Dim ricomparsaOrdinal As Integer = reader.GetOrdinal("VRA_RICOMPARSA")
        '                    Dim indicazioniOrdinal As Integer = reader.GetOrdinal("VRA_INDICAZIONI")
        '                    Dim richiamoOrdinal As Integer = reader.GetOrdinal("VRA_RICHIAMO")
        '                    Dim luogoOrdinal As Integer = reader.GetOrdinal("VRA_LUOGO")
        '                    Dim altroLuogoOrdinal As Integer = reader.GetOrdinal("VRA_ALTRO_LUOGO")
        '                    Dim farmacoConcomitanteOrdinal As Integer = reader.GetOrdinal("VRA_FARMACO_CONCOMITANTE")
        '                    Dim descrizioneFarmacoOrdinal As Integer = reader.GetOrdinal("VRA_FARMACO_DESCRIZIONE")
        '                    Dim usoConcomitanteOrdinal As Integer = reader.GetOrdinal("VRA_USO_CONCOMITANTE")
        '                    Dim condizioniConcomitantiOrdinal As Integer = reader.GetOrdinal("VRA_CONDIZIONI_CONCOMITANTI")
        '                    Dim qualificaOrdinal As Integer = reader.GetOrdinal("VRA_QUALIFICA")
        '                    Dim altraQualificaOrdinal As Integer = reader.GetOrdinal("VRA_ALTRA_QUALIFICA")
        '                    Dim cognomeSegnalatoreOrdinal As Integer = reader.GetOrdinal("VRA_COGNOME_SEGNALATORE")
        '                    Dim nomeSegnalatoreOrdinal As Integer = reader.GetOrdinal("VRA_NOME_SEGNALATORE")
        '                    Dim indirizzoSegnalatoreOrdinal As Integer = reader.GetOrdinal("VRA_INDIRIZZO_SEGNALATORE")
        '                    Dim telefonoSegnalatoreOrdinal As Integer = reader.GetOrdinal("VRA_TEL_SEGNALATORE")
        '                    Dim faxSegnalatoreOrdinal As Integer = reader.GetOrdinal("VRA_FAX_SEGNALATORE")
        '                    Dim mailSegnalatoreOrdinal As Integer = reader.GetOrdinal("VRA_MAIL_SEGNALATORE")
        '                    Dim dataEsitoOrdinal As Integer = reader.GetOrdinal("VRA_DATA_ESITO")
        '                    Dim dataCompilazioneOrdinal As Integer = reader.GetOrdinal("VRA_DATA_COMPILAZIONE")
        '                    Dim graveOrdinal As Integer = reader.GetOrdinal("VRA_GRAVE")
        '                    Dim esitoOrdinal As Integer = reader.GetOrdinal("VRA_ESITO")
        '                    Dim motivoDecessoOrdinal As Integer = reader.GetOrdinal("VRA_MOTIVO_DECESSO")
        '                    Dim scadutaOrdinal As Integer = reader.GetOrdinal("VRA_SCADUTA")
        '                    '--
        '                    While reader.Read
        '                        '--
        '                        Dim reazioneAvversaPaziente As New MovimentoCNS.ReazioneAvversaPaziente
        '                        '--
        '                        If Not reader.IsDBNull(codicePazientePrecedenteOrdinal) Then reazioneAvversaPaziente.CodicePazientePrecedente = reader.GetInt32(codicePazientePrecedenteOrdinal)
        '                        '--
        '                        reazioneAvversaPaziente.Vaccinazione = New MovimentoCNS.VaccinazioneInfo
        '                        reazioneAvversaPaziente.Vaccinazione.Codice = reader.GetString(codiceVaccinazioneOrdinal)
        '                        '--
        '                        If Not reader.IsDBNull(numeroRichiamoOrdinal) Then reazioneAvversaPaziente.NumeroRichiamo = reader.GetInt32(numeroRichiamoOrdinal)
        '                        If Not reader.IsDBNull(dataEffettuazioneOrdinal) Then reazioneAvversaPaziente.DataEffettuazione = reader.GetDateTime(dataEffettuazioneOrdinal)
        '                        If Not reader.IsDBNull(oraEffettuazioneOrdinal) Then reazioneAvversaPaziente.OraEffettuazione = reader.GetString(oraEffettuazioneOrdinal)
        '                        If Not reader.IsDBNull(dataReazioneOrdinal) Then reazioneAvversaPaziente.DataReazione = reader.GetDateTime(dataReazioneOrdinal)
        '                        If Not reader.IsDBNull(codiceReazioneOrdinal) Then
        '                            reazioneAvversaPaziente.Reazione = New MovimentoCNS.ReazioneInfo
        '                            reazioneAvversaPaziente.Reazione.Codice = reader.GetString(codiceReazioneOrdinal)
        '                        End If
        '                        If Not reader.IsDBNull(codiceReazione1Ordinal) Then
        '                            reazioneAvversaPaziente.Reazione1 = New MovimentoCNS.ReazioneInfo
        '                            reazioneAvversaPaziente.Reazione1.Codice = reader.GetString(codiceReazione1Ordinal)
        '                        End If
        '                        If Not reader.IsDBNull(codiceReazione2Ordinal) Then
        '                            reazioneAvversaPaziente.Reazione2 = New MovimentoCNS.ReazioneInfo
        '                            reazioneAvversaPaziente.Reazione2.Codice = reader.GetString(codiceReazione2Ordinal)
        '                        End If
        '                        If Not reader.IsDBNull(reazioneAltroOrdinal) Then reazioneAvversaPaziente.ReazioneAltro = reader.GetString(reazioneAltroOrdinal)
        '                        If Not reader.IsDBNull(gravitaReazioneOrdinal) Then reazioneAvversaPaziente.GravitaReazione = reader.GetString(gravitaReazioneOrdinal)
        '                        If Not reader.IsDBNull(visitaOrdinal) Then reazioneAvversaPaziente.Visita = reader.GetString(visitaOrdinal)
        '                        If Not reader.IsDBNull(terapiaOrdinal) Then reazioneAvversaPaziente.Terapia = reader.GetString(terapiaOrdinal)
        '                        If Not reader.IsDBNull(codiceEsitoTerapiaOrdinal) Then
        '                            reazioneAvversaPaziente.EsitoTerapia = New MovimentoCNS.EsitoTerapiaInfo
        '                            reazioneAvversaPaziente.EsitoTerapia.Codice = reader.GetString(codiceEsitoTerapiaOrdinal)
        '                        End If
        '                        If Not reader.IsDBNull(descrizioneNomeCommercialeOrdinal) Then reazioneAvversaPaziente.DescrizioneNomeCommerciale = reader.GetString(descrizioneNomeCommercialeOrdinal)
        '                        If Not reader.IsDBNull(codiceLottoOrdinal) Then
        '                            reazioneAvversaPaziente.Lotto = New MovimentoCNS.LottoInfo
        '                            reazioneAvversaPaziente.Lotto.Codice = reader.GetString(codiceLottoOrdinal)
        '                        End If
        '                        If Not reader.IsDBNull(dataScadenzaLottoOrdinal) Then reazioneAvversaPaziente.DataScadenzaLotto = reader.GetDateTime(dataScadenzaLottoOrdinal)
        '                        If Not reader.IsDBNull(dosaggioOrdinal) Then reazioneAvversaPaziente.Dosaggio = reader.GetString(dosaggioOrdinal)
        '                        If Not reader.IsDBNull(somministrazioneOrdinal) Then reazioneAvversaPaziente.Somministrazione = reader.GetString(somministrazioneOrdinal)
        '                        If Not reader.IsDBNull(codiceSitoInoculazioneOrdinal) Then
        '                            reazioneAvversaPaziente.SitoInoculazione = New MovimentoCNS.SitoInoculazioneInfo
        '                            reazioneAvversaPaziente.SitoInoculazione.Codice = reader.GetString(codiceSitoInoculazioneOrdinal)
        '                        End If
        '                        If Not reader.IsDBNull(sospesoOrdinal) Then reazioneAvversaPaziente.Sospeso = reader.GetString(sospesoOrdinal) = "S"
        '                        If Not reader.IsDBNull(migliorataOrdinal) Then reazioneAvversaPaziente.Migliorata = reader.GetString(migliorataOrdinal) = "S"
        '                        If Not reader.IsDBNull(ripresoOrdinal) Then reazioneAvversaPaziente.Ripreso = reader.GetString(ripresoOrdinal) = "S"
        '                        If Not reader.IsDBNull(ricomparsaOrdinal) Then reazioneAvversaPaziente.Ricomparsa = reader.GetString(ricomparsaOrdinal) = "S"
        '                        If Not reader.IsDBNull(indicazioniOrdinal) Then reazioneAvversaPaziente.Indicazioni = reader.GetString(indicazioniOrdinal)
        '                        If Not reader.IsDBNull(richiamoOrdinal) Then reazioneAvversaPaziente.Richiamo = reader.GetInt32(richiamoOrdinal)
        '                        If Not reader.IsDBNull(luogoOrdinal) Then reazioneAvversaPaziente.Luogo = reader.GetString(luogoOrdinal)
        '                        If Not reader.IsDBNull(altroLuogoOrdinal) Then reazioneAvversaPaziente.AltroLuogo = reader.GetString(altroLuogoOrdinal)
        '                        If Not reader.IsDBNull(farmacoConcomitanteOrdinal) Then reazioneAvversaPaziente.FarmacoConcomitante = reader.GetString(farmacoConcomitanteOrdinal) = "S"
        '                        If Not reader.IsDBNull(descrizioneFarmacoOrdinal) Then reazioneAvversaPaziente.DescrizioneFarmaco = reader.GetString(descrizioneFarmacoOrdinal)
        '                        If Not reader.IsDBNull(usoConcomitanteOrdinal) Then reazioneAvversaPaziente.UsoConcomitante = reader.GetString(usoConcomitanteOrdinal)
        '                        If Not reader.IsDBNull(condizioniConcomitantiOrdinal) Then reazioneAvversaPaziente.CondizioniConcomitanti = reader.GetString(condizioniConcomitantiOrdinal)
        '                        If Not reader.IsDBNull(qualificaOrdinal) Then reazioneAvversaPaziente.Qualifica = reader.GetString(qualificaOrdinal)
        '                        If Not reader.IsDBNull(altraQualificaOrdinal) Then reazioneAvversaPaziente.AltraQualifica = reader.GetString(altraQualificaOrdinal)
        '                        If Not reader.IsDBNull(cognomeSegnalatoreOrdinal) Then reazioneAvversaPaziente.CognomeSegnalatore = reader.GetString(cognomeSegnalatoreOrdinal)
        '                        If Not reader.IsDBNull(nomeSegnalatoreOrdinal) Then reazioneAvversaPaziente.NomeSegnalatore = reader.GetString(nomeSegnalatoreOrdinal)
        '                        If Not reader.IsDBNull(indirizzoSegnalatoreOrdinal) Then reazioneAvversaPaziente.IndirizzoSegnalatore = reader.GetString(indirizzoSegnalatoreOrdinal)
        '                        If Not reader.IsDBNull(telefonoSegnalatoreOrdinal) Then reazioneAvversaPaziente.TelefonoSegnalatore = reader.GetString(telefonoSegnalatoreOrdinal)
        '                        If Not reader.IsDBNull(faxSegnalatoreOrdinal) Then reazioneAvversaPaziente.FaxSegnalatore = reader.GetString(faxSegnalatoreOrdinal)
        '                        If Not reader.IsDBNull(mailSegnalatoreOrdinal) Then reazioneAvversaPaziente.MailSegnalatore = reader.GetString(mailSegnalatoreOrdinal)
        '                        If Not reader.IsDBNull(dataEsitoOrdinal) Then reazioneAvversaPaziente.DataEsito = reader.GetDateTime(dataEsitoOrdinal)
        '                        If Not reader.IsDBNull(dataCompilazioneOrdinal) Then reazioneAvversaPaziente.DataCompilazione = reader.GetDateTime(dataCompilazioneOrdinal)
        '                        If Not reader.IsDBNull(graveOrdinal) Then reazioneAvversaPaziente.Grave = reader.GetString(graveOrdinal) = "S"
        '                        If Not reader.IsDBNull(esitoOrdinal) Then reazioneAvversaPaziente.Esito = reader.GetString(esitoOrdinal)
        '                        If Not reader.IsDBNull(motivoDecessoOrdinal) Then reazioneAvversaPaziente.MotivoDecesso = reader.GetString(motivoDecessoOrdinal)
        '                        '--
        '                        reazioneAvversaPaziente.Scaduta = reader.GetString(scadutaOrdinal) = "S"
        '                        '--
        '                        reazioneAvversaPazienteCollection.Add(reazioneAvversaPaziente)
        '                        '--
        '                    End While
        '                    '--

        '                End If

        '            Finally

        '                If Not reader Is Nothing Then reader.Close()
        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '            Return reazioneAvversaPazienteCollection

        '        End Function

        '        Public Sub InserisciReazioniAvversePaziente(codicePaziente As String, reazioniAvversaPaziente As System.Collections.ObjectModel.Collection(Of Entities.MovimentoCNS.ReazioneAvversaPaziente)) Implements IMovimentiEsterniCNSProvider.InserisciReazioniAvversePaziente

        '            Dim cmd As OracleClient.OracleCommand = Nothing

        '            Dim ownTransaction As Boolean = False

        '            Try
        '                cmd = New OracleClient.OracleCommand()
        '                cmd.Connection = Me.Connection

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    ownTransaction = True
        '                    Me._DAM.BeginTrans()
        '                End If

        '                For Each reazioneAvversaPaziente As MovimentoCNS.ReazioneAvversaPaziente In reazioniAvversaPaziente

        '                    If reazioneAvversaPaziente.Scaduta Then
        '                        cmd.CommandText = "INSERT INTO T_VAC_REAZIONI_SCADUTE(VRS_PAZ_CODICE, VRS_PAZ_CODICE_OLD, VRS_VAC_CODICE,  VRS_N_RICHIAMO, VRS_RES_DATA_EFFETTUAZIONE,  VRS_ORA_EFFETTUAZIONE,  VRS_DATA_REAZIONE, VRS_REA_CODICE,  VRS_RE1_CODICE, VRS_RE2_CODICE, VRS_REA_ALTRO,  VRS_GRAVITA_REAZIONE, VRS_VISITA, VRS_TERAPIA,  VRS_ESI_CODICE, VRS_NOC_DESCRIZIONE, VRS_LOT_CODICE,  VRS_LOT_DATA_SCADENZA, VRS_DOSAGGIO, VRS_SOMMINISTRAZIONE,  VRS_SII_CODICE, VRS_SOSPESO, VRS_MIGLIORATA, VRS_RIPRESO,  VRS_RICOMPARSA, VRS_INDICAZIONI, VRS_RICHIAMO,  VRS_LUOGO,  VRS_ALTRO_LUOGO,  VRS_FARMACO_CONCOMITANTE, VRS_FARMACO_DESCRIZIONE, VRS_USO_CONCOMITANTE,  VRS_CONDIZIONI_CONCOMITANTI, VRS_QUALIFICA, VRS_ALTRA_QUALIFICA,  VRS_COGNOME_SEGNALATORE, VRS_NOME_SEGNALATORE,  VRS_INDIRIZZO_SEGNALATORE, VRS_TEL_SEGNALATORE,  VRS_FAX_SEGNALATORE, VRS_MAIL_SEGNALATORE, VRS_DATA_ESITO,  VRS_DATA_COMPILAZIONE, VRS_GRAVE, VRS_ESITO, VRS_MOTIVO_DECESSO) VALUES(:codicePaziente, :codicePazientePrecedente, :codiceVaccinazione, :numeroRichiamo, :dataEffettuazione, :oraEffettuazione, :dataReazione, :codiceReazione, :codiceReazione1, :codiceReazione2, :reazioneAltro, :gravitaReazione, :visita, :terapia, :codiceEsitoTerapia, :descrizioneNomeCommerciale, :codiceLotto, :dataScadenzaLotto, :dosaggio,  :somministrazione, :codiceSitoInoculazione, :sospeso, :migliorata, :ripreso, :ricomparsa, :indicazioni, :richiamo, :luogo, :altroLuogo, :farmacoConcomitante,  :descrizioneFarmaco, :usoConcomitante, :condizioniConcomitanti, :qualifica, :altraQualifica, :cognomeSegnalatore, :nomeSegnalatore, :indirizzoSegnalatore, :telefonoSegnalatore, :faxSegnalatore, :mailSegnalatore, :dataEsito, :dataCompilazione, :grave, :esito, :motivoDecesso)"
        '                    Else
        '                        cmd.CommandText = "INSERT INTO T_VAC_REAZIONI_AVVERSE(VRA_PAZ_CODICE, VRA_PAZ_CODICE_OLD, VRA_VAC_CODICE,  VRA_N_RICHIAMO, VRA_RES_DATA_EFFETTUAZIONE,  VRA_ORA_EFFETTUAZIONE,  VRA_DATA_REAZIONE, VRA_REA_CODICE,  VRA_RE1_CODICE, VRA_RE2_CODICE, VRA_REA_ALTRO,  VRA_GRAVITA_REAZIONE, VRA_VISITA, VRA_TERAPIA,  VRA_ESI_CODICE, VRA_NOC_DESCRIZIONE, VRA_LOT_CODICE,  VRA_LOT_DATA_SCADENZA, VRA_DOSAGGIO, VRA_SOMMINISTRAZIONE,  VRA_SII_CODICE, VRA_SOSPESO, VRA_MIGLIORATA, VRA_RIPRESO,  VRA_RICOMPARSA, VRA_INDICAZIONI, VRA_RICHIAMO,  VRA_LUOGO,  VRA_ALTRO_LUOGO,  VRA_FARMACO_CONCOMITANTE, VRA_FARMACO_DESCRIZIONE, VRA_USO_CONCOMITANTE,  VRA_CONDIZIONI_CONCOMITANTI, VRA_QUALIFICA, VRA_ALTRA_QUALIFICA,  VRA_COGNOME_SEGNALATORE, VRA_NOME_SEGNALATORE,  VRA_INDIRIZZO_SEGNALATORE, VRA_TEL_SEGNALATORE,  VRA_FAX_SEGNALATORE, VRA_MAIL_SEGNALATORE, VRA_DATA_ESITO,  VRA_DATA_COMPILAZIONE, VRA_GRAVE, VRA_ESITO, VRA_MOTIVO_DECESSO) VALUES(:codicePaziente, :codicePazientePrecedente, :codiceVaccinazione, :numeroRichiamo, :dataEffettuazione, :oraEffettuazione, :dataReazione, :codiceReazione, :codiceReazione1, :codiceReazione2, :reazioneAltro, :gravitaReazione, :visita, :terapia, :codiceEsitoTerapia, :descrizioneNomeCommerciale, :codiceLotto, :dataScadenzaLotto, :dosaggio,  :somministrazione, :codiceSitoInoculazione, :sospeso, :migliorata, :ripreso, :ricomparsa, :indicazioni, :richiamo, :luogo, :altroLuogo, :farmacoConcomitante,  :descrizioneFarmaco, :usoConcomitante, :condizioniConcomitanti, :qualifica, :altraQualifica, :cognomeSegnalatore, :nomeSegnalatore, :indirizzoSegnalatore, :telefonoSegnalatore, :faxSegnalatore, :mailSegnalatore, :dataEsito, :dataCompilazione, :grave, :esito, :motivoDecesso)"
        '                    End If

        '                    cmd.Parameters.Clear()

        '                    cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

        '                    If Not reazioneAvversaPaziente.CodicePazientePrecedente Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codicePazientePrecedente", reazioneAvversaPaziente.CodicePazientePrecedente)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codicePazientePrecedente", DBNull.Value)
        '                    End If

        '                    cmd.Parameters.AddWithValue("codiceVaccinazione", reazioneAvversaPaziente.Vaccinazione.Codice)
        '                    cmd.Parameters.AddWithValue("numeroRichiamo", reazioneAvversaPaziente.NumeroRichiamo)
        '                    cmd.Parameters.AddWithValue("dataEffettuazione", reazioneAvversaPaziente.DataEffettuazione)

        '                    If Not reazioneAvversaPaziente.OraEffettuazione Is Nothing Then
        '                        cmd.Parameters.AddWithValue("oraEffettuazione", reazioneAvversaPaziente.OraEffettuazione)
        '                    Else
        '                        cmd.Parameters.AddWithValue("oraEffettuazione", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.DataReazione Is Nothing Then
        '                        cmd.Parameters.AddWithValue("dataReazione", reazioneAvversaPaziente.DataReazione)
        '                    Else
        '                        cmd.Parameters.AddWithValue("dataReazione", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.Reazione Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codiceReazione", reazioneAvversaPaziente.Reazione.Codice)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codiceReazione", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.Reazione1 Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codiceReazione1", reazioneAvversaPaziente.Reazione1.Codice)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codiceReazione1", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.Reazione2 Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codiceReazione2", reazioneAvversaPaziente.Reazione2.Codice)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codiceReazione2", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.ReazioneAltro Is Nothing Then
        '                        cmd.Parameters.AddWithValue("reazioneAltro", reazioneAvversaPaziente.ReazioneAltro)
        '                    Else
        '                        cmd.Parameters.AddWithValue("reazioneAltro", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.GravitaReazione Is Nothing Then
        '                        cmd.Parameters.AddWithValue("gravitaReazione", reazioneAvversaPaziente.GravitaReazione)
        '                    Else
        '                        cmd.Parameters.AddWithValue("gravitaReazione", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.Visita Is Nothing Then
        '                        cmd.Parameters.AddWithValue("visita", reazioneAvversaPaziente.Visita)
        '                    Else
        '                        cmd.Parameters.AddWithValue("visita", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.Terapia Is Nothing Then
        '                        cmd.Parameters.AddWithValue("terapia", reazioneAvversaPaziente.Terapia)
        '                    Else
        '                        cmd.Parameters.AddWithValue("terapia", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.EsitoTerapia Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codiceEsitoTerapia", reazioneAvversaPaziente.EsitoTerapia.Codice)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codiceEsitoTerapia", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.DescrizioneNomeCommerciale Is Nothing Then
        '                        cmd.Parameters.AddWithValue("descrizioneNomeCommerciale", reazioneAvversaPaziente.DescrizioneNomeCommerciale)
        '                    Else
        '                        cmd.Parameters.AddWithValue("descrizioneNomeCommerciale", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.Lotto Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codiceLotto", reazioneAvversaPaziente.Lotto.Codice)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codiceLotto", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.DataScadenzaLotto Is Nothing Then
        '                        cmd.Parameters.AddWithValue("dataScadenzaLotto", reazioneAvversaPaziente.DataScadenzaLotto)
        '                    Else
        '                        cmd.Parameters.AddWithValue("dataScadenzaLotto", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.Dosaggio Is Nothing Then
        '                        cmd.Parameters.AddWithValue("dosaggio", reazioneAvversaPaziente.Dosaggio)
        '                    Else
        '                        cmd.Parameters.AddWithValue("dosaggio", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.Somministrazione Is Nothing Then
        '                        cmd.Parameters.AddWithValue("somministrazione", reazioneAvversaPaziente.Somministrazione)
        '                    Else
        '                        cmd.Parameters.AddWithValue("somministrazione", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.SitoInoculazione Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codiceSitoInoculazione", reazioneAvversaPaziente.SitoInoculazione.Codice)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codiceSitoInoculazione", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.Sospeso Is Nothing Then
        '                        cmd.Parameters.AddWithValue("sospeso", IIf(reazioneAvversaPaziente.Sospeso, "S", "N"))
        '                    Else
        '                        cmd.Parameters.AddWithValue("sospeso", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.Migliorata Is Nothing Then
        '                        cmd.Parameters.AddWithValue("migliorata", IIf(reazioneAvversaPaziente.Migliorata, "S", "N"))
        '                    Else
        '                        cmd.Parameters.AddWithValue("migliorata", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.Ripreso Is Nothing Then
        '                        cmd.Parameters.AddWithValue("ripreso", IIf(reazioneAvversaPaziente.Ripreso, "S", "N"))
        '                    Else
        '                        cmd.Parameters.AddWithValue("ripreso", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.Ricomparsa Is Nothing Then
        '                        cmd.Parameters.AddWithValue("ricomparsa", IIf(reazioneAvversaPaziente.Ricomparsa, "S", "N"))
        '                    Else
        '                        cmd.Parameters.AddWithValue("ricomparsa", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.Indicazioni Is Nothing Then
        '                        cmd.Parameters.AddWithValue("indicazioni", reazioneAvversaPaziente.Indicazioni)
        '                    Else
        '                        cmd.Parameters.AddWithValue("indicazioni", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.Richiamo Is Nothing Then
        '                        cmd.Parameters.AddWithValue("richiamo", reazioneAvversaPaziente.Richiamo)
        '                    Else
        '                        cmd.Parameters.AddWithValue("richiamo", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.Luogo Is Nothing Then
        '                        cmd.Parameters.AddWithValue("luogo", reazioneAvversaPaziente.Luogo)
        '                    Else
        '                        cmd.Parameters.AddWithValue("luogo", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.AltroLuogo Is Nothing Then
        '                        cmd.Parameters.AddWithValue("altroLuogo", reazioneAvversaPaziente.AltroLuogo)
        '                    Else
        '                        cmd.Parameters.AddWithValue("altroLuogo", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.FarmacoConcomitante Is Nothing Then
        '                        cmd.Parameters.AddWithValue("farmacoConcomitante", IIf(reazioneAvversaPaziente.FarmacoConcomitante, "S", "N"))
        '                    Else
        '                        cmd.Parameters.AddWithValue("farmacoConcomitante", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.DescrizioneFarmaco Is Nothing Then
        '                        cmd.Parameters.AddWithValue("descrizioneFarmaco", reazioneAvversaPaziente.DescrizioneFarmaco)
        '                    Else
        '                        cmd.Parameters.AddWithValue("descrizioneFarmaco", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.UsoConcomitante Is Nothing Then
        '                        cmd.Parameters.AddWithValue("usoConcomitante", reazioneAvversaPaziente.UsoConcomitante)
        '                    Else
        '                        cmd.Parameters.AddWithValue("usoConcomitante", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.CondizioniConcomitanti Is Nothing Then
        '                        cmd.Parameters.AddWithValue("condizioniConcomitanti", reazioneAvversaPaziente.CondizioniConcomitanti)
        '                    Else
        '                        cmd.Parameters.AddWithValue("condizioniConcomitanti", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.Qualifica Is Nothing Then
        '                        cmd.Parameters.AddWithValue("qualifica", reazioneAvversaPaziente.Qualifica)
        '                    Else
        '                        cmd.Parameters.AddWithValue("qualifica", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.AltraQualifica Is Nothing Then
        '                        cmd.Parameters.AddWithValue("altraQualifica", reazioneAvversaPaziente.AltraQualifica)
        '                    Else
        '                        cmd.Parameters.AddWithValue("altraQualifica", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.CognomeSegnalatore Is Nothing Then
        '                        cmd.Parameters.AddWithValue("cognomeSegnalatore", reazioneAvversaPaziente.CognomeSegnalatore)
        '                    Else
        '                        cmd.Parameters.AddWithValue("cognomeSegnalatore", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.NomeSegnalatore Is Nothing Then
        '                        cmd.Parameters.AddWithValue("nomeSegnalatore", reazioneAvversaPaziente.NomeSegnalatore)
        '                    Else
        '                        cmd.Parameters.AddWithValue("nomeSegnalatore", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.IndirizzoSegnalatore Is Nothing Then
        '                        cmd.Parameters.AddWithValue("indirizzoSegnalatore", reazioneAvversaPaziente.IndirizzoSegnalatore)
        '                    Else
        '                        cmd.Parameters.AddWithValue("indirizzoSegnalatore", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.TelefonoSegnalatore Is Nothing Then
        '                        cmd.Parameters.AddWithValue("telefonoSegnalatore", reazioneAvversaPaziente.TelefonoSegnalatore)
        '                    Else
        '                        cmd.Parameters.AddWithValue("telefonoSegnalatore", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.FaxSegnalatore Is Nothing Then
        '                        cmd.Parameters.AddWithValue("faxSegnalatore", reazioneAvversaPaziente.FaxSegnalatore)
        '                    Else
        '                        cmd.Parameters.AddWithValue("faxSegnalatore", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.MailSegnalatore Is Nothing Then
        '                        cmd.Parameters.AddWithValue("mailSegnalatore", reazioneAvversaPaziente.MailSegnalatore)
        '                    Else
        '                        cmd.Parameters.AddWithValue("mailSegnalatore", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.DataEsito Is Nothing Then
        '                        cmd.Parameters.AddWithValue("dataEsito", reazioneAvversaPaziente.DataEsito)
        '                    Else
        '                        cmd.Parameters.AddWithValue("dataEsito", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.DataCompilazione Is Nothing Then
        '                        cmd.Parameters.AddWithValue("dataCompilazione", reazioneAvversaPaziente.DataCompilazione)
        '                    Else
        '                        cmd.Parameters.AddWithValue("dataCompilazione", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.Grave Is Nothing Then
        '                        cmd.Parameters.AddWithValue("grave", IIf(reazioneAvversaPaziente.Grave, "S", "N"))
        '                    Else
        '                        cmd.Parameters.AddWithValue("grave", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.Esito Is Nothing Then
        '                        cmd.Parameters.AddWithValue("esito", reazioneAvversaPaziente.Esito)
        '                    Else
        '                        cmd.Parameters.AddWithValue("esito", DBNull.Value)
        '                    End If

        '                    If Not reazioneAvversaPaziente.MotivoDecesso Is Nothing Then
        '                        cmd.Parameters.AddWithValue("motivoDecesso", reazioneAvversaPaziente.MotivoDecesso)
        '                    Else
        '                        cmd.Parameters.AddWithValue("motivoDecesso", DBNull.Value)
        '                    End If

        '                    cmd.ExecuteNonQuery()

        '                Next

        '                If ownTransaction Then
        '                    Me.Transaction.Commit()
        '                End If

        '            Catch ex As Exception

        '                If ownTransaction Then
        '                    Me.Transaction.Rollback()
        '                End If

        '                ex.InternalPreserveStackTrace()
        '                Throw

        '            Finally

        '                If Not Me.Transaction Is Nothing AndAlso ownTransaction Then Me.Transaction.Dispose()
        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '        End Sub

        '#End Region

        '#Region " Bilanci "

        '        Public Function LoadBilanciProgrammatiPaziente(codicePaziente As String) As System.Collections.ObjectModel.Collection(Of Entities.MovimentoCNS.BilancioProgrammatoPaziente) Implements IMovimentiEsterniCNSProvider.LoadBilanciProgrammatiPaziente

        '            Dim bilancioProgrammatoPazienteCollection As New Collection(Of MovimentoCNS.BilancioProgrammatoPaziente)

        '            Dim cmd As OracleClient.OracleCommand = Nothing
        '            Dim reader As IDataReader = Nothing

        '            Try
        '                cmd = New OracleClient.OracleCommand("SELECT * FROM T_BIL_PROGRAMMATI LEFT OUTER JOIN T_BIL_SOLLECITI ON T_BIL_PROGRAMMATI.ID = T_BIL_SOLLECITI.BIS_BIP_ID WHERE BIP_PAZ_CODICE = :codicePaziente ORDER BY T_BIL_PROGRAMMATI.ID", Me.Connection)

        '                cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    Me.conditionalOpenConnection()
        '                End If

        '                reader = cmd.ExecuteReader()

        '                If Not reader Is Nothing Then

        '                    '--
        '                    Dim codicePazientePrecedenteOrdinal As Integer = reader.GetOrdinal("BIP_PAZ_CODICE_OLD")
        '                    Dim numeroOrdinal As Integer = reader.GetOrdinal("BIP_BIL_NUMERO")
        '                    Dim codiceMalattiaOrdinal As Integer = reader.GetOrdinal("BIP_MAL_CODICE")
        '                    Dim dataConvocazioneOrdinal As Integer = reader.GetOrdinal("BIP_CNV_DATA")
        '                    Dim dataInvioOrdinal As Integer = reader.GetOrdinal("BIP_DATA_INVIO")
        '                    Dim statoOrdinal As Integer = reader.GetOrdinal("BIP_STATO")
        '                    '--
        '                    Dim idBilancioSollecitoOrdinal As Integer = reader.GetOrdinal("BIS_BIP_ID")
        '                    Dim dataInvioSollecitoOrdinal As Integer = reader.GetOrdinal("BIS_DATA_INVIO")
        '                    '--
        '                    Dim bilancioProgrammatoPaziente As MovimentoCNS.BilancioProgrammatoPaziente = Nothing
        '                    '--
        '                    While reader.Read
        '                        '--
        '                        Dim numero As Int16 = Convert.ToInt16(reader.GetInt32(numeroOrdinal))
        '                        Dim codiceMalattia As String = reader.GetString(codiceMalattiaOrdinal)
        '                        '--
        '                        If bilancioProgrammatoPaziente Is Nothing OrElse bilancioProgrammatoPaziente.Numero <> numero OrElse bilancioProgrammatoPaziente.Malattia.Codice <> codiceMalattia Then
        '                            '--
        '                            bilancioProgrammatoPaziente = New MovimentoCNS.BilancioProgrammatoPaziente()
        '                            bilancioProgrammatoPaziente.Malattia = New MovimentoCNS.MalattiaInfo()
        '                            bilancioProgrammatoPaziente.Malattia.Codice = codiceMalattia
        '                            bilancioProgrammatoPaziente.Numero = numero
        '                            If Not reader.IsDBNull(codicePazientePrecedenteOrdinal) Then bilancioProgrammatoPaziente.CodicePazientePrecedente = reader.GetInt32(codicePazientePrecedenteOrdinal)
        '                            If Not reader.IsDBNull(dataConvocazioneOrdinal) Then bilancioProgrammatoPaziente.DataConvocazione = reader.GetDateTime(dataConvocazioneOrdinal)
        '                            If Not reader.IsDBNull(dataInvioOrdinal) Then bilancioProgrammatoPaziente.DataInvio = reader.GetDateTime(dataInvioOrdinal)
        '                            If Not reader.IsDBNull(statoOrdinal) Then bilancioProgrammatoPaziente.Stato = reader.GetString(statoOrdinal)
        '                            '--
        '                            bilancioProgrammatoPazienteCollection.Add(bilancioProgrammatoPaziente)
        '                            '--
        '                        End If
        '                        '--
        '                        If Not reader.IsDBNull(idBilancioSollecitoOrdinal) Then
        '                            '--
        '                            Dim sollecitoBilancioProgrammatoPaziente As New MovimentoCNS.SollecitoBilancioProgrammatoPaziente
        '                            '--
        '                            If Not reader.IsDBNull(dataInvioSollecitoOrdinal) Then sollecitoBilancioProgrammatoPaziente.DataInvio = reader.GetDateTime(dataInvioSollecitoOrdinal)
        '                            '--
        '                            bilancioProgrammatoPaziente.SollecitiBilancioProgrammato.Add(sollecitoBilancioProgrammatoPaziente)
        '                            '--
        '                        End If
        '                        '--
        '                    End While
        '                    '--

        '                End If

        '            Finally

        '                If Not reader Is Nothing Then reader.Close()
        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '            Return bilancioProgrammatoPazienteCollection

        '        End Function

        '        Public Function CountBilanciProgrammatiPaziente(codicePaziente As String) As Integer Implements IMovimentiEsterniCNSProvider.CountBilanciProgrammatiPaziente

        '            Dim cmd As OracleClient.OracleCommand = Nothing

        '            Try
        '                cmd = New OracleClient.OracleCommand("SELECT COUNT(*) FROM T_BIL_PROGRAMMATI WHERE BIP_PAZ_CODICE = :codicePaziente", Me.Connection)

        '                cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    Me.conditionalOpenConnection()
        '                End If

        '                Return Convert.ToInt32(cmd.ExecuteScalar())

        '            Finally

        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '        End Function

        '        Public Sub EliminaBilanciProgrammatiPaziente(codicePaziente As String) Implements IMovimentiEsterniCNSProvider.EliminaBilanciProgrammatiPaziente

        '            Dim cmd As OracleClient.OracleCommand = Nothing

        '            Dim ownTransaction As Boolean = False

        '            Try

        '                cmd = New OracleClient.OracleCommand("DELETE FROM T_BIL_SOLLECITI WHERE BIS_BIP_ID IN (SELECT ID FROM T_BIL_PROGRAMMATI WHERE BIP_PAZ_CODICE = :codicePaziente)", Me.Connection)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    ownTransaction = True
        '                    Me._DAM.BeginTrans()
        '                End If

        '                cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)

        '                cmd.ExecuteNonQuery()

        '                cmd.CommandText = "DELETE FROM T_BIL_PROGRAMMATI WHERE BIP_PAZ_CODICE = :codicePaziente"

        '                cmd.ExecuteNonQuery()

        '                If ownTransaction Then
        '                    Me._DAM.Commit()
        '                End If

        '            Catch ex As Exception

        '                If ownTransaction Then
        '                    Me._DAM.Rollback()
        '                End If

        '                ex.InternalPreserveStackTrace()
        '                Throw

        '            Finally

        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '        End Sub

        '        Public Sub InserisciBilanciProgrammatiPaziente(codicePaziente As String, bilanciProgrammatiPaziente As System.Collections.ObjectModel.Collection(Of Entities.MovimentoCNS.BilancioProgrammatoPaziente)) Implements IMovimentiEsterniCNSProvider.InserisciBilanciProgrammatiPaziente

        '            Dim cmd As OracleClient.OracleCommand = Nothing

        '            Dim ownTransaction As Boolean = False

        '            Try
        '                cmd = New OracleClient.OracleCommand()

        '                cmd.Connection = Me.Connection

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    ownTransaction = True
        '                    Me._DAM.BeginTrans()
        '                End If

        '                For Each bilancioProgrammatoPaziente As MovimentoCNS.BilancioProgrammatoPaziente In bilanciProgrammatiPaziente

        '                    cmd.CommandText = "INSERT INTO T_BIL_PROGRAMMATI(BIP_PAZ_CODICE, BIP_PAZ_CODICE_OLD, BIP_BIL_NUMERO, BIP_MAL_CODICE, BIP_CNV_DATA, BIP_DATA_INVIO, BIP_STATO) VALUES(:codicePaziente, :codicePazientePrecedente, :numeroBilancio, :codiceMalattia, :dataConvocazione, :dataInvio, :stato)"

        '                    cmd.Parameters.Clear()

        '                    cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)
        '                    cmd.Parameters.AddWithValue("codiceMalattia", bilancioProgrammatoPaziente.Malattia.Codice)
        '                    cmd.Parameters.AddWithValue("numeroBilancio", bilancioProgrammatoPaziente.Numero)

        '                    If Not bilancioProgrammatoPaziente.CodicePazientePrecedente Is Nothing Then
        '                        cmd.Parameters.AddWithValue("codicePazientePrecedente", bilancioProgrammatoPaziente.CodicePazientePrecedente)
        '                    Else
        '                        cmd.Parameters.AddWithValue("codicePazientePrecedente", DBNull.Value)
        '                    End If

        '                    If Not bilancioProgrammatoPaziente.DataConvocazione Is Nothing Then
        '                        cmd.Parameters.AddWithValue("dataConvocazione", bilancioProgrammatoPaziente.DataConvocazione)
        '                    Else
        '                        cmd.Parameters.AddWithValue("dataConvocazione", DBNull.Value)
        '                    End If

        '                    If Not bilancioProgrammatoPaziente.DataInvio Is Nothing Then
        '                        cmd.Parameters.AddWithValue("dataInvio", bilancioProgrammatoPaziente.DataInvio)
        '                    Else
        '                        cmd.Parameters.AddWithValue("dataInvio", DBNull.Value)
        '                    End If

        '                    If Not bilancioProgrammatoPaziente.Stato Is Nothing Then
        '                        cmd.Parameters.AddWithValue("stato", bilancioProgrammatoPaziente.Stato)
        '                    Else
        '                        cmd.Parameters.AddWithValue("stato", DBNull.Value)
        '                    End If

        '                    cmd.ExecuteNonQuery()

        '                    cmd.CommandText = "SELECT ID FROM T_BIL_PROGRAMMATI WHERE BIP_PAZ_CODICE = :codicePaziente AND BIP_MAL_CODICE = :codiceMalattia AND BIP_BIL_NUMERO = :numeroBilancio"

        '                    cmd.Parameters.Clear()

        '                    cmd.Parameters.AddWithValue("codicePaziente", codicePaziente)
        '                    cmd.Parameters.AddWithValue("codiceMalattia", bilancioProgrammatoPaziente.Malattia.Codice)
        '                    cmd.Parameters.AddWithValue("numeroBilancio", bilancioProgrammatoPaziente.Numero)

        '                    Dim idBilancio As Integer = cmd.ExecuteScalar()

        '                    For Each sollecitoBilancioProgrammatoPaziente As MovimentoCNS.SollecitoBilancioProgrammatoPaziente In bilancioProgrammatoPaziente.SollecitiBilancioProgrammato

        '                        cmd.CommandText = "INSERT INTO T_BIL_SOLLECITI(BIS_BIP_ID, BIS_DATA_INVIO) VALUES(:idBilancio, :dataInvio)"

        '                        cmd.Parameters.Clear()

        '                        cmd.Parameters.AddWithValue("idBilancio", idBilancio)

        '                        If Not sollecitoBilancioProgrammatoPaziente.DataInvio Is Nothing Then
        '                            cmd.Parameters.AddWithValue("dataInvio", sollecitoBilancioProgrammatoPaziente.DataInvio)
        '                        Else
        '                            cmd.Parameters.AddWithValue("dataInvio", DBNull.Value)
        '                        End If

        '                        cmd.ExecuteNonQuery()

        '                    Next

        '                Next

        '                If ownTransaction Then
        '                    Me.Transaction.Commit()
        '                End If

        '            Catch ex As Exception

        '                If ownTransaction Then
        '                    Me.Transaction.Rollback()
        '                End If

        '                ex.InternalPreserveStackTrace()
        '                Throw

        '            Finally

        '                If Not Me.Transaction Is Nothing AndAlso ownTransaction Then Me.Transaction.Dispose()
        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '        End Sub

        '#End Region

        '#Region " Consultorio / Applicazione "

        '        Public Function LoadConsultorioInfoByCodiceComune(codiceComune As String) As Entities.MovimentoCNS.ConsultorioInfo Implements IMovimentiEsterniCNSProvider.LoadConsultorioInfoByCodiceComune

        '            Dim consultorioInfo As MovimentoCNS.ConsultorioInfo = Nothing

        '            Dim cmd As OracleClient.OracleCommand = Nothing
        '            Dim reader As IDataReader = Nothing

        '            Try
        '                cmd = New OracleClient.OracleCommand("SELECT CNS_CODICE, CNS_DESCRIZIONE FROM T_ANA_CONSULTORI WHERE CNS_COM_CODICE = :codiceComune", Me.Connection)

        '                cmd.Parameters.AddWithValue("codiceComune", codiceComune)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    Me.conditionalOpenConnection()
        '                End If

        '                reader = cmd.ExecuteReader()

        '                If reader.Read Then
        '                    '--
        '                    Dim codiceOrdinal As Integer = reader.GetOrdinal("CNS_CODICE")
        '                    Dim descrizioneOrdinal As Integer = reader.GetOrdinal("CNS_DESCRIZIONE")
        '                    '--
        '                    consultorioInfo = New MovimentoCNS.ConsultorioInfo()
        '                    consultorioInfo.Codice = reader.GetString(codiceOrdinal)
        '                    consultorioInfo.Descrizione = reader.GetString(reader.GetOrdinal(descrizioneOrdinal))
        '                    '--

        '                End If

        '            Finally

        '                If Not reader Is Nothing Then reader.Close()
        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '            Return consultorioInfo

        '        End Function

        '        Public Function LoadConsultorioInfoSmistamento() As Entities.MovimentoCNS.ConsultorioInfo Implements IMovimentiEsterniCNSProvider.LoadConsultorioInfoSmistamento

        '            Dim consultorioInfo As MovimentoCNS.ConsultorioInfo = Nothing

        '            Dim cmd As OracleClient.OracleCommand = Nothing
        '            Dim reader As IDataReader = Nothing

        '            Try
        '                cmd = New OracleClient.OracleCommand("SELECT CNS_CODICE, CNS_DESCRIZIONE FROM T_ANA_CONSULTORI WHERE CNS_SMISTAMENTO = 'S'", Me.Connection)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    Me.conditionalOpenConnection()
        '                End If

        '                reader = cmd.ExecuteReader()

        '                If reader.Read Then
        '                    '--
        '                    Dim codiceOrdinal As Integer = reader.GetOrdinal("CNS_CODICE")
        '                    Dim descrizioneOrdinal As Integer = reader.GetOrdinal("CNS_DESCRIZIONE")
        '                    '--
        '                    consultorioInfo = New MovimentoCNS.ConsultorioInfo()
        '                    consultorioInfo.Codice = reader.GetString(codiceOrdinal)
        '                    consultorioInfo.Descrizione = reader.GetString(descrizioneOrdinal)
        '                    '--

        '                End If

        '            Finally

        '                If Not reader Is Nothing Then reader.Close()
        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '            Return consultorioInfo

        '        End Function

        '        Public Function LoadIDApplicazioneUslGestitaByCodiceComune(codiceComune As String) As String Implements IMovimentiEsterniCNSProvider.LoadIDApplicazioneUslGestitaByCodiceComune

        '            Dim cmd As OracleClient.OracleCommand = Nothing

        '            Try
        '                cmd = New OracleClient.OracleCommand("SELECT UGS_APP_ID FROM T_USL_GESTITE INNER JOIN T_ANA_LINK_COMUNI_USL ON UGS_USL_CODICE = LCU_USL_CODICE WHERE LCU_COM_CODICE = :codiceComune", Me.Connection)

        '                cmd.Parameters.AddWithValue("codiceComune", codiceComune)

        '                If Not Me.Transaction Is Nothing Then
        '                    cmd.Transaction = Me.Transaction
        '                Else
        '                    Me.conditionalOpenConnection()
        '                End If

        '                Return cmd.ExecuteScalar()

        '            Finally

        '                If Not cmd Is Nothing Then cmd.Dispose()

        '            End Try

        '        End Function

        '#End Region

    End Class

End Namespace

