Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager

Partial Class OnVac_Bilanci_Osservazioni
    Inherits OnVac.Common.PageBase

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

#Region " Types "

    Private Enum GridOsservazioniColumnIndex
        Selezione = 0
        Codice = 1
        Descrizione = 2
        TipoRisposta = 3
    End Enum

    Private Enum GridOsservazioniAssociateColumnIndex
        Ordine = 0
        Selezione = 1
        Codice = 2
        Descrizione = 3
        Sezione = 4
        Obbligatorio = 5
        Condizioni = 6
        TipoRisposta = 7
    End Enum

    Private Enum StatoPagina
        View = 0
        Edit = 1
    End Enum

#End Region

#Region " Properties "

    Private Property NumeroBilancio() As Integer
        Get
            Return ViewState("NumBil")
        End Get
        Set(Value As Integer)
            ViewState("NumBil") = Value
        End Set
    End Property

    Private Property CodiceMalattia() As String
        Get
            Return ViewState("CodiceMalattia")
        End Get
        Set(Value As String)
            ViewState("CodiceMalattia") = Value
        End Set
    End Property

    Property OsservazioneSelezionata() As String
        Get
            Return ViewState("OsservazioneSelezionata")
        End Get
        Set(Value As String)
            ViewState("OsservazioneSelezionata") = Value
        End Set
    End Property

    Private Property dtaCondizioni() As DataTable
        Get
            Return Session("dtaCondizioni")
        End Get
        Set(Value As DataTable)
            Session("dtaCondizioni") = Value
        End Set
    End Property

    Private Property Sezioni As List(Of Entities.BilancioSezione)
        Get
            If ViewState("SEZ") Is Nothing Then ViewState("SEZ") = New List(Of Entities.BilancioSezione)()
            Return ViewState("SEZ")
        End Get
        Set(value As List(Of Entities.BilancioSezione))
            ViewState("SEZ") = value
        End Set
    End Property

    Private Property StatoCorrentePagina() As StatoPagina
        Get
            If ViewState("statoPagina") Is Nothing Then ViewState("statoPagina") = StatoPagina.View
            Return ViewState("statoPagina")
        End Get
        Set(Value As StatoPagina)
            ViewState("statoPagina") = Value
        End Set
    End Property

#End Region

#Region " Page Events "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack Then

            SetLayout(StatoPagina.View)

            Dim codice As String() = Request.QueryString("CODICE").Split("|")

            'Reperisco il nome del bilancio dal codice
            If codice.GetUpperBound(0) >= 0 Then
                Me.NumeroBilancio = Convert.ToInt32(codice(0))
                Me.CodiceMalattia = codice(1)
                Me.titolo.InnerText = codice(2) & " - " & codice(0)
            End If

            LoadSezioni()

            LoadOsservazioni()

        End If

    End Sub

#End Region

#Region " Controls Events "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btnAggiungi"

                AddOsservazioni()

            Case "btnElimina"

                DeleteOsservazioni()

            Case "btnSalva"

                Dim result As Biz.BizGenericResult = SalvaOsservazioni()

                If Not String.IsNullOrWhiteSpace(result.Message) Then
                    Me.OnitLayout31.InsertRoutineJS(String.Format("alert('{0}');", HttpUtility.JavaScriptStringEncode(result.Message)))
                End If

                If result.Success Then
                    SetLayout(StatoPagina.View)
                    LoadOsservazioni()
                End If

            Case "btnAnnulla"

                SetLayout(StatoPagina.View)
                LoadOsservazioni()

            Case "btnIndietro"

                Response.Redirect("Bilanci.aspx", False)

        End Select

    End Sub

    Private Sub tlbCondizioni_ButtonClicked(sender As System.Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles tlbCondizioni.ButtonClicked

        Select Case be.Button.Key

            Case "btnAggiungi"

                CreaCondizione()

            Case "btnElimina"

                EliminaCondizioni()

            Case "btnAnnulla"

                Me.fmCondizioni.VisibileMD = False

        End Select

    End Sub

    Private Sub dgrOsservazioniAssociate_ItemCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgrOsservazioniAssociate.ItemCommand

        Select Case e.CommandName

            Case "MoveUp"

                SwitchOsservazioniAssociate(e.Item.ItemIndex, e.Item.ItemIndex - 1)

            Case "MoveDown"

                SwitchOsservazioniAssociate(e.Item.ItemIndex, e.Item.ItemIndex + 1)

            Case "SetCondizioni"

                Me.OsservazioneSelezionata = HttpUtility.HtmlDecode(e.Item.Cells(GridOsservazioniAssociateColumnIndex.Codice).Text).Trim()

                CaricaCondizioni(Me.OsservazioneSelezionata)
                CaricaComboCondizioni(Me.OsservazioneSelezionata)

                Me.fmCondizioni.Title = String.Format("Condizioni per {0}", HttpUtility.HtmlDecode(e.Item.Cells(GridOsservazioniAssociateColumnIndex.Descrizione).Text).Trim())
                Me.fmCondizioni.VisibileMD = True

        End Select

    End Sub

    Private Sub dgrOsservazioniAssociate_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgrOsservazioniAssociate.ItemDataBound

        Select Case e.Item.ItemType

            Case ListItemType.Item, ListItemType.AlternatingItem, ListItemType.EditItem, ListItemType.SelectedItem

                Dim list As List(Of Entities.BilancioOsservazioneAssociata) = DirectCast(dgrOsservazioniAssociate.DataSource, List(Of Entities.BilancioOsservazioneAssociata))

                If Not list.IsNullOrEmpty() Then

                    Dim osservazione As Entities.BilancioOsservazioneAssociata =
                        DirectCast(e.Item.DataItem, Entities.BilancioOsservazioneAssociata)

                    ' Pulsanti ordinamento
                    If e.Item.ItemIndex = 0 Then

                        Dim btnUp As ImageButton = DirectCast(e.Item.FindControl("btnUp"), ImageButton)

                        btnUp.Enabled = False
                        btnUp.ImageUrl = ResolveClientUrl("~/images/arrow_blue12_up_dis.png")
                        btnUp.ToolTip = String.Empty

                    End If

                    If e.Item.ItemIndex = list.Count - 1 Then

                        Dim btnDown As ImageButton = DirectCast(e.Item.FindControl("btnDown"), ImageButton)

                        btnDown.Enabled = False
                        btnDown.ImageUrl = ResolveClientUrl("~/images/arrow_blue12_down_dis.png")
                        btnDown.ToolTip = String.Empty

                    End If

                    ' Sezioni
                    Dim cmbSezione As DropDownList = e.Item.FindControl("cmbSezione")

                    cmbSezione.DataSource = Me.Sezioni
                    cmbSezione.DataTextField = "DescrizioneSezione"
                    cmbSezione.DataValueField = "CodiceSezione"
                    cmbSezione.DataBind()

                    cmbSezione.SelectedValue = osservazione.CodiceSezione

                    ' Pulsante condizioni
                    Dim btnCondizioni As ImageButton = DirectCast(e.Item.FindControl("btnCondizioni"), ImageButton)

                    If osservazione.TipoRisposta = Constants.TipoRispostaOsservazioneBilancio.TestoLibero Then

                        btnCondizioni.Style.Item("display") = "none"

                    Else

                        btnCondizioni.Style.Item("display") = "block"

                        If Me.StatoCorrentePagina = StatoPagina.Edit Then
                            btnCondizioni.Enabled = False
                            btnCondizioni.ImageUrl = ResolveClientUrl("~/images/ricalcola_dis.gif")
                        Else
                            btnCondizioni.Enabled = True
                            btnCondizioni.ImageUrl = ResolveClientUrl("~/images/ricalcola.gif")
                        End If

                    End If
                End If

        End Select

    End Sub

    Private Sub cmbOsservazione_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cmbOsservazione.SelectedIndexChanged

        CaricaRisposte(Me.cmbOsservazione.SelectedValue, Me.cmbRispostaDefault)

    End Sub

#End Region

#Region " Private Methods "

    Private Sub SetLayout(stato As StatoPagina)

        Me.StatoCorrentePagina = stato

        Dim isInEdit As Boolean = (stato = StatoPagina.Edit)

        Me.ToolBar.Items.FromKeyButton("btnAggiungi").Enabled = True
        Me.ToolBar.Items.FromKeyButton("btnElimina").Enabled = True
        Me.ToolBar.Items.FromKeyButton("btnSalva").Enabled = True
        Me.ToolBar.Items.FromKeyButton("btnAnnulla").Enabled = True
        Me.ToolBar.Items.FromKeyButton("btnIndietro").Enabled = Not isInEdit

        Me.OnitLayout31.Busy = isInEdit

    End Sub

    Private Sub LoadSezioni()

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAnaBilanci As New Biz.BizAnaBilanci(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                Me.Sezioni = bizAnaBilanci.GetSezioniBilancio(Me.NumeroBilancio, Me.CodiceMalattia, True)

            End Using
        End Using

    End Sub

    Private Sub LoadOsservazioni()

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAnaBilanci As New Biz.BizAnaBilanci(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                Dim listAssociate As List(Of Entities.BilancioOsservazioneAssociata) = bizAnaBilanci.GetOsservazioniAssociate(Me.NumeroBilancio, Me.CodiceMalattia)

                Me.dgrOsservazioniAssociate.DataSource = listAssociate
                Me.dgrOsservazioniAssociate.DataBind()

                Dim listAssociabili As List(Of Entities.BilancioOsservazioneAnagrafica) = bizAnaBilanci.GetOsservazioniAssociabili(listAssociate)

                Me.dgrOsservazioni.DataSource = listAssociabili.OrderBy(Function(p) p.CodiceOsservazione).ToList()
                Me.dgrOsservazioni.DataBind()

            End Using
        End Using

    End Sub

    Private Function CreateOsservazioneAnagrafica(gridItem As DataGridItem) As Entities.BilancioOsservazioneAnagrafica

        Dim item As New Entities.BilancioOsservazioneAnagrafica()
        item.CodiceOsservazione = HttpUtility.HtmlDecode(gridItem.Cells(GridOsservazioniColumnIndex.Codice).Text).Trim()
        item.DescrizioneOsservazione = HttpUtility.HtmlDecode(gridItem.Cells(GridOsservazioniColumnIndex.Descrizione).Text).Trim()
        item.TipoRisposta = HttpUtility.HtmlDecode(gridItem.Cells(GridOsservazioniColumnIndex.TipoRisposta).Text).Trim()

        Return item

    End Function

    Private Function CreateOsservazioneAssociata(gridItem As DataGridItem) As Entities.BilancioOsservazioneAssociata

        Dim item As New Entities.BilancioOsservazioneAssociata()

        item.NumeroBilancio = Me.NumeroBilancio
        item.CodiceMalattia = Me.CodiceMalattia
        item.CodiceOsservazione = HttpUtility.HtmlDecode(gridItem.Cells(GridOsservazioniAssociateColumnIndex.Codice).Text).Trim()
        item.DescrizioneOsservazione = HttpUtility.HtmlDecode(gridItem.Cells(GridOsservazioniAssociateColumnIndex.Descrizione).Text).Trim()
        item.CodiceSezione = DirectCast(gridItem.FindControl("cmbSezione"), DropDownList).SelectedValue
        item.IsObbligatoria = DirectCast(gridItem.FindControl("chkObbligatorio"), CheckBox).Checked
        item.NumeroOsservazione = gridItem.ItemIndex + 1
        item.TipoRisposta = HttpUtility.HtmlDecode(gridItem.Cells(GridOsservazioniAssociateColumnIndex.TipoRisposta).Text).Trim()

        Return item

    End Function

    Private Function GetOsservazioniAnagrafiche() As List(Of Entities.BilancioOsservazioneAnagrafica)

        Dim list As New List(Of Entities.BilancioOsservazioneAnagrafica)()

        For Each gridItem As DataGridItem In Me.dgrOsservazioni.Items

            list.Add(CreateOsservazioneAnagrafica(gridItem))

        Next

        Return list

    End Function

    Private Function GetOsservazioniAssociate() As List(Of Entities.BilancioOsservazioneAssociata)

        Dim list As New List(Of Entities.BilancioOsservazioneAssociata)()

        For Each gridItem As DataGridItem In Me.dgrOsservazioniAssociate.Items

            list.Add(CreateOsservazioneAssociata(gridItem))

        Next

        Return list

    End Function

    Private Sub SwitchOsservazioniAssociate(sourceIndex As Integer, destinationIndex As Integer)

        Dim listAssociate As List(Of Entities.BilancioOsservazioneAssociata) = GetOsservazioniAssociate()

        Dim osservazioneDaSpostare As Entities.BilancioOsservazioneAssociata = listAssociate(sourceIndex).Clone()

        listAssociate.RemoveAt(sourceIndex)
        listAssociate.Insert(destinationIndex, osservazioneDaSpostare)

        Me.dgrOsservazioniAssociate.DataSource = listAssociate
        Me.dgrOsservazioniAssociate.DataBind()

    End Sub

    Private Sub AddOsservazioni()

        Dim listDaAssociare As New List(Of Entities.BilancioOsservazioneAnagrafica)()
        Dim listRimaste As New List(Of Entities.BilancioOsservazioneAnagrafica)()

        For Each gridItem As DataGridItem In Me.dgrOsservazioni.Items
            If DirectCast(gridItem.FindControl("chkSelezione"), CheckBox).Checked Then

                listDaAssociare.Add(CreateOsservazioneAnagrafica(gridItem))

            Else

                listRimaste.Add(CreateOsservazioneAnagrafica(gridItem))

            End If
        Next

        If listDaAssociare.IsNullOrEmpty() Then
            Me.OnitLayout31.InsertRoutineJS("alert('Nessuna osservazione selezionata tra quelle associabili.');")
            Return
        End If

        SetLayout(StatoPagina.Edit)

        Me.dgrOsservazioni.DataSource = listRimaste
        Me.dgrOsservazioni.DataBind()

        Dim listAssociate As List(Of Entities.BilancioOsservazioneAssociata) = GetOsservazioniAssociate()

        For Each item As Entities.BilancioOsservazioneAnagrafica In listDaAssociare

            Dim osservazione As New Entities.BilancioOsservazioneAssociata()

            osservazione.NumeroBilancio = Me.NumeroBilancio
            osservazione.CodiceMalattia = Me.CodiceMalattia
            osservazione.CodiceOsservazione = item.CodiceOsservazione
            osservazione.DescrizioneOsservazione = item.DescrizioneOsservazione
            osservazione.CodiceSezione = String.Empty
            osservazione.IsObbligatoria = False
            osservazione.NumeroOsservazione = listAssociate.Count + 1
            osservazione.TipoRisposta = item.TipoRisposta

            listAssociate.Add(osservazione)

        Next

        Me.dgrOsservazioniAssociate.DataSource = listAssociate
        Me.dgrOsservazioniAssociate.DataBind()

    End Sub

    Private Sub DeleteOsservazioni()

        Dim listDaCancellare As New List(Of Entities.BilancioOsservazioneAssociata)()
        Dim listRimaste As New List(Of Entities.BilancioOsservazioneAssociata)()

        For Each gridItem As DataGridItem In Me.dgrOsservazioniAssociate.Items
            If DirectCast(gridItem.FindControl("chkSelezioneAssociata"), CheckBox).Checked Then

                listDaCancellare.Add(CreateOsservazioneAssociata(gridItem))

            Else

                listRimaste.Add(CreateOsservazioneAssociata(gridItem))

            End If
        Next

        If listDaCancellare.IsNullOrEmpty() Then
            Me.OnitLayout31.InsertRoutineJS("alert('Nessuna osservazione selezionata tra quelle associate.');")
            Return
        End If

        SetLayout(StatoPagina.Edit)

        Me.dgrOsservazioniAssociate.DataSource = listRimaste
        Me.dgrOsservazioniAssociate.DataBind()

        Dim listAssociabili As List(Of Entities.BilancioOsservazioneAnagrafica) = GetOsservazioniAnagrafiche()

        For Each item As Entities.BilancioOsservazioneAssociata In listDaCancellare

            Dim osservazione As New Entities.BilancioOsservazioneAnagrafica()
            osservazione.CodiceOsservazione = item.CodiceOsservazione
            osservazione.DescrizioneOsservazione = item.DescrizioneOsservazione
            osservazione.TipoRisposta = item.TipoRisposta

            listAssociabili.Add(osservazione)

        Next

        Me.dgrOsservazioni.DataSource = listAssociabili.OrderBy(Function(p) p.CodiceOsservazione).ToList()
        Me.dgrOsservazioni.DataBind()

    End Sub

    Private Function SalvaOsservazioni() As Biz.BizGenericResult

        Dim result As Biz.BizGenericResult = Nothing

        Dim listAssociate As List(Of Entities.BilancioOsservazioneAssociata) = GetOsservazioniAssociate()

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAnaBilanci As New Biz.BizAnaBilanci(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                result = bizAnaBilanci.SalvaOsservazioniAssociate(Me.NumeroBilancio, Me.CodiceMalattia, listAssociate)

            End Using
        End Using

        Return result

    End Function

#Region " Condizioni "

    Private Sub CaricaCondizioni(codiceOsservazione As String)

        Me.dtaCondizioni = New DataTable()

        Using DAM As IDAM = OnVacUtility.OpenDam()

            DAM.QB.NewQuery()
            DAM.QB.AddTables("T_ANA_LINK_BIL_OSS_RISP_OSS, T_ANA_OSSERVAZIONI, T_ANA_RISPOSTE")
            DAM.QB.AddSelectFields("T_ANA_LINK_BIL_OSS_RISP_OSS.*, OSS_DESCRIZIONE, RIS_DESCRIZIONE")
            DAM.QB.AddWhereCondition("OSS_CODICE", Comparatori.Uguale, "LRO_OSS_CODICE", DataTypes.Join)
            DAM.QB.AddWhereCondition("RIS_CODICE", Comparatori.Uguale, "LRO_RIS_CODICE_COLLEGATA", DataTypes.Join)
            DAM.QB.AddWhereCondition("LRO_BIL_N_BILANCIO", Comparatori.Uguale, Me.NumeroBilancio, DataTypes.Numero)
            DAM.QB.AddWhereCondition("LRO_BIL_MAL_CODICE", Comparatori.Uguale, Me.CodiceMalattia, DataTypes.Stringa)
            DAM.QB.AddWhereCondition("LRO_OSS_CODICE_COLLEGATA", Comparatori.Uguale, codiceOsservazione, DataTypes.Stringa)

            DAM.BuildDataTable(Me.dtaCondizioni)

        End Using

        Me.dlsCondizioni.DataSource = Me.dtaCondizioni
        Me.dlsCondizioni.DataBind()

    End Sub

    Private Sub CaricaComboCondizioni(osservazione As String)

        CaricaRisposte(osservazione, cmbRisposta)

        cmbOsservazione.DataSource = GetOsservazioniAssociate()
        cmbOsservazione.DataTextField = "DescrizioneOsservazione"
        cmbOsservazione.DataValueField = "CodiceOsservazione"
        cmbOsservazione.DataBind()

    End Sub

    Private Sub CaricaRisposte(osservazione As String, cmb As DropDownList)

        Dim dtaRisposte As New DataTable()

        Using DAM As IDAM = OnVacUtility.OpenDam()

            DAM.QB.NewQuery()
            DAM.QB.AddTables("T_ANA_LINK_OSS_RISPOSTE", "T_ANA_RISPOSTE")
            DAM.QB.AddSelectFields("*")
            DAM.QB.AddWhereCondition("RIS_CODICE", Comparatori.Uguale, "RIO_RIS_CODICE", DataTypes.Join)
            DAM.QB.AddWhereCondition("RIO_OSS_CODICE", Comparatori.Uguale, osservazione, DataTypes.Stringa)

            DAM.BuildDataTable(dtaRisposte)

        End Using

        cmb.DataSource = dtaRisposte
        cmb.DataTextField = "RIS_DESCRIZIONE"
        cmb.DataValueField = "RIS_CODICE"
        cmb.DataBind()

    End Sub

    Private Sub CreaCondizione()

        If Me.chkDisabilita.Checked Or Me.chkCollegata.Checked Then

            Try
                Using DAM As IDAM = OnVacUtility.OpenDam()

                    DAM.QB.NewQuery()
                    DAM.QB.AddTables("T_ANA_LINK_BIL_OSS_RISP_OSS")
                    DAM.QB.AddInsertField("LRO_BIL_N_BILANCIO", Me.NumeroBilancio, DataTypes.Numero)
                    DAM.QB.AddInsertField("LRO_BIL_MAL_CODICE", Me.CodiceMalattia, DataTypes.Stringa)
                    DAM.QB.AddInsertField("LRO_OSS_CODICE_COLLEGATA", Me.OsservazioneSelezionata, DataTypes.Stringa)
                    DAM.QB.AddInsertField("LRO_RIS_CODICE_COLLEGATA", Me.cmbRisposta.SelectedValue, DataTypes.Stringa)
                    DAM.QB.AddInsertField("LRO_OSS_CODICE", Me.cmbOsservazione.SelectedValue, DataTypes.Stringa)
                    DAM.QB.AddInsertField("LRO_RIS_DEFAULT", Me.cmbRispostaDefault.SelectedValue, DataTypes.Stringa)
                    DAM.QB.AddInsertField("LRO_FLAG_VISIBILE", IIf(Me.chkDisabilita.Checked, "S", "N"), DataTypes.Stringa)
                    DAM.QB.AddInsertField("LRO_FLAG_DEFAULT", IIf(Me.chkImpostaDefault.Checked, "S", "N"), DataTypes.Stringa)
                    DAM.QB.AddInsertField("LRO_FLAG_COLLEGATA", IIf(Me.chkCollegata.Checked, "S", "N"), DataTypes.Stringa)

                    DAM.ExecNonQuery(ExecQueryType.Insert)

                End Using

            Catch ex As Exception
                Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Condizione duplicata!", "", False, False))
            End Try

            CaricaCondizioni(Me.OsservazioneSelezionata)

        Else
            Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Per aggiungere una condizione è necessario specificare una azione da eseguire!", "", False, False))
        End If

    End Sub

    Private Sub EliminaCondizioni()

        Using DAM As IDAM = OnVacUtility.OpenDam()

            For i As Integer = Me.dtaCondizioni.Rows.Count - 1 To 0 Step -1

                If DirectCast(Me.dlsCondizioni.Items(i).FindControl("chkSelezione"), CheckBox).Checked Then

                    DAM.QB.NewQuery()
                    DAM.QB.AddTables("T_ANA_LINK_BIL_OSS_RISP_OSS")
                    DAM.QB.AddWhereCondition("LRO_BIL_N_BILANCIO", Comparatori.Uguale, Me.NumeroBilancio, DataTypes.Numero)
                    DAM.QB.AddWhereCondition("LRO_BIL_MAL_CODICE", Comparatori.Uguale, Me.CodiceMalattia, DataTypes.Stringa)
                    DAM.QB.AddWhereCondition("LRO_OSS_CODICE_COLLEGATA", Comparatori.Uguale, Me.OsservazioneSelezionata, DataTypes.Stringa)
                    DAM.QB.AddWhereCondition("LRO_RIS_CODICE_COLLEGATA", Comparatori.Uguale, Me.dtaCondizioni.Rows(i)("LRO_RIS_CODICE_COLLEGATA"), DataTypes.Stringa)
                    DAM.QB.AddWhereCondition("LRO_OSS_CODICE", Comparatori.Uguale, Me.dtaCondizioni.Rows(i)("LRO_OSS_CODICE"), DataTypes.Stringa)

                    DAM.ExecNonQuery(ExecQueryType.Delete)

                End If

            Next i

        End Using

        CaricaCondizioni(Me.OsservazioneSelezionata)

    End Sub

#End Region

#End Region

End Class

