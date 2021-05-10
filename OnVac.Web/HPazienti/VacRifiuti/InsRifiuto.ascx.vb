Imports Onit.Database.DataAccessManager


Partial Class InsRifiuto
    Inherits Common.UserControlFinestraModalePageBase

    Public Event InsRifiuto()

    Protected Sub OnInsRifiuto()
        RaiseEvent InsRifiuto()
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

    Property dt_Rifiuti() As DataTable
        Get
            Return Session("OnVac_dt_Rifiuti")
        End Get
        Set(ByVal Value As DataTable)
            Session("OnVac_dt_Rifiuti") = Value
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

        Using DAM As IDAM = OnVacUtility.OpenDam()

            DAM.QB.NewQuery()
            DAM.QB.AddTables("t_ana_vaccinazioni")
            DAM.QB.AddSelectFields("vac_descrizione,vac_codice")
            '        DAM.QB.AddWhereCondition("vac_obbligatoria", Comparatori.Uguale, "A", DataTypes.Stringa)
            For i As Int16 = 0 To dt_Rifiuti.Rows.Count - 1
                If Not dt_Rifiuti.Rows(i).RowState = DataRowState.Deleted Then
                    DAM.QB.AddWhereCondition("vac_codice", Comparatori.Diverso, dt_Rifiuti.Rows(i)("prf_vac_codice"), DataTypes.Stringa)
                End If
            Next
            '---
            'NUOVA VERSIONE [modifica 22/08/2005]
            'non deve considerare le vaccinazioni che sono sostitute
            DAM.QB.AddWhereCondition("vac_cod_sostituta", Comparatori.Is, "null", DataTypes.Replace)
            '---
            DAM.QB.AddOrderByFields("vac_ordine ASC")

            DAM.BuildDataTable(dt_VacObb)

        End Using

        dg_vac.DataSource = dt_VacObb
        dg_vac.DataBind()

    End Sub

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btn_Conferma"

                Dim nAdd As Int16
                Dim newrow As DataRow
                Dim item As DataGridItem

                For Each item In dg_vac.Items

                    If (CType(item.Cells(0).FindControl("cb"), CheckBox).Checked) Then
                        newrow = dt_Rifiuti.NewRow()
                        newrow("vac_descrizione") = item.Cells(1).Text
                        newrow("prf_vac_codice") = item.Cells(2).Text
                        newrow("prf_paz_codice") = OnVacUtility.Variabili.PazId
                        newrow("prf_data_rifiuto") = Date.Now.Day & "/" & Date.Now.Month & "/" & Date.Now.Year
                        newrow("prf_ute_id") = OnVacContext.UserId
                        newrow("ute_descrizione") = OnVacContext.UserDescription
                        '------------------------------------------------
                        'nuova codifica degli stati (modifica 11/01/2005)
                        '1 --> TERMINE PERENTORIO
                        '2 --> COMUNICAZIONE AL SINDACO
                        '3 --> CASO CONCLUSO
                        '------------------------------------------------
                        'newrow("pin_stato") = "3"
                        'newrow("cod_descrizione") = "CASO CONCLUSO"
                        dt_Rifiuti.Rows.InsertAt(newrow, 0)
                        nAdd += 1
                    End If

                Next

                dt_Rifiuti.DefaultView.Sort = "prf_data_rifiuto DESC"
                dt_Rifiuti.DefaultView.Sort = ""

                If (nAdd > 0) Then
                    OnInsRifiuto()
                End If

        End Select

    End Sub

End Class