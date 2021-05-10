Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.Web.UI.WebControls.Validators


Partial Class OnVac_VacRifiuti
    Inherits OnVac.Common.PageBase

#Region " Fields "

    Protected WithEvents uscInsRifiuto As OnVac.InsRifiuto

#End Region

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

    Property rowKey() As Object()
        Get
            Return Session("OnVac_rowKey")
        End Get
        Set(ByVal Value As Object())
            Session("OnVac_rowKey") = Value
        End Set
    End Property

    Property nMod() As Int16
        Get
            Return Session("OnVac_Rifiuti_nMod")
        End Get
        Set(ByVal Value As Int16)
            Session("OnVac_Rifiuti_nMod") = Value
        End Set
    End Property

    Property openPopUp() As Boolean
        Get
            Return Session("OnVac_Rifiuti_openPopUp")
        End Get
        Set(ByVal Value As Boolean)
            Session("OnVac_Rifiuti_openPopUp") = Value
        End Set
    End Property

    Property dt_Rifiuti() As DataTable
        Get
            Return Session("OnVac_dt_Rifiuti")
        End Get
        Set(ByVal Value As DataTable)
            Session("OnVac_dt_Rifiuti") = Value
        End Set
    End Property

    'messaggio visualizzato all'eliminazione della programmazione
    Public Property msgElimProg() As String
        Get
            Return Session("InadempienzeElim")
        End Get
        Set(ByVal Value As String)
            Session("InadempienzeElim") = Value
        End Set
    End Property

#End Region

#Region " Public "

    Public strJS As String

#End Region

#Region " Page Events "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack() Then

            openPopUp = False

            If Not dt_Rifiuti Is Nothing Then dt_Rifiuti.Dispose()
            dt_Rifiuti = New DataTable()

            Using DAM As IDAM = OnVacUtility.OpenDam()

                OnVacUtility.ImpostaIntestazioniPagina(Me.OnitLayout31, Me.LayoutTitolo, Nothing, Me.Settings, Me.IsGestioneCentrale)

                DAM.QB.NewQuery()
                DAM.QB.AddTables("T_PAZ_RIFIUTI,T_ANA_VACCINAZIONI,T_ANA_UTENTI")
                DAM.QB.AddSelectFields("prf_paz_codice", "prf_vac_codice", "prf_data_rifiuto", "prf_note_rifiuto", _
                                        "prf_ute_id", "prf_n_richiamo", "prf_genitore", "vac_descrizione", "ute_descrizione")
                DAM.QB.AddWhereCondition("prf_paz_codice", Comparatori.Uguale, OnVacUtility.Variabili.PazId, DataTypes.Numero)
                DAM.QB.AddWhereCondition("prf_vac_codice", Comparatori.Uguale, "vac_codice", DataTypes.Join)
                DAM.QB.AddWhereCondition("prf_ute_id", Comparatori.Uguale, "ute_id", DataTypes.Join)
                DAM.QB.AddOrderByFields("prf_data_rifiuto")

                DAM.BuildDataTable(Me.dt_Rifiuti)

                Me.dt_Rifiuti.DefaultView.Sort = "prf_data_rifiuto DESC"

                Me.dg_VacInad.DataSource = Me.dt_Rifiuti.DefaultView
                Me.dg_VacInad.DataBind()

                OnVacUtility.addKey(Me.dt_Rifiuti, "prf_paz_codice", "prf_vac_codice")

            End Using

            'il messaggio di eliminazione programmazione deve essere vuoto
            msgElimProg = String.Empty

            ShowPrintButtons()

        End If

        Select Case Request.Form("__EVENTTARGET")

            Case "ConfermaInsVac"

                nMod += 1

                Me.OnitLayout31.Busy = True

                Me.dt_Rifiuti.DefaultView.Sort = "prf_data_rifiuto DESC"

                Me.dg_VacInad.DataSource = Me.dt_Rifiuti.DefaultView
                Me.dg_VacInad.DataBind()

        End Select

    End Sub

#End Region

#Region " Datagrid Events "

    Private Sub dg_VacInad_DeleteCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_VacInad.DeleteCommand

        If Me.dg_VacInad.EditItemIndex = -1 Then

            If Not openPopUp Then

                Dim row As DataRow = FindRow(e.Item.ItemIndex)

                'il messaggio di eliminazione programmazione conterrà il codice dell'inadempienza
                If msgElimProg = "" Then
                    msgElimProg &= "Si vuole eliminare il/i rifiuto/ti rilevato/ti in data\r" & row("prf_data_rifiuto")
                Else
                    msgElimProg &= "\r" & row("prf_data_rifiuto")
                End If

                row.Delete()

                dt_Rifiuti.DefaultView.Sort = "prf_data_rifiuto DESC"

                dg_VacInad.DataSource = dt_Rifiuti.DefaultView
                dg_VacInad.DataBind()

                nMod += 1

                Me.OnitLayout31.Busy = True

            Else
                strJS &= "alert('Non è possibile effettuare modifiche alla tabella se la finestra di inserimento è aperta!')" & vbCrLf
            End If

        Else
            strJS &= "alert('Cliccare AGGIORNA O ANNULLA della riga che si sta editando prima di cancellare questa riga!')"
        End If

    End Sub

    Private Sub dg_VacInad_EditCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_VacInad.EditCommand

        If dg_VacInad.EditItemIndex = -1 Then

            Dim row As DataRow = FindRow(e.Item.ItemIndex)

            dg_VacInad.EditItemIndex = e.Item.ItemIndex
            dt_Rifiuti.DefaultView.Sort = "prf_data_rifiuto DESC"

            dg_VacInad.DataSource = dt_Rifiuti.DefaultView
            dg_VacInad.DataBind()

            nMod += 1

            Me.OnitLayout31.Busy = True
            Me.ToolBar.Enabled = False

            'modifica della data ('modifica 27/05/2004)
            DirectCast(dg_VacInad.Items(e.Item.ItemIndex).FindControl("dpkData"), OnitDatePick).Text = row("prf_data_rifiuto")

            If Not row("prf_note_rifiuto") Is DBNull.Value Then

                DirectCast(dg_VacInad.Items(e.Item.ItemIndex).FindControl("tb_noteRif"), TextBox).Text = row("prf_note_rifiuto")

            End If

        Else
            strJS &= "alert('Cliccare AGGIORNA O ANNULLA della riga che si sta editando prima di modificare un\'altra riga!')"
        End If

    End Sub

    Private Sub dg_VacInad_CancelCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_VacInad.CancelCommand

        Me.dg_VacInad.EditItemIndex = -1
        Me.dt_Rifiuti.DefaultView.Sort = "prf_data_rifiuto DESC"

        Me.dg_VacInad.DataSource = dt_Rifiuti.DefaultView
        Me.dg_VacInad.DataBind()

        nMod -= 1
        If nMod = 0 Then Me.OnitLayout31.Busy = False

        Me.ToolBar.Enabled = True

    End Sub

    Private Sub dg_VacInad_UpdateCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_VacInad.UpdateCommand

        Dim row As DataRow = dt_Rifiuti.Rows.Find(rowKey)

        If DirectCast(dg_VacInad.Items(e.Item.ItemIndex).FindControl("dpkData"), OnitDatePick).Text <> row("prf_data_rifiuto").ToString() Or
           DirectCast(dg_VacInad.Items(e.Item.ItemIndex).FindControl("tb_noteRif"), TextBox).Text <> return_only_char("prf_note_rifiuto") Or
           DirectCast(dg_VacInad.Items(e.Item.ItemIndex).FindControl("OnitJsValidator1"), OnitJsValidator).Text <> return_only_char("prf_n_richiamo") Then

            row("prf_ute_id") = OnVacContext.UserId
            row("ute_descrizione") = OnVacContext.UserDescription
            row("prf_genitore") = CType(dg_VacInad.Items.Item(e.Item.ItemIndex).FindControl("tb_genitore"), TextBox).Text
            row("prf_note_rifiuto") = CType(dg_VacInad.Items.Item(e.Item.ItemIndex).FindControl("tb_noteRif"), TextBox).Text
            row("prf_data_rifiuto") = CType(dg_VacInad.Items.Item(e.Item.ItemIndex).FindControl("dpkData"), OnitDatePick).Text

            ' una volta inserito un numero non permette di immettere ""

            Dim onitJsValidator1 As OnitJsValidator = DirectCast(Me.dg_VacInad.Items(e.Item.ItemIndex).FindControl("OnitJsValidator1"), OnitJsValidator)

            If onitJsValidator1.Text <> "" Then

                Dim numRichiamo As Integer = CType(onitJsValidator1.Text, Integer)

                If verifica_vac_eseguite(numRichiamo) Then
                    row("prf_n_richiamo") = numRichiamo
                Else
                    ' inserire un messagio di errore
                    Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Il richiamo indicato è stato già effettuato dal paziente", "Errore", False, False))
                End If
            End If

        End If

        Me.dg_VacInad.EditItemIndex = -1
        Me.dt_Rifiuti.DefaultView.Sort = "prf_data_rifiuto DESC"

        Me.dg_VacInad.DataSource = dt_Rifiuti.DefaultView
        Me.dg_VacInad.DataBind()

        Me.dg_VacInad.Enabled = True

        Me.ToolBar.Enabled = True

    End Sub

#End Region

#Region " Toolbar Events "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btn_Salva"
                Salva()
                msgElimProg = String.Empty

            Case "btn_Annulla"
                Annulla()
                'il messaggio di eliminazione programmazione deve essere vuoto
                msgElimProg = String.Empty

            Case "btn_Inserisci"
                uscInsRifiuto.ModaleName = "modInsRifiuto"
                uscInsRifiuto.LoadModale()
                modInsRifiuto.VisibileMD = True

            Case "btn_Stampa"
                Stampa()

        End Select

    End Sub

#End Region

#Region " User Control Events "

    Private Sub uscInsRifiuto_InsRifiuto() Handles uscInsRifiuto.InsRifiuto

        nMod += 1

        Me.OnitLayout31.Busy = True

        Me.dt_Rifiuti.DefaultView.Sort = "prf_data_rifiuto DESC"
        Me.dg_VacInad.DataSource = Me.dt_Rifiuti.DefaultView
        Me.dg_VacInad.DataBind()

    End Sub

#End Region

#Region " Private Methods "

    Private Function return_only_char(item As String) As String

        Dim row As DataRow = dt_Rifiuti.Rows.Find(rowKey)
        If row(item) Is DBNull.Value Then
            Return String.Empty
        End If

        Return row(item)

    End Function

    Private Function verifica_vac_eseguite(item As Integer) As Boolean

        Dim risultato As Object
        Dim row As DataRow = dt_Rifiuti.Rows.Find(rowKey)

        Using DAM As IDAM = OnVacUtility.OpenDam()

            DAM.QB.NewQuery()
            DAM.QB.AddTables("T_VAC_ESEGUITE")
            DAM.QB.AddSelectFields("MAX(ves_n_richiamo)")
            DAM.QB.AddWhereCondition("ves_paz_codice", Comparatori.Uguale, OnVacUtility.Variabili.PazId, DataTypes.Numero)
            DAM.QB.AddWhereCondition("ves_vac_codice", Comparatori.Uguale, row("prf_vac_codice"), DataTypes.Stringa)

            risultato = DAM.ExecScalar()

        End Using

        If Not risultato Is DBNull.Value Then
            If risultato.ToString < item.ToString() Then
                Return True
            Else
                Return False
            End If
        End If

        ' nel caso non sia presente nessun record nella tabella vuol dire che il paziente
        ' non ha mai eseguito una vaccinazione del tipo richiesto

        Return True

    End Function

    Private Sub Stampa()

        Dim rpt As New ReportParameter()
        Dim dt As New DataTable()
        Dim ds As New DSVaccinazioniRifiutatePaziente()

        Using DAM As IDAM = OnVacUtility.OpenDam()

            With DAM.QB
                .NewQuery()
                .AddSelectFields("PAZ_CODICE", "PAZ_NOME", "PAZ_COGNOME", "PAZ_DATA_NASCITA", "PAZ_SESSO", "PAZ_INDIRIZZO_RESIDENZA",
                                 "prf_genitore", "prf_note_rifiuto", "prf_data_rifiuto", "VAC_DESCRIZIONE", "prf_n_richiamo",
                                 "VAC_OBBLIGATORIA", "CNS_DESCRIZIONE", "COM_DESCRIZIONE", "UTE_DESCRIZIONE")
                .AddTables("T_PAZ_RIFIUTI, T_PAZ_PAZIENTI, T_ANA_VACCINAZIONI, T_ANA_CONSULTORI,T_ANA_COMUNI", "T_ANA_UTENTI")
                .AddWhereCondition("PRF_PAZ_CODICE", Comparatori.Uguale, OnVacUtility.Variabili.PazId, DataTypes.Numero)
                .AddWhereCondition("PRF_PAZ_CODICE", Comparatori.Uguale, "PAZ_CODICE", DataTypes.Join)
                .AddWhereCondition("PRF_VAC_CODICE", Comparatori.Uguale, "VAC_CODICE", DataTypes.Join)
                .AddWhereCondition("PAZ_COM_CODICE_RESIDENZA", Comparatori.Uguale, "COM_CODICE", DataTypes.OutJoinLeft)
                .AddWhereCondition("PAZ_CNS_CODICE", Comparatori.Uguale, "CNS_CODICE", DataTypes.OutJoinLeft)
                .AddWhereCondition("PRF_UTE_ID", Comparatori.Uguale, "UTE_ID", DataTypes.OutJoinLeft)
            End With

            DAM.BuildDataTable(ds.VaccinazioniRifiutatePaziente)
            rpt.set_dataset(ds)

            Using genericProvider As New DAL.DbGenericProvider(DAM)
                Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                    If Not OnVacReport.StampaReport(Page.Request.Path, Constants.ReportName.VaccinazioniRifiutatePaziente, String.Empty, rpt, , , bizReport.GetReportFolder(Constants.ReportName.VaccinazioniRifiutatePaziente)) Then
                        OnVacUtility.StampaNonPresente(Page, Constants.ReportName.VaccinazioniRifiutatePaziente)
                    End If

                End Using
            End Using

        End Using

    End Sub

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()
        listPrintButtons.Add(New PrintButton(Constants.ReportName.VaccinazioniRifiutatePaziente, "btn_Stampa"))

        Me.ShowToolbarPrintButtons(listPrintButtons, Me.ToolBar)

    End Sub

    Private Function FindRow(dgIndex As Integer) As DataRow

        Dim key(1) As Object

        key(0) = DirectCast(dg_VacInad.Items(dgIndex).FindControl("lb_pazcodice"), Label).Text
        key(1) = DirectCast(dg_VacInad.Items(dgIndex).FindControl("lb_codVac"), Label).Text

        rowKey = key

        Return Me.dt_Rifiuti.Rows.Find(key)

    End Function

    Private Sub Salva()

        Dim DAM As IDAM = OnVacUtility.OpenDam()

        ''**************LOG
        'Dim tst As Testata() = LogBox.GetTestataDataTable(dt_Rifiuti, "RIFIUTI", New String() {"prf_data_rifiuto", "prf_vac_codice", "prf_n_richiamo", "prf_note_rifiuto"})
        'LogBox.WriteData(tst)
        ''**************
        Try
            DAM.BeginTrans()

            Dim row As DataRow

            For Each row In dt_Rifiuti.Rows

                Select Case row.RowState

                    Case DataRowState.Deleted

                        DAM.QB.NewQuery()
                        DAM.QB.AddTables("T_PAZ_RIFIUTI")
                        DAM.QB.AddWhereCondition("prf_paz_codice", Comparatori.Uguale, OnVacUtility.Variabili.PazId, DataTypes.Numero)
                        DAM.QB.AddWhereCondition("prf_vac_codice", Comparatori.Uguale, row("prf_vac_codice", DataRowVersion.Original), DataTypes.Stringa)

                        DAM.ExecNonQuery(ExecQueryType.Delete)

                        '*********** LOG
                        Dim t As New Testata(DataLogStructure.TipiArgomento.T_PAZ_RIFIUTI, Operazione.Eliminazione, False)
                        Dim r As New Record()
                        r.Campi.Add(New Campo("PRF_VAC_CODICE", row("PRF_VAC_CODICE", DataRowVersion.Original).ToString))
                        t.Records.Add(r)

                        LogBox.WriteData(t)
                        '**************
                End Select

            Next

            For Each row In dt_Rifiuti.Rows

                Select Case row.RowState

                    Case DataRowState.Added

                        DAM.QB.NewQuery()
                        DAM.QB.AddTables("T_PAZ_RIFIUTI")
                        DAM.QB.AddInsertField("prf_paz_codice", OnVacUtility.Variabili.PazId, DataTypes.Numero)
                        DAM.QB.AddInsertField("prf_vac_codice", row("prf_vac_codice"), DataTypes.Stringa)
                        DAM.QB.AddInsertField("prf_data_rifiuto", row("prf_data_rifiuto"), DataTypes.Data)
                        DAM.QB.AddInsertField("prf_genitore", row("prf_genitore"), DataTypes.Stringa)
                        DAM.QB.AddInsertField("prf_note_rifiuto", row("prf_note_rifiuto"), DataTypes.Stringa)
                        DAM.QB.AddInsertField("prf_ute_id", OnVacContext.UserId, DataTypes.Stringa)

                        DAM.ExecNonQuery(ExecQueryType.Insert)

                        '**********LOG
                        Dim tIns As New Testata(DataLogStructure.TipiArgomento.T_PAZ_RIFIUTI, Operazione.Inserimento, False)
                        Dim RIns As New Record
                        RIns.Campi.Add(New Campo("PRF_DATA_RIFIUTO", "", row("PRF_DATA_RIFIUTO").ToString))
                        RIns.Campi.Add(New Campo("PRF_VAC_CODICE", "", row("PRF_VAC_CODICE").ToString))
                        RIns.Campi.Add(New Campo("PRF_UTE_ID", "", row("PRF_UTE_ID").ToString))
                        RIns.Campi.Add(New Campo("PRF_GENITORE", "", row("PRF_GENITORE").ToString))
                        RIns.Campi.Add(New Campo("PRF_N_RICHIAMO", "", row("PRF_N_RICHIAMO").ToString))
                        tIns.Records.Add(RIns)

                        LogBox.WriteData(tIns)
                        '*************

                    Case DataRowState.Modified

                        DAM.QB.NewQuery()
                        DAM.QB.AddTables("T_PAZ_RIFIUTI")
                        DAM.QB.AddUpdateField("prf_paz_codice", OnVacUtility.Variabili.PazId, DataTypes.Numero)
                        DAM.QB.AddUpdateField("prf_vac_codice", row("prf_vac_codice"), DataTypes.Stringa)
                        DAM.QB.AddUpdateField("prf_data_rifiuto", row("prf_data_rifiuto"), DataTypes.Data)
                        DAM.QB.AddUpdateField("prf_genitore", row("prf_genitore"), DataTypes.Stringa)
                        DAM.QB.AddUpdateField("prf_note_rifiuto", row("prf_note_rifiuto"), DataTypes.Stringa)
                        DAM.QB.AddUpdateField("prf_ute_id", OnVacContext.UserId, DataTypes.Stringa)
                        DAM.QB.AddUpdateField("prf_n_richiamo", row("prf_n_richiamo"), DataTypes.Numero)

                        DAM.QB.AddWhereCondition("prf_paz_codice", Comparatori.Uguale, OnVacUtility.Variabili.PazId, DataTypes.Numero)
                        DAM.QB.AddWhereCondition("prf_vac_codice", Comparatori.Uguale, row("prf_vac_codice", DataRowVersion.Original), DataTypes.Stringa)

                        DAM.ExecNonQuery(ExecQueryType.Update)

                        '********** LOG
                        Dim tMod As New Testata(DataLogStructure.TipiArgomento.T_PAZ_RIFIUTI, Operazione.Modifica, False)
                        Dim RMod As New Record
                        RMod.Campi.Add(New Campo("prf_data_rifiuto", row("prf_data_rifiuto", DataRowVersion.Original).ToString, row("prf_data_rifiuto").ToString))
                        RMod.Campi.Add(New Campo("prf_vac_codice", row("prf_vac_codice", DataRowVersion.Original).ToString, row("prf_vac_codice").ToString))
                        RMod.Campi.Add(New Campo("prf_ute_id", row("prf_ute_id", DataRowVersion.Original).ToString, row("prf_ute_id").ToString))
                        RMod.Campi.Add(New Campo("prf_genitore", row("prf_genitore", DataRowVersion.Original).ToString, row("prf_genitore").ToString))
                        RMod.Campi.Add(New Campo("prf_n_richiamo", row("prf_n_richiamo", DataRowVersion.Original).ToString, row("prf_n_richiamo").ToString))
                        tMod.Records.Add(RMod)

                        If tMod.ChangedValues Then LogBox.WriteData(tMod)
                        '*************

                End Select

            Next

            dt_Rifiuti.AcceptChanges()

            nMod = 0

            Me.OnitLayout31.Busy = False

            DAM.Commit()

        Catch exc As Exception

            DAM.Rollback()

            exc.InternalPreserveStackTrace()
            Throw

        Finally

            OnVacUtility.CloseDam(DAM)

        End Try

    End Sub

    Private Sub Annulla()

        Me.dt_Rifiuti.RejectChanges()

        nMod = 0

        Me.OnitLayout31.Busy = False

        Me.dt_Rifiuti.DefaultView.Sort = "prf_data_rifiuto DESC"

        Me.dg_VacInad.DataSource = Me.dt_Rifiuti.DefaultView
        Me.dg_VacInad.DataBind()

    End Sub

#End Region

#Region " Public Methods "

    'ritorna il valore dello stato dell'inadempienza associato ad un codice specificato [modifica 28/02/2005]
    Public Function RecuperaStato(codStato As String) As String

        Dim stato As Object

        Using dam As IDAM = OnVacUtility.OpenDam()

            With dam.QB
                .NewQuery()
                .AddSelectFields("COD_DESCRIZIONE")
                .AddTables("T_ANA_CODIFICHE")
                .AddWhereCondition("COD_CAMPO", Comparatori.Uguale, "PIN_STATO", DataTypes.Stringa)
                .AddWhereCondition("COD_CODICE", Comparatori.Uguale, codStato, DataTypes.Stringa)
            End With

            stato = dam.ExecScalar()

        End Using

        If stato Is Nothing Then Return String.Empty

        Return stato.ToString()

    End Function

#End Region

End Class
