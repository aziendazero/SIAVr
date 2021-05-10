Imports Onit.Database.DataAccessManager


Partial Class USL
    Inherits OnVac.Common.PageBase

#Region " Contants "

    Private Const SAME_COD_SAME_VAL_WARNING_KEY As String = "SameCodSameVal"

#End Region

#Region " Properties "

    ReadOnly Property CodUSL() As String
        Get
            Return Request.QueryString("COD_USL")
        End Get
    End Property

    ReadOnly Property FiltroRicerca() As String
        Get
            Return Request.QueryString("FILTRO")
        End Get
    End Property

    ReadOnly Property Record() As String
        Get
            Return Request.QueryString("RECORD")
        End Get
    End Property

    Public ReadOnly Property Connessione() As String
        Get
            Return OnVacContext.Connection.ConnectionString
        End Get
    End Property

#End Region

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

#Region " Eventi Page "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        Dim btnLinkComuni As Infragistics.WebUI.UltraWebToolbar.TBarButton = Me.ToolBar.Items.FromKeyButton("btnLinkComuni")

        If Not IsPostBack Then

            If Not String.IsNullOrEmpty(Me.FiltroRicerca) Then Me.WzFilterKeyBase.Text = Me.FiltroRicerca

            Me.PanUSL.Find()

            If Not Me.Record Is Nothing Then
                Me.PanUSL.MoveRecord(Onit.Controls.OnitDataPanel.OnitDataPanel.RecordMoveTo.NextRecord, Me.Record)
                btnLinkComuni.Enabled = Me.dgrUSL.Rows.Count > 0
            Else
                btnLinkComuni.Enabled = True
            End If

        End If
        '--
    End Sub

#End Region

#Region " Eventi Toolbar "

    Private Sub ToolBar_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btnLinkComuni"
                Me.Response.Redirect(String.Format("Comuni3.aspx?COD_USL={0}&DESC_USL={1}&FILTRO={2}&RECORD={3}",
                                                   HttpUtility.UrlEncode(Me.tbCodice.Text),
                                                   HttpUtility.UrlEncode(Me.tbDesc.Text),
                                                   HttpUtility.UrlEncode(Me.WzFilterKeyBase.Text),
                                                   HttpUtility.UrlEncode(Me.PanUSL.CurrentRecord())),
                                               False)
        End Select
    End Sub

#End Region

#Region " Eventi OnitDataPanel "

    Private Sub PanUSL_onError(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, err As Onit.Controls.OnitDataPanel.OnitDataPanelError) Handles PanUSL.onError

        If err.exc.Message.Split(":")(0) = Constants.OracleErrors.ORA_00001 Then
            err.generateError("Il codice inserito è già utilizzato: impossibile eseguire il salvataggio!!")
        End If

    End Sub

    Private Sub PanUSL_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles PanUSL.afterOperation
        '--
        Dim btnLinkComuni As Infragistics.WebUI.UltraWebToolbar.TBarButton = Me.ToolBar.Items.FromKeyButton("btnLinkComuni")
        '--
        Select Case operation
            '--
            Case Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.EditRecord
                Me.dgrUSL.needSelPostBack = Onit.Controls.OnitDataPanel.wzDataGrid.needPosts.lock
                btnLinkComuni.Enabled = False
                '--
            Case Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord
                btnLinkComuni.Enabled = False
                '--
            Case Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.SaveRecord, Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.CancelRecord
                Me.dgrUSL.needSelPostBack = Onit.Controls.OnitDataPanel.wzDataGrid.needPosts.True
                If Me.dgrUSL.Rows.Count > 0 Then
                    btnLinkComuni.Enabled = True
                End If
                '--
            Case Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.Find
                If Me.dgrUSL.Rows.Count > 0 Then
                    btnLinkComuni.Enabled = True
                Else
                    btnLinkComuni.Enabled = False
                End If
                '--
        End Select
        '--
        Me.OnitLayout31.Busy = Me.GetLayoutState(operation, PanUSL.CurrentOperation)
        '--
    End Sub

#End Region

#Region " Eventi OnitLayout "

    Private Sub OnitLayout31_ConfirmClick(sender As Object, e As Onit.Controls.PagesLayout.OnitLayout3.ConfirmEventArgs) Handles OnitLayout31.ConfirmClick
        Select Case e.Key
            Case SAME_COD_SAME_VAL_WARNING_KEY
                If e.Result Then
                    SaveData()
                End If
        End Select
    End Sub

#End Region

#Region " Private "

    Private Sub SaveData()
        Me.PanUSL.SaveData(True)
        Me.PanUSL.MoveRecord(Onit.Controls.OnitDataPanel.OnitDataPanel.RecordMoveTo.NextRecord, Me.PanUSL.CurrentRecord())
    End Sub

    Private Function GetLayoutState(operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes, currentState As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) As Boolean
        '--
        Select Case operation
            '--
            Case Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.CancelRecord,
                 Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.SaveRecord,
                 Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.Error,
                 Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.Find,
                 Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.None,
                 Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.MoveRecord
                '--
                If currentState = Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.EditRecord Then
                    Return True
                Else
                    Return False
                End If
                '--
            Case Else
                '--
                Return True
                '--
        End Select
        '--
    End Function

#End Region

End Class
