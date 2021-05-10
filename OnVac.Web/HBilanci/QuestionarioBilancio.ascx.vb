Imports System.Collections.Generic

Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.Biz
Imports Telerik.Web.UI

Partial Class QuestionarioBilancio
    Inherits Common.UserControlPageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

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

    Private _compilazione As Enumerators.TipoCompilazioneBilancio
    Property Compilazione() As Enumerators.TipoCompilazioneBilancio
        Get
            Return _compilazione
        End Get
        Set(Value As Enumerators.TipoCompilazioneBilancio)
            _compilazione = Value
        End Set
    End Property

    Private _dtSezioni As DataTable
    Property DtSezioni() As DataTable
        Get
            Return _dtSezioni
        End Get
        Set(Value As DataTable)
            _dtSezioni = Value
        End Set
    End Property

    Private _dtDomande As DataTable
    Property DtDomande() As DataTable
        Get
            Return _dtDomande
        End Get
        Set(Value As DataTable)
            _dtDomande = Value
        End Set
    End Property

    Private _dtRisposte As DataTable
    Property DtRisposte() As DataTable
        Get
            Return _dtRisposte
        End Get
        Set(Value As DataTable)
            _dtRisposte = Value
        End Set
    End Property

    Private _dtCondizioniBilancio As DataTable
    Public Property DtCondizioniBilancio As DataTable
        Get
            Return _dtCondizioniBilancio
        End Get
        Set(value As DataTable)
            _dtCondizioniBilancio = value
        End Set
    End Property

    Private _dtRispostePossibili As DataTable
    Public Property DtRispostePossibili As DataTable
        Get
            Return _dtRispostePossibili
        End Get
        Set(value As DataTable)
            _dtRispostePossibili = value
        End Set
    End Property

    Property NumeroBilancio() As Long
        Get
            Return ViewState("QuestionarioBilancio_NumeroBilancio")
        End Get
        Set(Value As Long)
            ViewState("QuestionarioBilancio_NumeroBilancio") = Value
        End Set
    End Property

    Property CodiceMalattia() As String
        Get
            Return ViewState("QuestionarioBilancio_CodiceMalattia")
        End Get
        Set(Value As String)
            ViewState("QuestionarioBilancio_CodiceMalattia") = Value
        End Set
    End Property

    Property CodicePaziente() As Integer
        Get
            Return ViewState("QuestionarioBilancio_CodicePaziente")
        End Get
        Set(Value As Integer)
            ViewState("QuestionarioBilancio_CodicePaziente") = Value
        End Set
    End Property

    Property DataVisita() As DateTime
        Get
            Return ViewState("QuestionarioBilancio_DataVisita")
        End Get
        Set(Value As DateTime)
            ViewState("QuestionarioBilancio_DataVisita") = Value
        End Set
    End Property

    Property DataRegistrazione() As DateTime
        Get
            Return ViewState("QuestionarioBilancio_DataRegistrazione")
        End Get
        Set(Value As DateTime)
            ViewState("QuestionarioBilancio_DataRegistrazione") = Value
        End Set
    End Property

    Property IdVisita() As Integer
        Get
            Return ViewState("QuestionarioBilancio_IdVisita")
        End Get
        Set(Value As Integer)
            ViewState("QuestionarioBilancio_IdVisita") = Value
        End Set
    End Property

    Property Enabled() As Boolean
        Get
            Return ViewState("QuestionarioBilancio_Enabled")
        End Get
        Set(Value As Boolean)
            ViewState("QuestionarioBilancio_Enabled") = Value
        End Set
    End Property

#End Region

#Region " Page Events "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        Page.ClientScript.RegisterClientScriptBlock(
            Me.GetType(),
            "OnVac_QuestionarioBilancio_GestioneDisabilitazione",
            "<script type='text/javascript' src='" & Page.Request.ApplicationPath & "/HBilanci/GestioneDisabilitazione.js'></script>")

        Page.ClientScript.RegisterStartupScript(
            Me.GetType(),
            "OnVac_QuestionarioBilancio_GestioneDisabilitazione_ControllaStato",
            "<script type='text/javascript'>controllaStato();</script>")

    End Sub

#End Region

#Region " Datalist "

    Private Sub dlsQuestionarioCompleto_ItemDataBound(sender As System.Object, e As System.Web.UI.WebControls.DataListItemEventArgs) Handles dlsQuestionarioCompleto.ItemDataBound

        If e.Item.ItemIndex >= 0 Then

            Dim dlsQuestionarioSezione As DataList = e.Item.FindControl("dlsQuestionarioSezione")
            AddHandler dlsQuestionarioSezione.ItemDataBound, AddressOf dlsQuestionarioSezione_ItemDataBound

            Dim codiceSezione As String = e.Item.DataItem("OSB_SEZ_CODICE").ToString()

            dlsQuestionarioSezione.DataSource = _dtDomande.Select(String.Format("OSB_SEZ_CODICE='{0}'", codiceSezione))
            dlsQuestionarioSezione.DataBind()

            DirectCast(e.Item.FindControl("lblSezioneCodice"), Label).Text = codiceSezione
            DirectCast(e.Item.FindControl("lblSezione"), Label).Text = e.Item.DataItem("SEZ_DESCRIZIONE").ToString()
            DirectCast(e.Item.FindControl("lblSezioneN"), Label).Text = e.Item.DataItem("SEZ_N_SEZIONE").ToString()

        End If

    End Sub

    Private Sub dlsQuestionarioSezione_ItemDataBound(sender As System.Object, e As System.Web.UI.WebControls.DataListItemEventArgs)

        If e.Item.ItemIndex >= 0 Then

            Dim codiceOsservazione As String = DirectCast(e.Item.FindControl("lblCodice"), Label).Text

            Dim txtCondizioni As TextBox = DirectCast(e.Item.FindControl("txtCondizioni"), TextBox)
            txtCondizioni.Text = Me.GeneraCondizione(Me.NumeroBilancio, Me.CodiceMalattia, codiceOsservazione)

            Dim cmbRisposta As DropDownList = DirectCast(e.Item.FindControl("cmbRisposta"), DropDownList)
            Dim cmbRispostaMultipla As RadComboBox = DirectCast(e.Item.FindControl("cmbRispostaMultipla"), RadComboBox)
            Dim txtRisposta As TextBox = DirectCast(e.Item.FindControl("txtRisposta"), TextBox)

            Dim dt As DataTable = Me.GeneraRispostePossibiliOsservazione(codiceOsservazione)
            Select Case e.Item.DataItem("OSS_TIPO_RISPOSTA").ToString()
                Case Constants.TipoRispostaOsservazioneBilancio.TestoLibero
                    cmbRisposta.Visible = False
                    cmbRispostaMultipla.Visible = False
                Case Constants.TipoRispostaOsservazioneBilancio.CodificataSingola
                    cmbRispostaMultipla.Visible = False
                    txtRisposta.Visible = False
                    cmbRisposta.DataSource = dt
                    cmbRisposta.DataBind()
                    cmbRisposta.Items.Insert(0, New ListItem(String.Empty))
                Case Constants.TipoRispostaOsservazioneBilancio.CodificataMultipla
                    cmbRisposta.Visible = False
                    txtRisposta.Visible = False
                    cmbRispostaMultipla.Visible = True
                    cmbRispostaMultipla.DataSource = dt
                    cmbRispostaMultipla.DataBind()
                    'cmbRispostaMultipla.Items.Insert(0, New RadComboBoxItem(String.Empty))
                Case Constants.TipoRispostaOsservazioneBilancio.NonPrevista
                    cmbRisposta.Visible = False
                    txtRisposta.Visible = False
                    cmbRispostaMultipla.Visible = False
                Case Else
                    ' Nothing
            End Select
            Select Case Me.Compilazione

                Case Enumerators.TipoCompilazioneBilancio.Compilata,
                     Enumerators.TipoCompilazioneBilancio.RispostaPrecedente

                    ImpostaRispostaCompilata(e.Item)

                Case Enumerators.TipoCompilazioneBilancio.Predefinita

                    ImpostaRispostaPredefinita(cmbRisposta, dt)

                Case Else
                    ' nothing

            End Select

            'se la risposta è libera, visualizza solo il valore
            'se è codificata, visualizza codice + valore (modifica 18/06/2004)
            'If e.Item.DataItem("OSS_TIPO_RISPOSTA").ToString() = Constants.TipoRispostaOsservazioneBilancio.TestoLibero Then
            '    cmbRisposta.Visible = False
            'Else
            '    txtRisposta.Visible = False
            'End If


            cmbRisposta.Enabled = Me.Enabled
            txtRisposta.Enabled = Me.Enabled
            cmbRispostaMultipla.Enabled = Me.Enabled

            If Me.Enabled Then

                If e.Item.DataItem("OSB_OBBLIGATORIA").ToString() = "S" Then
                    cmbRisposta.CssClass += "TextBox_Stringa_Obbligatorio"
                    txtRisposta.CssClass += "TextBox_Stringa_Obbligatorio"
                    cmbRispostaMultipla.BackColor = Color.LightYellow
                Else
                    cmbRisposta.CssClass = "TextBox_Stringa"
                    txtRisposta.CssClass = "TextBox_Stringa"
                    cmbRispostaMultipla.CssClass = "TextBox_Stringa"
                End If

            Else
                cmbRisposta.CssClass = "TextBox_Stringa_Disabilitato"
                txtRisposta.CssClass = "TextBox_Stringa_Disabilitato"
                cmbRispostaMultipla.DropDownCssClass = "TextBox_Stringa_Disabilitato"
                cmbRispostaMultipla.BackColor = Color.LightGray
            End If

            cmbRisposta.Attributes.Add("onchange", "GestisciRisposta();")

        End If

    End Sub

    Private Function GeneraRispostePossibiliOsservazione(codiceOsservazione As String) As DataTable

        Dim dv As New DataView(Me.DtRispostePossibili)
        dv.RowFilter = String.Format("RIO_OSS_CODICE='{0}'", codiceOsservazione)

        Return dv.ToTable()

    End Function

    Private Function GeneraCondizione(numeroBilancio As Integer, codiceMalattia As String, codiceOsservazione As String) As String

        Dim dv As New DataView(Me.DtCondizioniBilancio)
        dv.RowFilter = String.Format("LRO_OSS_CODICE_COLLEGATA='{0}'", codiceOsservazione)

        Dim condizione As New System.Text.StringBuilder()

        If dv.Count > 0 Then

            For i As Integer = 0 To dv.Count - 1
                condizione.Append(dv(i)("LRO_RIS_CODICE_COLLEGATA"))
                condizione.Append("|")
                condizione.Append(dv(i)("LRO_OSS_CODICE"))
                condizione.Append("|")
                condizione.Append(dv(i)("LRO_FLAG_VISIBILE"))
                condizione.Append("|")
                condizione.Append(dv(i)("LRO_FLAG_COLLEGATA"))
                condizione.Append(":")
            Next

            condizione.Remove(condizione.Length - 1, 1)

        End If

        Return condizione.ToString()

    End Function

#End Region

#Region " Public "

    Public Overrides Sub DataBind()

        Me.dlsQuestionarioCompleto.DataSource = DtSezioni
        Me.dlsQuestionarioCompleto.DataBind()

    End Sub

    Public Enum CheckCampiQuestionarioType
        NessunErrore = 0
        Warning = 1
        ErroreBloccante = 2
    End Enum

    Public Class CheckCampiQuestionarioResult
        Public ErrorType As CheckCampiQuestionarioType
        Public Message As String
    End Class

    Public Function CheckCampiQuestionario() As CheckCampiQuestionarioResult

        Dim result As New CheckCampiQuestionarioResult()

        Dim codiciOsservazioniEmpty As New List(Of String)()
        Dim codiciOsservazioniObbligatorieEmpty As New List(Of String)()

        For Each itemQuestionarioCompleto As DataListItem In dlsQuestionarioCompleto.Items

            Dim dlsQuestionarioSezione As DataList = itemQuestionarioCompleto.FindControl("dlsQuestionarioSezione")

            For Each itemQuestionarioSezione As DataListItem In dlsQuestionarioSezione.Items

                Dim txtControlloValore As TextBox = DirectCast(itemQuestionarioSezione.FindControl("txtControlloValore"), TextBox)

                If txtControlloValore.Text <> String.Empty Then

                    Dim cmbRisposta As DropDownList = DirectCast(itemQuestionarioSezione.FindControl("cmbRisposta"), DropDownList)
                    Dim txtRisposta As TextBox = DirectCast(itemQuestionarioSezione.FindControl("txtRisposta"), TextBox)
                    Dim cmbRispostaMultipla As RadComboBox = DirectCast(itemQuestionarioSezione.FindControl("cmbRispostaMultipla"), RadComboBox)

                    If (cmbRisposta.SelectedItem Is Nothing OrElse String.IsNullOrEmpty(cmbRisposta.SelectedItem.Value)) AndAlso
                       (String.IsNullOrWhiteSpace(txtRisposta.Text)) AndAlso (cmbRispostaMultipla.CheckedItems.IsNullOrEmpty) Then

                        Dim isValidEmpty As Boolean = False

                        If String.IsNullOrWhiteSpace(txtRisposta.Text) Then

                            Dim isAltraCollegataValorizzata As Boolean = False

                            Dim lblCodice As Label = DirectCast(itemQuestionarioSezione.FindControl("lblCodice"), Label)

                            isValidEmpty = Me.ExistsCollegataNotEmpty(lblCodice.Text)

                        End If

                        If Not isValidEmpty Then

                            codiciOsservazioniEmpty.Add(txtControlloValore.Text)

                            Dim hidObbligo As HiddenField = DirectCast(itemQuestionarioSezione.FindControl("hidObbligo"), HiddenField)
                            If Not hidObbligo Is Nothing AndAlso hidObbligo.Value = "S" Then
                                codiciOsservazioniObbligatorieEmpty.Add(txtControlloValore.Text)
                            End If

                        End If

                    End If
                End If

            Next
        Next

        Dim resultMessage As New System.Text.StringBuilder()

        If codiciOsservazioniEmpty.Count = 0 Then

            result.ErrorType = CheckCampiQuestionarioType.NessunErrore

        Else

            If codiciOsservazioniObbligatorieEmpty.Count > 0 Then

                result.ErrorType = CheckCampiQuestionarioType.ErroreBloccante

                resultMessage.AppendFormat("Le seguenti osservazioni OBBLIGATORIE non sono state valorizzate:\n{0}.",
                                           String.Join(", ", codiciOsservazioniObbligatorieEmpty.ToArray()))

            Else

                result.ErrorType = CheckCampiQuestionarioType.Warning

                resultMessage.AppendFormat("Non è stata impostata nessuna risposta alle osservazioni seguenti:\n{0}.",
                                           String.Join(", ", codiciOsservazioniEmpty.ToArray()))

            End If

        End If

        result.Message = resultMessage.ToString()

        Return result

    End Function

    Public Function GetOsservazioni(genericProvider As DAL.DbGenericProvider) As List(Of Osservazione)

        Dim osservazioneOriginaleEnumerable As IEnumerable(Of Osservazione) = Nothing

        If Me.IdVisita > 0 Then
            osservazioneOriginaleEnumerable = genericProvider.Visite.GetOsservazioniByVisita(Me.IdVisita)
        End If

        Dim listOsservazioni As New List(Of Osservazione)()

        For Each itemQuestionarioCompleto As DataListItem In dlsQuestionarioCompleto.Items

            Dim dlsQuestionarioSezione As DataList = itemQuestionarioCompleto.FindControl("dlsQuestionarioSezione")

            For Each itemQuestionarioSezione As DataListItem In dlsQuestionarioSezione.Items

                Dim osservazione As Osservazione = Nothing

                Dim idOsservazione As String = GetHiddenFieldValue(itemQuestionarioSezione, "hdIdOsservazione")

                If Not String.IsNullOrEmpty(idOsservazione) Then
                    If Not osservazioneOriginaleEnumerable Is Nothing Then
                        osservazione = osservazioneOriginaleEnumerable.First(Function(oo) oo.Id = idOsservazione)
                    End If
                End If

                If osservazione Is Nothing Then
                    osservazione = New Osservazione()
                End If

                osservazione.CodicePaziente = Me.CodicePaziente
                osservazione.CodiceMalattia = Me.CodiceMalattia
                osservazione.DataVisita = Me.DataVisita
                osservazione.IdVisita = Me.IdVisita
                osservazione.NumeroBilancio = Me.NumeroBilancio

                osservazione.SezioneCodice = Me.GetLabelText(itemQuestionarioCompleto, "lblSezioneCodice")
                osservazione.SezioneDescrizione = Me.GetLabelText(itemQuestionarioCompleto, "lblSezione")
                osservazione.SezioneNumero = Me.GetLabelText(itemQuestionarioCompleto, "lblSezioneN")

                osservazione.OsservazioneCodice = Me.GetLabelText(itemQuestionarioSezione, "lblCodice")
                osservazione.OsservazioneDescrizione = Me.GetLabelText(itemQuestionarioSezione, "lblDescrizione")

                Dim osservazioneDisabilitata As String = GetHiddenFieldValue(itemQuestionarioSezione, "lblOsservazioneDisabilitata")
                If IsNumeric(osservazioneDisabilitata) Then
                    osservazione.OsservazioneDisabilitata = True
                End If

                Dim numeroOsservazione As String = GetHiddenFieldValue(itemQuestionarioSezione, "lblOsservazioneN")
                If IsNumeric(numeroOsservazione) Then
                    osservazione.OsservazioneNumero = Convert.ToInt32(numeroOsservazione)
                End If

                ' Risposta codificata
                If DirectCast(itemQuestionarioSezione.FindControl("txtControlloValore"), TextBox).Text <> "" Then

                    Dim cmbRisposta As DropDownList = DirectCast(itemQuestionarioSezione.FindControl("cmbRisposta"), DropDownList)

                    If cmbRisposta.SelectedItem Is Nothing OrElse cmbRisposta.SelectedValue = String.Empty Then
                        '' multipla
                        Dim cmbRispostaMultipla As RadComboBox = DirectCast(itemQuestionarioSezione.FindControl("cmbRispostaMultipla"), RadComboBox)
                        If cmbRispostaMultipla.CheckedItems.IsNullOrEmpty Then
                            osservazione.RispostaCodice = String.Empty
                            osservazione.RispostaDescrizione = String.Empty
                        Else
                            Dim listaCodici As String = String.Empty
                            Dim listaDescrizioni As String = String.Empty
                            For Each ss As RadComboBoxItem In cmbRispostaMultipla.CheckedItems
                                listaCodici = listaCodici + ss.Value + "|"
                                listaDescrizioni = listaDescrizioni + ss.Text + "|"
                            Next
                            osservazione.RispostaCodice = listaCodici.Substring(0, listaCodici.Length - 1)
                            osservazione.RispostaDescrizione = listaDescrizioni.Substring(0, listaDescrizioni.Length - 1)
                        End If
                    Else
                        osservazione.RispostaCodice = cmbRisposta.SelectedItem.Value
                        osservazione.RispostaDescrizione = cmbRisposta.SelectedItem.Text
                    End If




                Else
                    osservazione.RispostaCodice = String.Empty
                    osservazione.RispostaDescrizione = String.Empty
                End If

                ' Risposta a testo libero
                Dim txtRispostaTestoLibero As TextBox = DirectCast(itemQuestionarioSezione.FindControl("txtRisposta"), TextBox)

                txtRispostaTestoLibero.Text = txtRispostaTestoLibero.Text.Trim()

                If txtRispostaTestoLibero.Text.Length > txtRispostaTestoLibero.MaxLength Then
                    txtRispostaTestoLibero.Text = txtRispostaTestoLibero.Text.Substring(0, txtRispostaTestoLibero.MaxLength)
                End If

                osservazione.RispostaTesto = txtRispostaTestoLibero.Text

                listOsservazioni.Add(osservazione)

            Next
        Next

        Return listOsservazioni

    End Function

#End Region

#Region " Private "

    Private Function GetHiddenFieldValue(dataListItem As DataListItem, hiddenFieldId As String) As String

        Dim hiddenField As HiddenField = DirectCast(dataListItem.FindControl(hiddenFieldId), HiddenField)

        If hiddenField Is Nothing Then Return String.Empty

        Return hiddenField.Value

    End Function

    Private Function GetLabelText(item As DataListItem, labelId As String) As String

        Dim lbl As Label = DirectCast(item.FindControl(labelId), Label)

        If lbl Is Nothing Then Return String.Empty

        Return lbl.Text

    End Function

    Private Function IfRispostaEmpty(codiceOsservazione As String) As Boolean

        Dim lblCodice As Label = Nothing

        For Each itemQuestionarioCompleto As DataListItem In dlsQuestionarioCompleto.Items

            Dim dlsQuestionarioSezione As DataList = itemQuestionarioCompleto.FindControl("dlsQuestionarioSezione")

            For Each itemQuestionarioSezione As DataListItem In dlsQuestionarioSezione.Items

                lblCodice = DirectCast(itemQuestionarioSezione.FindControl("lblCodice"), Label)

                If lblCodice.Text = codiceOsservazione Then

                    Dim cmbRisposta As DropDownList = DirectCast(itemQuestionarioSezione.FindControl("cmbRisposta"), DropDownList)
                    Dim cmbRispostaMultipla As RadComboBox = DirectCast(itemQuestionarioSezione.FindControl("cmbRispostaMultipla"), RadComboBox)
                    Dim txtRisposta As TextBox = DirectCast(itemQuestionarioSezione.FindControl("txtRisposta"), TextBox)

                    Return (cmbRisposta.SelectedItem Is Nothing OrElse cmbRisposta.SelectedItem.Value = String.Empty) AndAlso txtRisposta.Text = String.Empty AndAlso cmbRispostaMultipla.CheckedItems.IsNullOrEmpty

                End If

            Next
        Next

        Return False

    End Function

    ' Imposta nei campi i valori caricati da db
    Private Sub ImpostaRispostaCompilata(dataListCurrentItem As DataListItem)

        If Not DtRisposte Is Nothing Then

            Dim codiceOsservazione As String = DirectCast(dataListCurrentItem.FindControl("lblCodice"), Label).Text

            Dim rowRisposta As DataRow = (From row As DataRow In Me.DtRisposte.Rows
                                          Where row("VOS_OSS_CODICE").ToString() = codiceOsservazione
                                          Select row).FirstOrDefault()

            If Not rowRisposta Is Nothing Then

                Dim codiceRisposta As String = rowRisposta("VOS_RIS_CODICE").ToString()

                Dim cmbRisposta As DropDownList = DirectCast(dataListCurrentItem.FindControl("cmbRisposta"), DropDownList)
                If Not cmbRisposta.Items.FindByValue(codiceRisposta) Is Nothing Then
                    cmbRisposta.Items.FindByValue(codiceRisposta).Selected = True
                End If

                Dim cmbRispostaMultipla As RadComboBox = DirectCast(dataListCurrentItem.FindControl("cmbRispostaMultipla"), RadComboBox)
                If Not String.IsNullOrWhiteSpace(codiceRisposta) Then
                    For Each i As String In codiceRisposta.Split("|")
                        If Not cmbRispostaMultipla.Items.FindItemByValue(i) Is Nothing Then
                            cmbRispostaMultipla.Items.FindItemByValue(i).Checked = True
                        End If
                    Next


                End If

                DirectCast(dataListCurrentItem.FindControl("txtRisposta"), TextBox).Text = rowRisposta("VOS_RISPOSTA").ToString()

                    Dim hdIdOsservazione As HiddenField = DirectCast(dataListCurrentItem.FindControl("hdIdOsservazione"), HiddenField)
                    hdIdOsservazione.Value = rowRisposta("VOS_ID").ToString()

                End If

            End If

    End Sub

    Private Sub ImpostaRispostaPredefinita(ByRef ddl As DropDownList, dtRispostePossibili As DataTable)

        If dtRispostePossibili.Select("RIO_DEFAULT = 'S'").Length > 0 Then
            ddl.SelectedValue = dtRispostePossibili.Select("RIO_DEFAULT = 'S'")(0).Item("RIS_CODICE")
        End If

    End Sub

    Private Function ExistsCollegataNotEmpty(codiceOsservazione As String) As Boolean

        Dim collegataNotEmpty As Boolean = False

        For Each dlsQuestionarioCompletoItem As DataListItem In dlsQuestionarioCompleto.Items

            Dim dlsQuestionarioSezione As DataList = dlsQuestionarioCompletoItem.FindControl("dlsQuestionarioSezione")

            For Each dlsQuestionarioSezioneItem As DataListItem In dlsQuestionarioSezione.Items

                Dim lblCodice As Label = DirectCast(dlsQuestionarioSezioneItem.FindControl("lblCodice"), Label)

                If lblCodice.Text <> codiceOsservazione Then

                    Dim txtCondizioni As TextBox = DirectCast(dlsQuestionarioSezioneItem.FindControl("txtCondizioni"), TextBox)

                    If txtCondizioni.Text <> String.Empty Then

                        Dim cmbRisposta As DropDownList = DirectCast(dlsQuestionarioSezioneItem.FindControl("cmbRisposta"), DropDownList)

                        Dim condizioni As String() = BizBilancioProgrammato.SplittaCondizioni(txtCondizioni.Text)

                        Dim inCondizione As Boolean = False
                        Dim altriCodiciOsservazioniCollegate As New ArrayList()

                        For Each condizione As String In condizioni

                            Dim codiceRisposta As String = String.Empty
                            Dim codiceOsservazioneCollegata As String = String.Empty
                            Dim disabilitata As Boolean
                            Dim collegata As Boolean

                            BizBilancioProgrammato.SplittaCondizione(condizione, codiceRisposta, codiceOsservazioneCollegata, disabilitata, collegata)

                            If cmbRisposta.SelectedValue = codiceRisposta AndAlso collegata Then

                                If codiceOsservazioneCollegata = codiceOsservazione Then
                                    inCondizione = True
                                Else
                                    altriCodiciOsservazioniCollegate.Add(codiceOsservazioneCollegata)
                                End If

                            End If

                        Next

                        If inCondizione Then

                            For Each altroCodiceOsservazioneCollegata As String In altriCodiciOsservazioniCollegate

                                If Not IfRispostaEmpty(altroCodiceOsservazioneCollegata) Then

                                    collegataNotEmpty = True
                                    Exit For

                                End If

                            Next

                        End If
                    End If
                End If

                If collegataNotEmpty Then
                    Exit For
                End If

            Next

            If collegataNotEmpty Then
                Exit For
            End If

        Next

        Return collegataNotEmpty

    End Function

#End Region

End Class
