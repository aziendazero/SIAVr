Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.Web


Partial Class RegistroProcessi
    Inherits OnVac.Common.PageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Property "

    Private Property DtJobs() As DataTable
        Get
            If (Session("dtJobsOnVac") Is Nothing) Then Return Nothing
            Return DirectCast(Session("dtJobsOnVac"), DataTable)
        End Get
        Set(ByVal Value As DataTable)
            Session("dtJobsOnVac") = Value
        End Set
    End Property

    Private Property HtUtenti() As Hashtable
        Get
            If (Session("htUtentiOnVac") Is Nothing) Then Return Nothing
            Return DirectCast(Session("htUtentiOnVac"), Hashtable)
        End Get
        Set(ByVal Value As Hashtable)
            Session("htUtentiOnVac") = Value
        End Set
    End Property

    Private Property HtProcedure() As Hashtable
        Get
            If (Session("htProcedureOnVac") Is Nothing) Then Return Nothing
            Return DirectCast(Session("htProcedureOnVac"), Hashtable)
        End Get
        Set(ByVal Value As Hashtable)
            Session("htProcedureOnVac") = Value
        End Set
    End Property

    Private Property ListaCodiciProcedureNoStampaRisultati As List(Of Integer)
        Get
            If ViewState("ListaCodiciProcedureNoStampaRisultati") Is Nothing Then ViewState("ListaCodiciProcedureNoStampaRisultati") = New List(Of Integer)()
            Return ViewState("ListaCodiciProcedureNoStampaRisultati")
        End Get
        Set(value As List(Of Integer))
            ViewState("ListaCodiciProcedureNoStampaRisultati") = value
        End Set
    End Property

    Private Property FiltriCorrenti() As FiltriRicerca
        Get
            If Session("FiltriCorrenti") Is Nothing Then Session("FiltriCorrenti") = New FiltriRicerca
            Return DirectCast(Session("FiltriCorrenti"), FiltriRicerca)
        End Get
        Set(Value As FiltriRicerca)
            Session("FiltriCorrenti") = Value
        End Set
    End Property

    <Serializable>
    Private Class FiltriRicerca
        Public Property CodiceOperatore As String
        Public Property DescrizioneOperatore As String
        Public Property IdOperatore As String
        Public Property IdProcedura As Integer
        Public Property InizioDataEsecuzione As Date
        Public Property FineDataEsecuzione As Date
    End Class

#End Region

#Region " Enums "

    Private Enum ColumnIndex
        Stampa = 0
        Operatore = 1
        Procedura = 2
        Processo = 3
        Richiesta = 4
        Inizio = 5
        Fine = 6
        Tot = 7
        Elab = 8
        Err = 9
        Stato = 10
        IdProcedura = 11
    End Enum

#End Region

#Region " Event handlers "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not Me.IsPostBack Then

            Me.HtProcedure = New Hashtable()
            Me.HtUtenti = New Hashtable()
            '--
            'Utilizza i filtri precedentemente utilizzati nella sessione
            If Not String.IsNullOrWhiteSpace(FiltriCorrenti.CodiceOperatore) Then
                omlOperatore.Codice = FiltriCorrenti.CodiceOperatore
                omlOperatore.Descrizione = FiltriCorrenti.DescrizioneOperatore
                omlOperatore.ValoriAltriCampi("Id") = FiltriCorrenti.IdOperatore
            End If
            cmbProcesso.ClearSelection()
            If FiltriCorrenti.IdProcedura > 0 Then
                cmbProcesso.SelectedValue = FiltriCorrenti.IdProcedura.ToString()
            End If
            ' Filtro intervallo esecuzione procedure
            If FiltriCorrenti.FineDataEsecuzione <> Date.MinValue Then
                dpkDataEsecuzioneA.Data = FiltriCorrenti.FineDataEsecuzione
            Else
                dpkDataEsecuzioneA.Data = Date.Now
            End If

            Dim giorniIntervalloEsecuzione As Integer = 30
            If Not Me.Settings.REGPROC_INTERVALLO_GIORNI_FILTRO_ESECUZIONE Is Nothing Then

                giorniIntervalloEsecuzione = Me.Settings.REGPROC_INTERVALLO_GIORNI_FILTRO_ESECUZIONE

            End If

            If FiltriCorrenti.InizioDataEsecuzione <> Date.MinValue Then
                dpkDataEsecuzioneDa.Data = FiltriCorrenti.InizioDataEsecuzione
            Else
                dpkDataEsecuzioneDa.Data = dpkDataEsecuzioneA.Data.AddDays(-giorniIntervalloEsecuzione)
            End If

            Me.dpkDataEsecuzioneDa.Data = Date.Now.AddDays(-giorniIntervalloEsecuzione)
            '--
            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                ' Lettura procedure per cui non deve essere abilitata la stampa dei risultati
                Me.ListaCodiciProcedureNoStampaRisultati = genericProvider.Procedure.GetProcedureNoStampaRisultati()

                ' Lettura informazioni sulle procedure
                Dim dt As DataTable = Me.GetProcedure(genericProvider)

                For i As Integer = 0 To dt.Rows.Count - 1

                    Me.HtProcedure.Add(CInt(dt.Rows(i)("prd_codice")), dt.Rows(i)("prd_descrizione"))

                Next

                If Not dt Is Nothing Then

                    Dim dv As New DataView(dt)
                    dv.Sort = "prd_descrizione"

                    Me.cmbProcesso.DataTextField = "prd_descrizione"
                    Me.cmbProcesso.DataValueField = "prd_codice"
                    Me.cmbProcesso.DataSource = dv
                    Me.cmbProcesso.DataBind()

                End If

            End Using

            Me.BindDgrProcessi()

        End If

    End Sub

    Private Sub ToolBar_ButtonClicked(ByVal sender As Object, ByVal be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case be.Button.Key

            Case "btnCerca" ', "btnAggiorna"
                Me.SalvaFiltriInSession()
                Me.BindDgrProcessi()

            Case "btnPulisci"
                Me.PulisciFiltri()
                Me.SalvaFiltriInSession()
                Me.BindDgrProcessi()

        End Select

    End Sub

    Private Sub SalvaFiltriInSession()
        ' Memorizzazione filtri in session
        FiltriCorrenti.CodiceOperatore = omlOperatore.Codice
        FiltriCorrenti.DescrizioneOperatore = omlOperatore.Descrizione
        FiltriCorrenti.IdOperatore = omlOperatore.ValoriAltriCampi("Id")

        If String.IsNullOrWhiteSpace(cmbProcesso.SelectedValue) Then
            FiltriCorrenti.IdProcedura = 0
        Else
            FiltriCorrenti.IdProcedura = Convert.ToInt32(cmbProcesso.SelectedValue)
        End If

        FiltriCorrenti.InizioDataEsecuzione = dpkDataEsecuzioneDa.Data
        FiltriCorrenti.FineDataEsecuzione = dpkDataEsecuzioneA.Data
    End Sub

    Private Sub PulisciFiltri()
        omlOperatore.Codice = ""
        omlOperatore.Descrizione = ""
        cmbProcesso.ClearSelection()
        dpkDataEsecuzioneA.Data = Date.Now
        Dim giorniIntervalloEsecuzione As Integer = 30
        If Not Settings.REGPROC_INTERVALLO_GIORNI_FILTRO_ESECUZIONE Is Nothing Then
            giorniIntervalloEsecuzione = Settings.REGPROC_INTERVALLO_GIORNI_FILTRO_ESECUZIONE
        End If
        dpkDataEsecuzioneDa.Data = dpkDataEsecuzioneA.Data.AddDays(-giorniIntervalloEsecuzione)


    End Sub

    Private Sub omlOperatore_SetUpFiletr(sender As Object) Handles omlOperatore.SetUpFiletr

        Me.omlOperatore.Filtro = OnVacUtility.GetFiltroUtenteForOnitModalList(False)

    End Sub

#Region " Eventi Datagrid "

    Private Sub dgrProcessi_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgrProcessi.ItemDataBound

        Select Case e.Item.ItemType

            Case ListItemType.AlternatingItem, ListItemType.EditItem, ListItemType.Item, ListItemType.SelectedItem

                Dim textIdProcedura As String = e.Item.Cells(ColumnIndex.IdProcedura).Text

                If Not String.IsNullOrEmpty(textIdProcedura) Then

                    Dim procedureId As Integer

                    If Integer.TryParse(textIdProcedura, procedureId) Then

                        If Me.ListaCodiciProcedureNoStampaRisultati.Contains(procedureId) Then

                            Dim btnStampa As ImageButton = DirectCast(e.Item.Cells(ColumnIndex.Stampa).FindControl("btnStampa"), ImageButton)

                            If Not btnStampa Is Nothing Then btnStampa.Visible = False

                        End If

                    End If

                End If

        End Select

    End Sub

    Private Sub dgrProcessi_ItemCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgrProcessi.ItemCommand

        If e.CommandName = "Stampa" Then

            Dim jobId As Long = 0

            Try
                jobId = Convert.ToInt64(e.Item.Cells(ColumnIndex.Processo).Text)
            Catch ex As Exception
                jobId = 0
            End Try

            Me.Stampa(jobId)

        End If

    End Sub

#End Region

#End Region

#Region " Private members "

    Private Function GetFilterFromPageStatus() As wsBatch.JobSearchFilters

        Dim jobFilter As New wsBatch.JobSearchFilters()

        jobFilter.IdApplicativo = OnVacContext.AppId

        If Me.cmbProcesso.SelectedIndex > 0 Then   ' l'indice 0 è l'elemento ""
            jobFilter.IdProcedura = Convert.ToInt32(Me.cmbProcesso.SelectedValue)
        End If

        If Me.omlOperatore.Codice <> Nothing Then
            jobFilter.IdUtente = Convert.ToInt32(Me.omlOperatore.ValoriAltriCampi("Id"))
        End If

        If Me.dpkDataEsecuzioneDa.Data <> Nothing Then
            jobFilter.DataInizioRichiesta = Me.dpkDataEsecuzioneDa.Data
        End If

        If Me.dpkDataEsecuzioneA.Data <> Nothing Then
            jobFilter.DataFineRichiesta = Me.dpkDataEsecuzioneA.Data
        End If

        Return jobFilter

    End Function

    Private Function BuildJobsTable() As DataTable

        Dim wsBatch As New wsBatch.wsBatch()

        Dim jobFilter As wsBatch.JobSearchFilters = Me.GetFilterFromPageStatus()

        Dim jobId As Long() = wsBatch.GetJobsId(jobFilter)

        If Me.DtJobs Is Nothing Then

            Me.DtJobs = New DataTable()

            Me.DtJobs.Columns.Add("OPERATORE", System.Type.GetType("System.String"))
            Me.DtJobs.Columns.Add("PROCEDURA", System.Type.GetType("System.String"))
            Me.DtJobs.Columns.Add("PROCESSO", System.Type.GetType("System.String"))
            Me.DtJobs.Columns.Add("RICHIESTA", System.Type.GetType("System.DateTime"))
            Me.DtJobs.Columns.Add("INIZIO", System.Type.GetType("System.DateTime"))
            Me.DtJobs.Columns.Add("FINE", System.Type.GetType("System.DateTime"))
            Me.DtJobs.Columns.Add("TOT")
            Me.DtJobs.Columns.Add("ELAB")
            Me.DtJobs.Columns.Add("ERR")
            Me.DtJobs.Columns.Add("STATO", System.Type.GetType("System.String"))
            Me.DtJobs.Columns.Add("IDPROC", System.Type.GetType("System.String"))

            Me.DtJobs.AcceptChanges()

        End If

        Me.DtJobs.Clear()

        Dim jobInfo As wsBatch.JobInfo

        If Not jobId Is Nothing AndAlso jobId.Length > 0 Then

            Dim jobRow As DataRow

            For i As Integer = 0 To jobId.Length - 1

                jobInfo = wsBatch.GetJobInfo(jobId(i))

                jobRow = Me.DtJobs.NewRow()

                If Not Me.HtUtenti.Contains(jobInfo.IdUtente) Then

                    Dim ute_cod As String = String.Empty

                    Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                        Dim biz As New Biz.BizBatch(genericProvider)

                        ute_cod = biz.GetUtenteCodice(jobInfo.IdUtente)

                    End Using

                    Me.HtUtenti.Add(jobInfo.IdUtente, ute_cod)

                End If

                jobRow("OPERATORE") = Me.HtUtenti(jobInfo.IdUtente)
                jobRow("IDPROC") = jobInfo.IdProcedura
                jobRow("PROCEDURA") = Me.HtProcedure(jobInfo.IdProcedura)
                jobRow("PROCESSO") = jobInfo.IdProcesso
                If jobInfo.DataRichiesta = Date.MinValue Then
                    jobRow("RICHIESTA") = DBNull.Value
                Else
                    jobRow("RICHIESTA") = jobInfo.DataRichiesta
                End If
                If jobInfo.Status.StartingTime = Date.MinValue Then
                    jobRow("INIZIO") = DBNull.Value
                Else
                    jobRow("INIZIO") = jobInfo.Status.StartingTime
                End If
                If jobInfo.Status.FinishingTime = Date.MinValue Then
                    jobRow("FINE") = DBNull.Value
                Else
                    jobRow("FINE") = jobInfo.Status.FinishingTime
                End If
                jobRow("TOT") = jobInfo.Status.TotalItems
                jobRow("ELAB") = jobInfo.Status.ProcessedItems
                jobRow("ERR") = jobInfo.Status.NumberOfErrors
                jobRow("STATO") = jobInfo.Status.Status

                Me.DtJobs.Rows.Add(jobRow)

                Me.DtJobs.AcceptChanges()

            Next

        End If

        Return Me.DtJobs

    End Function

    Private Sub BindDgrProcessi()

        Me.dgrProcessi.DataSource = BuildJobsTable()
        Me.dgrProcessi.DataBind()

        If Not ExistsRptProcessi() Then
            Me.dgrProcessi.Columns(ColumnIndex.Stampa).Visible = False
        End If

    End Sub

    Private Function ExistsRptProcessi() As Boolean

        Dim exists As Boolean = False

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                exists = bizReport.ExistsReport(Constants.ReportName.RisultatiProcessi)

            End Using
        End Using

        Return exists

    End Function

    Private Sub Stampa(jobId As Long)

        Dim rpt As New ReportParameter()

        Dim strFiltro As String = "{T_PAZ_ELABORAZIONI.PLB_ID} = " + jobId.ToString()

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                If Not OnVacReport.StampaReport(Page.Request.Path, Constants.ReportName.RisultatiProcessi, strFiltro, rpt, , , bizReport.GetReportFolder(Constants.ReportName.RisultatiProcessi)) Then
                    OnVacUtility.StampaNonPresente(Page, Constants.ReportName.RisultatiProcessi)
                End If

            End Using
        End Using

    End Sub

    Private Function GetProcedure(genericProvider As DAL.DbGenericProvider) As DataTable

        Dim biz As New Biz.BizBatch(genericProvider)
        Dim result As Biz.BizBatch.GetProceduresResult = biz.GetProcedures()

        If Not result.Success Then
            Me.OnitLayout31.InsertRoutineJS(String.Format("alert('{0}');", result.Message))
        End If

        Return result.DataTableProcedure

    End Function

#End Region

End Class
