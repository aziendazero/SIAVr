Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Enumerators

Public Class BizRegistrazioneVaccinazioni
    Inherits BizClass

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos)

        MyBase.New(genericprovider, settings, contextInfos, Nothing)

    End Sub

#End Region

#Region " Types "

    <Serializable>
    Public Class DatiAggiuntiviVaccinazione
        Public CodiceNomeCommerciale As String
        Public CodiceLotto As String
        Public DataScadenzaLotto As String
        Public CodiceViaSomministrazione As String
        Public CodiceSitoInoculazione As String
        Public CodiceMedicoResponsabile As String
        Public CodiceVaccinatore As String
        Public CodiceComuneStato As String
        Public Note As String
        Public TipoComuneStato As String
        Public CodiceStruttura As String
        Public CodiceUsl As String
    End Class

    Public Class IdControlli
        Public Const txtConsultorioItem As String = "txtConsultorioItem"
        Public Const omlNomeCommerciale As String = "omlNomeCommerciale"
        Public Const txtLotto As String = "txtLotto"
        Public Const dpkDataScadenzaLotto As String = "dpkDataScadenzaLotto"
        Public Const ddlVii As String = "ddlVii"
        Public Const ddlSii As String = "ddlSii"
        Public Const omlMedicoResp As String = "omlMedicoResp"
        Public Const omlVaccinatore As String = "omlVaccinatore"
        Public Const omlComuneStato As String = "omlComuneStato"
        Public Const txtNoteVac As String = "txtNoteVac"
        Public Const omlCondSanitaria As String = "omlCondSanitaria"
        Public Const omlCondRischio As String = "omlCondRischio"
        Public Const omlStrutture As String = "omlStrutture"
        Public Const omlUsl As String = "omlUsl"
    End Class


    'TODO: SPOSTARE
    <Serializable>
    Public Class DatiPagamentoVaccinazione
        Public GuidTipoPagamento As Guid
        Public CodiceMalattia As String
        Public CodiceEsenzione As String
        Public Importo As String
    End Class


    <Serializable>
    Public Class AssociazioneDaRegistrare

        Public CodiceAssociazione As String
        Public NumeroDoseAssociazione As Integer
        Public DataEsecuzione As DateTime?
        Public CodiceConsultorio As String
        Public CodiceLuogoEsecuzione As String
        Public TipoErogatore As String
        Public TipoLuogoEsecuzione As String
        Public IsLuogoDefaultConsultorio As Boolean
        Public IdCampiObbligatori As List(Of String)
        Public VaccinazioniDaRegistrare As List(Of VaccinazioneDaRegistrare)

        Public Class VaccinazioneDaRegistrare
            Public CodiceVaccinazione As String
            Public NumeroDoseVaccinazione As Integer
            Public IdCondizioneSanitaria As String
            Public CodiceCondizioneSanitaria As String
            Public IdCondizioneRischio As String
            Public CodiceCondizioneRischio As String
        End Class

        Public Sub New()
            Me.VaccinazioniDaRegistrare = New List(Of VaccinazioneDaRegistrare)()
        End Sub

    End Class

    <Serializable>
    Public Class CheckDatiAssociazioneResult
        Public Success As Boolean
        Public ControlList As List(Of BizVaccinazioniEseguite.ControlloEsecuzioneItem)

        Public Sub New()
            Me.Success = True
            Me.ControlList = New List(Of BizVaccinazioniEseguite.ControlloEsecuzioneItem)()
        End Sub

    End Class

#End Region

#Region " Public "

    ''' <summary>
    ''' Restituisce true se il controllo con id specificato fa parte dei campi obbligatori
    ''' </summary>
    ''' <param name="idControllo"></param>
    ''' <param name="idCampiObbligatori"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function IsCampoObbligatorio(idControllo As String, idCampiObbligatori As List(Of String)) As Boolean

        Dim isObbligatorio As Boolean = False

        If Not idCampiObbligatori Is Nothing Then
            isObbligatorio = (idCampiObbligatori.Any(Function(id) id = idControllo))
        End If

        Return isObbligatorio

    End Function

    ''' <summary>
    ''' Restituisce true se almeno un campo dei dati aggiuntivi è valorizzato
    ''' </summary>
    ''' <param name="datiAggiuntivi"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ExistsDatiAggiuntivi(datiAggiuntivi As BizRegistrazioneVaccinazioni.DatiAggiuntiviVaccinazione) As Boolean

        If datiAggiuntivi Is Nothing Then Return False

        If Not String.IsNullOrWhiteSpace(datiAggiuntivi.CodiceNomeCommerciale) Then Return True
        If Not String.IsNullOrWhiteSpace(datiAggiuntivi.CodiceLotto) Then Return True
        If Not String.IsNullOrWhiteSpace(datiAggiuntivi.CodiceViaSomministrazione) Then Return True
        If Not String.IsNullOrWhiteSpace(datiAggiuntivi.CodiceSitoInoculazione) Then Return True
        If Not String.IsNullOrWhiteSpace(datiAggiuntivi.CodiceMedicoResponsabile) Then Return True
        If Not String.IsNullOrWhiteSpace(datiAggiuntivi.CodiceVaccinatore) Then Return True
        If Not String.IsNullOrWhiteSpace(datiAggiuntivi.CodiceComuneStato) Then Return True
        If Not String.IsNullOrWhiteSpace(datiAggiuntivi.Note) Then Return True
        If Not String.IsNullOrWhiteSpace(datiAggiuntivi.CodiceUsl) Then Return True
        If Not String.IsNullOrWhiteSpace(datiAggiuntivi.CodiceStruttura) Then Return True

        Return False

    End Function

    ''' <summary>
    ''' Controlla i dati dell'associazione da registrare
    ''' </summary>
    ''' <param name="datiAssociazione"></param>
    ''' <param name="datiAggiuntivi"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckDatiAssociazione(datiAssociazione As AssociazioneDaRegistrare, datiAggiuntivi As DatiAggiuntiviVaccinazione, datiPagamento As DatiPagamentoVaccinazione) As CheckDatiAssociazioneResult

        ' Codice di associazione
        If String.IsNullOrWhiteSpace(datiAssociazione.CodiceAssociazione) Then

            Return CreateFailureResult(String.Empty, 0,
                                       BizVaccinazioniEseguite.ControlloEsecuzioneItemType.DatiIncompleti)

        End If

        ' Dose di associazione
        If datiAssociazione.NumeroDoseAssociazione <= 0 Then

            Return CreateFailureResult(datiAssociazione.CodiceAssociazione, 0,
                                       BizVaccinazioniEseguite.ControlloEsecuzioneItemType.DatiIncompleti)

        End If

        ' Data di esecuzione
        If Not datiAssociazione.DataEsecuzione.HasValue OrElse (datiAssociazione.DataEsecuzione.Value = Date.MinValue) Then

            Return CreateFailureResult(datiAssociazione.CodiceAssociazione, datiAssociazione.NumeroDoseAssociazione,
                                       BizVaccinazioniEseguite.ControlloEsecuzioneItemType.DatiIncompleti)

        End If
        ' Tipo Erogatore
        If String.IsNullOrWhiteSpace(datiAssociazione.TipoErogatore) Then

            Return CreateFailureResult(datiAssociazione.CodiceAssociazione, datiAssociazione.NumeroDoseAssociazione,
                                       BizVaccinazioniEseguite.ControlloEsecuzioneItemType.DatiIncompleti)

        End If
        'Consultorio
        If IsCampoObbligatorio(IdControlli.txtConsultorioItem, datiAssociazione.IdCampiObbligatori) AndAlso String.IsNullOrWhiteSpace(datiAssociazione.CodiceConsultorio) Then
            Return CreateFailureResult(datiAssociazione.CodiceAssociazione, datiAssociazione.NumeroDoseAssociazione,
                           BizVaccinazioniEseguite.ControlloEsecuzioneItemType.DatiIncompleti)
        End If

        ' Luogo di esecuzione
        If String.IsNullOrWhiteSpace(datiAssociazione.CodiceLuogoEsecuzione) Then

            Return CreateFailureResult(datiAssociazione.CodiceAssociazione, datiAssociazione.NumeroDoseAssociazione,
                                       BizVaccinazioniEseguite.ControlloEsecuzioneItemType.DatiIncompleti)

        End If

        '' Se il luogo selezionato è "in azienda", il codice del consultorio è obbligatorio
        'If datiAssociazione.TipoLuogoEsecuzione = Constants.TipoLuogoEsecuzioneVaccinazione.InAzienda Then

        '    If datiAssociazione.IsLuogoDefaultConsultorio AndAlso String.IsNullOrWhiteSpace(datiAssociazione.CodiceConsultorio) Then

        '        Return CreateFailureResult(datiAssociazione.CodiceAssociazione, datiAssociazione.NumeroDoseAssociazione,
        '                                   BizVaccinazioniEseguite.ControlloEsecuzioneItemType.DatiIncompleti)

        '    End If

        'End If

        ' Vaccinazioni
        If Not datiAssociazione.VaccinazioniDaRegistrare Is Nothing AndAlso datiAssociazione.VaccinazioniDaRegistrare.Count > 0 Then

            For Each vaccinazioneDaRegistrare As AssociazioneDaRegistrare.VaccinazioneDaRegistrare In datiAssociazione.VaccinazioniDaRegistrare

                ' Codice di vaccinazione
                If String.IsNullOrWhiteSpace(vaccinazioneDaRegistrare.CodiceVaccinazione) Then

                    Return CreateFailureResult(datiAssociazione.CodiceAssociazione, datiAssociazione.NumeroDoseAssociazione,
                                               BizVaccinazioniEseguite.ControlloEsecuzioneItemType.DatiIncompleti)

                End If

                ' Dose di vaccinazione
                If vaccinazioneDaRegistrare.NumeroDoseVaccinazione <= 0 Then

                    Return CreateFailureResult(datiAssociazione.CodiceAssociazione, datiAssociazione.NumeroDoseAssociazione,
                                               BizVaccinazioniEseguite.ControlloEsecuzioneItemType.DatiIncompleti)

                End If

                ' Condizione sanitaria
                If BizRegistrazioneVaccinazioni.IsCampoObbligatorio(vaccinazioneDaRegistrare.IdCondizioneSanitaria, datiAssociazione.IdCampiObbligatori) Then

                    If String.IsNullOrWhiteSpace(vaccinazioneDaRegistrare.CodiceCondizioneSanitaria) Then

                        Return CreateFailureResult(datiAssociazione.CodiceAssociazione, datiAssociazione.NumeroDoseAssociazione,
                                                   BizVaccinazioniEseguite.ControlloEsecuzioneItemType.DatiIncompleti)

                    End If
                End If

                ' Condizione di rischio
                If BizRegistrazioneVaccinazioni.IsCampoObbligatorio(vaccinazioneDaRegistrare.IdCondizioneRischio, datiAssociazione.IdCampiObbligatori) Then

                    If String.IsNullOrWhiteSpace(vaccinazioneDaRegistrare.CodiceCondizioneRischio) Then

                        Return CreateFailureResult(datiAssociazione.CodiceAssociazione, datiAssociazione.NumeroDoseAssociazione,
                                                   BizVaccinazioniEseguite.ControlloEsecuzioneItemType.DatiIncompleti)

                    End If
                End If

            Next

        End If

        If datiAggiuntivi IsNot Nothing Then

            '' Se il luogo selezionato è "in azienda", sbianco il codice del comune o stato
            'If datiAssociazione.TipoLuogoEsecuzione = Constants.TipoLuogoEsecuzioneVaccinazione.InAzienda Then

            '    datiAggiuntivi.CodiceComuneStato = String.Empty

            'End If

            ' Congruenza luogo - comune o stato
            If Not String.IsNullOrWhiteSpace(datiAggiuntivi.CodiceComuneStato) Then

                If (datiAssociazione.TipoLuogoEsecuzione = Constants.TipoLuogoEsecuzioneVaccinazione.Estero AndAlso datiAggiuntivi.TipoComuneStato = Constants.TipologiaComune.ComuneItaliano) OrElse
                (datiAssociazione.TipoLuogoEsecuzione <> Constants.TipoLuogoEsecuzioneVaccinazione.Estero AndAlso datiAggiuntivi.TipoComuneStato = Constants.TipologiaComune.Stato) Then

                    Return CreateFailureResult(datiAssociazione.CodiceAssociazione, datiAssociazione.NumeroDoseAssociazione,
                                            BizVaccinazioniEseguite.ControlloEsecuzioneItemType.LuogoComuneStatoIncongruenti)

                End If

            End If

            ' Obbligatorietà campi aggiuntivi
            If Not CheckDatiAggiuntiviObbligatoriValorizzati(datiAggiuntivi, datiAssociazione.IdCampiObbligatori, datiAssociazione.TipoLuogoEsecuzione) Then

                Return CreateFailureResult(datiAssociazione.CodiceAssociazione, datiAssociazione.NumeroDoseAssociazione,
                                        BizVaccinazioniEseguite.ControlloEsecuzioneItemType.DatiAggiuntiviIncompleti)

            End If

        End If

        If Not CheckDatiPagamentoObbligatori(datiPagamento) Then
            Return CreateFailureResult(datiAssociazione.CodiceAssociazione, datiAssociazione.NumeroDoseAssociazione,
                                        BizVaccinazioniEseguite.ControlloEsecuzioneItemType.DatiPagamentoIncompleti)
        End If



        Return New CheckDatiAssociazioneResult()

    End Function

#End Region

#Region " Private "

    Private Shared Function CreateFailureResult(codice As String, dose As Int16?, controlType As BizVaccinazioniEseguite.ControlloEsecuzioneItemType) As CheckDatiAssociazioneResult

        Dim numeroDose As Int16 = 0
        If dose.HasValue Then numeroDose = dose.Value

        Dim result As New CheckDatiAssociazioneResult()
        result.Success = False
        result.ControlList.Add(New BizVaccinazioniEseguite.ControlloEsecuzioneItem(codice, numeroDose, True, controlType))

        Return result

    End Function

    ''' <summary>
    ''' Restituisce true se tutti i dati aggiuntivi impostati come obbligatori sono valorizzati, false altrimenti
    ''' </summary>
    ''' <param name="datiAggiuntivi"></param>
    ''' <param name="idCampiObbligatori"></param>
    ''' <param name="tipoLuogoEsecuzione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function CheckDatiAggiuntiviObbligatoriValorizzati(datiAggiuntivi As BizRegistrazioneVaccinazioni.DatiAggiuntiviVaccinazione, idCampiObbligatori As List(Of String), tipoLuogoEsecuzione As String) As Boolean

        If idCampiObbligatori Is Nothing OrElse idCampiObbligatori.Count = 0 Then
            Return True
        End If

        If BizRegistrazioneVaccinazioni.IsCampoObbligatorio(IdControlli.omlNomeCommerciale, idCampiObbligatori) Then
            If String.IsNullOrWhiteSpace(datiAggiuntivi.CodiceNomeCommerciale) Then Return False
        End If

        If BizRegistrazioneVaccinazioni.IsCampoObbligatorio(IdControlli.txtLotto, idCampiObbligatori) Then
            If String.IsNullOrWhiteSpace(datiAggiuntivi.CodiceLotto) Then Return False
        End If

        If BizRegistrazioneVaccinazioni.IsCampoObbligatorio(IdControlli.dpkDataScadenzaLotto, idCampiObbligatori) Then
            If String.IsNullOrWhiteSpace(datiAggiuntivi.DataScadenzaLotto) Then Return False
        End If

        If BizRegistrazioneVaccinazioni.IsCampoObbligatorio(IdControlli.ddlVii, idCampiObbligatori) Then
            If String.IsNullOrWhiteSpace(datiAggiuntivi.CodiceViaSomministrazione) Then Return False
        End If

        If BizRegistrazioneVaccinazioni.IsCampoObbligatorio(IdControlli.ddlSii, idCampiObbligatori) Then
            If String.IsNullOrWhiteSpace(datiAggiuntivi.CodiceSitoInoculazione) Then Return False
        End If

        If BizRegistrazioneVaccinazioni.IsCampoObbligatorio(IdControlli.omlMedicoResp, idCampiObbligatori) Then
            If String.IsNullOrWhiteSpace(datiAggiuntivi.CodiceMedicoResponsabile) Then Return False
        End If

        If BizRegistrazioneVaccinazioni.IsCampoObbligatorio(IdControlli.omlVaccinatore, idCampiObbligatori) Then
            If String.IsNullOrWhiteSpace(datiAggiuntivi.CodiceVaccinatore) Then Return False
        End If

        If BizRegistrazioneVaccinazioni.IsCampoObbligatorio(IdControlli.omlStrutture, idCampiObbligatori) Then
            If String.IsNullOrWhiteSpace(datiAggiuntivi.CodiceStruttura) Then Return False
        End If

        If BizRegistrazioneVaccinazioni.IsCampoObbligatorio(IdControlli.omlUsl, idCampiObbligatori) Then
            If String.IsNullOrWhiteSpace(datiAggiuntivi.CodiceUsl) Then Return False
        End If

        If BizRegistrazioneVaccinazioni.IsCampoObbligatorio(IdControlli.omlComuneStato, idCampiObbligatori) Then
            If String.IsNullOrWhiteSpace(datiAggiuntivi.CodiceComuneStato) Then Return False

            ' N.B. : anche se il campo comune_stato è impostato come obbligatorio, se il tipo di luogo selezionato è "In azienda"
            '        non deve controllare se è stato valorizzato.
            'If tipoLuogoEsecuzione <> Constants.TipoLuogoEsecuzioneVaccinazione.InAzienda Then
            '    If String.IsNullOrWhiteSpace(datiAggiuntivi.CodiceComuneStato) Then Return False
            'End If

        End If

        If BizRegistrazioneVaccinazioni.IsCampoObbligatorio(IdControlli.txtNoteVac, idCampiObbligatori) Then
            If String.IsNullOrWhiteSpace(datiAggiuntivi.Note) Then Return False
        End If

        Return True

    End Function

    Private Function CheckDatiPagamentoObbligatori(datiPagamento As BizRegistrazioneVaccinazioni.DatiPagamentoVaccinazione) As Boolean
        If datiPagamento IsNot Nothing Then
            Dim tipoPagamento As New Entities.TipiPagamento

            Using bizNomiCommerciali As New Biz.BizNomiCommerciali(GenericProvider, Settings, ContextInfos)
                '--
                tipoPagamento = bizNomiCommerciali.GetTipoPagamento(datiPagamento.GuidTipoPagamento)
                '--
            End Using

            If tipoPagamento.FlagStatoCampoEsenzione = StatoAbilitazioneCampo.Obbligatorio AndAlso String.IsNullOrWhiteSpace(datiPagamento.CodiceEsenzione) Then
                Return False
            End If

            If tipoPagamento.FlagStatoCampoImporto = StatoAbilitazioneCampo.Obbligatorio AndAlso String.IsNullOrWhiteSpace(datiPagamento.Importo) Then
                Return False
            End If

        End If
        Return True
    End Function

#End Region


End Class
