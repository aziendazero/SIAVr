Imports Onit.Database.DataAccessManager


Partial Class TopFrame
    Inherits Page

#Region " Fields "

    Protected WithEvents lnk_Lock As LinkButton

#End Region

#Region " Public "

    Public Shared htmlRendered As String
    Public Shared JsRendered As String

#End Region

#Region " Properties "

    Private ReadOnly Property LeftVirtualPath() As String
        Get
            Dim leftPath As String = Request.Url.AbsolutePath
            leftPath = leftPath.Substring(0, leftPath.LastIndexOf("/") + 1)
            leftPath += "LeftFrame.aspx"
            Return leftPath
        End Get
    End Property

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

#Region " Eventi pagina "

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If Not IsPostBack() Then

            JsRendered = String.Empty

            Dim ds As System.Data.DataSet = LoadPersonalAppMenu()
            RenderMenu(ds)

            lblNomeApp.Text = String.Empty

            Dim nomeServer As String = ConfigurationManager.AppSettings.Get("NomeServer")
            If nomeServer Is Nothing Then nomeServer = String.Empty

            lblUtente.Text = String.Format("{0} ({1})", OnVacContext.UserDescription, nomeServer)

            ' **** integrazione con "SharePoint" (zorz 6/7/06)  ****
            imgLockUser.Visible = Not OnVacUtility.OnSharePortalIntegrated

        End If

    End Sub

#End Region

#Region " Caricamento "

    ' restituisce una tabella contenente tutte le voci e i menu del'applicazione corrente
    Private Function LoadPersonalAppMenu() As System.Data.DataSet

        Dim strAppCorrente As String = OnVacContext.MenAppId
        Dim idUtente As String = OnVacContext.UserId

        Dim ds As New System.Data.DataSet
        Dim dtab As New DataTable("DtMenu")

        Using dam As IDAM = OnVacUtility.OpenDamToOnitManager()

            ' Query per la ricerca dei menù nel DB
            dam.QB.NewQuery()
            dam.QB.AddTables("t_ana_menu, t_ana_link_utenti_menu")
            dam.QB.AddSelectFields("men_class_name, men_app_id, men_url, men_nome, men_posizione, men_img, men_weight, men_disabilitato")
            dam.QB.AddWhereCondition("lum_ute_id", Comparatori.Uguale, idUtente, DataTypes.Numero)
            dam.QB.AddWhereCondition("lum_men_app_id", Comparatori.Uguale, strAppCorrente, DataTypes.Stringa)
            dam.QB.AddWhereCondition("men_app_id", Comparatori.Uguale, "lum_men_app_id", DataTypes.Join)
            dam.QB.AddWhereCondition("men_weight", Comparatori.Uguale, "lum_men_weight", DataTypes.Join)
            dam.QB.AddWhereCondition("men_azi_codice", Comparatori.Uguale, "lum_azi_codice", DataTypes.Join)
            dam.QB.AddWhereCondition("men_azi_codice", Comparatori.Uguale, OnVacContext.Azienda, DataTypes.Stringa)

            dam.QB.AddUnion(dam.QB.GetSelect())

            dam.QB.NewQuery(False)
            dam.QB.AddTables("t_ana_link_gruppi_menu, t_ana_menu")
            dam.QB.AddSelectFields("men_class_name, men_app_id, men_url, men_nome, men_posizione, men_img, men_weight, men_disabilitato")
            dam.QB.AddWhereCondition("lgm_men_app_id", Comparatori.Uguale, strAppCorrente, DataTypes.Stringa)
            dam.QB.AddWhereCondition("lgm_men_weight", Comparatori.Uguale, "men_weight", DataTypes.Join)
            dam.QB.AddWhereCondition("lgm_men_app_id", Comparatori.Uguale, "men_app_id", DataTypes.Join)
            dam.QB.AddWhereCondition("lgm_azi_codice", Comparatori.Uguale, "men_azi_codice", DataTypes.Join)
            dam.QB.AddWhereCondition("men_azi_codice", Comparatori.Uguale, OnVacContext.Azienda, DataTypes.Stringa)

            ' **** integrazione con "SharePoint" (zorz 6/7/06)  ****
            If Not OnVacUtility.OnSharePortalIntegrated Then
                dam.QB.AddTables("t_ana_link_gruppi_utenti")
                dam.QB.AddWhereCondition("lgu_ute_id", Comparatori.Uguale, idUtente, DataTypes.Numero)
                dam.QB.AddWhereCondition("lgu_gru_id", Comparatori.Uguale, "lgm_gru_id", DataTypes.Join)
            Else
                ''--
                'Dim membershipProvider As New OnSharePortalMembershipService.MembershipService
                'Dim userGroupMemberships As OnSharePortalMembershipService.Membership()
                'Dim sharePointCurrentUserGroupsId As New List(Of String)
                'Dim onSharePortalWebServicesUserInfo As portale.Shared.OnSharePortalWebServicesUserInfo = portale.Shared.GlobalUtility.OnSharePortalWebServicesUserInfo
                ''--
                'membershipProvider.Credentials = New System.Net.NetworkCredential(onSharePortalWebServicesUserInfo.UserName, onSharePortalWebServicesUserInfo.Password, onSharePortalWebServicesUserInfo.Domain)
                ''--
                'userGroupMemberships = membershipProvider.GetUserGroupMemberships()
                ''--
                'For Each userGroupMembership As OnSharePortalMembershipService.Membership In userGroupMemberships
                '    If userGroupMembership.IsDomainGroup AndAlso context.User.IsInRole(userGroupMembership.LoginName) Then sharePointCurrentUserGroupsId.Add(userGroupMembership.Id.ToString)
                'Next
                ''--
                'dam.QB.AddWhereCondition("lgm_gru_id", Comparatori.In, String.Join(",", sharePointCurrentUserGroupsId.ToArray()), DataTypes.Replace)
                ''--
            End If
            ' **** ********************************************* ****

            dam.QB.AddOrderByFields("men_posizione")

            dam.QB.AddUnion(dam.QB.GetSelect())

            ' Pulitura e creazione dataTable
            dtab.Clear()
            dtab.Columns.Clear()
            dam.BuildDataTable(dtab)

            Dim dt As New DataTable("DtMenuDis")

            'Leggo la tabella t_ana_link_menu0_menudis e creo una relazione tra 
            'le voci di menu di livello 0 e i menu disabilitati
            dam.QB.NewQuery()
            dam.QB.AddSelectFields("*")
            dam.QB.AddTables("t_ana_link_menu0_menudis")
            dam.QB.AddWhereCondition("lmm_app_id_0", Comparatori.Uguale, OnVacContext.MenAppId, DataTypes.Stringa)
            dam.QB.AddWhereCondition("lmm_azi_codice", Comparatori.Uguale, OnVacContext.Azienda, DataTypes.Stringa)

            dam.BuildDataTable(dt)

            ds.Tables.Add(dtab)
            ds.Tables.Add(dt)

            If (dt.Rows.Count > 0 And dtab.Rows.Count > 0) Then

                'Prima di creare la relazione verifico manualmente che tutti i figli abbiano un padre nella relazione
                'altrimenti restituisce un errore
                Dim spy As Boolean
                Dim RowsToDel(dt.Rows.Count) As Short
                Dim IndexToDel As Short = -1

                For i As Integer = 0 To dt.Rows.Count - 1
                    spy = False
                    For j As Integer = dtab.Rows.Count - 1 To 0 Step -1
                        If dt.Rows(i)("lmm_men_weight_0") = dtab.Rows(j)("men_weight") And dt.Rows(i)("lmm_app_id_0") = dtab.Rows(j)("men_app_id") Then
                            spy = True
                            Exit For
                        End If
                    Next
                    If (Not spy) Then
                        'salvo l'indice delle righe da cancellare
                        IndexToDel += 1
                        RowsToDel(IndexToDel) = i
                    End If
                Next

                'cancello tutte le righe dal datatable figlio(dt) che non hanno padre in dtab
                For i As Integer = 0 To IndexToDel
                    dt.Rows(RowsToDel(i)).Delete()
                Next
                dt.AcceptChanges()

                Dim ParentColumns(1) As DataColumn
                ParentColumns(0) = dtab.Columns("men_app_id")
                ParentColumns(1) = dtab.Columns("men_weight")

                Dim ChildColumns(1) As DataColumn
                ChildColumns(0) = dt.Columns("lmm_app_id_0")
                ChildColumns(1) = dt.Columns("lmm_men_weight_0")

                ds.Relations.Add(New DataRelation("MenuRel", ParentColumns, ChildColumns))

            End If

            ' Tutti i campi della query che riempie la tabella dtab
            ' devono avere valore uguale perchè nella UNION devono venire unificati
            ' in modo che rimanga solo una sola riga con chiave (men_app_id, men_weight)
            ' In particolare controlla se il men_nome non contenga differenze come degli spazi
            ' tra un menu e l'altro

        End Using

        '*********
        LoadPersonalAppMenu = ds
        '*********

    End Function

#End Region

#Region " Rendering "

    Private Sub RenderMenu(ByRef ds As System.Data.DataSet)

        ' selezione delle righe che interessano i menu null (root)
        Dim prePath = ConfigurationManager.AppSettings.Get("MainWebFolder").ToString()

        Dim dv As DataView = ds.Tables("DtMenu").DefaultView()
        dv.Sort = "men_posizione ASC"

        dv.RowFilter = "men_weight like '%" +
            StrDup(Constants.CommonConstants.MENU_LEVELS - Constants.CommonConstants.MENU_VOICE_LENGHT, Constants.CommonConstants.MENU_PADDING_CHARACTER) +
            "%' and (men_disabilitato <>'T' or men_disabilitato is NULL)"

        ' --- Rendering --- '

        Dim riga As DataRowView = Nothing

        Dim spy As Boolean = False
        Dim TmpMenDis As String = String.Empty

        Dim stbJs As New System.Text.StringBuilder()

        Dim stbHtml As New System.Text.StringBuilder()

        Dim controllaRiga As Boolean = False

        For Each riga In dv

            stbJs.Remove(0, stbJs.Length)

            ' Caricamento leftFrame
            If Not IsDBNull(riga("men_url")) Then

                'carica direttamente il MainFrame
                'Modifica Mauro 13-08-2003: modifica alla gestione dei menu disabilitati
                Dim sb As New System.Text.StringBuilder()

                Dim dr() As DataRow = riga.Row.GetChildRows("MenuRel")

                For i As Integer = 0 To dr.Length - 1
                    sb.AppendFormat("{0}-{1}*", dr(i)("lmm_app_id_dis").ToString(), dr(i)("lmm_men_weight_dis").ToString())
                Next
                If sb.Length > 0 Then sb.Remove(sb.Length - 1, 1)

                'creo la querystring da passare al Main contenente l'elenco dei menu da caricare in seguito nella left
                stbJs.AppendFormat("topMenu_LoadMain('{0}{1}{2}",
                                   prePath.ToString(),
                                   riga("men_url").ToString(),
                                   GetQueryStringForAppId(riga("men_url").ToString(), riga("men_app_id").ToString()))

                If dr.Length > 0 Then
                    If Not spy Then TmpMenDis = sb.ToString()
                    stbJs.AppendFormat("&menu_dis={0}", sb.ToString())
                End If

                stbJs.Append("'); ")

                If Not spy Then spy = True

            Else

                controllaRiga = True

                'Modifica Mauro 28-07-2003: Eliminazione del men_id dalla chiave dei menu
                stbJs.AppendFormat("topMenu_ItemClick('{0}{1}&men_weight={2}');",
                                   LeftVirtualPath,
                                   GetQueryStringForAppId(LeftVirtualPath, riga("men_app_id").ToString()),
                                   riga("men_weight").ToString())

            End If

            'riga fissa
            stbHtml.Append("<TD class=""out"" onmouseover=""this.className='over'; "" onmouseout=""this.className='out'; "">")

            Dim target As String = String.Empty
            If controllaRiga Then
                target = "LeftFrame"
            Else
                target = "TopFrame"
            End If

            stbHtml.AppendFormat("<A class=""menu"" href=""Javascript: {0}"" TARGET=""{1}"" >", stbJs.ToString(), target)

            '2° parametro
            stbHtml.AppendFormat("{0}</A></TD>", riga("men_nome").ToString())

        Next

        htmlRendered = stbHtml.ToString()

        ' --- Script per autocaricamento --- '
        stbJs.Remove(0, stbJs.Length)
        If (dv.Count > 0) Then

            If Not IsDBNull(dv(0)("men_url")) Then

                Dim menuDis As String = String.Empty
                If Not String.IsNullOrEmpty(TmpMenDis) Then
                    menuDis = String.Format("&menu_dis={0}", TmpMenDis)
                End If

                stbJs.AppendFormat(" top.frames[2].location='{0}{1}{2}{3}' ; ",
                                   prePath.ToString(),
                                   dv(0)("men_url").ToString(),
                                   GetQueryStringForAppId(dv(0)("men_url").ToString(), riga("men_app_id").ToString()),
                                   menuDis)

            End If

            RenderScript(dv(0)("men_app_id").ToString, dv(0)("men_weight").ToString, stbJs.ToString())

        End If

    End Sub

    ' Renderizza lo script per l'auto apertura dei frame sinistro e centrale
    Private Sub RenderScript(men_app_id As String, men_weight As String, mainUrl As String)

        Dim stb As New System.Text.StringBuilder()

        stb.Append("<script language=javascript> window.onload = apriLeft; ")
        stb.Append("function apriLeft(){")
        stb.Append("tryShowWaitScreen();")
        stb.AppendFormat("top.frames[1].location=""{0}{1}&men_weight={2}""; {3}",
                         LeftVirtualPath,
                         GetQueryStringForAppId(LeftVirtualPath, men_app_id),
                         men_weight,
                         mainUrl)

        stb.Append(" return true;} </script>")

        JsRendered = stb.ToString()

    End Sub

    ' Aggiunta Marco 31.08.2012 (vedi sopra...!)
    ' Se nell'url è già presente il carattere "?", concatena l'appId utilizzando il "&"
    Private Function GetQueryStringForAppId(urlToCheck As String, appIdToAdd As String) As String

        Dim separator As String = "?"
        If urlToCheck.Contains("?") Then separator = "&"

        Return String.Format("{0}MenAppId={1}&AppId={2}", separator, appIdToAdd, OnVacContext.AppId)

    End Function

#End Region

End Class
