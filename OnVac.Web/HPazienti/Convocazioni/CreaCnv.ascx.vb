Imports Onit.Database.DataAccessManager

Partial Class CreaCnv
    Inherits Common.UserControlFinestraModalePageBase

#Region " Eventi "

    Public Event CreaCnv(sender As Object, e As CreaCnvEventArgs)

    Public Class CreaCnvEventArgs
        Inherits EventArgs

        Public Enum CnvType
            Automatica
            Odierna
            Futura
        End Enum

        Public Type As CnvType
        Public CnsCodice As String
        Public CnsNome As String

    End Class

    Protected Sub onCreaCnv(ByRef e As CreaCnvEventArgs)
        RaiseEvent CreaCnv(Me, e)
    End Sub

#End Region

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

    Public ReadOnly Property CurrentType() As CreaCnvEventArgs.CnvType
        Get
            If Me.rbCNVAutomatica.Checked Then
                Return CreaCnvEventArgs.CnvType.Automatica
            ElseIf Me.rbCNVOdierna.Checked Then
                Return CreaCnvEventArgs.CnvType.Odierna
            ElseIf Me.rbCNVFutura.Checked Then
                Return CreaCnvEventArgs.CnvType.Futura
            End If
        End Get
    End Property

    Public ReadOnly Property IsCurrentCNSPaziente() As String
        Get
            Return Me.rbCNSPaziente.Checked
        End Get
    End Property

    Public Property CodiceCNSPaziente() As String
        Get
            Return ViewState("CodiceCNSPaziente")
        End Get
        Private Set(value As String)
            ViewState("CodiceCNSPaziente") = value
        End Set
    End Property

#End Region


    Public Overrides Sub LoadModale()
        '--
        Dim DAM As IDAM = OnVacUtility.OpenDam()
        '--
        With DAM.QB
            .NewQuery()
            .AddTables("t_paz_pazienti", "t_ana_consultori")
            .AddSelectFields("paz_cns_codice", "cns_descrizione")
            .AddWhereCondition("paz_codice", Comparatori.Uguale, OnVacUtility.Variabili.PazId, DataTypes.Numero)
            .AddWhereCondition("cns_codice", Comparatori.Uguale, "paz_cns_codice", DataTypes.Join)
        End With
        '--
        Me.CodiceCNSPaziente = Nothing
        '--
        Try
            '--
            Using reader As IDataReader = DAM.BuildDataReader()
                '--
                If Not reader Is Nothing Then
                    '--
                    Dim pos_cns As Int16 = reader.GetOrdinal("paz_cns_codice")
                    Dim pos_desc As Int16 = reader.GetOrdinal("cns_descrizione")
                    '--
                    If reader.Read() Then
                        '--
                        Dim codiceCNSPazienteTemp As String = reader(pos_cns).ToString()
                        '--
                        If codiceCNSPazienteTemp <> OnVacUtility.Variabili.CNS.Codice Then
                            '--
                            Me.rbCNSLavoro.Text = "Lavoro: " + OnVacUtility.Variabili.CNS.Descrizione
                            Me.rbCNSPaziente.Text = "Paziente: " + reader.Item(pos_desc).ToString()
                            '--
                            Me.CodiceCNSPaziente = codiceCNSPazienteTemp
                            '--
                        End If
                        '--
                    End If
                    '--
                End If
                '--
            End Using
            '--
            If Me.Settings.TIPOCNV = "A" Then
                Me.rbCNVAutomatica.Checked = True
                Me.rbCNVOdierna.Checked = False
                Me.rbCNVFutura.Checked = False
            ElseIf Me.Settings.TIPOCNV = "O" Then
                Me.rbCNVOdierna.Checked = True
                Me.rbCNVAutomatica.Checked = False
                Me.rbCNVFutura.Checked = False
            ElseIf Me.Settings.TIPOCNV = "F" Then
                Me.rbCNVFutura.Checked = True
                Me.rbCNVOdierna.Checked = False
                Me.rbCNVAutomatica.Checked = False
            Else
                Throw New NotSupportedException()
            End If
            '--
            If String.IsNullOrEmpty(Me.CodiceCNSPaziente) Then
                '--
                Me.lbSameCNS.Text = OnVacUtility.Variabili.CNS.Descrizione
                '--
                Me.tblCNS.Visible = False
                '--
            Else
                '--
                If Me.Settings.CNSCNV = "L" Then
                    Me.rbCNSLavoro.Checked = True
                    Me.rbCNSPaziente.Checked = False
                Else
                    Me.rbCNSPaziente.Checked = True
                    Me.rbCNSLavoro.Checked = False
                End If
                '--
                Me.EnableCNV()
                '--
                Me.lbSameCNS.Visible = False
                '--
            End If
            '--
        Finally
            '--
            OnVacUtility.CloseDam(DAM)
            '--
        End Try
        '--
    End Sub

    Private Sub rbCNSLavoro_CheckedChanged(sender As Object, e As EventArgs) Handles rbCNSLavoro.CheckedChanged
        Me.EnableCNV()
    End Sub

    Private Sub rbCNSPaziente_CheckedChanged(sender As Object, e As EventArgs) Handles rbCNSPaziente.CheckedChanged
        Me.EnableCNV()
    End Sub

    Private Sub btnCrea_Click(sender As System.Object, e As System.EventArgs) Handles btnCrea.Click

        Dim arg As New CreaCnvEventArgs

        If Me.IsCurrentCNSPaziente Then
            arg.CnsCodice = Me.CodiceCNSPaziente
            arg.CnsNome = rbCNSPaziente.Text.Replace("Paziente: ", "")
        Else
            arg.CnsCodice = OnVacUtility.Variabili.CNS.Codice
            arg.CnsNome = OnVacUtility.Variabili.CNS.Descrizione
        End If

        arg.Type = Me.CurrentType

        onCreaCnv(arg)

    End Sub

    Private Sub EnableCNV()
        '--
        If Me.rbCNSLavoro.Checked Then
            '--
            Me.rbCNVAutomatica.Enabled = False
            Me.rbCNVAutomatica.Checked = False
            '--
            Me.rbCNVOdierna.Checked = True
            Me.rbCNVOdierna.Enabled = True
            '--
            Me.rbCNVFutura.Checked = False
            Me.rbCNVFutura.Enabled = True
            '--
        Else
            '--
            Me.rbCNVAutomatica.Enabled = True
            '--
            Me.rbCNVOdierna.Enabled = True
            '--
            Me.rbCNVFutura.Enabled = True
            '--
        End If
        '--
    End Sub

End Class
