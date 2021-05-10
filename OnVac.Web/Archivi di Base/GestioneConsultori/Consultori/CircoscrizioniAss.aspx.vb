Imports Onit.Database.DataAccessManager
Imports Onit.Controls


Partial Class OnVac_CircoscrizioniAss
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


#Region " Properties "


    Property nMod() As Int16
        Get
            Return Session("OnVac_nMod")
        End Get
        Set(ByVal Value As Int16)
            Session("OnVac_nMod") = Value
        End Set
    End Property

    'tabella riempita dal database 
    Property Circoscrizioni() As DataTable
        Get
            Return Session("OnVac_Circoscrizioni")
        End Get
        Set(ByVal Value As DataTable)
            Session("OnVac_Circoscrizioni") = Value
        End Set
    End Property

    'variabile che contiene il codice del consultorio in esame
    Property codConsultorio() As String
        Get
            Return Session("OnVac_codConsultorio")
        End Get
        Set(ByVal Value As String)
            Session("OnVac_codConsultorio") = Value
        End Set
    End Property


#End Region


    'variabile che contiene qualsiasi stringa 'sql'
    Private sql As String

    ' la variabile 'strJS' contiene qualsiasi script che deve essere eseguito dal browser al caricamento successivo della pag.
    '(lo script verrà renderizzato tramite il comando asp 'response.write' nella pag HTML)
    Public strJS As String


    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack() Then

            Dim _bizCns As Biz.BizConsultori

            nMod = 0
            codConsultorio = Request.QueryString("CODICE").ToString.Split("|")(0) 'il valore è passato dalla pag. 'consultori.aspx'

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                _bizCns = New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                LayoutTitolo.Text = _bizCns.GetCnsDescrizione(codConsultorio)

                'la sql recupera i valori codice e descrizione dei comuni associati tramite un join 
                Circoscrizioni = _bizCns.GetCircoscrizioniInConsultorio(codConsultorio)
            End Using

            SetPrimaryKeyCircoscrizioni(False)

            'riempie il datagrid con la tab circoscrizioni
            Circoscrizioni.DefaultView.Sort = "cir_descrizione"
            dg_circoscrizioni.DataSource = Circoscrizioni.DefaultView
            dg_circoscrizioni.DataBind()

        End If

        'imposta la stringa a nulla nel caso precedentemente contenesse uno script
        strJS = String.Empty

    End Sub

    Private Sub Salva()

        If checkLock() Then Exit Sub

        Dim DAM As IDAM = OnVacUtility.OpenDam()

        Try

            DAM.BeginTrans()

            Dim row As DataRow
            SetPrimaryKeyCircoscrizioni(True)

            ' Cancellazioni
            For Each row In Circoscrizioni.Rows
                If (row.RowState = DataRowState.Deleted) Then
                    DAM.QB.NewQuery()
                    DAM.QB.AddTables("t_ana_link_circoscrizioni_cns")
                    DAM.QB.AddWhereCondition("rco_cns_codice", Comparatori.Uguale, codConsultorio, DataTypes.Stringa)
                    DAM.QB.AddWhereCondition("rco_cir_codice", Comparatori.Uguale, row("rco_cir_codice", DataRowVersion.Original), DataTypes.Stringa)
                    DAM.ExecNonQuery(ExecQueryType.Delete)
                End If
            Next

            ' Inserimenti
            For Each row In Circoscrizioni.Rows
                If row.RowState = DataRowState.Added Then
                    DAM.QB.NewQuery()
                    DAM.QB.AddTables("t_ana_link_circoscrizioni_cns")
                    DAM.QB.AddInsertField("rco_cns_codice", codConsultorio, DataTypes.Stringa)
                    DAM.QB.AddInsertField("rco_cir_codice", row("rco_cir_codice") & "", DataTypes.Stringa)
                    DAM.ExecNonQuery(ExecQueryType.Insert)
                End If
            Next

            ' Ripristino primary key del datatable
            SetPrimaryKeyCircoscrizioni(False)

            ' Conferma modifiche del datatable
            Circoscrizioni.AcceptChanges()

            DAM.Commit()

        Catch exc As Exception
            DAM.Rollback()
            exc.InternalPreserveStackTrace()
            Throw

        Finally
            OnVacUtility.CloseDam(DAM)
        End Try

    End Sub

    Private Sub Inserisci()

        'aggiungo nuova riga alla tabella comuni
        Dim newrow As DataRow
        newrow = Circoscrizioni.NewRow
        newrow("rco_cir_codice") = "X"
        Circoscrizioni.Rows.InsertAt(newrow, 0)

        'ricarica la tabella nel datagrid
        Circoscrizioni.DefaultView.Sort = String.Empty

        dg_circoscrizioni.EditItemIndex = 0
        dg_circoscrizioni.DataSource = Circoscrizioni.DefaultView
        dg_circoscrizioni.DataBind()

        ToolBar.Enabled = False

        Dim dvCirc As New DataView(Circoscrizioni)
        dvCirc.RowFilter = "rco_cir_codice <> 'X'"


        Dim stbFiltro As New System.Text.StringBuilder

        If dvCirc.Count = 0 Then
            ' Devo filtrare le circoscrizioni inseribili in base al comune associato al consultorio, per cui cerco il comune.
            Dim _bizCns As Biz.BizConsultori
            Dim cod_comune As String = String.Empty

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                _bizCns = New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                cod_comune = _bizCns.GetComuneConsultorio(codConsultorio)
            End Using

            If (cod_comune = String.Empty) Then
                Throw New Exception("Il centro vaccinale corrente non ha un comune associato.")
            End If

            stbFiltro.AppendFormat(" cir_com_codice = '{0}' ", cod_comune)
        Else
            ' Se ci sono già circoscrizioni associate, imposto il filtro della modale in modo che non vengano elencate
            ' per far sì che non vengano inserite due volte. Il codice del comune è nella riga, per cui non c'è bisogno di cercarlo.
            stbFiltro.AppendFormat(" cir_com_codice = '{0}' ", dvCirc(0)("cns_com_codice"))
            Dim i As Int16
            For i = 0 To dvCirc.Count - 1
                If dvCirc(i).Row.RowState <> DataRowState.Deleted Then
                    stbFiltro.AppendFormat(" and cir_codice <> '{0}' ", dvCirc(i)("rco_cir_codice"))
                End If
            Next
        End If

        stbFiltro.Append(" order by cir_descrizione")

        Dim _fm As OnitModalList
        _fm = DirectCast(dg_circoscrizioni.Items(dg_circoscrizioni.EditItemIndex).FindControl("fm_cir"), Onit.Controls.OnitModalList)
        _fm.Filtro = stbFiltro.ToString
        _fm.Codice = String.Empty

        'lo script dà il fuoco al primo campo di testo della riga da edit
        strJS = "<script language='javascript'>document.getElementById(""dg_circoscrizioni"").rows[" & dg_circoscrizioni.EditItemIndex + 1 & "].cells[1].firstChild.firstChild.focus()</script>"
        OnitLayout31.Busy = True

        nMod += 1
    End Sub


    Private Sub dg_circoscrizioni_UpdateCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_circoscrizioni.UpdateCommand

        If checkLock() Then Exit Sub

        'recupera i valori immessi nella riga del datagrid e li inserisce nel datatable 'circoscrizioni'
        Dim _fm As OnitModalList
        _fm = DirectCast(e.Item.Cells(1).FindControl("fm_cir"), Onit.Controls.OnitModalList)

        Circoscrizioni.Rows(0)(0) = _fm.Codice
        Circoscrizioni.Rows(0)(1) = _fm.Descrizione
        Circoscrizioni.Rows(0)(2) = _fm.ValoriAltriCampi("cir_com_codice")

        'ricarica il datagrid senza la riga editabile
        dg_circoscrizioni.EditItemIndex = -1
        Circoscrizioni.DefaultView.Sort = "cir_descrizione"
        dg_circoscrizioni.DataSource = Circoscrizioni.DefaultView
        dg_circoscrizioni.DataBind()
        ToolBar.Enabled = True
    End Sub


    Private Sub dg_circoscrizioni_CancelCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_circoscrizioni.CancelCommand
        If checkLock() Then
            Exit Sub
        End If
        cancellaRiga(Circoscrizioni.Rows(0))
        nMod -= 1
        If (nMod = 0) Then OnitLayout31.Busy = False
    End Sub


    Private Sub cancellaRiga(ByRef row As DataRow)
        row.Delete()
        dg_circoscrizioni.EditItemIndex = -1
        'controlla se eliminando la riga si deve visualizzare la pag precedente in quanto 
        'in quella corrente non ne rimangono 
        If (dg_circoscrizioni.Items.Count = 1) Then
            If (dg_circoscrizioni.CurrentPageIndex <> 0) Then
                dg_circoscrizioni.CurrentPageIndex -= 1
            End If
        End If
        'ricarico il datagrid
        Circoscrizioni.DefaultView.Sort = "cir_descrizione"
        dg_circoscrizioni.DataSource = Circoscrizioni.DefaultView
        dg_circoscrizioni.DataBind()
        ToolBar.Enabled = True
    End Sub


    'N.B.l'evento non rende la riga editabile, anche se il nome dice così, ma la elimina!!
    Private Sub dg_circoscrizioni_EditCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_circoscrizioni.EditCommand
        If checkLock() Then Exit Sub

        'controlla se si è in fase 'editabile'
        If (dg_circoscrizioni.EditItemIndex = -1) Then
            'calcola il numero di riga da eliminare
            Dim row As DataRow
            row = Circoscrizioni.Rows.Find(CType(dg_circoscrizioni.Items.Item(e.Item.ItemIndex).FindControl("tb_codCir"), Label).Text)
            cancellaRiga(row)
            OnitLayout31.Busy = True
            nMod += 1
        Else
            '   avverte che si è in fase edit
            strJS = "<script language='javascript'>alert('Cliccare AGGIORNA O ANNULLA della riga che si sta modificando prima di eliminare questa riga')</script>"
            ''marco  ToolBar.Enabled = False
        End If
    End Sub


    Private Sub OnitLayout31_BusyChanged(ByVal Sender As Object, ByVal E As System.EventArgs, ByVal State As Boolean) Handles OnitLayout31.BusyChanged
        If (State) Then
            OnitLayout31.lock.Lock(0, codConsultorio)
        Else
            OnitLayout31.lock.EndLock(0)
        End If
    End Sub


    Private Function checkLock() As Boolean
        lb_warning.Visible = False
        If (OnitLayout31.lock.IsLocked(0) AndAlso codConsultorio = OnitLayout31.lock.GetLockInfo(0).Info) Then
            If Not (lb_warning.Visible) Then
                lb_warning.Visible = True
                lb_warning.Text = "ATTENZIONE.. L'APPLICAZIONE E' BLOCCATA DA " & OnitLayout31.lock.GetLockInfo(0).UserCodice & ".<BR>RIPROVARE FRA QUALCHE MINUTO."
            End If
            Return True
        End If
        Return False
    End Function


    Private Sub OnitLayout31_TimeoutLock(ByVal Sender As Object, ByVal E As System.EventArgs, ByVal int As String) Handles OnitLayout31.TimeoutLock
        lb_warning.Visible = True
        lb_warning.Text = "ATTENZIONE..IL BLOCCO SULL'APPLICAZIONE E' SCADUTO..CHIUDERE<BR> LA PAGINA SENZA SALVARE E RIPROVARE FRA QUALCHE MINUTO."
    End Sub


    Private Sub ToolBar_ButtonClicked(ByVal sender As Object, ByVal e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked
        Select Case e.Button.Key
            Case ("btn_Salva")
                Salva()
                OnitLayout31.Busy = False
                nMod = 0

            Case ("btn_Inserisci")
                If checkLock() Then Exit Sub
                Inserisci()

            Case ("btn_Annulla")
                If checkLock() Then Exit Sub

                SetPrimaryKeyCircoscrizioni(True)

                Circoscrizioni.RejectChanges()

                Circoscrizioni.DefaultView.Sort = "cir_descrizione"
                dg_circoscrizioni.DataSource = Circoscrizioni.DefaultView
                dg_circoscrizioni.DataBind()

                SetPrimaryKeyCircoscrizioni(False)

                OnitLayout31.Busy = False
                nMod = 0
        End Select
    End Sub


    ' Imposta la chiave del datatable Circoscrizioni. Se il parametro è true, la chiave viene eliminata
    Private Sub SetPrimaryKeyCircoscrizioni(ByVal setToNothing As Boolean)
        If (setToNothing) Then
            Circoscrizioni.PrimaryKey = Nothing
        Else
            Dim com_key(0) As DataColumn
            com_key(0) = Circoscrizioni.Columns("rco_cir_codice")
            Circoscrizioni.PrimaryKey = com_key
        End If
    End Sub


End Class
