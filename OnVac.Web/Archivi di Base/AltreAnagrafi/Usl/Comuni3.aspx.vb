Imports Onit.Database.DataAccessManager


Partial Class Comuni3
    Inherits OnVac.Common.PageBase

#Region " Properties "

    ReadOnly Property CodUSL() As String
        Get
            Return Request.QueryString("COD_USL")
        End Get
    End Property

    ReadOnly Property DescUSL() As String
        Get
            Return Request.QueryString("DESC_USL")
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

        If Not IsPostBack Then

            Me.ToolBar.Items.FromKeyButton("btnBack").Enabled = True
            Me.LayoutTitolo.Text = DescUSL

        End If

    End Sub

#End Region

#Region " Eventi Toolbar "

    Private Sub ToolBar_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Dim btnBack As Infragistics.WebUI.UltraWebToolbar.TBarButton = Me.ToolBar.Items.FromKeyButton("btnBack")

        Select Case e.Button.Key

            Case "btnBack"
                Response.Redirect(String.Format("USL.aspx?COD_USL={0}&DESC_USL={1}&FILTRO={2}&RECORD={3}",
                                                HttpUtility.UrlEncode(CodUSL),
                                                "",
                                                HttpUtility.UrlEncode(FiltroRicerca),
                                                HttpUtility.UrlEncode(Record)),
                                            False)

            Case "btnDelete"
                btnBack.Enabled = False

        End Select

    End Sub

#End Region

#Region " Eventi OnitDataPanel "

    Private Sub PanComuni3_afterSaveData(sender As Onit.Controls.OnitDataPanel.OnitDataPanel) Handles PanComuni3.afterSaveData
        Me.PanComuni3.MoveRecord(Onit.Controls.OnitDataPanel.OnitDataPanel.RecordMoveTo.NextRecord, Me.PanComuni3.CurrentRecord())
    End Sub

    Private Sub PanComuni3_onError(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, err As Onit.Controls.OnitDataPanel.OnitDataPanelError) Handles PanComuni3.onError

        If err.exc.Message.Split(":")(0) = Constants.OracleErrors.ORA_00001 Then
            err.generateError("Il codice inserito è già utilizzato: impossibile eseguire il salvataggio!!")
        End If

    End Sub

    Private Sub PanComuni3_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles PanComuni3.afterOperation
        '--
        Select Case operation
            '--
            Case Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord
                If Me.DescUSL = "System.DBNull" Then
                    Me.WzFmUSL.Descrizione = "Descrizione non disponibile"
                Else
                    Me.WzFmUSL.Descrizione = DescUSL
                End If
                Me.WzFmUSL.Codice = CodUSL
                '--
            Case Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.SaveRecord, Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.CancelRecord
                dgrComuni3.needSelPostBack = Onit.Controls.OnitDataPanel.wzDataGrid.needPosts.False
                '--
        End Select
        '--
        Me.OnitLayout31.Busy = Me.GetLayoutState(operation, Me.PanComuni3.CurrentOperation)
        Me.ToolBar.Items.FromKeyButton("btnBack").Enabled = Not Me.OnitLayout31.Busy
        '--
    End Sub

#End Region

#Region " Private "

    Private Function GetLayoutState(operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes, currentState As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) As Boolean

        Select Case operation

            Case Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.CancelRecord,
                 Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.SaveRecord,
                 Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.Error,
                 Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.Find,
                 Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.None,
                 Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.MoveRecord

                If currentState = Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.EditRecord Then
                    Return True
                Else
                    Return False
                End If

            Case Else

                Return True

        End Select

    End Function

#End Region

End Class
