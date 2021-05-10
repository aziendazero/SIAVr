Imports Onit.Database.DataAccessManager


Public Class InsCicli
    Inherits Common.UserControlFinestraModalePageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub
    Protected WithEvents ToolBar As Infragistics.WebUI.UltraWebToolbar.UltraWebToolbar
    Protected WithEvents dgrCicli As System.Web.UI.WebControls.DataGrid

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

    'restituisce il valore del codice della categoria rischio
    Private ReadOnly Property CodCatRischio()
        Get
            Return HttpContext.Current.Session("codCatRischio")
        End Get
    End Property

#End Region

#Region " Events "

    Public Event OnAnnullaCicli()
    Public Event OnConfermaCicli(codCicli As ArrayList)

#End Region

#Region " Overrides "

    'caricamento della modale
    Overrides Sub LoadModale()

        Dim dtCicli As New DataTable()

        Using dam As IDAM = OnVacUtility.OpenDam()

            'caricamento dei cicli
            With dam.QB
                .AddSelectFields("1")
                .AddTables("T_ANA_LINK_RISCHIO_CICLI")
                .AddWhereCondition("RCI_CIC_CODICE", Comparatori.Uguale, "CIC_CODICE", DataTypes.Replace)
                .AddWhereCondition("RCI_RSC_CODICE", Comparatori.Uguale, CodCatRischio, DataTypes.Stringa)
            End With
            Dim queryCicli As String = dam.QB.GetSelect()

            With dam.QB
                .NewQuery(False, False)
                .AddSelectFields("CIC_CODICE", "CIC_DESCRIZIONE")
                .AddTables("T_ANA_CICLI")
                .AddWhereCondition("CIC_OBSOLETO", Comparatori.Uguale, "N", DataTypes.Stringa)
                .AddWhereCondition("", Comparatori.NotExist, "(" & queryCicli & ")", DataTypes.Replace)
            End With

            dam.BuildDataTable(dtCicli)

        End Using

        'aggiorna il datagrid
        Dim dwCicli As DataView = dtCicli.DefaultView
        dwCicli.Sort = "CIC_DESCRIZIONE"

        dgrCicli.DataSource = dwCicli
        dgrCicli.DataBind()

    End Sub

#End Region

#Region " Eventi Toolbar "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btn_Annulla"
                Annulla()

            Case "btn_Conferma"
                Conferma()

        End Select

    End Sub

#End Region

#Region " Private "

    'annullamento della modale
    Private Sub Annulla()
        RaiseEvent OnAnnullaCicli()
    End Sub

    'conferma dei cicli selezionati
    Private Sub Conferma()

        Dim codCicli As New ArrayList()

        'recupero dei codici
        For count As Integer = 0 To dgrCicli.Items.Count - 1
            If CType(dgrCicli.Items(count).Cells(0).FindControl("cb"), CheckBox).Checked Then
                codCicli.Add(dgrCicli.Items(count).Cells(2).Text)
            End If
        Next

        'evento
        RaiseEvent OnConfermaCicli(codCicli)

    End Sub

#End Region

End Class
