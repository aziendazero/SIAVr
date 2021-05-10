Imports Onit.Database.DataAccessManager

Partial  Class OnVacSceltaCicli
    Inherits System.Web.UI.UserControl

#Region " Events "

    Public Event OnConferma(dtCicliScelti As DataTable)
    Public Event OnAnnulla(sender As Object)

#End Region

#Region " Properties "

    Private _dataNascita As Date
    Public Property DataNascita() As Date
        Get
            Return _dataNascita
        End Get
        Set(ByVal Value As Date)
            _dataNascita = Value
        End Set
    End Property

    Private _sesso As String
    Public Property Sesso() As String
        Get
            Return _sesso
        End Get
        Set(ByVal Value As String)
            _sesso = Value
        End Set
    End Property

#End Region

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

    Private Function RecuperaCicli(ByRef excudedCycleCodes As ArrayList) As DataTable

        Dim dtListaCicli As New DataTable("listaCicli")

        Using dam As IDAM = OnVacUtility.OpenDam()

            dam.QB.NewQuery()
            dam.QB.AddSelectFields("0 as CIC_SCELTO", "CIC_CODICE", "CIC_DESCRIZIONE", "CIC_DATA_INTRODUZIONE", "CIC_DATA_FINE", "CIC_STANDARD")
            dam.QB.AddTables("T_ANA_CICLI")

            'imposto le condizioni per escludere i cicli già presenti nella maschera
            If Not IsNothing(excudedCycleCodes) Then
                For i As Integer = 0 To excudedCycleCodes.Count - 1
                    dam.QB.AddWhereCondition("CIC_CODICE", Comparatori.Diverso, excudedCycleCodes(i).ToString, DataTypes.Stringa)
                Next
            End If

            '-- MGR 21/02/2008 controllo sesso nei cicli
            dam.QB.OpenParanthesis()
            dam.QB.AddWhereCondition("cic_sesso", Comparatori.Uguale, "E", DataTypes.Stringa)
            If Sesso <> String.Empty Then
                dam.QB.AddWhereCondition("cic_sesso", Comparatori.Uguale, Sesso, DataTypes.Stringa, "OR")
            End If
            dam.QB.CloseParanthesis()
            '-- fine GMR

            '-- Filtro per cicli attivi
            dam.QB.AddWhereCondition("CIC_OBSOLETO", Comparatori.Uguale, "N", DataTypes.Stringa)


            'ordinamento della query per descrizione (SAMU 11/05/2004)
            dam.QB.AddOrderByFields("CIC_DESCRIZIONE")

            Try
                dam.BuildDataTable(dtListaCicli)
            Finally
                OnVacUtility.CloseDam(dam)
            End Try

        End Using

        Return dtListaCicli

    End Function

    Public Sub CaricaCicli(excudedCycleCodes As ArrayList)
        dgrCicli.DataSource = RecuperaCicli(excudedCycleCodes)
        dgrCicli.DataBind()
    End Sub

    Private Sub UltraWebToolbar1_ButtonClicked(sender As System.Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles UltraWebToolbar1.ButtonClicked
        Select Case e.Button.Key
            Case "btnConferma"
                ConfermaEChiudi()
            Case "btnAnnulla"
                RaiseEvent OnAnnulla(Me)
        End Select
    End Sub

    Private Sub ConfermaEChiudi()

        Dim dt As New DataTable()

        Dim drNew As DataRow
        Dim chk As CheckBox
        Dim strTemp As String

        dt.Columns.Add("CIC_CODICE")
        dt.Columns.Add("CIC_DESCRIZIONE")

        For i As Integer = 0 To dgrCicli.Items.Count - 1
            chk = dgrCicli.Items(i).FindControl("chkCic_scelta")
            If Not IsNothing(chk) AndAlso chk.Checked Then
                drNew = dt.NewRow()
                strTemp = dgrCicli.Items(i).Cells(1).Text
                If strTemp = "&nbsp;" Then
                    strTemp = ""
                End If
                drNew("CIC_CODICE") = strTemp 'codice

                strTemp = dgrCicli.Items(i).Cells(2).Text
                If strTemp = "&nbsp;" Then
                    strTemp = ""
                End If
                drNew("CIC_DESCRIZIONE") = strTemp 'descrizione
                dt.Rows.Add(drNew)

            End If
        Next

        RaiseEvent OnConferma(dt)

    End Sub

    Private Sub dgrCicli_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgrCicli.ItemDataBound

        If e.Item.ItemIndex >= 0 Then

            If Date.Parse(e.Item.DataItem("CIC_DATA_INTRODUZIONE")) <= DataNascita And (e.Item.DataItem("CIC_DATA_FINE") Is DBNull.Value OrElse Date.Parse(e.Item.DataItem("CIC_DATA_FINE")) >= DataNascita) Then
                e.Item.ForeColor = Color.Black
            Else
                e.Item.ForeColor = Color.Red
            End If
            If e.Item.DataItem("CIC_STANDARD") = "T" Then
                e.Item.Font.Bold = True
            End If

        End If

    End Sub

End Class
