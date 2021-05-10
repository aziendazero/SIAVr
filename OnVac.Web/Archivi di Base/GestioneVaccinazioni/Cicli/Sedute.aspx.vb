Imports Onit.Database.DataAccessManager
Imports Onit.Controls


Partial Class OnVac_Sedute
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

#Region " Properties "

    Property nMod() As Int16
        Get
            Return Session("OnVac_nMod")
        End Get
        Set(Value As Int16)
            Session("OnVac_nMod") = Value
        End Set
    End Property

    Property codCiclo() As String
        Get
            Return Session("OnVac_codCiclo")
        End Get
        Set(Value As String)
            Session("OnVac_codCiclo") = Value
        End Set
    End Property

    Property tb_sedute() As DataTable
        Get
            If Session("OnVac_tb_sedute") Is Nothing Then Return Nothing
            Return DirectCast(Session("OnVac_tb_sedute"), SerializableDataTableContainer).Data
        End Get
        Set(Value As DataTable)
            If Session("OnVac_tb_sedute") Is Nothing Then
                Session("OnVac_tb_sedute") = New SerializableDataTableContainer
            End If
            DirectCast(Session("OnVac_tb_sedute"), SerializableDataTableContainer).Data = Value
        End Set
    End Property

    Property tb_vac_sed() As DataTable
        Get
            If Session("OnVac_tb_vac_sed") Is Nothing Then Return Nothing
            Return DirectCast(Session("OnVac_tb_vac_sed"), SerializableDataTableContainer).Data
        End Get
        Set(Value As DataTable)
            If Session("OnVac_tb_vac_sed") Is Nothing Then
                Session("OnVac_tb_vac_sed") = New SerializableDataTableContainer
            End If
            DirectCast(Session("OnVac_tb_vac_sed"), SerializableDataTableContainer).Data = Value
        End Set
    End Property

    Property newrow() As Boolean
        Get
            Return Session("OnVac_newrow")
        End Get
        Set(Value As Boolean)
            Session("OnVac_newrow") = Value
        End Set
    End Property

    Property nSed() As Integer
        Get
            Return Session("OnVac_nSed")
        End Get
        Set(Value As Integer)
            Session("OnVac_nSed") = Value
        End Set
    End Property

    Property tipoVac() As String
        Get
            Return Session("OnVac_tipoVac")
        End Get
        Set(Value As String)
            Session("OnVac_tipoVac") = Value
        End Set
    End Property

    Property tb_ass_sed() As DataTable
        Get
            If Session("OnVac_tb_ass_sed") Is Nothing Then Return Nothing
            Return DirectCast(Session("OnVac_tb_ass_sed"), SerializableDataTableContainer).Data
        End Get
        Set(Value As DataTable)
            If Session("OnVac_tb_ass_sed") Is Nothing Then
                Session("OnVac_tb_ass_sed") = New SerializableDataTableContainer
            End If
            DirectCast(Session("OnVac_tb_ass_sed"), SerializableDataTableContainer).Data = Value
        End Set
    End Property

    Property tipoAss() As String
        Get
            Return Session("OnVac_tipoAss")
        End Get
        Set(Value As String)
            Session("OnVac_tipoAss") = Value
        End Set
    End Property

#End Region

#Region " Private/Public Variables "

    Private sql As String
    Public strJS As String

#End Region

#Region " Eventi Page "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        '
        If Not IsPostBack() Then
            '
            codCiclo = Request.QueryString("CIC").Split("|")(0)
            LayoutTitolo.Text = Request.QueryString("CIC").Split("|")(1)
            '
            nMod = 0
            '
            CaricaDati()
            '
        End If
        '
        strJS = ""
        Select Case Request.Form("__EVENTTARGET")
            Case "NuovaSeduta"
                'dg_sedute.EditItemIndex <> -1 And 
                If (dg_vac_sed.EditItemIndex <> -1 Or dg_ass_sed.EditItemIndex <> -1) Then
                    strJS = "<script language=""javascript"">alert(""Cliccare ANNULLA o AGGIORNA della riga che si sta modificando prima di inserire una nuova riga"")</script>"
                    Exit Sub
                End If
                'dg_vac_sed.Visible = False
                'sezioneVac.Visible = False
                '
                dg_ass_sed.DataSource = Nothing
                dg_ass_sed.DataBind()
                dg_vac_sed.DataSource = Nothing
                dg_vac_sed.DataBind()
                '
                newrow = True
                Dim row As DataRow
                Dim i, j As Integer
                dg_sedute.SelectedIndex = -1
                'aggiungo nuova riga alla tabella
                row = tb_sedute.NewRow
                For i = 0 To tb_sedute.Rows.Count - 1
                    If Not tb_sedute.Rows(i).RowState = DataRowState.Deleted Then
                        j += 1
                    End If
                Next
                row("tsd_n_seduta") = j + 1

                'imposta il valore di default della colonna posticipo seduta al valore 'N' [modifica 24/02/2005]
                row("tsd_posticipo_seduta") = "N"

                tb_sedute.Rows.Add(row)
                j += 1
                'determina la pagina e il numero di item della riga da editare
                If (j > dg_sedute.PageSize) Then
                    Dim item_rimanenti = j Mod dg_sedute.PageSize
                    dg_sedute.CurrentPageIndex = Int(j / dg_sedute.PageSize) - 1
                    If (item_rimanenti > 0) Then
                        dg_sedute.CurrentPageIndex += 1
                    End If
                    If (item_rimanenti = 0) Then
                        dg_sedute.EditItemIndex = dg_sedute.PageSize - 1
                    Else
                        dg_sedute.EditItemIndex = item_rimanenti - 1
                    End If
                Else
                    dg_sedute.EditItemIndex = j - 1
                End If

                'ricarica la tabella nel datagrid
                dg_sedute.DataSource = tb_sedute
                dg_sedute.DataBind()
                ToolBar.Enabled = False
                If (j > 1) Then
                    Dim rowSelected As DataRow = tb_sedute.Rows.Find(j - 1)
                    etaPrec.Text = rowSelected("tsd_eta_seduta") + rowSelected("tsd_intervallo_prossima")
                End If
                nascosto.Text = "DisNS"
                OnitLayout1.Busy = True
                nMod += 1
                strJS = "<script language=""javascript"">document.getElementById(""dg_sedute"").firstChild.childNodes[" & (1 + dg_sedute.EditItemIndex) & "].childNodes[4].firstChild.focus()</script>"

            Case "NuovaVac"
                If (dg_sedute.EditItemIndex <> -1 Or dg_ass_sed.EditItemIndex <> -1) Then
                    strJS = "<script language=""javascript"">alert(""Cliccare ANNULLA o AGGIORNA della riga che si sta modificando prima di inserire una nuova riga"")</script>"
                    Exit Sub
                End If
                '
                If dg_ass_sed.Items.Count > 0 Then
                    strJS = "<script language=""javascript"">alert(""Ci sono associazioni relative al ciclo selezionato: impossibile aggiungere o modificare vaccinazioni."")</script>"
                    Exit Sub
                End If
                '
                newrow = True
                Dim row As DataRow
                Dim j As Integer = 0
                row = tb_vac_sed.NewRow()
                row("sed_cic_codice") = codCiclo
                row("sed_n_seduta") = nSed
                row("sed_n_richiamo") = 1
                row("sed_vac_codice") = "x"
                tb_vac_sed.Rows.Add(row)
                tb_vac_sed.DefaultView.RowFilter = "sed_n_seduta = " & nSed
                'determina la pagina e il numero di item della riga da editare
                If (tb_vac_sed.DefaultView.Count > dg_vac_sed.PageSize) Then
                    Dim item_rimanenti = tb_vac_sed.DefaultView.Count Mod dg_vac_sed.PageSize
                    dg_vac_sed.CurrentPageIndex = Int(tb_vac_sed.DefaultView.Count / dg_vac_sed.PageSize) - 1
                    If (item_rimanenti > 0) Then
                        dg_vac_sed.CurrentPageIndex += 1
                    End If
                    If (item_rimanenti = 0) Then
                        dg_vac_sed.EditItemIndex = dg_vac_sed.PageSize - 1
                    Else
                        dg_vac_sed.EditItemIndex = item_rimanenti - 1
                    End If
                Else
                    dg_vac_sed.EditItemIndex = tb_vac_sed.DefaultView.Count - 1
                End If

                'ricarica la tabella nel datagrid
                dg_vac_sed.DataSource = tb_vac_sed.DefaultView
                dg_vac_sed.DataBind()
                CType(dg_vac_sed.Items(dg_vac_sed.Items.Count - 1).FindControl("fm_vac_edit"), Onit.Controls.OnitModalList).Codice = ""
                CType(dg_vac_sed.Items(dg_vac_sed.Items.Count - 1).FindControl("tb_n_ric_edit"), Label).Text = ""
                ToolBar.Enabled = False
                nascosto.Text = "DisNV"
                'btn_Ins_vac.Enabled = False
                'Dim filtro As String

                Dim stbFiltro As New System.Text.StringBuilder
                Dim i As Integer
                For i = 0 To tb_vac_sed.DefaultView.Count - 2
                    'If (i <> 0) Then
                    '    filtro = filtro & " and "
                    'End If
                    'filtro = filtro & "vac_codice<>'" & tb_vac_sed.DefaultView.Item(i)("sed_vac_codice") & "'"
                    stbFiltro.AppendFormat(" vac_codice<>'{0}' and", tb_vac_sed.DefaultView.Item(i)("sed_vac_codice"))
                Next
                'If (filtro = "") Then
                '    filtro = "'true'='true'"
                'End If
                'filtro &= " order by vac_descrizione"
                'CType(dg_vac_sed.Items(dg_vac_sed.EditItemIndex).FindControl("fm_vac_edit"), Onit.Controls.OnitModalList).Filtro = filtro

                If stbFiltro.Length = 0 Then
                    stbFiltro.Append("'true'='true'")
                Else
                    stbFiltro.Remove(stbFiltro.Length - 3, 3)
                End If
                stbFiltro.Append(" order by vac_descrizione")

                CType(dg_vac_sed.Items(dg_vac_sed.EditItemIndex).FindControl("fm_vac_edit"), Onit.Controls.OnitModalList).Filtro = stbFiltro.ToString
                strJS = "<script language=""javascript"">document.getElementById(""dg_vac_sed"").firstChild.childNodes[" & (1 + dg_vac_sed.EditItemIndex) & "].childNodes[3].firstChild.childNodes[0].focus()</script>"
                OnitLayout1.Busy = True
                nMod += 1

            Case "NuovaAss"
                If (dg_sedute.EditItemIndex <> -1 Or dg_vac_sed.EditItemIndex <> -1) Then
                    strJS = "<script language=""javascript"">alert(""Cliccare ANNULLA o AGGIORNA della riga che si sta modificando prima di inserire una nuova riga"")</script>"
                    Exit Sub
                End If
                If dg_ass_sed.Items.Count = 0 And dg_vac_sed.Items.Count > 0 Then
                    strJS = "<script language=""javascript"">alert(""Ci sono vaccinazioni relative al ciclo selezionato: impossibile aggiungere o modificare associazioni."")</script>"
                    Exit Sub
                End If
                newrow = True
                Dim row As DataRow
                Dim j As Integer = 0
                row = tb_ass_sed.NewRow()
                row("sas_cic_codice") = codCiclo
                row("sas_n_seduta") = nSed
                row("sas_n_richiamo") = 1
                row("sas_ass_codice") = "x"
                tb_ass_sed.Rows.Add(row)
                tb_ass_sed.DefaultView.RowFilter = "sas_n_seduta = " & nSed
                'determina la pagina e il numero di item della riga da editare
                If (tb_ass_sed.DefaultView.Count > dg_ass_sed.PageSize) Then
                    Dim item_rimanenti = tb_ass_sed.DefaultView.Count Mod dg_ass_sed.PageSize
                    dg_ass_sed.CurrentPageIndex = Int(tb_ass_sed.DefaultView.Count / dg_ass_sed.PageSize) - 1
                    If (item_rimanenti > 0) Then
                        dg_ass_sed.CurrentPageIndex += 1
                    End If
                    If (item_rimanenti = 0) Then
                        dg_ass_sed.EditItemIndex = dg_ass_sed.PageSize - 1
                    Else
                        dg_ass_sed.EditItemIndex = item_rimanenti - 1
                    End If
                Else
                    dg_ass_sed.EditItemIndex = tb_ass_sed.DefaultView.Count - 1
                End If
                '

                'ricarica la tabella nel datagrid
                dg_ass_sed.DataSource = tb_ass_sed.DefaultView
                dg_ass_sed.DataBind()
                CType(dg_ass_sed.Items(dg_ass_sed.Items.Count - 1).FindControl("fm_ass_edit"), Onit.Controls.OnitModalList).Codice = ""
                CType(dg_ass_sed.Items(dg_ass_sed.Items.Count - 1).FindControl("tb_n_ric_edit_ass"), Label).Text = ""
                ToolBar.Enabled = False
                nascosto.Text = "DisNA"

                '
                ' --------- Impostazione filtro modale --------- '
                Dim filtro As String = ""
                Dim i As Integer
                Dim stbFiltroAss As New System.Text.StringBuilder
                Dim stbFiltroVac As New System.Text.StringBuilder

                ' Elenco associazioni già presenti nella seduta
                For i = 0 To tb_ass_sed.DefaultView.Count - 2
                    stbFiltroAss.AppendFormat("'{0}',", tb_ass_sed.DefaultView.Item(i)("sas_ass_codice"))
                Next

                ' Elenco vaccinazioni già presenti nella seduta
                Dim dvVaccAss As New DataView(tb_vac_sed)
                dvVaccAss.RowFilter = "sed_n_seduta=" + nSed.ToString ' vaccinazioni associate alla seduta
                For i = 0 To dvVaccAss.Count - 1
                    stbFiltroVac.AppendFormat("'{0}',", dvVaccAss(i)("sed_vac_codice"))
                Next

                ' Filtro associazioni già presenti
                If stbFiltroAss.Length > 0 Then
                    filtro = "ass_codice not in (" + stbFiltroAss.Remove(stbFiltroAss.Length - 1, 1).ToString + ")"
                End If
                '
                ' Filtro vaccinazioni associate ad associazioni già presenti
                If stbFiltroVac.Length > 0 Then
                    If filtro <> "" Then
                        filtro += " and "
                    End If
                    filtro += "ass_codice not in (select val_ass_codice from t_ana_link_ass_vaccinazioni where val_vac_codice in (" + stbFiltroVac.Remove(stbFiltroVac.Length - 1, 1).ToString + ") )"
                End If
                '
                ' Order by
                If filtro = "" Then
                    filtro += "'true'='true'"
                End If
                filtro += " order by ass_descrizione"

                ' Imposto il filtro della modale
                CType(dg_ass_sed.Items(dg_ass_sed.EditItemIndex).FindControl("fm_ass_edit"), Onit.Controls.OnitModalList).Filtro = filtro
                '
                strJS = "<script language=""javascript"">document.getElementById(""dg_ass_sed"").firstChild.childNodes[" & (1 + dg_ass_sed.EditItemIndex) & "].childNodes[4].firstChild.childNodes[0].focus()</script>"
                OnitLayout1.Busy = True
                nMod += 1
                '
                ' Nascondo le vaccinazioni (al conferma carico quelle relative alla nuova associazione)
                dg_vac_sed.DataSource = Nothing
                dg_vac_sed.DataBind()

        End Select

    End Sub

#End Region

#Region " Eventi Toolbar "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key
            Case "btn_Salva"
                Salva()

            Case "btn_Annulla"
                tb_sedute.PrimaryKey = Nothing
                tb_vac_sed.PrimaryKey = Nothing
                tb_ass_sed.PrimaryKey = Nothing
                tb_sedute.RejectChanges()
                tb_vac_sed.RejectChanges()
                tb_ass_sed.RejectChanges()
                dg_sedute.DataSource = tb_sedute
                dg_sedute.DataBind()
                '
                OnitLayout1.Busy = False
                nMod = 0
                '
                Dim ass_keys(1) As DataColumn
                ass_keys(0) = tb_ass_sed.Columns("sas_n_seduta")
                ass_keys(1) = tb_ass_sed.Columns("sas_ass_codice")
                tb_ass_sed.PrimaryKey = ass_keys
                Dim vac_keys(1) As DataColumn
                vac_keys(0) = tb_vac_sed.Columns("sed_n_seduta")
                vac_keys(1) = tb_vac_sed.Columns("sed_vac_codice")
                tb_vac_sed.PrimaryKey = vac_keys
                Dim sed_key(0) As DataColumn
                sed_key(0) = tb_sedute.Columns("tsd_n_seduta")
                tb_sedute.PrimaryKey = sed_key
                '
                If dg_sedute.Items.Count = 0 Then
                    dg_sedute.SelectedIndex = -1
                    dg_vac_sed.Visible = False
                    dg_ass_sed.Visible = False
                    sezioneVac.Visible = False
                    sezioneAss.Visible = False
                Else
                    dg_sedute.SelectedIndex = 0
                    dg_sedute_SelectedIndexChanged(Nothing, Nothing)
                End If
        End Select

    End Sub

    Private Sub uwtVisualizzaParametri_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles uwtVisualizzaParametri.ButtonClicked

        Select Case be.Button.Key
            Case "btnConferma"
                Dim indice As Integer = fmVisualizzaParametri.ToolTip

                'valorizzazione dei parametri
                If ControllaSedutaVacObbligatorie(indice) Then
                    CType(dg_sedute.Items(indice).FindControl("txtNumSolleciti"), TextBox).Text = txtNumSol.Text
                    CType(dg_sedute.Items(indice).FindControl("txtNumSollecitiRac"), TextBox).Text = txtNumSolRac.Text
                Else
                    CType(dg_sedute.Items(indice).FindControl("txtGiorniPosticipo"), TextBox).Text = txtGiorniPost.Text
                    CType(dg_sedute.Items(indice).FindControl("txtSollecitiNonObbligatori"), TextBox).Text = txtNumSolNonObbl.Text
                    CType(dg_sedute.Items(indice).FindControl("ddlPosticipoSeduta"), DropDownList).SelectedValue = ddlPostSed.SelectedValue
                End If
                fmVisualizzaParametri.VisibileMD = False

                'visualizzazione dell'immagine associata ai parametri
                CType(dg_sedute.Items(indice).FindControl("imbSeduteParametri"), System.Web.UI.WebControls.ImageButton).ImageUrl = IIf(VisualizzaImmagineParametri(indice), "../../../Images/sedute_parametri_ok.gif", "../../../Images/sedute_parametri_no.gif")
        End Select

    End Sub

#End Region

#Region " Eventi Datagrid "

    Private Sub dg_vac_sed_UpdateCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_vac_sed.UpdateCommand

        If dg_ass_sed.Items.Count > 0 Then
            strJS = "<script language=""javascript"">alert(""Ci sono associazioni relative al ciclo selezionato: impossibile aggiungere o modificare vaccinazioni."")</script>"
            Exit Sub
        End If
        '
        If Not newrow Then
            Dim valueKeys(1) As Object
            Dim row As DataRow
            valueKeys(0) = nSed
            valueKeys(1) = tipoVac
            row = tb_vac_sed.Rows.Find(valueKeys)
            row("sed_vac_codice") = CType(e.Item.FindControl("fm_vac_edit"), Onit.Controls.OnitModalList).Codice
            row("vac_descrizione") = CType(e.Item.FindControl("fm_vac_edit"), Onit.Controls.OnitModalList).Descrizione
            row("sed_n_richiamo") = CType(e.Item.FindControl("tb_n_ric_edit"), Label).Text
            row("sed_sii_codice") = CType(e.Item.FindControl("fm_sii_edit"), Onit.Controls.OnitModalList).Codice
            row("sii_descrizione") = CType(e.Item.FindControl("fm_sii_edit"), Onit.Controls.OnitModalList).Descrizione
            'valorizzazione del campo VAC_OBBLIGATORIA [modifica 14/02/2005]
            row("vac_obbligatoria") = CType(e.Item.FindControl("fm_vac_edit"), Onit.Controls.OnitModalList).GetAltriCampi(2)
        Else
            tb_vac_sed.Rows(tb_vac_sed.Rows.Count - 1)("sed_vac_codice") = CType(e.Item.FindControl("fm_vac_edit"), Onit.Controls.OnitModalList).Codice
            tb_vac_sed.Rows(tb_vac_sed.Rows.Count - 1)("sed_n_richiamo") = 0
            tb_vac_sed.Rows(tb_vac_sed.Rows.Count - 1)("sed_sii_codice") = CType(e.Item.FindControl("fm_sii_edit"), Onit.Controls.OnitModalList).Codice
            tb_vac_sed.Rows(tb_vac_sed.Rows.Count - 1)("sii_descrizione") = CType(e.Item.FindControl("fm_sii_edit"), Onit.Controls.OnitModalList).Descrizione
            tb_vac_sed.Rows(tb_vac_sed.Rows.Count - 1)("vac_descrizione") = CType(e.Item.FindControl("fm_vac_edit"), Onit.Controls.OnitModalList).Descrizione
            'valorizzazione del campo VAC_OBBLIGATORIA [modifica 14/02/2005]
            tb_vac_sed.Rows(tb_vac_sed.Rows.Count - 1)("vac_obbligatoria") = CType(e.Item.FindControl("fm_vac_edit"), Onit.Controls.OnitModalList).GetAltriCampi(2)
        End If

        Setta_nRichiamo(tb_vac_sed.Rows(tb_vac_sed.Rows.Count - 1)("sed_vac_codice"))
        dg_vac_sed.EditItemIndex = -1
        dg_vac_sed.SelectedIndex = -1
        dg_vac_sed.DataSource = tb_vac_sed.DefaultView
        dg_vac_sed.DataBind()
        ToolBar.Enabled = True
        nascosto.Text = ""
        newrow = False
        tipoVac = ""

        'valorizzazione parametri a seconda delle vaccinazioni rimaste [modifica15/05/2006]
        ControllaVaccinazioniRimaste()

    End Sub

    Private Sub dg_vac_sed_CancelCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_vac_sed.CancelCommand

        If dg_ass_sed.Items.Count > 0 Then
            strJS = "<script language=""javascript"">alert(""Ci sono associazioni relative al ciclo selezionato: impossibile aggiungere o modificare vaccinazioni."")</script>"
            Exit Sub
        End If

        If newrow Then
            Dim valueKeys(1) As Object
            valueKeys(0) = nSed
            valueKeys(1) = "x"
            tb_vac_sed.Rows.Find(valueKeys).Delete()
            If (dg_vac_sed.Items.Count = 1) Then
                If (dg_vac_sed.CurrentPageIndex <> 0) Then
                    dg_vac_sed.CurrentPageIndex -= 1
                End If
            End If
            newrow = False
        End If
        dg_vac_sed.EditItemIndex = -1
        dg_vac_sed.DataSource = tb_vac_sed.DefaultView
        dg_vac_sed.DataBind()

        ToolBar.Enabled = True
        nascosto.Text = ""

        nMod -= 1
        If (nMod = 0) Then OnitLayout1.Busy = False

    End Sub

    Private Sub dg_vac_sed_EditCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_vac_sed.EditCommand

        If dg_ass_sed.Items.Count > 0 Then
            strJS = "<script language=""javascript"">alert(""Ci sono associazioni relative al ciclo selezionato: impossibile aggiungere o modificare vaccinazioni."")</script>"
            Exit Sub
        End If
        '
        If (dg_sedute.EditItemIndex = -1 And dg_vac_sed.EditItemIndex = -1 And dg_ass_sed.EditItemIndex = -1) Then
            tipoVac = CType(dg_vac_sed.Items(e.Item.ItemIndex).FindControl("tb_codVac"), Label).Text
            dg_vac_sed.EditItemIndex = e.Item.ItemIndex
            dg_vac_sed.DataSource = tb_vac_sed.DefaultView
            dg_vac_sed.DataBind()
            ToolBar.Enabled = False
            nascosto.Text = "DisNV"
            'btn_Ins_vac.Enabled = False

            Dim stbFiltro As New System.Text.StringBuilder

            Dim item As DataRowView
            For Each item In tb_vac_sed.DefaultView
                If Not (item("sed_vac_codice") = tipoVac) Then
                    stbFiltro.AppendFormat(" vac_codice <> '{0}' and", item("sed_vac_codice"))
                End If
            Next

            If stbFiltro.Length = 0 Then
                stbFiltro.Append("'true'='true'")
            Else
                stbFiltro.Remove(stbFiltro.Length - 3, 3)
            End If
            stbFiltro.Append(" order by vac_descrizione")

            CType(dg_vac_sed.Items(dg_vac_sed.EditItemIndex).FindControl("fm_vac_edit"), Onit.Controls.OnitModalList).Filtro = stbFiltro.ToString
            strJS = "<script language=""javascript"">document.getElementById(""dg_vac_sed"").firstChild.childNodes[" & (1 + dg_vac_sed.EditItemIndex) & "].childNodes[3].firstChild.firstChild.focus()</script>"
            OnitLayout1.Busy = True
            nMod += 1
        Else
            strJS = "<script language=""javascript"">alert(""Cliccare ANNULLA o AGGIORNA della riga che si sta modificando prima di modificare questa riga"")</script>"
        End If

    End Sub

    Private Sub dg_vac_sed_DeleteCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_vac_sed.DeleteCommand
        If dg_ass_sed.Items.Count > 0 Then
            strJS = "<script language=""javascript"">alert(""Ci sono associazioni relative al ciclo selezionato: impossibile aggiungere o modificare vaccinazioni."")</script>"
            Exit Sub
        End If

        If (dg_sedute.EditItemIndex = -1 And dg_vac_sed.EditItemIndex = -1 And dg_ass_sed.EditItemIndex = -1) Then
            'calcola il numero di riga da eliminare

            ' PROVA
            Dim valueKeys(1) As Object
            valueKeys(0) = nSed
            valueKeys(1) = CType(dg_vac_sed.Items(e.Item.ItemIndex).FindControl("tb_codVac"), Label).Text
            tb_vac_sed.Rows.Find(valueKeys).Delete()
            Setta_nRichiamo(valueKeys(1))
            'DirectCast(e.Item.DataItem, DataRowView).Row.Delete()
            'Setta_nRichiamo(CType(dg_vac_sed.Items(e.Item.ItemIndex).FindControl("tb_codVac"), Label).Text)
            '
            'calcola la pagina da visualizzare dopo l'eliminazione
            If (dg_vac_sed.Items.Count = 1) Then
                If (dg_vac_sed.CurrentPageIndex <> 0) Then
                    dg_vac_sed.CurrentPageIndex -= 1
                End If
            End If
            'ricarico il datagrid
            dg_vac_sed.DataSource = tb_vac_sed.DefaultView
            dg_vac_sed.DataBind()

            'controllo se è rimasta almeno una vaccinazione per l'associazione dei parametri [modifica 15/05/2006]
            ControllaVaccinazioniRimaste()

            OnitLayout1.Busy = True
            nMod += 1
        Else
            strJS = "<script language='javascript'>alert('Cliccare AGGIORNA o ANNULLA della riga che si sta modificando prima di eliminare questa riga')</script>"
        End If

    End Sub

    Private Sub dg_vac_sed_PageIndexChanged(source As Object, e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dg_vac_sed.PageIndexChanged
        If (dg_sedute.EditItemIndex = -1 And dg_vac_sed.EditItemIndex = -1 And dg_ass_sed.EditItemIndex = -1) Then
            dg_vac_sed.CurrentPageIndex = e.NewPageIndex
            dg_vac_sed.DataSource = tb_vac_sed.DefaultView
            dg_vac_sed.DataBind()
        Else
            strJS = "<script language='javascript'>alert('Cliccare ANNULLA o AGGIORNA della riga che si sta modificando prima di cambiare pagina')</script>"
        End If
    End Sub

    Private Sub dg_sedute_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles dg_sedute.SelectedIndexChanged

        If (dg_vac_sed.EditItemIndex = -1 And dg_sedute.EditItemIndex = -1 And dg_ass_sed.EditItemIndex = -1) Then
            nSed = CType(dg_sedute.Items(dg_sedute.SelectedIndex).FindControl("lb_index"), Label).Text
            ' Associazioni
            dg_ass_sed.Visible = True
            sezioneAss.Visible = True
            dg_ass_sed.CurrentPageIndex = 0
            '
            ' Filtro per visualizzare le associazioni relative alla seduta selezionata
            tb_ass_sed.DefaultView.RowFilter = "sas_n_seduta = " & nSed
            '
            'tb_ass_sed.DefaultView.Sort = "ass_descrizione"
            '
            dg_ass_sed.DataSource = tb_ass_sed.DefaultView
            dg_ass_sed.DataBind()
            '
            If tb_ass_sed.DefaultView.Count > 0 Then
                dg_ass_sed.SelectedIndex = 0
                dg_ass_sed_SelectedIndexChanged(Nothing, Nothing)
            Else
                ' Vaccinazioni
                dg_vac_sed.Visible = True
                sezioneVac.Visible = True
                dg_vac_sed.CurrentPageIndex = 0
                '
                ' Filtro per visualizzare le vaccinazioni della seduta selezionata e non relative ad associazioni
                'tb_vac_sed.DefaultView.RowFilter = "sed_n_seduta = " & nSed
                tb_vac_sed.DefaultView.RowFilter = String.Format("sed_n_seduta = {0} and sas_ass_codice is null", nSed)
                '
                'tb_vac_sed.DefaultView.Sort = "vac_descrizione"
                dg_vac_sed.DataSource = tb_vac_sed.DefaultView
                dg_vac_sed.DataBind()
            End If
        Else
            If (nSed <> -1) Then
                dg_sedute.SelectedIndex = nSed - dg_sedute.PageSize * dg_sedute.CurrentPageIndex - 1
            Else
                dg_sedute.SelectedIndex = nSed
            End If
            strJS = "<script language='javascript'>alert('Cliccare ANNULLA o AGGIORNA della riga che si sta modificando prima di selezionare la riga')</script>"
        End If

    End Sub

    Private Sub dg_sedute_PageIndexChanged(source As System.Object, e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dg_sedute.PageIndexChanged

        If (dg_sedute.EditItemIndex = -1 And dg_vac_sed.EditItemIndex = -1 And dg_ass_sed.EditItemIndex = -1) Then
            dg_vac_sed.Visible = False
            sezioneVac.Visible = False
            '
            dg_ass_sed.Visible = False
            sezioneAss.Visible = False
            '
            'dg_ass_sed.DataSource = Nothing
            'dg_ass_sed.DataBind()
            'dg_vac_sed.DataSource = Nothing
            'dg_vac_sed.DataBind()
            'dg_sedute.SelectedIndex = -1
            '
            '
            dg_sedute.SelectedIndex = -1
            dg_sedute.CurrentPageIndex = e.NewPageIndex
            dg_sedute.DataSource = tb_sedute
            dg_sedute.DataBind()
        Else
            strJS = "<script language='javascript'>alert('Cliccare ANNULLA o AGGIORNA della riga che si sta modificando prima di cambiare pagina')</script>"
        End If

    End Sub

    Private Sub dg_sedute_DeleteCommand(source As System.Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_sedute.DeleteCommand

        If (dg_sedute.EditItemIndex = -1 And dg_vac_sed.EditItemIndex = -1 And dg_ass_sed.EditItemIndex = -1) Then
            'dg_vac_sed.Visible = False
            'sezioneVac.Visible = False
            '
            dg_ass_sed.DataSource = Nothing
            dg_ass_sed.DataBind()
            dg_vac_sed.DataSource = Nothing
            dg_vac_sed.DataBind()
            dg_sedute.SelectedIndex = -1
            '
            Dim row As DataRow
            Dim valueKey(0) As Object
            valueKey(0) = CType(dg_sedute.Items(e.Item.ItemIndex).Cells(0).FindControl("lb_index"), Label).Text
            row = tb_sedute.Rows.Find(valueKey)
            row.Delete()
            dg_sedute.DataSource = tb_sedute
            'calcola la pagina da visualizzare dopo l'eliminazione
            If (dg_sedute.Items.Count = 1) Then
                If (dg_sedute.CurrentPageIndex <> 0) Then
                    dg_sedute.CurrentPageIndex -= 1
                End If
            End If
            'riassegna gli indici a tb_sedute
            'Dim i As Integer = 1
            'For Each row In tb_sedute.Rows
            '    If Not (row.RowState = DataRowState.Deleted) Then
            '        row.Item("tsd_n_seduta") = i
            '        i += 1
            '    End If
            'Next
            Dim i As Integer = 1
            For Each row In tb_sedute.Rows
                If row.RowState <> DataRowState.Deleted _
                   AndAlso row("tsd_n_seduta") > valueKey(0) Then
                    row("tsd_n_seduta") -= 1
                End If
            Next
            '

            '
            ' --- Associazioni --- '
            '
            ' Elimina i 'figli' in tb_ass_sed  
            For i = tb_ass_sed.Rows.Count - 1 To 0 Step -1
                If (tb_ass_sed.Rows(i).RowState <> DataRowState.Deleted) _
                   AndAlso (tb_ass_sed.Rows(i).Item("sas_n_seduta") = valueKey(0)) Then
                    tb_ass_sed.Rows(i).Delete()
                End If
            Next
            '  Riassegna gli indici a tb_ass_sed
            For Each row In tb_ass_sed.Rows
                If (row.RowState <> DataRowState.Deleted) Then
                    If (row.Item("sas_n_seduta") > valueKey(0)) Then
                        row.Item("sas_n_seduta") -= 1
                    End If
                End If
            Next

            '
            ' --- Vaccinazioni --- '
            '
            Dim codVacTemp As String
            'elimina i 'figli' in tb_vac_sed  
            For i = tb_vac_sed.Rows.Count - 1 To 0 Step -1
                If (tb_vac_sed.Rows(i).RowState <> DataRowState.Deleted) _
                   AndAlso (tb_vac_sed.Rows(i).Item("sed_n_seduta") = valueKey(0)) Then
                    codVacTemp = tb_vac_sed.Rows(i).Item("sed_vac_codice")
                    tb_vac_sed.Rows(i).Delete()
                    Setta_nRichiamo(codVacTemp)
                End If
            Next
            '  riassegna gli indici a tb_vac_sed
            For Each row In tb_vac_sed.Rows
                If (row.RowState <> DataRowState.Deleted) _
                   AndAlso (row.Item("sed_n_seduta") > valueKey(0)) Then
                    row.Item("sed_n_seduta") -= 1
                End If
            Next

            'ricarico il datagrid
            dg_sedute.DataSource = tb_sedute
            dg_sedute.DataBind()
            OnitLayout1.Busy = True
            nMod += 1
        Else
            strJS = "<script language='javascript'>alert('Cliccare AGGIORNA o ANNULLA della riga che si sta modificando prima di eliminare questa riga')</script>"
        End If
    End Sub

    Private Sub dg_sedute_EditCommand(source As System.Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_sedute.EditCommand

        If (dg_sedute.EditItemIndex = -1 And dg_vac_sed.EditItemIndex = -1 And dg_ass_sed.EditItemIndex = -1) Then
            '
            dg_ass_sed.DataSource = Nothing
            dg_ass_sed.DataBind()
            dg_vac_sed.DataSource = Nothing
            dg_vac_sed.DataBind()
            dg_sedute.SelectedIndex = -1
            '
            ToolBar.Enabled = False
            nascosto.Text = "DisNS"
            Dim valueKeyPrec(0) As Object
            Dim valueKeySuc(0) As Object
            valueKeyPrec(0) = Int(CType(e.Item.FindControl("lb_index"), Label).Text) - 1
            valueKeySuc(0) = Int(CType(e.Item.FindControl("lb_index"), Label).Text) + 1
            If valueKeyPrec(0) < 1 Then
                etaPrec.Text = 0
            Else
                Dim row As DataRow = tb_sedute.Rows.Find(valueKeyPrec)
                etaPrec.Text = row("tsd_eta_seduta") + row("tsd_intervallo_prossima")
            End If
            If valueKeySuc(0) > tb_sedute.Rows.Count Then
                etaSuc.Text = "99999"
            Else
                etaSuc.Text = tb_sedute.Rows.Find(valueKeySuc)("tsd_eta_seduta")
            End If

            'abilitazione/disabilitazione del campo TSD_NUM_SOLLECITO a seconda del fatto
            'se la seduta contiene almeno una vaccinazione obbligatoria [modifica 14/02/2005]
            'Dim abilitaNumSolleciti As Boolean
            'abilitaNumSolleciti = ControllaSedutaVacObbligatorie(e.Item.ItemIndex)

            'abilitazione/disabilitazione del campo TSD_POSTICIPO_SEDUTA a seconda del fatto
            'se la seduta contiene almeno una vaccinazione obbligatoria [modifica 24/02/2005]
            'Dim abilitaPosticipoSeduta As Boolean
            'abilitaPosticipoSeduta = ControllaSedutaVacObbligatorie(e.Item.ItemIndex)

            dg_sedute.EditItemIndex = e.Item.ItemIndex
            dg_sedute.DataSource = tb_sedute
            dg_sedute.DataBind()
            'CType(dg_sedute.Items(e.Item.ItemIndex).FindControl("txtNumSolleciti"), TextBox).CssClass = IIf(abilitaNumSolleciti, "TextBox_Stringa", "TextBox_Stringa_disabilitato")
            'CType(dg_sedute.Items(e.Item.ItemIndex).FindControl("txtNumSolleciti"), TextBox).ReadOnly = Not abilitaNumSolleciti

            'aggiunta del valore eliminazione appuntamento [modifica 27/07/2005]
            'CType(dg_sedute.Items(e.Item.ItemIndex).FindControl("ddlPosticipoSeduta"), DropDownList).Enabled = Not abilitaPosticipoSeduta
            'CType(dg_sedute.Items(e.Item.ItemIndex).FindControl("chkPosticipoSeduta"), CheckBox).Enabled = Not abilitaPosticipoSeduta

            OnitLayout1.Busy = True
            nMod += 1
            strJS = "<script language=""javascript"">document.getElementById(""dg_sedute"").firstChild.childNodes[" & (1 + e.Item.ItemIndex) & "].childNodes[4].firstChild.focus()</script>"
        Else
            strJS = "<script language=""javascript"">alert(""Cliccare ANNULLA o AGGIORNA della riga che si sta modificando prima di modificare questa riga"")</script>"
        End If

    End Sub

    Private Sub dg_sedute_CancelCommand(source As System.Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_sedute.CancelCommand

        dg_ass_sed.DataSource = Nothing
        dg_ass_sed.DataBind()
        dg_vac_sed.DataSource = Nothing
        dg_vac_sed.DataBind()
        dg_sedute.SelectedIndex = -1
        '
        If (newrow) Then
            tb_sedute.Rows(tb_sedute.Rows.Count - 1).Delete()
            If (dg_sedute.Items.Count = 1) Then
                If (dg_sedute.CurrentPageIndex <> 0) Then
                    dg_sedute.CurrentPageIndex -= 1
                End If
            End If
            newrow = False
        End If
        dg_sedute.EditItemIndex = -1
        dg_sedute.DataSource = tb_sedute
        dg_sedute.DataBind()
        ToolBar.Enabled = True
        nascosto.Text = ""

        'btn_Ins_sed.Enabled = True
        nMod -= 1
        If (nMod = 0) Then OnitLayout1.Busy = False

    End Sub

    Private Sub dg_sedute_UpdateCommand(source As System.Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_sedute.UpdateCommand
        '
        dg_ass_sed.DataSource = Nothing
        dg_ass_sed.DataBind()
        dg_vac_sed.DataSource = Nothing
        dg_vac_sed.DataBind()
        dg_sedute.SelectedIndex = -1
        '
        Dim etaTemp As Integer
        Dim strTemp_gg As String
        Dim strTemp_mm As String
        Dim strTemp_aa As String
        Dim strTemp As String
        Dim valueKey(0) As Object

        valueKey(0) = CType(e.Item.FindControl("lb_index_edit"), Label).Text

        tb_sedute.Rows.Find(valueKey)("tsd_intervallo") = CType(e.Item.FindControl("tb_int_edit"), TextBox).Text
        strTemp = CType(e.Item.FindControl("tb_val_edit"), TextBox).Text
        If (strTemp = "") Then
            tb_sedute.Rows.Find(valueKey)("tsd_validita") = DBNull.Value
        Else
            tb_sedute.Rows.Find(valueKey)("tsd_validita") = strTemp
        End If

        tb_sedute.Rows.Find(valueKey)("tsd_durata_seduta") = CType(e.Item.FindControl("tb_dur_edit"), TextBox).Text

        strTemp_gg = CType(e.Item.FindControl("tb_gg_edit"), TextBox).Text
        strTemp_mm = CType(e.Item.FindControl("tb_mm_edit"), TextBox).Text
        strTemp_aa = CType(e.Item.FindControl("tb_aa_edit"), TextBox).Text
        If (strTemp_aa = "" And strTemp_gg = "" And strTemp_mm = "") Then
            tb_sedute.Rows.Find(valueKey)("tsd_eta_seduta") = DBNull.Value
        Else
            If (strTemp_gg <> "") Then etaTemp += Int(strTemp_gg)
            If (strTemp_mm <> "") Then etaTemp += Int(strTemp_mm) * 30
            If (strTemp_aa <> "") Then etaTemp += Int(strTemp_aa) * 360
            tb_sedute.Rows.Find(valueKey)("tsd_eta_seduta") = etaTemp
        End If
        etaTemp = 0
        strTemp_gg = CType(e.Item.FindControl("tb_pros_gg_edit"), TextBox).Text
        strTemp_mm = CType(e.Item.FindControl("tb_pros_mm_edit"), TextBox).Text
        strTemp_aa = CType(e.Item.FindControl("tb_pros_aa_edit"), TextBox).Text
        If (strTemp_aa = "" And strTemp_gg = "" And strTemp_mm = "") Then
            tb_sedute.Rows.Find(valueKey)("tsd_intervallo_prossima") = 0
        Else
            If (strTemp_gg <> "") Then etaTemp += Int(strTemp_gg)
            If (strTemp_mm <> "") Then etaTemp += Int(strTemp_mm) * 30
            If (strTemp_aa <> "") Then etaTemp += Int(strTemp_aa) * 360
            tb_sedute.Rows.Find(valueKey)("tsd_intervallo_prossima") = etaTemp
        End If
        'brgoz 18-01-05
        'tb_sedute.Rows.Find(valueKey)("tsd_num_solleciti_rac") = CType(e.Item.FindControl("txtNumSolleciti"), TextBox).Text
        'parametri associati alle sedute [modifica 29/03/2006]
        If Trim(CType(e.Item.FindControl("txtNumSollecitiRac"), TextBox).Text) <> "" Then
            tb_sedute.Rows.Find(valueKey)("tsd_num_solleciti_rac") = CType(e.Item.FindControl("txtNumSollecitiRac"), TextBox).Text
        Else
            tb_sedute.Rows.Find(valueKey)("tsd_num_solleciti_rac") = DBNull.Value
        End If
        'tb_sedute.Rows.Find(valueKey)("tsd_num_solleciti_rac") = IIf(CType(e.Item.FindControl("txtNumSollecitiRac"), TextBox).Text <> "", CType(e.Item.FindControl("txtNumSollecitiRac"), TextBox).Text, DBNull.Value)
        If Trim(CType(e.Item.FindControl("txtNumSolleciti"), TextBox).Text) <> "" Then
            tb_sedute.Rows.Find(valueKey)("tsd_num_solleciti") = CType(e.Item.FindControl("txtNumSolleciti"), TextBox).Text
        Else
            tb_sedute.Rows.Find(valueKey)("tsd_num_solleciti") = DBNull.Value
        End If
        'tb_sedute.Rows.Find(valueKey)("tsd_num_solleciti") = IIf(CType(e.Item.FindControl("txtNumSolleciti"), TextBox).Text <> "", CType(e.Item.FindControl("txtNumSolleciti"), TextBox).Text, DBNull.Value)
        If CType(e.Item.FindControl("txtGiorniPosticipo"), TextBox).Text <> "" Then
            tb_sedute.Rows.Find(valueKey)("tsd_giorni_posticipo") = CType(e.Item.FindControl("txtGiorniPosticipo"), TextBox).Text
        Else
            tb_sedute.Rows.Find(valueKey)("tsd_giorni_posticipo") = DBNull.Value
        End If
        If CType(e.Item.FindControl("txtSollecitiNonObbligatori"), TextBox).Text <> "" Then
            tb_sedute.Rows.Find(valueKey)("tsd_num_soll_non_obbl") = CType(e.Item.FindControl("txtSollecitiNonObbligatori"), TextBox).Text
        Else
            tb_sedute.Rows.Find(valueKey)("tsd_num_soll_non_obbl") = DBNull.Value
        End If

        tb_sedute.Rows.Find(valueKey)("tsd_posticipo_seduta") = CType(e.Item.FindControl("ddlPosticipoSeduta"), DropDownList).SelectedValue

        'aggiunta del campo per il posticipo delle vaccinazioni non obbligatorie [modifica 24/02/2005]
        'tb_sedute.Rows.Find(valueKey)("tsd_posticipo_seduta") = IIf(CType(e.Item.FindControl("chkPosticipoSeduta"), CheckBox).Checked, "S", "N")
        'aggiunta del valore eliminazione appuntamento [modifica 27/07/2005]
        tb_sedute.Rows.Find(valueKey)("tsd_note") = CType(e.Item.FindControl("tb_note_edit"), TextBox).Text

        Dim indiceModificato As Integer = dg_sedute.EditItemIndex
        dg_sedute.EditItemIndex = -1
        'dg_sedute.SelectedIndex = indiceModificato
        dg_sedute.DataSource = tb_sedute
        dg_sedute.DataBind()

        ToolBar.Enabled = True
        nascosto.Text = ""
        etaPrec.Text = ""
        'btn_Ins_sed.Enabled = True
        newrow = False
    End Sub

    'individua l'elemento della dropdownlist selezionata [modifica 27/07/2005]
    Private Sub dg_sedute_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dg_sedute.ItemDataBound
        If e.Item.ItemIndex <> -1 AndAlso Not e.Item.FindControl("ddlPosticipoSeduta") Is Nothing Then
            CType(e.Item.FindControl("ddlPosticipoSeduta"), DropDownList).SelectedValue = tb_sedute.Rows(e.Item.ItemIndex)("TSD_POSTICIPO_SEDUTA")
        End If
        'associa all'immagine dei parametri il numero della seduta [modifica 28/03/2006]

    End Sub

    'visualizza la modale con i parametri associati alla seduta [modifica 28/03/2006]
    Private Sub dg_sedute_ItemCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_sedute.ItemCommand

        Select Case e.CommandName
            Case "VisualizzaParametri"
                'per visualizzare i parametri devo controllare se sono presenti vaccinazioni [modifica 15/05/2006]
                Dim nSed As Integer = CType(dg_sedute.Items(e.Item.ItemIndex).FindControl("lb_index_edit"), Label).Text
                tb_vac_sed.DefaultView.RowFilter = "sed_n_seduta = " & nSed
                If tb_vac_sed.DefaultView.Count > 0 Then
                    fmVisualizzaParametri.Title = "Parametri associati alla Seduta " & e.CommandArgument
                    CaricaParametriSeduta(e.Item.ItemIndex)
                    fmVisualizzaParametri.VisibileMD = True
                Else
                    OnitLayout1.InsertRoutineJS("alert('Attenzione: è necessario specificare le vaccinazioni associate al ciclo per impostarne i parametri!');")
                End If
                tb_vac_sed.DefaultView.RowFilter = ""
        End Select

    End Sub

    Private Sub dg_ass_sed_EditCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_ass_sed.EditCommand

        If dg_sedute.EditItemIndex = -1 And dg_vac_sed.EditItemIndex = -1 And dg_ass_sed.EditItemIndex = -1 Then
            tipoAss = CType(dg_ass_sed.Items(e.Item.ItemIndex).FindControl("tb_codAss"), Label).Text
            dg_ass_sed.EditItemIndex = e.Item.ItemIndex
            dg_ass_sed.DataSource = tb_ass_sed.DefaultView
            dg_ass_sed.DataBind()
            ToolBar.Enabled = False
            nascosto.Text = "DisNA"

            ' --------- Impostazione filtro modale --------- '
            Dim filtro As String = ""
            Dim i As Integer
            Dim stbFiltroAss As New System.Text.StringBuilder
            Dim stbFiltroVac As New System.Text.StringBuilder
            '
            ' Elenco associazioni già presenti nella seduta
            For i = 0 To tb_ass_sed.DefaultView.Count - 2
                If tb_ass_sed.DefaultView.Item(i).Row.RowState <> DataRowState.Deleted Then
                    stbFiltroAss.AppendFormat("'{0}',", tb_ass_sed.DefaultView.Item(i)("sas_ass_codice"))
                End If
            Next

            ' Elenco vaccinazioni già presenti nella seduta
            Dim dvVaccAss As New DataView(tb_vac_sed)
            dvVaccAss.RowFilter = "sed_n_seduta=" + nSed.ToString ' vaccinazioni associate alla seduta
            For i = 0 To dvVaccAss.Count - 1
                If dvVaccAss(i).Row.RowState <> DataRowState.Deleted Then
                    stbFiltroVac.AppendFormat("'{0}',", dvVaccAss(i)("sed_vac_codice"))
                End If
            Next

            ' Filtro associazioni già presenti
            If stbFiltroAss.Length > 0 Then
                filtro = "ass_codice not in (" + stbFiltroAss.Remove(stbFiltroAss.Length - 1, 1).ToString + ")"
            End If

            ' Filtro vaccinazioni associate ad associazioni già presenti
            If stbFiltroVac.Length > 0 Then
                If filtro <> "" Then
                    filtro += " and "
                End If
                filtro += "ass_codice not in (select val_ass_codice from t_ana_link_ass_vaccinazioni where val_vac_codice in (" + stbFiltroVac.Remove(stbFiltroVac.Length - 1, 1).ToString + ") )"
            End If

            ' Order by
            If filtro = "" Then
                filtro += "'true'='true'"
            End If
            filtro += " order by ass_descrizione"

            ' Imposto il filtro della modale
            CType(dg_ass_sed.Items(dg_ass_sed.EditItemIndex).FindControl("fm_ass_edit"), Onit.Controls.OnitModalList).Filtro = filtro
            strJS = "<script language=""javascript"">document.getElementById(""dg_ass_sed"").firstChild.childNodes[" & (1 + dg_ass_sed.EditItemIndex) & "].childNodes[4].firstChild.firstChild.focus()</script>"
            OnitLayout1.Busy = True
            nMod += 1
            '
            ' In edit, non visualizzo le vaccinazioni associate
            dg_vac_sed.DataSource = Nothing
            dg_vac_sed.DataBind()
            ' seleziono la riga
            dg_ass_sed.SelectedIndex = e.Item.ItemIndex
        Else
            strJS = "<script language=""javascript"">alert(""Cliccare ANNULLA o AGGIORNA della riga che si sta modificando prima di modificare questa riga"")</script>"
        End If

    End Sub

    Private Sub dg_ass_sed_CancelCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_ass_sed.CancelCommand

        If newrow Then
            Dim valueKeys(1) As Object
            valueKeys(0) = nSed
            valueKeys(1) = "x"
            tb_ass_sed.Rows.Find(valueKeys).Delete()
            If (dg_ass_sed.Items.Count = 1) Then
                If (dg_ass_sed.CurrentPageIndex <> 0) Then
                    dg_ass_sed.CurrentPageIndex -= 1
                End If
            End If
            newrow = False
        End If

        dg_ass_sed.EditItemIndex = -1
        dg_ass_sed.DataSource = tb_ass_sed.DefaultView
        dg_ass_sed.DataBind()

        ToolBar.Enabled = True
        nascosto.Text = ""
        nMod -= 1

        ' Aggiorna le vaccinazioni relative (se ci sono associazioni)
        If dg_ass_sed.Items.Count > 0 Then
            If dg_ass_sed.Items.Count > e.Item.ItemIndex Then
                dg_ass_sed.SelectedIndex = e.Item.ItemIndex
            Else
                dg_ass_sed.SelectedIndex = dg_ass_sed.Items.Count - 1
            End If
            dg_ass_sed_SelectedIndexChanged(Nothing, Nothing)
        Else
            tb_vac_sed.DefaultView.RowFilter = String.Format("sed_n_seduta = {0} and sas_ass_codice is null", nSed)
            dg_vac_sed.DataSource = tb_vac_sed.DefaultView
            dg_vac_sed.DataBind()
        End If

        If nMod = 0 Then OnitLayout1.Busy = False

    End Sub

    Private Sub dg_ass_sed_DeleteCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_ass_sed.DeleteCommand
        If (dg_sedute.EditItemIndex = -1 And dg_vac_sed.EditItemIndex = -1 And dg_ass_sed.EditItemIndex = -1) Then
            'calcola il numero di riga da eliminare
            Dim valueKeys(1) As Object
            valueKeys(0) = nSed
            valueKeys(1) = CType(dg_ass_sed.Items(e.Item.ItemIndex).FindControl("tb_codAss"), Label).Text
            '
            tb_ass_sed.Rows.Find(valueKeys).Delete()
            'Setta_nRichiamoAss(valueKeys(1))
            'calcola la pagina da visualizzare dopo l'eliminazione
            If (dg_ass_sed.Items.Count = 1) Then
                If (dg_ass_sed.CurrentPageIndex <> 0) Then
                    dg_ass_sed.CurrentPageIndex -= 1
                End If
            End If

            ' Elimino anche le vaccinazioni associate alla seduta
            Dim idxVac As Integer
            Dim codVacTemp As String
            For idxVac = tb_vac_sed.Rows.Count - 1 To 0 Step -1
                If (tb_vac_sed.Rows(idxVac).RowState <> DataRowState.Deleted) _
                   AndAlso (tb_vac_sed.Rows(idxVac)("sed_n_seduta").ToString = nSed) _
                   AndAlso tb_vac_sed.Rows(idxVac)("sas_ass_codice").ToString = valueKeys(1) Then
                    '
                    codVacTemp = tb_vac_sed.Rows(idxVac).Item("sed_vac_codice")
                    tb_vac_sed.Rows(idxVac).Delete()
                    Setta_nRichiamo(codVacTemp)
                    '
                End If
            Next

            'ricarico il datagrid
            dg_ass_sed.DataSource = tb_ass_sed.DefaultView
            dg_ass_sed.DataBind()

            ' CHIEDERE A DENIS SE CI VA O NO
            'controllo se è rimasta almeno una vaccinazione per l'associazione dei parametri [modifica 15/05/2006]
            'ControllaVaccinazioniRimaste()

            ' Selezione riga
            If dg_ass_sed.Items.Count > 0 Then
                If dg_ass_sed.Items.Count = 1 Then
                    dg_ass_sed.SelectedIndex = 0
                Else
                    dg_ass_sed.SelectedIndex = e.Item.ItemIndex - 1
                End If
                dg_ass_sed_SelectedIndexChanged(Nothing, Nothing)
            Else
                tb_vac_sed.DefaultView.RowFilter = String.Format("sed_n_seduta = {0} and sas_ass_codice is null", nSed)
                dg_vac_sed.DataSource = tb_vac_sed.DefaultView
                dg_vac_sed.DataBind()
            End If
            '
            OnitLayout1.Busy = True
            nMod += 1
        Else
            strJS = "<script language='javascript'>alert('Cliccare AGGIORNA o ANNULLA della riga che si sta modificando prima di eliminare questa riga')</script>"
        End If
    End Sub

    Private Sub dg_ass_sed_PageIndexChanged(source As Object, e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dg_ass_sed.PageIndexChanged

        If dg_sedute.EditItemIndex = -1 And dg_vac_sed.EditItemIndex = -1 And dg_ass_sed.EditItemIndex = -1 Then

            dg_ass_sed.CurrentPageIndex = e.NewPageIndex
            dg_ass_sed.DataSource = tb_ass_sed.DefaultView
            dg_ass_sed.DataBind()

            ' Aggiorna le vaccinazioni associate
            dg_ass_sed.SelectedIndex = -1
            dg_ass_sed_SelectedIndexChanged(Nothing, Nothing)

        Else
            strJS = "<script language='javascript'>alert('Cliccare ANNULLA o AGGIORNA della riga che si sta modificando prima di cambiare pagina')</script>"
        End If

    End Sub

    Private Sub dg_ass_sed_UpdateCommand(source As Object, e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dg_ass_sed.UpdateCommand

        If Not newrow Then
            Dim valueKeys(1) As Object
            Dim row As DataRow
            valueKeys(0) = nSed
            valueKeys(1) = tipoAss
            row = tb_ass_sed.Rows.Find(valueKeys)
            row("sas_ass_codice") = CType(e.Item.FindControl("fm_ass_edit"), Onit.Controls.OnitModalList).Codice
            row("ass_descrizione") = CType(e.Item.FindControl("fm_ass_edit"), Onit.Controls.OnitModalList).Descrizione
            row("sas_n_richiamo") = CType(e.Item.FindControl("tb_n_ric_edit_ass"), Label).Text
            row("sas_sii_codice") = CType(e.Item.FindControl("fm_sii_edit_ass"), Onit.Controls.OnitModalList).Codice
            row("sii_descrizione") = CType(e.Item.FindControl("fm_sii_edit_ass"), Onit.Controls.OnitModalList).Descrizione

            row("sas_vii_codice") = CType(e.Item.FindControl("fm_vii_edit_ass"), Onit.Controls.OnitModalList).Codice
            row("vii_descrizione") = CType(e.Item.FindControl("fm_vii_edit_ass"), Onit.Controls.OnitModalList).Descrizione

            ' Se ho modificato il codice dell'associazione, devo cancellare tutte le vaccinazioni 
            ' relative all'associazione vecchia ed aggiungere quelle relative alla nuova
            If tipoAss <> row("sas_ass_codice") Then
                Dim idxVac As Integer
                Dim codVacTemp As String
                For idxVac = tb_vac_sed.Rows.Count - 1 To 0 Step -1
                    If (tb_vac_sed.Rows(idxVac).RowState <> DataRowState.Deleted) _
                       AndAlso (tb_vac_sed.Rows(idxVac)("sed_n_seduta").ToString = nSed) _
                       AndAlso tb_vac_sed.Rows(idxVac)("sas_ass_codice").ToString = tipoAss Then
                        '
                        codVacTemp = tb_vac_sed.Rows(idxVac).Item("sed_vac_codice")
                        tb_vac_sed.Rows(idxVac).Delete()
                        Setta_nRichiamo(codVacTemp)
                        '
                    End If
                Next

                ' Se ho inserito una nuova associazione, devo caricare le vaccinazioni associate
                Dim dtVac As DataTable = CaricaVacc_Associazione(row("sas_ass_codice"))
                Dim i As Integer
                Dim new_row As DataRow
                For i = 0 To dtVac.Rows.Count - 1
                    new_row = tb_vac_sed.NewRow()
                    new_row("vac_descrizione") = dtVac.Rows(i)("vac_descrizione").ToString
                    'new_row("sii_descrizione") = tb_ass_sed.Rows(tb_ass_sed.Rows.Count - 1)("sii_descrizione").ToString
                    new_row("sed_cic_codice") = codCiclo
                    new_row("sed_n_seduta") = nSed
                    new_row("sed_vac_codice") = dtVac.Rows(i)("vac_codice").ToString
                    'new_row("sed_n_richiamo") = tb_ass_sed.Rows(tb_ass_sed.Rows.Count - 1)("sas_n_richiamo")
                    'new_row("sed_sii_codice") = tb_ass_sed.Rows(tb_ass_sed.Rows.Count - 1)("sas_sii_codice").ToString
                    new_row("vac_obbligatoria") = dtVac.Rows(i)("vac_obbligatoria").ToString
                    new_row("cancellata") = 0
                    new_row("sas_ass_codice") = row("sas_ass_codice")
                    '
                    tb_vac_sed.Rows.Add(new_row)
                    '
                    Setta_nRichiamo(new_row("sed_vac_codice"))
                Next

            End If
        Else
            Dim cod_new_ass As String = CType(e.Item.FindControl("fm_ass_edit"), Onit.Controls.OnitModalList).Codice
            tb_ass_sed.Rows(tb_ass_sed.Rows.Count - 1)("sas_ass_codice") = cod_new_ass
            tb_ass_sed.Rows(tb_ass_sed.Rows.Count - 1)("sas_n_richiamo") = 0
            tb_ass_sed.Rows(tb_ass_sed.Rows.Count - 1)("sas_sii_codice") = CType(e.Item.FindControl("fm_sii_edit_ass"), Onit.Controls.OnitModalList).Codice
            tb_ass_sed.Rows(tb_ass_sed.Rows.Count - 1)("sii_descrizione") = CType(e.Item.FindControl("fm_sii_edit_ass"), Onit.Controls.OnitModalList).Descrizione
            tb_ass_sed.Rows(tb_ass_sed.Rows.Count - 1)("sas_vii_codice") = CType(e.Item.FindControl("fm_vii_edit_ass"), Onit.Controls.OnitModalList).Codice
            tb_ass_sed.Rows(tb_ass_sed.Rows.Count - 1)("vii_descrizione") = CType(e.Item.FindControl("fm_vii_edit_ass"), Onit.Controls.OnitModalList).Descrizione
            tb_ass_sed.Rows(tb_ass_sed.Rows.Count - 1)("ass_descrizione") = CType(e.Item.FindControl("fm_ass_edit"), Onit.Controls.OnitModalList).Descrizione
            '
            'Setta_nRichiamoAss(tb_ass_sed.Rows(tb_ass_sed.Rows.Count - 1)("sas_ass_codice"))
            '
            ' Se ho inserito una nuova associazione, devo caricare le vaccinazioni associate
            Dim dtVac As DataTable = CaricaVacc_Associazione(cod_new_ass)
            Dim i As Integer
            Dim new_row As DataRow
            For i = 0 To dtVac.Rows.Count - 1
                new_row = tb_vac_sed.NewRow()
                '
                new_row("vac_descrizione") = dtVac.Rows(i)("vac_descrizione").ToString
                new_row("sed_cic_codice") = codCiclo
                new_row("sed_n_seduta") = nSed
                new_row("sed_vac_codice") = dtVac.Rows(i)("vac_codice").ToString
                new_row("sed_n_richiamo") = tb_ass_sed.Rows(tb_ass_sed.Rows.Count - 1)("sas_n_richiamo")
                new_row("sed_sii_codice") = tb_ass_sed.Rows(tb_ass_sed.Rows.Count - 1)("sas_sii_codice").ToString
                new_row("sii_descrizione") = tb_ass_sed.Rows(tb_ass_sed.Rows.Count - 1)("sii_descrizione").ToString
                new_row("sas_vii_codice") = tb_ass_sed.Rows(tb_ass_sed.Rows.Count - 1)("sas_vii_codice").ToString
                new_row("vii_descrizione") = tb_ass_sed.Rows(tb_ass_sed.Rows.Count - 1)("vii_descrizione").ToString
                new_row("vac_obbligatoria") = dtVac.Rows(i)("vac_obbligatoria").ToString
                new_row("cancellata") = 0
                new_row("sas_ass_codice") = cod_new_ass
                '
                tb_vac_sed.Rows.Add(new_row)
                '
                Setta_nRichiamo(new_row("sed_vac_codice"))
            Next
            '
        End If

        'Setta_nRichiamoAss(tb_ass_sed.Rows(tb_ass_sed.Rows.Count - 1)("sas_ass_codice"))
        dg_ass_sed.EditItemIndex = -1
        dg_ass_sed.SelectedIndex = -1
        dg_ass_sed.DataSource = tb_ass_sed.DefaultView
        dg_ass_sed.DataBind()
        ToolBar.Enabled = True
        nascosto.Text = ""
        'btn_Ins_vac.Enabled = True
        newrow = False
        tipoAss = ""
        '
        ' Aggiorna le vaccinazioni associate
        If dg_ass_sed.Items.Count > 0 Then
            dg_ass_sed.SelectedIndex = e.Item.ItemIndex
            dg_ass_sed_SelectedIndexChanged(Nothing, Nothing)
        Else
            tb_vac_sed.DefaultView.RowFilter = String.Format("sed_n_seduta = {0} and sas_ass_codice is null", nSed)
            dg_vac_sed.DataSource = tb_vac_sed.DefaultView
            dg_vac_sed.DataBind()
        End If

        ' CHIEDERE A DENIS
        ''valorizzazione parametri a seconda delle vaccinazioni rimaste [modifica15/05/2006]
        'ControllaVaccinazioniRimaste()
    End Sub

    Private Sub dg_ass_sed_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles dg_ass_sed.SelectedIndexChanged

        If (dg_sedute.EditItemIndex = -1 And dg_vac_sed.EditItemIndex = -1 And dg_ass_sed.EditItemIndex = -1) Then

            ' Visualizzo le vaccinazioni relative all'associazione selezionata
            Dim cod_ass As String
            If Not IsNothing(dg_ass_sed.SelectedItem) Then
                If Not IsNothing(dg_ass_sed.SelectedItem.FindControl("tb_codAss")) Then
                    cod_ass = DirectCast(dg_ass_sed.SelectedItem.FindControl("tb_codAss"), Label).Text
                Else
                    If Not IsNothing(dg_ass_sed.SelectedItem.FindControl("fm_ass_edit")) Then
                        cod_ass = DirectCast(dg_ass_sed.SelectedItem.FindControl("fm_ass_edit"), Onit.Controls.OnitModalList).Codice
                    Else
                        cod_ass = ""
                    End If
                End If
            Else
                'Se ho cambiato pagina il selectedindex è nothing
                cod_ass = ""
            End If

            dg_vac_sed.Visible = True
            sezioneVac.Visible = True
            dg_vac_sed.CurrentPageIndex = 0

            ' Filtro per visualizzare le vaccinazioni della seduta selezionata e relative all'associazione selezionata
            tb_vac_sed.DefaultView.RowFilter = String.Format("sed_n_seduta = {0} and sas_ass_codice = '{1}'", nSed, cod_ass)
            dg_vac_sed.DataSource = tb_vac_sed.DefaultView
            dg_vac_sed.DataBind()
        Else
            strJS = "<script language='javascript'>alert('Cliccare ANNULLA o AGGIORNA della riga che si sta modificando prima di cambiare riga')</script>"
        End If

    End Sub

#End Region

#Region " Public Methods "

    'visualizza l'immagine associata ai parametri della seduta [modifica 28/03/2006]
    Public Function VisualizzaImmagineParametri(cont As Object) As Boolean

        If Not IsNumeric(cont) Then

            Dim item As Object = DataBinder.Eval(cont, "DataItem")

            If Not item("tsd_num_solleciti") Is DBNull.Value Then Return True
            If Not item("tsd_num_solleciti_rac") Is DBNull.Value Then Return True
            If Not item("tsd_giorni_posticipo") Is DBNull.Value AndAlso CInt(item("tsd_giorni_posticipo")) > 0 Then Return True
            If Not item("tsd_num_soll_non_obbl") Is DBNull.Value AndAlso CInt(item("tsd_num_soll_non_obbl")) > 0 Then Return True
            If Not item("tsd_posticipo_seduta") Is DBNull.Value AndAlso item("tsd_posticipo_seduta") & "" <> "N" Then Return True

        Else

            If Trim(CType(dg_sedute.Items(cont).FindControl("txtNumSollecitiRac"), TextBox).Text) <> "" Then Return True
            If Trim(CType(dg_sedute.Items(cont).FindControl("txtNumSolleciti"), TextBox).Text) <> "" Then Return True
            If CType(dg_sedute.Items(cont).FindControl("txtGiorniPosticipo"), TextBox).Text > 0 Then Return True
            If CType(dg_sedute.Items(cont).FindControl("txtSollecitiNonObbligatori"), TextBox).Text > 0 Then Return True
            If CType(dg_sedute.Items(cont).FindControl("ddlPosticipoSeduta"), DropDownList).SelectedValue <> "N" Then Return True

        End If

        Return False

    End Function

    Public Function Conv(n As Object, tipo As String)

        If Not n Is DBNull.Value Then
            Dim _eta As New Entities.Eta(n)
            Select Case tipo
                Case "aa"
                    Return _eta.Anni.ToString()
                Case "mm"
                    Return _eta.Mesi.ToString()
                Case "gg"
                    Return _eta.Giorni.ToString()
            End Select
        End If

        Return String.Empty

    End Function

    'individua l'elemento della dropdownlist selezionato e lo assegna alla label [modifica 27/07/2005]
    Public Function SelezionaPosticipoSeduta(sel As String) As String

        Select Case sel
            Case "N"
                Return "NO"
            Case "S"
                Return "SI"
            Case "E"
                Return "APP"
        End Select

        Return String.Empty

    End Function

#End Region

#Region " Private Methods "

    Private Sub CaricaDati()

        ' Sedute
        If Not tb_sedute Is Nothing Then
            tb_sedute.Dispose()
        End If
        tb_sedute = New DataTable()

        ' Vaccinazioni
        If Not tb_vac_sed Is Nothing Then
            tb_vac_sed.Dispose()
        End If
        tb_vac_sed = New DataTable()

        ' Associazioni
        If Not tb_ass_sed Is Nothing Then
            tb_ass_sed.Dispose()
        End If
        tb_ass_sed = New DataTable()

        ' --------- Caricamento Sedute --------- '
        Using DAM As IDAM = OnVacUtility.OpenDam()

            DAM.QB.NewQuery()
            DAM.QB.AddTables("t_ana_tempi_sedute")
            DAM.QB.AddSelectFields("*")
            DAM.QB.AddWhereCondition("tsd_cic_codice", Comparatori.Uguale, codCiclo, DataTypes.Stringa)
            DAM.QB.AddOrderByFields("tsd_n_seduta")

            DAM.BuildDataTable(tb_sedute)

            dg_sedute.DataSource = tb_sedute
            dg_sedute.DataBind()

            ''-- Vaccinazioni relative alle associazioni
            ''SELECT vac_descrizione, sii_descrizione, sas_cic_codice AS sed_cic_codice, sas_n_seduta AS sed_n_seduta, 
            ''	   sas_vac_codice AS sed_vac_codice, sas_n_richiamo AS sed_n_richiamo, sas_sii_codice AS sed_sii_codice, 
            ''	   vac_obbligatoria, 0 CANCELLATA, sas_ass_codice
            ''FROM T_ANA_ASSOCIAZIONI_SEDUTE, T_ANA_VACCINAZIONI, T_ANA_SITI_INOCULAZIONE 
            ''WHERE sas_cic_codice='CICPRO2'
            ''--AND sas_ass_codice='DT'
            ''--AND sas_n_seduta=1
            ''AND sas_vac_codice = vac_codice
            ''AND sas_sii_codice = sii_codice(+)
            ''UNION
            ''-- Vaccinazioni non relative ad associazioni
            ''SELECT vac_descrizione, sii_descrizione, sed_cic_codice, sed_n_seduta, sed_vac_codice, sed_n_richiamo, 
            ''	   sed_sii_codice, vac_obbligatoria, 0 CANCELLATA, NULL AS sas_ass_codice
            ''FROM T_ANA_VACCINAZIONI_SEDUTE, T_ANA_VACCINAZIONI, T_ANA_SITI_INOCULAZIONE
            ''WHERE sed_cic_codice = 'CICPRO2'
            ''AND sed_vac_codice = vac_codice
            ''AND sed_sii_codice = sii_codice(+)
            '
            DAM.QB.NewQuery()
            DAM.QB.AddSelectFields("vac_descrizione", "sas_cic_codice AS sed_cic_codice", "sas_n_seduta AS sed_n_seduta", "sas_vac_codice AS sed_vac_codice", "sas_n_richiamo AS sed_n_richiamo", "sas_sii_codice AS sed_sii_codice", "sii_descrizione", "sas_vii_codice", "vii_descrizione", "vac_obbligatoria", "0 CANCELLATA", "sas_ass_codice")
            DAM.QB.AddTables("t_ana_associazioni_sedute", "t_ana_vaccinazioni", "t_ana_siti_inoculazione", "t_ana_vie_somministrazione")
            DAM.QB.AddWhereCondition("sas_cic_codice", Comparatori.Uguale, codCiclo, DataTypes.Stringa)
            DAM.QB.AddWhereCondition("sas_vac_codice", Comparatori.Uguale, "vac_codice", DataTypes.Join)
            DAM.QB.AddWhereCondition("sas_sii_codice", Comparatori.Uguale, "sii_codice", DataTypes.OutJoinLeft)
            DAM.QB.AddWhereCondition("vii_codice", Comparatori.Uguale, "sas_vii_codice", DataTypes.OutJoinRight)

            DAM.QB.AddUnion(DAM.QB.GetSelect())

            DAM.QB.NewQuery(False, False)
            DAM.QB.AddSelectFields("vac_descrizione", "sed_cic_codice", "sed_n_seduta", "sed_vac_codice", "sed_n_richiamo", "sed_sii_codice", "sii_descrizione", "null as sas_vii_codice", "null as vii_descrizione", "vac_obbligatoria", "0 CANCELLATA", "NULL AS sas_ass_codice")
            DAM.QB.AddTables("t_ana_vaccinazioni_sedute", "t_ana_vaccinazioni", "t_ana_siti_inoculazione")
            DAM.QB.AddWhereCondition("sed_cic_codice", Comparatori.Uguale, codCiclo, DataTypes.Stringa)
            DAM.QB.AddWhereCondition("sed_vac_codice", Comparatori.Uguale, "vac_codice", DataTypes.Join)
            DAM.QB.AddWhereCondition("sed_sii_codice", Comparatori.Uguale, "sii_codice", DataTypes.OutJoinLeft)
            '
            DAM.QB.AddUnion(DAM.QB.GetSelect())
            '
            DAM.BuildDataTable(tb_vac_sed)

            ' --------- Caricamento Associazioni --------- '
            DAM.QB.NewQuery()
            DAM.QB.IsDistinct = True
            DAM.QB.AddTables("t_ana_associazioni_sedute,t_ana_associazioni,t_ana_siti_inoculazione", "t_ana_vie_somministrazione")
            DAM.QB.AddSelectFields("ass_descrizione", "sas_cic_codice", "sas_n_seduta", "sas_ass_codice", "null sas_n_richiamo", "sas_sii_codice", "sii_descrizione", "sas_vii_codice", "vii_descrizione")
            DAM.QB.AddWhereCondition("sas_cic_codice", Comparatori.Uguale, codCiclo, DataTypes.Stringa)
            DAM.QB.AddWhereCondition("sas_ass_codice", Comparatori.Uguale, "ass_codice", DataTypes.Join)
            DAM.QB.AddWhereCondition("sii_codice", Comparatori.Uguale, "sas_sii_codice", DataTypes.OutJoinRight)
            DAM.QB.AddWhereCondition("vii_codice", Comparatori.Uguale, "sas_vii_codice", DataTypes.OutJoinRight)

            DAM.BuildDataTable(tb_ass_sed)

        End Using

        nSed = -1

        Dim vac_keys(1) As DataColumn
        vac_keys(0) = tb_vac_sed.Columns("sed_n_seduta")
        vac_keys(1) = tb_vac_sed.Columns("sed_vac_codice")
        tb_vac_sed.PrimaryKey = vac_keys

        Dim sed_key(0) As DataColumn
        sed_key(0) = tb_sedute.Columns("tsd_n_seduta")
        tb_sedute.PrimaryKey = sed_key

        Dim ass_key(1) As DataColumn
        ass_key(0) = tb_ass_sed.Columns("sas_n_seduta")
        ass_key(1) = tb_ass_sed.Columns("sas_ass_codice")
        tb_ass_sed.PrimaryKey = ass_key

    End Sub

    'controllo: se nella seduta le vaccinazioni sono tutte non obbligatorie,
    'non deve essere valorizzato il n° sollecito [modifica 14/02/2005]
    Private Function ControllaSedutaVacObbligatorie(indice As Integer) As Boolean

        tb_vac_sed.DefaultView.RowFilter = "sed_n_seduta = " & indice + 1

        For count As Integer = 0 To tb_vac_sed.DefaultView.Count - 1
            If tb_vac_sed.DefaultView.Item(count).Item("vac_obbligatoria") = "A" Then Return True
        Next

        Return False

    End Function

    Private Sub Setta_nRichiamo(newcodVac As String)
        Dim vac_index As Hashtable = New Hashtable
        Dim filtro As String
        vac_index.Add(newcodVac, 1)
        filtro = "sed_vac_codice ='" & newcodVac & "'"
        If (Not newrow And tipoVac <> "") Then
            If (newcodVac <> tipoVac) Then
                vac_index.Add(tipoVac, 1)
                filtro = filtro & " or sed_vac_codice='" & tipoVac & "'"
            End If
        End If
        Dim dw As DataView = New DataView(tb_vac_sed)
        dw.RowFilter = filtro
        dw.Sort = "sed_n_seduta"
        Dim item As DataRowView
        For Each item In dw
            item("sed_n_richiamo") = vac_index.Item(item("sed_vac_codice"))
            vac_index.Item(item("sed_vac_codice")) += 1
        Next

    End Sub

    Private Sub Salva()

        Dim rowVac As DataRow
        Dim rowSed As DataRow
        Dim rowAss As DataRow
        Dim vacCnt As Int16

        Dim DAM As IDAM = OnVacUtility.OpenDam()

        DAM.BeginTrans()



        '                                              '
        ' ------------------ SEDUTE ------------------ '
        '                                              '
        Dim dtSed_Deleted As DataTable = tb_sedute.GetChanges(DataRowState.Deleted)
        Dim dtSed_Modified As DataTable = tb_sedute.GetChanges(DataRowState.Modified)
        Dim dtSed_Added As DataTable = tb_sedute.GetChanges(DataRowState.Added)
        '
        If Not IsNothing(dtSed_Deleted) Then
            For Each rowSed In dtSed_Deleted.Rows

                ' SIO 05/09/2003
                ' Per gestione FK
                ' Elimino la vaccinazioni definitivamente dal datagrid per non
                ' ricancellarla quando salvo le vaccinazioni delle sedute
                For vacCnt = tb_vac_sed.Rows.Count - 1 To 0 Step -1
                    If tb_vac_sed.Rows(vacCnt).RowState = DataRowState.Deleted AndAlso _
                       tb_vac_sed.Rows(vacCnt)("sed_n_seduta", DataRowVersion.Original) = rowSed("tsd_n_seduta", DataRowVersion.Original) Then
                        tb_vac_sed.Rows(vacCnt).AcceptChanges()
                    End If
                Next
                ' fine SIO 05/09/2003

                DAM.QB.NewQuery()
                DAM.QB.AddTables("t_ana_vaccinazioni_sedute")
                DAM.QB.AddWhereCondition("sed_cic_codice", Comparatori.Uguale, codCiclo, DataTypes.Stringa)
                DAM.QB.AddWhereCondition("sed_n_seduta", Comparatori.Uguale, rowSed("tsd_n_seduta", DataRowVersion.Original), DataTypes.Numero)

                Try
                    DAM.ExecNonQuery(ExecQueryType.Delete)

                Catch exc As Exception
                    DAM.Rollback()
                    OnVacUtility.CloseDam(DAM)

                    If exc.Message.ToString().Contains(Constants.OracleErrors.ORA_02292) Then
                        Dim msgbox As New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Impossibile cancellare la seduta del ciclo essendo già utilizzata all'interno del programma!!", "", False, False)
                        OnitLayout1.ShowMsgBox(msgbox)
                        Exit Sub
                    Else
                        Throw
                    End If

                End Try

                'Serve per leggere la riga eliminata
                DAM.QB.NewQuery()
                DAM.QB.AddTables("t_ana_tempi_sedute")
                DAM.QB.AddWhereCondition("tsd_cic_codice", Comparatori.Uguale, codCiclo, DataTypes.Stringa)
                DAM.QB.AddWhereCondition("tsd_n_seduta", Comparatori.Uguale, rowSed("tsd_n_seduta", DataRowVersion.Original), DataTypes.Numero)

                Try
                    DAM.ExecNonQuery(ExecQueryType.Delete)

                Catch exc As Exception
                    DAM.Rollback()
                    OnVacUtility.CloseDam(DAM)

                    If exc.Message.ToString().Contains(Constants.OracleErrors.ORA_02292) Then
                        Dim msgbox As New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Impossibile cancellare la seduta del ciclo essendo già utilizzata all'interno del programma!!", "", False, False)
                        OnitLayout1.ShowMsgBox(msgbox)
                        Exit Sub
                    Else
                        Throw
                    End If

                End Try

            Next

        End If

        If Not IsNothing(dtSed_Modified) Then
            For Each rowSed In dtSed_Modified.Rows

                'aggiorna la riga sul db
                ' SIO 04/09/2003
                ' Aggiunto if per gestire la foreign key
                If rowSed("tsd_n_seduta", DataRowVersion.Original) = rowSed("tsd_n_seduta") Then
                    DAM.QB.NewQuery()
                    DAM.QB.AddTables("t_ana_tempi_sedute")
                    DAM.QB.AddUpdateField("tsd_intervallo", rowSed("tsd_intervallo"), DataTypes.Numero)
                    DAM.QB.AddUpdateField("tsd_validita", rowSed("tsd_validita") & "", DataTypes.Numero)
                    DAM.QB.AddUpdateField("tsd_durata_seduta", rowSed("tsd_durata_seduta") & "", DataTypes.Numero)
                    DAM.QB.AddUpdateField("tsd_eta_seduta", rowSed("tsd_eta_seduta") & "", DataTypes.Numero)
                    DAM.QB.AddUpdateField("tsd_intervallo_prossima", rowSed("tsd_intervallo_prossima") & "", DataTypes.Numero)
                    DAM.QB.AddUpdateField("tsd_note", rowSed("tsd_note") & "", DataTypes.Stringa)
                    DAM.QB.AddUpdateField("tsd_n_seduta", rowSed("tsd_n_seduta"), DataTypes.Numero)
                    DAM.QB.AddWhereCondition("tsd_cic_codice", Comparatori.Uguale, codCiclo, DataTypes.Stringa)
                    DAM.QB.AddWhereCondition("tsd_n_seduta", Comparatori.Uguale, rowSed("tsd_n_seduta", DataRowVersion.Original), DataTypes.Numero)
                    'brigoz 18-01-05
                    'il campo TSD_NUM_SOLLECITI diventa TSD_NUM_SOLLECITI_RAC [modifica 28/03/2006]
                    DAM.QB.AddUpdateField("tsd_num_solleciti_rac", rowSed("tsd_num_solleciti_rac"), DataTypes.Numero)
                    DAM.QB.AddUpdateField("tsd_num_solleciti", rowSed("tsd_num_solleciti"), DataTypes.Numero)
                    DAM.QB.AddUpdateField("tsd_giorni_posticipo", rowSed("tsd_giorni_posticipo"), DataTypes.Numero)
                    DAM.QB.AddUpdateField("tsd_num_soll_non_obbl", rowSed("tsd_num_soll_non_obbl"), DataTypes.Numero)
                    'aggiunta del campo posticipo seduta [modifica 24/02/2005]
                    DAM.QB.AddUpdateField("tsd_posticipo_seduta", rowSed("tsd_posticipo_seduta"), DataTypes.Stringa)
                    Try
                        DAM.ExecNonQuery(ExecQueryType.Update)
                    Catch exc As Exception
                        DAM.Rollback()
                        OnVacUtility.CloseDam(DAM)
                        Throw
                    End Try
                Else
                    ' Se il numero della seduta è stato modificato, vuol dire che è slittata la seduta
                    ' Per rispettare l'integrità referenziale devo
                    ' 1 - creare la nuova seduta
                    ' 2 - spostare le vaccinazioni che sul DB sono sulla vecchia seduta nella nuova
                    ' 3 - eliminare la vecchia seduta
                    ' Creo la nuova seduta
                    DAM.QB.NewQuery()
                    DAM.QB.AddTables("t_ana_tempi_sedute")
                    DAM.QB.AddInsertField("tsd_cic_codice", codCiclo, DataTypes.Stringa)
                    DAM.QB.AddInsertField("tsd_n_seduta", rowSed("tsd_n_seduta"), DataTypes.Numero)
                    DAM.QB.AddInsertField("tsd_intervallo", rowSed("tsd_intervallo"), DataTypes.Numero)
                    DAM.QB.AddInsertField("tsd_validita", rowSed("tsd_validita") & "", DataTypes.Numero)
                    DAM.QB.AddInsertField("tsd_durata_seduta", rowSed("tsd_durata_seduta") & "", DataTypes.Numero)
                    DAM.QB.AddInsertField("tsd_eta_seduta", rowSed("tsd_eta_seduta") & "", DataTypes.Numero)
                    DAM.QB.AddInsertField("tsd_intervallo_prossima", rowSed("tsd_intervallo_prossima") & "", DataTypes.Numero)
                    DAM.QB.AddInsertField("tsd_note", rowSed("tsd_note") & "", DataTypes.Stringa)
                    'brigoz 18-01-05
                    'il campo TSD_NUM_SOLLECITI diventa TSD_NUM_SOLLECITI_RAC [modifica 28/03/2006]
                    DAM.QB.AddInsertField("tsd_num_solleciti_rac", rowSed("tsd_num_solleciti_rac"), DataTypes.Numero)
                    DAM.QB.AddInsertField("tsd_num_solleciti", rowSed("tsd_num_solleciti"), DataTypes.Numero)
                    DAM.QB.AddInsertField("tsd_giorni_posticipo", rowSed("tsd_giorni_posticipo"), DataTypes.Numero)
                    DAM.QB.AddInsertField("tsd_num_soll_non_obbl", rowSed("tsd_num_soll_non_obbl"), DataTypes.Numero)
                    'aggiunta del campo posticipo seduta [modifica 24/02/2005]
                    DAM.QB.AddInsertField("tsd_posticipo_seduta", rowSed("tsd_posticipo_seduta"), DataTypes.Stringa)
                    Try
                        DAM.ExecNonQuery(ExecQueryType.Insert)
                    Catch exc As Exception
                        DAM.Rollback()
                        OnVacUtility.CloseDam(DAM)
                        Throw
                    End Try

                    ' Sposto le vaccinazioni della vecchia seduta sulla nuova
                    DAM.QB.NewQuery()
                    DAM.QB.AddTables("t_ana_vaccinazioni_sedute")
                    DAM.QB.AddUpdateField("sed_n_seduta", rowSed("tsd_n_seduta"), DataTypes.Numero)
                    DAM.QB.AddWhereCondition("sed_cic_codice", Comparatori.Uguale, codCiclo, DataTypes.Stringa)
                    DAM.QB.AddWhereCondition("sed_n_seduta", Comparatori.Uguale, rowSed("tsd_n_seduta", DataRowVersion.Original), DataTypes.Numero)
                    Try
                        DAM.ExecNonQuery(ExecQueryType.Update)
                    Catch exc As Exception
                        DAM.Rollback()
                        OnVacUtility.CloseDam(DAM)
                        Throw
                    End Try

                    ' Elimino la vecchia seduta
                    DAM.QB.NewQuery()
                    DAM.QB.AddTables("t_ana_tempi_sedute")
                    DAM.QB.AddWhereCondition("tsd_cic_codice", Comparatori.Uguale, codCiclo, DataTypes.Stringa)
                    DAM.QB.AddWhereCondition("tsd_n_seduta", Comparatori.Uguale, rowSed("tsd_n_seduta", DataRowVersion.Original), DataTypes.Numero)

                    Try
                        DAM.ExecNonQuery(ExecQueryType.Delete)
                    Catch exc As Exception
                        DAM.Rollback()
                        OnVacUtility.CloseDam(DAM)
                        Throw
                    End Try
                End If
            Next
        End If

        If Not IsNothing(dtSed_Added) Then
            For Each rowSed In dtSed_Added.Rows
                'aggiungi riga al db
                DAM.QB.NewQuery()
                DAM.QB.AddTables("t_ana_tempi_sedute")
                DAM.QB.AddInsertField("tsd_cic_codice", codCiclo, DataTypes.Stringa)
                DAM.QB.AddInsertField("tsd_n_seduta", rowSed("tsd_n_seduta"), DataTypes.Numero)
                DAM.QB.AddInsertField("tsd_intervallo", rowSed("tsd_intervallo"), DataTypes.Numero)
                DAM.QB.AddInsertField("tsd_validita", rowSed("tsd_validita") & "", DataTypes.Numero)
                DAM.QB.AddInsertField("tsd_durata_seduta", rowSed("tsd_durata_seduta") & "", DataTypes.Numero)
                DAM.QB.AddInsertField("tsd_eta_seduta", rowSed("tsd_eta_seduta") & "", DataTypes.Numero)
                DAM.QB.AddInsertField("tsd_intervallo_prossima", rowSed("tsd_intervallo_prossima") & "", DataTypes.Numero)
                'brigoz 18-01-05
                'il campo TSD_NUM_SOLLECITI diventa TSD_NUM_SOLLECITI_RAC [modifica 28/03/2006]
                DAM.QB.AddInsertField("tsd_num_solleciti_rac", rowSed("tsd_num_solleciti_rac") & "", DataTypes.Numero)
                DAM.QB.AddInsertField("tsd_num_solleciti", rowSed("tsd_num_solleciti") & "", DataTypes.Numero)
                DAM.QB.AddInsertField("tsd_giorni_posticipo", rowSed("tsd_giorni_posticipo") & "", DataTypes.Numero)
                DAM.QB.AddInsertField("tsd_num_soll_non_obbl", rowSed("tsd_num_soll_non_obbl") & "", DataTypes.Numero)
                'aggiunta del campo posticipo seduta [modifica 24/02/2005]
                DAM.QB.AddInsertField("tsd_posticipo_seduta", rowSed("tsd_posticipo_seduta") & "", DataTypes.Stringa)

                DAM.QB.AddInsertField("tsd_note", rowSed("tsd_note") & "", DataTypes.Stringa)
                Try
                    DAM.ExecNonQuery(ExecQueryType.Insert)
                Catch exc As Exception
                    DAM.Rollback()
                    OnVacUtility.CloseDam(DAM)
                    Throw
                End Try

            Next
        End If

        tb_sedute.AcceptChanges()


        '                                                    '
        ' ------------------ ASSOCIAZIONI ------------------ '
        '                                                    '
        Dim dtAss_Deleted As DataTable = tb_ass_sed.GetChanges(DataRowState.Deleted)
        Dim dtAss_Modified As DataTable = tb_ass_sed.GetChanges(DataRowState.Modified)
        Dim dtAss_Added As DataTable = tb_ass_sed.GetChanges(DataRowState.Added)

        Dim dvVaccAss As DataView

        If Not IsNothing(dtAss_Deleted) Then
            For Each rowAss In dtAss_Deleted.Rows
                If Not rowAss("sas_n_seduta", DataRowVersion.Original) Is DBNull.Value Then
                    DAM.QB.NewQuery()
                    DAM.QB.AddTables("t_ana_associazioni_sedute")
                    DAM.QB.AddWhereCondition("sas_cic_codice", Comparatori.Uguale, codCiclo, DataTypes.Stringa)
                    DAM.QB.AddWhereCondition("sas_n_seduta", Comparatori.Uguale, rowAss("sas_n_seduta", DataRowVersion.Original), DataTypes.Numero)
                    DAM.QB.AddWhereCondition("sas_ass_codice", Comparatori.Uguale, rowAss("sas_ass_codice", DataRowVersion.Original), DataTypes.Stringa)

                    Try
                        DAM.ExecNonQuery(ExecQueryType.Delete)

                    Catch exc As Exception
                        DAM.Rollback()
                        OnVacUtility.CloseDam(DAM)

                        If exc.Message.ToString().Contains(Constants.OracleErrors.ORA_02292) Then
                            Dim msgbox As New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Impossibile cancellare la seduta del ciclo essendo è già utilizzata all'interno del programma!!", "", False, False)
                            OnitLayout1.ShowMsgBox(msgbox)
                            Exit For
                        Else
                            Throw
                        End If

                    End Try

                End If
            Next
        End If

        If Not IsNothing(dtAss_Modified) Then
            For Each rowAss In dtAss_Modified.Rows
                ' Devo modificare tutti i record relativi all'associazione (comprese le vaccinazioni)
                dvVaccAss = New DataView(tb_vac_sed)
                dvVaccAss.RowFilter = String.Format("sed_n_seduta='{0}' and sas_ass_codice='{1}'", rowAss("sas_n_seduta", DataRowVersion.Original), rowAss("sas_ass_codice", DataRowVersion.Original))
                '
                Dim i As Integer
                For i = 0 To dvVaccAss.Count - 1
                    DAM.QB.NewQuery()
                    DAM.QB.AddTables("t_ana_associazioni_sedute")
                    DAM.QB.AddUpdateField("sas_ass_codice", rowAss("sas_ass_codice") & "", DataTypes.Stringa)
                    DAM.QB.AddUpdateField("sas_n_richiamo", dvVaccAss(i)("sed_n_richiamo"), DataTypes.Numero)
                    DAM.QB.AddUpdateField("sas_sii_codice", rowAss("sas_sii_codice") & "", DataTypes.Stringa)
                    DAM.QB.AddUpdateField("sas_vii_codice", rowAss("sas_vii_codice") & "", DataTypes.Stringa)
                    '
                    DAM.QB.AddWhereCondition("sas_cic_codice", Comparatori.Uguale, codCiclo, DataTypes.Stringa)
                    DAM.QB.AddWhereCondition("sas_n_seduta", Comparatori.Uguale, rowAss("sas_n_seduta"), DataTypes.Numero)
                    DAM.QB.AddWhereCondition("sas_ass_codice", Comparatori.Uguale, rowAss("sas_ass_codice", DataRowVersion.Original), DataTypes.Stringa)
                    DAM.QB.AddWhereCondition("sas_vac_codice", Comparatori.Uguale, dvVaccAss(i)("sed_vac_codice") & "", DataTypes.Stringa)
                    Try
                        DAM.ExecNonQuery(ExecQueryType.Update)
                    Catch exc As Exception
                        DAM.Rollback()
                        OnVacUtility.CloseDam(DAM)
                        Throw
                    End Try
                Next
            Next
        End If

        If Not IsNothing(dtAss_Added) Then
            For Each rowAss In dtAss_Added.Rows
                ' Devo inserire un record per ogni vaccinazione relativa all'associazione
                dvVaccAss = New DataView(tb_vac_sed)
                dvVaccAss.RowFilter = String.Format("sed_n_seduta='{0}' and sas_ass_codice='{1}'", rowAss("sas_n_seduta"), rowAss("sas_ass_codice"))
                '
                Dim i As Integer
                For i = 0 To dvVaccAss.Count - 1
                    DAM.QB.NewQuery()
                    DAM.QB.AddTables("t_ana_associazioni_sedute")
                    DAM.QB.AddInsertField("sas_cic_codice", codCiclo, DataTypes.Stringa)
                    DAM.QB.AddInsertField("sas_n_seduta", rowAss("sas_n_seduta"), DataTypes.Numero)
                    DAM.QB.AddInsertField("sas_ass_codice", rowAss("sas_ass_codice") & "", DataTypes.Stringa)
                    DAM.QB.AddInsertField("sas_vac_codice", dvVaccAss(i)("sed_vac_codice") & "", DataTypes.Stringa)
                    DAM.QB.AddInsertField("sas_n_richiamo", dvVaccAss(i)("sed_n_richiamo"), DataTypes.Numero)
                    DAM.QB.AddInsertField("sas_sii_codice", rowAss("sas_sii_codice") & "", DataTypes.Stringa)
                    DAM.QB.AddInsertField("sas_vii_codice", rowAss("sas_vii_codice") & "", DataTypes.Stringa)
                    Try
                        DAM.ExecNonQuery(ExecQueryType.Insert)
                    Catch exc As Exception
                        DAM.Rollback()
                        OnVacUtility.CloseDam(DAM)
                        Throw
                    End Try
                Next
            Next
        End If

        tb_ass_sed.AcceptChanges()

        '                                                    '
        ' ------------------ VACCINAZIONI ------------------ '
        '                                                    '
        Dim dtVac_Deleted As DataTable = tb_vac_sed.GetChanges(DataRowState.Deleted)
        Dim dtVac_Modified As DataTable = tb_vac_sed.GetChanges(DataRowState.Modified)
        Dim dtVac_Added As DataTable = tb_vac_sed.GetChanges(DataRowState.Added)

        If Not IsNothing(dtVac_Deleted) Then
            For Each rowVac In dtVac_Deleted.Rows
                If rowVac("sas_ass_codice", DataRowVersion.Original) Is DBNull.Value Then
                    If Not rowVac("sed_n_seduta", DataRowVersion.Original) Is DBNull.Value Then
                        DAM.QB.NewQuery()
                        DAM.QB.AddTables("t_ana_vaccinazioni_sedute")
                        DAM.QB.AddWhereCondition("sed_cic_codice", Comparatori.Uguale, codCiclo, DataTypes.Stringa)
                        DAM.QB.AddWhereCondition("sed_n_seduta", Comparatori.Uguale, rowVac("sed_n_seduta", DataRowVersion.Original), DataTypes.Numero)
                        DAM.QB.AddWhereCondition("sed_vac_codice", Comparatori.Uguale, rowVac("sed_vac_codice", DataRowVersion.Original), DataTypes.Stringa)

                        Try
                            DAM.ExecNonQuery(ExecQueryType.Delete)

                        Catch exc As Exception
                            DAM.Rollback()
                            OnVacUtility.CloseDam(DAM)

                            If exc.Message.ToString().Contains(Constants.OracleErrors.ORA_02292) Then
                                Dim msgbox As New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("Impossibile cancellare la seduta del ciclo essendo è già utilizzata all'interno del programma!!", "", False, False)
                                OnitLayout1.ShowMsgBox(msgbox)
                                Exit For
                            Else
                                Throw
                            End If

                        End Try

                    End If
                End If
            Next
        End If

        If Not IsNothing(dtVac_Modified) Then
            For Each rowVac In dtVac_Modified.Rows
                If rowVac("sas_ass_codice") Is DBNull.Value Then
                    DAM.QB.NewQuery()
                    DAM.QB.AddTables("t_ana_vaccinazioni_sedute")
                    DAM.QB.AddUpdateField("sed_vac_codice", rowVac("sed_vac_codice") & "", DataTypes.Stringa)
                    DAM.QB.AddUpdateField("sed_n_richiamo", rowVac("sed_n_richiamo"), DataTypes.Numero)
                    DAM.QB.AddUpdateField("sed_sii_codice", rowVac("sed_sii_codice") & "", DataTypes.Stringa)
                    DAM.QB.AddWhereCondition("sed_cic_codice", Comparatori.Uguale, codCiclo, DataTypes.Stringa)
                    DAM.QB.AddWhereCondition("sed_n_seduta", Comparatori.Uguale, rowVac("sed_n_seduta"), DataTypes.Numero)
                    DAM.QB.AddWhereCondition("sed_vac_codice", Comparatori.Uguale, rowVac("sed_vac_codice", DataRowVersion.Original), DataTypes.Stringa)
                    Try
                        DAM.ExecNonQuery(ExecQueryType.Update)
                    Catch exc As Exception
                        DAM.Rollback()
                        OnVacUtility.CloseDam(DAM)
                        Throw
                    End Try
                End If
            Next
        End If

        If Not IsNothing(dtVac_Added) Then
            For Each rowVac In dtVac_Added.Rows
                If rowVac("sas_ass_codice") Is DBNull.Value Then
                    DAM.QB.NewQuery()
                    DAM.QB.AddTables("t_ana_vaccinazioni_sedute")
                    DAM.QB.AddInsertField("sed_cic_codice", codCiclo, DataTypes.Stringa)
                    DAM.QB.AddInsertField("sed_n_seduta", rowVac("sed_n_seduta"), DataTypes.Numero)
                    DAM.QB.AddInsertField("sed_vac_codice", rowVac("sed_vac_codice") & "", DataTypes.Stringa)
                    DAM.QB.AddInsertField("sed_n_richiamo", rowVac("sed_n_richiamo"), DataTypes.Numero)
                    DAM.QB.AddInsertField("sed_sii_codice", rowVac("sed_sii_codice") & "", DataTypes.Stringa)
                    Try
                        DAM.ExecNonQuery(ExecQueryType.Insert)
                    Catch exc As Exception
                        DAM.Rollback()
                        OnVacUtility.CloseDam(DAM)
                        Throw
                    End Try
                End If
            Next
        End If

        tb_vac_sed.AcceptChanges()

        OnitLayout1.Busy = False
        nMod = 0
        DAM.Commit()
        OnVacUtility.CloseDam(DAM)
    End Sub

    Private Sub ControllaVaccinazioniRimaste()
        Dim vacRimaste As Boolean = False
        Dim rowVacSed As DataRow
        For Each rowVacSed In tb_vac_sed.Rows
            If rowVacSed.RowState <> DataRowState.Deleted Then vacRimaste = True
        Next

        Dim indice As Integer = dg_sedute.SelectedIndex
        Dim seduta As Integer = CType(dg_sedute.Items(indice).FindControl("lb_index"), Label).Text

        If Not vacRimaste Then
            'se non rimangono vaccinazioni per la determinata seduta, non deve proporre i parametri

            tb_sedute.Rows.Find(seduta)("tsd_num_solleciti_rac") = DBNull.Value
            tb_sedute.Rows.Find(seduta)("tsd_num_solleciti") = DBNull.Value
            tb_sedute.Rows.Find(seduta)("tsd_giorni_posticipo") = 0
            tb_sedute.Rows.Find(seduta)("tsd_num_soll_non_obbl") = 0
            tb_sedute.Rows.Find(seduta)("tsd_posticipo_seduta") = "N"

        End If

        If ControllaSedutaVacObbligatorie(indice) Then
            'se nella seduta sono presenti delle obbligatorie deve cancellare i parametri che non ne fanno parte
            tb_sedute.Rows.Find(seduta)("tsd_giorni_posticipo") = 0
            tb_sedute.Rows.Find(seduta)("tsd_num_soll_non_obbl") = 0
            tb_sedute.Rows.Find(seduta)("tsd_posticipo_seduta") = "N"
        Else
            'se nella seduta sono presenti delle non obbligatorie deve cancellare i parametri che non ne fanno parte
            tb_sedute.Rows.Find(seduta)("tsd_num_solleciti_rac") = DBNull.Value
            tb_sedute.Rows.Find(seduta)("tsd_num_solleciti") = DBNull.Value
        End If

        dg_sedute.DataSource = tb_sedute
        dg_sedute.DataBind()

    End Sub

    'carica i parametri relativi ad una determinata seduta [modifica 28/03/2006]
    Private Sub CaricaParametriSeduta(indice As Integer)

        'valorizzazione dei parametri
        txtNumSolRac.Text = CType(dg_sedute.Items(indice).FindControl("txtNumSollecitiRac"), TextBox).Text
        ddlPostSed.SelectedValue = CType(dg_sedute.Items(indice).FindControl("ddlPosticipoSeduta"), DropDownList).SelectedValue
        txtNumSol.Text = CType(dg_sedute.Items(indice).FindControl("txtNumSolleciti"), TextBox).Text
        If Settings.NUMSOL = -1 Then
            txtNumSolGlobal.Text = String.Empty
        Else
            txtNumSolGlobal.Text = Settings.NUMSOL
        End If
        txtGiorniPost.Text = IIf(CType(dg_sedute.Items(indice).FindControl("txtGiorniPosticipo"), TextBox).Text <> "", CType(dg_sedute.Items(indice).FindControl("txtGiorniPosticipo"), TextBox).Text, 0)
        txtNumSol.ReadOnly = (Trim(CType(dg_sedute.Items(indice).FindControl("txtNumSollecitiRac"), TextBox).Text) <> "")
        txtNumSol.CssClass = IIf(Trim(CType(dg_sedute.Items(indice).FindControl("txtNumSollecitiRac"), TextBox).Text) <> "", "textbox_numerico_disabilitato", "textbox_numerico_obbligatorio")
        txtNumSolNonObbl.Text = IIf(CType(dg_sedute.Items(indice).FindControl("txtSollecitiNonObbligatori"), TextBox).Text <> "", CType(dg_sedute.Items(indice).FindControl("txtSollecitiNonObbligatori"), TextBox).Text, 0)

        'visualizzazione dei parametri
        'div1 --> se seduta con vaccinazioni obbligatorie
        'div2 --> se seduta con tutte vaccinazioni non obbligatorie
        Dim strJs As String = ""
        strJs &= "if (document.getElementById('parametriVacObbligatorie') != null)" & Chr(13) & Chr(10)
        strJs &= "document.getElementById('parametriVacObbligatorie').style.display = '" & IIf(ControllaSedutaVacObbligatorie(indice), "inline", "none") & "';" & Chr(13) & Chr(10)
        strJs &= "if (document.getElementById('parametriVacNonObbligatorie') != null)" & Chr(13) & Chr(10)
        strJs &= "document.getElementById('parametriVacNonObbligatorie').style.display = '" & IIf(Not ControllaSedutaVacObbligatorie(indice), "inline", "none") & "';" & Chr(13) & Chr(10)
        OnitLayout1.InsertRoutineJS(strJs)
        'ridimensionamento della modale
        fmVisualizzaParametri.Width = IIf(ControllaSedutaVacObbligatorie(indice), New Unit(450), New Unit(500))
        fmVisualizzaParametri.ToolTip = indice

    End Sub

    Private Function CaricaVacc_Associazione(cod_new_ass As String) As DataTable

        Dim dt As New DataTable()

        Dim dam As IDAM = OnVacUtility.OpenDam()
        With dam.QB
            .NewQuery()
            .AddSelectFields("vac_codice, vac_descrizione, vac_obbligatoria")
            .AddTables("t_ana_link_ass_vaccinazioni, t_ana_vaccinazioni")
            .AddWhereCondition("val_ass_codice", Comparatori.Uguale, cod_new_ass, DataTypes.Stringa)
            .AddWhereCondition("val_vac_codice", Comparatori.Uguale, "vac_codice", DataTypes.Join)
        End With
        '
        Try
            dam.BuildDataTable(dt)
        Catch exc As Exception
            exc.InternalPreserveStackTrace()
            Throw
        Finally
            OnVacUtility.CloseDam(dam)
        End Try

        Return dt

    End Function

#End Region

End Class
