Public Class CampiEtaAttivazione
    Inherits Common.UserControlPageBase

#Region " Properties "

    Public Property LabelCssClass() As String
        Get
            Return ViewState("css_lbl")
        End Get
        Set(value As String)
            ViewState("css_lbl") = value
            lblEtaAnni.CssClass = value
            lblEtaMesi.CssClass = value
            lblEtaGiorni.CssClass = value
        End Set
    End Property

    Public Property TextBoxCssClass() As String
        Get
            Return ViewState("css_txt")
        End Get
        Set(value As String)
            ViewState("css_txt") = value
        End Set
    End Property

    Public Property TextBoxDisabledCssClass() As String
        Get
            Return ViewState("css_txt_dis")
        End Get
        Set(value As String)
            ViewState("css_txt_dis") = value
        End Set
    End Property

    Public Overrides Property Visible As Boolean
        Get
            Return MyBase.Visible
        End Get
        Set(value As Boolean)
            MyBase.Visible = value
            lblEtaAnni.Visible = value
            lblEtaMesi.Visible = value
            lblEtaGiorni.Visible = value
            txtEtaAnni.Visible = value
            txtEtaMesi.Visible = value
            txtEtaGiorni.Visible = value
        End Set
    End Property

    Public Property Enabled As Boolean
        Get
            If ViewState("en") Is Nothing Then ViewState("en") = True
            Return ViewState("en")
        End Get
        Set(value As Boolean)

            ViewState("en") = value

            txtEtaAnni.Enabled = value
            txtEtaMesi.Enabled = value
            txtEtaGiorni.Enabled = value

            If value Then
                txtEtaAnni.CssClass = TextBoxCssClass
                txtEtaMesi.CssClass = TextBoxCssClass
                txtEtaGiorni.CssClass = TextBoxCssClass
            Else
                txtEtaAnni.CssClass = TextBoxDisabledCssClass
                txtEtaMesi.CssClass = TextBoxDisabledCssClass
                txtEtaGiorni.CssClass = TextBoxDisabledCssClass
            End If

        End Set
    End Property

#End Region

#Region " Eventi Page "

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        If Enabled Then
            txtEtaAnni.CssClass = TextBoxCssClass
            txtEtaMesi.CssClass = TextBoxCssClass
            txtEtaGiorni.CssClass = TextBoxCssClass
        Else
            txtEtaAnni.CssClass = TextBoxDisabledCssClass
            txtEtaMesi.CssClass = TextBoxDisabledCssClass
            txtEtaGiorni.CssClass = TextBoxDisabledCssClass
        End If

    End Sub

#End Region

#Region " Public "

    Public Sub Clear()
        txtEtaAnni.Text = String.Empty
        txtEtaMesi.Text = String.Empty
        txtEtaGiorni.Text = String.Empty
    End Sub

    Public Sub SetGiorni(giorniTotali As Integer?)

        If Not giorniTotali.HasValue Then

            Clear()

        Else

            Dim etaAttivazione As New Entities.Eta(giorniTotali.Value)

            txtEtaAnni.Text = etaAttivazione.Anni.ToString()
            txtEtaMesi.Text = etaAttivazione.Mesi.ToString()
            txtEtaGiorni.Text = etaAttivazione.Giorni.ToString()

        End If

    End Sub

    Public Sub SetValueAnnoMeseGiorno(anno As Integer?, mese As Integer?, giorno As Integer?)

        If Not anno.HasValue Then
            txtEtaAnni.Text = String.Empty
        Else
            txtEtaAnni.Text = anno.ToString()
        End If

        If Not mese.HasValue Then
            txtEtaMesi.Text = String.Empty
        Else
            txtEtaMesi.Text = mese.ToString()
        End If

        If Not giorno.HasValue Then
            txtEtaGiorni.Text = String.Empty
        Else
            txtEtaGiorni.Text = giorno.ToString()
        End If

    End Sub

    Public Function GetGiorniTotali() As Integer?

        Dim etaAttivazione As Entities.Eta = Biz.BizLotti.GetEtaFromValoreCampi(txtEtaAnni.Text, txtEtaMesi.Text, txtEtaGiorni.Text)

        Dim totGiorniEta As Integer? = Nothing

        If etaAttivazione Is Nothing Then

            Clear()

        Else

            txtEtaAnni.Text = etaAttivazione.Anni.ToString()
            txtEtaMesi.Text = etaAttivazione.Mesi.ToString()
            txtEtaGiorni.Text = etaAttivazione.Giorni.ToString()

            totGiorniEta = etaAttivazione.GiorniTotali

        End If

        Return totGiorniEta

    End Function

    Public Function GetClientIdCampoAnno()

        Return txtEtaAnni.ClientID

    End Function

    Public Function GetValueCampoMese() As String

        Return txtEtaMesi.Text

    End Function

    Public Function GetValueCampoAnno() As String

        Return txtEtaAnni.Text

    End Function

    Public Function GetValueCampoGiorno() As String

        Return txtEtaGiorni.Text

    End Function

#End Region

End Class