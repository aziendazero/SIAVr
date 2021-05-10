Imports System.Collections.Generic

Partial Public Class FiltriRicercaMagazzino
    Inherits Common.UserControlPageBase

    ' N.B. : questo usercontrol non eredita da OnVac.Common.UserControlPageBase perchè non utilizza i Settings

#Region " Properties "
    Public Property ShowFiltroDistretti As Boolean
        Get
            If ViewState("ShowFiltroDistretti") Is Nothing Then ViewState("ShowFiltroDistretti") = False
            Return ViewState("ShowFiltroDistretti")
        End Get
        Set(value As Boolean)
            ViewState("ShowFiltroDistretti") = value
        End Set
    End Property
    Private Property UltimoFiltroCodiceLotto As String
        Get
            If ViewState("UltimoFiltroCodiceLotto_FiltriMagazzino") Is Nothing Then ViewState("UltimoFiltroCodiceLotto_FiltriMagazzino") = String.Empty
            Return ViewState("UltimoFiltroCodiceLotto_FiltriMagazzino")
        End Get
        Set(value As String)
            ViewState("UltimoFiltroCodiceLotto_FiltriMagazzino") = value
        End Set
    End Property

    Private Property UltimoFiltroDescrizioneLotto As String
        Get
            If ViewState("UltimoFiltroDescrizioneLotto_FiltriMagazzino") Is Nothing Then ViewState("UltimoFiltroDescrizioneLotto_FiltriMagazzino") = String.Empty
            Return ViewState("UltimoFiltroDescrizioneLotto_FiltriMagazzino")
        End Get
        Set(value As String)
            ViewState("UltimoFiltroDescrizioneLotto_FiltriMagazzino") = value
        End Set
    End Property

    Private Property UltimoFiltroCodiceNomeCommerciale As String
        Get
            If ViewState("UltimoFiltroCodiceNomeCommerciale_FiltriMagazzino") Is Nothing Then ViewState("UltimoFiltroCodiceNomeCommerciale_FiltriMagazzino") = String.Empty
            Return ViewState("UltimoFiltroCodiceNomeCommerciale_FiltriMagazzino")
        End Get
        Set(value As String)
            ViewState("UltimoFiltroCodiceNomeCommerciale_FiltriMagazzino") = value
        End Set
    End Property

    Private Property UltimoFiltroDescrizioneNomeCommerciale As String
        Get
            If ViewState("UltimoFiltroDescrizioneNomeCommerciale_FiltriMagazzino") Is Nothing Then ViewState("UltimoFiltroDescrizioneNomeCommerciale_FiltriMagazzino") = String.Empty
            Return ViewState("UltimoFiltroDescrizioneNomeCommerciale_FiltriMagazzino")
        End Get
        Set(value As String)
            ViewState("UltimoFiltroDescrizioneNomeCommerciale_FiltriMagazzino") = value
        End Set
    End Property

    ' Il flag dei lotti scaduti è spuntato di defult, quindi questa property di default deve valere true anzichè false come le altre
    Private Property UltimoFiltroLottiScaduti As Boolean
        Get
            If ViewState("UltimoFiltroLottiScaduti_FiltriMagazzino") Is Nothing Then ViewState("UltimoFiltroLottiScaduti_FiltriMagazzino") = True
            Return ViewState("UltimoFiltroLottiScaduti_FiltriMagazzino")
        End Get
        Set(value As Boolean)
            ViewState("UltimoFiltroLottiScaduti_FiltriMagazzino") = value
        End Set
    End Property

    Private Property UltimoFiltroLottiSequestrati As Boolean
        Get
            If ViewState("UltimoFiltroLottiSequestrati_FiltriMagazzino") Is Nothing Then ViewState("UltimoFiltroLottiSequestrati_FiltriMagazzino") = False
            Return ViewState("UltimoFiltroLottiSequestrati_FiltriMagazzino")
        End Get
        Set(value As Boolean)
            ViewState("UltimoFiltroLottiSequestrati_FiltriMagazzino") = value
        End Set
    End Property

    Private Property UltimoFiltroLottiScortaNulla As Boolean
        Get
            If ViewState("UltimoFiltroLottiScortaNulla_FiltriMagazzino") Is Nothing Then ViewState("UltimoFiltroLottiScortaNulla_FiltriMagazzino") = False
            Return ViewState("UltimoFiltroLottiScortaNulla_FiltriMagazzino")
        End Get
        Set(value As Boolean)
            ViewState("UltimoFiltroLottiScortaNulla_FiltriMagazzino") = value
        End Set
    End Property
    Private Property UltimoFiltroCodiceDistretto As String
        Get
            If ViewState("UltimoFiltroCodiceDistretto_FiltriMagazzino") Is Nothing Then ViewState("UltimoFiltroCodiceDistretto_FiltriMagazzino") = String.Empty
            Return ViewState("UltimoFiltroCodiceDistretto_FiltriMagazzino")
        End Get
        Set(value As String)
            ViewState("UltimoFiltroCodiceDistretto_FiltriMagazzino") = value
        End Set
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then

            lblDistrietti.Visible = ShowFiltroDistretti
            ddlDistretti.Visible = ShowFiltroDistretti
            LoadDistretto(OnVacContext.UserId, UltimoFiltroCodiceDistretto)

        End If
    End Sub

#End Region

#Region " Public Methods "

    Public Sub SetFiltri(filtroLottiMagazzino As Filters.FiltriRicercaLottiMagazzino)

        Me.txtCodiceLotto.Text = filtroLottiMagazzino.CodiceLotto
        Me.txtDescrizioneLotto.Text = filtroLottiMagazzino.DescrizioneLotto
        Me.txtCodiceNomeCommerciale.Text = filtroLottiMagazzino.CodiceNomeCommerciale
        Me.txtDescrizioneNomeCommerciale.Text = filtroLottiMagazzino.DescrizioneNomeCommerciale
        Me.chkLottiScaduti.Checked = filtroLottiMagazzino.NoLottiScaduti
        Me.chkLottiSequestrati.Checked = filtroLottiMagazzino.SoloLottiSequestrati
        Me.chkLottiScortaNulla.Checked = filtroLottiMagazzino.NoLottiScortaNulla
        LoadDistretto(OnVacContext.UserId, filtroLottiMagazzino.CodiceDistretto)


    End Sub

    Public Sub Clear()

        Me.txtCodiceLotto.Text = String.Empty
        Me.txtDescrizioneLotto.Text = String.Empty
        Me.txtCodiceNomeCommerciale.Text = String.Empty
        Me.txtDescrizioneNomeCommerciale.Text = String.Empty
        Me.chkLottiScaduti.Checked = False
        Me.chkLottiSequestrati.Checked = False
        Me.chkLottiScortaNulla.Checked = False
        LoadDistretto(OnVacContext.UserId, String.Empty)

        Me.ResetUltimiFiltriUtilizzati()

    End Sub

    Public Sub SetFiltroLottiScaduti(checked As Boolean)

        Me.chkLottiScaduti.Checked = checked

    End Sub

    ''' <summary>
    ''' Restituisce una struttura contenente i valori attuali dei filtri, impostati dall'utente.
    ''' Tali valori vengono aggiornati anche nei campi che mantengono gli ultimi filtri utilizzati, se il parametro è true.
    ''' </summary>
    Public Function GetFiltriCorrenti(setUltimiFiltri As Boolean) As Filters.FiltriRicercaLottiMagazzino

        If setUltimiFiltri Then Me.SetUltimiFiltriUtilizzati()

        Return New Filters.FiltriRicercaLottiMagazzino(Me.txtCodiceLotto.Text,
                                                       Me.txtDescrizioneLotto.Text,
                                                       Me.txtCodiceNomeCommerciale.Text,
                                                       Me.txtDescrizioneNomeCommerciale.Text,
                                                       Me.chkLottiScaduti.Checked,
                                                       Me.chkLottiSequestrati.Checked,
                                                       Me.chkLottiScortaNulla.Checked, ddlDistretti.SelectedValue.ToString())

    End Function

    ''' <summary>
    ''' Restituisce una struttura contenente gli ultimi valori dei filtri utilizzati (gli ultimi restituiti da GetFiltriCorrenti), 
    ''' non quelli attualmente impostati dall'utente che potrebbero essere diversi.
    ''' </summary>
    Public Function GetUltimiFiltriUtilizzati() As Filters.FiltriRicercaLottiMagazzino

        Return New Filters.FiltriRicercaLottiMagazzino(Me.UltimoFiltroCodiceLotto,
                                                       Me.UltimoFiltroDescrizioneLotto,
                                                       Me.UltimoFiltroCodiceNomeCommerciale,
                                                       Me.UltimoFiltroDescrizioneNomeCommerciale,
                                                       Me.UltimoFiltroLottiScaduti,
                                                       Me.UltimoFiltroLottiSequestrati,
                                                       Me.UltimoFiltroLottiScortaNulla, UltimoFiltroCodiceDistretto)

    End Function

#End Region

#Region " Private Methods "

    Private Sub SetUltimiFiltriUtilizzati()

        Me.UltimoFiltroCodiceLotto = Me.txtCodiceLotto.Text
        Me.UltimoFiltroDescrizioneLotto = Me.txtDescrizioneLotto.Text
        Me.UltimoFiltroCodiceNomeCommerciale = Me.txtCodiceNomeCommerciale.Text
        Me.UltimoFiltroDescrizioneNomeCommerciale = Me.txtDescrizioneNomeCommerciale.Text
        Me.UltimoFiltroLottiScaduti = Me.chkLottiScaduti.Checked
        Me.UltimoFiltroLottiSequestrati = Me.chkLottiSequestrati.Checked
        Me.UltimoFiltroLottiScortaNulla = Me.chkLottiScortaNulla.Checked
        UltimoFiltroCodiceDistretto = ddlDistretti.SelectedValue.ToString()

    End Sub

    Private Sub ResetUltimiFiltriUtilizzati()

        Me.UltimoFiltroCodiceLotto = String.Empty
        Me.UltimoFiltroDescrizioneLotto = String.Empty
        Me.UltimoFiltroCodiceNomeCommerciale = String.Empty
        Me.UltimoFiltroDescrizioneNomeCommerciale = String.Empty
        Me.UltimoFiltroLottiScaduti = True          ' di default è true
        Me.UltimoFiltroLottiSequestrati = False
        Me.UltimoFiltroLottiScortaNulla = False
        UltimoFiltroCodiceDistretto = String.Empty

    End Sub

    Private Sub LoadDistretto(idUtente As Long, codSel As String)
        ddlDistretti.Items.Clear()
        ddlDistretti.ClearSelection()

        Dim listaDistr As List(Of Entities.DistrettoDDL) = Nothing
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAnagrafica As New Biz.BizAnagrafiche(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                listaDistr = bizAnagrafica.GetListAnagraficaDistrettiCNSUtente(idUtente)
            End Using
        End Using
        ddlDistretti.DataTextField = "Descrizione"
        ddlDistretti.DataValueField = "Codice"

        If listaDistr.Count > 0 Then
            listaDistr = listaDistr.OrderBy(Function(p) p.Descrizione).ToList()
            listaDistr.Insert(0, New Entities.DistrettoDDL() With {.Codice = String.Empty, .Descrizione = "TUTTI"})
            ddlDistretti.SelectedValue = codSel
        End If
        ddlDistretti.DataSource = listaDistr
        ddlDistretti.DataBind()


    End Sub

#End Region

End Class