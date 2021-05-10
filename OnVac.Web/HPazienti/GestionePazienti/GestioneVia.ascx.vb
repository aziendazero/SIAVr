Imports Onit.Database.DataAccessManager

Partial Class GestioneVia
    Inherits Common.UserControlPageBase

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

    Protected Enum IndexCampiModalListVia As Integer
        CodiceVia = 0
        DescrizioneVia = 1
        CodiceCircoscrizione = 2
        DescrizioneCircoscrizione = 3
        Cap = 4
    End Enum

    Public Class ShowDatiIndirizzoEventArgs
        Inherits EventArgs

        Public TipologiaIndirizzo As Enumerators.TipoIndirizzo

        Public Sub New(tipoIndirizzo As Enumerators.TipoIndirizzo)
            Me.TipologiaIndirizzo = tipoIndirizzo
        End Sub

    End Class

#End Region

#Region " Properties "

    Public Property PazCodice() As String
        Get
            Return ViewState("PazCodice")
        End Get
        Set(Value As String)
            ViewState("PazCodice") = Value
        End Set
    End Property

    Public Property PazCircoscrizioneDescrizione() As String
        Get
            If ViewState("PazCircDescr") Is Nothing Then ViewState("PazCircDescr") = String.Empty
            Return ViewState("PazCircDescr")
        End Get
        Set(Value As String)
            ViewState("PazCircDescr") = Value
        End Set
    End Property

    Public Property PazCircoscrizioneCodice() As String
        Get
            If ViewState("PazCircCod") Is Nothing Then ViewState("PazCircCod") = String.Empty
            Return ViewState("PazCircCod")
        End Get
        Set(Value As String)
            ViewState("PazCircCod") = Value
        End Set
    End Property

    Public Property ViaCentrale() As String
        Get
            Return ViewState("ViaCentrale")
        End Get
        Set(Value As String)
            ViewState("ViaCentrale") = Value
        End Set
    End Property

    Public Property ValoreVecchioResidenza() As String
        Get
            Return ViewState("ValoreVecchioResidenza")
        End Get
        Set(Value As String)
            ViewState("ValoreVecchioResidenza") = Value
        End Set
    End Property

    Public Property ValoreVecchioDomicilio() As String
        Get
            Return ViewState("ValoreVecchioDomicilio")
        End Get
        Set(Value As String)
            ViewState("ValoreVecchioDomicilio") = Value
        End Set
    End Property

    Public Property InCentrale() As Boolean
        Get
            Return ViewState("InCentrale")
        End Get
        Set(Value As Boolean)
            ViewState("InCentrale") = Value
            Abilitazione()
        End Set
    End Property

    Public Property Enabled() As Boolean
        Get
            Return ViewState("Enabled")
        End Get
        Set(Value As Boolean)
            ViewState("Enabled") = Value
            Abilitazione()
        End Set
    End Property

    Public ReadOnly Property GestioneVie() As Boolean
        Get
            If Me.InCentrale Then Return False
            Return Me.Settings.GESVIE
        End Get
    End Property

    Public Property IndirizzoCorrente() As Entities.IndirizzoPaziente
        Get
            Return ViewState("IndirizzoCorrente")
        End Get
        Set(Value As Entities.IndirizzoPaziente)
            ViewState("IndirizzoCorrente") = Value
        End Set
    End Property

    Public ReadOnly Property txtIndirizzo() As TextBox
        Get
            Return Me.txtVia
        End Get
    End Property

    Public Property Tipo() As Enumerators.TipoIndirizzo
        Get
            Return ViewState("Tipo")
        End Get
        Set(Value As Enumerators.TipoIndirizzo)
            ViewState("Tipo") = Value
        End Set
    End Property

    Public ReadOnly Property AggiornaCirByVia() As String
        Get
            Return Me.Settings.AGGIORNACIRBYVIA.ToString().ToLower()
        End Get
    End Property

#End Region

#Region " Eventi "

    Public Event ConfermaModificaCircoscrizione(sender As Object, e As EventArgs)
    Public Event ShowDatiIndirizzo(sender As Object, e As ShowDatiIndirizzoEventArgs)

#End Region

#Region " Page "

    Protected Overrides Sub OnInit(e As EventArgs)

        MyBase.OnInit(e)
        Me.uwtImpostaIndirizzo.ClientSideEvents.Click = "ToolBarClickIndirizzo" & Me.ofmImpostaIndirizzo.ClientID

    End Sub

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        Select Case Request.Form("__EVENTTARGET")

            Case "CambiaCircoscrizione_residenza"

                If IndirizzoCorrente.Tipo = Enumerators.TipoIndirizzo.Residenza Then

                    Me.ValoreVecchioResidenza = IndirizzoCorrente.Libero
                    Me.CambiaCircoscrizione()

                End If

            Case "CambiaCircoscrizione_domicilio"

                If IndirizzoCorrente.Tipo = Enumerators.TipoIndirizzo.Domicilio Then

                    Me.ValoreVecchioDomicilio = IndirizzoCorrente.Libero
                    Me.CambiaCircoscrizione()

                End If

        End Select

    End Sub

#End Region

#Region " ModalList "

    Private Sub omlVia_SetUpFiletr(sender As Object) Handles omlVia.SetUpFiletr

        Me.omlVia.Filtro = " VIA_CIRCOSCRIZIONE = CIR_CODICE (+)"

    End Sub

#End Region

#Region " Toolbar "

    Private Sub uwtImpostaIndirizzo_ButtonClicked(sender As System.Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles uwtImpostaIndirizzo.ButtonClicked

        Select Case be.Button.Key

            Case "btnConferma"

                If IndirizzoCorrente.Tipo = Enumerators.TipoIndirizzo.Residenza Then
                    Me.ValoreVecchioResidenza = Me.IndirizzoCorrente.Libero
                Else
                    Me.ValoreVecchioDomicilio = Me.IndirizzoCorrente.Libero
                End If

                Me.ImpostaDatiIndirizzo()

                Me.IndirizzoCorrente.IsModified = True

                Me.ofmImpostaIndirizzo.VisibileMD = False

                Me.LoadVia(IndirizzoCorrente)

            Case "btnAnnulla"

                Me.ofmImpostaIndirizzo.VisibileMD = False

        End Select

    End Sub

#End Region

#Region " Public "

    Public Sub Disabilita(disabilita As Boolean)

        Me.txtVia.Enabled = Not disabilita
        Me.txtVia.CssClass = "textbox_stringa" + IIf(disabilita, "_disabilitato", "")
        Me.btnIndicazioniVia.Enabled = Not disabilita
        Me.ofmImpostaIndirizzo.Enabled = Not disabilita

    End Sub

    Public Function CaricaVia(dam As IDAM) As Entities.IndirizzoPaziente

        Dim ind As Entities.IndirizzoPaziente
        Dim dta As New DataTable()

        dam.QB.NewQuery()
        dam.QB.AddTables("T_PAZ_INDIRIZZI, T_PAZ_PAZIENTI, T_ANA_VIE")
        dam.QB.AddSelectFields("*")
        If Tipo = Enumerators.TipoIndirizzo.Residenza Then
            dam.QB.AddWhereCondition("PAZ_IND_CODICE_RES", Comparatori.Uguale, "IND_CODICE", DataTypes.OutJoinLeft)
        Else
            dam.QB.AddWhereCondition("PAZ_IND_CODICE_DOM", Comparatori.Uguale, "IND_CODICE", DataTypes.OutJoinLeft)
        End If
        dam.QB.AddWhereCondition("PAZ_CODICE", Comparatori.Uguale, PazCodice, DataTypes.Numero)
        dam.QB.AddWhereCondition("VIA_CODICE", Comparatori.Uguale, "IND_VIA_CODICE", DataTypes.OutJoinRight)

        dam.BuildDataTable(dta)

        If dta.Rows.Count > 0 Then

            Dim cod As Object

            If Tipo = Enumerators.TipoIndirizzo.Residenza Then
                cod = dta.Rows(0)("PAZ_IND_CODICE_RES")
            Else
                cod = dta.Rows(0)("PAZ_IND_CODICE_DOM")
            End If

            If Not cod Is DBNull.Value And GestioneVie Then

                ind = New Entities.IndirizzoPaziente(dta.Rows(0)("IND_CODICE").ToString, PazCodice)

                ind.NCivico = dta.Rows(0)("IND_N_CIVICO").ToString
                ind.Interno = dta.Rows(0)("IND_INTERNO").ToString
                ind.Lotto = dta.Rows(0)("IND_LOTTO").ToString
                ind.Palazzina = dta.Rows(0)("IND_PALAZZINA").ToString
                ind.Scala = dta.Rows(0)("IND_SCALA").ToString
                ind.Piano = dta.Rows(0)("IND_PIANO").ToString
                ind.CivicoLettera = dta.Rows(0)("IND_CIVICO_LETTERA").ToString

                ind.Via.Codice = dta.Rows(0)("VIA_CODICE").ToString
                ind.Via.Descrizione = dta.Rows(0)("VIA_DESCRIZIONE").ToString

                If Tipo = Enumerators.TipoIndirizzo.Residenza Then
                    ind.Libero = dta.Rows(0)("PAZ_INDIRIZZO_RESIDENZA").ToString
                Else
                    ind.Libero = dta.Rows(0)("PAZ_INDIRIZZO_DOMICILIO").ToString
                End If

            Else

                ind = New Entities.IndirizzoPaziente(-1, PazCodice)

                ind.Via.Codice = ""
                ind.Via.Descrizione = ""
                ind.CivicoLettera = ""
                ind.Interno = ""
                ind.Lotto = ""
                ind.NCivico = ""
                ind.Palazzina = ""
                ind.Piano = ""
                ind.Scala = ""

                If Tipo = Enumerators.TipoIndirizzo.Residenza Then
                    If dta.Rows(0)("PAZ_INDIRIZZO_RESIDENZA").ToString.Trim <> "" Then
                        ind.Manuale = True
                        ind.Libero = dta.Rows(0)("PAZ_INDIRIZZO_RESIDENZA").ToString
                    End If
                Else
                    If dta.Rows(0)("PAZ_INDIRIZZO_DOMICILIO").ToString.Trim <> "" Then
                        ind.Manuale = True
                        ind.Libero = dta.Rows(0)("PAZ_INDIRIZZO_DOMICILIO").ToString
                    End If
                End If

            End If

        Else

            ind = New Entities.IndirizzoPaziente()

        End If

        ind.Tipo = Tipo

        'Controllo successivo se gestione in centrale abilitata
        If InCentrale Then
            ind.Manuale = True
            ind.Libero = ViaCentrale
        End If

        Return ind

    End Function

    Public Sub LoadViaDB(dam As IDAM)

        Dim ind As Entities.IndirizzoPaziente = CaricaVia(dam)

        IndirizzoCorrente = ind

        LoadVia(IndirizzoCorrente)

    End Sub

    Public Sub LoadVia(indirizzoPaziente As Entities.IndirizzoPaziente)

        If Not indirizzoPaziente Is Nothing Then

            If Tipo = Enumerators.TipoIndirizzo.Residenza Then
                If indirizzoPaziente.Manuale Then
                    chkManuale.Checked = True
                End If
            Else
                If indirizzoPaziente.Manuale Then
                    chkManuale.Checked = True
                End If
            End If

            If indirizzoPaziente.IsModified Then
                txtLibero.Text = Me.CreaIndirizzoLibero()
            Else
                txtLibero.Text = indirizzoPaziente.Libero
            End If

            If Not indirizzoPaziente.Via.Codice Is Nothing Then
                omlVia.Codice = indirizzoPaziente.Via.Codice
                omlVia.Descrizione = indirizzoPaziente.Via.Descrizione
                omlVia.RefreshDataBind()
            End If

            txtCivicoLettera.Text = indirizzoPaziente.CivicoLettera
            txtInterno.Text = indirizzoPaziente.Interno
            txtLotto.Text = indirizzoPaziente.Lotto
            txtNumeroCivico.Text = indirizzoPaziente.NCivico
            txtPalazzina.Text = indirizzoPaziente.Palazzina
            txtPiano.Text = indirizzoPaziente.Piano
            txtScala.Text = indirizzoPaziente.Scala

            If indirizzoPaziente.Manuale Then
                txtVia.Text = indirizzoPaziente.Libero
                txtLibero.Text = indirizzoPaziente.Libero
            Else
                If indirizzoPaziente.Via.Codice <> "" Then
                    txtVia.Text = indirizzoPaziente.Via.Descrizione & ", " & indirizzoPaziente.NCivico & IIf(indirizzoPaziente.CivicoLettera = "", "", "/" & indirizzoPaziente.CivicoLettera) & IIf(indirizzoPaziente.Interno = "", "", ", Int:" & indirizzoPaziente.Interno) & IIf(indirizzoPaziente.Lotto = "", "", ", Lotto:" & indirizzoPaziente.Lotto) & IIf(indirizzoPaziente.Palazzina = "", "", ", Palaz.:" & indirizzoPaziente.Palazzina) & IIf(indirizzoPaziente.Scala = "", "", ", Scala:" & indirizzoPaziente.Scala) & IIf(indirizzoPaziente.Piano = "", "", ", Piano:" & indirizzoPaziente.Piano)
                End If
            End If

        End If

    End Sub

    Public Sub SaveVia(dam As IDAM)

        Me.ImpostaDatiIndirizzo()

        Using genericProvider As New DAL.DbGenericProvider(dam)

            Using bizVia As New Biz.BizVia(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                bizVia.SaveIndirizzoPaziente(Me.IndirizzoCorrente)

            End Using

        End Using

    End Sub

#End Region

#Region " Private "

    Private Sub CambiaCircoscrizione()

        Me.ImpostaDatiIndirizzo()

        Me.IndirizzoCorrente.IsModified = True

        Me.ofmImpostaIndirizzo.VisibileMD = False

        Me.LoadVia(IndirizzoCorrente)

        Me.PazCircoscrizioneCodice = Me.omlVia.GetAltriCampi(IndexCampiModalListVia.CodiceCircoscrizione)
        Me.PazCircoscrizioneDescrizione = Me.omlVia.GetAltriCampi(IndexCampiModalListVia.DescrizioneCircoscrizione)

        RaiseEvent ConfermaModificaCircoscrizione(Me, EventArgs.Empty)

        Me.ofmImpostaIndirizzo.VisibileMD = False

    End Sub

    Private Sub Abilitazione()

        If Me.Enabled Then

            Me.btnIndicazioniVia.Enabled = True

            If Me.GestioneVie Then
                Me.btnIndicazioniVia.Visible = True
                Me.txtVia.ReadOnly = True
            Else
                Me.btnIndicazioniVia.Visible = False
                Me.txtVia.ReadOnly = False
                Me.txtVia.CssClass = "TextBox_Stringa"
            End If

        Else

            If Not Me.GestioneVie Then
                Me.btnIndicazioniVia.Visible = False
            End If

            Me.btnIndicazioniVia.Enabled = False
            Me.txtVia.ReadOnly = True
            Me.txtVia.CssClass = "TextBox_Stringa_Disabilitato"

        End If

    End Sub

    Private Sub ImpostaDatiIndirizzo()

        Me.IndirizzoCorrente.Manuale = True
        Me.IndirizzoCorrente.Via.Codice = String.Empty

        If Not Me.GestioneVie Then
            Me.IndirizzoCorrente.Libero = Me.txtVia.Text.Trim()
        Else
            If Me.chkManuale.Checked Then
                Me.IndirizzoCorrente.Libero = Me.txtLibero.Text.Trim()
            Else
                Me.IndirizzoCorrente.Manuale = False
                Me.IndirizzoCorrente.Via.Codice = Me.omlVia.Codice
                Me.IndirizzoCorrente.Via.Descrizione = Me.omlVia.Descrizione
                Me.IndirizzoCorrente.CivicoLettera = Me.txtCivicoLettera.Text
                Me.IndirizzoCorrente.Interno = Me.txtInterno.Text
                Me.IndirizzoCorrente.Lotto = Me.txtLotto.Text
                Me.IndirizzoCorrente.NCivico = Me.txtNumeroCivico.Text
                Me.IndirizzoCorrente.Palazzina = Me.txtPalazzina.Text
                Me.IndirizzoCorrente.Piano = Me.txtPiano.Text
                Me.IndirizzoCorrente.Scala = Me.txtScala.Text
                Me.IndirizzoCorrente.Libero = Me.CreaIndirizzoLibero()
            End If
        End If

    End Sub

    Private Function CreaIndirizzoLibero()

        Dim indirizzo As New System.Text.StringBuilder()

        indirizzo.AppendFormat("{0}, {1}", Me.IndirizzoCorrente.Via.Descrizione, Me.IndirizzoCorrente.NCivico)

        If Not String.IsNullOrEmpty(Me.IndirizzoCorrente.CivicoLettera) Then
            indirizzo.AppendFormat("/{0}", Me.IndirizzoCorrente.CivicoLettera)
        End If

        If Not String.IsNullOrEmpty(Me.IndirizzoCorrente.Interno) Then
            indirizzo.AppendFormat(", Int:", Me.IndirizzoCorrente.Interno)
        End If

        If Not String.IsNullOrEmpty(Me.IndirizzoCorrente.Lotto) Then
            indirizzo.AppendFormat(", Lotto:", Me.IndirizzoCorrente.Lotto)
        End If

        If Not String.IsNullOrEmpty(Me.IndirizzoCorrente.Palazzina) Then
            indirizzo.AppendFormat(", Palaz:", Me.IndirizzoCorrente.Palazzina)
        End If

        If Not String.IsNullOrEmpty(Me.IndirizzoCorrente.Scala) Then
            indirizzo.AppendFormat(", Scala:", Me.IndirizzoCorrente.Scala)
        End If

        If Not String.IsNullOrEmpty(Me.IndirizzoCorrente.Piano) Then
            indirizzo.AppendFormat(", Piano:", Me.IndirizzoCorrente.Piano)
        End If

        Return indirizzo.ToString()

    End Function

#End Region

    Private Sub btnIndicazioniVia_Click(sender As Object, e As System.EventArgs) Handles btnIndicazioniVia.Click

        RaiseEvent ShowDatiIndirizzo(Me, New ShowDatiIndirizzoEventArgs(Me.Tipo))
        Me.ofmImpostaIndirizzo.VisibileMD = True

    End Sub

End Class
