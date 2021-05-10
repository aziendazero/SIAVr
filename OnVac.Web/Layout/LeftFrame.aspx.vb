Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager


Partial Class LeftFrame
    Inherits OnVac.Common.PageBase

    Protected dtVociMenuPersonali As New DataTable()

#Region " Properties "

    Private Property prePath() As String
        Get
            Return ViewState("prePath")
        End Get
        Set(Value As String)
            ViewState("prePath") = Value
        End Set
    End Property

    Private Property caricaScript() As Boolean
        Get
            Return ViewState("caricaScript")
        End Get
        Set(Value As Boolean)
            ViewState("caricaScript") = Value
        End Set
    End Property

    ' Vengono passati tramite QueryString i onmeddb.Parametri: 
    ' app_id: identificativo dell'applicativo corrente
    ' men_id: menu corrente dell'applicativo (quello per cui si sta costruendo la leftbar)
    ' enable: indica se la left va abilitata (cioè con i link funzionanti) oppure no
    ' url   : stringa contenente l'url della prima pagina da caricare a sinistra. 
    '         Se c'è, ha la precedenza sugli altri (del db)
    Private Property men_weight() As String
        Get
            Return ViewState("men_weight")
        End Get
        Set(Value As String)
            ViewState("men_weight") = Value
        End Set
    End Property

    Private Property app_id() As String
        Get
            Return ViewState("app_id")
        End Get
        Set(Value As String)
            ViewState("app_id") = Value
        End Set
    End Property

    Private Property enable() As Boolean
        Get
            Return ViewState("enable")
        End Get
        Set(Value As Boolean)
            ViewState("enable") = Value
        End Set
    End Property

    Private Property url() As String
        Get
            Return ViewState("url")
        End Get
        Set(Value As String)
            ViewState("url") = Value
        End Set
    End Property

    Private Property MenuVector() As String(,)
        Get
            Return Session("MenuVector")
        End Get
        Set(Value As String(,))
            Session("MenuVector") = Value
        End Set
    End Property

    Private Property FromMain() As String
        Get
            Return ViewState("FromMain")
        End Get
        Set(Value As String)
            ViewState("FromMain") = Value
        End Set
    End Property

    Private Property MenuDis() As String
        Get
            Return ViewState("MenuDis")
        End Get
        Set(Value As String)
            ViewState("MenuDis") = Value
        End Set
    End Property

    Private Property SLF() As Boolean
        Get
            Return ViewState("SLF")
        End Get
        Set(Value As Boolean)
            ViewState("SLF") = Value
        End Set
    End Property

    Private ReadOnly Property CodiceAzienda() As String
        Get
            Return OnVacContext.Azienda
        End Get
    End Property

#End Region

#Region " Codice generato da Progettazione Web Form "


    <Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

    'Sub utilizzata per costruire un vettore contenente i nomi delle classi delle singole voci di menu
    Private Sub AddMenuToMenuVector(value As String, grp As Integer)

        Dim i As Integer = 0

        While i < MenuVector.GetUpperBound(1) AndAlso Not MenuVector(grp, i) Is Nothing
            i += 1
        End While

        If i < MenuVector.GetUpperBound(1) Then MenuVector(grp, i) = value

    End Sub

    'Questa funzione formatta il vettore MenuVector in javascript per poter sputare il vettore lato client con un response.Write
    Private Function RenderScriptMenuVector(MenuVector As String(,)) As String

        Dim sb As New Text.StringBuilder()

        'Riga 0
        sb.Append("<script language=""javascript"">")
        sb.Append(vbNewLine)

        'Riga 1
        sb.Append("MenuVector=new Array(")
        sb.Append(MenuVector.GetUpperBound(0))
        sb.Append(");")
        sb.Append(vbNewLine)

        'Riga 2
        sb.Append("for(i=0;i<")
        sb.Append(MenuVector.GetUpperBound(0))
        sb.Append(";i++)")
        sb.Append(vbNewLine)

        'Riga 3
        sb.Append("MenuVector[i]=new Array(")
        sb.Append(MenuVector.GetUpperBound(1))
        sb.Append(");")
        sb.Append(vbNewLine)

        'Righe 4 -> Menuvector.length + 2
        For i As Integer = 0 To MenuVector.GetUpperBound(0) - 1
            For j As Integer = 0 To MenuVector.GetUpperBound(1) - 1
                sb.Append("   MenuVector[")
                sb.Append(i)
                sb.Append("][")
                sb.Append(j)
                sb.Append("]=""")
                sb.Append(MenuVector(i, j))
                sb.Append(""";")
                sb.Append(vbNewLine)
            Next
        Next

        'Riga Menuvector.length + 3
        'Aggiungo una funzione lato client che restituisce <tab>|<pos> della voce di menu identificata dalla classe <classname>
        sb.Append(vbNewLine)
        sb.Append("function GetTabPos(classname)")
        sb.Append("{")
        sb.Append("   found=false;")
        sb.Append(" for (i=0;i<") : sb.Append(MenuVector.GetUpperBound(0)) : sb.Append("&& !found;i++)")
        sb.Append(" { for (j=0;j<") : sb.Append(MenuVector.GetUpperBound(1)) : sb.Append("&& !found;j++)")
        sb.Append(" { if(MenuVector[i][j]==classname) found=true;")
        sb.Append("}")
        sb.Append("}")
        sb.Append("if(found) return ") : sb.Append("(i-1).toString()+") : sb.Append("""|""") : sb.Append("+(j-1).toString()") : sb.Append(";")
        sb.Append(" else return """";")
        sb.Append("}")
        sb.Append(vbNewLine)
        sb.Append("</script>")

        Return sb.ToString()

    End Function

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Me.SLF = False

        If Not IsPostBack Then

            ' Lettura parametri dalla query string passati da TopFrame.aspx
            If Not Request.QueryString.Item("enable") Is Nothing Then
                enable = Request.QueryString("enable")
            Else
                enable = True
            End If

            If Not Request.QueryString("url") Is Nothing Then
                url = Request.QueryString("url")
            Else
                url = ""
            End If

            FromMain = Request.QueryString.Item("FromMain")
            If Not FromMain Is Nothing Then
                MenuDis = Request.QueryString("MenuDis")
            End If

            men_weight = Request.QueryString("men_weight")

            app_id = Request.QueryString("MenAppId")
            If String.IsNullOrEmpty(app_id) Then app_id = String.Empty ' Se è Nothing, la query successiva dà un'eccezione "non tutte le variabili sono associate".

            dtVociMenuPersonali.Clear()
            dtVociMenuPersonali.Columns.Clear()

            prePath = ConfigurationManager.AppSettings.Get("MainWebFolder").ToString()

            UltraWebListbar.ClientSideEvents.InitializeListbar = "impostaListbar"

            ClientScript.RegisterStartupScript(Me.GetType(), "startEnableLeft", "<script language='javascript'> function impostaListbar(oCtl){oCtl.setEnabled(" & enable.ToString().ToLower() & ");}</script>")

            ' **** integrazione con "SharePoint" (zorz 6/7/06)  ****
            ' --
            Dim sharePointCurrentUserGroupsId As New List(Of String)
            ' --
            'If OnMedUtility.OnSharePortalIntegrated Then
            '    '--
            '    Dim membershipService As New OnSharePortalMembershipService.MembershipService
            '    Dim userGroupMemberships As OnSharePortalMembershipService.Membership()
            '    Dim onSharePortalWebServicesUserInfo As portale.Shared.OnSharePortalWebServicesUserInfo = portale.Shared.GlobalUtility.OnSharePortalWebServicesUserInfo
            '    ' --
            '    membershipService.Credentials = New System.Net.NetworkCredential(OnSharePortalWebServicesUserInfo.UserName, OnSharePortalWebServicesUserInfo.Password, OnSharePortalWebServicesUserInfo.Domain)
            '    '--
            '    userGroupMemberships = membershipService.GetUserGroupMemberships()
            '    '--
            '    For Each userGroupMembership As OnSharePortalMembershipService.Membership In userGroupMemberships
            '        If userGroupMembership.IsDomainGroup AndAlso context.User.IsInRole(userGroupMembership.LoginName) Then sharePointCurrentUserGroupsId.Add(userGroupMembership.Id.ToString)
            '    Next
            '    '--
            'End If
            '' **** ********************************************  ****

            Try

                ' Creazione DAM in base al provider
                Using mp As IDAM = OnVacUtility.OpenDamToOnitManager()

                    ' Query per la ricerca dei menù nel DB
                    ' La query seleziona, dalla tabella t_ana_menu, tutti gli elementi che formano i 
                    ' menù dell'applicazione specificata (app_id); solo gli elementi a cui l'utente 
                    ' è abilitato ad accedere saranno selezionati da t_ana_menu. Tali elementi 
                    ' sono mantenuti ordinati dentro il datatable dtVociMenuPersonali; in base a
                    ' questo datatable viene creata la LeftBar, che visualizzerà tutte le voci e i
                    ' gruppi del menù selezionato dal TopFrame (men_id). Il menù selezionato è il
                    ' primo tab della LeftBar, che conterrà tutte le voci di menù al suo interno. 
                    ' Eventuali sottomenù di men_id verranno visualizzati come tab separati.
                    ' Il primo link del primo tab viene aperto automaticamente nella pagina di 
                    ' sinistra (Mainframe), a meno che anche il menù selezionato (men_id) non abbia
                    ' associato un URL: in questo caso viene aperto automaticamente questo url, 
                    ' direttamente da TopFrame
                    mp.QB.NewQuery()

                    mp.QB.AddTables("t_ana_menu, t_ana_link_utenti_menu, t_ana_link_applicativi_moduli, t_ana_moduli")

                    mp.QB.AddSelectFields("men_app_id", mp.QB.FC.IsNull("men_url", "mod_url", DataTypes.Replace) & " men_url")
                    mp.QB.AddSelectFields("men_nome, men_posizione", mp.QB.FC.IsNull("men_img", "mod_img", DataTypes.Replace) & " men_img")
                    mp.QB.AddSelectFields("men_weight, men_class_name, men_mod_codice")

                    mp.QB.AddWhereCondition("lum_ute_id", Comparatori.Uguale, OnVacContext.UserId, DataTypes.Numero)
                    mp.QB.AddWhereCondition("lum_men_app_id", Comparatori.Uguale, app_id, DataTypes.Stringa)
                    mp.QB.AddWhereCondition("men_app_id", Comparatori.Uguale, "lum_men_app_id", DataTypes.Join)
                    mp.QB.AddWhereCondition("men_weight", Comparatori.Uguale, "lum_men_weight", DataTypes.Join)
                    mp.QB.AddWhereCondition("men_azi_codice", Comparatori.Uguale, CodiceAzienda, DataTypes.Stringa)
                    mp.QB.AddWhereCondition("lum_azi_codice", Comparatori.Uguale, CodiceAzienda, DataTypes.Stringa)
                    mp.QB.AddWhereCondition("men_mod_codice", Comparatori.Uguale, "amo_mod_codice", DataTypes.OutJoinLeft)
                    mp.QB.AddWhereCondition("amo_mod_codice", Comparatori.Uguale, "mod_codice", DataTypes.OutJoinLeft)

                    mp.QB.AddUnion(mp.QB.GetSelect)

                    mp.QB.NewQuery(False)

                    mp.QB.AddTables("t_ana_link_gruppi_menu, t_ana_menu, t_ana_link_applicativi_moduli, t_ana_moduli")

                    mp.QB.AddSelectFields("men_app_id", mp.QB.FC.IsNull("men_url", "mod_url", DataTypes.Replace) & " men_url")
                    mp.QB.AddSelectFields("men_nome, men_posizione", mp.QB.FC.IsNull("men_img", "mod_img", DataTypes.Replace) & " men_img")
                    mp.QB.AddSelectFields("men_weight, men_class_name, men_mod_codice")

                    mp.QB.AddWhereCondition("lgm_men_app_id", Comparatori.Uguale, app_id, DataTypes.Stringa)
                    mp.QB.AddWhereCondition("lgm_men_weight", Comparatori.Uguale, "men_weight", DataTypes.Join)
                    mp.QB.AddWhereCondition("lgm_men_app_id", Comparatori.Uguale, "men_app_id", DataTypes.Join)
                    mp.QB.AddWhereCondition("lgm_azi_codice", Comparatori.Uguale, CodiceAzienda, DataTypes.Stringa)
                    mp.QB.AddWhereCondition("men_azi_codice", Comparatori.Uguale, CodiceAzienda, DataTypes.Stringa)
                    mp.QB.AddWhereCondition("men_mod_codice", Comparatori.Uguale, "amo_mod_codice", DataTypes.OutJoinLeft)
                    mp.QB.AddWhereCondition("amo_mod_codice", Comparatori.Uguale, "mod_codice", DataTypes.OutJoinLeft)

                    ' **** integrazione con "SharePoint" (zorz 6/7/06)  ****
                    If Not OnVacUtility.OnSharePortalIntegrated Then
                        mp.QB.AddTables("t_ana_link_gruppi_utenti")
                        mp.QB.AddWhereCondition("lgu_ute_id", Comparatori.Uguale, OnVacContext.UserId, DataTypes.Numero)
                        mp.QB.AddWhereCondition("lgu_gru_id", Comparatori.Uguale, "lgm_gru_id", DataTypes.Join)
                    Else
                        mp.QB.AddWhereCondition("lgm_gru_id", Comparatori.In,
                                                String.Join(",", sharePointCurrentUserGroupsId.ToArray()),
                                                DataTypes.Replace)
                    End If

                    mp.QB.AddOrderByFields("men_posizione")

                    mp.QB.AddUnion(mp.QB.GetSelect)

                    ' Pulitura e creazione dataTable
                    dtVociMenuPersonali.Clear()
                    dtVociMenuPersonali.Columns.Clear()

                    mp.BuildDataTable(dtVociMenuPersonali) ' La connessione viene aperta e chiusa dal DAM all'interno di questa funzione

                    ApplyAppIdToUrl(dtVociMenuPersonali)

                End Using

                ' Il datatable ha 6 colonne:
                ' Rows(x).Item(0), nome dell'elemento di menù
                ' Rows(x).Item(1), nome del menù a cui l'elemento appartiene
                ' Rows(x).Item(2), è l'url a cui l'elemento punta
                ' Rows(x).Item(3), caption dell'elemento
                ' Rows(x).Item(4), posizione in cui deve comparire l'elemento nella lista
                ' Rows(x).Item(5), path dell'icona
                ' Rows(x).Item(6), struttura che indica se l'elemento è un gruppo o una voce di menù, 
                '                   e il suo percorso fino alla radice (menù principale)


                ' ********* RIEMPIMENTO TOOLBAR *********
                ' Creazione Vista dei gruppi ordinati per posizione
                Dim dvVistaVociMenu As DataView = dtVociMenuPersonali.DefaultView

                ' Imposto a 99 la posizione dei menù che hanno men_posizione NULL
                Dim i As Integer
                For i = 0 To dvVistaVociMenu.Count - 1
                    If IsDBNull(dvVistaVociMenu(i)("men_posizione")) Then
                        dvVistaVociMenu(i)("men_posizione") = CalcolaPosizione(dvVistaVociMenu(i)("men_weight"))
                    End If
                Next

                Dim nuovo As Boolean ' Serve per la compatibilità con i vecchi applicativi
                Dim intMen As Integer ' Serve per la compatibilità con i vecchi applicativi

                Dim idxGruppo As Integer
                Dim strWeight As String = ""
                Dim strFiltro As String = ""
                Dim strUrl As String = ""

                'Modifica Mauro 13-08-2003: Ulteriore modalita' di gestione della leftbar; se viene invocata
                'dal MainFrame, allora questo passa in querystring il parametro FromMain con valore 1; in questo
                'caso la leftbar si occupa di caricare l'elenco dei menu passati in querystring nella voce MenuDis
                If Not FromMain Is Nothing AndAlso FromMain = "1" Then 'la leftbar viene caricata dal MainFrame

                    'Creo i gruppi
                    Dim dtgruppi As New DataTable()
                    Dim dtvoci As New DataTable()

                    Dim md() As String = MenuDis.Split("*")

                    If md.GetUpperBound(0) >= 0 And md(0) <> "" Then

                        Using dam As IDAM = OnVacUtility.OpenDamToOnitManager()

                            'ricavo i gruppi in base ai permessi degli utenti
                            dam.QB.AddSelectFields("men_app_id, men_nome, men_url, men_descrizione, men_posizione, men_img")
                            dam.QB.AddSelectFields("men_weight, men_disabilitato, men_class_name, men_mod_codice")

                            dam.QB.AddTables("t_ana_menu, t_ana_link_utenti_menu")

                            For i = 0 To md.Length - 1
                                dam.QB.OpenParanthesis()
                                dam.QB.AddWhereCondition("men_app_id", Comparatori.Uguale, md(i).Split("-")(0), DataTypes.Stringa, "or")
                                dam.QB.AddWhereCondition("men_weight", Comparatori.Uguale, md(i).Split("-")(1), DataTypes.Stringa)
                                dam.QB.AddWhereCondition("lum_ute_id", Comparatori.Uguale, OnVacContext.UserId, DataTypes.Stringa)
                                dam.QB.AddWhereCondition("lum_men_app_id", Comparatori.Uguale, "men_app_id", DataTypes.Join)
                                dam.QB.AddWhereCondition("lum_men_weight", Comparatori.Uguale, "men_weight", DataTypes.Join)
                                'filtro x azienda
                                dam.QB.AddWhereCondition("lum_azi_codice", Comparatori.Uguale, CodiceAzienda, DataTypes.Stringa)
                                dam.QB.AddWhereCondition("men_azi_codice", Comparatori.Uguale, CodiceAzienda, DataTypes.Stringa)
                                dam.QB.CloseParanthesis()
                            Next

                            Dim qs As String = dam.QB.GetSelect()

                            dam.QB.NewQuery(False, False)

                            'ricavo i gruppi in base ai permessi dei gruppi
                            dam.QB.AddSelectFields("men_app_id, men_nome, men_url, men_descrizione, men_posizione, men_img")
                            dam.QB.AddSelectFields("men_weight, men_disabilitato, men_class_name, men_mod_codice")
                            dam.QB.AddTables("t_ana_menu, t_ana_link_gruppi_menu")

                            ' **** integrazione con "SharePoint" (zorz 6/7/06)  ****
                            If Not OnVacUtility.OnSharePortalIntegrated Then
                                dam.QB.AddTables("t_ana_link_gruppi_utenti")
                            End If

                            For i = 0 To md.Length - 1

                                dam.QB.OpenParanthesis()

                                dam.QB.AddWhereCondition("men_app_id", Comparatori.Uguale, md(i).Split("-")(0), DataTypes.Stringa, "or")
                                dam.QB.AddWhereCondition("men_weight", Comparatori.Uguale, md(i).Split("-")(1), DataTypes.Stringa)
                                If Not OnVacUtility.OnSharePortalIntegrated Then
                                    dam.QB.AddWhereCondition("lgu_ute_id", Comparatori.Uguale, OnVacContext.UserId, DataTypes.Stringa)
                                    dam.QB.AddWhereCondition("lgu_gru_id", Comparatori.Uguale, "lgm_gru_id", DataTypes.Join)
                                Else
                                    dam.QB.AddWhereCondition("lgm_gru_id", Comparatori.In,
                                                             String.Join(",", sharePointCurrentUserGroupsId.ToArray()),
                                                             DataTypes.Replace)
                                End If
                                dam.QB.AddWhereCondition("men_app_id", Comparatori.Uguale, "lgm_men_app_id", DataTypes.Join)
                                dam.QB.AddWhereCondition("men_weight", Comparatori.Uguale, "lgm_men_weight", DataTypes.Join)
                                dam.QB.AddWhereCondition("men_azi_codice", Comparatori.Uguale, CodiceAzienda, DataTypes.Stringa)
                                dam.QB.AddWhereCondition("LGM_azi_codice", Comparatori.Uguale, CodiceAzienda, DataTypes.Stringa)

                                dam.QB.CloseParanthesis()

                            Next

                            dam.QB.AddOrderByFields("men_posizione")

                            dam.QB.AddUnion(qs, dam.QB.GetSelect)

                            'Costruisco il datatable dei gruppi
                            dam.BuildDataTable(dtgruppi)

                            Me.ApplyAppIdToUrl(dtgruppi)

                            'Modifica Mauro 18-08-2003: modifica alla gestione delle voci di menu disabilitate
                            'Devo caricare tutti i sottomenu dei gruppi disabilitati associati al menu corrente:
                            'infatti devo caricare tutti i links da visualizzare nel left frame
                            Dim EndWeight As String
                            Dim ind, len As Short

                            'seleziono le voci in base ai permessi degli utenti
                            dam.QB.NewQuery()

                            dam.QB.AddSelectFields("men_nome", dam.QB.FC.IsNull("men_url", "mod_url", DataTypes.Replace) & " men_url")
                            dam.QB.AddSelectFields("men_weight, men_posizione, men_class_name", dam.QB.FC.IsNull("men_img", "mod_img", DataTypes.Replace) & " men_img")
                            dam.QB.AddSelectFields("men_mod_codice, men_app_id")

                            dam.QB.AddTables("t_ana_menu, t_ana_link_utenti_menu, t_ana_link_applicativi_moduli, t_ana_moduli")

                            dam.QB.AddWhereCondition("men_app_id", Comparatori.Uguale, app_id, DataTypes.Stringa)
                            dam.QB.AddWhereCondition("lum_ute_id", Comparatori.Uguale, OnVacContext.UserId, DataTypes.Stringa)
                            dam.QB.AddWhereCondition("lum_men_app_id", Comparatori.Uguale, "men_app_id", DataTypes.Join)
                            dam.QB.AddWhereCondition("lum_men_weight", Comparatori.Uguale, "men_weight", DataTypes.Join)
                            dam.QB.AddWhereCondition("men_azi_codice", Comparatori.Uguale, CodiceAzienda, DataTypes.Stringa)
                            dam.QB.AddWhereCondition("lum_azi_codice", Comparatori.Uguale, CodiceAzienda, DataTypes.Stringa)
                            dam.QB.AddWhereCondition("men_mod_codice", Comparatori.Uguale, "amo_mod_codice", DataTypes.OutJoinLeft)
                            dam.QB.AddWhereCondition("amo_mod_codice", Comparatori.Uguale, "mod_codice", DataTypes.OutJoinLeft)

                            dam.QB.OpenParanthesis()
                            For i = 0 To dtgruppi.Rows.Count - 1
                                ind = dtgruppi.Rows(i).Item("men_weight").ToString().LastIndexOf(Constants.CommonConstants.MENU_PADDING_CHARACTER)
                                len = dtgruppi.Rows(i).Item("men_weight").ToString().Length - ind - 1
                                EndWeight = dtgruppi.Rows(i).Item("men_weight").ToString().Substring(ind + 1, len)
                                dam.QB.OpenParanthesis()
                                If (i = 0) Then
                                    dam.QB.AddWhereCondition("men_weight", Comparatori.Like, "%" + EndWeight, DataTypes.Stringa)
                                Else
                                    dam.QB.AddWhereCondition("men_weight", Comparatori.Like, "%" + EndWeight, DataTypes.Stringa, "or")
                                End If
                                dam.QB.AddWhereCondition("men_weight", Comparatori.Diverso, dtgruppi.Rows(i).Item("men_weight"), DataTypes.Stringa)
                                dam.QB.CloseParanthesis()
                            Next
                            dam.QB.CloseParanthesis()

                            'Seleziono le voci in base ai permessi dei gruppi
                            qs = dam.QB.GetSelect(False)

                            dam.QB.NewQuery(False, False)

                            dam.QB.AddTables("t_ana_menu, t_ana_link_gruppi_menu, t_ana_link_applicativi_moduli, t_ana_moduli")
                            dam.QB.AddSelectFields("men_nome", dam.QB.FC.IsNull("men_url", "mod_url", DataTypes.Replace) & " men_url")
                            dam.QB.AddSelectFields("men_weight, men_posizione, men_class_name", dam.QB.FC.IsNull("men_img", "mod_img", DataTypes.Replace) & " men_img")
                            dam.QB.AddSelectFields("men_mod_codice, men_app_id")

                            ' **** integrazione con "SharePoint" (zorz 6/7/06)  ****
                            If Not OnVacUtility.OnSharePortalIntegrated Then
                                dam.QB.AddTables("t_ana_link_gruppi_utenti")
                            End If

                            dam.QB.AddWhereCondition("men_app_id", Comparatori.Uguale, app_id, DataTypes.Stringa)
                            dam.QB.AddWhereCondition("men_azi_codice", Comparatori.Uguale, CodiceAzienda, DataTypes.Stringa)
                            dam.QB.AddWhereCondition("lgm_azi_codice", Comparatori.Uguale, CodiceAzienda, DataTypes.Stringa)


                            ' **** integrazione con "SharePoint" (zorz 6/7/06)  ****
                            If Not OnVacUtility.OnSharePortalIntegrated Then
                                dam.QB.AddWhereCondition("lgu_ute_id", Comparatori.Uguale, OnVacContext.UserId, DataTypes.Stringa)
                                dam.QB.AddWhereCondition("lgu_gru_id", Comparatori.Uguale, "lgm_gru_id", DataTypes.Join)
                            Else
                                dam.QB.AddWhereCondition("lgm_gru_id", Comparatori.In,
                                                         String.Join(",", sharePointCurrentUserGroupsId.ToArray()),
                                                         DataTypes.Replace)
                            End If

                            dam.QB.AddWhereCondition("men_app_id", Comparatori.Uguale, "lgm_men_app_id", DataTypes.Join)
                            dam.QB.AddWhereCondition("men_weight", Comparatori.Uguale, "lgm_men_weight", DataTypes.Join)
                            dam.QB.AddWhereCondition("men_mod_codice", Comparatori.Uguale, "amo_mod_codice", DataTypes.OutJoinLeft)
                            dam.QB.AddWhereCondition("amo_mod_codice", Comparatori.Uguale, "mod_codice", DataTypes.OutJoinLeft)

                            dam.QB.OpenParanthesis()
                            For i = 0 To dtgruppi.Rows.Count - 1
                                ind = dtgruppi.Rows(i).Item("men_weight").ToString().LastIndexOf(Constants.CommonConstants.MENU_PADDING_CHARACTER)
                                len = dtgruppi.Rows(i).Item("men_weight").ToString().Length - ind - 1
                                EndWeight = dtgruppi.Rows(i).Item("men_weight").ToString().Substring(ind + 1, len)
                                dam.QB.OpenParanthesis()
                                If (i = 0) Then
                                    dam.QB.AddWhereCondition("men_weight", Comparatori.Like, "%" + EndWeight, DataTypes.Stringa)
                                Else
                                    dam.QB.AddWhereCondition("men_weight", Comparatori.Like, "%" + EndWeight, DataTypes.Stringa, "or")
                                End If
                                dam.QB.AddWhereCondition("men_weight", Comparatori.Diverso, dtgruppi.Rows(i).Item("men_weight"), DataTypes.Stringa)
                                dam.QB.CloseParanthesis()
                            Next
                            dam.QB.CloseParanthesis()

                            dam.QB.AddOrderByFields("men_posizione")

                            dam.QB.AddUnion(qs, dam.QB.GetSelect)

                            'costruisco il datatable contenente le voci dei menu per ogni gruppo
                            dam.BuildDataTable(dtvoci)

                            Me.ApplyAppIdToUrl(dtvoci)

                        End Using

                    End If

                    '***********************************************************
                    'AGGIUNGO LE VOCI DI MENU ALLA LEFTBAR INFRAGISTICS
                    If dtgruppi.Rows.Count > 0 Then dtgruppi.DefaultView.Sort = "men_posizione ASC"

                    For i = 0 To dtgruppi.Rows.Count - 1
                        If Not IsDBNull(dtgruppi.Rows(i)("men_nome")) Then
                            Me.UltraWebListbar.Groups.Add(CreaGruppoLeftBarInfragistic(dtgruppi.Rows(i)("men_nome"), i))
                        Else
                            Me.UltraWebListbar.Groups.Add(CreaGruppoLeftBarInfragistic("", i))
                        End If
                    Next

                    For j As Integer = 0 To dtvoci.Rows.Count - 1

                        'Determino a quale gruppo appartiene l'item da creare                        
                        idxGruppo = DeterminaGruppo(dtgruppi, dtvoci.Rows(j)("men_weight").ToString())
                        If idxGruppo <> -1 Then
                            'Creo l'item
                            Me.UltraWebListbar.Groups(idxGruppo).Items.Add(CreaItemLeftBarInfragisticEnable(dtvoci.Rows(j)("men_nome").ToString(),
                                                                                                         dtvoci.Rows(j)("men_url").ToString(),
                                                                                                         dtvoci.Rows(j)("men_img").ToString()))
                        End If

                    Next
                    '***********************************************************

                    If dtvoci.Rows.Count = 0 Then
                        'nessun  menu da renderizzare nella left bar (?) => chiudo la leftbar
                        SLF = True
                        If Me.UltraWebListbar.Groups.Count = 0 Then Me.UltraWebListbar.Groups.Add(New Infragistics.WebUI.UltraWebListbar.Group())
                        Me.UltraWebListbar.Groups(0).Items.Add(New Infragistics.WebUI.UltraWebListbar.Item())
                    Else
                        SLF = False
                    End If

                Else 'la leftbar viene caricata dal TopFrame

                    ' Ricerca nel datatable dell'indice del menù 
                    intMen = CercaElemento(app_id, men_weight)

                    ' Se l'elemento è stato trovato 
                    If intMen <> -1 Then

                        ' nuovo è impostato in base alla presenza o meno del men_weight
                        nuovo = Not dtVociMenuPersonali.Rows(intMen).IsNull("men_weight")

                        strWeight = dtVociMenuPersonali.Rows(intMen)("men_weight").ToString()

                        strUrl = prePath & dtVociMenuPersonali.Rows(intMen)("men_url").ToString()

                        dvVistaVociMenu.Sort = "men_posizione"

                        ' Creazione Vista dei soli elementi che sono gruppi
                        strFiltro = "(men_weight like 'G%') and (men_weight like '%" & strWeight.Substring(strWeight.Length - 2) & "')"

                        dvVistaVociMenu.RowFilter = strFiltro

                        ' Creazione dei gruppi ordinati
                        For i = 0 To dvVistaVociMenu.Count - 1

                            Me.UltraWebListbar.Groups.Add(
                                CreaGruppoLeftBarInfragistic(dvVistaVociMenu(i)("men_nome").ToString(), i))

                        Next

                        ' Creazione Vista delle voci di menù dell'applicazione, ordinate per posizione
                        dvVistaVociMenu.RowFilter = "men_weight like 'V%' and (men_weight like '%" & strWeight.Substring(strWeight.Length - 2) & "')"

                        'LEFTBAR INFRAGISTICS
                        Dim strPesoElemento As String = ""
                        Dim captionGruppo As String = ""

                        For i = 0 To dvVistaVociMenu.Count - 1

                            ' Determino a quale gruppo appartiene l'item da creare
                            strPesoElemento = dvVistaVociMenu(i)("men_weight").ToString()

                            captionGruppo = DeterminaGruppo(strPesoElemento)

                            ' Se il caption è "" il gruppo a cui l'elemento appartiene
                            ' non esiste o è disabilitato, perciò l'item non va creata
                            If captionGruppo <> "" Then
                                ' Ricerco il gruppo di appartenenza nella leftbar
                                idxGruppo = CercaGruppo(captionGruppo, Me.UltraWebListbar)
                                If (idxGruppo <> -1) Then
                                    ' Creo l'item
                                    Me.UltraWebListbar.Groups(idxGruppo).Items.Add(CreaItemLeftBarInfragisticEnable(dvVistaVociMenu(i)("men_nome").ToString(),
                                                                                                                 dvVistaVociMenu(i)("men_url").ToString(),
                                                                                                                 dvVistaVociMenu(i)("men_img").ToString()))
                                End If 'idxGruppo <> -1
                            End If 'captiongruppo <> ""

                        Next

                    End If ' if (intMen <> -1)

                    'INFRAGISTICS
                    ' Rimozione Gruppi vuoti (senza item)
                    For i = Me.UltraWebListbar.Groups.Count - 1 To 0 Step -1
                        ' Cancellazione gruppi vuoti
                        If Me.UltraWebListbar.Groups(i).Items.Count = 0 Then
                            Me.UltraWebListbar.Groups.Remove(Me.UltraWebListbar.Groups(i))
                        End If
                    Next

                    ' Caricamento automatico pagina destra, in base al campo url
                    If url = "" Then

                        If Me.UltraWebListbar.Groups.Count > 0 Then

                            'esistono menu da renderizzare (che sia qui il controllo buono ???)
                            ' Controlla che non ci sia già impostato l'url dell'applicazione
                            If strUrl = prePath And Me.UltraWebListbar.Groups(0).Items.Count <> 0 Then
                                strUrl = Me.UltraWebListbar.Groups(0).Items(0).TargetUrl
                                If Not strUrl Is Nothing AndAlso strUrl <> String.Empty Then
                                    RenderScript(strUrl)
                                End If
                            End If

                            'nessun  menu da renderizzare nella left bar (?) => chiudo la leftbar
                            'INFRAGISTICS
                            If Me.UltraWebListbar.Groups.Count > 1 Then
                                SLF = False
                            Else
                                If Me.UltraWebListbar.Groups.Count = 1 Then
                                    If Me.UltraWebListbar.Groups(0).Items.Count < 2 Then
                                        SLF = True
                                    Else
                                        SLF = False
                                    End If
                                Else
                                    Me.UltraWebListbar.Groups.Add(New Infragistics.WebUI.UltraWebListbar.Group())
                                    Me.UltraWebListbar.Groups(0).Items.Add(New Infragistics.WebUI.UltraWebListbar.Item())
                                    SLF = True
                                End If
                            End If

                        Else

                            Me.UltraWebListbar.Groups.Add(New Infragistics.WebUI.UltraWebListbar.Group())
                            Me.UltraWebListbar.Groups(0).Items.Add(New Infragistics.WebUI.UltraWebListbar.Item())

                            SLF = True

                        End If

                    Else

                        RenderScript(url)

                    End If

                End If 'la leftbar viene caricata dal MainFrame

            Finally

                dtVociMenuPersonali.Dispose()
                dtVociMenuPersonali = Nothing

            End Try

        Else

            Me.UltraWebListbar.ClientSideEvents.InitializeListbar = Nothing

        End If

    End Sub

    ' Controlla la presenza del gruppo con caption=id.
    ' Ritorna la posizione del gruppo nella leftBar se c'è, -1 se è disabilitato o non c'è
    Private Function CercaGruppo(strCaptionGruppo As String, infraLeftBar As Infragistics.WebUI.UltraWebListbar.UltraWebListbar) As Integer

        Dim i As Integer = 0
        Dim trovato As Boolean = False

        If infraLeftBar.Groups.Count = 0 Then

            i = -1

        Else

            While (i < infraLeftBar.Groups.Count) And (Not trovato)
                If (infraLeftBar.Groups(i).Text = strCaptionGruppo) Then
                    trovato = True
                End If
                i += 1
            End While

            If Not trovato Then
                i = -1
            Else
                i -= 1
            End If

        End If

        Return i

    End Function

    ' Cerca il gruppo di appartenenza dell'elemento con men_weight = al peso dato.
    ' Ritorna il caption del gruppo o "" se non lo trova (caso che non dovrebbe capitare mai!)
    Private Function DeterminaGruppo(strPesoElemento As String) As String

        Dim idxSub, i As Integer
        Dim trovato As Boolean

        ' Calcolo l'inizio della sottostringa del peso dell'elemento 
        ' che indica il peso del gruppo padre
        idxSub = strPesoElemento.LastIndexOf("X")

        If (idxSub = -1) Then
            ' Se non trova nessuna "X", l'elemento è di ultimo livello con peso:
            ' "V12345678", quindi il gruppo padre è "GXX345678", 
            ' perciò la sottostringa da considerare parte dal  3° carattere
            idxSub = 3
        Else
            ' L'elemento è di un livello qualsiasi, ad esempio: "VXXXX5678".
            ' idxSub=4, la sottostringa padre deve partire da 7, quindi aggiungo 3
            idxSub += 3
        End If

        ' Se l'elemento è di primo livello ("VXXXXXX78", idxSub+3 supera la lunghezza 
        ' della stringa, perciò la sottostringa padre va fatta partire dagli ultimi 
        ' 2 caratteri 
        '(caso che non dovrebbe capitare mai perchè al primo lvl non ci sono voci di menù)
        If idxSub >= strPesoElemento.Length Then
            idxSub = strPesoElemento.Length - 2
        End If

        ' Aggiunta delle "X" per il men_weight del padre
        Dim stbX As New System.Text.StringBuilder()
        For i = 1 To idxSub - 1
            stbX.Append("X")
        Next

        Dim strPesoGruppo As String = String.Format("G{0}{1}", stbX.ToString(), strPesoElemento.Substring(idxSub))

        ' Cerca il caption del gruppo nel datatable in base al peso
        i = 0
        trovato = False
        While (i < dtVociMenuPersonali.Rows.Count) And (Not trovato)
            If (strPesoGruppo <> dtVociMenuPersonali.Rows(i).Item("men_weight").ToString()) Then
                i += 1
            Else
                trovato = True
            End If
        End While

        Dim strCaptionGruppo As String = String.Empty

        If trovato Then
            strCaptionGruppo = Microsoft.VisualBasic.RTrim(dtVociMenuPersonali.Rows(i).Item("men_nome").ToString())
        End If

        Return strCaptionGruppo

    End Function

    Private Function DeterminaGruppo(dtGruppi As DataTable, strPesoElemento As String) As Integer

        Dim ind As Short = strPesoElemento.LastIndexOf(Constants.CommonConstants.MENU_PADDING_CHARACTER)

        For i As Integer = 0 To dtGruppi.Rows.Count - 1
            If (strPesoElemento = dtGruppi.Rows(i)("men_weight").ToString()) Then
                Return -1
            ElseIf (strPesoElemento.Substring(ind + 1 + Constants.CommonConstants.MENU_VOICE_LENGHT) = dtGruppi.Rows(i)("men_weight").ToString().Substring(ind + 1 + Constants.CommonConstants.MENU_VOICE_LENGHT)) Then
                Return i
            End If
        Next

        Return -1

    End Function

    ' Ritorna l'indice dell'elemento cercato all'interno del dataTable
    Private Function CercaElemento(men_app_id As String, m_weight As String) As Integer

        If m_weight Is Nothing Then Return -1

        Dim i As Integer = 0
        Dim trovato As Boolean = False

        While (i < dtVociMenuPersonali.Rows().Count()) And (Not trovato)
            If (Microsoft.VisualBasic.RTrim(dtVociMenuPersonali.Rows(i).Item("men_app_id").ToString().ToLower) <> app_id.ToLower OrElse Microsoft.VisualBasic.RTrim(dtVociMenuPersonali.Rows(i).Item("men_weight").ToString().ToLower) <> m_weight.ToLower) Then
                i += 1
            Else
                trovato = True
            End If
        End While

        ' Ritorna i se l'elemento c'è, -1 altrimenti
        If Not trovato Then
            i = -1
        End If

        Return i

    End Function

    'crea un gruppo per la leftbar dell'infragistics
    Private Function CreaGruppoLeftBarInfragistic(caption As String, groupKey As Integer) As Infragistics.WebUI.UltraWebListbar.Group

        'evitare che il key del gruppo sia in integer, poichè può causare anomalie nell'eliminazione dei gruppi vuoti
        Dim gr As New Infragistics.WebUI.UltraWebListbar.Group(caption, "gp_" + groupKey.ToString())
        gr.Text = caption

        Return gr

    End Function

    Private Function CreaItemLeftBarInfragisticEnable(caption As String, url As String, img As String) As Infragistics.WebUI.UltraWebListbar.Item

        Dim item As New Infragistics.WebUI.UltraWebListbar.Item("Item1", "")

        ' --- Modifica per passaggio a Infragistics 7.1 --- '
        ' La versione 7 degli Infragistics renderizza ogni item dalla listbar con la descrizione di fianco all'icona, sulla stessa riga,
        ' anzichè nella riga sottostante. Per evitare ciò, ho aggiunto un tag <br> prima della descrizione, per farla andare a capo. 
        ' Non ho trovato nessuna proprietà da impostare per l'oggetto, per ottenere lo stesso risultato in maniera migliore.
        '
        item.Text = "<br>" + caption
        ' ------------------------------------------------- '

        If img <> "" Then
            item.Image = prePath & img
        End If

        If url <> "" Then
            item.TargetUrl = prePath & url
        End If

        item.TargetFrame = "MainFrame"

        Return item

    End Function

    ' renderizza lo script per l'auto-apertura del frame sinistro
    Private Sub RenderScript(url As String)

        If url = prePath Then
            url = String.Empty
        End If

        Dim stb As New System.Text.StringBuilder()
        stb.AppendFormat("<script language=javascript> function apriCenter(){{top.frames[2].location='{0}'; return true;}} apriCenter(); </script>", url)

        Response.Write(stb.ToString())

    End Sub

    ' Restitisce la posizione del menù relativa al livello a cui il menù appartiene
    Private Function CalcolaPosizione(strPeso As String) As Integer

        ' strPeso è il men_weight
        Dim lvl As Integer = 0

        ' Calcolo del livello: voglio ricavare la parte finale di strPeso, quella senza le X
        ' perchè, dalla sua lunghezza, ottengo il livello del menù
        strPeso = strPeso.Substring(strPeso.LastIndexOf("X") + 1)

        lvl = strPeso.Length / 2 - 1    ' /2 xchè ogni livello è descritto da 2 cifre, -1 xchè i livelli partono da 0

        Return (99 + 100 * lvl)

    End Function

    Private Function WriteScriptSLF(slf As Boolean) As String

        Dim sb As New System.Text.StringBuilder()

        sb.Append("<script language=""javascript1.3"">")
        sb.Append(vbNewLine)
        sb.Append("function BodyOnLoad(){")
        If (slf) Then
            sb.Append("SuppressLeftFrame(true)")
        Else
            sb.Append("SuppressLeftFrame(false)")
        End If
        sb.Append("}")
        sb.Append(vbNewLine)
        sb.Append("</script>")

        Return sb.ToString()

    End Function

    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles MyBase.PreRender

        If Not MenuVector Is Nothing Then ClientScript.RegisterStartupScript(Me.GetType(), "RenderMenuVector", RenderScriptMenuVector(MenuVector))

        ClientScript.RegisterStartupScript(Me.GetType(), "BodyOnLoad", WriteScriptSLF(SLF))

    End Sub

    Public Sub ApplyAppIdToUrl(dtVoci As DataTable)

        Dim dr As DataRow
        Dim strUrl As String

        Dim drs() As DataRow = dtVoci.Select("not men_mod_codice is null")

        For i As Integer = 0 To drs.Length - 1

            dr = drs(i)

            strUrl = dr("men_url").ToString()

            If strUrl <> String.Empty Then
                strUrl = strUrl + IIf(strUrl.IndexOf("?") > 0, "&", "?") + String.Format("AppId={0}&MenWeight={1}", OnVacContext.AppId, dr("men_weight"))
                dr("men_url") = strUrl
                dr.AcceptChanges()
            End If

        Next

    End Sub

End Class

