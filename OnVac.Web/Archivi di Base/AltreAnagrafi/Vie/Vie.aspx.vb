Imports Onit.Controls.OnitDataPanel
Imports Onit.Controls.PagesLayout


Partial Class OnVac_Vie
    Inherits OnVac.Common.PageBase

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

#Region " Constants "

    Private Const LAYOUTKEY_WARNINGMESSAGE_SAVE As String = "LAYOUTKEY_WARNINGMESSAGE_SAVE"

#End Region

#Region " Private members "

    Private PanelUtility As OnitDataPanelUtility

#End Region

#Region " Properties "

    Public ReadOnly Property Connessione() As String
        Get
            Return OnVacContext.Connection.ConnectionString
        End Get
    End Property

#End Region

#Region " Page Events "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        Me.PanelUtility = New OnitDataPanelUtility(ToolBar)

        AddHandler PanelUtility.BeforeCopy, AddressOf PanelUtility_BeforeCopy
        AddHandler PanelUtility.BeforeSave, AddressOf PanelUtility_BeforeSave
        AddHandler PanelUtility.BeforeNew, AddressOf PanelUtility_BeforeNew

        Me.PanelUtility.FindButtonName = "btnCerca"
        Me.PanelUtility.DeleteButtonName = "btnElimina"
        Me.PanelUtility.SaveButtonName = "btnSalva"
        Me.PanelUtility.CancelButtonName = "btnAnnulla"
        Me.PanelUtility.MasterDataPanel = odpVie
        Me.PanelUtility.WZDataGrid = dgrVie
        Me.PanelUtility.WZRicBase = filFiltro.FindControl("WzFilterKeyBase")
        Me.PanelUtility.SetToolbarButtonImages()

        If Not IsPostBack() Then
            Me.PanelUtility.InitToolbar()
            'slokkiamo il paziente lokkato...
            Me.Onitlayout31.lock.EndLock(OnVacUtility.Variabili.PazId)
        End If

    End Sub

#End Region

#Region " Toolbar Events "

    Private Sub ToolBar_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked
        Me.PanelUtility.ManagingToolbar(be.Button.Key)
    End Sub

#End Region

#Region " OnitDataPanel Events "

    Private Sub odpVie_afterOperation(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, operation As Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpVie.afterOperation
        Me.Onitlayout31.Busy = Me.PanelUtility.CheckToolBarState(operation)
    End Sub

    Private Sub odpVie_afterSaveData(sender As Onit.Controls.OnitDataPanel.OnitDataPanel) Handles odpVie.afterSaveData
        Me.odpVie.Find()
    End Sub

    Private Sub odpVie_onCreateQuery(ByRef QB As Object) Handles odpVie.onCreateQuery
        DirectCast(QB, AbstractQB).AddOrderByFields("via_descrizione")
    End Sub

#End Region

#Region " PanelUtility Events "

    Private Sub PanelUtility_BeforeNew(sender As Object, e As OnitDataPanelUtility.BeforeOperationEventArgs)

        Me.dgrVie.needSelPostBack = wzDataGrid.needPosts.lock

        Me.odpVie.NewRecord(False)

        If Not WzDropDownList1.Items.FindByValue(Constants.TipoNumeroCivicoVia.Tutti) Is Nothing Then
            Me.WzDropDownList1.SelectedValue = Constants.TipoNumeroCivicoVia.Tutti
        End If

        e.Cancel = True

    End Sub

    Private Sub PanelUtility_BeforeCopy(sender As Object, e As OnitDataPanelUtility.BeforeOperationEventArgs)

        Me.odpVie.NewRecord(True)

        Me.chkDefault.Checked = False

        e.Cancel = True

    End Sub

    Private Sub PanelUtility_BeforeSave(sender As Object, e As OnitDataPanelUtility.BeforeOperationEventArgs)

        If Me.odpVie.CurrentOperation <> OnitDataPanel.CurrentOperationTypes.DeleteRecord Then

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                Using bizVia As New Biz.BizVia(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                    Dim warningMessage As New System.Text.StringBuilder()

                    Dim progressivoVia As Integer = 0

                    Dim objProgressivo As Object = Me.odpVie.getCurrentDataRow("VIA_PROGRESSIVO")

                    If Not objProgressivo Is Nothing AndAlso Not objProgressivo Is DBNull.Value Then
                        Try
                            progressivoVia = Convert.ToInt32(objProgressivo)
                        Catch ex As Exception
                            progressivoVia = 0
                        End Try
                    End If

                    ' Controllo non bloccante sul flag di default: se ci sono altri record, relativi al codice via e al comune correnti,
                    ' già impostati come default, chiede all'utente se vuole continuare con il salvataggio
                    If Me.chkDefault.Checked AndAlso bizVia.ExistsDefault(Me.WzTextBox1.Text, Me.fmComuni.Codice, progressivoVia) Then
                        warningMessage.AppendFormat(" - E' già presente una via marcata come default, diversa da quella corrente, relativamente al codice della via e al comune specificati.{0}", Environment.NewLine)
                    End If

                    ' Controllo non bloccante sulla descrizione: se ci sono altre descrizioni, diverse da quella corrente, 
                    ' relative allo stesso codice via e allo stesso comune, chiede all'utente se vuole continuare con il salvataggio
                    If bizVia.ExistsDescrizioneDiversa(Me.WzTextBox1.Text, Me.fmComuni.Codice, Me.WzTextBox2.Text, progressivoVia) Then
                        warningMessage.AppendFormat(" - E' già presente almeno una descrizione, diversa da quella corrente, relativamente al codice della via e al comune specificati.{0}", Environment.NewLine)
                    End If

                    If warningMessage.Length > 0 Then

                        Me.ShowWarningMessage(String.Format("ATTENZIONE: {0}{1}Continuare con il salvataggio?", Environment.NewLine, warningMessage.ToString()), LAYOUTKEY_WARNINGMESSAGE_SAVE)
                        e.Cancel = True

                    End If

                End Using

            End Using

        End If

    End Sub

#End Region

#Region " OnitLayout Events "

    Private Sub Onitlayout31_ConfirmClick(sender As Object, e As Controls.PagesLayout.OnitLayout3.ConfirmEventArgs) Handles Onitlayout31.ConfirmClick

        Select Case e.Key

            Case LAYOUTKEY_WARNINGMESSAGE_SAVE

                If e.Result Then
                    Me.dgrVie.needSelPostBack = wzDataGrid.needPosts.NotSet
                    Me.odpVie.SaveData()
                End If

        End Select

    End Sub

#End Region

#Region " Private Methods "

    Private Sub ShowWarningMessage(message As String, key As String)

        If String.IsNullOrEmpty(message) Then Return

        Dim msgKey As String = key

        If String.IsNullOrEmpty(key) Then

            Dim rnd As New Random()
            msgKey = String.Format("msg{0}", rnd.Next(0, 255).ToString())

        End If

        Me.Onitlayout31.ShowMsgBox(New OnitLayout3.OnitLayoutMsgBox(Me.ApplyEscapeJS(message), msgKey, True, True))

    End Sub

#End Region

End Class
