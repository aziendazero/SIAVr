Imports System.Collections
Imports System.Collections.Generic

Public Class RinnovaEsc
    Inherits Common.UserControlFinestraModalePageBase

#Region " Events "

    <Serializable>
    Public Class ResultRinnovaEsclusioneItem
        Public VaccinazioneCodice As String
        'Public Dose As Integer
        Public NuovaDataVisita As DateTime?
        Public NuovaDataScadenza As DateTime?
    End Class

    Private Class ConfermaRinnovoEsclusioniResult

        Private _success As Boolean
        Public Property Success() As Boolean
            Get
                Return _success
            End Get
            Private Set(value As Boolean)
                _success = value
            End Set
        End Property

        Private _resultList As List(Of ResultRinnovaEsclusioneItem)
        Public Property ResultList() As List(Of ResultRinnovaEsclusioneItem)
            Get
                Return _resultList
            End Get
            Private Set(value As List(Of ResultRinnovaEsclusioneItem))
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

        Public Sub New(resultList As List(Of ResultRinnovaEsclusioneItem))
            Me.Success = True
            Me.ResultList = resultList
            Me.ErrorMessage = String.Empty
        End Sub

    End Class

    Public Event Conferma(listRinnova As List(Of ResultRinnovaEsclusioneItem))
    Public Event RiabilitaLayout()

    'scatena l'evento per riabilitare il layout
    Protected Sub OnRiabilitaLayout()
        RaiseEvent RiabilitaLayout()
    End Sub

    Protected Sub OnConferma(listRinnovi As List(Of ResultRinnovaEsclusioneItem))
        RaiseEvent Conferma(listRinnovi)
    End Sub

#End Region

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

#Region " Controls Events "

    Private Sub ToolBar_RinnovaEsc_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar_RinnovaEsc.ButtonClicked

        Select Case e.Button.Key

            ' N.B. : in realtà, questo non viene richiamato mai perchè la modale viene chiusa lato client
            Case "btn_Annulla"

                OnRiabilitaLayout()

            Case "btn_Salva"

                Dim rinnovoEsclusioni As ConfermaRinnovoEsclusioniResult = ConfermaRinnovo()

                OnConferma(rinnovoEsclusioni.ResultList)

        End Select

    End Sub

    Private CountNonRinnovabili As Integer

    Public Sub Inizializza(dv As DataView)

        CountNonRinnovabili = 0

        dg_vacExRinnovate.DataSource = dv
        dg_vacExRinnovate.DataBind()

    End Sub

    Private Sub dg_vacExRinnovate_ItemDataBound(sender As Object, e As DataGridItemEventArgs) Handles dg_vacExRinnovate.ItemDataBound

        If e.Item.ItemIndex <> -1 Then

            Dim lblNuovaDataVisita As Label = DirectCast(e.Item.FindControl("lblNuovaDataVisita"), Label)
            Dim lblNuovaDataScadenza As Label = DirectCast(e.Item.FindControl("lblNuovaDataScadenza"), Label)
            Dim scaduta As Label = DirectCast(e.Item.FindControl("Scaduta"), Label)

            If String.IsNullOrWhiteSpace(scaduta.Text) OrElse Convert.ToBoolean(scaduta.Text) = False Then

                lblNuovaDataVisita.Text = String.Empty
                lblNuovaDataScadenza.CssClass = "non-rinnovabile"
                lblNuovaDataScadenza.Text = "NON RINNOVABILE: l'esclusione non è scaduta"
                CountNonRinnovabili += 1

            ElseIf Convert.ToDateTime(lblNuovaDataScadenza.Text) = Date.MinValue Then

                lblNuovaDataVisita.Text = String.Empty
                lblNuovaDataScadenza.CssClass = "non-rinnovabile"
                lblNuovaDataScadenza.Text = "NON RINNOVABILE: data scadenza non calcolabile per configurazione"
                CountNonRinnovabili += 1

            Else

                'Dim txtDose As TextBox = DirectCast(e.Item.FindControl("txtDose"), TextBox)

                lblNuovaDataScadenza.Text = Convert.ToDateTime(lblNuovaDataScadenza.Text).ToShortDateString()
                'txtDose.ReadOnly = False
                'txtDose.CssClass = "TextBox_Stringa_Obbligatorio"

                Dim hdVacCodice As HiddenField = DirectCast(e.Item.FindControl("hdCodVaccinazione"), HiddenField)
                'Dim doseDefault As Integer = 0

                'Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                '    Using biz As New Biz.BizVaccinazioniEscluse(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                '        doseDefault = biz.GetMaxDoseEseguitaVaccinazioneEsclusa(OnVacUtility.Variabili.PazId, hdVacCodice.Value)

                '    End Using
                'End Using

                'txtDose.Text = doseDefault.ToString()
            End If
        End If

    End Sub

    Private Function ConfermaRinnovo() As ConfermaRinnovoEsclusioniResult

        Dim nuovaDataVisita As New DateTime?
        Dim nuovaDataScadenza As New DateTime

        'Dim result As New ResultRinnovaEsclusioneItem
        Dim resultList As New List(Of ResultRinnovaEsclusioneItem)()

        For i As Int16 = 0 To dg_vacExRinnovate.Items.Count - 1

            Dim codVaccinazione As String = DirectCast(dg_vacExRinnovate.Items(i).FindControl("hdCodVaccinazione"), HiddenField).Value

            If Not String.IsNullOrWhiteSpace(codVaccinazione) Then
                Dim result As New ResultRinnovaEsclusioneItem
                result.VaccinazioneCodice = codVaccinazione

                'result.Dose = cod

                If IsDate(DirectCast(dg_vacExRinnovate.Items(i).FindControl("lblNuovaDataScadenza"), Label).Text) Then
                    nuovaDataScadenza = Convert.ToDateTime(DirectCast(dg_vacExRinnovate.Items(i).FindControl("lblNuovaDataScadenza"), Label).Text)
                    If Not nuovaDataScadenza = Date.MinValue Then
                        result.NuovaDataScadenza = nuovaDataScadenza.Date
                    Else
                        result.NuovaDataScadenza = Nothing
                    End If
                Else
                    result.NuovaDataScadenza = Nothing
                End If

                If IsDate(DirectCast(dg_vacExRinnovate.Items(i).FindControl("lblNuovaDataVisita"), Label).Text) Then
                    nuovaDataVisita = Convert.ToDateTime(DirectCast(dg_vacExRinnovate.Items(i).FindControl("lblNuovaDataVisita"), Label).Text)
                    If Not nuovaDataVisita = Date.MinValue Then
                        result.NuovaDataVisita = nuovaDataVisita.Value
                    Else
                        result.NuovaDataVisita = Nothing
                    End If
                Else
                    result.NuovaDataVisita = Nothing
                End If

                resultList.Add(result)

            End If

        Next

        Return New ConfermaRinnovoEsclusioniResult(resultList)

    End Function

#End Region

#Region " Metodi per datagrid "

    Protected Function ConvertToDateString(data As Object, includeHours As Boolean) As String

        If data Is DBNull.Value OrElse data Is Nothing Then
            Return String.Empty
        End If

        If includeHours Then
            Return Convert.ToDateTime(data).ToString("dd/MM/yyyy HH:mm")
        End If

        Return Convert.ToDateTime(data).ToString("dd/MM/yyyy")

    End Function

    Private Sub RinnovaEsc_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

        ToolBar_RinnovaEsc.Items.FromKeyButton("btn_Salva").Enabled = dg_vacExRinnovate.Items.Count <> CountNonRinnovabili

    End Sub

#End Region

End Class