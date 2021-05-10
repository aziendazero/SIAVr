Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager
Imports Onit.Controls
Imports Onit.OnAssistnet.OnVac.Entities

Partial Class OnVac_Malattie
    Inherits OnVac.Common.PageBase

    Protected WithEvents cmbFlagVisita As Onit.Controls.OnitDataPanel.wzDropDownList

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

    Private _saving As Boolean

#End Region

#Region " Properties "

    Public ReadOnly Property Connessione() As String
        Get
            Return OnVacContext.Connection.ConnectionString
        End Get
    End Property

    Private _PanelUtility As OnitDataPanelUtility
    Public Property PanelUtility() As OnitDataPanelUtility
        Get
            Return _PanelUtility
        End Get
        Set(Value As OnitDataPanelUtility)
            _PanelUtility = Value
        End Set
    End Property

#End Region

#Region " Eventi Page "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        Me.PanelUtility = New OnitDataPanelUtility(Me.ToolBar)
        Me.PanelUtility.FindButtonName = "btnCerca"
        Me.PanelUtility.DeleteButtonName = "btnElimina"
        Me.PanelUtility.SaveButtonName = "btnSalva"
        Me.PanelUtility.CancelButtonName = "btnAnnulla"
        Me.PanelUtility.OtherButtonsNames = "btnVaccinazioni"
        Me.PanelUtility.MasterDataPanel = Me.odpMalattieMaster
        Me.PanelUtility.WZDataGrid = Me.dgrMalattie
        Me.PanelUtility.WZRicBase = Me.filFiltro.FindControl("WzFilterKeyBase")
        Me.PanelUtility.SetToolbarButtonImages()

        AddHandler Me.PanelUtility.BeforeSave, AddressOf PanelUtility_BeforeSave

        If Not IsPostBack() Then

            Me.PanelUtility.InitToolbar()
            'slokkiamo il paziente lokkato...
            Me.Onitlayout31.lock.EndLock(OnVacUtility.Variabili.PazId)

            ' load delle tipologie malattia
            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Me.ddlTipologia.DataSource = genericProvider.Malattie.GetTipologiaMalattie()
                Me.ddlTipologia.DataBind()
            End Using

        End If

    End Sub

#End Region

#Region " Eventi Toolbar "

    Private Sub ToolBar_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btnSalva"

                Me.dgrMalattie.needSelPostBack = Onit.Controls.OnitDataPanel.wzDataGrid.needPosts.NotSet
                If Me.odpMalattieMaster.CurrentOperation = Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.DeleteRecord Then
                    Me.SalvaEliminazioneMalattia()
                Else
                    If Me.SalvaDatiDettaglio() Then
                        Me.dgrMalattie.needSelPostBack = Onit.Controls.OnitDataPanel.wzDataGrid.needPosts.NotSet
                    End If
                End If

            Case "btnNew"

                Me.ddlTipologia.Enabled = True
                Me.PanelUtility.ManagingToolbar(e.Button.Key)

            Case "btnVaccinazioni"
                RedirectToPage("VaccinazioniMal.aspx")

            Case Else

                Me.PanelUtility.ManagingToolbar(e.Button.Key)

        End Select

    End Sub

#End Region

#Region " Eventi Datagrid "

    'imposta il valore della colonna del flag visita [modifica 21/07/2005]
    Private Sub dgrMalattie_InitializeRow(sender As Object, e As Infragistics.WebUI.UltraWebGrid.RowEventArgs) Handles dgrMalattie.InitializeRow

        If e.Row.Cells.FromKey("MAL_FLAG_VISITA").Text <> Nothing Then
            e.Row.Cells.FromKey("MAL_FLAG_VISITA").Text = IIf(e.Row.Cells.FromKey("MAL_FLAG_VISITA").Text.ToString() = "S", "SI", "NO")
        End If

    End Sub

#End Region

#Region " Eventi OnitDataPanel e PanelUtility "

    Private Sub PanelUtility_BeforeSave(sender As Object, e As OnitDataPanelUtility.BeforeOperationEventArgs)

        If Me.odpMalattieMaster.CurrentOperation = Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord Then
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

    Private Sub odpMalattieMaster_afterOperation(sender As Controls.OnitDataPanel.OnitDataPanel, operation As Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes) Handles odpMalattieMaster.afterOperation
        Me.EnableTipologiaMalattia(operation)
        Me.Onitlayout31.Busy = Me.PanelUtility.CheckToolBarState(operation)
    End Sub

    Private Sub odpMalattieMaster_afterUpdateWzControls(sender As Controls.OnitDataPanel.OnitDataPanel) Handles odpMalattieMaster.afterUpdateWzControls
        If Not _saving Then
            Me.UploadTipologiaMalattia(sender.CurrentOperation)
        End If
    End Sub

    Private Sub odpMalattieMaster_onCreateQuery(ByRef QB As Object) Handles odpMalattieMaster.onCreateQuery
        DirectCast(QB, AbstractQB).AddOrderByFields("mal_descrizione")
    End Sub

    'nel caso in cui sia impossibile eliminare le malattie già utilizzate (modifica 30/12/2004)
    Private Sub odpMalattieMaster_onError(sender As Onit.Controls.OnitDataPanel.OnitDataPanel, err As Onit.Controls.OnitDataPanel.OnitDataPanelError) Handles odpMalattieMaster.onError

        If Not err.exc Is Nothing AndAlso err.exc.Message.Contains(Constants.OracleErrors.ORA_02292) Then
            err.exc = New Exception(" (le malattie risultano già utilizzate nel programma)")
            err.comment = "Attenzione: non è stato possibile eliminare le malattie selezionate!"
        End If

    End Sub

#End Region

#Region " Private Methods "

    Private Sub EnableTipologiaMalattia(operation As Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes)

        ' abilitazione della tipologia
        Select Case operation

            Case Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.EditRecord,
                 Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord

                Me.ddlTipologia.Enabled = True

            Case Else

                Me.ddlTipologia.Enabled = False

        End Select

    End Sub

    Private Sub UploadTipologiaMalattia(operation As Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes)

        ' load della selezione delle tipologie malattia
        Select Case operation

            Case Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.MoveRecord,
                 Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.None

                Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                    Dim m As Malattia = genericProvider.Malattie.LoadMalattia(Me.GetCodiceMalattia())
                    If Not m Is Nothing Then
                        For i As Integer = 0 To Me.ddlTipologia.Items.Count - 1
                            Me.ddlTipologia.Items(i).Checked = m.Tipologia.Select(Function(p) p.Codice).Contains(Me.ddlTipologia.Items(i).Value)
                        Next
                    End If
                End Using

            Case Onit.Controls.OnitDataPanel.OnitDataPanel.CurrentOperationTypes.NewRecord

                Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                    Dim m As List(Of Malattia.TipologiaMalattia) = genericProvider.Malattie.GetTipologiaMalattie()
                    If Not m Is Nothing Then
                        For i As Integer = 0 To Me.ddlTipologia.Items.Count - 1
                            Me.ddlTipologia.Items(i).Checked = m.Where(Function(p) p.FlagDefault).Select(Function(p) p.Codice).Contains(Me.ddlTipologia.Items(i).Value)
                        Next
                    End If
                End Using

        End Select

    End Sub

    Private Function SalvaDatiDettaglio() As Boolean

        Dim saveOk As Boolean = False
        Dim dam As IDAM = Nothing

        Try
            _saving = True
            dam = OnVacUtility.OpenDam()
            dam.BeginTrans()

            ' salvataggio pannello
            If Me.odpMalattieMaster.SaveData(True, True, dam).ok Then

                ' salvataggio delle tipologie malattia
                Using genericProvider As New DAL.DbGenericProvider(dam)
                    Dim codiceMalattia As String = Me.GetCodiceMalattia()
                    genericProvider.Malattie.DeleteTipologiaMalattia(codiceMalattia)
                    genericProvider.Malattie.InsertTipologiaMalattia(codiceMalattia, ddlTipologia.CheckedItems.Select(Function(p) p.Value).ToList())
                End Using

            End If

            dam.Commit()
            saveOk = True

        Catch ex As Exception

            dam.Rollback()
            ex.InternalPreserveStackTrace()
            Throw

        Finally

            _saving = False
            OnVacUtility.CloseDam(dam)

        End Try

        Return saveOk

    End Function

    Private Function SalvaEliminazioneMalattia() As Boolean

        Dim saveOk As Boolean = False
        Dim dam As IDAM = Nothing

        Try
            _saving = True
            dam = OnVacUtility.OpenDam()
            dam.BeginTrans()

            ' salvataggio delle eliminazione delle tipologie malattia
            Using genericProvider As New DAL.DbGenericProvider(dam)
                Dim codiceMalattia As String = Me.GetCodiceMalattia()
                genericProvider.Malattie.DeleteTipologiaMalattia(codiceMalattia)
            End Using

            ' salvataggio pannello
            If Me.odpMalattieMaster.SaveData(True, True, dam).ok Then

                dam.Commit()
                saveOk = True

            End If

        Catch ex As Exception

            dam.Rollback()
            ex.InternalPreserveStackTrace()
            Throw

        Finally

            _saving = False
            OnVacUtility.CloseDam(dam)

        End Try

        Return saveOk

    End Function

    Private Function GetCodiceMalattia()
        Return Me.txtCodice.Text
    End Function

    Private Sub RedirectToPage(pageName As String)
        Me.Response.Redirect(String.Format("{0}?MAL={1}|{2}", pageName, HttpUtility.UrlEncode(Me.txtCodice.Text), HttpUtility.UrlEncode(Me.WzTextBox2.Text)), False)
    End Sub

#End Region

End Class
