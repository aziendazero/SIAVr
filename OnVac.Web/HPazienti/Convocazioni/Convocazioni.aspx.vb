Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager
Imports Onit.Controls
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Biz
Imports Onit.OnAssistnet.OnVac.Entities

Partial Class Convocazioni
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

#Region " Consts "

    Protected Const SelectCommandDataGridColumnIndex As Int16 = 0
    Protected Const EditCommandDataGridColumnIndex As Int16 = 1
    Protected Const DeleteCommandDataGridColumnIndex As Int16 = 2
    Protected Const RitardoDataGridColumnIndex As Int16 = 3
    Protected Const DescrizioneCentroVaccinaleDataGridColumnIndex As Int16 = 4
    Protected Const DataConvocazioneDataGridColumnIndex As Int16 = 5
    Protected Const DataInvitoDataGridColumnIndex As Int16 = 6
    Protected Const DataAppuntamentoDataGridColumnIndex As Int16 = 7
    Protected Const DurataAppuntamentoDataGridColumnIndex As Int16 = 8
    Protected Const DescrizioneCicloDataGridColumnIndex As Int16 = 9
    Protected Const NumeroSedutaDataGridColumnIndex As Int16 = 10
    Protected Const DescrizioneAssociazioneDataGridColumnIndex As Int16 = 11
    Protected Const DescrizioneVaccinazioneDataGridColumnIndex As Int16 = 12
    Protected Const TipoConvocazioneDataGridColumnIndex As Int16 = 13

    Private Const ONITLAYOUT_KEY_WARNING As String = "Warning"
    Private Const ONITLAYOUT_KEY_ASK_CONFIRM_AUTO_CNV As String = "AskConfirmAutoCnv"

#End Region

#Region " Properties "

    Public ReadOnly Property RedirectFromVacProg() As Boolean
        Get
            Return Not Me.Request.QueryString("EditCnv") Is Nothing
        End Get
    End Property

    Public ReadOnly Property IgnoreAutoCnv() As Boolean
        Get
            Return Not Me.Request.QueryString("IgnoreAutoCnv") Is Nothing
        End Get
    End Property

    Public Property Editable() As Boolean
        Get
            If Me.ViewState("EDTBL") Is Nothing AndAlso RedirectFromVacProg Then
                Return Boolean.Parse(Me.Request.QueryString("EditCnv"))
            Else
                Return Me.ViewState("EDTBL")
            End If
        End Get
        Set(value As Boolean)
            Me.ViewState("EDTBL") = value
        End Set
    End Property

    Public ReadOnly Property AskAutoCnv() As Boolean
        Get
            Dim askAutoCnvQueryString As String = Me.Request.QueryString("AskAutoCnv")
            If askAutoCnvQueryString <> Nothing Then
                Return Boolean.Parse(askAutoCnvQueryString)
            Else
                Return False
            End If
        End Get
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

#Region " Overrides "

    Protected Overrides Sub OnLoad(e As EventArgs)

        MyBase.OnLoad(e)

        If Not IsPostBack() Then

            OnVacUtility.ImpostaIntestazioniPagina(OnitLayout31, LayoutTitolo, Nothing, Settings, IsGestioneCentrale)

            StoricoVaccinaleCentralizzatoDaRecuperare = False

            ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
            If FlagConsensoVaccUslCorrente Then

                ToolBar.FindItemByValue("btnRecuperaStoricoVacc").Visible = True

                Dim statoAcquisizioneDatiVaccinaliCentrale As Enumerators.StatoAcquisizioneDatiVaccinaliCentrale? =
                    Common.OnVacStoricoVaccinaleCentralizzato.GetStatoAcquisizioneDatiVaccinaliCentralePaziente(OnVacUtility.Variabili.PazId)

                If Not statoAcquisizioneDatiVaccinaliCentrale.HasValue OrElse
                   statoAcquisizioneDatiVaccinaliCentrale = Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Parziale Then

                    StoricoVaccinaleCentralizzatoDaRecuperare = True
                    Editable = False

                    InizializzaConvocazioni(False, False)

                    OnitLayout31.InsertRoutineJS(Common.OnVacStoricoVaccinaleCentralizzato.AlertMessageRecuperoStoricoVaccinale)

                Else

                    InizializzazionePagina()

                End If

            Else

                ToolBar.FindItemByValue("btnRecuperaStoricoVacc").Visible = False
                InizializzazionePagina()

            End If

        End If

    End Sub

#End Region

#Region " Events Handles "

    Protected Sub ToolBar_ButtonClick(sender As Object, e As Telerik.Web.UI.RadToolBarEventArgs) Handles ToolBar.ButtonClick
        '--
        Select Case e.Item.Value
            '--
            Case "CreaCNV"
                '---
                Me.ShowModaleCreaCNV()
                '---
            Case "btnRecuperaStoricoVacc"
                '---
                Dim command As New Common.OnVacStoricoVaccinaleCentralizzato.AcquisisciStoricoCommand()
                command.CodicePaziente = OnVacUtility.Variabili.PazId
                command.RichiediConfermaSovrascrittura = False
                command.Settings = Me.Settings
                command.OnitLayout3 = Me.OnitLayout31
                command.BizLogOptions = OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.VAC_PROGRAMMATE)
                command.Note = "Recupero Storico Vaccinale da maschera Convocazioni"
                '--
                Common.OnVacStoricoVaccinaleCentralizzato.AcquisisciDatiVaccinaliCentraliPaziente(command)
                '---
                Me.StoricoVaccinaleCentralizzatoDaRecuperare = False
                Me.Editable = True
                '---
                Me.InizializzazionePagina()
                '---
            Case "btnCompilaBilancio"
                '---
                RedirectToGestioneBilanci()
                '---
            Case "btnVisioneBilanci"
                '---
                RedirectToVisioneBilanci()
                '---
        End Select
        '--
    End Sub

    Private Sub dg_Cnv_EditCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_Cnv.EditCommand
        '--
        Me.ModificaConvocazione(DateTime.Parse(e.Item.Cells(Convocazioni.DataConvocazioneDataGridColumnIndex).Text))
        '--
    End Sub

    Private Sub dg_Cnv_DeleteCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_Cnv.DeleteCommand

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizConvocazione As New BizConvocazione(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), New BizLogOptions(DataLogStructure.TipiArgomento.VAC_PROGRAMMATE, False))

                Dim dataConvocazione As DateTime = DateTime.Parse(e.Item.Cells(Convocazioni.DataConvocazioneDataGridColumnIndex).Text)

                Dim command As New BizConvocazione.EliminaConvocazioniSollecitiBilanciCommand()
                command.CodicePaziente = Convert.ToInt64(OnVacUtility.Variabili.PazId)
                command.DataConvocazione = dataConvocazione
                command.CancellaBilanciAssociati = True
                command.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.EliminazioneConvocazione
                command.DataEliminazione = DateTime.Now
                command.NoteEliminazione = "Eliminata convocazione da maschera Convocazioni"
                command.WriteLog = True

                bizConvocazione.EliminaConvocazioniSollecitiBilanci(command)

            End Using
        End Using

        Me.CaricaConvocazioni()

    End Sub

    Private Sub dg_Cnv_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles dg_Cnv.SelectedIndexChanged

        Dim lstBilanci As List(Of BilancioProgrammato) = Nothing

        ' Caricamento bilanci solo se lo storico vaccinale non deve essere recuperato
        If Not Me.StoricoVaccinaleCentralizzatoDaRecuperare Then

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                Using bizBilanci As New Biz.BizBilancioProgrammato(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                    lstBilanci = bizBilanci.GetBilanciPaziente(OnVacUtility.Variabili.PazId)
                End Using

            End Using

        End If

        If Not lstBilanci Is Nothing AndAlso lstBilanci.Count > 0 Then

            ' Se lo storico è stato recuperato e ci sono bilanci, viene visualizzata la modale
            Me.modRicercaBilancioCnv.VisibileMD = True
            Me.uscRicercaBilancioCnv.DataSource = lstBilanci
            Me.uscRicercaBilancioCnv.DataBind()

        Else

            Me.modRicercaBilancioCnv.VisibileMD = False

            Me.Response.Redirect(String.Format("../VacProg/VacProg.aspx?DataCnv={0}&EditCnv={1}",
                                               HttpUtility.UrlEncode(Me.dg_Cnv.SelectedItem.Cells(Convocazioni.DataConvocazioneDataGridColumnIndex).Text),
                                               HttpUtility.UrlEncode(Me.Editable.ToString())))

        End If

    End Sub

    Private Sub uscRicercaBilancioCnv_AnnullaBilancio() Handles uscRicercaBilancioCnv.AnnullaBilancio

        Dim lstBilanci As List(Of BilancioProgrammato)
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizBilanci As New Biz.BizBilancioProgrammato(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                lstBilanci = bizBilanci.GetBilanciPaziente(OnVacUtility.Variabili.PazId)
            End Using

        End Using

        ' Abilitazione dell'esecuzione da vaccinazioni programmate solo se non sono presenti bilanci obbligatori
        Dim flagEditable As Boolean = Me.Editable

        If lstBilanci.Where(Function(p) p.BIL_OBBLIGATORIO).Count > 0 Then
            flagEditable = False
        End If

        Me.Response.Redirect(String.Format("../VacProg/VacProg.aspx?DataCnv={0}&EditCnv={1}",
                                           HttpUtility.UrlEncode(Me.dg_Cnv.SelectedItem.Cells(Convocazioni.DataConvocazioneDataGridColumnIndex).Text),
                                           HttpUtility.UrlEncode(flagEditable)))

    End Sub

    Private Sub uscRicercaBilancioCnv_ReturnBilancio(bil_numero As Integer, mal_codice As String) Handles uscRicercaBilancioCnv.ReturnBilancio

        Me.Response.Redirect(String.Format("../../HBilanci/GestioneBilancio/GestioneBilancio.aspx?isVacProgRequest=True&bil_numero={0}&mal_codice={1}&DataCnv={2}&EditCnv={3}",
                                           HttpUtility.UrlEncode(bil_numero),
                                           HttpUtility.UrlEncode(mal_codice),
                                           HttpUtility.UrlEncode(Me.dg_Cnv.SelectedItem.Cells(Convocazioni.DataConvocazioneDataGridColumnIndex).Text),
                                           HttpUtility.UrlEncode(Me.Editable.ToString())))

    End Sub

    Private Sub OnitLayout31_ConfirmClick(sender As Object, eventArgs As Onit.Controls.PagesLayout.OnitLayout3.ConfirmEventArgs) Handles OnitLayout31.ConfirmClick
        '--
        Select Case eventArgs.Key
            '--
            Case ONITLAYOUT_KEY_WARNING
                '--
                Me.Editable = eventArgs.Result
                '--
                Me.InizializzaConvocazioni(False, False)
                '--
            Case ONITLAYOUT_KEY_ASK_CONFIRM_AUTO_CNV
                '--
                Me.InizializzaConvocazioni(Not eventArgs.Result, False)
                '--
        End Select
        '--
    End Sub

    Private Sub uscInsAssociazione_Conferma(sender As Object, confermaEventArgs As InsAssociazione.ConfermaEventArgs) Handles uscInsAssociazione.Conferma
        '--
        If confermaEventArgs.Esito Then
            '-- Inserimento vaccinazioni programmate --- '
            Using dam As IDAM = OnVacUtility.OpenDam()
                '--
                Try
                    '--
                    dam.BeginTrans()
                    '--
                    Using genericProvider As Onit.OnAssistnet.OnVac.DAL.DbGenericProvider = New Onit.OnAssistnet.OnVac.DAL.DbGenericProvider(dam)
                        '--
                        If Not genericProvider.Convocazione.Exists(OnVacUtility.Variabili.PazId, Me.uscInsAssociazione.DataConvocazione) Then
                            '--
                            ' Nelle convocazioni manuali il bilancio e la malattia non possono essere lasciati nulli,
                            ' perché la gestione di questi prevede che il bilancio non presente sia uguale al valore 0 
                            ' e la malattia "Nessuna" è uguale a "0" (parametro su db).
                            '--
                            Dim codiceConsultorio As String
                            '--
                            If Me.uscCreaCnv.IsCurrentCNSPaziente Then
                                '--
                                codiceConsultorio = Me.uscCreaCnv.CodiceCNSPaziente
                                '--
                            Else
                                '--
                                codiceConsultorio = OnVacUtility.Variabili.CNS.Codice
                                '--
                            End If
                            '--
                            Using gestioneConvocazioni As New CalcoloConvocazioni.GestioneConvocazioni(codiceConsultorio, OnVacContext.CreateBizContextInfos(), dam.Provider, dam.Connection, dam.Transaction)
                                gestioneConvocazioni.CreaSingolaConvocazione(OnVacUtility.Variabili.PazId, Me.uscInsAssociazione.DataConvocazione, Convert.ToInt16(Me.Settings.TEMPOSED))
                            End Using
                            '--
                        End If

                        Using bizVacProg As New Biz.BizVaccinazioneProg(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)

                            Dim now As DateTime = DateTime.Now

                            For Each drVaccinazioneSelezionata As DataRow In confermaEventArgs.VaccinazioniSelezionate.Rows

                                bizVacProg.InsertVaccinazioneProgrammata(OnVacUtility.Variabili.PazId, Me.uscInsAssociazione.DataConvocazione,
                                                                         drVaccinazioneSelezionata("VAC_CODICE").ToString(), drVaccinazioneSelezionata("VAL_ASS_CODICE").ToString(),
                                                                         now, OnVacContext.UserId)

                            Next

                        End Using

                    End Using
                    '--
                    dam.Commit()
                    '--
                Catch ex As Exception
                    '--
                    If dam.ExistTra Then dam.Rollback()
                    '--
                    ex.InternalPreserveStackTrace()
                    Throw
                    '--
                End Try
                '--
            End Using
            '--
            Me.CaricaConvocazioni()
            '--
            If Me.uscCreaCnv.CurrentType = CreaCnv.CreaCnvEventArgs.CnvType.Futura Then
                '--
                Me.ModificaConvocazione(Me.uscInsAssociazione.DataConvocazione)
                '--
            End If
            '--
            Me.modInsAssociazione.VisibileMD = False
            '--
        End If
        '--
    End Sub

    Private Sub uscInsAssociazione_Annulla(sender As Object, e As EventArgs) Handles uscInsAssociazione.Annulla
        '--
        Me.modInsAssociazione.VisibileMD = False
        '--
    End Sub

    Private Sub uscCreaCnv_CreaCnv(sender As Object, e As OnVac.CreaCnv.CreaCnvEventArgs) Handles uscCreaCnv.CreaCnv
        '--
        Select Case e.Type
            '--
            Case CreaCnv.CreaCnvEventArgs.CnvType.Futura
                '--
                Me.CreaCnvManuale(e.CnsCodice, True)
                '--
            Case CreaCnv.CreaCnvEventArgs.CnvType.Odierna
                '--
                Me.CreaCnvManuale(e.CnsCodice, False)
                '--
            Case CreaCnv.CreaCnvEventArgs.CnvType.Automatica
                '--
                Me.CreaCnvAutomatica(e.CnsCodice)
                '--
        End Select
        '--
        Me.modCreaCnv.VisibileMD = False
        '--
        Me.OnitLayout31.Busy = False
        '--
    End Sub

    Private Sub uscModConv_Salvato(sender As Object, e As EventArgs) Handles uscModConv.Salvato
        '--
        Me.CaricaConvocazioni()
        '--
        Me.modModConv.VisibileMD = False
        '--
    End Sub

#End Region

#Region " Private "

    Private Sub InizializzazionePagina()
        '-- 
        If Me.RedirectFromVacProg Then
            '--
            Me.InizializzaConvocazioni(Me.IgnoreAutoCnv, Me.AskAutoCnv)
            '--
        Else
            '--
            Me.Editable = False
            '--
            Using dam As IDAM = OnVacUtility.OpenDam()
                '--
                Using genericProvider As New DbGenericProvider(dam)
                    '--
                    Dim bizPaziente As New Biz.BizPaziente(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                    '--
                    Dim campiObbligatoriMancanti As Boolean = bizPaziente.ControlloCampiObbligatori(OnVacUtility.Variabili.PazId, Nothing, Nothing)
                    '--
                    If Not campiObbligatoriMancanti Then
                        '--
                        Dim dtPaziente As DataTable = bizPaziente.GetFromKey(OnVacUtility.Variabili.PazId)
                        '--
                        ' Controlli bloccanti sui dati del paziente
                        Dim esitoControlliBloccanti As Boolean = Me.ControlliBloccanti(dtPaziente, genericProvider)
                        '--
                        If esitoControlliBloccanti Then
                            '--
                            Dim message As New System.Text.StringBuilder()
                            '--
                            ' Controllo flag CANCELLATO
                            '--
                            Dim esitoControlloFlagCancellato As Boolean = True
                            '--
                            If Me.Settings.FLAG_CANCELLATO_CHECK Then
                                '--
                                Dim flagCancellato As Boolean = (dtPaziente.Rows(0)("PAZ_CANCELLATO").ToString() = "S")
                                '--
                                If flagCancellato Then
                                    message.Append("\nIl paziente è marcato come CANCELLATO.")
                                    esitoControlloFlagCancellato = False
                                End If
                                '--
                            End If
                            '--
                            If esitoControlloFlagCancellato Then
                                '--
                                ' Controlli Non Bloccanti
                                '--
                                message.Append(Me.GetMessaggioControlliNonBloccanti(dtPaziente, bizPaziente))
                                '--
                            End If
                            '--
                            'Gestione Consenso
                            '--
                            message.Append(Me.GetAlertConsenso(genericProvider))
                            '--
                            If message.Length = 0 Then
                                '--
                                Me.Editable = True
                                '--
                                Me.InizializzaConvocazioni(False, False)
                                '--
                            Else
                                '--
                                message.Insert(0, "ATTENZIONE !!!\n")
                                '--
                                If Not esitoControlloFlagCancellato Then
                                    '--
                                    ' Blocco per paziente cancellato: toolbar bloccata, possibilità di redirect alla convocazione selezionata
                                    '--
                                    Me.Editable = False
                                    '--
                                    Me.InizializzaConvocazioni(False, False)
                                    '--
                                    Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(message.ToString(), "ControlloFlagCancellato", False, False))
                                    '--
                                Else
                                    '--
                                    ' Richiesta all'utente se continuare oppure no
                                    '--
                                    message.Append("\n\nContinuare ?")
                                    '--
                                    Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(message.ToString(), ONITLAYOUT_KEY_WARNING, True, True))
                                    '--
                                End If
                                '--
                            End If
                            '--
                        End If
                        '-- 
                        ' Visualizzazione dell'eventuale presenza di reazioni avverse e note vaccinali del paziente 
                        '--
                        Dim hasReazioniAvverse As Boolean
                        '--
                        Using bizVaccEseguite = New Biz.BizVaccinazioniEseguite(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                            '--
                            hasReazioniAvverse = bizVaccEseguite.HasReazioniAvverse(OnVacUtility.Variabili.PazId, Me.IsGestioneCentrale)
                            '--
                        End Using
                        '--
                        ' Controllo presenza note nei campi liberi del paziente
                        '--
                        Dim hasNote As Boolean = False
                        '--
                        If Me.Settings.VISNOTE Then
                            '--
                            hasNote = bizPaziente.HasNote(OnVacUtility.Variabili.PazId, True, IsGestioneCentrale)
                            '--
                        End If
                        '--
                        ' Apertura modale visualizzazione note e presenza reazioni avverse
                        '--
                        If hasReazioniAvverse OrElse hasNote Then
                            '--
                            Me.uscNotePaziente.HasReazioniAvvese = hasReazioniAvverse
                            '--
                            Me.uscNotePaziente.LoadModale()
                            '--
                            Me.modNotePaziente.VisibileMD = True
                            '--
                        End If
                        '--
                        ' Left menu
                        '--
                        If Not Request.QueryString("fromRedirect") Is Nothing Then
                            '--
                            Me.OnitLayout31.InsertRoutineJS(Me.GetOpenLeftFrameScript(True) + Environment.NewLine)
                            '--
                        End If
                        '--                   
                    Else
                        '--
                        Me.RedirectToGestionePaziente(OnVacUtility.Variabili.PazId)
                        '--
                    End If
                    '--
                End Using
                '--
            End Using
            '--
        End If
        '--
    End Sub

    Private Sub InizializzaConvocazioni(ignoreAutoCnv As Boolean, askConfirmIfAutoCnv As Boolean)
        '--
        If Me.Editable Then
            '--
            If Not ignoreAutoCnv AndAlso Me.Settings.AUTOCONV Then
                '--
                Using dam As IDAM = OnVacUtility.OpenDam()
                    '--
                    Using genericProvider As New DbGenericProvider(dam)
                        '--
                        Using bizPaziente As New Biz.BizPaziente(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                            '--
                            Dim dtPaziente As DataTable = bizPaziente.GetFromKey(OnVacUtility.Variabili.PazId)
                            '--
                            Dim codiceConsultorio As String = dtPaziente.Rows(0)("PAZ_CNS_CODICE").ToString()
                            Dim statoAnagrafico As Enumerators.StatoAnagrafico = [Enum].Parse(GetType(Enumerators.StatoAnagrafico), dtPaziente.Rows(0)("PAZ_STATO_ANAGRAFICO").ToString())
                            '--
                            If codiceConsultorio = OnVacUtility.Variabili.CNS.Codice Then
                                '--
                                Dim statiAnagraficiAttivi As Hashtable = bizPaziente.StatiAnagrafici(True)
                                '--
                                If statiAnagraficiAttivi.ContainsKey(statoAnagrafico.ToString("d")) Then
                                    '--
                                    If Not askConfirmIfAutoCnv Then
                                        '-- 
                                        Using gestioneConvocazioni As New CalcoloConvocazioni.GestioneConvocazioni(OnVacUtility.Variabili.CNS.Codice, OnVacContext.CreateBizContextInfos, dam.Provider, dam.Connection, dam.Transaction)
                                            gestioneConvocazioni.CalcolaConvocazioni(OnVacUtility.Variabili.PazId, OnVacContext.UserId)
                                        End Using
                                        '--
                                    Else
                                        '--
                                        Me.OnitLayout31.ShowMsgBox(New PagesLayout.OnitLayout3.OnitLayoutMsgBox("Attenzione !!!\nVerrà calcolata in automatico la programmazione vaccinale del paziente.\nContinuare ?",
                                                                                                                ONITLAYOUT_KEY_ASK_CONFIRM_AUTO_CNV, True, True))
                                        '-- 
                                    End If
                                    '---
                                End If
                                '--
                            Else
                                '--
                                Me.ShowModaleCreaCNV()
                                '--
                            End If
                            '---
                        End Using
                        '--
                    End Using
                    '--
                End Using
                '--
            End If
            '---
        End If
        '--
        Me.CaricaConvocazioni()
        '--
        Me.ToolBar.FindItemByValue("CreaCNV").Enabled = Me.Editable
        Me.ToolBar.FindItemByValue("btnRecuperaStoricoVacc").Enabled = Me.StoricoVaccinaleCentralizzatoDaRecuperare
        Me.ToolBar.FindItemByValue("btnCompilaBilancio").Enabled = Not Me.StoricoVaccinaleCentralizzatoDaRecuperare
        Me.ToolBar.FindItemByValue("btnVisioneBilanci").Enabled = Not Me.StoricoVaccinaleCentralizzatoDaRecuperare
        '--
    End Sub

    Private Sub CaricaConvocazioni()
        '--
        Dim dt_vacCnv As DataTable = Nothing
        '--
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            dt_vacCnv = genericProvider.Convocazione.GetDtConvocazioniPaziente(OnVacUtility.Variabili.PazId)
        End Using
        '--
        Me.dg_Cnv.DataSource = dt_vacCnv
        Me.dg_Cnv.DataBind()
        '--
        Dim alternatingCssClass As String = Me.dg_Cnv.AlternatingItemStyle.CssClass
        Dim itemCssClass As String = Me.dg_Cnv.ItemStyle.CssClass
        '--
        Dim dataTemp As String = String.Empty
        Dim cicTemp As String = String.Empty
        Dim sedTemp As String = String.Empty
        '--
        For i As Int16 = 0 To Me.dg_Cnv.Items.Count - 1
            '-- controllo se il ciclo si ripete (controllo ciclo-seduta, altrimenti viene cancellata la descrizione del ciclo anche se la seduta è diversa)
            If Me.dg_Cnv.Items.Item(i).Cells(Convocazioni.DescrizioneCicloDataGridColumnIndex).Text = cicTemp AndAlso
               Me.dg_Cnv.Items.Item(i).Cells(Convocazioni.NumeroSedutaDataGridColumnIndex).Text = sedTemp Then
                '--
                Me.dg_Cnv.Items.Item(i).Cells(Convocazioni.DescrizioneCicloDataGridColumnIndex).Text = "&nbsp;"
                '--
            Else
                '--
                cicTemp = Me.dg_Cnv.Items.Item(i).Cells(Convocazioni.DescrizioneCicloDataGridColumnIndex).Text
                sedTemp = Me.dg_Cnv.Items.Item(i).Cells(Convocazioni.NumeroSedutaDataGridColumnIndex).Text
                '--
            End If
            '--
            If i = 0 Then
                '--
                dataTemp = Me.dg_Cnv.Items.Item(i).Cells(Convocazioni.DataConvocazioneDataGridColumnIndex).Text
                '--
                Me.dg_Cnv.Items.Item(i).CssClass = itemCssClass
                '--
            Else
                '--
                If Me.dg_Cnv.Items.Item(i).Cells(Convocazioni.DataConvocazioneDataGridColumnIndex).Text = dataTemp Then
                    '--
                    Me.dg_Cnv.Items.Item(i).CssClass = Me.dg_Cnv.Items.Item(i - 1).CssClass
                    Me.dg_Cnv.Items.Item(i).Cells(Convocazioni.SelectCommandDataGridColumnIndex).FindControl("imgSelect").Visible = False
                    Me.dg_Cnv.Items.Item(i).Cells(Convocazioni.EditCommandDataGridColumnIndex).FindControl("imgEdit").Visible = False
                    Me.dg_Cnv.Items.Item(i).Cells(Convocazioni.DeleteCommandDataGridColumnIndex).FindControl("imgDelete").Visible = False
                    Me.dg_Cnv.Items.Item(i).Cells(Convocazioni.RitardoDataGridColumnIndex).FindControl("imgRitardo").Visible = False
                    Me.dg_Cnv.Items.Item(i).Cells(Convocazioni.DescrizioneCentroVaccinaleDataGridColumnIndex).Text = "&nbsp;"
                    Me.dg_Cnv.Items.Item(i).Cells(Convocazioni.DataConvocazioneDataGridColumnIndex).Text = "&nbsp;"
                    Me.dg_Cnv.Items.Item(i).Cells(Convocazioni.DataInvitoDataGridColumnIndex).Text = "&nbsp;"
                    Me.dg_Cnv.Items.Item(i).Cells(Convocazioni.DataAppuntamentoDataGridColumnIndex).Text = "&nbsp;"
                    Me.dg_Cnv.Items.Item(i).Cells(Convocazioni.DurataAppuntamentoDataGridColumnIndex).Text = "&nbsp;"
                    '--
                Else
                    '--
                    dataTemp = Me.dg_Cnv.Items.Item(i).Cells(Convocazioni.DataConvocazioneDataGridColumnIndex).Text
                    '--
                    If Me.dg_Cnv.Items.Item(i - 1).CssClass = alternatingCssClass Then
                        Me.dg_Cnv.Items.Item(i).CssClass = itemCssClass
                    Else
                        Me.dg_Cnv.Items.Item(i).CssClass = alternatingCssClass
                    End If
                    '--
                End If
                '--
            End If
            '--
            If Not Me.Editable Then
                Me.dg_Cnv.Items(i).Cells(Convocazioni.EditCommandDataGridColumnIndex).FindControl("imgEdit").Visible = False
                Me.dg_Cnv.Items(i).Cells(Convocazioni.DeleteCommandDataGridColumnIndex).FindControl("imgDelete").Visible = False
            End If
            '--
        Next
        '--
    End Sub

    Private Sub ModificaConvocazione(dataConvocazione As DateTime)
        '--
        Me.uscModConv.DataCnv = dataConvocazione
        Me.uscModConv.IsGestioneCentrale = Me.IsGestioneCentrale
        Me.uscModConv.ModaleName = "modModConv"
        Me.uscModConv.LoadModale()
        '--
        Me.modModConv.VisibileMD = True
        '--
    End Sub

    Private Sub CreaCnvManuale(cnsCodice As String, futura As Boolean)
        '--
        Dim dataFineSospensione As DateTime = DateTime.MinValue
        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizVisite As New BizVisite(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                dataFineSospensione = bizVisite.GetMaxDataFineSospensione(OnVacUtility.Variabili.PazId)
            End Using
        End Using
        '--
        Dim dataConvocazione As DateTime = DateTime.Now.Date
        If dataConvocazione < dataFineSospensione Then dataConvocazione = dataFineSospensione
        '--
        If futura Then
            '--
            While Me.ConvocazioneEsistente(dataConvocazione)
                '--
                dataConvocazione = dataConvocazione.AddDays(1)
                '--
            End While
            '--
        Else
            '--
            If Me.ConvocazioneEsistente(dataConvocazione) Then
                '--
                Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("ATTENZIONE !!!\n\nConvocazione odierna gia\' presente:\nutilizzare l\'apposito comando per aggiungere le vaccinazioni alla convocazione.", "OdiernaPreesistente", False, False))
                Exit Sub
                '--
            End If
            '--
        End If
        '--
        Me.uscInsAssociazione.DataConvocazione = dataConvocazione
        Me.uscInsAssociazione.CnsCodice = cnsCodice
        '--
        Me.uscInsAssociazione.ModaleName = "modInsAssociazione"
        Me.uscInsAssociazione.LoadModale()
        '--
        Me.modInsAssociazione.VisibileMD = True
        '--
    End Sub

    Private Sub CreaCnvAutomatica(cnsCodice As String)
        '--
        Using dam As IDAM = OnVacUtility.OpenDam()
            Using gestioneConvocazioni As New CalcoloConvocazioni.GestioneConvocazioni(cnsCodice, OnVacContext.CreateBizContextInfos, dam.Provider, dam.Connection, dam.Transaction)
                gestioneConvocazioni.CalcolaConvocazioni(OnVacUtility.Variabili.PazId, OnVacContext.UserId)
            End Using
        End Using
        '--
        Me.CaricaConvocazioni()
        '--
    End Sub

    Private Function ConvocazioneEsistente(data As DateTime) As Boolean

        For i As Integer = 0 To Me.dg_Cnv.Items.Count - 1

            Dim dataConvocazione As DateTime

            If Date.TryParse(Me.dg_Cnv.Items(i).Cells(Convocazioni.DataConvocazioneDataGridColumnIndex).Text, dataConvocazione) Then
                If dataConvocazione = data Then Return True
            End If

        Next

        Return False

    End Function

    '' Prepara il messaggio da visualizzare all'utente nel caso in cui il calcolo automatico delle convocazioni scarti alcune convocazioni da programmare. 
    'Private Sub ShowMessaggioCnvScartateIfNeeded(collConvocazioniScartate As BizConvocazione.DatiConvocazioneScartata())
    '    '--
    '    If Not collConvocazioniScartate Is Nothing AndAlso collConvocazioniScartate.Count > 0 Then
    '        '---
    '        Dim msg As New System.Text.StringBuilder("ATTENZIONE: sono state scartate le seguenti convocazioni:\n")
    '        '---
    '        For i As Integer = 0 To collConvocazioniScartate.Count - 1
    '            '--
    '            msg.AppendFormat("\n--------- Data Convocazione: {0:dd/MM/yyyy} ---------\n", collConvocazioniScartate(i).DataConvocazione)
    '            msg.AppendFormat("Motivo: {0}\n", collConvocazioniScartate(i).DescrizioneMotivo.Replace("'", "\'"))
    '            msg.AppendFormat("Ciclo: {0}\n", collConvocazioniScartate(i).CodiceCiclo, Environment.NewLine)
    '            msg.AppendFormat("Seduta: {0}\n", collConvocazioniScartate(i).NumeroSeduta, Environment.NewLine)
    '            msg.Append("Vaccinazioni da programmare per il ciclo: ")
    '            If collConvocazioniScartate(i).CodiceVaccinazioneList.Count > 0 Then
    '                For j As Integer = 0 To collConvocazioniScartate(i).CodiceVaccinazioneList.Count - 1
    '                    msg.AppendFormat("{0}, ", collConvocazioniScartate(i).CodiceVaccinazioneList(j).ToString())
    '                Next
    '                msg.Remove(msg.Length - 2, 2)
    '            End If
    '            msg.Append("\n----------------------------------------------------------\n")
    '            '--
    '        Next
    '        '---
    '        Me.OnitLayout31.ShowMsgBox(New PagesLayout.OnitLayout3.OnitLayoutMsgBox(msg.ToString(), "CnvScartate", False, False))
    '        '---
    '    End If
    '    '--
    'End Sub

    Private Sub ShowModaleCreaCNV()
        '--
        Me.uscCreaCnv.ModaleName = "modCreaCnv"
        Me.uscCreaCnv.LoadModale()
        '--
        Me.modCreaCnv.VisibileMD = True
        '--
    End Sub

    Private Function ControlliBloccanti(dtPaziente As DataTable, genericProvider As DbGenericProvider) As Boolean
        '--
        Dim messageControlliBloccanti As New System.Text.StringBuilder()
        '--
        Dim statoAnagrafico As Enumerators.StatoAnagrafico = [Enum].Parse(GetType(Enumerators.StatoAnagrafico), dtPaziente.Rows(0)("PAZ_STATO_ANAGRAFICO").ToString())
        Dim dataDecesso As Nullable(Of DateTime)
        '--
        If Not dtPaziente.Rows(0).IsNull("PAZ_DATA_DECESSO") Then dataDecesso = dtPaziente.Rows(0)("PAZ_DATA_DECESSO")
        '--
        ' Controllo paziente DECEDUTO
        '--
        If Me.Settings.VACPROG_BLOCCO_DECEDUTI Then
            '--
            If statoAnagrafico = Enumerators.StatoAnagrafico.DECEDUTO OrElse Not dataDecesso Is Nothing Then
                '--
                ' Cancellazione convocazioni del paziente
                Using bizConvocazione As New BizConvocazione(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), New BizLogOptions(DataLogStructure.TipiArgomento.VAC_PROGRAMMATE, False))

                    Dim now As DateTime = DateTime.Now

                    Dim command As New BizConvocazione.EliminaConvocazioniSollecitiBilanciCommand()
                    command.CodicePaziente = Convert.ToInt64(OnVacUtility.Variabili.PazId)
                    command.DataConvocazione = now
                    command.CancellaBilanciAssociati = True
                    command.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.EliminazioneConvocazione
                    command.DataEliminazione = now
                    command.NoteEliminazione = "Eliminata convocazione paziente deceduto da maschera Convocazioni"
                    command.WriteLog = True

                    bizConvocazione.EliminaConvocazioniSollecitiBilanci(command)

                End Using
                '--
                ' Messaggio all'utente
                If statoAnagrafico = Enumerators.StatoAnagrafico.DECEDUTO Then
                    messageControlliBloccanti.AppendFormat("\nStato Anagrafico: {0}", genericProvider.Paziente.statoAnag(OnVacUtility.Variabili.PazId).ToUpper())
                End If
                '--
                If Not dataDecesso Is Nothing Then
                    messageControlliBloccanti.AppendFormat("\nData Decesso: {0}", dataDecesso.Value.ToString("dd/MM/yyyy"))
                End If
                '--
                messageControlliBloccanti.Append("\n\nLa Programmazione Vaccinale del Paziente è stata eliminata.")
                '--
            End If
            '--
        End If
        '--
        If messageControlliBloccanti.Length > 0 Then
            '--
            messageControlliBloccanti.Insert(0, "ATTENZIONE !!!\n")
            '--
            ' Aggiunta messaggio Consenso (sempre, anche se non è bloccante)
            '--
            messageControlliBloccanti.Append(Me.GetAlertConsenso(genericProvider))
            '--
            Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(messageControlliBloccanti.ToString(), "ControlloDeceduti", False, False))
            '--
            Return False
            '--
        End If
        '--
        Return True
        '--
    End Function

    Private Function GetMessaggioControlliNonBloccanti(dtPaziente As DataTable, bizPaz As BizPaziente) As String

        Dim message As New System.Text.StringBuilder()

        Dim statiAnagraficiAttivi As Hashtable = bizPaz.StatiAnagrafici(True)

        Dim daCompletare As Boolean = dtPaziente.Rows(0)("PAZ_COMPLETARE").ToString() = "S"
        Dim regolarizzato As Boolean = dtPaziente.Rows(0)("PAZ_REGOLARIZZATO").ToString() = "S"
        Dim statoVaccinale As Enumerators.StatiVaccinali = [Enum].Parse(GetType(Enumerators.StatiVaccinali), dtPaziente.Rows(0)("PAZ_STATO").ToString())
        Dim statoAnagrafico As Enumerators.StatoAnagrafico = [Enum].Parse(GetType(Enumerators.StatoAnagrafico), dtPaziente.Rows(0)("PAZ_STATO_ANAGRAFICO").ToString())
        Dim dataDecesso As Nullable(Of DateTime)

        If Not dtPaziente.Rows(0).IsNull("PAZ_DATA_DECESSO") Then dataDecesso = dtPaziente.Rows(0)("PAZ_DATA_DECESSO")

        If daCompletare Then
            message.Append("\nPaziente DA COMPLETARE")
        End If

        If Not statiAnagraficiAttivi.ContainsKey(statoAnagrafico.ToString("d")) Then
            message.AppendFormat("\nStato Anagrafico: {0}", bizPaz.GetDescrizioneStatoAnagraficoPaziente(OnVacUtility.Variabili.PazId).ToUpper())
        End If

        If statoVaccinale = Enumerators.StatiVaccinali.InadempienteTotale Then
            message.AppendFormat("\nStato Vaccinale: INADEMPIENTE TOTALE", statiAnagraficiAttivi(statoAnagrafico))
        End If

        If Not regolarizzato Then
            message.Append("\nPaziente NON REGOLARIZZATO")
        End If

        If Not dataDecesso Is Nothing Then
            message.AppendFormat("\nData Decesso: {0}", dataDecesso.Value.ToString("dd/MM/yyyy"))
        End If

        Return message.ToString()

    End Function

    Private Sub RedirectToGestioneBilanci()

        Me.OnitLayout31.InsertRoutineJS(Me.GetOpenLeftFrameScript(True) + Environment.NewLine)
        Me.Response.Redirect("../../HBilanci/GestioneBilancio/GestioneBilancio.aspx?isCnvRequest=True", True)

    End Sub

    Private Sub RedirectToVisioneBilanci()

        Me.OnitLayout31.InsertRoutineJS(Me.GetOpenLeftFrameScript(True) + Environment.NewLine)
        Me.Response.Redirect("../../HBilanci/VisioneBilanci/VisioneBilanci.aspx?isCnvRequest=True", True)

    End Sub

#Region " Gestione Consenso "

    Private Function GetAlertConsenso(genericProvider As DbGenericProvider) As String

        Dim msgConsenso As String = String.Empty

        If Me.Settings.CONSENSO_GES Then

            Dim pazCodiceAusiliario As String = genericProvider.Paziente.GetCodiceAusiliario(OnVacUtility.Variabili.PazId)
            Dim consenso As Entities.Consenso.StatoConsensoPaziente = Nothing

            ' Lettura stato consenso per il paziente

            consenso = OnVacUtility.GetConsensoGlobalePaziente(pazCodiceAusiliario, Me.Settings)

            If consenso IsNot Nothing AndAlso consenso.Controllo <> Enumerators.ControlloConsenso.NonBloccante AndAlso Not String.IsNullOrEmpty(consenso.DescrizioneStatoConsenso) Then

                msgConsenso = String.Format("\n\nStato Consenso Paziente:\n{0}", consenso.DescrizioneStatoConsenso)

            End If

        End If

        Return msgConsenso

    End Function

#End Region

#End Region

End Class
