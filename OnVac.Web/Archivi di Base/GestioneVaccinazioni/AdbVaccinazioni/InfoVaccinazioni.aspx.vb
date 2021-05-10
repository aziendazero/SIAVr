Public Class InfoVaccinazioni
    Inherits Common.PageBase

#Region " Constants "

    Private Const VAC As String = "VAC"
    Private Const LAYOUT_KEY_SALVATAGGIO_NON_EFFETTUATO As String = "NO_SAVE"

#End Region

#Region " Types "

    Private Enum LayoutStatus
        View = 0
        Edit = 1
    End Enum

    <Serializable()>
    Private Class VaccinazioneCorrente
        Public Codice As String
        Public Descrizione As String
    End Class

#End Region

#Region " Properties "

    Private ReadOnly Property DatiVaccinazioneCorrente() As VaccinazioneCorrente
        Get
            If ViewState(VAC) Is Nothing Then

                Dim vaccinazione As New VaccinazioneCorrente()

                Dim value As String = Request.QueryString.Get(VAC)
                If Not String.IsNullOrWhiteSpace(value) Then

                    Dim values As String() = value.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)

                    vaccinazione.Codice = HttpUtility.UrlDecode(values(0).Trim())
                    vaccinazione.Descrizione = HttpUtility.UrlDecode(values(1).Trim())
                End If

                ViewState(VAC) = vaccinazione
            End If

            Return DirectCast(ViewState(VAC), VaccinazioneCorrente)
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then

            Me.LayoutTitolo.Text = String.Format("&nbsp;Informazioni sulla vaccinazione: {0}", Me.DatiVaccinazioneCorrente.Descrizione)

            SetLayoutStatus(LayoutStatus.View)
            LoadInfo()

        End If

    End Sub

    Private Sub ToolBarInfo_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBarInfo.ButtonClicked

        Select Case be.Button.Key

            Case "btnEdit"
                SetLayoutStatus(LayoutStatus.Edit)

            Case "btnPulisci"
                Me.txtDescrizione.Text = String.Empty
                Me.txtTitolo.Text = String.Empty

            Case "btnSalva"
                If Salva() Then
                    SetLayoutStatus(LayoutStatus.View)
                    LoadInfo()
                End If

            Case "btnAnnulla"
                SetLayoutStatus(LayoutStatus.View)
                LoadInfo()

        End Select

    End Sub

#End Region

#Region " Private "

    Private Sub SetLayoutStatus(status As LayoutStatus)

        Dim isInEdit As Boolean = (status = LayoutStatus.Edit)

        Me.ToolBarInfo.Items.FromKeyButton("btnEdit").Enabled = Not isInEdit
        Me.ToolBarInfo.Items.FromKeyButton("btnPulisci").Enabled = isInEdit
        Me.ToolBarInfo.Items.FromKeyButton("btnSalva").Enabled = isInEdit
        Me.ToolBarInfo.Items.FromKeyButton("btnAnnulla").Enabled = isInEdit

        Me.txtTitolo.Enabled = isInEdit
        Me.txtDescrizione.Enabled = isInEdit

    End Sub

    Private Sub LoadInfo()

        Dim vaccinazioneInfo As Entities.VaccinazioneInfo = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAnagVacc As New Biz.BizAnaVaccinazioni(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                vaccinazioneInfo = bizAnagVacc.GetVaccinazioneInfo(Me.DatiVaccinazioneCorrente.Codice)

            End Using
        End Using

        If String.IsNullOrWhiteSpace(vaccinazioneInfo.Titolo) Then
            Me.txtTitolo.Text = Me.DatiVaccinazioneCorrente.Descrizione
        Else
            Me.txtTitolo.Text = vaccinazioneInfo.Titolo
        End If

        If String.IsNullOrWhiteSpace(vaccinazioneInfo.Descrizione) Then
            Me.txtDescrizione.Text = Me.Settings.INFO_VAC_TEMPLATE_DESCRIZIONE
        Else
            Me.txtDescrizione.Text = vaccinazioneInfo.Descrizione
        End If

        If vaccinazioneInfo.Id.HasValue Then
            Me.hidId.Value = vaccinazioneInfo.Id.ToString()
        Else
            Me.hidId.Value = String.Empty
        End If

    End Sub

    Private Function Salva() As Boolean

        Dim result As Biz.BizAnaVaccinazioni.SaveVaccinazioneInfoResult = Nothing

        Dim id As Integer? = Nothing
        If Not String.IsNullOrWhiteSpace(Me.hidId.Value) Then id = Convert.ToInt32(Me.hidId.Value)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAnagVacc As New Biz.BizAnaVaccinazioni(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                result = bizAnagVacc.SaveVaccinazioneInfo(id, Me.DatiVaccinazioneCorrente.Codice, Me.txtTitolo.Text, Me.txtDescrizione.Text)

            End Using
        End Using

        If Not result.Success Then
            Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(result.Message, LAYOUT_KEY_SALVATAGGIO_NON_EFFETTUATO, False, False))
        End If

        Return result.Success

    End Function

#End Region

End Class