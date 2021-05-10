Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Log.DataLogStructure

Public Class BizMalattie
    Inherits BizClass

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)

        Me.New(genericprovider, settings, contextInfos, Nothing, logOptions)

    End Sub

    Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos, bizUslGestitaAllineaSettingsProvider As BizUslGestitaAllineaSettingsProvider, logOptions As BizLogOptions)

        MyBase.New(genericprovider, settings, bizUslGestitaAllineaSettingsProvider, contextInfos, logOptions)

    End Sub

#End Region

#Region " Types "

    Public Class SalvaMalattiePazienteResult

        Public Property Message As String
        Public Property MalattieAggiunte As Boolean
        Public Property MalattieEliminate As Boolean
        Public Property BilanciEliminati As Boolean

        Public Sub New()
            MalattieAggiunte = False
            MalattieEliminate = False
            BilanciEliminati = False
        End Sub

    End Class

    Public Class GetCondizioneSanitariaDefaultPazienteCommand
        Public CodiceVaccinazione As String
        Public CondizioniSanitariePaziente As List(Of Entities.CondizioneSanitaria)
        Public CodiceCondizioneSanitariaDefault As String
        Public DescrizioneCondizioneSanitariaDefault As String
        Public CodiceNessunaMalattia As String
    End Class

    <Serializable()>
    Public Class CodiceDescrizioneMalattia
        Public Property CodiceMalattia As String
        Public Property DescrizioneMalattia As String
    End Class

#End Region

#Region " Shared "

    ''' <summary>
    ''' In base alla vaccinazione specificata e all'elenco di condizioni associate al paziente, restituisce la condizione sanitaria da impostare in automatico.
    ''' </summary>
    ''' <param name="command"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetCondizioneSanitariaDefaultPaziente(command As GetCondizioneSanitariaDefaultPazienteCommand) As Entities.CondizioneSanitaria

        ' Valorizzazione condizione sanitaria:
        ' controllo le condizioni sanitarie associate al paziente:
        '   - se non ne ha nessuna => restituisco la condizione sanitaria di default (parametro)
        '   - se c'è solo 1 condizione sanitaria associata alla vaccinazione corrente => restituisco l'unica condizione sanitaria
        '   - se ci sono solo 2 condizioni sanitarie di cui 1 è la malattia "nessuna" => restituisco quella diversa da "nessuna"
        '   - altrimenti (più di una, esclusa la malattia "nessuna") => restituisco un oggetto vuoto

        Dim condizioneSanitaria As New Entities.CondizioneSanitaria()
        condizioneSanitaria.CodiceMalattia = String.Empty
        condizioneSanitaria.DescrizioneMalattia = String.Empty
        condizioneSanitaria.CodiceVaccinazione = String.Empty

        If command.CondizioniSanitariePaziente Is Nothing OrElse command.CondizioniSanitariePaziente.Count = 0 Then

            ' Nessuna condizione sanitaria associata => restituisco il default
            condizioneSanitaria.CodiceMalattia = command.CodiceCondizioneSanitariaDefault
            condizioneSanitaria.DescrizioneMalattia = command.DescrizioneCondizioneSanitariaDefault
            Return condizioneSanitaria

        End If

        ' Lista condizioni sanitarie del paziente relative alla vaccinazione specificata
        Dim list As List(Of Entities.CondizioneSanitaria) =
            command.CondizioniSanitariePaziente.Where(Function(p) p.CodiceVaccinazione = command.CodiceVaccinazione).ToList()

        If list Is Nothing Then Return condizioneSanitaria

        If list.Count = 1 Then

            ' C'è solo 1 condizione sanitaria associata alla vaccinazione corrente
            condizioneSanitaria = list.First

        ElseIf list.Count = 2 Then

            ' Ci sono 2 condizioni sanitarie associate alla vaccinazione corrente
            If list.Any(Function(p) p.CodiceMalattia = command.CodiceNessunaMalattia) Then

                ' Se una delle due è la malattia "nessuna" => restituisco l'altra 
                condizioneSanitaria = list.First(Function(p) p.CodiceMalattia <> command.CodiceNessunaMalattia)

            End If

        End If

        Return condizioneSanitaria

    End Function

#End Region

#Region " Public "

    ''' <summary>
    ''' Restituisce una lista di codice-descrizione malattie dall'anagrafica, in base ai filtri.
    ''' </summary>
    ''' <param name="includiNessunaMalattia"></param>
    ''' <param name="filtraObsoleti"></param>
    ''' <param name="soloTipologieCompilazioneBilanci"></param>
    ''' <returns></returns>
    Public Function GetMalattie(includiNessunaMalattia As Boolean, filtraObsoleti As Boolean, soloTipologieCompilazioneBilanci As Boolean) As List(Of CodiceDescrizioneMalattia)

        Dim listMalattie As List(Of CodiceDescrizioneMalattia) = Nothing

        Dim list As List(Of KeyValuePair(Of String, String)) = Nothing

        If soloTipologieCompilazioneBilanci Then
            list = GenericProvider.Malattie.GetCodiceDescrizioneMalattie(filtraObsoleti, Settings.VACPROG_TIPOLOGIA_MALATTIA)
        Else
            list = GenericProvider.Malattie.GetCodiceDescrizioneMalattie(filtraObsoleti, Nothing)
        End If

        If list.IsNullOrEmpty() Then

            listMalattie = New List(Of CodiceDescrizioneMalattia)()

        Else

            If includiNessunaMalattia Then
                If Not list.Any(Function(p) p.Key = Settings.CODNOMAL) Then

                    Dim descrizioneNoMal As String = GenericProvider.Malattie.LoadDescrizioneMalattia(Settings.CODNOMAL)

                    If Not String.IsNullOrWhiteSpace(descrizioneNoMal) Then
                        list.Add(New KeyValuePair(Of String, String)(Settings.CODNOMAL, descrizioneNoMal))
                    End If

                End If
            Else
                list.RemoveAll(Function(p) p.Key = Settings.CODNOMAL)
            End If

            listMalattie =
                list.Select(Function(item) New CodiceDescrizioneMalattia() With {
                    .CodiceMalattia = item.Key,
                    .DescrizioneMalattia = item.Value
                }).OrderBy(Function(p) p.DescrizioneMalattia).ToList()

        End If

        Return listMalattie

    End Function

    ''' <summary>
    ''' Restituisce una lista di elementi di tipo malattia, contenente solo quelli aventi codice esenzione valorizzato.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LoadMalattieEsenzione() As List(Of Entities.Malattia)

        Return GenericProvider.Malattie.LoadMalattieEsenzione()

    End Function

    Public Function LoadMalattiePazienteDefault(codicePaziente As Integer) As dsMalattie.MalattieDataTable

        Dim dtaMalattie As dsMalattie.MalattieDataTable = GenericProvider.Malattie.LoadMalattiePaziente(codicePaziente)

        '----------------------------------------------
        ' CONTROLLO PRESENZA ED IMPOSTAZIONE MALATTIA
        ' se non è presente alcuna malattia, occorre impostare il valore "NESSUNA"
        ' E' qui che bisognerebbe implementare l'eventuale aggiunta di malattie standard 
        If dtaMalattie.Rows.Count = 0 Then

            Dim rowMalattie As DataRow = dtaMalattie.NewRow()
            rowMalattie.Item("PMA_MAL_CODICE") = Settings.CODNOMAL
            rowMalattie.Item("PMA_N_MALATTIA") = "1"
            rowMalattie.Item("MAL_DESCRIZIONE") = GenericProvider.Malattie.LoadDescrizioneMalattia(Settings.CODNOMAL)
            rowMalattie.Item("PMA_FOLLOW_UP") = "S"

            dtaMalattie.Rows.Add(rowMalattie)
            dtaMalattie.AcceptChanges()

            ' l'inserimento deve essere eseguito solamente se il codice paziente è già impostato (modifica 13/07/2004)
            If codicePaziente > 0 Then

                ' memorizzazione della malattia nel database
                SalvaMalattiePaziente(codicePaziente, dtaMalattie)

            End If

        End If

        Return dtaMalattie

    End Function

    Public Function SalvaMalattiePaziente(codicePaziente As Integer, dtaMalattie As dsMalattie.MalattieDataTable) As SalvaMalattiePazienteResult

        Dim result As New SalvaMalattiePazienteResult()

        Dim dtMalattiePrecedenti As DataTable = GenericProvider.Malattie.LoadMalattiePaziente(codicePaziente)

        GenericProvider.Malattie.SalvaMalattiePaziente(codicePaziente, dtaMalattie)

        Dim testataLogMalattieEliminate As New Testata(LogOptions.CodiceArgomento, Operazione.Eliminazione, LogOptions.Automatico)
        Dim testataLogMalattieAggiunte As New Testata(LogOptions.CodiceArgomento, Operazione.Inserimento, LogOptions.Automatico)

        Dim recordLog1 As New Record()

        For i As Integer = 0 To dtaMalattie.Rows.Count - 1

            Dim aggiunto As Boolean = True

            For j As Int16 = 0 To dtMalattiePrecedenti.Rows.Count - 1
                If dtaMalattie.Rows(i).RowState <> DataRowState.Deleted AndAlso dtaMalattie.Rows(i)("PMA_MAL_CODICE") = dtMalattiePrecedenti.Rows(j)("PMA_MAL_CODICE") Then
                    aggiunto = False
                    Exit For
                End If
            Next

            If aggiunto Then
                recordLog1.Campi.Add(New Campo("PMA_MAL_CODICE", "", dtaMalattie.Rows(i).Item("PMA_MAL_CODICE")))
            End If

        Next

        If recordLog1.Campi.Count > 0 Then testataLogMalattieAggiunte.Records.Add(recordLog1)

        Dim recordLog2 As New Record()

        For i As Integer = 0 To dtMalattiePrecedenti.Rows.Count - 1

            Dim eliminato As Boolean = True

            For j As Int16 = 0 To dtaMalattie.Rows.Count - 1
                If dtaMalattie.Rows(j).RowState <> DataRowState.Deleted AndAlso dtaMalattie.Rows(j).Item("PMA_MAL_CODICE") = dtMalattiePrecedenti.Rows(i).Item("PMA_MAL_CODICE") Then
                    eliminato = False
                    Exit For
                End If
            Next

            If eliminato Then
                recordLog2.Campi.Add(New Campo("PMA_MAL_CODICE", dtMalattiePrecedenti.Rows(i).Item("PMA_MAL_CODICE")))
            End If

        Next

        If recordLog2.Campi.Count > 0 Then testataLogMalattieEliminate.Records.Add(recordLog2)

        If Not testataLogMalattieAggiunte Is Nothing AndAlso testataLogMalattieAggiunte.Records.Count > 0 Then
            WriteLog(testataLogMalattieAggiunte)
            result.MalattieAggiunte = True
        End If

        If Not testataLogMalattieEliminate Is Nothing AndAlso testataLogMalattieEliminate.Records.Count > 0 Then
            WriteLog(testataLogMalattieEliminate)
            result.MalattieEliminate = True
        End If

        ' Se sono state fatte delle modifiche sulle malattie e si gestiscono i bilanci
        ' mando un messaggio per ricordare di controllare il check "FOLLOW UP" di ogni malattia per gestire bene i bilanci
        If result.MalattieAggiunte OrElse result.MalattieEliminate Then

            If Settings.GESBIL Then

                ' se non vengono eliminate malattie e viene aggiunta solo la malattia "NESSUNA" (in genere è il caso dell'inserimento di un nuovo paziente), non viene mandato nessun messaggio
                If testataLogMalattieEliminate.Records.Count > 0 OrElse Not (testataLogMalattieAggiunte.Records.Count = 1 And testataLogMalattieAggiunte.Records.Item(0).Campi.Item(0).ValoreNuovo.ToString() = Settings.CODNOMAL) Then
                    result.Message = "Sono state modificate delle malattie. \nAssicurarsi di avere impostato bene il FOLLOW UP di tutte le malattie."
                End If

            End If

        End If

        ' per ogni malattia eliminata devo verificare se esistono dei bilanci in programmazione ed eventualmente eliminarli
        If Not testataLogMalattieEliminate Is Nothing AndAlso testataLogMalattieEliminate.Records.Count > 0 Then

            Using bilancioBiz As New BizBilancioProgrammato(GenericProvider, Settings, ContextInfos, LogOptions)

                For j As Short = 0 To testataLogMalattieEliminate.Records.Count - 1

                    ' cancello tutti i bilanci programmati e relativi solleciti delle malattie eliminate
                    Dim codiceMalattia As String = testataLogMalattieEliminate.Records.Item(j).Campi.Item(j).ValoreVecchio.ToString()

                    result.BilanciEliminati = bilancioBiz.CancellaBilanciDiMalattia(codicePaziente, codiceMalattia)

                Next

            End Using

        End If

        Return result

    End Function

    ''' <summary>
    ''' Restituisce il valore booleano del flag visita, per la malattia specificata
    ''' </summary>
    ''' <param name="codiceMalattia"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFlagVisita(codiceMalattia As String) As Boolean

        Return (GenericProvider.Malattie.GetFlagVisita(codiceMalattia) = "S")

    End Function

    ''' <summary>
    ''' Restituisce il datatable con le malattie presenti in anagrafica.
    ''' </summary>
    ''' <param name="includiNessunaMalattia">True se va inclusa anche la malattia "Nessuna", il cui codice è parametrizzato</param>
    ''' <param name="aggiungiRigaTutte">True se deve essere aggiunta anche una riga con codice vuoto e descrizione "TUTTE"</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDtCodiceDescrizioneMalattie(includiNessunaMalattia As Boolean, aggiungiRigaTutte As Boolean) As DataTable

        Dim codiceEsclusioneNessunaMalattia As String = String.Empty

        If Not includiNessunaMalattia Then codiceEsclusioneNessunaMalattia = Settings.CODNOMAL

        Dim dt As DataTable = GenericProvider.Malattie.GetMalattie(codiceEsclusioneNessunaMalattia)

        If aggiungiRigaTutte Then

            Dim emptyRow As DataRow = dt.NewRow()
            emptyRow("MAL_CODICE") = String.Empty
            emptyRow("MAL_DESCRIZIONE") = "TUTTE"

            dt.Rows.InsertAt(emptyRow, 0)

        End If

        Return dt

    End Function

    ''' <summary>
    ''' Restituisce true se la malattia è associata solo a tipologie modificabili
    ''' </summary>
    ''' <param name="codiceMalattia"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsTipologiaMalattiaModificabile(codiceMalattia As String) As Boolean

        Dim codiciTipologiaMalattia As List(Of String) = GenericProvider.Malattie.GetCodiceTipologieByMalattia(codiceMalattia)

        If Settings.GESTPAZ_MALATTIE_NON_MODIFICABILI.IsNullOrEmpty() Then
            Return True
        End If

        Return codiciTipologiaMalattia.All(Function(malattia) Not Settings.GESTPAZ_MALATTIE_NON_MODIFICABILI.Contains(malattia))

    End Function

    ''' <summary>
    ''' Restituisce il codice interno della malattia associata all'esenzione specificata
    ''' </summary>
    ''' <param name="codiceEsenzione"></param>
    ''' <returns></returns>
    Public Function GetCodiceMalattiaByCodiceEsenzione(codiceEsenzione As String) As String

        Return GenericProvider.Malattie.GetCodiceMalattiaByCodiceEsenzione(codiceEsenzione)

    End Function

    ''' <summary>
    ''' Allinea le malattie del paziente: elimina le malattie del paziente con tipologia non editabile e inserisce quelle ricevute dal centrale.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="esenzioniMalattia"></param>
    Public Sub GestioneEsenzioniMalattiaPaziente(codicePaziente As Integer, esenzioniMalattia As List(Of Entities.EsenzioneMalattia))

        ' Questo parametro va valorizzato, altrimenti non so quali malattie devo gestire
        If Settings.GESTPAZ_MALATTIE_NON_MODIFICABILI.IsNullOrEmpty() Then
            Throw New InvalidOperationException("Impossibile gestire le esenzioni malattia senza aver valorizzato il parametro GESTPAZ_MALATTIE_NON_MODIFICABILI")
        End If

        Dim codiceArgomentoLog As String = Log.DataLogStructure.TipiArgomento.PAZIENTI
        Dim flagAutomatico As Boolean = True

        If Not LogOptions Is Nothing Then
            codiceArgomentoLog = LogOptions.CodiceArgomento
            flagAutomatico = LogOptions.Automatico
        End If

        Dim testataLogMalattieInserite As New Testata(codiceArgomentoLog, Operazione.Inserimento, codicePaziente, flagAutomatico)
        Dim testataLogMalattieEliminate As New Testata(codiceArgomentoLog, Operazione.Eliminazione, codicePaziente, flagAutomatico)

        ' Eliminazione malattie che hanno una tipologia non modificabile
        Dim esenzioniDaEliminare As List(Of Entities.EsenzioneMalattia) = GenericProvider.Malattie.GetEsenzioniMalattiaPaziente(codicePaziente)

        If Not esenzioniDaEliminare.IsNullOrEmpty() Then

            For Each item As Entities.EsenzioneMalattia In esenzioniDaEliminare

                If Not IsTipologiaMalattiaModificabile(item.Codice) Then

                    If GenericProvider.Malattie.DeleteMalattiePaziente(codicePaziente, {item.Codice}.ToList()) > 0 Then
                        testataLogMalattieEliminate.Records.Add(CreateRecordLogEsenzioneMalattia(item))
                    End If

                End If

            Next

        End If

        ' Inserimento esenzioni malattia ricevute
        Dim esenzioniDaInserire As New List(Of Entities.EsenzioneMalattia)()

        ' Controlla la presenza della malattia di default "nessuna": se non c'è la aggiunge.
        If Not String.IsNullOrWhiteSpace(Settings.CODNOMAL) Then

            Dim esenzioneNessunaMalattia As Entities.EsenzioneMalattia = GenericProvider.Malattie.GetEsenzioneMalattiaPazienteByCodiceMalattia(codicePaziente, Settings.CODNOMAL)

            If esenzioneNessunaMalattia Is Nothing Then

                esenzioneNessunaMalattia = New Entities.EsenzioneMalattia()
                esenzioneNessunaMalattia.Codice = Settings.CODNOMAL
                esenzioneNessunaMalattia.CodiceEsenzione = Settings.CODNOMAL
                ' esenzioneNessunaMalattia.Numero verrà valorizzato in base alle malattie già presenti

                esenzioniDaInserire.Add(esenzioneNessunaMalattia)

            End If

        End If

        ' Aggiungo le esenzioni malattia ricevute da centrale
        If Not esenzioniMalattia.IsNullOrEmpty() Then
            esenzioniDaInserire.AddRange(esenzioniMalattia)
        End If

        If Not esenzioniDaInserire.IsNullOrEmpty() Then

            ' Non inserisco le esenzioni malattia già scadute non le inserisco
            esenzioniDaInserire.RemoveAll(Function(p) p.DataFineValidita.HasValue AndAlso p.DataFineValidita <= DateTime.Now.Date)

            If Not esenzioniDaInserire.IsNullOrEmpty() Then

                Dim numero As Integer = GenericProvider.Malattie.MaxNumeroMalattiaPaziente(codicePaziente)

                For i As Integer = 0 To esenzioniDaInserire.Count - 1

                    Dim item As Entities.EsenzioneMalattia = esenzioniDaInserire(i)

                    Dim recordLog As New Record()

                    Dim codiceMalattia As String = GenericProvider.Malattie.GetCodiceMalattiaByCodiceEsenzione(item.CodiceEsenzione)

                    If String.IsNullOrWhiteSpace(codiceMalattia) Then

                        recordLog.Campi.Add(New Campo(String.Format("Inserimento esenzione malattia ({0}) non effettuato: mancato mapping codice esenzione - codice interno", item.CodiceEsenzione), String.Empty))

                    Else

                        ' Inserimento
                        Try
                            numero += 1
                            item.Numero = numero
                            item.Codice = codiceMalattia

                            If GenericProvider.Malattie.InsertEsenzioneMalattiaPaziente(codicePaziente, item) > 0 Then
                                recordLog = CreateRecordLogEsenzioneMalattia(item)
                            End If

                        Catch ex As Exception

                            numero -= 1 ' per non fare buchi

                            Common.Utility.EventLogHelper.EventLogWrite(
                                String.Format("Malattia {0} non inserita - Eccezione: {1}", item.Codice, ex.ToString()), ContextInfos.IDApplicazione)

                            recordLog = New Log.DataLogStructure.Record()
                            recordLog.Campi.Add(New Campo("PMA_MAL_CODICE", item.Codice))
                            recordLog.Campi.Add(New Campo("MAL_CODICE_ESENZIONE", item.CodiceEsenzione))
                            recordLog.Campi.Add(New Campo(String.Format("Malattia {0} non inserita - Eccezione", item.Codice), ex.ToString()))

                        End Try

                    End If

                    testataLogMalattieInserite.Records.Add(recordLog)

                Next

            End If

        End If

        Try
            If testataLogMalattieInserite.Records.Count > 0 Then
                WriteLog(testataLogMalattieInserite)
            End If

            If testataLogMalattieEliminate.Records.Count > 0 Then
                WriteLog(testataLogMalattieEliminate)
            End If

        Catch ex As Exception

            ' Errore durante la scrittura del log delle esenzioni da centrale: nascondo l'eccezione + scrittura su eventlog
            Common.Utility.EventLogHelper.EventLogWrite(ex, ContextInfos.IDApplicazione)

        End Try

    End Sub

    ''' <summary>
    ''' Restituisce la descrizione della malattia in base al codice
    ''' </summary>
    ''' <param name="codiceMalattia"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDescrizioneMalattia(codiceMalattia As String) As String

        Return GenericProvider.Malattie.LoadDescrizioneMalattia(codiceMalattia)

    End Function

    ''' <summary>
    ''' Restituisce una lista di entità CondizioneSanitaria contenenti codice malattia, descrizione malattia e codice vaccinazione, 
    ''' per le malattie associate al paziente e relative alle tipologie impostate nel parametro CONDIZIONE_SANITARIA_TIPOLOGIE_MALATTIA.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCondizioniSanitariePaziente(codicePaziente As Integer) As List(Of Entities.CondizioneSanitaria)

        Return GenericProvider.Malattie.GetCondizioniSanitariePaziente(codicePaziente, Settings.CONDIZIONE_SANITARIA_TIPOLOGIE_MALATTIA)

    End Function

    ''' <summary>
    ''' Restituisce le condizioni sanitarie relative al paziente e alla vaccinazione specificati
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="codiceVaccinazione"></param>
    ''' <returns></returns>
    Public Function GetCondizioniSanitariePazienteVaccinazione(codicePaziente As Long, codiceVaccinazione As String) As List(Of Entities.PazienteCondizioneSanitaria)

        Return GenericProvider.Malattie.GetCondizioniSanitarie(codicePaziente, codiceVaccinazione)

    End Function
    Public Function GetCondizioniSanitarieByAssociazione(codicePaziente As Long, codiceAssociazione As String) As List(Of Entities.PazienteCondizioneSanitaria)

        Return GenericProvider.Malattie.GetCondizioniSanitarieByAssociazione(codicePaziente, codiceAssociazione)
    End Function

    ''' <summary>
    ''' Restituisce le coppie codice-descrizione per le malattie specificate
    ''' </summary>
    ''' <param name="codiciMalattie"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCodiceDescrizioneMalattie(codiciMalattie As List(Of String)) As List(Of KeyValuePair(Of String, String))

        Return GenericProvider.Malattie.GetCodiceDescrizioneMalattie(codiciMalattie)

    End Function

#End Region

#Region " Private "

    Private Function CreateRecordLogEsenzioneMalattia(esenzioneMalattia As Entities.EsenzioneMalattia) As Log.DataLogStructure.Record

        Dim recordLog As New Log.DataLogStructure.Record()

        recordLog.Campi.Add(New Campo("PMA_MAL_CODICE", esenzioneMalattia.Codice))
        recordLog.Campi.Add(New Campo("MAL_CODICE_ESENZIONE", esenzioneMalattia.CodiceEsenzione))

        If esenzioneMalattia.Numero.HasValue Then
            recordLog.Campi.Add(New Campo("PMA_N_MALATTIA", esenzioneMalattia.Numero.Value))
        Else
            recordLog.Campi.Add(New Campo("PMA_N_MALATTIA", String.Empty))
        End If

        If esenzioneMalattia.DataInizioValidita.HasValue Then
            recordLog.Campi.Add(New Campo("PMA_DATA_DIAGNOSI", esenzioneMalattia.DataInizioValidita.Value))
        Else
            recordLog.Campi.Add(New Campo("PMA_DATA_DIAGNOSI", String.Empty))
        End If

        If esenzioneMalattia.DataFineValidita.HasValue Then
            recordLog.Campi.Add(New Campo("PMA_DATA_ULTIMA_VISITA", esenzioneMalattia.DataFineValidita.Value))
        Else
            recordLog.Campi.Add(New Campo("PMA_DATA_ULTIMA_VISITA", String.Empty))
        End If

        Return recordLog

    End Function

#End Region

End Class
