Imports Onit.Controls


Partial Class OnVac_MotiviSospensione
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
        Me.PanelUtility.DeleteButtonName = "btnElimina"
        Me.PanelUtility.SaveButtonName = "btnSalva"
        Me.PanelUtility.CancelButtonName = "btnAnnulla"
        Me.PanelUtility.MasterDataPanel = Me.odpMotivoSospensioneMaster
        Me.PanelUtility.DetailDataPanel = Me.odpMotivoSospensioneDetail
        Me.PanelUtility.WZDataGrid = Me.dgrSospensioni
        Me.PanelUtility.WZRicBase = Me.TabFiltri.FindControl("WzFilterKeyBase")
        Me.PanelUtility.SetToolbarButtonImages()

        If Not IsPostBack Then
            Me.PanelUtility.InitToolbar()
            'slokkiamo il paziente lokkato...
            Me.OnitLayout31.lock.EndLock(OnVacUtility.Variabili.PazId)
        End If

    End Sub

#End Region

#Region " Eventi Toolbar "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Me.PanelUtility.ManagingToolbar(e.Button.Key)

    End Sub

#End Region

#Region " Eventi OnitDataPanel "

    Private Sub odpMotivoSospensioneMaster_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpMotivoSospensioneMaster.afterOperation

        Me.OnitLayout31.Busy = Me.PanelUtility.CheckToolBarState(operation)

    End Sub

    Private Sub odpMotivoSospensioneMaster_onCreateQuery(ByRef QB As Object) Handles odpMotivoSospensioneMaster.onCreateQuery

        DirectCast(QB, AbstractQB).AddOrderByFields("MOS_DESCRIZIONE")

    End Sub

    'nel caso in cui si tenti di eliminare un motivo di sospensione già utilizzato (modifica 30/12/2004)
    Private Sub odpMotivoSospensioneMaster_onError(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, err As Onit.Controls.OnitDataPanel.OnitDataPanelError) Handles odpMotivoSospensioneMaster.onError

        If Not err.exc Is Nothing AndAlso err.exc.Message.Contains(Constants.OracleErrors.ORA_02292) Then
            err.exc = New Exception(" (i motivi di sospensione risultano già utilizzati nel programma)")
            err.comment = "Attenzione: non è stato possibile eliminare i motivi di sospensione selezionati!"
        End If

    End Sub

    Private Sub odpMotivoSospensioneDetail_afterSaveData(sender As Onit.Controls.OnitDataPanel.OnitDataPanel) Handles odpMotivoSospensioneDetail.afterSaveData

        Me.odpMotivoSospensioneMaster.Find()

    End Sub

    Private Sub odpMotivoSospensioneDetail_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpMotivoSospensioneDetail.afterOperation

        Me.OnitLayout31.Busy = Me.PanelUtility.CheckToolBarState(operation)

    End Sub

#End Region

End Class
