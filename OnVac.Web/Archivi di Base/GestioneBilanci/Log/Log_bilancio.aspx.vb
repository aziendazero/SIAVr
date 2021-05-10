Imports Onit.Database.DataAccessManager


Partial Class Log_bilancio
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

#Region " Page "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles MyBase.PreRender

        'abilitazione/disabilitazione del pulsante per la compilazione del bilancio
        Me.ToolBar.Items.FromKeyButton("btnBilancio").Enabled = True

    End Sub

#End Region

#Region " Controls "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btnBilancio"

                Me.uscRicBil.ModaleName = "modRicBil"
                Me.uscRicBil.LoadModale()
                Me.modRicBil.VisibileMD = True
                Me.ToolBar.Items.FromKeyButton("btnBilancio").Enabled = False

            Case "Pulisci"

                Me.txtDaData.Text = String.Empty
                Me.txtAdata.Text = String.Empty
                Me.txtNumero.Text = String.Empty
                Me.txtMalattia.Text = String.Empty

                Me.dgr_log_bilancio_oss.DataBind()
                Me.dgr_log_oss_risp.DataBind()

        End Select

    End Sub

    Private Sub uscRicBil_ReturnBilancio(bilancioSelezionato As Entities.BilancioAnagrafica) Handles uscRicBil.ReturnBilancio

        'imposto i campi della finestra relativi al bilancio selezionato
        Me.txtMalattia.Text = bilancioSelezionato.DescrizioneMalattia
        Me.txtNumero.Text = bilancioSelezionato.NumeroBilancio

        Dim dt_log_bil_oss As New DataTable()

        ' TODO: spostare query log bilanci in dal (+ usare biz)

        Dim DAM As IDAM = OnVacUtility.OpenDam()

        Try
            DAM.QB.NewQuery()

            DAM.QB.AddSelectFields("TLM_OPERAZIONE,TLM_DATA,UTE_DESCRIZIONE,TLM_OSS_CODICE,OSS_DESCRIZIONE")
            DAM.QB.AddTables("T_LOG_MOD_BILANCI")
            DAM.QB.AddTables("T_ANA_UTENTI")
            DAM.QB.AddTables("T_ANA_OSSERVAZIONI")
            DAM.QB.AddTables("T_LOG_LINK_MOD_BILANCI")

            DAM.QB.AddWhereCondition("tlb_tlm_id", Comparatori.Uguale, "tlm_id", DataTypes.Join)

            DAM.QB.AddWhereCondition("tlb_bil_numero", Comparatori.Uguale, bilancioSelezionato.NumeroBilancio, DataTypes.Numero)
            DAM.QB.AddWhereCondition("tlb_mal_codice", Comparatori.Uguale, bilancioSelezionato.CodiceMalattia, DataTypes.Stringa)

            DAM.QB.AddWhereCondition("tlm_ute_id", Comparatori.Uguale, "ute_id", DataTypes.OutJoinLeft)
            DAM.QB.AddWhereCondition("oss_codice", Comparatori.Uguale, "tlm_oss_codice", DataTypes.Join)

            DAM.QB.AddWhereCondition("tlm_ris_codice", Comparatori.Is, "NULL", DataTypes.Join)

            If Me.txtDaData.Data > DateTime.MinValue AndAlso Me.txtAdata.Data > DateTime.MinValue AndAlso Me.txtAdata.Data >= Me.txtDaData.Data Then

                DAM.QB.AddWhereCondition("tlm_data", Comparatori.MaggioreUguale, Me.txtDaData.Data, DataTypes.Data)
                DAM.QB.AddWhereCondition("tlm_data", Comparatori.Minore, Me.txtAdata.Data.AddDays(1), DataTypes.Data)


            End If

            DAM.QB.AddOrderByFields("tlm_data DESC")

            DAM.BuildDataTable(dt_log_bil_oss)

            dgr_log_bilancio_oss.DataSource = dt_log_bil_oss
            dgr_log_bilancio_oss.DataBind()

        Finally
            OnVacUtility.CloseDam(DAM)
            dt_log_bil_oss.Dispose()
            Me.ToolBar.Items.FromKeyButton("btnBilancio").Enabled = True
        End Try

        Dim dt_log_bil_ris As New DataTable()

        DAM = OnVacUtility.OpenDam()

        Try
            DAM.QB.NewQuery()

            DAM.QB.AddSelectFields("TLM_OPERAZIONE,TLM_DATA,UTE_DESCRIZIONE,OSS_DESCRIZIONE,RIS_DESCRIZIONE")
            DAM.QB.AddTables("T_LOG_MOD_BILANCI")
            DAM.QB.AddTables("T_LOG_LINK_MOD_BILANCI")
            DAM.QB.AddTables("T_ANA_OSSERVAZIONI")
            DAM.QB.AddTables("T_ANA_RISPOSTE")
            DAM.QB.AddTables("T_ANA_UTENTI")

            DAM.QB.AddWhereCondition("tlm_id", Comparatori.Uguale, "tlb_tlm_id", DataTypes.Join)
            DAM.QB.AddWhereCondition("tlm_oss_codice", Comparatori.Uguale, "oss_codice", DataTypes.Join)
            DAM.QB.AddWhereCondition("tlm_ris_codice", Comparatori.Uguale, "ris_codice", DataTypes.Join)
            DAM.QB.AddWhereCondition("tlm_ute_id", Comparatori.Uguale, "ute_id", DataTypes.OutJoinLeft)
            DAM.QB.AddWhereCondition("tlb_bil_numero", Comparatori.Uguale, bilancioSelezionato.NumeroBilancio, DataTypes.Numero)
            DAM.QB.AddWhereCondition("tlb_mal_codice", Comparatori.Uguale, bilancioSelezionato.CodiceMalattia, DataTypes.Stringa)
            DAM.QB.AddWhereCondition("tlm_ris_codice", Comparatori.[IsNot], "Null", DataTypes.Stringa)
            DAM.QB.AddOrderByFields("tlm_data DESC")

            If ((txtDaData.Text <> "" And txtAdata.Text <> "") And (txtAdata.Text >= txtDaData.Text)) Then

                DAM.QB.AddWhereCondition("tlm_data", Comparatori.MinoreUguale, "to_date('" & txtAdata.Text & "','DD/MM/YYYY')", DataTypes.Replace)
                DAM.QB.AddWhereCondition("tlm_data", Comparatori.MaggioreUguale, "to_date('" & txtDaData.Text & "','DD/MM/YYYY')", DataTypes.Replace)

            End If

            DAM.BuildDataTable(dt_log_bil_ris)

            dgr_log_oss_risp.DataSource = dt_log_bil_ris
            dgr_log_oss_risp.DataBind()

        Finally
            OnVacUtility.CloseDam(DAM)
            dt_log_bil_ris.Dispose()
            Me.ToolBar.Items.FromKeyButton("btnBilancio").Enabled = True
        End Try

    End Sub

    Private Sub uscRicBil_AnnullaBilancio() Handles uscRicBil.AnnullaBilancio

        'abilita nuovamente il pulsante di scelta bilancio
        Me.ToolBar.Items.FromKeyButton("btnBilancio").Enabled = True

    End Sub

#End Region

End Class