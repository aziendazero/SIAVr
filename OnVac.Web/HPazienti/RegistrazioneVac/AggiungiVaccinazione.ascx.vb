Imports Onit.Database.DataAccessManager

Partial Class AggiungiVaccinazione
    Inherits Common.UserControlFinestraModalePageBase

    Protected WithEvents OnitLayout31 As Onit.Controls.PagesLayout.OnitLayout3

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region


    Public Event InviaDati(ByVal dtCodici As DataTable, ByVal tipoAggiunta As String)
    Public Event AbilitaBar()

    Protected Sub OnInviaDati(ByVal dtCodici As DataTable, ByVal tipoAggiunta As String)
        RaiseEvent InviaDati(dtCodici, tipoAggiunta)
    End Sub

    'abilita Left e TopBar disabilitati precedentemente (modifica 08/06/2004)
    Protected Sub OnAbilitaBar()
        RaiseEvent AbilitaBar()
    End Sub

    Public Overrides Sub LoadModale()
       
        Dim dam As IDAM = OnVacUtility.OpenDam()
        Dim dta As New DataTable
        '--
        dam.QB.NewQuery()
        dam.QB.IsDistinct = True
        dam.QB.AddSelectFields("ASS_CODICE", "ASS_DESCRIZIONE", "ASS_ORDINE")

        dam.QB.AddTables("T_ANA_ASSOCIAZIONI", "T_ANA_LINK_ASS_VACCINAZIONI")
        If Me.Settings.ASSOCIAZIONI_TIPO_CNS Then
            dam.QB.AddTables("T_ANA_ASSOCIAZIONI_TIPI_CNS", "T_ANA_CONSULTORI")
        End If

        dam.QB.AddWhereCondition("ASS_CODICE", Comparatori.Uguale, "VAL_ASS_CODICE", DataTypes.Join)
        If Me.Settings.ASSOCIAZIONI_TIPO_CNS Then

            dam.QB.AddWhereCondition("VAL_ASS_CODICE", Comparatori.Uguale, "ATC_ASS_CODICE", DataTypes.Join)
            dam.QB.AddWhereCondition("ATC_CNS_TIPO", Comparatori.Uguale, "CNS_TIPO", DataTypes.Join)
            dam.QB.AddWhereCondition("CNS_CODICE", Comparatori.Uguale, OnVacUtility.Variabili.CNS.Codice, DataTypes.Stringa)

        End If
        dam.QB.AddOrderByFields("ASS_ORDINE", "ASS_DESCRIZIONE")

        Try
            dam.BuildDataTable(dta)
        Finally
            OnVacUtility.CloseDam(dam)
        End Try

        dgrElencoAss.DataSource = dta
        dgrElencoAss.DataBind()
        dgrElencoAss.Visible = True

    End Sub


    Private Sub ToolBar_ButtonClicked(ByVal sender As Object, ByVal e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked
        Dim dtCodici As DataTable
        Select Case (e.Button.Key)
            Case "btnAggiungi"

                dtCodici = associateCekkate()
                OnInviaDati(dtCodici, "Associate")
               
                'nel caso dell'annullamento deve disabilitare Left e TopBar (modifica 08/06/2004)
            Case "btnAnnulla"
                OnAbilitaBar()
        End Select
    End Sub

    Private Function associateCekkate() As DataTable

        Dim dtCodici As New DataTable
        Dim i, dose As Integer
        Dim txt As TextBox

        dtCodici.Columns.Add("codice", GetType(String))
        dtCodici.Columns.Add("dose", GetType(Integer))

        For i = 0 To dgrElencoAss.Items.Count - 1
            txt = DirectCast(dgrElencoAss.Items(i).FindControl("txt_dosi"), TextBox)
            If (DirectCast(dgrElencoAss.Items(i).FindControl("chkSelezionaAss"), CheckBox).Checked) Then
                If (txt.Text = "") Then
                    dose = 1
                Else
                    dose = Integer.Parse(txt.Text)
                End If
                dtCodici.Rows.Add(New Object() {dgrElencoAss.Items(i).Cells(1).Text, dose})
            End If
        Next

        Return dtCodici

    End Function


End Class
