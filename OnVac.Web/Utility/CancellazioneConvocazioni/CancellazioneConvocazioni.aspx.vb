Imports System.Collections.Generic

Public Class CancellazioneConvocazioni
    Inherits OnVac.Common.PageBase

#Region " Classes "

    <Serializable()>
    Public Class FiltriCancellazioneConvocazioni

        'Public StatiAnagrafici As List(Of String)

        Public Associazioni As List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro1)
        Public DosiAssociazioni As List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro2)

        Public Cicli As List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro1)
        Public Sedute As List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro2)

        Public Vaccinazioni As List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro1)
        Public DosiVaccinazioni As List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro2)

        Public Sub New()
            'StatiAnagrafici = New List(Of String)
            Associazioni = New List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro1)
            DosiAssociazioni = New List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro2)
            Cicli = New List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro1)
            Associazioni = New List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro1)
            Sedute = New List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro2)
            Vaccinazioni = New List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro1)
            DosiVaccinazioni = New List(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro2)
        End Sub

    End Class

#End Region

#Region " Enum "

    Private Enum DgrConvocazioniColumnIndex
        Selezione = 0
        CampiNascosti = 1
        PazienteCognome = 2
        PazienteNome = 3
        DataNascita = 4
        DataConvocazione = 5
        Vaccinazioni = 6
        DataAppuntamento = 7
    End Enum

#End Region

#Region " Proprietà private "

    Private Property CampoOrdinamento As String
        Get
            If ViewState("CampoOrd") Is Nothing Then ViewState("CampoOrd") = String.Empty
            Return ViewState("CampoOrd")
        End Get
        Set(value As String)
            ViewState("CampoOrd") = value
        End Set
    End Property

    Private Property VersoOrdinamento As String
        Get
            If ViewState("VersoOrd") Is Nothing Then ViewState("VersoOrd") = String.Empty
            Return ViewState("VersoOrd")
        End Get
        Set(value As String)
            ViewState("VersoOrd") = value
        End Set
    End Property

    Private Property FiltriMaschera() As FiltriCancellazioneConvocazioni
        Get
            If ViewState("FCC") Is Nothing Then ViewState("FCC") = New FiltriCancellazioneConvocazioni()
            Return ViewState("FCC")
        End Get
        Set(value As FiltriCancellazioneConvocazioni)
            ViewState("FCC") = value
        End Set
    End Property

    ''' <summary>
    ''' Indica se il checkbox "Seleziona tutti" è selezionato o deselezionato
    ''' </summary>
    Private Property SelezionaTuttiChecked As Boolean
        Get
            If ViewState("CHKALL") Is Nothing Then ViewState("CHKALL") = False
            Return DirectCast(ViewState("CHKALL"), Boolean)
        End Get
        Set(value As Boolean)
            ViewState("CHKALL") = value
        End Set
    End Property

    ''' <summary>
    ''' Lista contenente le cnv selezionate da cancellare
    ''' </summary>
    Private Property IdConvocazioniSelezionate() As List(Of Entities.ConvocazionePK)
        Get
            If ViewState("IDCNV") Is Nothing Then ViewState("IDCNV") = New List(Of Entities.ConvocazionePK)()
            Return DirectCast(ViewState("IDCNV"), List(Of Entities.ConvocazionePK))
        End Get
        Set(value As List(Of Entities.ConvocazionePK))
            ViewState("IDCNV") = value
        End Set
    End Property

    ''' <summary>
    ''' Filtri impostati per la ricerca corrente
    ''' </summary>
    Private Property FiltriRicercaCorrente As Entities.FiltriConvocazioneDaCancellare
        Get
            If ViewState("FR") Is Nothing Then ViewState("FR") = New Entities.FiltriConvocazioneDaCancellare()
            Return DirectCast(ViewState("FR"), Entities.FiltriConvocazioneDaCancellare)
        End Get
        Set(value As Entities.FiltriConvocazioneDaCancellare)
            ViewState("FR") = value
        End Set
    End Property

#End Region

#Region " Page Events "

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then

            ResetFilters()

        End If

        Select Case Request.Form.Item("__EVENTTARGET")

            Case "selectAll"

                Me.SelectAll()

        End Select

    End Sub

    Private Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender

        Me.SetImmagineOrdinamento()

    End Sub

#End Region

#Region " Controls Events "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btnCerca"

                RicercaConvocazioni()

            Case "btnElimina"

                CancellazioneConvocazioni()

            Case "btnStampa"

                'TODO [cancCNV]: stampa 

            Case "btnPulisci"

                ResetFilters()

        End Select

    End Sub

    Private Sub dgrConvocazioni_SortCommand(source As Object, e As DataGridSortCommandEventArgs) Handles dgrConvocazioni.SortCommand

        If e.SortExpression = Me.CampoOrdinamento Then
            If Me.VersoOrdinamento = Constants.VersoOrdinamento.Crescente Then
                Me.VersoOrdinamento = Constants.VersoOrdinamento.Decrescente
            Else
                Me.VersoOrdinamento = Constants.VersoOrdinamento.Crescente
            End If
        Else
            Me.CampoOrdinamento = e.SortExpression
            Me.VersoOrdinamento = Constants.VersoOrdinamento.Crescente
        End If

        Me.CaricaDati(Me.dgrConvocazioni.CurrentPageIndex, Me.FiltriRicercaCorrente)

    End Sub

    Private Sub dgrConvocazioni_ItemDataBound(sender As Object, e As DataGridItemEventArgs) Handles dgrConvocazioni.ItemDataBound

        Select Case e.Item.ItemType

            Case ListItemType.Header

                DirectCast(e.Item.FindControl("chkSelezioneHeader"), HtmlControls.HtmlInputCheckBox).Checked = Me.SelezionaTuttiChecked

            Case ListItemType.AlternatingItem, ListItemType.EditItem, ListItemType.Item, ListItemType.SelectedItem

                ' Gestione checkbox selezione
                If Me.IdConvocazioniSelezionate.Count > 0 Then

                    Dim chkSelezioneItem As CheckBox = DirectCast(e.Item.FindControl("chkSelezioneItem"), CheckBox)
                    If Not chkSelezioneItem Is Nothing Then

                        Dim pkCnv As Entities.ConvocazionePK = GetConvocazionePKFromDataGridItem(e.Item)
                        chkSelezioneItem.Checked = Me.IdConvocazioniSelezionate.Any(Function(cnv) cnv.IdPaziente = pkCnv.IdPaziente AndAlso cnv.Data = pkCnv.Data)

                    End If
                End If

        End Select

    End Sub

    Private Sub dgrConvocazioni_PageIndexChanged(source As Object, e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dgrConvocazioni.PageIndexChanged

        Me.SetIdConvocazioniSelezionate()

        Me.CaricaDati(e.NewPageIndex, Me.FiltriRicercaCorrente)

    End Sub

#Region " Associazioni Dosi "

    Private Sub btnImgAssociazioniDosi_Click(sender As Object, e As System.EventArgs) Handles btnImgAssociazioniDosi.Click

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

    Private Sub btnOk_FiltroAssociazioniDosi_Click(sender As System.Object, e As System.EventArgs) Handles btnOk_FiltroAssociazioniDosi.Click

        'Chiusura della modale
        Me.fmFiltroAssociazioniDosi.VisibileMD = False

        'Aggiornamento dei filtri nel viewstate
        Dim dtAssociazioni As DataTable = Me.UscFiltroAssociazioniDosi.getValoriSelezionatiFiltro1()
        Me.FiltriMaschera.Associazioni = dtAssociazioni.ConvertToList(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro1)()
        Dim dtDosiAssociazioni As DataTable = Me.UscFiltroAssociazioniDosi.getValoriSelezionatiFiltro2()
        Me.FiltriMaschera.DosiAssociazioni = dtDosiAssociazioni.ConvertToList(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro2)()

        'Aggiornamento label
        Me.lblAssociazioniDosi.Text = Me.UscFiltroAssociazioniDosi.getStringaFormattata()

    End Sub

    Private Sub btnAnnulla_FiltroAssociazioniDosi_Click(sender As System.Object, e As System.EventArgs) Handles btnAnnulla_FiltroAssociazioniDosi.Click

        Me.fmFiltroAssociazioniDosi.VisibileMD = False

    End Sub

#End Region

#Region " Cicli Sedute "

    Private Sub btnImgCicliSedute_Click(sender As Object, e As System.EventArgs) Handles btnImgCicliSedute.Click

        'Set degli eventuali filtri cicli
        If FiltriMaschera.Cicli.Count > 0 Then

            Dim cicli = FiltriMaschera.Cicli.ConvertToDataTable()
            Me.UscFiltroCicliSedute.setValoriSelezionatiFiltro1(cicli)

        End If

        'Set degli eventuali filtri sedute
        If FiltriMaschera.Sedute.Count > 0 Then

            Dim sedute = FiltriMaschera.Sedute.ConvertToDataTable()
            Me.UscFiltroCicliSedute.setValoriSelezionatiFiltro2(sedute)

        End If

        'Apertura della modale
        Me.fmFiltroCicliSedute.VisibileMD = True

    End Sub

    Private Sub btnOk_FiltroCicliSedute_Click(sender As System.Object, e As System.EventArgs) Handles btnOk_FiltroCicliSedute.Click

        'Chiusura della modale
        Me.fmFiltroCicliSedute.VisibileMD = False

        'Aggiornamento dei filtri nel viewstate
        Dim dtCicli As DataTable = Me.UscFiltroCicliSedute.getValoriSelezionatiFiltro1()
        Me.FiltriMaschera.Cicli = dtCicli.ConvertToList(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro1)()
        Dim dtSedute As DataTable = Me.UscFiltroCicliSedute.getValoriSelezionatiFiltro2()
        Me.FiltriMaschera.Sedute = dtSedute.ConvertToList(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro2)()

        'Aggiornamento label
        Me.lblCicliSedute.Text = Me.UscFiltroCicliSedute.getStringaFormattata()

    End Sub

    Private Sub btnAnnulla_FiltroCicliSedute_Click(sender As System.Object, e As System.EventArgs) Handles btnAnnulla_FiltroCicliSedute.Click

        Me.fmFiltroCicliSedute.VisibileMD = False

    End Sub

#End Region

#Region " Vaccinazioni Dosi "

    Private Sub btnImgVaccinazioniDosi_Click(sender As Object, e As System.EventArgs) Handles btnImgVaccinazioniDosi.Click

        'Set degli eventuali filtri vaccinazioni
        If FiltriMaschera.Vaccinazioni.Count > 0 Then

            Dim vaccinazioni = FiltriMaschera.Vaccinazioni.ConvertToDataTable()
            Me.UscFiltroVaccinazioniDosi.setValoriSelezionatiFiltro1(vaccinazioni)

        End If

        'Set degli eventuali filtri dosi vaccinazioni
        If FiltriMaschera.DosiVaccinazioni.Count > 0 Then

            Dim dosiVaccinazioni = FiltriMaschera.DosiVaccinazioni.ConvertToDataTable()
            Me.UscFiltroVaccinazioniDosi.setValoriSelezionatiFiltro2(dosiVaccinazioni)

        End If

        'Apertura della modale
        Me.fmFiltroVaccinazioniDosi.VisibileMD = True

    End Sub

    Private Sub btnOk_FiltroVaccinazioniDosi_Click(sender As System.Object, e As System.EventArgs) Handles btnOk_FiltroVaccinazioniDosi.Click

        'Chiusura della modale
        Me.fmFiltroVaccinazioniDosi.VisibileMD = False

        'Aggiornamento dei filtri nel viewstate
        Dim dtVaccinazioni As DataTable = Me.UscFiltroVaccinazioniDosi.getValoriSelezionatiFiltro1()
        Me.FiltriMaschera.Vaccinazioni = dtVaccinazioni.ConvertToList(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro1)()
        Dim dtDosiVaccinazioni As DataTable = Me.UscFiltroVaccinazioniDosi.getValoriSelezionatiFiltro2()
        Me.FiltriMaschera.DosiVaccinazioni = dtDosiVaccinazioni.ConvertToList(Of UscFiltroPrenotazioneSelezioneMultipla.Filtro2)()

        'Aggiornamento label
        Me.lblVaccinazioniDosi.Text = Me.UscFiltroVaccinazioniDosi.getStringaFormattata()

    End Sub

    Private Sub btnAnnulla_FiltroVaccinazioniDosi_Click(sender As System.Object, e As System.EventArgs) Handles btnAnnulla_FiltroVaccinazioniDosi.Click

        Me.fmFiltroVaccinazioniDosi.VisibileMD = False

    End Sub

#End Region

#End Region

#Region " Methods "

    Private Sub ResetFilters()

        Me.CampoOrdinamento = Nothing
        Me.VersoOrdinamento = Nothing

        Me.FiltriMaschera = New FiltriCancellazioneConvocazioni()
        Me.FiltriRicercaCorrente = New Entities.FiltriConvocazioneDaCancellare()

        'Filtri pazienti
        Me.dpkDataNascitaDa.Text = String.Empty
        Me.dpkDataNascitaA.Text = String.Empty
        Me.ddlSesso.SelectedIndex = -1
        Me.omlCategorieRischio.Codice = String.Empty
        Me.omlCategorieRischio.Descrizione = String.Empty
        Me.omlMalattia.Codice = String.Empty
        Me.omlMalattia.Descrizione = String.Empty

        'Filtri convocazioni
        Me.omlConsultorio.Codice = OnVacUtility.Variabili.CNS.Codice
        Me.omlConsultorio.RefreshDataBind()
        Me.dpkDataCnvDa.Text = String.Empty
        Me.dpkDataCnvA.Text = String.Empty
        Me.rbCicliSedute.Checked = False
        Me.lblCicliSedute.Text = String.Empty
        Me.rbAssociazioniDosi.Checked = False
        Me.lblAssociazioniDosi.Text = String.Empty
        Me.rbVaccinazioniDosi.Checked = False
        Me.lblVaccinazioniDosi.Text = String.Empty
        Me.rbNessunFiltro.Checked = True

        'Operazioni
        Me.chkCnvConAppuntamento.Checked = False
        Me.chkCnvConSollecito.Checked = False
        Me.chkCancellaInteraCnv.Checked = False

        'Griglia
        Me.dgrConvocazioni.SelectedIndex = -1
        Me.dgrConvocazioni.DataSource = Nothing
        Me.dgrConvocazioni.DataBind()

    End Sub

    Private Sub SetIdConvocazioniSelezionate()

        For Each item As DataGridItem In Me.dgrConvocazioni.Items

            Dim pkCnv As Entities.ConvocazionePK = GetConvocazionePKFromDataGridItem(item)

            If DirectCast(item.FindControl("chkSelezioneItem"), CheckBox).Checked Then

                If Not Me.IdConvocazioniSelezionate.Any(Function(cnv) cnv.IdPaziente = pkCnv.IdPaziente AndAlso cnv.Data = pkCnv.Data) Then
                    Me.IdConvocazioniSelezionate.Add(pkCnv)
                End If

            Else

                Dim cnv As Entities.ConvocazionePK =
                    Me.IdConvocazioniSelezionate.FirstOrDefault(Function(p) p.IdPaziente = pkCnv.IdPaziente AndAlso p.Data = pkCnv.Data)

                If Not cnv Is Nothing Then Me.IdConvocazioniSelezionate.Remove(cnv)

            End If

        Next

    End Sub

    Private Function GetConvocazionePKFromDataGridItem(item As DataGridItem) As Entities.ConvocazionePK

        If item Is Nothing Then Return Nothing

        Dim pkCnv As New Entities.ConvocazionePK()
        pkCnv.IdPaziente = Convert.ToInt64(HttpUtility.HtmlDecode(DirectCast(item.FindControl("hidIdPaziente"), HiddenField).Value))
        pkCnv.Data = Convert.ToDateTime(HttpUtility.HtmlDecode(DirectCast(item.FindControl("hidData"), HiddenField).Value))

        Return pkCnv

    End Function

    Protected Function ConvertToDateString(data As Object) As String

        If data IsNot Nothing Then
            Return CDate(data).ToString("dd/MM/yyyy HH:mm")
        Else
            Return String.Empty
        End If

    End Function

    Private Function GetFiltriRicerca() As Entities.FiltriConvocazioneDaCancellare

        Dim filtri As New Entities.FiltriConvocazioneDaCancellare()

        'Filtri obbligatori
        filtri.CodiceCentroVaccinale = Me.omlConsultorio.Codice
        filtri.DescrizioneCentroVaccinale = Me.omlConsultorio.Descrizione

        'Data nascita paziente
        If Not String.IsNullOrWhiteSpace(Me.dpkDataNascitaDa.Text) Then
            filtri.DataNascitaDa = Me.dpkDataNascitaDa.Data
        End If
        If Not String.IsNullOrWhiteSpace(Me.dpkDataNascitaA.Text) Then
            filtri.DataNascitaA = Me.dpkDataNascitaA.Data
        End If

        'Sesso paziente
        If Not String.IsNullOrWhiteSpace(Me.ddlSesso.SelectedValue) Then
            filtri.Sesso = Me.ddlSesso.SelectedValue
        End If

        'Categoria rischio
        If Not String.IsNullOrWhiteSpace(Me.omlCategorieRischio.Codice) Then
            filtri.CodiceCategoriaRischio = Me.omlCategorieRischio.Codice
            filtri.DescrizioneCategoriaRischio = Me.omlCategorieRischio.Descrizione
        End If

        'Malattia
        If Not String.IsNullOrWhiteSpace(Me.omlMalattia.Codice) Then
            filtri.CodiceMalattia = Me.omlMalattia.Codice
            filtri.DescrizioneMalattia = Me.omlMalattia.Descrizione
        End If

        'Stati anagrafici
        Dim lstStatiAnag As List(Of String) = Me.staStatiAnagrafici.GetListStatiAnagraficiSelezionati()
        If lstStatiAnag IsNot Nothing AndAlso lstStatiAnag.Count > 0 Then
            filtri.CodiciStatiAnagrafici = lstStatiAnag
            filtri.DescrizioniStatiAnagrafici = Me.staStatiAnagrafici.GetSelectedDescriptions()
        End If

        'Data convocazione
        If Not String.IsNullOrWhiteSpace(Me.dpkDataCnvDa.Text) Then
            filtri.DataConvocazioneDa = Me.dpkDataCnvDa.Data
        End If
        If Not String.IsNullOrWhiteSpace(Me.dpkDataCnvA.Text) Then
            filtri.DataConvocazioneA = Me.dpkDataCnvA.Data
        End If

        'FILTRI COMPOSTI: Cicli-Sedute Associazioni-Dosi Vaccinazioni-Dosi
        Dim filter As New Entities.FiltroComposto()

        If Me.rbCicliSedute.Checked Then 'Cicli-Sedute

            If FiltriMaschera.Cicli.Count > 0 OrElse FiltriMaschera.Sedute.Count > 0 Then

                'Cicli
                filter.CodiceValore = FiltriMaschera.Cicli.Select(Function(cic) New KeyValuePair(Of String, String)(cic.Codice, cic.Valore)).ToList()

                'Sedute
                Dim sedute As List(Of String) = FiltriMaschera.Sedute.Select(Function(sed) sed.Codice).ToList()
                If (sedute IsNot Nothing AndAlso sedute.Count > 0) Then
                    filter.Valori = sedute.Select(Function(sed) Integer.Parse(sed)).ToList()
                End If

                filtri.CicliSedute = filter

            End If

        ElseIf Me.rbAssociazioniDosi.Checked Then 'Associazioni-Dosi

            If FiltriMaschera.Associazioni.Count > 0 OrElse FiltriMaschera.DosiAssociazioni.Count > 0 Then

                'Associazioni
                filter.CodiceValore = FiltriMaschera.Associazioni.Select(Function(ass) New KeyValuePair(Of String, String)(ass.Codice, ass.Valore)).ToList()

                'Dosi
                Dim dosi As List(Of String) = FiltriMaschera.DosiAssociazioni.Select(Function(dos) dos.Codice).ToList()
                If (dosi IsNot Nothing AndAlso dosi.Count > 0) Then
                    filter.Valori = dosi.Select(Function(dos) Integer.Parse(dos)).ToList()
                End If

                filtri.AssociazioniDosi = filter

            End If

        ElseIf Me.rbVaccinazioniDosi.Checked Then 'Vaccinazioni-Dosi

            If FiltriMaschera.Vaccinazioni.Count > 0 OrElse FiltriMaschera.DosiVaccinazioni.Count > 0 Then

                'Vaccinazioni
                filter.CodiceValore = FiltriMaschera.Vaccinazioni.Select(Function(vac) New KeyValuePair(Of String, String)(vac.Codice, vac.Valore)).ToList()

                'Dosi
                Dim dosi As List(Of String) = FiltriMaschera.DosiVaccinazioni.Select(Function(dos) dos.Codice).ToList()
                If (dosi IsNot Nothing AndAlso dosi.Count > 0) Then
                    filter.Valori = dosi.Select(Function(dos) Integer.Parse(dos)).ToList()
                End If

                filtri.VaccinazioniDosi = filter

            End If

        End If

        Return filtri

    End Function

    ''' <summary>
    ''' Click del checkbox nell'header del datagrid per selezionare/deselezionare tutte le righe del datagrid
    ''' </summary>
    Private Sub SelectAll()

        Dim selezionaTutti As Boolean = False

        If Not Boolean.TryParse(Request.Form.Item("__EVENTARGUMENT"), selezionaTutti) Then
            Return
        End If

        Me.IdConvocazioniSelezionate = New List(Of Entities.ConvocazionePK)()
        Me.SelezionaTuttiChecked = selezionaTutti

        If Me.SelezionaTuttiChecked Then

            Dim listIdCnv As List(Of Entities.ConvocazionePK) = Nothing

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using biz As New Biz.BizCancellazioneConvocazioni(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())
                    listIdCnv = biz.GetIdConvocazioniPerUtilityCancellazione(Me.FiltriRicercaCorrente)
                End Using
            End Using

            If Not listIdCnv Is Nothing AndAlso listIdCnv.Count > 0 Then
                Me.IdConvocazioniSelezionate = listIdCnv.Clone()
            End If

        End If

        Me.CaricaDati(Me.dgrConvocazioni.CurrentPageIndex, Me.FiltriRicercaCorrente)

    End Sub

    Private Sub RicercaConvocazioni()

        Me.IdConvocazioniSelezionate = New List(Of Entities.ConvocazionePK)()
        Me.SelezionaTuttiChecked = False

        CaricaDati(0)

    End Sub

    Private Sub CaricaDati(currentPageIndex As Int32)

        Me.CaricaDati(currentPageIndex, Me.GetFiltriRicerca())

    End Sub

    Private Sub CaricaDati(currentPageIndex As Int32, filtri As Entities.FiltriConvocazioneDaCancellare)

        If filtri Is Nothing Then filtri = Me.GetFiltriRicerca()

        ' Check filtri obbligatori
        If String.IsNullOrWhiteSpace(filtri.CodiceCentroVaccinale) Then
            Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Selezionare il Centro Vaccinale per proseguire con la ricerca.", "ERR", False, False))
            Return
        End If

        ' Impostazione filtri correnti
        Me.FiltriRicercaCorrente = filtri.Clone()

        Dim result As Biz.BizCancellazioneConvocazioni.GetConvocazioniResult = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using biz As New Biz.BizCancellazioneConvocazioni(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                Dim paramToPass As Biz.BizCancellazioneConvocazioni.ParametriGetConvocazioniBiz = New Biz.BizCancellazioneConvocazioni.ParametriGetConvocazioniBiz()
                paramToPass.Filtri = Me.FiltriRicercaCorrente
                paramToPass.PageIndex = currentPageIndex
                paramToPass.PageSize = Me.dgrConvocazioni.PageSize
                paramToPass.CampoOrdinamento = Me.CampoOrdinamento
                paramToPass.VersoOrdinamento = Me.VersoOrdinamento

                result = biz.GetConvocazioniPerUtilityCancellazione(paramToPass)

            End Using
        End Using

        Me.dgrConvocazioni.VirtualItemCount = result.CountConvocazioni
        Me.dgrConvocazioni.CurrentPageIndex = currentPageIndex

        Me.dgrConvocazioni.SelectedIndex = -1
        Me.dgrConvocazioni.DataSource = result.Convocazioni
        Me.dgrConvocazioni.DataBind()

    End Sub

    Private Sub SetImmagineOrdinamento()

        Dim id As String = String.Empty

        Select Case Me.CampoOrdinamento
            Case Me.dgrConvocazioni.Columns(DgrConvocazioniColumnIndex.PazienteCognome).SortExpression
                id = "imgCog"
            Case Me.dgrConvocazioni.Columns(DgrConvocazioniColumnIndex.PazienteNome).SortExpression
                id = "imgNom"
            Case Me.dgrConvocazioni.Columns(DgrConvocazioniColumnIndex.DataNascita).SortExpression
                id = "imgNas"
            Case Me.dgrConvocazioni.Columns(DgrConvocazioniColumnIndex.DataConvocazione).SortExpression
                id = "imgCnv"
            Case Me.dgrConvocazioni.Columns(DgrConvocazioniColumnIndex.Vaccinazioni).SortExpression
                id = "imgVac"
            Case Me.dgrConvocazioni.Columns(DgrConvocazioniColumnIndex.DataAppuntamento).SortExpression
                id = "imgDApp"
        End Select

        Dim imageUrl As String = String.Empty

        If Me.VersoOrdinamento = "ASC" Then
            imageUrl = Me.ResolveClientUrl("~/Images/arrow_up_small.gif")
        Else
            imageUrl = Me.ResolveClientUrl("~/Images/arrow_down_small.gif")
        End If

        Me.OnitLayout31.InsertRoutineJS(String.Format("ImpostaImmagineOrdinamento('{0}', '{1}')", id, imageUrl))

    End Sub

    ''' <summary>
    ''' Avvia il batch di cancellazione
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CancellazioneConvocazioni()

        SetIdConvocazioniSelezionate()

        If IdConvocazioniSelezionate.Count = 0 Then
            OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Nessuna convocazione selezionata: processo di cancellazione programmazione NON avviato.", "ERR", False, False))
            Return
        End If

        Dim maxCnv As Integer = Convert.ToInt32(HttpUtility.HtmlDecode(hidMaxCnvDaElab.Value))

        If maxCnv > 0 AndAlso IdConvocazioniSelezionate.Count > maxCnv Then
            OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(String.Format("Possono essere selezionate al massimo {0} convocazioni da eliminare: processo di cancellazione programmazione NON avviato.", maxCnv.ToString()), "ERR", False, False))
            Return
        End If

        Dim result As Biz.BizGenericResult = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using biz As New Biz.BizCancellazioneConvocazioni(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                Dim command As New Biz.BizCancellazioneConvocazioni.StartBatchCancellazioneConvocazioniCommand()

                command.CancellaInteraConvocazione = chkCancellaInteraCnv.Checked
                command.CancellaConSolleciti = chkCnvConSollecito.Checked
                command.CancellaConAppuntamenti = chkCnvConAppuntamento.Checked
                command.PazientiConvocazioniDaElaborare = IdConvocazioniSelezionate

                ' N.B. : i tre filtri sono esclusivi
                If Not FiltriRicercaCorrente.AssociazioniDosi Is Nothing Then
                    command.FiltriProgrammazione = GetFiltriProgrammazioneFromFiltroComposto(FiltriRicercaCorrente.AssociazioniDosi, ",")
                    command.TipoFiltri = Entities.TipoFiltriProgrammazione.AssociazioneDose
                ElseIf Not FiltriRicercaCorrente.CicliSedute Is Nothing Then
                    command.FiltriProgrammazione = GetFiltriProgrammazioneFromFiltroComposto(FiltriRicercaCorrente.CicliSedute, ",")
                    command.TipoFiltri = Entities.TipoFiltriProgrammazione.CicloSeduta
                ElseIf Not FiltriRicercaCorrente.VaccinazioniDosi Is Nothing Then
                    command.FiltriProgrammazione = GetFiltriProgrammazioneFromFiltroComposto(FiltriRicercaCorrente.VaccinazioniDosi, ",")
                    command.TipoFiltri = Entities.TipoFiltriProgrammazione.VaccinazioneDose
                Else
                    command.FiltriProgrammazione = Nothing
                    command.TipoFiltri = Nothing
                End If

                ' Filtri relativi alla ricerca effettuata, che verranno stampati nel report
                command.DatiReport.CodiceCentroVaccinale = FiltriRicercaCorrente.CodiceCentroVaccinale
                command.DatiReport.DescrizioneCentroVaccinale = FiltriRicercaCorrente.DescrizioneCentroVaccinale
                command.DatiReport.DataNascitaDa = FiltriRicercaCorrente.DataNascitaDa
                command.DatiReport.DataNascitaA = FiltriRicercaCorrente.DataNascitaA
                command.DatiReport.Sesso = FiltriRicercaCorrente.Sesso
                command.DatiReport.DescrizioneMalattia = FiltriRicercaCorrente.DescrizioneMalattia
                command.DatiReport.DescrizioneCategoriaRischio = FiltriRicercaCorrente.DescrizioneCategoriaRischio
                command.DatiReport.DescrizioneStatiAnagrafici = FiltriRicercaCorrente.DescrizioniStatiAnagrafici
                command.DatiReport.DataConvocazioneDa = FiltriRicercaCorrente.DataConvocazioneDa
                command.DatiReport.DataConvocazioneA = FiltriRicercaCorrente.DataConvocazioneA

                result = biz.StartBatchCancellazioneConvocazioni(command)

            End Using
        End Using

        OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(result.Message, "MSG", False, False))

    End Sub

    Private Function GetFiltriProgrammazioneFromFiltroComposto(filtroComposto As Entities.FiltroComposto, separator As String) As List(Of Entities.FiltroProgrammazione)

        If filtroComposto Is Nothing Then Return Nothing

        Dim filtroProgrammazione As New List(Of Entities.FiltroProgrammazione)()

        Dim item As Entities.FiltroProgrammazione = Nothing

        If Not filtroComposto.CodiceValore Is Nothing AndAlso filtroComposto.CodiceValore.Count > 0 Then

            For Each f As KeyValuePair(Of String, String) In filtroComposto.CodiceValore

                If Not String.IsNullOrWhiteSpace(f.Value) Then

                    Dim values() As String = f.Value.Split(separator)
                    For Each value As String In values

                        Dim valoreFiltro As Integer = 0

                        If Integer.TryParse(value, valoreFiltro) Then
                            item = New Entities.FiltroProgrammazione()
                            item.Codice = f.Key
                            item.Valore = valoreFiltro
                            filtroProgrammazione.Add(item)
                        End If

                    Next

                Else

                    item = New Entities.FiltroProgrammazione()
                    item.Codice = f.Key
                    item.Valore = Nothing
                    filtroProgrammazione.Add(item)

                End If

            Next

        End If

        If Not filtroComposto.Valori Is Nothing AndAlso filtroComposto.Valori.Count > 0 Then

            For Each valoreFiltro As Integer In filtroComposto.Valori
                item = New Entities.FiltroProgrammazione()
                item.Codice = String.Empty
                item.Valore = valoreFiltro
                filtroProgrammazione.Add(item)
            Next

        End If

        Return filtroProgrammazione

    End Function

#End Region

End Class