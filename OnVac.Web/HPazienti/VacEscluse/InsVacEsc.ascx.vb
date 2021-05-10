Imports System.Collections.Generic
Imports Onit.Controls
Imports Onit.Web.UI.WebControls.Validators

Partial Class InsVacEsc
    Inherits Common.UserControlFinestraModalePageBase

#Region " Types "

    <Serializable>
    Public Class ResultItem
        Public VaccinazioneCodice As String
        Public VaccinazioneDescrizione As String
        Public DataVisita As DateTime
        Public MotivoCodice As String
        Public MotivoDescrizione As String
        Public MedicoCodice As String
        Public MedicoNome As String
        Public DataScadenza As DateTime
        Public NumeroDose As Int16
        Public Note As String
    End Class

    Private Class ConfermaResult

        Private _success As Boolean
        Public Property Success() As Boolean
            Get
                Return _success
            End Get
            Private Set(value As Boolean)
                _success = value
            End Set
        End Property

        Private _resultList As List(Of ResultItem)
        Public Property ResultList() As List(Of ResultItem)
            Get
                Return _resultList
            End Get
            Private Set(value As List(Of ResultItem))
                _resultList = value
            End Set
        End Property

        Private _errorMessage As String
        Public Property ErrorMessage() As String
            Get
                Return _errorMessage
            End Get
            Set(value As String)
                _errorMessage = value
            End Set
        End Property

        Public Sub New(errorMessage As String)
            Me.Success = False
            Me.ResultList = Nothing
            Me.ErrorMessage = errorMessage
        End Sub

        Public Sub New(resultList As List(Of ResultItem))
            Me.Success = True
            Me.ResultList = resultList
            Me.ErrorMessage = String.Empty
        End Sub

    End Class

#End Region

#Region " Events "

    Public Event Conferma(lst As List(Of ResultItem))
    Public Event RiabilitaLayout()

    'scatena l'evento per riabilitare il layout
    Protected Sub OnRiabilitaLayout()
        RaiseEvent RiabilitaLayout()
    End Sub

    'invio codici vaccinazioni selezionate alla pagina principale
    Protected Sub OnConferma(resultList As List(Of ResultItem))
        RaiseEvent Conferma(resultList)
    End Sub

#End Region

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(sender As System.Object, e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

#Region " Overrides "

    <Obsolete("NON USARE, sostituire con il metodo Inizializza(dtVaccinazioniEscluse As DataTable)")>
    Public Overrides Sub LoadModale()

        Throw New NotImplementedException("InsVacEsc.LoadModale è obsoleto. Sostituire con il metodo InsVacEsc.Inizializza")

    End Sub

#End Region

#Region " Public "

    Public Sub Inizializza(dtVaccinazioniEscluse As DataTable)

        ' Caricamento vaccinazioni da proporre per l'esclusione
        Dim vaccinazioniDaEscludere As List(Of Biz.BizVaccinazioniEscluse.VaccinazioneDaEscludere)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizEscluse As New Biz.BizVaccinazioniEscluse(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                vaccinazioniDaEscludere = bizEscluse.GetVaccinazioniDaEscludere(dtVaccinazioniEscluse)

            End Using
        End Using

        Me.dtlVacEsc.DataSource = vaccinazioniDaEscludere
        Me.dtlVacEsc.DataBind()

    End Sub

#End Region

#Region " Controls Events "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            ' N.B. : in realtà, questo non viene richiamato mai perchè la modale viene chiusa lato client
            Case "btn_Annulla"

                OnRiabilitaLayout()

            Case "btn_Conferma"

                Dim confermaResult As ConfermaResult = ConfermaVaccinazioni()

                If confermaResult.Success Then
                    OnConferma(confermaResult.ResultList)
                Else
                    Me.RegisterStartupScriptCustom("InsVacEsc_CheckDati", confermaResult.ErrorMessage)
                End If

        End Select

    End Sub

    Protected Sub fmMotivo_Change(sender As Object, e As Controls.OnitModalList.ModalListaEventArgument)

        Dim currentItem As DataListItem = GetCurrentDataListItem(sender, "fmMotivo")
        If Not currentItem Is Nothing Then

            Dim fmMotivo As OnitModalList = DirectCast(sender, OnitModalList)
            fmMotivo.ToolTip = fmMotivo.Descrizione

            Dim dpkDataScadenza As OnitDatePick = DirectCast(currentItem.FindControl("dpkDataScadenza"), OnitDatePick)
            Dim dpkDataVisita As OnitDatePick = DirectCast(currentItem.FindControl("dpkDataVisita"), OnitDatePick)

            Dim codiceVaccinazioneCorrente As String = DirectCast(currentItem.FindControl("lblVacCodice"), Label).Text

            If dpkDataScadenza.Data = Date.MinValue Then

                Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                    Using biz As New Biz.BizMotiviEsclusione(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                        dpkDataScadenza.Data = biz.GetScadenzaMotivoEsclusione(OnVacUtility.Variabili.PazId, fmMotivo.Codice, codiceVaccinazioneCorrente, dpkDataVisita.Data)

                    End Using
                End Using

            End If

        End If

    End Sub

    Protected Sub btnCopia_Click(sender As Object, e As EventArgs)

        Dim currentItem As DataListItem = GetCurrentDataListItem(sender, "btnCopia")
        If Not currentItem Is Nothing Then

            Dim masterfmMotivo As OnitModalList = DirectCast(currentItem.FindControl("fmMotivo"), OnitModalList)
            Dim masterfm_medico As OnitModalList = DirectCast(currentItem.FindControl("fm_medico"), OnitModalList)
            Dim masterdpkDataVisita As OnitDatePick = DirectCast(currentItem.FindControl("dpkDataVisita"), OnitDatePick)
            Dim masterdpkDataScadenza As OnitDatePick = DirectCast(currentItem.FindControl("dpkDataScadenza"), OnitDatePick)

            ' campi non copiati
            'Dim mastertxtDose As TextBox = DirectCast(currentItem.FindControl("txtDose"), TextBox)
            'Dim mastertxtNote As TextBox = DirectCast(currentItem.FindControl("txtNote"), TextBox)

            For i As Integer = 0 To Me.dtlVacEsc.Items.Count - 1

                If currentItem.ItemIndex <> i AndAlso DirectCast(Me.dtlVacEsc.Items(i).FindControl("chkVaccinazione"), CheckBox).Checked Then

                    Dim fmMotivo As OnitModalList = DirectCast(dtlVacEsc.Items(i).FindControl("fmMotivo"), OnitModalList)
                    Dim fm_medico As OnitModalList = DirectCast(dtlVacEsc.Items(i).FindControl("fm_medico"), OnitModalList)
                    Dim dpkDataScadenza As OnitDatePick = DirectCast(dtlVacEsc.Items(i).FindControl("dpkDataScadenza"), OnitDatePick)
                    Dim dpkDataVisita As OnitDatePick = DirectCast(dtlVacEsc.Items(i).FindControl("dpkDataVisita"), OnitDatePick)

                    fmMotivo.Codice = masterfmMotivo.Codice
                    fmMotivo.Descrizione = masterfmMotivo.Descrizione
                    fmMotivo.ToolTip = masterfmMotivo.ToolTip

                    fm_medico.Codice = masterfm_medico.Codice
                    fm_medico.Descrizione = masterfm_medico.Descrizione

                    dpkDataVisita.Data = masterdpkDataVisita.Data
                    dpkDataScadenza.Data = masterdpkDataScadenza.Data

                End If

            Next

        End If

    End Sub

    Private Sub dtlVacEsc_ItemDataBound(sender As Object, e As DataListItemEventArgs) Handles dtlVacEsc.ItemDataBound

        Select Case e.Item.ItemType

            Case ListItemType.Item, ListItemType.EditItem, ListItemType.AlternatingItem

                Dim dpkDataVisita As OnitDatePick = DirectCast(e.Item.FindControl("dpkDataVisita"), OnitDatePick)
                If dpkDataVisita.Data = DateTime.MinValue Then
                    dpkDataVisita.Data = DateTime.Now
                End If

                Dim txtDose As TextBox = DirectCast(e.Item.FindControl("txtDose"), TextBox)
                Dim lblVacCodice As Label = DirectCast(e.Item.FindControl("lblVacCodice"), Label)
                Dim doseDefault As Integer = 0

                Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                    Using biz As New Biz.BizVaccinazioniEscluse(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                        doseDefault = biz.GetMaxDoseEseguitaVaccinazioneEsclusa(OnVacUtility.Variabili.PazId, lblVacCodice.Text)

                    End Using
                End Using

                txtDose.Text = doseDefault.ToString()

        End Select

    End Sub

#End Region

#Region " Private "

    Private Function ConfermaVaccinazioni() As ConfermaResult

        Dim resultList As New List(Of ResultItem)()

        For Each dataListItem As DataListItem In Me.dtlVacEsc.Items

            If DirectCast(dataListItem.FindControl("chkVaccinazione"), CheckBox).Checked Then

                Dim codiceVaccinazione As String = DirectCast(dataListItem.FindControl("lblVacCodice"), Label).Text

                Dim fmMotivo As OnitModalList = DirectCast(dataListItem.FindControl("fmMotivo"), OnitModalList)
                Dim fmMedico As OnitModalList = DirectCast(dataListItem.FindControl("fm_medico"), OnitModalList)

                Dim dpkDataScadenza As OnitDatePick = DirectCast(dataListItem.FindControl("dpkDataScadenza"), OnitDatePick)
                Dim dpkDataVisita As OnitDatePick = DirectCast(dataListItem.FindControl("dpkDataVisita"), OnitDatePick)

                Dim txtDose As TextBox = DirectCast(dataListItem.FindControl("txtDose"), TextBox)

                ' Controlli sulla dose
                txtDose.Text = txtDose.Text.Trim()

                Dim checkDoseResult As Biz.BizVaccinazioniEscluse.CheckValoreDoseResult = Biz.BizVaccinazioniEscluse.CheckValoreDose(txtDose.Text, True)

                If Not checkDoseResult.Success Then
                    Return New ConfermaResult(String.Format("alert('{0} per {1}.');", HttpUtility.JavaScriptStringEncode(checkDoseResult.Message), codiceVaccinazione))
                End If

                ' Controllo sulla valorizzazione della data visita
                If dpkDataVisita.Data = Date.MinValue Then
                    Return New ConfermaResult(String.Format("alert('La data della visita per {0} non può essere vuota.');", codiceVaccinazione))
                End If

                ' Controllo sulla data visita
                If dpkDataVisita.Data > Date.Now Then
                    Return New ConfermaResult(String.Format("alert('La data della visita per {0} non può essere futura.');", codiceVaccinazione))
                End If

                ' Controllo sulla valorizzazione del motivo di esclusione
                If String.IsNullOrEmpty(fmMotivo.Codice) Then
                    Return New ConfermaResult(String.Format("alert('Il motivo di esclusione per {0} è un dato obligatorio.');", codiceVaccinazione))
                End If

                ' Controllo sulla coerenza della data di scadenza
                If dpkDataScadenza.Data > Date.MinValue AndAlso dpkDataScadenza.Data < dpkDataVisita.Data Then
                    Return New ConfermaResult(String.Format("alert('La data di scadenza per {0} non può essere precedente a quella della visita.');", codiceVaccinazione))
                End If

                ' Aggiunta del risultato alla lista da restituire
                Dim result As New ResultItem()
                result.VaccinazioneCodice = codiceVaccinazione
                result.VaccinazioneDescrizione = DirectCast(dataListItem.FindControl("lblVacDescrizione"), Label).Text
                result.MotivoCodice = fmMotivo.Codice
                result.MotivoDescrizione = fmMotivo.Descrizione
                result.MedicoCodice = fmMedico.Codice
                result.MedicoNome = fmMedico.Descrizione
                result.DataVisita = dpkDataVisita.Data
                result.DataScadenza = dpkDataScadenza.Data
                result.NumeroDose = Integer.Parse(txtDose.Text.Trim())
                result.Note = DirectCast(dataListItem.FindControl("txtNote"), TextBox).Text.Trim()

                resultList.Add(result)

            End If

        Next

        If resultList.Count = 0 Then
            Return New ConfermaResult("alert('Selezionare almeno una vaccinazione per confermare!');")
        End If

        Return New ConfermaResult(resultList)

    End Function

    Private Function GetCurrentDataListItem(sender As Object, controlId As String) As DataListItem

        For Each item As DataListItem In Me.dtlVacEsc.Items

            If item.FindControl(controlId) Is sender Then
                Return item
            End If

        Next

        Return Nothing

    End Function

#End Region

End Class
