Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities


Partial Class VisioneBilanci
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

#Region " Constants "

    Private Const CONFIRM_ERROR_EMPTY_FIELDS As String = "NR"
    Private Const CONFIRM_FIRMA_DIGITALE As String = "FD"

#End Region

#Region " Properties "

    Private Property VisitaSelezionata() As Visita
        Get
            Return ViewState("VisitaSelezionata")
        End Get
        Set(Value As Visita)
            ViewState("VisitaSelezionata") = Value
        End Set
    End Property

    Private Property VisitaSelezionataOld() As Visita
        Get
            Return ViewState("VisitaSelezionataOld")
        End Get
        Set(Value As Visita)
            ViewState("VisitaSelezionataOld") = Value
        End Set
    End Property

    Private _CodiceMedicoUtenteLoggato As String
    Private ReadOnly Property CodiceMedicoUtenteLoggato()
        Get
            If _CodiceMedicoUtenteLoggato Is Nothing Then

                Using genericProvider = New DAL.DbGenericProvider(OnVacContext.Connection)
                    _CodiceMedicoUtenteLoggato = genericProvider.Utenti.GetMedicoDaUtente(OnVacContext.UserId)
                End Using

            End If

            Return _CodiceMedicoUtenteLoggato
        End Get
    End Property

    Private ReadOnly Property UtenteLoggatoIsMedico() As Boolean
        Get
            Return Me.CodiceMedicoUtenteLoggato <> Nothing
        End Get
    End Property

    Private Property SessoPaziente As String
        Get
            Return ViewState("PazSesso")
        End Get
        Set(value As String)
            ViewState("PazSesso") = value
        End Set
    End Property

    Private Property DataNascitaPaziente As DateTime
        Get
            Return ViewState("PazDataNas")
        End Get
        Set(value As DateTime)
            ViewState("PazDataNas") = value
        End Set
    End Property

    Private _DtElencoBilanci As DataTable
    Property DtElencoBilanci() As DataTable
        Get
            Return _DtElencoBilanci
        End Get
        Set(Value As DataTable)
            _DtElencoBilanci = Value
        End Set
    End Property

    Public Property IsVacProgRequest() As Boolean
        Get
            If ViewState("IsVacProgRequest") Is Nothing Then ViewState("IsVacProgRequest") = False
            Return Convert.ToBoolean(ViewState("IsVacProgRequest"))
        End Get
        Set(value As Boolean)
            ViewState("IsVacProgRequest") = value
        End Set
    End Property

    Public Property IsCnvRequest() As Boolean
        Get
            If ViewState("IsCnvRequest") Is Nothing Then ViewState("IsCnvRequest") = False
            Return Convert.ToBoolean(ViewState("IsCnvRequest"))
        End Get
        Set(value As Boolean)
            ViewState("IsCnvRequest") = value
        End Set
    End Property

    Public Property EditCnv() As String
        Get
            Return ViewState("EditCnv")
        End Get
        Set(value As String)
            ViewState("EditCnv") = value
        End Set
    End Property

    Public Property DataCnv() As String
        Get
            Return ViewState("DataCnv")
        End Get
        Set(value As String)
            ViewState("DataCnv") = value
        End Set
    End Property

    Private Property StoricoVaccinaleCentralizzatoDaRecuperare() As String
        Get
            If ViewState("StoricoVaccinaleCentralizzatoDaRecuperare") Is Nothing Then ViewState("StoricoVaccinaleCentralizzatoDaRecuperare") = False
            Return ViewState("StoricoVaccinaleCentralizzatoDaRecuperare")
        End Get
        Set(value As String)
            ViewState("StoricoVaccinaleCentralizzatoDaRecuperare") = value
        End Set
    End Property

    Private Property IsRowVisible() As Boolean
        Get
            If ViewState("IsRowVisible") Is Nothing Then
                ViewState("IsRowVisible") = True
            End If
            Return ViewState("IsRowVisible")
        End Get
        Set(Value As Boolean)
            ViewState("IsRowVisible") = Value
        End Set
    End Property

    Private Property IsRowEditable() As Boolean
        Get
            If ViewState("IsRowEditable") Is Nothing Then
                ViewState("IsRowEditable") = True
            End If
            Return ViewState("IsRowEditable")
        End Get
        Set(Value As Boolean)
            ViewState("IsRowEditable") = Value
        End Set
    End Property

#End Region

#Region " Private "

    Private ControlsEnabled As Boolean = False

#End Region

#Region " Enum "

    Private Enum PageStatus
        EDIT = 0
        SELECTED = 1
        LIST = 2
    End Enum

#End Region

#Region " Page Events "

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '--
        'Il datatable dei bilanci non è in sessione, per cui va caricato ogni volta (è una scelta)
        CaricaTestateBilanci()
        AggiornaUltimaVisitaSelezionata()
        '--
        If Not IsPostBack Then
            '--
            StoricoVaccinaleCentralizzatoDaRecuperare = False
            '--
            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                '--
                OnVacUtility.ImpostaIntestazioniPagina(OnitLayout31, LayoutTitolo, genericProvider, Settings, IsGestioneCentrale)
                '--
                Using bizPaziente As New Biz.BizPaziente(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                    '--
                    Dim datiPaziente As PazienteDatiAnagrafici = bizPaziente.GetDatiAnagraficiPaziente(OnVacUtility.Variabili.PazId, IsGestioneCentrale)
                    If Not datiPaziente Is Nothing Then
                        '--
                        DataNascitaPaziente = datiPaziente.DataNascita.Value
                        SessoPaziente = datiPaziente.Sesso
                        '--
                    End If
                    '--
                End Using
                '--
            End Using
            '--
            ReadQueryStringParameters()
            '--
            If IsGestioneCentrale Then
                '--
                LayoutTitolo_sezione.Text += " [CENTRALE]"
                '--
            Else
                '--
                ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
                If FlagConsensoVaccUslCorrente Then
                    '--
                    Dim statoAcquisizioneDatiVaccinaliCentrale As Enumerators.StatoAcquisizioneDatiVaccinaliCentrale? =
                       Common.OnVacStoricoVaccinaleCentralizzato.GetStatoAcquisizioneDatiVaccinaliCentralePaziente(OnVacUtility.Variabili.PazId)
                    '--
                    If Not statoAcquisizioneDatiVaccinaliCentrale.HasValue OrElse
                        statoAcquisizioneDatiVaccinaliCentrale = Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Parziale Then
                        '--
                        StoricoVaccinaleCentralizzatoDaRecuperare = True
                        '--
                        OnitLayout31.InsertRoutineJS(Common.OnVacStoricoVaccinaleCentralizzato.AlertMessageRecuperoStoricoVaccinale)
                    End If
                    '--
                End If
                '--
            End If
            '--
            GestioneToolbar(PageStatus.LIST)
            '--
            BindControls(PageStatus.LIST)
            '--
            ShowPrintButtons()
            '--
        End If
        '--
    End Sub

#End Region

#Region " Datalist Events "

    Private Sub dlsBilanci_ItemDataBound(sender As Object, e As DataListItemEventArgs) Handles dlsBilanci.ItemDataBound

        ' Gestione visibilità immagine di selezione riga
        Dim imgSeleziona As ImageButton
        Dim btnFollowupRow As ImageButton

        If dlsBilanci.SelectedIndex <> -1 Then
            imgSeleziona = DirectCast(e.Item.FindControl("imgSeleziona"), ImageButton)
            btnFollowupRow = DirectCast(e.Item.FindControl("btnFollowupRow"), ImageButton)
            If Not btnFollowupRow Is Nothing Then
                btnFollowupRow.Enabled = Not ControlsEnabled
                If ControlsEnabled Then
                    btnFollowupRow.ImageUrl = Page.ResolveClientUrl("~/Images/document-edit-dis.png")
                Else
                    btnFollowupRow.ImageUrl = Page.ResolveClientUrl("~/Images/document-edit.png")
                End If

            End If
            If Not imgSeleziona Is Nothing Then imgSeleziona.Visible = Not ControlsEnabled

        End If

        ' Gestione visibilità colonne
        SetDataListColumnVisibility("tdHeaderUslInserimento", e.Item, True)
        SetDataListColumnVisibility("tdItemUslInserimento", e.Item, True)

        SetDataListColumnVisibility("tdHeaderFlagVisibilita", e.Item, True)
        SetDataListColumnVisibility("tdItemFlagVisibilita", e.Item, True)

        SetDataListColumnVisibility("tdHeaderFlagFirma", e.Item, Not IsGestioneCentrale And Settings.FIRMADIGITALE_ANAMNESI_ON)
        SetDataListColumnVisibility("tdItemFlagFirma", e.Item, Not IsGestioneCentrale And Settings.FIRMADIGITALE_ANAMNESI_ON)

        ' Gestione abilitazione campi modificabili
        Dim dtpDataBilancio As Onit.Web.UI.WebControls.Validators.OnitDatePick
        Dim txtFirmaBil As TextBox
        Dim omlMedicoBilancio As Onit.Controls.OnitModalList
        Dim omlRilevatore As Onit.Controls.OnitModalList

        If Me.dlsBilanci.SelectedIndex <> -1 AndAlso e.Item.ItemIndex = Me.dlsBilanci.SelectedIndex Then

            dtpDataBilancio = DirectCast(e.Item.FindControl("dtpDataBilancio"), Onit.Web.UI.WebControls.Validators.OnitDatePick)
            txtFirmaBil = DirectCast(e.Item.FindControl("txtFirmaBil"), TextBox)
            omlMedicoBilancio = DirectCast(e.Item.FindControl("omlMedicoBilancio"), Onit.Controls.OnitModalList)
            omlMedicoBilancio.Filtro = OnVacUtility.GetModalListFilterOperatori(True, False)

            omlRilevatore = DirectCast(e.Item.FindControl("omlRilevatore"), Onit.Controls.OnitModalList)
            omlRilevatore.Filtro = OnVacUtility.GetModalListFilterOperatori(False, False)

            dtpDataBilancio.Enabled = Me.ControlsEnabled
            txtFirmaBil.ReadOnly = Not Me.ControlsEnabled
            omlMedicoBilancio.Enabled = Me.ControlsEnabled
            omlRilevatore.Enabled = Me.ControlsEnabled

            If Not Me.ControlsEnabled Then
                dtpDataBilancio.CssClass = "TextBox_Data_Disabilitato"
                txtFirmaBil.CssClass = "TextBox_Stringa_Disabilitato"
                omlMedicoBilancio.CssClass = "TextBox_Stringa_Disabilitato"
                omlRilevatore.CssClass = "TextBox_Stringa_Disabilitato"
            End If

        Else

            If Not e.Item.FindControl("dtpDataBilancio") Is Nothing AndAlso
               Not e.Item.FindControl("txtFirmaBil") Is Nothing AndAlso
               Not e.Item.FindControl("omlMedicoBilancio") Is Nothing AndAlso
               Not e.Item.FindControl("omlRilevatore") Is Nothing Then

                dtpDataBilancio = DirectCast(e.Item.FindControl("dtpDataBilancio"), Onit.Web.UI.WebControls.Validators.OnitDatePick)
                txtFirmaBil = DirectCast(e.Item.FindControl("txtFirmaBil"), TextBox)
                omlMedicoBilancio = DirectCast(e.Item.FindControl("omlMedicoBilancio"), Onit.Controls.OnitModalList)
                omlRilevatore = DirectCast(e.Item.FindControl("omlRilevatore"), Onit.Controls.OnitModalList)

                dtpDataBilancio.Enabled = False
                txtFirmaBil.ReadOnly = True
                omlMedicoBilancio.Enabled = False
                omlRilevatore.Enabled = False

                dtpDataBilancio.CssClass = "TextBox_Data_Disabilitato"
                txtFirmaBil.CssClass = "TextBox_Stringa_Disabilitato"
                omlMedicoBilancio.CssClass = "TextBox_Stringa_Disabilitato"
                omlRilevatore.CssClass = "TextBox_Stringa_Disabilitato"

            End If

        End If

        ' Gestione flag firma/archiviazione
        Dim btnFlagFirma As ImageButton = DirectCast(e.Item.FindControl("btnFlagFirma"), ImageButton)

        If Not btnFlagFirma Is Nothing Then

            Dim hidUteFirma As HiddenField = DirectCast(e.Item.FindControl("hidUteFirma"), HiddenField)
            Dim hidUteArchiviazione As HiddenField = DirectCast(e.Item.FindControl("hidUteArchiviazione"), HiddenField)

            If Not hidUteFirma Is Nothing AndAlso Not hidUteArchiviazione Is Nothing Then

                Dim flagFirma As String = HttpUtility.HtmlDecode(hidUteFirma.Value)
                Dim flagArchiviazione As String = HttpUtility.HtmlDecode(hidUteArchiviazione.Value)

                If String.IsNullOrEmpty(flagFirma) Then
                    ' NO firma, NO archiviazione
                    btnFlagFirma.Visible = False
                Else
                    ' SI firma
                    btnFlagFirma.Visible = True
                    btnFlagFirma.ImageUrl = OnVacUtility.GetFlagFirmaImageUrlValue(flagFirma, flagArchiviazione, Me.Page)
                    btnFlagFirma.ToolTip = OnVacUtility.GetFlagFirmaToolTipValue(flagFirma, flagArchiviazione)
                End If
            End If
        End If

        'modificabilità del record (all'apertura della maschera)
        If Not IsPostBack Then
            If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim enableEditDelete As Boolean = False
                Dim isPrimaRiga As Boolean = e.Item.ItemIndex = 0
                If isPrimaRiga Then
                    If OnVacContext.CodiceUslCorrente = DirectCast(e.Item.FindControl("lblCodiceUslInserimento"), Label).Text Then
                        IsRowVisible = True
                        IsRowEditable = True
                    Else
                        If CtrlIsInUslUnificata(DirectCast(e.Item.FindControl("lblCodiceUslInserimento"), Label).Text) Then
                            IsRowVisible = True
                            IsRowEditable = True
                        Else
                            IsRowVisible = DirectCast(e.Item.FindControl("HiddenField_FLAGVISIBILITA"), HiddenField).Value = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente
                            IsRowEditable = DirectCast(e.Item.FindControl("HiddenField_FLAGVISIBILITA"), HiddenField).Value = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente
                        End If
                    End If
                End If
            End If
        End If

        'abilitazione del pulsante per visualizzare le note
        Dim imgNote As ImageButton = DirectCast(e.Item.FindControl("imgNote"), ImageButton)

        If Not imgNote Is Nothing Then
            Dim noteEnable As Boolean = True
            If OnVacContext.CodiceUslCorrente <> DirectCast(e.Item.FindControl("lblCodiceUslInserimento"), Label).Text AndAlso Not CtrlIsInUslUnificata(DirectCast(e.Item.FindControl("lblCodiceUslInserimento"), Label).Text) AndAlso DirectCast(e.Item.FindControl("HiddenField_FLAGVISIBILITA"), HiddenField).Value <> Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente Then
                noteEnable = False
            End If
            imgNote.Enabled = noteEnable
        End If

    End Sub

    Private Function CtrlIsInUslUnificata(codiceUsl As String) As Boolean

        'verifico se il codice usl è una vecchia usl che fa parte della nuova ulss
        If Not String.IsNullOrWhiteSpace(codiceUsl) Then
            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizUsl As New Biz.BizUsl(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                    If bizUsl.IsInUslUnificata(codiceUsl, OnVacContext.CodiceUslCorrente) Then
                        Return True
                    End If

                End Using
            End Using
        End If
        Return False

    End Function

    Private Sub SetQuestionarioDetail(isVisible As Boolean)

        ucDatiOpzionaliBilancio.Visible = isVisible
        divPercentili.Visible = isVisible
        Questionario.Visible = isVisible
        Lbl_StatoDetail.Visible = Not isVisible

    End Sub

    Private Sub dlsBilanci_ItemCommand(source As Object, e As System.Web.UI.WebControls.DataListCommandEventArgs) Handles dlsBilanci.ItemCommand

        Select Case e.CommandName

            Case "Seleziona"

                ' Click selezione riga

                If OnVacContext.CodiceUslCorrente = DirectCast(e.Item.FindControl("lblCodiceUslInserimento"), Label).Text Then
                    IsRowVisible = True
                    IsRowEditable = True
                Else
                    If CtrlIsInUslUnificata(DirectCast(e.Item.FindControl("lblCodiceUslInserimento"), Label).Text) Then
                        IsRowVisible = True
                        IsRowEditable = True
                    Else
                        IsRowVisible = DirectCast(e.Item.FindControl("HiddenField_FLAGVISIBILITA"), HiddenField).Value = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente
                        IsRowEditable = DirectCast(e.Item.FindControl("HiddenField_FLAGVISIBILITA"), HiddenField).Value = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente
                    End If
                End If
                SetQuestionarioDetail(IsRowVisible)

                Dim idVisita As Integer = -1

                Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                    Dim txtIdVisita As HiddenField = DirectCast(e.Item.FindControl("txtIdVisita"), HiddenField)
                    Dim lblCodiceUslInserimento As Label = DirectCast(e.Item.FindControl("lblCodiceUslInserimento"), Label)

                    If String.IsNullOrEmpty(txtIdVisita.Value) OrElse Not Integer.TryParse(txtIdVisita.Value, idVisita) Then

                        idVisita = -1

                        OnitLayout31.InsertRoutineJS(String.Format("alert(""{0}"");", "L'id assegnato alla visita non è presente oppure non è un valore valido!"))

                    End If

                    Using bizBilancio As New Biz.BizBilancioProgrammato(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                        If idVisita > 0 Then
                            VisitaSelezionata = bizBilancio.GetVisitaBilancio(idVisita, lblCodiceUslInserimento.Text, IsGestioneCentrale)
                        End If

                    End Using

                End Using

                If idVisita > 0 Then

                    VisitaSelezionataOld = VisitaSelezionata

                    dlsBilanci.SelectedIndex = e.Item.ItemIndex

                    If dlsBilanci.SelectedIndex <> -1 Then
                        GestioneToolbar(PageStatus.SELECTED)
                        BindControls(PageStatus.SELECTED)
                    Else
                        GestioneToolbar(PageStatus.LIST)
                        BindControls(PageStatus.LIST)
                    End If

                End If

            Case "InfoArchiviazione"

                ' Click pulsante flag firma/archiviazione
                Dim idVisitaCorrente As String = DirectCast(e.Item.FindControl("txtIdVisita"), HiddenField).Value

                If String.IsNullOrWhiteSpace(idVisitaCorrente) Then
                    Me.OnitLayout31.InsertRoutineJS(String.Format("alert(""{0}"");", "Nessuna visita selezionata."))
                Else
                    Dim codiceUslInserimento As String = DirectCast(e.Item.FindControl("lblCodiceUslInserimento"), Label).Text

                    Me.ucInfoFirma.SetInfoFirmaDigitaleArchiviazioneSostitutiva(Convert.ToInt64(idVisitaCorrente), codiceUslInserimento)
                    Me.fmInfoArchiviazione.VisibileMD = True
                End If

            Case "InfoNote"

                Dim idVisita As Integer = -1
                Dim visita As New Visita()

                Dim txtIdVisita As HiddenField = DirectCast(e.Item.FindControl("txtIdVisita"), HiddenField)
                Dim lblCodiceUslInserimento As Label = DirectCast(e.Item.FindControl("lblCodiceUslInserimento"), Label)

                If String.IsNullOrEmpty(txtIdVisita.Value) OrElse Not Integer.TryParse(txtIdVisita.Value, idVisita) Then

                    idVisita = -1
                    Me.OnitLayout31.InsertRoutineJS(String.Format("alert(""{0}"");", "L'id assegnato alla visita non è presente oppure non è un valore valido!"))

                End If

                Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                    Using bizBilancio As New Biz.BizBilancioProgrammato(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                        If idVisita > 0 Then
                            visita = bizBilancio.GetVisitaBilancio(idVisita, lblCodiceUslInserimento.Text, Me.IsGestioneCentrale)
                        End If

                    End Using
                End Using

                If Not visita.Note.IsNullOrEmpty Then
                    tbNote.Text = visita.Note
                Else
                    tbNote.Text = ""
                End If

                fmInfoNote.VisibileMD = True
            Case "GoFollowUp"
                Dim idVisita As Integer = -1
                Dim visita As New Visita()

                Dim hidFollowup As HiddenField = DirectCast(e.Item.FindControl("hidFollowup"), HiddenField)
                Dim lblCodiceUslInserimento As Label = DirectCast(e.Item.FindControl("lblCodiceUslInserimento"), Label)
                ' se esiste un follow up calcolo la vista da sosotituire con quella figlia
                If Not String.IsNullOrEmpty(hidFollowup.Value) AndAlso Integer.TryParse(hidFollowup.Value, idVisita) Then
                    Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                        Using bizBilancio As New Biz.BizBilancioProgrammato(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                            If idVisita > 0 Then
                                visita = bizBilancio.GetVisitaBilancio(idVisita, lblCodiceUslInserimento.Text, Me.IsGestioneCentrale)
                            End If

                        End Using
                    End Using
                End If
                ' se ho una visita
                If idVisita > 0 Then
                    ' modifico dati della visita
                    VisitaSelezionataOld = VisitaSelezionata
                    VisitaSelezionata = visita
                    'Mi calcolo posizione della nuova visita
                    For Each item As DataListItem In dlsBilanci.Items
                            If CType(item.FindControl("txtIdVisita"), HiddenField).Value = VisitaSelezionata.IdVisita.ToString() Then
                                dlsBilanci.SelectedIndex = item.ItemIndex
                                Exit For
                            End If
                        Next
                    ' setto pagina
                    If dlsBilanci.SelectedIndex <> -1 Then
                        GestioneToolbar(PageStatus.SELECTED)
                        BindControls(PageStatus.SELECTED)
                    Else
                        GestioneToolbar(PageStatus.LIST)
                        BindControls(PageStatus.LIST)
                    End If

                End If

        End Select

    End Sub

#End Region

#Region " Toolbar Events "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btnIndietro"

                If IsVacProgRequest Then
                    Response.Redirect(String.Format("../../HPazienti/VacProg/VacProg.aspx?DataCnv={0}&EditCnv={1}", HttpUtility.UrlEncode(DataCnv), HttpUtility.UrlEncode(EditCnv)))
                ElseIf IsCnvRequest Then
                    RedirectToConvocazioniPaziente(OnVacUtility.Variabili.PazId)
                End If

            Case "btnModifica"

                ' Se l'anamnesi ha già una firma digitale, non può essere modificata
                If GetFlagFirmaDigitaleVisitaSelezionata() Then
                    OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(Biz.BizFirmaDigitale.Messages.NO_EDIT_ANAMNESI_FIRMATA, "errFirma", False, False))
                    Return
                End If

                ' Controlli sui dati della visita per impostare il layout in modifica
                Dim result As ControlloVisitaResult = ControlloVisitaSelezionata(True)

                If result.SetLayout Then
                    BindControls(result.Status)
                    GestioneToolbar(result.Status)
                End If

                ' Se controllo fallito, messaggio all'utente
                If Not result.Success Then
                    OnitLayout31.InsertRoutineJS(result.Message)
                End If

            Case "btnElimina"

                ' N.B. : confirm firma digitale lato client => da non ripetere qui se no viene chiesto due volte

                ' Controlli sui dati della visita per impostare il layout in modifica
                Dim result As ControlloVisitaResult = ControlloVisitaSelezionata(True)

                ' Se controllo fallito, messaggio all'utente
                If Not result.Success Then
                    OnitLayout31.InsertRoutineJS(result.Message)
                Else
                    EliminaBilancioSelezionato()
                End If

            Case "btnSalva"

                If CheckQuestionario() Then
                    Salva()
                End If

            Case "btnAnnulla"

                AnnullaBilancio()

            Case "btnModificaConsenso"

                SetVisibilitaDatiVaccinali()

            Case "btnStampa"

                Dim command As New CommonBilancio.StampaBilancioCommand()
                command.CodicePaziente = OnVacUtility.Variabili.PazId
                command.IdVisita = VisitaSelezionata.IdVisita
                command.IsCentrale = IsGestioneCentrale
                command.NumeroBilancio = VisitaSelezionata.BilancioNumero
                command.CodiceMalattia = VisitaSelezionata.MalattiaCodice
                command.Page = Me.Page
                command.Settings = Settings

                Dim result As CommonBilancio.StampaBilancioResult = CommonBilancio.StampaBilancio(command)

                If Not result.Success Then
                    OnitLayout31.InsertRoutineJS(String.Format("alert('{0}')", HttpUtility.JavaScriptStringEncode(result.Message)))
                End If
            Case "btnFollowUp"
                CreaFollowUp()

            Case "btnRecuperaStoricoVacc"

                RecuperaStoricoVaccinale()

            Case "btnFirma"

                ' N.B. : in configurazione centrale (IsGestioneCentrale = True), la funzionalità di firma non è presente (tutta la toolbar viene nascosta)

                ' Se l'anamnesi ha già una firma digitale, non può essere modificata
                If GetFlagFirmaDigitaleVisitaSelezionata() Then
                    OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(Biz.BizFirmaDigitale.Messages.NO_EDIT_ANAMNESI_FIRMATA, "errFirma", False, False))
                    Return
                End If

                ' Se fallisce il controllo sulla usl di inserimento, l'anamnesi non può essere modificata nè firmata.
                Dim result As ControlloVisitaResult = ControlloVisitaSelezionata(False)

                If Not result.Success Then
                    ' Controlli falliti => Firma digitale non possibile
                    OnitLayout31.InsertRoutineJS(result.Message)
                Else
                    ' Controlli ok => richiesta di conferma
                    SwitchViewFirmaDigitale()
                End If

        End Select

    End Sub

#End Region

#Region " OnitLayout Events "

    Private Sub OnitLayout31_ConfirmClick(sender As Object, e As Onit.Controls.PagesLayout.OnitLayout3.ConfirmEventArgs) Handles OnitLayout31.ConfirmClick

        Select Case e.Key

            Case CONFIRM_ERROR_EMPTY_FIELDS

                If e.Result Then Me.Salva()

        End Select

    End Sub

#End Region

#Region " Public Methods "

    Public Function GetStringEta(cont As Object, campo As String) As String

        Dim eta As New Entities.Eta(cont.Item(campo))

        Return eta.Anni & " aa " & eta.Mesi & " mm<br/>" & eta.Giorni & " gg"

    End Function

#End Region

#Region " Private Methods "

    Private Sub ReadQueryStringParameters()

        ' Controllo se la richiesta di compilazione arriva dalle vaccinazioni programmate
        IsVacProgRequest = GetBooleanValueFromQueryString("IsVacProgRequest")

        ' Controllo se la richiesta di compilazione arriva dalle convocazioni
        IsCnvRequest = GetBooleanValueFromQueryString("IsCnvRequest")

        If Not Request.QueryString("EditCnv") Is Nothing Then
            EditCnv = Request.QueryString("EditCnv")
        End If

        If Not Request.QueryString("DataCnv") Is Nothing Then
            DataCnv = Request.QueryString("DataCnv")
        End If

    End Sub

    Private Function GetBooleanValueFromQueryString(parameterName As String) As Boolean

        Dim value As Boolean = False

        If Not String.IsNullOrWhiteSpace(Request.QueryString.Get(parameterName)) Then

            If Not Boolean.TryParse(Request.QueryString(parameterName), value) Then
                value = False
            End If

        End If

        Return value

    End Function

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of PrintButton)()
        listPrintButtons.Add(New PrintButton(Constants.ReportName.AnamnesiDefault, "btnStampa"))

        ShowToolbarPrintButtons(listPrintButtons, ToolBar)

    End Sub

    ' Aggiorno i dati della visita selezionata dall'utente
    Private Sub AggiornaUltimaVisitaSelezionata()

        If dlsBilanci.SelectedIndex <> -1 Then

            VisitaSelezionata.IdVisita = DirectCast(dlsBilanci.SelectedItem.FindControl("txtIdVisita"), HiddenField).Value

            'VisitaSelezionata.CodicePaziente = DirectCast(dlsBilanci.SelectedItem.FindControl("hdCodicePaziente"), HiddenField).Value
            If String.IsNullOrWhiteSpace(DirectCast(dlsBilanci.SelectedItem.FindControl("hdCodicePaziente"), HiddenField).Value) Then
                VisitaSelezionata.CodicePaziente = OnVacUtility.Variabili.PazId
            Else
                VisitaSelezionata.CodicePaziente = DirectCast(dlsBilanci.SelectedItem.FindControl("hdCodicePaziente"), HiddenField).Value
            End If

            VisitaSelezionata.DataVisita = DirectCast(dlsBilanci.SelectedItem.FindControl("dtpDataBilancio"), Onit.Web.UI.WebControls.Validators.OnitDatePick).Data
            VisitaSelezionata.MalattiaCodice = DirectCast(dlsBilanci.SelectedItem.FindControl("lblMalattia"), Label).Text
            VisitaSelezionata.MalattiaCodFollowUp = DirectCast(dlsBilanci.SelectedItem.FindControl("HiddenMalCodFollowUp"), HiddenField).Value

            Dim omlMedicoBilancio As Onit.Controls.OnitModalList = DirectCast(dlsBilanci.SelectedItem.FindControl("omlMedicoBilancio"), Onit.Controls.OnitModalList)
            VisitaSelezionata.MedicoCodice = omlMedicoBilancio.Codice
            VisitaSelezionata.MedicoDescrizione = omlMedicoBilancio.Descrizione

            Dim omlRilevatore As Onit.Controls.OnitModalList = DirectCast(dlsBilanci.SelectedItem.FindControl("omlRilevatore"), Onit.Controls.OnitModalList)
            VisitaSelezionata.RilevatoreCodice = omlRilevatore.Codice
            VisitaSelezionata.RilevatoreDescrizione = omlRilevatore.Descrizione

            VisitaSelezionata.Peso = GetDoubleValue(txtPeso.Text.Replace(".", ","))
            VisitaSelezionata.Altezza = GetDoubleValue(txtAltezza.Text.Replace(".", ","))
            VisitaSelezionata.Cranio = GetDoubleValue(txtCranio.Text.Replace(".", ","))
            VisitaSelezionata.Firma = DirectCast(dlsBilanci.SelectedItem.FindControl("txtFirmaBil"), TextBox).Text

            VisitaSelezionata.CodiceUslInserimento = DirectCast(dlsBilanci.SelectedItem.FindControl("lblCodiceUslInserimento"), Label).Text
            VisitaSelezionata.FlagVisibilitaDatiVaccinaliCentrale = DirectCast(dlsBilanci.SelectedItem.FindControl("lblFlagVisibilita"), Label).Text

            Dim datiViaggioCorrente As DatiOpzionaliBilancio.DatiViaggio = ucDatiOpzionaliBilancio.GetDatiViaggio()
            VisitaSelezionata.DataInizioViaggio = datiViaggioCorrente.DataInizioViaggio
            VisitaSelezionata.DataFineViaggio = datiViaggioCorrente.DataFineViaggio
            VisitaSelezionata.PaeseViaggioCodice = datiViaggioCorrente.PaeseViaggioCodice
            VisitaSelezionata.PaeseViaggioDescrizione = datiViaggioCorrente.PaeseViaggioDescrizione

            VisitaSelezionata.VaccinazioniBilancio = ucDatiOpzionaliBilancio.GetDatiVaccinazioni()

        End If

    End Sub

    Private Function GetDoubleValue(value As String)

        Dim doubleValue As Double = 0

        If Not Double.TryParse(value, doubleValue) Then
            doubleValue = 0
        End If

        Return doubleValue

    End Function

    Private Sub CaricaTestateBilanci()

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizBilancio As New Biz.BizBilancioProgrammato(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                DtElencoBilanci = bizBilancio.GetDtVisiteBilanci(OnVacUtility.Variabili.PazId, IsGestioneCentrale)

            End Using
        End Using

    End Sub

    Private Sub GeneraQuestionario()

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Questionario.IdVisita = VisitaSelezionata.IdVisita

            Questionario.CodicePaziente = VisitaSelezionata.CodicePaziente
            Questionario.CodiceMalattia = VisitaSelezionata.MalattiaCodice
            Questionario.DataVisita = VisitaSelezionata.DataVisita
            Questionario.DataRegistrazione = VisitaSelezionata.DataRegistrazione

            Questionario.NumeroBilancio = VisitaSelezionata.BilancioNumero.Value
            Questionario.Compilazione = Enumerators.TipoCompilazioneBilancio.Compilata

            Using bizBilancioProgrammato As New Biz.BizBilancioProgrammato(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Dim datiBilancioPazienteResult As Biz.BizBilancioProgrammato.DatiBilancioPazienteResult =
                    bizBilancioProgrammato.GetDatiBilancioPaziente(VisitaSelezionata.IdVisita, VisitaSelezionata.CodiceUslInserimento, OnVacUtility.Variabili.PazId,
                                                                   VisitaSelezionata.BilancioNumero.Value, VisitaSelezionata.MalattiaCodice, SessoPaziente,
                                                                   VisitaSelezionata.DataVisita, IsGestioneCentrale, VisitaSelezionata.DataRegistrazione)

                Questionario.DtSezioni = datiBilancioPazienteResult.Sezioni
                Questionario.DtCondizioniBilancio = datiBilancioPazienteResult.CondizioniBilancio
                Questionario.DtRispostePossibili = datiBilancioPazienteResult.RispostePossibili
                Questionario.DtDomande = datiBilancioPazienteResult.Domande
                Questionario.DtRisposte = datiBilancioPazienteResult.Risposte

            End Using
        End Using

    End Sub

    Private Function Salva() As Boolean

        Dim success As Boolean = True

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, OnVacUtility.GetReadCommittedTransactionOptions())

            Using dbGenericProviderFactory As New Biz.DbGenericProviderFactory()

                Dim genericProvider As DAL.DbGenericProvider = dbGenericProviderFactory.GetDbGenericProvider(OnVacContext.AppId, OnVacContext.Azienda)

                Dim dataNascita As DateTime = genericProvider.Paziente.GetDataNascita(OnVacUtility.Variabili.PazId)

                If VisitaSelezionata.DataVisita < dataNascita Then

                    ' Data visita antecedente nascita
                    OnitLayout31.InsertRoutineJS("alert('Salvataggio non effettuato.\nLa data della visita non puo\' essere antecedente alla data di nascita del paziente.')")
                    success = False

                    'ElseIf VisitaSelezionata.DataInizioViaggio.HasValue AndAlso VisitaSelezionata.DataFineViaggio.HasValue AndAlso VisitaSelezionata.DataFineViaggio < VisitaSelezionata.DataInizioViaggio Then

                    '    ' Data inizio viaggio successiva a data fine
                    '    OnitLayout31.InsertRoutineJS("alert('Salvataggio non effettuato.\nLa data di inizio del viaggio non puo\' essere successiva alla data di fine.')")
                    '    success = False

                Else
                    ' verifico dati del viaggio
                    Dim resultViaggi As Biz.BizGenericResult = ucDatiOpzionaliBilancio.CheckViaggi()
                    If Not resultViaggi.Success Then
                        OnitLayout31.InsertRoutineJS(String.Format("alert('{0}')", resultViaggi.Message))
                        success = False
                    Else
                        Dim result As Biz.BizGenericResult = ucDatiOpzionaliBilancio.CheckDosiVaccinazioniSelezionate()

                        If Not result.Success Then

                            OnitLayout31.InsertRoutineJS(String.Format("alert('Salvataggio non effettuato.\n{0}')", result.Message))
                            success = False

                        Else

                            ' Calcolo Percentili
                            Using bizBilancio As New Biz.BizBilancioProgrammato(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                                VisitaSelezionata.PercentilePeso =
                                    bizBilancio.CalcolaPercentile(Enumerators.TipoPercentile.Peso, VisitaSelezionata.Peso, DataNascitaPaziente, SessoPaziente)

                                VisitaSelezionata.PercentileAltezza =
                                    bizBilancio.CalcolaPercentile(Enumerators.TipoPercentile.Altezza, VisitaSelezionata.Altezza, DataNascitaPaziente, SessoPaziente)

                                VisitaSelezionata.PercentileCranio =
                                    bizBilancio.CalcolaPercentile(Enumerators.TipoPercentile.Cranio, VisitaSelezionata.Cranio, DataNascitaPaziente, SessoPaziente)

                            End Using

                            ' Osservazioni
                            Dim osservazioniModificate As Osservazione() = Questionario.GetOsservazioni(genericProvider).ToArray()
                            Dim listaViaggi As List(Of ViaggioVisita) = ucDatiOpzionaliBilancio.GetListaDatiViaggio()
                            If Not listaViaggi Is Nothing Then
                                VisitaSelezionata.DataFollowUpPrevisto = ucDatiOpzionaliBilancio.GetDataFollowUpPrevista()
                                VisitaSelezionata.DataFollowUpEffettivo = ucDatiOpzionaliBilancio.GetDataFollowUpEffetiva()
                            End If


                            ' Salvataggio
                            Using bizVisite As New Biz.BizVisite(dbGenericProviderFactory, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                                Dim updateVisitaCommand As New Biz.BizVisite.ModificaVisitaCommand()
                                updateVisitaCommand.Visita = VisitaSelezionata
                                updateVisitaCommand.VisitaOriginale = VisitaSelezionataOld
                                updateVisitaCommand.Osservazioni = osservazioniModificate
                                updateVisitaCommand.Note = "Update visita da maschera VisioneBilanci"
                                updateVisitaCommand.SovrascriviInfoModifica = True
                                updateVisitaCommand.ListaViaggi = listaViaggi

                                'Modifica visita
                                Dim updateVisitaBizResult As Biz.BizVisite.ModificaVisitaBizResult = bizVisite.UpdateVisita(updateVisitaCommand, True)

                                'LOG
                                Dim testataLog As New Testata(TipiArgomento.GEST_BIL, Operazione.Modifica, OnVacUtility.Variabili.PazId, False)

                                If updateVisitaBizResult.Success Then

                                    ' Impostazione del rilevatore
                                    If Settings.BILANCI_PREVALORIZZA_OPERATORI Then

                                        If Not String.IsNullOrWhiteSpace(VisitaSelezionata.RilevatoreCodice) Then

                                            ' Memorizzo il rilevatore nelle OnVacUtility solo se è stato modificato (rispetto alla visita originale)
                                            If VisitaSelezionataOld.RilevatoreCodice <> VisitaSelezionata.RilevatoreCodice Then
                                                OnVacUtility.Variabili.MedicoRilevatore.Codice = VisitaSelezionata.RilevatoreCodice
                                                OnVacUtility.Variabili.MedicoRilevatore.Nome = VisitaSelezionata.RilevatoreDescrizione
                                            End If

                                        Else
                                            ' N.B. : se il rilevatore viene lasciato vuoto, non lo sbianco nelle OnVacUtility perchè in gestione bilanci è obbligatorio.
                                        End If

                                    End If

                                    Dim recordLog As New DataLogStructure.Record()
                                    recordLog.Campi.Add(New Campo("Modificata visita (VIS_ID):", updateVisitaCommand.Visita.IdVisita))
                                    If updateVisitaCommand.Visita.DataVisita <> VisitaSelezionataOld.DataVisita Then recordLog.Campi.Add(New Campo("VIS_DATA_VISITA", VisitaSelezionataOld.DataVisita, updateVisitaCommand.Visita.DataVisita))
                                    If updateVisitaCommand.Visita.MedicoCodice <> VisitaSelezionataOld.MedicoCodice Then recordLog.Campi.Add(New Campo("VIS_OPE_CODICE", VisitaSelezionataOld.MedicoCodice, updateVisitaCommand.Visita.MedicoCodice))
                                    If updateVisitaCommand.Visita.Firma <> VisitaSelezionataOld.Firma Then recordLog.Campi.Add(New Campo("VIS_FIRMA", VisitaSelezionataOld.Firma, updateVisitaCommand.Visita.Firma))

                                    testataLog.Records.Add(recordLog)

                                End If

                                For Each logRecord As DataLogStructure.Record In updateVisitaBizResult.OsservazioniLogRecords
                                    testataLog.Records.Add(logRecord)
                                Next

                                If testataLog.Records.Count > 0 Then
                                    LogBox.WriteData(testataLog)
                                End If

                                If Not updateVisitaBizResult.Success Then

                                    ' Se la data della visita è già presente, non vado avanti col salvataggio.
                                    OnitLayout31.InsertRoutineJS(String.Format("alert('Salvataggio non effettuato.\n{0}');", updateVisitaBizResult.Messages.ToJavascriptString()))
                                    success = False

                                Else

                                    If updateVisitaBizResult.ConvocazioniDeleted Then OnitLayout31.InsertRoutineJS("alert('Eliminazione effettuata!')")

                                End If

                            End Using

                            If success Then

                                ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
                                If FlagAbilitazioneVaccUslCorrente Then

                                    Using bizPaziente As Biz.BizPaziente = New Biz.BizPaziente(dbGenericProviderFactory, Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.GEST_BIL, True))

                                        Dim aggiornaDatiVaccinaliCentraliCommand As New Biz.BizPaziente.AggiornaDatiVaccinaliCentraliCommand()
                                        aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale = bizPaziente.GenericProvider.Paziente.GetCodiceAusiliario(OnVacUtility.Variabili.PazId)
                                        aggiornaDatiVaccinaliCentraliCommand.VisitaEnumerable = {VisitaSelezionata}
                                        aggiornaDatiVaccinaliCentraliCommand.OsservazioneEnumerable = osservazioniModificate.AsEnumerable()

                                        bizPaziente.AggiornaDatiVaccinaliCentrali(aggiornaDatiVaccinaliCentraliCommand)

                                    End Using

                                End If
                            End If
                        End If
                    End If
                End If

            End Using

            transactionScope.Complete()

        End Using

        If success Then

            VisitaSelezionataOld = VisitaSelezionata

            CaricaTestateBilanci()

            Dim riga As DataRow() = DtElencoBilanci.Select("VIS_ID='" & VisitaSelezionataOld.IdVisita.ToString() & "'")
            If VisitaSelezionataOld Is Nothing OrElse riga.Length = 0 Then
                dlsBilanci.SelectedIndex = -1
                Questionario.DtSezioni = Nothing
            End If

            If dlsBilanci.SelectedIndex <> -1 Then
                GestioneToolbar(PageStatus.SELECTED)
                BindControls(PageStatus.SELECTED)
            Else
                GestioneToolbar(PageStatus.LIST)
                BindControls(PageStatus.LIST)
            End If

            If dlsBilanci.SelectedIndex <> -1 Then
                For Each item As DataListItem In dlsBilanci.Items
                    If CType(item.FindControl("txtIdVisita"), HiddenField).Value = VisitaSelezionataOld.IdVisita.ToString() Then
                        dlsBilanci.SelectedIndex = item.ItemIndex
                        Exit For
                    End If
                Next
            End If

        End If

        Return success

    End Function

    Private Sub BindControls(status As PageStatus)

        Select Case status

            Case PageStatus.EDIT

                SetLayoutSezioni(True)

            Case PageStatus.SELECTED

                SetLayoutSezioni(False)

            Case PageStatus.LIST

                divPercentili.Visible = False
                ucDatiOpzionaliBilancio.Visible = False

                ControlsEnabled = False

                txtPeso.ReadOnly = True
                txtPeso.CssClass = "TextBox_Numerico"

                txtAltezza.ReadOnly = True
                txtAltezza.CssClass = "TextBox_Numerico"

                txtCranio.ReadOnly = True
                txtCranio.CssClass = "TextBox_Numerico"

                Questionario.Enabled = False

        End Select

        Questionario.DataBind()

        ' Bind datalist
        dlsBilanci.DataSource = DtElencoBilanci
        dlsBilanci.DataBind()

        OnitLayout31.Busy = ControlsEnabled OrElse IsVacProgRequest

    End Sub

    Private Sub SetLayoutSezioni(inEdit As Boolean)

        If Not VisitaSelezionata.BilancioNumero.HasValue OrElse String.IsNullOrWhiteSpace(Me.VisitaSelezionata.MalattiaCodice) Then
            Throw New ArgumentException("Non è possibile continuare: tra i dati della visita selezionata non sono presenti il numero del bilancio e/o il codice della malattia")
        End If

        ' Recupero anagrafica bilancio relativo alla visita selezionata
        Dim bilancioSelezionato As BilancioAnagrafica =
            CommonBilancio.GetBilancioAnagrafica(VisitaSelezionata.BilancioNumero.Value, VisitaSelezionata.MalattiaCodice, Settings)
        Dim viaggio As New List(Of ViaggioVisita)
        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizviaggi As New Biz.BizVisite(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                viaggio = bizviaggi.GetListaViaggiVisita(VisitaSelezionata.IdVisita)

            End Using
        End Using




        divPercentili.Visible = bilancioSelezionato.GestioneCranio OrElse bilancioSelezionato.GestioneAltezza OrElse bilancioSelezionato.GestionePeso

        ControlsEnabled = inEdit

        If bilancioSelezionato.GestioneCranio Then
            If VisitaSelezionata.Cranio <> 0 Then
                txtCranio.Text = VisitaSelezionata.Cranio
            Else
                txtCranio.Text = String.Empty
            End If
            lblCranio.Visible = True
            lblPercentileCranio.Visible = True
            txtCranio.Visible = True
            txtCranio.ReadOnly = Not inEdit
            txtCranio.CssClass = IIf(inEdit, "TextBox_Numerico", "TextBox_Numerico_Disabilitato")
            txtPercentileCranio.Visible = True
            txtPercentileCranio.Text = VisitaSelezionata.PercentileCranio
        Else
            lblCranio.Visible = False
            lblPercentileCranio.Visible = False
            txtCranio.Visible = False
            txtCranio.Text = String.Empty
            txtPercentileCranio.Visible = False
            txtPercentileCranio.Text = String.Empty
        End If

        If bilancioSelezionato.GestioneAltezza Then
            If VisitaSelezionata.Altezza <> 0 Then
                txtAltezza.Text = VisitaSelezionata.Altezza
            Else
                txtAltezza.Text = String.Empty
            End If
            lblAltezza.Visible = True
            lblPercentileAltezza.Visible = True
            txtAltezza.Visible = True
            txtAltezza.ReadOnly = Not inEdit
            txtAltezza.CssClass = IIf(inEdit, "TextBox_Numerico", "TextBox_Numerico_Disabilitato")
            txtPercentileAltezza.Visible = True
            txtPercentileAltezza.Text = VisitaSelezionata.PercentileAltezza
        Else
            lblAltezza.Visible = False
            lblPercentileAltezza.Visible = False
            txtAltezza.Visible = False
            txtAltezza.Text = String.Empty
            txtPercentileAltezza.Visible = False
            txtPercentileAltezza.Text = String.Empty
        End If

        If bilancioSelezionato.GestionePeso Then
            If VisitaSelezionata.Peso <> 0 Then
                txtPeso.Text = VisitaSelezionata.Peso
            Else
                txtPeso.Text = String.Empty
            End If
            lblPeso.Visible = True
            lblPercentilePeso.Visible = True
            txtPeso.Visible = True
            txtPeso.ReadOnly = Not inEdit
            txtPeso.CssClass = IIf(inEdit, "TextBox_Numerico", "TextBox_Numerico_Disabilitato")
            txtPercentilePeso.Visible = True
            txtPercentilePeso.Text = VisitaSelezionata.PercentilePeso
        Else
            lblPeso.Visible = False
            lblPercentilePeso.Visible = False
            txtPeso.Visible = False
            txtPeso.Text = String.Empty
            txtPercentilePeso.Visible = False
            txtPercentilePeso.Text = String.Empty
        End If

        ucDatiOpzionaliBilancio.Enabled = inEdit

        '' If Not inEdit Then

        Dim datiOpzionali As New DatiOpzionaliBilancio.DatiOpzionaliSettings()

            datiOpzionali.ShowDatiVaccinazioni = bilancioSelezionato.GestioneVaccinazioni AndAlso IsRowVisible
            datiOpzionali.ShowDatiViaggio = bilancioSelezionato.GestioneViaggi AndAlso IsRowVisible

            datiOpzionali.DatiViaggioCorrente.DataInizioViaggio = VisitaSelezionata.DataInizioViaggio
            datiOpzionali.DatiViaggioCorrente.DataFineViaggio = VisitaSelezionata.DataFineViaggio
            datiOpzionali.DatiViaggioCorrente.PaeseViaggioCodice = VisitaSelezionata.PaeseViaggioCodice
        datiOpzionali.DatiViaggioCorrente.PaeseViaggioDescrizione = VisitaSelezionata.PaeseViaggioDescrizione
        datiOpzionali.DataFollowUpEffettiva = VisitaSelezionata.DataFollowUpEffettivo
        datiOpzionali.DataFollowUpPrevista = VisitaSelezionata.DataFollowUpPrevisto
        Dim vis As Boolean = False
        If VisitaSelezionata.DataFollowUpEffettivo.HasValue AndAlso VisitaSelezionata.DataFollowUpEffettivo > Date.MinValue Then
            vis = True
        End If
        datiOpzionali.VisibleDataEffettiva = vis


        datiOpzionali.DatiVaccinazioniBilancio = VisitaSelezionata.VaccinazioniBilancio
        datiOpzionali.DatiViaggi = viaggio

            ucDatiOpzionaliBilancio.SetDatiOpzionali(datiOpzionali)

        ''End If

        Questionario.Enabled = inEdit

        GeneraQuestionario()

    End Sub

    Private Sub GestioneToolbar(pageStatus As PageStatus)

        If IsGestioneCentrale Then

            ToolBar.Visible = False

        Else

            Dim btnIndietro As Infragistics.WebUI.UltraWebToolbar.TBarButton = ToolBar.Items.FromKeyButton("btnIndietro")
            Dim btnModifica As Infragistics.WebUI.UltraWebToolbar.TBarButton = ToolBar.Items.FromKeyButton("btnModifica")
            Dim btnSalva As Infragistics.WebUI.UltraWebToolbar.TBarButton = ToolBar.Items.FromKeyButton("btnSalva")
            Dim btnAnnulla As Infragistics.WebUI.UltraWebToolbar.TBarButton = ToolBar.Items.FromKeyButton("btnAnnulla")
            Dim btnElimina As Infragistics.WebUI.UltraWebToolbar.TBarButton = ToolBar.Items.FromKeyButton("btnElimina")
            Dim btnModificaConsenso As Infragistics.WebUI.UltraWebToolbar.TBarButton = ToolBar.Items.FromKeyButton("btnModificaConsenso")
            Dim btnStampa As Infragistics.WebUI.UltraWebToolbar.TBarButton = ToolBar.Items.FromKeyButton("btnStampa")
            Dim btnFollowUp As Infragistics.WebUI.UltraWebToolbar.TBarButton = ToolBar.Items.FromKeyButton("btnFollowUp")
            Dim btnRecuperaStoricoVacc As Infragistics.WebUI.UltraWebToolbar.TBarButton = ToolBar.Items.FromKeyButton("btnRecuperaStoricoVacc")
            Dim btnFirma As Infragistics.WebUI.UltraWebToolbar.TBarButton = ToolBar.Items.FromKeyButton("btnFirma")

            ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
            btnRecuperaStoricoVacc.Visible = FlagConsensoVaccUslCorrente

            btnModificaConsenso.Visible = True
            btnIndietro.Visible = IsVacProgRequest Or IsCnvRequest

            btnFirma.Visible = Settings.FIRMADIGITALE_ANAMNESI_ON
            If Not Settings.FIRMADIGITALE_ANAMNESI_ON Then
                ToolBar.Items.FromKeySeparator("sepFirma").Image = ResolveClientUrl("~/images/transparent16.gif")
            End If

            If StoricoVaccinaleCentralizzatoDaRecuperare Then

                btnIndietro.Enabled = True
                btnModifica.Enabled = False
                btnSalva.Enabled = False
                btnAnnulla.Enabled = False
                btnElimina.Enabled = False
                btnModificaConsenso.Enabled = False
                btnStampa.Enabled = (dlsBilanci.SelectedIndex > -1)
                btnFollowUp.Enabled = False
                btnRecuperaStoricoVacc.Enabled = True
                btnFirma.Enabled = False

            Else

                btnRecuperaStoricoVacc.Enabled = False

                Select Case pageStatus

                    Case PageStatus.EDIT
                        btnIndietro.Enabled = False
                        btnModifica.Enabled = False
                        btnSalva.Enabled = True
                        btnAnnulla.Enabled = True
                        btnElimina.Enabled = False
                        btnModificaConsenso.Enabled = False
                        btnStampa.Enabled = False
                        btnFollowUp.Enabled = False
                        btnFirma.Enabled = False

                    Case PageStatus.LIST
                        btnIndietro.Enabled = True
                        btnModifica.Enabled = False
                        btnSalva.Enabled = False
                        btnAnnulla.Enabled = False
                        btnElimina.Enabled = False
                        btnModificaConsenso.Enabled = False
                        btnStampa.Enabled = False
                        btnFollowUp.Enabled = False
                        btnFirma.Enabled = False

                    Case PageStatus.SELECTED
                        btnIndietro.Enabled = True
                        btnModifica.Enabled = IsRowVisible AndAlso IsRowEditable
                        btnSalva.Enabled = False
                        btnAnnulla.Enabled = False
                        btnElimina.Enabled = Not UtenteLoggatoIsMedico AndAlso IsRowVisible AndAlso IsRowEditable
                        btnModificaConsenso.Enabled = IsRowVisible AndAlso IsRowEditable
                        btnStampa.Enabled = IsRowVisible
                        btnFollowUp.Enabled = IsRowVisible
                        btnFirma.Enabled = IsRowVisible

                End Select

            End If
        End If

    End Sub

    Private Sub AnnullaBilancio()

        'campi percentili
        lblCranio.Visible = False
        txtCranio.Visible = False
        txtCranio.Text = String.Empty
        lblPercentileCranio.Visible = False
        txtPercentileCranio.Visible = False
        txtPercentileCranio.Text = String.Empty

        lblAltezza.Visible = False
        txtAltezza.Visible = False
        txtAltezza.Text = String.Empty
        lblPercentileAltezza.Visible = False
        txtPercentileAltezza.Visible = False
        txtPercentileAltezza.Text = String.Empty

        lblPeso.Visible = False
        txtPeso.Visible = False
        txtPeso.Text = String.Empty
        lblPercentilePeso.Visible = False
        txtPercentilePeso.Visible = False
        txtPercentilePeso.Text = String.Empty

        ' campi opzionali
        ucDatiOpzionaliBilancio.Clear()

        'datalist bilancio
        dlsBilanci.SelectedIndex = -1

        'datalist questionario compilato
        Questionario.DtSezioni = Nothing

        BindControls(PageStatus.LIST)
        GestioneToolbar(PageStatus.LIST)

    End Sub

    ''' <summary>
    ''' Eliminazione bilancio, rfresh dei dati e impostazione del layout
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub EliminaBilancioSelezionato()

        Try
            Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, OnVacUtility.GetReadCommittedTransactionOptions())

                Using dbGenericProviderFactory As New Biz.DbGenericProviderFactory()

                    Dim visitaDaEliminare As Visita = Nothing
                    Dim visitaDaEliminareFollowUp As Visita = Nothing

                    Dim deleteVisitaAndOsservazioniResult As Biz.BizVisite.EliminaVisitaAndOsservazioniResult
                    Dim deleteVisitaAndOsservazioniResultFollowUp As Biz.BizVisite.EliminaVisitaAndOsservazioniResult

                    Using bizVisite As New Biz.BizVisite(dbGenericProviderFactory, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                        visitaDaEliminare = bizVisite.GetVisitaById(VisitaSelezionataOld.IdVisita)
                        ' controllo se visita è padre di un followup in questo caso prima cancello visita followup
                        If VisitaSelezionataOld.FollowUpId.HasValue Then
                            visitaDaEliminareFollowUp = bizVisite.GetVisitaById(VisitaSelezionataOld.FollowUpId)
                            deleteVisitaAndOsservazioniResultFollowUp = bizVisite.DeleteVisitaAndOsservazioni(visitaDaEliminareFollowUp)
                        End If

                        deleteVisitaAndOsservazioniResult = bizVisite.DeleteVisitaAndOsservazioni(visitaDaEliminare)

                    End Using

                    ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
                    If FlagAbilitazioneVaccUslCorrente Then

                        Using bizPaziente As Biz.BizPaziente = New Biz.BizPaziente(dbGenericProviderFactory, Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.GEST_BIL, True))

                            Dim aggiornaDatiVaccinaliCentraliCommand As New Biz.BizPaziente.AggiornaDatiVaccinaliCentraliCommand()
                            aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale = bizPaziente.GenericProvider.Paziente.GetCodiceAusiliario(OnVacUtility.Variabili.PazId)
                            aggiornaDatiVaccinaliCentraliCommand.VisitaEliminataEnumerable = {visitaDaEliminare}
                            aggiornaDatiVaccinaliCentraliCommand.OsservazioneEliminataEnumerable = deleteVisitaAndOsservazioniResult.OsservazioniEliminate.AsEnumerable()

                            bizPaziente.AggiornaDatiVaccinaliCentrali(aggiornaDatiVaccinaliCentraliCommand)

                        End Using

                    End If

                End Using

                transactionScope.Complete()

            End Using

        Catch ex As Exception

            LogBox.WriteData(LogBox.GetTestataException(ex))

            ex.InternalPreserveStackTrace()
            Throw

        End Try

        CaricaTestateBilanci()

        Questionario.DtSezioni = Nothing
        dlsBilanci.SelectedIndex = -1

        BindControls(PageStatus.LIST)
        GestioneToolbar(PageStatus.LIST)

    End Sub

    Private Function CheckQuestionario() As Boolean

        Dim errorMsg As New System.Text.StringBuilder()

        ' Controllo campi obbligatori della visita
        If Me.VisitaSelezionata.DataVisita = Date.MinValue Then
            errorMsg.Append("La Data della visita e\' obbligatoria.\n")
        End If

        If String.IsNullOrWhiteSpace(Me.VisitaSelezionata.MedicoCodice) Then
            errorMsg.Append("Il Medico e\' obbligatorio.\n")
        End If

        If String.IsNullOrWhiteSpace(Me.VisitaSelezionata.RilevatoreCodice) Then
            errorMsg.Append("L\'Operatore e\' obbligatorio.\n")
        End If

        ' Controllo osservazioni del questionario
        Dim checkResult As QuestionarioBilancio.CheckCampiQuestionarioResult = Me.Questionario.CheckCampiQuestionario()

        If checkResult.ErrorType = QuestionarioBilancio.CheckCampiQuestionarioType.ErroreBloccante Then

            errorMsg.AppendFormat("{0}\n", checkResult.Message)

        End If

        If errorMsg.Length > 0 Then

            Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(String.Format("Impossibile effettuare il salvataggio.\n{0}", errorMsg.ToString()), CONFIRM_ERROR_EMPTY_FIELDS, False, False))
            Return False

        End If

        ' Controllo campi singoli della visita e composizione messaggio per warning
        If String.IsNullOrWhiteSpace(Me.VisitaSelezionata.Firma) Then
            errorMsg.Append("Firma bilancio assente.\n")
        End If

        If Me.VisitaSelezionata.RegistraPeso AndAlso Me.VisitaSelezionata.Peso = 0 Then
            errorMsg.Append("Peso assente o valore incoerente.\n")
        End If

        If Me.VisitaSelezionata.RegistraAltezza AndAlso Me.VisitaSelezionata.Altezza = 0 Then
            errorMsg.Append("Altezza assente o valore incoerente.\n")
        End If

        If Me.VisitaSelezionata.RegistraCranio AndAlso Me.VisitaSelezionata.Cranio = 0 Then
            errorMsg.Append("Circonferenza cranica assente o valore incoerente.\n")
        End If

        If checkResult.ErrorType = QuestionarioBilancio.CheckCampiQuestionarioType.Warning Then

            errorMsg.AppendFormat("{0}\n", checkResult.Message)

        End If

        If errorMsg.Length > 0 Then

            If Me.UtenteLoggatoIsMedico Then

                ' Bloccante (l'OnitLayoutMsgBox non chiede conferma ma mostra il messaggio e blocca)
                Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(String.Format("Impossibile effettuare il salvataggio.\n{0}", errorMsg.ToString()), CONFIRM_ERROR_EMPTY_FIELDS, False, False))
                Return False

                'Else

                '    ' Non bloccante (l'OnitLayoutMsgBox chiede conferma all'utente)
                '    Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(String.Format("ATTENZIONE!\n{0}\n\nContinuare?", errorMsg.ToString()), CONFIRM_ERROR_EMPTY_FIELDS, True, True))

            End If

            'Return False

        End If

        Return True

    End Function
    Private Function CheckViaggi() As Boolean
        Dim errorMsg As New System.Text.StringBuilder()
        Dim viaggi As List(Of ViaggioVisita) = ucDatiOpzionaliBilancio.GetListaDatiViaggio()
        Dim viaggiEdit As List(Of ViaggioVisita) = viaggi.Where(Function(p) p.Operazione <> OperazioneViaggio.Delete).OrderBy(Function(p) p.DataInizioViaggio).ToList()
        'Verifico che ci sia almeno un viaggio
        If viaggiEdit.Count = 0 Then
            errorMsg.Append("Deve esistere al meno un viaggio!")
            Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(String.Format("Impossibile effettuare il salvataggio.\n{0}", errorMsg.ToString()), "VNR", False, False))
            Return False
        End If
        ' Controllo se ci sono dati nulli o intervalli delle date sovrapposte
        Dim dataFineApp As Date = Date.MinValue
        For Each v As ViaggioVisita In viaggiEdit
            If v.DataInizioViaggio = Date.MinValue OrElse v.DataFineViaggio = Date.MinValue Then
                errorMsg.Append("Le Date del viaggio sono\' obbligatorie.\n")
            End If
            If String.IsNullOrWhiteSpace(v.CodicePaese.ToString()) Then
                errorMsg.Append("Il paese del viaggio e\' obbligatorio.\n")
            End If
            If v.DataInizioViaggio < v.DataFineViaggio Then
                errorMsg.Append("Data di fine viaggio non puo\' essere minore dell\'inizio.\n")
            End If
            ' Se data inizio del viaggio è minore della data di fine del viaggio precedente,
            ' allora non posso salvare
            If v.DataInizioViaggio.Date < dataFineApp.Date Then
                errorMsg.Append("Date del viaggio si sovreppongono.\n")
            Else
                dataFineApp = v.DataFineViaggio
            End If
        Next
        If errorMsg.Length > 0 Then

            Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(String.Format("Impossibile effettuare il salvataggio.\n{0}", errorMsg.ToString()), "VNR", False, False))
            Return False

        End If

        Return True
    End Function

    Private Sub SetDataListColumnVisibility(tableCellId As String, dataListItem As DataListItem, visible As Boolean)

        Dim td As HtmlControls.HtmlTableCell = DirectCast(dataListItem.FindControl(tableCellId), HtmlControls.HtmlTableCell)

        If Not td Is Nothing Then td.Visible = visible

    End Sub

    Private Sub RecuperaStoricoVaccinale()

        Dim command As New Common.OnVacStoricoVaccinaleCentralizzato.AcquisisciStoricoCommand()
        command.CodicePaziente = OnVacUtility.Variabili.PazId
        command.RichiediConfermaSovrascrittura = False
        command.Settings = Me.Settings
        command.OnitLayout3 = Me.OnitLayout31
        command.BizLogOptions = Nothing
        command.Note = "Recupero Storico Vaccinale da maschera VisioneBilanci"

        Dim acquisisciDatiVaccinaliCentraliResult As Biz.BizPaziente.AcquisisciDatiVaccinaliCentraliResult =
               Common.OnVacStoricoVaccinaleCentralizzato.AcquisisciDatiVaccinaliCentraliPaziente(command)

        If acquisisciDatiVaccinaliCentraliResult.StatoAcquisizioneDatiVaccinaliCentrale <> Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Parziale Then

            ' Se il recupero è andato a buon fine
            Me.StoricoVaccinaleCentralizzatoDaRecuperare = False

        End If

        '--
        'Il datatable dei bilanci non è in sessione, per cui va caricato ogni volta (è una scelta)
        CaricaTestateBilanci()
        AggiornaUltimaVisitaSelezionata()
        '--

        Me.BindControls(PageStatus.LIST)

        If Me.dlsBilanci.SelectedIndex > -1 Then
            GestioneToolbar(PageStatus.SELECTED)
        Else
            GestioneToolbar(PageStatus.LIST)
        End If

    End Sub

#Region " Controlli sulla visita "

    Private Class ControlloVisitaResult
        Public Success As Boolean
        Public Message As String
        Public SetLayout As Boolean
        Public Status As PageStatus
    End Class

    Private Function ControlloVisitaSelezionata(checkGiorniTrascorsi As Boolean) As ControlloVisitaResult

        Dim result As New ControlloVisitaResult()

        ' Controlli e modifica
        Dim message As String = String.Empty

        ' ------------------------------------ '
        ' [Unificazione Ulss]: Eliminato controllo usl inserimento
        ' ------------------------------------ '
        ' Dim gestioneStoricoVacc As New Common.OnVacStoricoVaccinaleCentralizzato(Me.Settings)

        'If FlagAbilitazioneVaccUslCorrente AndAlso Not gestioneStoricoVacc.CheckVisitaStessaUsl(Me.VisitaSelezionata) Then

        '    ' Controllo usl inserimento visita (se viene gestito lo storico vaccinale centralizzato)
        '    result.Message = Common.OnVacStoricoVaccinaleCentralizzato.AlertMessageModificaVisitaNoUslCorrente
        '    result.Success = False
        '    result.SetLayout = False

        'Else
        ' ------------------------------------ '
        If checkGiorniTrascorsi AndAlso Not OnVacUtility.CheckGiorniTrascorsiVariazioneDatiVaccinali(VisitaSelezionata.DataRegistrazione, Settings) Then

            ' Controllo giorni trascorsi da modifica (indipendentemente dalla gestione dello storico vaccinale centralizzato)
            result.Message = String.Format("alert('Il numero di giorni trascorsi dalla data di registrazione del bilancio è superiore al limite massimo impostato ({0}): impossibile effettuare la modifica.');",
                                           Settings.GIORNI_VARIAZIONE_VACCINAZIONE_ESEGUITA.ToString())

            result.Success = False
            result.SetLayout = False

        Else

            ' Controllo utente: se l'utente è un medico può modificare il bilancio solo se sono trascorsi meno giorni di quelli parametrizzati.
            If checkGiorniTrascorsi AndAlso Me.UtenteLoggatoIsMedico AndAlso Date.Now.Subtract(VisitaSelezionata.DataRegistrazione).Days >= Settings.GIORNI_MODIFICA_BILANCIO_MEDICO Then

                message = String.Format("alert('Attenzione, sono passati più di {0} giorni dalla data di registrazione del bilancio. Contattare il centro vaccinale per la modifica.');",
                                        Settings.GIORNI_MODIFICA_BILANCIO_MEDICO)

                result.Success = False
                result.SetLayout = True
                result.Status = PageStatus.SELECTED

            Else

                result.Success = True
                result.SetLayout = True
                result.Status = PageStatus.EDIT

            End If

        End If

        Return result

    End Function

    ''' <summary>
    ''' Restituisce il valore del flag firma, per la visita selezionata
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Function GetFlagFirmaDigitaleVisitaSelezionata() As Boolean

        If Me.dlsBilanci.SelectedIndex = -1 Then Return False

        Dim hid As HiddenField = DirectCast(Me.dlsBilanci.SelectedItem.FindControl("hidUteFirma"), HiddenField)
        If hid Is Nothing Then Return False

        If String.IsNullOrEmpty(HttpUtility.HtmlDecode(hid.Value)) Then Return False

        Return True

    End Function

#End Region

#Region " Consenti / Nega Visibilità dati vaccinali "

    Private Sub SetVisibilitaDatiVaccinali()

        ' Controlli e modifica

        ' ------------------------------------ '
        ' [Unificazione Ulss]: Eliminato controllo usl inserimento
        ' ------------------------------------ '
        ' Dim gestioneStoricoVacc As New Common.OnVacStoricoVaccinaleCentralizzato(Settings)

        'If FlagAbilitazioneVaccUslCorrente AndAlso Not gestioneStoricoVacc.CheckVisitaStessaUsl(Me.VisitaSelezionata) Then

        '    ' Controllo usl inserimento visita (se viene gestito lo storico vaccinale centralizzato)
        '    Me.OnitLayout31.InsertRoutineJS(Common.OnVacStoricoVaccinaleCentralizzato.AlertMessageModificaVisitaNoUslCorrente)

        '    Return

        'End If
        ' ------------------------------------ '

        Dim flagVisibilita As String = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente

        If VisitaSelezionata.FlagVisibilitaDatiVaccinaliCentrale = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente Then

            flagVisibilita = Constants.ValoriVisibilitaDatiVaccinali.NegatoDaPaziente

        End If

        ' ------------------------------------ '
        ' [Unificazione Ulss]: Sostituito aggiornamento visibilità nei db locali delle varie ULSS con update nel db unico
        ' ------------------------------------ '
        'Dim listIdVisita As New List(Of Int64)()
        'listIdVisita.Add(VisitaSelezionata.IdVisita)

        'Using bizPaziente As New Biz.BizPaziente(Settings, Nothing, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(TipiArgomento.PAZIENTI))

        '    bizPaziente.AggiornaVisibilitaDatiVaccinaliCentrali(OnVacUtility.Variabili.PazId, Nothing, Nothing, Nothing, listIdVisita.ToArray(), flagVisibilita)

        'End Using
        ' ------------------------------------ '

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizVisite As New Biz.BizVisite(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(TipiArgomento.PAZIENTI))

                bizVisite.UpdateFlagVisibilita(VisitaSelezionata.IdVisita, flagVisibilita)

            End Using
        End Using

        VisitaSelezionataOld = VisitaSelezionata

        CaricaTestateBilanci()

        dlsBilanci.SelectedIndex = -1
        Questionario.DtSezioni = Nothing

        BindControls(PageStatus.LIST)
        GestioneToolbar(PageStatus.LIST)

    End Sub

#End Region

#Region "Follow up"

    Private Sub CreaFollowUp()
        If Not GetFlagMalattiaSoloFollowUp() Then
            OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(Biz.BizVisite.Messages.NO_MALATTIA_FOLLOW_UP, "errMalattiaFollowUp", False, False))
            Return
        End If
        Dim hidId As HiddenField = DirectCast(Me.dlsBilanci.SelectedItem.FindControl("HiddenIdFollowUp"), HiddenField)
        If Not String.IsNullOrWhiteSpace(hidId.Value) Then
            OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(Biz.BizVisite.Messages.NO_ID_FOLLOW_UP_VALORIZZATO, "errIdFollowUp", False, False))
            Return
        End If

        Me.Response.Redirect(String.Format("../../HBilanci/GestioneBilancio/GestioneBilancio.aspx?isFollowUpRequest=True&vesIdPadre={0}&bil_numero={1}&mal_codice={2}&EditCnv={3}",
                                          HttpUtility.UrlEncode(VisitaSelezionata.IdVisita.ToString()),
                                           HttpUtility.UrlEncode(VisitaSelezionata.BilancioNumero.ToString()),
                                           HttpUtility.UrlEncode(VisitaSelezionata.MalattiaCodFollowUp),
                                            HttpUtility.UrlEncode(True.ToString())))
    End Sub
    Protected Function GetFlagMalattiaSoloFollowUp() As Boolean

        If Me.dlsBilanci.SelectedIndex = -1 Then Return False

        Dim hid As HiddenField = DirectCast(Me.dlsBilanci.SelectedItem.FindControl("HiddenSoloFollowUp"), HiddenField)
        If hid Is Nothing Then Return False

        If String.IsNullOrEmpty(HttpUtility.HtmlDecode(hid.Value)) OrElse HttpUtility.HtmlDecode(hid.Value).ToString() = "N" Then Return False

        Return True

    End Function
#End Region

#Region " Firma Digitale "

    Private Enum ViewIndex
        ViewDati = 0
        ViewFirmaDigitale = 1
    End Enum

    Private Sub SwitchViewDati()

        Me.multiViewMain.SetActiveView(Me.multiViewMain.Views(ViewIndex.ViewDati))

    End Sub

    Private Sub SwitchViewFirmaDigitale()

        Me.LayoutTitoloViewFirma.Text = Me.LayoutTitolo.Text

        Me.multiViewMain.SetActiveView(Me.multiViewMain.Views(ViewIndex.ViewFirmaDigitale))

        Me.ucFirma.AnteprimaAnamnesi(Me.VisitaSelezionata.IdVisita)

    End Sub

    Private Sub ucFirma_ClickIndietro(idDocumento As Long) Handles ucFirma.ClickIndietro

        Me.SwitchViewDati()

    End Sub

    Private Sub ucFirma_FirmaDigitaleCompleted(success As Boolean, message As String) Handles ucFirma.FirmaDigitaleCompleted

        Me.ShowMessageFirmaDigitale(message)

        If success Then

            Me.SwitchViewDati()

            ' TODO [firma digitale]: reimpostare la selezione ???
            Me.VisitaSelezionataOld = Me.VisitaSelezionata

            Me.CaricaTestateBilanci()

            Me.dlsBilanci.SelectedIndex = -1
            Me.Questionario.DtSezioni = Nothing

            Me.BindControls(PageStatus.LIST)
            Me.GestioneToolbar(PageStatus.LIST)

        End If

    End Sub

    Private Sub ucFirma_ShowMessage(message As String) Handles ucFirma.ShowMessage

        Me.ShowMessageFirmaDigitale(message)

    End Sub

    Private Sub ShowMessageFirmaDigitale(message As String)

        ClientScript.RegisterClientScriptBlock(GetType(Page), "js",
            String.Format("<script type='text/javascript'>alert('{0}');</script>", HttpUtility.JavaScriptStringEncode(message)))

    End Sub

#End Region

#End Region

#Region " Protected Methods "

    Protected Function BindFlagVisibilitaImageUrlValue(dataItem As Object) As String

        Return Common.OnVacStoricoVaccinaleCentralizzato.GetImageUrlFlagVisibilita(dataItem, "VIS_FLAG_VISIBILITA", Me)

    End Function

    Protected Function BindFlagVisibilitaToolTipValue(dataItem As Object) As String

        Return Common.OnVacStoricoVaccinaleCentralizzato.GetToolTipFlagVisibilita(dataItem, "VIS_FLAG_VISIBILITA")

    End Function

    Protected Function BindFlagFirmaImageUrlValue(dataItem As Object) As String

        Return OnVacUtility.GetFlagFirmaImageUrlValue(DataBinder.Eval(dataItem, "VIS_UTE_ID_FIRMA"), DataBinder.Eval(dataItem, "VIS_UTE_ID_ARCHIVIAZIONE"), Me.Page)

    End Function

    Protected Function BindFlagFirmaToolTipValue(dataItem As Object) As String

        Return OnVacUtility.GetFlagFirmaToolTipValue(DataBinder.Eval(dataItem, "VIS_UTE_ID_FIRMA"), DataBinder.Eval(dataItem, "VIS_UTE_ID_ARCHIVIAZIONE"))

    End Function
    Protected Function BindFlagFollowUpImageUrlValue(dataItem As Object) As String
        Dim idFollowUp As String = DataBinder.Eval(dataItem, "VIS_VIS_ID_FOLLOW_UP").ToString()
        Dim ret As String
        If Not String.IsNullOrWhiteSpace(idFollowUp) Then
            ret = Page.ResolveClientUrl("~/Images/document-edit.png")
        Else
            ret = Page.ResolveClientUrl("~/Images/transparent16.gif")
        End If

        Return ret

    End Function

    Protected Function BindFlagFollowUPToolTipValue(dataItem As Object) As String
        Dim idFollowUp As String = DataBinder.Eval(dataItem, "VIS_VIS_ID_FOLLOW_UP").ToString()

        Dim visita As Visita = Nothing

        Dim ret As String
        If Not String.IsNullOrWhiteSpace(idFollowUp) Then
            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizVisite As New Biz.BizVisite(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(TipiArgomento.PAZIENTI))
                    Dim id As Integer = DataBinder.Eval(dataItem, "VIS_VIS_ID_FOLLOW_UP")
                    visita = bizVisite.GetVisitaById(id)

                End Using
            End Using
            ret = String.Format("La visita ha un follow up con data visita {0}", visita.DataVisita.ToShortDateString())
        Else
            ret = String.Empty
        End If
        Return ret

    End Function

    Protected Function BindNoteImageUrlValue(dataItem As Object) As String
        Dim stringret As String = String.Empty

        If Not dataItem Is Nothing AndAlso Not dataItem Is DBNull.Value Then

            Dim flagVisibilita As Object = DataBinder.Eval(dataItem, "VIS_NOTE")

            If Not flagVisibilita Is Nothing AndAlso
               Not flagVisibilita Is DBNull.Value Then
                stringret = ResolveClientUrl("~/Images/note.png")
            End If
        End If
        Return stringret
    End Function

    Protected Function BindNoteToolTipValue(dataItem As Object) As String

        Dim stringret As String = String.Empty

        If Not dataItem Is Nothing AndAlso Not dataItem Is DBNull.Value Then



            Dim flagVisibilita As Object = DataBinder.Eval(dataItem, "VIS_NOTE")

            If Not flagVisibilita Is Nothing AndAlso
               Not flagVisibilita Is DBNull.Value Then
                stringret = "Note della visita"
            End If
        End If
        Return stringret


    End Function
    Protected Function BindNoteVisible(dataItem As Object) As Boolean

        Dim boolret As Boolean = False

        If Not dataItem Is Nothing AndAlso Not dataItem Is DBNull.Value Then



            Dim flagVisibilita As Object = DataBinder.Eval(dataItem, "VIS_NOTE")

            If Not flagVisibilita Is Nothing AndAlso
               Not flagVisibilita Is DBNull.Value Then
                boolret = True
            End If
        End If
        Return boolret


    End Function
    Protected Function BindFollowUpVisible(dataItem As Object) As Boolean

        Dim boolret As Boolean = False

        If Not dataItem Is Nothing AndAlso Not dataItem Is DBNull.Value Then



            Dim flagFollowUp As Object = DataBinder.Eval(dataItem, "VIS_VIS_ID_FOLLOW_UP")

            If Not flagFollowUp Is Nothing AndAlso
               Not flagFollowUp Is DBNull.Value Then
                boolret = True
            End If
        End If
        Return boolret


    End Function



#End Region

End Class
