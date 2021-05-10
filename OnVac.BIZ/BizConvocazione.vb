Imports System.Collections.Generic

Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Collection
Imports Onit.OnAssistnet.OnVac.Log


Public Class BizConvocazione
    Inherits BizClass

#Region " Constructors "

    Public Sub New(dbGenericProviderFactory As DbGenericProviderFactory, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)

        MyBase.New(dbGenericProviderFactory, settings, Nothing, contextInfos, logOptions)

    End Sub

    Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos, bizLogOptions As Biz.BizLogOptions)

        MyBase.New(genericprovider, settings, contextInfos, bizLogOptions)

    End Sub

#End Region

#Region " Calendario "

    Public Function GetCalendarioVaccinaleDS(codicePaziente As Integer) As DSCalendarioVaccinale

        Return Me.GenericProvider.Convocazione.GetCalendarioVaccinaleDS(codicePaziente)

    End Function

    Public Function GetCalendarioVaccinale(codicePaziente As Integer) As List(Of Entities.SedutaCalendario)

        Dim ds As DSCalendarioVaccinale = Me.GenericProvider.Convocazione.GetCalendarioVaccinaleDS(codicePaziente)

        Return Me.GetCalendarioVaccinale(ds)

    End Function

    Public Function GetCalendarioVaccinale(ds As DSCalendarioVaccinale) As List(Of Entities.SedutaCalendario)

        Dim listaSeduteMerge As New List(Of Entities.SedutaCalendario)()
        Dim listaSedute As New List(Of Entities.SedutaCalendario)()

        ' Raggruppo per cicli e sedute
        Dim cicTemp As String = String.Empty
        Dim sedTemp As Int16 = 0
        Dim contatoreSeduta As Integer = 1

        For i As Integer = 0 To ds.dtCicSedVac.Rows.Count - 1

            If (cicTemp <> ds.dtCicSedVac.Rows(i)("cic_codice") OrElse sedTemp <> ds.dtCicSedVac.Rows(i)("tsd_n_seduta")) Then

                cicTemp = ds.dtCicSedVac.Rows(i)("cic_codice").ToString()

                If ds.dtCicSedVac.Rows(i)("tsd_n_seduta") Is DBNull.Value Then
                    sedTemp = 0
                Else
                    sedTemp = ds.dtCicSedVac.Rows(i)("tsd_n_seduta")
                End If

                Dim seduta As New Entities.SedutaCalendario()

                seduta.Vaccinazioni = New List(Of Entities.SedutaCalendario.Vaccinazione)()

                Dim v As New Entities.SedutaCalendario.Vaccinazione()

                v.Codice = ds.dtCicSedVac.Rows(i)("sed_vac_codice").ToString()
                v.Descrizione = ds.dtCicSedVac.Rows(i)("vac_descrizione").ToString()
                v.Dose = Me.GetIntegerFromDataRowItem(ds.dtCicSedVac.Rows(i)("sed_n_richiamo"))
                v.CicloCodice = ds.dtCicSedVac.Rows(i)("cic_codice").ToString()
                v.CicloDescrizione = ds.dtCicSedVac.Rows(i)("cic_descrizione").ToString()
                v.CicloSeduta = Me.GetIntegerFromDataRowItem(ds.dtCicSedVac.Rows(i)("tsd_n_seduta"))

                seduta.Vaccinazioni.Add(v)

                seduta.ID = String.Format("{0}{1}", ds.dtCicSedVac.Rows(i)("cic_codice").ToString(), contatoreSeduta.ToString())
                seduta.Seduta = contatoreSeduta
                seduta.Eta = Me.GetIntegerFromDataRowItem(ds.dtCicSedVac.Rows(i)("tsd_eta_seduta"))
                seduta.Intervallo = Me.GetIntegerFromDataRowItem(ds.dtCicSedVac.Rows(i)("tsd_intervallo"))
                seduta.IntervalloProssima = Me.GetIntegerFromDataRowItem(ds.dtCicSedVac.Rows(i)("tsd_intervallo_prossima"))
                seduta.Durata = Me.GetIntegerFromDataRowItem(ds.dtCicSedVac.Rows(i)("tsd_durata_seduta"))

                contatoreSeduta = contatoreSeduta + 1

                listaSedute.Add(seduta)

            Else

                Dim v As New Entities.SedutaCalendario.Vaccinazione()

                v.Codice = ds.dtCicSedVac.Rows(i)("sed_vac_codice").ToString()
                v.Descrizione = ds.dtCicSedVac.Rows(i)("vac_descrizione").ToString()
                v.Dose = Me.GetIntegerFromDataRowItem(ds.dtCicSedVac.Rows(i)("sed_n_richiamo"))
                v.CicloCodice = ds.dtCicSedVac.Rows(i)("cic_codice").ToString()
                v.CicloDescrizione = ds.dtCicSedVac.Rows(i)("cic_descrizione").ToString()
                v.CicloSeduta = Me.GetIntegerFromDataRowItem(ds.dtCicSedVac.Rows(i)("tsd_n_seduta"))

                listaSedute.Last().Vaccinazioni.Add(v)

            End If

        Next

        Dim find As Boolean = False
        Dim idtemp As String = ""

        For i As Integer = 0 To listaSedute.Count - 1

            ' SIO 20/01/2004
            ' Deve controllare anche se cambia la seduta?
            If (listaSedute(i).ID <> idtemp) Then

                idtemp = listaSedute(i).ID

                For j As Integer = 0 To listaSeduteMerge.Count - 1

                    Dim seduta As Entities.SedutaCalendario = listaSeduteMerge(j)

                    If listaSedute(i).Eta = seduta.Eta Then

                        If seduta.Durata < listaSedute(i).Durata Then
                            seduta.Durata = listaSedute(i).Durata
                        End If

                        If seduta.Intervallo > listaSedute(i).Intervallo Then
                            seduta.Intervallo = listaSedute(i).Intervallo
                        End If

                        For k As Integer = 0 To listaSedute(i).Vaccinazioni.Count - 1
                            seduta.Vaccinazioni.Add(listaSedute(i).Vaccinazioni(k))
                        Next

                        find = True
                        Exit For

                    ElseIf listaSedute(i).Eta < seduta.Eta Then

                        If listaSedute(i).Eta + listaSedute(i).Intervallo >= seduta.Eta Then

                            If seduta.Intervallo > listaSedute(i).Eta + listaSedute(i).Intervallo - seduta.Eta Then
                                seduta.Intervallo = listaSedute(i).Eta + listaSedute(i).Intervallo - seduta.Eta
                            End If

                            If seduta.Durata < listaSedute(i).Durata Then
                                seduta.Durata = listaSedute(i).Durata
                            End If

                            For k As Integer = 0 To listaSedute(i).Vaccinazioni.Count - 1
                                seduta.Vaccinazioni.Add(listaSedute(i).Vaccinazioni(k))
                            Next

                            find = True
                            Exit For

                        End If

                    ElseIf listaSedute(i).Eta > seduta.Eta Then

                        If seduta.Eta + seduta.Intervallo >= listaSedute(i).Eta Then

                            seduta.Eta = listaSedute(i).Eta

                            If seduta.Intervallo > listaSedute(i).Eta + listaSedute(i).Intervallo - seduta.Eta Then
                                seduta.Intervallo = listaSedute(i).Intervallo
                            Else
                                seduta.Intervallo = seduta.Intervallo - listaSedute(i).Eta + seduta.Eta
                            End If

                            If (seduta.Durata < listaSedute(i).Durata) Then
                                seduta.Durata = listaSedute(i).Durata
                            End If

                            For k As Integer = 0 To listaSedute(i).Vaccinazioni.Count - 1
                                seduta.Vaccinazioni.Add(listaSedute(i).Vaccinazioni(k))
                            Next

                            find = True
                            Exit For

                        End If

                    End If

                Next

                If Not find Then

                    listaSeduteMerge.Add(listaSedute(i))

                End If

                find = False

            Else

                listaSeduteMerge.Add(listaSedute(i))

            End If
        Next

        Return listaSeduteMerge

    End Function

    Private Function GetIntegerFromDataRowItem(item As Object) As Integer

        If item Is Nothing OrElse item Is DBNull.Value Then
            Return 0
        End If

        Return item

    End Function

#End Region

#Region " Metodi di Select "

    Public Function CercaConvocazione(codicePaziente As Integer, dataCnvMin As Date, dataCnvMax As Date) As Date

        Dim dtCnvDate As DataTable = Me.GenericProvider.Convocazione.GetCnvFromInterval(codicePaziente, dataCnvMin, dataCnvMax)

        If Not dtCnvDate Is Nothing AndAlso dtCnvDate.Rows.Count > 0 Then

            Return dtCnvDate.Rows(0)("cnv_data")

        End If

        Return Date.MinValue

    End Function

    ''' <summary>
    ''' Restituisce un datatable con i dati delle convocazioni del paziente, con e senza appuntamento.
    ''' Se il parametro CONVOCAZIONI_ALTRI_CONSULTORI è impostato al valore "NonVisibili", vengono restituite
    ''' solo le convocazioni sul consultorio corrente, altrimenti non filtra per consultorio.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="codiceConsultorioCorrente"></param>
    ''' <param name="livelloUtenteConvocazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDtConvocazioniAppuntamentiPaziente(codicePaziente As Integer, codiceConsultorioCorrente As String, livelloUtenteConvocazione As Enumerators.LivelloUtenteConvocazione) As DataTable

        Dim soloConsultorioCorrente As Boolean = IsUtenteCnvDefault(livelloUtenteConvocazione)

        Return Me.GenericProvider.Prenotazioni.CercaConvocazioniAppuntamentiPaziente(codicePaziente, codiceConsultorioCorrente, soloConsultorioCorrente)

    End Function

    ''' <summary>
    ''' Restituisce la convocazione del paziente nella data specificata
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataConvocazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetConvocazione(codicePaziente As Integer, dataConvocazione As Date) As Entities.Convocazione

        Dim convocazione As New Entities.Convocazione(Date.MinValue, codicePaziente, String.Empty, 0)

        Dim dtCon As DataTable = Me.GenericProvider.Convocazione.GetFromKey(codicePaziente, dataConvocazione)

        If ((Not dtCon Is Nothing) AndAlso (dtCon.Rows.Count > 0)) Then

            convocazione.Data_CNV = dtCon.Rows(0).Item("cnv_data")
            convocazione.Cns_Codice = dtCon.Rows(0).Item("cnv_cns_codice")
            convocazione.Durata_Appuntamento = dtCon.Rows(0).Item("cnv_durata_appuntamento")

        End If

        Return convocazione

    End Function

    ''' <summary>
    ''' Restituisce una lista con le date di convocazione precedenti a quella massima specificata.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="maxDataConvocazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDateConvocazioniPaziente(codicePaziente As Integer, maxDataConvocazione As Date) As IEnumerable(Of DateTime)

        If maxDataConvocazione = DateTime.MinValue Then Return Nothing

        Return Me.GenericProvider.Convocazione.GetDateConvocazioniPaziente(codicePaziente, maxDataConvocazione)

    End Function

    Public Function GetMaxSollecitoVaccinazioni(codicePaziente As Integer, dataConvocazione As DateTime) As Integer

        Return Me.GenericProvider.Convocazione.GetMaxSollecitoVaccinazioni(codicePaziente, dataConvocazione)

    End Function

    ''' <summary>
    ''' Restituisce True se il paziente ha dei solleciti
    ''' False altrimenti.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataConvocazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsRitardatario(codicePaziente As Integer, dataConvocazione As DateTime) As Boolean

        Return Me.GenericProvider.Convocazione.GetMaxSollecitoVaccinazioni(codicePaziente, dataConvocazione) > 0

    End Function

    ''' <summary>
    ''' Restituisce True se il paziente ha dei solleciti
    ''' False altrimenti.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function TerminePerentorio(codicePaziente As Integer) As Boolean

        Return Me.GenericProvider.Convocazione.TerminePerentorio(codicePaziente, Nothing, Me.Settings.NUMSOL)

    End Function

    ''' <summary>
    ''' Restituisce True se il paziente ha dei solleciti
    ''' False altrimenti.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataConvocazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function TerminePerentorio(codicePaziente As Integer, dataConvocazione As Nullable(Of DateTime)) As Boolean

        Return Me.GenericProvider.Convocazione.TerminePerentorio(codicePaziente, dataConvocazione, Me.Settings.NUMSOL)

    End Function

    ''' <summary>
    ''' Restituisce True se trova una convocazione per il paziente nella data specificata
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataConvocazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Exists(codicePaziente As Integer, dataConvocazione As DateTime) As Boolean

        Return Me.GenericProvider.Convocazione.Exists(codicePaziente, dataConvocazione)

    End Function

    ''' <summary>
    ''' Restituisce false se, per il paziente, non esistono convocazioni precedenti 
    ''' la data di fine sospensione specificata, oppure se non è stata impostata nessuna data.
    ''' Se ci sono convocazioni con data inferiore alla data di fine sospensione, restituisce true.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataFineSospensione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function EsistonoCnvPrecedentiFineSospensione(codicePaziente As Integer, dataFineSospensione As DateTime) As Boolean

        If dataFineSospensione = Date.MinValue Then Return False

        Dim countConvocazioniSospensione As Integer = Me.CountConvocazioniPrecedentiFineSospensione(codicePaziente, dataFineSospensione)

        Return (countConvocazioniSospensione > 0)

    End Function

    ''' <summary>
    ''' Restituisce il numero di convocazioni del paziente con data inferiore alla data di fine sospensione specificata.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataFineSospensione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountConvocazioniPrecedentiFineSospensione(codicePaziente As Integer, dataFineSospensione As DateTime) As Integer

        Return Me.GenericProvider.Convocazione.CountConvocazioniPrecedentiFineSospensione(codicePaziente, dataFineSospensione)

    End Function

    ''' <summary>
    ''' Restituisce il numero di convocazioni con appuntamento per il paziente specificato.
    ''' Se il parametro CONVOCAZIONI_ALTRI_CONSULTORI è impostato al valore "NonVisibili", il calcolo considera 
    ''' solo le convocazioni sul consultorio corrente, altrimenti non filtra per consultorio.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="codiceConsultorioCorrente"></param>
    ''' <param name="livelloUtenteConvocazione"></param>
    ''' <param name="isGestioneCentrale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountConvocazioniConAppuntamento(codicePaziente As String, codiceConsultorioCorrente As String, livelloUtenteConvocazione As Enumerators.LivelloUtenteConvocazione, isGestioneCentrale As Boolean) As Integer

        If isGestioneCentrale Then Return 0

        Dim soloConsultorioCorrente As Boolean = Me.IsUtenteCnvDefault(livelloUtenteConvocazione)

        Return Me.GenericProvider.Convocazione.CountConvocazioniConAppuntamento(codicePaziente, codiceConsultorioCorrente, soloConsultorioCorrente)

    End Function

    ''' <summary>
    ''' Restituisce il numero di convocazioni senza appuntamento per il paziente specificato.
    ''' Se il parametro CONVOCAZIONI_ALTRI_CONSULTORI è impostato al valore "NonVisibili", il calcolo considera 
    ''' solo le convocazioni sul consultorio corrente, altrimenti non filtra per consultorio
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="codiceConsultorioCorrente"></param>
    ''' <param name="livelloUtenteConvocazione"></param>
    ''' <param name="isGestioneCentrale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountConvocazioniSenzaAppuntamento(codicePaziente As String, codiceConsultorioCorrente As String, livelloUtenteConvocazione As Enumerators.LivelloUtenteConvocazione, isGestioneCentrale As Boolean) As Integer

        If isGestioneCentrale Then Return 0

        Dim soloConsultorioCorrente As Boolean = Me.IsUtenteCnvDefault(livelloUtenteConvocazione)

        Return Me.GenericProvider.Convocazione.CountConvocazioniSenzaAppuntamento(codicePaziente, codiceConsultorioCorrente, soloConsultorioCorrente)

    End Function

    ''' <summary>
    ''' Restituisce true se il livello dell'utente è Default.
    ''' Restituisce true anche nel caso in cui il livello è impostato a Undefined e il parametro CONVOCAZIONI_ALTRI_CONSULTORI vale "NonVisibili".
    ''' In tutti gli altri casi restituisce false.
    ''' </summary>
    ''' <param name="livelloUtenteConvocazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function IsUtenteCnvDefault(livelloUtenteConvocazione As Enumerators.LivelloUtenteConvocazione) As Boolean

        If livelloUtenteConvocazione = Enumerators.LivelloUtenteConvocazione.Undefined Then

            Return (Me.Settings.CONVOCAZIONI_ALTRI_CONSULTORI = Enumerators.TipoGestioneConvocazioniAltriConsultori.NonVisibili)

        End If

        Return (livelloUtenteConvocazione = Enumerators.LivelloUtenteConvocazione.Default)

    End Function

#End Region

#Region "Metodi di Update/Insert"

    Public Sub CreaConvocazioni(bilanciDaProgrammare As BilancioProgrammatoCollection, codicePaziente As Integer, dataInserimento As DateTime, idUtenteInserimento As Long)

        Dim codiceConsultorioPaziente As String

        Using bizPaziente As New Biz.BizPaziente(Me.GenericProvider, Me.Settings, Me.ContextInfos, Me.LogOptions)

            codiceConsultorioPaziente = bizPaziente.GetCodiceConsultorio(codicePaziente)

        End Using

        Dim nuovaConvocazione As Entities.Convocazione = Nothing

        For i As Integer = 0 To bilanciDaProgrammare.Count - 1

            Dim bilancio As Entities.BilancioProgrammato = bilanciDaProgrammare(i)

            If bilancio.New_Cnv And codicePaziente > 0 Then

                ' quanto dura l'appuntamento in caso di solo Bilancio???

                nuovaConvocazione = New Entities.Convocazione(bilancio.Data_CNV, codicePaziente, codiceConsultorioPaziente, 0)
                nuovaConvocazione.DataInserimento = dataInserimento
                nuovaConvocazione.IdUtenteInserimento = idUtenteInserimento

                Me.CreaConvocazione(nuovaConvocazione)

            End If

        Next

    End Sub

    Public Sub CreaConvocazioni(dtNuoveCnv As DataTable, codicePaziente As Integer, dataInserimento As DateTime, idUtenteInserimento As Long)

        ' Consultorio del paziente
        Dim codiceConsultorioPaziente As String

        Using bizPaziente As New Biz.BizPaziente(Me.GenericProvider, Me.Settings, Me.ContextInfos, Me.LogOptions)

            codiceConsultorioPaziente = bizPaziente.GetCodiceConsultorio(codicePaziente)

        End Using

        If Not dtNuoveCnv Is Nothing AndAlso dtNuoveCnv.Rows.Count > 0 Then

            Dim nuovaConvocazione As Entities.Convocazione = Nothing
            Dim convocazioneEsistente As Entities.Convocazione = Nothing

            For i As Integer = 0 To dtNuoveCnv.Rows.Count - 1

                ' verificare l'esistenza della convocazione
                ' se esiste non la replico ma l'aggiorno (in particolare la durata) => in realtà non l'aggiorna...
                convocazioneEsistente = Me.GetConvocazione(codicePaziente, dtNuoveCnv.DefaultView(i)("CONVOCAZIONE"))

                If convocazioneEsistente.Data_CNV = Date.MinValue Then

                    nuovaConvocazione = New Entities.Convocazione(dtNuoveCnv.DefaultView(i)("CONVOCAZIONE"), codicePaziente, codiceConsultorioPaziente, dtNuoveCnv.DefaultView(i)("TSD_DURATA_SEDUTA"))
                    nuovaConvocazione.DataInserimento = dataInserimento
                    nuovaConvocazione.IdUtenteInserimento = idUtenteInserimento

                    Me.CreaConvocazione(nuovaConvocazione)

                Else

                    ' DOVREI FARE L'UPDATE DELLA CONVOCAZIONE NEL CASO DI MODIFICA DEI DATI??? <----------------------
                    'Dim nuovaconvocazione As New Convocazione(dtNuoveCnv.DefaultView(i)("CONVOCAZIONE"), pazCodice, cns_paziente, dtNuoveCnv.DefaultView(i)("TSD_DURATA_SEDUTA"))
                    ' inserire 

                End If

            Next

        End If

    End Sub

    Private Sub CreaConvocazione(convocazione As Entities.Convocazione)

        If Not convocazione Is Nothing Then

            Me.GenericProvider.Convocazione.InsertConvocazione(convocazione)

        End If

    End Sub

#Region " Update convocazione e storico appuntamenti "

    Public Function UpdateConvocazione_DatiAppuntamento(convocazioneAppuntamento As Entities.ConvocazioneAppuntamento) As Integer

        Return UpdateConvocazione_DatiAppuntamento(convocazioneAppuntamento, convocazioneAppuntamento.IdMotivoEliminazioneAppuntamento)

    End Function

    Public Function UpdateConvocazione_DatiAppuntamento(convocazioneAppuntamento As Entities.ConvocazioneAppuntamento, idMotivoSpostamento As String) As Integer

        Dim count As Integer = 0

        ' N.B. : la data di appuntamento va letta prima dell'update!

        ' Se sto prenotando, carico la data di appuntamento pre-modifica
        Dim dataAppuntamentoOriginale As DateTime? = Nothing

        If convocazioneAppuntamento.DataAppuntamento.HasValue Then

            Dim convocazioneOriginale As Entities.Convocazione =
                Me.GenericProvider.Convocazione.GetConvocazionePaziente(convocazioneAppuntamento.CodicePaziente, convocazioneAppuntamento.DataConvocazione)

            If Not convocazioneOriginale Is Nothing AndAlso convocazioneOriginale.DataAppuntamento > DateTime.MinValue Then

                dataAppuntamentoOriginale = convocazioneOriginale.DataAppuntamento

            End If

        End If

        ' Update convocazione
        count = Me.GenericProvider.Prenotazioni.UpdateConvocazione_DatiAppuntamento(convocazioneAppuntamento)

        If count > 0 Then

            If Not convocazioneAppuntamento.DataAppuntamento.HasValue Then
                '---
                ' Eliminazione Appuntamento
                '---
                If Not convocazioneAppuntamento.DataEliminazioneAppuntamento.HasValue Then convocazioneAppuntamento.DataEliminazioneAppuntamento = DateTime.Now
                If String.IsNullOrWhiteSpace(convocazioneAppuntamento.IdMotivoEliminazioneAppuntamento) Then convocazioneAppuntamento.IdMotivoEliminazioneAppuntamento = Constants.MotiviEliminazioneAppuntamento.EliminazioneAppuntamento
                If String.IsNullOrWhiteSpace(convocazioneAppuntamento.Note) Then convocazioneAppuntamento.Note = "Eliminazione appuntamento"

                ' Appuntamento eliminato => update in storico appuntamenti (più recente)
                UpdateStoricoAppuntamenti_Eliminazione(
                    convocazioneAppuntamento.CodicePaziente, {convocazioneAppuntamento.DataConvocazione}.ToList(), convocazioneAppuntamento.DataEliminazioneAppuntamento.Value,
                    convocazioneAppuntamento.IdMotivoEliminazioneAppuntamento, convocazioneAppuntamento.Note, convocazioneAppuntamento.NoteModificaAppuntamento)

            Else
                '---
                ' Update Appuntamento
                '---
                Dim noteInserimentoAppuntamentoSpostato As String = String.Empty

                ' Se era già presente una data di appuntamento, nello storico deve essere valorizzata la data di eliminazione e inserita la nuova data appuntamento.
                ' Se non c'era nessuna data di appuntamento, nello storico deve essere inserita la nuova data appuntamento.
                If dataAppuntamentoOriginale.HasValue Then

                    ' N.B. : la data di eliminazione deve essere la stessa della registrazione del nuovo appuntamento perchè siamo nel caso di spostamento: 
                    '        nella stessa data deve esserci un'eliminazione del vecchio appuntamento e una prenotazione del nuovo.
                    Dim dataEliminazione As DateTime = convocazioneAppuntamento.DataRegistrazioneAppuntamento

                    If String.IsNullOrWhiteSpace(idMotivoSpostamento) Then idMotivoSpostamento = Constants.MotiviEliminazioneAppuntamento.SpostamentoAppuntamento

                    ' Appuntamento eliminato => update in storico appuntamenti (più recente)
                    UpdateStoricoAppuntamenti_Eliminazione(
                        convocazioneAppuntamento.CodicePaziente, {convocazioneAppuntamento.DataConvocazione}.ToList(), dataEliminazione,
                        idMotivoSpostamento, "Eliminazione per spostamento appuntamento", convocazioneAppuntamento.NoteModificaAppuntamento)

                    ' Note per il record che verrà inserito
                    noteInserimentoAppuntamentoSpostato =
                        String.Format(" (Spostamento appuntamento del {0:dd/MM/yyyy HH:mm})", dataAppuntamentoOriginale.Value)

                End If

                ' Valorizzazione data primo appuntamento (in convocazione) se non presente
                Dim dataPrimoAppuntamento As DateTime? =
                    UpdateDataPrimoAppuntamentoIfNotExists(convocazioneAppuntamento.CodicePaziente, convocazioneAppuntamento.DataConvocazione, convocazioneAppuntamento.DataAppuntamento)

                ' Valorizzazione data primo appuntamento (in storico), uguale a quella della convocazione
                convocazioneAppuntamento.DataPrimoAppuntamento = dataPrimoAppuntamento

                If Not String.IsNullOrWhiteSpace(noteInserimentoAppuntamentoSpostato) Then convocazioneAppuntamento.Note += noteInserimentoAppuntamentoSpostato

                ' Appuntamento prenotato => inserimento in storico appuntamenti
                InsertStoricoAppuntamenti(convocazioneAppuntamento)

            End If

        End If

        Return count

    End Function

    ''' <summary>
    ''' Effettua l'update della data del primo appuntamento per la convocazione specificata.
    ''' Se la cnv ha già una data di primo appuntamento valorizzata, non la sovrascrive.
    ''' Restituisce la data primo appuntamento della convocazione (null se non ce l'ha).
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataConvocazione"></param>
    ''' <param name="dataPrimoAppuntamento"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdateDataPrimoAppuntamentoIfNotExists(codicePaziente As Long, dataConvocazione As DateTime, dataPrimoAppuntamento As DateTime?) As DateTime?

        ' Data primo appuntamento non specificata => nessun aggiornamento e restituisco null
        If Not dataPrimoAppuntamento.HasValue Then Return Nothing

        ' Data primo appuntamento già valorizzata => nessun aggiornamento e restituisco la data stessa
        Dim dataEsistente As DateTime? = Me.GenericProvider.Prenotazioni.GetDataPrimoAppuntamento(codicePaziente, dataConvocazione)
        If dataEsistente.HasValue Then Return dataEsistente

        ' Data primo appuntamento già valorizzata => aggiornamento e, se ok, restituisco la data impostata
        If Me.GenericProvider.Prenotazioni.UpdateDataPrimoAppuntamento(codicePaziente, dataConvocazione, dataPrimoAppuntamento.Value) Then
            Return dataPrimoAppuntamento.Value
        End If

        ' Se l'aggiornamento della prima data non è andato a buon fine => restituisco null
        Return Nothing

    End Function

#End Region

#Region " Spostamento convocazioni "

    Public Class SpostaConvocazioneCommand
        Public CodicePaziente As Long
        Public DataConvocazioneNew As DateTime
        Public DurataAppuntamentoNew As Integer?
        Public DateConvocazioneOld() As DateTime
        Public ControlloAssociabilita As Boolean
        Public MantieniAppuntamento As Boolean
        Public OperazioneAutomatica As Boolean
        Public NoteSpostamentoAppuntamento As String
    End Class

    ''' <summary>
    ''' Spostamento ad una nuova data: nel caso ci sia già una convocazione esistente, la unisce.
    ''' Se il flag controlloAssociabilita è true controlla anche che le vaccinazioni delle due convocazioni siano somministrabili nella stessa data (solo se è true anche il parametro CTRL_ASSOCIABILITA_VAC).
    ''' Se il flag mantieniAppuntamento è true copia i dati dell'appuntamento più recente tra le due convocazioni. Altrimenti i dati relativi all'appuntamento vengono eliminati.
    ''' Se la nuova convocazione in realtà non esiste, la crea eliminando eventuali dati dell'appuntamento.
    ''' Il flag operazioneAutomatica serve per la gestione del log.
    ''' </summary>
    ''' <param name="command"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SpostaConvocazione(command As SpostaConvocazioneCommand) As SpostaConvocazioneBizResult

        Dim success As Boolean = False

        ' Se una data è nulla non deve unire niente
        If command.DataConvocazioneNew = Date.MinValue OrElse command.DateConvocazioneOld.Contains(Date.MinValue) Then Return New SpostaConvocazioneBizResult()

        ' Note spostamento per storico appuntamenti
        If String.IsNullOrWhiteSpace(command.NoteSpostamentoAppuntamento) Then
            command.NoteSpostamentoAppuntamento = "BizConvocazione.SpostaAppuntamento"
        End If

        Dim datiConvocazioneScartataList As New List(Of DatiConvocazioneScartata)()

        ' --- Vaccinazioni nella vecchia data --- '
        For Each oldDataConvocazione As DateTime In command.DateConvocazioneOld

            Dim codiciVaccinazioniVecchiaData As ArrayList = Me.GenericProvider.VaccinazioneProg.GetCodiceVacProgrammatePazienteByData(command.CodicePaziente, oldDataConvocazione)

            ' Se la vecchia convocazione non ha vaccinazioni programmate non deve fare niente
            If codiciVaccinazioniVecchiaData.Count = 0 Then
                datiConvocazioneScartataList.Add(BizConvocazione.CreateConvocazioneScartata(command.CodicePaziente, oldDataConvocazione, command.DataConvocazioneNew, CodiceMotivoConvocazioneScartata.VaccinazioniProgrammateAssenti))
            End If

        Next

        ' --- Controllo vaccinazioni in comune --- '
        ' Se nelle due convocazioni che si devono unire ci sono vaccinazioni uguali, non le unisce.
        For i As Int16 = 0 To command.DateConvocazioneOld.Length - 1

            Dim dataConvocazioneOld As DateTime = command.DateConvocazioneOld(i)

            If Me.GenericProvider.Convocazione.ExistsStessaVaccinazioneInConvocazioni(command.CodicePaziente, dataConvocazioneOld, command.DataConvocazioneNew) Then

                datiConvocazioneScartataList.Add(BizConvocazione.CreateConvocazioneScartata(command.CodicePaziente, dataConvocazioneOld, command.DataConvocazioneNew, CodiceMotivoConvocazioneScartata.StesseVaccinazioniInDataUnione))

            End If

            For i2 As Int16 = i To command.DateConvocazioneOld.Length - 1

                Dim oldDataConvocazione2 As DateTime = command.DateConvocazioneOld(i2)

                ' N.B. : effettua il controllo anche sulla stessa data, ma non dà problemi per come è fatta la query
                If Me.GenericProvider.Convocazione.ExistsStessaVaccinazioneInConvocazioni(command.CodicePaziente, oldDataConvocazione2, dataConvocazioneOld) Then

                    datiConvocazioneScartataList.Add(BizConvocazione.CreateConvocazioneScartata(command.CodicePaziente, oldDataConvocazione2, dataConvocazioneOld, CodiceMotivoConvocazioneScartata.StesseVaccinazioniInDataUnione))

                End If

            Next

        Next

        ' --- Controllo associabilità vaccinazioni --- '
        ' Se nelle due convocazioni che si devono unire ci sono vaccinazioni incompatibili, non le unisce.
        ' Questo controllo viene effettuato solo se sono verificate entrambe le condizioni seguenti:
        '       - il parametro su db è configurato per abilitare il controllo di non associabilità;
        '       - il flag controllo_associabilita è impostato a true.
        '
        ' Se le vac non sono somministrabili lo stesso giorno, non unisce le cnv
        If Me.Settings.CTRL_ASSOCIABILITA_VAC AndAlso command.ControlloAssociabilita Then
            '--
            ' Continuo con il controllo solo se ci sono vaccinazioni nella nuova data
            '--
            Dim codiciVaccinazioniNuovaData As ArrayList = Me.GenericProvider.VaccinazioneProg.GetCodiceVacProgrammatePazienteByData(command.CodicePaziente, command.DataConvocazioneNew)
            '--
            Dim oldDataCodiceConvocazioniDictionary As New Dictionary(Of DateTime, ArrayList)
            '--
            For Each oldDataConvocazione As DateTime In command.DateConvocazioneOld
                oldDataCodiceConvocazioniDictionary.Add(oldDataConvocazione, Me.GenericProvider.VaccinazioneProg.GetCodiceVacProgrammatePazienteByData(command.CodicePaziente, oldDataConvocazione))
            Next
            '--
            If codiciVaccinazioniNuovaData.Count > 0 AndAlso
                oldDataCodiceConvocazioniDictionary.Select(Function(odcckvp) odcckvp.Value).Any(Function(cc) cc.Count > 0) Then
                '--
                ' Creazione oggetto utilizzato per effettuare i controlli di associabilità
                '--
                Dim ctrlAssociabilita As New OnVac.Associabilita.ControlloAssociabilita(Me.GenericProvider.Provider, Me.GenericProvider.Connection, Me.GenericProvider.Transaction)
                '--
                For i As Int16 = 0 To command.DateConvocazioneOld.Length - 1
                    '--
                    Dim oldDataConvocazione As DateTime = command.DateConvocazioneOld(i)
                    '--
                    Dim codiciVaccinazioniVecchiaData As ArrayList = oldDataCodiceConvocazioniDictionary(oldDataConvocazione)
                    '--
                    If codiciVaccinazioniVecchiaData.Count > 0 Then
                        '--
                        If codiciVaccinazioniNuovaData.Count > 0 Then
                            '--
                            If Not ctrlAssociabilita.VaccinazioniAssociabili(codiciVaccinazioniVecchiaData, codiciVaccinazioniNuovaData) Then
                                '--
                                datiConvocazioneScartataList.Add(BizConvocazione.CreateConvocazioneScartata(command.CodicePaziente, oldDataConvocazione, command.DataConvocazioneNew, CodiceMotivoConvocazioneScartata.NonAssociabilitaInDataUnione))
                                '--
                            End If
                            '--
                        End If
                        '--
                        For i2 As Int16 = i To command.DateConvocazioneOld.Length - 1
                            '--
                            Dim oldDataConvocazione2 As DateTime = command.DateConvocazioneOld(i2)
                            '--
                            Dim codiciVaccinazioniVecchiaData2 As ArrayList = oldDataCodiceConvocazioniDictionary(oldDataConvocazione2)
                            '--
                            If codiciVaccinazioniNuovaData.Count > 0 Then
                                '--
                                If Not ctrlAssociabilita.VaccinazioniAssociabili(codiciVaccinazioniVecchiaData2, codiciVaccinazioniVecchiaData) Then
                                    '--
                                    datiConvocazioneScartataList.Add(BizConvocazione.CreateConvocazioneScartata(command.CodicePaziente, oldDataConvocazione2, command.DataConvocazioneNew, CodiceMotivoConvocazioneScartata.NonAssociabilitaInDataUnione))
                                    '--
                                End If
                                '--
                            End If
                            '--
                        Next
                        '--
                    End If
                    '--
                Next
                '--
            End If
            '--
        End If

        If Not datiConvocazioneScartataList.Any(Function(dcs) dcs.CodiceMotivo <> CodiceMotivoConvocazioneScartata.VaccinazioniProgrammateAssenti) Then

            Dim dataOperazione As DateTime = DateTime.Now

            ' Tipo di operazione che verrà registrata nel Log
            Dim tipoOperazione As String = DataLogStructure.TipiArgomento.CNV_MANUALI
            If command.OperazioneAutomatica Then tipoOperazione = DataLogStructure.TipiArgomento.CNV_AUTOMATICHE

            For Each dataConvocazioneOld As DateTime In command.DateConvocazioneOld

                ' E' la stessa cosa ma, usando la variabile del for..each nella lambda expression, visual studio dà un warning
                Dim oldDataCnv As DateTime = dataConvocazioneOld

                If Not datiConvocazioneScartataList.Any(Function(dcs) dcs.DataConvocazione = oldDataCnv) Then
                    '--
                    ' La convocazione non è tra le scartate => procedo con la logica di spostamento/unione
                    '--

                    ' --- Unione convocazioni --- '
                    ' 1 - Se, nella data di unione, non esiste già una convocazione, deve essere creata.
                    ' 2 - Altrimenti, i dati della convocazione nella data di unione vengono modificati 
                    '     in base alla presenza o meno di un appuntamento e se questo appuntamento è da mantenere.
                    ' 3 - Viene modificata la data di convocazione per le vaccinazioni programmate, i cicli,
                    '     i solleciti e i ritardi relativi alla vecchia convocazione, per associarli alla nuova.
                    ' 4 - Controllo bilanci: se è possibile, li sposta nella nuova data, altrimenti li lascia nella data vecchia.
                    ' 5 - Se la convocazione nella data vecchia non ha bilanci associati, la cancella.
                    ' 6 - Altrimenti, modifica i dati cnv_campagna = 'N' e cnv_durata_appuntamento = TEMPOBIL perchè è una cnv solo bilancio.

                    ' Dati della convocazione nella data nuova
                    Dim convocazioneNew As Entities.Convocazione =
                        Me.GenericProvider.Convocazione.GetConvocazionePaziente(command.CodicePaziente, command.DataConvocazioneNew)

                    ' Dati della convocazione nella data vecchia
                    Dim convocazioneOld As Entities.Convocazione =
                        Me.GenericProvider.Convocazione.GetConvocazionePaziente(command.CodicePaziente, dataConvocazioneOld)

                    ' Inserimento/Modifica della nuova convocazione, che viene creata o modificata.
                    ' Vengono gestiti lo storico appuntamenti e il log.
                    InsertUpdateNuovaConvocazione(convocazioneNew, convocazioneOld, dataOperazione, command, tipoOperazione)

                    ' -- Modifica i dati associati alla vecchia convocazione che è stata unita -- '
                    ' La data di convocazione relativa a cicli, solleciti e ritardi del paziente diventa la nuova data (quella di unione). 
                    ' I cicli vengono spostati nella nuova data solo se non sono già presenti, altrimenti non avviene l'update.
                    ' In questo caso, cancello i cicli rimasti nella vecchia data.

                    ' Vaccinazioni Programmate 
                    Me.GenericProvider.VaccinazioneProg.UpdateDataCnv(command.CodicePaziente, dataConvocazioneOld, command.DataConvocazioneNew)

                    ' TODO [storicoAppunt]: vengono spostate le programmate => lo storico deve contenere un record con vaccinazioni-dosi aggiornato?
                    'If countVacProg > 0 AndAlso convocazioneNew.DataAppuntamento > DateTime.MinValue Then
                    '
                    '   Dim convocazioneAppuntamento As Entities.ConvocazioneAppuntamento =
                    '       Me.GenericProvider.Prenotazioni.GetLastStoricoAppuntamenti(command.CodicePaziente, command.DataConvocazioneNew)
                    '
                    '   If Not convocazioneAppuntamento Is Nothing AndAlso Not convocazioneAppuntamento.DataEliminazioneAppuntamento.HasValue Then
                    '       convocazioneAppuntamento.Vaccinazioni = GetVaccinazioniConvocazioneAppuntamento(command.CodicePaziente, command.DataConvocazioneNew)
                    '   End If
                    '
                    '   InsertStoricoAppuntamenti(convocazioneAppuntamento)  oppure update dell'ultimo storico?
                    '
                    'End If

                    ' Cicli (inserimento in nuova data)
                    ' N.B. a differenza della campagna vaccinale, i solleciti del ciclo della vaccinazione da spostare NON vengono persi
                    Me.GenericProvider.Convocazione.InsertCicliCnvUnita(command.CodicePaziente, dataConvocazioneOld, command.DataConvocazioneNew)

                    ' Ritardi (update in nuova data)
                    Me.GenericProvider.Convocazione.UpdateRitardiCnvUnita(command.CodicePaziente, dataConvocazioneOld, command.DataConvocazioneNew)

                    ' Cicli (delete in vecchia data)
                    Me.GenericProvider.Convocazione.DeleteCicliCnvUnita(command.CodicePaziente, dataConvocazioneOld)

                    ' Spostamento bilanci (update in nuova data di convocazione), solo se l'esecuzione dei bilanci è compatibile con tale data.
                    Dim tuttiBilanciSpostati As Boolean =
                        SpostaBilanci(command.CodicePaziente, dataConvocazioneOld, command.DataConvocazioneNew)

                    ' --- Eliminazione/Modifica vecchia convocazione spostata --- '
                    ' La convocazione nella vecchia data deve essere eliminata, a meno chè non ci siano bilanci che non sono stati spostati.
                    If tuttiBilanciSpostati Then

                        ' Non ci sono bilanci associati, quindi elimino la cnv nella vecchia data
                        Dim eliminaConvocazioniCommand As New EliminaConvocazioniCommand()
                        eliminaConvocazioniCommand.CodicePaziente = command.CodicePaziente
                        eliminaConvocazioniCommand.DataConvocazione = dataConvocazioneOld
                        eliminaConvocazioniCommand.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.SpostamentoConvocazione
                        eliminaConvocazioniCommand.DataEliminazione = dataOperazione
                        eliminaConvocazioniCommand.NoteEliminazione = command.NoteSpostamentoAppuntamento + String.Format(" - Eliminazione cnv per spostamento in nuova data {0:dd/MM/yyyy}", command.DataConvocazioneNew)

                        Dim count As Integer = EliminaConvocazioni(eliminaConvocazioniCommand)

                        ' --- Log --- '
                        If count > 0 Then

                            ' I dati della convocazione eliminata vengono copiati in un datatable per eseguire la scrittura nel log
                            Dim dtLogCancCnv As DataTable = Me.CreaDtLog(convocazioneOld)
                            dtLogCancCnv.AcceptChanges()

                            ' Log dell'operazione di cancellazione della vecchia cnv
                            dtLogCancCnv.Rows(0).Delete()

                            LogBox.WriteData(LogBox.GetTestataDataTable(dtLogCancCnv, tipoOperazione))

                        End If

                    Else
                        ' La convocazione non deve essere eliminata perchè è associata ad un bilancio che non è stato spostato.
                        ' In questo caso, aggiorno i dati della convocazione impostando cnv_campagna = 'N' 
                        ' e durata appuntamento uguale al parametro TEMPOBIL (se uno dei due è diverso).
                        If convocazioneOld.CampagnaVaccinale <> "N" Or convocazioneOld.Durata_Appuntamento <> Me.Settings.TEMPOBIL Then

                            Dim flagCampagna As String = "N"

                            Dim durataAppuntamento As Integer = 0
                            If Me.GenericProvider.Paziente.IsSenzaPediatra(command.CodicePaziente) Then
                                durataAppuntamento = Me.Settings.TEMPOBIL
                            End If

                            ' Update convocazione
                            Me.GenericProvider.Convocazione.UpdateDatiConvocazioneSoloBilancio(command.CodicePaziente, dataConvocazioneOld, durataAppuntamento, flagCampagna)

                            ' Inserimento storico appuntamento, se presente e se l'appuntamento non è già stato cancellato.
                            Dim convocazioneAppuntamento As Entities.ConvocazioneAppuntamento =
                                Me.GenericProvider.Prenotazioni.GetLastStoricoAppuntamenti(command.CodicePaziente, dataConvocazioneOld)

                            If Not convocazioneAppuntamento Is Nothing AndAlso Not convocazioneAppuntamento.DataEliminazioneAppuntamento.HasValue Then

                                convocazioneAppuntamento.Id = Me.GenericProvider.Prenotazioni.GetNextIdStoricoAppuntamenti()
                                convocazioneAppuntamento.DurataAppuntamento = durataAppuntamento
                                convocazioneAppuntamento.Campagna = flagCampagna
                                convocazioneAppuntamento.Note = command.NoteSpostamentoAppuntamento +
                                    String.Format(" - Convocazione non eliminata: presenza bilancio non posticipabile in nuova data cnv: {0:dd/MM/yyyy}", command.DataConvocazioneNew) +
                                    GetNoteUtenteDataVariazione(dataOperazione)

                                InsertStoricoAppuntamenti(convocazioneAppuntamento)

                            End If

                            ' --- Log --- '
                            Dim recordLog As New DataLogStructure.Record()
                            recordLog.Campi.Add(New DataLogStructure.Campo("CNV_CAMPAGNA", convocazioneOld.CampagnaVaccinale, flagCampagna))
                            recordLog.Campi.Add(New DataLogStructure.Campo("CNV_DURATA_APPUNTAMENTO", convocazioneOld.Durata_Appuntamento, Me.Settings.TEMPOBIL))

                            Dim testataLog As New DataLogStructure.Testata(tipoOperazione, DataLogStructure.Operazione.Modifica)
                            testataLog.Records.Add(recordLog)

                            LogBox.WriteData(testataLog)

                        End If
                    End If

                Else
                    ' In questo caso, la convocazione non ha vaccinazioni associate. 
                    ' Se eventuali bilanci nella data di convocazione sono <> "UX" posso eliminare la cnv.
                    Dim bilanci As BilancioProgrammatoCollection = Nothing
                    Using bizBilanci As New Biz.BizBilancioProgrammato(Me.GenericProvider, Me.Settings, Me.ContextInfos, Me.LogOptions)
                        bilanci = bizBilanci.CercaBilanci(command.CodicePaziente, dataConvocazioneOld, Constants.StatiBilancio.UNEXECUTED)
                    End Using

                    If bilanci Is Nothing OrElse bilanci.Count = 0 Then

                        ' Eliminazione convocazione
                        Dim eliminaConvocazioniCommand As New EliminaConvocazioniCommand()
                        eliminaConvocazioniCommand.CodicePaziente = command.CodicePaziente
                        eliminaConvocazioniCommand.DataConvocazione = dataConvocazioneOld
                        eliminaConvocazioniCommand.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.SpostamentoConvocazione
                        eliminaConvocazioniCommand.DataEliminazione = dataOperazione
                        eliminaConvocazioniCommand.NoteEliminazione = command.NoteSpostamentoAppuntamento + String.Format(" - Eliminazione cnv per spostamento in nuova data {0:dd/MM/yyyy}", command.DataConvocazioneNew)

                        EliminaConvocazioni(eliminaConvocazioniCommand)

                    End If
                End If

            Next

            success = True

        End If

        Return New SpostaConvocazioneBizResult(success, datiConvocazioneScartataList.ToArray())

    End Function

    Private Sub InsertUpdateNuovaConvocazione(convocazioneNew As Entities.Convocazione, convocazioneOld As Entities.Convocazione, dataOperazione As DateTime, command As SpostaConvocazioneCommand, tipoOperazioneLog As String)

        If convocazioneNew Is Nothing Then
            '--
            ' Nella nuova data non c'è la convocazione => la creo in base ai dati della vecchia.
            '--

            ' Copia i dati della vecchia convocazione nella nuova, tranne la data e quelli relativi all'appuntamento, che viene annullato
            convocazioneNew = New Entities.Convocazione()
            convocazioneNew.Paz_codice = convocazioneOld.Paz_codice
            convocazioneNew.Data_CNV = command.DataConvocazioneNew      ' la data di convocazione è la nuova data
            convocazioneNew.EtaPomeriggio = convocazioneOld.EtaPomeriggio
            convocazioneNew.Rinvio = convocazioneOld.Rinvio
            convocazioneNew.DataAppuntamento = Date.MinValue
            convocazioneNew.TipoAppuntamento = String.Empty

            If command.DurataAppuntamentoNew.HasValue Then
                convocazioneNew.Durata_Appuntamento = command.DurataAppuntamentoNew.Value
            Else
                convocazioneNew.Durata_Appuntamento = convocazioneOld.Durata_Appuntamento
            End If

            If convocazioneOld.DataPrimoAppuntamento = DateTime.MinValue Then
                convocazioneNew.DataPrimoAppuntamento = convocazioneOld.DataAppuntamento
            Else
                convocazioneNew.DataPrimoAppuntamento = convocazioneOld.DataPrimoAppuntamento
            End If

            convocazioneNew.DataInvio = Date.MinValue
            convocazioneNew.Cns_Codice = convocazioneOld.Cns_Codice
            convocazioneNew.IdUtente = convocazioneOld.IdUtente
            convocazioneNew.CodiceAmbulatorio = -1
            convocazioneNew.CampagnaVaccinale = convocazioneOld.CampagnaVaccinale

            ' I dati relativi all'inserimento rimangono quelli della VECCHIA convocazione
            convocazioneNew.DataInserimento = convocazioneOld.DataInserimento
            convocazioneNew.IdUtenteInserimento = convocazioneOld.IdUtenteInserimento

            ' Da non usare:
            '   convocazioneNew.NumeroBilancio()
            '   convocazioneNew.CodMalattiaBilancio()

            ' Inserimento dati della nuova convocazione
            If Me.GenericProvider.Convocazione.InsertConvocazione(convocazioneNew) Then

                ' N.B. : non effettuo l'inserimento in storico appuntamenti perchè la nuova convocazione non ha l'appuntamento

                ' Log dell'operazione di inserimento della nuova cnv
                Dim dtLogCnv As DataTable = Me.CreaDtLog(convocazioneNew)
                LogBox.WriteData(LogBox.GetTestataDataTable(dtLogCnv, tipoOperazioneLog))

            End If

        Else
            '--
            ' La nuova convocazione esiste già: è necessario modificarne i dati in base alla presenza o meno di appuntamenti
            '--

            ' I campi seguenti non vengono modificati (mantengono i dati della nuova cnv):
            '   convocazioneNew.Paz_codice
            '   convocazioneNew.Data_CNV
            '   convocazioneNew.EtaPomeriggio
            '   convocazioneNew.Rinvio

            ' Creazione datatable per Log. Il datatable contiene i dati originali della nuova convocazione. Questi dati verranno
            ' modificati sia nella cnv che nel datatable. Il log registrerà le modifiche effettuate.
            Dim dtLogCnv As DataTable = CreaDtLog(convocazioneNew)

            ' Calcolo dati dell'appuntamento da mantenere (se il flag è true)
            ' L'appuntamento da mantenere è:
            '   - quello vecchio, se solo la cnv vecchia aveva l'appuntamento (e la nuova no).
            '   - quello nuovo, se solo la cnv nuova aveva l'appuntamento o se ce lo avevano entrambe.

            ' Mantengo la vecchia data del nuovo appuntamento per gestire lo storico degli appuntamenti
            Dim convocazioneNew_dataAppuntamentoOld As DateTime = convocazioneNew.DataAppuntamento

            ' Imposto in questo oggetto convocazione i dati che mi servono per la gestione dell'appuntamento
            Dim datiCnvApp As Entities.Convocazione =
                GetDatiAppuntamentoPerConvocazione(convocazioneNew, convocazioneOld, command.DurataAppuntamentoNew)

            If command.MantieniAppuntamento Then

                ' Se il flag è impostato a true, nella nuova convocazione devono essere replicati
                ' tutti i dati relativi all'appuntamento determinato in precedenza.
                convocazioneNew.DataAppuntamento = datiCnvApp.DataAppuntamento
                convocazioneNew.IdUtente = datiCnvApp.IdUtente
                convocazioneNew.TipoAppuntamento = datiCnvApp.TipoAppuntamento
                convocazioneNew.Durata_Appuntamento = datiCnvApp.Durata_Appuntamento
                convocazioneNew.Cns_Codice = datiCnvApp.Cns_Codice
                convocazioneNew.CodiceAmbulatorio = datiCnvApp.CodiceAmbulatorio
                convocazioneNew.DataInvio = datiCnvApp.DataInvio

            Else

                ' I dati di un eventuale appuntamento devono essere annullati 
                convocazioneNew.DataAppuntamento = Date.MinValue
                convocazioneNew.TipoAppuntamento = String.Empty
                convocazioneNew.Durata_Appuntamento = Me.Settings.TEMPOSED
                convocazioneNew.CodiceAmbulatorio = -1
                convocazioneNew.DataInvio = Date.MinValue

                ' I seguenti dati rimangono quelli della nuova convocazione:
                '   convocazioneNew.IdUtente
                '   convocazioneNew.Cns_Codice

            End If

            ' Se la data del primo appuntamento della nuova cnv non è impostata la valorizzo, altrimenti la lascio com'è.
            ' Viene assegnata la data del primo appuntamento calcolata precedentemente.
            If convocazioneNew.DataPrimoAppuntamento = Date.MinValue Then
                convocazioneNew.DataPrimoAppuntamento = datiCnvApp.DataPrimoAppuntamento
            End If

            ' Se una delle due convocazioni fa parte della campagna, la nuova diventa in campagna
            If convocazioneNew.CampagnaVaccinale = "S" Or convocazioneOld.CampagnaVaccinale = "S" Then
                convocazioneNew.CampagnaVaccinale = "S"
            Else
                convocazioneNew.CampagnaVaccinale = "N"
            End If

            ' N.B. : i dati relativi all'inserimento rimangono quelli della NUOVA convocazione
            ' convocazioneNew.DataInserimento
            ' convocazioneNew.IdUtenteInserimento

            ' Update della nuova convocazione
            If Me.GenericProvider.Convocazione.UpdateConvocazione(convocazioneNew) Then

                ' Storico Appuntamenti (solo se la data di appuntamento è cambiata)
                If convocazioneNew_dataAppuntamentoOld <> convocazioneNew.DataAppuntamento Then
                    '--
                    ' Se la data di appuntamento è stata cancellata
                    '       => update storico appuntamenti con i dati di eliminazione
                    '--
                    ' Se la data di appuntamento è stata variata 
                    '       => update storico appuntamenti con i dati di eliminazione + insert storico appuntamenti con i dati del nuovo appuntamento
                    '--

                    Dim noteAppuntamento As String = command.NoteSpostamentoAppuntamento +
                        String.Format(" - Update convocazione del: {0:dd/MM/yyyy} per UNIONE con vecchia convocazione del: {1:dd/MM/yyyy}", convocazioneNew.Data_CNV, convocazioneOld.Data_CNV)

                    If convocazioneNew_dataAppuntamentoOld > DateTime.MinValue Then

                        ' Nuova convocazione => Appuntamento eliminato
                        ' Aggiornamento storico appuntamento per cancellazione appuntamento nuova convocazione
                        UpdateStoricoAppuntamenti_Eliminazione(command.CodicePaziente, {command.DataConvocazioneNew}.ToList(), dataOperazione,
                                                               Constants.MotiviEliminazioneAppuntamento.SpostamentoConvocazione, noteAppuntamento + " - Eliminazione appuntamento", String.Empty)

                    End If

                    If convocazioneNew.DataAppuntamento > DateTime.MinValue Then

                        ' Nuova convocazione => Appuntamento modificato
                        ' Inserimento storico appuntamento per creazione nuovo appuntamento della nuova convocazione
                        Dim convocazioneAppuntamento As Entities.ConvocazioneAppuntamento =
                            CreateConvocazioneAppuntamento(convocazioneNew, noteAppuntamento + " - Variazione data appuntamento", dataOperazione)

                        InsertStoricoAppuntamenti(convocazioneAppuntamento)

                    End If

                End If

                ' --- Log --- '
                ' Contiene i dati della nuova cnv prima delle modifiche.
                dtLogCnv.AcceptChanges()

                ' Contiene i dati della nuova cnv dopo le modifiche.
                Dim dtLogCnvUpdated As DataTable = Me.CreaDtLog(convocazioneNew)

                ' Ciclo di modifica dei dati
                For j As Int16 = 0 To dtLogCnv.Columns.Count - 1
                    dtLogCnv.Rows(0)(j) = dtLogCnvUpdated.Rows(0)(j)
                Next

                LogBox.WriteData(LogBox.GetTestataDataTable(dtLogCnv, tipoOperazioneLog))

            End If
        End If

    End Sub

    Private Function CreaDtLog(convocazione As Entities.Convocazione) As DataTable

        Dim dt As New DataTable()
        dt.Columns.Add("cnv_cns_codice")
        dt.Columns.Add("cnv_paz_codice")
        dt.Columns.Add("cnv_data", GetType(Date))
        dt.Columns.Add("cnv_data_appuntamento", GetType(Date))
        dt.Columns.Add("cnv_durata_appuntamento")
        dt.Columns.Add("cnv_data_invio", GetType(Date))
        dt.Columns.Add("cnv_eta_pomeriggio")
        dt.Columns.Add("cnv_rinvio")
        dt.Columns.Add("cnv_tipo_appuntamento")
        dt.Columns.Add("cnv_ute_id")
        dt.Columns.Add("cnv_primo_appuntamento", GetType(Date))
        dt.Columns.Add("cnv_amb_codice")
        dt.Columns.Add("cnv_campagna")
        dt.Columns.Add("cnv_data_inserimento", GetType(Date))
        dt.Columns.Add("cnv_ute_id_inserimento")

        Dim row As DataRow = dt.NewRow()

        row("cnv_cns_codice") = convocazione.Cns_Codice
        row("cnv_paz_codice") = convocazione.Paz_codice
        row("cnv_data") = convocazione.Data_CNV

        If convocazione.DataAppuntamento = Date.MinValue Then
            row("cnv_data_appuntamento") = DBNull.Value
        Else
            row("cnv_data_appuntamento") = convocazione.DataAppuntamento
        End If

        If convocazione.Durata_Appuntamento = -1 Then
            row("cnv_durata_appuntamento") = DBNull.Value
        Else
            row("cnv_durata_appuntamento") = convocazione.Durata_Appuntamento
        End If

        If convocazione.DataInvio = Date.MinValue Then
            row("cnv_data_invio") = DBNull.Value
        Else
            row("cnv_data_invio") = convocazione.DataInvio
        End If

        row("cnv_eta_pomeriggio") = convocazione.EtaPomeriggio
        row("cnv_rinvio") = convocazione.Rinvio
        row("cnv_tipo_appuntamento") = convocazione.TipoAppuntamento

        If convocazione.IdUtente = -1 Then
            row("cnv_ute_id") = DBNull.Value
        Else
            row("cnv_ute_id") = convocazione.IdUtente
        End If

        If convocazione.DataPrimoAppuntamento = Date.MinValue Then
            row("cnv_primo_appuntamento") = DBNull.Value
        Else
            row("cnv_primo_appuntamento") = convocazione.DataPrimoAppuntamento
        End If

        If convocazione.CodiceAmbulatorio = -1 Then
            row("cnv_amb_codice") = DBNull.Value
        Else
            row("cnv_amb_codice") = convocazione.CodiceAmbulatorio
        End If

        row("cnv_campagna") = convocazione.CampagnaVaccinale

        If convocazione.DataInserimento.HasValue Then
            row("cnv_data_inserimento") = convocazione.DataInserimento.Value
        Else
            row("cnv_data_inserimento") = DBNull.Value
        End If

        If convocazione.IdUtenteInserimento.HasValue Then
            row("cnv_ute_id_inserimento") = convocazione.IdUtenteInserimento.Value
        Else
            row("cnv_ute_id_inserimento") = DBNull.Value
        End If

        dt.Rows.Add(row)

        Return dt

    End Function

    Private Function SpostaBilanci(codicePaziente As Long, dataConvocazioneOld As DateTime, dataConvocazioneNew As DateTime) As Boolean

        Dim tuttiBilanciSpostati As Boolean = True

        Using bizBilanci As New Biz.BizBilancioProgrammato(Me.GenericProvider, Me.Settings, Me.ContextInfos, Me.LogOptions)

            ' Caricamento bilanci in stato "UX" del paziente nella data di convocazione
            Dim bilancioProgrammatoCollection As Collection.BilancioProgrammatoCollection =
                bizBilanci.CercaBilanci(codicePaziente, dataConvocazioneOld, Constants.StatiBilancio.UNEXECUTED)

            If Not bilancioProgrammatoCollection Is Nothing AndAlso bilancioProgrammatoCollection.Count > 0 Then

                ' Per ogni bilancio in stato UX, controllo se è possibile eseguirlo nella nuova data di convocazione.
                ' Se si, modifico la data di cnv del bilancio, altrimenti il bilancio rimane nella vecchia data.
                ' Se almeno un bilancio non viene spostato, la vecchia cnv non deve essere eliminata.
                For j As Int16 = 0 To bilancioProgrammatoCollection.Count - 1

                    If (bizBilanci.VerificaDataCNV(codicePaziente, bilancioProgrammatoCollection(j).N_bilancio, bilancioProgrammatoCollection(j).Mal_codice, dataConvocazioneNew)) Then

                        bilancioProgrammatoCollection(j).New_Cnv = True
                        bilancioProgrammatoCollection(j).Data_CNV = dataConvocazioneNew

                        bizBilanci.AggiornaBilancio(bilancioProgrammatoCollection(j))

                    Else

                        tuttiBilanciSpostati = False

                    End If

                Next

            End If

        End Using

        Return tuttiBilanciSpostati

    End Function

    ''' <summary>
    ''' Restituisce un oggetto convocazione. I dati relativi all'appuntamento vengono valorizzati con:  
    '''  - i dati appartenenti alla nuova convocazione, se ha un appuntamento associato;
    '''  - i dati della vecchia convocazione, se la nuova non ha un appuntamento.
    ''' </summary>
    ''' <param name="convocazioneNew"></param>
    ''' <param name="convocazioneOld"></param>
    ''' <param name="durataNew"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetDatiAppuntamentoPerConvocazione(convocazioneNew As Entities.Convocazione, convocazioneOld As Entities.Convocazione, durataNew As Integer?) As Entities.Convocazione

        Dim datiCnvApp As New Entities.Convocazione()

        ' Assegno i dati dell'appuntamento della nuova cnv. 
        ' Se ci sarà solo l'appuntamento per la vecchia cnv, verranno sovrascritti.
        datiCnvApp.DataAppuntamento = convocazioneNew.DataAppuntamento
        datiCnvApp.IdUtente = convocazioneNew.IdUtente
        datiCnvApp.TipoAppuntamento = convocazioneNew.TipoAppuntamento
        datiCnvApp.Durata_Appuntamento = convocazioneNew.Durata_Appuntamento
        datiCnvApp.Cns_Codice = convocazioneNew.Cns_Codice
        datiCnvApp.CodiceAmbulatorio = convocazioneNew.CodiceAmbulatorio
        datiCnvApp.DataInvio = convocazioneNew.DataInvio
        datiCnvApp.DataPrimoAppuntamento = convocazioneNew.DataPrimoAppuntamento

        ' Determino se deve essere mantenuto l'appuntamento vecchio
        If convocazioneOld.DataAppuntamento > Date.MinValue AndAlso convocazioneNew.DataAppuntamento = Date.MinValue Then

            ' La vecchia cnv ha un appuntamento associato && la nuova cnv non ha un appuntamento
            '   => assegno i dati dell'appuntamento della vecchia cnv
            datiCnvApp.DataAppuntamento = convocazioneOld.DataAppuntamento
            datiCnvApp.IdUtente = convocazioneOld.IdUtente
            datiCnvApp.TipoAppuntamento = convocazioneOld.TipoAppuntamento

            If durataNew.HasValue Then
                datiCnvApp.Durata_Appuntamento = durataNew.Value
            Else
                datiCnvApp.Durata_Appuntamento = convocazioneOld.Durata_Appuntamento
            End If

            datiCnvApp.Cns_Codice = convocazioneOld.Cns_Codice
            datiCnvApp.CodiceAmbulatorio = convocazioneOld.CodiceAmbulatorio
            datiCnvApp.DataInvio = convocazioneOld.DataInvio
            datiCnvApp.DataPrimoAppuntamento = convocazioneOld.DataPrimoAppuntamento

        End If

        Return datiCnvApp

    End Function

    Private Function GetNoteUtenteDataVariazione(dataOperazione As DateTime) As String

        Return String.Format(" [utente: {0}; data operazione: {1:dd/MM/yyyy HH:mm:ss}]", Me.ContextInfos.IDUtente, dataOperazione)

    End Function

#Region " Convocazioni Scartate "

    Public Shared Function CreateConvocazioneScartata(codicePaziente As Integer, oldDataConvocazione As Date, newDataConvocazione As Date, codiceCiclo As String, numeroSeduta As Integer, intervallo As Integer, codiceMotivoScarto As String, codiceVaccinazioneList As List(Of String)) As DatiConvocazioneScartata

        Dim convocazioneScartata As New DatiConvocazioneScartata()
        convocazioneScartata.CodicePaziente = codicePaziente
        convocazioneScartata.DataConvocazione = oldDataConvocazione
        convocazioneScartata.CodiceCiclo = codiceCiclo
        convocazioneScartata.NumeroSeduta = numeroSeduta
        convocazioneScartata.Intervallo = intervallo
        convocazioneScartata.CodiceMotivo = codiceMotivoScarto
        convocazioneScartata.DescrizioneMotivo = BizConvocazione.GetDescrizioneMotivoCnvScartata(codiceMotivoScarto, oldDataConvocazione, newDataConvocazione, codiceVaccinazioneList)
        convocazioneScartata.CodiceVaccinazioneList = codiceVaccinazioneList

        Return convocazioneScartata

    End Function

    Public Shared Function CreateConvocazioneScartata(codicePaziente As Integer, oldDataConvocazione As Date, newDataConvocazione As Date, codiceMotivoScarto As String) As DatiConvocazioneScartata

        Return BizConvocazione.CreateConvocazioneScartata(codicePaziente, oldDataConvocazione, newDataConvocazione, String.Empty, 0, 0, codiceMotivoScarto, Nothing)

    End Function

    Private Shared Function GetDescrizioneMotivoCnvScartata(motivo As CodiceMotivoConvocazioneScartata, dataConvocazione As DateTime, dataConvocazioneUnione As DateTime, codiceVaccinazioneList As List(Of String)) As String

        Select Case motivo

            Case CodiceMotivoConvocazioneScartata.StesseVaccinazioniInDataUnione
                ' Nell'unione cnv, se c'è la stessa vaccinazione in entrambe le convocazioni che si vogliono unire.
                Return String.Format("La convocazione del {0:dd/MM/yyyy} contiene una o più vaccinazioni in comune con quella del {1:dd/MM/yyyy} che si sta cercando di unire ad essa.", dataConvocazioneUnione, dataConvocazione)

            Case CodiceMotivoConvocazioneScartata.NonAssociabilitaInDataUnione
                ' Nell'unione cnv, se ci sono vaccinazioni non co-somministrabili nella data di unione.
                Return String.Format("La convocazione in data {0:dd/MM/yyyy} contiene vaccinazioni non somministrabili contemporaneamente a quelle della convocazione del {1:dd/MM/yyyy} che si sta cercando di unire ad essa.", dataConvocazioneUnione, dataConvocazione)

            Case CodiceMotivoConvocazioneScartata.NonAssociabilitaIntervallo
                ' Nel calcolo convocazioni
                Return String.Format("La convocazione in data {0:dd/MM/yyyy} non puo' essere programmata poiche' esistono, nell'intervallo di validita', altre convocazioni per vaccinazioni non somministrabili contemporaneamente ad essa.", dataConvocazione)

            Case CodiceMotivoConvocazioneScartata.NonAssociabilitaDataCnv
                ' Caso previsto ma non ancora gestito
                Return String.Format("La convocazione del {0:dd/MM/yyyy} non puo' essere programmata poiche' esistono, in tale data, altre convocazioni per vaccinazioni non somministrabili contemporaneamente ad essa.", dataConvocazione)

            Case CodiceMotivoConvocazioneScartata.CampagnaVaccinazioneProgrammata
                ' Nella campagna vaccinale, se ci sono vaccinazioni già programmate
                Dim codiceVaccinazione As String = String.Empty
                If Not codiceVaccinazioneList Is Nothing AndAlso codiceVaccinazioneList.Count > 0 Then codiceVaccinazione = codiceVaccinazioneList(0).ToString() + " "
                Return String.Format("La convocazione non puo' essere programmata poiche' la vaccinazione {0}e' gia' programmata in un'altra seduta.", codiceVaccinazione, dataConvocazione)

            Case CodiceMotivoConvocazioneScartata.CampagnaVaccinazioneEsclusa
                ' Nella campagna vaccinale, se ci sono vaccinazioni escluse
                Dim codiceVaccinazione As String = String.Empty
                If Not codiceVaccinazioneList Is Nothing AndAlso codiceVaccinazioneList.Count > 0 Then codiceVaccinazione = codiceVaccinazioneList(0).ToString()
                Return String.Format("E' presente un'esclusione per la vaccinazione{0}.", codiceVaccinazione)

            Case CodiceMotivoConvocazioneScartata.VaccinazioniProgrammateAssenti
                Return "La convocazione non può essere spostata poiche' non e' presente alcuna vaccinazione programmata."

            Case CodiceMotivoConvocazioneScartata.CampagnaConvocazioneEsistenteInStessaDataAltroCns
                ' Nella campagna vaccinale, se esistono cnv in stessa data e in altro cns
                Return String.Format("La convocazione del {0:dd/MM/yyyy} non puo' essere programmata poiche' il paziente ha gia' una convocazione nella stessa data in un altro centro vaccinale.", dataConvocazione)

            Case Else
                Throw New NotImplementedException()

        End Select

        Return String.Empty

    End Function

#End Region

#End Region

#Region " Modifica dati di convocazione "

    ''' <summary>
    ''' Update durata della convocazione specificata. Se la convocazione ha l'appuntamento, 
    ''' viene inserito anche un record nello storico degli appuntamenti, per registrare la modifica della durata.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataConvocazione"></param>
    ''' <param name="durataAppuntamentoOld"></param>
    ''' <param name="durataAppuntamentoNew"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ModificaDurataAppuntamento(codicePaziente As Long, dataConvocazione As DateTime, durataAppuntamentoNew As Integer?) As BizGenericResult

        Dim result As New BizGenericResult()

        Try
            ' Caricamento cnv per log vecchia durata
            Dim convocazioneOld As Entities.Convocazione = Me.GenericProvider.Convocazione.GetConvocazionePaziente(codicePaziente, dataConvocazione)

            If Not durataAppuntamentoNew.HasValue Then
                durataAppuntamentoNew = Me.Settings.TEMPOSED
            End If

            ' Modifica della durata
            Dim updated As Boolean = Me.GenericProvider.Convocazione.UpdateDurataAppuntamento(codicePaziente, dataConvocazione, durataAppuntamentoNew)

            If updated Then

                ' Storico appuntamenti: inserimento nuova riga di storico con la durata modificata
                Dim convocazioneAppuntamentoOld As Entities.ConvocazioneAppuntamento = Me.GenericProvider.Prenotazioni.GetLastStoricoAppuntamenti(codicePaziente, dataConvocazione)
                If Not convocazioneAppuntamentoOld Is Nothing Then

                    Dim convocazioneAppuntamentoNew As Entities.ConvocazioneAppuntamento = convocazioneAppuntamentoOld.Clone()
                    convocazioneAppuntamentoNew.DurataAppuntamento = durataAppuntamentoNew
                    convocazioneAppuntamentoNew.Note = String.Format("Durata dell'appuntamento modificata [utente: {0}, data di modifica: {1:dd/MM/yyyy HH:mm}]", Me.ContextInfos.IDUtente, Date.Now)

                    InsertStoricoAppuntamenti(convocazioneAppuntamentoNew)

                End If

                ' Log
                Dim logOptions As BizLogOptions = Me.LogOptions
                If logOptions Is Nothing OrElse String.IsNullOrWhiteSpace(logOptions.CodiceArgomento) Then
                    logOptions = New BizLogOptions(DataLogStructure.TipiArgomento.CNV_MANUALI, False)
                End If

                Dim recordLog As New DataLogStructure.Record()
                recordLog.Campi.Add(New DataLogStructure.Campo("Convocazione", dataConvocazione.ToString("dd/MM/yyyy")))
                recordLog.Campi.Add(New DataLogStructure.Campo("Durata", convocazioneOld.Durata_Appuntamento.ToString(), durataAppuntamentoNew.ToString()))

                Dim testataLog As New DataLogStructure.Testata(logOptions.CodiceArgomento, DataLogStructure.Operazione.Modifica, logOptions.Automatico)
                testataLog.Records.Add(recordLog)

                LogBox.WriteData(testataLog)

            End If

            result.Message = "La durata dell'appuntamento è stata modificata con successo!"
            result.Success = True

        Catch ex As Exception
            result.Message = "E' avvenuto un errore nel salvataggio dei dati: " + ex.Message
            result.Success = False
        End Try

        Return result

    End Function

#End Region

#End Region

#Region "Metodi di Delete"

#Region " Types "

    Public Class EliminaConvocazioniCommand

        Public CodicePaziente As Integer
        Public DataConvocazione As DateTime?
        Public IdMotivoEliminazione As String
        Public DataEliminazione As DateTime?
        Public NoteEliminazione As String

    End Class

    Public Class EliminaConvocazioniSollecitiBilanciCommand
        Inherits EliminaConvocazioniCommand

        Public WriteLog As Boolean
        Public CancellaBilanciAssociati As Boolean

    End Class

    Public Class EliminaConvocazioneEmptyCommand
        Inherits EliminaConvocazioniCommand

        Public WriteLog As Boolean

    End Class

    Public Class EliminaConvocazioneEmptyResult
        Public Success As Boolean
        Public ResultType As EliminaConvocazioneEmptyResultType
        Public Message As String
    End Class

    Public Enum EliminaConvocazioneEmptyResultType
        Success
        CnvNonEliminata
        Exception
    End Enum

#End Region

    ''' <summary>
    ''' Cancella esclusivamente le convocazioni specificate (non cancella bilanci nè solleciti di bilancio).
    ''' Non scrive il log delle operazioni.
    ''' In caso di cancellazione di una convocazione con appuntamento, aggiorna anche lo storico degli appuntamenti.
    ''' </summary>
    ''' <param name="command"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function EliminaConvocazioni(command As EliminaConvocazioniCommand) As Integer

        ' Cancellazione delle sole convocazioni (no bilanci) 
        Dim count As Integer = Me.GenericProvider.Convocazione.DeleteConvocazione(command.CodicePaziente, command.DataConvocazione)

        ' Aggiornamento storico appuntamenti per dati eliminazione
        If count > 0 Then

            ' Data di eliminazione appuntamenti
            Dim dataEliminazioneEffettiva As DateTime = DateTime.Now
            If command.DataEliminazione.HasValue Then dataEliminazioneEffettiva = command.DataEliminazione.Value

            ' Motivo eliminazione appuntamenti
            Dim idMotivoEliminazione As String = command.IdMotivoEliminazione
            If String.IsNullOrWhiteSpace(idMotivoEliminazione) Then idMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.EliminazioneConvocazione

            ' Note eliminazione appuntamenti
            Dim note As String = command.NoteEliminazione
            If String.IsNullOrWhiteSpace(note) Then note = "Eliminazione convocazione"

            UpdateStoricoAppuntamenti_Eliminazione(command.CodicePaziente, command.DataConvocazione, dataEliminazioneEffettiva, idMotivoEliminazione, note, String.Empty)

        End If

        Return count

    End Function

    ''' <summary>
    ''' Elimina le convocazioni nelle date specificate, se non ci sono dati associati
    ''' </summary>
    ''' <param name="command"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function EliminaConvocazioneEmpty(command As EliminaConvocazioneEmptyCommand) As EliminaConvocazioneEmptyResult

        Dim result As New EliminaConvocazioneEmptyResult()
        result.Success = True
        result.ResultType = EliminaConvocazioneEmptyResultType.Success
        result.Message = String.Empty

        ' Date di convocazione
        Dim dateConvocazioniEnumerable As IEnumerable(Of DateTime) = Nothing

        If command.DataConvocazione.HasValue Then
            dateConvocazioniEnumerable = {command.DataConvocazione.Value}.AsEnumerable()
        Else
            dateConvocazioniEnumerable = Me.GenericProvider.Convocazione.GetDateConvocazioniPaziente(command.CodicePaziente, Nothing)
        End If

        ' Data di eliminazione appuntamenti
        Dim dataEliminazioneEffettiva As DateTime = DateTime.Now
        If command.DataEliminazione.HasValue Then dataEliminazioneEffettiva = command.DataEliminazione.Value

        ' Motivo eliminazione appuntamenti
        Dim idMotivoEliminazione As String = command.IdMotivoEliminazione
        If String.IsNullOrWhiteSpace(idMotivoEliminazione) Then idMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.EliminazioneConvocazione

        ' Note eliminazione appuntamenti
        Dim note As String = command.NoteEliminazione
        If String.IsNullOrWhiteSpace(note) Then note = "Eliminazione convocazione"

        Dim deleted As Boolean = False

        Try
            For Each dataSingolaConvocazione As DateTime In dateConvocazioniEnumerable

                If Me.GenericProvider.Convocazione.DeleteEmpty(command.CodicePaziente, dataSingolaConvocazione) Then

                    deleted = True
                    UpdateStoricoAppuntamenti_Eliminazione(command.CodicePaziente, dataSingolaConvocazione, dataEliminazioneEffettiva, idMotivoEliminazione, note, String.Empty)

                Else
                    result.Success = False
                    result.ResultType = EliminaConvocazioneEmptyResultType.CnvNonEliminata
                    result.Message = "Convocazione non eliminata: dati associati."
                End If

            Next

        Catch ex As Exception
            result.Success = False
            result.ResultType = EliminaConvocazioneEmptyResultType.Exception
            result.Message = ex.Message
        End Try

        If deleted AndAlso command.WriteLog Then

            Dim recordLog As New DataLogStructure.Record()
            recordLog.Campi.Add(New DataLogStructure.Campo("CNV", "Cancellazione convocazioni paziente (EliminaConvocazioneEmpty)"))

            Dim testataLog As New DataLogStructure.Testata(DataLogStructure.TipiArgomento.ELIMINA_PROG, DataLogStructure.Operazione.Eliminazione, command.CodicePaziente, True)
            testataLog.Records.Add(recordLog)

            LogBox.WriteData(testataLog)

        End If

        Return result

    End Function

    ''' <summary>
    ''' Elimina le convocazioni specificate.
    ''' Se non è specificata nessuna data, elimina tutte le convocazioni del paziente.
    ''' Se CancellaBilanciAssociati è true, cancella anche i bilanci associati alla convocazione.
    ''' Per le convocazioni con appuntamento, aggiorna lo storico appuntamenti con i dati di eliminazione (data, utente, motivo, note).
    ''' Se WriteLog è true, scrive anche il log delle operazioni effettuate.
    ''' </summary>
    ''' <param name="command"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function EliminaConvocazioniSollecitiBilanci(command As EliminaConvocazioniSollecitiBilanciCommand) As Boolean

        ' Lista date di convocazioni di cui deve essere aggiornato lo storico
        Dim listDateConvocazione As New List(Of DateTime)()

        If command.DataConvocazione.HasValue Then

            listDateConvocazione.Add(command.DataConvocazione.Value)

        Else

            Dim convocazioni As ICollection(Of Entities.Convocazione) = Me.GenericProvider.Convocazione.GetConvocazioniPaziente(command.CodicePaziente.ToString(), True)

            If Not convocazioni.IsNullOrEmpty() Then
                listDateConvocazione.AddRange(convocazioni.Select(Function(item) item.Data_CNV))
            End If

        End If

        Dim deleted As Boolean = False

        Dim transactionOptions As New Transactions.TransactionOptions()
        transactionOptions.IsolationLevel = Transactions.IsolationLevel.ReadCommitted

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, transactionOptions)

            ' Eliminazione della convocazione specificata
            deleted = Me.GenericProvider.Convocazione.Delete(command.CodicePaziente, command.DataConvocazione, command.CancellaBilanciAssociati)

            If deleted Then

                ' Storico Appuntamenti
                If Not listDateConvocazione.IsNullOrEmpty() Then

                    UpdateStoricoAppuntamenti_Eliminazione(command.CodicePaziente, listDateConvocazione, command.DataEliminazione, command.IdMotivoEliminazione, command.NoteEliminazione, String.Empty)

                End If

                ' Log
                If command.WriteLog Then

                    Dim recordLog As New DataLogStructure.Record()

                    If command.DataConvocazione.HasValue Then
                        recordLog.Campi.Add(New DataLogStructure.Campo("Data convocazione eliminata", command.DataConvocazione.Value))
                    Else
                        recordLog.Campi.Add(New DataLogStructure.Campo("Eliminate convocazioni paziente", ""))
                    End If

                    Dim codiceArgomentoLog As String = DataLogStructure.TipiArgomento.VAC_PROGRAMMATE
                    Dim isOperazioneAutomatica As Boolean = True

                    If Not Me.LogOptions Is Nothing Then

                        If Not String.IsNullOrWhiteSpace(Me.LogOptions.CodiceArgomento) Then
                            codiceArgomentoLog = Me.LogOptions.CodiceArgomento
                        End If

                        isOperazioneAutomatica = Me.LogOptions.Automatico

                    End If

                    Dim testataLog As New DataLogStructure.Testata(codiceArgomentoLog, DataLogStructure.Operazione.Eliminazione, isOperazioneAutomatica)
                    testataLog.Records.Add(recordLog)

                    LogBox.WriteData(testataLog)

                End If

            End If

            transactionScope.Complete()

        End Using

        Return deleted

    End Function

#End Region

#Region " Storico Appuntamenti "

    ''' <summary>
    ''' Inserimento in storico appuntamenti.
    ''' </summary>
    ''' <param name="convocazioneAppuntamento"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function InsertStoricoAppuntamenti(convocazioneAppuntamento As Entities.ConvocazioneAppuntamento) As Integer

        convocazioneAppuntamento.Id = Me.GenericProvider.Prenotazioni.GetNextIdStoricoAppuntamenti()

        convocazioneAppuntamento.Note = GetNoteStoricoAppuntamenti(convocazioneAppuntamento.Note)

        Return Me.GenericProvider.Prenotazioni.InsertStoricoAppuntamenti(convocazioneAppuntamento)

    End Function

    ''' <summary>
    ''' Aggiornamento storico appuntamenti per dati eliminazione.
    ''' L'aggiornamento dello storico avviene per tutte le cnv nelle date specificate, aventi un appuntamento che non sia già stato eliminato.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dateConvocazione"></param>
    ''' <param name="dataOperazione"></param>
    ''' <param name="idMotivoEliminazione"></param>
    ''' <param name="noteEliminazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdateStoricoAppuntamenti_Eliminazione(codicePaziente As Long, dateConvocazione As IEnumerable(Of DateTime), dataOperazione As DateTime?, idMotivoEliminazione As String, noteEliminazione As String, noteUtenteModifica As String) As Integer

        If dateConvocazione.IsNullOrEmpty() Then
            Throw New ArgumentNullException("Update Storico Appuntamenti - Eliminazione: valorizzare le date di convocazione!")
        End If

        Dim count As Integer = 0

        Dim dataEliminazione As DateTime = DateTime.Now
        If dataOperazione.HasValue Then dataEliminazione = dataOperazione

        For Each dataConvocazione As DateTime In dateConvocazione

            UpdateStoricoAppuntamenti_Eliminazione(codicePaziente, dataConvocazione, dataEliminazione, idMotivoEliminazione, noteEliminazione, noteUtenteModifica)

        Next

        Return count

    End Function

    Private Function UpdateStoricoAppuntamenti_Eliminazione(codicePaziente As Long, dataConvocazione As DateTime, dataEliminazione As DateTime, idMotivoEliminazione As String, noteEliminazione As String, noteUtenteModifica As String) As Integer

        Dim count As Integer = 0

        ' Appuntamento eliminato => update in storico appuntamenti (più recente)
        Dim convocazioneAppuntamento As Entities.ConvocazioneAppuntamento =
            Me.GenericProvider.Prenotazioni.GetLastStoricoAppuntamenti(codicePaziente, dataConvocazione)

        ' Se è stato già registrato uno storico e non ha la data di eliminazione dell'appuntamento => update dati di eliminazione appuntamento
        If Not convocazioneAppuntamento Is Nothing AndAlso Not convocazioneAppuntamento.DataEliminazioneAppuntamento.HasValue Then

            noteEliminazione = GetNoteStoricoAppuntamenti(convocazioneAppuntamento.Note + " - " + noteEliminazione)

            count += Me.GenericProvider.Prenotazioni.UpdateStoricoAppuntamenti_Eliminazione(
                convocazioneAppuntamento.Id, Me.ContextInfos.IDUtente, dataEliminazione, idMotivoEliminazione, noteEliminazione, noteUtenteModifica)

        End If

        Return count

    End Function

    Private Function GetNoteStoricoAppuntamenti(note As String) As String

        Dim maxLength As Integer = 1000

        If note.Length > maxLength Then
            note = String.Format("[...] {0}", note.Substring(note.Length - maxLength + 6)) ' i 6 caratteri servono per aggiungere i puntini iniziali e stare nei 1000
        End If

        Return note

    End Function

    ''' <summary>
    ''' Restituisce un oggetto ConvocazioneAppuntamento valorizzato in base all'oggetto Convocazione specificato.
    ''' </summary>
    ''' <param name="convocazione"></param>
    ''' <param name="noteAppuntamento"></param>
    ''' <param name="dataOperazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateConvocazioneAppuntamento(convocazione As Entities.Convocazione, noteAppuntamento As String, dataOperazione As DateTime?) As Entities.ConvocazioneAppuntamento

        Dim convocazioneAppuntamento As New Entities.ConvocazioneAppuntamento()
        convocazioneAppuntamento.CodicePaziente = convocazione.Paz_codice
        convocazioneAppuntamento.DataConvocazione = convocazione.Data_CNV
        convocazioneAppuntamento.CodiceConsultorio = convocazione.Cns_Codice
        convocazioneAppuntamento.CodiceAmbulatorio = convocazione.CodiceAmbulatorio
        convocazioneAppuntamento.DurataAppuntamento = convocazione.Durata_Appuntamento
        convocazioneAppuntamento.Note = noteAppuntamento

        If convocazione.DataInvio > DateTime.MinValue Then
            convocazioneAppuntamento.DataInvio = convocazione.DataInvio
            convocazioneAppuntamento.IdUtenteInvio = Me.ContextInfos.IDUtente
        Else
            convocazioneAppuntamento.DataInvio = Nothing
            convocazioneAppuntamento.IdUtenteInvio = Nothing
        End If

        If Not dataOperazione.HasValue Then dataOperazione = DateTime.Now

        If convocazione.DataAppuntamento > DateTime.MinValue Then
            convocazioneAppuntamento.DataAppuntamento = convocazione.DataAppuntamento
            convocazioneAppuntamento.DataRegistrazioneAppuntamento = dataOperazione.Value
            convocazioneAppuntamento.IdUtenteRegistrazioneAppuntamento = Me.ContextInfos.IDUtente
            If String.IsNullOrWhiteSpace(convocazione.TipoAppuntamento) Then
                convocazioneAppuntamento.TipoAppuntamento = Constants.TipoPrenotazioneAppuntamento.Automatica
            Else
                convocazioneAppuntamento.TipoAppuntamento = convocazione.TipoAppuntamento
            End If
        Else
            convocazioneAppuntamento.DataAppuntamento = Nothing
            convocazioneAppuntamento.DataRegistrazioneAppuntamento = Nothing
            convocazioneAppuntamento.IdUtenteRegistrazioneAppuntamento = Nothing
            convocazioneAppuntamento.TipoAppuntamento = Nothing
        End If

        convocazioneAppuntamento.DataEliminazioneAppuntamento = Nothing
        convocazioneAppuntamento.IdUtenteEliminazioneAppuntamento = Nothing
        convocazioneAppuntamento.IdMotivoEliminazioneAppuntamento = Nothing

        convocazioneAppuntamento.Vaccinazioni = GetVaccinazioniConvocazioneAppuntamento(convocazione.Paz_codice, convocazione.Data_CNV)

        Return convocazioneAppuntamento

    End Function

    ''' <summary>
    ''' Restituisce un oggetto ConvocazioneAppuntamento con i dati impostati per l'eliminazione dei dati di appuntamento della convocazione.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataConvocazione"></param>
    ''' <param name="durataAppuntamento"></param>
    ''' <param name="dataEliminazione"></param>
    ''' <param name="idMotivoEliminazione"></param>
    ''' <param name="noteEliminazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateConvocazioneAppuntamentoToDelete(codicePaziente As Long, dataConvocazione As DateTime, durataAppuntamento As Integer?, dataEliminazione As DateTime, idMotivoEliminazione As String, noteEliminazione As String) As Entities.ConvocazioneAppuntamento

        ' Nella convocazione c'è solo questo bilancio: cancellazione dati di appuntamento e invio.
        Dim convocazioneAppuntamento As New Entities.ConvocazioneAppuntamento()
        convocazioneAppuntamento.CodicePaziente = codicePaziente
        convocazioneAppuntamento.DataConvocazione = dataConvocazione
        convocazioneAppuntamento.CodiceConsultorio = Nothing
        convocazioneAppuntamento.CodiceAmbulatorio = Nothing
        convocazioneAppuntamento.DataInvio = Nothing
        convocazioneAppuntamento.IdUtenteInvio = Nothing
        convocazioneAppuntamento.DataAppuntamento = Nothing
        convocazioneAppuntamento.DataRegistrazioneAppuntamento = Nothing
        convocazioneAppuntamento.IdUtenteRegistrazioneAppuntamento = Nothing
        convocazioneAppuntamento.TipoAppuntamento = Nothing
        convocazioneAppuntamento.DurataAppuntamento = durataAppuntamento

        convocazioneAppuntamento.DataEliminazioneAppuntamento = dataEliminazione
        convocazioneAppuntamento.IdUtenteEliminazioneAppuntamento = Me.ContextInfos.IDUtente
        convocazioneAppuntamento.IdMotivoEliminazioneAppuntamento = idMotivoEliminazione
        convocazioneAppuntamento.Note = noteEliminazione

        Return convocazioneAppuntamento

    End Function

    Public Class GetStoricoAppuntamentiCommand
        Public CodicePaziente As Long
        Public DataConvocazione As DateTime?
        Public PageIndex As Integer
        Public PageSize As Integer
        Public CampoOrdinamento As String
        Public VersoOrdinamento As String
    End Class

    Public Class GetStoricoAppuntamentiResult
        Public CountStoricoAppuntamenti As Integer
        Public ListStoricoAppuntamenti As List(Of Entities.StoricoAppuntamento)
    End Class

    ''' <summary>
    ''' Restituisce una lista di oggetti StoricoAppuntamenti, per il paziente specificato.
    ''' </summary>
    ''' <param name="command"></param>
    ''' <returns></returns>
    Public Function GetStoricoAppuntamenti(command As GetStoricoAppuntamentiCommand) As GetStoricoAppuntamentiResult

        Dim result As New GetStoricoAppuntamentiResult()

        result.CountStoricoAppuntamenti = GenericProvider.Prenotazioni.CountStoricoAppuntamenti(command.CodicePaziente, command.DataConvocazione)

        If result.CountStoricoAppuntamenti = 0 Then

            result.ListStoricoAppuntamenti = New List(Of Entities.StoricoAppuntamento)()

        Else

            Dim orderBy As String = GetOrderByStoricoAppuntamenti(command.CampoOrdinamento, command.VersoOrdinamento)

            Dim pagingOptions As New Onit.OnAssistnet.Data.PagingOptions()
            pagingOptions.PageIndex = command.PageIndex
            pagingOptions.PageSize = command.PageSize
            pagingOptions.StartRecordIndex = command.PageIndex * command.PageSize
            pagingOptions.EndRecordIndex = pagingOptions.StartRecordIndex + command.PageSize

            result.ListStoricoAppuntamenti =
                GenericProvider.Prenotazioni.GetStoricoAppuntamenti(command.CodicePaziente, command.DataConvocazione, orderBy, pagingOptions)

        End If

        Return result

    End Function

    Private Function GetOrderByStoricoAppuntamenti(campoOrdinamento As String, versoOrdinamento As String) As String

        ' Ordinamento di default
        Dim defaultOrder As String = " cna_cnv_data desc, cna_id desc"

        Dim orderBy As String = String.Empty

        ' Ordinamento selezionato dall'utente
        If Not String.IsNullOrWhiteSpace(campoOrdinamento) Then

            'Aggiornamento campoOrdinamento
            Select Case campoOrdinamento
                Case "Id"
                    orderBy = String.Format("cna_id {0}", versoOrdinamento)
                Case "DataConvocazione"
                    orderBy = String.Format("cna_cnv_data {0}, cna_id desc", versoOrdinamento)
                Case "Vaccinazioni"
                    orderBy = String.Format("cna_vaccinazioni {0}, ", versoOrdinamento) + defaultOrder
                Case "CodiceConsultorio"
                    orderBy = String.Format("cna_cns_codice {0}, ", versoOrdinamento) + defaultOrder
                Case "Ambulatorio"
                    orderBy = String.Format("cna_cns_codice {0}, ", versoOrdinamento) + defaultOrder
                Case "DescrizioneAmbulatorio"
                    orderBy = String.Format("amb_descrizione {0}, ", versoOrdinamento) + defaultOrder
                Case "DataAppuntamento"
                    orderBy = String.Format("cna_data_appuntamento {0}, ", versoOrdinamento) + defaultOrder
                Case "DataRegistrazioneAppuntamento"
                    orderBy = String.Format("cna_data_registrazione_app {0}, ", versoOrdinamento) + defaultOrder
                Case "UtenteRegistrazioneAppuntamento"
                    orderBy = String.Format("ute_codice_reg {0}, ", versoOrdinamento) + defaultOrder
                Case "TipoAppuntamento"
                    orderBy = String.Format("cna_tipo_appuntamento {0}, ", versoOrdinamento) + defaultOrder
                Case "DataInvio"
                    orderBy = String.Format("cna_data_invio {0}, ", versoOrdinamento) + defaultOrder
                Case "DataEliminazioneAppuntamento"
                    orderBy = String.Format("cna_data_eliminazione {0}, ", versoOrdinamento) + defaultOrder
                Case "UtenteEliminazioneAppuntamento"
                    orderBy = String.Format("ute_codice_del {0}, ", versoOrdinamento) + defaultOrder
                Case "MotivoEliminazioneAppuntamento"
                    orderBy = String.Format("mea_descrizione {0}, ", versoOrdinamento) + defaultOrder
                Case "Note"
                    orderBy = String.Format("cna_note {0}, ", versoOrdinamento) + defaultOrder
                Case "NoteAvvisi"
                    orderBy = String.Format("cna_note_avvisi {0}, cna_note_modifica_appuntamento {0}, ", versoOrdinamento) + defaultOrder
            End Select

        End If

        If String.IsNullOrWhiteSpace(orderBy) Then orderBy = defaultOrder

        Return orderBy

    End Function

#End Region

#Region " Private "

    ''' <summary>
    ''' Restituisce una stringa contenente tutte le vaccinazioni e le relative dosi, per la convocazione specificata.
    ''' La stringa è nel formato: "codiceVaccinazione1 (dose1), codiceVaccinazione2 (dose2), ... "
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataConvocazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetVaccinazioniConvocazioneAppuntamento(codicePaziente As Long, dataConvocazione As DateTime) As String

        Dim vaccinazioni As String = String.Empty

        Dim list As List(Of KeyValuePair(Of String, Integer)) =
            Me.GenericProvider.VaccinazioneProg.GetVacProgNotEseguiteAndNotEscluseListCodiceDose(codicePaziente, dataConvocazione)

        If Not list.IsNullOrEmpty() Then
            vaccinazioni = String.Join(", ", list.Select(Function(item) String.Format("{0} ({1})", item.Key, item.Value)))
        End If

        Return vaccinazioni

    End Function

#End Region

#Region " Types "

    <Serializable()>
    Public Class DatiConvocazioneScartata

        Public Sub New()
            Me.CodiceVaccinazioneList = New List(Of String)()
        End Sub

        Public Property CodicePaziente() As Integer
        Public Property DataConvocazione() As Date
        Public Property CodiceCiclo() As String
        Public Property NumeroSeduta() As Integer
        Public Property Intervallo() As Integer

        ' Motivo per cui la cnv è stata scartata dal calcolo
        Public Property CodiceMotivo() As CodiceMotivoConvocazioneScartata

        ' Motivo per cui la cnv è stata scartata dal calcolo
        Public Property DescrizioneMotivo() As String

        Public Property CodiceVaccinazioneList() As List(Of String)

    End Class

    ''' <summary>
    ''' Elenco dei possibili motivi per cui vengono scartate le convocazioni.
    ''' Utilizzato anche nella libreria OnVac.CalcoloConvocazioni.
    ''' </summary>
    Public Enum CodiceMotivoConvocazioneScartata
        NonAssociabilitaIntervallo = 0
        NonAssociabilitaDataCnv = 1
        StesseVaccinazioniInDataUnione = 2
        NonAssociabilitaInDataUnione = 3
        CampagnaVaccinazioneProgrammata = 4
        CampagnaVaccinazioneEsclusa = 5
        VaccinazioniProgrammateAssenti = 6
        CampagnaConvocazioneEsistenteInStessaDataAltroCns = 7
    End Enum

#Region " Results "

    Public Class SpostaConvocazioneBizResult
        Inherits BizResult

        Public ReadOnly DatiConvocazioneScartata() As BizConvocazione.DatiConvocazioneScartata

        Sub New()
            MyBase.New()
        End Sub

        Sub New(success As Boolean, datiConvocazioniScartate As DatiConvocazioneScartata())

            MyBase.New(success, datiConvocazioniScartate.Select(Function(cs) cs.DescrizioneMotivo), Nothing)

            Me.DatiConvocazioneScartata = datiConvocazioniScartate

        End Sub

    End Class

#End Region

#End Region

End Class
