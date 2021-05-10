Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports Onit.Database.DataAccessManager

Namespace Common.Solleciti

    <Serializable()>
    Public Class ControlloSolleciti

#Region " Private "

        Private ReadOnly Settings As Settings.Settings
        Private ReadOnly FlagConsensoVaccUslCorrente As Boolean
        Private ReadOnly FlagAbilitazioneVaccUslCorrente As Boolean

#End Region

#Region " Constructors "

        ' [Unificazione Ulss]: i flag Consenso e Abilitazione vengono sempre passati con il valore false per le ulss unificate => OK
        Public Sub New(settings As Settings.Settings, flagConsensoVaccUslCorrente As Boolean, flagAbilitazioneVaccUslCorrente As Boolean)
            Me.Settings = settings
            Me.FlagConsensoVaccUslCorrente = flagConsensoVaccUslCorrente
            Me.FlagAbilitazioneVaccUslCorrente = flagAbilitazioneVaccUslCorrente
        End Sub

#End Region

#Region " Properties "

        Private _PazientiCicliObbligatoriNoTP As ArrayList
        Public ReadOnly Property PazientiCicliObbligatoriNoTP() As ArrayList
            Get
                Return _PazientiCicliObbligatoriNoTP
            End Get
        End Property

        Private _PazientiCicliObbligatoriInTP As ArrayList
        Public ReadOnly Property PazientiCicliObbligatoriInTP() As ArrayList
            Get
                Return _PazientiCicliObbligatoriInTP
            End Get
        End Property

        Private _PazientiCicliRaccomandatiTerminati As ArrayList
        Public ReadOnly Property PazientiCicliRaccomandatiTerminati() As ArrayList
            Get
                Return _PazientiCicliRaccomandatiTerminati
            End Get
        End Property

        Private _PazientiCicliNonObbligatori As ArrayList
        Public ReadOnly Property PazientiCicliNonObbligatori() As ArrayList
            Get
                Return _PazientiCicliNonObbligatori
            End Get
        End Property

        Private _PazientiNoCicli As ArrayList
        Public ReadOnly Property PazientiNoCicli() As ArrayList
            Get
                Return _PazientiNoCicli
            End Get
        End Property

        Private _PazientiDisallineati As ArrayList
        Public ReadOnly Property PazientiDisallineati() As ArrayList
            Get
                Return _PazientiDisallineati
            End Get
        End Property

#End Region

#Region " Public Methods "

        Public Sub CaricaPazienti(cnsCodice As String)

            _PazientiCicliObbligatoriNoTP = New ArrayList()
            _PazientiCicliObbligatoriInTP = New ArrayList()
            _PazientiCicliRaccomandatiTerminati = New ArrayList()
            _PazientiCicliNonObbligatori = New ArrayList()
            _PazientiNoCicli = New ArrayList()
            _PazientiDisallineati = New ArrayList()

            Using dam As IDAM = OnVacUtility.OpenDam()

                Dim dtStatiAnag As DataTable = Nothing
                Dim dataOdierna As DateTime = Date.Now.Date
                Dim dataFinestra As DateTime = dataOdierna.AddDays(-Math.Min(Math.Min(Settings.TEMPORIT, Settings.TEMPOINADEMPIENZA), Settings.TEMPOESCLUSIONE))

                Using genericProvider As New DAL.DbGenericProvider(dam)
                    Using bizStatiAnagrafici As New Biz.BizStatiAnagrafici(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                        dtStatiAnag = bizStatiAnagrafici.LeggiStatiAnagrafici()

                    End Using
                End Using

                dtStatiAnag.PrimaryKey = New DataColumn() {dtStatiAnag.Columns("SAN_CODICE")}

                With dam.QB

                    .NewQuery()
                    .AddSelectFields("1")
                    .AddTables("t_vac_escluse")
                    .AddWhereCondition("vex_paz_codice", Comparatori.Uguale, "cnv_paz_codice", DataTypes.Join)
                    .AddWhereCondition("vex_vac_codice", Comparatori.Uguale, "vpr_vac_codice", DataTypes.Join)
                    .OpenParanthesis()
                    .AddWhereCondition("vex_data_scadenza", Comparatori.Is, "NULL", DataTypes.Replace)
                    .AddWhereCondition("vex_data_scadenza", Comparatori.Maggiore, DateTime.Today, DataTypes.Data, "OR")
                    .CloseParanthesis()

                    Dim strQueryVacEscluse As String = .GetSelect()

                    .NewQuery(False, True)
                    .AddSelectFields("1")
                    .AddTables("t_vac_eseguite")
                    .AddWhereCondition("ves_paz_codice", Comparatori.Uguale, "cnv_paz_codice", DataTypes.Join)
                    .AddWhereCondition("ves_vac_codice", Comparatori.Uguale, "vpr_vac_codice", DataTypes.Join)
                    .AddWhereCondition("ves_n_richiamo", Comparatori.Uguale, "vpr_n_richiamo", DataTypes.Join)

                    Dim strQueryVacEseguite As String = .GetSelect()

                    .NewQuery(False, True)
                    .AddSelectFields("SAN_CODICE")
                    .AddTables("t_ana_stati_anagrafici")
                    .AddWhereCondition("san_chiamata", Comparatori.Uguale, "S", DataTypes.Stringa)

                    Dim strQueryStatiAnagrafici As String = .GetSelect()

                    .NewQuery(False, True)
                    .AddSelectFields("'S'")
                    .IsDistinct = True
                    .AddTables("V_ANA_ASS_VACC_SEDUTE", "T_ANA_VACCINAZIONI")
                    .AddWhereCondition("SAS_VAC_CODICE", Comparatori.Uguale, "VAC_CODICE", DataTypes.Join)
                    .AddWhereCondition("VAC_OBBLIGATORIA", Comparatori.Uguale, "A", DataTypes.Stringa)
                    .AddWhereCondition("SAS_CIC_CODICE", Comparatori.Uguale, "CNC_CIC_CODICE", DataTypes.Replace)
                    .AddWhereCondition("SAS_N_SEDUTA", Comparatori.Uguale, "CNC_SED_N_SEDUTA", DataTypes.Replace)

                    Dim querySelectSedutaCicloObbligatoria As String = .GetSelect()

                    .NewQuery(False, True)
                    .AddSelectFields("MAX(VIS_FINE_SOSPENSIONE) VIS_FINE_SOSPENSIONE")
                    .AddTables("T_VIS_VISITE")
                    .AddWhereCondition("VIS_PAZ_CODICE", Comparatori.Uguale, "CNV_PAZ_CODICE", DataTypes.Replace)

                    Dim querySelectFineSospensione As String = .GetSelect()

                    .NewQuery(False, True)
                    .AddSelectFields("paz_codice Codice", "paz_cognome Cognome", "paz_nome Nome", "paz_data_nascita DataNascita", "paz_stato_anagrafico CodiceStatoAnagrafico", "PAZ_STATO_ACQUISIZIONE")
                    .AddSelectFields("cnv_data_appuntamento DataAppuntamento", "cnv_data DataConvocazione", "cnv_data_invio DataInvio", "cnc_data_invio_sollecito")
                    .AddSelectFields("vpr_cic_codice CodiceCiclo", "vpr_n_seduta NumeroSedutaCiclo", "(" & querySelectSedutaCicloObbligatoria & ") SedutaCicloObbligatoria")
                    .AddSelectFields("vac_codice VaccinazioneCodice", "vac_obbligatoria VaccinazioneObbligatoria")
                    .AddSelectFields(.FC.IsNull("tsd_num_solleciti", 0, DataTypes.Numero) & " TsdNumSolleciti")
                    .AddSelectFields(.FC.IsNull("tsd_num_solleciti_rac", 0, DataTypes.Numero) & " TsdNumSollecitiRac")
                    .AddSelectFields(.FC.IsNull("tsd_giorni_posticipo", 0, DataTypes.Numero) & " TsdGiorniPosticipo")
                    .AddSelectFields(.FC.IsNull("tsd_num_soll_non_obbl", 0, DataTypes.Numero) & " tsd_num_soll_non_obbl")
                    .AddSelectFields(.FC.IsNull("tsd_posticipo_seduta", "N", DataTypes.Stringa) & " tsd_posticipo_seduta")
                    .AddSelectFields(.FC.IsNull("cnc_n_sollecito", 0, DataTypes.Numero) & " cnc_n_sollecito")
                    .AddSelectFields(.FC.IsNull("cnc_flag_giorni_posticipo", "N", DataTypes.Stringa) & " cnc_flag_giorni_posticipo")
                    .AddSelectFields(.FC.IsNull("cnc_flag_posticipo_seduta", "N", DataTypes.Stringa) & " cnc_flag_posticipo_seduta")
                    .AddTables("t_paz_pazienti", "t_cnv_convocazioni", "t_vac_programmate", "t_cnv_cicli")
                    .AddTables("t_ana_tempi_sedute", "t_ana_vaccinazioni")

                    .OpenParanthesis()
                    .AddWhereCondition("cnv_data_appuntamento", Comparatori.Minore, dataFinestra, DataTypes.DataOra)
                    .AddWhereCondition(dam.QB.FC.Tronca("cnv_data_appuntamento"), Comparatori.Maggiore, dam.QB.FC.IsNull("(" & querySelectFineSospensione & ")", New DateTime(1901, 1, 1), DataTypes.Data), DataTypes.Replace)
                    .OpenParanthesis()
                    .AddWhereCondition("cnv_data_appuntamento", Comparatori.Is, "NULL", DataTypes.Replace, "OR")
                    .AddWhereCondition("cnc_n_sollecito", Comparatori.Maggiore, .FC.Switch(.FC.IsNull("tsd_num_solleciti", 0, DataTypes.Numero), "0", Me.Settings.NUMSOL.ToString(), "tsd_num_solleciti"), DataTypes.Replace)
                    .AddWhereCondition(.FC.IsNull("tsd_num_solleciti_rac", 0, DataTypes.Numero), Comparatori.Uguale, 0, DataTypes.Numero)
                    .AddWhereCondition("'S'", Comparatori.Uguale, "(" & querySelectSedutaCicloObbligatoria & ")", DataTypes.Replace)
                    .AddWhereCondition("cnc_data_invio_sollecito", Comparatori.Minore, dataFinestra, DataTypes.DataOra)
                    .AddWhereCondition(.FC.Now, Comparatori.Maggiore, dam.QB.FC.IsNull("(" & querySelectFineSospensione & ")", New DateTime(1901, 1, 1), DataTypes.Data), DataTypes.Replace)
                    .CloseParanthesis()
                    .CloseParanthesis()
                    .AddWhereCondition("cnv_paz_codice", Comparatori.Uguale, "paz_codice", DataTypes.Join)
                    .AddWhereCondition("cnv_paz_codice", Comparatori.Uguale, "vpr_paz_codice", DataTypes.Join)
                    .AddWhereCondition("cnv_data", Comparatori.Uguale, "vpr_cnv_data", DataTypes.Join)
                    .AddWhereCondition("vpr_cic_codice", Comparatori.Uguale, "tsd_cic_codice", DataTypes.OutJoinLeft)
                    .AddWhereCondition("vpr_n_seduta", Comparatori.Uguale, "tsd_n_seduta", DataTypes.OutJoinLeft)
                    .AddWhereCondition("vpr_paz_codice", Comparatori.Uguale, "cnc_cnv_paz_codice", DataTypes.OutJoinLeft)
                    .AddWhereCondition("vpr_cnv_data", Comparatori.Uguale, "cnc_cnv_data", DataTypes.OutJoinLeft)
                    .AddWhereCondition("vpr_cic_codice", Comparatori.Uguale, "cnc_cic_codice", DataTypes.OutJoinLeft)
                    .AddWhereCondition("vpr_n_seduta", Comparatori.Uguale, "cnc_sed_n_seduta", DataTypes.OutJoinLeft)
                    .AddWhereCondition("vpr_vac_codice", Comparatori.Uguale, "vac_codice", DataTypes.Join)

                    .AddWhereCondition("cnv_cns_codice", Comparatori.Uguale, cnsCodice, DataTypes.Stringa)
                    .AddWhereCondition("paz_stato_anagrafico", Comparatori.In, strQueryStatiAnagrafici, DataTypes.Replace)

                    .AddWhereCondition("", Comparatori.NotExist, " ( " + strQueryVacEscluse + " ) ", DataTypes.Replace)
                    .AddWhereCondition("", Comparatori.NotExist, " (" + strQueryVacEseguite + " )", DataTypes.Replace)

                    ' -- Test -- '
                    '.AddWhereCondition("paz_codice", Comparatori.Uguale, 29437, DataTypes.Numero)

                    .AddOrderByFields("paz_cognome", "paz_nome", "paz_codice", "cnv_data", "vpr_cic_codice", "vpr_n_seduta", "vac_codice")

                End With

                Using reader As IDataReader = dam.BuildDataReader()

                    Dim paz_stato_acquisizione_ordinal As Integer = reader.GetOrdinal("PAZ_STATO_ACQUISIZIONE")
                    Dim codiceCiclo_ordinal As Integer = reader.GetOrdinal("CodiceCiclo")
                    Dim dataAppuntamento_ordinal As Integer = reader.GetOrdinal("DataAppuntamento")
                    Dim sedutaCicloObbligatoria_ordinal As Integer = reader.GetOrdinal("SedutaCicloObbligatoria")
                    Dim tsdNumSollecitiRac_ordinal As Integer = reader.GetOrdinal("TsdNumSollecitiRac")
                    Dim tsdNumSolleciti_ordinal As Integer = reader.GetOrdinal("TsdNumSolleciti")
                    Dim cnc_n_sollecito_ordinal As Integer = reader.GetOrdinal("cnc_n_sollecito")
                    Dim codiceStatoAnagrafico_ordinal As Integer = reader.GetOrdinal("CodiceStatoAnagrafico")
                    Dim cnc_data_invio_sollecito_ordinal As Integer = reader.GetOrdinal("cnc_data_invio_sollecito")
                    Dim tsdGiorniPosticipo_ordinal As Integer = reader.GetOrdinal("TsdGiorniPosticipo")
                    Dim cnc_flag_giorni_posticipo_ordinal As Integer = reader.GetOrdinal("cnc_flag_giorni_posticipo")
                    Dim tsd_num_soll_non_obbl_ordinal As Integer = reader.GetOrdinal("tsd_num_soll_non_obbl")
                    Dim tsd_posticipo_seduta_ordinal As Integer = reader.GetOrdinal("tsd_posticipo_seduta")
                    Dim cnc_flag_posticipo_seduta_ordinal As Integer = reader.GetOrdinal("cnc_flag_posticipo_seduta")

                    While reader.Read()

                        ' Caso di pazienti disallineato (PAZ_STATO_ACQUISIZIONE <> 0,2)
                        If FlagConsensoVaccUslCorrente AndAlso
                           (reader.IsDBNull(paz_stato_acquisizione_ordinal) OrElse
                            Convert.ToInt32(reader(paz_stato_acquisizione_ordinal)) = Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Parziale) Then

                            AddPazientiDisallineati(reader, dtStatiAnag)

                            ' Passo al paziente successivo
                            Continue While

                        End If

                        Dim dataAppuntamentoCorrente As DateTime? = Nothing
                        If Not reader.IsDBNull(dataAppuntamento_ordinal) Then
                            dataAppuntamentoCorrente = reader.GetDateTime(dataAppuntamento_ordinal)
                        End If

                        Dim dataInvioSollecito As DateTime? = Nothing
                        If Not reader.IsDBNull(cnc_data_invio_sollecito_ordinal) Then
                            dataInvioSollecito = reader.GetDateTime(cnc_data_invio_sollecito_ordinal)
                        End If

                        ' Caso di pazienti senza ciclo
                        If reader.IsDBNull(codiceCiclo_ordinal) Then

                            If Not dataAppuntamentoCorrente.HasValue Then
                                Throw New ApplicationException("La data appuntamento non deve essere nulla se il sollecito non ha il ciclo associato")
                            End If

                            If dataAppuntamentoCorrente.Value < dataOdierna.AddDays(-Me.Settings.TEMPOESCLUSIONE) Then
                                AddPazientiNoCicli(reader, dtStatiAnag)
                            End If

                            ' Passo al paziente successivo
                            Continue While

                        End If

                        ' Caso di pazienti con seduta ciclo obbligatoria
                        If reader(sedutaCicloObbligatoria_ordinal).ToString() = "S" Then

                            Dim maxNumeroSolleciti As Short
                            Dim maxSollecitiSeduta As Boolean
                            Dim tipoStatoPazientiDaSollecitare As TipoStatoPazientiDaSollecitare

                            If reader(tsdNumSollecitiRac_ordinal) > 0 Then

                                maxNumeroSolleciti = reader(tsdNumSollecitiRac_ordinal)
                                maxSollecitiSeduta = True

                                tipoStatoPazientiDaSollecitare = tipoStatoPazientiDaSollecitare.SollecitoRaccomandato

                            Else

                                If reader(tsdNumSolleciti_ordinal) > 0 Then

                                    maxNumeroSolleciti = reader(tsdNumSolleciti_ordinal)
                                    maxSollecitiSeduta = True

                                Else

                                    maxNumeroSolleciti = Me.Settings.NUMSOL
                                    maxSollecitiSeduta = False

                                End If

                                tipoStatoPazientiDaSollecitare = tipoStatoPazientiDaSollecitare.Sollecito

                            End If

                            If dataAppuntamentoCorrente.HasValue Then

                                ' **********************
                                ' ** CON APPUNTAMENTO **
                                ' **********************

                                If reader(cnc_n_sollecito_ordinal) < maxNumeroSolleciti Then

                                    If dataAppuntamentoCorrente.Value < dataOdierna.AddDays(-Me.Settings.TEMPORIT) Then
                                        AddPazientiCicliObbligatoriNoTP(reader, tipoStatoPazientiDaSollecitare, maxNumeroSolleciti, maxSollecitiSeduta, dtStatiAnag)
                                    End If

                                Else

                                    If reader(tsdNumSollecitiRac_ordinal) > 0 Then

                                        If dataAppuntamentoCorrente.Value < dataOdierna.AddDays(-Me.Settings.TEMPOESCLUSIONE) Then
                                            AddPazientiCicliRaccomandatiTerminati(reader, maxNumeroSolleciti, dtStatiAnag)
                                        End If

                                    Else

                                        If reader(cnc_n_sollecito_ordinal) = maxNumeroSolleciti Then

                                            Dim _statoAnag As Int16 = System.Convert.ToInt16(reader(codiceStatoAnagrafico_ordinal))
                                            If _statoAnag = Enumerators.StatoAnagrafico.DOMICILIATO Then

                                                If dataAppuntamentoCorrente.Value < dataOdierna.AddDays(-Me.Settings.TEMPOINADEMPIENZA) Then
                                                    AddPazientiCicliObbligatoriNoTP(reader, tipoStatoPazientiDaSollecitare.TerminePerentorio, maxNumeroSolleciti, maxSollecitiSeduta, dtStatiAnag)
                                                End If

                                            Else

                                                If dataAppuntamentoCorrente.Value < dataOdierna.AddDays(-Me.Settings.TEMPORIT) Then
                                                    AddPazientiCicliObbligatoriNoTP(reader, tipoStatoPazientiDaSollecitare.TerminePerentorio, maxNumeroSolleciti, maxSollecitiSeduta, dtStatiAnag)
                                                End If

                                            End If

                                        Else

                                            If dataAppuntamentoCorrente.Value < dataOdierna.AddDays(-Me.Settings.TEMPOINADEMPIENZA) Then
                                                AddPazientiCicliObbligatoriInTP(reader, maxNumeroSolleciti, dtStatiAnag)
                                            End If

                                        End If
                                    End If
                                End If

                            Else

                                ' ************************
                                ' ** SENZA APPUNTAMENTO **
                                ' ************************
                                If dataInvioSollecito.HasValue AndAlso dataInvioSollecito.Value < dataOdierna.AddDays(-Me.Settings.TEMPOINADEMPIENZA) Then
                                    AddPazientiCicliObbligatoriInTP(reader, maxNumeroSolleciti, dtStatiAnag)
                                End If

                            End If

                            ' Passo al paziente successivo
                            Continue While

                        End If

                        ' ***********************************
                        ' ** SEDUTA CICLO NON OBBLIGATORIA **
                        ' ***********************************
                        Dim tipoStato As TipoStatoPazientiVaccinazioniNonObbligatorie

                        If reader(tsdGiorniPosticipo_ordinal) > 0 AndAlso reader(cnc_flag_giorni_posticipo_ordinal) = "N" Then

                            tipoStato = TipoStatoPazientiVaccinazioniNonObbligatorie.PosticipoGiorni
                            If Not dataAppuntamentoCorrente.HasValue Then
                                If dataInvioSollecito.HasValue AndAlso dataInvioSollecito.Value < dataOdierna.AddDays(-Me.Settings.TEMPORIT) Then
                                    AddPazientiCicliNonObbligatori(reader, tipoStato, dtStatiAnag)
                                End If
                            Else
                                If dataAppuntamentoCorrente.Value < dataOdierna.AddDays(-Me.Settings.TEMPORIT) Then
                                    AddPazientiCicliNonObbligatori(reader, tipoStato, dtStatiAnag)
                                End If
                            End If

                        ElseIf reader(tsd_num_soll_non_obbl_ordinal) > 0 AndAlso reader(cnc_n_sollecito_ordinal) < reader(tsd_num_soll_non_obbl_ordinal) Then

                            tipoStato = TipoStatoPazientiVaccinazioniNonObbligatorie.SollecitoStandard
                            If Not dataAppuntamentoCorrente.HasValue Then
                                If dataInvioSollecito.HasValue AndAlso dataInvioSollecito.Value < dataOdierna.AddDays(-Me.Settings.TEMPORIT) Then
                                    AddPazientiCicliNonObbligatori(reader, tipoStato, dtStatiAnag)
                                End If
                            Else
                                If dataAppuntamentoCorrente.Value < dataOdierna.AddDays(-Me.Settings.TEMPORIT) Then
                                    AddPazientiCicliNonObbligatori(reader, tipoStato, dtStatiAnag)
                                End If
                            End If

                        ElseIf reader(tsd_posticipo_seduta_ordinal) = "S" AndAlso reader(cnc_flag_posticipo_seduta_ordinal) = "N" Then

                            tipoStato = TipoStatoPazientiVaccinazioniNonObbligatorie.PosticipoSeduta
                            If Not dataAppuntamentoCorrente.HasValue Then
                                If dataInvioSollecito.HasValue AndAlso dataInvioSollecito.Value < dataOdierna.AddDays(-Me.Settings.TEMPORIT) Then
                                    AddPazientiCicliNonObbligatori(reader, tipoStato, dtStatiAnag)
                                End If
                            Else
                                If dataAppuntamentoCorrente.Value < dataOdierna.AddDays(-Me.Settings.TEMPORIT) Then
                                    AddPazientiCicliNonObbligatori(reader, tipoStato, dtStatiAnag)
                                End If
                            End If

                        Else

                            tipoStato = TipoStatoPazientiVaccinazioniNonObbligatorie.EsclusioneStandard
                            If Not dataAppuntamentoCorrente.HasValue Then
                                If dataInvioSollecito.HasValue AndAlso dataInvioSollecito.Value < dataOdierna.AddDays(-Me.Settings.TEMPOESCLUSIONE) Then
                                    AddPazientiCicliNonObbligatori(reader, tipoStato, dtStatiAnag)
                                End If
                            Else
                                If dataAppuntamentoCorrente.Value < dataOdierna.AddDays(-Me.Settings.TEMPOESCLUSIONE) Then
                                    AddPazientiCicliNonObbligatori(reader, tipoStato, dtStatiAnag)
                                End If
                            End If

                        End If

                    End While

                End Using
            End Using

        End Sub

        Public Sub ProcessaPazienti()

            Dim dataElaborazioneSolleciti As DateTime = DateTime.Now

            ProcessaPazientiCicloObbligatorioNoTP()

            ProcessaPazientiCicloObbligatorioInTP()

            ' **************************************************************************************************************
            ' N.B. *********************************************************************************************************
            ' **************************************************************************************************************
            ' se "Me.settings.ESCLUDINONOBBLSETI = TRUE" e il paziente è TOTALMENTE INADEMPIENTE, il metodo 
            ' "OnVacUtility.controllaSeTotalmenteInadempiente" esclude le vaccinazioni RACCOMANDATE e/o NON OBBLIGATORIE dei 
            ' CICLI ATTUALI del paziente, e quindi si DEVONO considerare SOLO quelle PROGRAMMATE
            ' ***************************************************************************************************************

            If Settings.ESCLUDINONOBBLSETI Then

                EliminaPazientiRecordsConvocazioneInesistente(PazientiCicliRaccomandatiTerminati)
                EliminaPazientiRecordsConvocazioneInesistente(PazientiCicliNonObbligatori)

            End If

            ProcessaPazientiCicloRaccomandatoTerminato()

            ProcessaPazientiCicloNonObbligatorio(dataElaborazioneSolleciti)

            ProcessaPazientiNoCiclo()

        End Sub

#End Region

#Region " Private Methods "

#Region " Ciclo Obbligatorio\Raccomandato "

        Private Sub AddPazientiCicliObbligatoriNoTP(reader As IDataReader, tipoPazienteDaSollecitare As TipoStatoPazientiDaSollecitare, maxSolleciti As Short, isMaxSollecitiSeduta As Boolean, dtaStatiAnagrafici As DataTable)

            Dim codicePaziente As String = reader("Codice")
            Dim dataCnv As Date = reader("DataConvocazione")
            Dim codiceCiclo As String = reader("CodiceCiclo")
            Dim numeroSedutaCiclo As String = reader("NumeroSedutaCiclo")

            Dim pazienteCicloObbligatorioNoTP As PazienteCicloObbligatorioNoTP = Nothing

            For Each pazienteCicloObbligatorioNoTPTemp As PazienteCicloObbligatorioNoTP In Me.PazientiCicliObbligatoriNoTP
                If pazienteCicloObbligatorioNoTPTemp.PazienteCodice = codicePaziente AndAlso
                   pazienteCicloObbligatorioNoTPTemp.DataConvocazione = dataCnv AndAlso
                   pazienteCicloObbligatorioNoTPTemp.TipoStato = tipoPazienteDaSollecitare AndAlso
                   pazienteCicloObbligatorioNoTPTemp.Ciclo = codiceCiclo AndAlso
                   pazienteCicloObbligatorioNoTPTemp.Seduta.ToString() = numeroSedutaCiclo Then

                    pazienteCicloObbligatorioNoTP = pazienteCicloObbligatorioNoTPTemp
                    Exit For

                End If
            Next

            If pazienteCicloObbligatorioNoTP Is Nothing Then

                pazienteCicloObbligatorioNoTP = New PazienteCicloObbligatorioNoTP()

                pazienteCicloObbligatorioNoTP.PazienteCodice = reader("Codice").ToString()
                pazienteCicloObbligatorioNoTP.DataConvocazione = reader("DataConvocazione")
                pazienteCicloObbligatorioNoTP.Nome = reader("Nome").ToString()
                pazienteCicloObbligatorioNoTP.Cognome = reader("Cognome").ToString()
                pazienteCicloObbligatorioNoTP.DataNascita = IIf(reader("DataNascita") Is DBNull.Value, Date.MinValue, reader("DataNascita"))
                pazienteCicloObbligatorioNoTP.CodiceStatoAnagrafico = IIf(reader("CodiceStatoAnagrafico") Is DBNull.Value, Nothing, reader("CodiceStatoAnagrafico"))
                pazienteCicloObbligatorioNoTP.DescStatoAnagrafico = dtaStatiAnagrafici.Rows.Find(pazienteCicloObbligatorioNoTP.CodiceStatoAnagrafico).Item("san_descrizione")

                pazienteCicloObbligatorioNoTP.DataInvio = IIf(reader("DataInvio") Is DBNull.Value, Date.MinValue, reader("DataInvio"))
                pazienteCicloObbligatorioNoTP.DataAppuntamento = IIf(reader("DataAppuntamento") Is DBNull.Value, Date.MinValue, reader("DataAppuntamento"))

                pazienteCicloObbligatorioNoTP.Ciclo = reader("CodiceCiclo").ToString()
                pazienteCicloObbligatorioNoTP.Seduta = IIf(numeroSedutaCiclo = String.Empty, 0, Short.Parse(numeroSedutaCiclo))
                pazienteCicloObbligatorioNoTP.IsMaxSollecitiSeduta = isMaxSollecitiSeduta
                pazienteCicloObbligatorioNoTP.MaxSolleciti = maxSolleciti
                pazienteCicloObbligatorioNoTP.NumSollecitoSeduta = reader("cnc_n_sollecito")

                pazienteCicloObbligatorioNoTP.TipoStato = tipoPazienteDaSollecitare

                Me.PazientiCicliObbligatoriNoTP.Add(pazienteCicloObbligatorioNoTP)

            End If

            Dim vaccinazione As New Vaccinazione()
            vaccinazione.Codice = reader("VaccinazioneCodice")
            vaccinazione.Obbligatoria = reader("VaccinazioneObbligatoria") = Constants.ObbligatorietaVaccinazione.Obbligatoria

            pazienteCicloObbligatorioNoTP.Vaccinazioni.Add(vaccinazione)

        End Sub

        Private Sub AddPazientiCicliObbligatoriInTP(reader As IDataReader, maxSolleciti As Short, dtaStatiAnagrafici As DataTable)

            Dim codicePaziente As String = reader("Codice")
            Dim dataCnv As Date = reader("DataConvocazione")
            Dim codiceCiclo As String = reader("CodiceCiclo")
            Dim numeroSedutaCiclo As String = reader("NumeroSedutaCiclo")

            Dim pazienteCicloObbligatorioInTP As PazienteCicloObbligatorioInTP = Nothing

            For Each pazienteCicloObbligatorioInTPTemp As PazienteCicloObbligatorioInTP In Me.PazientiCicliObbligatoriInTP
                If pazienteCicloObbligatorioInTPTemp.PazienteCodice = codicePaziente AndAlso
                   pazienteCicloObbligatorioInTPTemp.DataConvocazione = dataCnv AndAlso
                   pazienteCicloObbligatorioInTPTemp.Ciclo = codiceCiclo AndAlso
                   pazienteCicloObbligatorioInTPTemp.Seduta.ToString() = numeroSedutaCiclo Then

                    pazienteCicloObbligatorioInTP = pazienteCicloObbligatorioInTPTemp
                    Exit For

                End If
            Next

            If pazienteCicloObbligatorioInTP Is Nothing Then

                pazienteCicloObbligatorioInTP = New PazienteCicloObbligatorioInTP()

                pazienteCicloObbligatorioInTP.PazienteCodice = codicePaziente

                pazienteCicloObbligatorioInTP.Cognome = reader("Cognome").ToString()
                pazienteCicloObbligatorioInTP.Nome = reader("Nome").ToString()
                pazienteCicloObbligatorioInTP.DataNascita = IIf(reader("DataNascita") Is DBNull.Value, Date.MinValue, reader("DataNascita"))
                pazienteCicloObbligatorioInTP.CodiceStatoAnagrafico = IIf(reader("CodiceStatoAnagrafico") Is DBNull.Value, Nothing, reader("CodiceStatoAnagrafico"))
                pazienteCicloObbligatorioInTP.DescStatoAnagrafico = dtaStatiAnagrafici.Rows.Find(pazienteCicloObbligatorioInTP.CodiceStatoAnagrafico).Item("san_descrizione")

                pazienteCicloObbligatorioInTP.DataConvocazione = reader("DataConvocazione")
                pazienteCicloObbligatorioInTP.DataAppuntamento = IIf(Not reader("DataAppuntamento") Is DBNull.Value, reader("DataAppuntamento"), Date.MinValue)
                pazienteCicloObbligatorioInTP.DataInvio = IIf(Not reader("DataInvio") Is DBNull.Value, reader("DataInvio"), Date.MinValue)

                pazienteCicloObbligatorioInTP.Ciclo = codiceCiclo
                pazienteCicloObbligatorioInTP.Seduta = IIf(numeroSedutaCiclo = String.Empty, 0, Short.Parse(numeroSedutaCiclo))
                pazienteCicloObbligatorioInTP.MaxSolleciti = maxSolleciti
                pazienteCicloObbligatorioInTP.NumSollecitoSeduta = reader("cnc_n_sollecito")

                Me.PazientiCicliObbligatoriInTP.Add(pazienteCicloObbligatorioInTP)

            End If

            Dim vaccinazione As New Vaccinazione()
            vaccinazione.Codice = reader("VaccinazioneCodice")
            vaccinazione.Obbligatoria = reader("VaccinazioneObbligatoria") = Constants.ObbligatorietaVaccinazione.Obbligatoria

            pazienteCicloObbligatorioInTP.Vaccinazioni.Add(vaccinazione)

        End Sub

        Private Sub AddPazientiCicliRaccomandatiTerminati(reader As IDataReader, maxSolleciti As Short, dtaStatiAnagrafici As DataTable)

            Dim codicePaziente As String = reader("Codice")
            Dim dataCnv As Date = reader("DataConvocazione")
            Dim codiceCiclo As String = reader("CodiceCiclo")
            Dim numeroSedutaCiclo As String = reader("NumeroSedutaCiclo")

            Dim pazienteCicloRaccomandatoTerminato As PazienteCicloRaccomandatoTerminato = Nothing

            For Each pazienteCicloRaccomandatoTerminatoTemp As PazienteCicloRaccomandatoTerminato In Me.PazientiCicliRaccomandatiTerminati
                If pazienteCicloRaccomandatoTerminatoTemp.PazienteCodice = codicePaziente AndAlso
                   pazienteCicloRaccomandatoTerminatoTemp.DataConvocazione = dataCnv AndAlso
                   pazienteCicloRaccomandatoTerminatoTemp.Ciclo = codiceCiclo AndAlso
                   pazienteCicloRaccomandatoTerminatoTemp.Seduta.ToString() = numeroSedutaCiclo Then

                    pazienteCicloRaccomandatoTerminato = pazienteCicloRaccomandatoTerminatoTemp
                    Exit For

                End If
            Next

            If pazienteCicloRaccomandatoTerminato Is Nothing Then

                pazienteCicloRaccomandatoTerminato = New PazienteCicloRaccomandatoTerminato()

                pazienteCicloRaccomandatoTerminato.PazienteCodice = codicePaziente

                pazienteCicloRaccomandatoTerminato.DataConvocazione = dataCnv
                pazienteCicloRaccomandatoTerminato.DataAppuntamento = reader("DataAppuntamento")
                pazienteCicloRaccomandatoTerminato.Nome = reader("Nome").ToString()
                pazienteCicloRaccomandatoTerminato.Cognome = reader("Cognome").ToString()
                pazienteCicloRaccomandatoTerminato.DataNascita = IIf(reader("DataNascita") Is DBNull.Value, Date.MinValue, reader("DataNascita"))
                pazienteCicloRaccomandatoTerminato.CodiceStatoAnagrafico = IIf(reader("CodiceStatoAnagrafico") Is DBNull.Value, Nothing, reader("CodiceStatoAnagrafico"))
                pazienteCicloRaccomandatoTerminato.DescStatoAnagrafico = dtaStatiAnagrafici.Rows.Find(pazienteCicloRaccomandatoTerminato.CodiceStatoAnagrafico).Item("san_descrizione")

                pazienteCicloRaccomandatoTerminato.Ciclo = codiceCiclo
                pazienteCicloRaccomandatoTerminato.Seduta = IIf(numeroSedutaCiclo = String.Empty, 0, Short.Parse(numeroSedutaCiclo))
                pazienteCicloRaccomandatoTerminato.MaxSolleciti = maxSolleciti
                pazienteCicloRaccomandatoTerminato.NumSollecitoSeduta = reader("cnc_n_sollecito")

                Me.PazientiCicliRaccomandatiTerminati.Add(pazienteCicloRaccomandatoTerminato)

            End If

            Dim vaccinazione As New Vaccinazione()
            vaccinazione.Codice = reader("VaccinazioneCodice")
            vaccinazione.Obbligatoria = reader("VaccinazioneObbligatoria") = Constants.ObbligatorietaVaccinazione.Obbligatoria

            pazienteCicloRaccomandatoTerminato.Vaccinazioni.Add(vaccinazione)

        End Sub

        Private Sub ProcessaPazientiCicloObbligatorioNoTP()

            Dim max As Integer? = GetIntegerAppSettingValue("MaxProcessaPazientiSolleciti")

            For i As Integer = 0 To PazientiCicliObbligatoriNoTP.Count - 1

                ' Parametro da valorizzare nel web.config se si vuol processare solo un determinato numero di sollecito, per test!
                If max.HasValue AndAlso max.Value > 0 Then

                    If i >= max Then Exit For

                End If

                Dim record As PazienteCicloObbligatorioNoTP = Me.PazientiCicliObbligatoriNoTP(i)

                Try
                    Using transactionScope As New System.Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, OnVacUtility.GetReadCommittedTransactionOptions())

                        Dim vaccinazioneEsclusaMotivoCentraleAggiuntaList As New List(Of Entities.VaccinazioneEsclusa)()
                        Dim vaccinazioneEsclusaMotivoCentraleEliminataList As New List(Of Entities.VaccinazioneEsclusa)()

                        Using dam As IDAM = OnVacUtility.OpenDam()

                            Select Case record.TipoStato

                                Case TipoStatoPazientiDaSollecitare.Sollecito, TipoStatoPazientiDaSollecitare.SollecitoRaccomandato

                                    CreaSollecitoRitardo(dam, Nothing, record)
                                    AggiornaNote(dam, record)

                                Case TipoStatoPazientiDaSollecitare.TerminePerentorio

                                    '----------------------------------------------------------------------
                                    'se il paziente è residente deve passare allo stato termine perentorio,
                                    'altrimenti creare direttamente l'inadempienza per non ritrovarlo
                                    'in una nuova ricerca
                                    '----------------------------------------------------------------------
                                    Dim statoAnagraficoPaziente As Int16 = System.Convert.ToInt16(record.CodiceStatoAnagrafico)

                                    If statoAnagraficoPaziente = Enumerators.StatoAnagrafico.DOMICILIATO Then

                                        Me.CreaInadempienza(dam, record.PazienteCodice, vaccinazioneEsclusaMotivoCentraleAggiuntaList,
                                                            vaccinazioneEsclusaMotivoCentraleEliminataList, Nothing, record)

                                    Else

                                        CreaSollecitoRitardo(dam, Nothing, record)
                                        AggiornaNote(dam, record)

                                    End If

                            End Select

                        End Using

                        Me.AggiornaDatiVaccinaliCentrali(record.PazienteCodice, vaccinazioneEsclusaMotivoCentraleAggiuntaList,
                                                         vaccinazioneEsclusaMotivoCentraleEliminataList)

                        transactionScope.Complete()

                    End Using

                Catch ex As Exception
                    Common.Utility.EventLogHelper.EventLogWrite(ex, OnVacContext.AppId)
                End Try

            Next

        End Sub

        'processa i pazienti in termine perentorio che devono passare allo stato comunicazione al sindaco [modifica 31/03/2006]
        Private Sub ProcessaPazientiCicloObbligatorioInTP()

            Dim max As Integer? = GetIntegerAppSettingValue("MaxProcessaPazientiSolleciti")

            'porta lo stato dei pazienti a Comunicazione Al Sindaco creando direttamente l'inadempienza
            For i As Integer = 0 To Me.PazientiCicliObbligatoriInTP.Count - 1

                ' Parametro da valorizzare nel web.config se si vuol processare solo un determinato numero di sollecito, per test!
                If max.HasValue AndAlso max.Value > 0 Then

                    If i >= max Then Exit For

                End If

                Try
                    Using transactionScope As New System.Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, OnVacUtility.GetReadCommittedTransactionOptions())

                        Dim vaccinazioneEsclusaMotivoCentraleAggiuntaList As New List(Of Entities.VaccinazioneEsclusa)()
                        Dim vaccinazioneEsclusaMotivoCentraleEliminataList As New List(Of Entities.VaccinazioneEsclusa)()
                        Dim pazCodice = DirectCast(Me.PazientiCicliObbligatoriInTP(i), PazienteCicloObbligatorioInTP).PazienteCodice

                        Using dam As IDAM = OnVacUtility.OpenDam()

                            Me.CreaInadempienza(dam, pazCodice, vaccinazioneEsclusaMotivoCentraleAggiuntaList, vaccinazioneEsclusaMotivoCentraleEliminataList,
                                                Me.PazientiCicliObbligatoriInTP(i), Nothing)

                            Me.AggiornaDatiVaccinaliCentrali(pazCodice, vaccinazioneEsclusaMotivoCentraleAggiuntaList,
                                                             vaccinazioneEsclusaMotivoCentraleEliminataList)

                        End Using

                        transactionScope.Complete()

                    End Using

                Catch ex As Exception
                    Common.Utility.EventLogHelper.EventLogWrite(ex, OnVacContext.AppId)
                End Try

            Next

        End Sub

        Private Function CreaInadempienza(ByRef dam As IDAM, pazCodice As Integer, vaccinazioneEsclusaMotivoCentraleAggiuntaList As List(Of Entities.VaccinazioneEsclusa), vaccinazioneEsclusaMotivoCentraleEliminataList As List(Of Entities.VaccinazioneEsclusa), tp As PazienteCicloObbligatorioInTP, pds As PazienteCicloObbligatorioNoTP) As Boolean

            Dim record As PazienteCicloCommon = Nothing

            If Not tp Is Nothing Then
                record = tp
            ElseIf Not pds Is Nothing Then
                record = pds
            End If

            Dim codiceMotivoEsclusioneInadempienza As String = String.Empty
            Dim dateAppuntamentiRitardiPaziente As Entities.DateAppuntamentiRitardi = Nothing
            Dim dataInvioSollecito As DateTime? = Nothing

            Using genericProvider As New DAL.DbGenericProvider(dam)

                ' Codice del motivo di esclusione che si crea per l'inadempienza
                codiceMotivoEsclusioneInadempienza = genericProvider.MotiviEsclusione.GetCodiceMotivoEsclusioneDefaultInadempienza()

                'devono essere recuperate le date di appuntamento dalla tabella dei ritardi per mantenerne traccia alla creazione dell'inadempienza
                dateAppuntamentiRitardiPaziente = genericProvider.Paziente.GetDateAppuntamentiRitardi(record.PazienteCodice, record.DataConvocazione, record.Ciclo, record.Seduta)

                ' Data di invio del sollecito
                If Not tp Is Nothing Then
                    dataInvioSollecito = genericProvider.Cicli.GetDataInvioSollecito(
                        record.PazienteCodice, record.DataConvocazione, record.Ciclo, record.Seduta, record.NumSollecitoSeduta)
                End If

                Dim now As DateTime = DateTime.Now

                ' Filtro le sole vaccinazioni obbligatorie
                Dim vaccinazioniObbligatorie As List(Of Vaccinazione) = record.Vaccinazioni.Where(Function(p) p.Obbligatoria).ToList()

                If Not vaccinazioniObbligatorie.IsNullOrEmpty() Then

                    For Each vaccinazione As Vaccinazione In vaccinazioniObbligatorie

                        ' Se non c'è già un record di inadempienza per il paziente e la vaccinazione specificata, lo crea.
                        Dim countInadempienze As Integer = genericProvider.Paziente.CountInadempienze(record.PazienteCodice, vaccinazione.Codice)

                        If countInadempienze = 0 Then

                            ' Inserimento inadempienza
                            Dim command As New InsertInadempienzaCommand()
                            command.CodicePaziente = record.PazienteCodice
                            command.CodiceVaccinazione = vaccinazione.Codice
                            command.FlagStampato = "N"

                            If Not tp Is Nothing Then
                                command.StatoInadempienza = Enumerators.StatoInadempienza.ComunicazioneAlSindaco
                            ElseIf Not pds Is Nothing Then
                                command.StatoInadempienza = Enumerators.StatoInadempienza.TerminePerentorio
                            Else
                                command.StatoInadempienza = Nothing
                            End If

                            command.IdUtenteInserimento = OnVacContext.UserId
                            command.DataInserimento = now
                            command.DateAppuntamentiRitardiPaziente = dateAppuntamentiRitardiPaziente
                            command.DataInvioSollecito = dataInvioSollecito

                            If record.DataAppuntamento = Date.MinValue Then
                                command.DataAppuntamento = Nothing
                            Else
                                command.DataAppuntamento = record.DataAppuntamento
                            End If

                            genericProvider.Paziente.InsertInadempienza(command)

                        End If

                    Next
                End If

            End Using

            InserisciVaccinazioniEscluse(record.PazienteCodice, record.Vaccinazioni, codiceMotivoEsclusioneInadempienza, dam, vaccinazioneEsclusaMotivoCentraleAggiuntaList, vaccinazioneEsclusaMotivoCentraleEliminataList)

            OnVacUtility.ControllaSeTotalmenteInadempiente(record.PazienteCodice, dam, Settings)

        End Function

        Private Sub ProcessaPazientiCicloRaccomandatoTerminato()

            Dim max As Integer? = GetIntegerAppSettingValue("MaxProcessaPazientiSolleciti")

            For i As Integer = 0 To PazientiCicliRaccomandatiTerminati.Count - 1

                ' Parametro da valorizzare nel web.config se si vuol processare solo un determinato numero di sollecito, per test!
                If max.HasValue AndAlso max.Value > 0 Then

                    If i >= max Then Exit For

                End If

                Dim pazienteCicloRaccomandatoTerminato As PazienteCicloRaccomandatoTerminato = PazientiCicliRaccomandatiTerminati(i)

                Try
                    Using transactionScope As New System.Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, OnVacUtility.GetReadCommittedTransactionOptions())

                        Dim vaccinazioneEsclusaMotivoCentraleAggiuntaList As New List(Of Entities.VaccinazioneEsclusa)()
                        Dim vaccinazioneEsclusaMotivoCentraleEliminataList As New List(Of Entities.VaccinazioneEsclusa)()

                        Using dam As IDAM = OnVacUtility.OpenDam()

                            Me.InserisciVaccinazioniEscluse(PazienteCicloRaccomandatoTerminato.PazienteCodice, PazienteCicloRaccomandatoTerminato.Vaccinazioni,
                                                            Me.Settings.CODESCL, dam, vaccinazioneEsclusaMotivoCentraleAggiuntaList, vaccinazioneEsclusaMotivoCentraleEliminataList)

                            Me.AggiornaDatiVaccinaliCentrali(PazienteCicloRaccomandatoTerminato.PazienteCodice, vaccinazioneEsclusaMotivoCentraleAggiuntaList,
                                                             vaccinazioneEsclusaMotivoCentraleEliminataList)

                        End Using

                        transactionScope.Complete()

                    End Using

                Catch ex As Exception
                    Common.Utility.EventLogHelper.EventLogWrite(ex, OnVacContext.AppId)
                End Try

            Next

        End Sub

        Private Sub InserisciVaccinazioniEscluse(codicePaziente As String, vaccinazioni As Collection(Of Vaccinazione), codiceMotivoEsclusione As String, dam As IDAM, vaccinazioneEsclusaMotivoCentraleAggiuntaList As List(Of Entities.VaccinazioneEsclusa), vaccinazioneEsclusaMotivoCentraleEliminataList As List(Of Entities.VaccinazioneEsclusa))

            Using genericProvider As New DAL.DbGenericProvider(dam)

                ' Controllo consenso GLOBALE del paziente: se bloccante => no inserimento
                Using bizPaziente As New Biz.BizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                    If Not bizPaziente.IsVisibilitaConcessaByStatoConsensoGlobale(Convert.ToInt64(codicePaziente)) Then Return

                End Using

                Dim bizVaccinazioniEscluse As New Biz.BizVaccinazioniEscluse(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                Dim bizMotiviEsclusione As New Biz.BizMotiviEsclusione(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                Try
                    ' N.B. : il valore di visibilità dei dati, in questo caso, dipende solo dallo stato del consenso alla COMUNICAZIONE (e non da quello globale).
                    Dim flagVisibilita As String =
                        Common.OnVacStoricoVaccinaleCentralizzato.GetValoreVisibilitaDatiVaccinaliDefault(Settings, codicePaziente, True)

                    'conteggio delle escluse aggiunte/modificate. Variabile utilizzata per la parte di FSE
                    Dim nrVaccEscluseAggiunte As Integer = 0

                    For Each vaccinazione As Vaccinazione In vaccinazioni

                        Dim dataScadenza As Date = bizMotiviEsclusione.GetScadenzaMotivoEsclusione(codicePaziente, codiceMotivoEsclusione, vaccinazione.Codice, DateTime.Today)
                        Dim dataScadenzaNullable As Date? = IIf(dataScadenza <> Date.MinValue, dataScadenza, Nothing)

                        Dim vaccinazioneEsclusa As New Entities.VaccinazioneEsclusa()
                        vaccinazioneEsclusa.CodicePaziente = codicePaziente
                        vaccinazioneEsclusa.CodiceVaccinazione = vaccinazione.Codice
                        vaccinazioneEsclusa.DataVisita = DateTime.Today
                        vaccinazioneEsclusa.CodiceMotivoEsclusione = codiceMotivoEsclusione
                        vaccinazioneEsclusa.CodiceOperatore = OnVacContext.UserId.ToString()
                        vaccinazioneEsclusa.DataScadenza = dataScadenza
                        vaccinazioneEsclusa.DataRegistrazione = DateTime.Now
                        vaccinazioneEsclusa.IdUtenteRegistrazione = OnVacContext.UserId
                        vaccinazioneEsclusa.FlagVisibilita = flagVisibilita

                        Dim vaccinazioneEsclusaEliminata As Entities.VaccinazioneEsclusa =
                            bizVaccinazioniEscluse.AggiornaVaccinazioneEsclusa(vaccinazioneEsclusa, True, "Solleciti")

                        If Not vaccinazioneEsclusaEliminata Is Nothing Then
                            nrVaccEscluseAggiunte += 1
                            vaccinazioneEsclusaMotivoCentraleEliminataList.Add(vaccinazioneEsclusaEliminata)
                        End If

                        vaccinazioneEsclusaMotivoCentraleAggiuntaList.Add(vaccinazioneEsclusa)

                    Next


#Region " FSE "
                    If nrVaccEscluseAggiunte > 0 Then
                        If Settings.FSE_GESTIONE Then

                            Dim indicizzazioneResult As Biz.BizGenericResult = OnVacUtility.FSEHelper.IndicizzaSuRegistry(
                                Convert.ToInt64(codicePaziente),
                                Constants.TipoDocumentoFSE.CertificatoVaccinale,
                                Constants.FunzionalitaNotificaFSE.Solleciti_InserisciVaccinazioniEscluse,
                                Constants.EventoNotificaFSE.InserimentoVaccinazione,
                                Settings,
                                String.Empty)

                            If Not indicizzazioneResult.Success AndAlso Not String.IsNullOrWhiteSpace(indicizzazioneResult.Message) Then
                                'In caso di esito negativo dell'indicizzazione sul Registry Regionale NON ritorno alcun messaggio di errore
                            End If

                        End If
                    End If
#End Region


                Finally
                    If Not bizVaccinazioniEscluse Is Nothing Then bizVaccinazioniEscluse.Dispose()
                    If Not bizMotiviEsclusione Is Nothing Then bizMotiviEsclusione.Dispose()
                End Try

            End Using

        End Sub

#End Region

#Region " Ciclo Non Obbligatorio "

        Private Sub AddPazientiCicliNonObbligatori(reader As IDataReader, tipoStatoPazientiVaccinazioniNonObbligatorie As TipoStatoPazientiVaccinazioniNonObbligatorie, dtaStatiAnagrafici As DataTable)

            Dim codicePaziente As String = reader("Codice")
            Dim dataCnv As Date = reader("DataConvocazione")
            Dim codiceCiclo As String = reader("CodiceCiclo")
            Dim numeroSedutaCiclo As String = reader("NumeroSedutaCiclo")

            Dim pazienteCicloNonObbligatorio As PazienteCicloNonObbligatorio = Nothing

            For Each pazienteCicloNonObbligatorioTemp As PazienteCicloNonObbligatorio In Me.PazientiCicliNonObbligatori

                If pazienteCicloNonObbligatorioTemp.PazienteCodice = codicePaziente AndAlso
                   pazienteCicloNonObbligatorioTemp.DataConvocazione = dataCnv AndAlso
                   pazienteCicloNonObbligatorioTemp.TipoStato = tipoStatoPazientiVaccinazioniNonObbligatorie AndAlso
                   pazienteCicloNonObbligatorioTemp.Ciclo = codiceCiclo AndAlso
                   pazienteCicloNonObbligatorioTemp.Seduta.ToString() = numeroSedutaCiclo Then

                    pazienteCicloNonObbligatorio = pazienteCicloNonObbligatorioTemp
                    Exit For

                End If

            Next

            If pazienteCicloNonObbligatorio Is Nothing Then

                pazienteCicloNonObbligatorio = New PazienteCicloNonObbligatorio()

                pazienteCicloNonObbligatorio.PazienteCodice = reader("Codice").ToString()
                pazienteCicloNonObbligatorio.DataConvocazione = reader("DataConvocazione")
                pazienteCicloNonObbligatorio.Nome = reader("Nome").ToString()
                pazienteCicloNonObbligatorio.Cognome = reader("Cognome").ToString()

                pazienteCicloNonObbligatorio.DataInvio = IIf(reader("DataInvio") Is DBNull.Value, Date.MinValue, reader("DataInvio"))
                pazienteCicloNonObbligatorio.DataAppuntamento = IIf(reader("DataAppuntamento") Is DBNull.Value, Date.MinValue, reader("DataAppuntamento"))
                pazienteCicloNonObbligatorio.DataNascita = IIf(reader("DataNascita") Is DBNull.Value, Date.MinValue, reader("DataNascita"))
                pazienteCicloNonObbligatorio.CodiceStatoAnagrafico = IIf(reader("CodiceStatoAnagrafico") Is DBNull.Value, Nothing, reader("CodiceStatoAnagrafico"))
                pazienteCicloNonObbligatorio.DescStatoAnagrafico = dtaStatiAnagrafici.Rows.Find(pazienteCicloNonObbligatorio.CodiceStatoAnagrafico).Item("san_descrizione")

                pazienteCicloNonObbligatorio.Ciclo = reader("CodiceCiclo").ToString()
                pazienteCicloNonObbligatorio.Seduta = IIf(numeroSedutaCiclo = String.Empty, 0, Short.Parse(numeroSedutaCiclo))
                pazienteCicloNonObbligatorio.MaxSolleciti = reader("tsd_num_soll_non_obbl")
                pazienteCicloNonObbligatorio.NumSollecitoSeduta = reader("cnc_n_sollecito")
                pazienteCicloNonObbligatorio.GiorniPosticipo = reader("TsdGiorniPosticipo")

                pazienteCicloNonObbligatorio.TipoStato = tipoStatoPazientiVaccinazioniNonObbligatorie

                Me.PazientiCicliNonObbligatori.Add(pazienteCicloNonObbligatorio)

            End If

            Dim vaccinazione As New Vaccinazione()
            vaccinazione.Codice = reader("VaccinazioneCodice")
            vaccinazione.Obbligatoria = reader("VaccinazioneObbligatoria") = Constants.ObbligatorietaVaccinazione.Obbligatoria

            pazienteCicloNonObbligatorio.Vaccinazioni.Add(vaccinazione)

        End Sub

        Private Sub ProcessaPazientiCicloNonObbligatorio(dataElaborazioneSolleciti As DateTime)

            ' Parametro da valorizzare nel web.config se si vuol processare solo un determinato numero di sollecito, per test!
            Dim max As Integer? = GetIntegerAppSettingValue("MaxProcessaPazientiSolleciti")

            For i As Integer = 0 To PazientiCicliNonObbligatori.Count - 1

                ' Parametro da valorizzare nel web.config se si vuol processare solo un determinato numero di sollecito, per test!
                If max.HasValue AndAlso max.Value > 0 Then

                    If i >= max Then Exit For

                End If

                Dim pazienteCicloNonObbligatorio As PazienteCicloNonObbligatorio = PazientiCicliNonObbligatori(i)

                Try
                    '--
                    Using transactionScope As New System.Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, OnVacUtility.GetReadCommittedTransactionOptions())
                        '--
                        Dim vaccinazioneEsclusaMotivoCentraleAggiuntaList As New List(Of Entities.VaccinazioneEsclusa)()
                        Dim vaccinazioneEsclusaMotivoCentraleEliminataList As New List(Of Entities.VaccinazioneEsclusa)()
                        '--
                        Using dam As IDAM = OnVacUtility.OpenDam()
                            '--
                            Select Case pazienteCicloNonObbligatorio.TipoStato
                                '--
                                Case TipoStatoPazientiVaccinazioniNonObbligatorie.PosticipoGiorni
                                    '--
                                    Me.PosticipaGiorni(dam, pazienteCicloNonObbligatorio, vaccinazioneEsclusaMotivoCentraleAggiuntaList,
                                                       vaccinazioneEsclusaMotivoCentraleEliminataList, dataElaborazioneSolleciti)
                                    '--
                                Case TipoStatoPazientiVaccinazioniNonObbligatorie.SollecitoStandard
                                    '--
                                    Me.CreaSollecitoRitardo(dam, pazienteCicloNonObbligatorio, Nothing)
                                    AggiornaNote(dam, pazienteCicloNonObbligatorio)
                                    '--
                                Case TipoStatoPazientiVaccinazioniNonObbligatorie.PosticipoSeduta
                                    '--
                                    Me.PosticipaCicloPrimaConvocazioneObbligatoria(dam, pazienteCicloNonObbligatorio, vaccinazioneEsclusaMotivoCentraleAggiuntaList,
                                                                                   vaccinazioneEsclusaMotivoCentraleEliminataList, dataElaborazioneSolleciti)
                                    '--
                                Case TipoStatoPazientiVaccinazioniNonObbligatorie.EsclusioneStandard
                                    '--
                                    Me.InserisciVaccinazioniEscluse(pazienteCicloNonObbligatorio.PazienteCodice, pazienteCicloNonObbligatorio.Vaccinazioni,
                                                                    Me.Settings.CODESCLNONOBBL, dam, vaccinazioneEsclusaMotivoCentraleAggiuntaList,
                                                                    vaccinazioneEsclusaMotivoCentraleEliminataList)
                                    '--
                            End Select
                            '--
                        End Using
                        '--
                        Me.AggiornaDatiVaccinaliCentrali(pazienteCicloNonObbligatorio.PazienteCodice, vaccinazioneEsclusaMotivoCentraleAggiuntaList,
                                                         vaccinazioneEsclusaMotivoCentraleEliminataList)
                        '--
                        transactionScope.Complete()
                        '--
                    End Using
                    '--
                Catch ex As Exception
                    '--
                    Common.Utility.EventLogHelper.EventLogWrite(ex, OnVacContext.AppId)
                    '--
                End Try
                '--            
            Next
            '--
        End Sub

        ''' <summary>
        ''' Posticipa il ciclo alla prima convocazione con obbligatoria
        ''' </summary>
        ''' <param name="dam"></param>
        ''' <param name="record"></param>
        ''' <param name="vaccinazioneEsclusaMotivoCentraleAggiuntaList"></param>
        ''' <param name="vaccinazioneEsclusaMotivoCentraleEliminataList"></param>
        ''' <param name="dataElaborazioneSolleciti"></param>
        ''' <remarks></remarks>
        Private Sub PosticipaCicloPrimaConvocazioneObbligatoria(ByRef dam As IDAM, record As PazienteCicloNonObbligatorio, vaccinazioneEsclusaMotivoCentraleAggiuntaList As List(Of Entities.VaccinazioneEsclusa), vaccinazioneEsclusaMotivoCentraleEliminataList As List(Of Entities.VaccinazioneEsclusa), dataElaborazioneSolleciti As DateTime)
            '--
            'controlla se per il paziente è prevista almeno una vaccinazione obbligatoria successiva a quella considerata
            '--
            Dim cnvObblSuccessiva As Object = CnvObbligatoriaSuccessiva(dam, record.PazienteCodice, record.DataConvocazione)
            '--
            If Not cnvObblSuccessiva Is Nothing Then
                '--- ACCORPAMENTO ALLA PRIMA CONVOCAZIONE OBBLIGATORIA
                Me.SpostaVaccinazioniProgrammate(dam, record, cnvObblSuccessiva, vaccinazioneEsclusaMotivoCentraleAggiuntaList, vaccinazioneEsclusaMotivoCentraleEliminataList, dataElaborazioneSolleciti)
            Else
                '--- RICALCOLO CONVOCAZIONE CON EVENTUALE ACCORPAMENTO
                Using gestioneConvocazioni As New CalcoloConvocazioni.GestioneConvocazioni(OnVacUtility.Variabili.CNS.Codice, OnVacContext.CreateBizContextInfos, dam.Provider, dam.Connection, dam.Transaction)
                    gestioneConvocazioni.CalcolaConvocazioni(record.PazienteCodice, OnVacContext.UserId)
                End Using
                '---
                cnvObblSuccessiva = CnvObbligatoriaSuccessiva(dam, record.PazienteCodice)
                '---
                If Not cnvObblSuccessiva Is Nothing Then
                    Me.SpostaVaccinazioniProgrammate(dam, record, cnvObblSuccessiva, vaccinazioneEsclusaMotivoCentraleAggiuntaList, vaccinazioneEsclusaMotivoCentraleEliminataList, dataElaborazioneSolleciti)
                End If
                '---
            End If
            '--
            If cnvObblSuccessiva Is Nothing Then
                '--
                Me.InserisciVaccinazioniEscluse(record.PazienteCodice, record.Vaccinazioni, Me.Settings.CODESCLNONOBBL, dam,
                                                vaccinazioneEsclusaMotivoCentraleAggiuntaList, vaccinazioneEsclusaMotivoCentraleEliminataList)
                '--
            End If
            '--
        End Sub

        Private Sub PosticipaGiorni(ByRef dam As IDAM, record As PazienteCicloNonObbligatorio, vaccinazioneEsclusaMotivoCentraleAggiuntaList As List(Of Entities.VaccinazioneEsclusa), vaccinazioneEsclusaMotivoCentraleEliminataList As List(Of Entities.VaccinazioneEsclusa), dataElaborazioneSolleciti As DateTime)
            '--
            Dim dataNuovaConvocazione As Date = record.DataConvocazione.AddDays(record.GiorniPosticipo)
            '--
            Me.SpostaVaccinazioniProgrammate(dam, record, dataNuovaConvocazione, vaccinazioneEsclusaMotivoCentraleAggiuntaList, vaccinazioneEsclusaMotivoCentraleEliminataList, dataElaborazioneSolleciti)
            '--
        End Sub

        Private Sub SpostaVaccinazioniProgrammate(dam As IDAM, record As PazienteCicloNonObbligatorio, dataNuovaConvocazione As Date, vaccinazioneEsclusaAggiuntaList As List(Of Entities.VaccinazioneEsclusa), vaccinazioneEsclusaEliminataList As List(Of Entities.VaccinazioneEsclusa), dataElaborazioneSolleciti As DateTime)
            '--
            Dim esisteConvocazioneNuovaData As Boolean = False
            Dim daEscludere As Boolean = False
            '--
            Using genericProvider As New Onit.OnAssistnet.OnVac.DAL.DbGenericProvider(dam)
                ' --- Controllo associabilità --- '
                If Me.Settings.CTRL_ASSOCIABILITA_VAC Then
                    '--
                    ' Elenco vaccinazioni nella nuova data
                    '--
                    Dim codiciVaccinazioniProgrammate As ArrayList = genericProvider.VaccinazioneProg.GetCodiceVacProgrammatePazienteByData(record.PazienteCodice, dataNuovaConvocazione)
                    '--
                    ' Se ci sono vaccinazioni, controllo che siano associabili con le nuove
                    '--
                    If codiciVaccinazioniProgrammate.Count > 0 Then
                        '--
                        ' Creazione oggetto utilizzato per effettuare i controlli di associabilità
                        '--
                        Dim ctrlAssociabilita As New OnVac.Associabilita.ControlloAssociabilita(dam.Provider, dam.Connection, dam.Transaction)
                        '--
                        If Not ctrlAssociabilita.VaccinazioniAssociabili(New ArrayList(record.Vaccinazioni), codiciVaccinazioniProgrammate) Then
                            '--
                            daEscludere = True
                            '--
                        End If
                        '--
                        esisteConvocazioneNuovaData = True
                        '--
                    End If
                    '--
                Else
                    '--
                    esisteConvocazioneNuovaData = genericProvider.Convocazione.Exists(record.PazienteCodice, dataNuovaConvocazione)
                    '--
                End If
                '--
                If Not daEscludere Then
                    '--
                    If Not esisteConvocazioneNuovaData Then
                        '--
                        ' La cnv non è presente nella data specificata: la creo
                        '--
                        Dim datiCnv As New Entities.Convocazione()
                        datiCnv.Paz_codice = record.PazienteCodice
                        datiCnv.Data_CNV = dataNuovaConvocazione
                        datiCnv.Durata_Appuntamento = Me.Settings.TEMPOSED
                        datiCnv.Cns_Codice = OnVacUtility.Variabili.CNS.Codice
                        datiCnv.IdUtente = OnVacContext.UserId
                        datiCnv.DataInserimento = dataElaborazioneSolleciti
                        datiCnv.IdUtenteInserimento = OnVacContext.UserId
                        '--
                        genericProvider.Convocazione.InsertConvocazione(datiCnv)
                        '--
                    End If

                    ' --- spostamento ciclo --- 
                    ' NB: In parte la logica è duplicata con la dll OnVac.CalcoloConvocazioni.GestioneConvocazioni

                    ' Prima duplico il record nella t_cnv_cicli
                    genericProvider.Convocazione.InsertCicliCnvUnita(record.PazienteCodice, record.DataConvocazione, dataNuovaConvocazione)

                    ' Aggiorno il flag sul record con la cnv_data nuova
                    dam.QB.NewQuery()
                    dam.QB.AddTables("t_cnv_cicli")
                    Select Case record.TipoStato
                        Case TipoStatoPazientiVaccinazioniNonObbligatorie.PosticipoGiorni
                            dam.QB.AddUpdateField("cnc_flag_giorni_posticipo", "S", DataTypes.Stringa)
                        Case TipoStatoPazientiVaccinazioniNonObbligatorie.PosticipoSeduta
                            dam.QB.AddUpdateField("cnc_flag_posticipo_seduta", "S", DataTypes.Stringa)
                    End Select
                    dam.QB.AddWhereCondition("cnc_cnv_paz_codice", Comparatori.Uguale, record.PazienteCodice, DataTypes.Numero)
                    dam.QB.AddWhereCondition("cnc_cnv_data", Comparatori.Uguale, dataNuovaConvocazione, DataTypes.Data)
                    dam.QB.AddWhereCondition("cnc_cic_codice", Comparatori.Uguale, record.Ciclo, DataTypes.Stringa)
                    dam.QB.AddWhereCondition("cnc_sed_n_seduta", Comparatori.Uguale, record.Seduta, DataTypes.Numero)
                    dam.ExecNonQuery(ExecQueryType.Update)

                    ' Aggiornamento della t_paz_ritardi
                    dam.QB.NewQuery()
                    dam.QB.AddTables("t_paz_ritardi")
                    dam.QB.AddUpdateField("pri_cnv_data", dataNuovaConvocazione, DataTypes.Data)
                    dam.QB.AddWhereCondition("pri_paz_codice", Comparatori.Uguale, record.PazienteCodice, DataTypes.Numero)
                    dam.QB.AddWhereCondition("pri_cnv_data", Comparatori.Uguale, record.DataConvocazione, DataTypes.Data)
                    dam.QB.AddWhereCondition("pri_cic_codice", Comparatori.Uguale, record.Ciclo, DataTypes.Stringa)
                    dam.QB.AddWhereCondition("pri_sed_n_seduta", Comparatori.Uguale, record.Seduta, DataTypes.Numero)
                    dam.ExecNonQuery(ExecQueryType.Update)

                    ' Eliminazione del record in t_cnv_cicli
                    dam.QB.NewQuery()
                    dam.QB.AddTables("t_cnv_cicli")
                    dam.QB.AddWhereCondition("cnc_cnv_paz_codice", Comparatori.Uguale, record.PazienteCodice, DataTypes.Numero)
                    dam.QB.AddWhereCondition("cnc_cnv_data", Comparatori.Uguale, record.DataConvocazione, DataTypes.Data)
                    dam.QB.AddWhereCondition("cnc_cic_codice", Comparatori.Uguale, record.Ciclo, DataTypes.Stringa)
                    dam.QB.AddWhereCondition("cnc_sed_n_seduta", Comparatori.Uguale, record.Seduta, DataTypes.Numero)
                    dam.ExecNonQuery(ExecQueryType.Delete)

                    ' --- spostamento programmate --- '
                    For Each vaccinazione As Vaccinazione In record.Vaccinazioni

                        dam.QB.NewQuery()
                        dam.QB.AddTables("t_vac_programmate")
                        dam.QB.AddUpdateField("vpr_cnv_data", dataNuovaConvocazione, DataTypes.Data)
                        dam.QB.AddWhereCondition("vpr_paz_codice", Comparatori.Uguale, record.PazienteCodice, DataTypes.Numero)
                        dam.QB.AddWhereCondition("vpr_cnv_data", Comparatori.Uguale, record.DataConvocazione, DataTypes.Data)
                        dam.QB.AddWhereCondition("vpr_cic_codice", Comparatori.Uguale, record.Ciclo, DataTypes.Stringa)
                        dam.QB.AddWhereCondition("vpr_n_seduta", Comparatori.Uguale, record.Seduta, DataTypes.Numero)
                        dam.QB.AddWhereCondition("vpr_vac_codice", Comparatori.Uguale, vaccinazione.Codice, DataTypes.Stringa)

                        dam.ExecNonQuery(ExecQueryType.Update)

                    Next

                    ' elimino la cnv se no bil e no vac prog
                    Using bizConvocazione As New Biz.BizConvocazione(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)

                        Dim command As New Biz.BizConvocazione.EliminaConvocazioneEmptyCommand()
                        command.CodicePaziente = record.PazienteCodice
                        command.DataConvocazione = record.DataConvocazione
                        command.DataEliminazione = DateTime.Now
                        command.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.Sollecito
                        command.NoteEliminazione = "Spostamento convocazione da Solleciti"
                        command.WriteLog = True

                        bizConvocazione.EliminaConvocazioneEmpty(command)

                    End Using

                Else

                    Me.InserisciVaccinazioniEscluse(record.PazienteCodice, record.Vaccinazioni, Me.Settings.CODESCLNONOBBL,
                                                    dam, vaccinazioneEsclusaAggiuntaList, vaccinazioneEsclusaEliminataList)

                End If

            End Using

        End Sub

        'controlla se è presente una convocazione obbligatoria successiva [modifica 01/03/2005]
        Private Function CnvObbligatoriaSuccessiva(dam As IDAM, codPaziente As Integer, Optional cnvData As Date = #1/1/1900#) As Object

            With dam.QB
                .NewQuery()
                .AddSelectFields("CNV_DATA")
                .AddTables("T_CNV_CONVOCAZIONI", "T_VAC_PROGRAMMATE", "T_ANA_VACCINAZIONI")
                'join
                .AddWhereCondition("CNV_PAZ_CODICE", Comparatori.Uguale, "VPR_PAZ_CODICE", DataTypes.Join)
                .AddWhereCondition("CNV_DATA", Comparatori.Uguale, "VPR_CNV_DATA", DataTypes.Join)
                .AddWhereCondition("VPR_VAC_CODICE", Comparatori.Uguale, "VAC_CODICE", DataTypes.Join)
                'filtro su paziente e convocazione
                .AddWhereCondition("CNV_PAZ_CODICE", Comparatori.Uguale, codPaziente, DataTypes.Numero)
                .AddWhereCondition("CNV_DATA", Comparatori.Maggiore, IIf(cnvData <> #1/1/1900#, cnvData, Date.Now), DataTypes.Data)
                'filtro sulle vaccinazioni obbligatorie
                .AddWhereCondition("VAC_OBBLIGATORIA", Comparatori.Uguale, Constants.ObbligatorietaVaccinazione.Obbligatoria, DataTypes.Stringa)
            End With

            Return dam.ExecScalar()

        End Function

#End Region

#Region " No Ciclo "

        Private Sub AddPazientiNoCicli(reader As IDataReader, dtaStatiAnagrafici As DataTable)

            Dim codicePaziente As String = reader("Codice")
            Dim dataConvocazione As Date = reader("DataConvocazione")

            Dim pazienteNoCiclo As PazienteNoCiclo = Nothing

            For Each pazienteNoCicloTemp As PazienteNoCiclo In Me.PazientiNoCicli
                If pazienteNoCicloTemp.PazienteCodice = codicePaziente AndAlso pazienteNoCicloTemp.DataConvocazione = dataConvocazione Then

                    pazienteNoCiclo = pazienteNoCicloTemp
                    Exit For

                End If
            Next

            If pazienteNoCiclo Is Nothing Then

                pazienteNoCiclo = New PazienteNoCiclo()

                pazienteNoCiclo.PazienteCodice = reader("Codice").ToString()
                pazienteNoCiclo.DataConvocazione = reader("DataConvocazione")
                pazienteNoCiclo.Nome = reader("Nome").ToString()
                pazienteNoCiclo.Cognome = reader("Cognome").ToString()

                pazienteNoCiclo.DataAppuntamento = IIf(reader("DataAppuntamento") Is DBNull.Value, Date.MinValue, reader("DataAppuntamento"))
                pazienteNoCiclo.DataNascita = IIf(reader("DataNascita") Is DBNull.Value, Date.MinValue, reader("DataNascita"))
                pazienteNoCiclo.CodiceStatoAnagrafico = IIf(reader("CodiceStatoAnagrafico") Is DBNull.Value, Nothing, reader("CodiceStatoAnagrafico"))
                pazienteNoCiclo.DescStatoAnagrafico = dtaStatiAnagrafici.Rows.Find(pazienteNoCiclo.CodiceStatoAnagrafico).Item("san_descrizione")

                Me.PazientiNoCicli.Add(pazienteNoCiclo)

            End If

            Dim vaccinazione As New Vaccinazione()
            vaccinazione.Codice = reader("VaccinazioneCodice")
            vaccinazione.Obbligatoria = reader("VaccinazioneObbligatoria") = Constants.ObbligatorietaVaccinazione.Obbligatoria

            pazienteNoCiclo.Vaccinazioni.Add(vaccinazione)

        End Sub

        Private Sub ProcessaPazientiNoCiclo()

            Dim now As DateTime = DateTime.Now

            ' Parametro da valorizzare nel web.config se si vuol processare solo un determinato numero di sollecito, per test!
            Dim max As Integer? = GetIntegerAppSettingValue("MaxProcessaPazientiSolleciti")

            For i As Integer = 0 To PazientiNoCicli.Count - 1

                If max.HasValue AndAlso max.Value > 0 Then

                    If i >= max Then Exit For

                End If

                Dim pazienteNoCiclo As PazienteNoCiclo = PazientiNoCicli(0)

                Try
                    Using transactionScope As New System.Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, OnVacUtility.GetReadCommittedTransactionOptions())

                        Dim datiVaccinaliCentraleAcquisitiSuccess As Boolean = True
                        Dim vaccinazioneEsclusaMotivoCentraleAggiuntaList As New List(Of Entities.VaccinazioneEsclusa)()
                        Dim vaccinazioneEsclusaMotivoCentraleEliminataList As New List(Of Entities.VaccinazioneEsclusa)()

                        Using dam As IDAM = OnVacUtility.OpenDam()

                            If Me.Settings.ESCLUDISENOCICLO Then

                                Me.InserisciVaccinazioniEscluse(pazienteNoCiclo.PazienteCodice, pazienteNoCiclo.Vaccinazioni, Me.Settings.CODESCLNOCICLO,
                                                                dam, vaccinazioneEsclusaMotivoCentraleAggiuntaList, vaccinazioneEsclusaMotivoCentraleEliminataList)

                            Else

                                ' Cancellazione dati appuntamento nella convocazione
                                Using genericProvider As New DAL.DbGenericProvider(dam)
                                    Using bizConvocazione As New Biz.BizConvocazione(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                                        Dim convocazioneAppuntamento As Entities.ConvocazioneAppuntamento = bizConvocazione.CreateConvocazioneAppuntamentoToDelete(
                                            pazienteNoCiclo.PazienteCodice, pazienteNoCiclo.DataConvocazione, Nothing, now, Constants.MotiviEliminazioneAppuntamento.Sollecito,
                                            "Solleciti: eliminazione appuntamento per creazione sollecito")

                                        bizConvocazione.UpdateConvocazione_DatiAppuntamento(convocazioneAppuntamento)

                                    End Using
                                End Using

                                AggiornaNote(dam, pazienteNoCiclo)

                            End If

                        End Using

                        Me.AggiornaDatiVaccinaliCentrali(pazienteNoCiclo.PazienteCodice, vaccinazioneEsclusaMotivoCentraleAggiuntaList, vaccinazioneEsclusaMotivoCentraleEliminataList)

                        transactionScope.Complete()

                    End Using

                Catch ex As Exception
                    Common.Utility.EventLogHelper.EventLogWrite(ex, OnVacContext.AppId)
                End Try

            Next

        End Sub

        Private Sub AggiornaDatiVaccinaliCentrali(pazCodice As Integer, vaccinazioneEsclusaMotivoCentraleAggiuntaList As List(Of Entities.VaccinazioneEsclusa), vaccinazioneEsclusaMotivoCentraleEliminataList As List(Of Entities.VaccinazioneEsclusa))

            If FlagAbilitazioneVaccUslCorrente Then

                Using dbGenericProviderFactory As New Biz.DbGenericProviderFactory()
                    Using bizPaziente As New Biz.BizPaziente(dbGenericProviderFactory, Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.ESCLUSIONE, True))

                        Dim aggiornaDatiVaccinaliCentraliCommand As New Biz.BizPaziente.AggiornaDatiVaccinaliCentraliCommand()
                        aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale = bizPaziente.GenericProvider.Paziente.GetCodiceAusiliario(pazCodice)
                        aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEsclusaEnumerable = vaccinazioneEsclusaMotivoCentraleAggiuntaList.AsEnumerable()
                        aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEsclusaEliminataEnumerable = vaccinazioneEsclusaMotivoCentraleEliminataList.AsEnumerable()

                        bizPaziente.AggiornaDatiVaccinaliCentrali(aggiornaDatiVaccinaliCentraliCommand)

                    End Using
                End Using

            End If

        End Sub

#End Region

#Region " Disallineati "

        Private Sub AddPazientiDisallineati(reader As IDataReader, dtaStatiAnagrafici As DataTable)

            Dim codicePaziente As String = reader("Codice")
            Dim dataConvocazione As Date = reader("DataConvocazione")

            Dim pazienteDisallineato As PazienteDisallineato = Nothing

            For Each pazienteDisallineatoTemp As PazienteDisallineato In PazientiDisallineati
                If pazienteDisallineatoTemp.PazienteCodice = codicePaziente AndAlso pazienteDisallineatoTemp.DataConvocazione = dataConvocazione Then

                    pazienteDisallineato = pazienteDisallineatoTemp
                    Exit For

                End If
            Next

            If pazienteDisallineato Is Nothing Then

                pazienteDisallineato = New PazienteDisallineato()

                pazienteDisallineato.PazienteCodice = reader("Codice").ToString()
                pazienteDisallineato.DataConvocazione = reader("DataConvocazione")
                pazienteDisallineato.Nome = reader("Nome").ToString()
                pazienteDisallineato.Cognome = reader("Cognome").ToString()

                pazienteDisallineato.DataAppuntamento = IIf(reader("DataAppuntamento") Is DBNull.Value, Date.MinValue, reader("DataAppuntamento"))
                pazienteDisallineato.DataNascita = IIf(reader("DataNascita") Is DBNull.Value, Date.MinValue, reader("DataNascita"))
                pazienteDisallineato.CodiceStatoAnagrafico = IIf(reader("CodiceStatoAnagrafico") Is DBNull.Value, Nothing, reader("CodiceStatoAnagrafico"))
                pazienteDisallineato.DescStatoAnagrafico = dtaStatiAnagrafici.Rows.Find(pazienteDisallineato.CodiceStatoAnagrafico).Item("san_descrizione")

                PazientiDisallineati.Add(pazienteDisallineato)

            End If

            Dim vaccinazione As New Vaccinazione()
            vaccinazione.Codice = reader("VaccinazioneCodice")
            vaccinazione.Obbligatoria = reader("VaccinazioneObbligatoria") = Constants.ObbligatorietaVaccinazione.Obbligatoria

            pazienteDisallineato.Vaccinazioni.Add(vaccinazione)

        End Sub

#End Region

#Region " Update note paziente per sollecito "

        ''' <summary>
        ''' Aggiorna le note sollecito inserendo i dati dell'appuntamento e del ciclo al quale il paziente non è venuto generando il sollecito
        ''' </summary>
        ''' <param name="dam"></param>
        ''' <param name="pazienteCicloObbligatorioNoTP"></param>
        ''' <remarks></remarks>
        Private Sub AggiornaNote(ByRef dam As IDAM, pazienteCicloObbligatorioNoTP As PazienteCicloObbligatorioNoTP)

            Using genericProvider As New DAL.DbGenericProvider(dam)
                Using bizPaziente As New Biz.BizPaziente(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                    bizPaziente.UpdateNotePazienteSollecito(pazienteCicloObbligatorioNoTP.PazienteCodice,
                                                            pazienteCicloObbligatorioNoTP.NumSollecitoSeduta,
                                                            pazienteCicloObbligatorioNoTP.DataAppuntamento,
                                                            pazienteCicloObbligatorioNoTP.DataConvocazione,
                                                            pazienteCicloObbligatorioNoTP.Ciclo,
                                                            pazienteCicloObbligatorioNoTP.Seduta)
                End Using
            End Using

        End Sub

        Private Sub AggiornaNote(ByRef dam As IDAM, pazienteCicloNonObbligatorio As PazienteCicloNonObbligatorio)

            Using genericProvider As New DAL.DbGenericProvider(dam)
                Using bizPaziente As New Biz.BizPaziente(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                    bizPaziente.UpdateNotePazienteSollecito(pazienteCicloNonObbligatorio.PazienteCodice,
                                                            pazienteCicloNonObbligatorio.NumSollecitoSeduta,
                                                            pazienteCicloNonObbligatorio.DataAppuntamento,
                                                            pazienteCicloNonObbligatorio.DataConvocazione,
                                                            pazienteCicloNonObbligatorio.Ciclo,
                                                            pazienteCicloNonObbligatorio.Seduta)
                End Using
            End Using

        End Sub

        Private Sub AggiornaNote(ByRef dam As IDAM, pazienteCicloNonObbligatorio As PazienteNoCiclo)

            Using genericProvider As New DAL.DbGenericProvider(dam)
                Using bizPaziente As New Biz.BizPaziente(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                    bizPaziente.UpdateNotePazienteSollecito(pazienteCicloNonObbligatorio.PazienteCodice,
                                                            pazienteCicloNonObbligatorio.DataAppuntamento,
                                                            pazienteCicloNonObbligatorio.DataConvocazione)
                End Using
            End Using

        End Sub

#End Region

        Private Sub CreaSollecitoRitardo(ByRef dam As IDAM, Optional pno As PazienteCicloNonObbligatorio = Nothing, Optional pds As PazienteCicloObbligatorioNoTP = Nothing)

            Dim record As Object = Nothing

            If Not pno Is Nothing Then
                record = pno
            ElseIf Not pds Is Nothing Then
                record = pds
            End If

            Dim _obj As Object = Nothing

            Dim numeroSollecitoDaCreare As Short = record.NumSollecitoSeduta + 1

            'update T_CNV_CICLI
            With dam.QB
                .NewQuery()
                .AddTables("T_CNV_CICLI")
                .AddUpdateField("CNC_N_SOLLECITO", numeroSollecitoDaCreare, DataTypes.Numero)
                .AddUpdateField("CNC_DATA_INVIO_SOLLECITO", "NULL", DataTypes.Replace)
                .AddWhereCondition("CNC_CNV_PAZ_CODICE", Comparatori.Uguale, record.PazienteCodice, DataTypes.Numero)
                .AddWhereCondition("CNC_CNV_DATA", Comparatori.Uguale, record.DataConvocazione, DataTypes.Data)
                .AddWhereCondition("CNC_CIC_CODICE", Comparatori.Uguale, record.Ciclo, DataTypes.Stringa)
                .AddWhereCondition("CNC_SED_N_SEDUTA", Comparatori.Uguale, record.Seduta, DataTypes.Numero)
            End With

            dam.ExecNonQuery(ExecQueryType.Update)

            'controlla se il ritardo è già presente in tabella
            With dam.QB
                .NewQuery()
                .AddSelectFields("1")
                .AddTables("T_PAZ_RITARDI")
                .AddWhereCondition("PRI_PAZ_CODICE", Comparatori.Uguale, record.PazienteCodice, DataTypes.Numero)
                .AddWhereCondition("PRI_CNV_DATA", Comparatori.Uguale, record.DataConvocazione, DataTypes.Data)
                .AddWhereCondition("PRI_CIC_CODICE", Comparatori.Uguale, record.Ciclo, DataTypes.Stringa)
                .AddWhereCondition("PRI_SED_N_SEDUTA", Comparatori.Uguale, record.Seduta, DataTypes.Numero)
            End With

            _obj = dam.ExecScalar()

            If _obj Is Nothing Then

                'Crea il ritardo per la convocazione
                With dam.QB

                    .NewQuery()
                    .AddTables("T_PAZ_RITARDI")

                    .AddInsertField("PRI_PAZ_CODICE", record.PazienteCodice, DataTypes.Numero)
                    .AddInsertField("PRI_CNV_DATA", record.DataConvocazione, DataTypes.Data)
                    .AddInsertField("PRI_CIC_CODICE", record.Ciclo, DataTypes.Stringa)
                    .AddInsertField("PRI_SED_N_SEDUTA", record.Seduta, DataTypes.Numero)

                    If (record.DataAppuntamento = Date.MinValue) Then
                        .AddInsertField("PRI_DATA_APPUNTAMENTO" + numeroSollecitoDaCreare.ToString, "NULL", DataTypes.Replace)
                    Else
                        .AddInsertField("PRI_DATA_APPUNTAMENTO" + numeroSollecitoDaCreare.ToString, record.DataAppuntamento, DataTypes.DataOra)
                    End If

                    If numeroSollecitoDaCreare = 1 Then

                        If (record.DataInvio = Date.MinValue) Then
                            .AddInsertField("PRI_DATA_INVIO1", "NULL", DataTypes.Replace)
                        Else
                            .AddInsertField("PRI_DATA_INVIO1", record.DataInvio, DataTypes.Data)
                        End If

                    End If

                End With

                dam.ExecNonQuery(ExecQueryType.Insert)

            Else

                'Aggiorna il ritardo per la convocazione
                With dam.QB

                    .NewQuery()
                    .AddTables("T_PAZ_RITARDI")

                    If (record.DataAppuntamento = Date.MinValue) Then
                        .AddUpdateField("PRI_DATA_APPUNTAMENTO" + numeroSollecitoDaCreare.ToString, "NULL", DataTypes.Replace)
                    Else
                        .AddUpdateField("PRI_DATA_APPUNTAMENTO" + numeroSollecitoDaCreare.ToString, record.DataAppuntamento, DataTypes.DataOra)
                    End If

                    .AddWhereCondition("PRI_PAZ_CODICE", Comparatori.Uguale, record.PazienteCodice, DataTypes.Numero)
                    .AddWhereCondition("PRI_CNV_DATA", Comparatori.Uguale, record.DataConvocazione, DataTypes.Data)
                    .AddWhereCondition("PRI_CIC_CODICE", Comparatori.Uguale, record.Ciclo, DataTypes.Stringa)
                    .AddWhereCondition("PRI_SED_N_SEDUTA", Comparatori.Uguale, record.Seduta, DataTypes.Numero)

                End With

                dam.ExecNonQuery(ExecQueryType.Update)

            End If

            ' Cancellazione dati appuntamento nella convocazione
            Dim codicePaziente As Long = Convert.ToInt64(record.PazienteCodice)
            Dim dataConvocazione As DateTime = Convert.ToDateTime(record.DataConvocazione)

            Using genericProvider As New DAL.DbGenericProvider(dam)
                Using bizConvocazione As New Biz.BizConvocazione(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                    Dim convocazioneAppuntamento As Entities.ConvocazioneAppuntamento = bizConvocazione.CreateConvocazioneAppuntamentoToDelete(
                        codicePaziente, dataConvocazione, Nothing, DateTime.Now, Constants.MotiviEliminazioneAppuntamento.Sollecito,
                        "Solleciti: eliminazione appuntamento per creazione sollecito")

                    bizConvocazione.UpdateConvocazione_DatiAppuntamento(convocazioneAppuntamento)

                End Using
            End Using

        End Sub

        Private Sub EliminaPazientiRecordsConvocazioneInesistente(pazientiRecords As ArrayList)

            ' Parametro da valorizzare nel web.config se si vuol processare solo un determinato numero di sollecito, per test!
            Dim max As Integer? = GetIntegerAppSettingValue("MaxProcessaPazientiSolleciti")

            Dim contextInfos As Biz.BizContextInfos = OnVacContext.CreateBizContextInfos()

            For j As Int16 = pazientiRecords.Count - 1 To 0 Step -1

                ' Parametro da valorizzare nel web.config se si vuol processare solo un determinato numero di sollecito, per test!
                If max.HasValue AndAlso max.Value > 0 Then

                    If j >= max Then Exit For

                End If

                Using dam As IDAM = OnVacUtility.OpenDam()
                    Using genericProvider As New DAL.DbGenericProvider(dam)

                        Dim isTotalmenteInadempiente As Boolean = False

                        Using bizPaziente As New Biz.BizPaziente(genericProvider, Me.Settings, contextInfos, Nothing)

                            isTotalmenteInadempiente = bizPaziente.IsTotalmenteInadempiente(Convert.ToInt64(pazientiRecords(j).PazienteCodice))

                        End Using

                        If isTotalmenteInadempiente Then
                            '--
                            Dim existsCnv As Boolean = False
                            '--
                            Using bizCnv As New Biz.BizConvocazione(genericProvider, Me.Settings, contextInfos, Nothing)
                                existsCnv = bizCnv.Exists(pazientiRecords(j).PazienteCodice, pazientiRecords(j).DataConvocazione)
                            End Using
                            '--
                            If existsCnv Then
                                '--
                                Using bizVaccinazioneProg As New Biz.BizVaccinazioneProg(genericProvider, Me.Settings, contextInfos, Nothing)

                                    For i As Int16 = pazientiRecords(j).Vaccinazioni.Count - 1 To 0 Step -1
                                        '--
                                        If Not bizVaccinazioneProg.EsisteVaccinazioneProgrammataByConvocazione(pazientiRecords(j).PazienteCodice, pazientiRecords(j).Vaccinazioni(i).Codice, pazientiRecords(j).DataConvocazione) Then
                                            '--
                                            pazientiRecords(j).Vaccinazioni.RemoveAt(i)
                                            '--
                                        End If
                                        '--
                                    Next

                                End Using
                                '--
                            Else
                                '--
                                pazientiRecords.RemoveAt(j)
                                '--
                            End If
                            '--
                        End If

                    End Using
                End Using

                If pazientiRecords(j).Vaccinazioni.Count = 0 Then pazientiRecords.RemoveAt(j)
                '--
            Next
            '--
        End Sub

        Private Function GetIntegerAppSettingValue(key As String) As Integer?

            Dim value As String = ConfigurationManager.AppSettings.Get(key)

            If Not String.IsNullOrWhiteSpace(value) Then

                Dim count As Integer

                If Integer.TryParse(value, count) Then
                    Return count
                End If

            End If

            Return Nothing

        End Function

#End Region

    End Class

End Namespace