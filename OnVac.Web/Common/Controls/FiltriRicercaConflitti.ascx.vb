Public Class FiltriRicercaConflitti
    Inherits System.Web.UI.UserControl

#Region " Properties "

    Public Property UpperCase() As Boolean
        Get
            If ViewState("FiltriRicercaConflitti_UpperCase") Is Nothing Then ViewState("FiltriRicercaConflitti_UpperCase") = True
            Return ViewState("FiltriRicercaConflitti_UpperCase")
        End Get
        Set(value As Boolean)
            ViewState("FiltriRicercaConflitti_UpperCase") = value
        End Set
    End Property

    Public Property Enabled As Boolean
        Get
            If ViewState("FiltriRicercaConflitti_Enabled") Is Nothing Then ViewState("FiltriRicercaConflitti_Enabled") = True
            Return ViewState("FiltriRicercaConflitti_Enabled")
        End Get
        Set(value As Boolean)
            Me.lblCognome.Enabled = value
            Me.txtCognome.Enabled = value
            Me.lblNome.Enabled = value
            Me.txtNome.Enabled = value
            Me.lblDataNascita.Enabled = value
            Me.lblDa.Enabled = value
            Me.dpkDataNascitaDa.Enabled = value
            Me.lblA.Enabled = value
            Me.dpkDataNascitaA.Enabled = value
            ViewState("FiltriRicercaConflitti_Enabled") = value
        End Set
    End Property

#End Region

#Region " Page Events "

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

    End Sub

#End Region

#Region " Public "

    Public Function GetFiltriRicerca() As Filters.FiltriRicercaConflittiDatiVaccinali

        Dim filtriRicerca As New Filters.FiltriRicercaConflittiDatiVaccinali()

        If Me.UpperCase Then
            Me.txtCognome.Text = Me.txtCognome.Text.Trim().ToUpper()
            Me.txtNome.Text = Me.txtNome.Text.Trim().ToUpper()
        End If

        filtriRicerca.CognomePaziente = Me.txtCognome.Text
        filtriRicerca.NomePaziente = Me.txtNome.Text

        If String.IsNullOrEmpty(Me.dpkDataNascitaDa.Text) Then
            filtriRicerca.DataNascitaMinima = Nothing
        Else
            filtriRicerca.DataNascitaMinima = Me.dpkDataNascitaDa.Data
        End If

        If String.IsNullOrEmpty(Me.dpkDataNascitaA.Text) Then
            filtriRicerca.DataNascitaMassima = Nothing
        Else
            filtriRicerca.DataNascitaMassima = Me.dpkDataNascitaA.Data
        End If

        Return filtriRicerca

    End Function

    Public Sub SetFiltriRicerca(filtriRicerca As Filters.FiltriRicercaConflittiDatiVaccinali)

        If filtriRicerca Is Nothing Then
            Me.txtCognome.Text = String.Empty
            Me.txtNome.Text = String.Empty
            Me.dpkDataNascitaDa.Text = String.Empty
            Me.dpkDataNascitaA.Text = String.Empty
        Else
            Me.txtCognome.Text = filtriRicerca.CognomePaziente
            Me.txtNome.Text = filtriRicerca.NomePaziente
            Me.dpkDataNascitaDa.Data = filtriRicerca.DataNascitaMinima
            Me.dpkDataNascitaA.Data = filtriRicerca.DataNascitaMassima
        End If

    End Sub

    Public Function IsEmpty() As Boolean

        If String.IsNullOrEmpty(Me.txtCognome.Text) And
           String.IsNullOrEmpty(Me.txtNome.Text) And
           String.IsNullOrEmpty(Me.dpkDataNascitaDa.Text) And
           String.IsNullOrEmpty(Me.dpkDataNascitaA.Text) Then

            Return True

        End If

        Return False

    End Function

#End Region

End Class