Partial Class OnVac_Distretti
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

        PanelUtility = New OnitDataPanelUtility(ToolBar)
        PanelUtility.FindButtonName = "btnCerca"
        PanelUtility.DeleteButtonName = "btnElimina"
        PanelUtility.SaveButtonName = "btnSalva"
        PanelUtility.CancelButtonName = "btnAnnulla"
        PanelUtility.MasterDataPanel = odpDistrettiMaster
        PanelUtility.DetailDataPanel = odpDistrettiDetail
        PanelUtility.WZMsDataGrid = dgrDistretti
        PanelUtility.WZRicBase = WzFilter1.FindControl("WzFilterKeyBase")
        PanelUtility.SetToolbarButtonImages()

        If Not IsPostBack Then
            PanelUtility.InitToolbar()
            'slokkiamo il paziente lokkato...
            OnitLayout31.lock.EndLock(OnVacUtility.Variabili.PazId)
        End If

    End Sub

#End Region

#Region " Eventi Toolbar "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked
        PanelUtility.ManagingToolbar(e.Button.Key)
    End Sub

#End Region

#Region " Eventi OnitDataPanel "

    Private Sub odpDistrettiMaster_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpDistrettiMaster.afterOperation
        Me.OnitLayout31.Busy = Me.PanelUtility.CheckToolBarState(operation)
    End Sub

    Private Sub odpDistrettiMaster_onCreateQuery(ByRef QB As System.Object) Handles odpDistrettiMaster.onCreateQuery
        DirectCast(QB, AbstractQB).AddOrderByFields("DIS_CODICE")
    End Sub

    Private Sub odpDistrettiDetail_afterSaveData(sender As Onit.Controls.OnitDataPanel.OnitDataPanel) Handles odpDistrettiDetail.afterSaveData
        Me.odpDistrettiMaster.Find()
    End Sub

    Private Sub odpDistrettiDetail_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpDistrettiDetail.afterOperation
        Me.OnitLayout31.Busy = Me.PanelUtility.CheckToolBarState(operation)
    End Sub

#End Region

End Class
