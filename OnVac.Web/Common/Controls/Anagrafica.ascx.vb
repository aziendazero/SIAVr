Imports System.Collections.Generic

Namespace Common.Controls

    Partial Class Anagrafica
        Inherits Common.UserControlPageBase

#Region "Types"
        Public Enum TipoAnagrafica
            Farmaco
            IndicazioniFarmaco
        End Enum
        Private Enum dgrRicercaColumnIndex As Int16
            SelectorColumn = 0
            Codice = 1
            Descrizione = 2
            CodiceEsterno = 3
            Obsoleto = 4
        End Enum


#End Region
#Region " Consts "

        Private Const COLOR_LIGHTGRAY As String = "LightGray"
        Private Const COLOR_WHITE As String = "White"
        Private Const COLOR_YELLOW As String = "LightYellow"

#End Region
#Region " Codice generato da Progettazione Web Form "

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

#Region " Properties "
        Public Property Label() As String
            Get
                If ViewState("Label") Is Nothing Then Return ""
                Return ViewState("Label")
            End Get
            Set(Value As String)
                ViewState("Label") = Value
            End Set
        End Property

        Public Property CodiceAnagrafica() As String
            Get
                If ViewState("CodiceAnagrafica") Is Nothing Then Return ""
                Return ViewState("CodiceAnagrafica")
            End Get
            Set(Value As String)
                ViewState("CodiceAnagrafica") = Value
            End Set
        End Property



        Public Property TipiAnagrafica() As TipoAnagrafica
            Get
                If ViewState("tipoAnagrafica") Is Nothing Then ViewState("tipoAnagrafica") = TipoAnagrafica.IndicazioniFarmaco
                Return DirectCast(ViewState("tipoAnagrafica"), TipoAnagrafica)
            End Get
            Set(value As TipoAnagrafica)
                ViewState("tipoAnagrafica") = value
            End Set
        End Property

        Private ReadOnly Property UrlCancella As String
            Get
                Return "~/images/Pulisci.gif"
            End Get
        End Property



        Private ReadOnly Property UrlCancellaDisabled As String
            Get
                Return "~/images/Pulisci_dis.gif"
            End Get
        End Property






#End Region

#Region " Overrides "

        Protected Overrides Sub OnInit(e As EventArgs)

            If Not IsPostBack Then
                LoadStatiAnagrafici()
            End If

        End Sub

        Protected Overrides Sub OnPreRender(e As System.EventArgs)

            Me.lblAnagrafe.Text = Label
            Me.lblAnagrafe.ToolTip = Label

        End Sub

#End Region

#Region " Public "





        Public Function CreateStringFieldToCheck(description As String, maxLength As Integer, isRequired As Boolean, idText As String) As Entities.FieldToCheck(Of String)

            Dim fieldToCheck As New Entities.FieldToCheck(Of String)()

            fieldToCheck.Value = lblAnagrafe.Text
            fieldToCheck.Description = String.Format("{0} - {1}", idText, description)
            fieldToCheck.MaxLength = maxLength
            fieldToCheck.Required = isRequired

            Return fieldToCheck

        End Function

        Public Sub SetTextBoxLayoutStringa(isReadOnly As Boolean, isRequired As Boolean)

            If isReadOnly Then
                SetEnable(False)
                tdAnagrafica.BgColor = COLOR_LIGHTGRAY
            Else
                SetEnable(True)
                If isRequired Then
                    lblAnagrafe.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Stringa_Obbligatorio
                    tdAnagrafica.BgColor = COLOR_YELLOW
                Else
                    'lblAnagrafe.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Stringa
                    tdAnagrafica.BgColor = COLOR_WHITE
                End If
            End If

        End Sub

        ''' <summary>
        ''' Caricamento Stati Anagrafici dalla tabella t_ana_stati_anagrafici su db.
        ''' Selezione degli stati anagrafici in base alla lista specificata. Se la lista è nulla o vuota, la selezione avviene in base al flag san_chiamata.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub LoadStatiAnagrafici()

            BindStatiAnagrafici("")

        End Sub

#End Region

#Region " Button Events "

        Protected Sub btnAnnullaAnagrafica_Click(sender As Object, e As System.EventArgs) Handles btnAnnullaAnagrafica.Click

            Label = lblAnagrafe.Text
            CodiceAnagrafica = lblCodice.Text
            Me.fmAnagrafici.VisibileMD = False

        End Sub

        Protected Sub btnConfermaAnagrafica_Click(sender As Object, e As System.EventArgs) Handles btnConfermaAnagrafica.Click

            If dgrRicerca1.SelectedIndex > -1 Then
                Dim codice As String = HttpUtility.HtmlDecode(dgrRicerca1.SelectedItem.Cells(dgrRicercaColumnIndex.Codice).Text)
                Dim descrizione As String = HttpUtility.HtmlDecode(dgrRicerca1.SelectedItem.Cells(dgrRicercaColumnIndex.Descrizione).Text)
                lblCodice.Text = codice
                lblAnagrafe.Text = descrizione
                CodiceAnagrafica = codice
                Label = descrizione
            End If

            Me.fmAnagrafici.VisibileMD = False

        End Sub
        Protected Sub cerca_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cerca.Click
            BindStatiAnagrafici(txtFiltro.Text.ToUpper())


        End Sub


        Private Sub btnAggiungi2_Click(sender As Object, e As EventArgs) Handles btnAggiungi2.Click
            txtFiltro.Text = ""
            dgrRicerca1.SelectedIndex = -1

            BindStatiAnagrafici(txtFiltro.Text.ToUpper())
            Me.fmAnagrafici.VisibileMD = True

        End Sub
        Private Sub btnCancalla_Click(sender As Object, e As EventArgs) Handles btnCancalla.Click
            lblCodice.Text = ""
            lblAnagrafe.Text = ""
            CodiceAnagrafica = ""
            Label = ""
        End Sub




#End Region

#Region " Private "


        Private Sub SetEnable(enable As Boolean)
            btnAggiungi2.Enabled = enable
            btnCancalla.Enabled = enable
            btnCancalla.ImageUrl = IIf(enable, UrlCancella, UrlCancellaDisabled)
        End Sub

        Private Function BindStatiAnagrafici(filter As String) As DataTable

            Dim dt As DataTable = Nothing
            Dim llAnagrafica As New List(Of Entities.AnagrafeCodiceDescrizione)


            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizAnagrafica As New Biz.BizAnagrafiche(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())
                    llAnagrafica = bizAnagrafica.GetListAnagrafica("", TipiAnagrafica, filter)
                    If Not String.IsNullOrWhiteSpace(bizAnagrafica.ERRORMESSAGE) Then
                        Throw New Exception(bizAnagrafica.ERRORMESSAGE)
                    End If
                End Using


            End Using


            dgrRicerca1.DataSource = llAnagrafica
            dgrRicerca1.DataBind()

            Return dt

        End Function

        Private Sub Anagrafica_Load(sender As Object, e As EventArgs) Handles Me.Load
            Page.SetFocus(txtFiltro)
            Dim sTitolo As String = "Anagrafica farmaci"
            If TipiAnagrafica = TipoAnagrafica.IndicazioniFarmaco Then
                sTitolo = "Anagrafica indicazioni"
            End If
            fmAnagrafici.Title = String.Format("<div style=" + """font-family:'Microsoft Sans Serif';text-align:center; font-size: 11pt;padding-bottom:2px"">{0}</div>", sTitolo)
        End Sub





#End Region

    End Class

End Namespace
