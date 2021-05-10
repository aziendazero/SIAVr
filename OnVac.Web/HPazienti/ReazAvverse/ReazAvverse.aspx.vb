Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.Biz
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities


Partial Class OnVac_ReazAvverse
    Inherits Common.PageBase

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

    Private Property nMod() As Int16
        Get
            Return ViewState("OnVac_nMod")
        End Get
        Set(Value As Int16)
            ViewState("OnVac_nMod") = Value
        End Set
    End Property

    Private Property dt_vacEseguite() As DataTable
        Get
            If Session("OnVac_dt_vacEseguite") Is Nothing Then Return Nothing
            Return DirectCast(Session("OnVac_dt_vacEseguite"), SerializableDataTableContainer).Data
        End Get
        Set(Value As DataTable)
            If Session("OnVac_dt_vacEseguite") Is Nothing Then
                Session("OnVac_dt_vacEseguite") = New SerializableDataTableContainer()
            End If
            DirectCast(Session("OnVac_dt_vacEseguite"), SerializableDataTableContainer).Data = Value
        End Set
    End Property

    Private Property RowKey() As Object()
        Get
            Return Session("OnVac_rowKey")
        End Get
        Set(Value As Object())
            Session("OnVac_rowKey") = Value
        End Set
    End Property

    Private Property ShowMessaggioAggiornamentoAnagrafeCentrale() As Boolean
        Get
            If ViewState("OnVac_ShowMessaggioAggiornamentoAnagrafeCentrale") Is Nothing Then
                ViewState("OnVac_ShowMessaggioAggiornamentoAnagrafeCentrale") = True
            End If
            Return ViewState("OnVac_ShowMessaggioAggiornamentoAnagrafeCentrale")
        End Get
        Set(Value As Boolean)
            ViewState("OnVac_ShowMessaggioAggiornamentoAnagrafeCentrale") = Value
        End Set
    End Property

    Private Property StoricoVaccinaleCentralizzatoDaRecuperare() As Boolean
        Get
            If ViewState("StoricoVaccinaleCentralizzatoDaRecuperare") Is Nothing Then ViewState("StoricoVaccinaleCentralizzatoDaRecuperare") = False
            Return ViewState("StoricoVaccinaleCentralizzatoDaRecuperare")
        End Get
        Set(value As Boolean)
            ViewState("StoricoVaccinaleCentralizzatoDaRecuperare") = value
        End Set
    End Property

#End Region

#Region " Enums "

    Enum ColumnIndex
        BtnDettaglioReazione = 0
        BtnElimina = 1
        BtnStampa = 2
        ShowDettaglioVaccinazioni = 3
        DataOraVaccinazione = 4
        Associazione = 5
        Dose = 6
        NomeCommerciale = 7
        ReazioneAvversa = 8
        Reazione2 = 9
        Reazione3 = 10
        DataReazione = 11
        DescrizioneUslInserimentoReazione = 12
        FlagVisibilita = 13
        Scaduta = 14
        CodiceReazione = 15
        CodiceReazione2 = 16
        CodiceReazione3 = 17
        CodiceUslInserimento = 18
    End Enum

#End Region

#Region " Public "

    Public strJS As String
    Public Warning1 As WarningMessage
    Public Warning2 As WarningMessage

#End Region

#Region " Eventi Page "

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Warning1 = New WarningMessage(lblWarning1)
        Warning2 = New WarningMessage(lblWarning2)

        If Not IsPostBack() Then

            StoricoVaccinaleCentralizzatoDaRecuperare = False

            If IsGestioneCentrale Then

                LayoutTitolo_sezione.Text += " [CENTRALE]"
                ToolBar.Visible = False
                ToolBarDetail.Items.FromKeyButton("btn_Stampa_Mod1").Visible = False

            Else
                ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK

                ToolBar.Items.FromKeyButton("btnRecuperaStoricoVacc").Visible = FlagConsensoVaccUslCorrente

                ' Controllo se è gestito lo storico centralizzato
                If FlagConsensoVaccUslCorrente Then

                    Dim statoAcquisizioneDatiVaccinaliCentrale As Enumerators.StatoAcquisizioneDatiVaccinaliCentrale? =
                       Common.OnVacStoricoVaccinaleCentralizzato.GetStatoAcquisizioneDatiVaccinaliCentralePaziente(OnVacUtility.Variabili.PazId)

                    If Not statoAcquisizioneDatiVaccinaliCentrale.HasValue OrElse
                        statoAcquisizioneDatiVaccinaliCentrale = Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Parziale Then

                        strJS &= Common.OnVacStoricoVaccinaleCentralizzato.AlertMessageRecuperoStoricoVaccinale

                        StoricoVaccinaleCentralizzatoDaRecuperare = True

                    End If

                End If

            End If

            InizializzazionePagina()

        End If

    End Sub

#End Region

#Region " DataGrid Events "

    ' Inserimento/Modifica/Visualizzazione reazione avversa
    Private Sub dg_Reaz_SelectedIndexChanged(sender As Object, e As EventArgs) Handles dg_Reaz.SelectedIndexChanged

        Dim rowReazioneSelezionata As DataRow = FindRow(dg_Reaz.SelectedIndex)

        Dim dettaglioReazioneReadOnly As Boolean = False

        If IsGestioneCentrale Then

            dettaglioReazioneReadOnly = True

            ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
        ElseIf FlagAbilitazioneVaccUslCorrente AndAlso StoricoVaccinaleCentralizzatoDaRecuperare Then

            ' Storico gestito e dati da recuperare --> Visualizzazione read-only
            strJS &= "alert('Impossibile inserire o modificare la reazione avversa: non è stato effettuato il recupero dei dati vaccinali centralizzati.');"
            dettaglioReazioneReadOnly = True

            ' ------------------------------------ '
            ' [Unificazione Ulss]: Eliminato controllo usl inserimento
            ' ------------------------------------ '
            'ElseIf FlagAbilitazioneVaccUslCorrente AndAlso Not gestioneStoricoVacc.CheckReazioneAvversaStessaUsl(rowReazioneSelezionata) Then

            '    ' Storico gestito e la reazione è stata registrata da un'altra azienda --> Visualizzazione read-only
            '    strJS &= Common.OnVacStoricoVaccinaleCentralizzato.AlertMessageModificaReazioneNoUslCorrente
            '    dettaglioReazioneReadOnly = True
            ' ------------------------------------ '

        Else

            ' Controllo numero giorni trascorsi dalla registrazione della reazione avversa (indipendentemente dalla gestione dello storico centralizzato)
            If Not OnVacUtility.CheckGiorniTrascorsiVariazioneDatiVaccinali(rowReazioneSelezionata("vra_data_compilazione"), Settings) Then

                strJS &= String.Format("alert('Il numero di giorni trascorsi dalla data di compilazione è superiore al limite massimo impostato ({0}): impossibile effettuare la modifica.');", Settings.GIORNI_VARIAZIONE_VACCINAZIONE_ESEGUITA.ToString())
                dettaglioReazioneReadOnly = True

            End If

        End If

        If dettaglioReazioneReadOnly Then
            ReazAvverseDetail.StatoControlloCorrente = ReazAvverseDetail.StatoControllo.SolaLettura
        Else
            ReazAvverseDetail.StatoControlloCorrente = ReazAvverseDetail.StatoControllo.Modifica
        End If

        LoadControlDataReazAvverse(dg_Reaz.SelectedIndex)

        If dettaglioReazioneReadOnly Then

            SetToolbarDetailStatus(False)

        Else

            ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
            If FlagAbilitazioneVaccUslCorrente Then
                ShowAlertAggiornamentoStoricoCentralizzato()
            End If

            Me.nMod += 1
            SetToolbarStatus(nMod > 0)
            SetToolbarDetailStatus(True)

        End If

        mlvMaster.SetActiveView(pan_Det)
        dg_Reaz.SelectedIndex = -1

    End Sub

    ' Cancellazione reazione
    Private Sub dg_Reaz_DeleteCommand(source As Object, e As DataGridCommandEventArgs) Handles dg_Reaz.DeleteCommand

        If dg_Reaz.EditItemIndex > -1 Then

            strJS &= "alert('Cliccare AGGIORNA O ANNULLA della riga che si sta modificando prima di cancellare questa riga!')"
            Return

        End If

        Dim rowReazioneSelezionata As DataRow = FindRow(e.Item.ItemIndex)

        ' ------------------------------------ '
        ' [Unificazione Ulss]: Eliminato controllo usl inserimento
        ' ------------------------------------ '
        'Dim gestioneStoricoVacc As New Common.OnVacStoricoVaccinaleCentralizzato(Me.Settings)

        '' Controllo se esiste già una reazione registrata da un'altra azienda (se è gestito lo storico centralizzato)
        'If FlagAbilitazioneVaccUslCorrente AndAlso Not gestioneStoricoVacc.CheckReazioneAvversaStessaUsl(rowReazioneSelezionata) Then

        '    Me.strJS &= Common.OnVacStoricoVaccinaleCentralizzato.AlertMessageCancellazioneReazioneNoUslCorrente
        '    Return

        'End If
        ' ------------------------------------ '

        ' Controllo numero giorni trascorsi dalla registrazione della reazione avversa (indipendentemente dalla gestione dello storico centralizzato)
        If Not OnVacUtility.CheckGiorniTrascorsiVariazioneDatiVaccinali(rowReazioneSelezionata("vra_data_compilazione"), Settings) Then

            strJS &= String.Format("alert('Il numero di giorni trascorsi dalla data di compilazione è superiore al limite massimo impostato ({0}): impossibile effettuare l\'eliminazione.');",
                                   Settings.GIORNI_VARIAZIONE_VACCINAZIONE_ESEGUITA.ToString())
            Return

        End If

        ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
        If FlagAbilitazioneVaccUslCorrente Then
            ShowAlertAggiornamentoStoricoCentralizzato()
        End If

        For i As Integer = 0 To Me.dt_vacEseguite.Rows.Count - 1

            Dim cRow As DataRow = Me.dt_vacEseguite.Rows(i)

            If Not cRow Is rowReazioneSelezionata AndAlso cRow.RowState <> DataRowState.Deleted Then

                If cRow("ves_ass_n_dose").ToString() = rowReazioneSelezionata("ves_ass_n_dose").ToString() AndAlso
                   cRow("ves_ass_codice").ToString() = rowReazioneSelezionata("ves_ass_codice").ToString() AndAlso
                   cRow("ves_lot_codice").ToString() = rowReazioneSelezionata("ves_lot_codice").ToString() AndAlso
                   cRow("ves_data_effettuazione").ToString() = rowReazioneSelezionata("ves_data_effettuazione").ToString() Then

                    cRow.Delete()

                End If

            End If

        Next

        rowReazioneSelezionata.Delete()

        DataBindDatagrid()

        Me.nMod += 1
        SetToolbarStatus(Me.nMod > 0)

    End Sub

    ' Stampa modulo reazione
    Private Sub dg_Reaz_ItemCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_Reaz.ItemCommand

        Select Case e.CommandName

            Case "Stampa"

                Common.ReazioniAvverseCommon.StampaModuloReazioniAvverse(Me.FindRow(e.Item.ItemIndex), False, Me.Page, Me.Settings)

        End Select

    End Sub

#End Region

#Region " Toolbar Events "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btn_Salva"

                Me.Salva()

            Case "btn_Annulla"

                Me.Annulla()

            Case "btnRecuperaStoricoVacc"

                Me.RecuperaStoricoVaccinale()

        End Select

    End Sub

    Private Sub ToolBarDetail_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBarDetail.ButtonClicked

        Select Case e.Button.Key

            Case "btn_Indietro"

                Indietro()

            Case "btn_Conferma"

                Conferma()

            Case "btn_Stampa_Mod1"

                Dim listEseguite As List(Of Entities.VaccinazioneEseguita) = Me.ReazAvverseDetail.GetListEseguiteReazioniAvverse(False)

                Common.ReazioniAvverseCommon.StampaModuloReazioniAvverse(listEseguite(0), False, Me.Page, Me.Settings)

        End Select

    End Sub

#End Region

#Region " Private Methods "

#Region " Metodi elenco reazioni avverse "

    Private Function GetDataGridSort() As String

        ' Ordinamento iniziale in base al parametro impostato su db
        Dim paramSort As String = Me.Settings.LASTORDVACESEGUITE
        Dim sort As String = String.Empty
        Dim order As String = String.Empty

        If paramSort.Replace(";", "").Trim() = "" Then

            sort = "ves_data_effettuazione"
            order = "DESC"
            paramSort = sort + " " + order

        Else

            ' Formato di param_sort (da db): "campo1 verso1;campo2 verso2; ... "
            ' Il datagrid è ordinato per tutti i campi specificati. La freccia è impostata solo
            ' per quanto riguarda il primo.
            Dim s1 As String = paramSort.Split(";")(0) ' split della stringa per leggere tutti i campi

            If s1.Length > 0 Then

                ' Split del primo campo per avere il campo e il verso della freccia
                Dim s2() As String = s1.Split(" ")
                sort = s2(0).Trim()

                If s2.Length > 1 Then
                    order = s2(1).Trim()
                Else
                    order = "ASC"
                End If

            Else

                ' Se non è stato specificato nessun campo, prendo per default la data
                sort = "ves_data_effettuazione"
                order = "DESC"

            End If

            ' param_sort deve essere usata per ordinare il datagrid
            paramSort = paramSort.Replace(";", ",").Trim()

            If paramSort.EndsWith(",") Then paramSort = paramSort.Substring(0, paramSort.Length - 1)

        End If

        Return paramSort

    End Function

    Private Sub DataBindDatagrid()

        Dim dtReazioniAvverse As DataTable

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using biz As New BizVaccinazioniEseguite(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                Dim dtAssociazioni As DataTable = biz.CreaDtAssociazioni(dt_vacEseguite)

                dtReazioniAvverse = OnVacUtility.DataTableSelect(dtAssociazioni, "vra_data_reazione is not null")

            End Using
        End Using

        Dim dv As New DataView(dtReazioniAvverse)

        dg_Reaz.DataSource = dv
        dv.Sort = GetDataGridSort()
        dg_Reaz.DataBind()

        ' Visibilità colonne
        If IsGestioneCentrale Then

            SetColumnVisibility("BtnStampa", False)
            SetColumnVisibility("BtnElimina", False)
            SetColumnVisibility("usl_inserimento_vra_descr", True)
            SetColumnVisibility("FlagVisibilita", True)

        Else

            If Not ExistsRpt() Then SetColumnVisibility("BtnStampa", False)
            SetColumnVisibility("BtnElimina", Not StoricoVaccinaleCentralizzatoDaRecuperare)
            SetColumnVisibility("usl_inserimento_vra_descr", True)
            SetColumnVisibility("FlagVisibilita", True)

        End If

        ' --------- Datagrid interni --------- '

        ' Dettaglio
        Dim dg_dett As DataGrid
        Dim dv_dett As DataView

        ' Per ogni riga del datagrid faccio il bind dei 2 datagrid interni
        For i As Integer = 0 To dg_Reaz.Items.Count - 1

            ' Dettaglio
            dg_dett = DirectCast(dg_Reaz.Items(i).FindControl("dgrDettaglio"), DataGrid)
            dv_dett = New DataView(dt_vacEseguite)

            If dv(i)("ves_ass_codice") = String.Empty Then
                dv_dett.RowFilter = String.Format("ves_ass_codice is null and ves_ass_n_dose is null and ves_data_effettuazione = {0}",
                                                  FormatForDataView(dv(i)("ves_data_effettuazione")))
            Else
                dv_dett.RowFilter = String.Format("ves_ass_codice='{0}' and ves_ass_n_dose = '{1}' and ves_data_effettuazione = {2}",
                                                  dv(i)("ves_ass_codice").ToString(),
                                                  dv(i)("ves_ass_n_dose").ToString(),
                                                  FormatForDataView(dv(i)("ves_data_effettuazione")))
            End If

            dg_dett.DataSource = dv_dett
            dg_dett.DataBind()

        Next

    End Sub

    Private Function FormatForDataView(d As DateTime) As String

        Return String.Format(System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat, "#{0}#", d)

    End Function

    Private Function ExistsRpt() As Boolean

        Dim exists As Boolean = False

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                exists = bizReport.ExistsReport(Constants.ReportName.ReazioniAvverse)

            End Using
        End Using

        Return exists

    End Function

    Private Sub Salva()
		Dim vaccinazioniEseguiteResult As List(Of ReazioneAvversa)
		Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using biz As New Biz.BizVaccinazioniEseguite(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.REAZ_AVVERSE))

				vaccinazioniEseguiteResult = biz.SalvaReazioniAvverse(OnVacUtility.Variabili.PazId, dt_vacEseguite)

			End Using
        End Using

		dt_vacEseguite.AcceptChanges()
		' Inserisco segnalazione nel sistema d'integrazione vigifarmaco
		Dim messaggio As String = String.Empty

		If Settings.REAZIONE_AVVERSA_INTEGRAZIONE Then

			Dim listaTotaleRea As List(Of ReazioneAvversa) = vaccinazioniEseguiteResult.Where(Function(item) item.FlagInviato = "N").ToList()
			If listaTotaleRea.Count > 0 Then


				Using dbGenericProvider As New DbGenericProviderFactory
					Using biz As New BizIntegrazioneReazioniAvverse(dbGenericProvider, Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(TipiArgomento.VAC_ESEGUITE))
						messaggio = biz.InserisciReazioneAversaPu(OnVacUtility.Variabili.PazId, OnVacUtility.Variabili.CNS.Codice, listaTotaleRea)
					End Using
				End Using



			End If

		End If
		If Not messaggio.IsNullOrEmpty Then
			strJS &= String.Format("alert('{0}');", HttpUtility.JavaScriptStringEncode(messaggio)) & vbCrLf
		End If

		nMod = 0

        SetToolbarStatus(nMod > 0)

        DataBindDatagrid()

    End Sub

    Private Sub Annulla()

        dt_vacEseguite.RejectChanges()

        nMod = 0

        SetToolbarStatus(nMod > 0)

        DataBindDatagrid()

    End Sub

    Private Sub SetToolbarStatus(busy As Boolean)

        ToolBar.Items.FromKeyButton("btn_Salva").Enabled = busy And Not StoricoVaccinaleCentralizzatoDaRecuperare
        ToolBar.Items.FromKeyButton("btn_Annulla").Enabled = busy And Not StoricoVaccinaleCentralizzatoDaRecuperare
        ToolBar.Items.FromKeyButton("btnRecuperaStoricoVacc").Enabled = StoricoVaccinaleCentralizzatoDaRecuperare

        OnitLayout31.Busy = busy

    End Sub

    Private Function FindRow(rowIndex As Integer) As DataRow

        Dim item As DataGridItem = dg_Reaz.Items.Item(rowIndex)

        If IsGestioneCentrale Then

            RowKey = New Object() {DirectCast(item.FindControl("tb_ves_id"), HiddenField).Value,
                                   DirectCast(item.FindControl("hdUslInserimento"), HiddenField).Value}

        Else

            RowKey = New Object() {DirectCast(item.FindControl("tb_ves_id"), HiddenField).Value}

        End If

        Return dt_vacEseguite.Rows.Find(RowKey)

    End Function

    Private Sub LoadControlDataReazAvverse(selectedIndex As Integer)

        Dim rowReazioneSelezionata As DataRow = Me.FindRow(selectedIndex)

        Me.ReazAvverseDetail.LoadDataIntoControl(Me.ReazAvverseDetail.CreateListEseguiteReazioniSelezionate(Nothing, rowReazioneSelezionata, Me.IsGestioneCentrale))

    End Sub

#End Region

#Region " Dettaglio reazioni avverse "

    Private Sub SetToolbarDetailStatus(enabled As Boolean)

        Me.ToolBarDetail.Items.FromKeyButton("btn_Conferma").Enabled = enabled

    End Sub

    Private Sub Indietro()

        Me.mlvMaster.SetActiveView(pan_Reaz)
        Me.nMod -= 1

        SetToolbarStatus(Me.nMod > 0)

    End Sub

    Private Sub Conferma()

        ' N.B. : Il metodo GetListEseguiteReazioniAvverse effettua anche i controlli sui campi. 
        '        Se i controlli non vanno a buon fine, solleva l'evento con il messaggio di errore.
        Dim listVaccinazioniEseguite As List(Of Entities.VaccinazioneEseguita) = Me.ReazAvverseDetail.GetListEseguiteReazioniAvverse(True)

        If listVaccinazioniEseguite.IsNullOrEmpty() Then Return

        Dim eseguitaMaster As Entities.VaccinazioneEseguita = listVaccinazioniEseguite(0)

        For Each vaccinazioneEseguita As Entities.VaccinazioneEseguita In listVaccinazioniEseguite

            Dim rowKey As Object()

            If Me.IsGestioneCentrale Then
                rowKey = New Object() {vaccinazioneEseguita.ves_id, vaccinazioneEseguita.ves_usl_inserimento}
            Else
                rowKey = New Object() {vaccinazioneEseguita.ves_id}
            End If

            Dim row As DataRow = Me.dt_vacEseguite.Rows.Find(rowKey)

            ' Modifica dati reazione della riga relativa alla reazione
            Common.ReazioniAvverseCommon.UpdateDataRowByEntity(row, vaccinazioneEseguita.ReazioneAvversa)

            ' Modifica dei dati relativi alla reazione anche per le altre vaccinazioni della stessa associazione-dose
            InserisciReazioniAvverseAltreVacccinazioniStessaAssociazione(row)

        Next

        mlvMaster.SetActiveView(pan_Reaz)

        DataBindDatagrid()

    End Sub

    Private Sub InserisciReazioniAvverseAltreVacccinazioniStessaAssociazione(sourceRow As DataRow)

        For i As Int16 = 0 To Me.dt_vacEseguite.Rows.Count - 1

            ' Modifica i dati della reazione per le vaccinazioni presenti nel datatable in sessione 
            ' (aventi stessa associazione, dose, lotto e data di esecuzione di quella passata per parametro) 
            Dim currentRow As DataRow = Me.dt_vacEseguite.Rows(i)

            If Not currentRow Is sourceRow AndAlso currentRow.RowState <> DataRowState.Deleted Then

                If currentRow("ves_ass_n_dose").ToString() = sourceRow("ves_ass_n_dose").ToString() AndAlso
                   currentRow("ves_ass_codice").ToString() = sourceRow("ves_ass_codice").ToString() AndAlso
                   currentRow("ves_lot_codice").ToString() = sourceRow("ves_lot_codice").ToString() AndAlso
                   currentRow("ves_data_effettuazione").ToString() = sourceRow("ves_data_effettuazione").ToString() Then

                    Common.ReazioniAvverseCommon.CopyRowsReazioniAvverse(sourceRow, currentRow)

                End If

            End If

        Next

    End Sub

#End Region

    Private Function ShowAlertAggiornamentoStoricoCentralizzato() As Boolean

        ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK

        ' Controllo per visualizzare alert solo se la usl corrente ha dato visibilità
        If FlagConsensoVaccUslCorrente Then

            If Settings.ALERT_AGGIORNAMENTO_DATI_CENTRALIZZATI AndAlso ShowMessaggioAggiornamentoAnagrafeCentrale Then

                strJS &= Common.OnVacStoricoVaccinaleCentralizzato.AlertMessageAggiornamentoAnagrafeCentrale

                ShowMessaggioAggiornamentoAnagrafeCentrale = False

            End If

        End If

    End Function

    Private Sub SetColumnVisibility(sortExpression As String, visible As Boolean)

        Dim column As DataGridColumn =
            (From item As DataGridColumn In Me.dg_Reaz.Columns
             Where item.SortExpression = sortExpression
             Select item).FirstOrDefault()

        If Not column Is Nothing Then column.Visible = visible

    End Sub

    Private Sub InizializzazionePagina()

        Me.nMod = 0

        SetToolbarStatus(False)

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            OnVacUtility.ImpostaIntestazioniPagina(Me.OnitLayout31, Me.LayoutTitolo1, genericProvider, Me.Settings, Me.IsGestioneCentrale)

            Using biz As New Biz.BizVaccinazioniEseguite(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                Me.dt_vacEseguite = biz.GetVaccinazioniEseguite(OnVacUtility.Variabili.PazId, Me.IsGestioneCentrale)

                If Me.IsGestioneCentrale Then
                    Me.dt_vacEseguite.PrimaryKey = New DataColumn() {Me.dt_vacEseguite.Columns("ves_id"), Me.dt_vacEseguite.Columns("ves_usl_inserimento")}
                Else
                    Me.dt_vacEseguite.PrimaryKey = New DataColumn() {Me.dt_vacEseguite.Columns("ves_id")}
                End If

            End Using

        End Using

        DataBindDatagrid()

        Me.LayoutTitolo2.Text = Me.LayoutTitolo1.Text
        Me.mlvMaster.SetActiveView(Me.pan_Reaz)

    End Sub

    Private Sub RecuperaStoricoVaccinale()

        Dim command As New Common.OnVacStoricoVaccinaleCentralizzato.AcquisisciStoricoCommand()
        command.CodicePaziente = OnVacUtility.Variabili.PazId
        command.RichiediConfermaSovrascrittura = False
        command.Settings = Me.Settings
        command.OnitLayout3 = Me.OnitLayout31
        command.BizLogOptions = OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.REAZ_AVVERSE)
        command.Note = "Recupero Storico Vaccinale da maschera ReazAvverse"

        Dim acquisisciDatiVaccinaliCentraliResult As Biz.BizPaziente.AcquisisciDatiVaccinaliCentraliResult =
            Common.OnVacStoricoVaccinaleCentralizzato.AcquisisciDatiVaccinaliCentraliPaziente(command)

        If acquisisciDatiVaccinaliCentraliResult.StatoAcquisizioneDatiVaccinaliCentrale <> Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Parziale Then

            Me.StoricoVaccinaleCentralizzatoDaRecuperare = False

        End If

        InizializzazionePagina()

    End Sub

#End Region

#Region " Protected Methods "

    Protected Function BindFlagVisibilitaImageUrlValue(dataItem As Object) As String

        Return Common.OnVacStoricoVaccinaleCentralizzato.GetImageUrlFlagVisibilita(dataItem, "VES_FLAG_VISIBILITA", Me)

    End Function

    Protected Function BindFlagVisibilitaToolTipValue(dataItem As Object) As String

        Return Common.OnVacStoricoVaccinaleCentralizzato.GetToolTipFlagVisibilita(dataItem, "VES_FLAG_VISIBILITA")

    End Function

#End Region

#Region " Eventi User control Reazioni "

    Private Sub ReazAvverseDetail_ShowMessage(sender As Object, e As Common.ReazioniAvverseCommon.ShowMessageReazioniEventArgs) Handles ReazAvverseDetail.ShowMessage

        Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(HttpUtility.JavaScriptStringEncode(e.Message), "ALERT", False, False))

    End Sub

    Private Sub ReazAvverseDetail_RecuperaConcomitanti(sender As Object, ByRef farmaciRecuperabili As List(Of Common.ReazioniAvverseCommon.FarmacoRecupero)) Handles ReazAvverseDetail.RecuperaConcomitanti

        Dim dtAssociazioni As DataTable = Nothing

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using biz As New Biz.BizVaccinazioniEseguite(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                dtAssociazioni = biz.CreaDtAssociazioni(Me.dt_vacEseguite)

            End Using
        End Using

        farmaciRecuperabili = Common.ReazioniAvverseCommon.DataTableToListFarmacoRecupero(dtAssociazioni)

    End Sub

#End Region

End Class
