Imports Onit.Database.DataAccessManager

Partial Class RischioCicli
    Inherits OnVac.Common.PageBase

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

#Region " Properties "

    'datatable contenente i cicli associati alla categoria
    Private Property dtRischioCicli() As DataTable
        Get
            Return Session("dtRischioCicli")
        End Get
        Set(ByVal Value As DataTable)
            Session("dtRischioCicli") = Value
        End Set
    End Property

    'contiene il valore del codice della categoria rischio
    Private Property CodiceCategoriaRischioCorrente() As String
        Get
            Return HttpContext.Current.Session("codCatRischio")
        End Get
        Set(ByVal Value As String)
            HttpContext.Current.Session("codCatRischio") = Value
        End Set
    End Property

#End Region

#Region " Eventi Page "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            Me.CodiceCategoriaRischioCorrente = Request.QueryString("codCatRischio")

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                '--
                ' Impostazione descrizione categoria nel titolo
                '--
                Dim descrizione As String = String.Empty
                '--
                Using bizCategorieRischio As New Biz.BizCategorieRischio(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())
                    descrizione = bizCategorieRischio.GetDescrizioneCategoriaRischio(Me.CodiceCategoriaRischioCorrente)
                End Using
                '--
                Me.LayoutTitolo.Text = String.Format("CATEGORIA RISCHIO: {0}", descrizione)
                '--
                ' Caricamento cicli per la categoria
                '--
                'caricamento dei cicli associati alla categoria di rischio
                CaricaCicli(Me.CodiceCategoriaRischioCorrente, genericProvider)
                '--
            End Using

        End If

    End Sub

#End Region

#Region " Eventi Toolbar "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btnIndietro"
                Server.Transfer("CategorieRischio.aspx", False)

            Case "btnAnnulla"
                Annulla()

            Case "btnInserisci"
                Inserisci()

            Case "btnSalva"
                Salva()

        End Select

    End Sub

#End Region

#Region " Eventi Datagrid "

    'cancellazione di un ciclo
    Private Sub dgrRischioCicli_DeleteCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgrRischioCicli.DeleteCommand

        Dim codCiclo As String = CType(dgrRischioCicli.Items(e.Item.ItemIndex).Cells(0).FindControl("lblCodRischioCiclo"), Label).Text

        'aggiorna lo stato della riga selezionata nel datatable
        Dim count As Integer
        For count = 0 To dtRischioCicli.Rows.Count - 1
            If Not dtRischioCicli.Rows(count).RowState = DataRowState.Deleted AndAlso dtRischioCicli.Rows(count)("cic_codice") = codCiclo Then
                dtRischioCicli.Rows(count).Delete()
                Exit For
            End If
        Next

        'aggiornamento del datagrid
        AggiornaDg()
        'disabilitazione di Left e TopBar
        OnitLayout31.Busy = True
        'disabilitazione dei pulsanti di inserimento e annullamento
        SettaToolBar(False)

    End Sub

#End Region

#Region " Eventi UserControl "

    'riabilita il layout e i pulsanti della toolbar
    Private Sub InsCicli1_OnAnnullaCicli() Handles InsCicli1.OnAnnullaCicli

        SettaToolBar(True)
        Me.OnitLayout31.Busy = False

    End Sub

    Private Sub InsCicli1_OnConfermaCicli(codCicli As System.Collections.ArrayList) Handles InsCicli1.OnConfermaCicli

        Dim dtCicli As New DataTable()

        Using dam As IDAM = OnVacUtility.OpenDam()

            'recupero dei dati dei cicli
            With dam.QB
                .NewQuery()
                .AddSelectFields("CIC_CODICE, CIC_DESCRIZIONE, CIC_DATA_INTRODUZIONE, CIC_DATA_FINE, COD_DESCRIZIONE")
                .AddTables("T_ANA_CICLI, T_ANA_CODIFICHE")
                .AddWhereCondition("CIC_STANDARD", Comparatori.Uguale, "COD_CODICE", DataTypes.OutJoinLeft)
                .AddWhereCondition("COD_CAMPO", Comparatori.Uguale, "CIC_STANDARD", DataTypes.Stringa)
                .OpenParanthesis()
                For i As Integer = 0 To codCicli.Count - 1
                    If i <> 0 Then
                        .AddWhereCondition("CIC_CODICE", Comparatori.Uguale, codCicli(i), DataTypes.Stringa, "OR")
                    Else
                        .AddWhereCondition("CIC_CODICE", Comparatori.Uguale, codCicli(i), DataTypes.Stringa)
                    End If
                Next
                .CloseParanthesis()
            End With

            dam.BuildDataTable(dtCicli)

        End Using

        'importazione delle righe nel datatable principale
        Dim row As DataRow
        For count As Integer = 0 To dtCicli.Rows.Count - 1

            row = dtRischioCicli.NewRow()
            row("CIC_CODICE") = dtCicli.Rows(count)("CIC_CODICE")
            row("CIC_DESCRIZIONE") = dtCicli.Rows(count)("CIC_DESCRIZIONE")
            row("CIC_DATA_INTRODUZIONE") = dtCicli.Rows(count)("CIC_DATA_INTRODUZIONE")
            row("CIC_DATA_FINE") = dtCicli.Rows(count)("CIC_DATA_FINE")
            row("COD_DESCRIZIONE") = dtCicli.Rows(count)("COD_DESCRIZIONE")

            dtRischioCicli.Rows.Add(row)

        Next

        'aggiornamento del datagrid
        AggiornaDg()

    End Sub

#End Region

#Region " Private "

#Region " Caricamento dei cicli associati alla categoria "

    Private Sub CaricaCicli(codiceCategoriaRischio As String)
        '--
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            CaricaCicli(codiceCategoriaRischio, genericProvider)
        End Using
        '--
    End Sub

    Private Sub CaricaCicli(codiceCategoriaRischio As String, genericProvider As DAL.DbGenericProvider)
        '--
        Me.dtRischioCicli = New DataTable()
        '--
        Using bizCicli As New Biz.BizCicli(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())
            '--
            Me.dtRischioCicli = bizCicli.GetDtCicliByCategoriaRischio(codiceCategoriaRischio)
            '--
        End Using
        '--
        'aggiornamento del datagrid
        AggiornaDg()
        '--
    End Sub

#End Region

    'aggiorna le righe del datagrid
    Private Sub AggiornaDg()

        Dim dwRischioCicli As DataView = dtRischioCicli.DefaultView
        dwRischioCicli.Sort = "CIC_DESCRIZIONE"

        dgrRischioCicli.DataSource = dwRischioCicli
        dgrRischioCicli.DataBind()

    End Sub

    'salva le modifiche alla lista dei cicli
    Private Sub Salva()

        Dim row As DataRow = dtRischioCicli.NewRow()

        Using dam As IDAM = OnVacUtility.OpenDam()

            dam.BeginTrans()

            Try

                For Each row In dtRischioCicli.Rows

                    'controllo dello stato delle righe
                    Select Case row.RowState

                        Case DataRowState.Deleted
                            'eliminazione dei cicli dal DB
                            row.RejectChanges()
                            With dam.QB
                                .NewQuery()
                                .AddTables("T_ANA_LINK_RISCHIO_CICLI")
                                .AddWhereCondition("RCI_RSC_CODICE", Comparatori.Uguale, CodiceCategoriaRischioCorrente, DataTypes.Stringa)
                                .AddWhereCondition("RCI_CIC_CODICE", Comparatori.Uguale, row("CIC_CODICE"), DataTypes.Stringa)
                            End With
                            dam.ExecNonQuery(ExecQueryType.Delete)
                            row.Delete()

                        Case DataRowState.Added
                            'aggiunta dei cicli al DB
                            With dam.QB
                                .NewQuery()
                                .AddInsertField("RCI_RSC_CODICE", CodiceCategoriaRischioCorrente, DataTypes.Stringa)
                                .AddInsertField("RCI_CIC_CODICE", row("CIC_CODICE"), DataTypes.Stringa)
                                .AddTables("T_ANA_LINK_RISCHIO_CICLI")
                            End With
                            dam.ExecNonQuery(ExecQueryType.Insert)

                    End Select

                Next

                dam.Commit()

            Catch ex As Exception

                dam.Rollback()
                ex.InternalPreserveStackTrace()
                Throw

            End Try

        End Using

        'aggiornamento del datagrid
        dtRischioCicli.AcceptChanges()
        AggiornaDg()

        'riabilitazione di Left e TopBar
        Me.OnitLayout31.Busy = False

        'riabilitazione del pulsante della ToolBar
        SettaToolBar(True)

    End Sub

    'inserimento dei cicli
    Private Sub Inserisci()

        'apertura della modale
        InsCicli1.ModaleName = "fmCicliRischio"
        InsCicli1.LoadModale()
        fmCicliRischio.VisibileMD = True

        'disabilitazione di Left e TopBar
        Me.OnitLayout31.Busy = True

        'disabilita i pulsanti della ToolBar
        SettaToolBar(False)

    End Sub

    'annulla le modifiche al datagrid 
    Private Sub Annulla()

        ' Caricamento cicli e aggiornamento datagrid
        CaricaCicli(Me.CodiceCategoriaRischioCorrente)

        'riabilita Left e TopBar
        Me.OnitLayout31.Busy = False

        'riabilita i pulsanti della ToolBar
        SettaToolBar(True)

    End Sub

    'setta lo stato dei pulsanti della ToolBar
    Private Sub SettaToolBar(en As Boolean)

        Me.ToolBar.Items.FromKeyButton("btnIndietro").Enabled = en

    End Sub

#End Region

End Class
