Partial Public Class GestionePazientiMessage
    Inherits System.Web.UI.UserControl

#Region "Properties"

    Public Property DescrizionePaziente() As String
        Get
            Return ViewState("PazMsg_Descr")
        End Get
        Set(value As String)
            ViewState("PazMsg_Descr") = value
        End Set
    End Property

    Public Property CentroVaccinaleCorrente() As String
        Get
            Return ViewState("PazMsg_CNSCorrente")
        End Get
        Set(value As String)
            ViewState("PazMsg_CNSCorrente") = value
        End Set
    End Property

    Public Property CentroVaccinalePaziente() As String
        Get
            Return ViewState("PazMsg_CNSPaz")
        End Get
        Set(value As String)
            ViewState("PazMsg_CNSPaz") = value
        End Set
    End Property

    Private WithEvents _warnDeceduto As GestionePazientiMessageItem
    Public ReadOnly Property WarnDeceduto() As GestionePazientiMessageItem
        Get
            Return _warnDeceduto
        End Get
    End Property

    Private _warnConsultorio As GestionePazientiMessageItem
    Public ReadOnly Property WarnConsultorio() As GestionePazientiMessageItem
        Get
            Return _warnConsultorio
        End Get
    End Property

    Private _warnCampiObbligatori As GestionePazientiMessageItem
    Public ReadOnly Property WarnCampiObbligatori() As GestionePazientiMessageItem
        Get
            Return _warnCampiObbligatori
        End Get
    End Property

    Private _warnCampiWarning As GestionePazientiMessageItem
    Public ReadOnly Property WarnCampiWarning() As GestionePazientiMessageItem
        Get
            Return _warnCampiWarning
        End Get
    End Property

    Private _statoConsenso As GestionePazientiMessageStatoConsenso
    Public ReadOnly Property StatoConsenso() As GestionePazientiMessageStatoConsenso
        Get
            Return _statoConsenso
        End Get
    End Property

    Private _warnAssistenzaScaduta As GestionePazientiMessageItem
    Public Property WarnAssistenzaScaduta() As GestionePazientiMessageItem
        Get
            Return _warnAssistenzaScaduta
        End Get
        Set(value As GestionePazientiMessageItem)
            _warnAssistenzaScaduta = value
        End Set
    End Property

    Private _warnCancellato As GestionePazientiMessageItem
    Public Property WarnCancellato() As GestionePazientiMessageItem
        Get
            Return _warnCancellato
        End Get
        Set(value As GestionePazientiMessageItem)
            _warnCancellato = value
        End Set
    End Property

    Private _console As OnVac.WarningMessage
    Public Property Console() As OnVac.WarningMessage
        Get
            Return _console
        End Get
        Set(value As OnVac.WarningMessage)
            _console = value
        End Set
    End Property

#End Region

#Region " Page Events "

    Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        InitMessages(True)

    End Sub

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
    End Sub

#End Region

#Region " Warning Events "

    Private Sub WarnDeceduto_OnVisibleChange(sender As Object, e As System.EventArgs)

        Me.OnVisibleChange(Me.tblPazDeceduto)

    End Sub

    Private Sub WarnConsultorio_OnVisibleChange(sender As Object, e As System.EventArgs)

        Me.OnVisibleChange(Me.tblCnsCorrente)

    End Sub

    Private Sub StatoConsenso_OnVisibleChange(sender As Object, e As System.EventArgs)

        Dim s As GestionePazientiMessageStatoConsenso = DirectCast(sender, GestionePazientiMessageStatoConsenso)

        Me.imgWarningStatoConsenso.ImageUrl = s.ImageUrl
        Me.imgWarningStatoConsenso.ToolTip = s.ToolTip
        Me.lblWarningStatoConsenso.Text = s.Text

        Me.OnVisibleChange(Me.tblStatoConsenso)

    End Sub

    Private Sub WarnCampiObligatori_OnVisibleChange(sender As Object, e As System.EventArgs)

        Me.lblCampiNoSet.Text = DirectCast(sender, GestionePazientiMessageItem).Text

        Me.OnVisibleChange(Me.tblCampiObbligatori)

    End Sub

    Private Sub WarnCampiWarning_OnVisibleChange(sender As Object, e As System.EventArgs)

        Me.lblCampiWarningNoSet.Text = DirectCast(sender, GestionePazientiMessageItem).Text

        Me.OnVisibleChange(Me.tblCampiWarning)

    End Sub

    Private Sub WarnAssistenzaScaduta_OnVisibleChange(sender As Object, e As System.EventArgs)

        Me.OnVisibleChange(Me.tblAssistenzaScaduta)

    End Sub

    Private Sub WarnCancellato_OnVisibleChange(sender As Object, e As System.EventArgs)

        Me.OnVisibleChange(Me.tblPazCancellato)

    End Sub

    Private Sub OnVisibleChange(htmlTable As HtmlTable)

        htmlTable.Style.Item("display") = "block"

        Me.fmWarningMessage.VisibileMD = True

    End Sub

#End Region

#Region " Public "

    Public Sub ClearWarning()

        InitMessages(False)

    End Sub

#End Region

#Region " Private "

    Private Sub InitMessages(initConsole As Boolean)

        _warnDeceduto = New GestionePazientiMessageItem()
        _warnConsultorio = New GestionePazientiMessageItem()
        _statoConsenso = New GestionePazientiMessageStatoConsenso()
        _warnCampiObbligatori = New GestionePazientiMessageItem()
        _warnCampiWarning = New GestionePazientiMessageItem()
        _warnAssistenzaScaduta = New GestionePazientiMessageItem()
        _warnCancellato = New GestionePazientiMessageItem()
        If initConsole Then _console = New OnVac.WarningMessage(Me.lblConsole)

        AddHandler WarnDeceduto.OnVisibleChange, AddressOf WarnDeceduto_OnVisibleChange
        AddHandler WarnConsultorio.OnVisibleChange, AddressOf WarnConsultorio_OnVisibleChange
        AddHandler StatoConsenso.OnVisibleChange, AddressOf StatoConsenso_OnVisibleChange
        AddHandler WarnCampiObbligatori.OnVisibleChange, AddressOf WarnCampiObligatori_OnVisibleChange
        AddHandler WarnCampiWarning.OnVisibleChange, AddressOf WarnCampiWarning_OnVisibleChange
        AddHandler WarnAssistenzaScaduta.OnVisibleChange, AddressOf WarnAssistenzaScaduta_OnVisibleChange
        AddHandler WarnCancellato.OnVisibleChange, AddressOf WarnCancellato_OnVisibleChange

        Me.tblPazDeceduto.Style.Item("display") = "none"
        Me.tblCnsCorrente.Style.Item("display") = "none"
        Me.tblStatoConsenso.Style.Item("display") = "none"
        Me.tblCampiObbligatori.Style.Item("display") = "none"
        Me.tblCampiWarning.Style.Item("display") = "none"
        Me.tblAssistenzaScaduta.Style.Item("display") = "none"
        Me.tblPazCancellato.Style.Item("display") = "none"

        Me.fmWarningMessage.VisibileMD = False

    End Sub

#End Region

#Region " Classes "

    Public Class GestionePazientiMessageItem

        Public Event OnVisibleChange(sender As Object, e As EventArgs)

        Private _visible As Boolean
        Public Property Visible() As Boolean
            Get
                Return _visible
            End Get
            Set(value As Boolean)
                _visible = value
                RaiseEvent OnVisibleChange(Me, Nothing)
            End Set
        End Property

        Private _text As String
        Public Property Text() As String
            Get
                Return _text
            End Get
            Set(value As String)
                _text = value
            End Set
        End Property

        Public Sub New()
            Me.Visible = False
            Me.Text = String.Empty
        End Sub

    End Class

    Public Class GestionePazientiMessageStatoConsenso
        Inherits GestionePazientiMessageItem

        Public ImageUrl As String
        Public ToolTip As String

    End Class

#End Region

End Class