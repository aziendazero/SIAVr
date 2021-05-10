Imports System.Collections.Generic
Imports Onit.Controls
Imports Onit.OnAssistnet.OnVac.Biz
Imports Onit.OnAssistnet.OnVac.DAL

Partial Class ElencoVaccinazioniEseguite
    Inherits Common.UserControlFinestraModalePageBase

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

#Region " Events "

    Public Event DosiModificate()

#End Region

#Region " Public Members "

    Public strJS As String

#End Region

#Region " Properties "

    Property ArgomentoLog() As String
        Get
            Return DirectCast(ViewState("ArgomentoLog"), String)
        End Get
        Set(Value As String)
            ViewState("ArgomentoLog") = Value
        End Set
    End Property

    Property dt_vacEseguite() As DataTable
        Get
            If Session("OnVac_dt_Eff") Is Nothing Then Return Nothing
            Return DirectCast(Session("OnVac_dt_Eff"), SerializableDataTableContainer).Data
        End Get
        Set(Value As DataTable)
            If Session("OnVac_dt_Eff") Is Nothing Then
                Session("OnVac_dt_Eff") = New SerializableDataTableContainer()
            End If
            DirectCast(Session("OnVac_dt_Eff"), SerializableDataTableContainer).Data = Value
        End Set
    End Property

    Property ord() As Boolean()
        Get
            Return DirectCast(ViewState("ord"), Boolean())
        End Get
        Set(Value As Boolean())
            ViewState("ord") = Value
        End Set
    End Property

    Public Property IsGestioneCentrale As Boolean
        Get
            If ViewState("ElencoVaccinazioniEseguite_IsGestioneCentrale") Is Nothing Then ViewState("ElencoVaccinazioniEseguite_IsGestioneCentrale") = False
            Return ViewState("ElencoVaccinazioniEseguite_IsGestioneCentrale")
        End Get
        Set(Value As Boolean)
            ViewState("ElencoVaccinazioniEseguite_IsGestioneCentrale") = Value
        End Set
    End Property

    Private Property ListCondizioniSanitarie As List(Of KeyValuePair(Of String, String))
        Get
            If ViewState("CS") Is Nothing Then ViewState("CS") = New List(Of KeyValuePair(Of String, String))()
            Return DirectCast(ViewState("CS"), List(Of KeyValuePair(Of String, String)))
        End Get
        Set(Value As List(Of KeyValuePair(Of String, String)))
            ViewState("CS") = Value
        End Set
    End Property

    Private Property ListCondizioniRischio As List(Of KeyValuePair(Of String, String))
        Get
            If ViewState("CR") Is Nothing Then ViewState("CR") = New List(Of KeyValuePair(Of String, String))()
            Return DirectCast(ViewState("CR"), List(Of KeyValuePair(Of String, String)))
        End Get
        Set(Value As List(Of KeyValuePair(Of String, String)))
            ViewState("CR") = Value
        End Set
    End Property

#End Region

#Region " Types "

    Private Enum ErroreEx
        DataPrimaSedPrec
        DataDopoSedSucc
        StessaDose
    End Enum

    Private Class VacNoEseguita

        Public Errore As ErroreEx
        Public DescrizioneVaccinazione As String
        Public DoseVaccinazione As Integer

        Public Sub New(descrizioneVaccinazione As String, doseVaccinazione As Integer, tipoErrore As ErroreEx)
            Errore = tipoErrore
            descrizioneVaccinazione = descrizioneVaccinazione
            doseVaccinazione = doseVaccinazione
        End Sub

    End Class

#End Region

#Region " Overrides "

    Public Overrides Sub LoadModale()

        'Dim ordine() As Boolean = {True, True, True, True, True, True, True, True, True, True, True}
        'ord = ordine

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)
            Using biz As New BizVaccinazioniEseguite(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                dt_vacEseguite = biz.GetVaccinazioniEseguite(OnVacUtility.Variabili.PazId, IsGestioneCentrale)

            End Using
        End Using

        'dg_vacEff.DataSource = dt_vacEseguite
        'dg_vacEff.DataBind()

        Me.OrdinamentoASC(CampiOrdinamento.DataEffettuazione.IndiceColonna) = True
        BindDataGridEseguite(CampiOrdinamento.DataEffettuazione.SortExpression)

        If dg_vacEff.Items.Count = 0 Then
            SetEnableToolbarButton("btn_Modifica", False)
        Else
            SetEnableToolbarButton("btn_Modifica", Settings.VACPROG_MODVACEFFETTUATE)
        End If

        SetEnableToolbarButton("btn_Salva", False)
        SetEnableToolbarButton("btn_Annulla", False)

    End Sub

#End Region

#Region " Ordinamento campi "

    Private Class CampoOrdinamento

        Public Property SortExpression As String
        Public Property IndiceColonna As Integer
        Public Property IdImmagine As String
        Private Property SortFormula As String

        Public Sub New(indiceColonna As Integer, sortExpression As String, idImmagine As String)

            Me.New(indiceColonna, sortExpression, idImmagine, sortExpression + " {0}, vac_obbligatoria, vac_descrizione")

        End Sub

        Public Sub New(indiceColonna As Integer, sortExpression As String, idImmagine As String, sortFormula As String)
            Me.IndiceColonna = indiceColonna
            Me.SortExpression = sortExpression
            Me.IdImmagine = idImmagine
            Me.SortFormula = sortFormula
        End Sub

        Public Function GetSortFormula(sortOrder As String) As String
            Return String.Format(SortFormula, sortOrder)
        End Function

    End Class

    Private Class CampiOrdinamento

        Public Shared ReadOnly Property DescrizioneVaccinazione As CampoOrdinamento
            Get
                Return New CampoOrdinamento(0, "vac_descrizione", "ordDesc")
            End Get
        End Property

        Public Shared ReadOnly Property CodiceVaccinazione As CampoOrdinamento
            Get
                Return New CampoOrdinamento(1, "ves_vac_codice", "ordCod")
            End Get
        End Property

        Public Shared ReadOnly Property DoseVaccinazione As CampoOrdinamento
            Get
                Return New CampoOrdinamento(2, "ves_n_richiamo", "ordDosi")
            End Get
        End Property

        Public Shared ReadOnly Property CondizioneSanitaria As CampoOrdinamento
            Get
                Return New CampoOrdinamento(3, "ves_mal_codice_cond_sanitaria", "ordCondSan")
            End Get
        End Property

        Public Shared ReadOnly Property CondizioneRischio As CampoOrdinamento
            Get
                Return New CampoOrdinamento(4, "ves_rsc_codice", "ordCondRischio")
            End Get
        End Property

        Public Shared ReadOnly Property DataEffettuazione As CampoOrdinamento
            Get
                Return New CampoOrdinamento(5, "ves_data_effettuazione", "ordData")
            End Get
        End Property

        Public Shared ReadOnly Property Associazione As CampoOrdinamento
            Get
                Return New CampoOrdinamento(6, "associazione", "ordAss", "ves_ass_codice {0}, ves_ass_n_dose {0}, vac_obbligatoria, vac_descrizione")
            End Get
        End Property

        Public Shared ReadOnly Property Lotto As CampoOrdinamento
            Get
                Return New CampoOrdinamento(7, "ves_lot_codice", "ordLotto")
            End Get
        End Property

        Public Shared ReadOnly Property NomeCommerciale As CampoOrdinamento
            Get
                Return New CampoOrdinamento(8, "noc_descrizione", "ordNC")
            End Get
        End Property

        Public Shared ReadOnly Property Operatore As CampoOrdinamento
            Get
                Return New CampoOrdinamento(9, "ope_nome", "ordOp")
            End Get
        End Property

        Public Shared ReadOnly Property Consultorio As CampoOrdinamento
            Get
                Return New CampoOrdinamento(10, "cns_descrizione", "ordCNS")
            End Get
        End Property

    End Class

    Private Property OrdinamentoASC() As List(Of Boolean)
        Get
            If ViewState("ord") Is Nothing Then ViewState("ord") = New List(Of Boolean)()

            Dim list As List(Of Boolean) = DirectCast(ViewState("ord"), List(Of Boolean))

            For i As Integer = 0 To 10
                list.Add(True)
            Next

            Return list
        End Get
        Set(Value As List(Of Boolean))
            ViewState("ord") = Value
        End Set
    End Property

    Private Sub SetOrdinamento(campoOrdinamento As CampoOrdinamento)

        Dim imageUrlArrowDown As String = ResolveClientUrl("~/Images/arrow_down_small.gif")
        Dim imageUrlArrowUp As String = ResolveClientUrl("~/Images/arrow_up_small.gif")

        If OrdinamentoASC(campoOrdinamento.IndiceColonna) Then
            dt_vacEseguite.DefaultView.Sort = campoOrdinamento.GetSortFormula("DESC")
            strJS &= GetScriptModificaHeader(campoOrdinamento.IdImmagine, imageUrlArrowDown)
            OrdinamentoASC(campoOrdinamento.IndiceColonna) = False
        Else
            dt_vacEseguite.DefaultView.Sort = campoOrdinamento.GetSortFormula("ASC")
            strJS &= GetScriptModificaHeader(campoOrdinamento.IdImmagine, imageUrlArrowUp)
            OrdinamentoASC(campoOrdinamento.IndiceColonna) = True
        End If

    End Sub

#End Region

#Region " Datagrid Events "

    Private Sub BindDataGridEseguite(sortExpression As String)

        Select Case sortExpression

            Case CampiOrdinamento.DescrizioneVaccinazione.SortExpression

                SetOrdinamento(CampiOrdinamento.DescrizioneVaccinazione)

            Case CampiOrdinamento.CodiceVaccinazione.SortExpression

                SetOrdinamento(CampiOrdinamento.CodiceVaccinazione)

            Case CampiOrdinamento.DoseVaccinazione.SortExpression

                SetOrdinamento(CampiOrdinamento.DoseVaccinazione)

            Case CampiOrdinamento.CondizioneSanitaria.SortExpression

                SetOrdinamento(CampiOrdinamento.CondizioneSanitaria)

            Case CampiOrdinamento.CondizioneRischio.SortExpression

                SetOrdinamento(CampiOrdinamento.CondizioneRischio)

            Case CampiOrdinamento.DataEffettuazione.SortExpression

                SetOrdinamento(CampiOrdinamento.DataEffettuazione)

            Case CampiOrdinamento.Associazione.SortExpression

                SetOrdinamento(CampiOrdinamento.Associazione)

            Case CampiOrdinamento.Lotto.SortExpression

                SetOrdinamento(CampiOrdinamento.Lotto)

            Case CampiOrdinamento.NomeCommerciale.SortExpression

                SetOrdinamento(CampiOrdinamento.NomeCommerciale)

            Case CampiOrdinamento.Operatore.SortExpression

                SetOrdinamento(CampiOrdinamento.Operatore)

            Case CampiOrdinamento.Consultorio.SortExpression

                SetOrdinamento(CampiOrdinamento.Consultorio)

        End Select

        Dim codiciMalattie As New List(Of String)()
        Dim codiciCategorieRischio As New List(Of String)()

        For Each item As DataRowView In dt_vacEseguite.DefaultView

            If Not item("ves_mal_codice_cond_sanitaria") Is Nothing AndAlso Not item("ves_mal_codice_cond_sanitaria") Is DBNull.Value Then

                Dim codiceMalattia As String = item("ves_mal_codice_cond_sanitaria").ToString()
                If Not String.IsNullOrWhiteSpace(codiceMalattia) Then

                    If Not codiciMalattie.Contains(codiceMalattia) Then
                        codiciMalattie.Add(codiceMalattia)
                    End If

                End If

            End If

            If Not item("ves_rsc_codice") Is Nothing AndAlso Not item("ves_rsc_codice") Is DBNull.Value Then

                Dim codiceRischio As String = item("ves_rsc_codice").ToString()
                If Not String.IsNullOrWhiteSpace(codiceRischio) Then

                    If Not codiciCategorieRischio.Contains(codiceRischio) Then
                        codiciCategorieRischio.Add(codiceRischio)
                    End If

                End If

            End If

        Next

        Using genericProvider As New DbGenericProvider(OnVacContext.Connection)

            If codiciMalattie.Count > 0 Then
                Using bizMalattie As New BizMalattie(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                    ListCondizioniSanitarie = bizMalattie.GetCodiceDescrizioneMalattie(codiciMalattie)
                End Using
            End If

            If codiciCategorieRischio.Count > 0 Then
                Using bizRischio As New BizCategorieRischio(genericProvider, Settings, OnVacContext.CreateBizContextInfos())
                    ListCondizioniRischio = bizRischio.GetCodiceDescrizioneCategorieRischio(codiciCategorieRischio)
                End Using
            End If

        End Using

        dg_vacEff.DataSource = dt_vacEseguite.DefaultView
        dg_vacEff.DataBind()

    End Sub

    Private Sub dg_vacEff_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dg_vacEff.ItemDataBound

        Select Case e.Item.ItemType

            Case ListItemType.Item, ListItemType.EditItem, ListItemType.AlternatingItem

                Dim currentRow As DataRowView = e.Item.DataItem

                ' Condizione sanitaria
                Dim codiceMalattia As String = currentRow("ves_mal_codice_cond_sanitaria").ToString()
                Dim descrizioneMalattia As String = ListCondizioniSanitarie.Where(Function(p) p.Key = codiceMalattia).Select(Function(q) q.Value).FirstOrDefault()

                SetModalToolTip(e.Item, "omlCondSanitaria", codiceMalattia, descrizioneMalattia)
                SetLabelToolTip(e.Item, "lblCondSanitaria", codiceMalattia, descrizioneMalattia)

                ' Condizione rischio
                Dim codiceRischio As String = currentRow("ves_rsc_codice").ToString()
                Dim descrizioneRischio As String = ListCondizioniRischio.Where(Function(p) p.Key = codiceRischio).Select(Function(q) q.Value).FirstOrDefault()

                SetModalToolTip(e.Item, "omlCondRischio", codiceRischio, descrizioneRischio)
                SetLabelToolTip(e.Item, "lblCondRischio", codiceRischio, descrizioneRischio)

        End Select

    End Sub

    Private Sub dg_vacEff_SortCommand(source As Object, e As System.Web.UI.WebControls.DataGridSortCommandEventArgs) Handles dg_vacEff.SortCommand

        Dim imageUrlArrowDown As String = ResolveClientUrl("~/Images/arrow_down_small.gif")
        Dim imageUrlArrowUp As String = ResolveClientUrl("~/Images/arrow_up_small.gif")

        Select Case e.SortExpression

            Case "vac_descrizione"

                If ord(0) Then
                    dt_vacEseguite.DefaultView.Sort = GetSortExpression(e.SortExpression, "DESC")
                    strJS &= GetScriptModificaHeader("ordDesc", imageUrlArrowDown)
                    ord(0) = False
                Else
                    dt_vacEseguite.DefaultView.Sort = GetSortExpression(e.SortExpression, "ASC")
                    strJS &= GetScriptModificaHeader("ordDesc", imageUrlArrowUp)
                    ord(0) = True
                End If

            Case "ves_vac_codice"

                If ord(1) Then
                    dt_vacEseguite.DefaultView.Sort = GetSortExpression(e.SortExpression, "DESC")
                    strJS &= GetScriptModificaHeader("ordCod", imageUrlArrowDown)
                    ord(1) = False
                Else
                    dt_vacEseguite.DefaultView.Sort = GetSortExpression(e.SortExpression, "ASC")
                    strJS &= GetScriptModificaHeader("ordCod", imageUrlArrowUp)
                    ord(1) = True
                End If

            Case "ves_n_richiamo"

                If ord(2) Then
                    dt_vacEseguite.DefaultView.Sort = GetSortExpression(e.SortExpression, "DESC")
                    strJS &= GetScriptModificaHeader("ordDosi", imageUrlArrowDown)
                    ord(2) = False
                Else
                    dt_vacEseguite.DefaultView.Sort = GetSortExpression(e.SortExpression, "ASC")
                    strJS &= GetScriptModificaHeader("ordDosi", imageUrlArrowUp)
                    ord(2) = True
                End If

            Case "ves_mal_codice_cond_sanitaria"

                If ord(3) Then
                    dt_vacEseguite.DefaultView.Sort = GetSortExpression(e.SortExpression, "DESC")
                    strJS &= GetScriptModificaHeader("ordCondSan", imageUrlArrowDown)
                    ord(3) = False
                Else
                    dt_vacEseguite.DefaultView.Sort = GetSortExpression(e.SortExpression, "ASC")
                    strJS &= GetScriptModificaHeader("ordCondSan", imageUrlArrowUp)
                    ord(3) = True
                End If

            Case "ves_rsc_codice"

                If ord(4) Then
                    dt_vacEseguite.DefaultView.Sort = GetSortExpression(e.SortExpression, "DESC")
                    strJS &= GetScriptModificaHeader("ordCondRischio", imageUrlArrowDown)
                    ord(4) = False
                Else
                    dt_vacEseguite.DefaultView.Sort = GetSortExpression(e.SortExpression, "ASC")
                    strJS &= GetScriptModificaHeader("ordCondRischio", imageUrlArrowUp)
                    ord(4) = True
                End If

            Case "ves_data_effettuazione"

                If ord(5) Then
                    dt_vacEseguite.DefaultView.Sort = GetSortExpression(e.SortExpression, "DESC")
                    strJS &= GetScriptModificaHeader("ordData", imageUrlArrowDown)
                    ord(5) = False
                Else
                    dt_vacEseguite.DefaultView.Sort = GetSortExpression(e.SortExpression, "ASC")
                    strJS &= GetScriptModificaHeader("ordData", imageUrlArrowUp)
                    ord(5) = True
                End If

            Case "ass_descrizione"

                If ord(6) Then
                    dt_vacEseguite.DefaultView.Sort = GetSortExpression(e.SortExpression, "DESC")
                    strJS &= GetScriptModificaHeader("ordAss", imageUrlArrowDown)
                    ord(6) = False
                Else
                    dt_vacEseguite.DefaultView.Sort = GetSortExpression(e.SortExpression, "ASC")
                    strJS &= GetScriptModificaHeader("ordAss", imageUrlArrowUp)
                    ord(6) = True
                End If

            Case "ves_lot_codice"

                If ord(7) Then
                    dt_vacEseguite.DefaultView.Sort = GetSortExpression(e.SortExpression, "DESC")
                    strJS &= GetScriptModificaHeader("ordLotto", imageUrlArrowDown)
                    ord(7) = False
                Else
                    dt_vacEseguite.DefaultView.Sort = GetSortExpression(e.SortExpression, "ASC")
                    strJS &= GetScriptModificaHeader("ordLotto", imageUrlArrowUp)
                    ord(7) = True
                End If

            Case "noc_descrizione"
                If ord(8) Then
                    dt_vacEseguite.DefaultView.Sort = GetSortExpression(e.SortExpression, "DESC")
                    strJS &= GetScriptModificaHeader("ordNC", imageUrlArrowDown)
                    ord(8) = False
                Else
                    dt_vacEseguite.DefaultView.Sort = GetSortExpression(e.SortExpression, "ASC")
                    strJS &= GetScriptModificaHeader("ordNC", imageUrlArrowUp)
                    ord(8) = True
                End If

            Case "ope_nome"

                If ord(9) Then
                    dt_vacEseguite.DefaultView.Sort = GetSortExpression(e.SortExpression, "DESC")
                    strJS &= GetScriptModificaHeader("ordOp", imageUrlArrowDown)
                    ord(9) = False
                Else
                    dt_vacEseguite.DefaultView.Sort = GetSortExpression(e.SortExpression, "ASC")
                    strJS &= GetScriptModificaHeader("ordOp", imageUrlArrowUp)
                    ord(9) = True
                End If

            Case "cns_descrizione"

                If ord(10) Then
                    dt_vacEseguite.DefaultView.Sort = GetSortExpression(e.SortExpression, "DESC")
                    strJS &= GetScriptModificaHeader("ordCNS", imageUrlArrowDown)
                    ord(10) = False
                Else
                    dt_vacEseguite.DefaultView.Sort = GetSortExpression(e.SortExpression, "ASC")
                    strJS &= GetScriptModificaHeader("ordCNS", imageUrlArrowUp)
                    ord(10) = True
                End If

        End Select

        dg_vacEff.DataSource = dt_vacEseguite.DefaultView
        dg_vacEff.DataBind()

    End Sub

    Private Function GetSortExpression(sortExpression As String, order As String) As String

        Return String.Format("{0} {1}, vac_obbligatoria, vac_descrizione", sortExpression, order)

    End Function

    Private Function GetScriptModificaHeader(elementId As String, arrowImageUrl As String) As String

        Return String.Format("document.getElementById('{0}').style.display = 'inline';document.getElementById('{0}').src='{1}'{2}",
                             elementId,
                             arrowImageUrl,
                             Environment.NewLine)
    End Function

    Private Sub SetModalToolTip(datagriditem As DataGridItem, modalId As String, codice As String, descrizione As String)

        Dim control As Control = datagriditem.FindControl(modalId)
        If Not control Is Nothing Then

            Dim oml As OnitModalList = DirectCast(control, OnitModalList)

            If String.IsNullOrWhiteSpace(codice) Then
                oml.Codice = String.Empty
                oml.Descrizione = String.Empty
                oml.ToolTip = String.Empty
            Else
                oml.Codice = codice
                oml.Descrizione = descrizione
                oml.ToolTip = descrizione
            End If

        End If

    End Sub

    Private Sub SetLabelToolTip(datagriditem As DataGridItem, labelId As String, codice As String, descrizione As String)

        Dim control As Control = datagriditem.FindControl(labelId)
        If Not control Is Nothing Then

            Dim lbl As Label = DirectCast(control, Label)

            If String.IsNullOrWhiteSpace(codice) Then
                lbl.Text = String.Empty
                lbl.ToolTip = String.Empty
            Else
                lbl.Text = codice
                lbl.ToolTip = descrizione
            End If

        End If

    End Sub

#End Region

#Region " Toolbar "

    Private Sub Toolbar_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles Toolbar.ButtonClicked

        Select Case be.Button.Key

            Case "btn_Modifica"

                Modifica()

            Case "btn_Salva"

                Salva()

            Case "btn_Annulla"

                Ricarica()

        End Select

    End Sub

#End Region

#Region " Modali Condizione Sanitaria e Condizioni di Rischio "

    Protected Sub omlCondSanitaria_SetUpFiletr(sender As Object)

        Dim codiceVaccinazioneCorrente As String = GetCodiceVaccinazioneCurrentRow(sender, "omlCondSanitaria")

        Dim omlCondSanitaria As OnitModalList = DirectCast(sender, OnitModalList)

        omlCondSanitaria.Filtro =
            String.Format("VCS_PAZ_CODICE = {0} AND VCS_VAC_CODICE = '{1}' ORDER BY Paziente, Is_Default, Descrizione",
                          OnVacUtility.Variabili.PazId, codiceVaccinazioneCorrente)

    End Sub

    Protected Sub omlCondSanitaria_Change(Sender As Object, E As OnitModalList.ModalListaEventArgument)

        Dim omlCondSanitaria As OnitModalList = DirectCast(Sender, OnitModalList)
        omlCondSanitaria.ToolTip = omlCondSanitaria.Descrizione

    End Sub

    Protected Sub omlCondRischio_SetUpFiletr(sender As Object)

        Dim codiceVaccinazioneCorrente As String = GetCodiceVaccinazioneCurrentRow(sender, "omlCondRischio")

        Dim omlCondRischio As OnitModalList = DirectCast(sender, OnitModalList)

        omlCondRischio.Filtro =
            String.Format("VCR_PAZ_CODICE = {0} AND VCR_VAC_CODICE = '{1}' ORDER BY Paziente, Is_Default, Descrizione",
                          OnVacUtility.Variabili.PazId, codiceVaccinazioneCorrente)

    End Sub

    Protected Sub omlCondRischio_Change(Sender As Object, E As OnitModalList.ModalListaEventArgument)

        Dim omlCondRischio As OnitModalList = DirectCast(Sender, OnitModalList)
        omlCondRischio.ToolTip = omlCondRischio.Descrizione

    End Sub

    Private Function GetCodiceVaccinazioneCurrentRow(sender As Object, controlId As String) As String

        Dim codiceVaccinazioneCorrente As String = String.Empty

        Dim currentGridItem As DataGridItem = GetCurrentDGVacProgItem(sender, controlId)
        If Not currentGridItem Is Nothing Then

            codiceVaccinazioneCorrente = dt_vacEseguite.DefaultView(currentGridItem.ItemIndex)("ves_vac_codice").ToString()

        End If

        Return codiceVaccinazioneCorrente

    End Function

    Private Function GetCurrentDGVacProgItem(sender As Object, controlId As String) As DataGridItem

        For Each item As DataGridItem In dg_vacEff.Items

            If item.FindControl(controlId) Is sender Then
                Return item
            End If

        Next

        Return Nothing

    End Function

#End Region

#Region " Modifica Dose Vaccinazione "

    Private Sub Modifica()

        Dim gestioneStoricoVacc As New Common.OnVacStoricoVaccinaleCentralizzato(Settings)

        Dim listaStatiNonModificabili As List(Of String) = Nothing ' N.B. : era stato copiato il parametro Settings.AVRRT_STATI_VACC_NON_MODIFICABILI da OnVac di default, ma qui siamo in Veneto e AVRRT non c'è!!!

        Dim listCheckModificaDoseResult As New List(Of ModificaDoseResult)()

        ' Controllo ogni riga del datagrid delle vaccinazioni per decidere se è modificabile
        For i As Int16 = 0 To dg_vacEff.Items.Count - 1

            If Not dt_vacEseguite.DefaultView(i)("vra_rea_codice") Is DBNull.Value Then

                ' Presenza Reazione Avversa
                listCheckModificaDoseResult.Add(GetModificaDoseFailureResult(i, ModificaDoseResult.MotivoNoEditVaccinazione.ReazioneAvversa))

            ElseIf dt_vacEseguite.DefaultView(i)("scaduta").ToString() = "S" Then

                ' Vaccinazione Scaduta
                listCheckModificaDoseResult.Add(GetModificaDoseFailureResult(i, ModificaDoseResult.MotivoNoEditVaccinazione.VaccinazioneScaduta))

                ' ------------------------------------ '
                ' [Unificazione Ulss]: Eliminato controllo usl inserimento
                ' ------------------------------------ '
                'ElseIf FlagAbilitazioneVaccUslCorrente AndAlso Not gestioneStoricoVacc.CheckEseguitaStessaUsl(dt_vacEseguite.DefaultView(i).Row) Then

                '    ' Usl Inserimento diversa dalla Usl Corrente
                '    listCheckModificaDoseResult.Add(GetModificaDoseFailureResult(i, ModificaDoseResult.MotivoNoEditVaccinazione.AltraUslInserimento))
                ' ------------------------------------ '

            ElseIf Not OnVacUtility.CheckGiorniTrascorsiVariazioneDatiVaccinali(dt_vacEseguite.DefaultView(i)("ves_data_registrazione"), Settings) Then

                ' Trascorsi più giorni di quelli previsti dal parametro
                listCheckModificaDoseResult.Add(GetModificaDoseFailureResult(i, ModificaDoseResult.MotivoNoEditVaccinazione.NumeroGiorniVariazione))

            Else

                ' Tutto OK => dose modificabile
                listCheckModificaDoseResult.Add(GetModificaDoseSuccessResult(i))

                Dim txtDose As TextBox = DirectCast(dg_vacEff.Items(i).FindControl("txt_dose"), TextBox)

                txtDose.ReadOnly = False
                txtDose.CssClass = "TextBox_Stringa_Obbligatorio"

            End If

        Next

        Dim countCondizioniNoEdit As Integer = 0

        ' Controllo ogni riga del datagrid delle vaccinazioni per decidere se i campi condizione sanitaria e condizione di rischio sono modificabili
        For i As Int16 = 0 To dg_vacEff.Items.Count - 1

            If FlagAbilitazioneVaccUslCorrente AndAlso Not gestioneStoricoVacc.CheckEseguitaStessaUsl(dt_vacEseguite.DefaultView(i).Row) Then

                ' Usl Inserimento diversa dalla Usl Corrente
                countCondizioniNoEdit += 1

            ElseIf Not OnVacUtility.CheckGiorniTrascorsiVariazioneDatiVaccinali(dt_vacEseguite.DefaultView(i)("ves_data_registrazione"), Settings) Then

                ' Trascorsi più giorni di quelli previsti dal parametro
                countCondizioniNoEdit += 1

            ElseIf Not listaStatiNonModificabili Is Nothing AndAlso listaStatiNonModificabili.Count > 0 AndAlso listaStatiNonModificabili.Contains(dt_vacEseguite.DefaultView(i)("ves_stato").ToString()) Then

                ' Vaccinazione con stato non modificabile
                countCondizioniNoEdit += 1

            Else

                ' Tutto OK => campi modificabili
                EditCondizioneSanitaria(dg_vacEff.Items(i), True)
                EditCondizioneRischio(dg_vacEff.Items(i), True)

            End If

        Next

        Dim numRigheInEdit As Int16 = listCheckModificaDoseResult.Where(Function(p) p.Success).Count()

        If numRigheInEdit = 0 Then

            ' Nessuna riga in edit, visualizzo un alert per indicare il motivo, riga per riga.
            ShowRiepilogo(listCheckModificaDoseResult, String.Empty, True)

        Else

            If numRigheInEdit < dg_vacEff.Items.Count Then

                ' Se alcune righe non possono essere in edit, visualizzo un alert per indicare quali e il motivo.
                Dim messaggioAggiornamentoCentrale As String = String.Empty

                ' Controllo per visualizzare alert solo se la usl corrente ha dato visibilità
                ' [Unificazione Ulss]: i flag Consenso e Abilitazione hanno sempre valore false per le ulss unificate => OK
                If FlagAbilitazioneVaccUslCorrente AndAlso FlagConsensoVaccUslCorrente Then
                    messaggioAggiornamentoCentrale = Common.OnVacStoricoVaccinaleCentralizzato.MessaggioAggiornamentoAnagrafeCentrale
                End If

                ShowRiepilogo(listCheckModificaDoseResult, messaggioAggiornamentoCentrale, False)

            End If

            SetEnableToolbarButton("btn_Salva", True)
            SetEnableToolbarButton("btn_Annulla", True)
            SetEnableToolbarButton("btn_Modifica", False)

        End If

    End Sub

    Private Class ModificaDoseResult

        Public Success As Boolean
        Public Vaccinazione As String
        Public Dose As String
        Public Riga As Int16
        Public MotivoNoEdit As MotivoNoEditVaccinazione

        Public Enum MotivoNoEditVaccinazione
            Nessuno = 0
            ReazioneAvversa = 1
            VaccinazioneScaduta = 2
            AltraUslInserimento = 3
            NumeroGiorniVariazione = 4
        End Enum

    End Class

    Private Function GetModificaDoseFailureResult(rowIndex As Int16, motivoNoEditVaccinazione As ModificaDoseResult.MotivoNoEditVaccinazione) As ModificaDoseResult

        Return New ModificaDoseResult() With {
            .Success = False,
            .Vaccinazione = dt_vacEseguite.DefaultView(rowIndex)("ves_vac_codice").ToString(),
            .Dose = dt_vacEseguite.DefaultView(rowIndex)("ves_n_richiamo").ToString(),
            .Riga = rowIndex + 1,
            .MotivoNoEdit = motivoNoEditVaccinazione
        }

    End Function

    Private Function GetModificaDoseSuccessResult(rowIndex As Int16) As ModificaDoseResult

        Return New ModificaDoseResult() With {
            .Success = True,
            .Vaccinazione = dt_vacEseguite.DefaultView(rowIndex)("ves_vac_codice").ToString(),
            .Dose = dt_vacEseguite.DefaultView(rowIndex)("ves_n_richiamo").ToString(),
            .Riga = rowIndex + 1,
            .MotivoNoEdit = ModificaDoseResult.MotivoNoEditVaccinazione.Nessuno
        }

    End Function

    Private Sub ShowRiepilogo(listModificaDoseResult As List(Of ModificaDoseResult), messaggioAggiornamentoCentrale As String, noVaccinazioniInEdit As Boolean)

        If listModificaDoseResult Is Nothing OrElse listModificaDoseResult.Count = 0 Then Return

        Dim listModificaDoseFailureResult As List(Of ModificaDoseResult) = listModificaDoseResult.Where(Function(p) Not p.Success).ToList()

        If listModificaDoseFailureResult Is Nothing OrElse listModificaDoseFailureResult.Count = 0 Then Return

        ' Messaggio centrale
        lblMessaggioAggiornamentoCentrale.Visible = False
        If Settings.ALERT_AGGIORNAMENTO_DATI_CENTRALIZZATI AndAlso Not String.IsNullOrEmpty(messaggioAggiornamentoCentrale) Then
            lblMessaggioAggiornamentoCentrale.Visible = True
            lblMessaggioAggiornamentoCentrale.Text = "ATTENZIONE: " + messaggioAggiornamentoCentrale
        End If

        ' Intestazione messaggio
        If noVaccinazioniInEdit Then
            lblIntestazioneRiepilogo.Text = "Nessuna vaccinazione puo' essere modificata:"
        Else
            lblIntestazioneRiepilogo.Text = "Le seguenti vaccinazioni non possono essere modificate, in base al motivo indicato:"
        End If

        ' Messaggio di riepilogo vaccinazioni non in edit
        Dim messaggioRiepilogo As New System.Text.StringBuilder()

        messaggioRiepilogo.Append(GetMessaggioMotivoNoEdit(listModificaDoseFailureResult, ModificaDoseResult.MotivoNoEditVaccinazione.ReazioneAvversa))
        messaggioRiepilogo.Append(GetMessaggioMotivoNoEdit(listModificaDoseFailureResult, ModificaDoseResult.MotivoNoEditVaccinazione.VaccinazioneScaduta))
        messaggioRiepilogo.Append(GetMessaggioMotivoNoEdit(listModificaDoseFailureResult, ModificaDoseResult.MotivoNoEditVaccinazione.AltraUslInserimento))
        messaggioRiepilogo.Append(GetMessaggioMotivoNoEdit(listModificaDoseFailureResult, ModificaDoseResult.MotivoNoEditVaccinazione.NumeroGiorniVariazione))

        lblMessaggioRiepilogo.Text = messaggioRiepilogo.ToString()

        modRiepilogo.VisibileMD = True

    End Sub

    Private Function GetMessaggioMotivoNoEdit(listModificaDoseFailureResult As List(Of ModificaDoseResult), motivoNoEdit As ModificaDoseResult.MotivoNoEditVaccinazione) As String

        Dim messaggio As New System.Text.StringBuilder()

        Dim listModificaDoseMotivoResult As List(Of ModificaDoseResult) =
            listModificaDoseFailureResult.Where(Function(p) p.MotivoNoEdit = motivoNoEdit).ToList()

        If Not listModificaDoseMotivoResult Is Nothing AndAlso listModificaDoseMotivoResult.Count > 0 Then

            Select Case motivoNoEdit
                Case ModificaDoseResult.MotivoNoEditVaccinazione.ReazioneAvversa
                    messaggio.Append(" <b>Reazione avversa presente:</b><br />")
                Case ModificaDoseResult.MotivoNoEditVaccinazione.VaccinazioneScaduta
                    messaggio.Append(" <b>Vaccinazione scaduta:</b><br />")
                Case ModificaDoseResult.MotivoNoEditVaccinazione.AltraUslInserimento
                    messaggio.Append(" <b>Vaccinazione inserita da altra azienda:</b><br />")
                Case ModificaDoseResult.MotivoNoEditVaccinazione.NumeroGiorniVariazione
                    messaggio.AppendFormat(" <b>Vaccinazione inserita da piu' di {0} giorni:</b><br />", Settings.GIORNI_VARIAZIONE_VACCINAZIONE_ESEGUITA.ToString())
            End Select

            messaggio.Append("<div style='margin-left:15px; margin-bottom:5px'>")

            For Each modificaDoseMotivoResult As ModificaDoseResult In listModificaDoseMotivoResult

                messaggio.AppendFormat("Riga {0}: vaccinazione {1} ({2})<br />",
                                       modificaDoseMotivoResult.Riga.ToString(),
                                       modificaDoseMotivoResult.Vaccinazione,
                                       modificaDoseMotivoResult.Dose)
            Next

            messaggio.Append("</div>")

        End If

        Return messaggio.ToString()

    End Function

#End Region

#Region " Private Methods "

    Private Sub SetEnableToolbarButton(buttonKey As String, enable As Boolean)

        Toolbar.Items.FromKeyButton(buttonKey).Enabled = enable

    End Sub

    Private Sub Ricarica()

        LoadModale()

        For i As Integer = 0 To dg_vacEff.Items.Count - 1

            Dim txtDose As TextBox = DirectCast(dg_vacEff.Items(i).FindControl("txt_dose"), TextBox)

            txtDose.ReadOnly = True
            txtDose.CssClass = "textboxVacEs_noEdit"

        Next

    End Sub

    Private Sub EditCondizioneSanitaria(gridItem As DataGridItem, edit As Boolean)

        Dim lblCondSanitaria As Label = DirectCast(gridItem.FindControl("lblCondSanitaria"), Label)
        lblCondSanitaria.Visible = Not edit

        Dim omlCondSanitaria As OnitModalList = DirectCast(gridItem.FindControl("omlCondSanitaria"), OnitModalList)
        omlCondSanitaria.Visible = edit

    End Sub

    Private Sub EditCondizioneRischio(gridItem As DataGridItem, edit As Boolean)

        Dim lblCondRischio As Label = DirectCast(gridItem.FindControl("lblCondRischio"), Label)
        lblCondRischio.Visible = Not edit

        Dim omlCondRischio As OnitModalList = DirectCast(gridItem.FindControl("omlCondRischio"), OnitModalList)
        omlCondRischio.Visible = edit

    End Sub

    ' N.B. : questa ricerca è in LOCALE perchè lo user control viene utilizzato solo dalla pagina locale e non in modalità centrale.
    Private Sub Salva()

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, OnVacUtility.GetReadCommittedTransactionOptions)

            Dim errorMessage As String = ControllaDose()

            If Not String.IsNullOrWhiteSpace(errorMessage) Then

                Dim str As String = String.Format("<script language=""javascript"">{0}{1}{0}</script>", Environment.NewLine, errorMessage)
                Response.Write(str)

            Else

                For i As Integer = 0 To dg_vacEff.Items.Count - 1

                    Dim codiceVaccinazione As String = DirectCast(dg_vacEff.Items(i).FindControl("lblVacCodice"), Label).Text
                    Dim dataEffettuazione As Date = CDate(DirectCast(dg_vacEff.Items(i).FindControl("lblDataEffettuazione"), Label).Text)
                    Dim dose As Integer = CInt(DirectCast(dg_vacEff.Items(i).FindControl("txt_dose"), TextBox).Text)
                    Dim controlRischio As Control = dg_vacEff.Items(i).FindControl("omlCondRischio")
                    Dim controlSanitaria As Control = dg_vacEff.Items(i).FindControl("omlCondSanitaria")

                    If DirectCast(dt_vacEseguite.DefaultView(i)("ves_vac_codice"), String) <> codiceVaccinazione Then
                        Throw New ApplicationException("Il datatable in memoria non corrisponde ai dati restituiti dal client")
                    End If

                    If DirectCast(dt_vacEseguite.DefaultView(i)("ves_data_effettuazione"), Date) <> dataEffettuazione Then
                        Throw New ApplicationException("Il datatable in memoria non corrisponde ai dati restituiti dal client")
                    End If

                    If DirectCast(dt_vacEseguite.DefaultView(i)("ves_n_richiamo"), Decimal) <> dose Then
                        dt_vacEseguite.DefaultView(i)("ves_n_richiamo") = dose
                    End If

                    If Not controlSanitaria Is Nothing AndAlso controlSanitaria.Visible Then

                        Dim condizioneSanitaria As String = DirectCast(controlSanitaria, OnitModalList).Codice

                        If dt_vacEseguite.DefaultView(i)("ves_mal_codice_cond_sanitaria").ToString() <> condizioneSanitaria Then
                            dt_vacEseguite.DefaultView(i)("ves_mal_codice_cond_sanitaria") = condizioneSanitaria
                        End If

                    End If

                    If Not controlRischio Is Nothing AndAlso controlRischio.Visible Then

                        Dim condizioneRischio As String = DirectCast(controlRischio, OnitModalList).Codice

                        If dt_vacEseguite.DefaultView(i)("ves_rsc_codice").ToString() <> condizioneRischio Then
                            dt_vacEseguite.DefaultView(i)("ves_rsc_codice") = condizioneRischio
                        End If

                    End If

                Next

                Dim vaccinazioneEseguitaList As New List(Of Entities.VaccinazioneEseguita)
                Dim vaccinazioneEseguitaEliminataList As New List(Of Entities.VaccinazioneEseguita)

                Using dbGenericProviderFactory As New DbGenericProviderFactory()

                    Using bizVaccinazioniEseguite As New Biz.BizVaccinazioniEseguite(dbGenericProviderFactory, Settings, OnVacContext.CreateBizContextInfos(), OnVacUtility.CreateBizLogOptions(DataLogStructure.TipiArgomento.VAC_ESEGUITE))
                        bizVaccinazioniEseguite.Salva(OnVacUtility.Variabili.PazId, dt_vacEseguite)
                    End Using

                    Dim pazienteCorrente As Entities.Paziente = Nothing
                    Using bizPaziente As New BizPaziente(dbGenericProviderFactory, Settings, OnVacContext.CreateBizContextInfos(), Nothing)
                        pazienteCorrente = bizPaziente.CercaPaziente(OnVacUtility.Variabili.PazId)
                    End Using

                    OnVacMidSendManager.ModificaPaziente(pazienteCorrente, dt_vacEseguite)

                End Using

                Ricarica()

            End If

            transactionScope.Complete()

        End Using

        RaiseEvent DosiModificate()

    End Sub

    'Controllo che la dose modificata sia corretta
    Private Function ControllaDose() As String

        Dim listVaccinazioniErrate As New List(Of VacNoEseguita)
        Dim strJs As New System.Text.StringBuilder()

        For Each item As DataGridItem In dg_vacEff.Items

            Dim errorMessage As String = String.Empty
            Dim dose As Integer = 0

            Dim value As String = DirectCast(item.FindControl("txt_dose"), TextBox).Text.Trim()

            If String.IsNullOrWhiteSpace(value) Then
                errorMessage = "le vaccinazioni non possono avere dose nulla"
            Else
                If Integer.TryParse(value, dose) Then
                    If dose <= 0 Then
                        errorMessage = "la dose deve essere maggiore di 0"
                    End If
                Else
                    errorMessage = "La dose deve essere un valore numerico superiore a 0"
                End If
            End If

            If Not String.IsNullOrWhiteSpace(errorMessage) Then
                Return String.Format("alert('Modifiche non effettuate: {0}');", errorMessage)
            End If

        Next

        For i As Integer = 0 To dg_vacEff.Items.Count - 2

            If dt_vacEseguite.DefaultView(i)("scaduta").ToString() <> "S" Then

                Dim codiceVaccinazione As String = DirectCast(dg_vacEff.Items(i).FindControl("lblVacCodice"), Label).Text
                Dim dataEffettuazione As Date = CDate(DirectCast(dg_vacEff.Items(i).FindControl("lblDataEffettuazione"), Label).Text)
                Dim dose As Integer = Convert.ToInt32(DirectCast(dg_vacEff.Items(i).FindControl("txt_dose"), TextBox).Text)

                For j As Integer = i + 1 To dg_vacEff.Items.Count - 1

                    'Controllo che la dose non sia già esistente
                    If dt_vacEseguite.DefaultView.Item(j)("scaduta").ToString() <> "S" Then

                        Dim curr_vac_codice As String = DirectCast(dg_vacEff.Items(j).FindControl("lblVacCodice"), Label).Text
                        Dim curr_vac_desc As String = DirectCast(dg_vacEff.Items(j).FindControl("lblVacDesc"), Label).Text
                        Dim curr_data_effettuazione As Date = CDate(DirectCast(dg_vacEff.Items(j).FindControl("lblDataEffettuazione"), Label).Text)
                        Dim curr_dose As Integer = Convert.ToInt32(DirectCast(dg_vacEff.Items(j).FindControl("txt_dose"), TextBox).Text)

                        If codiceVaccinazione = curr_vac_codice AndAlso dose = curr_dose Then
                            listVaccinazioniErrate.Add(New VacNoEseguita(curr_vac_desc, dose, ErroreEx.StessaDose))
                        End If

                        'Controllo che non venga inserita una dose inferiore con una data successiva
                        If codiceVaccinazione = curr_vac_codice AndAlso dose > curr_dose AndAlso dataEffettuazione < curr_data_effettuazione Then
                            listVaccinazioniErrate.Add(New VacNoEseguita(curr_vac_desc, dose, ErroreEx.DataPrimaSedPrec))
                        End If

                        'Controllo che non venga inserita una dose superiore in una data precedente
                        If codiceVaccinazione = curr_vac_codice AndAlso dose < curr_dose AndAlso dataEffettuazione > curr_data_effettuazione Then
                            listVaccinazioniErrate.Add(New VacNoEseguita(curr_vac_desc, dose, ErroreEx.DataDopoSedSucc))
                        End If

                    End If

                Next
            End If

        Next

        If listVaccinazioniErrate.Count > 0 Then

            strJs.Append("alert(""Impossibile eseguire le seguenti vaccinazioni:\n")

            For i As Integer = 0 To listVaccinazioniErrate.Count - 1

                strJs.AppendFormat("{0} dose n.{1}: ", listVaccinazioniErrate(i).DescrizioneVaccinazione, listVaccinazioniErrate(i).DoseVaccinazione.ToString())

                Select Case listVaccinazioniErrate(i).Errore
                    Case ErroreEx.DataDopoSedSucc
                        strJs.Append("Data maggiore rispetto a quella della dose successiva\n")
                    Case ErroreEx.DataPrimaSedPrec
                        strJs.Append("Data inferiore rispetto a quella della dose precedente\n")
                    Case ErroreEx.StessaDose
                        strJs.Append("Esiste una vaccinazione dello stesso tipo con la stessa dose tra le eseguite\n")
                End Select

            Next

            strJs.Append(""");")

        End If

        Return strJs.ToString()

    End Function

#End Region

End Class
