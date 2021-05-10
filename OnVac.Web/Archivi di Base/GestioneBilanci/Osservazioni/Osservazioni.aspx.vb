Imports Onit.Controls


Partial Class OnVac_Osservazioni
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

#Region " Proeperties "

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
        Me.PanelUtility.OtherButtonsNames = "btnLinkRisposte"
        Me.PanelUtility.MasterDataPanel = Me.odpOsservazioni
        Me.PanelUtility.WZDataGrid = Me.dgrOsservazioni
        Me.PanelUtility.WZRicBase = Me.filFiltro.FindControl("WzFilterKeyBase")
        Me.PanelUtility.SetToolbarButtonImages()

        AddHandler Me.PanelUtility.BeforeSave, AddressOf PanelUtility_BeforeSave

        If Not IsPostBack() Then
            Me.PanelUtility.InitToolbar()
            'slokkiamo il paziente lokkato...
            Me.Onitlayout31.lock.EndLock(OnVacUtility.Variabili.PazId)
        End If

    End Sub

#End Region

#Region " Eventi Toolbar "

    Private Sub ToolBar_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Me.PanelUtility.ManagingToolbar(e.Button.Key)

        Select Case e.Button.Key
            Case "btnLinkRisposte"
                Response.Redirect(String.Format("Osservazioni-Risposte.aspx?CODICE={0}|{1}",
                                                HttpUtility.UrlEncode(txtCodice.Text),
                                                HttpUtility.UrlEncode(txtDescrizione.Text)),
                                                False)
        End Select

    End Sub

#End Region

#Region " Eventi OnitDataPanel e PanelUtility "

    Private Sub PanelUtility_BeforeSave(sender As Object, e As OnitDataPanelUtility.BeforeOperationEventArgs)

        If Me.odpOsservazioni.CurrentOperation = Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord Then
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

    Private Sub odpOsservazioni_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpOsservazioni.afterOperation
        Me.Onitlayout31.Busy = Me.PanelUtility.CheckToolBarState(operation)
    End Sub

    Private Sub odpOsservazioni_afterSaveData(sender As Onit.Controls.OnitDataPanel.OnitDataPanel) Handles odpOsservazioni.afterSaveData
        Me.odpOsservazioni.Find()
    End Sub

    Private Sub odpOsservazioni_onCreateQuery(ByRef QB As System.Object) Handles odpOsservazioni.onCreateQuery
        DirectCast(QB, AbstractQB).AddOrderByFields("oss_codice")
    End Sub

    'nel caso in cui sia impossibile eliminare un'osservazione già utilizzata (modifica 30/12/2004)
    Private Sub odpOsservazioni_onError(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, err As Onit.Controls.OnitDataPanel.OnitDataPanelError) Handles odpOsservazioni.onError

        If Not err.exc Is Nothing AndAlso err.exc.Message.Contains(Constants.OracleErrors.ORA_02292) Then
            err.exc = New Exception(" (le osservazioni risultano già utilizzate nel programma)")
            err.comment = "Attenzione: non è stato possibile eliminare le osservazioni selezionate!"
        End If

    End Sub

#End Region

End Class
