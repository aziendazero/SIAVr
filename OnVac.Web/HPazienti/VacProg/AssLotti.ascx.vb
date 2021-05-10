Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.Shared.Manager.OnitProfile


Partial Class OnVac_AssLotti
    Inherits Common.UserControlFinestraModalePageBase

#Region " Events "

    Public Event AssLottiConferma()

    Protected Sub OnAssLottiConferma()
        RaiseEvent AssLottiConferma()
    End Sub

    Public Event SetData(lottiFuoriEtaInclusi As Boolean, ByRef dtLottiInclusi As DataTable, ByRef codiceLottiUtilizzati As String())

    Protected Sub OnSetData(lottiFuoriEtaInclusi As Boolean, ByRef dtLottiInclusi As DataTable, ByRef codiceLottiUtilizzati As String())
        RaiseEvent SetData(lottiFuoriEtaInclusi, dtLottiInclusi, codiceLottiUtilizzati)
    End Sub

    Public Event LayoutBusy(disableLayout As Boolean)

    Public Event ShowMessage(message As String)

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

#Region " Variables "

    'La funzione viene chiamata due volte. Il flag impedisce che venga scritto due volte, nello stesso postback, lo script lato client
    Private CheckClientWritten As Boolean = False

    Public strJS As String

#End Region

#Region " Public Methods "

    Public Overrides Sub LoadModale()
        '--
        Me.chkPenna.Checked = True
        '--
        Me.chkLottiFuoriEta.Checked = False
        '--
        Me.BindLotti(True)
        '--
        ' Visibilità colonna di attivazione del lotto: solo se è gestito il magazzino e non è prevista la gestione della lettura del codice a barre attraverso la pistola
        SetColumnVisibility("LottoAttivo", Me.Settings.GESMAG AndAlso Not Me.Settings.GESBALOT)
        '--
    End Sub

    Public Function GetLottiUtilizzati() As String()

        Dim codiceLottiUtilizzati As New List(Of String)

        For Each item As DataGridItem In Me.dg_lotti.Items
            If DirectCast(item.FindControl("cb_sel"), CheckBox).Checked Then
                codiceLottiUtilizzati.Add(DirectCast(item.FindControl("lb_codLotto"), Label).Text)
            End If
        Next

        Return codiceLottiUtilizzati.ToArray()

    End Function

#End Region

#Region " Events Handlers "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key
            Case "btn_Conferma"
                Me.OnAssLottiConferma()
        End Select

    End Sub

    Private Sub chkLottiFuoriEta_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkLottiFuoriEta.CheckedChanged
        '--
        If Me.chkLottiFuoriEta.Checked Then
            '--
            Me.ShowDivPassword(True)
            '--
            Me.strJS &= "window.setTimeout('document.getElementById(""" & Me.txtPassword.ClientID & """).focus();',500);"
            '--
        Else
            '--
            Me.BindLotti(True)
            '--
        End If
        '--
    End Sub

    Private Sub btnOKPassword_Click(sender As System.Object, e As System.EventArgs) Handles btnOKPassword.Click
        '--
        Dim userAuthenticationInfo As OnVacContext.UserAuthenticationInfo = OnVacContext.GetCurrentUserAuthenticationInfo(Me.txtPassword.Text)
        '--
        If userAuthenticationInfo.IsAuthenticated Then
            '--
            Me.BindLotti(True)
            '--
        Else
            '--
            Me.strJS &= String.Format("alert('{0}');", userAuthenticationInfo.AuthenticationError)
            '--
            Me.chkLottiFuoriEta.Checked = False
            '--
        End If
        '--
        Me.ShowDivPassword(False)
        '--
    End Sub

    Private Sub btnAnullaPassword_Click(sender As System.Object, e As System.EventArgs) Handles btnAnullaPassword.Click
        '--
        Me.ShowDivPassword(False)
        '--
        Me.chkLottiFuoriEta.Checked = False
        '--
    End Sub

    Private Sub dg_lotti_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dg_lotti.ItemDataBound

        Select Case e.Item.ItemType

            Case ListItemType.Item, ListItemType.AlternatingItem, ListItemType.EditItem

                If e.Item.DataItem("fuori_eta") Then
                    e.Item.ForeColor = Color.Red
                End If

                ' Impostazioni user control di attivazione del lotto. 
                ' Visibile solo se è stata configurata la possibilità di attivare i lotti da questo controllo e se il lotto non è già attivo.
                Dim ucAttivaLotto As AttivazioneLotto = DirectCast(e.Item.FindControl("ucAttivaLotto"), AttivazioneLotto)

                If Me.Settings.VACPROG_ATTIVAZIONE_LOTTO Then

                    ' Impostazione dati del lotto relativo
                    Dim datiLotto As New AttivazioneLotto.DatiLotto()
                    datiLotto.CodiceLotto = e.Item.DataItem("codLotto")
                    datiLotto.DescrizioneLotto = e.Item.DataItem("descLotto")
                    datiLotto.CodiceConsultorio = e.Item.DataItem("lcn_cns_codice")
                    datiLotto.CodiceNomeCommerciale = e.Item.DataItem("codNC")
                    datiLotto.LottoAttivo = e.Item.DataItem("attivo")
                    datiLotto.TotGiorniMinAttivazione = GetGiorniTotaliEtaAttivazione(e.Item.DataItem("lcn_eta_min_attivazione"))
                    datiLotto.TotGiorniMaxAttivazione = GetGiorniTotaliEtaAttivazione(e.Item.DataItem("lcn_eta_max_attivazione"))

                    ucAttivaLotto.InitDatiLottoCorrente(datiLotto)

                Else
                    ucAttivaLotto.Visible = False
                End If

        End Select

    End Sub

#End Region

#Region " UserControl Events "

    Protected Sub ucAttivaLotto_OpenModaleAttivazioneLotto(attivaLotto As Boolean)

        RaiseEvent LayoutBusy(True)

    End Sub

    Protected Sub ucAttivaLotto_ShowMessage(message As String)

        RaiseEvent ShowMessage(message)

    End Sub

    Protected Sub ucAttivaLotto_SalvaAttivazioneLotto()

        RaiseEvent LayoutBusy(False)

        BindLotti(True)

    End Sub

    Protected Sub ucAttivaLotto_AnnullaAttivazioneLotto()

        RaiseEvent LayoutBusy(False)

    End Sub

#End Region

#Region " Private "

    Private Sub CheckClient(dtLottiInclusi As DataTable, codiceLottiUtilizzati As String())

        If Not Me.CheckClientWritten Then
            '--
            'controllo sulla gestione con pistola
            '--
            Dim gesBalotNormalizzato As Boolean = Me.Settings.GESMAG AndAlso Me.Settings.GESBALOT
            '--
            Dim js As New System.Text.StringBuilder()
            js.AppendLine("var arLottiPresenti=new Array();")
            '--
            For i As Int16 = 0 To codiceLottiUtilizzati.Length - 1
                js.AppendFormat("arLottiPresenti[{0}]='{1}'{2}",
                                i.ToString(),
                                HttpUtility.JavaScriptStringEncode(codiceLottiUtilizzati(i)),
                                Environment.NewLine)
            Next
            '--
            js.AppendLine("var arLotti=new Array();")
            '--
            For i As Int16 = 0 To dtLottiInclusi.DefaultView.Count - 1
                '--
                Dim valenzeVaccinazioni As String() = dtLottiInclusi.DefaultView.Item(i)("valVac").split("|")
                '--
                js.AppendFormat("arLotti[{0}]=['{1}',[",
                                i.ToString(),
                                HttpUtility.JavaScriptStringEncode(dtLottiInclusi.DefaultView(i)("codLotto").ToString()))
                '--
                For j As Int16 = 0 To valenzeVaccinazioni.Length - 1
                    If j > 0 Then js.Append(",")
                    js.AppendFormat("'{0}'", valenzeVaccinazioni(j).ToString())
                Next
                '--
                js.AppendFormat("]];{0}", Environment.NewLine)
                '--
            Next
            '--
            js.AppendLine("CekkaLotti();")
            '--
            js.AppendLine("for (i=1;i<document.getElementById(dg_lotti).rows.length;i++){")
            js.AppendLine("obj=document.getElementById(dg_lotti).rows[i].cells[0];")
            js.AppendLine("var objChkbox=GetElementByTag(obj,'INPUT',1,1,false);")
            '--
            js.AppendLine(IIf(gesBalotNormalizzato, "objChkbox.onclick=controlla_lottoBarCode;}", "objChkbox.onclick=controlla_lotto;}"))
            '--
            js.AppendLine("addProcToEvent(""window.onload"",""document.getElementById('" & tbCodLottoLCB.ClientID & "').focus();"",""GetFocusToLCB"")")
            '--
            'aggiunge il controllo client a seconda del parametro GESBALOT (modifica 28/12/2004)
            '--
            Me.tbCodLottoLCB.Attributes.Add("onblur", IIf(gesBalotNormalizzato, "CheckLottoFromLCBBarCode(this.value)", "CheckLottoFromLCB(this.value)"))
            '--
            'visibilità della casella di testo
            '--
            Me.tbCodLottoLCB.CssClass = IIf(gesBalotNormalizzato, "TextBox_Stringa", "inputLCB")
            Me.lblCodLottoLCB.CssClass = IIf(gesBalotNormalizzato, "TextBox_Stringa", "inputLCB")
            Me.chkPenna.CssClass = IIf(gesBalotNormalizzato, "TextBox_Stringa", "inputLCB")
            '--
            Me.strJS &= js.ToString()
            '--
            'Imposta lo stato del flag per non scrivere una seconda volta lo script
            '--
            Me.CheckClientWritten = True
            '--
        End If

    End Sub

    Private Sub BindLotti(checkClient As Boolean)
        '--
        Dim dtLottiInclusi As DataTable = Nothing
        Dim codiceLottiUtilizzati As String() = Nothing
        '--
        Me.OnSetData(Me.chkLottiFuoriEta.Checked, dtLottiInclusi, codiceLottiUtilizzati)
        '--
        dtLottiInclusi.DefaultView.Sort = "descNC,scadenza"
        '--
        Me.dg_lotti.DataSource = dtLottiInclusi.DefaultView
        Me.dg_lotti.DataBind()
        '--
        If checkClient Then Me.CheckClient(dtLottiInclusi, codiceLottiUtilizzati)
        '--
    End Sub

    Private Sub ShowDivPassword(show As Boolean)

        If show Then
            Me.strJS &= "document.getElementById('" + Me.divPassword.ClientID + "').style.display='block';"
        Else
            Me.strJS &= "document.getElementById('" + Me.divPassword.ClientID + "').style.display='none';"
        End If

        Me.tbCodLottoLCB.Enabled = Not show
        Me.ToolBar.Enabled = Not show
        Me.LayoutTitolo_sezione.Enabled = Not show
        Me.lblCodLottoLCB.Enabled = Not show
        Me.chkLottiFuoriEta.Enabled = Not show
        Me.chkPenna.Enabled = Not show
        Me.dg_lotti.Enabled = Not show

    End Sub

    Private Sub SetColumnVisibility(sortExpression As String, visible As Boolean)

        Dim column As DataGridColumn = GetColumnBySortExpression(sortExpression)

        If Not column Is Nothing Then column.Visible = visible

    End Sub

    Private Function GetColumnBySortExpression(sortExpression As String) As DataGridColumn

        If Me.dg_lotti.Items.Count = 0 Then Return Nothing

        Dim column As DataGridColumn =
            (From item As DataGridColumn In Me.dg_lotti.Columns
             Where item.SortExpression = sortExpression
             Select item).FirstOrDefault()

        Return column

    End Function

    Private Function GetGiorniTotaliEtaAttivazione(value As Object) As Integer?

        Dim etaAttivazione As Entities.Eta = Nothing

        If Not value Is Nothing AndAlso Not value Is DBNull.Value AndAlso Not String.IsNullOrEmpty(value.ToString()) Then

            etaAttivazione = New Entities.Eta(Convert.ToInt32(value))

        End If

        If etaAttivazione Is Nothing Then Return Nothing

        Return etaAttivazione.GiorniTotali

    End Function

#End Region

End Class
