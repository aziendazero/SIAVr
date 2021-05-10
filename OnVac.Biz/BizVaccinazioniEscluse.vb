Imports System.Collections.Generic
Imports System.Transactions
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.Log
Imports Onit.OnAssistnet.OnVac.Log.DataLogStructure


Public Class BizVaccinazioniEscluse
    Inherits BizClass

#Region " Costruttori "

    Public Sub New(dbGenericProviderFactory As DbGenericProviderFactory, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)

        MyBase.New(dbGenericProviderFactory, settings, Nothing, contextInfos, logOptions)

    End Sub

    Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)

        Me.New(genericprovider, settings, Nothing, contextInfos, logOptions)

    End Sub

    Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, uslGestitaAllineaSettingsProvider As BizUslGestitaAllineaSettingsProvider, contextInfos As BizContextInfos, logOptions As BizLogOptions)

        MyBase.New(genericprovider, settings, uslGestitaAllineaSettingsProvider, contextInfos, logOptions)

    End Sub

#End Region

#Region " IEqualityComparers "

    Private Class UslInserimentoEsclusaComparer
        Implements IEqualityComparer(Of Entities.VaccinazioneEsclusaCentrale)

        Public Function Equals1(x As Entities.VaccinazioneEsclusaCentrale, y As Entities.VaccinazioneEsclusaCentrale) As Boolean Implements IEqualityComparer(Of Entities.VaccinazioneEsclusaCentrale).Equals

            If String.IsNullOrEmpty(x.CodiceUslVaccinazioneEsclusa) AndAlso String.IsNullOrEmpty(y.CodiceUslVaccinazioneEsclusa) Then
                Return False
            End If

            Return (x.CodiceUslVaccinazioneEsclusa = y.CodiceUslVaccinazioneEsclusa)

        End Function

        Public Function GetHashCode1(obj As Entities.VaccinazioneEsclusaCentrale) As Integer Implements IEqualityComparer(Of Entities.VaccinazioneEsclusaCentrale).GetHashCode

            Return obj.CodiceUslVaccinazioneEsclusa.GetHashCode()

        End Function

    End Class

    Private Class UslInserimentoConflittoEscluseComparer
        Implements IEqualityComparer(Of Entities.ConflittoVaccinazioniEscluse.DatiEsclusioneInConflitto)

        Public Function Equals1(x As Entities.ConflittoVaccinazioniEscluse.DatiEsclusioneInConflitto, y As Entities.ConflittoVaccinazioniEscluse.DatiEsclusioneInConflitto) As Boolean Implements IEqualityComparer(Of Entities.ConflittoVaccinazioniEscluse.DatiEsclusioneInConflitto).Equals

            If String.IsNullOrEmpty(x.CodiceUslVaccinazioneEsclusa) AndAlso String.IsNullOrEmpty(y.CodiceUslVaccinazioneEsclusa) Then
                Return False
            End If

            Return (x.CodiceUslVaccinazioneEsclusa = y.CodiceUslVaccinazioneEsclusa)

        End Function

        Public Function GetHashCode1(obj As Entities.ConflittoVaccinazioniEscluse.DatiEsclusioneInConflitto) As Integer Implements IEqualityComparer(Of Entities.ConflittoVaccinazioniEscluse.DatiEsclusioneInConflitto).GetHashCode

            Return obj.CodiceUslVaccinazioneEsclusa.GetHashCode()

        End Function

    End Class

#End Region

#Region " Types "

    <Serializable>
Public Class VaccinazioneDaEscludere
        Public Property Codice As String
        Public Property Descrizione As String
    End Class

    Public Class SalvaVaccinazioneEsclusaCommand
        Inherits SalvaCommandBase

#Region " Public "

        Public Property VaccinazioneEsclusa As VaccinazioneEsclusa

        Public Property VaccinazioneEsclusaOriginale As VaccinazioneEsclusa

        Public Property OverwriteIfUpdateOperation As Boolean = True

#End Region

#Region " Friend "

        Friend Property UpdateVaccinazioneEsclusaCentraleInConflittoIfNeeded As Boolean

        Friend Property VaccinazioneEsclusaCentrale As Entities.VaccinazioneEsclusaCentrale

#End Region

    End Class

    Public Class SalvaVaccinazioneEsclusaResult
        Inherits BizResult

        Public Sub New(success As Boolean, resultMessageEnumerable As IEnumerable(Of ResultMessage))
            MyBase.New(success, resultMessageEnumerable)
            Me.VaccinazioniProgrammateEliminate = New List(Of VaccinazioneProgrammata)()
        End Sub

        Public Property VaccinazioneEsclusaEliminata As VaccinazioneEsclusa
        Public Property VaccinazioniProgrammateEliminate As List(Of VaccinazioneProgrammata)

    End Class

#Region " Centrale "

    Friend Class VaccinazioneEsclusaCentraleInfo

        Public Property VaccinazioneEsclusa As VaccinazioneEsclusa
        Public Property VaccinazioneEsclusaCentrale As VaccinazioneEsclusaCentrale

    End Class

#End Region

#End Region

#Region " Methods "

#Region " Shared "

    Public Class CheckValoreDoseResult

        Public Success As Boolean
        Public Message As String
        Public NumeroDose As Integer?

        Public Sub New()
        End Sub

        Public Sub New(success As Boolean, message As String, numeroDose As Integer?)
            Me.Success = success
            Me.Message = message
            Me.NumeroDose = numeroDose
        End Sub

    End Class

    ''' <summary>
    ''' Restituisce true se la dose è un numero intero positivo.
    ''' </summary>
    ''' <param name="dose"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CheckValoreDose(dose As String, isRequired As Boolean) As CheckValoreDoseResult

        dose = dose.Trim()

        If isRequired AndAlso String.IsNullOrWhiteSpace(dose) Then
            Return New CheckValoreDoseResult(False, "La dose è obbligatoria", Nothing)
        End If

        Dim numeroDose As Integer = 0

        If Not Integer.TryParse(dose, Globalization.NumberStyles.None, Globalization.CultureInfo.InvariantCulture, numeroDose) Then
            Return New CheckValoreDoseResult(False, "La dose deve essere un numero intero positivo", Nothing)
        End If

        If numeroDose <= 0 Then
            Return New CheckValoreDoseResult(False, "La dose deve essere un numero intero positivo", numeroDose)
        End If

        Return New CheckValoreDoseResult(True, String.Empty, numeroDose)

    End Function

#End Region

#Region " Public "

    Public Function AggiornaVaccinazioneEsclusa(vaccinazioneEsclusa As VaccinazioneEsclusa, deleteConvocazioneIfNeeded As Boolean, noteProvenienzaEsclusione As String) As VaccinazioneEsclusa

        If String.IsNullOrEmpty(vaccinazioneEsclusa.CodiceUslInserimento) Then
            vaccinazioneEsclusa.CodiceUslInserimento = ContextInfos.CodiceUsl
        End If

        Dim vaccinazioneEsclusaEliminata As VaccinazioneEsclusa = Nothing
        '--
        Using transactionScope As New System.Transactions.TransactionScope(TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions())
            '--
            Dim vaccinazioneEsclusaEsistente As Entities.VaccinazioneEsclusa = Me.GenericProvider.VaccinazioniEscluse.GetVaccinazioneEsclusaPaziente(vaccinazioneEsclusa.CodicePaziente, vaccinazioneEsclusa.CodiceVaccinazione)
            '--
            If Not vaccinazioneEsclusaEsistente Is Nothing Then

                ' Riporto l'id della esclusione trovata con vex_paz_codice, vex_vac_codice
                vaccinazioneEsclusa.Id = vaccinazioneEsclusaEsistente.Id

                Dim salvaVaccinazioneEsclusaResult As SalvaVaccinazioneEsclusaResult = Me.ModificaVaccinazioneEsclusa(vaccinazioneEsclusa, False)
                vaccinazioneEsclusaEliminata = salvaVaccinazioneEsclusaResult.VaccinazioneEsclusaEliminata
            Else
                InserisciVaccinazioneEsclusa(vaccinazioneEsclusa)
            End If
            '-- 
            Me.EliminaVaccinazioneProgrammataIfNeeded(vaccinazioneEsclusa)
            '--
            If deleteConvocazioneIfNeeded Then
                '--
                Using bizConvocazione As New BizConvocazione(Me.GenericProvider, Me.Settings, Me.ContextInfos, Nothing)

                    Dim command As New Biz.BizConvocazione.EliminaConvocazioneEmptyCommand()
                    command.CodicePaziente = vaccinazioneEsclusa.CodicePaziente
                    command.DataConvocazione = Nothing
                    command.DataEliminazione = DateTime.Now
                    command.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.Esclusione
                    command.NoteEliminazione = "Eliminazione convocazione per esclusione"
                    If Not String.IsNullOrWhiteSpace(noteProvenienzaEsclusione) Then
                        command.NoteEliminazione += String.Format(" ({0})", noteProvenienzaEsclusione)
                    End If
                    command.WriteLog = True

                    bizConvocazione.EliminaConvocazioneEmpty(command)

                End Using
                '--
            End If
            '--
            transactionScope.Complete()
            '--
        End Using
        '--
        Return vaccinazioneEsclusaEliminata
        '--
    End Function

    Public Function SalvaVaccinazioneEsclusa(salvaVaccinazioneEsclusaCommand As SalvaVaccinazioneEsclusaCommand) As SalvaVaccinazioneEsclusaResult
        Return SalvaVaccinazioneEsclusa(salvaVaccinazioneEsclusaCommand, Constants.StatoVaccinazioniEscluseEliminate.Eliminata)
    End Function

    Public Function SalvaVaccinazioneEsclusa(salvaVaccinazioneEsclusaCommand As SalvaVaccinazioneEsclusaCommand, tipoEliminazioneEsclusione As String) As SalvaVaccinazioneEsclusaResult

        Dim vaccinazioniProgrammateEliminate As New List(Of VaccinazioneProgrammata)()
        Dim vaccinazioneEsclusaEliminata As VaccinazioneEsclusa = Nothing

        Select Case salvaVaccinazioneEsclusaCommand.Operation

            Case Biz.BizClass.SalvaCommandOperation.Insert

                Dim inserisciVaccinazioneEsclusaResult As SalvaVaccinazioneEsclusaResult =
                    Me.InserisciVaccinazioneEsclusa(salvaVaccinazioneEsclusaCommand.VaccinazioneEsclusa)

                vaccinazioniProgrammateEliminate.AddRange(inserisciVaccinazioneEsclusaResult.VaccinazioniProgrammateEliminate)

            Case Biz.BizClass.SalvaCommandOperation.Update

                Dim modificaVaccinazioneEsclusaResult As SalvaVaccinazioneEsclusaResult =
                    Me.ModificaVaccinazioneEsclusa(salvaVaccinazioneEsclusaCommand.VaccinazioneEsclusa, salvaVaccinazioneEsclusaCommand.OverwriteIfUpdateOperation)

                vaccinazioniProgrammateEliminate.AddRange(modificaVaccinazioneEsclusaResult.VaccinazioniProgrammateEliminate)

                ' N.B. : in caso di update senza overwrite, l'eliminazione della vecchia esclusa deve essere riportata anche in centrale!
                vaccinazioneEsclusaEliminata = modificaVaccinazioneEsclusaResult.VaccinazioneEsclusaEliminata

            Case Biz.BizClass.SalvaCommandOperation.Delete

                Me.EliminaVaccinazioneEsclusa(salvaVaccinazioneEsclusaCommand.VaccinazioneEsclusa, tipoEliminazioneEsclusione)

        End Select

        Dim salvaVaccinazioneEsclusaResult As New SalvaVaccinazioneEsclusaResult(True, New BizResult.ResultMessage() {})
        salvaVaccinazioneEsclusaResult.VaccinazioniProgrammateEliminate = vaccinazioniProgrammateEliminate
        salvaVaccinazioneEsclusaResult.VaccinazioneEsclusaEliminata = vaccinazioneEsclusaEliminata

        Return salvaVaccinazioneEsclusaResult

    End Function

    ''' <summary>
    ''' Restituisce il numero di vaccinazioni escluse per il paziente specificato
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="isGestioneCentrale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    Public Function CountVaccinazioniEsclusePaziente(codicePaziente As String, isGestioneCentrale As Boolean) As Integer

        If isGestioneCentrale Then Return 0

        Return Me.GenericProvider.VaccinazioniEscluse.CountVaccinazioniEsclusePaziente(codicePaziente)

    End Function
    ''' <summary>
    ''' Data una vaccinazione, il seguente metodo crea una inadempienza data la vaccinazione
    ''' </summary>
    ''' <param name="pippo"></param>
    ''' <returns></returns>
    Public Function CreaInadempienza(pippo As String) As ResultSetPost
        Return GenericProvider.VaccinazioniEscluse.CreaInadempienza()
    End Function

    ''' <summary>
    ''' Restituisce la vaccinazione esclusa specificata per il paziente
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="codiceVaccinazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LoadVaccinazioneEsclusa(codicePaziente As Integer, codiceVaccinazione As String) As Entities.VaccinazioneEsclusa

        Return Me.GenericProvider.VaccinazioniEscluse.GetVaccinazioneEsclusaPaziente(codicePaziente, codiceVaccinazione)

    End Function

    ''' <summary>
    ''' Restituisce l'elenco delle vaccinazioni escluse per il paziente
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LoadVaccinazioniEscluse(codicePaziente As Integer) As List(Of Entities.VaccinazioneEsclusa)

        Return Me.GenericProvider.VaccinazioniEscluse.GetVaccinazioniEsclusePaziente(codicePaziente)

    End Function

    ''' <summary>
    ''' Restituisce l'elenco delle vaccinazioni escluse ed escluse eliminate per il paziente e per vaccinazione, in modo da ricreare uno storico per vaccinazione esclusa
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetVaccinazioniEscluseEliminateByPazienteVaccinazione(codicePaziente As Integer, codiceVaccinazione As String, sortColumn As String, filtraRinnovate As Boolean) As List(Of Entities.VaccinazioneEsclusaDettaglio)

        Return Me.GenericProvider.VaccinazioniEscluse.GetVaccinazioneEsclusaEliminataByPazienteVaccinazione(codicePaziente, codiceVaccinazione, sortColumn, filtraRinnovate)

    End Function

    ''' <summary>
    ''' Carica codice e descrizione delle vaccinazioni
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetVaccinazioniCodiceDescrizione(codiciVaccinazioni As List(Of String)) As List(Of KeyValuePair(Of String, String))

        Return Me.GenericProvider.AnaVaccinazioni.GetCodiceDescrizioneVaccinazioni(codiciVaccinazioni)

    End Function

    ''' <summary>
    ''' Restituisce un datatable con le vaccinazioni escluse del paziente specificato.
    ''' Se il parametro isGestioneCentrale vale true, esegue la ricerca di vaccinazioni e reazioni avverse 
    ''' nella tabella centrale e recupera i dati locali dalle usl di appartenenza.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="isGestioneCentrale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetVaccinazioniEscluse(codicePaziente As String, isGestioneCentrale As Boolean) As DataTable

        ' [Unificazione Ulss]: isGestioneCentrale => NON PIU' PREVISTA
        'If isGestioneCentrale Then
        '    Return Me.GetVaccinazioniEscluseCentralizzate(codicePaziente)
        'End If

        Return GenericProvider.VaccinazioniEscluse.GetDtVaccinazioniEsclusePaziente(codicePaziente)

    End Function

    ''' <summary>
    ''' Restituisce una lista di vaccinazioni da proporre per l'esclusione.
    ''' La lista è composta da tutte le vaccinazioni in anagrafe (non obsolete e che non hanno una sostituta specificata), tranne quelle che sono già state escluse.
    ''' </summary>
    ''' <param name="dtVacEsclusePaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetVaccinazioniDaEscludere(dtVacEsclusePaziente As DataTable) As List(Of VaccinazioneDaEscludere)

        Dim listVaccinazioniDaEscludere As New List(Of VaccinazioneDaEscludere)()

        Dim listAnagVaccinazioni As List(Of KeyValuePair(Of String, String)) = Me.GenericProvider.AnaVaccinazioni.GetCodiceDescrizioneVaccinazioni(False, False)
        If Not listAnagVaccinazioni Is Nothing AndAlso listAnagVaccinazioni.Count > 0 Then

            Dim codiciVaccinazioniGiaEscluse As New List(Of String)()

            Dim dv As New DataView(dtVacEsclusePaziente)
            If dv.Count > 0 Then
                For i As Integer = 0 To dv.Count - 1
                    codiciVaccinazioniGiaEscluse.Add(dv(i)("vex_vac_codice"))
                Next
            End If

            ' Dall'anagrafica, rimuove tutte le vaccinazioni contenute nella lista di quelle già escluse
            listAnagVaccinazioni.RemoveAll(Function(p) codiciVaccinazioniGiaEscluse.Any(Function(cod) cod = p.Key))

            ' Creo la lista di elementi tipizzati da resitutire
            For Each vaccinazione As KeyValuePair(Of String, String) In listAnagVaccinazioni

                Dim vaccinazioneDaEscludere As New VaccinazioneDaEscludere()
                vaccinazioneDaEscludere.Codice = vaccinazione.Key
                vaccinazioneDaEscludere.Descrizione = vaccinazione.Value

                listVaccinazioniDaEscludere.Add(vaccinazioneDaEscludere)
            Next

        End If

        Return listVaccinazioniDaEscludere

    End Function

    ''' <summary>
    ''' Imposta il flag di visibilità, id utente e data di modifica per le vaccinazioni specificate
    ''' </summary>
    ''' <param name="listIdVaccinazioniEscluse"></param>
    ''' <param name="flagVisibilita"></param>
    ''' <returns></returns>
    Public Function UpdateFlagVisibilita(codicePaziente As Long, listIdVaccinazioniEscluse As List(Of Long), flagVisibilita As String) As Integer

        Dim dataModifica As Date = Date.Now

        Dim vacEscluseOriginali As List(Of VaccinazioneEsclusa) = Nothing
        If listIdVaccinazioniEscluse.Count > 0 Then vacEscluseOriginali = GenericProvider.VaccinazioniEscluse.GetVaccinazioniEscluseById(listIdVaccinazioniEscluse)

        Dim countEscluseModificate As Integer = 0
        For Each idVaccinazioneEsclusa As Long In listIdVaccinazioniEscluse
            countEscluseModificate += GenericProvider.VaccinazioniEscluse.UpdateFlagVisibilita(idVaccinazioneEsclusa, flagVisibilita, ContextInfos.IDUtente, dataModifica)
        Next

        If countEscluseModificate > 0 Then

            Dim bizLogOptions As BizLogOptions

            If LogOptions Is Nothing Then
                bizLogOptions = New BizLogOptions(TipiArgomento.VAC_ESCLUSE, False)
            Else
                bizLogOptions = New BizLogOptions(LogOptions.CodiceArgomento, LogOptions.Automatico)
            End If

            Dim testataLog As New Testata(bizLogOptions.CodiceArgomento, Operazione.Modifica, codicePaziente, bizLogOptions.Automatico)

            For Each vac As VaccinazioneEsclusa In vacEscluseOriginali

                Dim recordLog As New DataLogStructure.Record()

                recordLog.Campi.Add(New Campo("Modificato flag visibilita escluse (VEX_ID):", vac.Id.ToString()))

                If String.IsNullOrWhiteSpace(vac.FlagVisibilita) Then
                    recordLog.Campi.Add(New Campo("VEX_FLAG_VISIBILITA", String.Empty, flagVisibilita))
                Else
                    recordLog.Campi.Add(New Campo("VEX_FLAG_VISIBILITA", vac.FlagVisibilita, flagVisibilita))
                End If

                If vac.IdUtenteModifica.HasValue Then
                    recordLog.Campi.Add(New Campo("VEX_UTE_ID_VARIAZIONE", vac.IdUtenteModifica.Value.ToString(), ContextInfos.IDUtente.ToString()))
                Else
                    recordLog.Campi.Add(New Campo("VEX_UTE_ID_VARIAZIONE", String.Empty, ContextInfos.IDUtente.ToString()))
                End If

                If vac.DataModifica.HasValue Then
                    recordLog.Campi.Add(New Campo("VEX_DATA_VARIAZIONE", vac.DataModifica.Value, dataModifica))
                Else
                    recordLog.Campi.Add(New Campo("VEX_DATA_VARIAZIONE", String.Empty, dataModifica))
                End If

                testataLog.Records.Add(recordLog)

            Next

            If testataLog.Records.Count > 0 Then
                    LogBox.WriteData(testataLog)
                End If

            End If

        Return countEscluseModificate

    End Function

    Public Function GetMaxDoseEseguitaVaccinazioneEsclusa(codicePaziente As String, codiceVaccinazione As String) As Integer

        Dim result As Integer = 0
        result = GenericProvider.VaccinazioniEscluse.GetMaxDoseEseguitaVaccinazioneEsclusa(codicePaziente, codiceVaccinazione) + 1
        Return result

    End Function

#Region " Conflitti Escluse "

    ''' <summary>
    ''' Restituisce il numero di vaccinazioni "master" aventi conflitti associati, in base ai filtri impostati.
    ''' </summary>
    ''' <param name="filtriRicercaConflitti"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountConflittiVaccinazioniEscluse(filtriRicercaConflitti As Filters.FiltriRicercaConflittiDatiVaccinali) As Integer

        Return Me.GenericProviderCentrale.VaccinazioneEsclusaCentrale.CountConflittiVaccinazioniEscluseCentrale(filtriRicercaConflitti)

    End Function

    Public Function GetConflittiVaccinazioniEscluse(filtriRicercaConflitti As Filters.FiltriRicercaConflittiDatiVaccinali, pageIndex As Integer?, pageSize As Integer?) As List(Of Entities.ConflittoVaccinazioniEscluse)

        Dim pagingOptions As OnAssistnet.Data.PagingOptions = Nothing

        If Not pageIndex Is Nothing AndAlso Not pageSize Is Nothing Then

            pagingOptions = New OnAssistnet.Data.PagingOptions()

            pagingOptions.PageIndex = pageIndex
            pagingOptions.PageSize = pageSize

        End If

        ' Restituisce le esclusioni "padre" che hanno conflitti. 
        ' Ogni elemento contiene i dati centrali del paziente e una lista di tutte le esclusioni in conflitto (padre compreso). 
        ' Nella lista dei conflitti sono presenti solo i dati centrali di ogni esclusione.
        ' I dati locali (data esclusione, vaccinazione, motivo, ...) devono essere recuperati in base alla usl a cui appartengono.
        Dim listConflittoVaccinazioniEscluse As List(Of Entities.ConflittoVaccinazioniEscluse) =
            Me.GenericProviderCentrale.VaccinazioneEsclusaCentrale.GetConflittiVaccinazioniEscluseCentrale(filtriRicercaConflitti, pagingOptions)

        If listConflittoVaccinazioniEscluse Is Nothing OrElse listConflittoVaccinazioniEscluse.Count = 0 Then Return Nothing

        ' Insieme delle accoppiate Usl-IdEsclusioni
        Dim esclusioniUsl As New Dictionary(Of String, List(Of Int64))

        ' Valorizzazione delle coppie usl-esclusioni
        For Each conflitto As Entities.ConflittoVaccinazioniEscluse In listConflittoVaccinazioniEscluse

            Dim listCodiciUsl As List(Of String) =
                conflitto.VaccinazioniEscluseInConflitto.
                    Distinct(New UslInserimentoConflittoEscluseComparer()).
                    Select(Function(v) v.CodiceUslVaccinazioneEsclusa).ToList()

            For Each codiceUsl As String In listCodiciUsl

                ' Ricerca id esclusioni nell'elenco corrente dei conflitti 
                Dim codiceUslCorrente As String = codiceUsl     ' --> utilizzata come filtro nella Where

                Dim listIdEsclusioni As List(Of Int64) =
                    conflitto.VaccinazioniEscluseInConflitto.
                        Where(Function(v) v.CodiceUslVaccinazioneEsclusa = codiceUslCorrente).
                        Select(Function(v) v.IdVaccinazioneEsclusa).ToList()

                If Not listIdEsclusioni Is Nothing AndAlso listIdEsclusioni.Count > 0 Then

                    If Not esclusioniUsl.ContainsKey(codiceUsl) Then
                        ' Aggiunta dell'accoppiata usl-esclusioni al dictionary
                        esclusioniUsl.Add(codiceUsl, listIdEsclusioni)
                    Else
                        ' Aggiunta delle esclusioni alla lista relativa alla usl
                        esclusioniUsl(codiceUsl) = esclusioniUsl(codiceUsl).Union(listIdEsclusioni).ToList()
                    End If

                End If

            Next

        Next

        Dim enumeratorEsclusioniUsl As IDictionaryEnumerator = esclusioniUsl.GetEnumerator()

        While enumeratorEsclusioniUsl.MoveNext()

            Dim codiceUslCorrente As String = enumeratorEsclusioniUsl.Key.ToString()
            Dim listaEsclusioniInConflittoCorrente As List(Of Int64) = DirectCast(enumeratorEsclusioniUsl.Value, List(Of Int64))

            ' Ricerca dati eseguite per ogni usl
            Dim vaccinazioniEscluseLocali As List(Of VaccinazioneEsclusa) = Nothing

            ' [Unificazione Ulss]: conflitti => solo vecchia versione
            Using genericProviderUsl As DbGenericProvider = GetDBGenericProviderByCodiceUslGestita(codiceUslCorrente)

                vaccinazioniEscluseLocali = genericProviderUsl.VaccinazioniEscluse.GetVaccinazioniEscluseById(listaEsclusioniInConflittoCorrente)

            End Using

            ' Assegnazione valori locali ai dati delle vaccinazioni in conflitto
            If Not vaccinazioniEscluseLocali Is Nothing Then

                For Each vaccinazioneEsclusaLocale As Entities.VaccinazioneEsclusa In vaccinazioniEscluseLocali

                    Dim idEsclusaCorrente As Int64 = vaccinazioneEsclusaLocale.Id

                    For Each conflitto As Entities.ConflittoVaccinazioniEscluse In listConflittoVaccinazioniEscluse

                        Dim datiEsclusioneInConflitto As Entities.ConflittoVaccinazioniEscluse.DatiEsclusioneInConflitto =
                            (From item As Entities.ConflittoVaccinazioniEscluse.DatiEsclusioneInConflitto In conflitto.VaccinazioniEscluseInConflitto
                             Where item.IdVaccinazioneEsclusa = idEsclusaCorrente _
                             And item.CodiceUslVaccinazioneEsclusa = codiceUslCorrente
                             Select item).FirstOrDefault()

                        If Not datiEsclusioneInConflitto Is Nothing Then

                            datiEsclusioneInConflitto.CodiceMotivoEsclusione = vaccinazioneEsclusaLocale.CodiceMotivoEsclusione
                            datiEsclusioneInConflitto.DescrizioneMotivoEsclusione = vaccinazioneEsclusaLocale.DescrizioneMotivoEsclusione
                            datiEsclusioneInConflitto.CodiceVaccinazione = vaccinazioneEsclusaLocale.CodiceVaccinazione
                            datiEsclusioneInConflitto.DataEsclusione = vaccinazioneEsclusaLocale.DataVisita
                            datiEsclusioneInConflitto.DataScadenza = vaccinazioneEsclusaLocale.DataScadenza

                            Exit For

                        End If

                    Next

                Next

            End If

        End While

        Return listConflittoVaccinazioniEscluse

    End Function

#End Region

#End Region

#Region " Private "

    ''' <summary>
    ''' Se l'esclusa deve essere sovrascritta, effettua la modifica della vaccinazione esclusa specificata.
    ''' Altrimenti, effettua l'eliminazione dell'esclusa specificata e ne crea una nuova con gli stessi dati.
    ''' </summary>
    ''' <param name="vaccinazioneEsclusa"></param>
    ''' <param name="overwrite"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ModificaVaccinazioneEsclusa(vaccinazioneEsclusa As VaccinazioneEsclusa, overwrite As Boolean) As SalvaVaccinazioneEsclusaResult
        '--
        Dim vaccinazioneEsclusaEliminata As VaccinazioneEsclusa = Nothing
        '--
        Using transactionScope As New System.Transactions.TransactionScope(TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions())
            '--
            If overwrite Then
                '--
                If Not vaccinazioneEsclusa.DataModifica.HasValue Then
                    vaccinazioneEsclusa.DataModifica = DateTime.Now
                End If
                '--
                If Not vaccinazioneEsclusa.IdUtenteModifica.HasValue Then
                    vaccinazioneEsclusa.IdUtenteModifica = Me.ContextInfos.IDUtente
                End If
                '--
                Me.GenericProvider.VaccinazioniEscluse.ModificaVaccinazioneEsclusa(vaccinazioneEsclusa)
                '--              
                Dim eliminaVaccinazioneProgrammataResult As BizVaccinazioneProg.EliminaVaccinazioniProgrammateResult =
                    Me.EliminaVaccinazioneProgrammataIfNeeded(vaccinazioneEsclusa)
                '--
            Else
                '--
                vaccinazioneEsclusaEliminata = vaccinazioneEsclusa.Clone()
                '--
                vaccinazioneEsclusaEliminata.DataEliminazione = Nothing
                vaccinazioneEsclusaEliminata.IdUtenteEliminazione = Nothing
                '--
                Me.EliminaVaccinazioneEsclusa(vaccinazioneEsclusaEliminata, Constants.StatoVaccinazioniEscluseEliminate.Eliminata)
                '--
                vaccinazioneEsclusa.Id = -1
                vaccinazioneEsclusa.DataRegistrazione = DateTime.MinValue
                vaccinazioneEsclusa.IdUtenteRegistrazione = -1
                '--
                Me.InserisciVaccinazioneEsclusa(vaccinazioneEsclusa)
                '--
            End If
            '--   
            transactionScope.Complete()
            '--
        End Using
        '--
        Dim salvaVaccinazioneEsclusaResult As New SalvaVaccinazioneEsclusaResult(True, {})
        salvaVaccinazioneEsclusaResult.VaccinazioneEsclusaEliminata = vaccinazioneEsclusaEliminata
        '--
        Return salvaVaccinazioneEsclusaResult
        '--
    End Function

    Private Function InserisciVaccinazioneEsclusa(vaccinazioneEsclusa As VaccinazioneEsclusa) As SalvaVaccinazioneEsclusaResult

        If String.IsNullOrEmpty(vaccinazioneEsclusa.CodiceUslInserimento) Then
            vaccinazioneEsclusa.CodiceUslInserimento = GetCodiceUslByIdApplicazione(ContextInfos.IDApplicazione)
        End If

        If vaccinazioneEsclusa.DataRegistrazione = DateTime.MinValue Then
            vaccinazioneEsclusa.DataRegistrazione = DateTime.Now
        End If

        If vaccinazioneEsclusa.IdUtenteRegistrazione <= 0 Then
            vaccinazioneEsclusa.IdUtenteRegistrazione = ContextInfos.IDUtente
        End If

        GenericProvider.VaccinazioniEscluse.InserisciVaccinazioneEsclusa(vaccinazioneEsclusa)

        Dim eliminaVaccinazioniProgrammateResult As BizVaccinazioneProg.EliminaVaccinazioniProgrammateResult =
            EliminaVaccinazioneProgrammataIfNeeded(vaccinazioneEsclusa)

        Dim salvaVaccinazioneEsclusaResult As New SalvaVaccinazioneEsclusaResult(True, {})
        salvaVaccinazioneEsclusaResult.VaccinazioniProgrammateEliminate = eliminaVaccinazioniProgrammateResult.VaccinazioniProgrammateEliminate

        Return salvaVaccinazioneEsclusaResult

    End Function

    Public Function EliminaVaccinazioneEsclusa(vaccinazioneEsclusaEliminata As VaccinazioneEsclusa, tipoEliminazioneEsclusione As String) As Boolean

        GenericProvider.VaccinazioniEscluse.DeleteVaccinazioneEsclusa(vaccinazioneEsclusaEliminata.Id)

        InserisciVaccinazioneEsclusaEliminata(vaccinazioneEsclusaEliminata, tipoEliminazioneEsclusione)
        Return True
    End Function

    'Private Function GetVaccinazioniEscluseByUslInserimentoEsclusaAndId(codiceUsl As String, listIdVaccinazioniEscluseUsl As List(Of Long)) As DataTable

    '    If listIdVaccinazioniEscluseUsl Is Nothing Then Return Nothing

    '    Using genericProviderUsl As DbGenericProvider = GetDBGenericProviderByCodiceUslGestita(codiceUsl)

    '        Return genericProviderUsl.VaccinazioniEscluse.GetDtVaccinazioniEscluseById(listIdVaccinazioniEscluseUsl)

    '    End Using

    'End Function

    Private Function EliminaVaccinazioneProgrammataIfNeeded(vaccinazioneEsclusa As VaccinazioneEsclusa) As BizVaccinazioneProg.EliminaVaccinazioniProgrammateResult
        '--
        Dim dataConvocazione As DateTime? = Me.GenericProvider.VaccinazioneProg.GetDataByVaccinazione(vaccinazioneEsclusa.CodicePaziente, vaccinazioneEsclusa.CodiceVaccinazione)
        '--
        If dataConvocazione.HasValue Then
            '--
            If vaccinazioneEsclusa.DataScadenza = DateTime.MinValue OrElse
            vaccinazioneEsclusa.DataScadenza > dataConvocazione Then
                '--
                Using bizVaccinazioneProg As New BizVaccinazioneProg(Me.GenericProvider, Me.Settings, Me.ContextInfos, Me.LogOptions)
                    '--
                    Dim eliminaVaccinazioniProgrammateCommand As New BizVaccinazioneProg.EliminaVaccinazioniProgrammateCommand
                    eliminaVaccinazioniProgrammateCommand.CodicePaziente = vaccinazioneEsclusa.CodicePaziente
                    eliminaVaccinazioniProgrammateCommand.CodiceVaccinazioni = {vaccinazioneEsclusa.CodiceVaccinazione}.AsEnumerable()
                    eliminaVaccinazioniProgrammateCommand.DataConvocazione = dataConvocazione
                    '--
                    Return bizVaccinazioneProg.EliminaVaccinazioniProgrammate(eliminaVaccinazioniProgrammateCommand)
                    '--
                End Using
                '--
            End If
            '--
        End If
        '--    
        Return New BizVaccinazioneProg.EliminaVaccinazioniProgrammateResult
        '--
    End Function

    Private Sub InserisciVaccinazioneEsclusaEliminata(vaccinazioneEsclusaEliminata As VaccinazioneEsclusa, tipoEliminazione As String)
        '--
        If Not vaccinazioneEsclusaEliminata.IdUtenteEliminazione.HasValue Then
            vaccinazioneEsclusaEliminata.IdUtenteEliminazione = Me.ContextInfos.IDUtente
        End If
        '--
        If Not vaccinazioneEsclusaEliminata.DataEliminazione.HasValue Then
            vaccinazioneEsclusaEliminata.DataEliminazione = DateTime.Now
        End If
        '--       
        Me.GenericProvider.VaccinazioniEscluse.InserisciVaccinazioneEsclusaEliminata(vaccinazioneEsclusaEliminata, tipoEliminazione)
        '--
    End Sub

#End Region

#Region " Centrale "

#Region " Public "

    'Private Function GetVaccinazioniEscluseCentralizzate(codicePazienteCentrale As String) As DataTable

    '    ' Elenco escluse del paziente presenti in centrale con visibilità concessa
    '    Dim listVaccinazioniEscluseCentrale As IEnumerable(Of Entities.VaccinazioneEsclusaCentrale) =
    '        Me.GenericProviderCentrale.VaccinazioneEsclusaCentrale.GetVaccinazioneEsclusaCentraleEnumerable(codicePazienteCentrale,
    '                                                                                                        Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente)

    '    ' Se nessun risultato: deve continuare per creare la struttura del datatable (che verrà restituito vuoto, ma con le colonne impostate)
    '    If listVaccinazioniEscluseCentrale Is Nothing Then
    '        listVaccinazioniEscluseCentrale = (New List(Of Entities.VaccinazioneEsclusaCentrale)()).AsEnumerable()
    '    End If

    '    Dim dtEscluseCentralizzate As New DataTable()

    '    ' Elenco usl inserimento delle vaccinazioni escluse
    '    Dim codiciUslInserimentoEscluse As List(Of String) =
    '        listVaccinazioniEscluseCentrale.Distinct(New UslInserimentoEsclusaComparer()).Select(Function(v) v.CodiceUslVaccinazioneEsclusa).ToList()

    '    ' Per ogni usl in cui sono presenti escluse del paziente, recupero i dati necessari in base a paz_codice locale e vex_id
    '    For Each codiceUslInserimentoEsclusa As String In codiciUslInserimentoEscluse

    '        Dim codiceUsl As String = codiceUslInserimentoEsclusa

    '        ' Elenco id delle escluse nella usl corrente
    '        Dim listEscluseUsl As IEnumerable(Of Entities.VaccinazioneEsclusaCentrale) =
    '            listVaccinazioniEscluseCentrale.Where(Function(v) v.CodiceUslVaccinazioneEsclusa = codiceUsl)

    '        If Not listEscluseUsl Is Nothing AndAlso listEscluseUsl.Count > 0 Then

    '            ' Query sulla singola usl per reperimento dati vaccinazioni escluse del paziente (con paz_codice e i vex_id presenti nella lista)
    '            Dim dtVacc As DataTable =
    '                Me.GetVaccinazioniEscluseByUslInserimentoEsclusaAndId(codiceUsl, listEscluseUsl.Select(Function(v) v.IdVaccinazioneEsclusa).ToList())

    '            ' Aggiunta risultati al datatable delle vaccinazioni da restituire
    '            If Not dtVacc Is Nothing Then

    '                If dtEscluseCentralizzate.Columns.Count = 0 Then

    '                    dtEscluseCentralizzate = dtVacc.Copy()

    '                Else

    '                    For Each row As DataRow In dtVacc.Rows

    '                        Dim newRow As DataRow = dtEscluseCentralizzate.NewRow()
    '                        newRow.ItemArray = row.ItemArray
    '                        dtEscluseCentralizzate.Rows.Add(newRow)

    '                    Next

    '                End If

    '            End If

    '        End If

    '    Next

    '    ' Se non è stata creata la struttura delle colonne, eseguo una query "fittizia" per averla comunque.
    '    If dtEscluseCentralizzate.Columns.Count = 0 Then
    '        Dim listId As New List(Of Long)()
    '        listId.Add(0)
    '        dtEscluseCentralizzate = Me.GenericProvider.VaccinazioniEscluse.GetDtVaccinazioniEscluseById(listId)
    '    End If

    '    Return dtEscluseCentralizzate

    'End Function

#End Region

#Region " Friend "

    Friend Function AggiornaVaccinazioneEsclusaCentrale(codicePazienteCentrale As String, vaccinazioneEsclusa As VaccinazioneEsclusa, tipoVaccinazioneEsclusaCentrale As String,
                                                        isMerge As Boolean, isRisoluzioneConflitto As Boolean) As VaccinazioneEsclusaCentrale

        ' TODO [Unificazione Ulss]: se questo metodo può essere chiamato dalla usl nuova, la Usl Gestita corrente è nulla => scoppia tutto!
        Dim vaccinazioneEsclusaCentrale As VaccinazioneEsclusaCentrale =
            Me.GenericProviderCentrale.VaccinazioneEsclusaCentrale.GetVaccinazioneEsclusaCentraleByIdLocale(vaccinazioneEsclusa.Id, Me.UslGestitaCorrente.Codice)

        Dim existsVaccinazioneEsclusaCentrale As Boolean = Not vaccinazioneEsclusaCentrale Is Nothing

        If Not existsVaccinazioneEsclusaCentrale Then

            vaccinazioneEsclusaCentrale = New VaccinazioneEsclusaCentrale()
            vaccinazioneEsclusaCentrale.IdVaccinazioneEsclusa = vaccinazioneEsclusa.Id
            vaccinazioneEsclusaCentrale.CodicePaziente = vaccinazioneEsclusa.CodicePaziente
            vaccinazioneEsclusaCentrale.CodicePazienteCentrale = codicePazienteCentrale
            vaccinazioneEsclusaCentrale.CodiceUslVaccinazioneEsclusa = vaccinazioneEsclusa.CodiceUslInserimento
            vaccinazioneEsclusaCentrale.DataInserimentoVaccinazioneEsclusa = vaccinazioneEsclusa.DataRegistrazione
            vaccinazioneEsclusaCentrale.IdUtenteInserimentoVaccinazioneEsclusa = vaccinazioneEsclusa.IdUtenteRegistrazione

        Else

            'VISIBILITA (REVOCA)
            Dim isRevocaVisibilitaCentrale As Boolean = False

            Dim isVisibilitaCentrale As Boolean = Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(vaccinazioneEsclusa.FlagVisibilita)

            If Not String.IsNullOrEmpty(vaccinazioneEsclusaCentrale.FlagVisibilitaCentrale) Then

                Dim wasVisibilitaCentrale As Boolean = Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(vaccinazioneEsclusaCentrale.FlagVisibilitaCentrale)

                If isVisibilitaCentrale AndAlso Not wasVisibilitaCentrale Then

                    vaccinazioneEsclusaCentrale.DataRevocaVisibilita = Nothing

                ElseIf Not isVisibilitaCentrale AndAlso wasVisibilitaCentrale Then

                    vaccinazioneEsclusaCentrale.DataRevocaVisibilita = DateTime.Now

                    isRevocaVisibilitaCentrale = True

                End If

            End If

            'CONFLITTI
            Dim idConflittoRimanente As Int64? = Nothing

            If isRisoluzioneConflitto Then

                'TODO [ DATI VAC CENTRALE ]: IdUtenteRisoluzioneConflitto = IdUtenteUltimaOperazione ?!?
                vaccinazioneEsclusaCentrale.IdUtenteRisoluzioneConflitto = Me.ContextInfos.IDUtente
                vaccinazioneEsclusaCentrale.DataRisoluzioneConflitto = DateTime.Now

            ElseIf (isRevocaVisibilitaCentrale OrElse tipoVaccinazioneEsclusaCentrale = Constants.TipoVaccinazioneEsclusaCentrale.Eliminata) AndAlso
                vaccinazioneEsclusaCentrale.IdConflitto.HasValue AndAlso Not vaccinazioneEsclusaCentrale.DataRisoluzioneConflitto.HasValue Then

                If isRevocaVisibilitaCentrale OrElse
                    (Me.GenericProviderCentrale.VaccinazioneEsclusaCentrale.CountVaccinazioneEsclusaCentraleInConflittoByIdConflitto(
                        vaccinazioneEsclusaCentrale.IdConflitto) = 2) Then

                    idConflittoRimanente = vaccinazioneEsclusaCentrale.IdConflitto

                End If

                vaccinazioneEsclusaCentrale.IdConflitto = Nothing

            End If

            If idConflittoRimanente.HasValue Then

                Me.GenericProviderCentrale.VaccinazioneEsclusaCentrale.UpdateIdConflittoVaccinazioneEsclusaCentraleByIdConflitto(
                    idConflittoRimanente.Value, Nothing)

            End If

        End If

        Select Case tipoVaccinazioneEsclusaCentrale

            Case Constants.TipoVaccinazioneEsclusaCentrale.Nessuno

                vaccinazioneEsclusaCentrale.IdUtenteModificaVaccinazioneEsclusa = vaccinazioneEsclusa.IdUtenteModifica
                vaccinazioneEsclusaCentrale.DataModificaVaccinazioneEsclusa = vaccinazioneEsclusa.DataModifica

                If isMerge Then
                    vaccinazioneEsclusaCentrale.MergeInfoCentrale.CodicePazienteCentraleAlias = vaccinazioneEsclusaCentrale.CodicePazienteCentrale
                End If

            Case Constants.TipoVaccinazioneEsclusaCentrale.Eliminata

                vaccinazioneEsclusaCentrale.IdUtenteEliminazioneVaccinazioneEsclusa = vaccinazioneEsclusa.IdUtenteEliminazione
                vaccinazioneEsclusaCentrale.DataEliminazioneVaccinazioneEsclusa = vaccinazioneEsclusa.DataEliminazione

                If isMerge Then
                    'TODO [ DATI VAC CENTRALE ]: IdUtenteRisoluzioneConflitto = IdUtenteUltimaOperazione ?!?
                    vaccinazioneEsclusaCentrale.MergeInfoCentrale.IdUtenteAlias = Me.ContextInfos.IDUtente
                    vaccinazioneEsclusaCentrale.MergeInfoCentrale.DataAlias = DateTime.Now
                End If

            Case Else
                Throw New NotImplementedException(String.Format("Tipo Vaccinazione Esclusa Centrale non implementata: {0}", tipoVaccinazioneEsclusaCentrale))

        End Select

        vaccinazioneEsclusaCentrale.FlagVisibilitaCentrale = vaccinazioneEsclusa.FlagVisibilita

        vaccinazioneEsclusaCentrale.TipoVaccinazioneEsclusaCentrale = tipoVaccinazioneEsclusaCentrale

        vaccinazioneEsclusaCentrale.IdUtenteUltimaOperazione = Me.ContextInfos.IDUtente

        If existsVaccinazioneEsclusaCentrale Then
            Me.GenericProviderCentrale.VaccinazioneEsclusaCentrale.UpdateVaccinazioneEsclusaCentrale(vaccinazioneEsclusaCentrale)
        Else
            Me.GenericProviderCentrale.VaccinazioneEsclusaCentrale.InsertVaccinazioneEsclusaCentrale(vaccinazioneEsclusaCentrale)
        End If

        ' TODO [Unificazione Ulss]: se questo metodo può essere chiamato dalla usl nuova, la Usl Gestita corrente è nulla => scoppia tutto!
        Dim vaccinazioneEsclusaCentraleDistributa As VaccinazioneEsclusaCentrale.VaccinazioneEsclusaDistribuita =
            Me.GenericProviderCentrale.VaccinazioneEsclusaCentrale.GetVaccinazioneEsclusaCentraleDistribuitaByIdCentrale(vaccinazioneEsclusaCentrale.Id, Me.UslGestitaCorrente.Codice)

        Me.AggiornaVaccinazioneEsclusaCentraleDistribuita(vaccinazioneEsclusa, vaccinazioneEsclusaCentrale, vaccinazioneEsclusaCentraleDistributa)


        Return vaccinazioneEsclusaCentrale

    End Function

    Friend Sub AcquisisciVaccinazioneEsclusaCentrale(codicePazienteDestinazione As Int64,
                                                     vaccinazioneEsclusaAcquisizioneCentraleInfos As VaccinazioneEsclusaCentraleInfo(),
                                                     tipoVaccinazioneEsclusaCentrale As String)

        ' [Unificazione Ulss]: viene richiamato solo per le usl gestite => OK

        Dim vaccinazioneProgrammataEliminataList As New List(Of VaccinazioneProgrammata)()
        Dim vaccinazioneEsclusaAddedList As New List(Of VaccinazioneEsclusa)()
        Dim vaccinazioneEsclusaModifiedList As New List(Of VaccinazioneEsclusa)()

        Dim codiceMotiviEsclusioneCentralizzati As String() = Me.GenericProvider.MotiviEsclusione.GetCodiciMotiviCentralizzati().ToArray()

        For Each vaccinazioneEsclusaAcquisizioneCentraleInfo As VaccinazioneEsclusaCentraleInfo In vaccinazioneEsclusaAcquisizioneCentraleInfos

            Dim vaccinazioneEsclusaCentraleDistributaDestinazione As VaccinazioneEsclusaCentrale.VaccinazioneEsclusaDistribuita =
                GenericProviderCentrale.VaccinazioneEsclusaCentrale.GetVaccinazioneEsclusaCentraleDistribuitaByIdCentrale(
                    vaccinazioneEsclusaAcquisizioneCentraleInfo.VaccinazioneEsclusaCentrale.Id, UslGestitaCorrente.Codice)

            Dim vaccinazioneEsclusaDestinazione As VaccinazioneEsclusa = Nothing

            Dim operazioneLogDatiVaccinaliCentrali As Log.DataLogStructure.Operazione? = Nothing
            Dim statoLogDatiVaccinaliCentrali As Enumerators.StatoLogDatiVaccinaliCentrali? = Nothing

            Dim isVisibilitaCentrale As Boolean = Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(vaccinazioneEsclusaAcquisizioneCentraleInfo.VaccinazioneEsclusa.FlagVisibilita)

            If tipoVaccinazioneEsclusaCentrale <> Constants.TipoVaccinazioneEsclusaCentrale.Eliminata Then

                If isVisibilitaCentrale OrElse Not vaccinazioneEsclusaCentraleDistributaDestinazione Is Nothing Then

                    vaccinazioneEsclusaDestinazione = Me.CreateVaccinazioneEsclusaDestinazioneByOrigine(vaccinazioneEsclusaAcquisizioneCentraleInfo.VaccinazioneEsclusa,
                                                                                                        vaccinazioneEsclusaAcquisizioneCentraleInfo.VaccinazioneEsclusaCentrale,
                                                                                                        codicePazienteDestinazione)

                    If vaccinazioneEsclusaCentraleDistributaDestinazione Is Nothing Then

                        Dim vaccinazioneEsclusaInConflitto As Entities.VaccinazioneEsclusa = Me.GenericProvider.VaccinazioniEscluse.GetVaccinazioneEsclusaPaziente(
                                codicePazienteDestinazione, vaccinazioneEsclusaAcquisizioneCentraleInfo.VaccinazioneEsclusa.CodiceVaccinazione)

                        If Not vaccinazioneEsclusaInConflitto Is Nothing Then

                            vaccinazioneEsclusaInConflitto.IdUtenteEliminazione = vaccinazioneEsclusaAcquisizioneCentraleInfo.VaccinazioneEsclusaCentrale.IdUtenteUltimaOperazione

                            Dim salvaVaccinazioneEsclusaInConflittoCommand As New BizVaccinazioniEscluse.SalvaVaccinazioneEsclusaCommand()

                            salvaVaccinazioneEsclusaInConflittoCommand.VaccinazioneEsclusa = vaccinazioneEsclusaInConflitto
                            salvaVaccinazioneEsclusaInConflittoCommand.Operation = Biz.BizClass.SalvaCommandOperation.Delete

                            Me.SalvaVaccinazioneEsclusa(salvaVaccinazioneEsclusaInConflittoCommand)

                            If codiceMotiviEsclusioneCentralizzati.Contains(salvaVaccinazioneEsclusaInConflittoCommand.VaccinazioneEsclusa.CodiceMotivoEsclusione) Then

                                Me.AggiornaVaccinazioneEsclusaCentrale(Me.GenericProvider.Paziente.GetCodiceAusiliario(codicePazienteDestinazione),
                                                                       vaccinazioneEsclusaInConflitto, Constants.TipoVaccinazioneEsclusaCentrale.Eliminata, Nothing, False)

                                Me.InsertLogAcquisizioneVaccinazioneEsclusaCentraleDistribuita(vaccinazioneEsclusaInConflitto,
                                    vaccinazioneEsclusaAcquisizioneCentraleInfo.VaccinazioneEsclusaCentrale.IdUtenteUltimaOperazione,
                                    vaccinazioneEsclusaAcquisizioneCentraleInfo.VaccinazioneEsclusaCentrale.CodiceUslVaccinazioneEsclusa,
                                    Constants.TipoVaccinazioneEsclusaCentrale.Eliminata, Log.DataLogStructure.Operazione.Eliminazione,
                                    Enumerators.StatoLogDatiVaccinaliCentrali.Success)

                            End If

                        End If

                        vaccinazioneEsclusaDestinazione.Id = -1
                        vaccinazioneEsclusaDestinazione.DataRegistrazione = Nothing

                        Dim salvaVaccinazioneEsclusaCommand As New BizVaccinazioniEscluse.SalvaVaccinazioneEsclusaCommand()

                        salvaVaccinazioneEsclusaCommand.VaccinazioneEsclusa = vaccinazioneEsclusaDestinazione
                        salvaVaccinazioneEsclusaCommand.Operation = Biz.BizClass.SalvaCommandOperation.Insert

                        Dim salvaVaccinazioneEsclusaResult As SalvaVaccinazioneEsclusaResult = Me.SalvaVaccinazioneEsclusa(salvaVaccinazioneEsclusaCommand)

                        vaccinazioneProgrammataEliminataList.AddRange(salvaVaccinazioneEsclusaResult.VaccinazioniProgrammateEliminate)

                        operazioneLogDatiVaccinaliCentrali = Log.DataLogStructure.Operazione.Inserimento

                        If Not vaccinazioneEsclusaInConflitto Is Nothing Then
                            statoLogDatiVaccinaliCentrali = Enumerators.StatoLogDatiVaccinaliCentrali.Warning
                        Else
                            statoLogDatiVaccinaliCentrali = Enumerators.StatoLogDatiVaccinaliCentrali.Success
                        End If

                        vaccinazioneEsclusaAddedList.Add(vaccinazioneEsclusaDestinazione)

                    Else

                        vaccinazioneEsclusaDestinazione.Id = vaccinazioneEsclusaCentraleDistributaDestinazione.IdVaccinazioneEsclusa
                        vaccinazioneEsclusaDestinazione.DataModifica = Nothing

                        Dim salvaVaccinazioneEsclusaCommand As New BizVaccinazioniEscluse.SalvaVaccinazioneEsclusaCommand()
                        salvaVaccinazioneEsclusaCommand.VaccinazioneEsclusa = vaccinazioneEsclusaDestinazione

                        salvaVaccinazioneEsclusaCommand.VaccinazioneEsclusaOriginale =
                            Me.GenericProvider.VaccinazioniEscluse.GetVaccinazioneEsclusaById(vaccinazioneEsclusaDestinazione.Id)

                        salvaVaccinazioneEsclusaCommand.Operation = Biz.BizClass.SalvaCommandOperation.Update

                        Dim salvaVaccinazioneEsclusaResult As SalvaVaccinazioneEsclusaResult = Me.SalvaVaccinazioneEsclusa(salvaVaccinazioneEsclusaCommand)

                        vaccinazioneProgrammataEliminataList.AddRange(salvaVaccinazioneEsclusaResult.VaccinazioniProgrammateEliminate)

                        operazioneLogDatiVaccinaliCentrali = Log.DataLogStructure.Operazione.Modifica
                        statoLogDatiVaccinaliCentrali = Enumerators.StatoLogDatiVaccinaliCentrali.Success

                        vaccinazioneEsclusaModifiedList.Add(vaccinazioneEsclusaDestinazione)

                    End If

                End If

            Else

                If Not vaccinazioneEsclusaCentraleDistributaDestinazione Is Nothing Then

                    vaccinazioneEsclusaDestinazione =
                       Me.GenericProvider.VaccinazioniEscluse.GetVaccinazioneEsclusaByIdIfExists(vaccinazioneEsclusaCentraleDistributaDestinazione.IdVaccinazioneEsclusa)

                    If Not vaccinazioneEsclusaDestinazione Is Nothing Then

                        vaccinazioneEsclusaDestinazione.IdUtenteEliminazione = vaccinazioneEsclusaAcquisizioneCentraleInfo.VaccinazioneEsclusa.IdUtenteEliminazione

                        Dim salvaVaccinazioneEsclusaCommand As New BizVaccinazioniEscluse.SalvaVaccinazioneEsclusaCommand
                        salvaVaccinazioneEsclusaCommand.VaccinazioneEsclusa = vaccinazioneEsclusaDestinazione
                        salvaVaccinazioneEsclusaCommand.Operation = Biz.BizClass.SalvaCommandOperation.Delete

                        Me.SalvaVaccinazioneEsclusa(salvaVaccinazioneEsclusaCommand)

                        operazioneLogDatiVaccinaliCentrali = Log.DataLogStructure.Operazione.Eliminazione
                        statoLogDatiVaccinaliCentrali = Enumerators.StatoLogDatiVaccinaliCentrali.Success

                    End If

                End If

            End If

            If operazioneLogDatiVaccinaliCentrali.HasValue Then

                If statoLogDatiVaccinaliCentrali <> Enumerators.StatoLogDatiVaccinaliCentrali.Error Then
                    Me.AggiornaVaccinazioneEsclusaCentraleDistribuita(vaccinazioneEsclusaDestinazione,
                        vaccinazioneEsclusaAcquisizioneCentraleInfo.VaccinazioneEsclusaCentrale, vaccinazioneEsclusaCentraleDistributaDestinazione)
                End If

                Me.InsertLogAcquisizioneVaccinazioneEsclusaCentraleDistribuita(vaccinazioneEsclusaDestinazione,
                                                                               vaccinazioneEsclusaAcquisizioneCentraleInfo.VaccinazioneEsclusaCentrale,
                                                                               operazioneLogDatiVaccinaliCentrali,
                                                                               statoLogDatiVaccinaliCentrali)

            End If

        Next

        If vaccinazioneProgrammataEliminataList.Count > 0 Then

            'LOG ELIMINAZIONE PROGRAMMATE
            For Each vac As VaccinazioneProgrammata In vaccinazioneProgrammataEliminataList

                Dim codiceVaccinazione As String = vac.CodiceVaccinazione
                Dim vaccinazioneEsclusa As VaccinazioneEsclusa = vaccinazioneEsclusaAddedList.Union(vaccinazioneEsclusaModifiedList).
                    First(Function(vex) vex.CodiceVaccinazione = codiceVaccinazione)

                Dim vaccinazioneEsclusaCentrale As VaccinazioneEsclusaCentrale =
                    GenericProviderCentrale.VaccinazioneEsclusaCentrale.GetVaccinazioneEsclusaCentraleByIdLocale(
                        vaccinazioneEsclusa.Id, UslGestitaCorrente.Codice)

                Dim logDatiVaccinali As New LogDatiVaccinali()
                logDatiVaccinali.Paziente.Paz_Codice = codicePazienteDestinazione
                logDatiVaccinali.Operazione = Log.DataLogStructure.Operazione.Eliminazione
                logDatiVaccinali.Argomento.Codice = Log.DataLogStructure.TipiArgomento.VAC_PROGRAMMATE
                logDatiVaccinali.Stato = Enumerators.StatoLogDatiVaccinaliCentrali.Success
                logDatiVaccinali.Usl.Codice = vaccinazioneEsclusaCentrale.CodiceUslVaccinazioneEsclusa
                logDatiVaccinali.Utente.Id = vaccinazioneEsclusaCentrale.IdUtenteUltimaOperazione
                logDatiVaccinali.DataOperazione = DateTime.Now
                logDatiVaccinali.Note = String.Format("{0}({1}) - {2}", vac.CodiceVaccinazione, vac.NumeroRichiamo, vac.DataConvocazione.ToString("dd/MM/yyyy"))

                Me.GenericProvider.Log.InsertLogDatiVaccinali(logDatiVaccinali)

            Next

            ' Cancellazione convocazioni senza vaccinazioni-bilanci
            Using bizConvocazione As New BizConvocazione(GenericProvider, Settings, CreateBizContextInfosByCodiceUslGestita(UslGestitaCorrente.Codice), Nothing)

                Dim command As New Biz.BizConvocazione.EliminaConvocazioneEmptyCommand()
                command.CodicePaziente = codicePazienteDestinazione
                command.DataConvocazione = Nothing
                command.DataEliminazione = DateTime.Now
                command.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.Esclusione
                command.NoteEliminazione = "Eliminazione convocazione per acquisizione vaccinazioni escluse da centrale"
                command.WriteLog = True

                bizConvocazione.EliminaConvocazioneEmpty(command)

            End Using

        End If

    End Sub

    Friend Function AggiornaVisibilitaVaccinazioneEsclusa(flagVisibilitaDatiVaccinaliCentrale As String,
                                                          vaccinazioneEsclusa As VaccinazioneEsclusa,
                                                          vaccinazioneEsclusaCentrale As VaccinazioneEsclusaCentrale) As BizResult

        Dim salvaVaccinazioneEsclusaBizResult As BizResult = Nothing

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions)

            vaccinazioneEsclusa.FlagVisibilita = flagVisibilitaDatiVaccinaliCentrale

            Dim salvaVaccinazioneEsclusaCommand As New SalvaVaccinazioneEsclusaCommand()

            salvaVaccinazioneEsclusaCommand.Operation = SalvaCommandOperation.Update
            salvaVaccinazioneEsclusaCommand.VaccinazioneEsclusa = vaccinazioneEsclusa

            salvaVaccinazioneEsclusaBizResult = Me.SalvaVaccinazioneEsclusa(salvaVaccinazioneEsclusaCommand)

            '-- LOG UPDATE FLAG VISIBILITA VACCINAZIONE ESCLUSA
            Me.InsertLogAcquisizioneVaccinazioneEsclusaCentraleDistribuita(vaccinazioneEsclusa,
                                                                           vaccinazioneEsclusaCentrale,
                                                                           Log.DataLogStructure.Operazione.Modifica,
                                                                           IIf(salvaVaccinazioneEsclusaBizResult.Success, Enumerators.StatoLogDatiVaccinaliCentrali.Success, Enumerators.StatoLogDatiVaccinaliCentrali.Error)) ',True)

            transactionScope.Complete()

        End Using

        Return salvaVaccinazioneEsclusaBizResult

    End Function

    Friend Sub InsertLogAcquisizioneVaccinazioneEsclusaCentraleDistribuita(vaccinazioneEsclusa As VaccinazioneEsclusa, vaccinazioneEsclusaCentrale As VaccinazioneEsclusaCentrale, operazione As Log.DataLogStructure.Operazione, statoLogDatiVaccinaliCentrali As Enumerators.StatoLogDatiVaccinaliCentrali)   ' , isVisibilitaModificata As Boolean

        Me.InsertLogAcquisizioneVaccinazioneEsclusaCentraleDistribuita(vaccinazioneEsclusa,
                                                                       vaccinazioneEsclusaCentrale.IdUtenteUltimaOperazione,
                                                                       vaccinazioneEsclusaCentrale.CodiceUslVaccinazioneEsclusa,
                                                                       vaccinazioneEsclusaCentrale.TipoVaccinazioneEsclusaCentrale,
                                                                       operazione,
                                                                       statoLogDatiVaccinaliCentrali)

    End Sub

    Friend Sub InsertLogAcquisizioneVaccinazioneEsclusaCentraleDistribuita(vaccinazioneEsclusa As VaccinazioneEsclusa, idUtente As Int64, codiceUsl As String,
                                                                           tipoVaccinazioneEsclusaCentrale As String, operazione As Log.DataLogStructure.Operazione,
                                                                           statoLogDatiVaccinaliCentrali As Enumerators.StatoLogDatiVaccinaliCentrali)

        Dim noteStringBuilder As New Text.StringBuilder()

        noteStringBuilder.AppendFormat("{0} - {1}", vaccinazioneEsclusa.CodiceVaccinazione, vaccinazioneEsclusa.CodiceMotivoEsclusione)

        Dim logDatiVaccinali As New LogDatiVaccinali()

        logDatiVaccinali.Paziente.Paz_Codice = vaccinazioneEsclusa.CodicePaziente
        logDatiVaccinali.Operazione = operazione
        logDatiVaccinali.Argomento.Codice = Log.DataLogStructure.TipiArgomento.VAC_ESCLUSE
        logDatiVaccinali.Stato = statoLogDatiVaccinaliCentrali
        logDatiVaccinali.Usl.Codice = codiceUsl
        logDatiVaccinali.Utente.Id = idUtente
        logDatiVaccinali.DataOperazione = DateTime.Now
        logDatiVaccinali.Note = noteStringBuilder.ToString()

        Me.GenericProvider.Log.InsertLogDatiVaccinali(logDatiVaccinali)

    End Sub

    ''' <summary>
    ''' Restituisce un dictionary contenente id locale della vaccinazione esclusa, codice della usl di inserimento 
    ''' ed elenco delle usl in cui tale vaccinazione è stata distribuita (esclusa la usl di inserimento).
    ''' </summary>
    ''' <param name="idVaccinazioniEscluseLocali"></param>
    ''' <param name="codiceUslInserimento"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Function GetCodiciUslDistribuiteVaccinazioniEscluse(idVaccinazioniEscluseLocali As List(Of Int64), codiceUslInserimento As String) As Dictionary(Of KeyValuePair(Of Int64, String), String())

        Dim datiUslDistribuite As New Dictionary(Of KeyValuePair(Of Int64, String), String())()

        If idVaccinazioniEscluseLocali Is Nothing OrElse idVaccinazioniEscluseLocali.Count = 0 Then Return datiUslDistribuite

        Dim uslDistribuitaDatoVaccinaleInfoList As List(Of Entities.UslDistribuitaDatoVaccinaleInfo) =
            Me.GenericProviderCentrale.VaccinazioneEsclusaCentrale.GetUslDistribuiteVaccinazioniEscluse(idVaccinazioniEscluseLocali, codiceUslInserimento)

        If Not uslDistribuitaDatoVaccinaleInfoList Is Nothing AndAlso uslDistribuitaDatoVaccinaleInfoList.Count > 0 Then

            For Each idLocale As Int64 In idVaccinazioniEscluseLocali

                Dim id As Int64 = idLocale

                datiUslDistribuite.Add(New KeyValuePair(Of Int64, String)(idLocale, codiceUslInserimento),
                                       uslDistribuitaDatoVaccinaleInfoList.Where(Function(p) p.IdDatoVaccinale = id AndAlso p.CodiceUslInserimento = codiceUslInserimento).
                                                                           Select(Function(p) p.CodiceUslDistribuita).ToArray())

            Next

        End If

        Return datiUslDistribuite

    End Function

#End Region

#Region " Private "

    Private Function AggiornaVaccinazioneEsclusaCentraleDistribuita(vaccinazioneEsclusa As VaccinazioneEsclusa, vaccinazioneEsclusaCentrale As VaccinazioneEsclusaCentrale, vaccinazioneEsclusaCentraleDistributa As VaccinazioneEsclusaCentrale.VaccinazioneEsclusaDistribuita) As VaccinazioneEsclusaCentrale.VaccinazioneEsclusaDistribuita

        If vaccinazioneEsclusaCentraleDistributa Is Nothing Then

            vaccinazioneEsclusaCentraleDistributa = New VaccinazioneEsclusaCentrale.VaccinazioneEsclusaDistribuita()
            vaccinazioneEsclusaCentraleDistributa.CodiceUslVaccinazioneEsclusa = Me.UslGestitaCorrente.Codice

            vaccinazioneEsclusaCentraleDistributa.IdVaccinazioneEsclusa = vaccinazioneEsclusa.Id
            vaccinazioneEsclusaCentraleDistributa.CodicePaziente = vaccinazioneEsclusa.CodicePaziente

            vaccinazioneEsclusaCentraleDistributa.DataInserimentoVaccinazioneEsclusa = DateTime.Now
            vaccinazioneEsclusaCentraleDistributa.IdUtenteInserimentoVaccinazioneEsclusa = vaccinazioneEsclusaCentrale.IdUtenteInserimentoVaccinazioneEsclusa
            vaccinazioneEsclusaCentraleDistributa.IdVaccinazioneEsclusaCentrale = vaccinazioneEsclusaCentrale.Id

            Me.GenericProviderCentrale.VaccinazioneEsclusaCentrale.InsertVaccinazioneEsclusaCentraleDistribuita(vaccinazioneEsclusaCentraleDistributa)

        Else

            vaccinazioneEsclusaCentraleDistributa.DataAggiornamentoVaccinazioneEsclusa = DateTime.Now

            Me.GenericProviderCentrale.VaccinazioneEsclusaCentrale.UpdateVaccinazioneEsclusaCentraleDistribuita(vaccinazioneEsclusaCentraleDistributa)

        End If

        Return vaccinazioneEsclusaCentraleDistributa

    End Function

    Private Function CreateVaccinazioneEsclusaDestinazioneByOrigine(vaccinazioneEsclusaOrigine As VaccinazioneEsclusa, vaccinazioneEsclusaCentrale As VaccinazioneEsclusaCentrale, codicePazienteDestinazione As Int64) As VaccinazioneEsclusa

        Dim vaccinazioneEsclusa As VaccinazioneEsclusa = vaccinazioneEsclusaOrigine.Clone()
        vaccinazioneEsclusa.CodicePaziente = codicePazienteDestinazione

        If vaccinazioneEsclusaCentrale.MergeInfoCentrale.CodicePazienteCentraleAlias Is Nothing Then
            vaccinazioneEsclusa.CodicePazientePrecedente = Nothing
        Else
            Dim codicePaziente As String = Me.GenericProvider.Paziente.GetCodicePazientiByCodiceAusiliario(vaccinazioneEsclusaCentrale.MergeInfoCentrale.CodicePazienteCentraleAlias).FirstOrDefault()
            If Not String.IsNullOrEmpty(codicePaziente) Then
                vaccinazioneEsclusa.CodicePazientePrecedente = New Int64?(Convert.ToInt64(codicePaziente))
            End If
        End If

        vaccinazioneEsclusa.CodiceOperatore = Nothing

        Return vaccinazioneEsclusa

    End Function

#End Region

#End Region

#Region " OnVac API "

    ''' <summary>
    ''' Restituisce la lista delle escluse del paziente
    ''' </summary>
    ''' <param name="listCodiciPazienti"></param>
    ''' <returns></returns>
    Public Function GetListVaccinazioniEsclusePazientiAPP(listCodiciPazienti As List(Of Int64)) As List(Of Entities.VaccinazioneEsclusaAPP)

        Dim list As List(Of Entities.VaccinazioneEsclusaAPP) =
            Me.GenericProvider.VaccinazioniEscluse.GetListVaccinazioniEsclusePazientiAPP(listCodiciPazienti)

        If Not list Is Nothing AndAlso list.Count > 0 Then

            For Each item As Entities.VaccinazioneEsclusaAPP In list
                item.AppIdAziendaLocale = Me.ContextInfos.IDApplicazione
            Next

        End If

        Return list

    End Function

    ''' <summary>
    ''' Restituisce la vaccinazione esclusa specificata
    ''' </summary>
    ''' <param name="idEsclusa"></param>
    ''' <returns></returns>
    Public Function GetVaccinazioneEsclusaPazienteAPP(idEsclusa As Int64) As Entities.VaccinazioneEsclusaAPP

        Dim esclusa As VaccinazioneEsclusaAPP =
            Me.GenericProvider.VaccinazioniEscluse.GetVaccinazioneEsclusaPazienteAPP(idEsclusa)

        If Not esclusa Is Nothing Then
            esclusa.AppIdAziendaLocale = Me.ContextInfos.IDApplicazione
        End If

        Return esclusa

    End Function

    ''' <summary>
    ''' Restituzione lista escluse per il paziente e la vaccinazione specificati
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="codiceVaccinazioneCovid"></param>
    ''' <returns></returns>
    Public Function GetListVaccinazioniEscluseByPazienteVaccinazioneAPP(codicePaziente As Long, codiceVaccinazione As String) As List(Of VaccinazioneEsclusaAPP)

        Dim list As List(Of VaccinazioneEsclusaAPP) = GenericProvider.VaccinazioniEscluse.GetListVaccinazioniEsclusePazienteAPP(codicePaziente, codiceVaccinazione)

        If Not list Is Nothing AndAlso list.Count > 0 Then

            For Each item As VaccinazioneEsclusaAPP In list
                item.AppIdAziendaLocale = ContextInfos.IDApplicazione
            Next

        End If

        Return list

    End Function

    ''' <summary>
    ''' Restituisce la vaccinazione esclusa specificata
    ''' </summary>
    ''' <param name="idVaccinazioneEsclusa"></param>
    ''' <returns></returns>
    Public Function GetVaccinazioneEsclusaById(idVaccinazioneEsclusa As Long) As VaccinazioneEsclusa

        Return GenericProvider.VaccinazioniEscluse.GetVaccinazioneEsclusaById(idVaccinazioneEsclusa)

    End Function

#Region " FSE "

    ''' <summary>
    ''' Restituisce le vaccinazioni escluse di un dato paziente. Per l'FSE
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    Public Function GetListVaccinazioniEsclusePazienteFSE(codicePaziente As Long) As List(Of Entities.VaccinazioneFSE)

        Dim listEscluse As List(Of Entities.VaccinazioneFSE) = GenericProvider.VaccinazioniEscluse.GetListVaccinazioniEsclusePazienteFSE(codicePaziente)

        Return listEscluse

    End Function


#End Region

#End Region

#End Region

End Class

