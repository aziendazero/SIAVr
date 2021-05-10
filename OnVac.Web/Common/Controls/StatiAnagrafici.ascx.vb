Imports System.Collections.Generic

Namespace Common.Controls

    Partial Class StatiAnagrafici
        Inherits Common.UserControlPageBase

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

        Public Property ShowLabel() As Boolean
            Get
                If ViewState("ShowLabel") Is Nothing Then Return True
                Return Convert.ToBoolean(ViewState("ShowLabel"))
            End Get
            Set(Value As Boolean)
                ViewState("ShowLabel") = Value
            End Set
        End Property

        Public Property ListStatiAnagraficiDaSelezionare As List(Of Enumerators.StatoAnagrafico)
            Get
                If ViewState("StatiAnagSel") Is Nothing Then ViewState("StatiAnagSel") = New List(Of Enumerators.StatoAnagrafico)
                Return ViewState("StatiAnagSel")
            End Get
            Set(value As List(Of Enumerators.StatoAnagrafico))
                ViewState("StatiAnagSel") = value
                Me.LoadStatiAnagrafici()
            End Set
        End Property

        ' Memorizza i check selezionati, da ripristinare in caso di annullo
        Private Property StatiAnagrafici() As List(Of String)
            Get
                If ViewState("StatiAnagrafici") Is Nothing Then
                    ViewState("StatiAnagrafici") = New List(Of String)()
                End If
                Return DirectCast(ViewState("StatiAnagrafici"), List(Of String))
            End Get
            Set(Value As List(Of String))
                ViewState("StatiAnagrafici") = Value
            End Set
        End Property

        Public ReadOnly Property Items() As ListItemCollection
            Get
                Return Me.chlStatiAnagrafici.Items
            End Get
        End Property

        Public ReadOnly Property SelectedItemsCount() As Integer
            Get
                Return Me.chlStatiAnagrafici.SelectedItems.Count
            End Get
        End Property

        Public Property Enabled() As Boolean
            Get
                If ViewState("StatiAnagraficiEnabled") Is Nothing Then ViewState("StatiAnagraficiEnabled") = True
                Return Convert.ToBoolean(ViewState("StatiAnagraficiEnabled"))
            End Get
            Set(value As Boolean)
                ViewState("StatiAnagraficiEnabled") = value
                SetEnable(value)
            End Set
        End Property

        Private ReadOnly Property UrlFiltroStatiAnagrafici As String
            Get
                Return "~/images/filtro_statianag.gif"
            End Get
        End Property

        Private ReadOnly Property UrlFiltroStatiAnagraficiHover As String
            Get
                Return "~/images/filtro_statianag_hov.gif"
            End Get
        End Property

        Private ReadOnly Property UrlFiltroStatiAnagraficiDisabled As String
            Get
                Return "~/images/filtro_statianag_dis.gif"
            End Get
        End Property

#End Region

#Region " Overrides "

        Protected Overrides Sub OnInit(e As EventArgs)

            ' N.B. : così funziona; utilizzando la ResolveClientUrl, invece, renderizza il percorso sbagliato!
            Me.btnAggiungiStati.ImageUrl = Me.UrlFiltroStatiAnagrafici

            If String.IsNullOrEmpty(Me.btnAggiungiStati.Attributes.Item("onmouseover")) Then
                Me.btnAggiungiStati.Attributes.Add("onmouseover", String.Format("this.src='{0}'", Me.ResolveClientUrl(Me.UrlFiltroStatiAnagraficiHover)))
            End If

            If String.IsNullOrEmpty(Me.btnAggiungiStati.Attributes.Item("onmouseout")) Then
                Me.btnAggiungiStati.Attributes.Add("onmouseout", String.Format("this.src='{0}'", Me.ResolveClientUrl(Me.UrlFiltroStatiAnagrafici)))
            End If

            If Not IsPostBack Then
                LoadStatiAnagrafici()
            End If

        End Sub

        Protected Overrides Sub OnPreRender(e As System.EventArgs)

            Me.tdLabel.Visible = Me.ShowLabel
            Me.lblStatoAnagrafico.Text = Me.DescrizioneStatiAnagrafici()
            Me.lblStatoAnagrafico.ToolTip = Me.GetSelectedDescriptions()

        End Sub

#End Region

#Region " Public "

        ''' <summary>
        ''' Restituisce le descrizioni degli stati anagrafici selezionati separate da ";".
        ''' Se non è stato selezionato niente, restituisce la stringa vuota.
        ''' </summary>
        Public Function GetSelectedDescriptions() As String

            Dim listDescrizioniSelezionate As String() = (From listItem As ListItem In Me.chlStatiAnagrafici.Items
                                                          Where listItem.Selected
                                                          Select listItem.Text).ToArray()

            Return String.Join(";", listDescrizioniSelezionate)

        End Function

        ''' <summary>
        ''' Restituisce una stringa con tutti i codici degli stati anagrafici selezionati, separati da virgole (senza apici).
        ''' Se non è stato selezionato niente, restituisce la stringa vuota.
        ''' </summary>
        Public Function GetSelectedValues() As String

            Dim listCodiciSelezionati As String() = (From listItem As ListItem In Me.chlStatiAnagrafici.Items
                                                     Where listItem.Selected
                                                     Select listItem.Value).ToArray()

            Return String.Join(",", listCodiciSelezionati)

        End Function

        ''' <summary>
        ''' Restituisce una stringa con i codici degli stati anagrafici selezionati, tra apici e separati da virgole.
        ''' Se non è stato selezionato niente, restituisce la stringa vuota.
        ''' </summary>
        Public Function GetSelectedValuesForQuery() As String

            Dim codiciSelezionati As String() = (From listItem As ListItem In Me.chlStatiAnagrafici.Items
                                                 Where listItem.Selected
                                                 Select "'" + listItem.Value + "'").ToArray()

            Return String.Join(",", codiciSelezionati)

        End Function

        ''' <summary>
        ''' Restituisce una lista di stringhe con i codici degli stati anagrafici selezionati.
        ''' </summary>
        Public Function GetListStatiAnagraficiSelezionati() As List(Of String)

            Return Me.chlStatiAnagrafici.SelectedValues.ToList()

        End Function

        ''' <summary>
        ''' Restituisce un array contenente gli stati anagrafici selezionati.
        ''' </summary>
        Public Function GetStatiAnagraficiSelezionati() As Enumerators.StatoAnagrafico()

            Dim listStatiAnagrafici As New List(Of Enumerators.StatoAnagrafico)()

            For Each selectedValue As String In Me.chlStatiAnagrafici.SelectedValues.ToList()

                listStatiAnagrafici.Add([Enum].Parse(GetType(Enumerators.StatoAnagrafico), selectedValue))

            Next

            Return listStatiAnagrafici.ToArray()

        End Function

        ''' <summary>
        ''' Imposta la selezione sugli stati anagrafici specificati nella lista di codici specificata.
        ''' Se la lista è nulla o vuota, cancella tutti gli elementi selezionati dalla checklist.
        ''' </summary>
        Public Sub SetStatiAnagrafici(listCodiciStatiAnagrafici As List(Of String))

            Me.chlStatiAnagrafici.ClearSelection()
            Me.StatiAnagrafici.Clear()

            If listCodiciStatiAnagrafici Is Nothing OrElse listCodiciStatiAnagrafici.Count = 0 Then Return

            For Each codice As String In listCodiciStatiAnagrafici

                Dim listItem As ListItem = Me.chlStatiAnagrafici.Items.FindByValue(codice)

                If Not listItem Is Nothing Then

                    listItem.Selected = True
                    Me.StatiAnagrafici.Add(codice)

                End If

            Next

        End Sub

        ''' <summary>
        ''' Caricamento Stati Anagrafici dalla tabella t_ana_stati_anagrafici su db.
        ''' Selezione degli stati anagrafici in base alla lista specificata. Se la lista è nulla o vuota, la selezione avviene in base al flag san_chiamata.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub LoadStatiAnagrafici()

            SetStatiAnagrafici(Nothing)

            If Me.ListStatiAnagraficiDaSelezionare.Count = 0 Then

                ' Caricamento stati anagrafici da db
                Dim dt As DataTable = BindStatiAnagrafici()

                ' Selezione in base al valore del flag su db
                For i As Integer = 0 To dt.Rows.Count - 1

                    If dt.Rows(i)("SAN_CHIAMATA").ToString() = "S" Then
                        SelezionaStatoAnagrafico(dt.Rows(i)("SAN_CODICE").ToString())
                    End If

                Next

            Else

                ' Caricamento stati anagrafici solo se non sono già stati caricati
                If Me.chlStatiAnagrafici.Items.Count = 0 Then
                    BindStatiAnagrafici()
                End If

                ' Selezione tramite lista stati anagrafici
                For i As Integer = 0 To Me.chlStatiAnagrafici.Items.Count - 1

                    If Me.ListStatiAnagraficiDaSelezionare.Contains(Me.chlStatiAnagrafici.Items(i).Value) Then
                        SelezionaStatoAnagrafico(Me.chlStatiAnagrafici.Items(i).Value)
                    End If

                Next

            End If

        End Sub

#End Region

#Region " Button Events "

        Protected Sub btnAggiungiStati_Click(sender As System.Object, e As System.Web.UI.ImageClickEventArgs) Handles btnAggiungiStati.Click

            Me.fmStatiAnagrafici.VisibileMD = True

        End Sub

        Protected Sub btnAnnullaSelezionaStati_Click(sender As Object, e As System.EventArgs) Handles btnAnnullaSelezionaStati.Click

            ' Reimposto i check in base a quelli selezionati in precedenza 
            '(e confermati: se ho premuto annulla, non devo mantenere i check ma ripristinare quelli precedenti)
            If Not IsNothing(Me.StatiAnagrafici) Then

                For i As Integer = 0 To Me.chlStatiAnagrafici.Items.Count - 1

                    If Me.StatiAnagrafici.Contains(Me.chlStatiAnagrafici.Items(i).Value) Then
                        Me.chlStatiAnagrafici.Items(i).Selected = True
                    End If

                Next

            End If

            Me.fmStatiAnagrafici.VisibileMD = False

        End Sub

        Protected Sub btnConfermaSelezionaStati_Click(sender As Object, e As System.EventArgs) Handles btnConfermaSelezionaStati.Click

            Me.StatiAnagrafici.Clear()

            For i As Integer = 0 To Me.chlStatiAnagrafici.Items.Count - 1

                If Me.chlStatiAnagrafici.Items(i).Selected Then
                    Me.StatiAnagrafici.Add(Me.chlStatiAnagrafici.Items(i).Value)
                End If

            Next

            Me.fmStatiAnagrafici.VisibileMD = False

        End Sub

#End Region

#Region " Private "

        ' Restituisce la descrizione degli stati anagrafici selezionati (modifica 24/01/2005)
        ' Utilizzata anche nell'aspx per il databind della label con gli stati selezionati (21.11.2006)
        Private Function DescrizioneStatiAnagrafici() As String

            Dim statoFiltro As New System.Text.StringBuilder
            If Not Me.chlStatiAnagrafici.SelectedItems Is Nothing Then
                For count As Integer = 0 To Me.chlStatiAnagrafici.SelectedItems.Count - 1
                    statoFiltro.AppendFormat("{0}, ", Me.chlStatiAnagrafici.SelectedItems(count).Text)
                Next
                If statoFiltro.Length > 0 Then statoFiltro.Remove(statoFiltro.Length - 2, 2)

            End If

            Return statoFiltro.ToString()

        End Function

        Private Sub SetEnable(enable As Boolean)
            Me.btnAggiungiStati.Enabled = enable
            Me.btnAggiungiStati.ImageUrl = IIf(enable, Me.UrlFiltroStatiAnagrafici, UrlFiltroStatiAnagraficiDisabled)
        End Sub

        Private Function BindStatiAnagrafici() As DataTable

            Dim dt As DataTable = Nothing

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizStatiAnagrafici As New Biz.BizStatiAnagrafici(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                    dt = bizStatiAnagrafici.LeggiStatiAnagrafici()

                    If Not String.IsNullOrWhiteSpace(bizStatiAnagrafici.Message) Then
                        Throw New Exception(bizStatiAnagrafici.Message)
                    End If

                End Using
            End Using

                Me.chlStatiAnagrafici.Items.Clear()
                Me.chlStatiAnagrafici.DataTextField = "SAN_DESCRIZIONE"
                Me.chlStatiAnagrafici.DataValueField = "SAN_CODICE"
                Me.chlStatiAnagrafici.DataSource = dt
                Me.chlStatiAnagrafici.DataBind()

                Return dt

        End Function

        Private Sub SelezionaStatoAnagrafico(codiceStatoAnagrafico As String)

            ' Aggiorno l'arraylist degli stati anagrafici selezionati
            Me.StatiAnagrafici.Add(codiceStatoAnagrafico)

            ' Seleziono il check corrispondente nella checklist
            Dim selectedItem As ListItem = Me.chlStatiAnagrafici.Items.FindByValue(codiceStatoAnagrafico)
            If Not selectedItem Is Nothing Then selectedItem.Selected = True

        End Sub

#End Region

    End Class

End Namespace
