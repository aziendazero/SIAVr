Imports System.Collections.Generic
Imports Onit.Controls
Imports Onit.Web.UI.WebControls.Validators

Partial Class OnVac_VacEscluse
    Inherits OnVac.Common.PageBase

#Region " Types "

    Private Enum DgVacExColumnIndex
        CheckSelect = 0
        DeleteButton = 1
        UpdateButton = 2
        RinnovoDetailButton = 3
        Vaccinazione = 4
        Dose = 5
        DataVisita = 6
        MotivoEsclusione = 7
        Medico = 8
        DataScadenza = 9
        Note = 10
        DescrizioneUslInserimento = 11
        FlagVisibilita = 12
        FlagScaduta = 13
        Rinnovo = 14
        CodiceUslInserimento = 15
    End Enum

    Private Enum tipoEliminazione
        CancellazioneUtente
        Rinnovo
    End Enum

#End Region

#Region " Public "

    Public strJS As String

#End Region

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

    ReadOnly Property UltimaConvocazioneVisitata() As Date
        Get
            Return Session("UltimaConvocazioneVisitata")
        End Get
    End Property

    Property arVacEx() As ArrayList
        Get
            Return Session("OnVac_arVacEx")
        End Get
        Set(Value As ArrayList)
            Session("OnVac_arVacEx") = Value
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

    Public Property dt_vacEx() As DataTable
        Get
            Return Session("OnVac_dt_vacEx")
        End Get
        Set(Value As DataTable)
            Session("OnVac_dt_vacEx") = Value
        End Set
    End Property

    Private Property rowKey() As Object()
        Get
            Return Session("OnVac_rowKey")
        End Get
        Set(Value As Object())
            Session("OnVac_rowKey") = Value
        End Set
    End Property

    Private ReadOnly Property ValoreVisibilitaDefaultPaziente() As String
        Get
            If ViewState("ValoreVisibilitaDefaultPaziente") Is Nothing Then
                ViewState("ValoreVisibilitaDefaultPaziente") = GetValoreVisibilitaDatiVaccinaliDefault(OnVacUtility.Variabili.PazId)
            End If
            Return ViewState("ValoreVisibilitaDefaultPaziente").ToString()
        End Get
    End Property

#End Region

#Region " Eventi pagina "

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If Not IsPostBack() Then

            nMod = 0
            arVacEx = New ArrayList()

            OnVacUtility.ImpostaIntestazioniPagina(OnitLayout31, LayoutTitolo, Nothing, Settings, IsGestioneCentrale)

            If IsGestioneCentrale Then

                LayoutTitolo_sezione.Text += " [CENTRALE]"
                Toolbar.Visible = False

                CaricaEscluse(False)

            Else
                ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK

                ' Visibilità pulsante Recupera Storico 
                Toolbar.Items.FindItemByValue("btnRecuperaStoricoVacc").Visible = FlagConsensoVaccUslCorrente

                If FlagConsensoVaccUslCorrente Then

                    Dim statoAcquisizioneDatiVaccinaliCentrale As Enumerators.StatoAcquisizioneDatiVaccinaliCentrale? =
                         Common.OnVacStoricoVaccinaleCentralizzato.GetStatoAcquisizioneDatiVaccinaliCentralePaziente(OnVacUtility.Variabili.PazId)

                    If Not statoAcquisizioneDatiVaccinaliCentrale.HasValue OrElse
                       statoAcquisizioneDatiVaccinaliCentrale = Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Parziale Then

                        strJS &= Common.OnVacStoricoVaccinaleCentralizzato.AlertMessageRecuperoStoricoVaccinale

                        SetToolbarStatus(False, True)
                        CaricaEscluse(False)

                    Else
                        SetToolbarStatus(True, False)
                        CaricaEscluse(True)
                    End If

                Else
                    SetToolbarStatus(True, False)
                    CaricaEscluse(True)
                End If

            End If
        End If

    End Sub

#End Region

#Region " Eventi toolbar "

    Protected Sub Toolbar_ButtonClick(sender As Object, e As Telerik.Web.UI.RadToolBarEventArgs) Handles Toolbar.ButtonClick

        Select Case e.Item.Value

            Case "btn_Salva"

                Me.Salva()

                Me.CaricaEscluse(True)

            Case "btn_Annulla"

                Me.Annulla()

            Case "btn_Inserisci"

                Me.Inserisci()

            Case "btnRecuperaStoricoVacc"

                Me.RecuperaStoricoVaccinale()

            Case "btn_Modifica_Data_Scadenza"

                Me.ModificaDataScadenzaMultipla()

            Case "btn_Rinnova_Esclusione"

                Me.ApriRinnovaEsclusione()

        End Select

    End Sub

#End Region

#Region " Eventi datagrid "

    Private Sub dg_vacEx_CancelCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_vacEx.CancelCommand

        Me.dg_vacEx.EditItemIndex = -1

        BindDatagrid()

        Me.nMod -= 1
        If Me.nMod = 0 Then Me.OnitLayout31.Busy = False

        SetToolbarStatus(True, False)

    End Sub

    Private Sub dg_vacEx_DeleteCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_vacEx.DeleteCommand

        Dim row As DataRow = Me.FindRow(Me.FindRowKey(e.Item.ItemIndex))

        If Me.AvvertiVaccinazioneNonModificabile(row) Then

            row.Delete()

            Me.BindDatagrid()

            Me.nMod += 1
            Me.OnitLayout31.Busy = True

        End If

    End Sub

    Private Sub dg_vacEx_EditCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_vacEx.EditCommand

        Me.rowKey = Me.FindRowKey(e.Item.ItemIndex)

        Dim row As DataRow = Me.FindRow(rowKey)

        If Me.AvvertiVaccinazioneNonModificabile(row) Then

            Me.dg_vacEx.EditItemIndex = e.Item.ItemIndex

            Me.BindDatagrid()

            Me.nMod += 1
            Me.OnitLayout31.Busy = True

            SetToolbarStatus(False, False)

            Me.strJS &= "OnitDatePick.tb_data_visita_edit[0].Focus(1,true)" + Environment.NewLine

        End If

    End Sub

    Private Sub dg_vacEx_UpdateCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_vacEx.UpdateCommand

        ' N.B. : i controlli sui valori inseriti nella riga sono solo lato client.

        Dim row As DataRow = FindRow(rowKey)

        Dim fmMedicoEdit As OnitModalList = DirectCast(dg_vacEx.Items(e.Item.ItemIndex).FindControl("fm_medico_edit"), OnitModalList)
        Dim fmMotivoEdit As OnitModalList = DirectCast(dg_vacEx.Items(e.Item.ItemIndex).FindControl("fm_motivo_edit"), OnitModalList)

        row("vex_data_visita") = DirectCast(dg_vacEx.Items(e.Item.ItemIndex).FindControl("tb_data_visita_edit"), OnitDatePick).Data
        row("vex_ope_codice") = fmMedicoEdit.Codice
        row("ope_nome") = fmMedicoEdit.Descrizione
        row("vex_moe_codice") = fmMotivoEdit.Codice
        row("moe_descrizione") = fmMotivoEdit.Descrizione

        Dim dataScadenza As DateTime
        If DateTime.TryParse(DirectCast(Me.dg_vacEx.Items(e.Item.ItemIndex).FindControl("tb_data_scadenza_edit"), OnitDatePick).Text, dataScadenza) Then
            row("vex_data_scadenza") = dataScadenza
        Else
            row("vex_data_scadenza") = DBNull.Value
        End If

        If row("vex_data_scadenza") Is DBNull.Value OrElse row("vex_data_scadenza") > DateTime.Now Then
            row("s") = False
        Else
            row("s") = True
        End If

        Dim txtDose As OnitJsValidator = DirectCast(dg_vacEx.Items(e.Item.ItemIndex).FindControl("txtDose"), OnitJsValidator)
        txtDose.Text = txtDose.Text.Trim()

        Dim result As Biz.BizVaccinazioniEscluse.CheckValoreDoseResult = Biz.BizVaccinazioniEscluse.CheckValoreDose(txtDose.Text, True)

        If Not result.Success Then
            strJS &= String.Format("alert('{0}');", HttpUtility.JavaScriptStringEncode(result.Message))
            Return
        End If

        row("vex_dose") = result.NumeroDose.Value

        Dim txtNote As TextBox = DirectCast(dg_vacEx.Items(e.Item.ItemIndex).FindControl("txtNote"), TextBox)
        txtNote.Text = txtNote.Text.Trim()

        If txtNote.Text.Length > txtNote.MaxLength Then
            txtNote.Text = txtNote.Text.Substring(0, txtNote.MaxLength)
        End If

        row("vex_note") = txtNote.Text

        dg_vacEx.EditItemIndex = -1

        BindDatagrid()

        SetToolbarStatus(True, False)

    End Sub

    Private Sub dg_vacEx_ItemDataBound(sender As Object, e As DataGridItemEventArgs) Handles dg_vacEx.ItemDataBound

        Dim count As Integer

        If e.Item.ItemIndex <> -1 Then

            Using dbGenericProviderFactory As New Biz.DbGenericProviderFactory()

                Dim genericProvider As DAL.DbGenericProvider = dbGenericProviderFactory.GetDbGenericProvider(OnVacContext.AppId, OnVacContext.Azienda)

                Using bizVaccinazioniEscluse As New Biz.BizVaccinazioniEscluse(dbGenericProviderFactory, Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.VAC_ESCLUSE))

                    count = bizVaccinazioniEscluse.GetVaccinazioniEscluseEliminateByPazienteVaccinazione(OnVacUtility.Variabili.PazId, DirectCast(e.Item.FindControl("tb_codVac"), Label).Text, String.Empty, True).Count
                    e.Item.FindControl("btnDettaglioStoricoRinnovi").Visible = (count > 0)
                    e.Item.FindControl("lblRinnovo").Visible = (count > 0)
                End Using

            End Using

        End If

    End Sub

    Private Sub dg_vacEx_ItemCommand(source As Object, e As DataGridCommandEventArgs) Handles dg_vacEx.ItemCommand

        Select Case e.CommandName

            Case "DettaglioStorico"

                Dim codiceVaccinazione As Label = DirectCast(Me.dg_vacEx.Items(e.Item.ItemIndex).FindControl("tb_codVac"), Label)

                StoricoRinnovi.Inizializza(codiceVaccinazione.Text)

                StoricoRinnovi.ModaleName = modStoricoRinnovi.ClientID
                modStoricoRinnovi.VisibileMD = True
                OnitLayout31.Busy = True

        End Select

    End Sub

#End Region

#Region " Eventi modale "

    Protected Sub fmMotivo_Change(sender As Object, e As Controls.OnitModalList.ModalListaEventArgument)

        Dim fmMotivo As OnitModalList = DirectCast(sender, OnitModalList)
        Dim tb_data_scadenza As OnitDatePick = DirectCast(fmMotivo.Parent.FindControl("tb_data_scadenza_edit"), OnitDatePick)
        Dim tb_data_visita As OnitDatePick = DirectCast(fmMotivo.Parent.FindControl("tb_data_visita_edit"), OnitDatePick)

        If tb_data_scadenza.Data = Date.MinValue Then

            Dim codiceVaccinazioneCorrente As String = DirectCast(fmMotivo.Parent.FindControl("tb_codVac"), Label).Text

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using biz As New Biz.BizMotiviEsclusione(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                    tb_data_scadenza.Data = biz.GetScadenzaMotivoEsclusione(OnVacUtility.Variabili.PazId, fmMotivo.Codice, codiceVaccinazioneCorrente, tb_data_visita.Data)

                End Using
            End Using

        End If

    End Sub

#End Region

#Region " Eventi user controls "

    'riabilita il layout alla chiusura della modale delle vaccinazioni (modifica 08/07/2004)
    Private Sub InsVacEscluse_RiabilitaLayout() Handles InsVacEscluse.RiabilitaLayout

        RicaricaDGEscluse()
        OnitLayout31.Busy = False

    End Sub

    'recupera i codici delle vaccinazioni selezionati nella modale (modifica 08/07/2004)
    Private Sub InsVacEscluse_Conferma(listEscluse As List(Of InsVacEsc.ResultItem)) Handles InsVacEscluse.Conferma

        Dim row As DataRow = Nothing

        'inserimento dei dati nel datatable di salvataggio e nel datagrid
        For Each item As InsVacEsc.ResultItem In listEscluse

            row = dt_vacEx.NewRow()

            row("vex_data_visita") = item.DataVisita
            row("vex_vac_codice") = item.VaccinazioneCodice
            row("vac_descrizione") = item.VaccinazioneDescrizione
            row("vex_ope_codice") = item.MedicoCodice
            row("ope_nome") = item.MedicoNome
            row("vex_moe_codice") = item.MotivoCodice
            row("moe_descrizione") = item.MotivoDescrizione
            row("vex_dose") = item.NumeroDose
            row("vex_note") = item.Note

            row("vex_flag_visibilita") = ValoreVisibilitaDefaultPaziente

            If item.DataScadenza <> Date.MinValue Then
                row("vex_data_scadenza") = item.DataScadenza
            End If

            ' Flag scadenza
            If row("vex_data_scadenza") Is DBNull.Value OrElse
               row("vex_data_scadenza") = DateTime.MinValue OrElse
               row("vex_data_scadenza") > Date.Now() Then

                row("s") = False

            Else
                row("s") = True
            End If

            dt_vacEx.Rows.Add(row)

        Next

        modInserimentoEsclusioni.VisibileMD = False

        BindDatagrid()

    End Sub

    Private Sub ModDataVacEsc_RiabilitaLayout() Handles ModDataVacEsc.RiabilitaLayout

        modModificaDataScadenzaMultipla.VisibileMD = False
        Me.RicaricaDGEscluse()
        Me.OnitLayout31.Busy = False

    End Sub

    Private Sub ModDataVacEsc_Conferma(dataModifica As Date) Handles ModDataVacEsc.Conferma

        Dim chk As CheckBox = Nothing
        Dim row As DataRow = Nothing
        Dim rows As DataRow() = Nothing
        Dim index As Integer

        dt_vacEx.Columns.Add("chkSelezione")

        For i As Int16 = 0 To dg_vacEx.Items.Count - 1

            Dim codVaccinazione As String = DirectCast(dg_vacEx.Items(i).FindControl("tb_codVac"), Label).Text

            If Not String.IsNullOrWhiteSpace(codVaccinazione) Then

                row = dt_vacEx.Select(String.Format("VEX_VAC_CODICE = '{0}'", codVaccinazione)).FirstOrDefault()

                If Not row Is Nothing Then
                    index = dt_vacEx.Rows.IndexOf(row)

                    row.Item("chkSelezione") = (DirectCast(Me.dg_vacEx.Items(i).FindControl("chkSelezione"), CheckBox).Checked).ToString()

                    If Convert.ToBoolean(row.Item("chkSelezione")) Then
                        If Not dataModifica = Date.MinValue AndAlso Convert.ToDateTime(row.Item("vex_data_visita")) >= dataModifica Then
                            AlertClientMsg("Inserire una data di scadenza superiore rispetto alla data di visita più recente delle vaccinazioni selezionate.")
                            dt_vacEx.Columns.Remove("chkSelezione")
                            Exit Sub
                            Me.dt_vacEx.PrimaryKey = Nothing
                            Me.dt_vacEx.RejectChanges()
                        End If

                        If Not dataModifica = Date.MinValue Then
                            dt_vacEx.Rows(index).Item("vex_data_scadenza") = dataModifica.ToShortDateString.ToString()
                        Else
                            dt_vacEx.Rows(index).Item("vex_data_scadenza") = DBNull.Value
                        End If

                    End If

                End If

            End If
        Next

        dt_vacEx.Columns.Remove("chkSelezione")

        'BindDatagrid()

        modModificaDataScadenzaMultipla.VisibileMD = False
        'Me.RicaricaDGEscluse()
        Me.OnitLayout31.Busy = True

        Salva()

        CaricaEscluse(True)

    End Sub

    Private Sub RinnovaEsclusione_Conferma(listRinnovi As List(Of RinnovaEsc.ResultRinnovaEsclusioneItem)) Handles RinnovaEsclusione.Conferma

        Dim vaccinazioneEsclusaOld As Entities.VaccinazioneEsclusa = Nothing
        Dim vaccinazioneEsclusaNew As Entities.VaccinazioneEsclusa = Nothing
        Dim rinnovo As New RinnovaEsc.ResultRinnovaEsclusioneItem

        Dim newRow As DataRow = Nothing
        Dim oldRow As DataRow = Nothing
        Dim codiciVaccinazioniRinnovate As New List(Of String)

        dt_vacEx.Columns.Add("chkSelezione")

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizVaccinazioniEscluse As New Biz.BizVaccinazioniEscluse(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.VAC_ESCLUSE))

                For i As Int16 = 0 To dg_vacEx.Items.Count - 1

                    Dim codVaccinazione As String = DirectCast(dg_vacEx.Items(i).FindControl("tb_codVac"), Label).Text

                    If Not String.IsNullOrWhiteSpace(codVaccinazione) Then

                        oldRow = dt_vacEx.Select(String.Format(" VEX_VAC_CODICE = '{0}' ", codVaccinazione)).FirstOrDefault()

                        If Not oldRow Is Nothing Then

                            'If Not String.IsNullOrWhiteSpace(oldRow("vex_data_scadenza").ToString()) AndAlso Convert.ToDateTime(oldRow("vex_data_scadenza")) < Date.Now Then

                            vaccinazioneEsclusaOld = CreateVaccinazioneEsclusaFromDataRow(oldRow)

                            rinnovo = listRinnovi.Where(Function(p) p.VaccinazioneCodice = codVaccinazione).FirstOrDefault()

                            If Not rinnovo Is Nothing AndAlso rinnovo.NuovaDataScadenza.HasValue Then

                                Dim salvaVaccinazioneEsclusaScadutaCommand As New Biz.BizVaccinazioniEscluse.SalvaVaccinazioneEsclusaCommand()

                                salvaVaccinazioneEsclusaScadutaCommand.VaccinazioneEsclusa = vaccinazioneEsclusaOld
                                salvaVaccinazioneEsclusaScadutaCommand.Operation = Biz.BizClass.SalvaCommandOperation.Delete

                                bizVaccinazioniEscluse.SalvaVaccinazioneEsclusa(salvaVaccinazioneEsclusaScadutaCommand, Constants.StatoVaccinazioniEscluseEliminate.Rinnovata)

                                OnVacUtility.EliminaInadempienza(vaccinazioneEsclusaOld.CodiceVaccinazione, OnVacUtility.Variabili.PazId, genericProvider.DAM, True)

                                vaccinazioneEsclusaNew = vaccinazioneEsclusaOld

                                'vaccinazioneEsclusaNew.DataVisita = vaccinazioneEsclusaOld.DataScadenza.AddDays(1)
                                vaccinazioneEsclusaNew.DataVisita = rinnovo.NuovaDataVisita
                                vaccinazioneEsclusaNew.DataScadenza = rinnovo.NuovaDataScadenza
                                vaccinazioneEsclusaNew.CodiceOperatore = Nothing
                                vaccinazioneEsclusaNew.DataRegistrazione = DateTime.MinValue
                                'Cosi viene richiamata la sequence collegata alle vac escluse
                                vaccinazioneEsclusaNew.Id = 0

                                Dim salvaVaccinazioneEsclusaRinnovataCommand As New Biz.BizVaccinazioniEscluse.SalvaVaccinazioneEsclusaCommand()

                                salvaVaccinazioneEsclusaRinnovataCommand.VaccinazioneEsclusa = vaccinazioneEsclusaNew
                                salvaVaccinazioneEsclusaRinnovataCommand.Operation = Biz.BizClass.SalvaCommandOperation.Insert

                                bizVaccinazioniEscluse.SalvaVaccinazioneEsclusa(salvaVaccinazioneEsclusaRinnovataCommand)

                                'creo l'eventuale inadempienza                                
                                OnVacUtility.CreaInadempienza(vaccinazioneEsclusaNew.CodiceMotivoEsclusione, vaccinazioneEsclusaNew.CodiceVaccinazione, OnVacUtility.Variabili.PazId, vaccinazioneEsclusaNew.DataVisita, OnVacContext.UserId.ToString(), genericProvider.DAM, True)

                                codiciVaccinazioniRinnovate.Add(vaccinazioneEsclusaNew.CodiceVaccinazione.ToString())

                                'End If
                            End If
                        End If
                    End If
                Next
            End Using
        End Using

        dt_vacEx.Columns.Remove("chkSelezione")

        CaricaEscluse(True)

        modRinnovaEsclusione.VisibileMD = False
        OnitLayout31.Busy = False

    End Sub

    Private Sub RinnovaEsclusione_RiabilitaLayout() Handles RinnovaEsclusione.RiabilitaLayout

        modRinnovaEsclusione.VisibileMD = False
        Me.RicaricaDGEscluse()
        Me.OnitLayout31.Busy = False

    End Sub

#End Region

#Region " Private "

    Private Sub CaricaEscluse(editable As Boolean)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizEscluse As New Biz.BizVaccinazioniEscluse(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                dt_vacEx = bizEscluse.GetVaccinazioniEscluse(OnVacUtility.Variabili.PazId, IsGestioneCentrale)

            End Using
        End Using

        Dim col As New DataColumn("s", GetType(Boolean))
        dt_vacEx.Columns.Add(col)
        dt_vacEx.AcceptChanges()

        SetFlagScadenza()

        dg_vacEx.DataSource = dt_vacEx.DefaultView
        dg_vacEx.DataBind()

        SetDataTablePrimaryKey()

        SetColumnVisibility("usl_inserimento_vex_descr", True)
        SetColumnVisibility("vex_flag_visibilita", True)

        ' Se la pagina non è in modifica (recupero storico non effettuato), i pulsanti di modifica ed eliminazione non devono essere visibili
        dg_vacEx.Columns(DgVacExColumnIndex.DeleteButton).Visible = editable
        dg_vacEx.Columns(DgVacExColumnIndex.UpdateButton).Visible = editable

    End Sub

    Private Sub Inserisci()

        'caricamento della modale contenente l'elenco delle vaccinazioni
        InsVacEscluse.ModaleName = modInserimentoEsclusioni.ClientID
        InsVacEscluse.Inizializza(dt_vacEx)

        modInserimentoEsclusioni.VisibileMD = True

        OnitLayout31.Busy = True

    End Sub

    Private Sub ApriRinnovaEsclusione()

        Dim chk As CheckBox = Nothing
        Dim row As DataRow = Nothing
        Dim rows As DataRow() = Nothing
        Dim index As Integer
        Dim hasAnyChecked As Boolean = False

        For i As Int16 = 0 To Me.dt_vacEx.Rows.Count - 1
            If (DirectCast(Me.dg_vacEx.Items(i).FindControl("chkSelezione"), CheckBox).Checked) Then
                hasAnyChecked = True
                Exit For
            End If
        Next
        If hasAnyChecked Then

            dt_vacEx.Columns.Add("chkSelezione")
            dt_vacEx.Columns.Add("newDataScadenza")
            dt_vacEx.Columns.Add("newDataVisita")

            For i As Int16 = 0 To dg_vacEx.Items.Count - 1

                Dim codVaccinazione As String = DirectCast(dg_vacEx.Items(i).FindControl("tb_codVac"), Label).Text

                If Not String.IsNullOrWhiteSpace(codVaccinazione) Then

                    row = dt_vacEx.Select(String.Format("VEX_VAC_CODICE = '{0}'", codVaccinazione)).FirstOrDefault()

                    If Not row Is Nothing Then
                        index = dt_vacEx.Rows.IndexOf(row)

                        row.Item("chkSelezione") = (DirectCast(Me.dg_vacEx.Items(i).FindControl("chkSelezione"), CheckBox).Checked).ToString()

                        If Convert.ToBoolean(row.Item("chkSelezione")) Then
                            dt_vacEx.Rows(index).Item("chkSelezione") = True
                        Else
                            dt_vacEx.Rows(index).Item("chkSelezione") = False

                        End If

                    End If

                End If
            Next

            'Dim dr As DataRow() = dt_vacEx.Select("chkSelezione = True")

            Dim dv As DataView = dt_vacEx.DefaultView
            dv.RowFilter = "chkSelezione = True"

            Dim dataScadenzaEsclusione As New DateTime
            Dim newDataVisita As New DateTime?

            For Each d As DataRowView In dv
                Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                    Using bizMotiviEsclusione As New Biz.BizMotiviEsclusione(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                        If Not String.IsNullOrWhiteSpace(d.Item("vex_data_scadenza").ToString) Then
                            d.Item("newDataVisita") = Date.Now
                        End If
                        If Not String.IsNullOrWhiteSpace(d.Item("newDataVisita").ToString()) Then
                            dataScadenzaEsclusione = bizMotiviEsclusione.GetScadenzaMotivoEsclusione(OnVacUtility.Variabili.PazId, d.Item("vex_moe_codice"), d.Item("vex_vac_codice"), d.Item("newDataVisita"))
                        Else
                            dataScadenzaEsclusione = Date.MinValue
                        End If

                        d.Item("newDataScadenza") = dataScadenzaEsclusione
                    End Using
                End Using
            Next

            RinnovaEsclusione.Inizializza(dv)
            dt_vacEx.Columns.Remove("chkSelezione")
            dt_vacEx.Columns.Remove("newDataScadenza")
            dt_vacEx.Columns.Remove("newDataVisita")
            RinnovaEsclusione.ModaleName = Me.modRinnovaEsclusione.ClientID

            Me.modRinnovaEsclusione.VisibileMD = True
            Me.OnitLayout31.Busy = True
        Else
            AlertClientMsg("Selezionare almeno una vaccinazione dall'elenco.")
            Return
        End If



    End Sub

    Private Sub ModificaDataScadenzaMultipla()

        Dim hasAnyChecked As Boolean = False

        For i As Int16 = 0 To Me.dt_vacEx.Rows.Count - 1
            If (DirectCast(Me.dg_vacEx.Items(i).FindControl("chkSelezione"), CheckBox).Checked) Then
                hasAnyChecked = True
                Exit For
            End If
        Next
        If hasAnyChecked Then
            ModDataVacEsc.ModaleName = Me.modModificaDataScadenzaMultipla.ClientID

            Me.modModificaDataScadenzaMultipla.VisibileMD = True
            Me.OnitLayout31.Busy = True
        Else
            AlertClientMsg("Selezionare almeno una vaccinazione dall'elenco.")
            Return
        End If

    End Sub

    Private Sub Salva()

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, OnVacUtility.GetReadCommittedTransactionOptions())

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                'la variabile inviaFSE serve solo per fare inviare la chiamata di indicizzazione al Registry solo nei casi di una nuova vaccinazione inserita 
                'o anche di una vaccinazione modificata (per eventuale allineamento di una paziente)
                Dim inviaFSE As Boolean

                Using bizVaccinazioniEscluse As New Biz.BizVaccinazioniEscluse(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.VAC_ESCLUSE))

                    For Each row As DataRow In dt_vacEx.Rows

                        Select Case row.RowState

                            Case DataRowState.Deleted

                                Dim vaccinazioneEsclusa As Entities.VaccinazioneEsclusa = CreateVaccinazioneEsclusaFromDataRow(row)

                                Dim salvaVaccinazioneEsclusaCommand As New Biz.BizVaccinazioniEscluse.SalvaVaccinazioneEsclusaCommand()
                                salvaVaccinazioneEsclusaCommand.VaccinazioneEsclusa = vaccinazioneEsclusa
                                salvaVaccinazioneEsclusaCommand.Operation = Biz.BizClass.SalvaCommandOperation.Delete

                                bizVaccinazioniEscluse.SalvaVaccinazioneEsclusa(salvaVaccinazioneEsclusaCommand)

                                ' TODO: LOG => spostare nel bizvacescluse
                                Dim recordLog As New Record()
                                recordLog.Campi.Add(New Campo("VEX_VAC_CODICE", row("VEX_VAC_CODICE", DataRowVersion.Original).ToString()))

                                Dim testataLog As New Testata(DataLogStructure.TipiArgomento.VAC_ESCLUSE, Operazione.Eliminazione, False)
                                testataLog.Records.Add(recordLog)

                                LogBox.WriteData(testataLog)

                                'cancello l'eventuale inadempienza collegata
                                OnVacUtility.EliminaInadempienza(row("vex_vac_codice", DataRowVersion.Original).ToString(), OnVacUtility.Variabili.PazId, genericProvider.DAM, True)

                            Case DataRowState.Added

								Dim vaccinazioneEsclusa As Entities.VaccinazioneEsclusa = CreateVaccinazioneEsclusaFromDataRow(row)

								Dim salvaVaccinazioneEsclusaCommand As New Biz.BizVaccinazioniEscluse.SalvaVaccinazioneEsclusaCommand()
								salvaVaccinazioneEsclusaCommand.VaccinazioneEsclusa = vaccinazioneEsclusa
								salvaVaccinazioneEsclusaCommand.Operation = Biz.BizClass.SalvaCommandOperation.Insert

								bizVaccinazioniEscluse.SalvaVaccinazioneEsclusa(salvaVaccinazioneEsclusaCommand)

								' TODO: LOG => spostare nel bizvacescluse
								Dim testataLog As New Testata(DataLogStructure.TipiArgomento.VAC_ESCLUSE, Operazione.Inserimento, False)
								testataLog.Records.Add(CreateRecordLogInsertVaccinazioneEsclusa(row))

								LogBox.WriteData(testataLog)

								'creo l'eventuale inadempienza
								OnVacUtility.CreaInadempienza(row("vex_moe_codice").ToString(), row("vex_vac_codice").ToString(), OnVacUtility.Variabili.PazId, row("vex_data_visita"), OnVacContext.UserId.ToString(), genericProvider.DAM, True)

								inviaFSE = True

							Case DataRowState.Modified

								Dim vaccinazioneEsclusa As Entities.VaccinazioneEsclusa = CreateVaccinazioneEsclusaFromDataRow(row)

								Dim salvaVaccinazioneEsclusaCommand As New Biz.BizVaccinazioniEscluse.SalvaVaccinazioneEsclusaCommand()
								salvaVaccinazioneEsclusaCommand.VaccinazioneEsclusa = vaccinazioneEsclusa
								salvaVaccinazioneEsclusaCommand.Operation = Biz.BizClass.SalvaCommandOperation.Update

								bizVaccinazioniEscluse.SalvaVaccinazioneEsclusa(salvaVaccinazioneEsclusaCommand)

								' TODO: LOG => spostare nel bizvacescluse
								Dim testataLog As New Testata(DataLogStructure.TipiArgomento.VAC_ESCLUSE, Operazione.Modifica, False)
								testataLog.Records.Add(CreateRecordLogUpdateVaccinazioneEsclusa(row))

								If testataLog.ChangedValues Then LogBox.WriteData(testataLog)

								'Se è cambiato il motivo di esclusione cancello la vecchia inadempienza e ne creo (se necessario) una nuova
								If row("vex_moe_codice", DataRowVersion.Original).ToString() <> row("vex_moe_codice").ToString() Then
									OnVacUtility.EliminaInadempienza(row("vex_vac_codice").ToString(), OnVacUtility.Variabili.PazId, genericProvider.DAM, True)
									OnVacUtility.CreaInadempienza(row("vex_moe_codice").ToString(), row("vex_vac_codice").ToString(), OnVacUtility.Variabili.PazId, row("vex_data_visita").ToString(), OnVacContext.UserId.ToString(), genericProvider.DAM, True)
								End If

								inviaFSE = True

						End Select

					Next

				End Using

				OnVacUtility.ControllaSeTotalmenteInadempiente(OnVacUtility.Variabili.PazId, genericProvider.DAM, Settings)

				Using bizConvocazione As New Biz.BizConvocazione(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.VAC_ESCLUSE, True))

					Dim command As New Biz.BizConvocazione.EliminaConvocazioneEmptyCommand()
					command.CodicePaziente = Convert.ToInt64(OnVacUtility.Variabili.PazId)
					command.DataConvocazione = Nothing
					command.DataEliminazione = DateTime.Now
					command.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.Esclusione
					command.NoteEliminazione = "Eliminazione appuntamento per inserimento esclusione da maschera Vaccinazioni Escluse"
					command.WriteLog = True

					bizConvocazione.EliminaConvocazioneEmpty(command)

				End Using

#Region " FSE "
				If Settings.FSE_GESTIONE Then
					If inviaFSE Then

						Dim indicizzazioneResult As Biz.BizGenericResult = OnVacUtility.FSEHelper.IndicizzaSuRegistry(
							Convert.ToInt64(OnVacUtility.Variabili.PazId),
							Constants.TipoDocumentoFSE.CertificatoVaccinale,
							Constants.FunzionalitaNotificaFSE.VacEscluse_Salva,
							Constants.EventoNotificaFSE.InserimentoVaccinazione,
							Settings,
							String.Empty)

						If Not indicizzazioneResult.Success AndAlso Not String.IsNullOrWhiteSpace(indicizzazioneResult.Message) Then
							Me.OnitLayout31.InsertRoutineJS("alert('Indicizzazione sul Registry Regionale non avvenuta!');")
						End If

					End If
				End If
#End Region

			End Using

			transactionScope.Complete()

		End Using

		dt_vacEx.AcceptChanges()

		nMod = 0

		Me.OnitLayout31.Busy = False

	End Sub

	Private Sub Annulla()

		Me.dt_vacEx.PrimaryKey = Nothing
		Me.dt_vacEx.RejectChanges()

		SetFlagScadenza()
		BindDatagrid()

		Me.OnitLayout31.Busy = False

		nMod = 0

		SetDataTablePrimaryKey()

	End Sub

	Private Sub SetDataTablePrimaryKey()

		If Me.IsGestioneCentrale Then
			OnVacUtility.addKey(dt_vacEx, "vex_vac_codice", "vex_id")
		Else
			OnVacUtility.addKey(dt_vacEx, "vex_vac_codice")
		End If

	End Sub

	Private Sub SetFlagScadenza()

		Dim row As DataRow

		For Each row In Me.dt_vacEx.Rows
			If (row("vex_data_scadenza") Is DBNull.Value OrElse row("vex_data_scadenza") = DateTime.MinValue OrElse row("vex_data_scadenza") > Date.Now) Then
				row("s") = False
			Else
				row("s") = True
			End If
		Next

		Me.dt_vacEx.AcceptChanges()

	End Sub

	Private Sub BindDatagrid()

		Me.dt_vacEx.DefaultView.Sort = "vex_vac_codice"
		Me.dt_vacEx.DefaultView.Sort = ""

		Me.dg_vacEx.DataSource = Me.dt_vacEx.DefaultView
		Me.dg_vacEx.DataBind()

	End Sub

	'ricarica il datagrid all'annullamento dell'operazione
	Private Sub RicaricaDGEscluse()

		Me.dt_vacEx.PrimaryKey = Nothing
		Me.dt_vacEx.RejectChanges()

		SetFlagScadenza()
		BindDatagrid()

	End Sub

	Private Function FindRowKey(rowIndex As Integer) As Object()

		Dim codiceVaccinazioneCorrente As String = DirectCast(Me.dg_vacEx.Items(rowIndex).FindControl("tb_codVac"), Label).Text

		If Me.IsGestioneCentrale Then

			Dim key(1) As Object

			key(0) = codiceVaccinazioneCorrente
			key(1) = DirectCast(Me.dg_vacEx.Items(rowIndex).FindControl("hdCodiceUslInserimento"), HiddenField).Value

			Return key

		End If

		Return New Object() {codiceVaccinazioneCorrente}

	End Function

	Private Function FindRow(rowKey As Object()) As DataRow

		If Me.IsGestioneCentrale Then
			Me.dt_vacEx.PrimaryKey = New DataColumn() {Me.dt_vacEx.Columns("vex_vac_codice"), Me.dt_vacEx.Columns("vex_usl_inserimento")}
		Else
			Me.dt_vacEx.PrimaryKey = New DataColumn() {Me.dt_vacEx.Columns("vex_vac_codice")}
		End If

		Dim row As DataRow = dt_vacEx.Rows.Find(rowKey)
		Me.dt_vacEx.PrimaryKey = Nothing

		Return row

	End Function

	Private Function AvvertiVaccinazioneNonModificabile(row As DataRow) As Boolean

		If dg_vacEx.EditItemIndex <> -1 Then
			strJS &= "alert('Cliccare AGGIORNA O ANNULLA della riga che si sta editando prima di modificare un\'altra riga.')"
			Return False
		End If

		' Controllo Usl inserimento uguale alla usl corrente => da non effettuare per le escluse
		'Dim gestioneStoricoVacc As New Common.OnVacStoricoVaccinaleCentralizzato(Me.Settings)

		'If FlagAbilitazioneVaccUslCorrente AndAlso Not gestioneStoricoVacc.CheckEsclusaStessaUsl(row) Then
		'    Me.strJS &= Common.OnVacStoricoVaccinaleCentralizzato.AlertMessageEsclusaNoUslCorrente
		'    Return False
		'End If

		If Not OnVacUtility.CheckGiorniTrascorsiVariazioneDatiVaccinali(row("vex_data_registrazione"), Settings) Then
			strJS &= String.Format("alert('Il numero di giorni trascorsi dalla data di registrazione è superiore al limite massimo impostato ({0}): impossibile effettuare la modifica.');",
								   Settings.GIORNI_VARIAZIONE_VACCINAZIONE_ESEGUITA.ToString())
			Return False
		End If

		Return True

	End Function

	Private Function CreateVaccinazioneEsclusaFromDataRow(row As DataRow) As Entities.VaccinazioneEsclusa

		Dim dataRowVersion As DataRowVersion = System.Data.DataRowVersion.Default

		If row.RowState = DataRowState.Deleted Then
			dataRowVersion = System.Data.DataRowVersion.Original
		End If

		Dim vaccinazioneEsclusa As New Entities.VaccinazioneEsclusa()

		If row.RowState <> DataRowState.Added Then
			vaccinazioneEsclusa.Id = row("vex_id", dataRowVersion)
			vaccinazioneEsclusa.CodiceUslInserimento = row("vex_usl_inserimento", dataRowVersion).ToString()
			vaccinazioneEsclusa.DataRegistrazione = row("vex_data_registrazione", dataRowVersion)
			vaccinazioneEsclusa.IdUtenteRegistrazione = row("vex_ute_id_registrazione", dataRowVersion)
		End If

		vaccinazioneEsclusa.CodicePaziente = OnVacUtility.Variabili.PazId
		vaccinazioneEsclusa.CodiceVaccinazione = row("vex_vac_codice", dataRowVersion).ToString()

		If Not row("vex_data_visita", dataRowVersion) Is DBNull.Value Then
			vaccinazioneEsclusa.DataVisita = row("vex_data_visita", dataRowVersion)
		End If

		vaccinazioneEsclusa.CodiceMotivoEsclusione = row("vex_moe_codice", dataRowVersion).ToString()

		If Not row("vex_data_scadenza", dataRowVersion) Is DBNull.Value Then
			vaccinazioneEsclusa.DataScadenza = row("vex_data_scadenza", dataRowVersion)
		End If

		vaccinazioneEsclusa.CodiceOperatore = row("vex_ope_codice", dataRowVersion).ToString()
		vaccinazioneEsclusa.FlagVisibilita = ValoreVisibilitaDefaultPaziente

		If row("vex_dose", dataRowVersion) Is Nothing OrElse row("vex_dose", dataRowVersion) Is DBNull.Value Then
			vaccinazioneEsclusa.NumeroDose = Nothing
		Else
			Dim checkDoseResult As Biz.BizVaccinazioniEscluse.CheckValoreDoseResult =
				Biz.BizVaccinazioniEscluse.CheckValoreDose(row("vex_dose", dataRowVersion).ToString(), True)

			If checkDoseResult.Success Then
				vaccinazioneEsclusa.NumeroDose = checkDoseResult.NumeroDose.Value
			Else
				vaccinazioneEsclusa.NumeroDose = Nothing
			End If
		End If

		If row("vex_note", dataRowVersion) Is Nothing OrElse row("vex_note", dataRowVersion) Is DBNull.Value Then
			vaccinazioneEsclusa.Note = String.Empty
		Else
			vaccinazioneEsclusa.Note = row("vex_note", dataRowVersion).ToString()
		End If

		vaccinazioneEsclusa.IdACN = row("vex_id_acn", dataRowVersion).ToString()

		Return vaccinazioneEsclusa

	End Function

	Private Sub SetToolbarStatus(enableButtons As Boolean, enableRecuperaStoricoVacc As Boolean)

		Toolbar.Items.FindItemByValue("btn_Salva").Enabled = enableButtons
		Toolbar.Items.FindItemByValue("btn_Annulla").Enabled = enableButtons
		Toolbar.Items.FindItemByValue("btn_Inserisci").Enabled = enableButtons
		Toolbar.Items.FindItemByValue("btn_Modifica_Data_Scadenza").Enabled = enableButtons
		Toolbar.Items.FindItemByValue("btn_Rinnova_Esclusione").Enabled = enableButtons
		Toolbar.Items.FindItemByValue("btnRecuperaStoricoVacc").Enabled = enableRecuperaStoricoVacc

	End Sub

	Private Sub RecuperaStoricoVaccinale()

        Dim command As New Common.OnVacStoricoVaccinaleCentralizzato.AcquisisciStoricoCommand()
        command.CodicePaziente = OnVacUtility.Variabili.PazId
        command.RichiediConfermaSovrascrittura = False
        command.Settings = Me.Settings
        command.OnitLayout3 = Me.OnitLayout31
        command.BizLogOptions = OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.VAC_ESCLUSE)
        command.Note = "Recupero Storico Vaccinale da maschera VacEscluse"

        Dim acquisisciDatiVaccinaliCentraliResult As Biz.BizPaziente.AcquisisciDatiVaccinaliCentraliResult =
           Common.OnVacStoricoVaccinaleCentralizzato.AcquisisciDatiVaccinaliCentraliPaziente(command)

        If acquisisciDatiVaccinaliCentraliResult.StatoAcquisizioneDatiVaccinaliCentrale <> Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Parziale Then

            ' Se il recupero è andato a buon fine
            SetToolbarStatus(True, False)

        End If

        Me.CaricaEscluse(True)

    End Sub

    Private Sub SetColumnVisibility(sortExpression As String, visible As Boolean)

        Dim column As DataGridColumn = (From item As DataGridColumn In Me.dg_vacEx.Columns
                                        Where item.SortExpression = sortExpression
                                        Select item).FirstOrDefault()

        If Not column Is Nothing Then column.Visible = visible

    End Sub

#Region " Log "

    Private Function CreateRecordLogInsertVaccinazioneEsclusa(row As DataRow) As Record

        Dim recordLog As New Record()

        recordLog.Campi.Add(New Campo("VEX_DATA_VISITA", "", row("VEX_DATA_VISITA").ToString()))
        recordLog.Campi.Add(New Campo("VEX_VAC_CODICE", "", row("VEX_VAC_CODICE").ToString()))
        recordLog.Campi.Add(New Campo("VEX_OPE_CODICE", "", row("VEX_OPE_CODICE").ToString()))
        recordLog.Campi.Add(New Campo("MOE_DESCRIZIONE", "", row("MOE_DESCRIZIONE").ToString()))
        recordLog.Campi.Add(New Campo("VEX_DATA_SCADENZA", "", row("VEX_DATA_SCADENZA").ToString()))
        recordLog.Campi.Add(New Campo("VEX_ID", "", row("VEX_ID").ToString()))
        recordLog.Campi.Add(New Campo("VEX_USL_INSERIMENTO", "", row("VEX_USL_INSERIMENTO").ToString()))
        recordLog.Campi.Add(New Campo("VEX_FLAG_VISIBILITA", "", row("VEX_FLAG_VISIBILITA").ToString()))
        recordLog.Campi.Add(New Campo("VEX_DOSE", "", row("VEX_DOSE").ToString()))
        recordLog.Campi.Add(New Campo("VEX_NOTE", "", row("VEX_NOTE").ToString()))

        Return recordLog

    End Function

    Private Function CreateRecordLogUpdateVaccinazioneEsclusa(row As DataRow) As Record

        Dim recordLog As New Record()

        recordLog.Campi.Add(New Campo("VEX_DATA_VISITA", row("VEX_DATA_VISITA", DataRowVersion.Original).ToString(), row("VEX_DATA_VISITA").ToString()))
        recordLog.Campi.Add(New Campo("VEX_VAC_CODICE", row("VEX_VAC_CODICE").ToString(), row("VEX_VAC_CODICE").ToString()))
        recordLog.Campi.Add(New Campo("VEX_OPE_CODICE", row("VEX_OPE_CODICE", DataRowVersion.Original).ToString(), row("VEX_OPE_CODICE").ToString()))
        recordLog.Campi.Add(New Campo("MOE_DESCRIZIONE", row("MOE_DESCRIZIONE", DataRowVersion.Original).ToString(), row("MOE_DESCRIZIONE").ToString()))
        recordLog.Campi.Add(New Campo("VEX_DATA_SCADENZA", row("VEX_DATA_SCADENZA", DataRowVersion.Original).ToString(), row("VEX_DATA_SCADENZA").ToString()))
        recordLog.Campi.Add(New Campo("VEX_ID", row("VEX_ID", DataRowVersion.Original).ToString(), row("VEX_ID").ToString()))
        recordLog.Campi.Add(New Campo("VEX_USL_INSERIMENTO", row("VEX_USL_INSERIMENTO", DataRowVersion.Original).ToString(), row("VEX_USL_INSERIMENTO").ToString()))
        recordLog.Campi.Add(New Campo("VEX_FLAG_VISIBILITA", row("VEX_FLAG_VISIBILITA", DataRowVersion.Original).ToString(), row("VEX_FLAG_VISIBILITA").ToString()))
        recordLog.Campi.Add(New Campo("VEX_DOSE", row("VEX_DOSE", DataRowVersion.Original).ToString(), row("VEX_DOSE").ToString()))
        recordLog.Campi.Add(New Campo("VEX_NOTE", row("VEX_NOTE", DataRowVersion.Original).ToString(), row("VEX_NOTE").ToString()))

        Return recordLog

    End Function

#End Region

#End Region

#Region " Protected Methods "

    Protected Function BindFlagVisibilitaImageUrlValue(dataItem As Object) As String

        Return Common.OnVacStoricoVaccinaleCentralizzato.GetImageUrlFlagVisibilita(dataItem, "VEX_FLAG_VISIBILITA", Me)

    End Function

    Protected Function BindFlagVisibilitaToolTipValue(dataItem As Object) As String

        Return Common.OnVacStoricoVaccinaleCentralizzato.GetToolTipFlagVisibilita(dataItem, "VEX_FLAG_VISIBILITA")

    End Function

    Private Sub StoricoRinnovi_RiabilitaLayout() Handles StoricoRinnovi.RiabilitaLayout

        modStoricoRinnovi.VisibileMD = False
        Me.RicaricaDGEscluse()
        Me.OnitLayout31.Busy = False

    End Sub

#End Region

End Class
