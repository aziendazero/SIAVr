Imports Onit.Controls


Partial Class OnVac_Risposte
    Inherits OnVac.Common.PageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
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

        Me.PanelUtility = New OnitDataPanelUtility(Me.ToolBar)
        Me.PanelUtility.FindButtonName = "btnCerca"
        Me.PanelUtility.DeleteButtonName = "btnElimina"
        Me.PanelUtility.SaveButtonName = "btnSalva"
        Me.PanelUtility.CancelButtonName = "btnAnnulla"
        Me.PanelUtility.MasterDataPanel = Me.odpRisposte
        Me.PanelUtility.WZDataGrid = Me.dgrRisposte
        Me.PanelUtility.WZRicBase = DirectCast(Me.WzFilter1.FindControl("WzFilterKeyBase"), TextBox)
        Me.PanelUtility.SetToolbarButtonImages()

        If Not IsPostBack Then
            Me.PanelUtility.InitToolbar()
            'slokkiamo il paziente lokkato...
            Me.Onitlayout31.lock.EndLock(OnVacUtility.Variabili.PazId.ToString())
        End If

    End Sub

#End Region

#Region " Eventi Toolbar "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked
        Me.PanelUtility.ManagingToolbar(e.Button.Key)
    End Sub

#End Region

#Region " Eventi OnitDataPanel e PanelUtility "

    Private Sub PanelUtility_BeforeSave(sender As Object, e As OnitDataPanelUtility.BeforeOperationEventArgs)

        If Me.odpRisposte.CurrentOperation = Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord Then
            '--
            Me.txtCodice.Text = Me.txtCodice.Text.Trim()
            Dim result As CheckResult = Me.CheckCampoCodice(Me.txtCodice.Text)
            '--
            If Not result.Success Then
                e.Cancel = True
                Me.Onitlayout31.ShowMsgBox(New PagesLayout.OnitLayout3.OnitLayoutMsgBox("Salvataggio non effettuato.\n" + result.Message, "CodErr", False, False))
                Return
            End If
            '--
        End If

    End Sub

    Private Sub OdpRisposte_afterSaveData(sender As Onit.Controls.OnitDataPanel.OnitDataPanel) Handles odpRisposte.afterSaveData
        Me.odpRisposte.Find()
    End Sub

    Private Sub OdpRisposte_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpRisposte.afterOperation
        Me.Onitlayout31.Busy = Me.PanelUtility.CheckToolBarState(operation)
    End Sub

    Private Sub odpRisposte_onCreateQuery(ByRef QB As Object) Handles odpRisposte.onCreateQuery
        DirectCast(QB, AbstractQB).AddOrderByFields("ris_codice")
    End Sub

    'nel caso in cui sia impossibile eliminare una risposta già utilizzata (modifica 30/12/2004)
    Private Sub odpRisposte_onError(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, err As Onit.Controls.OnitDataPanel.OnitDataPanelError) Handles odpRisposte.onError

        If Not err.exc Is Nothing AndAlso err.exc.Message.Contains(Constants.OracleErrors.ORA_02292) Then
            err.exc = New Exception(" (le risposte risultano già utilizzate nel programma)")
            err.comment = "Attenzione: non è stato possibile eliminare le risposte selezionate!"
        End If

    End Sub

#End Region

End Class
