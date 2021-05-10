Imports Onit.Database.DataAccessManager
Imports Onit.Controls


Partial Class MotivoEsc
    Inherits Common.UserControlFinestraModalePageBase

#Region " Event "

    Public Event InviaCodMotEsc(arrMotEscCod As ArrayList, arrMotEscDesc As ArrayList)
    Public Event RiabilitaLayout()

    'scatena l'evento per riabilitare il layout
    Protected Sub OnRiabilitaLayout()
        RaiseEvent RiabilitaLayout()
    End Sub

    'invio codici dei motivi di esclusione selezionati alla pagina principale
    Protected Sub OnInviaCodMotEsc(ByVal arrMotEscCod As ArrayList, ByVal arrMotEscDesc As ArrayList)
        RaiseEvent InviaCodMotEsc(arrMotEscCod, arrMotEscDesc)
    End Sub

#End Region

    Public Property MotiviSelezionati() As String
        Get
            Return Session("OnVac_MotiviSelezionati")
        End Get
        Set(ByVal Value As String)
            Session("OnVac_MotiviSelezionati") = Value
        End Set
    End Property

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

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then
            MotiviSelezionati = String.Empty
        End If

    End Sub

    Public Overrides Sub LoadModale()
        CaricaMotiviEsclusione()
        SelezionaMotiviEsclusione(MotiviSelezionati)
    End Sub

    Private Sub CaricaMotiviEsclusione()

        Dim dtMotiviEsclusione As New DataTable()

        Using DAM As IDAM = OnVacUtility.OpenDam()

            With DAM
                .QB.NewQuery(False, False)
                .QB.AddSelectFields("MOE_CODICE", "MOE_DESCRIZIONE")
                .QB.AddTables("T_ANA_MOTIVI_ESCLUSIONE")

                ' N.B. : no filtro MOE_OBSOLETO = 'N' perchè questo U.C. è usato solo in punti in cui devono essere visualizzati tutti i motivi.

                .BuildDataTable(dtMotiviEsclusione)
            End With

        End Using

        dtMotiviEsclusione.DefaultView.Sort = "MOE_CODICE ASC"

        dg_moe_esc.DataSource = dtMotiviEsclusione.DefaultView
        dg_moe_esc.DataBind()

    End Sub

    ' Visualizza all'apertura della modale i motivi di esclusione selezionati precedentemente con un segno di spunta 
    Private Sub SelezionaMotiviEsclusione(motivi As String)

        ' Ricerca il codice del motivo di esclusione all'inyterno del textbox preceduto e seguito dal separatore virgola 
        ' Se trova il codice valorizza il motivo di esclusione con il segno di spunta
        motivi = "," + motivi + ","

        For i As Int16 = 0 To dg_moe_esc.Items.Count - 1

            Dim codice As String = dg_moe_esc.Items(i).Cells(2).Text
            Dim idx As Integer

            idx = motivi.IndexOf("," + codice + ",")

            Dim chk As CheckBox = DirectCast(dg_moe_esc.Items(i).FindControl("cb"), CheckBox)
            If idx = -1 Then
                chk.Checked = False
            Else
                chk.Checked = True
            End If

        Next

    End Sub

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btn_Annulla"
                'nel caso di annulla deve solamente riabilitare il layout
                OnRiabilitaLayout()
                MotiviSelezionati = String.Empty

            Case "btn_Conferma"
                'deve memorizzare i codici dei motivi di esclusione selezionati
                ConfermaMotiviEsclusione()
                MotiviSelezionati = String.Empty

        End Select

    End Sub

    Private Sub ConfermaMotiviEsclusione()

        Dim arrMotEscCod As New ArrayList
        Dim arrMotEscDesc As New ArrayList

        For count As Integer = 0 To dg_moe_esc.Items.Count - 1
            If DirectCast(dg_moe_esc.Items(count).FindControl("cb"), CheckBox).Checked Then
                arrMotEscCod.Add(dg_moe_esc.Items(count).Cells(2).Text)
                arrMotEscDesc.Add(dg_moe_esc.Items(count).Cells(1).Text)
            End If
        Next

        OnInviaCodMotEsc(arrMotEscCod, arrMotEscDesc)

    End Sub

    Public Function GetSelectedDescriptions() As String

        Dim s As New System.Text.StringBuilder()

        For count As Integer = 0 To dg_moe_esc.Items.Count - 1
            If DirectCast(dg_moe_esc.Items(count).FindControl("cb"), CheckBox).Checked Then
                s.Append(dg_moe_esc.Items(count).Cells(1).Text)
                s.Append("; ")
            End If
        Next

        Return s.ToString()

    End Function

End Class
