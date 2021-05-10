Public Class InfoAssociazioni
    Inherits Common.PageBase

#Region " Constants "

    Private Const ASS As String = "ASS"
    Private Const LAYOUT_KEY_SALVATAGGIO_NON_EFFETTUATO As String = "NO_SAVE"

#End Region

#Region " Types "

    Private Enum LayoutStatus
        View = 0
        Edit = 1
    End Enum

    <Serializable()>
    Private Class AssociazioneCorrente
        Public Codice As String
        Public Descrizione As String
    End Class

#End Region

#Region " Properties "

    Private ReadOnly Property DatiAssociazioneCorrente() As AssociazioneCorrente
        Get
            If ViewState(ASS) Is Nothing Then

                Dim associazione As New AssociazioneCorrente()

                Dim value As String = Request.QueryString.Get(ASS)
                If Not String.IsNullOrWhiteSpace(value) Then

                    Dim values As String() = value.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)

                    associazione.Codice = HttpUtility.UrlDecode(values(0).Trim())
                    associazione.Descrizione = HttpUtility.UrlDecode(values(1).Trim())
                End If

                ViewState(ASS) = associazione
            End If

            Return DirectCast(ViewState(ASS), AssociazioneCorrente)
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then

            Me.LayoutTitolo.Text = String.Format("&nbsp;Informazioni sulla associazione: {0}", Me.DatiAssociazioneCorrente.Descrizione)

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

        Dim associazioneInfo As Entities.AssociazioneInfo = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAssociazioni As New Biz.BizAssociazioni(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                associazioneInfo = bizAssociazioni.GetAssociazioneInfo(Me.DatiAssociazioneCorrente.Codice)

            End Using
        End Using

        If String.IsNullOrWhiteSpace(associazioneInfo.Titolo) Then
            Me.txtTitolo.Text = Me.DatiAssociazioneCorrente.Descrizione
        Else
            Me.txtTitolo.Text = associazioneInfo.Titolo
        End If

        If String.IsNullOrWhiteSpace(associazioneInfo.Descrizione) Then
            Me.txtDescrizione.Text = Me.Settings.INFO_ASS_TEMPLATE_DESCRIZIONE
        Else
            Me.txtDescrizione.Text = associazioneInfo.Descrizione
        End If

        If associazioneInfo.Id.HasValue Then
            Me.hidId.Value = associazioneInfo.Id.ToString()
        Else
            Me.hidId.Value = String.Empty
        End If

    End Sub

    Private Function Salva() As Boolean

        Dim result As Biz.BizAssociazioni.SaveAssociazioneInfoResult = Nothing

        Dim id As Integer? = Nothing
        If Not String.IsNullOrWhiteSpace(Me.hidId.Value) Then id = Convert.ToInt32(Me.hidId.Value)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAssociazioni As New Biz.BizAssociazioni(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                result = bizAssociazioni.SaveAssociazioneInfo(id, Me.DatiAssociazioneCorrente.Codice, Me.txtTitolo.Text, Me.txtDescrizione.Text)

            End Using
        End Using

        If Not result.Success Then
            Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(result.Message, LAYOUT_KEY_SALVATAGGIO_NON_EFFETTUATO, False, False))
        End If

        Return result.Success

    End Function

#End Region

End Class