Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.Web.UI.WebControls.Validators


Partial Class OnVac_Indisponibilita
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

#Region " Public/Protected "

    Public strJS As String
    Protected tb_data_edit_ClientId As String
    Protected tb_oraInizio_edit_ClientId As String
    Protected tb_oraFine_edit_ClientId As String

#End Region

#Region " Enums "

    Private Enum DatagridColumnIndex
        DeleteButton = 0
        EditButton = 1
        DataIndisponibilita = 2
        Festivita = 3
        Ricorsivita = 4
        Descrizione = 5
        OraInizioIndisponibilita = 5
        OraFineIndisponibilita = 6
        ' colonna vuota = 8
    End Enum

#End Region

#Region " Properties "

    Property itemTemp() As Object()
        Get
            Return Session("OnVac_itemTemp")
        End Get
        Set(Value As Object())
            Session("OnVac_itemTemp") = Value
        End Set
    End Property

    Property codAmbulatorio() As Integer
        Get
            Return Session("OnVac_codAmbulatorio")
        End Get
        Set(Value As Integer)
            Session("OnVac_codAmbulatorio") = Value
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

    Property indisp_str() As DataTable
        Get
            Return Session("OnVac_indisp_str")
        End Get
        Set(Value As DataTable)
            Session("OnVac_indisp_str") = Value
        End Set
    End Property

    Property newrow() As Boolean
        Get
            Return Session("OnVac_newrow")
        End Get
        Set(Value As Boolean)
            Session("OnVac_newrow") = Value
        End Set
    End Property

#End Region

#Region " Page Events "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        lb_warning.Text = String.Empty

        If Not IsPostBack() Then

            nMod = 0
            newrow = False
            codAmbulatorio = Request.QueryString("codice").Split("|")(0)

            Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                LayoutTitolo.Text = genericProvider.Consultori.GetAmbDescrizione(codAmbulatorio)
            End Using

            Using DAM As IDAM = OnVacUtility.OpenDam()

                indisp_str = BuildTableIndisponibilita(DAM)

                dg_indisp.DataSource = indisp_str
                dg_indisp.DataBind()

            End Using

        End If

        strJS = ""

    End Sub

    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

        Me.OnitLayout31.Busy = (nMod <> 0)

        If lb_warning.Text.Length = 0 Then
            Dim radd As Integer = New DataView(indisp_str, String.Empty, String.Empty, DataViewRowState.Added).Count
            Dim rmod As Integer = New DataView(indisp_str, String.Empty, String.Empty, DataViewRowState.ModifiedCurrent).Count
            Dim rdel As Integer = New DataView(indisp_str, String.Empty, String.Empty, DataViewRowState.Deleted).Count

            If radd > 0 OrElse rmod > 0 OrElse rdel > 0 Then
                Dim lst As New List(Of String)
                If radd > 0 Then lst.Add(String.Format("aggiunte: {0}", radd))
                If rmod > 0 Then lst.Add(String.Format(" modificate: {0}", rmod))
                If rdel > 0 Then lst.Add(String.Format("eliminate: {0}", rdel))
                lb_warning.Text = String.Format("Righe {0}", lst.Aggregate(Function(p, g) p & ", " & g))
            Else
                lb_warning.Text = String.Empty
            End If
        End If
        lb_warning.Visible = (lb_warning.Text.Length > 0)

    End Sub

#End Region

#Region " Datagrid Events "

    Private Sub dg_indisp_DeleteCommand(source As System.Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_indisp.DeleteCommand

        If CheckLock() Then
            Exit Sub
        End If

        'controlla se si è in fase 'editabile'
        If dg_indisp.EditItemIndex = -1 Then

            'calcola il numero di riga da eliminare
            Dim row As DataRow = indisp_str.Rows.Find(dg_indisp.DataKeys(e.Item.ItemIndex))
            row.Delete()

            dg_indisp.DataSource = indisp_str

            'calcola la pagina da visualizzare dopo l'eliminazione
            If dg_indisp.Items.Count = 1 Then
                If dg_indisp.CurrentPageIndex <> 0 Then
                    dg_indisp.CurrentPageIndex -= 1
                End If
            End If

            'ricarico il datagrid
            dg_indisp.DataBind()

            nMod += 1

        Else
            strJS = "<script language='javascript'>alert('Cliccare ANNULLA o AGGIORNA della riga che si sta modificando prima di eliminare questa riga')</script>"
            dg_indisp.DataSource = indisp_str
            dg_indisp.DataBind()
        End If

    End Sub

    Private Sub dg_indisp_EditCommand(source As System.Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_indisp.EditCommand

        If CheckLock() Then
            Exit Sub
        End If

        'controlla se si è in fase 'editabile'
        If dg_indisp.EditItemIndex = -1 Then

            'calcola il numero di riga da eliminare
            dg_indisp.EditItemIndex = e.Item.ItemIndex
            dg_indisp.DataSource = indisp_str
            dg_indisp.DataBind()

            Me.ToolBar.Enabled = False

            strJS = "<script language=""javascript"">document.getElementById(""dg_indisp"").rows[" & dg_indisp.EditItemIndex + 1 & "].cells[2].firstChild.firstChild.focus()</script>"

            Dim row As DataRow = indisp_str.Rows.Find(dg_indisp.DataKeys(e.Item.ItemIndex))
            itemTemp = row.ItemArray()

            nMod += 1

            Me.UpdateClientIdInserimento()

        Else
            strJS = "<script language='javascript'>alert('Cliccare ANNULLA o AGGIORNA della riga che si sta modificando prima di modificare questa riga')</script>"
            dg_indisp.SelectedIndex = -1
            dg_indisp.DataSource = indisp_str
            dg_indisp.DataBind()
        End If

    End Sub

    Private Sub dg_indisp_UpdateCommand(source As System.Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_indisp.UpdateCommand

        If CheckLock() Then
            Exit Sub
        End If

        Dim dpkDataIndisponibilita As OnitDatePick = DirectCast(e.Item.FindControl("tb_data_edit"), OnitDatePick)
        If String.IsNullOrEmpty(dpkDataIndisponibilita.Text) Then
            Me.OnitLayout31.InsertRoutineJS("alert(""Il campo 'Data' è vuoto. Non è possibile aggiornare la tabella."");")
            dg_indisp.DataSource = indisp_str
            dg_indisp.DataBind()
            Exit Sub
        End If

        Dim row As DataRow = indisp_str.Rows.Find(dg_indisp.DataKeys(e.Item.ItemIndex))

        row("fas_data") = dpkDataIndisponibilita.Text
        row("ric") = CType(e.Item.FindControl("cb_ric_edit"), CheckBox).Checked
        row("fas_descrizione") = CType(e.Item.FindControl("tb_desc_edit"), TextBox).Text
        row("fas_ora_inizio") = CType(e.Item.FindControl("tb_orainizio_edit"), TextBox).Text
        row("fas_ora_fine") = CType(e.Item.FindControl("tb_orafine_edit"), TextBox).Text()

        If row("ric") Then row("fas_data") = row("fas_data").substring(0, 5)

        Dim risCheck As ControlloInserimento = CheckDataRow(row)

        Select Case risCheck.Tipo

            Case ControlloInserimento.TipoErrore.NONE

                dg_indisp.EditItemIndex = -1
                dg_indisp.SelectedIndex = -1

                Me.ToolBar.Enabled = True
                newrow = False

            Case ControlloInserimento.TipoErrore.SOVRAPPOSIZIONE_INDISPONIBILITA

                Me.UpdateClientIdInserimento()
                Dim indexDataIndisponibilita As Integer = DatagridColumnIndex.DataIndisponibilita
                strJS = "<script language=""javascript""> alert(""Esiste già un'indisponibilità relativa allo stesso giorno. Non è possibile aggiornare la tabella!"");"
                strJS &= "document.getElementById(""dg_indisp"").rows[" & dg_indisp.EditItemIndex + 1 & "].cells[" + indexDataIndisponibilita.ToString() + "].firstChild.firstChild.focus()</script>"

            Case ControlloInserimento.TipoErrore.SOVRAPPOSIZIONE_FESTIVITA

                Me.UpdateClientIdInserimento()
                Dim indexDataIndisponibilita As Integer = DatagridColumnIndex.DataIndisponibilita
                strJS = "<script language=""javascript""> alert(""La data inserita è già una festività. Non è possibile aggiornare la tabella!"");"
                strJS &= "document.getElementById(""dg_indisp"").rows[" & dg_indisp.EditItemIndex + 1 & "].cells[" + indexDataIndisponibilita.ToString() + "].firstChild.firstChild.focus()</script>"

            Case ControlloInserimento.TipoErrore.PRESENZA_PRENOTATI

                Me.UpdateClientIdInserimento()
                Me.OnitLayout31.InsertRoutineJS(String.Format("alert('ATTENZIONE:{0}');", risCheck.Messaggio))

            Case Else

                Throw New NotSupportedException()

        End Select

        Me.dg_indisp.DataSource = indisp_str.DefaultView
        Me.dg_indisp.DataBind()

    End Sub

    Private Sub dg_indisp_CancelCommand(source As System.Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_indisp.CancelCommand

        If CheckLock() Then
            Exit Sub
        End If

        If newrow Then
            indisp_str.Rows(0).Delete()
            If (dg_indisp.Items.Count = 1) Then
                If (dg_indisp.CurrentPageIndex <> 0) Then
                    dg_indisp.CurrentPageIndex -= 1
                End If
            End If
            newrow = False
        Else
            Dim row As DataRow = indisp_str.Rows.Find(dg_indisp.DataKeys(e.Item.ItemIndex))
            row.ItemArray = itemTemp
        End If

        dg_indisp.EditItemIndex = -1
        dg_indisp.DataSource = indisp_str
        dg_indisp.DataBind()

        Me.ToolBar.Enabled = True

        nMod -= 1

    End Sub

    Private Sub dg_indisp_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dg_indisp.ItemDataBound

        If e.Item.ItemType <> ListItemType.Header And e.Item.ItemType <> ListItemType.Footer Then

            If Me.dg_indisp.EditItemIndex = -1 Then
                ' Grid in sola visualizzazione: le festività fisse non possono essere cancellate nè modificate
                Dim festivita As Boolean = DirectCast(e.Item.DataItem, System.Data.DataRowView).Row("festivita")
                If festivita Then
                    e.Item.Cells(DatagridColumnIndex.DeleteButton).Controls.Clear()
                    e.Item.Cells(DatagridColumnIndex.EditButton).Controls.Clear()
                End If
            Else
                ' Grid in edit
                e.Item.Cells(DatagridColumnIndex.DeleteButton).Controls.Clear()
                If e.Item.ItemIndex <> Me.dg_indisp.EditItemIndex Then
                    e.Item.Cells(DatagridColumnIndex.EditButton).Controls.Clear()
                End If
            End If

        End If

    End Sub

#End Region

#Region " Toolbar Events "

    Protected Sub ToolBar_ButtonClick(sender As Object, e As Telerik.Web.UI.RadToolBarEventArgs)

        Select Case e.Item.Value

            Case "btn_Salva"
                Salva()

            Case "btn_Inserisci"
                Inserisci()

            Case "btnIndisponibilitaPeriodiche"

                ' clear data
                odpDataInizio.Text = String.Empty
                odpDataFine.Text = String.Empty
                txtOraInizio.Text = String.Empty
                txtOraFine.Text = String.Empty
                txtMotivo.Text = String.Empty
                chkGioniSettimana.ClearSelection()

                dg_indisp.DataSource = indisp_str
                dg_indisp.DataBind()

                fmIndisponibilitaPeriodiche.VisibileMD = True

            Case "btn_Annulla"
                If CheckLock() Then Exit Sub

                indisp_str.RejectChanges()

                dg_indisp.DataSource = indisp_str
                dg_indisp.DataBind()

                nMod = 0

        End Select

    End Sub

    Protected Sub TolbarIndisponibilitaPeriodiche_ButtonClick(sender As Object, e As Telerik.Web.UI.RadToolBarEventArgs)

        Select Case e.Item.Value

            Case "btnConferma"

                Dim strb As String = Me.CheckData()
                If strb.Length > 0 Then
                    Dim msg As String = Me.ApplyEscapeJS(String.Format("ATTENZIONE:{0}{1}", Environment.NewLine, strb))
                    Me.OnitLayout31.InsertRoutineJS(String.Format("alert('{0}');", msg))
                    Return
                End If

                Dim lstGiorniSelezionati As New List(Of DateTime)
                Dim dataInizio As DateTime = odpDataInizio.Data
                Dim dataFine As DateTime = odpDataFine.Data

                For i As Integer = 0 To chkGioniSettimana.SelectedValues.Count - 1

                    Dim primoGiorno As DateTime = dataInizio.GetNextWeekday(chkGioniSettimana.SelectedValues(i))
                    While primoGiorno <= dataFine
                        lstGiorniSelezionati.Add(primoGiorno)
                        primoGiorno = primoGiorno.AddDays(7)
                    End While

                Next

                Dim lstSovrappostiIndisponibilita As New List(Of String)
                Dim lstSovrappostiFestivita As New List(Of String)
                Dim lstPresentiPrenotati As New List(Of String)

                lstGiorniSelezionati.Sort()
                lstGiorniSelezionati.Reverse()

                Dim dataInserimento As DateTime = DateTime.Now
                For i As Integer = 0 To lstGiorniSelezionati.Count - 1

                    'aggiungo nuova riga alla tabella
                    Dim row As DataRow = indisp_str.NewRow()
                    row("fas_data") = lstGiorniSelezionati(i).ToShortDateString()
                    row("fas_ora_inizio") = txtOraInizio.Text.PadLeft(5, "0")
                    row("fas_ora_fine") = txtOraFine.Text.PadLeft(5, "0")
                    row("ric") = False
                    row("festivita") = False
                    row("fas_descrizione") = txtMotivo.Text
                    row("key") = dataInserimento.AddTicks(i)

                    Dim risCheck As ControlloInserimento = CheckDataRow(row)

                    Select Case risCheck.Tipo

                        Case ControlloInserimento.TipoErrore.NONE

                            indisp_str.Rows.InsertAt(row, 0)

                        Case ControlloInserimento.TipoErrore.SOVRAPPOSIZIONE_INDISPONIBILITA

                            lstSovrappostiIndisponibilita.Add(row("fas_data"))

                        Case ControlloInserimento.TipoErrore.SOVRAPPOSIZIONE_FESTIVITA

                            lstSovrappostiFestivita.Add(row("fas_data"))

                        Case ControlloInserimento.TipoErrore.PRESENZA_PRENOTATI

                            lstPresentiPrenotati.Add(row("fas_data"))

                        Case Else

                            Throw New NotSupportedException()


                    End Select

                    nMod += 1

                Next


                If lstSovrappostiIndisponibilita.Count > 0 OrElse lstSovrappostiFestivita.Count > 0 OrElse lstPresentiPrenotati.Count > 0 Then

                    Dim stbr As New System.Text.StringBuilder("ATTENZIONE: le seguenti date non sono state inserite.")

                    If lstSovrappostiIndisponibilita.Count > 0 Then
                        If lstSovrappostiIndisponibilita.Count > 10 Then
                            lstSovrappostiIndisponibilita = lstSovrappostiIndisponibilita.Take(10).ToList()
                            lstSovrappostiIndisponibilita.Add("...")
                        End If
                        stbr.AppendLine("Esiste già un'indisponibilità relativa ai giorni:")
                        stbr.AppendLine(lstSovrappostiIndisponibilita.Aggregate(Function(p, g) p & Environment.NewLine & g))
                    End If

                    If lstSovrappostiFestivita.Count > 0 Then
                        If lstSovrappostiFestivita.Count > 10 Then
                            lstSovrappostiFestivita = lstSovrappostiFestivita.Take(10).ToList()
                            lstSovrappostiFestivita.Add("...")
                        End If
                        stbr.AppendLine("Le date inserite sono già una festività:")
                        stbr.AppendLine(lstSovrappostiFestivita.Aggregate(Function(p, g) p & Environment.NewLine & g))
                    End If

                    If lstPresentiPrenotati.Count > 0 Then
                        If lstPresentiPrenotati.Count > 10 Then
                            lstPresentiPrenotati = lstPresentiPrenotati.Take(10).ToList()
                            lstPresentiPrenotati.Add("...")
                        End If
                        stbr.AppendLine("Nelle date inserite sono già presenti prenotazioni:")
                        stbr.AppendLine(lstPresentiPrenotati.Aggregate(Function(p, g) p & Environment.NewLine & g))
                    End If

                    Me.OnitLayout31.InsertRoutineJS(String.Format("alert('{0}');", Me.ApplyEscapeJS(stbr.ToString())))

                End If

            Case "btnAnnulla"

        End Select

        dg_indisp.EditItemIndex = -1
        dg_indisp.DataSource = indisp_str
        dg_indisp.DataBind()

        fmIndisponibilitaPeriodiche.VisibileMD = False

    End Sub

#End Region

#Region " OnitLayout Events "

    Private Sub OnitLayout31_BusyChanged(sender As Object, e As System.EventArgs, state As Boolean) Handles OnitLayout31.BusyChanged

        If state Then
            Me.OnitLayout31.lock.Lock(0, CStr(codAmbulatorio))
        Else
            Me.OnitLayout31.lock.EndLock(0)
        End If

    End Sub

    Private Sub OnitLayout31_TimeoutLock(sender As Object, e As System.EventArgs, int As String) Handles OnitLayout31.TimeoutLock

        lb_warning.Text = "ATTENZIONE..IL BLOCCO SULL'APPLICAZIONE E' SCADUTO..CHIUDERE<BR> LA PAGINA SENZA SALVARE E RIPROVARE FRA QUALCHE MINUTO."

    End Sub

#End Region

#Region " Private "

    Private Class ControlloInserimento

        Public Enum TipoErrore As Short
            NONE = 0
            SOVRAPPOSIZIONE_INDISPONIBILITA = 1
            SOVRAPPOSIZIONE_FESTIVITA = 2
            PRESENZA_PRENOTATI = 3
        End Enum

        Property Tipo As TipoErrore
        Property Messaggio As String

    End Class

    Private Function CheckDataRow(ByRef row As DataRow) As ControlloInserimento

        Dim result As New ControlloInserimento

        For Each rowTemp As DataRow In indisp_str.Rows

            If Not rowTemp Is row AndAlso rowTemp.RowState <> DataRowState.Deleted Then

                If (rowTemp("fas_data").substring(0, 5) = row("fas_data").substring(0, 5)) Then

                    If rowTemp("festivita") Then
                        result.Tipo = ControlloInserimento.TipoErrore.SOVRAPPOSIZIONE_FESTIVITA

                    Else

                        If (rowTemp("fas_data").length > 5 And row("fas_data").length > 5) Then
                            If (rowTemp("fas_data").substring(6, 4) = row("fas_data").Substring(6, 4)) Then
                                result.Tipo = ControlloInserimento.TipoErrore.SOVRAPPOSIZIONE_INDISPONIBILITA
                            End If
                        Else
                            If (rowTemp("fas_data").length = 5 And row("fas_data").length = 5) Then
                                result.Tipo = ControlloInserimento.TipoErrore.SOVRAPPOSIZIONE_INDISPONIBILITA
                            End If
                        End If

                    End If

                End If

            End If

        Next

        Dim strDataNew As String = GetDataRicorsiva(row)
        Using dam As IDAM = OnVacUtility.OpenDam()

            Dim numeroPrenotati As Integer = GetNumeroPrenotati(strDataNew & " " & row("fas_ora_inizio"), strDataNew & " " & row("fas_ora_fine"), dam)
            If numeroPrenotati > 0 Then

                result.Tipo = ControlloInserimento.TipoErrore.PRESENZA_PRENOTATI
                result.Messaggio = Me.SetMessaggioErrorePrenotati(strDataNew, row, numeroPrenotati)

            End If

        End Using

        Return result

    End Function

    Public Function formattaData(data As String) As String

        If data.Length <= 5 Then
            Return CDate(data).ToString("dd/MM")
        Else
            Return CDate(data).ToString("dd/MM/yyyy")
        End If

    End Function

    Private Sub Inserisci()

        If CheckLock() Then
            Exit Sub
        End If

        newrow = True

        'aggiungo nuova riga alla tabella
        Dim row As DataRow = indisp_str.NewRow()
        row("fas_data") = "x"
        row("fas_ora_inizio") = ""
        row("ric") = False
        row("festivita") = False
        row("key") = DateTime.Now

        indisp_str.Rows.InsertAt(row, 0)
        dg_indisp.EditItemIndex = 0

        'ricarica la tabella nel datagrid
        dg_indisp.DataSource = indisp_str
        dg_indisp.DataBind()

        CType(dg_indisp.Items(dg_indisp.EditItemIndex).FindControl("tb_data_edit"), OnitDatePick).Text = ""

        Me.ToolBar.Enabled = False

        strJS = "<script language=""javascript"">document.getElementById(""dg_indisp"").rows(" & dg_indisp.EditItemIndex + 1 & ").cells(2).firstChild.firstChild.focus()</script>"

        nMod += 1

        Me.UpdateClientIdInserimento()

    End Sub

    Private Sub UpdateClientIdInserimento()

        tb_data_edit_ClientId = CType(dg_indisp.Items(dg_indisp.EditItemIndex).FindControl("tb_data_edit"), OnitDatePick).ClientID
        tb_oraInizio_edit_ClientId = CType(dg_indisp.Items(dg_indisp.EditItemIndex).FindControl("tb_orainizio_edit"), TextBox).ClientID
        tb_oraFine_edit_ClientId = CType(dg_indisp.Items(dg_indisp.EditItemIndex).FindControl("tb_orafine_edit"), TextBox).ClientID

    End Sub

    Private Sub Salva()

        Dim datiModificati As Boolean = False

        If CheckLock() Then
            Exit Sub
        End If

        Using DAM As IDAM = OnVacUtility.OpenDam()

            DAM.BeginTrans()

            Try

                For i As Integer = indisp_str.Rows.Count - 1 To 0 Step -1

                    Dim row As DataRow = indisp_str.Rows(i)

                    Trace.Warn("Save: Indisponibilita " & row.RowState.ToString())

                    Select Case row.RowState

                        Case DataRowState.Added

                            ' Ricontrollo al salvataggio
                            Dim risCheck As ControlloInserimento = CheckDataRow(row)
                            If risCheck.Tipo = ControlloInserimento.TipoErrore.NONE Then

                                Dim strDataNew As String = GetDataRicorsiva(row)
                                With DAM.QB
                                    .NewQuery()
                                    .AddTables("t_ana_fasce_indisponibilita")
                                    .AddInsertField("fas_amb_codice", codAmbulatorio, DataTypes.Numero)
                                    .AddInsertField("fas_data", strDataNew, DataTypes.Data)
                                    .AddInsertField("fas_ora_inizio", strDataNew & " " & row("fas_ora_inizio"), DataTypes.DataOra)
                                    .AddInsertField("fas_ora_fine", strDataNew & " " & row("fas_ora_fine"), DataTypes.DataOra)
                                    .AddInsertField("fas_descrizione", row("fas_descrizione").Replace("'", "''") & "", DataTypes.Stringa)
                                End With

                                DAM.ExecNonQuery(ExecQueryType.Insert)

                            Else
                                datiModificati = True
                            End If

                        Case DataRowState.Deleted

                            row.RejectChanges()

                            Dim strDataNew As String = GetDataRicorsiva(row)

                            With DAM.QB
                                .NewQuery()
                                .AddTables("t_ana_fasce_indisponibilita")
                                .AddWhereCondition("fas_data", Comparatori.Uguale, strDataNew, DataTypes.Data)
                                .AddWhereCondition("fas_ora_inizio", Comparatori.Uguale, strDataNew & " " & row("fas_ora_inizio"), DataTypes.DataOra)
                                .AddWhereCondition("fas_amb_codice", Comparatori.Uguale, codAmbulatorio, DataTypes.Numero)
                            End With

                            DAM.ExecNonQuery(ExecQueryType.Delete)

                            row.Delete()

                        Case DataRowState.Modified

                            ' Ricontrollo al salvataggio
                            Dim risCheck As ControlloInserimento = CheckDataRow(row)
                            If risCheck.Tipo = ControlloInserimento.TipoErrore.NONE Then

                                Dim strDataOld As String
                                Dim strDataNew As String = GetDataRicorsiva(row)

                                If (row("RIC", DataRowVersion.Original) = True) Then
                                    strDataOld = row("fas_data", DataRowVersion.Original) & "/" & Constants.CommonConstants.RECURSIVE_YEAR
                                Else
                                    strDataOld = row("fas_data", DataRowVersion.Original)
                                End If

                                With DAM.QB
                                    .NewQuery()
                                    .AddTables("t_ana_fasce_indisponibilita")
                                    .AddUpdateField("fas_amb_codice", codAmbulatorio, DataTypes.Stringa)
                                    .AddUpdateField("fas_data", strDataNew, DataTypes.Data)
                                    .AddUpdateField("fas_descrizione", row("fas_descrizione").Replace("'", "''") & "", DataTypes.Stringa)
                                    .AddUpdateField("fas_ora_inizio", strDataNew & " " & row("fas_ora_inizio"), DataTypes.DataOra)
                                    .AddUpdateField("fas_ora_fine", strDataNew & " " & row("fas_ora_fine"), DataTypes.DataOra)
                                    .AddWhereCondition("fas_data", Comparatori.Uguale, strDataOld, DataTypes.Data)
                                    .AddWhereCondition("fas_ora_inizio", Comparatori.Uguale, strDataOld & " " & row("fas_ora_inizio", DataRowVersion.Original), DataTypes.DataOra)
                                    .AddWhereCondition("fas_amb_codice", Comparatori.Uguale, codAmbulatorio, DataTypes.Stringa)
                                End With

                                DAM.ExecNonQuery(ExecQueryType.Update)
                            Else
                                datiModificati = True
                            End If

                    End Select

                Next

                DAM.Commit()

            Catch exc As Exception

                DAM.Rollback()

                exc.InternalPreserveStackTrace()
                Throw

            End Try

            indisp_str = BuildTableIndisponibilita(DAM)

        End Using

        dg_indisp.DataSource = indisp_str
        dg_indisp.DataBind()

        nMod = 0

        If datiModificati Then
            Me.OnitLayout31.InsertRoutineJS(String.Format("alert('{0}');", Me.ApplyEscapeJS("Non tutti i dati sono stati salvati a causa di modifiche effettuate da altri utenti.")))
        End If

    End Sub

    Private Function GetDataRicorsiva(row As DataRow) As String

        If (row("ric") = True) Then
            Return row("fas_data") & "/" & Constants.CommonConstants.RECURSIVE_YEAR
        End If

        Return row("fas_data")

    End Function

    Private Function CheckLock() As Boolean

        If (OnitLayout31.lock.IsLocked(0) AndAlso codAmbulatorio = OnitLayout31.lock.GetLockInfo(0).Info) Then
            lb_warning.Text = "ATTENZIONE... L'APPLICAZIONE E' BLOCCATA DA " & OnitLayout31.lock.GetLockInfo(0).UserCodice & ".<BR>RIPROVARE FRA QUALCHE MINUTO."
            Return True
        End If

        Return False

    End Function

    Private Function BuildTableIndisponibilita(ByRef dam As IDAM) As DataTable

        Dim dt As New DataTable()
        dt.Columns.Add(New DataColumn("key", GetType(DateTime)))
        dt.Columns.Add(New DataColumn("festivita", GetType(Boolean)))
        dt.Columns.Add(New DataColumn("ric", GetType(Boolean)))
        dt.Columns.Add(New DataColumn("fas_data", GetType(String)))
        dt.Columns.Add(New DataColumn("fas_descrizione", GetType(String)))
        dt.Columns.Add(New DataColumn("fas_ora_inizio", GetType(String)))
        dt.Columns.Add(New DataColumn("fas_ora_fine", GetType(String)))

        With dam.QB
            .NewQuery()
            .AddSelectFields("fas_data", "fas_descrizione", "fas_ora_inizio", "fas_ora_fine")
            .AddTables("t_ana_fasce_indisponibilita")
            .AddWhereCondition("fas_amb_codice", Comparatori.Uguale, codAmbulatorio, DataTypes.Numero)
        End With

        Using idr As IDataReader = dam.BuildDataReader()
            If Not idr Is Nothing Then

                For j As Integer = 0 To idr.FieldCount - 1
                    If Not dt.Columns.Contains(idr.GetName(j)) Then
                        dt.Columns.Add(idr.GetName(j), idr.GetFieldType(j))
                    End If
                Next

                Dim fasOraInizio As Integer = idr.GetOrdinal("fas_ora_inizio")
                Dim fasOraFine As Integer = idr.GetOrdinal("fas_ora_fine")
                Dim fasDescrizione As Integer = idr.GetOrdinal("fas_descrizione")
                Dim fasData As Integer = idr.GetOrdinal("fas_data")

                While idr.Read()

                    Dim dr As DataRow = dt.NewRow()
                    Dim dataInizio As DateTime = CDate(idr(fasData))
                    Dim oraInizio As DateTime = CDate(idr(fasOraInizio))

                    dr("festivita") = False
                    dr("fas_ora_inizio") = oraInizio.ToShortTimeString()
                    dr("fas_ora_fine") = idr(fasOraFine).ToShortTimeString()
                    dr("fas_descrizione") = idr(fasDescrizione)
                    If (idr(fasData).Year.ToString() = Constants.CommonConstants.RECURSIVE_YEAR) Then
                        dr("fas_data") = dataInizio.ToString("dd/MM")
                        dr("ric") = True
                    Else
                        dr("fas_data") = dataInizio.ToString("dd/MM/yyyy")
                        dr("ric") = False
                    End If
                    dr("key") = Me.GetDataOraInizio(dataInizio, oraInizio)

                    dt.Rows.Add(dr)

                End While

            End If
        End Using

        With dam.QB
            .NewQuery()
            .AddSelectFields("fes_data", "fes_descrizione")
            .AddTables("t_ana_festivita")
        End With

        Using idr As IDataReader = dam.BuildDataReader()

            If Not idr Is Nothing Then

                Dim fasDescrizione As Integer = idr.GetOrdinal("fes_descrizione")
                Dim fasData As Integer = idr.GetOrdinal("fes_data")

                While idr.Read()

                    Dim dr As DataRow = dt.NewRow()
                    Dim dataInizio As DateTime = CDate(idr(fasData))
                    Dim oraInizio As DateTime = DateTime.MinValue
                    dr("festivita") = True
                    dr("fas_descrizione") = idr(fasDescrizione)
                    If (idr(fasData).Year.ToString() = Constants.CommonConstants.RECURSIVE_YEAR) Then
                        dr("fas_data") = dataInizio.ToString("dd/MM")
                        dr("ric") = True
                    Else
                        dr("fas_data") = dataInizio.ToString("dd/MM/yyyy")
                        dr("ric") = False
                    End If
                    dr("key") = Me.GetDataOraInizio(dataInizio, oraInizio)

                    dt.Rows.Add(dr)

                End While

            End If
        End Using

        ' Ordinamento tabella
        Dim dt1 As DataTable = dt.Clone()
        Dim dv As DataView

        dv = New DataView(dt)
        dv.RowFilter = "ric = true"
        dv.Sort = "key"

        For i As Integer = 0 To dv.Count - 1
            dt1.ImportRow(dv(i).Row)
        Next

        dv = New DataView(dt)
        dv.RowFilter = "ric = false"
        dv.Sort = "key desc"

        For i As Integer = 0 To dv.Count - 1
            dt1.ImportRow(dv(i).Row)
        Next

        dt1.AcceptChanges()

        'dt.DefaultView.Sort = "ric desc, key"
        'dt = dt.DefaultView.ToTable()
        'dt.AcceptChanges()

        Dim key(0) As DataColumn
        key(0) = dt1.Columns("key")
        dt1.PrimaryKey = key

        Return dt1

    End Function


    Private Function GetDataOraInizio(dataInizio As DateTime, oraInizio As DateTime) As DateTime

        Return New DateTime(dataInizio.Year, dataInizio.Month, dataInizio.Day, oraInizio.Hour, oraInizio.Minute, oraInizio.Second)

    End Function

    Private Function GetNumeroPrenotati(dataInizio As Date, dataFine As Date, ByRef dam As IDAM) As Integer

        Dim _result As Integer

        With dam.QB
            .NewQuery()
            .AddSelectFields("COUNT(*)")
            .AddTables("T_CNV_CONVOCAZIONI")
            .AddWhereCondition("CNV_DATA_APPUNTAMENTO", Comparatori.MaggioreUguale, dataInizio, DataTypes.DataOra)
            .AddWhereCondition("CNV_DATA_APPUNTAMENTO", Comparatori.MinoreUguale, dataFine, DataTypes.DataOra)
            .AddWhereCondition("CNV_AMB_CODICE", Comparatori.Uguale, codAmbulatorio, DataTypes.Stringa)
        End With

        _result = dam.ExecScalar()

        Return _result

    End Function

    Private Function SetMessaggioErrorePrenotati(strDataNew As String, row As DataRow, numeroPrenotati As Integer)

        Return String.Format("\nNel giorno {0} dalle {1} alle {2} sono già prenotati {3} pazienti.", strDataNew, row("fas_ora_inizio"), row("fas_ora_fine"), numeroPrenotati)

    End Function

    Public Function IsValidTime(time As String) As Boolean

        Dim checktime As New System.Text.RegularExpressions.Regex("^(20|21|22|23|[01]\d|\d)(([.][0-5]\d){1,2})$")
        Return checktime.IsMatch(time)

    End Function

    Private Function CheckData() As String

        Dim strb As New System.Text.StringBuilder()

        ' 1a fase di controlli
        If String.IsNullOrEmpty(odpDataInizio.Text) Then strb.AppendLine("Il campo 'Periodo da' è vuoto.")
        If String.IsNullOrEmpty(odpDataFine.Text) Then strb.AppendLine("Il campo 'Periodo a' è vuoto.")
        If txtMotivo.Text.ToString().Length() > 30 Then strb.AppendLine("Il campo 'descrizione' non può contenere più di 30 caratteri.")
        If chkGioniSettimana.SelectedItems.Count = 0 Then strb.AppendLine("Non è stato selezionato nessun giorno della settimana.")

        If strb.Length > 0 Then Return strb.ToString()

        ' 2a fase di controlli
        If odpDataInizio.Data < DateTime.Today Then strb.AppendLine("Il campo 'Periodo da' contiene un giorno già passato.")
        If odpDataInizio.Data > odpDataFine.Data Then strb.AppendLine("La data di inizio del periodo è maggiore della data di fine.")

        If String.IsNullOrEmpty(txtOraInizio.Text) Then txtOraInizio.Text = "00.00"
        If Not Me.IsValidTime(txtOraInizio.Text) Then strb.AppendLine("L'orario di inizio non è valido o non è nel formato corretto (hh.mm).")

        If String.IsNullOrEmpty(txtOraFine.Text) Then txtOraFine.Text = "23.59"
        If Not Me.IsValidTime(txtOraFine.Text) Then strb.AppendLine("L'orario di fine non è valido o non è nel formato corretto (hh.mm).")

        If strb.Length > 0 Then Return strb.ToString()

        ' 3a fase di controlli
        Dim oraInizio As New DateTime(1, 1, 1)
        Dim oraFine As New DateTime(1, 1, 1)

        oraInizio = oraInizio.AddHours(txtOraInizio.Text.Split(".")(0)).AddMinutes(txtOraInizio.Text.Split(".")(1))
        oraFine = oraFine.AddHours(txtOraFine.Text.Split(".")(0)).AddMinutes(txtOraFine.Text.Split(".")(1))

        If oraInizio > oraFine Then strb.AppendLine("L'ora di inizio del periodo è maggiore dell'ora di fine.")

        Return strb.ToString()

    End Function

#End Region

End Class
