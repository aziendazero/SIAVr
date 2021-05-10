Imports System.Collections.Generic

Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.Collection
Imports Onit.OnAssistnet.OnVac.Log.DataLogStructure


Public Class BizVaccinazioneProg
    Inherits BizClass

#Region " Constructors "

    Public Sub New(dbGenericProviderFactory As DbGenericProviderFactory, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)
        MyBase.New(dbGenericProviderFactory, settings, Nothing, contextInfos, logOptions)
    End Sub

    Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)
        MyBase.New(genericprovider, settings, contextInfos, logOptions)
    End Sub

    Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos)
        Me.New(genericprovider, settings, contextInfos, New BizLogOptions(Log.DataLogStructure.TipiArgomento.VAC_PROGRAMMATE))
    End Sub

#End Region

#Region " Public "

#Region " Shared "

    ''' <summary>
    ''' Controlla se nel datatable specificato ci sono vaccinazioni da effettuare
    ''' Restituisce TRUE se ci sono vaccinazioni da eseguire o vaccinazioni escluse ma scadute (in base al flag)
    ''' Restituisce FALSE se tutte le vaccinazioni sono marcate come eseguite o escluse (non scadute)
    ''' </summary>
    ''' <param name="dtVaccinazioniProgrammate"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function EsistonoVaccinazioniDaEseguire(dtVaccinazioniProgrammate As DataTable, dataConvocazione As DateTime) As Boolean

        ' colonna "E": flag per indicare "eseguita" o "esclusa"

        If dtVaccinazioniProgrammate Is Nothing Then Return False

        For Each row As DataRow In dtVaccinazioniProgrammate.Rows

            If row.RowState <> DataRowState.Deleted Then

                If row("E") Is DBNull.Value Then

                    Return True

                ElseIf row("E") = "esclusa" AndAlso
                       Not row("vex_data_scadenza") Is DBNull.Value AndAlso
                        row("vex_data_scadenza") <> DateTime.MinValue AndAlso
                        row("vex_data_scadenza") <= dataConvocazione Then

                    Return True

                End If

            End If

        Next

        Return False

    End Function

    ''' <summary>
    ''' Restituisce True se la vaccinazione è eseguita o esclusa non scaduta (in base alla data di scadenza)
    ''' </summary>
    ''' <param name="rowVaccinazioneProgrammata"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function IsEsclusaEseguita(rowVaccinazioneProgrammata As DataRow) As Boolean

        Return rowVaccinazioneProgrammata("E").ToString() = "eseguita" OrElse
               (rowVaccinazioneProgrammata("E").ToString() = "esclusa" AndAlso (rowVaccinazioneProgrammata("vex_data_scadenza") Is DBNull.Value OrElse
                                                                                rowVaccinazioneProgrammata("vex_data_scadenza") > Date.Now()))
    End Function

#End Region

#Region " Metodi di Selezione "

    ''' <summary>
    ''' Restituisce un datatable con tutte le vaccinazioni presenti, ordinate per descrizione
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDataTableCodiceDescrizioneVaccinazioni() As DataTable

        Return Me.GenericProvider.AnaVaccinazioni.GetDataTableCodiceDescrizioneVaccinazioni("VAC_DESCRIZIONE")

    End Function

    Public Function EsisteVaccinazioneProgrammataByConvocazione(codicePaziente As Integer, codiceVaccinazione As String, dataConvocazione As DateTime) As Boolean

        Return Me.GenericProvider.VaccinazioneProg.ExistsVaccinazioneProgrammataByConvocazione(codicePaziente, codiceVaccinazione, dataConvocazione)

    End Function

    ''' <summary>
    ''' Restituisce il numero di vaccinazioni programmate per il paziente specificato
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="isGestioneCentrale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountProgrammatePaziente(codicePaziente As String, isGestioneCentrale As Boolean) As Integer

        If isGestioneCentrale Then Return 0

        Return Me.GenericProvider.VaccinazioneProg.CountProgrammatePaziente(codicePaziente)

    End Function

    ''' <summary>
    ''' Restituisce un datatable con i lotti utilizzabili per l'esecuzione delle vaccinazioni programmate, in base ai dati specificati
    ''' </summary>
    ''' <param name="dataConvocazione"></param>
    ''' <param name="dtVaccinazioniProgrammate"></param>
    ''' <param name="codiceConsultorio"></param>
    ''' <param name="codiceConsultorioMagazzino"></param>
    ''' <param name="sessoPaziente"></param>
    ''' <param name="etaPaziente"></param>
    ''' <param name="includiLottiFuoriEta"></param>
    ''' <param name="soloLottiAttivi"></param>
    ''' <param name="includiImportiDefault"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LoadLottiUtilizzabili(dataConvocazione As DateTime, dtVaccinazioniProgrammate As DataTable, codiceConsultorio As String, codiceConsultorioMagazzino As String, sessoPaziente As String, etaPaziente As Double, includiLottiFuoriEta As Boolean, soloLottiAttivi As Boolean, includiImportiDefault As Boolean) As DataTable

        Dim dtLottiUtilizzabili As New DataTable()

        dtLottiUtilizzabili.Columns.Add(New DataColumn("codLotto", GetType(String)))
        dtLottiUtilizzabili.Columns.Add(New DataColumn("descLotto", GetType(String)))
        dtLottiUtilizzabili.Columns.Add(New DataColumn("codNC", GetType(String)))
        dtLottiUtilizzabili.Columns.Add(New DataColumn("descNC", GetType(String)))
        dtLottiUtilizzabili.Columns.Add(New DataColumn("scadenza", GetType(Date)))
        dtLottiUtilizzabili.Columns.Add(New DataColumn("valVac", GetType(String)))
        dtLottiUtilizzabili.Columns.Add(New DataColumn("nValVac", GetType(Integer)))
        dtLottiUtilizzabili.Columns.Add(New DataColumn("giacenza", GetType(Integer)))
        dtLottiUtilizzabili.Columns.Add(New DataColumn("eta_inizio", GetType(Integer)))
        dtLottiUtilizzabili.Columns.Add(New DataColumn("eta_fine", GetType(Integer)))
        dtLottiUtilizzabili.Columns.Add(New DataColumn("nValAss", GetType(Integer)))
        dtLottiUtilizzabili.Columns.Add(New DataColumn("fuori_eta", GetType(Boolean)))
        dtLottiUtilizzabili.Columns.Add(New DataColumn("attivo", GetType(Boolean)))
        dtLottiUtilizzabili.Columns.Add(New DataColumn("lcn_cns_codice", GetType(String)))
        dtLottiUtilizzabili.Columns.Add(New DataColumn("lcn_eta_min_attivazione", GetType(Integer)))
        dtLottiUtilizzabili.Columns.Add(New DataColumn("lcn_eta_max_attivazione", GetType(Integer)))
        dtLottiUtilizzabili.Columns.Add(New DataColumn("importo", GetType(Double)))
        dtLottiUtilizzabili.Columns.Add(New DataColumn("flag_importo", GetType(Integer)))
        dtLottiUtilizzabili.Columns.Add(New DataColumn("flag_esenzione", GetType(Integer)))
        dtLottiUtilizzabili.Columns.Add(New DataColumn("noc_tpa_guid_tipi_pagamento", GetType(Byte())))

        dtLottiUtilizzabili.PrimaryKey = New DataColumn() {dtLottiUtilizzabili.Columns("codLotto")}

        If EsistonoVaccinazioniDaEseguire(dtVaccinazioniProgrammate, dataConvocazione) Then

            ' Determina la data da cui calcolare la scadenza del lotto (il lotto deve scadere dopo tale data).
            Dim dataMax As DateTime = Me.FindMaxDataEffettuazione(dtVaccinazioniProgrammate)
            If dataMax = DateTime.MinValue OrElse dataMax < DateTime.Today Then
                dataMax = DateTime.Now
            End If

            Dim etaPazAggiustata As Double = Math.Round(etaPaziente / Me.Settings.AGGGIORNI)

            ' Determina i lotti associabili alle vaccinazioni, escludendo quelli che scadono prima della dataMax
            Dim dtLottiVac As DataTable =
                Me.GenericProvider.Lotti.LoadLottiUtilizzabili(codiceConsultorio, codiceConsultorioMagazzino, Me.Settings.ASSOCIAZIONI_TIPO_CNS, sessoPaziente, etaPazAggiustata,
                                                               dataMax, includiLottiFuoriEta, soloLottiAttivi, Me.Settings.ASSOCIA_LOTTI_ETA, includiImportiDefault)
            Dim i As Int16

            Dim newrow As DataRow = Nothing

            Dim found As Boolean = False

            Dim valenzeVaccinazioniTemp As List(Of String) = Nothing
            Dim valenzeAssociazioniTemp As List(Of String) = Nothing

            Dim lottoTemp As String = String.Empty

            For i = 0 To dtLottiVac.Rows.Count - 1

                Dim codiceLotto As String = dtLottiVac.Rows(i)("lot_codice")

                If lottoTemp <> codiceLotto Then
                    '--
                    If found Then
                        newrow("valVac") = String.Join("|", valenzeVaccinazioniTemp.ToArray())
                        newrow("nValVac") = valenzeVaccinazioniTemp.Count
                        newrow("nValAss") = valenzeAssociazioniTemp.Count
                        dtLottiUtilizzabili.Rows.Add(newrow)
                    End If
                    '--
                    newrow = dtLottiUtilizzabili.NewRow()
                    '--
                    newrow("codLotto") = codiceLotto
                    newrow("descLotto") = dtLottiVac.Rows(i)("lot_descrizione")
                    newrow("scadenza") = dtLottiVac.Rows(i)("lot_data_scadenza")
                    newrow("codNC") = dtLottiVac.Rows(i)("lot_noc_codice")
                    newrow("descNC") = dtLottiVac.Rows(i)("noc_descrizione")
                    newrow("giacenza") = dtLottiVac.Rows(i)("lcn_dosi_rimaste")
                    newrow("eta_inizio") = dtLottiVac.Rows(i)("noc_eta_inizio")
                    newrow("eta_fine") = dtLottiVac.Rows(i)("noc_eta_fine")
                    newrow("fuori_eta") = (dtLottiVac.Rows(i)("noc_eta_inizio") > etaPazAggiustata OrElse dtLottiVac.Rows(i)("noc_eta_fine") < etaPazAggiustata)
                    newrow("attivo") = (dtLottiVac.Rows(i)("lcn_attivo").ToString() = "S")
                    newrow("lcn_cns_codice") = dtLottiVac.Rows(i)("lcn_cns_codice")
                    newrow("lcn_eta_min_attivazione") = dtLottiVac.Rows(i)("lcn_eta_min_attivazione")
                    newrow("lcn_eta_max_attivazione") = dtLottiVac.Rows(i)("lcn_eta_max_attivazione")
                    newrow("noc_tpa_guid_tipi_pagamento") = dtLottiVac.Rows(i)("noc_tpa_guid_tipi_pagamento")
                    '--
                    ' Valorizzazione importo
                    newrow("importo") = DBNull.Value
                    '--
                    If includiImportiDefault Then
                        '--
                        If Not dtLottiVac.Rows(i)("cpg_auto_set_importo") Is DBNull.Value Then
                            '--
                            If dtLottiVac.Rows(i)("cpg_auto_set_importo").ToString() = "S" Then
                                newrow("importo") = dtLottiVac.Rows(i)("noc_costo_unitario")
                            End If
                            '--
                        ElseIf Not dtLottiVac.Rows(i)("tpa_auto_set_importo") Is DBNull.Value Then
                            '--
                            If dtLottiVac.Rows(i)("tpa_auto_set_importo").ToString() = "S" Then
                                newrow("importo") = dtLottiVac.Rows(i)("noc_costo_unitario")
                            End If
                            '--
                        End If
                        '--
                    End If
                    '--
                    ' Valorizzazione flag stato campi importo ed esenzione.
                    ' Usa per primi i valori impostati nella tabella delle condizioni per fascia di età relative ad un certo nome commerciale.
                    ' Se non presenti, utilizza i valori di default impostati nella tabella dei tipi pagamento (per nome commerciale, ma senza considerare l'età).
                    newrow("flag_importo") = GetStatoAbilitazione(dtLottiVac.Rows(i)("cpg_flag_importo"), dtLottiVac.Rows(i)("tpa_flag_importo"))
                    newrow("flag_esenzione") = GetStatoAbilitazione(dtLottiVac.Rows(i)("cpg_flag_esenzione"), dtLottiVac.Rows(i)("tpa_flag_esenzione"))
                    '--
                    lottoTemp = codiceLotto
                    '--
                    valenzeVaccinazioniTemp = New List(Of String)()
                    valenzeAssociazioniTemp = New List(Of String)()
                    '--
                    found = True
                    '--
                End If

                ' TODO [bug associazione lotti]: è necessario modificare il modo di scelta dei lotti da utilizzare
                '       ADESSO: 
                '       nValVac è il numero di vaccinazioni che vengono coperte da quel lotto, ed è il primo indicatore per la scelta
                '       nValAss è il numero di associazioni che vengono coperte da quel lotto, ed è il secondo indicatore per la scelta
                '       L'errore sta nel fatto che ogni lotto viene considerato a sè e non assieme agli altri, per vedere quale è la combinazione migliore dei lotti che copre tutte le vaccinazioni.
                '       La combinazione migliore dovrebbe essere quella per cui, col minor numero di lotti, si coprono tutte le vaccinazioni con il minor numero di sovrapposizioni.
                '       DOPO LA MODIFICA: 
                '       restituisco una struttura contenente solo i lotti 
                '       nValVac sarà il numero di vaccinazioni "sovrapposte", cioè le vaccinazioni in più che si effettuano combinando i lotti selezionati
                '       nValAss sarà il numero di lotti utilizzati per eseguire tutte le vaccinazioni
                '       Una volta corretto l'algoritmo, il datatable dei lotti utilizzabili dovrà essere ordinato come segue:
                '           dtLottiUtilizzabili.DefaultView.Sort = "nValVac ASC, nValAss ASC, scadenza ASC, giacenza DESC"
                '
                '       Dentro all'If found, l'algoritmo dovrebbe essere il seguente:
                '       
                '       in base a dtVaccinazioniProgrammate, determino quali sono le vaccinazioni da effettuare.
                '       ciclo per ogni lotto di dtLottiVac
                '           se il lotto copre tutte le vaccinazioni da effettuare => trovato lotto

                If found Then

                    For Each rowVaccinazioneProgrammata As DataRow In dtVaccinazioniProgrammate.Rows

                        If Not rowVaccinazioneProgrammata.RowState = DataRowState.Deleted AndAlso
                           Not IsEsclusaEseguita(rowVaccinazioneProgrammata) Then

                            Dim codiceVaccinazioneProgrammata As String = rowVaccinazioneProgrammata("vpr_vac_codice").ToString()

                            If codiceVaccinazioneProgrammata = dtLottiVac.Rows(i)("val_vac_codice") Then

                                If Not valenzeVaccinazioniTemp.Contains(codiceVaccinazioneProgrammata) Then
                                    valenzeVaccinazioniTemp.Add(codiceVaccinazioneProgrammata)
                                End If

                                Dim codiceAssociazioneVaccinazioneProgrammata As String = rowVaccinazioneProgrammata("vpr_ass_codice").ToString()
                                Dim codiceAssociazioneLotto As String = dtLottiVac.Rows(i)("val_ass_codice").ToString()

                                If Not String.IsNullOrEmpty(codiceAssociazioneVaccinazioneProgrammata) AndAlso
                                   Not String.IsNullOrEmpty(codiceAssociazioneLotto) Then

                                    If codiceAssociazioneVaccinazioneProgrammata = codiceAssociazioneLotto Then
                                        valenzeAssociazioniTemp.Add(codiceAssociazioneVaccinazioneProgrammata)
                                    End If

                                End If

                                found = True
                                Exit For

                            Else
                                found = False
                            End If

                        End If
                    Next
                End If
            Next

            If found And i = dtLottiVac.Rows.Count Then

                newrow("valVac") = String.Join("|", valenzeVaccinazioniTemp.ToArray())
                newrow("nValVac") = valenzeVaccinazioniTemp.Count
                newrow("nValAss") = valenzeAssociazioniTemp.Count

                dtLottiUtilizzabili.Rows.Add(newrow)

            End If

            dtLottiUtilizzabili.DefaultView.Sort = "nValVac DESC, nValAss DESC, scadenza ASC, giacenza DESC"

        End If

        Return dtLottiUtilizzabili

    End Function

    ''' <summary>
    ''' Recupera sito di inoculazione e via di somministrazione in base ai criteri seguenti (nell'ordine):
    '''  1- recupera sito e via in base a ciclo/seduta/vaccinazione;
    '''  2- se sito e/o via non recuperati, cerca in base all'associazione;
    '''  3- se sito e/o via non recuperati, cerca in base al nome commerciale.
    ''' La valorizzazione in base al ciclo avviene sempre, quelle successive solo se i parametri 
    ''' SITO_INOCULAZIONE_SET_DEFAULT e VIA_SOMMINISTRAZIONE_SET_DEFAULT sono impostati a true
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetInfoSomministrazione(command As GetInfoSomministrazioneCommand) As Entities.InfoSomministrazione

        Dim infoSomministrazione As New InfoSomministrazione()

        If Not String.IsNullOrEmpty(command.CodiceCiclo) AndAlso command.NumeroSeduta.HasValue AndAlso Not String.IsNullOrEmpty(command.CodiceVaccinazione) Then
            '--
            ' Recupero info su sito inoculazione e via di somministrazione in base al ciclo
            '--
            infoSomministrazione = Me.GenericProvider.Cicli.GetInfoSomministrazioneDefaultCiclo(command.CodiceCiclo, command.NumeroSeduta.Value, command.CodiceVaccinazione)
            '--
            If Not String.IsNullOrEmpty(infoSomministrazione.CodiceSitoInoculazione) Then
                infoSomministrazione.FlagTipoValorizzazioneSito = Constants.TipoValorizzazioneSitoVia.DaCiclo
            End If
            '--
            If Not String.IsNullOrEmpty(infoSomministrazione.CodiceViaSomministrazione) Then
                infoSomministrazione.FlagTipoValorizzazioneVia = Constants.TipoValorizzazioneSitoVia.DaCiclo
            End If
            '--
        End If

        ' Se ho trovato tutti i dati in base al ciclo, mi fermo e non cerco per associazione/nome commerciale
        If Not String.IsNullOrEmpty(infoSomministrazione.CodiceSitoInoculazione) AndAlso
           Not String.IsNullOrEmpty(infoSomministrazione.CodiceViaSomministrazione) Then
            '--
            Return infoSomministrazione
            '--
        End If

        ' Se entrambi i parametri sono impostati a false, non vado avanti
        If Not Me.Settings.SITO_INOCULAZIONE_SET_DEFAULT AndAlso Not Me.Settings.VIA_SOMMINISTRAZIONE_SET_DEFAULT Then
            Return infoSomministrazione
        End If

        If Not String.IsNullOrEmpty(command.CodiceAssociazione) Then
            '--
            ' Recupero info su sito inoculazione e via di somministrazione in base all'associazione
            '--
            Dim infoSomministrazioneAssociazione As InfoSomministrazione =
                Me.GenericProvider.Associazioni.GetInfoSomministrazioneDefaultAssociazione(command.CodiceAssociazione)
            '--
            If Me.Settings.SITO_INOCULAZIONE_SET_DEFAULT AndAlso
               String.IsNullOrEmpty(infoSomministrazione.CodiceSitoInoculazione) AndAlso
               Not String.IsNullOrEmpty(infoSomministrazioneAssociazione.CodiceSitoInoculazione) Then
                '--
                infoSomministrazione.CodiceSitoInoculazione = infoSomministrazioneAssociazione.CodiceSitoInoculazione
                infoSomministrazione.DescrizioneSitoInoculazione = infoSomministrazioneAssociazione.DescrizioneSitoInoculazione
                infoSomministrazione.FlagTipoValorizzazioneSito = Constants.TipoValorizzazioneSitoVia.DaAssociazione
                '--
            End If
            '--
            If Me.Settings.VIA_SOMMINISTRAZIONE_SET_DEFAULT AndAlso
               String.IsNullOrEmpty(infoSomministrazione.CodiceViaSomministrazione) AndAlso
               Not String.IsNullOrEmpty(infoSomministrazioneAssociazione.CodiceViaSomministrazione) Then
                '--
                infoSomministrazione.CodiceViaSomministrazione = infoSomministrazioneAssociazione.CodiceViaSomministrazione
                infoSomministrazione.DescrizioneViaSomministrazione = infoSomministrazioneAssociazione.DescrizioneViaSomministrazione
                infoSomministrazione.FlagTipoValorizzazioneVia = Constants.TipoValorizzazioneSitoVia.DaAssociazione
                '--
            End If
            '--
        End If

        ' Se ho trovato tutti i dati in base a ciclo/associazione, mi fermo e non cerco per nome commerciale
        If Not String.IsNullOrEmpty(infoSomministrazione.CodiceSitoInoculazione) AndAlso
           Not String.IsNullOrEmpty(infoSomministrazione.CodiceViaSomministrazione) Then
            '--
            Return infoSomministrazione
            '--
        End If

        If Not String.IsNullOrEmpty(command.CodiceNomeCommerciale) Then
            '--
            ' Recupero info su sito inoculazione e via di somministrazione in base al nome commerciale
            '--
            Dim infoSomministrazioneNomeCommerciale As InfoSomministrazione =
                Me.GenericProvider.NomiCommerciali.GetInfoSomministrazioneDefaultNomeCommerciale(command.CodiceNomeCommerciale)
            '--
            If Me.Settings.SITO_INOCULAZIONE_SET_DEFAULT AndAlso
               String.IsNullOrEmpty(infoSomministrazione.CodiceSitoInoculazione) AndAlso
               Not String.IsNullOrEmpty(infoSomministrazioneNomeCommerciale.CodiceSitoInoculazione) Then
                '--
                infoSomministrazione.CodiceSitoInoculazione = infoSomministrazioneNomeCommerciale.CodiceSitoInoculazione
                infoSomministrazione.DescrizioneSitoInoculazione = infoSomministrazioneNomeCommerciale.DescrizioneSitoInoculazione
                infoSomministrazione.FlagTipoValorizzazioneSito = Constants.TipoValorizzazioneSitoVia.DaNomeCommerciale
                '--
            End If
            '--
            If Me.Settings.VIA_SOMMINISTRAZIONE_SET_DEFAULT AndAlso
               String.IsNullOrEmpty(infoSomministrazione.CodiceViaSomministrazione) AndAlso
               Not String.IsNullOrEmpty(infoSomministrazioneNomeCommerciale.CodiceViaSomministrazione) Then
                '--
                infoSomministrazione.CodiceViaSomministrazione = infoSomministrazioneNomeCommerciale.CodiceViaSomministrazione
                infoSomministrazione.DescrizioneViaSomministrazione = infoSomministrazioneNomeCommerciale.DescrizioneViaSomministrazione
                infoSomministrazione.FlagTipoValorizzazioneVia = Constants.TipoValorizzazioneSitoVia.DaNomeCommerciale
                '--
            End If
            '--
        End If

        Return infoSomministrazione

    End Function

    ''' <summary>
    ''' Restituisce il datatable delle vaccinazioni programmate del paziente nella data di convocazione specificata.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataConvocazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDtVaccinazioniProgrammatePazienteByData(codicePaziente As String, dataConvocazione As DateTime) As DataTable

        Return GenericProvider.VaccinazioneProg.GetVacProgrammatePazienteByData(codicePaziente, dataConvocazione)

    End Function

#End Region

#Region " Metodi di Delete "

    Public Function EliminaVaccinazioniProgrammate(eliminaVaccinazioniProgrammateCommand As EliminaVaccinazioniProgrammateCommand) As EliminaVaccinazioniProgrammateResult

        Dim eliminaVaccinazioniProgrammateResult As New EliminaVaccinazioniProgrammateResult()

        For Each codiceVaccinazione As String In eliminaVaccinazioniProgrammateCommand.CodiceVaccinazioni
            Dim vacLst As List(Of VaccinazioneProgrammata) = Me.GenericProvider.VaccinazioneProg.GetVaccinazioneProgrammata(eliminaVaccinazioniProgrammateCommand.CodicePaziente, codiceVaccinazione, eliminaVaccinazioniProgrammateCommand.DataConvocazione)
            Dim vacEliminate As Integer = Me.GenericProvider.VaccinazioneProg.EliminaVaccinazioneProgrammata(eliminaVaccinazioniProgrammateCommand.CodicePaziente, codiceVaccinazione, eliminaVaccinazioniProgrammateCommand.DataConvocazione)
            If vacEliminate > 0 Then
                eliminaVaccinazioniProgrammateResult.VaccinazioniProgrammateEliminate.AddRange(vacLst)
            End If
        Next

        If eliminaVaccinazioniProgrammateResult.VaccinazioniProgrammateEliminate.Count > 0 Then
            Me.GenericProvider.Convocazione.EliminaCicliSenzaVaccinazioniProgrammate(
                eliminaVaccinazioniProgrammateCommand.CodicePaziente,
                eliminaVaccinazioniProgrammateCommand.DataConvocazione)
        End If

        Return eliminaVaccinazioniProgrammateResult

    End Function

    ''' <summary>
    ''' Eliminazione vaccinazione programmata del paziente in base al richiamo
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="codiceVaccinazione"></param>
    ''' <param name="richiamo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function EliminaVaccinazioneProgrammataByRichiamo(codicePaziente As Integer, codiceVaccinazione As String, richiamo As Integer) As Integer

        Return Me.GenericProvider.VaccinazioneProg.EliminaVaccinazioneProgrammataByRichiamo(codicePaziente, codiceVaccinazione, richiamo)

    End Function

    Public Class EliminaProgrammazioneCommand
        Public Property CodicePaziente As Integer
        Public Property DataConvocazione As DateTime?
        Public Property EliminaAppuntamenti As Boolean
        Public Property EliminaSollecitiBilancio As Boolean
        Public Property EliminaBilanci As Boolean
        Public Property TipoArgomentoLog As String
        Public Property OperazioneAutomatica As Boolean
        Public Property IdMotivoEliminazione As String
        Public Property NoteEliminazione As String
        Public Property CodiciCicliEliminazioneProgrammazione As List(Of String)
    End Class

    ''' <summary>
    ''' Eliminazione della programmazione del paziente (convocazioni, solleciti di bilancio, bilanci) e valorizzazione storico appuntamenti.
    ''' </summary>
    ''' <param name="command"></param>
    ''' <remarks></remarks>
    Public Sub EliminaProgrammazione(command As EliminaProgrammazioneCommand)

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, GetReadCommittedTransactionOptions())

            ' Caricamento dati programmazione da cancellare
            Dim listProgrammazioniDaEliminare As List(Of ProgrammazioneDaEliminare) =
                GenericProvider.VaccinazioneProg.GetProgrammazioneDaEliminare(
                    command.CodicePaziente, command.DataConvocazione, command.EliminaAppuntamenti, command.EliminaSollecitiBilancio,
                    command.EliminaBilanci, command.CodiciCicliEliminazioneProgrammazione)

            If Not listProgrammazioniDaEliminare Is Nothing AndAlso listProgrammazioniDaEliminare.Count > 0 Then

                Dim testataLog As Testata = Nothing
                Dim recordLog As Record = Nothing

                ' Log
                If Not String.IsNullOrEmpty(command.TipoArgomentoLog) Then
                    testataLog = New Testata(command.TipoArgomentoLog, Operazione.Eliminazione, command.CodicePaziente, command.OperazioneAutomatica)
                End If

                Dim dataEliminazione As DateTime = DateTime.Now

                For Each programmazioneDaEliminare As Entities.ProgrammazioneDaEliminare In listProgrammazioniDaEliminare

                    ' Cancellazione solleciti di bilancio e bilanci della convocazione
                    Dim numSollecitiBilancioEliminati As Integer = 0
                    Dim numBilanciEliminati As Integer = 0

                    If command.EliminaBilanci Then

                        numSollecitiBilancioEliminati = GenericProvider.Convocazione.DeleteSollecitiBilancioConvocazione(programmazioneDaEliminare.CodicePaziente, programmazioneDaEliminare.DataConvocazione)

                        numBilanciEliminati = GenericProvider.Convocazione.DeleteBilancioConvocazione(programmazioneDaEliminare.CodicePaziente, programmazioneDaEliminare.DataConvocazione)

                    End If

                    ' Cancellazione convocazione
                    Dim convocazioneEliminata As Boolean = False

                    Using bizConvocazione As New BizConvocazione(GenericProvider, Settings, ContextInfos, LogOptions)

                        Dim eliminaCnvCommand As New BizConvocazione.EliminaConvocazioniSollecitiBilanciCommand()
                        eliminaCnvCommand.CodicePaziente = programmazioneDaEliminare.CodicePaziente
                        eliminaCnvCommand.DataConvocazione = programmazioneDaEliminare.DataConvocazione
                        eliminaCnvCommand.CancellaBilanciAssociati = False
                        If String.IsNullOrWhiteSpace(command.IdMotivoEliminazione) Then
                            eliminaCnvCommand.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.EliminazioneConvocazione
                        Else
                            eliminaCnvCommand.IdMotivoEliminazione = command.IdMotivoEliminazione
                        End If
                        eliminaCnvCommand.DataEliminazione = dataEliminazione
                        eliminaCnvCommand.NoteEliminazione = command.NoteEliminazione
                        eliminaCnvCommand.WriteLog = False

                        convocazioneEliminata = bizConvocazione.EliminaConvocazioniSollecitiBilanci(eliminaCnvCommand)

                    End Using

                    ' Log
                    If Not String.IsNullOrEmpty(command.TipoArgomentoLog) Then

                        recordLog = New Record()

                        If convocazioneEliminata Then
                            recordLog.Campi.Add(New Campo("CNV_DATA", programmazioneDaEliminare.DataConvocazione.ToString("dd/MM/yyyy")))
                        End If

                        If numSollecitiBilancioEliminati > 0 Then
                            recordLog.Campi.Add(New Campo("Solleciti di bilancio eliminati", numSollecitiBilancioEliminati.ToString()))
                        End If

                        If numBilanciEliminati > 0 Then
                            recordLog.Campi.Add(New Campo("Bilanci eliminati", numBilanciEliminati.ToString()))
                        End If

                        If convocazioneEliminata OrElse (numSollecitiBilancioEliminati + numBilanciEliminati > 0) Then
                            testataLog.Records.Add(recordLog)
                        End If

                    End If

                Next

                ' Scrittura Log
                If Not String.IsNullOrEmpty(command.TipoArgomentoLog) AndAlso testataLog.Records.Count > 0 Then
                    Log.LogBox.WriteData(testataLog)
                End If

            End If

            transactionScope.Complete()

        End Using

    End Sub

    ''' <summary>
    ''' Restituisce il numero di programmate da eliminare per il paziente, in base ai parametri specificati
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataConvocazione"></param>
    ''' <param name="eliminaAppuntamenti"></param>
    ''' <param name="eliminaSollecitiBilancio"></param>
    ''' <param name="eliminaBilanci"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountProgrammazioneDaEliminare(codicePaziente As Integer, dataConvocazione As DateTime?, eliminaAppuntamenti As Boolean, eliminaSollecitiBilancio As Boolean, eliminaBilanci As Boolean) As Integer
        Return Me.GenericProvider.VaccinazioneProg.CountProgrammazioneDaEliminare(codicePaziente, dataConvocazione, eliminaAppuntamenti, eliminaSollecitiBilancio, eliminaBilanci)
    End Function

#End Region

#Region " Metodi di Insert "

    ''' <summary>
    ''' Inserimento dati di vaccinazione specificati, compreso numero richiamo calcolato come il successivo rispetto al massimo richiamo eseguito per la vaccinazione.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataConvocazione"></param>
    ''' <param name="codiceVaccinazione"></param>
    ''' <param name="codiceAssociazione"></param>
    ''' <param name="dataInserimento"></param>
    ''' <param name="idUtenteInserimento"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function InsertVaccinazioneProgrammata(codicePaziente As Integer, dataConvocazione As DateTime, codiceVaccinazione As String, codiceAssociazione As String, dataInserimento As DateTime?, idUtenteInserimento As Long?) As Integer

        Dim numeroRichiamo As Integer = Me.GenericProvider.VaccinazioniEseguite.GetMaxRichiamo(codicePaziente, codiceVaccinazione) + 1

        If Not dataInserimento.HasValue Then
            dataInserimento = DateTime.Now
        End If

        If Not idUtenteInserimento.HasValue Then
            idUtenteInserimento = Me.ContextInfos.IDUtente
        End If

        Return Me.GenericProvider.VaccinazioneProg.InsertVaccinazioneProgrammata(
            codicePaziente, dataConvocazione, codiceVaccinazione, codiceAssociazione, numeroRichiamo, dataInserimento, idUtenteInserimento)

    End Function

#End Region

#Region " Report "

    Public Class ReportVaccinazioniProgrammateCommand

        Public DataAppuntamentoInizio As DateTime
        Public DataAppuntamentoFine As DateTime

        Public DataNascitaInizio As DateTime
        Public DataNascitaFine As DateTime

		Public ListaConsultorioCodice As List(Of String)
		Public ConsultorioDescrizione As String

        Public MedicoCodice As String
        Public MedicoDescrizione As String

        Public ListaCodiciStatiAnagraficiSelezionati As List(Of String)
        Public ListaDescrizioniStatiAnagraficiSelezionati As List(Of String)

        Public NumeroDose As String

        Public ListaCodiciVaccinazioniSelezionate As List(Of String)


        Public Sub New()
            Me.ListaCodiciStatiAnagraficiSelezionati = New List(Of String)()
            Me.ListaDescrizioniStatiAnagraficiSelezionati = New List(Of String)()
			Me.ListaCodiciVaccinazioniSelezionate = New List(Of String)()
			ListaConsultorioCodice = New List(Of String)()
		End Sub

    End Class

    Public Class ReportVaccinazioniProgrammateResult

        Public Success As Boolean

        Public FiltroReport As String

        Private _parametriReport As List(Of KeyValuePair(Of String, String))
        Public Property ParametriReport As List(Of KeyValuePair(Of String, String))
            Get
                Return _parametriReport
            End Get
            Private Set(value As List(Of KeyValuePair(Of String, String)))
                _parametriReport = value
            End Set
        End Property


        Public Sub New(success As Boolean)
            Me.Success = success
            Me.ParametriReport = New List(Of KeyValuePair(Of String, String))()
        End Sub

        Public Sub AddParameter(key As String, value As String)
            Me.ParametriReport.Add(New KeyValuePair(Of String, String)(key, value))
        End Sub

    End Class
    Public Class ReportVaccinazioniAssociazioniProgrammateResult

        Public Success As Boolean

        Public DstStatVacProgAss As StatVacProgAssDST

        Private _parametriReport As List(Of KeyValuePair(Of String, String))
        Public Property ParametriReport As List(Of KeyValuePair(Of String, String))
            Get
                Return _parametriReport
            End Get
            Private Set(value As List(Of KeyValuePair(Of String, String)))
                _parametriReport = value
            End Set
        End Property


        Public Sub New(success As Boolean, dstStatVacProgAss As StatVacProgAssDST)
            Me.Success = success
            Me.ParametriReport = New List(Of KeyValuePair(Of String, String))()
            Me.DstStatVacProgAss = dstStatVacProgAss
        End Sub

        Public Sub AddParameter(key As String, value As String)
            Me.ParametriReport.Add(New KeyValuePair(Of String, String)(key, value))
        End Sub

    End Class

    Public Shared Function GetReportVaccinazioniProgrammate(command As ReportVaccinazioniProgrammateCommand) As ReportVaccinazioniProgrammateResult

        ' Controllo date appuntamento   
        If command.DataAppuntamentoInizio = DateTime.MinValue Or command.DataAppuntamentoFine = DateTime.MinValue Then
            Return New ReportVaccinazioniProgrammateResult(False)
        End If

        ' Controllo date di nascita
        If (command.DataNascitaInizio = DateTime.MinValue And command.DataNascitaFine > DateTime.MinValue) Or
           (command.DataNascitaInizio > DateTime.MinValue And command.DataNascitaFine = DateTime.MinValue) Then
            Return New ReportVaccinazioniProgrammateResult(False)
        End If

        ' Controllo dose
        If Not String.IsNullOrEmpty(command.NumeroDose) Then

            command.NumeroDose = command.NumeroDose.Trim()

            Dim numeroDose As Integer
            If Not Integer.TryParse(command.NumeroDose, numeroDose) Then
                Return New ReportVaccinazioniProgrammateResult(False)
            End If

            If numeroDose <= 0 Then
                Return New ReportVaccinazioniProgrammateResult(False)
            End If

        End If

        Dim result As New ReportVaccinazioniProgrammateResult(True)

        result.AddParameter("DataAppuntamentoIniz", command.DataAppuntamentoInizio.ToString("dd/MM/yyyy"))
        result.AddParameter("DataAppuntamentoFin", command.DataAppuntamentoFine.ToString("dd/MM/yyyy"))

        Dim filtroReport As New System.Text.StringBuilder()

        ' Filtro data appuntamento
        filtroReport.AppendFormat("{{T_CNV_CONVOCAZIONI.CNV_DATA_APPUNTAMENTO}} {0} ",
                                  BizReport.GetBetweenDateReportFilter(command.DataAppuntamentoInizio, command.DataAppuntamentoFine.AddDays(1)))

        ' Filtro consultorio
        If command.ListaConsultorioCodice.Count > 0 Then
			filtroReport.AppendFormat(" AND ({{T_CNV_CONVOCAZIONI.CNV_CNS_CODICE}} IN ['{0}']) ", String.Join("', '", command.ListaConsultorioCodice.ToArray()))
		End If

		' Filtro medico di base
		If Not String.IsNullOrEmpty(command.MedicoCodice) And Not String.IsNullOrEmpty(command.MedicoDescrizione) Then
            filtroReport.AppendFormat(" AND {{T_PAZ_PAZIENTI.PAZ_MED_CODICE_BASE}} = '{0}' ", command.MedicoCodice)
            result.AddParameter("MedicoBase", command.MedicoDescrizione)
        Else
            result.AddParameter("MedicoBase", "TUTTI")
        End If

        ' Filtro data di nascita
        If command.DataNascitaInizio > DateTime.MinValue And command.DataNascitaFine > DateTime.MinValue Then

            filtroReport.AppendFormat(" AND {{T_PAZ_PAZIENTI.PAZ_DATA_NASCITA}} {0} ",
                                      BizReport.GetBetweenDateReportFilter(command.DataNascitaInizio, command.DataNascitaFine))

            result.AddParameter("DataNascitaIniz", command.DataNascitaInizio.ToString("dd/MM/yyyy"))
            result.AddParameter("DataNascitaFin", command.DataNascitaFine.ToString("dd/MM/yyyy"))
        Else
            result.AddParameter("DataNascitaIniz", String.Empty)
            result.AddParameter("DataNascitaFin", String.Empty)
        End If

        ' Stato anagrafico
        If Not command.ListaCodiciStatiAnagraficiSelezionati Is Nothing AndAlso command.ListaCodiciStatiAnagraficiSelezionati.Count > 0 Then

            filtroReport.Append(" AND (")

            For Each codiceStatoAnagraficoSelezionato As String In command.ListaCodiciStatiAnagraficiSelezionati
                filtroReport.AppendFormat(" {{T_PAZ_PAZIENTI.PAZ_STATO_ANAGRAFICO}} = '{0}' OR", codiceStatoAnagraficoSelezionato)
            Next
            filtroReport.Remove(filtroReport.Length - 3, 3)

            filtroReport.Append(") ")

            result.AddParameter("StatoAnagrafico", String.Join(", ", command.ListaDescrizioniStatiAnagraficiSelezionati.ToArray()))

        Else
            result.AddParameter("StatoAnagrafico", "TUTTI")
        End If

        ' Dose
        If Not String.IsNullOrEmpty(command.NumeroDose) Then
            filtroReport.AppendFormat(" AND {{T_VAC_PROGRAMMATE.VPR_N_RICHIAMO}} = {0} ", command.NumeroDose)
            result.AddParameter("NumeroDose", command.NumeroDose)
        Else
            result.AddParameter("NumeroDose", "NON SPECIFICATA")
        End If

        ' Vaccinazioni
        If Not command.ListaCodiciVaccinazioniSelezionate Is Nothing AndAlso command.ListaCodiciVaccinazioniSelezionate.Count > 0 Then

            filtroReport.Append(" AND (")

            For Each codiceVaccinazioneSelezionata As String In command.ListaCodiciVaccinazioniSelezionate
                filtroReport.AppendFormat(" {{T_VAC_PROGRAMMATE.VPR_VAC_CODICE}} = '{0}' OR", codiceVaccinazioneSelezionata)
            Next
            filtroReport.Remove(filtroReport.Length - 3, 3)

            filtroReport.Append(") ")

            result.AddParameter("Vaccinazioni", String.Join(",", command.ListaCodiciVaccinazioniSelezionate.ToArray()))

        Else
            result.AddParameter("Vaccinazioni", "TUTTE")
        End If

        result.FiltroReport = filtroReport.ToString()

        Return result

    End Function

    Public Function GetReportVaccinazioniProgrammateAssociazioni(command As ReportVaccinazioniProgrammateCommand) As ReportVaccinazioniAssociazioniProgrammateResult
        Dim listaStati As String = String.Empty
        Dim listaVacini As String = String.Empty
        Dim dataNascitaInizio As Date = Nothing
        Dim dataNasciataFine As Date = Nothing

        ' Controllo date appuntamento   
        If command.DataAppuntamentoInizio = DateTime.MinValue Or command.DataAppuntamentoFine = DateTime.MinValue Then
            Return New ReportVaccinazioniAssociazioniProgrammateResult(False, New StatVacProgAssDST)
        End If

        ' Controllo date di nascita
        If (command.DataNascitaInizio = DateTime.MinValue And command.DataNascitaFine > DateTime.MinValue) Or
           (command.DataNascitaInizio > DateTime.MinValue And command.DataNascitaFine = DateTime.MinValue) Then
            Return New ReportVaccinazioniAssociazioniProgrammateResult(False, New StatVacProgAssDST)
        End If
        If command.DataNascitaInizio > Date.MinValue And command.DataNascitaFine > DateTime.MinValue Then
            dataNascitaInizio = command.DataNascitaInizio
            dataNasciataFine = command.DataNascitaFine
        End If


        ' Controllo dose
        If Not String.IsNullOrEmpty(command.NumeroDose) Then

            command.NumeroDose = command.NumeroDose.Trim()

            Dim numeroDose As Integer
            If Not Integer.TryParse(command.NumeroDose, numeroDose) Then
                Return New ReportVaccinazioniAssociazioniProgrammateResult(False, New StatVacProgAssDST)
            End If

            If numeroDose <= 0 Then
                Return New ReportVaccinazioniAssociazioniProgrammateResult(False, New StatVacProgAssDST)
            End If

        End If

        Dim result As New ReportVaccinazioniAssociazioniProgrammateResult(True, New StatVacProgAssDST)

        result.AddParameter("DataAppuntamentoIniz", command.DataAppuntamentoInizio.ToString("dd/MM/yyyy"))
        result.AddParameter("DataAppuntamentoFin", command.DataAppuntamentoFine.ToString("dd/MM/yyyy"))



        ' Filtro medico di base
        If Not String.IsNullOrEmpty(command.MedicoCodice) And Not String.IsNullOrEmpty(command.MedicoDescrizione) Then
            result.AddParameter("MedicoBase", command.MedicoDescrizione)
        Else
            result.AddParameter("MedicoBase", "TUTTI")
        End If

        ' Filtro data di nascita
        If command.DataNascitaInizio > DateTime.MinValue And command.DataNascitaFine > DateTime.MinValue Then


            result.AddParameter("DataNascitaIniz", command.DataNascitaInizio.ToString("dd/MM/yyyy"))
            result.AddParameter("DataNascitaFin", command.DataNascitaFine.ToString("dd/MM/yyyy"))
        Else
            result.AddParameter("DataNascitaIniz", String.Empty)
            result.AddParameter("DataNascitaFin", String.Empty)
        End If

        ' Stato anagrafico
        If Not command.ListaCodiciStatiAnagraficiSelezionati Is Nothing AndAlso command.ListaCodiciStatiAnagraficiSelezionati.Count > 0 Then


            result.AddParameter("StatoAnagrafico", String.Join(", ", command.ListaDescrizioniStatiAnagraficiSelezionati.ToArray()))
            listaStati = "'" + String.Join("', '", command.ListaCodiciStatiAnagraficiSelezionati.ToArray()) + "'"

        Else
            result.AddParameter("StatoAnagrafico", "TUTTI")
        End If

        ' Dose
        If Not String.IsNullOrEmpty(command.NumeroDose) Then

            result.AddParameter("NumeroDose", command.NumeroDose)
        Else
            result.AddParameter("NumeroDose", "NON SPECIFICATA")
        End If

        ' Vaccinazioni
        If Not command.ListaCodiciVaccinazioniSelezionate Is Nothing AndAlso command.ListaCodiciVaccinazioniSelezionate.Count > 0 Then

            result.AddParameter("Vaccinazioni", String.Join(",", command.ListaCodiciVaccinazioniSelezionate.ToArray()))
            listaVacini = "'" + String.Join("', '", command.ListaCodiciVaccinazioniSelezionate.ToArray()) + "'"
        Else
            result.AddParameter("Vaccinazioni", "TUTTE")
        End If
		Dim vacinazioni As List(Of Entities.StatVaccinazioneProgrammateAssociate) = GenericProvider.VaccinazioneProg.GetVaccinazioniProgrammateAssociate(command.DataAppuntamentoInizio, command.DataAppuntamentoFine, dataNascitaInizio, dataNasciataFine, command.MedicoCodice, listaStati, listaVacini, command.NumeroDose, command.ListaConsultorioCodice)
        Dim assoProgrammate As New List(Of String)


        Dim associazioniVacinazioni As List(Of Entities.StatVacciniAssociati) = GenericProvider.Associazioni.GetListaVacAss(Nothing)


        Dim listaFinale As New List(Of Entities.StatVacciniAssociatiFinale)
        listaFinale = CalcolaListaVacciniAssociazioni(vacinazioni, associazioniVacinazioni)


        Dim tabella As New StatVacProgAssDST()
        Dim count As Integer = 0
        Dim rowAssVac As DataRow

        For Each b As StatVacciniAssociatiFinale In listaFinale
            rowAssVac = tabella.Tables("StatAssTable").NewRow()
            rowAssVac("CodiceAssociazione") = b.CodiceAssociazione
            rowAssVac("CodiceConsultorio") = b.CodiceConsultorio
            rowAssVac("NumeroAssociazioniConsultorio") = b.NumeroAssociazioniConsultorio
            rowAssVac("DescrizioneAssociazione") = b.DescrizioneAssociazione
            tabella.Tables("StatAssTable").Rows.Add(rowAssVac)
        Next

        result.DstStatVacProgAss = tabella

        Return result

    End Function
    Private Function CalcolaListaVacciniAssociazioni(vacinazione As List(Of Entities.StatVaccinazioneProgrammateAssociate), associazioniVaccini As List(Of Entities.StatVacciniAssociati)) As List(Of Entities.StatVacciniAssociatiFinale)
        Dim listaFinale As New List(Of Entities.StatVacciniAssociatiFinale)
        For Each cnv As Entities.StatVaccinazioneProgrammateAssociate In vacinazione

            Dim listaParziale As New List(Of Entities.StatVacciniAssociati)
            Dim listaParzialeClone As New List(Of Entities.StatVacciniAssociati)

            ' Recupero le associazioni che copre il maggior numero di vaccinazioni
            For Each assvac As Entities.StatVacciniAssociati In associazioniVaccini.Where(Function(p) p.Obsoleta = "N").OrderByDescending(Function(p) p.CountVac)
                ' verifico se l'associazione copre i vaccini programmati totali o in parte
                Dim ok As Boolean = ListaVacInAssVac(cnv.ListaVacProgrammate, assvac.ListaVaccini, True)
                If ok Then
                    Dim appoggio As New Entities.StatVacciniAssociati
                    appoggio.CodiceAssociazione = assvac.CodiceAssociazione
                    appoggio.DefaultAssociazione = assvac.DefaultAssociazione
                    appoggio.DescrizioneAssociazione = assvac.DescrizioneAssociazione
                    appoggio.ListaVaccini = assvac.ListaVaccini
                    If Not listaParziale.IsNullOrEmpty Then
                        Dim okNewList As Boolean = True
                        For Each liste As Entities.StatVacciniAssociati In listaParziale.Clone
                            If Not ListaVacInAssVac(liste.ListaVaccini, appoggio.ListaVaccini, False) Then
                                okNewList = False
                            End If
                        Next
                        If okNewList Then
                            listaParziale.Add(appoggio)
                        End If

                    Else
                        listaParziale.Add(appoggio)
                    End If
                End If
            Next
            ' verifico se l'associazione trovata da ciclo precedente è corretta
            For Each liste As Entities.StatVacciniAssociati In listaParziale.OrderBy(Function(a) a.CodiceAssociazione).ThenByDescending(Function(a) a.CountVac)
                Dim list As StatVacciniAssociati = VerificaAssociazioniVaccini(liste, cnv.ListaAssProgrammate)
                listaParzialeClone.Add(list)
            Next
            listaParziale = listaParzialeClone
            ' aggiungo le associazioni al calcolo finale.
            For Each liste As Entities.StatVacciniAssociati In listaParziale.OrderBy(Function(a) a.CodiceAssociazione).ThenByDescending(Function(a) a.CountVac)
                If Not listaFinale.IsNullOrEmpty Then
                    Dim item As List(Of Entities.StatVacciniAssociatiFinale) = listaFinale.Where(Function(k) k.CodiceConsultorio = cnv.CodiceConsultorio And k.CodiceAssociazione = liste.CodiceAssociazione).ToList()
                    If Not item.IsNullOrEmpty Then
                        item.First.NumeroAssociazioniConsultorio += 1
                    Else
                        'insert
                        Dim list As New Entities.StatVacciniAssociatiFinale
                        list.CodiceAssociazione = liste.CodiceAssociazione
                        list.CodiceConsultorio = cnv.CodiceConsultorio
                        list.DescrizioneAssociazione = liste.DescrizioneAssociazione
                        list.NumeroAssociazioniConsultorio = 1
                        listaFinale.Add(list)
                    End If
                Else
                    Dim list As New Entities.StatVacciniAssociatiFinale
                    list.CodiceAssociazione = liste.CodiceAssociazione
                    list.CodiceConsultorio = cnv.CodiceConsultorio
                    list.DescrizioneAssociazione = liste.DescrizioneAssociazione
                    list.NumeroAssociazioniConsultorio = 1
                    listaFinale.Add(list)
                End If

            Next
        Next
        Return listaFinale
    End Function
    Private Function VerificaAssociazioniVaccini(listaVac As StatVacciniAssociati, lisAssoProgr As List(Of String)) As StatVacciniAssociati
        Dim ret As StatVacciniAssociati = listaVac
        Dim listAssociazioni As List(Of StatVacciniAssociatiControllo) = GenericProvider.AnaVaccinazioni.GetListAssociazioneByListVacc(listaVac.ListaVaccini, listaVac.CountVac)
        If Not listAssociazioni.IsNullOrEmpty() Then
            If listAssociazioni.Count > 1 Then
                If Not lisAssoProgr.Contains(listaVac.CodiceAssociazione) Then
                    ' CONTROLLO SE UNA DELLE ASSOCIAZIONI è CONTENUTA NELLE PROGRAMMATE
                    If lisAssoProgr.Any(Function(p) listAssociazioni.Any(Function(b) b.CodiceAssociazione = p)) Then
                        For Each item As String In lisAssoProgr
                            For Each item2 As StatVacciniAssociatiControllo In listAssociazioni
                                If item = item2.CodiceAssociazione Then
                                    ret.CodiceAssociazione = item2.CodiceAssociazione
                                    ret.DefaultAssociazione = item2.DefaultAssociazione
                                    ret.DescrizioneAssociazione = item2.DescrizioneAssociazione
                                    ret.ListaVaccini = listaVac.ListaVaccini
                                End If
                            Next
                        Next
                    Else
                        ' controllo se una della associazioni è un default
                        Dim listaAssoDefault As List(Of StatVacciniAssociatiControllo) = listAssociazioni.Where(Function(p) p.DefaultAssociazione = "S").ToList()
                        If Not listaAssoDefault.IsNullOrEmpty() Then
                            Dim lista As StatVacciniAssociatiControllo = listaAssoDefault.FirstOrDefault()
                            ret.CodiceAssociazione = lista.CodiceAssociazione
                            ret.DescrizioneAssociazione = lista.DescrizioneAssociazione
                            ret.DefaultAssociazione = lista.DefaultAssociazione
                            ret.ListaVaccini = listaVac.ListaVaccini
                        End If
                    End If
                    End If
                End If
        End If
        Return ret
    End Function

    Private Function ListaVacInAssVac(listaVaccini As List(Of String), listaAssVac As List(Of String), contenuti As Boolean) As Boolean
        Dim retVerifica As Boolean = False
        If contenuti Then
            retVerifica = listaAssVac.TrueForAll(Function(a) listaVaccini.Any(Function(b) a = b))
        Else
            retVerifica = listaAssVac.TrueForAll(Function(a) listaVaccini.All(Function(b) a <> b))
        End If



        Return retVerifica
    End Function
#End Region

#Region " Copertura Nomi Commerciali "

    Public Class CopertureNomiCommerciali

        Public CodiceNomeCommerciale As String
        Public CodiceVaccinazione As String
        Public IsVaccinazioneSelezionataPerEsecuzione As Boolean

        Public Sub New(codiceNomeCommerciale As String, codiceVaccinazione As String, isVaccinazioneSelezionataPerEsecuzione As Boolean)
            Me.CodiceNomeCommerciale = codiceNomeCommerciale
            Me.CodiceVaccinazione = codiceVaccinazione
            Me.IsVaccinazioneSelezionataPerEsecuzione = isVaccinazioneSelezionataPerEsecuzione
        End Sub

    End Class

    ''' <summary>
    ''' Retituisce true se, dato il nome commerciale specificato, tutte le vaccinazioni che dovrebbe coprire sono effettivamente 
    ''' associate a quel nome commerciale e sono selezionate per l'esecuzione.
    ''' </summary>
    ''' <param name="codiceNomeCommerciale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsCoperturaVaccinazioniCompleta(codiceNomeCommerciale As String, coperture As List(Of CopertureNomiCommerciali)) As Boolean

        If coperture Is Nothing OrElse coperture.Count = 0 Then Return False

        Dim codiciVaccinazioniDaCoprire As List(Of String) = Me.GenericProvider.NomiCommerciali.GetCodiciVaccinazioniByNomeCommerciale(codiceNomeCommerciale)

        For Each codiceVaccinazione As String In codiciVaccinazioniDaCoprire

            If Not coperture.Any(Function(p) p.CodiceNomeCommerciale = codiceNomeCommerciale And p.CodiceVaccinazione = codiceVaccinazione And p.IsVaccinazioneSelezionataPerEsecuzione) Then
                Return False
            End If

        Next

        Return True

    End Function

#End Region

#End Region

#Region " Private "

    ''' <summary>
    ''' Cerca la massima data di effettuazione presente nel datagrid con le vaccinazioni programmate.
    ''' Nella ricerca vengono considerate le vaccinazioni escluse scadute
    ''' </summary>
    ''' <param name="dtVaccinazioniProgrammate"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function FindMaxDataEffettuazione(dtVaccinazioniProgrammate As DataTable) As Date

        Dim dataMax As Date = Date.MinValue

        If Not dtVaccinazioniProgrammate Is Nothing Then

            For Each row As DataRow In dtVaccinazioniProgrammate.Rows

                If (Not row.RowState = DataRowState.Deleted) AndAlso
                   (row("E") Is DBNull.Value OrElse row("E").ToString() = String.Empty OrElse
                    (Not row("VEX_DATA_SCADENZA") Is DBNull.Value AndAlso row("VEX_DATA_SCADENZA") <= DateTime.Today)) Then

                    If row("ves_data_effettuazione") > dataMax Then
                        dataMax = row("ves_data_effettuazione")
                    End If

                End If

            Next

        End If

        Return dataMax

    End Function

    Private Function GetStatoAbilitazione(flagCondizione As Object, flagDefault As Object) As Enumerators.StatoAbilitazioneCampo
        '--
        Dim statoAbilitazione As Enumerators.StatoAbilitazioneCampo = Enumerators.StatoAbilitazioneCampo.Disabilitato
        '--
        If Not flagCondizione Is Nothing AndAlso Not flagCondizione Is DBNull.Value Then
            '--
            statoAbilitazione = DirectCast(Convert.ToInt32(flagCondizione), Enumerators.StatoAbilitazioneCampo)
            '--
        ElseIf Not flagDefault Is Nothing AndAlso Not flagDefault Is DBNull.Value Then
            '--
            statoAbilitazione = DirectCast(Convert.ToInt32(flagDefault), Enumerators.StatoAbilitazioneCampo)
            '--
        End If
        '--
        Return statoAbilitazione
        '--
    End Function

#End Region

#Region " Types "

#Region " Commands "

    Public Class EliminaVaccinazioniProgrammateCommand
        Public Property CodicePaziente As Int64
        Public Property CodiceVaccinazioni As IEnumerable(Of String)
        Public Property DataConvocazione As DateTime?
    End Class

    Public Class EliminaVaccinazioniProgrammateResult

        Public Sub New()
            Me.VaccinazioniProgrammateEliminate = New List(Of VaccinazioneProgrammata)()
        End Sub

        Public Property VaccinazioniProgrammateEliminate As List(Of VaccinazioneProgrammata)

    End Class

    Public Class GetInfoSomministrazioneCommand
        Public CodiceCiclo As String
        Public NumeroSeduta As Integer?
        Public CodiceVaccinazione As String
        Public CodiceAssociazione As String
        Public CodiceNomeCommerciale As String
    End Class

#End Region

#End Region

End Class
