Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager


Public Class AttivazioneLotto
    Inherits Common.UserControlPageBase

#Region " Properties "

    Public Property ToolTip As String
        Get
            Return Me.btnAttivaDisattivaLotto.ToolTip
        End Get
        Set(value As String)
            Me.btnAttivaDisattivaLotto.ToolTip = value
        End Set
    End Property

    Public Property DatiLottoCorrente() As DatiLotto
        Get
            Return ViewState("_DatiLottoCorrente")
        End Get
        Set(value As DatiLotto)
            ViewState("_DatiLottoCorrente") = value
        End Set
    End Property

    Public Property MostraStatoAttivazioneLotto() As Boolean
        Get
            If ViewState("_MostraStatoAttivazioneLotto") Is Nothing Then ViewState("_MostraStatoAttivazioneLotto") = False
            Return ViewState("_MostraStatoAttivazioneLotto")
        End Get
        Set(value As Boolean)
            ViewState("_MostraStatoAttivazioneLotto") = value
        End Set
    End Property

#End Region

#Region " Types "

    <Serializable()>
    Public Class DatiLotto

        Public CodiceLotto As String
        Public DescrizioneLotto As String
        Public CodiceConsultorio As String
        Public CodiceNomeCommerciale As String
        Public LottoAttivo As Boolean
        Public TotGiorniMinAttivazione As Integer?
        Public TotGiorniMaxAttivazione As Integer?

    End Class

    Private Class CheckLottoAttivabileResult

        Public Success As Boolean
        Public Message As String

        Public Sub New(success As Boolean, message As String)
            Me.Success = success
            Me.Message = message
        End Sub

    End Class

#End Region

#Region " Events "

    Public Event SalvaAttivazioneLotto()
    Public Event AnnullaAttivazioneLotto()
    Public Event OpenModaleAttivazioneLotto(attivaLotto As Boolean)
    Public Event ShowMessage(message As String)

#End Region

#Region " Page Events "

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        Me.SetLayout()

    End Sub

#End Region

#Region " Button events "

    Private Sub btnAttivaDisattivaLotto_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnAttivaDisattivaLotto.Click

        Me.ToggleAttivazioneLotto()

    End Sub

#End Region

#Region " Public "

    Public Sub InitDatiLottoCorrente(datiLotto As DatiLotto)

        Me.DatiLottoCorrente = New DatiLotto()

        Me.DatiLottoCorrente.CodiceLotto = datiLotto.CodiceLotto
        Me.DatiLottoCorrente.DescrizioneLotto = datiLotto.DescrizioneLotto
        Me.DatiLottoCorrente.CodiceConsultorio = datiLotto.CodiceConsultorio
        Me.DatiLottoCorrente.CodiceNomeCommerciale = datiLotto.CodiceNomeCommerciale
        Me.DatiLottoCorrente.LottoAttivo = datiLotto.LottoAttivo
        Me.DatiLottoCorrente.TotGiorniMinAttivazione = datiLotto.TotGiorniMinAttivazione
        Me.DatiLottoCorrente.TotGiorniMaxAttivazione = datiLotto.TotGiorniMaxAttivazione

        Me.SetLayout()

    End Sub

#End Region

#Region " Private "

    Private Sub SetLayout()

        If Me.MostraStatoAttivazioneLotto AndAlso Not Me.DatiLottoCorrente Is Nothing Then

            If Me.DatiLottoCorrente.LottoAttivo Then
                Me.ToolTip = "Lotto attivo, click per disattivarlo"
                Me.btnAttivaDisattivaLotto.ImageUrl = Me.ResolveClientUrl("~/images/lottoAttivo.gif")
            Else
                Me.ToolTip = "Lotto disattivato, click per attivarlo"
                Me.btnAttivaDisattivaLotto.ImageUrl = Me.ResolveClientUrl("~/images/star.png")
            End If

        Else
            If String.IsNullOrEmpty(Me.ToolTip) Then Me.ToolTip = "Attiva/Disattiva il lotto"
            Me.btnAttivaDisattivaLotto.ImageUrl = Me.ResolveClientUrl("~/images/star.png")
        End If

    End Sub

    Private Sub ToggleAttivazioneLotto()

        If Me.DatiLottoCorrente Is Nothing Then Return

        If Not Me.DatiLottoCorrente.LottoAttivo Then

            ' Lotto da attivare: controlla se è attivabile nel consultorio corrente.
            Dim result As CheckLottoAttivabileResult = Me.CheckLottoAttivabile()

            If Not result.Success Then
                RaiseEvent ShowMessage("Impossibile eseguire l'attivazione del lotto:\n" + result.Message)
                Return
            End If

        End If

        RaiseEvent OpenModaleAttivazioneLotto(Me.DatiLottoCorrente.LottoAttivo)

        ' Conferma attivazione/disattivazione
        Me.ApriModaleAttivazioneLotto()

    End Sub

    ' Controlla se il lotto si può attivare nel consultorio specificato e restituisce un messaggio in caso negativo.
    ' Restituisce il motivo per cui il lotto non è attivabile. Se è attivabile restituisce stringa vuota
    Private Function CheckLottoAttivabile() As CheckLottoAttivabileResult

        Dim success As Boolean = True
        Dim msgErroreAttivazione As String = String.Empty

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizLotti As New Biz.BizLotti(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                Dim bizLottiResult As Biz.BizLotti.BizLottiResult =
                    bizLotti.IsLottoAttivabile(False, Me.DatiLottoCorrente.CodiceLotto, Me.DatiLottoCorrente.CodiceNomeCommerciale, Me.DatiLottoCorrente.CodiceConsultorio,
                                               OnVacUtility.Variabili.CNS.Codice, OnVacUtility.Variabili.CNSMagazzino.Codice)

                If bizLottiResult.Result <> Biz.BizLotti.BizLottiResult.ResultType.Success Then

                    success = False
                    msgErroreAttivazione = bizLottiResult.Message

                End If

            End Using

        End Using

        Return New CheckLottoAttivabileResult(success, msgErroreAttivazione)

    End Function

#Region " Finestra modale attivazione lotto "

    Private Sub ApriModaleAttivazioneLotto()
        '--
        ' N.B. : qui non uso gli u.c. CampiEtaAttivazione per non rischiare che succeda come per il magazzino: uc ripetuti in ogni riga del datagrid, ad un certo punto
        '        alcuni browser non riuscivano più a gestire la renderizzazione. Qui sono meno e funziona, ma aggiungendone due per ogni riga non so cosa possa succedere...
        '--
        Me.txtEtaMinAnni.Text = String.Empty
        Me.txtEtaMinMesi.Text = String.Empty
        Me.txtEtaMinGiorni.Text = String.Empty
        '--
        Me.txtEtaMaxAnni.Text = String.Empty
        Me.txtEtaMaxMesi.Text = String.Empty
        Me.txtEtaMaxGiorni.Text = String.Empty
        '--
        If Not Me.DatiLottoCorrente.LottoAttivo Then
            '--
            Me.modAttivazioneLotto.Title = "&nbsp;Attivazione lotto"
            Me.lblWarningOperazione.Text = "Il lotto verrà ATTIVATO."
            '--
            Me.lblEtaMin.Visible = Me.Settings.ASSOCIA_LOTTI_ETA
            Me.txtEtaMinAnni.Visible = Me.Settings.ASSOCIA_LOTTI_ETA
            Me.lblEtaMinAnni.Visible = Me.Settings.ASSOCIA_LOTTI_ETA
            Me.txtEtaMinMesi.Visible = Me.Settings.ASSOCIA_LOTTI_ETA
            Me.lblEtaMinMesi.Visible = Me.Settings.ASSOCIA_LOTTI_ETA
            Me.txtEtaMinGiorni.Visible = Me.Settings.ASSOCIA_LOTTI_ETA
            Me.lblEtaMinGiorni.Visible = Me.Settings.ASSOCIA_LOTTI_ETA
            '--
            Me.lblEtaMax.Visible = Me.Settings.ASSOCIA_LOTTI_ETA
            Me.txtEtaMaxAnni.Visible = Me.Settings.ASSOCIA_LOTTI_ETA
            Me.lblEtaMaxAnni.Visible = Me.Settings.ASSOCIA_LOTTI_ETA
            Me.txtEtaMaxMesi.Visible = Me.Settings.ASSOCIA_LOTTI_ETA
            Me.lblEtaMaxMesi.Visible = Me.Settings.ASSOCIA_LOTTI_ETA
            Me.txtEtaMaxGiorni.Visible = Me.Settings.ASSOCIA_LOTTI_ETA
            Me.lblEtaMaxGiorni.Visible = Me.Settings.ASSOCIA_LOTTI_ETA
            '--
            If Me.DatiLottoCorrente.TotGiorniMinAttivazione.HasValue Then
                '--
                Dim etaMinAttivazione As New Entities.Eta(Me.DatiLottoCorrente.TotGiorniMinAttivazione.Value)
                '--
                Me.txtEtaMinAnni.Text = etaMinAttivazione.Anni.ToString()
                Me.txtEtaMinMesi.Text = etaMinAttivazione.Mesi.ToString()
                Me.txtEtaMinGiorni.Text = etaMinAttivazione.Giorni.ToString()
            End If
            '--
            If Me.DatiLottoCorrente.TotGiorniMaxAttivazione.HasValue Then
                '--
                Dim etaMaxAttivazione As New Entities.Eta(Me.DatiLottoCorrente.TotGiorniMaxAttivazione.Value)
                '--
                Me.txtEtaMaxAnni.Text = etaMaxAttivazione.Anni.ToString()
                Me.txtEtaMaxMesi.Text = etaMaxAttivazione.Mesi.ToString()
                Me.txtEtaMaxGiorni.Text = etaMaxAttivazione.Giorni.ToString()
            End If
            '--
        Else
            '--
            Me.modAttivazioneLotto.Title = "&nbsp;Disattivazione lotto"
            Me.lblWarningOperazione.Text = "Il lotto verrà DISATTIVATO."
            '--
            Me.lblEtaMin.Visible = False
            Me.txtEtaMinAnni.Visible = False
            Me.lblEtaMinAnni.Visible = False
            Me.txtEtaMinMesi.Visible = False
            Me.lblEtaMinMesi.Visible = False
            Me.txtEtaMinGiorni.Visible = False
            Me.lblEtaMinGiorni.Visible = False
            '--
            Me.lblEtaMax.Visible = False
            Me.txtEtaMaxAnni.Visible = False
            Me.lblEtaMaxAnni.Visible = False
            Me.txtEtaMaxMesi.Visible = False
            Me.lblEtaMaxMesi.Visible = False
            Me.txtEtaMaxGiorni.Visible = False
            Me.lblEtaMaxGiorni.Visible = False
            '--
        End If
        '--
        Me.lblCodiceLotto.Text = "Codice: " + Me.DatiLottoCorrente.CodiceLotto
        Me.lblDescrizioneLotto.Text = "Descrizione: " + Me.DatiLottoCorrente.DescrizioneLotto
        '--
        Me.modAttivazioneLotto.VisibileMD = True
        '--
    End Sub

    Private Function GetGiorniTotaliAttivazioneFromCampi(txtAnni As TextBox, txtMesi As TextBox, txtGiorni As TextBox) As Integer?

        Dim etaAttivazione As Entities.Eta = Biz.BizLotti.GetEtaFromValoreCampi(txtAnni.Text, txtMesi.Text, txtGiorni.Text)
        '--
        Dim totGiorniEta As Integer? = Nothing
        '--
        If etaAttivazione Is Nothing Then
            '--
            txtAnni.Text = String.Empty
            txtMesi.Text = String.Empty
            txtGiorni.Text = String.Empty
            '--
        Else
            '--
            txtAnni.Text = etaAttivazione.Anni.ToString()
            txtMesi.Text = etaAttivazione.Mesi.ToString()
            txtGiorni.Text = etaAttivazione.Giorni.ToString()
            '--
            totGiorniEta = etaAttivazione.GiorniTotali
            '--
        End If
        '--
        Return totGiorniEta
        '--
    End Function

    Private Sub btnSalvaAttivazioneLotto_Click(sender As Object, e As System.EventArgs) Handles btnSalvaAttivazioneLotto.Click

        ' Controlli età attivazione
        Dim totGiorniEtaMinima As Integer? = Nothing
        Dim totGiorniEtaMassima As Integer? = Nothing

        Dim lottoDaAttivare As Boolean = Not Me.DatiLottoCorrente.LottoAttivo

        If lottoDaAttivare Then
            '--
            totGiorniEtaMinima = GetGiorniTotaliAttivazioneFromCampi(Me.txtEtaMinAnni, Me.txtEtaMinMesi, Me.txtEtaMinGiorni)
            totGiorniEtaMassima = GetGiorniTotaliAttivazioneFromCampi(Me.txtEtaMaxAnni, Me.txtEtaMaxMesi, Me.txtEtaMaxGiorni)
            '--
            If totGiorniEtaMinima.HasValue AndAlso totGiorniEtaMassima.HasValue AndAlso totGiorniEtaMinima.Value > totGiorniEtaMassima.Value Then
                '--
                RaiseEvent ShowMessage("Impossibile eseguire l'attivazione del lotto:\nl'Eta minima non può superare l'Eta massima.")
                Return
                '--
            End If
            '--
        End If

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            genericProvider.BeginTransaction()

            Try
                Dim listTestateLog As New List(Of Testata)()

                Using bizLotti As New Biz.BizLotti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                    If lottoDaAttivare Then
                        bizLotti.AttivaLotto(Me.DatiLottoCorrente.CodiceLotto, OnVacUtility.Variabili.CNS.Codice, 0, totGiorniEtaMinima, totGiorniEtaMassima, listTestateLog)
                    Else
                        bizLotti.DisattivaLotto(Me.DatiLottoCorrente.CodiceLotto, OnVacUtility.Variabili.CNS.Codice, listTestateLog)
                    End If

                End Using

                genericProvider.Commit()

                ' Scrittura Log
                For Each testata As Testata In listTestateLog
                    LogBox.WriteData(testata)
                Next

            Catch exc As Exception

                If Not genericProvider Is Nothing Then genericProvider.Rollback()

                exc.InternalPreserveStackTrace()
                Throw

            End Try

        End Using

        Me.modAttivazioneLotto.VisibileMD = False

        RaiseEvent SalvaAttivazioneLotto()

    End Sub

    Private Sub btnAnnullaAttivazioneLotto_Click(sender As Object, e As System.EventArgs) Handles btnAnnullaAttivazioneLotto.Click

        Me.modAttivazioneLotto.VisibileMD = False

        RaiseEvent AnnullaAttivazioneLotto()

    End Sub

#End Region

#End Region

End Class