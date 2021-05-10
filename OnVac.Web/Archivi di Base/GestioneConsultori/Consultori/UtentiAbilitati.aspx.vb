Imports System.Collections.Generic

Public Class UtentiAbilitati
    Inherits Common.PageBase

#Region " Types "

    Private Enum DgrUtentiRicercaColumnIndex As Int16
        SelectorColumn = 0
        IdUtente = 1
        Codice = 2
        Descrizione = 3
        Cognome = 4
        Nome = 5
    End Enum

    Private Enum DgrUtentiAssociatiColumnIndex As Int16
        SelectorColumn = 0
        IdUtente = 1
        Codice = 2
        Descrizione = 3
        Cognome = 4
        Nome = 5
    End Enum

#End Region

#Region " Properties "

    Private Property CodiceConsultorio As String
        Get
            Return ViewState("CodiceConsultorio")
        End Get
        Set(value As String)
            ViewState("CodiceConsultorio") = value
        End Set
    End Property

    Private Property ListUtentiAssociati As List(Of Integer)
        Get
            Return ViewState("listUteAssociati")
        End Get
        Set(value As List(Of Integer))
            ViewState("listUteAssociati") = value
        End Set
    End Property

    'Private ReadOnly Property IdUtenteRicercaSelezionato As Integer
    '    Get
    '        If Me.dgrUtentiRicerca.SelectedItem Is Nothing Then
    '            Return -1
    '        End If
    '        Return Convert.ToInt32(HttpUtility.HtmlDecode(Me.dgrUtentiRicerca.SelectedItem.Cells(DgrUtentiRicercaColumnIndex.IdUtente).Text))
    '    End Get
    'End Property

    'Private ReadOnly Property IdUtenteAssociatoSelezionato As Integer
    '    Get
    '        If Me.dgrUtentiAssociati.SelectedItem Is Nothing Then
    '            Return -1
    '        End If
    '        Return Convert.ToInt32(HttpUtility.HtmlDecode(Me.dgrUtentiAssociati.SelectedItem.Cells(DgrUtentiAssociatiColumnIndex.IdUtente).Text))
    '    End Get
    'End Property

#End Region

#Region " Page Events "

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then

            Me.dgrUtentiRicerca.SelectedIndex = -1
            Me.dgrUtentiAssociati.SelectedIndex = -1

            Me.CodiceConsultorio = HttpUtility.UrlDecode(Request.QueryString("CODICE"))

            Dim descrizioneConsultorio As String = String.Empty

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

                descrizioneConsultorio = genericProvider.Consultori.GetCnsDescrizione(Me.CodiceConsultorio)

            End Using

            Me.LayoutTitolo.Text = String.Format("&nbsp;Centro Vaccinale selezionato: {0} [{1}]",
                                                 descrizioneConsultorio,
                                                 Me.CodiceConsultorio)

            Me.LoadUtentiAssociati(0, Nothing)
            Me.LoadUtentiRicerca(0, Nothing)

        End If

    End Sub

#End Region

#Region " Toolbar "

    'Private Sub ToolBar_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

    '    Select Case be.Button.Key

    '        Case "btnAggiungi"

    '            If Me.IdUtenteRicercaSelezionato < 0 Then
    '                ShowMessageBox("Selezionare l'utente da associare.")
    '                Return
    '            End If

    '            InsertUtenteConsultorio()

    '        Case "btnElimina"

    '            If Me.IdUtenteAssociatoSelezionato < 0 Then
    '                ShowMessageBox("Nessun utente selezionato per l'eliminazione.")
    '                Return
    '            End If

    '            DeleteUtenteConsultorio()

    '    End Select

    'End Sub

#End Region

#Region " Datagrid Events "

    Private Sub dgrUtentiRicerca_PageIndexChanged(source As Object, e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dgrUtentiRicerca.PageIndexChanged

        LoadUtentiRicerca(e.NewPageIndex, Me.ucFiltroRicercaUtenti.GetFilter())

    End Sub

    Private Sub dgrUtentiRicerca_SelectedIndexChanged(sender As Object, e As EventArgs) Handles dgrUtentiRicerca.SelectedIndexChanged

        InsertUtenteConsultorio(Convert.ToInt32(HttpUtility.HtmlDecode(Me.dgrUtentiRicerca.SelectedItem.Cells(DgrUtentiRicercaColumnIndex.IdUtente).Text)))

    End Sub

    Private Sub dgrUtentiAssociati_PageIndexChanged(source As Object, e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dgrUtentiAssociati.PageIndexChanged

        LoadUtentiAssociati(e.NewPageIndex, Nothing)

    End Sub

    Private Sub dgrUtentiAssociati_SelectedIndexChanged(sender As Object, e As EventArgs) Handles dgrUtentiAssociati.SelectedIndexChanged

        DeleteUtenteConsultorio(Convert.ToInt32(HttpUtility.HtmlDecode(Me.dgrUtentiAssociati.SelectedItem.Cells(DgrUtentiAssociatiColumnIndex.IdUtente).Text)))

    End Sub

#End Region

#Region " User Control Events "

    Private Sub ucFiltroRicercaUtenti_Cerca(filtro As String) Handles ucFiltroRicercaUtenti.Cerca

        Me.dgrUtentiRicerca.SelectedIndex = -1

        LoadUtentiRicerca(0, filtro)

    End Sub

#End Region

#Region " Private "

    Private Sub LoadUtentiRicerca(currentPageIndex As Integer, filtro As String)

        Dim result As Biz.BizUtenti.GetListUtentiResult = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizUtenti As New Biz.BizUtenti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                Dim command As New Biz.BizUtenti.GetListUtentiCommand()
                command.AppId = OnVacContext.AppId
                command.CodiceAzienda = OnVacContext.Azienda
                command.Filtro = filtro
                command.PageIndex = currentPageIndex
                command.PageSize = Me.dgrUtentiRicerca.PageSize

                result = bizUtenti.GetListUtenti(command)

            End Using
        End Using

        Me.dgrUtentiRicerca.VirtualItemCount = result.TotalCountUtenti
        Me.dgrUtentiRicerca.CurrentPageIndex = result.CurrentPageIndex

        Dim listUtentiRicerca As List(Of Entities.Utente) = Nothing

        If Me.ListUtentiAssociati.IsNullOrEmpty() Then
            listUtentiRicerca = result.ListUtenti
        Else
            listUtentiRicerca = result.ListUtenti.Where(Function(ute) Not Me.ListUtentiAssociati.Contains(ute.Id)).ToList()
        End If

        Me.dgrUtentiRicerca.DataSource = listUtentiRicerca
        Me.dgrUtentiRicerca.DataBind()

        Me.dgrUtentiRicerca.SelectedIndex = -1

    End Sub

    Private Sub LoadUtentiAssociati(currentPageIndex As Integer, filtro As String)

        Dim result As Biz.BizUtenti.GetListUtentiResult = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizUtenti As New Biz.BizUtenti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                Dim command As New Biz.BizUtenti.GetListUtentiCommand()
                command.AppId = OnVacContext.AppId
                command.CodiceAzienda = OnVacContext.Azienda
                command.Filtro = filtro
                command.CodiceConsultorio = Me.CodiceConsultorio
                command.PageIndex = currentPageIndex
                command.PageSize = Me.dgrUtentiAssociati.PageSize

                result = bizUtenti.GetListUtenti(command)

            End Using
        End Using

        Me.dgrUtentiAssociati.VirtualItemCount = result.TotalCountUtenti
        Me.dgrUtentiAssociati.CurrentPageIndex = result.CurrentPageIndex

        Me.dgrUtentiAssociati.DataSource = result.ListUtenti
        Me.dgrUtentiAssociati.DataBind()

        Me.dgrUtentiAssociati.SelectedIndex = -1

        Me.lblUtentiAbilitati.Text = "UTENTI ABILITATI AL CENTRO VACCINALE SELEZIONATO"

        If result.ListUtenti.IsNullOrEmpty() Then
            Me.ListUtentiAssociati = New List(Of Integer)()
        Else
            Me.ListUtentiAssociati = result.ListUtenti.Select(Function(ute) ute.Id).ToList()
        End If

        Me.lblUtentiAbilitati.Text += String.Format(" [{0}]", Me.ListUtentiAssociati.Count)

    End Sub

    Private Sub InsertUtenteConsultorio(idUtente As Integer)

        Dim result As Biz.BizGenericResult = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizUtenti As New Biz.BizUtenti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                result = bizUtenti.InserConsultoriotUtente(idUtente, Me.CodiceConsultorio)

            End Using
        End Using

        If Not result.Success Then
            ShowMessageBox(result.Message)
        Else
            LoadUtentiAssociati(0, Nothing)
            LoadUtentiRicerca(0, Me.ucFiltroRicercaUtenti.GetFilter())
        End If

    End Sub

    Private Sub DeleteUtenteConsultorio(idUtente As Integer)

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizUtenti As New Biz.BizUtenti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                bizUtenti.DeleteConsultorioUtente(idUtente, Me.CodiceConsultorio)

            End Using
        End Using

        LoadUtentiAssociati(0, Nothing)
        LoadUtentiRicerca(0, Me.ucFiltroRicercaUtenti.GetFilter())

    End Sub

    Private Sub ShowMessageBox(message As String)

        Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(message, "MSG", False, False))

    End Sub

#End Region

End Class