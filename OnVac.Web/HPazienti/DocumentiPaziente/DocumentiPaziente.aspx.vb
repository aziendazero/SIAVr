Imports Onit.Database.DataAccessManager

Partial Class DocumentiPaziente
    Inherits Common.PageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Enum "

    Protected Enum DocumentiState
        None
        Edit
        Delete
    End Enum

    Protected Enum IndexColonneDgrDocumenti
        CheckBox = 0
        Id = 1
        Descrizione = 2
        Note = 3
        DataArchiviazione = 4
        UslDescr = 5
        UslCod = 6
        FlagVisibilita = 7
    End Enum

#End Region

#Region " Properties "

    Protected Property State() As DocumentiState
        Get
            Return viewstate("State")
        End Get
        Set(Value As DocumentiState)
            ViewState("State") = Value
        End Set
    End Property

    Protected Property dgrIndex() As Integer
        Get
            Return Session("dgrIndex")
        End Get
        Set(Value As Integer)
            Session("dgrIndex") = Value
        End Set
    End Property

    Private Property IsRowVisible() As Boolean
        Get
            If ViewState("IsRowVisible") Is Nothing Then
                ViewState("IsRowVisible") = True
            End If
            Return ViewState("IsRowVisible")
        End Get
        Set(Value As Boolean)
            ViewState("IsRowVisible") = Value
        End Set
    End Property

    Private Property IsRowEditable() As Boolean
        Get
            If ViewState("IsRowEditable") Is Nothing Then
                ViewState("IsRowEditable") = True
            End If
            Return ViewState("IsRowEditable")
        End Get
        Set(Value As Boolean)
            ViewState("IsRowEditable") = Value
        End Set
    End Property

    Private ReadOnly Property ValoreVisibilitaDefaultPaziente() As String
        Get
            If ViewState("ValoreVisibilitaDefaultPaziente") Is Nothing Then
                ViewState("ValoreVisibilitaDefaultPaziente") = GetValoreVisibilitaDatiVaccinaliDefault(OnVacUtility.Variabili.PazId)
            End If
            Return ViewState("ValoreVisibilitaDefaultPaziente").ToString()
        End Get
    End Property

#End Region

#Region " Page Events "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not Me.IsPostBack Then

            Dim collCodifiche As Collection.CodificheCollection = Nothing

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                ' Impostazione titolo pagina e label dati paziente
                OnVacUtility.ImpostaIntestazioniPagina(Me.OnitLayout31, Me.LayoutTitolo, genericProvider, Me.Settings, Me.IsGestioneCentrale)

                ' Caricamento valori tipo documento
                Using bizCodifiche As New Biz.BizCodifiche(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                    collCodifiche = bizCodifiche.GetCodifiche("DOC_TYPE")

                End Using

            End Using

            ddlDettaglioDescrizioneUpload.DataValueField = "Codice"
            ddlDettaglioDescrizioneUpload.DataTextField = "Descrizione"
            ddlDettaglioDescrizioneUpload.DataSource = collCodifiche
            ddlDettaglioDescrizioneUpload.DataBind()

            ddlDettaglioDescrizioneUpload.Items.Insert(0, New ListItem(String.Empty, String.Empty))
            ddlDettaglioDescrizioneUpload.SelectedIndex = 0

            ' DropDownList dettaglio
            ddlDettaglioDescrizione.DataValueField = "Codice"
            ddlDettaglioDescrizione.DataTextField = "Descrizione"
            ddlDettaglioDescrizione.DataSource = collCodifiche
            ddlDettaglioDescrizione.DataBind()

            ' DropDownList filtri (con aggiunta elemento "TUTTE")
            ddlFiltroDescrizione.DataValueField = "Codice"
            ddlFiltroDescrizione.DataTextField = "Descrizione"
            ddlFiltroDescrizione.DataSource = collCodifiche
            ddlFiltroDescrizione.DataBind()
            ddlFiltroDescrizione.Items.Insert(0, New ListItem("TUTTE", "TT"))
            ddlFiltroDescrizione.SelectedValue = "TT"

            ' Caricamento documenti del paziente
            LoadDocumenti()

            ' Impostazione pulsanti toolbar
            SetStateToolbar()

            ' Impostazione controlli
            SetStateControls()

        End If

    End Sub

#End Region

#Region " Toolbar Events "

    Private Sub Toolbar_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles Toolbar.ButtonClicked

        Select Case be.Button.Key

            Case "btnNew"

                fmUpload.VisibileMD = True
                ' Valorizzazione flag di visibilità per i dati vaccinali del paziente
                chkFlagVisibilita_new.Enabled = True

                ' [Unificazione ULSS]: è stato chiesto di impostare sempre questo flag di consenso a false in inserimento. Dovrebbe essere comandato da un consenso ad hoc e non da quello di adesso.
                chkFlagVisibilita_new.Checked = False
                'chkFlagVisibilita_new.Checked = (ValoreVisibilitaDefaultPaziente = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente)

            Case "btnEdit"

                State = DocumentiState.Edit
                SetStateControls()
                SetStateToolbar()
                'setto il check del consenso
                If String.IsNullOrWhiteSpace(CType(dgrDocumenti.SelectedItem.FindControl("HiddenField_FLAGVISIBILITA"), HiddenField).Value) Then
                    chkFlagVisibilita.Checked = (ValoreVisibilitaDefaultPaziente = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente)
                End If

            Case "btnDelete"

                State = DocumentiState.Delete
                dgrDocumenti.Columns(0).Visible = True
                dgrDocumenti.SelectionOption = Onit.Controls.OnitGrid.OnitGrid.SelectionOptions.none
                SetStateToolbar()

            Case "btnDownload"

                OnitLayout31.InsertRoutineJS(String.Format("window.open(""./DownloadDocumento.aspx?Id={0}"",""Download"",""top=0,left=0,width=700,height=500,menubar=0,resizable=1,scrollbars=1"")",
                                                           dgrDocumenti.SelectedItem.Cells(IndexColonneDgrDocumenti.Id).Text()))

            Case "btnSave"

                Select Case State
                    Case DocumentiState.Edit
                        AggiornaDocumento()
                    Case DocumentiState.Delete
                        EliminaDocumenti()
                End Select
                dgrDocumenti.Columns(0).Visible = False
                dgrDocumenti.SelectionOption = Onit.Controls.OnitGrid.OnitGrid.SelectionOptions.rowClick
                State = DocumentiState.None
                LoadDocumenti()
                SetStateToolbar()
                SetStateControls()

            Case "btnCancel"

                dgrDocumenti.Columns(0).Visible = False
                dgrDocumenti.SelectionOption = Onit.Controls.OnitGrid.OnitGrid.SelectionOptions.rowClick
                State = DocumentiState.None
                LoadDocumenti()
                SetStateToolbar()
                SetStateControls()

        End Select

    End Sub

#End Region

#Region " Datagrid Events "

    Private Sub dgrDocumenti_ItemDataBound(sender As Object, e As DataGridItemEventArgs) Handles dgrDocumenti.ItemDataBound

        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim enableEditDelete As Boolean = False
            Dim isPrimaRiga As Boolean = e.Item.ItemIndex = 0
            If isPrimaRiga Then
                If OnVacContext.CodiceUslCorrente = e.Item.Cells(IndexColonneDgrDocumenti.UslCod).Text Then
                    IsRowVisible = True
                    IsRowEditable = True
                Else
                    If CtrlIsInUslUnificata(e.Item.Cells(IndexColonneDgrDocumenti.UslCod).Text) Then
                        IsRowVisible = True
                        IsRowEditable = True
                    Else
                        IsRowVisible = DirectCast(e.Item.FindControl("HiddenField_FLAGVISIBILITA"), HiddenField).Value = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente
                        IsRowEditable = DirectCast(e.Item.FindControl("HiddenField_FLAGVISIBILITA"), HiddenField).Value = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente
                    End If
                End If
                SetDocumentoDetail(IsRowVisible)
            End If
        End If

    End Sub

    Private Sub dgrDocumenti_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles dgrDocumenti.SelectedIndexChanged

        dgrIndex = dgrDocumenti.SelectedIndex
        LoadDocumento()

        'visibilità del dettaglio
        If OnVacContext.CodiceUslCorrente = dgrDocumenti.SelectedItem.Cells(IndexColonneDgrDocumenti.UslCod).Text Then
            IsRowVisible = True
            IsRowEditable = True
        Else
            If CtrlIsInUslUnificata(dgrDocumenti.SelectedItem.Cells(IndexColonneDgrDocumenti.UslCod).Text) Then
                IsRowVisible = True
                IsRowEditable = True
            Else
                IsRowVisible = DirectCast(dgrDocumenti.SelectedItem.FindControl("HiddenField_FLAGVISIBILITA"), HiddenField).Value = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente
                IsRowEditable = DirectCast(dgrDocumenti.SelectedItem.FindControl("HiddenField_FLAGVISIBILITA"), HiddenField).Value = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente
            End If
        End If
        SetDocumentoDetail(IsRowVisible)

        SetStateToolbar()

    End Sub

    Private Function CtrlIsInUslUnificata(codiceUsl As String) As Boolean

        'verifico se il codice usl è una vecchia usl che fa parte della nuova ulss
        If Not String.IsNullOrWhiteSpace(codiceUsl) Then
            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizUsl As New Biz.BizUsl(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                    If bizUsl.IsInUslUnificata(codiceUsl, OnVacContext.CodiceUslCorrente) Then
                        Return True
                    End If

                End Using
            End Using
        End If
        Return False

    End Function

    Private Sub SetDocumentoDetail(isVisible As Boolean)

        Table_Documento.Visible = isVisible
        Lbl_StatoDetail.Visible = Not isVisible

    End Sub

    Protected Function BindFlagVisibilitaImageUrlValue(dataItem As Object) As String

        Return Common.OnVacStoricoVaccinaleCentralizzato.GetImageUrlFlagVisibilita(dataItem, "PDO_FLAG_VISIBILITA", Me)

    End Function

    Protected Function BindFlagVisibilitaToolTipValue(dataItem As Object) As String

        Return Common.OnVacStoricoVaccinaleCentralizzato.GetToolTipFlagVisibilita(dataItem, "PDO_FLAG_VISIBILITA")

    End Function

#End Region

#Region " DropDownList Events "

    Private Sub ddlFiltroDescrizione_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles ddlFiltroDescrizione.SelectedIndexChanged

        LoadDocumenti()

    End Sub

#End Region

#Region " Public "

    Public Sub LoadDocumento()

        Using dam As IDAM = OnVacUtility.OpenDam()

            dam.QB.NewQuery()
            dam.QB.AddSelectFields("COD_CODICE, PDO_DESCRIZIONE, PDO_NOTE, PDO_FLAG_VISIBILITA")
            dam.QB.AddTables("T_PAZ_DOCUMENTI, T_ANA_CODIFICHE")
            dam.QB.AddWhereCondition("PDO_PAZ_CODICE", Comparatori.Uguale, OnVacUtility.Variabili.PazId, DataTypes.Numero)
            dam.QB.AddWhereCondition("COD_CAMPO", Comparatori.Uguale, "DOC_TYPE", DataTypes.Stringa)
            dam.QB.AddWhereCondition("PDO_DESCRIZIONE", Comparatori.Uguale, "COD_DESCRIZIONE", DataTypes.Join)
            dam.QB.AddWhereCondition("PDO_ID", Comparatori.Uguale, dgrDocumenti.SelectedItem.Cells(IndexColonneDgrDocumenti.Id).Text(), DataTypes.Numero)

            Using reader As IDataReader = dam.BuildDataReader()

                If reader.Read() Then
                    ' deve mostrare nella combo il valore selezionato ...
                    ddlDettaglioDescrizione.SelectedValue = reader("COD_CODICE").ToString()
                    txtNote.Text = reader("PDO_NOTE").ToString()
                    If String.IsNullOrWhiteSpace(reader("PDO_FLAG_VISIBILITA").ToString()) Then
                        chkFlagVisibilita.Checked = False
                    Else
                        chkFlagVisibilita.Checked = (reader("PDO_FLAG_VISIBILITA").ToString() = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente)
                    End If
                End If

            End Using

        End Using

    End Sub

#End Region

#Region " Private "

    Private Sub SetStateControls()

        If State = DocumentiState.Edit Then
            ddlDettaglioDescrizione.Enabled = True
            ddlFiltroDescrizione.Enabled = False
            txtNote.ReadOnly = False
            txtNote.CssClass = "textbox_stringa"
            chkFlagVisibilita.Enabled = True
        Else
            ddlDettaglioDescrizione.Enabled = False
            ddlFiltroDescrizione.Enabled = True
            txtNote.ReadOnly = True
            txtNote.CssClass = "textbox_stringa_disabilitato"
            chkFlagVisibilita.Enabled = False
        End If

    End Sub

    Private Sub SetStateToolbar()

        Toolbar.Items.FromKeyButton("btnDelete").Enabled = State = DocumentiState.None AndAlso dgrDocumenti.Items.Count > 0 AndAlso IsRowVisible AndAlso IsRowEditable
        Toolbar.Items.FromKeyButton("btnEdit").Enabled = State = DocumentiState.None AndAlso dgrDocumenti.Items.Count > 0 AndAlso IsRowVisible AndAlso IsRowEditable
        Toolbar.Items.FromKeyButton("btnNew").Enabled = State = DocumentiState.None
        Toolbar.Items.FromKeyButton("btnSave").Enabled = State <> DocumentiState.None
        Toolbar.Items.FromKeyButton("btnCancel").Enabled = State <> DocumentiState.None
        Toolbar.Items.FromKeyButton("btnDownload").Enabled = State = DocumentiState.None AndAlso dgrDocumenti.Items.Count > 0 AndAlso IsRowVisible

    End Sub

    Private Sub LoadDocumenti()

        Dim dtDocumenti As New DataTable()

        Using dam As IDAM = OnVacUtility.OpenDam()

            dam.QB.NewQuery()
            dam.QB.AddSelectFields("PDO_ID, PDO_DATA_ARCHIVIAZIONE, PDO_DESCRIZIONE, PDO_NOTE, PDO_USL_INSERIMENTO, USL_DESCRIZIONE USL_INSERIMENTO_PDO_DESCR, PDO_FLAG_VISIBILITA")
            dam.QB.AddTables("T_PAZ_DOCUMENTI, T_ANA_USL")
            dam.QB.AddWhereCondition("PDO_PAZ_CODICE", Comparatori.Uguale, OnVacUtility.Variabili.PazId, DataTypes.Numero)
            dam.QB.AddWhereCondition("PDO_USL_INSERIMENTO", Comparatori.Uguale, "USL_CODICE", DataTypes.OutJoinLeft)
            dam.QB.AddOrderByFields("PDO_DATA_ARCHIVIAZIONE DESC, PDO_DESCRIZIONE")

            If State <> DocumentiState.Edit And ddlFiltroDescrizione.SelectedValue <> "TT" Then
                dam.QB.AddWhereCondition("PDO_DESCRIZIONE", Comparatori.Uguale, ddlFiltroDescrizione.SelectedItem.Text, DataTypes.Stringa)
            End If

            dam.BuildDataTable(dtDocumenti)

        End Using

        txtNote.Text = String.Empty
        ddlDettaglioDescrizione.SelectedIndex = -1
        chkFlagVisibilita.Checked = False

        dgrDocumenti.SelectedIndex = -1
        dgrDocumenti.DataSource = dtDocumenti
        dgrDocumenti.DataBind()

        If dgrDocumenti.Items.Count > 0 Then
            If State <> DocumentiState.Edit Then
                dgrDocumenti.SelectedIndex = 0
            Else
                dgrDocumenti.SelectedIndex = dgrIndex
            End If

            LoadDocumento()

        End If

    End Sub

    Private Sub EliminaDocumenti()

        Dim idDocToDelete As New ArrayList()

        For i As Integer = 0 To Me.dgrDocumenti.Items.Count - 1
            If DirectCast(Me.dgrDocumenti.Items(i).FindControl(Onit.Controls.OnitGrid.OnitMultiSelColumn.CheckBoxName), CheckBox).Checked Then
                idDocToDelete.Add(Me.dgrDocumenti.Items(i).Cells(IndexColonneDgrDocumenti.Id).Text())
            End If
        Next

        If idDocToDelete.Count > 0 Then

            Using dam As IDAM = OnVacUtility.OpenDam()

                dam.QB.NewQuery()
                dam.QB.AddTables("T_PAZ_DOCUMENTI")
                dam.QB.AddWhereCondition("PDO_ID", Comparatori.In,
                                         String.Join(",", DirectCast(idDocToDelete.ToArray(GetType(String)), String())),
                                         DataTypes.Replace)

                dam.ExecNonQuery(ExecQueryType.Delete)

            End Using

        End If

    End Sub

    Private Sub AggiornaDocumento()

        Using dam As IDAM = OnVacUtility.OpenDam()

            dam.QB.NewQuery()
            dam.QB.AddTables("T_PAZ_DOCUMENTI")
            dam.QB.AddUpdateField("PDO_DESCRIZIONE", ddlDettaglioDescrizione.SelectedItem.Text, DataTypes.Stringa)
            dam.QB.AddUpdateField("PDO_NOTE", txtNote.Text, DataTypes.Stringa)
            dam.QB.AddUpdateField("PDO_FLAG_VISIBILITA", Common.OnVacStoricoVaccinaleCentralizzato.GetValoreVisibilitaDatiVaccinali(chkFlagVisibilita), DataTypes.Stringa)
            dam.QB.AddWhereCondition("PDO_ID", Comparatori.Uguale, dgrDocumenti.SelectedItem.Cells(IndexColonneDgrDocumenti.Id).Text(), DataTypes.Numero)

            dam.ExecNonQuery(ExecQueryType.Update)

        End Using

    End Sub

#End Region

#Region " Finestra Modale Nuovo Documento "

    Protected Sub SbiancaModale()
        ddlDettaglioDescrizioneUpload.ClearSelection()
        txtNoteUpload.Text = String.Empty
        chkFlagVisibilita_new.Checked = False
    End Sub

    Protected Sub btnConfermaUpload_Click(sender As Object, e As EventArgs)

        DocumentiHelper.SaveDocumento(fup.PostedFile, ddlDettaglioDescrizioneUpload.SelectedItem.Text, txtNoteUpload.Text, OnVacContext.CodiceUslCorrente, Common.OnVacStoricoVaccinaleCentralizzato.GetValoreVisibilitaDatiVaccinali(chkFlagVisibilita_new))
        txtNote.Text = String.Empty
        OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Upload eseguito con successo!", "UPLOAD", False, False))
        fmUpload.VisibileMD = False

        SbiancaModale()

        State = DocumentiState.None
        ' Caricamento documenti del paziente
        LoadDocumenti()
        SetStateToolbar()
        SetStateControls()

    End Sub

#End Region

End Class
