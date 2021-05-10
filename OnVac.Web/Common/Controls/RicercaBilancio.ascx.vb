Imports System.Collections.Generic

''' <summary>
''' Controllo per la scelta di un bilancio di una malattia
''' </summary>
''' <remarks> Attenzione, questo controllo può filtrare su un determinato paziente in base al valore
''' di OnVacUtility.Variabili.PazId (mostrando solo i bilanci non ancora compilati per quel paziente).
''' Se si vuole vedere tutti i bilanci valorizzare la proprietà SoloBilanciNonCompilatiPaziente a false dall'aspx che
''' include questo comando (es: <uc1:RicercaBilancio id="ucRicBil" filtraPazCodice="False" runat="server"></uc1:RicercaBilancio>).
''' </remarks>
Partial Class RicercaBilancio
    Inherits Common.UserControlFinestraModalePageBase

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

#Region " Types "

    Private Enum DgrBilanciColumnIndex
        BtnSelect = 0
        NumeroBilancio = 1
        DescrizioneBilancio = 2
        EtaMinima = 3
        EtaMassima = 4
    End Enum

#End Region

#Region " Events "

    Event ReturnBilancio(configurazioneBilancioSelezionato As Entities.BilancioAnagrafica)

    Event AnnullaBilancio()

    Event Pulisci()

#End Region

#Region " Properties "

    Public Property SoloBilanciNonCompilatiPaziente() As Boolean
        Get
            If ViewState("Bil") Is Nothing Then ViewState("Bil") = True
            Return Convert.ToBoolean(ViewState("Bil"))
        End Get
        Set(Value As Boolean)
            ViewState("Bil") = Value
        End Set
    End Property

    Public Property IncludiNessunaMalattia() As Boolean
        Get
            If (ViewState("IncludiNessunaMalattia") Is Nothing) Then Return False
            Return Convert.ToBoolean(ViewState("IncludiNessunaMalattia"))
        End Get
        Set(Value As Boolean)
            ViewState("IncludiNessunaMalattia") = Value
        End Set
    End Property

    Protected ReadOnly Property UrlIconaSelezione As String
        Get
            Return "~/images/sel.gif"
        End Get
    End Property

    Public Property SoloTipologieMalattiaPerCompilazioneBilanci() As Boolean
        Get
            If ViewState("TIPOLOGIE") Is Nothing Then Return False
            Return Convert.ToBoolean(ViewState("TIPOLOGIE"))
        End Get
        Set(value As Boolean)
            ViewState("TIPOLOGIE") = value
        End Set
    End Property

#End Region

#Region " Public Methods "

    Public Overrides Sub LoadModale()

        Response.Expires = -1

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizMalattie As New Biz.BizMalattie(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                Dim list As List(Of Biz.BizMalattie.CodiceDescrizioneMalattia) =
                    bizMalattie.GetMalattie(IncludiNessunaMalattia, True, SoloTipologieMalattiaPerCompilazioneBilanci)

                Me.cmbMalattie.DataSource = list
                Me.cmbMalattie.DataBind()

            End Using
        End Using

        If Me.cmbMalattie.Items.FindByValue(Me.Settings.CODNOMAL) IsNot Nothing Then
            Me.cmbMalattie.SelectedValue = Me.Settings.CODNOMAL
        Else
            If Me.cmbMalattie.Items.Count > 0 Then
                Me.cmbMalattie.SelectedIndex = 0
            End If
        End If

        Me.cmbMalattie_SelectedIndexChanged(cmbMalattie, Nothing)
        Me.cmbMalattie.Visible = True

        If Me.dgrBilanci.Items.Count > 0 Then
            Me.dgrBilanci.SelectedIndex = 0
        Else
            Me.dgrBilanci.SelectedIndex = -1
        End If

    End Sub

#End Region

#Region " Protected "

    Protected Function GetStringEta(campo As Object) As String

        If Not campo Is Nothing AndAlso Not campo Is DBNull.Value Then

            Dim eta As New Entities.Eta(campo)

            Return String.Format("{0} anni {1} mesi {2} giorni", eta.Anni, eta.Mesi, eta.Giorni)

        End If

        Return "-"

    End Function

#End Region

#Region " DropDownList Events "

    Private Sub cmbMalattie_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cmbMalattie.SelectedIndexChanged

        Dim list As List(Of Entities.BilancioAnagrafica) = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAnaBilanci As New Biz.BizAnaBilanci(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                If Me.SoloBilanciNonCompilatiPaziente Then
                    list = bizAnaBilanci.GetAnagraficaBilanciNonCompilatiPazienteByMalattia(Convert.ToInt64(OnVacUtility.Variabili.PazId), Me.cmbMalattie.SelectedValue)
                Else
                    list = bizAnaBilanci.GetAnagraficaBilanciByMalattia(Me.cmbMalattie.SelectedValue)
                End If

            End Using
        End Using

        Me.dgrBilanci.SelectedIndex = -1
        Me.dgrBilanci.DataSource = list
        Me.dgrBilanci.DataBind()

        If Me.dgrBilanci.Items.Count > 0 Then
            Me.dgrBilanci.SelectedIndex = 0
        End If

    End Sub

#End Region

#Region " Toolbar Events "

    Private Sub ToolBar_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar.ButtonClicked

        Select Case e.Button.Key

            Case "btnConferma"

                If Not Me.dgrBilanci.SelectedItem Is Nothing Then

                    Dim bilancioSelezionato As Entities.BilancioAnagrafica = Nothing

                    Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                        Using bizAnaBilanci As New Biz.BizAnaBilanci(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())

                            bilancioSelezionato = bizAnaBilanci.GetAnagraficaBilancio(
                                Convert.ToInt32(Me.dgrBilanci.SelectedItem.Cells(DgrBilanciColumnIndex.NumeroBilancio).Text),
                                Me.cmbMalattie.SelectedItem.Value)

                        End Using
                    End Using

                    RaiseEvent ReturnBilancio(bilancioSelezionato)

                Else
                    RaiseEvent AnnullaBilancio()
                End If

            Case "btnAnnulla"

                RaiseEvent AnnullaBilancio()
                RaiseEvent Pulisci()

        End Select

    End Sub

#End Region

End Class
