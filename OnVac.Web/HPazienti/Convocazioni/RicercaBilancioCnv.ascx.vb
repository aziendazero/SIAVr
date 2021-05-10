Imports Onit.Database.DataAccessManager
Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.Entities


Partial Class RicercaBilancioCnv
    Inherits Common.UserControlFinestraModalePageBase

    Protected WithEvents OnitLayout31 As Onit.Controls.PagesLayout.OnitLayout3

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo Ë richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

    Event ReturnBilancio(bil_numero As Integer, mal_codice As String)
    Event AnnullaBilancio()

#Region " Properties "

    Private _datasource As List(Of BilancioProgrammato)
    Public Property DataSource() As List(Of BilancioProgrammato)
        Get
            Return _datasource
        End Get
        Set(ByVal value As List(Of BilancioProgrammato))
            _datasource = value
            Me.LoadDataSource(value)
        End Set
    End Property

#End Region

    Public Sub LoadDataSource(lst As List(Of BilancioProgrammato))

        Response.Expires = -1

        grvBilanci.SelectedIndex = -1
        grvBilanci.DataSource = lst
        grvBilanci.DataBind()

        If grvBilanci.Rows.Count > 0 Then
            grvBilanci.SelectedIndex = 0
        Else
            grvBilanci.SelectedIndex = -1
        End If

    End Sub

    Public Function GetStringEta(ByVal giorniTotale As Integer) As String
        If giorniTotale > 0 Then
            Dim _eta As New Entities.Eta(giorniTotale)
            Return _eta.Anni & " anni " & _eta.Mesi & " mesi " & _eta.Giorni & " giorni"
        Else
            Return "-"
        End If
    End Function

    Private Sub grvBilanci_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grvBilanci.RowCommand
        If e.CommandName = "Seleziona" Then
            '  grvBilanci.SelectedIndex = Convert.ToInt32(e.CommandArgument)
        End If
    End Sub

    Private Sub grvBilanci_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grvBilanci.RowDataBound
        If e.Row.RowIndex <> -1 Then
            Dim b As BilancioProgrammato = e.Row.DataItem
            DirectCast(e.Row.FindControl("lblEt‡Minima"), Label).Text = GetStringEta(b.Eta_Minima)
            DirectCast(e.Row.FindControl("lblEt‡Massima"), Label).Text = GetStringEta(b.Eta_Massima)
            DirectCast(e.Row.FindControl("imgObbligatorio"), System.Web.UI.WebControls.Image).Visible = b.BIL_OBBLIGATORIO
        End If
    End Sub

    Private Sub ToolBar_ButtonClicked(ByVal sender As Object, ByVal e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked
        Select Case e.Button.Key
            Case "btnConferma"
                If Not grvBilanci.SelectedRow Is Nothing Then

                    Dim mal_codice As String = grvBilanci.SelectedDataKey.Values("MAL_CODICE")
                    Dim bil_numero As Integer = grvBilanci.SelectedDataKey.Values("N_BILANCIO")
                    RaiseEvent ReturnBilancio(bil_numero, mal_codice)

                Else
                    RaiseEvent AnnullaBilancio()
                End If
            Case "btnAnnulla"
                RaiseEvent AnnullaBilancio()
        End Select
    End Sub

End Class
