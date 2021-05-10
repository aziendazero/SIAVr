Imports Onit.Controls


Partial Class OnVac_Cittadinanze
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

    Private _PanelUtility As OnitDataPanelUtility
    Public Property PanelUtility() As OnitDataPanelUtility
        Get
            Return _PanelUtility
        End Get
        Set(Value As OnitDataPanelUtility)
            _PanelUtility = Value
        End Set
    End Property

    Public ReadOnly Property Connessione() As String
        Get
            Return OnVacContext.Connection.ConnectionString
        End Get
    End Property

#End Region

#Region " Eventi Page "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        Me.PanelUtility = New OnitDataPanelUtility(Me.ToolBar)
        Me.PanelUtility.FindButtonName = "btnCerca"
        Me.PanelUtility.SaveButtonName = "btnSalva"
        Me.PanelUtility.DeleteButtonName = "btnElimina"
        Me.PanelUtility.CancelButtonName = "btnAnnulla"
        Me.PanelUtility.EditButtonName = "btnEdit"
        Me.PanelUtility.NewButtonName = "btnNew"
        Me.PanelUtility.MasterDataPanel = Me.odpCittadinanzeMaster
        Me.PanelUtility.DetailDataPanel = Me.odpCittadinanzeDetail
        Me.PanelUtility.WZDataGrid = Me.dgrCittadinanze
        Me.PanelUtility.WZRicBase = Me.WzFilter1.FindControl("WzFilterKeyBase")
        Me.PanelUtility.SetToolbarButtonImages()

        If Not IsPostBack Then
            Me.PanelUtility.InitToolbar()
        End If

    End Sub

#End Region

#Region " Eventi Toolbar "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked
        Me.PanelUtility.ManagingToolbar(e.Button.Key)
    End Sub

#End Region

#Region " Eventi OnitDataPanel "

    Private Sub odpCittadinanzeDetail_afterSaveData(sender As Onit.Controls.OnitDataPanel.OnitDataPanel) Handles odpCittadinanzeDetail.afterSaveData
        Me.odpCittadinanzeMaster.Find()
    End Sub

    Private Sub odpCittadinanzeDetail_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpCittadinanzeDetail.afterOperation
        '--
        Me.OnitLayout31.Busy = Me.PanelUtility.CheckToolBarState(operation)
        '--
        If Me.odpCittadinanzeDetail.getCurrentDataRow("CIT_CEE").ToString() = String.Empty OrElse Me.odpCittadinanzeDetail.getCurrentDataRow("CIT_CEE").ToString() = "N" Then
            Me.chkCee.Checked = False
        Else
            Me.chkCee.Checked = True
        End If
        '--
        Select Case operation
            Case OnitDataPanel.OnitDataPanel.CurrentOperationTypes.EditRecord, OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord
                Me.chkCee.Enabled = True
            Case Else
                Me.chkCee.Enabled = False
        End Select
        '--
    End Sub

    Private Sub odpCittadinanzeMaster_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpCittadinanzeMaster.afterOperation
        Me.OnitLayout31.Busy = Me.PanelUtility.CheckToolBarState(operation)
    End Sub

    Private Sub odpCittadinanzeMaster_onCreateQuery(ByRef QB As System.Object) Handles odpCittadinanzeMaster.onCreateQuery
        DirectCast(QB, AbstractQB).AddOrderByFields("CIT_CODICE")
    End Sub

#End Region

End Class
