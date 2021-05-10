Imports Onit.Database.DataAccessManager

Partial Class ScegliAss
    Inherits System.Web.UI.UserControl

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub
    Protected WithEvents lblLottoDesc As System.Web.UI.WebControls.Label


    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Properties "

    Private _inLotto As String
    Public Property inLotto() As String
        Get
            Return _inLotto
        End Get
        Set(ByVal Value As String)
            _inLotto = Value
        End Set
    End Property

    Private _inNomeCommerciale As String
    Public Property inNomeCommerciale() As String
        Get
            Return _inNomeCommerciale
        End Get
        Set(ByVal Value As String)
            _inNomeCommerciale = Value
        End Set
    End Property

    Private _codAssDefault As String = ""
    Public Property codAssDefault() As String
        Get
            Return _codAssDefault
        End Get
        Set(ByVal Value As String)
            _codAssDefault = Value
        End Set
    End Property

#End Region

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        Me.lblLotto.Text = _inLotto.Trim()

        Me.lblNomeCommerciale.Text = "Nome Commerciale: " & _inNomeCommerciale.Trim()

        Dim dt As New DataTable()

        Using dam As IDAM = OnVacUtility.OpenDam()
            dam.QB.NewQuery()
            dam.QB.AddSelectFields("ass_codice", "ass_descrizione")
            dam.QB.AddTables("t_ana_link_noc_associazioni", "t_ana_lotti", "t_ana_associazioni")
            dam.QB.AddWhereCondition("nal_noc_codice", Comparatori.Uguale, "lot_noc_codice", DataTypes.Join)
            dam.QB.AddWhereCondition("ass_codice", Comparatori.Uguale, "nal_ass_codice", DataTypes.Join)
            dam.QB.AddWhereCondition("lot_codice", Comparatori.Uguale, _inLotto, DataTypes.Stringa)
            dam.BuildDataTable(dt)
        End Using

        Me.rblAssociazioni.DataSource = dt
        Me.rblAssociazioni.DataTextField = "ass_codice" '"ass_descrizione"
        Me.rblAssociazioni.DataValueField = "ass_codice"
        Me.rblAssociazioni.DataBind()

        If _codAssDefault = "" Then
            Me.rblAssociazioni.SelectedIndex = 0
        Else
            If Me.rblAssociazioni.Items.FindByValue(_codAssDefault) Is Nothing Then
                Me.rblAssociazioni.SelectedIndex = 0
            Else
                Me.rblAssociazioni.SelectedValue = _codAssDefault
            End If
        End If

    End Sub

End Class
