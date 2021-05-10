Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.Controls


Partial Class OnVac_Orari
    Inherits OnVac.Common.PageBase

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

#Region " Properties "

    Property nMod() As Int16
        Get
            Return Session("OnVac_nMod")
        End Get
        Set(ByVal Value As Int16)
            Session("OnVac_nMod") = Value
        End Set
    End Property

    Property codAmbulatorio() As Integer
        Get
            Return Session("OnVac_codAmbulatorio")
        End Get
        Set(ByVal Value As Integer)
            Session("OnVac_codAmbulatorio") = Value
        End Set
    End Property

    Property orari() As DataTable
        Get
            Return Session("OnVac_orari")
        End Get
        Set(ByVal Value As DataTable)
            Session("OnVac_orari") = Value
        End Set
    End Property

    Property orariApp() As DataTable
        Get
            Return Session("OnVac_orari_app")
        End Get
        Set(ByVal Value As DataTable)
            Session("OnVac_orari_app") = Value
        End Set
    End Property

    ReadOnly Property orari_view() As DataView
        Get
            orari.DefaultView.Sort = "org_giorno"
            Return orari.DefaultView
            'Return Session("OnVac_orari_view")
        End Get
        'Set(ByVal Value As DataView)
        '    Session("OnVac_orari_view") = Value
        'End Set
    End Property

    ReadOnly Property orariApp_view() As DataView
        Get
            orariApp.DefaultView.Sort = "ora_giorno"
            Return orariApp.DefaultView
            'Return Session("OnVac_orari_app_view")
        End Get
        'Set(ByVal Value As DataView)
        '    Session("OnVac_orari_app_view") = Value
        'End Set
    End Property

    Private Property descAmbulatorio() As String
        Get
            Return Session("descAmbulatorio")
        End Get
        Set(ByVal Value As String)
            Session("descAmbulatorio") = Value
        End Set
    End Property

#End Region

    Public strJS As String

#Region " Enum "

    Private Enum TipoOrari
        Giornalieri
        Appuntamenti
    End Enum

#End Region

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Not IsPostBack() Then

            codAmbulatorio = CInt(Request.QueryString("codice").Split("|")(0))

            Using DAM As IDAM = OnVacUtility.OpenDam()

                With DAM.QB
                    .NewQuery()
                    .AddTables("t_ana_ambulatori")
                    .AddSelectFields("amb_descrizione")
                    .AddWhereCondition("amb_codice", Comparatori.Uguale, codAmbulatorio, DataTypes.Numero)
                End With

                descAmbulatorio = DAM.ExecScalar()

            End Using

            LayoutTitolo.Text = descAmbulatorio

            RiempiOrari(TipoOrari.Giornalieri)
            ReadOnlyTB(TipoOrari.Giornalieri)

            RiempiOrari(TipoOrari.Appuntamenti)
            ReadOnlyTB(TipoOrari.Appuntamenti)

            ShowPrintButtons()

        End If

    End Sub

    Private Sub ReadOnlyTB(tipoOrari As TipoOrari)

        Dim pref As String = String.Empty
        Select Case tipoOrari
            Case OnVac_Orari.TipoOrari.Giornalieri
                pref = ""
            Case OnVac_Orari.TipoOrari.Appuntamenti
                pref = "app"
        End Select

        Dim stbScript As New System.Text.StringBuilder()

        For i As Integer = 0 To 6
            For j As Integer = 0 To 3
                stbScript.AppendFormat("var item = document.getElementById('or{0}_{1}{2}');{3}", i.ToString(), j.ToString(), pref, vbNewLine)
                stbScript.AppendFormat("item.style.border='';{0}", vbCrLf)
                stbScript.AppendFormat("item.style.background='Transparent';{0}", vbCrLf)
                stbScript.AppendFormat("item.readOnly='true';{0}", vbCrLf)
            Next
        Next
        strJS += stbScript.ToString()

    End Sub

    Private Sub RiempiOrari(tipoOrari As TipoOrari)

        Dim giorno As Int16
        Dim row As DataRow
        Dim tabella As String = String.Empty
        Dim pref As String = String.Empty

        'impostazione dei dati da elaborare a seconda del parametro passato (modifica 19/07/2004)
        Select Case tipoOrari

            Case OnVac_Orari.TipoOrari.Giornalieri
                If Not orari Is Nothing Then
                    orari.Dispose()
                End If
                orari = New DataTable()
                tabella = "t_ana_orari_giornalieri"
                pref = "org"

            Case OnVac_Orari.TipoOrari.Appuntamenti
                If Not orariApp Is Nothing Then
                    orariApp.Dispose()
                End If
                orariApp = New DataTable()
                tabella = "t_ana_orari_appuntamenti"
                pref = "ora"

        End Select

        Using DAM As IDAM = OnVacUtility.OpenDam()

            DAM.QB.NewQuery()
            DAM.QB.AddTables(tabella)
            DAM.QB.AddSelectFields(pref & "_giorno", pref & "_am_inizio as inizio_mat", pref & "_am_fine as fine_mat", pref & "_pm_inizio as inizio_pom", pref & "_pm_fine as fine_pom")
            DAM.QB.AddWhereCondition(pref & "_amb_codice", Comparatori.Uguale, codAmbulatorio, DataTypes.Numero)

            Select Case tipoOrari
                Case OnVac_Orari.TipoOrari.Giornalieri
                    DAM.BuildDataTable(orari)
                Case OnVac_Orari.TipoOrari.Appuntamenti
                    DAM.BuildDataTable(orariApp)
            End Select

        End Using

        Select Case tipoOrari

            Case OnVac_Orari.TipoOrari.Giornalieri
                If orari.Rows.Count = 0 Then
                    For giorno = 0 To 6
                        row = orari.NewRow
                        row("org_giorno") = giorno
                        orari.Rows.Add(row)
                    Next
                End If
                FillHTMLTable(tipoOrari)

            Case OnVac_Orari.TipoOrari.Appuntamenti
                If orariApp.Rows.Count = 0 Then
                    For giorno = 0 To 6
                        row = orariApp.NewRow
                        row("ora_giorno") = giorno
                        orariApp.Rows.Add(row)
                    Next
                End If
                FillHTMLTable(tipoOrari)

        End Select

    End Sub

    'devono essere gestite entrambe le tabelle orari (modifica 19/07/2004)
    Private Sub FillHTMLTable(tipoOrari As TipoOrari)

        Select Case tipoOrari

            Case OnVac_Orari.TipoOrari.Giornalieri

                For i As Integer = 0 To 6
                    For j As Integer = 1 To 4
                        If (Not (orari_view.Count = 0 OrElse orari_view.Item(i)(j) Is DBNull.Value)) Then
                            CType(Page.FindControl("or" & i & "_" & (j - 1)), TextBox).Text = CType(orari_view.Item(i)(j), DateTime).ToShortTimeString
                        Else
                            CType(Page.FindControl("or" & i & "_" & (j - 1)), TextBox).Text = ""
                        End If
                    Next
                Next

            Case OnVac_Orari.TipoOrari.Appuntamenti

                For i As Integer = 0 To 6
                    For j As Integer = 1 To 4
                        If (Not (orariApp_view.Count = 0 OrElse orariApp_view.Item(i)(j) Is DBNull.Value)) Then
                            CType(Page.FindControl("or" & i & "_" & (j - 1) & "app"), TextBox).Text = CType(orariApp_view.Item(i)(j), DateTime).ToShortTimeString
                        Else
                            CType(Page.FindControl("or" & i & "_" & (j - 1) & "app"), TextBox).Text = ""
                        End If
                    Next
                Next

        End Select

    End Sub

    'anche la modifica del DT deve essere effettuata per entrambi gli orari (modifica 20/07/2004)
    Private Function AggiornaDT(tipoOrari As TipoOrari) As Boolean

        Try
            Dim dateOrario As DateTime

            Select Case tipoOrari

                Case OnVac_Orari.TipoOrari.Giornalieri

                    For i As Integer = 0 To 6

                        For j As Integer = 1 To 4

                            dateOrario = SetOrario(i, j, tipoOrari)

                            If dateOrario = DateTime.MinValue Then
                                orari_view.Item(i)(j) = DBNull.Value
                            Else
                                orari_view.Item(i)(j) = dateOrario
                            End If

                        Next

                    Next

                Case OnVac_Orari.TipoOrari.Appuntamenti

                    For i As Integer = 0 To 6

                        For j As Integer = 1 To 4

                            dateOrario = SetOrario(i, j, tipoOrari)

                            If dateOrario = DateTime.MinValue Then
                                orariApp_view.Item(i)(j) = DBNull.Value
                            Else
                                orariApp_view.Item(i)(j) = dateOrario
                            End If

                        Next

                    Next

            End Select

        Catch ex As Exception
            Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("ATTENZIONE: gli orari inseriti non sono tutti validi. Impossibile effettuare il salvataggio.", "erroreOrario", False, False))
            Return False

        End Try

        Return True

    End Function

    ' Restituisce l'orario impostato dall'utente oppure DbNull se vuoto. 
    ' In caso di errore nel formato dell'orario inserito, 
    Private Function SetOrario(i As Integer, j As Integer, tipoOrari As TipoOrari) As DateTime

        Dim tipo As String = String.Empty
        If tipoOrari = OnVac_Orari.TipoOrari.Appuntamenti Then
            tipo = "app"
        End If

        Dim control As Object = Page.FindControl(String.Format("or{0}_{1}{2}", i.ToString(), (j - 1).ToString(), tipo))
        If (control Is Nothing) Then
            Return DateTime.MinValue
        End If

        Dim txtOrario As TextBox = DirectCast(control, TextBox)
        If (String.IsNullOrEmpty(txtOrario.Text)) Then
            Return DateTime.MinValue
        End If

        Return Convert.ToDateTime(txtOrario.Text)

    End Function

    ' Restituisce false se almeno un orario impostato non è nel formato corretto.
    ' Altrimenti restituisce true.
    Private Function CheckOrari() As Boolean

        Try
            Dim dateOrario As DateTime

            For i As Integer = 0 To 6
                For j As Integer = 1 To 4
                    dateOrario = SetOrario(i, j, TipoOrari.Giornalieri)
                Next
            Next

            For i As Integer = 0 To 6
                For j As Integer = 1 To 4
                    dateOrario = SetOrario(i, j, TipoOrari.Appuntamenti)
                Next
            Next

        Catch ex As Exception
            Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox("ATTENZIONE: gli orari inseriti non sono tutti validi. Salvataggio non effettuato.", "erroreOrario", False, False))
            Return False

        End Try

        Return True

    End Function

    Private Sub Salva()

        If checkLock() Then
            Exit Sub
        End If

        If Not CheckOrari() Then
            Exit Sub
        End If

        Dim DAM As IDAM = OnVacUtility.OpenDam()

        Try
            DAM.BeginTrans()

            'il salvataggio deve tenere conto di entrambi gli orari (modifica 20/07/2004)
            Dim des() As String = {"", "app"}
            Dim pref() As String = {"org", "ora"}
            Dim tab() As String = {"giornalieri", "appuntamenti"}
            Dim count As Integer

            Dim oraTemp As String

            For count = 0 To des.Length - 1

                oraTemp = String.Empty

                DAM.QB.NewQuery()
                DAM.QB.AddTables("t_ana_orari_" & tab(count))
                DAM.QB.AddWhereCondition(pref(count) & "_amb_codice", Comparatori.Uguale, codAmbulatorio, DataTypes.Numero)

                DAM.ExecNonQuery(ExecQueryType.Delete)

                For i As Integer = 0 To 6
                    DAM.QB.NewQuery()
                    DAM.QB.AddTables("t_ana_orari_" & tab(count))
                    DAM.QB.AddInsertField(pref(count) & "_cns_codice", "NULL", DataTypes.Stringa)
                    DAM.QB.AddInsertField(pref(count) & "_amb_codice", codAmbulatorio, DataTypes.Numero)
                    DAM.QB.AddInsertField(pref(count) & "_giorno", i, DataTypes.Stringa)

                    oraTemp = CType(Page.FindControl("or" & i & "_0" & des(count)), TextBox).Text
                    If (oraTemp = "") Then
                        DAM.QB.AddInsertField(pref(count) & "_am_inizio", "", DataTypes.DataOra)
                    Else
                        DAM.QB.AddInsertField(pref(count) & "_am_inizio", "1/1/1900 " & oraTemp, DataTypes.DataOra)
                    End If
                    oraTemp = CType(Page.FindControl("or" & i & "_1" & des(count)), TextBox).Text
                    If (oraTemp = "") Then
                        DAM.QB.AddInsertField(pref(count) & "_am_fine", "", DataTypes.DataOra)
                    Else
                        DAM.QB.AddInsertField(pref(count) & "_am_fine", "1/1/1900 " & oraTemp, DataTypes.DataOra)
                    End If
                    oraTemp = CType(Page.FindControl("or" & i & "_2" & des(count)), TextBox).Text
                    If (oraTemp = "") Then
                        DAM.QB.AddInsertField(pref(count) & "_pm_inizio", "", DataTypes.DataOra)
                    Else
                        DAM.QB.AddInsertField(pref(count) & "_pm_inizio", "1/1/1900 " & oraTemp, DataTypes.DataOra)
                    End If
                    oraTemp = CType(Page.FindControl("or" & i & "_3" & des(count)), TextBox).Text
                    If (oraTemp = "") Then
                        DAM.QB.AddInsertField(pref(count) & "_pm_fine", "", DataTypes.DataOra)
                    Else
                        DAM.QB.AddInsertField(pref(count) & "_pm_fine", "1/1/1900 " & oraTemp, DataTypes.DataOra)
                    End If

                    DAM.ExecNonQuery(ExecQueryType.Insert)

                Next

                orari.AcceptChanges()
                orariApp.AcceptChanges()

            Next

            DAM.Commit()

        Catch ex As Exception

            DAM.Rollback()
            ex.InternalPreserveStackTrace()
            Throw

        Finally

            OnVacUtility.CloseDam(DAM)

        End Try

    End Sub

    Private Sub addMainBut()

        Me.ToolBar.Items.Clear()

        Dim item_salva As New Infragistics.WebUI.UltraWebToolbar.TBarButton()
        item_salva.Text = "Salva"
        item_salva.Image = OnVacUtility.GetIconUrl("salva.gif")
        item_salva.Key = "btn_Salva"
        Me.ToolBar.Items.Add(item_salva)

        Dim item_ann As New Infragistics.WebUI.UltraWebToolbar.TBarButton()
        item_ann.Text = "Annulla"
        item_ann.Image = OnVacUtility.GetIconUrl("annulla.gif")
        item_ann.Key = "btn_Annulla"
        Me.ToolBar.Items.Add(item_ann)

        Dim item_mod As New Infragistics.WebUI.UltraWebToolbar.TBarButton()
        item_mod.Text = "Modifica"
        item_mod.Image = OnVacUtility.GetIconUrl("modifica.gif")
        item_mod.Key = "btn_Modifica"
        Me.ToolBar.Items.Add(item_mod)

        Me.ToolBar.Items.Add(New Infragistics.WebUI.UltraWebToolbar.TBSeparator())

        Dim item_sta As New Infragistics.WebUI.UltraWebToolbar.TBarButton()
        item_sta.Text = "Stampa"
        item_sta.Image = OnVacUtility.GetIconUrl("stampa.gif")
        item_sta.Key = "btn_Stampa"
        Me.ToolBar.Items.Add(item_sta)

        ShowPrintButtons()

    End Sub

    Private Sub ShowPrintButtons()

        Dim listPrintButtons As New List(Of Common.PageBase.PrintButton)()

        listPrintButtons.Add(New PrintButton(Constants.ReportName.OrariAmbulatorio, "btn_Stampa"))

        Me.ShowToolbarPrintButtons(listPrintButtons, ToolBar)

    End Sub

    Private Sub OnitLayout31_BusyChanged(Sender As System.Object, E As System.EventArgs, State As Boolean) Handles OnitLayout31.BusyChanged
        If State Then
            Me.OnitLayout31.lock.Lock(0, codAmbulatorio.ToString())
        Else
            Me.OnitLayout31.lock.EndLock(0)
        End If
    End Sub

    Private Function checkLock() As Boolean

        lb_warning.Visible = False

        If (OnitLayout31.lock.IsLocked(0) AndAlso codAmbulatorio = OnitLayout31.lock.GetLockInfo(0).Info) Then
            If Not (lb_warning.Visible) Then
                lb_warning.Visible = True
                lb_warning.Text = "ATTENZIONE: L'APPLICAZIONE E' BLOCCATA DA " & OnitLayout31.lock.GetLockInfo(0).UserCodice & ".<BR>RIPROVARE FRA QUALCHE MINUTO."
            End If
            Return True
        End If

        Return False

    End Function

    Private Sub OnitLayout1_TimeoutLock(Sender As System.Object, E As System.EventArgs, int As Short)
        lb_warning.Visible = True
        lb_warning.Text = "ATTENZIONE: IL BLOCCO SULL'APPLICAZIONE E' SCADUTO. CHIUDERE<BR> LA PAGINA SENZA SALVARE E RIPROVARE FRA QUALCHE MINUTO."
    End Sub

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btn_Salva"
                Salva()

                OnitLayout31.Busy = False

                nMod = 0

                ReadOnlyTB(TipoOrari.Giornalieri)
                ReadOnlyTB(TipoOrari.Appuntamenti)

            Case "btn_Annulla"
                If checkLock() Then Exit Sub

                orari.RejectChanges()

                RiempiOrari(TipoOrari.Giornalieri)
                RiempiOrari(TipoOrari.Appuntamenti)

                OnitLayout31.Busy = False

                nMod = 0

                ReadOnlyTB(TipoOrari.Giornalieri)
                ReadOnlyTB(TipoOrari.Appuntamenti)

            Case "btn_Modifica"
                If checkLock() Then Exit Sub

                ToolBar.Items.Clear()

                Dim item_conf As New Infragistics.WebUI.UltraWebToolbar.TBarButton
                item_conf.Text = "Conferma"
                item_conf.Image = OnVacUtility.GetIconUrl("conferma.gif")
                item_conf.Key = "btn_Conferma"
                ToolBar.Items.Add(item_conf)

                Dim item_ind As New Infragistics.WebUI.UltraWebToolbar.TBarButton
                item_ind.Text = "Annulla"
                item_ind.Image = OnVacUtility.GetIconUrl("delete.gif")
                item_ind.Key = "btn_Indietro"
                ToolBar.Items.Add(item_ind)

                OnitLayout31.Busy = True

                nMod += 1
                strJS &= "document.getElementById(""or1_0"").focus()" & vbCrLf
                strJS &= "function checkMod(){" & vbCrLf

                'deve produrre il codice JS per entrambe le tabelle (modifica 19/07/2004)
                Dim i, j, countDes As Int16
                Dim des() As String = {"", "app"}
                For countDes = 0 To des.Length - 1
                    For i = 0 To 6
                        For j = 0 To 3
                            strJS &= "if (document.getElementById(""or" & i & "_" & j & des(countDes) & """).value!=""" & CType(Page.FindControl("or" & i & "_" & j & des(countDes)), TextBox).Text & """){" & vbCrLf
                            strJS &= "return true" & vbCrLf
                            strJS &= "}" & vbCrLf
                        Next
                    Next
                Next
                strJS &= "return false}" & vbCrLf

            Case "btn_Indietro"
                If checkLock() Then Exit Sub

                ReadOnlyTB(TipoOrari.Giornalieri)
                ReadOnlyTB(TipoOrari.Appuntamenti)

                addMainBut()

                FillHTMLTable(TipoOrari.Giornalieri)
                FillHTMLTable(TipoOrari.Appuntamenti)

                nMod -= 1
                If (nMod = 0) Then OnitLayout31.Busy = False

            Case "btn_Conferma"
                If checkLock() Then Exit Sub

                Dim ok As Boolean = AggiornaDT(TipoOrari.Giornalieri)

                If ok Then
                    ok = AggiornaDT(TipoOrari.Appuntamenti)
                End If

                If ok Then
                    ReadOnlyTB(TipoOrari.Giornalieri)
                    ReadOnlyTB(TipoOrari.Appuntamenti)

                    addMainBut()
                End If

            Case "btn_Stampa"
                Stampa()

        End Select

    End Sub

    Private Sub Stampa()

        Dim rpt As New ReportParameter
        Dim strFiltro As String = "{T_ANA_AMBULATORI.AMB_CODICE} = " & codAmbulatorio

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizReport As New Biz.BizReport(genericProvider, Settings, OnVacContext.CreateBizContextInfos())

                If Not OnVacReport.StampaReport(Page.Request.Path & "?CODICE=" & codAmbulatorio, Constants.ReportName.OrariAmbulatorio, strFiltro, rpt, , , bizReport.GetReportFolder(Constants.ReportName.OrariAmbulatorio)) Then
                    OnVacUtility.StampaNonPresente(Page, Constants.ReportName.OrariAmbulatorio)
                End If

            End Using
        End Using

    End Sub

End Class
