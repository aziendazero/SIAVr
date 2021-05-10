Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager


Partial Class ReazAvverseDetail
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

#Region " Events "

    Public Event RecuperaConcomitanti(sender As Object, ByRef farmaciRecuperabili As List(Of Common.ReazioniAvverseCommon.FarmacoRecupero))

    Private Function OnRecuperaConcomitanti() As List(Of Common.ReazioniAvverseCommon.FarmacoRecupero)

        Dim farmaciRecuperabili As List(Of Common.ReazioniAvverseCommon.FarmacoRecupero) = Nothing

        RaiseEvent RecuperaConcomitanti(Me, farmaciRecuperabili)

        Return farmaciRecuperabili

    End Function

    Public Event ShowMessage(sender As Object, e As Common.ReazioniAvverseCommon.ShowMessageReazioniEventArgs)

    Private Sub OnShowMessage(message As String)

        RaiseEvent ShowMessage(Me, New Common.ReazioniAvverseCommon.ShowMessageReazioniEventArgs(message))

    End Sub

#End Region

#Region " Types "

    Private Class QualificaSegnalatore
        Public Const Pediatra As String = "PEDIATRA DI LIBERA SCELTA"
        Public Const MedicoDistretto As String = "MEDICO DISTRETTO"
        Public Const MedicoOspedaliero As String = "MEDICO OSPEDALIERO"
        Public Const MedicoMedicinaGenerale As String = "MMG"
        Public Const Farmacista As String = "FARMACISTA"
        Public Const Specialista As String = "SPECIALISTA"
        Public Const Infermiere As String = "INF"
        Public Const Cav As String = "CAV"
        Public Const Altro As String = "ALTRO"
    End Class

    Private Class ValoreGravidanza
        Public Const No As String = "N"
        Public Const PrimoTrimestre As String = "1"
        Public Const SecondoTrimestre As String = "2"
        Public Const TerzoTrimestre As String = "3"
        Public Const Sconosciuta As String = "0"
    End Class

    Private Class TipoCausaReazioneOsservata
        Public Const Nessuna As String = "N"
        Public Const Interazione As String = "INT"
        Public Const ErroreTerapeutico As String = "ERR"
        Public Const Abuso As String = "ABS"
        Public Const Misuso As String = "MIS"
        Public Const OffLabel As String = "OFF"
        Public Const Overdose As String = "OVR"
        Public Const EsposizioneProfessionale As String = "ESP"
    End Class

    Private Class ImpostazioniFarmaco
        Public FarmacoInfo As Common.ReazioniAvverseCommon.FarmacoInfo
        Public IsVisible As Boolean
    End Class

    Private Enum GridRecuperoColumnIndex
        CheckBox = 0
        DataOraEsecuzioneVaccinazione = 1
        DescrizioneAssociazione = 2
        DoseAssociazione = 3
        DescrizioneNomeCommerciale = 4
        CodiceAssociazione = 5
        CodiceLotto = 6
        CodiceSitoInoculazione = 7
        DescrizioneSitoInoculazione = 8
        CodiceViaSomministrazione = 9
        DescrizioneViaSomministrazione = 10
        VesId = 11
        DataScadenzaLotto = 12
        CodiceNomeCommerciale = 13
    End Enum

#End Region

#Region " Enums "

    Private Enum Esito
        RisoluzioneCompleta = 1
        RisoluzioneConPostumi = 2
        Miglioramento = 3
        ReazioneInvariata = 4
        DataDecesso = 5
        NonDisponibile = 6
    End Enum

    Private Enum GravitaReazione
        Decesso = 1
        Ospedalizzazione = 2
        Invalidita = 3
        PericoloVita = 4
        Anomalie = 5
        AltraCondizione = 6
    End Enum

    Private Enum MotivoDecesso
        ReazioneAvversa = 1
        Farmaco = 2
        NoFarmaco = 3
        CausaSconosciuta = 4
    End Enum

    Public Enum StatoControllo
        NonAbilitato = 0
        SolaLettura = 1
        Modifica = 2
        Inserimento = 3
    End Enum

#End Region

#Region " Properties "

    Public Property StatoControlloCorrente() As StatoControllo
        Get
            Return ViewState("[ReadOnly]")
        End Get
        Set(Value As StatoControllo)
            ViewState("[ReadOnly]") = Value
        End Set
    End Property

    Public ReadOnly Property IsAltraReazioneObbligatoria() As Boolean
        Get
            Return Me.tb_altroReaz.Enabled
        End Get
    End Property

    Public ReadOnly Property GetFmDesc() As String
        Get
            Return Me.fm_tipoReaz.Descrizione
        End Get
    End Property

    Private Property IsPazienteDeceduto() As Boolean
        Get
            Return ViewState("OnVac_decesso")
        End Get
        Set(Value As Boolean)
            ViewState("OnVac_decesso") = Value
        End Set
    End Property

    Private Property ListaEseguiteReazioni As List(Of Entities.VaccinazioneEseguita)
        Get
            If ViewState("listEseRea") Is Nothing Then ViewState("listEseRea") = New List(Of Entities.VaccinazioneEseguita)()
            Return DirectCast(ViewState("listEseRea"), List(Of Entities.VaccinazioneEseguita))
        End Get
        Set(value As List(Of Entities.VaccinazioneEseguita))
            ViewState("listEseRea") = value
        End Set
    End Property

#End Region

#Region " Public "

    Public Function GetDataOraVaccinazioneString() As String

        If Me.ucFarmacoSospetto1.Visible Then
            Return Me.ucFarmacoSospetto1.GetDataOraVaccinazioneString()
        End If

        Return String.Empty

    End Function

#End Region

#Region " Public Variables "

    Public Warning As WarningMessage

#End Region

#Region " Page Events "

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        AddHandler rb_risCompleta.CheckedChanged, AddressOf Esito_CheckedChanged
        AddHandler rb_risPostumi.CheckedChanged, AddressOf Esito_CheckedChanged
        AddHandler rb_miglioramento.CheckedChanged, AddressOf Esito_CheckedChanged
        AddHandler rb_reazioneInvariata.CheckedChanged, AddressOf Esito_CheckedChanged
        AddHandler rb_EsitoDecesso.CheckedChanged, AddressOf Esito_CheckedChanged
        AddHandler rb_noDisponibile.CheckedChanged, AddressOf Esito_CheckedChanged

        AddHandler rb_grave.CheckedChanged, AddressOf GravitaReazione_CheckedChanged
        AddHandler rb_noGrave.CheckedChanged, AddressOf GravitaReazione_CheckedChanged

        ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "CheckDettaglioReazione", GetJSCheckDettaglioReazione(), False)

    End Sub

    Private Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles MyBase.PreRender

        Dim enabled As Boolean = Me.StatoControlloCorrente <> StatoControllo.SolaLettura

        Me.tb_vacCod.Enabled = enabled
        Me.tb_vacDesc.Enabled = enabled
        Me.dpkDataReazione.Enabled = enabled

        ' N.B. : non tutti i vecchi campi sono gestiti. Perchè???
        Me.txtPeso.Enabled = enabled
        Me.txtAltezza.Enabled = enabled
        Me.rblCausa.Enabled = enabled

        ' Da non impostare, l'abilitazione è gestita in base al sesso
        'Me.dpkUltimaMestruazione.Enabled = enabled
        'Me.ddlAllattamento.Enabled = enabled
        'Me.rblGravidanza.Enabled = enabled

        Me.fm_tipoReaz.Enabled = enabled
        Me.fm_tipoReaz1.Enabled = enabled
        Me.fm_tipoReaz2.Enabled = enabled

        If enabled Then

            If Me.fm_tipoReaz.Descrizione.Trim() = "ALTRO" Or fm_tipoReaz1.Descrizione.Trim() = "ALTRO" Or fm_tipoReaz2.Descrizione.Trim() = "ALTRO" Then
                Me.tb_altroReaz.Enabled = True
                Me.tb_altroReaz.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Stringa_Obbligatorio
            Else
                Me.tb_altroReaz.Enabled = False
                Me.tb_altroReaz.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Stringa_Disabilitato
                Me.tb_altroReaz.Text = String.Empty
            End If

        Else

            Me.tb_altroReaz.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Stringa_Disabilitato
            Me.tb_altroReaz.Enabled = False

        End If

        Me.tb_Terapie.Enabled = enabled
        Me.tb_VisRic.Enabled = enabled

        Me.ddlFarmaciConcomitanti.Enabled = enabled

        If Settings.REAZIONE_AVVERSA_INTEGRAZIONE Then
            txt_CognomeSeg.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Stringa_Obbligatorio
            txt_emailSeg.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Stringa_Obbligatorio
        Else
            txt_CognomeSeg.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Stringa
            txt_emailSeg.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Stringa
        End If

    End Sub

#End Region

#Region " Public Methods "

    Public Sub LoadDataIntoControl(listEseguiteReazioni As List(Of Entities.VaccinazioneEseguita))

        If listEseguiteReazioni.IsNullOrEmpty() Then Throw New Exception("Nessun dato da visualizzare")

        Me.ListaEseguiteReazioni = listEseguiteReazioni

        ' Il primo elemento della lista è quello da cui leggere tutti i dati.
        ' Gli altri, se presenti, servono per i dati dei farmaci.
        Dim eseguitaMaster As Entities.VaccinazioneEseguita = listEseguiteReazioni(0)

        Me.IsPazienteDeceduto =
            (eseguitaMaster.ReazioneAvversa.DataDecesso > Date.MinValue OrElse eseguitaMaster.ReazioneAvversa.StatoAnagrafico = Enumerators.StatoAnagrafico.DECEDUTO)

        Me.txtCodiceUslAppartenenzaReazione.Value = eseguitaMaster.ReazioneAvversa.CodiceUslInserimento

        Me.tb_vacDesc.Text = eseguitaMaster.vac_descrizione
        Me.tb_vacCod.Text = eseguitaMaster.ves_vac_codice
        Me.tb_assCod.Text = eseguitaMaster.ves_ass_codice
        Me.tb_assDesc.Text = eseguitaMaster.ass_descrizione

        Dim showVac As Boolean = String.IsNullOrWhiteSpace(eseguitaMaster.ves_ass_codice)

        Me.lblVac.Visible = showVac
        Me.tb_vacCod.Visible = showVac
        Me.tb_vacDesc.Visible = showVac
        Me.lblAss.Visible = Not showVac
        Me.tb_assCod.Visible = Not showVac
        Me.tb_assDesc.Visible = Not showVac

        Me.dpkDataReazione.Data = eseguitaMaster.ReazioneAvversa.DataReazione

        If eseguitaMaster.ReazioneAvversa.Peso.HasValue Then
            Me.txtPeso.Text = String.Format("{0:0.000}", eseguitaMaster.ReazioneAvversa.Peso.Value)
        Else
            Me.txtPeso.Text = String.Empty
        End If

        If eseguitaMaster.ReazioneAvversa.Altezza.HasValue Then
            Me.txtAltezza.Text = eseguitaMaster.ReazioneAvversa.Altezza.Value.ToString()
        Else
            Me.txtAltezza.Text = String.Empty
        End If

        Me.dpkUltimaMestruazione.Text = String.Empty
        Me.ddlAllattamento.ClearSelection()

        If eseguitaMaster.ReazioneAvversa.SessoPaziente = "M" Then

            Me.dpkUltimaMestruazione.Enabled = False
            Me.dpkUltimaMestruazione.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Data_Disabilitato

            Me.ddlAllattamento.Enabled = False

            Me.rblGravidanza.ClearSelection()
            Me.rblGravidanza.SelectedValue = ValoreGravidanza.No
            Me.rblGravidanza.Enabled = False

        Else

            Me.dpkUltimaMestruazione.Enabled = True
            Me.dpkUltimaMestruazione.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Data

            If eseguitaMaster.ReazioneAvversa.DataUltimaMestruazione.HasValue Then
                Me.dpkUltimaMestruazione.Data = eseguitaMaster.ReazioneAvversa.DataUltimaMestruazione.Value
            End If

            Me.ddlAllattamento.Enabled = True
            Me.ddlAllattamento.SelectedValue = eseguitaMaster.ReazioneAvversa.Allattamento

            LoadDataIntoGravidanza(eseguitaMaster)

        End If

        LoadDataIntoCausa(eseguitaMaster)

        LoadDataIntoReazioni(eseguitaMaster)

        Me.tb_Terapie.Text = eseguitaMaster.ReazioneAvversa.Terapie
        Me.tb_VisRic.Text = eseguitaMaster.ReazioneAvversa.VisiteRicoveri

        LoadDataIntoGravitaReazione(eseguitaMaster)

        LoadDataIntoEsito(eseguitaMaster)

        LoadDataIntoFarmaciSospetti(listEseguiteReazioni)

        LoadDataIntoFarmaciConcomitanti(eseguitaMaster)

        Me.tb_altriFarmaci.Text = eseguitaMaster.ReazioneAvversa.UsoConcomitante
        Me.tb_condConcomitanti.Text = eseguitaMaster.ReazioneAvversa.CondizioniConcomitanti
        Me.txtAltreInfo.Text = eseguitaMaster.ReazioneAvversa.AltreInformazioni

        ' Caricamento dei dati relativi al segnalatore
        LoadDataIntoQualifica(eseguitaMaster)

        LoadDataIntoAmbitoOsservazione(eseguitaMaster)
        Dim idUtente As Integer = OnVacContext.UserId
        Dim datiUte As Entities.Utente = Nothing
        Dim consult As Entities.Cns = Nothing
        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizCns As New Biz.BizConsultori(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos, Nothing)
                consult = bizCns.GetConsultorio(OnVacUtility.Variabili.CNS.Codice)
            End Using
            Using utebiz As New Biz.BizUtenti(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos)
                datiUte = utebiz.Getutente(idUtente)
            End Using
        End Using
        'Aggiunto verifiche necessariìe nel caso dell'integrazione con vigigarmaco
        If Not eseguitaMaster.ReazioneAvversa.CognomeSegnalatore Is Nothing AndAlso Not String.IsNullOrEmpty(eseguitaMaster.ReazioneAvversa.CognomeSegnalatore) Then
            Me.txt_CognomeSeg.Text = eseguitaMaster.ReazioneAvversa.CognomeSegnalatore
        Else
            'If Settings.REAZIONE_AVVERSA_INTEGRAZIONE Then
            '    If Not String.IsNullOrWhiteSpace(datiUte.Cognome) Then
            '        txt_CognomeSeg.Text = datiUte.Cognome
            '    Else
            '        txt_CognomeSeg.Text = consult.Descrizione
            '    End If
            'Else
            '    Me.txt_CognomeSeg.Text = eseguitaMaster.ReazioneAvversa.CognomeSegnalatore
            'End If
            If Not datiUte.Cognome Is Nothing AndAlso Not String.IsNullOrWhiteSpace(datiUte.Cognome) Then
                txt_CognomeSeg.Text = datiUte.Cognome
            ElseIf Not consult.Descrizione Is Nothing AndAlso Not String.IsNullOrWhiteSpace(consult.Descrizione) Then
                txt_CognomeSeg.Text = consult.Descrizione
            Else
                txt_CognomeSeg.Text = eseguitaMaster.ReazioneAvversa.CognomeSegnalatore
            End If
        End If
        If Not eseguitaMaster.ReazioneAvversa.NomeSegnalatore Is Nothing AndAlso Not String.IsNullOrEmpty(eseguitaMaster.ReazioneAvversa.NomeSegnalatore) Then
            Me.txt_nomeSeg.Text = eseguitaMaster.ReazioneAvversa.NomeSegnalatore
        Else
            If Not datiUte.Nome Is Nothing AndAlso Not String.IsNullOrWhiteSpace(datiUte.Nome) Then
                txt_nomeSeg.Text = datiUte.Nome
            Else
                txt_nomeSeg.Text = eseguitaMaster.ReazioneAvversa.NomeSegnalatore
            End If
        End If

        Me.txt_indirizzoSeg.Text = eseguitaMaster.ReazioneAvversa.IndirizzoSegnalatore
        Me.txt_telSeg.Text = eseguitaMaster.ReazioneAvversa.TelSegnalatore
        Me.txt_faxSeg.Text = eseguitaMaster.ReazioneAvversa.FaxSegnalatore
        'Aggiunto verifiche necessarie nel caso dell'integrazione con vigigarmaco
        If Not eseguitaMaster.ReazioneAvversa.MailSegnalatore Is Nothing AndAlso Not String.IsNullOrEmpty(eseguitaMaster.ReazioneAvversa.MailSegnalatore) Then
            Me.txt_emailSeg.Text = eseguitaMaster.ReazioneAvversa.MailSegnalatore
        Else
            'If Settings.REAZIONE_AVVERSA_INTEGRAZIONE Then
            '    If Not String.IsNullOrWhiteSpace(datiUte.Email) Then
            '        txt_emailSeg.Text = datiUte.Email
            '    Else
            '        txt_emailSeg.Text = consult.Email
            '    End If
            'Else
            '    txt_emailSeg.Text = eseguitaMaster.ReazioneAvversa.MailSegnalatore
            'End If
            If Not datiUte.Email Is Nothing AndAlso Not String.IsNullOrWhiteSpace(datiUte.Email) Then
                txt_emailSeg.Text = datiUte.Email
            ElseIf Not consult.Email Is Nothing AndAlso Not String.IsNullOrWhiteSpace(consult.Email) Then
                txt_emailSeg.Text = consult.Email
            Else
                txt_emailSeg.Text = eseguitaMaster.ReazioneAvversa.MailSegnalatore
            End If
        End If
        'Me.txt_emailSeg.Text = eseguitaMaster.ReazioneAvversa.MailSegnalatore
        Me.txtFirmaSegnalatore.Text = eseguitaMaster.ReazioneAvversa.FirmaSegnalatore

        LoadOrigineEtnica(eseguitaMaster)

    End Sub

    Public Function CreateListEseguiteReazioniSelezionate(listRows As List(Of DataRow), rowAssociazioneSelezionata As DataRow, isGestioneCentrale As Boolean) As List(Of Entities.VaccinazioneEseguita)

        Dim listEseguiteReazioniSelezionate As New List(Of Entities.VaccinazioneEseguita)()

        Dim now As DateTime = DateTime.Now

        If Not listRows.IsNullOrEmpty() Then

            For Each row As DataRow In listRows
                listEseguiteReazioniSelezionate.Add(Common.ReazioniAvverseCommon.CreateEseguitaReazioneByDataRow(row, now))
            Next

        End If

        If Not rowAssociazioneSelezionata Is Nothing Then

            If listEseguiteReazioniSelezionate.IsNullOrEmpty() Then

                ' Se non ci sono eseguite selezionate, creazione entità relative alla riga cliccata
                listEseguiteReazioniSelezionate.Add(Common.ReazioniAvverseCommon.CreateEseguitaReazioneByDataRow(rowAssociazioneSelezionata, now))

            Else

                ' Ordinamento per nome commerciale
                listEseguiteReazioniSelezionate = listEseguiteReazioniSelezionate.OrderBy(Function(p) p.noc_descrizione).ToList()

                ' L'entità relativa alla riga selezionata (se c'è) va sempre per prima
                Dim firstItem As Entities.VaccinazioneEseguita = Nothing

                If isGestioneCentrale Then
                    firstItem = listEseguiteReazioniSelezionate.FirstOrDefault(
                        Function(item) item.ves_id = rowAssociazioneSelezionata("ves_id") AndAlso item.ves_usl_inserimento = rowAssociazioneSelezionata("ves_usl_inserimento"))
                Else
                    firstItem = listEseguiteReazioniSelezionate.FirstOrDefault(
                        Function(item) item.ves_id = rowAssociazioneSelezionata("ves_id"))
                End If

                If Not firstItem Is Nothing Then
                    listEseguiteReazioniSelezionate.Remove(firstItem)
                    listEseguiteReazioniSelezionate.Insert(0, firstItem)
                End If

            End If

        End If

        If Not listEseguiteReazioniSelezionate.IsNullOrEmpty() Then

            Dim statoAnagrafico As Enumerators.StatoAnagrafico = Enumerators.StatoAnagrafico.RESIDENTE
            Dim dataDecesso As DateTime? = Nothing
            Dim sesso As String = String.Empty

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizPaziente As New Biz.BizPaziente(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos(), Nothing)

                    If isGestioneCentrale Then

                        ' N.B. : in centrale lo stato anagrafico non è in tabella, quindi resta il valore RESIDENTE.
                        '        Non è bello ma non è un problema perchè, nelle reazioni, serve solo sapere se è deceduto o no.

                        dataDecesso = bizPaziente.GetDataDecessoPaziente(OnVacUtility.Variabili.PazId, isGestioneCentrale)
                        sesso = bizPaziente.GetSessoPaziente(OnVacUtility.Variabili.PazId, isGestioneCentrale)

                    Else

                        Dim paziente As Entities.Paziente = bizPaziente.CercaPaziente(OnVacUtility.Variabili.PazId)

                        statoAnagrafico = paziente.StatoAnagrafico
                        dataDecesso = paziente.DataDecesso
                        sesso = paziente.Sesso

                    End If

                End Using
            End Using

            For Each eseguitaReazione As Entities.VaccinazioneEseguita In listEseguiteReazioniSelezionate
                eseguitaReazione.ReazioneAvversa.StatoAnagrafico = statoAnagrafico
                eseguitaReazione.ReazioneAvversa.DataDecesso = dataDecesso
                eseguitaReazione.ReazioneAvversa.SessoPaziente = sesso
            Next

        End If

        Return listEseguiteReazioniSelezionate

    End Function

    Public Function GetListEseguiteReazioniAvverse(controllaDati As Boolean) As List(Of Entities.VaccinazioneEseguita)

        If controllaDati Then

            Dim checkResult As Common.ReazioniAvverseCommon.CheckResult = CheckFields()

            If Not checkResult.Success Then
                OnShowMessage(checkResult.Message)
                Return Nothing
            End If

            If Me.dpkDataEsito.Data > DateTime.MinValue Then

                If Me.dpkDataEsito.Data < Me.dpkDataReazione.Data Then
                    OnShowMessage("La data di esito della reazione non può precedere la data di insorgenza della reazione!")
                    Return Nothing
                End If

                If Me.dpkDataEsito.Data > DateTime.Now.Date Then
                    OnShowMessage("La data di esito della reazione non può essere successiva alla data odierna!")
                    Return Nothing
                End If

            End If

            If Me.dpkDataReazione.Data < Me.ListaEseguiteReazioni(0).ves_dataora_effettuazione.Date Then
                OnShowMessage("La data di insorgenza della reazione non può precedere la data della vaccinazione!")
                Return Nothing
            End If

			If Me.dpkDataReazione.Data > DateTime.Now.Date Then
				OnShowMessage("La data di insorgenza della reazione non può essere successiva alla data odierna!")
				Return Nothing
			End If
			'' 
			If Settings.REAZIONE_AVVERSA_INTEGRAZIONE Then
				If rblCausa.SelectedValue = TipoCausaReazioneOsservata.Interazione AndAlso ListaEseguiteReazioni.Count = 1 Then
					OnShowMessage("Se la reazione osservata deriva da Interazione è necessario avere almeno due farmaci sospetti!")
					Return Nothing
				End If

			End If


		End If

			' Nel caso controllaDati==false, evito che scoppi non valorizzando i campi.
			Dim peso As Double? = Nothing
        If Not String.IsNullOrWhiteSpace(Me.txtPeso.Text) Then

            Dim value As Double
            If Double.TryParse(Me.txtPeso.Text, value) Then
                peso = value
            End If

        End If

        Dim altezza As Integer? = Nothing
        If Not String.IsNullOrWhiteSpace(Me.txtAltezza.Text) Then

            Dim value As Integer
            If Integer.TryParse(Me.txtAltezza.Text, value) Then
                altezza = value
            End If

        End If

        For i As Integer = 0 To Me.ListaEseguiteReazioni.Count - 1

            Dim vaccinazioneEseguita As Entities.VaccinazioneEseguita = Me.ListaEseguiteReazioni(i)

            vaccinazioneEseguita.ReazioneAvversa.DataReazione = Me.dpkDataReazione.Data

            vaccinazioneEseguita.ReazioneAvversa.Peso = peso
            vaccinazioneEseguita.ReazioneAvversa.Altezza = altezza

            If String.IsNullOrWhiteSpace(Me.dpkUltimaMestruazione.Text) Then
                vaccinazioneEseguita.ReazioneAvversa.DataUltimaMestruazione = Nothing
            Else
                vaccinazioneEseguita.ReazioneAvversa.DataUltimaMestruazione = Me.dpkUltimaMestruazione.Data
            End If

            vaccinazioneEseguita.ReazioneAvversa.Allattamento = Me.ddlAllattamento.SelectedValue
            vaccinazioneEseguita.ReazioneAvversa.Gravidanza = Me.rblGravidanza.SelectedValue
            vaccinazioneEseguita.ReazioneAvversa.CausaReazioneOsservata = Me.rblCausa.SelectedValue

            vaccinazioneEseguita.ReazioneAvversa.CodiceReazione = Me.fm_tipoReaz.Codice
            vaccinazioneEseguita.ReazioneAvversa.DescrizioneReazione = Me.fm_tipoReaz.Descrizione
            vaccinazioneEseguita.ReazioneAvversa.CodiceReazione1 = Me.fm_tipoReaz1.Codice
            vaccinazioneEseguita.ReazioneAvversa.DescrizioneReazione1 = Me.fm_tipoReaz1.Descrizione
            vaccinazioneEseguita.ReazioneAvversa.CodiceReazione2 = Me.fm_tipoReaz2.Codice
            vaccinazioneEseguita.ReazioneAvversa.DescrizioneReazione2 = Me.fm_tipoReaz2.Descrizione

            vaccinazioneEseguita.ReazioneAvversa.Terapie = Me.tb_Terapie.Text
            vaccinazioneEseguita.ReazioneAvversa.AltraReazione = Me.tb_altroReaz.Text

            If Me.rb_grave.Checked Then
                vaccinazioneEseguita.ReazioneAvversa.Grave = "S"
                If Me.rb_decesso.Checked Then
                    vaccinazioneEseguita.ReazioneAvversa.GravitaReazione = GravitaReazione.Decesso
                ElseIf Me.rb_ospedalizz.Checked Then
                    vaccinazioneEseguita.ReazioneAvversa.GravitaReazione = GravitaReazione.Ospedalizzazione
                ElseIf Me.rb_invalidita.Checked Then
                    vaccinazioneEseguita.ReazioneAvversa.GravitaReazione = GravitaReazione.Invalidita
                ElseIf Me.rb_pericolo.Checked Then
                    vaccinazioneEseguita.ReazioneAvversa.GravitaReazione = GravitaReazione.PericoloVita
                ElseIf Me.rb_anomalie.Checked Then
                    vaccinazioneEseguita.ReazioneAvversa.GravitaReazione = GravitaReazione.Anomalie
                ElseIf Me.rb_altro.Checked Then
                    vaccinazioneEseguita.ReazioneAvversa.GravitaReazione = GravitaReazione.AltraCondizione
                End If
            Else
                vaccinazioneEseguita.ReazioneAvversa.Grave = "N"
                vaccinazioneEseguita.ReazioneAvversa.GravitaReazione = String.Empty
            End If

            vaccinazioneEseguita.ReazioneAvversa.Esito = String.Empty
            vaccinazioneEseguita.ReazioneAvversa.DataEsito = Date.MinValue
            vaccinazioneEseguita.ReazioneAvversa.MotivoDecesso = String.Empty

            If Me.rb_risCompleta.Checked Then
                vaccinazioneEseguita.ReazioneAvversa.Esito = Esito.RisoluzioneCompleta
                vaccinazioneEseguita.ReazioneAvversa.DataEsito = Me.dpkDataEsito.Data
            ElseIf Me.rb_risPostumi.Checked Then
                vaccinazioneEseguita.ReazioneAvversa.Esito = Esito.RisoluzioneConPostumi
            ElseIf Me.rb_miglioramento.Checked Then
                vaccinazioneEseguita.ReazioneAvversa.Esito = Esito.Miglioramento
            ElseIf Me.rb_reazioneInvariata.Checked Then
                vaccinazioneEseguita.ReazioneAvversa.Esito = Esito.ReazioneInvariata
            ElseIf Me.rb_EsitoDecesso.Checked Then
                vaccinazioneEseguita.ReazioneAvversa.Esito = Esito.DataDecesso
                vaccinazioneEseguita.ReazioneAvversa.DataEsito = Me.dpkDataEsito.Data
                If Me.rb_reazAvversa.Checked Then
                    vaccinazioneEseguita.ReazioneAvversa.MotivoDecesso = MotivoDecesso.ReazioneAvversa
                ElseIf Me.rb_farmaco.Checked Then
                    vaccinazioneEseguita.ReazioneAvversa.MotivoDecesso = MotivoDecesso.Farmaco
                ElseIf Me.rb_noFarmaco.Checked Then
                    vaccinazioneEseguita.ReazioneAvversa.MotivoDecesso = MotivoDecesso.NoFarmaco
                ElseIf Me.rb_sconosciuta.Checked Then
                    vaccinazioneEseguita.ReazioneAvversa.MotivoDecesso = MotivoDecesso.CausaSconosciuta
                End If
            ElseIf Me.rb_noDisponibile.Checked Then
                vaccinazioneEseguita.ReazioneAvversa.Esito = Esito.NonDisponibile
            End If

            vaccinazioneEseguita.ReazioneAvversa.VisiteRicoveri = Me.tb_VisRic.Text

            SetDatiFarmacoSospetto(vaccinazioneEseguita, i)

            SetDatiFarmacoConcomitante(vaccinazioneEseguita, 0)
            SetDatiFarmacoConcomitante(vaccinazioneEseguita, 1)

            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante = Me.ddlFarmaciConcomitanti.SelectedValue
            'vaccinazioneEseguita.ReazioneAvversa.FarmacoDescrizione => non più gestito
            vaccinazioneEseguita.ReazioneAvversa.UsoConcomitante = Me.tb_altriFarmaci.Text
            vaccinazioneEseguita.ReazioneAvversa.CondizioniConcomitanti = Me.tb_condConcomitanti.Text
            vaccinazioneEseguita.ReazioneAvversa.AltreInformazioni = Me.txtAltreInfo.Text

            vaccinazioneEseguita.ReazioneAvversa.Qualifica = Me.rbl_qualifica.SelectedValue
            vaccinazioneEseguita.ReazioneAvversa.AltraQualifica = Me.ddl_altroSeg.SelectedValue
            vaccinazioneEseguita.ReazioneAvversa.CognomeSegnalatore = Me.txt_CognomeSeg.Text
            vaccinazioneEseguita.ReazioneAvversa.NomeSegnalatore = Me.txt_nomeSeg.Text
            vaccinazioneEseguita.ReazioneAvversa.IndirizzoSegnalatore = Me.txt_indirizzoSeg.Text
            vaccinazioneEseguita.ReazioneAvversa.TelSegnalatore = Me.txt_telSeg.Text
            vaccinazioneEseguita.ReazioneAvversa.FaxSegnalatore = Me.txt_faxSeg.Text
            vaccinazioneEseguita.ReazioneAvversa.MailSegnalatore = Me.txt_emailSeg.Text
            vaccinazioneEseguita.ReazioneAvversa.FirmaSegnalatore = Me.txtFirmaSegnalatore.Text

            vaccinazioneEseguita.ReazioneAvversa.DataModifica = DateTime.Now
            vaccinazioneEseguita.ReazioneAvversa.CodiceUslInserimento = Me.txtCodiceUslAppartenenzaReazione.Value
            vaccinazioneEseguita.ReazioneAvversa.CodiceOrigineEtnica = ddlOrigineEtnica.SelectedValue

            SetAmbitoOsservazioneFromControl(vaccinazioneEseguita)

        Next

        Return Me.ListaEseguiteReazioni

    End Function

    Private Function CheckFields() As Common.ReazioniAvverseCommon.CheckResult

        ' Controllo campi di tipo String
        Dim stringFieldList As List(Of Entities.FieldToCheck(Of String)) = Me.GetStringFieldList()

        If Me.ucFarmacoSospetto1.Visible Then AddCheckFieldsToList(Me.ucFarmacoSospetto1.GetStringFieldsToCheck(), stringFieldList)
        If Me.ucFarmacoSospetto2.Visible Then AddCheckFieldsToList(Me.ucFarmacoSospetto2.GetStringFieldsToCheck(), stringFieldList)
        If Me.ucFarmacoSospetto3.Visible Then AddCheckFieldsToList(Me.ucFarmacoSospetto3.GetStringFieldsToCheck(), stringFieldList)
        If Me.ucFarmacoConcomitante1.Visible Then AddCheckFieldsToList(Me.ucFarmacoConcomitante1.GetStringFieldsToCheck(), stringFieldList)

        Dim stringResult As Biz.BizClass.CheckResult(Of String) = Biz.BizVaccinazioniEseguite.CheckFields(stringFieldList)

        ' Controllo campi di tipo DateTime
        Dim dateTimeFieldList As List(Of Entities.FieldToCheck(Of DateTime)) = Me.GetDateTimeFieldList()

        If Me.ucFarmacoSospetto1.Visible Then AddCheckFieldsToList(Me.ucFarmacoSospetto1.GetDateTimeFieldsToCheck(), dateTimeFieldList)
        If Me.ucFarmacoSospetto2.Visible Then AddCheckFieldsToList(Me.ucFarmacoSospetto2.GetDateTimeFieldsToCheck(), dateTimeFieldList)
        If Me.ucFarmacoSospetto3.Visible Then AddCheckFieldsToList(Me.ucFarmacoSospetto3.GetDateTimeFieldsToCheck(), dateTimeFieldList)
        If Me.ucFarmacoConcomitante1.Visible Then AddCheckFieldsToList(Me.ucFarmacoConcomitante1.GetDateTimeFieldsToCheck(), dateTimeFieldList)

        Dim dateTimeResult As Biz.BizClass.CheckResult(Of DateTime) = Biz.BizVaccinazioniEseguite.CheckFields(dateTimeFieldList)

        ' Descrizione campi obbligatori non compilati
        Dim listaCampiObbligatoriNonCompilati As New List(Of String)()

        If Not stringResult.ListaCampiObbligatoriNonCompilati.IsNullOrEmpty() Then
            listaCampiObbligatoriNonCompilati.AddRange(stringResult.ListaCampiObbligatoriNonCompilati.Select(Function(p) p.Description))
        End If

        If Not dateTimeResult.ListaCampiObbligatoriNonCompilati.IsNullOrEmpty() Then
            listaCampiObbligatoriNonCompilati.AddRange(dateTimeResult.ListaCampiObbligatoriNonCompilati.Select(Function(p) p.Description))
        End If

        ' Descrizione campi che superano la lunghezza massima
        Dim listaCampiLunghezzaMassimaSuperata As New List(Of String)()

        If Not stringResult.ListaCampiLunghezzaMassimaSuperata.IsNullOrEmpty() Then
            listaCampiLunghezzaMassimaSuperata.AddRange(stringResult.ListaCampiLunghezzaMassimaSuperata.Select(Function(p) String.Format("{0} [max {1}]", p.Description, p.MaxLength.ToString())))
        End If

        If Not dateTimeResult.ListaCampiLunghezzaMassimaSuperata.IsNullOrEmpty() Then
            listaCampiLunghezzaMassimaSuperata.AddRange(dateTimeResult.ListaCampiLunghezzaMassimaSuperata.Select(Function(p) String.Format("{0} [max {1}]", p.Description, p.MaxLength.ToString())))
        End If

        ' Controlli validità campi
        Dim listaCampiNonValidi As New List(Of String)()

        Dim peso As Double? = Nothing
        If Not String.IsNullOrWhiteSpace(Me.txtPeso.Text) Then

            Dim value As Double
            If Not Double.TryParse(Me.txtPeso.Text, value) Then
                listaCampiNonValidi.Add(Me.lblPeso.Text)
            End If

        End If

        Dim altezza As Integer? = Nothing
        If Not String.IsNullOrWhiteSpace(Me.txtAltezza.Text) Then

            Dim value As Integer
            If Not Integer.TryParse(Me.txtAltezza.Text, value) Then
                listaCampiNonValidi.Add(Me.lblAltezza.Text)
            End If
        End If

        ' Messaggio
        Dim message As New System.Text.StringBuilder()

        If listaCampiObbligatoriNonCompilati.Count > 0 Then
            message.AppendLine("Non sono stati valorizzati i seguenti campi obbligatori:")
            message.Append(String.Join(Environment.NewLine, listaCampiObbligatoriNonCompilati.ToArray()))
        End If

        If listaCampiLunghezzaMassimaSuperata.Count > 0 Then
            If message.Length > 0 Then message.AppendLine().AppendLine()
            message.AppendLine("I valori presenti nei seguenti campi superano la lunghezza massima consentita:")
            message.Append(String.Join(Environment.NewLine, listaCampiLunghezzaMassimaSuperata.ToArray()))
        End If

        If listaCampiNonValidi.Count > 0 Then
            If message.Length > 0 Then message.AppendLine().AppendLine()
            message.AppendLine("I seguenti campi contengono valori non validi:")
            message.Append(String.Join(Environment.NewLine, listaCampiNonValidi.ToArray()))
        End If

        Return New Common.ReazioniAvverseCommon.CheckResult(stringResult.Success And dateTimeResult.Success, message.ToString())

    End Function

#End Region

#Region " RadioButtonList Events "

    Private Sub rbl_qualifica_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles rbl_qualifica.SelectedIndexChanged

        If Me.rbl_qualifica.SelectedValue = QualificaSegnalatore.Altro Then
            Me.ddl_altroSeg.Enabled = True
            Me.ddl_altroSeg.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Stringa_Obbligatorio
        Else
            Me.ddl_altroSeg.Enabled = False
            Me.ddl_altroSeg.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Stringa
        End If

    End Sub

#End Region

#Region " Eventi Change delle modali tipoReaz "

    ' Al change di una modale va impostato il filtro delle altre due in modo da non visualizzare la reazione
    ' già selezionata nell'elenco.

    Private Sub fm_tipoReaz_Change(sender As Object, e As Onit.Controls.OnitModalList.ModalListaEventArgument) Handles fm_tipoReaz.Change

        SetFilterTipoReaz(Me.fm_tipoReaz, Me.fm_tipoReaz1, Me.fm_tipoReaz2)

    End Sub

    Private Sub fm_tipoReaz1_Change(sender As Object, e As Onit.Controls.OnitModalList.ModalListaEventArgument) Handles fm_tipoReaz1.Change

        SetFilterTipoReaz(Me.fm_tipoReaz1, Me.fm_tipoReaz, Me.fm_tipoReaz2)

    End Sub

    Private Sub fm_tipoReaz2_Change(sender As Object, e As Onit.Controls.OnitModalList.ModalListaEventArgument) Handles fm_tipoReaz2.Change

        SetFilterTipoReaz(Me.fm_tipoReaz2, Me.fm_tipoReaz, Me.fm_tipoReaz1)

    End Sub

    ' tipoReazChanged è la modale che ha scatenato l'evento changed
    ' tipoReaz1 e tipoReaz2 sono le due modali a cui impostare il filtro
    Private Sub SetFilterTipoReaz(tipoReazChanged As Onit.Controls.OnitModalList, ByRef tipoReaz1 As Onit.Controls.OnitModalList, ByRef tipoReaz2 As Onit.Controls.OnitModalList)

        tipoReaz1.Filtro = GetFiltroBaseReazione()
        tipoReaz2.Filtro = GetFiltroBaseReazione()

        ' Imposto tipoReaz1 e tipoReaz2 in base a tipoReazChanged (se c'è)
        If Not String.IsNullOrWhiteSpace(tipoReazChanged.Codice) Then
            tipoReaz1.Filtro += GetFiltroCodiceReazione(tipoReazChanged.Codice)
            tipoReaz2.Filtro += GetFiltroCodiceReazione(tipoReazChanged.Codice)
        End If

        ' Imposto il filtro di tipoReaz1 in base a tipoReaz2 (se c'è)
        If Not String.IsNullOrWhiteSpace(tipoReaz2.Codice) Then
            tipoReaz1.Filtro += GetFiltroCodiceReazione(tipoReaz2.Codice)
        End If

        ' Imposto il filtro di tipoReaz2 in base a tipoReaz1 (se c'è)
        If Not String.IsNullOrWhiteSpace(tipoReaz1.Codice) Then
            tipoReaz2.Filtro += GetFiltroCodiceReazione(tipoReaz1.Codice)
        End If

        ' Aggiungo l'ordinamento per descrizione
        tipoReaz1.Filtro += GetOrderByReazione()
        tipoReaz2.Filtro += GetOrderByReazione()

    End Sub

#End Region

#Region " Eventi DropDownList "

    Protected Sub ddlFarmaciConcomitanti_SelectedIndexChanged(sender As Object, e As EventArgs)

        ManageFarmaciConcomitanti(Me.ListaEseguiteReazioni(0))

    End Sub

    Private Sub ManageFarmaciConcomitanti(vaccinazioneEseguita As Entities.VaccinazioneEseguita)

        Dim enable As Boolean = (Me.StatoControlloCorrente = StatoControllo.Inserimento Or Me.StatoControlloCorrente = StatoControllo.Modifica)

        If Me.ddlFarmaciConcomitanti.SelectedValue = "S" Then
            Me.btnRecuperaConcomitanti.Enabled = enable
            Me.ucFarmacoConcomitante1.Inizializza(GetInfoFarmacoConcomitante1(vaccinazioneEseguita), enable)
            Me.ucFarmacoConcomitante1.Visible = True
            Me.ucFarmacoConcomitante2.Inizializza(GetInfoFarmacoConcomitante2(vaccinazioneEseguita), enable)
            Me.ucFarmacoConcomitante2.Visible = True
        Else
            Me.btnRecuperaConcomitanti.Enabled = False
            Me.ucFarmacoConcomitante1.Inizializza(Nothing, False)
            Me.ucFarmacoConcomitante1.Visible = False
            Me.ucFarmacoConcomitante2.Inizializza(Nothing, False)
            Me.ucFarmacoConcomitante2.Visible = False
        End If

    End Sub

#End Region

#Region " Eventi Button "

    Private Sub btnRecuperaConcomitanti_Click(sender As Object, e As EventArgs) Handles btnRecuperaConcomitanti.Click

        Me.lblMesiRecupero.Text = String.Format("Mesi precedenti la data di effettuazione ({0:dd/MM/yyyy}):", Me.ListaEseguiteReazioni(0).ves_dataora_effettuazione)
        Me.txtMesiRecupero.Text = "1"
        Me.btnRefreshRecupero.ImageUrl = ResolveClientUrl("~/images/refresh.png")

        Me.fmRecuperaConcomitanti.VisibileMD = True

        BindFarmaciRecuperabili()

    End Sub

    Private Sub btnRefreshRecupero_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles btnRefreshRecupero.Click

        BindFarmaciRecuperabili()

    End Sub

    Private Sub BindFarmaciRecuperabili()

        ' Sollevo l'evento per far sì che i possibili concomitanti mi vengano forniti dal padre dello user control
        Dim farmaciRecuperabili As List(Of Common.ReazioniAvverseCommon.FarmacoRecupero) = OnRecuperaConcomitanti()

        ' Creo la lista dei farmaci sospetti presenti nello user control
        Dim farmaciSospetti As New List(Of Common.ReazioniAvverseCommon.FarmacoInfo)()

        If Me.ucFarmacoSospetto1.Visible Then
            farmaciSospetti.Add(Me.ucFarmacoSospetto1.GetFarmacoInfo())
        End If

        If Me.ucFarmacoSospetto2.Visible Then
            farmaciSospetti.Add(Me.ucFarmacoSospetto2.GetFarmacoInfo())
        End If

        If Me.ucFarmacoSospetto3.Visible Then
            farmaciSospetti.Add(Me.ucFarmacoSospetto3.GetFarmacoInfo())
        End If

        Dim mesi As Integer = 1

        ' Imposto i mesi di ricerca dei concomitanti in base al valore inserito dall'utente
        If String.IsNullOrWhiteSpace(Me.txtMesiRecupero.Text) Then

            Me.txtMesiRecupero.Text = "1"
            mesi = 1

        Else

            If Not Integer.TryParse(Me.txtMesiRecupero.Text, mesi) Then
                Me.txtMesiRecupero.Text = "1"
                mesi = 1
            ElseIf mesi <= 0 Then
                Me.txtMesiRecupero.Text = "1"
                mesi = 1
            End If

        End If

        Dim sourceList As List(Of Common.ReazioniAvverseCommon.FarmacoRecupero) = Nothing

        If farmaciRecuperabili.IsNullOrEmpty() Then
            sourceList = New List(Of Common.ReazioniAvverseCommon.FarmacoRecupero)()
        Else
            sourceList = farmaciRecuperabili.
                Where(Function(rec) Not rec.IsFittizia AndAlso
                          rec.DataOraEsecuzioneVaccinazione.Date <= Me.ListaEseguiteReazioni(0).ves_dataora_effettuazione.Date AndAlso
                          rec.DataOraEsecuzioneVaccinazione.Date >= Me.ListaEseguiteReazioni(0).ves_dataora_effettuazione.Date.AddMonths(-mesi) AndAlso
                          Not farmaciSospetti.Any(Function(sosp) sosp.VesId.HasValue AndAlso sosp.VesId.Value = rec.VesId)).
                OrderByDescending(Function(p) p.DataOraEsecuzioneVaccinazione).
                ThenBy(Function(p) p.DescrizioneAssociazione).
                ThenByDescending(Function(p) p.DoseAssociazione).ToList()
        End If

        Me.lblNoResult.Visible = sourceList.IsNullOrEmpty()

        Me.dgrRecupero.DataSource = sourceList
        Me.dgrRecupero.DataBind()

    End Sub

#End Region

#Region " Eventi User Controls "

    Private Sub ucFarmacoSospetto1_ReplicaFarmaco(sender As Object, e As Common.ReazioniAvverseCommon.ReplicaEventArgs) Handles ucFarmacoSospetto1.ReplicaFarmaco

        If Me.ucFarmacoSospetto2.Visible Then
            Me.ucFarmacoSospetto2.ReplicaInfoFarmaco(e.FarmacoDaReplicare)
        End If

        If Me.ucFarmacoSospetto3.Visible Then
            Me.ucFarmacoSospetto3.ReplicaInfoFarmaco(e.FarmacoDaReplicare)
        End If

    End Sub

    Private Sub ucFarmacoConcomitante1_ReplicaFarmaco(sender As Object, e As Common.ReazioniAvverseCommon.ReplicaEventArgs) Handles ucFarmacoConcomitante1.ReplicaFarmaco

        If Me.ucFarmacoConcomitante2.Visible Then
            Me.ucFarmacoConcomitante2.ReplicaInfoFarmaco(e.FarmacoDaReplicare)
        End If

    End Sub

#End Region

#Region " Eventi Toolbar "

    Private Sub tlbRecupero_ButtonClicked(sender As Object, be As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles tlbRecupero.ButtonClicked

        Select Case be.Button.Key

            Case "btnConfermaRecupero"

                ConfermaRecupero()

        End Select

    End Sub

    Private Sub ConfermaRecupero()

        Dim list As New List(Of Common.ReazioniAvverseCommon.FarmacoInfo)()

        Dim ordinal As Integer = 0

        For i As Integer = 0 To Me.dgrRecupero.Items.Count - 1

            If ordinal >= 2 Then
                OnShowMessage("E' possibile selezionare al massimo 2 farmaci concomitanti.")
                Return
            End If

            If DirectCast(Me.dgrRecupero.Items(i).FindControl("cb"), CheckBox).Checked Then

                ordinal += 1
                list.Add(CreateFarmacoConcomitanteFromDataGridItem(Me.dgrRecupero.Items(i), ordinal))

            End If

        Next

        If Not list.IsNullOrEmpty() Then

            Dim enable As Boolean = (Me.StatoControlloCorrente = StatoControllo.Inserimento Or Me.StatoControlloCorrente = StatoControllo.Modifica)

            ' I concomitanti vanno sempre mostrati entrambi, anche se ne è stato selezionato solo 1.
            Me.ucFarmacoConcomitante1.Visible = True
            Me.ucFarmacoConcomitante2.Visible = True

            Me.ucFarmacoConcomitante1.Inizializza(list(0), enable)

            If list.Count = 1 Then
                Me.ucFarmacoConcomitante2.Inizializza(CreateFarmacoConcomitanteFromDataGridItem(Nothing, 2), enable)
            ElseIf list.Count = 2 Then
                Me.ucFarmacoConcomitante2.Inizializza(list(1), enable)
            End If

        End If

        Me.fmRecuperaConcomitanti.VisibileMD = False

    End Sub

    Private Function CreateFarmacoConcomitanteFromDataGridItem(selectedItem As DataGridItem, ordinal As Integer) As Common.ReazioniAvverseCommon.FarmacoInfo

        Dim farmacoInfo As New Common.ReazioniAvverseCommon.FarmacoInfo()

        farmacoInfo.Ordinal = ordinal

        If Not selectedItem Is Nothing Then

            farmacoInfo.DescrizioneNomeCommerciale = HttpUtility.HtmlDecode(selectedItem.Cells(GridRecuperoColumnIndex.DescrizioneNomeCommerciale).Text)
            farmacoInfo.CodiceLotto = HttpUtility.HtmlDecode(selectedItem.Cells(GridRecuperoColumnIndex.CodiceLotto).Text)
            farmacoInfo.CodiceSitoInoculazione = HttpUtility.HtmlDecode(selectedItem.Cells(GridRecuperoColumnIndex.CodiceSitoInoculazione).Text)
            farmacoInfo.DescrizioneSitoInoculazione = HttpUtility.HtmlDecode(selectedItem.Cells(GridRecuperoColumnIndex.DescrizioneSitoInoculazione).Text)
            farmacoInfo.CodiceViaSomministrazione = HttpUtility.HtmlDecode(selectedItem.Cells(GridRecuperoColumnIndex.CodiceViaSomministrazione).Text)
            farmacoInfo.DescrizioneViaSomministrazione = HttpUtility.HtmlDecode(selectedItem.Cells(GridRecuperoColumnIndex.DescrizioneViaSomministrazione).Text)

            Dim value As String = HttpUtility.HtmlDecode(selectedItem.Cells(GridRecuperoColumnIndex.DataOraEsecuzioneVaccinazione).Text)
            If String.IsNullOrWhiteSpace(value) Then
                farmacoInfo.DataOraEsecuzioneVaccinazione = Nothing
            Else
                farmacoInfo.DataOraEsecuzioneVaccinazione = Convert.ToDateTime(value)
            End If

            value = HttpUtility.HtmlDecode(selectedItem.Cells(GridRecuperoColumnIndex.DoseAssociazione).Text)
            If String.IsNullOrWhiteSpace(value) Then
                farmacoInfo.Dose = Nothing
            Else
                farmacoInfo.Dose = Convert.ToInt32(value)
            End If

            value = HttpUtility.HtmlDecode(selectedItem.Cells(GridRecuperoColumnIndex.VesId).Text)
            If String.IsNullOrWhiteSpace(value) Then
                farmacoInfo.VesId = Nothing
            Else
                farmacoInfo.VesId = Convert.ToInt64(value)
            End If
            value = HttpUtility.HtmlDecode(selectedItem.Cells(GridRecuperoColumnIndex.DataScadenzaLotto).Text)
            If String.IsNullOrWhiteSpace(value) Then
                farmacoInfo.DataScadLotto = Nothing
            Else
                farmacoInfo.DataScadLotto = Convert.ToDateTime(value)
            End If

            farmacoInfo.CodiceNomeCommerciale = HttpUtility.HtmlDecode(selectedItem.Cells(GridRecuperoColumnIndex.CodiceNomeCommerciale).Text)
            farmacoInfo.DosaggioFiale = String.Empty
            farmacoInfo.Indicazioni = String.Empty
            farmacoInfo.CodiceIndicazioni = String.Empty
            farmacoInfo.Richiamo = Nothing
            farmacoInfo.Sospeso = String.Empty
            farmacoInfo.ReazioneMigliorata = String.Empty
            farmacoInfo.Ripreso = String.Empty
            farmacoInfo.RicomparsiSintomi = String.Empty

        End If

        Return farmacoInfo

    End Function

#End Region

#Region " Private Methods "

#Region " Valorizzazione dati "

    Private Sub LoadDataIntoGravitaReazione(vaccinazioneEseguita As Entities.VaccinazioneEseguita)

        Me.rb_decesso.Enabled = Me.IsPazienteDeceduto

        Me.rb_grave.Checked = False
        Me.rb_noGrave.Checked = False

        If vaccinazioneEseguita.ReazioneAvversa.Grave = "S" Then
            If Not vaccinazioneEseguita.ReazioneAvversa.GravitaReazione Is DBNull.Value Then
                Select Case vaccinazioneEseguita.ReazioneAvversa.GravitaReazione
                    Case GravitaReazione.Decesso
                        Me.rb_decesso.Checked = True
                    Case GravitaReazione.Ospedalizzazione
                        Me.rb_ospedalizz.Checked = True
                    Case GravitaReazione.Invalidita
                        Me.rb_invalidita.Checked = True
                    Case GravitaReazione.PericoloVita
                        Me.rb_pericolo.Checked = True
                    Case GravitaReazione.Anomalie
                        Me.rb_anomalie.Checked = True
                    Case GravitaReazione.AltraCondizione
                        Me.rb_altro.Checked = True
                End Select
            End If
            Me.rb_grave.Checked = True
            Me.GravitaReazioneSetEnabled(Me.rb_grave.ID)
        Else
            Me.rb_noGrave.Checked = True
            Me.GravitaReazioneSetEnabled(Me.rb_noGrave.ID)
        End If

    End Sub

    Private Sub LoadDataIntoEsito(vaccinazioneEseguita As Entities.VaccinazioneEseguita)

        Me.rb_EsitoDecesso.Enabled = Me.IsPazienteDeceduto

        Me.dpkDataEsito.Enabled = False
        Me.dpkDataEsito.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Data_Disabilitato
        Me.dpkDataEsito.Text = String.Empty

        Me.rb_risCompleta.Checked = False
        Me.rb_risPostumi.Checked = False
        Me.rb_miglioramento.Checked = False
        Me.rb_reazioneInvariata.Checked = False
        Me.rb_EsitoDecesso.Checked = False
        Me.rb_noDisponibile.Checked = False

        If Not vaccinazioneEseguita.ReazioneAvversa.Esito Is DBNull.Value Then

            Select Case vaccinazioneEseguita.ReazioneAvversa.Esito

                Case Esito.RisoluzioneCompleta
                    Me.dpkDataEsito.Text = vaccinazioneEseguita.ReazioneAvversa.DataEsito
                    Me.rb_risCompleta.Checked = True
                    Me.EsitoSetEnabled(Me.rb_risCompleta.ID)

                Case Esito.RisoluzioneConPostumi
                    Me.rb_risPostumi.Checked = True
                    Me.EsitoSetEnabled(Me.rb_risPostumi.ID)

                Case Esito.Miglioramento
                    Me.rb_miglioramento.Checked = True
                    Me.EsitoSetEnabled(Me.rb_miglioramento.ID)

                Case Esito.ReazioneInvariata
                    Me.rb_reazioneInvariata.Checked = True
                    Me.EsitoSetEnabled(Me.rb_reazioneInvariata.ID)

                Case Esito.DataDecesso
                    Me.dpkDataEsito.Text = vaccinazioneEseguita.ReazioneAvversa.DataEsito
                    Me.rb_EsitoDecesso.Checked = True
                    Me.EsitoSetEnabled(Me.rb_EsitoDecesso.ID)

                    If Not vaccinazioneEseguita.ReazioneAvversa.MotivoDecesso Is DBNull.Value Then
                        Select Case vaccinazioneEseguita.ReazioneAvversa.MotivoDecesso
                            Case MotivoDecesso.ReazioneAvversa
                                Me.rb_reazAvversa.Checked = True
                            Case MotivoDecesso.Farmaco
                                Me.rb_farmaco.Checked = True
                            Case MotivoDecesso.NoFarmaco
                                Me.rb_noFarmaco.Checked = True
                            Case MotivoDecesso.CausaSconosciuta
                                Me.rb_sconosciuta.Checked = True
                        End Select
                    End If

                Case Esito.NonDisponibile
                    Me.rb_noDisponibile.Checked = True
                    Me.EsitoSetEnabled(Me.rb_noDisponibile.ID)

            End Select
        End If

    End Sub

    Private Sub LoadDataIntoQualifica(vaccinazioneEseguita As Entities.VaccinazioneEseguita)

        If Not String.IsNullOrWhiteSpace(vaccinazioneEseguita.ReazioneAvversa.Qualifica) Then
            Me.rbl_qualifica.SelectedValue = vaccinazioneEseguita.ReazioneAvversa.Qualifica
        End If

        ' Se è selezionata ALTRA QUALIFICA devo abilitare la ddl AltraQualifica
        If Me.rbl_qualifica.SelectedValue = QualificaSegnalatore.Altro Then
            Me.ddl_altroSeg.Enabled = True
            Me.ddl_altroSeg.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Stringa_Obbligatorio
            Me.ddl_altroSeg.SelectedValue = vaccinazioneEseguita.ReazioneAvversa.AltraQualifica
        Else
            Me.ddl_altroSeg.Enabled = False
            Me.ddl_altroSeg.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Stringa
            Me.ddl_altroSeg.SelectedValue = String.Empty
        End If

    End Sub
    ''' <summary>
    ''' Recupero dati da tabelle Origini Etniche
    ''' </summary>
    Private Sub LoadOrigineEtnica(vaccinazioneEseguita As Entities.VaccinazioneEseguita)
        Dim tableOrigineEtniche As DataTable = Nothing

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
            Using bizAnagraficheOrigineEtnica As New Biz.BizAnagrafiche(genericProvider, Me.Settings, OnVacContext.CreateBizContextInfos())
                tableOrigineEtniche = bizAnagraficheOrigineEtnica.GetTableAnagraficaOrigineEtniche("")
            End Using
        End Using
        ddlOrigineEtnica.DataSource = tableOrigineEtniche
        ddlOrigineEtnica.DataBind()
        ddlOrigineEtnica.Items.Insert(0, String.Empty)
        If Not vaccinazioneEseguita.CodiceOrigineEtnica.IsNullOrEmpty Then
            ddlOrigineEtnica.SelectedValue = vaccinazioneEseguita.CodiceOrigineEtnica
        End If
    End Sub

    Private Sub LoadDataIntoGravidanza(vaccinazioneEseguita As Entities.VaccinazioneEseguita)

        Me.rblGravidanza.ClearSelection()

        If String.IsNullOrWhiteSpace(vaccinazioneEseguita.ReazioneAvversa.Gravidanza) Then
            vaccinazioneEseguita.ReazioneAvversa.Gravidanza = ValoreGravidanza.No
        End If

        Me.rblGravidanza.SelectedValue = vaccinazioneEseguita.ReazioneAvversa.Gravidanza

    End Sub

    Private Sub LoadDataIntoCausa(vaccinazioneEseguita As Entities.VaccinazioneEseguita)

        Me.rblCausa.ClearSelection()

        If String.IsNullOrWhiteSpace(vaccinazioneEseguita.ReazioneAvversa.CausaReazioneOsservata) Then
            vaccinazioneEseguita.ReazioneAvversa.CausaReazioneOsservata = TipoCausaReazioneOsservata.Nessuna
        End If

        Me.rblCausa.SelectedValue = vaccinazioneEseguita.ReazioneAvversa.CausaReazioneOsservata

    End Sub

    Private Sub LoadDataIntoReazioni(vaccinazioneEseguita As Entities.VaccinazioneEseguita)

        Me.fm_tipoReaz.Filtro = GetFiltroBaseReazione()
        Me.fm_tipoReaz1.Filtro = GetFiltroBaseReazione()
        Me.fm_tipoReaz2.Filtro = GetFiltroBaseReazione()

        Me.fm_tipoReaz.Codice = vaccinazioneEseguita.ReazioneAvversa.CodiceReazione
        Me.fm_tipoReaz.Descrizione = vaccinazioneEseguita.ReazioneAvversa.DescrizioneReazione

        If vaccinazioneEseguita.ReazioneAvversa.CodiceReazione <> "" Then

            Me.fm_tipoReaz.RefreshDataBind()

            Dim filtroCodiceReazione As String = GetFiltroCodiceReazione(Me.fm_tipoReaz.Codice)

            Me.fm_tipoReaz1.Filtro += filtroCodiceReazione
            Me.fm_tipoReaz2.Filtro += filtroCodiceReazione

        Else
            Me.fm_tipoReaz.Descrizione = String.Empty
        End If

        Me.fm_tipoReaz1.Codice = vaccinazioneEseguita.ReazioneAvversa.CodiceReazione1
        Me.fm_tipoReaz1.Descrizione = vaccinazioneEseguita.ReazioneAvversa.DescrizioneReazione1

        If vaccinazioneEseguita.ReazioneAvversa.CodiceReazione1 <> "" Then

            Me.fm_tipoReaz1.RefreshDataBind()

            Dim filtroCodiceReazione As String = GetFiltroCodiceReazione(Me.fm_tipoReaz1.Codice)

            Me.fm_tipoReaz.Filtro += filtroCodiceReazione
            Me.fm_tipoReaz2.Filtro += filtroCodiceReazione

        Else
            Me.fm_tipoReaz1.Descrizione = String.Empty
        End If

        Me.fm_tipoReaz2.Codice = vaccinazioneEseguita.ReazioneAvversa.CodiceReazione2
        Me.fm_tipoReaz2.Descrizione = vaccinazioneEseguita.ReazioneAvversa.DescrizioneReazione2

        If vaccinazioneEseguita.ReazioneAvversa.CodiceReazione2 <> "" Then

            Me.fm_tipoReaz2.RefreshDataBind()

            Dim filtroCodiceReazione As String = GetFiltroCodiceReazione(Me.fm_tipoReaz2.Codice)

            Me.fm_tipoReaz.Filtro += filtroCodiceReazione
            Me.fm_tipoReaz1.Filtro += filtroCodiceReazione

        Else
            Me.fm_tipoReaz2.Descrizione = String.Empty
        End If

        Me.fm_tipoReaz.Filtro += GetOrderByReazione()
        Me.fm_tipoReaz1.Filtro += GetOrderByReazione()
        Me.fm_tipoReaz2.Filtro += GetOrderByReazione()

        Me.tb_altroReaz.Text = vaccinazioneEseguita.ReazioneAvversa.AltraReazione.ToString()

    End Sub

    ' Creazione farmaci sospetti
    Private Sub LoadDataIntoFarmaciSospetti(listEseguiteReazioni As List(Of Entities.VaccinazioneEseguita))

        Dim enable As Boolean = (Me.StatoControlloCorrente = StatoControllo.Inserimento Or Me.StatoControlloCorrente = StatoControllo.Modifica)

        Dim impostazioniFarmaco As ImpostazioniFarmaco = GetImpostazioniFarmacoSospetto(listEseguiteReazioni, 0)

        Me.ucFarmacoSospetto1.Inizializza(impostazioniFarmaco.FarmacoInfo, enable, listEseguiteReazioni.Count > 1)
        Me.ucFarmacoSospetto1.Visible = impostazioniFarmaco.IsVisible

        impostazioniFarmaco = GetImpostazioniFarmacoSospetto(listEseguiteReazioni, 1)

        Me.ucFarmacoSospetto2.Inizializza(impostazioniFarmaco.FarmacoInfo, enable)
        Me.ucFarmacoSospetto2.Visible = impostazioniFarmaco.IsVisible

        impostazioniFarmaco = GetImpostazioniFarmacoSospetto(listEseguiteReazioni, 2)

        Me.ucFarmacoSospetto3.Inizializza(impostazioniFarmaco.FarmacoInfo, enable)
        Me.ucFarmacoSospetto3.Visible = impostazioniFarmaco.IsVisible

    End Sub

    ' Creazione farmaci concomitanti
    Private Sub LoadDataIntoFarmaciConcomitanti(eseguitaMaster As Entities.VaccinazioneEseguita)

        Me.ddlFarmaciConcomitanti.SelectedValue = eseguitaMaster.ReazioneAvversa.FarmacoConcomitante

        ManageFarmaciConcomitanti(eseguitaMaster)

    End Sub

    Private Function GetImpostazioniFarmacoSospetto(listEseguiteReazioni As List(Of Entities.VaccinazioneEseguita), index As Integer) As ImpostazioniFarmaco

        Dim result As New ImpostazioniFarmaco()

        If listEseguiteReazioni.Count > index Then
            result.FarmacoInfo = CreateFarmacoSospetto(listEseguiteReazioni(index), index + 1)
            result.IsVisible = True
        Else
            result.FarmacoInfo = Nothing
            result.IsVisible = False
        End If

        Return result

    End Function

    Private Function CreateFarmacoSospetto(vaccinazioneEseguita As Entities.VaccinazioneEseguita, ordinal As Integer) As Common.ReazioniAvverseCommon.FarmacoInfo

        Dim farmaco As New Common.ReazioniAvverseCommon.FarmacoInfo()

        farmaco.Ordinal = ordinal

        farmaco.CodiceNomeCommerciale = vaccinazioneEseguita.ves_noc_codice
        farmaco.DescrizioneNomeCommerciale = vaccinazioneEseguita.noc_descrizione
        farmaco.CodiceLotto = vaccinazioneEseguita.ves_lot_codice
        farmaco.DataScadLotto = vaccinazioneEseguita.ReazioneAvversa.DataScadenzaLotto
        farmaco.DataOraEsecuzioneVaccinazione = vaccinazioneEseguita.ves_dataora_effettuazione
        farmaco.CodiceSitoInoculazione = vaccinazioneEseguita.ves_sii_codice
        farmaco.DescrizioneSitoInoculazione = vaccinazioneEseguita.sii_descrizione
        farmaco.CodiceViaSomministrazione = vaccinazioneEseguita.ves_vii_codice
        farmaco.DescrizioneViaSomministrazione = vaccinazioneEseguita.vii_descrizione
        farmaco.DosaggioFiale = vaccinazioneEseguita.ReazioneAvversa.Dosaggio
        farmaco.Dose = vaccinazioneEseguita.ves_n_richiamo
        farmaco.Indicazioni = vaccinazioneEseguita.ReazioneAvversa.Indicazioni
        farmaco.CodiceIndicazioni = vaccinazioneEseguita.ReazioneAvversa.CodiceIndicazioni
        farmaco.Richiamo = vaccinazioneEseguita.ReazioneAvversa.Richiamo
        farmaco.Sospeso = vaccinazioneEseguita.ReazioneAvversa.Sospeso
        farmaco.ReazioneMigliorata = vaccinazioneEseguita.ReazioneAvversa.Migliorata
        farmaco.Ripreso = vaccinazioneEseguita.ReazioneAvversa.Ripreso
        farmaco.RicomparsiSintomi = vaccinazioneEseguita.ReazioneAvversa.Ricomparsa
        farmaco.VesId = vaccinazioneEseguita.ves_id

        Return farmaco

    End Function

    Private Function GetInfoFarmacoConcomitante1(vaccinazioneEseguita As Entities.VaccinazioneEseguita) As Common.ReazioniAvverseCommon.FarmacoInfo

        Dim farmacoInfo As New Common.ReazioniAvverseCommon.FarmacoInfo()

        farmacoInfo.Ordinal = 1

        ' Per i concomitanti, nome commerciale, lotto e flag sospeso sono obbligatori.
        ' Se nei dati da impostare manca il nome commerciale, considero che non ci sia il concomitante.

        If Not String.IsNullOrWhiteSpace(vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_DescrizioneNomeCommerciale) Then

            farmacoInfo.CodiceNomeCommerciale = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_CodiceNomeCommerciale
            farmacoInfo.DescrizioneNomeCommerciale = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_DescrizioneNomeCommerciale
            farmacoInfo.CodiceLotto = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_CodiceLotto
            farmacoInfo.DataScadLotto = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_DataScadenzaLotto
            farmacoInfo.DataOraEsecuzioneVaccinazione = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_DataOraEffettuazione
            farmacoInfo.CodiceSitoInoculazione = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_CodiceSitoInoculazione
            farmacoInfo.DescrizioneSitoInoculazione = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_DescrizioneSitoInoculazione
            farmacoInfo.CodiceViaSomministrazione = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_CodiceViaSomministrazione
            farmacoInfo.DescrizioneViaSomministrazione = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_DescrizioneViaSomministrazione
            farmacoInfo.DosaggioFiale = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_Dosaggio
            farmacoInfo.Dose = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_Dose
            farmacoInfo.Indicazioni = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_Indicazioni
            farmacoInfo.CodiceIndicazioni = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_CodiceIndicazioni
            farmacoInfo.Richiamo = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_Richiamo
            farmacoInfo.Sospeso = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_Sospeso
            farmacoInfo.ReazioneMigliorata = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_Migliorata
            farmacoInfo.Ripreso = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_Ripreso
            farmacoInfo.RicomparsiSintomi = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_Ricomparsa
            farmacoInfo.VesId = vaccinazioneEseguita.ves_id

        End If

        Return farmacoInfo

    End Function

    Private Function GetInfoFarmacoConcomitante2(vaccinazioneEseguita As Entities.VaccinazioneEseguita) As Common.ReazioniAvverseCommon.FarmacoInfo

        Dim farmacoInfo As New Common.ReazioniAvverseCommon.FarmacoInfo()

        farmacoInfo.Ordinal = 2

        farmacoInfo.CodiceNomeCommerciale = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_CodiceNomeCommerciale
        farmacoInfo.DescrizioneNomeCommerciale = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_DescrizioneNomeCommerciale
        farmacoInfo.CodiceLotto = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_CodiceLotto
        farmacoInfo.DataScadLotto = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_DataScadenzaLotto
        farmacoInfo.DataOraEsecuzioneVaccinazione = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_DataOraEffettuazione
        farmacoInfo.CodiceSitoInoculazione = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_CodiceSitoInoculazione
        farmacoInfo.DescrizioneSitoInoculazione = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_DescrizioneSitoInoculazione
        farmacoInfo.CodiceViaSomministrazione = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_CodiceViaSomministrazione
        farmacoInfo.DescrizioneViaSomministrazione = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_DescrizioneViaSomministrazione
        farmacoInfo.DosaggioFiale = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_Dosaggio
        farmacoInfo.Dose = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_Dose
        farmacoInfo.Indicazioni = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_Indicazioni
        farmacoInfo.CodiceIndicazioni = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_CodiceIndicazioni
        farmacoInfo.Richiamo = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_Richiamo
        farmacoInfo.Sospeso = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_Sospeso
        farmacoInfo.ReazioneMigliorata = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_Migliorata
        farmacoInfo.Ripreso = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_Ripreso
        farmacoInfo.RicomparsiSintomi = vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_Ricomparsa
        farmacoInfo.VesId = vaccinazioneEseguita.ves_id

        Return farmacoInfo

    End Function

    Private Sub LoadDataIntoAmbitoOsservazione(vaccinazioneEseguita As Entities.VaccinazioneEseguita)

        If String.IsNullOrWhiteSpace(vaccinazioneEseguita.ReazioneAvversa.AmbitoOsservazione) Then
			vaccinazioneEseguita.ReazioneAvversa.AmbitoOsservazione = Constants.AmbitoOsservazioneReazioneAvversa.FarmacovigilanzaAttiva
		End If

        Me.rdbAmbito_NonOsservata.Checked = False
        Me.rdbAmbito_Farmacovigilanza.Checked = False
        Me.rdbAmbito_RegistroFarmaci.Checked = False
        Me.rdbAmbito_Studio.Checked = False

        Me.txtAmbitoStudio_Titolo.Text = String.Empty
        Me.txtAmbitoStudio_Titolo.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Stringa_Disabilitato
        Me.txtAmbitoStudio_Titolo.Enabled = False

        Me.txtAmbitoStudio_Tipologia.Text = String.Empty
        Me.txtAmbitoStudio_Tipologia.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Stringa_Disabilitato
        Me.txtAmbitoStudio_Tipologia.Enabled = False

        Me.txtAmbitoStudio_Numero.Text = String.Empty
        Me.txtAmbitoStudio_Numero.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Stringa_Disabilitato
        Me.txtAmbitoStudio_Numero.Enabled = False

        Select Case vaccinazioneEseguita.ReazioneAvversa.AmbitoOsservazione

            Case Constants.AmbitoOsservazioneReazioneAvversa.NonOsservata

                Me.rdbAmbito_NonOsservata.Checked = True

            Case Constants.AmbitoOsservazioneReazioneAvversa.FarmacovigilanzaAttiva

                Me.rdbAmbito_Farmacovigilanza.Checked = True

            Case Constants.AmbitoOsservazioneReazioneAvversa.RegistroFarmaci

                Me.rdbAmbito_RegistroFarmaci.Checked = True

            Case Constants.AmbitoOsservazioneReazioneAvversa.StudioOsservazionale

                Me.rdbAmbito_Studio.Checked = True

                Me.txtAmbitoStudio_Titolo.Text = vaccinazioneEseguita.ReazioneAvversa.AmbitoOsservazione_Studio_Titolo
                Me.txtAmbitoStudio_Titolo.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Stringa
                Me.txtAmbitoStudio_Titolo.Enabled = True

                Me.txtAmbitoStudio_Tipologia.Text = vaccinazioneEseguita.ReazioneAvversa.AmbitoOsservazione_Studio_Tipologia
                Me.txtAmbitoStudio_Tipologia.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Stringa
                Me.txtAmbitoStudio_Tipologia.Enabled = True

                Me.txtAmbitoStudio_Numero.Text = vaccinazioneEseguita.ReazioneAvversa.AmbitoOsservazione_Studio_Numero
                Me.txtAmbitoStudio_Numero.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Stringa
                Me.txtAmbitoStudio_Numero.Enabled = True

            Case Else

                Throw New NotSupportedException("Valore non previsto per AmbitoOsservazione")

        End Select

    End Sub

#End Region


#Region " Lettura dati impostati "

    Private Sub SetAmbitoOsservazioneFromControl(vaccinazioneEseguita As Entities.VaccinazioneEseguita)

        vaccinazioneEseguita.ReazioneAvversa.AmbitoOsservazione_Studio_Titolo = String.Empty
        vaccinazioneEseguita.ReazioneAvversa.AmbitoOsservazione_Studio_Tipologia = String.Empty
        vaccinazioneEseguita.ReazioneAvversa.AmbitoOsservazione_Studio_Numero = String.Empty

        If Me.rdbAmbito_Studio.Checked Then

            vaccinazioneEseguita.ReazioneAvversa.AmbitoOsservazione = Constants.AmbitoOsservazioneReazioneAvversa.StudioOsservazionale

            vaccinazioneEseguita.ReazioneAvversa.AmbitoOsservazione_Studio_Titolo = Me.txtAmbitoStudio_Titolo.Text.Trim().ToUpper()
            vaccinazioneEseguita.ReazioneAvversa.AmbitoOsservazione_Studio_Tipologia = Me.txtAmbitoStudio_Tipologia.Text.Trim().ToUpper()
            vaccinazioneEseguita.ReazioneAvversa.AmbitoOsservazione_Studio_Numero = Me.txtAmbitoStudio_Numero.Text.Trim().ToUpper()

        ElseIf Me.rdbAmbito_Farmacovigilanza.Checked Then

            vaccinazioneEseguita.ReazioneAvversa.AmbitoOsservazione = Constants.AmbitoOsservazioneReazioneAvversa.FarmacovigilanzaAttiva

        ElseIf Me.rdbAmbito_RegistroFarmaci.Checked Then

            vaccinazioneEseguita.ReazioneAvversa.AmbitoOsservazione = Constants.AmbitoOsservazioneReazioneAvversa.RegistroFarmaci

        Else

            vaccinazioneEseguita.ReazioneAvversa.AmbitoOsservazione = Constants.AmbitoOsservazioneReazioneAvversa.NonOsservata

        End If

    End Sub

    Private Sub SetDatiFarmacoSospetto(vaccinazioneEseguita As Entities.VaccinazioneEseguita, index As Integer)

        Dim farmacoInfo As Common.ReazioniAvverseCommon.FarmacoInfo

        If index = 0 Then

            If Me.ucFarmacoSospetto1.Visible Then
                farmacoInfo = Me.ucFarmacoSospetto1.GetFarmacoInfo()
            Else
                farmacoInfo = New Common.ReazioniAvverseCommon.FarmacoInfo()
            End If

        ElseIf index = 1 Then

            If Me.ucFarmacoSospetto2.Visible Then
                farmacoInfo = Me.ucFarmacoSospetto2.GetFarmacoInfo()
            Else
                farmacoInfo = New Common.ReazioniAvverseCommon.FarmacoInfo()
            End If

        ElseIf index = 2 Then

            If Me.ucFarmacoSospetto3.Visible Then
                farmacoInfo = Me.ucFarmacoSospetto3.GetFarmacoInfo()
            Else
                farmacoInfo = New Common.ReazioniAvverseCommon.FarmacoInfo()
            End If

        Else

            farmacoInfo = New Common.ReazioniAvverseCommon.FarmacoInfo()

        End If

        vaccinazioneEseguita.ReazioneAvversa.Dosaggio = farmacoInfo.DosaggioFiale
        vaccinazioneEseguita.ReazioneAvversa.Sospeso = farmacoInfo.Sospeso
        vaccinazioneEseguita.ReazioneAvversa.Migliorata = farmacoInfo.ReazioneMigliorata
        vaccinazioneEseguita.ReazioneAvversa.Ripreso = farmacoInfo.Ripreso
        vaccinazioneEseguita.ReazioneAvversa.Ricomparsa = farmacoInfo.RicomparsiSintomi
        vaccinazioneEseguita.ReazioneAvversa.Indicazioni = farmacoInfo.Indicazioni
        vaccinazioneEseguita.ReazioneAvversa.CodiceIndicazioni = farmacoInfo.CodiceIndicazioni

        If farmacoInfo.Richiamo.HasValue Then
            vaccinazioneEseguita.ReazioneAvversa.Richiamo = farmacoInfo.Richiamo.Value
        Else
            vaccinazioneEseguita.ReazioneAvversa.Richiamo = 0
        End If

    End Sub

    Private Sub SetDatiFarmacoConcomitante(vaccinazioneEseguita As Entities.VaccinazioneEseguita, index As Integer)

        Dim farmacoInfo As Common.ReazioniAvverseCommon.FarmacoInfo

        If index = 0 Then

            If Me.ucFarmacoConcomitante1.Visible Then
                farmacoInfo = Me.ucFarmacoConcomitante1.GetFarmacoInfo()
            Else
                farmacoInfo = New Common.ReazioniAvverseCommon.FarmacoInfo()
            End If

            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_CodiceLotto = farmacoInfo.CodiceLotto
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_CodiceSitoInoculazione = farmacoInfo.CodiceSitoInoculazione
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_CodiceViaSomministrazione = farmacoInfo.CodiceViaSomministrazione
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_DataOraEffettuazione = farmacoInfo.DataOraEsecuzioneVaccinazione
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_CodiceNomeCommerciale = farmacoInfo.CodiceNomeCommerciale
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_DescrizioneNomeCommerciale = farmacoInfo.DescrizioneNomeCommerciale
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_Dose = farmacoInfo.Dose
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_Dosaggio = farmacoInfo.DosaggioFiale
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_Sospeso = farmacoInfo.Sospeso
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_Migliorata = farmacoInfo.ReazioneMigliorata
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_Ripreso = farmacoInfo.Ripreso
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_Ricomparsa = farmacoInfo.RicomparsiSintomi
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_Indicazioni = farmacoInfo.Indicazioni
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_CodiceIndicazioni = farmacoInfo.CodiceIndicazioni
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_Richiamo = farmacoInfo.Richiamo
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante1_DataScadenzaLotto = farmacoInfo.DataScadLotto

        ElseIf index = 1 Then

            If Me.ucFarmacoConcomitante2.Visible Then
                farmacoInfo = Me.ucFarmacoConcomitante2.GetFarmacoInfo()
            Else
                farmacoInfo = New Common.ReazioniAvverseCommon.FarmacoInfo()
            End If

            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_CodiceLotto = farmacoInfo.CodiceLotto
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_CodiceSitoInoculazione = farmacoInfo.CodiceSitoInoculazione
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_CodiceViaSomministrazione = farmacoInfo.CodiceViaSomministrazione
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_DataOraEffettuazione = farmacoInfo.DataOraEsecuzioneVaccinazione
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_CodiceNomeCommerciale = farmacoInfo.CodiceNomeCommerciale
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_DescrizioneNomeCommerciale = farmacoInfo.DescrizioneNomeCommerciale
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_Dose = farmacoInfo.Dose
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_Dosaggio = farmacoInfo.DosaggioFiale
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_Sospeso = farmacoInfo.Sospeso
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_Migliorata = farmacoInfo.ReazioneMigliorata
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_Ripreso = farmacoInfo.Ripreso
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_Ricomparsa = farmacoInfo.RicomparsiSintomi
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_Indicazioni = farmacoInfo.Indicazioni
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_CodiceIndicazioni = farmacoInfo.CodiceIndicazioni
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_Richiamo = farmacoInfo.Richiamo
            vaccinazioneEseguita.ReazioneAvversa.FarmacoConcomitante2_DataScadenzaLotto = farmacoInfo.DataScadLotto

        End If

    End Sub

#End Region

    Private Sub Esito_CheckedChanged(sender As System.Object, e As System.EventArgs)

        Me.EsitoSetEnabled(sender.id)

    End Sub

    Private Sub EsitoSetEnabled(selectedRadioButtonId As String)

        Me.rb_EsitoDecesso.Checked = False
        Me.rb_reazAvversa.Enabled = False
        Me.rb_farmaco.Enabled = False
        Me.rb_noFarmaco.Enabled = False
        Me.rb_sconosciuta.Enabled = False

        Select Case selectedRadioButtonId

            Case Me.rb_risCompleta.ID

                Me.rb_reazAvversa.Checked = False
                Me.rb_farmaco.Checked = False
                Me.rb_noFarmaco.Checked = False
                Me.rb_sconosciuta.Checked = False

                Me.dpkDataEsito.Enabled = True
                Me.dpkDataEsito.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Data_Obbligatorio

            Case Me.rb_risPostumi.ID, Me.rb_miglioramento.ID, Me.rb_reazioneInvariata.ID, Me.rb_noDisponibile.ID

                Me.rb_reazAvversa.Checked = False
                Me.rb_farmaco.Checked = False
                Me.rb_noFarmaco.Checked = False
                Me.rb_sconosciuta.Checked = False

                Me.dpkDataEsito.Text = String.Empty
                Me.dpkDataEsito.Enabled = False
                Me.dpkDataEsito.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Data_Disabilitato

            Case Me.rb_EsitoDecesso.ID

                Me.rb_EsitoDecesso.Checked = True
                Me.rb_reazAvversa.Enabled = Me.rb_EsitoDecesso.Enabled
                Me.rb_farmaco.Enabled = Me.rb_EsitoDecesso.Enabled
                Me.rb_noFarmaco.Enabled = Me.rb_EsitoDecesso.Enabled
                Me.rb_sconosciuta.Enabled = Me.rb_EsitoDecesso.Enabled

                If Not Me.rb_reazAvversa.Checked AndAlso Not Me.rb_farmaco.Checked AndAlso Not Me.rb_noFarmaco.Checked AndAlso Not Me.rb_sconosciuta.Checked Then
                    Me.rb_reazAvversa.Checked = True
                End If

                Me.dpkDataEsito.Enabled = True
                Me.dpkDataEsito.CssClass = Common.ReazioniAvverseCommon.CssName.TextBox_Data_Obbligatorio

        End Select

    End Sub

    Private Sub GravitaReazione_CheckedChanged(sender As System.Object, e As System.EventArgs)

        Me.GravitaReazioneSetEnabled(sender.id)

    End Sub

    Private Sub GravitaReazioneSetEnabled(selectedRadioButtonId As String)

        Select Case selectedRadioButtonId

            Case Me.rb_grave.ID

                Me.rb_decesso.Enabled = Me.IsPazienteDeceduto
                Me.rb_ospedalizz.Enabled = True
                Me.rb_invalidita.Enabled = True
                Me.rb_pericolo.Enabled = True
                Me.rb_anomalie.Enabled = True
                Me.rb_altro.Enabled = True

                If Me.rb_decesso.Checked AndAlso Not Me.IsPazienteDeceduto Then
                    Me.rb_decesso.Checked = False
                    ' Me.rb_ospedalizz.Checked = True
                End If

                'If Not Me.rb_ospedalizz.Checked AndAlso Not Me.rb_invalidita.Checked AndAlso Not Me.rb_pericolo.Checked AndAlso Not Me.rb_anomalie.Checked AndAlso Not Me.rb_altro.Checked Then
                '    If PazienteDeceduto Then
                '        Me.rb_decesso.Checked = True
                '    Else
                '        Me.rb_ospedalizz.Checked = True
                '    End If
                'End If

            Case Me.rb_noGrave.ID

                Me.rb_decesso.Enabled = False
                Me.rb_ospedalizz.Enabled = False
                Me.rb_invalidita.Enabled = False
                Me.rb_pericolo.Enabled = False
                Me.rb_anomalie.Enabled = False
                Me.rb_altro.Enabled = False

                Me.rb_decesso.Checked = False
                Me.rb_ospedalizz.Checked = False
                Me.rb_invalidita.Checked = False
                Me.rb_pericolo.Checked = False
                Me.rb_anomalie.Checked = False
                Me.rb_altro.Checked = False

        End Select

    End Sub

    Private Function GetFiltroBaseReazione() As String

        Return " REA_OBSOLETO = 'N' "

    End Function

    Private Function GetFiltroCodiceReazione(codiceReazioneAvversa As String) As String

        Return String.Format(" AND REA_CODICE <> '{0}' ", codiceReazioneAvversa)

    End Function

    Private Function GetOrderByReazione() As String

        Return " ORDER BY REA_DESCRIZIONE "

    End Function

    ' Elenco campi di tipo string da controllare
    Private Function GetStringFieldList() As List(Of Entities.FieldToCheck(Of String))

        Dim stringFieldList As New List(Of Entities.FieldToCheck(Of String))

        Dim stringField As Entities.FieldToCheck(Of String) = Nothing

        stringField = New Entities.FieldToCheck(Of String)()
        stringField.Value = Me.fm_tipoReaz.Codice
        stringField.Description = Me.lblTipoReaz.Text
        stringField.MaxLength = 0
        stringField.Required = True

        stringFieldList.Add(stringField)

        stringField = New Entities.FieldToCheck(Of String)()
        stringField.Value = Me.tb_altroReaz.Text
        stringField.Description = Me.lblTipoReazAltro.Text
        stringField.MaxLength = Me.tb_altroReaz.MaxLength
        stringField.Required = Me.IsAltraReazioneObbligatoria

        stringFieldList.Add(stringField)

        stringField = New Entities.FieldToCheck(Of String)()
        stringField.Value = Me.ddlFarmaciConcomitanti.SelectedValue
        stringField.Description = Me.lblFarmacoConcDdl.Text
        stringField.MaxLength = 0
        stringField.Required = True

        stringFieldList.Add(stringField)

        stringField = New Entities.FieldToCheck(Of String)()
        stringField.Value = Me.tb_VisRic.Text
        stringField.Description = Me.lblVisRic.Text
        stringField.MaxLength = Me.tb_VisRic.MaxLength
        stringField.Required = False

        stringFieldList.Add(stringField)

        stringField = New Entities.FieldToCheck(Of String)()
        stringField.Value = Me.tb_Terapie.Text
        stringField.Description = Me.lblTerapie.Text
        stringField.MaxLength = Me.tb_Terapie.MaxLength
        stringField.Required = False

        stringFieldList.Add(stringField)

        stringField = New Entities.FieldToCheck(Of String)()
        stringField.Value = Me.tb_altriFarmaci.Text
        stringField.Description = Me.lblAltriFarmaci.Text
        stringField.MaxLength = Me.tb_altriFarmaci.MaxLength
        stringField.Required = False

        stringFieldList.Add(stringField)

        stringField = New Entities.FieldToCheck(Of String)()
        stringField.Value = Me.tb_condConcomitanti.Text
        stringField.Description = Me.lblCondConcomitanti.Text
        stringField.MaxLength = Me.tb_condConcomitanti.MaxLength
        stringField.Required = False

        stringFieldList.Add(stringField)

        stringField = New Entities.FieldToCheck(Of String)()
        stringField.Value = Me.txtAltreInfo.Text
        stringField.Description = Me.lblAltreInfo.Text
        stringField.MaxLength = Me.txtAltreInfo.MaxLength
        stringField.Required = False

        stringFieldList.Add(stringField)

        Return stringFieldList

    End Function

    Private Function GetDateTimeFieldList() As List(Of Entities.FieldToCheck(Of DateTime))

        Dim dateTimeFieldList As New List(Of Entities.FieldToCheck(Of DateTime))()

        Dim dateTimeField As New Entities.FieldToCheck(Of DateTime)()

        dateTimeField.Value = Me.dpkDataReazione.Data
        dateTimeField.Description = Me.lblDataReazione.Text
        dateTimeField.MaxLength = 0
        dateTimeField.Required = True

        dateTimeFieldList.Add(dateTimeField)

        dateTimeField = New Entities.FieldToCheck(Of DateTime)()
        dateTimeField.Value = Me.dpkDataEsito.Data
        dateTimeField.Description = "Data Esito"
        dateTimeField.MaxLength = 0
        dateTimeField.Required = (Me.rb_risCompleta.Checked Or Me.rb_EsitoDecesso.Checked)

        dateTimeFieldList.Add(dateTimeField)

        Return dateTimeFieldList

    End Function

    Private Sub AddCheckFieldsToList(Of T)(fieldsToAdd As List(Of Entities.FieldToCheck(Of T)), checkFieldList As List(Of Entities.FieldToCheck(Of T)))

        If Not fieldsToAdd.IsNullOrEmpty() Then

            If checkFieldList.IsNullOrEmpty() Then checkFieldList = New List(Of Entities.FieldToCheck(Of T))()

            checkFieldList.AddRange(fieldsToAdd)

        End If

    End Sub

    Private Function GetJSCheckDettaglioReazione() As String

        Dim script As New System.Text.StringBuilder()

        script.AppendLine("<script type='text/javascript'>")
        script.AppendLine("function CheckDettaglioReazione(valoreDataVac, isAltraReazioneObbligatoria) {")

        ' Controllo data reazione
        script.AppendFormat("var idCampo = '{0}';", Me.dpkDataReazione.ClientID).AppendLine()
        script.AppendFormat("var lblCampo = '{0}';", Me.lblDataReazione.Text).AppendLine()
        script.AppendLine("if (!ControlloDataReazione(idCampo, lblCampo, valoreDataVac)) return false;")

        ' Controllo campi Tipologia Reazione
        script.AppendFormat("var idTipoReaz1 = '{0}';", Me.fm_tipoReaz.ClientID).AppendLine()
        script.AppendFormat("var lblTipoReaz1 = '{0}';", Me.lblTipoReaz.Text).AppendLine()
        script.AppendFormat("var idTipoReaz2 = '{0}';", Me.fm_tipoReaz1.ClientID).AppendLine()
        script.AppendFormat("var lblTipoReaz2 = '{0}';", Me.lblTipoReaz2.Text).AppendLine()
        script.AppendFormat("var idTipoReaz3 = '{0}';", Me.fm_tipoReaz2.ClientID).AppendLine()
        script.AppendFormat("var lblTipoReaz3 = '{0}';", Me.lblTipoReaz3.Text).AppendLine()
        script.AppendFormat("var idTipoReazAltro = '{0}';", Me.tb_altroReaz.ClientID).AppendLine()
        script.AppendFormat("var lblTipoReazAltro = '{0}';", Me.lblTipoReazAltro.Text).AppendLine()
        script.AppendFormat("var maxLengthCampo = {0};", Me.tb_altroReaz.MaxLength.ToString()).AppendLine()
        script.AppendLine("if (!ControlloTipoReazione(idTipoReaz1, lblTipoReaz1, idTipoReaz2, lblTipoReaz2, idTipoReaz3, lblTipoReaz3, idTipoReazAltro, lblTipoReazAltro, isAltraReazioneObbligatoria, maxLengthCampo)) return false;")

        ' Controllo lunghezza campo Visite-Ricoveri
        script.AppendFormat("idCampo = '{0}';", Me.tb_VisRic.ClientID).AppendLine()
        script.AppendFormat("maxLengthCampo = {0};", Me.tb_VisRic.MaxLength.ToString()).AppendLine()
        script.AppendFormat("lblCampo = '{0}';", Me.lblVisRic.Text).AppendLine()
        script.AppendLine("if (!ControlloLunghezzaCampo(idCampo, maxLengthCampo, lblCampo)) return false;")

        ' Controllo lunghezza campo Terapie
        script.AppendFormat("idCampo = '{0}';", Me.tb_Terapie.ClientID).AppendLine()
        script.AppendFormat("maxLengthCampo = {0};", Me.tb_Terapie.MaxLength.ToString()).AppendLine()
        script.AppendFormat("lblCampo = '{0}';", Me.lblTerapie.Text).AppendLine()
        script.AppendLine("if (!ControlloLunghezzaCampo(idCampo, maxLengthCampo, lblCampo)) return false;")

        ' Controllo Farmaci Concomitanti
        script.AppendFormat("var idDdlFarmConc = '{0}';", Me.ddlFarmaciConcomitanti.ClientID).AppendLine()
        script.AppendFormat("var lblDdlFarmConc = '{0}';", Me.lblFarmacoConcDdl.Text).AppendLine()

        ' Controllo obbligatorietà ddlFarmaciConcomitanti 
        script.AppendLine("if (!ControlloCampoObbligatorio(idDdlFarmConc, lblDdlFarmConc)) return false;")

        ' Controllo lunghezza Altri Farmaci
        script.AppendFormat("idCampo = '{0}';", Me.tb_altriFarmaci.ClientID).AppendLine()
        script.AppendFormat("maxLengthCampo = {0};", Me.tb_altriFarmaci.MaxLength.ToString()).AppendLine()
        script.AppendFormat("lblCampo = '{0}';", Me.lblAltriFarmaci.Text).AppendLine()
        script.AppendLine("if (!ControlloLunghezzaCampo(idCampo, maxLengthCampo, lblCampo)) return false;")

        ' Controllo lunghezza Condizioni Concomitanti
        script.AppendFormat("idCampo = '{0}';", Me.tb_condConcomitanti.ClientID).AppendLine()
        script.AppendFormat("maxLengthCampo = {0};", Me.tb_condConcomitanti.MaxLength.ToString()).AppendLine()
        script.AppendFormat("lblCampo = '{0}';", Me.lblCondConcomitanti.Text).AppendLine()
        script.AppendLine("if (!ControlloLunghezzaCampo(idCampo, maxLengthCampo, lblCampo)) return false;")

        ' Controllo lunghezza Altre Informazioni
        script.AppendFormat("idCampo = '{0}';", Me.txtAltreInfo.ClientID).AppendLine()
        script.AppendFormat("maxLengthCampo = {0};", Me.txtAltreInfo.MaxLength.ToString()).AppendLine()
        script.AppendFormat("lblCampo = '{0}';", Me.lblAltreInfo.Text).AppendLine()
        script.AppendLine("if (!ControlloLunghezzaCampo(idCampo, maxLengthCampo, lblCampo)) return false;")

        ' Controllo Data Esito (obbligatoria se è selezionato "risoluzione completa" o "decesso")
        script.AppendFormat("var idRdbRis = '{0}';", Me.rb_risCompleta.ClientID).AppendLine()
        script.AppendFormat("var idRdbDec = '{0}';", Me.rb_EsitoDecesso.ClientID).AppendLine()
        script.AppendFormat("idCampo = '{0}';", Me.dpkDataEsito.ClientID).AppendLine()
        script.AppendLine("if (!ControlloDataEsito(idCampo, idRdbRis, idRdbDec)) return false;")

        If Settings.REAZIONE_AVVERSA_INTEGRAZIONE Then
            ' Controllo cognome segnalatore
            script.AppendFormat("var idCognome = '{0}';", txt_CognomeSeg.ClientID).AppendLine()
            script.AppendFormat("var lblCognome = '{0}';", "Cognome").AppendLine()

            ' Controllo obbligatorietà del cognome del segnalatore 
            script.AppendLine("if (!ControlloCampoObbligatorio(idCognome, lblCognome)) return false;")

            ' Controllo mail segnalatore
            script.AppendFormat("var idEmail = '{0}';", txt_emailSeg.ClientID).AppendLine()
            script.AppendFormat("var lblEmail = '{0}';", "E-mail").AppendLine()

            ' Controllo obbligatorietà della mail del segnalatore 
            script.AppendLine("if (!ControlloCampoObbligatorio(idEmail, lblEmail)) return false;")
        End If


        script.AppendLine("return true;")
        script.AppendLine("}</script>")

        Return script.ToString()

    End Function

#End Region

End Class
