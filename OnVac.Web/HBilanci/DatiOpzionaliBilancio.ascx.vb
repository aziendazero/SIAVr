Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.Entities

Public Class DatiOpzionaliBilancio
    Inherits Common.UserControlPageBase

#Region " Constants "

    ' N.B. : funzionano perchè il foglio di stile è incluso nella pagina contenitore
    Private Class CSS
        Public Const TEXTBOX_DATA As String = "TextBox_Data"
        Public Const TEXTBOX_DATA_DISABILITATO As String = "TextBox_Data_Disabilitato"
        Public Const TEXTBOX_STRINGA As String = "TextBox_Stringa"
        Public Const TEXTBOX_STRINGA_DISABILITATO As String = "TextBox_Stringa_Disabilitato"
        Public Const TEXTBOX_NUMERICO As String = "TextBox_Numerico"
        Public Const TEXTBOX_NUMERICO_DISABILITATO As String = "TextBox_Numerico_Disabilitato"
    End Class

#End Region

#Region " Properties "

    Public Property Enabled As Boolean
        Get
            If ViewState("ENBL") Is Nothing Then ViewState("ENBL") = True
            Return ViewState("ENBL")
        End Get
        Set(value As Boolean)
            ViewState("ENBL") = value
            AbilitaDatiVaccinazioni(value)
            AbilitaDatiViaggio(value)
        End Set
    End Property

    ''' <summary>
    ''' Se vale true i titoli delle sezioni sono maiuscoli
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UseUpperCaseCaption As Boolean
        Get
            If ViewState("UPPER") Is Nothing Then ViewState("UPPER") = False
            Return ViewState("UPPER")
        End Get
        Set(value As Boolean)
            ViewState("UPPER") = value
            If value Then
                Me.lblSezioneVaccinazioni.Text = Me.lblSezioneVaccinazioni.Text.ToUpper()
                Me.lblSezioneViaggio.Text = Me.lblSezioneViaggio.Text.ToUpper()
            End If
        End Set
    End Property


#End Region

#Region " Public "

    Public Class DatiOpzionaliSettings

        Public Property ShowDatiVaccinazioni As Boolean
        Public Property ShowDatiViaggio As Boolean

        Public Property DatiVaccinazioniBilancio As String
        Public Property DatiViaggioCorrente As DatiViaggio
        Public Property DataFollowUpPrevista As Date?
        Public Property DataFollowUpEffettiva As Date?
        Public Property DataConvocazione As DateTime?
        Public Property DatiViaggi As List(Of ViaggioVisita)
        Public Property VisibleDataEffettiva As Boolean

        Public Sub New()
            DatiViaggioCorrente = New DatiViaggio()
            DatiViaggi = New List(Of ViaggioVisita)
        End Sub

    End Class

    Public Class DatiViaggio

        Public Property DataInizioViaggio As DateTime?
        Public Property DataFineViaggio As DateTime?
        Public Property PaeseViaggioCodice As String
        Public Property PaeseViaggioDescrizione As String

    End Class

    Public Sub SetDatiOpzionali(settings As DatiOpzionaliSettings)

        Visible = (settings.ShowDatiVaccinazioni Or settings.ShowDatiViaggio)

        SetLayout(settings.ShowDatiVaccinazioni, settings.ShowDatiViaggio)

        If settings.ShowDatiViaggio Then
            SetDatiViaggio(settings.DatiViaggioCorrente)
            SetDatiViaggioRpt(settings.DatiViaggi)
            SetDateFollowUp(settings.DataFollowUpPrevista, settings.DataFollowUpEffettiva)
            dpkFollowUpEffettiva.Visible = settings.VisibleDataEffettiva
            LabelDataFineFollowUp.Visible = settings.VisibleDataEffettiva
        Else
            ClearDatiViaggio()
            SetDatiViaggioRpt(Nothing)
            SetDateFollowUp(Nothing, Nothing)
        End If

        If settings.ShowDatiVaccinazioni Then
            SetDatiVaccinazioni(settings.DatiVaccinazioniBilancio, settings.DataConvocazione)
        Else
            ClearDatiVaccinazioni()
        End If

    End Sub

    Public Function GetDatiViaggio() As DatiViaggio

        Dim datiViaggio As New DatiViaggio()

        If dypViaggio.Visible Then

            If dpkInizioViaggio.Data > DateTime.MinValue Then
                datiViaggio.DataInizioViaggio = dpkInizioViaggio.Data
            Else
                datiViaggio.DataInizioViaggio = Nothing
            End If

            If dpkFineViaggio.Data > DateTime.MinValue Then
                datiViaggio.DataFineViaggio = Me.dpkFineViaggio.Data
            Else
                datiViaggio.DataFineViaggio = Nothing
            End If

            If String.IsNullOrWhiteSpace(fmPaeseViaggio.Codice) Then
                datiViaggio.PaeseViaggioCodice = String.Empty
                datiViaggio.PaeseViaggioDescrizione = String.Empty
            Else
                datiViaggio.PaeseViaggioCodice = fmPaeseViaggio.Codice
                datiViaggio.PaeseViaggioDescrizione = fmPaeseViaggio.Descrizione
            End If

        End If

        Return datiViaggio

    End Function
    Public Function GetDataFollowUpEffetiva() As Date

        Dim data As Date = DateTime.MinValue
        If dypViaggio.Visible Then
            If dpkFollowUpEffettiva.Data > DateTime.MinValue Then
                data = dpkFollowUpEffettiva.Data
            End If
        End If

        Return data

    End Function
    Public Function GetDataFollowUpPrevista() As Date

        Dim data As Date = DateTime.MinValue

        If dypViaggio.Visible Then
            If dpkFollowUpPrevista.Data > DateTime.MinValue Then
                data = dpkFollowUpPrevista.Data
            End If
        End If

        Return data

    End Function
    Public Function GetListaDatiViaggio() As List(Of ViaggioVisita)
        Dim listaViaggi As New List(Of ViaggioVisita)
        For Index As Integer = 0 To rptViaggi.Items.Count - 1
            Dim lis As New ViaggioVisita
            Dim dpkInizioViaggioRpt As Onit.Web.UI.WebControls.Validators.OnitDatePick = rptViaggi.Items(Index).FindControl("dpkInizioViaggioRpt")
            Dim dpkFineViaggioRpt As Onit.Web.UI.WebControls.Validators.OnitDatePick = rptViaggi.Items(Index).FindControl("dpkFineViaggioRpt")
            Dim fmPaeseViaggioRpt As Onit.Controls.OnitModalList = rptViaggi.Items(Index).FindControl("fmPaeseViaggioRpt")
            Dim idViaggioRpt As Label = rptViaggi.Items(Index).FindControl("idViaggioRpt")
            Dim IdVisitaRpt As Label = rptViaggi.Items(Index).FindControl("IdVisitaRpt")
            Dim OperazioneRpt As Label = rptViaggi.Items(Index).FindControl("OperazioneRpt")
            Dim DeleteViaggi As ImageButton = rptViaggi.Items(Index).FindControl("DeleteViaggi")
            If Not String.IsNullOrWhiteSpace(dpkInizioViaggioRpt.Text) Then
                lis.DataInizioViaggio = dpkInizioViaggioRpt.Text
            End If
            If Not String.IsNullOrWhiteSpace(dpkFineViaggioRpt.Text) Then
                lis.DataFineViaggio = dpkFineViaggioRpt.Text
            End If
            lis.CodicePaese = fmPaeseViaggioRpt.Codice
            lis.DescPaese = fmPaeseViaggioRpt.Descrizione
            lis.Id = idViaggioRpt.Text
            lis.IdVisita = IdVisitaRpt.Text
            If OperazioneRpt.Text = OperazioneViaggio.Delete.ToString() Then
                lis.Operazione = OperazioneViaggio.Delete
            End If
            If OperazioneRpt.Text = OperazioneViaggio.Update.ToString() Then
                lis.Operazione = OperazioneViaggio.Update
            End If
            If OperazioneRpt.Text = OperazioneViaggio.Insert.ToString() Then
                lis.Operazione = OperazioneViaggio.Insert
            End If
            listaViaggi.Add(lis)
        Next
        Return listaViaggi
    End Function

    Public Function GetDatiVaccinazioni() As String

        Dim list As New List(Of String)()

        For Each item As DataListItem In dlsVaccinazioni.Items

            Select Case item.ItemType

                Case ListItemType.Item, ListItemType.SelectedItem, ListItemType.AlternatingItem, ListItemType.EditItem

                    Dim chkVac As CheckBox = DirectCast(item.FindControl("chkVac"), CheckBox)
                    If Not chkVac Is Nothing AndAlso chkVac.Checked Then

                        Dim codice As String = String.Empty
                        Dim dose As Integer = 0

                        Dim txtCodVac As TextBox = DirectCast(item.FindControl("txtCodVac"), TextBox)
                        If Not txtCodVac Is Nothing Then
                            codice = txtCodVac.Text
                        End If

                        Dim txtDose As TextBox = DirectCast(item.FindControl("txtDose"), TextBox)
                        If Not txtDose Is Nothing AndAlso Not String.IsNullOrWhiteSpace(txtDose.Text) Then

                            If Not Integer.TryParse(txtDose.Text, dose) Then
                                dose = 0
                            End If

                        End If

                        If Not String.IsNullOrWhiteSpace(codice) AndAlso dose > 0 Then

                            list.Add(String.Format("{0}|{1}", codice, dose))

                        End If

                    End If

            End Select

        Next

        Dim vaccinazioni As String = String.Empty

        If Not list.IsNullOrEmpty() Then
            vaccinazioni = String.Join(";", list.ToArray())
        End If

        Return vaccinazioni

    End Function

    Public Sub Clear()

        ClearDatiVaccinazioni()
        ClearDatiViaggio()
        ClearListaViaggi()
        ClearDateFollowUp()

    End Sub

    Public Function CheckDosiVaccinazioniSelezionate() As Biz.BizGenericResult

        Dim success As Boolean = True
        Dim messaggioDosiNulle As New System.Text.StringBuilder()
        Dim messaggioDosiNonValide As New System.Text.StringBuilder()

        For Each item As DataListItem In Me.dlsVaccinazioni.Items

            Select Case item.ItemType
                Case ListItemType.Item, ListItemType.SelectedItem, ListItemType.AlternatingItem, ListItemType.EditItem

                    Dim chkVac As CheckBox = DirectCast(item.FindControl("chkVac"), CheckBox)
                    If Not chkVac Is Nothing AndAlso chkVac.Checked Then

                        Dim txtDose As TextBox = DirectCast(item.FindControl("txtDose"), TextBox)
                        If Not txtDose Is Nothing Then

                            If String.IsNullOrWhiteSpace(txtDose.Text) Then

                                success = False
                                messaggioDosiNulle.AppendFormat("{0}\n", chkVac.Text)

                            Else

                                Dim dose As Integer = 0
                                If Not Integer.TryParse(txtDose.Text, dose) OrElse dose <= 0 Then
                                    success = False
                                    messaggioDosiNonValide.AppendFormat("{0}\n", chkVac.Text)
                                End If

                            End If

                        End If
                    End If

            End Select
        Next

        Dim result As New Biz.BizGenericResult()
        result.Success = success

        If messaggioDosiNulle.Length > 0 Then
            messaggioDosiNulle.Insert(0, "Non e\' stata specificata la dose per le vaccinazioni seguenti:\n")
        End If

        If messaggioDosiNonValide.Length > 0 Then
            messaggioDosiNonValide.Insert(0, "La dose specificata non e\' valida per le vaccinazioni seguenti:\n")
        End If

        result.Message = messaggioDosiNulle.ToString() + messaggioDosiNonValide.ToString()

        Return result

    End Function
    Public Function CheckViaggi() As Biz.BizGenericResult
        Dim success As Boolean = True
        Dim errorMsg As New System.Text.StringBuilder()
        If dypViaggio.Visible Then
            Dim viaggi As List(Of ViaggioVisita) = GetListaDatiViaggio()
            Dim viaggiEdit As List(Of ViaggioVisita) = viaggi.Where(Function(p) p.Operazione <> OperazioneViaggio.Delete).OrderBy(Function(p) p.DataInizioViaggio).ThenBy(Function(p) p.DataFineViaggio).ToList()
            'Verifico che ci sia almeno un viaggio
            If viaggiEdit.Count = 0 Then
                errorMsg.Append("Deve esistere almeno un viaggio!\n")
            End If
            ' Controllo se ci sono dati nulli o intervalli delle date sovrapposte
            Dim dataFineApp As Date = Date.MinValue
            For Each v As ViaggioVisita In viaggiEdit
                If v.DataInizioViaggio = Date.MinValue OrElse v.DataFineViaggio = Date.MinValue Then
                    errorMsg.Append("Le Date del viaggio sono\' obbligatorie.\n")
                End If
                If String.IsNullOrWhiteSpace(v.CodicePaese.ToString()) Then
                    errorMsg.Append("Il paese del viaggio e\' obbligatorio.\n")
                End If
                If v.DataInizioViaggio > v.DataFineViaggio Then
                    errorMsg.Append("Data di fine viaggio non puo\' essere minore dell\'inizio.\n")
                End If
                ' Se data inizio del viaggio è minore della data di fine del viaggio precedente,
                ' allora non posso salvare
                If v.DataInizioViaggio <> Date.MinValue Then
                    If v.DataInizioViaggio.Date.AddDays(1) < dataFineApp.Date Then
                        errorMsg.Append("Le date del viaggio si sovrappongono.\n")
                    Else
                        dataFineApp = v.DataFineViaggio
                    End If
                End If

            Next
        End If
        Dim result As New Biz.BizGenericResult()
        result.Success = success
        If errorMsg.Length > 0 Then
            result.Message = String.Format("Impossibile effettuare il salvataggio.\n{0}", errorMsg.ToString())
            result.Success = False
        End If

        Return result
    End Function

#End Region

#Region " Controls Events "

    Private Sub dlsVaccinazioni_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.DataListItemEventArgs) Handles dlsVaccinazioni.ItemDataBound

        Select Case e.Item.ItemType

            Case ListItemType.Item, ListItemType.SelectedItem, ListItemType.AlternatingItem, ListItemType.EditItem

                Dim vaccinazione As Entities.BilancioVaccinazione = DirectCast(e.Item.DataItem, Entities.BilancioVaccinazione)

                Dim chkVac As CheckBox = DirectCast(e.Item.FindControl("chkVac"), CheckBox)

                If Not chkVac Is Nothing Then
                    chkVac.Checked = vaccinazione.Dose.HasValue
                End If

        End Select

    End Sub

#End Region

#Region " Private "


    Private Sub DatiOpzionaliBilancio_Load(sender As Object, e As EventArgs) Handles Me.Load

        If IsPostBack Then
            SetDatiViaggioRepeater()
        End If

    End Sub



    Private Sub SetLayout(showVaccinazioni As Boolean, showViaggio As Boolean)

        dypLabelVaccinazioni.Visible = showVaccinazioni
        dypVaccinazioni.Visible = showVaccinazioni

        If showVaccinazioni Then
            AbilitaDatiVaccinazioni(Enabled)
        End If

        dypViaggio.Visible = showViaggio

        If showViaggio Then
            AbilitaDatiViaggio(Enabled)
        End If

    End Sub

    Private Sub AbilitaDatiVaccinazioni(enabled As Boolean)

        dlsVaccinazioni.Enabled = enabled

        For Each item As DataListItem In dlsVaccinazioni.Items

            Select Case item.ItemType
                Case ListItemType.Item, ListItemType.SelectedItem, ListItemType.AlternatingItem, ListItemType.EditItem

                    Dim chkVac As CheckBox = DirectCast(item.FindControl("chkVac"), CheckBox)
                    If Not chkVac Is Nothing Then
                        chkVac.Enabled = enabled
                    End If

                    Dim txtDose As TextBox = DirectCast(item.FindControl("txtDose"), TextBox)
                    If Not txtDose Is Nothing Then

                        txtDose.Enabled = enabled

                        If enabled Then
                            txtDose.CssClass = CSS.TEXTBOX_NUMERICO
                        Else
                            txtDose.CssClass = CSS.TEXTBOX_NUMERICO_DISABILITATO
                        End If

                    End If

            End Select
        Next

    End Sub

    Private Sub AbilitaDatiViaggio(enabled As Boolean)

        dpkInizioViaggio.Enabled = enabled
        dpkFineViaggio.Enabled = enabled
        fmPaeseViaggio.Enabled = enabled

        dpkFollowUpEffettiva.Enabled = enabled
        dpkFollowUpPrevista.Enabled = enabled

        If enabled Then
            dpkInizioViaggio.CssClass = CSS.TEXTBOX_DATA
            dpkFineViaggio.CssClass = CSS.TEXTBOX_DATA
            fmPaeseViaggio.CssClass = CSS.TEXTBOX_STRINGA

            dpkFollowUpEffettiva.CssClass = CSS.TEXTBOX_DATA
            dpkFollowUpPrevista.CssClass = CSS.TEXTBOX_DATA
        Else
            dpkInizioViaggio.CssClass = CSS.TEXTBOX_DATA_DISABILITATO
            dpkFineViaggio.CssClass = CSS.TEXTBOX_DATA_DISABILITATO
            fmPaeseViaggio.CssClass = CSS.TEXTBOX_STRINGA_DISABILITATO

            dpkFollowUpEffettiva.CssClass = CSS.TEXTBOX_DATA_DISABILITATO
            dpkFollowUpPrevista.CssClass = CSS.TEXTBOX_DATA_DISABILITATO
        End If

    End Sub

    Private Sub SetDatiViaggioRepeater()

        If Not rptViaggi Is Nothing AndAlso rptViaggi.Items.Count > 0 Then
            For Index As Integer = 0 To rptViaggi.Items.Count - 1

                If rptViaggi.Items(Index).ItemType = ListItemType.Item OrElse rptViaggi.Items(Index).ItemType = ListItemType.AlternatingItem Then
                    Dim dpkInizioViaggioRpt As Onit.Web.UI.WebControls.Validators.OnitDatePick = rptViaggi.Items(Index).FindControl("dpkInizioViaggioRpt")
                    Dim dpkFineViaggioRpt As Onit.Web.UI.WebControls.Validators.OnitDatePick = rptViaggi.Items(Index).FindControl("dpkFineViaggioRpt")
                    Dim txtGiorniViaggioRpt As TextBox = rptViaggi.Items(Index).FindControl("txtGiorniViaggioRpt")

                    Dim totGiorni As Integer? = Biz.BizVisite.CalcolaTotaleGiorniViaggio(dpkInizioViaggioRpt.Data, dpkFineViaggioRpt.Data)
                    If totGiorni.HasValue Then
                        txtGiorniViaggioRpt.Text = totGiorni.Value.ToString()
                    Else
                        txtGiorniViaggioRpt.Text = String.Empty
                    End If
                End If

            Next
        End If

    End Sub

    Private Sub SetDatiViaggio(datiViaggioCorrente As DatiViaggio)

        If datiViaggioCorrente Is Nothing Then

            ClearDatiViaggio()

        Else

            If datiViaggioCorrente.DataInizioViaggio.HasValue Then
                dpkInizioViaggio.Data = datiViaggioCorrente.DataInizioViaggio.Value
            Else
                dpkInizioViaggio.Data = DateTime.MinValue
            End If

            If datiViaggioCorrente.DataFineViaggio.HasValue Then
                dpkFineViaggio.Data = datiViaggioCorrente.DataFineViaggio.Value
            Else
                dpkFineViaggio.Data = DateTime.MinValue
            End If

            'If String.IsNullOrWhiteSpace(txtGiorniViaggio.Value) Then
            '    Dim totGiorni As Integer? = Biz.BizVisite.CalcolaTotaleGiorniViaggio(dpkInizioViaggio.Data, dpkFineViaggio.Data)
            '    If totGiorni.HasValue Then
            '        txtGiorniViaggio.Value = totGiorni.Value.ToString()
            '    Else
            '        txtGiorniViaggio.Value = String.Empty
            '    End If
            'End If


            If String.IsNullOrWhiteSpace(datiViaggioCorrente.PaeseViaggioCodice) Then
                fmPaeseViaggio.Codice = String.Empty
                fmPaeseViaggio.Descrizione = String.Empty
            Else
                fmPaeseViaggio.Codice = datiViaggioCorrente.PaeseViaggioCodice
                fmPaeseViaggio.Descrizione = datiViaggioCorrente.PaeseViaggioDescrizione
            End If

            fmPaeseViaggio.RefreshDataBind()

        End If

    End Sub
    Private Sub SetDatiViaggioRpt(listaViaggi As List(Of ViaggioVisita))

        rptViaggi.DataSource = listaViaggi
        rptViaggi.DataBind()

    End Sub
    Private Sub SetDateFollowUp(dataPrevista As Date?, dataEffettiva As Date?)
        If dataEffettiva.HasValue Then
            dpkFollowUpEffettiva.Data = dataEffettiva
        Else
            dpkFollowUpEffettiva.Data = Nothing
        End If
        If dataPrevista.HasValue Then
            dpkFollowUpPrevista.Data = dataPrevista
        Else
            dpkFollowUpPrevista.Data = Nothing
        End If


    End Sub

    Private Sub SetDatiVaccinazioni(vaccinazioniBilancio As String, dataConvocazione As DateTime?)

        ClearDatiVaccinazioni()

        Dim datiVaccinazioni As List(Of Entities.BilancioVaccinazione) = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizVisite As New Biz.BizVisite(genericProvider, Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                datiVaccinazioni = bizVisite.GetVaccinazioniBilancio(OnVacUtility.Variabili.PazId, vaccinazioniBilancio, dataConvocazione)

            End Using
        End Using

        dlsVaccinazioni.DataSource = datiVaccinazioni
        dlsVaccinazioni.DataBind()

        lblSezioneVaccinazioni.Text = "Vaccinazioni"
        lblSezioneVaccinazioni.ToolTip = String.Empty

        If Not datiVaccinazioni.IsNullOrEmpty() Then

            Dim selezionate As IEnumerable(Of Entities.BilancioVaccinazione) = datiVaccinazioni.Where(Function(selezionata) selezionata.Dose.HasValue)

            If selezionate.Count() > 0 Then

                lblSezioneVaccinazioni.Text += String.Format(" (selezionate: {0})", selezionate.Count().ToString())

                lblSezioneVaccinazioni.ToolTip =
                    String.Join(Environment.NewLine, selezionate.Select(Function(p) String.Format("{0} [{1}]", p.Descrizione, p.Dose)).ToArray())

            End If

        End If

        If UseUpperCaseCaption Then
            lblSezioneVaccinazioni.Text = lblSezioneVaccinazioni.Text.ToUpper()
        End If

    End Sub

    Private Sub ClearDatiVaccinazioni()

        dlsVaccinazioni.DataSource = Nothing
        dlsVaccinazioni.DataBind()

        lblSezioneVaccinazioni.Text = "Vaccinazioni"
        If UseUpperCaseCaption Then
            lblSezioneVaccinazioni.Text = lblSezioneVaccinazioni.Text.ToUpper()
        End If

        lblSezioneVaccinazioni.ToolTip = String.Empty

    End Sub

    Private Sub ClearDatiViaggio()

        dpkInizioViaggio.Data = DateTime.MinValue
        dpkFineViaggio.Data = DateTime.MinValue
        txtGiorniViaggio.Value = String.Empty

        fmPaeseViaggio.Codice = String.Empty
        fmPaeseViaggio.Descrizione = String.Empty
        fmPaeseViaggio.RefreshDataBind()

    End Sub
    Private Sub ClearListaViaggi()
        SetDatiViaggioRpt(Nothing)
    End Sub
    Private Sub ClearDateFollowUp()
        SetDateFollowUp(Nothing, Nothing)
    End Sub

    Private Sub rptViaggi_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptViaggi.ItemDataBound
        If e.Item.ItemType = ListItemType.Header Then
            Dim InsertViaggi As ImageButton = e.Item.FindControl("InsertViaggi")
            InsertViaggi.Enabled = Enabled
            If Enabled Then
                InsertViaggi.ImageUrl = ResolveClientUrl("~/Images/nuovo.gif")
            Else
                InsertViaggi.ImageUrl = ResolveClientUrl("~/Images/nuovo_dis.gif")
            End If
        End If


        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim enableViaggi As Boolean = Enabled
            Dim DeleteViaggi As ImageButton = e.Item.FindControl("DeleteViaggi")
            Dim dpkInizioViaggioRpt As Onit.Web.UI.WebControls.Validators.OnitDatePick = e.Item.FindControl("dpkInizioViaggioRpt")
            Dim dpkFineViaggioRpt As Onit.Web.UI.WebControls.Validators.OnitDatePick = e.Item.FindControl("dpkFineViaggioRpt")
            Dim fmPaeseViaggioRpt As Onit.Controls.OnitModalList = e.Item.FindControl("fmPaeseViaggioRpt")
            Dim txtGiorniViaggioRpt As TextBox = e.Item.FindControl("txtGiorniViaggioRpt")
            Dim OperazioneRpt As Label = e.Item.FindControl("OperazioneRpt")
            If OperazioneRpt.Text = OperazioneViaggio.Delete.ToString() Then
                enableViaggi = False
            Else
                enableViaggi = Enabled

            End If
            If Not DeleteViaggi Is Nothing Then
                DeleteViaggi.Enabled = enableViaggi
            End If
            If Not dpkInizioViaggioRpt Is Nothing Then
                dpkInizioViaggioRpt.Enabled = enableViaggi
            End If
            If Not dpkFineViaggioRpt Is Nothing Then
                dpkFineViaggioRpt.Enabled = enableViaggi
            End If
            If Not fmPaeseViaggioRpt Is Nothing Then
                fmPaeseViaggioRpt.Enabled = enableViaggi
            End If
            If enableViaggi Then
                DeleteViaggi.ImageUrl = ResolveClientUrl("~/Images/elimina.gif")
                dpkInizioViaggioRpt.CssClass = CSS.TEXTBOX_DATA
                dpkFineViaggioRpt.CssClass = CSS.TEXTBOX_DATA
            Else
                DeleteViaggi.ImageUrl = ResolveClientUrl("~/Images/elimina_dis.gif")
                dpkInizioViaggioRpt.CssClass = CSS.TEXTBOX_DATA_DISABILITATO
                dpkFineViaggioRpt.CssClass = CSS.TEXTBOX_DATA_DISABILITATO
            End If

            Dim totGiorni As Integer? = Biz.BizVisite.CalcolaTotaleGiorniViaggio(dpkInizioViaggioRpt.Data, dpkFineViaggioRpt.Data)
            If totGiorni.HasValue Then
                txtGiorniViaggioRpt.Text = totGiorni.Value.ToString()
            Else
                txtGiorniViaggioRpt.Text = String.Empty
            End If

        End If
    End Sub
    Protected Sub btnInsertViaggio_Click(sender As Object, e As EventArgs)
        Dim listaViaggiCopy As New List(Of ViaggioVisita)
        Dim insViaggio As New ViaggioVisita
        'insViaggio.DataInizioViaggio = Date.Today()
        insViaggio.Operazione = OperazioneViaggio.Insert
        listaViaggiCopy.Add(insViaggio)
        For Index As Integer = 0 To rptViaggi.Items.Count - 1
            Dim lis As New ViaggioVisita
            Dim dpkInizioViaggioRpt As Onit.Web.UI.WebControls.Validators.OnitDatePick = rptViaggi.Items(index).FindControl("dpkInizioViaggioRpt")
            Dim dpkFineViaggioRpt As Onit.Web.UI.WebControls.Validators.OnitDatePick = rptViaggi.Items(index).FindControl("dpkFineViaggioRpt")
            Dim fmPaeseViaggioRpt As Onit.Controls.OnitModalList = rptViaggi.Items(index).FindControl("fmPaeseViaggioRpt")
            Dim idViaggioRpt As Label = rptViaggi.Items(index).FindControl("idViaggioRpt")
            Dim IdVisitaRpt As Label = rptViaggi.Items(index).FindControl("IdVisitaRpt")
            Dim OperazioneRpt As Label = rptViaggi.Items(index).FindControl("OperazioneRpt")
            Dim DeleteViaggi As ImageButton = rptViaggi.Items(Index).FindControl("DeleteViaggi")
            If Not String.IsNullOrWhiteSpace(dpkInizioViaggioRpt.Text) Then
                lis.DataInizioViaggio = dpkInizioViaggioRpt.Text
            End If
            If Not String.IsNullOrWhiteSpace(dpkFineViaggioRpt.Text) Then
                lis.DataFineViaggio = dpkFineViaggioRpt.Text
            End If
            lis.CodicePaese = fmPaeseViaggioRpt.Codice
            lis.DescPaese = fmPaeseViaggioRpt.Descrizione
            lis.Id = idViaggioRpt.Text
            lis.IdVisita = IdVisitaRpt.Text
            If OperazioneRpt.Text = OperazioneViaggio.Delete.ToString() Then
                lis.Operazione = OperazioneViaggio.Delete
            End If
            If OperazioneRpt.Text = OperazioneViaggio.Update.ToString() Then
                lis.Operazione = OperazioneViaggio.Update
            End If
            If OperazioneRpt.Text = OperazioneViaggio.Insert.ToString() Then
                lis.Operazione = OperazioneViaggio.Insert
            End If
            listaViaggiCopy.Add(lis)
        Next

        SetDatiViaggioRpt(listaViaggiCopy)
    End Sub

    Private Sub rptViaggi_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rptViaggi.ItemCommand
        If e.Item.ItemIndex >= 0 Then
            If e.CommandName = "ClickDelete" Then
                Dim idViaggioRpt As Label = e.Item.FindControl("idViaggioRpt")
                If e.CommandArgument.ToString() = idViaggioRpt.Text Then
                    Dim operazione As Label = e.Item.FindControl("OperazioneRpt")
                    operazione.Text = Entities.OperazioneViaggio.Delete.ToString()
                    Dim DeleteViaggi As ImageButton = e.Item.FindControl("DeleteViaggi")
                    Dim dpkInizioViaggioRpt As Onit.Web.UI.WebControls.Validators.OnitDatePick = e.Item.FindControl("dpkInizioViaggioRpt")
                    Dim dpkFineViaggioRpt As Onit.Web.UI.WebControls.Validators.OnitDatePick = e.Item.FindControl("dpkFineViaggioRpt")
                    Dim fmPaeseViaggioRpt As Onit.Controls.OnitModalList = e.Item.FindControl("fmPaeseViaggioRpt")
                    dpkFineViaggioRpt.Enabled = False
                    dpkFineViaggioRpt.CssClass = CSS.TEXTBOX_DATA_DISABILITATO
                    dpkInizioViaggioRpt.Enabled = False
                    dpkInizioViaggioRpt.CssClass = CSS.TEXTBOX_DATA_DISABILITATO
                    fmPaeseViaggioRpt.Enabled = False
                    DeleteViaggi.Enabled = False
                    DeleteViaggi.ImageUrl = "../../Images/elimina_dis.gif"
                End If
            End If
        End If
    End Sub



#End Region

End Class