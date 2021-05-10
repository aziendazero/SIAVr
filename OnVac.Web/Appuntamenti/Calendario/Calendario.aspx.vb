Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Biz
Imports Onit.OnAssistnet.OnVac.DataSet


Partial Class OnVac_Calendario
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

#Region " Private "

    Private iconeConsensi As Onit.OnAssistnet.GestioneConsenso.Wcf.Proxy.ConsensiService.IconeConsensi

#End Region

#Region " Consts "

    Private Const OPEN_RILEVAZIONE_CONSENSO As String = "OPEN_CONS"
    Private Const MSG_BLOCCO_REDIRECT_CONSENSO As String = "Impossibile visualizzare i dati del paziente."

#End Region

#Region " Properties "

    'contiene i codici per la stampa [modifica 11/08/2005]
    Private Property CodiciPazientiDaStampare() As String
        Get
            Return Session("strPazStampa")
        End Get
        Set(Value As String)
            Session("strPazStampa") = Value
        End Set
    End Property

    ' Memorizza la data impostata nel filtro
    Private Property FiltroData() As DateTime?
        Get
            Return Session("FiltroDataCalendario")
        End Get
        Set(value As DateTime?)
            Session("FiltroDataCalendario") = value
        End Set
    End Property

    Private Property CodiceAmbulatorioPazienteSelezionato As String
        Get
            Return ViewState("AMB")
        End Get
        Set(value As String)
            ViewState("AMB") = value
        End Set
    End Property

    ''' <summary>
    ''' Indica se la modale di rilevazione del consenso si è aperta automaticamente a causa di un tentativo di redirect al dettaglio di un paziente con stato consenso bloccante
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property DestinazioneRedirectPazienteSelezionato() As DestinazioneRedirect
        Get
            If ViewState("DRPS") Is Nothing Then ViewState("DRPS") = DestinazioneRedirect.NessunaSelezione
            Return ViewState("DRPS")
        End Get
        Set(Value As DestinazioneRedirect)
            ViewState("DRPS") = Value
        End Set
    End Property

#End Region

#Region " Types "

    Private Enum DestinazioneRedirect
        NessunaSelezione = 0
        DettaglioPaziente = 1
        ConvocazioniPaziente = 2
    End Enum

#End Region

#Region " Eventi Page "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        'Pulizia e log della session
        Dim sc As New SessionCleaner(Me.Settings)
        sc.Start()

        Select Case Request.Form("__EVENTTARGET")

            Case "Ricerca"
                '--
                ' Ricerca avviata cliccando sul giorno nel calendario del datapicker
                '--
                Me.Cerca(String.Empty)

            Case "RefreshFromPopup"
                '--
                ' Se ho appena chiuso la modale di rilevazione del consenso, che si era aperta automaticamente perchè lo stato del cons del paz era bloccante,
                ' allora ricontrollo lo stato del consenso e, se non è più bloccante, faccio subito il redirect al dettaglio del paziente selezionato.
                '--
                Dim redirect As Boolean = False
                '--
                If Me.DestinazioneRedirectPazienteSelezionato <> DestinazioneRedirect.NessunaSelezione Then
                    '--
                    Dim destinazioneRedirectPaziente As DestinazioneRedirect = Me.DestinazioneRedirectPazienteSelezionato
                    Me.DestinazioneRedirectPazienteSelezionato = DestinazioneRedirect.NessunaSelezione
                    '--
                    ' N.B. : al click del pulsante di dettaglio viene memorizzato il paziente selezionato, poi viene aperta la pop-up di rilevazione.
                    '        In questo punto, sfrutto il codice memorizzato nella struttura per fare il redirect, quindi deve essere stata valorizzata!   
                    '--
                    Dim consensoGlobalePaziente As Entities.Consenso.StatoConsensoPaziente =
                        OnVacUtility.GetConsensoUltimoPazienteSelezionato(Me.UltimoPazienteSelezionato, Me.Settings)
                    '--
                    If consensoGlobalePaziente Is Nothing Then
                        '--
                        redirect = False
                        '--
                    Else
                        '--
                        If consensoGlobalePaziente.Controllo = Enumerators.ControlloConsenso.Bloccante Then
                            redirect = False
                        Else
                            redirect = True
                            RedirectPazienteSelezionato(destinazioneRedirectPaziente, False)
                        End If
                        '--
                    End If
                    '--
                End If
                '--
                ' Se non effettuo il redirect, eseguo la ricerca per aggiornare i dati
                If Not redirect Then
                    Me.Cerca(String.Empty)
                End If
                '--
        End Select

        If Not IsPostBack Then
            '-- 
            Me.CodiciPazientiDaStampare = Nothing
            '-- 
            If Me.FiltroData Is Nothing Then
                Me.txtData.Text = DateTime.Today
            Else
                Me.txtData.Text = Me.FiltroData
            End If
            '--
            Me.ShowPrintButtons()
            '-- 
            Me.uscScegliAmb.cnsCodice = OnVacUtility.Variabili.CNS.Codice
            Me.uscScegliAmb.cnsDescrizione = OnVacUtility.Variabili.CNS.Descrizione
            '-- 
            Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                '-- 
                ' Gestione Consenso
                Me.ShowLegendaConsenso(genericProvider)
                '--
                If Me.Settings.SET_AMB_CALENDARIO Then
                    '--
                    If OnVacUtility.Variabili.CNS.Ambulatorio.Codice > 0 Then
                        '--
                        Me.uscScegliAmb.ambCodice = OnVacUtility.Variabili.CNS.Ambulatorio.Codice
                        Me.uscScegliAmb.ambDescrizione = genericProvider.Consultori.GetAmbDescrizione(OnVacUtility.Variabili.CNS.Ambulatorio.Codice)
                        '--
                    End If
                    '--
                Else
                    '--
                    Me.uscScegliAmb.ambCodice = 0
                    '--
                End If
                '--
                If Me.uscScegliAmb.ambCodice = 0 Then
                    '--
                    Using bizConsultori As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                        '--
                        Dim ambulatorio As Entities.Ambulatorio = bizConsultori.GetAmbulatorioDefault(Me.uscScegliAmb.cnsCodice, True, True)
                        '--
                        Me.uscScegliAmb.ambCodice = ambulatorio.Codice
                        Me.uscScegliAmb.ambDescrizione = ambulatorio.Descrizione
                        '--
                    End Using
                    '---
                End If
                '--
                If Me.uscScegliAmb.ambCodice > 0 Then
                    '--
                    Me.Cerca(String.Empty)
                    '--
                End If
                '--
            End Using
            '--
            Me.uscScegliAmb.databind()
            '--
        End If

    End Sub

#End Region

#Region " Eventi Toolbar "

    Private Sub ToolBar_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btnCerca"

                Me.Cerca(String.Empty)

            Case "btnPulisci"

                Me.Clear()

            Case "btnStampa"

                If String.IsNullOrEmpty(Me.CodiciPazientiDaStampare) Then
                    Me.OnitLayout31.InsertRoutineJS("alert('Nessun Paziente da stampare!');")
                    Exit Sub
                End If

                ' Provider per accesso al db (utilizzato per accedere alla t_ana_report e per creare il dataset AppuntamentiGiorno)
                Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

                    ' Info sul report AppuntamentiDelGiorno
                    Dim rptInfo As Entities.Report = Nothing

                    Using reportBiz As New BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                        rptInfo = reportBiz.GetReport(Constants.ReportName.AppuntamentiDelGiorno)
                    End Using

                    If rptInfo Is Nothing Then
                        OnitLayout31.InsertRoutineJS("alert('Report non trovato');")
                        Exit Sub
                    End If

                    ' Definizione report e parametri
                    Dim rpt As New ReportParameter()

                    Dim orarioAperturaPomeridiana As String = Settings.ORAPM & ":00"

                    rpt.AddParameter("OrarioAperturaPom", orarioAperturaPomeridiana)

                    ' VERSIONE PER REPORT CON BILANCI
                    Dim dsAppuntamentiBil As AppuntamentiGiornoBilanci = Nothing

                    Using bizApp As New BizAppuntamentiGiorno(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                        dsAppuntamentiBil = bizApp.CreaDataSetAppuntamentiGiornoBilanci(
                        uscScegliAmb.cnsCodice, uscScegliAmb.ambCodice, txtData.Text, txtData.Text, Enumerators.FiltroAvvisati.Tutti, Nothing)

                    End Using

                    rpt.set_dataset(dsAppuntamentiBil)

                    ' Parametro presente solo in questa versione del report
                    rpt.AddParameter("CnsDescrizione", uscScegliAmb.cnsDescrizione)

                    If Not OnVacReport.StampaReport(Page.Request.Path, rptInfo.Nome, String.Empty, rpt, , , rptInfo.Cartella) Then
                        OnVacUtility.StampaNonPresente(Page, rptInfo.Nome)
                    End If

                End Using

            Case "btnStampaEtichette"

                If String.IsNullOrWhiteSpace(Me.txtData.Text) Then
                    OnitLayout31.InsertRoutineJS("alert('Nessuna data selezionata!');")
                    Exit Sub
                End If

                Dim dataInizio As DateTime = Me.txtData.Data
                Dim dataFine As DateTime = Me.txtData.Data.AddDays(1)

                Dim sbFiltroAvvisi As New System.Text.StringBuilder()

                sbFiltroAvvisi.Append("(")
                sbFiltroAvvisi.AppendFormat("{{V_AVVISI.CNV_DATA_APPUNTAMENTO}} >=  Date({0}, {1}, {2}) AND ", dataInizio.Year, dataInizio.Month, dataInizio.Day)
                sbFiltroAvvisi.AppendFormat("{{V_AVVISI.CNV_DATA_APPUNTAMENTO}} <  Date({0}, {1}, {2}) ", dataFine.Year, dataFine.Month, dataFine.Day)
                sbFiltroAvvisi.Append(")")

                If uscScegliAmb.cnsCodice <> "" And uscScegliAmb.cnsDescrizione <> "" Then
                    sbFiltroAvvisi.AppendFormat(" AND {{V_AVVISI.CNV_CNS_CODICE}}='{0}'", uscScegliAmb.cnsCodice)
                End If

                If uscScegliAmb.ambCodice <> 0 Then
                    sbFiltroAvvisi.AppendFormat(" AND {{V_AVVISI.CNV_AMB_CODICE}}= {0} ", uscScegliAmb.ambCodice)
                End If

                Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                    Using reportBiz As New BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                        Dim rpt As New ReportParameter()

                        If Not OnVacReport.StampaReport(Constants.ReportName.EtichetteAssistitiAvvisi, sbFiltroAvvisi.ToString(), rpt, Nothing, Nothing, reportBiz.GetReportFolder(Constants.ReportName.EtichetteAssistitiAvvisi)) Then
                            OnVacUtility.StampaNonPresente(Page, Constants.ReportName.EtichetteAssistitiAvvisi)
                        End If

                    End Using
                End Using

        End Select

    End Sub

#End Region

#Region " Eventi Datalist "

    Private Sub dlsAppuntamenti_ItemCommand(source As Object, e As System.Web.UI.WebControls.DataListCommandEventArgs) Handles dlsAppuntamenti.ItemCommand

        Select Case e.CommandName

            Case "ApriRilevazioneConsenso"

                Dim codiceLocalePaziente As Integer = 0

                If Integer.TryParse(GetCodicePazienteFromSelectedDataListItem(e.Item), codiceLocalePaziente) Then
                    ApriRilevazioneConsenso(codiceLocalePaziente, False)
                End If

            Case "Seleziona"
                '--
                ' Redirect a Convocazioni
                '--
                Dim codicePazienteSelezionato As String = GetCodicePazienteFromSelectedDataListItem(e.Item)
                Dim codiceAmbulatorioSelezionato As String = DirectCast(e.Item.FindControl("lblCodAmb"), Label).Text

                If GetRichiestaConsensoTrattamentoDatiUtente(OnVacUtility.Variabili.CNS.Codice) Then

                    ucConsensoUtente.CodicePaziente = Convert.ToInt64(codicePazienteSelezionato)
                    ucConsensoUtente.CodiceConsultorio = OnVacUtility.Variabili.CNS.Codice
                    ucConsensoUtente.CodiceAmbulatorio = codiceAmbulatorioSelezionato
                    ucConsensoUtente.Destinazione = ConsensoTrattamentoDatiUtente.DestinazioneRedirect.ConvocazioniPaziente

                    fmConsensoUtente.VisibileMD = True

                Else

                    ' Memorizzazione codice paziente per ricerca rapida (l'impostazione del paziente corrente quando si effettua il redirect vero e proprio)
                    UltimoPazienteSelezionato = New Entities.UltimoPazienteSelezionato(String.Empty, codicePazienteSelezionato)

                    ' Memorizzazione ambulatorio per impostarlo prima del redirect
                    CodiceAmbulatorioPazienteSelezionato = codiceAmbulatorioSelezionato

                    RedirectPazienteSelezionato(DestinazioneRedirect.ConvocazioniPaziente, True)

                End If

            Case "Nome"
                '--
                ' Redirect a Gestione Paziente
                '--
                Dim codicePazienteSelezionato As String = GetCodicePazienteFromSelectedDataListItem(e.Item)

                If GetRichiestaConsensoTrattamentoDatiUtente(OnVacUtility.Variabili.CNS.Codice) Then

                    ucConsensoUtente.CodicePaziente = Convert.ToInt64(codicePazienteSelezionato)
                    ucConsensoUtente.CodiceConsultorio = OnVacUtility.Variabili.CNS.Codice
                    ucConsensoUtente.CodiceAmbulatorio = 0
                    ucConsensoUtente.Destinazione = ConsensoTrattamentoDatiUtente.DestinazioneRedirect.DettaglioPaziente

                    fmConsensoUtente.VisibileMD = True

                Else

                    ' Memorizzazione codice paziente per ricerca rapida (l'impostazione del paziente corrente avviene nel metodo RedirectToGestionePaziente)
                    UltimoPazienteSelezionato = New Entities.UltimoPazienteSelezionato(String.Empty, codicePazienteSelezionato)

                    ' In questo caso non serve memorizzazione l'ambulatorio
                    CodiceAmbulatorioPazienteSelezionato = String.Empty

                    RedirectPazienteSelezionato(DestinazioneRedirect.DettaglioPaziente, True)

                End If

                'Case "SortOra"

                '    Cerca("ORA")

                'Case "SortAmb"

                '    Cerca("CNV_AMB_CODICE")

                'Case "SortCognomeNome"

                '    Cerca("NOME")

                'Case "SortDataNascita"

                '    Cerca("PAZ_DATA_NASCITA")

                'Case "SortVaccinazioni"

                '    Cerca("VACCINAZIONI")

                'Case "SortDosi"

                '    Cerca("DOSE")

                'Case "SortDurata"

                '    Cerca("CNV_DURATA_APPUNTAMENTO")

                'Case "SortIdApp"

                '    Cerca("CNV_ID_CONVOCAZIONE")
        End Select

    End Sub

#End Region

#Region " Eventi Usercontrol "

    Private Sub uscScegliAmb_AmbulatorioCambiato(cnsCodice As String, ambCodice As Integer) Handles uscScegliAmb.AmbulatorioCambiato

        ' Se è stato modificato l'ambulatorio e il parametro vale true, imposta in session ambulatorio e flag "MedicoinAmbulatorio".
        If Settings.SET_AMB_CALENDARIO Then

            SetAmbulatorio(cnsCodice, ambCodice)

        End If

        Cerca(String.Empty)

    End Sub

    Private Sub ucConsensoUtente_ConsensoTrattamentoDatiUtenteAccettato() Handles ucConsensoUtente.ConsensoTrattamentoDatiUtenteAccettato

        ' Memorizzazione codice paziente per ricerca rapida (l'impostazione del paziente corrente quando si effettua il redirect vero e proprio)
        UltimoPazienteSelezionato = New Entities.UltimoPazienteSelezionato(String.Empty, ucConsensoUtente.CodicePaziente)

        ' Memorizzazione ambulatorio per impostarlo prima del redirect
        CodiceAmbulatorioPazienteSelezionato = ucConsensoUtente.CodiceAmbulatorio

        Dim dest As DestinazioneRedirect = DestinazioneRedirect.NessunaSelezione

        Select Case ucConsensoUtente.Destinazione

            Case ConsensoTrattamentoDatiUtente.DestinazioneRedirect.NessunaSelezione
                dest = DestinazioneRedirect.NessunaSelezione

            Case ConsensoTrattamentoDatiUtente.DestinazioneRedirect.DettaglioPaziente
                dest = DestinazioneRedirect.DettaglioPaziente

            Case ConsensoTrattamentoDatiUtente.DestinazioneRedirect.ConvocazioniPaziente
                dest = DestinazioneRedirect.ConvocazioniPaziente

            Case Else
                Throw New NotImplementedException("Valore non previsto per ConsensoTrattamentoDatiUtente.DestinazioneRedirect")

        End Select

        ucConsensoUtente.ClearParameters()
        fmConsensoUtente.VisibileMD = False

        RedirectPazienteSelezionato(dest, True)

    End Sub

    Private Sub ucConsensoUtente_ConsensoTrattamentoDatiUtenteRifiutato() Handles ucConsensoUtente.ConsensoTrattamentoDatiUtenteRifiutato

        ucConsensoUtente.ClearParameters()
        fmConsensoUtente.VisibileMD = False

    End Sub

#End Region

#Region " Private Methods "

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()

        listPrintButtons.Add(New PrintButton(Constants.ReportName.AppuntamentiDelGiorno, "btnStampa"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.EtichetteAssistitiAvvisi, "btnStampaEtichette"))

        Me.ShowToolbarPrintButtons(listPrintButtons, ToolBar)

    End Sub

    Private Sub Cerca(sort As String)

        ' Memorizzazione filtro data
        Me.FiltroData = Me.txtData.Data

        Dim strPazBld As New System.Text.StringBuilder()

        Dim dta As DataTable

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            ' Ricerca appuntamenti del giorno specificato. Restituisce anche lo stato del consenso del paziente, se gestito.
            ' Altrimenti la colonna UrlIconaStatoConsenso è nulla.
            Using bizApp As New BizAppuntamentiGiorno(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                dta = bizApp.CercaAppuntamentiGiorno(Me.uscScegliAmb.cnsCodice, Me.uscScegliAmb.ambCodice, Me.txtData.Text, sort)

                If dta Is Nothing Then Exit Sub

                Dim dtaTmp As New DataTable()

                Dim stbCons As New System.Text.StringBuilder()
                Dim stbSedute As New System.Text.StringBuilder()

                For i As Integer = 0 To dta.Rows.Count - 1

                    dtaTmp = bizApp.GetVaccDosiPaziente(dta.Rows(i)("PAZ_CODICE"), dta.Rows(i)("CNV_DATA"))

                    ' Creazione stringhe di vaccinazione e dose
                    If Not dtaTmp Is Nothing Then

                        For t As Integer = 0 To dtaTmp.Rows.Count - 1
                            stbCons.AppendFormat("{0}<BR>", dtaTmp.Rows(t)("VAC_DESCRIZIONE"))
                            stbSedute.AppendFormat("{0}<BR>", dtaTmp.Rows(t)("VPR_N_RICHIAMO"))
                        Next

                    End If
                    dta.Rows(i)("VACCINAZIONI") = stbCons.ToString()
                    dta.Rows(i)("DOSE") = stbSedute.ToString()

                    ' Creazione stringa con i codici dei pazienti
                    strPazBld.AppendFormat("{0},", dta.Rows(i)("PAZ_CODICE"))

                    stbCons.Remove(0, stbCons.Length)
                    stbSedute.Remove(0, stbSedute.Length)

                    dtaTmp.Clear()
                    dtaTmp.Rows.Clear()

                Next

            End Using
        End Using

        If strPazBld.Length > 0 Then
            strPazBld.Remove(strPazBld.Length - 1, 1)
        End If

        Me.CodiciPazientiDaStampare = strPazBld.ToString()

        ' Carico i dati del consenso
        If Me.Settings.CONSENSO_GES Then

            Dim listaCodiciPaziente As List(Of String) = dta.AsEnumerable().Select(Function(p) p("paz_codice_ausiliario").ToString()).ToList()
            If listaCodiciPaziente.Count > 0 Then

                Dim codiceAziendaRegistrazione As String = OnVacUtility.GetCodiceAziendaRegistrazione(Me.Settings)

                Using consenso As New Onit.OnAssistnet.GestioneConsenso.Wcf.Proxy.ConsensiService.ConsensiServiceClient()
                    iconeConsensi = consenso.GetUltimoConsensoIconaPazienti(listaCodiciPaziente.ToArray, OnVacContext.Azienda, Me.Settings.CONSENSO_APP_ID, codiceAziendaRegistrazione)
                End Using

            Else
                iconeConsensi = New Onit.OnAssistnet.GestioneConsenso.Wcf.Proxy.ConsensiService.IconeConsensi()
            End If

        End If

        Me.dlsAppuntamenti.DataSource = dta
        Me.dlsAppuntamenti.DataBind()

    End Sub

    Private Sub Clear()

        Me.txtData.Data = DateTime.Today
        Me.FiltroData = Nothing

        Me.dlsAppuntamenti.DataSource = Nothing
        Me.dlsAppuntamenti.DataBind()

    End Sub

    ' Se il consultorio selezionato è uguale a quello corrente:
    '   - se l'ambulatorio selezionato è diverso dal precedente, imposta quello selezionato in session.
    '   - se l'ambulatorio selezionato è diverso da "TUTTI", imposta anche il relativo flag "MedicoInAmbulatorio"
    '   - se l'ambulatorio selezionato è "TUTTI", imposta il flag "MedicoInAmbulatorio" uguale a Nothing.
    Private Sub SetAmbulatorio(codiceConsultorioSelezionato As String, codiceAmbulatorioSelezionato As Integer)

        ' Controllo che il consultorio selezionato sia lo stesso del consultorio corrente: se sì, posso modificare
        ' l'ambulatorio corrente (perchè fa parte del consultorio corrente) e caricare il flag del medico in amb.
        If OnVacUtility.Variabili.CNS.Ambulatorio.Codice <> codiceAmbulatorioSelezionato Then

            ' Controllo che il consultorio corrente sia lo stesso di quello selezionato
            If OnVacUtility.Variabili.CNS.Codice = codiceConsultorioSelezionato Then

                ' Ambulatorio
                OnVacUtility.Variabili.CNS.Ambulatorio.Codice = codiceAmbulatorioSelezionato

                ' Flag medico in ambulatorio
                If OnVacUtility.Variabili.CNS.Ambulatorio.Codice > 0 Then

                    Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                        OnVacUtility.Variabili.MedicoInAmbulatorio = genericProvider.Consultori.GetMedicoInAmb(OnVacUtility.Variabili.CNS.Ambulatorio.Codice)
                    End Using

                Else
                    OnVacUtility.Variabili.MedicoInAmbulatorio = Nothing
                End If

            End If

        End If

    End Sub

    Private Function GetCodicePazienteFromSelectedDataListItem(dataItem As DataListItem) As String

        Return DirectCast(dataItem.FindControl("lblCodicePaz"), Label).Text

    End Function

    Private Sub RedirectPazienteSelezionato(destinazioneRedirectPaziente As DestinazioneRedirect, manageConsenso As Boolean)

        ' Il redirect viene bloccato se codice paziente nullo => qui il codice locale non deve mai essere nullo!
        If Me.UltimoPazienteSelezionato Is Nothing OrElse String.IsNullOrEmpty(Me.UltimoPazienteSelezionato.CodicePazienteLocale) Then Return

        ' Effettuo la gestione del consenso (controllo + apertura pop-up rilevazione se bloccante) solo se l'utente ha cliccato un pulsante di redirect.
        ' In caso di chiusura modale rilevazione, ho già controllato prima della chiamata di questo metodo.
        If manageConsenso Then

            Me.DestinazioneRedirectPazienteSelezionato = destinazioneRedirectPaziente

            ' --- Gestione Consenso --- '
            ' Nel caso in cui venga gestito il consenso, il redirect ai dati del paziente 
            ' non deve avvenire se il livello di consenso ha il flag controllo impostato a "bloccante".
            ' Se il flag vale "warning" il redirect avviene comunque, ma prima viene visualizzato un messaggio di alert.
            If Not CheckControlloConsenso(Me.UltimoPazienteSelezionato.CodicePazienteLocale) Then
                Return
            End If

        End If

        Select Case destinazioneRedirectPaziente

            Case DestinazioneRedirect.DettaglioPaziente

                Me.RedirectToGestionePaziente(Me.UltimoPazienteSelezionato.CodicePazienteLocale)

            Case DestinazioneRedirect.ConvocazioniPaziente

                ' Se il parametro è impostato a true, deve impostare in session l'ambulatorio 
                ' selezionato nell'appuntamento (se fa parte del cns corrente) e il relativo flag "MedicoInAmbulatorio"
                If Me.Settings.SET_AMB_ELENCO_CALENDARIO Then
                    Me.SetAmbulatorio(Me.uscScegliAmb.cnsCodice, Me.CodiceAmbulatorioPazienteSelezionato)
                End If

                Me.RedirectToConvocazioniPaziente(Me.UltimoPazienteSelezionato.CodicePazienteLocale)

        End Select

    End Sub

#End Region

#Region " Gestione consenso "

    ' Visualizzazione legenda delle icone del consenso, se gestito
    Private Sub ShowLegendaConsenso(genericProvider As DbGenericProvider)

        If Me.Settings.CONSENSO_GES Then

            ' Visualizza la legenda
            Me.cellLegendaConsenso.Visible = True

            ' Caricamento valori
            Using consenso As New Onit.OnAssistnet.GestioneConsenso.Wcf.Proxy.ConsensiService.ConsensiServiceClient()

                Dim listStatiConsenso As Onit.OnAssistnet.GestioneConsenso.Wcf.Proxy.ConsensiService.Icona() =
                    consenso.GetLegendaIconeConsenso(OnVacContext.Azienda, Me.Settings.CONSENSO_APP_ID)

                ' Bind repeater legenda
                Me.rtrConsenso.DataSource = listStatiConsenso
                Me.rtrConsenso.DataBind()

            End Using

        Else
            ' Nasconde la legenda
            Me.cellLegendaConsenso.Visible = False
        End If

    End Sub

    Private Sub dlsAppuntamenti_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataListItemEventArgs) Handles dlsAppuntamenti.ItemDataBound

        Select Case e.Item.ItemType

            Case ListItemType.Header
                e.Item.FindControl("headerConsensi").Visible = Me.Settings.CONSENSO_GES

            Case ListItemType.Item, ListItemType.AlternatingItem, ListItemType.SelectedItem

                e.Item.FindControl("itemConsenso").Visible = Me.Settings.CONSENSO_GES

                If Me.Settings.CONSENSO_GES Then

                    Dim imgBtnConsenso As System.Web.UI.WebControls.Image = DirectCast(e.Item.FindControl("imgBtnConsenso"), System.Web.UI.WebControls.Image)
                    Dim pazCodiceAusiliario As String = DirectCast(e.Item.FindControl("lblCodiceAusiliario"), System.Web.UI.WebControls.Label).Text

                    Dim statoConsenso As Onit.OnAssistnet.GestioneConsenso.Wcf.Proxy.ConsensiService.IconeConsensiItem =
                        iconeConsensi.Items.Where(Function(p) p.CodicePaziente = pazCodiceAusiliario).FirstOrDefault()

                    If statoConsenso Is Nothing Then
                        imgBtnConsenso.ImageUrl="~/Images/consensoAltro.png"
                    Else
                        Dim icona As Onit.OnAssistnet.GestioneConsenso.Wcf.Proxy.ConsensiService.Icona =
                            iconeConsensi.Icone.Where(Function(p) p.ID = statoConsenso.IconaID).FirstOrDefault()

                        imgBtnConsenso.ImageUrl = icona.Url
                    End If

                End If

        End Select

    End Sub

    ' Se il paziente ha il codice ausiliario apre la popup e carica l'applicativo per la rilevazione del consenso
    Private Sub ApriRilevazioneConsenso(codicePazienteLocale As Integer, autoEdit As Boolean)

        Dim codiceAusiliarioPaziente As String = String.Empty

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            codiceAusiliarioPaziente = genericProvider.Paziente.GetCodiceAusiliario(codicePazienteLocale)
        End Using

        If codiceAusiliarioPaziente = String.Empty Then
            AlertClientMsg(Me.Settings.CONSENSO_MSG_NO_COD_CENTRALE)
        Else
            ApriRilevazioneConsenso(codiceAusiliarioPaziente, autoEdit)
        End If

    End Sub

    ' Apre la popup e carica l'applicativo per la rilevazione del consenso, dato il codice ausiliario del paziente 
    Private Sub ApriRilevazioneConsenso(codiceCentralePaziente As String, autoEdit As Boolean)

        Me.modConsenso.VisibileMD = True
        Me.frameConsenso.Attributes.Add("src", Me.GetUrlMascheraRilevazioneConsenso(codiceCentralePaziente, autoEdit))

    End Sub

    ' Se il flag di controllo del consenso del paziente non è bloccante, restituisce true. 
    ' Altrimenti: visualizza un messaggio, apre la pop-up di rilevazione in edit (se il parametro CONSENSO_BLOCCANTE_AUTO_EDIT lo consente) e restituisce false.
    Private Function CheckControlloConsenso(pazCodice As String) As Boolean

        If Me.Settings.CONSENSO_GES Then

            ' Recupero codice ausiliario (CENTRALE) del paziente
            Dim codiceAusiliarioPaziente As String = String.Empty

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                codiceAusiliarioPaziente = genericProvider.Paziente.GetCodiceAusiliario(pazCodice)
            End Using

            If codiceAusiliarioPaziente = String.Empty Then
                AlertClientMsg(Me.Settings.CONSENSO_MSG_NO_COD_CENTRALE)
                Return False
            End If

            ' Recupero valore del consenso del paziente
            Dim consensoGlobalePaz As Entities.Consenso.StatoConsensoPaziente = OnVacUtility.GetConsensoGlobalePaziente(codiceAusiliarioPaziente, Me.Settings)

            If consensoGlobalePaz Is Nothing Then
                AlertClientMsg("Nessun consenso trovato. " + MSG_BLOCCO_REDIRECT_CONSENSO)
                Return False
            End If

            If consensoGlobalePaz.Controllo = Enumerators.ControlloConsenso.Bloccante Then
                '-- 
                ' Valore del consenso BLOCCANTE => blocco l'accesso al dettaglio/convocazioni e, se il param auto_edit vale true, apro la modale di rilevazione del consenso.
                '--
                Dim consensoBloccanteMessage As String =
                    consensoGlobalePaz.DescrizioneStatoConsenso + Environment.NewLine + MSG_BLOCCO_REDIRECT_CONSENSO

                If Me.Settings.CONSENSO_BLOCCANTE_AUTO_EDIT Then
                    ' Messaggio all'utente e apertura pop-up rilevazione del consenso in edit (nell'evento AlertClick dell'OnitLayout)
                    consensoBloccanteMessage += Environment.NewLine + "Verrà aperta la maschera di rilevazione del consenso."
                    Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(
                                               HttpUtility.JavaScriptStringEncode(consensoBloccanteMessage), OPEN_RILEVAZIONE_CONSENSO + "*" + codiceAusiliarioPaziente, False, True))
                Else
                    ' Messaggio all'utente
                    AlertClientMsg(consensoBloccanteMessage)
                End If

                Return False

            End If
        End If

        Return True

    End Function

    Private Sub OnitLayout31_AlertClick(sender As Object, e As Controls.PagesLayout.OnitLayout3.AlertEventArgs) Handles OnitLayout31.AlertClick

        If e.Key.StartsWith(OPEN_RILEVAZIONE_CONSENSO) Then

            Dim codiceCentralePaziente As String = String.Empty

            Dim key As String() = e.Key.Split("*")
            If key.Length > 1 Then
                codiceCentralePaziente = key(1)
            End If

            Me.ApriRilevazioneConsenso(codiceCentralePaziente, True)

        End If

    End Sub

#End Region

End Class
