Imports System.Collections.Generic

Imports Onit.Shared.Manager.OnitProfile
Imports Onit.Database.DataAccessManager

Imports Onit.OnAssistnet.Web
Imports Onit.OnAssistnet.OnVac.Biz
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities


Partial Class AssistitiEmigrati
    Inherits OnVac.Common.OnVacMovimentiPageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub
    Protected WithEvents UscFiltriEtichette As uscFiltriStampaEtichetteMovAusl

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(sender As System.Object, e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Properties "

    Private Property FiltriRicerca() As MovimentiCNSEmigratiFilter
        Get
            Return Session("MovimentiCNSEmigratiFilter")
        End Get
        Set(value As MovimentiCNSEmigratiFilter)
            Session("MovimentiCNSEmigratiFilter") = value
        End Set
    End Property

    Protected Overrides ReadOnly Property OnitLayout As Controls.PagesLayout.OnitLayout3
        Get
            Return Me.OnitLayout31
        End Get
    End Property

#End Region

#Region " Overrides "

    Protected Overrides Sub OnInit(e As System.EventArgs)
        '--
        MyBase.OnInit(e)
        '--
        Me.tblNotifica.Visible = Me.Settings.SPOSTAMENTO_ASSISTITI_MOV_CNS
        '--
        Me.dgrPazienti.Columns(Me.dgrPazienti.Columns.Count - 1).Visible = Me.Settings.SPOSTAMENTO_ASSISTITI_MOV_CNS
        '--
    End Sub

#End Region

#Region " Events Handlers "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            Me.ShowPrintButtons()

            Me.SetFiltriRicerca()

        End If

        If Me.tlbMovimenti.Items.FromKeyButton("btnCertificato").Visible Then

            Select Case Request.Form("__EVENTTARGET")

                Case "ImpostaStatoS"
                    Me.StampaCertificato(True)

                Case "ImpostaStatoN"
                    Me.StampaCertificato(False)

            End Select

        End If

    End Sub

    Private Sub tlbMovimenti_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles tlbMovimenti.ButtonClicked

        Select Case be.Button.Key

            Case "btnCerca"
                Me.CaricaDati(0)

            Case "btnStampaElenco"
                Me.StampaElenco(Constants.ReportName.ElencoEmigrati)

            Case "btnStampaElencoPerComune"
                Me.StampaElenco(Constants.ReportName.ElencoEmigratiComune)

        End Select

    End Sub

    Private Sub btnStampaEtichette_Click(sender As Object, e As System.EventArgs) Handles btnStampaEtichette.Click

        Me.StampaEtichette()

    End Sub

#Region " Datagrid "

    Private Sub dgrPazienti_PageIndexChanged(source As Object, e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dgrPazienti.PageIndexChanged

        Me.CaricaDati(e.NewPageIndex)

    End Sub

    Private Sub dgrPazienti_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgrPazienti.ItemDataBound

        Select Case e.Item.ItemType

            Case ListItemType.Item, ListItemType.AlternatingItem, ListItemType.EditItem, ListItemType.SelectedItem

                ' Gestione colonna "Notifica"
                Dim statoNotificaEmigrazione As Enumerators.MovimentiCNS.StatoNotificaEmigrazione = Enumerators.MovimentiCNS.StatoNotificaEmigrazione.Nessuno

                If Not e.Item.DataItem("paz_stato_notifica_emi") Is DBNull.Value Then
                    statoNotificaEmigrazione = [Enum].Parse(GetType(Enumerators.MovimentiCNS.StatoNotificaEmigrazione), e.Item.DataItem("paz_stato_notifica_emi").ToString())
                End If

                DirectCast(e.Item.FindControl("btnNotifica"), ImageButton).Visible = False
                DirectCast(e.Item.FindControl("lblNotifica"), Label).Visible = False

                If Me.StatoPagina = StatoPaginaMovimenti.Modifica Then
                    Me.DataGridItemDataBound(e.Item, Me.dgrPazienti.EditItemIndex)
                Else
                    Me.AddConfirmClickToImageButton(e.Item, "btnCertEmi", "E' stato stampato il certificato di emigrazione?")
                End If

                ' Visibilità pulsante di edit della riga
                Me.SetEditButtonVisibility(e.Item)

        End Select

    End Sub

    Private Sub dgrPazienti_ItemCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgrPazienti.ItemCommand

        Select Case e.CommandName

            Case "EditRowMovimenti"

                Me.dgrPazientiEditRowMovimenti(e)

            Case "CancelRowMovimenti"

                Me.dgrPazientiCancelRowMovimenti(e)

            Case "UpdateRowMovimenti"

                Me.dgrPazientiUpdateRowMovimenti(e)

            Case "DatiPaziente"

                Me.RedirectToGestionePaziente(Me.dgrPazienti.Items(e.Item.ItemIndex).Cells(0).Text)

            Case "CertEmi"

                Me.CertificatoStampato(e.Item.ItemIndex)

                Me.CaricaDati(0)

                'Case "Notifica"

                '    Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

                '        Dim bizMovimentiEsterniCNS As New BizMovimentiEsterniCNS(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                '        Dim notificaMovimentoEsternoResult As BizMovimentiEsterniCNS.NotificaMovimentoEsternoResult =
                '            bizMovimentiEsterniCNS.NotificaMovimentoEsterno(dgrPazienti.Items(e.Item.ItemIndex).Cells(0).Text)

                '        Dim message As New System.Text.StringBuilder

                '        Select Case notificaMovimentoEsternoResult.Paziente.StatoNotificaEmigrazione
                '            Case Enumerators.MovimentiCNS.StatoNotificaEmigrazione.Errore
                '                If notificaMovimentoEsternoResult.DatabaseAcquisizioneIrraggiungibile Then
                '                    message.Append("Impossibile eseguire l'operazione !!!\nDatabase di acquisizione momentaneamente non disponibile.\nContattare il supporto o riprovare successivamente.")
                '                Else
                '                    message.Append("Errore !!!\nContattare il supporto o riprovare successivamente.")
                '                End If
                '            Case Enumerators.MovimentiCNS.StatoNotificaEmigrazione.Avvertimento
                '                If notificaMovimentoEsternoResult.PazienteEsistente Then
                '                    message.Append("Paziente già esistente !!!")
                '                Else
                '                    Throw New NotSupportedException
                '                End If
                '            Case Enumerators.MovimentiCNS.StatoNotificaEmigrazione.Notificato
                '                message.Append("Notifica avvenuta con successo !!!")
                '        End Select

                '        If notificaMovimentoEsternoResult.Paziente.StatoNotificaEmigrazione = Enumerators.MovimentiCNS.StatoNotificaEmigrazione.Notificato Then
                '            e.Item.FindControl("btnNotifica").Visible = False
                '            e.Item.FindControl("lblNotifica").Visible = True
                '        End If

                '        Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(message.ToString(), "notificaMovimentoEsternoResult", False, False))

                '    End Using

        End Select

    End Sub

    Private Sub dgrPazientiEditRowMovimenti(e As System.Web.UI.WebControls.DataGridCommandEventArgs)

        Me.EditMovimento(e.Item.Cells(0).Text)

        ' Datagrid in edit
        Me.dgrPazienti.EditItemIndex = e.Item.ItemIndex

        ' Riesegue la ricerca e il bind del datagrid
        Me.SetFiltriRicerca()
        Me.CaricaDati(Me.dgrPazienti.CurrentPageIndex)

    End Sub

    Private Sub dgrPazientiCancelRowMovimenti(e As System.Web.UI.WebControls.DataGridCommandEventArgs)

        Me.ReloadData(Me.dgrPazienti.CurrentPageIndex)

    End Sub

    Private Sub dgrPazientiUpdateRowMovimenti(e As System.Web.UI.WebControls.DataGridCommandEventArgs)

        Dim codiceStatoAnagraficoOriginale As String = String.Empty

        If e.Item.ItemType = ListItemType.EditItem Then
            codiceStatoAnagraficoOriginale = DirectCast(e.Item.FindControl("lblCodiceStatoAnagraficoEdit"), Label).Text
        End If

        Dim ddlStatoAnagrafico As DropDownList = DirectCast(e.Item.FindControl("ddlStatoAnagrafico"), DropDownList)

        If Me.UpdateStatoAnagrafico(ddlStatoAnagrafico.SelectedValue, codiceStatoAnagraficoOriginale) Then

            Me.ReloadData(Me.dgrPazienti.CurrentPageIndex)

        End If

    End Sub

#End Region

#End Region

#Region " Overrides Pagina Base Movimenti "

    Protected Overrides Sub ImpostaLayoutMovimenti(stato As Common.OnVacMovimentiPageBase.StatoPaginaMovimenti)

        MyBase.ImpostaLayoutMovimenti(stato)

        Dim abilita As Boolean = (stato = StatoPaginaMovimenti.Lettura)

        ' Toolbar
        Me.tlbMovimenti.Items.FromKeyButton("btnCerca").Enabled = abilita
        Me.tlbMovimenti.Items.FromKeyButton("btnStampaElenco").Enabled = abilita
        Me.tlbMovimenti.Items.FromKeyButton("btnStampaElencoPerComune").Enabled = abilita
        Me.tlbMovimenti.Items.FromKeyButton("btnStampaEtichette").Enabled = abilita
        Me.tlbMovimenti.Items.FromKeyButton("btnCertificato").Enabled = abilita
        Me.tlbMovimenti.Items.FromKeyButton("btnPulisci").Enabled = abilita

        ' Filtri di ricerca
        Me.odpDaNascita.Enabled = abilita
        Me.odpANascita.Enabled = abilita
        Me.dpkDaEmig.Enabled = abilita
        Me.dpkAEmig.Enabled = abilita
        Me.rdbCertStampato.Enabled = abilita
        Me.rdbCertNonStampato.Enabled = abilita
        Me.rdbCertIgnora.Enabled = abilita
        Me.rdbNotificati.Enabled = abilita
        Me.rdbDaNotificare.Enabled = abilita
        Me.rdbIgnoraNotificare.Enabled = abilita

    End Sub

    Protected Overrides Sub EliminaProgrammazioneEffettuata()

        Me.ReloadData(Me.dgrPazienti.CurrentPageIndex)

    End Sub

    Protected Overrides Sub EliminaProgrammazioneNonEffettuata()

        Me.ReloadData(Me.dgrPazienti.CurrentPageIndex)

    End Sub

#End Region

#Region " Private "

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()

        listPrintButtons.Add(New PrintButton(Constants.ReportName.ElencoEmigrati, "btnStampaElenco"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.ElencoEmigratiComune, "btnStampaElencoPerComune"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.EtichetteImmigrati, "btnStampaEtichette"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.CertificatoVaccinale, "btnCertificato"))

        Me.ShowToolbarPrintButtons(listPrintButtons, tlbMovimenti)

    End Sub

    Private Sub CaricaDati(currentPageIndex As Int32)

        Dim dstMovimentiEsterni As DstMovimentiEsterni = Nothing
        Dim countEmigrati As Int32 = 0

        Dim pagingOptions As New MovimentiCNSPagingOptions()
        pagingOptions.StartRecordIndex = currentPageIndex * dgrPazienti.PageSize
        pagingOptions.EndRecordIndex = pagingOptions.StartRecordIndex + dgrPazienti.PageSize

        FiltriRicerca = GetFiltriRicerca()

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            dstMovimentiEsterni = genericProvider.MovimentiEsterniCNS.LoadEmigrati(FiltriRicerca, pagingOptions, OnVacContext.CodiceUslCorrente)

            countEmigrati = genericProvider.MovimentiEsterniCNS.CountEmigrati(FiltriRicerca, OnVacContext.CodiceUslCorrente)

        End Using

        dgrPazienti.VirtualItemCount = countEmigrati

        dgrPazienti.CurrentPageIndex = currentPageIndex

        ' Possibilità di cambio pagina solo se non in modifica
        dgrPazienti.PagerStyle.Visible = Not IsPageInEdit()

        dgrPazienti.DataSource = dstMovimentiEsterni.MovimentiEsterni
        dgrPazienti.DataBind()

        dgrPazienti.SelectedIndex = -1

        divSezioneMovimenti.InnerText = String.Format(" MOVIMENTI: {0} risultat{1}.", dgrPazienti.VirtualItemCount, IIf(dgrPazienti.VirtualItemCount = 1, "o", "i"))

    End Sub

    Private Sub ReloadData(currentPageIndex As Int32)

        Me.StatoPagina = StatoPaginaMovimenti.Lettura

        ' Datagrid in sola lettura
        Me.dgrPazienti.EditItemIndex = -1

        ' Riesegue la ricerca e il bind del datagrid
        Me.SetFiltriRicerca()
        Me.CaricaDati(currentPageIndex)

    End Sub

    Private Function GetFiltriRicerca() As MovimentiCNSEmigratiFilter

        Dim filter As New MovimentiCNSEmigratiFilter()

        filter.CodiceConsultorio = OnVacUtility.Variabili.CNS.Codice

        If Not String.IsNullOrEmpty(Me.odpDaNascita.Text) Then filter.DataNascitaInizio = Me.odpDaNascita.Data
        If Not String.IsNullOrEmpty(Me.odpANascita.Text) Then filter.DataNascitaFine = Me.odpANascita.Data
        If Not String.IsNullOrEmpty(Me.dpkDaEmig.Text) Then filter.DataEmigrazioneInizio = Me.dpkDaEmig.Data
        If Not String.IsNullOrEmpty(Me.dpkAEmig.Text) Then filter.DataEmigrazioneFine = Me.dpkAEmig.Data

        If Me.rdbCertStampato.Checked Then
            filter.CertificatoRichiesto = True
        ElseIf Me.rdbCertNonStampato.Checked Then
            filter.CertificatoRichiesto = False
        End If

        Dim statiNotificaImmigrazioneFilter As New List(Of Enumerators.MovimentiCNS.StatoNotificaEmigrazione)()

        If rdbNotificati.Checked Then
            statiNotificaImmigrazioneFilter.Add(Enumerators.MovimentiCNS.StatoNotificaEmigrazione.Notificato)
        ElseIf rdbDaNotificare.Checked Then
            statiNotificaImmigrazioneFilter.Add(Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Errore)
            statiNotificaImmigrazioneFilter.Add(Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Nessuno)
            statiNotificaImmigrazioneFilter.Add(Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Avvertimento)
        End If

        filter.StatiNotificaEmigrazione = statiNotificaImmigrazioneFilter.ToArray()

        Return filter

    End Function

    Private Sub SetFiltriRicerca()

        If Not Me.FiltriRicerca Is Nothing Then

            If Not Me.FiltriRicerca.DataNascitaInizio Is Nothing Then
                Me.odpDaNascita.Data = Me.FiltriRicerca.DataNascitaInizio
            End If

            If Not Me.FiltriRicerca.DataNascitaFine Is Nothing Then
                Me.odpANascita.Data = Me.FiltriRicerca.DataNascitaFine
            End If

            If Not Me.FiltriRicerca.DataEmigrazioneInizio Is Nothing Then
                Me.dpkDaEmig.Data = Me.FiltriRicerca.DataEmigrazioneInizio
            End If

            If Not Me.FiltriRicerca.DataEmigrazioneFine Is Nothing Then
                Me.dpkAEmig.Data = Me.FiltriRicerca.DataEmigrazioneFine
            End If

            Dim certStampato As Boolean = False
            Dim certNonStampato As Boolean = False
            Dim certIgnora As Boolean = False

            If Not Me.FiltriRicerca.CertificatoRichiesto Is Nothing Then
                If Me.FiltriRicerca.CertificatoRichiesto Then
                    certStampato = True
                Else
                    certNonStampato = True
                End If
            Else
                certIgnora = True
            End If

            Me.rdbCertStampato.Checked = certStampato
            Me.rdbCertNonStampato.Checked = certNonStampato
            Me.rdbCertIgnora.Checked = certIgnora

            Dim daNotificare As Boolean = False
            Dim notificati As Boolean = False
            Dim ignoraNotificare As Boolean = False

            If Not Me.FiltriRicerca.StatiNotificaEmigrazione Is Nothing AndAlso Me.FiltriRicerca.StatiNotificaEmigrazione.Length > 0 Then
                If Me.FiltriRicerca.StatiNotificaEmigrazione.Contains(Enumerators.MovimentiCNS.StatoNotificaEmigrazione.Errore) Or Me.FiltriRicerca.StatiNotificaEmigrazione.Contains(Enumerators.MovimentiCNS.StatoNotificaEmigrazione.Nessuno) Or Me.FiltriRicerca.StatiNotificaEmigrazione.Contains(Enumerators.MovimentiCNS.StatoNotificaEmigrazione.Avvertimento) Then
                    daNotificare = True
                ElseIf Me.FiltriRicerca.StatiNotificaEmigrazione.Contains(Enumerators.MovimentiCNS.StatoNotificaEmigrazione.Notificato) Then
                    notificati = True
                End If
            Else
                ignoraNotificare = True
            End If

            Me.rdbDaNotificare.Checked = daNotificare
            Me.rdbNotificati.Checked = notificati
            Me.rdbIgnoraNotificare.Checked = ignoraNotificare

            'Else
            '    odpDaNascita.Text = String.Empty
            '    odpANascita.Text = String.Empty
            '    dpkDaEmig.Text = String.Empty
            '    dpkAEmig.Text = String.Empty
            '    rdbCertIgnora.Checked = True
            '    rdbIgnoraNotificare.Checked = True
        End If

    End Sub

    Private Sub CertificatoStampato(idx As Integer)

        Dim DAM As IDAM = OnVacUtility.OpenDam()

        With DAM.QB
            .NewQuery()
            .AddUpdateField("paz_sta_certificato_emi", "S", DataTypes.Stringa)
            .AddTables("t_paz_pazienti")
            .AddWhereCondition("paz_codice", Comparatori.Uguale, Me.dgrPazienti.Items(idx).Cells(0).Text, DataTypes.Numero)
        End With

        Try
            DAM.BeginTrans()
            DAM.ExecNonQuery(ExecQueryType.Update)
            DAM.Commit()
        Catch exc As Exception
            DAM.Rollback()
            exc.InternalPreserveStackTrace()
            Throw
        Finally
            OnVacUtility.CloseDam(DAM)
        End Try

    End Sub

    Private Sub AddCommonsReportParameters(rpt As ReportParameter)

        Dim installazione As Installazione = OnVacUtility.GetDatiInstallazioneCorrente(Settings)

        If installazione Is Nothing Then
            rpt.AddParameter("UslCitta", String.Empty)
            rpt.AddParameter("UslDesc", String.Empty)
            rpt.AddParameter("UslReg", String.Empty)
        Else
            rpt.AddParameter("UslCitta", installazione.UslCitta)
            rpt.AddParameter("UslDesc", installazione.UslDescrizionePerReport)
            rpt.AddParameter("UslReg", installazione.UslRegione)
        End If

        rpt.AddParameter("Ambulatorio", OnVacUtility.Variabili.CNS.Descrizione + "(" + OnVacUtility.Variabili.CNS.Codice + ")")

        If Me.odpDaNascita.Text = "" Then
            rpt.AddParameter("Da_data_nascita", "")
        Else
            rpt.AddParameter("Da_data_nascita", String.Format("{0:dd/MM/yyyy}", Me.odpDaNascita.Data))
        End If

        If Me.odpANascita.Text = "" Then
            rpt.AddParameter("A_data_nascita", "")
        Else
            rpt.AddParameter("A_data_nascita", String.Format("{0:dd/MM/yyyy}", Me.odpANascita.Data))
        End If

        If Me.dpkDaEmig.Text = "" Then
            rpt.AddParameter("Da_data_emig", "")
        Else
            rpt.AddParameter("Da_data_emig", String.Format("{0:dd/MM/yyyy}", Me.dpkDaEmig.Data))
        End If

        If Me.dpkAEmig.Text = "" Then
            rpt.AddParameter("A_data_emig", "")
        Else
            rpt.AddParameter("A_data_emig", String.Format("{0:dd/MM/yyyy}", Me.dpkAEmig.Data))
        End If

        rpt.AddParameter("certificato", IIf(Me.rdbCertStampato.Checked, "0", IIf(Me.rdbCertNonStampato.Checked, "1", "2")))

    End Sub

#Region " Stampe "

    Private Sub StampaElenco(nomeReport As String)

        Me.FiltriRicerca = Me.GetFiltriRicerca()

        Dim rpt As New ReportParameter()

        Dim dst As DstMovimentiEsterni = Nothing
        Dim reportFolder As String = String.Empty

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            dst = genericProvider.MovimentiEsterniCNS.LoadEmigrati(FiltriRicerca, Nothing, OnVacContext.CodiceUslCorrente)

            Using bizReport As New BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                reportFolder = bizReport.GetReportFolder(nomeReport)

            End Using
        End Using

        rpt.set_dataset(dst)

        AddCommonsReportParameters(rpt)

        ' Stampa
        If Not OnVacReport.StampaReport(Page.Request.Path, nomeReport, String.Empty, rpt, Nothing, Nothing, reportFolder) Then
            OnVacUtility.StampaNonPresente(Page, nomeReport)
        End If

    End Sub

    Private Sub StampaCertificato(impostaStatoCertificatoStampato As Boolean)

        Me.FiltriRicerca = Me.GetFiltriRicerca()

        Dim strFiltro As String = String.Empty

        strFiltro &= " {T_PAZ_PAZIENTI.PAZ_CNS_CODICE}= '" & OnVacUtility.Variabili.CNS.Codice & "'"
        strFiltro &= " AND (NOT(ISNULL({T_PAZ_PAZIENTI.PAZ_COM_COMUNE_EMIGRAZIONE})) OR NOT(ISNULL({T_PAZ_PAZIENTI.PAZ_DATA_EMIGRAZIONE})))"

        If Me.odpDaNascita.Text <> "" Then
            strFiltro &= " AND {T_PAZ_PAZIENTI.PAZ_DATA_NASCITA} >= DateTime(" & Me.odpDaNascita.Data.Year.ToString() & "," & IIf(Me.odpDaNascita.Data.Month.ToString().Length > 1, Me.odpDaNascita.Data.Month.ToString(), "0" & Me.odpDaNascita.Data.Month.ToString()) & "," & IIf(Me.odpDaNascita.Data.Day.ToString().Length > 1, Me.odpDaNascita.Data.Day.ToString(), "0" & Me.odpDaNascita.Data.Day.ToString()) & ")"
        End If

        If Me.odpANascita.Text <> "" Then
            strFiltro &= " AND {T_PAZ_PAZIENTI.PAZ_DATA_NASCITA} <= DateTime(" & Me.odpANascita.Data.Year.ToString() & "," & IIf(Me.odpANascita.Data.Month.ToString().Length > 1, Me.odpANascita.Data.Month.ToString(), "0" & Me.odpANascita.Data.Month.ToString()) & "," & IIf(Me.odpANascita.Data.Day.ToString().Length > 1, Me.odpANascita.Data.Day.ToString(), "0" & Me.odpANascita.Data.Day.ToString()) & ")"
        End If

        If Me.dpkDaEmig.Text <> "" And Me.dpkAEmig.Text <> "" Then
            strFiltro &= " AND (ISNULL({T_PAZ_PAZIENTI.PAZ_DATA_EMIGRAZIONE}) OR ({T_PAZ_PAZIENTI.PAZ_DATA_EMIGRAZIONE} >= DateTime(" & Me.dpkDaEmig.Data.Year.ToString() & "," & IIf(Me.dpkDaEmig.Data.Month.ToString().Length > 1, Me.dpkDaEmig.Data.Month.ToString(), "0" & Me.dpkDaEmig.Data.Month.ToString()) & "," & IIf(Me.dpkDaEmig.Data.Day.ToString().Length > 1, Me.dpkDaEmig.Data.Day.ToString(), "0" & Me.dpkDaEmig.Data.Day.ToString()) & ")) AND ({T_PAZ_PAZIENTI.PAZ_DATA_EMIGRAZIONE} <= DateTime(" & Me.dpkAEmig.Data.Year.ToString() & "," & IIf(Me.dpkAEmig.Data.Month.ToString().Length > 1, Me.dpkAEmig.Data.Month.ToString(), "0" & Me.dpkAEmig.Data.Month.ToString()) & "," & IIf(Me.dpkAEmig.Data.Day.ToString().Length > 1, Me.dpkAEmig.Data.Day.ToString(), "0" & Me.dpkAEmig.Data.Day.ToString()) & ")))"
        End If

        If Me.dpkDaEmig.Text <> "" And Me.dpkAEmig.Text = "" Then
            strFiltro &= " AND (ISNULL({T_PAZ_PAZIENTI.PAZ_DATA_EMIGRAZIONE}) OR ({T_PAZ_PAZIENTI.PAZ_DATA_EMIGRAZIONE} >= DateTime(" & Me.dpkDaEmig.Data.Year.ToString() & "," & IIf(Me.dpkDaEmig.Data.Month.ToString().Length > 1, Me.dpkDaEmig.Data.Month.ToString(), "0" & Me.dpkDaEmig.Data.Month.ToString()) & "," & IIf(Me.dpkDaEmig.Data.Day.ToString().Length > 1, Me.dpkDaEmig.Data.Day.ToString(), "0" & Me.dpkDaEmig.Data.Day.ToString()) & ")))"
        End If

        If Me.dpkDaEmig.Text = "" And Me.dpkAEmig.Text <> "" Then
            strFiltro &= " AND (ISNULL({T_PAZ_PAZIENTI.PAZ_DATA_EMIGRAZIONE}) OR ({T_PAZ_PAZIENTI.PAZ_DATA_EMIGRAZIONE} <= DateTime(" & Me.dpkAEmig.Data.Year.ToString() & "," & IIf(Me.dpkAEmig.Data.Month.ToString().Length > 1, Me.dpkAEmig.Data.Month.ToString(), "0" & Me.dpkAEmig.Data.Month.ToString()) & "," & IIf(Me.dpkAEmig.Data.Day.ToString().Length > 1, Me.dpkAEmig.Data.Day.ToString(), "0" & Me.dpkAEmig.Data.Day.ToString()) & ")))"
        End If

        If Me.rdbCertStampato.Checked Then
            strFiltro &= " AND {T_PAZ_PAZIENTI.PAZ_STA_CERTIFICATO_EMI} = 'S'"
        ElseIf Me.rdbCertNonStampato.Checked Then
            strFiltro &= " AND (ISNULL({T_PAZ_PAZIENTI.PAZ_STA_CERTIFICATO_EMI}) OR ({T_PAZ_PAZIENTI.PAZ_STA_CERTIFICATO_EMI} = 'N'))"
        End If

        OnVacUtility.StampaCertificatoVaccinale(Me.Page, Me.Page.Request.Path, Me.Settings, strFiltro, True)

        '---CMR 22/03/07---'
        'se vengono stampati dei certificati vaccinali che ancora non erano stati stampati bisogna aggiornare 
        'il campo paz_sta_certificato_emi col valore "S" che indica che il certificato per quel paziente è già stato stampato
        If impostaStatoCertificatoStampato AndAlso Not Me.rdbCertStampato.Checked Then

            Using dam As IDAM = OnVacUtility.OpenDam()

                Try
                    dam.BeginTrans()

                    With dam.QB

                        .NewQuery()
                        .AddUpdateField("paz_sta_certificato_emi", "S", DataTypes.Stringa)
                        .AddTables("t_paz_pazienti")

                        .AddWhereCondition("paz_cns_codice", Comparatori.Uguale, OnVacUtility.Variabili.CNS.Codice, DataTypes.Stringa)
                        .OpenParanthesis()
                        .AddWhereCondition("paz_com_comune_emigrazione", Comparatori.[IsNot], "NULL", DataTypes.Replace)
                        .AddWhereCondition("paz_data_emigrazione", Comparatori.[IsNot], "NULL", DataTypes.Replace, "OR")
                        .CloseParanthesis()

                        If Me.odpDaNascita.Text <> "" Then
                            .AddWhereCondition("paz_data_nascita", Comparatori.MaggioreUguale, Me.odpDaNascita.Data, DataTypes.Data)
                        End If

                        If Me.odpANascita.Text <> "" Then
                            .AddWhereCondition("paz_data_nascita", Comparatori.MinoreUguale, Me.odpANascita.Data, DataTypes.Data)
                        End If

                        If Me.dpkDaEmig.Text <> "" Or Me.dpkAEmig.Text <> "" Then
                            .OpenParanthesis()
                            .OpenParanthesis()
                            If Me.dpkDaEmig.Text <> "" Then
                                .AddWhereCondition("paz_data_emigrazione", Comparatori.MaggioreUguale, Me.dpkDaEmig.Data, DataTypes.Data)
                            End If
                            If Me.dpkAEmig.Text <> "" Then
                                .AddWhereCondition("paz_data_emigrazione", Comparatori.MinoreUguale, Me.dpkAEmig.Data, DataTypes.Data)
                            End If
                            .CloseParanthesis()
                            .AddWhereCondition("paz_data_emigrazione", Comparatori.Is, "NULL", DataTypes.Replace, "OR")
                            .CloseParanthesis()
                        End If
                        '
                        .OpenParanthesis()
                        .AddWhereCondition("paz_sta_certificato_emi", Comparatori.Uguale, "N", DataTypes.Stringa)
                        .AddWhereCondition("paz_sta_certificato_emi", Comparatori.Is, "NULL", DataTypes.Replace, "OR")
                        .CloseParanthesis()
                    End With

                    dam.ExecNonQuery(ExecQueryType.Update)

                    dam.Commit()

                Catch exc As Exception

                    dam.Rollback()

                    exc.InternalPreserveStackTrace()
                    Throw

                End Try

            End Using

        End If

    End Sub

    Private Sub StampaEtichette()

        Dim dst As DstMovimentiEsterni = Nothing

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            dst = genericProvider.MovimentiEsterniCNS.LoadEmigrati(GetFiltriRicerca(), Nothing, OnVacContext.CodiceUslCorrente)

        End Using

        Dim errMsg As String = String.Empty

        If Not UscFiltriEtichette.StampaEtichette(dst, errMsg) Then

            ' Stampa non effettuata, mostro il messaggio di errore
            ClientScript.RegisterClientScriptBlock(GetType(String), "msg_stampa", String.Format("<script language='javascript'>alert('{0}');</script>", errMsg))

        End If

    End Sub

#End Region

#End Region

End Class
