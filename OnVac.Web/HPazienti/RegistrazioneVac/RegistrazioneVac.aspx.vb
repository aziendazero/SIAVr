Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Biz
Imports Onit.OnAssistnet.OnVac.Biz.BizVaccinazioniEseguite
Imports Onit.Controls
Imports System.Text
Imports Onit.Web.UI.WebControls.Validators

Partial Class RegistrazioneVac
    Inherits Common.PageBase

#Region " Public "

    Public strJS As String

    Private Class ControlloCampoObbligatorio
        Public Property Controllo As String
        Public Property CampoObbligatorio As String
    End Class

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

    Private Property dtaVaccinazioni() As DataTable
        Get
            Return Session("OnVac_dtaVaccinazioni")
        End Get
        Set(Value As DataTable)
            Session("OnVac_dtaVaccinazioni") = Value
        End Set
    End Property

    ' Memorizza il risultato della query per controllare se il paziente è deceduto oppure no
    Public ReadOnly Property PazDeceduto() As Boolean
        Get
            ' Se il paz è deceduto, non devo poter eseguire le vaccinazioni. 
            ' Qui faccio la query per sapere se è deceduto, nel click del pulsante Esegui faccio il controllo
            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizPaz As New Biz.BizPaziente(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                    PazDeceduto = bizPaz.IsDeceduto(OnVacUtility.Variabili.PazId)
                End Using
            End Using
        End Get
    End Property

    Public ReadOnly Property AssociazioneCodiceDescrizione() As Boolean
        Get
            Return (Settings.REGVAC_ASSCODICEDESCRIZIONE = "0")
        End Get
    End Property

    Private ReadOnly Property ValoreVisibilitaDefaultPaziente As String
        Get
            If ViewState("ValoreVisibilitaDefaultPaziente") Is Nothing Then
                ViewState("ValoreVisibilitaDefaultPaziente") = GetValoreVisibilitaDatiVaccinaliDefault(OnVacUtility.Variabili.PazId)
            End If
            Return ViewState("ValoreVisibilitaDefaultPaziente").ToString()
        End Get
    End Property

    Private Property LuoghiEsecuzione As List(Of Entities.LuoghiEsecuzioneVaccinazioni)
        Get
            If ViewState("LUOGHIVAC") Is Nothing Then ViewState("LUOGHIVAC") = New List(Of Entities.LuoghiEsecuzioneVaccinazioni)()
            Return DirectCast(ViewState("LUOGHIVAC"), List(Of Entities.LuoghiEsecuzioneVaccinazioni))
        End Get
        Set(Value As List(Of Entities.LuoghiEsecuzioneVaccinazioni))
            ViewState("LUOGHIVAC") = Value
        End Set
    End Property

    ''' <summary>
    ''' Lista di malattie associate al paziente filtrate per una delle tipologie impostate nel parametro CONDIZIONE_SANITARIA_TIPOLOGIE_MALATTIA.
    ''' E' una lista di elementi (codice malattia - codice vaccinazione).
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property CondizioniSanitariePaziente As List(Of Entities.CondizioneSanitaria)
        Get
            If ViewState("CSPAZ") Is Nothing Then ViewState("CSPAZ") = New List(Of Entities.CondizioneSanitaria)()
            Return DirectCast(ViewState("CSPAZ"), List(Of Entities.CondizioneSanitaria))
        End Get
        Set(Value As List(Of Entities.CondizioneSanitaria))
            ViewState("CSPAZ") = Value
        End Set
    End Property

    ''' <summary>
    ''' Condizione di rischio associata al paziente.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property CondizioniRischioPaziente As List(Of Entities.CondizioneRischio)
        Get
            If ViewState("CRPAZ") Is Nothing Then ViewState("CRPAZ") = New List(Of Entities.CondizioneRischio)()
            Return DirectCast(ViewState("CRPAZ"), List(Of Entities.CondizioneRischio))
        End Get
        Set(Value As List(Of Entities.CondizioneRischio))
            ViewState("CRPAZ") = Value
        End Set
    End Property

    Private ReadOnly Property DescrizioneMalattiaDefault As String
        Get
            If ViewState("DescrMalDefault") Is Nothing Then
                Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                    Using bizMalattie As New BizMalattie(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                        ViewState("DescrMalDefault") = bizMalattie.GetDescrizioneMalattia(Me.Settings.CONDIZIONE_SANITARIA_DEFAULT)
                    End Using
                End Using
            End If
            Return ViewState("DescrMalDefault").ToString()
        End Get
    End Property
    Private ReadOnly Property DescrizioneCategoriaRischioDefault As String
        Get
            If ViewState("DescrRscDefault") Is Nothing Then
                Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                    Using bizRischio As New BizCategorieRischio(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())
                        ViewState("DescrRscDefault") = bizRischio.GetDescrizioneCategoriaRischio(Me.Settings.CONDIZIONE_RISCHIO_DEFAULT)
                    End Using
                End Using
            End If
            Return ViewState("DescrRscDefault").ToString()
        End Get
    End Property

    ''' <summary>
    ''' Campi obbligatori in base al luogo selezionato
    ''' </summary>
    ''' <returns></returns>
    Private Property CampiObbligatoriByLuogoSelezionato As List(Of String)
        Get
            If ViewState("CAMPIOBBLIGATORI") Is Nothing Then ViewState("CAMPIOBBLIGATORI") = New List(Of String)()
            Return DirectCast(ViewState("CAMPIOBBLIGATORI"), List(Of String))
        End Get
        Set(Value As List(Of String))
            ViewState("CAMPIOBBLIGATORI") = Value
        End Set
    End Property
#Region " Gestione Icone per Note Vaccinazioni "

    Private ReadOnly Property IconaNotaSiEnabled() As String
        Get
            Return "NotaSi.gif"
        End Get
    End Property

    Private ReadOnly Property IconaNotaSiDisabled() As String
        Get
            Return "NotaSi_dis.gif"
        End Get
    End Property

    Private ReadOnly Property IconaNotaNoEnabled() As String
        Get
            Return "NotaNo.gif"
        End Get
    End Property

    Private ReadOnly Property IconaNotaNoDisabled() As String
        Get
            Return "NotaNo_dis.gif"
        End Get
    End Property

    Protected ReadOnly Property urlIconaNotaSi(enabled As Boolean) As String
        Get
            If enabled Then
                Return "~/images/" + IconaNotaSiEnabled
            Else
                Return "~/images/" + IconaNotaSiDisabled
            End If
        End Get
    End Property

    Protected ReadOnly Property urlIconaNotaNo(enabled As Boolean) As String
        Get
            If enabled Then
                Return "~/images/" + IconaNotaNoEnabled
            Else
                Return "~/images/" + IconaNotaNoDisabled
            End If
        End Get
    End Property

#End Region

#End Region

#Region " Types "
    Private Enum IndexDgrVac
        CheckSelezione = 0
        Replica = 1
        Associazione = 2
        DoseAssociazione = 3
        Vaccinazioni = 4
        DummyColumn = 5
        DataEsecuzione = 6
        LuogoEsecuzione = 7
        TipoErogatore = 8
        Consultorio = 9
        FlagPagamento = 10
        CheckFittizia = 11
        FlagVisibilita = 12
        FlagEseguita = 13
        DatiAggiuntivi = 14
        Note = 15
        MedicoResponsabile = 16
        Vaccinatore = 17
        Comune_Stato = 18
        ViaSomministrazione = 19
        SitoInoculazione = 20
        NomeCommerciale = 21
        CodiceLotto = 22
        DataScadenzaLotto = 23
        Tipo_Comune_Stato = 24
        GuidTipoPagamento = 25
        CodiceMalattia = 26
        CodiceEsenzione = 27
        Importo = 28
        CodiceStruttura = 29
        CodiceUsl = 30
    End Enum
#End Region

#Region " Gestione icone per dati aggiuntivi "

    Protected ReadOnly Property UrlIconaDatiSi(enabled As Boolean) As String
        Get
            If enabled Then
                Return "~/images/NotaSi.gif"
            Else
                Return "~/images/NotaSi_dis.gif"
            End If
        End Get
    End Property

    Protected ReadOnly Property UrlIconaDatiNo(enabled As Boolean) As String
        Get
            If enabled Then
                Return "~/images/NotaNo.gif"
            Else
                Return "~/images/NotaNo_dis.gif"
            End If
        End Get
    End Property

#End Region

#Region " Constants "

    Private Const KEY_ELIMINA_PROGRAMMAZIONE As String = "EliminaProgrammazione"
    Private Const KEY_ELIMINA_PROGRAMMAZIONE_APPUNTAMENTO As String = "EliminaProgrammazioneAppuntamento"

    Private Const CSS_CLASS_TEXTBOX_STRINGA As String = "TextBox_Stringa"
    Private Const CSS_CLASS_TEXTBOX_STRINGA_OBBLIGATORIO As String = "TextBox_Stringa_Obbligatorio"
    Private Const CSS_CLASS_TEXTBOX_STRINGA_DISABILITATO As String = "TextBox_Stringa_Disabilitato"

#End Region

#Region " Page Events "

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            InizializzazionePagina()

            ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
            If FlagConsensoVaccUslCorrente Then

                Dim statoAcquisizioneDatiVaccinaliCentrale As Enumerators.StatoAcquisizioneDatiVaccinaliCentrale? =
                       Common.OnVacStoricoVaccinaleCentralizzato.GetStatoAcquisizioneDatiVaccinaliCentralePaziente(OnVacUtility.Variabili.PazId)

                If Not statoAcquisizioneDatiVaccinaliCentrale.HasValue OrElse
                    statoAcquisizioneDatiVaccinaliCentrale = Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Parziale Then

                    strJS &= Common.OnVacStoricoVaccinaleCentralizzato.AlertMessageRecuperoStoricoVaccinale
                    SetToolbarStatus(True)

                Else

                    ControlloEliminazioneProgrammazione()
                    SetToolbarStatus(False)

                End If

            Else

                ControlloEliminazioneProgrammazione()
                SetToolbarStatus(False)

            End If

        End If

    End Sub

#End Region

    '#Region " Controlli in esecuzione "

    '    Private Function CheckCampiObbligatori(itemIndex As Int16, ByRef controlList As ControlloEsecuzione) As Boolean
    '        Dim result As New BizRegistrazioneVaccinazioni.CheckDatiAssociazioneResult

    '        Dim associazioneDaRegistrare As New BizRegistrazioneVaccinazioni.AssociazioneDaRegistrare()

    '        associazioneDaRegistrare.CodiceAssociazione = DirectCast(Me.dgrVaccinazioni.Items(itemIndex).FindControl("lblAssociazioneItem"), Label).Text
    '        ' Dose di associazione
    '        Dim textDoseAssociazione As String = DirectCast(Me.dgrVaccinazioni.Items(itemIndex).FindControl("txtAssDoseItem"), TextBox).Text

    '        ' Controlli di coerenza sui dati inseriti
    '        ' Per ogni vaccinazione contenuta nella riga di associazione:
    '        Dim dgrDettaglio As DataGrid = DirectCast(Me.dgrVaccinazioni.Items(itemIndex).FindControl("dgrDettaglio"), DataGrid)

    '        For i As Integer = 0 To dgrDettaglio.Items.Count - 1

    '            ' Dose di vaccinazione (unico campo modificabile sulle vaccinazioni contenute)
    '            Dim strDoseVac As String = DirectCast(dgrDettaglio.Items(i).FindControl("txtDettagliDoseVac"), TextBox).Text
    '            Dim vacCodice As String = DirectCast(dgrDettaglio.Items(i).FindControl("hdfVaccinazioneCodice"), HiddenField).Value

    '            If GetDoseFromString(strDoseVac) <= 0 Then
    '                controlList.Add(New ControlloEsecuzioneItem(vacCodice, 0, True, ControlloEsecuzioneItemType.DatiIncompleti))
    '                Return False
    '            End If

    '        Next

    '        Dim assCodice As String = DirectCast(Me.dgrVaccinazioni.Items(itemIndex).FindControl("lblAssociazioneItem"), Label).Text
    '        Dim strDose As String = DirectCast(Me.dgrVaccinazioni.Items(itemIndex).FindControl("txtAssDoseItem"), TextBox).Text

    '        ' Dose associazione errata
    '        If GetDoseFromString(strDose) <= 0 Then
    '            controlList.Add(New ControlloEsecuzioneItem(assCodice, strDose, True, ControlloEsecuzioneItemType.DatiIncompleti))
    '            Return False
    '        End If

    '        ' Data esecuzione errata
    '        If DirectCast(Me.dgrVaccinazioni.Items(itemIndex).FindControl("txtDataItem"), Onit.Web.UI.WebControls.Validators.OnitDatePick).Text = String.Empty Then
    '            controlList.Add(New ControlloEsecuzioneItem(assCodice, strDose, True, ControlloEsecuzioneItemType.DatiIncompleti))
    '            Return False
    '        End If

    '        For i As Integer = 0 To dgrDettaglio.Items.Count - 1

    '            Dim vaccinazioneDaRegistrare As New BizRegistrazioneVaccinazioni.AssociazioneDaRegistrare.VaccinazioneDaRegistrare()
    '            vaccinazioneDaRegistrare.CodiceVaccinazione = DirectCast(dgrDettaglio.Items(i).FindControl("hdfVaccinazioneCodice"), HiddenField).Value
    '            ' Dose di vaccinazione
    '            Dim textDoseVaccinazione As String = DirectCast(dgrDettaglio.Items(i).FindControl("txtDettagliDoseVac"), TextBox).Text

    '            Dim doseVaccinazione As Int16 = 0
    '            If Not Integer.TryParse(textDoseVaccinazione, Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, doseVaccinazione) Then
    '                '--
    '                controlList.Add(New ControlloEsecuzioneItem(vaccinazioneDaRegistrare.CodiceVaccinazione, 0, True, ControlloEsecuzioneItemType.DatiIncompleti))
    '                Return False
    '                '--
    '            Else
    '                If doseVaccinazione <= 0 Then
    '                    '--
    '                    controlList.Add(New ControlloEsecuzioneItem(vaccinazioneDaRegistrare.CodiceVaccinazione, 0, True, ControlloEsecuzioneItemType.DatiIncompleti))
    '                    Return False
    '                    '--
    '                End If
    '            End If
    '            vaccinazioneDaRegistrare.NumeroDoseVaccinazione = doseVaccinazione

    '            ' Condizione sanitaria
    '            Dim omlCondSanitaria As OnitModalList = DirectCast(dgrDettaglio.Items(i).FindControl("omlCondSanitaria"), OnitModalList)
    '            vaccinazioneDaRegistrare.IdCondizioneSanitaria = omlCondSanitaria.ID
    '            vaccinazioneDaRegistrare.CodiceCondizioneSanitaria = omlCondSanitaria.Codice

    '            ' Condizione di rischio
    '            Dim omlCondRischio As OnitModalList = DirectCast(dgrDettaglio.Items(i).FindControl("omlCondRischio"), OnitModalList)
    '            vaccinazioneDaRegistrare.IdCondizioneRischio = omlCondRischio.ID
    '            vaccinazioneDaRegistrare.CodiceCondizioneRischio = omlCondRischio.Codice

    '            associazioneDaRegistrare.VaccinazioniDaRegistrare.Add(vaccinazioneDaRegistrare)

    '        Next

    '        Dim datiAggiuntivi As BizRegistrazioneVaccinazioni.DatiAggiuntiviVaccinazione =
    '            GetDatiAggiuntiviFromDataGridItem(Me.dgrVaccinazioni.Items(itemIndex))
    '        Dim datiPagamento As BizRegistrazioneVaccinazioni.DatiPagamentoVaccinazione =
    '            GetDatiPagamentoFromDataGridItem(Me.dgrVaccinazioni.Items(itemIndex))

    '        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
    '            Using biz As New BizRegistrazioneVaccinazioni(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos)
    '                result = biz.CheckDatiAssociazione(associazioneDaRegistrare, datiAggiuntivi, datiPagamento)
    '            End Using
    '        End Using

    '    End Function

    '#End Region
#Region " Controlli in esecuzione "

    ''' <summary>
    ''' Controllo campi obbligatori di una riga da eseguire
    ''' </summary>
    ''' <param name="itemIndex"></param>
    ''' <param name="controlList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CheckCampiObbligatori(itemIndex As Int16, ByRef controlList As ControlloEsecuzione) As Boolean

        Dim result As New BizRegistrazioneVaccinazioni.CheckDatiAssociazioneResult

        Dim associazioneDaRegistrare As New BizRegistrazioneVaccinazioni.AssociazioneDaRegistrare()

        associazioneDaRegistrare.CodiceAssociazione = DirectCast(Me.dgrVaccinazioni.Items(itemIndex).FindControl("lblAssociazioneItem"), Label).Text

        ' Data di esecuzione
        associazioneDaRegistrare.DataEsecuzione =
            DirectCast(Me.dgrVaccinazioni.Items(itemIndex).FindControl("txtDataItem"), Onit.Web.UI.WebControls.Validators.OnitDatePick).Data

        ' Dose di associazione
        Dim textDoseAssociazione As String = DirectCast(Me.dgrVaccinazioni.Items(itemIndex).FindControl("txtAssDoseItem"), TextBox).Text

        Dim doseAssociazione As Int16 = 0
        If Not Int16.TryParse(textDoseAssociazione, Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, doseAssociazione) Then
            '--
            controlList.Add(New ControlloEsecuzioneItem(associazioneDaRegistrare.CodiceAssociazione, 0, True, ControlloEsecuzioneItemType.DatiIncompleti))
            Return False
            '--
        Else
            If doseAssociazione <= 0 Then
                '--
                controlList.Add(New ControlloEsecuzioneItem(associazioneDaRegistrare.CodiceAssociazione, 0, True, ControlloEsecuzioneItemType.DatiIncompleti))
                Return False
                '--
            End If
        End If

        associazioneDaRegistrare.NumeroDoseAssociazione = doseAssociazione

        ' Luogo di esecuzione della vaccinazione
        Dim luogoSelezionato As Entities.LuoghiEsecuzioneVaccinazioni = GetLuogoByComboLuogo(DirectCast(Me.dgrVaccinazioni.Items(itemIndex).FindControl("cmbLuogo"), OnitCombo))
        If luogoSelezionato Is Nothing Then

            associazioneDaRegistrare.CodiceLuogoEsecuzione = String.Empty
            associazioneDaRegistrare.TipoLuogoEsecuzione = String.Empty
            associazioneDaRegistrare.IsLuogoDefaultConsultorio = False
            associazioneDaRegistrare.IdCampiObbligatori = Nothing

        Else

            associazioneDaRegistrare.CodiceLuogoEsecuzione = luogoSelezionato.Codice
            associazioneDaRegistrare.TipoLuogoEsecuzione = luogoSelezionato.Tipo
            associazioneDaRegistrare.IsLuogoDefaultConsultorio = luogoSelezionato.IsDefaultConsultorio
            associazioneDaRegistrare.IdCampiObbligatori = GetCampiAggiuntiviObbligatori(luogoSelezionato.Codice, associazioneDaRegistrare.DataEsecuzione)

            ' Se il luogo selezionato nel combo è "in azienda", sbianco il codice presente nel campo ves_comune_stato
            If luogoSelezionato.Tipo = Constants.TipoLuogoEsecuzioneVaccinazione.InAzienda Then
                Me.dgrVaccinazioni.Items(itemIndex).Cells(IndexDgrVac.Comune_Stato).Text = String.Empty
            End If

        End If

        ' Consultorio
        associazioneDaRegistrare.CodiceConsultorio =
            DirectCast(Me.dgrVaccinazioni.Items(itemIndex).FindControl("txtConsultorioItem"), Onit.Controls.OnitModalList).Codice()
        ' Tipo Erogatore
        associazioneDaRegistrare.TipoErogatore = DirectCast(Me.dgrVaccinazioni.Items(itemIndex).FindControl("cmbTipoErogatore"), DropDownList).SelectedValue

        ' Righe di dettaglio
        Dim dgrDettaglio As DataGrid = DirectCast(Me.dgrVaccinazioni.Items(itemIndex).FindControl("dgrDettaglio"), DataGrid)

        For i As Integer = 0 To dgrDettaglio.Items.Count - 1

            Dim vaccinazioneDaRegistrare As New BizRegistrazioneVaccinazioni.AssociazioneDaRegistrare.VaccinazioneDaRegistrare()
            vaccinazioneDaRegistrare.CodiceVaccinazione = DirectCast(dgrDettaglio.Items(i).FindControl("hdfVaccinazioneCodice"), HiddenField).Value

            ' Dose di vaccinazione
            Dim textDoseVaccinazione As String = DirectCast(dgrDettaglio.Items(i).FindControl("txtDettagliDoseVac"), TextBox).Text

            Dim doseVaccinazione As Int16 = 0
            If Not Integer.TryParse(textDoseVaccinazione, Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, doseVaccinazione) Then
                '--
                controlList.Add(New ControlloEsecuzioneItem(vaccinazioneDaRegistrare.CodiceVaccinazione, 0, True, ControlloEsecuzioneItemType.DatiIncompleti))
                Return False
                '--
            Else
                If doseVaccinazione <= 0 Then
                    '--
                    controlList.Add(New ControlloEsecuzioneItem(vaccinazioneDaRegistrare.CodiceVaccinazione, 0, True, ControlloEsecuzioneItemType.DatiIncompleti))
                    Return False
                    '--
                End If
            End If
            vaccinazioneDaRegistrare.NumeroDoseVaccinazione = doseVaccinazione

            ' Condizione sanitaria
            Dim omlCondSanitaria As OnitModalList = DirectCast(dgrDettaglio.Items(i).FindControl("omlCondSanitaria"), OnitModalList)
            vaccinazioneDaRegistrare.IdCondizioneSanitaria = omlCondSanitaria.ID
            vaccinazioneDaRegistrare.CodiceCondizioneSanitaria = omlCondSanitaria.Codice

            ' Condizione di rischio
            Dim omlCondRischio As OnitModalList = DirectCast(dgrDettaglio.Items(i).FindControl("omlCondRischio"), OnitModalList)
            vaccinazioneDaRegistrare.IdCondizioneRischio = omlCondRischio.ID
            vaccinazioneDaRegistrare.CodiceCondizioneRischio = omlCondRischio.Codice

            associazioneDaRegistrare.VaccinazioniDaRegistrare.Add(vaccinazioneDaRegistrare)
        Next

        Dim datiAggiuntivi As BizRegistrazioneVaccinazioni.DatiAggiuntiviVaccinazione =
            GetDatiAggiuntiviFromDataGridItem(Me.dgrVaccinazioni.Items(itemIndex))

        Dim datiPagamento As BizRegistrazioneVaccinazioni.DatiPagamentoVaccinazione =
            GetDatiPagamentoFromDataGridItem(Me.dgrVaccinazioni.Items(itemIndex))

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using biz As New BizRegistrazioneVaccinazioni(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos)
                result = biz.CheckDatiAssociazione(associazioneDaRegistrare, datiAggiuntivi, datiPagamento)
            End Using
        End Using

        If Not result.Success Then
            controlList.AddRange(result.ControlList)
        End If

        Return result.Success

    End Function

#End Region

#Region " Gestione modal list "

    Private Sub RefreshModal(ByRef modal As Object)

        Dim mdl As Onit.Controls.OnitModalList = DirectCast(modal, Onit.Controls.OnitModalList)

        If mdl.Codice <> String.Empty Then
            mdl.RefreshDataBind()
        End If

    End Sub

    'Private Sub txtConsultorioItem_Change(sender As Object, e As Onit.Controls.OnitModalList.ModalListaEventArgument)

    '    strJS &= GetJSReplica(sender, 7)

    'End Sub

    'Private Sub txtMedicoItem_Change(sender As Object, e As Onit.Controls.OnitModalList.ModalListaEventArgument)

    '    strJS &= GetJSReplica(sender, 8)

    'End Sub

    'Private Sub txtLuogoItem_Change(sender As Object, e As Onit.Controls.OnitModalList.ModalListaEventArgument)

    '    strJS &= GetJSReplica(sender, 9)

    'End Sub

    Private Function GetJSReplica(obj As Object, num As Integer) As String

        Dim mdl As Onit.Controls.OnitModalList = DirectCast(obj, Onit.Controls.OnitModalList)

        Return String.Format("Replica({0},'{1}','{2}',document.getElementById('{3}'));", num, mdl.Descrizione, mdl.Codice, mdl.ClientID)

    End Function

#End Region

#Region " Toolbar Events "

    Protected Sub Toolbar_ButtonClick(sender As Object, e As Telerik.Web.UI.RadToolBarEventArgs) Handles Toolbar.ButtonClick

        Select Case e.Item.Value

            Case "btnSalva"

                UnbindDataGrid()
                Salva()

            Case "btnAnnulla"

                Me.dtaVaccinazioni.Clear()
                Me.dgrVaccinazioni.Visible = False
                Me.OnitLayout31.Busy = False

            Case "btnCalcolaSeduta"

                RicercaSedute()

            Case "btnAggiungiAssAssociate"

                Me.uscAggAss.ModaleName = "modAggAss"
                Me.uscAggAss.LoadModale()
                Me.modAggAss.VisibileMD = True
                Me.OnitLayout31.Busy = True

            Case "btnElimina"

                UnbindDataGrid()
                CancellaSelezionate()

            Case "btnEsegui"

                UnbindDataGrid()
                Esegui()

            Case "btnRecuperaStoricoVacc"

                RecuperaStoricoVaccinale()

        End Select

    End Sub

#End Region

#Region " Datagrid Events "

    Protected Sub dgrDettaglio_ItemDataBound(sender As Object, e As DataGridItemEventArgs)

        Select Case e.Item.ItemType

            Case ListItemType.Item, ListItemType.EditItem, ListItemType.AlternatingItem

                Dim currentRow As DataRowView = e.Item.DataItem

                ' Modale Condizione sanitaria
                Dim oml As OnitModalList = DirectCast(e.Item.FindControl("omlCondSanitaria"), OnitModalList)
                If Not oml Is Nothing Then

                    Dim codiceMalattia As String = currentRow("ves_mal_codice_cond_sanitaria").ToString()
                    Dim descrizioneMalattia As String = String.Empty

                    If Not String.IsNullOrWhiteSpace(codiceMalattia) Then

                        ' C'è un codice da associare => ricavo la descrizione

                        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                            Using bizMalattie As New BizMalattie(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                                descrizioneMalattia = bizMalattie.GetDescrizioneMalattia(codiceMalattia)

                            End Using
                        End Using

                    Else

                        ' Imposto la condizione sanitaria, tra quelle del paziente, in base alla vaccinazione associata.
                        Dim command As New BizMalattie.GetCondizioneSanitariaDefaultPazienteCommand()
                        command.CodiceVaccinazione = currentRow("ves_vac_codice").ToString()
                        command.CondizioniSanitariePaziente = Me.CondizioniSanitariePaziente
                        command.CodiceCondizioneSanitariaDefault = Me.Settings.CONDIZIONE_SANITARIA_DEFAULT
                        command.DescrizioneCondizioneSanitariaDefault = Me.DescrizioneMalattiaDefault
                        command.CodiceNessunaMalattia = Me.Settings.CODNOMAL

                        Dim condizioneSanitaria As Entities.CondizioneSanitaria = BizMalattie.GetCondizioneSanitariaDefaultPaziente(command)
                        codiceMalattia = condizioneSanitaria.CodiceMalattia
                        descrizioneMalattia = condizioneSanitaria.DescrizioneMalattia

                    End If

                    oml.Codice = codiceMalattia
                    oml.Descrizione = descrizioneMalattia
                    oml.ToolTip = descrizioneMalattia

                End If

                ' Modale Condizione rischio
                oml = DirectCast(e.Item.FindControl("omlCondRischio"), OnitModalList)
                If Not oml Is Nothing Then

                    Dim codiceRischio As String = currentRow("ves_rsc_codice").ToString()

                    If Not String.IsNullOrWhiteSpace(codiceRischio) Then

                        Dim descrizioneRischio As String = String.Empty
                        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                            Using bizRischio As New BizCategorieRischio(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())
                                descrizioneRischio = bizRischio.GetDescrizioneCategoriaRischio(codiceRischio)
                            End Using
                        End Using

                        oml.Codice = codiceRischio
                        oml.Descrizione = descrizioneRischio
                        oml.ToolTip = descrizioneRischio

                    Else
                        Dim codiceVaccinazione As String = currentRow("ves_vac_codice").ToString()
                        Dim VacInfluenzale As Boolean = False
                        ' Verifico se la vaccinazione 
                        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
                            Using bizVac As New BizVaccinazioniAnagrafica(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                                VacInfluenzale = bizVac.ExistsVaccinazioneAntiInfluenzale(codiceVaccinazione)

                            End Using
                        End Using
                        ' Valorizzazione condizione di rischio:
                        ' controllo il campo rischio del paziente:
                        '   - se il paziente ha 1 cond. rischio associata alla vaccinazione corrente => imposto quella
                        '   - altrimenti => imposto default (parametro)
                        If Not Me.CondizioniRischioPaziente Is Nothing AndAlso Me.CondizioniRischioPaziente.Count > 0 Then



                            ' Lista condizioni sanitarie del paziente relative alla vaccinazione della riga corrente
                            Dim list As List(Of Entities.CondizioneRischio) =
                                Me.CondizioniRischioPaziente.Where(Function(p) p.CodiceVaccinazione = codiceVaccinazione).ToList()

                            If Not list Is Nothing AndAlso list.Count = 1 Then
                                oml.Codice = list.First.CodiceCategoriaRischio
                                oml.Descrizione = list.First.DescrizioneCategoriaRischio
                                oml.ToolTip = list.First.DescrizioneCategoriaRischio
                            Else
                                If Not VacInfluenzale Then
                                    oml.Codice = Me.Settings.CONDIZIONE_RISCHIO_DEFAULT
                                    oml.Descrizione = Me.DescrizioneCategoriaRischioDefault
                                    oml.ToolTip = Me.DescrizioneCategoriaRischioDefault

                                End If
                            End If
                        Else
                            If Not VacInfluenzale Then
                                ' Nessuna condizione di rischio associata: imposto il default
                                oml.Codice = Me.Settings.CONDIZIONE_RISCHIO_DEFAULT
                                oml.Descrizione = Me.DescrizioneCategoriaRischioDefault
                                oml.ToolTip = Me.DescrizioneCategoriaRischioDefault
                            End If

                        End If

                    End If
                End If

        End Select

    End Sub

    Private Sub dgrVaccinazioni_ItemDataBound(sender As Object, e As DataGridItemEventArgs) Handles dgrVaccinazioni.ItemDataBound

        If e.Item.ItemIndex >= 0 Then
            Select Case e.Item.ItemType
                Case ListItemType.Item, ListItemType.EditItem, ListItemType.AlternatingItem
                    Dim cmbLuogo As OnitCombo = DirectCast(e.Item.FindControl("cmbLuogo"), OnitCombo)
                    If Not cmbLuogo Is Nothing AndAlso cmbLuogo.Visible Then

                        cmbLuogo.DataValueField = "Codice"
                        cmbLuogo.DataTextField = "Descrizione"
                        cmbLuogo.DataSource = LuoghiEsecuzione
                        cmbLuogo.DataBind()

                        SelectDdlValue(cmbLuogo, e.Item.DataItem("ves_luogo"))

                    End If

                    Dim cmbTipoErogatore As OnitCombo = DirectCast(e.Item.FindControl("cmbTipoErogatore"), OnitCombo)
                    If Not cmbTipoErogatore Is Nothing Then

                        Dim erogatori As New List(Of Entities.TipoErogatoreVacc)
                        erogatori.Add(New Entities.TipoErogatoreVacc() With {.Descrizione = ""})
                        erogatori.AddRange(LoadTipiErogatore(e.Item.DataItem("ves_luogo")))
                        cmbTipoErogatore.DataSource = erogatori
                        cmbTipoErogatore.DataBind()

                        SelectDdlValue(cmbTipoErogatore, e.Item.DataItem("ves_tipo_erogatore"))

                    End If

                    'Dim ddlTipoErogatore As OnitCombo = DirectCast(e.Item.FindControl("ddlTipoErogatore"), OnitCombo)
                    'If Not ddlTipoErogatore Is Nothing AndAlso ddlTipoErogatore.Visible Then

                    '    Dim erogatori As List(Of Entities.TipoErogatoreVacc) = LoadTipiErogatore(e.Item.DataItem("ves_luogo").ToString())

                    '    ddlTipoErogatore.DataValueField = "Codice"
                    '    ddlTipoErogatore.DataTextField = "Descrizione"
                    '    ddlTipoErogatore.DataSource = erogatori
                    '    ddlTipoErogatore.DataBind()

                    '    SelectDdlValue(cmbLuogo, e.Item.DataItem("ves_tipo_erogatore"))

                    'End If                    
            End Select

            RefreshModal(e.Item.FindControl("txtConsultorioItem"))
            'RefreshModal(e.Item.FindControl("txtLuogoItem"))
            'RefreshModal(e.Item.FindControl("txtMedicoItem"))

            ' Visualizzazione e valorizzazione Flag Visibilità.
            ' Se viene gestito il consenso, la valorizzazione avviene in base al valore presente nel datasource
            Dim chkFlagVisibilita As CheckBox = DirectCast(e.Item.FindControl("chkFlagVisibilita"), CheckBox)

            chkFlagVisibilita.Visible = True
            chkFlagVisibilita.Checked = (ValoreVisibilitaDefaultPaziente = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente)

            ' Visualizzazione Flag Fittizia
            Dim chkFittizia As CheckBox = DirectCast(e.Item.FindControl("chkFittizia"), CheckBox)

            If Settings.GESVACFITTIZIA Then
                chkFittizia.Visible = True
            Else
                chkFittizia.Visible = False
                chkFittizia.Checked = False
            End If

            ' Dettaglio
            Dim dgrDettaglio As DataGrid = DirectCast(e.Item.FindControl("dgrDettaglio"), DataGrid)
            Dim assCodice As String = DirectCast(e.Item.FindControl("lblAssociazioneItem"), Label).Text
            Dim assDose As String = DirectCast(e.Item.FindControl("txtAssDoseItem"), TextBox).Text

            Dim dv As DataView = New DataView(dtaVaccinazioni)

            dv.RowFilter = String.Format("ves_ass_codice='{0}' and ves_ass_n_dose = '{1}'", assCodice, assDose)

            dgrDettaglio.DataSource = dv
            dgrDettaglio.DataBind()

            SetLayout(e.Item)

        End If

    End Sub

#Region " Dati vaccinazione "

    Protected Sub btnDatiVac_Click(sender As Object, e As ImageClickEventArgs)

        Dim currentGridItem As DataGridItem = Me.GetCurrentDataGridVaccinazioniItem(sender, "btnDatiVac")
        Dim idCampiObbligatori As New List(Of String)
        ' L'associazione selezionata è eseguita se c'è l'immagine con la "E" a fine riga.
        Dim isEseguita As Boolean = False

        If Not currentGridItem Is Nothing Then

            ' Imposto l'indice della riga su cui sto lavorando
            Me.hidRowIndex.Value = currentGridItem.ItemIndex.ToString()

            'Data vaccinazione effettuata
            Dim dataVacEffettuata As Date? = DirectCast(currentGridItem.FindControl("txtDataItem"), Onit.Web.UI.WebControls.Validators.OnitDatePick).Data

            ' Imposto il codice dell'asspciazione relativa alla riga su cui sto lavorando
            Dim codiceAssociazione As String = DirectCast(currentGridItem.FindControl("lblAssociazioneItem"), Label).Text
            Me.hidCodiceAssociazione.Value = codiceAssociazione

            Dim control As Control = currentGridItem.FindControl("imgEseguita")
            If Not control Is Nothing Then
                isEseguita = control.Visible
            End If

            Dim datiAggiuntivi As BizRegistrazioneVaccinazioni.DatiAggiuntiviVaccinazione = GetDatiAggiuntiviFromDataGridItem(currentGridItem)

            If isEseguita Then
                '--
                ' Riga già eseguita => disabilito tutti i campi della modale
                '--
                SetLayoutCampiAggiuntivi(datiAggiuntivi, Nothing, idCampiObbligatori, dataVacEffettuata)
                '--
            Else

                Dim luogoSelezionato As Entities.LuoghiEsecuzioneVaccinazioni =
                    GetLuogoByComboLuogo(DirectCast(currentGridItem.FindControl("cmbLuogo"), OnitCombo))
                ' Lettura tipo luogo in base al codice impostato nella riga corrente
                If Not luogoSelezionato Is Nothing Then
                    idCampiObbligatori = GetCampiAggiuntiviObbligatori(luogoSelezionato.Codice, dataVacEffettuata)
                End If

                If luogoSelezionato Is Nothing Then
                    '--
                    ' Luogo non selezionato => disabilito tutti i campi della modale tranne le note
                    '--
                    SetLayoutCampiAggiuntivi(datiAggiuntivi, Nothing, idCampiObbligatori, dataVacEffettuata)
                    Me.txtNoteVac.Enabled = True
                    Me.txtNoteVac.CssClass = CSS_CLASS_TEXTBOX_STRINGA
                    '--
                Else
                    ' Riga non eseguita e luogo selezionato => abilito i campi aggiuntivi e li rendo obbligatori in base al luogo
                    '--
                    SetLayoutCampiAggiuntivi(datiAggiuntivi, luogoSelezionato, idCampiObbligatori, dataVacEffettuata)
                    '--
                    'Inizializzazione
                    Dim tipoErogatore As OnitCombo = DirectCast(currentGridItem.FindControl("cmbTipoErogatore"), OnitCombo)
                    Dim consultorio As OnitModalList = DirectCast(currentGridItem.FindControl("txtConsultorioItem"), OnitModalList)
                    consultorio.RefreshDataBind()

                    'Gestione campo Struttura => Dati Aggiuntivi
                    Dim filtroCodiceStruttura As New StringBuilder()
                    filtroCodiceStruttura.Append(" 1 = 1 ")

                    If Not tipoErogatore Is Nothing AndAlso Not String.IsNullOrWhiteSpace(tipoErogatore.SelectedValue) Then
                        filtroCodiceStruttura.Append(String.Format(" AND AST_TIPO_EROGATORE = '{0}' ", tipoErogatore.SelectedValue))
                    End If
                    If Not consultorio Is Nothing AndAlso Not String.IsNullOrWhiteSpace(consultorio.Codice) Then
                        filtroCodiceStruttura.Append(String.Format(" AND AST_CODICE = '{0}' ", consultorio.Codice))
                    End If

                    filtroCodiceStruttura.Append(" order by AST_DESCRIZIONE ")
                    omlStrutture.Filtro = filtroCodiceStruttura.ToString()

                    If String.IsNullOrWhiteSpace(datiAggiuntivi.CodiceStruttura) Then
                        datiAggiuntivi.CodiceStruttura = consultorio.Codice
                    End If

                    ' Filtro Nomi Commerciali in base all'associazione della riga selezionata
                    Dim filtroNomiCommerciali As New StringBuilder()
                    filtroNomiCommerciali.Append(" NOC_CODICE = NAL_NOC_CODICE ")
                    filtroNomiCommerciali.Append(" AND NOC_FOR_CODICE = FOR_CODICE (+) ")
                    filtroNomiCommerciali.AppendFormat(" AND NAL_ASS_CODICE = '{0}' ", codiceAssociazione)
                    filtroNomiCommerciali.Append(" ORDER BY NOC_DESCRIZIONE ")

                    omlNomeCommerciale.Filtro = filtroNomiCommerciali.ToString()

                End If

            End If

            ' Da valorizzare dopo aver impostato i filtri della modale comune_stato (che sono variabili a seconda del tipo luogo)
            SetCampiAggiuntivi(datiAggiuntivi, Not isEseguita)

            ' Apertura modale dati aggiuntivi
            Me.modDatiVac.VisibileMD = True

        End If

    End Sub

    Private Sub SetFiltroAsl(codiceLuogo As String)
        Dim filtroAsl As New System.Text.StringBuilder()
        filtroAsl.Append(" 1 = 1 AND LCU_USL_CODICE = USL_CODICE ")
        If Not String.IsNullOrWhiteSpace(codiceLuogo) Then
            filtroAsl.Append(String.Format(" AND LCU_COM_CODICE = '{0}' ", codiceLuogo))
        End If
        filtroAsl.Append(" ORDER BY USL_DESCRIZIONE")
        omlUsl.Filtro = filtroAsl.ToString()
    End Sub

    Private Sub SetFiltroComuneStato(codiceComune As String)
        Dim filtro As New System.Text.StringBuilder()
        filtro.Append(" 1 = 1 ")
        If Not String.IsNullOrWhiteSpace(codiceComune) Then
            filtro.Append(String.Format(" AND COM_CODICE = '{0}'", codiceComune))
        End If
        filtro.Append(" ORDER BY COM_DESCRIZIONE")
        omlComuneStato.Filtro = filtro.ToString()

    End Sub

#End Region

#End Region

#Region " User Control Events "

    Private Sub uscAggAss_InviaDati(dtCodici As DataTable, tipoAggiunta As String) Handles uscAggAss.InviaDati

        Using DAM As IDAM = OnVacUtility.OpenDam()

            UnbindDataGrid()

            For i As Int16 = 0 To dtCodici.Rows.Count - 1

                Dim assCodice As String = dtCodici.Rows(i)("codice")
                Dim dosiDaRegistrare As Integer = dtCodici.Rows(i)("dose")

                ' Aggiungo la dose cercando di riempire il buco nella sequenza
                For j As Integer = 1 To dosiDaRegistrare

                    Dim assDose As Integer = CalcolaDoseAssociazione(assCodice, DAM)
                    SplittaAssociazione(1, assCodice, assDose, DAM)

                Next

            Next

        End Using

        CheckPrime()

        Me.DataBindDatagrid(False)

    End Sub

    'riabilitazione di Left e TopBar tramite annullamento della modale (modifica 08/06/2004)
    Private Sub uscAggAss_AbilitaBar() Handles uscAggAss.AbilitaBar

        If Me.dgrVaccinazioni.Items.Count = 0 Then

            Me.OnitLayout31.Busy = False

        End If

    End Sub

#End Region

#Region " OnitLayout Events "

    Private Sub OnitLayout31_ConfirmClick(sender As Object, e As Controls.PagesLayout.OnitLayout3.ConfirmEventArgs) Handles OnitLayout31.ConfirmClick

        ' Eliminazione programmazione:
        '   1 - Prima cancella solo le cnv senza solleciti, appuntamenti e bilanci
        '   2 - Se ne rimangono, chiede esplicitamente se si vogliono eliminare anche quelle con appuntamenti, solleciti o bilanci associati

        Select Case e.Key

            Case KEY_ELIMINA_PROGRAMMAZIONE

                If e.Result Then

                    ' Eliminazione la programmazione del paziente, escluse le cnv con: 
                    ' appuntamenti, solleciti di bilancio o bilanci associati.
                    EliminaProgrammazioneVaccinale(False)

                    ' Verifica se sono rimaste delle convocazioni con appuntamento e/o solleciti e/o bilanci
                    If EsisteConvocazione() Then

                        Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(
                               "ATTENZIONE: sono rimaste convocazioni con appuntamenti, solleciti e/o bilanci:\nsi desidera eliminare la programmazione associata a queste convocazioni?",
                               KEY_ELIMINA_PROGRAMMAZIONE_APPUNTAMENTO, True, True))

                    Else

                        Me.OnitLayout31.InsertRoutineJS("alert('Programmazione eliminata con successo!');")

                    End If

                End If

            Case KEY_ELIMINA_PROGRAMMAZIONE_APPUNTAMENTO

                If e.Result Then

                    ' Eliminazione della programmazione del paziente.
                    ' Elimina anche le cnv con appuntamenti, solleciti di bilancio o bilanci associati.
                    EliminaProgrammazioneVaccinale(True)

                    Me.OnitLayout31.InsertRoutineJS("alert('Programmazione completamente eliminata con successo!');")

                Else

                    Me.OnitLayout31.InsertRoutineJS("alert('Programmazione SENZA appuntamenti e/o solleciti eliminata con successo!');")

                End If

        End Select

    End Sub

#End Region

#Region " Eliminazione Programmazione "

    Private Sub ControlloEliminazioneProgrammazione()

        'controllo se c'è una programmazione associata al paziente:
        'in questo caso occorre visualizzare un messaggio per la conferma dell'eliminazione programmazione [modifica 25/01/2005]
        If EsisteConvocazione() Then

            Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(
                                       "ATTENZIONE: al paziente è già associata una programmazione. Non è possibile quindi aggiungere le vaccinazioni già presenti tra le programmate.\nSi desidera eliminare tale programmazione?",
                                       KEY_ELIMINA_PROGRAMMAZIONE, True, True))
        End If

    End Sub

    'controlla se esiste una programmazione associata al paziente [modifica 25/01/2005]
    Private Function EsisteConvocazione() As Boolean

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Return genericProvider.Convocazione.Exists(OnVacUtility.Variabili.PazId)

        End Using

    End Function

    Private Sub EliminaProgrammazioneVaccinale(eliminaTutto As Boolean)

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            Using bizVaccinazioneProg As New BizVaccinazioneProg(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)

                Dim command As New BizVaccinazioneProg.EliminaProgrammazioneCommand()
                command.CodicePaziente = Convert.ToInt64(OnVacUtility.Variabili.PazId)
                command.DataConvocazione = Nothing
                command.EliminaAppuntamenti = eliminaTutto
                command.EliminaBilanci = eliminaTutto
                command.EliminaSollecitiBilancio = eliminaTutto
                command.TipoArgomentoLog = DataLogStructure.TipiArgomento.ELIMINA_PROG
                command.OperazioneAutomatica = False
                command.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.EliminazioneConvocazione
                command.NoteEliminazione = "Eliminazione convocazione paziente da maschera RegistrazioneVac"
                If eliminaTutto Then
                    command.NoteEliminazione += " (anche appuntamenti)"
                End If

                bizVaccinazioneProg.EliminaProgrammazione(command)

            End Using

        End Using

    End Sub

#End Region

#Region " Dati Aggiuntivi Vaccinazione "
    Protected Function GetDatiVacImageUrl(dataItem As DataRowView, enable As Boolean) As String

        Dim dati As BizRegistrazioneVaccinazioni.DatiAggiuntiviVaccinazione = GetDatiAggiuntiviFromDataRowView(dataItem)

        Return GetDatiVacImageUrl(dati, enable)

    End Function

    Private Function GetDatiVacImageUrl(datiAggiuntivi As BizRegistrazioneVaccinazioni.DatiAggiuntiviVaccinazione, enable As Boolean) As String

        If BizRegistrazioneVaccinazioni.ExistsDatiAggiuntivi(datiAggiuntivi) Then Return UrlIconaDatiSi(enable)

        Return UrlIconaDatiNo(enable)

    End Function

    Protected Function GetDatiVacToolTip(dataItem As DataRowView) As String

        Dim dati As BizRegistrazioneVaccinazioni.DatiAggiuntiviVaccinazione = GetDatiAggiuntiviFromDataRowView(dataItem)

        Return GetDatiVacToolTip(dati)

    End Function

    Private Function GetDatiVacToolTip(dati As BizRegistrazioneVaccinazioni.DatiAggiuntiviVaccinazione) As String

        If BizRegistrazioneVaccinazioni.ExistsDatiAggiuntivi(dati) Then Return "Dati aggiuntivi da registrare (PRESENTI)"

        Return "Dati aggiuntivi da registrare (NON PRESENTI)"

    End Function

    ''' <summary>
    ''' Valorizzo i filtri per omlStrutture, omlComuneStato, omlUsl; prendo anche i valori scaduti (no controlli su date)
    ''' </summary>
    ''' <param name="codiceStruttura"></param>
    ''' <param name="codiceLuogo"></param>
    ''' <param name="codiceUsl"></param>
    Private Sub SetLuogoAslDatiAggiuntiviFromStruttura(codiceStruttura As String, codiceLuogo As String, codiceUsl As String)
        omlStrutture.Codice = codiceStruttura
        omlStrutture.RefreshDataBind()

        If Not omlStrutture.IsValid() Then
            omlStrutture.Codice = String.Empty
            omlStrutture.RefreshDataBind()
        End If

        If Not String.IsNullOrWhiteSpace(omlStrutture.Codice) Then
            SetFiltroComuneStato(omlStrutture.ValoriAltriCampi("CodiceComune"))
        Else
            SetFiltroComuneStato(String.Empty)
        End If

        If String.IsNullOrWhiteSpace(codiceStruttura) Then
            omlComuneStato.Codice = String.Empty
        Else
            omlComuneStato.Codice = omlStrutture.ValoriAltriCampi("CodiceComune")
        End If
        omlComuneStato.RefreshDataBind()

        If Not omlComuneStato.IsValid() Then
            omlComuneStato.Codice = String.Empty
            omlComuneStato.RefreshDataBind()
        End If

        SetFiltroAsl(omlComuneStato.Codice)

        'Da analisi: se ho gia un codice usl => prendo un precedente, se ho la struttura => prendo quella
        If Not String.IsNullOrWhiteSpace(codiceUsl) Then
            omlUsl.Codice = codiceUsl
        Else
            If Not String.IsNullOrWhiteSpace(omlStrutture.Codice) Then
                omlUsl.Codice = omlStrutture.ValoriAltriCampi("Regione") + omlStrutture.ValoriAltriCampi("Asl")
            End If
        End If

        omlUsl.RefreshDataBind()

        If Not omlUsl.IsValid() Then
            omlUsl.Codice = String.Empty
            omlUsl.RefreshDataBind()
        End If
    End Sub

    Private Sub btnDatiVacOk_Click(sender As Object, e As System.EventArgs) Handles btnDatiVacOk.Click

        ' Indice della riga corrente
        Dim currentDataGridRowIndex As Int16 = Convert.ToInt16(Me.hidRowIndex.Value)

        ' Se la riga è già eseguita non aggiorno il datagrid
        Dim isEseguita As Boolean = False

        Dim control As Control = Me.dgrVaccinazioni.Items(currentDataGridRowIndex).FindControl("imgEseguita")
        If Not control Is Nothing Then
            isEseguita = control.Visible
        End If

        If Not isEseguita Then
            ' Associazione relativa alla riga corrente
            Dim codiceAssociazione As String = Me.hidCodiceAssociazione.Value

            ' Aggiorno i campi nel datagrid // Se voglio replicare i dati => uso ReplicaDatiAggiuntiviDataGrid
            AggiornaDatiAggiuntiviDataGrid(currentDataGridRowIndex, codiceAssociazione)

            ' Aggiorno il datatable
            UnbindDataGrid()
        End If

        ' Svuoto i campi contenenti indice corrente e associazione, e tutti i campi della modale
        Me.hidRowIndex.Value = String.Empty
        Me.hidCodiceAssociazione.Value = String.Empty
        SetCampiAggiuntivi(Nothing, False)

        ' Chiusura modale
        Me.modDatiVac.VisibileMD = False

    End Sub

    Private Sub btnDatiVacAnnulla_Click(sender As Object, e As System.EventArgs) Handles btnDatiVacAnnulla.Click

        ' Svuoto i campi contenenti indice corrente e associazione, e tutti i campi della modale
        Me.hidRowIndex.Value = String.Empty
        Me.hidCodiceAssociazione.Value = String.Empty
        SetCampiAggiuntivi(Nothing, False)

        ' Chiusura modale
        Me.modDatiVac.VisibileMD = False

    End Sub

    Private Sub ReplicaDatiAggiuntiviDataGrid(currentDataGridRowIndex As Integer, codiceAssociazione As String)

        ' Lettura dati inseriti dall'utente
        Dim datiAggiuntivi As BizRegistrazioneVaccinazioni.DatiAggiuntiviVaccinazione = GetDatiAggiuntiviFromControls()

        ' Se il campo note supera la maxLenght impostata, vengono tagliati i caratteri in eccedenza.
        If datiAggiuntivi.Note.Length > Me.txtNoteVac.MaxLength Then
            datiAggiuntivi.Note = datiAggiuntivi.Note.Substring(0, Me.txtNoteVac.MaxLength)
        End If

        Me.txtNoteVac.Text = datiAggiuntivi.Note

        Dim currentGridItem As DataGridItem = Me.dgrVaccinazioni.Items(currentDataGridRowIndex)

        Dim chk As CheckBox = DirectCast(currentGridItem.FindControl("chkSelezionaItem"), CheckBox)

        If chk.Checked Then

            ' Aggiorno i dati per tutte le righe selezionate
            For i As Int16 = 0 To Me.dgrVaccinazioni.Items.Count - 1

                chk = DirectCast(Me.dgrVaccinazioni.Items(i).FindControl("chkSelezionaItem"), CheckBox)

                ' Aggiorno il nome commerciale solo se è relativo alla stessa associazione della riga corrente
                Dim codiceAssociazioneRigaCorrente As String = DirectCast(Me.dgrVaccinazioni.Items(i).FindControl("lblAssociazioneItem"), Label).Text

                Dim aggiornaNomeCommerciale As Boolean = False
                If Not String.IsNullOrWhiteSpace(codiceAssociazioneRigaCorrente) Then
                    aggiornaNomeCommerciale = (codiceAssociazioneRigaCorrente = codiceAssociazione)
                End If

                If chk.Checked Then
                    AggiornaDatiAggiuntiviRiga(Me.dgrVaccinazioni.Items(i), datiAggiuntivi, aggiornaNomeCommerciale)
                End If

            Next

        Else

            ' Aggiorno i dati solo per la riga corrente
            AggiornaDatiAggiuntiviRiga(currentGridItem, datiAggiuntivi, True)

        End If

    End Sub

    Private Sub AggiornaDatiAggiuntiviDataGrid(currentDataGridRowIndex As Integer, codiceAssociazione As String)

        Dim datiAggiuntivi As BizRegistrazioneVaccinazioni.DatiAggiuntiviVaccinazione = GetDatiAggiuntiviFromControls()

        ' Se il campo note supera la maxLenght impostata, vengono tagliati i caratteri in eccedenza.
        If datiAggiuntivi.Note.Length > Me.txtNoteVac.MaxLength Then
            datiAggiuntivi.Note = datiAggiuntivi.Note.Substring(0, Me.txtNoteVac.MaxLength)
        End If

        Me.txtNoteVac.Text = datiAggiuntivi.Note

        Dim currentGridItem As DataGridItem = Me.dgrVaccinazioni.Items(currentDataGridRowIndex)

        Dim chk As CheckBox = DirectCast(currentGridItem.FindControl("chkSelezionaItem"), CheckBox)

        'If chk.Checked Then

        '    ' Aggiorno i dati per tutte le righe selezionate
        '    For i As Int16 = 0 To Me.dgrVaccinazioni.Items.Count - 1

        '        chk = DirectCast(Me.dgrVaccinazioni.Items(i).FindControl("chkSelezionaItem"), CheckBox)

        '        ' Aggiorno il nome commerciale solo se è relativo alla stessa associazione della riga corrente
        '        Dim codiceAssociazioneRigaCorrente As String = DirectCast(Me.dgrVaccinazioni.Items(i).FindControl("lblAssociazioneItem"), Label).Text

        '        Dim aggiornaNomeCommerciale As Boolean = False
        '        If Not String.IsNullOrWhiteSpace(codiceAssociazioneRigaCorrente) Then
        '            aggiornaNomeCommerciale = (codiceAssociazioneRigaCorrente = codiceAssociazione)
        '        End If

        '        If chk.Checked Then
        '            AggiornaDatiAggiuntiviRiga(Me.dgrVaccinazioni.Items(i), datiAggiuntivi, aggiornaNomeCommerciale)
        '        End If

        '    Next

        'Else

        ' Aggiorno i dati solo per la riga corrente
        AggiornaDatiAggiuntiviRiga(currentGridItem, datiAggiuntivi, True)

        'End If

    End Sub

    Private Sub AggiornaDatiAggiuntiviRiga(gridCurrentItem As DataGridItem, dati As BizRegistrazioneVaccinazioni.DatiAggiuntiviVaccinazione, aggiornaNomeCommerciale As Boolean)

        Dim datiDaReplicare As BizRegistrazioneVaccinazioni.DatiAggiuntiviVaccinazione = dati.Clone()

        If Not aggiornaNomeCommerciale Then
            ' Se non deve aggiornare il nome commerciale, lo sostituisce con quello già esistente nella riga
            datiDaReplicare.CodiceNomeCommerciale = HttpUtility.HtmlDecode(gridCurrentItem.Cells(IndexDgrVac.NomeCommerciale).Text).Trim()
        End If

        gridCurrentItem.Cells(IndexDgrVac.NomeCommerciale).Text = HttpUtility.HtmlEncode(datiDaReplicare.CodiceNomeCommerciale)
        gridCurrentItem.Cells(IndexDgrVac.CodiceLotto).Text = HttpUtility.HtmlEncode(datiDaReplicare.CodiceLotto)
        gridCurrentItem.Cells(IndexDgrVac.DataScadenzaLotto).Text = HttpUtility.HtmlEncode(datiDaReplicare.DataScadenzaLotto)
        gridCurrentItem.Cells(IndexDgrVac.ViaSomministrazione).Text = HttpUtility.HtmlEncode(datiDaReplicare.CodiceViaSomministrazione)
        gridCurrentItem.Cells(IndexDgrVac.SitoInoculazione).Text = HttpUtility.HtmlEncode(datiDaReplicare.CodiceSitoInoculazione)
        gridCurrentItem.Cells(IndexDgrVac.MedicoResponsabile).Text = HttpUtility.HtmlEncode(datiDaReplicare.CodiceMedicoResponsabile)
        gridCurrentItem.Cells(IndexDgrVac.Vaccinatore).Text = HttpUtility.HtmlEncode(datiDaReplicare.CodiceVaccinatore)
        gridCurrentItem.Cells(IndexDgrVac.Comune_Stato).Text = HttpUtility.HtmlEncode(datiDaReplicare.CodiceComuneStato)
        gridCurrentItem.Cells(IndexDgrVac.Note).Text = HttpUtility.HtmlEncode(datiDaReplicare.Note)
        gridCurrentItem.Cells(IndexDgrVac.Tipo_Comune_Stato).Text = HttpUtility.HtmlEncode(datiDaReplicare.TipoComuneStato)
        gridCurrentItem.Cells(IndexDgrVac.CodiceStruttura).Text = HttpUtility.HtmlEncode(datiDaReplicare.CodiceStruttura)
        gridCurrentItem.Cells(IndexDgrVac.CodiceUsl).Text = HttpUtility.HtmlEncode(datiDaReplicare.CodiceUsl)

        ' Aggiorna icona
        Dim btnDatiVac As ImageButton = DirectCast(gridCurrentItem.FindControl("btnDatiVac"), ImageButton)
        btnDatiVac.ImageUrl = GetDatiVacImageUrl(datiDaReplicare, True)
        btnDatiVac.ToolTip = GetDatiVacToolTip(datiDaReplicare)

    End Sub

    Private Sub AggiornaDatiPagamentoRiga(gridCurrentItem As DataGridItem, dati As BizRegistrazioneVaccinazioni.DatiPagamentoVaccinazione)

        Dim datiDaReplicare As BizRegistrazioneVaccinazioni.DatiPagamentoVaccinazione = dati.Clone()

        gridCurrentItem.Cells(IndexDgrVac.GuidTipoPagamento).Text = HttpUtility.HtmlEncode(datiDaReplicare.GuidTipoPagamento)
        gridCurrentItem.Cells(IndexDgrVac.CodiceEsenzione).Text = HttpUtility.HtmlEncode(datiDaReplicare.CodiceEsenzione)
        gridCurrentItem.Cells(IndexDgrVac.CodiceMalattia).Text = HttpUtility.HtmlEncode(datiDaReplicare.CodiceMalattia)
        gridCurrentItem.Cells(IndexDgrVac.Importo).Text = HttpUtility.HtmlEncode(datiDaReplicare.Importo)

    End Sub

    Protected Function GetAssociazioniTooltip(datiItem As DataRowView) As String
        Dim toolTipAssociazioni As String = String.Empty
        If Not datiItem Is Nothing Then
            toolTipAssociazioni = String.Format("{0} - {1}", datiItem("ves_ass_codice").ToString(), datiItem("ass_descrizione").ToString())
        End If
        Return toolTipAssociazioni
    End Function

    Private Sub SetLayoutCampiAggiuntivi(datiAggiuntivi As BizRegistrazioneVaccinazioni.DatiAggiuntiviVaccinazione, luogoSelezionato As Entities.LuoghiEsecuzioneVaccinazioni, campiObbligatori As List(Of String), dataEffettuazioneVac As Date?)

        If Not dataEffettuazioneVac.HasValue Then
            dataEffettuazioneVac = Date.MinValue
        End If
        If luogoSelezionato Is Nothing OrElse dataEffettuazioneVac.Value = Date.MinValue Then

            Me.omlNomeCommerciale.Enabled = False
            Me.omlNomeCommerciale.Obbligatorio = False

            dpkDataScadenzaLotto.Enabled = False
            dpkDataScadenzaLotto.CssClass = CSS_CLASS_TEXTBOX_STRINGA_DISABILITATO

            Me.txtLotto.Enabled = False
            Me.txtLotto.CssClass = CSS_CLASS_TEXTBOX_STRINGA_DISABILITATO

            Me.ddlVii.Enabled = False
            Me.ddlVii.CssClass = CSS_CLASS_TEXTBOX_STRINGA_DISABILITATO

            Me.ddlSii.Enabled = False
            Me.ddlSii.CssClass = CSS_CLASS_TEXTBOX_STRINGA_DISABILITATO

            Me.omlMedicoResp.Enabled = False
            Me.omlMedicoResp.CssClass = CSS_CLASS_TEXTBOX_STRINGA_DISABILITATO

            Me.omlVaccinatore.Enabled = False
            Me.omlVaccinatore.CssClass = CSS_CLASS_TEXTBOX_STRINGA_DISABILITATO

            Me.omlComuneStato.Enabled = False
            Me.omlComuneStato.CssClass = CSS_CLASS_TEXTBOX_STRINGA_DISABILITATO

            Me.txtNoteVac.Enabled = False
            Me.txtNoteVac.CssClass = CSS_CLASS_TEXTBOX_STRINGA_DISABILITATO

            Me.omlStrutture.Enabled = False
            Me.omlStrutture.CssClass = CSS_CLASS_TEXTBOX_STRINGA_DISABILITATO

            Me.omlUsl.Enabled = False
            Me.omlUsl.CssClass = CSS_CLASS_TEXTBOX_STRINGA_DISABILITATO

        Else

            Me.omlNomeCommerciale.Enabled = True
            Me.omlNomeCommerciale.Obbligatorio = BizRegistrazioneVaccinazioni.IsCampoObbligatorio(Me.omlNomeCommerciale.ID, campiObbligatori)

            dpkDataScadenzaLotto.Enabled = True
            If BizRegistrazioneVaccinazioni.IsCampoObbligatorio(Me.dpkDataScadenzaLotto.ID, campiObbligatori) Then
                dpkDataScadenzaLotto.CssClass = CSS_CLASS_TEXTBOX_STRINGA_OBBLIGATORIO
            Else
                dpkDataScadenzaLotto.CssClass = CSS_CLASS_TEXTBOX_STRINGA
            End If

            Me.txtLotto.Enabled = True

            If BizRegistrazioneVaccinazioni.IsCampoObbligatorio(Me.txtLotto.ID, campiObbligatori) Then
                Me.txtLotto.CssClass = CSS_CLASS_TEXTBOX_STRINGA_OBBLIGATORIO
            Else
                Me.txtLotto.CssClass = CSS_CLASS_TEXTBOX_STRINGA
            End If


            Me.ddlVii.Enabled = True
            If BizRegistrazioneVaccinazioni.IsCampoObbligatorio(Me.ddlVii.ID, campiObbligatori) Then
                Me.ddlVii.CssClass = CSS_CLASS_TEXTBOX_STRINGA_OBBLIGATORIO
            Else
                Me.ddlVii.CssClass = CSS_CLASS_TEXTBOX_STRINGA
            End If

            Me.ddlSii.Enabled = True
            If BizRegistrazioneVaccinazioni.IsCampoObbligatorio(Me.ddlSii.ID, campiObbligatori) Then
                Me.ddlSii.CssClass = CSS_CLASS_TEXTBOX_STRINGA_OBBLIGATORIO
            Else
                Me.ddlSii.CssClass = CSS_CLASS_TEXTBOX_STRINGA
            End If

            Me.omlMedicoResp.Enabled = True
            Me.omlMedicoResp.Obbligatorio = BizRegistrazioneVaccinazioni.IsCampoObbligatorio(Me.omlMedicoResp.ID, campiObbligatori)

            Me.omlVaccinatore.Enabled = True
            Me.omlVaccinatore.Obbligatorio = BizRegistrazioneVaccinazioni.IsCampoObbligatorio(Me.omlVaccinatore.ID, campiObbligatori)

            Me.omlComuneStato.Enabled = True
            Me.omlComuneStato.Obbligatorio = BizRegistrazioneVaccinazioni.IsCampoObbligatorio(Me.omlComuneStato.ID, campiObbligatori)

            Me.omlStrutture.Enabled = True
            Me.omlStrutture.Obbligatorio = BizRegistrazioneVaccinazioni.IsCampoObbligatorio(Me.omlStrutture.ID, campiObbligatori)

            Me.omlUsl.Enabled = True
            Me.omlUsl.Obbligatorio = BizRegistrazioneVaccinazioni.IsCampoObbligatorio(Me.omlUsl.ID, campiObbligatori)

            Me.txtNoteVac.Enabled = True
            If BizRegistrazioneVaccinazioni.IsCampoObbligatorio(Me.txtNoteVac.ID, campiObbligatori) Then
                Me.txtNoteVac.CssClass = CSS_CLASS_TEXTBOX_STRINGA_OBBLIGATORIO
            Else
                Me.txtNoteVac.CssClass = CSS_CLASS_TEXTBOX_STRINGA
            End If

        End If

    End Sub

    Private Function GetDatiAggiuntiviFromControls() As BizRegistrazioneVaccinazioni.DatiAggiuntiviVaccinazione


        Dim dati As New BizRegistrazioneVaccinazioni.DatiAggiuntiviVaccinazione()
        dati.CodiceNomeCommerciale = HttpUtility.HtmlDecode(Me.omlNomeCommerciale.Codice)
        dati.CodiceLotto = HttpUtility.HtmlDecode(Me.txtLotto.Text)
        dati.DataScadenzaLotto = HttpUtility.HtmlDecode(Me.dpkDataScadenzaLotto.Text)
        dati.CodiceSitoInoculazione = Me.ddlSii.SelectedValue
        dati.CodiceViaSomministrazione = Me.ddlVii.SelectedValue
        dati.CodiceMedicoResponsabile = HttpUtility.HtmlDecode(Me.omlMedicoResp.Codice)
        dati.CodiceVaccinatore = HttpUtility.HtmlDecode(Me.omlVaccinatore.Codice)
        dati.CodiceComuneStato = HttpUtility.HtmlDecode(Me.omlComuneStato.Codice)
        dati.Note = HttpUtility.HtmlDecode(Me.txtNoteVac.Text.Trim())
        dati.TipoComuneStato = HttpUtility.HtmlDecode(Me.hidTipoComuneStato.Value)
        dati.CodiceStruttura = HttpUtility.HtmlDecode(omlStrutture.Codice)
        dati.CodiceUsl = HttpUtility.HtmlDecode(omlUsl.Codice)

        Return dati

    End Function

    Private Function GetDatiAggiuntiviFromDataGridItem(currentGridItem As DataGridItem) As BizRegistrazioneVaccinazioni.DatiAggiuntiviVaccinazione

        Dim dati As New BizRegistrazioneVaccinazioni.DatiAggiuntiviVaccinazione()
        dati.CodiceNomeCommerciale = HttpUtility.HtmlDecode(currentGridItem.Cells(IndexDgrVac.NomeCommerciale).Text).Trim()
        dati.CodiceLotto = HttpUtility.HtmlDecode(currentGridItem.Cells(IndexDgrVac.CodiceLotto).Text).Trim()
        dati.DataScadenzaLotto = HttpUtility.HtmlDecode(currentGridItem.Cells(IndexDgrVac.DataScadenzaLotto).Text).Trim()
        dati.CodiceViaSomministrazione = HttpUtility.HtmlDecode(currentGridItem.Cells(IndexDgrVac.ViaSomministrazione).Text).Trim()
        dati.CodiceSitoInoculazione = HttpUtility.HtmlDecode(currentGridItem.Cells(IndexDgrVac.SitoInoculazione).Text).Trim()
        dati.CodiceMedicoResponsabile = HttpUtility.HtmlDecode(currentGridItem.Cells(IndexDgrVac.MedicoResponsabile).Text).Trim()
        dati.CodiceVaccinatore = HttpUtility.HtmlDecode(currentGridItem.Cells(IndexDgrVac.Vaccinatore).Text).Trim()
        dati.CodiceComuneStato = HttpUtility.HtmlDecode(currentGridItem.Cells(IndexDgrVac.Comune_Stato).Text).Trim()
        dati.Note = HttpUtility.HtmlDecode(currentGridItem.Cells(IndexDgrVac.Note).Text).Trim()
        dati.TipoComuneStato = HttpUtility.HtmlDecode(currentGridItem.Cells(IndexDgrVac.Tipo_Comune_Stato).Text).Trim()
        dati.CodiceStruttura = HttpUtility.HtmlDecode(currentGridItem.Cells(IndexDgrVac.CodiceStruttura).Text).Trim()
        dati.CodiceUsl = HttpUtility.HtmlDecode(currentGridItem.Cells(IndexDgrVac.CodiceUsl).Text).Trim()

        Return dati

    End Function

    Private Sub SetCampiAggiuntivi(dati As BizRegistrazioneVaccinazioni.DatiAggiuntiviVaccinazione, autocompletaStrutturaLuogoAsl As Boolean)

        If dati Is Nothing Then

            omlNomeCommerciale.Codice = String.Empty
            omlNomeCommerciale.Descrizione = String.Empty
            omlNomeCommerciale.RefreshDataBind()

            txtLotto.Text = String.Empty

            dpkDataScadenzaLotto.Text = String.Empty

            ddlVii.ClearSelection()
            ddlSii.ClearSelection()

            omlMedicoResp.Codice = String.Empty
            omlMedicoResp.Descrizione = String.Empty
            omlMedicoResp.RefreshDataBind()

            omlVaccinatore.Codice = String.Empty
            omlVaccinatore.Descrizione = String.Empty
            omlVaccinatore.RefreshDataBind()

            omlComuneStato.Codice = String.Empty
            omlComuneStato.Descrizione = String.Empty
            omlComuneStato.RefreshDataBind()

            txtNoteVac.Text = String.Empty

            hidTipoComuneStato.Value = String.Empty

            omlStrutture.Codice = String.Empty
            omlStrutture.Descrizione = String.Empty
            omlStrutture.RefreshDataBind()

            omlUsl.Codice = String.Empty
            omlUsl.Descrizione = String.Empty
            omlUsl.RefreshDataBind()

        Else

            Me.omlNomeCommerciale.Codice = dati.CodiceNomeCommerciale
            Me.omlNomeCommerciale.RefreshDataBind()

            Me.txtLotto.Text = dati.CodiceLotto

            dpkDataScadenzaLotto.Text = dati.DataScadenzaLotto

            SelectDdlValue(Me.ddlVii, dati.CodiceViaSomministrazione)
            SelectDdlValue(Me.ddlSii, dati.CodiceSitoInoculazione)

            Me.omlMedicoResp.Codice = dati.CodiceMedicoResponsabile
            Me.omlMedicoResp.RefreshDataBind()

            Me.omlVaccinatore.Codice = dati.CodiceVaccinatore
            Me.omlVaccinatore.RefreshDataBind()

            ' Il filtro della modale potrebbe essere stato modificato dalla scelta del luogo: 
            ' assegnando il codice potrebbe non trovare la descrizione, quindi controllo la validità ed eventualmente lo sbianco
            If Not Me.omlComuneStato.IsValid Then
                Me.omlComuneStato.Codice = String.Empty
                Me.omlComuneStato.Descrizione = String.Empty
                Me.omlComuneStato.RefreshDataBind()
            End If

            Me.txtNoteVac.Text = dati.Note

            Me.hidTipoComuneStato.Value = dati.TipoComuneStato

            If autocompletaStrutturaLuogoAsl Then
                SetLuogoAslDatiAggiuntiviFromStruttura(dati.CodiceStruttura, dati.CodiceComuneStato, dati.CodiceUsl)
            Else
                omlStrutture.Codice = dati.CodiceStruttura
                omlStrutture.RefreshDataBind()

                If Not omlStrutture.IsValid() Then
                    omlStrutture.Codice = String.Empty
                    omlStrutture.RefreshDataBind()
                End If

                Me.omlComuneStato.Codice = dati.CodiceComuneStato
                Me.omlComuneStato.RefreshDataBind()
                If Not Me.omlComuneStato.IsValid Then
                    Me.omlComuneStato.Codice = String.Empty
                    Me.omlComuneStato.Descrizione = String.Empty
                    Me.omlComuneStato.RefreshDataBind()
                End If

                omlUsl.Codice = dati.CodiceUsl
                omlUsl.RefreshDataBind()
                If Not omlUsl.IsValid() Then
                    omlUsl.Codice = String.Empty
                    omlUsl.RefreshDataBind()
                End If
            End If

        End If

    End Sub

    Private Function GetDatiAggiuntiviFromDataRowView(dataItem As DataRowView) As BizRegistrazioneVaccinazioni.DatiAggiuntiviVaccinazione

        Dim dati As BizRegistrazioneVaccinazioni.DatiAggiuntiviVaccinazione = Nothing

        If Not dataItem Is Nothing Then

            dati = New BizRegistrazioneVaccinazioni.DatiAggiuntiviVaccinazione()
            dati.CodiceNomeCommerciale = dataItem("ves_noc_codice").ToString()
            dati.CodiceLotto = dataItem("ves_lot_codice").ToString()
            dati.CodiceViaSomministrazione = dataItem("ves_vii_codice").ToString()
            dati.CodiceSitoInoculazione = dataItem("ves_sii_codice").ToString()
            dati.CodiceMedicoResponsabile = dataItem("ves_ope_codice").ToString()
            dati.CodiceVaccinatore = dataItem("ves_med_vaccinante").ToString()
            dati.CodiceComuneStato = dataItem("ves_comune_o_stato").ToString()
            dati.Note = dataItem("ves_note").ToString()
            dati.TipoComuneStato = dataItem("tipo_comune_o_stato").ToString()
            dati.CodiceStruttura = dataItem("struttura").ToString()
            dati.CodiceUsl = dataItem("ves_usl_cod_somministrazione").ToString()

        End If

        Return dati

    End Function

    Private Function LinkControlliCampiObbligatori() As List(Of ControlloCampoObbligatorio)
        Dim result As New List(Of ControlloCampoObbligatorio)
        Dim campiObbl As New List(Of String)
        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using bizAnag As New Biz.BizLuoghiEsecuzioneVaccinazioni(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                campiObbl = bizAnag.GetCampiObbligatori().Select(Function(f) f.CodCampo).ToList()
            End Using
        End Using

        For Each i As String In campiObbl
            Select Case i
                Case BizLuoghiEsecuzioneVaccinazioni.CampiObbligatori.CondizioneSanitaria
                    result.Add(New ControlloCampoObbligatorio With {.CampoObbligatorio = i, .Controllo = "omlCondSanitaria"})
                Case BizLuoghiEsecuzioneVaccinazioni.CampiObbligatori.CategoriaRischio
                    result.Add(New ControlloCampoObbligatorio With {.CampoObbligatorio = i, .Controllo = "omlCondRischio"})
                Case BizLuoghiEsecuzioneVaccinazioni.CampiObbligatori.CentroVaccinale
                    result.Add(New ControlloCampoObbligatorio With {.CampoObbligatorio = i, .Controllo = "txtConsultorioItem"})
                Case BizLuoghiEsecuzioneVaccinazioni.CampiObbligatori.NomeCommerciale
                    result.Add(New ControlloCampoObbligatorio With {.CampoObbligatorio = i, .Controllo = omlNomeCommerciale.ID})
                Case BizLuoghiEsecuzioneVaccinazioni.CampiObbligatori.CodiceLotto
                    result.Add(New ControlloCampoObbligatorio With {.CampoObbligatorio = i, .Controllo = txtLotto.ID})
                Case BizLuoghiEsecuzioneVaccinazioni.CampiObbligatori.DataScadenzaLotto
                    result.Add(New ControlloCampoObbligatorio With {.CampoObbligatorio = i, .Controllo = dpkDataScadenzaLotto.ID})
                Case BizLuoghiEsecuzioneVaccinazioni.CampiObbligatori.ViaSomministrazione
                    result.Add(New ControlloCampoObbligatorio With {.CampoObbligatorio = i, .Controllo = ddlVii.ID})
                Case BizLuoghiEsecuzioneVaccinazioni.CampiObbligatori.SitoInoculazione
                    result.Add(New ControlloCampoObbligatorio With {.CampoObbligatorio = i, .Controllo = ddlSii.ID})
                Case BizLuoghiEsecuzioneVaccinazioni.CampiObbligatori.MedicoResponsabile
                    result.Add(New ControlloCampoObbligatorio With {.CampoObbligatorio = i, .Controllo = omlMedicoResp.ID})
                Case BizLuoghiEsecuzioneVaccinazioni.CampiObbligatori.Vaccinatore
                    result.Add(New ControlloCampoObbligatorio With {.CampoObbligatorio = i, .Controllo = omlVaccinatore.ID})
                Case BizLuoghiEsecuzioneVaccinazioni.CampiObbligatori.CodiceStruttura
                    result.Add(New ControlloCampoObbligatorio With {.CampoObbligatorio = i, .Controllo = omlStrutture.ID})
                Case BizLuoghiEsecuzioneVaccinazioni.CampiObbligatori.Luogo
                    result.Add(New ControlloCampoObbligatorio With {.CampoObbligatorio = i, .Controllo = omlComuneStato.ID})
                Case BizLuoghiEsecuzioneVaccinazioni.CampiObbligatori.Asl
                    result.Add(New ControlloCampoObbligatorio With {.CampoObbligatorio = i, .Controllo = omlUsl.ID})
                Case BizLuoghiEsecuzioneVaccinazioni.CampiObbligatori.TipoErogatore
                    result.Add(New ControlloCampoObbligatorio With {.CampoObbligatorio = i, .Controllo = "cmbTIpoErogatore"})
            End Select
        Next

        Return result

    End Function

#End Region

#Region " Condizione Sanitaria e Condizione di Rischio "

    Protected Sub omlCondSanitaria_SetUpFilter(sender As Object)

        Dim codiceVaccinazioneCorrente As String = Me.GetCodiceVaccinazioneCurrentRow(sender, "omlCondSanitaria")

        Dim omlCondSanitaria As OnitModalList = DirectCast(sender, OnitModalList)

        omlCondSanitaria.Filtro =
            String.Format("VCS_PAZ_CODICE = {0} AND VCS_VAC_CODICE = '{1}' ORDER BY Paz, Def, Descrizione",
                          OnVacUtility.Variabili.PazId, codiceVaccinazioneCorrente)

    End Sub

    Protected Sub omlCondSanitaria_Change(sender As Object, e As OnitModalList.ModalListaEventArgument)

        Dim omlCondSanitaria As OnitModalList = DirectCast(sender, OnitModalList)
        omlCondSanitaria.ToolTip = omlCondSanitaria.Descrizione

    End Sub

    Protected Sub omlCondRischio_SetUpFilter(sender As Object)

        Dim codiceVaccinazioneCorrente As String = Me.GetCodiceVaccinazioneCurrentRow(sender, "omlCondRischio")

        Dim omlCondRischio As OnitModalList = DirectCast(sender, OnitModalList)

        omlCondRischio.Filtro =
            String.Format("VCR_PAZ_CODICE = {0} AND VCR_VAC_CODICE = '{1}' ORDER BY Paz, Def, Descrizione",
                          OnVacUtility.Variabili.PazId, codiceVaccinazioneCorrente)

    End Sub

    Protected Sub omlCondRischio_Change(sender As Object, e As OnitModalList.ModalListaEventArgument)

        Dim omlCondRischio As OnitModalList = DirectCast(sender, OnitModalList)
        omlCondRischio.ToolTip = omlCondRischio.Descrizione

    End Sub

#End Region


#Region " DataGrid "

    Private Function GetCurrentDataGridVacProgItem(sender As Object, controlId As String) As DataGridItem

        For Each item As DataGridItem In dgrVaccinazioni.Items

            If item.FindControl(controlId) Is sender Then
                Return item
            End If

        Next

        Return Nothing

    End Function

    Protected Sub lnkPagVac_Click(sender As Object, e As EventArgs)

        Dim dati As New BizRegistrazioneVaccinazioni.DatiPagamentoVaccinazione
        Dim currentGridItem As DataGridItem = Me.GetCurrentDataGridVacProgItem(sender, "lnkPagVac")
        If Not currentGridItem Is Nothing Then
            hdRowIndexPagVac.Value = currentGridItem.ItemIndex.ToString()
            modPagVac.VisibileMD = True
            dati = GetDatiPagamentoFromDataGridItem(currentGridItem)
            ucPagamentoVaccinazione.SetDatiPagamentoControls(dati)
        End If
    End Sub

    Protected Sub cmbTipoErogatore_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim currentGridItem As DataGridItem = Me.GetCurrentDataGridVaccinazioniItem(sender, "cmbTipoErogatore")
        SetLayout(currentGridItem)
    End Sub

    Private Function LoadTipiErogatore(codiceLuogo As String) As List(Of Entities.TipoErogatoreVacc)

        Dim result As New List(Of Entities.TipoErogatoreVacc)

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using biz As New Biz.BizErogatori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                result = biz.GetTipiErogatoriFromLuogoEsecuzione(codiceLuogo)

            End Using
        End Using

        Return result

    End Function

#End Region

#Region " Private "

    Private Sub InizializzazionePagina()

        OnVacUtility.ImpostaIntestazioniPagina(OnitLayout31, LayoutTitolo, Nothing, Settings, IsGestioneCentrale)

        CaricamentoDati()

        ' Visibilità pulsante Recupera Storico 
        ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
        Toolbar.FindItemByValue("btnRecuperaStoricoVacc").Visible = FlagConsensoVaccUslCorrente

        ' Valorizzazione flag visibilità dati vaccinali: in base al consenso del paziente, letto una sola volta (in questo momento)
        ' e impostato nel default della colonna del datatable (che verrà utilizzato come datasource per il datagrid).
        Dim valoreVisibilitaDatiVaccinaliDefault As String = GetValoreVisibilitaDatiVaccinaliDefault(OnVacUtility.Variabili.PazId)

        Dim key(1) As DataColumn

        If Not dtaVaccinazioni Is Nothing Then dtaVaccinazioni = Nothing

        dtaVaccinazioni = New DataTable()

        dtaVaccinazioni.Columns.Add("vac_descrizione", GetType(String))
        dtaVaccinazioni.Columns.Add("ves_vac_codice", GetType(String))
        dtaVaccinazioni.Columns.Add("ves_n_richiamo", GetType(Int16))
        dtaVaccinazioni.Columns.Add("ves_mal_codice_cond_sanitaria", GetType(String))
        dtaVaccinazioni.Columns.Add("ves_rsc_codice", GetType(String))
        dtaVaccinazioni.Columns.Add("ves_luogo", GetType(String))
        dtaVaccinazioni.Columns.Add("ves_tipo_erogatore", GetType(String))
        dtaVaccinazioni.Columns.Add("ves_cns_codice", GetType(String))
        dtaVaccinazioni.Columns.Add("ves_data_effettuazione", GetType(DateTime))
        dtaVaccinazioni.Columns.Add("ves_ass_codice", GetType(String))
        dtaVaccinazioni.Columns.Add("sel", GetType(Boolean)).DefaultValue = False
        dtaVaccinazioni.Columns.Add("eseguita", GetType(Boolean)).DefaultValue = False
        dtaVaccinazioni.Columns.Add("ves_esito", GetType(String)).DefaultValue = "N"
        dtaVaccinazioni.Columns.Add("ves_flag_fittizia", GetType(String)).DefaultValue = "N"
        dtaVaccinazioni.Columns.Add("ves_note", GetType(String))
        dtaVaccinazioni.Columns.Add("ves_ass_n_dose", GetType(Int16))
        dtaVaccinazioni.Columns.Add("ves_usl_inserimento", GetType(String))
        dtaVaccinazioni.Columns.Add("ves_flag_visibilita", GetType(String)).DefaultValue = valoreVisibilitaDatiVaccinaliDefault
        dtaVaccinazioni.Columns.Add("ass_descrizione", GetType(String))
        dtaVaccinazioni.Columns.Add("ves_noc_codice", GetType(String))
        dtaVaccinazioni.Columns.Add("ves_lot_codice", GetType(String))
        dtaVaccinazioni.Columns.Add("ves_lot_data_scadenza", GetType(String))
        dtaVaccinazioni.Columns.Add("ves_vii_codice", GetType(String))
        dtaVaccinazioni.Columns.Add("ves_sii_codice", GetType(String))
        dtaVaccinazioni.Columns.Add("ves_ope_codice", GetType(String))           ' Medico responsabile
        dtaVaccinazioni.Columns.Add("ves_med_vaccinante", GetType(String))       ' Vaccinatore
        dtaVaccinazioni.Columns.Add("ves_comune_o_stato", GetType(String))
        dtaVaccinazioni.Columns.Add("tipo_comune_o_stato", GetType(String))
        dtaVaccinazioni.Columns.Add("ves_tpa_guid_tipi_pagamento", GetType(Guid))
        dtaVaccinazioni.Columns.Add("ves_mal_codice_malattia", GetType(String))
        dtaVaccinazioni.Columns.Add("ves_codice_esenzione", GetType(String))
        dtaVaccinazioni.Columns.Add("ves_importo", GetType(String))
        dtaVaccinazioni.Columns.Add("struttura", GetType(String))
        dtaVaccinazioni.Columns.Add("ves_usl_cod_somministrazione", GetType(String))
        dtaVaccinazioni.Columns.Add("ves_antigene", GetType(String))

        ' Primary Key Datatable Vaccinazioni 
        key(0) = dtaVaccinazioni.Columns(1)
        key(1) = dtaVaccinazioni.Columns(2)

        dtaVaccinazioni.PrimaryKey = key

    End Sub
    Private Sub CaricamentoDati()
        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            ' Caricamento vie di somministrazione
            Using bizVieSomministrazione As New BizVieSomministrazione(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                BindDdl(ddlVii, Nothing, bizVieSomministrazione.GetVieSomministrazione())
            End Using

            ' Caricamento siti di inoculazione
            Using bizSitiInoculazione As New BizSitiInoculazione(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                BindDdl(ddlSii, Nothing, bizSitiInoculazione.GetSitiInoculazione())
            End Using

            ' Caricamento condizioni sanitarie del paziente 
            Using bizMalattie As New BizMalattie(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                CondizioniSanitariePaziente = bizMalattie.GetCondizioniSanitariePaziente(OnVacUtility.Variabili.PazId)
            End Using

            ' Caricamento condizione di rischio associata al paziente
            Using bizRischio As New BizCategorieRischio(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                CondizioniRischioPaziente = bizRischio.GetCondizioniRischioPaziente(OnVacUtility.Variabili.PazId)
            End Using

            ' Caricamento luoghi di esecuzione vaccinazioni
            Using bizLuoghiEsecuzioneVaccinazioni As New BizLuoghiEsecuzioneVaccinazioni(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                LuoghiEsecuzione = bizLuoghiEsecuzioneVaccinazioni.GetLuoghiEsecuzioneVaccinazioni(True)

            End Using

        End Using
    End Sub

    Private Function GetDoseFromString(stringValue As String) As Integer

        If String.IsNullOrWhiteSpace(stringValue) Then
            Return 0
        End If

        Dim dose As Integer = 0
        If Integer.TryParse(stringValue, dose) Then

            If dose <= 0 Then Return 0

        Else
            Return 0
        End If

        Return dose

    End Function

    Private Sub UnbindDataGrid()

        'tolgo la chiave prima di cambiare i dati poi la rimetto. Se non la tolgo ho dei problemi in certe situazioni
        dtaVaccinazioni.PrimaryKey = Nothing

        'Salva lo stato
        If dtaVaccinazioni.Rows.Count > 0 Then

            ' Per ogni riga di associazione
            For i As Int16 = 0 To dgrVaccinazioni.Items.Count - 1

                Try
                    Dim dgrDettaglio As DataGrid = DirectCast(dgrVaccinazioni.Items(i).FindControl("dgrDettaglio"), DataGrid)

                    Dim assCodice As String = DirectCast(dgrVaccinazioni.Items(i).FindControl("lblAssociazioneItem"), Label).Text
                    Dim assDoseOrig As String = DirectCast(dgrVaccinazioni.Items(i).FindControl("hdfAssociazioneDose"), HiddenField).Value
                    Dim luogo As String = DirectCast(dgrVaccinazioni.Items(i).FindControl("cmbLuogo"), Onit.Controls.OnitCombo).SelectedItem.Value
                    Dim ves_cns_codice As String = DirectCast(dgrVaccinazioni.Items(i).FindControl("txtConsultorioItem"), Onit.Controls.OnitModalList).Codice
                    Dim ves_data_effettuazione As DateTime = DirectCast(dgrVaccinazioni.Items(i).FindControl("txtDataItem"), Onit.Web.UI.WebControls.Validators.OnitDatePick).Data
                    'Dim ves_ope_codice As String = DirectCast(dgrVaccinazioni.Items(i).FindControl("txtMedicoItem"), Onit.Controls.OnitModalList).Codice
                    Dim Sel As String = DirectCast(dgrVaccinazioni.Items(i).FindControl("chkSelezionaItem"), CheckBox).Checked.ToString
                    Dim fittizia As Boolean = DirectCast(dgrVaccinazioni.Items(i).FindControl("chkFittizia"), CheckBox).Checked
                    Dim ves_ass_n_dose As String = DirectCast(dgrVaccinazioni.Items(i).FindControl("txtAssDoseItem"), TextBox).Text
                    Dim ves_tipo_erogatore As String = DirectCast(dgrVaccinazioni.Items(i).FindControl("cmbTipoErogatore"), Onit.Controls.OnitCombo).SelectedValue
                    Dim ves_note As String = HttpUtility.HtmlDecode(dgrVaccinazioni.Items(i).Cells(IndexDgrVac.Note).Text).Trim()

                    'Dati Aggiuntivi
                    Dim ves_noc_codice As String = HttpUtility.HtmlDecode(Me.dgrVaccinazioni.Items(i).Cells(IndexDgrVac.NomeCommerciale).Text).Trim()
                    Dim ves_lot_codice As String = HttpUtility.HtmlDecode(Me.dgrVaccinazioni.Items(i).Cells(IndexDgrVac.CodiceLotto).Text).Trim()
                    Dim ves_lot_data_scadenza As String = HttpUtility.HtmlDecode(Me.dgrVaccinazioni.Items(i).Cells(IndexDgrVac.DataScadenzaLotto).Text).Trim()
                    Dim ves_vii_codice As String = HttpUtility.HtmlDecode(Me.dgrVaccinazioni.Items(i).Cells(IndexDgrVac.ViaSomministrazione).Text).Trim()
                    Dim ves_sii_codice As String = HttpUtility.HtmlDecode(Me.dgrVaccinazioni.Items(i).Cells(IndexDgrVac.SitoInoculazione).Text).Trim()
                    Dim ves_ope_codice As String = HttpUtility.HtmlDecode(Me.dgrVaccinazioni.Items(i).Cells(IndexDgrVac.MedicoResponsabile).Text).Trim()   ' Medico Responsabile
                    Dim ves_med_vaccinante As String = HttpUtility.HtmlDecode(Me.dgrVaccinazioni.Items(i).Cells(IndexDgrVac.Vaccinatore).Text).Trim()      ' Vaccinatore
                    Dim ves_comune_o_stato As String = HttpUtility.HtmlDecode(Me.dgrVaccinazioni.Items(i).Cells(IndexDgrVac.Comune_Stato).Text).Trim()
                    Dim tipo_comune_o_stato As String = HttpUtility.HtmlDecode(Me.dgrVaccinazioni.Items(i).Cells(IndexDgrVac.Tipo_Comune_Stato).Text).Trim()
                    Dim struttura As String = HttpUtility.HtmlDecode(Me.dgrVaccinazioni.Items(i).Cells(IndexDgrVac.CodiceStruttura).Text).Trim()
                    Dim ves_usl_cod_somministrazione As String = HttpUtility.HtmlDecode(Me.dgrVaccinazioni.Items(i).Cells(IndexDgrVac.CodiceUsl).Text).Trim()

                    'Dati Pagamento
                    Dim ves_tpa_guid_tipi_pagamento As Guid = New Guid()
                    If Not String.IsNullOrWhiteSpace(HttpUtility.HtmlDecode(dgrVaccinazioni.Items(i).Cells(IndexDgrVac.GuidTipoPagamento).Text).Trim()) AndAlso
                        Guid.Parse(HttpUtility.HtmlDecode(dgrVaccinazioni.Items(i).Cells(IndexDgrVac.GuidTipoPagamento).Text).Trim()) <> Guid.Empty Then

                        Guid.TryParse(HttpUtility.HtmlDecode(dgrVaccinazioni.Items(i).Cells(IndexDgrVac.GuidTipoPagamento).Text).Trim(), ves_tpa_guid_tipi_pagamento)
                    Else
                        ' se non è già stato valorizzato il tipo pagamento, prendo quello previsto da configurazione
                        ves_tpa_guid_tipi_pagamento = New Guid(Settings.TIPOPAGAMENTO_DEFAULT)
                    End If

                    Dim ves_mal_codice_malattia As String = HttpUtility.HtmlDecode(dgrVaccinazioni.Items(i).Cells(IndexDgrVac.CodiceMalattia).Text).Trim()
                    Dim ves_codice_esenzione As String = HttpUtility.HtmlDecode(dgrVaccinazioni.Items(i).Cells(IndexDgrVac.CodiceEsenzione).Text).Trim()
                    Dim ves_importo As String = HttpUtility.HtmlDecode(dgrVaccinazioni.Items(i).Cells(IndexDgrVac.Importo).Text).Trim()

                    ' Valorizzazione flag visibilità dati vaccinali
                    Dim valoreVisibilitaDatiVaccinali As String =
                        Common.OnVacStoricoVaccinaleCentralizzato.GetValoreVisibilitaDatiVaccinali(dgrVaccinazioni.Items(i), "chkFlagVisibilita")

                    ' Per ogni vaccinazione contenuta nella riga di associazione
                    For k As Integer = 0 To dgrDettaglio.Items.Count - 1

                        ' Dose di vaccinazione (unico campo modificabile sulle vaccinazioni contenute)
                        Dim strDose As String = DirectCast(dgrDettaglio.Items(k).FindControl("txtDettagliDoseVac"), TextBox).Text
                        Dim vacCodice As String = DirectCast(dgrDettaglio.Items(k).FindControl("hdfVaccinazioneCodice"), HiddenField).Value

                        Dim condSanitaria As String = DirectCast(dgrDettaglio.Items(k).FindControl("omlCondSanitaria"), OnitModalList).Codice
                        Dim condRischio As String = DirectCast(dgrDettaglio.Items(k).FindControl("omlCondRischio"), OnitModalList).Codice

                        ' Aggiorno il datatable delle vaccinazioni
                        For j As Integer = 0 To dtaVaccinazioni.Rows.Count - 1

                            Dim drow As DataRow = dtaVaccinazioni.Rows(j)

                            If drow("ves_ass_codice").ToString() = assCodice AndAlso
                               drow("ves_vac_codice").ToString() = vacCodice AndAlso
                               drow("ves_ass_n_dose").ToString() = assDoseOrig Then

                                drow("ves_n_richiamo") = GetDoseFromString(strDose)
                                drow("ves_ass_n_dose") = GetDoseFromString(ves_ass_n_dose)

                                drow("ves_mal_codice_cond_sanitaria") = condSanitaria
                                drow("ves_rsc_codice") = condRischio

                                drow("ves_luogo") = luogo
                                drow("ves_tipo_erogatore") = ves_tipo_erogatore
                                drow("ves_cns_codice") = ves_cns_codice
                                drow("ves_data_effettuazione") = IIf(ves_data_effettuazione <> Date.MinValue, ves_data_effettuazione, DBNull.Value)
                                drow("Sel") = Sel
                                drow("ves_flag_fittizia") = IIf(fittizia, "S", "N")
                                drow("ves_note") = ves_note
                                drow("ves_flag_visibilita") = valoreVisibilitaDatiVaccinali
                                'Dati Aggiuntivi
                                drow("ves_noc_codice") = ves_noc_codice
                                drow("ves_lot_codice") = ves_lot_codice
                                drow("ves_lot_data_scadenza") = IIf(Not String.IsNullOrWhiteSpace(ves_lot_data_scadenza), ves_lot_data_scadenza, DBNull.Value)
                                drow("ves_vii_codice") = ves_vii_codice
                                drow("ves_sii_codice") = ves_sii_codice
                                drow("ves_ope_codice") = ves_ope_codice             ' Medico responsabile
                                drow("ves_med_vaccinante") = ves_med_vaccinante     ' Vaccinatore
                                drow("ves_comune_o_stato") = ves_comune_o_stato
                                drow("tipo_comune_o_stato") = tipo_comune_o_stato
                                drow("struttura") = struttura
                                drow("ves_usl_cod_somministrazione") = ves_usl_cod_somministrazione
                                'Dati Pagamento
                                drow("ves_tpa_guid_tipi_pagamento") = ves_tpa_guid_tipi_pagamento
                                drow("ves_mal_codice_malattia") = ves_mal_codice_malattia
                                drow("ves_codice_esenzione") = ves_codice_esenzione
                                drow("ves_importo") = ves_importo

                            End If

                        Next
                    Next

                Catch ex As Exception

                    Diagnostics.Trace.Write("E' avvenuto un errore non gestito nella conversione di tipo")
                    Diagnostics.Trace.Write(ex)

                    ex.InternalPreserveStackTrace()
                    Throw

                End Try

            Next

        End If

        'Rimetto la chiave al dt
        Dim key(1) As DataColumn
        key(0) = dtaVaccinazioni.Columns(1)
        key(1) = dtaVaccinazioni.Columns(2)

        Try
            dtaVaccinazioni.PrimaryKey = key
        Catch ex As Exception
            OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(
                                    "Si è verificato un problema nella gestione dei dati: verificare di non aver eseguito più volte la stessa vaccinazione con lo stesso numero di dose.",
                                    "DataTablePrimaryKeyError", False, False))
        End Try

    End Sub

    Private Sub CheckPrime()

        Dim minRichiamo As Integer = Integer.MaxValue

        For i As Integer = 0 To Me.dtaVaccinazioni.Rows.Count - 1

            If Me.dtaVaccinazioni.Rows(i)("Eseguita") = "False" Then
                If minRichiamo > Me.dtaVaccinazioni.Rows(i)("ves_ass_n_dose") Then
                    minRichiamo = Me.dtaVaccinazioni.Rows(i)("ves_ass_n_dose")
                End If
            End If

            Me.dtaVaccinazioni.Rows(i)("Sel") = "False"

        Next

        For i As Integer = 0 To Me.dtaVaccinazioni.Rows.Count - 1

            If Me.dtaVaccinazioni.Rows(i)("Eseguita") = "False" Then
                If minRichiamo = Me.dtaVaccinazioni.Rows(i)("ves_ass_n_dose") Then
                    Me.dtaVaccinazioni.Rows(i)("Sel") = "True"
                End If
            End If

        Next

    End Sub

    Private Sub txtLuogoItem_Filter(obj As Object)

        Dim objFm As Onit.Controls.OnitModalList = DirectCast(obj, Onit.Controls.OnitModalList)
        Dim cmb As Onit.Controls.OnitCombo = DirectCast(objFm.Parent.Parent.FindControl("cmbLuogo"), Onit.Controls.OnitCombo)

        If cmb.SelectedValue = Constants.CodiceLuogoVaccinazione.Estero Then
            objFm.Filtro = String.Format("COM_TIPO = '{0}' order by com_descrizione", Constants.TipologiaComune.Stato)
        ElseIf cmb.SelectedValue = Constants.CodiceLuogoVaccinazione.AltraAusl Then
            objFm.Filtro = String.Format("(COM_TIPO = '{0}') order by com_descrizione", Constants.TipologiaComune.ComuneItaliano)
        End If

    End Sub

    ' Codice e Descrizione dei luoghi sono definiti nel parametro LUOGHI. La gestione è nelle OnVacUtility.
    'Private Sub CaricaLuoghiEsecuzioneVaccinazioni(ByRef cmb As DropDownList)

    '    Dim luoghi As Collection.LuoghiEsecuzioneVacCollection = OnVacUtility.GetLuoghiEsecuzioneVaccinazioni(Me.Settings)

    '    Dim li As ListItem = Nothing

    '    For i As Integer = 0 To luoghi.Count - 1
    '        li = New ListItem(luoghi(i).DescrizioneLuogo, luoghi(i).CodiceLuogo)
    '        If Not cmb.Items.Contains(li) Then cmb.Items.Add(li)
    '    Next

    'End Sub

    Private Function CreaDtAssociazioni(dt_vacEseguite As DataTable) As DataTable

        Dim dtAssociazioni As DataTable = dt_vacEseguite.Clone()
        dtAssociazioni.Columns.Remove("ves_ass_n_dose")
        dtAssociazioni.Columns.Add(New DataColumn("ves_ass_n_dose", GetType(String)))

        For i As Integer = 0 To dt_vacEseguite.Rows.Count - 1

            If dt_vacEseguite.Rows(i).RowState <> DataRowState.Deleted Then

                Dim dv As New DataView(dtAssociazioni)
                dv.RowFilter = String.Format("ves_ass_codice='{0}' and ves_ass_n_dose = '{1}'",
                                             dt_vacEseguite.Rows(i)("ves_ass_codice").ToString(),
                                             dt_vacEseguite.Rows(i)("ves_ass_n_dose").ToString())

                If dv.Count = 0 Then

                    ' Aggiungo la riga (associazione, data) all'arraylist e alla tabella dtAssociazioni.
                    Dim drow As DataRow = dtAssociazioni.NewRow()

                    For j As Integer = 0 To dt_vacEseguite.Columns.Count - 1

                        If (dt_vacEseguite.Columns(j).ColumnName.ToLower = "ves_ass_codice" Or
                            dt_vacEseguite.Columns(j).ColumnName.ToLower() = "ves_ass_n_dose") Then

                            drow(dt_vacEseguite.Columns(j).ColumnName) = dt_vacEseguite.Rows(i)(j).ToString()

                        Else

                            drow(dt_vacEseguite.Columns(j).ColumnName) = dt_vacEseguite.Rows(i)(j)

                        End If

                    Next

                    dtAssociazioni.Rows.Add(drow)

                End If

            End If

        Next

        dtAssociazioni.AcceptChanges()

        Return dtAssociazioni

    End Function

    Private Sub SplittaAssociazione(dosiVaccinazioneDaRegistrare As Integer, assCodice As String, assDose As String, dam As IDAM)

        dam.QB.NewQuery()
        dam.QB.AddSelectFields("vac_codice", "vac_descrizione", "ass_descrizione")
        dam.QB.AddTables("t_ana_link_ass_vaccinazioni", "t_ana_vaccinazioni", "t_ana_associazioni")
        dam.QB.AddWhereCondition("val_ass_codice", Comparatori.Uguale, assCodice, DataTypes.Stringa)
        dam.QB.AddWhereCondition("val_vac_codice", Comparatori.Uguale, "vac_codice", DataTypes.Join)
        dam.QB.AddWhereCondition("val_ass_codice", Comparatori.Uguale, "ass_codice", DataTypes.Join)

        Using reader As IDataReader = dam.BuildDataReader()

            If Not reader Is Nothing Then

                Dim cod As Integer = reader.GetOrdinal("vac_codice")
                Dim descr As Integer = reader.GetOrdinal("vac_descrizione")
                Dim descr_ass As Integer = reader.GetOrdinal("ass_descrizione")

                While reader.Read()
                    AggiungiSingolaVaccinazione(reader(cod), reader(descr), reader(descr_ass), dosiVaccinazioneDaRegistrare, assCodice, assDose, dam)
                End While

            End If

        End Using

    End Sub

    Private Sub AggiungiSingolaVaccinazione(vacCodice As String, vacDescrizione As String, assDescrizione As String, dosiVaccinazioneDaRegistrare As Integer, assCodice As String, assDose As String, DAM As IDAM)

        Dim vacCodiceOrig As String = String.Empty
        Dim vacDescrizioneOrig As String = String.Empty

        'controlla se per la vaccinazione selezionata esiste la sostituta [modifica 08/08/2005]
        OnVacUtility.ControllaVaccinazioneSostituta(vacCodice, vacCodiceOrig, vacDescrizioneOrig)

        Dim vacDose As Integer = CalcolaDoseVaccinazione(vacCodice, DAM)
        AddRowDtVaccinazioni(vacCodice, vacDescrizione, assDescrizione, vacDose, assCodice, assDose)

        'riassegna il valore della descrizione della vaccinazione originale a video [modifica 08/08/2005]
        If vacCodiceOrig <> "" Then

            If Me.dtaVaccinazioni.Select("ves_vac_codice = '" & vacCodice & "'").Length > 0 Then

                Me.dtaVaccinazioni.Select("ves_vac_codice = '" & vacCodice & "'")(0).Item("vac_descrizione") = vacDescrizioneOrig
                Me.dtaVaccinazioni.Select("ves_vac_codice = '" & vacCodice & "'")(0).Item("ves_vac_codice") = vacCodiceOrig

            End If

        End If

    End Sub

    Private Function CalcolaDoseAssociazione(assCodice As String, DAM As IDAM) As Integer

        Dim assDose As Integer = 1
        Dim dt As DataTable = BuildMergedDtAssociazioni(assCodice, DAM)

        If dt.Rows.Count > 0 Then

            Dim buco As Boolean = False

            Dim dv As New DataView(dt)
            dv.Sort = "ves_ass_n_dose"

            For k As Integer = 0 To dv.Count - 1

                If assDose <> dv(k)("ves_ass_n_dose") Then

                    Dim drow As DataRow = dt.NewRow()
                    drow("ves_ass_codice") = assCodice
                    drow("ves_ass_n_dose") = assDose
                    dt.Rows.Add(drow)

                    buco = True
                    Exit For

                End If

                assDose = assDose + 1

            Next

            If Not buco Then
                assDose = dt.Compute("max(ves_ass_n_dose)", "ves_ass_n_dose is not null") + 1
            End If

        End If

        Return assDose

    End Function

    Private Function CalcolaDoseVaccinazione(vacCodice As String, DAM As IDAM) As Integer

        Dim vacDose As Integer = 1
        Dim dt As DataTable = BuildMergedDtVaccinazioni(vacCodice, DAM)

        ' Aggiungo la dose cercando di riempire il buco nella sequenza
        If dt.Rows.Count > 0 Then

            Dim dv As New DataView(dt)
            Dim buco As Boolean = False
            dv.Sort = "ves_n_richiamo"

            For k As Integer = 0 To dv.Count - 1

                If vacDose <> dv(k)("ves_n_richiamo") Then

                    Dim drow As DataRow = dt.NewRow()
                    drow("ves_vac_codice") = vacCodice
                    drow("ves_n_richiamo") = vacDose
                    dt.Rows.Add(drow)

                    buco = True
                    Exit For

                End If
                vacDose = vacDose + 1

            Next

            If Not buco Then
                vacDose = dt.Compute("max(ves_n_richiamo)", "ves_n_richiamo is not null") + 1
            End If

        End If

        Return vacDose

    End Function

    Private Function BuildMergedDtAssociazioni(assCodice As String, DAM As IDAM) As DataTable

        Dim dt As DataTable = Nothing

        Using genericProvider As New DAL.DbGenericProvider(DAM)
            dt = genericProvider.VaccinazioniEseguite.GetDtNumeroDosiAssociazionePaziente(OnVacUtility.Variabili.PazId, assCodice)
        End Using

        For j As Integer = 0 To Me.dtaVaccinazioni.Rows.Count - 1

            If Me.dtaVaccinazioni.Rows(j).RowState <> DataRowState.Deleted Then

                If Me.dtaVaccinazioni.Rows(j)("ves_ass_codice") = assCodice Then

                    Dim dv As New DataView(dt)
                    dv.RowFilter = String.Format("ves_ass_codice='{0}' and ves_ass_n_dose = {1}",
                                                 Me.dtaVaccinazioni.Rows(j)("ves_ass_codice").ToString(),
                                                 Me.dtaVaccinazioni.Rows(j)("ves_ass_n_dose"))

                    If dv.Count = 0 Then

                        Dim drow As DataRow = dt.NewRow()
                        drow("ves_ass_codice") = Me.dtaVaccinazioni.Rows(j)("ves_ass_codice")
                        drow("ves_ass_n_dose") = Me.dtaVaccinazioni.Rows(j)("ves_ass_n_dose")

                        dt.Rows.Add(drow)

                    End If

                End If

            End If

        Next

        Return dt

    End Function

    Private Function BuildMergedDtVaccinazioni(vacCodice As String, DAM As IDAM) As DataTable

        Dim dt As DataTable = Nothing

        Using genericProvider As New DAL.DbGenericProvider(DAM)
            dt = genericProvider.VaccinazioniEseguite.GetDtRichiamiVaccinazionePaziente(OnVacUtility.Variabili.PazId, vacCodice)
        End Using

        For i As Integer = 0 To Me.dtaVaccinazioni.Rows.Count - 1

            If Me.dtaVaccinazioni.Rows(i).RowState <> DataRowState.Deleted Then

                If Me.dtaVaccinazioni.Rows(i)("ves_vac_codice").ToString() = vacCodice Then

                    Dim dv As New DataView(dt)
                    dv.RowFilter = String.Format("ves_vac_codice='{0}' and ves_n_richiamo = {1}",
                                                 Me.dtaVaccinazioni.Rows(i)("ves_vac_codice").ToString(),
                                                 Me.dtaVaccinazioni.Rows(i)("ves_n_richiamo"))

                    If dv.Count = 0 Then

                        Dim drow As DataRow = dt.NewRow()
                        drow("ves_vac_codice") = Me.dtaVaccinazioni.Rows(i)("ves_vac_codice")
                        drow("ves_n_richiamo") = Me.dtaVaccinazioni.Rows(i)("ves_n_richiamo")
                        dt.Rows.Add(drow)

                    End If

                End If

            End If

        Next

        Return dt

    End Function

    Private Sub AddRowDtVaccinazioni(vacCodice As String, vacDescrizione As String, assDescrizione As String, vacDose As Integer, assCodice As String, assDose As String)

        Dim drow As DataRow = Me.dtaVaccinazioni.NewRow()

        drow("vac_descrizione") = vacDescrizione
        drow("ves_vac_codice") = vacCodice
        drow("ves_n_richiamo") = vacDose
        drow("ves_mal_codice_cond_sanitaria") = String.Empty
        drow("ves_rsc_codice") = String.Empty
        drow("ves_luogo") = String.Empty
        drow("ves_tipo_erogatore") = String.Empty
        drow("ves_cns_codice") = String.Empty
        drow("ves_data_effettuazione") = DBNull.Value
        drow("ves_ope_codice") = String.Empty
        drow("ves_ass_codice") = assCodice
        drow("ves_note") = String.Empty
        drow("ves_ass_n_dose") = assDose
        drow("ass_descrizione") = assDescrizione

        Me.dtaVaccinazioni.Rows.Add(drow)

    End Sub

    Private Sub RicercaSedute()

        Using dam As IDAM = OnVacUtility.OpenDam()

            Using genericProvider As New DbGenericProvider(dam)

                Using bizCicli As New Biz.BizCicli(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                    If bizCicli.ExistsCicliConVaccinazione(OnVacUtility.Variabili.PazId) Then
                        Me.strJS &= "alert('Attenzione: è possibile creare in automatico la seduta solo per i cicli con sedute di associazioni.\n Il paziente ha associati cicli con sedute di sole vaccinazioni, ma queste non verranno proposte.');"
                    End If

                End Using

                Dim dt As DataTable = Nothing
                Using biz As New Biz.BizVaccinazioniEseguite(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())
                    dt = biz.GetProssimaSedutaDaRegistrare(OnVacUtility.Variabili.PazId)
                End Using

                Dim minEtaSeduta As Integer = 0
                Dim minIntervallo As Integer = 0
                Dim etaPaziente As Integer = 0

                Dim assCodice As String
                Dim assDose As Integer = 0

                Dim row As DataRow = Nothing

                Me.dtaVaccinazioni.Clear()

                For i As Integer = 0 To dt.Rows.Count - 1

                    row = dt.Rows(i)
                    If i = 0 Then
                        etaPaziente = Common.Utility.PazienteHelper.CalcoloEta(dt.Rows(i)("paz_data_nascita")).GiorniTotali
                    End If

                    If etaPaziente > row("tsd_eta_seduta") Then

                        assCodice = row("sas_ass_codice")

                        If i = 0 Then

                            etaPaziente = Common.Utility.PazienteHelper.CalcoloEta(dt.Rows(i)("paz_data_nascita")).GiorniTotali
                            minEtaSeduta = row("tsd_eta_seduta")
                            minIntervallo = row("tsd_intervallo")

                            assDose = Me.CalcolaDoseAssociazione(assCodice, dam)

                            Me.SplittaAssociazione(1, assCodice, assDose, dam)

                        Else

                            Dim dv As New DataView(Me.dtaVaccinazioni)
                            dv.RowFilter = String.Format("ves_ass_codice='{0}'", assCodice)

                            If dv.Count = 0 Then

                                If (row("tsd_eta_seduta") <= minEtaSeduta + minIntervallo) Then

                                    ' minEtaSeduta = row("tsd_eta_seduta")
                                    If (row("tsd_intervallo") < minIntervallo) Then
                                        minIntervallo = row("tsd_intervallo")
                                    End If

                                    assDose = Me.CalcolaDoseAssociazione(assCodice, dam)

                                    Me.SplittaAssociazione(1, assCodice, assDose, dam)

                                Else

                                    Exit For

                                End If

                            End If

                        End If

                    Else

                        Exit For

                    End If

                Next

            End Using

        End Using

        Me.DataBindDatagrid(True)

        Me.OnitLayout31.Busy = True

    End Sub

    Private Sub CancellaSelezionate()

        Me.dtaVaccinazioni.AcceptChanges()

        For i As Integer = 0 To Me.dgrVaccinazioni.Items.Count - 1

            If DirectCast(Me.dgrVaccinazioni.Items(i).FindControl("chkSelezionaItem"), CheckBox).Checked Then

                Dim assCodice As String = DirectCast(Me.dgrVaccinazioni.Items(i).FindControl("lblAssociazioneItem"), Label).Text
                Dim assDose As String = DirectCast(Me.dgrVaccinazioni.Items(i).FindControl("txtAssDoseItem"), TextBox).Text

                For j As Integer = 0 To Me.dtaVaccinazioni.Rows.Count - 1

                    Dim drow As DataRow = Me.dtaVaccinazioni.Rows(j)

                    If drow.RowState <> DataRowState.Deleted Then

                        If drow("ves_ass_codice").ToString() = assCodice AndAlso
                           drow("ves_ass_n_dose").ToString() = assDose Then

                            Me.dtaVaccinazioni.Rows(i).Delete()

                        End If

                    End If

                Next

            End If

        Next

        Me.dtaVaccinazioni.AcceptChanges()

        Me.DataBindDatagrid(False)

    End Sub

    Private Sub Salva()

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, OnVacUtility.GetReadCommittedTransactionOptions)
            '--
            Using dbGenericProviderFactory As New DbGenericProviderFactory()
                '--
                Dim vaccinazioneEseguitaList As New List(Of Entities.VaccinazioneEseguita)()
                Dim vaccinazioneEseguitaEliminataList As New List(Of Entities.VaccinazioneEseguita)()
                '--
                Dim dt_vacEseguite As DataTable = Nothing
                '--
                Using bizVaccinazioniEseguite As New Biz.BizVaccinazioniEseguite(dbGenericProviderFactory, Me.Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.REGISTRAZIONE_VAC))
                    '--
                    dt_vacEseguite = bizVaccinazioniEseguite.GetVaccinazioniEseguite(OnVacUtility.Variabili.PazId, Me.IsGestioneCentrale)
                    '--
                    For i As Int16 = Me.dtaVaccinazioni.Rows.Count - 1 To 0 Step -1
                        '--
                        If Me.dtaVaccinazioni.Rows(i)("eseguita") = True Then
                            '--
                            ''******** LOG
                            'dtVacAppoggio = New DataTable
                            'dtVacAppoggio = Me.dtaVaccinazioni.Clone
                            'Dim rowVacAppoggio As DataRow = dtVacAppoggio.NewRow
                            'Dim campiConsiderati() As String = {"ves_vac_codice", "vac_descrizione", "ves_n_richiamo", "ves_luogo", "ves_cns_codice", "ves_data_effettuazione", "ves_ope_codice", "ves_ass_codice", "ves_esito"}
                            ''creazione di un datatable di appoggio per considerare le righe in inserimento,
                            ''altrimenti le vede come modificate [modifica 15/05/2006]
                            'rowVacAppoggio.ItemArray = Me.dtaVaccinazioni.Rows(i).ItemArray
                            'dtVacAppoggio.Rows.Add(rowVacAppoggio)
                            'LogBox.WriteData(LogBox.GetTestataDataTable(dtVacAppoggio, "REGISTRAZIONE_VAC", campiConsiderati))
                            ''************
                            '--
                            Dim tempImporto As Decimal

                            Dim r As DataRow = Me.dtaVaccinazioni.Rows(i)
                            Dim nrow As DataRow = dt_vacEseguite.NewRow()
                            '--
                            nrow("paz_codice") = OnVacUtility.Variabili.PazId
                            nrow("scaduta") = "N"
                            nrow("ves_vac_codice") = r("ves_vac_codice")
                            nrow("ves_n_richiamo") = r("ves_n_richiamo")
                            nrow("ves_mal_codice_cond_sanitaria") = r("ves_mal_codice_cond_sanitaria")
                            nrow("ves_rsc_codice") = r("ves_rsc_codice")
                            nrow("ves_data_effettuazione") = r("ves_data_effettuazione")
                            nrow("ves_dataora_effettuazione") = r("ves_data_effettuazione")
                            nrow("ves_cns_codice") = r("ves_cns_codice")
                            nrow("ves_stato") = "R"
                            nrow("ves_n_seduta") = "0"
                            nrow("ves_sii_codice") = ""
                            nrow("ves_ass_codice") = r("ves_ass_codice")
                            nrow("ves_luogo") = r("ves_luogo")
                            nrow("ves_tipo_erogatore") = r("ves_tipo_erogatore")
                            nrow("ves_cns_registrazione") = OnVacUtility.Variabili.CNS.Codice.ToString()
                            nrow("ves_note") = r("ves_note")
                            nrow("ves_ass_n_dose") = r("ves_ass_n_dose")
                            nrow("ves_esito") = r("ves_esito")
                            nrow("ves_flag_fittizia") = r("ves_flag_fittizia")
                            nrow("ves_flag_visibilita") = r("ves_flag_visibilita")

                            'Dati Aggiuntivi
                            nrow("ves_noc_codice") = r("ves_noc_codice")
                            nrow("ves_lot_codice") = r("ves_lot_codice")
                            nrow("ves_lot_data_scadenza") = r("ves_lot_data_scadenza")
                            nrow("ves_vii_codice") = r("ves_vii_codice")
                            nrow("ves_sii_codice") = r("ves_sii_codice")
                            nrow("ves_ope_codice") = r("ves_ope_codice") ' Medico responsabile
                            nrow("ves_med_vaccinante") = r("ves_med_vaccinante")     ' Vaccinatore
                            nrow("ves_comune_o_stato") = r("ves_comune_o_stato")
                            nrow("ves_usl_cod_somministrazione") = r("ves_usl_cod_somministrazione")
                            'nrow("tipo_comune_o_stato") = r("tipo_comune_o_stato")
                            'Dati Pagamento
                            nrow("ves_tpa_guid_tipi_pagamento") = New Guid(r("ves_tpa_guid_tipi_pagamento").ToString()).ToByteArray()
                            nrow("ves_mal_codice_malattia") = r("ves_mal_codice_malattia")
                            nrow("ves_codice_esenzione") = r("ves_codice_esenzione")
                            If Not String.IsNullOrWhiteSpace(r("ves_importo").ToString()) AndAlso Decimal.TryParse(r("ves_importo").ToString(), tempImporto) Then
                                nrow("ves_importo") = tempImporto
                            Else
                                nrow("ves_importo") = DBNull.Value
                            End If

                            'Altro (Campi calcolati)
                            nrow("ves_codice_struttura") = GetStruttura(r("struttura"), r("ves_tipo_erogatore")).CodiceStruttura
                            nrow("ves_antigene") = GetCodiceAntigene(r("ves_ass_codice"), r("ves_vac_codice"))
                            '--
                            dt_vacEseguite.Rows.Add(nrow)
                            '--
                            Me.dtaVaccinazioni.Rows(i).Delete()
                            '--
                        End If
                        '--
                    Next
                    '--
                    bizVaccinazioniEseguite.Salva(OnVacUtility.Variabili.PazId, dt_vacEseguite)
                    '--
#Region " FSE "
                    If Settings.FSE_GESTIONE Then

                        Dim indicizzazioneResult As BizGenericResult = OnVacUtility.FSEHelper.IndicizzaSuRegistry(
                            Convert.ToInt64(OnVacUtility.Variabili.PazId),
                            Constants.TipoDocumentoFSE.CertificatoVaccinale,
                            Constants.FunzionalitaNotificaFSE.RegistrazioneVac_Salva,
                            Constants.EventoNotificaFSE.InserimentoVaccinazione,
                            Settings,
                            String.Empty)

                        If Not indicizzazioneResult.Success AndAlso Not String.IsNullOrWhiteSpace(indicizzazioneResult.Message) Then
                            Me.OnitLayout31.InsertRoutineJS("alert('Indicizzazione sul Registry Regionale non avvenuta!');")
                        End If

                    End If
#End Region

                End Using
                '--
                Dim pazienteCorrente As Entities.Paziente = Nothing
                '--
                Using bizPaziente As New Biz.BizPaziente(dbGenericProviderFactory, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                    '--
                    ' N.B. : questa maschera è utilizzabile solo in modalità locale e non in centrale
                    pazienteCorrente = bizPaziente.CercaPaziente(OnVacUtility.Variabili.PazId)
                    '--
                End Using
                '--
                OnVacMidSendManager.ModificaPaziente(pazienteCorrente, dt_vacEseguite)
                '--
            End Using
            '--
            transactionScope.Complete()
            '--
        End Using

        Me.OnitLayout31.Busy = False

        Me.dtaVaccinazioni.AcceptChanges()

        Me.DataBindDatagrid(False)

    End Sub

    ''' <summary>
    ''' Per effettuare i controlli viene caricato il datatable delle eseguite, a cui vengono aggiunte le righe inserite
    ''' da registrazione storico.
    '''</summary>
    Private Sub Esegui()

        Dim control As Boolean = False
        Dim controlList As New ControlloEsecuzione()

        For i As Integer = 0 To Me.dgrVaccinazioni.Items.Count - 1
            If DirectCast(Me.dgrVaccinazioni.Items(i).FindControl("chkSelezionaItem"), CheckBox).Checked Then

                Dim assCodice As String = DirectCast(Me.dgrVaccinazioni.Items(i).FindControl("lblAssociazioneItem"), Label).Text
                Dim assDose As String = DirectCast(Me.dgrVaccinazioni.Items(i).FindControl("txtAssDoseItem"), TextBox).Text

                For j As Integer = 0 To Me.dtaVaccinazioni.Rows.Count - 1

                    Dim drow As DataRow = Me.dtaVaccinazioni.Rows(j)

                    If drow("ves_ass_codice").ToString() = assCodice AndAlso drow("ves_ass_n_dose").ToString() = assDose Then

                        'se la vaccinazione prevede una sostituta, è necessario effettuare i controlli su quest'ultima
                        OnVacUtility.ControllaVaccinazioneSostituta(Me.dtaVaccinazioni.Rows(i)("ves_vac_codice"))
                        CheckCampiObbligatori(i, controlList)

                        If controlList.Count > 0 Then Exit For

                    End If

                Next

            End If
        Next

        If Not controlList.HasError Then

            Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

                ' Riempio un datatable con vaccinazioni da registrare e gia' eseguite
                Dim dt_vacEseguite As DataTable = Nothing

                Using biz As New Biz.BizVaccinazioniEseguite(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                    dt_vacEseguite = biz.GetVaccinazioniEseguite(OnVacUtility.Variabili.PazId, Me.IsGestioneCentrale)

                    For i As Int16 = 0 To Me.dtaVaccinazioni.Rows.Count - 1

                        If Me.dtaVaccinazioni.Rows(i).RowState <> DataRowState.Deleted AndAlso Me.dtaVaccinazioni.Rows(i)("sel") Then

                            Dim drow As DataRow = dt_vacEseguite.NewRow()

                            drow("ves_vac_codice") = Me.dtaVaccinazioni.Rows(i)("ves_vac_codice")
                            drow("vac_descrizione") = Me.dtaVaccinazioni.Rows(i)("vac_descrizione")
                            drow("ves_n_richiamo") = Me.dtaVaccinazioni.Rows(i)("ves_n_richiamo")
                            drow("ves_ass_codice") = Me.dtaVaccinazioni.Rows(i)("ves_ass_codice")
                            drow("ves_ass_n_dose") = Me.dtaVaccinazioni.Rows(i)("ves_ass_n_dose")
                            drow("ves_mal_codice_cond_sanitaria") = Me.dtaVaccinazioni.Rows(i)("ves_mal_codice_cond_sanitaria")
                            drow("ves_rsc_codice") = Me.dtaVaccinazioni.Rows(i)("ves_rsc_codice")
                            drow("ves_data_effettuazione") = Me.dtaVaccinazioni.Rows(i)("ves_data_effettuazione")
                            drow("scaduta") = "N"
                            drow("ves_esito") = Me.dtaVaccinazioni.Rows(i)("ves_esito")
                            drow("ves_flag_fittizia") = Me.dtaVaccinazioni.Rows(i)("ves_flag_fittizia")
                            drow("ves_flag_visibilita") = Me.dtaVaccinazioni.Rows(i)("ves_flag_visibilita")
                            drow("ves_tipo_erogatore") = Me.dtaVaccinazioni.Rows(i)("ves_tipo_erogatore")

                            dt_vacEseguite.Rows.Add(drow)

                        End If

                    Next

                    ' Creo il datatable con le associazioni per effettuare i controlli  su di esse
                    Dim dtAssociazioni As DataTable = biz.CreaDtAssociazioni(dt_vacEseguite)

                    For i As Int16 = 0 To dtAssociazioni.Rows.Count - 1

                        ' Effettuo il controllo solo sulle righe inserite da registrazione storico (non hanno ves_id)
                        If dtAssociazioni.Rows(i)("ves_id") Is System.DBNull.Value Then
                            biz.CheckDatiAssociazione(i, dtAssociazioni, controlList)
                        End If

                    Next

                    ' Controlli sulle singole vaccinazioni
                    For i As Int16 = 0 To dt_vacEseguite.Rows.Count - 1

                        ' Effettuo il controllo solo sulle righe inserite da registrazione storico (non hanno ves_id)
                        If dt_vacEseguite.Rows(i)("ves_id") Is System.DBNull.Value Then

                            biz.CheckDatiVaccinazione(i, dt_vacEseguite, controlList)
                            biz.CheckDataEffettuazione(i, dt_vacEseguite, controlList, OnVacUtility.Variabili.PazId)
                            biz.CheckVaccinazioneProgrammata(i, dt_vacEseguite, controlList, OnVacUtility.Variabili.PazId)

                        End If

                    Next

                End Using

                If Not controlList.HasError Then

                    For i As Int16 = 0 To dt_vacEseguite.Rows.Count - 1

                        ' Effettuo il controllo solo sulle righe inserite da registrazione storico (non hanno ves_id)
                        If dt_vacEseguite.Rows(i)("ves_id") Is System.DBNull.Value Then

                            Dim key(1) As Object
                            key(0) = dt_vacEseguite.Rows(i)("ves_vac_codice")
                            key(1) = dt_vacEseguite.Rows(i)("ves_n_richiamo")

                            Me.dtaVaccinazioni.Rows.Find(key)("eseguita") = True

                            control = True

                        End If

                    Next

                End If

            End Using

        End If

        If controlList.Count > 0 Then Me.strJS &= controlList.GetAlertJS()

        Me.DataBindDatagrid(False)

        For i As Int16 = 0 To Me.dgrVaccinazioni.Items.Count - 1

            Dim luogoSelezionato As Entities.LuoghiEsecuzioneVaccinazioni =
                            GetLuogoByComboLuogo(DirectCast(Me.dgrVaccinazioni.Items(i).FindControl("cmbLuogo"), OnitCombo))
            AggiornaDatiLuogoRiga(dgrVaccinazioni.Items(i), luogoSelezionato, True)
            SetObbligatorietaCampiDettaglioRiga(Me.dgrVaccinazioni.Items(i), luogoSelezionato)

        Next

        ' deve costringere a salvare o annullare se ha eseguito alcune vaccinazioni 
        If control Then Me.OnitLayout31.Busy = True

        CheckPrime()

    End Sub

    Private Sub SetColumnHeaderVisibility(sortExpression As String, visible As Boolean)

        Dim column As DataGridColumn = (From item As DataGridColumn In Me.dgrVaccinazioni.Columns
                                        Where item.SortExpression = sortExpression
                                        Select item).FirstOrDefault()

        If Not visible Then column.HeaderText = String.Empty

    End Sub

    Private Sub SetToolbarStatus(recuperaStoricoEnabled As Boolean)

        Me.Toolbar.FindItemByValue("btnSalva").Enabled = Not recuperaStoricoEnabled
        Me.Toolbar.FindItemByValue("btnAnnulla").Enabled = Not recuperaStoricoEnabled
        Me.Toolbar.FindItemByValue("btnElimina").Enabled = Not recuperaStoricoEnabled
        Me.Toolbar.FindItemByValue("btnCalcolaSeduta").Enabled = Not recuperaStoricoEnabled
        Me.Toolbar.FindItemByValue("btnAggiungiAssAssociate").Enabled = Not recuperaStoricoEnabled
        Me.Toolbar.FindItemByValue("btnEsegui").Enabled = Not recuperaStoricoEnabled
        Me.Toolbar.FindItemByValue("btnRecuperaStoricoVacc").Enabled = recuperaStoricoEnabled

    End Sub

    Private Sub SetLayout(gridItem As DataGridItem)

        Dim eseguita As Control = gridItem.FindControl("imgEseguita")
        If eseguita Is Nothing Then
            eseguita.Visible = False
        End If

        Dim cmbLuogoCorrente As OnitCombo = gridItem.FindControl("cmbLuogo")
        Dim cmbTipoErogatore As OnitCombo = gridItem.FindControl("cmbTipoErogatore")
        Dim txtConsultorioItem As OnitModalList = gridItem.FindControl("txtConsultorioItem")
        Dim btnDatiVac As ImageButton = gridItem.FindControl("btnDatiVac")
        Dim lnkPagVac As LinkButton = gridItem.FindControl("lnkPagVac")

        Dim luogoSelezionato As Entities.LuoghiEsecuzioneVaccinazioni = GetLuogoByComboLuogo(cmbLuogoCorrente)

        If cmbLuogoCorrente.SelectedIndex > 0 AndAlso Not eseguita.Visible Then
            cmbTipoErogatore.Enabled = True
            cmbTipoErogatore.CssClass = "Textbox_stringa_obbligatorio"
            If cmbTipoErogatore.SelectedIndex > 0 Then
                If Not luogoSelezionato Is Nothing AndAlso BizRegistrazioneVaccinazioni.IsCampoObbligatorio(txtConsultorioItem.ID, CampiObbligatoriByLuogoSelezionato) Then
                    txtConsultorioItem.Enabled = True
                    txtConsultorioItem.Obbligatorio = True
                Else
                    txtConsultorioItem.Enabled = True
                    txtConsultorioItem.Obbligatorio = False
                End If
                btnDatiVac.Enabled = True
                lnkPagVac.Enabled = True
            Else
                txtConsultorioItem.Enabled = False
                txtConsultorioItem.Obbligatorio = False
                btnDatiVac.Enabled = False
                lnkPagVac.Enabled = False
            End If

        ElseIf eseguita.Visible Then
            cmbTipoErogatore.Enabled = False
            cmbTipoErogatore.CssClass = "Textbox_stringa_disabilitato"
            txtConsultorioItem.Enabled = False
            txtConsultorioItem.Obbligatorio = False
            btnDatiVac.Enabled = True
            lnkPagVac.Enabled = True
        Else
            cmbTipoErogatore.Enabled = False
            cmbTipoErogatore.CssClass = "Textbox_stringa_disabilitato"
            txtConsultorioItem.Enabled = False
            txtConsultorioItem.Obbligatorio = False
            btnDatiVac.Enabled = False
            lnkPagVac.Enabled = False
        End If


    End Sub


    Private Sub DataBindDatagrid(selezionaItem As Boolean)

        dgrVaccinazioni.Visible = True

        dgrVaccinazioni.DataSource = CreaDtAssociazioni(Me.dtaVaccinazioni)
        dgrVaccinazioni.DataBind()

        ' Visibilità colonna flag consenso del paziente (la valorizzazione del checkbox viene gestita nell'ItemDataBound del datagrid)
        SetColumnHeaderVisibility("ves_flag_visibilita", True)

        ' Visibilità colonna vaccinazione fittizia
        SetColumnHeaderVisibility("ves_flag_fittizia", Settings.GESVACFITTIZIA)

        If selezionaItem Then

            For i As Integer = 0 To dgrVaccinazioni.Items.Count - 1
                DirectCast(dgrVaccinazioni.Items(i).FindControl("chkSelezionaItem"), CheckBox).Checked = True
            Next

        End If

    End Sub

    Private Sub RecuperaStoricoVaccinale()

        Dim command As New Common.OnVacStoricoVaccinaleCentralizzato.AcquisisciStoricoCommand()
        command.CodicePaziente = OnVacUtility.Variabili.PazId
        command.RichiediConfermaSovrascrittura = False
        command.Settings = Settings
        command.OnitLayout3 = OnitLayout31
        command.BizLogOptions = OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.REGISTRAZIONE_VAC)
        command.Note = "Recupero Storico Vaccinale da maschera RegistrazioneVac"

        Dim acquisisciDatiVaccinaliCentraliResult As BizPaziente.AcquisisciDatiVaccinaliCentraliResult =
            Common.OnVacStoricoVaccinaleCentralizzato.AcquisisciDatiVaccinaliCentraliPaziente(command)

        If acquisisciDatiVaccinaliCentraliResult.StatoAcquisizioneDatiVaccinaliCentrale <> Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Parziale Then

            SetToolbarStatus(False)
            ControlloEliminazioneProgrammazione()

        End If

        InizializzazionePagina()

    End Sub

    Private Function GetCurrentDataGridVaccinazioniItem(sender As Object, controlId As String) As DataGridItem

        For Each item As DataGridItem In Me.dgrVaccinazioni.Items

            If item.FindControl(controlId) Is sender Then
                Return item
            End If

        Next

        Return Nothing

    End Function

    Private Function GetCodiceVaccinazioneCurrentRow(sender As Object, controlId As String) As String

        Dim codiceVaccinazioneCorrente As String = String.Empty

        Dim currentGridItem As DataGridItem = Me.GetCurrentDataGridVaccinazioniItemByDettaglio(sender, controlId)
        If Not currentGridItem Is Nothing Then

            codiceVaccinazioneCorrente = DirectCast(currentGridItem.FindControl("hdfVaccinazioneCodice"), HiddenField).Value

        End If

        Return codiceVaccinazioneCorrente

    End Function

    Private Function GetCurrentDataGridVaccinazioniItemByDettaglio(sender As Object, controlId As String) As DataGridItem

        For Each item As DataGridItem In Me.dgrVaccinazioni.Items

            Dim control As Control = item.FindControl("dgrDettaglio")
            If Not control Is Nothing Then

                Dim dgrDettaglio As DataGrid = DirectCast(control, DataGrid)

                For Each detailItem As DataGridItem In dgrDettaglio.Items

                    If detailItem.FindControl(controlId) Is sender Then
                        Return detailItem
                    End If

                Next

            End If

        Next

        Return Nothing

    End Function

    Private Sub BindDdl(Of T As New)(ddl As OnitCombo, valoreCampoCodice As Object, dataSourceList As List(Of T))

        If Not ddl Is Nothing Then

            ddl.DataValueField = "Codice"
            ddl.DataTextField = "Descrizione"
            ddl.DataSource = dataSourceList
            ddl.DataBind()

            SelectDdlValue(ddl, valoreCampoCodice)

        End If

    End Sub

    Private Sub SelectDdlValue(ddl As OnitCombo, valoreCampoCodice As Object)

        ddl.ClearSelection()

        If Not valoreCampoCodice Is Nothing AndAlso
           Not valoreCampoCodice Is DBNull.Value AndAlso
           Not String.IsNullOrWhiteSpace(valoreCampoCodice.ToString()) Then

            Dim listItem As ListItem = ddl.Items.FindByValue(valoreCampoCodice.ToString())
            If Not listItem Is Nothing Then
                listItem.Selected = True
            End If

        End If

    End Sub

#End Region

#Region " OnitCombo Luogo "

    Protected Sub cmbLuogo_SelectedIndexChanged(sender As Object, e As EventArgs)

        Dim currentGridItem As DataGridItem = Me.GetCurrentDataGridVaccinazioniItem(sender, "cmbLuogo")
        If Not currentGridItem Is Nothing Then

            'Se modifico il luogo => forzo il Tipo Erogatore a ""
            Dim cmbTipoErogatore As OnitCombo = currentGridItem.FindControl("cmbTipoErogatore")
            Dim dataVacEffettuata As Date? = DirectCast(currentGridItem.FindControl("txtDataItem"), OnitDatePick).Data
            cmbTipoErogatore.ClearSelection()

            AggiornaDatiAggiuntiviRiga(currentGridItem, New BizRegistrazioneVaccinazioni.DatiAggiuntiviVaccinazione, True)
            AggiornaDatiPagamentoRiga(currentGridItem, New BizRegistrazioneVaccinazioni.DatiPagamentoVaccinazione())

            Dim cmbLuogoCorrente As OnitCombo = DirectCast(sender, OnitCombo)

            Dim luogoSelezionato As Entities.LuoghiEsecuzioneVaccinazioni = GetLuogoByComboLuogo(cmbLuogoCorrente)

            ReplicaLuogoEsecuzione(currentGridItem, luogoSelezionato)

            If Not luogoSelezionato Is Nothing Then
                CampiObbligatoriByLuogoSelezionato = GetCampiAggiuntiviObbligatori(luogoSelezionato.Codice, dataVacEffettuata)
            End If

        End If

    End Sub

    Private Sub ReplicaLuogoEsecuzione(currentGridItem As DataGridItem, luogoSelezionato As Entities.LuoghiEsecuzioneVaccinazioni)
        Dim chk As CheckBox = DirectCast(currentGridItem.FindControl("chkSelezionaItem"), CheckBox)

#Region " Replica su righe selezionate (funzione non utilizzata)"
        'If chk.Checked Then

        '    ' Se la riga è selezionata per l'esecuzione, aggiorno tutte le righe selezionate
        '    For i As Int16 = 0 To Me.dgrVaccinazioni.Items.Count - 1

        '        chk = DirectCast(Me.dgrVaccinazioni.Items(i).FindControl("chkSelezionaItem"), CheckBox)

        '        If chk.Checked Then
        '            AggiornaDatiLuogoRiga(Me.dgrVaccinazioni.Items(i), luogoSelezionato, (currentGridItem.ItemIndex = i))
        '        End If

        '    Next

        'Else

        '    ' Se la riga non è selezionata per l'esecuzione, aggiorno solo la riga stessa
        '    AggiornaDatiLuogoRiga(currentGridItem, luogoSelezionato, True)

        'End If
#End Region

        ' Se la riga non è selezionata per l'esecuzione, aggiorno solo la riga stessa
        AggiornaDatiLuogoRiga(currentGridItem, luogoSelezionato, True)
    End Sub

    Private Sub AggiornaDatiLuogoRiga(gridItem As DataGridItem, luogoSelezionato As Entities.LuoghiEsecuzioneVaccinazioni, isSameRow As Boolean)

        Dim selectedErogatore As String = String.Empty

        Dim cmbTipoErogatore As OnitCombo = gridItem.FindControl("cmbTipoErogatore")
        ' Valorizzazione luogo
        Dim cmbLuogo As OnitCombo = gridItem.FindControl("cmbLuogo")

        Dim codiceLuogoSelezionato As String = String.Empty
        If Not luogoSelezionato Is Nothing Then codiceLuogoSelezionato = luogoSelezionato.Codice

        'Valorizzazione Tipo Erogatore
        If Not String.IsNullOrWhiteSpace(codiceLuogoSelezionato) Then

            If Not cmbTipoErogatore Is Nothing Then

                selectedErogatore = cmbTipoErogatore.SelectedValue
                Dim erogatori As New List(Of Entities.TipoErogatoreVacc)
                erogatori.Add(New Entities.TipoErogatoreVacc() With {.Descrizione = ""})
                erogatori.AddRange(LoadTipiErogatore(codiceLuogoSelezionato))
                cmbTipoErogatore.DataSource = erogatori
                cmbTipoErogatore.DataBind()

                SelectDdlValue(cmbTipoErogatore, selectedErogatore)

            End If

        Else

            If Not cmbTipoErogatore Is Nothing Then
                cmbTipoErogatore.ClearSelection()
                cmbTipoErogatore.Items.Clear()
            End If

        End If

        If isSameRow Or cmbLuogo.SelectedValue <> codiceLuogoSelezionato Then

            'cmbLuogo.SelectedValue = codiceLuogoSelezionato

            '' Abilitazione e obbligatorietà consultorio se luogo = consultorio
            Dim txtConsultorioItem As OnitModalList = gridItem.FindControl("txtConsultorioItem")

            If String.IsNullOrWhiteSpace(selectedErogatore) Then
                txtConsultorioItem.Codice = String.Empty
                txtConsultorioItem.Descrizione = String.Empty
            End If

            txtConsultorioItem.RefreshDataBind()

            SetObbligatorietaCampiDettaglioRiga(gridItem, luogoSelezionato)

        End If
        SetLayout(gridItem)

    End Sub

    ''' <summary>
    ''' Imposta l'obbligatorietà dei campidi dettaglio sulla riga del datagrid
    ''' </summary>
    ''' <param name="gridItem"></param>
    ''' <param name="luogoSelezionato"></param>
    ''' <remarks></remarks>
    Private Sub SetObbligatorietaCampiDettaglioRiga(gridItem As DataGridItem, luogoSelezionato As Entities.LuoghiEsecuzioneVaccinazioni)

        ' Obbligatorietà condizione sanitaria e condizione di rischio
        Dim dgrDettaglio As DataGrid = DirectCast(gridItem.FindControl("dgrDettaglio"), DataGrid)

        Dim idCampiObbligatori As List(Of String) = Nothing
        'If Not luogoSelezionato Is Nothing Then idCampiObbligatori = luogoSelezionato.IdCampiObbligatori
        If Not luogoSelezionato Is Nothing Then
            Dim dataVacEffettuata As Date? = DirectCast(gridItem.FindControl("txtDataItem"), Onit.Web.UI.WebControls.Validators.OnitDatePick).Data
            idCampiObbligatori = GetCampiAggiuntiviObbligatori(luogoSelezionato.Codice, dataVacEffettuata)
        End If
        'Dim txtConsultorioItem As OnitModalList = gridItem.FindControl("txtConsultorioItem")

        For i As Integer = 0 To dgrDettaglio.Items.Count - 1

            ' Obbligatorietà condizione sanitaria
            Dim omlCondSanitaria As OnitModalList = DirectCast(dgrDettaglio.Items(i).FindControl("omlCondSanitaria"), OnitModalList)
            omlCondSanitaria.Obbligatorio = BizRegistrazioneVaccinazioni.IsCampoObbligatorio(omlCondSanitaria.ID, idCampiObbligatori)

            ' Obbligatorietà condizione di rischio
            Dim omlCondRischio As OnitModalList = DirectCast(dgrDettaglio.Items(i).FindControl("omlCondRischio"), OnitModalList)
            omlCondRischio.Obbligatorio = BizRegistrazioneVaccinazioni.IsCampoObbligatorio(omlCondRischio.ID, idCampiObbligatori)

        Next

    End Sub

    Private Function GetCampiAggiuntiviObbligatori(luogoSelezionato As String, dataEsecuzione As Date?) As List(Of String)
        Dim result As New List(Of String)
        Dim campiObbligatori As New List(Of String)
        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using biz As New BizLuoghiEsecuzioneVaccinazioni(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos)
                If Not luogoSelezionato Is Nothing Then
                    If dataEsecuzione.HasValue AndAlso (Not dataEsecuzione.Value = Date.MinValue) Then
                        campiObbligatori = biz.GetCampiObbligatoriByLuogo(luogoSelezionato, dataEsecuzione).Select(Function(f) f.CodCampo).ToList()
                    Else
                        campiObbligatori = biz.GetCampiObbligatoriByLuogo(luogoSelezionato, Date.MinValue).Select(Function(f) f.CodCampo).ToList()
                    End If
                End If
                Dim listControlliCampiObbligatori As List(Of ControlloCampoObbligatorio) = LinkControlliCampiObbligatori()
                For Each i As String In campiObbligatori
                    result.Add(listControlliCampiObbligatori.Where(Function(m) m.CampoObbligatorio = i).Select(Function(f) f.Controllo).FirstOrDefault())
                Next
            End Using
        End Using
        Return result
    End Function

    Private Function GetLuogoByComboLuogo(cmbLuogo As OnitCombo) As Entities.LuoghiEsecuzioneVaccinazioni

        If cmbLuogo Is Nothing OrElse cmbLuogo.SelectedIndex < 0 Then
            Return Nothing
        End If

        Return Me.LuoghiEsecuzione.FirstOrDefault(Function(p) p.Codice = cmbLuogo.SelectedValue)

    End Function


#End Region

#Region " Pagamento Vaccinazione "
    Protected Sub btnPagVacOk_Click(sender As Object, e As EventArgs)
        btnPagVacModale_Click(True)
    End Sub


    Protected Sub btnPagVacAnnulla_Click(sender As Object, e As EventArgs)
        btnPagVacModale_Click(False)
    End Sub

    Private Sub btnPagVacModale_Click(edit As Boolean)
        If edit Then
            If Not String.IsNullOrWhiteSpace(Me.hdRowIndexPagVac.Value) Then
                Dim dati As New BizRegistrazioneVaccinazioni.DatiPagamentoVaccinazione
                Dim gridItemIndex As Integer = Convert.ToInt32(hdRowIndexPagVac.Value)

                Dim currentGridItem As DataGridItem = Me.dgrVaccinazioni.Items(hdRowIndexPagVac.Value)
                dati = GetDatiPagamentoFromControls()
                AggiornaDatiPagamentoRiga(dgrVaccinazioni.Items(hdRowIndexPagVac.Value), dati)
                UnbindDataGrid()
            End If
        End If
        SetDatiPagamento(Nothing)
        ucPagamentoVaccinazione.SetLayout()
        modPagVac.VisibileMD = False
    End Sub

    Private Sub SetDatiPagamento(dati As BizRegistrazioneVaccinazioni.DatiPagamentoVaccinazione)
        ucPagamentoVaccinazione.SetDatiPagamentoControls(dati)
    End Sub

    Private Function GetDatiPagamentoFromControls() As BizRegistrazioneVaccinazioni.DatiPagamentoVaccinazione
        Return ucPagamentoVaccinazione.GetDatiPagamentoFromControls()
    End Function

    Private Function GetDatiPagamentoFromDataGridItem(currentGridItem As DataGridItem) As BizRegistrazioneVaccinazioni.DatiPagamentoVaccinazione
        Dim dati As New BizRegistrazioneVaccinazioni.DatiPagamentoVaccinazione()
        If String.IsNullOrWhiteSpace(HttpUtility.HtmlDecode(currentGridItem.Cells(IndexDgrVac.GuidTipoPagamento).Text).Trim()) Then
            dati.GuidTipoPagamento = New Guid()
        ElseIf Guid.Parse(HttpUtility.HtmlDecode(currentGridItem.Cells(IndexDgrVac.GuidTipoPagamento).Text)) <> Guid.Empty Then
            dati.GuidTipoPagamento = New Guid(HttpUtility.HtmlDecode(currentGridItem.Cells(IndexDgrVac.GuidTipoPagamento).Text).Trim())
        Else
            ' se non è già stato valorizzato il tipo pagamento, prendo quello previsto da configurazione
            dati.GuidTipoPagamento = New Guid(Settings.TIPOPAGAMENTO_DEFAULT)
        End If

        dati.CodiceEsenzione = HttpUtility.HtmlDecode(currentGridItem.Cells(IndexDgrVac.CodiceEsenzione).Text).Trim()
        dati.CodiceMalattia = HttpUtility.HtmlDecode(currentGridItem.Cells(IndexDgrVac.CodiceMalattia).Text).Trim()
        dati.Importo = HttpUtility.HtmlDecode(currentGridItem.Cells(IndexDgrVac.Importo).Text).Trim()
        Return dati
    End Function

    Private Function GetDecimalOrDefault(value As String) As Decimal?
        Dim temp As Decimal
        If Decimal.TryParse(value, temp) Then
            Return temp
        End If
        Return Nothing
    End Function

#End Region

#Region " Gestione Campo Comune Stato "

    Protected Sub omlComuneStato_Change(Sender As Object, E As OnitModalList.ModalListaEventArgument)

        SetFiltroAsl(omlComuneStato.Codice)

        omlUsl.Codice = String.Empty
        omlUsl.Descrizione = String.Empty
        omlUsl.RefreshDataBind()

        SetFiltroStruttura(omlComuneStato.Codice)
    End Sub

#End Region

#Region " Gestione Campo Strutture "

    Private Sub SetFiltroStruttura(codice As String)
        Dim filtro As New System.Text.StringBuilder
        filtro.Append(" 1 = 1 ")
        If Not String.IsNullOrWhiteSpace(codice) Then
            filtro.Append(String.Format(" and AST_CODICE_COMUNE = '{0}' ", codice))
        End If
        filtro.Append(" order by AST_CODICE_COMUNE ")
        omlStrutture.Filtro = filtro.ToString()
    End Sub

    Protected Sub omlStrutture_Change(Sender As Object, E As OnitModalList.ModalListaEventArgument)

        If Not String.IsNullOrWhiteSpace(omlStrutture.Codice) Then
            omlComuneStato.Codice = omlStrutture.ValoriAltriCampi("CodiceComune")
            omlComuneStato.RefreshDataBind()
            SetFiltroComuneStato(omlStrutture.ValoriAltriCampi("CodiceComune"))

            omlUsl.Codice = omlStrutture.ValoriAltriCampi("Regione") + omlStrutture.ValoriAltriCampi("Asl")
            omlUsl.RefreshDataBind()

            If Not String.IsNullOrWhiteSpace(omlComuneStato.Codice) Then
                SetFiltroAsl(omlComuneStato.Codice)
            Else
                SetFiltroAsl("")
            End If

        Else
            omlComuneStato.Codice = String.Empty
            omlComuneStato.RefreshDataBind()
            SetFiltroComuneStato(String.Empty)

            omlUsl.Codice = String.Empty
            omlUsl.RefreshDataBind()
            SetFiltroAsl(String.Empty)
        End If
    End Sub

#End Region

#Region " Antigene / Codice Vaccinazione AVN "

    Private Function GetCodiceAntigene(codiceAss As String, codiceVac As String) As String
        Dim result As String = String.Empty
        Using dbGenericProviderFactory As New DbGenericProviderFactory()
            '--
            Using genericProvider As DbGenericProvider = dbGenericProviderFactory.GetDbGenericProvider(OnVacContext.AppId, OnVacContext.Azienda)

                Using biz As New BizVaccinazioniAnagrafica(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                    Return biz.GetAntigene(codiceAss, codiceVac)

                End Using

            End Using
        End Using

    End Function

#End Region

#Region " Struttura - Consultori AVN "

    Private Function GetStruttura(codiceCns As String, tipoErogatore As String) As Entities.Struttura
        Dim result As New List(Of Entities.Struttura)
        Using dbGenericProviderFactory As New DbGenericProviderFactory()
            '--
            Using genericProvider As DbGenericProvider = dbGenericProviderFactory.GetDbGenericProvider(OnVacContext.AppId, OnVacContext.Azienda)

                Using biz As New BizConsultori(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.VAC_PROGRAMMATE, True))

                    result = biz.GetStrutture(codiceCns, tipoErogatore)

                End Using

            End Using
        End Using

        If Not result Is Nothing AndAlso result.Count > 0 Then
            Return result.FirstOrDefault()
        End If

        Return New Entities.Struttura()

    End Function

#End Region

#Region " Replica Data, Luogo, Tipo erogatore, Centro Vaccinale "

    ' vengono replicati i dati sopra indicati su tutte le righe checkate, sovrascrivendoli
    Protected Sub btnReplica_Click(sender As ImageButton, e As EventArgs)
        Dim currentItemIndex As Integer = DirectCast(sender.NamingContainer, DataGridItem).ItemIndex
        Dim currentItem As DataGridItem = dgrVaccinazioni.Items(currentItemIndex)

        For Each assRow As DataGridItem In dgrVaccinazioni.Items
            If DirectCast(assRow.Cells(IndexDgrVac.CheckSelezione).FindControl("chkSelezionaItem"), CheckBox).Checked Then
                If assRow.ItemIndex <> currentItemIndex Then
                    DirectCast(assRow.Cells(IndexDgrVac.DataEsecuzione).FindControl("txtDataItem"), OnitDatePick).Text = DirectCast(currentItem.Cells(IndexDgrVac.DataEsecuzione).FindControl("txtDataItem"), OnitDatePick).Text

                    Dim cmbLuogo As OnitCombo = DirectCast(assRow.Cells(IndexDgrVac.LuogoEsecuzione).FindControl("cmbLuogo"), OnitCombo)
                    cmbLuogo.SelectedValue = DirectCast(currentItem.Cells(IndexDgrVac.LuogoEsecuzione).FindControl("cmbLuogo"), OnitCombo).SelectedValue
                    cmbLuogo_SelectedIndexChanged(cmbLuogo, New EventArgs)


                    Dim cmbTipoErogatore As OnitCombo = DirectCast(assRow.Cells(IndexDgrVac.LuogoEsecuzione).FindControl("cmbTipoErogatore"), OnitCombo)
                    cmbTipoErogatore.SelectedValue = DirectCast(currentItem.Cells(IndexDgrVac.TipoErogatore).FindControl("cmbTipoErogatore"), OnitCombo).SelectedValue
                    cmbTipoErogatore_SelectedIndexChanged(cmbTipoErogatore, New EventArgs)

                    Dim txtConsultorioItem As OnitModalList = DirectCast(assRow.Cells(IndexDgrVac.Consultorio).FindControl("txtConsultorioItem"), OnitModalList)
                    txtConsultorioItem.Codice = DirectCast(currentItem.Cells(IndexDgrVac.Consultorio).FindControl("txtConsultorioItem"), OnitModalList).Codice
                    txtConsultorioItem.RefreshDataBind()

                    SetLayout(assRow)
                End If
            End If
        Next
    End Sub

#End Region

End Class
