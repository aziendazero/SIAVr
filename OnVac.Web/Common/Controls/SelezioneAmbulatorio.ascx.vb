Imports System.ComponentModel

Imports Onit.Database.DataAccessManager
Imports Onit.OnAssistnet.OnVac.DAL


Namespace Common.Controls

    Partial Class SelezioneAmbulatorio
        Inherits System.Web.UI.UserControl

#Region " Private var "

        Private _cnsCodice As String
        Private _cnsDescrizione As String
        Private _ambCodice As Integer
        Private _ambDescrizione As String
        Private _Tutti As Boolean
        Private _TuttiCns As Boolean
        Private _SceltaConsultorio As Boolean = True
        Private _MostraLabel As Boolean = True
        Private _MostraCodici As Boolean = True
        Private _MostraPulsanteClean As Boolean = False

#End Region

#Region " Codice generato da Progettazione Web Form "

        ' Oggetti finestra principale
        Protected WithEvents txtChild_Desc As System.Web.UI.WebControls.TextBox

        ' Oggetti finestra modale
        Protected WithEvents fmSelect As Onit.Controls.OnitFinestraModale

        'Chiamata richiesta da Progettazione Web Form.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
        'Non spostarla o rimuoverla.
        Private designerPlaceholderDeclaration As System.Object

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
            'Non modificarla nell'editor del codice.
            InitializeComponent()
        End Sub

#End Region

#Region " Proprietà "

        Public Property CnsCodicePrecedente() As String
            Get
                Return ViewState("CnsCodicePrecedente")
            End Get
            Set(Value As String)
                ViewState("CnsCodicePrecedente") = Value
            End Set
        End Property

        Public Property cnsCodice() As String
            Get
                Return ViewState("cnsCodice")
            End Get
            Set(Value As String)
                ViewState("cnsCodice") = Value
            End Set
        End Property

        Public Property cnsDescrizione() As String
            Get
                Return ViewState("cnsDescrizione")
            End Get
            Set(Value As String)
                ViewState("cnsDescrizione") = Value
            End Set
        End Property

        Public Property ambCodice() As Integer
            Get
                Return ViewState("ambCodice")
            End Get
            Set(Value As Integer)
                ViewState("ambCodice") = Value
            End Set
        End Property

        Public Property ambDescrizione() As String
            Get
                Return ViewState("ambDescrizione")
            End Get
            Set(Value As String)
                ViewState("ambDescrizione") = Value
            End Set
        End Property

        Public Property Tutti() As Boolean
            Get
                Return _Tutti
            End Get
            Set(Value As Boolean)
                _Tutti = Value
            End Set
        End Property

        Public Property TuttiCns() As Boolean
            Get
                Return _TuttiCns
            End Get
            Set(Value As Boolean)
                _TuttiCns = Value
            End Set
        End Property

        <DefaultValue(True)>
        Public Property SceltaConsultorio() As Boolean
            Get
                Return _SceltaConsultorio
            End Get
            Set(Value As Boolean)
                _SceltaConsultorio = Value
            End Set
        End Property

        <DefaultValue(True)>
        Public Property MostraLabel As Boolean
            Get
                Return _MostraLabel
            End Get
            Set(Value As Boolean)
                _MostraLabel = Value
            End Set
        End Property

        <DefaultValue(True)>
        Public Property MostraCodici As Boolean
            Get
                Return _MostraCodici
            End Get
            Set(Value As Boolean)
                _MostraCodici = Value
            End Set
        End Property

        <DefaultValue(False)>
        Public Property MostraPulsanteClean As Boolean
            Get
                Return _MostraPulsanteClean
            End Get
            Set(Value As Boolean)
                _MostraPulsanteClean = Value
            End Set
        End Property

        Private Property DataInizioPeriodo As DateTime
            Get
                If ViewState("inizioPeriodo") Is Nothing Then ViewState("inizioPeriodo") = DateTime.Now.Date
                Return Convert.ToDateTime(ViewState("inizioPeriodo"))
            End Get
            Set(value As DateTime)
                ViewState("inizioPeriodo") = value
            End Set
        End Property

        Public Property DataFinePeriodo As DateTime
            Get
                If ViewState("finePeriodo") Is Nothing Then ViewState("finePeriodo") = DateTime.Now.Date
                Return Convert.ToDateTime(ViewState("finePeriodo"))
            End Get
            Set(value As DateTime)
                ViewState("finePeriodo") = value
            End Set
        End Property

#End Region

#Region " Eventi "

        Public Event AmbulatorioCambiato(cnsCodice As String, ambCodice As Integer)
        Public Event GetDatePeriodoValidita(ByRef dataInizio As DateTime?, ByRef dataFine As DateTime?)

#End Region

#Region " Page Events "

        Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

            ' Di default, il pulsante di cancellazione non è visualizzato
            Me.btnClean.Visible = Me.MostraPulsanteClean

            ' Se MostraLabel = False:
            ' nasconde le label "Centro Vaccinale" e "Ambulatorio", adatta le larghezze dei td, allinea a sinistra
            If Not Me.MostraLabel Then

                Me.tdLabelCentroVaccinale.InnerHtml = String.Empty

                Me.tdLabelCentroVaccinale.Width = "0px"
                Me.tdValoreCentroVaccinale.Width = "98%"

                Me.tdValoreCentroVaccinale.Align = "Left"
                Me.txtParent_Desc.Style.Item("Align") = "Left"
                Me.txtParent_Cod.Style.Item("Align") = "Left"

                Me.tdLabelAmbulatorio.InnerHtml = String.Empty

                Me.tdLabelAmbulatorio.Width = "0px"
                Me.tdValoreAmbulatorio.Width = "98%"

                Me.tdValoreAmbulatorio.Align = "Left"
                Me.txtChild_Desc.Style.Item("Align") = "Left"
                Me.txtChild_Cod.Style.Item("Align") = "Left"

            End If

            ' Se MostraCodici = False: 
            ' nasconde i campi "Codice" per consultorio e ambulatorio, adatta le larghezze dei campi codice e descrizione
            If Not Me.MostraCodici Then

                Me.txtParent_Cod.Visible = False

                Me.txtParent_Cod.Width = Unit.Pixel(0)
                Me.txtParent_Desc.Width = Unit.Percentage(99)

                Me.txtChild_Cod.Visible = False

                Me.txtChild_Cod.Width = Unit.Pixel(0)
                Me.txtChild_Desc.Width = Unit.Percentage(99)

            End If

            If Me.TuttiCns Then Me.alsConsultorio.Tutti = True
            If Not Me.Tutti Then Me.alsAmbulatorio.Tutti = False

            SetDatePeriodoValidita()

            Select Case Request.Form.Item("__EVENTTARGET")

                Case "selected"

                    Dim oldAmbCodice As String = Me.ambCodice

                    Dim evtArgs As String() = Request.Form.Item("__EVENTARGUMENT").Split("|")

                    Dim label As String = evtArgs(0)
                    Dim codice As String = evtArgs(1)
                    Dim descrizione As String = evtArgs(2)

                    Select Case label

                        Case Me.alsConsultorio.Label

                            Me.cnsCodice = codice
                            Me.cnsDescrizione = descrizione

                            Me.alsAmbulatorio.Filtro &= GetQueryFilter(codice)

                            Me.alsAmbulatorio.Codice = String.Empty
                            Me.alsAmbulatorio.Descrizione = String.Empty
                            Me.alsAmbulatorio.databind()

                            Me.lblCnsSelezionato.Text = GetMessage()

                        Case alsAmbulatorio.Label

                            Me.ambCodice = codice
                            Me.ambDescrizione = descrizione

                            Me.txtParent_Desc.Text = Me.cnsDescrizione
                            Me.txtParent_Cod.Text = Me.cnsCodice
                            Me.txtChild_Desc.Text = alsAmbulatorio.Descrizione
                            Me.txtChild_Cod.Text = alsAmbulatorio.Codice

                            Me.fmSelect.VisibileMD = False

                            If codice <> oldAmbCodice OrElse Me.cnsCodice <> Me.CnsCodicePrecedente Then
                                RaiseEvent AmbulatorioCambiato(Me.cnsCodice, codice)
                            End If

                    End Select

                Case Else

                    Dim bindingCode As String = String.Empty

                    If Not Me.SceltaConsultorio Then
                        Me.alsConsultorio.Visible = False
                        bindingCode = Me.cnsCodice
                    Else
                        Me.alsConsultorio.Visible = True
                        bindingCode = Me.alsConsultorio.Codice
                    End If

                    Me.alsAmbulatorio.Filtro &= GetQueryFilter(bindingCode)

            End Select

        End Sub

#End Region

#Region " Button Events "

        Protected Sub btnOpenFM_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnOpenFM.Click

            If e.X <> 0 OrElse e.Y <> 0 Then

                Me.alsAmbulatorio.Codice = String.Empty
                Me.alsAmbulatorio.Descrizione = String.Empty
                Me.alsAmbulatorio.databind()

                Me.CnsCodicePrecedente = Me.txtParent_Cod.Text

                Me.lblCnsSelezionato.Text = GetMessage()

                Me.fmSelect.VisibileMD = True

            End If

        End Sub

        Private Sub btnClean_Click(sender As Object, e As ImageClickEventArgs) Handles btnClean.Click

            Clear()

        End Sub

#End Region

#Region " Public "

        Public Overrides Sub databind()

            Me.alsConsultorio.Codice = Me.cnsCodice
            Me.alsConsultorio.Descrizione = Me.cnsDescrizione

            If Me.ambCodice <> 0 Then

                Me.alsAmbulatorio.Codice = Me.ambCodice
                Me.alsAmbulatorio.Descrizione = Me.ambDescrizione

            End If

            Me.txtParent_Desc.Text = Me.cnsDescrizione
            Me.txtParent_Cod.Text = Me.cnsCodice
            Me.txtChild_Desc.Text = Me.ambDescrizione
            Me.txtChild_Cod.Text = Me.ambCodice

        End Sub

        Public Function IsValid() As Boolean

            If Not Me.alsAmbulatorio.Codice Is Nothing AndAlso Me.alsAmbulatorio.Codice <> "" Then
                Return True
            Else
                Return False
            End If

        End Function

        Public Sub SetEnabled(enable As Boolean)

            Me.btnOpenFM.Enabled = enable

            If enable Then
                Me.btnOpenFM.ImageUrl="~/Images/modifica.gif"
                Me.btnOpenFM.Style("cursor") = "pointer"
            Else
                Me.btnOpenFM.ImageUrl="~/Images/modifica_dis.gif"
                Me.btnOpenFM.Style("cursor") = "default"
            End If

        End Sub

        Public Sub OpenModale()

            Me.btnOpenFM_Click(Nothing, Nothing)

        End Sub

        Public Sub Clear()
            '--
            Me.CnsCodicePrecedente = String.Empty
            Me.cnsCodice = String.Empty
            Me.cnsDescrizione = String.Empty
            Me.ambCodice = Constants.AmbulatorioTUTTI.Codice
            Me.ambDescrizione = String.Empty
            '--
            Me.alsConsultorio.CampoCodice = String.Empty
            Me.alsConsultorio.CampoDescrizione = String.Empty
            Me.alsConsultorio.Codice = String.Empty
            Me.alsConsultorio.Descrizione = String.Empty
            Me.alsConsultorio.databind()
            '--
            Me.alsAmbulatorio.CampoCodice = String.Empty
            Me.alsAmbulatorio.CampoDescrizione = String.Empty
            Me.alsAmbulatorio.Codice = String.Empty
            Me.alsAmbulatorio.Descrizione = String.Empty
            Me.alsAmbulatorio.databind()
            '--
            Me.txtParent_Cod.Text = String.Empty
            Me.txtParent_Desc.Text = String.Empty
            Me.txtChild_Cod.Text = String.Empty
            Me.txtChild_Desc.Text = String.Empty
            '--
        End Sub

#End Region

#Region " Private "

        Private Sub SetDatePeriodoValidita()

            Dim dataInizio As DateTime? = Nothing
            Dim dataFine As DateTime? = Nothing

            RaiseEvent GetDatePeriodoValidita(dataInizio, dataFine)

            If dataInizio.HasValue Then
                Me.DataInizioPeriodo = dataInizio.Value
            Else
                Me.DataInizioPeriodo = DateTime.Now.Date
            End If

            If dataFine.HasValue Then
                Me.DataFinePeriodo = dataFine.Value
            Else
                Me.DataFinePeriodo = DateTime.Now.Date
            End If

        End Sub

        Private Function GetMessage() As String

            Dim message As New System.Text.StringBuilder()

            message.AppendFormat("Ambulatori del centro vaccinale: {0} [{1}]", Me.cnsDescrizione, Me.cnsCodice)

            If Me.DataInizioPeriodo <> DateTime.Now.Date OrElse Me.DataFinePeriodo <> DateTime.Now.Date Then
                message.AppendFormat(",<br/>aperti nel periodo: dal {0:dd/MM/yyyy} al {1:dd/MM/yyyy}", Me.DataInizioPeriodo, Me.DataFinePeriodo)
            Else
                message.Append(",<br/>aperti ad oggi")
            End If

            Return message.ToString()

        End Function

        Private Function GetQueryFilter(codiceConsultorio As String) As String

            Return String.Format("AMB_CNS_CODICE = '{0}' AND AMB_DATA_APERTURA <= to_date('{1}','dd/mm/yyyy')" &
                                 " AND (AMB_DATA_CHIUSURA >= to_date('{2}','dd/mm/yyyy') OR AMB_DATA_CHIUSURA IS NULL)",
                                 codiceConsultorio, Me.DataFinePeriodo.ToShortDateString(), Me.DataInizioPeriodo.ToShortDateString())

        End Function

#End Region

    End Class

End Namespace
