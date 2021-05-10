Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager

Public Class DettaglioLottoMagazzinoCentrale
    Inherits OnVac.Common.PageBase

#Region " Properties "

    Private Property CodiceLottoSelezionato() As String
        Get
            If ViewState("CodiceLottoSelezionato") Is Nothing Then
                ViewState("CodiceLottoSelezionato") = Request.QueryString.Get("codSel")
            End If

            Return ViewState("CodiceLottoSelezionato")
        End Get
        Set(value As String)
            ViewState("CodiceLottoSelezionato") = value
        End Set
    End Property

    Private Property StatoPagina() As MagazzinoEnums.StatiPagina
        Get
            If ViewState("StatoPaginaDettaglio") Is Nothing Then
                ViewState("StatoPaginaDettaglio") = MagazzinoEnums.StatiPagina.VisualizzazioneDati
            End If

            Return ViewState("StatoPaginaDettaglio")
        End Get
        Set(value As MagazzinoEnums.StatiPagina)
            ViewState("StatoPaginaDettaglio") = value
        End Set
    End Property

#End Region

#Region " Classes "

    Private Class OnitLayout31ConfirmKeys
        Public Shared ReadOnly Property ConfirmSequestroLotto() As String
            Get
                Return "ConfirmSequestroLotto"
            End Get
        End Property
    End Class

#End Region

#Region " Page Events "

    Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        MagazzinoUtility.SetToolbarButtonImages("btnIndietro", "prev.gif", ToolBar)
        MagazzinoUtility.SetToolbarButtonImages("btnSalva", "salva.gif", ToolBar)
        MagazzinoUtility.SetToolbarButtonImages("btnAnnulla", "annulla.gif", ToolBar)

    End Sub

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then

            Me.ucDatiLotto.IsMagazzinoCentrale = True

            ' Lettura querystring per apertura in inserimento
            If String.IsNullOrEmpty(Me.CodiceLottoSelezionato) Then

                Me.SetDatiInserimentoLotto()

                Me.SetLayoutState(MagazzinoEnums.StatiPagina.InserimentoDati)

            Else

                ' Caricamento dati lotto
                Me.CaricamentoDatiLotto(Me.CodiceLottoSelezionato)

                ' Layout pagina
                Me.SetLayoutState(MagazzinoEnums.StatiPagina.VisualizzazioneDati)

            End If

        End If

    End Sub

#End Region

#Region " Toolbar Events "

    Private Sub ToolBar_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case be.Button.Key

            Case "btnIndietro"

                Me.RedirectToRicerca()

            Case "btnModifica"

                Me.SetLayoutState(MagazzinoEnums.StatiPagina.ModificaDati)

            Case "btnSalva"

                If Me.ConfermaSequestro() Then

                    ' Fa scattare l'evento ConfirmClick dell'OnitLayout31
                    Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(
                                               "Attenzione: il lotto verrà reso inutilizzabile per tutti i centri vaccinali. Continuare?",
                                               OnitLayout31ConfirmKeys.ConfirmSequestroLotto, True, True))
                Else

                    Me.Salva()

                End If

            Case "btnAnnulla"

                If String.IsNullOrEmpty(Me.CodiceLottoSelezionato) Then

                    Me.RedirectToRicerca()

                Else

                    ' Caricamento dati lotto
                    Me.CaricamentoDatiLotto(Me.CodiceLottoSelezionato)

                    Me.SetLayoutState(MagazzinoEnums.StatiPagina.VisualizzazioneDati)

                End If

        End Select

    End Sub

    Private Sub RedirectToRicerca()

        Dim codice As String = Me.CodiceLottoSelezionato

        Me.CodiceLottoSelezionato = String.Empty

        Server.Transfer(MagazzinoUtility.GetRedirectUrl(".\MagazzinoCentrale.aspx", Request.QueryString, codice))

    End Sub

#End Region

#Region " OnitLayout Events "

    Private Sub OnitLayout31_ConfirmClick(sender As Object, e As Controls.PagesLayout.OnitLayout3.ConfirmEventArgs) Handles OnitLayout31.ConfirmClick

        Select Case e.Key

            Case OnitLayout31ConfirmKeys.ConfirmSequestroLotto

                ' Richiesta di continuare con il salvataggio del lotto sequestrato: 
                ' prevede l'obsolescenza del lotto su tutti i consultori
                If e.Result Then
                    Me.Salva()
                End If

        End Select

    End Sub

#End Region

#Region " Layout "

    Private Sub SetLayoutState(stato As MagazzinoEnums.StatiPagina)

        Me.StatoPagina = stato

        ' Toolbar
        Me.SetToolbarState(stato)

        ' Dati lotto
        Select Case stato

            Case MagazzinoEnums.StatiPagina.VisualizzazioneDati
                ' uc e datagrid disabilitati
                Me.ucDatiLotto.SetEnable(False)

                Me.OnitLayout31.Busy = False

            Case MagazzinoEnums.StatiPagina.InserimentoDati, MagazzinoEnums.StatiPagina.ModificaDati
                ' uc abilitato, datagrid disabilitato
                Me.ucDatiLotto.SetEnable(True)

                Page.ClientScript.RegisterStartupScript(Me.GetType(), "jsFocusLotto", ucDatiLotto.GetJSFocusLotto())

                Me.OnitLayout31.Busy = True

        End Select

    End Sub

    Private Sub SetToolbarState(stato As MagazzinoEnums.StatiPagina)

        Me.ToolBar.Items.FromKeyButton("btnIndietro").Enabled = (stato = MagazzinoEnums.StatiPagina.VisualizzazioneDati)
        Me.ToolBar.Items.FromKeyButton("btnModifica").Enabled = (stato = MagazzinoEnums.StatiPagina.VisualizzazioneDati)
        Me.ToolBar.Items.FromKeyButton("btnSalva").Enabled = (stato <> MagazzinoEnums.StatiPagina.VisualizzazioneDati)
        Me.ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = (stato <> MagazzinoEnums.StatiPagina.VisualizzazioneDati)

    End Sub

#End Region

#Region " Dati lotto "

    ' Inserimento
    Private Sub SetDatiInserimentoLotto()

        Me.ucDatiLotto.Modalita = InsDatiLotto.Mode.Nuovo

        Dim filtroLottiMagazzino As Filters.FiltriRicercaLottiMagazzino = MagazzinoUtility.GetFiltersFromQueryString(Request.QueryString)

        Dim newLottoMagazzino As New Entities.LottoMagazzino()
        newLottoMagazzino.CodiceLotto = filtroLottiMagazzino.CodiceLotto
        newLottoMagazzino.DescrizioneLotto = filtroLottiMagazzino.DescrizioneLotto
        newLottoMagazzino.CodiceNomeCommerciale = filtroLottiMagazzino.CodiceNomeCommerciale
        newLottoMagazzino.DescrizioneNomeCommerciale = filtroLottiMagazzino.DescrizioneNomeCommerciale

        Me.ucDatiLotto.SetLottoMagazzino(newLottoMagazzino)

    End Sub

    ' Caricamento dati del lotto e impostazione valori nello user control
    Private Sub CaricamentoDatiLotto(codiceLotto As String)

        Dim lottoMagazzino As Entities.LottoMagazzino = Nothing
        Dim filtroLottiMagazzino As Filters.FiltriRicercaLottiMagazzino = MagazzinoUtility.GetFiltersFromQueryString(Request.QueryString)
        Using dam As IDAM = OnVacUtility.OpenDam()
            Using genericProvider As New DAL.DbGenericProvider(dam)
                Using bizLotti As New Biz.BizLotti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                    lottoMagazzino = bizLotti.LoadLottoMagazzinoCentrale(codiceLotto, OnVacContext.UserId, filtroLottiMagazzino.CodiceDistretto)

                End Using
            End Using
        End Using

        If lottoMagazzino Is Nothing Then lottoMagazzino = New Entities.LottoMagazzino()

        ' nel magazzino centrale non è gestito
        lottoMagazzino.Attivo = True

        ' N.B. : età min max => nel magazzino centrale non sono gestite, sono sempre Nothing

        Me.ucDatiLotto.Modalita = InsDatiLotto.Mode.Modifica
        Me.ucDatiLotto.SetLottoMagazzino(lottoMagazzino)

    End Sub

#End Region

#Region " Salvataggio "

    Private Sub Salva()

        If Me.SalvaDatiLotto() Then

            Me.CodiceLottoSelezionato = Me.ucDatiLotto.GetCodiceLotto()

            Me.CaricamentoDatiLotto(Me.CodiceLottoSelezionato)

            Me.SetLayoutState(MagazzinoEnums.StatiPagina.VisualizzazioneDati)

        End If

    End Sub

    Private Function SalvaDatiLotto() As Boolean

        Dim resultMessage As String = String.Empty

        Dim bizLottiResult As Biz.BizLotti.BizLottiResult

        Dim lottoMagazzino As Entities.LottoMagazzino = Me.ucDatiLotto.GetLottoMagazzino()

        Using dam As IDAM = OnVacUtility.OpenDam()
            Using genericProvider As New DAL.DbGenericProvider(dam)
                Using bizLotti As New Biz.BizLotti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                    Dim listTestateLog As New List(Of Log.DataLogStructure.Testata)()

                    If Me.StatoPagina = MagazzinoEnums.StatiPagina.InserimentoDati Then

                        ' Inserimento lotto.
                        ' Viene inserito il lotto in anagrafica lotti e scritto il log (tranne che in caso di errore).
                        ' Non viene impostato nessun record nella lotto-consultorio nè un primo carico.
                        ' Non viene gestita l'attivazione/disattivazione del lotto.
                        bizLottiResult = bizLotti.InsertLottoMagazzinoCentrale(lottoMagazzino)
                        resultMessage = bizLottiResult.Message

                    Else

                        ' Modifica anagrafica del lotto e scrittura del log (tranne che in caso di errore)
                        bizLottiResult = bizLotti.UpdateLottoMagazzinoCentrale(lottoMagazzino)
                        resultMessage = bizLottiResult.Message

                    End If

                End Using
            End Using
        End Using

        If Not String.IsNullOrEmpty(resultMessage) Then
            Me.OnitLayout31.InsertRoutineJS(String.Format("alert(""{0}"");", resultMessage))
        End If

        Return (bizLottiResult.Result <> Biz.BizLotti.BizLottiResult.ResultType.GenericError)

    End Function

#End Region

#Region " Private Methods "

    Private Sub SetDatagridItemLabel(datagridItem As DataGridItem, id As String, value As String)

        Dim ctrl As Control = datagridItem.FindControl(id)

        If Not ctrl Is Nothing Then

            DirectCast(ctrl, Label).Text = value

        End If

    End Sub

    Private Sub SetDatagridItemModalList(datagridItem As DataGridItem, id As String, codice As String)

        Dim ctrl As Control = datagridItem.FindControl(id)

        If Not ctrl Is Nothing Then

            Dim fm As Onit.Controls.OnitModalList = DirectCast(ctrl, Onit.Controls.OnitModalList)

            fm.Codice = codice

            fm.RefreshDataBind()

        End If

    End Sub

    Private Function GetDateTimeFromString(value As String) As DateTime

        Dim dataScadenza As DateTime

        If String.IsNullOrEmpty(value) Then
            dataScadenza = Date.MinValue
        Else
            Try
                dataScadenza = Convert.ToDateTime(value)
            Catch ex As Exception
                dataScadenza = Date.MinValue
            End Try
        End If

        Return dataScadenza

    End Function

    ' Richiesta di conferma del sequestro del lotto:
    ' deve avvenire solo se il lotto originale non era sequestrato e il lotto modificato è sequestrato,
    ' oppure se il lotto è nuovo (appena inserito) ed è stato spuntato il flag di sequestro.
    Private Function ConfermaSequestro() As Boolean

        Dim lottoMagazzino As Entities.LottoMagazzino = Me.ucDatiLotto.GetLottoMagazzino()

        ' Se tra i dati correnti del lotto, il check del sequestro non è stato spuntato, 
        ' non deve chiedere conferma del sequestro
        If Not lottoMagazzino.Obsoleto Then Return False

        ' Caricamento dati originali del lotto
        Dim lottoMagazzinoOriginale As Entities.LottoMagazzino = Nothing

        Using dam As IDAM = OnVacUtility.OpenDam()
            Using genericProvider As New DAL.DbGenericProvider(dam)
                Using bizLotti As New Biz.BizLotti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                    lottoMagazzinoOriginale = bizLotti.LoadLottoMagazzinoCentrale(lottoMagazzino.CodiceLotto, OnVacContext.UserId, String.Empty)

                End Using
            End Using
        End Using

        If lottoMagazzinoOriginale Is Nothing Then

            ' In caso di nuovo lotto chiedo sempre la conferma del sequestro 
            Return True

        Else

            ' In caso di lotto già esistente, chiedo la conferma del sequestro se l'originale su db non era già sequestrato
            If Not lottoMagazzinoOriginale.Obsoleto Then Return True

        End If

        Return False

    End Function

#End Region

End Class