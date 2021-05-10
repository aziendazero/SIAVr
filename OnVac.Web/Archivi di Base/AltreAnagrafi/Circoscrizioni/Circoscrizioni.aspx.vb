
Imports Onit.Controls

Partial Class OnVac_Circoscrizioni
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

        Me.PanelUtility = New OnitDataPanelUtility(ToolBar)
        Me.PanelUtility.FindButtonName = "btnCerca"
        Me.PanelUtility.SaveButtonName = "btnSalva"
        Me.PanelUtility.DeleteButtonName = "btnElimina"
        Me.PanelUtility.CancelButtonName = "btnAnnulla"
        Me.PanelUtility.EditButtonName = "btnEdit"
        Me.PanelUtility.NewButtonName = "btnNew"
        Me.PanelUtility.MasterDataPanel = Me.odpCircoscrizioniMaster
        Me.PanelUtility.DetailDataPanel = Me.odpCircoscrizioniDetail
        Me.PanelUtility.WZDataGrid = Me.dgrCircoscrizioni
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

    Private Sub odpCircoscrizioniMaster_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpCircoscrizioniMaster.afterOperation

        Me.OnitLayout31.Busy = Me.PanelUtility.CheckToolBarState(operation)

    End Sub

    Private Sub odpCircoscrizioniMaster_onCreateQuery(ByRef QB As System.Object) Handles odpCircoscrizioniMaster.onCreateQuery

        DirectCast(QB, AbstractQB).AddOrderByFields("CIR_DESCRIZIONE")

    End Sub

    Private Sub odpCircoscrizioniDetail_afterSaveData(sender As Onit.Controls.OnitDataPanel.OnitDataPanel) Handles odpCircoscrizioniDetail.afterSaveData

        Me.odpCircoscrizioniMaster.Find()

    End Sub

    Private Sub odpCircoscrizioniDetail_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpCircoscrizioniDetail.afterOperation
        '--
        Me.OnitLayout31.Busy = Me.PanelUtility.CheckToolBarState(operation)
        '--
    End Sub

#End Region

End Class
