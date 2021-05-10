Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.Controls

Partial Class SelezioneVaccinazioni
    Inherits Onit.OnAssistnet.OnVac.Common.UserControlPageBase

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

    Public Event OnSelectionError(errorMessage As String)

    Private Sub SelectionError(errorMessage As String)
        RaiseEvent OnSelectionError(errorMessage)
    End Sub
#Region " Properties "

    '''<summary>
    ''' Stringa dei codici selezionati, separati da "|"
    '''</summary>
    Public Property CodiciSelezionati() As String
        Get
            If ViewState("CodiciSelezionati") Is Nothing Then Return String.Empty
            Return ViewState("CodiciSelezionati").ToString()
        End Get
        Set(ByVal value As String)
            ViewState("CodiciSelezionati") = value
        End Set
    End Property

    Public Property CodiciSelezionatiVisibiliMax() As Int16
        Get
            If ViewState("CodiciSelezionatiVisibiliMax") Is Nothing Then Return Int16.MaxValue
            Return Convert.ToInt16(ViewState("CodiciSelezionatiVisibiliMax"))
        End Get
        Set(ByVal value As Int16)
            ViewState("CodiciSelezionatiVisibiliMax") = value
        End Set
    End Property

    Public Property Enabled() As Boolean
        Get
            If ViewState("Enabled") Is Nothing Then Return False
            Return Convert.ToBoolean(ViewState("Enabled"))
        End Get
        Set(ByVal value As Boolean)
            ViewState("Enabled") = value
        End Set
    End Property

    Public Property ImageUrl() As String
        Get
            If ViewState("ImageUrl") Is Nothing Then Return String.Empty
            Return ViewState("ImageUrl")
        End Get
        Set(ByVal value As String)
            ViewState("ImageUrl") = value
        End Set
    End Property

    Public Property ImageUrlHovered() As String
        Get
            If ViewState("ImageUrlHovered") Is Nothing Then Return String.Empty
            Return ViewState("ImageUrlHovered")
        End Get
        Set(ByVal value As String)
            ViewState("ImageUrlHovered") = value
        End Set
    End Property

    Public Property ImageUrlDisabled() As String
        Get
            If ViewState("ImageUrlDisabled") Is Nothing Then Return String.Empty
            Return ViewState("ImageUrlDisabled")
        End Get
        Set(ByVal value As String)
            ViewState("ImageUrlDisabled") = value
        End Set
    End Property

#End Region

    Private Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender

        Me.btnSelezionaVac.Enabled = Me.Enabled

        If Me.Enabled Then
            Me.btnSelezionaVac.ImageUrl = Me.ImageUrl

            Me.Attributes.Add("style", "cursor: pointer;")

            ' attributi per l'hover
            Me.btnSelezionaVac.Attributes.Add("onmouseover", String.Format("this.src='{0}'", Me.ImageUrlHovered))
            Me.btnSelezionaVac.Attributes.Add("onmouseout", String.Format("this.src='{0}'", Me.ImageUrl))

        Else
            Me.btnSelezionaVac.ImageUrl = Me.ImageUrlDisabled
            ' Dim s As String() = Me.btnSelezionaVac.ImageUrl.Split(".")
            ' String.Format("{0}_dis.{1}", String.Join("", s.Take(s.Length - 1)), s.Last())
        End If

        Me.lblVaccinazioni.Text = Me.GetDescrizioneLabel()

    End Sub

#Region " Public Methods "

    ' Visualizza all'apertura della modale i motivi di esclusione selezionati precedentemente con un segno di spunta 
    Private Sub SelezionaVaccinazioni(codiciVaccinazioni As String)

        ' Ricerca il codice del motivo di esclusione all'inyterno del textbox preceduto e seguito dal separatore virgola 
        ' Se trova il codice valorizza il motivo di esclusione con il segno di spunta
        codiciVaccinazioni = "|" + codiciVaccinazioni + "|"

        For i As Int16 = 0 To dgrVaccinazioni.Items.Count - 1

            Dim codice As String = dgrVaccinazioni.Items(i).Cells(2).Text

            Dim idx As Integer = codiciVaccinazioni.IndexOf("|" + codice + "|")

            Dim chk As CheckBox = DirectCast(dgrVaccinazioni.Items(i).FindControl("cb"), CheckBox)
            If idx = -1 Then
                chk.Checked = False
            Else
                chk.Checked = True
            End If

        Next

    End Sub

#End Region

#Region " Private Methods "

    Private Sub LoadVaccinazioni()

        Dim dt As New DataTable()

        Using DAM As IDAM = OnVacUtility.OpenDam()

            With DAM
                .QB.NewQuery(False, False)
                .QB.AddSelectFields("vac_codice", "vac_descrizione")
                .QB.AddTables("t_ana_vaccinazioni")
                .BuildDataTable(dt)
            End With

        End Using

        dt.DefaultView.Sort = "vac_codice asc"

        dgrVaccinazioni.DataSource = dt.DefaultView
        dgrVaccinazioni.DataBind()

    End Sub

    Private Function GetCodiceVaccinazioniSelezionati() As String

        Dim s As New List(Of String)
        Dim count As Integer

        For count = 0 To dgrVaccinazioni.Items.Count - 1
            If CType(dgrVaccinazioni.Items(count).FindControl("cb"), CheckBox).Checked Then
                s.Add(dgrVaccinazioni.Items(count).Cells(2).Text)
            End If
        Next

        Return String.Join("|", s.ToArray())

    End Function

    Private Function GetDescrizioneLabel()

        Dim result As String = String.Empty
        Dim s As List(Of String) = Me.CodiciSelezionati.Split("|").ToList()

        If s.Count > 0 Then
            result = String.Join(", ", s.Take(CodiciSelezionatiVisibiliMax))
            If s.Count > Me.CodiciSelezionatiVisibiliMax Then result = String.Format("{0}, ...", result)
        End If

        Return result

    End Function

#End Region

#Region " Button Events "

    Protected Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btn_Conferma"

                ' Memorizzazione selezione, e chiusura finestra modale
                Me.CodiciSelezionati = Me.GetCodiceVaccinazioniSelezionati()
                Me.fmVaccinazioni.VisibileMD = False

            Case "btn_Annulla"

                Me.fmVaccinazioni.VisibileMD = False

        End Select

    End Sub

    Protected Sub btnSelezionaVac_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnSelezionaVac.Click

        Me.LoadVaccinazioni()
        Me.SelezionaVaccinazioni(Me.CodiciSelezionati)
        Me.fmVaccinazioni.VisibileMD = True

    End Sub

#End Region

End Class
