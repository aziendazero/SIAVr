Imports Onit.OnAssistnet.OnVac.Collection
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.DAL
Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.Log
Imports Onit.OnAssistnet.OnVac.Enumerators

Public Class BizRicercaAppuntamenti
    Inherits BizClass

#Region " Costruttori "


    Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)

        MyBase.New(genericprovider, settings, contextInfos, logOptions)

    End Sub
#End Region

#Region " Public "
    Public Function GetIdConvocazione(pazCodice As Long, dataConv As DateTime) As String

        Return GenericProvider.Convocazione.GetIdConvocazione(pazCodice, dataConv)

    End Function
    Public Function CheckAppuntamento(nuovaData As String, durataAppuntamento As String, ambCodice As Integer, orario As String) As String

        Dim result As String = String.Empty

        Dim bolVal As Boolean =
            Me.GenericProvider.Prenotazioni.OrarioCompresoInOrarioAperturaAmb(nuovaData, durataAppuntamento, ambCodice, orario)

        If Not bolVal Then
            result = "L'orario selezionato è al di fuori dell'orario di apertura del centro vaccinale nel giorno selezionato"
        End If

        bolVal = GenericProvider.Prenotazioni.AppuntamentoSovrapposto(nuovaData, durataAppuntamento, ambCodice, orario)

        If bolVal Then
            result = "L'orario selezionato si sovrappone con quello di un appuntamento già fissato!!!!"
        End If

        Return result

    End Function

    Public Function PazienteSenzaPediatra(pazCodice As Integer, dataCnv As Date) As Boolean

        Using bizBil As New BizBilancioProgrammato(Me.GenericProvider, Me.Settings, Me.ContextInfos, Me.LogOptions)

            Dim senzaPediatra As Boolean = Me.GenericProvider.Paziente.IsSenzaPediatra(pazCodice)

            Dim dtBil As DataTable = bizBil.CercaBilanciDt(pazCodice, dataCnv)

            If dtBil.Rows.Count <> 0 And senzaPediatra Then

                For i As Integer = 0 To dtBil.Rows.Count - 1
                    If Not (dtBil.Rows(i)("BIP_BIL_NUMERO") Is DBNull.Value Or dtBil.Rows(i)("BIP_BIL_NUMERO") = 0) Then
                        Return True
                    End If
                Next

            End If

        End Using

        Return False

    End Function

    ''' <summary>
    ''' ' Classe contenente tutti i parametri che verranno utilizzati dal consultorio di prenotazione
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ParametriConsultorioAppuntamento
        Public SedManu As Integer
        Public TempoBil As Integer
        Public TempoSed As Integer
    End Class

    Public Function GetParametriConsultorioAppuntamento(codiceConsultorioAppuntamento As String) As ParametriConsultorioAppuntamento

        ' Codici dei parametri che verranno restituiti
        Dim parametro_SED_MANU As String = [Enum].GetName(GetType(Onit.OnAssistnet.OnVac.Settings.Settings.SettingName), Onit.OnAssistnet.OnVac.Settings.Settings.SettingName.SED_MANU)
        Dim parametro_TEMPOBIL As String = [Enum].GetName(GetType(Onit.OnAssistnet.OnVac.Settings.Settings.SettingName), Onit.OnAssistnet.OnVac.Settings.Settings.SettingName.TEMPOBIL)
        Dim parametro_TEMPOSED As String = [Enum].GetName(GetType(Onit.OnAssistnet.OnVac.Settings.Settings.SettingName), Onit.OnAssistnet.OnVac.Settings.Settings.SettingName.TEMPOSED)

        Dim listCodiciParametri As New List(Of String)()
        listCodiciParametri.Add(parametro_SED_MANU)
        listCodiciParametri.Add(parametro_TEMPOBIL)
        listCodiciParametri.Add(parametro_TEMPOSED)

        ' Caricamento parametri
        Dim listParametri As List(Of KeyValuePair(Of String, Object)) =
            Me.GenericProvider.Parametri.GetParametriCns(codiceConsultorioAppuntamento, listCodiciParametri)

        Dim parametriConsultorioPrenotazione As New ParametriConsultorioAppuntamento()
        parametriConsultorioPrenotazione.SedManu = Me.GetValoreParametro(parametro_SED_MANU, listParametri)
        parametriConsultorioPrenotazione.TempoBil = Me.GetValoreParametro(parametro_TEMPOBIL, listParametri)
        parametriConsultorioPrenotazione.TempoSed = Me.GetValoreParametro(parametro_TEMPOSED, listParametri)

        Return parametriConsultorioPrenotazione

    End Function

    Public Function GetFiltriApertura(codiceAmbulatorio As Integer, fuoriOrario As Boolean, giorno As DayOfWeek) As DataTable

        Return GenericProvider.Prenotazioni.GetFiltriApertura(codiceAmbulatorio, fuoriOrario, giorno)

    End Function

    Public Function ExistsDataAppuntamento(codicePaziente As Long, dataConvocazione As DateTime) As Boolean

        Dim dataAppuntamento As DateTime? = Me.GenericProvider.Prenotazioni.GetDataAppuntamento(codicePaziente, dataConvocazione)

        Return dataAppuntamento.HasValue

    End Function

    <Serializable>
    Public Class MotivoEliminazioneAppuntamento
        Public Property Codice As String
        Public Property Descrizione As String
    End Class

    Public Function GetMotiviEliminazioneAppuntamento() As List(Of MotivoEliminazioneAppuntamento)

        Dim list As List(Of KeyValuePair(Of String, String)) = Me.GenericProvider.Prenotazioni.GetMotiviEliminazioneAppuntamento()

        Dim listMotivi As New List(Of MotivoEliminazioneAppuntamento)()

        If Not list.IsNullOrEmpty() Then

            For Each item As KeyValuePair(Of String, String) In list

                Dim motivo As New MotivoEliminazioneAppuntamento()
                motivo.Codice = item.Key
                motivo.Descrizione = item.Value

                listMotivi.Add(motivo)
            Next

        End If

        Return listMotivi

    End Function


    Public Function CercaPeriodiLiberi(command As CercaPeriodiLiberiCommand) As CercaPeriodiLiberiResult

        'If command.CodicePaziente <= 0 Then
        '    Return New CercaPeriodiLiberiResult() With {.Success = False, .Message = "Paziente non specificato"}
        'End If

        If command.DurataAppuntamento < 0 Then
            Return New CercaPeriodiLiberiResult() With {.Success = False, .Message = "La durata dell'appuntamento deve essere maggiore di zero"}
        End If

        If command.DataInizio = Date.MinValue OrElse command.DataFine = Date.MinValue Then
            Return New CercaPeriodiLiberiResult() With {.Success = False, .Message = "Specificare le date di inizio e fine del periodo di appuntamento"}
        End If

        If command.DataFine < command.DataInizio Then
            Return New CercaPeriodiLiberiResult() With {.Success = False, .Message = "La data di fine del periodo di appuntamento non può essere inferiore alla data di inizio"}
        End If

        Dim giorniDisponibili As New List(Of DateTime)()
        Dim dtaAppuntamentiAmbulatorio As New DataTable()
        Dim listAppuntamentiTotali As New List(Of AppuntamentoPrenotato)()

        Dim dtaOrariValidi As New DataTable()
        dtaOrariValidi.Columns.Add("Data", GetType(DateTime))
        dtaOrariValidi.Columns.Add("Ora", GetType(DateTime))
        dtaOrariValidi.Columns.Add("ambCodice", GetType(Integer))
        dtaOrariValidi.Columns.Add("Amb", GetType(String))
        dtaOrariValidi.Columns.Add("Vaccinazioni", GetType(String))

        'Dim paz As New CalcoloAppuntamenti.CnvPaziente(command.CodicePaziente, Date.MinValue, False, False)

        Dim consultorioCalcoloAppuntamenti As New CalcoloAppuntamenti.Consultorio()
        consultorioCalcoloAppuntamenti.DaData = command.DataInizio
        consultorioCalcoloAppuntamenti.AData = command.DataFine

        If command.CodiceAmbulatorio <> 0 Then
            SetAmbulatorioCalcoloAppuntamenti(command.CodiceAmbulatorio, command.DescrizioneAmbulatorio, consultorioCalcoloAppuntamenti)
        Else
            SetListAmbulatoriCalcoloAppuntamenti(command.CodiceConsultorio, consultorioCalcoloAppuntamenti)
        End If
        '--
        '[Imposto i filtri per ciascun ambulatorio]
        '--
        For Each amb As CalcoloAppuntamenti.Ambulatorio In consultorioCalcoloAppuntamenti.Ambulatori

            Dim codiceAmbulatorioCorrente As Integer = Convert.ToInt32(amb.Codice)

            dtaAppuntamentiAmbulatorio.Rows.Clear()
            dtaAppuntamentiAmbulatorio = GenericProvider.Prenotazioni.GetPrenotati(codiceAmbulatorioCorrente, command.DataInizio, command.DataFine.AddDays(1))

            If Not dtaAppuntamentiAmbulatorio Is Nothing AndAlso dtaAppuntamentiAmbulatorio.Rows.Count > 0 Then

                Dim list As List(Of AppuntamentoPrenotato) = dtaAppuntamentiAmbulatorio.ConvertToList(Of AppuntamentoPrenotato)()

                If Not list Is Nothing AndAlso list.Count > 0 Then
                    listAppuntamentiTotali.AddRange(list)
                End If

            End If

            Dim filtriOcc As CalcoloAppuntamenti.Filtri.FiltroCollection = RestituisciPrenotati(dtaAppuntamentiAmbulatorio)

            For Each f As CalcoloAppuntamenti.Filtri.IFiltro In filtriOcc
                amb.FiltriAmbulatorio.Add(f)
            Next

            Dim filtriOrari As CalcoloAppuntamenti.Filtri.FiltroCollection = CaricaFiltriApertura(codiceAmbulatorioCorrente, False, -1, PartiGiornata.Entrambi)

            For Each f As CalcoloAppuntamenti.Filtri.IFiltro In filtriOrari
                amb.FiltriAmbulatorio.Add(f)
            Next

            Dim filtriFesta As CalcoloAppuntamenti.Filtri.FiltroCollection = CaricaFiltriFeste(codiceAmbulatorioCorrente)

            For Each f As CalcoloAppuntamenti.Filtri.IFiltro In filtriFesta
                amb.FiltriAmbulatorio.Add(f)
            Next

        Next

        For Each amb As CalcoloAppuntamenti.Ambulatorio In consultorioCalcoloAppuntamenti.Ambulatori

            Dim codiceAmbulatorioCorrente As Integer = Convert.ToInt32(amb.Codice)

            'If consultorioCalcoloAppuntamenti.Pazienti.Count > 0 Then
            '    consultorioCalcoloAppuntamenti.Pazienti.Remove(paz)
            'End If

            'paz.Ambulatorio = codiceAmbulatorioCorrente

            'consultorioCalcoloAppuntamenti.Pazienti.Add(paz)
            '--
            '27/08/2008 MGR se utilizzo la ricerca appuntamenti della classe consultorio per gli ambulatori successivi al primo
            'vengono duplicati i record del singolo orario tante volte quanto è l'indice dell'ambulatorio in esame, qs perchè la funzione
            'FindAppuntamenti in qs classe ha a sua volta un ciclo for per tutti gli ambulatori.
            'utilizzando la funzione nella classe Ambulatorio qs problema non si verifica.
            'calcoloAppuntamenti.FindAppuntamenti()
            '--
            amb.FindAppuntamenti()

            ' N.B. : CaricaGiorniIndisponibilita => serve solo in maschera per popolare il datatable delle indisponibilità
            ' CaricaGiorniIndisponibilita(codiceAmbulatorioCorrente)

            If Not amb.Giorni Is Nothing AndAlso amb.Giorni.Count > 0 Then
                For Each d As CalcoloAppuntamenti.Timing.Day In amb.Giorni
                    For Each t As CalcoloAppuntamenti.Timing.TimeBlock In d.TimeBlocks
                        If t.IsFree(0) Then

                            Dim oraInizio As Date = t.Inizio
                            Dim orafine As Date = t.Fine

                            Dim numAppLiberi As Double = Math.Floor((orafine.TimeOfDay.TotalMinutes - oraInizio.TimeOfDay.TotalMinutes) / command.DurataAppuntamento)

                            For i As Integer = 0 To numAppLiberi - 1

                                If Not giorniDisponibili.Contains(d.Giorno) Then
                                    giorniDisponibili.Add(d.Giorno)
                                End If

                                Dim r As DataRow = dtaOrariValidi.NewRow()

                                r("Data") = d.Giorno
                                r("Ora") = oraInizio.AddMinutes(i * command.DurataAppuntamento)
                                r("ambCodice") = codiceAmbulatorioCorrente
                                r("Amb") = amb.Descrizione
                                r("Vaccinazioni") = String.Empty

                                dtaOrariValidi.Rows.Add(r)

                            Next
                        End If
                    Next
                Next
            End If
        Next

        If giorniDisponibili.Count > 0 Then
            giorniDisponibili.Sort()
        End If

        'Gestione visualizzazione prenotazioni
        If command.MostraPrenotazioni AndAlso listAppuntamentiTotali IsNot Nothing Then

            For Each item As AppuntamentoPrenotato In listAppuntamentiTotali

                Dim rov As DataRow = dtaOrariValidi.NewRow()
                rov("Data") = item.CNV_DATA_APPUNTAMENTO.Date
                rov("Ora") = New DateTime(1, 1, 1, item.CNV_DATA_APPUNTAMENTO.Hour, item.CNV_DATA_APPUNTAMENTO.Minute, item.CNV_DATA_APPUNTAMENTO.Second)
                rov("ambCodice") = item.AMB_CODICE.ToString()
                rov("Amb") = item.AMB_DES
                rov("Vaccinazioni") = IIf(Not String.IsNullOrWhiteSpace(item.VACCINAZIONI), item.VACCINAZIONI, "Solo bilancio")

                dtaOrariValidi.Rows.Add(rov)

            Next

        End If

        Dim result As New CercaPeriodiLiberiResult()
        result.Appuntamenti = dtaAppuntamentiAmbulatorio
        result.GiorniDisponibili = giorniDisponibili
        result.OrariValidi = dtaOrariValidi
        result.AppuntamentiTotali = listAppuntamentiTotali
        result.Success = True
        result.Message = String.Empty

        Return result

    End Function

    Public Function SalvaAppuntamento(command As SalvaAppuntamentiCommand, inviaNotifica As Boolean) As SalvaAppuntamentiResult

        Dim codiceFiscale As String = GenericProvider.Paziente.GetDatiAnagraficiPaziente(command.CodicePaziente).CodiceFiscale

        If String.IsNullOrWhiteSpace(ContextInfos.CodiceCentroVaccinale) Then
            Throw New ArgumentNullException("Codice centro vaccinale mancante")
        End If

        Dim logOptionsAppuntamenti As BizLogOptions = LogOptions

        If logOptionsAppuntamenti Is Nothing Then
            logOptionsAppuntamenti = New BizLogOptions(DataLogStructure.TipiArgomento.APPUNTAMENTO, False)
        End If

        Dim result As New SalvaAppuntamentiResult()
        result.AppuntamentoSpostato = False

        ' campi utili per PhoneBar
        Dim tipoOperazione As New OperazioneCallCenter
        Dim hasException As Boolean = False
        Dim logNote As String = String.Empty

        Try
            Dim paziente As PazienteDatiAnagrafici = GenericProvider.Paziente.GetDatiAnagraficiPaziente(command.CodicePaziente)
            result.CodiceFiscale = paziente.CodiceFiscale

            GenericProvider.BeginTransaction()

            Dim dataOperazione As DateTime = DateTime.Now

            If command.EliminaBilancio Then

                Dim convocazioneDaEliminare As Convocazione = Nothing
                Dim deleted As Boolean = False

                Using bizConvocazione As New BizConvocazione(GenericProvider, Settings, ContextInfos, Nothing)

                    If LogBox.IsEnabled Then
                        convocazioneDaEliminare = bizConvocazione.GetConvocazione(command.CodicePaziente, command.DataConvocazione)
                    End If

                    Dim eliminaCnvCommand As New BizConvocazione.EliminaConvocazioniSollecitiBilanciCommand()
                    eliminaCnvCommand.CodicePaziente = command.CodicePaziente
                    eliminaCnvCommand.DataConvocazione = command.DataConvocazione
                    eliminaCnvCommand.CancellaBilanciAssociati = True
                    eliminaCnvCommand.DataEliminazione = dataOperazione
                    eliminaCnvCommand.IdMotivoEliminazione = command.IdMotivoEliminazioneAppuntamento
                    eliminaCnvCommand.NoteEliminazione = command.NoteAppuntamento
                    eliminaCnvCommand.WriteLog = False

                    deleted = bizConvocazione.EliminaConvocazioniSollecitiBilanci(eliminaCnvCommand)

                End Using

                If deleted AndAlso LogBox.IsEnabled Then
                    LogBox.WriteData(GetTestataLogEliminazioneConvocazioneBilancio(convocazioneDaEliminare, logOptionsAppuntamenti))
                End If

            Else

                Dim convocazioneOriginale As Convocazione = GenericProvider.Convocazione.GetConvocazionePaziente(command.CodicePaziente, command.DataConvocazione)
                'se c'era già una data è uno spostamento se no è una prenotazione

                Dim convocazioneAppuntamento As New ConvocazioneAppuntamento()
                convocazioneAppuntamento.CodicePaziente = command.CodicePaziente
                convocazioneAppuntamento.DataConvocazione = command.DataConvocazione
                convocazioneAppuntamento.CodiceConsultorio = command.CodiceConsultorioAppuntamento
                convocazioneAppuntamento.Note = command.NoteAppuntamento
                convocazioneAppuntamento.DataInvio = Nothing
                convocazioneAppuntamento.IdUtenteInvio = Nothing

                If command.DataAppuntamento.HasValue Then
                    '---
                    ' Update dati appuntamento
                    '---

                    If convocazioneOriginale.DataAppuntamento > Date.MinValue Then
                        'è un update
                        tipoOperazione = OperazioneCallCenter.SpostamentoAppuntamento



                    Else
                        'è un primo inserimento
                        tipoOperazione = OperazioneCallCenter.PrenotazioneAppuntamento



                    End If

                    convocazioneAppuntamento.DataAppuntamento = command.DataAppuntamento.Value
                    convocazioneAppuntamento.DataRegistrazioneAppuntamento = dataOperazione
                    convocazioneAppuntamento.IdUtenteRegistrazioneAppuntamento = ContextInfos.IDUtente

                    convocazioneAppuntamento.DataEliminazioneAppuntamento = Nothing
                    convocazioneAppuntamento.IdUtenteEliminazioneAppuntamento = Nothing
                    convocazioneAppuntamento.IdMotivoEliminazioneAppuntamento = Nothing

                    convocazioneAppuntamento.CodiceAmbulatorio = command.CodiceAmbulatorio
                    convocazioneAppuntamento.TipoAppuntamento = Constants.TipoPrenotazioneAppuntamento.ManualeDaRicercaAppuntamenti

                    convocazioneAppuntamento.DurataAppuntamento = command.DurataAppuntamento

                    convocazioneAppuntamento.NoteAvvisi = String.Empty
                    convocazioneAppuntamento.NoteModificaAppuntamento = command.NoteUtenteModificaAppuntamento

                    Dim list As List(Of KeyValuePair(Of String, Integer)) =
                    GenericProvider.VaccinazioneProg.GetVacProgNotEseguiteAndNotEscluseListCodiceDose(command.CodicePaziente, command.DataConvocazione)

                    If Not list.IsNullOrEmpty Then
                        convocazioneAppuntamento.Vaccinazioni = String.Join(", ", list.Select(Function(item) String.Format("{0} ({1})", item.Key, item.Value)))
                    End If

                Else
                    '---
                    ' Cancellazione dati di appuntamento
                    '---
                    tipoOperazione = OperazioneCallCenter.EliminazioneAppuntamento

                    convocazioneAppuntamento.DataAppuntamento = Nothing
                    convocazioneAppuntamento.DataRegistrazioneAppuntamento = Nothing
                    convocazioneAppuntamento.IdUtenteRegistrazioneAppuntamento = Nothing

                    convocazioneAppuntamento.DataEliminazioneAppuntamento = dataOperazione
                    convocazioneAppuntamento.IdUtenteEliminazioneAppuntamento = ContextInfos.IDUtente
                    convocazioneAppuntamento.IdMotivoEliminazioneAppuntamento = command.IdMotivoEliminazioneAppuntamento
                    convocazioneAppuntamento.NoteAvvisi = String.Empty
                    convocazioneAppuntamento.NoteModificaAppuntamento = command.NoteUtenteModificaAppuntamento

                    convocazioneAppuntamento.CodiceAmbulatorio = Nothing
                    convocazioneAppuntamento.TipoAppuntamento = Nothing

                    If command.IsSoloBilancio Then

                        ' Solo bilancio => durata 0
                        convocazioneAppuntamento.DurataAppuntamento = 0

                    Else

                        ' Durata di default in base al consultorio
                        Dim cnsAppuntamento_TEMPOSED As Integer = Settings.TEMPOSED

                        If command.CodiceConsultorioAppuntamento <> ContextInfos.CodiceCentroVaccinale Then

                            ' Caricamento parametro Me.Settings.TEMPOSED in base al consultorio in cui viene dato l'appuntamento
                            Dim parametriConsultorioAppuntamento As BizRicercaAppuntamenti.ParametriConsultorioAppuntamento =
                            GetParametriConsultorioAppuntamento(command.CodiceConsultorioAppuntamento)

                            cnsAppuntamento_TEMPOSED = parametriConsultorioAppuntamento.TempoSed

                        End If

                        convocazioneAppuntamento.DurataAppuntamento = cnsAppuntamento_TEMPOSED

                    End If
                End If

                ' Update convocazione e storico appuntamenti
                Dim count As Integer = 0

                Using bizConvocazione As New BizConvocazione(GenericProvider, Settings, ContextInfos, LogOptions)

                    count = bizConvocazione.UpdateConvocazione_DatiAppuntamento(convocazioneAppuntamento, command.IdMotivoEliminazioneAppuntamento)

                End Using

                If count > 0 Then

                    If command.CodiceConsultorioAppuntamento <> ContextInfos.CodiceCentroVaccinale Then
                        result.AppuntamentoSpostato = True
                    End If

                    If LogBox.IsEnabled AndAlso count > 0 Then
                        LogBox.WriteData(GetTestataLogModificaConvocazioneAppuntamento(convocazioneAppuntamento, convocazioneOriginale, logOptionsAppuntamenti))
                    End If

                    result.Success = True
                    result.Message = String.Empty

                End If
                If count = 0 Then

                    result.Success = False
                    result.Message = "Operazione non eseguita"

                End If
            End If

            GenericProvider.Commit()

        Catch ex As Exception

            GenericProvider.Rollback()

            result.Success = False
            result.Message = ex.Message

            LogBox.WriteData(LogBox.GetTestataException(ex))

            hasException = True
            logNote = ex.Message & ex.ToString()

        Finally

        End Try

        Return result

    End Function

    Private Sub SetAmbulatorioCalcoloAppuntamenti(codiceAmbulatorio As Integer, descrizioneAmbulatorio As String, consultorioCalcoloAppuntamenti As CalcoloAppuntamenti.Consultorio)

        Dim ambulatorioSelezionato As Ambulatorio = Nothing

        Using bizConsultori As New BizConsultori(GenericProvider, Settings, ContextInfos, LogOptions)

            ambulatorioSelezionato = bizConsultori.GetAmbulatorio(codiceAmbulatorio)

        End Using

        Dim ambulatorioCalcoloAppuntamenti As New CalcoloAppuntamenti.Ambulatorio(codiceAmbulatorio, False, consultorioCalcoloAppuntamenti)

        ambulatorioCalcoloAppuntamenti.Descrizione = ambulatorioSelezionato.Descrizione

        If Not ambulatorioSelezionato Is Nothing Then
            ambulatorioCalcoloAppuntamenti.DataAperturaAmbulatorio = ambulatorioSelezionato.DataApertura
            ambulatorioCalcoloAppuntamenti.DataChiusuraAmbulatorio = ambulatorioSelezionato.DataChiusura
        End If

        consultorioCalcoloAppuntamenti.Ambulatori.Add(ambulatorioCalcoloAppuntamenti)

    End Sub
    Private Sub SetListAmbulatoriCalcoloAppuntamenti(codiceConsultorioSelezionato As String, consultorioCalcoloAppuntamenti As CalcoloAppuntamenti.Consultorio)

        Dim listAmbulatori As List(Of Ambulatorio) = Nothing

        Using bizConsultori As New BizConsultori(GenericProvider, Settings, ContextInfos, LogOptions)

            listAmbulatori = bizConsultori.GetAmbulatori(codiceConsultorioSelezionato, True)

        End Using

        If Not listAmbulatori.IsNullOrEmpty() Then

            For Each ambulatorio As Ambulatorio In listAmbulatori

                Dim ambulatorioCalcoloAppuntamenti As New CalcoloAppuntamenti.Ambulatorio(ambulatorio.Codice, False, consultorioCalcoloAppuntamenti)
                ambulatorioCalcoloAppuntamenti.Descrizione = ambulatorio.Descrizione
                ambulatorioCalcoloAppuntamenti.DataAperturaAmbulatorio = ambulatorio.DataApertura
                ambulatorioCalcoloAppuntamenti.DataChiusuraAmbulatorio = ambulatorio.DataChiusura

                consultorioCalcoloAppuntamenti.Ambulatori.Add(ambulatorioCalcoloAppuntamenti)

            Next

        End If

    End Sub
    Private Function RestituisciPrenotati(dtAppuntamenti As DataTable) As CalcoloAppuntamenti.Filtri.FiltroCollection

        Dim retFilter As New CalcoloAppuntamenti.Filtri.FiltroCollection()

        For Each r As DataRow In dtAppuntamenti.Rows

            Dim f As New CalcoloAppuntamenti.Filtri.FiltroOccupato(r("CNV_DATA_APPUNTAMENTO").date, r("CNV_DATA_APPUNTAMENTO"), r("CNV_DATA_APPUNTAMENTO").addminutes(r("CNV_DURATA_APPUNTAMENTO")))

            retFilter.Add(f)

        Next

        Return retFilter

    End Function
    Private Function CaricaFiltriFeste(ambCodice As Integer) As CalcoloAppuntamenti.Filtri.FiltroCollection

        Dim dta As DataTable = GenericProvider.Prenotazioni.GetFestivita(ambCodice)

        Dim filters As New CalcoloAppuntamenti.Filtri.FiltroCollection()

        For Each r As DataRow In dta.Rows

            Dim f As New CalcoloAppuntamenti.Filtri.FiltroIndisponibilitaGenerica
            Dim data As Date = r("DATA")

            If Not r("RICORSIVITA") Is DBNull.Value AndAlso r("RICORSIVITA").ToString() = OnVac.Constants.CommonConstants.RECURSIVE_YEAR Then
                f.Ricorsivita = CalcoloAppuntamenti.Filtri.Ricorsivita.Annuale
            Else
                f.Ricorsivita = CalcoloAppuntamenti.Filtri.Ricorsivita.Singola
            End If

            f.Anno = data.Year
            f.Mese = data.Month
            f.Giorno = data.Day

            If Not r("INIZIO") Is DBNull.Value Then
                f.DaOra = r("INIZIO")
                f.AOra = r("FINE")
            Else
                f.DaOra = CalcoloAppuntamenti.Timing.Day.MinOrario
                f.AOra = CalcoloAppuntamenti.Timing.Day.MaxOrario
            End If

            filters.Add(f)

        Next

        Return filters

    End Function
    Private Function CaricaFiltriApertura(ambCodice As Integer, fuoriOrario As Boolean, giorno As DayOfWeek, parte As PartiGiornata) As CalcoloAppuntamenti.Filtri.FiltroCollection

        Dim f As New CalcoloAppuntamenti.Filtri.FiltroCollection()

        Dim dta As DataTable = GetFiltriApertura(ambCodice, fuoriOrario, giorno)

        Dim oraPm As Object = Settings.ORAPM
        Dim oraAm As Object = Date.Parse("08:00")
        If oraPm Is Nothing Then oraPm = "13:00"

        oraPm = Date.Parse(oraPm)

        If dta.Rows.Count > 0 Then

            For Each r As DataRow In dta.Rows

                Dim defMat As Boolean = Not r("AM_INIZIO") Is DBNull.Value AndAlso Not r("AM_FINE") Is DBNull.Value
                Dim defPom As Boolean = Not r("PM_INIZIO") Is DBNull.Value AndAlso Not r("PM_FINE") Is DBNull.Value

                If parte = PartiGiornata.Mattina Then
                    If defMat And defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), CalcoloAppuntamenti.Timing.Day.MinOrario, r("AM_INIZIO")))
                        f.Add(CreaOrarioApertura(r("GIORNO"), r("PM_INIZIO"), CalcoloAppuntamenti.Timing.Day.MaxOrario))
                    ElseIf Not defMat AndAlso Not defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), CalcoloAppuntamenti.Timing.Day.MinOrario, oraAm))
                        f.Add(CreaOrarioApertura(r("GIORNO"), oraPm, CalcoloAppuntamenti.Timing.Day.MaxOrario))
                    ElseIf defMat AndAlso Not defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), CalcoloAppuntamenti.Timing.Day.MinOrario, r("AM_INIZIO")))
                        f.Add(CreaOrarioApertura(r("GIORNO"), oraPm, CalcoloAppuntamenti.Timing.Day.MaxOrario))
                    ElseIf Not defMat AndAlso defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), CalcoloAppuntamenti.Timing.Day.MinOrario, oraAm))
                        f.Add(CreaOrarioApertura(r("GIORNO"), r("PM_INIZIO"), CalcoloAppuntamenti.Timing.Day.MaxOrario))
                    End If
                ElseIf parte = PartiGiornata.Pomeriggio Then
                    If defMat And defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), CalcoloAppuntamenti.Timing.Day.MinOrario, r("PM_INIZIO")))
                    ElseIf Not defMat AndAlso Not defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), CalcoloAppuntamenti.Timing.Day.MinOrario, oraPm))
                    ElseIf defMat AndAlso Not defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), CalcoloAppuntamenti.Timing.Day.MinOrario, oraPm))
                    ElseIf Not defMat AndAlso defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), CalcoloAppuntamenti.Timing.Day.MinOrario, r("PM_INIZIO")))
                    End If
                Else
                    If defMat And defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), CalcoloAppuntamenti.Timing.Day.MinOrario, r("AM_INIZIO")))
                        f.Add(CreaOrarioApertura(r("GIORNO"), r("AM_FINE"), r("PM_INIZIO")))
                        f.Add(CreaOrarioApertura(r("GIORNO"), r("PM_FINE"), CalcoloAppuntamenti.Timing.Day.MaxOrario))
                    ElseIf Not defMat AndAlso Not defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), CalcoloAppuntamenti.Timing.Day.MinOrario, CalcoloAppuntamenti.Timing.Day.MaxOrario))
                    ElseIf defMat AndAlso Not defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), CalcoloAppuntamenti.Timing.Day.MinOrario, r("AM_INIZIO")))
                        f.Add(CreaOrarioApertura(r("GIORNO"), r("AM_FINE"), CalcoloAppuntamenti.Timing.Day.MaxOrario))
                    ElseIf Not defMat AndAlso defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), CalcoloAppuntamenti.Timing.Day.MinOrario, r("PM_INIZIO")))
                        f.Add(CreaOrarioApertura(r("GIORNO"), r("PM_FINE"), CalcoloAppuntamenti.Timing.Day.MaxOrario))
                    End If
                End If
            Next
        Else
            ' non sono impostati gli orari di apertura
            For i As Integer = 0 To 6
                f.Add(CreaOrarioApertura(i, CalcoloAppuntamenti.Timing.Day.MinOrario, CalcoloAppuntamenti.Timing.Day.MaxOrario))
            Next
        End If

        Return f

    End Function
    Private Function CreaOrarioApertura(giorno As DayOfWeek, minDate As Date, maxDate As Date) As CalcoloAppuntamenti.Filtri.IFiltro

        Dim fAp As New CalcoloAppuntamenti.Filtri.FiltroIndisponibilitaGenerica()

        fAp.Ricorsivita = CalcoloAppuntamenti.Filtri.Ricorsivita.Settimanale
        fAp.GiornoSettimana = giorno
        fAp.DaOra = minDate
        fAp.AOra = maxDate

        Return fAp

    End Function

    Public Function CreaCnvPerAppuntamentoApp(command As CreaCnvPerAppuntamentoAppCommand) As CreaCnvPerAppuntamentoAppResult

        If command.DataAppuntamento.Date < Date.Now.Date Then
            Return New CreaCnvPerAppuntamentoAppResult() With {
                .Success = False,
                .Message = "La data di appuntamento non può essere precedente rispetto alla data odierna"
            }
        End If

        Dim dataConvocazione As Date = Date.Now.Date

        Dim dataFineSospensione As Date = Date.MinValue

        Using bizVisite As New BizVisite(GenericProvider, Settings, ContextInfos, Nothing)
            dataFineSospensione = bizVisite.GetMaxDataFineSospensione(command.CodicePaziente)
        End Using

        If dataConvocazione.Date < dataFineSospensione.Date Then dataConvocazione = dataFineSospensione.Date

        If command.DataAppuntamento.Date < dataFineSospensione.Date Then
            Return New CreaCnvPerAppuntamentoAppResult() With {
                .Success = False,
                .Message = "La data di appuntamento non può essere precedente rispetto alla data di fine sospensione del paziente"
            }
        End If

        ' Controllo se esiste già una cnv nella data specificata, e vado al giorno successivo, finchè non ne trovo uno libero
        While GenericProvider.Convocazione.Exists(command.CodicePaziente, dataConvocazione)

            dataConvocazione = dataConvocazione.AddDays(1)

        End While

        ' Vaccinazioni che compongono l'associazione
        Dim listVac As List(Of VaccinazioneAssociazione) = GenericProvider.Associazioni.GetVaccinazioniAssociazioni(command.CodiciAssociazioni)

        Dim codiciVaccinazioniSelezionatiTemp As New List(Of String)()

        For Each vac As VaccinazioneAssociazione In listVac

            If codiciVaccinazioniSelezionatiTemp.Contains(vac.CodiceVaccinazione) Then

                Return New CreaCnvPerAppuntamentoAppResult() With {
                    .Success = False,
                    .Message = "Impossibile prenotare: sono state selezionate associazioni diverse con vaccinazioni in comune"
                }

            Else

                If Settings.CTRL_ASSOCIABILITA_VAC Then

                    ' TODO [API]: CreaCnvPerAppuntamentoApp => salto il controllo sull'associabilità (che è sempre spento...). 
                    '             Se dovesse essere necessario implementarlo, un esempio è nel ConfermaAssociazioni in InsAssociazione.ascx.vb r.205

                End If

                codiciVaccinazioniSelezionatiTemp.Add(vac.CodiceVaccinazione)

            End If

        Next

        Try
            GenericProvider.BeginTransaction()

            Dim codiceConsultorio As String = command.CodiceConsultorio

            If String.IsNullOrWhiteSpace(codiceConsultorio) Then

                Using bizPaziente As New BizPaziente(GenericProvider, Settings, ContextInfos, Nothing)

                    codiceConsultorio = bizPaziente.GetCodiceConsultorio(command.CodicePaziente)

                End Using

            End If

            ' N.B. : la parte qui di seguito è stata presa dal metodo CreaSingolaConvocazione della classe CalcoloConvocazioni

            ' Inserimento convocazione
            Dim convocazione As New Convocazione()

            convocazione.Paz_codice = command.CodicePaziente
            convocazione.Data_CNV = dataConvocazione
            convocazione.Cns_Codice = command.CodiceConsultorio
            convocazione.Durata_Appuntamento = If(command.DurataAppuntamento > 0, command.DurataAppuntamento, Settings.TEMPOSED)
            convocazione.DataInserimento = Date.Now
            convocazione.IdUtenteInserimento = ContextInfos.IDUtente

            If GenericProvider.Convocazione.InsertConvocazione(convocazione) Then

                ' Modifica stato vaccinale paziente: se è in stato Terminato, lo imposta a In Corso (e scrive il log)
                Using bizPaziente As New BizPaziente(GenericProvider, Settings, ContextInfos, Nothing)

                    bizPaziente.UpdateStatoVaccinalePaziente(command.CodicePaziente, StatiVaccinali.Terminato, StatiVaccinali.InCorso)

                End Using

                ' Inserimento vaccinazioni programmate
                Using bizVacProg As New BizVaccinazioneProg(GenericProvider, Settings, ContextInfos, Nothing)

                    Dim now As Date = Date.Now

                    For Each vac As VaccinazioneAssociazione In listVac

                        bizVacProg.InsertVaccinazioneProgrammata(command.CodicePaziente, dataConvocazione, vac.CodiceVaccinazione, vac.CodiceAssociazione, now, ContextInfos.IDUtente)

                    Next

                End Using

            End If

            GenericProvider.Commit()

        Catch ex As Exception

            GenericProvider.Rollback()

            ex.InternalPreserveStackTrace()
            Throw

        End Try

        Return New CreaCnvPerAppuntamentoAppResult() With {
            .Success = True,
            .Message = String.Empty,
            .DataConvocazione = dataConvocazione
        }

    End Function

    Public Function GetLockAppuntamento(DataAppuntamento As DateTime, CodiceAmbulatorio As Long) As LockAppuntamento
        Dim parametro_NPARAMETRO As Integer = Settings.CALCOLOAPP_MINUTI_LOCK_DISPONIBILITA
        Return GenericProvider.AppuntamentiGiorno.GetLockAppuntamento(DataAppuntamento, CodiceAmbulatorio, parametro_NPARAMETRO)
    End Function
    Public Function InserisciLockAppuntamento(CodiceAmb As Long, DataAppuntamento As DateTime) As Integer
        Return GenericProvider.AppuntamentiGiorno.InserisciLockAppuntamento(CodiceAmb, DataAppuntamento)
    End Function
    Public Class CreaCnvPerAppuntamentoAppResult
        Inherits BizGenericResult

        Public Property DataConvocazione As Date

    End Class
    Public Class CercaPeriodiLiberiCommand

        Public Property CodicePaziente As Long
        Public Property CodiceConsultorio As String
        Public Property CodiceAmbulatorio As Integer
        Public Property DescrizioneAmbulatorio As String
        Public Property DurataAppuntamento As Integer
        Public Property DataInizio As Date
        Public Property DataFine As Date
        Public Property MostraPrenotazioni As Boolean
        Public Property PrenotazioneEsterni As Boolean

    End Class
    Public Class CercaPeriodiLiberiResult
        Public GiorniDisponibili As List(Of DateTime)
        Public Appuntamenti As DataTable
        Public OrariValidi As DataTable
        Public AppuntamentiTotali As List(Of AppuntamentoPrenotato)
        Public Success As Boolean
        Public Message As String
    End Class
    <Serializable>
    Public Class AppuntamentoPrenotato
        Public Property CNV_DATA_APPUNTAMENTO As DateTime
        Public Property CNV_FINE_APPUNTAMENTO As DateTime
        Public Property CNV_DURATA_APPUNTAMENTO As Decimal
        Public Property PAZ_SOLO_BILANCIO As Decimal
        Public Property VACCINAZIONI As String
        Public Property AMB_CODICE As Decimal
        Public Property AMB_DES As String
    End Class

    Private Enum PartiGiornata
        Mattina
        Pomeriggio
        Entrambi
    End Enum
#Region " Salvataggio appuntamenti "

    Public Class SalvaAppuntamentiCommand
        Public CodicePaziente As Long
        Public DataConvocazione As DateTime
        Public EliminaBilancio As Boolean
        Public DataAppuntamento As DateTime?
        Public DurataAppuntamento As Integer
        Public CodiceConsultorioAppuntamento As String
        Public CodiceAmbulatorio As Integer
        Public IsSoloBilancio As Boolean
        Public IdMotivoEliminazioneAppuntamento As String
        Public NoteAppuntamento As String
        Public NoteUtenteModificaAppuntamento As String
        Public IdConvocazione As String
        ' campo da valorizzare solo per integrazione phonebar
        Public IdLogChiamata As String
        Public SuppressLog As Boolean
    End Class

    Public Class SalvaAppuntamentiResult
        Inherits BizGenericResult
        Public Property CodiceFiscale As String
        Public Property AppuntamentoSpostato As Boolean

    End Class

    ''' <summary>
    ''' Salvataggio dati appuntamento nella convocazione specificata
    ''' </summary>
    ''' <param name="command"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SalvaAppuntamento(command As SalvaAppuntamentiCommand) As SalvaAppuntamentiResult

        If String.IsNullOrWhiteSpace(Me.ContextInfos.CodiceCentroVaccinale) Then
            Throw New ArgumentNullException("Codice centro vaccinale mancante")
        End If

        Dim logOptionsAppuntamenti As BizLogOptions = Me.LogOptions

        If logOptionsAppuntamenti Is Nothing Then
            logOptionsAppuntamenti = New BizLogOptions(DataLogStructure.TipiArgomento.APPUNTAMENTO, False)
        End If

        Dim result As New SalvaAppuntamentiResult()
        result.AppuntamentoSpostato = False

        Try
            Me.GenericProvider.BeginTransaction()

            Dim dataOperazione As DateTime = DateTime.Now

            If command.EliminaBilancio Then

                Dim convocazioneDaEliminare As Entities.Convocazione = Nothing
                Dim deleted As Boolean = False

                Using bizConvocazione As New BizConvocazione(Me.GenericProvider, Me.Settings, Me.ContextInfos, Nothing)

                    If LogBox.IsEnabled Then
                        convocazioneDaEliminare = bizConvocazione.GetConvocazione(command.CodicePaziente, command.DataConvocazione)
                    End If

                    Dim eliminaCnvCommand As New BizConvocazione.EliminaConvocazioniSollecitiBilanciCommand()
                    eliminaCnvCommand.CodicePaziente = command.CodicePaziente
                    eliminaCnvCommand.DataConvocazione = command.DataConvocazione
                    eliminaCnvCommand.CancellaBilanciAssociati = True
                    eliminaCnvCommand.DataEliminazione = dataOperazione
                    eliminaCnvCommand.IdMotivoEliminazione = command.IdMotivoEliminazioneAppuntamento
                    eliminaCnvCommand.NoteEliminazione = command.NoteAppuntamento
                    eliminaCnvCommand.WriteLog = False

                    deleted = bizConvocazione.EliminaConvocazioniSollecitiBilanci(eliminaCnvCommand)

                End Using

                If deleted AndAlso LogBox.IsEnabled Then
                    LogBox.WriteData(GetTestataLogEliminazioneConvocazioneBilancio(convocazioneDaEliminare, logOptionsAppuntamenti))
                End If

            Else

                Dim convocazioneOriginale As Entities.Convocazione = Nothing
                If LogBox.IsEnabled Then
                    convocazioneOriginale = Me.GenericProvider.Convocazione.GetConvocazionePaziente(command.CodicePaziente, command.DataConvocazione)
                End If

                Dim convocazioneAppuntamento As New Entities.ConvocazioneAppuntamento()
                convocazioneAppuntamento.CodicePaziente = command.CodicePaziente
                convocazioneAppuntamento.DataConvocazione = command.DataConvocazione
                convocazioneAppuntamento.CodiceConsultorio = command.CodiceConsultorioAppuntamento
                convocazioneAppuntamento.Note = command.NoteAppuntamento
                convocazioneAppuntamento.DataInvio = Nothing
                convocazioneAppuntamento.IdUtenteInvio = Nothing

                If command.DataAppuntamento.HasValue Then
                    '---
                    ' Update dati appuntamento
                    '---
                    convocazioneAppuntamento.DataAppuntamento = command.DataAppuntamento.Value
                    convocazioneAppuntamento.DataRegistrazioneAppuntamento = dataOperazione
                    convocazioneAppuntamento.IdUtenteRegistrazioneAppuntamento = Me.ContextInfos.IDUtente

                    convocazioneAppuntamento.DataEliminazioneAppuntamento = Nothing
                    convocazioneAppuntamento.IdUtenteEliminazioneAppuntamento = Nothing
                    convocazioneAppuntamento.IdMotivoEliminazioneAppuntamento = Nothing

                    convocazioneAppuntamento.CodiceAmbulatorio = command.CodiceAmbulatorio
                    convocazioneAppuntamento.TipoAppuntamento = Constants.TipoPrenotazioneAppuntamento.ManualeDaRicercaAppuntamenti

                    convocazioneAppuntamento.DurataAppuntamento = command.DurataAppuntamento

                    convocazioneAppuntamento.NoteAvvisi = String.Empty
                    convocazioneAppuntamento.NoteModificaAppuntamento = command.NoteUtenteModificaAppuntamento

                    Dim list As List(Of KeyValuePair(Of String, Integer)) =
                        Me.GenericProvider.VaccinazioneProg.GetVacProgNotEseguiteAndNotEscluseListCodiceDose(command.CodicePaziente, command.DataConvocazione)

                    If Not list.IsNullOrEmpty Then
                        convocazioneAppuntamento.Vaccinazioni =
                            String.Join(", ", list.Select(Function(item) String.Format("{0} ({1})", item.Key, item.Value)))
                    End If
                Else
                    '---
                    ' Cancellazione dati di appuntamento
                    '---
                    convocazioneAppuntamento.DataAppuntamento = Nothing
                    convocazioneAppuntamento.DataRegistrazioneAppuntamento = Nothing
                    convocazioneAppuntamento.IdUtenteRegistrazioneAppuntamento = Nothing

                    convocazioneAppuntamento.DataEliminazioneAppuntamento = dataOperazione
                    convocazioneAppuntamento.IdUtenteEliminazioneAppuntamento = Me.ContextInfos.IDUtente
                    convocazioneAppuntamento.IdMotivoEliminazioneAppuntamento = command.IdMotivoEliminazioneAppuntamento
                    convocazioneAppuntamento.NoteAvvisi = String.Empty
                    convocazioneAppuntamento.NoteModificaAppuntamento = command.NoteUtenteModificaAppuntamento

                    convocazioneAppuntamento.CodiceAmbulatorio = Nothing
                    convocazioneAppuntamento.TipoAppuntamento = Nothing

                    If command.IsSoloBilancio Then

                        ' Solo bilancio => durata 0
                        convocazioneAppuntamento.DurataAppuntamento = 0

                    Else

                        ' Durata di default in base al consultorio
                        Dim cnsAppuntamento_TEMPOSED As Integer = Me.Settings.TEMPOSED

                        If command.CodiceConsultorioAppuntamento <> Me.ContextInfos.CodiceCentroVaccinale Then

                            ' Caricamento parametro Me.Settings.TEMPOSED in base al consultorio in cui viene dato l'appuntamento
                            Dim parametriConsultorioAppuntamento As BizRicercaAppuntamenti.ParametriConsultorioAppuntamento =
                                Me.GetParametriConsultorioAppuntamento(command.CodiceConsultorioAppuntamento)

                            cnsAppuntamento_TEMPOSED = parametriConsultorioAppuntamento.TempoSed

                        End If

                        convocazioneAppuntamento.DurataAppuntamento = cnsAppuntamento_TEMPOSED

                    End If
                End If

                ' Update convocazione e storico appuntamenti
                Dim count As Integer = 0

                Using bizConvocazione As New BizConvocazione(Me.GenericProvider, Me.Settings, Me.ContextInfos, Me.LogOptions)

                    count = bizConvocazione.UpdateConvocazione_DatiAppuntamento(convocazioneAppuntamento, command.IdMotivoEliminazioneAppuntamento)

                End Using

                If count > 0 Then

                    If command.CodiceConsultorioAppuntamento <> Me.ContextInfos.CodiceCentroVaccinale Then
                        result.AppuntamentoSpostato = True
                    End If

                    If LogBox.IsEnabled AndAlso count > 0 Then
                        LogBox.WriteData(GetTestataLogModificaConvocazioneAppuntamento(convocazioneAppuntamento, convocazioneOriginale, logOptionsAppuntamenti))
                    End If

                End If

            End If

            Me.GenericProvider.Commit()

            result.Success = True
            result.Message = String.Empty

        Catch ex As Exception

            Me.GenericProvider.Rollback()

            result.Success = False
            result.Message = ex.Message

            LogBox.WriteData(LogBox.GetTestataException(ex))

        End Try

        Return result

    End Function

    Private Function GetTestataLogEliminazioneConvocazioneBilancio(convocazioneDaEliminare As Entities.Convocazione, logOptionsAppuntamenti As BizLogOptions) As DataLogStructure.Testata

        Dim recordLog As New DataLogStructure.Record()
        recordLog.Campi.Add(New DataLogStructure.Campo("CNV_DATA", convocazioneDaEliminare.Data_CNV.ToString("dd/MM/yyyy")))
        recordLog.Campi.Add(New DataLogStructure.Campo("CNV_DURATA_APPUNTAMENTO", convocazioneDaEliminare.Durata_Appuntamento.ToString()))
        recordLog.Campi.Add(New DataLogStructure.Campo("CNV_AMB_CODICE", convocazioneDaEliminare.CodiceAmbulatorio.ToString()))

        If convocazioneDaEliminare.DataAppuntamento > DateTime.MinValue Then
            recordLog.Campi.Add(New DataLogStructure.Campo("CNV_DATA_APPUNTAMENTO", convocazioneDaEliminare.DataAppuntamento.ToString("dd/MM/yyyy")))
        Else
            recordLog.Campi.Add(New DataLogStructure.Campo("CNV_DATA_APPUNTAMENTO", "nessuna"))
        End If

        Dim testataLog As New DataLogStructure.Testata(logOptionsAppuntamenti.CodiceArgomento, DataLogStructure.Operazione.Eliminazione, logOptionsAppuntamenti.Automatico)
        testataLog.Records.Add(recordLog)

        Return testataLog

    End Function

    Private Function GetTestataLogModificaConvocazioneAppuntamento(convocazioneAppuntamento As Entities.ConvocazioneAppuntamento, convocazioneOriginale As Entities.Convocazione, logOptionsAppuntamenti As BizLogOptions) As DataLogStructure.Testata

        Dim recordLog As New DataLogStructure.Record()

        If convocazioneAppuntamento.DataConvocazione <> convocazioneOriginale.Data_CNV Then
            recordLog.Campi.Add(New DataLogStructure.Campo("CNV_DATA", convocazioneOriginale.Data_CNV.ToString("dd/MM/yyyy"), convocazioneAppuntamento.DataConvocazione.ToString("dd/MM/yyyy")))
        End If

        If convocazioneAppuntamento.DurataAppuntamento <> convocazioneOriginale.Durata_Appuntamento Then
            recordLog.Campi.Add(New DataLogStructure.Campo("CNV_DURATA_APPUNTAMENTO", convocazioneOriginale.Durata_Appuntamento.ToString(), convocazioneAppuntamento.DurataAppuntamento.Value.ToString()))
        End If

        If Not String.IsNullOrWhiteSpace(convocazioneAppuntamento.CodiceConsultorio) Then
            If convocazioneAppuntamento.CodiceConsultorio <> convocazioneOriginale.Cns_Codice Then
                recordLog.Campi.Add(New DataLogStructure.Campo("CNV_CNS_CODICE", convocazioneOriginale.Cns_Codice, convocazioneAppuntamento.CodiceConsultorio))
            End If
        End If

        If convocazioneAppuntamento.CodiceAmbulatorio.HasValue Then
            If convocazioneAppuntamento.CodiceAmbulatorio.Value <> convocazioneOriginale.CodiceAmbulatorio Then
                recordLog.Campi.Add(New DataLogStructure.Campo("CNV_AMB_CODICE", convocazioneOriginale.CodiceAmbulatorio.ToString(), convocazioneAppuntamento.CodiceAmbulatorio.Value.ToString()))
            End If
        End If

        If convocazioneAppuntamento.DataAppuntamento.HasValue Then
            recordLog.Campi.Add(New DataLogStructure.Campo("CNV_DATA_APPUNTAMENTO", convocazioneAppuntamento.DataAppuntamento.Value.ToString("dd/MM/yyyy")))
        Else
            recordLog.Campi.Add(New DataLogStructure.Campo("CNV_DATA_APPUNTAMENTO", "eliminata"))
        End If

        If Not String.IsNullOrWhiteSpace(convocazioneAppuntamento.TipoAppuntamento) Then
            recordLog.Campi.Add(New DataLogStructure.Campo("CNV_TIPO_APPUNTAMENTO", convocazioneAppuntamento.TipoAppuntamento))
        End If

        If convocazioneAppuntamento.DataPrimoAppuntamento.HasValue Then
            recordLog.Campi.Add(New DataLogStructure.Campo("CNV_PRIMO_APPUNTAMENTO", convocazioneAppuntamento.DataPrimoAppuntamento.Value.ToString("dd/MM/yyyy")))
        End If

        Dim operazioneLog As DataLogStructure.Operazione
        If convocazioneAppuntamento.DataAppuntamento.HasValue Then
            operazioneLog = DataLogStructure.Operazione.Modifica
        Else
            operazioneLog = DataLogStructure.Operazione.Eliminazione
        End If

        Dim testataLog As New DataLogStructure.Testata(logOptionsAppuntamenti.CodiceArgomento, operazioneLog, logOptionsAppuntamenti.Automatico)
        testataLog.Records.Add(recordLog)

        Return testataLog

    End Function

#End Region


#End Region

#Region " Private "

    Private Function GetValoreParametro(nomeParametro As String, listParametri As List(Of KeyValuePair(Of String, Object))) As Integer

        Dim parametro As KeyValuePair(Of String, Object) = (From item As KeyValuePair(Of String, Object) In listParametri
                                                            Where (item.Key = nomeParametro)
                                                            Select item).FirstOrDefault()

        ' N.B. : i parametri in questione non possono essere nulli!
        Return Convert.ToInt32(parametro.Value)

    End Function

#End Region
    Public Class CreaCnvPerAppuntamentoAppCommand

        Public Property CodicePaziente As Long
        Public Property DataAppuntamento As Date
        Public Property DurataAppuntamento As Integer
        Public Property CodiciAssociazioni As List(Of String)
        Public Property CodiceConsultorio As String

    End Class
End Class
