Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager

Imports Onit.Controls


Partial Class OnVac_TipiCNSAss
    Inherits OnVac.Common.PageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(sender As System.Object, e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Properties "

    Private ReadOnly Property CodiceAssociazione() As String
        Get
            Return Request.QueryString("ASS").ToString.Split("|".ToCharArray())(0)
        End Get
    End Property

    Private ReadOnly Property DescrizioneAssociazione() As String
        Get
            Return Request.QueryString("ASS").ToString.Split("|".ToCharArray())(1)
        End Get
    End Property

#End Region

#Region " Page "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        '--
        If Not Me.IsPostBack() Then
            '--
            Me.Initialize()
            '--
            Me.LayoutTitolo.Text = Me.DescrizioneAssociazione
            '--
        End If
        '--
    End Sub

#End Region

#Region " Toolbar "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked
        Select Case (e.Button.Key)
            Case "btn_Salva"
                '--
                Salva()
                '--
            Case "btn_Annulla"
                '--
                Annulla()
                '--
        End Select
    End Sub

#End Region

#Region " Private "

    Private Sub Initialize()
        '--
        Dim tipiCNS As String() = Me.LoadTipiCns()
        '--
        Me.BindTipiCns(tipiCNS)
        '--
    End Sub

    Private Function LoadTipiCns() As String()
        '--
        Dim tipiCns As New List(Of String)
        '--
        Dim dam As IDAM = OnVacUtility.OpenDam()
        '--
        Try
            '--
            dam.QB.NewQuery()
            dam.QB.AddTables("T_ANA_ASSOCIAZIONI_TIPI_CNS")
            dam.QB.AddSelectFields("ATC_CNS_TIPO")
            dam.QB.AddWhereCondition("ATC_ASS_CODICE", Comparatori.Uguale, Me.CodiceAssociazione, DataTypes.Stringa)
            '--
            Using dataReader As IDataReader = dam.BuildDataReader()
                '--
                While dataReader.Read
                    tipiCns.Add(dataReader.GetString(0))
                End While
                '--
            End Using
            '--
        Finally
            OnVacUtility.CloseDam(dam)
        End Try
        '--
        Return tipiCns.ToArray()
        '--
    End Function

    Private Sub BindTipiCns(tipiCns As String())
        '--
        Me.chklTipiCNS.ClearSelection()
        '--
        For Each cbliTipiCNS As ListItem In Me.chklTipiCNS.Items
            cbliTipiCNS.Selected = Array.IndexOf(tipiCns, cbliTipiCNS.Value) > -1
        Next
        '--
    End Sub

    Private Sub Annulla()
        '--
        Me.Initialize()
        '--
    End Sub

    Private Sub Salva()
        '--
        Dim dam As IDAM = OnVacUtility.OpenDam()
        '--
        Try
            '--
            dam.BeginTrans()
            '--
            dam.QB.NewQuery()
            dam.QB.AddTables("T_ANA_ASSOCIAZIONI_TIPI_CNS")
            dam.QB.AddWhereCondition("ATC_ASS_CODICE", Comparatori.Uguale, Me.CodiceAssociazione, DataTypes.Stringa)
            '--
            dam.ExecNonQuery(ExecQueryType.Delete)
            '--
            For Each cbliTipiCNS As ListItem In Me.chklTipiCNS.Items
                '--
                If cbliTipiCNS.Selected Then
                    '--
                    dam.QB.NewQuery()
                    dam.QB.AddTables("T_ANA_ASSOCIAZIONI_TIPI_CNS")
                    dam.QB.AddInsertField("ATC_ASS_CODICE", Me.CodiceAssociazione, DataTypes.Stringa)
                    dam.QB.AddInsertField("ATC_CNS_TIPO", cbliTipiCNS.Value, DataTypes.Stringa)
                    '--
                    dam.ExecNonQuery(ExecQueryType.Insert)
                    '--
                End If
            Next
            '--
            dam.Commit()
            '--
        Catch
            '--
            If dam.ExistTra Then dam.Rollback()
            '--
        Finally
            '--
            OnVacUtility.CloseDam(dam)
            '--
        End Try
        '--
    End Sub

#End Region

End Class
