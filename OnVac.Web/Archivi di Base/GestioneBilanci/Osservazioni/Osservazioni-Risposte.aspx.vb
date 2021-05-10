Imports Onit.Database.DataAccessManager

Partial Class Osservazioni_Risposte
    Inherits OnVac.Common.PageBase
    Protected WithEvents lblTitolo3 As System.Web.UI.WebControls.Label

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

    Property Codice() As String
        Get
            If ViewState("Codice") Is Nothing Then Return String.Empty
            Return ViewState("Codice").ToString()
        End Get
        Set(ByVal Value As String)
            ViewState("Codice") = Value
        End Set
    End Property

    Property dtaAssociate() As DataTable
        Get
            Return DirectCast(Session("dtaRisposteAssociate"), DataTable)
        End Get
        Set(ByVal Value As DataTable)
            Session("dtaRisposteAssociate") = Value
        End Set
    End Property

    Property dtaRisposte() As DataTable
        Get
            Return DirectCast(Session("dtaRisposte"), DataTable)
        End Get
        Set(ByVal Value As DataTable)
            Session("dtaRisposte") = Value
        End Set
    End Property

#End Region

    Protected Sub InitSession()

        If Not dtaRisposte Is Nothing Then dtaRisposte = Nothing
        If Not dtaAssociate Is Nothing Then dtaAssociate = Nothing

        dtaRisposte = New DataTable()
        dtaAssociate = New DataTable()

    End Sub

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            InitSession()

            Dim separator As Char = "|"c

            If Not Request.QueryString("CODICE") Is Nothing Then
                Codice = Request.QueryString("CODICE").Split(separator)(0)
                titolo.InnerText = Request.QueryString("CODICE").Split(separator)(1)
            Else
                lblTitolo.Text = "Eseguire la pagina passando il codice dell'osservazione!"
            End If

            Using DAM As IDAM = OnVacUtility.OpenDam()
                LoadRisposteAssociate(DAM)
                LoadRisposte(DAM)
            End Using

        End If

    End Sub

    Protected Sub LoadRisposte(DAM As IDAM)

        DAM.QB.NewQuery()
        DAM.QB.AddSelectFields("RIS_CODICE", "RIS_DESCRIZIONE")
        DAM.QB.AddTables("T_ANA_RISPOSTE")
        DAM.QB.AddWhereCondition("RIS_OBSOLETO", Comparatori.Diverso, "S", DataTypes.Stringa)
        DAM.BuildDataTable(dtaRisposte)

        dtaRisposte.PrimaryKey = New DataColumn() {dtaRisposte.Columns(0)}

        Dim r As DataRow
        For i As Integer = 0 To dtaAssociate.Rows.Count - 1
            If dtaAssociate.Rows(i).RowState <> DataRowState.Deleted Then
                r = dtaRisposte.Rows.Find(dtaAssociate.Rows(i).Item("RIO_RIS_CODICE"))
                If Not r Is Nothing Then
                    r.Delete()
                End If
            End If
        Next
        dtaRisposte.AcceptChanges()

        'ordinamento risposte secondo codice (modifica 29/06/2004)
        Dim dwRisposte As DataView = dtaRisposte.DefaultView
        dwRisposte.Sort = "RIS_CODICE"

        dlsRisposte.DataSource = dwRisposte
        dlsRisposte.DataBind()

    End Sub

    Protected Sub LoadRisposteAssociate(DAM As IDAM)

        DAM.QB.NewQuery()
        DAM.QB.AddSelectFields("RIO_OSS_CODICE", "RIO_RIS_CODICE", "RIO_N_RISPOSTA", "RIO_DEFAULT", "RIS_DESCRIZIONE")
        DAM.QB.AddTables("T_ANA_RISPOSTE", "T_ANA_LINK_OSS_RISPOSTE")
        DAM.QB.AddWhereCondition("RIO_RIS_CODICE", Comparatori.Uguale, "RIS_CODICE", DataTypes.Join)
        DAM.QB.AddWhereCondition("RIO_OSS_CODICE", Comparatori.Uguale, Codice, DataTypes.Stringa)
        DAM.BuildDataTable(dtaAssociate)

        dtaAssociate.DefaultView.Sort = "RIO_N_RISPOSTA"
        dlsRisposteAssociate.DataSource = dtaAssociate.DefaultView
        dlsRisposteAssociate.DataBind()

        Dim columnArray1 As DataColumn() = New DataColumn() {dtaAssociate.Columns.Item("RIO_RIS_CODICE")}
        
        Me.dtaAssociate.PrimaryKey = columnArray1

    End Sub

    Protected Sub AddRisposta(Cod As String, DAM As IDAM)

        Dim MaxNum As Integer = 0
        Dim numRisp As Integer = 0
        For i As Integer = 0 To dtaAssociate.Rows.Count - 1
            If (dtaAssociate.Rows(i).RowState <> DataRowState.Deleted) Then
                numRisp = GetIntValue(dtaAssociate.Rows(i)("RIO_N_RISPOSTA"))
                If numRisp > MaxNum Then MaxNum = numRisp
            End If
        Next

        DAM.QB.NewQuery()
        DAM.QB.AddSelectFields("RIS_DESCRIZIONE")
        DAM.QB.AddTables("T_ANA_RISPOSTE")
        DAM.QB.AddWhereCondition("RIS_CODICE", Comparatori.Uguale, Cod, DataTypes.Stringa)
        Dim obj As Object = DAM.ExecScalar()

        Dim Descrizione As String = String.Empty
        If (Not obj Is Nothing) Then Descrizione = obj.ToString()

        Dim r As DataRow = dtaAssociate.NewRow
        r("RIO_OSS_CODICE") = Codice
        r("RIO_RIS_CODICE") = Cod
        r("RIO_N_RISPOSTA") = MaxNum + 1
        r("RIS_DESCRIZIONE") = Descrizione
        r("RIO_DEFAULT") = "N"
        dtaAssociate.Rows.Add(r)

        'r = dtaAssociateVisualizza.NewRow
        'r("RIO_OSS_CODICE") = Codice
        'r("RIO_RIS_CODICE") = Cod
        'r("RIO_N_RISPOSTA") = MaxNum + 1
        'r("RIS_DESCRIZIONE") = Descrizione
        'r("RIO_DEFAULT") = "N"
        'dtaAssociateVisualizza.Rows.Add(r)

        OnitLayout31.Busy = True

    End Sub

    Protected Sub DelRisposta(Cod As String, DAM As IDAM)

        dtaAssociate.Select("RIO_RIS_CODICE = '" + Cod + "'", "", DataViewRowState.CurrentRows)(0).Delete()

        OnitLayout31.Busy = True

    End Sub

    Protected Sub AggiornaDtOsservazioniAssociate()

        Dim val As String = String.Empty
        Dim strPos As String = String.Empty
        Dim intPos As Integer = 0

        For i As Integer = 0 To dlsRisposteAssociate.Items.Count - 1
            val = CType(dlsRisposteAssociate.Items(i).FindControl("lblCodice"), TextBox).Text

            intPos = GetIntValue(CType(dlsRisposteAssociate.Items(i).FindControl("txtPosizione"), TextBox).Text)

            For t As Integer = 0 To dtaAssociate.Rows.Count - 1
                If (dtaAssociate.Rows(t).RowState <> DataRowState.Deleted) Then
                    Dim numRisp As Integer = GetIntValue(dtaAssociate.Rows(t)("RIO_N_RISPOSTA"))

                    If dtaAssociate.Rows(t)("RIO_RIS_CODICE").ToString() = val And numRisp <> intPos Then
                        dtaAssociate.Rows(t)("RIO_N_RISPOSTA") = intPos
                        Exit For
                    End If
                End If
            Next
        Next

    End Sub

    Private Function GetIntValue(ByVal value As String) As Integer
        If (value = String.Empty) Then Return 0

        Dim intVal As Integer = 0
        Try
            intVal = Convert.ToInt32(value)
        Catch ex As Exception
            intVal = 0
        End Try

        Return intVal
    End Function

    Private Function GetIntValue(ByVal value As Object) As Integer
        If (value Is Nothing) Then Return 0

        Dim intVal As Integer = 0
        Try
            intVal = Convert.ToInt32(value)
        Catch ex As Exception
            intVal = 0
        End Try

        Return intVal
    End Function

    Private Sub ToolBar_ButtonClicked(ByVal sender As Object, ByVal e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Dim i As Integer

        Dim DAM As IDAM = Nothing

        Try
            DAM = OnVacUtility.OpenDam()

            Select Case e.Button.Key

                Case "btnAggiungi"

                    AggiornaDtOsservazioniAssociate()

                    For i = 0 To dlsRisposte.Items.Count - 1
                        If DirectCast(dlsRisposte.Items(i).FindControl("chkSeleziona"), CheckBox).Checked Then
                            AddRisposta(CType(dlsRisposte.Items(i).FindControl("lblCodice"), Label).Text, DAM)
                        End If
                    Next

                Case "btnElimina"

                    AggiornaDtOsservazioniAssociate()

                    For i = 0 To dlsRisposteAssociate.Items.Count - 1
                        If DirectCast(dlsRisposteAssociate.Items(i).FindControl("chkSeleziona"), CheckBox).Checked Then
                            DelRisposta(CType(dlsRisposteAssociate.Items(i).FindControl("lblCodice"), TextBox).Text, DAM)
                        End If
                    Next

                Case "btnSalva"

                    AggiornaDtOsservazioniAssociate()

                    ' Osservazioni Aggiunte
                    If (dtaAssociate.Select("", "", DataViewRowState.Added).Length > 0) Then
                        log_modifche_osservazioni(dtaAssociate.Select("", "", DataViewRowState.Added), "1")
                    End If

                    'Osservazioni Eliminate
                    If (dtaAssociate.Select("", "", DataViewRowState.Deleted).Length > 0) Then
                        log_modifche_osservazioni(dtaAssociate.Select("", "", DataViewRowState.Deleted), "0")
                    End If

                    Dim err As String = String.Empty

                    DAM.QB.NewQuery()
                    DAM.QB.AddSelectFields("RIO_OSS_CODICE", "RIO_RIS_CODICE", "RIO_N_RISPOSTA", "RIO_DEFAULT")
                    DAM.QB.AddTables("T_ANA_LINK_OSS_RISPOSTE")
                    DAM.UpdateTable(True, True, True, dtaAssociate, True, err)

                    dtaAssociate.Clear()
                    '    dtaAssociateVisualizza.Clear()
                    LoadRisposteAssociate(DAM)

                    If err <> String.Empty Then
                        Throw New Exception(err)
                    Else
                        OnitLayout31.InsertRoutineJS("alert('Salvataggio effettuato');")
                    End If

                    OnitLayout31.Busy = False

                Case "btnAnnulla"
                    dtaAssociate.Clear()
                    '  dtaAssociateVisualizza.Clear()
                    LoadRisposteAssociate(DAM)

                    OnitLayout31.Busy = False

                Case "btnDefault"

                    AggiornaDtOsservazioniAssociate()

                    Dim checkedRows As Integer = 0
                    Dim checkedKey As String = String.Empty

                    For i = 0 To dlsRisposteAssociate.Items.Count - 1
                        If (DirectCast(dlsRisposteAssociate.Items.Item(i).FindControl("chkSeleziona"), CheckBox).Checked) Then
                            checkedRows = checkedRows + 1
                            checkedKey = CType(dlsRisposteAssociate.Items.Item(i).FindControl("lblCodice"), TextBox).Text
                         End If
                    Next

                    Select Case checkedRows
                        Case 0
                            OnitLayout31.InsertRoutineJS("alert('Attenzione: " & ChrW(232) & " necessario specificare una risposta associata per impostarla come predefinita!')")
                        Case 1
                            For i = 0 To dtaAssociate.Rows.Count - 1
                                If (dtaAssociate.Rows(i).RowState <> DataRowState.Deleted) Then
                                    If dtaAssociate.Rows(i)("RIO_RIS_CODICE") = checkedKey Then
                                        If (dtaAssociate.Rows(i)("RIO_DEFAULT") = "S") Then
                                            dtaAssociate.Rows(i)("RIO_DEFAULT") = "N"
                                        Else
                                            dtaAssociate.Rows(i)("RIO_DEFAULT") = "S"
                                        End If
                                    Else
                                        dtaAssociate.Rows(i)("RIO_DEFAULT") = "N"
                                    End If
                                End If
                            Next

                        Case Else
                            OnitLayout31.InsertRoutineJS("alert('Attenzione: " & ChrW(232) & " necessario specificare una sola risposta associata per impostarla come predefinita!')")
                    End Select

            End Select

            LoadRisposte(DAM)

            dtaAssociate.DefaultView.Sort = "RIO_N_RISPOSTA"
            dlsRisposteAssociate.DataSource = dtaAssociate
            dlsRisposteAssociate.DataBind()

        Finally
            OnVacUtility.CloseDam(DAM)
        End Try

    End Sub

    Private Sub log_modifche_osservazioni(ByVal select_row() As DataRow, ByVal switch As String)

        Dim LOG_DAM As IDAM
        Dim BIL_DAM As IDAM
        Dim dt_bilanci As DataTable
        Dim switch_row As DataRowVersion
        Dim now As Date
        Dim id_mod_bilanci As Integer


        If (switch = "0") Then
            switch_row = DataRowVersion.Original
        Else
            switch_row = DataRowVersion.Current
        End If


        For i As Integer = 0 To (select_row.Length - 1)
            now = System.DateTime.Now()

            BIL_DAM = OnVacUtility.OpenDam()

            BIL_DAM.QB.NewQuery()
            BIL_DAM.QB.AddSelectFields("OSB_BIL_N_BILANCIO,OSB_BIL_MAL_CODICE")
            BIL_DAM.QB.AddTables("T_ANA_LINK_OSS_BILANCI")
            BIL_DAM.QB.AddWhereCondition("OSB_OSS_CODICE", Comparatori.Uguale, "'" + select_row(i)("RIO_OSS_CODICE", switch_row).ToString() + "'", DataTypes.Join)

            Try
                dt_bilanci = New DataTable()
                BIL_DAM.BuildDataTable(dt_bilanci)
            Finally
                OnVacUtility.CloseDam(BIL_DAM)
            End Try

            LOG_DAM = OnVacUtility.OpenDam()


            Try
                LOG_DAM.BeginTrans()

                LOG_DAM.QB.NewQuery()
                LOG_DAM.QB.AddSelectFields("SEQ_LOG_MOD_BILANCI.nextval")
                LOG_DAM.QB.AddTables("DUAL")

                id_mod_bilanci = GetIntValue(LOG_DAM.ExecScalar())

                LOG_DAM.QB.NewQuery()
                LOG_DAM.QB.AddTables("T_LOG_MOD_BILANCI")
                LOG_DAM.QB.AddInsertField("tlm_id", id_mod_bilanci, DataTypes.Numero)
                LOG_DAM.QB.AddInsertField("tlm_data", now, DataTypes.DataOra)
                LOG_DAM.QB.AddInsertField("tlm_ute_id", OnVacContext.UserId, DataTypes.Stringa)
                LOG_DAM.QB.AddInsertField("tlm_oss_codice", select_row(i)("RIO_OSS_CODICE", switch_row), DataTypes.Stringa)
                LOG_DAM.QB.AddInsertField("tlm_ris_codice", select_row(i)("RIO_RIS_CODICE", switch_row), DataTypes.Stringa)
                LOG_DAM.QB.AddInsertField("tlm_operazione", switch, DataTypes.Stringa)

                LOG_DAM.ExecNonQuery(ExecQueryType.Insert)


                For j As Integer = 0 To dt_bilanci.Rows.Count - 1
                    LOG_DAM.QB.NewQuery()
                    LOG_DAM.QB.AddTables("T_LOG_LINK_MOD_BILANCI")
                    LOG_DAM.QB.AddInsertField("tlb_tlm_id", id_mod_bilanci, DataTypes.Numero)
                    LOG_DAM.QB.AddInsertField("tlb_bil_numero", dt_bilanci.Rows(j)("OSB_BIL_N_BILANCIO"), DataTypes.Numero)
                    LOG_DAM.QB.AddInsertField("tlb_mal_codice", dt_bilanci.Rows(j)("OSB_BIL_MAL_CODICE"), DataTypes.Stringa)

                    LOG_DAM.ExecNonQuery(ExecQueryType.Insert)
                Next

                LOG_DAM.Commit()

            Catch exc As Exception
                LOG_DAM.Rollback()
                exc.InternalPreserveStackTrace()
                Throw
            Finally
                OnVacUtility.CloseDam(LOG_DAM)
                dt_bilanci.Dispose()
            End Try

        Next

    End Sub

    Private Sub dlsRisposteAssociate_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataListItemEventArgs) Handles dlsRisposteAssociate.ItemDataBound

        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then

            DirectCast(e.Item.FindControl("lblCodice"), WebControl).Attributes.Add("readOnly", "true")
            DirectCast(e.Item.FindControl("txtPosizione"), WebControl).Attributes.Add("readOnly", "true")
            If e.Item.DataItem("RIO_DEFAULT") Is Nothing OrElse Not e.Item.DataItem("RIO_DEFAULT") = "S" Then
                DirectCast(e.Item.FindControl("imgDefault"), WebControl).Style.Add("display", "none")
            End If

        End If

    End Sub

End Class
