Imports System.Collections.Generic
Imports Onit.Database.DataAccessManager


Partial Class InsAssociazione
    Inherits Common.UserControlFinestraModalePageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(sender As System.Object, e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

#Region " Types "

    Public Class ConfermaEventArgs
        Inherits EventArgs

        Public Esito As Boolean
        Public VaccinazioniSelezionate As DataTable

    End Class

    Public Enum DgrAssColumnIndex
        CheckBox = 0
        DescrizioneAssociazione = 1
        CodiceAssociazione = 2
        ViaSomministrazione = 3
        SitoInoculazione = 4
        ViaSomministrazioneDefault = 5
        SitoInoculazioneDefault = 6
    End Enum

#End Region

#Region " Events "

    Public Event Conferma(sender As Object, confermaEventArgs As ConfermaEventArgs)

    Private Sub OnConferma(confermaEventArgs As ConfermaEventArgs)
        RaiseEvent Conferma(Me, confermaEventArgs)
    End Sub

    Public Event Annulla(sender As Object, e As EventArgs)

    Private Sub OnAnnulla(e As EventArgs)
        RaiseEvent Annulla(Me, e)
    End Sub

#End Region

#Region " Properties "

    Public Property DataConvocazione() As DateTime
        Get
            Return ViewState("DC")
        End Get
        Set(Value As DateTime)
            ViewState("DC") = Value
        End Set
    End Property

    Public Property CnsCodice() As String
        Get
            Return ViewState("CnsCodice")
        End Get
        Set(Value As String)
            ViewState("CnsCodice") = Value
        End Set
    End Property

    Public Property CodiciVaccinazioniProgrammateConvocazione() As String()
        Get
            Return ViewState("CVPC")
        End Get
        Set(Value As String())
            ViewState("CVPC") = Value
        End Set
    End Property

    Public Property ShowInfoSomministrazione As Boolean
        Get
            If ViewState("InfoSomm") Is Nothing Then ViewState("InfoSomm") = True
            Return DirectCast(ViewState("InfoSomm"), Boolean)
        End Get
        Set(value As Boolean)
            ViewState("InfoSomm") = value
        End Set
    End Property

#End Region

#Region " Overrides "

    Public Overrides Sub LoadModale()
        '--
        ' Caricamento associazioni da inserire. Vengono escluse dall'elenco:
        ' - le escluse non scadute (comprese le vac sostitute)
        ' - le programmate (comprese le vac sostitute)
        ' - quelle presenti nell'array Me.CodiciVaccinazioniProgrammateConvocazione
        '--
        Dim dtAss As DataTable = Nothing
        '--
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            '--
            ' Prendo i settings del cns di convocazione, solo se il cns è diverso da quello corrente
            Dim _settings As Settings.Settings = Nothing
            If OnVacUtility.Variabili.CNS.Codice = Me.CnsCodice Then
                _settings = Me.Settings
            Else
                _settings = LoadSettingsFromDb(Me.CnsCodice)
            End If
            '--
            Using bizAssociazioni As New Biz.BizAssociazioni(genericProvider, _settings, OnVacContext.CreateBizContextInfos())
                '--
                dtAss = bizAssociazioni.GetDtAssociazioniInseribiliInConvocazione(OnVacUtility.Variabili.PazId, Me.DataConvocazione,
                                                                                  Me.CnsCodice, Me.CodiciVaccinazioniProgrammateConvocazione)
                '--
            End Using
            '--
        End Using
        '--
        If Not Me.ShowInfoSomministrazione Then
            '--
            Me.dg_ass.Columns(DgrAssColumnIndex.SitoInoculazione).HeaderText = String.Empty
            Me.dg_ass.Columns(DgrAssColumnIndex.ViaSomministrazione).HeaderText = String.Empty
            '--
            Me.dg_ass.Columns(DgrAssColumnIndex.DescrizioneAssociazione).HeaderStyle.Width = Unit.Percentage(55)
            Me.dg_ass.Columns(DgrAssColumnIndex.CodiceAssociazione).HeaderStyle.Width = Unit.Percentage(44)
            Me.dg_ass.Columns(DgrAssColumnIndex.SitoInoculazione).HeaderStyle.Width = Unit.Percentage(0)
            Me.dg_ass.Columns(DgrAssColumnIndex.ViaSomministrazione).HeaderStyle.Width = Unit.Percentage(0)
            '--
        End If
        '--
        Me.dg_ass.DataSource = dtAss
        Me.dg_ass.DataBind()
        '--
    End Sub

#End Region

#Region " Eventi Datagrid "

    Private Sub dg_ass_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dg_ass.ItemDataBound

        Select Case e.Item.ItemType

            Case ListItemType.Item, ListItemType.AlternatingItem, ListItemType.EditItem, ListItemType.SelectedItem

                ' Visibilità campi sito inoculazione e via di somministrazione
                Dim fm As Onit.Controls.OnitModalList =
                    DirectCast(e.Item.FindControl("fmSito"), Onit.Controls.OnitModalList)

                If Not fm Is Nothing Then
                    fm.Visible = Me.ShowInfoSomministrazione
                End If

                fm = DirectCast(e.Item.FindControl("fmVia"), Onit.Controls.OnitModalList)

                If Not fm Is Nothing Then
                    fm.Visible = Me.ShowInfoSomministrazione
                End If

        End Select

    End Sub

#End Region

#Region " Toolbar Events "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case (e.Button.Key)

            Case "btn_Conferma"

                Me.ConfermaAssociazioni()

            Case "btn_Annulla"

                Me.OnAnnulla(EventArgs.Empty)

        End Select

    End Sub

#End Region

#Region " Private "

    Private Sub ConfermaAssociazioni()
        '--
        Dim confermaEventArgs As New ConfermaEventArgs()
        confermaEventArgs.VaccinazioniSelezionate = Me.GetVaccinazioniSelezionate()
        confermaEventArgs.Esito = True
        '--
        Dim messaggio As New System.Text.StringBuilder()
        '--
        Using dam As IDAM = OnVacUtility.OpenDam()
            '--
            ' controlla che non ci siano duplicati
            Dim codiciVaccinazioniSelezionatiTemp As New List(Of String)()
            Dim esisteVaccinazioneDuplicata As Boolean = False
            '--
            For Each vaccinazioneSelezionata As DataRow In confermaEventArgs.VaccinazioniSelezionate.Rows
                '--
                Dim codiceVaccinazioneSelezionata As String = DirectCast(vaccinazioneSelezionata("VAC_CODICE"), String)
                '--
                If Not codiciVaccinazioniSelezionatiTemp.Contains(codiceVaccinazioneSelezionata) Then
                    ' --- Controllo Associabilità vaccinazioni da inserire con vaccinazioni già presenti --- '
                    If Me.Settings.CTRL_ASSOCIABILITA_VAC Then
                        '--
                        If Not Me.CodiciVaccinazioniProgrammateConvocazione Is Nothing AndAlso Me.CodiciVaccinazioniProgrammateConvocazione.Count > 0 Then
                            '--
                            Dim ctrlAssociabilita As New Associabilita.ControlloAssociabilita(dam.Provider, dam.Connection, dam.Transaction)
                            '--
                            For i As Integer = 0 To Me.CodiciVaccinazioniProgrammateConvocazione.Count - 1
                                '--
                                If Not ctrlAssociabilita.VaccinazioniAssociabili(codiceVaccinazioneSelezionata, Me.CodiciVaccinazioniProgrammateConvocazione(i)) Then
                                    '--
                                    ' La vaccinazione non è associabile: mostro un messaggio di non associabilità
                                    '--
                                    messaggio.Append("Impossibile eseguire l'operazione !!!\nNon tutte le vaccinazioni selezionate sono associabili con quelle già presenti.\nNessuna associazione inserita.")
                                    '--
                                    confermaEventArgs.Esito = False
                                    '--
                                    Exit For
                                    '--
                                End If
                                '--
                            Next
                            '--
                            If Not confermaEventArgs.Esito Then Exit For
                            '--
                        End If
                        '--
                    End If
                    '--      
                    codiciVaccinazioniSelezionatiTemp.Add(codiceVaccinazioneSelezionata)
                    '--
                Else
                    '--  
                    messaggio.Append("Impossibile eseguire l'operazione !!!\nNon è possibile selezionare associazioni diverse con vaccinazioni in comune.")
                    '-- 
                    confermaEventArgs.Esito = False
                    '-- 
                End If
                '--
                If Not confermaEventArgs.Esito Then Exit For
                '--
            Next
            '--
        End Using
        '--
        If messaggio.Length > 0 Then
            Me.OnitLayout31.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(messaggio.ToString(), "MSG", False, False))
        End If
        '--
        Me.OnConferma(confermaEventArgs)
        '--
    End Sub

    ''' <summary>
    ''' Restituisce un datatable con le informazioni sulle vaccinazioni relative alle associazioni selezionate.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetVaccinazioniSelezionate() As DataTable
        '--
        Dim dtVaccinazioniSelezionate As New DataTable()
        '--
        ' Creazione della lista dei codici delle associazioni selezionate
        Dim listDatiSomministrazioneAssociazioniSelezionate As New List(Of Biz.BizAssociazioni.DatiSomministrazioneAssociazione)()
        '--
        For i As Integer = 0 To Me.dg_ass.Items.Count - 1
            '--
            If DirectCast(dg_ass.Items.Item(i).FindControl("cb"), CheckBox).Checked Then
                '--
                Dim codiceAssociazione As String = HttpUtility.HtmlDecode(dg_ass.Items(i).Cells(DgrAssColumnIndex.CodiceAssociazione).Text).Trim()
                '--
                Dim infoSomministrazione As New Entities.InfoSomministrazione()
                '--
                If Me.ShowInfoSomministrazione Then
                    '--
                    Dim fmSito As Onit.Controls.OnitModalList = DirectCast(dg_ass.Items(i).FindControl("fmSito"), Onit.Controls.OnitModalList)
                    If Not fmSito Is Nothing Then
                        '--
                        infoSomministrazione.CodiceSitoInoculazione = fmSito.Codice
                        infoSomministrazione.DescrizioneSitoInoculazione = fmSito.Descrizione
                        infoSomministrazione.FlagTipoValorizzazioneSito = GetFlagValorizzazione(HttpUtility.HtmlDecode(dg_ass.Items(i).Cells(DgrAssColumnIndex.SitoInoculazioneDefault).Text).Trim(), fmSito.Codice)
                        '--
                    End If
                    '--
                    Dim fmVia As Onit.Controls.OnitModalList = DirectCast(dg_ass.Items(i).FindControl("fmVia"), Onit.Controls.OnitModalList)
                    If Not fmVia Is Nothing Then
                        infoSomministrazione.CodiceViaSomministrazione = fmVia.Codice
                        infoSomministrazione.DescrizioneViaSomministrazione = fmVia.Descrizione
                        infoSomministrazione.FlagTipoValorizzazioneVia = GetFlagValorizzazione(HttpUtility.HtmlDecode(dg_ass.Items(i).Cells(DgrAssColumnIndex.ViaSomministrazioneDefault).Text).Trim(), fmVia.Codice)
                    End If
                    '--
                End If
                '--
                listDatiSomministrazioneAssociazioniSelezionate.Add(
                    New Biz.BizAssociazioni.DatiSomministrazioneAssociazione(codiceAssociazione, infoSomministrazione))
                '--
            End If
            '--
        Next
        '--
        ' Recupero le vaccinazioni che compongono le associazioni
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAssociazioni As New Biz.BizAssociazioni(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())
                '--
                dtVaccinazioniSelezionate = bizAssociazioni.GetDtVaccinazioniAssociazioni(listDatiSomministrazioneAssociazioniSelezionate)
                '--
            End Using
        End Using
        '--
        Return dtVaccinazioniSelezionate
        '--
    End Function

    Private Function GetFlagValorizzazione(valoreDefault As String, valoreSelezionato As String) As String

        If String.IsNullOrEmpty(valoreSelezionato) Then
            Return String.Empty
        End If

        If Not String.IsNullOrEmpty(valoreDefault) AndAlso valoreDefault = valoreSelezionato Then
            Return Constants.TipoValorizzazioneSitoVia.DaAssociazione
        End If

        Return Constants.TipoValorizzazioneSitoVia.Manuale

    End Function

#End Region

End Class

