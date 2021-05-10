Imports System.Collections.Generic
Imports System.Collections.ObjectModel

Imports Onit.Database.DataAccessManager

Imports Onit.OnAssistnet.OnVac.CalcoloAppuntamenti
Imports Onit.OnAssistnet.OnVac.Filters
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Enumerators


Partial Class GestioneAppuntamenti
    Inherits OnVac.Common.PageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(sender As System.Object, e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Enums "

    Enum PartiGiornata
        Mattina
        Pomeriggio
        Entrambi
    End Enum

    Private Enum stato_pagina
        seleziona_ambulatorio
        ricarica_calendario
        stabile_iterativo
    End Enum

    ''' <summary>
    ''' Stabilisce il tipo di operazione effettuato nella maschera per la visualizzazione dei messaggi
    ''' </summary>
    ''' <remarks></remarks>
    Private Enum TipoOperazione As Int16
        Pulisci
        Salvataggio
        Info
        Errore
        Alert
        Rimozione_Appuntamento
        Prenotazione_Automatica
        Prenotazione_Manuale
        Spostamento_Appuntamento
    End Enum

#End Region

#Region " Costanti "

    ReadOnly NUMEROMASSIMOITERAZIONI = 1000
    ReadOnly INFO = "<img src='" + VirtualPathUtility.ToAbsolute("~/images/info.gif") + "' align = 'absmiddle' />&nbsp;"
    ReadOnly ERRORE = "<img src='" + VirtualPathUtility.ToAbsolute("~/images/annulla.gif") + "' align = 'absmiddle' />&nbsp;"
    ReadOnly ALERTICO = "<img src='" + VirtualPathUtility.ToAbsolute("~/images/avvertimento.gif") + "' align = 'absmiddle' />&nbsp;"
    ReadOnly CONFERMA = "<img src='" + VirtualPathUtility.ToAbsolute("~/images/conferma.gif") + "' align = 'absmiddle' />&nbsp;"

    ' Massimo numero di righe in ogni datagrid della modale dei solleciti
    Const MAX_ELEMENT_DGR_RIEPILOGO As Int16 = 100

    Private Const ICONA_STATO_SOLLECITO As String = "../../Images/stato_sollecito.gif"
    Private Const ICONA_STATO_SOLLECITO_STANDARD As String = "../../Images/stato_sollecito_standard.gif"
    Private Const ICONA_STATO_ESCLUSIONE_STANDARD As String = "../../Images/stato_esclusione_standard.gif"
    Private Const ICONA_STATO_TERMINE_PERENTORIO As String = "../../Images/stato_termine_perentorio.gif"
    Private Const ICONA_STATO_POSTICIPO_GIORNI As String = "../../Images/stato_posticipo_giorni.gif"
    Private Const ICONA_STATO_POSTICIPO_SEDUTA As String = "../../Images/stato_posticipo_seduta.gif"

    Private Const NUM_PAZIENTI_PAGINA_RICERCA_CONVOCATI As Integer = 30

    Private Class TipoPrenotazione
        Public Const APPUNTAMENTO As String = "N"       ' "Normale"...
        Public Const RITARDO As String = "R"
        Public Const BILANCIO As String = "B"
    End Class

#End Region

#Region " Variabili private "

    Private _unSoloAmbulatorio As Boolean

#End Region

#Region " Proprietà pubbliche "

    Public ReadOnly Property Solleciti() As Common.Solleciti.ControlloSolleciti
        Get
            If Session("Solleciti") Is Nothing Then
                ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
                Session("Solleciti") = New Common.Solleciti.ControlloSolleciti(Settings, FlagConsensoVaccUslCorrente, FlagAbilitazioneVaccUslCorrente)
            End If
            Return Session("Solleciti")
        End Get
    End Property

    Public Property eliminataPrenotazione() As Boolean
        Get
            Return Session("eliminataPrenotazione")
        End Get
        Set(Value As Boolean)
            Session("eliminataPrenotazione") = Value
        End Set
    End Property

    Public Property State(index) As Boolean
        Get
            Return ViewState("OnVac_State")(index)
        End Get
        Set(Value As Boolean)
            If ViewState("OnVac_State") Is Nothing Then ViewState("OnVac_State") = New Boolean() {False, False}
            ViewState("OnVac_State")(index) = Value
        End Set
    End Property

    Public Property LastMonth() As Int16
        Get
            Return ViewState("OnVac_LastMonth")
        End Get
        Set(Value As Int16)
            ViewState("OnVac_LastMonth") = Value
        End Set
    End Property

    Public Property LastYear() As Int16
        Get
            Return ViewState("OnVac_LastYear")
        End Get
        Set(Value As Int16)
            ViewState("OnVac_LastYear") = Value
        End Set
    End Property

    Public Property dtaFeste() As DataTable
        Get
            Return Session("OnVac_staFeste")
        End Get
        Set(Value As DataTable)
            Session("OnVac_staFeste") = Value
        End Set
    End Property

    Public Property dtaIndisp() As DataTable
        Get
            Return Session("OnVac_staIndisp")
        End Get
        Set(Value As DataTable)
            Session("OnVac_staIndisp") = Value
        End Set
    End Property

    Property dtaRicerca() As DataTable
        Get
            If Session("OnVac_dtaRicerca") Is Nothing Then Return Nothing
            Return DirectCast(Session("OnVac_dtaRicerca"), SerializableDataTableContainer).Data
        End Get
        Set(Value As DataTable)
            If Session("OnVac_dtaRicerca") Is Nothing Then
                Session("OnVac_dtaRicerca") = New SerializableDataTableContainer
            End If
            DirectCast(Session("OnVac_dtaRicerca"), SerializableDataTableContainer).Data = Value
        End Set
    End Property

    Public Property dtaPrenotazioni() As DataTable
        Get
            Return Session("OnVac_dtaPrenotazioni")
        End Get
        Set(Value As DataTable)
            Session("OnVac_dtaPrenotazioni") = Value
        End Set
    End Property

    Public Property dtaNAppuntamenti() As DataTable
        Get
            Return Session("OnVac_dtaNAppuntamenti")
        End Get
        Set(Value As DataTable)
            Session("OnVac_dtaNAppuntamenti") = Value
        End Set
    End Property

    Public Property dtaMattino() As DataTable
        Get
            Return Session("OnVac_dtaMattino")
        End Get
        Set(Value As DataTable)
            Session("OnVac_dtaMattino") = Value
        End Set
    End Property

    Public Property dtaPomeriggio() As DataTable
        Get
            Return Session("OnVac_dtaPomeriggio")
        End Get
        Set(Value As DataTable)
            Session("OnVac_dtaPomeriggio") = Value
        End Set
    End Property

    Public Property oraAMPM() As DateTime
        Get
            Return ViewState("OnVac_oraAMPM")
        End Get
        Set(Value As DateTime)
            ViewState("OnVac_oraAMPM") = Value
        End Set
    End Property

    Public Property oraMinAM() As DateTime
        Get
            Return ViewState("OnVac_oraMinAM")
        End Get
        Set(Value As DateTime)
            ViewState("OnVac_oraMinAM") = Value
        End Set
    End Property

    Public Property oraMinPM() As DateTime
        Get
            Return ViewState("OnVac_oraMinPM")
        End Get
        Set(Value As DateTime)
            ViewState("OnVac_oraMinPM") = Value
        End Set
    End Property

    Public Property oraMaxAM() As DateTime
        Get
            Return ViewState("OnVac_oraMaxAM")
        End Get
        Set(Value As DateTime)
            ViewState("OnVac_oraMaxAM") = Value
        End Set
    End Property

    Public Property oraMaxPM() As DateTime
        Get
            Return ViewState("OnVac_oraMaxPM")
        End Get
        Set(Value As DateTime)
            ViewState("OnVac_oraMaxPM") = Value
        End Set
    End Property

    Public Property oralmat() As String
        Get
            Return ViewState("OnVac_oralmat")
        End Get
        Set(Value As String)
            ViewState("OnVac_oralmat") = Value
        End Set
    End Property

    Public Property oralpom() As String
        Get
            Return ViewState("OnVac_oralpom")
        End Get
        Set(Value As String)
            ViewState("OnVac_oralpom") = Value
        End Set
    End Property

    Public Property Modificato() As Boolean
        Get
            Return ViewState("OnVac_Modificato")
        End Get
        Set(Value As Boolean)
            ViewState("OnVac_Modificato") = Value
            OnitLayout31.Busy = Value
            If Not ToolBar.Items.FromKeyButton("Salva") Is Nothing Then ToolBar.Items.FromKeyButton("Salva").Enabled = Value
            If Not ToolBar.Items.FromKeyButton("AnnullaCambiamenti") Is Nothing Then ToolBar.Items.FromKeyButton("AnnullaCambiamenti").Enabled = Value
            If Not ToolBar.Items.FromKeyButton("Pazienti") Is Nothing Then ToolBar.Items.FromKeyButton("Pazienti").Enabled = Not Value
        End Set
    End Property

    Public Property modSalvataggio() As Boolean
        Get
            Return ViewState("OnVac_modSalvataggio")
        End Get
        Set(Value As Boolean)
            ViewState("OnVac_modSalvataggio") = Value
        End Set
    End Property

    Public Property LoadedDates() As System.Collections.ArrayList
        Get
            Return ViewState("OnVac_LoadedDates")
        End Get
        Set(Value As System.Collections.ArrayList)
            ViewState("OnVac_LoadedDates") = Value
        End Set
    End Property

    'ritorna il numero di pazienti prenotati nel mattino
    Public ReadOnly Property ElementiMattino() As Integer
        Get
            Return lsMattino.Items.Count
        End Get
    End Property

    'ritorna il numero di pazienti prenotati nel pomeriggio
    Public ReadOnly Property ElementiPomeriggio() As Integer
        Get
            Return lsPomeriggio.Items.Count
        End Get
    End Property

    Public Property AmbCodice() As Integer
        Get
            Return ViewState("OnVac_ambCodice")
        End Get
        Set(Value As Integer)
            ViewState("OnVac_ambCodice") = Value
        End Set
    End Property

    Public Property AmbDescrizione() As String
        Get
            Return ViewState("OnVac_ambDescrizione")
        End Get
        Set(Value As String)
            ViewState("OnVac_ambDescrizione") = Value
        End Set
    End Property

    Private Property unSoloAmbulatorio() As Boolean
        Get
            Return Session("OnVac_unSoloAmbulatorio")
        End Get
        Set(Value As Boolean)
            Session("OnVac_unSoloAmbulatorio") = Value
        End Set
    End Property

    Public Property SpostamentoAppuntamenti() As Boolean
        Get
            If ViewState("spostApp") Is Nothing Then ViewState("spostApp") = False
            Return Convert.ToBoolean(ViewState("spostApp"))
        End Get
        Set(value As Boolean)
            ViewState("spostApp") = value
        End Set
    End Property

#End Region

#Region " Proprietà private "

    'controlla se gli orari provengono da T_ANA_ORARI_GIORNALIERI o APPUNTAMENTI
    Private Property cnsControl() As Boolean
        Get
            Return Session("cnsControl")
        End Get
        Set(Value As Boolean)
            Session("cnsControl") = Value
        End Set
    End Property

    Private Property PageStatus() As stato_pagina
        Get
            Return Session("PageStatus")
        End Get
        Set(Value As stato_pagina)
            Session("PageStatus") = Value
        End Set
    End Property

    'serve per passare il dataset alla finsestra di popup della stampa
    Private Property ReportPopUp_DataSource() As System.Data.DataSet
        Get
            Return Session("ReportFileName_PopUp_dataset")
        End Get
        Set(Value As System.Data.DataSet)
            Session("ReportFileName_PopUp_dataset") = Value
        End Set
    End Property

    'parametri del report
    Private Property ReportPar(rpt As String) As ArrayList
        Get
            Return Session(rpt & "_param")
        End Get
        Set(Value As ArrayList)
            Session(rpt & "_param") = Value
        End Set
    End Property

    'filtro del report
    Private Property ReportPopUp_Filtro(rpt As String) As String
        Get
            Return Session(rpt)
        End Get
        Set(Value As String)
            Session(rpt) = Value
        End Set
    End Property

    Private Property pazientiAllineati() As Integer
        Get
            Return Session("pazientiAllineati")
        End Get
        Set(Value As Integer)
            Session("pazientiAllineati") = Value
        End Set
    End Property

    'Dataset con i dati dell'ultima ricerca:serve per la stampa
    Property dts_Ricerca() As dtsRicerca
        Get
            Return Session("OnVac_GestioneApp_dtsRicerca")
        End Get
        Set(Value As dtsRicerca)
            Session("OnVac_GestioneApp_dtsRicerca") = Value
        End Set
    End Property

    'dataset con i dati del log
    Private Property dts_log() As dtsLog
        Get
            Return Session("OnVac_GestioneApp_dsLog")
        End Get
        Set(Value As dtsLog)
            Session("OnVac_GestioneApp_dsLog") = Value
        End Set
    End Property

    'memorizza il numero di pagina corrente
    Private Property nPagina() As Integer
        Get
            Return Session("nPagina")
        End Get
        Set(Value As Integer)
            Session("nPagina") = Value
        End Set
    End Property

    'check di selezione convocati
    Private Property SelTutti() As Boolean
        Get
            Return Session("selTutti")
        End Get
        Set(Value As Boolean)
            Session("selTutti") = Value
        End Set
    End Property

    Private Property dtOrariPersonalizzati() As DataTable
        Get
            Return Session("OnVac_OrariPersonalizzati")
        End Get
        Set(Value As DataTable)
            Session("OnVac_OrariPersonalizzati") = Value
        End Set
    End Property

    ' Codice fittizio assegnato ai nuovi orari inseriti (per identificarli univocamente se devo cercarli nel datatable
    ' Parte da -1 e viene decrementato di uno ogni volta, per non andare in conflitto con i codici reali.
    ' Al momento del salvataggio, sarà il db a dare all'orario il codice giusto.
    Private Property CodNuovoOrarioPers() As Integer
        Get
            Return Session("OnVac_CodNuovoOrarioPers")
        End Get
        Set(Value As Integer)
            Session("OnVac_CodNuovoOrarioPers") = Value
        End Set
    End Property

    ' Se viene aperta una finestra modale (per impostare i filtri di ricerca o gli orari personalizzati)
    ' questa proprietà andrà impostata a true. Nel prerender viene gestito l'OnitLayout.Busy in base al
    ' valore di questa e del flag Modificato.
    Private Property FinestraModaleVisibile() As Boolean
        Get
            Return Session("OnVac_FinestraModaleVisibile")
        End Get
        Set(Value As Boolean)
            Session("OnVac_FinestraModaleVisibile") = Value
        End Set
    End Property

    Private Property RicercaEffettuata() As Boolean
        Get
            Return ViewState("RicercaEffettuata")
        End Get
        Set(Value As Boolean)
            ViewState("RicercaEffettuata") = Value
        End Set
    End Property

    Private Property FiltriMaschera() As FiltriGestioneAppuntamenti
        Get
            Return Session("FiltriGestioneAppuntamenti")
        End Get
        Set(Value As FiltriGestioneAppuntamenti)
            Session("FiltriGestioneAppuntamenti") = Value
        End Set
    End Property

    Private Property CodiciAmbulatoriConsultorioCorrente() As List(Of Integer)
        Get
            Return Session("CodiciAmbulatoriConsultorioCorrente")
        End Get
        Set(value As List(Of Integer))
            Session("CodiciAmbulatoriConsultorioCorrente") = value
        End Set
    End Property

#End Region

#Region " Inizializzazione "

    Private Sub InitSession()

        If Not dtaNAppuntamenti Is Nothing Then dtaNAppuntamenti.Dispose()
        If Not LoadedDates Is Nothing Then LoadedDates.Clear()
        If Not dtOrariPersonalizzati Is Nothing Then dtOrariPersonalizzati.Dispose()

        dtaPrenotazioni = New DataTable()
        dtaMattino = New DataTable("Mattino")
        dtaPomeriggio = New DataTable("Pomeriggio")
        dtaFeste = New DataTable()
        dtaIndisp = New DataTable()
        dtaRicerca = New DataTable()

        dts_Ricerca = New dtsRicerca()
        dts_log = New dtsLog()

        LoadedDates = New ArrayList()
        LastMonth = 0
        LastYear = 0

        Me.Modificato = False
        Me.SpostamentoAppuntamenti = False

        modSalvataggio = False
        oralmat = String.Empty
        oralpom = String.Empty
        CodNuovoOrarioPers = -1

        'check di selezione
        eliminataPrenotazione = False
        SelTutti = False

        State(0) = False
        State(1) = False

        Me.NascondiPulsanteCicliSedute()

        If FiltriMaschera Is Nothing Then FiltriMaschera = New FiltriGestioneAppuntamenti()

    End Sub

#End Region

#Region " Eventi controlli "

    Private Sub chkAppInBilancio_CheckedChanged(sender As Object, e As System.EventArgs) Handles chkAppInBilancio.CheckedChanged

        Me.CaricaCalendario()

    End Sub

#End Region

#Region " Public Methods "

    ' Controlla se la dataAppuntamento si trova tra oraApertura o oraChiusura
    Public Function InRange(dataAppuntamento As TimeSpan, oraApertura As TimeSpan, oraChiusura As TimeSpan, Optional compreso As Boolean = True) As Int16

        If TimeSpan.Compare(dataAppuntamento, oraApertura) >= 0 And TimeSpan.Compare(dataAppuntamento, oraChiusura) <= 0 Then

            If compreso OrElse TimeSpan.Compare(dataAppuntamento, oraChiusura) < 0 Then
                Return True
            End If

        End If

        Return False

    End Function

    ' Restituisce l'ultima data di ricerca di convocazioni del consultorio
    Public Function UltimaDataConsultorio() As String

        Dim result As String

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizGestioneAppuntamenti As New Biz.BizGestioneAppuntamenti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                result = bizGestioneAppuntamenti.UltimaDataCnvConsultorio(OnVacUtility.Variabili.CNS.Codice)

            End Using
        End Using

        Return result

    End Function

    'arrotondo i minuti della data passata(10:33 diventa 10:30 ---  10:36 diventa 10:40)
    Public Function roundDate(data As DateTime) As DateTime

        Dim minuti As Integer = data.Minute
        minuti = Math.Round(minuti / 10) * 10

        data = DateTime.op_Addition(DateTime.op_Subtraction(data, New TimeSpan(0, data.Minute, 0)), New TimeSpan(0, minuti, 0))

        Return data

    End Function

    Public Function GetImageAvviso(strDataInvio As String) As String

        If String.IsNullOrEmpty(strDataInvio) Then
            Return String.Empty
        End If

        Return String.Format("<img nomeImmagine='0' src='{0}' alt='{1}' />", OnVacUtility.GetIconUrl("document.gif"), "Avviso stampato")

    End Function

    Public Function GetImageAgenda(strNumSol As String, strTP As String, strEseguita As String, strEsclusa As String) As String

        Dim numSol As Integer = 0
        Dim eseguita As Integer = 0
        Dim esclusa As Integer = 0

        If IsNumeric(strNumSol) Then numSol = Integer.Parse(strNumSol)
        If IsNumeric(strEseguita) Then eseguita = Integer.Parse(strEseguita)
        If IsNumeric(strEsclusa) Then esclusa = Integer.Parse(strEsclusa)

        Dim tooltip As New System.Text.StringBuilder()
        Dim img As String = String.Empty

        If numSol > 0 OrElse eseguita > 0 OrElse esclusa > 0 Then
            img = OnVacUtility.GetIconUrl("alert.gif")
        End If

        If numSol > 0 Then
            If strTP.ToLower = "true" Then
                img = OnVacUtility.GetIconUrl("TermPer.gif")
                tooltip.Append("Termine Perentorio")
            Else
                tooltip.AppendFormat("{0}° Ritardo", numSol)
            End If
        End If

        If eseguita = 1 Then
            If tooltip.Length > 0 Then tooltip.AppendLine()
            tooltip.AppendFormat("{0} Vaccinazione già eseguita", eseguita)
        ElseIf eseguita > 1 Then
            If tooltip.Length > 0 Then tooltip.AppendLine()
            tooltip.AppendFormat("{0} Vaccinazioni già eseguite", eseguita)
        End If

        If esclusa = 1 Then
            If tooltip.Length > 0 Then tooltip.AppendLine()
            tooltip.AppendFormat("{0} Vaccinazione esclusa", esclusa)
        ElseIf esclusa > 1 Then
            If tooltip.Length > 0 Then tooltip.AppendLine()
            tooltip.AppendFormat("{0} Vaccinazioni escluse", esclusa)
        End If

        If String.IsNullOrEmpty(img) Then
            Return String.Empty
        End If

        Return String.Format("<img nomeImmagine='1' src='{0}' alt='{1}' />", img, tooltip.ToString())

    End Function

    Public Function GetLabelVaccinazioniAgenda(strVaccinazioni As String, strBilanci As String, strEseguita As String, strEsclusa As String) As String

        Dim bilanci As Integer = 0
        Dim eseguita As Integer = 0
        Dim esclusa As Integer = 0

        If IsNumeric(strBilanci) Then bilanci = Integer.Parse(strBilanci)
        If IsNumeric(strEseguita) Then eseguita = Integer.Parse(strEseguita)
        If IsNumeric(strEsclusa) Then esclusa = Integer.Parse(strEsclusa)

        Dim s As New System.Text.StringBuilder()

        If String.IsNullOrEmpty(strVaccinazioni) Then

            If eseguita = 0 AndAlso esclusa = 0 Then
                If bilanci = 0 Then
                    ' Convocazione anomala
                End If
                s.Append("SOLO BILANCIO")
            Else
                s.Append("VACCINAZIONI ESCLUSE/ESEGUITE")
                If bilanci > 0 AndAlso chkAppInBilancio.Checked Then
                    s.Append(", BILANCIO")
                End If
            End If

        Else

            s.Append(strVaccinazioni)
            If bilanci > 0 AndAlso chkAppInBilancio.Checked Then
                s.Append(", BILANCIO")
            End If

        End If

        Return s.ToString()

    End Function

#Region " Warning Message "

    Private Sub WarningMessage(prefix As String, ex As Exception)

        WarningMessage(prefix, ex.Message, TipoOperazione.Errore)

    End Sub

    Private Sub WarningMessage(prefix As String, message As String, operazione As TipoOperazione)

        '----------------------------------------------------
        ' i messaggi del log devono essere proposti in una 
        ' modale e non accodati alla label di avvertimento
        '----------------------------------------------------
        Dim statusMessage As String = String.Empty
        Dim alertMessage As String = String.Empty

        If String.IsNullOrEmpty(prefix) Then
            statusMessage = message
            alertMessage = HttpUtility.JavaScriptStringEncode(message)
        Else
            statusMessage = String.Format("{0}<br />{1}", prefix, message)
            alertMessage = String.Format("{0}{1}", prefix, HttpUtility.JavaScriptStringEncode(Environment.NewLine + message))
        End If

        Select Case operazione

            Case TipoOperazione.Pulisci

                Me.lblMessaggi.Text = String.Empty

            Case TipoOperazione.Info

                'in caso si vogliano visualizzare delle semplici informazioni a video senza inserirle anche nel db di sessione
                Me.lblMessaggi.Text += String.Format("<font color='blue'>{0} ({1}) {2}</font>", INFO, DateTime.Now.ToLongTimeString(), statusMessage)

            Case TipoOperazione.Salvataggio

                Me.lblMessaggi.Text += String.Format("<font color='green'>{0} ({1}) {2}</font>", INFO, DateTime.Now.ToLongTimeString(), statusMessage)

            Case TipoOperazione.Errore

                ' Se c'è già un errore nella label, non mostro il successivo
                If Not Me.lblMessaggi.Text.Contains(ERRORE) Then
                    Me.lblMessaggi.Text += String.Format("{0} ({1}) {2}", ERRORE, DateTime.Now.ToLongTimeString(), statusMessage)
                End If

                'se è un errore, è opportuno che venga visualizzato un alert
                Me.RegisterStartupScriptCustom("Logga", String.Format("alert('{0}');", Me.MessageReplaceForJS(alertMessage)))

        End Select

        Me.Logga(statusMessage, operazione)

    End Sub

    Private Sub Logga(message As String, operazione As TipoOperazione)

        If operazione <> TipoOperazione.Pulisci Then

            Dim newRow As dtsLog.T_LOGRow = Me.dts_log.T_LOG.NewRow()
            newRow.Data = Date.Now
            newRow.Messaggio = message
            newRow.TipoOperazione = operazione

            Select Case operazione
                Case TipoOperazione.Info, TipoOperazione.Salvataggio
                    newRow.TipoMessaggio = "info"
                Case TipoOperazione.Errore
                    newRow.TipoMessaggio = "errore"
                Case TipoOperazione.Alert
                    newRow.TipoMessaggio = "alert"
                Case Else
                    newRow.TipoMessaggio = "conferma"
            End Select

            Me.dts_log.T_LOG.Rows.Add(newRow)

        End If

    End Sub

    Private Function MessageReplaceForJS(message As String) As String

        Return message.Replace("<b>", " ").Replace("</b>", " ").Replace("<B>", " ").Replace("</B>", " ").
            Replace("<br>", " ").Replace("<BR>", " ").Replace("<BR/>", " ").Replace("<BR />", " ").Replace("<br/>", " ").Replace("<br />", " ")

    End Function

#End Region

#End Region

#Region " Gestione pulsanti di stampa "

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()
        listPrintButtons.Add(New PrintButton(Constants.ReportName.ElencoConvocati, "StampaElenco"))

        Me.ShowToolbarPrintButtons(listPrintButtons, Me.ToolBar)

        Me.ShowPrintButton(Constants.ReportName.LogGestioneAppuntamenti, btnStampaLog)

    End Sub

    Private Sub ShowButtonStampaConvocati()

        Dim tableStampaConvocati As HtmlTable = DirectCast(panelOrdinamenti.FindControl("tableStampaConvocati"), HtmlTable)

        If tableStampaConvocati Is Nothing Then
            Return
        End If

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizRpt As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                tableStampaConvocati.Visible = bizRpt.ExistsReport(Constants.ReportName.ElencoConvocatiSelezionati)

            End Using
        End Using

    End Sub

#End Region

    ' Salva l'ultima data di ricerca convocazioni del consultorio
    Private Sub SalvaUltimaDataConsultorio(data As Date)

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizGestioneAppuntamenti As New Biz.BizGestioneAppuntamenti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                bizGestioneAppuntamenti.SalvaUltimaDataCnvConsultorio(OnVacUtility.Variabili.CNS.Codice, data)

            End Using
        End Using

    End Sub

#Region " Page Events "

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim strFiltro As String = String.Format("AMB_CNS_CODICE = '{0}' AND AMB_DATA_APERTURA <= to_date('{1}','dd/mm/yyyy')" &
                                                " AND (AMB_DATA_CHIUSURA > to_date('{1}','dd/mm/yyyy') OR AMB_DATA_CHIUSURA IS NULL)",
                                                OnVacUtility.Variabili.CNS.Codice,
                                                Date.Now.ToShortDateString())
        SpostaSceltaAmb.Filtro = strFiltro

        If Not IsPostBack Then

            dypCalendario.Show = False

            ' Impostazione del controllo di selezione dei centri vaccinali
            Dim livelloUtente As LivelloUtenteConvocazione = OnVacUtility.GetLivelloUtenteConvocazione(Request.QueryString, True)

            Select Case livelloUtente

                Case LivelloUtenteConvocazione.Default, LivelloUtenteConvocazione.Undefined
                    ucSelezioneConsultori.MostraPulsanteSelezione = False
                    ucSelezioneConsultori.SelezioneMultipla = False

                Case LivelloUtenteConvocazione.Standard
                    ucSelezioneConsultori.MostraPulsanteSelezione = True
                    ucSelezioneConsultori.SelezioneMultipla = False

                Case LivelloUtenteConvocazione.Administrator
                    ucSelezioneConsultori.MostraPulsanteSelezione = True
                    ucSelezioneConsultori.SelezioneMultipla = True

                Case Else
                    Throw New NotSupportedException("LivelloUtenteConvocazione non definito!")

            End Select

            ambAjaxlist.Filtro = strFiltro
            ShowPrintButtons()

            'Inizializzazione delle variabili session
            InitSession()

            'Controllo orari di apertura del consultorio
            Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                Using bizGestioneAppuntamenti As New Biz.BizGestioneAppuntamenti(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                    AmbCodice = bizGestioneAppuntamenti.GetAmbulatorio(OnVacUtility.Variabili.CNS.Codice)
                    AmbDescrizione = bizGestioneAppuntamenti.GetAmbDescrizione(AmbCodice)

                End Using
            End Using

            If AmbCodice <> Nothing AndAlso AmbCodice <> 0 Then
                unSoloAmbulatorio = True
                InizializzaConAmbulatorio(AmbCodice, AmbDescrizione)
                PageStatus = stato_pagina.stabile_iterativo
            Else
                fmSceltaAmb.VisibileMD = True
                PageStatus = stato_pagina.seleziona_ambulatorio
            End If

            RicercaEffettuata = FiltriMaschera.SetFiltriMaschera
            If FiltriMaschera.SetFiltriMaschera Then
                SetFiltriRicerca()
                Cerca(True)
            Else
                ' Imposto il valore di default dei filtri
                DefaultFiltriRicerca()
            End If

        End If

        If dlsRicercaConvocati.Items.Count = 0 Then
            panelOrdinamenti.Visible = False
            dlsRicercaConvocati.Visible = False
        End If

        ' warning = True
        Me.FinestraModaleVisibile = False

        Select Case Me.PageStatus

            Case stato_pagina.seleziona_ambulatorio

                Select Case Request.Form("__EVENTTARGET")
                    Case "selected"
                        Dim evtArgs As String() = Request.Form.Item("__EVENTARGUMENT").Split("|")
                        AmbCodice = evtArgs(1)
                        AmbDescrizione = evtArgs(2)
                End Select

                If AmbCodice <> Nothing AndAlso AmbCodice <> 0 Then

                    InizializzaConAmbulatorio(AmbCodice, AmbDescrizione)
                    PageStatus = stato_pagina.stabile_iterativo

                Else

                    fmSceltaAmb.VisibileMD = True

                End If

            Case stato_pagina.stabile_iterativo

                Try

                    WarningMessage(String.Empty, String.Empty, TipoOperazione.Pulisci)

                    'Controlla se sono stati inviati eventi lato server da codice HTML
                    Select Case Request.Form("__EVENTTARGET")

                        Case "StampaLog"
                            Dim soloFalliti As Boolean = Request.Form("__EVENTARGUMENT") = "S"
                            lblLog.Text = String.Empty
                            StampaLog(soloFalliti)
                            fmLog.VisibileMD = False
                            OnitLayout31.Busy = False

                        Case "Sort"
                            Sort(Request.Form("__EVENTARGUMENT"))

                        Case "EspandilsPomeriggio"
                            State(0) = Not State(0)

                        Case "EspandilsMattino"
                            State(1) = Not State(1)

                        Case "AggiuntaManuale"
                            AggiuntaManuale(Request.Form("__EVENTARGUMENT"))
                            chkSelDeselDtaRicercaConvocati.Checked = False
                            SelezionaTuttiConvocati(False)

                        Case "NuovoOrario"
                            CambiaOrario(Request.Form("__EVENTARGUMENT"))

                        Case "Salva"
                            Salva()
                            modSalvataggio = True
                            ContaAppuntamenti()

                        Case "AppuntamentoAutomatico"
                            modSalvataggio = False
                            AggiungiAppuntamentoAutomatico()
                            SelezionaTuttiConvocati(False)
                            chkSelDeselDtaRicercaConvocati.Checked = False

                        Case "AnnullaCambiamenti"
                            dts_log.T_LOG.Clear()
                            modSalvataggio = False
                            dtaPrenotazioni.Rows.Clear()
                            dtaMattino.Rows.Clear()
                            dtaPomeriggio.Rows.Clear()
                            dtaFeste.Rows.Clear()
                            dtaIndisp.Rows.Clear()
                            dtaNAppuntamenti.Rows.Clear()
                            LoadedDates.Clear()

                            Modificato = False
                            SpostamentoAppuntamenti = False

                            If dtaRicerca.Rows.Count > 0 Then
                                Cerca(True)
                                SelezionaTuttiConvocati(False)
                            End If
                            If CaricaCalendario() Then
                                chkSelDeselDtaRicercaConvocati.Checked = False
                            End If
                            ContaAppuntamenti()

                        Case "CaricaPagina"
                            'cambiamento del numero di pagina del datalist dei convocati
                            ControllaSelezioneConvocati()
                            ImpaginaDataListRicercaConvocati(Request.Form("__EVENTARGUMENT"))
                            ContaAppuntamenti()

                        Case "SelezionaTutti"
                            'selezione di tutti i convocati con impaginazione
                            SelezionaTuttiConvocati(Request.Form("__EVENTARGUMENT"))
                            '     ContaAppuntamenti()
                            'Case "Calendario"
                            '    ' non fa nulla ma soprattutto non fa ContaAppuntamenti()
                            'Case Else
                            '    ContaAppuntamenti()
                    End Select

                Catch ex As IncorrectDataException
                    ex.InternalPreserveStackTrace()
                    Throw

                Catch ex As Exception
                    Common.Utility.EventLogHelper.EventLogWrite(ex, OnVacContext.AppId)
                    WarningMessage("Errore in <b>Page_Load</b>", ex)

                End Try

        End Select

    End Sub

    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles MyBase.PreRender

        OnitLayout31.Busy = Modificato Or FinestraModaleVisibile

    End Sub

#Region " Private "

    Private Sub InizializzaConAmbulatorio(codiceAmbulatorio As Integer, descrizioneAmbulatorio As String)

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            Using bizGestioneAppuntamenti As New Biz.BizGestioneAppuntamenti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                ImpostaOrariPersonalizzati()

                fmSceltaAmb.VisibileMD = False

                lblTitoloRicerca.Text =
                    String.Format("FILTRI di RICERCA / PRENOTAZIONE sul Centro: [{0}] ({1}) - Amb: [{2}] ({3})",
                                  OnVacUtility.Variabili.CNS.Codice, OnVacUtility.Variabili.CNS.Descrizione, codiceAmbulatorio.ToString(), descrizioneAmbulatorio)

                If bizGestioneAppuntamenti.ControllaOrariApertura(codiceAmbulatorio) Then

                    lblMessaggio.Text = "Attenzione: gli Orari di Apertura dell'ambulatorio non sono impostati: è necessario effettuare questa operazione per eseguire la Ricerca di una Data!"

                    fmPopUp.Title = "Orari di Apertura Ambulatorio"
                    fmPopUp.VisibileMD = True

                End If

            End Using

            'Aggiorna l'ultima data in cui si è eseguito una ricerca nel consultorio
            lblUltimaData.DataBind()

            'Carica le feste e le date di indisponibilità
            FindFeste()

            Calendario.SelectedDate = DateTime.Today

            If CaricaCalendario() Then

                'imposto il parametro: reperisce i dati dalla T_ANA_ORARI_APPUNTAMENTI (modifica 26/07/2004)
                cnsControl = False

                ContaAppuntamenti()

            End If

        End Using

    End Sub

    Private Sub NascondiPulsanteCicliSedute()

        If Not Settings.GESCICLISEDUTE Then

            btnImgCicliSedute.Enabled = False
            btnImgCicliSedute.Style.Item("cursor") = "default"
            btnImgCicliSedute.ImageUrl = "../../images/filtro_cicli_dis.gif"
            btnImgCicliSedute.AlternateText = String.Empty
            btnImgCicliSedute.Attributes.Remove("title")

            lblFiltroCicSed.Enabled = False

        End If

    End Sub

#End Region

#End Region

#Region " Solleciti "

#Region " Types "

    Public Class TabItems

        Public Shared ReadOnly Property TabPazientiDaSollecitare As TabProperties
            Get
                Return New TabProperties(0, "tabPazientiDaSollecitare")
            End Get
        End Property

        Public Shared ReadOnly Property TabPazientiInTerminePerentorio As TabProperties
            Get
                Return New TabProperties(1, "tabPazientiInTerminePerentorio")
            End Get
        End Property

        Public Shared ReadOnly Property TabPazientiElimProgRicalcolaConv As TabProperties
            Get
                Return New TabProperties(2, "tabPazientiElimProgRicalcolaConv")
            End Get
        End Property

        Public Shared ReadOnly Property TabPazientiPosticipaVaccNonObbl As TabProperties
            Get
                Return New TabProperties(3, "tabPazientiPosticipaVaccNonObbl")
            End Get
        End Property

        Public Shared ReadOnly Property TabPazientiNoCiclo As TabProperties
            Get
                Return New TabProperties(4, "tabPazientiNoCiclo")
            End Get
        End Property

        Public Shared ReadOnly Property TabPazientiDisallineati As TabProperties
            Get
                Return New TabProperties(5, "tabPazientiDisallineati")
            End Get
        End Property

        Public Class TabProperties

            Public Index As Integer
            Public Key As String

            Public Sub New(index, key)
                Me.Index = index
                Me.Key = key
            End Sub

        End Class

    End Class

#End Region

    Private Sub CheckSolleciti()

        Solleciti.CaricaPazienti(OnVacUtility.Variabili.CNS.Codice)
        BindViewGestioneSolleciti()

    End Sub

    Private Sub BindViewGestioneSolleciti()

        ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK

        If Solleciti.PazientiCicliObbligatoriNoTP.Count > 0 OrElse
           Solleciti.PazientiCicliObbligatoriInTP.Count > 0 OrElse
           Solleciti.PazientiCicliRaccomandatiTerminati.Count > 0 OrElse
           Solleciti.PazientiCicliNonObbligatori.Count > 0 OrElse
           Solleciti.PazientiNoCicli.Count > 0 OrElse
           (FlagConsensoVaccUslCorrente AndAlso Solleciti.PazientiDisallineati.Count > 0) Then

            ' tabPazientiDaSollecitare
            BindListaPazienti(Solleciti.PazientiCicliObbligatoriNoTP, dgrPazientiDaSollecitare, divPazientiDaSollecitare, lblNessunPazienteDaSollecitare, lblPazSollLimitato)

            ' tabPazientiInTerminePerentorio
            BindListaPazienti(Solleciti.PazientiCicliObbligatoriInTP, dgrPazientiInTerminePerentorio, divPazientiInTerminePerentorio, lblNessunPazienteTerminePerentorio, lblPazTPLimitato)

            ' tabPazientiElimProgRicalcolaConv
            BindListaPazienti(Solleciti.PazientiCicliRaccomandatiTerminati, dgrPazientiVaccinazioniRaccomandate, DivPazientiElimProgRicalcolaConv, lblNessunPazienteElimProgRicalcolaConv, lblPazElimProgLimitato)

            ' tabPazientiPosticipaVaccNonObbl
            BindListaPazienti(Solleciti.PazientiCicliNonObbligatori, dgrPazientiVaccinazioniNonObbligatorie, divPazientiPosticipaVaccNonObbl, lblNessunPazientePosticipaVaccNonObbl, lblPazPostLimitato)

            ' tabPazientiNoCiclo
            BindListaPazienti(Solleciti.PazientiNoCicli, dgrPazientiNoCiclo, divPazientiNoCiclo, lblNessunPazienteNoCiclo, lblPazNoCicloLimitato)

            ' tabPazientiDisallineati
            If FlagConsensoVaccUslCorrente Then

                ' Il tab Pazienti Disallineati viene visualizzato solo se la usl ha dato il consenso alla gestione dello storico vaccinale centralizzato
                ultraTabRiepilogo.Tabs.FromKeyTab(TabItems.TabPazientiDisallineati.Key).Visible = True

                BindListaPazienti(Solleciti.PazientiDisallineati, dgrDisallineati, divPazientiDisallineati, lblNessunPazienteDisallineati, lblPazientiLimitatiDisallineati)

            Else

                ' Il tab Pazienti Disallineati viene visualizzato solo se la usl ha dato il consenso alla gestione dello storico vaccinale centralizzato
                ultraTabRiepilogo.Tabs.FromKeyTab(TabItems.TabPazientiDisallineati.Key).Visible = False

            End If

            ' Visualizzazione view solleciti
            mltView.SetActiveView(viewGestioneSolleciti)

            ' Selezione del primo tab contenente elementi
            If Solleciti.PazientiCicliObbligatoriNoTP.Count > 0 Then
                ultraTabRiepilogo.SelectedTab = TabItems.TabPazientiDaSollecitare.Index
            ElseIf Solleciti.PazientiCicliObbligatoriInTP.Count > 0 Then
                ultraTabRiepilogo.SelectedTab = TabItems.TabPazientiInTerminePerentorio.Index
            ElseIf Solleciti.PazientiCicliRaccomandatiTerminati.Count > 0 Then
                ultraTabRiepilogo.SelectedTab = TabItems.TabPazientiElimProgRicalcolaConv.Index
            ElseIf Solleciti.PazientiCicliNonObbligatori.Count > 0 Then
                ultraTabRiepilogo.SelectedTab = TabItems.TabPazientiPosticipaVaccNonObbl.Index
            ElseIf Solleciti.PazientiNoCicli.Count > 0 Then
                ultraTabRiepilogo.SelectedTab = TabItems.TabPazientiNoCiclo.Index
            ElseIf (FlagConsensoVaccUslCorrente AndAlso Solleciti.PazientiDisallineati.Count > 0) Then
                ultraTabRiepilogo.SelectedTab = TabItems.TabPazientiDisallineati.Index
            End If

        End If

    End Sub

    Private Sub BindListaPazienti(pazientiSolleciti As ArrayList, dgr As DataGrid, div As HtmlGenericControl, lblNoPaz As Label, lblPazLimitati As Label)

        If pazientiSolleciti.Count > 0 Then

            ' Se ci sono troppi elementi verranno visualizzati solo i primi MAX_ELEMENT_DGR_RIEPILOGO
            dgr.DataSource = GetBoundedArrayList(pazientiSolleciti, MAX_ELEMENT_DGR_RIEPILOGO)
            dgr.DataBind()

            div.Style.Remove("display")
            lblNoPaz.Visible = False
            lblPazLimitati.Visible = (pazientiSolleciti.Count > MAX_ELEMENT_DGR_RIEPILOGO)
            lblPazLimitati.Text = Me.GetTextLabelLimitata(MAX_ELEMENT_DGR_RIEPILOGO, pazientiSolleciti.Count)

        Else

            div.Style.Add("display", "none")
            lblNoPaz.Visible = True
            lblPazLimitati.Visible = False

        End If

    End Sub

    ''' <summary>
    ''' Restituisce un arraylist limitato al numero di elementi indicati dal parametro maxElement
    ''' </summary>
    ''' <param name="listToBound"></param>
    ''' <param name="maxElement"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetBoundedArrayList(listToBound As ArrayList, maxElement As Int16) As ArrayList

        ' Se l'array ha un numero di elementi inferiore a maxElement, restituisco l'array stesso
        If listToBound.Count <= maxElement Then Return listToBound

        ' Altrimenti restituisco solo i primi max_element elementi
        Dim temp As New ArrayList()
        For i As Int16 = 0 To maxElement - 1
            temp.Add(listToBound(i))
        Next

        Return temp

    End Function

    Private Function GetTextLabelLimitata(max As Int16, tot As Integer)

        Return String.Format("Numero pazienti limitato a {0} (Totale:{1})", max.ToString(), tot.ToString())

    End Function

#End Region

#Region " Gestione Avvio Ricerca Pazienti "

    ' Cerca's
    Private Sub btnCerca_Click(sender As Object, e As EventArgs)

        If EsistonoOperazioniDaEseguire() Then
            ' Se l'applicativo è abilitato, in base ai parametri, ad eseguire operazioni automatiche, 
            ' apre la modale di conferma dell'esecuzione.
            ShowModaleAvviaRicerca()
        Else
            ' Altrimenti avvia direttamente la ricerca 
            Cerca(True)
        End If

    End Sub

    ' Apre la modale con l'elenco delle operazioni effettuabili prima della ricerca. Mostra solo quelle abilitate, in base ai parametri.
    Private Sub ShowModaleAvviaRicerca()

        ' Controllo solleciti
        lblAvviaRicerca_Solleciti.Visible = Settings.GESSOLLECITI

        ' Se dovranno essere aggiunte altre operazioni automatiche: aggiungere qui il codice per visualizzarle o meno nell'elenco

        ' Apertura della modale
        fmAvviaRicerca.VisibileMD = True

    End Sub

    ' Se saranno inserite altre procedure, aggiungere qui i controlli se sono true o false i parametri che le gestiscono.
    ' Se è abilitata almeno una operazione automatica da eseguire prima della ricerca, restituisce true
    Private Function EsistonoOperazioniDaEseguire() As Boolean

        ' Controllo solleciti
        If Settings.GESSOLLECITI Then Return True

        Return False

    End Function

    ' Click del pulsante "Effettua le operazioni elencate e avvia la ricerca" della modale fmAvviaRicerca
    Private Sub btnAvviaRicerca_OperazioniERicerca_Click(sender As Object, e As EventArgs) Handles btnAvviaRicerca_OperazioniERicerca.Click

        ' --- Effettua le operazioni automatiche che sono abilitate --- '

        ' Controllo solleciti
        If Settings.GESSOLLECITI Then CheckSolleciti()

        ' Se dovranno essere aggiunte altre operazioni automatiche: aggiungerle qui

        ' --- Cerca i convocati --- '
        Cerca(True)

        fmAvviaRicerca.VisibileMD = False

    End Sub

    ' Click del pulsante "Avvia solo la ricerca" della modale fmAvviaRicerca
    Private Sub btnAvviaRicerca_SoloRicerca_Click(sender As Object, e As EventArgs) Handles btnAvviaRicerca_SoloRicerca.Click

        Cerca(True)

        fmAvviaRicerca.VisibileMD = False

    End Sub

#End Region

#Region " Cerca "

    ' Riempie dtaRicerca con tutti i dati delle convocazioni trovate
    Private Sub Cerca(caricaDaDB As Boolean)

        Dim row As DataRow

        If caricaDaDB Then

            dlsRicercaConvocati.Visible = True

            ' Mostra il panel con i check dei campi di ordinamento
            panelOrdinamenti.Visible = True

            ' Mostra/nasconde il pulsante di stampa dei convocati (se c'è il report nell'installazione corrente)
            ShowButtonStampaConvocati()

            Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                Using bizGestioneAppuntamenti As New Biz.BizGestioneAppuntamenti(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                    dtaRicerca = bizGestioneAppuntamenti.CercaConvocazioni(ucSelezioneConsultori.GetConsultoriSelezionati(), GetFiltriRicerca())

                End Using
            End Using

            RicercaEffettuata = True

        End If

        ' Salvataggio dell'ultima data di convocazione impostata
        If txtFinoAData.Text <> String.Empty Then
            SalvaUltimaDataConsultorio(Me.txtFinoAData.Data)
        End If

        lblUltimaData.DataBind()

        ' --------------------------- OPERAZIONI SUL DATATABLE DEI RISULTATI --------------------------- '

        ' Flag che indica se la riga è da cancellare
        Dim rigaDaCancellare As Boolean

        ' Lettura filtro Associazioni-Dosi (stringa separata da |)
        Dim associazioneFiltro As String = UscFiltroAssociazioniDosi.getStringaFiltro1("|")
        Dim doseFiltro As String = UscFiltroAssociazioniDosi.getStringaFiltro2("|")

        ' Lettura filtro Cicli-Sedute (stringa separata da |)
        Dim cicloFiltro As String = UscFiltroCicliSedute.getStringaFiltro1("|")
        Dim sedutaFiltro As String = UscFiltroCicliSedute.getStringaFiltro2("|")

        Dim associazioneRisultato As String
        Dim doseRisultato As String

        Dim cicloRisultato As String
        Dim sedutaRisultato As String

        Dim i As Integer = 0

        While i < dtaRicerca.Rows.Count

            rigaDaCancellare = False

            ' Toglie da dtaRicerca le convocazioni che hanno appuntamento
            If Not dtaRicerca.Rows(i).IsNull("CNV_DATA_APPUNTAMENTO") Then
                ' Tira fuori troppa roba
                If Not LoadedDates.Contains(CType(dtaRicerca.Rows(i).Item("CNV_DATA_APPUNTAMENTO"), DateTime).Date) Then
                    rigaDaCancellare = True
                End If
            End If

            ' --- Filtro associazioni-dosi --- '
            If Not rigaDaCancellare Then

                ' Valori di associazioni e dosi per il paziente corrente, trovati dalla ricerca effettuata
                associazioneRisultato = dtaRicerca.Rows(i)("ASSOCIAZIONI").ToString()
                doseRisultato = dtaRicerca.Rows(i)("DOSI").ToString()

                ' Se non è stato impostato il filtro non controllo
                If Not String.IsNullOrEmpty(associazioneFiltro) OrElse Not String.IsNullOrEmpty(doseFiltro) Then

                    ' Controllo se almeno una tra le accoppiate associazione-dose del risultato è presente tra quelle  
                    ' dei filtri associazioni-dosi specificati: se non ce n'è nessuna, la riga è da cancellare.
                    rigaDaCancellare = Not Me.CheckElementiPresenti(associazioneFiltro, doseFiltro, associazioneRisultato, doseRisultato)

                End If
            End If

            ' --- Filtro cicli-sedute --- '
            If Not rigaDaCancellare Then

                ' Valori di cicli e sedute per il paziente corrente, trovati dalla ricerca effettuata
                cicloRisultato = dtaRicerca.Rows(i)("CICLI").ToString()
                sedutaRisultato = dtaRicerca.Rows(i)("SEDUTE").ToString()

                ' Se non è stato impostato il filtro non controllo
                If Not String.IsNullOrEmpty(cicloFiltro) OrElse Not String.IsNullOrEmpty(sedutaFiltro) Then

                    ' Controllo se almeno una tra le accoppiate ciclo-seduta del risultato è presente tra quelle 
                    ' dei filtri cicli-sedute specificati: se non ce n'è nessuna, la riga è da cancellare.
                    rigaDaCancellare = Not CheckElementiPresenti(cicloFiltro, sedutaFiltro, cicloRisultato, sedutaRisultato)

                End If

            End If

            If rigaDaCancellare Then
                dtaRicerca.Rows.Remove(dtaRicerca.Rows(i))
                i -= 1
            End If

            i += 1

        End While

        '' SIO 29/03/2004
        '' Questo è da sistemare, perché non si può togliere, ma così non va bene:
        '' toglie dalla ricerca delle convocazioni anche quelli che sono in ritardo e a cui devo assegnare
        '' il nuovo appuntamento
        For i = 0 To dtaMattino.Rows.Count - 1
            If dtaRicerca.Rows.Count <> 0 Then
                row = dtaRicerca.Rows.Find(New Object() {dtaMattino.Rows(i)("PAZ_CODICE"), dtaMattino.Rows(i)("CNV_DATA")})
            Else
                row = Nothing
            End If
            If Not row Is Nothing Then
                If Not dtaMattino.Rows(i).IsNull("CNV_DATA_APPUNTAMENTO") Then
                    dtaRicerca.Rows.Remove(row)
                End If
            End If
        Next i
        For i = 0 To dtaPomeriggio.Rows.Count - 1
            If dtaRicerca.Rows.Count <> 0 Then
                row = dtaRicerca.Rows.Find(New Object() {dtaPomeriggio.Rows(i)("PAZ_CODICE"), dtaPomeriggio.Rows(i)("CNV_DATA")})
            Else
                row = Nothing
            End If
            If Not row Is Nothing Then
                If Not dtaPomeriggio.Rows(i).IsNull("CNV_DATA_APPUNTAMENTO") Then
                    dtaRicerca.Rows.Remove(row)
                End If
            End If
        Next i
        '' fine SIO 29/03/2004

        ImpaginaDataListRicercaConvocati(1)

        'Caricamento dei dati nel dataset per la stampa
        dts_Ricerca.T_RICERCA.Clear()

        For i = 0 To dtaRicerca.Rows.Count - 1
            dts_Ricerca.T_RICERCA.ImportRow(dtaRicerca.Rows(i))
        Next

        ' --------------------------- FINE OPERAZIONI SUL DATATABLE DEI RISULTATI --------------------------- '

        Dim str As String = ""
        If Settings.TIPOANAG <> TipoAnags.SoloLocale AndAlso Settings.AUTOALLINEA Then
            str = " (Pazienti allineati: " & pazientiAllineati & ")"
        End If

        lblRis.Text = "RISULTATI RICERCA (Convocazioni trovate: " & Me.dtaRicerca.Rows.Count & ") " & str

    End Sub

#Region " Gestione Filtri associazioni-dosi e cicli-sedute "

    Private Class CoppiaElementi

        Private _Codice As String
        Public Property Codice() As String
            Get
                Return _Codice
            End Get
            Set(value As String)
                _Codice = value
            End Set
        End Property

        Private _Numero As String
        Public Property Numero() As String
            Get
                Return _Numero
            End Get
            Set(value As String)
                _Numero = value
            End Set
        End Property

        Public Sub New()
        End Sub

        Public Sub New(codice, numero)
            Me.Codice = codice
            Me.Numero = numero
        End Sub

    End Class

    ' Restituisce una lista di elementi contenente le coppie di elementi 1 e 2 con elemento1 splittato in base a "|"
    ' e elemento2 splittato prima in base a "|" poi in base a ",".
    ' Ad esempio, se i due parametri sono: elemento1 = "A|B|C" e elemento2 = "1,2|3|1,5", la lista di elementi che
    ' si ottiene è la seguente: [A;1] [A;2] [B;3] [C;1] [C;5]
    ' Se elemento2 è nullo, la lista conterrà coppie in cui è valorizzato il solo elemento1, o viceversa.
    ' Ad esempio se elemento1 = "A|B|C" e elemento2 = "", si ottiene la lista [A;] [B;] [C;]
    ' Se elemento1 = "" e elemento2 = "1|2|3|4", si ottiene la lista [;1] [;2] [;3] [;4]
    Private Function SetListaElementi(elemento1 As String, elemento2 As String) As List(Of CoppiaElementi)

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
    Private Function CheckElementiPresenti(elemento1Filtro As String, elemento2Filtro As String, elemento1Risultato As String, elemento2Risultato As String) As Boolean

        Dim elementiPresenti As Boolean = False

        ' Elementi impostati nel filtro associazioni-dosi
        Dim listElementiFiltro As List(Of CoppiaElementi) = Me.SetListaElementi(elemento1Filtro, elemento2Filtro)

        If Not listElementiFiltro Is Nothing AndAlso listElementiFiltro.Count > 0 Then

            ' Elementi associazioni-dosi ottenuti come risultato per il paziente corrente
            Dim listElementiRisultato As List(Of CoppiaElementi) = Me.SetListaElementi(elemento1Risultato, elemento2Risultato)

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
                     Or (String.IsNullOrEmpty(coppiaFiltro.Codice) And coppiaRisultato.Numero = coppiaFiltro.Numero) _
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

#End Region

    Private Function GetFiltriRicerca() As FiltriGestioneAppuntamenti

        Dim f As New FiltriGestioneAppuntamenti()

        ' Tab Appuntamenti
        f.ConsultoriSelezionati = ucSelezioneConsultori.GetConsultoriSelezionati(False).Replace(", ", "|")
        f.StatiAnagrafici = ucStatiAnagrafici.GetListStatiAnagraficiSelezionati()

        ' Se la data di ultima convocazione è vuota, la imposta ad oggi
        If txtFinoAData.Text = "" Then
            f.FinoAData = Date.Now
        Else
            f.FinoAData = txtFinoAData.Data
        End If

        f.malattia = omlMalattia.Codice
        f.chkObb = chkTipoVaccObbligatoria.Checked
        f.chkFac = chkTipoVaccFacoltativa.Checked
        f.chkRac = chkTipoVaccRaccomandata.Checked
        f.chkNonExtracomPrima = chkImmigratiNonExtracomPrimaVolta.Checked
        f.chkExtracom = chkImmigratiExtracom.Checked
        f.chkCronici = chkCronici.Checked
        f.chkSoloRitardatari = chkSoloRitardatari.Checked
        f.chkDataSingola = chkDataSingola.Checked

        If txtDaDataNascita.Text <> "" Then
            f.DaDataNascita = txtDaDataNascita.Data
        End If
        If txtADataNascita.Text <> "" Then
            f.ADataNascita = txtADataNascita.Data
        End If

        f.fmMedCodice = txtMedico.Codice
        f.categ_rischio = omlCategorieRischio.Codice
        f.sesso = ddlSesso.SelectedValue
        f.codiceComune = fmComune.Codice
        f.vaccinazioniTutteEscluse = chkEscluse.Checked
        f.dtFiltroAssociazioniSel = UscFiltroAssociazioniDosi.getValoriSelezionatiFiltro1()
        f.dtFiltroDosiSel = UscFiltroAssociazioniDosi.getValoriSelezionatiFiltro2()
        f.dtFiltroCicliSel = UscFiltroCicliSedute.getValoriSelezionatiFiltro1()
        f.dtFiltroSeduteSel = UscFiltroCicliSedute.getValoriSelezionatiFiltro2()

        ' Tab Opzioni
        f.chkRicConvocazioni = chkRicConvocazioni.Checked
        f.chkRicCiclo = chkRicCiclo.Checked
        f.chkRicMedico = chkRicMedico.Checked
        f.chkRicBilancio = chkRicBilancio.Checked
        f.chkOrdineAlfabeticoRicerca = chkOrdineAlfabeticoRicerca.Checked

        f.txtNumPazientiAlGiorno = txtNumPazientiAlGiorno.Text
        f.txtNumNuoviPazientiAlGiorno = txtNumNuoviPazientiAlGiorno.Text
        f.txtDurata = txtDurata.Text
        f.chkOrdineAlfabeticoPrenotazione = chkOrdineAlfabeticoPrenotazione.Checked

        ' Fieldset Prenotazione
        f.odpDataInizPrenotazioni = odpDataInizPrenotazioni.Data
        f.odpDataFinePrenotazioni = odpDataFinePrenotazioni.Data
        f.chkOrariPers = chkOrariPers.Checked
        f.chkSovrapponiRit = chkSovrapponiRit.Checked
        f.chkFiltroPomeriggioObbligatorio = chkFiltroPomeriggioObbligatorio.Checked

        Return f

    End Function

    Private Sub SetFiltriRicerca()

        If Not FiltriMaschera Is Nothing Then

            ' Tab Appuntamenti

            ' Reimposto la selezione dei consultori
            ucSelezioneConsultori.SelezionaConsultori(FiltriMaschera.ConsultoriSelezionati)

            ' Reimposto la selezione degli stati anagrafici
            'Dim s As String = String.Empty
            'If Not FiltriMaschera.StatiAnagrafici Is Nothing AndAlso FiltriMaschera.StatiAnagrafici.Count > 0 Then
            '    s = FiltriMaschera.StatiAnagrafici.Aggregate(Function(p, g) p & "|" & g)
            'End If

            ucStatiAnagrafici.SetStatiAnagrafici(FiltriMaschera.StatiAnagrafici)

            If FiltriMaschera.FinoAData <> DateTime.MinValue Then txtFinoAData.Data = FiltriMaschera.FinoAData

            omlMalattia.Codice = FiltriMaschera.malattia
            omlMalattia.RefreshDataBind()
            chkTipoVaccObbligatoria.Checked = FiltriMaschera.chkObb
            chkTipoVaccFacoltativa.Checked = FiltriMaschera.chkFac
            chkTipoVaccRaccomandata.Checked = FiltriMaschera.chkRac
            chkImmigratiNonExtracomPrimaVolta.Checked = FiltriMaschera.chkNonExtracomPrima
            chkImmigratiExtracom.Checked = FiltriMaschera.chkExtracom
            chkCronici.Checked = FiltriMaschera.chkCronici
            chkSoloRitardatari.Checked = FiltriMaschera.chkSoloRitardatari
            chkDataSingola.Checked = FiltriMaschera.chkDataSingola

            If FiltriMaschera.DaDataNascita <> Date.MinValue Then txtDaDataNascita.Data = FiltriMaschera.DaDataNascita
            If FiltriMaschera.ADataNascita <> Date.MinValue Then txtADataNascita.Data = FiltriMaschera.ADataNascita

            txtMedico.Codice = FiltriMaschera.fmMedCodice
            txtMedico.RefreshDataBind()
            omlCategorieRischio.Codice = FiltriMaschera.categ_rischio
            omlCategorieRischio.RefreshDataBind()
            ddlSesso.SelectedValue = FiltriMaschera.sesso
            fmComune.Codice = FiltriMaschera.codiceComune
            fmComune.RefreshDataBind()
            chkEscluse.Checked = FiltriMaschera.vaccinazioniTutteEscluse
            UscFiltroAssociazioniDosi.setValoriSelezionatiFiltro1(FiltriMaschera.dtFiltroAssociazioniSel)
            UscFiltroAssociazioniDosi.setValoriSelezionatiFiltro2(FiltriMaschera.dtFiltroDosiSel)
            UscFiltroCicliSedute.setValoriSelezionatiFiltro1(FiltriMaschera.dtFiltroCicliSel)
            UscFiltroCicliSedute.setValoriSelezionatiFiltro2(FiltriMaschera.dtFiltroSeduteSel)

            ' Aggiornamento descrizione selezione dei controlli con modale e riassiunto selezione
            lblCicliSedute.Text = UscFiltroCicliSedute.getStringaFormattata()
            lblAssociazioniDosi.Text = UscFiltroAssociazioniDosi.getStringaFormattata()

            ' Tab Opzioni
            chkRicConvocazioni.Checked = FiltriMaschera.chkRicConvocazioni
            chkRicCiclo.Checked = FiltriMaschera.chkRicCiclo
            chkRicMedico.Checked = FiltriMaschera.chkRicMedico
            chkRicBilancio.Checked = FiltriMaschera.chkRicBilancio
            chkOrdineAlfabeticoRicerca.Checked = FiltriMaschera.chkOrdineAlfabeticoRicerca

            txtNumPazientiAlGiorno.Text = FiltriMaschera.txtNumPazientiAlGiorno
            txtNumNuoviPazientiAlGiorno.Text = FiltriMaschera.txtNumNuoviPazientiAlGiorno
            txtDurata.Text = FiltriMaschera.txtDurata
            chkOrdineAlfabeticoPrenotazione.Checked = FiltriMaschera.chkOrdineAlfabeticoPrenotazione

            ' Fieldset Prenotazione
            If FiltriMaschera.odpDataInizPrenotazioni <> Date.MinValue Then odpDataInizPrenotazioni.Data = FiltriMaschera.odpDataInizPrenotazioni
            If FiltriMaschera.odpDataFinePrenotazioni <> Date.MinValue Then odpDataFinePrenotazioni.Data = FiltriMaschera.odpDataFinePrenotazioni
            chkOrariPers.Checked = FiltriMaschera.chkOrariPers
            chkSovrapponiRit.Checked = FiltriMaschera.chkSovrapponiRit
            chkFiltroPomeriggioObbligatorio.Checked = FiltriMaschera.chkFiltroPomeriggioObbligatorio

        End If

    End Sub

    Private Sub DefaultFiltriRicerca()

        ' Tab Appuntamenti 

        ' Reimposto la selezione dei consultori
        ucSelezioneConsultori.SelezionaConsultori(OnVacUtility.Variabili.CNS.Codice)

        ' Reimposto la selezione degli stati anagrafici
        ucStatiAnagrafici.LoadStatiAnagrafici()

        txtFinoAData.Data = Date.MinValue

        omlMalattia.Codice = String.Empty
        omlMalattia.Descrizione = String.Empty
        chkTipoVaccObbligatoria.Checked = False
        chkTipoVaccFacoltativa.Checked = False
        chkTipoVaccRaccomandata.Checked = False
        chkImmigratiNonExtracomPrimaVolta.Checked = False
        chkImmigratiExtracom.Checked = False
        chkCronici.Checked = False
        chkSoloRitardatari.Checked = False

        If Settings.CAMPVACCINALE Then
            chkDataSingola.Checked = True
            chkDataSingola.Enabled = True
        Else
            chkDataSingola.Checked = False
            chkDataSingola.Enabled = True
        End If

        txtDaDataNascita.Data = Date.MinValue
        txtADataNascita.Data = Date.MinValue

        txtMedico.Codice = String.Empty
        txtMedico.Descrizione = String.Empty
        omlCategorieRischio.Codice = String.Empty
        omlCategorieRischio.Descrizione = String.Empty
        ddlSesso.SelectedValue = String.Empty
        fmComune.Codice = String.Empty
        fmComune.Descrizione = String.Empty
        chkEscluse.Checked = False
        UscFiltroAssociazioniDosi.setValoriSelezionatiFiltro1(Nothing)
        UscFiltroAssociazioniDosi.setValoriSelezionatiFiltro2(Nothing)
        UscFiltroCicliSedute.setValoriSelezionatiFiltro1(Nothing)
        UscFiltroCicliSedute.setValoriSelezionatiFiltro2(Nothing)

        ' Aggiornamento descrizione selezione dei controlli con modale e riassiunto selezione
        lblCicliSedute.Text = UscFiltroCicliSedute.getStringaFormattata()
        lblAssociazioniDosi.Text = UscFiltroAssociazioniDosi.getStringaFormattata()

        If Not FiltriMaschera Is Nothing Then
            If Not FiltriMaschera.dtFiltroCicliSel Is Nothing Then FiltriMaschera.dtFiltroCicliSel.Rows.Clear()
            If Not FiltriMaschera.dtFiltroDosiSel Is Nothing Then FiltriMaschera.dtFiltroDosiSel.Rows.Clear()
            If Not FiltriMaschera.dtFiltroAssociazioniSel Is Nothing Then FiltriMaschera.dtFiltroAssociazioniSel.Rows.Clear()
            If Not FiltriMaschera.dtFiltroSeduteSel Is Nothing Then FiltriMaschera.dtFiltroSeduteSel.Rows.Clear()
        End If

        ' Tab Opzioni
        txtNumPazientiAlGiorno.Text = String.Empty
        txtNumNuoviPazientiAlGiorno.Text = String.Empty
        txtDurata.Text = String.Empty
        chkOrdineAlfabeticoPrenotazione.Checked = False

        ' Selezione dei filtri di default in base al parametro GES_APP_OPZIONI_VISUALIZZAZIONE
        chkRicConvocazioni.Checked = Settings.GES_APP_OPZIONI_VISUALIZZAZIONE.Contains(chkRicConvocazioni.CommandArgument)
        chkRicCiclo.Checked = Settings.GES_APP_OPZIONI_VISUALIZZAZIONE.Contains(chkRicCiclo.CommandArgument)
        chkRicMedico.Checked = Settings.GES_APP_OPZIONI_VISUALIZZAZIONE.Contains(chkRicMedico.CommandArgument)
        chkRicBilancio.Checked = Settings.GES_APP_OPZIONI_VISUALIZZAZIONE.Contains(chkRicBilancio.CommandArgument)
        chkOrdineAlfabeticoRicerca.Checked = True

        ' Fieldset Prenotazione
        odpDataInizPrenotazioni.Data = Date.MinValue
        odpDataFinePrenotazioni.Data = Date.MinValue
        chkOrariPers.Checked = False
        chkSovrapponiRit.Checked = False
        chkFiltroPomeriggioObbligatorio.Checked = False

    End Sub

#End Region

#Region " Calendario "

    ' Visualizza il diario del giorno selezionato, 
    ' con le indicazioni sugli orari di apertura e con gli appuntamenti presenti
    Private Sub Calendario_SelectionChanged(sender As System.Object, e As System.EventArgs) Handles Calendario.SelectionChanged

        Me.CaricaCalendario()

    End Sub

    ' Imposta l'aspetto di un giorno del mese visibile nel calendario
    Private Sub Calendario_DayRender(sender As System.Object, e As System.Web.UI.WebControls.DayRenderEventArgs) Handles Calendario.DayRender

        Try
            Dim giorno As Date = e.Day.Date

            If giorno.DayOfWeek = DayOfWeek.Sunday Then
                e.Cell.CssClass = "Festa"
                e.Cell.ToolTip = "Domenica"
            End If

            'Cerca le feste
            Dim rowFeste As DataRow

            If Not dtaFeste Is Nothing AndAlso dtaFeste.Rows.Count > 0 Then
                rowFeste = dtaFeste.Rows.Find(CType(giorno.Day & "/" & giorno.Month & "/" & OnVac.Constants.CommonConstants.RECURSIVE_YEAR, DateTime))
                If rowFeste Is Nothing Then rowFeste = dtaFeste.Rows.Find(giorno)
                If Not rowFeste Is Nothing Then
                    e.Cell.CssClass = "Festa"
                    e.Cell.ToolTip = rowFeste.Item("FES_DESCRIZIONE")
                End If
            End If

            If Not dtaIndisp Is Nothing AndAlso dtaIndisp.Rows.Count > 0 Then
                Dim rowIndisp As DataRow
                rowIndisp = dtaIndisp.Rows.Find(CType(giorno.Day & "/" & giorno.Month & "/" & OnVac.Constants.CommonConstants.RECURSIVE_YEAR, DateTime))
                If rowIndisp Is Nothing Then rowIndisp = dtaIndisp.Rows.Find(giorno)
                If Not rowIndisp Is Nothing Then
                    e.Cell.CssClass = "Indisponibile"
                    e.Cell.ToolTip = rowIndisp.Item("PERIODO_INDISP")
                End If
            End If

            If Not dtaNAppuntamenti Is Nothing AndAlso dtaNAppuntamenti.Rows.Count > 0 Then

                Dim rowNAppuntamenti As DataRow

                'Controlla se sono presenti prenotazioni in questa data
                rowNAppuntamenti = dtaNAppuntamenti.Rows.Find(giorno)

                If Not rowNAppuntamenti Is Nothing AndAlso (rowNAppuntamenti.Item("N_NORMALI").ToString() <> "" OrElse rowNAppuntamenti.Item("N_RITARDI").ToString() <> "" OrElse rowNAppuntamenti.Item("N_BILANCI").ToString() <> "") Then

                    'aggiunta dei ritardatari e dei bilanci
                    Dim strPrenotazioni As String = String.Empty
                    Dim showRedDay As Boolean = False

                    If rowNAppuntamenti("N_NORMALI").ToString() <> "" AndAlso rowNAppuntamenti("N_NORMALI").ToString() <> "0" Then
                        strPrenotazioni = "[" & rowNAppuntamenti("N_NORMALI").ToString() & "] appuntamenti semplici"
                        showRedDay = True
                    End If
                    If rowNAppuntamenti("N_RITARDI").ToString() <> "" AndAlso rowNAppuntamenti("N_RITARDI").ToString() <> "0" Then
                        If strPrenotazioni <> "" Then strPrenotazioni &= " - "
                        strPrenotazioni &= "[" & rowNAppuntamenti("N_RITARDI").ToString() & "] ritardatari"
                        showRedDay = True
                    End If
                    If chkAppInBilancio.Checked AndAlso rowNAppuntamenti("N_BILANCI").ToString() <> "" AndAlso rowNAppuntamenti("N_BILANCI").ToString() <> "0" Then
                        If strPrenotazioni <> "" Then strPrenotazioni &= " e "
                        strPrenotazioni &= "[" & rowNAppuntamenti("N_BILANCI").ToString() & "] solo bilanci"
                        showRedDay = True
                    End If

                    If showRedDay Then
                        e.Cell.CssClass &= " Prenotato"
                        e.Cell.ForeColor = Color.Red
                    End If

                    If e.Cell.ToolTip = "" Then
                        e.Cell.ToolTip = strPrenotazioni
                    Else
                        e.Cell.ToolTip &= "  -  " & strPrenotazioni
                    End If

                End If

            End If

        Catch ex As Exception
            Common.Utility.EventLogHelper.EventLogWrite(ex, OnVacContext.AppId)
            WarningMessage("Errore in <b>Calendario_DayRender</b>:", ex)
        End Try

    End Sub

    Private Function CaricaCalendario() As Boolean

        Try
            ReloadCodiciAmbulatoriConsultorioCorrente()

            Dim consultorioCalcoloAppuntamenti As Consultorio = CreateConsultorioCalcoloAppuntamenti()
            Dim ambulatorioCalcoloAppuntamenti As Ambulatorio = CreateAmbulatorioCalcoloAppuntamenti(AmbCodice, False, consultorioCalcoloAppuntamenti)

            consultorioCalcoloAppuntamenti.DaData = Me.Calendario.SelectedDate
            consultorioCalcoloAppuntamenti.AData = Me.Calendario.SelectedDate

            consultorioCalcoloAppuntamenti.Ambulatori.Add(ambulatorioCalcoloAppuntamenti)

            ' N.B. : I filtri ragionano al contrario (impostano le indisponibilità)
            Dim filtriBlocchi As New ArrayList()
            Dim numBlocchiGiorno As Integer

            Dim filtriOrari As Filtri.FiltroCollection =
                CaricaFiltriApertura(False, Me.Calendario.SelectedDate.DayOfWeek, PartiGiornata.Entrambi, filtriBlocchi, numBlocchiGiorno)

            For Each f As Filtri.IFiltro In filtriOrari
                ambulatorioCalcoloAppuntamenti.FiltriAmbulatorio.Add(f)
            Next

            If Not filtriBlocchi Is Nothing AndAlso filtriBlocchi.Count > 0 Then

                For idxBlocchi As Int16 = 0 To filtriBlocchi.Count - 1
                    ambulatorioCalcoloAppuntamenti.FiltriVariabili.SetParam(filtriBlocchi(idxBlocchi)(0), filtriBlocchi(idxBlocchi)(1))
                Next

                ambulatorioCalcoloAppuntamenti.FiltriVariabili.NumeroMassimoBlocchiPersonalizzatiGiornalieri = numBlocchiGiorno

            End If

            Dim filtriFesta As Filtri.FiltroCollection = CaricaFiltriFeste()

            For Each f As Filtri.IFiltro In filtriFesta
                ambulatorioCalcoloAppuntamenti.FiltriAmbulatorio.Add(f)
            Next

            consultorioCalcoloAppuntamenti.FindAppuntamenti()

            Dim oraAm As DateTime = New DateTime(1, 1, 1, 8, 0, 0)
            Dim oraPm As DateTime
            If String.IsNullOrEmpty(Me.Settings.ORAPM) Then
                oraPm = New DateTime(1, 1, 1, 13, 0, 0)
            Else
                Dim d As DateTime = System.Convert.ToDateTime(Me.Settings.ORAPM)
                oraPm = New DateTime(1, 1, 1, d.Hour, d.Minute, d.Second)
            End If

            oraMinAM = oraAm
            oraMaxAM = oraAm
            oraMinPM = oraPm
            oraMaxPM = oraPm

            Dim fasceOrarieAM As New System.Text.StringBuilder()
            Dim fasceOrariePM As New System.Text.StringBuilder()

            If Not ambulatorioCalcoloAppuntamenti.Giorni Is Nothing AndAlso ambulatorioCalcoloAppuntamenti.Giorni.Count > 0 Then
                For Each c As Timing.TimeBlock In ambulatorioCalcoloAppuntamenti.Giorni(0).TimeBlocks
                    If c.IsFree(0) Then

                        Dim fasciaOraria As String = GetStringFasciaOraria(c.Inizio, c.Fine)

                        If c.Fine > oraPm Then
                            If c.Inizio < oraMinPM Then oraMinPM = c.Inizio
                            If c.Fine > oraMaxPM Then oraMaxPM = c.Fine
                            fasceOrariePM.AppendFormat("{0}, ", fasciaOraria)
                        Else
                            If c.Inizio < oraMinAM Then oraMinAM = c.Inizio
                            If c.Fine > oraMaxAM Then oraMaxAM = c.Fine
                            fasceOrarieAM.AppendFormat("{0}, ", fasciaOraria)
                        End If

                    End If
                Next
            End If

            If fasceOrarieAM.Length = 0 Then
                fasceOrarieAM.AppendFormat(GetStringFasciaOraria(oraMinAM, oraMaxAM))
            Else
                fasceOrarieAM.RemoveLast(2)
            End If

            If fasceOrariePM.Length = 0 Then
                fasceOrariePM.AppendFormat(GetStringFasciaOraria(oraMinPM, oraMaxPM))
            Else
                fasceOrariePM.RemoveLast(2)
            End If

            If oraMinPM < oraMaxAM And oraMinAM < oraMaxAM Then
                oraAMPM = oraMaxAM
            Else
                oraAMPM = oraMinPM
            End If

            BuildDataTableAppuntamenti(Me.Calendario.SelectedDate.Date)

            Dim dtv As DataView

            ' --- MATTINO --- '
            dtv = New DataView(dtaMattino)
            dtv.RowFilter = GetFiltro(Calendario.SelectedDate, True)
            dtv.Sort = "CNV_DATA_APPUNTAMENTO"

            Me.lsMattino.DataSource = dtv
            Me.lsMattino.DataBind()

            ' Appuntamenti totali (mattino)
            Dim numAppuntamentiTotaliMattino As Int16 = dtv.Count

            ' Appuntamenti per ritardatari (mattino)
            Dim numRitardatariMattino As Int16 = ContaRitardatari(dtv)

            ' Appuntamenti solo bilancio (mattino)
            Dim numSoloBilancioMattino As Int16 = 0
            If chkAppInBilancio.Checked Then
                dtv.RowFilter += " and vaccinazioni is null "
                numSoloBilancioMattino = dtv.Count
            End If

            ' --- POMERIGGIO --- '
            dtv = New DataView(dtaPomeriggio)
            dtv.RowFilter = GetFiltro(Calendario.SelectedDate, False)
            dtv.Sort = "CNV_DATA_APPUNTAMENTO"

            Me.lsPomeriggio.DataSource = dtv
            Me.lsPomeriggio.DataBind()

            ' Appuntamenti totali (pomeriggio)
            Dim numAppuntamentiTotaliPomeriggio As Int16 = dtv.Count

            ' Appuntamenti per ritardatari (pomeriggio)
            Dim numRitardatariPomeriggio As Int16 = ContaRitardatari(dtv)

            ' Appuntamenti solo bilancio (pomeriggio)
            Dim numSoloBilancioPomeriggio As Int16 = 0
            If Me.chkAppInBilancio.Checked Then
                dtv.RowFilter += " and vaccinazioni is null "
                numSoloBilancioPomeriggio = dtv.Count
            End If

            ' Titolo Appuntamenti Mattina
            If oraMinAM = oraMaxAM Or TimeSpan.Compare(oraMinAM.TimeOfDay, TimeSpan.Zero) = 0 Then
                Me.oralmat = String.Format("Chiuso {0}", GetCaptionAppuntamenti(String.Empty, numAppuntamentiTotaliMattino, numSoloBilancioMattino, numRitardatariMattino))
                Me.State(1) = (Me.lsMattino.Items.Count = 0)
            Else
                Me.oralmat = String.Format("Mattina {0}", GetCaptionAppuntamenti(fasceOrarieAM.ToString(), numAppuntamentiTotaliMattino, numSoloBilancioMattino, numRitardatariMattino))
                Me.State(1) = False
            End If

            ' Titolo Appuntamenti Pomeriggio
            If oraMinPM = oraMaxPM Or TimeSpan.Compare(oraMaxPM.TimeOfDay, TimeSpan.MaxValue) = 0 Then
                Me.oralpom = String.Format("Chiuso {0}", GetCaptionAppuntamenti(String.Empty, numAppuntamentiTotaliPomeriggio, numSoloBilancioPomeriggio, numRitardatariPomeriggio))
                Me.State(0) = (Me.lsPomeriggio.Items.Count = 0)
            Else
                Me.oralpom = String.Format("Pomeriggio {0}", GetCaptionAppuntamenti(fasceOrariePM.ToString(), numAppuntamentiTotaliPomeriggio, numSoloBilancioPomeriggio, numRitardatariPomeriggio))
                Me.State(0) = False
            End If

        Catch ex As Exception
            Common.Utility.EventLogHelper.EventLogWrite(ex, OnVacContext.AppId)
            WarningMessage("Errore in <b>CaricaCalendario</b>", ex)
            Return False
        End Try

        'marco: gestisto l'apertura automatica o meno del datalist a seconda che
        ' vi siano o meno prenotazioni per quel giorno
        If Me.lsPomeriggio.Items.Count > 0 Then
            Me.State(0) = True
        End If
        If Me.lsMattino.Items.Count > 0 Then
            Me.State(1) = True
        End If

        Return True

    End Function

    Private Function GetStringFasciaOraria(inizio As DateTime, fine As DateTime) As String

        Return String.Format("{0} - {1}", inizio.ToShortTimeString(), fine.ToShortTimeString())

    End Function

    Private Function GetCaptionAppuntamenti(fasceOrarie As String, numAppuntamentiTotali As Integer, numSoloBilanci As Integer, numRitardi As Integer) As String

        Dim caption As New System.Text.StringBuilder()

        If Not String.IsNullOrWhiteSpace(fasceOrarie) Then
            caption.AppendFormat("({0}) ", fasceOrarie)
        End If

        ' Se presenti, visualizzazione del numero di appuntamenti/bilanci/ritardi
        Dim numAppuntamentiSemplici As Integer = (numAppuntamentiTotali - numSoloBilanci - numRitardi)

        If numAppuntamentiSemplici > 0 OrElse numRitardi > 0 OrElse numSoloBilanci > 0 Then

            caption.Append(" - ")

            If numAppuntamentiSemplici > 0 Then caption.AppendFormat("Appuntamenti semplici: {0}, ", numAppuntamentiSemplici)

            If numRitardi > 0 Then caption.AppendFormat("Ritardatari: {0}, ", numRitardi)

            If numSoloBilanci > 0 Then caption.AppendFormat("Solo bilanci: {0}, ", numSoloBilanci)

            caption.RemoveLast(2)

        End If

        Return caption.ToString()

    End Function

    'calcola il numero dei ritardatari tra quelli presenti negli appuntamenti
    Private Function ContaRitardatari(dtv As DataView) As Int16

        Dim filtro As String = dtv.RowFilter

        Dim dtRitardatari As DataView = dtv.Table.Copy().DefaultView
        dtRitardatari.RowFilter = filtro & " and SOLLECITO > 0"

        Return dtRitardatari.Count

    End Function

    ' Imposta il filtro per selezionare solo gli appuntamenti del mattino o solo quelli del pomeriggio
    Private Function GetFiltro(dataAppuntamento As DateTime, isMattino As Boolean) As String

        Dim giorno As String = dataAppuntamento.ToShortDateString()
        Dim orario As String = oraAMPM.ToShortTimeString().Replace(".", ":")

        Dim filtro As New System.Text.StringBuilder()

        If isMattino Then
            filtro.AppendFormat("CNV_DATA_APPUNTAMENTO >= CONVERT('{0} 00:00', 'System.DateTime') ", giorno)
            filtro.AppendFormat("AND CNV_DATA_APPUNTAMENTO < CONVERT('{0} {1}', 'System.DateTime')", giorno, orario)
        Else
            filtro.AppendFormat("CNV_DATA_APPUNTAMENTO >= CONVERT('{0} {1}', 'System.DateTime') ", giorno, orario)
            filtro.AppendFormat("AND CNV_DATA_APPUNTAMENTO <= CONVERT('{0} 23:59', 'System.DateTime')", giorno)
        End If

        If Not chkAppInBilancio.Checked Then
            filtro.Append(" AND (vaccinazioni is not null OR (esclusa is not null AND esclusa <> 0) OR (eseguita is not null AND eseguita <> 0))")
        End If

        filtro.AppendFormat(" AND CNV_AMB_CODICE = {0}", Me.AmbCodice.ToString())

        Return filtro.ToString()

    End Function

    ' Riempie il DataTable dtaFeste con tutte le festività nazionali e le indisponibilità del consultorio
    Sub FindFeste()

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizGestioneAppuntamenti As New Biz.BizGestioneAppuntamenti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Me.dtaFeste = bizGestioneAppuntamenti.BuildDtFeste()
                Me.dtaFeste.PrimaryKey = New DataColumn() {dtaFeste.Columns("FES_DATA")}

                Me.dtaIndisp = bizGestioneAppuntamenti.BuildDtIndisponibilita(AmbCodice)
                Me.dtaIndisp.PrimaryKey = New DataColumn() {dtaIndisp.Columns("FAS_DATA")}

            End Using
        End Using

    End Sub

#End Region

    'selezione di tutti i convocati in tutte le pagine del datalist
    Private Sub SelezionaTuttiConvocati(tipo As String)

        For Each row As DataRow In dtaRicerca.Rows
            If Not Boolean.Parse(row("TP")) Then
                row("SEL") = IIf(tipo, "S", "N")
            Else
                row("SEL") = "N"
            End If
        Next

        SelTutti = CBool(tipo).ToString().ToLower()

        ImpaginaDataListRicercaConvocati(nPagina)

    End Sub

    Private Sub ContaAppuntamenti()

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizGestioneAppuntamenti As New Biz.BizGestioneAppuntamenti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                dtaNAppuntamenti = bizGestioneAppuntamenti.FillDtAppuntamenti(AmbCodice)
                dtaNAppuntamenti.PrimaryKey = New DataColumn() {dtaNAppuntamenti.Columns("CNV_DATA_APPUNTAMENTO")}

            End Using
        End Using

    End Sub

    'Cambia un orario di un appuntamento. Non vengono effettuati controlli di sovrapposizione
    Private Sub CambiaOrario(PomMat As String)

        Dim dtaMattinoPomeriggio As DataTable = IIf(PomMat = "Mat", dtaMattino, dtaPomeriggio)

        'deve cambiare tutti gli orari della fascia mattino/pomeriggio
        Dim dlRef As DataList = IIf(PomMat = "Mat", lsMattino, lsPomeriggio)

        For count As Integer = 0 To dlRef.Items.Count - 1

            Dim codicePaziente As String = DirectCast(dlRef.Items(count).FindControl("lblCodice"), Label).Text
            Dim dataConvocazione As String = DirectCast(dlRef.Items(count).FindControl("lblCnvData"), Label).Text
            Dim nuovaOra As String = DirectCast(dlRef.Items(count).FindControl("txtOrario"), TextBox).Text

            For Each r As DataRow In dtaMattinoPomeriggio.Rows

                If r("PAZ_CODICE") = codicePaziente AndAlso r("CNV_DATA") = dataConvocazione AndAlso Not IsDBNull(r("CNV_DATA_APPUNTAMENTO")) AndAlso
                   CType(r("CNV_DATA_APPUNTAMENTO"), DateTime).TimeOfDay.CompareTo(CType(CType(r("CNV_DATA_APPUNTAMENTO"), DateTime).Date & " " & nuovaOra, DateTime).TimeOfDay) <> 0 Then

                    ' Aggiornamento dell'orario di appuntamento
                    r("CNV_DATA_APPUNTAMENTO") = CType(CType(r("CNV_DATA_APPUNTAMENTO"), DateTime).Date & " " & nuovaOra, DateTime)

                    If CType(r("CNV_DATA_APPUNTAMENTO"), DateTime).TimeOfDay.CompareTo(oraAMPM.TimeOfDay) = 1 And dtaMattinoPomeriggio Is dtaMattino Then
                        Dim rowadded As DataRow = dtaPomeriggio.NewRow()
                        rowadded.ItemArray = r.ItemArray
                        dtaPomeriggio.Rows.Add(rowadded)
                        dtaMattinoPomeriggio.Rows.Remove(r)
                    ElseIf CType(r("CNV_DATA_APPUNTAMENTO"), DateTime).TimeOfDay.CompareTo(oraAMPM.TimeOfDay) <= 0 And dtaMattinoPomeriggio Is dtaPomeriggio Then
                        Dim rowadded As DataRow = dtaMattino.NewRow()
                        rowadded.ItemArray = r.ItemArray
                        dtaMattino.Rows.Add(rowadded)
                        dtaMattinoPomeriggio.Rows.Remove(r)
                    End If

                    r("CNV_TIPO_APPUNTAMENTO") = Constants.TipoPrenotazioneAppuntamento.ManualeDaGestioneAppuntamenti

                    Exit For

                End If

            Next

        Next

        If Me.CaricaCalendario() Then
            Me.Modificato = True
        End If

    End Sub

    Sub AggiuntaManuale(dove As String)

        Dim dataMinima As Date = Calendario.SelectedDate
        Dim dataMassima As Date = Calendario.SelectedDate
        Dim fuoriOrario As Boolean = cnsControl

        Me.ReloadCodiciAmbulatoriConsultorioCorrente()

        Dim consultorioCalcoloAppuntamenti As Consultorio = CreateConsultorioCalcoloAppuntamenti()
        Dim ambulatorioCalcoloAppuntamenti As Ambulatorio = CreateAmbulatorioCalcoloAppuntamenti(AmbCodice, False, consultorioCalcoloAppuntamenti)

        consultorioCalcoloAppuntamenti.DaData = dataMinima
        consultorioCalcoloAppuntamenti.AData = dataMassima

        consultorioCalcoloAppuntamenti.Ambulatori.Add(ambulatorioCalcoloAppuntamenti)

        Dim pazCollection As CnvPazienteCollection = RestituisciPazienti(False, True)

        If pazCollection.Count = 0 Then
            WarningMessage(String.Empty, "Nessuna convocazione selezionata!", TipoOperazione.Info)
            Exit Sub
        End If

        For Each paz As CnvPaziente In pazCollection
            consultorioCalcoloAppuntamenti.Pazienti.Add(paz)
        Next

        Dim filtriOcc As Filtri.FiltroCollection = CaricaPrenotati(False, dataMinima, dataMassima)

        For Each f As Filtri.IFiltro In filtriOcc
            ambulatorioCalcoloAppuntamenti.FiltriVariabili.Add(f)
        Next

        Dim caso As PartiGiornata
        If dove = "lsMattina" Then caso = PartiGiornata.Mattina
        If dove = "lsPomeriggio" Then caso = PartiGiornata.Pomeriggio

        Dim filtriBlocchi As New ArrayList()

        Dim numBlocchiGiorno As Integer
        Dim filtriOrari As Filtri.FiltroCollection = CaricaFiltriApertura(fuoriOrario, Calendario.SelectedDate.DayOfWeek, caso, filtriBlocchi, numBlocchiGiorno)
        For Each f As Filtri.IFiltro In filtriOrari
            ambulatorioCalcoloAppuntamenti.FiltriAmbulatorio.Add(f)
        Next

        If Not IsNothing(filtriBlocchi) AndAlso filtriBlocchi.Count > 0 Then
            For idxBlocchi As Int16 = 0 To filtriBlocchi.Count - 1
                ambulatorioCalcoloAppuntamenti.FiltriVariabili.SetParam(filtriBlocchi(idxBlocchi)(0), filtriBlocchi(idxBlocchi)(1))
            Next
            ambulatorioCalcoloAppuntamenti.FiltriVariabili.NumeroMassimoBlocchiPersonalizzatiGiornalieri = numBlocchiGiorno
        End If

        consultorioCalcoloAppuntamenti.FindAppuntamenti()

        Dim esitiNegativi As Boolean = False

        Using dam As IDAM = OnVacUtility.OpenDam()

            For Each paz As CnvPaziente In consultorioCalcoloAppuntamenti.Pazienti

                If paz.EsitoProcedura.Successo Then

                    Dim isRitardatarioReale As Boolean = paz.Info("RitardatarioReale")

                    CreaAppuntamento(dam, paz.Codice, paz.DataConvocazione, paz.EsitoProcedura.DataAppuntamento,
                                     paz.DurataAppuntamento, dove, Constants.TipoPrenotazioneAppuntamento.ManualeDaGestioneAppuntamenti,
                                     paz.Ritardatario, paz.SoloBilancio, isRitardatarioReale)

                Else

                    If Not esitiNegativi AndAlso Not paz.EsitoProcedura.MotivoRifiuto Is Nothing AndAlso Not paz.EsitoProcedura.MotivoRifiuto(0) Is Nothing Then

                        esitiNegativi = True
                        WarningMessage(String.Empty, paz.EsitoProcedura.MotivoRifiuto(0).MessaggioStandard, TipoOperazione.Info)

                    End If

                End If

            Next

        End Using

        Cerca(False)

        If Me.CaricaCalendario() Then
            AggiungiAlLogTemporaneo(consultorioCalcoloAppuntamenti, TipoOperazione.Prenotazione_Manuale)
        End If

    End Sub

    'Crea un appuntamento al paziente con convocazione in data DataConvocazione nella DataAppuntamento
    'in Dove (lsMattina,lsPomeriggio) e indica se salvare o no i cambiamenti sul DB
    Private Sub CreaAppuntamento(ByRef dam As IDAM, codicePaziente As String, dataConvocazione As DateTime, dataAppuntamento As DateTime, durataApp As Integer, dove As String, Optional tipoAppuntamento As String = Constants.TipoPrenotazioneAppuntamento.ManualeDaGestioneAppuntamenti, Optional isRitardatario As Boolean = False, Optional isSoloBilancio As Boolean = False, Optional isRitardatarioReale As Boolean = False)

        Dim dtaMattinoPomeriggio As DataTable
        Dim rowsMattinoPomeriggio As DataRow = Nothing

        If dove = "" Then
            dtaMattinoPomeriggio = IIf(InMattino(dam, dataAppuntamento), dtaMattino, dtaPomeriggio)
        Else
            dtaMattinoPomeriggio = IIf(dove = "lsMattino", dtaMattino, dtaPomeriggio)
        End If

        Dim rowarr As DataRow() = dtaMattinoPomeriggio.Select(String.Format("PAZ_CODICE = '{0}' AND CNV_DATA = CONVERT('{1}', 'System.DateTime')", codicePaziente, dataConvocazione))
        If rowarr.Length = 0 Then

            With dam.QB
                .NewQuery()
                .AddSelectFields("PAZ_NOME, PAZ_COGNOME, PAZ_DATA_NASCITA, CNV_DATA_INVIO, MED_DESCRIZIONE, CNV_DATA_INVIO")
                .AddTables("T_PAZ_PAZIENTI, T_CNV_CONVOCAZIONI, T_ANA_MEDICI")
                .AddWhereCondition("CNV_PAZ_CODICE", Comparatori.Uguale, "PAZ_CODICE", DataTypes.Join)
                .AddWhereCondition("PAZ_MED_CODICE_BASE", Comparatori.Uguale, "MED_CODICE", DataTypes.OutJoinLeft)
                .AddWhereCondition("CNV_PAZ_CODICE", Comparatori.Uguale, codicePaziente, DataTypes.Stringa)
                .AddWhereCondition("CNV_DATA", Comparatori.Uguale, dataConvocazione, DataTypes.DataOra)
            End With

            Using idr As System.Data.IDataReader = dam.BuildDataReader()
                If idr.Read() Then
                    rowsMattinoPomeriggio = dtaMattinoPomeriggio.NewRow()
                    rowsMattinoPomeriggio("PAZ_CODICE") = codicePaziente
                    rowsMattinoPomeriggio("CNV_DATA") = dataConvocazione
                    rowsMattinoPomeriggio("PAZ_NOME") = idr("PAZ_NOME")
                    rowsMattinoPomeriggio("PAZ_COGNOME") = idr("PAZ_COGNOME")
                    rowsMattinoPomeriggio("PAZ_DATA_NASCITA") = idr("PAZ_DATA_NASCITA")
                    rowsMattinoPomeriggio("CNV_DATA_INVIO") = idr("CNV_DATA_INVIO")
                    rowsMattinoPomeriggio("MED_DESCRIZIONE") = idr("MED_DESCRIZIONE")
                    rowsMattinoPomeriggio("CNV_DATA_INVIO") = idr("CNV_DATA_INVIO")
                    dtaMattinoPomeriggio.Rows.Add(rowsMattinoPomeriggio)
                End If
            End Using

        Else
            rowsMattinoPomeriggio = rowarr(0)
        End If

        Dim newRow() As DataRow = dtaRicerca.Select(String.Format("PAZ_CODICE = '{0}' AND CNV_DATA = CONVERT('{1}', 'System.DateTime')", codicePaziente, dataConvocazione))

        If newRow.Length > 0 Then

            rowsMattinoPomeriggio("CNV_DATA_APPUNTAMENTO") = dataAppuntamento
            rowsMattinoPomeriggio("CNV_TIPO_APPUNTAMENTO") = tipoAppuntamento
            rowsMattinoPomeriggio("VACCINAZIONI") = IIf(newRow(0)("VACCINAZIONI").ToString() = "", "()", newRow(0)("VACCINAZIONI"))
            rowsMattinoPomeriggio("SOLLECITO") = newRow(0)("SOLLECITO")
            rowsMattinoPomeriggio("TP") = newRow(0)("TP")
            rowsMattinoPomeriggio("CNV_AMB_CODICE") = Me.AmbCodice

            '--- GSB20 Gestione dei solo bilanci 20 minuti
            'Le convocazioni per solo bilancio devono avere sempre una durata di 0 minuti, mentre prima ai solo bilancio senza pediatra veniva dato un appuntamento di 20 minuti
            If isRitardatario Or isSoloBilancio Then
                ' Se è ritardatario o solobilancio
                rowsMattinoPomeriggio.Item("CNV_DURATA_APPUNTAMENTO") = 0
            Else
                rowsMattinoPomeriggio.Item("CNV_DURATA_APPUNTAMENTO") = durataApp
            End If

            'Se non vuoto il campo quando ricarica gli appuntamenti del giorno con la selected_day_changed
            'mi mette l'iconcina di invio dell'avviso perchè il campo è stato aggiornato solo nel db e non nel dta
            rowsMattinoPomeriggio.Item("CNV_DATA_INVIO") = System.DBNull.Value

        End If

        Me.Modificato = True

        Dim tipo As String = TipoPrenotazione.APPUNTAMENTO
        If isRitardatarioReale Then tipo = TipoPrenotazione.RITARDO
        If isSoloBilancio Then tipo = TipoPrenotazione.BILANCIO

        Dim dapp As DateTime = dataAppuntamento.Date

        Dim row As DataRow = dtaNAppuntamenti.Rows.Find(New Object() {dapp})

        If row Is Nothing Then

            Dim nr As DataRow = dtaNAppuntamenti.NewRow()

            nr("CNV_DATA_APPUNTAMENTO") = dapp
            nr("N_NORMALI") = 0
            nr("N_RITARDI") = 0
            nr("N_BILANCI") = 0

            Select Case tipo
                Case TipoPrenotazione.APPUNTAMENTO
                    nr("N_NORMALI") = 1
                Case TipoPrenotazione.RITARDO
                    nr("N_RITARDI") = 1
                Case TipoPrenotazione.BILANCIO
                    nr("N_BILANCI") = 1
            End Select

            dtaNAppuntamenti.Rows.Add(nr)

        Else

            If row.RowState <> DataRowState.Unchanged Then

                Select Case tipo
                    Case TipoPrenotazione.APPUNTAMENTO
                        row("N_NORMALI") += 1
                    Case TipoPrenotazione.RITARDO
                        row("N_RITARDI") += 1
                    Case TipoPrenotazione.BILANCIO
                        row("N_BILANCI") += 1
                End Select

            End If

        End If

    End Sub

    'controlla quali convocati della pagina sono selezionati
    Private Sub ControllaSelezioneConvocati()

        For count As Integer = 0 To dlsRicercaConvocati.Items.Count - 1

            Dim rowFind As DataRow = dtaRicerca.Rows.Find(New Object() {CType(dlsRicercaConvocati.Items(count).FindControl("lblPazCodice"), Label).Text, CDate(CType(dlsRicercaConvocati.Items(count).FindControl("lblDataConvocazione"), Label).Text)})
            rowFind("SEL") = IIf(CType(dlsRicercaConvocati.Items(count).FindControl("chkSelezione"), System.Web.UI.WebControls.CheckBox).Checked, "S", "N")

        Next

    End Sub

    'impaginazione del datalist per la ricerca dei convocati
    Private Sub ImpaginaDataListRicercaConvocati(numPag As Integer, Optional campoOrdinamento As String = Nothing)

        If numPag = 0 Then Exit Sub

        Dim totPagine As Integer

        If dtaRicerca.Rows.Count > 0 Then
            totPagine = dtaRicerca.Rows.Count / NUM_PAZIENTI_PAGINA_RICERCA_CONVOCATI
            If totPagine * NUM_PAZIENTI_PAGINA_RICERCA_CONVOCATI < dtaRicerca.Rows.Count Then totPagine += 1
        Else
            totPagine = 1
        End If

        Dim dtRicercaPagina As DataTable = dtaRicerca.Clone

        'nel caso in cui sia richiesto l'ordinamento
        If campoOrdinamento <> Nothing Then
            Dim dwRicerca As DataView = dtaRicerca.DefaultView
            dwRicerca.Sort = campoOrdinamento

            Dim dtRicercaClone As DataTable = dtaRicerca.Clone
            For count As Integer = 0 To dwRicerca.Count - 1
                Dim rowOrdine As DataRow = dtRicercaClone.NewRow()
                rowOrdine.ItemArray = dwRicerca.Item(count).Row.ItemArray
                dtRicercaClone.Rows.Add(rowOrdine)
            Next
            dtaRicerca = dtRicercaClone
        End If

        Dim row As DataRow
        For count As Integer = ((numPag - 1) * NUM_PAZIENTI_PAGINA_RICERCA_CONVOCATI) To IIf((numPag * NUM_PAZIENTI_PAGINA_RICERCA_CONVOCATI - 1) <= dtaRicerca.Rows.Count - 1, (numPag * NUM_PAZIENTI_PAGINA_RICERCA_CONVOCATI - 1), dtaRicerca.Rows.Count - 1)
            row = dtRicercaPagina.NewRow()
            row.ItemArray = dtaRicerca.Rows(count).ItemArray
            dtRicercaPagina.Rows.Add(row)
        Next

        dlsRicercaConvocati.DataSource = dtRicercaPagina
        dlsRicercaConvocati.DataBind()

        ' Creazione layout pulsanti di spostamento tre le pagine
        Dim stbNumPag As New System.Text.StringBuilder()

        stbNumPag.Append("<SPAN class=""NumeroPagina"">pag.</SPAN>&nbsp;&nbsp;")

        For count As Integer = 1 To numPag - 1
            stbNumPag.AppendFormat("<a href=""Javascript:__doPostBack('CaricaPagina','{0}');"" class=""NumeroPagina"">{0}</a>&nbsp;&nbsp;", count)
        Next
        stbNumPag.AppendFormat("<SPAN class=""NumeroPaginaSel"">{0}</SPAN>&nbsp;&nbsp;", numPag)
        For count As Integer = numPag + 1 To totPagine
            stbNumPag.AppendFormat("<a href=""Javascript:__doPostBack('CaricaPagina','{0}');"" class=""NumeroPagina"">{0}</a>&nbsp;&nbsp;", count)
        Next

        lblNumeroPagine.Text = stbNumPag.ToString()

        nPagina = numPag

    End Sub

    Private Sub Sort(Key As String)

        dtaRicerca.DefaultView.Sort = Key

        'deve ordinare in tutte le pagine del datalist
        ImpaginaDataListRicercaConvocati(nPagina, Key)

        ' Caricamento dei dati ordinati nel dataset per la stampa
        dts_Ricerca.T_RICERCA.Rows.Clear()

        For i As Integer = 0 To dtaRicerca.Rows.Count - 1
            dts_Ricerca.T_RICERCA.ImportRow(dtaRicerca.DefaultView.Item(i).Row)
        Next

    End Sub

    ' Riempie i DataTable dtaMattino e dtaPomeriggio con gli appuntamenti del giorno passato per parametro
    Private Sub BuildDataTableAppuntamenti(dataAppuntamento As String)

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizGestioneAppuntamenti As New Biz.BizGestioneAppuntamenti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Try
                    bizGestioneAppuntamenti.BuildDataTableAppuntamenti(Me.AmbCodice, dataAppuntamento, dtaMattino, dtaPomeriggio, oraAMPM, oraMinPM, LoadedDates)
                Catch ex As Exception
                    Common.Utility.EventLogHelper.EventLogWrite(ex, OnVacContext.AppId)
                    WarningMessage("Errore in <b>BuildDataTableAppuntamenti</b>:", ex)
                End Try

            End Using
        End Using

    End Sub

    Private Sub btnPulisci_Click(sender As System.Object, e As System.EventArgs)

        dtaIndisp.Clear()
        dtaFeste.Clear()

        Me.CaricaCalendario()

    End Sub

    Private Sub btnAppApplica_Click(sender As System.Object, e As System.EventArgs)

        Me.CaricaCalendario()

    End Sub

    ' Contrassegna gli appuntamenti selezionati per l'eliminazione, cancellando la data dell'appuntamento
    Private Sub Sprenota()

        Try
            Dim Paziente As String
            Dim Data As String

            For i As Int16 = 0 To lsMattino.Items.Count - 1

                ' TODO: refactoring Sprenota
                If CType(lsMattino.Items(i).FindControl("chkSelezione"), System.Web.UI.WebControls.CheckBox).Checked Then

                    Paziente = CType(lsMattino.Items(i).FindControl("lblCodice"), Label).Text
                    Data = CType(lsMattino.Items(i).FindControl("lblData"), Label).Text

                    Dim dtr As DataRow() = dtaMattino.Select("PAZ_CODICE='" & Paziente & "' AND CNV_DATA=CONVERT('" & Data & "','System.DateTime')")

                    If dtr.Length > 0 Then

                        WarningMessage(String.Empty, "Rimosso paziente " & dtr(0)("PAZ_COGNOME") & " " & dtr(0)("PAZ_NOME") & " [" & dtr(0)("PAZ_DATA_NASCITA") & "] " &
                                       " con appuntamento " & CType(dtr(0)("CNV_DATA_APPUNTAMENTO"), Date).ToString("dd/MM/yyyy HH:mm"), TipoOperazione.Rimozione_Appuntamento)

                        Dim tipo As String = TipoPrenotazione.APPUNTAMENTO

                        If dtr(0)("SOLLECITO") > 0 Then
                            tipo = TipoPrenotazione.RITARDO
                        ElseIf dtr(0)("VACCINAZIONI").ToString() = "" Or dtr(0)("VACCINAZIONI").ToString().Trim() = "()" Then
                            tipo = TipoPrenotazione.BILANCIO
                        End If

                        Dim dapp As DateTime = dtr(0)("CNV_DATA_APPUNTAMENTO").Date
                        Dim row As DataRow = dtaNAppuntamenti.Rows.Find(New Object() {dapp})

                        If Not row Is Nothing Then
                            Select Case tipo
                                Case TipoPrenotazione.APPUNTAMENTO
                                    If row("N_NORMALI") > 0 Then row("N_NORMALI") -= 1
                                Case TipoPrenotazione.RITARDO
                                    If row("N_RITARDI") > 0 Then row("N_RITARDI") -= 1
                                Case TipoPrenotazione.BILANCIO
                                    If row("N_BILANCI") > 0 Then row("N_BILANCI") -= 1
                            End Select

                            If row("N_NORMALI") = 0 AndAlso row("N_RITARDI") = 0 AndAlso row("N_BILANCI") = 0 Then
                                dtaNAppuntamenti.Rows.Remove(row)
                            End If
                        End If

                        dtr(0)("CNV_DATA_APPUNTAMENTO") = DBNull.Value
                        dtr(0)("CNV_AMB_CODICE") = DBNull.Value

                        'deve ripristinare anche la durata dell'appuntamento secondo il valore
                        'del parametro TEMPOSED solo se non è né un ritardo, né un solo bilancio [modifica 11/08/2005]
                        If dtr(0)("CNV_DURATA_APPUNTAMENTO") > 0 Then
                            If dtr(0)("VACCINAZIONI").ToString().Trim() <> "()" Then
                                If Me.Settings.TEMPOSED > 0 Then dtr(0)("CNV_DURATA_APPUNTAMENTO") = Me.Settings.TEMPOSED
                            Else
                                dtr(0)("CNV_DURATA_APPUNTAMENTO") = 0
                            End If
                        Else
                            dtr(0)("CNV_DURATA_APPUNTAMENTO") = 0
                        End If

                    End If

                End If
            Next

            For i As Int16 = 0 To lsPomeriggio.Items.Count - 1

                ' TODO: refactoring Sprenota
                If CType(lsPomeriggio.Items(i).FindControl("chkSelezione"), System.Web.UI.WebControls.CheckBox).Checked Then

                    Paziente = CType(lsPomeriggio.Items(i).FindControl("lblCodice"), Label).Text
                    Data = CType(lsPomeriggio.Items(i).FindControl("lblData"), Label).Text

                    Dim dtr As DataRow() = dtaPomeriggio.Select("PAZ_CODICE='" & Paziente & "' AND CNV_DATA=CONVERT('" & Data & "','System.DateTime')")

                    If dtr.Length > 0 Then

                        WarningMessage(String.Empty, "Rimosso paziente " & dtr(0)("PAZ_COGNOME") & " " & dtr(0)("PAZ_NOME") & " [" & dtr(0)("PAZ_DATA_NASCITA") & "] " &
                                       " con appuntamento " & CType(dtr(0)("CNV_DATA_APPUNTAMENTO"), Date).ToString("dd/MM/yyyy HH:mm"), TipoOperazione.Rimozione_Appuntamento)

                        Dim tipo As String = TipoPrenotazione.APPUNTAMENTO
                        If dtr(0)("SOLLECITO") > 0 Then
                            tipo = TipoPrenotazione.RITARDO
                        ElseIf dtr(0)("VACCINAZIONI").ToString() = "" Or dtr(0)("VACCINAZIONI").ToString().Trim() = "()" Then
                            tipo = TipoPrenotazione.BILANCIO
                        End If

                        Dim dapp As DateTime = dtr(0)("CNV_DATA_APPUNTAMENTO").Date
                        Dim row As DataRow = dtaNAppuntamenti.Rows.Find(New Object() {dapp})

                        If Not row Is Nothing Then
                            Select Case tipo
                                Case TipoPrenotazione.APPUNTAMENTO
                                    If row("N_NORMALI") > 0 Then row("N_NORMALI") -= 1
                                Case TipoPrenotazione.RITARDO
                                    If row("N_RITARDI") > 0 Then row("N_RITARDI") -= 1
                                Case TipoPrenotazione.BILANCIO
                                    If row("N_BILANCI") > 0 Then row("N_BILANCI") -= 1
                            End Select

                            If row("N_NORMALI") = 0 AndAlso row("N_RITARDI") = 0 AndAlso row("N_BILANCI") = 0 Then
                                dtaNAppuntamenti.Rows.Remove(row)
                            End If

                        End If

                        dtr(0)("CNV_DATA_APPUNTAMENTO") = DBNull.Value
                        dtr(0)("CNV_AMB_CODICE") = DBNull.Value

                        'deve ripristinare anche la durata dell'appuntamento secondo il valore
                        'del parametro TEMPOSED solo se non è né un ritardo, né un solo bilancio

                        If dtr(0)("CNV_DURATA_APPUNTAMENTO") > 0 Then
                            If dtr(0)("VACCINAZIONI").ToString().Trim() <> "()" Then
                                If Me.Settings.TEMPOSED > 0 Then dtr(0)("CNV_DURATA_APPUNTAMENTO") = Me.Settings.TEMPOSED
                            Else
                                dtr(0)("CNV_DURATA_APPUNTAMENTO") = 0
                            End If
                        Else
                            dtr(0)("CNV_DURATA_APPUNTAMENTO") = 0
                        End If

                    End If

                End If

            Next

            If Me.CaricaCalendario() Then
                Me.Modificato = True
            End If

        Catch ex As Exception
            Common.Utility.EventLogHelper.EventLogWrite(ex, OnVacContext.AppId)
            WarningMessage("Errore", ex)
        End Try

    End Sub

    ''' <summary>
    ''' Salvataggio sul DB di tutte le modifiche manuali sugli appuntamenti
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Salva()

        Dim result As Biz.BizGenericResult = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizGestioneAppuntamenti As New Biz.BizGestioneAppuntamenti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), New Biz.BizLogOptions(DataLogStructure.TipiArgomento.APPUNTAMENTO, False))

                result = bizGestioneAppuntamenti.SalvaAppuntamenti(Me.dtaMattino, Me.dtaPomeriggio, OnVacUtility.Variabili.CNS.Codice, "Maschera GestioneAppuntamenti")

            End Using
        End Using

        If Not result.Success Then
            WarningMessage(String.Empty, result.Message, TipoOperazione.Errore)
            Return
        End If

        Me.dtaMattino.Rows.Clear()
        Me.dtaPomeriggio.Rows.Clear()

        Me.LoadedDates.Clear()

        BuildDataTableAppuntamenti(Calendario.SelectedDate.Date)

        WarningMessage(String.Empty, "Dati salvati con successo!", TipoOperazione.Salvataggio)

        Me.Modificato = False
        Me.SpostamentoAppuntamenti = False

    End Sub

    ' Evento sollevato dal CalcoloAppuntamenti prima di effettuare la ricerca di un appuntamento.
    ' Viene gestito per controllare se l'ambulatorio fa parte del consultorio.
    Private Sub OnPreRicercaAppuntamentoLiberoPaziente(cnvPaziente As CnvPaziente, ByRef eseguiRicerca As Boolean, ByRef motivoRifiutoAppuntamento As MotiviRifiuto.IMotivoRifiuto)

        If Not Me.CodiciAmbulatoriConsultorioCorrente.Contains(cnvPaziente.Ambulatorio) Then
            eseguiRicerca = False
            motivoRifiutoAppuntamento = New CalcoloAppuntamenti.MotiviRifiuto.RifiutoAmbulatorioNonInConsultorio()
        Else
            eseguiRicerca = True
            motivoRifiutoAppuntamento = Nothing
        End If

    End Sub

    Private Sub ReloadCodiciAmbulatoriConsultorioCorrente()

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizConsultori As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Me.CodiciAmbulatoriConsultorioCorrente = bizConsultori.GetCodiciAmbulatori(OnVacUtility.Variabili.CNS.Codice, True)

            End Using
        End Using

    End Sub

    Private Sub AggiungiAppuntamentoAutomatico()

        If Me.odpDataInizPrenotazioni.Text = "" OrElse Me.odpDataFinePrenotazioni.Text = "" Then
            WarningMessage(String.Empty, "Valorizzare le date di inizio e fine appuntamento.", TipoOperazione.Info)
            Exit Sub
        End If

        Dim dataMinima As Date = Me.odpDataInizPrenotazioni.Data
        Dim dataMassima As Date = Me.odpDataFinePrenotazioni.Data
        Dim overbooking As Boolean = Me.chkSovrapponiRit.Checked
        Dim fuoriOrario As Boolean = Me.cnsControl

        Dim numeroMassimoNuovi As String = Me.txtNumPazientiAlGiorno.Text
        Dim numeroNuovi As String = Me.txtNumNuoviPazientiAlGiorno.Text

        Me.ReloadCodiciAmbulatoriConsultorioCorrente()

        Dim consultorioCalcoloAppuntamenti As Consultorio = CreateConsultorioCalcoloAppuntamenti()
        Dim ambulatorioCalcoloAppuntamenti As Ambulatorio = CreateAmbulatorioCalcoloAppuntamenti(AmbCodice, overbooking, consultorioCalcoloAppuntamenti)

        If numeroMassimoNuovi <> "" AndAlso IsNumeric(numeroMassimoNuovi) Then

            Dim fmax As New Filtri.FiltroNumeroMassimoAppuntamentiGiornalieri()
            ambulatorioCalcoloAppuntamenti.FiltriVariabili.SetParam(Filtri.FiltroNumeroMassimoAppuntamentiGiornalieri.NumeroMassimoPrenotatiGiornaliero, numeroMassimoNuovi)
            ambulatorioCalcoloAppuntamenti.FiltriVariabili.Add(fmax)

        End If

        '-- aggiunta del filtro per aggiungere N nuovi pazienti al giorno oltre a quelli eventualmente già presenti
        If numeroNuovi <> "" AndAlso IsNumeric(numeroNuovi) Then

            Dim fnuovi As New Filtri.FiltroNumeroMassimoNuoviAppuntamentiGiornalieri()
            ambulatorioCalcoloAppuntamenti.FiltriVariabili.SetParam(Filtri.FiltroNumeroMassimoNuoviAppuntamentiGiornalieri.NumeroMassimoNuoviPrenotatiGiornaliero, numeroNuovi)
            ambulatorioCalcoloAppuntamenti.FiltriVariabili.Add(fnuovi)

        End If

        If dataMinima < DateTime.Today.Date Then dataMinima = DateTime.Today.Date
        consultorioCalcoloAppuntamenti.DaData = dataMinima
        consultorioCalcoloAppuntamenti.AData = dataMassima

        Dim numAppRitardi As Object = Me.Settings.N_RITARDATARI
        If numAppRitardi Is Nothing Then numAppRitardi = 5
        ambulatorioCalcoloAppuntamenti.FiltriVariabili.SetParam(Filtri.FiltroNumeroMassimoRitardatariGiornalieri.NumeroMassimoRitardatariGiornaliero, numAppRitardi)

        consultorioCalcoloAppuntamenti.Ambulatori.Add(ambulatorioCalcoloAppuntamenti)

        Dim pazCollection As CnvPazienteCollection = Me.RestituisciPazienti(overbooking, False)

        If pazCollection.Count = 0 Then
            WarningMessage(String.Empty, "Nessuna convocazione selezionata!", TipoOperazione.Info)
            Exit Sub
        End If

        For Each paz As CnvPaziente In pazCollection
            consultorioCalcoloAppuntamenti.Pazienti.Add(paz)
        Next

        Dim filtriOcc As Filtri.FiltroCollection = Me.CaricaPrenotati(overbooking, dataMinima, dataMassima)
        For Each f As Filtri.IFiltro In filtriOcc
            ambulatorioCalcoloAppuntamenti.FiltriVariabili.Add(f)
        Next

        ' --------- Modifica 7.12.2006 --------- '
        ' La funzione di impostazione dei filtri per la procedura di prenotazione degli appuntamenti è stata modificata per poter prevedere 
        ' gli orari personalizzati (numero variabile di blocchi ogni giorno, con diverse durate e diverso numero di pazienti prenotabili al massimo). 
        ' E' stato creato anche un nuovo tipo di filtro nel CalcoloAppuntamenti, per gestire questa situazione.
        ' I nuovi filtri creati, se presenti, vengono aggiunti ai FiltriVariabili dell'ambulatorio. 
        ' Deve essere anche impostata la proprietà NumeroMassimoBlocchiPersonalizzatiGiornalieri per far sì che la procedura sia più performante.
        '--
        ' N.B. : I filtri ragionano al contrario (impostano le indisponibilità)
        '--
        Dim filtriBlocchi As New ArrayList()

        Dim numBlocchiGiorno As Integer

        Dim filtriOrari As Filtri.FiltroCollection = Me.CaricaFiltriApertura(fuoriOrario, -1, PartiGiornata.Entrambi, filtriBlocchi, numBlocchiGiorno)

        For Each f As Filtri.IFiltro In filtriOrari
            ambulatorioCalcoloAppuntamenti.FiltriAmbulatorio.Add(f)
        Next

        If Not IsNothing(filtriBlocchi) AndAlso filtriBlocchi.Count > 0 Then

            For idxBlocchi As Int16 = 0 To filtriBlocchi.Count - 1
                ambulatorioCalcoloAppuntamenti.FiltriVariabili.SetParam(filtriBlocchi(idxBlocchi)(0), filtriBlocchi(idxBlocchi)(1))
            Next
            ambulatorioCalcoloAppuntamenti.FiltriVariabili.NumeroMassimoBlocchiPersonalizzatiGiornalieri = numBlocchiGiorno

            Dim f As New Filtri.FiltroNumeroMassimoAppuntamentiBlocco()
            ambulatorioCalcoloAppuntamenti.FiltriVariabili.Add(f)

        End If
        ' -------------------------------------- '

        Dim filtriFesta As Filtri.FiltroCollection = Me.CaricaFiltriFeste()
        For Each f As Filtri.IFiltro In filtriFesta
            ambulatorioCalcoloAppuntamenti.FiltriAmbulatorio.Add(f)
        Next

        consultorioCalcoloAppuntamenti.FindAppuntamenti()

        Dim numeroPrenotati As Integer = 0

        Using dam As IDAM = OnVacUtility.OpenDam()

            For Each paz As CnvPaziente In consultorioCalcoloAppuntamenti.Pazienti

                If paz.EsitoProcedura.Successo Then

                    Dim isRitardatarioReale As Boolean = paz.Info("RitardatarioReale")

                    Me.CreaAppuntamento(dam, paz.Codice, paz.DataConvocazione, paz.EsitoProcedura.DataAppuntamento,
                                        paz.DurataAppuntamento, String.Empty, Constants.TipoPrenotazioneAppuntamento.Automatica,
                                        paz.Ritardatario, paz.SoloBilancio, isRitardatarioReale)

                    numeroPrenotati += 1

                End If

            Next

        End Using

        Me.Cerca(False)

        If Me.CaricaCalendario() Then

            Me.AggiungiAlLogTemporaneo(consultorioCalcoloAppuntamenti, TipoOperazione.Prenotazione_Automatica)

            ' Se sono state effettuate prenotazioni scrivo un messaggio col numero di pazienti prenotati con l'ultimo "Prenota"
            If numeroPrenotati > 0 Then
                WarningMessage(String.Empty, "Numero pazienti prenotati: " & numeroPrenotati & ".", TipoOperazione.Info)
            Else
                WarningMessage(String.Empty, "Nessun paziente prenotato.", TipoOperazione.Info)
            End If

        End If

    End Sub

    Private Function CreateConsultorioCalcoloAppuntamenti() As CalcoloAppuntamenti.Consultorio

        Dim consultorioCalcoloAppuntamenti As New Consultorio()
        consultorioCalcoloAppuntamenti.Pazienti.UsaOrdineAlfabetico = chkOrdineAlfabeticoPrenotazione.Checked

        Return consultorioCalcoloAppuntamenti

    End Function

    Private Function CreateAmbulatorioCalcoloAppuntamenti(codiceAmbulatorio As Integer, flagOverbooking As Boolean, consultorioCalcoloAppuntamenti As CalcoloAppuntamenti.Consultorio) As CalcoloAppuntamenti.Ambulatorio

        Dim ambulatorioCalcoloAppuntamenti As New CalcoloAppuntamenti.Ambulatorio(codiceAmbulatorio, flagOverbooking, consultorioCalcoloAppuntamenti)

        AddHandler ambulatorioCalcoloAppuntamenti.OnPreRicercaAppuntamentoLiberoPaziente, AddressOf OnPreRicercaAppuntamentoLiberoPaziente

        Dim ambulatorio As Entities.Ambulatorio = Nothing

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizConsultori As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                ambulatorio = bizConsultori.GetAmbulatorio(codiceAmbulatorio)

            End Using
        End Using

        If Not ambulatorio Is Nothing Then
            ambulatorioCalcoloAppuntamenti.DataAperturaAmbulatorio = ambulatorio.DataApertura
            ambulatorioCalcoloAppuntamenti.DataChiusuraAmbulatorio = ambulatorio.DataChiusura
        End If

        Return ambulatorioCalcoloAppuntamenti

    End Function

    Private Sub AggiungiAlLogTemporaneo(cnv As Consultorio, tipo As TipoOperazione)

        cnv.Pazienti.Sort()

        Dim i As Integer = 0

        For Each p As CnvPaziente In cnv.Pazienti

            Dim str As New System.Text.StringBuilder()
            i = i + 1

            str.AppendFormat("[{6}°]    <b>{0} {1}</b> nato/a il {2}   Convocazione: {3}   {4} {5} <BR> ",
                             p.Cognome, p.Nome, p.DataNascita.ToShortDateString(), p.DataConvocazione.ToShortDateString(),
                             IIf(p.Info("RitardatarioReale") = True, "Ritardatario", ""),
                             IIf(p.SoloBilancio, "Solo Bilancio", ""), i)

            str.AppendFormat("<SPAN style='font-size:10px'>")
            For Each pref As Object In p.Preferenze
                str.AppendFormat("{0}  - Ambulatorio: {1}", pref.ToString(), p.Ambulatorio)
            Next
            str.AppendFormat("</SPAN>")

            If p.EsitoProcedura.Successo Then
                str.AppendFormat("<BR><b>Successo: {0}</b> ", p.EsitoProcedura.DataAppuntamento)
            Else
                str.AppendFormat("<BR><b>Fallito!</b>")
            End If

            str.AppendFormat("<SPAN style='font-size:10px'>")
            For Each rifiuto As MotiviRifiuto.IMotivoRifiuto In p.EsitoProcedura.MotivoRifiuto
                str.AppendFormat("<BR> {0}", rifiuto.ToString())
            Next
            str.AppendFormat("</SPAN>")

            WarningMessage(String.Empty, str.ToString(), IIf(p.EsitoProcedura.Successo, tipo, TipoOperazione.Alert))

        Next

    End Sub

    Function RestituisciPazienti(overbooking As Boolean, calcoloManuale As Boolean) As CnvPazienteCollection

        Dim retPazienti As New CnvPazienteCollection()
        Dim consideraSoloBilancio As Boolean

        ControllaSelezioneConvocati()

        For Each r As DataRow In dtaRicerca.Rows

            If r("SEL") = "S" Then

                'il massimo numero di solleciti deve essere recuperato dal parametro
                Dim isRitardatario As Boolean = overbooking AndAlso r("SOLLECITO") > 0 AndAlso Not Boolean.Parse(r("TP"))
                Dim isRitardatarioReale As Boolean = r("SOLLECITO") > 0 AndAlso Not Boolean.Parse(r("TP"))

                ' -- Gestione dei solo bilanci 20 minuti --
                ' Le convocazioni per solo bilancio devono avere sempre una durata di 0 minuti, mentre prima ai solo bilancio senza pediatra veniva dato un appuntamento di 20 minuti
                ' Se è un solo bilancio
                consideraSoloBilancio = Not IsDBNull(r("SOLOBILANCIO"))

                Dim paz As New CnvPaziente(r("PAZ_CODICE"), r("CNV_DATA"), consideraSoloBilancio, isRitardatario)

                paz.Info.Add("RitardatarioReale", isRitardatarioReale)

                paz.Ambulatorio = AmbCodice

                Try
                    paz.Cognome = r("PAZ_COGNOME")
                    paz.Nome = r("PAZ_NOME")
                    paz.DataConvocazione = r("CNV_DATA")
                    paz.DataNascita = r("PAZ_DATA_NASCITA")

                Catch ex As Exception
                    Dim msg As String = "Errore nei dati del paziente letti da database: " + r("PAZ_CODICE").ToString()
                    Throw New IncorrectDataException(msg, ex)
                End Try

                Dim durata As String = Me.txtDurata.Text

                If IsNumeric(durata) AndAlso durata > 0 Then

                    paz.DurataAppuntamento = durata

                ElseIf Me.Settings.SED_AUTO > 0 Then

                    If (r("TEMPO_BIL") = "N") Then
                        ' Se non è un bilancio senza pediatra
                        paz.DurataAppuntamento = Me.Settings.SED_AUTO
                    Else
                        paz.DurataAppuntamento = r("CNV_DURATA_APPUNTAMENTO")
                    End If

                Else

                    paz.DurataAppuntamento = r("CNV_DURATA_APPUNTAMENTO")

                End If

                paz.Priorita = r("MASSIMO")

                Dim filValidita As New Filtri.FiltroDataValidita
                filValidita.DataCnv = r("CNV_DATA")

                If (Me.Settings.GESDATAVALIDITA) And (IsDBNull(r("SOLOBILANCIO"))) Then

                    If calcoloManuale Then
                        filValidita.DataValidita = Date.MaxValue
                    Else
                        filValidita.DataValidita = r("MASSIMO")
                    End If

                Else

                    filValidita.DataValidita = Date.MaxValue

                End If

                paz.Preferenze.Add(filValidita)


                If Not calcoloManuale Then

                    Dim eta As Integer = Math.Floor(Date.Now.Subtract(r("PAZ_DATA_NASCITA")).TotalDays / 365)
                    Dim parEta As Object = Me.Settings.APPETAPM

                    If Not parEta Is Nothing AndAlso eta >= parEta Then

                        Dim oraPm As Object = Me.Settings.ORAPM

                        If Not oraPm Is Nothing Then

                            Dim filtroPomeriggio As Filtri.FiltroMattinaPomeriggio

                            If chkFiltroPomeriggioObbligatorio.Checked Then
                                filtroPomeriggio = New FiltroMattinaPomeriggioObbligatorio()
                            Else
                                filtroPomeriggio = New Filtri.FiltroMattinaPomeriggio()
                            End If

                            filtroPomeriggio.InizioPomeriggio = Date.Parse(oraPm)
                            filtroPomeriggio.MattinaPomeriggio = Filtri.MattinaPomeriggio.Pomeriggio

                            paz.Preferenze.Add(filtroPomeriggio)

                        End If

                    End If

                    If Not (r("PAZ_GIORNO") Is DBNull.Value OrElse r("PAZ_GIORNO") = 0) Then

                        Dim giornoPreferenza As New Filtri.FiltroGiornoPreferenza(r("PAZ_GIORNO"))
                        paz.Preferenze.Add(giornoPreferenza)

                    End If

                End If

                retPazienti.Add(paz)

            End If

        Next

        Return retPazienti

    End Function

    Function CaricaPrenotati(overbooking As Boolean, dataMinima As Date, dataMassima As Date) As Filtri.FiltroCollection

        Dim filtri As New Filtri.FiltroCollection()

        Dim dta As New DataTable()

        Using dam As IDAM = OnVacUtility.OpenDam()

            dam.QB.NewQuery()

            Dim SQL As String
            SQL = " SELECT da, a, tp"
            SQL = SQL & " FROM "
            SQL = SQL & "   (SELECT cnv_data_appuntamento da,"
            SQL = SQL & "          cnv_data_appuntamento + (cnv_durata_appuntamento / 24 / 60) a,"
            SQL = SQL & "          NVL("
            SQL = SQL & "           (SELECT DISTINCT 'True' "
            SQL = SQL & "           FROM T_CNV_CICLI, T_ANA_TEMPI_SEDUTE"
            SQL = SQL & "           WHERE cnc_cnv_paz_codice = cnv_paz_codice AND cnc_cnv_data = cnv_data"
            SQL = SQL & "           AND cnc_cic_codice = tsd_cic_codice AND cnc_sed_n_seduta = tsd_n_seduta"
            SQL = SQL & "           AND cnc_n_sollecito > CASE WHEN NVL(tsd_num_solleciti, 0) > 0 THEN tsd_num_solleciti ELSE " & Me.Settings.NUMSOL & " END"
            SQL = SQL & "           AND NVL(tsd_num_solleciti_rac, 0) = 0"
            SQL = SQL & "           AND EXISTS "
            SQL = SQL & "               (SELECT 1 "
            SQL = SQL & "               FROM V_ANA_ASS_VACC_SEDUTE, T_ANA_VACCINAZIONI "
            SQL = SQL & "               WHERE sas_vac_codice = vac_codice"
            SQL = SQL & "               AND VAC_OBBLIGATORIA = 'A'"
            SQL = SQL & "               AND SAS_CIC_CODICE = CNC_CIC_CODICE"
            SQL = SQL & "               AND SAS_N_SEDUTA = CNC_SED_N_SEDUTA))"
            SQL = SQL & "               ,'False') tp"
            SQL = SQL & "   FROM T_CNV_CONVOCAZIONI"
            SQL = SQL & "   WHERE cnv_data_appuntamento >= TO_DATE('" & dataMinima & "','dd/MM/yyyy') "
            SQL = SQL & "   AND cnv_data_appuntamento < TO_DATE('" & dataMassima.AddDays(1) & "','dd/MM/yyyy')"
            SQL = SQL & "   AND cnv_amb_codice = '" & AmbCodice & "')"
            SQL = SQL & " ORDER BY a"

            dam.BuildDataTable(SQL, dta)

        End Using

        For Each r As DataRow In dta.Rows

            If Not LoadedDates.Contains(r("da").date) Then

                Dim isRitardatario As Boolean = overbooking AndAlso Not Boolean.Parse(r("tp"))
                Dim dataApp As Date = r("da")
                Dim inizio As Date = dataApp
                Dim fine As Date = r("a")
                Dim fOcc As New Filtri.FiltroOccupato(dataApp.Date, inizio, fine)

                filtri.Add(fOcc)

                If isRitardatario Then
                    Dim fRit As New Filtri.FiltroNumeroMassimoRitardatariGiornalieri(dataApp, inizio, fine)
                    filtri.Add(fRit)
                End If

            End If

        Next

        For Each r As DataRow In dtaMattino.Rows

            If Not IsDBNull(r("CNV_DATA_APPUNTAMENTO")) Then

                Dim isRitardatario As Boolean = overbooking AndAlso r("SOLLECITO") > 0 AndAlso Not Boolean.Parse(r("TP"))

                Dim dataApp As Date = r("CNV_DATA_APPUNTAMENTO")
                Dim inizio As Date = dataApp
                Dim fine As Date = CType(r("CNV_DATA_APPUNTAMENTO"), Date).AddMinutes(r("CNV_DURATA_APPUNTAMENTO"))
                Dim fOcc As New Filtri.FiltroOccupato(dataApp.Date, inizio, fine)

                filtri.Add(fOcc)

                If isRitardatario Then
                    Dim fRit As New Filtri.FiltroNumeroMassimoRitardatariGiornalieri(dataApp, inizio, fine)
                    filtri.Add(fRit)
                End If

            End If

        Next

        For Each r As DataRow In dtaPomeriggio.Rows

            If Not IsDBNull(r("CNV_DATA_APPUNTAMENTO")) Then

                Dim isRitardatario As Boolean = overbooking AndAlso r("SOLLECITO") > 0 AndAlso Not Boolean.Parse(r("TP"))

                Dim dataApp As Date = r("CNV_DATA_APPUNTAMENTO")
                Dim inizio As Date = dataApp
                Dim fine As Date = CType(r("CNV_DATA_APPUNTAMENTO"), Date).AddMinutes(r("CNV_DURATA_APPUNTAMENTO"))
                Dim fOcc As New Filtri.FiltroOccupato(dataApp.Date, inizio, fine)

                filtri.Add(fOcc)

                If isRitardatario Then
                    Dim fRit As New Filtri.FiltroNumeroMassimoRitardatariGiornalieri(dataApp, inizio, fine)
                    filtri.Add(fRit)
                End If

            End If

        Next

        Return filtri

    End Function

    ' Modifica 7.12.2006: i blocchi personalizzati, se ci sono, devono essere aggiunti ai filtri variabili dell'ambulatorio
    Function CaricaFiltriApertura(fuoriOrario As Boolean, giorno As DayOfWeek, parte As PartiGiornata, ByRef FiltriVar As ArrayList, ByRef maxBlocchiGiorno As Integer) As Filtri.FiltroCollection

        Dim filtri As Filtri.FiltroCollection = Nothing

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizGestioneAppuntamenti As New Biz.BizGestioneAppuntamenti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                ' Data iniziale e finale di prenotazione
                Dim dataMinima As Date
                Dim dataMassima As Date

                If Me.chkOrariPers.Checked Then
                    dataMinima = Me.odpDataInizPrenotazioni.Data
                    dataMassima = Me.odpDataFinePrenotazioni.Data
                End If

                Return bizGestioneAppuntamenti.CaricaFiltriApertura(
                    Me.AmbCodice, fuoriOrario, giorno, parte, FiltriVar, maxBlocchiGiorno, Me.chkOrariPers.Checked, dataMinima, dataMassima, Me.dtOrariPersonalizzati)

            End Using
        End Using

    End Function

    Function CaricaFiltriFeste() As Filtri.FiltroCollection

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizGestioneAppuntamenti As New Biz.BizGestioneAppuntamenti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Return bizGestioneAppuntamenti.CaricaFiltriFeste(Me.AmbCodice)

            End Using
        End Using

    End Function

#Region " Toolbar Events "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "SpostaPrenotazioni"

                SpostaPrenotazioni()

            Case "Sprenota"

                Sprenota()

                eliminataPrenotazione = True

            Case "Salva"

                Me.Salva()

                ' Deve rifare la ricerca solo se, nella maschera, è impostata una data di convocazione
                If Not String.IsNullOrWhiteSpace(Me.txtFinoAData.Text) Then

                    If Me.eliminataPrenotazione Then
                        Me.Cerca(True)
                    End If

                End If

                Me.eliminataPrenotazione = False

                Me.CaricaCalendario()

            Case "Cerca"

                btnCerca_Click(sender, e)

            Case "Pazienti"

                Me.GoToDettaglioPaziente()

            Case "StampaElenco"

                StampaElencoConvocati()

            Case "VisualizzaLog"

                VisualizzaLog()

        End Select

    End Sub

#End Region

#Region " Redirect a DettaglioPaziente "

    Private Sub GoToDettaglioPaziente()

        If Modificato Then

            Dim strScript As String = "alert('Non e\' possibile visualizzare il dettaglio paziente poiche\' c\'e\' una operazione di prenotazione o eliminazione prenotazione in corso. Premere Salva o Annulla prima di eseguire questa operazione!');"
            RegisterStartupScriptCustom("GoToDettaglioPaziente1", strScript)

        Else

            Dim codicePazienteSelezionato As String = CercaPazienteInList()

            If codicePazienteSelezionato = String.Empty Then
                RegisterStartupScriptCustom("GoToDettaglioPaziente2", "alert('Selezionare un paziente per visualizzarne i dati');")
            Else

                If GetRichiestaConsensoTrattamentoDatiUtente(OnVacUtility.Variabili.CNS.Codice) Then

                    ucConsensoUtente.CodicePaziente = Convert.ToInt64(codicePazienteSelezionato)
                    ucConsensoUtente.CodiceConsultorio = OnVacUtility.Variabili.CNS.Codice
                    ucConsensoUtente.Destinazione = ConsensoTrattamentoDatiUtente.DestinazioneRedirect.DettaglioPaziente

                    fmConsensoUtente.VisibileMD = True

                Else

                    FiltriMaschera = GetFiltriRicerca()
                    FiltriMaschera.SetFiltriMaschera = Settings.GES_APP_RICORDA_FILTRI AndAlso RicercaEffettuata

                    ' Memorizzazione codice paziente per ricerca rapida (l'impostazione del paziente corrente avviene nel metodo RedirectToGestionePaziente)
                    UltimoPazienteSelezionato = New Entities.UltimoPazienteSelezionato(String.Empty, codicePazienteSelezionato)

                    RedirectToGestionePaziente(codicePazienteSelezionato)

                End If
            End If

        End If

    End Sub

    ''' <summary>
    ''' Restituisce il codice del primo paziente selezionato in uno dei datalist, nell'ordine: dlsRicercaConvocati, lsMattino e lsPomeriggio
    ''' Se non trova nessun paziente selezionato, restituisce la stringa vuota.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CercaPazienteInList() As String

        ' Ricerca del primo paziente selezionato nel datalist contenente i risultati della ricerca dei convocati.
        Dim codicePazienteSelezionato As String = Me.SearchCodPazInList(dlsRicercaConvocati, "chkSelezione", "lblPazCodice")
        If codicePazienteSelezionato <> String.Empty Then Return codicePazienteSelezionato

        ' Se non ha trovato nessun paz selezionato tra i convocati, cerca nel datalist degli appuntamenti del mattino
        codicePazienteSelezionato = Me.SearchCodPazInList(lsMattino, "chkSelezione", "lblCodice")
        If codicePazienteSelezionato <> String.Empty Then Return codicePazienteSelezionato

        ' Se non ha trovato nessun paz selezionato al mattino, cerca nel datalist degli appuntamenti del pomeriggio
        codicePazienteSelezionato = Me.SearchCodPazInList(lsPomeriggio, "chkSelezione", "lblCodice")

        Return codicePazienteSelezionato

    End Function

    Private Function SearchCodPazInList(ByRef list As DataList, idChk As String, idLbl As String) As String

        For i As Integer = 0 To list.Items.Count - 1
            If DirectCast(list.Items(i).FindControl(idChk), System.Web.UI.WebControls.CheckBox).Checked Then
                Return DirectCast(list.Items(i).FindControl(idLbl), Label).Text
            End If
        Next

        Return String.Empty

    End Function

#End Region

    'visualizza il log delle operazioni [modifica 26/04/2005]
    Private Sub VisualizzaLog()

        Dim log As New System.Text.StringBuilder()
        Dim tipoOperazione As String = String.Empty
        Dim dataOperazione As Date

        For Each row As dtsLog.T_LOGRow In dts_log.T_LOG
            dataOperazione = row.Data
            tipoOperazione = [Enum].GetName(GetType(TipoOperazione), row.TipoOperazione).ToString().Replace("_", " ")

            Select Case row.TipoMessaggio
                Case "alert"
                    log.AppendFormat("<HR>{0}<B>{1} {2} - {3}</B><BR>{4}<BR>", ALERTICO, dataOperazione.ToShortDateString(), dataOperazione.ToLongTimeString(), tipoOperazione, row.Messaggio)
                Case "conferma"
                    log.AppendFormat("<HR>{0}<B>{1} {2} - {3}</B><BR>{4}<BR>", CONFERMA, dataOperazione.ToShortDateString(), dataOperazione.ToLongTimeString(), tipoOperazione, row.Messaggio)
            End Select
        Next

        Me.lblLog.Text = log.ToString()

        Me.fmLog.VisibileMD = True
        Me.OnitLayout31.Busy = True

    End Sub

#Region " Button events "

    Private Sub btnAssegna_Click(sender As System.Object, e As System.Web.UI.ImageClickEventArgs) Handles btnAssegna.Click

        If Me.txtVaiAData.Text <> "" Then

            Me.Calendario.TodaysDate = Me.txtVaiAData.Text

            Try
                Me.Calendario.SelectedDate = Me.txtVaiAData.Text
            Catch ex As Exception
                Common.Utility.EventLogHelper.EventLogWrite(ex, OnVacContext.AppId)
                WarningMessage("Errore in <b>btnAssegna_Click</b>", ex)
            End Try

            Me.CaricaCalendario()

        End If

    End Sub

    'chiude la modale del messaggio di orari di apertura consultorio (modifica 21/07/2004)
    Private Sub btnOrariApertura_Click(sender As Object, e As System.EventArgs) Handles btnOrariApertura.Click

        Me.fmPopUp.VisibileMD = False

    End Sub

    'chiude la modale del log [modifica 26/04/2005]
    Private Sub btnChiudiLog_Click(sender As Object, e As System.EventArgs) Handles btnChiudiLog.Click

        Me.lblLog.Text = String.Empty
        Me.fmLog.VisibileMD = False
        Me.OnitLayout31.Busy = False

    End Sub

    Private Sub btnImgAssociazioniDosi_Click(sender As Object, e As System.EventArgs) Handles btnImgAssociazioniDosi.Click

        Me.UscFiltroAssociazioniDosi.setValoriSelezionatiFiltro1(FiltriMaschera.dtFiltroAssociazioniSel)
        Me.UscFiltroAssociazioniDosi.setValoriSelezionatiFiltro2(FiltriMaschera.dtFiltroDosiSel)

        ' Il flag Modificato gestisce il blocco del layout (nel prerender)
        Me.FinestraModaleVisibile = True
        Me.fmFiltroAssociazioniDosi.VisibileMD = True

    End Sub

    Private Sub btnOk_FiltroAssociazioniDosi_Click(sender As System.Object, e As System.EventArgs) Handles btnOk_FiltroAssociazioniDosi.Click

        Me.FinestraModaleVisibile = False
        Me.fmFiltroAssociazioniDosi.VisibileMD = False

        FiltriMaschera.dtFiltroAssociazioniSel = Me.UscFiltroAssociazioniDosi.getValoriSelezionatiFiltro1()
        FiltriMaschera.dtFiltroDosiSel = Me.UscFiltroAssociazioniDosi.getValoriSelezionatiFiltro2()

        Me.lblAssociazioniDosi.Text = Me.UscFiltroAssociazioniDosi.getStringaFormattata()

    End Sub

    Private Sub btnAnnulla_FiltroAssociazioniDosi_Click(sender As System.Object, e As System.EventArgs) Handles btnAnnulla_FiltroAssociazioniDosi.Click

        Me.FinestraModaleVisibile = False
        Me.fmFiltroAssociazioniDosi.VisibileMD = False

    End Sub

    Private Sub btnImgCicliSedute_Click(sender As Object, e As System.EventArgs) Handles btnImgCicliSedute.Click

        Me.UscFiltroCicliSedute.setValoriSelezionatiFiltro1(FiltriMaschera.dtFiltroCicliSel)
        Me.UscFiltroCicliSedute.setValoriSelezionatiFiltro2(FiltriMaschera.dtFiltroSeduteSel)

        Me.FinestraModaleVisibile = True
        Me.fmFiltroCicliSedute.VisibileMD = True

    End Sub

    Private Sub btnOk_FiltroCicliSedute_Click(sender As System.Object, e As System.EventArgs) Handles btnOk_FiltroCicliSedute.Click

        Me.FinestraModaleVisibile = False
        Me.fmFiltroCicliSedute.VisibileMD = False

        FiltriMaschera.dtFiltroCicliSel = Me.UscFiltroCicliSedute.getValoriSelezionatiFiltro1()
        FiltriMaschera.dtFiltroSeduteSel = Me.UscFiltroCicliSedute.getValoriSelezionatiFiltro2()

        Me.lblCicliSedute.Text = Me.UscFiltroCicliSedute.getStringaFormattata()

    End Sub

    Private Sub btnAnnulla_FiltroCicliSedute_Click(sender As System.Object, e As System.EventArgs) Handles btnAnnulla_FiltroCicliSedute.Click

        Me.FinestraModaleVisibile = False
        Me.fmFiltroCicliSedute.VisibileMD = False

    End Sub

#End Region

    ' Restituisce la presenza dell'appuntamento negli orari della mattina (true) o del pomeriggio
    Private Function InMattino(ByRef DAM As IDAM, appuntamento As DateTime) As Boolean

        Using genericProvider As New DbGenericProvider(DAM)
            Using bizGestioneAppuntamenti As New Biz.BizGestioneAppuntamenti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Return bizGestioneAppuntamenti.InMattino(Me.AmbCodice, Me.cnsControl, appuntamento)

            End Using
        End Using

    End Function

#Region " Datagrid events "

    'valorizza l'immagine con lo stato del paziente
    Private Sub dgrPazientiDaSollecitare_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgrPazientiDaSollecitare.ItemDataBound

        If e.Item.ItemIndex <> -1 Then

            Dim pazienteCicloObbligatorioNoTP As Common.Solleciti.PazienteCicloObbligatorioNoTP = e.Item.DataItem
            Dim imgDescrizioneStatoPazientiDaSollecitare As System.Web.UI.WebControls.Image = DirectCast(e.Item.FindControl("imgDescrizioneStatoPazientiDaSollecitare"), System.Web.UI.WebControls.Image)

            imgDescrizioneStatoPazientiDaSollecitare.ImageUrl =
                IIf((pazienteCicloObbligatorioNoTP.TipoStato = Common.Solleciti.TipoStatoPazientiDaSollecitare.Sollecito) Or
                    (pazienteCicloObbligatorioNoTP.TipoStato = Common.Solleciti.TipoStatoPazientiDaSollecitare.SollecitoRaccomandato),
                    ICONA_STATO_SOLLECITO, ICONA_STATO_TERMINE_PERENTORIO)

            'aggiunge la descrizione delle vaccinazioni
            Select Case pazienteCicloObbligatorioNoTP.TipoStato

                Case Common.Solleciti.TipoStatoPazientiDaSollecitare.Sollecito,
                     Common.Solleciti.TipoStatoPazientiDaSollecitare.SollecitoRaccomandato

                    pazienteCicloObbligatorioNoTP.DescrizioneStato = String.Format("Al paziente verrà creato il {0}° sollecito per le seguenti vaccinazioni: {1} {2}",
                                                                                   (pazienteCicloObbligatorioNoTP.NumSollecitoSeduta + 1).ToString(),
                                                                                   GetElencoCodiciVaccinazioni(pazienteCicloObbligatorioNoTP.Vaccinazioni),
                                                                                   GetDescrizioneCicloSeduta(pazienteCicloObbligatorioNoTP.Ciclo, pazienteCicloObbligatorioNoTP.Seduta))

                    If pazienteCicloObbligatorioNoTP.TipoStato = Common.Solleciti.TipoStatoPazientiDaSollecitare.SollecitoRaccomandato Then
                        pazienteCicloObbligatorioNoTP.DescrizioneStato += String.Format(", dato che il numero massimo di solleciti raccomandati sulla seduta è: {0}.", pazienteCicloObbligatorioNoTP.MaxSolleciti.ToString())
                    Else
                        pazienteCicloObbligatorioNoTP.DescrizioneStato += "."
                    End If

                Case Common.Solleciti.TipoStatoPazientiDaSollecitare.TerminePerentorio

                    If pazienteCicloObbligatorioNoTP.CodiceStatoAnagrafico <> "2" Then

                        pazienteCicloObbligatorioNoTP.DescrizioneStato = "Lo stato del paziente sarà passato a 'Termine Perentorio' in quanto " &
                            IIf(pazienteCicloObbligatorioNoTP.IsMaxSollecitiSeduta,
                                "il numero massimo di solleciti sulle sedute delle seguenti vaccinazioni: " + GetElencoCodiciVaccinazioni(pazienteCicloObbligatorioNoTP.Vaccinazioni) + " " + GetDescrizioneCicloSeduta(pazienteCicloObbligatorioNoTP.Ciclo, pazienteCicloObbligatorioNoTP.Seduta) + " è: " & pazienteCicloObbligatorioNoTP.MaxSolleciti,
                                "il numero massimo di solleciti impostato a livello globale è: " & pazienteCicloObbligatorioNoTP.MaxSolleciti) &
                            "."

                    ElseIf pazienteCicloObbligatorioNoTP.CodiceStatoAnagrafico = "2" Then

                        pazienteCicloObbligatorioNoTP.DescrizioneStato = "Al paziente verrà creata l'inadempienza in quanto " &
                            IIf(pazienteCicloObbligatorioNoTP.IsMaxSollecitiSeduta,
                                "il numero massimo di solleciti sulle sedute delle seguenti vaccinazioni: " + GetElencoCodiciVaccinazioni(pazienteCicloObbligatorioNoTP.Vaccinazioni) + " " + GetDescrizioneCicloSeduta(pazienteCicloObbligatorioNoTP.Ciclo, pazienteCicloObbligatorioNoTP.Seduta) + " è: " & pazienteCicloObbligatorioNoTP.MaxSolleciti,
                                "il numero massimo di solleciti impostato a livello globale è: " & pazienteCicloObbligatorioNoTP.MaxSolleciti) &
                            " ed il paziente è DOMICILIATO."

                    End If

            End Select

            imgDescrizioneStatoPazientiDaSollecitare.ToolTip = pazienteCicloObbligatorioNoTP.DescrizioneStato

            e.Item.Cells(2).Text = pazienteCicloObbligatorioNoTP.NumSollecitoSeduta + 1

        End If

    End Sub

    'valorizza l'immagine con lo stato del paziente 
    Private Sub dgrPazientiVaccinazioniNonObbligatorie_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgrPazientiVaccinazioniNonObbligatorie.ItemDataBound

        If e.Item.ItemIndex <> -1 Then

            Dim pazienteCicloNonObbligatorio As Common.Solleciti.PazienteCicloNonObbligatorio = e.Item.DataItem

            Dim imgDescrizioneStatoVaccinazioniNonObbligatorie As System.Web.UI.WebControls.Image =
                DirectCast(e.Item.FindControl("imgDescrizioneStatoVaccinazioniNonObbligatorie"), System.Web.UI.WebControls.Image)

            Select Case pazienteCicloNonObbligatorio.TipoStato

                Case Common.Solleciti.TipoStatoPazientiVaccinazioniNonObbligatorie.EsclusioneStandard

                    imgDescrizioneStatoVaccinazioniNonObbligatorie.ImageUrl = ICONA_STATO_ESCLUSIONE_STANDARD

                    pazienteCicloNonObbligatorio.DescrizioneStato = String.Format("Al paziente verrà creata l'esclusione per le seguenti vaccinazioni: {0} {1}.",
                                                                                  GetElencoCodiciVaccinazioni(pazienteCicloNonObbligatorio.Vaccinazioni),
                                                                                  GetDescrizioneCicloSeduta(pazienteCicloNonObbligatorio.Ciclo, pazienteCicloNonObbligatorio.Seduta))

                Case Common.Solleciti.TipoStatoPazientiVaccinazioniNonObbligatorie.PosticipoGiorni

                    imgDescrizioneStatoVaccinazioniNonObbligatorie.ImageUrl = ICONA_STATO_POSTICIPO_GIORNI

                    pazienteCicloNonObbligatorio.DescrizioneStato = String.Format("Le vaccinazioni: {0} {1} del paziente verranno posticipate di {2} giorni come indicato nel parametro relativo.",
                                                                                  GetElencoCodiciVaccinazioni(pazienteCicloNonObbligatorio.Vaccinazioni),
                                                                                  GetDescrizioneCicloSeduta(pazienteCicloNonObbligatorio.Ciclo, pazienteCicloNonObbligatorio.Seduta),
                                                                                  pazienteCicloNonObbligatorio.GiorniPosticipo.ToString())

                Case Common.Solleciti.TipoStatoPazientiVaccinazioniNonObbligatorie.PosticipoSeduta

                    imgDescrizioneStatoVaccinazioniNonObbligatorie.ImageUrl = ICONA_STATO_POSTICIPO_SEDUTA

                    pazienteCicloNonObbligatorio.DescrizioneStato = String.Format("Le vaccinazioni: {0} {1} del paziente verranno posticipate alla prima convocazione obbligatoria come indicato nel parametro relativo. Nel caso in cui non esistano convocazioni obbligatorie successive o non sia possibile calcolarle verrà creata automaticamente l'esclusione.",
                                                                                  GetElencoCodiciVaccinazioni(pazienteCicloNonObbligatorio.Vaccinazioni),
                                                                                  GetDescrizioneCicloSeduta(pazienteCicloNonObbligatorio.Ciclo, pazienteCicloNonObbligatorio.Seduta))

                Case Common.Solleciti.TipoStatoPazientiVaccinazioniNonObbligatorie.SollecitoStandard

                    imgDescrizioneStatoVaccinazioniNonObbligatorie.ImageUrl = ICONA_STATO_SOLLECITO_STANDARD

                    pazienteCicloNonObbligatorio.DescrizioneStato = String.Format("Al paziente verrà creato il {0}° sollecito sulle seguenti vaccinazioni: {1} {2}.",
                                                                                  (pazienteCicloNonObbligatorio.NumSollecitoSeduta + 1).ToString(),
                                                                                  GetElencoCodiciVaccinazioni(pazienteCicloNonObbligatorio.Vaccinazioni),
                                                                                  GetDescrizioneCicloSeduta(pazienteCicloNonObbligatorio.Ciclo, pazienteCicloNonObbligatorio.Seduta))

            End Select

            imgDescrizioneStatoVaccinazioniNonObbligatorie.ToolTip = pazienteCicloNonObbligatorio.DescrizioneStato

        End If

    End Sub

    'valorizza l'immagine con lo stato del paziente
    Private Sub dgrPazientiInTerminePerentorio_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgrPazientiInTerminePerentorio.ItemDataBound

        If e.Item.ItemIndex <> -1 Then

            Dim pazienteCicloObbligatorioInTP As Common.Solleciti.PazienteCicloObbligatorioInTP = e.Item.DataItem

            Dim imgDescrizioneStatoTerminePerentorio As System.Web.UI.WebControls.Image =
                DirectCast(e.Item.FindControl("imgDescrizioneStatoTerminePerentorio"), System.Web.UI.WebControls.Image)

            pazienteCicloObbligatorioInTP.DescrizioneStato = String.Format("Il paziente risulta in 'Termine Perentorio' per le vaccinazioni: {0} {1} in quanto il numero massimo di solleciti per le medesime è: {2}",
                                                                           GetElencoCodiciVaccinazioni(pazienteCicloObbligatorioInTP.Vaccinazioni),
                                                                           GetDescrizioneCicloSeduta(pazienteCicloObbligatorioInTP.Ciclo, pazienteCicloObbligatorioInTP.Seduta),
                                                                           pazienteCicloObbligatorioInTP.MaxSolleciti.ToString())

            imgDescrizioneStatoTerminePerentorio.ImageUrl = ICONA_STATO_TERMINE_PERENTORIO
            imgDescrizioneStatoTerminePerentorio.ToolTip = pazienteCicloObbligatorioInTP.DescrizioneStato

        End If

    End Sub

    'valorizza l'immagine con lo stato del paziente
    Private Sub dgrPazientiVaccinazioniRaccomandate_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgrPazientiVaccinazioniRaccomandate.ItemDataBound

        If e.Item.ItemIndex <> -1 Then

            Dim pazienteCicloRaccomandatoTerminato As Common.Solleciti.PazienteCicloRaccomandatoTerminato = e.Item.DataItem

            Dim imgDescrizioneStatoVaccinazioniRaccomandate As System.Web.UI.WebControls.Image =
                DirectCast(e.Item.FindControl("imgDescrizioneStatoVaccinazioniRaccomandate"), System.Web.UI.WebControls.Image)

            pazienteCicloRaccomandatoTerminato.DescrizioneStato = String.Format("Al paziente verrà creata l'esclusione per le seguenti vaccinazioni: {0} {1}, in quanto il numero massimo di solleciti sulla seduta è: {2}",
                                                                                GetElencoCodiciVaccinazioni(pazienteCicloRaccomandatoTerminato.Vaccinazioni),
                                                                                GetDescrizioneCicloSeduta(pazienteCicloRaccomandatoTerminato.Ciclo, pazienteCicloRaccomandatoTerminato.Seduta),
                                                                                pazienteCicloRaccomandatoTerminato.MaxSolleciti.ToString())

            imgDescrizioneStatoVaccinazioniRaccomandate.ImageUrl = ICONA_STATO_ESCLUSIONE_STANDARD
            imgDescrizioneStatoVaccinazioniRaccomandate.ToolTip = pazienteCicloRaccomandatoTerminato.DescrizioneStato

        End If

    End Sub

    'valorizza l'immagine con lo stato del paziente 
    Private Sub dgrPazientiNoCiclo_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgrPazientiNoCiclo.ItemDataBound

        If e.Item.ItemIndex <> -1 Then

            Dim pazienteNoCiclo As Common.Solleciti.PazienteNoCiclo = e.Item.DataItem

            Dim imgDescrizioneStatoNoCiclo As System.Web.UI.WebControls.Image =
                DirectCast(e.Item.FindControl("imgDescrizioneStatoNoCiclo"), System.Web.UI.WebControls.Image)

            If Me.Settings.ESCLUDISENOCICLO Then

                pazienteNoCiclo.DescrizioneStato = String.Format("Al paziente verrà creata l'esclusione per le seguenti vaccinazioni: {0}, in quanto non associate a nessun ciclo.",
                                                                 GetElencoCodiciVaccinazioni(pazienteNoCiclo.Vaccinazioni))

                imgDescrizioneStatoNoCiclo.ImageUrl = ICONA_STATO_ESCLUSIONE_STANDARD
                imgDescrizioneStatoNoCiclo.ToolTip = pazienteNoCiclo.DescrizioneStato

            Else

                pazienteNoCiclo.DescrizioneStato = String.Format("Al paziente verrà cancellato l'appuntamento per le seguenti vaccinazioni: {0}, in quanto non associate a nessun ciclo.",
                                                                 GetElencoCodiciVaccinazioni(pazienteNoCiclo.Vaccinazioni))

                imgDescrizioneStatoNoCiclo.ImageUrl = ICONA_STATO_SOLLECITO
                imgDescrizioneStatoNoCiclo.ToolTip = pazienteNoCiclo.DescrizioneStato

            End If

        End If

    End Sub

    'valorizza l'immagine con lo stato del paziente 
    Private Sub dgrDisallineati_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgrDisallineati.ItemDataBound

        If e.Item.ItemIndex <> -1 Then

            Dim pazienteDisallineato As Common.Solleciti.PazienteDisallineato = e.Item.DataItem

            Dim imgDescrizioneStatoDisallineati As System.Web.UI.WebControls.Image =
                DirectCast(e.Item.FindControl("imgDescrizioneStatoDisallineati"), System.Web.UI.WebControls.Image)

            pazienteDisallineato.DescrizioneStato = String.Format("Il paziente con appuntamento per le seguenti vaccinazioni: {0} non verrà processato in quanto disallineato.",
                                                                  GetElencoCodiciVaccinazioni(pazienteDisallineato.Vaccinazioni))

            imgDescrizioneStatoDisallineati.ImageUrl = ICONA_STATO_SOLLECITO
            imgDescrizioneStatoDisallineati.ToolTip = pazienteDisallineato.DescrizioneStato

        End If

    End Sub

    ''' <summary>
    ''' Restituice una stringa contenente il codice delle vaccinazioni specificate, separate da virgola.
    ''' </summary>
    ''' <param name="vaccinazioni"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetElencoCodiciVaccinazioni(vaccinazioni As Collection(Of Common.Solleciti.Vaccinazione)) As String

        If vaccinazioni.IsNullOrEmpty() Then Return String.Empty

        Return String.Join(", ", vaccinazioni.Select(Function(p) p.Codice).ToArray())

    End Function

    Private Function GetDescrizioneCicloSeduta(codiceCiclo As String, numeroSeduta As Integer) As String

        Return String.Format("[ cic. {0} / sed. {1} ]", codiceCiclo, numeroSeduta.ToString())

    End Function

#End Region

#Region " Spostamento Prenotazione "

    Private Sub SpostaPrenotazioni()

        Me.FinestraModaleVisibile = True   ' Nel PreRender mette OnitLayout31.Busy a true o false a seconda di FinestraModaleVisibile... era già così
        Me.fmSpostaPrenotazioni.VisibileMD = True

        If Me.unSoloAmbulatorio Then
            Me.SpostaSceltaAmb.Codice = Me.AmbCodice
            Me.SpostaSceltaAmb.Descrizione = Me.AmbDescrizione
            Me.SpostaSceltaAmb.databind()
        End If

    End Sub

    Private Sub btnOkSposta_Click(sender As Object, e As System.EventArgs) Handles btnOkSposta.Click

        ' Controllo che sia stata compilata la data di spostamento
        If String.IsNullOrWhiteSpace(Me.dpkDataSpostamento.Text) Then
            WarningMessage(String.Empty, "Spostamento non effettuato: nessuna data di spostamento inserita.", TipoOperazione.Info)
            Exit Sub
        End If

        ' Controllo che sia stata compilata la data di spostamento
        If Me.dpkDataSpostamento.Data < DateTime.Today.Date Then
            WarningMessage(String.Empty, "Spostamento non effettuato: data di spostamento inferiore alla data odierna.", TipoOperazione.Info)
            Exit Sub
        End If
        If SpostaSceltaAmb.Codice.IsNullOrEmpty Then
            WarningMessage(String.Empty, "Spostamento non effettuato: ambulatorio obbligatorio.", TipoOperazione.Info)
            Exit Sub
        End If
        ' Controlla che non ci siano già convocazioni nella data impostata dall'utente
        If DataSpostamentoOccupata(Me.SpostaSceltaAmb.Codice) Then
            WarningMessage(String.Empty, "Spostamento non effettuato: sono già presenti convocazioni nella data selezionata.", TipoOperazione.Info)
            Exit Sub
        End If

        Dim count As Integer = 0

        Try
            Dim codicePaziente As String
            Dim dataConvocazione As String
            Dim tipo As String
            Dim dapp_old, dapp_new As DateTime
            Dim amb_old, amb_new As Integer
            Dim row_old, row_new As DataRow

            Dim MattinaPomeriggio() As String = {"Mattino", "Pomeriggio"}
            Dim currentBlock As String

            For Each currentBlock In MattinaPomeriggio

                Dim dtlist As DataList
                Dim dt As DataTable

                If currentBlock = "Mattino" Then
                    dtlist = lsMattino
                    dt = dtaMattino
                Else
                    dtlist = lsPomeriggio
                    dt = dtaPomeriggio
                End If

                For i As Int16 = 0 To dtlist.Items.Count - 1
                    If DirectCast(dtlist.Items(i).FindControl("chkSelezione"), System.Web.UI.WebControls.CheckBox).Checked Then

                        ' Conteggio pazienti selezionati
                        count += 1

                        codicePaziente = DirectCast(dtlist.Items(i).FindControl("lblCodice"), Label).Text
                        dataConvocazione = DirectCast(dtlist.Items(i).FindControl("lblData"), Label).Text

                        Dim dtr As DataRow() = dt.Select("PAZ_CODICE='" & codicePaziente & "' AND CNV_DATA=CONVERT('" & dataConvocazione & "','System.DateTime')")

                        If dtr.Length > 0 Then
                            ' Data appuntamento da spostare
                            Dim dataAppuntamentoOld As DateTime = Convert.ToDateTime(dtr(0)("CNV_DATA_APPUNTAMENTO"))
                            dapp_old = dataAppuntamentoOld.Date
                            amb_old = dtr(0)("CNV_AMB_CODICE")

                            ' Data nuovo appuntamento
                            dapp_new = dpkDataSpostamento.Data
                            amb_new = SpostaSceltaAmb.Codice

                            ' Scrive solo nel log
                            WarningMessage(String.Empty, "Paziente spostato: " + dtr(0)("PAZ_COGNOME").ToString() + " " + dtr(0)("PAZ_NOME").ToString() +
                                           " [" + Convert.ToDateTime(dtr(0)("PAZ_DATA_NASCITA")).ToString("dd/MM/yyyy") + "] " +
                                           " da appuntamento " + dataAppuntamentoOld.ToString("dd/MM/yyyy HH:mm") +
                                           " in ambulatorio " + amb_old.ToString() +
                                           " a appuntamento " + dapp_new.ToString("dd/MM/yyyy") +
                                           " in ambulatorio " + amb_new.ToString(),
                                           TipoOperazione.Spostamento_Appuntamento)

                            ' --- Aggiornamento totale appuntamenti del giorno --- '
                            tipo = TipoPrenotazione.APPUNTAMENTO

                            If dtr(0)("SOLLECITO") > 0 Then
                                tipo = TipoPrenotazione.RITARDO
                            ElseIf dtr(0)("VACCINAZIONI").ToString() = "" Or dtr(0)("VACCINAZIONI").ToString().Trim() = "()" Then
                                tipo = TipoPrenotazione.BILANCIO
                            End If

                            row_old = dtaNAppuntamenti.Rows.Find(New Object() {dapp_old})
                            If Not row_old Is Nothing Then
                                Select Case tipo
                                    Case TipoPrenotazione.APPUNTAMENTO
                                        If row_old("N_NORMALI") > 0 Then row_old("N_NORMALI") -= 1
                                    Case TipoPrenotazione.RITARDO
                                        If row_old("N_RITARDI") > 0 Then row_old("N_RITARDI") -= 1
                                    Case TipoPrenotazione.BILANCIO
                                        If row_old("N_BILANCI") > 0 Then row_old("N_BILANCI") -= 1
                                End Select
                            End If
                            If row_old("N_NORMALI") = 0 AndAlso row_old("N_RITARDI") = 0 AndAlso row_old("N_BILANCI") = 0 Then
                                dtaNAppuntamenti.Rows.Remove(row_old)
                            End If
                            ' ---------------------------------------------------- '

                            ' Orario di prenotazione
                            Dim orario() As String = String.Format("{0:HH.mm}", dtr(0)("CNV_DATA_APPUNTAMENTO")).Split(".")

                            ' Aggiornamento data
                            dtr(0)("CNV_DATA_APPUNTAMENTO") = New Date(dpkDataSpostamento.Data.Year, dpkDataSpostamento.Data.Month, dpkDataSpostamento.Data.Day, CInt(orario(0)), CInt(orario(1)), 0)
                            dtr(0)("CNV_AMB_CODICE") = amb_new

                            If amb_new = amb_old Then
                                ' --- Aggiornamento totale appuntamenti del giorno --- '
                                row_new = dtaNAppuntamenti.Rows.Find(New Object() {dapp_new})
                                If row_new Is Nothing Then
                                    row_new = dtaNAppuntamenti.NewRow
                                    row_new("CNV_DATA_APPUNTAMENTO") = dapp_new
                                    row_new("N_NORMALI") = IIf(tipo = TipoPrenotazione.APPUNTAMENTO, 1, 0)
                                    row_new("N_RITARDI") = IIf(tipo = TipoPrenotazione.RITARDO, 1, 0)
                                    row_new("N_BILANCI") = IIf(tipo = TipoPrenotazione.BILANCIO, 1, 0)
                                    dtaNAppuntamenti.Rows.Add(row_new)
                                Else
                                    Select Case tipo
                                        Case TipoPrenotazione.APPUNTAMENTO
                                            row_new("N_NORMALI") += 1
                                        Case TipoPrenotazione.RITARDO
                                            row_new("N_RITARDI") += 1
                                        Case TipoPrenotazione.BILANCIO
                                            row_new("N_BILANCI") += 1
                                    End Select
                                End If
                            End If
                        End If
                    End If
                Next
            Next

            ' Fa il refresh del diario giornaliero
            If Me.CaricaCalendario() Then

                ' Se sono stati effettuati spostamenti, il layout deve rimanere bloccato
                If count > 0 Then
                    Me.Modificato = True
                    Me.SpostamentoAppuntamenti = True
                    WarningMessage(String.Empty, "Numero convocazioni spostate: " & count.ToString() & ". Premere Salva per rendere definitivi gli spostamenti effettuati.", TipoOperazione.Info)
                Else
                    WarningMessage(String.Empty, "Nessuno spostamento effettuato: nessuna convocazione selezionata.", TipoOperazione.Info)
                End If

                dpkDataSpostamento.Text = ""

            End If

        Catch ex As Exception
            Common.Utility.EventLogHelper.EventLogWrite(ex, OnVacContext.AppId)
            WarningMessage("Errore", ex)
        End Try

        ' Chiusura modale
        Me.FinestraModaleVisibile = False
        Me.fmSpostaPrenotazioni.VisibileMD = False

    End Sub

    Private Sub btnAnnullaSposta_Click(sender As Object, e As System.EventArgs) Handles btnAnnullaSposta.Click

        Me.FinestraModaleVisibile = False
        Me.fmSpostaPrenotazioni.VisibileMD = False
        Me.dpkDataSpostamento.Text = String.Empty

    End Sub

    ' Restituisce true se trova una convocazione nella data specificata, false altrimenti
    Private Function DataSpostamentoOccupata(ambCodice As Integer) As Boolean

        Dim ret As String = String.Empty

        Using dam As IDAM = OnVacUtility.OpenDam()

            With dam.QB
                .NewQuery()
                .AddSelectFields("cnv_paz_codice")
                .AddTables("t_cnv_convocazioni")
                .AddWhereCondition("cnv_amb_codice", Comparatori.Uguale, ambCodice, DataTypes.Numero)
                .AddWhereCondition("cnv_data_appuntamento", Comparatori.MaggioreUguale, Me.dpkDataSpostamento.Text + " 0.00", DataTypes.DataOra)
                .AddWhereCondition("cnv_data_appuntamento", Comparatori.MinoreUguale, Me.dpkDataSpostamento.Text + " 23.59", DataTypes.DataOra)
            End With

            ret = dam.ExecScalar() & ""

        End Using

        Return (ret <> "")

    End Function

#End Region

#Region " Orari Personalizzati "

    Private Sub btnOrariPers_Click(sender As Object, e As System.EventArgs) Handles btnOrariPers.Click

        Me.ImpostaOrariPersonalizzati()
        Me.CodNuovoOrarioPers = -1

        Me.FinestraModaleVisibile = True
        Me.fmOrariPersonalizzati.VisibileMD = True

    End Sub

    Private Sub ImpostaOrariPersonalizzati()

        Me.dtOrariPersonalizzati = CaricaOrariPersonalizzatiAmbulatorioCorrente()

        If Me.dtOrariPersonalizzati Is Nothing Then Return ' errore di caricamento dati

#If DEBUG Then
        Dim renderator As New Renderator()
        Dim str As String = renderator.ExportToHtml(Me.dtOrariPersonalizzati, renderator.Type.table)
#End If

        ' Inserisco il numero di fasce orarie nel textbox nascosto (per controllare il check senza dover andare al server)
        Me.hid_txt_num_orari_pers.Text = Me.dtOrariPersonalizzati.Rows.Count.ToString()

        Dim dv As New DataView(Me.dtOrariPersonalizzati)
        dv.Sort = "ordine,orp_ora_inizio"

        Me.dgrOrariPersonalizzati.DataSource = dv
        Me.dgrOrariPersonalizzati.DataBind()

    End Sub

    Private Sub dgrOrariPersonalizzati_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgrOrariPersonalizzati.ItemDataBound

        If e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.EditItem Or e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.SelectedItem Then

            If Not IsNothing(e.Item.DataItem) Then

                Dim gg As String = DirectCast(e.Item.DataItem, DataRowView)("ORP_GIORNO").ToString().ToUpper()

                If gg <> "" Then
                    Dim ddl As DropDownList = DirectCast(e.Item.FindControl("ddlGiorno"), DropDownList)
                    ddl.SelectedValue = gg
                End If

            End If
        End If

    End Sub

    Private Function CaricaOrariPersonalizzatiAmbulatorioCorrente() As DataTable

        Dim dt As New DataTable()

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizGestioneAppuntamenti As New Biz.BizGestioneAppuntamenti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Try
                    dt = bizGestioneAppuntamenti.CaricaOrariPersonalizzati(Me.AmbCodice.ToString())
                Catch ex As Exception
                    Common.Utility.EventLogHelper.EventLogWrite(ex, OnVacContext.AppId)
                    WarningMessage("Errore in <b>CaricaOrariPersonalizzati</b>:", ex)
                End Try

            End Using
        End Using

        Return dt

    End Function

    Private Function SalvaOrariPersonalizzati() As Boolean

        Dim result As Boolean = True

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizGestioneAppuntamenti As New Biz.BizGestioneAppuntamenti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Try
                    bizGestioneAppuntamenti.SalvaOrariPersonalizzati(Me.AmbCodice, Me.dtOrariPersonalizzati)
                Catch ex As Exception
                    Common.Utility.EventLogHelper.EventLogWrite(ex, OnVacContext.AppId)
                    result = False
                    WarningMessage("Errore in <b>SalvaOrariPersonalizzati</b>:", ex)
                End Try

            End Using
        End Using

        Return result

    End Function

    Private Sub tlbOrariPersonalizzati_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles tlbOrariPersonalizzati.ButtonClicked

        Select Case be.Button.Key
            Case "btnConfermaOrariPers"
                ConfermaOrariPersonalizzati()

            Case "btnAnnullaOrariPers"
                AnnullaOrariPersonalizzati()

            Case "btnAggiungiOrariPers"
                AggiungiOrariPersonalizzati()

        End Select

    End Sub

    Private Sub ConfermaOrariPersonalizzati()

        LeggiOrariPersInseriti()

        ' Controllo dati inseriti
        If Not ControlloOrariPersonalizzati(True) Then
            Dim strJs As String = "<script language='javascript'>"
            strJs += "alert('Non tutti gli orari inseriti sono corretti: potrebbero essere vuoti o sovrapposti.\n Correggere gli orari o premere Annulla per continuare.');"
            strJs += "</script>"
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "errore_controllo_orari", strJs)
            Return
        End If

        ' Salvataggio
        If SalvaOrariPersonalizzati() Then

            ' Chiudo la modale e riabilito il layout
            Me.FinestraModaleVisibile = False
            Me.fmOrariPersonalizzati.VisibileMD = False

            ' Spunta il check di utilizzo degli orari personalizzati (se ce ne sono)
            Me.chkOrariPers.Checked = (Me.dtOrariPersonalizzati.Rows.Count > 0)

            ' Inserisco il numero di fasce orarie nel textbox nascosto (per controllare il check senza dover andare al server)
            Me.hid_txt_num_orari_pers.Text = Me.dtOrariPersonalizzati.Rows.Count.ToString()

        Else

            ' Ricarico gli orari personalizzati
            Me.dtOrariPersonalizzati = CaricaOrariPersonalizzatiAmbulatorioCorrente()

        End If

    End Sub

    Private Sub AnnullaOrariPersonalizzati()

        Me.dtOrariPersonalizzati = CaricaOrariPersonalizzatiAmbulatorioCorrente()

        Me.FinestraModaleVisibile = False
        Me.fmOrariPersonalizzati.VisibileMD = False

    End Sub

    Private Sub AggiungiOrariPersonalizzati()

        LeggiOrariPersInseriti()

        If Not ControlloOrariPersonalizzati(False) Then
            Dim strJs As String = "<script language='javascript'>"
            strJs += "alert('Attenzione: orari sovrapposti. Correggere gli orari o premere Annulla per continuare.');"
            strJs += "</script>"
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "errore_controllo_orari", strJs)
            Return
        End If

        ' Aggiunta al datatable
        dtOrariPersonalizzati.Rows.Add(New Object() {CodNuovoOrarioPers, "LUN", Nothing, Nothing, Nothing, Nothing, 0}) ' 0 è l'ordine così la riga va all'inizio

        ' Aggiornamento valore del codice fittizio
        CodNuovoOrarioPers -= 1

        ' Databind per aggiunta al datagrid
        Dim dv As New DataView(dtOrariPersonalizzati)
        dv.Sort = "ordine, orp_ora_inizio"
        dgrOrariPersonalizzati.DataSource = dv
        dgrOrariPersonalizzati.DataBind()

    End Sub

    ' Controllo valori datatable
    Private Function ControlloOrariPersonalizzati(check_orari_vuoti As Boolean) As Boolean

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizGestioneAppuntamenti As New Biz.BizGestioneAppuntamenti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Return bizGestioneAppuntamenti.ControlloOrariPersonalizzati(dtOrariPersonalizzati, check_orari_vuoti)

            End Using
        End Using

    End Function

    ' Riempie il datatable degli orari personalizzati in base a ciò che è stato inserito nel datagrid
    Private Sub LeggiOrariPersInseriti()

        If Not IsNothing(dtOrariPersonalizzati) Then dtOrariPersonalizzati = Nothing

        dtOrariPersonalizzati = New DataTable()
        dtOrariPersonalizzati.Columns.Add("ORP_CODICE")
        dtOrariPersonalizzati.Columns.Add("ORP_GIORNO")
        dtOrariPersonalizzati.Columns.Add("ORP_ORA_INIZIO")
        dtOrariPersonalizzati.Columns.Add("ORP_ORA_FINE")
        dtOrariPersonalizzati.Columns.Add("ORP_NUM_PAZIENTI")
        dtOrariPersonalizzati.Columns.Add("ORP_DURATA")
        dtOrariPersonalizzati.Columns.Add("ordine")

        Dim newRow As DataRow

        For i As Integer = 0 To dgrOrariPersonalizzati.Items.Count - 1

            newRow = dtOrariPersonalizzati.NewRow()
            newRow("ORP_CODICE") = dgrOrariPersonalizzati.Items(i).Cells(dgrOrariPersonalizzati.getColumnNumberByKey("Codice")).Text
            newRow("ORP_GIORNO") = DirectCast(dgrOrariPersonalizzati.Items(i).FindControl("ddlGiorno"), DropDownList).SelectedValue
            newRow("ORP_ORA_INIZIO") = DirectCast(dgrOrariPersonalizzati.Items(i).FindControl("txtOraInizio"), TextBox).Text
            newRow("ORP_ORA_FINE") = DirectCast(dgrOrariPersonalizzati.Items(i).FindControl("txtOraFine"), TextBox).Text
            newRow("ORP_NUM_PAZIENTI") = DirectCast(dgrOrariPersonalizzati.Items(i).FindControl("txtNumPaz"), TextBox).Text
            newRow("ORP_DURATA") = DirectCast(dgrOrariPersonalizzati.Items(i).FindControl("txtDurataPers"), TextBox).Text
            newRow("ordine") = CodificaGiorno(newRow("ORP_GIORNO").ToString(), False)

            dtOrariPersonalizzati.Rows.Add(newRow)

        Next

        dtOrariPersonalizzati.AcceptChanges()

    End Sub

    ' Per compatibilità con la funzione di calcolo delle prenotazioni, devo restituire 0 per la domenica
    ' Invece, per ordinare il datagrid degli orari personalizzati, devo restituire 7 per la domenica.
    ' Il parametro domenica_zero indica in quale dei due casi ci si trova.
    Private Function CodificaGiorno(giorno As String, domenica_zero As Boolean) As Integer

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
                If domenica_zero Then
                    Return 0
                Else
                    Return 7
                End If
        End Select

    End Function

    Private Sub dgrOrariPersonalizzati_DeleteCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgrOrariPersonalizzati.DeleteCommand

        Dim cod As Integer = e.Item.Cells(dgrOrariPersonalizzati.getColumnNumberByKey("Codice")).Text

        LeggiOrariPersInseriti()

        Dim keyCol(0) As DataColumn
        keyCol(0) = dtOrariPersonalizzati.Columns("ORP_CODICE")

        Dim key(0) As Object
        key(0) = cod

        dtOrariPersonalizzati.PrimaryKey = keyCol
        dtOrariPersonalizzati.Rows.Find(key).Delete()
        dtOrariPersonalizzati.PrimaryKey = Nothing
        dtOrariPersonalizzati.AcceptChanges()

        Dim dv As New DataView(dtOrariPersonalizzati)
        dv.Sort = "ordine, orp_ora_inizio"
        dgrOrariPersonalizzati.DataSource = dv
        dgrOrariPersonalizzati.DataBind()

    End Sub

#End Region

#Region " Stampe "

#Region " Stampa Elenco Convocati "

    Private Sub StampaElencoConvocati()

        If Me.dtaRicerca.Rows.Count = 0 Then
            Dim strJs As String = "<script language='javascript'>"
            strJs += "alert('L\'elenco dei convocati e\' vuoto: nessun risultato da stampare.');</script>"
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "msgNoStampa", strJs)
            Return
        End If

        Me.ReportPopUp_DataSource = Me.dts_Ricerca

        Dim repPar As New ArrayList()
        repPar.Add("Ciclo|" & Me.UscFiltroCicliSedute.getStringaFormattata.Replace("Sedute:", ""))
        repPar.Add("Seduta|" & Me.UscFiltroAssociazioniDosi.getStringaFormattata.Replace("Dosi:", ""))
        repPar.Add("Comune" & "|" & Me.fmComune.Descrizione)
        repPar.Add("Medico" & "|" & Me.txtMedico.Descrizione)
        repPar.Add("DataNascitaIniz" & "|" & Me.txtDaDataNascita.Text)
        repPar.Add("DataNascitaFin" & "|" & Me.txtADataNascita.Text)
        repPar.Add("FinoA" & "|" & Me.txtFinoAData.Text)
        repPar.Add("SoloRitardatari" & "|" & Me.chkSoloRitardatari.Checked.ToString())
        repPar.Add("OverbookingRit" & "|" & Me.chkSovrapponiRit.Checked.ToString())
        repPar.Add("StatoAnagrafico" & "|" & Me.DescrizioneStatiAnagrafici())
        repPar.Add("CodiceUsl" & "|" & OnVacContext.CodiceUslCorrente)


        ' Filtro Cittadinanza
        Dim strFiltroCitt As String = String.Empty
        If Me.chkImmigratiNonExtracomPrimaVolta.Checked Then
            strFiltroCitt = "Non extracom. prima volta"
        End If
        If Me.chkImmigratiExtracom.Checked Then
            If strFiltroCitt <> "" Then
                strFiltroCitt += ", "
            End If
            strFiltroCitt += "Extracom."
        End If
        If strFiltroCitt = "" Then
            strFiltroCitt = "Non specificata"
        End If
        repPar.Add("Cittadinanza" + "|" + strFiltroCitt)

        repPar.Add("VaccObbl" & "|" & Me.chkTipoVaccObbligatoria.Checked.ToString())
        repPar.Add("VaccFac" & "|" & Me.chkTipoVaccFacoltativa.Checked.ToString())
        repPar.Add("VaccRacc" & "|" & Me.chkTipoVaccRaccomandata.Checked.ToString())
        repPar.Add("CategRischio" & "|" & Me.omlCategorieRischio.Descrizione)
        repPar.Add("Malattia" & "|" & Me.omlMalattia.Descrizione)
        repPar.Add("Cronici" & "|" & Me.chkCronici.Checked.ToString())
        repPar.Add("Consultorio" & "|" & Me.ucSelezioneConsultori.GetConsultoriSelezionati(False))
        repPar.Add("Sesso" & "|" & Me.ddlSesso.SelectedValue.ToString())
        repPar.Add("CnvDataSingola" & "|" & Me.chkDataSingola.Checked.ToString())
        repPar.Add("SoloEscluse" & "|" & Me.chkEscluse.Checked.ToString())

        Dim rptName As String = Constants.ReportName.ElencoConvocati.Replace(".rpt", String.Empty)

        Me.ReportPar(rptName) = repPar

        Me.OpenReportPopup(rptName, Nothing)

    End Sub

#End Region

#Region " Stampa Log "

    Private Sub StampaLog(soloFalliti As Boolean)

        Me.ReportPopUp_DataSource = New System.Data.DataSet()

        If soloFalliti Then
            Me.ReportPopUp_DataSource = Me.dts_log.Clone()
            For Each row As dtsLog.T_LOGRow In Me.dts_log.T_LOG
                If row.TipoMessaggio = "errore" Or row.TipoMessaggio = "alert" Then
                    Me.ReportPopUp_DataSource.Tables(0).ImportRow(row)
                End If
            Next
            Me.ReportPopUp_DataSource.AcceptChanges()
        Else
            Me.ReportPopUp_DataSource = Me.dts_log
        End If

        'Creazione dell'intestazione della stampa
        Dim filtro As New System.Text.StringBuilder(1000)
        Dim params As New ArrayList()

        ' FILTRO 1
        filtro.AppendFormat("Data nascita da: {0}{1}{2}", Me.txtDaDataNascita.Text, vbTab, "- ")
        filtro.AppendFormat("Cicli-Sedute: {0}{1}", Me.UscFiltroCicliSedute.getStringaFormattata.Replace("Sedute:", ""), vbCrLf)
        filtro.AppendFormat("Data nascita a: {0}{1}{2}", Me.txtADataNascita.Text, vbTab, "- ")
        filtro.AppendFormat("Associazioni-Dosi: {0}{1}", Me.UscFiltroAssociazioniDosi.getStringaFormattata.Replace("Dosi:", ""), vbCrLf)
        filtro.AppendFormat("Comune: {0}{1}{2}", Me.fmComune.Descrizione, vbTab, "- ")
        filtro.AppendFormat("Medico: {0}{1}", Me.txtMedico.Descrizione, Environment.NewLine)
        filtro.AppendFormat("Sesso: {0}{1}{2}", IIf(String.IsNullOrEmpty(Me.ddlSesso.SelectedValue), "Entrambi", Me.ddlSesso.SelectedItem.Text), vbTab, "- ")

        ' Filtro Cittadinanza
        Dim strFiltroCitt As String = String.Empty
        If Me.chkImmigratiNonExtracomPrimaVolta.Checked Then
            strFiltroCitt = "Non extracom. prima volta"
        End If
        If Me.chkImmigratiExtracom.Checked Then
            If strFiltroCitt <> "" Then
                strFiltroCitt += ", "
            End If
            strFiltroCitt += "Extracom."
        End If
        If strFiltroCitt = "" Then
            strFiltroCitt = "Non specificata"
        End If
        filtro.AppendFormat("Cittadinanza: {0}{1}", strFiltroCitt, Environment.NewLine)

        filtro.AppendFormat("Categoria a rischio: {0}{1}{2}", Me.omlCategorieRischio.Descrizione, vbTab, "- ")
        filtro.AppendFormat("Malattia: {0}{1}{2}", Me.omlMalattia.Descrizione, vbTab, "- ")
        filtro.AppendFormat("Cronici : {0}{1}", IIf(Me.chkCronici.Checked, "Sì", "No"), Environment.NewLine)

        ' Filtro Tipo Vaccinazione
        Dim strFiltroTipoVaccinazione As String = String.Empty
        If Me.chkTipoVaccObbligatoria.Checked Then
            strFiltroTipoVaccinazione = "Obbligatoria"
        End If
        If Me.chkTipoVaccFacoltativa.Checked Then
            If strFiltroTipoVaccinazione <> "" Then
                strFiltroTipoVaccinazione += ", "
            End If
            strFiltroTipoVaccinazione += "Facoltativa"
        End If
        If Me.chkTipoVaccRaccomandata.Checked Then
            If strFiltroTipoVaccinazione <> "" Then
                strFiltroTipoVaccinazione += ", "
            End If
            strFiltroTipoVaccinazione += "Raccomandata"
        End If
        If strFiltroTipoVaccinazione = "" Then
            strFiltroTipoVaccinazione = "Non specificato"
        End If
        filtro.AppendFormat("Tipo vaccinazione: {0}{1}", strFiltroTipoVaccinazione, Environment.NewLine)
        params.Add("Filtro1|" & filtro.ToString())

        ' FILTRO 2
        filtro.Clear()
        filtro.AppendFormat("Cnv fino a data: {0}{1}{2}", Me.txtFinoAData.Text, vbTab, "- ")
        filtro.AppendFormat("Consultorio: {0}{1}", Me.ucSelezioneConsultori.GetConsultoriSelezionati(False), Environment.NewLine)
        filtro.AppendFormat("Solo ritardatari: {0}{1}{2}", IIf(Me.chkSoloRitardatari.Checked, "Sì", "No"), vbTab, "- ")
        filtro.AppendFormat("Data Singola: {0}{1}{2}", IIf(Me.chkDataSingola.Checked, "Sì", "No"), vbTab, "- ")
        filtro.AppendFormat("Solo Escluse: {0}{1}", IIf(Me.chkEscluse.Checked, "Sì", "No"), Environment.NewLine)
        params.Add("Filtro2|" & filtro.ToString())

        ' FILTRO 3
        filtro.Clear()
        filtro.AppendFormat("Stato anagrafico: {0}", Me.DescrizioneStatiAnagrafici())
        params.Add("Filtro3|" & filtro.ToString())

        ' FILTRO 4
        filtro.Clear()
        filtro.AppendFormat("Prenotaz. dal {0}", Me.odpDataInizPrenotazioni.Text)
        If Me.odpDataFinePrenotazioni.Data <> Date.MinValue Then
            filtro.AppendFormat("al {0}", Me.odpDataFinePrenotazioni.Text)
        End If
        filtro.Append(" - ")
        filtro.AppendFormat("Utilizza orari personalizzati: {0}{1}{2}", IIf(Me.chkOrariPers.Checked, "Sì", "No"), vbTab, "- ")
        filtro.AppendFormat("Overbooking ritardi: {0}{1}{2}", IIf(Me.chkSovrapponiRit.Checked, "Sì", "No"), vbTab, "- ")
        filtro.AppendFormat("Pomeriggio Obbl.: {0}{1}{2}", IIf(Me.chkFiltroPomeriggioObbligatorio.Checked, "Sì", "No"), "", vbCrLf)
        params.Add("Filtro4|" & filtro.ToString())

        Dim nomeRpt As String = Constants.ReportName.LogGestioneAppuntamenti.Replace(".rpt", String.Empty)

        Session(nomeRpt + "_param") = params

        Me.OpenReportPopup(nomeRpt, Nothing)

    End Sub

#End Region

#Region " Stampa Pazienti da Sollecitare "

    Private Sub StampaPazientiDaSollecitare()

        Dim dtsSol As New dtsSolleciti()
        Dim rptName As String = "SollecitiPazienti"

        Dim rptPar As New ArrayList()
        rptPar.Add("DataConvocazione|" + Me.txtFinoAData.Text)
        rptPar.Add("DataNascitaDa|" + Me.txtDaDataNascita.Text)
        rptPar.Add("DataNascitaA|" + Me.txtADataNascita.Text)

        Me.ReportPar(rptName) = rptPar

        Dim rowSol As DataRow

        Dim pazDaSollecitare As Common.Solleciti.PazienteCicloObbligatorioNoTP = Nothing

        For count As Integer = 0 To Me.Solleciti.PazientiCicliObbligatoriNoTP.Count - 1

            pazDaSollecitare = Me.Solleciti.PazientiCicliObbligatoriNoTP(count)

            rowSol = dtsSol.Tables("SOLLECITI").NewRow()
            rowSol("sol_tipo") = "NOTP"
            rowSol("sol_ordine") = 1
            rowSol("sol_codice") = pazDaSollecitare.PazienteCodice
            rowSol("sol_cognome") = pazDaSollecitare.Cognome
            rowSol("sol_nome") = pazDaSollecitare.Nome
            rowSol("sol_numero_sollecito") = pazDaSollecitare.NumSollecitoSeduta + 1
            rowSol("sol_data_nascita") = pazDaSollecitare.DataNascita
            rowSol("sol_data_convocazione") = pazDaSollecitare.DataConvocazione
            rowSol("sol_data_appuntamento") = pazDaSollecitare.DataAppuntamento
            rowSol("sol_stato_anagrafico") = pazDaSollecitare.DescStatoAnagrafico
            rowSol("sol_stato") = [Enum].GetName(GetType(Common.Solleciti.TipoStatoPazientiDaSollecitare), pazDaSollecitare.TipoStato)
            rowSol("sol_data_invio") = pazDaSollecitare.DataInvio

            dtsSol.Tables("SOLLECITI").Rows.Add(rowSol)

        Next

        For count As Integer = 0 To Me.Solleciti.PazientiCicliObbligatoriInTP.Count - 1

            Dim pazInTermPer As Common.Solleciti.PazienteCicloObbligatorioInTP = Me.Solleciti.PazientiCicliObbligatoriInTP(count)

            rowSol = dtsSol.Tables("SOLLECITI").NewRow()
            rowSol("sol_tipo") = "INTP"
            rowSol("sol_ordine") = 2
            rowSol("sol_codice") = pazInTermPer.PazienteCodice
            rowSol("sol_cognome") = pazInTermPer.Cognome
            rowSol("sol_nome") = pazInTermPer.Nome
            rowSol("sol_numero_sollecito") = pazInTermPer.NumSollecitoSeduta + 1
            rowSol("sol_data_nascita") = pazInTermPer.DataNascita
            rowSol("sol_data_convocazione") = pazInTermPer.DataConvocazione
            rowSol("sol_data_appuntamento") = pazInTermPer.DataAppuntamento
            rowSol("sol_stato_anagrafico") = pazInTermPer.DescStatoAnagrafico
            rowSol("sol_stato") = "ComunicazioneAlSindaco"
            rowSol("sol_data_invio") = pazInTermPer.DataInvio

            dtsSol.Tables("SOLLECITI").Rows.Add(rowSol)

        Next

        For count As Integer = 0 To Me.Solleciti.PazientiCicliNonObbligatori.Count - 1

            Dim pazVacNonObbl As Common.Solleciti.PazienteCicloNonObbligatorio = Me.Solleciti.PazientiCicliNonObbligatori(count)

            rowSol = dtsSol.Tables("SOLLECITI").NewRow()
            rowSol("sol_tipo") = "NOBL"
            rowSol("sol_ordine") = 3
            rowSol("sol_codice") = pazVacNonObbl.PazienteCodice
            rowSol("sol_cognome") = pazVacNonObbl.Cognome
            rowSol("sol_nome") = pazVacNonObbl.Nome
            rowSol("sol_numero_sollecito") = pazVacNonObbl.NumSollecitoSeduta + 1
            rowSol("sol_data_nascita") = pazVacNonObbl.DataNascita
            rowSol("sol_data_convocazione") = pazVacNonObbl.DataConvocazione
            rowSol("sol_data_appuntamento") = pazVacNonObbl.DataAppuntamento
            rowSol("sol_stato_anagrafico") = pazVacNonObbl.DescStatoAnagrafico
            rowSol("sol_stato") = [Enum].GetName(GetType(Common.Solleciti.TipoStatoPazientiVaccinazioniNonObbligatorie), pazVacNonObbl.TipoStato)
            rowSol("sol_data_invio") = pazVacNonObbl.DataInvio

            dtsSol.Tables("SOLLECITI").Rows.Add(rowSol)

        Next

        For count As Integer = 0 To Me.Solleciti.PazientiCicliRaccomandatiTerminati.Count - 1

            Dim pazVacRacc As Common.Solleciti.PazienteCicloRaccomandatoTerminato = Me.Solleciti.PazientiCicliRaccomandatiTerminati(count)

            rowSol = dtsSol.Tables("SOLLECITI").NewRow()
            rowSol("sol_tipo") = "RACC"
            rowSol("sol_ordine") = 4
            rowSol("sol_codice") = pazVacRacc.PazienteCodice
            rowSol("sol_cognome") = pazVacRacc.Cognome
            rowSol("sol_nome") = pazVacRacc.Nome
            rowSol("sol_numero_sollecito") = pazVacRacc.NumSollecitoSeduta + 1
            rowSol("sol_data_nascita") = pazVacRacc.DataNascita
            rowSol("sol_data_convocazione") = pazVacRacc.DataConvocazione
            rowSol("sol_data_appuntamento") = pazVacRacc.DataAppuntamento
            rowSol("sol_stato_anagrafico") = pazVacRacc.DescStatoAnagrafico
            rowSol("sol_stato") = String.Empty
            rowSol("sol_data_invio") = DBNull.Value

            dtsSol.Tables("SOLLECITI").Rows.Add(rowSol)

        Next

        For count As Integer = 0 To Me.Solleciti.PazientiNoCicli.Count - 1

            Dim pazienteNoCiclo As Common.Solleciti.PazienteNoCiclo = Me.Solleciti.PazientiNoCicli(count)

            rowSol = dtsSol.Tables("SOLLECITI").NewRow()
            rowSol("sol_tipo") = "NOCIC"
            rowSol("sol_ordine") = 4
            rowSol("sol_codice") = pazienteNoCiclo.PazienteCodice
            rowSol("sol_cognome") = pazienteNoCiclo.Cognome
            rowSol("sol_nome") = pazienteNoCiclo.Nome
            rowSol("sol_numero_sollecito") = DBNull.Value
            rowSol("sol_data_nascita") = pazienteNoCiclo.DataNascita
            rowSol("sol_data_convocazione") = pazienteNoCiclo.DataConvocazione
            rowSol("sol_data_appuntamento") = pazienteNoCiclo.DataAppuntamento
            rowSol("sol_stato_anagrafico") = pazienteNoCiclo.DescStatoAnagrafico
            rowSol("sol_stato") = IIf(Me.Settings.ESCLUDISENOCICLO, String.Empty, "CancellazioneAppuntamento")
            rowSol("sol_data_invio") = DBNull.Value

            dtsSol.Tables("SOLLECITI").Rows.Add(rowSol)

        Next

        Me.OpenReportPopup(rptName, dtsSol)

    End Sub

#End Region

#Region " Stampa Convocati Selezionati "

    Private Sub btnStampaSelezionati_Click(sender As Object, e As System.EventArgs) Handles btnStampaSelezionati.Click

        ' Stampa uguale a quella di ElencoAvvisi.rpt, ma senza data e ora di convocazione, perchè la cnv non è ancora registrata.
        ' Non ci sono i dati nella v_avvisi, bisogna creare un dataset ad hoc e passarlo al nuovo report (AvvisoConvocati.rpt).
        Me.StampaConvocatiSelezionati()

    End Sub

    Private Sub StampaConvocatiSelezionati()

        ' Se non ci sono dati nel datatable, esce subito
        If Me.dtaRicerca Is Nothing OrElse Me.dtaRicerca.Rows.Count = 0 Then
            WarningMessage(String.Empty, "Attenzione: nessun dato da stampare. Il report Elenco Convocati Selezionati non verrà creato.", TipoOperazione.Info)
            Exit Sub
        End If

        ' --- Aggiornamento pazienti selezionati nella pagina corrente (altrimenti non verrebbero considerati) --- '
        Me.ControllaSelezioneConvocati()

        ' --- Filtro pazienti selezionati --- '
        ' La data di convocazione viene concatenata al codice del paziente, così se nell'elenco 
        ' un paziente compare più volte (perchè ha più convocazioni), il datatable non dà errore di chiave duplicata.
        Dim hVacPazSelezionati As New Hashtable()
        Dim _key As String

        For Each row As DataRow In dtaRicerca.Select("SEL='S'")
            _key = row("paz_codice").ToString() + "*" + row("cnv_data").ToString()
            If Not (hVacPazSelezionati.ContainsKey(_key)) Then
                hVacPazSelezionati.Add(_key, row("vaccinazioni"))   '  hashtable con le vaccinazioni associate ad ogni paziente in ogni convocazione
            End If
        Next

        ' Se non ci sono pazienti selezionati, non stampo il report
        If hVacPazSelezionati.Count = 0 Then
            WarningMessage(String.Empty, "Attenzione: nessun dato da stampare. Il report Elenco Convocati Selezionati non verrà creato.", TipoOperazione.Info)
            Exit Sub
        End If

        ' --- Creazione dataset per report --- '
        Dim dstPazVac As dtsAvvisoConvocati

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizPazientiVaccinazioni As New Biz.BizPazientiVaccinazioni(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Try
                    dstPazVac = bizPazientiVaccinazioni.CreateDtsAvvisoConvocatiSelezionati(hVacPazSelezionati, OnVacUtility.Variabili.CNS.Codice)

                    If dstPazVac Is Nothing Then
                        WarningMessage(String.Empty, bizPazientiVaccinazioni.ERRORMESSAGE.Replace(Environment.NewLine, "<br />"), TipoOperazione.Errore)
                        Exit Sub
                    End If

                    If dstPazVac.Convocati.Rows.Count = 0 Then
                        WarningMessage(String.Empty, "Attenzione: nessun dato da stampare. Il report Elenco Convocati Selezionati non verrà creato", TipoOperazione.Info)
                        Exit Sub
                    End If

                Catch ex As Exception
                    Common.Utility.EventLogHelper.EventLogWrite(ex, OnVacContext.AppId)
                    WarningMessage("Errore durante il caricamento dei dati da passare al report Elenco Convocati Selezionati.", ex)
                    Exit Sub

                End Try

            End Using
        End Using

        ' --- Apertura pagina di creazione e anteprima del report --- '
        Me.OpenReportPopup(Constants.ReportName.ElencoConvocatiSelezionati.Replace(".rpt", String.Empty), dstPazVac)

    End Sub

#End Region

    Private Sub OpenReportPopup(rptName As String, dst As System.Data.DataSet)

        If Not dst Is Nothing Then
            ReportPopUp_DataSource = dst
        End If

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                If bizReport.GetReportFolder(rptName + ".rpt") = String.Empty Then
                    OnVacUtility.StampaNonPresente(Page, rptName + ".rpt")
                Else
                    RegisterStartupScriptCustom("OpenReportPopup", String.Format("window.open('../../Stampe/StampaReportPopUp.aspx?report={0}','{0}','top=0,left=0,width=500,height=500,resizable=1')", rptName))
                End If

            End Using
        End Using

    End Sub

#End Region

    Private Function DescrizioneStatiAnagrafici() As String

        Return Me.ucStatiAnagrafici.GetSelectedDescriptions()

    End Function

    Private Sub ucSelezioneConsultori_OnSelectionError(errorMessage As String) Handles ucSelezioneConsultori.OnSelectionError

        Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(errorMessage, "errMsgSelezioneCns", False, False))

    End Sub

    Private Sub btnPulisciFiltri_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnPulisciFiltri.Click

        FiltriMaschera.SetFiltriMaschera = False
        Me.RicercaEffettuata = False
        Me.DefaultFiltriRicerca()

    End Sub

    Private Sub ToolBarGestioneSolleciti_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBarGestioneSolleciti.ButtonClicked

        Select Case e.Button.Key

            Case "Procedi"

                Me.Solleciti.ProcessaPazienti()

                If Me.CaricaCalendario() Then
                    Me.Cerca(True)
                End If

                Me.mltView.SetActiveView(Me.viewGestioneAppuntamenti)

            Case "Annulla"

                Me.mltView.SetActiveView(Me.viewGestioneAppuntamenti)

            Case "Stampa"

                Me.StampaPazientiDaSollecitare()
                Me.BindViewGestioneSolleciti()

        End Select

    End Sub

#Region " Consenso utente "

    Private Sub ucConsensoUtente_ConsensoTrattamentoDatiUtenteAccettato() Handles ucConsensoUtente.ConsensoTrattamentoDatiUtenteAccettato

        Dim codicePazienteSelezionato As String = ucConsensoUtente.CodicePaziente

        FiltriMaschera = GetFiltriRicerca()
        FiltriMaschera.SetFiltriMaschera = Settings.GES_APP_RICORDA_FILTRI AndAlso RicercaEffettuata

        ' Memorizzazione codice paziente per ricerca rapida (l'impostazione del paziente corrente avviene nel metodo RedirectToGestionePaziente)
        UltimoPazienteSelezionato = New Entities.UltimoPazienteSelezionato(String.Empty, codicePazienteSelezionato)

        ucConsensoUtente.ClearParameters()
        fmConsensoUtente.VisibileMD = False

        RedirectToGestionePaziente(codicePazienteSelezionato)

    End Sub

    Private Sub ucConsensoUtente_ConsensoTrattamentoDatiUtenteRifiutato() Handles ucConsensoUtente.ConsensoTrattamentoDatiUtenteRifiutato

        ucConsensoUtente.ClearParameters()
        fmConsensoUtente.VisibileMD = False

    End Sub

#End Region

End Class

Public Class FiltroMattinaPomeriggioObbligatorio
    Inherits Filtri.FiltroMattinaPomeriggio

    Public Overrides ReadOnly Property Obbligatorio() As Boolean
        Get
            Return True
        End Get
    End Property

End Class

