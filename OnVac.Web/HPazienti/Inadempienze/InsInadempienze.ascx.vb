Imports Onit.Database.DataAccessManager


Partial Class InsInadempienze
    Inherits Common.UserControlFinestraModalePageBase

    Public Event InsInadempienza()

    Protected Sub OnInsInadempienza()
        RaiseEvent InsInadempienza()
    End Sub

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

    Property dt_Inadempienze() As DataTable
        Get
            Return Session("OnVac_dt_Inadempienze")
        End Get
        Set(ByVal Value As DataTable)
            Session("OnVac_dt_Inadempienze") = Value
        End Set
    End Property

    Property dt_VacObb() As DataTable
        Get
            Return Session("OnVac_dt_VacObb")
        End Get
        Set(ByVal Value As DataTable)
            Session("OnVac_dt_VacObb") = Value
        End Set
    End Property

    Public strJS As String

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
    End Sub

    Public Overrides Sub LoadModale()

        If Not dt_VacObb Is Nothing Then dt_VacObb.Dispose()
        dt_VacObb = New DataTable()

        Dim DAM As IDAM = OnVacUtility.OpenDam()

        With DAM.QB
            .NewQuery()
            .AddTables("t_ana_vaccinazioni")
            .AddSelectFields("vac_descrizione,vac_codice")
            .AddWhereCondition("vac_obbligatoria", Comparatori.Uguale, "A", DataTypes.Stringa)
            For i As Int16 = 0 To dt_Inadempienze.Rows.Count - 1
                If Not dt_Inadempienze.Rows(i).RowState = DataRowState.Deleted Then
                    .AddWhereCondition("vac_codice", Comparatori.Diverso, dt_Inadempienze.Rows(i)("pin_vac_codice"), DataTypes.Stringa)
                End If
            Next
            'non deve considerare le vaccinazioni che sono sostitute
            .AddWhereCondition("vac_cod_sostituta", Comparatori.Is, "null", DataTypes.Replace)
            .AddOrderByFields("vac_ordine ASC")
        End With

        Try
            DAM.BuildDataTable(dt_VacObb)
        Finally
            OnVacUtility.CloseDam(DAM)
        End Try

        dg_vac.DataSource = dt_VacObb
        dg_vac.DataBind()

    End Sub

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btn_Conferma"
                ' ------------------------------------------------------ '
                ' Codifica degli stati (impostati nella t_ana_codifiche)
                '   1: TERMINE PERENTORIO
                '   2: COMUNICAZIONE AL SINDACO
                '   3: CASO CONCLUSO
                ' ------------------------------------------------------ '
                Dim _caso_concluso As Int16
                _caso_concluso = Enumerators.StatoInadempienza.CasoConcluso

                Dim nAdd As Int16
                Dim newrow As DataRow
                Dim item As DataGridItem

                For Each item In dg_vac.Items

                    If (CType(item.Cells(0).FindControl("cb"), CheckBox).Checked) Then
                        newrow = dt_Inadempienze.NewRow()
                        newrow("vac_descrizione") = item.Cells(1).Text
                        newrow("pin_vac_codice") = item.Cells(2).Text
                        newrow("pin_paz_codice") = OnVacUtility.Variabili.PazId
                        newrow("pin_data") = Date.Now.Day & "/" & Date.Now.Month & "/" & Date.Now.Year
                        newrow("pin_ute_id") = OnVacContext.UserId
                        newrow("ute_descrizione") = OnVacContext.UserDescription
                        newrow("pin_stato") = _caso_concluso.ToString()
                        newrow("cod_descrizione") = "CASO CONCLUSO"
                        dt_Inadempienze.Rows.InsertAt(newrow, 0)
                        nAdd += 1
                    End If

                Next

                dt_Inadempienze.DefaultView.Sort = "pin_vac_codice ASC"
                dt_Inadempienze.DefaultView.Sort = ""

                If (nAdd > 0) Then
                    OnInsInadempienza()
                End If

        End Select

    End Sub

End Class