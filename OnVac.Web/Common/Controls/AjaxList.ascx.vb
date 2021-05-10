Imports System.Data.OracleClient

Imports Onit.Shared.NTier.Security
Imports Onit.OnAssistnet.Web
Imports Onit.Shared.Manager.OnitProfile
Imports Microsoft.Security.Application


'<Assembly: TagPrefix("OnVac.Common.Controls", "onitcontrols")> 

Namespace Common.Controls

    Partial Class AjaxList
        Inherits System.Web.UI.UserControl

#Region "Codice generato da Progettazione Web Form "

        Private WithEvents dtParent As System.Web.UI.WebControls.DataGrid
        Protected WithEvents dgrParent As System.Web.UI.WebControls.DataGrid

        'Chiamata richiesta da Progettazione Web Form.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
        'Non spostarla o rimuoverla.
        Private designerPlaceholderDeclaration As System.Object

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
            'Non modificarla nell'editor del codice.
            InitializeComponent()
        End Sub

#End Region

#Region "Private var"

        Private _label As String
        Private _codice As String
        Private _descrizione As String
        Private _tabella As String
        Private _filtro As String
        Private _PostBackOnSelect As Boolean
        Private _Tutti As Boolean
        Private _app_id As String

        Private codClientId As String
        Private descClientId As String
        Private parentClientId As String
        Private _MieiCNS As Boolean

#End Region

#Region " Proprietà "

        Public Property Label() As String
            Get
                Return _label
            End Get
            Set(Value As String)
                _label = Value
            End Set
        End Property

        Public Property Codice() As String
            Get
                Return Me.parentCod.Text
            End Get
            Set(Value As String)
                Me.parentCod.Text = Value
            End Set
        End Property

        Public Property Descrizione() As String
            Get
                Return Me.parentDesc.Text
            End Get
            Set(Value As String)
                Me.parentDesc.Text = Value
            End Set
        End Property

        Public Property CampoCodice() As String
            Get
                Return _codice
            End Get
            Set(Value As String)
                _codice = Value
            End Set
        End Property

        Public Property CampoDescrizione() As String
            Get
                Return _descrizione
            End Get
            Set(Value As String)
                _descrizione = Value
            End Set
        End Property

        Public Property Tabella() As String
            Get
                Return _tabella
            End Get
            Set(Value As String)
                _tabella = Value
            End Set
        End Property
        Public Property MieiCNS As Boolean
            Get
                Return _MieiCNS
            End Get
            Set(Value As Boolean)
                _MieiCNS = Value
            End Set
        End Property

        Public Property Filtro() As String
            Get
                Return _filtro
            End Get
            Set(Value As String)
                _filtro = Value
            End Set
        End Property

        Public Property PostBackOnSelect() As Boolean
            Get
                Return _PostBackOnSelect
            End Get
            Set(Value As Boolean)
                _PostBackOnSelect = Value
            End Set
        End Property

        Public Property Tutti() As Boolean
            Get
                Return _Tutti
            End Get
            Set(Value As Boolean)
                _Tutti = Value
            End Set
        End Property

        Private Property queryString() As String
            Get
                Return Session(Me.Display.ClientID & "_queryString")
            End Get
            Set(Value As String)
                Session(Me.Display.ClientID & "_queryString") = Value
            End Set
        End Property

#End Region

        Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

            Ajax.Utility.RegisterTypeForAjax(GetType(OnVac.Common.Controls.AjaxList))

            Dim str As String

            If Not Filtro Is Nothing AndAlso Filtro <> "" Then
                str = String.Format("select {0} as descrizione,{1} as codice from {2} where {3} like :codice and {4} like :descrizione and {5}", Me.CampoDescrizione, Me.CampoCodice, Me.Tabella, Me.CampoCodice, Me.CampoDescrizione, Me.Filtro)
            Else
                str = String.Format("select {0} as descrizione,{1} as codice from {2} where {3} like :codice and {4} like :descrizione", Me.CampoDescrizione, Me.CampoCodice, Me.Tabella, Me.CampoCodice, Me.CampoDescrizione)
            End If
            If MieiCNS Then
                str = String.Format(" {0} and exists(select 1 from T_ANA_LINK_UTENTI_CONSULTORI where LUC_UTE_ID = {1} and LUC_CNS_CODICE = CNS_CODICE)", str, OnVacContext.UserId)
            End If


            Dim crypto As New Crypto(Providers.Rijndael)
            queryString = crypto.Encrypt(str)

            Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), "OnVac_AjaxList", Page.Request.ApplicationPath & "/Common/scripts/AjaxList.js")

        End Sub

        Private Function getOracleConnection(appId As String, aziCodice As String) As OracleClient.OracleConnection
            ' non posso usare il context perchè non c'è la session
            Dim app As Onit.Shared.Manager.Apps.App = Onit.Shared.Manager.Apps.App.getInstance(appId, aziCodice)
            Dim cn As Onit.Shared.NTier.Dal.DAAB.DbConnectionInfo = app.getConnectionInfo()
            Return New OracleClient.OracleConnection(cn.ConnectionString)
        End Function

        <Ajax.AjaxMethod()>
        Public Function getTable(cod As String, desc As String, codClientId As String, descClientId As String, parentClientId As String, PostBackOnSelect As Boolean, querystr As String, label As String, Tutti As Boolean, appID As String, aziCodice As String) As String

            Dim result As String
            Dim dt As New DataTable()

            Dim crypto As New Crypto(Providers.Rijndael)
            Dim query As String = crypto.Decrypt(querystr)

            ' Encoding contro XSS attack
            Me.codClientId = Encoder.HtmlEncode(codClientId)
            Me.descClientId = Encoder.HtmlEncode(descClientId)
            Me.parentClientId = Encoder.HtmlEncode(parentClientId)
            Me.Label = Encoder.HtmlEncode(label)

            Me.PostBackOnSelect = PostBackOnSelect
            Me.Tutti = Tutti

            _app_id = appID

            cod = cod.ToUpper
            desc = desc.ToUpper

            dtParent = New System.Web.UI.WebControls.DataGrid

            Try

                Using oraConn As OracleClient.OracleConnection = getOracleConnection(appID, aziCodice)

                    Using cmd As New OracleClient.OracleCommand(query, oraConn)

                        cmd.Parameters.AddWithValue(":codice", cod & "%")
                        cmd.Parameters.AddWithValue(":descrizione", desc & "%")

                        Using dad As New OracleClient.OracleDataAdapter(cmd)
                            dad.Fill(dt)
                        End Using

                        If Tutti <> Nothing AndAlso Tutti = True Then
                            Dim drow As DataRow
                            drow = dt.NewRow
                            drow("DESCRIZIONE") = Constants.AmbulatorioTUTTI.Descrizione
                            drow("CODICE") = Constants.AmbulatorioTUTTI.Codice
                            dt.Rows.Add(drow)
                            dt.AcceptChanges()
                        End If

                    End Using

                End Using

            Catch ex As Exception

                Return ex.ToString()

            End Try

            Dim ajaxRender As New Renderator()

            result = ajaxRender.ExportToHtml(dtParent, dt, Renderator.Type.table)

            Return result

        End Function

        Protected Sub dtParent_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dtParent.ItemDataBound

            Dim l As ListItemType = e.Item.ItemType
            Dim btn As LinkButton

            If l = ListItemType.Item Or l = ListItemType.AlternatingItem Then

                btn = CType(e.Item.Cells(0).Controls(0), LinkButton)

                If Not btn Is Nothing Then
                    e.Item.Attributes("oldClass") = ""
                    e.Item.Attributes("onclick") = String.Format("rowSelected(this,{0},'{1}','{2}');", parentClientId, codClientId, descClientId)
                    If PostBackOnSelect <> Nothing AndAlso PostBackOnSelect = True Then
                        e.Item.Attributes("onclick") &= String.Format("__doPostBack('selected','{0}|{1}|{2}');", Label, e.Item.DataItem.Row.ItemArray(1).ToString().Replace("'", "\'"), e.Item.DataItem.Row.ItemArray(0).ToString().Replace("'", "\'"))
                    End If

                End If

            End If

        End Sub

        Private Sub parentDesc_PreRender(sender As Object, e As System.EventArgs) Handles parentDesc.PreRender, parentCod.PreRender

            DirectCast(sender, System.Web.UI.WebControls.TextBox).Attributes("onkeyup") =
                String.Format("parentTable_OnTextChanged('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                              Display.ClientID, parentCod.ClientID, parentDesc.ClientID, PostBackOnSelect, queryString, Label, Tutti, OnVacContext.AppId, OnVacContext.Azienda)

        End Sub

        Public Overrides Sub databind()
            Me.parentDesc.Text = Descrizione
            Me.parentCod.Text = Codice
        End Sub

        Private Sub Display_PreRender(sender As Object, e As System.EventArgs) Handles Display.PreRender

            Page.ClientScript.RegisterStartupScript(
                Me.GetType(), Display.ClientID,
                String.Format("<script type=""text/javascript"">parentTable_OnTextChanged('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')</script>",
                Display.ClientID, parentCod.ClientID, parentDesc.ClientID, PostBackOnSelect, queryString, Label, Tutti, OnVacContext.AppId, OnVacContext.Azienda))

        End Sub

        Public Function IsValid() As Boolean
            Return True
        End Function

        Private Sub btnImgPulisciFiltri_PreRender(sender As Object, e As System.EventArgs) Handles btnImgPulisciFiltri.PreRender

            btnImgPulisciFiltri.Attributes("onclick") = String.Format("pulisciFiltri(this,{0},'{1}','{2}'); ", Display.ClientID, parentCod.ClientID, parentDesc.ClientID)
            btnImgPulisciFiltri.Attributes("onclick") += String.Format("parentTable_OnTextChanged('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                Display.ClientID, parentCod.ClientID, parentDesc.ClientID, PostBackOnSelect, queryString, Label, Tutti, OnVacContext.AppId, OnVacContext.Azienda)

        End Sub

    End Class

End Namespace