Imports System.Collections.Generic

Public Class ReazAvverseFarmaco
    Inherits Common.UserControlPageBase

#Region " Types "

    Public Enum TipoFarmacoReazione
        Sospetto
        Concomitante
    End Enum

#End Region

#Region " Properties "

    Public Property TipoFarmaco() As TipoFarmacoReazione
        Get
            If ViewState("tipo") Is Nothing Then ViewState("tipo") = TipoFarmacoReazione.Sospetto
            Return DirectCast(ViewState("tipo"), TipoFarmacoReazione)
        End Get
        Set(value As TipoFarmacoReazione)
            ViewState("tipo") = value
        End Set
    End Property

    ''' <summary>
    ''' Numero del farmaco, parte da 1
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property Ordinal As Integer
        Get
            If ViewState("ordinal") Is Nothing Then ViewState("ordinal") = 1
            Return DirectCast(ViewState("ordinal"), Integer)
        End Get
        Set(value As Integer)
            ViewState("ordinal") = value
        End Set
    End Property

    Protected ReadOnly Property ClickSospesoFunctionName As String
        Get
            Return String.Format("{0}_clickSospeso", Me.ClientID)
        End Get
    End Property

    Protected Property Enabled As Boolean
        Get
            If ViewState("enabled") Is Nothing Then ViewState("enabled") = True
            Return Convert.ToBoolean(ViewState("enabled"))
        End Get
        Set(value As Boolean)
            ViewState("enabled") = value
        End Set
    End Property

#End Region

#Region " Public Events "

    Public Event ReplicaFarmaco(sender As Object, e As Common.ReazioniAvverseCommon.ReplicaEventArgs)

    Private Sub OnReplicaFarmaco(farmacoInfo As Common.ReazioniAvverseCommon.FarmacoInfo)

        RaiseEvent ReplicaFarmaco(Me, New Common.ReazioniAvverseCommon.ReplicaEventArgs(farmacoInfo))

    End Sub

#End Region

#Region " Page Events "

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), String.Format("{0}_clickSosp", Me.ClientID), GetJSClickSospesoFunction(), False)

        Me.ddlSospeso.Attributes("onchange") = String.Format("{0}(this, 'S');", Me.ClickSospesoFunctionName)

    End Sub

#End Region

#Region " Public "

    Public Sub Inizializza(farmacoCorrente As Common.ReazioniAvverseCommon.FarmacoInfo, enabled As Boolean)

        Inizializza(farmacoCorrente, enabled, False)

    End Sub

    Public Sub Inizializza(farmacoCorrente As Common.ReazioniAvverseCommon.FarmacoInfo, enabled As Boolean, showReplicaIfFirst As Boolean)

        If farmacoCorrente Is Nothing Then
            Clear()
            Return
        End If

        Me.Ordinal = farmacoCorrente.Ordinal
        Me.Enabled = enabled

        If Me.Ordinal = 1 Then
            ' Il primo dei farmaci sospetti deve avere il pulsante replica solo se ce ne sono altri. => ce lo facciamo dire da chi inizializza con il parametro showReplicaIfFirst.
            ' Il primo dei farmaci concomitanti deve avere il pulsante replica sempre, perchè i concomitanti, se ci sono, sono sempre due (poi il secondo può non essere compilato).
            Me.btnReplica.Visible = showReplicaIfFirst Or Me.TipoFarmaco = TipoFarmacoReazione.Concomitante
            Me.btnReplica.Enabled = enabled
            Me.chkReplica.Visible = False
        Else
            Me.btnReplica.Visible = False
            Me.chkReplica.Visible = True
            Me.chkReplica.Enabled = enabled
        End If

        Me.chkReplica.Checked = False

        If farmacoCorrente.VesId.HasValue Then
            Me.hidVesId.Value = farmacoCorrente.VesId.Value.ToString()
        Else
            Me.hidVesId.Value = String.Empty
        End If

        Me.txtNomeCommerciale.Text = farmacoCorrente.DescrizioneNomeCommerciale
        farmaco.Label = farmacoCorrente.DescrizioneNomeCommerciale
        farmaco.CodiceAnagrafica = farmacoCorrente.CodiceNomeCommerciale
        Me.txtLotto.Text = farmacoCorrente.CodiceLotto

        If farmacoCorrente.Dose.HasValue Then
            Me.txtDose.Text = farmacoCorrente.Dose.Value.ToString()
        Else
            Me.txtDose.Text = String.Empty
        End If

        Me.fmViaSomministrazione.Codice = farmacoCorrente.CodiceViaSomministrazione
        Me.fmViaSomministrazione.Descrizione = farmacoCorrente.DescrizioneViaSomministrazione
        Me.fmViaSomministrazione.RefreshDataBind()

        Me.fmSitoInoculo.Codice = farmacoCorrente.CodiceSitoInoculazione
        Me.fmSitoInoculo.Descrizione = farmacoCorrente.DescrizioneSitoInoculazione
        Me.fmSitoInoculo.RefreshDataBind()

        Select Case Me.TipoFarmaco

            Case TipoFarmacoReazione.Sospetto

                Me.lblId.Text = String.Format("Farmaco Sospetto {0}", Me.Ordinal.ToString())

                'SetTextBoxLayoutStringa(Me.txtNomeCommerciale, True, True)
                farmaco.SetTextBoxLayoutStringa(True, True)
                SetTextBoxLayoutStringa(Me.txtLotto, True, True)
                SetTextBoxLayoutNumerico(Me.txtDose, False, False)

                Me.fmViaSomministrazione.Enabled = False
                Me.fmSitoInoculo.Enabled = False

                ' Data-ora editabili solo per concomitanti
                If farmacoCorrente.DataOraEsecuzioneVaccinazione.HasValue Then
                    Me.txtDataOraEsecuzioneVac.Text = farmacoCorrente.DataOraEsecuzioneVaccinazione.Value.ToString("dd/MM/yyyy HH:mm")
                Else
                    Me.txtDataOraEsecuzioneVac.Text = String.Empty
                End If

                Me.txtDataOraEsecuzioneVac.Visible = True
                SetTextBoxLayoutStringa(Me.txtDataOraEsecuzioneVac, True, False)

                Me.dpkDataEsecuzioneVac.Text = String.Empty
                Me.txtOraEsecuzioneVac.Text = String.Empty
                Me.dpkDataEsecuzioneVac.Visible = False
                Me.txtOraEsecuzioneVac.Visible = False

                If enabled Then
                    Me.ddlSospeso.Enabled = True
                    Me.ddlSospeso.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Stringa_Obbligatorio
                Else
                    Me.ddlSospeso.Enabled = False
                    Me.ddlSospeso.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Stringa_Disabilitato
                End If
                SetDataPickerLayout(dpkDataScadenza, False, False)
                If farmacoCorrente.DataScadLotto.HasValue Then
                    dpkDataScadenza.Text = farmacoCorrente.DataScadLotto.Value.ToString("dd/MM/yyyy")
                Else
                    dpkDataScadenza.Text = String.Empty
                End If



            Case TipoFarmacoReazione.Concomitante

                Me.lblId.Text = String.Format("Concomitante {0}", Me.Ordinal.ToString())

                'SetTextBoxLayoutStringa(Me.txtNomeCommerciale, Not enabled, Me.Ordinal = 1)
                farmaco.SetTextBoxLayoutStringa(Not enabled, Ordinal = 1)
                SetTextBoxLayoutStringa(Me.txtLotto, Not enabled, Me.Ordinal = 1)
                SetTextBoxLayoutNumerico(Me.txtDose, Not enabled, False)
                SetDataPickerLayout(dpkDataScadenza, enabled, False)
                If farmacoCorrente.DataScadLotto.HasValue Then
                    dpkDataScadenza.Text = farmacoCorrente.DataScadLotto.Value.ToString("dd/MM/yyyy")
                Else
                    dpkDataScadenza.Text = String.Empty
                End If
                Me.fmViaSomministrazione.Enabled = enabled
                Me.fmSitoInoculo.Enabled = enabled

                ' Data-ora editabili solo per concomitanti e solo se lo uc è abilitato
                If enabled Then

                    Me.txtDataOraEsecuzioneVac.Text = String.Empty
                    Me.txtDataOraEsecuzioneVac.Visible = False

                    Me.dpkDataEsecuzioneVac.Visible = True
                    Me.txtOraEsecuzioneVac.Visible = True
                    SetDataPickerLayout(Me.dpkDataEsecuzioneVac, True, False)
                    SetTextBoxLayoutStringa(Me.txtOraEsecuzioneVac, False, False)

                    If farmacoCorrente.DataOraEsecuzioneVaccinazione.HasValue Then
                        Me.dpkDataEsecuzioneVac.Data = farmacoCorrente.DataOraEsecuzioneVaccinazione.Value
                        Me.txtOraEsecuzioneVac.Text = farmacoCorrente.DataOraEsecuzioneVaccinazione.Value.ToShortTimeString()
                    Else
                        Me.dpkDataEsecuzioneVac.Text = String.Empty
                        Me.txtOraEsecuzioneVac.Text = String.Empty
                    End If

                Else

                    Me.txtDataOraEsecuzioneVac.Visible = True
                    SetTextBoxLayoutStringa(Me.txtDataOraEsecuzioneVac, True, False)

                    If farmacoCorrente.DataOraEsecuzioneVaccinazione.HasValue Then
                        Me.txtDataOraEsecuzioneVac.Text = farmacoCorrente.DataOraEsecuzioneVaccinazione.Value.ToString("dd/MM/yyyy HH:mm")
                    Else
                        Me.txtDataOraEsecuzioneVac.Text = String.Empty
                    End If

                    Me.dpkDataEsecuzioneVac.Text = String.Empty
                    Me.txtOraEsecuzioneVac.Text = String.Empty
                    Me.dpkDataEsecuzioneVac.Visible = False
                    Me.txtOraEsecuzioneVac.Visible = False

                End If

                If enabled Then

                    Me.ddlSospeso.Enabled = True

                    If Me.Ordinal = 1 Then
                        Me.ddlSospeso.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Stringa_Obbligatorio
                    Else
                        Me.ddlSospeso.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Stringa
                    End If

                Else

                    Me.ddlSospeso.Enabled = False
                    Me.ddlSospeso.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Stringa_Disabilitato

                End If

            Case Else

                Throw New NotSupportedException("TipoFarmaco non previsto")

        End Select

        Me.ddlMiglioramento.Enabled = enabled
        Me.ddlRipreso.Enabled = enabled
        Me.ddlRicomparsa.Enabled = enabled
        Me.txtIndicazioni.Enabled = enabled
        Me.txtDosaggio.Enabled = enabled
        Me.txtRichiamo.Enabled = enabled

        SetInfoFarmacoReplicabili(farmacoCorrente)


        ' tb_farmaco => txtNomeCommerciale
        ' tb_lotto => txtLotto
        ' odpScadenzaLotto => tolto: mantenere ???
        ' tb_dose => txtDose
        ' fm_Somministrazione => fmViaSomministrazione
        ' lblOraVacc => lblDataOraEsecuzioneVac
        ' tb_ora => txtDataOraEsecuzioneVac
        ' fm_SitoInoculo => fmSitoInoculo
        ' ddl_sospeso => ddlSospeso
        ' ddl_reazione => ddlMiglioramento
        ' ddl_ripreso => ddlRipreso
        ' ddl_ricomparsa => ddlRicomparsa
        ' tb_Motivo => txtIndicazioni
        ' tb_fiale => txtDosaggio
        ' tb_richiamo => txtRichiamo

    End Sub

    Public Sub ReplicaInfoFarmaco(farmacoInfo As Common.ReazioniAvverseCommon.FarmacoInfo)

        If Me.chkReplica.Checked Then
            SetInfoFarmacoReplicabili(farmacoInfo)
        End If

    End Sub

    Public Function GetFarmacoInfo() As Common.ReazioniAvverseCommon.FarmacoInfo

        Dim farmacoCorrente As New Common.ReazioniAvverseCommon.FarmacoInfo()

        farmacoCorrente.Ordinal = Me.Ordinal
        'farmacoCorrente.DescrizioneNomeCommerciale = Me.txtNomeCommerciale.Text
        farmacoCorrente.DescrizioneNomeCommerciale = farmaco.Label
        farmacoCorrente.CodiceNomeCommerciale = farmaco.CodiceAnagrafica
        farmacoCorrente.CodiceLotto = Me.txtLotto.Text

        If String.IsNullOrWhiteSpace(dpkDataScadenza.Text) Then
            farmacoCorrente.DataScadLotto = Nothing
        Else
            farmacoCorrente.DataScadLotto = Convert.ToDateTime(String.Format("{0}", Me.dpkDataScadenza.Text))
        End If

        If String.IsNullOrWhiteSpace(Me.txtDose.Text) Then
            farmacoCorrente.Dose = Nothing
        Else
            farmacoCorrente.Dose = Convert.ToInt32(Me.txtDose.Text)
        End If

        If Me.TipoFarmaco = TipoFarmacoReazione.Sospetto Then

            If String.IsNullOrWhiteSpace(Me.txtDataOraEsecuzioneVac.Text) Then
                farmacoCorrente.DataOraEsecuzioneVaccinazione = Nothing
            Else
                farmacoCorrente.DataOraEsecuzioneVaccinazione = Convert.ToDateTime(Me.txtDataOraEsecuzioneVac.Text)
            End If

        Else

            If String.IsNullOrWhiteSpace(Me.dpkDataEsecuzioneVac.Text) Then
                farmacoCorrente.DataOraEsecuzioneVaccinazione = Nothing
            Else

                If String.IsNullOrWhiteSpace(Me.txtOraEsecuzioneVac.Text) Then
                    Me.txtOraEsecuzioneVac.Text = "00:00"
                Else
                    ' TODO [ReazAvv]: manca controllo campo ora
                End If

                farmacoCorrente.DataOraEsecuzioneVaccinazione =
                    Convert.ToDateTime(String.Format("{0} {1}", Me.dpkDataEsecuzioneVac.Text, Me.txtOraEsecuzioneVac.Text.Replace(".", ":")))

            End If

        End If

        farmacoCorrente.CodiceViaSomministrazione = Me.fmViaSomministrazione.Codice
        farmacoCorrente.DescrizioneViaSomministrazione = Me.fmViaSomministrazione.Descrizione
        farmacoCorrente.CodiceSitoInoculazione = Me.fmSitoInoculo.Codice
        farmacoCorrente.DescrizioneSitoInoculazione = Me.fmSitoInoculo.Descrizione

        farmacoCorrente.Sospeso = Me.ddlSospeso.SelectedValue
        farmacoCorrente.ReazioneMigliorata = Me.ddlMiglioramento.SelectedValue
        farmacoCorrente.Ripreso = Me.ddlRipreso.SelectedValue
        farmacoCorrente.RicomparsiSintomi = Me.ddlRicomparsa.SelectedValue
        'farmacoCorrente.Indicazioni = Me.txtIndicazioni.Text
        farmacoCorrente.Indicazioni = Indicazioni.Label
        farmacoCorrente.CodiceIndicazioni = Indicazioni.CodiceAnagrafica
        farmacoCorrente.DosaggioFiale = Me.txtDosaggio.Text

        If String.IsNullOrWhiteSpace(Me.txtRichiamo.Text) Then
            farmacoCorrente.Richiamo = Nothing
        Else
            farmacoCorrente.Richiamo = Convert.ToInt32(Me.txtRichiamo.Text)
        End If

        If String.IsNullOrWhiteSpace(Me.hidVesId.Value) Then
            farmacoCorrente.VesId = Nothing
        Else
            farmacoCorrente.VesId = Convert.ToInt64(Me.hidVesId.Value)
        End If

        Return farmacoCorrente

    End Function

    Public Function GetStringFieldsToCheck() As List(Of Entities.FieldToCheck(Of String))

        Dim list As New List(Of Entities.FieldToCheck(Of String))()

        Select Case Me.TipoFarmaco

            Case TipoFarmacoReazione.Sospetto

                list.Add(CreateStringFieldToCheck(Me.ddlSospeso.SelectedValue, Me.lblSospeso.Text, 0, True))

            Case TipoFarmacoReazione.Concomitante

                'list.Add(CreateStringFieldToCheck(Me.txtNomeCommerciale.Text, Me.lblNomeCommerciale.Text, Me.txtNomeCommerciale.MaxLength, Me.Ordinal = 1))
                list.Add(farmaco.CreateStringFieldToCheck(lblNomeCommerciale.Text, 64, Ordinal = 1, lblId.Text))
                list.Add(CreateStringFieldToCheck(Me.txtLotto.Text, Me.lblLotto.Text, Me.txtLotto.MaxLength, Me.Ordinal = 1))

                If Me.Ordinal = 1 Then
                    list.Add(CreateStringFieldToCheck(Me.ddlSospeso.SelectedValue, Me.lblSospeso.Text, 0, True))
                End If

            Case Else

                Throw New NotSupportedException("TipoFarmaco non previsto")

        End Select

        Return list

    End Function

    Public Function GetDateTimeFieldsToCheck() As List(Of Entities.FieldToCheck(Of DateTime))

        ' Nessun campo datetime da controllare
        Return Nothing

    End Function

    Public Function GetDataOraVaccinazioneString() As String

        If Me.txtDataOraEsecuzioneVac.Visible Then
            Return Me.txtDataOraEsecuzioneVac.Text
        End If

        Return Me.dpkDataEsecuzioneVac.Data.ToString("dd/MM/yyyy")

    End Function

    Public Function GetDataOraVaccinazioneDateTime() As DateTime

        If Me.txtDataOraEsecuzioneVac.Visible Then
            Return Convert.ToDateTime(Me.txtDataOraEsecuzioneVac.Text)
        End If

        Return Me.dpkDataEsecuzioneVac.Data

    End Function

#End Region

#Region " Button Events "

    Private Sub btnReplica_Click(sender As Object, e As EventArgs) Handles btnReplica.Click

        OnReplicaFarmaco(GetFarmacoInfo())

    End Sub

#End Region

#Region " Private "

    Private Function GetJSClickSospesoFunction() As String

        Dim script As New System.Text.StringBuilder()

        script.AppendLine("<script type='text/javascript'>")
        script.AppendFormat("function {0}(ddlSospeso, val) {{", Me.ClickSospesoFunctionName).AppendLine()
        script.AppendFormat("var ddlMiglioramento = document.getElementById('{0}');", Me.ddlMiglioramento.ClientID).AppendLine()
        script.AppendFormat("var ddlRipreso = document.getElementById('{0}');", Me.ddlRipreso.ClientID).AppendLine()
        script.AppendFormat("var ddlRicomparsa = document.getElementById('{0}');", Me.ddlRicomparsa.ClientID)
        script.AppendLine("if (ddlSospeso.options[ddlSospeso.selectedIndex].value == val) {")
        script.AppendLine("ddlMiglioramento.disabled = false;")
        script.AppendLine("ddlRipreso.disabled = false;")
        script.AppendLine("ddlRicomparsa.disabled = false;")
        script.AppendLine("} else {")
        script.AppendLine("ddlMiglioramento.disabled = true;")
        script.AppendLine("ddlMiglioramento.value = '';")
        script.AppendLine("ddlRipreso.disabled = true;")
        script.AppendLine("ddlRipreso.value = '';")
        script.AppendLine("ddlRicomparsa.disabled = true;")
        script.AppendLine("ddlRicomparsa.value = '';")
        script.AppendLine("}}</script>")

        Return script.ToString()

    End Function

    Private Sub Clear()

        Me.hidVesId.Value = String.Empty
        Me.lblId.Text = String.Empty

        Me.txtNomeCommerciale.Text = String.Empty
        Me.txtLotto.Text = String.Empty
        Me.txtDose.Text = String.Empty

        Me.fmViaSomministrazione.Codice = String.Empty
        Me.fmViaSomministrazione.Descrizione = String.Empty
        Me.fmViaSomministrazione.RefreshDataBind()
        Me.fmViaSomministrazione.Enabled = False

        Me.txtDataOraEsecuzioneVac.Text = String.Empty
        Me.dpkDataEsecuzioneVac.Text = String.Empty
        Me.txtOraEsecuzioneVac.Text = String.Empty

        Me.fmSitoInoculo.Codice = String.Empty
        Me.fmSitoInoculo.Descrizione = String.Empty
        Me.fmSitoInoculo.RefreshDataBind()
        Me.fmSitoInoculo.Enabled = False

        Me.ddlSospeso.ClearSelection()
        Me.ddlSospeso.SelectedValue = String.Empty

        Me.ddlMiglioramento.ClearSelection()
        Me.ddlMiglioramento.SelectedValue = String.Empty

        Me.ddlRipreso.ClearSelection()
        Me.ddlRipreso.SelectedValue = String.Empty

        Me.ddlRicomparsa.ClearSelection()
        Me.ddlRicomparsa.SelectedValue = String.Empty

        Me.txtIndicazioni.Text = String.Empty
        Me.txtDosaggio.Text = String.Empty
        Me.txtRichiamo.Text = String.Empty

    End Sub

    Private Sub SetInfoFarmacoReplicabili(farmacoInfo As Common.ReazioniAvverseCommon.FarmacoInfo)

        Me.ddlSospeso.ClearSelection()
        Me.ddlSospeso.SelectedValue = farmacoInfo.Sospeso

        Me.ddlMiglioramento.ClearSelection()
        Me.ddlMiglioramento.SelectedValue = farmacoInfo.ReazioneMigliorata

        Me.ddlRipreso.ClearSelection()
        Me.ddlRipreso.SelectedValue = farmacoInfo.Ripreso

        Me.ddlRicomparsa.ClearSelection()
        Me.ddlRicomparsa.SelectedValue = farmacoInfo.RicomparsiSintomi

        Me.txtIndicazioni.Text = farmacoInfo.Indicazioni
        Indicazioni.Label = farmacoInfo.Indicazioni
        Indicazioni.CodiceAnagrafica = farmacoInfo.CodiceIndicazioni

        If String.IsNullOrWhiteSpace(farmacoInfo.DosaggioFiale) Then
            Me.txtDosaggio.Text = String.Empty
        Else
            Me.txtDosaggio.Text = farmacoInfo.DosaggioFiale
        End If

        If farmacoInfo.Richiamo.HasValue Then
            Me.txtRichiamo.Text = farmacoInfo.Richiamo.Value.ToString()
        Else
            Me.txtRichiamo.Text = String.Empty
        End If

    End Sub

    Private Sub SetTextBoxLayoutStringa(txt As TextBox, isReadOnly As Boolean, isRequired As Boolean)

        If isReadOnly Then
            txt.ReadOnly = True
            txt.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Stringa_Disabilitato
        Else
            txt.ReadOnly = False
            If isRequired Then
                txt.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Stringa_Obbligatorio
            Else
                txt.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Stringa
            End If
        End If

    End Sub

    Private Sub SetTextBoxLayoutNumerico(txt As Onit.Web.UI.WebControls.Validators.OnitJsValidator, isReadOnly As Boolean, isRequired As Boolean)

        If isReadOnly Then
            txt.ReadOnly = True
            txt.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Numerico_Disabilitato

        Else
            txt.ReadOnly = False
            If isRequired Then
                txt.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Numerico_Obbligatorio
            Else
                txt.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Numerico
            End If
        End If

    End Sub

    Private Sub SetDataPickerLayout(dpk As Onit.Web.UI.WebControls.Validators.OnitDatePick, isEnabled As Boolean, isRequired As Boolean)

        If isEnabled Then
            dpk.Enabled = True
            If isRequired Then
                dpk.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Data_Obbligatorio
            Else
                dpk.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Data
            End If
        Else
            dpk.Enabled = False
            dpk.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Data_Disabilitato
        End If

    End Sub

    Private Function CreateStringFieldToCheck(value As String, description As String, maxLength As Integer, isRequired As Boolean) As Entities.FieldToCheck(Of String)

        Dim fieldToCheck As New Entities.FieldToCheck(Of String)()

        fieldToCheck.Value = value
        fieldToCheck.Description = String.Format("{0} - {1}", Me.lblId.Text, description)
        fieldToCheck.MaxLength = maxLength
        fieldToCheck.Required = isRequired

        Return fieldToCheck

    End Function

#End Region

End Class