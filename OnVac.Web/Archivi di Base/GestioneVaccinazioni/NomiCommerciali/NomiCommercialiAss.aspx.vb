Imports Onit.Database.DataAccessManager
Imports Onit.Controls


Partial Class Onass_NomiCommercialiAss
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

#Region " Properties "

    Property nMod() As Int16
        Get
            Return Session("Onass_nMod")
        End Get
        Set(Value As Int16)
            Session("Onass_nMod") = Value
        End Set
    End Property

    'tabella riempita dal database   
    Property ass() As DataTable
        Get
            Return Session("Onass_ass")
        End Get
        Set(Value As DataTable)
            Session("Onass_ass") = Value
        End Set
    End Property

    'variabile che contiene il codice del nomecom in esame
    Property codCom() As String
        Get
            Return Session("Onass_codCom")
        End Get
        Set(Value As String)
            Session("Onass_codCom") = Value
        End Set
    End Property

#End Region

#Region " Private/Public "

    'variabile che contiene qualsiasi stringa 'sql'
    Private sql As String

    ' la variabile 'strJS'  contiene qualsiasi script che deve essere eseguito dal browser al caricamento successivo della pag.
    '(lo script verrà renderizzato tramite il comando asp 'response.write' nella pag HTML)
    Public strJS As String

#End Region

#Region " Page "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack() Then

            Me.nMod = 0

            Dim s As String() = Request.QueryString("NOC").ToString().Split("|")
            Me.codCom = s(0)
            Me.LayoutTitolo.Text = s(1)

            If Not ass Is Nothing Then
                ass.Dispose()
            End If
            ass = New DataTable()

            Using DAM As IDAM = OnVacUtility.OpenDam()

                DAM.QB.NewQuery()
                DAM.QB.AddTables("t_ana_link_noc_associazioni,t_ana_associazioni")
                DAM.QB.AddSelectFields("ass_codice,ass_descrizione")
                DAM.QB.AddWhereCondition("nal_ass_codice", Comparatori.Uguale, "ass_codice", DataTypes.Join)
                DAM.QB.AddWhereCondition("nal_noc_codice", Comparatori.Uguale, codCom, DataTypes.Stringa)
                DAM.QB.AddOrderByFields("ass_descrizione")

                Try
                    DAM.BuildDataTable(ass)
                Catch exc As Exception      ' => PERCHE' NASCONDE L'ECCEZIONE ?!?
                    'Throw 
                End Try

            End Using

            Dim ass_key(0) As DataColumn
            ass_key(0) = ass.Columns("ass_codice")
            ass.PrimaryKey = ass_key

            'riempie il datagrid con la tab ass
            Me.dg_ass.DataSource = ass.DefaultView
            Me.dg_ass.DataBind()

        End If

        'imposta la stringa a nulla nel caso precedentemente contenesse uno script
        strJS = ""

    End Sub

#End Region

#Region " Datagrid "

    Private Sub dg_ass_UpdateCommand(source As System.Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_ass.UpdateCommand

        'recupera i valori immessi nella riga del datagrid e li inserisce nella tabella 'ass'
        Dim fm_ass_edit As OnitModalList = DirectCast(e.Item.Cells(1).FindControl("fm_ass_edit"), OnitModalList)

        ass.Rows.Item(0)(0) = fm_ass_edit.Codice
        ass.Rows.Item(0)(1) = fm_ass_edit.Descrizione

        'ricarica il datagrid senza la riga editabile
        Me.dg_ass.EditItemIndex = -1
        ass.DefaultView.Sort = "ass_codice"
        ass.DefaultView.Sort = ""

        Me.dg_ass.DataSource = ass.DefaultView
        Me.dg_ass.DataBind()

        Me.ToolBar.Enabled = True

    End Sub

    Private Sub dg_ass_CancelCommand(source As System.Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_ass.CancelCommand
        CancellaRiga(ass.Rows(0))
        Me.nMod -= 1
        If Me.nMod = 0 Then Me.OnitLayout31.Busy = False
    End Sub

    'N.B.l'evento non rende la riga editabile, anche se il nome dice così, ma la elimina!!
    Private Sub dg_ass_EditCommand(source As System.Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_ass.EditCommand

        'controlla se si è in fase 'editabile'
        If dg_ass.EditItemIndex = -1 Then
            'calcola il numero di riga da eliminare
            Dim row As DataRow = ass.Rows.Find(CType(dg_ass.Items.Item(e.Item.ItemIndex).Cells(1).FindControl("tb_codass"), Label).Text)
            CancellaRiga(row)
            Me.OnitLayout31.Busy = True
            Me.nMod += 1
        Else
            '  avverte che si è in fase edit
            strJS = "<script language='javascript'>alert('Cliccare AGGIORNA O ANNULLA della riga che si sta modificando prima di eliminare questa riga')</script>"
            Me.ToolBar.Enabled = False
        End If

    End Sub

#End Region

#Region " Toolbar "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btn_Indietro"

                Me.Response.Redirect(String.Format("NomiCommerciali.aspx?NOC={0}", HttpUtility.UrlEncode(codCom)), False)

            Case "btn_Salva"

                Salva()
                Me.OnitLayout31.Busy = False
                nMod = 0

            Case "btn_Inserisci"

                Inserisci()

            Case "btn_Annulla"

                ass.PrimaryKey = Nothing
                ass.RejectChanges()
                ass.DefaultView.Sort = "ass_codice"
                ass.DefaultView.Sort = ""

                Me.dg_ass.DataSource = ass.DefaultView
                Me.dg_ass.DataBind()

                Dim ass_key(0) As DataColumn
                ass_key(0) = ass.Columns("ass_codice")
                ass.PrimaryKey = ass_key

                Me.OnitLayout31.Busy = False
                Me.nMod = 0

        End Select

    End Sub

#End Region

#Region " Private Methods "

    Private Sub Salva()

        ass.PrimaryKey = Nothing

        Using DAM As IDAM = OnVacUtility.OpenDam()

            Try
                DAM.BeginTrans()

                For Each row As DataRow In ass.Rows

                    If row.RowState = DataRowState.Added Then

                        DAM.QB.NewQuery()
                        DAM.QB.AddTables("t_ana_link_noc_associazioni")
                        DAM.QB.AddInsertField("nal_noc_codice", codCom, DataTypes.Stringa)
                        DAM.QB.AddInsertField("nal_ass_codice", row("ass_codice"), DataTypes.Stringa)
                        DAM.ExecNonQuery(ExecQueryType.Insert)

                    End If

                    If row.RowState = DataRowState.Deleted Then

                        row.RejectChanges()

                        DAM.QB.NewQuery()
                        DAM.QB.AddTables("t_ana_link_noc_associazioni")
                        DAM.QB.AddWhereCondition("nal_noc_codice", Comparatori.Uguale, codCom, DataTypes.Stringa)
                        DAM.QB.AddWhereCondition("nal_ass_codice", Comparatori.Uguale, row("ass_codice"), DataTypes.Stringa)
                        DAM.ExecNonQuery(ExecQueryType.Delete)

                        row.Delete()

                    End If

                Next

                DAM.Commit()

            Catch exc As Exception

                DAM.Rollback()
                exc.InternalPreserveStackTrace()
                Throw

            End Try

        End Using

        Dim ass_key(0) As DataColumn
        ass_key(0) = ass.Columns("ass_codice")
        ass.PrimaryKey = ass_key
        ass.AcceptChanges()

    End Sub

    Private Sub Inserisci()

        'aggiungo nuova riga alla tabella ass
        Dim newrow As DataRow = ass.NewRow()
        newrow("ass_codice") = "X"

        ass.Rows.InsertAt(newrow, 0)

        Me.dg_ass.EditItemIndex = 0

        'ricarica la tabella nel datagrid
        ass.DefaultView.Sort = "ass_codice"
        ass.DefaultView.Sort = ""
        Me.dg_ass.DataSource = ass.DefaultView
        Me.dg_ass.DataBind()

        Me.ToolBar.Enabled = False

        Dim dvAss As New DataView(ass)
        dvAss.RowFilter = "ass_codice <> 'X'"

        Dim stbFiltro As New System.Text.StringBuilder()

        For i As Int16 = 0 To dvAss.Count - 1
            If dvAss(i).Row.RowState <> DataRowState.Deleted Then
                stbFiltro.AppendFormat(" ass_codice <> '{0}' and", dvAss(i)("ass_codice"))
            End If
        Next

        If (stbFiltro.Length = 0) Then
            stbFiltro.Append("'true'='true'")
        Else
            stbFiltro.Remove(stbFiltro.Length - 3, 3)
        End If

        stbFiltro.Append(" order by ass_ordine")

        Dim fm_ass_edit As OnitModalList = DirectCast(dg_ass.Items(dg_ass.EditItemIndex).FindControl("fm_ass_edit"), OnitModalList)

        fm_ass_edit.Filtro = stbFiltro.ToString()
        fm_ass_edit.Codice = ""

        'lo script da il fuoco al primo campo di testo della riga da edit
        strJS = "<script language='javascript'>document.getElementById(""dg_ass"").rows(" & Me.dg_ass.EditItemIndex + 1 & ").cells(1).firstChild.childNodes[0].focus()</script>"
        Me.OnitLayout31.Busy = True

        Me.nMod += 1

    End Sub

    Private Sub CancellaRiga(ByRef row As DataRow)

        row.Delete()

        dg_ass.EditItemIndex = -1

        'controlla se eliminando la riga si deve visualizzare la pag precedente in quanto 
        'in quella corrente non ne rimangono 
        If dg_ass.Items.Count = 1 Then
            If dg_ass.CurrentPageIndex <> 0 Then
                dg_ass.CurrentPageIndex -= 1
            End If
        End If

        'ricarico il datagrid
        ass.DefaultView.Sort = "ass_codice"
        ass.DefaultView.Sort = ""
        dg_ass.DataSource = ass.DefaultView
        dg_ass.DataBind()

        Me.ToolBar.Enabled = True

    End Sub

#End Region

End Class
