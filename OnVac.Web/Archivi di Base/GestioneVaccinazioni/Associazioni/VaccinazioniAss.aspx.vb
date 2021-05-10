Imports Onit.Database.DataAccessManager

Partial Class OnVac_VaccinazioniAss
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
            Return Session("OnVac_nMod")
        End Get
        Set(Value As Int16)
            Session("OnVac_nMod") = Value
        End Set
    End Property
   
    'tabella riempita dal database   
    Property vac() As DataTable
        Get
            Return Session("OnVac_vac")
        End Get
        Set(Value As DataTable)
            Session("OnVac_vac") = Value
        End Set
    End Property

    'variabile che contiene il codice del nomecom in esame
    Property codCom() As String
        Get
            Return Session("OnVac_codCom")
        End Get
        Set(Value As String)
            Session("OnVac_codCom") = Value
        End Set
    End Property

#End Region

#Region " Variables "

    'variabile che contiene qualsiasi stringa 'sql'
    Private sql As String

    ' la variabile 'strJS'  contiene qualsiasi script che deve essere eseguito dal browser al caricamento successivo della pag.
    '(lo script verrà renderizzato tramite il comando asp 'response.write' nella pag HTML)
    Public strJS As String

#End Region

#Region " Page "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack() Then

            nMod = 0
            codCom = Request.QueryString("ASS").ToString.Split("|")(0) 'il valore è passato dalla pag. 'associazioni.aspx'
            LayoutTitolo.Text = Request.QueryString("ASS").ToString.Split("|")(1)

            If Not vac Is Nothing Then
                vac.Dispose()
            End If
            vac = New DataTable()

            Dim DAM As IDAM = OnVacUtility.OpenDam()

            DAM.QB.NewQuery()
            DAM.QB.AddTables("t_ana_link_ass_vaccinazioni,t_ana_vaccinazioni")
            DAM.QB.AddSelectFields("vac_codice,vac_descrizione")
            DAM.QB.AddWhereCondition("val_vac_codice", Comparatori.Uguale, "vac_codice", DataTypes.Join)
            DAM.QB.AddWhereCondition("val_ass_codice", Comparatori.Uguale, codCom, DataTypes.Stringa)
            DAM.QB.AddOrderByFields("vac_descrizione")

            Try
                DAM.BuildDataTable(vac)
            Catch exc As Exception
                'Throw
            Finally
                OnVacUtility.CloseDam(DAM)
            End Try

            Dim vac_key(0) As DataColumn
            vac_key(0) = vac.Columns("vac_codice")
            vac.PrimaryKey = vac_key

            'riempie il datagrid con la tab vac
            dg_vac.DataSource = vac.DefaultView
            dg_vac.DataBind()

        End If

        'imposta la stringa a nulla nel caso precedentemente contenesse uno script
        strJS = ""

    End Sub

#End Region

#Region " Toolbar "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btn_Salva"
                Salva()
                OnitLayout31.Busy = False
                nMod = 0

            Case "btn_Inserisci"
                Inserisci()

            Case "btn_Annulla"
                vac.PrimaryKey = Nothing
                vac.RejectChanges()
                vac.DefaultView.Sort = "vac_codice"
                vac.DefaultView.Sort = ""

                dg_vac.DataSource = vac.DefaultView
                dg_vac.DataBind()

                Dim vac_key(0) As DataColumn
                vac_key(0) = vac.Columns("vac_codice")
                vac.PrimaryKey = vac_key
                OnitLayout31.Busy = False

                nMod = 0

        End Select

    End Sub

#End Region

#Region " DataGrid "

    Private Sub dg_vac_UpdateCommand(source As System.Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_vac.UpdateCommand

        'recupera i valori immessi nella riga del datagrid e li inserisce nella tabella 'vac'
        Dim fm_vac_edit As Onit.Controls.OnitModalList = DirectCast(e.Item.Cells(1).FindControl("fm_vac_edit"), Onit.Controls.OnitModalList)

        Dim cod As String = fm_vac_edit.Codice.ToString()
        Dim des As String = fm_vac_edit.Descrizione.ToString()

        vac.Rows.Item(0)(0) = cod
        vac.Rows.Item(0)(1) = des

        'ricarica il datagrid senza la riga editabile
        dg_vac.EditItemIndex = -1
        vac.DefaultView.Sort = "vac_codice"
        vac.DefaultView.Sort = ""

        dg_vac.DataSource = vac.DefaultView
        dg_vac.DataBind()

        Me.ToolBar.Enabled = True

    End Sub

    Private Sub dg_vac_CancelCommand(source As System.Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_vac.CancelCommand
        cancellaRiga(vac.Rows(0))
        nMod -= 1
        If (nMod = 0) Then OnitLayout31.Busy = False
    End Sub

    'N.B.l'evento non rende la riga editabile, anche se il nome dice così, ma la elimina!!
    Private Sub dg_vac_EditCommand(source As System.Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_vac.EditCommand

        'controlla se si è in fase 'editabile'
        If (dg_vac.EditItemIndex = -1) Then
            'calcola il numero di riga da eliminare
            Dim row As DataRow
            row = vac.Rows.Find(CType(dg_vac.Items.Item(e.Item.ItemIndex).Cells(1).FindControl("tb_codVac"), Label).Text)
            cancellaRiga(row)
            OnitLayout31.Busy = True
            nMod += 1
        Else
            '   avverte che si è in fase edit
            strJS = "<script language='javascript'>alert('Cliccare AGGIORNA O ANNULLA della riga che si sta modificando prima di eliminare questa riga')</script>"
            ToolBar.Enabled = False
        End If

    End Sub

#End Region

#Region " Private "

    Private Sub Salva()

        Dim row As DataRow
        vac.PrimaryKey = Nothing

        Dim DAM As IDAM = OnVacUtility.OpenDam()
        Try

            DAM.BeginTrans()

            For Each row In vac.Rows

                If (row.RowState = DataRowState.Added) Then
                    DAM.QB.NewQuery()
                    DAM.QB.AddTables("t_ana_link_ass_vaccinazioni")
                    DAM.QB.AddInsertField("val_ass_codice", codCom, DataTypes.Stringa)
                    DAM.QB.AddInsertField("val_vac_codice", row("vac_codice"), DataTypes.Stringa)
                    DAM.ExecNonQuery(ExecQueryType.Insert)
                End If

                If row.RowState = DataRowState.Deleted Then
                    row.RejectChanges()
                    DAM.QB.NewQuery()
                    DAM.QB.AddTables("t_ana_link_ass_vaccinazioni")
                    DAM.QB.AddWhereCondition("val_ass_codice", Comparatori.Uguale, codCom, DataTypes.Stringa)
                    DAM.QB.AddWhereCondition("val_vac_codice", Comparatori.Uguale, row("vac_codice"), DataTypes.Stringa)
                    DAM.ExecNonQuery(ExecQueryType.Delete)
                    row.Delete()
                End If

            Next

            DAM.Commit()

        Catch exc As Exception
            DAM.Rollback()
            exc.InternalPreserveStackTrace()
            Throw
        Finally
            OnVacUtility.CloseDam(DAM)
        End Try

        Dim vac_key(0) As DataColumn
        vac_key(0) = vac.Columns("vac_codice")
        vac.PrimaryKey = vac_key

        vac.AcceptChanges()

    End Sub

    Private Sub Inserisci()

        'aggiungo nuova riga alla tabella vac
        Dim newrow As DataRow
        newrow = vac.NewRow
        newrow("vac_codice") = "X"
        vac.Rows.InsertAt(newrow, 0)

        dg_vac.EditItemIndex = 0
        'ricarica la tabella nel datagrid
        vac.DefaultView.Sort = "vac_codice"
        vac.DefaultView.Sort = ""
        dg_vac.DataSource = vac.DefaultView
        dg_vac.DataBind()
        ToolBar.Enabled = False

        Dim dvVac As New DataView(vac)
        dvVac.RowFilter = "vac_codice <> 'X'"

        Dim stbFiltro As New System.Text.StringBuilder()

        For i As Int16 = 0 To dvVac.Count - 1
            If dvVac(i).Row.RowState <> DataRowState.Deleted Then
                stbFiltro.AppendFormat(" vac_codice <> '{0}' and", dvVac(i)("vac_codice"))
            End If
        Next

        If stbFiltro.Length = 0 Then
            stbFiltro.Append("'true'='true'")
        Else
            stbFiltro.Remove(stbFiltro.Length - 3, 3)
        End If
        stbFiltro.Append(" order by vac_ordine")

        Dim fm_vac_edit As Onit.Controls.OnitModalList = DirectCast(dg_vac.Items(dg_vac.EditItemIndex).FindControl("fm_vac_edit"), Onit.Controls.OnitModalList)

        fm_vac_edit.Filtro = stbFiltro.ToString()
        fm_vac_edit.Codice = ""

        'lo script da il fuoco al primo campo di testo della riga da edit
        strJS = "<script language='javascript'>document.getElementById(""dg_vac"").rows(" & dg_vac.EditItemIndex + 1 & ").cells(1).firstChild.childNodes[0].focus()</script>"

        Me.OnitLayout31.Busy = True
        nMod += 1

    End Sub

    Private Sub cancellaRiga(ByRef row As DataRow)

        row.Delete()
        dg_vac.EditItemIndex = -1

        'controlla se eliminando la riga si deve visualizzare la pag precedente in quanto 
        'in quella corrente non ne rimangono 
        If (dg_vac.Items.Count = 1) Then
            If (dg_vac.CurrentPageIndex <> 0) Then
                dg_vac.CurrentPageIndex -= 1
            End If
        End If

        'ricarico mil datagrid
        vac.DefaultView.Sort = "vac_codice"
        vac.DefaultView.Sort = ""
        dg_vac.DataSource = vac.DefaultView
        dg_vac.DataBind()

        ToolBar.Enabled = True

    End Sub

#End Region

End Class
