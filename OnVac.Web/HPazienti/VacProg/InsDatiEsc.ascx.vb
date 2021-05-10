Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.Entities


Partial Class OnVac_InsDatiEsc
    Inherits Common.UserControlFinestraModalePageBase

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

#Region " Members and Properties "

    Private Property IndiceVaccinazioneCorrente() As Int16
        Get
            Return ViewState("OnVac_IndiceVaccinazioneCorrente")
        End Get
        Set(Value As Int16)
            ViewState("OnVac_IndiceVaccinazioneCorrente") = Value
        End Set
    End Property

    ''' <summary>
    ''' Dati delle vaccinazioni da escludere
    ''' </summary>
    ''' <returns></returns>
    Private Property VaccinazioniDaEscludere As List(Of VaccinazioneProgrammataDaEscludere)
        Get
            If ViewState("VE") Is Nothing Then ViewState("VE") = New List(Of VaccinazioneProgrammataDaEscludere)()
            Return DirectCast(ViewState("VE"), List(Of VaccinazioneProgrammataDaEscludere))
        End Get
        Set(value As List(Of VaccinazioneProgrammataDaEscludere))
            ViewState("VE") = value
        End Set
    End Property

    ''' <summary>
    ''' Risultati delle esclusioni effettuate
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property DatiEsclusioniSelezionate As List(Of DatiEsclusione)
        Get
            If ViewState("des") Is Nothing Then ViewState("des") = New List(Of DatiEsclusione)()
            Return DirectCast(ViewState("des"), List(Of DatiEsclusione))
        End Get
        Set(value As List(Of DatiEsclusione))
            ViewState("des") = value
        End Set
    End Property

#End Region

#Region " Types "

    <Serializable>
    Public Class VaccinazioneProgrammataDaEscludere

        ''' <summary>
        ''' Indice della riga del datatable delle programmate
        ''' </summary>
        ''' <remarks></remarks>
        Public IndiceVaccinazioneProgrammata As Int16
        Public CodiceVaccinazione As String
        Public DescrizioneVaccinazione As String
        Public DoseVaccinazione As Int16

    End Class

    <Serializable>
    Public Class DatiEsclusione
        Public IndiceVaccinazioneProgrammata As Int16
        Public DataVisita As DateTime
        Public CodiceOperatore As String
        Public DescrizioneOperatore As String
        Public CodiceMotivoEsclusione As String
        Public NumeroDose As Int16
        Public Note As String
        Public DataScadenza As DateTime?
    End Class

    <Serializable>
    Private Class CheckDatiEsclusioneResult

        Public Success As Boolean
        Public Message As String

        Public Sub New(success As Boolean, message As String)
            Me.Success = success
            Me.Message = message
        End Sub

    End Class

#End Region

#Region " Events "

    Public Event InsDatiEscConferma(datiEsclusioniSelezionate As List(Of DatiEsclusione))

    Protected Sub OnInsDatiEscConferma(datiEsclusioniSelezionate As List(Of DatiEsclusione))
        RaiseEvent InsDatiEscConferma(datiEsclusioniSelezionate)
    End Sub

    Public Event InsDatiEscAnnulla()

    Protected Sub OnInsDatiEscAnnulla()
        RaiseEvent InsDatiEscAnnulla()
    End Sub

    Public Event ShowMessage(message As String)

    Protected Sub OnShowMessage(message As String)
        RaiseEvent ShowMessage(message)
    End Sub

#End Region

#Region " Public Methods "

    Public Sub Inizializza(vaccinazioniDaEscludere As List(Of VaccinazioneProgrammataDaEscludere))

        Me.VaccinazioniDaEscludere = vaccinazioniDaEscludere.Clone()
        '--
        Me.IndiceVaccinazioneCorrente = 0
        Me.DatiEsclusioniSelezionate = New List(Of DatiEsclusione)()
        '--
        ' Impostazione data odierna
        Me.tb_data_visita.Text = DateTime.Now.ToString("dd/MM/yyyy", Globalization.CultureInfo.InvariantCulture)
        '--
        ' Impostazione stessa dose della programmata
        Me.txtDose.Text = Me.VaccinazioniDaEscludere(Me.IndiceVaccinazioneCorrente).DoseVaccinazione.ToString()
        '--
        ' Cancellazione altri campi
        Me.fm_motivo.Codice = String.Empty
        Me.fm_motivo.Descrizione = String.Empty
        Me.fm_medico.Codice = String.Empty
        Me.fm_medico.Descrizione = String.Empty
        Me.tb_data_scadenza.Text = String.Empty
        Me.txtNote.Text = String.Empty
        '--
        Me.SetTitolo()
        '--
        ' Caricamento dati esclusione, se già presente, per avvertire l'utente
        Me.LoadEsclusione(Me.VaccinazioniDaEscludere(Me.IndiceVaccinazioneCorrente).CodiceVaccinazione)

    End Sub

    <Obsolete("NON USARE, sostituire con il metodo Inizializza(vaccinazioniDaEscludere As List(Of VaccinazioneProgrammataDaEscludere))")>
    Public Overrides Sub LoadModale()

        Throw New NotImplementedException("InsDatiEsc.LoadModale è obsoleto. Sostituire con il metodo InsDatiEsc.Inizializza")

    End Sub

#End Region

#Region " Toolbar Events "

    Private Sub ToolBar_ButtonClicked(ByVal sender As Object, ByVal e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked
        '--
        Select Case e.Button.Key
            '--
            Case "btn_Conferma"
                '--
                ' Controllo dati inseriti
                Dim result As CheckDatiEsclusioneResult = CheckDatiEsclusione()
                If Not result.Success Then
                    OnShowMessage(result.Message)
                    Return
                End If
                '--
                ' Se dati inseriti ok: aggiunta esclusione alla lista delle esclusioni che verranno restituite alla fine
                Dim datiEsclusa As New DatiEsclusione()

                datiEsclusa.IndiceVaccinazioneProgrammata = Me.VaccinazioniDaEscludere(Me.IndiceVaccinazioneCorrente).IndiceVaccinazioneProgrammata
                datiEsclusa.DataVisita = Me.tb_data_visita.Data
                datiEsclusa.CodiceOperatore = Me.fm_medico.Codice
                datiEsclusa.DescrizioneOperatore = Me.fm_medico.Descrizione
                datiEsclusa.CodiceMotivoEsclusione = Me.fm_motivo.Codice
                datiEsclusa.NumeroDose = Int16.Parse(Me.txtDose.Text, Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture)
                datiEsclusa.Note = Me.txtNote.Text.Trim()

                If Me.tb_data_scadenza.Data > DateTime.MinValue Then
                    datiEsclusa.DataScadenza = Me.tb_data_scadenza.Data
                Else
                    datiEsclusa.DataScadenza = Nothing
                End If

                Me.DatiEsclusioniSelezionate.Add(datiEsclusa)
                '--
                ' Se quella corrente è l'ultima esclusione => termina
                ' Altrimenti => passa all'esclusione successiva
                Me.CheckLastProcedure()
                '--
            Case "btn_Annulla"
                '--
                Me.CheckLastProcedure()
                '--
        End Select
        '--
    End Sub

    Private Function CheckDatiEsclusione() As CheckDatiEsclusioneResult

        If Me.tb_data_visita.Data = DateTime.MinValue Then
            Return New CheckDatiEsclusioneResult(False, "La data della visita è obbligatoria.")
        End If

        If Me.tb_data_visita.Data > DateTime.Now.Date Then
            Return New CheckDatiEsclusioneResult(False, "La data della visita non può essere futura.")
        End If

        If String.IsNullOrWhiteSpace(Me.fm_motivo.Codice) OrElse String.IsNullOrWhiteSpace(Me.fm_motivo.Descrizione) Then
            Return New CheckDatiEsclusioneResult(False, "Il motivo di esclusione è obbligatorio.")
        End If

        If Me.tb_data_scadenza.Data > DateTime.Now.Date AndAlso Me.tb_data_scadenza.Data < Me.tb_data_visita.Data Then
            Return New CheckDatiEsclusioneResult(False, "La data di scadenza non può essere precedente alla visita.")
        End If

        Me.txtDose.Text = Me.txtDose.Text.Trim()

        Dim dose As Int16 = 0
        If Not Int16.TryParse(Me.txtDose.Text, Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, dose) Then
            Return New CheckDatiEsclusioneResult(False, "La dose è obbligatoria.")
        End If

        Return New CheckDatiEsclusioneResult(True, String.Empty)

    End Function

#End Region

#Region " Private "

    Private Sub LoadEsclusione(codiceVaccinazione As String)

        Dim vaccinazioneEsclusa As Entities.VaccinazioneEsclusa = Nothing

        ' Caricamento esclusione corrente
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizEscluse As New Biz.BizVaccinazioniEscluse(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                vaccinazioneEsclusa = bizEscluse.LoadVaccinazioneEsclusa(OnVacUtility.Variabili.PazId, codiceVaccinazione)

            End Using

        End Using

        Me.SetDatiVaccinazioneEsclusa(vaccinazioneEsclusa)

    End Sub

    Private Sub CheckLastProcedure()
        '--
        If Me.VaccinazioniDaEscludere.Count - 1 = Me.IndiceVaccinazioneCorrente Then
            '--
            ' Quella corrente è l'ultima esclusione => se sono state inserite esclusioni, viene sollevato l'evento "Conferma" che restituisce le esclusioni impostate
            '                                          Altrimenti, viene sollevato l'evento "Annulla"
            '--
            If Me.DatiEsclusioniSelezionate.Count > 0 Then
                Me.OnInsDatiEscConferma(Me.DatiEsclusioniSelezionate)
            Else
                Me.OnInsDatiEscAnnulla()
            End If
            '--
        Else
            '--
            ' Quella corrente non è l'ultima esclusione => passa alla successiva, caricando la data di scadenza e impostando la dose della programmata
            '--
            Me.IndiceVaccinazioneCorrente += 1
            '--
            Me.SetTitolo()
            '--
            ' Caricamento dati esclusione, se già presente, per avvertire l'utente
            Me.LoadEsclusione(Me.VaccinazioniDaEscludere(Me.IndiceVaccinazioneCorrente).CodiceVaccinazione)
            '--
            ' Impostazione stessa dose della programmata
            Me.txtDose.Text = Me.VaccinazioniDaEscludere(Me.IndiceVaccinazioneCorrente).DoseVaccinazione.ToString()
            '--
            ' La data di scadenza viene ricaricata in base al motivo, perchè potrebbe essere stata modificata.
            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using biz As New Biz.BizMotiviEsclusione(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())
                    '--
                    Me.tb_data_scadenza.Data = biz.GetScadenzaMotivoEsclusione(OnVacUtility.Variabili.PazId, Me.fm_motivo.Codice,
                                                                               Me.VaccinazioniDaEscludere(Me.IndiceVaccinazioneCorrente).CodiceVaccinazione, Me.tb_data_visita.Data)
                    '--
                End Using
            End Using
            '--
        End If
        '--
    End Sub

    Private Sub SetTitolo()

        LayoutTitolo_sezione.Text = String.Format("VACCINAZIONE [{0}/{1}]: {2}",
                                                  (IndiceVaccinazioneCorrente + 1).ToString(),
                                                  VaccinazioniDaEscludere.Count.ToString(),
                                                  VaccinazioniDaEscludere(IndiceVaccinazioneCorrente).DescrizioneVaccinazione)
    End Sub

    Private Sub SetDatiVaccinazioneEsclusa(vaccinazioneEsclusa As VaccinazioneEsclusa)

        ' Bind dati nella maschera
        If vaccinazioneEsclusa Is Nothing Then

            lblAvvisoEsclusa.Text = "La vaccinazione non è presente tra le escluse del paziente."
            lblAvvisoEsclusa.CssClass = "blueLabel"

            panelDatiEsclusione.Visible = False

            lblEsclusioneDataVisita.Text = String.Empty
            lblEsclusioneMotivo.Text = String.Empty
            lblEsclusioneMedico.Text = String.Empty
            lblEsclusioneDataScadenza.Text = String.Empty
            lblEsclusioneDose.Text = String.Empty

        Else

            lblAvvisoEsclusa.Text = "ATTENZIONE: La vaccinazione è già presente tra le escluse del paziente."
            lblAvvisoEsclusa.CssClass = "redLabel"

            panelDatiEsclusione.Visible = True

            lblEsclusioneDataVisita.Text = vaccinazioneEsclusa.DataVisita.ToString("dd/MM/yyyy", Globalization.CultureInfo.InvariantCulture)
            lblEsclusioneMotivo.Text = vaccinazioneEsclusa.DescrizioneMotivoEsclusione
            lblEsclusioneMedico.Text = vaccinazioneEsclusa.DescrizioneOperatore

            If vaccinazioneEsclusa.DataScadenza = DateTime.MinValue Then
                lblEsclusioneDataScadenza.Text = String.Empty
            Else
                lblEsclusioneDataScadenza.Text = vaccinazioneEsclusa.DataScadenza.ToString("dd/MM/yyyy", Globalization.CultureInfo.InvariantCulture)
            End If

            lblEsclusioneDose.Text = vaccinazioneEsclusa.NumeroDose

        End If

    End Sub

#End Region

#Region " ModalList Events "

    Private Sub fm_motivo_Change(Sender As Object, e As Controls.OnitModalList.ModalListaEventArgument) Handles fm_motivo.Change

        If tb_data_scadenza.Data = Date.MinValue Then

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using biz As New Biz.BizMotiviEsclusione(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                    tb_data_scadenza.Data = biz.GetScadenzaMotivoEsclusione(OnVacUtility.Variabili.PazId, fm_motivo.Codice,
                                                                            VaccinazioniDaEscludere(IndiceVaccinazioneCorrente).CodiceVaccinazione,
                                                                            tb_data_visita.Data)
                End Using
            End Using

        End If

    End Sub

#End Region

End Class
