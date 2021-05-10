Imports System.Collections.Generic
Imports System.Text
Imports Newtonsoft.Json
Imports Onit.OnAssistnet.OnVac.Collection
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.Filters
Imports Onit.OnAssistnet.OnVac.Log
Imports Onit.OnAssistnet.OnVac.Log.DataLogManager

Public Class BizPaziente
    Inherits BizClass

#Region " Consts "

    Public Const PazienteCentraleTipoMaster As String = "m"
    Public Const PazienteCentraleTipoProvvisorio As String = "P"
    Public Const PazienteCentraleTipoAlias As String = "A"

#End Region

#Region " Fields / Properties "

    Public Filtri As FiltriRicercaPaziente = New FiltriRicercaPaziente()

#End Region

#Region " Proprietà per Criteri Calcolo Consultori "

    Private _CalcoloConsultorioCriterias As IEnumerable(Of CalcoloConsultorioCriteria)
    Protected Overridable ReadOnly Property CalcoloConsultorioCriterias() As IEnumerable(Of CalcoloConsultorioCriteria)
        Get
            If _CalcoloConsultorioCriterias Is Nothing Then
                Dim calcoloConsultorioDefaultModeList As New List(Of CalcoloConsultorioCriteria)
                calcoloConsultorioDefaultModeList.Add(CalcoloConsultorioCriteria.ComuneResidenza)
                calcoloConsultorioDefaultModeList.Add(CalcoloConsultorioCriteria.ComuneDomicilio)
                calcoloConsultorioDefaultModeList.Add(CalcoloConsultorioCriteria.Smistamento)
                _CalcoloConsultorioCriterias = calcoloConsultorioDefaultModeList.AsEnumerable()
            End If
            Return _CalcoloConsultorioCriterias
        End Get
    End Property

    Protected Overridable ReadOnly Property CalcoloConsultorioTerritorialeCriterias() As IEnumerable(Of CalcoloConsultorioCriteria)
        Get
            Return Nothing
        End Get
    End Property

#End Region

#Region " Costruttori "

    Public Sub New(dbGenericProvider As DbGenericProvider, settings As Settings.Settings, uslGestitaAllineaSettingsProvider As BizUslGestitaAllineaSettingsProvider, contextInfos As BizContextInfos, logOptions As BizLogOptions)
        MyBase.New(dbGenericProvider, settings, uslGestitaAllineaSettingsProvider, contextInfos, logOptions)
    End Sub

    Public Sub New(dbGenericProviderFactory As DbGenericProviderFactory, settings As Settings.Settings, uslGestitaAllineaSettingsProvider As BizUslGestitaAllineaSettingsProvider, contextInfos As BizContextInfos, logOptions As BizLogOptions)
        MyBase.New(dbGenericProviderFactory, settings, uslGestitaAllineaSettingsProvider, contextInfos, logOptions)
    End Sub

    Public Sub New(dbGenericProviderFactory As DbGenericProviderFactory, settings As Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)
        MyBase.New(dbGenericProviderFactory, settings, Nothing, contextInfos, logOptions)
    End Sub

    Public Sub New(ByRef settings As Settings.Settings, uslGestitaAllineaSettingsProvider As BizUslGestitaAllineaSettingsProvider, ByVal contextInfos As BizContextInfos, ByVal logOptions As BizLogOptions)
        MyBase.New(settings, uslGestitaAllineaSettingsProvider, contextInfos, logOptions)
    End Sub

    Public Sub New(dbGenericProvider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)
        Me.New(dbGenericProvider, settings, Nothing, contextInfos, logOptions)
    End Sub

#End Region

#Region " Ricerca Pazienti "

    ''' <summary>
    ''' Restituisce un datatable con i dati del paziente specificato.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFromKey(codicePaziente As Integer) As DataTable

        Return GenericProvider.Paziente.GetFromKey(codicePaziente)

    End Function

#Region " Integrazione Anagrafica "

    Public Class RicercaQPv2Result
        Inherits BizGenericResult

        Public Property Paziente As Paziente

    End Class

    Private Class QPv2Response
        Public Property Result As BizGenericResult
        Public Property Paziente As PazienteAuraRicevutoConUnderscore
    End Class

    <Serializable>
    Private Class PazienteAuraRicevutoConUnderscore

        Public Property _mpi As String
        Public Property _fiscalCode As String
        Public Property _stp As String
        Public Property _stpDateBegin As Date?
        Public Property _stpDateEnd As Date?
        Public Property _cs As String
        Public Property _csRegion As String
        Public Property _csDateBegin As Date?
        Public Property _csDateEnd As Date?
        Public Property _eni As String
        Public Property _eniDateBegin As Date?
        Public Property _eniDateEnd As Date?
        Public Property _teamPers As String
        Public Property _teamInst As String
        Public Property _teamDateEnd As Date?
        Public Property _teamIdent As String
        Public Property _teamCode As String
        Public Property _nameFam As String
        Public Property _nameGiv As String
        Public Property _birthTime As Date
        Public Property _genderCode As String
        Public Property _birthplaceCode As String
        Public Property _countryOfBirth As String
        Public Property _addrStr As String
        Public Property _addrBnr As String
        Public Property _addrCode As String
        Public Property _addrZip As String
        Public Property _countryOfAddr As String
        Public Property _domAddrStr As String
        Public Property _domAddrBnr As String
        Public Property _domAddrCode As String
        Public Property _domAddrZip As String
        Public Property _countryOfDom As String
        Public Property _telecomH As String
        Public Property _telecomHp As String
        Public Property _telecomBad As String
        Public Property _telecomMc As String
        Public Property _telecomMail As String
        Public Property _telecomTmp As String
        Public Property _telecomEc As String
        Public Property _telecomPg As String
        Public Property _deathDate As Date?
        Public Property _reliability As String
        Public Property _mmgRegionalCode As String
        Public Property _mmgNameFam As String
        Public Property _mmgNameGiv As String
        Public Property _mmgDateBegin As Date?
        Public Property _mmgDateEnd As Date?
        Public Property _istatUlssAss As String
        Public Property _citizenship As String
        Public Property _category As String

    End Class

    Public Function RicercaQPv2(codiceFiscalePaziente As String) As RicercaQPv2Result

        Dim pazAura As BizCovid19Tamponi.PazienteAura

        Try
            Dim url As String = String.Format(Settings.RICERCA_PAZ_QPV2_API_ENDPOINT, codiceFiscalePaziente)

            Dim request As Net.WebRequest = Net.WebRequest.Create(url)

            ' If required by the server, set the credentials.
            request.Credentials = Net.CredentialCache.DefaultCredentials

            ' Get the response.
            Using response As Net.WebResponse = request.GetResponse()

                ' Display the status.
                'Console.WriteLine(CType(response, HttpWebResponse).StatusDescription)

                ' Get the stream containing content returned by the server.
                Using dataStream As IO.Stream = response.GetResponseStream()

                    ' Open the stream using a StreamReader for easy access.
                    Using reader As New IO.StreamReader(dataStream)

                        ' Read the content.
                        Dim responseFromServer As String = reader.ReadToEnd()

                        Dim qpv2Response As QPv2Response = JsonConvert.DeserializeObject(Of QPv2Response)(responseFromServer)

                        If Not qpv2Response.Result.Success Then

                            Return New RicercaQPv2Result() With
                            {
                                .Success = False,
                                .Message = qpv2Response.Result.Message
                            }

                        End If

                        pazAura = New BizCovid19Tamponi.PazienteAura() With
                        {
                            .mpi = qpv2Response.Paziente._mpi,
                            .fiscalCode = qpv2Response.Paziente._fiscalCode,
                            .stp = qpv2Response.Paziente._stp,
                            .stpDateBegin = qpv2Response.Paziente._stpDateBegin,
                            .stpDateEnd = qpv2Response.Paziente._stpDateEnd,
                            .cs = qpv2Response.Paziente._cs,
                            .csRegion = qpv2Response.Paziente._csRegion,
                            .csDateBegin = qpv2Response.Paziente._csDateBegin,
                            .csDateEnd = qpv2Response.Paziente._csDateEnd,
                            .eni = qpv2Response.Paziente._eni,
                            .eniDateBegin = qpv2Response.Paziente._eniDateBegin,
                            .eniDateEnd = qpv2Response.Paziente._eniDateEnd,
                            .teamPers = qpv2Response.Paziente._teamPers,
                            .teamInst = qpv2Response.Paziente._teamInst,
                            .teamDateEnd = qpv2Response.Paziente._teamDateEnd,
                            .teamIdent = qpv2Response.Paziente._teamIdent,
                            .teamCode = qpv2Response.Paziente._teamCode,
                            .nameFam = qpv2Response.Paziente._nameFam,
                            .nameGiv = qpv2Response.Paziente._nameGiv,
                            .birthTime = qpv2Response.Paziente._birthTime,
                            .genderCode = qpv2Response.Paziente._genderCode,
                            .birthplaceCode = qpv2Response.Paziente._birthplaceCode,
                            .countryOfBirth = qpv2Response.Paziente._countryOfBirth,
                            .addrStr = qpv2Response.Paziente._addrStr,
                            .addrBnr = qpv2Response.Paziente._addrBnr,
                            .addrCode = qpv2Response.Paziente._addrCode,
                            .addrZip = qpv2Response.Paziente._addrZip,
                            .countryOfAddr = qpv2Response.Paziente._countryOfAddr,
                            .domAddrStr = qpv2Response.Paziente._domAddrStr,
                            .domAddrBnr = qpv2Response.Paziente._domAddrBnr,
                            .domAddrCode = qpv2Response.Paziente._domAddrCode,
                            .domAddrZip = qpv2Response.Paziente._domAddrZip,
                            .countryOfDom = qpv2Response.Paziente._countryOfDom,
                            .telecomH = qpv2Response.Paziente._telecomH,
                            .telecomHp = qpv2Response.Paziente._telecomHp,
                            .telecomBad = qpv2Response.Paziente._telecomBad,
                            .telecomMc = qpv2Response.Paziente._telecomMc,
                            .telecomMail = qpv2Response.Paziente._telecomMail,
                            .telecomTmp = qpv2Response.Paziente._telecomTmp,
                            .telecomEc = qpv2Response.Paziente._telecomEc,
                            .telecomPg = qpv2Response.Paziente._telecomPg,
                            .deathDate = qpv2Response.Paziente._deathDate,
                            .reliability = qpv2Response.Paziente._reliability,
                            .mmgRegionalCode = qpv2Response.Paziente._mmgRegionalCode,
                            .mmgNameFam = qpv2Response.Paziente._mmgNameFam,
                            .mmgNameGiv = qpv2Response.Paziente._mmgNameGiv,
                            .mmgDateBegin = qpv2Response.Paziente._mmgDateBegin,
                            .mmgDateEnd = qpv2Response.Paziente._mmgDateEnd,
                            .istatUlssAss = qpv2Response.Paziente._istatUlssAss,
                            .citizenship = qpv2Response.Paziente._citizenship,
                            .category = qpv2Response.Paziente._category
                        }

                    End Using
                End Using
            End Using

        Catch ex As Exception

            Return New RicercaQPv2Result() With
            {
                .Success = False,
                .Message = ex.Message
            }

        End Try

        Dim pazienteVac As Paziente

        Using bizTamponi As New BizCovid19Tamponi(GenericProvider, Settings, ContextInfos)

            pazienteVac = bizTamponi.MappingPazienteAuraToPazienteVac(pazAura)

        End Using

        Return New RicercaQPv2Result() With
        {
            .Success = True,
            .Message = String.Empty,
            .Paziente = pazienteVac
        }

    End Function

    ''' <summary>
    ''' Lo usa MVC
    ''' </summary>
    ''' <param name="filtro"></param>
    ''' <returns></returns>
    Public Function RicercaPazientiLocale(filtro As FiltroRicercaPaziente) As RicercaPazientiResult

        VerificaCriteriRicercaPaziente(filtro)

        Dim maxRecordRaggiunto As Boolean = False

        Dim maxRecord As Integer?

        If Not filtro.MaxRecords.HasValue Then
            filtro.MaxRecords = 200
        End If

        If filtro.MaxRecords.HasValue Then filtro.MaxRecords += 1

        ' ---
        ' Ricerca in locale
        ' ---
        Dim pazientiTrovati As List(Of PazienteTrovato) = GenericProvider.Paziente.RicercaPazientiLocale(filtro, Settings.RICERCA_PAZ_ORDINAMENTO).Take(filtro.MaxRecords).ToList()

        'calcolo della chiave identificativa
        For Each p As PazienteTrovato In pazientiTrovati
            p.ID = String.Format("{0}|{1}|{2}|{3}", p.Fonte, p.CodiceLocale, p.CodiceCentrale, p.CodiceRegionale)
        Next

#Region " Non gestiti "

        ' ---------------------------------------------------------------------------------------- '
        ' TODO [RicPaz]: i flag vaccinali della ricerca per ora non sono gestiti

        '' calcolo dei flag vaccinali: solo per pazienti importati
        'Dim risultatiLocali As List(Of PazienteTrovato) = (From p In result Where (p.Fonte = Enumerators.FonteAnagrafica.AnagrafeLocale OrElse p.Fonte = Enumerators.FonteAnagrafica.Mista) AndAlso p.CodiceLocale.HasValue Select p).ToList()

        'If risultatiLocali.Any Then

        '    Dim codiciLocali As List(Of Integer) = risultatiLocali.Select(Function(p) p.CodiceLocale.Value).ToList()

        '    If Settings.RICERCA_PAZ_SHOW_FLAG_VACCINAZIONI Then
        '        Using bizVaccinazioniEseguite As New BizVaccinazioniEseguite(GenericProvider, Settings, ContextInfos, Nothing)
        '            Dim iconeVaccinazioni As List(Of KeyValuePair(Of Integer, Integer)) = bizVaccinazioniEseguite.CountVaccinazioniEseguite(codiciLocali, True)
        '            For Each paz As PazienteTrovato In risultatiLocali
        '                paz.VaccinazioniEseguite = iconeVaccinazioni.Where(Function(p) p.Key = paz.CodiceLocale.Value).Select(Function(q) q.Value).FirstOrDefault()
        '            Next
        '        End Using
        '    End If

        '    If Settings.RICERCA_PAZ_SHOW_FLAG_APPUNTAMENTI Then
        '        Using bizCnv As New BizConvocazione(GenericProvider, Settings, ContextInfos, Nothing)
        '            Dim iconeAppuntamenti As List(Of KeyValuePair(Of Integer, Integer)) = bizCnv.CountConvocazioniConAppuntamento(codiciLocali)
        '            For Each paz As PazienteTrovato In risultatiLocali
        '                paz.Appuntamenti = iconeAppuntamenti.Where(Function(p) p.Key = paz.CodiceLocale.Value).Select(Function(q) q.Value).FirstOrDefault()
        '            Next
        '        End Using
        '    End If

        '    If Settings.RICERCA_PAZ_SHOW_FLAG_ESCLUSIONI Then
        '        Using bizVaccinazioniEscluse As New BizVaccinazioniEscluse(GenericProvider, Settings, ContextInfos, Nothing)
        '            Dim iconeEscluse As List(Of KeyValuePair(Of Integer, Integer)) = bizVaccinazioniEscluse.CountVaccinazioniEscluse(codiciLocali)
        '            For Each paz As PazienteTrovato In risultatiLocali
        '                paz.VaccinazioniEscluse = iconeEscluse.Where(Function(p) p.Key = paz.CodiceLocale.Value).Select(Function(q) q.Value).FirstOrDefault()
        '            Next
        '        End Using
        '    End If

        'End If
        ' ---------------------------------------------------------------------------------------- '

#End Region

        Return New RicercaPazientiResult() With {
            .ListaPazienti = pazientiTrovati,
            .MaxRecordRaggiunto = maxRecordRaggiunto
        }

    End Function


    Public Function RicercaPazientiCentrale(filtro As FiltroRicercaPaziente) As RicercaPazientiResult

        VerificaCriteriRicercaPaziente(filtro)

        Dim maxRecordRaggiunto As Boolean = False
        Dim fromQPv2 As Boolean = False
        Dim pazienteFromQPv2 As Paziente = Nothing

        filtro.MaxRecords = Settings.RICERCA_PAZ_MAX_RECORDS + 1

        Dim pazientiTrovati As List(Of PazienteTrovato) = GenericProviderCentrale.Paziente.RicercaPazientiCentrale(filtro, Settings.RICERCA_PAZ_ORDINAMENTO)

        If Settings.RICERCA_PAZ_MAX_RECORDS.HasValue AndAlso
           Settings.RICERCA_PAZ_MAX_RECORDS.Value > 0 AndAlso
           pazientiTrovati.Count > Settings.RICERCA_PAZ_MAX_RECORDS.Value Then

            pazientiTrovati = pazientiTrovati.Take(Settings.RICERCA_PAZ_MAX_RECORDS.Value).ToList()
            maxRecordRaggiunto = True

        End If

        If Settings.RICERCA_PAZ_QPV2 AndAlso pazientiTrovati.Count = 0 Then

            ' TODO [QPv2]: effettuare ricerca QPv2 solo se l'unico filtro impostato è il codice fiscale?

            If Not String.IsNullOrWhiteSpace(filtro.CodiceFiscale) Then

                ' Chiamata al servizio QPv2
                Dim qpv2Result As RicercaQPv2Result = RicercaQPv2(filtro.CodiceFiscale)

                fromQPv2 = True
                pazienteFromQPv2 = qpv2Result.Paziente

                If qpv2Result.Success Then

                    ' Mapping
                    Dim paz As New PazienteTrovato()

                    paz.Fonte = Enumerators.FonteAnagrafica.AnagrafeCentrale

                    paz.Cancellato = qpv2Result.Paziente.FlagCancellato = "S"
                    paz.CodiceCentrale = qpv2Result.Paziente.CodiceAusiliario
                    paz.CodiceCentroVaccinale = qpv2Result.Paziente.Paz_Cns_Codice
                    paz.CodiceFiscale = qpv2Result.Paziente.PAZ_CODICE_FISCALE
                    paz.CodiceLocale = qpv2Result.Paziente.Paz_Codice
                    paz.CodiceRegionale = qpv2Result.Paziente.PAZ_CODICE_REGIONALE
                    paz.CodiceUslAssistenza = qpv2Result.Paziente.UslAssistenza_Codice
                    'paz.CodiceUslDomicilio = qpv2Result.Paziente...
                    paz.Cognome = qpv2Result.Paziente.PAZ_COGNOME
                    paz.ComuneNascita = qpv2Result.Paziente.ComuneNascita_Codice
                    paz.ComuneResidenza = qpv2Result.Paziente.ComuneResidenza_Codice
                    paz.DataNascita = qpv2Result.Paziente.Data_Nascita
                    paz.IndirizzoResidenza = qpv2Result.Paziente.IndirizzoResidenza
                    paz.Nome = qpv2Result.Paziente.PAZ_NOME
                    paz.PazTipo = qpv2Result.Paziente.Tipo
                    paz.Sesso = qpv2Result.Paziente.Sesso
                    paz.Tessera = qpv2Result.Paziente.Tessera

                    If qpv2Result.Paziente.StatoAnagrafico.HasValue Then
                        Dim valore As Integer = Convert.ToInt32(qpv2Result.Paziente.StatoAnagrafico.Value)
                        paz.StatoAnagrafico = valore.ToString()
                    Else
                        paz.StatoAnagrafico = String.Empty
                    End If

                    pazientiTrovati.Add(paz)

                End If

            End If

        End If

        'calcolo della chiave identificativa (mantenuto da vecchia versione, ma qui non credo serva)
        For Each p As PazienteTrovato In pazientiTrovati
            p.ID = String.Format("{0}|{1}|{2}|{3}", p.Fonte, p.CodiceLocale, p.CodiceCentrale, p.CodiceRegionale)
        Next

#Region " Non gestiti "

        ' ---------------------------------------------------------------------------------------- '
        ' TODO [RicPaz]: i flag vaccinali della ricerca per ora non sono gestiti

        '' calcolo dei flag vaccinali: solo per pazienti importati
        'Dim risultatiLocali As List(Of PazienteTrovato) = (From p In result Where (p.Fonte = Enumerators.FonteAnagrafica.AnagrafeLocale OrElse p.Fonte = Enumerators.FonteAnagrafica.Mista) AndAlso p.CodiceLocale.HasValue Select p).ToList()

        'If risultatiLocali.Any Then

        '    Dim codiciLocali As List(Of Integer) = risultatiLocali.Select(Function(p) p.CodiceLocale.Value).ToList()

        '    If Settings.RICERCA_PAZ_SHOW_FLAG_VACCINAZIONI Then
        '        Using bizVaccinazioniEseguite As New BizVaccinazioniEseguite(GenericProvider, Settings, ContextInfos, Nothing)
        '            Dim iconeVaccinazioni As List(Of KeyValuePair(Of Integer, Integer)) = bizVaccinazioniEseguite.CountVaccinazioniEseguite(codiciLocali, True)
        '            For Each paz As PazienteTrovato In risultatiLocali
        '                paz.VaccinazioniEseguite = iconeVaccinazioni.Where(Function(p) p.Key = paz.CodiceLocale.Value).Select(Function(q) q.Value).FirstOrDefault()
        '            Next
        '        End Using
        '    End If

        '    If Settings.RICERCA_PAZ_SHOW_FLAG_APPUNTAMENTI Then
        '        Using bizCnv As New BizConvocazione(GenericProvider, Settings, ContextInfos, Nothing)
        '            Dim iconeAppuntamenti As List(Of KeyValuePair(Of Integer, Integer)) = bizCnv.CountConvocazioniConAppuntamento(codiciLocali)
        '            For Each paz As PazienteTrovato In risultatiLocali
        '                paz.Appuntamenti = iconeAppuntamenti.Where(Function(p) p.Key = paz.CodiceLocale.Value).Select(Function(q) q.Value).FirstOrDefault()
        '            Next
        '        End Using
        '    End If

        '    If Settings.RICERCA_PAZ_SHOW_FLAG_ESCLUSIONI Then
        '        Using bizVaccinazioniEscluse As New BizVaccinazioniEscluse(GenericProvider, Settings, ContextInfos, Nothing)
        '            Dim iconeEscluse As List(Of KeyValuePair(Of Integer, Integer)) = bizVaccinazioniEscluse.CountVaccinazioniEscluse(codiciLocali)
        '            For Each paz As PazienteTrovato In risultatiLocali
        '                paz.VaccinazioniEscluse = iconeEscluse.Where(Function(p) p.Key = paz.CodiceLocale.Value).Select(Function(q) q.Value).FirstOrDefault()
        '            Next
        '        End Using
        '    End If

        'End If
        ' ---------------------------------------------------------------------------------------- '

#End Region

        Return New RicercaPazientiResult() With {
            .ListaPazienti = pazientiTrovati,
            .MaxRecordRaggiunto = maxRecordRaggiunto,
            .InterrogatoServizioQPv2 = fromQPv2,
            .PazienteQPv2 = pazienteFromQPv2
        }

    End Function

    Private Sub VerificaCriteriRicercaPaziente(filtro As FiltroRicercaPaziente)

        ' TODO [RicPaz]: implementare i filtri minimi per la ricerca in centrale

        'If filtro.SoloLocale Then
        '    If Not String.IsNullOrWhiteSpace(filtro.CodiceComuneResidenza) AndAlso filtro.AnnoNascita.HasValue Then
        '        Return
        '    End If
        'End If

        If filtro.CodiceLocale.HasValue Then
            Return
        End If
        If Not String.IsNullOrWhiteSpace(filtro.CodiceCentrale) Then
            Return
        End If
        If Not String.IsNullOrWhiteSpace(filtro.CodiceFiscale) Then
            Return
        End If
        If Not String.IsNullOrWhiteSpace(filtro.CodiceTesseraSanitaria) Then
            Return
        End If
        If Not String.IsNullOrWhiteSpace(filtro.Cognome) OrElse Not String.IsNullOrWhiteSpace(filtro.Nome) Then
            Return
        End If
        If Not String.IsNullOrWhiteSpace(filtro.CognomeNome) Then
            Return
        End If
        If filtro.DataNascita.HasValue Then
            Return
        End If

        Throw New BizToUserMessageException("criteri_insufficienti", "Criteri di ricerca insufficienti.")

    End Sub

    ''' <summary>
    ''' Inserimento del paziente in locale prendendo i dati da centrale
    ''' </summary>
    ''' <param name="codiceAusiliarioPaziente"></param>
    ''' <returns></returns>
    Public Function InserisciPazienteFromCentrale(codiceAusiliarioPaziente As String) As Paziente

        Dim paziente As Paziente = GenericProviderCentrale.Paziente.RicercaPazienteCentrale(codiceAusiliarioPaziente)

        If paziente IsNot Nothing Then

            InserisciPaziente(New InserisciPazienteCommand() With {.Paziente = paziente})

        End If

        Return paziente

    End Function

#End Region

    ''' <summary>
    ''' Restituisce un oggetto Entities.Paziente con i dati del paziente specificato, in base al codice.
    ''' Se non lo trova, restituisce Nothing.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CercaPaziente(codicePaziente As Integer) As Paziente

        Dim collPazienti As Collection.PazienteCollection = GenericProvider.Paziente.GetPazienti(codicePaziente, ContextInfos.CodiceUsl)

        If Not collPazienti Is Nothing AndAlso collPazienti.Count > 0 Then

            Return collPazienti(0)

        End If

        Return Nothing

    End Function

    Public Function GetPazientiByCF(codiceFiscale As String) As List(Of InfoAssistito)

        Return GenericProvider.Paziente.GetPazientiByCF(codiceFiscale)

    End Function

    ''' <summary>
    ''' Ricerca Pazienti corrispondenti ai filtri impostati. 
    ''' </summary>
    ''' <param name="joinComuni">Impostare a true per effettuare il join con le tabelle dei comuni per ottenere comune di nascita, residenza e domicilio</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CercaPazienti(joinComuni As Boolean) As DataTable

        Return GenericProvider.Paziente.GetPazienti(Filtri, joinComuni)

    End Function

    ''' <summary>
    ''' Restituisce una collection di oggetti paziente, contenente i pazienti con i codici specificati
    ''' </summary>
    ''' <param name="codiciPazienti">Stringa di codici separati da virgole</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CercaPazienti(codiciPazienti As String) As Collection.PazienteCollection

        Return GenericProvider.Paziente.GetPazienti(codiciPazienti, ContextInfos.CodiceUsl)

    End Function

    ''' <summary>
    ''' Ricerca dei pazienti deceduti
    ''' </summary>
    ''' <param name="codiceConsultorio"></param>
    ''' <returns></returns>
    ''' <remarks>Le funzioni pubbliche del biz possono essere usate da tutti, ad esempio da OnBatch</remarks>
    Public Function CercaPazientiDecedutiConConvocazioni(codiceConsultorio As String) As Integer()

        Return GenericProvider.Paziente.CercaPazientiDecedutiConConvocazioni(codiceConsultorio)

    End Function

    ''' <summary>
    ''' Restituisce un oggetto Entities.PazienteDatiAnagrafici con i dati del paziente specificato, in base al codice.
    ''' Se non lo trova, restituisce Nothing.
    ''' Se il parametro isGestioneCentrale vale true, effettua la ricerca in centrale.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="isGestioneCentrale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDatiAnagraficiPaziente(codicePaziente As String, isGestioneCentrale As Boolean) As Entities.PazienteDatiAnagrafici

        If isGestioneCentrale Then
            Return GenericProviderCentrale.Paziente.GetDatiAnagraficiPazienteCentrale(codicePaziente)
        End If

        Return GenericProvider.Paziente.GetDatiAnagraficiPaziente(codicePaziente)

    End Function

    ''' <summary>
    ''' Restituisce la data di nascita del paziente specificato.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="isGestioneCentrale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDataNascitaPaziente(codicePaziente As String, isGestioneCentrale As Boolean) As DateTime

        If isGestioneCentrale Then
            Return GenericProviderCentrale.Paziente.GetDataNascitaCentrale(codicePaziente)
        End If

        Return GenericProvider.Paziente.GetDataNascita(codicePaziente)

    End Function

    ''' <summary>
    ''' Restituisce la data di decesso del paziente specificato.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="isGestioneCentrale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDataDecessoPaziente(codicePaziente As String, isGestioneCentrale As Boolean) As DateTime

        If isGestioneCentrale Then
            Return GenericProviderCentrale.Paziente.GetDataDecessoCentrale(codicePaziente)
        End If

        Return GenericProvider.Paziente.GetDataDecesso(codicePaziente)

    End Function

    ''' <summary>
    ''' Restituisce il sesso del paziente specificato.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="isGestioneCentrale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetSessoPaziente(codicePaziente As String, isGestioneCentrale As Boolean) As String

        If isGestioneCentrale Then
            Return GenericProviderCentrale.Paziente.GetSessoPazienteCentrale(codicePaziente)
        End If

        Return GenericProvider.Paziente.GetSessoPaziente(codicePaziente)

    End Function

    ''' <summary>
    ''' Restituisce un hashtable con i valori dei campi specificati per il paziente
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="isGestioneCentrale"></param>
    ''' <param name="campi"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetValoreCampiPaziente(codicePaziente As String, isGestioneCentrale As Boolean, ParamArray campi() As String) As Hashtable

        If isGestioneCentrale OrElse campi Is Nothing OrElse campi.Count = 0 Then Return Nothing

        Return GenericProvider.Paziente.GetValoreCampiPaziente(codicePaziente, campi.ToList())

    End Function

    ''' <summary>
    ''' Restituisce un oggetto Entities.PazienteDatiAnagraficiIntestazioneBilancio con i dati del paziente specificato, in base al codice.
    ''' Se non lo trova, restituisce Nothing.
    ''' Se il parametro isGestioneCentrale vale true, effettua la ricerca in centrale.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="isGestioneCentrale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDatiAnagraficiPazienteIntestazioneBilancio(codicePaziente As String, isGestioneCentrale As Boolean) As PazienteDatiAnagraficiIntestazioneBilancio

        If isGestioneCentrale Then
            Return GenericProviderCentrale.Paziente.GetDatiAnagraficiPazienteCentraleIntestazioneBilancio(codicePaziente)
        End If

        Return GenericProvider.Paziente.GetDatiAnagraficiPazienteIntestazioneBilancio(Convert.ToInt32(codicePaziente))

    End Function

    Public Function CercaPazienteFSE(codicePaziente As Integer) As PazienteFSE

        Dim paziente As PazienteFSE = GenericProvider.Paziente.GetDatiPazienteFSE(codicePaziente)

        Return paziente

    End Function

#End Region

#Region " Campi Obbligatori "

    Public Function GetCampiAnagraficiObbligatori(codicePaziente As Integer, bloccanti As Boolean, Optional ByVal cittadinanza As String = "") As ArrayList

        Dim listCampiObbligatori As New ArrayList()

        Dim dt As DataTable = GenericProvider.Paziente.GetCampiAnagraficiObbligatori(codicePaziente, bloccanti, cittadinanza)

        For i As Integer = 0 To dt.Rows.Count - 1
            listCampiObbligatori.Add(dt.Rows(i)(0).Trim().ToUpper())
        Next

        Return listCampiObbligatori

    End Function

    Public Function ControlloCampiObbligatori(codicePaziente As Integer, ByRef campiObbligatoriMancanti As ArrayList, ByRef campiWarningMancanti As ArrayList) As Boolean

        Dim dt As DataTable = Me.GenericProvider.Paziente.GetFromKey(codicePaziente)

        Dim result As Boolean = False

        If campiObbligatoriMancanti Is Nothing OrElse campiObbligatoriMancanti.Count <> 0 Then
            campiObbligatoriMancanti = New ArrayList()
        End If

        Dim listCampiObbligatori As ArrayList

        listCampiObbligatori = Me.GetCampiAnagraficiObbligatori(codicePaziente, True)

        For i As Integer = 0 To listCampiObbligatori.Count - 1
            If dt.Rows(0)(listCampiObbligatori(i)) Is Nothing OrElse dt.Rows(0)(listCampiObbligatori(i)) Is System.DBNull.Value Then
                campiObbligatoriMancanti.Add(listCampiObbligatori(i))
                result = True
            End If
        Next

        If campiWarningMancanti Is Nothing OrElse campiWarningMancanti.Count <> 0 Then
            campiWarningMancanti = New ArrayList()
        End If

        listCampiObbligatori = Me.GetCampiAnagraficiObbligatori(codicePaziente, False)

        For i As Integer = 0 To listCampiObbligatori.Count - 1
            If dt.Rows(0)(listCampiObbligatori(i)) Is Nothing OrElse dt.Rows(0)(listCampiObbligatori(i)) Is System.DBNull.Value Then
                campiWarningMancanti.Add(listCampiObbligatori(i))
                result = True
            End If
        Next

        Return result

    End Function

#End Region

#Region " Stato Anagrafico "

    Public Function StatiAnagrafici(attiviInChiamata As Boolean) As Hashtable

        Dim dt As New DataTable()
        Dim ht As New Hashtable()

        dt = Me.GenericProvider.StatiAnagrafici.GetStatiAnagrafici()

        For i As Int16 = 0 To dt.Rows.Count - 1
            ' Creo l'hashtable con i codici e le descrizioni degli stati anagrafici,
            ' che servirà per recuperare la descrizione in base al codice dell'elemento.

            If attiviInChiamata Then
                If dt.Rows(i)("SAN_CHIAMATA").ToString() = "S" Then
                    ht.Add(dt.Rows(i)("SAN_CODICE").ToString(), dt.Rows(i)("SAN_DESCRIZIONE").ToString())
                End If
            Else
                ht.Add(dt.Rows(i)("SAN_CODICE").ToString(), dt.Rows(i)("SAN_DESCRIZIONE").ToString())
            End If

        Next

        Return ht

    End Function

    ''' <summary>
    ''' Restituisce la descrizione dello stato anagrafico del paziente
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDescrizioneStatoAnagraficoPaziente(codicePaziente As Integer) As String

        Return Me.GenericProvider.Paziente.statoAnag(codicePaziente)

    End Function

    ''' <summary>
    ''' Modifica lo stato anagrafico del paziente selezionato. Restituisce true se l'operazione va a buon fine.
    ''' Restituisce false se codiceStatoAnagrafico è nullo.
    ''' Effettua la scrittura del log.
    ''' Il codice dello stato anagrafico precedente è utilizzato solo per il log.
    ''' Se il codice dello stato anagrafico non corrisponde ad un codice appartenente all'enumerazione, mantiene il valore specificato 
    ''' perchè potrebbe essere stato aggiunto direttamente sul database.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="codiceStatoAnagrafico"></param>
    ''' <param name="codiceStatoAnagraficoPrecedente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateStatoAnagrafico(codicePaziente As Integer, codiceStatoAnagrafico As String, codiceStatoAnagraficoPrecedente As String) As Boolean

        Dim enumValueStatoAnagrafico As Enumerators.StatoAnagrafico?

        ' Se stato anagrafico nullo -> return false
        If String.IsNullOrEmpty(codiceStatoAnagrafico) Then Return False

        '' Se stato anagrafico non presente nell'enumerazione -> return false
        'If Not [Enum].GetValues(GetType(Enumerators.StatoAnagrafico)).Cast(Of Enumerators.StatoAnagrafico).ToList().Contains(codiceStatoAnagrafico) Then
        '    Return False
        'End If

        Try
            enumValueStatoAnagrafico = [Enum].Parse(GetType(Enumerators.StatoAnagrafico), codiceStatoAnagrafico)
        Catch ex As Exception
            Return False
        End Try

        Dim enumValueStatoAnagraficoPrecedente As Enumerators.StatoAnagrafico?

        If String.IsNullOrEmpty(codiceStatoAnagraficoPrecedente) Then

            enumValueStatoAnagraficoPrecedente = Nothing

        Else

            Try
                enumValueStatoAnagraficoPrecedente = [Enum].Parse(GetType(Enumerators.StatoAnagrafico), codiceStatoAnagraficoPrecedente)
            Catch ex As Exception
                Return False
            End Try

        End If

        Return UpdateStatoAnagrafico(codicePaziente, enumValueStatoAnagrafico, enumValueStatoAnagraficoPrecedente, Nothing)

    End Function

    ''' <summary>
    ''' Modifica lo stato anagrafico del paziente selezionato. Restituisce true se l'operazione va a buon fine.
    ''' Restituisce false se il codice dello stato anagrafico non corrisponde ad uno stato valido per l'applicativo.
    ''' Aggiunge un record alla testata specificata. Se la testata è Nothing, viene creata ed effettuata la scrittura del log.
    ''' Il codice dello stato anagrafico precedente è utilizzato solo per il log (può essere lasciato nullo).
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="codiceStatoAnagrafico"></param>
    ''' <param name="codiceStatoAnagraficoPrecedente"></param>
    ''' <param name="testataLog"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateStatoAnagrafico(codicePaziente As Integer, codiceStatoAnagrafico As Enumerators.StatoAnagrafico?, codiceStatoAnagraficoPrecedente As Enumerators.StatoAnagrafico?, testataLog As DataLogStructure.Testata) As Boolean

        Dim writeLog As Boolean = False

        If testataLog Is Nothing Then
            testataLog = New DataLogStructure.Testata(DataLogStructure.TipiArgomento.PAZIENTI, DataLogStructure.Operazione.Modifica, codicePaziente, False)
            writeLog = True
        End If

        Dim numUpdate As Integer =
            Me.GenericProvider.Paziente.UpdateStatoAnagrafico(codicePaziente, codiceStatoAnagrafico)

        If numUpdate > 0 Then

            Dim statoAnagraficoCorrente As String = String.Empty
            Dim statoAnagraficoPrecedente As String = String.Empty

            If Not codiceStatoAnagrafico Is Nothing Then
                statoAnagraficoCorrente = codiceStatoAnagrafico
            End If

            If Not codiceStatoAnagraficoPrecedente Is Nothing Then
                statoAnagraficoPrecedente = codiceStatoAnagraficoPrecedente
            End If

            Dim recordLog As New DataLogStructure.Record()
            recordLog.Campi.Add(New DataLogStructure.Campo("PAZ_STATO_ANAGRAFICO", statoAnagraficoPrecedente, statoAnagraficoCorrente))

            testataLog.Records.Add(recordLog)

            ' Scrittura del log solo se la testata non è stata passata dal chiamante
            If writeLog Then Log.LogBox.WriteData(testataLog)

        End If

        Return True

    End Function

#End Region

#Region " Stato Vaccinale "

    ''' <summary>
    ''' Restituisce il valore dell'enumerazione corrispondente allo stato vaccinale del paziente.
    ''' Se il valore su db non corrisponde a nessuno dei valori enumerati, restituisce nothing.
    ''' </summary>
    Public Function GetStatoVaccinale(codicePaziente As Integer) As Enumerators.StatiVaccinali

        Return Me.GenericProvider.Paziente.GetCodiceStatoVaccinale(codicePaziente)

    End Function

    ''' <summary>
    ''' Restituisce, in formato stringa, il valore dell'enumerazione corrispondente allo stato vaccinale del paziente.
    ''' Se il valore su db non corrisponde a nessuno dei valori enumerati, restituisce nothing.
    ''' </summary>
    Public Function GetStatoVaccinaleString(codicePaziente As Integer) As String

        Dim statoVaccinale As Enumerators.StatiVaccinali = Me.GenericProvider.Paziente.GetCodiceStatoVaccinale(codicePaziente)

        If statoVaccinale = Nothing Then Return String.Empty

        Return statoVaccinale.ToString("d")

    End Function

    ''' <summary>
    ''' Esegue la modifica dello stato vaccinale del paziente specificato da statoVaccinaleOld a statoVaccinaleNew
    ''' Scrive il log dell'operazione effettuata.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="statoVaccinaleOld"></param>
    ''' <param name="statoVaccinaleNew"></param>
    ''' <remarks></remarks>
    Public Function UpdateStatoVaccinalePaziente(codicePaziente As Integer, statoVaccinaleOld As Enumerators.StatiVaccinali, statoVaccinaleNew As Enumerators.StatiVaccinali) As Integer

        Return UpdateStatoVaccinale(codicePaziente, statoVaccinaleOld, statoVaccinaleNew)

    End Function

    ''' <summary>
    ''' Esegue la modifica dello stato vaccinale del paziente specificato, senza controllare lo stato precedente.
    ''' Scrive il log dell'operazione effettuata.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="statoVaccinaleNew"></param>
    ''' <remarks></remarks>
    Public Function UpdateStatoVaccinalePaziente(codicePaziente As Integer, statoVaccinaleNew As Enumerators.StatiVaccinali) As Integer

        Return UpdateStatoVaccinale(codicePaziente, Nothing, statoVaccinaleNew)

    End Function

    Private Function UpdateStatoVaccinale(codicePaziente As Integer, statoVaccinaleOld As Enumerators.StatiVaccinali?, statoVaccinaleNew As Enumerators.StatiVaccinali) As Integer

        Dim newStato As String = Convert.ToString(statoVaccinaleNew)

        Dim oldStato As String = String.Empty
        If statoVaccinaleOld.HasValue Then oldStato = Convert.ToString(statoVaccinaleOld.Value)

        Dim count As Integer = Me.GenericProvider.Paziente.UpdateStatoVaccinalePaziente(codicePaziente, oldStato, newStato)

        If count > 0 Then

            Dim testataLog As New DataLogStructure.Testata(DataLogStructure.TipiArgomento.VAR_ANA_AUTO, DataLogStructure.Operazione.Modifica, True)
            Dim recordLog As New DataLogStructure.Record()

            recordLog.Campi.Add(New DataLogStructure.Campo("PAZ_STATO", oldStato, newStato))
            testataLog.Records.Add(recordLog)

            LogBox.WriteData(testataLog)

        End If

        Return count

    End Function

#End Region

#Region " AggiornaCNSPazientiFuoriEta "

    ''' <summary> Ricerca dei pazienti deceduti</summary>
    ''' <remarks>Le funzioni pubbliche del biz possono essere usate da tutti, ad esempio da OnBatch</remarks>
    Public Function CercaPazientiFuoriEtaCns(filtroRicercaPazienti As Filters.FiltriRicercaPaziente, aggiornaAnchePazientiConAppuntamenti As Boolean) As Integer()

        Return Me.GenericProvider.Paziente.CercaPazientiFuoriEtaCns(filtroRicercaPazienti, aggiornaAnchePazientiConAppuntamenti)

    End Function

    ''' <summary>
    ''' Seleziona il nuovo consultorio in base a quelli disponibili secondo il criterio passato
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="selectionOrder"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CercaNuovoCns(codicePaziente As Integer, selectionOrder As String) As String

        Dim htCns As Hashtable
        Dim keys As String() = selectionOrder.Split("|")

        htCns = Me.GenericProvider.Paziente.CercaNuovoCns(codicePaziente)

        If Not keys Is Nothing Then

            For i As Integer = 0 To keys.Length - 1
                If htCns.ContainsKey(keys(i)) Then
                    Return htCns(keys(i)).ToString()
                End If
            Next

        End If

        Return String.Empty

    End Function

#End Region

#Region " Count "

    ''' <summary>
    ''' Restituisce il numero di documenti relativi al paziente specificato
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="isGestioneCentrale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountDocumentiPaziente(codicePaziente As String, isGestioneCentrale As Boolean) As Integer

        If isGestioneCentrale Then Return 0

        Return Me.GenericProvider.Paziente.CountDocumenti(codicePaziente)

    End Function

    ''' <summary>
    ''' Restituisce il numero di inadempienze relative al paziente specificato
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="isGestioneCentrale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountInadempienzePaziente(codicePaziente As String, isGestioneCentrale As Boolean) As Integer

        If isGestioneCentrale Then Return 0

        Return Me.GenericProvider.Paziente.CountInadempienze(codicePaziente)

    End Function

    ''' <summary>
    ''' Restituisce il numero di visite relative al paziente specificato
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="isGestioneCentrale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountVisitePaziente(codicePaziente As String, isGestioneCentrale As Boolean) As Integer

        If isGestioneCentrale Then Return 0

        Return Me.GenericProvider.Paziente.CountVisite(codicePaziente)

    End Function

    ''' <summary>
    ''' Restituisce il numero di rifiuti relativi al paziente specificato
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="isGestioneCentrale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountRifiutiPaziente(codicePaziente As String, isGestioneCentrale As Boolean) As Integer

        If isGestioneCentrale Then Return 0

        Return Me.GenericProvider.Paziente.CountRifiuti(codicePaziente)

    End Function

    ''' <summary>
    ''' Restituisce il numero di consulenze relative al paziente specificato
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="isGestioneCentrale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountConsulenzePaziente(codicePaziente As String, isGestioneCentrale As Boolean) As Integer

        If isGestioneCentrale Then Return 0

    End Function

#End Region

#Region " Consultorio "

    ''' <summary>
    ''' Restituisce il codice del consultorio vaccinale a cui appartiene il paziente specificato.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCodiceConsultorio(codicePaziente As Integer) As String

        Return GenericProvider.Paziente.GetCodiceConsultorio(codicePaziente)

    End Function

    Public Class ModificaConsultorioPazienteCommand

        Public CodicePaziente As Long
        Public CodiceConsultorioNew As String
        Public CodiceConsultorioOld As String
        Public DataAssegnazioneConsultorio As DateTime
        Public FlagInvioCartella As Boolean
        Public FlagMovimentoAutomaticoPassaggioAdulti As Boolean
        Public DataEliminazione As DateTime
        Public NoteEliminazione As String
        Public UpdateConsultoriAnagraficaPaziente As UpdateConsultoriAnagraficaPazienteType

        ''' <summary>
        ''' Se vale true, vengono modificati i dati delle sole convocazioni relative al consultorio OLD.
        ''' </summary>
        ''' <remarks></remarks>
        Public UpdateConvocazioniSoloConsultorioOld As Boolean

    End Class

    Public Class ModificaConsultorioPazienteResult
        Public CountMovimentiInseriti As Integer
        Public CountConvocazioniModificate As Integer
        Public CountConsultoriModificati As Integer
    End Class

    Public Enum UpdateConsultoriAnagraficaPazienteType

        ''' <summary>
        ''' Non effettua l'update dei consultori.
        ''' </summary>
        ''' <remarks></remarks>
        NoUpdateConsultori

        ''' <summary>
        ''' Effettua solo l'update del consultorio vaccinale (e del vaccinale precedente).
        ''' </summary>
        ''' <remarks></remarks>
        UpdateConsultorioVaccinaleOnly

        ''' <summary>
        ''' Effettua l'update del vaccinale, del precedente e del consultorio territoriale.
        ''' </summary>
        ''' <remarks></remarks>
        UpdateConsultorioVaccinaleETerritoriale

    End Enum

    Public Function ModificaConsultorioPaziente(command As ModificaConsultorioPazienteCommand) As ModificaConsultorioPazienteResult

        Dim result As New ModificaConsultorioPazienteResult()

        If String.IsNullOrWhiteSpace(command.CodiceConsultorioNew) OrElse command.CodiceConsultorioOld = command.CodiceConsultorioNew Then
            Return result
        End If

        If command.UpdateConsultoriAnagraficaPaziente <> UpdateConsultoriAnagraficaPazienteType.NoUpdateConsultori Then

            ' Nell'anagrafica del paziente, aggiornamento del consultorio vaccinale, del consultorio vaccinale precedente e, 
            ' se specificato, del consultorio territoriale
            Dim updateConsultorioTerritoriale As Boolean =
                (command.UpdateConsultoriAnagraficaPaziente = UpdateConsultoriAnagraficaPazienteType.UpdateConsultorioVaccinaleETerritoriale)

            result.CountConsultoriModificati =
                Me.GenericProvider.Paziente.UpdateConsultori(command.CodicePaziente, command.CodiceConsultorioOld, command.CodiceConsultorioNew,
                                                             command.DataAssegnazioneConsultorio, updateConsultorioTerritoriale)

        End If

        ' Inserisce un movimento per tenere traccia della variazione di consultorio
        result.CountMovimentiInseriti =
            Me.GenericProvider.MovimentiInterniCNS.InserimentoMovimentoPaziente(command.CodicePaziente, command.CodiceConsultorioOld, command.CodiceConsultorioNew,
                                                                                command.DataAssegnazioneConsultorio, command.FlagInvioCartella,
                                                                                False, command.FlagMovimentoAutomaticoPassaggioAdulti)

        ' Cerca tutte le convocazioni del paziente
        Dim convocazioni As List(Of Entities.Convocazione) = Me.GenericProvider.Convocazione.GetConvocazioniPaziente(command.CodicePaziente)
        If convocazioni.IsNullOrEmpty() Then
            ' Non ci sono convocazioni
            Return result
        End If

        ' Filtra le sole convocazioni nel consultorio vecchio
        If command.UpdateConvocazioniSoloConsultorioOld Then

            convocazioni = convocazioni.Where(Function(cnv) cnv.Cns_Codice = command.CodiceConsultorioOld).ToList()
            If convocazioni.IsNullOrEmpty() Then
                ' Non ci sono convocazioni sul vecchio consultorio
                Return result
            End If

        End If

        Using bizConvocazione As New BizConvocazione(Me.GenericProvider, Me.Settings, Me.ContextInfos, Me.LogOptions)

            Dim dataEliminazioneAppuntamento As DateTime = command.DataEliminazione
            If dataEliminazioneAppuntamento = DateTime.MinValue Then dataEliminazioneAppuntamento = DateTime.Now

            Dim noteEliminazioneAppuntamento As String = command.NoteEliminazione
            If String.IsNullOrWhiteSpace(noteEliminazioneAppuntamento) Then noteEliminazioneAppuntamento = "Eliminazione appuntamento per variazione centro vaccinale"

            Dim count As Integer = 0

            For Each convocazione As Entities.Convocazione In convocazioni

                convocazione.Cns_Codice = command.CodiceConsultorioNew
                convocazione.DataAppuntamento = DateTime.MinValue
                convocazione.DataInvio = DateTime.MinValue

                Dim convocazioneAppuntamento As Entities.ConvocazioneAppuntamento =
                    bizConvocazione.CreateConvocazioneAppuntamento(convocazione, noteEliminazioneAppuntamento, command.DataAssegnazioneConsultorio)

                convocazioneAppuntamento.DataEliminazioneAppuntamento = dataEliminazioneAppuntamento
                convocazioneAppuntamento.IdUtenteEliminazioneAppuntamento = Me.ContextInfos.IDUtente
                convocazioneAppuntamento.IdMotivoEliminazioneAppuntamento = Constants.MotiviEliminazioneAppuntamento.VariazioneConsultorio
                convocazioneAppuntamento.CodiceAmbulatorio = Nothing

                count += bizConvocazione.UpdateConvocazione_DatiAppuntamento(convocazioneAppuntamento)

            Next

            result.CountConvocazioniModificate = count

        End Using

        Return result

    End Function

#End Region

#Region " Comuni "

    Public Class CheckComuniPazienteCommand
        Public Property CodiceComuneNascita As String
        Public Property DataNascita As Date?
        Public Property CodiceComuneResidenza As String
        Public Property CodiceComuneDomicilio As String
        Public Property CodiceComuneEmigrazione As String
        Public Property DataEmigrazione As Date?
        Public Property CodiceComuneImmigrazione As String
        Public Property DataImmigrazione As Date?
    End Class

    Public Function CheckComuniPaziente(command As CheckComuniPazienteCommand) As BizGenericResult

        Dim errorMsg As New StringBuilder()

        If Not ControllaValiditaComune(command.CodiceComuneNascita, command.DataNascita) Then
            errorMsg.AppendFormat(" - Comune di nascita non valido.{0}", Environment.NewLine)
        End If

        If Not ControllaValiditaComune(command.CodiceComuneResidenza, Nothing) Then
            errorMsg.AppendFormat(" - Comune di residenza non valido.{0}", Environment.NewLine)
        End If

        If Not ControllaValiditaComune(command.CodiceComuneDomicilio, Nothing) Then
            errorMsg.AppendFormat(" - Comune di domicilio non valido.{0}", Environment.NewLine)
        End If

        If Not ControllaValiditaComune(command.CodiceComuneEmigrazione, command.DataEmigrazione) Then
            errorMsg.AppendFormat(" - Comune di emigrazione non valido.{0}", Environment.NewLine)
        End If

        If Not ControllaValiditaComune(command.CodiceComuneImmigrazione, command.DataImmigrazione) Then
            errorMsg.AppendFormat(" - Comune di immigrazione non valido.{0}", Environment.NewLine)
        End If

        Return New BizGenericResult With {
            .Success = errorMsg.Length = 0,
            .Message = errorMsg.ToString()
        }

    End Function

    Private Function ControllaValiditaComune(codiceComune As String, dataValidita As Date?) As Boolean

        If String.IsNullOrWhiteSpace(codiceComune) Then Return True

        Dim dataControllo As Date = Date.Now

        If dataValidita.HasValue Then dataControllo = dataValidita.Value

        Using bizComuni As New BizComuni(GenericProvider, Settings, ContextInfos, LogOptions)
            Return bizComuni.CheckValiditaComune(codiceComune, dataControllo)
        End Using

    End Function

#End Region

    Public Function GetCodicePazientiByCodiceAusiliario(codiceAusiliarioPaziente As String) As List(Of String)

        Dim codiciPazienti As ICollection(Of String) = Me.GenericProvider.Paziente.GetCodicePazientiByCodiceAusiliario(codiceAusiliarioPaziente)

        If codiciPazienti Is Nothing Then Return New List(Of String)()

        Return codiciPazienti.ToList()

    End Function

    ''' <summary>
    ''' Restituisce il codice ausiliario del paziente specificato.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCodiceAusiliario(codicePaziente As Integer) As String

        Return Me.GenericProvider.Paziente.GetCodiceAusiliario(codicePaziente)

    End Function

    ''' <summary>
    ''' Restituisce il codice regionale del paziente specificato.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCodiceRegionale(codicePaziente As Integer) As String

        Return Me.GenericProvider.Paziente.GetCodiceRegionale(codicePaziente)

    End Function
    Public Function GetCodicePazienteByCodiceRegionale(codiceRegionale As String) As List(Of String)
        Return GenericProvider.Paziente.GetCodicePazienteByCodiceRegionale(codiceRegionale)
    End Function

    '''' <summary>
    '''' Restituisce le note di acquisizione del paziente specificato
    '''' </summary>
    '''' <param name="codicePaziente"></param>
    '''' <returns></returns>
    '''' <remarks></remarks>
    'Public Function GetNoteAcquisizione(codicePaziente As Integer) As String

    '    Return Me.GenericProvider.Paziente.GetNoteAcquisizione(codicePaziente)

    'End Function


    Public Function GetDescrizioneNotePaziente(codiceTipoNota As String) As String

        Return GenericProvider.Paziente.GetDescrizioneNotePaziente(codiceTipoNota)

    End Function


    Public Function GetNotePaziente(codicePaziente As Long, codiceUsl As String) As List(Of Entities.PazienteNote)

        Return GenericProvider.Paziente.GetNotePaziente(codicePaziente, codiceUsl)

    End Function

    Public Function UpdateNotePaziente(idNote As Long?, codicePaziente As Long, codiceUsl As String, codiceTipoNote As String, testoNote As String) As Integer

        Dim now As DateTime = DateTime.Now

        If idNote.HasValue Then
            Return GenericProvider.Paziente.UpdateNotePaziente(idNote, codicePaziente, codiceUsl, codiceTipoNote, IIf(String.IsNullOrWhiteSpace(testoNote), Nothing, testoNote), ContextInfos.IDUtente, now)
        Else
            If Not String.IsNullOrWhiteSpace(testoNote) Then
                Return GenericProvider.Paziente.InsertNotePaziente(codicePaziente, codiceUsl, codiceTipoNote, testoNote, ContextInfos.IDUtente, now)
            End If
        End If
    End Function

    ''' <summary>
    ''' Restituisce la data massima di fine sospensione del paziente.
    ''' Se il paziente è sospeso senza data fine o se non è sospeso, restituisce Date.MinValue.
    ''' </summary>
    Public Function GetMaxDataFineSospensione(codicePaziente As Integer) As Date

        Return Me.GenericProvider.Paziente.GetMaxDataFineSospensione(codicePaziente)

    End Function

    ''' <summary>
    ''' Restituisce True se la data di decesso del paziente è valorizzata o lo stato anagrafico del paziente è "DECEDUTO". False altrimenti.
    ''' </summary>
    Public Function IsDeceduto(codicePaziente As Integer) As Boolean

        Return Me.GenericProvider.Paziente.PazienteDeceduto(codicePaziente)

    End Function

    ''' <summary>
    ''' Restituisce true se esiste lo stesso codice fiscale anche per un altro paziente, oltre a quello specificato.
    ''' Altrimenti, il codice fiscale è unico e restituisce false.
    ''' </summary>
    Public Function IsCodiceFiscaleDuplicato(codicePaziente As Integer, codiceFiscale As String) As Boolean

        Dim altroPazienteStessoCodiceFiscale As Integer = Me.GenericProvider.Paziente.GetAltroPazienteStessoCodFiscale(codicePaziente, codiceFiscale)

        Return (altroPazienteStessoCodiceFiscale <> -1)

    End Function

    ''' <summary>
    ''' Restituisce true se uno dei campi note è valorizzato per il paziente specificato.
    ''' Se il parametro soloCampiShowCnv vale true, controlla solo i campi visualizzabili per le convocazioni, altrimenti anche gli altri campi note.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="soloCampiShowCnv"></param>
    ''' <param name="isGestioneCentrale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function HasNote(codicePaziente As String, soloCampiShowCnv As Boolean, isGestioneCentrale As Boolean) As Boolean

        If isGestioneCentrale Then Return False

        Return GenericProvider.Paziente.CountNote(codicePaziente, ContextInfos.CodiceUsl, soloCampiShowCnv) > 0

    End Function

    Public Function CountNotePaziente(codicePaziente As Long, soloCampiShowCnv As Boolean) As Integer

        Return GenericProvider.Paziente.CountNote(codicePaziente, ContextInfos.CodiceUsl, soloCampiShowCnv)

    End Function

    ''' <summary>
    ''' Restituisce true se il paziente specificato ha ritardi associati
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="isGestioneCentrale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function HasRitardi(codicePaziente As String, isGestioneCentrale As Boolean) As Boolean

        If isGestioneCentrale Then Return False

        Return GenericProvider.Paziente.HasRitardi(codicePaziente)

    End Function

    ''' <summary>
    ''' Restituisce il codice precedente della circoscrizione del paziente e lo aggiorna con quello specificato
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="codiceCircoscrizioneNew"></param>
    ''' <param name="isCircoscrizioneResidenza"></param>
    ''' <param name="recordToLog"></param>
    ''' <remarks></remarks>
    Public Sub UpdateCircoscrizione(codicePaziente As Integer, codiceCircoscrizioneNew As String, isCircoscrizioneResidenza As Boolean, ByRef recordToLog As DataLogStructure.Record)

        Dim codiceCircoscrizioneOld As String = String.Empty

        If isCircoscrizioneResidenza Then

            codiceCircoscrizioneOld = GenericProvider.Paziente.GetCircoscrizioneResidenza(codicePaziente)

            GenericProvider.Paziente.UpdateCircoscrizioneResidenza(codicePaziente, codiceCircoscrizioneNew)

        Else

            codiceCircoscrizioneOld = GenericProvider.Paziente.GetCircoscrizioneDomicilio(codicePaziente)

            GenericProvider.Paziente.UpdateCircoscrizioneDomicilio(codicePaziente, codiceCircoscrizioneNew)

        End If

        If Not recordToLog Is Nothing Then
            recordToLog.Campi.Add(New DataLogStructure.Campo(IIf(isCircoscrizioneResidenza, "PAZ_CIR_CODICE", "PAZ_CIR_CODICE_2"), codiceCircoscrizioneOld, codiceCircoscrizioneNew))
        End If

    End Sub

    ''' <summary>
    ''' Imposta a true il flag Regolarizzato del paziente
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SetFlagRegolarizzaPaziente(codicePaziente As Integer) As Integer

        Dim count As Integer = 0

        Dim ownTransaction As Boolean = False

        Try
            If GenericProvider.Transaction Is Nothing Then
                GenericProvider.BeginTransaction()
                ownTransaction = True
            End If

            count = GenericProvider.Paziente.SetFlagRegolarizzato(codicePaziente, True)

            If ownTransaction Then GenericProvider.Commit()

        Catch ex As Exception

            If ownTransaction Then GenericProvider.Rollback()

            ex.InternalPreserveStackTrace()
            Throw

        End Try

        Return count

    End Function

    ''' <summary>
    ''' Imposta il flag Richiesta Cartella del paziente al valore certificato
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SetFlagRichiestaCertificatoPaziente(codicePaziente As Integer, flagRichiestaCertificato As Boolean) As Integer

        Dim count As Integer = 0

        Dim ownTransaction As Boolean = False

        Try
            If GenericProvider.Transaction Is Nothing Then
                GenericProvider.BeginTransaction()
                ownTransaction = True
            End If

            count = GenericProvider.Paziente.SetFlagRichiestaCertificato(codicePaziente, flagRichiestaCertificato)

            If ownTransaction Then GenericProvider.Commit()

        Catch ex As Exception

            If ownTransaction Then GenericProvider.Rollback()

            ex.InternalPreserveStackTrace()
            Throw

        End Try

        Return count

    End Function

    ''' <summary>
    ''' Concatena alle note del paziente le informazioni specificate per il sollecito.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="numeroSollecitoSeduta"></param>
    ''' <param name="dataAppuntamento"></param>
    ''' <param name="dataConvocazione"></param>
    ''' <param name="codiceCiclo"></param>
    ''' <param name="numeroSeduta"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateNotePazienteSollecito(codicePaziente As Integer, numeroSollecitoSeduta As Integer, dataAppuntamento As DateTime, dataConvocazione As DateTime, codiceCiclo As String, numeroSeduta As Integer) As Integer

        Dim strTMP As String = String.Empty

        Select Case numeroSollecitoSeduta
            Case 0
                strTMP = "App. Avviso: "
            Case 1
                strTMP = "App. 1°Soll: "
            Case 2
                strTMP = "App. 2°Soll: "
            Case 3
                strTMP = "App. 3°Soll: "
        End Select

        Dim note As String = String.Format("[{0}{1:dd/MM/yyyy} CNV: {2:dd/MM/yyyy} CIC: {3} SED: {4}]",
                                           strTMP,
                                           dataAppuntamento,
                                           dataConvocazione,
                                           codiceCiclo,
                                           numeroSeduta.ToString())

        Return GenericProvider.Paziente.UpdateNotePazienteSollecito(codicePaziente, note, ContextInfos.CodiceUsl, ContextInfos.IDUtente)

    End Function

    Public Function UpdateNotePazienteSollecito(codicePaziente As Integer, dataAppuntamento As DateTime, dataConvocazione As DateTime) As Integer

        Dim strTMP As String = "App. Avviso: "
        Dim note As String = String.Format("[{0}{1:dd/MM/yyyy} CNV: {2:dd/MM/yyyy} CIC: no ciclo]",
                                           strTMP,
                                           dataAppuntamento,
                                           dataConvocazione)

        Return GenericProvider.Paziente.UpdateNotePazienteSollecito(codicePaziente, note, ContextInfos.CodiceUsl, ContextInfos.IDUtente)

    End Function

    ''' <summary>
    ''' Restituisce un datatable con i dati delle mantoux relative al paziente specificato
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDtMantouxPaziente(codicePaziente As Long) As DataTable

        Return GenericProvider.Paziente.GetDtMantoux(codicePaziente)

    End Function

    ''' <summary>
    ''' Restituisce il numero di mantoux relative al paziente specificato.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="isGestioneCentrale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountMantouxPaziente(codicePaziente As String, isGestioneCentrale As Boolean) As Integer

        If isGestioneCentrale Then Return 0

        Return GenericProvider.Paziente.CountMantoux(codicePaziente)

    End Function

    ''' <summary>
    ''' Restituisce true se il paziente è totalmente inadempiente per le vaccinazioni obbligatorie.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsTotalmenteInadempiente(codicePaziente As Long) As Boolean

        Return GenericProvider.Paziente.IsTotalmenteInadempiente(codicePaziente)

    End Function

    ''' <summary>
    ''' Aggiorna l'id ACN del paziente
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="idACN"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateIdACNPaziente(codicePaziente As Long, idACN As String) As Integer

        Return GenericProvider.Paziente.UpdateIdACNPaziente(codicePaziente, idACN)

    End Function

    ''' <summary>
    ''' Restituisce il numero di episodi di sorveglianza COVID-19 per il paziente
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    Public Function CountEpisodiCovid(codicePaziente As Long) As Integer

        Return GenericProvider.Paziente.CountEpisodiCovid(codicePaziente)

    End Function

    ''' <summary>
    ''' Restituisce il numero di tamponi COVID-19 per il paziente
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    Public Function CountTamponiCovid(codicePaziente As Long) As Integer

        Return GenericProvider.Paziente.CountTamponiCovid(codicePaziente)

    End Function

    ''' <summary>
    ''' Restituisce il numero di test sierologici COVID-19 per il paziente
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    Public Function CountTestSierologiciCovid(codicePaziente As Long) As Integer

        Return GenericProvider.Paziente.CountTestSierologiciCovid(codicePaziente)

    End Function

    ''' <summary>
    ''' Restituisce il numero di ricoveri COVID-19 per il paziente
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    Public Function CountRicoveriCovid(codicePaziente As Long) As Integer

        Return GenericProvider.Paziente.CountRicoveriCovid(codicePaziente)

    End Function

#Region " Riconduzione Pazienti "

    ''' <summary>
    ''' Restituisce una collection di pazienti che possono essere ricondotti al paziente specificato.
    ''' La ricerca avviene in base ai campi specificati nel parametro RICONDUZIONE_INS_PAZ_CAMPI_RICERCA, e per i soli pazienti
    ''' aventi il codice ausiliario nullo.
    ''' </summary>
    ''' <param name="paziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Function GetPazientiRiconduzione(paziente As Entities.Paziente) As Collection.PazienteCollection

        ' Lettura parametro contenente i campi di ricerca specificati.
        If Me.Settings.RICONDUZIONE_INS_PAZ_CAMPI_RICERCA Is Nothing OrElse
           Me.Settings.RICONDUZIONE_INS_PAZ_CAMPI_RICERCA.Count = 0 Then

            Return Nothing

        End If

        ' Valorizzazione filtri di ricerca in base ai campi specificati nel parametro
        Dim filtriRiconduzione As New Filters.FiltriRiconduzionePazienti()

        For Each nomeCampo As String In Me.Settings.RICONDUZIONE_INS_PAZ_CAMPI_RICERCA

            Select Case nomeCampo.Trim().ToUpper()
                Case "PAZ_COGNOME"
                    filtriRiconduzione.Cognome = paziente.PAZ_COGNOME
                Case "PAZ_NOME"
                    filtriRiconduzione.Nome = paziente.PAZ_NOME
                Case "PAZ_CODICE_FISCALE"
                    filtriRiconduzione.CodiceFiscale = paziente.PAZ_CODICE_FISCALE
                Case "PAZ_TESSERA"
                    filtriRiconduzione.Tessera = paziente.Tessera
                Case "PAZ_DATA_NASCITA"
                    filtriRiconduzione.DataNascita = paziente.Data_Nascita
                Case "PAZ_COM_CODICE_NASCITA"
                    filtriRiconduzione.CodiceComuneNascita = paziente.ComuneNascita_Codice
                Case "PAZ_SESSO"
                    filtriRiconduzione.Sesso = paziente.Sesso
                Case "PAZ_COM_CODICE_RESIDENZA"
                    filtriRiconduzione.CodiceComuneResidenza = paziente.ComuneResidenza_Codice
                Case "PAZ_COM_CODICE_DOMICILIO"
                    filtriRiconduzione.CodiceComuneDomicilio = paziente.ComuneDomicilio_Codice
                Case "PAZ_CIT_CODICE"
                    filtriRiconduzione.CodiceCittadinanza = paziente.Cittadinanza_Codice
            End Select

        Next

        ' Ricerca in base ai campi specificati
        Return Me.GenericProvider.Paziente.GetPazientiRiconduzione(filtriRiconduzione, ContextInfos.CodiceUsl)

    End Function

#End Region

#Region " IBizPaziente "

#Region " InserisciPaziente "

    ''' <summary>
    ''' Inserimento del paziente specificato. Viene effettuato il calcolo dello stato anagrafico, 
    ''' dei consultori (vaccinale e territoriale), del flag di regolarizzazione.
    ''' Vengono associati i cicli e viene inserito il movimento di consultorio.
    ''' </summary>
    ''' <param name="InserisciPazienteCommand"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Function InserisciPaziente(inserisciPazienteCommand As InserisciPazienteCommand) As BizResult

        Dim success As Boolean = True

        Dim bizResult As BizResult = Nothing
        Dim malattieResult As BizGenericResult = Nothing

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions)

            ' Stato Anagrafico
            SetCodiceStatoAnagrafico(inserisciPazienteCommand.Paziente, Nothing)

            ' Il paziente verrà inserito solo se il suo stato anagrafico fa parte degli stati anagrafici 
            ' elencati nel parametro. Se il parametro è nullo, non viene effettuato nessun controllo
            If Not Settings.STATIANAG_INSERT_PAZIENTE Is Nothing AndAlso Settings.STATIANAG_INSERT_PAZIENTE.Count > 0 Then

                If Not Settings.STATIANAG_INSERT_PAZIENTE.Contains(inserisciPazienteCommand.Paziente.StatoAnagrafico) Then

                    success = False
                    bizResult = New BizResult(False,
                                              String.Format("Inserimento paziente non effettuato: lo stato anagrafico del paziente non è tra quelli ammessi per l'inserimento (codice ausiliario: {0} - stato anagrafico: {1})",
                                                            inserisciPazienteCommand.Paziente.CodiceAusiliario,
                                                            inserisciPazienteCommand.Paziente.StatoAnagrafico.ToString()))
                End If

            End If

            If success Then

                SetCodiceCNS(inserisciPazienteCommand.Paziente, Nothing)
                SetCodiceCNSTerritoriale(inserisciPazienteCommand.Paziente, Nothing)

                SetFlagRegolarizzato(inserisciPazienteCommand.Paziente, Nothing)
                SetDefaultValueFlags(inserisciPazienteCommand.Paziente)

                ' Stato Vaccinale: se non specificato, imposto quello di default.
                If Not inserisciPazienteCommand.Paziente.Stato.HasValue Then
                    inserisciPazienteCommand.Paziente.Stato = Constants.CommonConstants.StatoVaccinaleDefault
                End If

                ' Data inserimento: se non specificata, inserisco la data corrente.
                If inserisciPazienteCommand.Paziente.DataInserimento = DateTime.MinValue Then
                    inserisciPazienteCommand.Paziente.DataInserimento = DateTime.Now
                End If


                ' TODO [AVN - AURV]: scrittura TR
                'If RicalcolaResidenza(inserisciPazienteCommand.Paziente) Then

                '   ...

                'End If


                ' Inserimento paziente
                GenericProvider.Paziente.InserisciPaziente(inserisciPazienteCommand.Paziente)

                ' Calcolo cicli standard per il paziente
                Dim codiceCicliStandard As List(Of String) = GetCodiceCicliStandardPaziente(inserisciPazienteCommand.Paziente)

                ' Inserimento cicli del paziente
                If Not codiceCicliStandard Is Nothing Then
                    For Each codiceCiclo As String In codiceCicliStandard
                        GenericProvider.Cicli.InsertCicloPaziente(inserisciPazienteCommand.Paziente.Paz_Codice, codiceCiclo)
                    Next
                End If

                ' Inserimento movimento di consultorio relativo al paziente
                GenericProvider.MovimentiInterniCNS.InserimentoMovimentoPaziente(
                    inserisciPazienteCommand.Paziente.Paz_Codice, String.Empty, inserisciPazienteCommand.Paziente.Paz_Cns_Codice,
                    inserisciPazienteCommand.Paziente.DataInserimento, False, False, False)

                ' Malattie
                malattieResult = GestioneEsenzioniMalattiaPaziente(inserisciPazienteCommand.Paziente)

#Region " Log "
                If Not LogOptions Is Nothing Then

                    Dim testataLog As New DataLogStructure.Testata(LogOptions.CodiceArgomento, DataLogStructure.Operazione.Inserimento, inserisciPazienteCommand.Paziente.Paz_Codice, LogOptions.Automatico)

                    testataLog.Records.Add(PazienteLogManager.GetLogPaziente(inserisciPazienteCommand.Paziente, Nothing))
                    testataLog.Records.Add(CicloPazienteLogManager.GetInsertCicliPazienteLog(codiceCicliStandard))
                    testataLog.Records.Add(MovimentoCNSLogManager.GetInsertMovimentoCNSLog(inserisciPazienteCommand.Paziente, Nothing))

                    WriteLog(testataLog)

                End If
#End Region

            End If

            transactionScope.Complete()

        End Using

        If success Then

            Dim resultMessagesDescriptions As New List(Of String)()

            If Not String.IsNullOrWhiteSpace(malattieResult.Message) Then
                resultMessagesDescriptions.Add(malattieResult.Message)
            End If

            Return New BizResult(True, resultMessagesDescriptions.ToArray(), Nothing)

        End If

        Return bizResult

    End Function

#End Region

#Region " ModificaPaziente "

    ''' <summary>
    ''' Update paziente
    ''' </summary>
    ''' <param name="modificaPazienteCommand"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Function ModificaPaziente(modificaPazienteCommand As ModificaPazienteCommand) As BizResult

        Dim resultMessagesDescriptions As New List(Of String)()
        Dim success As Boolean = True

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions)

            Dim pazienteOriginale As Paziente = GenericProvider.Paziente.GetPazienti(modificaPazienteCommand.Paziente.Paz_Codice, ContextInfos.CodiceUsl)(0)

            Dim checkCicliResult As BizResult = CheckCicli(modificaPazienteCommand.Paziente, pazienteOriginale)

            If checkCicliResult.Success Then

                SetCodiceStatoAnagrafico(modificaPazienteCommand.Paziente, pazienteOriginale)

                SetCodiceCNS(modificaPazienteCommand.Paziente, pazienteOriginale)
                SetCodiceCNSTerritoriale(modificaPazienteCommand.Paziente, pazienteOriginale)

                SetFlagRegolarizzato(modificaPazienteCommand.Paziente, pazienteOriginale)
                SetDefaultValueFlags(modificaPazienteCommand.Paziente)

                ' Data Aggiornamento: se non specificata, inserisco la data corrente.
                If modificaPazienteCommand.Paziente.DataAggiornamento = DateTime.MinValue Then
                    modificaPazienteCommand.Paziente.DataAggiornamento = DateTime.Now
                End If


                ' TODO [AVN - AURV]: scrittura TR
                'If RicalcolaResidenza(modificaPazienteCommand.Paziente) Then

                '    ...

                'End If


                GenericProvider.Paziente.ModificaPaziente(modificaPazienteCommand.Paziente)

                Dim convocazioniOriginali As ICollection(Of Convocazione) = Nothing
                Dim convocazioniModificate As ICollection(Of Convocazione) = Nothing
                Dim convocazioniEliminate As ICollection(Of Convocazione) = Nothing

                ' Se lo stato anagrafico è variato ed è uno di quelli per cui cancellare le convocazioni
                If modificaPazienteCommand.Paziente.StatoAnagrafico <> pazienteOriginale.StatoAnagrafico AndAlso
                   Settings.STATIANAG_CANCCNV.Contains(modificaPazienteCommand.Paziente.StatoAnagrafico) Then

                    If Settings.DELCNVSTATO Then

                        convocazioniEliminate = GenericProvider.Convocazione.GetConvocazioniPaziente(modificaPazienteCommand.Paziente.Paz_Codice.ToString())

                        Using bizConvocazione As New BizConvocazione(GenericProvider, Settings, ContextInfos, LogOptions)

                            Dim command As New BizConvocazione.EliminaConvocazioniSollecitiBilanciCommand()
                            command.CodicePaziente = modificaPazienteCommand.Paziente.Paz_Codice
                            command.DataConvocazione = Nothing
                            command.CancellaBilanciAssociati = True
                            command.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.VariazioneStatoAnagrafico
                            command.NoteEliminazione = "Eliminazione appuntamento per variazione stato anagrafico paziente"
                            command.DataEliminazione = modificaPazienteCommand.Paziente.DataAggiornamento
                            command.WriteLog = False

                            bizConvocazione.EliminaConvocazioniSollecitiBilanci(command)

                        End Using

                    End If

                Else

                    If modificaPazienteCommand.Paziente.Paz_Cns_Codice <> pazienteOriginale.Paz_Cns_Codice Then

                        GenericProvider.MovimentiInterniCNS.InserimentoMovimentoPaziente(
                            modificaPazienteCommand.Paziente.Paz_Codice, pazienteOriginale.Paz_Cns_Codice, modificaPazienteCommand.Paziente.Paz_Cns_Codice,
                            modificaPazienteCommand.Paziente.DataAggiornamento, False, False, False)

                        Dim appuntamenti As Boolean? = Nothing

                        If Not Settings.UPDCNV_DELAPP AndAlso Not Settings.UPDCNV_UPDAPP Then
                            appuntamenti = False
                        End If

                        convocazioniOriginali = GenericProvider.Convocazione.GetConvocazioniPaziente(modificaPazienteCommand.Paziente.Paz_Codice, appuntamenti)

                        convocazioniModificate = ModificaConvocazioni(modificaPazienteCommand.Paziente, convocazioniOriginali)

                    End If

                End If

                Dim malattieResult As BizGenericResult = GestioneEsenzioniMalattiaPaziente(modificaPazienteCommand.Paziente)

                If Not String.IsNullOrWhiteSpace(malattieResult.Message) Then
                    resultMessagesDescriptions.Add(malattieResult.Message)
                End If

#Region " Log "
                If Not LogOptions Is Nothing Then

                    Dim testataLog As New DataLogStructure.Testata(LogOptions.CodiceArgomento, DataLogStructure.Operazione.Modifica, modificaPazienteCommand.Paziente.Paz_Codice, LogOptions.Automatico)

                    testataLog.Records.Add(PazienteLogManager.GetLogPaziente(modificaPazienteCommand.Paziente, pazienteOriginale))
                    testataLog.Records.Add(MovimentoCNSLogManager.GetInsertMovimentoCNSLog(modificaPazienteCommand.Paziente, pazienteOriginale))

                    For Each record As DataLogStructure.Record In ConvocazioneLogManager.GetDeleteConvocazioniLog(modificaPazienteCommand.Paziente, convocazioniEliminate)
                        testataLog.Records.Add(record)
                    Next

                    For Each record As DataLogStructure.Record In ConvocazioneLogManager.GetUpdateConvocazioniLog(modificaPazienteCommand.Paziente, convocazioniModificate, convocazioniOriginali)
                        testataLog.Records.Add(record)
                    Next

                    WriteLog(testataLog)

                End If
#End Region

            Else

                For Each resultMessage As BizResult.ResultMessage In checkCicliResult.Messages
                    resultMessagesDescriptions.Add(resultMessage.Description)
                Next

                success = False

            End If

            transactionScope.Complete()

        End Using

        Return New BizResult(success, resultMessagesDescriptions, Nothing)

    End Function

#End Region

#Region " UnisciPazienti "

    ''' <summary>
    ''' Controlla i codici regionali di Master e Alias per determinare se procedere con il merge.
    ''' Se il parametro ALIAS_CONTROLLO_CODICI_REGIONALI è false, non effettua nessun controllo e restituisce true.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckCodiciRegionaliMasterAlias(codicePazienteMaster As Int64, arrayCodiciPazientiAlias As Int64()) As CheckCodiciRegionaliMasterAliasResult

        If Not Me.Settings.ALIAS_CONTROLLO_CODICI_REGIONALI Then Return New CheckCodiciRegionaliMasterAliasResult(CheckCodiciRegionaliMasterAliasResult.ResultType.Success)

        ' Controllo codici regionali
        Dim codiceRegionaleMaster As String = Me.GetCodiceRegionale(codicePazienteMaster)
        Dim codiceRegionaleAlias As String = String.Empty

        For Each codicePazienteAlias As Long In arrayCodiciPazientiAlias
            codiceRegionaleAlias = Me.GetCodiceRegionale(codicePazienteAlias)
            If Not String.IsNullOrEmpty(codiceRegionaleAlias) Then Exit For
        Next

        If Not String.IsNullOrEmpty(codiceRegionaleMaster) AndAlso Not String.IsNullOrEmpty(codiceRegionaleAlias) Then

            ' Se il Master e almeno 1 Alias hanno codice regionale valorizzato => NO MERGE
            Return New CheckCodiciRegionaliMasterAliasResult(CheckCodiciRegionaliMasterAliasResult.ResultType.Failure,
                                                             "Impossibile eseguire il merge: Master e Alias hanno codice regionale valorizzato.")

        ElseIf String.IsNullOrEmpty(codiceRegionaleMaster) AndAlso Not String.IsNullOrEmpty(codiceRegionaleAlias) Then

            ' Se il Master non ha codice regionale e almeno 1 Alias ce l'ha => CONFERMA ALL'UTENTE
            Return New CheckCodiciRegionaliMasterAliasResult(CheckCodiciRegionaliMasterAliasResult.ResultType.Warning,
                                                             "Solo il paziente Alias ha codice regionale valorizzato.")

        End If

        Return New CheckCodiciRegionaliMasterAliasResult(CheckCodiciRegionaliMasterAliasResult.ResultType.Success)

    End Function

    ''' <summary>
    ''' Merge dei due pazienti specificati.
    ''' </summary>
    ''' <param name="codicePazienteMaster">Codice del paziente master in anagrafe locale</param>
    ''' <param name="codicePazienteAlias">Codice del paziente alias in anagrafe locale</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Obsolete()>
    Public Overridable Function UnisciPazienti(codicePazienteMaster As Integer, codicePazienteAlias As Integer) As BizResult

        ' --------- Caricamento Pazienti da DB --------- '

        ' Caricamento Master
        Dim pazienteMaster As Entities.Paziente = Me.CercaPaziente(codicePazienteMaster)

        If pazienteMaster Is Nothing Then

            Dim msg As String = String.Format("Il paziente indicato come Master (codice: {0}) non è presente in anagrafe.", codicePazienteMaster.ToString())

            Return New BizResult(False, msg)

        End If

        ' Caricamento Alias
        Dim pazienteAlias As Entities.Paziente = Me.CercaPaziente(codicePazienteAlias)

        If pazienteAlias Is Nothing Then

            Return New BizResult(False, String.Format("Il paziente indicato come Alias (codice: {0}) non è presente in anagrafe.", codicePazienteAlias.ToString()))

        End If

        Dim unisciPazientiCommand As New UnisciPazientiCommand()
        unisciPazientiCommand.PazienteMaster = pazienteMaster
        unisciPazientiCommand.PazienteAlias = pazienteAlias

        Return Me.UnisciPazienti(unisciPazientiCommand)

    End Function

    Private Function GetUnsuccessfulBizResult(message As String, messaggioDatiMasterAlias As String) As BizResult

        ' Se il master e l'alias hanno entrambi vaccinazioni escluse, il merge non può essere eseguito
        Dim resultMessageList As New List(Of BizResult.ResultMessage)()

        Dim resultMessage As New BizResult.ResultMessage(Me.GetMessageUnisciPaziente(message, messaggioDatiMasterAlias), False)

        resultMessageList.Add(resultMessage)

        Return New BizResult(False, resultMessageList)

    End Function

    ''' <summary>
    ''' Merge dei due pazienti specificati.
    ''' </summary>
    ''' <param name="unisciPazientiCommand">Paziente master e alias in anagrafe locale</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Function UnisciPazienti(unisciPazientiCommand As UnisciPazientiCommand) As BizResult

        Dim bizResult As BizResult = Nothing

        Dim messaggioDatiMasterAlias As String = Nothing

        Dim success As Boolean = True

        ' --------- Esecuzione Merge --------- '

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions())

            Dim idUtente As Integer

            If Not ContextInfos Is Nothing Then
                idUtente = ContextInfos.IDUtente
            Else
                idUtente = 0
            End If

            messaggioDatiMasterAlias = GetMessaggioDatiMasterAlias(unisciPazientiCommand.PazienteMaster, unisciPazientiCommand.PazienteAlias)

            Dim now As DateTime = DateTime.Now

            ' --------- Controllo dati vaccinali --------- '
            ' Controllo che siano unificabili, ovvero: 
            ' 1 - eseguite e scadute solo Master o solo Alias
            ' 2 - esclusioni solo master oppure solo Alias
            ' 3 - visite solo master oppure solo Alias

            ' Controllo vaccinazioni eseguite e scadute
            Dim countVacEseguiteMaster As Integer = GenericProvider.Paziente.CountVaccinazioniEseguite(unisciPazientiCommand.PazienteMaster.Paz_Codice)
            Dim countVacEseguiteAlias As Integer = GenericProvider.Paziente.CountVaccinazioniEseguite(unisciPazientiCommand.PazienteAlias.Paz_Codice)
            Dim countVacScaduteAlias As Integer = GenericProvider.Paziente.CountVaccinazioniScadute(unisciPazientiCommand.PazienteAlias.Paz_Codice)

            If countVacEseguiteAlias + countVacScaduteAlias > 0 Then

                Dim countVacScaduteMaster As Integer = GenericProvider.Paziente.CountVaccinazioniScadute(unisciPazientiCommand.PazienteMaster.Paz_Codice)

                If countVacEseguiteMaster + countVacScaduteMaster > 0 Then

                    ' Se eseguite e scadute sono presenti sia nel master che nell'alias, il merge non può essere eseguito
                    bizResult = GetUnsuccessfulBizResult("Il master e l'alias hanno entrambi vaccinazioni eseguite e scadute.", messaggioDatiMasterAlias)
                    success = False

                End If

            End If

            ' Controllo vaccinazioni escluse
            If success Then

                Dim countVacEscluseAlias As Integer = GenericProvider.Paziente.CountVaccinazioniEscluse(unisciPazientiCommand.PazienteAlias.Paz_Codice)

                If countVacEscluseAlias > 0 Then

                    Dim countVacEscluseMaster As Integer = GenericProvider.Paziente.CountVaccinazioniEscluse(unisciPazientiCommand.PazienteMaster.Paz_Codice)

                    If countVacEscluseMaster > 0 Then

                        ' Se il master e l'alias hanno entrambi vaccinazioni escluse, il merge non può essere eseguito
                        bizResult = GetUnsuccessfulBizResult("Il master e l'alias hanno entrambi vaccinazioni escluse.", messaggioDatiMasterAlias)
                        success = False

                    End If

                End If

            End If

            ' Controllo visite con sospensione
            If success Then

                Dim countSospensioniAlias As Integer = GenericProvider.Paziente.CountSospensioni(unisciPazientiCommand.PazienteAlias.Paz_Codice)

                If countSospensioniAlias > 0 Then

                    Dim countSospensioniMaster As Integer = GenericProvider.Paziente.CountSospensioni(unisciPazientiCommand.PazienteMaster.Paz_Codice)

                    If countSospensioniMaster > 0 Then

                        ' Se il master e l'alias hanno entrambi sospensioni, il merge non può essere eseguito
                        bizResult = GetUnsuccessfulBizResult("Il master e l'alias hanno entrambi visite con sospensione.", messaggioDatiMasterAlias)
                        success = False

                    End If

                End If

            End If

            ' Controllo bilanci (visite + osservazioni)
            If success Then

                Dim countOsservazioniAlias As Integer = GenericProvider.Paziente.CountOsservazioni(unisciPazientiCommand.PazienteAlias.Paz_Codice)

                If countOsservazioniAlias > 0 Then

                    Dim countOsservazioniMaster As Integer = GenericProvider.Paziente.CountOsservazioni(unisciPazientiCommand.PazienteMaster.Paz_Codice)

                    If countOsservazioniMaster > 0 Then

                        ' Se il master e l'alias hanno entrambi bilanci, il merge non può essere eseguito
                        bizResult = GetUnsuccessfulBizResult("Il master e l'alias hanno entrambi bilanci.", messaggioDatiMasterAlias)
                        success = False

                    End If

                End If

            End If

            If success Then

                ' --------- Scelta dati vaccinali da cancellare --------- '
                ' La scelta di quali dati mantenere viene effettuata in base a chi dei due pazienti ha le eseguite.
                ' Se la programmazione vaccinale del master verrà cancellata, e verrà utilizzata quella dell'alias,
                ' devo aggiornare il consultorio del master impostando quello dell'alias.
                ' N.B. : se UsaAliasComeMasterVaccinale è true, viene forzata la scelta dell'alias.
                '        se UsaCNSMasterAnagrafico è true, viene mantenuto il consultorio del master.
                Dim codicePazienteVaccDaEliminare As Integer
                Dim updateCnsMaster As Boolean = False

                If Not unisciPazientiCommand.UsaAliasComeMasterVaccinale AndAlso countVacEseguiteAlias = 0 Then

                    ' L'alias non ha vaccinazioni eseguite: dovrò cancellare i dati vaccinali dell'alias.
                    codicePazienteVaccDaEliminare = unisciPazientiCommand.PazienteAlias.Paz_Codice

                    ' Il consultorio del master non deve essere sovrascritto da quello dell'alias
                    updateCnsMaster = False

                Else

                    ' L'alias ha vac eseguite: per il controllo effettuato in precedenza, il master non può avere eseguite.
                    ' Quindi cancello i dati vaccinali del master.
                    codicePazienteVaccDaEliminare = unisciPazientiCommand.PazienteMaster.Paz_Codice

                    ' Il consultorio dell'alias andrà impostato nel master
                    updateCnsMaster = True

                End If

                If Settings.ALIAS_USA_CNS_MASTER_ANAGRAFICO Then

                    ' Il consultorio del master non deve essere mai sovrascritto (nel caso in cui il master vaccinale sia l'alias)
                    updateCnsMaster = False

                End If

                ' --- Eliminazione dati vaccinali del paziente senza eseguite --- '
                ' Tutti i dati che non devono essere riportati sul master vengono eliminati (i dati da non riportare sono quelli
                ' del paziente senza le eseguite, determinato in precedenza)

                ' Cancellazione convocazioni, cicli, programmate e bilanci

                ' N.B. : NO gestione storico in UnisciPazienti => configurazione per spostare storico da alias a master
                GenericProvider.Convocazione.Delete(codicePazienteVaccDaEliminare, True)

                ' Cancellazione ritardi
                GenericProvider.Paziente.EliminaRitardiPaziente(codicePazienteVaccDaEliminare)

                ' Cancellazione cicli paziente
                GenericProvider.Paziente.EliminaCicliPaziente(codicePazienteVaccDaEliminare)

                ' --- Update campo codice paziente da alias a master --- '
                ' Per le tabelle dove è previsto, viene fatto l'update diretto del campo paz_codice.
                ' Viene aggiornato il paz_codice_old con l'alias e il paz_codice con il master
                Dim listMergeUpdateInfo As List(Of MergeUpdateInfo) = GenericProvider.AliasPazienti.LoadMergeUpdateInfo()

                If Not listMergeUpdateInfo.IsNullOrEmpty() Then

                    For Each info As MergeUpdateInfo In listMergeUpdateInfo
                        GenericProvider.AliasPazienti.UpdateTabellaMerge(info, unisciPazientiCommand.PazienteMaster.Paz_Codice, unisciPazientiCommand.PazienteAlias.Paz_Codice)
                    Next

                End If

                ' --- Associazione dei dati dell'alias al master --- '
                ' I dati vaccinali dell'alias vengono associati al master (in base a quanto impostato nella t_ana_colonne_alias).
                ' L'associazione avviene inserendo i dati dell'alias nel master, se non sono già presenti (per evitare 
                ' errori oracle di chiavi duplicate), con il codice dell'alias nella colonna "paz_codice_old" di
                ' ogni tabella (per poterli recuperare nel caso in cui si debba effettuare l'operazione di unmerge).

                ' Carico i dati delle tabelle da allineare
                Dim listColonneAlias As List(Of ColonneAlias) = GenericProvider.AliasPazienti.LoadColonneAlias()

                ' Inserimento dati vaccinali dell'alias associandoli al master nelle tabelle impostate nella t_ana_colonne_alias
                For i As Integer = 0 To listColonneAlias.Count - 1
                    GenericProvider.AliasPazienti.InsertDatiMergeAlias(listColonneAlias(i), unisciPazientiCommand.PazienteMaster.Paz_Codice, unisciPazientiCommand.PazienteAlias.Paz_Codice)
                Next

                ' Eliminazione dati vaccinali dell'alias dalle tabelle impostate nella t_ana_colonne_alias
                For i As Integer = listColonneAlias.Count - 1 To 0 Step -1
                    GenericProvider.AliasPazienti.DeleteDatiMergeAlias(listColonneAlias(i).Tabella, listColonneAlias(i).CampoCodicePaziente, unisciPazientiCommand.PazienteAlias.Paz_Codice)
                Next

                ' Inserimento dati nelle tabelle di tipo padre-figlia con chiave esterna autogenerata
                ' N.B. : per le visite, nella logica dell'applicativo, il controllo che la visita esista già è gestito dal parametro
                '        VISITE_STESSA_DATA. In questo caso, non potendolo utilizzare, i campi per cui effettuare il controllo
                '        devono essere diversi (nelle tabelle di configurazione) a seconda del parametro:
                '        se VISITE_STESSA_DATA = "S" -> nella T_ANA_LINK_TABELLE_MERGE, il campo TLM_CAMPI_INDICE_PADRE 
                '                                       deve contenere "VIS_DATA_VISITA, VIS_MAL_CODICE"
                '        se VISITE_STESSA_DATA = "N" -> tale campo deve valere "VIS_DATA_VISITA".

                ' Lettura informazioni per relative alle tabelle da valorizzare
                Dim listMergeInfo As List(Of MergeInfo) = GenericProvider.AliasPazienti.LoadMergeInfo()

                If Not listMergeInfo Is Nothing AndAlso listMergeInfo.Count > 0 Then

                    ' Ciclo sulle tabelle da valorizzare
                    For Each mergeInfo As MergeInfo In listMergeInfo

                        ' Selezione valori da inserire (da Alias a Master)
                        Dim dtValoriMerge As DataTable = GenericProvider.AliasPazienti.GetDtValoriAlias(mergeInfo, unisciPazientiCommand.PazienteMaster.Paz_Codice, unisciPazientiCommand.PazienteAlias.Paz_Codice, False)

                        ' Ciclo per ogni elemento
                        If Not dtValoriMerge Is Nothing AndAlso dtValoriMerge.Rows.Count > 0 Then

                            For Each row As DataRow In dtValoriMerge.Rows

                                ' Vecchia sequence di riferimento
                                Dim oldSequenceId As Integer = row(mergeInfo.CampoFkPadre)

                                ' Nuova sequence che verrà inserita
                                Dim newSequenceId As Integer = Me.GenericProvider.AliasPazienti.GetNextSequenceValue(mergeInfo.NomeSequence)

                                ' Inserimento dati dell'alias nella tabella padre
                                GenericProvider.AliasPazienti.InsertValoreAliasPadre(mergeInfo, unisciPazientiCommand.PazienteMaster.Paz_Codice, unisciPazientiCommand.PazienteAlias.Paz_Codice, newSequenceId, row, False)

                                ' Inserimento dati dell'alias nella tabella figlia
                                GenericProvider.AliasPazienti.InsertValoriAliasFiglia(mergeInfo, unisciPazientiCommand.PazienteMaster.Paz_Codice, unisciPazientiCommand.PazienteAlias.Paz_Codice, oldSequenceId, newSequenceId, False)

                            Next

                        End If

                        ' Eliminazione dati nella tabella figlia
                        GenericProvider.AliasPazienti.DeleteDatiMergeAlias(mergeInfo.NomeTabellaFiglia, mergeInfo.CampoCodicePazienteFiglia, unisciPazientiCommand.PazienteAlias.Paz_Codice)

                        ' Eliminazione dati nella tabella padre 
                        GenericProvider.AliasPazienti.DeleteDatiMergeAlias(mergeInfo.NomeTabellaPadre, mergeInfo.CampoCodicePazientePadre, unisciPazientiCommand.PazienteAlias.Paz_Codice)

                    Next

                End If

                ' Copia dell'alias nella t_tmp_pazienti_alias, prima di eliminarlo dalla t_paz_pazienti
                GenericProvider.AliasPazienti.CloneAlias(unisciPazientiCommand.PazienteMaster.Paz_Codice, unisciPazientiCommand.PazienteAlias.Paz_Codice, now, idUtente)

                ' Flag che indica se ci sono dati del master da modificare
                Dim updateMaster As Boolean = False

                ' Se ho cancellato la programmazione vaccinale del master e utilizzato quella dell'alias,
                ' imposto il consultorio del master uguale a quello dell'alias.
                If updateCnsMaster Then

                    ' Se ci sono i dati dell'alias e se è impostato il consultorio dell'alias, eseguo l'update del cns
                    If (Not String.IsNullOrEmpty(unisciPazientiCommand.PazienteAlias.Paz_Cns_Codice)) Then

                        Dim codCnsMaster As String = GenericProvider.Paziente.GetCodiceConsultorio(unisciPazientiCommand.PazienteMaster.Paz_Codice)
                        Dim codCnsAlias As String = unisciPazientiCommand.PazienteAlias.Paz_Cns_Codice

                        If codCnsMaster <> codCnsAlias Then

                            unisciPazientiCommand.PazienteMaster.Paz_Cns_Codice_Old = unisciPazientiCommand.PazienteMaster.Paz_Cns_Codice
                            unisciPazientiCommand.PazienteMaster.Paz_Cns_Codice = codCnsAlias
                            unisciPazientiCommand.PazienteMaster.Paz_Cns_Data_Assegnazione = now

                            ' Il master è stato modificato, quindi bisognerà fare l'update su db
                            updateMaster = True

                        End If

                    End If

                End If

                ' Assegna lo stato acquisizione TOTALE al Master (se nullo)
                If unisciPazientiCommand.SetStatoAcquisizioneTotaleIfNull AndAlso
                   Not unisciPazientiCommand.PazienteMaster.StatoAcquisizioneDatiVaccinaliCentrale.HasValue Then

                    unisciPazientiCommand.PazienteMaster.StatoAcquisizioneDatiVaccinaliCentrale = Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Totale
                    updateMaster = True

                End If

                ' Eliminazione alias dall'anagrafica dei pazienti
                GenericProvider.AliasPazienti.DeleteAliasFromPazienti(unisciPazientiCommand.PazienteAlias.Paz_Codice)

                ' TODO [BizPaziente]: UnisciPaziente => LOG eliminazione alias da anagrafe !!!

                ' Integrazione dati nulli del master con quelli dell'alias
                If Settings.ALIAS_UPDATE_MASTER_NULL Then

                    Dim masterValue, aliasValue As Object

                    Dim properties() As Reflection.PropertyInfo = GetType(Entities.Paziente).GetProperties(Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)

                    For i As Integer = 0 To properties.Length - 1

                        ' Se il campo corrente è elencato nel parametro dei campi da escludere, passo direttamente al campo successivo
                        If Me.Settings.ALIAS_UPDATE_MASTER_NULL_CAMPI_ESCLUSI.Contains(properties(i).Name.ToUpper()) Then Continue For

                        masterValue = properties(i).GetValue(unisciPazientiCommand.PazienteMaster, Nothing)
                        aliasValue = properties(i).GetValue(unisciPazientiCommand.PazienteAlias, Nothing)

                        If properties(i).PropertyType Is GetType(String) Then

                            If (masterValue Is Nothing OrElse String.IsNullOrEmpty(masterValue.ToString())) AndAlso
                               (Not aliasValue Is Nothing AndAlso Not String.IsNullOrEmpty(aliasValue.ToString())) Then

                                properties(i).SetValue(unisciPazientiCommand.PazienteMaster, aliasValue, Nothing)
                                updateMaster = True

                            End If

                        ElseIf properties(i).PropertyType Is GetType(DateTime) Then

                            If (masterValue Is Nothing OrElse masterValue = DateTime.MinValue) AndAlso
                               (Not aliasValue Is Nothing AndAlso aliasValue > DateTime.MinValue) Then

                                properties(i).SetValue(unisciPazientiCommand.PazienteMaster, aliasValue, Nothing)
                                updateMaster = True

                            End If

                        ElseIf properties(i).PropertyType.IsGenericType AndAlso
                               properties(i).PropertyType.GetGenericTypeDefinition() Is GetType(Nullable(Of )) Then

                            If (masterValue = Nothing AndAlso Not aliasValue = Nothing) Then

                                properties(i).SetValue(unisciPazientiCommand.PazienteMaster, aliasValue, Nothing)
                                updateMaster = True

                            End If

                            ' le properties di altro tipo non vengono considerate

                        End If

                    Next

                End If

                ' Se sono stati modificati i dati del master, update su db
                If updateMaster Then

                    ' Inserimento movimento (se consultorio modificato)
                    If updateCnsMaster Then

                        ' TODO [BizPaziente]: UnisciPaziente => LOG
                        GenericProvider.MovimentiInterniCNS.InserimentoMovimentoPaziente(unisciPazientiCommand.PazienteMaster.Paz_Codice,
                                                                                         unisciPazientiCommand.PazienteMaster.Paz_Cns_Codice_Old,
                                                                                         unisciPazientiCommand.PazienteMaster.Paz_Cns_Codice,
                                                                                         now, False, False, False)

                    End If

                    ' Modifica dei dati del paziente
                    GenericProvider.Paziente.ModificaPaziente(unisciPazientiCommand.PazienteMaster)

                    ' TODO [BizPaziente]: UnisciPaziente => LOG modifica master (pazienteMaster, pazienteMasterOriginal) !!!

                End If

                If Not LogOptions Is Nothing Then

                    Dim testataLog As New DataLogStructure.Testata(LogOptions.CodiceArgomento, DataLogStructure.Operazione.Modifica, LogOptions.Automatico)

                    ' TODO [BizPaziente]: UnisciPazienti =>  Log !!!
                    ' testataLog.Records.Add(PazienteLogManager.WriteInsertUpdatePazienteLog(logTestata, Paziente, pazienteOriginale))
                    ' testataLog.Records.Add(MovimentoCNSLogManager.WriteInsertMovimentoCNSLog(logTestata, Paziente, pazienteOriginale))
                    ' testataLog.Records.Add(ConvocazioneLogManager.WriteDeleteConvocazioniLog(logTestata, Paziente, convocazioniEliminate))
                    ' testataLog.Records.Add(ConvocazioneLogManager.WriteUpdateConvocazioniLog(logTestata, Paziente, convocazioniModificate, convocazioniOriginali))

                    Me.WriteLog(testataLog)

                End If

            End If

            transactionScope.Complete()

        End Using

        If success Then bizResult = New BizResult(True, messaggioDatiMasterAlias)

        Return bizResult

    End Function

#Region " Merge Massivo "

    Public Class BatchMergePazienti

        Public Class ParameterName
            Public Const IdApplicazioneLocale As String = "IdApplicazioneLocale"
            Public Const IdUtenteUnificazione As String = "IdUtenteUnificazione"
            Public Const NumeroElaborazioniRefreshRisultatoParziale As String = "NumeroElaborazioniRefreshRisultatoParziale"
            Public Const SetStatoAcquisizioneTotaleIfNull As String = "SetStatoAcquisizioneTotaleIfNull"
            Public Const UsaAliasComeMasterVaccinale As String = "UsaAliasComeMasterVaccinale"
            Public Const CodiceUslCorrente As String = "CodiceUslCorrente"
        End Class

        Public Class MergeCommand
            Public Property ProgressivoElaborazione As Long
            Public Property UsaAliasComeMasterVaccinale As Boolean
            Public Property SetStatoAcquisizioneTotaleIfNull As Boolean
        End Class

        Public Class MergeResult
            Inherits BizGenericResult

            Public ErrorCode As Integer?
            Public StatoElaborazione As String

        End Class

    End Class

    ''' <summary>
    ''' Restituisce la lista con le coppie master-alias da unificare.
    ''' </summary>
    ''' <param name="dataUltimaElaborazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetProgressiviPazientiDaUnificare(idJob As Long, codiceProcedura As Integer, dataUltimaElaborazione As DateTime?) As List(Of Long)

        ' Lettura codici da t_paz_merge
        Dim listCodici As List(Of KeyValuePair(Of Integer, Integer)) = Me.GenericProvider.AliasPazienti.GetCodiciPazientiDaUnificare(dataUltimaElaborazione)

        If listCodici.IsNullOrEmpty() Then
            Return Nothing
        End If

        Dim listProgressivi As New List(Of Long)

        Dim dataRichiesta As DateTime = DateTime.Now

        ' Inserimento nella t_paz_elaborazioni
        For Each masterAlias As KeyValuePair(Of Integer, Integer) In listCodici

            Dim pazienteElaborazione As New Entities.PazienteElaborazione()

            pazienteElaborazione.CodicePaziente = masterAlias.Key
            pazienteElaborazione.CodicePazienteAlias = masterAlias.Value
            pazienteElaborazione.IdJob = idJob
            pazienteElaborazione.CodiceProcedura = codiceProcedura
            pazienteElaborazione.DataRichiesta = dataRichiesta
            pazienteElaborazione.IdUtenteRichiesta = Me.ContextInfos.IDUtente
            pazienteElaborazione.Stato = Constants.StatoElaborazioneBatch.DaElaborare

            Me.GenericProvider.Procedure.InsertPazientiElaborazioni(pazienteElaborazione)

            listProgressivi.Add(pazienteElaborazione.Progressivo)

        Next

        Return listProgressivi

    End Function

    Public Function MergePazienti(command As BatchMergePazienti.MergeCommand) As BatchMergePazienti.MergeResult

        Dim pazienteElaborazione As Entities.PazienteElaborazione = Me.GenericProvider.Procedure.GetPazienteElaborazione(command.ProgressivoElaborazione)

        ' Ricerca Master e Alias
        Dim pazienteMaster As Entities.Paziente = CercaPaziente(pazienteElaborazione.CodicePaziente)
        Dim pazienteAlias As Entities.Paziente = CercaPaziente(pazienteElaborazione.CodicePazienteAlias)

        Dim result As BatchMergePazienti.MergeResult = CheckCanMerge(pazienteMaster, pazienteAlias)

        If result.Success Then

            ' Merge
            Dim unisciPazientiCommand As New UnisciPazientiCommand()
            unisciPazientiCommand.PazienteMaster = pazienteMaster
            unisciPazientiCommand.PazienteAlias = pazienteAlias
            unisciPazientiCommand.SetStatoAcquisizioneTotaleIfNull = command.SetStatoAcquisizioneTotaleIfNull
            unisciPazientiCommand.UsaAliasComeMasterVaccinale = command.UsaAliasComeMasterVaccinale

            Dim unisciPazientiResult As BizResult = UnisciPazienti(unisciPazientiCommand)

            result.Success = unisciPazientiResult.Success
            result.Message = unisciPazientiResult.Messages.ToString()

            If result.Success Then
                result.ErrorCode = Nothing
                result.StatoElaborazione = Constants.StatoElaborazioneBatch.TerminataCorrettamente
            Else
                result.ErrorCode = 50
                result.StatoElaborazione = Constants.StatoElaborazioneBatch.Errore
            End If

        End If

        ' Aggiornamento t_paz_elaborazioni
        Me.GenericProvider.Procedure.UpdatePazienteElaborazione(command.ProgressivoElaborazione, result.ErrorCode, result.Message, DateTime.Now, result.StatoElaborazione)

        Return result

    End Function

    Private Function CheckCanMerge(pazienteMaster As Entities.Paziente, pazienteAlias As Entities.Paziente) As BatchMergePazienti.MergeResult

        Dim result As New BatchMergePazienti.MergeResult()
        result.ErrorCode = Nothing
        result.StatoElaborazione = Constants.StatoElaborazioneBatch.DaElaborare

        If pazienteMaster Is Nothing Then

            result.Success = False
            result.Message = "Unificazione non effettuata. Il paziente master non è presente in locale."
            result.ErrorCode = 51
            result.StatoElaborazione = Constants.StatoElaborazioneBatch.Errore
            Return result

        End If

        ' Controllo master cancellato
        If pazienteMaster.FlagCancellato = "S" Then

            result.Success = False
            result.Message = "Unificazione non effettuata. Il paziente master risulta cancellato."
            result.ErrorCode = 52
            result.StatoElaborazione = Constants.StatoElaborazioneBatch.Errore
            Return result

        End If

        If pazienteAlias Is Nothing Then

            result.Success = False
            result.Message = "Unificazione non effettuata. Il paziente alias non è presente in locale."
            result.ErrorCode = 53
            result.StatoElaborazione = Constants.StatoElaborazioneBatch.Errore
            Return result

        End If

        ' Controllo della presenza del codice ausiliario se l'alias non è cancellato
        If Me.Settings.CHECK_DATI_ALIAS_PER_MERGE AndAlso
           pazienteAlias.FlagCancellato = "N" AndAlso Not String.IsNullOrWhiteSpace(pazienteAlias.CodiceAusiliario) Then

            result.Success = False
            result.Message = "Unificazione non effettuata. Il paziente alias ha un codice centrale associato."
            result.ErrorCode = 54
            result.StatoElaborazione = Constants.StatoElaborazioneBatch.Errore
            Return result

        End If

        ' Controllo codici regionali
        If Me.Settings.ALIAS_CONTROLLO_CODICI_REGIONALI AndAlso
           Not String.IsNullOrWhiteSpace(pazienteMaster.PAZ_CODICE_REGIONALE) AndAlso Not String.IsNullOrWhiteSpace(pazienteAlias.PAZ_CODICE_REGIONALE) Then

            result.Success = False
            result.Message = "Unificazione non effettuata. Entrambi i pazienti hanno il codice regionale valorizzato."
            result.ErrorCode = 55
            result.StatoElaborazione = Constants.StatoElaborazioneBatch.Errore
            Return result

        End If

        Return result

    End Function

#End Region

#End Region

#Region " DisunisciPazienti "

    ''' <summary>
    ''' Unmerge dei due pazienti con codice specificato
    ''' </summary>
    ''' <param name="codicePazienteMaster"></param>
    ''' <param name="codicePazienteAlias"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Function DisunisciPazienti(codicePazienteMaster As Integer, codicePazienteAlias As Integer) As BizResult

        ' Controllo presenza Master
        Dim pazienteMaster As Entities.Paziente = Me.CercaPaziente(codicePazienteMaster)

        If pazienteMaster Is Nothing Then

            Return New BizResult(False, New String() {String.Format("Il paziente master specificato (codice: {0}) non è presente in anagrafe.", codicePazienteMaster)}, Nothing)

        End If

        ' Caricamento Alias dalla t_tmp_pazienti_alias in base al codice specificato
        Dim filtriRicercaAlias As New Onit.OnAssistnet.OnVac.Filters.FiltriRicercaPaziente()
        filtriRicercaAlias.Codice = codicePazienteAlias

        Dim listAlias As List(Of Entities.PazienteAlias) = Me.GenericProvider.AliasPazienti.LoadAlias(filtriRicercaAlias, Nothing)
        If listAlias Is Nothing OrElse listAlias.Count = 0 Then

            Return New BizResult(False, New String() {String.Format("Il paziente specificato (codice: {0}) non è presente nella tabella degli alias.", codicePazienteAlias)}, Nothing)

        End If

        Dim pazienteAlias As Entities.Paziente = listAlias(0)

        Dim disunisciPazientiCommand As New DisunisciPazientiCommand()
        disunisciPazientiCommand.PazienteMaster = pazienteMaster
        disunisciPazientiCommand.PazienteAlias = pazienteAlias

        Return Me.DisunisciPazienti(disunisciPazientiCommand)

    End Function

    ''' <summary>
    ''' Unmerge dei due pazienti
    ''' </summary>
    ''' <param name="disunisciPazientiCommand"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Function DisunisciPazienti(disunisciPazientiCommand As DisunisciPazientiCommand) As BizResult

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions)

            ' Inserimento alias nella t_paz_pazienti
            Me.GenericProvider.Paziente.InserisciPaziente(disunisciPazientiCommand.PazienteAlias)

            ' Cancellazione dalla t_tmp_pazienti_alias
            Me.GenericProvider.AliasPazienti.DeleteAliasFromTemp(disunisciPazientiCommand.PazienteAlias.Paz_Codice)

            ' Caricamento configurazione della t_ana_colonne_alias
            Dim listColonneAlias As List(Of Entities.ColonneAlias) = Me.GenericProvider.AliasPazienti.LoadColonneAlias()

            ' Inserimento dati vaccinali alias dissociandoli dal master, in base alla t_ana_colonne_alias
            For i As Integer = 0 To listColonneAlias.Count - 1
                Me.GenericProvider.AliasPazienti.InsertDatiUnmergeAlias(listColonneAlias(i), disunisciPazientiCommand.PazienteMaster.Paz_Codice, disunisciPazientiCommand.PazienteAlias.Paz_Codice)
            Next

            ' Eliminazione dati vaccinali master associati all'alias, in base alla t_ana_colonne_alias
            For i As Integer = listColonneAlias.Count - 1 To 0 Step -1
                Me.GenericProvider.AliasPazienti.DeleteDatiUnmergeAlias(listColonneAlias(i).Tabella, listColonneAlias(i).CampoCodicePaziente, listColonneAlias(i).CampoCodicePazienteOld, disunisciPazientiCommand.PazienteMaster.Paz_Codice, disunisciPazientiCommand.PazienteAlias.Paz_Codice)
            Next

            ' Lettura informazioni per relative alle tabelle da valorizzare
            Dim listMergeInfo As List(Of Entities.MergeInfo) = Me.GenericProvider.AliasPazienti.LoadMergeInfo()

            If Not listMergeInfo Is Nothing AndAlso listMergeInfo.Count > 0 Then

                ' Inserimento dati alias dissociandoli dal master, nelle tabelle di tipo padre-figlia con chiave esterna autogenerata

                ' Ciclo sulle tabelle da valorizzare
                For Each mergeInfo As Entities.MergeInfo In listMergeInfo

                    ' Selezione valori da inserire (da Master ad Alias)
                    Dim dtValoriMerge As DataTable = Me.GenericProvider.AliasPazienti.GetDtValoriAlias(mergeInfo, disunisciPazientiCommand.PazienteMaster.Paz_Codice, disunisciPazientiCommand.PazienteAlias.Paz_Codice, True)

                    ' Ciclo per ogni elemento
                    If Not dtValoriMerge Is Nothing AndAlso dtValoriMerge.Rows.Count > 0 Then

                        For Each row As DataRow In dtValoriMerge.Rows

                            ' Vecchia sequence di riferimento
                            Dim oldSequenceId As Integer = row(mergeInfo.CampoFkPadre)

                            ' Nuova sequence che verrà inserita
                            Dim newSequenceId As Integer = Me.GenericProvider.AliasPazienti.GetNextSequenceValue(mergeInfo.NomeSequence)

                            ' Inserimento dati dell'alias nella tabella padre
                            Me.GenericProvider.AliasPazienti.InsertValoreAliasPadre(mergeInfo, disunisciPazientiCommand.PazienteMaster.Paz_Codice, disunisciPazientiCommand.PazienteAlias.Paz_Codice, newSequenceId, row, True)

                            ' Inserimento dati dell'alias nella tabella figlia
                            Me.GenericProvider.AliasPazienti.InsertValoriAliasFiglia(mergeInfo, disunisciPazientiCommand.PazienteMaster.Paz_Codice, disunisciPazientiCommand.PazienteAlias.Paz_Codice, oldSequenceId, newSequenceId, True)

                        Next

                    End If

                    ' Eliminazione dati nella tabella figlia
                    Me.GenericProvider.AliasPazienti.DeleteDatiUnmergeAlias(mergeInfo.NomeTabellaFiglia, mergeInfo.CampoCodicePazienteFiglia, mergeInfo.CampoCodicePazienteOldFiglia, disunisciPazientiCommand.PazienteMaster.Paz_Codice, disunisciPazientiCommand.PazienteAlias.Paz_Codice)

                    ' Eliminazione dati nella tabella padre
                    Me.GenericProvider.AliasPazienti.DeleteDatiUnmergeAlias(mergeInfo.NomeTabellaPadre, mergeInfo.CampoCodicePazientePadre, mergeInfo.CampoCodicePazienteOldPadre, disunisciPazientiCommand.PazienteMaster.Paz_Codice, disunisciPazientiCommand.PazienteAlias.Paz_Codice)

                Next

            End If

            If Not LogOptions Is Nothing Then

                Dim testataLog As New DataLogStructure.Testata(LogOptions.CodiceArgomento, DataLogStructure.Operazione.Modifica, LogOptions.Automatico)

                ' TODO [BizPaziente]: DisunisciPazienti =>  Log !!!
                ' testataLog.Records.Add(PazienteLogManager.WriteInsertUpdatePazienteLog(logTestata, Paziente, pazienteOriginale))
                ' testataLog.Records.Add(MovimentoCNSLogManager.WriteInsertMovimentoCNSLog(logTestata, Paziente, pazienteOriginale))
                ' testataLog.Records.Add(ConvocazioneLogManager.WriteDeleteConvocazioniLog(logTestata, Paziente, convocazioniEliminate))
                ' testataLog.Records.Add(ConvocazioneLogManager.WriteUpdateConvocazioniLog(logTestata, Paziente, convocazioniModificate, convocazioniOriginali))

                WriteLog(testataLog)

            End If

            transactionScope.Complete()

        End Using

        ' TODO [BizPaziente]: DisunisciPazientiResult => success messages ?? 
        Return New BizResult()

    End Function

#End Region

    '''' <summary>
    '''' Restituisce true se ha ricalcolato la residenza del paziente.
    '''' Per i pazienti con categoria cittadino (PV1.18) = "91" e motivo di cessazione assistenza (GT1.15) = "01" 
    '''' viene modificata la residenza, impostandola uguale all'emigrazione (PID.11 di tipo "E"), se compilata
    '''' Se i dati di emigrazione sono vuoti, la residenza viene impostata a comune sconosciuto (999999)
    '''' </summary>
    '''' <param name="paziente"></param>
    '''' <returns></returns>
    'Private Function RicalcolaResidenza(paziente As Paziente) As Boolean

    '    If paziente.CategoriaCittadino = Constants.CategorieCittadino.Cessato AndAlso paziente.MotivoCessazioneAssistenza = Constants.MotiviCessazioneAssistenza.Trasferimento Then

    '        paziente.ComuneResidenza_DataInizio = Date.Now
    '        paziente.ComuneResidenza_DataFine = Date.MinValue

    '        paziente.ComuneResidenza_Cap = String.Empty
    '        paziente.ComuneResidenza_Descrizione = String.Empty
    '        paziente.ComuneResidenza_Provincia = String.Empty

    '        If String.IsNullOrWhiteSpace(paziente.ComuneEmigrazione_Codice) Then
    '            paziente.ComuneResidenza_Codice = Settings.COMUNE_SCONOSCIUTO
    '        Else
    '            paziente.ComuneResidenza_Codice = paziente.ComuneEmigrazione_Codice
    '        End If

    '        Return True

    '    End If

    '    Return False

    'End Function

#End Region

#Region " Dati Vaccinali Centrali "

#Region " Public "

    Public Function AcquisisciDatiVaccinaliCentrali(acquisisciDatiVaccinaliCentraliCommand As AcquisisciDatiVaccinaliCentraliCommand) As AcquisisciDatiVaccinaliCentraliResult

        ' [Unificazione Ulss]: viene richiamato solo per le usl gestite => OK

        Dim acquisisciDatiVaccinaliCentraliResult As New AcquisisciDatiVaccinaliCentraliResult()
        acquisisciDatiVaccinaliCentraliResult.AcquisisciVaccinazioneEseguitaCentraleResult = New BizVaccinazioniEseguite.AcquisisciVaccinazioneEseguitaCentraleResult()

        Dim noteAcquisizioneStringBuilder As New StringBuilder()

        Dim dataOperazione As DateTime = DateTime.Now

        Using transactionScope As New System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, GetReadCommittedTransactionOptions())

            Dim paziente As Paziente = GenericProvider.Paziente.GetPazienti(acquisisciDatiVaccinaliCentraliCommand.CodicePaziente, ContextInfos.CodiceUsl)(0)

            If acquisisciDatiVaccinaliCentraliCommand.LoadDatiVaccinaliCentrali Then

                Dim loadDatiVaccinaliCentraliCommand As New LoadDatiVaccinaliCentraliCommand()
                loadDatiVaccinaliCentraliCommand.LoadDatiVaccinaliUslGestitaCorrente = False
                loadDatiVaccinaliCentraliCommand.LoadDatiVaccinaliEliminati = False
                loadDatiVaccinaliCentraliCommand.LoadReazioneAvverse = True
                loadDatiVaccinaliCentraliCommand.LoadOsservazioni = True

                loadDatiVaccinaliCentraliCommand.VaccinazioniEseguiteCentrale = GenericProviderCentrale.VaccinazioneEseguitaCentrale.GetVaccinazioneEseguitaCentraleEnumerable(
                    acquisisciDatiVaccinaliCentraliCommand.CodicePazienteCentrale).Where(
                        Function(vec) (Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(vec.FlagVisibilitaCentrale))).ToArray()

                loadDatiVaccinaliCentraliCommand.VaccinazioniEscluseCentrale = GenericProviderCentrale.VaccinazioneEsclusaCentrale.GetVaccinazioneEsclusaCentraleEnumerable(
                    acquisisciDatiVaccinaliCentraliCommand.CodicePazienteCentrale).Where(
                        Function(vec) Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(vec.FlagVisibilitaCentrale)).ToArray()

                loadDatiVaccinaliCentraliCommand.VisiteCentrale = GenericProviderCentrale.VisitaCentrale.GetVisitaCentraleEnumerable(
                    acquisisciDatiVaccinaliCentraliCommand.CodicePazienteCentrale).Where(
                        Function(vec) Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(vec.FlagVisibilitaCentrale)).ToArray()

                Dim loadDatiVaccinaliCentraliResult As LoadDatiVaccinaliCentraliResult = LoadDatiVaccinaliCentrali(loadDatiVaccinaliCentraliCommand)

                If loadDatiVaccinaliCentraliResult.DatiVaccinaliCentraliInfo.Empty Then
                    acquisisciDatiVaccinaliCentraliResult.StatoAcquisizioneDatiVaccinaliCentrale = Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Vuota
                Else
                    acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo = loadDatiVaccinaliCentraliResult.DatiVaccinaliCentraliInfo
                End If

            End If

            If Not acquisisciDatiVaccinaliCentraliResult.StatoAcquisizioneDatiVaccinaliCentrale.HasValue OrElse
                acquisisciDatiVaccinaliCentraliResult.StatoAcquisizioneDatiVaccinaliCentrale <> Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Vuota Then

                Dim codicePazienteLocaleProgrammazione As String = Nothing
                Dim dbGenericProviderProgrammazione As DbGenericProvider = Nothing

                If Not String.IsNullOrEmpty(acquisisciDatiVaccinaliCentraliCommand.IdApplicazioneProgrammazione) Then

                    dbGenericProviderProgrammazione = Me.GenericProviderFactory.GetDbGenericProvider(GetUslGestitaByIDApplicazione(acquisisciDatiVaccinaliCentraliCommand.IdApplicazioneProgrammazione).Codice, Me.ContextInfos.CodiceAzienda)

                    codicePazienteLocaleProgrammazione = dbGenericProviderProgrammazione.Paziente.GetCodicePazientiByCodiceAusiliario(acquisisciDatiVaccinaliCentraliCommand.CodicePazienteCentrale)(0)

                End If

                If acquisisciDatiVaccinaliCentraliCommand.RichiediConfermaSovrascrittura Then

                    If Not String.IsNullOrEmpty(acquisisciDatiVaccinaliCentraliCommand.IdApplicazioneProgrammazione) Then
                        acquisisciDatiVaccinaliCentraliResult.ProgrammazionePresente = (Me.GenericProvider.Convocazione.Exists(acquisisciDatiVaccinaliCentraliCommand.CodicePaziente))
                    End If

                    acquisisciDatiVaccinaliCentraliResult.VaccinazioniEsclusePresenti =
                        (acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VaccinazioneEsclusaCentraleInfos.Count > 0)

                End If

                If Not acquisisciDatiVaccinaliCentraliCommand.RichiediConfermaSovrascrittura OrElse
                    (Not acquisisciDatiVaccinaliCentraliResult.VaccinazioniEsclusePresenti AndAlso Not acquisisciDatiVaccinaliCentraliResult.ProgrammazionePresente) Then

                    Dim vaccinazioniEseguiteInseriteList As New List(Of VaccinazioneEseguita)()
                    Dim vaccinazioniEseguiteNonInseriteList As New List(Of VaccinazioneEseguita)()
                    Dim vaccinazioniEseguiteScaduteInseriteList As New List(Of VaccinazioneEseguita)()
                    Dim vaccinazioniEseguiteScaduteNonInseriteList As New List(Of VaccinazioneEseguita)()

                    'VACCINAZIONI ESCLUSE
                    Using bizVaccinazioniEscluse As New BizVaccinazioniEscluse(Me.GenericProviderFactory, Me.Settings, Me.ContextInfos, Me.LogOptions)

                        'VACCINAZIONI ESCLUSE ELIMINATE
                        If Not acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VaccinazioneEsclusaEliminataCentraleInfos Is Nothing Then

                            bizVaccinazioniEscluse.AcquisisciVaccinazioneEsclusaCentrale(acquisisciDatiVaccinaliCentraliCommand.CodicePaziente,
                                acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VaccinazioneEsclusaEliminataCentraleInfos,
                                Constants.TipoVaccinazioneEsclusaCentrale.Eliminata)    ', acquisisciDatiVaccinaliCentraliCommand.IsVisibilitaModificata)

                        End If

                        'VACCINAZIONI ESCLUSE
                        If Not acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VaccinazioneEsclusaCentraleInfos Is Nothing Then

                            bizVaccinazioniEscluse.AcquisisciVaccinazioneEsclusaCentrale(acquisisciDatiVaccinaliCentraliCommand.CodicePaziente,
                                acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VaccinazioneEsclusaCentraleInfos,
                                Constants.TipoVaccinazioneEsclusaCentrale.Nessuno)  ', acquisisciDatiVaccinaliCentraliCommand.IsVisibilitaModificata)

                        End If

                    End Using

                    'VACCINAZIONI ESEGUITE / REAZIONI AVVERSE
                    Using bizVaccinazioniEseguite As New BizVaccinazioniEseguite(Me.GenericProviderFactory, Me.Settings, Me.ContextInfos, Me.LogOptions)

                        ' REAZIONI AVVERSE ELIMINATE
                        If Not acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.ReazioneAvversaEliminataCentraleInfos Is Nothing Then

                            bizVaccinazioniEseguite.AcquisisciReazioneAvversaCentrale(acquisisciDatiVaccinaliCentraliCommand.CodicePaziente,
                                acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.ReazioneAvversaEliminataCentraleInfos)

                        End If

                        'VACCINAZIONI ESEGUITE ELIMINATE
                        If Not acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VaccinazioneEseguitaEliminataCentraleInfos Is Nothing Then

                            bizVaccinazioniEseguite.AcquisisciVaccinazioneEseguitaCentrale(acquisisciDatiVaccinaliCentraliCommand.CodicePaziente,
                                acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VaccinazioneEseguitaEliminataCentraleInfos,
                                Constants.TipoVaccinazioneEseguitaCentrale.Eliminata, acquisisciDatiVaccinaliCentraliCommand.IsVisibilitaModificata)

                        End If

                        'VACCINAZIONE ESEGUITA SCADUTA ELIMINATA
                        If Not acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VaccinazioneEseguitaScadutaEliminataCentraleInfos Is Nothing Then

                            bizVaccinazioniEseguite.AcquisisciVaccinazioneEseguitaCentrale(acquisisciDatiVaccinaliCentraliCommand.CodicePaziente,
                                acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VaccinazioneEseguitaScadutaEliminataCentraleInfos,
                                Constants.TipoVaccinazioneEseguitaCentrale.ScadutaEliminata, acquisisciDatiVaccinaliCentraliCommand.IsVisibilitaModificata)

                        End If

                        'VACCINAZIONI ESEGUITE SCADUTE
                        If Not acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VaccinazioneEseguitaScadutaCentraleInfos Is Nothing Then

                            Dim acquisisciVaccinazioneEseguitaCentraleResult As BizVaccinazioniEseguite.AcquisisciVaccinazioneEseguitaCentraleResult =
                                bizVaccinazioniEseguite.AcquisisciVaccinazioneEseguitaCentrale(acquisisciDatiVaccinaliCentraliCommand.CodicePaziente,
                                    acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VaccinazioneEseguitaScadutaCentraleInfos,
                                    Constants.TipoVaccinazioneEseguitaCentrale.Scaduta, acquisisciDatiVaccinaliCentraliCommand.IsVisibilitaModificata)

                            vaccinazioniEseguiteScaduteInseriteList.AddRange(acquisisciVaccinazioneEseguitaCentraleResult.VaccinazioniEseguiteScaduteInserite)
                            vaccinazioniEseguiteScaduteNonInseriteList.AddRange(acquisisciVaccinazioneEseguitaCentraleResult.VaccinazioniEseguiteScaduteNonInserite)

                        End If

                        'VACCINAZIONI ESEGUITE RIPRISTINATE
                        If Not acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VaccinazioneEseguitaRipristinataCentraleInfos Is Nothing Then

                            Dim acquisisciVaccinazioneEseguitaCentraleResult As BizVaccinazioniEseguite.AcquisisciVaccinazioneEseguitaCentraleResult =
                                bizVaccinazioniEseguite.AcquisisciVaccinazioneEseguitaCentrale(acquisisciDatiVaccinaliCentraliCommand.CodicePaziente,
                                    acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VaccinazioneEseguitaRipristinataCentraleInfos,
                                    Constants.TipoVaccinazioneEseguitaCentrale.Ripristinata, acquisisciDatiVaccinaliCentraliCommand.IsVisibilitaModificata)

                            vaccinazioniEseguiteInseriteList.AddRange(acquisisciVaccinazioneEseguitaCentraleResult.VaccinazioniEseguiteInserite)
                            vaccinazioniEseguiteNonInseriteList.AddRange(acquisisciVaccinazioneEseguitaCentraleResult.VaccinazioniEseguiteNonInserite)

                        End If

                        'VACCINAZIONI ESEGUITE RECUPERATE
                        If Not acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VaccinazioneEseguitaRecuperataCentraleInfos Is Nothing Then

                            Dim acquisisciVaccinazioneEseguitaCentraleResult As BizVaccinazioniEseguite.AcquisisciVaccinazioneEseguitaCentraleResult =
                                    bizVaccinazioniEseguite.AcquisisciVaccinazioneEseguitaCentrale(acquisisciDatiVaccinaliCentraliCommand.CodicePaziente,
                                        acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VaccinazioneEseguitaRecuperataCentraleInfos,
                                        Constants.TipoVaccinazioneEseguitaCentrale.Recuperata, acquisisciDatiVaccinaliCentraliCommand.IsVisibilitaModificata)

                            vaccinazioniEseguiteInseriteList.AddRange(acquisisciVaccinazioneEseguitaCentraleResult.VaccinazioniEseguiteInserite)
                            vaccinazioniEseguiteNonInseriteList.AddRange(acquisisciVaccinazioneEseguitaCentraleResult.VaccinazioniEseguiteNonInserite)

                        End If

                        'VACCINAZIONI ESEGUITE REGISTRATE
                        If Not acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VaccinazioneEseguitaRegistrataCentraleInfos Is Nothing Then

                            Dim acquisisciVaccinazioneEseguitaCentraleResult As BizVaccinazioniEseguite.AcquisisciVaccinazioneEseguitaCentraleResult =
                                    bizVaccinazioniEseguite.AcquisisciVaccinazioneEseguitaCentrale(acquisisciDatiVaccinaliCentraliCommand.CodicePaziente,
                                        acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VaccinazioneEseguitaRegistrataCentraleInfos,
                                        Constants.TipoVaccinazioneEseguitaCentrale.Registrata, acquisisciDatiVaccinaliCentraliCommand.IsVisibilitaModificata)

                            vaccinazioniEseguiteInseriteList.AddRange(acquisisciVaccinazioneEseguitaCentraleResult.VaccinazioniEseguiteInserite)
                            vaccinazioniEseguiteNonInseriteList.AddRange(acquisisciVaccinazioneEseguitaCentraleResult.VaccinazioniEseguiteNonInserite)

                        End If

                        'VACCINAZIONI ESEGUITE PROGRAMMATE
                        If Not acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VaccinazioneEseguitaProgrammataCentraleInfos Is Nothing Then

                            Dim acquisisciVaccinazioneEseguitaCentraleResult As BizVaccinazioniEseguite.AcquisisciVaccinazioneEseguitaCentraleResult =
                                bizVaccinazioniEseguite.AcquisisciVaccinazioneEseguitaCentrale(acquisisciDatiVaccinaliCentraliCommand.CodicePaziente,
                                    acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VaccinazioneEseguitaProgrammataCentraleInfos,
                                    Constants.TipoVaccinazioneEseguitaCentrale.Programmata, acquisisciDatiVaccinaliCentraliCommand.IsVisibilitaModificata)

                            vaccinazioniEseguiteInseriteList.AddRange(acquisisciVaccinazioneEseguitaCentraleResult.VaccinazioniEseguiteInserite)
                            vaccinazioniEseguiteNonInseriteList.AddRange(acquisisciVaccinazioneEseguitaCentraleResult.VaccinazioniEseguiteNonInserite)

                        End If

                        'REAZIONI AVVERSE
                        If Not acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.ReazioneAvversaCentraleInfos Is Nothing Then

                            bizVaccinazioniEseguite.AcquisisciReazioneAvversaCentrale(acquisisciDatiVaccinaliCentraliCommand.CodicePaziente,
                                acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.ReazioneAvversaCentraleInfos)

                        End If

                    End Using

                    Using bizVisite As New BizVisite(Me.GenericProviderFactory, Me.Settings, Me.ContextInfos, Me.LogOptions)

                        Dim note As String = acquisisciDatiVaccinaliCentraliCommand.Note
                        If String.IsNullOrWhiteSpace(note) Then note = "Acquisizione visita da centrale"

                        ' VISITE ELIMINATE
                        If Not acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VisitaEliminataCentraleInfos Is Nothing Then

                            bizVisite.AcquisisciVisitaCentrale(acquisisciDatiVaccinaliCentraliCommand.CodicePaziente,
                                                               acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VisitaEliminataCentraleInfos,
                                                               Constants.TipoVisitaCentrale.Eliminata,
                                                               acquisisciDatiVaccinaliCentraliCommand.IsVisibilitaModificata,
                                                               note)

                        End If

                        ' VISITE 
                        If Not acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VisitaCentraleInfos Is Nothing Then

                            acquisisciDatiVaccinaliCentraliResult.AcquisisciVisitaCentraleResult =
                                bizVisite.AcquisisciVisitaCentrale(acquisisciDatiVaccinaliCentraliCommand.CodicePaziente,
                                                                   acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VisitaCentraleInfos,
                                                                   Constants.TipoVisitaCentrale.Nessuno,
                                                                   acquisisciDatiVaccinaliCentraliCommand.IsVisibilitaModificata,
                                                                   note)

                        End If

                    End Using

                    acquisisciDatiVaccinaliCentraliResult.AcquisisciVaccinazioneEseguitaCentraleResult.VaccinazioniEseguiteInserite = vaccinazioniEseguiteInseriteList.ToArray()
                    acquisisciDatiVaccinaliCentraliResult.AcquisisciVaccinazioneEseguitaCentraleResult.VaccinazioniEseguiteNonInserite = vaccinazioniEseguiteNonInseriteList.ToArray()
                    acquisisciDatiVaccinaliCentraliResult.AcquisisciVaccinazioneEseguitaCentraleResult.VaccinazioniEseguiteScaduteInserite = vaccinazioniEseguiteScaduteInseriteList.ToArray()
                    acquisisciDatiVaccinaliCentraliResult.AcquisisciVaccinazioneEseguitaCentraleResult.VaccinazioniEseguiteScaduteNonInserite = vaccinazioniEseguiteScaduteNonInseriteList.ToArray()

                    noteAcquisizioneStringBuilder.Append(acquisisciDatiVaccinaliCentraliResult.BuildMessage(Environment.NewLine))

                    If acquisisciDatiVaccinaliCentraliResult.DatiVaccinaliAcquisiti Then

                        If Not String.IsNullOrEmpty(acquisisciDatiVaccinaliCentraliCommand.IdApplicazioneProgrammazione) Then

                            '-- CONVOCAZIONI/CICLI/RITARDI/PROGRAMMATE

                            Dim convocazioneList As List(Of Entities.Convocazione) =
                                dbGenericProviderProgrammazione.Convocazione.GetConvocazioniPaziente(codicePazienteLocaleProgrammazione).ToList()

                            If convocazioneList.Count > 0 Then

                                Using bizConvocazione As New BizConvocazione(Me.GenericProviderFactory, Me.Settings, Me.ContextInfos, Me.LogOptions)

                                    Dim eliminaConvocazioniCommand As New BizConvocazione.EliminaConvocazioniCommand()
                                    eliminaConvocazioniCommand.CodicePaziente = acquisisciDatiVaccinaliCentraliCommand.CodicePaziente
                                    eliminaConvocazioniCommand.DataConvocazione = Nothing
                                    eliminaConvocazioniCommand.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.EliminazioneConvocazione
                                    eliminaConvocazioniCommand.DataEliminazione = dataOperazione
                                    eliminaConvocazioniCommand.NoteEliminazione = acquisisciDatiVaccinaliCentraliCommand.Note
                                    If Not String.IsNullOrWhiteSpace(eliminaConvocazioniCommand.NoteEliminazione) Then
                                        eliminaConvocazioniCommand.NoteEliminazione += " - "
                                    End If
                                    eliminaConvocazioniCommand.NoteEliminazione += "Eliminazione cnv per acquisizione da centrale"

                                    bizConvocazione.EliminaConvocazioni(eliminaConvocazioniCommand)

                                End Using

                                If acquisisciDatiVaccinaliCentraliResult.AcquisisciVaccinazioneEseguitaCentraleResult.VaccinazioniEseguiteAcquisite Then

                                    Dim cicliConvocazioniList As List(Of CicloConvocazione) = dbGenericProviderProgrammazione.Convocazione.GetCicliConvocazioniPaziente(codicePazienteLocaleProgrammazione).ToList()
                                    Dim ritardiCicliConvocazioniList As List(Of RitardoCicloConvocazione) = dbGenericProviderProgrammazione.Convocazione.GetRitardiCicliConvocazioniPaziente(codicePazienteLocaleProgrammazione).ToList()
                                    Dim vaccinazioneProgrammataList As List(Of VaccinazioneProgrammata) = dbGenericProviderProgrammazione.VaccinazioneProg.GetVaccinazioniProgrammatePaziente(codicePazienteLocaleProgrammazione).ToList()

                                    For Each convocazione As Entities.Convocazione In convocazioneList

                                        convocazione.Cns_Codice = acquisisciDatiVaccinaliCentraliCommand.CodiceConsultorioPaziente
                                        convocazione.Paz_codice = acquisisciDatiVaccinaliCentraliCommand.CodicePaziente
                                        convocazione.Paz_codice_old = New Int64?(Me.GenericProvider.Paziente.GetCodicePazientiByCodiceAusiliario(acquisisciDatiVaccinaliCentraliCommand.CodicePazienteCentrale).FirstOrDefault())

                                        convocazione.CodiceAmbulatorio = Nothing
                                        convocazione.DataInvio = Nothing
                                        convocazione.DataAppuntamento = Nothing

                                        Me.GenericProvider.Convocazione.InsertConvocazione(convocazione)

                                    Next

                                    For Each cicloConvocazione As CicloConvocazione In cicliConvocazioniList

                                        cicloConvocazione.CodicePaziente = acquisisciDatiVaccinaliCentraliCommand.CodicePaziente
                                        cicloConvocazione.CodicePazientePrecedente = New Int64?(Me.GenericProvider.Paziente.GetCodicePazientiByCodiceAusiliario(acquisisciDatiVaccinaliCentraliCommand.CodicePazienteCentrale).FirstOrDefault())

                                        Me.GenericProvider.Convocazione.InsertCicloConvocazione(cicloConvocazione)

                                    Next

                                    For Each ritardoCicloConvocazione As RitardoCicloConvocazione In ritardiCicliConvocazioniList

                                        ritardoCicloConvocazione.CodicePaziente = acquisisciDatiVaccinaliCentraliCommand.CodicePaziente
                                        ritardoCicloConvocazione.CodicePazientePrecedente = New Int64?(Me.GenericProvider.Paziente.GetCodicePazientiByCodiceAusiliario(acquisisciDatiVaccinaliCentraliCommand.CodicePazienteCentrale).FirstOrDefault())

                                        Me.GenericProvider.Convocazione.InsertRitardoCicloConvocazione(ritardoCicloConvocazione)

                                    Next

                                    For Each vaccinazioneProgrammata As VaccinazioneProgrammata In vaccinazioneProgrammataList

                                        vaccinazioneProgrammata.CodicePaziente = acquisisciDatiVaccinaliCentraliCommand.CodicePaziente
                                        vaccinazioneProgrammata.CodicePazientePrecedente = New Int64?(Me.GenericProvider.Paziente.GetCodicePazientiByCodiceAusiliario(acquisisciDatiVaccinaliCentraliCommand.CodicePazienteCentrale).FirstOrDefault())

                                        Me.GenericProvider.VaccinazioneProg.InsertVaccinazioneProgrammata(vaccinazioneProgrammata)

                                    Next

                                End If

                            End If

                        End If

                        acquisisciDatiVaccinaliCentraliResult.StatoAcquisizioneDatiVaccinaliCentrale = Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Totale

                    Else

                        acquisisciDatiVaccinaliCentraliResult.StatoAcquisizioneDatiVaccinaliCentrale = Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Parziale

                    End If

                End If

            End If

            If acquisisciDatiVaccinaliCentraliCommand.UpdateStatoAcquisizione Then

                Dim pazienteOriginale As Entities.Paziente = paziente.Clone()

                paziente.StatoAcquisizioneDatiVaccinaliCentrale = acquisisciDatiVaccinaliCentraliResult.StatoAcquisizioneDatiVaccinaliCentrale

                GenericProvider.Paziente.ModificaPaziente(paziente)

                'LOG
                If Not Me.LogOptions Is Nothing Then

                    Dim testataLog As New DataLogStructure.Testata(LogOptions.CodiceArgomento, DataLogStructure.Operazione.Modifica,
                                                                    acquisisciDatiVaccinaliCentraliCommand.CodicePaziente, Me.LogOptions.Automatico)

                    testataLog.Records.Add(PazienteLogManager.GetLogPaziente(paziente, pazienteOriginale))

                    WriteLog(testataLog)

                End If

            End If

            transactionScope.Complete()

        End Using

        Return acquisisciDatiVaccinaliCentraliResult

    End Function

    Public Sub AggiornaDatiVaccinaliCentrali(aggiornaDatiVaccinaliCentraliCommand As AggiornaDatiVaccinaliCentraliCommand)

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions())

            Dim acquisizioneNecessaria As Boolean = False

            ' **************************************
            ' CENTRALE - DISTRIBUITA USL INSERIMENTO
            ' **************************************

            ' *** VACCINAZIONE ESCLUSA ***
            Dim vaccinazioneEsclusaAcquisizioneCentraleInfoList As New List(Of BizVaccinazioniEscluse.VaccinazioneEsclusaCentraleInfo)()
            Dim vaccinazioneEsclusaEliminataAcquisizioneCentraleInfoList As New List(Of BizVaccinazioniEscluse.VaccinazioneEsclusaCentraleInfo)()

            Using bizVaccinazioniEscluse As New BizVaccinazioniEscluse(Me.GenericProviderFactory, Me.Settings, Me.ContextInfos, Me.LogOptions)

                ' VACCINAZIONE ESCLUSA ELIMINATA
                If Not aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEsclusaEliminataEnumerable Is Nothing Then

                    For Each vaccinazioneEsclusaEliminata As Entities.VaccinazioneEsclusa In aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEsclusaEliminataEnumerable

                        Dim vaccinazioneEsclusaAcquisizioneInfo As New BizVaccinazioniEscluse.VaccinazioneEsclusaCentraleInfo()
                        vaccinazioneEsclusaAcquisizioneInfo.VaccinazioneEsclusa = vaccinazioneEsclusaEliminata

                        vaccinazioneEsclusaAcquisizioneInfo.VaccinazioneEsclusaCentrale = bizVaccinazioniEscluse.AggiornaVaccinazioneEsclusaCentrale(
                            aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale, vaccinazioneEsclusaEliminata, Constants.TipoVaccinazioneEsclusaCentrale.Eliminata,
                            aggiornaDatiVaccinaliCentraliCommand.IsMerge, aggiornaDatiVaccinaliCentraliCommand.IsRisoluzioneConflitto)

                        If aggiornaDatiVaccinaliCentraliCommand.IsMerge OrElse
                            Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(vaccinazioneEsclusaAcquisizioneInfo.VaccinazioneEsclusaCentrale.FlagVisibilitaCentrale) Then

                            vaccinazioneEsclusaEliminataAcquisizioneCentraleInfoList.Add(vaccinazioneEsclusaAcquisizioneInfo)

                            acquisizioneNecessaria = True

                        End If

                    Next

                End If

                ' VACCINAZIONE ESCLUSA
                If Not aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEsclusaEnumerable Is Nothing Then

                    For Each vaccinazioneEsclusa As Entities.VaccinazioneEsclusa In aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEsclusaEnumerable

                        Dim vaccinazioneEsclusaAcquisizioneInfo As New BizVaccinazioniEscluse.VaccinazioneEsclusaCentraleInfo()
                        vaccinazioneEsclusaAcquisizioneInfo.VaccinazioneEsclusa = vaccinazioneEsclusa

                        vaccinazioneEsclusaAcquisizioneInfo.VaccinazioneEsclusaCentrale = bizVaccinazioniEscluse.AggiornaVaccinazioneEsclusaCentrale(
                            aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale, vaccinazioneEsclusa, Constants.TipoVaccinazioneEsclusaCentrale.Nessuno,
                            aggiornaDatiVaccinaliCentraliCommand.IsMerge, aggiornaDatiVaccinaliCentraliCommand.IsRisoluzioneConflitto)

                        If Not String.IsNullOrEmpty(aggiornaDatiVaccinaliCentraliCommand.FlagVisibilitaModificata) OrElse
                            Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(vaccinazioneEsclusaAcquisizioneInfo.VaccinazioneEsclusaCentrale.FlagVisibilitaCentrale) Then

                            vaccinazioneEsclusaAcquisizioneCentraleInfoList.Add(vaccinazioneEsclusaAcquisizioneInfo)

                            acquisizioneNecessaria = True

                        End If

                    Next

                End If

            End Using

            ' *** VACCINAZIONE ESEGUITE / REAZIONI AVVERSE ***
            Dim vaccinazioneEseguitaEliminataAcquisizioneCentraleInfoList As New List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)()
            Dim vaccinazioneEseguitaScadutaEliminataAcquisizioneCentraleInfoList As New List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)()
            Dim vaccinazioneEseguitaScadutaAcquisizioneCentraleInfoList As New List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)()
            Dim vaccinazioneEseguitaProgrammataAcquisizioneCentraleInfoList As New List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)()
            Dim vaccinazioneEseguitaRegistrataAcquisizioneCentraleInfoList As New List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)()
            Dim vaccinazioneEseguitaRecuperataAcquisizioneCentraleInfoList As New List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)()
            Dim vaccinazioneEseguitaRipristinataAcquisizioneCentraleInfoList As New List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)()

            Dim reazioneAvversaAcquisizioneCentraleInfoList As New List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)()
            Dim reazioneAvversaEliminataAcquisizioneCentraleInfoList As New List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)()

            Dim idReazioneAvversaAggiornataList As New List(Of Int64)()

            Using bizVaccinazioniEseguite As New BizVaccinazioniEseguite(Me.GenericProviderFactory, Me.Settings, Me.ContextInfos, Me.LogOptions)

                ' VACCINAZIONE ESEGUITA ELIMINATA
                If Not aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEseguitaEliminataEnumerable Is Nothing Then

                    For Each vaccinazioneEseguitaEliminata As Entities.VaccinazioneEseguita In aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEseguitaEliminataEnumerable

                        Dim vesId As Long? = vaccinazioneEseguitaEliminata.ves_id

                        Dim vaccinazioneEseguitaAcquisizioneInfo As New BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo()
                        vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguita = vaccinazioneEseguitaEliminata

                        If Not aggiornaDatiVaccinaliCentraliCommand.ReazioneAvversaEliminataEnumerable Is Nothing Then

                            vaccinazioneEseguitaAcquisizioneInfo.ReazioneAvversa = aggiornaDatiVaccinaliCentraliCommand.ReazioneAvversaEliminataEnumerable.FirstOrDefault(
                               Function(rea) rea.IdVaccinazioneEseguita = vesId)

                            If Not vaccinazioneEseguitaAcquisizioneInfo.ReazioneAvversa Is Nothing Then
                                idReazioneAvversaAggiornataList.Add(vaccinazioneEseguitaAcquisizioneInfo.ReazioneAvversa.IdReazioneAvversa)
                            End If

                        End If

                        vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguitaCentrale = bizVaccinazioniEseguite.AggiornaVaccinazioneEseguitaCentrale(
                            aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale, vaccinazioneEseguitaEliminata, vaccinazioneEseguitaAcquisizioneInfo.ReazioneAvversa,
                            Constants.TipoVaccinazioneEseguitaCentrale.Eliminata, Constants.TipoReazioneAvversaCentrale.Eliminata,
                            aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentraleAlias, aggiornaDatiVaccinaliCentraliCommand.IsMerge,
                            aggiornaDatiVaccinaliCentraliCommand.IsRisoluzioneConflitto, Not String.IsNullOrEmpty(aggiornaDatiVaccinaliCentraliCommand.FlagVisibilitaModificata))

                        If aggiornaDatiVaccinaliCentraliCommand.IsMerge OrElse
                            Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguitaCentrale.FlagVisibilitaCentrale) Then

                            vaccinazioneEseguitaEliminataAcquisizioneCentraleInfoList.Add(vaccinazioneEseguitaAcquisizioneInfo)

                            acquisizioneNecessaria = True

                        End If

                    Next

                End If

                ' VACCINAZIONE ESEGUITA SCADUTA ELIMINATA
                If Not aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEseguitaScadutaEliminataEnumerable Is Nothing Then

                    For Each vaccinazioneEseguitaScadutaEliminata As Entities.VaccinazioneEseguita In aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEseguitaScadutaEliminataEnumerable

                        Dim vesId As Long? = vaccinazioneEseguitaScadutaEliminata.ves_id

                        Dim vaccinazioneEseguitaAcquisizioneInfo As New BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo()
                        vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguita = vaccinazioneEseguitaScadutaEliminata

                        If Not aggiornaDatiVaccinaliCentraliCommand.ReazioneAvversaEliminataEnumerable Is Nothing Then

                            vaccinazioneEseguitaAcquisizioneInfo.ReazioneAvversa = aggiornaDatiVaccinaliCentraliCommand.ReazioneAvversaEliminataEnumerable.FirstOrDefault(
                               Function(rea) rea.IdVaccinazioneEseguita = vesId)

                            If Not vaccinazioneEseguitaAcquisizioneInfo.ReazioneAvversa Is Nothing Then
                                idReazioneAvversaAggiornataList.Add(vaccinazioneEseguitaAcquisizioneInfo.ReazioneAvversa.IdReazioneAvversa)
                            End If

                        End If

                        vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguitaCentrale = bizVaccinazioniEseguite.AggiornaVaccinazioneEseguitaCentrale(
                            aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale, vaccinazioneEseguitaScadutaEliminata,
                            vaccinazioneEseguitaAcquisizioneInfo.ReazioneAvversa, Constants.TipoVaccinazioneEseguitaCentrale.ScadutaEliminata,
                            Constants.TipoReazioneAvversaCentrale.Eliminata, aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentraleAlias,
                            aggiornaDatiVaccinaliCentraliCommand.IsMerge, aggiornaDatiVaccinaliCentraliCommand.IsRisoluzioneConflitto,
                            Not String.IsNullOrEmpty(aggiornaDatiVaccinaliCentraliCommand.FlagVisibilitaModificata))

                        If aggiornaDatiVaccinaliCentraliCommand.IsMerge OrElse
                            Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguitaCentrale.FlagVisibilitaCentrale) Then

                            vaccinazioneEseguitaScadutaEliminataAcquisizioneCentraleInfoList.Add(vaccinazioneEseguitaAcquisizioneInfo)

                            acquisizioneNecessaria = True

                        End If

                    Next

                End If

                ' VACCINAZIONE ESEGUITA SCADUTA
                If Not aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEseguitaScadutaEnumerable Is Nothing Then

                    For Each vaccinazioneEseguitaScaduta As Entities.VaccinazioneEseguita In aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEseguitaScadutaEnumerable

                        Dim vesId As Long? = vaccinazioneEseguitaScaduta.ves_id

                        Dim vaccinazioneEseguitaAcquisizioneInfo As New BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo()
                        vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguita = vaccinazioneEseguitaScaduta

                        If Not aggiornaDatiVaccinaliCentraliCommand.ReazioneAvversaEnumerable Is Nothing Then

                            vaccinazioneEseguitaAcquisizioneInfo.ReazioneAvversa = aggiornaDatiVaccinaliCentraliCommand.ReazioneAvversaEnumerable.FirstOrDefault(
                                Function(rea) rea.IdVaccinazioneEseguita = vesId)

                            If Not vaccinazioneEseguitaAcquisizioneInfo.ReazioneAvversa Is Nothing Then
                                idReazioneAvversaAggiornataList.Add(vaccinazioneEseguitaAcquisizioneInfo.ReazioneAvversa.IdReazioneAvversa)
                            End If

                        End If

                        vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguitaCentrale = bizVaccinazioniEseguite.AggiornaVaccinazioneEseguitaCentrale(
                           aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale, vaccinazioneEseguitaScaduta, vaccinazioneEseguitaAcquisizioneInfo.ReazioneAvversa,
                           Constants.TipoVaccinazioneEseguitaCentrale.Scaduta, Constants.TipoReazioneAvversaCentrale.Nessuno,
                           aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentraleAlias,
                           aggiornaDatiVaccinaliCentraliCommand.IsMerge, aggiornaDatiVaccinaliCentraliCommand.IsRisoluzioneConflitto,
                           Not String.IsNullOrEmpty(aggiornaDatiVaccinaliCentraliCommand.FlagVisibilitaModificata))

                        If aggiornaDatiVaccinaliCentraliCommand.IsMerge OrElse
                            Not String.IsNullOrEmpty(aggiornaDatiVaccinaliCentraliCommand.FlagVisibilitaModificata) OrElse
                            Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguitaCentrale.FlagVisibilitaCentrale) Then

                            vaccinazioneEseguitaScadutaAcquisizioneCentraleInfoList.Add(vaccinazioneEseguitaAcquisizioneInfo)

                            acquisizioneNecessaria = True

                        End If

                    Next

                End If

                ' VACCINAZIONE ESEGUITA
                If Not aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEseguitaEnumerable Is Nothing Then

                    For Each vaccinazioneEseguita As Entities.VaccinazioneEseguita In aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEseguitaEnumerable

                        Dim vesId As Long? = vaccinazioneEseguita.ves_id

                        Dim vaccinazioneEseguitaAcquisizioneInfo As New BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo()
                        vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguita = vaccinazioneEseguita

                        If Not aggiornaDatiVaccinaliCentraliCommand.ReazioneAvversaEnumerable Is Nothing Then

                            vaccinazioneEseguitaAcquisizioneInfo.ReazioneAvversa = aggiornaDatiVaccinaliCentraliCommand.ReazioneAvversaEnumerable.FirstOrDefault(
                                Function(rea) rea.IdVaccinazioneEseguita = vesId)

                            If Not vaccinazioneEseguitaAcquisizioneInfo.ReazioneAvversa Is Nothing Then
                                idReazioneAvversaAggiornataList.Add(vaccinazioneEseguitaAcquisizioneInfo.ReazioneAvversa.IdReazioneAvversa)
                            End If

                        End If

                        vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguitaCentrale = bizVaccinazioniEseguite.AggiornaVaccinazioneEseguitaCentrale(
                            aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale, vaccinazioneEseguita, vaccinazioneEseguitaAcquisizioneInfo.ReazioneAvversa,
                            Nothing, Constants.TipoReazioneAvversaCentrale.Nessuno, aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentraleAlias,
                            aggiornaDatiVaccinaliCentraliCommand.IsMerge, aggiornaDatiVaccinaliCentraliCommand.IsRisoluzioneConflitto,
                            Not String.IsNullOrEmpty(aggiornaDatiVaccinaliCentraliCommand.FlagVisibilitaModificata))

                        If aggiornaDatiVaccinaliCentraliCommand.IsMerge OrElse
                            Not String.IsNullOrEmpty(aggiornaDatiVaccinaliCentraliCommand.FlagVisibilitaModificata) OrElse
                            Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguitaCentrale.FlagVisibilitaCentrale) Then

                            Select Case vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguitaCentrale.TipoVaccinazioneEseguitaCentrale
                                Case Constants.TipoVaccinazioneEseguitaCentrale.Programmata
                                    vaccinazioneEseguitaProgrammataAcquisizioneCentraleInfoList.Add(vaccinazioneEseguitaAcquisizioneInfo)
                                Case Constants.TipoVaccinazioneEseguitaCentrale.Registrata
                                    vaccinazioneEseguitaRegistrataAcquisizioneCentraleInfoList.Add(vaccinazioneEseguitaAcquisizioneInfo)
                                Case Constants.TipoVaccinazioneEseguitaCentrale.Recuperata
                                    vaccinazioneEseguitaRecuperataAcquisizioneCentraleInfoList.Add(vaccinazioneEseguitaAcquisizioneInfo)
                                Case Constants.TipoVaccinazioneEseguitaCentrale.Ripristinata
                                    vaccinazioneEseguitaRipristinataAcquisizioneCentraleInfoList.Add(vaccinazioneEseguitaAcquisizioneInfo)
                                Case Else
                                    Throw New NotImplementedException()
                            End Select

                            acquisizioneNecessaria = True

                        End If

                    Next

                End If

                ' REAZIONI AVVERSE ELIMINATA
                If Not aggiornaDatiVaccinaliCentraliCommand.ReazioneAvversaEliminataEnumerable Is Nothing Then

                    For Each reazioneAvversaEliminata As Entities.ReazioneAvversa In
                        aggiornaDatiVaccinaliCentraliCommand.ReazioneAvversaEliminataEnumerable.Where(
                            Function(rae) Not idReazioneAvversaAggiornataList.Contains(rae.IdReazioneAvversa))

                        Dim reazioneAvversaAcquisizioneInfo As New BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo()
                        reazioneAvversaAcquisizioneInfo.ReazioneAvversa = reazioneAvversaEliminata

                        reazioneAvversaAcquisizioneInfo.VaccinazioneEseguitaCentrale =
                            bizVaccinazioniEseguite.AggiornaVaccinazioneEseguitaCentrale(aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale, Nothing,
                                reazioneAvversaEliminata, Nothing, Constants.TipoReazioneAvversaCentrale.Eliminata, aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentraleAlias,
                                aggiornaDatiVaccinaliCentraliCommand.IsMerge, aggiornaDatiVaccinaliCentraliCommand.IsRisoluzioneConflitto,
                                Not String.IsNullOrEmpty(aggiornaDatiVaccinaliCentraliCommand.FlagVisibilitaModificata))

                        If aggiornaDatiVaccinaliCentraliCommand.IsMerge OrElse
                            Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(reazioneAvversaAcquisizioneInfo.VaccinazioneEseguitaCentrale.FlagVisibilitaCentrale) Then

                            reazioneAvversaEliminataAcquisizioneCentraleInfoList.Add(reazioneAvversaAcquisizioneInfo)

                            acquisizioneNecessaria = True

                        End If

                    Next

                End If

                ' REAZIONI AVVERSE
                If Not aggiornaDatiVaccinaliCentraliCommand.ReazioneAvversaEnumerable Is Nothing Then

                    For Each reazioneAvversa As Entities.ReazioneAvversa In
                        aggiornaDatiVaccinaliCentraliCommand.ReazioneAvversaEnumerable.Where(
                            Function(ra) Not idReazioneAvversaAggiornataList.Contains(ra.IdReazioneAvversa))

                        Dim reazioneAvversaAcquisizioneInfo As New BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo()
                        reazioneAvversaAcquisizioneInfo.ReazioneAvversa = reazioneAvversa

                        reazioneAvversaAcquisizioneInfo.VaccinazioneEseguitaCentrale =
                            bizVaccinazioniEseguite.AggiornaVaccinazioneEseguitaCentrale(aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale, Nothing,
                                reazioneAvversa, Nothing, Constants.TipoReazioneAvversaCentrale.Nessuno, aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentraleAlias,
                                aggiornaDatiVaccinaliCentraliCommand.IsMerge, aggiornaDatiVaccinaliCentraliCommand.IsRisoluzioneConflitto,
                                Not String.IsNullOrEmpty(aggiornaDatiVaccinaliCentraliCommand.FlagVisibilitaModificata))

                        If aggiornaDatiVaccinaliCentraliCommand.IsMerge OrElse
                           Not String.IsNullOrEmpty(aggiornaDatiVaccinaliCentraliCommand.FlagVisibilitaModificata) OrElse
                           Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(reazioneAvversaAcquisizioneInfo.VaccinazioneEseguitaCentrale.FlagVisibilitaCentrale) Then

                            reazioneAvversaAcquisizioneCentraleInfoList.Add(reazioneAvversaAcquisizioneInfo)

                            acquisizioneNecessaria = True

                        End If

                    Next

                End If

            End Using


            '*** VISITE / OSSERVAZIONI ***
            Dim visitaAcquisizioneCentraleInfoList As New List(Of BizVisite.VisitaCentraleInfo)()
            Dim visitaEliminataAcquisizioneCentraleInfoList As New List(Of BizVisite.VisitaCentraleInfo)()

            Using bizVisite As New BizVisite(Me.GenericProviderFactory, Me.Settings, Me.ContextInfos, Me.LogOptions)

                ' VISITE ELIMINATA
                If Not aggiornaDatiVaccinaliCentraliCommand.VisitaEliminataEnumerable Is Nothing Then

                    For Each visitaEliminata As Entities.Visita In aggiornaDatiVaccinaliCentraliCommand.VisitaEliminataEnumerable

                        Dim idVisita As Integer = visitaEliminata.IdVisita

                        Dim visitaAcquisizioneCentraleInfo As New BizVisite.VisitaCentraleInfo()
                        visitaAcquisizioneCentraleInfo.Visita = visitaEliminata

                        visitaAcquisizioneCentraleInfo.Osservazioni =
                            aggiornaDatiVaccinaliCentraliCommand.OsservazioneEliminataEnumerable.Where(Function(oe) oe.IdVisita = idVisita).ToArray()

                        visitaAcquisizioneCentraleInfo.VisitaCentrale = bizVisite.AggiornaVisitaCentrale(aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale,
                            visitaEliminata, Constants.TipoVisitaCentrale.Eliminata, aggiornaDatiVaccinaliCentraliCommand.IsMerge,
                            aggiornaDatiVaccinaliCentraliCommand.IsRisoluzioneConflitto)

                        If aggiornaDatiVaccinaliCentraliCommand.IsMerge OrElse
                            Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(visitaAcquisizioneCentraleInfo.VisitaCentrale.FlagVisibilitaCentrale) Then

                            visitaEliminataAcquisizioneCentraleInfoList.Add(visitaAcquisizioneCentraleInfo)

                            acquisizioneNecessaria = True

                        End If

                    Next

                End If

                ' VISITE
                If Not aggiornaDatiVaccinaliCentraliCommand.VisitaEnumerable Is Nothing Then

                    For Each visita As Entities.Visita In aggiornaDatiVaccinaliCentraliCommand.VisitaEnumerable

                        Dim idVisita As Integer = visita.IdVisita

                        Dim visitaAcquisizioneCentraleInfo As New BizVisite.VisitaCentraleInfo()
                        visitaAcquisizioneCentraleInfo.Visita = visita

                        If Not aggiornaDatiVaccinaliCentraliCommand.OsservazioneEnumerable Is Nothing Then

                            visitaAcquisizioneCentraleInfo.Osservazioni =
                                aggiornaDatiVaccinaliCentraliCommand.OsservazioneEnumerable.Where(Function(o) o.IdVisita = idVisita).ToArray()

                        End If

                        visitaAcquisizioneCentraleInfo.VisitaCentrale = bizVisite.AggiornaVisitaCentrale(
                            aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale, visita, Constants.TipoVisitaCentrale.Nessuno,
                            aggiornaDatiVaccinaliCentraliCommand.IsMerge, aggiornaDatiVaccinaliCentraliCommand.IsRisoluzioneConflitto)

                        If aggiornaDatiVaccinaliCentraliCommand.IsMerge OrElse
                           Not String.IsNullOrEmpty(aggiornaDatiVaccinaliCentraliCommand.FlagVisibilitaModificata) OrElse
                           Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(visitaAcquisizioneCentraleInfo.VisitaCentrale.FlagVisibilitaCentrale) Then

                            visitaAcquisizioneCentraleInfoList.Add(visitaAcquisizioneCentraleInfo)

                            acquisizioneNecessaria = True

                        End If

                    Next

                End If

            End Using


            ' *************************************
            ' LOCALE - DISTRIBUITA USL DESTINAZIONE
            ' *************************************

            If acquisizioneNecessaria Then

                ' [Unificazione Ulss]: qui si prendono in considerazione solo le usl gestite => usl gestita corrente è OK
                For Each uslDestinazione As Usl In UslGestite.Where(Function(usl) usl.Codice <> UslGestitaCorrente.Codice)

                    Dim codiceUslDestinazione As String = uslDestinazione.Codice

                    If uslDestinazione.FlagConsensoDatiVaccinaliCentralizzati Then

                        Dim pazienteDistribuito As PazienteDistribuito = GenericProviderCentrale.Paziente.GetPazienteDistribuito(aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale, uslDestinazione.Codice)

                        If Not pazienteDistribuito Is Nothing Then

                            Dim genericProviderDestinazione As DbGenericProvider = GenericProviderFactory.GetDbGenericProvider(uslDestinazione.IDApplicazione, ContextInfos.CodiceAzienda)

                            Dim acquisisciDatiVaccinaliCentraliCommand As New AcquisisciDatiVaccinaliCentraliCommand()
                            acquisisciDatiVaccinaliCentraliCommand.IsVisibilitaModificata = Not String.IsNullOrEmpty(aggiornaDatiVaccinaliCentraliCommand.FlagVisibilitaModificata)
                            acquisisciDatiVaccinaliCentraliCommand.CodicePazienteCentrale = aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale
                            acquisisciDatiVaccinaliCentraliCommand.CodicePaziente = pazienteDistribuito.CodicePaziente

                            Dim statoAcquisizioneDatiVaccinaliCentrale As Enumerators.StatoAcquisizioneDatiVaccinaliCentrale? =
                                genericProviderDestinazione.Paziente.GetStatoAcquisizioneDatiVaccinaliCentrale(pazienteDistribuito.CodicePaziente)

                            Dim isVisibilitaCentrale As Boolean =
                                Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(aggiornaDatiVaccinaliCentraliCommand.FlagVisibilitaModificata)

                            If (statoAcquisizioneDatiVaccinaliCentrale.HasValue AndAlso
                                    statoAcquisizioneDatiVaccinaliCentrale.Value <> Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Parziale) OrElse
                                (Not String.IsNullOrEmpty(aggiornaDatiVaccinaliCentraliCommand.FlagVisibilitaModificata) AndAlso
                                    Not isVisibilitaCentrale) Then

                                Dim vaccinazioneEseguitaProgrammataAcquisizioneInfoListUslCorrente As List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)
                                Dim vaccinazioneEseguitaRegistrataAcquisizioneInfoListUslCorrente As List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)
                                Dim vaccinazioneEseguitaRecuperataAcquisizioneInfoListUslCorrente As List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)
                                Dim vaccinazioneEseguitaRipristinataAcquisizioneInfoListUslCorrente As List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)
                                Dim vaccinazioneEseguitaScadutaAcquisizioneInfoListUslCorrente As List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)
                                Dim reazioneAvversaAcquisizioneInfoListUslCorrente As List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)
                                Dim vaccinazioneEsclusaAcquisizioneInfoListUslCorrente As List(Of BizVaccinazioniEscluse.VaccinazioneEsclusaCentraleInfo)
                                Dim visitaAcquisizioneInfoListUslCorrente As List(Of BizVisite.VisitaCentraleInfo)

                                If aggiornaDatiVaccinaliCentraliCommand.IsMerge AndAlso Not isVisibilitaCentrale Then

                                    ' Nel caso di merge, i dati vaccinali devono essere aggiornati in centrale e nelle usl locali, senza considerare la visibilità

                                    '-- VACCINAZIONE ESEGUITA PROGRAMMATA
                                    vaccinazioneEseguitaProgrammataAcquisizioneInfoListUslCorrente = New List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)()

                                    For Each vaccinazioneEseguitaProgrammataAcquisizioneInfo As BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo In vaccinazioneEseguitaProgrammataAcquisizioneCentraleInfoList

                                        Dim vesId As Long? = vaccinazioneEseguitaProgrammataAcquisizioneInfo.VaccinazioneEseguita.ves_id
                                        Dim codiceUslInserimento As String = vaccinazioneEseguitaProgrammataAcquisizioneInfo.VaccinazioneEseguita.ves_usl_inserimento

                                        If aggiornaDatiVaccinaliCentraliCommand.VaccinazioniEseguiteCentraliMasterMergeInfo.Any(
                                            Function(p) p.IdDatoVaccinaleUslInserimentoMaster = vesId AndAlso
                                                        p.CodiceUslInserimentoMaster = codiceUslInserimento AndAlso
                                                        p.CodiciUslDatiVaccinaliDistribuiti.Contains(codiceUslDestinazione)) Then

                                            vaccinazioneEseguitaProgrammataAcquisizioneInfoListUslCorrente.Add(vaccinazioneEseguitaProgrammataAcquisizioneInfo)

                                        End If

                                    Next

                                    '-- VACCINAZIONE ESEGUITA REGISTRATA
                                    vaccinazioneEseguitaRegistrataAcquisizioneInfoListUslCorrente = New List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)()

                                    For Each vaccinazioneEseguitaRegistrataAcquisizioneInfo As BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo In vaccinazioneEseguitaRegistrataAcquisizioneCentraleInfoList

                                        Dim vesId As Long? = vaccinazioneEseguitaRegistrataAcquisizioneInfo.VaccinazioneEseguita.ves_id
                                        Dim codiceUslInserimento As String = vaccinazioneEseguitaRegistrataAcquisizioneInfo.VaccinazioneEseguita.ves_usl_inserimento

                                        If aggiornaDatiVaccinaliCentraliCommand.VaccinazioniEseguiteCentraliMasterMergeInfo.Any(
                                            Function(p) p.IdDatoVaccinaleUslInserimentoMaster = vesId AndAlso
                                                        p.CodiceUslInserimentoMaster = codiceUslInserimento AndAlso
                                                        p.CodiciUslDatiVaccinaliDistribuiti.Contains(codiceUslDestinazione)) Then

                                            vaccinazioneEseguitaRegistrataAcquisizioneInfoListUslCorrente.Add(vaccinazioneEseguitaRegistrataAcquisizioneInfo)

                                        End If

                                    Next

                                    '-- VACCINAZIONE ESEGUITA RECUPERATA
                                    vaccinazioneEseguitaRecuperataAcquisizioneInfoListUslCorrente = New List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)()

                                    For Each vaccinazioneEseguitaRecuperataAcquisizioneInfo As BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo In vaccinazioneEseguitaRecuperataAcquisizioneCentraleInfoList

                                        Dim vesId As Long? = vaccinazioneEseguitaRecuperataAcquisizioneInfo.VaccinazioneEseguita.ves_id
                                        Dim codiceUslInserimento As String = vaccinazioneEseguitaRecuperataAcquisizioneInfo.VaccinazioneEseguita.ves_usl_inserimento

                                        If aggiornaDatiVaccinaliCentraliCommand.VaccinazioniEseguiteCentraliMasterMergeInfo.Any(
                                            Function(p) p.IdDatoVaccinaleUslInserimentoMaster = vesId AndAlso
                                                        p.CodiceUslInserimentoMaster = codiceUslInserimento AndAlso
                                                        p.CodiciUslDatiVaccinaliDistribuiti.Contains(codiceUslDestinazione)) Then

                                            vaccinazioneEseguitaRecuperataAcquisizioneInfoListUslCorrente.Add(vaccinazioneEseguitaRecuperataAcquisizioneInfo)

                                        End If

                                    Next

                                    '-- VACCINAZIONE ESEGUITA RIPRISTINATA
                                    vaccinazioneEseguitaRipristinataAcquisizioneInfoListUslCorrente = New List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)()

                                    For Each vaccinazioneEseguitaRipristinataAcquisizioneInfo As BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo In vaccinazioneEseguitaRipristinataAcquisizioneCentraleInfoList

                                        Dim vesId As Long? = vaccinazioneEseguitaRipristinataAcquisizioneInfo.VaccinazioneEseguita.ves_id
                                        Dim codiceUslInserimento As String = vaccinazioneEseguitaRipristinataAcquisizioneInfo.VaccinazioneEseguita.ves_usl_inserimento

                                        If aggiornaDatiVaccinaliCentraliCommand.VaccinazioniEseguiteCentraliMasterMergeInfo.Any(
                                            Function(p) p.IdDatoVaccinaleUslInserimentoMaster = vesId AndAlso
                                                        p.CodiceUslInserimentoMaster = codiceUslInserimento AndAlso
                                                        p.CodiciUslDatiVaccinaliDistribuiti.Contains(codiceUslDestinazione)) Then

                                            vaccinazioneEseguitaRipristinataAcquisizioneInfoListUslCorrente.Add(vaccinazioneEseguitaRipristinataAcquisizioneInfo)

                                        End If

                                    Next

                                    '-- VACCINAZIONE ESEGUITA SCADUTA
                                    vaccinazioneEseguitaScadutaAcquisizioneInfoListUslCorrente = New List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)()

                                    For Each vaccinazioneEseguitaScadutaAcquisizioneInfo As BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo In vaccinazioneEseguitaScadutaAcquisizioneCentraleInfoList

                                        Dim vesId As Long? = vaccinazioneEseguitaScadutaAcquisizioneInfo.VaccinazioneEseguita.ves_id
                                        Dim codiceUslInserimento As String = vaccinazioneEseguitaScadutaAcquisizioneInfo.VaccinazioneEseguita.ves_usl_inserimento

                                        If aggiornaDatiVaccinaliCentraliCommand.VaccinazioniEseguiteCentraliMasterMergeInfo.Any(
                                            Function(p) p.IdDatoVaccinaleUslInserimentoMaster = vesId AndAlso
                                                        p.CodiceUslInserimentoMaster = codiceUslInserimento AndAlso
                                                        p.CodiciUslDatiVaccinaliDistribuiti.Contains(codiceUslDestinazione)) Then

                                            vaccinazioneEseguitaScadutaAcquisizioneInfoListUslCorrente.Add(vaccinazioneEseguitaScadutaAcquisizioneInfo)

                                        End If

                                    Next

                                    '-- REAZIONE AVVERSA
                                    reazioneAvversaAcquisizioneInfoListUslCorrente = New List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)()

                                    For Each reazioneAvversaAcquisizioneCentraleInfo As BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo In reazioneAvversaAcquisizioneCentraleInfoList

                                        Dim idReazione As Long? = reazioneAvversaAcquisizioneCentraleInfo.ReazioneAvversa.IdReazioneAvversa
                                        Dim codiceUslInserimento As String = reazioneAvversaAcquisizioneCentraleInfo.ReazioneAvversa.CodiceUslInserimento

                                        If aggiornaDatiVaccinaliCentraliCommand.ReazioniAvverseCentraliMasterMergeInfo.Any(
                                            Function(p) p.IdDatoVaccinaleUslInserimentoMaster = idReazione AndAlso
                                                        p.CodiceUslInserimentoMaster = codiceUslInserimento AndAlso
                                                        p.CodiciUslDatiVaccinaliDistribuiti.Contains(codiceUslDestinazione)) Then

                                            reazioneAvversaAcquisizioneInfoListUslCorrente.Add(reazioneAvversaAcquisizioneCentraleInfo)

                                        End If

                                    Next

                                    '-- VACCINAZIONE ESCLUSA
                                    vaccinazioneEsclusaAcquisizioneInfoListUslCorrente = New List(Of BizVaccinazioniEscluse.VaccinazioneEsclusaCentraleInfo)()

                                    For Each vaccinazioneEsclusaAcquisizioneInfo As BizVaccinazioniEscluse.VaccinazioneEsclusaCentraleInfo In vaccinazioneEsclusaAcquisizioneCentraleInfoList

                                        Dim vexId As Integer = vaccinazioneEsclusaAcquisizioneInfo.VaccinazioneEsclusa.Id
                                        Dim codiceUslInserimento As String = vaccinazioneEsclusaAcquisizioneInfo.VaccinazioneEsclusa.CodiceUslInserimento

                                        If aggiornaDatiVaccinaliCentraliCommand.VaccinazioniEscluseCentraliMasterMergeInfo.Any(
                                            Function(p) p.IdDatoVaccinaleUslInserimentoMaster = vexId AndAlso
                                                        p.CodiceUslInserimentoMaster = codiceUslInserimento AndAlso
                                                        p.CodiciUslDatiVaccinaliDistribuiti.Contains(codiceUslDestinazione)) Then

                                            vaccinazioneEsclusaAcquisizioneInfoListUslCorrente.Add(vaccinazioneEsclusaAcquisizioneInfo)

                                        End If

                                    Next

                                    '-- VISITA
                                    visitaAcquisizioneInfoListUslCorrente = New List(Of BizVisite.VisitaCentraleInfo)()

                                    For Each visitaAcquisizioneCentraleInfo As BizVisite.VisitaCentraleInfo In visitaAcquisizioneCentraleInfoList

                                        Dim idVisita As Integer = visitaAcquisizioneCentraleInfo.Visita.IdVisita
                                        Dim codiceUslInserimento As String = visitaAcquisizioneCentraleInfo.Visita.CodiceUslInserimento

                                        If aggiornaDatiVaccinaliCentraliCommand.VisiteCentraliMasterMergeInfo.Any(
                                            Function(p) p.IdDatoVaccinaleUslInserimentoMaster = idVisita AndAlso
                                                        p.CodiceUslInserimentoMaster = codiceUslInserimento AndAlso
                                                        p.CodiciUslDatiVaccinaliDistribuiti.Contains(codiceUslDestinazione)) Then

                                            visitaAcquisizioneInfoListUslCorrente.Add(visitaAcquisizioneCentraleInfo)

                                        End If

                                    Next

                                Else
                                    vaccinazioneEseguitaProgrammataAcquisizioneInfoListUslCorrente = vaccinazioneEseguitaProgrammataAcquisizioneCentraleInfoList
                                    vaccinazioneEseguitaRecuperataAcquisizioneInfoListUslCorrente = vaccinazioneEseguitaRecuperataAcquisizioneCentraleInfoList
                                    vaccinazioneEseguitaRegistrataAcquisizioneInfoListUslCorrente = vaccinazioneEseguitaRegistrataAcquisizioneCentraleInfoList
                                    vaccinazioneEseguitaRipristinataAcquisizioneInfoListUslCorrente = vaccinazioneEseguitaRipristinataAcquisizioneCentraleInfoList
                                    vaccinazioneEseguitaScadutaAcquisizioneInfoListUslCorrente = vaccinazioneEseguitaScadutaAcquisizioneCentraleInfoList
                                    reazioneAvversaAcquisizioneInfoListUslCorrente = reazioneAvversaAcquisizioneCentraleInfoList
                                    vaccinazioneEsclusaAcquisizioneInfoListUslCorrente = vaccinazioneEsclusaAcquisizioneCentraleInfoList
                                    visitaAcquisizioneInfoListUslCorrente = visitaAcquisizioneCentraleInfoList
                                End If

                                acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VaccinazioneEsclusaCentraleInfos = vaccinazioneEsclusaAcquisizioneInfoListUslCorrente.ToArray()

                                acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VaccinazioneEseguitaProgrammataCentraleInfos = vaccinazioneEseguitaProgrammataAcquisizioneInfoListUslCorrente.ToArray()
                                acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VaccinazioneEseguitaRecuperataCentraleInfos = vaccinazioneEseguitaRecuperataAcquisizioneInfoListUslCorrente.ToArray()
                                acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VaccinazioneEseguitaRegistrataCentraleInfos = vaccinazioneEseguitaRegistrataAcquisizioneInfoListUslCorrente.ToArray()
                                acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VaccinazioneEseguitaRipristinataCentraleInfos = vaccinazioneEseguitaRipristinataAcquisizioneInfoListUslCorrente.ToArray()

                                acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VaccinazioneEseguitaScadutaCentraleInfos = vaccinazioneEseguitaScadutaAcquisizioneInfoListUslCorrente.ToArray()

                                acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.ReazioneAvversaCentraleInfos = reazioneAvversaAcquisizioneInfoListUslCorrente.ToArray()

                                acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VisitaCentraleInfos = visitaAcquisizioneInfoListUslCorrente.ToArray()

                                'N.B. lo STATO e le NOTE di ACQUISIZIONE dipendono SOLO dai dati vaccinali AGGIUNTI/MODIFICATI e NON da quelli ELIMINATI
                                '     in caso di merge non deve essere aggiornato.  

                                acquisisciDatiVaccinaliCentraliCommand.UpdateStatoAcquisizione =
                                    Not aggiornaDatiVaccinaliCentraliCommand.IsMerge OrElse
                                    String.IsNullOrEmpty(aggiornaDatiVaccinaliCentraliCommand.FlagVisibilitaModificata) OrElse
                                    Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(aggiornaDatiVaccinaliCentraliCommand.FlagVisibilitaModificata)

                            End If

                            acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VaccinazioneEsclusaEliminataCentraleInfos = vaccinazioneEsclusaEliminataAcquisizioneCentraleInfoList.ToArray()
                            acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VaccinazioneEseguitaScadutaEliminataCentraleInfos = vaccinazioneEseguitaScadutaEliminataAcquisizioneCentraleInfoList.ToArray()
                            acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VaccinazioneEseguitaEliminataCentraleInfos = vaccinazioneEseguitaEliminataAcquisizioneCentraleInfoList.ToArray()

                            acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.ReazioneAvversaEliminataCentraleInfos = reazioneAvversaEliminataAcquisizioneCentraleInfoList.ToArray()

                            acquisisciDatiVaccinaliCentraliCommand.DatiVaccinaliCentraliAcquisizioneInfo.VisitaEliminataCentraleInfos = visitaEliminataAcquisizioneCentraleInfoList.ToArray()

                            acquisisciDatiVaccinaliCentraliCommand.LoadDatiVaccinaliCentrali = False

                            acquisisciDatiVaccinaliCentraliCommand.IsMerge = aggiornaDatiVaccinaliCentraliCommand.IsMerge

                            Using bizPaziente As BizPaziente = New BizPaziente(GenericProviderFactory, UslGestitaAllineaSettingsProvider.GetSettings(genericProviderDestinazione), Me.CreateBizContextInfosByCodiceUslGestita(uslDestinazione.Codice), Me.LogOptions)
                                bizPaziente.AcquisisciDatiVaccinaliCentrali(acquisisciDatiVaccinaliCentraliCommand)
                            End Using

                        End If

                    End If

                Next

            End If

            transactionScope.Complete()

        End Using

    End Sub

#Region " Visibilita "

    Public Sub AggiornaVisibilitaDatiVaccinaliCentrali(codicePaziente As Int64, flagVisibilitaDatiVaccinaliCentraleAggiornato As String, flagVisibilitaDatiVaccinaliCentralePrecedente As String)

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, GetReadCommittedTransactionOptions)

            Dim codiceUsl As String = GetCodiceUslByIdApplicazione(ContextInfos.IDApplicazione)

            AggiornaVisibilitaDatiVaccinaliCentrali(GenericProvider.Paziente.GetCodiceAusiliario(codicePaziente), codiceUsl,
                                                    Nothing, Nothing, Nothing, Nothing,
                                                    flagVisibilitaDatiVaccinaliCentraleAggiornato, flagVisibilitaDatiVaccinaliCentralePrecedente)

            transactionScope.Complete()

        End Using

    End Sub

    Public Sub AggiornaVisibilitaDatiVaccinaliCentrali(codicePazienteCentrale As String, flagVisibilitaDatiVaccinaliCentraleAggiornato As String, flagVisibilitaDatiVaccinaliCentralePrecedente As String)

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, GetReadCommittedTransactionOptions)

            AggiornaVisibilitaDatiVaccinaliCentrali(codicePazienteCentrale, Nothing, Nothing, Nothing, Nothing, Nothing,
                                                    flagVisibilitaDatiVaccinaliCentraleAggiornato, flagVisibilitaDatiVaccinaliCentralePrecedente)

            transactionScope.Complete()

        End Using

    End Sub

    Public Sub AggiornaVisibilitaDatiVaccinaliCentrali(codicePaziente As Int64, idVaccinazioniEseguite As Int64(), idVaccinazioniEseguiteScadute As Int64(),
                                                       idVaccinazioniEscluse As Int64(), idVisite As Int64(), flagVisibilitaDatiVaccinaliCentrale As String)

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, GetReadCommittedTransactionOptions)

            Dim codicePazienteCentrale As String = GenericProvider.Paziente.GetCodiceAusiliario(codicePaziente)

            Dim codiceUsl As String = GetCodiceUslByIdApplicazione(ContextInfos.IDApplicazione)

            AggiornaVisibilitaDatiVaccinaliCentrali(codicePazienteCentrale, codiceUsl,
                                                    idVaccinazioniEseguite, idVaccinazioniEseguiteScadute, idVaccinazioniEscluse, idVisite,
                                                    flagVisibilitaDatiVaccinaliCentrale, Nothing)

            transactionScope.Complete()

        End Using

    End Sub

#End Region

#Region " Conflitti "

    Public Sub RisolviConflittoVaccinazioniEseguiteCentrale(risolviConflittoVaccinazioniEseguiteCentraleCommand As RisolviConflittoVaccinazioniEseguiteCentraleCommand)

        Using transactionScope As New System.Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions())

            Dim vaccinazioniEseguiteCentraleDictionary As IDictionary(Of VaccinazioneEseguitaCentrale, List(Of VaccinazioneEseguitaCentrale)) =
                New Dictionary(Of VaccinazioneEseguitaCentrale, List(Of VaccinazioneEseguitaCentrale))

            For Each idVaccinazioniEseguiteCentraleKeyValuePair As KeyValuePair(Of Int64, IEnumerable(Of Int64)) In risolviConflittoVaccinazioniEseguiteCentraleCommand.IdVaccinazioniEseguiteCentralePazienteDictionary

                Dim vaccinazioneEseguitaCentrale As VaccinazioneEseguitaCentrale =
                    Me.GenericProviderCentrale.VaccinazioneEseguitaCentrale.GetVaccinazioneEseguitaCentraleById(idVaccinazioniEseguiteCentraleKeyValuePair.Key)

                Dim vaccinazioneEseguitaCentraleEliminataList As New List(Of VaccinazioneEseguitaCentrale)

                For Each idVaccinazioneEseguitaCentraleEliminata As Int64 In idVaccinazioniEseguiteCentraleKeyValuePair.Value

                    vaccinazioneEseguitaCentraleEliminataList.Add(
                        Me.GenericProviderCentrale.VaccinazioneEseguitaCentrale.GetVaccinazioneEseguitaCentraleById(idVaccinazioneEseguitaCentraleEliminata))
                Next

                vaccinazioniEseguiteCentraleDictionary.Add(vaccinazioneEseguitaCentrale, vaccinazioneEseguitaCentraleEliminataList)

            Next


            Dim vaccinazioneEseguitaCentraleDistribuita As VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita = Nothing

            For Each vaccinazioniEseguiteCentraleKeyValuePair As KeyValuePair(Of VaccinazioneEseguitaCentrale, List(Of VaccinazioneEseguitaCentrale)) In vaccinazioniEseguiteCentraleDictionary

                Dim aggiornaDatiVaccinaliCentraliCommand As AggiornaDatiVaccinaliCentraliCommand

                For Each vaccinazioneEseguitaCentraleEliminata As VaccinazioneEseguitaCentrale In vaccinazioniEseguiteCentraleKeyValuePair.Value

                    Dim isVaccinazioneEseguitaEliminataScaduta As Boolean = (vaccinazioneEseguitaCentraleEliminata.TipoVaccinazioneEseguitaCentrale = Constants.TipoVaccinazioneEseguitaCentrale.Scaduta)

                    Dim reazioneAvversa As ReazioneAvversa = Nothing

                    'Dim dbGenericProviderReazioneAvversaEliminata As DbGenericProvider = Nothing
                    'Dim settingsReazioneAvversaEliminata As Settings.Settings = Nothing
                    'Dim bizContextInfosReazioneAvversaEliminata As BizContextInfos = Nothing

                    ''If vaccinazioneEseguitaCentraleEliminata.IdReazioneAvversa.HasValue AndAlso
                    '   vaccinazioneEseguitaCentraleEliminata.TipoReazioneAvversaCentrale <> Constants.TipoReazioneAvversaCentrale.Eliminata Then

                    'vaccinazioneEseguitaCentraleDistribuita =
                    '        Me.GenericProviderCentrale.VaccinazioneEseguitaCentrale.GetVaccinazioneEseguitaCentraleDistribuitaByIdCentrale(
                    '            vaccinazioneEseguitaCentraleEliminata.Id, vaccinazioneEseguitaCentraleEliminata.CodiceUslReazioneAvversa)

                    'dbGenericProviderReazioneAvversaEliminata = Me.GenericProviderFactory.GetDbGenericProvider(
                    '    Me.GetUslGestitaByCodiceUsl(vaccinazioneEseguitaCentraleEliminata.CodiceUslReazioneAvversa).IDApplicazione, Me.ContextInfos.CodiceAzienda)

                    'settingsReazioneAvversaEliminata = Me.UslGestitaAllineaSettingsProvider.GetSettings(dbGenericProviderReazioneAvversaEliminata)
                    'bizContextInfosReazioneAvversaEliminata = Me.CreateBizContextInfosByCodiceUslGestita(vaccinazioneEseguitaCentraleEliminata.CodiceUslReazioneAvversa)

                    'Using bizVaccinazioniEseguiteReazioneAvversaEliminata As BizVaccinazioniEseguite = New BizVaccinazioniEseguite(Me.GenericProviderFactory, settingsReazioneAvversaEliminata, bizContextInfosReazioneAvversaEliminata, Me.LogOptions)

                    'If isVaccinazioneEseguitaEliminataScaduta Then
                    '    reazioneAvversa = dbGenericProviderReazioneAvversaEliminata.VaccinazioniEseguite.GetReazioneAvversaScadutaById(
                    '        vaccinazioneEseguitaCentraleDistribuita.IdReazioneAvversa)
                    'Else
                    '    reazioneAvversa = dbGenericProviderReazioneAvversaEliminata.VaccinazioniEseguite.GetReazioneAvversaById(
                    '        vaccinazioneEseguitaCentraleDistribuita.IdReazioneAvversa)
                    'End If

                    ''-- DELETE REAZIONE AVVERSA
                    'If isVaccinazioneEseguitaEliminataScaduta Then
                    '    bizVaccinazioniEseguiteReazioneAvversaEliminata.DeleteReazioneAvversaScaduta(reazioneAvversa)
                    'Else
                    '    bizVaccinazioniEseguiteReazioneAvversaEliminata.DeleteReazioneAvversa(reazioneAvversa)
                    'End If

                    ''--LOG DELETE REAZIONE AVVERSA
                    'BizVaccinazioniEseguite.InsertLogAcquisizioneReazioneAvversaCentraleDistribuita(reazioneAvversa,
                    '    vaccinazioneEseguitaCentraleEliminata, DataLogStructure.Operazione.Eliminazione, Enumerators.StatoLogDatiVaccinaliCentrali.Success)

                    'End Using

                    '    If vaccinazioneEseguitaCentraleEliminata.CodiceUslReazioneAvversa <> vaccinazioneEseguitaCentraleEliminata.CodiceUslVaccinazioneEseguita Then

                    '        '-- AGGIORNA DATI VACCINALI CENTRALI --
                    '        aggiornaDatiVaccinaliCentraliCommand = New AggiornaDatiVaccinaliCentraliCommand
                    '        aggiornaDatiVaccinaliCentraliCommand.ReazioneAvversaEliminataEnumerable = New ReazioneAvversa() {reazioneAvversa}
                    '        aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale = vaccinazioneEseguitaCentraleEliminata.CodicePazienteCentrale
                    '        aggiornaDatiVaccinaliCentraliCommand.IsRisoluzioneConflitto = True

                    '        Using bizPazienteReazioneAvversaEliminata As BizPaziente = New BizPaziente(Me.GenericProviderFactory, settingsReazioneAvversaEliminata,
                    '                                                          bizContextInfosReazioneAvversaEliminata, Me.LogOptions)

                    '            bizPazienteReazioneAvversaEliminata.AggiornaDatiVaccinaliCentrali(aggiornaDatiVaccinaliCentraliCommand)

                    '            reazioneAvversa = Nothing

                    '        End Using

                    '    End If

                    'End If

                    Dim dbGenericProviderVaccinazioneEseguitaEliminata As DbGenericProvider = Nothing
                    Dim settingsVaccinazioneEseguitaEliminata As Settings.Settings
                    Dim bizContextInfosVaccinazioneEseguitaEliminata As BizContextInfos

                    ' If vaccinazioneEseguitaCentraleEliminata.CodiceUslVaccinazioneEseguita <> vaccinazioneEseguitaCentraleEliminata.CodiceUslReazioneAvversa Then

                    dbGenericProviderVaccinazioneEseguitaEliminata = Me.GenericProviderFactory.GetDbGenericProvider(
                        GetIdApplicazioneByCodiceUsl(vaccinazioneEseguitaCentraleEliminata.CodiceUslVaccinazioneEseguita),
                        ContextInfos.CodiceAzienda)

                    settingsVaccinazioneEseguitaEliminata = Me.UslGestitaAllineaSettingsProvider.GetSettings(dbGenericProviderVaccinazioneEseguitaEliminata)
                    bizContextInfosVaccinazioneEseguitaEliminata = Me.CreateBizContextInfosByCodiceUslGestita(vaccinazioneEseguitaCentraleEliminata.CodiceUslVaccinazioneEseguita)

                    vaccinazioneEseguitaCentraleDistribuita =
                        Me.GenericProviderCentrale.VaccinazioneEseguitaCentrale.GetVaccinazioneEseguitaCentraleDistribuitaByIdCentrale(
                           vaccinazioneEseguitaCentraleEliminata.Id, vaccinazioneEseguitaCentraleEliminata.CodiceUslVaccinazioneEseguita)

                    'Else

                    '    dbGenericProviderVaccinazioneEseguitaEliminata = dbGenericProviderReazioneAvversaEliminata
                    '    settingsVaccinazioneEseguitaEliminata = settingsReazioneAvversaEliminata
                    '    bizContextInfosVaccinazioneEseguitaEliminata = bizContextInfosReazioneAvversaEliminata

                    'End If

                    Dim salvaVaccinazioneEseguitaCommand As New BizVaccinazioniEseguite.SalvaVaccinazioneEseguitaCommand()

                    Using bizVaccinazioniEseguiteEliminata As BizVaccinazioniEseguite = New BizVaccinazioniEseguite(Me.GenericProviderFactory, settingsVaccinazioneEseguitaEliminata, bizContextInfosVaccinazioneEseguitaEliminata, Me.LogOptions)

                        If vaccinazioneEseguitaCentraleEliminata.IdReazioneAvversa.HasValue AndAlso
                           vaccinazioneEseguitaCentraleEliminata.TipoReazioneAvversaCentrale <> Constants.TipoReazioneAvversaCentrale.Eliminata Then

                            '-- DELETE REAZIONE AVVERSA
                            If isVaccinazioneEseguitaEliminataScaduta Then
                                reazioneAvversa = dbGenericProviderVaccinazioneEseguitaEliminata.VaccinazioniEseguite.GetReazioneAvversaScadutaById(vaccinazioneEseguitaCentraleDistribuita.IdReazioneAvversa)
                                bizVaccinazioniEseguiteEliminata.DeleteReazioneAvversaScaduta(reazioneAvversa)
                            Else
                                reazioneAvversa = dbGenericProviderVaccinazioneEseguitaEliminata.VaccinazioniEseguite.GetReazioneAvversaById(vaccinazioneEseguitaCentraleDistribuita.IdReazioneAvversa)
                                bizVaccinazioniEseguiteEliminata.DeleteReazioneAvversa(reazioneAvversa)
                            End If

                            '--LOG DELETE REAZIONE AVVERSA
                            bizVaccinazioniEseguiteEliminata.InsertLogAcquisizioneReazioneAvversaCentraleDistribuita(reazioneAvversa,
                                vaccinazioneEseguitaCentraleEliminata, DataLogStructure.Operazione.Eliminazione, Enumerators.StatoLogDatiVaccinaliCentrali.Success)

                        End If

                        salvaVaccinazioneEseguitaCommand.Operation = SalvaCommandOperation.Delete
                        salvaVaccinazioneEseguitaCommand.IsVaccinazioneEseguitaScaduta = isVaccinazioneEseguitaEliminataScaduta

                        If salvaVaccinazioneEseguitaCommand.IsVaccinazioneEseguitaScaduta Then
                            salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita = dbGenericProviderVaccinazioneEseguitaEliminata.VaccinazioniEseguite.GetVaccinazioneEseguitaScadutaById(vaccinazioneEseguitaCentraleDistribuita.IdVaccinazioneEseguita)
                        Else
                            salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita = dbGenericProviderVaccinazioneEseguitaEliminata.VaccinazioniEseguite.GetVaccinazioneEseguitaById(vaccinazioneEseguitaCentraleDistribuita.IdVaccinazioneEseguita)
                        End If

                        '-- DELETE VACCINAZIONE ESEGUITA
                        bizVaccinazioniEseguiteEliminata.SalvaVaccinazioneEseguita(salvaVaccinazioneEseguitaCommand)

                        '--LOG DELETE VACCINAZIONE ESEGUITA
                        bizVaccinazioniEseguiteEliminata.InsertLogAcquisizioneVaccinazioneEseguitaCentraleDistribuita(salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita,
                            vaccinazioneEseguitaCentraleEliminata, DataLogStructure.Operazione.Eliminazione, Enumerators.StatoLogDatiVaccinaliCentrali.Success, False)

                    End Using

                    '-- AGGIORNA VACCINAZIONE ESEGUITA - REAZIONE AVVERSA ELIMINATA CENTRALE --
                    aggiornaDatiVaccinaliCentraliCommand = New AggiornaDatiVaccinaliCentraliCommand
                    aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale = vaccinazioneEseguitaCentraleEliminata.CodicePazienteCentrale
                    aggiornaDatiVaccinaliCentraliCommand.IsRisoluzioneConflitto = True

                    If salvaVaccinazioneEseguitaCommand.IsVaccinazioneEseguitaScaduta Then
                        aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEseguitaScadutaEliminataEnumerable = New VaccinazioneEseguita() {salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita}
                    Else
                        aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEseguitaEliminataEnumerable = New VaccinazioneEseguita() {salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita}
                    End If

                    If Not reazioneAvversa Is Nothing Then
                        aggiornaDatiVaccinaliCentraliCommand.ReazioneAvversaEliminataEnumerable = New ReazioneAvversa() {reazioneAvversa}
                    End If

                    Using bizPaziente As BizPaziente = New BizPaziente(Me.GenericProviderFactory, settingsVaccinazioneEseguitaEliminata, bizContextInfosVaccinazioneEseguitaEliminata, Me.LogOptions)
                        bizPaziente.AggiornaDatiVaccinaliCentrali(aggiornaDatiVaccinaliCentraliCommand)
                    End Using

                Next

                Dim dbGenericProviderVaccinazioneEseguita As DbGenericProvider = GenericProviderFactory.GetDbGenericProvider(
                    GetIdApplicazioneByCodiceUsl(vaccinazioniEseguiteCentraleKeyValuePair.Key.CodiceUslUltimaOperazioneVaccinazioneEseguita),
                    ContextInfos.CodiceAzienda)

                Dim settingsVaccinazioneEseguita As Settings.Settings = Me.UslGestitaAllineaSettingsProvider.GetSettings(dbGenericProviderVaccinazioneEseguita)
                Dim bizContextInfosVaccinazioneEseguita As BizContextInfos = Me.CreateBizContextInfosByCodiceUslGestita(vaccinazioniEseguiteCentraleKeyValuePair.Key.CodiceUslUltimaOperazioneVaccinazioneEseguita)

                Dim vaccinazioneEseguita As VaccinazioneEseguita = Nothing
                Dim isVaccinazioneEseguitaScaduta As Boolean = False

                Dim aggiornaVisibilitaVaccinazioniEseguiteBizResult As BizResult = Nothing

                vaccinazioneEseguitaCentraleDistribuita =
                    Me.GenericProviderCentrale.VaccinazioneEseguitaCentrale.GetVaccinazioneEseguitaCentraleDistribuitaByIdCentrale(
                        vaccinazioniEseguiteCentraleKeyValuePair.Key.Id, vaccinazioniEseguiteCentraleKeyValuePair.Key.CodiceUslUltimaOperazioneVaccinazioneEseguita)

                Using bizVaccinazioniEseguite As BizVaccinazioniEseguite = New BizVaccinazioniEseguite(Me.GenericProviderFactory, settingsVaccinazioneEseguita, bizContextInfosVaccinazioneEseguita, Me.LogOptions)

                    isVaccinazioneEseguitaScaduta =
                        (vaccinazioniEseguiteCentraleKeyValuePair.Key.TipoVaccinazioneEseguitaCentrale = Constants.TipoVaccinazioneEseguitaCentrale.Scaduta)

                    If isVaccinazioneEseguitaScaduta Then
                        vaccinazioneEseguita = dbGenericProviderVaccinazioneEseguita.VaccinazioniEseguite.GetVaccinazioneEseguitaScadutaById(vaccinazioneEseguitaCentraleDistribuita.IdVaccinazioneEseguita)
                    Else
                        vaccinazioneEseguita = dbGenericProviderVaccinazioneEseguita.VaccinazioniEseguite.GetVaccinazioneEseguitaById(vaccinazioneEseguitaCentraleDistribuita.IdVaccinazioneEseguita)
                    End If

                    '-- UPDATE FLAG VISIBILITA VACCINAZIONE ESEGUITA
                    If vaccinazioneEseguita.ves_flag_visibilita_vac_centrale <> Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente Then

                        Dim vaccinazioneEseguitaOriginale As VaccinazioneEseguita = vaccinazioneEseguita.Clone()

                        ' TODO [DATI VAC CENTRALE]: CheckConflittoVisibilitaCentraleIfNeeded ??
                        aggiornaVisibilitaVaccinazioniEseguiteBizResult = bizVaccinazioniEseguite.AggiornaVisibilitaVaccinazioneEseguita(
                            Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente, vaccinazioneEseguita, vaccinazioniEseguiteCentraleKeyValuePair.Key,
                            isVaccinazioneEseguitaScaduta)

                        If Not aggiornaVisibilitaVaccinazioniEseguiteBizResult.Success Then
                            vaccinazioneEseguita = vaccinazioneEseguitaOriginale
                        End If

                        '-- LOG UPDATE FLAG VISIBILITA VACCINAZIONE ESEGUITA
                        bizVaccinazioniEseguite.InsertLogAcquisizioneVaccinazioneEseguitaCentraleDistribuita(vaccinazioneEseguita,
                            vaccinazioniEseguiteCentraleKeyValuePair.Key, DataLogStructure.Operazione.Modifica,
                            IIf(aggiornaVisibilitaVaccinazioniEseguiteBizResult.Success, Enumerators.StatoLogDatiVaccinaliCentrali.Success, Enumerators.StatoLogDatiVaccinaliCentrali.Error),
                            True)

                    End If

                End Using

                If aggiornaVisibilitaVaccinazioniEseguiteBizResult Is Nothing OrElse aggiornaVisibilitaVaccinazioniEseguiteBizResult.Success Then

                    Dim dbGenericProviderReazioneAvversa As DbGenericProvider = Nothing

                    Dim reazioneAvversa As ReazioneAvversa = Nothing

                    If vaccinazioniEseguiteCentraleKeyValuePair.Key.IdReazioneAvversa.HasValue AndAlso
                             vaccinazioniEseguiteCentraleKeyValuePair.Key.TipoReazioneAvversaCentrale <> Constants.TipoReazioneAvversaCentrale.Eliminata Then

                        If vaccinazioniEseguiteCentraleKeyValuePair.Key.CodiceUslUltimaOperazioneReazioneAvversa <>
                            vaccinazioniEseguiteCentraleKeyValuePair.Key.CodiceUslUltimaOperazioneReazioneAvversa Then

                            dbGenericProviderReazioneAvversa = GenericProviderFactory.GetDbGenericProvider(
                                GetIdApplicazioneByCodiceUsl(vaccinazioniEseguiteCentraleKeyValuePair.Key.CodiceUslUltimaOperazioneReazioneAvversa),
                                ContextInfos.CodiceAzienda)

                            vaccinazioneEseguitaCentraleDistribuita =
                                Me.GenericProviderCentrale.VaccinazioneEseguitaCentrale.GetVaccinazioneEseguitaCentraleDistribuitaByIdCentrale(
                                    vaccinazioniEseguiteCentraleKeyValuePair.Key.Id, vaccinazioniEseguiteCentraleKeyValuePair.Key.CodiceUslUltimaOperazioneReazioneAvversa)
                        Else

                            dbGenericProviderReazioneAvversa = dbGenericProviderVaccinazioneEseguita

                        End If

                        If isVaccinazioneEseguitaScaduta Then

                            reazioneAvversa = dbGenericProviderReazioneAvversa.VaccinazioniEseguite.GetReazioneAvversaScadutaById(
                                                  vaccinazioneEseguitaCentraleDistribuita.IdReazioneAvversa)
                        Else

                            reazioneAvversa = dbGenericProviderReazioneAvversa.VaccinazioniEseguite.GetReazioneAvversaById(
                                             vaccinazioneEseguitaCentraleDistribuita.IdReazioneAvversa)
                        End If

                    End If

                    '-- AGGIORNA VACCINAZIONE ESEGUITA / REAZIONE AVVERSA CENTRALE --
                    aggiornaDatiVaccinaliCentraliCommand = New AggiornaDatiVaccinaliCentraliCommand()
                    aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale = vaccinazioniEseguiteCentraleKeyValuePair.Key.CodicePazienteCentrale
                    aggiornaDatiVaccinaliCentraliCommand.IsRisoluzioneConflitto = True

                    If isVaccinazioneEseguitaScaduta Then
                        aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEseguitaScadutaEnumerable = {vaccinazioneEseguita}
                    Else
                        aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEseguitaEnumerable = {vaccinazioneEseguita}
                    End If

                    If Not reazioneAvversa Is Nothing Then

                        If vaccinazioniEseguiteCentraleKeyValuePair.Key.CodiceUslUltimaOperazioneReazioneAvversa =
                            vaccinazioniEseguiteCentraleKeyValuePair.Key.CodiceUslUltimaOperazioneReazioneAvversa Then

                            aggiornaDatiVaccinaliCentraliCommand.ReazioneAvversaEnumerable = New ReazioneAvversa() {reazioneAvversa}

                            reazioneAvversa = Nothing

                        End If

                    End If

                    Using bizPaziente As BizPaziente = New BizPaziente(Me.GenericProviderFactory, settingsVaccinazioneEseguita, bizContextInfosVaccinazioneEseguita, Me.LogOptions)
                        bizPaziente.AggiornaDatiVaccinaliCentrali(aggiornaDatiVaccinaliCentraliCommand)
                    End Using

                    If Not reazioneAvversa Is Nothing Then

                        '-- AGGIORNA REAZIONE AVVERSA CENTRALI --
                        aggiornaDatiVaccinaliCentraliCommand = New AggiornaDatiVaccinaliCentraliCommand()
                        aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale = vaccinazioniEseguiteCentraleKeyValuePair.Key.CodicePazienteCentrale
                        aggiornaDatiVaccinaliCentraliCommand.IsRisoluzioneConflitto = True

                        aggiornaDatiVaccinaliCentraliCommand.ReazioneAvversaEnumerable = New ReazioneAvversa() {reazioneAvversa}

                        Dim settingsReazioneAvversa As Settings.Settings = Me.UslGestitaAllineaSettingsProvider.GetSettings(dbGenericProviderReazioneAvversa)
                        Dim bizContextInfosReazioneAvversa As BizContextInfos = Me.CreateBizContextInfosByCodiceUslGestita(vaccinazioniEseguiteCentraleKeyValuePair.Key.CodiceUslUltimaOperazioneReazioneAvversa)

                        Using bizPaziente As BizPaziente = New BizPaziente(Me.GenericProviderFactory, settingsReazioneAvversa, bizContextInfosReazioneAvversa, Me.LogOptions)
                            bizPaziente.AggiornaDatiVaccinaliCentrali(aggiornaDatiVaccinaliCentraliCommand)
                        End Using

                    End If

                End If

            Next

            transactionScope.Complete()

        End Using

    End Sub

    'Public Sub RisolviConflittiVaccinazioniEscluseCentrale(risolviConflittiVaccinazioniEscluseCentraleCommand As RisolviConflittiVaccinazioniEscluseCentraleCommand)

    '    Using transactionScope As New System.Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions())

    '        Dim vaccinazioniEscluseCentraleDictionary As IDictionary(Of VaccinazioneEsclusaCentrale, List(Of VaccinazioneEsclusaCentrale)) =
    '            New Dictionary(Of VaccinazioneEsclusaCentrale, List(Of VaccinazioneEsclusaCentrale))

    '        For Each idVaccinazioniEscluseCentraleKeyValuePair As KeyValuePair(Of Int64, IEnumerable(Of Int64)) In risolviConflittiVaccinazioniEscluseCentraleCommand.IdVaccinazioniEscluseCentraleDictionary

    '            Dim vaccinazioneEsclusaCentrale As VaccinazioneEsclusaCentrale =
    '                Me.GenericProviderCentrale.VaccinazioneEsclusaCentrale.GetVaccinazioneEsclusaCentraleById(idVaccinazioniEscluseCentraleKeyValuePair.Key)

    '            Dim vaccinazioneEsclusaCentraleEliminataList As New List(Of VaccinazioneEsclusaCentrale)

    '            For Each idVaccinazioneEsclusaCentraleEliminata As Int64 In idVaccinazioniEscluseCentraleKeyValuePair.Value

    '                vaccinazioneEsclusaCentraleEliminataList.Add(
    '                    Me.GenericProviderCentrale.VaccinazioneEsclusaCentrale.GetVaccinazioneEsclusaCentraleById(idVaccinazioneEsclusaCentraleEliminata))
    '            Next

    '            vaccinazioniEscluseCentraleDictionary.Add(vaccinazioneEsclusaCentrale, vaccinazioneEsclusaCentraleEliminataList)

    '        Next

    '        For Each vaccinazioniEscluseCentraleKeyValuePair As KeyValuePair(Of VaccinazioneEsclusaCentrale, List(Of VaccinazioneEsclusaCentrale)) In vaccinazioniEscluseCentraleDictionary

    '            Dim uslVaccinazioneEsclusa As Usl = Me.UslGestite.First(Function(u) u.Codice = vaccinazioniEscluseCentraleKeyValuePair.Key.CodiceUslVaccinazioneEsclusa)

    '            For Each vaccinazioneEsclusaCentraleEliminata As VaccinazioneEsclusaCentrale In vaccinazioniEscluseCentraleKeyValuePair.Value

    '                Dim dbGenericProviderEliminata As DbGenericProvider = Me.GenericProviderFactory.GetDbGenericProvider(
    '                    uslVaccinazioneEsclusa.IDApplicazione, Me.ContextInfos.CodiceAzienda)

    '                Dim settingsEliminata As Settings.Settings = Me.UslGestitaAllineaSettingsProvider.GetSettings(dbGenericProviderEliminata)
    '                Dim bizContextInfosEliminata As BizContextInfos = Me.CreateBizContextInfosByCodiceUslGestita(uslVaccinazioneEsclusa.Codice)

    '                Dim vaccinazioneEsclusaEliminata As VaccinazioneEsclusa =
    '                      dbGenericProviderEliminata.VaccinazioniEscluse.GetVaccinazioneEsclusaById(vaccinazioneEsclusaCentraleEliminata.IdVaccinazioneEsclusa)

    '                Using bizVaccinazioniEscluse As BizVaccinazioniEscluse = New BizVaccinazioniEscluse(Me.GenericProviderFactory, settingsEliminata, bizContextInfosEliminata, Me.LogOptions)

    '                    '-- DELETE VACCINAZIONE ESCLUSA
    '                    Dim salvaVaccinazioneEsclusaCommand As New BizVaccinazioniEscluse.SalvaVaccinazioneEsclusaCommand
    '                    salvaVaccinazioneEsclusaCommand.Operation = SalvaCommandOperation.Delete
    '                    salvaVaccinazioneEsclusaCommand.VaccinazioneEsclusa = vaccinazioneEsclusaEliminata

    '                    bizVaccinazioniEscluse.SalvaVaccinazioneEsclusa(salvaVaccinazioneEsclusaCommand)

    '                    '-- LOG DELETE VACCINAZIONE ESCLUSA
    '                    bizVaccinazioniEscluse.InsertLogAcquisizioneVaccinazioneEsclusaCentraleDistribuita(vaccinazioneEsclusaEliminata,
    '                        vaccinazioneEsclusaCentraleEliminata, Log.DataLogStructure.Operazione.Eliminazione,
    '                        Enumerators.StatoLogDatiVaccinaliCentrali.Success)

    '                End Using


    '                Dim aggiornaDatiVaccinaliCentraliCommand As New AggiornaDatiVaccinaliCentraliCommand
    '                aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEsclusaEliminataEnumerable = {vaccinazioneEsclusaEliminata}
    '                aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale = vaccinazioneEsclusaCentraleEliminata.CodicePazienteCentrale
    '                aggiornaDatiVaccinaliCentraliCommand.IsRisoluzioneConflitto = True

    '                Using bizPaziente As BizPaziente = New BizPaziente(Me.GenericProviderFactory, settingsEliminata, bizContextInfosEliminata, Me.LogOptions)
    '                    bizPaziente.AggiornaDatiVaccinaliCentrali(aggiornaDatiVaccinaliCentraliCommand)
    '                End Using

    '            Next


    '            Dim aggiornaVisibilitaVaccinazioneEsclusaBizResult As BizResult = Nothing

    '            Dim dbGenericProviderVaccinazioneEsclusa As DbGenericProvider = Me.GenericProviderFactory.GetDbGenericProvider(
    '                Me.GetUslGestitaByCodiceUsl(vaccinazioniEscluseCentraleKeyValuePair.Key.CodiceUslVaccinazioneEsclusa).IDApplicazione, Me.ContextInfos.CodiceAzienda)

    '            Dim settingsVaccinazioneEsclusa As Settings.Settings = Me.UslGestitaAllineaSettingsProvider.GetSettings(dbGenericProviderVaccinazioneEsclusa)
    '            Dim bizContextInfosVaccinazioneEsclusa As BizContextInfos = Me.CreateBizContextInfosByCodiceUslGestita(vaccinazioniEscluseCentraleKeyValuePair.Key.CodiceUslVaccinazioneEsclusa)

    '            Dim vaccinazioneEsclusa As VaccinazioneEsclusa =
    '                dbGenericProviderVaccinazioneEsclusa.VaccinazioniEscluse.GetVaccinazioneEsclusaById(vaccinazioniEscluseCentraleKeyValuePair.Key.IdVaccinazioneEsclusa)

    '            If vaccinazioneEsclusa.FlagVisibilita <> Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente Then

    '                Using bizVaccinazioniEscluse As BizVaccinazioniEscluse = New BizVaccinazioniEscluse(Me.GenericProviderFactory, settingsVaccinazioneEsclusa, bizContextInfosVaccinazioneEsclusa, Me.LogOptions)

    '                    Dim vaccinazioneEsclusaOriginale As VaccinazioneEsclusa = vaccinazioneEsclusa.Clone()

    '                    '-- UPDATE FLAG VISIBILITA VACCINAZIONE ESCLUSA
    '                    aggiornaVisibilitaVaccinazioneEsclusaBizResult = bizVaccinazioniEscluse.AggiornaVisibilitaVaccinazioneEsclusa(
    '                        Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente, vaccinazioneEsclusa, vaccinazioniEscluseCentraleKeyValuePair.Key)

    '                    If Not aggiornaVisibilitaVaccinazioneEsclusaBizResult.Success Then
    '                        vaccinazioneEsclusa = vaccinazioneEsclusaOriginale
    '                    End If

    '                    '-- LOG UPDATE FLAG VISIBILITA VACCINAZIONE ESCLUSA
    '                    bizVaccinazioniEscluse.InsertLogAcquisizioneVaccinazioneEsclusaCentraleDistribuita(vaccinazioneEsclusa,
    '                        vaccinazioniEscluseCentraleKeyValuePair.Key, Log.DataLogStructure.Operazione.Eliminazione,
    '                        Enumerators.StatoLogDatiVaccinaliCentrali.Success)

    '                End Using

    '            End If

    '            If aggiornaVisibilitaVaccinazioneEsclusaBizResult Is Nothing OrElse aggiornaVisibilitaVaccinazioneEsclusaBizResult.Success Then

    '                Dim aggiornaDatiVaccinaliCentraliCommand As New AggiornaDatiVaccinaliCentraliCommand
    '                aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEsclusaEnumerable = {vaccinazioneEsclusa}
    '                aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale = vaccinazioniEscluseCentraleKeyValuePair.Key.CodicePazienteCentrale
    '                aggiornaDatiVaccinaliCentraliCommand.IsRisoluzioneConflitto = True

    '                Using bizPaziente As BizPaziente = New BizPaziente(Me.GenericProviderFactory, settingsVaccinazioneEsclusa, bizContextInfosVaccinazioneEsclusa, Me.LogOptions)
    '                    bizPaziente.AggiornaDatiVaccinaliCentrali(aggiornaDatiVaccinaliCentraliCommand)
    '                End Using

    '            End If

    '        Next

    '        transactionScope.Complete()

    '    End Using

    'End Sub

    Public Sub RisolviConflittoVisiteCentrale(risolviConflittoVisiteCentraleCommand As RisolviConflittoVisiteCentraleCommand)

        Using transactionScope As New System.Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions())

            Dim visiteCentraleDictionary As IDictionary(Of VisitaCentrale, List(Of VisitaCentrale)) =
                New Dictionary(Of VisitaCentrale, List(Of VisitaCentrale))

            For Each idVaccinazioniEscluseCentraleKeyValuePair As KeyValuePair(Of Int64, IEnumerable(Of Int64)) In risolviConflittoVisiteCentraleCommand.IdVisiteCentraleDictionary

                Dim visitaCentrale As VisitaCentrale =
                    Me.GenericProviderCentrale.VisitaCentrale.GetVisitaCentraleById(idVaccinazioniEscluseCentraleKeyValuePair.Key)

                Dim visitaCentraleEliminataList As New List(Of VisitaCentrale)

                For Each idVisitaCentraleEliminata As Int64 In idVaccinazioniEscluseCentraleKeyValuePair.Value

                    visitaCentraleEliminataList.Add(
                        Me.GenericProviderCentrale.VisitaCentrale.GetVisitaCentraleById(idVisitaCentraleEliminata))
                Next

                visiteCentraleDictionary.Add(visitaCentrale, visitaCentraleEliminataList)

            Next

            For Each visiteCentraleKeyValuePair As KeyValuePair(Of VisitaCentrale, List(Of VisitaCentrale)) In visiteCentraleDictionary

                Dim codiceUsl As String = visiteCentraleKeyValuePair.Key.CodiceUslVisita

                ' [Unificazione Ulss]: nel nuovo questo metodo non si usa più, quindi qui lo lascio com'era => OK
                Dim uslOrigine As Usl = UslGestite.First(Function(u) u.Codice = codiceUsl)

                For Each visitaCentraleEliminata As VisitaCentrale In visiteCentraleKeyValuePair.Value

                    Dim dbGenericProviderEliminata As DbGenericProvider = GenericProviderFactory.GetDbGenericProvider(
                        GetUslGestitaByCodiceUsl(visitaCentraleEliminata.CodiceUslVisita).IDApplicazione, ContextInfos.CodiceAzienda)

                    Dim visitaEliminata As Visita = dbGenericProviderEliminata.Visite.GetVisitaById(visitaCentraleEliminata.IdVisita)

                    Dim eliminaVisitaAndOsservazioniResult As BizVisite.EliminaVisitaAndOsservazioniResult = Nothing

                    Dim settingsVisitaEliminata As Settings.Settings = Me.UslGestitaAllineaSettingsProvider.GetSettings(dbGenericProviderEliminata)
                    Dim bizContextInfosVisitaEliminata As BizContextInfos = Me.CreateBizContextInfosByCodiceUslGestita(visitaCentraleEliminata.CodiceUslVisita)

                    Using bizVisite As BizVisite = New BizVisite(Me.GenericProviderFactory, settingsVisitaEliminata, bizContextInfosVisitaEliminata, Me.LogOptions)

                        '-- DELETE VISITA/OSSERVAZIONI
                        eliminaVisitaAndOsservazioniResult = bizVisite.DeleteVisitaAndOsservazioni(visitaEliminata)

                        '-- LOG DELETE VISITA
                        bizVisite.InsertLogAcquisizioneVisitaCentraleDistribuita(visitaEliminata,
                                                                                 visitaCentraleEliminata,
                                                                                 Log.DataLogStructure.Operazione.Eliminazione,
                                                                                 Enumerators.StatoLogDatiVaccinaliCentrali.Success,
                                                                                 False)

                    End Using

                    '-- AGGIORNA DATI VACCINALI CENTRALI
                    Dim aggiornaDatiVaccinaliCentraliCommand As New AggiornaDatiVaccinaliCentraliCommand
                    aggiornaDatiVaccinaliCentraliCommand.VisitaEliminataEnumerable = {visitaEliminata}
                    aggiornaDatiVaccinaliCentraliCommand.OsservazioneEliminataEnumerable = eliminaVisitaAndOsservazioniResult.OsservazioniEliminate.AsEnumerable()
                    aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale = visiteCentraleKeyValuePair.Key.CodicePazienteCentrale
                    aggiornaDatiVaccinaliCentraliCommand.IsRisoluzioneConflitto = True

                    Using bizPaziente As BizPaziente = New BizPaziente(Me.GenericProviderFactory, settingsVisitaEliminata, bizContextInfosVisitaEliminata, Me.LogOptions)
                        bizPaziente.AggiornaDatiVaccinaliCentrali(aggiornaDatiVaccinaliCentraliCommand)
                    End Using

                Next

                Dim aggiornaVisibilitaVisitaBizResult As BizResult = Nothing

                Dim dbGenericProviderVisita As DbGenericProvider = Me.GenericProviderFactory.GetDbGenericProvider(
                    Me.GetUslGestitaByCodiceUsl(visiteCentraleKeyValuePair.Key.CodiceUslVisita).IDApplicazione, Me.ContextInfos.CodiceAzienda)

                Dim visita As Visita = dbGenericProviderVisita.Visite.GetVisitaById(visiteCentraleKeyValuePair.Key.IdVisita)

                Dim settingsVisita As Settings.Settings = Me.UslGestitaAllineaSettingsProvider.GetSettings(dbGenericProviderVisita)
                Dim bizContextInfosVisita As BizContextInfos = Me.CreateBizContextInfosByCodiceUslGestita(visiteCentraleKeyValuePair.Key.CodiceUslVisita)

                Using bizVisite As BizVisite = New BizVisite(Me.GenericProviderFactory, settingsVisita, bizContextInfosVisita, Me.LogOptions)

                    If visita.FlagVisibilitaDatiVaccinaliCentrale <> Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente Then

                        '-- UPDATE FLAG VISIBILITA VISITA
                        Dim visitaOriginale As Visita = visita.Clone()

                        aggiornaVisibilitaVisitaBizResult = bizVisite.AggiornaVisibilitaVisita(Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente, visita, visiteCentraleKeyValuePair.Key, "Risoluzione conflitto visita")

                        If Not aggiornaVisibilitaVisitaBizResult.Success Then
                            visita = visitaOriginale
                        End If

                        '-- LOG UPDATE FLAG VISIBILITA VISITA
                        bizVisite.InsertLogAcquisizioneVisitaCentraleDistribuita(visita,
                                                                                 visiteCentraleKeyValuePair.Key,
                                                                                 Log.DataLogStructure.Operazione.Eliminazione,
                                                                                 Enumerators.StatoLogDatiVaccinaliCentrali.Success,
                                                                                 True)

                    End If

                End Using

                If aggiornaVisibilitaVisitaBizResult Is Nothing OrElse aggiornaVisibilitaVisitaBizResult.Success Then

                    Dim aggiornaDatiVaccinaliCentraliCommand As New AggiornaDatiVaccinaliCentraliCommand()

                    aggiornaDatiVaccinaliCentraliCommand.VisitaEnumerable = {visita}
                    aggiornaDatiVaccinaliCentraliCommand.OsservazioneEnumerable = dbGenericProviderVisita.Visite.GetOsservazioniByVisita(visita.IdVisita).AsEnumerable()
                    aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale = visiteCentraleKeyValuePair.Key.CodicePazienteCentrale
                    aggiornaDatiVaccinaliCentraliCommand.IsRisoluzioneConflitto = True

                    Using bizPaziente As BizPaziente = New BizPaziente(Me.GenericProviderFactory, settingsVisita, bizContextInfosVisita, Me.LogOptions)
                        bizPaziente.AggiornaDatiVaccinaliCentrali(aggiornaDatiVaccinaliCentraliCommand)
                    End Using

                End If

            Next

            transactionScope.Complete()

        End Using

    End Sub

#End Region

#End Region

#Region " Private "

    Private Function LoadDatiVaccinaliCentrali(loadDatiVaccinaliCentraliCommand As LoadDatiVaccinaliCentraliCommand) As LoadDatiVaccinaliCentraliResult

        Dim codiceUslCorrente As String = GetCodiceUslByIdApplicazione(ContextInfos.IDApplicazione)

        Dim loadDatiVaccinaliCentraliResult As New LoadDatiVaccinaliCentraliResult()

        If (Not loadDatiVaccinaliCentraliCommand.VaccinazioniEseguiteCentrale Is Nothing AndAlso loadDatiVaccinaliCentraliCommand.VaccinazioniEseguiteCentrale.Length > 0) OrElse
           (Not loadDatiVaccinaliCentraliCommand.VaccinazioniEscluseCentrale Is Nothing AndAlso loadDatiVaccinaliCentraliCommand.VaccinazioniEscluseCentrale.Length > 0) OrElse
           (Not loadDatiVaccinaliCentraliCommand.VisiteCentrale Is Nothing AndAlso loadDatiVaccinaliCentraliCommand.VisiteCentrale.Length > 0) Then

            '-- VACCINAZIONI ESEGUITE / REAZIONI AVVERSE
            Dim vaccinazioneEseguitaProgrammataAcquisizioneInfoList As New List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)()
            Dim vaccinazioneEseguitaRecuperataAcquisizioneInfoList As New List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)()
            Dim vaccinazioneEseguitaRegistrataAcquisizioneInfoList As New List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)()
            Dim vaccinazioneEseguitaRipristinataAcquisizioneInfoList As New List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)()
            Dim vaccinazioneEseguitaScadutaAcquisizioneInfoList As New List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)()
            Dim vaccinazioneEseguitaScadutaEliminataAcquisizioneInfoList As New List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)()
            Dim vaccinazioneEseguitaEliminataAcquisizioneInfoList As New List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)()

            Dim reazioneAvversaAcquisizioneCentraleInfoList As New List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)()
            Dim reazioneAvversaEliminataAcquisizioneCentraleInfoList As New List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)()

            If Not loadDatiVaccinaliCentraliCommand.VaccinazioniEseguiteCentrale Is Nothing Then

                For Each vaccinazioneEseguitaCentrale As VaccinazioneEseguitaCentrale In loadDatiVaccinaliCentraliCommand.VaccinazioniEseguiteCentrale

                    '-- REAZIONI AVVERSE
                    Dim reazioneAvversa As ReazioneAvversa = Nothing

                    If loadDatiVaccinaliCentraliCommand.LoadReazioneAvverse AndAlso
                       vaccinazioneEseguitaCentrale.IdReazioneAvversa.HasValue AndAlso
                       (loadDatiVaccinaliCentraliCommand.LoadDatiVaccinaliEliminati OrElse
                        vaccinazioneEseguitaCentrale.TipoReazioneAvversaCentrale <> Constants.TipoReazioneAvversaCentrale.Eliminata) AndAlso
                       (loadDatiVaccinaliCentraliCommand.LoadDatiVaccinaliUslGestitaCorrente OrElse
                        vaccinazioneEseguitaCentrale.CodiceUslUltimaOperazioneReazioneAvversa <> codiceUslCorrente) Then

                        Dim vaccinazioneEseguitaCentraleDistribuitaUslUltimaOperazioneReazioneAvversa As VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita =
                            GenericProviderCentrale.VaccinazioneEseguitaCentrale.GetVaccinazioneEseguitaCentraleDistribuitaByIdCentrale(
                            vaccinazioneEseguitaCentrale.Id, vaccinazioneEseguitaCentrale.CodiceUslUltimaOperazioneReazioneAvversa)

                        Dim dbGenericProvider As DbGenericProvider = GenericProviderFactory.GetDbGenericProvider(
                             GetIdApplicazioneByCodiceUsl(vaccinazioneEseguitaCentrale.CodiceUslUltimaOperazioneReazioneAvversa), ContextInfos.CodiceAzienda)

                        Dim reazioneAvversaCentraleInfo As New BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo()
                        reazioneAvversaCentraleInfo.VaccinazioneEseguitaCentrale = vaccinazioneEseguitaCentrale
                        reazioneAvversaCentraleInfo.ReazioneAvversa = reazioneAvversa

                        Select Case vaccinazioneEseguitaCentrale.TipoReazioneAvversaCentrale

                            Case Constants.TipoReazioneAvversaCentrale.Nessuno

                                If vaccinazioneEseguitaCentrale.TipoVaccinazioneEseguitaCentrale = Constants.TipoVaccinazioneEseguitaCentrale.Scaduta Then
                                    reazioneAvversa = dbGenericProvider.VaccinazioniEseguite.GetReazioneAvversaScadutaById(vaccinazioneEseguitaCentraleDistribuitaUslUltimaOperazioneReazioneAvversa.IdReazioneAvversa)
                                Else
                                    reazioneAvversa = dbGenericProvider.VaccinazioniEseguite.GetReazioneAvversaById(vaccinazioneEseguitaCentraleDistribuitaUslUltimaOperazioneReazioneAvversa.IdReazioneAvversa)
                                End If

                                If vaccinazioneEseguitaCentrale.CodiceUslUltimaOperazioneReazioneAvversa <>
                                    vaccinazioneEseguitaCentrale.CodiceUslUltimaOperazioneVaccinazioneEseguita Then

                                    reazioneAvversaCentraleInfo.ReazioneAvversa = reazioneAvversa

                                    reazioneAvversaAcquisizioneCentraleInfoList.Add(reazioneAvversaCentraleInfo)

                                    reazioneAvversa = Nothing

                                End If

                            Case Constants.TipoReazioneAvversaCentrale.Eliminata

                                Dim reazioneAvversaEliminataOrigine As ReazioneAvversa = Nothing

                                If vaccinazioneEseguitaCentrale.TipoVaccinazioneEseguitaCentrale = Constants.TipoVaccinazioneEseguitaCentrale.ScadutaEliminata Then
                                    reazioneAvversa = dbGenericProvider.VaccinazioniEseguite.GetReazioneAvversaScadutaEliminataById(vaccinazioneEseguitaCentraleDistribuitaUslUltimaOperazioneReazioneAvversa.IdReazioneAvversa)
                                Else
                                    reazioneAvversa = dbGenericProvider.VaccinazioniEseguite.GetReazioneAvversaEliminataById(vaccinazioneEseguitaCentraleDistribuitaUslUltimaOperazioneReazioneAvversa.IdReazioneAvversa)
                                End If

                                If vaccinazioneEseguitaCentrale.CodiceUslUltimaOperazioneReazioneAvversa <>
                                    vaccinazioneEseguitaCentrale.CodiceUslUltimaOperazioneVaccinazioneEseguita Then

                                    reazioneAvversaCentraleInfo.ReazioneAvversa = reazioneAvversa

                                    reazioneAvversaEliminataAcquisizioneCentraleInfoList.Add(reazioneAvversaCentraleInfo)

                                    reazioneAvversa = Nothing

                                End If

                        End Select

                    End If

                    '-- VACCINAZIONI ESEGUITE
                    Dim vaccinazioneEseguitaCentraleDistribuitaUslUltimaOperazione As VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita =
                            GenericProviderCentrale.VaccinazioneEseguitaCentrale.GetVaccinazioneEseguitaCentraleDistribuitaByIdCentrale(
                            vaccinazioneEseguitaCentrale.Id, vaccinazioneEseguitaCentrale.CodiceUslUltimaOperazioneVaccinazioneEseguita)

                    Dim vaccinazioneEseguitaAcquisizioneInfo As New BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo()
                    vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguitaCentrale = vaccinazioneEseguitaCentrale
                    vaccinazioneEseguitaAcquisizioneInfo.ReazioneAvversa = reazioneAvversa

                    If loadDatiVaccinaliCentraliCommand.LoadDatiVaccinaliUslGestitaCorrente OrElse
                       vaccinazioneEseguitaCentrale.CodiceUslUltimaOperazioneVaccinazioneEseguita <> codiceUslCorrente Then

                        Dim dbGenericProvider As DbGenericProvider = GenericProviderFactory.GetDbGenericProvider(
                            GetIdApplicazioneByCodiceUsl(vaccinazioneEseguitaCentrale.CodiceUslUltimaOperazioneVaccinazioneEseguita), ContextInfos.CodiceAzienda)

                        Select Case vaccinazioneEseguitaCentrale.TipoVaccinazioneEseguitaCentrale

                            Case Constants.TipoVaccinazioneEseguitaCentrale.Programmata,
                                 Constants.TipoVaccinazioneEseguitaCentrale.Registrata,
                                 Constants.TipoVaccinazioneEseguitaCentrale.Recuperata,
                                 Constants.TipoVaccinazioneEseguitaCentrale.Ripristinata

                                vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguita = dbGenericProvider.VaccinazioniEseguite.GetVaccinazioneEseguitaById(
                                    vaccinazioneEseguitaCentraleDistribuitaUslUltimaOperazione.IdVaccinazioneEseguita)

                                Select Case vaccinazioneEseguitaCentrale.TipoVaccinazioneEseguitaCentrale
                                    Case Constants.TipoVaccinazioneEseguitaCentrale.Programmata
                                        vaccinazioneEseguitaProgrammataAcquisizioneInfoList.Add(vaccinazioneEseguitaAcquisizioneInfo)
                                    Case Constants.TipoVaccinazioneEseguitaCentrale.Registrata
                                        vaccinazioneEseguitaRegistrataAcquisizioneInfoList.Add(vaccinazioneEseguitaAcquisizioneInfo)
                                    Case Constants.TipoVaccinazioneEseguitaCentrale.Recuperata
                                        vaccinazioneEseguitaRecuperataAcquisizioneInfoList.Add(vaccinazioneEseguitaAcquisizioneInfo)
                                    Case Constants.TipoVaccinazioneEseguitaCentrale.Ripristinata
                                        vaccinazioneEseguitaRipristinataAcquisizioneInfoList.Add(vaccinazioneEseguitaAcquisizioneInfo)
                                    Case Else
                                        Throw New NotImplementedException()
                                End Select

                            Case Constants.TipoVaccinazioneEseguitaCentrale.Eliminata

                                vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguita = dbGenericProvider.VaccinazioniEseguite.GetVaccinazioneEseguitaEliminataById(
                                    vaccinazioneEseguitaCentraleDistribuitaUslUltimaOperazione.IdVaccinazioneEseguita)

                                vaccinazioneEseguitaEliminataAcquisizioneInfoList.Add(vaccinazioneEseguitaAcquisizioneInfo)

                            Case Constants.TipoVaccinazioneEseguitaCentrale.Scaduta

                                vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguita = dbGenericProvider.VaccinazioniEseguite.GetVaccinazioneEseguitaScadutaById(
                                    vaccinazioneEseguitaCentraleDistribuitaUslUltimaOperazione.IdVaccinazioneEseguita)

                                vaccinazioneEseguitaScadutaAcquisizioneInfoList.Add(vaccinazioneEseguitaAcquisizioneInfo)

                            Case Constants.TipoVaccinazioneEseguitaCentrale.ScadutaEliminata

                                vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguita =
                                    dbGenericProvider.VaccinazioniEseguite.GetVaccinazioneEseguitaScadutaEliminataById(
                                        vaccinazioneEseguitaCentraleDistribuitaUslUltimaOperazione.IdVaccinazioneEseguita)

                                vaccinazioneEseguitaScadutaEliminataAcquisizioneInfoList.Add(vaccinazioneEseguitaAcquisizioneInfo)

                        End Select

                    End If

                Next

            End If

            '-- VACCINAZIONI ESCLUSE
            Dim vaccinazioneEsclusaAcquisizioneInfoList As New List(Of BizVaccinazioniEscluse.VaccinazioneEsclusaCentraleInfo)()
            Dim vaccinazioneEsclusaEliminataAcquisizioneInfoList As New List(Of BizVaccinazioniEscluse.VaccinazioneEsclusaCentraleInfo)()

            If Not loadDatiVaccinaliCentraliCommand.VaccinazioniEscluseCentrale Is Nothing Then

                For Each vaccinazioneEsclusaCentrale As VaccinazioneEsclusaCentrale In loadDatiVaccinaliCentraliCommand.VaccinazioniEscluseCentrale

                    If loadDatiVaccinaliCentraliCommand.LoadDatiVaccinaliUslGestitaCorrente OrElse
                       vaccinazioneEsclusaCentrale.CodiceUslVaccinazioneEsclusa <> codiceUslCorrente Then

                        Dim dbGenericProvider As DbGenericProvider = GenericProviderFactory.GetDbGenericProvider(
                             GetIdApplicazioneByCodiceUsl(vaccinazioneEsclusaCentrale.CodiceUslVaccinazioneEsclusa), ContextInfos.CodiceAzienda)

                        Dim vaccinazioneEsclusaAcquisizioneInfo As New BizVaccinazioniEscluse.VaccinazioneEsclusaCentraleInfo()
                        vaccinazioneEsclusaAcquisizioneInfo.VaccinazioneEsclusaCentrale = vaccinazioneEsclusaCentrale

                        Select Case vaccinazioneEsclusaCentrale.TipoVaccinazioneEsclusaCentrale

                            Case Constants.TipoVaccinazioneEsclusaCentrale.Nessuno

                                vaccinazioneEsclusaAcquisizioneInfo.VaccinazioneEsclusa =
                                    dbGenericProvider.VaccinazioniEscluse.GetVaccinazioneEsclusaById(vaccinazioneEsclusaCentrale.IdVaccinazioneEsclusa)

                                vaccinazioneEsclusaAcquisizioneInfoList.Add(vaccinazioneEsclusaAcquisizioneInfo)

                            Case Constants.TipoVaccinazioneEsclusaCentrale.Eliminata

                                vaccinazioneEsclusaAcquisizioneInfo.VaccinazioneEsclusa =
                                    dbGenericProvider.VaccinazioniEscluse.GetVaccinazioneEsclusaEliminataById(vaccinazioneEsclusaCentrale.IdVaccinazioneEsclusa)

                                vaccinazioneEsclusaEliminataAcquisizioneInfoList.Add(vaccinazioneEsclusaAcquisizioneInfo)

                        End Select

                    End If

                Next

            End If

            '-- VISITE / OSSERVAZIONI
            Dim visitaAcquisizioneInfoList As New List(Of BizVisite.VisitaCentraleInfo)()
            Dim visitaEliminataAcquisizioneInfoList As New List(Of BizVisite.VisitaCentraleInfo)()

            If Not loadDatiVaccinaliCentraliCommand.VisiteCentrale Is Nothing Then

                For Each visitaCentrale As VisitaCentrale In loadDatiVaccinaliCentraliCommand.VisiteCentrale

                    If loadDatiVaccinaliCentraliCommand.LoadDatiVaccinaliUslGestitaCorrente OrElse
                        visitaCentrale.CodiceUslVisita <> codiceUslCorrente Then

                        Dim dbGenericProvider As DbGenericProvider = GenericProviderFactory.GetDbGenericProvider(
                            GetIdApplicazioneByCodiceUsl(visitaCentrale.CodiceUslVisita), ContextInfos.CodiceAzienda)

                        Dim visitaAcquisizioneInfo As New BizVisite.VisitaCentraleInfo()
                        visitaAcquisizioneInfo.VisitaCentrale = visitaCentrale

                        Select Case visitaCentrale.TipoVisitaCentrale

                            Case Constants.TipoVisitaCentrale.Nessuno

                                visitaAcquisizioneInfo.Visita = dbGenericProvider.Visite.GetVisitaById(visitaCentrale.IdVisita)

                                If loadDatiVaccinaliCentraliCommand.LoadOsservazioni Then
                                    visitaAcquisizioneInfo.Osservazioni = dbGenericProvider.Visite.GetOsservazioniByVisita(visitaCentrale.IdVisita)
                                End If

                                visitaAcquisizioneInfoList.Add(visitaAcquisizioneInfo)

                            Case Constants.TipoVisitaCentrale.Eliminata

                                visitaAcquisizioneInfo.Visita = dbGenericProvider.Visite.GetVisitaEliminataById(visitaCentrale.IdVisita)

                                If loadDatiVaccinaliCentraliCommand.LoadOsservazioni Then
                                    visitaAcquisizioneInfo.Osservazioni = dbGenericProvider.Visite.GetOsservazioniEliminateByIdVisitaEliminata(visitaCentrale.IdVisita)
                                End If

                                visitaEliminataAcquisizioneInfoList.Add(visitaAcquisizioneInfo)

                        End Select

                    End If

                Next

            End If

            loadDatiVaccinaliCentraliResult.DatiVaccinaliCentraliInfo.VaccinazioneEseguitaProgrammataCentraleInfos = vaccinazioneEseguitaProgrammataAcquisizioneInfoList.ToArray()
            loadDatiVaccinaliCentraliResult.DatiVaccinaliCentraliInfo.VaccinazioneEseguitaRecuperataCentraleInfos = vaccinazioneEseguitaRecuperataAcquisizioneInfoList.ToArray()
            loadDatiVaccinaliCentraliResult.DatiVaccinaliCentraliInfo.VaccinazioneEseguitaRegistrataCentraleInfos = vaccinazioneEseguitaRegistrataAcquisizioneInfoList.ToArray()
            loadDatiVaccinaliCentraliResult.DatiVaccinaliCentraliInfo.VaccinazioneEseguitaRipristinataCentraleInfos = vaccinazioneEseguitaRipristinataAcquisizioneInfoList.ToArray()
            loadDatiVaccinaliCentraliResult.DatiVaccinaliCentraliInfo.VaccinazioneEseguitaScadutaCentraleInfos = vaccinazioneEseguitaScadutaAcquisizioneInfoList.ToArray()
            loadDatiVaccinaliCentraliResult.DatiVaccinaliCentraliInfo.VaccinazioneEseguitaScadutaEliminataCentraleInfos = vaccinazioneEseguitaScadutaEliminataAcquisizioneInfoList.ToArray()
            loadDatiVaccinaliCentraliResult.DatiVaccinaliCentraliInfo.VaccinazioneEseguitaEliminataCentraleInfos = vaccinazioneEseguitaEliminataAcquisizioneInfoList.ToArray()

            loadDatiVaccinaliCentraliResult.DatiVaccinaliCentraliInfo.ReazioneAvversaCentraleInfos = reazioneAvversaAcquisizioneCentraleInfoList.ToArray()
            loadDatiVaccinaliCentraliResult.DatiVaccinaliCentraliInfo.ReazioneAvversaEliminataCentraleInfos = reazioneAvversaEliminataAcquisizioneCentraleInfoList.ToArray()

            loadDatiVaccinaliCentraliResult.DatiVaccinaliCentraliInfo.VaccinazioneEsclusaCentraleInfos = vaccinazioneEsclusaAcquisizioneInfoList.ToArray()
            loadDatiVaccinaliCentraliResult.DatiVaccinaliCentraliInfo.VaccinazioneEsclusaEliminataCentraleInfos = vaccinazioneEsclusaEliminataAcquisizioneInfoList.ToArray()

            loadDatiVaccinaliCentraliResult.DatiVaccinaliCentraliInfo.VisitaCentraleInfos = visitaAcquisizioneInfoList.ToArray()
            loadDatiVaccinaliCentraliResult.DatiVaccinaliCentraliInfo.VisitaEliminataCentraleInfos = visitaEliminataAcquisizioneInfoList.ToArray()

            loadDatiVaccinaliCentraliResult.DatiVaccinaliCentraliInfo.Empty = False

        End If

        Return loadDatiVaccinaliCentraliResult

    End Function

    Private Sub AggiornaVisibilitaDatiVaccinaliCentrali(codicePazienteCentrale As String, codiceUslInserimento As String, idVaccinazioniEseguite As Int64(),
                                                        idVaccinazioniEseguiteScadute As Int64(), idVaccinazioniEscluse As Int64(), idVisite As Int64(),
                                                        flagVisibilitaDatiVaccinaliCentraleAggiornato As String, flagVisibilitaDatiVaccinaliCentralePrecedente As String)

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions)

            Dim loadAll As Boolean =
                idVaccinazioniEseguite Is Nothing AndAlso idVaccinazioniEseguiteScadute Is Nothing AndAlso idVaccinazioniEscluse Is Nothing AndAlso idVisite Is Nothing

            Dim isVisibilitaCentrale As Boolean =
                Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(flagVisibilitaDatiVaccinaliCentraleAggiornato)

            Dim loadDatiVaccinaliCentraliCommand As New LoadDatiVaccinaliCentraliCommand()
            loadDatiVaccinaliCentraliCommand.LoadDatiVaccinaliUslGestitaCorrente = True
            loadDatiVaccinaliCentraliCommand.LoadDatiVaccinaliEliminati = True
            loadDatiVaccinaliCentraliCommand.LoadReazioneAvverse = isVisibilitaCentrale
            loadDatiVaccinaliCentraliCommand.LoadOsservazioni = isVisibilitaCentrale

            'VACCINAZIONE ESEGUITE
            If loadAll OrElse Not idVaccinazioniEseguite Is Nothing OrElse Not idVaccinazioniEseguiteScadute Is Nothing Then

                loadDatiVaccinaliCentraliCommand.VaccinazioniEseguiteCentrale =
                    Me.GenericProviderCentrale.VaccinazioneEseguitaCentrale.GetVaccinazioneEseguitaCentraleEnumerable(codicePazienteCentrale).ToArray()

                If Not idVaccinazioniEseguite Is Nothing OrElse Not idVaccinazioniEseguiteScadute Is Nothing Then

                    loadDatiVaccinaliCentraliCommand.VaccinazioniEseguiteCentrale = loadDatiVaccinaliCentraliCommand.VaccinazioniEseguiteCentrale.Where(
                        Function(vec) (Not idVaccinazioniEseguite Is Nothing AndAlso idVaccinazioniEseguite.Contains(vec.IdVaccinazioneEseguita)) OrElse
                            (Not idVaccinazioniEseguiteScadute Is Nothing AndAlso idVaccinazioniEseguiteScadute.Contains(vec.IdVaccinazioneEseguita))).ToArray()

                End If

            End If


            'VACCINAZIONE ESCLUSE
            If loadAll OrElse Not idVaccinazioniEscluse Is Nothing Then

                loadDatiVaccinaliCentraliCommand.VaccinazioniEscluseCentrale =
                    Me.GenericProviderCentrale.VaccinazioneEsclusaCentrale.GetVaccinazioneEsclusaCentraleEnumerable(codicePazienteCentrale).ToArray()

                If Not idVaccinazioniEscluse Is Nothing Then
                    loadDatiVaccinaliCentraliCommand.VaccinazioniEscluseCentrale = loadDatiVaccinaliCentraliCommand.VaccinazioniEscluseCentrale.Where(
                        Function(vec) idVaccinazioniEscluse.Contains(vec.IdVaccinazioneEsclusa)).ToArray()
                End If

            End If


            'VISITE
            If loadAll OrElse Not idVisite Is Nothing Then

                loadDatiVaccinaliCentraliCommand.VisiteCentrale =
                    Me.GenericProviderCentrale.VisitaCentrale.GetVisitaCentraleEnumerable(codicePazienteCentrale).ToArray()

                If Not idVisite Is Nothing Then
                    loadDatiVaccinaliCentraliCommand.VisiteCentrale = loadDatiVaccinaliCentraliCommand.VisiteCentrale.Where(
                        Function(vic) idVisite.Contains(vic.IdVisita)).ToArray()
                End If

            End If

            'CONFLITTI
            If Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(flagVisibilitaDatiVaccinaliCentraleAggiornato) Then

                If Not loadDatiVaccinaliCentraliCommand.VaccinazioniEseguiteCentrale Is Nothing Then
                    loadDatiVaccinaliCentraliCommand.VaccinazioniEseguiteCentrale = loadDatiVaccinaliCentraliCommand.VaccinazioniEseguiteCentrale.Where(
                      Function(vec) Not vec.IdConflitto.HasValue OrElse vec.DataRisoluzioneConflitto.HasValue).ToArray()
                End If

                If Not loadDatiVaccinaliCentraliCommand.VaccinazioniEscluseCentrale Is Nothing Then
                    loadDatiVaccinaliCentraliCommand.VaccinazioniEscluseCentrale = loadDatiVaccinaliCentraliCommand.VaccinazioniEscluseCentrale.Where(
                        Function(vec) Not vec.IdConflitto.HasValue OrElse vec.DataRisoluzioneConflitto.HasValue).ToArray()
                End If

                If Not loadDatiVaccinaliCentraliCommand.VisiteCentrale Is Nothing Then
                    loadDatiVaccinaliCentraliCommand.VisiteCentrale = loadDatiVaccinaliCentraliCommand.VisiteCentrale.Where(
                        Function(vic) Not vic.IdConflitto.HasValue OrElse vic.DataRisoluzioneConflitto.HasValue).ToArray()
                End If

            End If

            'VISIBILITA
            If Not String.IsNullOrEmpty(flagVisibilitaDatiVaccinaliCentralePrecedente) Then

                If Not loadDatiVaccinaliCentraliCommand.VaccinazioniEscluseCentrale Is Nothing Then
                    loadDatiVaccinaliCentraliCommand.VaccinazioniEscluseCentrale = loadDatiVaccinaliCentraliCommand.VaccinazioniEscluseCentrale.Where(
                        Function(vec) vec.FlagVisibilitaCentrale = flagVisibilitaDatiVaccinaliCentralePrecedente).ToArray()
                End If

                If Not loadDatiVaccinaliCentraliCommand.VaccinazioniEseguiteCentrale Is Nothing Then
                    loadDatiVaccinaliCentraliCommand.VaccinazioniEseguiteCentrale = loadDatiVaccinaliCentraliCommand.VaccinazioniEseguiteCentrale.Where(
                        Function(vec) vec.FlagVisibilitaCentrale = flagVisibilitaDatiVaccinaliCentralePrecedente).ToArray()
                End If

                If Not loadDatiVaccinaliCentraliCommand.VisiteCentrale Is Nothing Then
                    loadDatiVaccinaliCentraliCommand.VisiteCentrale = loadDatiVaccinaliCentraliCommand.VisiteCentrale.Where(
                        Function(vic) vic.FlagVisibilitaCentrale = flagVisibilitaDatiVaccinaliCentralePrecedente).ToArray()
                End If

            End If

            'USL INSERIMENTO
            If Not String.IsNullOrEmpty(codiceUslInserimento) Then

                If Not loadDatiVaccinaliCentraliCommand.VaccinazioniEseguiteCentrale Is Nothing Then
                    loadDatiVaccinaliCentraliCommand.VaccinazioniEseguiteCentrale = loadDatiVaccinaliCentraliCommand.VaccinazioniEseguiteCentrale.Where(
                        Function(vec) vec.CodiceUslVaccinazioneEseguita = codiceUslInserimento).ToArray()
                End If

                If Not loadDatiVaccinaliCentraliCommand.VaccinazioniEscluseCentrale Is Nothing Then
                    loadDatiVaccinaliCentraliCommand.VaccinazioniEscluseCentrale = loadDatiVaccinaliCentraliCommand.VaccinazioniEscluseCentrale.Where(
                        Function(vec) vec.CodiceUslVaccinazioneEsclusa = codiceUslInserimento).ToArray()
                End If

                If Not loadDatiVaccinaliCentraliCommand.VisiteCentrale Is Nothing Then
                    loadDatiVaccinaliCentraliCommand.VisiteCentrale = loadDatiVaccinaliCentraliCommand.VisiteCentrale.Where(
                        Function(vic) vic.CodiceUslVisita = codiceUslInserimento).ToArray()
                End If

            End If

            Dim loadDatiVaccinaliCentraliResult As LoadDatiVaccinaliCentraliResult = LoadDatiVaccinaliCentrali(loadDatiVaccinaliCentraliCommand)

            If Not loadDatiVaccinaliCentraliResult.DatiVaccinaliCentraliInfo.Empty Then

                Me.AggiornaVisibilitaDatiVaccinaliCentrali(codicePazienteCentrale, flagVisibilitaDatiVaccinaliCentraleAggiornato,
                                                           loadDatiVaccinaliCentraliResult.DatiVaccinaliCentraliInfo)
            End If

            transactionScope.Complete()

        End Using

    End Sub

    Private Sub AggiornaVisibilitaDatiVaccinaliCentrali(codicePazienteCentrale As String, flagVisibilitaDatiVaccinaliCentrale As String,
                                                        datiVaccinaliCentraliInfo As DatiVaccinaliCentraliInfo)

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions)

            Dim vaccinazioneEseguitaCentraleInfoEnumerable As IEnumerable(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo) =
                datiVaccinaliCentraliInfo.VaccinazioneEseguitaProgrammataCentraleInfos.
                    Union(datiVaccinaliCentraliInfo.VaccinazioneEseguitaRecuperataCentraleInfos).
                    Union(datiVaccinaliCentraliInfo.VaccinazioneEseguitaRegistrataCentraleInfos).
                    Union(datiVaccinaliCentraliInfo.VaccinazioneEseguitaRipristinataCentraleInfos).
                    Union(datiVaccinaliCentraliInfo.VaccinazioneEseguitaEliminataCentraleInfos)

            Dim vaccinazioneEseguitaScadutaCentraleInfoEnumerable As IEnumerable(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo) =
                datiVaccinaliCentraliInfo.VaccinazioneEseguitaScadutaCentraleInfos.
                    Union(datiVaccinaliCentraliInfo.VaccinazioneEseguitaScadutaEliminataCentraleInfos)

            Dim vaccinazioneEsclusaCentraleInfoEnumerable As IEnumerable(Of BizVaccinazioniEscluse.VaccinazioneEsclusaCentraleInfo) =
               datiVaccinaliCentraliInfo.VaccinazioneEsclusaCentraleInfos.
                    Union(datiVaccinaliCentraliInfo.VaccinazioneEsclusaEliminataCentraleInfos)

            Dim visitaCentraleInfoEnumerable As IEnumerable(Of BizVisite.VisitaCentraleInfo) =
               datiVaccinaliCentraliInfo.VisitaCentraleInfos.
                    Union(datiVaccinaliCentraliInfo.VisitaEliminataCentraleInfos)

            Dim codiceUslUltimaOperazioneVaccinazioneEseguitaEnumerable As IEnumerable(Of String) = vaccinazioneEseguitaCentraleInfoEnumerable.Select(
                Function(veci) veci.VaccinazioneEseguitaCentrale.CodiceUslUltimaOperazioneVaccinazioneEseguita).Distinct()

            Dim codiceUslUltimaOperazioneVaccinazioneEseguitaScadutaEnumerable As IEnumerable(Of String) = vaccinazioneEseguitaScadutaCentraleInfoEnumerable.Select(
                Function(vesci) vesci.VaccinazioneEseguitaCentrale.CodiceUslUltimaOperazioneVaccinazioneEseguita).Distinct()

            Dim codiceUslVaccinazioneEsclusaEnumerable As IEnumerable(Of String) = vaccinazioneEsclusaCentraleInfoEnumerable.Select(
              Function(veci) veci.VaccinazioneEsclusaCentrale.CodiceUslVaccinazioneEsclusa).Distinct()

            Dim codiceUslVisitaEnumerable As IEnumerable(Of String) = visitaCentraleInfoEnumerable.Select(
                Function(vci) vci.VisitaCentrale.CodiceUslVisita).Distinct()

            Dim codiceUslUltimaOperazioneEnumerable As IEnumerable(Of String) = codiceUslUltimaOperazioneVaccinazioneEseguitaEnumerable.
              Union(codiceUslVaccinazioneEsclusaEnumerable).Union(codiceUslUltimaOperazioneVaccinazioneEseguitaScadutaEnumerable).Union(codiceUslVisitaEnumerable).Distinct()

            Dim isFirstUsl As Boolean = True

            Dim isVisibiltaCentrale As Boolean = Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(flagVisibilitaDatiVaccinaliCentrale)

            For Each codiceUslUltimaOperazione As String In codiceUslUltimaOperazioneEnumerable

                Dim genericProviderUslUltimaOperazione As DbGenericProvider =
                    GenericProviderFactory.GetDbGenericProvider(GetIDApplicazioneByCodiceUslGestita(codiceUslUltimaOperazione), ContextInfos.CodiceAzienda)

                Dim settingsUslUltimaOperazione As Settings.Settings = Me.UslGestitaAllineaSettingsProvider.GetSettings(genericProviderUslUltimaOperazione)
                Dim bizContextInfosUslUltimaOperazione As BizContextInfos = Me.CreateBizContextInfosByCodiceUslGestita(codiceUslUltimaOperazione)

                Using bizPaziente As New Biz.BizPaziente(Me.GenericProviderFactory, settingsUslUltimaOperazione, bizContextInfosUslUltimaOperazione, Me.LogOptions)

                    Dim vaccinazioneEseguitaCentraleInfoList As New List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)()
                    Dim vaccinazioneEseguitaScadutaCentraleInfoList As New List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)()
                    Dim vaccinazioneEsclusaCentraleInfoList As New List(Of BizVaccinazioniEscluse.VaccinazioneEsclusaCentraleInfo)()
                    Dim visitaCentraleInfoList As New List(Of BizVisite.VisitaCentraleInfo)()

                    Dim codiceUslUltimaOperazioneFilter As String = codiceUslUltimaOperazione

                    Dim vaccinazioneEseguitaCentraleInfos As BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo() =
                        vaccinazioneEseguitaCentraleInfoEnumerable.Where(
                            Function(veci) veci.VaccinazioneEseguitaCentrale.CodiceUslUltimaOperazioneVaccinazioneEseguita = codiceUslUltimaOperazioneFilter).ToArray()

                    Dim vaccinazioneEseguitaScadutaCentraleInfos As BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo() =
                       vaccinazioneEseguitaScadutaCentraleInfoEnumerable.Where(
                            Function(veci) veci.VaccinazioneEseguitaCentrale.CodiceUslUltimaOperazioneVaccinazioneEseguita = codiceUslUltimaOperazioneFilter).ToArray()

                    Dim vaccinazioneEsclusaCentraleInfos As BizVaccinazioniEscluse.VaccinazioneEsclusaCentraleInfo() =
                        vaccinazioneEsclusaCentraleInfoEnumerable.Where(
                            Function(veci) veci.VaccinazioneEsclusaCentrale.CodiceUslVaccinazioneEsclusa = codiceUslUltimaOperazioneFilter).ToArray()

                    Dim visitaCentraleInfos As BizVisite.VisitaCentraleInfo() =
                            visitaCentraleInfoEnumerable.Where(
                                Function(vici) vici.VisitaCentrale.CodiceUslVisita = codiceUslUltimaOperazioneFilter).ToArray()

                    If Not isVisibiltaCentrale OrElse isFirstUsl Then

                        vaccinazioneEseguitaCentraleInfoList.AddRange(vaccinazioneEseguitaCentraleInfos)
                        vaccinazioneEseguitaScadutaCentraleInfoList.AddRange(vaccinazioneEseguitaScadutaCentraleInfos)
                        vaccinazioneEsclusaCentraleInfoList.AddRange(vaccinazioneEsclusaCentraleInfos)
                        visitaCentraleInfoList.AddRange(visitaCentraleInfos)

                    Else

                        For Each vaccinazioneEseguitaCentraleInfo As BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo In vaccinazioneEseguitaCentraleInfos
                            If vaccinazioneEseguitaCentraleInfo.VaccinazioneEseguitaCentrale.TipoVaccinazioneEseguitaCentrale = Constants.TipoVaccinazioneEseguitaCentrale.Eliminata OrElse
                                Not Me.GenericProviderCentrale.VaccinazioneEseguitaCentrale.IsVaccinazioneEseguitaCentraleInConflitto(vaccinazioneEseguitaCentraleInfo.VaccinazioneEseguitaCentrale.Id) Then
                                vaccinazioneEseguitaCentraleInfoList.Add(vaccinazioneEseguitaCentraleInfo)
                            End If
                        Next

                        For Each vaccinazioneEseguitaScadutaCentraleInfo As BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo In vaccinazioneEseguitaScadutaCentraleInfos
                            If vaccinazioneEseguitaScadutaCentraleInfo.VaccinazioneEseguitaCentrale.TipoVaccinazioneEseguitaCentrale = Constants.TipoVaccinazioneEseguitaCentrale.ScadutaEliminata OrElse
                                Not Me.GenericProviderCentrale.VaccinazioneEseguitaCentrale.IsVaccinazioneEseguitaCentraleInConflitto(vaccinazioneEseguitaScadutaCentraleInfo.VaccinazioneEseguitaCentrale.Id) Then
                                vaccinazioneEseguitaScadutaCentraleInfoList.Add(vaccinazioneEseguitaScadutaCentraleInfo)
                            End If
                        Next

                        For Each vaccinazioneEsclusaCentraleInfo As BizVaccinazioniEscluse.VaccinazioneEsclusaCentraleInfo In vaccinazioneEsclusaCentraleInfos
                            If vaccinazioneEsclusaCentraleInfo.VaccinazioneEsclusaCentrale.TipoVaccinazioneEsclusaCentrale = Constants.TipoVaccinazioneEsclusaCentrale.Eliminata OrElse
                                Not Me.GenericProviderCentrale.VaccinazioneEsclusaCentrale.IsVaccinazioneEsclusaCentraleEliminata(vaccinazioneEsclusaCentraleInfo.VaccinazioneEsclusaCentrale.Id) Then
                                vaccinazioneEsclusaCentraleInfoList.Add(vaccinazioneEsclusaCentraleInfo)
                            End If
                        Next

                        For Each visitaCentraleInfo As BizVisite.VisitaCentraleInfo In visitaCentraleInfos
                            If visitaCentraleInfo.VisitaCentrale.TipoVisitaCentrale = Constants.TipoVisitaCentrale.Eliminata OrElse
                                Not Me.GenericProviderCentrale.VisitaCentrale.IsVisitaCentraleInConflitto(visitaCentraleInfo.VisitaCentrale.Id) Then
                                visitaCentraleInfoList.Add(visitaCentraleInfo)
                            End If
                        Next

                    End If

                    bizPaziente.AggiornaVisibilitaDatiVaccinaliCentrali(codicePazienteCentrale, flagVisibilitaDatiVaccinaliCentrale,
                        vaccinazioneEsclusaCentraleInfoList.ToArray(), vaccinazioneEseguitaCentraleInfoList.ToArray(),
                            vaccinazioneEseguitaScadutaCentraleInfoList.ToArray(), visitaCentraleInfoList.ToArray())

                End Using

                isFirstUsl = False

            Next

            Dim reazioneAvversaCentraleInfos As IEnumerable(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo) =
                datiVaccinaliCentraliInfo.ReazioneAvversaCentraleInfos.
                    Union(datiVaccinaliCentraliInfo.ReazioneAvversaEliminataCentraleInfos)

            For Each codiceUslUltimaOperazioneReazioneAvversa As String In reazioneAvversaCentraleInfos.Select(
                   Function(raci) raci.VaccinazioneEseguitaCentrale.CodiceUslUltimaOperazioneReazioneAvversa).Distinct()

                Dim genericProviderUslUltimaOperazioneReazioneAvversa As DbGenericProvider = Me.GenericProviderFactory.GetDbGenericProvider(
                    GetIDApplicazioneByCodiceUslGestita(codiceUslUltimaOperazioneReazioneAvversa), Me.ContextInfos.CodiceAzienda)

                Dim settingsUslUltimaOperazioneReazioneAvversa As Settings.Settings = Me.UslGestitaAllineaSettingsProvider.GetSettings(genericProviderUslUltimaOperazioneReazioneAvversa)
                Dim bizContextInfosUslUltimaOperazioneReazioneAvversa As BizContextInfos = Me.CreateBizContextInfosByCodiceUslGestita(codiceUslUltimaOperazioneReazioneAvversa)

                Using bizPaziente As New Biz.BizPaziente(Me.GenericProviderFactory, settingsUslUltimaOperazioneReazioneAvversa, bizContextInfosUslUltimaOperazioneReazioneAvversa, Me.LogOptions)

                    Dim reazioneAvversaCentraleInfoList As New List(Of BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo)

                    Dim codiceUslUltimaOperazioneReazioneFilter As String = codiceUslUltimaOperazioneReazioneAvversa

                    Dim reazioneAvversaCentraleInfosUslUltimaOperazione As BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo() = reazioneAvversaCentraleInfos.
                        Where(Function(raci) raci.VaccinazioneEseguitaCentrale.CodiceUslUltimaOperazioneReazioneAvversa = codiceUslUltimaOperazioneReazioneFilter).ToArray()

                    If Not isVisibiltaCentrale Then
                        reazioneAvversaCentraleInfoList.AddRange(reazioneAvversaCentraleInfosUslUltimaOperazione)
                    Else
                        For Each reazioneAvversaCentraleInfo As BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo In reazioneAvversaCentraleInfosUslUltimaOperazione
                            If reazioneAvversaCentraleInfo.VaccinazioneEseguitaCentrale.TipoReazioneAvversaCentrale = Constants.TipoReazioneAvversaCentrale.Eliminata OrElse
                                Not Me.GenericProviderCentrale.VaccinazioneEseguitaCentrale.IsVaccinazioneEseguitaCentraleInConflitto(reazioneAvversaCentraleInfo.VaccinazioneEseguitaCentrale.Id) Then
                                reazioneAvversaCentraleInfoList.Add(reazioneAvversaCentraleInfo)
                            End If
                        Next
                    End If


                    Dim aggiornaDatiVaccinaliCentraliCommand As New AggiornaDatiVaccinaliCentraliCommand()
                    aggiornaDatiVaccinaliCentraliCommand.FlagVisibilitaModificata = flagVisibilitaDatiVaccinaliCentrale
                    aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale = codicePazienteCentrale

                    aggiornaDatiVaccinaliCentraliCommand.ReazioneAvversaEnumerable = reazioneAvversaCentraleInfoList.Where(
                        Function(raci) raci.VaccinazioneEseguitaCentrale.TipoReazioneAvversaCentrale <> Constants.TipoReazioneAvversaCentrale.Eliminata).Select(
                            Function(raci) raci.ReazioneAvversa)

                    aggiornaDatiVaccinaliCentraliCommand.ReazioneAvversaEliminataEnumerable = reazioneAvversaCentraleInfoList.Where(
                        Function(raci) raci.VaccinazioneEseguitaCentrale.TipoReazioneAvversaCentrale = Constants.TipoReazioneAvversaCentrale.Eliminata).Select(
                            Function(raci) raci.ReazioneAvversa)

                    bizPaziente.AggiornaDatiVaccinaliCentrali(aggiornaDatiVaccinaliCentraliCommand)

                End Using

            Next

            transactionScope.Complete()

        End Using

    End Sub

    Private Sub AggiornaVisibilitaDatiVaccinaliCentrali(codicePazienteCentrale As String, flagVisibilitaDatiVaccinaliCentrale As String,
                                                       vaccinazioneEsclusaCentraleInfos As BizVaccinazioniEscluse.VaccinazioneEsclusaCentraleInfo(),
                                                       vaccinazioneEseguitaCentraleInfos As BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo(),
                                                       vaccinazioneEseguitaScadutaCentraleInfos As BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo(),
                                                       visitaCentraleInfos As BizVisite.VisitaCentraleInfo())

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions)

            Dim vaccinazioneEsclusaList As New List(Of VaccinazioneEsclusa)
            Dim vaccinazioneEsclusaEliminataList As New List(Of VaccinazioneEsclusa)
            Dim vaccinazioneEseguitaList As New List(Of VaccinazioneEseguita)
            Dim vaccinazioneEseguitaEliminataList As New List(Of VaccinazioneEseguita)
            Dim vaccinazioneEseguitaScadutaList As New List(Of VaccinazioneEseguita)
            Dim vaccinazioneEseguitaScadutaEliminataList As New List(Of VaccinazioneEseguita)
            Dim reazioneAvversaList As New List(Of ReazioneAvversa)
            Dim reazioneAvversaEliminataList As New List(Of ReazioneAvversa)
            Dim visitaList As New List(Of Visita)
            Dim visitaEliminataList As New List(Of Visita)
            Dim osservazioneList As New List(Of Osservazione)
            Dim osservazioneEliminataList As New List(Of Osservazione)

            Dim codicePaziente As Int64 = Convert.ToInt64(Me.GenericProvider.Paziente.GetCodicePazientiByCodiceAusiliario(codicePazienteCentrale).First())

            Using bizVaccinazioniEscluse As New BizVaccinazioniEscluse(Me.GenericProviderFactory, Me.Settings, Me.ContextInfos, Me.LogOptions)

                For Each vaccinazioneEsclusaAcquisizioneCentraleInfo As BizVaccinazioniEscluse.VaccinazioneEsclusaCentraleInfo In vaccinazioneEsclusaCentraleInfos

                    If vaccinazioneEsclusaAcquisizioneCentraleInfo.VaccinazioneEsclusaCentrale.TipoVaccinazioneEsclusaCentrale <>
                        Constants.TipoVaccinazioneEsclusaCentrale.Eliminata Then

                        Dim bizResult As BizResult = bizVaccinazioniEscluse.AggiornaVisibilitaVaccinazioneEsclusa(flagVisibilitaDatiVaccinaliCentrale,
                            vaccinazioneEsclusaAcquisizioneCentraleInfo.VaccinazioneEsclusa, vaccinazioneEsclusaAcquisizioneCentraleInfo.VaccinazioneEsclusaCentrale)

                        If bizResult.Success Then
                            vaccinazioneEsclusaList.Add(vaccinazioneEsclusaAcquisizioneCentraleInfo.VaccinazioneEsclusa)
                        End If

                    Else

                        vaccinazioneEsclusaAcquisizioneCentraleInfo.VaccinazioneEsclusa.FlagVisibilita = flagVisibilitaDatiVaccinaliCentrale

                        vaccinazioneEsclusaEliminataList.Add(vaccinazioneEsclusaAcquisizioneCentraleInfo.VaccinazioneEsclusa)

                    End If

                Next

            End Using

            Using bizVaccinazioniEseguite As New BizVaccinazioniEseguite(Me.GenericProviderFactory, Me.Settings, Me.ContextInfos, Me.LogOptions)

                For Each vaccinazioneEseguitaScadutaAcquisizioneCentraleInfo As BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo In vaccinazioneEseguitaScadutaCentraleInfos

                    If vaccinazioneEseguitaScadutaAcquisizioneCentraleInfo.VaccinazioneEseguitaCentrale.TipoVaccinazioneEseguitaCentrale <>
                      Constants.TipoVaccinazioneEseguitaCentrale.ScadutaEliminata Then

                        Dim bizResult As BizResult = bizVaccinazioniEseguite.AggiornaVisibilitaVaccinazioneEseguita(flagVisibilitaDatiVaccinaliCentrale,
                            vaccinazioneEseguitaScadutaAcquisizioneCentraleInfo.VaccinazioneEseguita,
                            vaccinazioneEseguitaScadutaAcquisizioneCentraleInfo.VaccinazioneEseguitaCentrale, True)

                        If bizResult.Success Then

                            vaccinazioneEseguitaScadutaList.Add(vaccinazioneEseguitaScadutaAcquisizioneCentraleInfo.VaccinazioneEseguita)

                            If Not vaccinazioneEseguitaScadutaAcquisizioneCentraleInfo.ReazioneAvversa Is Nothing Then
                                reazioneAvversaList.Add(vaccinazioneEseguitaScadutaAcquisizioneCentraleInfo.ReazioneAvversa)
                            End If

                        End If

                    Else

                        vaccinazioneEseguitaScadutaAcquisizioneCentraleInfo.VaccinazioneEseguita.ves_flag_visibilita_vac_centrale = flagVisibilitaDatiVaccinaliCentrale

                        vaccinazioneEseguitaScadutaEliminataList.Add(vaccinazioneEseguitaScadutaAcquisizioneCentraleInfo.VaccinazioneEseguita)

                        If Not vaccinazioneEseguitaScadutaAcquisizioneCentraleInfo.ReazioneAvversa Is Nothing Then
                            reazioneAvversaEliminataList.Add(vaccinazioneEseguitaScadutaAcquisizioneCentraleInfo.ReazioneAvversa)
                        End If

                    End If

                Next

                For Each vaccinazioneEseguitaAcquisizioneCentraleInfo As BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo In vaccinazioneEseguitaCentraleInfos

                    If vaccinazioneEseguitaAcquisizioneCentraleInfo.VaccinazioneEseguitaCentrale.TipoVaccinazioneEseguitaCentrale <>
                        Constants.TipoVaccinazioneEseguitaCentrale.Eliminata Then

                        Dim bizResult As BizResult = bizVaccinazioniEseguite.AggiornaVisibilitaVaccinazioneEseguita(flagVisibilitaDatiVaccinaliCentrale,
                            vaccinazioneEseguitaAcquisizioneCentraleInfo.VaccinazioneEseguita,
                            vaccinazioneEseguitaAcquisizioneCentraleInfo.VaccinazioneEseguitaCentrale, False)

                        If bizResult.Success Then

                            vaccinazioneEseguitaList.Add(vaccinazioneEseguitaAcquisizioneCentraleInfo.VaccinazioneEseguita)

                            If Not vaccinazioneEseguitaAcquisizioneCentraleInfo.ReazioneAvversa Is Nothing Then
                                reazioneAvversaList.Add(vaccinazioneEseguitaAcquisizioneCentraleInfo.ReazioneAvversa)
                            End If

                        End If

                    Else

                        vaccinazioneEseguitaAcquisizioneCentraleInfo.VaccinazioneEseguita.ves_flag_visibilita_vac_centrale = flagVisibilitaDatiVaccinaliCentrale

                        vaccinazioneEseguitaEliminataList.Add(vaccinazioneEseguitaAcquisizioneCentraleInfo.VaccinazioneEseguita)

                        If Not vaccinazioneEseguitaAcquisizioneCentraleInfo.ReazioneAvversa Is Nothing Then
                            reazioneAvversaEliminataList.Add(vaccinazioneEseguitaAcquisizioneCentraleInfo.ReazioneAvversa)
                        End If

                    End If

                Next

            End Using

            Using bizVisite As New BizVisite(Me.GenericProviderFactory, Me.Settings, Me.ContextInfos, Me.LogOptions)

                For Each visitaAcquisizioneCentraleInfo As BizVisite.VisitaCentraleInfo In visitaCentraleInfos

                    If visitaAcquisizioneCentraleInfo.VisitaCentrale.TipoVisitaCentrale <>
                       Constants.TipoVisitaCentrale.Eliminata Then

                        Dim modificaVisitaBizResult As BizVisite.ModificaVisitaBizResult = bizVisite.AggiornaVisibilitaVisita(flagVisibilitaDatiVaccinaliCentrale,
                            visitaAcquisizioneCentraleInfo.Visita, visitaAcquisizioneCentraleInfo.VisitaCentrale, "Aggiorna visibilità dati vaccinali centrali")

                        If modificaVisitaBizResult.Success Then

                            visitaList.Add(visitaAcquisizioneCentraleInfo.Visita)

                            If Not visitaAcquisizioneCentraleInfo.Osservazioni Is Nothing Then
                                osservazioneList.AddRange(visitaAcquisizioneCentraleInfo.Osservazioni)
                            End If

                        End If

                    Else

                        visitaAcquisizioneCentraleInfo.Visita.FlagVisibilitaDatiVaccinaliCentrale = flagVisibilitaDatiVaccinaliCentrale

                        visitaEliminataList.Add(visitaAcquisizioneCentraleInfo.Visita)

                        If Not visitaAcquisizioneCentraleInfo.Osservazioni Is Nothing Then
                            osservazioneEliminataList.AddRange(visitaAcquisizioneCentraleInfo.Osservazioni)
                        End If

                    End If

                Next

            End Using

            Dim aggiornaDatiVaccinaliCentraliCommand As New AggiornaDatiVaccinaliCentraliCommand()

            aggiornaDatiVaccinaliCentraliCommand.FlagVisibilitaModificata = flagVisibilitaDatiVaccinaliCentrale
            aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEseguitaEnumerable = vaccinazioneEseguitaList.AsEnumerable()
            aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEseguitaEliminataEnumerable = vaccinazioneEseguitaEliminataList.AsEnumerable()
            aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEseguitaScadutaEnumerable = vaccinazioneEseguitaScadutaList.AsEnumerable()
            aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEseguitaScadutaEliminataEnumerable = vaccinazioneEseguitaScadutaEliminataList.AsEnumerable()
            aggiornaDatiVaccinaliCentraliCommand.ReazioneAvversaEnumerable = reazioneAvversaList.AsEnumerable()
            aggiornaDatiVaccinaliCentraliCommand.ReazioneAvversaEliminataEnumerable = reazioneAvversaEliminataList.AsEnumerable()
            aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEsclusaEnumerable = vaccinazioneEsclusaList.AsEnumerable()
            aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEsclusaEliminataEnumerable = vaccinazioneEsclusaEliminataList.AsEnumerable()
            aggiornaDatiVaccinaliCentraliCommand.VisitaEnumerable = visitaList.AsEnumerable()
            aggiornaDatiVaccinaliCentraliCommand.VisitaEliminataEnumerable = visitaEliminataList.AsEnumerable()
            aggiornaDatiVaccinaliCentraliCommand.OsservazioneEnumerable = osservazioneList.AsEnumerable()
            aggiornaDatiVaccinaliCentraliCommand.OsservazioneEliminataEnumerable = osservazioneEliminataList.AsEnumerable()
            aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale = codicePazienteCentrale

            Me.AggiornaDatiVaccinaliCentrali(aggiornaDatiVaccinaliCentraliCommand)

            transactionScope.Complete()

        End Using

    End Sub

#End Region

#End Region

#Region " Consenso "

    ''' <summary>
    ''' Restituisce lo stato del consenso alla comunicazione relativo al paziente, in base al codice (locale) indicato.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetConsensoComunicazionePaziente(codicePaziente As Int64) As Consenso.StatoConsensoPaziente
        Return GetConsensoPaziente(codicePaziente, Settings.CONSENSO_ID_COMUNICAZIONE)
    End Function

    ''' <summary>
    ''' Restituisce lo stato del consenso alla comunicazione relativo al paziente, in base al codice ausiliario indicato.
    ''' </summary>
    ''' <param name="codiceAusiliarioPaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetConsensoComunicazionePaziente(codiceAusiliarioPaziente As String) As Consenso.StatoConsensoPaziente
        Return GetConsensoPaziente(codiceAusiliarioPaziente, Settings.CONSENSO_ID_COMUNICAZIONE)
    End Function

    ''' <summary>
    ''' Restituisce lo stato del consenso globale del paziente (senza filtro per id consenso)
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetConsensoGlobalePaziente(codicePaziente As Int64) As Consenso.StatoConsensoPaziente
        Return GetConsensoPaziente(codicePaziente, Nothing)
    End Function

    ''' <summary>
    ''' Restituisce lo stato del consenso globale del paziente (senza filtro per id consenso)
    ''' </summary>
    ''' <param name="codiceAusiliarioPaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetConsensoGlobalePaziente(codiceAusiliarioPaziente As String) As Consenso.StatoConsensoPaziente
        Return GetConsensoPaziente(codiceAusiliarioPaziente, Nothing)
    End Function

    ''' <summary>
    ''' Restituisce lo stato del consenso del paziente relativo all'id specificato
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="idConsenso"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetConsensoPaziente(codicePaziente As Int64, idConsenso As Integer?) As Consenso.StatoConsensoPaziente
        Dim codiceAusiliarioPaziente As String = GetCodiceAusiliario(codicePaziente)
        Return GetConsensoPaziente(codiceAusiliarioPaziente, idConsenso)
    End Function

    ''' <summary>
    ''' Restituisce lo stato del consenso del paziente identificato dal suo codice centrale.
    ''' Lo stato è relativo al consenso di id specificato, se presente. Altrimenti, restituisce lo stato globale dei consensi del paziente.
    ''' </summary>
    ''' <param name="codiceAusiliarioPaziente"></param>
    ''' <param name="idConsenso"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetConsensoPaziente(codiceAusiliarioPaziente As String, idConsenso As Integer?) As Consenso.StatoConsensoPaziente

        Dim statoConsenso As Consenso.StatoConsensoPaziente = Nothing

        Dim listConsensi As List(Of Onit.OnAssistnet.GestioneConsenso.Wcf.Proxy.ConsensiService.ConsensoRegistrato) = Nothing
        Dim lst As List(Of Onit.OnAssistnet.GestioneConsenso.Wcf.Proxy.ConsensiService.ConsensoRegistrato) = Nothing

        ' Azienda di registrazione => quella relativa all'appId corrente
        Dim codiceAziendaRegistrazione As String = ContextInfos.CodiceUsl
        If String.IsNullOrEmpty(codiceAziendaRegistrazione) Then codiceAziendaRegistrazione = GetCodiceUslByIdApplicazione(ContextInfos.IDApplicazione)

        ' Lista consensi del paziente. Contiene tutti i consensi:
        '   quelli registrati, con il valore più recente
        '   quelli non registrati, con il valore "non rilevato"
        ' Dalla lista vengono rimossi i consensi obsoleti e quelli non usati per il calcolo dello stato globale
        Using consenso As New Onit.OnAssistnet.GestioneConsenso.Wcf.Proxy.ConsensiService.ConsensiServiceClient()

            Dim c As Onit.OnAssistnet.GestioneConsenso.Wcf.Proxy.ConsensiService.ConsensoRegistrato() =
                consenso.GetUltimiConsensiRegistrati(codiceAusiliarioPaziente, ContextInfos.CodiceAzienda, Settings.CONSENSO_APP_ID, codiceAziendaRegistrazione)

            If Not c Is Nothing Then listConsensi = c.Where(Function(p) Not p.Obsoleto).ToList()

        End Using

        If listConsensi Is Nothing OrElse listConsensi.Count = 0 Then Return Nothing

        If idConsenso.HasValue Then
            ' Consenso specificato => filtro per l'id del consenso
            listConsensi = listConsensi.Where(Function(p) p.ConsensoId = idConsenso).ToList()
        Else
            ' Consenso globale => filtro i soli consensi aventi il flag "UsaPerCalcoloStatoGlobale" a true
            listConsensi = listConsensi.Where(Function(p) p.UsaPerCalcoloStatoGlobale).ToList()
        End If

        ' Prendo i consensi con ordinamento max
        lst = listConsensi.Where(Function(p) p.StatoOrdinamento = listConsensi.Max(Function(m) m.StatoOrdinamento)).ToList()

        If lst IsNot Nothing AndAlso lst.Count > 0 Then
            statoConsenso = CreateStatoConsenso(lst.First())
        End If

        ' Innalzo il livello di controllo se l'ordinamento non rispetta il controllo richiesto dal paziente
        lst = listConsensi.Where(Function(p) ConvertiFlagControlloConsenso(p.FlagControllo) = Enumerators.ControlloConsenso.Bloccante).ToList()

        If lst IsNot Nothing AndAlso lst.Count > 0 Then

            If statoConsenso IsNot Nothing Then
                statoConsenso.Controllo = Enumerators.ControlloConsenso.Bloccante
            Else
                statoConsenso = CreateStatoConsenso(lst.First())
            End If

        Else

            lst = listConsensi.Where(Function(p) ConvertiFlagControlloConsenso(p.FlagControllo) = Enumerators.ControlloConsenso.Warning).ToList()

            If lst IsNot Nothing AndAlso lst.Count > 0 Then

                If statoConsenso IsNot Nothing Then
                    statoConsenso.Controllo = Enumerators.ControlloConsenso.Warning
                Else
                    statoConsenso = CreateStatoConsenso(lst.First())
                End If

            End If

        End If

        Return statoConsenso

    End Function

    ''' <summary>
    ''' Restituisce lo stato del consenso dei pazienti identificati dal codice centrale.
    ''' Lo stato è relativo al consenso di id specificato, se presente. Altrimenti, restituisce lo stato globale dei consensi del paziente.
    ''' Non utilizza il servizio del consenso ma effettua direttamente le stesse query su db.
    ''' </summary>
    ''' <param name="codiciAusiliariPazienti"></param>
    ''' <param name="codiceAziendaRegistrazione"></param>
    ''' <param name="listConsensiDefaultNonRilevato"></param>
    ''' <param name="idConsenso"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetStatoConsensoPazienti(codiciAusiliariPazienti As List(Of String), codiceAziendaRegistrazione As String, idConsenso As Integer?, listConsensiDefaultNonRilevato As List(Of Entities.Consenso.ConsensoRegistrato)) As List(Of Entities.Consenso.StatoConsensoPaziente)

        Dim statoConsensoPazienti As New List(Of Consenso.StatoConsensoPaziente)()

        ' Azienda di registrazione => se non specificata prendo quella associata all'app id corrente
        If String.IsNullOrWhiteSpace(codiceAziendaRegistrazione) Then
            codiceAziendaRegistrazione = GetCodiceUslByIdApplicazione(ContextInfos.IDApplicazione)
            If String.IsNullOrWhiteSpace(codiceAziendaRegistrazione) Then codiceAziendaRegistrazione = ContextInfos.CodiceAzienda
        End If

        ' Lista consensi del paziente. Contiene tutti i consensi:
        '   quelli registrati, con il valore più recente
        '   quelli non registrati, con il valore "non rilevato"
        ' Dalla lista vengono rimossi i consensi obsoleti e quelli non usati per il calcolo dello stato globale
        Dim listConsensi As List(Of Consenso.ConsensoRegistrato) =
            GetUltimiConsensiRegistrati(codiciAusiliariPazienti, idConsenso, listConsensiDefaultNonRilevato, codiceAziendaRegistrazione)

        If listConsensi Is Nothing OrElse listConsensi.Count = 0 Then Return Nothing

        For Each codiceAusiliario As String In codiciAusiliariPazienti

            Dim listConsensiPaziente As List(Of Consenso.ConsensoRegistrato) =
                listConsensi.Where(Function(p) p.CodicePaziente = codiceAusiliario).ToList()

            If Not listConsensiPaziente Is Nothing AndAlso listConsensiPaziente.Count > 0 Then

                Dim statoConsenso As Consenso.StatoConsensoPaziente = Nothing

                ' Prendo i consensi del paziente corrente con ordinamento max
                Dim lst As List(Of Consenso.ConsensoRegistrato) =
                    listConsensiPaziente.Where(Function(p) p.StatoOrdinamento = listConsensiPaziente.Max(Function(m) m.StatoOrdinamento)).ToList()

                If lst IsNot Nothing AndAlso lst.Count > 0 Then
                    statoConsenso = CreateStatoConsenso(lst.First())
                End If

                ' Innalzo il livello di controllo se l'ordinamento non rispetta il controllo richiesto dal paziente
                lst = listConsensiPaziente.Where(Function(p) ConvertiFlagControlloConsenso(p.FlagControllo) = Enumerators.ControlloConsenso.Bloccante).ToList()

                If lst IsNot Nothing AndAlso lst.Count > 0 Then

                    If statoConsenso IsNot Nothing Then
                        statoConsenso.Controllo = Enumerators.ControlloConsenso.Bloccante
                    Else
                        statoConsenso = CreateStatoConsenso(lst.First())
                    End If

                Else

                    lst = listConsensiPaziente.Where(Function(p) ConvertiFlagControlloConsenso(p.FlagControllo) = Enumerators.ControlloConsenso.Warning).ToList()

                    If lst IsNot Nothing AndAlso lst.Count > 0 Then

                        If statoConsenso IsNot Nothing Then
                            statoConsenso.Controllo = Enumerators.ControlloConsenso.Warning
                        Else
                            statoConsenso = CreateStatoConsenso(lst.First())
                        End If

                    End If

                End If

                statoConsensoPazienti.Add(statoConsenso)

            End If
        Next

        Return statoConsensoPazienti

    End Function

    ''' <summary>
    ''' Restituice i consensi di default nel caso "non rilevato"
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetConsensiDefaultNonRilevato() As List(Of Consenso.ConsensoRegistrato)

        Return GenericProviderCentrale.Paziente.GetConsensiLivelliDefaultNonRilevato(ContextInfos.CodiceAzienda)

    End Function

    Private Function GetUltimiConsensiRegistrati(listCodiciPazienti As List(Of String), idConsenso As Integer?, listConsensiDefault As List(Of Consenso.ConsensoRegistrato), codiceAziendaRegistrazione As String) As List(Of Consenso.ConsensoRegistrato)

        ' Lista consensi rilevati
        Dim listConsensi As List(Of Consenso.ConsensoRegistrato) = Nothing

        ' Lista consensi di default per i consensi senza rilevazione
        If listConsensiDefault Is Nothing Then
            listConsensiDefault = Me.GetConsensiDefaultNonRilevato()
        End If

        If idConsenso.HasValue Then
            listConsensi = GenericProviderCentrale.Paziente.GetUltimiConsensiRegistrati(listCodiciPazienti, idConsenso.Value, Nothing, codiceAziendaRegistrazione)
            listConsensiDefault = listConsensiDefault.Where(Function(p) p.ConsensoId = idConsenso.Value).ToList()
        Else
            listConsensi = GenericProviderCentrale.Paziente.GetUltimiConsensiRegistrati(listCodiciPazienti, Nothing, True, codiceAziendaRegistrazione)
            listConsensiDefault = listConsensiDefault.Where(Function(p) p.UsaPerCalcoloStatoGlobale).ToList()
        End If

        For Each codicePaziente As String In listCodiciPazienti

            Dim listConsensiPaziente As List(Of Entities.Consenso.ConsensoRegistrato) = listConsensi.Where(Function(p) p.CodicePaziente = codicePaziente).ToList()

            For i As Integer = 0 To listConsensiDefault.Count - 1

                Dim idDefault As Integer = listConsensiDefault(i).ConsensoId

                If Not listConsensiPaziente.Any(Function(p) p.ConsensoId = idDefault) Then

                    Dim added As Entities.Consenso.ConsensoRegistrato = listConsensiDefault(i).Clone()
                    added.CodicePaziente = codicePaziente
                    listConsensi.Add(added)

                End If

            Next
        Next

        Return listConsensi

    End Function

    Private Function CreateStatoConsenso(consenso As Entities.Consenso.ConsensoRegistrato) As Consenso.StatoConsensoPaziente

        Dim statoConsenso As New Consenso.StatoConsensoPaziente()

        statoConsenso.CodiceStatoConsenso = consenso.StatoIDIcona
        statoConsenso.DescrizioneStatoConsenso = consenso.StatoDescrizione
        statoConsenso.OrdinamentoStatoConsenso = consenso.StatoOrdinamento
        statoConsenso.UrlIconaStatoConsenso = consenso.StatoUrlIcona
        statoConsenso.Controllo = ConvertiFlagControlloConsenso(consenso.FlagControllo)
        statoConsenso.BloccoAccessiEsterni = consenso.BloccoAccessiEsterni

        Return statoConsenso

    End Function

    ''' <summary>
    ''' Restituisce il valore del flag di visibilità dei dati vaccinali, 
    ''' in base allo stato del consenso alla COMUNICAZIONE del paziente (recuperandolo in base al codice locale del paziente stesso)
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetValoreVisibilitaDatiVaccinaliPaziente(codicePaziente As Int64) As String

        Return GetValoreVisibilitaDatiVaccinaliPaziente(codicePaziente, True)

    End Function

    ''' <summary>
    ''' Restituisce il valore del flag di visibilità dei dati vaccinali.
    ''' Se il parametro controllaConsensoGlobale vale true, prende in considerazione solo lo stato del consenso alla COMUNICAZIONE del paziente 
    ''' recuperandolo in base al codice locale del paziente stesso.
    ''' Altrimenti, controlla lo stato di tutti i consensi (escludendo eventualmente quelli marcati come da non utilizzare per il calcolo dello stato globale).
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="controllaSoloConsensoComunicazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetValoreVisibilitaDatiVaccinaliPaziente(codicePaziente As Int64, controllaSoloConsensoComunicazione As Boolean) As String

        Dim valoreVisibilitaDatiVaccinaliPaziente As String = String.Empty
        Dim calcola As Boolean = False

        ' [Unificazione Ulss]: valorizzazione del flag visibilità per le nuove ulss non deve controllare i vecchi flag (che valgono "N")
        If UslGestitaCorrente Is Nothing OrElse UslGestitaCorrente.IsApplicazioneUnificata Then

            calcola = True

        Else

            If UslGestitaCorrente.FlagConsensoDatiVaccinaliCentralizzati Then
                calcola = True
            Else
                valoreVisibilitaDatiVaccinaliPaziente = Constants.ValoriVisibilitaDatiVaccinali.NegatoDaUsl
            End If

        End If

        If calcola Then

            Dim statoConsensoPaziente As Consenso.StatoConsensoPaziente = Nothing
            Dim listaValoriVisibilitaConcessa As List(Of String) = Nothing

            If controllaSoloConsensoComunicazione Then
                statoConsensoPaziente = GetConsensoComunicazionePaziente(codicePaziente)
                listaValoriVisibilitaConcessa = Settings.CONSENSO_VALORI_VISIBILITA_CONCESSA
            Else
                statoConsensoPaziente = GetConsensoGlobalePaziente(codicePaziente)
                listaValoriVisibilitaConcessa = Settings.CONSENSO_GLOBALE_VISIBILITA_CONCESSA
            End If

            valoreVisibilitaDatiVaccinaliPaziente = GetValoreVisibilitaDatiVaccinaliPaziente(statoConsensoPaziente.Controllo, listaValoriVisibilitaConcessa)

        End If

        Return valoreVisibilitaDatiVaccinaliPaziente

    End Function

    ''' <summary>
    ''' Restituisce il valore del flag di visibilità dei dati vaccinali in base allo stato del consenso del paziente.
    ''' Deve essere specificata anche la lista di valori del consenso per i quali il flag di visibilità va impostato a "Concesso".
    ''' </summary>
    ''' <param name="statoConsensoPaziente">Campo statoConsensoPaziente.Controllo</param>
    ''' <param name="listaValoriVisibilitaConcessa"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetValoreVisibilitaDatiVaccinaliPaziente(statoControlloConsensoPaziente As Enumerators.ControlloConsenso, listaValoriVisibilitaConcessa As List(Of String)) As String

        Dim flagControlloConsenso As String = GetValoreFlagControlloConsenso(statoControlloConsensoPaziente)

        Dim valoreVisibilita As String

        If Not listaValoriVisibilitaConcessa Is Nothing AndAlso listaValoriVisibilitaConcessa.Count > 0 Then

            ' Restituisce "Concesso" se il valore del flag è contenuto tra quelli specificati nella lista
            If listaValoriVisibilitaConcessa.Contains(flagControlloConsenso) Then
                valoreVisibilita = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente
            Else
                valoreVisibilita = Constants.ValoriVisibilitaDatiVaccinali.NegatoDaPaziente
            End If

        Else

            ' Restituisce "Concesso" solo se il controllo del consenso è non bloccante. 
            If statoControlloConsensoPaziente = Enumerators.ControlloConsenso.NonBloccante Then
                valoreVisibilita = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente
            Else
                valoreVisibilita = Constants.ValoriVisibilitaDatiVaccinali.NegatoDaPaziente
            End If

        End If

        Return valoreVisibilita

    End Function

    ''' <summary>
    ''' Restituisce true se il valore del flag di visibilità per il paziente è "CONCESSA", in base al consenso alla COMUNICAZIONE
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsVisibilitaCentraleDatiVaccinaliPaziente(codicePaziente As Integer) As Boolean

        ' Valorizzazione flag Visibilità in base a consenso del paziente
        Dim valoreVisibilitaDatiVaccinaliPaziente As String = GetValoreVisibilitaDatiVaccinaliPaziente(codicePaziente)

        Return (valoreVisibilitaDatiVaccinaliPaziente = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente)

    End Function

    ''' <summary>
    ''' Restituisce true se il consenso GLOBALE è concesso (in base ai valori contenuti nel parametro CONSENSO_GLOBALE_VISIBILITA_CONCESSA).
    ''' Se il consenso non è gestito, restituisce sempre true.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsVisibilitaConcessaByStatoConsensoGlobale(codicePaziente As Long) As Boolean

        If Not Settings.CONSENSO_GES Then Return True

        ' Controllo consenso globale del paziente
        Dim statoConsensoPaziente As Consenso.StatoConsensoPaziente = GetConsensoGlobalePaziente(codicePaziente)

        Dim valoreVisibilitaDatiVaccinaliPaziente As String = String.Empty

        If Not statoConsensoPaziente Is Nothing Then
            valoreVisibilitaDatiVaccinaliPaziente = GetValoreVisibilitaDatiVaccinaliPaziente(statoConsensoPaziente.Controllo, Settings.CONSENSO_GLOBALE_VISIBILITA_CONCESSA)
        End If

        Return (valoreVisibilitaDatiVaccinaliPaziente = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente)

    End Function

    ''' <summary>
    ''' Restituisce un nuovo oggetto StatoConsensoPaziente in base ai valori dei campo dell'oggetto ConsensoRegistrato
    ''' </summary>
    ''' <param name="consenso"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreateStatoConsenso(consenso As Onit.OnAssistnet.GestioneConsenso.Wcf.Proxy.ConsensiService.ConsensoRegistrato) As Consenso.StatoConsensoPaziente

        Dim statoConsenso As New Entities.Consenso.StatoConsensoPaziente()

        statoConsenso.CodiceStatoConsenso = consenso.StatoIDIcona
        statoConsenso.DescrizioneStatoConsenso = consenso.StatoDescrizione
        statoConsenso.OrdinamentoStatoConsenso = consenso.StatoOrdinamento
        statoConsenso.UrlIconaStatoConsenso = consenso.StatoUrlIcona
        statoConsenso.Controllo = ConvertiFlagControlloConsenso(consenso.FlagControllo)
        statoConsenso.BloccoAccessiEsterni = consenso.BloccoAccessiEsterni

        Return statoConsenso

    End Function

    Private Function ConvertiFlagControlloConsenso(valore As String) As Enumerators.ControlloConsenso

        If Not String.IsNullOrEmpty(valore) Then

            Select Case valore
                Case "E"
                    Return Enumerators.ControlloConsenso.Bloccante
                Case "W"
                    Return Enumerators.ControlloConsenso.Warning
                Case "N"
                    Return Enumerators.ControlloConsenso.NonBloccante
            End Select

        End If

        Return Enumerators.ControlloConsenso.Undefined

    End Function

    Private Function GetValoreFlagControlloConsenso(flagControlloConsenso As Enumerators.ControlloConsenso) As String

        Select Case flagControlloConsenso
            Case Enumerators.ControlloConsenso.Bloccante
                Return "E"
            Case Enumerators.ControlloConsenso.Warning
                Return "W"
            Case Enumerators.ControlloConsenso.NonBloccante
                Return "N"
        End Select

        Return String.Empty

    End Function

    ''' <summary>
    ''' Restituisce la lista con tutti i consensi registrati (del tipo specificato) per il paziente. 
    ''' Restituisce i consensi centralizzati e quelli non centralizzati registrati dall'azienda specificata.
    ''' Non restituisce i consensi non rilevati (perchè non legge dalla v_paz_ultimo_consenso ma dalla v_paz_consensi).
    ''' </summary>
    ''' <param name="codiceCentralePaziente"></param>
    ''' <param name="listIdConsensi"></param>
    ''' <param name="codiceAziendaRegistrazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetConsensiRegistratiPazienteAzienda(codiceCentralePaziente As String, listIdConsensi As List(Of Integer), codiceAziendaRegistrazione As String) As List(Of GetUltimoConsensoRegistratoResult)

        Dim resultList As New List(Of GetUltimoConsensoRegistratoResult)()

        Dim listUltimiConsensi As List(Of Consenso.ConsensoRegistrato) =
            GenericProviderCentrale.Paziente.GetConsensiRegistratiPazienteAzienda(codiceCentralePaziente, listIdConsensi, codiceAziendaRegistrazione)

        If Not listUltimiConsensi Is Nothing AndAlso listUltimiConsensi.Count > 0 Then

            For Each consenso As Consenso.ConsensoRegistrato In listUltimiConsensi

                Dim result As New GetUltimoConsensoRegistratoResult()

                result.UltimoConsenso = consenso.Clone()

                If result.UltimoConsenso Is Nothing Then
                    result.StatoControlloConsenso = Enumerators.ControlloConsenso.Undefined
                Else
                    result.StatoControlloConsenso = ConvertiFlagControlloConsenso(result.UltimoConsenso.FlagControllo)
                End If

                resultList.Add(result)

            Next

        End If

        Return resultList

    End Function

    ''' <summary>
    ''' Restituisce la descrizione dei consensi e dei livelli che verranno impostati in automatico, specificati nel parametro CONSENSO_ID_AUTORILEVAZIONE
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDescrizioneConsensiRilevazioneAutomatica() As String

        If Me.Settings.CONSENSO_ID_AUTORILEVAZIONE Is Nothing OrElse Me.Settings.CONSENSO_ID_AUTORILEVAZIONE.Count = 0 Then
            Return "Nessun consenso configurato per la rilevazione automatica"
        End If

        Dim message As New System.Text.StringBuilder()

        message.AppendLine("Premendo OK, verranno rilevati i seguenti consensi (se non già positivi):")

        For Each consenso As Entities.Consenso.ConsensoAutoRilevazione In Me.Settings.CONSENSO_ID_AUTORILEVAZIONE

            Dim descrizioneConsenso As String =
                Me.GenericProviderCentrale.Paziente.GetDescrizioneConsensoByIdConsenso(consenso.IdConsenso)

            Dim descrizioneLivello As String =
                Me.GenericProviderCentrale.Paziente.GetDescrizioneLivelloConsensoByIdLivello(consenso.IdLivelloConsenso)

            message.AppendLine()
            message.AppendFormat("  - {0}", descrizioneConsenso)
            message.AppendLine()
            message.AppendFormat("    che verrà impostato al valore ""{0}""", descrizioneLivello)
            message.AppendLine()

        Next

        message.AppendLine()
        message.Append("Continuare?")

        Return message.ToString()

    End Function

    Private Class ValoriCampiCustom

        Public IdConsenso As Integer
        Public CampoCustom1 As String
        Public CampoCustom2 As String
        Public CampoCustom3 As String

        Public Sub New(idConsenso As Integer)
            Me.New(idConsenso, String.Empty, String.Empty, String.Empty)
        End Sub

        Public Sub New(idConsenso As Integer, campoCustom1 As String, campoCustom2 As String, campoCustom3 As String)
            Me.IdConsenso = idConsenso
            Me.CampoCustom1 = campoCustom1
            Me.CampoCustom2 = campoCustom2
            Me.CampoCustom3 = campoCustom3
        End Sub

    End Class

    ''' <summary>
    ''' Inserimento automatico dei consensi indicati nel parametro CONSENSO_ID_AUTORILEVAZIONE, per il paziente specificato.
    ''' Verranno inseriti solo i consensi in uno stato diverso da NON BLOCCANTE.
    ''' </summary>
    ''' <param name="codiceCentralePaziente"></param>
    ''' <param name="codiceLocalePaziente"></param>
    ''' <param name="codiceAziendaRegistrazione"></param>
    ''' <remarks></remarks>
    Public Sub RilevazioneAutomaticaConsensi(codiceCentralePaziente As String, codiceLocalePaziente As String, codiceAziendaRegistrazione As String)

        If String.IsNullOrWhiteSpace(codiceAziendaRegistrazione) Then Throw New ArgumentNullException("codiceAziendaRegistrazione deve essere valorizzato.")

        If Settings.CONSENSO_ID_AUTORILEVAZIONE.IsNullOrEmpty() Then
            Return
        End If

        Dim listIdConsensi As List(Of Integer) = Settings.CONSENSO_ID_AUTORILEVAZIONE.Select(Function(p) p.IdConsenso).ToList()

        ' Lettura lista consensi del paziente. Viene restituito l'ultimo consenso di ogni tipo specificato. 
        ' Se non ancora registrato, viene restituito un consenso di default (senza progressivo).
        Dim listConsensiRegistratiResult As List(Of GetUltimoConsensoRegistratoResult) =
            GetConsensiRegistratiPazienteAzienda(codiceCentralePaziente, listIdConsensi, codiceAziendaRegistrazione)

        ' Accoppiate id-campo custom. Id consenso da inserire, campo custom relativo all'ultimo consenso inserito dello stesso tipo.
        Dim idConsensidaRegistrare As New List(Of ValoriCampiCustom)()

        If listConsensiRegistratiResult.IsNullOrEmpty() Then

            ' Nessun consenso già registrato tra quelli specificati => devo inserire tutti i consensi specificati
            For Each idConsenso As Integer In listIdConsensi
                idConsensidaRegistrare.Add(New ValoriCampiCustom(idConsenso))
            Next

        Else

            For Each idConsenso As Integer In listIdConsensi

                Dim listConsensiRegistratiById As IEnumerable(Of Consenso.ConsensoRegistrato) = listConsensiRegistratiResult.
                    Where(Function(p) p.UltimoConsenso.ConsensoId = idConsenso And
                         (p.UltimoConsenso.Centralizzato Or (Not p.UltimoConsenso.Centralizzato And p.UltimoConsenso.CodiceAziendaRegistrazione = codiceAziendaRegistrazione))).
                    Select(Function(p) p.UltimoConsenso)

                If listConsensiRegistratiById.IsNullOrEmpty() Then

                    ' Non è stato registrato un consenso con l'id corrente => consenso da registrare
                    idConsensidaRegistrare.Add(New ValoriCampiCustom(idConsenso))

                Else

                    ' E' stato registrato un consenso con l'id corrente. Se l'ultimo registrato è diverso da non bloccante => consenso da registrare
                    Dim itemMax As Consenso.ConsensoRegistrato =
                        listConsensiRegistratiById.First(Function(p) p.Progressivo = listConsensiRegistratiById.Max(Function(x) x.Progressivo))

                    If listConsensiRegistratiResult.Any(
                        Function(p) p.UltimoConsenso.Progressivo = itemMax.Progressivo And p.StatoControlloConsenso <> Enumerators.ControlloConsenso.NonBloccante) Then

                        idConsensidaRegistrare.Add(New ValoriCampiCustom(idConsenso, itemMax.CampoCustom1, itemMax.CampoCustom2, itemMax.CampoCustom3))

                    End If

                End If

            Next

        End If

        ' Per ogni consenso della lista inserisco un consenso del livello indicato
        If Not idConsensidaRegistrare.IsNullOrEmpty() Then

            '---
            ' N.B. : eseguo un progressivo a vuoto sul consenso perchè, se l'ultimo progressivo lo ha calcolato la libreria di gestione consenso, 
            '        lo ha lasciato indietro di 1 perchè ragiona diversamente rispetto al progressivo di onvac
            '---
            Using bizProgressivo As New BizProgressivi(GenericProviderCentrale, Settings, ContextInfos, LogOptions)
                bizProgressivo.CalcolaProgressivo(Constants.TipoProgressivo.Consenso, True)
            End Using
            '---

            Dim now As DateTime = DateTime.Now
            Dim ute As Onit.Shared.Manager.Entities.T_ANA_UTENTI =
                Onit.Shared.Manager.Security.UserDbManager.GetUser(ContextInfos.IDUtente)


            ' N.B. : non lo eseguo in transazione. Se dovesse servire, il calcolo del progressivo dà dei problemi di transazioni parallele => serve il transactionscope?

            Dim listIdConsensiDaRegistrare As List(Of Integer) = idConsensidaRegistrare.Select(Function(p) p.IdConsenso).ToList()

            For Each idConsensoDaRegistrare As Integer In listIdConsensiDaRegistrare

                ' ---
                ' Registrazione del consenso al livello indicato
                ' ---
                Dim idLivello As Integer = Settings.CONSENSO_ID_AUTORILEVAZIONE.
                    Where(Function(p) p.IdConsenso = idConsensoDaRegistrare).
                    Select(Function(item) item.IdLivelloConsenso).First()

                Dim consensoPaziente As New Consenso.ConsensoPaziente()

                Using bizProgressivo As New BizProgressivi(GenericProviderCentrale, Settings, ContextInfos, LogOptions)
                    consensoPaziente.ProgressivoConsenso = bizProgressivo.CalcolaProgressivo(Constants.TipoProgressivo.Consenso, True)
                End Using

                consensoPaziente.IdLivello = idLivello
                consensoPaziente.IdEnte = GenericProviderCentrale.Paziente.GetIdEnteDefaultConsenso(idConsensoDaRegistrare)
                consensoPaziente.Addetto = String.Format("rilevazione auto ({0})", ute.UTE_CODICE)
                consensoPaziente.Applicativo = ApplicazioneCentrale.Id
                consensoPaziente.Azienda = ApplicazioneCentrale.AziCodice
                consensoPaziente.CodiceAzienda = codiceAziendaRegistrazione
                consensoPaziente.DataEvento = now
                consensoPaziente.DataRegistrazione = now
                consensoPaziente.DataScadenza = Nothing
                consensoPaziente.CodicePaziente = codiceCentralePaziente

                Dim valoriCustom As ValoriCampiCustom = idConsensidaRegistrare.First(Function(p) p.IdConsenso = idConsensoDaRegistrare)
                If Not valoriCustom Is Nothing Then
                    consensoPaziente.CampoCustom1 = valoriCustom.CampoCustom1
                    consensoPaziente.CampoCustom2 = valoriCustom.CampoCustom2
                    consensoPaziente.CampoCustom3 = valoriCustom.CampoCustom3
                End If

                ' Insert consenso con il livello registrato
                GenericProviderCentrale.Paziente.InsertConsenso(consensoPaziente)

                ' ---
                ' Aggiornamento dati centrali (solo in caso di modifica del consenso alla comunicazione)
                ' ---
                If idConsensoDaRegistrare = Settings.CONSENSO_ID_COMUNICAZIONE Then

                    Dim flagVisibilitaDatiVaccinaliCentrali As String = Nothing

                    Dim tipoEventoLivello As String = Me.GenericProviderCentrale.Paziente.GetTipoEventoByIdLivelloConsenso(idLivello)

                    Select Case Onit.OnAssistnet.Contracts.Converter.ConvertiTipoEvento(tipoEventoLivello)

                        Case Onit.OnAssistnet.Contracts.TipoEvento.NEGAZIONECONSENSO,
                             Onit.OnAssistnet.Contracts.TipoEvento.REVOCACONSENSO,
                             Onit.OnAssistnet.Contracts.TipoEvento.REVOCACONCANCELLAZIONECONSENSO

                            flagVisibilitaDatiVaccinaliCentrali = Onit.OnAssistnet.OnVac.Constants.ValoriVisibilitaDatiVaccinali.NegatoDaPaziente

                        Case Onit.OnAssistnet.Contracts.TipoEvento.RILASCIOCONSENSO

                            flagVisibilitaDatiVaccinaliCentrali = Onit.OnAssistnet.OnVac.Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente

                        Case Else

                            Throw New NotImplementedException()

                    End Select


                    If Settings.UNIFICAZIONE_IN_CORSO Then
                        ' ------------------------------------ '
                        ' [Unificazione Ulss]: L'aggiornamento della visibilità nei db locali delle varie ULSS deve rimanere finchè tutte le ulss saranno unificate
                        ' ------------------------------------ '
                        AggiornaVisibilitaDatiVaccinaliCentrali(
                        codiceCentralePaziente,
                        flagVisibilitaDatiVaccinaliCentrali,
                        IIf(flagVisibilitaDatiVaccinaliCentrali = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente,
                            Constants.ValoriVisibilitaDatiVaccinali.NegatoDaPaziente,
                            Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente))
                        ' ------------------------------------ '
                    End If


                    ' ------------------------------------ '
                    ' [Unificazione Ulss]: in ogni caso, bisogna fare l'update del flag visibilità (+ ute e data variazione) per tutte le Eseguite, Scadute, Visite ed Escluse del paziente
                    ' ------------------------------------ '
                    Using bizVacEseguite As New BizVaccinazioniEseguite(GenericProvider, Settings, ContextInfos, LogOptions)

                        Dim listIdVaccinazioniEseguite As New List(Of Long)()
                        Dim listIdVaccinazioniScadute As New List(Of Long)()

                        Dim dt_vacEseguite As DataTable = bizVacEseguite.GetVaccinazioniEseguite(codiceLocalePaziente, False)
                        If Not dt_vacEseguite Is Nothing Then
                            For i As Int16 = dt_vacEseguite.Rows.Count - 1 To 0 Step -1
                                If dt_vacEseguite.Rows(i)("scaduta").ToString() = "N" Then
                                    listIdVaccinazioniEseguite.Add(Convert.ToInt64(dt_vacEseguite.Rows(i)("ves_id").ToString()))
                                Else
                                    listIdVaccinazioniScadute.Add(Convert.ToInt64(dt_vacEseguite.Rows(i)("ves_id").ToString()))
                                End If
                            Next
                        End If

                        bizVacEseguite.UpdateFlagVisibilita(codiceLocalePaziente, listIdVaccinazioniEseguite, listIdVaccinazioniScadute, flagVisibilitaDatiVaccinaliCentrali)

                    End Using

                    Using bizVacEscluse As New BizVaccinazioniEscluse(GenericProvider, Settings, ContextInfos, LogOptions)

                        Dim listIdVaccinazioniEscluse As New List(Of Long)()

                        Dim dt_vacEscluse As DataTable = bizVacEscluse.GetVaccinazioniEscluse(codiceLocalePaziente, False)
                        If Not dt_vacEscluse Is Nothing Then
                            For i As Int16 = dt_vacEscluse.Rows.Count - 1 To 0 Step -1
                                listIdVaccinazioniEscluse.Add(Convert.ToInt64(dt_vacEscluse.Rows(i)("vex_id").ToString()))
                            Next
                        End If

                        bizVacEscluse.UpdateFlagVisibilita(codiceLocalePaziente, listIdVaccinazioniEscluse, flagVisibilitaDatiVaccinaliCentrali)

                    End Using

                    Using bizVisite As New BizVisite(GenericProvider, Settings, ContextInfos, LogOptions)

                        Dim listVisite As List(Of Visita) = bizVisite.GetVisitePaziente(codiceLocalePaziente)
                        If Not listVisite Is Nothing AndAlso listVisite.Count > 0 Then
                            For Each visita As Visita In listVisite
                                bizVisite.UpdateFlagVisibilita(visita.IdVisita, flagVisibilitaDatiVaccinaliCentrali)
                            Next
                        End If

                    End Using

                End If

            Next
        End If

    End Sub

#End Region

#Region " Situazione Vaccinale "

    ''' <summary>
    ''' Restituisce il testo relativo all'esito del controllo sulla situazione vaccinale del paziente
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    Public Function GetEsitoControlloSituazioneVaccinale(codicePaziente As Long) As String

        Return GenericProvider.Paziente.GetEsitoControlloSituazioneVaccinale(codicePaziente)

    End Function

#End Region

#Region " Public / Protected Overridable "

#Region " StatoAnagrafico "

    ''' <summary>
    ''' Imposta lo stato anagrafico al paziente in base ai dati specificati
    ''' </summary>
    ''' <param name="pazienteModificato"></param>
    ''' <param name="pazienteOriginale"></param>
    ''' <remarks></remarks>
    Protected Overridable Sub SetCodiceStatoAnagrafico(pazienteModificato As Entities.Paziente, pazienteOriginale As Entities.Paziente)

        If pazienteOriginale Is Nothing Then

            ' Calcolo stato anagrafico in caso di inserimento

            If Me.Settings.GES_AUTO_STATO_ANAGRAFICO Then

                pazienteModificato.StatoAnagrafico = Me.CalcolaStatoAnagrafico(pazienteModificato, pazienteOriginale)

                ' Se il parametro di sostituzione è valorizzato, 
                ' sostituisco lo stato "IMMIGRATO" con quello indicato nel parametro.
                If pazienteModificato.StatoAnagrafico = Enumerators.StatoAnagrafico.IMMIGRATO AndAlso
                   Not Me.Settings.AUTO_STATO_ANAG_SOSTITUZIONE_IMMIGRATO Is Nothing Then

                    pazienteModificato.StatoAnagrafico = Me.Settings.AUTO_STATO_ANAG_SOSTITUZIONE_IMMIGRATO

                End If

            Else

                ' Se il parametro di calcolo automatico è impostato a false, assegno lo stato anagrafico di default.
                pazienteModificato.StatoAnagrafico = Me.GetStatoAnagraficoDefault()

            End If

        Else

            ' Calcolo stato anagrafico in caso di modifica

            ' Se la data di decesso è valorizzata, lo stato anagrafico deve sempre essere impostato a DECEDUTO.
            If pazienteModificato.DataDecesso > DateTime.MinValue Then

                pazienteModificato.StatoAnagrafico = Enumerators.StatoAnagrafico.DECEDUTO

            Else

                ' Il ricalcolo dello stato anagrafico avviene:
                ' 1 - se il parametro di calcolo automatico dello stato anagrafico è True, e
                ' 2 - se non deve essere controllato il flag "Locale", oppure se non vale "S" (pazienti non marcati come locali)
                If Me.Settings.GES_AUTO_STATO_ANAGRAFICO And
                   (Not Me.Settings.AUTO_STATO_ANAG_CHECK_LOCALE OrElse pazienteModificato.FlagLocale <> "S") Then

                    ' Calcola lo stato anagrafico in base ai nuovi dati del paziente
                    pazienteModificato.StatoAnagrafico = Me.CalcolaStatoAnagrafico(pazienteModificato, pazienteOriginale)

                    If pazienteModificato.StatoAnagrafico = Enumerators.StatoAnagrafico.IMMIGRATO Then

                        ' Se il paz è IMMIGRATO, potrebbe essere stato regolarizzato 
                        ' senza che risulti regolarizzato in centrale.
                        If pazienteModificato.ComuneProvenienza_Codice <> pazienteOriginale.ComuneProvenienza_Codice OrElse
                           pazienteModificato.ComuneEmigrazione_Codice <> pazienteOriginale.ComuneEmigrazione_Codice OrElse
                           pazienteModificato.ComuneDomicilio_Codice <> pazienteOriginale.ComuneDomicilio_Codice Then

                            ' Se uno dei campi che influisce sull'immigrazione è variato rispetto al precedente valore, 
                            ' assegno al paziente lo stato anagrafico IMMIGRATO (o il suo sostituto impostato nel parametro).
                            If Not Me.Settings.AUTO_STATO_ANAG_SOSTITUZIONE_IMMIGRATO Is Nothing Then

                                pazienteModificato.StatoAnagrafico = Me.Settings.AUTO_STATO_ANAG_SOSTITUZIONE_IMMIGRATO

                            End If

                        Else

                            ' Se i campi presi in considerazione non sono variati, 
                            ' lo stato anagrafico non varia rispetto al precedente valore.
                            pazienteModificato.StatoAnagrafico = pazienteOriginale.StatoAnagrafico

                        End If

                    ElseIf pazienteModificato.StatoAnagrafico = Enumerators.StatoAnagrafico.RESIDENTE AndAlso
                        pazienteModificato.ComuneResidenza_Codice = pazienteOriginale.ComuneResidenza_Codice AndAlso
                        pazienteModificato.ComuneDomicilio_Codice = pazienteOriginale.ComuneDomicilio_Codice Then

                        ' Nel caso in cui lo stato anagrafico ricalcolato è RESIDENTE ma non sono cambiati i dati di residenza e domicilio,
                        ' devo ripristinare lo stato anagrafico originale, in tutti i casi tranne i seguenti:
                        ' 1 - quando lo stato anagrafico originale era DECEDUTO e la data di decesso è stata sbiancata rispetto all'originale
                        ' 2 - quando lo stato anagrafico originale era IRREPERIBILE, il comune di emigrazione originale era sconosciuto,
                        '     la data di emigrazione era valorizzata e inferiore alla data di immigrazione del paziente modificato.

                        If Not (pazienteOriginale.StatoAnagrafico = Enumerators.StatoAnagrafico.DECEDUTO AndAlso
                                pazienteOriginale.DataDecesso <> DateTime.MinValue AndAlso
                                pazienteModificato.DataDecesso = DateTime.MinValue) AndAlso
                           Not (pazienteOriginale.StatoAnagrafico = Enumerators.StatoAnagrafico.IRREPERIBILE AndAlso
                                pazienteOriginale.ComuneEmigrazione_Codice = Me.Settings.COMUNE_SCONOSCIUTO AndAlso
                                pazienteOriginale.DataEmigrazione > DateTime.MinValue) Then

                            ' Se al paziente era stata valorizzata la data di decesso, poi è stata sbiancata, oppure se era irreperibile
                            ' viene mantenuto lo stato anagrafico ricalcolato.
                            ' Altrimenti, se il paziente risulta RESIDENTE in base ai dati modificati
                            ' ma non sono cambiati nè la residenza nè il domicilio: non cambio lo stato anagrafico.
                            ' Questo dovrebbe coprire i casi in cui lo stato anagrafico è stato variato a mano dagli utenti.

                            pazienteModificato.StatoAnagrafico = pazienteOriginale.StatoAnagrafico

                        End If

                    End If

                Else

                    ' Lo stato anagrafico rimane invariato se il parametro GES_AUTO_STATO_ANAGRAFICO è false, 
                    ' oppure se deve essere controllato il flag locale e il flag vale "S".
                    pazienteModificato.StatoAnagrafico = pazienteOriginale.StatoAnagrafico

                End If

            End If

        End If

    End Sub

    ''' <summary>
    ''' Calcola lo stato anagrafico del paziente in base ai dati specificati
    ''' </summary>
    ''' <param name="paziente"></param>
    ''' <param name="pazienteOriginale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overridable Function CalcolaStatoAnagrafico(paziente As Paziente, pazienteOriginale As Paziente) As Nullable(Of Enumerators.StatoAnagrafico)

        ' Deceduto
        If paziente.DataDecesso > DateTime.MinValue Then

            Return Enumerators.StatoAnagrafico.DECEDUTO

        End If

        Dim comuneDomicilio As String = paziente.ComuneDomicilio_Codice

        ' Irreperibile
        ' Il paziente è IRREPERIBILE se:
        ' 1- risulta emigrato in un comune sconosciuto e
        ' 2- le date di emigrazione e immigrazione non sono valorizzate oppure è presente una data di emigrazione successiva all'immigrazione
        If paziente.ComuneEmigrazione_Codice = Me.Settings.COMUNE_SCONOSCIUTO AndAlso
           ((paziente.DataEmigrazione = DateTime.MinValue AndAlso paziente.DataImmigrazione = DateTime.MinValue) OrElse
             paziente.DataEmigrazione > paziente.DataImmigrazione) Then

            Return Enumerators.StatoAnagrafico.IRREPERIBILE

        End If

        ' Controllo Emigrazione/Immigrazione

        ' Ricerca asl di emigrazione
        Dim aslEmigrazione As String = Me.GetCodiceAslByComune(paziente.ComuneEmigrazione_Codice)

        ' Ricerca asl di immigrazione
        Dim aslImmigrazione As String = Me.GetCodiceAslByComune(paziente.ComuneProvenienza_Codice)

        ' Controllo la presenza di entrambe le date di immigrazione/emigrazione:
        ' se mancano entrambe, ma c'è solo uno tra i due comuni (o immmigrazione o emigrazione), 
        ' assegno lo stato anagrafico relativo.
        If paziente.DataEmigrazione = DateTime.MinValue AndAlso paziente.DataImmigrazione = DateTime.MinValue Then

            If Not String.IsNullOrEmpty(paziente.ComuneEmigrazione_Codice) AndAlso
               String.IsNullOrEmpty(paziente.ComuneProvenienza_Codice) Then

                ' Controllo emigrazione:
                ' assegno lo stato emigrato solo se la asl di emigrazione è vuota o diversa da quella di lavoro
                If String.IsNullOrEmpty(aslEmigrazione) OrElse aslEmigrazione <> ContextInfos.CodiceUsl Then

                    Return Enumerators.StatoAnagrafico.EMIGRATO

                End If

            ElseIf Not String.IsNullOrEmpty(paziente.ComuneProvenienza_Codice) AndAlso
                String.IsNullOrEmpty(paziente.ComuneEmigrazione_Codice) Then

                ' Controllo immigrazione:
                ' assegno lo stato immigrato solo se la asl di immigrazione è vuota o diversa da quella di lavoro
                If String.IsNullOrEmpty(aslImmigrazione) OrElse aslImmigrazione <> ContextInfos.CodiceUsl Then

                    Return Enumerators.StatoAnagrafico.IMMIGRATO

                End If

            End If

        Else
            ' Almeno una data valorizzata.

            ' Emigrato
            ' Il paziente è EMIGRATO se:
            ' 1- la data di emigrazione è più recente rispetto a quella di immigrazione, e
            ' 2- la asl di emigrazione è nulla oppure diversa dalla asl di lavoro (comune di emigrazione fuori asl)
            If paziente.DataEmigrazione > paziente.DataImmigrazione AndAlso
               (String.IsNullOrEmpty(aslEmigrazione) OrElse aslEmigrazione <> ContextInfos.CodiceUsl) Then

                Return Enumerators.StatoAnagrafico.EMIGRATO

            End If

            ' Immigrato
            ' Il paziente è IMMIGRATO se:
            ' 1- la data di immigrazione è più recente rispetto a quella di emigrazione, e
            ' 2- la asl di immigrazione è nulla oppure diversa dalla asl di lavoro (comune di immigrazione fuori asl)
            If paziente.DataImmigrazione > paziente.DataEmigrazione AndAlso
               (String.IsNullOrEmpty(aslImmigrazione) OrElse aslImmigrazione <> ContextInfos.CodiceUsl) Then

                Return Enumerators.StatoAnagrafico.IMMIGRATO

            End If

        End If

        ' Ricerca asl di residenza
        Dim aslComuneResidenza As String = Me.GetCodiceAslByComune(paziente.ComuneResidenza_Codice)

        ' Ricerca asl di domicilio
        Dim aslComuneDomicilio As String = Me.GetCodiceAslByComune(paziente.ComuneDomicilio_Codice)

        ' Residente domiciliato fuori usl
        ' Il paziente è RESIDENTE DOMICILIATO FUORI USL se:
        ' 1- la asl relativa al comune di residenza è valorizzata ed è uguale alla asl di lavoro, e
        ' 2- la asl di assistenza è valorizzata e diversa dalla asl di lavoro, oppure 
        '    la asl di assistenza è nulla, la asl associata al comune di domicilio è valorizzata ed è diversa dalla asl di lavoro
        If Not String.IsNullOrEmpty(aslComuneResidenza) AndAlso aslComuneResidenza = ContextInfos.CodiceUsl AndAlso
           ((Not String.IsNullOrEmpty(paziente.UslAssistenza_Codice) AndAlso paziente.UslAssistenza_Codice <> ContextInfos.CodiceUsl) OrElse
            (String.IsNullOrEmpty(paziente.UslAssistenza_Codice) AndAlso Not String.IsNullOrEmpty(aslComuneDomicilio) AndAlso aslComuneDomicilio <> ContextInfos.CodiceUsl)) Then

            Return Enumerators.StatoAnagrafico.RESIDENTE_DOMICILIATO_FUORI_USL

        End If

        ' Residente
        ' Il paziente è RESIDENTE se:
        ' 1- la asl relativa al comune di residenza è valorizzata ed è uguale alla asl di lavoro, e
        ' 2- la asl relativa al comune di domicilio è nulla, oppure è valorizzata ed è uguale alla asl di lavoro, oppure
        '    la asl di assistenza è nulla, oppure è valorizzata ed è uguale alla asl corrente
        If Not String.IsNullOrEmpty(aslComuneResidenza) AndAlso aslComuneResidenza = ContextInfos.CodiceUsl AndAlso
           (String.IsNullOrEmpty(aslComuneDomicilio) OrElse aslComuneDomicilio = ContextInfos.CodiceUsl OrElse
            String.IsNullOrEmpty(paziente.UslAssistenza_Codice) OrElse paziente.UslAssistenza_Codice = ContextInfos.CodiceUsl) Then

            Return Enumerators.StatoAnagrafico.RESIDENTE

        End If

        ' Domiciliato
        ' Il paziente è DOMICILIATO se:
        ' 1- la asl relativa al comune di residenza è nulla, oppure è valorizzata e diversa dalla asl di lavoro, e
        ' 2- la asl relativa al domicilio è valorizzata e uguale alla asl di lavoro, oppure
        '    la asl di assistenza è valorizzata ed uguale alla asl di lavoro
        If (String.IsNullOrEmpty(aslComuneResidenza) OrElse aslComuneResidenza <> ContextInfos.CodiceUsl) AndAlso
           ((Not String.IsNullOrEmpty(aslComuneDomicilio) AndAlso aslComuneDomicilio = ContextInfos.CodiceUsl) OrElse
            (Not String.IsNullOrEmpty(paziente.UslAssistenza_Codice) AndAlso paziente.UslAssistenza_Codice = ContextInfos.CodiceUsl)) Then

            Return Enumerators.StatoAnagrafico.DOMICILIATO

        End If

        ' Non residente non domiciliato
        ' Se tutti i controlli precedenti non sono andati a buon fine, il paziente non è nè residente nè domiciliato.
        Return Enumerators.StatoAnagrafico.NON_RESIDENTE_NON_DOMICILIATO

    End Function

    ''' <summary>
    ''' Restituisce l'enumerazione corrispondente allo stato anagrafico di default letto da db
    ''' </summary>
    ''' <returns></returns>
    Protected Overridable Function GetStatoAnagraficoDefault() As Enumerators.StatoAnagrafico

        Dim statoAnag As String = Me.GenericProvider.StatiAnagrafici.GetStatoAnagraficoDefault()

        Return DirectCast([Enum].Parse(GetType(Enumerators.StatoAnagrafico), statoAnag, True), Enumerators.StatoAnagrafico)

    End Function


    ''' <summary>
    ''' Calcola lo stato anagrafico del paziente in base ai dati specificati (comune di residenza, comune di domicilio e usl assistenza)
    ''' </summary>
    ''' <param name="paziente"></param>
    ''' <returns></returns>
    Public Function CalcolaStatoAnagrafico(paziente As Paziente) As Nullable(Of Enumerators.StatoAnagrafico)

        Dim residenteInRegione As Boolean = False
        Dim domiciliatoInRegione As Boolean = False

        Using bizComuni As New BizComuni(GenericProvider, Settings, ContextInfos, Nothing)

            residenteInRegione = bizComuni.IsComuneInRegione(paziente.ComuneResidenza_Codice)
            domiciliatoInRegione = bizComuni.IsComuneInRegione(paziente.ComuneDomicilio_Codice)

        End Using

        ' Residenza in Regione e domicilio assente o in Regione => RESIDENTE o IMMIGRATO
        ' Il paziente risulta IMMIGRATO se l'età, in giorni, è all'interno dell'intervallo di regolarizzazione specificato. Altrimenti è RESIDENTE.
        If (residenteInRegione AndAlso (String.IsNullOrWhiteSpace(paziente.ComuneDomicilio_Codice) OrElse domiciliatoInRegione)) Then

            If paziente.Data_Nascita > DateTime.MinValue AndAlso
               paziente.DataInserimento.Subtract(paziente.Data_Nascita).Days >= Settings.NUM_GIORNI_REGOLARIZZAZIONE AndAlso
               paziente.DataInserimento.Subtract(paziente.Data_Nascita).Days <= Settings.NUM_MAX_GIORNI_REGOLARIZZAZIONE Then

                Return Enumerators.StatoAnagrafico.IMMIGRATO

            Else

                Return Enumerators.StatoAnagrafico.RESIDENTE

            End If

        End If

        ' Non residente in Regione e domicilio in Regione => DOMICILIATO
        If (Not residenteInRegione AndAlso domiciliatoInRegione) Then
            Return Enumerators.StatoAnagrafico.DOMICILIATO
        End If

        ' Non residente in Regione, non domiciliato in Regione ma con usl assistenza della regione => NON RES NON DOM ASSISTITO
        If (Not String.IsNullOrWhiteSpace(paziente.UslAssistenza_Codice)) Then

            Using bizUsl As New BizUsl(GenericProvider, ContextInfos)

                Dim codiceRegioneUslAssistenza As String = bizUsl.GetCodiceRegione(paziente.UslAssistenza_Codice)
                If (codiceRegioneUslAssistenza = Settings.CODICE_REGIONE) Then
                    Return Enumerators.StatoAnagrafico.NON_RESIDENTE_NON_DOMICILIATO_ASSISTITO
                End If

            End Using

        End If

        ' NON RESIDENTE NON DOMICILIATO
        ' Se tutti i controlli precedenti non sono andati a buon fine, il paziente non è nè residente nè domiciliato.
        Return Enumerators.StatoAnagrafico.NON_RESIDENTE_NON_DOMICILIATO

    End Function

#End Region

#Region " CNS "

    ''' <summary>
    ''' Imposta il consultorio vaccinale del paziente
    ''' </summary>
    ''' <param name="paziente"></param>
    ''' <param name="pazienteOriginale"></param>
    ''' <remarks></remarks>
    Protected Overridable Sub SetCodiceCNS(paziente As Entities.Paziente, pazienteOriginale As Entities.Paziente)

        ' Calcolo consultorio vaccinale solo se il parametro vale true
        If Me.Settings.AUTOSETCNS_INSLOCALE Then

            If Not pazienteOriginale Is Nothing Then

                ' Calcolo del consultorio in caso di modifica

                ' Il calcolo del consultorio vaccinale avviene solo se sono variati 
                ' il comune di residenza o domicilio rispetto ai valori precedenti.
                If paziente.ComuneResidenza_Codice <> pazienteOriginale.ComuneResidenza_Codice _
                   OrElse paziente.ComuneDomicilio_Codice <> pazienteOriginale.ComuneDomicilio_Codice Then

                    ' Cns vaccinale (calcolato in base ai dati ricevuti)
                    Dim codiceCnsNew As String = Me.CalcolaCodiceCns(paziente)

                    ' Calcolo del cns vaccinale in base ai dati del paziente presenti in locale
                    Dim codiceCnsLocale As String = Me.CalcolaCodiceCns(pazienteOriginale)

                    ' Se il cns vaccinale calcolato rispetto ai dati attuali in locale 
                    ' corrisponde a quello calcolato rispetto ai dati provenienti da centrale
                    ' ma e' diverso rispetto a quello attuale, lascio il vecchio cns perchè, evidentemente, 
                    ' e' stato richiesto espressamente dal paziente e non lo devo modificare in automatico.
                    If codiceCnsNew = codiceCnsLocale AndAlso pazienteOriginale.Paz_Cns_Codice <> codiceCnsLocale Then

                        codiceCnsNew = pazienteOriginale.Paz_Cns_Codice

                    End If

                    ' Se il consultorio calcolato con i nuovi dati del paziente è lo smistamento:
                    ' se il paz ha il cns valorizzato -> lascio il vecchio cns
                    ' se il paz ha il cns vuoto       -> assegno il nuovo
                    Dim codiceCnsSmistamento As String = Me.CalcolaCodiceCnsSmistamento(paziente)

                    If codiceCnsNew = codiceCnsSmistamento AndAlso Not String.IsNullOrEmpty(pazienteOriginale.Paz_Cns_Codice) Then

                        ' Altrimenti, mantengo il vecchio consultorio assegnato al paziente.
                        codiceCnsNew = pazienteOriginale.Paz_Cns_Codice

                    End If

                    ' Se il cns associato al paziente (già presente su db) è uguale a quello calcolato con i nuovi dati, 
                    ' non modifico nulla. Altrimenti valorizzo il cns old e la data di assegnazione.
                    If pazienteOriginale.Paz_Cns_Codice <> codiceCnsNew Then

                        paziente.Paz_Cns_Codice = codiceCnsNew
                        paziente.Paz_Cns_Codice_Old = pazienteOriginale.Paz_Cns_Codice
                        paziente.Paz_Cns_Data_Assegnazione = DateTime.Today

                    End If

                Else

                    ' Se non sono variati i comuni di residenza o domicilio, il cns rimane quello precedente
                    paziente.Paz_Cns_Codice = pazienteOriginale.Paz_Cns_Codice

                End If

            Else

                ' Calcolo del consultorio in caso di inserimento

                paziente.Paz_Cns_Codice = CalcolaCodiceCns(paziente)

            End If

        End If

    End Sub

    ''' <summary>
    ''' Calcolo consultorio del paziente specificato
    ''' </summary>
    ''' <param name="paziente"></param>
    Public Overridable Function CalcolaCodiceCns(paziente As Paziente) As String

        Return CalcolaCodiceCnsByCriterias(paziente, CalcoloConsultorioCriterias)

    End Function

    ''' <summary>
    ''' Calcolo consultorio del paziente, in base ai criteri specificati (nell'ordine dato)
    ''' </summary>
    ''' <param name="paziente"></param>
    ''' <param name="calcoloConsultorioCriterias"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Function CalcolaCodiceCnsByCriterias(paziente As Paziente, calcoloConsultorioCriterias As IEnumerable(Of CalcoloConsultorioCriteria)) As String

        Dim cnsVacc As String = String.Empty

        ' Calcolo età paziente
        Dim etaPaziente As Integer = CalcolaEtaPaziente(paziente)

        For Each calcoloConsultorioCriteria As CalcoloConsultorioCriteria In calcoloConsultorioCriterias

            cnsVacc = CalcolaCodiceConsultorioByCriteria(paziente, etaPaziente, calcoloConsultorioCriteria)

            If Not String.IsNullOrEmpty(cnsVacc) Then
                Exit For
            End If

        Next

        Return cnsVacc

    End Function

    Protected Overridable Function CalcolaCodiceConsultorioByCriteria(paziente As Paziente, etaPaziente As Integer, calcoloConsultorioCriteria As CalcoloConsultorioCriteria) As String

        Dim codiceConsultorio As String = Nothing

        Select Case calcoloConsultorioCriteria

            Case BizPaziente.CalcoloConsultorioCriteria.Pediatra
                If Not String.IsNullOrEmpty(paziente.MedicoBase_Codice) Then
                    codiceConsultorio = GenericProvider.Consultori.GetCnsByPediatra(paziente.MedicoBase_Codice, etaPaziente)
                End If
            Case BizPaziente.CalcoloConsultorioCriteria.Circoscrizione
                If Not String.IsNullOrEmpty(paziente.Circoscrizione_Codice) Then
                    codiceConsultorio = GenericProvider.Consultori.GetCnsByCircoscrizione(paziente.Circoscrizione_Codice, etaPaziente)
                End If
            Case BizPaziente.CalcoloConsultorioCriteria.Circoscrizione2
                If Not String.IsNullOrEmpty(paziente.Circoscrizione2_Codice) Then
                    codiceConsultorio = GenericProvider.Consultori.GetCnsByCircoscrizione(paziente.Circoscrizione2_Codice, etaPaziente)
                End If
            Case BizPaziente.CalcoloConsultorioCriteria.ComuneResidenza
                If Not String.IsNullOrEmpty(paziente.ComuneResidenza_Codice) Then
                    codiceConsultorio = GenericProvider.Consultori.GetCnsByComune(paziente.ComuneResidenza_Codice, etaPaziente)
                End If
            Case BizPaziente.CalcoloConsultorioCriteria.ComuneDomicilio
                If Not String.IsNullOrEmpty(paziente.ComuneDomicilio_Codice) Then
                    codiceConsultorio = GenericProvider.Consultori.GetCnsByComune(paziente.ComuneDomicilio_Codice, etaPaziente)
                End If
            Case BizPaziente.CalcoloConsultorioCriteria.Smistamento
                codiceConsultorio = GenericProvider.Consultori.GetCnsSmistamento(etaPaziente)

            Case Else
                Throw New NotImplementedException(String.Format("CalcoloConsultorioMode non supportato: {0}", calcoloConsultorioCriteria))

        End Select

        Return codiceConsultorio

    End Function

    ''' <summary>
    ''' Restituisce il codice del consultorio impostato come smistamento, in base all'età del paziente
    ''' </summary>
    ''' <param name="paziente"></param>    
    Protected Overridable Function CalcolaCodiceCnsSmistamento(paziente As Entities.Paziente) As String

        Dim etaPaziente As Integer = Me.CalcolaEtaPaziente(paziente)

        Return Me.GenericProvider.Consultori.GetCnsSmistamento(etaPaziente)

    End Function

    ''' <summary>
    ''' Imposta il codice del consultorio territoriale del paziente
    ''' </summary>
    ''' <param name="paziente"></param>
    ''' <param name="pazienteOriginale"></param>
    ''' <remarks></remarks>
    Protected Overridable Sub SetCodiceCNSTerritoriale(paziente As Entities.Paziente, pazienteOriginale As Entities.Paziente)

    End Sub

    ''' <summary>
    ''' Calcolo consultorio territoriale del paziente in base ai dati specificati
    ''' </summary>
    ''' <param name="paziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overridable Function CalcolaCodiceCNSTerritoriale(paziente As Entities.Paziente) As String

        Return Me.CalcolaCodiceCnsByCriterias(paziente, Me.CalcoloConsultorioTerritorialeCriterias)

    End Function

#End Region

#Region " Flag Regolarizzato "

    ''' <summary>
    ''' Imposta il flag regolarizzato del paziente
    ''' </summary>
    ''' <param name="paziente"></param>
    ''' <param name="pazienteOriginale"></param>
    ''' <remarks></remarks>
    Protected Overridable Sub SetFlagRegolarizzato(paziente As Paziente, pazienteOriginale As Paziente)

        If Not UslUnificataCorrente Is Nothing Then

            If pazienteOriginale Is Nothing Then

                paziente.FlagRegolarizzato = "S"

            End If

        Else

            If Settings.FLAG_REGOLARIZZATO_DEFAULT Is Nothing Then

                ' Se il parametro non è valorizzato, calcolo il flag regolarizzato

                Dim flagRegolarizzato As String = "N"

                If Not pazienteOriginale Is Nothing Then

                    ' MODIFICA PAZIENTE

                    ' Il flag regolarizzato va calcolato solo se il nuovo stato anagrafico è immigrato 
                    ' ed è diverso dal precedente, altrimenti rimane invariato.
                    If paziente.StatoAnagrafico = Enumerators.StatoAnagrafico.IMMIGRATO AndAlso
                       paziente.StatoAnagrafico <> pazienteOriginale.StatoAnagrafico Then

                        ' Il flag di regolarizzazione viene ricalcolato in due casi:
                        ' - se, rispetto ai dati originali, il comune di provenienza non varia e la nuova data di immigrazione è successiva;
                        ' - oppure se il nuovo comune di provenienza è diverso rispetto al precedente.
                        If (String.IsNullOrEmpty(pazienteOriginale.ComuneProvenienza_Codice) OrElse
                                String.IsNullOrEmpty(paziente.ComuneProvenienza_Codice) OrElse
                                pazienteOriginale.ComuneProvenienza_Codice = paziente.ComuneProvenienza_Codice) AndAlso
                           (pazienteOriginale.DataImmigrazione = DateTime.MinValue OrElse
                                paziente.DataImmigrazione = DateTime.MinValue OrElse
                                paziente.DataImmigrazione > pazienteOriginale.DataImmigrazione) Then

                            paziente.FlagRegolarizzato = CalcolaFlagRegolarizzato(paziente, pazienteOriginale)

                        ElseIf (String.IsNullOrEmpty(pazienteOriginale.ComuneProvenienza_Codice) OrElse
                            String.IsNullOrEmpty(paziente.ComuneProvenienza_Codice) OrElse
                            pazienteOriginale.ComuneProvenienza_Codice <> paziente.ComuneProvenienza_Codice) Then

                            paziente.FlagRegolarizzato = CalcolaFlagRegolarizzato(paziente, pazienteOriginale)

                        End If

                    Else

                        ' Se il nuovo stato anagrafico non è cambiato oppure non vale "immigrato", il flag non varia.
                        paziente.FlagRegolarizzato = pazienteOriginale.FlagRegolarizzato

                    End If

                Else
                    ' INSERIMENTO PAZIENTE
                    ' Il calcolo del flag regolarizzato avviene sempre
                    paziente.FlagRegolarizzato = CalcolaFlagRegolarizzato(paziente, pazienteOriginale)

                End If

            Else

                ' Se il parametro FLAG_REGOLARIZZATO_DEFAULT è valorizzato, imposto il flag in base a tale valore
                paziente.FlagRegolarizzato = IIf(Settings.FLAG_REGOLARIZZATO_DEFAULT, "S", "N")

            End If

        End If

    End Sub

    ''' <summary>
    ''' Calcolo del flag regolarizzato, in base ai dati specificati
    '''</summary>
    ''' <param name="paziente"></param>
    ''' <param name="pazienteOriginale"></param>
    ''' <returns></returns>
    Protected Overridable Function CalcolaFlagRegolarizzato(paziente As Entities.Paziente, pazienteOriginale As Entities.Paziente) As String

        Dim flagRegorizzato As String = "N"

        Dim now As DateTime = DateTime.Now

        If pazienteOriginale Is Nothing Then

            ' Calcolo flag regolarizzato in caso di inserimento

            ' Controllo giorni di età del paziente rispetto al parametro
            If (now - paziente.Data_Nascita).Days > Me.Settings.NUM_GIORNI_REGOLARIZZAZIONE Then

                ' Se il paziente ha un'età superiore al parametro, controllo se è residente non immigrato.
                If paziente.StatoAnagrafico = Enumerators.StatoAnagrafico.RESIDENTE AndAlso
                   String.IsNullOrEmpty(paziente.ComuneProvenienza_Codice) Then

                    flagRegorizzato = "S"

                End If

            Else

                ' Se il paziente ha un'età inferiore al parametro, è regolarizzato di default.
                flagRegorizzato = "S"

            End If

        End If

        Return flagRegorizzato

    End Function

    ''' <summary>
    ''' Calcolo del flag di regolarizzazione del paziente in base alla fascia di età (se previsto) e, in caso di regolarizzazione effettuata, 
    ''' modifica dello stato anagrafico a RESIDENTE per i pazienti aventi stato anagrafico IMMIGRATO.
    ''' Per le ULSS Unificate, viene impostato a true in inserimento.
    ''' </summary>
    ''' <param name="paziente"></param>
    ''' <param name="pazienteOriginale"></param>
    ''' <remarks></remarks>
    Public Overridable Sub UpdateFlagRegolarizzato(paziente As Paziente, pazienteOriginale As Paziente)

        UpdateFlagRegolarizzato(paziente, pazienteOriginale, Nothing)

    End Sub

    ''' <summary>
    ''' Calcolo del flag di regolarizzazione del paziente in base alla fascia di età (se previsto) e, in caso di regolarizzazione effettuata, 
    ''' modifica dello stato anagrafico a RESIDENTE per i pazienti aventi stato anagrafico IMMIGRATO.
    ''' Per le ULSS Unificate, viene impostato a true in inserimento.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <remarks></remarks>
    Public Overridable Sub UpdateFlagRegolarizzato(codicePaziente As Long)

        Dim paziente As Paziente = GenericProvider.Paziente.GetPazienti(codicePaziente.ToString(), ContextInfos.CodiceUsl)(0)
        Dim pazienteOriginale As Paziente = paziente.Clone()

        UpdateFlagRegolarizzato(paziente, pazienteOriginale, Nothing)

    End Sub

    ''' <summary>
    ''' Imposta il flag di regolarizzazione per il paziente specificato.
    ''' Se il flag è true e il paziente è IMMIGRATO, viene impostato a RESIDENTE.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="flagRegolarizzato"></param>
    ''' <remarks></remarks>
    Public Overridable Sub UpdateFlagRegolarizzato(codicePaziente As Long, flagRegolarizzato As Boolean)

        Dim paziente As Paziente = GenericProvider.Paziente.GetPazienti(codicePaziente.ToString(), ContextInfos.CodiceUsl)(0)
        Dim pazienteOriginale As Paziente = paziente.Clone()

        UpdateFlagRegolarizzato(paziente, pazienteOriginale, flagRegolarizzato)

    End Sub

    Private Sub UpdateFlagRegolarizzato(paziente As Entities.Paziente, pazienteOriginale As Entities.Paziente, flagRegolarizzato As Boolean?)

        If Not flagRegolarizzato.HasValue Then

            ' Calcolo flag
            paziente.FlagRegolarizzato = Me.CalcolaFlagRegolarizzato(paziente, pazienteOriginale)

        Else

            ' Forzatura flag
            If flagRegolarizzato Then

                paziente.FlagRegolarizzato = "S"

                If paziente.StatoAnagrafico = Enumerators.StatoAnagrafico.IMMIGRATO Then
                    paziente.StatoAnagrafico = Enumerators.StatoAnagrafico.RESIDENTE
                End If

            Else
                paziente.FlagRegolarizzato = "N"
            End If

        End If

        Me.GenericProvider.Paziente.ModificaPaziente(paziente)

        ' TODO [LOG]: If Not Me.LogOptions Is Nothing ??
        Dim testataLog As New Log.DataLogStructure.Testata(Me.LogOptions.CodiceArgomento, Log.DataLogStructure.Operazione.Modifica, paziente.Paz_Codice, Me.LogOptions.Automatico)
        testataLog.Records.Add(PazienteLogManager.GetLogPaziente(paziente, pazienteOriginale))
        Me.WriteLog(testataLog)

    End Sub

#End Region

#Region " Flag Stampa Maggiorenni "

    Public Function UpdateFlagStampaAvvisoMaggiorenni(codiceConsultorio As String, codiceComuneResidenza As String, dataNascitaIniziale As DateTime, dataNascitaFinale As DateTime) As Integer

        Me.GenericProvider.Paziente.UpdateFlagStampaAvvisoMaggiorenni(codiceConsultorio, codiceComuneResidenza, dataNascitaIniziale, dataNascitaFinale, "S")

    End Function

#End Region

#Region " Cicli "

    Protected Overridable Function GetCodiceCicliStandardPaziente(paziente As Paziente) As ICollection(Of String)

        Dim codiceCicliPaziente As List(Of String) = Nothing

        If Settings.AUTO_CALC_CICLI Then

            codiceCicliPaziente = GenericProvider.Cicli.GetCodiciCicliStandard(paziente)

        End If

        Return codiceCicliPaziente

    End Function

    ''' <summary>
    ''' Controllo coerenza cicli del paziente, dopo le modifiche.
    ''' </summary>
    ''' <param name="paziente"></param>
    ''' <param name="pazienteOriginale"></param>
    ''' <returns>
    ''' Struttura BizResult con flag Success a true se i cicli sono compatibili con le modifiche effettuate, 
    ''' false in caso contrario.
    ''' </returns>
    ''' <remarks>
    ''' Se sono modificati sesso e/o data di nascita, il paziente potrebbe avere uno o più cicli incompatibili
    ''' </remarks>
    Protected Overridable Function CheckCicli(paziente As Paziente, pazienteOriginale As Paziente) As BizResult

        Dim checkCicliResult As BizResult = Nothing

        Dim messages As New List(Of String)
        Dim success As Boolean = True

        If Settings.CHECK_CICLI Then

            If Not paziente Is Nothing AndAlso Not pazienteOriginale Is Nothing AndAlso
               (paziente.Sesso <> pazienteOriginale.Sesso OrElse paziente.Data_Nascita <> pazienteOriginale.Data_Nascita) Then

                If GenericProvider.Cicli.CountCicliIncompatibili(paziente) > 0 Then

                    Dim message As String = "In seguito alla modifica, il paziente presenta cicli vaccinali non coerenti per sesso e/o data nascita."

                    messages.Add(message)

                    If Settings.CHECK_CICLI_ERRORE Then
                        success = False
                    End If

                End If

            End If

        End If

        Return New BizResult(success, messages, Nothing)

    End Function

    ''' <summary>
    ''' Sostituisce, al paziente, i cicli incompatibili con il ciclo specificato. 
    ''' Elimina la programmazione per i cicli incompatibili, elimina i cicli incompatibili al paziente e aggiunge quello specificato
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="codiceNuovoCiclo"></param>
    ''' <returns></returns>
    Public Function SostituisciCicliIncompatibili(codicePaziente As Long, codiceNuovoCiclo As String) As BizGenericResult

        'TODO [API registrazione vac]: SostituisciCicliIncompatibiliEProgrammazione => valore di ritorno + TEST!!!

        ' controllare se il paz ha già il ciclo specificato. Se non è presente tra i cicli del paziente:
        ' - controllare se il paz ha cicli in conflitto con quello specificato
        ' - se si, eliminare programmazione per quei cicli ed eliminare i cicli in conflitto
        ' - inserire nuovo ciclo
        ' - calcolare programmazione, che non si deve fermare alla prima obbligatoria ma deve programmare il nuovo ciclo 

        ' Controllo presenza ciclo tra quelli del paziente
        If Not GenericProvider.Cicli.ExistsCicloPaziente(codicePaziente, codiceNuovoCiclo) Then

            ' Controllo incompatibilità
            Dim listaCicliIncompatibili As List(Of String) = GenericProvider.Cicli.GetCicliIncompatibili(codicePaziente, codiceNuovoCiclo)

            If Not listaCicliIncompatibili.IsNullOrEmpty() Then

                'TODO [API registrazione vac]:  Eliminazione programmazione per i cicli incompatibili => togliere transaction scope???

                ' Eliminazione programmazione per i cicli incompatibili
                Using bizVacProg As New BizVaccinazioneProg(GenericProvider, Settings, ContextInfos)

                    Dim command As New BizVaccinazioneProg.EliminaProgrammazioneCommand()
                    command.CodicePaziente = codicePaziente
                    command.DataConvocazione = Nothing
                    command.EliminaAppuntamenti = True
                    command.EliminaSollecitiBilancio = False
                    command.EliminaBilanci = False
                    command.TipoArgomentoLog = DataLogStructure.TipiArgomento.ELIMINA_PROG
                    command.OperazioneAutomatica = True
                    command.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.VariazioneCiclo
                    command.NoteEliminazione = "Eliminazione convocazioni paziente per sostituzione ciclo (SostituisciCicliIncompatibili) - Nuovo ciclo: " + codiceNuovoCiclo
                    command.CodiciCicliEliminazioneProgrammazione = listaCicliIncompatibili

                    bizVacProg.EliminaProgrammazione(command)

                End Using

                ' Eliminazione cicli da paziente
                GenericProvider.Cicli.DeleteCicliPaziente(codicePaziente, listaCicliIncompatibili)

            End If

            ' Inserimento nuovo ciclo a paziente
            If GenericProvider.Cicli.InsertCicloPaziente(codicePaziente, codiceNuovoCiclo) > 0 Then

                ' Calcolo convocazioni

                'TODO [API registrazione vac]: capire quale codice cns usare

                'TODO [API registrazione vac]: non richiamare calcolo convocazioni dal biz!

                'Using gestioneConvocazioni As New CalcoloConvocazioni.GestioneConvocazioni(ContextInfos.CodiceCentroVaccinale, ContextInfos, GenericProvider.Provider, GenericProvider.Connection)
                '    gestioneConvocazioni.CalcolaConvocazioni(codicePaziente, ContextInfos.IDUtente)
                'End Using

            End If

        End If

        Return New BizGenericResult() With {.Success = True}

    End Function

#End Region

#Region " Convocazione "

    Protected Overridable Function ModificaConvocazioni(paziente As Paziente, convocazioniOriginali As ICollection(Of Entities.Convocazione)) As ICollection(Of Entities.Convocazione)

        Dim convocazioni(convocazioniOriginali.Count - 1) As Entities.Convocazione

        If Me.Settings.UPDCNV_UPDCNS Then

            convocazioniOriginali.CopyTo(convocazioni, 0)

            Using bizConvocazione As New BizConvocazione(Me.GenericProvider, Me.Settings, Me.ContextInfos, Me.LogOptions)

                Dim now As DateTime = DateTime.Now

                Dim noteEliminazione As String = "Eliminazione appuntamento per modifica centro vaccinale del paziente"

                For Each convocazione As Entities.Convocazione In convocazioni

                    convocazione.Cns_Codice = paziente.Paz_Cns_Codice

                    If Me.Settings.UPDCNV_DELAPP Then

                        convocazione.DataAppuntamento = DateTime.MinValue
                        convocazione.CodiceAmbulatorio = Nothing
                        convocazione.TipoAppuntamento = Nothing
                        convocazione.Durata_Appuntamento = 0        ' N.B. : era inizializzato a Nothing, che per un integer significa impostarlo al suo default value, cioè a 0 (non è un nullable!).

                        If convocazione.DataPrimoAppuntamento = DateTime.MinValue Then
                            convocazione.DataPrimoAppuntamento = convocazione.Data_CNV
                        End If

                    End If

                    ' Update convocazione e storico appuntamenti
                    Dim convocazioneAppuntamento As Entities.ConvocazioneAppuntamento = bizConvocazione.CreateConvocazioneAppuntamento(convocazione, noteEliminazione, now)
                    convocazioneAppuntamento.IdMotivoEliminazioneAppuntamento = Constants.MotiviEliminazioneAppuntamento.VariazioneConsultorio

                    bizConvocazione.UpdateConvocazione_DatiAppuntamento(convocazioneAppuntamento)

                Next

            End Using
        End If

        Return convocazioni

    End Function

#End Region

#Region " Paziente "

    ''' <summary>
    ''' Restituisce l'età, in anni, del paziente.
    ''' </summary>
    ''' <param name="paziente"></param>
    ''' <returns></returns>
    Protected Overridable Function CalcolaEtaPaziente(paziente As Entities.Paziente) As Integer

        Dim etaPaziente As Integer = 0

        If paziente.Data_Nascita > DateTime.MinValue Then

            etaPaziente = Convert.ToInt32(Math.Round(DateTime.Now.Subtract(paziente.Data_Nascita).TotalDays / 365))

        End If

        Return etaPaziente

    End Function

    ''' <summary>
    ''' Restituisce il valore del flag che valorizza il campo paz_locale, in base alla presenza del codice regionale
    ''' </summary>
    ''' <param name="codiceRegionale"></param>
    ''' <returns></returns>
    Public Function CalcolaFlagLocale(codiceRegionale As String) As Boolean

        ' N.B. : in Veneto c'è il biz CUSTOM, rimasto da prima dello sdoppiamento. 
        '        Il codice che veniva eseguito non era quello "standard", che ho cancellato, ma quello che ho riportato qui 

        Return String.IsNullOrWhiteSpace(codiceRegionale)

    End Function

    Public Function CalcolaCodiceUslProgrammazioneVaccinale(codiceUslAssistenza As String, codiceComuneDomicilio As String, codiceUslResidenza As String) As String

        Dim codiceUslProgrammazioneVaccinale As String = Nothing

        If Not String.IsNullOrEmpty(codiceUslAssistenza) Then
            codiceUslProgrammazioneVaccinale = codiceUslAssistenza
        ElseIf Not String.IsNullOrEmpty(codiceComuneDomicilio) Then
            codiceUslProgrammazioneVaccinale = Me.GenericProvider.Usl.GetCodiciUslByComune(codiceComuneDomicilio).FirstOrDefault()
        End If

        If String.IsNullOrEmpty(codiceUslProgrammazioneVaccinale) Then
            codiceUslProgrammazioneVaccinale = codiceUslResidenza
        End If

        Return codiceUslProgrammazioneVaccinale

    End Function



    Public Class CalcolaUslEmigrazioneImmigrazioneResult

        Public Property Modified() As Boolean = False

        ' emigrazione
        Public Property LuogoEmigrazioneCodice() As String
        Public Property LuogoEmigrazioneDescrizione() As String
        Public Property LuogoEmigrazioneData() As Date = Date.MinValue
        ' immigrazione
        Public Property LuogoImmigrazioneCodice() As String
        Public Property LuogoImmigrazioneDescrizione() As String
        Public Property LuogoImmigrazioneData() As Date = Date.MinValue

    End Class

    Public Overridable Function CalcolaUslEmigrazioneImmigrazione(residenzaPrecedenteCodice As String,
                                                      residenzaPrecedenteDescrizione As String,
                                                      residenzaAttualeCodice As String,
                                                      residenzaAttualeDescrizione As String) As CalcolaUslEmigrazioneImmigrazioneResult

        Return New CalcolaUslEmigrazioneImmigrazioneResult()

    End Function

#End Region

#Region " ASL "

    ''' <summary>
    ''' Restituisce il codice della Asl in base al comune specificato.
    ''' Se non lo trova restituisce la stringa vuota.
    ''' </summary>
    ''' <param name="codiceComune"></param>
    ''' <returns></returns>
    Protected Overridable Function GetCodiceAslByComune(codiceComune As String) As String

        Dim codiceAsl As String = String.Empty

        If Not String.IsNullOrEmpty(codiceComune) Then

            Dim codiciAsl As ICollection(Of String) = Me.GenericProvider.Usl.GetCodiciUslByComune(codiceComune)

            If Not codiciAsl Is Nothing AndAlso codiciAsl.Count > 0 Then

                codiceAsl = codiciAsl.FirstOrDefault()

            End If

        End If

        Return codiceAsl

    End Function

#End Region

#End Region

#Region " Private "

    Private Sub SetDefaultValueFlags(paziente As Paziente)

        If String.IsNullOrEmpty(paziente.FlagCompletare) Then
            paziente.FlagCompletare = "N"
        End If

        If String.IsNullOrEmpty(paziente.FlagOccasionale) Then
            paziente.FlagOccasionale = "N"
        End If

        If String.IsNullOrEmpty(paziente.FlagIrreperibilita) Then
            paziente.FlagIrreperibilita = "N"
        End If

        If String.IsNullOrEmpty(paziente.FlagCancellato) Then
            paziente.FlagCancellato = "N"
        End If

        If paziente.LivelloCertificazione = Nothing Then
            paziente.LivelloCertificazione = 0
        End If

        If CalcolaFlagLocale(paziente.PAZ_CODICE_REGIONALE) Then
            paziente.FlagLocale = "S"
        Else
            paziente.FlagLocale = "N"
        End If

    End Sub

    Private Function GetMessaggioDatiMasterAlias(pazienteMaster As Paziente, pazienteAlias As Paziente) As String

        Dim messaggioDatiMasterAlias As New System.Text.StringBuilder()

        messaggioDatiMasterAlias.AppendLine(String.Format("PAZIENTE MASTER: Codice: {0}; Ausiliario: {1}",
                                                          pazienteMaster.Paz_Codice.ToString(),
                                                          pazienteMaster.CodiceAusiliario))

        messaggioDatiMasterAlias.AppendFormat("PAZIENTE ALIAS: Codice: {0}; Ausiliario: {1}; Cognome: {2}; Nome: {3}; Data Nascita: {4:dd/MM/yyyy}; Comune Nascita: {5}; Sesso: {6}; Codice Fiscale: {7};",
                                              pazienteAlias.Paz_Codice.ToString(),
                                              pazienteAlias.CodiceAusiliario,
                                              pazienteAlias.PAZ_COGNOME,
                                              pazienteAlias.PAZ_NOME,
                                              pazienteAlias.Data_Nascita,
                                              pazienteAlias.ComuneNascita_Descrizione,
                                              pazienteAlias.Sesso,
                                              pazienteAlias.PAZ_CODICE_FISCALE)

        Return messaggioDatiMasterAlias.ToString()

    End Function

    Private Function GetMessageUnisciPaziente(messaggioRisultato As String, messaggioDatiMasterAlias As String) As String

        Dim stbErrorMessage As New System.Text.StringBuilder()

        stbErrorMessage.AppendLine(messaggioRisultato)
        stbErrorMessage.Append(messaggioDatiMasterAlias)

        Return stbErrorMessage.ToString()

    End Function

    Public Function GestioneEsenzioniMalattiaPaziente(paziente As Paziente) As BizGenericResult

        Dim result As New BizGenericResult()
        result.Success = True
        result.Message = String.Empty

        If paziente Is Nothing OrElse paziente.Paz_Codice <= 0 Then
            Return result
        End If

        If paziente.Malattie Is Nothing OrElse paziente.Malattie.Rows.Count = 0 Then
            Return result
        End If

        Dim listEsenzioniMalattia As New List(Of EsenzioneMalattia)()

        Dim listCodiciEsenzioniNonMappate As New List(Of String)()

        Using bizMalattie As New BizMalattie(GenericProvider, Settings, ContextInfos, LogOptions)

            For Each row As dsMalattie.MalattieRow In paziente.Malattie.Rows

                Dim codice As String = bizMalattie.GetCodiceMalattiaByCodiceEsenzione(row.PMA_MAL_CODICE)

                If String.IsNullOrWhiteSpace(codice) Then
                    listCodiciEsenzioniNonMappate.Add(row.PMA_MAL_CODICE)
                    Continue For
                End If

                Dim item As New EsenzioneMalattia()
                item.Codice = codice
                item.CodiceEsenzione = row.PMA_MAL_CODICE
                item.DataInizioValidita = row.PMA_DATA_DIAGNOSI

                listEsenzioniMalattia.Add(item)

            Next

            If Not listCodiciEsenzioniNonMappate.IsNullOrEmpty() Then
                result.Message = "RICEVUTE ESENZIONI NON MAPPATE IN LOCALE: " + String.Join("; ", listCodiciEsenzioniNonMappate.ToArray())
            End If

            bizMalattie.GestioneEsenzioniMalattiaPaziente(paziente.Paz_Codice, listEsenzioniMalattia)

        End Using

        Return result

    End Function

#End Region

#Region " Types "

    Public Enum CalcoloConsultorioCriteria
        Pediatra
        Circoscrizione
        Circoscrizione2
        ComuneResidenza
        ComuneDomicilio
        Smistamento
    End Enum

#Region " Dati Anagrafici "

    ' [Unificazione ULSS]: Il nuovo flag FromVAC indica se l'operazione di insert/update/merge è stata ricevuta 
    ' da AURV (FromVAC=false) o da modifica applicativa (FromVAC=true)

    Public Class InserisciPazienteCommand
        Public Property Paziente As Paziente
        Public Property FromVAC As Boolean
        Public Property ForzaInserimento As Boolean
    End Class

    Public Class ModificaPazienteCommand
        Public Property Paziente As Paziente
        Public Property FromVAC As Boolean
    End Class

#End Region

#Region " Merge/Unmerge "

    Public Class UnisciPazientiCommand
        Public Property PazienteMaster As Paziente
        Public Property PazienteAlias As Paziente
        Public Property UsaAliasComeMasterVaccinale As Boolean
        Public Property SetStatoAcquisizioneTotaleIfNull As Boolean
        Public Property FromVAC As Boolean
    End Class

    Public Class DisunisciPazientiCommand
        Public Property PazienteMaster As Paziente
        Public Property PazienteAlias As Paziente
        Public Property FromVAC As Boolean
    End Class

    Public Class CheckCodiciRegionaliMasterAliasResult

        Public Enum ResultType
            Success = 0
            Failure = 1
            Warning = 2
        End Enum

        Public Result As ResultType
        Public Message As String

        Public Sub New(result As ResultType)
            Me.New(result, String.Empty)
        End Sub

        Public Sub New(result As ResultType, message As String)
            Me.Result = result
            Me.Message = message
        End Sub

    End Class

#End Region

#Region " Dati Vaccinali Centrale "

    Public Class AggiornaDatiVaccinaliCentraliCommand

        Public Property CodicePazienteCentrale As String

        Public Property VaccinazioneEseguitaEnumerable As IEnumerable(Of VaccinazioneEseguita)
        Public Property VaccinazioneEseguitaScadutaEnumerable As IEnumerable(Of VaccinazioneEseguita)
        Public Property VaccinazioneEseguitaScadutaEliminataEnumerable As IEnumerable(Of VaccinazioneEseguita)
        Public Property VaccinazioneEseguitaEliminataEnumerable As IEnumerable(Of VaccinazioneEseguita)

        Public Property ReazioneAvversaEnumerable As IEnumerable(Of ReazioneAvversa)
        Public Property ReazioneAvversaEliminataEnumerable As IEnumerable(Of ReazioneAvversa)

        Public Property VaccinazioneEsclusaEnumerable As IEnumerable(Of VaccinazioneEsclusa)
        Public Property VaccinazioneEsclusaEliminataEnumerable As IEnumerable(Of VaccinazioneEsclusa)

        Public Property VisitaEnumerable As IEnumerable(Of Visita)
        Public Property VisitaEliminataEnumerable As IEnumerable(Of Visita)

        Public Property OsservazioneEnumerable As IEnumerable(Of Osservazione)
        Public Property OsservazioneEliminataEnumerable As IEnumerable(Of Osservazione)

        Friend Property IsMerge As Boolean
        Friend Property CodicePazienteCentraleAlias As String
        Friend Property VaccinazioniEseguiteCentraliMasterMergeInfo As DatiVaccinaliCentraliMasterMergeInfo()
        Friend Property ReazioniAvverseCentraliMasterMergeInfo As DatiVaccinaliCentraliMasterMergeInfo()
        Friend Property VaccinazioniEscluseCentraliMasterMergeInfo As DatiVaccinaliCentraliMasterMergeInfo()
        Friend Property VisiteCentraliMasterMergeInfo As DatiVaccinaliCentraliMasterMergeInfo()

        Friend Property FlagVisibilitaModificata As String

        Friend Property IsRisoluzioneConflitto As Boolean

    End Class

    Friend Class DatiVaccinaliCentraliMasterMergeInfo
        Public Property IdDatoVaccinaleUslInserimentoMaster As Int64
        Public Property CodiceUslInserimentoMaster As String
        Public Property CodiciUslDatiVaccinaliDistribuiti As String()
    End Class

#Region " AcquisisciDatiVaccinaliCentrali "

    Public Class AcquisisciDatiVaccinaliCentraliCommand

        Public Property CodicePaziente As Int64
        Public Property CodicePazienteCentrale As String
        Public Property CodiceConsultorioPaziente As String

        Public Property RichiediConfermaSovrascrittura As Boolean

        Public Property Note As String

        Friend Property IdApplicazioneProgrammazione As String

        Friend Property DatiVaccinaliCentraliAcquisizioneInfo As New DatiVaccinaliCentraliInfo

        Friend Property IsMerge As Boolean = False

        Friend Property IsVisibilitaModificata As Boolean = False
        Friend Property UpdateStatoAcquisizione As Boolean = True

        Friend Property LoadDatiVaccinaliCentrali As Boolean = True

    End Class

    Public Class AcquisisciDatiVaccinaliCentraliResult

        Public Property VaccinazioniEsclusePresenti As Boolean
        Public Property ProgrammazionePresente As Boolean

        Public Property StatoAcquisizioneDatiVaccinaliCentrale As Enumerators.StatoAcquisizioneDatiVaccinaliCentrale?

        Public Property AcquisisciVaccinazioneEseguitaCentraleResult As BizVaccinazioniEseguite.AcquisisciVaccinazioneEseguitaCentraleResult
        Public Property AcquisisciVisitaCentraleResult As BizVisite.AcquisisciVisitaCentraleResult

        Public ReadOnly Property DatiVaccinaliAcquisiti As Boolean
            Get
                Return (Not Me.AcquisisciVaccinazioneEseguitaCentraleResult Is Nothing AndAlso
                        Me.AcquisisciVaccinazioneEseguitaCentraleResult.VaccinazioniEseguiteAcquisite AndAlso
                        Me.AcquisisciVaccinazioneEseguitaCentraleResult.VaccinazioniEseguiteScaduteAcquisite) AndAlso
                    (Not Me.AcquisisciVisitaCentraleResult Is Nothing AndAlso Me.AcquisisciVisitaCentraleResult.VisiteAcquisite)
            End Get
        End Property

        Public Function BuildMessage(newLine As String) As String

            Dim messageStringBuilder As New System.Text.StringBuilder()

            If Not Me.AcquisisciVaccinazioneEseguitaCentraleResult Is Nothing Then

                If Not Me.AcquisisciVaccinazioneEseguitaCentraleResult.VaccinazioniEseguiteAcquisite OrElse
                         Not Me.AcquisisciVaccinazioneEseguitaCentraleResult.VaccinazioniEseguiteScaduteAcquisite Then

                    messageStringBuilder.AppendFormat("Vaccinazioni {0}{1}{2}",
                          IIf(Not Me.AcquisisciVaccinazioneEseguitaCentraleResult.VaccinazioniEseguiteAcquisite, "Eseguite", String.Empty),
                          IIf((Not Me.AcquisisciVaccinazioneEseguitaCentraleResult.VaccinazioniEseguiteAcquisite AndAlso
                               Not Me.AcquisisciVaccinazioneEseguitaCentraleResult.VaccinazioniEseguiteScaduteAcquisite), ", ", String.Empty),
                          IIf(Not Me.AcquisisciVaccinazioneEseguitaCentraleResult.VaccinazioniEseguiteScaduteAcquisite, "Scadute", String.Empty))

                End If

            End If

            If Not Me.AcquisisciVisitaCentraleResult Is Nothing Then

                If Not Me.AcquisisciVisitaCentraleResult.VisiteAcquisite Then
                    messageStringBuilder.AppendFormat("{0}Visite", IIf((messageStringBuilder.Length > 0), " e ", String.Empty))
                End If

            End If

            If messageStringBuilder.Length > 0 Then
                messageStringBuilder.Append(" acquisite parzialmente.")
            End If

            Return messageStringBuilder.ToString()

        End Function

    End Class

#End Region

#Region " Conflitti "

    Public Class RisolviConflittoVaccinazioniEseguiteCentraleCommand

        Public Property IdVaccinazioniEseguiteCentralePazienteDictionary As IDictionary(Of Int64, IEnumerable(Of Int64))

    End Class

    Public Class RisolviConflittiVaccinazioniEscluseCentraleCommand

        Public Property IdVaccinazioniEscluseCentraleDictionary As IDictionary(Of Int64, IEnumerable(Of Int64))

    End Class

    Public Class RisolviConflittoVisiteCentraleCommand

        Public Property IdVisiteCentraleDictionary As IDictionary(Of Int64, IEnumerable(Of Int64))

    End Class

#End Region

    Private Class LoadDatiVaccinaliCentraliCommand

        Public Property VaccinazioniEseguiteCentrale As VaccinazioneEseguitaCentrale()
        Public Property VaccinazioniEscluseCentrale As VaccinazioneEsclusaCentrale()
        Public Property VisiteCentrale As VisitaCentrale()

        Public Property LoadDatiVaccinaliUslGestitaCorrente As Boolean
        Public Property LoadDatiVaccinaliEliminati As Boolean
        Public Property LoadReazioneAvverse As Boolean
        Public Property LoadOsservazioni As Boolean

    End Class

    Private Class LoadDatiVaccinaliCentraliResult

        Public Property DatiVaccinaliCentraliInfo As New DatiVaccinaliCentraliInfo

    End Class

    Friend Class DatiVaccinaliCentraliInfo

        Public Property VaccinazioneEseguitaProgrammataCentraleInfos As BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo()
        Public Property VaccinazioneEseguitaRecuperataCentraleInfos As BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo()
        Public Property VaccinazioneEseguitaRegistrataCentraleInfos As BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo()
        Public Property VaccinazioneEseguitaRipristinataCentraleInfos As BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo()

        Public Property VaccinazioneEseguitaScadutaCentraleInfos As BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo()
        Public Property VaccinazioneEseguitaScadutaEliminataCentraleInfos As BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo()
        Public Property VaccinazioneEseguitaEliminataCentraleInfos As BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo()

        Public Property ReazioneAvversaCentraleInfos As BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo()
        Public Property ReazioneAvversaEliminataCentraleInfos As BizVaccinazioniEseguite.VaccinazioneEseguitaCentraleInfo()

        Public Property VaccinazioneEsclusaCentraleInfos As BizVaccinazioniEscluse.VaccinazioneEsclusaCentraleInfo()
        Public Property VaccinazioneEsclusaEliminataCentraleInfos As BizVaccinazioniEscluse.VaccinazioneEsclusaCentraleInfo()

        Public Property VisitaCentraleInfos As BizVisite.VisitaCentraleInfo()
        Public Property VisitaEliminataCentraleInfos As BizVisite.VisitaCentraleInfo()

        Public Property Empty As Boolean = True

    End Class

#End Region

#End Region

#Region " OnVac API "

    ''' <summary>
    ''' Restituisce un'IEnumerable contenente i codici dei pazienti corrispondenti al codice fiscale specificato.
    ''' </summary>
    ''' <param name="codiceFiscale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCodiciPazientiByCodiceFiscale(codiceFiscale As String) As IEnumerable(Of String)

        Dim codici As ICollection(Of String) = GenericProvider.Paziente.GetCodicePazientiByCodiceFiscale(codiceFiscale)

        If codici Is Nothing Then
            Return (New List(Of String)).AsEnumerable()
        End If

        Return codici.AsEnumerable()

    End Function

    <Serializable>
    Public Class GetCodiceReferenteResult
        Inherits BizGenericResult

        Public Property CodiceCentrale As String

    End Class

    ''' <summary>
    ''' Restituisce il codice centrale del referente in base al fiscale specificato.
    ''' La ricerca viene effettuata direttamente sulla vista unione delle anagrafiche delle ulss non unificate + l'anagrafica della ulss unificata (db unico).
    ''' </summary>
    ''' <param name="codiceFiscale"></param>
    ''' <returns></returns>
    Public Function GetCodiceCentralePazienteReferenteByCodiceFiscale(codiceFiscale As String) As GetCodiceReferenteResult

        Dim result As New GetCodiceReferenteResult()

        Dim pazienti As IEnumerable(Of PazienteUlss) = GenericProvider.Paziente.GetPazientiUlssByCodiceFiscale(codiceFiscale)

        If pazienti.IsNullOrEmpty() Then
            result.Success = False
            result.CodiceCentrale = String.Empty
            result.Message = "Il codice fiscale indicato non corrisponde a nessun utente. Controllare il valore inserito oppure recarsi presso il proprio centro vaccinale per la registrazione."
            Return result
        End If

        Dim pazienteAttivoSelezionato As PazienteUlss

        ' Filtro stati anagrafici attivi
        Dim statiAnagraficiAttivi As IEnumerable(Of Enumerators.StatoAnagrafico) = GenericProvider.StatiAnagrafici.GetStatiAnagraficiAttivi()

        Dim pazientiAttivi As IEnumerable(Of PazienteUlss) =
            pazienti.Where(Function(p) p.StatoAnagrafico.HasValue AndAlso statiAnagraficiAttivi.Contains(p.StatoAnagrafico.Value))

        If pazientiAttivi.IsNullOrEmpty() Then

            ' Paziente non presente con stato anagrafico attivo => FINE

            result.Success = False
            result.CodiceCentrale = String.Empty
            result.Message = "Il codice fiscale indicato non corrisponde a nessun utente attivo. Controllare il valore inserito oppure recarsi presso il proprio centro vaccinale per la registrazione."
            Return result

        Else

            ' 1 o più pazienti attivi => Controllare

            ' Prendo il primo paziente tra quelli attivi (la lista è ordinata per codice ulss, quindi le vecchie vincono sull'unificata).
            ' Se nella ulss indicata c'è solo 1 paziente (controllando anche i non attivi!) => OK
            ' Altrimenti, il paziente non è identificabile univocamente => FINE

            pazienteAttivoSelezionato = pazientiAttivi.First()

            If pazienti.Count(Function(p) p.CodiceUsl = pazienteAttivoSelezionato.CodiceUsl) > 1 Then

                ' Paziente non univoco => FINE

                result.Success = False
                result.CodiceCentrale = String.Empty
                result.Message = "Utente non identificato correttamente. Controllare il codice fiscale inserito oppure recarsi presso il proprio centro vaccinale."
                Return result

            End If

        End If

        ' Paziente univoco => OK

        result.Success = True
        result.CodiceCentrale = pazienteAttivoSelezionato.CodiceCentralePaziente
        result.Message = String.Empty

        Return result

        ' TODO [Unificazione Ulss]: restituire anche codice locale e ulss? Controllare se serve

    End Function


    Public Function GetCodiceCentralePazienteReferenteByCellulare(cellulare As String, idConsenso As Integer, codiceAziendaRegistrazione As String) As GetCodiceReferenteResult

        Dim result As New GetCodiceReferenteResult()
        Dim resultConsenso As New GetUltimoConsensoRegistratoResult()

        Dim pazienti As IEnumerable(Of PazienteUlss) = Nothing
        resultConsenso.UltimoConsenso = GenericProvider.Paziente.GetPazientiUlssByCellulare(cellulare, idConsenso, codiceAziendaRegistrazione)

        If resultConsenso.UltimoConsenso Is Nothing Then
            result.Success = False
            result.CodiceCentrale = String.Empty
            result.Message = "Il Cellulare indicato non corrisponde a nessun utente. Controllare il valore inserito oppure recarsi presso il proprio centro vaccinale per la registrazione."
            Return result
        End If


        ' Paziente univoco => OK

        result.Success = True
        result.CodiceCentrale = resultConsenso.UltimoConsenso.CodicePaziente
        result.Message = String.Empty

        Return result

        ' TODO [Unificazione Ulss]: restituire anche codice locale e ulss? Controllare se serve

    End Function

    Public Function GetInfoAssistito(CF As String) As InfoAssistito
        Return GenericProvider.Paziente.GetInfoAssistito(CF)
    End Function

    Public Function GetContattiAssistito(CF As String) As ContattiAssistito
        Return GenericProvider.Paziente.GetContattiAssistito(CF)
    End Function
    Public Function GetDocumenti(CF As String) As List(Of DtoDocumento)
        Dim result As New List(Of DtoDocumento)
        Dim c As ICollection(Of String) = GenericProvider.Paziente.GetCodicePazientiByCodiceFiscale(CF)
        If c.IsNullOrEmpty() Then

        ElseIf c.Count > 1 Then

        Else
            result = GenericProvider.Paziente.GetDocumentiAssistitoByCodice(c.First())

        End If
        Return result
    End Function

    Public Function GetVaccini(CF As String) As DtoVaccino

    End Function
    Public Function SetContattoAssistito(Contatto As DTOSetContatto) As SetContattoResult

        Dim result As New SetContattoResult()

        ' Controllo CF
        Dim c As ICollection(Of String) = GenericProvider.Paziente.GetCodicePazientiByCodiceFiscale(Contatto.CodiceFiscale)

        ' AndAlso => &&
        ' OrElse => ||
        If c.IsNullOrEmpty() Then
            result.Success = False
            result.Message = "Codice fiscale non presente"
        ElseIf c.Count > 1 Then
            result.Success = False
            result.Message = "Codice fiscale duplicato"
        Else
            result = GenericProvider.Paziente.SetContattoAssistito(Contatto)
        End If

        Return result

    End Function

    Public Function SetIndirizzoTemporaneo(indirizzoTemporaneo As DTOSetIndirizzoTemporaneo) As ResultSetPost

        Dim result As New ResultSetPost()
        Dim c As ICollection(Of String) = GenericProvider.Paziente.GetCodicePazientiByCodiceFiscale(indirizzoTemporaneo.CodiceFiscale)
        If c.IsNullOrEmpty() Then
            result.Success = False
            result.Message = "Codice fiscale non presente"
        ElseIf c.Count > 1 Then
            result.Success = False
            result.Message = "Codice fiscale duplicato"
        Else
            result = GenericProvider.Paziente.SetIndirizzoTemporaneo(indirizzoTemporaneo)
        End If

        Return result

    End Function
#Region " Consenso "

    ' N.B. : Metodi diretti sul consenso, implementati ad hoc solo per il Veneto (centrale) per ottimizzare la app vaccinale.


    Public Class GetUltimoConsensoRegistratoResult

        Public Property UltimoConsenso As Entities.Consenso.ConsensoRegistrato
        Public Property StatoControlloConsenso As Enumerators.ControlloConsenso

    End Class

    ''' <summary>
    ''' Restituisce l'ultimo consenso registrato (del tipo indicato) per il paziente specificato.
    ''' Non viene utilizzato il servizio di gestione consenso ma effettua direttamente le query su db
    ''' </summary>
    ''' <param name="codiceCentralePaziente"></param>
    ''' <param name="idConsenso"></param>
    ''' <param name="codiceAziendaRegistrazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetUltimoConsensoRegistrato(codiceCentralePaziente As String, idConsenso As Integer, codiceAziendaRegistrazione As String) As GetUltimoConsensoRegistratoResult

        Dim result As New GetUltimoConsensoRegistratoResult()

        result.UltimoConsenso = GenericProvider.Paziente.GetUltimoConsensoRegistratoPaziente(codiceCentralePaziente, idConsenso, codiceAziendaRegistrazione)

        If result.UltimoConsenso Is Nothing Then
            result.StatoControlloConsenso = Enumerators.ControlloConsenso.Undefined
        Else
            result.StatoControlloConsenso = ConvertiFlagControlloConsenso(result.UltimoConsenso.FlagControllo)
        End If

        Return result

    End Function

    ''' <summary>
    ''' Restituisce i dati anagrafici relativi al paziente referente e ai suoi pazienti collegati.
    ''' I dati anagrafici vengono letti dalla usl del comune di domicilio (o, se nullo, del comune di residenza).
    ''' Per tutti i pazienti, restituisce solo quelli aventi stato anagrafico attivo.
    ''' Per i pazienti collegati, restituisce solo quelli aventi il consenso indicato in uno stato non bloccante.
    ''' Il consenso del referente non viene controllato.
    ''' </summary>
    ''' <param name="codiceCentraleReferente"></param>
    ''' <param name="idConsenso"></param>
    ''' <param name="idLivelliBloccanti"></param>
    ''' <param name="codiceAziendaRegistrazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDatiPazientiReferenteECollegati(codiceCentraleReferente As String, idConsenso As Integer, idLivelliBloccanti As List(Of Integer), codiceAziendaRegistrazione As String) As List(Of DatiPazienteAPP)

        Dim listCodiciCentrali As New List(Of String)()
        listCodiciCentrali.Add(codiceCentraleReferente)

        ' Ricerca codici centrali dei pazienti collegati al referente (solo quelli con consenso non bloccante)
        Dim listUltimiConsensiCollegati As List(Of Consenso.ConsensoRegistrato) =
            GenericProvider.Paziente.GetUltimoConsensoRegistratoPazientiCollegati(codiceCentraleReferente, idConsenso, codiceAziendaRegistrazione)

        If Not listUltimiConsensiCollegati Is Nothing AndAlso listUltimiConsensiCollegati.Count > 0 Then

            If idLivelliBloccanti Is Nothing Then

                listUltimiConsensiCollegati =
                    listUltimiConsensiCollegati.Where(Function(p) ConvertiFlagControlloConsenso(p.FlagControllo) = Enumerators.ControlloConsenso.NonBloccante).ToList()

            Else

                listUltimiConsensiCollegati =
                    listUltimiConsensiCollegati.Where(Function(p) Not idLivelliBloccanti.Contains(p.LivelloId)).ToList()

            End If

            If Not listUltimiConsensiCollegati Is Nothing AndAlso listUltimiConsensiCollegati.Count > 0 Then

                Dim listCodiciCentraliPazientiCollegati As List(Of String) = listUltimiConsensiCollegati.Select(Function(p) p.CodicePaziente).ToList()
                If Not listCodiciCentraliPazientiCollegati Is Nothing Then listCodiciCentrali.AddRange(listCodiciCentraliPazientiCollegati)

            End If

        End If

        ' Recupero info sui pazienti distribuiti (codice centrale, locale e usl)
        Dim listInfoDistribuzione As List(Of PazienteInfoDistribuzione) =
            Me.GenericProvider.Paziente.GetListInfoDistribuzionePazienti(listCodiciCentrali)

        If listInfoDistribuzione Is Nothing OrElse listInfoDistribuzione.Count = 0 Then
            Return Nothing
        End If

        Dim listDatiPazientiAPP As New List(Of DatiPazienteAPP)()

        ' Ricerca pazienti nel db locale indicato dall'appId per controllare se lo stato anagrafico è ATTIVO
        Dim listAppId As IEnumerable(Of String) = listInfoDistribuzione.Select(Function(p) p.AppIdUsl).Distinct()

        ' [Unificazione Ulss]: rimappare appId delle ULSS già unificate
        For Each appId As String In listAppId

            Dim listCodiciLocaliPazientiUsl As List(Of Int64) =
                listInfoDistribuzione.Where(Function(p) p.AppIdUsl = appId).Select(Function(item) item.CodiceLocalePaziente).ToList()

            Using genericProviderUsl As DbGenericProvider = GenericProviderFactory.GetDbGenericProvider(appId, ContextInfos.CodiceAzienda)

                Dim pazientiAttiviUsl As List(Of DatiPazienteAPP) =
                    genericProviderUsl.Paziente.GetPazientiStatoAnagraficoAttivo(listCodiciLocaliPazientiUsl)

                If Not pazientiAttiviUsl Is Nothing AndAlso pazientiAttiviUsl.Count > 0 Then

                    Dim datiPazienteAPP As DatiPazienteAPP = Nothing

                    For Each paz As DatiPazienteAPP In pazientiAttiviUsl

                        datiPazienteAPP = paz.Clone()
                        datiPazienteAPP.AppIdUsl = appId
                        datiPazienteAPP.CodiceCentralePaziente =
                            listInfoDistribuzione.First(Function(p) p.AppIdUsl = appId And p.CodiceLocalePaziente = paz.CodiceLocalePaziente).CodiceCentralePaziente

                        listDatiPazientiAPP.Add(datiPazienteAPP)

                    Next

                End If

            End Using

        Next

        Return listDatiPazientiAPP

    End Function

#End Region

    Public Class GetCertificatoVaccinaleCommand

        Public Property CodicePaziente As Long

        ''' <summary>
        ''' Percorso fisico e nome del file .rpt
        ''' </summary>
        ''' <returns></returns>
        Public Property ReportName As String

        Public Property StampaNotaValidita As Boolean

        Public Property StampaLottoNomeCommerciale As Boolean

        Public Property StampaScrittaCertificato As Boolean

        Public Property CodiceUslCorrente As String

    End Class

    ''' <summary>
    ''' Crea il report del certificato vaccinale e lo restituisce come stream di byte. Il parametro reportName deve contenere il percorso fisico del file .rpt.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="reportName"></param>
    ''' <param name="stampaNotaValidita"></param>
    ''' <param name="stampaLottoNomeCommerciale"></param>
    ''' <param name="stampaScrittaCertificato"></param>
    ''' <returns></returns>
    Public Function GetCertificatoVaccinale(command As GetCertificatoVaccinaleCommand) As Byte()

        'TODO [FSE]: Per i TEST: command.CodicePaziente = 590049

        If String.IsNullOrWhiteSpace(command.CodiceUslCorrente) Then command.CodiceUslCorrente = ContextInfos.CodiceUsl

        Dim codiceConsultorio As String = GetCodiceConsultorio(command.CodicePaziente)

        Using rptDocument As New CrystalDecisions.CrystalReports.Engine.ReportDocument()

            ' Caricamento file .rpt
            rptDocument.Load(command.ReportName, CrystalDecisions.Shared.OpenReportMethod.OpenReportByDefault)

            ' Impostazione della connessione a database
            Dim crypt As New [Shared].NTier.Security.Crypto([Shared].NTier.Security.Providers.Rijndael)

            Dim crConnectionInfo As New CrystalDecisions.Shared.ConnectionInfo()
            crConnectionInfo.ServerName = Applicazione.ConnServername
            crConnectionInfo.DatabaseName = Applicazione.ConnDbName
            crConnectionInfo.UserID = Applicazione.ConnUserId
            crConnectionInfo.Password = crypt.Decrypt(Applicazione.ConnPassword)

            Dim crTables As CrystalDecisions.CrystalReports.Engine.Tables = rptDocument.Database.Tables
            Dim crtableLogoninfo As New CrystalDecisions.Shared.TableLogOnInfo()

            For Each crTable As CrystalDecisions.CrystalReports.Engine.Table In crTables
                crtableLogoninfo = crTable.LogOnInfo
                crtableLogoninfo.ConnectionInfo = crConnectionInfo
                crTable.ApplyLogOnInfo(crtableLogoninfo)
            Next

            rptDocument.Refresh()

            ' Export dello stream
            Dim s As IO.MemoryStream

            Try
                rptDocument.RecordSelectionFormula = "{T_PAZ_PAZIENTI.PAZ_CODICE} = " + command.CodicePaziente.ToString()

                Dim consultorio As Cns = Nothing

                Using bizConsultori As New BizConsultori(GenericProvider, Settings, ContextInfos, LogOptions)
                    If Not String.IsNullOrWhiteSpace(codiceConsultorio) Then
                        consultorio = bizConsultori.GetConsultorio(codiceConsultorio)
                    End If
                End Using

                If consultorio Is Nothing Then
                    AddParameter(rptDocument, "DescrizioneComune", String.Empty)
                    AddParameter(rptDocument, "cnsStampaIndirizzo", String.Empty)
                    AddParameter(rptDocument, "cnsStampaCap", String.Empty)
                    AddParameter(rptDocument, "cnsStampaComune", String.Empty)
                    AddParameter(rptDocument, "cnsStampaTelefono", String.Empty)
                Else
                    AddParameter(rptDocument, "DescrizioneComune", consultorio.Comune)
                    AddParameter(rptDocument, "cnsStampaIndirizzo", consultorio.Indirizzo)
                    AddParameter(rptDocument, "cnsStampaCap", consultorio.Cap)
                    AddParameter(rptDocument, "cnsStampaComune", consultorio.Comune)
                    AddParameter(rptDocument, "cnsStampaTelefono", consultorio.Telefono)
                End If

                AddParameter(rptDocument, "cnsStampaCodice", codiceConsultorio)

                If command.StampaNotaValidita Then
                    AddParameter(rptDocument, "notaValidita", Settings.CERTIFICATO_VACCINALE_NOTA_VALIDITA)
                Else
                    AddParameter(rptDocument, "notaValidita", String.Empty)
                End If

                If command.StampaLottoNomeCommerciale Then
                    AddParameter(rptDocument, "StampaLotto", "S")
                Else
                    AddParameter(rptDocument, "StampaLotto", "N")
                End If

                Dim scrittaCertificato As String = String.Empty

                If command.StampaScrittaCertificato Then

                    scrittaCertificato = GetEsitoControlloSituazioneVaccinale(command.CodicePaziente)

                End If

                AddParameter(rptDocument, "scrittaCertificato", scrittaCertificato)

                AddParameter(rptDocument, "Installazione", command.CodiceUslCorrente)

                Dim exp As New CrystalDecisions.Shared.ExportOptions()
                exp.ExportFormatType = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat

                Dim req As New CrystalDecisions.Shared.ExportRequestContext()
                req.ExportInfo = exp

                s = rptDocument.FormatEngine.ExportToStream(req)

            Finally
                rptDocument.Close()
            End Try

            Return s.ToArray()

        End Using

    End Function

#Region " Creazione report COVID "

    Public Class GetComunicazioneSorveglianzaCOVID19Command
        Public Property CodicePaziente As Long
        Public Property DataInizioSorveglianza As Date?
        Public Property DataFineSorveglianza As Date?
        Public Property DataStampa As Date?
        Public Property PercorsoReport As String
        Public Property CodiceUslCorrente As String
    End Class

    Public Function GetComunicazioneSorveglianzaCOVID19(command As GetComunicazioneSorveglianzaCOVID19Command) As IO.Stream

        If command.CodicePaziente <= 0 Then
            Throw New Exception("Specificare il paziente")
        End If

        If String.IsNullOrWhiteSpace(command.CodiceUslCorrente) Then

            If String.IsNullOrWhiteSpace(ContextInfos.CodiceUsl) Then
                Throw New Exception("Specificare la ULSS di lavoro")
            End If

            command.CodiceUslCorrente = ContextInfos.CodiceUsl

        End If

        Using rptDocument As New CrystalDecisions.CrystalReports.Engine.ReportDocument()

            ' Caricamento file .rpt
            rptDocument.Load(command.PercorsoReport, CrystalDecisions.Shared.OpenReportMethod.OpenReportByDefault)

            ' Impostazione della connessione a database
            Dim crypt As New [Shared].NTier.Security.Crypto([Shared].NTier.Security.Providers.Rijndael)

            Dim crConnectionInfo As New CrystalDecisions.Shared.ConnectionInfo()
            crConnectionInfo.ServerName = Applicazione.ConnServername
            crConnectionInfo.DatabaseName = Applicazione.ConnDbName
            crConnectionInfo.UserID = Applicazione.ConnUserId
            crConnectionInfo.Password = crypt.Decrypt(Applicazione.ConnPassword)

            Dim crTables As CrystalDecisions.CrystalReports.Engine.Tables = rptDocument.Database.Tables
            Dim crtableLogoninfo As New CrystalDecisions.Shared.TableLogOnInfo()

            For Each crTable As CrystalDecisions.CrystalReports.Engine.Table In crTables
                crtableLogoninfo = crTable.LogOnInfo
                crtableLogoninfo.ConnectionInfo = crConnectionInfo
                crTable.ApplyLogOnInfo(crtableLogoninfo)
            Next

            rptDocument.Refresh()

            ' Export dello stream
            Try
                rptDocument.RecordSelectionFormula =
                    String.Format("{{T_PAZ_PAZIENTI.PAZ_CODICE}} = {0} AND {{T_ANA_INSTALLAZIONI.INS_USL_CODICE}} = '{1}'",
                                  command.CodicePaziente.ToString(), command.CodiceUslCorrente)

                AddParameter(rptDocument, "Installazione", command.CodiceUslCorrente)

                If command.DataStampa.HasValue Then
                    AddParameter(rptDocument, "DataStampa", command.DataStampa.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture))
                Else
                    AddParameter(rptDocument, "DataStampa", String.Empty)
                End If

                If command.DataInizioSorveglianza.HasValue Then
                    AddParameter(rptDocument, "DataInizioSorveglianza", command.DataInizioSorveglianza.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture))
                Else
                    AddParameter(rptDocument, "DataInizioSorveglianza", String.Empty)
                End If

                If command.DataFineSorveglianza.HasValue Then
                    AddParameter(rptDocument, "DataFineSorveglianza", command.DataFineSorveglianza.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture))
                Else
                    AddParameter(rptDocument, "DataFineSorveglianza", String.Empty)
                End If

                Dim exp As New CrystalDecisions.Shared.ExportOptions()
                exp.ExportFormatType = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat

                Dim req As New CrystalDecisions.Shared.ExportRequestContext()
                req.ExportInfo = exp

                Return rptDocument.FormatEngine.ExportToStream(req)

            Finally
                rptDocument.Close()
            End Try

        End Using

    End Function

    Public Class GetCertificatoNegativizzazioneCOVID19Command
        Public Property CodicePaziente As Long
        Public Property DataStampa As Date?
        Public Property PercorsoReport As String
        Public Property CodiceUslCorrente As String
        Public Property IdEpisodio As Long
    End Class

    Public Function GetCertificatoNegativizzazioneCOVID19(command As GetCertificatoNegativizzazioneCOVID19Command) As IO.Stream

        If command.CodicePaziente <= 0 Then
            Throw New Exception("Specificare il paziente")
        End If

        If String.IsNullOrWhiteSpace(command.CodiceUslCorrente) Then

            If String.IsNullOrWhiteSpace(ContextInfos.CodiceUsl) Then
                Throw New Exception("Specificare la ULSS di lavoro")
            End If

            command.CodiceUslCorrente = ContextInfos.CodiceUsl

        End If

        Dim listTamponi As List(Of TamponeCertificatoNeg) = GenericProvider.RilevazioniCovid19.GetTamponiCertificatoNeg(command.IdEpisodio)

        If listTamponi.IsNullOrEmpty() Then
            Throw New Exception("Nessun tampone effettuato")
        End If

        ' Lista ordinata per data, dalla più vecchia
        listTamponi = listTamponi.OrderBy(Function(t1) t1.DataTampone).ThenBy(Function(t2) t2.CodiceTampone).ToList()

        ' Dalla lista tamponi va tirato fuori il primo POSITIVO 
        ' N.B. : ATTENZIONE => primoTamponePositivo potrebbe essere Nothing!!! Si deve andare avanti comunque e passare al report la data vuota
        Dim primoTamponePositivo As TamponeCertificatoNeg = listTamponi.FirstOrDefault(Function(t) t.CodiceEsito = Settings.COV_ESITO_POSITIVO_TAMPONE)

        ' Lista ordinata per data, dalla più recente
        listTamponi = listTamponi.OrderByDescending(Function(t1) t1.DataTampone).ThenByDescending(Function(t2) t2.CodiceTampone).ToList()

        ' Dalla lista tamponi vanno presi i due NEGATIVI più recenti consecutivi. 
        ' Se non è stato trovato un tampone, la data corrispondente resta nulla

        ' Casi possibili nella lista, partendo sempre dal fondo (più recenti):
        ' 2 negativi => i primi due tamponi hanno esito = "N":
        '               valorizzo entrambe le date. Il primo tampone (data + recente) è tamponeNegativo2, il secondo è tamponeNegativo1
        ' 1 negativo => il primo che trovo è N, il secondo è S:
        '               valorizzo solo tamponeNegativo2, che è quello negativo e + recente
        ' 0 negativi => il primo che trovo è S
        '               non valorizzo nessuno dei due oggetti

        Dim dataTamponeNegativo1 As Date? = Nothing
        Dim dataTamponeNegativo2 As Date? = Nothing ' => questa deve essere la più recente delle due

        If listTamponi(0).CodiceEsito = Settings.COV_ESITO_NEGATIVO_TAMPONE Then

            dataTamponeNegativo1 = listTamponi(0).DataTampone

            If listTamponi.Count > 1 Then
                If listTamponi(1).CodiceEsito = Settings.COV_ESITO_NEGATIVO_TAMPONE Then
                    ' Siccome ci sono entrambe le date, tengo la 2 come data più recente 
                    dataTamponeNegativo1 = listTamponi(1).DataTampone
                    dataTamponeNegativo2 = listTamponi(0).DataTampone
                End If
            End If

        End If

        Using rptDocument As New CrystalDecisions.CrystalReports.Engine.ReportDocument()

            ' Caricamento file .rpt
            rptDocument.Load(command.PercorsoReport, CrystalDecisions.Shared.OpenReportMethod.OpenReportByDefault)

            ' Impostazione della connessione a database
            Dim crypt As New [Shared].NTier.Security.Crypto([Shared].NTier.Security.Providers.Rijndael)

            Dim crConnectionInfo As New CrystalDecisions.Shared.ConnectionInfo()
            crConnectionInfo.ServerName = Applicazione.ConnServername
            crConnectionInfo.DatabaseName = Applicazione.ConnDbName
            crConnectionInfo.UserID = Applicazione.ConnUserId
            crConnectionInfo.Password = crypt.Decrypt(Applicazione.ConnPassword)

            Dim crTables As CrystalDecisions.CrystalReports.Engine.Tables = rptDocument.Database.Tables
            Dim crtableLogoninfo As New CrystalDecisions.Shared.TableLogOnInfo()

            For Each crTable As CrystalDecisions.CrystalReports.Engine.Table In crTables
                crtableLogoninfo = crTable.LogOnInfo
                crtableLogoninfo.ConnectionInfo = crConnectionInfo
                crTable.ApplyLogOnInfo(crtableLogoninfo)
            Next

            rptDocument.Refresh()

            ' Export dello stream
            Try
                rptDocument.RecordSelectionFormula =
                    String.Format("{{T_PAZ_PAZIENTI.PAZ_CODICE}} = {0} AND {{T_ANA_INSTALLAZIONI.INS_USL_CODICE}} = '{1}'",
                                  command.CodicePaziente.ToString(), command.CodiceUslCorrente)

                AddParameter(rptDocument, "Installazione", command.CodiceUslCorrente)

                If command.DataStampa.HasValue Then
                    AddParameter(rptDocument, "DataStampa", command.DataStampa.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture))
                Else
                    AddParameter(rptDocument, "DataStampa", String.Empty)
                End If

                If primoTamponePositivo?.DataTampone.HasValue Then
                    AddParameter(rptDocument, "DataTamponePositivo", primoTamponePositivo.DataTampone.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture))
                Else
                    AddParameter(rptDocument, "DataTamponePositivo", String.Empty)
                End If

                If dataTamponeNegativo1.HasValue Then
                    AddParameter(rptDocument, "DataTamponeNegativo1", dataTamponeNegativo1.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture))
                Else
                    AddParameter(rptDocument, "DataTamponeNegativo1", String.Empty)
                End If

                If dataTamponeNegativo2.HasValue Then
                    AddParameter(rptDocument, "DataTamponeNegativo2", dataTamponeNegativo2.Value.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture))
                Else
                    AddParameter(rptDocument, "DataTamponeNegativo2", String.Empty)
                End If

                Dim exp As New CrystalDecisions.Shared.ExportOptions()
                exp.ExportFormatType = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat

                Dim req As New CrystalDecisions.Shared.ExportRequestContext()
                req.ExportInfo = exp

                Return rptDocument.FormatEngine.ExportToStream(req)

            Finally
                rptDocument.Close()
            End Try

        End Using

    End Function

    Public Class GetCertificatoTestAntigeneRapidoNegativoCOVID19Command
        Public Property CodicePaziente As Long?
        Public Property NumeroCampione As Long?
        Public Property CodiceCentro As String
        Public Property DataStampa As Date?
        Public Property PercorsoReport As String
        Public Property PercorsoReport_TAR As String
        Public Property CodiceUslCorrente As String
        Public Property DataTest As Date
    End Class

    Public Function GetCertificatoTestAntigeneRapidoNegativoCOVID19(command As GetCertificatoTestAntigeneRapidoNegativoCOVID19Command) As IO.Stream

        If (Not command.CodicePaziente.HasValue OrElse command.CodicePaziente.Value <= 0) AndAlso
           (Not command.NumeroCampione.HasValue OrElse command.NumeroCampione.Value <= 0) Then
            Throw New Exception("Specificare il paziente o il numero del campione")
        End If

        Dim fromPaz As Boolean = command.CodicePaziente.HasValue AndAlso command.CodicePaziente.Value > 0

        Dim codiceUslCorrente As String = command.CodiceUslCorrente

        ' TODO [COV - cert test neg]: ricavare ulss da centro vac paz?
        If String.IsNullOrWhiteSpace(codiceUslCorrente) Then

            If String.IsNullOrWhiteSpace(ContextInfos.CodiceUsl) Then
                Throw New Exception("Specificare la ULSS di lavoro")
            End If

            codiceUslCorrente = ContextInfos.CodiceUsl

        End If

        Using rptDocument As New CrystalDecisions.CrystalReports.Engine.ReportDocument()

            InitReportDocument(rptDocument, If(fromPaz, command.PercorsoReport, command.PercorsoReport_TAR))

            ' Export dello stream
            Try
                If fromPaz Then

                    rptDocument.RecordSelectionFormula =
                        String.Format("{{T_PAZ_PAZIENTI.PAZ_CODICE}} = {0} AND {{T_ANA_INSTALLAZIONI.INS_USL_CODICE}} = '{1}'",
                                      command.CodicePaziente.ToString(), codiceUslCorrente)

                Else

                    rptDocument.RecordSelectionFormula =
                        String.Format("{{T_ANA_INSTALLAZIONI.INS_USL_CODICE}} = '{0}' ", codiceUslCorrente)

                    If Not String.IsNullOrWhiteSpace(command.CodiceCentro) Then
                        rptDocument.RecordSelectionFormula +=
                            String.Format(" AND {{T_ANA_SCARICO_SCREENING_TAR.SCT_CENTRO}} = '{0}' ", command.CodiceCentro)
                    End If

                    rptDocument.RecordSelectionFormula +=
                        String.Format(" AND {{T_ANA_SCARICO_SCREENING_TAR.SCT_CAMPIONE_NR}} = {0} ", command.NumeroCampione.Value)

                End If

                AddParameter(rptDocument, "Installazione", codiceUslCorrente)

                Dim dataStampa As Date

                If command.DataStampa.HasValue Then
                    dataStampa = command.DataStampa.Value
                Else
                    dataStampa = Date.Now
                End If

                AddParameter(rptDocument, "DataStampa", dataStampa.ToString("dd/MM/yyyy", Globalization.CultureInfo.InvariantCulture))

                If command.DataTest = Date.MinValue Then
                    AddParameter(rptDocument, "DataTest", String.Empty)
                Else
                    AddParameter(rptDocument, "DataTest", command.DataTest.ToString("dd/MM/yyyy", Globalization.CultureInfo.InvariantCulture))
                End If

                Dim exp As New CrystalDecisions.Shared.ExportOptions()
                exp.ExportFormatType = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat

                Dim req As New CrystalDecisions.Shared.ExportRequestContext()
                req.ExportInfo = exp

                Return rptDocument.FormatEngine.ExportToStream(req)

            Finally
                rptDocument.Close()
            End Try

        End Using

    End Function

    Private Sub InitReportDocument(ByRef rptDocument As CrystalDecisions.CrystalReports.Engine.ReportDocument, percorsoReport As String)

        ' Caricamento file .rpt
        rptDocument.Load(percorsoReport, CrystalDecisions.Shared.OpenReportMethod.OpenReportByDefault)

        ' Impostazione della connessione a database
        Dim crypt As New [Shared].NTier.Security.Crypto([Shared].NTier.Security.Providers.Rijndael)

        Dim crConnectionInfo As New CrystalDecisions.Shared.ConnectionInfo()
        crConnectionInfo.ServerName = Applicazione.ConnServername
        crConnectionInfo.DatabaseName = Applicazione.ConnDbName
        crConnectionInfo.UserID = Applicazione.ConnUserId
        crConnectionInfo.Password = crypt.Decrypt(Applicazione.ConnPassword)

        Dim crTables As CrystalDecisions.CrystalReports.Engine.Tables = rptDocument.Database.Tables
        Dim crtableLogoninfo As New CrystalDecisions.Shared.TableLogOnInfo()

        For Each crTable As CrystalDecisions.CrystalReports.Engine.Table In crTables
            crtableLogoninfo = crTable.LogOnInfo
            crtableLogoninfo.ConnectionInfo = crConnectionInfo
            crTable.ApplyLogOnInfo(crtableLogoninfo)
        Next

        rptDocument.Refresh()

    End Sub

#End Region

#Region " FSE "

    ''' <summary>
    ''' Restituisce il pazCodice trovato in base al DocumentUniqueId
    ''' </summary>
    ''' <param name="documentUniqueId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCodicePazienteByDocumentUniqueId(documentUniqueId As String) As Integer?

        Dim codicePaziente As Integer? = GenericProvider.Paziente.GetCodicePazienteByDocumentUniqueId(documentUniqueId, Constants.TipoDocumentoFSE.CertificatoVaccinale)

        Return codicePaziente

    End Function

    ''' <summary>
    ''' Restituisce il DocumentUniqueId trovato in base al pazCodice
    ''' </summary>
    ''' <param name="pazCodice"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDocumentUniqueIdByCodicePaziente(pazCodice As Long, tipoDocumento As String) As String

        Dim documentUniqueId As String = GenericProvider.Paziente.GetDocumentUniqueIdByCodicePaziente(pazCodice, tipoDocumento)

        Return documentUniqueId

    End Function

#End Region

#End Region

#Region " Batch Recupero Storico Vaccinale "

    'Public Class BatchRecuperoStoricoVaccinale

    '    <DebuggerNonUserCode>
    '    Public Sub New()
    '    End Sub

    '    Public Class ParameterName

    '        Public Const IdApplicazioneCentrale As String = "IdApplicazioneCentrale"
    '        Public Const IdApplicazioneLocaleRecuperoStorico As String = "IdApplicazioneLocaleRecuperoStorico"
    '        Public Const IdUtenteRecuperoStorico As String = "IdUtenteRecuperoStorico"
    '        Public Const IdProcessoBatch As String = "IdProcessoBatch"
    '        Public Const ForzaRecuperoStorico As String = "ForzaRecuperoStorico"
    '        Public Const CodiciCentraliPazienti As String = "CodiciCentraliPazienti"
    '        Public Const NumeroElaborazioniRefreshRisultatoParziale As String = "NumeroElaborazioniRefreshRisultatoParziale"

    '        <DebuggerNonUserCode>
    '        Public Sub New()
    '        End Sub

    '    End Class

    '    Public Class RecuperoStoricoVaccinaleCommand

    '        ''' <summary>
    '        ''' Id dell'utente che effettua il recupero dello storico vaccinale.
    '        ''' </summary>
    '        ''' <remarks></remarks>
    '        Public IdUtenteRecuperoStorico As Long

    '        ''' <summary>
    '        ''' Id del processo batch che si sta eseguendo.
    '        ''' </summary>
    '        ''' <remarks></remarks>
    '        Public IdProcessoBatch As Long

    '        ''' <summary>
    '        ''' Se true: per ogni paziente, imposta lo stato acquisizione a null ed esegue il recupero dello storico.
    '        ''' Se false: effettua il recupero dello storico solo per i pazienti con stato acquisizione null.
    '        ''' </summary>
    '        ''' <remarks></remarks>
    '        Public ForzaRecuperoStorico As Boolean

    '        ''' <summary>
    '        ''' Lista codici centrali dei pazienti per cui recuperare lo storico vaccinale
    '        ''' </summary>
    '        ''' <remarks></remarks>
    '        Public CodiciCentraliPazienti As List(Of String)

    '        ''' <summary>
    '        ''' Numero di pazienti dopo i quali effettuare un refresh dei risultati parziali dell'elaborazione
    '        ''' </summary>
    '        ''' <remarks></remarks>
    '        Public NumeroElaborazioniRefreshRisultatoParziale As Integer

    '    End Class

    'End Class

    'Public Event RefreshTotalePazientiRecuperoStoricoVaccinale(e As BizBatch.RefreshTotaleElementiDaElaborareEventArgs)
    'Public Event RefreshParzialePazientiStoricoVaccinaleRecuperato(e As BizBatch.RefreshParzialeElementiElaboratiEventArgs)

    'Public Function RecuperoStoricoVaccinalePazienti(command As BatchRecuperoStoricoVaccinale.RecuperoStoricoVaccinaleCommand) As BizGenericResult

    '    Dim result As New BizGenericResult()
    '    result.Success = True
    '    result.Message = String.Empty

    '    Dim numeroElaborazioni As Integer = 0
    '    Dim numeroErrori As Integer = 0


    '    ' TODO [BatchRecuperoStoricoVaccinalePazienti]: da implementare e testare!

    '    ' 1 - Ricerca (in centrale) dei codici centrali dei pazienti da recuperare => in base a dati disallineati vaccinazioni centrale-distribuite
    '    Dim listCodiciCentrali As New List(Of String)()

    '    ' 2 - Inserimento codici in tabella appoggio (centrale) T_RECUPERO_STORICO 

    '    If command.ForzaRecuperoStorico Then
    '        ' 3a - Update stato acquisizione per tutti i pazienti trovati
    '    Else
    '        ' 3b - Filtro nella ulss locale per i codici centrali e per stato acquisizione =0 o =2. Se trovo il paziente, non devo forzare il recupero quindi lo elimino dalla T_RECUPERO_STORICO
    '    End If

    '    ' 4 - Acquisizione per ogni paziente della T_RECUPERO_STORICO
    '    For Each codiceAusiliario As String In listCodiciCentrali

    '        'Dim command As New Common.OnVacStoricoVaccinaleCentralizzato.AcquisisciStoricoCommand()
    '        'command.CodicePaziente = OnVacUtility.Variabili.PazId
    '        'command.RichiediConfermaSovrascrittura = False
    '        'command.Settings = Me.Settings
    '        'command.OnitLayout3 = Me.OnitLayout31
    '        'command.BizLogOptions = Nothing
    '        'command.Note = "Recupero Storico Vaccinale da maschera Visite"

    '        'Dim acquisisciDatiVaccinaliCentraliResult As Biz.BizPaziente.AcquisisciDatiVaccinaliCentraliResult =
    '        'Common.OnVacStoricoVaccinaleCentralizzato.AcquisisciDatiVaccinaliCentraliPaziente(command)

    '    Next

    '    Return result

    'End Function

    ''Private Function AcquisisciDatiVaccinaliCentraliPaziente(command As AcquisisciStoricoCommand) As Biz.BizPaziente.AcquisisciDatiVaccinaliCentraliResult

    ''    Dim acquisisciDatiVaccinaliCentraliResult As Biz.BizPaziente.AcquisisciDatiVaccinaliCentraliResult

    ''    Using transactionScope As New System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, OnVacUtility.GetReadCommittedTransactionOptions())

    ''        Using bizPaziente As New Biz.BizPaziente(command.Settings, Nothing, OnVacContext.CreateBizContextInfos(), command.BizLogOptions)

    ''            Dim note As String = command.Note
    ''            If String.IsNullOrWhiteSpace(note) Then note = "Recupero Storico Vaccinale"

    ''            Dim acquisisciDatiVaccinaliCentraliCommand As New Biz.BizPaziente.AcquisisciDatiVaccinaliCentraliCommand()
    ''            acquisisciDatiVaccinaliCentraliCommand.CodicePaziente = command.CodicePaziente
    ''            acquisisciDatiVaccinaliCentraliCommand.CodicePazienteCentrale = bizPaziente.GenericProvider.Paziente.GetCodiceAusiliario(command.CodicePaziente)
    ''            acquisisciDatiVaccinaliCentraliCommand.CodiceConsultorioPaziente = bizPaziente.GenericProvider.Paziente.GetCodiceConsultorio(command.CodicePaziente)
    ''            acquisisciDatiVaccinaliCentraliCommand.RichiediConfermaSovrascrittura = command.RichiediConfermaSovrascrittura
    ''            acquisisciDatiVaccinaliCentraliCommand.Note = note

    ''            acquisisciDatiVaccinaliCentraliResult = bizPaziente.AcquisisciDatiVaccinaliCentrali(acquisisciDatiVaccinaliCentraliCommand)

    ''        End Using

    ''        transactionScope.Complete()

    ''    End Using

    ''    Dim recuperoStoricoVaccinaleStringBuilder As New System.Text.StringBuilder()

    ''    If Not acquisisciDatiVaccinaliCentraliResult.StatoAcquisizioneDatiVaccinaliCentrale.HasValue Then

    ''        recuperoStoricoVaccinaleStringBuilder.Append("ATTENZIONE\n\nDurante l\'acquisizione sono stati riscontrati i seguenti problemi:\n\n")

    ''        OnVacStoricoVaccinaleCentralizzato.AddAcquisizionePartMessage(recuperoStoricoVaccinaleStringBuilder, "VACCINAZIONI ESCLUSE", acquisisciDatiVaccinaliCentraliResult.VaccinazioniEsclusePresenti)
    ''        OnVacStoricoVaccinaleCentralizzato.AddAcquisizionePartMessage(recuperoStoricoVaccinaleStringBuilder, "PROGRAMMAZIONE", acquisisciDatiVaccinaliCentraliResult.ProgrammazionePresente)

    ''        recuperoStoricoVaccinaleStringBuilder.Append("\nProcedere con l\'acquisizione sovrascrivendo i dati in locale ?")

    ''        command.OnitLayout3.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(recuperoStoricoVaccinaleStringBuilder.ToString(),
    ''                                                                                          OnVacStoricoVaccinaleCentralizzato.AcquisisciDatiVaccinaliCentraliConfirmKey,
    ''                                                                                          True, True))

    ''    Else

    ''        If acquisisciDatiVaccinaliCentraliResult.StatoAcquisizioneDatiVaccinaliCentrale = Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Parziale Then

    ''            recuperoStoricoVaccinaleStringBuilder.AppendFormat("Il recupero dello storico vaccinale e\' stato completato con errori:\n\n{0}", acquisisciDatiVaccinaliCentraliResult.BuildMessage("\n"))

    ''        Else

    ''            recuperoStoricoVaccinaleStringBuilder.Append("Il recupero dello storico vaccinale e\' stato completato con successo.")

    ''        End If

    ''        command.OnitLayout3.InsertRoutineJS("alert('" + recuperoStoricoVaccinaleStringBuilder.ToString() + "');")

    ''    End If

    ''    Return acquisisciDatiVaccinaliCentraliResult

    ''End Function

#End Region

#Region " FSE "

    Public Function IndicizzaSuRegistry(codPaziente As Long, tipoDocumento As String, funzionalitaLog As String, eventoLog As String, settings As Settings.Settings, codiceOperatore As String) As BizGenericResult

        Dim result As New BizGenericResult()
        result.Success = False

        '1) controllo che non sia già stato indicizzato sul registry regionale
        Dim isPazienteIndicizzato As Boolean = Not GetDocumentUniqueIdByCodicePaziente(codPaziente, tipoDocumento).IsNullOrEmpty()

        If Not isPazienteIndicizzato Then
            'paziente da indicizzare

            '2) controllo che non sia già stato inserito il record del log notifica (in attesa che sia processato dal poller)
            Dim existLogNotificaInCorso As Boolean
            Using bizLogNotifiche As New BizLogNotifiche(GenericProvider, ContextInfos)
                existLogNotificaInCorso = bizLogNotifiche.ExistLogNotificaDaInviare(codPaziente, Enumerators.OperazioneLogNotifica.IndicizzazioneCertVaccFSE)
            End Using

            If existLogNotificaInCorso Then
                'paziente non ancora indicizzato ma log notifica già inserito in attesa di essere processato
                result.Success = True
            Else

                '3) INSERT del log notifica
                Dim resultLog As New BizLogNotifiche.InsertLogNotificheDaInviareResult()

                Using bizLogNotifiche As New BizLogNotifiche(GenericProvider, ContextInfos)

                    Dim command As New BizLogNotifiche.InsertLogNotificheDaInviareCommand() With {
                        .CodicePaziente = codPaziente,
                        .FunzionalitaNotifica = funzionalitaLog,
                        .EventoNotifica = eventoLog,
                        .Operazione = Enumerators.OperazioneLogNotifica.IndicizzazioneCertVaccFSE,
                        .Operatore = codiceOperatore
                    }

                    resultLog = bizLogNotifiche.InsertLogNotificheDaInviare(command)

                End Using


                result.Success = resultLog.Success
                result.Message = resultLog.Message

            End If

        Else
            'paziente già indicizzato
            result.Success = True
        End If

        Return result

    End Function


#End Region

End Class
