Imports System.Collections.Generic
Imports Infragistics.WebUI.UltraWebToolbar
Imports Onit.Controls
Imports Onit.Controls.OnitDataPanel


Partial Class OnVac_Cicli
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

#Region " Properties "

    Public ReadOnly Property Connessione() As String
        Get
            Return OnVacContext.Connection.ConnectionString
        End Get
    End Property

    Private _PanelUtility As OnitDataPanelUtility
    Public Property PanelUtility() As OnitDataPanelUtility
        Get
            Return _PanelUtility
        End Get
        Set(Value As OnitDataPanelUtility)
            _PanelUtility = Value
        End Set
    End Property

#End Region

#Region " Eventi Page "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        Me.PanelUtility = New OnitDataPanelUtility(ToolBarCicli)
        Me.PanelUtility.FindButtonName = "btnCerca"
        Me.PanelUtility.DeleteButtonName = "btnElimina"
        Me.PanelUtility.SaveButtonName = "btnSalva"
        Me.PanelUtility.CancelButtonName = "btnAnnulla"
        Me.PanelUtility.MasterDataPanel = Me.OdpCicliMaster
        Me.PanelUtility.DetailDataPanel = Me.OdpCicliDetail
        Me.PanelUtility.OtherButtonsNames = "btnSedute"
        Me.PanelUtility.WZMsDataGrid = Me.WzDgrCicli
        Me.PanelUtility.WZRicBase = Me.WzFilter1.FindControl("WzFilterKeyBase")
        Me.PanelUtility.SetToolbarButtonImages()

        AddHandler Me.PanelUtility.BeforeSave, AddressOf PanelUtility_BeforeSave

        If Not IsPostBack() Then

            Me.PanelUtility.InitToolbar()

            'slokkiamo il paziente lokkato...
            Me.LayoutCicli.lock.EndLock(OnVacUtility.Variabili.PazId)

            ShowPrintButtons()

        End If

    End Sub

#End Region

#Region " Eventi Toolbar "

    Private Sub ToolBarCicli_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBarCicli.ButtonClicked

        Select Case e.Button.Key

            Case "btnSedute"
                Me.Response.Redirect(String.Format("Sedute.aspx?CIC={0}|{1}", HttpUtility.UrlEncode(WzTbCodCiclo.Text), HttpUtility.UrlEncode(WzTbDescCiclo.Text)), False)

            Case "btn_Stampa"
                Stampa()

            Case Else
                PanelUtility.ManagingToolbar(e.Button.Key)

        End Select

    End Sub

#End Region

#Region " Eventi OnitDataPanel e PanelUtility "

    Private Sub PanelUtility_BeforeSave(sender As Object, e As OnitDataPanelUtility.BeforeOperationEventArgs)

        If Me.OdpCicliDetail.CurrentOperation = Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord Then
            '--
            Me.WzTbCodCiclo.Text = Me.WzTbCodCiclo.Text.Trim()
            Dim result As CheckResult = Me.CheckCampoCodice(Me.WzTbCodCiclo.Text)
            '--
            If Not result.Success Then
                e.Cancel = True
                Me.LayoutCicli.ShowMsgBox(New PagesLayout.OnitLayout3.OnitLayoutMsgBox("Salvataggio non effettuato.\n" + result.Message, "CodErr", False, False))
                Return
            End If
            '--
        End If

    End Sub

    Private Sub OdpCicliMaster_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles OdpCicliMaster.afterOperation
        AfterPanelOperation(operation)
    End Sub

    Private Sub OdpCicliMaster_onCreateQuery(ByRef QB As System.Object)
        DirectCast(QB, AbstractQB).AddOrderByFields("cic_descrizione")
    End Sub

    'nel caso in cui sia impossibile eliminare i cicli già utilizzati (modifica 30/12/2004)
    Private Sub OdpCicliMaster_onError(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, err As Onit.Controls.OnitDataPanel.OnitDataPanelError)

        If Not err.exc Is Nothing AndAlso err.exc.Message.Contains(Constants.OracleErrors.ORA_02292) Then
            err.exc = New Exception(" (i cicli risultano già utilizzati nel programma)")
            err.comment = "Attenzione: non è stato possibile eliminare i cicli selezionati!"
        End If
    End Sub

    Private Sub OdpCicliDetail_afterSaveData(sender As Onit.Controls.OnitDataPanel.OnitDataPanel) Handles OdpCicliDetail.afterSaveData
        Me.OdpCicliMaster.Find()
    End Sub

    Private Sub OdpCicliDetail_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles OdpCicliDetail.afterOperation
        AfterPanelOperation(operation)
    End Sub

    Private Sub OdpCicliDetail_onCreateQuery(ByRef QB As Object) Handles OdpCicliDetail.onCreateQuery
        DirectCast(QB, AbstractQB).AddOrderByFields("cic_descrizione")
    End Sub

#End Region

#Region " Private "

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()
        listPrintButtons.Add(New PrintButton(Constants.ReportName.Cicli, "btn_Stampa"))

        Me.ShowToolbarPrintButtons(listPrintButtons, ToolBarCicli)

    End Sub

    'stampa il report con i cicli e le sedute relative (modifica 27/08/2004)
    Private Sub Stampa()

        'filtro sul parametro di ricerca
        Dim filtroRic As String = DirectCast(WzFilter1.FindControl("WzFilterKeyBase"), TextBox).Text.ToUpper()

        Dim stbFiltro As New System.Text.StringBuilder()
        stbFiltro.AppendFormat("(INSTR({{T_ANA_CICLI.CIC_CODICE}}, '{0}') > 0", filtroRic)
        stbFiltro.AppendFormat(" OR INSTR({{T_ANA_CICLI.CIC_DESCRIZIONE}}, '{0}') > 0)", filtroRic)

        'filtro per recuperare il campo standard
        stbFiltro.Append(" AND {T_ANA_CODIFICHE.COD_CAMPO} = 'CIC_STANDARD'")

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                If Not OnVacReport.StampaReport(Page.Request.Path, Constants.ReportName.Cicli, stbFiltro.ToString(), , , , bizReport.GetReportFolder(Constants.ReportName.Cicli)) Then
                    OnVacUtility.StampaNonPresente(Page, Constants.ReportName.Cicli)
                End If

            End Using
        End Using

    End Sub

    Private Sub AfterPanelOperation(operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes)

        LayoutCicli.Busy = PanelUtility.CheckToolBarState(operation)

        Dim btn_Stampa As TBarButton = Me.ToolBarCicli.Items.FromKeyButton("btn_Stampa")

        Select Case operation

            Case OnitDataPanel.CurrentOperationTypes.EditRecord,
                 OnitDataPanel.CurrentOperationTypes.DeleteRecord,
                 OnitDataPanel.CurrentOperationTypes.NewRecord

                If Not btn_Stampa Is Nothing Then btn_Stampa.Enabled = False

                WzSesso.Enabled = Settings.GESSESSOCICLI

            Case Else

                If Not btn_Stampa Is Nothing Then btn_Stampa.Enabled = True

        End Select

    End Sub

#End Region

End Class
