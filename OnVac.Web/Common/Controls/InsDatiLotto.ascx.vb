Imports Onit.OnAssistnet.OnVac.Entities

Partial  Class InsDatiLotto
    Inherits Common.UserControlPageBase

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

#Region " Enums "

    Public Enum Mode
        Modifica
        Nuovo
    End Enum

#End Region

#Region " Public Members "

    Public codLottoClientID As String
    Public descLottoClientID As String
    Public codNCClientID As String
    Public dataPreparazioneClientID As String
    Public dataScadenzaClientID As String
    Public fornitoreClientID As String
    Public noteClientID As String
    Public dosiScatolaClientID As String
    Public qtaMinimaClientID As String
    Public dosiRimasteClientID As String
    Public lottoAnnullatoClientID As String
    Public qtaInizialeClientID As String

#End Region

#Region " Properties "

    ' Marco:siccome ho uno UserControl dentro una modale(MOD1), il quale apre un'altra modale(MOD2)
    ' devo passare il nome della modale(MOD1) alla quale tornare quando MOD2 
    ' viene chiusa
    Public Property ModaleName() As String
        Get
            Return ViewState("OnVac_NomeModale")
        End Get
        Set(Value As String)
            ViewState("OnVac_NomeModale") = Value
        End Set
    End Property

    Public Property Modalita() As Mode
        Get
            Return DirectCast(ViewState("OnVac_ModalitaLotto"), Mode)
        End Get
        Set(Value As Mode)
            ViewState("OnVac_ModalitaLotto") = Value

            If Value = Mode.Modifica Then

                lbUnitaMisura.Visible = False

                tb_codLotto.ReadOnly = True
                tb_codLotto.CssClass = "textbox_stringa_disabilitato"

                rb_dose.Visible = False
                rb_scat.Visible = False

                tb_quantitaIniz.ReadOnly = True
                tb_quantitaIniz.CssClass = "textbox_numerico_disabilitato"

            Else

                lbUnitaMisura.Visible = True

                tb_codLotto.ReadOnly = False
                tb_codLotto.CssClass = "textbox_stringa_obbligatorio"

                rb_dose.Visible = True
                rb_scat.Visible = True

                tb_quantitaIniz.ReadOnly = False
                tb_quantitaIniz.CssClass = "textbox_numerico"

            End If

        End Set
    End Property

    ' Flag per indicare se i dettagli del lotto devono essere visualizzati dal magazzino centrale o da uno dei locali 
    Public Property IsMagazzinoCentrale() As Boolean
        Get
            If ViewState("IsMagazzinoCentrale_InsDatiLotto") Is Nothing Then
                ViewState("IsMagazzinoCentrale_InsDatiLotto") = False
            End If
            Return ViewState("IsMagazzinoCentrale_InsDatiLotto")
        End Get
        Set(value As Boolean)
            ViewState("IsMagazzinoCentrale_InsDatiLotto") = value
        End Set
    End Property

#End Region

#Region " Page Events "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        codLottoClientID = tb_codLotto.ClientID
        descLottoClientID = tb_descLotto.ClientID
        codNCClientID = fm_nomeCom.ClientID
        dataPreparazioneClientID = tb_dataPrep.ClientID
        dataScadenzaClientID = tb_dataScad.ClientID
        fornitoreClientID = fmFornitore.ClientID
        noteClientID = tb_note.ClientID
        dosiScatolaClientID = tb_dosiScat.ClientID
        qtaMinimaClientID = tb_quantitaMin.ClientID
        dosiRimasteClientID = tb_dosiRimaste.ClientID
        qtaInizialeClientID = tb_quantitaIniz.ClientID
        lottoAnnullatoClientID = cb_Annullato.ClientID

        ' Trim degli spazi nel codice lotto
        tb_codLotto.Text = tb_codLotto.Text.Trim().ToUpper()

    End Sub

    Private Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles MyBase.PreRender

        If IsMagazzinoCentrale Then

            ' Se il magazzino da cui vengono visualizzati i dati del lotto è il centrale, 
            ' il flag di attivazione e i relativi campi età non devono essere visibili 
            ' e il campo con la quantità iniziale deve essere disabilitato
            cb_Attivo.Visible = False
            tb_quantitaIniz.Enabled = False

            ucEtaMinAttivazione.Visible = False
            ucEtaMaxAttivazione.Visible = False

        Else

            ' Se il magazzino è uno dei locali, il flag di attivazione e i relativi campi età non devono essere visibili
            ' se entrambi i parametri di magazzino sono valorizzati a true
            If Settings.GESMAG AndAlso Settings.GESBALOT Then
                cb_Attivo.Visible = False
                ucEtaMinAttivazione.Visible = False
                ucEtaMaxAttivazione.Visible = False
            Else
                ucEtaMinAttivazione.Visible = Settings.ASSOCIA_LOTTI_ETA
                ucEtaMaxAttivazione.Visible = Settings.ASSOCIA_LOTTI_ETA
            End If

        End If

        'se il parametro GESDOSISCATOLA è valorizzato a "N" 
        ' devono essere disabilitati l'inserimento delle dosi per scatola e l'unità di misura "dose"
        If Not Settings.GESDOSISCATOLA Then
            lblDosiScatola.Visible = False
            tb_dosiScat.Visible = False
            lbUnitaMisura.Visible = False
            rb_dose.Visible = False
            rb_scat.Visible = False
        End If

    End Sub

#End Region

#Region " Eventi finestra modale "

    'carica la descrizione del lotto associata al nome commerciale (modifica 16/07/2004)
    Private Sub fm_nomeCom_Change(sender As Object, e As Onit.Controls.OnitModalList.ModalListaEventArgument) Handles fm_nomeCom.Change

        tb_descLotto.Text = tb_codLotto.Text

        fm_nomeCom.RefreshDataBind()
        fmFornitore.Codice = fm_nomeCom.GetAltriCampi(2)
        fmFornitore.Descrizione = fm_nomeCom.GetAltriCampi(3)

    End Sub

#End Region

#Region " Public Methods "

    Public Function GetJSFocusLotto() As String

        If tb_codLotto.ReadOnly Then
            Return "document.getElementById(""" & tb_descLotto.ClientID & """).focus()"
        End If

        Return "document.getElementById(""" & tb_codLotto.ClientID & """).focus()"

    End Function

    Public Sub SetLottoMagazzino(lottoMagazzino As Entities.LottoMagazzino)

        If lottoMagazzino Is Nothing Then lottoMagazzino = New Entities.LottoMagazzino()

        tb_codLotto.Text = lottoMagazzino.CodiceLotto
        tb_descLotto.Text = lottoMagazzino.DescrizioneLotto
        fm_nomeCom.Codice = lottoMagazzino.CodiceNomeCommerciale
        fm_nomeCom.Descrizione = lottoMagazzino.DescrizioneNomeCommerciale

        If Not String.IsNullOrEmpty(lottoMagazzino.CodiceNomeCommerciale) Then
            If Not fm_nomeCom.RefreshDataBind() Then
                fm_nomeCom.Descrizione = lottoMagazzino.DescrizioneNomeCommerciale
                fm_nomeCom.Codice = String.Empty
            End If
        End If

        If lottoMagazzino.DataPreparazione = DateTime.MinValue Then
            tb_dataPrep.Text = String.Empty
        Else
            tb_dataPrep.Data = lottoMagazzino.DataPreparazione
        End If

        If lottoMagazzino.DataScadenza = DateTime.MinValue Then
            tb_dataScad.Text = String.Empty
        Else
            tb_dataScad.Text = lottoMagazzino.DataScadenza
        End If

        tb_dosiScat.Text = IIf(lottoMagazzino.DosiScatola = 0, 1, lottoMagazzino.DosiScatola)
        tb_fornitore.Text = lottoMagazzino.Ditta
        tb_dosiRimaste.Text = lottoMagazzino.DosiRimaste.ToString()
        tb_quantitaMin.Text = lottoMagazzino.QuantitaMinima.ToString()
        tb_quantitaIniz.Text = lottoMagazzino.QuantitaIniziale.ToString()

        cb_Annullato.Checked = lottoMagazzino.Obsoleto
        cb_Attivo.Checked = lottoMagazzino.Attivo

        tb_note.Text = lottoMagazzino.Note

        rb_dose.Checked = (lottoMagazzino.UnitaMisura = Enumerators.UnitaMisuraLotto.Dose)
        rb_scat.Checked = Not (lottoMagazzino.UnitaMisura = Enumerators.UnitaMisuraLotto.Dose)

        fm_nomeCom.RefreshDataBind()
        fmFornitore.Codice = fm_nomeCom.GetAltriCampi(2)        ' Codice Fornitore
        fmFornitore.Descrizione = fm_nomeCom.GetAltriCampi(3)   ' Descrizione Fornitore

        ucEtaMinAttivazione.SetGiorni(lottoMagazzino.EtaMinimaAttivazione)
        ucEtaMaxAttivazione.SetGiorni(lottoMagazzino.EtaMassimaAttivazione)

    End Sub

    Public Function GetCodiceLotto() As String

        tb_codLotto.Text = tb_codLotto.Text.Trim().ToUpper()

        Return tb_codLotto.Text

    End Function

    Public Function GetQuantitaIniziale() As Integer

        Return GetInt32FromTextBox(tb_quantitaIniz)

    End Function

    Public Function GetUnitaMisuraDose() As Enumerators.UnitaMisuraLotto

        If rb_dose.Checked Then
            Return Enumerators.UnitaMisuraLotto.Dose
        End If

        Return Enumerators.UnitaMisuraLotto.Scatola

    End Function

    Public Function GetLottoMagazzino() As Entities.LottoMagazzino

        tb_codLotto.Text = tb_codLotto.Text.Trim().ToUpper()

        Dim lottoMagazzino As New Entities.LottoMagazzino()

        lottoMagazzino.CodiceLotto = tb_codLotto.Text
        lottoMagazzino.DescrizioneLotto = tb_descLotto.Text
        lottoMagazzino.CodiceNomeCommerciale = fm_nomeCom.Codice
        lottoMagazzino.DescrizioneNomeCommerciale = fm_nomeCom.Descrizione

        If String.IsNullOrEmpty(tb_dataPrep.Text) Then
            lottoMagazzino.DataPreparazione = DateTime.MinValue
        Else
            lottoMagazzino.DataPreparazione = tb_dataPrep.Data
        End If

        If String.IsNullOrEmpty(tb_dataScad.Text) Then
            lottoMagazzino.DataScadenza = DateTime.MinValue
        Else
            lottoMagazzino.DataScadenza = tb_dataScad.Data
        End If

        lottoMagazzino.Ditta = tb_fornitore.Text
        lottoMagazzino.Obsoleto = cb_Annullato.Checked
        lottoMagazzino.Attivo = cb_Attivo.Checked
        lottoMagazzino.Note = tb_note.Text
        lottoMagazzino.UnitaMisura = IIf(rb_dose.Checked, Enumerators.UnitaMisuraLotto.Dose, Enumerators.UnitaMisuraLotto.Scatola)

        lottoMagazzino.DosiScatola = GetInt32FromTextBox(tb_dosiScat)
        lottoMagazzino.DosiRimaste = GetInt32FromTextBox(tb_dosiRimaste)
        lottoMagazzino.QuantitaMinima = GetInt32FromTextBox(tb_quantitaMin)
        lottoMagazzino.QuantitaIniziale = GetInt32FromTextBox(tb_quantitaIniz)

        lottoMagazzino.EtaMinimaAttivazione = ucEtaMinAttivazione.GetGiorniTotali()
        lottoMagazzino.EtaMassimaAttivazione = ucEtaMaxAttivazione.GetGiorniTotali()

        Return lottoMagazzino

    End Function

    Public Function GetGiorniTotaliEtaMinimaAttivazione() As Integer?

        Return ucEtaMinAttivazione.GetGiorniTotali()

    End Function

    Public Function GetGiorniTotaliEtaMassimaAttivazione() As Integer?

        Return ucEtaMaxAttivazione.GetGiorniTotali()

    End Function

    Public Sub SetDosiRimaste(ByRef n As Int16)
        tb_dosiRimaste.Text = n
    End Sub

    Public Sub SetEnable(enabled As Boolean)

        tb_codLotto.CssClass = GetCssClass("textbox_stringa", Not tb_codLotto.ReadOnly, True)

        tb_quantitaIniz.CssClass = GetCssClass("textbox_numerico", Not tb_quantitaIniz.ReadOnly, False)

        fm_nomeCom.Enabled = enabled
        fm_nomeCom.CssClass = GetCssClass("textbox_stringa", enabled, True)

        tb_descLotto.Enabled = enabled
        tb_descLotto.CssClass = GetCssClass("textbox_stringa", enabled, True)

        tb_dataPrep.Enabled = enabled
        tb_dataPrep.CssClass = GetCssClass("textbox_data", enabled, False)

        tb_dataScad.Enabled = enabled
        tb_dataScad.CssClass = GetCssClass("textbox_data", enabled, True)

        tb_fornitore.Enabled = enabled
        fmFornitore.Enabled = False

        tb_dosiScat.Enabled = enabled
        tb_dosiScat.CssClass = GetCssClass("textbox_numerico", enabled, False)

        tb_quantitaMin.Enabled = enabled
        tb_quantitaMin.CssClass = GetCssClass("textbox_numerico", enabled, True)

        cb_Annullato.Enabled = enabled

        cb_Attivo.Enabled = enabled

        rb_scat.Enabled = enabled

        rb_dose.Enabled = enabled

        tb_note.Enabled = enabled
        tb_note.CssClass = GetCssClass("textbox_stringa", enabled, False)

        ucEtaMinAttivazione.Enabled = enabled
        ucEtaMaxAttivazione.Enabled = enabled

    End Sub

#End Region

#Region " Private Methods "

    Private Function GetCssClass(cssClassName As String, enabled As Boolean, obbligatorio As Boolean) As String

        Dim css As String() = cssClassName.Split(" ")

        If css.Length = 0 Then
            Return cssClassName
        End If

        Dim stbCssClassName As New System.Text.StringBuilder()

        If enabled Then

            If obbligatorio Then
                stbCssClassName.AppendFormat("{0}_obbligatorio", css(0))
            Else
                stbCssClassName.AppendFormat("{0}", css(0))
            End If

        Else
            stbCssClassName.AppendFormat("{0}_disabilitato", css(0))
        End If

        If css.Length > 1 Then
            For i As Integer = 1 To css.Length - 1
                stbCssClassName.AppendFormat(" {0}", css(i))
            Next
        End If

        Return stbCssClassName.ToString()

    End Function

    Private Function GetInt32FromTextBox(txt As TextBox) As Integer

        txt.Text = txt.Text.Trim()

        If String.IsNullOrEmpty(txt.Text) Then Return 0

        Dim value As Integer = 0
        If Not Int32.TryParse(txt.Text, value) Then Return 0

        Return value

    End Function

#End Region

End Class
