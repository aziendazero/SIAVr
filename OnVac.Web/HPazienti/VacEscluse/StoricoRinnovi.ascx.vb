Imports System.Collections.Generic

Public Class StoricoRinnovi
    Inherits Common.UserControlFinestraModalePageBase

#Region " Types "

    Private Enum dg_VacEscStoricoColumnIndex
        ImgRinnovo = 0
        IdHidden = 1
        CodiceVaccinazioneHidden = 2
        DescrizioneVaccinazione = 3
        DataVisita = 4
        DataScadenza = 5
        DataRegistrazione = 6
        DescrizioneOperatore = 7
        UtenteRegistrazione = 8
        UtenteModifica = 9
        DataEliminazione = 10
        StatoEliminazione = 11
        UtenteEliminazione = 12

    End Enum

    <Serializable>
    Public Class ResultItem
        Public VaccinazioneCodice As String
        Public VaccinazioneDescrizione As String
        Public DataVisita As DateTime
        Public MotivoCodice As String
        Public MotivoDescrizione As String
        Public MedicoCodice As String
        Public MedicoNome As String
        Public DataScadenza As DateTime
    End Class

    Private Class ConfermaResult

        Private _success As Boolean
        Public Property Success() As Boolean
            Get
                Return _success
            End Get
            Private Set(value As Boolean)
                _success = value
            End Set
        End Property

        Private _resultList As List(Of ResultItem)
        Public Property ResultList() As List(Of ResultItem)
            Get
                Return _resultList
            End Get
            Private Set(value As List(Of ResultItem))
                _resultList = value
            End Set
        End Property

        Private _errorMessage As String
        Public Property ErrorMessage() As String
            Get
                Return _errorMessage
            End Get
            Set(value As String)
                _errorMessage = value
            End Set
        End Property

        Public Sub New(errorMessage As String)
            Me.Success = False
            Me.ResultList = Nothing
            Me.ErrorMessage = errorMessage
        End Sub

        Public Sub New(resultList As List(Of ResultItem))
            Me.Success = True
            Me.ResultList = resultList
            Me.ErrorMessage = String.Empty
        End Sub

    End Class

    Private Property ColonnaOrdinamentoDatagrid_Vxe() As String
        Get
            If ViewState("ColonnaOrdinamentoDatagrid_Vxe") Is Nothing Then ViewState("ColonnaOrdinamentoDatagrid_Vxe") = String.Empty
            Return ViewState("ColonnaOrdinamentoDatagrid_Vxe")
        End Get
        Set(value As String)
            ViewState("ColonnaOrdinamentoDatagrid_Vxe") = value
        End Set
    End Property

    Private Property CampoOrdinamentoDatagrid_Vxe() As String
        Get
            If ViewState("CampoOrdinamentoDatagrid_Vxe") Is Nothing Then ViewState("CampoOrdinamentoDatagrid_Vxe") = String.Empty
            Return ViewState("CampoOrdinamentoDatagrid_Vxe")
        End Get
        Set(value As String)
            ViewState("CampoOrdinamentoDatagrid_Vxe") = value
        End Set
    End Property

    Private Property VersoOrdinamentoDatagrid_Vxe() As String
        Get
            If ViewState("VersoOrdinamentoDatagrid_Vxe") Is Nothing Then ViewState("VersoOrdinamentoDatagrid_Vxe") = "ASC"
            Return ViewState("VersoOrdinamentoDatagrid_Vxe")
        End Get
        Set(value As String)
            ViewState("VersoOrdinamentoDatagrid_Vxe") = value
        End Set
    End Property

    Public Property SoloRinnovati() As Boolean
        Get
            If ViewState("SoloRinnovati") Is Nothing Then ViewState("SoloRinnovati") = False

            Return DirectCast(ViewState("SoloRinnovati"), Boolean)
        End Get
        Set(value As Boolean)
            ViewState("SoloRinnovati") = value
        End Set
    End Property

#End Region

#Region " Events "

    Public Event Conferma(lst As List(Of ResultItem))
    Public Event RiabilitaLayout()

    'scatena l'evento per riabilitare il layout
    Protected Sub OnRiabilitaLayout()
        RaiseEvent RiabilitaLayout()
    End Sub

    'invio codici vaccinazioni selezionate alla pagina principale
    Protected Sub OnConferma(resultList As List(Of ResultItem))
        RaiseEvent Conferma(resultList)
    End Sub

#End Region

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

#Region " Page Events "

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If Not IsPostBack() Then

            VisibilitaMaschera(Not SoloRinnovati)

        End If

    End Sub

    'Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

    '    SetImmagineOrdinamento()

    'End Sub

#End Region

#Region " Public "

    Public Sub Inizializza(codiceVaccinazione As String)

        ' Caricamento vaccinazioni da proporre per l'esclusione
        Dim vaccinazioniEscluseStorico As List(Of Entities.VaccinazioneEsclusaDettaglio) = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizEscluse As New Biz.BizVaccinazioniEscluse(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                vaccinazioniEscluseStorico = bizEscluse.GetVaccinazioniEscluseEliminateByPazienteVaccinazione(OnVacUtility.Variabili.PazId, codiceVaccinazione, String.Empty, SoloRinnovati)

            End Using
        End Using

        If Not vaccinazioniEscluseStorico Is Nothing Then
            dg_VacEscStorico.DataSource = vaccinazioniEscluseStorico
            dg_VacEscStorico.DataBind()
        End If

    End Sub

#End Region

#Region " Control Events "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btn_Chiudi"

                OnRiabilitaLayout()

        End Select

    End Sub

#End Region

#Region " Protected "

    Protected Function ConvertToDateString(data As Object, includeHours As Boolean) As String

        If data Is Nothing Then
            Return String.Empty
        End If

        If includeHours Then
            Return Convert.ToDateTime(data).ToString("dd/MM/yyyy HH:mm")
        End If

        Return Convert.ToDateTime(data).ToString("dd/MM/yyyy")

    End Function

    Protected Function IsVisibleFlagRinnovo(statoEliminazione As Object) As String

        If statoEliminazione Is Nothing OrElse statoEliminazione Is DBNull.Value Then
            Return Boolean.FalseString
        End If

        If statoEliminazione.ToString() = Constants.StatoVaccinazioniEscluseEliminate.Rinnovata Then
            Return Boolean.TrueString
        End If

        Return Boolean.FalseString

    End Function

    Private Sub dg_VacEscStorico_SortCommand(source As Object, e As DataGridSortCommandEventArgs) Handles dg_VacEscStorico.SortCommand

        Dim codiceVaccinazione As String = String.Empty

        Dim vaccinazioniEscluseStorico As List(Of Entities.VaccinazioneEsclusaDettaglio) = Nothing

        Dim imageUrlArrowDown As String = ResolveClientUrl("~/Images/arrow_down_small.gif")
        Dim imageUrlArrowUp As String = ResolveClientUrl("~/Images/arrow_up_small.gif")

        Dim campoOrdinamento As String = SetCampiOrdinamentoDatagrid(e.SortExpression)

        If SoloRinnovati Then
            codiceVaccinazione = DirectCast(dg_VacEscStorico.Items(0).FindControl("CodiceVaccinazioneHidden"), HiddenField).Value
        End If

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizEscluse As New Biz.BizVaccinazioniEscluse(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                vaccinazioniEscluseStorico = bizEscluse.GetVaccinazioniEscluseEliminateByPazienteVaccinazione(OnVacUtility.Variabili.PazId, codiceVaccinazione, campoOrdinamento, SoloRinnovati)

            End Using
        End Using

        If Not vaccinazioniEscluseStorico Is Nothing Then
            dg_VacEscStorico.DataSource = vaccinazioniEscluseStorico
            dg_VacEscStorico.DataBind()
        End If

    End Sub

    ' Impostazione campi specificati per l'ordinamento del datagrid.
    Private Function SetCampiOrdinamentoDatagrid(sortExpression As String) As String

        Dim idArrowSort As String = String.Empty

        ' Impostazione Colonna e verso dell'ordinamento
        If ColonnaOrdinamentoDatagrid_Vxe = sortExpression Then
            If VersoOrdinamentoDatagrid_Vxe = "ASC" Then
                VersoOrdinamentoDatagrid_Vxe = "DESC"
            Else
                VersoOrdinamentoDatagrid_Vxe = "ASC"
            End If
        Else
            VersoOrdinamentoDatagrid_Vxe = "ASC"
        End If

        Select Case sortExpression
            Case "DescrizioneVaccinazione"
                CampoOrdinamentoDatagrid_Vxe = "vac_descrizione"
                idArrowSort = "imgVac"
            Case "DataVisita"
                CampoOrdinamentoDatagrid_Vxe = "vxe_data_visita"
                idArrowSort = "imgDataVis"
            Case "DataScadenza"
                CampoOrdinamentoDatagrid_Vxe = "vxe_data_scadenza"
                idArrowSort = "imgDataScad"
            Case "DataRegistrazione"
                CampoOrdinamentoDatagrid_Vxe = "vxe_data_registrazione"
                idArrowSort = "imgDataReg"
            Case "DataEliminazione"
                CampoOrdinamentoDatagrid_Vxe = "vxe_data_eliminazione"
                idArrowSort = "imgDataEli"
            Case "DescrizioneOperatore"
                CampoOrdinamentoDatagrid_Vxe = "ope_nome"
                idArrowSort = "imgOpe"
            Case "UtenteEliminazione"
                CampoOrdinamentoDatagrid_Vxe = "ute_eliminazione"
                idArrowSort = "imgUteEli"
        End Select

        ' Setto l'immagine dell'ordinamento
        SetImmagineOrdinamento(idArrowSort)

        ColonnaOrdinamentoDatagrid_Vxe = sortExpression

        'Me.CampoOrdinamentoDatagrid_Vxe = sortExpression

        Return String.Format(" {0} {1} ", CampoOrdinamentoDatagrid_Vxe, VersoOrdinamentoDatagrid_Vxe)

    End Function

    Private Sub VisibilitaMaschera(visibile As Boolean)

        ToolBar.Visible = Not visibile

        divLegenda.Visible = visibile

        dg_VacEscStorico.Columns(dg_VacEscStoricoColumnIndex.ImgRinnovo).Visible = visibile

    End Sub

    Private Sub SetImmagineOrdinamento(id As String)

        Dim imageUrl As String = String.Empty

        If VersoOrdinamentoDatagrid_Vxe = Constants.VersoOrdinamento.Crescente Then
            imageUrl = ResolveClientUrl("~/Images/arrow_up_small.gif")
        Else
            imageUrl = ResolveClientUrl("~/Images/arrow_down_small.gif")
        End If

        Page.ClientScript.RegisterStartupScript(Me.GetType(), "ord_img", String.Format("<script type='text/javascript'>ImpostaImmagineOrdinamento('{0}', '{1}');</script>", id, imageUrl))

    End Sub

#End Region

End Class