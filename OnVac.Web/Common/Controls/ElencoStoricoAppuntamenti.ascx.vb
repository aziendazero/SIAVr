Imports System.Collections.Generic


Public Class ElencoStoricoAppuntamenti
    Inherits Common.UserControlPageBase

#Region " Types "

    Private Enum DgrColumnIndex
        Id = 0
        DataAppuntamento = 1
        Vaccinazioni = 2
        Ambulatorio = 3
        DataEliminazioneAppuntamento = 4
        MotivoEliminazioneAppuntamento = 5
        NoteAvvisi = 6
        DataConvocazione = 7
        IconaNote = 8
        CodiceUtenteRegistrazioneAppuntamento = 9
        DataRegistrazioneAppuntamento = 10
        DataInvio = 11
        CodiceUtenteEliminazioneAppuntamento = 12
        Note = 13
        NoteModificaAppuntamento = 14

    End Enum

    Public Enum TipoVisualizzazione
        Completa = 0
        Ridotta = 1
    End Enum

#End Region

#Region " Properties "

    Public Property Visualizzazione As TipoVisualizzazione
        Get
            If ViewState("Vis") Is Nothing Then ViewState("Vis") = TipoVisualizzazione.Completa
            Return ViewState("Vis")
        End Get
        Set(value As TipoVisualizzazione)
            ViewState("Vis") = value
        End Set
    End Property

    Private Property CampoOrdinamento As String
        Get
            If ViewState("CampoOrd") Is Nothing Then ViewState("CampoOrd") = String.Empty
            Return ViewState("CampoOrd")
        End Get
        Set(value As String)
            ViewState("CampoOrd") = value
        End Set
    End Property

    Private Property VersoOrdinamento As String
        Get
            If ViewState("VersoOrd") Is Nothing Then ViewState("VersoOrd") = String.Empty
            Return ViewState("VersoOrd")
        End Get
        Set(value As String)
            ViewState("VersoOrd") = value
        End Set
    End Property

    Private Property CodicePaziente As Long
        Get
            If ViewState("CodPaz") Is Nothing Then ViewState("CodPaz") = 0
            Return ViewState("CodPaz")
        End Get
        Set(value As Long)
            ViewState("CodPaz") = value
        End Set
    End Property

    Private Property DataConvocazione As DateTime?
        Get
            Return ViewState("Cnv")
        End Get
        Set(value As DateTime?)
            ViewState("Cnv") = value
        End Set
    End Property

#End Region

#Region " Page Events "

    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

        SetImmagineOrdinamento()

    End Sub

#End Region

#Region " Public "

    ''' <summary>
    ''' Caricamento storico appuntamenti della convocazione specificata (o di tutte le convocazioni del paziente, se dataConvocazione nulla)
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataConvocazione"></param>
    ''' <remarks></remarks>
    Public Sub LoadStoricoAppuntamenti(codicePaziente As Long, dataConvocazione As DateTime?)

        Me.CodicePaziente = codicePaziente
        Me.DataConvocazione = dataConvocazione

        If Me.Visualizzazione = TipoVisualizzazione.Completa Then
            Me.CampoOrdinamento = Me.dgrStoricoAppuntamenti.Columns(DgrColumnIndex.DataConvocazione).SortExpression
        Else
            Me.CampoOrdinamento = Me.dgrStoricoAppuntamenti.Columns(DgrColumnIndex.Id).SortExpression
        End If

        Me.VersoOrdinamento = Constants.VersoOrdinamento.Decrescente

        LoadStoricoAppuntamenti(0)

    End Sub

#End Region

#Region " Datagrid Events "

    Private Sub dgrStoricoAppuntamenti_ItemCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgrStoricoAppuntamenti.ItemCommand

        Select Case e.CommandName

            Case "Info"

                Dim currentGridItem As DataGridItem = DirectCast(e.Item, DataGridItem)
                If Not currentGridItem Is Nothing Then
                    ShowInfo(currentGridItem.Cells(DgrColumnIndex.DataRegistrazioneAppuntamento).Text, currentGridItem.Cells(DgrColumnIndex.CodiceUtenteRegistrazioneAppuntamento).Text, currentGridItem.Cells(DgrColumnIndex.DataInvio).Text, currentGridItem.Cells(DgrColumnIndex.CodiceUtenteEliminazioneAppuntamento).Text, currentGridItem.Cells(DgrColumnIndex.Note).Text)
                End If

        End Select

    End Sub

    Private Sub dgrStoricoAppuntamenti_SortCommand(source As Object, e As System.Web.UI.WebControls.DataGridSortCommandEventArgs) Handles dgrStoricoAppuntamenti.SortCommand

        If e.SortExpression = Me.CampoOrdinamento Then
            If Me.VersoOrdinamento = Constants.VersoOrdinamento.Crescente Then
                Me.VersoOrdinamento = Constants.VersoOrdinamento.Decrescente
            Else
                Me.VersoOrdinamento = Constants.VersoOrdinamento.Crescente
            End If
        Else
            Me.CampoOrdinamento = e.SortExpression
            Me.VersoOrdinamento = Constants.VersoOrdinamento.Crescente
        End If

        LoadStoricoAppuntamenti(Me.dgrStoricoAppuntamenti.CurrentPageIndex)

    End Sub

    Private Sub dgrStoricoAppuntamenti_PageIndexChanged(source As Object, e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dgrStoricoAppuntamenti.PageIndexChanged

        LoadStoricoAppuntamenti(e.NewPageIndex)

    End Sub

#End Region

#Region " Private "

    Private Sub SetImmagineOrdinamento()

        Dim id As String = String.Empty

        Select Case Me.CampoOrdinamento
            Case Me.dgrStoricoAppuntamenti.Columns(DgrColumnIndex.Id).SortExpression
                id = "imgId"
            Case Me.dgrStoricoAppuntamenti.Columns(DgrColumnIndex.DataAppuntamento).SortExpression
                id = "imgApp"
            Case Me.dgrStoricoAppuntamenti.Columns(DgrColumnIndex.Vaccinazioni).SortExpression
                id = "imgVac"
            Case Me.dgrStoricoAppuntamenti.Columns(DgrColumnIndex.Ambulatorio).SortExpression
                id = "imgAmb"
            Case Me.dgrStoricoAppuntamenti.Columns(DgrColumnIndex.DataEliminazioneAppuntamento).SortExpression
                id = "imgDel"
            Case Me.dgrStoricoAppuntamenti.Columns(DgrColumnIndex.MotivoEliminazioneAppuntamento).SortExpression
                id = "imgMot"
            Case Me.dgrStoricoAppuntamenti.Columns(DgrColumnIndex.NoteAvvisi).SortExpression
                id = "imgNotAvv"
            Case Me.dgrStoricoAppuntamenti.Columns(DgrColumnIndex.Note).SortExpression
                id = "imgNot"
            Case Me.dgrStoricoAppuntamenti.Columns(DgrColumnIndex.DataConvocazione).SortExpression
                id = "imgCnv"
        End Select

        Dim imageUrl As String = String.Empty

        If Me.VersoOrdinamento = Constants.VersoOrdinamento.Crescente Then
            imageUrl = Me.ResolveClientUrl("~/Images/arrow_up_small.gif")
        Else
            imageUrl = Me.ResolveClientUrl("~/Images/arrow_down_small.gif")
        End If

        Page.ClientScript.RegisterStartupScript(Me.GetType(), "ord_img", String.Format("<script type='text/javascript'>ImpostaImmagineOrdinamento('{0}', '{1}');</script>", id, imageUrl))

    End Sub

    Private Sub LoadStoricoAppuntamenti(currentPageIndex As Integer)

        Dim result As Biz.BizConvocazione.GetStoricoAppuntamentiResult = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizConvocazione As New Biz.BizConvocazione(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Dim command As New Biz.BizConvocazione.GetStoricoAppuntamentiCommand()
                command.CodicePaziente = Me.CodicePaziente
                command.DataConvocazione = Me.DataConvocazione
                command.PageIndex = currentPageIndex
                command.PageSize = Me.dgrStoricoAppuntamenti.PageSize
                command.CampoOrdinamento = Me.CampoOrdinamento
                command.VersoOrdinamento = Me.VersoOrdinamento

                result = bizConvocazione.GetStoricoAppuntamenti(command)

            End Using
        End Using

        If result.CountStoricoAppuntamenti = 0 Then

            dgrStoricoAppuntamenti.Visible = False

            lblMessage.Visible = True
            lblMessage.Text = "Nessuno storico appuntamenti relativo al paziente."

        Else

            lblMessage.Visible = False

            dgrStoricoAppuntamenti.Visible = True
            dgrStoricoAppuntamenti.VirtualItemCount = result.CountStoricoAppuntamenti
            dgrStoricoAppuntamenti.CurrentPageIndex = currentPageIndex

            Dim showColumn As Boolean = (Me.Visualizzazione = TipoVisualizzazione.Completa)
            'dgrStoricoAppuntamenti.Columns(DgrColumnIndex.Id).Visible = showColumn
            dgrStoricoAppuntamenti.Columns(DgrColumnIndex.DataConvocazione).Visible = showColumn
            'dgrStoricoAppuntamenti.Columns(DgrColumnIndex.DurataAppuntamento).Visible = showColumn
            'dgrStoricoAppuntamenti.Columns(DgrColumnIndex.DataRegistrazioneAppuntamento).Visible = showColumn
            'dgrStoricoAppuntamenti.Columns(DgrColumnIndex.UtenteRegistrazioneAppuntamento).Visible = showColumn
            'dgrStoricoAppuntamenti.Columns(DgrColumnIndex.TipoAppuntamento).Visible = showColumn
            'dgrStoricoAppuntamenti.Columns(DgrColumnIndex.DataInvio).Visible = showColumn

            dgrStoricoAppuntamenti.DataSource = result.ListStoricoAppuntamenti
            dgrStoricoAppuntamenti.DataBind()

        End If

    End Sub

    Private Sub ShowInfo(dataRegistrazioneAppuntamento As String, utenteRegistrazioneAppuntamento As String, dataInvioAppuntamento As String, utenteEliminazioneAppuntamento As String, note As String)

        lblDataRegistrazioneAppuntamentoInfo.Text = dataRegistrazioneAppuntamento
        lblUtenteRegistrazioneAppuntamentoInfo.Text = utenteRegistrazioneAppuntamento
        lblDataInvioAppuntamentoInfo.Text = dataInvioAppuntamento
        lblUtenteEliminazioneAppuntamentoInfo.Text = utenteEliminazioneAppuntamento
        txtNoteInfo.Text = note

        fmInfo.VisibileMD = True

    End Sub

#End Region

#Region " Protected "

    Protected Function ConvertToDateString(data As Object, includeHours As Boolean) As String

        If data Is Nothing Then
            Return String.Empty
        End If

        If includeHours Then
            Return Convert.ToDateTime(data).ToString("dd/MM/yyyy HH:mm")
        End If

        Return Convert.ToDateTime(data).ToString("dd/MM/yyyy")

    End Function

    Protected Function GetToolTipTipo(tipo As Object) As String

        If tipo Is Nothing OrElse tipo Is DBNull.Value Then
            Return String.Empty
        End If

        Select Case tipo.ToString()

            Case Constants.TipoPrenotazioneAppuntamento.Automatica
                Return "Appuntamento automatico"

            Case Constants.TipoPrenotazioneAppuntamento.ManualeDaGestioneAppuntamenti
                Return "Appuntamento manuale da maschera Gestione Appuntamenti"

            Case Constants.TipoPrenotazioneAppuntamento.ManualeDaRicercaAppuntamenti
                Return "Appuntamento manuale da maschera Ricerca Appuntamenti"

        End Select

        Return String.Empty

    End Function

    Protected Function GetCodiceDescConsultorio(codiceConsultorio As Object, descrizioneConsultorio As Object) As String

        Return codiceConsultorio.ToString() + " - " + descrizioneConsultorio.ToString()

    End Function

    Protected Function GetNoteAvvisiAppuntamento(noteAvvisi As Object, noteUtenteModificaAppuntamento As Object) As String

        Dim note As New System.Text.StringBuilder()

        If Not String.IsNullOrWhiteSpace(noteAvvisi.ToString()) Then

            note.AppendFormat("<b>NOTE AVVISO:</b> {0}", noteAvvisi.ToString())

            If Not String.IsNullOrWhiteSpace(noteUtenteModificaAppuntamento.ToString()) Then
                note.Append("<br/>")
            End If

        End If

        If Not String.IsNullOrWhiteSpace(noteUtenteModificaAppuntamento.ToString()) Then

            note.AppendFormat("<b>NOTE APPUNTAMENTO:</b> {0}", noteUtenteModificaAppuntamento.ToString())

        End If

        Return note.ToString()

    End Function

#End Region

End Class