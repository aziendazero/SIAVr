Imports System.Collections.Generic
Imports System.Web
Imports Onit.OnAssistnet.OnVac.CalcoloAppuntamenti
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.Filters
Imports Onit.OnAssistnet.OnVac.Log

Public Class BizGestioneAppuntamenti
    Inherits BizClass

#Region " Costanti "

    ReadOnly NUMEROMASSIMOITERAZIONI As Integer = 1000
    ReadOnly INFO As String = "<IMG src='" + VirtualPathUtility.ToAbsolute("~/images/info.gif") + "' align=absmiddle>&nbsp;"
    ReadOnly ERRORE As String = "<IMG src='" + VirtualPathUtility.ToAbsolute("~/images/annulla.gif") + "' align=absmiddle>&nbsp;"
    ReadOnly ALERTICO As String = "<IMG src='" + VirtualPathUtility.ToAbsolute("~/images/avvertimento.gif") + "' align=absmiddle>&nbsp;"
    ReadOnly CONFERMA As String = "<IMG src='" + VirtualPathUtility.ToAbsolute("~/images/conferma.gif") + "' align=absmiddle>&nbsp;"

#End Region

#Region " Types "

    Enum PartiGiornata
        Mattina
        Pomeriggio
        Entrambi
    End Enum

#End Region

#Region " Costruttori "

    Public Sub New(genericprovider As DbGenericProvider, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)

        MyBase.New(genericprovider, settings, contextInfos, logOptions)

    End Sub

#End Region

    Public Function GetAmbulatorio(codiceConsultorio As String) As Integer

        Dim codiceAmbulatorio As Integer

        Dim dt As DataTable = Me.GenericProvider.Consultori.GetAmbulatoriAperti(codiceConsultorio)

        If dt.Rows.Count = 1 Then
            codiceAmbulatorio = dt.Rows(0)("codice")
        Else
            codiceAmbulatorio = Nothing
        End If

        Return codiceAmbulatorio

    End Function

    'effettua il controllo sugli orari di apertura del consultorio corrente (modifica 21/07/2004)
    Public Function ControllaOrariApertura(codiceAmbulatorio As Integer) As Boolean

        Return Me.GenericProvider.Prenotazioni.ControllaOrariApertura(codiceAmbulatorio)

    End Function

    Public Function BuildDtFeste() As DataTable

        Return Me.GenericProvider.Prenotazioni.BuildDtFeste()

    End Function

    Public Function BuildDtIndisponibilita(codiceAmbulatorio As Integer) As DataTable

        Return Me.GenericProvider.Prenotazioni.BuildDtIndisponibilita(codiceAmbulatorio)

    End Function

    Public Function UltimaDataCnvConsultorio(codiceConsultorio As String) As String

        Return Me.GenericProvider.Prenotazioni.UltimaDataCnvConsultorio(codiceConsultorio)

    End Function

    Public Sub SalvaUltimaDataCnvConsultorio(codiceConsultorio As String, data As Date)

        Me.GenericProvider.Prenotazioni.SalvaUltimaDataCnvConsultorio(codiceConsultorio, data)

    End Sub

    ' Per compatibilità con la funzione di calcolo delle prenotazioni, devo restituire 0 per la domenica
    ' Invece, per ordinare il datagrid degli orari personalizzati, devo restituire 7 per la domenica.
    ' Il parametro domenica_zero indica in quale dei due casi ci si trova.
    Private Function CodificaGiorno(giorno As String, domenicaZero As Boolean) As Integer

        Select Case giorno
            Case "LUN"
                Return 1
            Case "MAR"
                Return 2
            Case "MER"
                Return 3
            Case "GIO"
                Return 4
            Case "VEN"
                Return 5
            Case "SAB"
                Return 6
            Case "DOM"
                If domenicaZero Then
                    Return 0
                Else
                    Return 7
                End If
        End Select

    End Function

    Function CreaOrarioApertura(giorno As DayOfWeek, minDate As Date, maxDate As Date) As Filtri.IFiltro

        Dim fAp As New Filtri.FiltroIndisponibilitaGenerica()

        fAp.Ricorsivita = Filtri.Ricorsivita.Settimanale
        fAp.GiornoSettimana = giorno
        fAp.DaOra = minDate
        fAp.AOra = maxDate

        Return fAp

    End Function

    Function CaricaFiltriApertura(codiceAmbulatorio As Integer, fuoriOrario As Boolean, giorno As DayOfWeek, parte As PartiGiornata,
                                  ByRef FiltriVar As ArrayList, ByRef maxBlocchiGiorno As Integer, ByRef chkOrariPersChecked As Boolean,
                                  ByRef dataMinima As Date, ByRef dataMassima As Date, ByRef dtOrariPersonalizzati As DataTable) As Filtri.FiltroCollection

        Dim f As New Filtri.FiltroCollection()

        maxBlocchiGiorno = 0

        ' Orari iniziali del mattino e del pomeriggio
        Dim oraAm As Object = Date.Parse("08:00")

        Dim _oraPm As String = Me.Settings.ORAPM
        If _oraPm = String.Empty Then _oraPm = "13:00"

        Dim oraPm As Date = Date.Parse(_oraPm)

        ' Controllo se passare all'algoritmo di calcolo degli appuntamenti gli orari fissati (su db)
        ' o quelli inseriti nella maschera degli orari personalizzati
        If chkOrariPersChecked Then

            ' --------- Orari personalizzati --------- '

            ' Mantengo i giorni inseriti nell'arraylist
            Dim listGiorni As New ArrayList()

            ' Datatable con gli orari da passare alla funzione
            Dim dtFasceOrariePers As New DataTable()
            dtFasceOrariePers.Columns.Add("INIZIO", GetType(DateTime))
            dtFasceOrariePers.Columns.Add("FINE", GetType(DateTime))
            dtFasceOrariePers.Columns.Add("GIORNO")
            dtFasceOrariePers.Columns.Add("NUMPAZ")
            dtFasceOrariePers.Columns.Add("DURATA")

            ' Costruisco dtFasceOrariePers in base al datatable degli orari personalizzati
            For i As Integer = 0 To dtOrariPersonalizzati.Rows.Count - 1

                Dim aggiungiFascia As Boolean = True

                ' Controllo l'orario di fine della fascia rispetto all'orario battezzato come inizio del pomeriggio:
                ' - se la fascia termina nel pomeriggio, e il parametro "parte" vale "Mattina", non considero la fascia tra quelle prenotabili
                ' - se la fascia termina nella mattina, e il parametro "parte" vale "Pomeriggio", non considero la fascia tra quelle prenotabili
                Dim fine As DateTime = Date.Parse(dtOrariPersonalizzati.Rows(i)("orp_ora_fine").ToString())

                If parte = PartiGiornata.Mattina Then
                    If fine > oraPm Then
                        aggiungiFascia = False
                    End If
                ElseIf parte = PartiGiornata.Pomeriggio Then
                    If fine <= oraPm Then
                        aggiungiFascia = False
                    End If
                End If

                If aggiungiFascia Then

                    Dim newRow As DataRow = dtFasceOrariePers.NewRow()
                    newRow("INIZIO") = Date.Parse(dtOrariPersonalizzati.Rows(i)("orp_ora_inizio").ToString())
                    newRow("FINE") = fine
                    newRow("GIORNO") = CodificaGiorno(dtOrariPersonalizzati.Rows(i)("orp_giorno").ToString(), True)
                    newRow("NUMPAZ") = dtOrariPersonalizzati.Rows(i)("orp_num_pazienti").ToString()
                    newRow("DURATA") = dtOrariPersonalizzati.Rows(i)("orp_durata").ToString()

                    If Not listGiorni.Contains(newRow("GIORNO").ToString()) Then
                        listGiorni.Add(newRow("GIORNO").ToString())
                    End If

                    dtFasceOrariePers.Rows.Add(newRow)

                End If

            Next

            ' listGiorni contiene i giorni inseriti in dta. Devo inserire anche i giorni mancanti
            ' con tutti gli orari vuoti, per creare correttamente la struttura dei filtri.

            For i As Integer = 0 To 6   ' rappresentano i giorni della settimana

                If Not listGiorni.Contains(i.ToString()) Then

                    ' Creo un giorno senza orari
                    Dim newRow As DataRow = dtFasceOrariePers.NewRow()
                    newRow("INIZIO") = DBNull.Value
                    newRow("FINE") = DBNull.Value
                    newRow("GIORNO") = i.ToString()
                    newRow("NUMPAZ") = DBNull.Value
                    newRow("DURATA") = DBNull.Value

                    ' Calcolo la posizione in cui inserire l'elemento, a seconda del giorno della settimana che rappresenta
                    Dim pos As Integer = 0

                    For pos = 0 To dtFasceOrariePers.Rows.Count - 1
                        If dtFasceOrariePers.Rows(pos)("GIORNO").ToString() > newRow("GIORNO").ToString() Then
                            Exit For
                        End If
                    Next

                    dtFasceOrariePers.Rows.InsertAt(newRow, pos)

                End If
            Next

            ' --- Impostazione filtri per l'algoritmo di prenotazione --- '
            Dim giorniInseriti As New ArrayList()
            Dim idxRow As Integer

            Dim dv As DataView
            For Each r As DataRow In dtFasceOrariePers.Rows

                Dim fAp As New Filtri.FiltroIndisponibilitaGenerica()

                If Not giorniInseriti.Contains(r("GIORNO").ToString()) Then

                    giorniInseriti.Add(r("GIORNO").ToString())

                    ' Considero tutte le fasce orarie del giorno specificato
                    dv = New DataView(dtFasceOrariePers)
                    dv.RowFilter = "GIORNO='" + r("GIORNO").ToString() + "'"
                    dv.Sort = "INIZIO"

                    ' Numero massimo di fasce orarie impostate in un giorno
                    If dv.Count > maxBlocchiGiorno Then
                        maxBlocchiGiorno = dv.Count
                    End If

                    If dv.Count = 1 Then
                        ' --- Unica fascia oraria --- '

                        If dv(0)("INIZIO") Is DBNull.Value Then

                            ' Giorno non disponibile
                            f.Add(CreaOrarioApertura(r("GIORNO"), Timing.Day.MinOrario, Timing.Day.MaxOrario))

                        Else
                            ' Giorno disponibile con un'unica fascia oraria

                            ' Fasce orarie indisponibili
                            f.Add(CreaOrarioApertura(r("GIORNO"), Timing.Day.MinOrario, r("INIZIO")))
                            f.Add(CreaOrarioApertura(r("GIORNO"), r("FINE"), Timing.Day.MaxOrario))

                            ' Fascia disponibile
                            Dim h As New Hashtable()

                            For dayCount As Integer = 0 To dataMassima.Subtract(dataMinima).TotalDays

                                Dim d As Date = dataMinima.AddDays(dayCount)

                                If d.DayOfWeek = r("GIORNO") Then

                                    Dim fMax As New Filtri.ParameterBlocco()
                                    fMax.DaOra = dv(0)("INIZIO")
                                    fMax.AOra = dv(0)("FINE")

                                    If dv(0)("DURATA").ToString() = "" Then
                                        fMax.Durata = -1
                                        fMax.OverrideDurata = False
                                    Else
                                        fMax.Durata = dv(0)("DURATA")
                                        fMax.OverrideDurata = True
                                    End If

                                    If dv(0)("NUMPAZ").ToString() = "" Then
                                        fMax.Numero = -1
                                    Else
                                        fMax.Numero = dv(0)("NUMPAZ")
                                    End If

                                    If h.ContainsKey(d) Then
                                        h(d) += 1
                                    Else
                                        h.Add(d, 0)
                                    End If

                                    FiltriVar.Add(New Object() {Filtri.FiltroNumeroMassimoAppuntamentiBlocco.GetNewParameterID(d, h(d)), fMax})

                                End If
                            Next

                        End If

                    Else
                        ' --- Più di una fascia nello stesso giorno --- '

                        Dim inizio As DateTime = Timing.Day.MinOrario

                        Dim h As New Hashtable()

                        For idxRow = 0 To dv.Count - 1

                            ' Fascia oraria indisponibile
                            f.Add(CreaOrarioApertura(r("GIORNO"), inizio, dv(idxRow)("INIZIO")))
                            inizio = dv(idxRow)("FINE")

                            ' Fasce orarie disponibili
                            For dayCount As Integer = 0 To dataMassima.Subtract(dataMinima).TotalDays

                                Dim d As Date = dataMinima.AddDays(dayCount)

                                If d.DayOfWeek = r("GIORNO") Then

                                    Dim fMax As New Filtri.ParameterBlocco()

                                    fMax.DaOra = dv(idxRow)("INIZIO")
                                    fMax.AOra = dv(idxRow)("FINE")

                                    If dv(idxRow)("DURATA").ToString() = "" Then
                                        fMax.Durata = -1
                                        fMax.OverrideDurata = False
                                    Else
                                        fMax.Durata = dv(idxRow)("DURATA")
                                        fMax.OverrideDurata = True
                                    End If

                                    If dv(idxRow)("NUMPAZ").ToString() = "" Then
                                        fMax.Numero = -1
                                    Else
                                        fMax.Numero = dv(idxRow)("NUMPAZ")
                                    End If

                                    If h.ContainsKey(d) Then
                                        h(d) += 1
                                    Else
                                        h.Add(d, 0)
                                    End If

                                    FiltriVar.Add(New Object() {Filtri.FiltroNumeroMassimoAppuntamentiBlocco.GetNewParameterID(d, h(d)), fMax})

                                End If
                            Next

                        Next

                        ' Fascia indisponibile di chiusura (ultima del giorno)
                        f.Add(CreaOrarioApertura(r("GIORNO"), inizio, Timing.Day.MaxOrario))

                    End If

                End If
            Next

        Else
            ' --------- Orari fissi --------- '

            ' Datatable con gli orari da passare alla funzione
            Dim dta As DataTable = Me.GenericProvider.Prenotazioni.GetFiltriApertura(codiceAmbulatorio, fuoriOrario, giorno)

            ' --- Impostazione filtri --- '
            For Each r As DataRow In dta.Rows

                Dim fAp As New Filtri.FiltroIndisponibilitaGenerica()
                Dim defMat As Boolean = Not r("AM_INIZIO") Is DBNull.Value AndAlso Not r("AM_FINE") Is DBNull.Value
                Dim defPom As Boolean = Not r("PM_INIZIO") Is DBNull.Value AndAlso Not r("PM_FINE") Is DBNull.Value

                If parte = PartiGiornata.Mattina Then

                    If defMat And defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), Timing.Day.MinOrario, r("AM_INIZIO")))
                        f.Add(CreaOrarioApertura(r("GIORNO"), r("PM_INIZIO"), Timing.Day.MaxOrario))
                    ElseIf Not defMat AndAlso Not defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), Timing.Day.MinOrario, oraAm))
                        f.Add(CreaOrarioApertura(r("GIORNO"), oraPm, Timing.Day.MaxOrario))
                    ElseIf defMat AndAlso Not defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), Timing.Day.MinOrario, r("AM_INIZIO")))
                        f.Add(CreaOrarioApertura(r("GIORNO"), oraPm, Timing.Day.MaxOrario))
                    ElseIf Not defMat AndAlso defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), Timing.Day.MinOrario, oraAm))
                        f.Add(CreaOrarioApertura(r("GIORNO"), r("PM_INIZIO"), Timing.Day.MaxOrario))
                    End If

                ElseIf parte = PartiGiornata.Pomeriggio Then

                    If defMat And defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), Timing.Day.MinOrario, r("PM_INIZIO")))
                    ElseIf Not defMat AndAlso Not defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), Timing.Day.MinOrario, oraPm))
                    ElseIf defMat AndAlso Not defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), Timing.Day.MinOrario, oraPm))
                    ElseIf Not defMat AndAlso defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), Timing.Day.MinOrario, r("PM_INIZIO")))
                    End If

                Else

                    If defMat And defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), Timing.Day.MinOrario, r("AM_INIZIO")))
                        f.Add(CreaOrarioApertura(r("GIORNO"), r("AM_FINE"), r("PM_INIZIO")))
                        f.Add(CreaOrarioApertura(r("GIORNO"), r("PM_FINE"), Timing.Day.MaxOrario))
                    ElseIf Not defMat AndAlso Not defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), Timing.Day.MinOrario, Timing.Day.MaxOrario))
                    ElseIf defMat AndAlso Not defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), Timing.Day.MinOrario, r("AM_INIZIO")))
                        f.Add(CreaOrarioApertura(r("GIORNO"), r("AM_FINE"), Timing.Day.MaxOrario))
                    ElseIf Not defMat AndAlso defPom Then
                        f.Add(CreaOrarioApertura(r("GIORNO"), Timing.Day.MinOrario, r("PM_INIZIO")))
                        f.Add(CreaOrarioApertura(r("GIORNO"), r("PM_FINE"), Timing.Day.MaxOrario))
                    End If

                End If

            Next
        End If

        Return f

    End Function

    Function CaricaFiltriFeste(codiceAmbulatorio As String) As Filtri.FiltroCollection

        Dim filters As New Filtri.FiltroCollection()
        Dim dta As New DataTable()

        dta = Me.GenericProvider.Prenotazioni.GetFestivita(codiceAmbulatorio)

        For i As Integer = 0 To dta.Rows.Count - 1

            Dim r As DataRow = dta.Rows(i)

            Dim f As New Filtri.FiltroIndisponibilitaGenerica()
            Dim data As Date = r("DATA")

            If Not r("RICORSIVITA") Is DBNull.Value AndAlso r("RICORSIVITA").ToString() = OnVac.Constants.CommonConstants.RECURSIVE_YEAR Then
                f.Ricorsivita = Filtri.Ricorsivita.Annuale
            Else
                f.Ricorsivita = Filtri.Ricorsivita.Singola
            End If

            f.Anno = data.Year
            f.Mese = data.Month
            f.Giorno = data.Day

            If Not r("INIZIO") Is DBNull.Value Then
                f.DaOra = r("INIZIO")
                f.AOra = r("FINE")
            Else
                f.DaOra = Timing.Day.MinOrario
                f.AOra = Timing.Day.MaxOrario
            End If

            filters.Add(f)

        Next

        Return filters

    End Function

    Public Function InMattino(codiceAmbulatorio As Integer, ByRef cnsControl As Boolean, appuntamento As DateTime) As Boolean

        Dim oraAppuntamento As DateTime = New DateTime(1900, 1, 1, appuntamento.Hour, appuntamento.Minute, 0)

        Dim dtaGiorno As DataTable = Me.GenericProvider.Prenotazioni.GetFiltriApertura(codiceAmbulatorio, cnsControl)

        If dtaGiorno.Rows.Count > 0 Then

            Dim r As DataRow = dtaGiorno.Select("GIORNO=" & appuntamento.DayOfWeek)(0)

            Dim oraAperturaAM As DateTime = r.Item(0)
            Dim oraChiusuraAM As DateTime = r.Item(1)
            Dim oraAperturaPM As DateTime = r.Item(2)
            Dim oraChiusuraPM As DateTime = r.Item(3)

            If oraAppuntamento < oraChiusuraAM Then
                Return True
            ElseIf oraAppuntamento >= oraAperturaPM Then
                Return False
            End If

        End If

        Return TimeSpan.Compare(oraAppuntamento.TimeOfDay, #2:00:00 PM#.TimeOfDay) < 0

    End Function

    Public Function CaricaOrariPersonalizzati(codiceAmbulatorio As String) As DataTable

        Return Me.GenericProvider.Prenotazioni.CaricaOrariPersonalizzati(codiceAmbulatorio)

    End Function

    ' Salva gli orari inseriti
    Public Function SalvaOrariPersonalizzati(codiceAmbulatorio As Integer, ByRef dtOrariPersonalizzati As DataTable) As Boolean

        Return Me.GenericProvider.Prenotazioni.SalvaOrariPersonalizzati(codiceAmbulatorio, dtOrariPersonalizzati)

    End Function

    ' Il datatable deve essere già stato aggiornato.
    ' Controllo che i campi siano tutti riempiti e che gli orari non si sovrappongano.
    ' Non controllo la formattazione degli orari perchè avviene lato client.
    Public Function ControlloOrariPersonalizzati(ByRef dtOrariPersonalizzati As DataTable, checkOrariVuoti As Boolean) As Boolean

        Dim i, j As Integer

        If checkOrariVuoti Then
            For i = 0 To dtOrariPersonalizzati.Rows.Count - 1
                If dtOrariPersonalizzati.Rows(i)("ORP_ORA_INIZIO").ToString() = "" Then
                    Return False
                End If
                If dtOrariPersonalizzati.Rows(i)("ORP_ORA_FINE").ToString() = "" Then
                    Return False
                End If
            Next
        End If

        ' Controllo sovrapposizioni di orario
        Dim oraInizioCheck, oraFineCheck, oraInizio, oraFine As DateTime

        For i = 0 To dtOrariPersonalizzati.Rows.Count - 2
            ' Effettuo il controllo solo se gli orari non sono vuoti
            If dtOrariPersonalizzati.Rows(i)("ORP_ORA_INIZIO").ToString() <> "" And
               dtOrariPersonalizzati.Rows(i)("ORP_ORA_FINE").ToString() <> "" Then

                oraInizioCheck = Date.Parse(dtOrariPersonalizzati.Rows(i)("ORP_ORA_INIZIO").ToString())
                oraFineCheck = Date.Parse(dtOrariPersonalizzati.Rows(i)("ORP_ORA_FINE").ToString())

                ' Controllo che inizio non sia maggiore o uguale a fine
                If Date.op_GreaterThanOrEqual(oraInizioCheck, oraFineCheck) Then
                    Return False
                End If

                ' Controllo le righe successive
                For j = i + 1 To dtOrariPersonalizzati.Rows.Count - 1

                    ' Stesso giorno
                    If dtOrariPersonalizzati.Rows(i)("ORP_GIORNO").ToString() = dtOrariPersonalizzati.Rows(j)("ORP_GIORNO") Then
                        ' Effettuo il controllo solo se gli orari non sono vuoti
                        If dtOrariPersonalizzati.Rows(j)("ORP_ORA_INIZIO").ToString() <> "" And
                           dtOrariPersonalizzati.Rows(j)("ORP_ORA_FINE").ToString() <> "" Then

                            ' Confronto gli orari
                            oraInizio = Date.Parse(dtOrariPersonalizzati.Rows(j)("ORP_ORA_INIZIO").ToString())
                            oraFine = Date.Parse(dtOrariPersonalizzati.Rows(j)("ORP_ORA_FINE").ToString())

                            If Date.op_LessThan(oraInizio, oraFineCheck) And Date.op_GreaterThan(oraFine, oraInizioCheck) Then
                                ' Sovrapposti
                                Return False
                            End If

                        End If

                    End If

                Next

            End If
        Next

        ' Se c'è una sola riga nel datatable, non entra nel for precedente, perciò non effettua 
        ' nemmeno il controllo che inizio sia minore di fine per la stessa fascia oraria
        If dtOrariPersonalizzati.Rows.Count = 1 Then

            oraInizioCheck = Date.Parse(dtOrariPersonalizzati.Rows(i)("ORP_ORA_INIZIO").ToString())
            oraFineCheck = Date.Parse(dtOrariPersonalizzati.Rows(i)("ORP_ORA_FINE").ToString())

            ' Controllo che inizio non sia maggiore o uguale a fine
            If Date.op_GreaterThanOrEqual(oraInizioCheck, oraFineCheck) Then
                Return False
            End If

        End If

        Return True

    End Function

    Public Function CercaConvocazioni(codiciConsultorio As List(Of String), filtri As FiltriGestioneAppuntamenti) As DataTable

        Dim durataSedutaBilancioDefault As Integer = 0
        If Settings.TEMPOBIL > 0 Then durataSedutaBilancioDefault = Settings.TEMPOBIL

        'Modificata la validità dei solo bilanci 
        'in modo da aumentare la priorità dei "solo bilanci senza pediatra" che altrimenti non verrebbero prenotati

        Dim validitaSB As Integer = 365
        If Settings.VALIDITA_SB > -1 Then validitaSB = Settings.VALIDITA_SB

        Return GenericProvider.Prenotazioni.CercaConvocazioni(codiciConsultorio, durataSedutaBilancioDefault, Settings.NUMSOL, validitaSB, filtri, filtri.chkOrdineAlfabeticoRicerca)

    End Function

    Public Function GetAmbDescrizione(codiceAmbulatorio As Integer) As String

        Return GenericProvider.Consultori.GetAmbDescrizione(codiceAmbulatorio)

    End Function

    ''' <summary>
    ''' Controlla che la riga non sia già presente nel datatable per non generare l'errore di indice
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <param name="row"></param>
    Private Function ControllaDuplicazioneRiga(dt As DataTable, row As DataRow) As Boolean

        Dim rowOrig As DataRow

        For Each rowOrig In dt.Rows
            If rowOrig("PAZ_CODICE") = row("PAZ_CODICE") And rowOrig("CNV_DATA") = row("CNV_DATA") Then
                Return True
            End If
        Next

        Return False

    End Function

    Public Sub BuildDataTableAppuntamenti(codiceAmbulatorio As Integer, Data As String, ByRef dtaMattino As DataTable, ByRef dtaPomeriggio As DataTable,
                                          ByRef oraAMPM As DateTime, ByRef oraMinPM As DateTime, ByRef LoadedDates As ArrayList)

        If Not LoadedDates.Contains(CType(Data, Date)) Then

            ' Memorizza nell'Array LoadDates la data caricata in memoria (per evitare poi di riprendere le informazioni dal DB)
            LoadedDates.Add(CType(Data, Date))

            ' Datatable appuntamenti Mattino
            Dim tmp_dtaMattino As New DataTable("Mattino")
            Me.GenericProvider.Prenotazioni.FillDtMattino(tmp_dtaMattino, codiceAmbulatorio, Data, oraAMPM, Settings.NUMSOL)

            'Creo la struttura di colonne del DataTable principale
            If dtaMattino.Columns.Count = 0 Then
                dtaMattino = tmp_dtaMattino.Clone()
            End If

            'aggiungo le righe alla struttura usata per il bind del dataList
            For k As Integer = 0 To tmp_dtaMattino.Rows.Count - 1
                'controllo che le righe non siano già inserite per evitare l'errore [modifica 25/01/2005]
                If Not ControllaDuplicazioneRiga(dtaMattino, tmp_dtaMattino.Rows(k)) Then dtaMattino.ImportRow(tmp_dtaMattino.Rows(k))
            Next

            dtaMattino.PrimaryKey = New DataColumn() {dtaMattino.Columns("PAZ_CODICE"), dtaMattino.Columns("CNV_DATA")}

            ' Datatable appuntamenti Pomeriggio
            Dim tmp_dtaPomeriggio As New DataTable("Pomeriggio")
            Me.GenericProvider.Prenotazioni.FillDtPomeriggio(tmp_dtaPomeriggio, codiceAmbulatorio, Data, oraMinPM, Settings.NUMSOL)

            'Creo la struttura di colonne del DataTable principale
            If dtaPomeriggio.Columns.Count = 0 Then
                dtaPomeriggio = tmp_dtaPomeriggio.Clone()
            End If

            'aggiungo le righe alla struttura usata per il bind del dataList
            For k As Integer = 0 To tmp_dtaPomeriggio.Rows.Count - 1
                'controllo che le righe non siano già inserite per evitare l'errore [modifica 25/01/2005]
                If Not ControllaDuplicazioneRiga(dtaPomeriggio, tmp_dtaPomeriggio.Rows(k)) Then dtaPomeriggio.ImportRow(tmp_dtaPomeriggio.Rows(k))
            Next

            dtaPomeriggio.PrimaryKey = New DataColumn() {dtaPomeriggio.Columns("PAZ_CODICE"), dtaPomeriggio.Columns("CNV_DATA")}

        End If

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="codiceAmbulatorio"></param>
    ''' <returns>DataTable contenente il numero di appuntamenti divisi per tipo</returns>
    Public Function FillDtAppuntamenti(codiceAmbulatorio As Integer) As DataTable

        Return Me.GenericProvider.Prenotazioni.GetAppuntamenti(codiceAmbulatorio)

    End Function

#Region " Salvataggio appuntamenti "

    Public Function SalvaAppuntamenti(dtaMattino As DataTable, dtaPomeriggio As DataTable, codiceConsultorio As String, noteAppuntamento As String) As BizGenericResult

        Dim logOptionsAppuntamenti As BizLogOptions = Me.LogOptions

        If logOptionsAppuntamenti Is Nothing OrElse String.IsNullOrWhiteSpace(logOptionsAppuntamenti.CodiceArgomento) Then
            logOptionsAppuntamenti = New BizLogOptions(DataLogStructure.TipiArgomento.APPUNTAMENTO, False)
        End If

        Dim result As New BizGenericResult()

        Try
            Me.GenericProvider.BeginTransaction()

            Dim dataOperazione As DateTime = DateTime.Now

            Dim listConvocazioniAppuntamenti As New List(Of Entities.ConvocazioneAppuntamento)()

            For Each rowMattino As DataRow In dtaMattino.Rows

                If rowMattino.RowState = DataRowState.Modified OrElse rowMattino.RowState = DataRowState.Added Then

                    listConvocazioniAppuntamenti.Add(GetConvocazioneAppuntamento(rowMattino, codiceConsultorio, dataOperazione, noteAppuntamento))

                End If

            Next

            For Each rowPomeriggio As DataRow In dtaPomeriggio.Rows

                If rowPomeriggio.RowState = DataRowState.Modified OrElse rowPomeriggio.RowState = DataRowState.Added Then

                    Dim codicePaziente As Long = Convert.ToInt64(rowPomeriggio("PAZ_CODICE"))
                    Dim dataConvocazione As DateTime = Convert.ToDateTime(rowPomeriggio("CNV_DATA"))

                    Dim itemMattino As Entities.ConvocazioneAppuntamento =
                        listConvocazioniAppuntamenti.FirstOrDefault(Function(p) p.CodicePaziente = codicePaziente And p.DataConvocazione = dataConvocazione)

                    ' Controllo che la cnv non sia già presente, oppure ci sia ma senza appuntamento
                    If itemMattino Is Nothing OrElse Not itemMattino.DataAppuntamento.HasValue Then

                        listConvocazioniAppuntamenti.Add(GetConvocazioneAppuntamento(rowPomeriggio, codiceConsultorio, dataOperazione, noteAppuntamento))

                    End If

                End If

            Next

            If Not listConvocazioniAppuntamenti.IsNullOrEmpty() Then

                Dim listTestateLog As New List(Of DataLogStructure.Testata)()

                For Each convocazioneAppuntamento As Entities.ConvocazioneAppuntamento In listConvocazioniAppuntamenti

                    Dim count As Integer = 0

                    ' Aggiornamento/eliminazione dati dell'appuntamento relativi alla convocazione
                    Using bizConvocazione As New BizConvocazione(Me.GenericProvider, Me.Settings, Me.ContextInfos, Me.LogOptions)
                        count = bizConvocazione.UpdateConvocazione_DatiAppuntamento(convocazioneAppuntamento)
                    End Using

                    ' Creazione testata per log
                    If count > 0 Then
                        listTestateLog.Add(GetTestataLogAppuntamento(convocazioneAppuntamento, logOptionsAppuntamenti))
                    End If

                Next

                If Not listTestateLog.IsNullOrEmpty() Then
                    LogBox.WriteData(listTestateLog.ToArray())
                End If

            End If

            Me.GenericProvider.Commit()

        Catch ex As Exception

            Common.Utility.EventLogHelper.EventLogWrite(ex, Me.ContextInfos.IDApplicazione)

            result.Message = "Errore durante il salvataggio degli appuntamenti." + Environment.NewLine + ex.Message
            result.Success = False

            Me.GenericProvider.Rollback()

        End Try

        Return result

    End Function

    Private Function GetTestataLogAppuntamento(convocazioneAppuntamento As Entities.ConvocazioneAppuntamento, logOptionsAppuntamenti As BizLogOptions) As DataLogStructure.Testata

        Dim recordLog As New DataLogStructure.Record()
        recordLog.Campi.Add(New DataLogStructure.Campo("CNV_DATA", convocazioneAppuntamento.DataConvocazione.ToString("dd/MM/yyyy")))
        recordLog.Campi.Add(New DataLogStructure.Campo("CNV_CNS_CODICE", convocazioneAppuntamento.CodiceConsultorio))

        If convocazioneAppuntamento.CodiceAmbulatorio.HasValue Then
            recordLog.Campi.Add(New DataLogStructure.Campo("CNV_AMB_CODICE", convocazioneAppuntamento.CodiceAmbulatorio.Value.ToString()))
        End If

        If convocazioneAppuntamento.DurataAppuntamento.HasValue Then
            recordLog.Campi.Add(New DataLogStructure.Campo("CNV_DURATA_APPUNTAMENTO", convocazioneAppuntamento.DurataAppuntamento.Value.ToString()))
        End If

        If convocazioneAppuntamento.DataAppuntamento.HasValue Then
            recordLog.Campi.Add(New DataLogStructure.Campo("CNV_DATA_APPUNTAMENTO", convocazioneAppuntamento.DataAppuntamento.Value.ToString("dd/MM/yyyy")))
        Else
            recordLog.Campi.Add(New DataLogStructure.Campo("CNV_DATA_APPUNTAMENTO", "eliminata"))
        End If

        If String.IsNullOrWhiteSpace(convocazioneAppuntamento.TipoAppuntamento) Then
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
        testataLog.Automatico = (convocazioneAppuntamento.TipoAppuntamento = Constants.TipoPrenotazioneAppuntamento.Automatica)
        testataLog.Records.Add(recordLog)

        Return testataLog

    End Function

    Private Function GetConvocazioneAppuntamento(row As DataRow, codiceConsultorio As String, dataOperazione As DateTime?, noteAppuntamento As String) As Entities.ConvocazioneAppuntamento

        Dim convocazioneAppuntamento As New Entities.ConvocazioneAppuntamento()
        convocazioneAppuntamento.CodicePaziente = Convert.ToInt64(row("PAZ_CODICE"))
        convocazioneAppuntamento.DataConvocazione = Convert.ToDateTime(row("CNV_DATA"))
        convocazioneAppuntamento.CodiceConsultorio = codiceConsultorio

        convocazioneAppuntamento.DataInvio = Nothing
        convocazioneAppuntamento.IdUtenteInvio = Nothing

        ' La durata è obbligatoria
        convocazioneAppuntamento.DurataAppuntamento = Convert.ToInt32(row("CNV_DURATA_APPUNTAMENTO"))

        If Not dataOperazione.HasValue Then dataOperazione = DateTime.Now

        If row.IsNull("CNV_DATA_APPUNTAMENTO") Then
            '---
            ' Cancellazione appuntamento
            '---
            convocazioneAppuntamento.DataAppuntamento = Nothing
            convocazioneAppuntamento.DataRegistrazioneAppuntamento = Nothing
            convocazioneAppuntamento.IdUtenteRegistrazioneAppuntamento = Nothing

            convocazioneAppuntamento.DataEliminazioneAppuntamento = dataOperazione
            convocazioneAppuntamento.IdUtenteEliminazioneAppuntamento = Me.ContextInfos.IDUtente
            convocazioneAppuntamento.IdMotivoEliminazioneAppuntamento = Constants.MotiviEliminazioneAppuntamento.EliminazioneAppuntamento

            convocazioneAppuntamento.CodiceAmbulatorio = Nothing
            convocazioneAppuntamento.TipoAppuntamento = Nothing

            convocazioneAppuntamento.Note = noteAppuntamento + ": Eliminazione appuntamento"

            ' TODO [storicoAppunt]: se prevista cancellazione, valorizzare durata come bizRicercaApp (accomunare i due metodi)?
            'If Command.IsSoloBilancio Then

            '    ' Solo bilancio => durata 0
            '    convocazioneAppuntamento.DurataAppuntamento = 0

            'Else

            '    ' Durata di default in base al consultorio
            '    Dim cnsAppuntamento_TEMPOSED As Integer = Me.Settings.TEMPOSED

            '    If Command.CodiceConsultorioAppuntamento <> Me.ContextInfos.CodiceCentroVaccinale Then

            '        ' Caricamento parametro Me.Settings.TEMPOSED in base al consultorio in cui viene dato l'appuntamento
            '        Dim parametriConsultorioAppuntamento As BizRicercaAppuntamenti.ParametriConsultorioAppuntamento =
            '            Me.GetParametriConsultorioAppuntamento(Command.CodiceConsultorioAppuntamento)

            '        cnsAppuntamento_TEMPOSED = parametriConsultorioAppuntamento.TempoSed

            '    End If

            '    convocazioneAppuntamento.DurataAppuntamento = cnsAppuntamento_TEMPOSED

            'End If

        Else
            '---
            ' Update dati appuntamento della convocazione
            '---
            convocazioneAppuntamento.DataAppuntamento = Convert.ToDateTime(row("CNV_DATA_APPUNTAMENTO"))
            convocazioneAppuntamento.DataRegistrazioneAppuntamento = dataOperazione
            convocazioneAppuntamento.IdUtenteRegistrazioneAppuntamento = Me.ContextInfos.IDUtente

            convocazioneAppuntamento.DataEliminazioneAppuntamento = Nothing
            convocazioneAppuntamento.IdUtenteEliminazioneAppuntamento = Nothing
            convocazioneAppuntamento.IdMotivoEliminazioneAppuntamento = Nothing

            If row.RowState = DataRowState.Modified AndAlso row("CNV_DATA_APPUNTAMENTO", DataRowVersion.Original) <> row("CNV_DATA_APPUNTAMENTO", DataRowVersion.Current) Then
                convocazioneAppuntamento.Note = noteAppuntamento    ' N.B. : l'indicazione dello spostamento è nel metodo del bizConvocazioni che viene richiamato successivamente
            Else
                convocazioneAppuntamento.Note = noteAppuntamento + ": Prenotazione appuntamento"
            End If

            ' In prenotazione, l'ambulatorio è obbligatorio
            convocazioneAppuntamento.CodiceAmbulatorio = Convert.ToInt32(row("CNV_AMB_CODICE"))

            If row.IsNull("CNV_TIPO_APPUNTAMENTO") Then
                ' Se il campo tipo appuntamento è vuoto => imposto per default al tipo prenotazione automatica
                convocazioneAppuntamento.TipoAppuntamento = Constants.TipoPrenotazioneAppuntamento.Automatica
            Else
                ' Il tipo appuntamento è valorizzato (valori possibili: prenotazione automatica o prenotazione manuale da gestione app.) 
                convocazioneAppuntamento.TipoAppuntamento = row("CNV_TIPO_APPUNTAMENTO").ToString()
            End If

            Dim list As List(Of KeyValuePair(Of String, Integer)) = Me.GenericProvider.VaccinazioneProg.GetVacProgNotEseguiteAndNotEscluseListCodiceDose(
                convocazioneAppuntamento.CodicePaziente, convocazioneAppuntamento.DataConvocazione)

            If Not list.IsNullOrEmpty Then
                convocazioneAppuntamento.Vaccinazioni = String.Join(",", list.Select(Function(item) String.Format("{0} ({1})", item.Key, item.Value)))
            End If
        End If

        ' Non presenti nel datatable: CNV_ETA_POMERIGGIO, CNV_CAMPAGNA, CNV_PAZ_CODICE_OLD
        'convocazioneAppuntamento.EtaPomeriggio = row("CNV_ETA_POMERIGGIO").ToString()
        'convocazioneAppuntamento.Campagna = row("CNV_CAMPAGNA").ToString()

        'If row.IsNull("CNV_PAZ_CODICE_OLD") Then
        '    convocazioneAppuntamento.CodicePazienteOld = Nothing
        'Else
        '    convocazioneAppuntamento.CodicePazienteOld = Convert.ToInt64(row("CNV_PAZ_CODICE_OLD"))
        'End If

        Return convocazioneAppuntamento

    End Function

#End Region

#Region " Prenotazione automatica "


    Public Const ID_PROCESSO_PRENOTAZIONE_AUTOMATICA As Integer = 27
    Public Const DESCRIZIONE_PROCESSO_PRENOTAZIONE_AUTOMATICA As String = "Processo di prenotazione automatica appuntamenti"

    Public Class BatchPrenotazioneAutomatica

        Public Class ParameterName
            Public Const IdApplicazioneCorrente As String = "IdApplicazioneCorrente"
            Public Const IdUtentePrenotazione As String = "IdUtentePrenotazione"
            ' TODO [App Auto]: Public Const FiltriProgrammazione As String = "FiltriProgrammazione"
            ' TODO [App Auto]: Public Const TipoFiltri As String = "TipoFiltri"
            Public Const CodiceUslCorrente As String = "CodiceUslCorrente"

            Public Const DataNascitaInizio As String = "DataNascitaInizio"
            Public Const DataNascitaFine As String = "DataNascitaFine"
            Public Const Sesso As String = "Sesso"
            Public Const DataUltimaConvocazione As String = "DataUltimaConvocazione"
            Public Const CodiciConsultoriRicerca As String = "CodiciConsultoriRicerca"
            Public Const CodiceDistretto As String = "CodiceDistretto"
            Public Const FiltroAssociazioniRicerca As String = "FiltroAssociazioniRicerca"
            Public Const FiltroDosiRicerca As String = "FiltroDosiRicerca"
            Public Const TipoComunicazione As String = "TipoComunicazione"
            Public Const DataPrenotazioneInizio As String = "DataPrenotazioneInizio"
            Public Const DataPrenotazioneFine As String = "DataPrenotazioneFine"
            Public Const DataSchedulazione As String = "DataSchedulazione"
            Public Const MaxPazientiGiorno As String = "MaxPazientiGiorno"
            Public Const CodiceConsultorioPrenotazione As String = "CodiceConsultorioPrenotazione"
        End Class

    End Class

    Public Class StartBatchPrenotazioneAutomaticaCommand

        Public Property DataNascitaInizio As Date
        Public Property DataNascitaFine As Date
        Public Property Sesso As String
        Public Property DataUltimaConvocazione As Date
        Public Property CodiciConsultoriRicerca As List(Of String)
        Public Property CodiceDistretto As String
        Public Property FiltroAssociazioniRicerca As String     ' Filtro Associazioni-Dosi (stringa separata da |)
        Public Property FiltroDosiRicerca As String             ' Filtro Associazioni-Dosi (stringa separata da |)
        Public Property TipoComunicazione As String
        Public Property DataPrenotazioneInizio As Date
        Public Property DataPrenotazioneFine As Date
        Public Property MaxPazientiGiorno As Integer?
        Public Property CodiceConsultorioPrenotazione As String
        Public Property DataSchedulazione As Date
        Public Property DatiReport As DatiReportPrenotazioneAutomatica

        Public Class DatiReportPrenotazioneAutomatica
            Public Property CodiceCentroVaccinale As String
            Public Property DescrizioneCentroVaccinale As String
            Public Property DataNascitaInizio As Date
            Public Property DataNascitaFine As Date
            Public Property Sesso As String
            Public Property DataUltimaConvocazione As Date
            Public Property CodiceConsultorioPrenotazione As String
            Public Property MaxPazientiGiorno As Integer?
            Public Property DescrizioneStatiAnagrafici As String
            Public Property DataPrenotazioneInizio As DateTime?
            Public Property DataPrenotazioneFine As DateTime?
        End Class

        Public Sub New()
            CodiciConsultoriRicerca = New List(Of String)()
        End Sub

    End Class

    Public Function StartBatchPrenotazioneAutomatica(command As StartBatchPrenotazioneAutomaticaCommand) As BizGenericResult

        Dim message As New Text.StringBuilder()

        ' Se non sono stati specificati i codici dei centri vaccinali su cui cercare, li ricavo dal distretto (se c'è) oppure dalla usl
        Dim listConsultori As List(Of String) = Nothing

        If Not command.CodiciConsultoriRicerca.IsNullOrEmpty() Then

            listConsultori = command.CodiciConsultoriRicerca

        Else

            If Not String.IsNullOrWhiteSpace(command.CodiceDistretto) Then
                ' Centri del distretto specificato
                listConsultori = GenericProvider.Consultori.GetCodiciConsultoriDistretto(command.CodiceDistretto, True)
            Else
                ' Centri della usl specificata
                listConsultori = GenericProvider.Consultori.GetCodiciConsultoriUsl(ContextInfos.CodiceUsl, True)
            End If

        End If

        If listConsultori.IsNullOrEmpty() Then
            message.AppendLine("- Nessun centro vaccinale selezionato")
        End If

        If String.IsNullOrWhiteSpace(ContextInfos.CodiceUsl) Then
            message.AppendLine("- Codice USL corrente non presente")
        End If

        If command.DataPrenotazioneInizio = Date.MinValue OrElse command.DataPrenotazioneFine = Date.MinValue Then
            message.AppendLine("- Periodo di prenotazione non specificato")
        End If

        If command.DataPrenotazioneInizio > command.DataPrenotazioneFine Then
            message.AppendLine("- La data di inizio del periodo di prenotazione non può essere successiva alla data di fine")
        End If

        If command.DataUltimaConvocazione = Date.MinValue Then
            message.AppendLine("- La data limite di convocazione è obbligatoria")
        End If

        ' TODO [App Auto]: controllare altri campi obbligatori o coerenza date o cazzi vari

        If message.Length > 0 Then

            Dim errorMessage As New Text.StringBuilder()
            errorMessage.AppendLine(DESCRIZIONE_PROCESSO_PRENOTAZIONE_AUTOMATICA)
            errorMessage.AppendLine(message.ToString())

            Return New BizGenericResult(False, errorMessage.ToString())

        End If

        If command.DataSchedulazione = Date.MinValue Then command.DataSchedulazione = Date.Now

        If Not command.MaxPazientiGiorno.HasValue Then command.MaxPazientiGiorno = 0

        Dim codiciConsultoriRicerca As String = String.Join("|", listConsultori.ToArray())

        Dim startBatchCommand As New BizBatch.StartBatchProcedureCommand()
        startBatchCommand.ProcedureId = ID_PROCESSO_PRENOTAZIONE_AUTOMATICA
        startBatchCommand.ProcedureDescription = DESCRIZIONE_PROCESSO_PRENOTAZIONE_AUTOMATICA
        startBatchCommand.StartingAppId = ContextInfos.IDApplicazione

        ' TODO [App Auto]: è il codice regionale o quello della specifica usl? 
        startBatchCommand.StartingCodiceAzienda = ContextInfos.CodiceAzienda
        startBatchCommand.StartingUserId = ContextInfos.IDUtente

        ' TODO [App Auto]: questo serve per l'inserimento in t_paz_elaborazioni => da fare dopo la ricerca
        'startBatchCommand.PazientiConvocazioniDaElaborare = cancellazioneCNVcommand.PazientiConvocazioniDaElaborare

        startBatchCommand.ListAppIdConnections.Add(ContextInfos.IDApplicazione)

        'Dim ser As New System.Web.Script.Serialization.JavaScriptSerializer()

        'Dim parameterFiltriProgrammazione As String = String.Empty
        'If Not cancellazioneCNVcommand.FiltriProgrammazione Is Nothing Then
        '    parameterFiltriProgrammazione = ser.Serialize(cancellazioneCNVcommand.FiltriProgrammazione)
        'End If

        'Dim parameterTipoFiltri As String = String.Empty
        'If Not cancellazioneCNVcommand.TipoFiltri Is Nothing Then
        '    parameterTipoFiltri = ser.Serialize(cancellazioneCNVcommand.TipoFiltri)
        'End If

        ' Parametri del processo batch
        startBatchCommand.ListParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                             BatchPrenotazioneAutomatica.ParameterName.CodiceUslCorrente, ContextInfos.CodiceUsl))

        startBatchCommand.ListParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                             BatchPrenotazioneAutomatica.ParameterName.DataNascitaInizio, command.DataNascitaInizio))

        startBatchCommand.ListParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                             BatchPrenotazioneAutomatica.ParameterName.DataNascitaFine, command.DataNascitaFine))

        startBatchCommand.ListParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                             BatchPrenotazioneAutomatica.ParameterName.Sesso, command.Sesso))

        startBatchCommand.ListParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                             BatchPrenotazioneAutomatica.ParameterName.DataUltimaConvocazione, command.DataUltimaConvocazione))

        startBatchCommand.ListParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                             BatchPrenotazioneAutomatica.ParameterName.CodiciConsultoriRicerca, codiciConsultoriRicerca))

        startBatchCommand.ListParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                             BatchPrenotazioneAutomatica.ParameterName.FiltroAssociazioniRicerca, command.FiltroAssociazioniRicerca))

        startBatchCommand.ListParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                             BatchPrenotazioneAutomatica.ParameterName.FiltroDosiRicerca, command.FiltroDosiRicerca))

        startBatchCommand.ListParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                             BatchPrenotazioneAutomatica.ParameterName.TipoComunicazione, command.TipoComunicazione))

        startBatchCommand.ListParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                             BatchPrenotazioneAutomatica.ParameterName.DataPrenotazioneInizio, command.DataPrenotazioneInizio))

        startBatchCommand.ListParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                             BatchPrenotazioneAutomatica.ParameterName.DataPrenotazioneFine, command.DataPrenotazioneFine))

        startBatchCommand.ListParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                             BatchPrenotazioneAutomatica.ParameterName.MaxPazientiGiorno, command.MaxPazientiGiorno.Value))

        startBatchCommand.ListParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
                                             BatchPrenotazioneAutomatica.ParameterName.CodiceConsultorioPrenotazione, command.CodiceConsultorioPrenotazione))

        ' TODO [App Auto]: parametri report
#Region " Parametri report "

        '' Parametri del report
        'startBatchCommand.ListReportParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
        '                                           "Centro vaccinale", String.Format("{0} ({1})", cancellazioneCNVcommand.DatiReport.DescrizioneCentroVaccinale, cancellazioneCNVcommand.DatiReport.CodiceCentroVaccinale)))

        'If cancellazioneCNVcommand.DatiReport.DataNascitaDa.HasValue OrElse cancellazioneCNVcommand.DatiReport.DataNascitaA.HasValue Then
        '    startBatchCommand.ListReportParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
        '                                               "Nascita", Me.GetIntervalloDate(cancellazioneCNVcommand.DatiReport.DataNascitaDa, cancellazioneCNVcommand.DatiReport.DataNascitaA)))
        'End If

        'If cancellazioneCNVcommand.DatiReport.DataConvocazioneDa.HasValue OrElse cancellazioneCNVcommand.DatiReport.DataConvocazioneA.HasValue Then
        '    startBatchCommand.ListReportParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
        '                                               "Convocazione", Me.GetIntervalloDate(cancellazioneCNVcommand.DatiReport.DataConvocazioneDa, cancellazioneCNVcommand.DatiReport.DataConvocazioneA)))
        'End If

        'If Not String.IsNullOrWhiteSpace(cancellazioneCNVcommand.DatiReport.Sesso) Then
        '    startBatchCommand.ListReportParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
        '                                               "Sesso", cancellazioneCNVcommand.DatiReport.Sesso))
        'End If

        'If Not String.IsNullOrWhiteSpace(cancellazioneCNVcommand.DatiReport.DescrizioneStatiAnagrafici) Then
        '    startBatchCommand.ListReportParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
        '                                               "Stati anagrafici", cancellazioneCNVcommand.DatiReport.DescrizioneStatiAnagrafici))
        'End If

        'If Not String.IsNullOrWhiteSpace(cancellazioneCNVcommand.DatiReport.DescrizioneMalattia) Then
        '    startBatchCommand.ListReportParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
        '                                               "Malattia", cancellazioneCNVcommand.DatiReport.DescrizioneMalattia))
        'End If

        'If Not String.IsNullOrWhiteSpace(cancellazioneCNVcommand.DatiReport.DescrizioneCategoriaRischio) Then
        '    startBatchCommand.ListReportParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
        '                                               "Categoria rischio", cancellazioneCNVcommand.DatiReport.DescrizioneCategoriaRischio))
        'End If

        '' Filtro programmazione (cicli/associazioni/vaccinazioni)
        'If Not command.FiltriProgrammazione Is Nothing Then

        '    Dim parametro As KeyValuePair(Of String, String) =
        '        GetNomeValoreFiltroProgrammazione(command.FiltriProgrammazione, command.TipoFiltri)

        '    startBatchCommand.ListReportParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
        '                                               parametro.Key, parametro.Value))

        'End If

        'startBatchCommand.ListReportParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
        '                                           "Cancella intera convocazione", Me.GetReportParameterBooleanValue(cancellazioneCNVcommand.CancellaInteraConvocazione)))

        'startBatchCommand.ListReportParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
        '                                           "Cancella anche convocazioni con sollecito", Me.GetReportParameterBooleanValue(cancellazioneCNVcommand.CancellaConSolleciti)))

        'startBatchCommand.ListReportParameters.Add(New BizBatch.StartBatchProcedureCommand.BatchProcedureParameter(
        '                                           "Cancella anche convocazioni con appuntamento", Me.GetReportParameterBooleanValue(cancellazioneCNVcommand.CancellaConAppuntamenti)))
#End Region

        Dim bizBatch As New BizBatch(GenericProvider)

        Return bizBatch.StartBatchProcedure(startBatchCommand)

    End Function

    Private Function GetNomeValoreFiltroProgrammazione(filtriProgrammazione As List(Of FiltroProgrammazione), tipoFiltri As TipoFiltriProgrammazione) As KeyValuePair(Of String, String)

        Dim nomeParametro As String = String.Empty
        Select Case tipoFiltri
            Case TipoFiltriProgrammazione.AssociazioneDose
                nomeParametro = "Associazioni-dosi selezionate"
            Case TipoFiltriProgrammazione.CicloSeduta
                nomeParametro = "Cicli-sedute selezionati"
            Case TipoFiltriProgrammazione.VaccinazioneDose
                nomeParametro = "Vaccinazioni-dosi selezionate"
        End Select

        Dim valoreParametro As New Text.StringBuilder()

        For Each filtro As FiltroProgrammazione In filtriProgrammazione

            If Not String.IsNullOrWhiteSpace(filtro.Codice) Then
                valoreParametro.Append(filtro.Codice)
            End If

            If filtro.Valore.HasValue Then

                If String.IsNullOrWhiteSpace(filtro.Codice) Then
                    valoreParametro.Append(filtro.Valore.Value.ToString())
                Else
                    valoreParametro.AppendFormat("({0})", filtro.Valore.Value.ToString())
                End If

            End If

            valoreParametro.Append("; ")
        Next

        If valoreParametro.Length > 0 Then
            valoreParametro.Remove(valoreParametro.Length - 2, 2)
        End If

        Return New KeyValuePair(Of String, String)(nomeParametro, valoreParametro.ToString())

    End Function



    Public Event RefreshTotaleElementiDaElaborare(e As BizBatch.RefreshTotaleElementiDaElaborareEventArgs)
    Public Event RefreshParzialeElementiElaborati(e As BizBatch.RefreshParzialeElementiElaboratiEventArgs)

    Public Class CoppiaElementi

        Public Property Codice As String
        Public Property Numero As String

        Public Sub New()
        End Sub

        Public Sub New(codice As String, numero As String)
            Me.Codice = codice
            Me.Numero = numero
        End Sub

    End Class




    Public Class PrenotazioneAutomaticaCommand

        Public Property DataInserimentoPrenotazioni As Date

        'Public Property DataNascitaInizio As Date
        'Public Property DataNascitaFine As Date
        'Public Property DataConvocazioneFine As Date
        'Public Property ConsultorioPrenotazione As String


        Public Property Filtri As FiltriGestioneAppuntamenti

        Public Sub New()
            Filtri = New FiltriGestioneAppuntamenti()
        End Sub

    End Class


    Public Function PrenotazioneAutomaticaAppuntamenti(command As PrenotazioneAutomaticaCommand) As BizGenericResult

        Dim result As New BizGenericResult()

        RaiseEvent RefreshTotaleElementiDaElaborare(New BizBatch.RefreshTotaleElementiDaElaborareEventArgs(100))

        ' TODO [App Auto]: avvio calcolo cnv? ==> NOOO!!!! 

        Return result

    End Function

    Private Class FiltroMattinaPomeriggioObbligatorio
        Inherits Filtri.FiltroMattinaPomeriggio

        Public Overrides ReadOnly Property Obbligatorio() As Boolean
            Get
                Return True
            End Get
        End Property

    End Class



    ' *********************************************************** '
    ' TODO [App Auto]: da fare prenotazione automatica, presa da GestioneAppuntamenti e richiamata da OnBatch

#Region " DA FARE "

    ' Riempie dtaRicerca con tutti i dati delle convocazioni trovate
    Private Function Cerca(filtri As FiltriGestioneAppuntamenti, filtroAssociazioniRicerca As String, filtroDosiRicerca As String) As DataTable

        ' TODO [App Auto]: controllare carattere separatore
        Dim cns As String() = filtri.ConsultoriSelezionati.Split("|")

        Dim dtaRicerca As DataTable = CercaConvocazioni(cns.ToList(), filtri)

        ' --------------------------- OPERAZIONI SUL DATATABLE DEI RISULTATI --------------------------- '

        Dim rigaDaCancellare As Boolean

        Dim i As Integer = 0

        While i < dtaRicerca.Rows.Count

            rigaDaCancellare = False

            ' Toglie da dtaRicerca le convocazioni che hanno appuntamento
            If Not dtaRicerca.Rows(i).IsNull("CNV_DATA_APPUNTAMENTO") Then
                ' Tira fuori troppa roba
                '--MC--'If Not LoadedDates.Contains(CType(dtaRicerca.Rows(i).Item("CNV_DATA_APPUNTAMENTO"), DateTime).Date) Then
                rigaDaCancellare = True
                '--MC--''End If
            End If

            ' --- Filtro associazioni-dosi --- '
            If Not rigaDaCancellare Then

                ' Valori di associazioni e dosi per il paziente corrente, trovati dalla ricerca effettuata
                Dim associazioneRisultato As String = dtaRicerca.Rows(i)("ASSOCIAZIONI").ToString()
                Dim doseRisultato As String = dtaRicerca.Rows(i)("DOSI").ToString()

                ' Se non è stato impostato il filtro non controllo
                If Not String.IsNullOrWhiteSpace(filtroAssociazioniRicerca) OrElse Not String.IsNullOrWhiteSpace(filtroDosiRicerca) Then

                    ' Controllo se almeno una tra le accoppiate associazione-dose del risultato è presente tra quelle  
                    ' dei filtri associazioni-dosi specificati: se non ce n'è nessuna, la riga è da cancellare.
                    rigaDaCancellare = Not CheckElementiPresenti(filtroAssociazioniRicerca, filtroDosiRicerca, associazioneRisultato, doseRisultato)

                End If
            End If

            ' --- MCH => qui no
            '' --- Filtro cicli-sedute --- '
            'If Not rigaDaCancellare Then

            '    ' Valori di cicli e sedute per il paziente corrente, trovati dalla ricerca effettuata
            '    Dim cicloRisultato As String = dtaRicerca.Rows(i)("CICLI").ToString()
            '    Dim sedutaRisultato As String = dtaRicerca.Rows(i)("SEDUTE").ToString()

            '    ' Se non è stato impostato il filtro non controllo
            '    If Not String.IsNullOrEmpty(cicloFiltro) OrElse Not String.IsNullOrEmpty(sedutaFiltro) Then

            '        ' Controllo se almeno una tra le accoppiate ciclo-seduta del risultato è presente tra quelle 
            '        ' dei filtri cicli-sedute specificati: se non ce n'è nessuna, la riga è da cancellare.
            '        rigaDaCancellare = Not CheckElementiPresenti(cicloFiltro, sedutaFiltro, cicloRisultato, sedutaRisultato)

            '    End If

            'End If

            If rigaDaCancellare Then
                dtaRicerca.Rows.Remove(dtaRicerca.Rows(i))
                i -= 1
            End If

            i += 1

        End While

        '--- MCH ---'
        '' SIO 29/03/2004
        '' Questo è da sistemare, perché non si può togliere, ma così non va bene:
        '' toglie dalla ricerca delle convocazioni anche quelli che sono in ritardo e a cui devo assegnare
        '' il nuovo appuntamento
        'For i = 0 To dtaMattino.Rows.Count - 1
        '    If dtaRicerca.Rows.Count <> 0 Then
        '        row = dtaRicerca.Rows.Find(New Object() {dtaMattino.Rows(i)("PAZ_CODICE"), dtaMattino.Rows(i)("CNV_DATA")})
        '    Else
        '        row = Nothing
        '    End If
        '    If Not row Is Nothing Then
        '        If Not dtaMattino.Rows(i).IsNull("CNV_DATA_APPUNTAMENTO") Then
        '            dtaRicerca.Rows.Remove(row)
        '        End If
        '    End If
        'Next i
        'For i = 0 To dtaPomeriggio.Rows.Count - 1
        '    If dtaRicerca.Rows.Count <> 0 Then
        '        row = dtaRicerca.Rows.Find(New Object() {dtaPomeriggio.Rows(i)("PAZ_CODICE"), dtaPomeriggio.Rows(i)("CNV_DATA")})
        '    Else
        '        row = Nothing
        '    End If
        '    If Not row Is Nothing Then
        '        If Not dtaPomeriggio.Rows(i).IsNull("CNV_DATA_APPUNTAMENTO") Then
        '            dtaRicerca.Rows.Remove(row)
        '        End If
        '    End If
        'Next i
        '' fine SIO 29/03/2004

        'ImpaginaDataListRicercaConvocati(1)

        ''Caricamento dei dati nel dataset per la stampa
        'dts_Ricerca.T_RICERCA.Clear()

        'For i = 0 To dtaRicerca.Rows.Count - 1
        '    dts_Ricerca.T_RICERCA.ImportRow(dtaRicerca.Rows(i))
        'Next

        '' --------------------------- FINE OPERAZIONI SUL DATATABLE DEI RISULTATI --------------------------- '

        'Dim str As String = ""
        'If Settings.TIPOANAG <> TipoAnags.SoloLocale AndAlso Settings.AUTOALLINEA Then
        '    str = " (Pazienti allineati: " & pazientiAllineati & ")"
        'End If

        'lblRis.Text = "RISULTATI RICERCA (Convocazioni trovate: " & Me.dtaRicerca.Rows.Count & ") " & str
        '--- MCH ---'

        Return dtaRicerca

    End Function

    'Private Function GetFiltriRicerca() As FiltriGestioneAppuntamenti

    '    Dim f As New FiltriGestioneAppuntamenti()

    '    ' Tab Appuntamenti
    '    f.ConsultoriSelezionati = ucSelezioneConsultori.GetConsultoriSelezionati(False).Replace(", ", "|")
    '    f.StatiAnagrafici = ucStatiAnagrafici.GetListStatiAnagraficiSelezionati()

    '    ' Se la data di ultima convocazione è vuota, la imposta ad oggi
    '    If txtFinoAData.Text = "" Then
    '        f.FinoAData = Date.Now
    '    Else
    '        f.FinoAData = txtFinoAData.Data
    '    End If

    '    f.malattia = omlMalattia.Codice
    '    f.chkObb = chkTipoVaccObbligatoria.Checked
    '    f.chkFac = chkTipoVaccFacoltativa.Checked
    '    f.chkRac = chkTipoVaccRaccomandata.Checked
    '    f.chkNonExtracomPrima = chkImmigratiNonExtracomPrimaVolta.Checked
    '    f.chkExtracom = chkImmigratiExtracom.Checked
    '    f.chkCronici = chkCronici.Checked
    '    f.chkSoloRitardatari = chkSoloRitardatari.Checked
    '    f.chkDataSingola = chkDataSingola.Checked

    '    If txtDaDataNascita.Text <> "" Then
    '        f.DaDataNascita = txtDaDataNascita.Data
    '    End If
    '    If txtADataNascita.Text <> "" Then
    '        f.ADataNascita = txtADataNascita.Data
    '    End If

    '    f.fmMedCodice = txtMedico.Codice
    '    f.categ_rischio = omlCategorieRischio.Codice
    '    f.sesso = ddlSesso.SelectedValue
    '    f.codiceComune = fmComune.Codice
    '    f.vaccinazioniTutteEscluse = chkEscluse.Checked
    '    f.dtFiltroAssociazioniSel = UscFiltroAssociazioniDosi.getValoriSelezionatiFiltro1()
    '    f.dtFiltroDosiSel = UscFiltroAssociazioniDosi.getValoriSelezionatiFiltro2()
    '    f.dtFiltroCicliSel = UscFiltroCicliSedute.getValoriSelezionatiFiltro1()
    '    f.dtFiltroSeduteSel = UscFiltroCicliSedute.getValoriSelezionatiFiltro2()

    '    ' Tab Opzioni
    '    f.chkRicConvocazioni = chkRicConvocazioni.Checked
    '    f.chkRicCiclo = chkRicCiclo.Checked
    '    f.chkRicMedico = chkRicMedico.Checked
    '    f.chkRicBilancio = chkRicBilancio.Checked
    '    f.chkOrdineAlfabeticoRicerca = chkOrdineAlfabeticoRicerca.Checked

    '    f.txtNumPazientiAlGiorno = txtNumPazientiAlGiorno.Text
    '    f.txtNumNuoviPazientiAlGiorno = txtNumNuoviPazientiAlGiorno.Text
    '    f.txtDurata = txtDurata.Text
    '    f.chkOrdineAlfabeticoPrenotazione = chkOrdineAlfabeticoPrenotazione.Checked

    '    ' Fieldset Prenotazione
    '    f.odpDataInizPrenotazioni = odpDataInizPrenotazioni.Data
    '    f.odpDataFinePrenotazioni = odpDataFinePrenotazioni.Data
    '    f.chkOrariPers = chkOrariPers.Checked
    '    f.chkSovrapponiRit = chkSovrapponiRit.Checked
    '    f.chkFiltroPomeriggioObbligatorio = chkFiltroPomeriggioObbligatorio.Checked

    '    Return f

    'End Function




    ' Restituisce una lista di elementi contenente le coppie di elementi 1 e 2 con elemento1 splittato in base a "|"
    ' e elemento2 splittato prima in base a "|" poi in base a ",".
    ' Ad esempio, se i due parametri sono: elemento1 = "A|B|C" e elemento2 = "1,2|3|1,5", la lista di elementi che
    ' si ottiene è la seguente: [A;1] [A;2] [B;3] [C;1] [C;5]
    ' Se elemento2 è nullo, la lista conterrà coppie in cui è valorizzato il solo elemento1, o viceversa.
    ' Ad esempio se elemento1 = "A|B|C" e elemento2 = "", si ottiene la lista [A;] [B;] [C;]
    ' Se elemento1 = "" e elemento2 = "1|2|3|4", si ottiene la lista [;1] [;2] [;3] [;4]
    Public Shared Function SetListaElementi(elemento1 As String, elemento2 As String) As List(Of CoppiaElementi)

        Dim list As New List(Of CoppiaElementi)()

        Dim array1() As String = elemento1.Split("|")
        Dim array2() As String = elemento2.Split("|")

        If String.IsNullOrEmpty(elemento1) Then

            ' Elemento1 nullo, considero solo Elemento2

            For idxElemento2 As Integer = 0 To array2.Length - 1

                If Not String.IsNullOrEmpty(array2(idxElemento2).Trim()) Then

                    list.Add(New CoppiaElementi(String.Empty, array2(idxElemento2).Trim()))

                End If

            Next

        ElseIf String.IsNullOrEmpty(elemento2) Then

            ' Elemento2 nullo, considero solo Elemento1

            For idxElemento1 As Integer = 0 To array1.Length - 1

                If Not String.IsNullOrEmpty(array1(idxElemento1).Trim()) Then

                    list.Add(New CoppiaElementi(array1(idxElemento1).Trim(), String.Empty))

                End If

            Next

        Else
            ' Entrambi gli elementi non nulli

            For idxElemento1 As Integer = 0 To array1.Length - 1

                If Not String.IsNullOrEmpty(array1(idxElemento1).Trim()) Then

                    If String.IsNullOrEmpty(array2(idxElemento1).Trim()) Then

                        list.Add(New CoppiaElementi(array1(idxElemento1).Trim(), String.Empty))

                    Else

                        Dim arrayElementi2() As String = array2(idxElemento1).Split(",")

                        For idxElemento2 As Integer = 0 To arrayElementi2.Length - 1

                            list.Add(New CoppiaElementi(array1(idxElemento1).Trim(), arrayElementi2(idxElemento2).Trim()))

                        Next

                    End If

                End If

            Next

        End If

        Return list

    End Function

    ' Restituisce true se almeno una coppia (Elemento1;Elemento2) di risultati è presente tra i filtri.
    ' Se i filtri non sono impostati, restituisce false
    Public Shared Function CheckElementiPresenti(elemento1Filtro As String, elemento2Filtro As String, elemento1Risultato As String, elemento2Risultato As String) As Boolean

        Dim elementiPresenti As Boolean = False

        ' Elementi impostati nel filtro associazioni-dosi
        Dim listElementiFiltro As List(Of CoppiaElementi) = SetListaElementi(elemento1Filtro, elemento2Filtro)

        If Not listElementiFiltro Is Nothing AndAlso listElementiFiltro.Count > 0 Then

            ' Elementi associazioni-dosi ottenuti come risultato per il paziente corrente
            Dim listElementiRisultato As List(Of CoppiaElementi) = SetListaElementi(elemento1Risultato, elemento2Risultato)

            ' Controllo che almeno una coppia tra quelle dei risultati sia presente anche nei filtri.
            For Each coppia As CoppiaElementi In listElementiFiltro

                Dim coppiaFiltro As CoppiaElementi = coppia

                ' Risultati che hanno stesso codice e stesso numero rispetto a quelli impostati nel filtro 
                ' oppure stesso codice e numero non specificato nel filtro
                ' oppure stesso numero e codice non specificato nel filtro.
                Dim listRisultatiFiltrati As List(Of CoppiaElementi) =
                    (From coppiaRisultato As CoppiaElementi In listElementiRisultato
                     Where (coppiaRisultato.Codice = coppiaFiltro.Codice And coppiaRisultato.Numero = coppiaFiltro.Numero) _
                     Or (coppiaRisultato.Codice = coppiaFiltro.Codice And String.IsNullOrEmpty(coppiaFiltro.Numero)) _
                     Or (String.IsNullOrEmpty(coppiaFiltro.Codice) And coppiaRisultato.Numero = coppiaFiltro.Numero)
                     Select coppiaRisultato).ToList()

                ' Se ho trovato almeno un risultato che soddisfa il filtro, non continuo la ricerca
                If Not listRisultatiFiltrati Is Nothing AndAlso listRisultatiFiltrati.Count > 0 Then
                    elementiPresenti = True
                    Exit For
                End If

            Next

        End If

        Return elementiPresenti

    End Function


    'Private Sub AggiungiAppuntamentoAutomatico(codiceConsultorioCorrente As String, codiceAmbulatorio As Integer, dtaRicerca As DataTable, dataInizioPrenotazioni As Date, dataFinePrenotazioni As Date, numeroMassimoNuovi As Integer, numeroNuovi As Integer, durataAppuntamento As Integer, filtroPomeriggioObbligatorio As Boolean)

    '    ' --- MCH --- '
    '    ' TODO [App Auto]: controllare valorizzazione date di prenotazione all'inizio, non qui
    '    'If dataInizioPrenotazioni = Date.MinValue OrElse dataFinePrenotazioni = Date.MinValue Then
    '    '    WarningMessage(String.Empty, "Valorizzare le date di inizio e fine appuntamento.", TipoOperazione.Info)
    '    '    Exit Sub
    '    'End If

    '    Dim dataMinima As Date = dataInizioPrenotazioni.Date
    '    Dim dataMassima As Date = dataFinePrenotazioni.Date


    '    ' TODO [App Auto]: overbooking e fuori orario => li metterei sempre false
    '    'Dim overbooking As Boolean = Me.chkSovrapponiRit.Checked
    '    'Dim fuoriOrario As Boolean = Me.cnsControl
    '    Dim overbooking As Boolean = False
    '    Dim fuoriOrario As Boolean = False

    '    ' TODO [App Auto]: numeroMassimoNuovi e numeroNuovi => vanno controllati all'inizio, qui dò per scontato che siano numeri
    '    'Dim numeroMassimoNuovi As String = Me.txtNumPazientiAlGiorno.Text
    '    'Dim numeroNuovi As String = Me.txtNumNuoviPazientiAlGiorno.Text

    '    ' TODO [App Auto]: controllare che l'ambulatorio esista e faccia parte del centro 
    '    ' TODO [App Auto]: qui fa il controllo che l'amb sia del cns => da fare all'inizio 
    '    'AddHandler ambulatorioCalcoloAppuntamenti.OnPreRicercaAppuntamentoLiberoPaziente, AddressOf OnPreRicercaAppuntamentoLiberoPaziente 
    '    ' Dim codiciAmbulatoriConsultorioCorrente As List(Of Integer) = Nothing
    '    'codiciAmbulatoriConsultorioCorrente = bizConsultori.GetCodiciAmbulatori(codiceConsultorioCorrente, True)


    '    Dim ambulatorio As Entities.Ambulatorio = Nothing

    '    Using bizConsultori As New BizConsultori(GenericProvider, Settings, ContextInfos, LogOptions)

    '        ambulatorio = bizConsultori.GetAmbulatorio(codiceAmbulatorio)

    '    End Using

    '    ' TODO [App Auto]: fisso ordine alfabetico a true per pazienti da prenotare
    '    Dim consultorioCalcoloAppuntamenti As New CalcoloAppuntamenti.Consultorio()
    '    consultorioCalcoloAppuntamenti.Pazienti.UsaOrdineAlfabetico = True

    '    If dataMinima < DateTime.Today.Date Then dataMinima = DateTime.Today.Date
    '    consultorioCalcoloAppuntamenti.DaData = dataMinima
    '    consultorioCalcoloAppuntamenti.AData = dataMassima


    '    Dim ambulatorioCalcoloAppuntamenti As New CalcoloAppuntamenti.Ambulatorio(codiceAmbulatorio, overbooking, consultorioCalcoloAppuntamenti)
    '    ambulatorioCalcoloAppuntamenti.DataAperturaAmbulatorio = ambulatorio.DataApertura
    '    ambulatorioCalcoloAppuntamenti.DataChiusuraAmbulatorio = ambulatorio.DataChiusura

    '    If numeroMassimoNuovi > 0 Then

    '        Dim fmax As New Filtri.FiltroNumeroMassimoAppuntamentiGiornalieri()
    '        ambulatorioCalcoloAppuntamenti.FiltriVariabili.SetParam(Filtri.FiltroNumeroMassimoAppuntamentiGiornalieri.NumeroMassimoPrenotatiGiornaliero, numeroMassimoNuovi)
    '        ambulatorioCalcoloAppuntamenti.FiltriVariabili.Add(fmax)

    '    End If

    '    '-- aggiunta del filtro per aggiungere N nuovi pazienti al giorno oltre a quelli eventualmente già presenti
    '    If numeroNuovi > 0 Then

    '        Dim fnuovi As New Filtri.FiltroNumeroMassimoNuoviAppuntamentiGiornalieri()
    '        ambulatorioCalcoloAppuntamenti.FiltriVariabili.SetParam(Filtri.FiltroNumeroMassimoNuoviAppuntamentiGiornalieri.NumeroMassimoNuoviPrenotatiGiornaliero, numeroNuovi)
    '        ambulatorioCalcoloAppuntamenti.FiltriVariabili.Add(fnuovi)

    '    End If

    '    Dim numAppRitardi As Integer = Settings.N_RITARDATARI
    '    If numAppRitardi = 0 Then numAppRitardi = 5

    '    ambulatorioCalcoloAppuntamenti.FiltriVariabili.SetParam(Filtri.FiltroNumeroMassimoRitardatariGiornalieri.NumeroMassimoRitardatariGiornaliero, numAppRitardi)

    '    consultorioCalcoloAppuntamenti.Ambulatori.Add(ambulatorioCalcoloAppuntamenti)

    '    Dim pazCollection As CnvPazienteCollection = RestituisciPazienti(dtaRicerca, overbooking, False, durataAppuntamento, filtroPomeriggioObbligatorio)

    '    If pazCollection.Count = 0 Then
    '        ' TODO [App Auto]: decidere come gestire la mancanza di pazienti
    '        'WarningMessage(String.Empty, "Nessuna convocazione selezionata!", TipoOperazione.Info)
    '        Exit Sub
    '    End If

    '    For Each paz As CnvPaziente In pazCollection
    '        consultorioCalcoloAppuntamenti.Pazienti.Add(paz)
    '    Next

    '    Dim filtriOcc As Filtri.FiltroCollection = CaricaPrenotati(overbooking, dataMinima, dataMassima)
    '    For Each f As Filtri.IFiltro In filtriOcc
    '        ambulatorioCalcoloAppuntamenti.FiltriVariabili.Add(f)
    '    Next

    '    ' --------- Modifica 7.12.2006 --------- '
    '    ' La funzione di impostazione dei filtri per la procedura di prenotazione degli appuntamenti è stata modificata per poter prevedere 
    '    ' gli orari personalizzati (numero variabile di blocchi ogni giorno, con diverse durate e diverso numero di pazienti prenotabili al massimo). 
    '    ' E' stato creato anche un nuovo tipo di filtro nel CalcoloAppuntamenti, per gestire questa situazione.
    '    ' I nuovi filtri creati, se presenti, vengono aggiunti ai FiltriVariabili dell'ambulatorio. 
    '    ' Deve essere anche impostata la proprietà NumeroMassimoBlocchiPersonalizzatiGiornalieri per far sì che la procedura sia più performante.
    '    '--
    '    ' N.B. : I filtri ragionano al contrario (impostano le indisponibilità)
    '    '--
    '    Dim filtriBlocchi As New ArrayList()

    '    Dim numBlocchiGiorno As Integer

    '    Dim filtriOrari As Filtri.FiltroCollection = Me.CaricaFiltriApertura(fuoriOrario, -1, PartiGiornata.Entrambi, filtriBlocchi, numBlocchiGiorno)

    '    For Each f As Filtri.IFiltro In filtriOrari
    '        ambulatorioCalcoloAppuntamenti.FiltriAmbulatorio.Add(f)
    '    Next

    '    If Not IsNothing(filtriBlocchi) AndAlso filtriBlocchi.Count > 0 Then

    '        For idxBlocchi As Int16 = 0 To filtriBlocchi.Count - 1
    '            ambulatorioCalcoloAppuntamenti.FiltriVariabili.SetParam(filtriBlocchi(idxBlocchi)(0), filtriBlocchi(idxBlocchi)(1))
    '        Next
    '        ambulatorioCalcoloAppuntamenti.FiltriVariabili.NumeroMassimoBlocchiPersonalizzatiGiornalieri = numBlocchiGiorno

    '        Dim f As New Filtri.FiltroNumeroMassimoAppuntamentiBlocco()
    '        ambulatorioCalcoloAppuntamenti.FiltriVariabili.Add(f)

    '    End If
    '    ' -------------------------------------- '

    '    Dim filtriFesta As Filtri.FiltroCollection = Me.CaricaFiltriFeste()
    '    For Each f As Filtri.IFiltro In filtriFesta
    '        ambulatorioCalcoloAppuntamenti.FiltriAmbulatorio.Add(f)
    '    Next

    '    consultorioCalcoloAppuntamenti.FindAppuntamenti()

    '    Dim numeroPrenotati As Integer = 0

    '    Using dam As IDAM = OnVacUtility.OpenDam()

    '        For Each paz As CnvPaziente In consultorioCalcoloAppuntamenti.Pazienti

    '            If paz.EsitoProcedura.Successo Then

    '                Dim isRitardatarioReale As Boolean = paz.Info("RitardatarioReale")

    '                Me.CreaAppuntamento(dam, paz.Codice, paz.DataConvocazione, paz.EsitoProcedura.DataAppuntamento,
    '                                    paz.DurataAppuntamento, String.Empty, Constants.TipoPrenotazioneAppuntamento.Automatica,
    '                                    paz.Ritardatario, paz.SoloBilancio, isRitardatarioReale)

    '                numeroPrenotati += 1

    '            End If

    '        Next

    '    End Using

    '    Me.Cerca(False)

    '    If Me.CaricaCalendario() Then

    '        Me.AggiungiAlLogTemporaneo(consultorioCalcoloAppuntamenti, TipoOperazione.Prenotazione_Automatica)

    '        ' Se sono state effettuate prenotazioni scrivo un messaggio col numero di pazienti prenotati con l'ultimo "Prenota"
    '        If numeroPrenotati > 0 Then
    '            WarningMessage(String.Empty, "Numero pazienti prenotati: " & numeroPrenotati & ".", TipoOperazione.Info)
    '        Else
    '            WarningMessage(String.Empty, "Nessun paziente prenotato.", TipoOperazione.Info)
    '        End If

    '    End If

    'End Sub

    'Function RestituisciPazienti(dtaRicerca As DataTable, overbooking As Boolean, calcoloManuale As Boolean, durataAppuntamento As Integer, filtroPomeriggioObbligatorio As Boolean) As CnvPazienteCollection

    '    Dim retPazienti As New CnvPazienteCollection()

    '    Dim consideraSoloBilancio As Boolean

    '    For Each r As DataRow In dtaRicerca.Rows

    '        ' TODO [App Auto]: controllare cos'è "SEL"
    '        If r("SEL") = "S" Then

    '            'il massimo numero di solleciti deve essere recuperato dal parametro
    '            Dim isRitardatario As Boolean = overbooking AndAlso r("SOLLECITO") > 0 AndAlso Not Boolean.Parse(r("TP"))
    '            Dim isRitardatarioReale As Boolean = r("SOLLECITO") > 0 AndAlso Not Boolean.Parse(r("TP"))

    '            ' -- Gestione dei solo bilanci 20 minuti --
    '            ' Le convocazioni per solo bilancio devono avere sempre una durata di 0 minuti, mentre prima ai solo bilancio senza pediatra veniva dato un appuntamento di 20 minuti
    '            ' Se è un solo bilancio
    '            consideraSoloBilancio = Not IsDBNull(r("SOLOBILANCIO"))

    '            Dim paz As New CnvPaziente(r("PAZ_CODICE"), r("CNV_DATA"), consideraSoloBilancio, isRitardatario)

    '            paz.Info.Add("RitardatarioReale", isRitardatarioReale)

    '            paz.Ambulatorio = AmbCodice

    '            paz.Cognome = r("PAZ_COGNOME")
    '            paz.Nome = r("PAZ_NOME")
    '            paz.DataConvocazione = r("CNV_DATA")
    '            paz.DataNascita = r("PAZ_DATA_NASCITA")

    '            ' TODO [App Auto]: durata appuntamento => da controllare all'inizio, qui dò per scontato che sia integer
    '            'Dim durata As String = Me.txtDurata.Text

    '            'If IsNumeric(durata) AndAlso durata > 0 Then
    '            If durataAppuntamento > 0 Then

    '                paz.DurataAppuntamento = durataAppuntamento

    '            ElseIf Settings.SED_AUTO > 0 Then

    '                If r("TEMPO_BIL") = "N" Then
    '                    ' Se non è un bilancio senza pediatra
    '                    paz.DurataAppuntamento = Settings.SED_AUTO
    '                Else
    '                    paz.DurataAppuntamento = r("CNV_DURATA_APPUNTAMENTO")
    '                End If

    '            Else

    '                paz.DurataAppuntamento = r("CNV_DURATA_APPUNTAMENTO")

    '            End If

    '            paz.Priorita = r("MASSIMO")

    '            Dim filValidita As New Filtri.FiltroDataValidita()
    '            filValidita.DataCnv = r("CNV_DATA")

    '            If Settings.GESDATAVALIDITA And r.IsNull("SOLOBILANCIO") Then

    '                If calcoloManuale Then
    '                    filValidita.DataValidita = Date.MaxValue
    '                Else
    '                    filValidita.DataValidita = r("MASSIMO")
    '                End If

    '            Else

    '                filValidita.DataValidita = Date.MaxValue

    '            End If

    '            paz.Preferenze.Add(filValidita)


    '            If Not calcoloManuale Then

    '                Dim eta As Integer = Math.Floor(Date.Now.Subtract(r("PAZ_DATA_NASCITA")).TotalDays / 365)

    '                If Settings.APPETAPM > 0 AndAlso eta >= Settings.APPETAPM Then

    '                    Dim oraPm As Object = Settings.ORAPM

    '                    If Not oraPm Is Nothing Then

    '                        Dim filtroPomeriggio As Filtri.FiltroMattinaPomeriggio

    '                        ' TODO [App Auto]: controllare se serve un parametro per chkFiltroPomeriggioObbligatorio
    '                        'If chkFiltroPomeriggioObbligatorio.Checked Then
    '                        If filtroPomeriggioObbligatorio Then
    '                            filtroPomeriggio = New FiltroMattinaPomeriggioObbligatorio()
    '                        Else
    '                            filtroPomeriggio = New Filtri.FiltroMattinaPomeriggio()
    '                        End If

    '                        filtroPomeriggio.InizioPomeriggio = Date.Parse(oraPm)
    '                        filtroPomeriggio.MattinaPomeriggio = Filtri.MattinaPomeriggio.Pomeriggio

    '                        paz.Preferenze.Add(filtroPomeriggio)

    '                    End If

    '                End If

    '                If Not (r("PAZ_GIORNO") Is DBNull.Value OrElse r("PAZ_GIORNO") = 0) Then

    '                    Dim giornoPreferenza As New Filtri.FiltroGiornoPreferenza(r("PAZ_GIORNO"))
    '                    paz.Preferenze.Add(giornoPreferenza)

    '                End If

    '            End If

    '            retPazienti.Add(paz)

    '        End If

    '    Next

    '    Return retPazienti

    'End Function

#End Region

#End Region

End Class
