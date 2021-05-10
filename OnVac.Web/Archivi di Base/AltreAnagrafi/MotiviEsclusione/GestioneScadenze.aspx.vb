
Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.Web.UI.WebControls.Validators

Partial Class GestioneScadenze
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

    Public Property CodiceEsclusione() As String
        Get
            Return ViewState("CodiceEsclusione")
        End Get
        Set(ByVal value As String)
            ViewState("CodiceEsclusione") = value
        End Set
    End Property


    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            CodiceEsclusione = Request.QueryString("moe_codice")
            Me.BindGrid(Me.GetDataSource())


        End If

    End Sub

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btnNew"

                If (dgrGestioneScadenze.EditItemIndex = -1) Then

                    Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                        Dim s As MotivoEsclusione = genericProvider.MotiviEsclusione.GetMotivoEsclusione(CodiceEsclusione)
                        If s Is Nothing Then
                            Throw New ApplicationException("Non è possibile avere più di un motivo di esclusione contrassegnato come motivo di default per le inadempienze!!")
                        End If

                        If s.CalcoloScadenza = Enumerators.MotiviEsclusioneCalcoloScadenza.Registrazione AndAlso s.Scadenze.Count > 0 Then
                            Me.RegisterStartupScriptCustom("ToolBar_ButtonClicked2", "alert('Con il calcolo della scadenza impostato su REGISTRAZIONE è possibile inserire una sola scadenza!');")
                        Else
                            Dim dt As DataTable = Me.GetDataSource()
                            Dim drow As DataRow = dt.NewRow

                            drow("mos_mesi") = 0
                            drow("mos_anni") = 0
                            dt.Rows.InsertAt(drow, 0)

                            dgrGestioneScadenze.EditItemIndex = 0

                            Me.ToolBar.Enabled = False
                            Me.BindGrid(dt)
                        End If

                    End Using

                Else
                    Me.RegisterStartupScriptCustom("ToolBar_ButtonClicked1", "alert('Cliccare AGGIORNA O ANNULLA della riga che si sta editando prima di selezionare questa riga!');")
                End If

            Case "btnIndietro"
                Me.Response.Redirect("./MotiviEsclusione.aspx")

        End Select

    End Sub

    Private Sub BindGrid(dt As DataTable)

        dgrGestioneScadenze.DataSource = dt
        dgrGestioneScadenze.DataBind()

    End Sub

    Private Function GetDataSource() As DataTable

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Return genericProvider.MotiviEsclusione.GetScadenzeEsclusioneDt(CodiceEsclusione)

        End Using

    End Function

    Private Sub dgrGestioneScadenze_CancelCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgrGestioneScadenze.CancelCommand

        If Not IsNothing(Me.dgrGestioneScadenze) AndAlso Me.dgrGestioneScadenze.Items.Count > 0 Then

            Me.dgrGestioneScadenze.EditItemIndex = -1
            Me.ToolBar.Enabled = True
            Me.BindGrid(Me.GetDataSource())

        End If

    End Sub

    Private Sub dgrGestioneScadenze_DeleteCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgrGestioneScadenze.DeleteCommand

        If Not IsNothing(Me.dgrGestioneScadenze) AndAlso Me.dgrGestioneScadenze.Items.Count > 0 Then

            ' Controlli preventivi
            If (dgrGestioneScadenze.EditItemIndex <> -1) Then
                Me.RegisterStartupScriptCustom("dgrGestioneScadenze_DeleteCommand", "alert('Cliccare AGGIORNA O ANNULLA della riga che si sta editando prima di cancellare questa riga');")
                Return
            End If

            Dim dgrItem As DataGridItem = Me.dgrGestioneScadenze.Items(e.Item.ItemIndex)

            ' unbind del datagrid
            Dim strId As String = DirectCast(dgrItem.FindControl("hidKey"), HiddenField).Value

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                ' delete
                If Not IsNumeric(strId) Then Throw New ApplicationException("Il campo chiave MOS_ID non è un valore mumerico")
                genericProvider.MotiviEsclusione.DeleteScadenzaEsclusione(Integer.Parse(strId))

            End Using

            Me.dgrGestioneScadenze.EditItemIndex = -1
            Me.ToolBar.Enabled = True
            Me.BindGrid(Me.GetDataSource())

        End If

    End Sub

    Private Sub dgrGestioneScadenze_EditCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgrGestioneScadenze.EditCommand

        If Not IsNothing(Me.dgrGestioneScadenze) AndAlso Me.dgrGestioneScadenze.Items.Count > 0 Then

            ' Controlli preventivi
            If (Me.dgrGestioneScadenze.EditItemIndex <> -1) Then
                Me.RegisterStartupScriptCustom("dgrGestioneScadenze_DeleteCommand", "alert('Cliccare AGGIORNA O ANNULLA della riga che si sta editando prima di editare questa riga');")
                Return
            End If

            Me.dgrGestioneScadenze.EditItemIndex = e.Item.ItemIndex
            Me.ToolBar.Enabled = False
            Me.BindGrid(Me.GetDataSource())

        End If

    End Sub

    Private Sub dgrGestioneScadenze_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles dgrGestioneScadenze.SelectedIndexChanged

        If Not IsNothing(Me.dgrGestioneScadenze) AndAlso Me.dgrGestioneScadenze.Items.Count > 0 Then

            ' Controlli preventivi
            If OnitLayout31.Busy Then

                Me.RegisterStartupScriptCustom("dgrGestioneScadenze_SelectedIndexChanged1", "alert('I dati non sono stati salvati. E\' possibile che le modifiche non siano visibili. Si consiglia di salvare prima di inserire la reazione avversa.');")
                Me.dgrGestioneScadenze.SelectedIndex = -1
                Return

            End If

            If (Me.dgrGestioneScadenze.EditItemIndex <> -1) Then

                Me.RegisterStartupScriptCustom("dgrGestioneScadenze_SelectedIndexChanged2", "alert('Cliccare AGGIORNA O ANNULLA della riga che si sta editando prima di selezionare questa riga!');")
                Me.dgrGestioneScadenze.SelectedIndex = -1
                Return

            End If

        End If

    End Sub

    Private Sub dgrGestioneScadenze_UpdateCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgrGestioneScadenze.UpdateCommand

        If Not IsNothing(Me.dgrGestioneScadenze) AndAlso Me.dgrGestioneScadenze.Items.Count > 0 Then

            Dim dgrItem As DataGridItem = Me.dgrGestioneScadenze.Items(e.Item.ItemIndex)

            ' unbind del datagrid
            Dim strId As String = DirectCast(dgrItem.FindControl("hidKey"), HiddenField).Value
            Dim strMesi As String = DirectCast(dgrItem.FindControl("txtMesi"), OnitJsValidator).Text
            Dim strAnni As String = DirectCast(dgrItem.FindControl("txtAnni"), OnitJsValidator).Text
            Dim strCodiciVaccinazioni As String = DirectCast(dgrItem.FindControl("ucSelezioneVaccinazioniEdit"), SelezioneVaccinazioni).CodiciSelezionati

            ' controllo sulla selezione di almeno una vaccinazione per la scadenza
            If strCodiciVaccinazioni = String.Empty Then
                Me.RegisterStartupScriptCustom("dgrGestioneScadenze_UpdateCommand", "alert('Si deve associare almeno una vaccinazione su questa scadenza!');")
                Return
            End If

            ' controllo sulla valorizzazione di almeno un campo
            If strMesi = "0" AndAlso strAnni = "0" Then
                Me.RegisterStartupScriptCustom("dgrGestioneScadenze_UpdateCommand", "alert('Il campo anni o il campo mesi non possono essere entrambi 0!');")
                Return
            End If

            ' controlli sui dati già effuttuati lato js
            If Not IsNumeric(strMesi) Then Throw New ApplicationException("Il campo mesi non è un numerico")
            If Not IsNumeric(strAnni) Then Throw New ApplicationException("Il campo anni non è un numerico")

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                If String.IsNullOrEmpty(strId) Then
                    ' insert
                    genericProvider.MotiviEsclusione.InsertScadenzaEsclusione(CodiceEsclusione, Integer.Parse(strMesi), Integer.Parse(strAnni), strCodiciVaccinazioni)
                Else
                    ' update
                    If Not IsNumeric(strId) Then Throw New ApplicationException("Il campo chiave MOS_ID non è un valore mumerico")
                    genericProvider.MotiviEsclusione.UpdateScadenzaEsclusione(Integer.Parse(strId), Integer.Parse(strMesi), Integer.Parse(strAnni), strCodiciVaccinazioni)
                End If

            End Using

            Me.dgrGestioneScadenze.EditItemIndex = -1
            Me.ToolBar.Enabled = True
            Me.BindGrid(Me.GetDataSource())

        End If

    End Sub

    Private Sub dgrGestioneScadenze_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgrGestioneScadenze.ItemDataBound

    End Sub

End Class
