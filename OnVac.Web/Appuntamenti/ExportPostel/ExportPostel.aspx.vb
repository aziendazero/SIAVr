Imports System.Collections.Generic


Public Class ExportPostel
    Inherits Common.PageBase

#Region " Properties "

    Private Property FiltriMaschera() As FiltriAssociazioniDosi
        Get
            If ViewState("FCC") Is Nothing Then ViewState("FCC") = New FiltriAssociazioniDosi()
            Return ViewState("FCC")
        End Get
        Set(value As FiltriAssociazioniDosi)
            ViewState("FCC") = value
        End Set
    End Property

#End Region

#Region " Page "

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then

            OnVacUtility.BindTipoAvvisoPostel(Me.rblExportPostel, Me.Settings)
            OnVacUtility.BindTipoSoggettiAvviso(Me.rdbFiltroSoggetti)
            LoadDistretto()

            Me.CaricaDateUltimaStampa()

        End If

    End Sub

#End Region

#Region " Toolbar "

    Private Sub Toolbar_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles Toolbar.ButtonClicked
        '--
        Select Case be.Button.Key
            '--
            Case "btnExport"
                '--
                ' Export del tracciato + salvataggio data invio e date ultima stampa
                '--
                Dim result As Biz.BizGenericResult = ExportDati()
                '--
                If Not result.Success Then
                    Me.OnitLayout31.ShowMsgBox(New Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(result.Message, "ERR", False, False))
                    Return
                End If
                '--
                Me.CaricaDateUltimaStampa()
                '--
            Case "btnPulisci"
                '--
                Me.uscScegliAmb.Clear()
                '--
                Me.dpkDataAppInizio.Text = String.Empty
                Me.dpkDataAppFine.Text = String.Empty
                Me.dpkNascitaInizio.Text = String.Empty
                Me.dpkNascitaFine.Text = String.Empty
                '--
                Me.fmCittadinanza.Codice = String.Empty
                Me.fmCittadinanza.Descrizione = String.Empty
                Me.fmCittadinanza.RefreshDataBind()
                LoadDistretto()
                '--
                OnVacUtility.BindTipoAvvisoPostel(Me.rblExportPostel, Me.Settings)
                OnVacUtility.BindTipoSoggettiAvviso(Me.rdbFiltroSoggetti)
                '--
                Me.FiltriMaschera = New FiltriAssociazioniDosi()
                '--
        End Select
        '--
    End Sub

#End Region

#Region " User Controls "

#Region " Scelta Ambulatorio "

    Private Sub uscScegliAmb_Load(sender As Object, e As System.EventArgs) Handles uscScegliAmb.Load

        If Not IsPostBack Then

            Me.uscScegliAmb.Clear()

        End If

    End Sub

#End Region

#Region " Associazioni Dosi "

#Region " Types "

    <Serializable()>
    Public Class FiltriAssociazioniDosi

        Public Associazioni As List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro1)
        Public DosiAssociazioni As List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro2)

        Public Sub New()
            Associazioni = New List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro1)()
            DosiAssociazioni = New List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro2)()
        End Sub

    End Class

#End Region

    Private Sub btnImgAssociazioniDosi_Click(sender As Object, e As ImageClickEventArgs) Handles btnImgAssociazioniDosi.Click

        'Set degli eventuali filtri associazioni
        If FiltriMaschera.Associazioni.Count > 0 Then

            Dim associazioni As DataTable = FiltriMaschera.Associazioni.ConvertToDataTable()
            Me.UscFiltroAssociazioniDosi.setValoriSelezionatiFiltro1(associazioni)

        End If

        'Set degli eventuali filtri dosi associazioni
        If FiltriMaschera.DosiAssociazioni.Count > 0 Then

            Dim dosiAssociazioni = FiltriMaschera.DosiAssociazioni.ConvertToDataTable()
            Me.UscFiltroAssociazioniDosi.setValoriSelezionatiFiltro2(dosiAssociazioni)

        End If

        'Apertura della modale
        Me.fmFiltroAssociazioniDosi.VisibileMD = True

    End Sub

    Private Sub btnOk_FiltroAssociazioniDosi_Click(sender As Object, e As EventArgs) Handles btnOk_FiltroAssociazioniDosi.Click

        'Chiusura della modale
        Me.fmFiltroAssociazioniDosi.VisibileMD = False

        ' Aggiornamento dei filtri nel viewstate
        Dim dtAssociazioni As DataTable = Me.UscFiltroAssociazioniDosi.getValoriSelezionatiFiltro1()
        Me.FiltriMaschera.Associazioni = dtAssociazioni.ConvertToList(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro1)()
        Dim dtDosiAssociazioni As DataTable = Me.UscFiltroAssociazioniDosi.getValoriSelezionatiFiltro2()
        Me.FiltriMaschera.DosiAssociazioni = dtDosiAssociazioni.ConvertToList(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro2)()

        'Aggiornamento label
        Me.lblAssociazioniDosi.Text = Me.UscFiltroAssociazioniDosi.getStringaFormattata()

    End Sub

    Private Sub btnAnnulla_FiltroAssociazioniDosi_Click(sender As Object, e As EventArgs) Handles btnAnnulla_FiltroAssociazioniDosi.Click

        Me.fmFiltroAssociazioniDosi.VisibileMD = False

    End Sub

#End Region

#End Region

#Region " Private "

    Private Sub CaricaDateUltimaStampa()

        Dim dateInfoConsultorio As Entities.ConsultorioDateInfo = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizConsultori As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                dateInfoConsultorio = bizConsultori.GetDateInfoConsultorio(OnVacUtility.Variabili.CNS.Codice)

            End Using
        End Using

        If Not dateInfoConsultorio Is Nothing Then
            OnVacUtility.SetLabelCampoDate(Me.lblUltimoAvvisoDataDa, dateInfoConsultorio.DataUltimaStampaAvvisoInizio)
            OnVacUtility.SetLabelCampoDate(Me.lblUltimoAvvisoDataA, dateInfoConsultorio.DataUltimaStampaAvvisoFine)
        End If

    End Sub

    ''' <summary>
    ''' Legge il valore selezionato dal radiobutton dei pazienti avvisati e restituisce l'enumerazione corrispondente
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetFiltroPazientiAvvisati() As OnVac.Enumerators.FiltroAvvisati

        Return DirectCast(Convert.ToInt32(Me.rdbFiltroSoggetti.SelectedValue), Enumerators.FiltroAvvisati)

    End Function

    Private Function ExportDati() As Biz.BizGenericResult

        Dim result As New Biz.BizGenericResult()

        If String.IsNullOrEmpty(Me.dpkDataAppInizio.Text) Then
            result.Success = False
            result.Message = "Export non avviato: la data iniziale del periodo di appuntamento è obbligatoria."
            Return result
        End If

        If String.IsNullOrEmpty(Me.dpkDataAppFine.Text) Then
            result.Success = False
            result.Message = "Export non avviato: la data finale del periodo di appuntamento è obbligatoria."
            Return result
        End If

        If Me.dpkDataAppInizio.Data > Me.dpkDataAppFine.Data Then
            result.Success = False
            result.Message = "Export non avviato: la data iniziale del periodo di appuntamento non può superare la data finale."
            Return result
        End If

        If (String.IsNullOrEmpty(Me.uscScegliAmb.cnsDescrizione) AndAlso Not String.IsNullOrEmpty(Me.uscScegliAmb.ambDescrizione)) OrElse
           (Not String.IsNullOrEmpty(Me.uscScegliAmb.cnsDescrizione) AndAlso String.IsNullOrEmpty(Me.uscScegliAmb.ambDescrizione)) Then

            result.Success = False
            result.Message = "Export non avviato: selezionare un centro vaccinale ed un ambulatorio, oppure lasciarli vuoti entrambi."
            Return result

        End If

        If Not String.IsNullOrEmpty(Me.dpkNascitaInizio.Text) AndAlso
           Not String.IsNullOrEmpty(Me.dpkNascitaFine.Text) AndAlso
           Me.dpkNascitaInizio.Data > Me.dpkNascitaFine.Data Then

            result.Success = False
            result.Message = "Export non avviato: la data iniziale dell'intervallo di nascita non può superare la data finale."
            Return result

        End If

        ' Redirect all'handler per esportazione file postel
        Dim command As New OnVacUtility.OpenPostelHandlerCommand()
        command.CodiceConsultorio = Me.uscScegliAmb.cnsCodice
        command.CodiceAmbulatorio = Me.uscScegliAmb.ambCodice
        command.DataInizioAppuntamenti = Me.dpkDataAppInizio.Data
        command.DataFineAppuntamenti = Me.dpkDataAppFine.Data
        command.FiltroPazientiAvvisati = GetFiltroPazientiAvvisati()
        command.CodiceCittadinanza = Me.fmCittadinanza.Codice
        command.DataInizioNascita = Me.dpkNascitaInizio.Data
        command.DataFineNascita = Me.dpkNascitaFine.Data
        command.TipoAvviso = Me.rblExportPostel.SelectedValue
        command.Distretto = ddlDistretti.SelectedValue()
        command.ArgomentoExport = Me.Settings.EXPORT_POSTEL_ARGOMENTO
        command.CurrentPage = Me.Page

        Dim filtroAssociazioniDosi As Entities.FiltroComposto = Nothing

        If FiltriMaschera.Associazioni.Count > 0 OrElse FiltriMaschera.DosiAssociazioni.Count > 0 Then

            Dim associazioniSelezionate As String = UscFiltroAssociazioniDosi.getStringaFiltro1("|")
            Dim dosiSelezionate As String = UscFiltroAssociazioniDosi.getStringaFiltro2("|")

            filtroAssociazioniDosi = New Entities.FiltroComposto()

            'Associazioni
            filtroAssociazioniDosi.CodiceValore =
                FiltriMaschera.Associazioni.Select(Function(ass) New KeyValuePair(Of String, String)(ass.Codice, ass.Valore)).ToList()

            'Dosi
            Dim dosi As List(Of String) = FiltriMaschera.DosiAssociazioni.Select(Function(dos) dos.Codice).ToList()
            If Not dosi.IsNullOrEmpty() Then
                filtroAssociazioniDosi.Valori = dosi.Select(Function(dos) Integer.Parse(dos)).ToList()
            End If

        End If

        command.FiltroAssociazioniDosi = filtroAssociazioniDosi

        OnVacUtility.OpenPostelHandler(command)

        Return result

    End Function
    Private Sub LoadDistretto()
        ddlDistretti.Items.Clear()
        ddlDistretti.ClearSelection()

        Dim listaDistr As List(Of Entities.DistrettoDDL) = Nothing
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAnagrafica As New Biz.BizAnagrafiche(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                listaDistr = bizAnagrafica.GetListAnagraficaDistrettiCNSUtente(OnVacContext.UserId)
            End Using
        End Using
        ddlDistretti.DataTextField = "Descrizione"
        ddlDistretti.DataValueField = "Codice"

        If listaDistr.Count > 0 Then
            listaDistr = listaDistr.OrderBy(Function(p) p.Descrizione).ToList()
            listaDistr.Insert(0, New Entities.DistrettoDDL() With {.Codice = String.Empty, .Descrizione = "TUTTI"})
            ddlDistretti.SelectedValue = String.Empty
        End If
        ddlDistretti.DataSource = listaDistr
        ddlDistretti.DataBind()


    End Sub

#End Region

End Class