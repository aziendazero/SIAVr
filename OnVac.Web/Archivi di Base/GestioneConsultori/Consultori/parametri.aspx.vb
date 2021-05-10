Imports Onit.Database.DataAccessManager
Imports Onit.Controls

Imports Onit.OnAssistnet.OnVac.Biz
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.Collection


Partial Class OnVac_Parametri
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

    Property CodConsultorio() As String
        Get
            Return Session("OnVac_codConsultorio")
        End Get
        Set(Value As String)
            Session("OnVac_codConsultorio") = Value
        End Set
    End Property

    Property nMod() As Int16
        Get
            Return Session("OnVac_nMod")
        End Get
        Set(Value As Int16)
            Session("OnVac_nMod") = Value
        End Set
    End Property

    Property Tb_par() As DataTable
        Get
            Return Session("OnVac_tb_par")
        End Get
        Set(Value As DataTable)
            Session("OnVac_tb_par") = Value
        End Set
    End Property

    Property Newrow() As Boolean
        Get
            Return Session("OnVac_newrow")
        End Get
        Set(Value As Boolean)
            Session("OnVac_newrow") = Value
        End Set
    End Property

    Property Par_cns_cod() As String
        Get
            Return Session("OnVac_par_cns_cod")
        End Get
        Set(Value As String)
            Session("OnVac_par_cns_cod") = Value
        End Set
    End Property

#End Region

#Region " Public "

    Public strJS As String

#End Region

#Region " Page Events "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack() Then

            Me.nMod = 0
            Me.Newrow = False

            Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

                If (Request.QueryString("tipo").Split("|")(0) = "1") Then

                    Me.CodConsultorio = Request.QueryString("CODICE").Split("|")(0)

                    Me.LayoutTitolo.Text = genericProvider.Consultori.GetCnsDescrizione(Me.CodConsultorio)

                    Me.Par_cns_cod = Me.CodConsultorio

                    Dim item As New Infragistics.WebUI.UltraWebToolbar.TBarButton() With
                    {
                        .Key = "btn_Inserisci",
                        .Image = OnVacUtility.GetIconUrl("nuovo.gif"),
                        .Text = "Inserisci"
                    }

                    Me.ToolBar.Items.Add(item)

                    Me.dg_par.Columns(4).Visible = False

                    Me.OnitLayout31.Titolo = "Parametri"

                Else

                    Me.CodConsultorio = String.Empty

                    Me.Par_cns_cod = Constants.CommonConstants.CodiceConsultorioSistema

                    Me.dg_par.Columns(0).Visible = False
                    Me.dg_par.Columns(4).Visible = False

                    Me.OnitLayout31.Titolo = "Parametri Globali"

                End If

                If Not Me.Tb_par Is Nothing Then Me.Tb_par.Dispose()

                ' --- Lettura parametri --- '
                Using paramBiz As New BizParametri(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                    If Me.CodConsultorio = String.Empty Then
                        Me.Tb_par = paramBiz.GetDtGenericParameters()
                    Else
                        Me.Tb_par = paramBiz.GetDtParameters(Me.CodConsultorio)
                    End If

                End Using

            End Using

            ' --- Chiave primaria del datatable --- '
            Dim tb_par_key(0) As DataColumn
            tb_par_key(0) = Tb_par.Columns("par_codice")
            Tb_par.PrimaryKey = tb_par_key

            dg_par.DataSource = Tb_par.DefaultView
            dg_par.DataBind()

        End If

        strJS = ""

    End Sub

#End Region

#Region " OnitLayout Events "

    Private Sub OnitLayout1_BusyChanged(sender As System.Object, e As System.EventArgs, state As Boolean)

        If state Then
            Me.OnitLayout31.lock.Lock(0, CodConsultorio)
        Else
            Me.OnitLayout31.lock.EndLock(0)
        End If

    End Sub

    Private Sub OnitLayout1_TimeoutLock(sender As System.Object, e As System.EventArgs, int As Short)

        Me.lb_warning.Visible = True
        Me.lb_warning.Text = "ATTENZIONE! IL BLOCCO SULL'APPLICAZIONE E' SCADUTO.<BR>CHIUDERE LA PAGINA SENZA SALVARE E RIPROVARE FRA QUALCHE MINUTO."

    End Sub

#End Region

#Region " Toolbar Events "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btn_Salva"

                Salva()
                Me.OnitLayout31.Busy = False
                Me.nMod = 0

            Case "btn_Inserisci"

                Inserisci()

            Case "btn_Annulla"

                If CheckLock() Then Exit Sub

                Me.Tb_par.PrimaryKey = Nothing
                Me.Tb_par.RejectChanges()
                Me.Tb_par.DefaultView.Sort = "par_valore"
                Me.Tb_par.DefaultView.Sort = ""

                Me.dg_par.DataSource = Me.Tb_par.DefaultView
                Me.dg_par.DataBind()

                Dim tb_par_key(0) As DataColumn
                tb_par_key(0) = Me.Tb_par.Columns("par_codice")
                Me.Tb_par.PrimaryKey = tb_par_key

                Me.OnitLayout31.Busy = False

                Me.nMod = 0

        End Select

    End Sub

#End Region

#Region " Datagrid Events "

    Private Sub dg_par_CancelCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_par.CancelCommand

        If CheckLock() Then Exit Sub

        If Me.Newrow Then

            Me.Tb_par.Rows(0).Delete()

            If Me.dg_par.Items.Count = 1 Then

                If Me.dg_par.CurrentPageIndex <> 0 Then
                    Me.dg_par.CurrentPageIndex -= 1
                End If

            End If

            Me.Newrow = False

        End If

        Me.dg_par.EditItemIndex = -1
        Me.Tb_par.DefaultView.Sort = "par_valore"
        Me.Tb_par.DefaultView.Sort = ""
        Me.dg_par.DataSource = Tb_par.DefaultView
        Me.dg_par.DataBind()

        Me.ToolBar.Enabled = True

        Me.nMod -= 1
        If Me.nMod = 0 Then Me.OnitLayout31.Busy = False

    End Sub

    Private Sub dg_par_DeleteCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_par.DeleteCommand

        If CheckLock() Then Exit Sub

        'controlla se si è in fase 'editabile'
        If Me.dg_par.EditItemIndex = -1 Then

            'calcola il numero di riga da eliminare
            Dim row As DataRow =
                Me.Tb_par.Rows.Find(DirectCast(Me.dg_par.Items(e.Item.ItemIndex).Cells(2).FindControl("tb_codice"), Label).Text)

            row.Delete()

            'calcola la pagina da visualizzare dopo l'eliminazione
            If Me.dg_par.Items.Count = 1 Then

                If Me.dg_par.CurrentPageIndex <> 0 Then
                    Me.dg_par.CurrentPageIndex -= 1
                End If

            End If

            'ricarico il datagrid
            Me.Tb_par.DefaultView.Sort = "par_valore"
            Me.Tb_par.DefaultView.Sort = ""
            Me.dg_par.DataSource = Tb_par.DefaultView
            Me.dg_par.DataBind()

            Me.OnitLayout31.Busy = True
            Me.nMod += 1

        Else

            strJS = "<script language='javascript'>alert('Cliccare AGGIORNA o ANNULLA della riga che si sta modificando prima di eliminare questa riga')</script>"
            Me.ToolBar.Enabled = False

        End If

    End Sub

    Private Sub dg_par_UpdateCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_par.UpdateCommand

        If CheckLock() Then Exit Sub

        If Newrow Then

            Dim fmPar As Onit.Controls.OnitModalList = DirectCast(e.Item.FindControl("fm_par"), Onit.Controls.OnitModalList)
            Me.Tb_par.Rows(0)("par_descrizione") = fmPar.Descrizione
            Me.Tb_par.Rows(0)("par_valore") = DirectCast(e.Item.FindControl("tb_valore_edit"), TextBox).Text
            Me.Tb_par.Rows(0)("par_codice") = fmPar.Codice

        Else

            Dim row As DataRow
            row = Tb_par.Rows.Find(DirectCast(dg_par.Items(e.Item.ItemIndex).Cells(2).FindControl("fm_par"), Onit.Controls.OnitModalList).Codice)
            row("par_valore") = DirectCast(e.Item.FindControl("tb_valore_edit"), TextBox).Text

        End If

        Me.dg_par.EditItemIndex = -1
        Me.dg_par.SelectedIndex = -1

        Me.Tb_par.DefaultView.Sort = "par_valore"
        Me.Tb_par.DefaultView.Sort = ""

        Me.dg_par.DataSource = Me.Tb_par.DefaultView
        Me.dg_par.DataBind()

        Me.ToolBar.Enabled = True

        Me.Newrow = False

    End Sub

    Private Sub dg_par_EditCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_par.EditCommand

        If CheckLock() Then Exit Sub

        If Me.dg_par.EditItemIndex = -1 Then

            Me.dg_par.EditItemIndex = e.Item.ItemIndex
            Me.dg_par.DataSource = Me.Tb_par
            Me.dg_par.DataBind()

            DirectCast(Me.dg_par.Items(e.Item.ItemIndex).Cells(2).FindControl("fm_par"), Onit.Controls.OnitModalList).Enabled = False
            Me.ToolBar.Enabled = False

            If Me.Par_cns_cod = Constants.CommonConstants.CodiceConsultorioSistema Then
                strJS = "<script language=""javascript"">document.getElementById(""dg_par"").firstChild.childNodes[" & (1 + e.Item.ItemIndex) & "].childNodes[2].firstChild.focus()</script>"
            Else
                strJS = "<script language=""javascript"">document.getElementById(""dg_par"").firstChild.childNodes[" & (1 + e.Item.ItemIndex) & "].childNodes[3].firstChild.focus()</script>"
            End If

            Me.OnitLayout31.Busy = True

            Me.nMod += 1

        Else
            strJS = "<script language=""javascript"">alert(""Cliccare ANNULLA o AGGIORNA della riga che si sta modificando prima di modificare questa riga"")</script>"
        End If

    End Sub

#End Region

#Region " Private Methods "

    Private Function CreateParametro(row As DataRow) As Entities.Parametro

        Dim parametro As New Entities.Parametro()

        parametro.Codice = row("par_codice").ToString()
        parametro.Descrizione = row("par_descrizione").ToString().Replace("'", "''")
        parametro.Valore = row("par_valore").ToString()
        parametro.Consultorio = Me.Par_cns_cod
        If Me.Par_cns_cod = Constants.CommonConstants.CodiceConsultorioSistema Then
            parametro.Centrale = (row("par_centrale").ToString() = "S")
        Else
            parametro.Centrale = False
        End If

        Return parametro

    End Function

    Private Sub Salva()

        If CheckLock() Then Exit Sub

        Me.Tb_par.PrimaryKey = Nothing

        Using DAM As IDAM = OnVacUtility.OpenDam()

            Try
                DAM.BeginTrans()

                Using genericProvider As New DbGenericProvider(DAM)

                    Using bizParam As New BizParametri(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                        For Each row As DataRow In Tb_par.Rows

                            Select Case row.RowState

                                Case DataRowState.Added

                                    bizParam.InsertParameter(Me.CreateParametro(row))

                                Case DataRowState.Modified

                                    bizParam.UpdateParameter(Me.CreateParametro(row))

                                Case DataRowState.Deleted

                                    row.RejectChanges()

                                    bizParam.DeleteParameter(Me.CreateParametro(row))

                                    row.Delete()

                            End Select

                        Next

                    End Using

                End Using

                If DAM.ExistTra() Then DAM.Commit()

            Catch ex As Exception

                If DAM.ExistTra() Then DAM.Rollback()

                Me.OnitLayout31.InsertRoutineJS("alert('Errore durante il salvataggio su database. Modifiche non salvate.')")

            End Try

        End Using

        Dim tb_par_key(0) As DataColumn
        tb_par_key(0) = Tb_par.Columns("par_codice")
        Tb_par.PrimaryKey = tb_par_key
        Tb_par.AcceptChanges()

        Me.OnitLayout31.Busy = False

    End Sub

    Private Sub Inserisci()

        If CheckLock() Then Exit Sub

        Me.Newrow = True

        Dim row As DataRow = Nothing

        'aggiungo nuova riga alla tabella
        row = Me.Tb_par.NewRow()
        row("par_codice") = "X"
        Me.Tb_par.Rows.InsertAt(row, 0)

        'determina la pagina e il numero di item della riga da editare
        Me.dg_par.EditItemIndex = 0

        'ricarica la tabella nel datagrid
        Me.Tb_par.DefaultView.Sort = "par_valore"
        Me.Tb_par.DefaultView.Sort = ""
        Me.dg_par.DataSource = Me.Tb_par.DefaultView
        Me.dg_par.DataBind()

        Me.ToolBar.Enabled = False

        DirectCast(Me.dg_par.Items(Me.dg_par.EditItemIndex).Cells(2).FindControl("fm_par"), Onit.Controls.OnitModalList).Codice = ""

        If Me.Par_cns_cod <> Constants.CommonConstants.CodiceConsultorioSistema Then

            Dim stbFiltro As New System.Text.StringBuilder()

            'inversione del loop per considerare anche il primo parametro inserito (modifica 05/08/2004)
            For i As Int16 = Me.Tb_par.Rows.Count - 1 To 1 Step -1
                If Not (Me.Tb_par.Rows(i).RowState = DataRowState.Deleted) Then
                    stbFiltro.AppendFormat(" and par_codice <> '{0}'", Me.Tb_par.Rows(i)("par_codice"))
                End If
            Next

            stbFiltro.Append(" order by par_descrizione")

            DirectCast(Me.dg_par.Items(Me.dg_par.EditItemIndex).FindControl("fm_par"), Onit.Controls.OnitModalList).Filtro =
                String.Format("par_centrale='N' and par_cns_codice='{0}'{1}", Constants.CommonConstants.CodiceConsultorioSistema, stbFiltro.ToString())

        End If

        strJS = "<script language=""javascript"">document.getElementById(""dg_par"").firstChild.childNodes[" & (1 + Me.dg_par.EditItemIndex) & "].childNodes[2].firstChild.firstChild.focus()</script>"
        Me.OnitLayout31.Busy = True
        Me.nMod += 1

    End Sub

    Private Function CheckLock() As Boolean

        Me.lb_warning.Visible = False

        If Me.OnitLayout31.lock.IsLocked(0) AndAlso CodConsultorio = Me.OnitLayout31.lock.GetLockInfo(0).Info Then

            If Not Me.lb_warning.Visible Then
                Me.lb_warning.Visible = True
                Me.lb_warning.Text = "ATTENZIONE! L'APPLICAZIONE E' BLOCCATA DA " & Me.OnitLayout31.lock.GetLockInfo(0).UserCodice & ".<BR>RIPROVARE FRA QUALCHE MINUTO."
            End If

            Return True

        End If

        Return False

    End Function

#End Region

End Class
