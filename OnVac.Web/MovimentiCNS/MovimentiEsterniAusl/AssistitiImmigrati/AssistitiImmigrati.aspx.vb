Imports System.Collections.Generic

Imports Onit.Shared.Manager.OnitProfile
Imports Onit.Database.DataAccessManager

Imports Onit.OnAssistnet.OnVac.DAL


Partial Class AssistitiImmigrati
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

    Private Property FiltriRicerca() As MovimentiCNSImmigratiFilter
        Get
            Return Session("MovimentiCNSImmigratiFilter")
        End Get
        Set(value As MovimentiCNSImmigratiFilter)
            Session("MovimentiCNSImmigratiFilter") = value
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
        Me.tblAcquisizione.Visible = Me.Settings.SPOSTAMENTO_ASSISTITI_MOV_CNS
        '--
        Me.dgrPazienti.Columns(Me.dgrPazienti.Columns.Count - 3).Visible = Me.Settings.SPOSTAMENTO_ASSISTITI_MOV_CNS
        '--
    End Sub

#End Region

#Region " Events Handlers "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            Me.ShowPrintButtons()

            Me.SetFiltriRicerca()

        End If

    End Sub

    Private Sub tlbMovimenti_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles tlbMovimenti.ButtonClicked

        Select Case be.Button.Key

            Case "btnCerca"
                Me.CaricaDati(0)

            Case "btnStampaElenco"
                Me.StampaElenco(Constants.ReportName.ElencoImmigrati)

            Case "btnStampaElencoPerComune"
                Me.StampaElenco(Constants.ReportName.ElencoImmigratiComune)

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
        '--
        Select Case e.Item.ItemType
            '--
            Case ListItemType.Item, ListItemType.AlternatingItem, ListItemType.EditItem, ListItemType.SelectedItem
                '--
                Dim statoAcquisizioneImmigrazione As Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione = Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Nessuno
                '--
                If Not e.Item.DataItem("paz_stato_acquisizione_imi") Is DBNull.Value Then
                    statoAcquisizioneImmigrazione = [Enum].Parse(GetType(Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione), e.Item.DataItem("paz_stato_acquisizione_imi").ToString())
                End If
                '--
                Me.SetAcquisizioneCommand(e.Item.FindControl("lblAcquisizione"), e.Item.FindControl("btnAcquisizione"), e.Item.FindControl("imgAcquisizione"), Not e.Item.DataItem("ugs_app_id_imi") Is DBNull.Value, statoAcquisizioneImmigrazione)
                '--
                If Me.StatoPagina = StatoPaginaMovimenti.Modifica Then
                    '--
                    Me.DataGridItemDataBound(e.Item, dgrPazienti.EditItemIndex)
                    '--
                Else
                    '--
                    Me.AddConfirmClickToImageButton(e.Item, "btnRegPaz", "Il paziente e' stato regolarizzato?")
                    Me.AddConfirmClickToImageButton(e.Item, "btnRichiestaCertificato", "E' stato richiesto il certificato?")
                    '--
                End If
                '--
                Me.SetEditButtonVisibility(e.Item)
                '--
        End Select
        '--
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

            Case "RegPaz"

                Me.RegolarizzaPaziente(e.Item.ItemIndex)

                Me.CaricaDati(0)

            Case "RichiestaCertificato"

                Me.RichiediCertificato(e.Item.ItemIndex)

                Me.CaricaDati(0)

            Case "Acquisisci"

                Me.Acquisisci(e)

        End Select

    End Sub

    <MovimentiCV()>
    Private Sub Acquisisci(e As System.Web.UI.WebControls.DataGridCommandEventArgs)

        Me.CodicePazienteSelezionato = Me.dgrPazienti.Items(e.Item.ItemIndex).Cells(0).Text

        Using transactionScope As New System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, OnVacUtility.GetReadCommittedTransactionOptions())

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizPaziente As Biz.BizPaziente = Biz.BizFactory.Instance.CreateBizPaziente(genericProvider, OnVacContext.CreateBizContextInfos(), Nothing)

                    Dim paziente As Entities.Paziente = bizPaziente.CercaPaziente(Me.CodicePazienteSelezionato)
                    Using bizMovimentiEsterniCNS As New Biz.BizMovimentiEsterniCNS(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                        Dim acquisizioneMovimentoEsternoResult As Biz.BizMovimentiEsterniCNS.AcquisizioneMovimentoEsternoResult = bizMovimentiEsterniCNS.AcquisisciMovimentoEsterno(paziente, False)

                        Dim onitLayoutMsgBox As New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(String.Empty, "acquisizioneMovimentoEsternoResult", False, False)

                        Dim message As New System.Text.StringBuilder()

                        If Not acquisizioneMovimentoEsternoResult.DatiPresenti Then

                            Select Case acquisizioneMovimentoEsternoResult.Paziente.StatoAcquisizioneImmigrazione

                                Case Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Nessuno

                                    message.Append("Impossibile eseguire l'operazione !!!\nPaziente già presente nel database di acquisizione.")

                                Case Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Errore

                                    If acquisizioneMovimentoEsternoResult.PazienteInesistente Then
                                        message.Append("Impossibile eseguire l'operazione !!!\nPaziente non presente nel database di notifica.")
                                    ElseIf acquisizioneMovimentoEsternoResult.PazienteInesistente Then
                                        message.Append("Impossibile eseguire l'operazione !!!\nDatabase di notifica momentaneamente non disponibile o inesistente.\nContattare il supporto o riprovare in un secondo momento.")
                                    ElseIf acquisizioneMovimentoEsternoResult.VaccinazioneEseguiteScaduteSovrapposte Then
                                        message.Append("Impossibile eseguire l'operazione !!!\nE' presente la stessa vaccinazione nella azienda di acquisizione e di notifica, ma con stato validità (scadenza) in contrasto.")
                                    Else
                                        message.Append("Errore !!!\nContattare il supporto o riprovare in un secondo momento.")
                                    End If

                                Case Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Acquisito

                                    transactionScope.Complete()

                                    message.Append("Acquisizione avvenuta con successo !!!")

                                    e.Item.FindControl("btnAcquisizione").Visible = False
                                    e.Item.FindControl("lblAcquisizione").Visible = True

                                Case Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Avvertimento

                                    Throw New NotSupportedException()

                            End Select

                        Else

                            message.Append("ATTENZIONE\n\nDurante l\'acquisizione sono stati riscontrati i seguenti problemi:\n\n")

                            'Me.AddAcquisizionePartMessage(message, "NOTE ANAGRAFICHE", acquisizioneMovimentoEsternoResult.NoteAnagrafichePresenti)
                            'Me.AddAcquisizionePartMessage(message, "MALATTIE", acquisizioneMovimentoEsternoResult.MalattiePresenti)
                            'Me.AddAcquisizionePartMessage(message, "CATEGORIE RISCHIO", acquisizioneMovimentoEsternoResult.CategorieRischioPresenti)
                            'Me.AddAcquisizionePartMessage(message, "CICLI VACCINALI", acquisizioneMovimentoEsternoResult.CicliVaccinaliPresenti)
                            Me.AddAcquisizionePartMessage(message, "VACCINAZIONI ESEGUITE", acquisizioneMovimentoEsternoResult.VaccinazioneEseguitePresenti)
                            Me.AddAcquisizionePartMessage(message, "REAZIONI AVVERSE", acquisizioneMovimentoEsternoResult.ReazioneAvversePresenti)
                            Me.AddAcquisizionePartMessage(message, "VACCINAZIONI SCADUTE", acquisizioneMovimentoEsternoResult.VaccinazioneScadutePresenti)
                            Me.AddAcquisizionePartMessage(message, "REAZIONI SCADUTE", acquisizioneMovimentoEsternoResult.ReazioneScadutePresenti)
                            Me.AddAcquisizionePartMessage(message, "VISITE", acquisizioneMovimentoEsternoResult.VisitePresenti)
                            Me.AddAcquisizionePartMessage(message, "ESCLUSIONI", acquisizioneMovimentoEsternoResult.EsclusioniPresenti)
                            'Me.AddAcquisizionePartMessage(message, "RIFIUTI", acquisizioneMovimentoEsternoResult.RifiutiPresenti)
                            'Me.AddAcquisizionePartMessage(message, "INADEMPIENZE", acquisizioneMovimentoEsternoResult.InadempienzePresenti)
                            'Me.AddAcquisizionePartMessage(message, "PROGRAMMAZIONE", acquisizioneMovimentoEsternoResult.ProgrammazionePresente)

                            message.Append("\nProcedere con l\'acquisizione sovrascrivendo i dati in locale ?")

                            onitLayoutMsgBox.askConfirm = True
                            onitLayoutMsgBox.autoPostBack = True

                        End If

                        onitLayoutMsgBox.text = message.ToString()

                        Me.OnitLayout31.ShowMsgBox(onitLayoutMsgBox)


                    End Using
                End Using
            End Using

        End Using

    End Sub

    Private Sub OnitLayout31_ConfirmClick(source As Object, e As Onit.Controls.PagesLayout.OnitLayout3.ConfirmEventArgs) Handles OnitLayout31.ConfirmClick

        Select Case e.Key

            Case "acquisizioneMovimentoEsternoResult"

                If e.Result Then

                    Using transactionScope As New System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, OnVacUtility.GetReadCommittedTransactionOptions())

                        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                            Using bizPaziente As Biz.BizPaziente = Biz.BizFactory.Instance.CreateBizPaziente(genericProvider, OnVacContext.CreateBizContextInfos(), Nothing)

                                Dim paziente As Entities.Paziente = bizPaziente.CercaPaziente(Me.CodicePazienteSelezionato)
                                Using bizMovimentiEsterniCNS As New Biz.BizMovimentiEsterniCNS(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                                    Dim acquisizioneMovimentoEsternoResult As Biz.BizMovimentiEsterniCNS.AcquisizioneMovimentoEsternoResult =
                                        bizMovimentiEsterniCNS.AcquisisciMovimentoEsterno(paziente, True)

                                    Dim message As New System.Text.StringBuilder()

                                    Select Case acquisizioneMovimentoEsternoResult.Paziente.StatoAcquisizioneImmigrazione

                                        Case Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Errore
                                            '--
                                            If acquisizioneMovimentoEsternoResult.PazienteInesistente Then
                                                message.Append("Impossibile eseguire l'operazione !!!\nPaziente non presente nel database di notifica.")
                                            ElseIf acquisizioneMovimentoEsternoResult.PazienteInesistente Then
                                                message.Append("Impossibile eseguire l'operazione !!!\nDatabase di notifica momentaneamente non disponibile o inesistente.\nContattare il supporto o riprovare in un secondo momento.")
                                            ElseIf acquisizioneMovimentoEsternoResult.VaccinazioneEseguiteScaduteSovrapposte Then
                                                message.Append("Impossibile eseguire l'operazione !!!\nE' presente la stessa vaccinazione nella azienda di acquisizione e di notifica, ma con stato validità (scadenza) in contrasto.")
                                            Else
                                                message.Append("Errore !!!\nContattare il supporto o riprovare in un secondo momento.")
                                            End If
                                            '--
                                        Case Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Avvertimento
                                            '--
                                            transactionScope.Complete()
                                            '--
                                            message.Append("ATTENZIONE\n\nNon è stato possibile acquisire i seguenti dati:\n")
                                            '--
                                            If Not acquisizioneMovimentoEsternoResult.VaccinazioniReazioniEseguiteAcquisite Then
                                                message.Append("\nVaccinazioni eseguite e Reazioni avverse")
                                            End If
                                            '--
                                            If Not acquisizioneMovimentoEsternoResult.VaccinazioniReazioniScaduteAcquisite Then
                                                message.Append("\nVaccinazioni e Reazioni scadute")
                                            End If
                                            '--
                                            'If Not acquisizioneMovimentoEsternoResult.ProgrammazioneEliminata Then
                                            '    message.Append("\nProgrammazione vaccinale non eliminata")
                                            'End If
                                            '--
                                            message.Append("\n\nProcedere con l'inserimento manuale.")
                                            '--
                                        Case Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Acquisito
                                            '--
                                            transactionScope.Complete()
                                            message.Append("Acquisizione avvenuta con successo !!!")
                                            '--
                                    End Select
                                    '--
                                    For Each dgrItemPaziente As DataGridItem In Me.dgrPazienti.Items
                                        '--
                                        If dgrItemPaziente.Cells(0).Text = Me.CodicePazienteSelezionato Then
                                            Me.SetAcquisizioneCommand(dgrItemPaziente.FindControl("lblAcquisizione"), dgrItemPaziente.FindControl("btnAcquisizione"), dgrItemPaziente.FindControl("imgAcquisizione"), True, acquisizioneMovimentoEsternoResult.Paziente.StatoAcquisizioneImmigrazione)
                                            Exit For
                                        End If
                                        '--
                                    Next
                                    '--
                                    Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(message.ToString(), "acquisizioneMovimentoEsternoResult", False, False))
                                    '--
                                End Using
                            End Using
                        End Using

                    End Using

                End If

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
        Me.tlbMovimenti.Items.FromKeyButton("btnPulisci").Enabled = abilita

        ' Filtri di ricerca
        Me.odpDaNascita.Enabled = abilita
        Me.odpANascita.Enabled = abilita
        Me.dpkDaImm.Enabled = abilita
        Me.dpkAImm.Enabled = abilita
        Me.rdbPazReg.Enabled = abilita
        Me.rdbPazNoReg.Enabled = abilita
        Me.rdbIgnoraPazReg.Enabled = abilita
        Me.rdbCertRich.Enabled = abilita
        Me.rdbCertNonRich.Enabled = abilita
        Me.rdbCertIgnora.Enabled = abilita
        Me.rdbAcquisiti.Enabled = abilita
        Me.rdbAcquisitiErrore.Enabled = abilita
        Me.rdbDaAcquisire.Enabled = abilita
        Me.rdbIgnoraAcquisizione.Enabled = abilita

    End Sub

    Protected Overrides Sub EliminaProgrammazioneEffettuata()

        ReloadData(Me.dgrPazienti.CurrentPageIndex)

    End Sub

    Protected Overrides Sub EliminaProgrammazioneNonEffettuata()

        ReloadData(Me.dgrPazienti.CurrentPageIndex)

    End Sub

#End Region

#Region " Private "

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()

        listPrintButtons.Add(New PrintButton(Constants.ReportName.ElencoImmigrati, "btnStampaElenco"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.ElencoImmigratiComune, "btnStampaElencoPerComune"))
        listPrintButtons.Add(New PrintButton(Constants.ReportName.EtichetteImmigrati, "btnStampaEtichette"))

        Me.ShowToolbarPrintButtons(listPrintButtons, tlbMovimenti)

    End Sub

    Private Sub CaricaDati(currentPageIndex As Int32)

        Dim dstMovimentiEsterni As DstMovimentiEsterni = Nothing

        Dim countImmigrati As Int32 = 0

        Dim pagingOptions As New MovimentiCNSPagingOptions()
        pagingOptions.StartRecordIndex = currentPageIndex * Me.dgrPazienti.PageSize
        pagingOptions.EndRecordIndex = pagingOptions.StartRecordIndex + Me.dgrPazienti.PageSize

        Me.FiltriRicerca = Me.GetFiltriRicerca()

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            dstMovimentiEsterni = genericProvider.MovimentiEsterniCNS.LoadImmigrati(Me.FiltriRicerca, pagingOptions)

            countImmigrati = genericProvider.MovimentiEsterniCNS.CountImmigrati(Me.FiltriRicerca)

        End Using

        Me.dgrPazienti.VirtualItemCount = countImmigrati
        Me.dgrPazienti.CurrentPageIndex = currentPageIndex

        ' Possibilità di cambio pagina solo se non in modifica
        Me.dgrPazienti.PagerStyle.Visible = Not Me.IsPageInEdit()

        Me.dgrPazienti.DataSource = dstMovimentiEsterni.MovimentiEsterni
        Me.dgrPazienti.DataBind()

        Me.dgrPazienti.SelectedIndex = -1

        Me.divSezioneMovimenti.InnerText = String.Format(" MOVIMENTI: {0} risultat{1}.", Me.dgrPazienti.VirtualItemCount, IIf(Me.dgrPazienti.VirtualItemCount = 1, "o", "i"))

    End Sub

    Private Sub ReloadData(currentPageIndex As Int32)

        StatoPagina = StatoPaginaMovimenti.Lettura

        ' Datagrid in sola lettura
        dgrPazienti.EditItemIndex = -1

        ' Riesegue la ricerca e il bind del datagrid
        SetFiltriRicerca()
        CaricaDati(currentPageIndex)

    End Sub

    Private Function GetFiltriRicerca() As MovimentiCNSImmigratiFilter

        Dim filter As New MovimentiCNSImmigratiFilter()

        filter.CodiceAsl = OnVacContext.CodiceUslCorrente
        filter.CodiceConsultorio = OnVacUtility.Variabili.CNS.Codice

        If Not String.IsNullOrEmpty(odpDaNascita.Text) Then filter.DataNascitaInizio = odpDaNascita.Data
        If Not String.IsNullOrEmpty(odpANascita.Text) Then filter.DataNascitaFine = odpANascita.Data
        If Not String.IsNullOrEmpty(dpkDaImm.Text) Then filter.DataImmigrazioneInizio = dpkDaImm.Data
        If Not String.IsNullOrEmpty(dpkAImm.Text) Then filter.DataImmigrazioneFine = dpkAImm.Data

        If Me.rdbPazReg.Checked Then
            filter.Regolarizzato = True
        ElseIf Me.rdbPazNoReg.Checked Then
            filter.Regolarizzato = False
        End If

        If Me.rdbCertRich.Checked Then
            filter.CertificatoRichiesto = True
        ElseIf Me.rdbCertNonRich.Checked Then
            filter.CertificatoRichiesto = False
        End If

        Dim statiAcquisizioneImmigrazioneFilter As New List(Of Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione)()

        If Me.rdbAcquisiti.Checked Then
            statiAcquisizioneImmigrazioneFilter.Add(Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Acquisito)
        ElseIf Me.rdbAcquisitiErrore.Checked Then
            statiAcquisizioneImmigrazioneFilter.Add(Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Avvertimento)
        ElseIf Me.rdbDaAcquisire.Checked Then
            statiAcquisizioneImmigrazioneFilter.Add(Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Errore)
            statiAcquisizioneImmigrazioneFilter.Add(Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Nessuno)
        End If

        filter.StatiAcquisizioneImmigrazione = statiAcquisizioneImmigrazioneFilter.ToArray()

        Return filter

    End Function

    Private Sub SetFiltriRicerca()

        If Not FiltriRicerca Is Nothing Then

            FiltriRicerca.CodiceAsl = OnVacContext.CodiceUslCorrente

            If Not FiltriRicerca.DataNascitaInizio Is Nothing Then
                odpDaNascita.Data = FiltriRicerca.DataNascitaInizio
            End If

            If Not FiltriRicerca.DataNascitaFine Is Nothing Then
                odpANascita.Data = FiltriRicerca.DataNascitaFine
            End If

            If Not FiltriRicerca.DataImmigrazioneInizio Is Nothing Then
                dpkDaImm.Data = FiltriRicerca.DataImmigrazioneInizio
            End If

            If Not FiltriRicerca.DataImmigrazioneFine Is Nothing Then
                dpkAImm.Data = FiltriRicerca.DataImmigrazioneFine
            End If

            Dim pazReg As Boolean = False
            Dim pazNoReg As Boolean = False
            Dim ignoraPazReg As Boolean = False

            If Not FiltriRicerca.Regolarizzato Is Nothing Then
                If FiltriRicerca.Regolarizzato Then
                    pazReg = True
                Else
                    pazNoReg = True
                End If
            Else
                ignoraPazReg = True
            End If

            rdbPazReg.Checked = pazReg
            rdbPazNoReg.Checked = pazNoReg
            rdbIgnoraPazReg.Checked = ignoraPazReg

            Dim certRich As Boolean = False
            Dim certNonRich As Boolean = False
            Dim certCertIgnora As Boolean = False

            If Not FiltriRicerca.CertificatoRichiesto Is Nothing Then
                If FiltriRicerca.CertificatoRichiesto Then
                    certRich = True
                Else
                    certNonRich = True
                End If
            Else
                certCertIgnora = True
            End If

            rdbCertRich.Checked = certRich
            rdbCertNonRich.Checked = certNonRich
            rdbCertIgnora.Checked = certCertIgnora

            Dim daAcquisire As Boolean = False
            Dim acquisitiErrore As Boolean = False
            Dim acquisiti As Boolean = False
            Dim ignoraAcquisizione As Boolean = False

            If Not FiltriRicerca.StatiAcquisizioneImmigrazione Is Nothing AndAlso FiltriRicerca.StatiAcquisizioneImmigrazione.Length > 0 Then
                If FiltriRicerca.StatiAcquisizioneImmigrazione.Contains(Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Errore) Or FiltriRicerca.StatiAcquisizioneImmigrazione.Contains(Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Nessuno) Then
                    daAcquisire = True
                ElseIf FiltriRicerca.StatiAcquisizioneImmigrazione.Contains(Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Avvertimento) Then
                    acquisitiErrore = True
                ElseIf FiltriRicerca.StatiAcquisizioneImmigrazione.Contains(Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Acquisito) Then
                    acquisiti = True
                End If
            Else
                ignoraAcquisizione = True
            End If

            rdbDaAcquisire.Checked = daAcquisire
            rdbAcquisitiErrore.Checked = acquisitiErrore
            rdbAcquisiti.Checked = acquisiti
            rdbIgnoraAcquisizione.Checked = ignoraAcquisizione

        End If

    End Sub

    Private Sub SetAcquisizioneCommand(lblAcquisizione As Label, btnAcquisizione As ImageButton, imgAcquisizione As System.Web.UI.WebControls.Image, acquisibile As Boolean, statoAcquisizione As Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione)
        '--
        Dim lblVisibility As Boolean = False
        Dim btnVisibility As Boolean = False
        Dim imgVisibility As Boolean = False
        '--
        If acquisibile Then
            Select Case statoAcquisizione
                Case Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Nessuno, Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Errore
                    btnVisibility = True
                Case Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Avvertimento
                    imgVisibility = True
                Case Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione.Acquisito
                    lblVisibility = True
            End Select
        End If
        '--
        lblAcquisizione.Visible = lblVisibility
        btnAcquisizione.Visible = btnVisibility
        imgAcquisizione.Visible = imgVisibility
        '--
    End Sub

    Private Sub RegolarizzaPaziente(idx As Integer)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizPaziente As New Biz.BizPaziente(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                bizPaziente.SetFlagRegolarizzaPaziente(Me.dgrPazienti.Items(idx).Cells(0).Text)

            End Using
        End Using

    End Sub

    Private Sub RichiediCertificato(idx As Integer)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizPaziente As New Biz.BizPaziente(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                bizPaziente.SetFlagRichiestaCertificatoPaziente(Me.dgrPazienti.Items(idx).Cells(0).Text, True)

            End Using
        End Using

    End Sub

    Private Sub AddAcquisizionePartMessage(message As System.Text.StringBuilder, titolo As String, presenti As Boolean)

        message.AppendFormat("{0}: ", titolo.ToUpper())

        If Not presenti Then
            message.Append("ok")
        Else
            message.Append("già presenti")
        End If

        message.Append("\n")

    End Sub

#Region " Stampe "

    Private Sub StampaElenco(nomeReport As String)

        FiltriRicerca = GetFiltriRicerca()

        Dim datiIntestazione As Entities.DatiIntestazioneReport = Nothing

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            ' Caricamento dati da stampare 
            Dim dst As DstMovimentiEsterni = genericProvider.MovimentiEsterniCNS.LoadImmigrati(FiltriRicerca, Nothing)

            Dim rpt As New ReportParameter()

            rpt.set_dataset(dst)

            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                ' Caricamento parametri intestazione report
                datiIntestazione = bizReport.GetDatiIntestazione()

                ' Parametri intestazione
                rpt.AddParameter("UslCitta", datiIntestazione.ComuneUsl)
                rpt.AddParameter("UslDesc", datiIntestazione.DescrizioneUslPerReport)
                rpt.AddParameter("UslReg", datiIntestazione.RegioneUsl)

                rpt.AddParameter("certificato", IIf(Me.rdbCertRich.Checked, Me.rdbCertRich.Text, IIf(Me.rdbCertNonRich.Checked, Me.rdbCertNonRich.Text, "Entrambi")))
                rpt.AddParameter("Ambulatorio", OnVacUtility.Variabili.CNS.Descrizione + "(" + OnVacUtility.Variabili.CNS.Codice + ")")

                rpt.AddParameter("TitoloReport", "Elenco Assistiti Immigrati")

                ' Filtri
                rpt.AddParameter("Da_data_nascita", Me.GetDateParameterValue(Me.odpDaNascita))
                rpt.AddParameter("A_data_nascita", Me.GetDateParameterValue(Me.odpANascita))

                rpt.AddParameter("Da_data_imm", Me.GetDateParameterValue(Me.dpkDaImm))
                rpt.AddParameter("A_data_imm", Me.GetDateParameterValue(Me.dpkAImm))

                rpt.AddParameter("Da_data_inizio_residenza", String.Empty)
                rpt.AddParameter("A_data_inizio_residenza", String.Empty)

                rpt.AddParameter("Da_data_inizio_domicilio", String.Empty)
                rpt.AddParameter("A_data_inizio_domicilio", String.Empty)

                rpt.AddParameter("Da_data_inizio_assistenza", String.Empty)
                rpt.AddParameter("A_data_inizio_assistenza", String.Empty)

                rpt.AddParameter("stato_anagrafico", String.Empty)

                rpt.AddParameter("regolarizzato", IIf(Me.rdbPazReg.Checked, "regolarizzati", IIf(Me.rdbPazNoReg.Checked, "non regolarizzati", "entrambi")))

                ' Stampa
                If Not OnVacReport.StampaReport(Page.Request.Path, nomeReport, String.Empty, rpt, Nothing, Nothing, bizReport.GetReportFolder(nomeReport)) Then
                    OnVacUtility.StampaNonPresente(Page, nomeReport)
                End If

            End Using
        End Using

    End Sub

    Private Sub StampaEtichette()

        Dim dst As DstMovimentiEsterni = Nothing

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            dst = genericProvider.MovimentiEsterniCNS.LoadImmigrati(Me.GetFiltriRicerca(), Nothing)

        End Using

        Dim errMsg As String = String.Empty

        If Not UscFiltriEtichette.StampaEtichette(dst, errMsg) Then

            ' Stampa non effettuata, mostro il messaggio di errore
            ClientScript.RegisterClientScriptBlock(GetType(String),
                                                   "msg_stampa",
                                                   String.Format("<script type='text/javascript'>alert('{0}');</script>", errMsg))
        End If

    End Sub

#End Region

#End Region

End Class
