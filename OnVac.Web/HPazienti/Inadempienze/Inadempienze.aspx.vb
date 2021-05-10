Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.Web.UI.WebControls.Validators


Partial Class OnVac_Inadempienze
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

#Region " Public "

    Public strJS As String

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
            Return Session("OnVac_nMod")
        End Get
        Set(ByVal Value As Int16)
            Session("OnVac_nMod") = Value
        End Set
    End Property

    Property openPopUp() As Boolean
        Get
            Return Session("OnVac_openPopUp")
        End Get
        Set(ByVal Value As Boolean)
            Session("OnVac_openPopUp") = Value
        End Set
    End Property

    Property dt_Inadempienze() As DataTable
        Get
            Return Session("OnVac_dt_Inadempienze")
        End Get
        Set(ByVal Value As DataTable)
            Session("OnVac_dt_Inadempienze") = Value
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

    ' DMI 08/08/2007
    Public Property PazienteScelto() As String
        Get
            Return Session("PAZIENTE_SCELTO")
        End Get
        Set(ByVal Value As String)
            Session("PAZIENTE_SCELTO") = Value
        End Set
    End Property

    Public Property CollCodifiche As Collection.CodificheCollection
        Get
            Return Session("CollCodifiche")
        End Get
        Set(value As Collection.CodificheCollection)
            Session("CollCodifiche") = value
        End Set
    End Property

#End Region

#Region " Page Events "

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack() Then

            ShowPrintButtons()

            openPopUp = False

            dt_Inadempienze = New DataTable()

            Using DAM As IDAM = OnVacUtility.OpenDam()

                With DAM.QB
                    .NewQuery()
                    .AddTables("t_paz_inadempienze, t_ana_vaccinazioni, t_ana_utenti, t_ana_codifiche")
                    .AddSelectFields("pin_paz_codice, pin_vac_codice, vac_descrizione, pin_stato, pin_stampato, pin_data, pin_ute_id, ute_descrizione, cod_descrizione")
                    .AddWhereCondition("pin_paz_codice", Comparatori.Uguale, OnVacUtility.Variabili.PazId, DataTypes.Numero)
                    .AddWhereCondition("pin_vac_codice", Comparatori.Uguale, "vac_codice", DataTypes.Join)
                    .AddWhereCondition("pin_ute_id", Comparatori.Uguale, "ute_id", DataTypes.OutJoinLeft)
                    .AddWhereCondition("cod_campo", Comparatori.Uguale, "PIN_STATO", DataTypes.Stringa)
                    .AddWhereCondition("pin_stato", Comparatori.Uguale, "cod_codice", DataTypes.OutJoinLeft)
                    .AddOrderByFields("vac_ordine")
                End With

                DAM.BuildDataTable(dt_Inadempienze)

                dg_VacInad.DataSource = dt_Inadempienze.DefaultView
                dg_VacInad.DataBind()

                OnVacUtility.addKey(dt_Inadempienze, "pin_vac_codice")

                Using genericProvider As New DAL.DbGenericProvider(DAM)

                    OnVacUtility.ImpostaIntestazioniPagina(Me.OnitLayout31, Me.LayoutTitolo, genericProvider, Me.Settings, Me.IsGestioneCentrale)

                    Using bizCodifiche As New Biz.BizCodifiche(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                        Me.CollCodifiche = bizCodifiche.GetCodifiche("PIN_STATO")

                    End Using

                End Using

                'il messaggio di eliminazione programmazione deve essere vuoto
                msgElimProg = String.Empty

            End Using

        End If

        Select Case Request.Form("__EVENTTARGET")

            Case "ConfermaInsVac"

                nMod += 1

                Me.OnitLayout31.Busy = True

                dg_VacInad.DataSource = dt_Inadempienze.DefaultView
                dg_VacInad.DataBind()

            Case "Stampa"

                If Me.ToolBar.Items.FromKeyButton("btn_StampaComunicazioneAlSindaco").Visible Then

                    Stampa(Constants.ReportName.ElencoNotifiche)

                    If Request.Form("__EVENTARGUMENT") = "ImpostaStatoS" Then
                        AggiornaInadempienzeStatoConcluso()
                    End If

                    AggiornaStatoStampa()

                End If

        End Select

    End Sub

#End Region

#Region " Datagrid Events "

    Private Sub dg_VacInad_DeleteCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_VacInad.DeleteCommand

        If (dg_VacInad.EditItemIndex = -1) Then

            If Not openPopUp Then

                Dim row As DataRow = FindRow(e.Item.ItemIndex)

                'il messaggio di eliminazione programmazione conterrà il codice dell'inadempienza
                If msgElimProg = "" Then
                    msgElimProg &= "Si vuole eliminare l\'inadempenza per le seguenti vaccinazioni?\r\n" & row("vac_descrizione")
                Else
                    msgElimProg &= "\r\n" & row("vac_descrizione")
                End If

                row.Delete()
                dt_Inadempienze.DefaultView.Sort = "pin_vac_codice ASC"
                dt_Inadempienze.DefaultView.Sort = ""
                dg_VacInad.DataSource = dt_Inadempienze.DefaultView
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

    Private Sub dg_VacInad_EditCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_VacInad.EditCommand

        If (dg_VacInad.EditItemIndex = -1) Then

            Dim ddl As DropDownList
            Dim i As Int16

            Dim row As DataRow = FindRow(e.Item.ItemIndex)

            dg_VacInad.EditItemIndex = e.Item.ItemIndex
            dt_Inadempienze.DefaultView.Sort = "pin_vac_codice ASC"
            dt_Inadempienze.DefaultView.Sort = ""
            dg_VacInad.DataSource = dt_Inadempienze.DefaultView
            dg_VacInad.DataBind()

            nMod += 1

            Me.OnitLayout31.Busy = True

            Me.ToolBar.Enabled = False

            ddl = DirectCast(dg_VacInad.Items.Item(e.Item.ItemIndex).FindControl("ddlStato"), DropDownList)
            For i = 0 To ddl.Items.Count - 1
                If ddl.Items(i).Value = row("pin_stato") Then
                    ddl.SelectedIndex = i
                End If
            Next

            DirectCast(dg_VacInad.Items.Item(e.Item.ItemIndex).FindControl("dpkData"), OnitDatePick).Text = row("pin_data")
        Else
            strJS &= "alert('Cliccare AGGIORNA O ANNULLA della riga che si sta editando prima di modificare un\'altra riga!')"
        End If

    End Sub

    Private Sub dg_VacInad_CancelCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_VacInad.CancelCommand

        dg_VacInad.EditItemIndex = -1
        dt_Inadempienze.DefaultView.Sort = "pin_vac_codice ASC"
        dt_Inadempienze.DefaultView.Sort = ""
        dg_VacInad.DataSource = dt_Inadempienze.DefaultView
        dg_VacInad.DataBind()

        nMod -= 1

        If (nMod = 0) Then Me.OnitLayout31.Busy = False

        Me.ToolBar.Enabled = True

    End Sub

    Private Sub dg_VacInad_UpdateCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_VacInad.UpdateCommand

        Dim _ddlStato As DropDownList = DirectCast(dg_VacInad.Items.Item(e.Item.ItemIndex).FindControl("ddlStato"), DropDownList)
        Dim _dpkData As OnitDatePick = DirectCast(dg_VacInad.Items.Item(e.Item.ItemIndex).FindControl("dpkData"), OnitDatePick)

        Dim row As DataRow = dt_Inadempienze.Rows.Find(rowKey)

        'modifica della data ('modifica 27/05/2004)
        If ((row("pin_stato") <> _ddlStato.SelectedItem.Value)) Or (_dpkData.Text <> row("pin_data")) Then
            row("pin_stato") = _ddlStato.SelectedItem.Value
            row("cod_descrizione") = _ddlStato.SelectedItem.Text
            row("pin_ute_id") = OnVacContext.UserId
            row("ute_descrizione") = OnVacContext.UserDescription
            row("pin_data") = _dpkData.Text
        End If
        dg_VacInad.EditItemIndex = -1
        dt_Inadempienze.DefaultView.Sort = "pin_vac_codice ASC"
        dt_Inadempienze.DefaultView.Sort = ""
        dg_VacInad.DataSource = dt_Inadempienze.DefaultView
        dg_VacInad.DataBind()
        dg_VacInad.Enabled = True

        Me.ToolBar.Enabled = True

    End Sub

#End Region

#Region " Toolbar Events "

    Private Sub ToolBar_ButtonClicked(ByVal sender As Object, ByVal e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case (e.Button.Key)

            Case "btn_Salva"
                Salva()
                msgElimProg = ""

            Case "btn_Annulla"
                Annulla()
                'il messaggio di eliminazione programmazione deve essere vuoto
                msgElimProg = ""

            Case "btn_Inserisci"
                uscInsInadempienze.ModaleName = "modInsInadempienze"
                uscInsInadempienze.LoadModale()
                modInsInadempienze.VisibileMD = True

            Case "btn_StampaTerminePerentorio"
                Stampa(Constants.ReportName.RistampaTerminePerentorio)

        End Select

    End Sub

    Private Sub toolbarMod_ButtonClicked(ByVal sender As Object, ByVal be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles toolbarMod.ButtonClicked

        Select Case be.Button.Key

            Case "btn_Chiudi"
                modDettaglio.VisibileMD = False

        End Select

    End Sub

#End Region

#Region " User Control Events "

    Private Sub uscInsInadempienze_InsInadempienza() Handles uscInsInadempienze.InsInadempienza

        nMod += 1

        Me.OnitLayout31.Busy = True

        dt_Inadempienze.DefaultView.Sort = "pin_vac_codice ASC"
        dt_Inadempienze.DefaultView.Sort = ""

        dg_VacInad.DataSource = dt_Inadempienze.DefaultView
        dg_VacInad.DataBind()

    End Sub

#End Region

#Region " Public Methods "

    'ritorna il valore dello stato dell'inadempienza associato ad un codice specificato [modifica 28/02/2005]
    Public Function RecuperaStato(ByVal codStato As Enumerators.StatoInadempienza) As String

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizCodifiche As New Biz.BizCodifiche(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Return bizCodifiche.GetDescrizioneCodifica("PIN_STATO", codStato)

            End Using

        End Using

    End Function

    Public Sub apriDettaglio(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)

        Dim vacCodice As String = DirectCast(sender, System.Web.UI.WebControls.ImageButton).CommandArgument

        Using dam As IDAM = OnVacUtility.OpenDam()

            With dam.QB
                .NewQuery()
                .AddTables("T_PAZ_INADEMPIENZE")
                .AddSelectFields("*")
                .AddWhereCondition("PIN_PAZ_CODICE", Comparatori.Uguale, OnVacUtility.Variabili.PazId, DataTypes.Stringa)
                .AddWhereCondition("PIN_VAC_CODICE", Comparatori.Uguale, vacCodice, DataTypes.Stringa)
            End With

            Using dr As IDataReader = dam.BuildDataReader()

                If Not dr Is Nothing Then

                    Dim pos_data_app1 As Int16 = dr.GetOrdinal("PIN_PRI_DATA_APPUNTAMENTO1")
                    Dim pos_data_app2 As Int16 = dr.GetOrdinal("PIN_PRI_DATA_APPUNTAMENTO2")
                    Dim pos_data_app3 As Int16 = dr.GetOrdinal("PIN_PRI_DATA_APPUNTAMENTO3")
                    Dim pos_data_app4 As Int16 = dr.GetOrdinal("PIN_PRI_DATA_APPUNTAMENTO4")
                    Dim pos_data_stampa As Int16 = dr.GetOrdinal("PIN_PRI_DATA_STAMPA_TP")

                    While dr.Read()
                        lblData1.Text = (dr(pos_data_app1).ToString().Split(" ")(0)).ToString()
                        lblData2.Text = (dr(pos_data_app2).ToString().Split(" ")(0)).ToString()
                        lblData3.Text = (dr(pos_data_app3).ToString().Split(" ")(0)).ToString()
                        lblData4.Text = (dr(pos_data_app4).ToString().Split(" ")(0)).ToString()
                        lblData5.Text = (dr(pos_data_stampa).ToString().Split(" ")(0)).ToString()
                    End While

                End If

            End Using

        End Using

        modDettaglio.VisibileMD = True

    End Sub

#End Region

#Region " Private Methods "

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()

        listPrintButtons.Add(New PrintButton(Constants.ReportName.RistampaTerminePerentorio, "btn_StampaTerminePerentorio"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.ElencoNotifiche, "btn_StampaComunicazioneAlSindaco"))

        Me.ShowToolbarPrintButtons(listPrintButtons, ToolBar)

    End Sub

    Private Function FindRow(dgIndex As Integer) As DataRow

        Dim key(0) As Object
        key(0) = DirectCast(dg_VacInad.Items.Item(dgIndex).FindControl("lb_codVac"), Label).Text
        rowKey = key

        Return dt_Inadempienze.Rows.Find(key)

    End Function

    Private Sub Salva()

        Using transactionScope As New System.Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, OnVacUtility.GetReadCommittedTransactionOptions())

            LogBox.WriteData(LogBox.GetTestataDataTable(dt_Inadempienze, TipiArgomento.INADEMPIENZE, New String() {"vac_descrizione", "pin_data", "cod_descrizione"}))

            Using DAM As IDAM = OnVacUtility.OpenDam()

                Dim row As DataRow
                Dim idUtente As Integer = OnVacContext.UserId

                For Each row In dt_Inadempienze.Rows

                    Select Case row.RowState

                        Case DataRowState.Deleted

                            With DAM.QB
                                .NewQuery()
                                .AddTables("t_paz_inadempienze")
                                .AddWhereCondition("pin_paz_codice", Comparatori.Uguale, OnVacUtility.Variabili.PazId, DataTypes.Numero)
                                .AddWhereCondition("pin_vac_codice", Comparatori.Uguale, row("pin_vac_codice", DataRowVersion.Original), DataTypes.Stringa)
                            End With

                            DAM.ExecNonQuery(ExecQueryType.Delete)

                            EliminaEsclusioneLegataAdInadempienza(row("pin_vac_codice", DataRowVersion.Original), OnVacUtility.Variabili.PazId, DAM)

                        Case DataRowState.Added

                            With DAM.QB
                                .NewQuery()
                                .AddTables("t_paz_inadempienze")
                                .AddInsertField("pin_paz_codice", OnVacUtility.Variabili.PazId, DataTypes.Numero)
                                .AddInsertField("pin_vac_codice", row("pin_vac_codice"), DataTypes.Stringa)
                                .AddInsertField("pin_data", row("pin_data"), DataTypes.Data)
                                .AddInsertField("pin_ute_id", idUtente, DataTypes.Numero)
                                .AddInsertField("pin_stato", row("pin_stato"), DataTypes.Stringa)
                            End With

                            DAM.ExecNonQuery(ExecQueryType.Insert)

                            Me.CreaEsclusioneLegataAdInadempienza(OnVacUtility.Variabili.PazId, idUtente, row("pin_vac_codice"), row("pin_data"), DAM)

                            ''elimina la programmazione della vaccinazione
                            'EliminaProgrammazione(row("pin_vac_codice"), DAM)

                            ''deve controllare se esiste una vaccinazione sostituta
                            ''e, in caso positivo, eliminare la programmazione anche per questa
                            'Dim vacSostituta As String = row("pin_vac_codice")
                            'OnVacUtility.ControllaVaccinazioneSostituta(vacSostituta, , , Enumerators.DipendenzaSostituta.Sinistra)
                            'If vacSostituta <> row("pin_vac_codice") Then
                            '    EliminaProgrammazione(vacSostituta, DAM)
                            'End If

                        Case DataRowState.Modified

                            With DAM.QB
                                .NewQuery()
                                .AddTables("t_paz_inadempienze")
                                .AddUpdateField("pin_data", row("pin_data"), DataTypes.Data)
                                .AddUpdateField("pin_ute_id", idUtente, DataTypes.Numero)
                                .AddUpdateField("pin_stato", row("pin_stato"), DataTypes.Stringa)
                                .AddWhereCondition("pin_paz_codice", Comparatori.Uguale, OnVacUtility.Variabili.PazId, DataTypes.Numero)
                                .AddWhereCondition("pin_vac_codice", Comparatori.Uguale, row("pin_vac_codice"), DataTypes.Stringa)
                            End With

                            DAM.ExecNonQuery(ExecQueryType.Update)

                            CreaEsclusioneLegataAdInadempienza(OnVacUtility.Variabili.PazId, idUtente, row("pin_vac_codice"), row("pin_data"), DAM)

                    End Select

                Next

                OnVacUtility.ControllaSeTotalmenteInadempiente(OnVacUtility.Variabili.PazId, DAM, Settings)

            End Using

            transactionScope.Complete()

        End Using

        dt_Inadempienze.AcceptChanges()

        nMod = 0

        Me.OnitLayout31.Busy = False

    End Sub

    Private Sub Annulla()

        dt_Inadempienze.RejectChanges()

        nMod = 0

        Me.OnitLayout31.Busy = False

        dt_Inadempienze.DefaultView.Sort = "pin_vac_codice ASC"
        dt_Inadempienze.DefaultView.Sort = ""

        dg_VacInad.DataSource = dt_Inadempienze.DefaultView
        dg_VacInad.DataBind()

    End Sub

    'esegue la stampa [modifica 25/02/2005]
    Private Sub Stampa(nomeReport As String)

        Dim rpt As New ReportParameter()

        Dim strFiltro As String = String.Empty

        Dim terminePerentorio As Int16 = Enumerators.StatoInadempienza.TerminePerentorio
        Dim comunicazioneSindaco As Int16 = Enumerators.StatoInadempienza.ComunicazioneAlSindaco
        Dim casoConcluso As Int16 = Enumerators.StatoInadempienza.CasoConcluso

        Select Case nomeReport

            Case Constants.ReportName.RistampaTerminePerentorio
                'stampa il termine perentorio per le vaccinazioni obbligatorie presenti nella v_avvisi e per le inadempienze con stato 1 o 2
                If Me.Settings.TIPOFILTROSTAMPATP = terminePerentorio.ToString() Then
                    strFiltro = "{T_PAZ_PAZIENTI.PAZ_CODICE}= " + OnVacUtility.Variabili.PazId.ToString()
                ElseIf Me.Settings.TIPOFILTROSTAMPATP = comunicazioneSindaco.ToString() Then
                    strFiltro = "{V_AVVISI.PAZ_CODICE}= " + OnVacUtility.Variabili.PazId.ToString()
                End If

            Case Constants.ReportName.ElencoNotifiche
                'stampa la comunicazione al sindaco per le inadempienze con stato 2 o 3
                strFiltro = "{T_PAZ_PAZIENTI.PAZ_CODICE} = " + OnVacUtility.Variabili.PazId.ToString()
                strFiltro += String.Format(" AND ({{T_PAZ_INADEMPIENZE.PIN_STATO}} = '{0}' OR {{T_PAZ_INADEMPIENZE.PIN_STATO}} = '{1}')", comunicazioneSindaco.ToString, casoConcluso.ToString)

        End Select

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                If Not OnVacReport.StampaReport(Page.Request.Path, nomeReport, strFiltro, , , , bizReport.GetReportFolder(nomeReport)) Then
                    OnVacUtility.StampaNonPresente(Page, nomeReport)
                End If

            End Using
        End Using

    End Sub

    'imposta lo stato delle inadempienze a CASO CONCLUSO [modifica 28/02/2005]
    Private Sub AggiornaInadempienzeStatoConcluso()

        Using dam As IDAM = OnVacUtility.OpenDam()

            Try
                dam.BeginTrans()

                With dam.QB
                    'impostazione del nuovo stato
                    .NewQuery(False, False)
                    .AddUpdateField("PIN_STATO", Enumerators.StatoInadempienza.CasoConcluso, DataTypes.Stringa)
                    .AddTables("T_PAZ_INADEMPIENZE")
                    .AddWhereCondition("PIN_PAZ_CODICE", Comparatori.Uguale, OnVacUtility.Variabili.PazId, DataTypes.Numero)
                    .AddWhereCondition("PIN_STATO", Comparatori.Uguale, Enumerators.StatoInadempienza.ComunicazioneAlSindaco, DataTypes.Stringa)
                End With

                dam.ExecNonQuery(ExecQueryType.Update)

                dam.Commit()

            Catch ex As Exception

                dam.Rollback()

                ex.InternalPreserveStackTrace()
                Throw

            End Try

        End Using

    End Sub

    'imposta la data di stampa della CS e il campo pin_stampato='S'
    Private Sub AggiornaStatoStampa()

        Using dam As IDAM = OnVacUtility.OpenDam()

            Try
                dam.BeginTrans()

                With dam.QB
                    'impostazione del nuovo stato
                    .NewQuery(False, False)
                    .AddUpdateField("PIN_STAMPATO", "S", DataTypes.Stringa)
                    .AddUpdateField("PIN_PRI_DATA_STAMPA_CS", Date.Now, DataTypes.DataOra)
                    .AddUpdateField("PIN_UTE_ID_STAMPA_CS", OnVacContext.UserId, DataTypes.Numero)
                    .AddTables("T_PAZ_INADEMPIENZE")
                    .AddWhereCondition("PIN_PAZ_CODICE", Comparatori.Uguale, OnVacUtility.Variabili.PazId, DataTypes.Numero)
                End With

                dam.ExecNonQuery(ExecQueryType.Update)

                dam.Commit()

            Catch ex As Exception

                dam.Rollback()

                ex.InternalPreserveStackTrace()
                Throw

            End Try

        End Using

    End Sub

    'creo un'esclusione relativa l'indempienza
    Private Sub CreaEsclusioneLegataAdInadempienza(pazCodice As String, opeCodice As String, vacCodice As String, data As Date, dam As IDAM)
        '---
        With dam.QB
            '---
            .NewQuery()
            .AddTables("T_ANA_MOTIVI_ESCLUSIONE")
            .AddSelectFields("MOE_CODICE")
            .AddWhereCondition("MOE_DEFAULT_INAD", Comparatori.Uguale, "S", DataTypes.Stringa)
            .AddWhereCondition("MOE_OBSOLETO", Comparatori.Uguale, "N", DataTypes.Stringa)
            '---
            Dim codiceMotivoEsclusione As String = dam.ExecScalar()
            '---
            If String.IsNullOrWhiteSpace(codiceMotivoEsclusione) Then
                Exit Sub
            End If
            '---
            Dim recordLog As New Record()
            Dim operazioneLog As Operazione
            '---
            Using genericProvider As New DbGenericProvider(dam)

                Using bizVaccinazioniEscluse As New Biz.BizVaccinazioniEscluse(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.INADEMPIENZE))

                    Dim vaccinazioneEsclusa As New Entities.VaccinazioneEsclusa()
                    vaccinazioneEsclusa.CodicePaziente = pazCodice
                    vaccinazioneEsclusa.CodiceVaccinazione = vacCodice
                    vaccinazioneEsclusa.DataVisita = data
                    vaccinazioneEsclusa.CodiceMotivoEsclusione = codiceMotivoEsclusione

                    Dim vaccinazioneEsclusaEliminata As Entities.VaccinazioneEsclusa =
                        bizVaccinazioniEscluse.AggiornaVaccinazioneEsclusa(vaccinazioneEsclusa, False, "Maschera Inadempienze")

                    If vaccinazioneEsclusaEliminata Is Nothing Then

                        recordLog.Campi.Add(New Campo("vex_vac_codice", "", vacCodice))
                        recordLog.Campi.Add(New Campo("vex_data_visita", "", data.ToString))
                        recordLog.Campi.Add(New Campo("vex_data_scadenza", "", ""))
                        recordLog.Campi.Add(New Campo("vex_moe_codice", "", codiceMotivoEsclusione))
                        recordLog.Campi.Add(New Campo("vex_ute_id_registrazione", "", opeCodice))
                        operazioneLog = Operazione.Inserimento

                    Else

                        'todo: CreaEsclusioneLegataAdInadempienza => LOG modifica esclusione
                        operazioneLog = Operazione.Modifica

                    End If

                End Using

            End Using

            Dim testataLog As New Testata(DataLogStructure.TipiArgomento.VAC_ESCLUSE, operazioneLog)
            testataLog.Records.Add(recordLog)

            LogBox.WriteData(testataLog)

        End With

    End Sub

    'elimino l'esclusione
    Private Sub EliminaEsclusioneLegataAdInadempienza(vacCodice As String, pazCodice As String, dam As IDAM)

        Using dbGenericProvider As New DbGenericProvider(dam)

            Using bizVaccinazioniEscluse As New Biz.BizVaccinazioniEscluse(dbGenericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.INADEMPIENZE))

                Dim vaccinazioneEsclusa As Entities.VaccinazioneEsclusa = bizVaccinazioniEscluse.LoadVaccinazioneEsclusa(pazCodice, vacCodice)

                If Not vaccinazioneEsclusa Is Nothing Then

                    Dim salvaVaccinazioneEsclusaCommand As New Biz.BizVaccinazioniEscluse.SalvaVaccinazioneEsclusaCommand
                    salvaVaccinazioneEsclusaCommand.VaccinazioneEsclusa = vaccinazioneEsclusa
                    salvaVaccinazioneEsclusaCommand.Operation = OnVac.Biz.BizClass.SalvaCommandOperation.Delete

                    bizVaccinazioniEscluse.SalvaVaccinazioneEsclusa(salvaVaccinazioneEsclusaCommand)

                    '*************LOG
                    Dim t As New Testata(DataLogStructure.TipiArgomento.VAC_ESCLUSE, Operazione.Eliminazione, True)
                    Dim r As New Record()
                    r.Campi.Add(New Campo("VEX_VAC_CODICE", vacCodice, vacCodice))

                    t.Records.Add(r)

                    LogBox.WriteData(t)
                    '**************

                End If

            End Using

        End Using

    End Sub

#End Region

End Class
