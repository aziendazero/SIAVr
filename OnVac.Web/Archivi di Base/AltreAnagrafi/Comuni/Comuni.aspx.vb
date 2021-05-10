Imports Onit.Controls


Partial Class OnVac_Comuni
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

    Private PanelUtility As OnitDataPanelUtility

#End Region

#Region " Properties "

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
        PanelUtility.SaveButtonName = "btnSalva"
        PanelUtility.DeleteButtonName = "btnElimina"
        PanelUtility.CancelButtonName = "btnAnnulla"
        PanelUtility.EditButtonName = "btnEdit"
        PanelUtility.NewButtonName = "btnNew"
        PanelUtility.MasterDataPanel = odpComuni
        PanelUtility.WZMsDataGrid = dgrComuni
        PanelUtility.WZRicBase = WzFilter1.FindControl("WzFilterKeyBase")
        PanelUtility.SetToolbarButtonImages()

        If Not IsPostBack Then
            PanelUtility.InitToolbar()
        End If

    End Sub

#End Region

#Region " Eventi Toolbar "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked
        PanelUtility.ManagingToolbar(e.Button.Key)
    End Sub

#End Region

#Region " Eventi OnitDataPanel "

    Private Sub odpComuni_afterSaveData(sender As Onit.Controls.OnitDataPanel.OnitDataPanel) Handles odpComuni.afterSaveData
        Me.odpComuni.Find()
    End Sub

    Private Sub odpComuni_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpComuni.afterOperation
        '--
        Me.OnitLayout31.Busy = Me.PanelUtility.CheckToolBarState(operation)
        '--
        If Me.odpComuni.getCurrentDataRow("COM_OBSOLETO").ToString() = String.Empty OrElse Me.odpComuni.getCurrentDataRow("COM_OBSOLETO").ToString() = "N" Then
            Me.chkObsoleto.Checked = False
        Else
            Me.chkObsoleto.Checked = True
        End If
        '--
        If Me.odpComuni.getCurrentDataRow("COM_SCADENZA").ToString() = String.Empty OrElse Me.odpComuni.getCurrentDataRow("COM_SCADENZA").ToString() = "N" Then
            Me.chkScaduto.Checked = False
        Else
            Me.chkScaduto.Checked = True
        End If
        '--
        Select Case operation
            Case OnitDataPanel.OnitDataPanel.CurrentOperationTypes.EditRecord, OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord
                Me.chkObsoleto.Enabled = True
                Me.chkScaduto.Enabled = True
            Case Else
                Me.chkObsoleto.Enabled = False
                Me.chkScaduto.Enabled = False
        End Select
        '--
    End Sub

    Private Sub odpComuni_onCreateQuery(ByRef QB As System.Object) Handles odpComuni.onCreateQuery
        DirectCast(QB, AbstractQB).AddOrderByFields("COM_CODICE")
    End Sub

#End Region

End Class
