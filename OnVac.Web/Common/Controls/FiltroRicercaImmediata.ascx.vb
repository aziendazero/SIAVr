Public Class FiltroRicercaImmediata
    Inherits Common.UserControlPageBase

#Region " Events "

    Public Event Cerca(filtro As String)

#End Region

#Region " Properties "

    Public Property Enabled As Boolean
        Get
            If ViewState("Enable") Is Nothing Then ViewState("Enable") = False
            Return ViewState("Enable")
        End Get
        Set(value As Boolean)
            ViewState("Enable") = value
            Me.SetLayout(value)
        End Set
    End Property

#End Region

#Region " Page Events "

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then

            Me.btnRicerca.ImageUrl = Me.ResolveUrl("~/images/cerca.gif")

        End If

    End Sub

#End Region

#Region " Button Events "

    Private Sub btnRicerca_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnRicerca.Click

        RaiseEvent Cerca(Me.txtFiltro.Text)

    End Sub

#End Region

#Region " Public "

    Public Sub ClearFilter()

        Me.txtFiltro.Text = String.Empty

    End Sub

    Public Sub SetFilter(filter As String)

        Me.txtFiltro.Text = filter

    End Sub

    Public Function GetFilter() As String

        Return Me.txtFiltro.Text

    End Function

#End Region

#Region " Private "

    Private Sub SetLayout(enabled As Boolean)

        Me.txtFiltro.Enabled = enabled

        Me.btnRicerca.Enabled = enabled

        If enabled Then
            Me.btnRicerca.ImageUrl = Me.ResolveUrl("~/images/cerca.gif")
        Else
            Me.btnRicerca.ImageUrl = Me.ResolveUrl("~/images/cerca_dis.gif")
        End If

    End Sub

#End Region

End Class