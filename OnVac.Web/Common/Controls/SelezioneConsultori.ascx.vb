Imports System.Collections.Generic

Public Class SelezioneConsultori
    Inherits Onit.OnAssistnet.OnVac.Common.UserControlPageBase

#Region " Events "

    Public Event OnSelectionError(errorMessage As String)

    Private Sub SelectionError(errorMessage As String)

        RaiseEvent OnSelectionError(errorMessage)

    End Sub

#End Region

#Region " Properties "

    '''<summary>
    ''' Numero massimo di consultori da visualizzare nell'elenco dei risultati.
    ''' Impostare a 0 per non limitare la lista.
    '''</summary>
    Public Property MaxCnsInListaSelezionati() As Integer
        Get
            If ViewState("MaxCnsInListaSelezionati") Is Nothing Then Return 0
            Return ViewState("MaxCnsInListaSelezionati")
        End Get
        Set(value As Integer)
            ViewState("MaxCnsInListaSelezionati") = value
        End Set
    End Property

    '''<summary>
    ''' Flag che indica se deve essere selezionato di default il consultorio corrente.
    '''</summary>
    Public Property ImpostaCnsCorrente() As Boolean
        Get
            If ViewState("ImpostaCnsCorrente") Is Nothing Then Return True
            Return ViewState("ImpostaCnsCorrente")
        End Get
        Set(value As Boolean)
            ViewState("ImpostaCnsCorrente") = value
        End Set
    End Property

    '''<summary>
    ''' Flag che indica se è possibile selezionare più di un consultorio dall'elenco.
    '''</summary>
    Public Property SelezioneMultipla() As Boolean
        Get
            If ViewState("SelezioneMultipla") Is Nothing Then Return True
            Return ViewState("SelezioneMultipla")
        End Get
        Set(value As Boolean)
            ViewState("SelezioneMultipla") = value
        End Set
    End Property

    '''<summary>
    ''' Flag che indica se deve essere visualizzato il pulsante di apertura della modale dei consultori.
    ''' Default TRUE
    '''</summary>
    Public Property MostraPulsanteSelezione() As Boolean
        Get
            If ViewState("MostraPulsanteSelezione") Is Nothing Then Return True
            Return ViewState("MostraPulsanteSelezione")
        End Get
        Set(value As Boolean)
            ViewState("MostraPulsanteSelezione") = value
        End Set
    End Property

    '''<summary>
    ''' Stringa dei codici selezionati, separati da "|"
    '''</summary>
    Private Property CodiciSelezionati() As String
        Get
            If ViewState("CodiciSelezionati") Is Nothing Then Return String.Empty
            Return ViewState("CodiciSelezionati").ToString()
        End Get
        Set(ByVal value As String)
            ViewState("CodiciSelezionati") = value
        End Set
    End Property

    ''' <summary>
    ''' Default TRUE
    ''' </summary>
    ''' <returns></returns>
    Public Property MostraSoloAperti() As Boolean
        Get
            If ViewState("MostraSoloAperti") Is Nothing Then Return True
            Return ViewState("MostraSoloAperti").ToString()
        End Get
        Set(ByVal value As Boolean)
            ViewState("MostraSoloAperti") = value
        End Set
    End Property

    ''' <summary>
    ''' Default TRUE
    ''' </summary>
    ''' <returns></returns>
    Public Property MostraCnsUtente() As Boolean
        Get
            If ViewState("MostraCnsUtente") Is Nothing Then Return True
            Return ViewState("MostraCnsUtente").ToString()
        End Get
        Set(ByVal value As Boolean)
            ViewState("MostraCnsUtente") = value
        End Set
    End Property

    Public Property FiltroDistretto() As String
		Get
			If ViewState("FiltroDistretto") Is Nothing Then Return String.Empty
			Return ViewState("FiltroDistretto").ToString()
		End Get
		Set(ByVal value As String)
			ViewState("FiltroDistretto") = value
		End Set
	End Property

    ''' <summary>
    ''' Default FALSE
    ''' </summary>
    ''' <returns></returns>
    Public Property MostraSoloCnsUslCorrente() As Boolean
        Get
            If ViewState("MostraSoloCnsUslCorrente") Is Nothing Then ViewState("MostraSoloCnsUslCorrente") = False
            Return Convert.ToBoolean(ViewState("MostraSoloCnsUslCorrente"))
        End Get
        Set(value As Boolean)
            ViewState("MostraSoloCnsUslCorrente") = value
        End Set
    End Property

#End Region

#Region " Control Events "

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init

        If Not IsPostBack Then

            LoadGetCodici()

        End If

    End Sub

    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

        ' Visualizzazione Button di selezione dei consultori in base al valore della proprietà
        Me.btnSelezionaCns.Visible = Me.MostraPulsanteSelezione

        If Not Me.MostraPulsanteSelezione Then
            tdBtnSelezione.Width = "0px"
            tdLblConsultori.Width = "100%"
        End If

        ' Visualizzazione CheckBox "Tutti" in base al valore della proprietà
        Me.chkTutti.Visible = Me.SelezioneMultipla

        ' Aggiornamento label descrizioni consultori selezionati
        Me.lblConsultori.Text = Me.GetDescrizioneConsultoriSelezionati()

    End Sub

#End Region

#Region " Public Methods "

    ''' <summary>
    ''' Restituisce una lista di stringhe con i codici dei consultori selezionati.
    ''' </summary>
    Public Function GetConsultoriSelezionati() As List(Of String)

        Return chklConsultori.SelectedValues

    End Function

    ''' <summary>
    ''' Restituisce una stringa con i codici dei consultori selezionati.
    ''' Se il parametro sqlFormat vale true, la stringa viene restituita in un formato utilizzabile in una query sql,
    ''' con i codici racchiusi tra apici.
    ''' </summary>
    Public Function GetConsultoriSelezionati(sqlFormat As Boolean) As String

        Dim selectedValues As Onit.OnAssistnet.Web.UI.WebControls.CheckBoxList.SelectedValues = Me.chklConsultori.SelectedValues
        Return selectedValues.ToString(sqlFormat)

    End Function

    ''' <summary>
    ''' Imposta la selezione sui consultori specificati nella stringa passata. 
    ''' I codici devono essere separati dal delimitatore "|".
    ''' </summary>
    Public Sub SelezionaConsultori(codiciConsultori As String)

        chklConsultori.ClearSelection()

        If String.IsNullOrEmpty(codiciConsultori) Then Return

        Dim arrayCodiciConsultori As String() = codiciConsultori.Split("|")

        For Each codice As String In arrayCodiciConsultori

            Dim listItem As ListItem = chklConsultori.Items.FindByValue(codice)

            If Not listItem Is Nothing Then

                listItem.Selected = True

            End If

        Next

        ' Memorizzazione selezione, e chiusura finestra modale
        CodiciSelezionati = GetCodiceConsultoriSelezionati()

    End Sub

#End Region

#Region " Private Methods "

	' Caricamento consultori. La descrizione di ogni consultorio viene impostata nel formato "descrizione [codice]".
	Private Function LoadConsultori() As List(Of Entities.ConsultorioAperti)

		Dim listConsultori As List(Of Entities.ConsultorioAperti)

		Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizConsultori As New Biz.BizConsultori(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Dim uslCorrente As String = String.Empty

                Dim utenteCorrente As Integer = 0

                If MostraCnsUtente Then
                    utenteCorrente = OnVacContext.UserId
                End If

                If MostraSoloCnsUslCorrente Then
                    uslCorrente = OnVacContext.CodiceUslCorrente
                End If

                listConsultori = bizConsultori.GetListCodiceDescrizioneConsultori(MostraSoloAperti, Nothing, utenteCorrente, FiltroDistretto, uslCorrente)

                For Each consultorio As Entities.ConsultorioAperti In listConsultori

                    Dim chiuso As String = String.Empty

                    If Not String.IsNullOrEmpty(consultorio.DescrizioneChiuso) Then
                        chiuso = String.Format("- {0}", consultorio.DescrizioneChiuso)
                    End If

                    consultorio.Descrizione = String.Format("{0} [{1}] {2}", consultorio.Descrizione, consultorio.Codice, chiuso)

                Next

            End Using
        End Using

        chklConsultori.DataSource = listConsultori
        chklConsultori.DataBind()

        Return listConsultori

    End Function

    ' Restituisce i codici dei consultori selezionati, separati da "|"
    Private Function GetCodiceConsultoriSelezionati() As String

        Dim selectedValues As Onit.OnAssistnet.Web.UI.WebControls.CheckBoxList.SelectedValues = Me.chklConsultori.SelectedValues

        If selectedValues Is Nothing OrElse selectedValues.Count = 0 Then
            Return String.Empty
        End If

        Return String.Join("|", selectedValues.ToArray())

    End Function

    Public Sub LoadGetCodici()

        ' Caricamento consultori aperti
        Dim lista As List(Of Entities.ConsultorioAperti) = LoadConsultori()

		' Selezione consultorio corrente in base al valore della proprietà
		If Me.ImpostaCnsCorrente Then

			Dim listItem As ListItem = Me.chklConsultori.Items.FindByValue(OnVacUtility.Variabili.CNS.Codice)

			If Not listItem Is Nothing Then listItem.Selected = True

		End If
		If Not String.IsNullOrWhiteSpace(FiltroDistretto) Then
			Dim listItem As ListItemCollection = chklConsultori.Items
			For Each s As Entities.ConsultorioAperti In lista
				If String.IsNullOrEmpty(s.DescrizioneChiuso) Then
					Dim listItemd As ListItem = Me.chklConsultori.Items.FindByValue(s.Codice)
					If Not listItem Is Nothing Then listItemd.Selected = True
				End If
			Next
		End If

		' Aggiornamento codici consultori selezionati
		Me.CodiciSelezionati = Me.GetCodiceConsultoriSelezionati()
	End Sub

	' Restituisce le descrizioni dei consultori selezionati, separati da ","
	' Se il numero di consultori è superiore al parametro MaxCnsInListaSelezionati, 
	' la stringa viene limitata e vengono aggiunti i puntini in fondo.
	Public Function GetDescrizioneConsultoriSelezionati() As String

		Dim selectedItems As Onit.OnAssistnet.Web.UI.WebControls.CheckBoxList.SelectedItems = Me.chklConsultori.SelectedItems

		If selectedItems Is Nothing OrElse selectedItems.Count = 0 Then
			Return String.Empty
		End If

		Dim descrizioni As List(Of String) = (From items In selectedItems Select items.Text).ToList()

		If Me.MaxCnsInListaSelezionati > 0 And descrizioni.Count > Me.MaxCnsInListaSelezionati Then

			descrizioni.RemoveRange(Me.MaxCnsInListaSelezionati, descrizioni.Count - Me.MaxCnsInListaSelezionati)

			descrizioni.Add("...")

		End If

		Return String.Join(", ", descrizioni.ToArray())

	End Function
	Public Function GetDescrizioneConsultoriSelezionatiPerStampe() As String
		Dim sRet As String
		Dim max As Integer = MaxCnsInListaSelezionati
		MaxCnsInListaSelezionati = 0
		sRet = GetDescrizioneConsultoriSelezionati()
		MaxCnsInListaSelezionati = max


		Return sRet

	End Function
#End Region

#Region " Button Events "

	Protected Sub btnSelezionaCns_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnSelezionaCns.Click

        Me.fmConsultori.VisibileMD = True

    End Sub

    Private Sub btnConfermaSelezione_Click(sender As Object, e As System.EventArgs) Handles btnConfermaSelezione.Click

        Dim erroreSelezione As Boolean = False

        ' Controllo consultori selezionati
        If Not Me.SelezioneMultipla Then

            Dim selectedValues As Onit.OnAssistnet.Web.UI.WebControls.CheckBoxList.SelectedValues = Me.chklConsultori.SelectedValues

            If Not selectedValues Is Nothing AndAlso selectedValues.Count > 1 Then

                erroreSelezione = True

                SelectionError("E' possibile selezionare un solo centro vaccinale!")

            End If

        End If

        If Not erroreSelezione Then

            ' Memorizzazione selezione, e chiusura finestra modale
            Me.CodiciSelezionati = Me.GetCodiceConsultoriSelezionati()

            Me.fmConsultori.VisibileMD = False

        End If

    End Sub

    Private Sub btnAnnullaSelezione_Click(sender As Object, e As System.EventArgs) Handles btnAnnullaSelezione.Click

        ' Ripristina i check precedenti e chiude la finestra modale
        SelezionaConsultori(Me.CodiciSelezionati)

        Me.fmConsultori.VisibileMD = False

    End Sub

#End Region

#Region " CheckBox Events "

    Private Sub chkTutti_CheckedChanged(sender As Object, e As System.EventArgs) Handles chkTutti.CheckedChanged

        If Me.chkTutti.Checked Then
			Me.chklConsultori.SelectAll()
		Else
            Me.chklConsultori.UnselectAll()
        End If

    End Sub



#End Region

End Class