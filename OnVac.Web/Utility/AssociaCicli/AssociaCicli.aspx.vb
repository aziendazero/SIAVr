Imports System.Threading

Imports Onit.Database.DataAccessManager
Imports Onit.Shared.Manager.OnitProfile
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Biz


Partial Class AssociaCicli
    Inherits OnVac.Common.PageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(sender As System.Object, e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Properties "

    ' Hashtable per mantenere codici e descrizioni degli stati anagrafici caricati da db
    Private Property HashStatiAnagrafici() As Hashtable
        Get
            Return Session("OnVacAssociaStati_HashStatiAnag")
        End Get
        Set(ByVal Value As Hashtable)
            Session("OnVacAssociaStati_HashStatiAnag") = Value
        End Set
    End Property

    Public Property StatiAnagrafici() As ArrayList
        Get
            If ViewState("StatiAnagrafici") Is Nothing Then
                ViewState("StatiAnagrafici") = New ArrayList
            End If

            Return ViewState("StatiAnagrafici")
        End Get
        Set(ByVal Value As ArrayList)
            ViewState("StatiAnagrafici") = Value
        End Set
    End Property

    Private Property dtPazienti() As DataTable
        Get
            Return Session("dtPazienti_AssociaCicli")
        End Get
        Set(ByVal Value As DataTable)
            Session("dtPazienti_AssociaCicli") = Value
        End Set
    End Property

    Public Property selTutti() As Boolean
        Get
            Return ViewState("selTutti")
        End Get
        Set(ByVal Value As Boolean)
            ViewState("selTutti") = Value
        End Set
    End Property

#End Region

#Region " Constants "

    Private Const DGR_PAGE = 100

#End Region

#Region " Page Events "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            selTutti = True

            dtPazienti = Nothing
            dtPazienti = New DataTable()

            LoadStatiAnagrafici()
            Me.lblStatoAnagrafico.Text = DescrizioneStatiAnagrafici()

            Me.omlConsultorio.Codice = OnVacUtility.Variabili.CNS.Codice
            Me.omlConsultorio.RefreshDataBind()

        End If

        Select Case Request.Form.Item("__EVENTTARGET")

            Case "selPazienti"
                selTutti = CBool(Request.Form.Item("__EVENTARGUMENT"))
                SelezionaPazienti(True)

        End Select

    End Sub

    Private Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles MyBase.PreRender

        If Me.dgrPazienti.Items.Count > 0 Then
            Me.OnitLayout31.InsertRoutineJS(String.Format("document.getElementById('chkSelTutti').checked = {0};", selTutti.ToString.ToLower()))
        End If

    End Sub

#End Region

#Region " Gestione Scelta Stati Anagrafici "

    ' Caricamento Stati Anagrafici dalla tabella t_ana_stati_anagrafici
    ' su db. Selezione degli stati anagrafici in base al flag san_chiamata
    Private Sub LoadStatiAnagrafici()

        Dim dt As DataTable = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizStatiAnagrafici As New Biz.BizStatiAnagrafici(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                dt = bizStatiAnagrafici.LeggiStatiAnagrafici()

                If Not String.IsNullOrWhiteSpace(bizStatiAnagrafici.Message) Then
                    Me.OnitLayout31.InsertRoutineJS(String.Format("alert('{0}');", bizStatiAnagrafici.Message))
                    Exit Sub
                End If

            End Using
        End Using

        Me.chlStatiAnagrafici.DataTextField = "SAN_DESCRIZIONE"
        Me.chlStatiAnagrafici.DataValueField = "SAN_CODICE"
        Me.chlStatiAnagrafici.DataSource = dt
        Me.chlStatiAnagrafici.DataBind()

        If (Not IsNothing(HashStatiAnagrafici)) Then HashStatiAnagrafici = Nothing
        HashStatiAnagrafici = New Hashtable()

        For i As Int16 = 0 To dt.Rows.Count - 1

            ' Creo l'hashtable con i codici e le descrizioni degli stati anagrafici,
            ' che servirà per recuperare la descrizione in base al codice dell'elemento.
            HashStatiAnagrafici.Add(dt.Rows(i)("SAN_CODICE").ToString(), dt.Rows(i)("SAN_DESCRIZIONE").ToString())

            If dt.Rows(i)("SAN_CHIAMATA").ToString() = "S" Then
                ' Aggiorno l'arraylist degli stati anagrafici selezionati
                StatiAnagrafici.Add(dt.Rows(i)("SAN_CODICE").ToString())
                ' Seleziono il check corrispondente nella checklist
                chlStatiAnagrafici.Items(i).Selected = True
            End If

        Next

    End Sub

    ' Restituisce la descrizione degli stati anagrafici selezionati (modifica 24/01/2005)
    ' Utilizzata anche nell'aspx per il databind della label con gli stati selezionati (21.11.2006)
    Protected Function DescrizioneStatiAnagrafici() As String

        Dim statoFiltro As New System.Text.StringBuilder()

        For count As Integer = 0 To StatiAnagrafici.Count - 1
            statoFiltro.AppendFormat("{0},", HashStatiAnagrafici(StatiAnagrafici(count).ToString))
        Next

        If statoFiltro.Length > 0 Then
            statoFiltro.Remove(statoFiltro.Length - 1, 1)
        End If

        Return statoFiltro.ToString()

    End Function

    Private Sub btnAggiungiStati_Click(sender As Object, e As System.EventArgs) Handles btnAggiungiStati.Click

        ' Imposto i check in base a quelli selezionati in precedenza (e confermati: se ho premuto annulla,
        ' non devo mantenere i check ma ripristinare quelli precedenti)
        If Not IsNothing(Me.StatiAnagrafici) Then

            For i As Integer = 0 To Me.chlStatiAnagrafici.Items.Count - 1
                If Me.StatiAnagrafici.Contains(Me.chlStatiAnagrafici.Items(i).Value) Then
                    Me.chlStatiAnagrafici.Items(i).Selected = True
                End If
            Next

        End If

        Me.fmStatiAnagrafici.VisibileMD = True

    End Sub

    Private Sub btnAnnullaSelezionaStati_Click(sender As Object, e As System.EventArgs) Handles btnAnnullaSelezionaStati.Click

        Me.fmStatiAnagrafici.VisibileMD = False

    End Sub

    Private Sub btnConfermaSelezionaStati_Click(sender As Object, e As System.EventArgs) Handles btnConfermaSelezionaStati.Click

        Me.StatiAnagrafici.Clear()

        For i As Integer = 0 To Me.chlStatiAnagrafici.Items.Count - 1
            If Me.chlStatiAnagrafici.Items(i).Selected Then
                Me.StatiAnagrafici.Add(Me.chlStatiAnagrafici.Items(i).Value)
            End If
        Next

        Me.lblStatoAnagrafico.Text = DescrizioneStatiAnagrafici()
        Me.fmStatiAnagrafici.VisibileMD = False

    End Sub

#End Region

#Region " Event Handlers "

    Private Sub ToolBar_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case be.Button.Key

            Case "btnCerca"

                Dim dt As DataTable = CercaPazienti()

                If dt Is Nothing Then Exit Sub

                If dtPazienti Is Nothing Then
                    dtPazienti = New DataTable()
                Else
                    dtPazienti.PrimaryKey = Nothing
                    dtPazienti.Clear()
                    dtPazienti.Columns.Clear()
                End If

                dtPazienti = dt.Copy
                dtPazienti.Columns.Add("Sel")

                For i As Integer = 0 To dtPazienti.Rows.Count - 1
                    dtPazienti.Rows(i)("Sel") = "S"
                Next

                dtPazienti.PrimaryKey = New DataColumn() {dtPazienti.Columns("PAZ_CODICE")}

                BindDgrPazienti()

            Case "btnAssocia"

                ' Aggiorna i check
                SelezionaPazienti(False)

                ' Controlla che ci sia almeno un paziente selezionato prima di avviare la procedura di associazione
                Dim dv As New DataView(dtPazienti)

                dv.RowFilter = "Sel='S'"

                If dv.Count = 0 Then
                    Me.OnitLayout31.InsertRoutineJS("alert('Nessun paziente selezionato: associazione non effettuata');")
                    Exit Sub
                End If

                '--------------------------------------------------

                Dim bizBatch As New BizBatch()

                Dim jobId As Integer
                Dim procId As Integer = 1   ' ID PROCEDURA ASSOCIA CICLI (FISSO!)

                Dim i As Integer

                Dim worker As New wsBatch.wsBatch()

                Dim returnedMessage As String = String.Empty
                Dim hasError As Boolean = False

                Try
                    bizBatch.addSelectionParameters("Consultorio", Me.omlConsultorio.Descrizione)
                    bizBatch.addSelectionParameters("Data di Nascita", String.Format("dal {0} al {1}", Me.dpkDataNascitaDa.Data.ToShortDateString(), Me.dpkDataNascitaA.Data.ToShortDateString()))
                    bizBatch.addSelectionParameters("Sesso", Me.ddlSesso.SelectedValue)
                    bizBatch.addSelectionParameters("Malattia", Me.omlMalattia.Descrizione)
                    bizBatch.addSelectionParameters("Categoria a Rischio", Me.omlCategorieRischio.Descrizione)
                    bizBatch.addSelectionParameters("Ciclo", Me.omlCicloCNV.Descrizione)
                    bizBatch.addSelectionParameters("Utente", OnVacContext.UserDescription)

                    Dim s As New System.Text.StringBuilder()

                    For i = 0 To chlStatiAnagrafici.Items.Count - 1

                        If Me.chlStatiAnagrafici.Items(i).Selected Then
                            s.Append(chlStatiAnagrafici.Items(i).Text)
                            s.Append("; ")
                        End If

                    Next

                    bizBatch.addSelectionParameters("Stato anagrafico", s.ToString)

                    ' Dati del job
                    Dim job_data As New wsBatch.JobInputData()

                    job_data.ProcedureId = procId
                    job_data.Paused = True
                    job_data.TotalItems = dv.Count
                    job_data.Input = New wsBatch.InputPort
                    job_data.Input.ApplicationId = OnVacContext.AppId
                    job_data.Input.UserId = OnVacContext.UserId
                    job_data.Input.AziendaCodice = OnVacContext.Azienda

                    ' Connessione al db
                    Dim conn(0) As wsBatch.PortConnection
                    conn(0) = New wsBatch.PortConnection
                    conn(0).Name = OnVacContext.AppId

                    job_data.Input.PortConnections = conn

                    ' Creazione del job
                    Try
                        jobId = worker.CreateJob(job_data)
                    Catch ex As Exception
                        Common.Utility.EventLogHelper.EventLogWrite(ex, OnVacContext.AppId)
                        jobId = Nothing
                    End Try

                    If (jobId <> Nothing AndAlso jobId > 0) Then

                        ' Procedura di riempimento della tabella T_PAZ_ELABORAZIONI con i pazienti da processare
                        bizBatch.jobId = jobId
                        bizBatch.procId() = procId
                        bizBatch.uteId = OnVacContext.UserId
                        bizBatch.addParameter(omlCicloCNV.Codice)

                        For i = 0 To dv.Count - 1
                            Dim codicePaziente As Integer = dv.Item(i)("paz_codice")
                            bizBatch.addPatient(codicePaziente)
                        Next

                        Try
                            Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                                bizBatch.setProvider(genericProvider)
                                bizBatch.fillTable()
                                bizBatch.storeParameters()
                            End Using

                        Catch ex As Exception
                            Common.Utility.EventLogHelper.EventLogWrite(ex, OnVacContext.AppId)
                            returnedMessage = "Problemi di comunicazione con il database. Processo di associazione NON avviato"
                            hasError = True
                        End Try

                        If Not hasError Then
                            ' Richiesta del servizio al web service
                            worker.ResumeJob(jobId)
                            returnedMessage = "Processo di associazione avviato"
                        End If

                    Else
                        returnedMessage = "Problemi di comunicazione con il servizio. Processo di associazione NON avviato"
                    End If
                    '--------------------------------------------------

                    Me.OnitLayout31.InsertRoutineJS(String.Format("alert('{0}');", returnedMessage))

                Finally

                    If Not worker Is Nothing Then worker.Dispose()

                End Try

        End Select

    End Sub

    Private Sub dgrPazienti_ChangePageEvnt(sender As Object, e As Onit.Controls.OnitGrid.ChangePageEventArgs) Handles dgrPazienti.ChangePageEvnt

        SelezionaPazienti(False)

        Me.dgrPazienti.CurrentPageIndex = e.new_page
        Me.dgrPazienti.DataSource = dtPazienti
        Me.dgrPazienti.DataBind()
        Me.dgrPazienti.SelectedIndex = -1

    End Sub

#End Region

#Region " Private "

    Private Sub BindDgrPazienti()

        Me.dgrPazienti.CurrentPageIndex = 0
        Me.dgrPazienti.DataSource = dtPazienti
        Me.dgrPazienti.DataBind()

        If dtPazienti Is Nothing Then

            Me.omlCicloCNV.Enabled = False
            Me.omlCicloCNV.CssClass = "TextBox_Data_Disabilitato"
            Me.ddlPager.Visible = False
            Me.dgrPazienti.Visible = False

            Me.lblPazientiTrovati.Text = "Pazienti trovati"

        Else

            Me.ddlPager.Visible = dtPazienti.Rows.Count > DGR_PAGE

            If dtPazienti.Rows.Count > 0 Then
                Me.omlCicloCNV.Enabled = True
                Me.omlCicloCNV.CssClass = "TextBox_Data_Obbligatorio"
                Me.ddlPager.Visible = True
                Me.dgrPazienti.Visible = True
            Else
                Me.omlCicloCNV.Enabled = False
                Me.omlCicloCNV.CssClass = "TextBox_Data_Disabilitato"
                Me.ddlPager.Visible = False
                Me.dgrPazienti.Visible = False
            End If

            Me.lblPazientiTrovati.Text = "Pazienti trovati: " + dtPazienti.Rows.Count.ToString()

        End If

    End Sub

    Private Function CercaPazienti() As DataTable

        Dim dt As DataTable = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using pazienteBiz As New Biz.BizPaziente(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                ' Filtri di ricerca
                If Me.omlConsultorio.Codice <> String.Empty Then
                    pazienteBiz.Filtri.Consultorio = omlConsultorio.Codice
                End If
                If Me.omlMalattia.Codice <> String.Empty Then
                    pazienteBiz.Filtri.Malattia = omlMalattia.Codice
                End If
                If Me.omlCategorieRischio.Codice <> String.Empty Then
                    pazienteBiz.Filtri.CategoriaRischio = omlCategorieRischio.Codice
                End If
                If Me.dpkDataNascitaDa.Text <> String.Empty Then
                    pazienteBiz.Filtri.DataNascita_Da = dpkDataNascitaDa.Data
                End If
                If Me.dpkDataNascitaA.Text <> String.Empty Then
                    pazienteBiz.Filtri.DataNascita_A = dpkDataNascitaA.Data
                End If
                If Me.ddlSesso.SelectedValue <> String.Empty Then
                    pazienteBiz.Filtri.Sesso = ddlSesso.SelectedValue
                End If
                If Me.StatiAnagrafici.Count > 0 Then
                    Dim s(Me.StatiAnagrafici.Count - 1) As String
                    For i As Integer = 0 To Me.StatiAnagrafici.Count - 1
                        s(i) = Me.StatiAnagrafici(i)
                    Next
                    pazienteBiz.Filtri.StatoAnagrafico = s
                End If

                Try
                    dt = pazienteBiz.CercaPazienti(True)
                Catch ex As Exception
                    Me.OnitLayout31.InsertRoutineJS("alert('Errore durante la ricerca dei pazienti.');")
                End Try

            End Using

        End Using

        Return dt

    End Function

    ' Prende la dtPazienti e pone nel campo "Sel" di ogni paziente una S o una N in base allo stato del check (paziente selezionato)
    Private Sub SelezionaPazienti(tutti As Boolean)

        If Not tutti Then

            For count As Integer = 0 To Me.dgrPazienti.Items.Count - 1
                'verifica la valorizzazione della selezione sul singolo paziente
                dtPazienti.Rows.Find(Me.dgrPazienti.Items(count).Cells(Me.dgrPazienti.getColumnNumberByKey("PAZ_CODICE")).Text)("Sel") =
                    IIf(CType(Me.dgrPazienti.Items(count).Cells(Me.dgrPazienti.getColumnNumberByKey("CheckBox")).FindControl("chkSel"), CheckBox).Checked, "S", "N")
            Next

        Else

            Dim strChk As String = IIf(selTutti, "S", "N")

            'imposta la selezione per tutti i pazienti
            For count As Integer = 0 To dtPazienti.Rows.Count - 1
                dtPazienti.Rows(count).Item("Sel") = strChk
            Next

            Me.dgrPazienti.DataSource = dtPazienti
            Me.dgrPazienti.DataBind()

        End If

    End Sub

#End Region

End Class


