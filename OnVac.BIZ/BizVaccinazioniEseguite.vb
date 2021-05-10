Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Log.DataLogStructure
Imports Onit.OnAssistnet.OnVac.Log

Public Class BizVaccinazioniEseguite
    Inherits BizClass

#Region " Constructors "

    Public Sub New(dbGenericProviderFactory As DbGenericProviderFactory, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)

        MyBase.New(dbGenericProviderFactory, settings, Nothing, contextInfos, logOptions)

    End Sub

    Public Sub New(genericprovider As DbGenericProvider, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)

        MyBase.New(genericprovider, settings, contextInfos, logOptions)

    End Sub

    Public Sub New(genericprovider As DbGenericProvider, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos)

        Me.New(genericprovider, settings, contextInfos, New BizLogOptions(Log.DataLogStructure.TipiArgomento.VAC_ESEGUITE))

    End Sub

#End Region

#Region " IEqualityComparers "

    Private Class UslInserimentoEseguitaComparer
        Implements IEqualityComparer(Of Entities.VaccinazioneEseguitaCentrale)

        Public Function Equals1(x As Entities.VaccinazioneEseguitaCentrale, y As Entities.VaccinazioneEseguitaCentrale) As Boolean Implements IEqualityComparer(Of Entities.VaccinazioneEseguitaCentrale).Equals

            If String.IsNullOrEmpty(x.CodiceUslVaccinazioneEseguita) AndAlso String.IsNullOrEmpty(y.CodiceUslVaccinazioneEseguita) Then
                Return False
            End If

            Return (x.CodiceUslVaccinazioneEseguita = y.CodiceUslVaccinazioneEseguita)

        End Function

        Public Function GetHashCode1(obj As Entities.VaccinazioneEseguitaCentrale) As Integer Implements IEqualityComparer(Of Entities.VaccinazioneEseguitaCentrale).GetHashCode

            Return obj.CodiceUslVaccinazioneEseguita.GetHashCode()

        End Function

    End Class

    Private Class UslInserimentoReazioneComparer
        Implements IEqualityComparer(Of Entities.VaccinazioneEseguitaCentrale)

        Public Function Equals1(x As Entities.VaccinazioneEseguitaCentrale, y As Entities.VaccinazioneEseguitaCentrale) As Boolean Implements System.Collections.Generic.IEqualityComparer(Of Entities.VaccinazioneEseguitaCentrale).Equals

            If String.IsNullOrEmpty(x.CodiceUslReazioneAvversa) AndAlso String.IsNullOrEmpty(y.CodiceUslReazioneAvversa) Then
                Return False
            End If

            Return (x.CodiceUslReazioneAvversa = y.CodiceUslReazioneAvversa)

        End Function

        Public Function GetHashCode1(obj As Entities.VaccinazioneEseguitaCentrale) As Integer Implements System.Collections.Generic.IEqualityComparer(Of Entities.VaccinazioneEseguitaCentrale).GetHashCode

            Return obj.CodiceUslReazioneAvversa.GetHashCode()

        End Function

    End Class

    Private Class UslInserimentoConflittoVaccinazioniComparer
        Implements IEqualityComparer(Of Entities.ConflittoVaccinazioniEseguite.DatiVaccinazioneInConflitto)

        Public Function Equals1(x As Entities.ConflittoVaccinazioniEseguite.DatiVaccinazioneInConflitto, y As Entities.ConflittoVaccinazioniEseguite.DatiVaccinazioneInConflitto) As Boolean Implements IEqualityComparer(Of Entities.ConflittoVaccinazioniEseguite.DatiVaccinazioneInConflitto).Equals

            If String.IsNullOrEmpty(x.CodiceUslVaccinazioneEseguita) AndAlso String.IsNullOrEmpty(y.CodiceUslVaccinazioneEseguita) Then
                Return False
            End If

            Return (x.CodiceUslVaccinazioneEseguita = y.CodiceUslVaccinazioneEseguita)

        End Function

        Public Function GetHashCode1(obj As Entities.ConflittoVaccinazioniEseguite.DatiVaccinazioneInConflitto) As Integer Implements IEqualityComparer(Of Entities.ConflittoVaccinazioniEseguite.DatiVaccinazioneInConflitto).GetHashCode

            Return obj.CodiceUslVaccinazioneEseguita.GetHashCode()

        End Function

    End Class

    ''' <summary>
    ''' Comparer per eseguire il distinct delle righe in base al progressivo di associazione
    ''' </summary>
    ''' <remarks></remarks>
    Private Class DeletedIdAssociazioneLottiComparer
        Implements IEqualityComparer(Of DataRow)

        Public Function Equals1(x As System.Data.DataRow, y As System.Data.DataRow) As Boolean Implements System.Collections.Generic.IEqualityComparer(Of System.Data.DataRow).Equals

            If x("ves_ass_prog", DataRowVersion.Original) Is DBNull.Value _
               AndAlso y("ves_ass_prog", DataRowVersion.Original) Is DBNull.Value Then

                Return False

            End If

            Return (x("ves_ass_prog", DataRowVersion.Original) = y("ves_ass_prog", DataRowVersion.Original))

        End Function

        Public Function GetHashCode1(obj As System.Data.DataRow) As Integer Implements System.Collections.Generic.IEqualityComparer(Of System.Data.DataRow).GetHashCode

            Return obj("ves_ass_prog", DataRowVersion.Original).GetHashCode()

        End Function

    End Class

    ''' <summary>
    ''' Comparer per eseguire il distinct delle righe in base a codice vaccinazione
    ''' </summary>
    ''' <remarks></remarks>
    Private Class DeletedVaccinazioniLottiComparer
        Implements IEqualityComparer(Of DataRow)

        Public Function Equals1(x As System.Data.DataRow, y As System.Data.DataRow) As Boolean Implements System.Collections.Generic.IEqualityComparer(Of System.Data.DataRow).Equals

            If x("ves_vac_codice", DataRowVersion.Original) Is DBNull.Value AndAlso
               y("ves_vac_codice", DataRowVersion.Original) Is DBNull.Value Then

                Return False

            End If

            Return (x("ves_vac_codice", DataRowVersion.Original) = y("ves_vac_codice", DataRowVersion.Original))

        End Function

        Public Function GetHashCode1(obj As System.Data.DataRow) As Integer Implements System.Collections.Generic.IEqualityComparer(Of System.Data.DataRow).GetHashCode

            Return obj("ves_vac_codice", DataRowVersion.Original).GetHashCode()

        End Function

    End Class

#End Region

#Region " Select "

    ''' <summary>
    ''' Restituisce il datatable delle vaccinazioni eseguite per il paziente selezionato.
    ''' Se il parametro isGestioneCentrale vale true, esegue la ricerca di vaccinazioni e reazioni avverse 
    ''' nella tabella centrale e recupera i dati locali dalle usl di appartenenza 
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="isGestioneCentrale">Non più previsto dopo la centralizzazione</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Obsolete("Questo metodo è obsoleto: utilizzare GetDtVaccinazioniEseguite(codicePaziente As String)", False)>
    Public Function GetVaccinazioniEseguite(codicePaziente As String, isGestioneCentrale As Boolean) As DataTable

        ' [Unificazione Ulss]: isGestioneCentrale => NON PIU' PREVISTA
        'If isGestioneCentrale Then
        '    Return GetVaccinazioniEseguiteCentralizzate(codicePaziente)
        'End If

        Return GenericProvider.VaccinazioniEseguite.GetVaccinazioniEseguite(codicePaziente)

    End Function

    Public Function GetVaccinazioni(idPaziente As Integer) As List(Of VaccinazioniAPI)
        Return GenericProvider.VaccinazioniEseguite.GetVaccinazioni(idPaziente)
    End Function


    ''' <summary>
    ''' Restituisce un datatable contenente tutte le vaccinazioni (eseguite e scadute) del paziente selezionato
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    Public Function GetDtVaccinazioniEseguite(codicePaziente As Long) As DataTable

        Return GenericProvider.VaccinazioniEseguite.GetVaccinazioniEseguite(codicePaziente)

    End Function

    ''' <summary>
    ''' Restituisce un datatable contenente solo le vaccinazioni (eseguite e scadute) indicate, per il paziente selezionato
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="listaIdVaccinazione"></param>
    ''' <returns></returns>
    Public Function GetDtVaccinazioniEseguite(codicePaziente As Long, listaIdVaccinazione As List(Of Long)) As DataTable

        Return GenericProvider.VaccinazioniEseguite.GetVaccinazioniEseguite(codicePaziente, listaIdVaccinazione)

    End Function

    Public Function GetVaccinazioniDosePaziente(idPaz As Long, dataInizio As Date, dataFine As Date) As List(Of VaccinazioneDose)
        Return GenericProvider.VaccinazioniEseguite.GetVaccinazioniDosePaziente(idPaz, dataInizio, dataFine)
    End Function

    Public Function GetVaccinazioniEseguiteIntegrazione(listareazioni As List(Of Integer)) As DataTable

        Return GenericProvider.VaccinazioniEseguite.GetVaccinazioniEseguiteIntegrazione(listareazioni)

    End Function

#Region " Get Eseguite Centrali "

    'Private Function GetVaccinazioniEseguiteCentralizzate(codicePaziente As Int64) As DataTable

    '    Return GetVaccinazioniEseguiteCentralizzate(GenericProvider.Paziente.GetCodiceAusiliario(codicePaziente))

    'End Function

    'Private Function GetVaccinazioniEseguiteCentralizzate(codicePazienteCentrale As String) As DataTable

    '    ' Elenco eseguite del paziente presenti in centrale con visibilità concessa
    '    Dim listVaccinazioniEseguiteCentrale As IEnumerable(Of VaccinazioneEseguitaCentrale) =
    '        Me.GenericProviderCentrale.VaccinazioneEseguitaCentrale.GetVaccinazioneEseguitaCentraleEnumerable(codicePazienteCentrale, Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente)

    '    ' Se nessun risultato: deve continuare per creare la struttura del datatable (che verrà restituito vuoto, ma con le colonne impostate)
    '    If listVaccinazioniEseguiteCentrale Is Nothing Then
    '        listVaccinazioniEseguiteCentrale = (New List(Of VaccinazioneEseguitaCentrale)()).AsEnumerable()
    '    End If

    '    ' Datatable che verrà restituito (contenente i dati delle vaccinazioni e delle reazioni avverse)
    '    Dim dtVaccinazioniCentralizzate As New DataTable()


    '    ' ------------ '
    '    ' VACCINAZIONI '
    '    ' ------------ '

    '    ' Elenco usl inserimento delle vaccinazioni eseguite
    '    Dim codiciUslInserimentoEseguite As List(Of String) =
    '        listVaccinazioniEseguiteCentrale.
    '        Distinct(New UslInserimentoEseguitaComparer()).
    '        Select(Function(v) v.CodiceUslVaccinazioneEseguita).
    '        ToList()

    '    ' Per ogni usl in cui sono presenti eseguite del paziente, recupero i dati necessari in base agli id
    '    For Each codiceUslInserimentoEseguita As String In codiciUslInserimentoEseguite

    '        Dim codiceUsl As String = codiceUslInserimentoEseguita

    '        ' Elenco id centrali delle eseguite che hanno come usl di inserimento la usl corrente
    '        Dim listEseguiteUsl As IEnumerable(Of Entities.VaccinazioneEseguitaCentrale) =
    '            listVaccinazioniEseguiteCentrale.Where(Function(v) v.CodiceUslVaccinazioneEseguita = codiceUsl)

    '        If Not listEseguiteUsl Is Nothing AndAlso listEseguiteUsl.Count > 0 Then

    '            ' Query sulla singola usl per reperimento dati vaccinazioni eseguite (in base agli id locali presenti nella lista)
    '            Dim dtVaccinazioniUsl As DataTable =
    '                Me.GetDtVaccinazioniEseguiteByUslInserimentoEseguitaAndId(codiceUsl, listEseguiteUsl.Select(Function(v) v.IdVaccinazioneEseguita).ToList())

    '            ' Aggiunta risultati al datatable delle vaccinazioni da restituire
    '            If Not dtVaccinazioniUsl Is Nothing Then

    '                If dtVaccinazioniCentralizzate.Columns.Count = 0 Then

    '                    dtVaccinazioniCentralizzate = dtVaccinazioniUsl.Copy()

    '                Else

    '                    For Each row As DataRow In dtVaccinazioniUsl.Rows

    '                        Dim newRow As DataRow = dtVaccinazioniCentralizzate.NewRow()
    '                        newRow.ItemArray = row.ItemArray
    '                        dtVaccinazioniCentralizzate.Rows.Add(newRow)

    '                    Next

    '                End If
    '            End If
    '        End If

    '    Next

    '    ' Se non è stata creata la struttura delle colonne, eseguo una query "fittizia" per averla comunque.
    '    If dtVaccinazioniCentralizzate.Columns.Count = 0 Then
    '        dtVaccinazioniCentralizzate = Me.GetDtVaccinazioniEseguiteByUslInserimentoEseguitaAndId(Nothing, Nothing)
    '    End If


    '    ' ---------------- '
    '    ' REAZIONI AVVERSE '
    '    ' ---------------- '

    '    ' Datatable contenente i dati delle reazioni avverse
    '    Dim dtReazioniCentralizzate As New DataTable()

    '    ' Elenco usl inserimento reazioni avverse
    '    Dim codiciUslInserimentoReazioni As List(Of String) =
    '        listVaccinazioniEseguiteCentrale.
    '        Where(Function(v) Not String.IsNullOrEmpty(v.CodiceUslReazioneAvversa)).
    '        Distinct(New UslInserimentoReazioneComparer()).
    '        Select(Function(v) v.CodiceUslReazioneAvversa).
    '        ToList()

    '    For Each codiceUslInserimentoReazione As String In codiciUslInserimentoReazioni

    '        Dim codiceUsl As String = codiceUslInserimentoReazione

    '        ' Elenco vaccinazioni centrali aventi la usl corrente come usl di inserimento della reazione avversa
    '        Dim listReazioniUsl As IEnumerable(Of Entities.VaccinazioneEseguitaCentrale) =
    '            listVaccinazioniEseguiteCentrale.Where(Function(v) v.CodiceUslReazioneAvversa = codiceUsl)

    '        If Not listReazioniUsl Is Nothing AndAlso listReazioniUsl.Count > 0 Then

    '            ' Query sulla singola usl per reperimento dati reazioni avverse (in base agli id locali presenti nella lista)
    '            Dim dtReazioniUsl As DataTable =
    '                Me.GetReazioniAvverseByUslInserimentoReazioneAndId(codiceUsl, listReazioniUsl.Select(Function(v) v.IdReazioneAvversa).ToList())

    '            ' Aggiunta risultati al datatable delle reazioni, che verrà unito al datatable da restituire
    '            If Not dtReazioniUsl Is Nothing Then

    '                If dtReazioniCentralizzate.Columns.Count = 0 Then

    '                    dtReazioniCentralizzate = dtReazioniUsl.Copy()

    '                Else

    '                    For Each row As DataRow In dtReazioniUsl.Rows

    '                        Dim newRow As DataRow = dtReazioniCentralizzate.NewRow()
    '                        newRow.ItemArray = row.ItemArray
    '                        dtReazioniCentralizzate.Rows.Add(newRow)

    '                    Next

    '                End If
    '            End If
    '        End If

    '    Next

    '    ' Se non è stata creata la struttura delle colonne, eseguo una query "fittizia" per averla comunque.
    '    If dtReazioniCentralizzate.Columns.Count = 0 Then
    '        dtReazioniCentralizzate = Me.GetReazioniAvverseByUslInserimentoReazioneAndId(Nothing, Nothing)
    '    End If


    '    ' ----------- '
    '    ' UNIONE DATI '
    '    ' ----------- '

    '    ' Aggiunta colonne reazioni avverse al datatable delle vaccinazioni
    '    For Each column As DataColumn In dtReazioniCentralizzate.Columns
    '        dtVaccinazioniCentralizzate.Columns.Add(column.ColumnName)
    '    Next

    '    ' Unione dati vaccinazioni con reazioni avverse
    '    If dtVaccinazioniCentralizzate.Rows.Count > 0 AndAlso dtReazioniCentralizzate.Rows.Count > 0 Then

    '        For Each rowReazione As DataRow In dtReazioniCentralizzate.Rows

    '            Dim codiceUslReazione As String = rowReazione("vra_usl_inserimento")
    '            Dim idReazione As Long = rowReazione("vra_id")

    '            Dim vaccinazioneEseguitaCentrale As Entities.VaccinazioneEseguitaCentrale =
    '                listVaccinazioniEseguiteCentrale.Where(Function(v) v.CodiceUslReazioneAvversa = codiceUslReazione And v.IdReazioneAvversa = idReazione).FirstOrDefault()

    '            If Not vaccinazioneEseguitaCentrale Is Nothing Then

    '                Dim rowVaccDaValorizzare As DataRow = (From r As DataRow In dtVaccinazioniCentralizzate.Rows
    '                                                       Where r("ves_usl_inserimento") = vaccinazioneEseguitaCentrale.CodiceUslVaccinazioneEseguita _
    '                                                       And r("ves_id") = vaccinazioneEseguitaCentrale.IdVaccinazioneEseguita
    '                                                       Select r).FirstOrDefault()

    '                If Not rowVaccDaValorizzare Is Nothing Then

    '                    For Each columnReazione As DataColumn In rowReazione.Table.Columns

    '                        rowVaccDaValorizzare(columnReazione.ColumnName) = rowReazione(columnReazione.ColumnName)

    '                    Next

    '                End If
    '            End If

    '        Next

    '    End If

    '    Return dtVaccinazioniCentralizzate

    'End Function

    '' Query sulla singola usl per reperimento dati vaccinazioni eseguite (in base agli id locali presenti nella lista)
    'Private Function GetDtVaccinazioniEseguiteByUslInserimentoEseguitaAndId(codiceUsl As String, listIdVaccinazioniEseguiteUsl As List(Of Long)) As DataTable

    '    Dim dtVaccinazioniUsl As DataTable = Nothing

    '    If String.IsNullOrEmpty(codiceUsl) AndAlso listIdVaccinazioniEseguiteUsl Is Nothing Then

    '        ' Query "fittizia" per recuperare la struttura da dare al datatable che verrà restituito (senza dati)
    '        Dim listId As New List(Of Long)()
    '        listId.Add(0)

    '        dtVaccinazioniUsl = Me.GenericProvider.VaccinazioniEseguite.GetDtVaccinazioniEseguiteById(listId)

    '    Else

    '        If listIdVaccinazioniEseguiteUsl Is Nothing Then Return Nothing

    '        ' Caricamento dati locali nella usl specificata, in base agli id delle eseguite
    '        Using genericProviderUsl As DbGenericProvider = Me.GetDBGenericProviderByCodiceUslGestita(codiceUsl)

    '            dtVaccinazioniUsl = genericProviderUsl.VaccinazioniEseguite.GetDtVaccinazioniEseguiteById(listIdVaccinazioniEseguiteUsl)

    '        End Using

    '    End If

    '    Return dtVaccinazioniUsl

    'End Function

    'Private Function GetReazioniAvverseByUslInserimentoReazioneAndId(codiceUsl As String, listIdReazioniAvverseUsl As List(Of Long?)) As DataTable

    '    Dim dtReazioniUsl As DataTable = Nothing

    '    Dim listReazioniUsl As List(Of Entities.VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita) = Nothing

    '    If String.IsNullOrEmpty(codiceUsl) AndAlso listIdReazioniAvverseUsl Is Nothing Then

    '        ' Query "fittizia" per recuperare la struttura da dare al datatable che verrà restituito (senza dati)
    '        Dim listId As New List(Of Long?)()
    '        listId.Add(0)

    '        dtReazioniUsl = Me.GenericProvider.VaccinazioniEseguite.GetDtReazioniAvverseById(listId)

    '    Else

    '        If listIdReazioniAvverseUsl Is Nothing Then Return Nothing

    '        ' Caricamento dati locali nella usl specificata, in base agli id delle reazioni
    '        Using genericProviderUsl As DbGenericProvider = Me.GetDBGenericProviderByCodiceUslGestita(codiceUsl)

    '            dtReazioniUsl = genericProviderUsl.VaccinazioniEseguite.GetDtReazioniAvverseById(listIdReazioniAvverseUsl)

    '        End Using

    '    End If

    '    Return dtReazioniUsl

    'End Function

#End Region

#Region " Conflitti Eseguite "

    ''' <summary>
    ''' Conteggio conflitti in base ai filtri impostati
    ''' </summary>
    ''' <param name="filtriRicercaConflitti"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountConflittiVaccinazioniEseguite(filtriRicercaConflitti As Filters.FiltriRicercaConflittiDatiVaccinali) As Integer

        Return Me.GenericProviderCentrale.VaccinazioneEseguitaCentrale.CountConflittiVaccinazioniEseguiteCentrale(filtriRicercaConflitti)

    End Function

    ''' <summary>
    ''' Restituisce la lista delle vaccinazioni eseguite in conflitto, suddivise per paziente, in base ai filtri impostati.
    ''' </summary>
    ''' <param name="filtriRicercaConflitti"></param>
    ''' <param name="pageIndex"></param>
    ''' <param name="pageSize"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetConflittiVaccinazioniEseguite(filtriRicercaConflitti As Filters.FiltriRicercaConflittiDatiVaccinali, pageIndex As Integer?, pageSize As Integer?) As List(Of Entities.ConflittoVaccinazioniEseguite)

        Dim pagingOptions As OnAssistnet.Data.PagingOptions = Nothing

        If Not pageIndex Is Nothing AndAlso Not pageSize Is Nothing Then

            pagingOptions = New OnAssistnet.Data.PagingOptions()

            pagingOptions.PageIndex = pageIndex
            pagingOptions.PageSize = pageSize

        End If

        ' Restituisce le vaccinazioni "padre" che hanno conflitti. 
        ' Ogni elemento contiene i dati centrali del paziente e una lista di tutte le vaccinazioni in conflitto (padre compreso). 
        ' Nella lista dei conflitti sono presenti solo i dati centrali di ogni vaccinazione.
        ' I dati locali (data esecuzione, vaccinazione, dose, ...) devono essere recuperati in base alla usl a cui appartengono.
        Dim listConflittoVaccinazioniEseguite As List(Of Entities.ConflittoVaccinazioniEseguite) =
            Me.GenericProviderCentrale.VaccinazioneEseguitaCentrale.GetConflittiVaccinazioniEseguiteCentrale(filtriRicercaConflitti, pagingOptions)

        If listConflittoVaccinazioniEseguite.IsNullOrEmpty() Then Return Nothing

        CompletaConflittiConDatiLocali(listConflittoVaccinazioniEseguite)

        Return listConflittoVaccinazioniEseguite

    End Function

    ''' <summary>
    ''' Restituisce la lista delle vaccinazioni eseguite in conflitto, in base ai parametri specificati
    ''' </summary>
    ''' <param name="codiciCentraliPazienti"></param>
    ''' <param name="risolviTentativiGiaEffettuati"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetConflittiVaccinazioniEseguite(codiciCentraliPazienti As List(Of String), risolviTentativiGiaEffettuati As Boolean) As List(Of Entities.ConflittoVaccinazioniEseguite)

        Dim listConflittoVaccinazioniEseguite As List(Of Entities.ConflittoVaccinazioniEseguite) =
            Me.GenericProviderCentrale.VaccinazioneEseguitaCentrale.GetConflittiVaccinazioniEseguiteCentrale(codiciCentraliPazienti, risolviTentativiGiaEffettuati)

        If listConflittoVaccinazioniEseguite.IsNullOrEmpty() Then Return Nothing

        CompletaConflittiConDatiLocali(listConflittoVaccinazioniEseguite)

        Return listConflittoVaccinazioniEseguite

    End Function

    ''' <summary>
    ''' Recupera i dati locali di ogni conflitto presente nella lista (data esecuzione, vaccinazione, dose, ...), in base alla usl a cui appartengono i dati.
    ''' </summary>
    ''' <param name="listConflittoVaccinazioniEseguite"></param>
    ''' <remarks></remarks>
    Private Sub CompletaConflittiConDatiLocali(listConflittoVaccinazioniEseguite As List(Of Entities.ConflittoVaccinazioniEseguite))

        If listConflittoVaccinazioniEseguite.IsNullOrEmpty() Then Return

        ' Insieme delle accoppiate Usl-IdVaccinazioni
        Dim vaccinazioniUsl As New Dictionary(Of String, List(Of Int64))()

        ' Valorizzazione delle coppie usl-vaccinazioni
        For Each conflitto As Entities.ConflittoVaccinazioniEseguite In listConflittoVaccinazioniEseguite

            Dim listCodiciUsl As List(Of String) =
                conflitto.VaccinazioniEseguiteInConflitto.
                    Distinct(New UslInserimentoConflittoVaccinazioniComparer()).
                    Select(Function(v) v.CodiceUslVaccinazioneEseguita).ToList()

            For Each codiceUsl As String In listCodiciUsl

                ' Ricerca id vaccinazioni nell'elenco corrente dei conflitti 
                Dim codiceUslCorrente As String = codiceUsl     ' --> utilizzata come filtro nella Where

                Dim listIdVaccinazioni As List(Of Int64) =
                    conflitto.VaccinazioniEseguiteInConflitto.
                        Where(Function(v) v.CodiceUslVaccinazioneEseguita = codiceUslCorrente).
                        Select(Function(v) v.IdVaccinazioneEseguita).ToList()

                If Not listIdVaccinazioni Is Nothing AndAlso listIdVaccinazioni.Count > 0 Then

                    If Not vaccinazioniUsl.ContainsKey(codiceUsl) Then
                        ' Aggiunta dell'accoppiata usl-vaccinazioni al dictionary
                        vaccinazioniUsl.Add(codiceUsl, listIdVaccinazioni)
                    Else
                        ' Aggiunta delle vaccinazioni alla lista relativa alla usl
                        vaccinazioniUsl(codiceUsl) = vaccinazioniUsl(codiceUsl).Union(listIdVaccinazioni).ToList()
                    End If

                End If

            Next
        Next

        Dim enumeratorVaccinazioniUsl As IDictionaryEnumerator = vaccinazioniUsl.GetEnumerator()

        While enumeratorVaccinazioniUsl.MoveNext()

            Dim codiceUslCorrente As String = enumeratorVaccinazioniUsl.Key.ToString()
            Dim listaVaccinazioniInConflittoCorrente As List(Of Int64) = DirectCast(enumeratorVaccinazioniUsl.Value, List(Of Int64))

            ' Ricerca dati eseguite per ogni usl
            Dim vaccinazioniEseguiteLocali As List(Of VaccinazioneEseguita) = Nothing

            ' [Unificazione Ulss]: conflitti => solo vecchia versione
            Using genericProviderUsl As DbGenericProvider = GetDBGenericProviderByCodiceUslGestita(codiceUslCorrente)
                vaccinazioniEseguiteLocali = genericProviderUsl.VaccinazioniEseguite.GetVaccinazioniEseguiteById(listaVaccinazioniInConflittoCorrente)
            End Using

            ' Assegnazione valori locali ai dati delle vaccinazioni in conflitto
            If Not vaccinazioniEseguiteLocali Is Nothing Then

                For Each vaccinazioneEseguitaLocale As VaccinazioneEseguita In vaccinazioniEseguiteLocali

                    Dim idEseguitaCorrente As Int64 = vaccinazioneEseguitaLocale.ves_id

                    For Each conflitto As ConflittoVaccinazioniEseguite In listConflittoVaccinazioniEseguite

                        Dim datiVaccinazioneInConflitto As ConflittoVaccinazioniEseguite.DatiVaccinazioneInConflitto =
                            (From item As ConflittoVaccinazioniEseguite.DatiVaccinazioneInConflitto In conflitto.VaccinazioniEseguiteInConflitto
                             Where item.IdVaccinazioneEseguita = idEseguitaCorrente And item.CodiceUslVaccinazioneEseguita = codiceUslCorrente
                             Select item).FirstOrDefault()

                        If Not datiVaccinazioneInConflitto Is Nothing Then

                            datiVaccinazioneInConflitto.CodiceAssociazione = vaccinazioneEseguitaLocale.ves_ass_codice
                            datiVaccinazioneInConflitto.CodiceLotto = vaccinazioneEseguitaLocale.ves_lot_codice
                            datiVaccinazioneInConflitto.CodiceNomeCommerciale = vaccinazioneEseguitaLocale.ves_noc_codice
                            datiVaccinazioneInConflitto.CodiceVaccinazione = vaccinazioneEseguitaLocale.ves_vac_codice
                            datiVaccinazioneInConflitto.DataEffettuazione = vaccinazioneEseguitaLocale.ves_dataora_effettuazione
                            datiVaccinazioneInConflitto.DoseAssociazione = vaccinazioneEseguitaLocale.ves_ass_n_dose
                            datiVaccinazioneInConflitto.DoseVaccinazione = vaccinazioneEseguitaLocale.ves_n_richiamo
                            datiVaccinazioneInConflitto.DataRegistrazione = vaccinazioneEseguitaLocale.ves_data_registrazione
                            datiVaccinazioneInConflitto.Stato = vaccinazioneEseguitaLocale.ves_stato

                            Exit For

                        End If

                    Next
                Next

            End If

        End While

    End Sub

#Region " Types "

    Public Class AutoRisoluzioneConflittiResult
        Inherits BizGenericResult

        Public ConflittoPrincipale As Entities.ConflittoVaccinazioniEseguite.DatiVaccinazioneInConflitto

        ''' <summary>
        ''' Id dei conflitti che non sono stati risolti, anche se il conflitto è stato risolto.
        ''' In caso di risoluzione effettuata, contiene i conflitti non considerati dalla risoluzione perchè non aventi stessa data, vaccinazione e dose del master.
        ''' </summary>
        ''' <remarks></remarks>
        Public IdConflittiNonRisolti As List(Of Long)

        <DebuggerNonUserCode>
        Public Sub New()
            Me.IdConflittiNonRisolti = New List(Of Long)()
        End Sub

    End Class

    Public Class GetConflittoPrincipaleResult
        Inherits BizGenericResult

        Public ConflittoPrincipale As Entities.ConflittoVaccinazioniEseguite.DatiVaccinazioneInConflitto

    End Class

    Public Class BatchRisoluzioneConflitti

        <DebuggerNonUserCode>
        Public Sub New()
        End Sub

        Public Class ParameterName

            Public Const IdApplicazioneCentrale As String = "IdApplicazioneCentrale"
            Public Const IdUtenteRisoluzione As String = "IdUtenteRisoluzione"
            Public Const IdProcessoBatch As String = "IdProcessoBatch"
            Public Const CodiciCentraliPazienti As String = "CodiciCentraliPazienti"
            Public Const RisolviTentativiGiaEffettuati As String = "RisolviTentativiGiaEffettuati"
            Public Const NumeroConflittiRefreshRisultatoParziale As String = "NumeroConflittiRefreshRisultatoParziale"

            <DebuggerNonUserCode>
            Public Sub New()
            End Sub

        End Class

        Public Class StatoRisoluzione
            Public Const ConflittoRisolto As String = "S"
            Public Const ConflittoNonRisolto As String = "N"
        End Class

        Public Class AutoRisoluzioneConflittiEsistentiCommand

            ''' <summary>
            ''' Id dell'utente che effettua la risoluzione dei conflitti.
            ''' </summary>
            ''' <remarks></remarks>
            Public IdUtenteRisoluzione As Long

            ''' <summary>
            ''' Id del processo batch che si sta eseguendo.
            ''' </summary>
            ''' <remarks></remarks>
            Public IdProcessoBatch As Long

            ''' <summary>
            ''' Se true: tenta di risolvere tutti i conflitti che trova.
            ''' Se false: tenta di risolvere solo i conflitti che non aveva già tentato di risolvere in precedenza.
            ''' </summary>
            ''' <remarks></remarks>
            Public RisolviTentativiGiaEffettuati As Boolean

            ''' <summary>
            ''' Lista codici centrali dei pazienti a cui risolvere i conflitti
            ''' </summary>
            ''' <remarks></remarks>
            Public CodiciCentraliPazienti As List(Of String)

            ''' <summary>
            ''' Numero di conflitti master dopo i quali effettuare un refresh dei risultati parziali dell'eleaborazione
            ''' </summary>
            ''' <remarks></remarks>
            Public NumeroConflittiRefreshRisultatoParziale As Integer

        End Class

    End Class

#End Region

#Region " Autorisoluzione conflitti "

    Public Event RefreshTotaleConflittiDaElaborare(e As BizBatch.RefreshTotaleElementiDaElaborareEventArgs)
    Public Event RefreshParzialeConflittiElaborati(e As BizBatch.RefreshParzialeElementiElaboratiEventArgs)

    ''' <summary>
    ''' Metodo di risoluzione automatica dei conflitti già esistenti sul db centrale.
    ''' </summary>
    ''' <param name="command"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AutoRisoluzioneConflittiEsistenti(command As BatchRisoluzioneConflitti.AutoRisoluzioneConflittiEsistentiCommand) As BizGenericResult

        Dim result As New BizGenericResult()
        result.Success = True
        result.Message = String.Empty

        Dim numeroElaborazioni As Integer = 0
        Dim numeroErrori As Integer = 0

        ' Recupero conflitti da T_VACCINAZIONI_CENTRALE 
        Dim listConflittiMaster As List(Of Entities.ConflittoVaccinazioniEseguite) =
            Me.GetConflittiVaccinazioniEseguite(command.CodiciCentraliPazienti, command.RisolviTentativiGiaEffettuati)

        Try
            Dim countConflitti As Integer = 0

            ' Inserimento in T_CONFLITTI_RISOLUZIONE (se presenti)
            If Not listConflittiMaster.IsNullOrEmpty() Then
                countConflitti = listConflittiMaster.Count
                InsertConflittiRisoluzione(command.IdProcessoBatch, listConflittiMaster)
            End If

            ' Aggiornamento del totale dei conflitti da risolvere
            RaiseEvent RefreshTotaleConflittiDaElaborare(New Biz.BizBatch.RefreshTotaleElementiDaElaborareEventArgs(countConflitti))

            If Not listConflittiMaster.IsNullOrEmpty() Then

                Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions())

                    Try
                        For Each conflittoMaster As Entities.ConflittoVaccinazioniEseguite In listConflittiMaster

                            numeroElaborazioni += 1

                            ' --------- '
                            ' Ricerca del conflitto principale e, se trovato, risoluzione.
                            Dim conflittoMasterRisoltoResult As AutoRisoluzioneConflittiResult =
                                AutoRisolviConflittoVaccinazioneEseguitaCentrale(conflittoMaster)
                            ' --------- '

                            If Not conflittoMasterRisoltoResult.Success Then
                                numeroErrori += 1
                            End If

                            If Not conflittoMaster.VaccinazioniEseguiteInConflitto.IsNullOrEmpty() Then

                                ' 1. Creazione di una entity conflittoRisoluzione per ogni item della conflitto.VaccinazioniEseguiteInConflitto 
                                ' 2. Valorizzazione stato, message e, se conflitto risolto, id conflitto vincitore
                                ' 3. Update t_conflitti_risoluzione con i dati di risoluzione
                                For Each conflitto As Entities.ConflittoVaccinazioniEseguite.DatiVaccinazioneInConflitto In conflittoMaster.VaccinazioniEseguiteInConflitto

                                    ' Informazioni sul risultato della risoluzione, loggate in t_conflitti_risoluzione
                                    Dim conflittoRisoluzione As Entities.ConflittoEseguiteRisoluzione =
                                        CreateConflittoRisoluzioneByDatiConflitto(command.IdProcessoBatch, conflittoMaster, conflitto)

                                    Dim isConflittoRisolto As Boolean =
                                        Not conflittoMasterRisoltoResult.IdConflittiNonRisolti.Contains(conflittoRisoluzione.IdVaccinazioneEseguitaCentrale)

                                    If conflittoMasterRisoltoResult.Success Then

                                        If isConflittoRisolto Then

                                            ' L'autorisoluzione ha determinato il conflitto principale e anche il conflitto corrente è risolto
                                            conflittoRisoluzione.StatoRisoluzioneConflitto = BatchRisoluzioneConflitti.StatoRisoluzione.ConflittoRisolto
                                            conflittoRisoluzione.IdConflittoRisoluzione = conflittoMasterRisoltoResult.ConflittoPrincipale.Id
                                            conflittoRisoluzione.Message = conflittoMasterRisoltoResult.Message

                                        Else

                                            ' L'autorisoluzione ha determinato il conflitto principale ma il conflitto corrente non è risolto
                                            conflittoRisoluzione.StatoRisoluzioneConflitto = BatchRisoluzioneConflitti.StatoRisoluzione.ConflittoNonRisolto
                                            conflittoRisoluzione.IdConflittoRisoluzione = Nothing

                                            If conflittoMasterRisoltoResult.ConflittoPrincipale Is Nothing Then
                                                conflittoRisoluzione.Message = String.Empty
                                            Else
                                                conflittoRisoluzione.Message = "Conflitto non risolto: data esecuzione, vaccinazione o dose diversi dal conflitto principale"
                                            End If

                                        End If

                                    Else

                                        ' L'autorisoluzione non ha risolto il conflitto
                                        conflittoRisoluzione.StatoRisoluzioneConflitto = BatchRisoluzioneConflitti.StatoRisoluzione.ConflittoNonRisolto
                                        conflittoRisoluzione.IdConflittoRisoluzione = Nothing
                                        conflittoRisoluzione.Message = conflittoMasterRisoltoResult.Message

                                    End If

                                    Me.GenericProvider.VaccinazioneEseguitaCentrale.UpdateConflittoEseguiteRisoluzione(conflittoRisoluzione)

                                Next

                                ' In caso di risoluzione del conflitto => sbiancamento id conflitto per tutti i conflitti non risolti
                                If conflittoMasterRisoltoResult.Success Then
                                    For Each id As Long In conflittoMasterRisoltoResult.IdConflittiNonRisolti
                                        Me.GenericProvider.VaccinazioneEseguitaCentrale.CancellaIdConflittoVaccinazioneEseguitaCentrale(id)
                                    Next
                                End If

                            End If

                            ' --------- '
                            ' N.B. : commentato perchè internamente al transaction scope non deve essere richiamato!!!
                            '        Il problema è che la query che viene eseguita da onbatch per l'aggiornamento dei parziali apre una sua transazione 
                            '        che va in conflitto con il transactionscope e genera un'eccezione di "transazione in attesa di lock".
                            ' --------- '
                            ' Aggiornamento risultato parziale dell'elaborazione
                            'If numeroElaborazioni Mod command.NumeroConflittiRefreshRisultatoParziale = 0 Then
                            '    RaiseEvent RefreshParzialeConflittiElaborati(New Biz.BizBatch.RefreshParzialeElementiElaboratiEventArgs(numeroElaborazioni, numeroErrori))
                            'End If
                            ' --------- '

                        Next

                        ' Acquisizione dati vaccinali per sistemare lo stato acquisizione di ogni paziente e gli eventuali conflitti non risolti
                        '   1. per ogni codice centrale paziente, recuperare tutte le ulss in cui è presente 
                        '   2. per ogni ulss, effettuare l'acquisizione dati dei pazienti in stato acquisizione parziale
                        Dim listPazientiCentrali As List(Of String) =
                                listConflittiMaster.Select(Function(p) p.CodicePazienteCentrale).Distinct().ToList()

                        Dim listDistribuzionePazientiUsl As List(Of Entities.PazienteInfoDistribuzione) =
                            Me.GenericProviderCentrale.Paziente.GetInfoDistribuzioneCompletaPazienti(listPazientiCentrali)

                        Dim listAppId As IEnumerable(Of String) = listDistribuzionePazientiUsl.Select(Function(p) p.AppIdUsl).Distinct()

                        Dim acquisisciDatiVaccinaliCentraliResult As BizPaziente.AcquisisciDatiVaccinaliCentraliResult = Nothing

                        For Each appId As String In listAppId

                            Dim listPazientiUsl As IEnumerable(Of PazienteInfoDistribuzione) =
                                listDistribuzionePazientiUsl.Where(Function(p) p.AppIdUsl = appId).ToList()

                            Using genericProviderUsl As DbGenericProvider = GenericProviderFactory.GetDbGenericProvider(appId, ContextInfos.CodiceAzienda)

                                Dim bizContextInfosUsl As New BizContextInfos(ContextInfos.IDUtente, ContextInfos.CodiceAzienda, appId, String.Empty, ContextInfos.CodiceUsl, Nothing)
                                Dim settingsUsl As New Settings.Settings(genericProviderUsl)

                                Using bizPazienteUsl As New BizPaziente(genericProviderUsl, settingsUsl, bizContextInfosUsl, LogOptions)

                                    For Each pazienteUsl As PazienteInfoDistribuzione In listPazientiUsl

                                        Dim statoAcquisizione As Enumerators.StatoAcquisizioneDatiVaccinaliCentrale? =
                                            genericProviderUsl.Paziente.GetStatoAcquisizioneDatiVaccinaliCentrale(pazienteUsl.CodiceLocalePaziente)

                                        If statoAcquisizione = Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Parziale Then

                                            Dim acquisisciDatiVaccinaliCentraliCommand As New BizPaziente.AcquisisciDatiVaccinaliCentraliCommand()
                                            acquisisciDatiVaccinaliCentraliCommand.CodicePaziente = pazienteUsl.CodiceLocalePaziente
                                            acquisisciDatiVaccinaliCentraliCommand.CodicePazienteCentrale = pazienteUsl.CodiceCentralePaziente
                                            acquisisciDatiVaccinaliCentraliCommand.CodiceConsultorioPaziente = bizPazienteUsl.GenericProvider.Paziente.GetCodiceConsultorio(pazienteUsl.CodiceLocalePaziente)
                                            acquisisciDatiVaccinaliCentraliCommand.RichiediConfermaSovrascrittura = False
                                            acquisisciDatiVaccinaliCentraliCommand.Note = "Risoluzione automatica conflitti"

                                            acquisisciDatiVaccinaliCentraliResult = bizPazienteUsl.AcquisisciDatiVaccinaliCentrali(acquisisciDatiVaccinaliCentraliCommand)

                                        End If

                                    Next

                                End Using
                            End Using

                        Next

                    Catch ex As Exception
                        result.Success = False
                        result.Message = ex.ToString()
                    Finally
                        transactionScope.Complete()
                    End Try

                End Using
            End If

        Catch ex As Exception
            numeroErrori += 1
            ex.InternalPreserveStackTrace()
            Throw
        Finally
            ' Aggiornamento del numero di elaborazioni e di errori, per avere il numero totale al termine dell'elaborazione e fuori dal transaction scope!
            RaiseEvent RefreshParzialeConflittiElaborati(New Biz.BizBatch.RefreshParzialeElementiElaboratiEventArgs(numeroElaborazioni, numeroErrori))
        End Try

        Return result

    End Function

    Private Sub InsertConflittiRisoluzione(idProcessoBatch As Long, listConflittiMaster As List(Of Entities.ConflittoVaccinazioniEseguite))

        If listConflittiMaster.IsNullOrEmpty Then Return

        For Each conflittoMaster As Entities.ConflittoVaccinazioniEseguite In listConflittiMaster
            For Each conflitto As Entities.ConflittoVaccinazioniEseguite.DatiVaccinazioneInConflitto In conflittoMaster.VaccinazioniEseguiteInConflitto

                Dim conflittoRisoluzione As Entities.ConflittoEseguiteRisoluzione =
                    CreateConflittoRisoluzioneByDatiConflitto(idProcessoBatch, conflittoMaster, conflitto)

                Me.GenericProvider.VaccinazioneEseguitaCentrale.InsertConflittoEseguiteRisoluzione(conflittoRisoluzione)

            Next
        Next

    End Sub

    Private Function CreateConflittoRisoluzioneByDatiConflitto(idProcessoBatch As Long, conflittoMaster As Entities.ConflittoVaccinazioniEseguite, datiEseguitaInConflitto As Entities.ConflittoVaccinazioniEseguite.DatiVaccinazioneInConflitto) As Entities.ConflittoEseguiteRisoluzione

        Dim conflittoRisoluzione As New Entities.ConflittoEseguiteRisoluzione()
        conflittoRisoluzione.IdProcessoBatch = idProcessoBatch
        conflittoRisoluzione.IdVaccinazioneEseguitaCentrale = datiEseguitaInConflitto.Id
        conflittoRisoluzione.IdVaccinazioneEseguitaLocale = datiEseguitaInConflitto.IdVaccinazioneEseguita
        conflittoRisoluzione.CodiceUslInserimento = datiEseguitaInConflitto.CodiceUslVaccinazioneEseguita
        conflittoRisoluzione.CodicePazienteCentrale = datiEseguitaInConflitto.CodicePazienteCentrale
        conflittoRisoluzione.CodicePazienteLocale = datiEseguitaInConflitto.CodicePaziente
        conflittoRisoluzione.IdConflitto = conflittoMaster.IdVaccinazioneEseguitaCentrale

        Return conflittoRisoluzione

    End Function

    ''' <summary>
    ''' Applica la logica di auto-risoluzione ad un singolo conflitto tra n vaccinazioni eseguite.
    ''' </summary>
    ''' <param name="conflitto"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function AutoRisolviConflittoVaccinazioneEseguitaCentrale(conflitto As ConflittoVaccinazioniEseguite) As AutoRisoluzioneConflittiResult

        Dim result As New AutoRisoluzioneConflittiResult()

        If conflitto Is Nothing OrElse conflitto.VaccinazioniEseguiteInConflitto.IsNullOrEmpty() Then
            result.Success = False
            result.Message = "Nessun conflitto da risolvere"
            Return result
        End If

        ' Recupero i dati del master del conflitto
        Dim vaccinazioneInConflittoMaster As Entities.ConflittoVaccinazioniEseguite.DatiVaccinazioneInConflitto =
            conflitto.VaccinazioniEseguiteInConflitto.FirstOrDefault(Function(p) p.Id = conflitto.IdVaccinazioneEseguitaCentrale)

        If vaccinazioneInConflittoMaster Is Nothing Then
            result.Success = False
            result.Message = "Tra i conflitti non è presente la vaccinazione master"
            Return result
        End If

        ' Se nella lista iniziale dei conflitti c'è SOLO il master 
        '   =>  risolve il finto conflitto del master con se stesso, impostando data e utente di risoluzione del conflitto 
        '       nella t_vaccinazioni_centrale (a livello di db locali non c'è niente da fare)
        If conflitto.VaccinazioniEseguiteInConflitto.Count = 1 Then

            Me.GenericProvider.VaccinazioneEseguitaCentrale.UpdateConflittoEseguitaCentrale(conflitto.IdVaccinazioneEseguitaCentrale, Me.ContextInfos.IDUtente, DateTime.Now)

            result.Success = True
            result.Message = "Vaccinazione eseguita in conflitto solo con se stessa: conflitto risolto eliminandolo da centrale"
            result.ConflittoPrincipale = conflitto.VaccinazioniEseguiteInConflitto.Single()
            Return result

        End If


        ' TODO [conflitti]: questa logica viene utilizzata anche nel metodo AutoRisolviConflittoVaccinazioneEseguitaCentrale (che è un caso particolare con 2 sole vacc in conflitto).
        '                   I 2 metodi andrebbero accorpati in uno solo, ma non ho tempo!                   

        ' LOGICA RISOLUZIONE CONFLITTI x n vaccinazioni in conflitto:
        '   Nel caso in cui i dati coincidano (data effettuazione, vaccinazione, dose), il conflitto si auto-risolve secondo questa logica:
        '    1. Se, tra tutte le vaccinazioni in conflitto, solo una ha il dato del lotto => vaccinazione "master", con i dati corretti (compresa la ulss proprietaria)
        '    2. Else: controllo presenza nome commerciale per una sola vaccinazione
        '    3. Else: controllo se solo una ha il ves_stato a “eseguita”
        '    4. Else: seleziono quella con data di registrazione inferiore (se vaccinazione univoca)
        '    5. Else: ULSS proprietaria del dato la prima ULSS che ha scritto il dato in centrale (per data inserimento o, se uguale, per vcc_id inferiore)


        ' Dalla lista dei conflitti, considero solo quelli che hanno stessa data, stessa vaccinazione e stessa dose.
        Dim listConflittiDaRisolvere As List(Of Entities.ConflittoVaccinazioniEseguite.DatiVaccinazioneInConflitto) =
            conflitto.VaccinazioniEseguiteInConflitto.Where(
                Function(p) p.DataEffettuazione = vaccinazioneInConflittoMaster.DataEffettuazione And
                            p.CodiceVaccinazione = vaccinazioneInConflittoMaster.CodiceVaccinazione And
                            p.DoseVaccinazione = vaccinazioneInConflittoMaster.DoseVaccinazione).ToList()

        If listConflittiDaRisolvere.Count < conflitto.VaccinazioniEseguiteInConflitto.Count Then

            result.IdConflittiNonRisolti = conflitto.VaccinazioniEseguiteInConflitto.
                Where(Function(p) Not listConflittiDaRisolvere.Any(Function(q) q.Id = p.Id)).
                Select(Function(item) item.Id).ToList()

        End If

        ' Nella lista dei conflitti da risolvere (quelli che hanno stessa data effettuazione, stessa vaccinazione e stessa dose) 
        ' c'è sempre almeno 1 elemento: il conflitto indicato come "master". 
        ' Se nella lista dei conflitti da risolvere c'è SOLO il master e nella lista iniziale dei conflitti ci sono anche altri elementi
        '   => non posso risolvere nessun conflitto
        If listConflittiDaRisolvere.Count = 1 AndAlso conflitto.VaccinazioniEseguiteInConflitto.Count > 1 Then
            result.Success = False
            result.Message = "Le vaccinazioni in conflitto non hanno data esecuzione, vaccinazione e dose uguali."
            Return result
        End If

        ' Tento di stabilire quale tra le vaccinazioni in conflitto possa essere presa come riferimento
        Dim conflittoPrincipaleResult As GetConflittoPrincipaleResult =
            GetConflittoPrincipaleAutoRisoluzione(listConflittiDaRisolvere)

        result.Message = conflittoPrincipaleResult.Message
        result.ConflittoPrincipale = conflittoPrincipaleResult.ConflittoPrincipale

        ' Impossibile determinare in automatico quale conflitto vince => conflitto non risolto
        If conflittoPrincipaleResult.ConflittoPrincipale Is Nothing Then
            result.Success = False
            Return result
        End If

        ' Trovata vaccinazione principale "vincitrice" del conflitto => Risoluzione dei conflitti
        Try
            Using bizPaziente As Biz.BizPaziente = New Biz.BizPaziente(Me.GenericProviderFactory, Me.Settings, Me.ContextInfos, Me.LogOptions)

                ' Creo la lista con gli id centrali delle vaccinazioni in conflitto (esclusa la vincitrice)
                Dim listIdCentraleVaccinazioniEseguiteInConflitto As List(Of Long) =
                    listConflittiDaRisolvere.Where(Function(p) p.Id <> conflittoPrincipaleResult.ConflittoPrincipale.Id).Select(Function(p) p.Id).ToList()

                Dim risolviConflittoCommand As New Biz.BizPaziente.RisolviConflittoVaccinazioniEseguiteCentraleCommand()
                risolviConflittoCommand.IdVaccinazioniEseguiteCentralePazienteDictionary = New Dictionary(Of Int64, IEnumerable(Of Int64))()

                ' Imposto id centrale vaccinazione master e lista id centrali vaccinazioni in conflitto
                risolviConflittoCommand.IdVaccinazioniEseguiteCentralePazienteDictionary.Add(
                    conflittoPrincipaleResult.ConflittoPrincipale.Id, listIdCentraleVaccinazioniEseguiteInConflitto.AsEnumerable())

                bizPaziente.RisolviConflittoVaccinazioniEseguiteCentrale(risolviConflittoCommand)

            End Using
        Catch ex As Exception
            result.Success = False
            result.Message = ex.ToString()
        End Try

        Return result

    End Function

    Private Function GetConflittoPrincipaleAutoRisoluzione(listConflittiDaRisolvere As List(Of Entities.ConflittoVaccinazioniEseguite.DatiVaccinazioneInConflitto)) As GetConflittoPrincipaleResult

        Dim result As New GetConflittoPrincipaleResult()

        ' Se non ci sono vaccinazioni in conflitto, non c'è niente da risolvere
        If listConflittiDaRisolvere.IsNullOrEmpty() Then
            result.Success = True
            result.Message = "Nessuna vaccinazione in conflitto"
            result.ConflittoPrincipale = Nothing
            Return result
        End If

        Dim dummyList As IEnumerable(Of Entities.ConflittoVaccinazioniEseguite.DatiVaccinazioneInConflitto)

        ' Controllo sul lotto
        dummyList = listConflittiDaRisolvere.Where(Function(p) Not String.IsNullOrWhiteSpace(p.CodiceLotto))
        If dummyList.Count() = 1 Then
            result.Success = True
            result.Message = "Conflitto risolto in base al LOTTO"
            result.ConflittoPrincipale = listConflittiDaRisolvere.Single(Function(p) Not String.IsNullOrWhiteSpace(p.CodiceLotto))
            Return result
        End If

        ' Controllo sul nome commerciale
        dummyList = listConflittiDaRisolvere.Where(Function(p) Not String.IsNullOrWhiteSpace(p.CodiceNomeCommerciale))
        If dummyList.Count() = 1 Then
            result.Success = True
            result.Message = "Conflitto risolto in base al NOME COMMERCIALE"
            result.ConflittoPrincipale = listConflittiDaRisolvere.Single(Function(p) Not String.IsNullOrWhiteSpace(p.CodiceNomeCommerciale))
            Return result
        End If

        ' Controllo sul ves_stato
        dummyList = listConflittiDaRisolvere.Where(Function(p) IsStatoVaccinazioneEseguita(p.Stato))
        If dummyList.Count() = 1 Then
            result.Success = True
            result.Message = "Conflitto risolto in base allo STATO dell'eseguita in locale"
            result.ConflittoPrincipale = listConflittiDaRisolvere.Single(Function(p) IsStatoVaccinazioneEseguita(p.Stato))
            Return result
        End If

        ' Data di registrazione inferiore (se presente)
        If Not listConflittiDaRisolvere.All(Function(p) Not p.DataRegistrazione.HasValue) Then

            Dim dataMinimaRegistrazione As DateTime =
                listConflittiDaRisolvere.Where(Function(p) p.DataRegistrazione.HasValue).Min(Function(p) p.DataRegistrazione.Value)

            dummyList = listConflittiDaRisolvere.Where(Function(p) p.DataRegistrazione.HasValue AndAlso p.DataRegistrazione.Value = dataMinimaRegistrazione)
            If dummyList.Count() = 1 Then
                result.Success = True
                result.Message = "Conflitto risolto in base alla DATA DI REGISTRAZIONE dell'eseguita in locale"
                result.ConflittoPrincipale = listConflittiDaRisolvere.Single(Function(p) p.DataRegistrazione.HasValue AndAlso p.DataRegistrazione.Value = dataMinimaRegistrazione)
                Return result
            End If

        End If

        '  Data inserimento in centrale minore (sempre presente)
        Dim dataMinimaInserimento As DateTime = listConflittiDaRisolvere.Min(Function(p) p.DataInserimentoCentrale)

        dummyList = listConflittiDaRisolvere.Where(Function(p) p.DataInserimentoCentrale = dataMinimaInserimento)
        If dummyList.Count() = 1 Then
            result.Success = True
            result.Message = "Conflitto risolto in base alla DATA DI INSERIMENTO in centrale"
            result.ConflittoPrincipale = listConflittiDaRisolvere.Single(Function(p) p.DataInserimentoCentrale = dataMinimaInserimento)
            Return result
        End If

        ' ID centrale minore (ce n'è sicuramente solo uno)
        result.Success = True
        result.Message = "Conflitto risolto in base all'ID di inserimento in centrale"
        result.ConflittoPrincipale = listConflittiDaRisolvere.OrderBy(Function(p) p.Id).First()
        Return result

    End Function

    Private Function IsStatoVaccinazioneEseguita(statoVaccinazione As String) As Boolean

        Return String.IsNullOrWhiteSpace(statoVaccinazione) OrElse statoVaccinazione = "E"

    End Function

#End Region

#End Region

    Public Function GetProssimaSedutaDaRegistrare(codicePaziente As Integer) As DataTable

        Return Me.GenericProvider.VaccinazioniEseguite.GetProssimaSedutaDaRegistrare(codicePaziente)

    End Function

    ''' <summary>
    ''' Restituisce il numero di vaccinazioni eseguite del paziente. Se il parametro includiScadute è True, 
    ''' aggiunge anche il numero di vaccinazioni scadute
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="includiScadute"></param>
    ''' <param name="isGestioneCentrale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountVaccinazioniEseguite(codicePaziente As String, includiScadute As Boolean, isGestioneCentrale As Boolean) As Integer

        If isGestioneCentrale Then Return 0

        Dim numVaccinazioniEseguite As Integer = Me.GenericProvider.VaccinazioniEseguite.CountVaccinazioniEseguite(codicePaziente)
        Dim numVaccinazioniScadute As Integer = 0

        If includiScadute Then
            numVaccinazioniScadute = Me.GenericProvider.VaccinazioniEseguite.CountVaccinazioniScadute(codicePaziente)
        End If

        Return numVaccinazioniEseguite + numVaccinazioniScadute

    End Function

    ''' <summary>
    ''' Restituisce il numero di reazioni avverse del paziente
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="isGestioneCentrale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountReazioniAvverse(codicePaziente As String, isGestioneCentrale As Boolean) As Integer

        If isGestioneCentrale Then Return 0

        Return Me.GenericProvider.VaccinazioniEseguite.CountReazioniAvverse(codicePaziente)

    End Function

    ''' <summary>
    ''' Restituisce il numero di reazioni avverse del paziente
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="isGestioneCentrale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function HasReazioniAvverse(codicePaziente As String, isGestioneCentrale As Boolean) As Integer

        Dim countReazioneAvverse As Integer = Me.CountReazioniAvverse(codicePaziente, isGestioneCentrale)

        Return (countReazioneAvverse > 0)

    End Function

#Region " Lotti "

    ''' <summary>
    ''' Restituisce l'elenco dei lotti relativi alla reazione avversa del paziente nella data di effettuazione specificata.
    ''' Il parametro codiceLottoDaEscludere, se valorizzato, indica il codice del lotto da non restituire nella lista.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataEffettuazione"></param>
    ''' <param name="codiceLottoDaEscludere"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCodiciLottiReazioneByDataEffettuazione(codicePaziente As Integer, dataEffettuazione As DateTime, codiceLottoDaEscludere As String) As List(Of String)

        Dim listCodiciLotti As List(Of String) = Me.GenericProvider.VaccinazioniEseguite.GetCodiciLottiReazioneByDataEffettuazione(codicePaziente, dataEffettuazione.Date)

        If listCodiciLotti Is Nothing Then Return Nothing

        listCodiciLotti.Remove(codiceLottoDaEscludere)

        Return listCodiciLotti

    End Function

    ''' <summary>
    ''' Restituisce l'elenco dei lotti relativi alla reazione avversa del paziente nella data di comparsa della reazione specificata.
    ''' Il parametro codiceLottoDaEscludere, se valorizzato, indica il codice del lotto da non restituire nella lista.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataReazione"></param>
    ''' <param name="codiceLottoDaEscludere"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCodiciLottiReazioneByDataReazione(codicePaziente As Integer, dataReazione As DateTime, codiceLottoDaEscludere As String) As List(Of String)

        Dim listCodiciLotti As List(Of String) = Me.GenericProvider.VaccinazioniEseguite.GetCodiciLottiReazioneByDataReazione(codicePaziente, dataReazione)

        If listCodiciLotti Is Nothing Then Return Nothing

        listCodiciLotti.Remove(codiceLottoDaEscludere)

        Return listCodiciLotti

    End Function

    ''' <summary>
    ''' Restituisce i dati di tutti i lotti eseguiti che sono stati eliminati, in base allo stato delle righe del datatable.
    ''' Non considera le eseguite aventi provenienza ACN o RSA
    ''' </summary>
    ''' <param name="dtVaccinazioniEseguite"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDatiLottiEliminati(dtVaccinazioniEseguite As DataTable) As List(Of DatiLottoEliminato)

        Dim listDatiLottiEliminati As New List(Of DatiLottoEliminato)()


        ' TODO [API - RSA]: per la provenienza RSA, bisognerebbe creare un parametro per accendere e spegnere l'aggiornamento delle giacenze (e utilizzarlo anche nell'API).


        ' Vacc. eliminate senza progressivo associazione:
        ' prendo in considerazione solo quelle in stato Deleted con codice del lotto valorizzato e progressivo di associazione non valorizzato
        Dim rowsVaccinazioniDaEliminareAssProgNullo As DataRow() = dtVaccinazioniEseguite.Select(
            String.Format(
                "(not ves_lot_codice is null and ves_lot_codice <> '') and (ves_ass_prog is null or ves_ass_prog = '') and (ves_provenienza is null or (ves_provenienza <> '{0}' and ves_provenienza <> '{1}'))",
                [Enum].GetName(GetType(Enumerators.ProvenienzaVaccinazioni), Enumerators.ProvenienzaVaccinazioni.ACN),
                [Enum].GetName(GetType(Enumerators.ProvenienzaVaccinazioni), Enumerators.ProvenienzaVaccinazioni.RSA)),
            String.Empty,
            DataViewRowState.Deleted)

        ' Lista di elementi distinti (codice lotto, data esecuzione)
        For Each row As DataRow In rowsVaccinazioniDaEliminareAssProgNullo

            Dim codiceLotto As String = row("ves_lot_codice", DataRowVersion.Original).ToString()
            Dim dataEsecuzione As DateTime = row("ves_data_effettuazione", DataRowVersion.Original)
            Dim cnsCodiceEsecuzione As String = row("ves_cns_codice", DataRowVersion.Original).ToString()

            Dim list As List(Of DatiLottoEliminato) =
                listDatiLottiEliminati.Where(Function(l) l.CodiceLotto = codiceLotto And l.DataEsecuzione = dataEsecuzione).ToList()

            If list.IsNullOrEmpty() OrElse list.Count = 0 Then

                Dim statoVaccinazione As String = String.Empty

                If Not row("ves_stato", DataRowVersion.Original) Is Nothing AndAlso Not row("ves_stato", DataRowVersion.Original) Is DBNull.Value Then
                    statoVaccinazione = row("ves_stato", DataRowVersion.Original).ToString()
                End If

                Dim isRegistrato As Boolean = statoVaccinazione = Constants.StatoVaccinazioneEseguita.Registrata

                If Not Settings.VAC_STATI_NON_ESEGUITE.IsNullOrEmpty() Then
                    isRegistrato = Settings.VAC_STATI_NON_ESEGUITE.Contains(statoVaccinazione)
                End If

                listDatiLottiEliminati.Add(New DatiLottoEliminato(codiceLotto, dataEsecuzione, 1, String.Empty, String.Empty, cnsCodiceEsecuzione, String.Empty, isRegistrato))

            End If

        Next

        ' Calcolo numero dosi da ripristinare per ogni lotto eliminato nella data di effettuazione.
        For Each datiLotto As DatiLottoEliminato In listDatiLottiEliminati

            datiLotto.NumeroDosiDaRipristinare =
                CalcolaNumeroDosiDaRipristinare(datiLotto.CodiceLotto, datiLotto.DataEsecuzione, rowsVaccinazioniDaEliminareAssProgNullo)

        Next

        ' Vacc. eliminate con progressivo associazione: 
        ' prendo in considerazione solo quelle in stato Deleted che hanno codice del lotto e progressivo di associazione valorizzati
        Dim rowsVaccinazioniDaEliminareAssProgValorizzato As DataRow() = dtVaccinazioniEseguite.Select(
            String.Format(
                "(not ves_lot_codice is null and ves_lot_codice <> '') and (not ves_ass_prog is null and ves_ass_prog <> '') and (ves_provenienza is null or (ves_provenienza <> '{0}' and ves_provenienza <> '{1}'))",
                [Enum].GetName(GetType(Enumerators.ProvenienzaVaccinazioni), Enumerators.ProvenienzaVaccinazioni.ACN),
                [Enum].GetName(GetType(Enumerators.ProvenienzaVaccinazioni), Enumerators.ProvenienzaVaccinazioni.RSA)),
            String.Empty,
            DataViewRowState.Deleted)

        ' Righe distinte in base al progressivo di associazione. Poichè il progressivo è univoco, ripristino 1 dose per ogni movimento
        Dim listRowDatiLotto As List(Of DataRow) =
            rowsVaccinazioniDaEliminareAssProgValorizzato.Distinct(New DeletedIdAssociazioneLottiComparer()).ToList()

        For Each rowAssProg As DataRow In listRowDatiLotto

            Dim idAssociazione As String = rowAssProg("ves_ass_prog", DataRowVersion.Original).ToString()

            ' Compongo la stringa contenente le vaccinazioni effettuate relative all'id di associazione
            ' Esempio: vac1(dose1), vac2(dose2), ...
            Dim listRowVaccinazioni As List(Of DataRow) =
                rowsVaccinazioniDaEliminareAssProgValorizzato.Where(Function(p) p("ves_ass_prog", DataRowVersion.Original) = idAssociazione).ToList()

            Dim vaccinazioni As String =
                String.Join(";", listRowVaccinazioni.Select(Function(v) String.Format("{0}({1})",
                                                                                      v("ves_vac_codice", DataRowVersion.Original).ToString(),
                                                                                      v("ves_n_richiamo", DataRowVersion.Original).ToString())
                                                            ).ToArray())

            Dim statoVaccinazione As String = String.Empty

            If Not rowAssProg("ves_stato", DataRowVersion.Original) Is Nothing AndAlso Not rowAssProg("ves_stato", DataRowVersion.Original) Is DBNull.Value Then
                statoVaccinazione = rowAssProg("ves_stato", DataRowVersion.Original).ToString()
            End If

            Dim isRegistrato As Boolean = statoVaccinazione = Constants.StatoVaccinazioneEseguita.Registrata

            If Not Settings.VAC_STATI_NON_ESEGUITE.IsNullOrEmpty() Then
                isRegistrato = Settings.VAC_STATI_NON_ESEGUITE.Contains(statoVaccinazione)
            End If

            listDatiLottiEliminati.Add(New DatiLottoEliminato(rowAssProg("ves_lot_codice", DataRowVersion.Original).ToString(),
                                                              Convert.ToDateTime(rowAssProg("ves_data_effettuazione", DataRowVersion.Original)),
                                                              1,
                                                              idAssociazione,
                                                              vaccinazioni,
                                                              rowAssProg("ves_cns_codice", DataRowVersion.Original).ToString(),
                                                              String.Empty,
                                                              isRegistrato))
        Next

        ' Recupero codice consultorio di magazzino per ogni consultorio specificato
        For Each datiLotto As DatiLottoEliminato In listDatiLottiEliminati
            datiLotto.CnsCodiceMagazzino = GenericProvider.Consultori.GetConsultorioMagazzino(datiLotto.CnsCodiceEsecuzione).Codice
        Next

        Return listDatiLottiEliminati

    End Function

#End Region

#End Region

#Region " Salva "

    Public Class VaccinazioniEseguiteSalvaCommand
        Public Property CodicePaziente As Long
        Public Property DtEseguite As DataTable
        Public Property DatiVaccinazioniProgrammateDaEliminare As List(Of VaccinazioneProgrammata)
    End Class

    Public Sub Salva(codicePaziente As Long, dtEseguite As DataTable)

        Dim command As New VaccinazioniEseguiteSalvaCommand() With {
            .CodicePaziente = codicePaziente,
            .DtEseguite = dtEseguite
        }

        Salva(command, Nothing, False, Nothing)

    End Sub

    Public Sub SalvaNoTransactionScope(codicePaziente As Long, dtEseguite As DataTable)

        Dim command As New VaccinazioniEseguiteSalvaCommand() With {
            .CodicePaziente = codicePaziente,
            .DtEseguite = dtEseguite
        }

        SalvaNoTransactionScope(command, Nothing, False, Nothing)

    End Sub

    ''' <summary>
    ''' Salvataggio vaccinazioni eseguite in base al datatable specificato. 
    ''' Vengono valorizzati i dati relativi ai lotti eliminati.
    ''' </summary>
    ''' <param name="command"></param>
    ''' <param name="listDatiLottiEliminati"></param>
    ''' <remarks></remarks>
    Public Function Salva(command As VaccinazioniEseguiteSalvaCommand, ByRef listDatiLottiEliminati As List(Of Entities.DatiLottoEliminato)) As List(Of ReazioneAvversa)

        Return Salva(command, listDatiLottiEliminati, False, Nothing).ReazioneAvversaList

    End Function

    Friend Function Salva(codicePaziente As Long, dtEseguite As DataTable, updateVaccinazioneEseguitaCentraleInConflittoInNeeded As Boolean, vaccinazioneEseguitaCentrale As VaccinazioneEseguitaCentrale) As VaccinazioniEseguiteSalvaResult

        Dim command As New VaccinazioniEseguiteSalvaCommand() With {
            .CodicePaziente = codicePaziente,
            .DtEseguite = dtEseguite
        }

        Return Salva(command, Nothing, updateVaccinazioneEseguitaCentraleInConflittoInNeeded, vaccinazioneEseguitaCentrale)

    End Function

    ''' <summary>
    ''' Salvataggio vaccinazioni eseguite in base al datatable specificato. 
    ''' Vengono valorizzati i dati relativi ai lotti eliminati.
    ''' </summary>
    ''' <param name="command"></param>
    ''' <param name="listDatiLottiEliminati"></param>
    ''' <remarks></remarks>
    Private Function Salva(command As VaccinazioniEseguiteSalvaCommand, ByRef listDatiLottiEliminati As List(Of DatiLottoEliminato), updateVaccinazioneEseguitaCentraleInConflittoInNeeded As Boolean, vaccinazioneEseguitaCentrale As VaccinazioneEseguitaCentrale) As VaccinazioniEseguiteSalvaResult

        Dim vaccinazioniEseguiteSalvaResult As VaccinazioniEseguiteSalvaResult = Nothing

        Using transactionScope As New System.Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, GetReadCommittedTransactionOptions())

            vaccinazioniEseguiteSalvaResult = SalvaNoTransactionScope(command, listDatiLottiEliminati, updateVaccinazioneEseguitaCentraleInConflittoInNeeded, vaccinazioneEseguitaCentrale)

            transactionScope.Complete()

        End Using

        Return vaccinazioniEseguiteSalvaResult

    End Function


    ''' <summary>
    ''' Salvataggio vaccinazioni eseguite in base al datatable specificato. 
    ''' Vengono valorizzati i dati relativi ai lotti eliminati.
    ''' </summary>
    ''' <param name="command"></param>
    ''' <param name="listDatiLottiEliminati"></param>
    ''' <remarks></remarks>
    Private Function SalvaNoTransactionScope(command As VaccinazioniEseguiteSalvaCommand, ByRef listDatiLottiEliminati As List(Of DatiLottoEliminato), updateVaccinazioneEseguitaCentraleInConflittoInNeeded As Boolean, vaccinazioneEseguitaCentrale As VaccinazioneEseguitaCentrale) As VaccinazioniEseguiteSalvaResult

        Dim vaccinazioneEseguitaList As New List(Of VaccinazioneEseguita)()
        Dim vaccinazioneEseguitaScadutaList As New List(Of VaccinazioneEseguita)()
        Dim vaccinazioneEseguitaEliminataList As New List(Of VaccinazioneEseguita)()
        Dim vaccinazioneEseguitaScadutaEliminataList As New List(Of VaccinazioneEseguita)()
        Dim reazioneAvversaList As New List(Of ReazioneAvversa)()
        Dim reazioneAvversaEliminataList As New List(Of ReazioneAvversa)()

        Dim now As DateTime = DateTime.Now

        ' Salvataggio vaccinazioni eseguite e ripristino eseguite eliminate
        Dim dvModified As New DataView(command.DtEseguite, String.Empty, String.Empty, DataViewRowState.ModifiedCurrent)
        Dim dvAdded As New DataView(command.DtEseguite, String.Empty, String.Empty, DataViewRowState.Added)

        Dim dvDeleted As New DataView(command.DtEseguite, String.Empty, String.Empty, DataViewRowState.Deleted)

        ' Aggiunta del progressivo di associazione
        InsertProgressivoAssociazione(command.DtEseguite)

        ' Patch per mantenere stesso progressivo associazione per stesso lotto (no comment...)
        PatchProgressivoAssociazione(command.DtEseguite)

        ' --- Eliminazione Vaccinazioni Programmate --- '
#Region " Eliminazione programmate "
        If Not command.DatiVaccinazioniProgrammateDaEliminare.IsNullOrEmpty() Then

            Using bizVacProg As New BizVaccinazioneProg(GenericProvider, Settings, ContextInfos, LogOptions)

                For Each vacProg As VaccinazioneProgrammata In command.DatiVaccinazioniProgrammateDaEliminare

                    Dim eliminaVacProgCommand As New BizVaccinazioneProg.EliminaVaccinazioniProgrammateCommand()
                    eliminaVacProgCommand.CodicePaziente = command.CodicePaziente
                    eliminaVacProgCommand.CodiceVaccinazioni = {vacProg.CodiceVaccinazione}.AsEnumerable()
                    eliminaVacProgCommand.DataConvocazione = IIf(vacProg.DataConvocazione = Date.MinValue, Nothing, vacProg.DataConvocazione)

                    Dim eliminaProgr As BizVaccinazioneProg.EliminaVaccinazioniProgrammateResult = bizVacProg.EliminaVaccinazioniProgrammate(eliminaVacProgCommand)

                    If eliminaProgr.VaccinazioniProgrammateEliminate.Count > 0 Then

                        Dim testataLog As New Testata(TipiArgomento.VAC_PROGRAMMATE, Operazione.Eliminazione)
                        Dim recordLog As New DataLogStructure.Record()
                        recordLog.Campi.Add(New Campo("VPR_CIC_CODICE", String.Empty, vacProg.CodiceCiclo))
                        recordLog.Campi.Add(New Campo("VPR_N_SEDUTA", String.Empty, IIf(vacProg.NumeroSeduta.HasValue, vacProg.NumeroSeduta.ToString(), String.Empty)))
                        recordLog.Campi.Add(New Campo("VPR_ASS_CODICE", String.Empty, vacProg.CodiceAssociazione))
                        recordLog.Campi.Add(New Campo("VPR_VAC_CODICE", String.Empty, vacProg.CodiceVaccinazione))
                        recordLog.Campi.Add(New Campo("VPR_N_RICHIAMO", String.Empty, IIf(vacProg.NumeroRichiamo.HasValue, vacProg.NumeroRichiamo.ToString(), String.Empty)))
                        recordLog.Campi.Add(New Campo("VPR_CNV_DATA", String.Empty, vacProg.DataConvocazione.ToShortDateString()))
                        testataLog.Records.Add(recordLog)

                        LogBox.WriteData(testataLog)

                    End If

                Next

            End Using

        End If
#End Region

#Region " Righe eliminate "

        ' --- DELETED --- '
        For Each dataRowView As DataRowView In dvDeleted

            Dim row As DataRow = dataRowView.Row

            Dim vaccinazioneEseguitaEliminata As VaccinazioneEseguita = CreateVaccinazioneEseguitaFromDataRow(row)
            Dim reazioneAvversaEliminata As ReazioneAvversa = CreateReazioneAvversaFromDataRow(vaccinazioneEseguitaEliminata, row)

            If Not row("scaduta", DataRowVersion.Original) Is DBNull.Value AndAlso row("scaduta", DataRowVersion.Original) = "S" Then

                ' Cancellazione reazione avversa scaduta
                If Not row("VRA_DATA_REAZIONE", DataRowVersion.Original) Is DBNull.Value Then

                    DeleteReazioneAvversaScaduta(reazioneAvversaEliminata, row)

                    reazioneAvversaEliminataList.Add(reazioneAvversaEliminata)

                End If

                ' Cancellazione vaccinazione scaduta
                DeleteVaccinazioneEseguita(vaccinazioneEseguitaEliminata, True, True)

                vaccinazioneEseguitaScadutaEliminataList.Add(vaccinazioneEseguitaEliminata)

            Else

                ' Cancellazione reazione avversa
                If Not row("VRA_DATA_REAZIONE", DataRowVersion.Original) Is DBNull.Value Then

                    vaccinazioneEseguitaEliminata.IdUtenteEliminazione = Nothing
                    vaccinazioneEseguitaEliminata.DataEliminazione = Nothing

                    DeleteReazioneAvversa(reazioneAvversaEliminata, row)

                    reazioneAvversaEliminataList.Add(reazioneAvversaEliminata)

                End If

                vaccinazioneEseguitaEliminata.IdUtenteEliminazione = Nothing
                vaccinazioneEseguitaEliminata.DataEliminazione = Nothing

                ' Cancellazione vaccinazione eseguita
                DeleteVaccinazioneEseguita(vaccinazioneEseguitaEliminata, False, True)

                vaccinazioneEseguitaEliminataList.Add(vaccinazioneEseguitaEliminata)

            End If

        Next

#End Region

#Region " Righe modificate "

        ' --- MODIFIED ---'
        For Each dataRowView As DataRowView In dvModified

            Dim row As DataRow = dataRowView.Row

            Dim vaccinazioneEseguita As VaccinazioneEseguita = CreateVaccinazioneEseguitaFromDataRow(row)
            vaccinazioneEseguita.ves_data_ultima_variazione = now
            vaccinazioneEseguita.ves_ute_id_ultima_variazione = ContextInfos.IDUtente

            Dim reazioneAvversa As ReazioneAvversa = CreateReazioneAvversaFromDataRow(vaccinazioneEseguita, row)
            If reazioneAvversa IsNot Nothing Then
                reazioneAvversa.DataModifica = vaccinazioneEseguita.ves_data_ultima_variazione
                reazioneAvversa.IdUtenteModifica = vaccinazioneEseguita.ves_ute_id_ultima_variazione
            End If

            If row("scaduta").ToString() = "S" Then

                ' VACCINAZIONE SCADUTA

                'elimimo vaccinazione e reazione no scaduta (se esiste)
                If row("scaduta", DataRowVersion.Original).ToString() <> "S" Then

                    '-- elimino REA AVVERSA --
                    If Not row("VRA_DATA_REAZIONE") Is DBNull.Value Then
                        GenericProvider.VaccinazioniEseguite.DeleteReazioneAvversaById(reazioneAvversa.IdReazioneAvversa)
                    End If

                    '-- elimino VAC ESEGUITA --
                    GenericProvider.VaccinazioniEseguite.DeleteVaccinazioneEseguitaById(vaccinazioneEseguita.ves_id)

                End If

                '-- inserisco / modifico VAC ESEGUITA (SCADUTA) --
                If row("scaduta", DataRowVersion.Original).ToString() = "S" Then

                    ' [Unificazione Ulss]: non devo più controllare la usl di inserimento per le modifiche alle vaccinazioni
                    'If UslGestitaCorrente Is Nothing OrElse UslGestitaCorrente.Codice = vaccinazioneEseguita.ves_usl_inserimento Then

                    UpdateVaccinazioneEseguitaScaduta(vaccinazioneEseguita, row)

                    vaccinazioneEseguitaScadutaList.Add(vaccinazioneEseguita)

                    'End If

                Else

                    InsertVaccinazioneEseguitaScaduta(vaccinazioneEseguita, row)

                    vaccinazioneEseguitaScadutaList.Add(vaccinazioneEseguita)

                End If


                '-- inserisco / modifico REAZIONE (SCADUTA) (se esiste) --
                If Not row("VRA_DATA_REAZIONE") Is DBNull.Value Then

                    'N.B. in questo caso row("vra_data_reazione", DataRowVersion.Original) è valorizzato !!!!
                    If (Not reazioneAvversa.IdReazioneAvversa.HasValue OrElse Not UpdateReazioneAvversaScaduta(reazioneAvversa, row)) Then
                        'reazioneAvversa.FlgInsert = True
                        'reazioneAvversa.FlgScaduta = True
                        InsertReazioneAvversaScaduta(reazioneAvversa, vaccinazioneEseguita.ves_flag_visibilita_vac_centrale, row)
                    End If

                    reazioneAvversaList.Add(reazioneAvversa)

                End If

            Else

                ' VACCINAZIONE NON SCADUTA, OPPURE RIPRISTINATA

                ' Elimina una VAC_SCADUTA se prima era scaduta e non lo è più (No reazione avversa)
                If row("scaduta", DataRowVersion.Original).ToString() = "S" Then

                    ' Reazione avversa associata
                    If Not row("vra_data_reazione") Is DBNull.Value Then
                        Me.GenericProvider.VaccinazioniEseguite.DeleteReazioneAvversaScadutaById(reazioneAvversa.IdReazioneAvversa)
                    End If

                    ' Cancellazione scaduta
                    Me.GenericProvider.VaccinazioniEseguite.DeleteVaccinazioneEseguitaScadutaById(vaccinazioneEseguita.ves_id)

                    vaccinazioneEseguitaList.Add(vaccinazioneEseguita)

                    ' Inserimento eseguita
                    Me.GenericProvider.VaccinazioniEseguite.InsertVaccinazioneEseguita(vaccinazioneEseguita)

                    ' Inserimento reazione avversa
                    If Not row("vra_data_reazione") Is DBNull.Value Then
                        reazioneAvversa.FlgInsert = False
                        Me.InsertReazioneAvversa(reazioneAvversa, vaccinazioneEseguita.ves_flag_visibilita_vac_centrale, row)

                        reazioneAvversaList.Add(reazioneAvversa)

                    End If

                Else

                    ' E' stato eseguito un aggiornamento della vaccinazione eseguita

                    ' aggiorno/aggiungo/elimino reazione
                    If Not row("vra_data_reazione") Is DBNull.Value Then

                        If row("vra_data_reazione", DataRowVersion.Original) Is DBNull.Value Then
                            ' aggiungo reazione
                            reazioneAvversa.FlgInsert = True
                            InsertReazioneAvversa(reazioneAvversa, vaccinazioneEseguita.ves_flag_visibilita_vac_centrale, row)
                        Else
                            UpdateReazioneAvversa(reazioneAvversa, row)
                        End If

                        reazioneAvversaList.Add(reazioneAvversa)

                    Else

                        ' elimino reazione (se c'era)
                        If Not row("VRA_DATA_REAZIONE", DataRowVersion.Original) Is DBNull.Value Then

                            DeleteReazioneAvversa(reazioneAvversa, row)

                            reazioneAvversaEliminataList.Add(reazioneAvversa)

                        End If

                    End If

                    ' -- AGGIORNO VAC EFFETTUATA --
                    ' [Unificazione Ulss]: non devo più controllare la usl di inserimento per le modifiche alle vaccinazioni
                    'If UslGestitaCorrente Is Nothing OrElse UslGestitaCorrente.Codice = vaccinazioneEseguita.ves_usl_inserimento OrElse
                    '   String.IsNullOrEmpty(vaccinazioneEseguita.ves_usl_inserimento) OrElse vaccinazioneEseguita.ves_usl_inserimento Is DBNull.Value Then

                    UpdateVaccinazioneEseguita(vaccinazioneEseguita, row)

                    vaccinazioneEseguitaList.Add(vaccinazioneEseguita)

                    'End If

                End If

            End If

        Next

#End Region

#Region " Righe inserite "

        ' --- ADDED ---'
        For Each dataRowView As DataRowView In dvAdded

            Dim row As DataRow = dataRowView.Row

            Dim vaccinazioneEseguita As VaccinazioneEseguita = CreateVaccinazioneEseguitaFromDataRow(row)
            Dim reazioneAvversa As ReazioneAvversa = CreateReazioneAvversaFromDataRow(vaccinazioneEseguita, row)

            If row("scaduta").ToString() = "S" Then

                '-- aggiungo VAC SCADUTA -- 
                InsertVaccinazioneEseguitaScaduta(vaccinazioneEseguita, row)

                vaccinazioneEseguitaScadutaList.Add(vaccinazioneEseguita)

                '-- aggiungo REAZIONE (SCADUTA) (se esiste) --
                If Not row("VRA_DATA_REAZIONE") Is DBNull.Value Then

                    reazioneAvversa.IdVaccinazioneEseguita = vaccinazioneEseguita.ves_id
                    reazioneAvversa.FlgInsert = True
                    reazioneAvversa.FlgScaduta = True
                    InsertReazioneAvversaScaduta(reazioneAvversa, vaccinazioneEseguita.ves_flag_visibilita_vac_centrale, row)

                    reazioneAvversaList.Add(reazioneAvversa)

                End If

            Else

                '-- aggiungo VAC ESEGUITA -- 
                InsertVaccinazioneEseguita(vaccinazioneEseguita, row)

                vaccinazioneEseguitaList.Add(vaccinazioneEseguita)

                '-- aggiungo REAZIONE (se esiste) --
                If Not row("vra_data_reazione") Is DBNull.Value Then

                    reazioneAvversa.IdVaccinazioneEseguita = vaccinazioneEseguita.ves_id
                    reazioneAvversa.FlgInsert = True

                    InsertReazioneAvversa(reazioneAvversa, vaccinazioneEseguita.ves_flag_visibilita_vac_centrale, row)

                    reazioneAvversaList.Add(reazioneAvversa)

                End If

            End If

        Next

#End Region

        ' Valorizzazione dati dei lotti eliminati
        listDatiLottiEliminati = GetDatiLottiEliminati(command.DtEseguite)

        ' [Unificazione Ulss]: questo pezzo vale solo per le vecchie ulss => Adesso l'unica usl gestita è la 050500, che ha i flag Abilitazione e Consenso ad N
        'If Not UslGestitaCorrente Is Nothing AndAlso UslGestitaCorrente.FlagAbilitazioneDatiVaccinaliCentralizzati Then

        '    Using bizPaziente As New BizPaziente(GenericProviderFactory, Settings, ContextInfos, LogOptions)

        '        Dim aggiornaDatiVaccinaliCentraliCommand As New BizPaziente.AggiornaDatiVaccinaliCentraliCommand()
        '        aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEseguitaEnumerable = vaccinazioneEseguitaList.AsEnumerable()
        '        aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEseguitaScadutaEnumerable = vaccinazioneEseguitaScadutaList.AsEnumerable()
        '        aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEseguitaScadutaEliminataEnumerable = vaccinazioneEseguitaScadutaEliminataList.AsEnumerable()
        '        aggiornaDatiVaccinaliCentraliCommand.VaccinazioneEseguitaEliminataEnumerable = vaccinazioneEseguitaEliminataList.AsEnumerable()
        '        aggiornaDatiVaccinaliCentraliCommand.ReazioneAvversaEnumerable = reazioneAvversaList.AsEnumerable()
        '        aggiornaDatiVaccinaliCentraliCommand.ReazioneAvversaEliminataEnumerable = reazioneAvversaEliminataList.AsEnumerable()
        '        aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale = GenericProvider.Paziente.GetCodiceAusiliario(command.CodicePaziente)

        '        bizPaziente.AggiornaDatiVaccinaliCentrali(aggiornaDatiVaccinaliCentraliCommand)

        '    End Using

        'End If

        Dim vaccinazioniEseguiteSalvaResult As New VaccinazioniEseguiteSalvaResult()
        vaccinazioniEseguiteSalvaResult.ReazioneAvversaList = reazioneAvversaList

        Return vaccinazioniEseguiteSalvaResult

    End Function

    Private Function CalcolaNumeroDosiDaRipristinare(codiceLotto As String, dataEsecuzione As DateTime, rowsVaccinazioniDaEliminareAssProgNullo As DataRow()) As Integer

        Dim listRowLottoStessaData As List(Of DataRow) =
            rowsVaccinazioniDaEliminareAssProgNullo.Where(
                Function(row) row("ves_lot_codice", DataRowVersion.Original) = codiceLotto And
                              row("ves_data_effettuazione", DataRowVersion.Original) = dataEsecuzione).ToList()

        ' Se ci sono più vaccinazioni eseguite nella stessa data e con lo stesso lotto, devo contare quanti lotti sono stati usati
        ' considerando le coppie vaccinazioni-dosi.
        If Not listRowLottoStessaData Is Nothing AndAlso listRowLottoStessaData.Count > 0 Then

            ' Elenco vaccinazioni distinte tra quelle di lotto e data specificati
            Dim listDistinctVaccinazioni As List(Of String) =
                listRowLottoStessaData.Distinct(New DeletedVaccinazioniLottiComparer()).Select(
                    Function(row) row("ves_vac_codice", DataRowVersion.Original).ToString()).ToList()

            ' Per ogni vaccinazione determino quante dosi sono state eseguite
            Dim listDosi As New List(Of Integer)()

            For Each vaccinazione As String In listDistinctVaccinazioni

                Dim filtroVacc As String = vaccinazione

                listDosi.Add(listRowLottoStessaData.Where(Function(row) row("ves_vac_codice", DataRowVersion.Original) = filtroVacc).Count())

            Next

            ' Il massimo tra il numero di escuzioni (non il numero dose) è il numero di dosi da ripristinare per il lotto, nella data di effettuazione.
            If listDosi.Count > 0 Then

                Return listDosi.Max()

            End If

        End If

        Return 0

    End Function

    ''' <summary>
    ''' Salvataggio reazioni avverse
    ''' </summary>
    ''' <param name="dtEseguite"></param>
    ''' <remarks></remarks>
    Public Function SalvaReazioniAvverse(codicePaziente As Int64, dtEseguite As DataTable) As List(Of ReazioneAvversa)

        Dim reazioneAvversaList As New List(Of ReazioneAvversa)
        Dim reazioneAvversaEliminataList As New List(Of ReazioneAvversa)

        Dim j As Int16
        Dim row As DataRow

        Using transactionScope As New System.Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions())

            Dim dv As New DataView(dtEseguite)
            dv.RowStateFilter = DataViewRowState.Deleted

            For j = 0 To dv.Count - 1

                row = dv(j).Row

                Dim vaccinazioneEseguita As VaccinazioneEseguita = Me.CreateVaccinazioneEseguitaFromDataRow(row)
                Dim reazioneAvversa As ReazioneAvversa = Me.CreateReazioneAvversaFromDataRow(vaccinazioneEseguita, row)

                If Not reazioneAvversa Is Nothing Then
                    If row("scaduta", DataRowVersion.Original) = "S" Then

                        Me.DeleteReazioneAvversaScaduta(reazioneAvversa, row)
                    Else

                        Me.DeleteReazioneAvversa(reazioneAvversa, row)

                    End If

                    reazioneAvversaEliminataList.Add(reazioneAvversa)
                End If


            Next

            dv.RowStateFilter = DataViewRowState.ModifiedCurrent

            For j = 0 To dv.Count - 1

                row = dv(j).Row

                Dim vaccinazioneEseguita As VaccinazioneEseguita = Me.CreateVaccinazioneEseguitaFromDataRow(row)
                Dim reazioneAvversa As ReazioneAvversa = Me.CreateReazioneAvversaFromDataRow(vaccinazioneEseguita, row)

                If Not reazioneAvversa Is Nothing Then
                    If row("scaduta", DataRowVersion.Original) = "S" Then

                        UpdateReazioneAvversaScaduta(reazioneAvversa, row)

                    Else

                        UpdateReazioneAvversa(reazioneAvversa, row)

                    End If

                    reazioneAvversaList.Add(reazioneAvversa)
                End If


            Next

            ' [Unificazione Ulss]: non devo più controllare la usl di inserimento per le modifiche alle vaccinazioni
            If Not UslGestitaCorrente Is Nothing AndAlso UslGestitaCorrente.FlagAbilitazioneDatiVaccinaliCentralizzati Then

                Using bizPaziente As New BizPaziente(GenericProviderFactory, Settings, ContextInfos, LogOptions)

                    Dim aggiornaDatiVaccinaliCentraliCommand As New BizPaziente.AggiornaDatiVaccinaliCentraliCommand()
                    aggiornaDatiVaccinaliCentraliCommand.CodicePazienteCentrale = GenericProvider.Paziente.GetCodiceAusiliario(codicePaziente)
                    aggiornaDatiVaccinaliCentraliCommand.ReazioneAvversaEnumerable = reazioneAvversaList.AsEnumerable()
                    aggiornaDatiVaccinaliCentraliCommand.ReazioneAvversaEliminataEnumerable = reazioneAvversaEliminataList.AsEnumerable()

                    bizPaziente.AggiornaDatiVaccinaliCentrali(aggiornaDatiVaccinaliCentraliCommand)

                End Using

            End If

            transactionScope.Complete()

        End Using
        Return reazioneAvversaList
    End Function

    ''' <summary>
    ''' Viene creato il datatable con le sole associazioni, generato a partire dal dt_VacEseguite
    ''' </summary>
    ''' <param name="dt_vacEseguite"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreaDtAssociazioni(dt_vacEseguite As DataTable) As DataTable

        Dim dtAssociazioni As DataTable

        ' Copio la struttura delle vaccinazioni eseguite
        dtAssociazioni = dt_vacEseguite.Clone()
        dtAssociazioni.Columns.Remove("ves_ass_n_dose")
        dtAssociazioni.Columns.Add(New DataColumn("ves_ass_n_dose", GetType(String)))

        For i As Integer = dt_vacEseguite.Rows.Count - 1 To 0 Step -1

            If dt_vacEseguite.Rows(i).RowState <> DataRowState.Deleted Then


                If dt_vacEseguite.Rows(i)("ves_tpa_guid_tipi_pagamento") Is DBNull.Value Then
                    dt_vacEseguite.Rows(i)("ves_tpa_guid_tipi_pagamento") = New Guid().ToByteArray()
                End If

                Dim dv As New DataView(dtAssociazioni)
                dv.RowFilter = String.Format("ves_ass_codice='{0}' and ves_ass_n_dose = '{1}' and ves_data_effettuazione = {2}",
                                             dt_vacEseguite.Rows(i)("ves_ass_codice").ToString(),
                                             dt_vacEseguite.Rows(i)("ves_ass_n_dose").ToString(),
                                             FormatForDataView(dt_vacEseguite.Rows(i)("ves_data_effettuazione")))

                If dv.Count = 0 Then

                    ' Aggiungo la riga (associazione, data) all'arraylist e alla tabella dtAssociazioni.
                    Dim drow As DataRow = dtAssociazioni.NewRow()

                    For j As Integer = 0 To dt_vacEseguite.Columns.Count - 1

                        If (dt_vacEseguite.Columns(j).ColumnName.ToLower = "ves_ass_codice" Or dt_vacEseguite.Columns(j).ColumnName.ToLower = "ves_ass_n_dose") Then
                            drow(dt_vacEseguite.Columns(j).ColumnName) = dt_vacEseguite.Rows(i)(j).ToString()
                        Else
                            drow(dt_vacEseguite.Columns(j).ColumnName) = dt_vacEseguite.Rows(i)(j)
                        End If

                    Next


                    dtAssociazioni.Rows.Add(drow)

                End If

            End If

        Next

        dtAssociazioni.AcceptChanges()

        Return dtAssociazioni

    End Function

    ''' <summary>
    ''' Restituisce il filtro da applicare al datatable delle eseguite, per ottenere le vaccinazioni che compongono l'associazione della riga specificata
    ''' </summary>
    ''' <param name="dataRowView"></param>
    ''' <returns></returns>
    Public Shared Function GetRowFilterAssociazione(dataRowView As DataRowView) As String

        ' N.B. : nella dataViewRow, la colonna ves_ass_n_dose è di tipo Decimal

        Dim codiceAssociazione As String = String.Empty

        If Not IsNullField(dataRowView, "ves_ass_codice") Then
            codiceAssociazione = dataRowView("ves_ass_codice").ToString()
        End If

        Dim codiceLotto As String = String.Empty

        If Not IsNullField(dataRowView, "ves_lot_codice") Then
            codiceLotto = dataRowView("ves_lot_codice").ToString()
        End If

        Dim codiceNomeCommerciale As String = String.Empty

        If Not IsNullField(dataRowView, "ves_noc_codice") Then
            codiceNomeCommerciale = dataRowView("ves_noc_codice").ToString()
        End If

        Dim rowFilter As String = GetRowFilterAssociazioneDataLottoNomeCommerciale(codiceAssociazione, dataRowView("ves_data_effettuazione"), codiceLotto, codiceNomeCommerciale)

        If IsNullField(dataRowView, "ves_ass_n_dose") Then
            rowFilter += " AND VES_ASS_N_DOSE IS NULL "
        Else
            rowFilter += String.Format(" AND VES_ASS_N_DOSE = {0} ", Convert.ToInt32(dataRowView("ves_ass_n_dose")))
        End If

        Return rowFilter

    End Function

    ''' <summary>
    ''' Restituisce il filtro da applicare al datatable delle eseguite, relativamente ai campi specificati
    ''' </summary>
    ''' <param name="codiceAssociazione"></param>
    ''' <param name="dataEffettuazione"></param>
    ''' <param name="codiceLotto"></param>
    ''' <param name="codiceNomeCommerciale"></param>
    ''' <returns></returns>
    Private Shared Function GetRowFilterAssociazioneDataLottoNomeCommerciale(codiceAssociazione As String, dataEffettuazione As Date, codiceLotto As String, codiceNomeCommerciale As String) As String

        Dim rowFilter As New Text.StringBuilder()

        If String.IsNullOrWhiteSpace(codiceAssociazione) Then
            rowFilter.Append("(VES_ASS_CODICE IS NULL OR VES_ASS_CODICE = '') ")
        Else
            rowFilter.AppendFormat("VES_ASS_CODICE = '{0}' ", codiceAssociazione)
        End If

        rowFilter.AppendFormat(" AND VES_DATA_EFFETTUAZIONE = {0} ", FormatForDataView(dataEffettuazione))

        If String.IsNullOrWhiteSpace(codiceLotto) Then
            rowFilter.Append(" AND (VES_LOT_CODICE IS NULL OR VES_LOT_CODICE = '') ")
        Else
            rowFilter.AppendFormat(" AND VES_LOT_CODICE = '{0}' ", codiceLotto)
        End If

        If String.IsNullOrWhiteSpace(codiceNomeCommerciale) Then
            rowFilter.Append(" AND (VES_NOC_CODICE IS NULL OR VES_NOC_CODICE = '') ")
        Else
            rowFilter.AppendFormat(" AND VES_NOC_CODICE = '{0}' ", codiceNomeCommerciale)
        End If

        Return rowFilter.ToString()

    End Function

    Private Shared Function IsNullField(row As DataRow, fieldName As String) As Boolean

        Return row(fieldName) Is Nothing OrElse row(fieldName) Is DBNull.Value OrElse String.IsNullOrWhiteSpace(row(fieldName).ToString())

    End Function

    Private Shared Function IsNullField(dataRowView As DataRowView, fieldName As String) As Boolean

        Return dataRowView(fieldName) Is Nothing OrElse dataRowView(fieldName) Is DBNull.Value OrElse String.IsNullOrWhiteSpace(dataRowView(fieldName).ToString())

    End Function

    ''' <summary>
    ''' Restituisce una stringa formattata per il dataview
    ''' </summary>
    ''' <param name="dateValue"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function FormatForDataView(dateValue As DateTime) As String

        Return String.Format(System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat, "#{0}#", dateValue)

    End Function

    ''' <summary>
    ''' Per ogni vaccinazione presente nel datatable in stato added, viene valutato 
    ''' l'inserimento di un progressivo di associazione
    ''' </summary>
    ''' <param name="dt_vacEseguite"></param>
    ''' <remarks>Non viene aggiunto il progressivo alle vaccinazioni aggiunte da registrazione storico 
    ''' (il progressivo viene calcolato solo per le vaccinazioni aggiunte e con ves_stato nullo).</remarks>
    Public Sub InsertProgressivoAssociazione(ByRef dt_vacEseguite As DataTable)

        Using bizProg As New BizProgressivi(ContextInfos, LogOptions)

            Dim p As String = Nothing
            Dim curAssCodice As String = Nothing
            Dim curAssDose As String = Nothing
            Dim curData As DateTime = Nothing

            Dim dv As New DataView(dt_vacEseguite)

            dv.RowStateFilter = DataViewRowState.Added
            dv.RowFilter = "ves_stato is null and ves_ass_codice is not null and ves_ass_n_dose is not null "
            dv.Sort = "ves_ass_codice, ves_ass_n_dose, ves_data_effettuazione"

            For i As Integer = 0 To dv.Count - 1

                If dv(i)("ves_ass_codice") <> curAssCodice OrElse
                   dv(i)("ves_ass_n_dose") <> curAssDose OrElse
                   dv(i)("ves_data_effettuazione") <> curData Then

                    p = bizProg.CalcolaProgressivo(Constants.TipoProgressivo.AssociazioneProg, True)

                    curAssCodice = dv(i)("ves_ass_codice")
                    curAssDose = dv(i)("ves_ass_n_dose")
                    curData = dv(i)("ves_data_effettuazione")

                End If

                dv(i)("ves_ass_prog") = p

            Next

        End Using

    End Sub

    ''' <summary>
    ''' Per ogni vaccinazione presente nel datatable in stato added e per cui è stato calcolato un progressivo di associazione, 
    ''' viene controllato che il progressivo sia lo stesso per vaccinazioni (aggiunte) aventi stesso lotto.
    ''' </summary>
    ''' <param name="dt_vacEseguite"></param>
    ''' <remarks>Questa pezza è stata fatta perchè sono stati segnalati casi sgaffi di stesse vaccinazioni (stessa data, associazione, dose, lotto) con ass_prog diverso
    ''' che hanno generato più movimenti di magazzino anzichè uno solo.</remarks>
    Public Sub PatchProgressivoAssociazione(ByRef dt_vacEseguite As DataTable)

        Dim curLotCodice As String = String.Empty
        Dim curAssProg As String = String.Empty

        Dim dv As New DataView(dt_vacEseguite)

        dv.RowStateFilter = DataViewRowState.Added
        dv.RowFilter = "ves_stato is null and ves_lot_codice is not null and ves_ass_prog is not null"

        ' Ordinando in questo modo, ad ogni lotto assegno l'ass_prog minore.
        dv.Sort = "ves_lot_codice, ves_ass_prog"

        For i As Integer = 0 To dv.Count - 1

            If dv(i)("ves_lot_codice").ToString() <> curLotCodice Then

                curLotCodice = dv(i)("ves_lot_codice").ToString()
                curAssProg = dv(i)("ves_ass_prog").ToString()

            ElseIf dv(i)("ves_ass_prog").ToString() <> curAssProg Then

                dv(i)("ves_ass_prog") = curAssProg

            End If

        Next

    End Sub

    Public Function SalvaVaccinazioneEseguita(salvaVaccinazioneEseguitaCommand As SalvaVaccinazioneEseguitaCommand) As SalvaVaccinazioneEseguitaResult

        Dim success As Boolean = True
        Dim conflittoRisolto As Boolean = True
        Dim resultMessageList As New List(Of BizResult.ResultMessage)()

        Dim idVaccinazioneEseguitaEsistente As Int64? = Nothing

        Dim canSaveVaccinazioneEseguitaCommand As New CanSaveVaccinazioneEseguitaCommand()
        canSaveVaccinazioneEseguitaCommand.VaccinazioneEseguita = salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita
        canSaveVaccinazioneEseguitaCommand.IsVaccinazioneEseguitaScaduta = salvaVaccinazioneEseguitaCommand.IsVaccinazioneEseguitaScaduta
        canSaveVaccinazioneEseguitaCommand.Operation = salvaVaccinazioneEseguitaCommand.Operation

        Dim canSaveVaccinazioneEseguitaResult As CanSaveVaccinazioneEseguitaResult = CanSaveVaccinazioneEseguita(canSaveVaccinazioneEseguitaCommand)
        resultMessageList.AddRange(canSaveVaccinazioneEseguitaResult.Messages)
        success = canSaveVaccinazioneEseguitaResult.Success

        If success Then

            Select Case salvaVaccinazioneEseguitaCommand.Operation

                Case SalvaCommandOperation.Insert

                    InsertVaccinazioneEseguita(salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita, salvaVaccinazioneEseguitaCommand.IsVaccinazioneEseguitaScaduta)

                Case SalvaCommandOperation.Update

                    UpdateVaccinazioneEseguita(salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita, salvaVaccinazioneEseguitaCommand.IsVaccinazioneEseguitaScaduta)

                Case SalvaCommandOperation.Delete

                    DeleteVaccinazioneEseguita(salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita, salvaVaccinazioneEseguitaCommand.IsVaccinazioneEseguitaScaduta, False)

                Case Else
                    Throw New NotImplementedException()

            End Select

        Else

            'Gestione dei conflitti
            If canSaveVaccinazioneEseguitaResult.IdVaccinazioneEseguitaEsistente.HasValue Then

                If salvaVaccinazioneEseguitaCommand.UpdateVaccinazioneEseguitaCentraleInConflittoIfNeeded Then
                    conflittoRisolto = UpdateConflittoVaccinazioneEseguitaCentraleIfNeeded(canSaveVaccinazioneEseguitaResult.IdVaccinazioneEseguitaEsistente.Value,
                        salvaVaccinazioneEseguitaCommand.VaccinazioneEseguitaOriginale, canSaveVaccinazioneEseguitaResult.VaccinazioneEseguitaEsistente,
                        salvaVaccinazioneEseguitaCommand.VaccinazioneEseguitaCentrale)
                End If

            End If

        End If

        Return New SalvaVaccinazioneEseguitaResult(success, conflittoRisolto, resultMessageList.AsEnumerable())

    End Function

    Public Function InsertReazioneAvversa(reazioneAvversa As ReazioneAvversa, flagVisibilitaCentrale As String) As Boolean

        SetReazioneAvversaAddedContextInfosIfNeeded(reazioneAvversa, flagVisibilitaCentrale)

        Dim inserted As Boolean = GenericProvider.VaccinazioniEseguite.InsertReazioneAvversa(reazioneAvversa)

        Return inserted

    End Function

    Public Function InsertReazioneAvversaScaduta(reazioneAvversaScaduta As ReazioneAvversa, flagVisibilitaCentrale As String) As Boolean

        SetReazioneAvversaAddedContextInfosIfNeeded(reazioneAvversaScaduta, flagVisibilitaCentrale)

        Dim inserted As Boolean = GenericProvider.VaccinazioniEseguite.InsertReazioneAvversaScaduta(reazioneAvversaScaduta)

        Return inserted

    End Function

    Public Function InsertLinkReazLogInvio(idReazioneAvversa As Long, idLogInvio As Long) As Boolean
        Return GenericProvider.VaccinazioniEseguite.InsertLinkReazLogInvio(idReazioneAvversa, idLogInvio) > 0
    End Function

    Public Function UpdateReazioneAvversa(reazioneAvversa As ReazioneAvversa) As Boolean

        SetReazioneAvversaModifiedContextInfosIfNeeded(reazioneAvversa)

        Dim updated As Boolean = GenericProvider.VaccinazioniEseguite.UpdateReazioneAvversa(reazioneAvversa)

        Return updated

    End Function

    Public Function UpdateReazioneAvversaIdScheda(reazioneAvversa As ReazioneAvversa) As Boolean

        SetReazioneAvversaModifiedContextInfosIfNeeded(reazioneAvversa)

        Dim updated As Boolean = GenericProvider.VaccinazioniEseguite.UpdateReazioneAvversaIdscheda(reazioneAvversa)

        Return updated

    End Function

    Public Function UpdateReazioneAvversaScaduta(reazioneAvversaScaduta As ReazioneAvversa) As Boolean

        SetReazioneAvversaModifiedContextInfosIfNeeded(reazioneAvversaScaduta)

        Dim updated As Boolean = GenericProvider.VaccinazioniEseguite.UpdateReazioneAvversaScaduta(reazioneAvversaScaduta)

        Return updated

    End Function

    Private Sub InsertVaccinazioneEseguitaScadutaEliminata(vaccinazioneEseguitaScadutaEliminata As VaccinazioneEseguita)

        SetVaccinazioneEseguitaDeletedContextInfosIfNeeded(vaccinazioneEseguitaScadutaEliminata)

        GenericProvider.VaccinazioniEseguite.InsertVaccinazioneEseguitaScadutaEliminata(vaccinazioneEseguitaScadutaEliminata)

    End Sub

    Private Sub InsertVaccinazioneEseguitaEliminata(vaccinazioneEseguitaEliminata As VaccinazioneEseguita)

        SetVaccinazioneEseguitaDeletedContextInfosIfNeeded(vaccinazioneEseguitaEliminata)

        GenericProvider.VaccinazioniEseguite.InsertVaccinazioneEseguitaEliminata(vaccinazioneEseguitaEliminata)

    End Sub

    Public Function DeleteReazioneAvversa(reazioneAvversaEliminata As ReazioneAvversa) As Boolean

        Dim deleted As Boolean = GenericProvider.VaccinazioniEseguite.DeleteReazioneAvversaById(reazioneAvversaEliminata.IdReazioneAvversa)

        InsertReazioneAvversaEliminata(reazioneAvversaEliminata)

        Return deleted

    End Function

    Public Sub InsertReazioneAvversaEliminata(reazioneAvversaEliminata As ReazioneAvversa)

        SetReazioneAvversaDeletedContextInfosIfNeeded(reazioneAvversaEliminata)

        GenericProvider.VaccinazioniEseguite.InsertReazioneAvversaEliminata(reazioneAvversaEliminata)

    End Sub

    Public Function DeleteReazioneAvversaScaduta(reazioneAvversaScadutaEliminata As ReazioneAvversa) As Boolean

        Dim deleted As Boolean = GenericProvider.VaccinazioniEseguite.DeleteReazioneAvversaScadutaById(reazioneAvversaScadutaEliminata.IdReazioneAvversa)

        InsertReazioneAvversaScadutaEliminata(reazioneAvversaScadutaEliminata)

        Return deleted

    End Function

    Public Sub InsertReazioneAvversaScadutaEliminata(reazioneAvversaScadutaEliminata As ReazioneAvversa)

        SetReazioneAvversaDeletedContextInfosIfNeeded(reazioneAvversaScadutaEliminata)

        GenericProvider.VaccinazioniEseguite.InsertReazioneAvversaScadutaEliminata(reazioneAvversaScadutaEliminata)

    End Sub

    ''' <summary>
    ''' Imposta il flag di visibilità, id utente e data di modifica per le vaccinazioni specificate
    ''' </summary>
    ''' <param name="listIdVaccinazioniEseguite"></param>
    ''' <param name="listIdVaccinazioniScadute"></param>
    ''' <param name="flagVisibilita"></param>
    ''' <returns></returns>
    Public Function UpdateFlagVisibilita(codicePaziente As Long, listIdVaccinazioniEseguite As List(Of Long), listIdVaccinazioniScadute As List(Of Long), flagVisibilita As String) As Integer

        Dim dataModifica As Date = Date.Now

        Dim vacEseguiteOriginali As List(Of VaccinazioneEseguita) = Nothing
        If listIdVaccinazioniEseguite.Count > 0 Then vacEseguiteOriginali = GenericProvider.VaccinazioniEseguite.GetVaccinazioniEseguiteById(listIdVaccinazioniEseguite)

        Dim countEseguiteModificate As Integer = 0
        For Each idVaccinazioneEseguita As Long In listIdVaccinazioniEseguite
            countEseguiteModificate += GenericProvider.VaccinazioniEseguite.UpdateFlagVisibilitaEseguite(idVaccinazioneEseguita, flagVisibilita, ContextInfos.IDUtente, dataModifica)
        Next

        Dim vacScaduteOriginali As List(Of VaccinazioneEseguita) = Nothing
        If listIdVaccinazioniScadute.Count > 0 Then vacScaduteOriginali = GenericProvider.VaccinazioniEseguite.GetVaccinazioniEseguiteById(listIdVaccinazioniScadute)

        Dim countScaduteModificate As Integer = 0
        For Each idVaccinazioneScaduta As Long In listIdVaccinazioniScadute
            countScaduteModificate += GenericProvider.VaccinazioniEseguite.UpdateFlagVisibilitaScadute(idVaccinazioneScaduta, flagVisibilita, ContextInfos.IDUtente, dataModifica)
        Next

        If countEseguiteModificate + countScaduteModificate > 0 Then

            Dim bizLogOptions As BizLogOptions

            If LogOptions Is Nothing Then
                bizLogOptions = New BizLogOptions(TipiArgomento.VAC_ESEGUITE, False)
            Else
                bizLogOptions = New BizLogOptions(LogOptions.CodiceArgomento, LogOptions.Automatico)
            End If

            Dim testataLog As New Testata(bizLogOptions.CodiceArgomento, Operazione.Modifica, codicePaziente, bizLogOptions.Automatico)

            If countEseguiteModificate > 0 Then

                For Each vac As VaccinazioneEseguita In vacEseguiteOriginali

                    Dim recordLog As New DataLogStructure.Record()

                    recordLog.Campi.Add(New Campo("Modificato flag visibilita eseguite (VES_ID):", vac.ves_id.ToString()))

                    If String.IsNullOrWhiteSpace(vac.ves_flag_visibilita_vac_centrale) Then
                        recordLog.Campi.Add(New Campo("VES_FLAG_VISIBILITA", String.Empty, flagVisibilita))
                    Else
                        recordLog.Campi.Add(New Campo("VES_FLAG_VISIBILITA", vac.ves_flag_visibilita_vac_centrale, flagVisibilita))
                    End If

                    If vac.ves_ute_id_ultima_variazione.HasValue Then
                        recordLog.Campi.Add(New Campo("VES_UTE_ID_ULTIMA_VARIAZIONE", vac.ves_ute_id_ultima_variazione.Value.ToString(), ContextInfos.IDUtente.ToString()))
                    Else
                        recordLog.Campi.Add(New Campo("VES_UTE_ID_ULTIMA_VARIAZIONE", String.Empty, ContextInfos.IDUtente.ToString()))
                    End If

                    If vac.ves_data_ultima_variazione.HasValue Then
                        recordLog.Campi.Add(New Campo("VES_DATA_ULTIMA_VARIAZIONE", vac.ves_data_ultima_variazione.Value, dataModifica))
                    Else
                        recordLog.Campi.Add(New Campo("VES_DATA_ULTIMA_VARIAZIONE", String.Empty, dataModifica))
                    End If

                    testataLog.Records.Add(recordLog)

                Next

            End If

            If countScaduteModificate > 0 Then

                For Each vacScaduta As VaccinazioneEseguita In vacScaduteOriginali

                    Dim recordLog As New DataLogStructure.Record()

                    recordLog.Campi.Add(New Campo("Modificato flag visibilita scadute (VSC_ID):", vacScaduta.ves_id.ToString()))

                    If String.IsNullOrWhiteSpace(vacScaduta.ves_flag_visibilita_vac_centrale) Then
                        recordLog.Campi.Add(New Campo("VSC_FLAG_VISIBILITA", String.Empty, flagVisibilita))
                    Else
                        recordLog.Campi.Add(New Campo("VSC_FLAG_VISIBILITA", vacScaduta.ves_flag_visibilita_vac_centrale, flagVisibilita))
                    End If

                    If vacScaduta.ves_ute_id_ultima_variazione.HasValue Then
                        recordLog.Campi.Add(New Campo("VSC_UTE_ID_ULTIMA_VARIAZIONE", vacScaduta.ves_ute_id_ultima_variazione.Value.ToString(), ContextInfos.IDUtente.ToString()))
                    Else
                        recordLog.Campi.Add(New Campo("VSC_UTE_ID_ULTIMA_VARIAZIONE", String.Empty, ContextInfos.IDUtente.ToString()))
                    End If

                    If vacScaduta.ves_data_ultima_variazione.HasValue Then
                        recordLog.Campi.Add(New Campo("VSC_DATA_ULTIMA_VARIAZIONE", vacScaduta.ves_data_ultima_variazione.Value, dataModifica))
                    Else
                        recordLog.Campi.Add(New Campo("VSC_DATA_ULTIMA_VARIAZIONE", String.Empty, dataModifica))
                    End If

                    testataLog.Records.Add(recordLog)

                Next

            End If

            If testataLog.Records.Count > 0 Then
                LogBox.WriteData(testataLog)
            End If

        End If

        Return countEseguiteModificate + countScaduteModificate

    End Function

#End Region

#Region " Controlli "

    ''' <summary>
    ''' Controllo [VAC_CODICE,VAC_N_RICHIAMO] non presente in vac eseguite
    ''' Controllo [VAC_CODICE,DATA_EFFETTUAZIONE] non presente in vac eseguite
    ''' Controllo [VAC_CODICE,VAC_N_RICHIAMO,DATA_EFFETTUAZIONE] non presente in vac scadute
    ''' Controllo sulla sequenzialità dei richiami di vaccinazione inseriti, a partire dal richiamo max in vac eseguite
    ''' Controllo [VAC_CODICE,VAC_N_RICHIAMO,DATA_EFFETTUAZIONE] non incoerenti con altre [VAC_CODICE,VAC_N_RICHIAMO,DATA_EFFETTUAZIONE]
    ''' </summary>
    ''' <param name="el"></param>
    ''' <param name="dt"></param>
    ''' <param name="controlList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckDatiVaccinazione(el As Int16, dt As DataTable, controlList As ControlloEsecuzione) As Boolean

        Dim result As Boolean = False
        Dim dv1 As DataView
        Dim drow As DataRow = dt.Rows(el)
        Dim curVacSeduta As Int16 = drow("ves_n_richiamo")
        Dim curVacCodice As String = drow("ves_vac_codice")
        Dim curVacDescrizione As String = drow("vac_descrizione")
        Dim curData As Date = drow("ves_data_effettuazione")

        ' per codice vaccinazione, controllo sulla data di esecuzione
        dv1 = New DataView(dt)

        dv1.RowFilter = String.Format("ves_vac_codice = '{0}' and ves_data_effettuazione = {1} and scaduta = 'N'",
                                      curVacCodice,
                                      FormatForDataView(curData))
        If dv1.Count > 1 Then

            controlList.Add(New ControlloEsecuzioneItem(curVacDescrizione, curVacSeduta, True, ControlloEsecuzioneItemType.StessaData))
            result = True

        End If

        ' per codice vaccinazione, controllo sulla dose di esecuzione
        dv1 = New DataView(dt)

        dv1.RowFilter = String.Format("ves_vac_codice = '{0}' and ves_n_richiamo = {1} and scaduta = 'N'",
                                      curVacCodice,
                                      curVacSeduta)
        If dv1.Count > 1 Then

            controlList.Add(New ControlloEsecuzioneItem(curVacDescrizione, curVacSeduta, True, ControlloEsecuzioneItemType.StessaDoseVaccinazione))
            result = True

        End If

        ' per codice vaccinazione, controllo esistenza scaduta uguale
        dv1 = New DataView(dt)

        dv1.RowFilter = String.Format("ves_vac_codice = '{0}' and ves_n_richiamo = {1} and ves_data_effettuazione = {2} and scaduta = 'S'",
                                      curVacCodice,
                                      curVacSeduta,
                                      FormatForDataView(curData))

        If dv1.GetEnumerator.MoveNext() Then

            controlList.Add(New ControlloEsecuzioneItem(curVacDescrizione, curVacSeduta, True, ControlloEsecuzioneItemType.ScadutaEsistente))
            Return True

        End If

        ' controllo coerenza date e richiamo vaccinazione
        dv1 = New DataView(dt)

        dv1.RowFilter = String.Format("ves_vac_codice = '{0}' and ves_n_richiamo < {1} and ves_data_effettuazione > {2} and scaduta = 'N'",
                                      curVacCodice,
                                      curVacSeduta,
                                      FormatForDataView(curData))

        If dv1.GetEnumerator.MoveNext() Then

            controlList.Add(New ControlloEsecuzioneItem(curVacDescrizione, curVacSeduta, True, ControlloEsecuzioneItemType.DataDopoSedSuc))
            result = True

        End If

        ' controllo coerenza date e richiamo vaccinazione
        dv1 = New DataView(dt)

        dv1.RowFilter = String.Format("ves_vac_codice = '{0}' and ves_n_richiamo > {1} and ves_data_effettuazione < {2}  and scaduta = 'N'",
                                      curVacCodice,
                                      curVacSeduta,
                                      FormatForDataView(curData))

        If dv1.GetEnumerator.MoveNext Then

            controlList.Add(New ControlloEsecuzioneItem(curVacDescrizione, curVacSeduta, True, ControlloEsecuzioneItemType.DataPrimaSedPrec))
            result = True

        End If

        ' controllo sequenza dosi vaccinazione
        Dim obj As Object = dt.Compute("max(ves_n_richiamo)", String.Format("ves_vac_codice = '{0}' and ves_id is not null and ves_n_richiamo is not null and scaduta = 'N'", curVacCodice))

        Dim maxVacRichiamoEseguito As Integer = 1

        If IsNumeric(obj) Then
            maxVacRichiamoEseguito = System.Convert.ToInt32(obj)
        End If

        For j As Integer = maxVacRichiamoEseguito To curVacSeduta - 1

            dv1 = New DataView(dt)

            dv1.RowFilter = String.Format("ves_vac_codice = '{0}' and ves_n_richiamo = {1}", curVacCodice, j)

            If Not dv1.GetEnumerator.MoveNext Then

                controlList.Add(New ControlloEsecuzioneItem(curVacDescrizione,
                                                            curVacSeduta,
                                                            False,
                                                            ControlloEsecuzioneItemType.VacconazioneDoseNonSuccessiva))
                result = True

                Exit For

            End If

        Next

        Return result

    End Function

    Private Function ExistsStessaScaduta(codiceVaccinazione As String, doseVaccinazione As Integer, dataEsecuzione As DateTime, dtEseguite As DataTable) As Boolean

        Dim dv As New DataView(dtEseguite)
        dv.RowFilter = String.Format("ves_vac_codice = '{0}' and ves_n_richiamo = {1} and ves_data_effettuazione = {2} and scaduta = 'S'",
                                      codiceVaccinazione, doseVaccinazione.ToString(), FormatForDataView(dataEsecuzione))

        If dv.GetEnumerator().MoveNext() Then
            Return True
        End If

        Return False

    End Function

    ''' <summary>
    ''' Controllo [ves_sii_codice,ves_vii_codice] non valorizzato (controllo parametrizzato)
    ''' </summary>
    ''' <param name="el"></param>
    ''' <param name="dt"></param>
    ''' <param name="controlList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckDatiInoculo(el As Int16, dt As DataTable, controlList As ControlloEsecuzione) As Boolean

        Dim result As Boolean = False
        Dim drow As DataRow = dt.Rows(el)
        Dim curVacSeduta As Int16 = drow("ves_n_richiamo")
        Dim curVacDescrizione As String = drow("vac_descrizione")

        ' Controllo dei dati obbligatori
        If Me.Settings.CHECK_SITO_INOCULO AndAlso String.IsNullOrEmpty(dt.Rows(el)("ves_sii_codice").ToString()) Then

            controlList.Add(New ControlloEsecuzioneItem(curVacDescrizione, curVacSeduta, True, ControlloEsecuzioneItemType.SitoInoculoAssente))
            result = True

        End If

        If Me.Settings.CHECK_VIA_SOMMINISTRAZIONE AndAlso String.IsNullOrEmpty(dt.Rows(el)("ves_vii_codice").ToString()) Then

            controlList.Add(New ControlloEsecuzioneItem(curVacDescrizione, curVacSeduta, True, ControlloEsecuzioneItemType.ViaSomministrazioneAssente))
            result = True

        End If

        Return result

    End Function

    ''' <summary>
    ''' Controllo [ASS_CODICE,ASS_N_DOSE] non presente in vac eseguite
    ''' Controllo sulla sequenzialità delle dosi di associazione inserite, a partire dalla dose max in vac eseguite
    ''' Controllo [ASS_CODICE,ASS_N_DOSE,DATA_EFFETTUAZIONE] non incoerenti con altre [ASS_CODICE,ASS_N_DOSE,DATA_EFFETTUAZIONE]
    ''' </summary>
    ''' <param name="el"></param>
    ''' <param name="dt"></param>
    ''' <param name="controlList"></param>
    ''' <returns></returns>
    ''' <remarks>Se il campo ASS_CODICE è valorizzato, DEVE essere valorizzato anche ASS_N_DOSE!</remarks>
    Public Function CheckDatiAssociazione(el As Int16, dt As DataTable, controlList As ControlloEsecuzione) As Boolean

        Dim result As Boolean = False

        Dim dv1 As DataView
        Dim drow As DataRow = dt.Rows(el)

        Dim curAssDose As Int16 = drow("ves_ass_n_dose")
        Dim curAssCodice As String = drow("ves_ass_codice")
        Dim curData As Date = drow("ves_data_effettuazione")

        ' per codice associazione, controllo sulla dose di esecuzione
        dv1 = New DataView(dt)

        dv1.RowFilter = String.Format("ves_ass_codice = '{0}' and ves_ass_n_dose = {1} and scaduta = 'N'",
                                      curAssCodice,
                                      curAssDose)
        If dv1.Count > 1 Then

            controlList.Add(New ControlloEsecuzioneItem(curAssCodice, curAssDose, True, ControlloEsecuzioneItemType.StessaDoseAssociazione))
            result = True

        End If

        ' controllo coerenza date e richiamo associazione
        dv1 = New DataView(dt)

        dv1.RowFilter = String.Format("ves_ass_codice = '{0}' and ves_ass_n_dose < {1} and ves_data_effettuazione > {2} and scaduta = 'N'",
                                      curAssCodice,
                                      curAssDose,
                                      FormatForDataView(curData))

        If dv1.GetEnumerator.MoveNext Then

            controlList.Add(New ControlloEsecuzioneItem(curAssCodice, curAssDose, True, ControlloEsecuzioneItemType.DataDopoSedSuc))
            result = True

        End If

        ' controllo coerenza date e richiamo associazione
        dv1 = New DataView(dt)

        dv1.RowFilter = String.Format("ves_ass_codice = '{0}' and ves_ass_n_dose > {1} and ves_data_effettuazione < {2}  and scaduta = 'N'",
                                      curAssCodice,
                                      curAssDose,
                                      FormatForDataView(curData))

        If dv1.GetEnumerator.MoveNext Then

            controlList.Add(New ControlloEsecuzioneItem(curAssCodice, curAssDose, True, ControlloEsecuzioneItemType.DataPrimaSedPrec))
            result = True

        End If

        ' controllo sequenza dosi associazione
        Dim obj As Object = dt.Compute("max(ves_ass_n_dose)",
                                       String.Format("ves_ass_codice = '{0}' and ves_id is not null and ves_ass_n_dose is not null and scaduta = 'N'",
                                                     curAssCodice))
        If IsNumeric(obj) Then

            Dim maxAssDoseEseguito As Integer = System.Convert.ToInt32(obj)

            For j As Integer = maxAssDoseEseguito To curAssDose - 1

                dv1 = New DataView(dt)
                dv1.RowFilter = String.Format("ves_ass_codice = '{0}' and ves_ass_n_dose = {1}", curAssCodice, j)

                If Not dv1.GetEnumerator.MoveNext Then

                    controlList.Add(New ControlloEsecuzioneItem(curAssCodice, curAssDose, False, ControlloEsecuzioneItemType.AssociazioneDoseNonSuccessiva))
                    result = True

                    Exit For

                End If

            Next

        End If

        Return result

    End Function

    ''' <summary>
    ''' Controllo data non futura
    ''' Controllo data non precedente data di nascita
    ''' </summary>
    ''' <param name="el"></param>
    ''' <param name="dt"></param>
    ''' <param name="controlList"></param>
    ''' <param name="pazCodice"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckDataEffettuazione(el As Int16, dt As DataTable, controlList As ControlloEsecuzione, pazCodice As Integer) As Boolean

        Dim result As Boolean = False
        Dim curVacSeduta As Int16 = dt.Rows(el)("ves_n_richiamo")
        Dim curVacDescrizione As String = dt.Rows(el)("vac_descrizione")
        Dim curData As Date = dt.Rows(el)("ves_data_effettuazione")
        Dim dataNascita As Date = Me.GenericProvider.Paziente.GetDataNascita(pazCodice)

        If curData < dataNascita Then

            controlList.Add(New ControlloEsecuzioneItem(curVacDescrizione, curVacSeduta, True, ControlloEsecuzioneItemType.DataPrimaNascita))
            result = True

        End If

        If curData > Date.Now Then

            controlList.Add(New ControlloEsecuzioneItem(curVacDescrizione, curVacSeduta, True, ControlloEsecuzioneItemType.DataFutura))
            result = True

        End If

        Return result

    End Function

    ''' <summary>
    ''' Controllo data non futura e data non precedente data di nascita del paziente
    ''' </summary>
    ''' <param name="dose"></param>
    ''' <param name="descrizione"></param>
    ''' <param name="dataEsecuzione"></param>
    ''' <param name="controlList"></param>
    ''' <param name="dataNascitaPaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CheckDataEffettuazione(dose As Integer, descrizione As String, dataEsecuzione As DateTime, controlList As ControlloEsecuzione, dataNascitaPaziente As DateTime) As Boolean

        Dim result As Boolean = False

        If dataEsecuzione < dataNascitaPaziente Then
            controlList.Add(New ControlloEsecuzioneItem(descrizione, dose, True, ControlloEsecuzioneItemType.DataPrimaNascita))
            result = True
        End If

        If dataEsecuzione > Date.Now Then
            controlList.Add(New ControlloEsecuzioneItem(descrizione, dose, True, ControlloEsecuzioneItemType.DataFutura))
            result = True
        End If

        Return result

    End Function

    ''' <summary>
    ''' Controllo non bloccante su [VAC_CODICE,VAC_N_RICHIAMO] non presente in vac programmate
    ''' </summary>
    ''' <param name="el"></param>
    ''' <param name="dt"></param>
    ''' <param name="controlList"></param>
    ''' <param name="pazCodice"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckVaccinazioneProgrammata(el As Int16, dt As DataTable, controlList As ControlloEsecuzione, pazCodice As Integer) As Boolean

        Dim result As Boolean = False

        Dim drow As DataRow = dt.Rows(el)

        Dim curNumeroRichiamo As Int16 = drow("ves_n_richiamo")
        Dim curVacCodice As String = drow("ves_vac_codice")
        Dim curVacDescrizione As String = drow("vac_descrizione")

        Dim exists As Boolean = Me.GenericProvider.VaccinazioneProg.ExistsVaccinazioneProgrammataByRichiamo(pazCodice,
                                                                                                            curVacCodice,
                                                                                                            curNumeroRichiamo)
        If exists Then

            controlList.Add(New ControlloEsecuzioneItem(curVacDescrizione, curNumeroRichiamo, True, ControlloEsecuzioneItemType.VaccinazioneProgrammataEsistente))
            result = True

        End If

        Return result

    End Function

    ''' <summary>
    ''' Restituisce true se nel dataview è presente almeno una vaccinazione nella data specificata
    ''' </summary>
    ''' <param name="codiceVaccinazione"></param>
    ''' <param name="dataEsecuzione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ExistsVaccinazioneStessaData(codiceVaccinazione As String, dataEsecuzione As DateTime, dvEseguite As DataView) As Boolean

        For i As Integer = 0 To dvEseguite.Count - 1

            Dim vac As String = dvEseguite(i)("ves_vac_codice").ToString()
            Dim data As DateTime = Convert.ToDateTime(dvEseguite(i)("ves_data_effettuazione"))

            If vac = codiceVaccinazione AndAlso data = dataEsecuzione Then
                Return True
            End If

        Next

        Return False

    End Function

    ''' <summary>
    ''' Restituisce true se esiste la vaccinazione eseguita indicata
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataEsecuzione"></param>
    ''' <param name="codiceVaccinazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ExistsVaccinazioneEseguita(codicePaziente As Integer, dataEsecuzione As DateTime, codiceVaccinazione As String) As Boolean

        Return GenericProvider.VaccinazioniEseguite.EsisteVaccinazioneEseguita(codicePaziente, dataEsecuzione, codiceVaccinazione)

    End Function

    ''' <summary>
    ''' Restituisce true se esiste la vaccinazione scaduta indicata
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataEsecuzione"></param>
    ''' <param name="codiceVaccinazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ExistsVaccinazioneScaduta(codicePaziente As Integer, dataEsecuzione As DateTime, codiceVaccinazione As String) As Boolean

        Return GenericProvider.VaccinazioniEseguite.EsisteVaccinazioneScaduta(codicePaziente, dataEsecuzione, codiceVaccinazione)

    End Function

    ''' <summary>
    ''' Restituisce true se esiste l'associazione eseguita indicata, per il paziente, nella data specificata
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataEsecuzione"></param>
    ''' <param name="codiceAssociazione"></param>
    ''' <returns></returns>
    Public Function ExistsAssociazioneEseguita(codicePaziente As Long, dataEsecuzione As Date, codiceAssociazione As String) As Boolean

        Return GenericProvider.VaccinazioniEseguite.EsisteAssociazioneEseguita(codicePaziente, dataEsecuzione, codiceAssociazione)

    End Function

#End Region

#Region " Centrale "

#Region " Friend "

    Friend Function AggiornaVaccinazioneEseguitaCentrale(codicePazienteCentrale As String, vaccinazioneEseguita As VaccinazioneEseguita, reazioneAvversa As ReazioneAvversa,
                                                         tipoVaccinazioneEseguitaCentrale As String, tipoReazioneAvversaCentrale As String, codicePazienteCentraleAlias As String,
                                                         isMerge As Boolean, isRisoluzioneConflitto As Boolean, isVisibilitaModificata As Boolean) As VaccinazioneEseguitaCentrale

        Dim codiceUslCorrente As String = GetCodiceUslByIdApplicazione(ContextInfos.IDApplicazione)

        Dim vaccinazioneEseguitaCentrale As VaccinazioneEseguitaCentrale = Nothing

        Dim isTipoVaccinazioneEseguitaEliminata As Boolean = False

        Dim idVaccinazioneEseguita As Int64

        If vaccinazioneEseguita Is Nothing Then
            idVaccinazioneEseguita = reazioneAvversa.IdVaccinazioneEseguita
        Else
            idVaccinazioneEseguita = vaccinazioneEseguita.ves_id
        End If

        vaccinazioneEseguitaCentrale = GenericProviderCentrale.VaccinazioneEseguitaCentrale.GetVaccinazioneEseguitaCentraleByIdLocale(idVaccinazioneEseguita, codiceUslCorrente)

        Dim existsVaccinazioneEseguitaCentrale As Boolean = (Not vaccinazioneEseguitaCentrale Is Nothing)

        If Not vaccinazioneEseguita Is Nothing Then

            If Not existsVaccinazioneEseguitaCentrale Then

                vaccinazioneEseguitaCentrale = New VaccinazioneEseguitaCentrale()
                vaccinazioneEseguitaCentrale.IdVaccinazioneEseguita = vaccinazioneEseguita.ves_id
                vaccinazioneEseguitaCentrale.CodicePaziente = vaccinazioneEseguita.paz_codice
                vaccinazioneEseguitaCentrale.CodicePazienteCentrale = codicePazienteCentrale
                vaccinazioneEseguitaCentrale.CodiceUslVaccinazioneEseguita = vaccinazioneEseguita.ves_usl_inserimento
                vaccinazioneEseguitaCentrale.DataInserimentoVaccinazioneEseguita = vaccinazioneEseguita.ves_data_registrazione
                vaccinazioneEseguitaCentrale.IdUtenteInserimentoVaccinazioneEseguita = vaccinazioneEseguita.ves_ute_id

                If String.IsNullOrEmpty(tipoVaccinazioneEseguitaCentrale) Then

                    Select Case vaccinazioneEseguita.ves_stato
                        Case "R"
                            tipoVaccinazioneEseguitaCentrale = Constants.TipoVaccinazioneEseguitaCentrale.Registrata
                        Case "E"
                            tipoVaccinazioneEseguitaCentrale = Constants.TipoVaccinazioneEseguitaCentrale.Recuperata
                        Case ""
                            tipoVaccinazioneEseguitaCentrale = Constants.TipoVaccinazioneEseguitaCentrale.Programmata
                        Case Else
                            Throw New NotImplementedException()
                    End Select

                End If

            Else

                If String.IsNullOrEmpty(tipoVaccinazioneEseguitaCentrale) Then

                    If vaccinazioneEseguita.ves_ute_id_ripristino.HasValue Then
                        tipoVaccinazioneEseguitaCentrale = Constants.TipoVaccinazioneEseguitaCentrale.Ripristinata
                    Else
                        tipoVaccinazioneEseguitaCentrale = vaccinazioneEseguitaCentrale.TipoVaccinazioneEseguitaCentrale
                    End If

                End If

                'VISIBILITA (REVOCA)
                Dim isRevocaVisibilitaCentrale As Boolean = False
                Dim isVisibilitaCentrale As Boolean = Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(vaccinazioneEseguita.ves_flag_visibilita_vac_centrale)

                If Not String.IsNullOrEmpty(vaccinazioneEseguitaCentrale.FlagVisibilitaCentrale) Then

                    Dim wasVisibilitaCentrale As Boolean = Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(vaccinazioneEseguitaCentrale.FlagVisibilitaCentrale)

                    If isVisibilitaCentrale AndAlso Not wasVisibilitaCentrale Then

                        vaccinazioneEseguitaCentrale.DataRevocaVisibilita = Nothing

                    ElseIf Not isVisibilitaCentrale AndAlso wasVisibilitaCentrale Then

                        vaccinazioneEseguitaCentrale.DataRevocaVisibilita = DateTime.Now

                        isRevocaVisibilitaCentrale = True

                    End If

                End If


                'CONFLITTI
                Dim idConflittoRimanente As Int64? = Nothing

                If isRisoluzioneConflitto Then

                    'TODO [ DATI VAC CENTRALE ]: IdUtenteRisoluzioneConflitto = IdUtenteUltimaOperazione ?!?
                    vaccinazioneEseguitaCentrale.IdUtenteRisoluzioneConflitto = Me.ContextInfos.IDUtente
                    vaccinazioneEseguitaCentrale.DataRisoluzioneConflitto = DateTime.Now

                ElseIf (isRevocaVisibilitaCentrale OrElse isTipoVaccinazioneEseguitaEliminata) AndAlso
                    vaccinazioneEseguitaCentrale.IdConflitto.HasValue AndAlso Not vaccinazioneEseguitaCentrale.DataRisoluzioneConflitto.HasValue Then

                    If isRevocaVisibilitaCentrale OrElse
                        (Me.GenericProviderCentrale.VaccinazioneEseguitaCentrale.CountVaccinazioneEseguitaCentraleInConflittoByIdConflitto(
                            vaccinazioneEseguitaCentrale.IdConflitto) = 2) Then

                        idConflittoRimanente = vaccinazioneEseguitaCentrale.IdConflitto

                    End If

                    vaccinazioneEseguitaCentrale.IdConflitto = Nothing

                End If

                If idConflittoRimanente.HasValue Then

                    Me.GenericProviderCentrale.VaccinazioneEseguitaCentrale.UpdateIdConflittoVaccinazioneEseguitaCentraleByIdConflitto(
                        idConflittoRimanente.Value, Nothing)

                End If

            End If


            'MERGE
            If isMerge Then

                Select Case tipoVaccinazioneEseguitaCentrale

                    Case Constants.TipoVaccinazioneEseguitaCentrale.Eliminata,
                         Constants.TipoVaccinazioneEseguitaCentrale.ScadutaEliminata
                        '-- nothing

                    Case Else

                        'TODO [ DATI VAC CENTRALE ]: idUtenteAlias = IdUtenteUltimaOperazione ?!?
                        vaccinazioneEseguitaCentrale.MergeInfoCentrale.CodicePazienteCentraleAlias = codicePazienteCentraleAlias
                        vaccinazioneEseguitaCentrale.MergeInfoCentrale.IdUtenteAlias = Me.ContextInfos.IDUtente
                        vaccinazioneEseguitaCentrale.MergeInfoCentrale.DataAlias = DateTime.Now

                End Select

            End If


            Select Case tipoVaccinazioneEseguitaCentrale

                Case Constants.TipoVaccinazioneEseguitaCentrale.Programmata,
                     Constants.TipoVaccinazioneEseguitaCentrale.Recuperata,
                     Constants.TipoVaccinazioneEseguitaCentrale.Registrata

                    vaccinazioneEseguitaCentrale.IdUtenteModificaVaccinazioneEseguita = vaccinazioneEseguita.ves_ute_id_ultima_variazione
                    vaccinazioneEseguitaCentrale.DataModificaVaccinazioneEseguita = vaccinazioneEseguita.ves_data_ultima_variazione

                Case Constants.TipoVaccinazioneEseguitaCentrale.Scaduta

                    vaccinazioneEseguitaCentrale.IdUtenteScadenzaVaccinazioneEseguita = vaccinazioneEseguita.ves_ute_id_scadenza
                    vaccinazioneEseguitaCentrale.DataScadenzaVaccinazioneEseguita = vaccinazioneEseguita.ves_data_scadenza
                    vaccinazioneEseguitaCentrale.CodiceUslScadenzaVaccinazioneEseguita = vaccinazioneEseguita.ves_usl_scadenza

                Case Constants.TipoVaccinazioneEseguitaCentrale.Ripristinata

                    vaccinazioneEseguitaCentrale.IdUtenteRipristinoVaccinazioneEseguita = vaccinazioneEseguita.ves_ute_id_ripristino
                    vaccinazioneEseguitaCentrale.DataRipristinoVaccinazioneEseguita = vaccinazioneEseguita.ves_data_ripristino

                Case Constants.TipoVaccinazioneEseguitaCentrale.Eliminata,
                     Constants.TipoVaccinazioneEseguitaCentrale.ScadutaEliminata

                    vaccinazioneEseguitaCentrale.IdUtenteEliminazioneVaccinazioneEseguita = vaccinazioneEseguita.IdUtenteEliminazione
                    vaccinazioneEseguitaCentrale.DataEliminazioneVaccinazioneEseguita = vaccinazioneEseguita.DataEliminazione

                    isTipoVaccinazioneEseguitaEliminata = True

                Case Else
                    Throw New NotImplementedException(String.Format("Tipo Vaccinazione Eseguita Centrale non implementata: {0}", vaccinazioneEseguitaCentrale.TipoVaccinazioneEseguitaCentrale))

            End Select

            vaccinazioneEseguitaCentrale.FlagVisibilitaCentrale = vaccinazioneEseguita.ves_flag_visibilita_vac_centrale

            vaccinazioneEseguitaCentrale.TipoVaccinazioneEseguitaCentrale = tipoVaccinazioneEseguitaCentrale

            If Not isVisibilitaModificata Then
                vaccinazioneEseguitaCentrale.CodiceUslUltimaOperazioneVaccinazioneEseguita = GetCodiceUslByIdApplicazione(ContextInfos.IDApplicazione)
            End If

            vaccinazioneEseguitaCentrale.IdUtenteUltimaOperazioneVaccinazioneEseguita = ContextInfos.IDUtente

        End If

        If Not reazioneAvversa Is Nothing Then

            Dim existsReazioneAvversaCentrale As Boolean = (vaccinazioneEseguitaCentrale.IdReazioneAvversa.HasValue AndAlso
                    vaccinazioneEseguitaCentrale.TipoReazioneAvversaCentrale <> Constants.TipoReazioneAvversaCentrale.Eliminata)

            If Not existsReazioneAvversaCentrale OrElse
               (Not Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(vaccinazioneEseguitaCentrale.FlagVisibilitaCentrale) AndAlso
               (vaccinazioneEseguitaCentrale.IdReazioneAvversa <> reazioneAvversa.IdReazioneAvversa OrElse
               vaccinazioneEseguitaCentrale.CodiceUslReazioneAvversa <> reazioneAvversa.CodiceUslInserimento)) Then

                vaccinazioneEseguitaCentrale.IdReazioneAvversa = reazioneAvversa.IdReazioneAvversa
                vaccinazioneEseguitaCentrale.CodiceUslReazioneAvversa = reazioneAvversa.CodiceUslInserimento

                vaccinazioneEseguitaCentrale.DataInserimentoReazioneAvversa = reazioneAvversa.DataCompilazione
                vaccinazioneEseguitaCentrale.IdUtenteInserimentoReazioneAvversa = reazioneAvversa.IdUtenteCompilazione

                vaccinazioneEseguitaCentrale.IdUtenteModificaReazioneAvversa = Nothing
                vaccinazioneEseguitaCentrale.DataModificaReazioneAvversa = Nothing

                vaccinazioneEseguitaCentrale.IdUtenteEliminazioneReazioneAvversa = Nothing
                vaccinazioneEseguitaCentrale.DataEliminazioneReazioneAvversa = Nothing

            End If

            Select Case tipoReazioneAvversaCentrale

                Case Constants.TipoReazioneAvversaCentrale.Nessuno

                    vaccinazioneEseguitaCentrale.IdUtenteModificaReazioneAvversa = reazioneAvversa.IdUtenteModifica
                    vaccinazioneEseguitaCentrale.DataModificaReazioneAvversa = reazioneAvversa.DataModifica

                Case Constants.TipoReazioneAvversaCentrale.Eliminata

                    vaccinazioneEseguitaCentrale.IdUtenteEliminazioneReazioneAvversa = reazioneAvversa.IdUtenteCompilazione
                    vaccinazioneEseguitaCentrale.DataEliminazioneReazioneAvversa = reazioneAvversa.DataEliminazione

                Case Else

                    Throw New NotImplementedException(String.Format("Tipo Reazione Avversa Centrale non implementata: {0}", vaccinazioneEseguitaCentrale.TipoReazioneAvversaCentrale))

            End Select

            If Not isVisibilitaModificata Then
                vaccinazioneEseguitaCentrale.TipoReazioneAvversaCentrale = tipoReazioneAvversaCentrale
                vaccinazioneEseguitaCentrale.CodiceUslUltimaOperazioneReazioneAvversa = codiceUslCorrente
            End If

            vaccinazioneEseguitaCentrale.IdUtenteUltimaOperazioneReazioneAvversa = ContextInfos.IDUtente

        End If

        If existsVaccinazioneEseguitaCentrale Then
            GenericProviderCentrale.VaccinazioneEseguitaCentrale.UpdateVaccinazioneEseguitaCentrale(vaccinazioneEseguitaCentrale)
        Else
            GenericProviderCentrale.VaccinazioneEseguitaCentrale.InsertVaccinazioneEseguitaCentrale(vaccinazioneEseguitaCentrale)
        End If


        Dim vaccinazioneEseguitaCentraleDistribuita As VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita =
            GenericProviderCentrale.VaccinazioneEseguitaCentrale.GetVaccinazioneEseguitaCentraleDistribuitaByIdCentrale(vaccinazioneEseguitaCentrale.Id, codiceUslCorrente)

        AggiornaVaccinazioneEseguitaCentraleDistribuita(vaccinazioneEseguita, reazioneAvversa, vaccinazioneEseguitaCentrale, vaccinazioneEseguitaCentraleDistribuita)

        Return vaccinazioneEseguitaCentrale

    End Function

    Friend Function AcquisisciVaccinazioneEseguitaCentrale(codicePazienteDestinazione As Int64,
                                                           vaccinazioneEseguitaAcquisizioneInfos As VaccinazioneEseguitaCentraleInfo(),
                                                           tipoVaccinazioneEseguitaCentrale As String,
                                                           isVisibilitaModificata As Boolean) As AcquisisciVaccinazioneEseguitaCentraleResult

        Dim vaccinazioniEseguiteInseriteList As New List(Of VaccinazioneEseguita)()
        Dim vaccinazioniEseguiteScaduteInseriteList As New List(Of VaccinazioneEseguita)()

        Dim vaccinazioniEseguiteNonInseriteList As New List(Of VaccinazioneEseguita)()
        Dim vaccinazioniEseguiteScaduteNonInseriteList As New List(Of VaccinazioneEseguita)()

        For Each vaccinazioneEseguitaAcquisizioneInfo As VaccinazioneEseguitaCentraleInfo In vaccinazioneEseguitaAcquisizioneInfos

            Dim operazioneLogDatiVaccinaliCentrali As DataLogStructure.Operazione? = Nothing
            Dim statoLogDatiVaccinaliCentrali As Enumerators.StatoLogDatiVaccinaliCentrali? = Nothing

            Dim reazioneAvversaDestinazione As ReazioneAvversa = Nothing

            Dim salvaVaccinazioneEseguitaCommand As New BizVaccinazioniEseguite.SalvaVaccinazioneEseguitaCommand()
            salvaVaccinazioneEseguitaCommand.UpdateVaccinazioneEseguitaCentraleInConflittoIfNeeded = True
            salvaVaccinazioneEseguitaCommand.IsVaccinazioneEseguitaScaduta = (tipoVaccinazioneEseguitaCentrale = Constants.TipoVaccinazioneEseguitaCentrale.Scaduta OrElse
                                                                              tipoVaccinazioneEseguitaCentrale = Constants.TipoVaccinazioneEseguitaCentrale.ScadutaEliminata)

            salvaVaccinazioneEseguitaCommand.VaccinazioneEseguitaCentrale = vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguitaCentrale

            salvaVaccinazioneEseguitaCommand.VaccinazioneEseguitaOriginale = vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguita.Clone()

            Dim vaccinazioneEseguitaCentraleDistributaDestinazione As VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita =
                Me.GenericProviderCentrale.VaccinazioneEseguitaCentrale.GetVaccinazioneEseguitaCentraleDistribuitaByIdCentrale(
                    vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguitaCentrale.Id, UslGestitaCorrente.Codice)

            Dim isVisibilitaCentrale As Boolean =
                Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguita.ves_flag_visibilita_vac_centrale)

            If (tipoVaccinazioneEseguitaCentrale <> Constants.TipoVaccinazioneEseguitaCentrale.Eliminata AndAlso
                tipoVaccinazioneEseguitaCentrale <> Constants.TipoVaccinazioneEseguitaCentrale.ScadutaEliminata) Then


                If salvaVaccinazioneEseguitaCommand.VaccinazioneEseguitaCentrale.CodiceUslUltimaOperazioneVaccinazioneEseguita =
                        salvaVaccinazioneEseguitaCommand.VaccinazioneEseguitaCentrale.CodiceUslVaccinazioneEseguita Then

                    salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita = Me.CreateVaccinazioneEseguitaDestinazioneByOrigine(
                        vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguita, salvaVaccinazioneEseguitaCommand.VaccinazioneEseguitaCentrale)

                ElseIf salvaVaccinazioneEseguitaCommand.VaccinazioneEseguitaCentrale.CodiceUslVaccinazioneEseguita = UslGestitaCorrente.Codice Then

                    Select Case tipoVaccinazioneEseguitaCentrale

                        Case Constants.TipoVaccinazioneEseguitaCentrale.Ripristinata

                            salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita = Me.GenericProvider.VaccinazioniEseguite.GetVaccinazioneEseguitaScadutaById(vaccinazioneEseguitaCentraleDistributaDestinazione.IdVaccinazioneEseguita)

                            If salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita Is Nothing Then
                                salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita = Me.GenericProvider.VaccinazioniEseguite.GetVaccinazioneEseguitaById(vaccinazioneEseguitaCentraleDistributaDestinazione.IdVaccinazioneEseguita)
                            End If

                            salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita.ves_data_ripristino = DateTime.Now
                            salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita.ves_ute_id_ripristino = vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguita.ves_ute_id_ripristino

                        Case Constants.TipoVaccinazioneEseguitaCentrale.Programmata,
                            Constants.TipoVaccinazioneEseguitaCentrale.Registrata,
                            Constants.TipoVaccinazioneEseguitaCentrale.Recuperata,
                            Constants.TipoVaccinazioneEseguitaCentrale.Scaduta

                            salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita = Me.GenericProvider.VaccinazioniEseguite.GetVaccinazioneEseguitaById(vaccinazioneEseguitaCentraleDistributaDestinazione.IdVaccinazioneEseguita)

                            If tipoVaccinazioneEseguitaCentrale = Constants.TipoVaccinazioneEseguitaCentrale.Scaduta Then

                                If salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita Is Nothing Then
                                    salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita = Me.GenericProvider.VaccinazioniEseguite.GetVaccinazioneEseguitaScadutaById(vaccinazioneEseguitaCentraleDistributaDestinazione.IdVaccinazioneEseguita)
                                End If

                                salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita.ves_data_scadenza = DateTime.Now
                                salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita.ves_ute_id_scadenza = vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguita.ves_ute_id_scadenza
                                salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita.ves_usl_scadenza = vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguita.ves_usl_scadenza

                            End If

                        Case Else
                            Throw New NotSupportedException()

                    End Select

                    salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita.ves_flag_visibilita_vac_centrale = vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguita.ves_flag_visibilita_vac_centrale

                Else

                    salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita = vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguita.Clone()

                End If

                salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita.paz_codice = codicePazienteDestinazione

                Dim saveVaccinazioneEseguitaResult As SalvaVaccinazioneEseguitaResult = Nothing

                If vaccinazioneEseguitaCentraleDistributaDestinazione Is Nothing Then

                    salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita.ves_data_registrazione = Nothing
                    salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita.ves_id = Nothing

                    salvaVaccinazioneEseguitaCommand.Operation = Biz.BizClass.SalvaCommandOperation.Insert

                    saveVaccinazioneEseguitaResult = Me.SalvaVaccinazioneEseguita(salvaVaccinazioneEseguitaCommand)

                    If saveVaccinazioneEseguitaResult.Success Then

                        If Not vaccinazioneEseguitaAcquisizioneInfo.ReazioneAvversa Is Nothing Then

                            reazioneAvversaDestinazione = Me.AcquisisciReazioneAvversaCentrale(codicePazienteDestinazione,
                                                                                               salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita.ves_id,
                                                                                               vaccinazioneEseguitaAcquisizioneInfo.ReazioneAvversa,
                                                                                               salvaVaccinazioneEseguitaCommand.VaccinazioneEseguitaCentrale,
                                                                                               vaccinazioneEseguitaCentraleDistributaDestinazione)

                        End If


                        statoLogDatiVaccinaliCentrali = Enumerators.StatoLogDatiVaccinaliCentrali.Success

                        If salvaVaccinazioneEseguitaCommand.IsVaccinazioneEseguitaScaduta Then
                            vaccinazioniEseguiteScaduteInseriteList.Add(salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita)
                        Else
                            vaccinazioniEseguiteInseriteList.Add(salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita)
                        End If

                    Else

                        statoLogDatiVaccinaliCentrali = Enumerators.StatoLogDatiVaccinaliCentrali.Error

                        'Fix: solo nel caso in cui non sia stato auto-risolto il conflitto devo aggiungere  la vaccinazione eseguita alle liste.
                        'Non entrando in questo if, non viene impostato male il flag di stato di acquisizione del paziente nel caso dell'autorisoluzione
                        If (Not saveVaccinazioneEseguitaResult.ConflittoRisolto) Then

                            If salvaVaccinazioneEseguitaCommand.IsVaccinazioneEseguitaScaduta Then
                                vaccinazioniEseguiteScaduteNonInseriteList.Add(salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita)
                            Else
                                vaccinazioniEseguiteNonInseriteList.Add(salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita)
                            End If

                        End If

                    End If

                    operazioneLogDatiVaccinaliCentrali = DataLogStructure.Operazione.Inserimento

                Else

                    Dim vaccinazioneEsistenteEliminata As VaccinazioneEseguita = Nothing

                    salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita.ves_id = vaccinazioneEseguitaCentraleDistributaDestinazione.IdVaccinazioneEseguita

                    If tipoVaccinazioneEseguitaCentrale = Constants.TipoVaccinazioneEseguitaCentrale.Scaduta Then

                        vaccinazioneEsistenteEliminata = Me.GenericProvider.VaccinazioniEseguite.GetVaccinazioneEseguitaByIdIfExists(vaccinazioneEseguitaCentraleDistributaDestinazione.IdVaccinazioneEseguita)

                    ElseIf tipoVaccinazioneEseguitaCentrale = Constants.TipoVaccinazioneEseguitaCentrale.Ripristinata Then

                        vaccinazioneEsistenteEliminata = Me.GenericProvider.VaccinazioniEseguite.GetVaccinazioneEseguitaScadutaByIdIfExists(vaccinazioneEseguitaCentraleDistributaDestinazione.IdVaccinazioneEseguita)

                    Else

                        salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita.ves_data_ultima_variazione = Nothing

                    End If

                    If Not vaccinazioneEsistenteEliminata Is Nothing Then

                        salvaVaccinazioneEseguitaCommand.Operation = Biz.BizClass.SalvaCommandOperation.Insert

                        operazioneLogDatiVaccinaliCentrali = DataLogStructure.Operazione.Inserimento

                    Else

                        salvaVaccinazioneEseguitaCommand.Operation = Biz.BizClass.SalvaCommandOperation.Update

                        operazioneLogDatiVaccinaliCentrali = DataLogStructure.Operazione.Modifica

                    End If

                    saveVaccinazioneEseguitaResult = Me.SalvaVaccinazioneEseguita(salvaVaccinazioneEseguitaCommand)

                    If saveVaccinazioneEseguitaResult.Success Then

                        If isVisibilitaCentrale Then

                            If Not vaccinazioneEseguitaAcquisizioneInfo.ReazioneAvversa Is Nothing Then

                                Me.AcquisisciReazioneAvversaCentrale(codicePazienteDestinazione,
                                                                     salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita.ves_id,
                                                                     vaccinazioneEseguitaAcquisizioneInfo.ReazioneAvversa,
                                                                     salvaVaccinazioneEseguitaCommand.VaccinazioneEseguitaCentrale,
                                                                     vaccinazioneEseguitaCentraleDistributaDestinazione)
                            End If

                        End If

                        If Not vaccinazioneEsistenteEliminata Is Nothing Then

                            If salvaVaccinazioneEseguitaCommand.IsVaccinazioneEseguitaScaduta Then
                                Me.GenericProvider.VaccinazioniEseguite.DeleteVaccinazioneEseguitaById(vaccinazioneEsistenteEliminata.ves_id)
                            Else
                                Me.GenericProvider.VaccinazioniEseguite.DeleteVaccinazioneEseguitaScadutaById(vaccinazioneEsistenteEliminata.ves_id)
                            End If

                        End If

                        statoLogDatiVaccinaliCentrali = Enumerators.StatoLogDatiVaccinaliCentrali.Success

                    Else

                        statoLogDatiVaccinaliCentrali = Enumerators.StatoLogDatiVaccinaliCentrali.Error

                    End If

                End If

            Else

                If Not vaccinazioneEseguitaCentraleDistributaDestinazione Is Nothing Then

                    Select Case tipoVaccinazioneEseguitaCentrale

                        Case Constants.TipoVaccinazioneEseguitaCentrale.ScadutaEliminata

                            salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita = Me.GenericProvider.VaccinazioniEseguite.GetVaccinazioneEseguitaScadutaByIdIfExists(vaccinazioneEseguitaCentraleDistributaDestinazione.IdVaccinazioneEseguita)

                            If salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita Is Nothing Then

                                salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita = Me.GenericProvider.VaccinazioniEseguite.GetVaccinazioneEseguitaByIdIfExists(vaccinazioneEseguitaCentraleDistributaDestinazione.IdVaccinazioneEseguita)

                                If Not salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita Is Nothing Then
                                    salvaVaccinazioneEseguitaCommand.IsVaccinazioneEseguitaScaduta = False
                                End If

                            End If

                            If Not salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita Is Nothing Then
                                salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita.IdUtenteEliminazione = vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguita.IdUtenteEliminazione
                            End If

                        Case Constants.TipoVaccinazioneEseguitaCentrale.Eliminata

                            salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita = Me.GenericProvider.VaccinazioniEseguite.GetVaccinazioneEseguitaByIdIfExists(vaccinazioneEseguitaCentraleDistributaDestinazione.IdVaccinazioneEseguita)

                            If salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita Is Nothing Then

                                salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita = Me.GenericProvider.VaccinazioniEseguite.GetVaccinazioneEseguitaScadutaByIdIfExists(vaccinazioneEseguitaCentraleDistributaDestinazione.IdVaccinazioneEseguita)

                                If Not salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita Is Nothing Then
                                    salvaVaccinazioneEseguitaCommand.IsVaccinazioneEseguitaScaduta = True
                                End If

                            End If

                            If Not salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita Is Nothing Then
                                salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita.IdUtenteEliminazione = salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita.IdUtenteEliminazione
                            End If

                    End Select


                    If Not salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita Is Nothing Then

                        If Not vaccinazioneEseguitaAcquisizioneInfo.ReazioneAvversa Is Nothing Then

                            reazioneAvversaDestinazione = Me.AcquisisciReazioneAvversaCentrale(codicePazienteDestinazione,
                                                                                               salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita.ves_id,
                                                                                               vaccinazioneEseguitaAcquisizioneInfo.ReazioneAvversa,
                                                                                               salvaVaccinazioneEseguitaCommand.VaccinazioneEseguitaCentrale,
                                                                                               vaccinazioneEseguitaCentraleDistributaDestinazione)

                        End If

                        salvaVaccinazioneEseguitaCommand.Operation = Biz.BizClass.SalvaCommandOperation.Delete

                        Me.SalvaVaccinazioneEseguita(salvaVaccinazioneEseguitaCommand)

                        operazioneLogDatiVaccinaliCentrali = DataLogStructure.Operazione.Eliminazione
                        statoLogDatiVaccinaliCentrali = Enumerators.StatoLogDatiVaccinaliCentrali.Success

                    End If

                End If

            End If

            If operazioneLogDatiVaccinaliCentrali.HasValue Then

                If statoLogDatiVaccinaliCentrali <> Enumerators.StatoLogDatiVaccinaliCentrali.Error Then

                    Me.AggiornaVaccinazioneEseguitaCentraleDistribuita(salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita,
                                                                       reazioneAvversaDestinazione,
                                                                       salvaVaccinazioneEseguitaCommand.VaccinazioneEseguitaCentrale,
                                                                       vaccinazioneEseguitaCentraleDistributaDestinazione)

                End If

                Me.InsertLogAcquisizioneVaccinazioneEseguitaCentraleDistribuita(salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita,
                                                                                salvaVaccinazioneEseguitaCommand.VaccinazioneEseguitaCentrale,
                                                                                operazioneLogDatiVaccinaliCentrali,
                                                                                statoLogDatiVaccinaliCentrali,
                                                                                isVisibilitaModificata)

            End If

        Next

        If vaccinazioniEseguiteInseriteList.Count > 0 Then

            'ELIMINAZIONE PROGRAMMATE
            Dim eliminaVaccinazioneProgrammataCommand As New Biz.BizVaccinazioneProg.EliminaVaccinazioniProgrammateCommand()

            Using bizVaccinazioneProg As New BizVaccinazioneProg(Me.GenericProvider, Me.Settings, Me.ContextInfos, Me.LogOptions)

                Dim eliminaVaccinazioniProgrammateCommand As New BizVaccinazioneProg.EliminaVaccinazioniProgrammateCommand
                eliminaVaccinazioneProgrammataCommand.CodicePaziente = codicePazienteDestinazione
                eliminaVaccinazioneProgrammataCommand.CodiceVaccinazioni = vaccinazioniEseguiteInseriteList.Select(Function(ve) ve.ves_vac_codice).AsEnumerable()

                Dim eliminaVaccinazioniProgrammateResult As BizVaccinazioneProg.EliminaVaccinazioniProgrammateResult =
                    bizVaccinazioneProg.EliminaVaccinazioniProgrammate(eliminaVaccinazioneProgrammataCommand)

                Dim vacLst As List(Of VaccinazioneProgrammata) = eliminaVaccinazioniProgrammateResult.VaccinazioniProgrammateEliminate

                For Each vac As VaccinazioneProgrammata In vacLst

                    Dim codiceVaccinazione As String = vac.CodiceVaccinazione
                    Dim vaccinazioneEseguita As VaccinazioneEseguita = vaccinazioniEseguiteInseriteList.First(Function(ve) ve.ves_vac_codice = codiceVaccinazione)

                    Dim vaccinazioneEseguitaCentrale As VaccinazioneEseguitaCentrale =
                        Me.GenericProviderCentrale.VaccinazioneEseguitaCentrale.GetVaccinazioneEseguitaCentraleByIdLocale(
                            vaccinazioneEseguita.ves_id, UslGestitaCorrente.Codice)

                    'LOG ELIMINAZIONE PROGRAMMATA
                    Dim logDatiVaccinali As New LogDatiVaccinali()

                    logDatiVaccinali.Paziente.Paz_Codice = codicePazienteDestinazione
                    logDatiVaccinali.Operazione = DataLogStructure.Operazione.Eliminazione
                    logDatiVaccinali.Argomento.Codice = Log.DataLogStructure.TipiArgomento.VAC_PROGRAMMATE
                    logDatiVaccinali.Stato = Enumerators.StatoLogDatiVaccinaliCentrali.Success
                    logDatiVaccinali.Usl.Codice = vaccinazioneEseguitaCentrale.CodiceUslUltimaOperazioneVaccinazioneEseguita
                    logDatiVaccinali.Utente.Id = vaccinazioneEseguitaCentrale.IdUtenteUltimaOperazioneVaccinazioneEseguita
                    logDatiVaccinali.DataOperazione = DateTime.Now
                    logDatiVaccinali.Note = String.Format("{0}({1}) - {2}", vac.CodiceVaccinazione, vac.NumeroRichiamo, vac.DataConvocazione.ToString("dd/MM/yyyy"))

                    Me.GenericProvider.Log.InsertLogDatiVaccinali(logDatiVaccinali)

                Next

            End Using

            ' Cancellazione convocazioni senza vaccinazioni-bilanci
            Using bizConvocazione As New BizConvocazione(GenericProvider, Settings, CreateBizContextInfosByCodiceUslGestita(UslGestitaCorrente.Codice), Nothing)

                Dim command As New BizConvocazione.EliminaConvocazioneEmptyCommand()
                command.CodicePaziente = codicePazienteDestinazione
                command.DataConvocazione = Nothing
                command.DataEliminazione = DateTime.Now
                command.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.Esecuzione
                command.NoteEliminazione = "Eliminazione convocazione per acquisizione vaccinazioni eseguite da centrale"
                command.WriteLog = True

                bizConvocazione.EliminaConvocazioneEmpty(command)

            End Using

        End If

        Dim aggiornaVaccinazioneEseguitaCentraleDistribuitaResult As New AcquisisciVaccinazioneEseguitaCentraleResult()

        aggiornaVaccinazioneEseguitaCentraleDistribuitaResult.VaccinazioniEseguiteInserite = vaccinazioniEseguiteInseriteList.ToArray()
        aggiornaVaccinazioneEseguitaCentraleDistribuitaResult.VaccinazioniEseguiteScaduteInserite = vaccinazioniEseguiteScaduteInseriteList.ToArray()
        aggiornaVaccinazioneEseguitaCentraleDistribuitaResult.VaccinazioniEseguiteNonInserite = vaccinazioniEseguiteNonInseriteList.ToArray()
        aggiornaVaccinazioneEseguitaCentraleDistribuitaResult.VaccinazioniEseguiteScaduteNonInserite = vaccinazioniEseguiteScaduteNonInseriteList.ToArray()

        Return aggiornaVaccinazioneEseguitaCentraleDistribuitaResult

    End Function

    Friend Sub AcquisisciReazioneAvversaCentrale(codicePazienteDestinazione As Int64, reazioneAvversaAcquisizioneInfos As VaccinazioneEseguitaCentraleInfo())

        For Each vaccinazioneEseguitaAcquisizioneInfo As VaccinazioneEseguitaCentraleInfo In reazioneAvversaAcquisizioneInfos

            Dim vaccinazioneEseguitaCentraleDistributaDestinazione As VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita =
                GenericProviderCentrale.VaccinazioneEseguitaCentrale.GetVaccinazioneEseguitaCentraleDistribuitaByIdCentrale(
                vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguitaCentrale.Id, UslGestitaCorrente.Codice)

            If Not vaccinazioneEseguitaCentraleDistributaDestinazione Is Nothing Then

                Dim reazioneAvversaDestinazione As ReazioneAvversa = AcquisisciReazioneAvversaCentrale(codicePazienteDestinazione, vaccinazioneEseguitaCentraleDistributaDestinazione.IdVaccinazioneEseguita,
                    vaccinazioneEseguitaAcquisizioneInfo.ReazioneAvversa, vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguitaCentrale, vaccinazioneEseguitaCentraleDistributaDestinazione)

                If Not reazioneAvversaDestinazione Is Nothing Then

                    AggiornaVaccinazioneEseguitaCentraleDistribuita(Nothing, reazioneAvversaDestinazione,
                       vaccinazioneEseguitaAcquisizioneInfo.VaccinazioneEseguitaCentrale, vaccinazioneEseguitaCentraleDistributaDestinazione)

                End If

            End If

        Next

    End Sub

    Friend Function AggiornaVisibilitaVaccinazioneEseguita(flagVisibilitaDatiVaccinaliCentrale As String, vaccinazioneEseguita As VaccinazioneEseguita,
                                                           vaccinazioneEseguitaCentrale As VaccinazioneEseguitaCentrale, scaduta As Boolean) As BizResult


        Dim vaccinazioneEseguitaOriginale As VaccinazioneEseguita = vaccinazioneEseguita.Clone()

        Dim vaccinazioniEseguiteSalvaResult As BizResult = Nothing

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions)

            Dim vaccinazioneEseguitaSavedList As New List(Of VaccinazioneEseguita)
            Dim vaccinazioneEseguitaScadutaSavedList As New List(Of VaccinazioneEseguita)

            vaccinazioneEseguita.ves_flag_visibilita_vac_centrale = flagVisibilitaDatiVaccinaliCentrale

            Dim salvaVaccinazioneEseguitaCommand As New BizVaccinazioniEseguite.SalvaVaccinazioneEseguitaCommand
            salvaVaccinazioneEseguitaCommand.Operation = SalvaCommandOperation.Update
            salvaVaccinazioneEseguitaCommand.IsVaccinazioneEseguitaScaduta = scaduta
            salvaVaccinazioneEseguitaCommand.VaccinazioneEseguita = vaccinazioneEseguita

            vaccinazioniEseguiteSalvaResult = Me.SalvaVaccinazioneEseguita(salvaVaccinazioneEseguitaCommand)

            '-- LOG UPDATE FLAG VISIBILITA VACCINAZIONE ESEGUITA
            Me.InsertLogAcquisizioneVaccinazioneEseguitaCentraleDistribuita(vaccinazioneEseguita,
                                                                            vaccinazioneEseguitaCentrale,
                                                                            DataLogStructure.Operazione.Modifica,
                                                                            IIf(vaccinazioniEseguiteSalvaResult.Success, Enumerators.StatoLogDatiVaccinaliCentrali.Success, Enumerators.StatoLogDatiVaccinaliCentrali.Error),
                                                                            True)

            transactionScope.Complete()

        End Using

        Return vaccinazioniEseguiteSalvaResult

    End Function

    Friend Sub InsertLogAcquisizioneVaccinazioneEseguitaCentraleDistribuita(vaccinazioneEseguita As VaccinazioneEseguita, vaccinazioneEseguitaCentrale As VaccinazioneEseguitaCentrale, operazione As DataLogStructure.Operazione, statoLogDatiVaccinaliCentrali As Enumerators.StatoLogDatiVaccinaliCentrali, isVisibilitaModificata As Boolean)

        Me.InsertLogAcquisizioneVaccinazioneEseguitaCentraleDistribuita(vaccinazioneEseguita, vaccinazioneEseguitaCentrale.IdUtenteUltimaOperazioneVaccinazioneEseguita, vaccinazioneEseguitaCentrale.CodiceUslUltimaOperazioneVaccinazioneEseguita, vaccinazioneEseguitaCentrale.TipoVaccinazioneEseguitaCentrale, operazione, statoLogDatiVaccinaliCentrali, isVisibilitaModificata)

    End Sub

    Friend Sub InsertLogAcquisizioneVaccinazioneEseguitaCentraleDistribuita(vaccinazioneEseguita As VaccinazioneEseguita, idUtente As Int64, codiceUsl As String, tipoVaccinazioneEseguitaCentrale As String, operazione As DataLogStructure.Operazione, statoLogDatiVaccinaliCentrali As Enumerators.StatoLogDatiVaccinaliCentrali, isVisibilitaModificata As Boolean)

        Dim codiceArgomento As String

        Dim noteStringBuilder As New Text.StringBuilder()

        noteStringBuilder.AppendFormat("{0} - {1}", vaccinazioneEseguita.ves_vac_codice, vaccinazioneEseguita.ves_n_richiamo)

        Select Case tipoVaccinazioneEseguitaCentrale

            Case Constants.TipoVaccinazioneEseguitaCentrale.Scaduta,
                 Constants.TipoVaccinazioneEseguitaCentrale.ScadutaEliminata

                codiceArgomento = DataLogStructure.TipiArgomento.VAC_SCADUTE

            Case Else

                Select Case tipoVaccinazioneEseguitaCentrale

                    Case Constants.TipoVaccinazioneEseguitaCentrale.Ripristinata

                        noteStringBuilder.Append(" - RIPRISTINATA ")

                    Case Else

                        Select Case tipoVaccinazioneEseguitaCentrale

                            Case Constants.TipoVaccinazioneEseguitaCentrale.Programmata
                                noteStringBuilder.Append(" - PROGRAMMATA ")
                            Case Constants.TipoVaccinazioneEseguitaCentrale.Registrata
                                noteStringBuilder.Append(" - REGISTRATA ")
                            Case Constants.TipoVaccinazioneEseguitaCentrale.Recuperata
                                noteStringBuilder.Append(" - RECUPERATA ")
                        End Select

                End Select

                codiceArgomento = DataLogStructure.TipiArgomento.VAC_ESEGUITE

        End Select

        If isVisibilitaModificata Then

            noteStringBuilder.AppendFormat(" [Nuovo valore consenso: {0}]",
                                           IIf(vaccinazioneEseguita.ves_flag_visibilita_vac_centrale = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente, "CONCESSO", "NEGATO"))

        End If

        Dim logDatiVaccinali As New LogDatiVaccinali()

        logDatiVaccinali.Paziente.Paz_Codice = vaccinazioneEseguita.paz_codice
        logDatiVaccinali.Operazione = operazione
        logDatiVaccinali.Argomento.Codice = codiceArgomento
        logDatiVaccinali.Stato = statoLogDatiVaccinaliCentrali
        logDatiVaccinali.Usl.Codice = codiceUsl
        logDatiVaccinali.Utente.Id = idUtente
        logDatiVaccinali.DataOperazione = DateTime.Now
        logDatiVaccinali.Note = noteStringBuilder.ToString()

        Me.GenericProvider.Log.InsertLogDatiVaccinali(logDatiVaccinali)

    End Sub

    Friend Sub InsertLogAcquisizioneReazioneAvversaCentraleDistribuita(reazioneAvversa As ReazioneAvversa, vaccinazioneEseguitaCentrale As VaccinazioneEseguitaCentrale, operazione As DataLogStructure.Operazione, statoLogDatiVaccinaliCentrali As Enumerators.StatoLogDatiVaccinaliCentrali)

        Dim codiceArgomento As String

        Dim noteStringBuilder As New Text.StringBuilder

        noteStringBuilder.AppendFormat("{0} - {1} - {2}", reazioneAvversa.DataReazione.ToString("dd/MM/yyyy"), reazioneAvversa.CodiceVaccinazione, reazioneAvversa.NumeroRichiamo)

        Select Case vaccinazioneEseguitaCentrale.TipoVaccinazioneEseguitaCentrale

            Case Constants.TipoVaccinazioneEseguitaCentrale.Scaduta,
                 Constants.TipoVaccinazioneEseguitaCentrale.ScadutaEliminata

                codiceArgomento = DataLogStructure.TipiArgomento.REAZ_AVVERSE_SCADUTE

                noteStringBuilder.Append(" - SCADUTA ")

            Case Else

                Select Case vaccinazioneEseguitaCentrale.TipoVaccinazioneEseguitaCentrale

                    Case Constants.TipoVaccinazioneEseguitaCentrale.Ripristinata

                        noteStringBuilder.Append(" - RIPRISTINATA ")

                End Select

                codiceArgomento = DataLogStructure.TipiArgomento.REAZ_AVVERSE

        End Select

        Dim logDatiVaccinali As New LogDatiVaccinali()

        logDatiVaccinali.Paziente.Paz_Codice = reazioneAvversa.CodicePaziente
        logDatiVaccinali.Operazione = operazione
        logDatiVaccinali.Argomento.Codice = codiceArgomento
        logDatiVaccinali.Stato = statoLogDatiVaccinaliCentrali
        logDatiVaccinali.Usl.Codice = vaccinazioneEseguitaCentrale.CodiceUslUltimaOperazioneReazioneAvversa
        logDatiVaccinali.Utente.Id = vaccinazioneEseguitaCentrale.IdUtenteUltimaOperazioneReazioneAvversa
        logDatiVaccinali.DataOperazione = DateTime.Now
        logDatiVaccinali.Note = noteStringBuilder.ToString()

        Me.GenericProvider.Log.InsertLogDatiVaccinali(logDatiVaccinali)

    End Sub

    ''' <summary>
    ''' Restituisce una struttura contenente codice usl inserimento, id vaccinazione eseguita, elenco usl in cui è stata distribuita (esclusa la usl di inserimento),
    ''' per ogni vaccinazione eseguita (locale) specificata.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Function GetCodiciUslDistribuiteVaccinazioniEseguiteLocali(idVaccinazioniEseguiteLocali As List(Of Int64), codiceUslInserimento As String) As Dictionary(Of KeyValuePair(Of Int64, String), String())

        If idVaccinazioniEseguiteLocali Is Nothing OrElse idVaccinazioniEseguiteLocali.Count = 0 Then Return New Dictionary(Of KeyValuePair(Of Int64, String), String())()

        Dim uslDistribuitaDatoVaccinaleInfoList As List(Of Entities.UslDistribuitaDatoVaccinaleInfo) =
            Me.GenericProviderCentrale.VaccinazioneEseguitaCentrale.GetUslDistribuiteVaccinazioniEseguite(idVaccinazioniEseguiteLocali, codiceUslInserimento)

        Return Me.GetCodiciUslDistribuiteDatiVaccinaliLocali(idVaccinazioniEseguiteLocali, codiceUslInserimento, uslDistribuitaDatoVaccinaleInfoList)

    End Function

    ''' <summary>
    ''' Restituisce una struttura contenente codice usl inserimento, id reazione avversa, elenco usl in cui è stata distribuita (esclusa la usl di inserimento),
    ''' per ogni reazione avversa (locale) specificata.
    ''' </summary>
    ''' <param name="idReazioniAvverseLocali"></param>
    ''' <param name="codiceUslInserimento"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Function GetCodiciUslDistribuiteReazioniAvverseLocali(idReazioniAvverseLocali As List(Of Int64), codiceUslInserimento As String) As Dictionary(Of KeyValuePair(Of Int64, String), String())

        If idReazioniAvverseLocali Is Nothing OrElse idReazioniAvverseLocali.Count = 0 Then Return New Dictionary(Of KeyValuePair(Of Int64, String), String())()

        Dim uslDistribuitaDatoVaccinaleInfoList As List(Of Entities.UslDistribuitaDatoVaccinaleInfo) =
            Me.GenericProviderCentrale.VaccinazioneEseguitaCentrale.GetUslDistribuiteReazioniAvverse(idReazioniAvverseLocali, codiceUslInserimento)

        Return Me.GetCodiciUslDistribuiteDatiVaccinaliLocali(idReazioniAvverseLocali, codiceUslInserimento, uslDistribuitaDatoVaccinaleInfoList)

    End Function

    Private Function GetCodiciUslDistribuiteDatiVaccinaliLocali(idLocali As List(Of Int64), codiceUslInserimento As String, uslDistribuitaDatoVaccinaleInfoList As List(Of Entities.UslDistribuitaDatoVaccinaleInfo)) As Dictionary(Of KeyValuePair(Of Int64, String), String())

        Dim datiUslDistribuite As New Dictionary(Of KeyValuePair(Of Int64, String), String())()

        If Not idLocali Is Nothing AndAlso Not uslDistribuitaDatoVaccinaleInfoList Is Nothing AndAlso uslDistribuitaDatoVaccinaleInfoList.Count > 0 Then

            For Each idLocale As Int64 In idLocali

                Dim id As Int64 = idLocale

                Dim codiciUslDistribuite As String() =
                    uslDistribuitaDatoVaccinaleInfoList.Where(Function(p) p.IdDatoVaccinale = id AndAlso p.CodiceUslInserimento = codiceUslInserimento).
                                                        Select(Function(p) p.CodiceUslDistribuita).ToArray()

                datiUslDistribuite.Add(New KeyValuePair(Of Int64, String)(idLocale, codiceUslInserimento), codiciUslDistribuite)

            Next

        End If

        Return datiUslDistribuite

    End Function

#End Region

#Region " Private "

    Private Function FindVaccinazioneEseguitaCentrale(vaccinazioneEseguitaOrigine As VaccinazioneEseguita, uslOrigine As Usl, vaccinazioneEseguitaCentraleEnumerable As IEnumerable(Of VaccinazioneEseguitaCentrale)) As VaccinazioneEseguitaCentrale

        ' N.B. OTTIMIZZAZIONE !!!
        If vaccinazioneEseguitaOrigine.ves_usl_inserimento = uslOrigine.Codice Then

            Return vaccinazioneEseguitaCentraleEnumerable.First(
                Function(vec) vec.IdVaccinazioneEseguita = vaccinazioneEseguitaOrigine.ves_id AndAlso
                    vec.CodiceUslVaccinazioneEseguita = uslOrigine.Codice)
        Else

            Dim vaccinazioneEseguitaCentraleDistributaOrigine As VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita =
                Me.GenericProviderCentrale.VaccinazioneEseguitaCentrale.GetVaccinazioneEseguitaCentraleDistribuitaByUsl(vaccinazioneEseguitaOrigine.ves_id,
                    uslOrigine.Codice)

            Return vaccinazioneEseguitaCentraleEnumerable.First(Function(vec) vec.Id = vaccinazioneEseguitaCentraleDistributaOrigine.IdVaccinazioneEseguitaCentrale)

        End If

    End Function

    Private Function AcquisisciReazioneAvversaCentrale(codicePazienteDestinazione As Int64, idVaccinazioneEseguitaDestinazione As Int64,
                                                       reazioneAvversaOrigine As ReazioneAvversa, vaccinazioneEseguitaCentrale As VaccinazioneEseguitaCentrale,
                                                       vaccinazioneEseguitaCentraleDistributa As VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita) As ReazioneAvversa

        Dim reazioneAvversaDestinazione As ReazioneAvversa = Nothing

        If vaccinazioneEseguitaCentrale.TipoReazioneAvversaCentrale <> Constants.TipoReazioneAvversaCentrale.Eliminata Then

            Dim operazioneLogDatiVaccinaliCentrali As Log.DataLogStructure.Operazione? = Nothing

            If Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(vaccinazioneEseguitaCentrale.FlagVisibilitaCentrale) OrElse
               Not vaccinazioneEseguitaCentraleDistributa Is Nothing Then

                If vaccinazioneEseguitaCentrale.CodiceUslUltimaOperazioneReazioneAvversa = vaccinazioneEseguitaCentrale.CodiceUslReazioneAvversa Then

                    reazioneAvversaDestinazione = CreateReazioneAvversaDestinazioneByOrigine(reazioneAvversaOrigine, vaccinazioneEseguitaCentrale,
                                                                                             vaccinazioneEseguitaCentrale.TipoReazioneAvversaCentrale)

                ElseIf vaccinazioneEseguitaCentrale.CodiceUslReazioneAvversa = UslGestitaCorrente.Codice Then
                    ' TODO [Unificazione Ulss]: usl inserimento - Qui ci va il nuovo codice ContextInfos.CodiceUsl ???

                    If vaccinazioneEseguitaCentrale.TipoVaccinazioneEseguitaCentrale = Constants.TipoVaccinazioneEseguitaCentrale.Ripristinata Then

                        reazioneAvversaDestinazione = Me.GenericProvider.VaccinazioniEseguite.GetReazioneAvversaScadutaByIdIfExists(vaccinazioneEseguitaCentraleDistributa.IdReazioneAvversa)

                        If reazioneAvversaDestinazione Is Nothing Then
                            reazioneAvversaDestinazione = Me.GenericProvider.VaccinazioniEseguite.GetReazioneAvversaById(vaccinazioneEseguitaCentraleDistributa.IdReazioneAvversa)
                        End If

                    Else

                        reazioneAvversaDestinazione = Me.GenericProvider.VaccinazioniEseguite.GetReazioneAvversaByIdIfExists(vaccinazioneEseguitaCentraleDistributa.IdReazioneAvversa)

                        If reazioneAvversaDestinazione Is Nothing Then
                            reazioneAvversaDestinazione = Me.GenericProvider.VaccinazioniEseguite.GetReazioneAvversaScadutaById(vaccinazioneEseguitaCentraleDistributa.IdReazioneAvversa)
                        End If

                    End If

                Else

                    reazioneAvversaDestinazione = reazioneAvversaOrigine.Clone()

                End If

                reazioneAvversaDestinazione.CodicePaziente = codicePazienteDestinazione
                reazioneAvversaDestinazione.IdVaccinazioneEseguita = idVaccinazioneEseguitaDestinazione

                Dim isVisibilitaCentrale As Boolean = Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(vaccinazioneEseguitaCentrale.FlagVisibilitaCentrale)

                If vaccinazioneEseguitaCentraleDistributa Is Nothing OrElse
                    Not vaccinazioneEseguitaCentraleDistributa.IdReazioneAvversa.HasValue OrElse
                    vaccinazioneEseguitaCentraleDistributa.IdReazioneAvversaVaccinazioneEseguitaCentrale <> vaccinazioneEseguitaCentrale.IdReazioneAvversa OrElse
                    vaccinazioneEseguitaCentraleDistributa.CodiceUslReazioneAvversaVaccinazioneEseguitaCentrale <> vaccinazioneEseguitaCentrale.CodiceUslReazioneAvversa Then

                    If Not vaccinazioneEseguitaCentraleDistributa Is Nothing AndAlso vaccinazioneEseguitaCentraleDistributa.IdReazioneAvversa.HasValue AndAlso
                        (vaccinazioneEseguitaCentraleDistributa.IdReazioneAvversaVaccinazioneEseguitaCentrale <> vaccinazioneEseguitaCentrale.IdReazioneAvversa OrElse
                        vaccinazioneEseguitaCentraleDistributa.CodiceUslReazioneAvversaVaccinazioneEseguitaCentrale <> vaccinazioneEseguitaCentrale.CodiceUslReazioneAvversa) Then

                        Dim reaOld As ReazioneAvversa = Me.GenericProvider.VaccinazioniEseguite.GetReazioneAvversaByIdIfExists(vaccinazioneEseguitaCentraleDistributa.IdReazioneAvversa)
                        If Not reaOld Is Nothing Then
                            Me.DeleteReazioneAvversa(reaOld)
                        Else
                            reaOld = Me.GenericProvider.VaccinazioniEseguite.GetReazioneAvversaScadutaByIdIfExists(vaccinazioneEseguitaCentraleDistributa.IdReazioneAvversa)
                            If Not reaOld Is Nothing Then
                                Me.DeleteReazioneAvversaScaduta(reaOld)
                            End If
                        End If

                    End If

                    reazioneAvversaDestinazione.IdReazioneAvversa = Nothing
                    reazioneAvversaDestinazione.DataCompilazione = DateTime.MinValue

                    If vaccinazioneEseguitaCentrale.TipoVaccinazioneEseguitaCentrale = Constants.TipoVaccinazioneEseguitaCentrale.Scaduta Then
                        Me.InsertReazioneAvversaScaduta(reazioneAvversaDestinazione, vaccinazioneEseguitaCentrale.FlagVisibilitaCentrale)
                    Else
                        Me.InsertReazioneAvversa(reazioneAvversaDestinazione, vaccinazioneEseguitaCentrale.FlagVisibilitaCentrale)
                    End If

                    If Not vaccinazioneEseguitaCentraleDistributa Is Nothing Then
                        vaccinazioneEseguitaCentraleDistributa.IdReazioneAvversa = reazioneAvversaDestinazione.IdReazioneAvversa
                    End If

                    operazioneLogDatiVaccinaliCentrali = DataLogStructure.Operazione.Inserimento

                Else

                    reazioneAvversaDestinazione.IdReazioneAvversa = vaccinazioneEseguitaCentraleDistributa.IdReazioneAvversa.Value

                    If vaccinazioneEseguitaCentrale.TipoVaccinazioneEseguitaCentrale = Constants.TipoVaccinazioneEseguitaCentrale.Scaduta Then

                        If Not Me.UpdateReazioneAvversaScaduta(reazioneAvversaDestinazione) Then

                            Me.GenericProvider.VaccinazioniEseguite.DeleteReazioneAvversaById(reazioneAvversaDestinazione.IdReazioneAvversa)

                            Me.InsertReazioneAvversaScaduta(reazioneAvversaDestinazione, vaccinazioneEseguitaCentrale.FlagVisibilitaCentrale)

                            operazioneLogDatiVaccinaliCentrali = DataLogStructure.Operazione.Inserimento

                        Else

                            operazioneLogDatiVaccinaliCentrali = DataLogStructure.Operazione.Modifica

                        End If

                    Else

                        If Not Me.UpdateReazioneAvversa(reazioneAvversaDestinazione) Then

                            Me.GenericProvider.VaccinazioniEseguite.DeleteReazioneAvversaScadutaById(reazioneAvversaDestinazione.IdReazioneAvversa)

                            Me.InsertReazioneAvversa(reazioneAvversaDestinazione, vaccinazioneEseguitaCentrale.FlagVisibilitaCentrale)

                            operazioneLogDatiVaccinaliCentrali = DataLogStructure.Operazione.Inserimento

                        Else

                            operazioneLogDatiVaccinaliCentrali = DataLogStructure.Operazione.Modifica

                        End If

                    End If

                End If

            End If

            If operazioneLogDatiVaccinaliCentrali.HasValue Then
                Me.InsertLogAcquisizioneReazioneAvversaCentraleDistribuita(reazioneAvversaDestinazione, vaccinazioneEseguitaCentrale, operazioneLogDatiVaccinaliCentrali.Value,
                                                                            Enumerators.StatoLogDatiVaccinaliCentrali.Success)
            End If

        Else

            If Not vaccinazioneEseguitaCentraleDistributa Is Nothing AndAlso vaccinazioneEseguitaCentraleDistributa.IdReazioneAvversa.HasValue Then
                reazioneAvversaDestinazione = Me.EliminaReazioneAvversaCentrale(reazioneAvversaOrigine.IdUtenteEliminazione, vaccinazioneEseguitaCentrale, vaccinazioneEseguitaCentraleDistributa)
            End If

        End If

        Return reazioneAvversaDestinazione

    End Function

    Private Function EliminaReazioneAvversaCentrale(idUtenteEliminazione As Int64, vaccinazioneEseguitaCentrale As VaccinazioneEseguitaCentrale, vaccinazioneEseguitaCentraleDistributa As VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita) As ReazioneAvversa

        Dim reazioneAvversaDestinazione As ReazioneAvversa = Nothing

        Dim operazioneLogDatiVaccinaliCentrali As Log.DataLogStructure.Operazione? = Nothing

        If vaccinazioneEseguitaCentrale.TipoVaccinazioneEseguitaCentrale = Constants.TipoVaccinazioneEseguitaCentrale.ScadutaEliminata Then

            reazioneAvversaDestinazione = GenericProvider.VaccinazioniEseguite.GetReazioneAvversaScadutaByIdIfExists(vaccinazioneEseguitaCentraleDistributa.IdReazioneAvversa.Value)

            If reazioneAvversaDestinazione Is Nothing Then

                reazioneAvversaDestinazione = GenericProvider.VaccinazioniEseguite.GetReazioneAvversaByIdIfExists(vaccinazioneEseguitaCentraleDistributa.IdReazioneAvversa.Value)

                If Not reazioneAvversaDestinazione Is Nothing Then

                    reazioneAvversaDestinazione.IdUtenteEliminazione = idUtenteEliminazione

                    DeleteReazioneAvversa(reazioneAvversaDestinazione)

                    operazioneLogDatiVaccinaliCentrali = DataLogStructure.Operazione.Eliminazione

                End If

            Else

                reazioneAvversaDestinazione.IdUtenteEliminazione = idUtenteEliminazione

                DeleteReazioneAvversaScaduta(reazioneAvversaDestinazione)

                operazioneLogDatiVaccinaliCentrali = DataLogStructure.Operazione.Eliminazione

            End If

        ElseIf vaccinazioneEseguitaCentrale.TipoVaccinazioneEseguitaCentrale = Constants.TipoVaccinazioneEseguitaCentrale.Eliminata OrElse
            vaccinazioneEseguitaCentrale.TipoReazioneAvversaCentrale = Constants.TipoReazioneAvversaCentrale.Eliminata Then

            reazioneAvversaDestinazione = GenericProvider.VaccinazioniEseguite.GetReazioneAvversaByIdIfExists(vaccinazioneEseguitaCentraleDistributa.IdReazioneAvversa.Value)

            If reazioneAvversaDestinazione Is Nothing Then

                reazioneAvversaDestinazione = GenericProvider.VaccinazioniEseguite.GetReazioneAvversaScadutaByIdIfExists(vaccinazioneEseguitaCentraleDistributa.IdReazioneAvversa.Value)

                If Not reazioneAvversaDestinazione Is Nothing Then

                    reazioneAvversaDestinazione.IdUtenteEliminazione = idUtenteEliminazione

                    DeleteReazioneAvversaScaduta(reazioneAvversaDestinazione)

                End If

            Else

                reazioneAvversaDestinazione.IdUtenteEliminazione = idUtenteEliminazione

                DeleteReazioneAvversa(reazioneAvversaDestinazione)

                operazioneLogDatiVaccinaliCentrali = DataLogStructure.Operazione.Eliminazione

            End If

        End If

        If operazioneLogDatiVaccinaliCentrali.HasValue Then
            InsertLogAcquisizioneReazioneAvversaCentraleDistribuita(
                reazioneAvversaDestinazione, vaccinazioneEseguitaCentrale, operazioneLogDatiVaccinaliCentrali.Value, Enumerators.StatoLogDatiVaccinaliCentrali.Success)
        End If

        Return reazioneAvversaDestinazione

    End Function

    Private Function AggiornaVaccinazioneEseguitaCentraleDistribuita(vaccinazioneEseguita As VaccinazioneEseguita, reazioneAvversa As ReazioneAvversa, vaccinazioneEseguitaCentrale As VaccinazioneEseguitaCentrale, vaccinazioneEseguitaCentraleDistribuita As VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita) As VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita

        If vaccinazioneEseguitaCentraleDistribuita Is Nothing Then

            vaccinazioneEseguitaCentraleDistribuita = New VaccinazioneEseguitaCentrale.VaccinazioneEseguitaDistribuita()

            ' TODO [Unificazione Ulss]: usl inserimento - Qui ci va il nuovo codice ContextInfos.CodiceUsl ???
            vaccinazioneEseguitaCentraleDistribuita.CodiceUslVaccinazioneEseguita = UslGestitaCorrente.Codice

            vaccinazioneEseguitaCentraleDistribuita.IdVaccinazioneEseguita = vaccinazioneEseguita.ves_id

            If Not reazioneAvversa Is Nothing Then
                vaccinazioneEseguitaCentraleDistribuita.IdReazioneAvversa = reazioneAvversa.IdReazioneAvversa
                vaccinazioneEseguitaCentraleDistribuita.IdReazioneAvversaVaccinazioneEseguitaCentrale = vaccinazioneEseguitaCentrale.IdReazioneAvversa
                vaccinazioneEseguitaCentraleDistribuita.CodiceUslReazioneAvversaVaccinazioneEseguitaCentrale = vaccinazioneEseguitaCentrale.CodiceUslReazioneAvversa
            End If

            vaccinazioneEseguitaCentraleDistribuita.CodicePaziente = vaccinazioneEseguita.paz_codice

            vaccinazioneEseguitaCentraleDistribuita.DataInserimentoVaccinazioneEseguita = DateTime.Now
            vaccinazioneEseguitaCentraleDistribuita.IdUtenteInserimentoVaccinazioneEseguita = vaccinazioneEseguitaCentrale.IdUtenteInserimentoVaccinazioneEseguita
            vaccinazioneEseguitaCentraleDistribuita.IdVaccinazioneEseguitaCentrale = vaccinazioneEseguitaCentrale.Id

            GenericProviderCentrale.VaccinazioneEseguitaCentrale.InsertVaccinazioneEseguitaCentraleDistribuita(vaccinazioneEseguitaCentraleDistribuita)

        Else

            vaccinazioneEseguitaCentraleDistribuita.DataAggiornamentoVaccinazioneEseguita = DateTime.Now

            If Not reazioneAvversa Is Nothing Then
                vaccinazioneEseguitaCentraleDistribuita.IdReazioneAvversa = reazioneAvversa.IdReazioneAvversa
            End If

            vaccinazioneEseguitaCentraleDistribuita.IdReazioneAvversaVaccinazioneEseguitaCentrale = vaccinazioneEseguitaCentrale.IdReazioneAvversa
            vaccinazioneEseguitaCentraleDistribuita.CodiceUslReazioneAvversaVaccinazioneEseguitaCentrale = vaccinazioneEseguitaCentrale.CodiceUslReazioneAvversa

            GenericProviderCentrale.VaccinazioneEseguitaCentrale.UpdateVaccinazioneEseguitaCentraleDistribuita(vaccinazioneEseguitaCentraleDistribuita)

        End If

        Return vaccinazioneEseguitaCentraleDistribuita

    End Function

    Private Function CreateVaccinazioneEseguitaDestinazioneByOrigine(vaccinazioneEseguitaOrigine As VaccinazioneEseguita, vaccinazioneEseguitaCentrale As VaccinazioneEseguitaCentrale) As VaccinazioneEseguita

        Dim vaccinazioneEseguita As VaccinazioneEseguita = vaccinazioneEseguitaOrigine.Clone()

        If vaccinazioneEseguitaCentrale.MergeInfoCentrale.CodicePazienteCentraleAlias Is Nothing Then
            vaccinazioneEseguita.ves_paz_codice_old = Nothing
        Else
            Dim codicePaziente As String = GenericProvider.Paziente.GetCodicePazientiByCodiceAusiliario(vaccinazioneEseguitaCentrale.MergeInfoCentrale.CodicePazienteCentraleAlias).FirstOrDefault()
            If Not String.IsNullOrEmpty(codicePaziente) Then
                vaccinazioneEseguita.ves_paz_codice_old = New Int64?(Convert.ToInt64(codicePaziente))
            End If
        End If

        vaccinazioneEseguita.ves_stato = "R"

        Dim noteAcquisizioneStringBuilder As New System.Text.StringBuilder()

        If Not String.IsNullOrEmpty(vaccinazioneEseguitaOrigine.ves_cns_codice) Then
            noteAcquisizioneStringBuilder.AppendFormat("Centro Vaccinale: {0} / ", vaccinazioneEseguitaOrigine.ves_cns_codice)
        End If

        If Not String.IsNullOrEmpty(vaccinazioneEseguitaOrigine.ves_lot_codice) Then
            noteAcquisizioneStringBuilder.AppendFormat("Lotto: {0} / ", vaccinazioneEseguitaOrigine.ves_lot_codice)
        End If

        ' Le proprietà ves_med_vaccinante_nome, amb_descrizione non vengono valorizzate dalla query (mancano i join)
        'If Not String.IsNullOrEmpty(vaccinazioneEseguitaOrigine.ves_med_vaccinante_codice) Then
        '    noteAcquisizioneStringBuilder.AppendFormat("Medico: {0} / ", vaccinazioneEseguitaOrigine.ves_med_vaccinante_nome)
        'End If

        'If Not String.IsNullOrEmpty(vaccinazioneEseguitaOrigine.ves_amb_codice) Then
        '    noteAcquisizioneStringBuilder.AppendFormat("Ambulatorio : {0} / ", vaccinazioneEseguitaOrigine.amb_descrizione)
        'End If

        If Not String.IsNullOrEmpty(vaccinazioneEseguitaOrigine.ves_luogo) Then
            noteAcquisizioneStringBuilder.AppendFormat("Luogo : {0} / ", vaccinazioneEseguitaOrigine.ves_luogo)
        End If

        If noteAcquisizioneStringBuilder.Length > 0 Then
            noteAcquisizioneStringBuilder.Remove(noteAcquisizioneStringBuilder.Length - 3, 3)
        End If

        vaccinazioneEseguita.ves_note_acquisizione_vac_centrale = noteAcquisizioneStringBuilder.ToString()

        vaccinazioneEseguita.ves_cns_codice = Nothing
        vaccinazioneEseguita.ves_lot_codice = Nothing
        vaccinazioneEseguita.ves_amb_codice = Nothing
        vaccinazioneEseguita.ves_med_vaccinante_codice = Nothing
        vaccinazioneEseguita.ves_luogo = Nothing
        vaccinazioneEseguita.ves_ope_codice = Nothing
        vaccinazioneEseguita.ves_ass_prog = Nothing

        Return vaccinazioneEseguita

    End Function

    Private Function CreateReazioneAvversaDestinazioneByOrigine(reazioneAvversaOrigine As ReazioneAvversa, vaccinazioneEseguitaCentrale As VaccinazioneEseguitaCentrale, tipoReazioneAvversaCentrale As String) As ReazioneAvversa

        Dim reazioneAvversa As ReazioneAvversa = reazioneAvversaOrigine.Clone()

        If vaccinazioneEseguitaCentrale.MergeInfoCentrale.CodicePazienteCentraleAlias Is Nothing Then
            reazioneAvversa.CodicePazientePrecedente = Nothing
        Else
            reazioneAvversa.CodicePazientePrecedente = New Int64?(GenericProvider.Paziente.GetCodicePazientiByCodiceAusiliario(vaccinazioneEseguitaCentrale.MergeInfoCentrale.CodicePazienteCentraleAlias).FirstOrDefault())
        End If

        reazioneAvversa.CodiceLotto = Nothing

        Return reazioneAvversa

    End Function

    Private Function CanSaveVaccinazioneEseguita(canSaveVaccinazioneEseguitaCommand As CanSaveVaccinazioneEseguitaCommand) As CanSaveVaccinazioneEseguitaResult

        Dim canSaveVaccinazioneEseguitaResult As New CanSaveVaccinazioneEseguitaResult()

        Select Case canSaveVaccinazioneEseguitaCommand.Operation

            Case SalvaCommandOperation.Insert, SalvaCommandOperation.Update

                canSaveVaccinazioneEseguitaResult =
                    CanSaveVaccinazioneEseguita(canSaveVaccinazioneEseguitaCommand.VaccinazioneEseguita, True, canSaveVaccinazioneEseguitaCommand.IsVaccinazioneEseguitaScaduta)

            Case SalvaCommandOperation.Delete
                'NOTHING

            Case Else
                Throw New NotImplementedException()

        End Select

        Return canSaveVaccinazioneEseguitaResult

    End Function

    'Private Sub CanSaveVaccinazioniEseguiteCentraleIfNeeded(dataView As DataView, drVaccinazioneEseguitaCentralizzataList As List(Of DataRow), vaccinazioniEseguiteCentralizzatePazientiDictionary As IDictionary(Of Int64, DataTable))

    '    For Each dataRowView As DataRowView In dataView

    '        Dim flagVisibilitaCorrente As String = dataRowView.Row("ves_flag_visibilita").ToString()
    '        Dim flagVisibilitaOriginale As String = String.Empty

    '        If dataRowView.Row.RowState = DataRowState.Modified Then
    '            flagVisibilitaOriginale = dataRowView.Row("ves_flag_visibilita", DataRowVersion.Original).ToString()
    '        End If

    '        If Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(flagVisibilitaCorrente) AndAlso
    '            Not Me.Settings.VALORI_VISIBILITA_VACC_CENTRALE.Contains(flagVisibilitaOriginale) Then

    '            Dim codicePaziente As Int64 = dataRowView.Row("paz_codice")

    '            Dim dtVaccinazioniEseguiteCentralizzate As DataTable = Nothing

    '            If Not vaccinazioniEseguiteCentralizzatePazientiDictionary.ContainsKey(codicePaziente) Then
    '                dtVaccinazioniEseguiteCentralizzate = Me.GetVaccinazioniEseguiteCentralizzate(codicePaziente)
    '                vaccinazioniEseguiteCentralizzatePazientiDictionary.Add(codicePaziente, dtVaccinazioniEseguiteCentralizzate)
    '            End If

    '            Dim drVaccinazioniEseguiteCentralizzata As DataRow =
    '                Me.CanSaveVaccinazioniEseguiteCentrale(dataRowView.Row("ves_vac_codice"), dataRowView.Row("ves_n_richiamo"),
    '                    dataRowView.Row("ves_data_effettuazione"), (dataRowView.Row("scaduta").ToString() = "S"), dtVaccinazioniEseguiteCentralizzate)

    '            If Not drVaccinazioniEseguiteCentralizzata Is Nothing Then
    '                drVaccinazioneEseguitaCentralizzataList.Add(drVaccinazioniEseguiteCentralizzata)
    '            End If

    '        End If

    '    Next

    'End Sub

    Private Function CanSaveVaccinazioniEseguiteCentrale(codiceVaccinazione As String, numeroRichiamo As Int16, dataEffettuazione As String, scaduta As Boolean, dtVaccinazioniEseguiteCentralizzateEsistenti As DataTable) As DataRow

        Dim drVaccinazioniEseguiteCentralizzata As DataRow = Nothing

        If Not scaduta Then

            drVaccinazioniEseguiteCentralizzata = dtVaccinazioniEseguiteCentralizzateEsistenti.AsEnumerable().FirstOrDefault(
                Function(dr) dr("ves_vac_codice") = codiceVaccinazione AndAlso
                              dr("ves_n_richiamo") = numeroRichiamo AndAlso
                              dr("scaduta").ToString() <> "S")
        End If

        If drVaccinazioniEseguiteCentralizzata Is Nothing Then

            drVaccinazioniEseguiteCentralizzata = dtVaccinazioniEseguiteCentralizzateEsistenti.AsEnumerable().FirstOrDefault(
                    Function(dr) dr("ves_vac_codice") = codiceVaccinazione AndAlso
                                 dr("ves_n_richiamo") = numeroRichiamo AndAlso
                                 dr("ves_data_effettuazione") = dataEffettuazione AndAlso
                                 dr("scaduta").ToString() = "S")
        End If

        If drVaccinazioniEseguiteCentralizzata Is Nothing Then

            If scaduta Then

                drVaccinazioniEseguiteCentralizzata = dtVaccinazioniEseguiteCentralizzateEsistenti.AsEnumerable().FirstOrDefault(
                     Function(dr) dr("ves_vac_codice") = codiceVaccinazione AndAlso
                                  dr("ves_n_richiamo") = numeroRichiamo AndAlso
                                  dr("ves_data_effettuazione") = dataEffettuazione AndAlso
                                  dr("scaduta").ToString() <> "S")
            End If

        End If

        Return drVaccinazioniEseguiteCentralizzata

    End Function

    Private Function UpdateConflittoVaccinazioneEseguitaCentraleIfNeeded(idVaccinazioneEseguitaEsistente As Int64, vacLocaleOriginale As VaccinazioneEseguita,
                        vacLocaleInConflitto As VaccinazioneEseguita, vaccinazioneEseguitaCentraleEsistente As VaccinazioneEseguitaCentrale) As Boolean

        Dim conflittoRisolto As Boolean = True

        'Recupero in centrale la vaccinazione eseguita
        Dim vacCentraleInConflitto As VaccinazioneEseguitaCentrale =
            GenericProviderCentrale.VaccinazioneEseguitaCentrale.GetVaccinazioneEseguitaCentraleByIdLocale(idVaccinazioneEseguitaEsistente, UslGestitaCorrente.Codice)

        If vacCentraleInConflitto Is Nothing Then Return True

        'Controllo se è presente un conflitto
        Dim conflittoAccettato As Boolean = False

        If vacCentraleInConflitto.TipoVaccinazioneEseguitaCentrale <> Constants.TipoVaccinazioneEseguitaCentrale.Eliminata Then
            If vacCentraleInConflitto.IdConflitto.HasValue Then
                If (vacCentraleInConflitto.DataRisoluzioneConflitto.HasValue) Then
                    conflittoAccettato = True
                End If
            Else
                conflittoAccettato = True
            End If
        End If

        If conflittoAccettato Then

            'Provo a risolvere il conflitto, nel caso non riesca lascio il conflitto come irrisolto
            If Not vacLocaleOriginale Is Nothing Then

                If Settings.CONFLITTI_AUTORISOLUZIONE Then
                    conflittoRisolto = AutoRisolviConflittoVaccinazioneEseguitaCentrale(vacLocaleOriginale, vacLocaleInConflitto, vaccinazioneEseguitaCentraleEsistente, vacCentraleInConflitto)
                Else
                    conflittoRisolto = False
                End If

            End If

            If Not conflittoRisolto Then

                'É presente un conflitto: vengono inserite le info sul conflitto per le vaccinazioni in centrale
                vacCentraleInConflitto.IdConflitto = vacCentraleInConflitto.Id
                vacCentraleInConflitto.DataRisoluzioneConflitto = Nothing
                vacCentraleInConflitto.IdUtenteRisoluzioneConflitto = Nothing

                GenericProviderCentrale.VaccinazioneEseguitaCentrale.UpdateVaccinazioneEseguitaCentrale(vacCentraleInConflitto)

                vaccinazioneEseguitaCentraleEsistente.IdConflitto = vacCentraleInConflitto.Id
                vaccinazioneEseguitaCentraleEsistente.DataRisoluzioneConflitto = Nothing
                vaccinazioneEseguitaCentraleEsistente.IdUtenteRisoluzioneConflitto = Nothing

                GenericProviderCentrale.VaccinazioneEseguitaCentrale.UpdateVaccinazioneEseguitaCentrale(vaccinazioneEseguitaCentraleEsistente)

            End If

        End If

        Return conflittoRisolto

    End Function

    ''' <summary>
    ''' Metodo che permette di autorisolvere conflitti di registrazione vaccinazioni eseguite in centrale
    ''' </summary>
    Private Function AutoRisolviConflittoVaccinazioneEseguitaCentrale(vacOriginale As VaccinazioneEseguita, vacInConflitto As VaccinazioneEseguita,
            vacCentraleEsistente As VaccinazioneEseguitaCentrale, vacCentraleInConflitto As VaccinazioneEseguitaCentrale) As Boolean

        Dim idVacVincitore As Long?
        Dim idVacCentraleVincitore As Long?

        'Nel caso in cui i dati coincidano (data effettuazione, vaccinazione, dose), il conflitto si auto-risolve secondo questa logica:
        '   1. In presenza di lotto -> ULSS proprietaria del dato
        '   2. Else: presenza di nome commerciale -> ULSS proprietaria del dato
        '   3. Else: ves_stato = “eseguita” (contro “registrata”) -> ULSS proprietaria del dato
        '   4. Else: data di registrazione inferiore -> ULSS proprietaria del dato
        '   5. Else: -> ULSS proprietaria del dato la prima ULSS che ha scritto il dato in centrale (per data inserimento o, se uguale, per vcc_id inferiore)

        If (vacOriginale.ves_data_effettuazione = vacInConflitto.ves_data_effettuazione AndAlso
            vacOriginale.ves_vac_codice = vacInConflitto.ves_vac_codice AndAlso vacOriginale.ves_n_richiamo = vacInConflitto.ves_n_richiamo) Then

            If (Not String.IsNullOrWhiteSpace(vacOriginale.ves_lot_codice) Xor Not String.IsNullOrWhiteSpace(vacInConflitto.ves_lot_codice)) Then

                '1. Solo se uno dei due ha il lotto vince
                If (Not String.IsNullOrWhiteSpace(vacOriginale.ves_lot_codice)) Then
                    idVacVincitore = vacOriginale.ves_id
                    idVacCentraleVincitore = vacCentraleEsistente.Id
                Else
                    idVacVincitore = vacInConflitto.ves_id
                    idVacCentraleVincitore = vacCentraleInConflitto.Id
                End If

            ElseIf (Not String.IsNullOrWhiteSpace(vacOriginale.ves_noc_codice) Xor Not String.IsNullOrWhiteSpace(vacInConflitto.ves_noc_codice)) Then

                '2. Solo se uno dei due ha il nome commerciale
                If (Not String.IsNullOrWhiteSpace(vacOriginale.ves_noc_codice)) Then
                    idVacVincitore = vacOriginale.ves_id
                    idVacCentraleVincitore = vacCentraleEsistente.Id
                Else
                    idVacVincitore = vacInConflitto.ves_id
                    idVacCentraleVincitore = vacCentraleInConflitto.Id
                End If

            ElseIf (((String.IsNullOrWhiteSpace(vacOriginale.ves_stato) OrElse vacOriginale.ves_stato = "E") AndAlso vacInConflitto.ves_stato = "R") OrElse
                ((String.IsNullOrWhiteSpace(vacInConflitto.ves_stato) OrElse vacInConflitto.ves_stato = "E") AndAlso vacOriginale.ves_stato = "R")) Then

                '3. Else: ves_stato = “eseguita” (contro “registrata”)
                If (String.IsNullOrWhiteSpace(vacOriginale.ves_stato) OrElse vacOriginale.ves_stato = "E") Then
                    idVacVincitore = vacOriginale.ves_id
                    idVacCentraleVincitore = vacCentraleEsistente.Id
                Else
                    idVacVincitore = vacInConflitto.ves_id
                    idVacCentraleVincitore = vacCentraleInConflitto.Id
                End If

            ElseIf (vacOriginale.ves_data_registrazione <> vacInConflitto.ves_data_registrazione) Then

                '4. Else: data di registrazione inferiore
                If (vacOriginale.ves_data_registrazione < vacInConflitto.ves_data_registrazione) Then
                    idVacVincitore = vacOriginale.ves_id
                    idVacCentraleVincitore = vacCentraleEsistente.Id
                Else
                    idVacVincitore = vacInConflitto.ves_id
                    idVacCentraleVincitore = vacCentraleInConflitto.Id
                End If

            Else

                '5. Verifica delle vaccinazioni in centrale (per data inserimento o, se uguale, per vcc_id inferiore)
                If (vacCentraleEsistente.DataInserimentoVaccinazioneEseguita <> vacCentraleInConflitto.DataInserimentoVaccinazioneEseguita) Then

                    'Prendo la vaccinazione in centrale che ha data inserimento minore
                    If (vacCentraleEsistente.DataInserimentoVaccinazioneEseguita < vacCentraleInConflitto.DataInserimentoVaccinazioneEseguita) Then
                        idVacVincitore = vacOriginale.ves_id
                        idVacCentraleVincitore = vacCentraleEsistente.Id
                    Else
                        idVacVincitore = vacInConflitto.ves_id
                        idVacCentraleVincitore = vacCentraleInConflitto.Id
                    End If

                Else

                    'Prendo la vaccinazione in centrale con ID inferiore
                    If (vacCentraleEsistente.Id < vacCentraleInConflitto.Id) Then
                        idVacVincitore = vacOriginale.ves_id
                        idVacCentraleVincitore = vacCentraleEsistente.Id
                    Else
                        idVacVincitore = vacInConflitto.ves_id
                        idVacCentraleVincitore = vacCentraleInConflitto.Id
                    End If

                End If

            End If

        End If

        If (idVacVincitore.HasValue AndAlso idVacCentraleVincitore.HasValue) Then

            'Risoluzione dei conflitti
            Using bizPaziente As Biz.BizPaziente = New Biz.BizPaziente(Me.GenericProviderFactory, Me.Settings, Me.ContextInfos, Me.LogOptions)

                Dim risolviConflittoCommand As New Biz.BizPaziente.RisolviConflittoVaccinazioniEseguiteCentraleCommand()
                risolviConflittoCommand.IdVaccinazioniEseguiteCentralePazienteDictionary = New Dictionary(Of Int64, IEnumerable(Of Int64))()

                'Aggiungo l'Id della vaccinazione in centrale in conflitto con la principale
                Dim listIdVaccinazioniEseguiteInConflitto As New List(Of Long)()
                If (vacCentraleEsistente.Id = idVacCentraleVincitore.Value) Then
                    listIdVaccinazioniEseguiteInConflitto.Add(vacCentraleInConflitto.Id)
                Else
                    listIdVaccinazioniEseguiteInConflitto.Add(vacCentraleEsistente.Id)
                End If

                ' Imposto id vaccinazione master e lista id vaccinazioni in conflitto
                risolviConflittoCommand.IdVaccinazioniEseguiteCentralePazienteDictionary.Add(idVacCentraleVincitore.Value,
                                                                                     listIdVaccinazioniEseguiteInConflitto.AsEnumerable())

                bizPaziente.RisolviConflittoVaccinazioniEseguiteCentrale(risolviConflittoCommand)
            End Using

            Return True

        End If

        Return False

    End Function

#End Region

#End Region

#Region " Reports "

    Public Function GetFiltroReportReazioniAvverse(codicePaziente As String, codiceVaccinazione As String, numeroRichiamo As Integer, dataEffettuazione As DateTime, dataReazione As DateTime) As String

        Dim filtro As New System.Text.StringBuilder()

        filtro.AppendFormat(" {{T_STA_REAZIONI_AVVERSE.VRA_PAZ_CODICE}} = {0} ", codicePaziente)

        filtro.AppendFormat(" AND {{T_STA_REAZIONI_AVVERSE.VRA_DATA_REAZIONE}} = DateTime ({0}, {1}, {2}, 00, 00, 00) ",
                            dataReazione.Year,
                            dataReazione.Month,
                            dataReazione.Day)

        filtro.AppendFormat(" AND {{T_STA_REAZIONI_AVVERSE.VRA_VAC_CODICE}} = '{0}'", codiceVaccinazione)
        filtro.AppendFormat(" AND {{T_STA_REAZIONI_AVVERSE.VRA_N_RICHIAMO}} = {0} ", numeroRichiamo)

        filtro.AppendFormat(" AND {{T_STA_REAZIONI_AVVERSE.VRA_RES_DATA_EFFETTUAZIONE}} = DateTime ({0}, {1}, {2}, 00, 00, 00) ",
                            dataEffettuazione.Year,
                            dataEffettuazione.Month,
                            dataEffettuazione.Day)

        Return filtro.ToString()

    End Function

#End Region

#Region " Private "

    Private Function CanSaveVaccinazioneEseguita(vaccinazioneEseguita As VaccinazioneEseguita, excludeSame As Boolean, scaduta As Boolean) As CanSaveVaccinazioneEseguitaResult

        Dim success As Boolean = True
        Dim resultMessageEnumerable As IEnumerable(Of BizResult.ResultMessage) = {}
        Dim idVaccinazioneEseguitaEsistente As Int64? = Nothing
        Dim isVaccinazioneEseguitaEsistenteScaduta As Boolean = False

        Dim vaccinazioneEseguitaEsistente As VaccinazioneEseguita = Nothing

        If scaduta Then

            vaccinazioneEseguitaEsistente = GenericProvider.VaccinazioniEseguite.GetVaccinazioneEseguitaPaziente(
                vaccinazioneEseguita.paz_codice,
                vaccinazioneEseguita.ves_vac_codice,
                vaccinazioneEseguita.ves_n_richiamo,
                vaccinazioneEseguita.ves_data_effettuazione)

        Else

            vaccinazioneEseguitaEsistente = GenericProvider.VaccinazioniEseguite.GetVaccinazioneEseguitaPaziente(
                vaccinazioneEseguita.paz_codice,
                vaccinazioneEseguita.ves_vac_codice,
                vaccinazioneEseguita.ves_n_richiamo)

        End If

        If Not vaccinazioneEseguitaEsistente Is Nothing AndAlso (Not vaccinazioneEseguita.ves_id.HasValue OrElse (Not excludeSame OrElse vaccinazioneEseguita.ves_id <> vaccinazioneEseguitaEsistente.ves_id)) Then
            idVaccinazioneEseguitaEsistente = vaccinazioneEseguitaEsistente.ves_id
            success = False
        End If

        If success Then

            vaccinazioneEseguitaEsistente = GenericProvider.VaccinazioniEseguite.GetVaccinazioneEseguitaScadutaPaziente(
                vaccinazioneEseguita.paz_codice,
                vaccinazioneEseguita.ves_vac_codice,
                vaccinazioneEseguita.ves_n_richiamo,
                vaccinazioneEseguita.ves_data_effettuazione)

            If Not vaccinazioneEseguitaEsistente Is Nothing AndAlso (Not vaccinazioneEseguita.ves_id.HasValue OrElse (Not excludeSame OrElse vaccinazioneEseguita.ves_id <> vaccinazioneEseguitaEsistente.ves_id)) Then
                idVaccinazioneEseguitaEsistente = vaccinazioneEseguitaEsistente.ves_id
                isVaccinazioneEseguitaEsistenteScaduta = True
                success = False
            End If

        End If

        Return New CanSaveVaccinazioneEseguitaResult(success, idVaccinazioneEseguitaEsistente,
            isVaccinazioneEseguitaEsistenteScaduta, resultMessageEnumerable, vaccinazioneEseguitaEsistente)

    End Function

    Private Function CreateVaccinazioneEseguitaFromDataRow(row As DataRow) As VaccinazioneEseguita

        Dim dataRowVersion As DataRowVersion
        If row.RowState = DataRowState.Deleted Then
            dataRowVersion = System.Data.DataRowVersion.Original
        Else
            dataRowVersion = System.Data.DataRowVersion.Current
        End If

        Dim vaccinazioneEseguita As New VaccinazioneEseguita()

        If Not row("VES_ID", dataRowVersion) Is DBNull.Value Then vaccinazioneEseguita.ves_id = Convert.ToInt64(row("VES_ID", dataRowVersion))
        vaccinazioneEseguita.paz_codice = Convert.ToInt32(row("PAZ_CODICE", dataRowVersion))
        If Not row("VES_CNV_DATA", dataRowVersion) Is DBNull.Value Then vaccinazioneEseguita.cnv_data = row("VES_CNV_DATA", dataRowVersion)
        vaccinazioneEseguita.ves_vac_codice = row("VES_VAC_CODICE", dataRowVersion).ToString()
        vaccinazioneEseguita.ves_n_richiamo = Convert.ToInt32(row("VES_N_RICHIAMO", dataRowVersion))
        vaccinazioneEseguita.ves_dataora_effettuazione = row("VES_DATAORA_EFFETTUAZIONE", dataRowVersion)
        vaccinazioneEseguita.ves_sii_codice = row("VES_SII_CODICE", dataRowVersion).ToString()
        vaccinazioneEseguita.ves_vii_codice = row("VES_VII_CODICE", dataRowVersion).ToString()
        vaccinazioneEseguita.ves_luogo = row("VES_LUOGO", dataRowVersion).ToString()
        vaccinazioneEseguita.ves_ope_codice = row("VES_OPE_CODICE", dataRowVersion).ToString()
        vaccinazioneEseguita.ves_med_vaccinante_codice = row("VES_MED_VACCINANTE", dataRowVersion).ToString()

        If Not row("VES_DATA_REGISTRAZIONE", dataRowVersion) Is DBNull.Value Then
            vaccinazioneEseguita.ves_data_registrazione = row("VES_DATA_REGISTRAZIONE", dataRowVersion)
        End If
        If Not row("VES_UTE_ID", dataRowVersion) Is DBNull.Value Then
            vaccinazioneEseguita.ves_ute_id = Convert.ToInt64(row("VES_UTE_ID", dataRowVersion))
        End If
        If Not row("VES_UTE_ID_ULTIMA_VARIAZIONE", dataRowVersion) Is DBNull.Value Then
            vaccinazioneEseguita.ves_ute_id_ultima_variazione = Convert.ToInt64(row("VES_UTE_ID_ULTIMA_VARIAZIONE", dataRowVersion))
        End If
        If Not row("VES_DATA_ULTIMA_VARIAZIONE", dataRowVersion) Is DBNull.Value Then
            vaccinazioneEseguita.ves_data_ultima_variazione = row("VES_DATA_ULTIMA_VARIAZIONE", dataRowVersion)
        End If
        vaccinazioneEseguita.ves_noc_codice = row("VES_NOC_CODICE", dataRowVersion).ToString()
        vaccinazioneEseguita.ves_lot_codice = row("VES_LOT_CODICE", dataRowVersion).ToString()

        If Not row("VES_N_SEDUTA", dataRowVersion) Is DBNull.Value Then
            vaccinazioneEseguita.ves_n_seduta = Convert.ToInt32(row("VES_N_SEDUTA", dataRowVersion))
        End If

        vaccinazioneEseguita.ves_cic_codice = row("VES_CIC_CODICE", dataRowVersion).ToString()
        vaccinazioneEseguita.ves_ass_codice = row("VES_ASS_CODICE", dataRowVersion).ToString()

        If Not row("VES_ASS_N_DOSE", dataRowVersion) Is DBNull.Value Then
            vaccinazioneEseguita.ves_ass_n_dose = Convert.ToInt32(row("VES_ASS_N_DOSE", dataRowVersion))
        End If

        vaccinazioneEseguita.ves_stato = row("VES_STATO", dataRowVersion).ToString()
        vaccinazioneEseguita.ves_comune_o_stato = row("VES_COMUNE_O_STATO", dataRowVersion).ToString()
        vaccinazioneEseguita.ves_in_campagna = row("VES_IN_CAMPAGNA", dataRowVersion).ToString()
        vaccinazioneEseguita.ves_ope_in_ambulatorio = row("VES_OPE_IN_AMBULATORIO", dataRowVersion).ToString()
        vaccinazioneEseguita.ves_cns_codice = row("VES_CNS_CODICE", dataRowVersion).ToString()
        If Not row("VES_PAZ_CODICE_OLD", dataRowVersion) Is DBNull.Value Then
            vaccinazioneEseguita.ves_paz_codice_old = Convert.ToInt32(row("VES_PAZ_CODICE_OLD", dataRowVersion))
        End If
        vaccinazioneEseguita.ves_esito = row("VES_ESITO", dataRowVersion).ToString()
        If Not row("VES_FLAG_FITTIZIA", dataRowVersion) Is DBNull.Value Then
            vaccinazioneEseguita.ves_flag_fittizia = row("VES_FLAG_FITTIZIA", dataRowVersion).ToString()
        Else
            vaccinazioneEseguita.ves_flag_fittizia = "N"
        End If
        vaccinazioneEseguita.ves_note = row("VES_NOTE", dataRowVersion).ToString()
        vaccinazioneEseguita.ves_cns_registrazione = row("VES_CNS_REGISTRAZIONE", dataRowVersion).ToString()
        vaccinazioneEseguita.ves_accesso = row("VES_ACCESSO", dataRowVersion).ToString()
        If Not row("VES_AMB_CODICE", dataRowVersion) Is DBNull.Value Then
            vaccinazioneEseguita.ves_amb_codice = Convert.ToInt32(row("VES_AMB_CODICE", dataRowVersion))
        End If
        If Not row("VES_IMPORTO", dataRowVersion) Is DBNull.Value Then
            vaccinazioneEseguita.ves_importo = Convert.ToDecimal(row("VES_IMPORTO", dataRowVersion))
        End If
        If Not row("VES_MAL_CODICE_MALATTIA", dataRowVersion) Is DBNull.Value Then
            vaccinazioneEseguita.ves_mal_codice_malattia = row("VES_MAL_CODICE_MALATTIA", dataRowVersion).ToString()
        End If
        If Not row("VES_CODICE_ESENZIONE", dataRowVersion) Is DBNull.Value Then
            vaccinazioneEseguita.ves_codice_esenzione = row("VES_CODICE_ESENZIONE", dataRowVersion).ToString()
        End If
        If Not row("VES_CNV_DATA_PRIMO_APP", dataRowVersion) Is DBNull.Value Then
            vaccinazioneEseguita.ves_cnv_data_primo_app = row("VES_CNV_DATA_PRIMO_APP", dataRowVersion)
        End If
        If Not row("VES_ASS_PROG", dataRowVersion) Is DBNull.Value Then
            vaccinazioneEseguita.ves_ass_prog = row("VES_ASS_PROG", dataRowVersion)
        End If

        vaccinazioneEseguita.ass_descrizione = row("ASS_DESCRIZIONE", dataRowVersion).ToString()

        If Not row("LOT_DATA_SCADENZA", dataRowVersion) Is DBNull.Value Then
            vaccinazioneEseguita.lot_data_scadenza = row("LOT_DATA_SCADENZA", dataRowVersion)
        End If

        vaccinazioneEseguita.noc_descrizione = row("NOC_DESCRIZIONE", dataRowVersion).ToString()
        vaccinazioneEseguita.sii_descrizione = row("SII_DESCRIZIONE", dataRowVersion).ToString()
        vaccinazioneEseguita.vac_descrizione = row("VAC_DESCRIZIONE", dataRowVersion).ToString()
        vaccinazioneEseguita.ves_accesso = row("VES_ACCESSO", dataRowVersion).ToString()

        vaccinazioneEseguita.ves_usl_inserimento = row("VES_USL_INSERIMENTO", dataRowVersion).ToString()
        vaccinazioneEseguita.ves_usl_scadenza = row("VES_USL_SCADENZA", dataRowVersion).ToString()

        If Not row("VES_DATA_SCADENZA", dataRowVersion) Is DBNull.Value Then vaccinazioneEseguita.ves_data_scadenza = row("VES_DATA_SCADENZA", dataRowVersion)
        If Not row("VES_UTE_ID_SCADENZA", dataRowVersion) Is DBNull.Value Then vaccinazioneEseguita.ves_ute_id_scadenza = Convert.ToInt64(row("VES_UTE_ID_SCADENZA", dataRowVersion))

        If Not row("VES_DATA_RIPRISTINO", dataRowVersion) Is DBNull.Value Then vaccinazioneEseguita.ves_data_ripristino = row("VES_DATA_RIPRISTINO", dataRowVersion)
        If Not row("VES_UTE_ID_RIPRISTINO", dataRowVersion) Is DBNull.Value Then vaccinazioneEseguita.ves_ute_id_ripristino = Convert.ToInt64(row("VES_UTE_ID_RIPRISTINO", dataRowVersion))

        vaccinazioneEseguita.ves_flag_visibilita_vac_centrale = row("VES_FLAG_VISIBILITA", dataRowVersion).ToString()
        vaccinazioneEseguita.ves_note_acquisizione_vac_centrale = row("VES_NOTE_ACQUISIZIONE", dataRowVersion).ToString()
        vaccinazioneEseguita.ves_id_acn = row("VES_ID_ACN", dataRowVersion).ToString()
        vaccinazioneEseguita.ves_provenienza = row("VES_PROVENIENZA", dataRowVersion).ToString()

        vaccinazioneEseguita.ves_mal_codice_cond_sanitaria = row("VES_MAL_CODICE_COND_SANITARIA", dataRowVersion).ToString()
        vaccinazioneEseguita.ves_rsc_codice = row("VES_RSC_CODICE", dataRowVersion).ToString()
        If Not row("VES_TPA_GUID_TIPI_PAGAMENTO", dataRowVersion) Is DBNull.Value Then vaccinazioneEseguita.ves_tpa_guid_tipi_pagamento = New Guid(DirectCast(row("VES_TPA_GUID_TIPI_PAGAMENTO", dataRowVersion), Byte()))
        If Not row("VES_LOT_DATA_SCADENZA", dataRowVersion) Is DBNull.Value Then vaccinazioneEseguita.ves_lot_data_scadenza = row("VES_LOT_DATA_SCADENZA", dataRowVersion)
        vaccinazioneEseguita.ves_antigene = row("VES_ANTIGENE", dataRowVersion).ToString()
        vaccinazioneEseguita.ves_tipo_erogatore = row("VES_TIPO_EROGATORE", dataRowVersion).ToString()
        vaccinazioneEseguita.ves_tipo_associazione_acn = row("VES_TIPO_ASSOCIAZIONE_ACN", dataRowVersion).ToString()

        vaccinazioneEseguita.ves_codice_struttura = row("VES_CODICE_STRUTTURA", dataRowVersion).ToString()
        vaccinazioneEseguita.ves_usl_cod_somministrazione = row("VES_USL_COD_SOMMINISTRAZIONE", dataRowVersion).ToString()

        Return vaccinazioneEseguita

    End Function

    Private Function CreateReazioneAvversaFromDataRow(vaccinazioneEseguita As VaccinazioneEseguita, row As DataRow) As ReazioneAvversa

        Dim reazioneAvversa As ReazioneAvversa = Nothing

        Dim dataRowVersion As DataRowVersion
        If row.RowState = DataRowState.Deleted Then
            dataRowVersion = System.Data.DataRowVersion.Original
        Else
            dataRowVersion = System.Data.DataRowVersion.Current
        End If

        If Not row("vra_data_reazione", dataRowVersion) Is DBNull.Value Then

            reazioneAvversa = New ReazioneAvversa()

            reazioneAvversa.CodicePaziente = vaccinazioneEseguita.paz_codice
            If vaccinazioneEseguita.ves_paz_codice_old > 0 Then reazioneAvversa.CodicePazientePrecedente = vaccinazioneEseguita.ves_paz_codice_old
            reazioneAvversa.CodiceVaccinazione = vaccinazioneEseguita.ves_vac_codice
            reazioneAvversa.NumeroRichiamo = vaccinazioneEseguita.ves_n_richiamo
            reazioneAvversa.DataEffettuazione = Convert.ToDateTime(vaccinazioneEseguita.ves_data_effettuazione)
            reazioneAvversa.DescrizioneNomeCommerciale = vaccinazioneEseguita.noc_descrizione
            reazioneAvversa.CodiceLotto = vaccinazioneEseguita.ves_lot_codice
            reazioneAvversa.DataScadenzaLotto = vaccinazioneEseguita.lot_data_scadenza
            reazioneAvversa.CodiceSitoInoculazione = vaccinazioneEseguita.ves_sii_codice
            reazioneAvversa.OraEffettuazione = vaccinazioneEseguita.ves_ora_effettuazione
            ' reazioneAvversa.FlagVisibilita = vaccinazioneEseguita.ves_flag_visibilita_vac_centrale

            reazioneAvversa.DataReazione = row("vra_data_reazione", dataRowVersion)
            reazioneAvversa.CodiceReazione = row("vra_rea_codice", dataRowVersion).ToString()
            reazioneAvversa.CodiceReazione1 = row("vra_re1_codice", dataRowVersion).ToString()
            reazioneAvversa.CodiceReazione2 = row("vra_re2_codice", dataRowVersion).ToString()
            reazioneAvversa.AltraReazione = row("vra_rea_altro", dataRowVersion).ToString()
            reazioneAvversa.VisiteRicoveri = row("vra_visita", dataRowVersion).ToString()
            reazioneAvversa.Terapie = row("vra_terapia", dataRowVersion).ToString()
            reazioneAvversa.Grave = row("vra_grave", dataRowVersion).ToString()
            reazioneAvversa.GravitaReazione = row("vra_gravita_reazione", dataRowVersion).ToString()
            reazioneAvversa.Esito = row("vra_esito", dataRowVersion).ToString()
            reazioneAvversa.MotivoDecesso = row("vra_motivo_decesso", dataRowVersion).ToString()
            reazioneAvversa.CodiceEsito = row("vra_esi_codice", dataRowVersion).ToString()
            If Not row("vra_data_esito", dataRowVersion) Is DBNull.Value Then reazioneAvversa.DataEsito = row("vra_data_esito", dataRowVersion)
            reazioneAvversa.Dosaggio = row("vra_dosaggio", dataRowVersion).ToString()
            reazioneAvversa.Sospeso = row("vra_sospeso", dataRowVersion).ToString()
            reazioneAvversa.Migliorata = row("vra_migliorata", dataRowVersion).ToString()
            reazioneAvversa.Ripreso = row("vra_ripreso", dataRowVersion).ToString()
            reazioneAvversa.Ricomparsa = row("vra_ricomparsa", dataRowVersion).ToString()
            reazioneAvversa.Indicazioni = row("vra_indicazioni", dataRowVersion).ToString()
            reazioneAvversa.CodiceIndicazioni = row("vra_noi_codice_indicazioni", dataRowVersion).ToString()
            If Not row("vra_richiamo", dataRowVersion) Is DBNull.Value Then reazioneAvversa.Richiamo = row("vra_richiamo", dataRowVersion)
            reazioneAvversa.LuogoDescrizione = row("vra_luogo", dataRowVersion).ToString()
            reazioneAvversa.AltroLuogoDescrizione = row("vra_altro_luogo", dataRowVersion).ToString()
            reazioneAvversa.FarmacoConcomitante = row("vra_farmaco_concomitante", dataRowVersion).ToString()
            reazioneAvversa.FarmacoDescrizione = row("vra_farmaco_descrizione", dataRowVersion).ToString()
            reazioneAvversa.UsoConcomitante = row("vra_uso_concomitante", dataRowVersion).ToString()
            reazioneAvversa.CondizioniConcomitanti = row("vra_condizioni_concomitanti", dataRowVersion).ToString()
            reazioneAvversa.Qualifica = row("vra_qualifica", dataRowVersion).ToString()
            reazioneAvversa.AltraQualifica = row("vra_altra_qualifica", dataRowVersion).ToString()
            reazioneAvversa.CognomeSegnalatore = row("vra_cognome_segnalatore", dataRowVersion).ToString()
            reazioneAvversa.NomeSegnalatore = row("vra_nome_segnalatore", dataRowVersion).ToString()
            reazioneAvversa.IndirizzoSegnalatore = row("vra_indirizzo_segnalatore", dataRowVersion).ToString()
            reazioneAvversa.TelSegnalatore = row("vra_tel_segnalatore", dataRowVersion).ToString()
            reazioneAvversa.FaxSegnalatore = row("vra_fax_segnalatore", dataRowVersion).ToString()
            reazioneAvversa.MailSegnalatore = row("vra_mail_segnalatore", dataRowVersion).ToString()
            If Not row("paz_data_decesso", dataRowVersion) Is DBNull.Value Then reazioneAvversa.DataDecesso = row("paz_data_decesso", dataRowVersion)
            If Not row("paz_stato_anagrafico", dataRowVersion) Is DBNull.Value Then reazioneAvversa.StatoAnagrafico = [Enum].Parse(GetType(Enumerators.StatoAnagrafico), row("paz_stato_anagrafico", dataRowVersion).ToString())
            reazioneAvversa.DescrizioneReazione = row("rea_descrizione", dataRowVersion).ToString()
            reazioneAvversa.DescrizioneReazione1 = row("rea_descrizione1", dataRowVersion).ToString()
            reazioneAvversa.DescrizioneReazione2 = row("rea_descrizione2", dataRowVersion).ToString()
            If Not row("vra_data_compilazione", dataRowVersion) Is DBNull.Value Then reazioneAvversa.DataCompilazione = row("vra_data_compilazione", dataRowVersion)
            If Not row("vra_ute_id_compilazione", dataRowVersion) Is DBNull.Value Then reazioneAvversa.IdUtenteCompilazione = Convert.ToInt64(row("vra_ute_id_compilazione", dataRowVersion))
            If Not row("VRA_DATA_VARIAZIONE", dataRowVersion) Is DBNull.Value Then reazioneAvversa.DataModifica = row("VRA_DATA_VARIAZIONE", dataRowVersion)
            If Not row("VRA_UTE_ID_VARIAZIONE", dataRowVersion) Is DBNull.Value Then reazioneAvversa.IdUtenteModifica = Convert.ToInt64(row("VRA_UTE_ID_VARIAZIONE", dataRowVersion))

            reazioneAvversa.CodiceUslInserimento = row("vra_usl_inserimento", dataRowVersion).ToString()
            reazioneAvversa.DescrizioneUslInserimento = row("usl_inserimento_vra_descr", dataRowVersion).ToString()

            If Not row("ves_id", dataRowVersion) Is DBNull.Value Then reazioneAvversa.IdVaccinazioneEseguita = Convert.ToInt64(row("ves_id", dataRowVersion))
            If Not row("vra_id", dataRowVersion) Is DBNull.Value Then reazioneAvversa.IdReazioneAvversa = Convert.ToInt64(row("vra_id", dataRowVersion))

            reazioneAvversa.AltreInformazioni = row("vra_altre_informazioni", dataRowVersion).ToString()
            reazioneAvversa.AmbitoOsservazione = row("vra_ambito_osservazione", dataRowVersion).ToString()
            reazioneAvversa.AmbitoOsservazione_Studio_Titolo = row("vra_ambito_studio_titolo", dataRowVersion).ToString()
            reazioneAvversa.AmbitoOsservazione_Studio_Tipologia = row("vra_ambito_studio_tipologia", dataRowVersion).ToString()
            reazioneAvversa.AmbitoOsservazione_Studio_Numero = row("vra_ambito_studio_numero", dataRowVersion).ToString()
            If Not row("vra_peso", dataRowVersion) Is DBNull.Value Then reazioneAvversa.Peso = Convert.ToDouble(row("vra_peso", dataRowVersion))
            If Not row("vra_altezza", dataRowVersion) Is DBNull.Value Then reazioneAvversa.Altezza = Convert.ToInt32(row("vra_altezza", dataRowVersion))
            If Not row("vra_data_ultima_mestruazione", dataRowVersion) Is DBNull.Value Then reazioneAvversa.DataUltimaMestruazione = Convert.ToDateTime(row("vra_data_ultima_mestruazione", dataRowVersion))
            reazioneAvversa.Allattamento = row("vra_allattamento", dataRowVersion).ToString()
            reazioneAvversa.Gravidanza = row("vra_gravidanza", dataRowVersion).ToString()
            reazioneAvversa.CausaReazioneOsservata = row("vra_causa_osservata", dataRowVersion).ToString()

            reazioneAvversa.FarmacoConcomitante1_CodiceNomeCommerciale = row("vra_farmconc1_noc_codice", dataRowVersion).ToString()
            reazioneAvversa.FarmacoConcomitante1_DescrizioneNomeCommerciale = row("vra_farmconc1_noc_descrizione", dataRowVersion).ToString()
            reazioneAvversa.FarmacoConcomitante1_CodiceLotto = row("vra_farmconc1_lot_codice", dataRowVersion).ToString()
            If row("vra_farmconc1_lot_data_scad", dataRowVersion) Is DBNull.Value Then
                reazioneAvversa.FarmacoConcomitante1_DataScadenzaLotto = Nothing
            Else
                reazioneAvversa.FarmacoConcomitante1_DataScadenzaLotto = row("vra_farmconc1_lot_data_scad", dataRowVersion)
            End If


            If row("vra_farmconc1_dataora_eff", dataRowVersion) Is DBNull.Value Then
                reazioneAvversa.FarmacoConcomitante1_DataOraEffettuazione = Nothing
            Else
                reazioneAvversa.FarmacoConcomitante1_DataOraEffettuazione = Convert.ToDateTime(row("vra_farmconc1_dataora_eff", dataRowVersion))
            End If

            If row("vra_farmconc1_dose", dataRowVersion) Is DBNull.Value Then
                reazioneAvversa.FarmacoConcomitante1_Dose = Nothing
            Else
                reazioneAvversa.FarmacoConcomitante1_Dose = Convert.ToInt32(row("vra_farmconc1_dose", dataRowVersion))
            End If

            reazioneAvversa.FarmacoConcomitante1_CodiceSitoInoculazione = row("vra_farmconc1_sii_codice", dataRowVersion).ToString()
            reazioneAvversa.FarmacoConcomitante1_CodiceViaSomministrazione = row("vra_farmconc1_vii_codice", dataRowVersion).ToString()
            reazioneAvversa.FarmacoConcomitante1_Sospeso = row("vra_farmconc1_sospeso", dataRowVersion).ToString()
            reazioneAvversa.FarmacoConcomitante1_Migliorata = row("vra_farmconc1_migliorata", dataRowVersion).ToString()
            reazioneAvversa.FarmacoConcomitante1_Ripreso = row("vra_farmconc1_ripreso", dataRowVersion).ToString()
            reazioneAvversa.FarmacoConcomitante1_Ricomparsa = row("vra_farmconc1_ricomparsa", dataRowVersion).ToString()
            reazioneAvversa.FarmacoConcomitante1_Indicazioni = row("vra_farmconc1_indicazioni", dataRowVersion).ToString()
            reazioneAvversa.FarmacoConcomitante1_CodiceIndicazioni = row("vra_farmconc1_noi_cod_indic", dataRowVersion).ToString()
            reazioneAvversa.FarmacoConcomitante1_Dosaggio = row("vra_farmconc1_dosaggio", dataRowVersion).ToString()

            If row("vra_farmconc1_richiamo", dataRowVersion) Is DBNull.Value Then
                reazioneAvversa.FarmacoConcomitante1_Richiamo = Nothing
            Else
                reazioneAvversa.FarmacoConcomitante1_Richiamo = Convert.ToInt32(row("vra_farmconc1_richiamo", dataRowVersion))
            End If

            reazioneAvversa.FarmacoConcomitante2_CodiceNomeCommerciale = row("vra_farmconc2_noc_codice", dataRowVersion).ToString()
            reazioneAvversa.FarmacoConcomitante2_DescrizioneNomeCommerciale = row("vra_farmconc2_noc_descrizione", dataRowVersion).ToString()
            reazioneAvversa.FarmacoConcomitante2_CodiceLotto = row("vra_farmconc2_lot_codice", dataRowVersion).ToString()
            If row("vra_farmconc2_lot_data_scad", dataRowVersion) Is DBNull.Value Then
                reazioneAvversa.FarmacoConcomitante2_DataScadenzaLotto = Nothing
            Else
                reazioneAvversa.FarmacoConcomitante2_DataScadenzaLotto = row("vra_farmconc2_lot_data_scad", dataRowVersion)
            End If
            If row("vra_farmconc2_dataora_eff", dataRowVersion) Is DBNull.Value Then
                reazioneAvversa.FarmacoConcomitante2_DataOraEffettuazione = Nothing
            Else
                reazioneAvversa.FarmacoConcomitante2_DataOraEffettuazione = Convert.ToDateTime(row("vra_farmconc2_dataora_eff", dataRowVersion))
            End If

            If row("vra_farmconc2_dose", dataRowVersion) Is DBNull.Value Then
                reazioneAvversa.FarmacoConcomitante2_Dose = Nothing
            Else
                reazioneAvversa.FarmacoConcomitante2_Dose = Convert.ToInt32(row("vra_farmconc2_dose", dataRowVersion))
            End If

            reazioneAvversa.FarmacoConcomitante2_CodiceSitoInoculazione = row("vra_farmconc2_sii_codice", dataRowVersion).ToString()
            reazioneAvversa.FarmacoConcomitante2_CodiceViaSomministrazione = row("vra_farmconc2_vii_codice", dataRowVersion).ToString()
            reazioneAvversa.FarmacoConcomitante2_Sospeso = row("vra_farmconc2_sospeso", dataRowVersion).ToString()
            reazioneAvversa.FarmacoConcomitante2_Migliorata = row("vra_farmconc2_migliorata", dataRowVersion).ToString()
            reazioneAvversa.FarmacoConcomitante2_Ripreso = row("vra_farmconc2_ripreso", dataRowVersion).ToString()
            reazioneAvversa.FarmacoConcomitante2_Ricomparsa = row("vra_farmconc2_ricomparsa", dataRowVersion).ToString()
            reazioneAvversa.FarmacoConcomitante2_Indicazioni = row("vra_farmconc2_indicazioni", dataRowVersion).ToString()
            reazioneAvversa.FarmacoConcomitante2_CodiceIndicazioni = row("vra_farmconc2_noi_cod_indic", dataRowVersion).ToString()
            reazioneAvversa.FarmacoConcomitante2_Dosaggio = row("vra_farmconc2_dosaggio", dataRowVersion).ToString()

            If row("vra_farmconc2_richiamo", dataRowVersion) Is DBNull.Value Then
                reazioneAvversa.FarmacoConcomitante2_Richiamo = Nothing
            Else
                reazioneAvversa.FarmacoConcomitante2_Richiamo = Convert.ToInt32(row("vra_farmconc2_richiamo", dataRowVersion))
            End If

            reazioneAvversa.FirmaSegnalatore = row("vra_firma_segnalatore", dataRowVersion).ToString()
            reazioneAvversa.CodiceOrigineEtnica = row("vra_oet_codice", dataRowVersion).ToString()
            reazioneAvversa.IdScheda = row("vra_id_scheda", dataRowVersion).ToString()
            reazioneAvversa.SegnalazioneId = row("vra_segnalazione_id", dataRowVersion).ToString()
            reazioneAvversa.VesAssCodice = vaccinazioneEseguita.ves_ass_codice
            reazioneAvversa.VesAssNDose = vaccinazioneEseguita.ves_ass_n_dose
            If row("vra_data_invio", dataRowVersion) Is DBNull.Value Then
                reazioneAvversa.DataInvio = Nothing
            Else
                reazioneAvversa.DataInvio = Convert.ToDateTime(row("vra_data_invio", dataRowVersion))
            End If
            If row("vra_ute_id_invio", dataRowVersion) Is DBNull.Value Then
                reazioneAvversa.UtenteInvio = Nothing
            Else
                reazioneAvversa.UtenteInvio = Convert.ToInt32(row("vra_ute_id_invio", dataRowVersion))
            End If
            If row("vra_data_invio", dataRowVersion) Is DBNull.Value Then
                reazioneAvversa.FlagInviato = "N"
            Else
                reazioneAvversa.FlagInviato = row("vra_flag_inviato", dataRowVersion).ToString()
            End If

        End If

        Return reazioneAvversa

    End Function

    Private Function InsertVaccinazioneEseguita(vaccinazioneEseguita As VaccinazioneEseguita, scaduta As Boolean) As Boolean

        Dim success As Boolean

        SetVaccinazioneEseguitaAddedContextInfosIfNeeded(vaccinazioneEseguita)

        If scaduta Then
            success = GenericProvider.VaccinazioniEseguite.InsertVaccinazioneEseguitaScaduta(vaccinazioneEseguita)
        Else
            success = GenericProvider.VaccinazioniEseguite.InsertVaccinazioneEseguita(vaccinazioneEseguita)
        End If

        Return success

    End Function

    Private Function InsertVaccinazioneEseguita(vaccinazioneEseguita As VaccinazioneEseguita, row As DataRow) As Boolean

        Dim success As Boolean = InsertVaccinazioneEseguita(vaccinazioneEseguita, False)

        If success Then

            Dim testataLog As New Testata(TipiArgomento.VAC_ESEGUITE, Operazione.Inserimento)
            testataLog.Records.Add(LogBox.GetRecordFromRow(row))

            LogBox.WriteData(testataLog)

        End If

        Return success

    End Function

    Private Function InsertVaccinazioneEseguitaScaduta(vaccinazioneEseguitaScaduta As VaccinazioneEseguita, row As DataRow) As Boolean

        Dim success As Boolean = InsertVaccinazioneEseguita(vaccinazioneEseguitaScaduta, True)

        If success Then

            Dim t As New Testata(TipiArgomento.VAC_SCADUTE, Operazione.Inserimento)
            Dim r As Record = LogBox.GetRecordFromRow(row)

            t.Records.Add(r)

            LogBox.WriteData(t)

        End If

        Return success

    End Function

    Private Function UpdateVaccinazioneEseguita(vaccinazioneEseguita As VaccinazioneEseguita, scaduta As Boolean) As Boolean

        Dim success As Boolean

        SetVaccinazioneEseguitaModifiedContextInfosIfNeeded(vaccinazioneEseguita)

        If scaduta Then
            success = GenericProvider.VaccinazioniEseguite.UpdateVaccinazioneEseguitaScaduta(vaccinazioneEseguita)
        Else
            success = GenericProvider.VaccinazioniEseguite.UpdateVaccinazioneEseguita(vaccinazioneEseguita)
        End If

        Return success

    End Function

    Private Function UpdateVaccinazioneEseguita(vaccinazioneEseguita As VaccinazioneEseguita, row As DataRow) As Boolean

        Dim success As Boolean = UpdateVaccinazioneEseguita(vaccinazioneEseguita, False)

        If success Then

            Dim testataLog As New Testata(TipiArgomento.VAC_ESEGUITE, Operazione.Modifica)
            testataLog.Records.Add(LogBox.GetRecordFromRow(row))

            If testataLog.ChangedValues Then LogBox.WriteData(testataLog)

        End If

        Return success

    End Function

    Private Function UpdateVaccinazioneEseguitaScaduta(vaccinazioneEseguitaScaduta As VaccinazioneEseguita, row As DataRow) As Boolean

        Dim success As Boolean = UpdateVaccinazioneEseguita(vaccinazioneEseguitaScaduta, True)

        If success Then

            Dim testataLog As New Testata(TipiArgomento.VAC_SCADUTE, Operazione.Modifica)
            testataLog.Records.Add(LogBox.GetRecordFromRow(row))

            If testataLog.ChangedValues Then LogBox.WriteData(testataLog)

        End If

        Return success

    End Function

    Private Function DeleteVaccinazioneEseguita(vaccinazioneEseguitaEliminata As VaccinazioneEseguita, scaduta As Boolean, writeLogIfNeeded As Boolean) As Boolean

        Dim success As Boolean

        If scaduta Then

            success = GenericProvider.VaccinazioniEseguite.DeleteVaccinazioneEseguitaScadutaById(vaccinazioneEseguitaEliminata.ves_id)

            If success Then

                If writeLogIfNeeded Then

                    Dim testataLog As New Testata(TipiArgomento.VAC_SCADUTE, Operazione.Eliminazione)
                    Dim recordLog As New Record()

                    recordLog.Campi.AddIfChanged(New Campo("VSC_VAC_CODICE", vaccinazioneEseguitaEliminata.ves_vac_codice.ToString()))
                    recordLog.Campi.AddIfChanged(New Campo("VSC_N_RICHIAMO", vaccinazioneEseguitaEliminata.ves_n_richiamo.ToString()))
                    recordLog.Campi.AddIfChanged(New Campo("VSC_DATA_EFFETTUAZIONE", vaccinazioneEseguitaEliminata.ves_data_effettuazione.ToString()))

                    testataLog.Records.Add(recordLog)

                    'LogBox.WriteData(testataLog)

                End If

                InsertVaccinazioneEseguitaScadutaEliminata(vaccinazioneEseguitaEliminata)

            End If

        Else

            success = GenericProvider.VaccinazioniEseguite.DeleteVaccinazioneEseguitaById(vaccinazioneEseguitaEliminata.ves_id)

            If success Then

                If writeLogIfNeeded Then

                    Dim testataLog As New Testata(TipiArgomento.VAC_ESEGUITE, Operazione.Eliminazione)
                    Dim recordLog As New Record()

                    recordLog.Campi.AddIfChanged(New Campo("VES_VAC_CODICE", vaccinazioneEseguitaEliminata.ves_vac_codice.ToString()))
                    recordLog.Campi.AddIfChanged(New Campo("VES_DATA_EFFETTUAZIONE", vaccinazioneEseguitaEliminata.ves_data_effettuazione.ToString()))
                    recordLog.Campi.AddIfChanged(New Campo("VES_N_RICHIAMO", vaccinazioneEseguitaEliminata.ves_n_richiamo.ToString()))

                    testataLog.Records.Add(recordLog)

                    'LogBox.WriteData(testataLog)

                End If

                InsertVaccinazioneEseguitaEliminata(vaccinazioneEseguitaEliminata)

            End If

        End If

    End Function

    Private Function InsertReazioneAvversa(reazioneAvversa As ReazioneAvversa, flagVisibilitaCentrale As String, row As DataRow) As Boolean

        Dim inserted As Boolean = InsertReazioneAvversa(reazioneAvversa, flagVisibilitaCentrale)

        If inserted Then

            Dim t As New Testata(TipiArgomento.REAZ_AVVERSE, Operazione.Inserimento)
            Dim r As Record = LogBox.GetRecordFromRow(row)

            t.Records.Add(r)

            LogBox.WriteData(t)

        End If

        Return inserted

    End Function

    Private Function InsertReazioneAvversaScaduta(reazioneAvversaScaduta As ReazioneAvversa, flagVisibilitaCentrale As String, row As DataRow) As Boolean

        Dim inserted As Boolean = InsertReazioneAvversaScaduta(reazioneAvversaScaduta, flagVisibilitaCentrale)

        If inserted Then

            Dim testataLog As New Testata(TipiArgomento.REAZ_AVVERSE_SCADUTE, Operazione.Inserimento)
            Dim recordLog As Record = LogBox.GetRecordFromRow(row)

            testataLog.Records.Add(recordLog)
            LogBox.WriteData(testataLog)

        End If

        Return inserted

    End Function

    Private Function UpdateReazioneAvversa(reazioneAvversa As ReazioneAvversa, row As DataRow) As Boolean

        Dim updated As Boolean = UpdateReazioneAvversa(reazioneAvversa)

        If updated Then

            Dim testataLog As New Testata(TipiArgomento.REAZ_AVVERSE, Operazione.Modifica)
            testataLog.Records.Add(LogBox.GetRecordFromRow(row))

            If testataLog.ChangedValues Then LogBox.WriteData(testataLog)

        End If

        Return updated

    End Function

    Private Function UpdateReazioneAvversaScaduta(reazioneAvversaScaduta As ReazioneAvversa, row As DataRow) As Boolean

        Dim updated As Boolean = UpdateReazioneAvversaScaduta(reazioneAvversaScaduta)

        If updated Then

            Dim testataLog As New Testata(TipiArgomento.REAZ_AVVERSE, Operazione.Modifica)
            testataLog.Records.Add(LogBox.GetRecordFromRow(row))

            If testataLog.ChangedValues Then LogBox.WriteData(testataLog)

        End If

        Return updated

    End Function

    Private Function DeleteReazioneAvversaScaduta(reazioneAvversaScadutaEliminata As ReazioneAvversa, row As DataRow) As Boolean

        Dim deleted As Boolean = DeleteReazioneAvversaScaduta(reazioneAvversaScadutaEliminata)

        If deleted Then

            Dim testataLog As New Testata(TipiArgomento.REAZ_AVVERSE, Operazione.Eliminazione)
            Dim recordLog As New Record()

            recordLog.Campi.AddIfChanged(New Campo("VRS_VAC_CODICE", row("ves_vac_codice", DataRowVersion.Original).ToString()))
            recordLog.Campi.AddIfChanged(New Campo("VRS_RES_DATA_EFFETTUAZIONE", row("ves_data_effettuazione", DataRowVersion.Original).ToString()))
            recordLog.Campi.AddIfChanged(New Campo("VRS_N_RICHIAMO", row("ves_n_richiamo", DataRowVersion.Original).ToString()))
            recordLog.Campi.AddIfChanged(New Campo("VRS_DATA_REAZIONE", row("vra_data_reazione", DataRowVersion.Original).ToString()))
            recordLog.Campi.AddIfChanged(New Campo("Scaduta", row("scaduta", DataRowVersion.Original).ToString() = "S"))

            testataLog.Records.Add(recordLog)

            LogBox.WriteData(testataLog)

        End If

        Return deleted

    End Function

    Private Function DeleteReazioneAvversa(reazioneAvversaEliminata As ReazioneAvversa, row As DataRow) As Boolean

        Dim deleted As Boolean = DeleteReazioneAvversa(reazioneAvversaEliminata)

        If deleted Then

            Dim testataLog As New Testata(TipiArgomento.REAZ_AVVERSE, Operazione.Eliminazione)
            Dim recordLog As New Record()

            recordLog.Campi.AddIfChanged(New Campo("VRA_VAC_CODICE", row("VES_VAC_CODICE", DataRowVersion.Original).ToString()))
            recordLog.Campi.AddIfChanged(New Campo("VRA_RES_DATA_EFFETTUAZIONE", row("VES_DATA_EFFETTUAZIONE", DataRowVersion.Original).ToString()))
            recordLog.Campi.AddIfChanged(New Campo("VRA_N_RICHIAMO", row("VES_N_RICHIAMO", DataRowVersion.Original).ToString()))
            recordLog.Campi.AddIfChanged(New Campo("VRA_DATA_REAZIONE", row("VRA_DATA_REAZIONE", DataRowVersion.Original).ToString()))
            recordLog.Campi.AddIfChanged(New Campo("Scaduta", row("scaduta", DataRowVersion.Original).ToString() = "S"))

            testataLog.Records.Add(recordLog)

            LogBox.WriteData(testataLog)

        End If

        Return deleted

    End Function

    Private Sub SetVaccinazioneEseguitaAddedContextInfosIfNeeded(vaccinazioneEseguita As VaccinazioneEseguita)

        If Not vaccinazioneEseguita.ves_data_registrazione.HasValue Then
            vaccinazioneEseguita.ves_data_registrazione = DateTime.Now
        End If

        If Not vaccinazioneEseguita.ves_ute_id.HasValue Then
            vaccinazioneEseguita.ves_ute_id = ContextInfos.IDUtente
        End If

        If String.IsNullOrEmpty(vaccinazioneEseguita.ves_usl_inserimento) Then
            vaccinazioneEseguita.ves_usl_inserimento = ContextInfos.CodiceUsl
        End If

    End Sub

    Private Sub SetVaccinazioneEseguitaModifiedContextInfosIfNeeded(vaccinazioneEseguita As VaccinazioneEseguita)

        If Not vaccinazioneEseguita.ves_data_ultima_variazione.HasValue Then
            vaccinazioneEseguita.ves_data_ultima_variazione = DateTime.Now
        End If

        If Not vaccinazioneEseguita.ves_ute_id_ultima_variazione.HasValue Then
            vaccinazioneEseguita.ves_ute_id_ultima_variazione = ContextInfos.IDUtente
        End If

    End Sub

    Private Sub SetVaccinazioneEseguitaDeletedContextInfosIfNeeded(vaccinazioneEseguita As VaccinazioneEseguita)

        If Not vaccinazioneEseguita.DataEliminazione.HasValue Then
            vaccinazioneEseguita.DataEliminazione = DateTime.Now
        End If

        If Not vaccinazioneEseguita.IdUtenteEliminazione.HasValue Then
            vaccinazioneEseguita.IdUtenteEliminazione = ContextInfos.IDUtente
        End If

    End Sub

    Private Sub SetReazioneAvversaAddedContextInfosIfNeeded(reazioneAvversa As ReazioneAvversa, flagVisibilitaCentrale As String)

        If reazioneAvversa.DataCompilazione = DateTime.MinValue Then
            reazioneAvversa.DataCompilazione = DateTime.Now
        End If

        If Not reazioneAvversa.IdUtenteCompilazione.HasValue Then
            reazioneAvversa.IdUtenteCompilazione = ContextInfos.IDUtente
        End If

        If String.IsNullOrEmpty(reazioneAvversa.CodiceUslInserimento) Then
            reazioneAvversa.CodiceUslInserimento = ContextInfos.CodiceUsl
        End If

    End Sub

    Private Sub SetReazioneAvversaModifiedContextInfosIfNeeded(reazioneAvversa As ReazioneAvversa)

        If Not reazioneAvversa.DataModifica.HasValue Then
            reazioneAvversa.DataModifica = DateTime.Now
        End If

        If Not reazioneAvversa.IdUtenteModifica.HasValue Then
            reazioneAvversa.IdUtenteModifica = ContextInfos.IDUtente
        End If

    End Sub

    Private Sub SetReazioneAvversaDeletedContextInfosIfNeeded(reazioneAvversa As ReazioneAvversa)

        If Not reazioneAvversa.DataEliminazione.HasValue Then
            reazioneAvversa.DataEliminazione = DateTime.Now
        End If

        If Not reazioneAvversa.IdUtenteEliminazione.HasValue Then
            reazioneAvversa.IdUtenteEliminazione = ContextInfos.IDUtente
        End If

    End Sub

#End Region


#Region " Types "

    Public Class VaccinazioniEseguiteSalvaResult
        Inherits BizResult

        Public Sub New()
        End Sub

        'Private _VaccinazioniEseguiteCentralizzateEsistenti As DataRow()
        'Public Property VaccinazioniEseguiteCentralizzateEsistenti As DataRow()
        '    Get
        '        Return _VaccinazioniEseguiteCentralizzateEsistenti
        '    End Get
        '    Set(value As DataRow())
        '        _VaccinazioniEseguiteCentralizzateEsistenti = value
        '    End Set
        'End Property
        Private _ReazioneAvversaList As List(Of ReazioneAvversa)
        Public Property ReazioneAvversaList As List(Of ReazioneAvversa)
            Get
                Return _ReazioneAvversaList
            End Get
            Set(value As List(Of ReazioneAvversa))
                _ReazioneAvversaList = value
            End Set
        End Property

    End Class

    'Public Class ReazioniAvverseSalvaResult

    '    Public Property ReazioneAvversaEnumerable As IEnumerable(Of ReazioneAvversa)
    '    Public Property ReazioneAvversaEliminataEnumerable As IEnumerable(Of ReazioneAvversa)

    'End Class

    Public Class ControlloEsecuzione
        Inherits System.Collections.Generic.List(Of ControlloEsecuzioneItem)

        Public ReadOnly Property HasError() As Boolean
            Get
                Dim _hasError As Boolean = False
                For i As Integer = 0 To Me.Count - 1
                    If Me.Item(i).hasError Then
                        _hasError = True
                        Exit For
                    End If
                Next
                Return _hasError
            End Get
        End Property

        Public Function GetAlertJS() As String

            Dim strJS As New System.Text.StringBuilder()

            If Me.HasError Then
                strJS.Append("Impossibile eseguire le seguenti vaccinazioni:\n")
            ElseIf Me.Count > 0 Then
                strJS.Append("Attenzione:\n")
            End If

            For i As Int16 = 0 To Me.Count - 1

                strJS.AppendFormat("{0} ", Me.Item(i).nome)
                If Me.Item(i).dose > 0 Then
                    strJS.AppendFormat("dose n. {0}", Me.Item(i).dose)
                End If
                strJS.Append(": ")

                Select Case Me.Item(i).type


                    Case ControlloEsecuzioneItemType.DatiIncompleti
                        strJS.Append("Non tutti i campi obbligatori sono impostati correttamente\n")

                    Case ControlloEsecuzioneItemType.DataFutura
                        strJS.Append("Data futura\n")

                    Case ControlloEsecuzioneItemType.DataPrimaNascita
                        strJS.Append("Data minore di quella di nascita\n")

                    Case ControlloEsecuzioneItemType.DataPrimaSedPrec
                        strJS.Append("Data inferiore a quella della dose precedente\n")

                    Case ControlloEsecuzioneItemType.DataDopoSedSuc
                        strJS.Append("Data inferiore di quella della dose successiva\n")

                    Case ControlloEsecuzioneItemType.StessaData
                        strJS.Append("Esiste una vaccinazione dello stesso tipo con la stessa data\n")

                    Case ControlloEsecuzioneItemType.StessaDoseVaccinazione
                        strJS.Append("Esiste una vaccinazione dello stesso tipo con la stessa dose tra le eseguite\n")

                    Case ControlloEsecuzioneItemType.StessaDoseAssociazione
                        strJS.Append("Esiste una associazione dello stesso tipo con la stessa dose tra le eseguite\n")

                    Case ControlloEsecuzioneItemType.ScadutaEsistente
                        strJS.Append("Esiste una vaccinazione scaduta con lo stesso codice, la stessa dose e la stessa data\n")

                    Case ControlloEsecuzioneItemType.VacconazioneDoseNonSuccessiva
                        strJS.Append("La dose della vaccinazione inserita non è quella successiva all\'ultima eseguita\n")

                    Case ControlloEsecuzioneItemType.AssociazioneDoseNonSuccessiva
                        strJS.Append("La dose della associazione inserita non è quella successiva all\'ultima eseguita\n")

                    Case ControlloEsecuzioneItemType.VaccinazioneProgrammataEsistente
                        strJS.Append("La vaccinazione è presente con stessa dose tra quelle programmate\n")

                    Case ControlloEsecuzioneItemType.SitoInoculoAssente
                        strJS.Append("Il sito di inoculo è obbligatorio\n")

                    Case ControlloEsecuzioneItemType.ViaSomministrazioneAssente
                        strJS.Append("La via di somministrazione è obbligatoria\n")

                    Case ControlloEsecuzioneItemType.DatiAggiuntiviIncompleti
                        strJS.Append("I dati di vaccinazione sono incompleti\n")

                    Case ControlloEsecuzioneItemType.DatiPagamentoIncompleti
                        strJS.Append("I dati di pagamento sono incompleti\n")

                    Case Else
                        Throw New NotSupportedException(String.Format("ControlloEsecuzioneItemType non supportato: {0}", Me.Item(i).type))
                End Select

            Next

            Return String.Format("alert('{0}');", strJS.ToString())

        End Function

    End Class

    Public Enum ControlloEsecuzioneItemType
        DatiIncompleti
        DataFutura
        DataPrimaNascita
        DataPrimaSedPrec
        DataDopoSedSuc
        StessaData
        StessaDoseVaccinazione
        StessaDoseAssociazione
        ScadutaEsistente
        VacconazioneDoseNonSuccessiva
        AssociazioneDoseNonSuccessiva
        VaccinazioneProgrammataEsistente
        SitoInoculoAssente
        ViaSomministrazioneAssente
        DatiAggiuntiviIncompleti
        LuogoComuneStatoIncongruenti
        DatiPagamentoIncompleti
    End Enum

    Public Class ControlloEsecuzioneItem

        Public hasError As Boolean
        Public type As ControlloEsecuzioneItemType
        Public nome As String
        Public dose As Int16

        Public Sub New(nome As String, dose As Int16, hasError As Boolean, type As ControlloEsecuzioneItemType)
            Me.hasError = hasError
            Me.type = type
            Me.nome = nome
            Me.dose = dose
        End Sub

    End Class

    Public Class CanSaveVaccinazioneEseguitaResult
        Inherits BizResult

        Public ReadOnly IdVaccinazioneEseguitaEsistente As Int64?
        Public ReadOnly IsVaccinazioneEseguitaEsistenteScaduta As Boolean
        Public ReadOnly VaccinazioneEseguitaEsistente As VaccinazioneEseguita

        Public Sub New()
        End Sub

        Public Sub New(success As Boolean, idVaccinazioneEseguitaEsistente As Int64?, isVaccinazioneEseguitaEsistenteScaduta As Boolean,
                       resultMessageEnumerable As IEnumerable(Of ResultMessage))

            MyBase.New(success, resultMessageEnumerable)

            Me.IdVaccinazioneEseguitaEsistente = idVaccinazioneEseguitaEsistente
            Me.IsVaccinazioneEseguitaEsistenteScaduta = isVaccinazioneEseguitaEsistenteScaduta

        End Sub

        Public Sub New(success As Boolean, idVaccinazioneEseguitaEsistente As Int64?, isVaccinazioneEseguitaEsistenteScaduta As Boolean,
                       resultMessageEnumerable As IEnumerable(Of ResultMessage), vaccinazioneEseguitaEsistente As VaccinazioneEseguita)

            MyBase.New(success, resultMessageEnumerable)

            Me.IdVaccinazioneEseguitaEsistente = idVaccinazioneEseguitaEsistente
            Me.IsVaccinazioneEseguitaEsistenteScaduta = isVaccinazioneEseguitaEsistenteScaduta
            Me.VaccinazioneEseguitaEsistente = vaccinazioneEseguitaEsistente

        End Sub

    End Class

    Public Class SalvaVaccinazioneEseguitaResult
        Inherits BizResult

        Public ReadOnly ConflittoRisolto As Boolean

        Public Sub New()
        End Sub

        Public Sub New(success As Boolean, conflittoRisolto As Boolean,
                       resultMessageEnumerable As IEnumerable(Of ResultMessage))

            MyBase.New(success, resultMessageEnumerable)

            Me.ConflittoRisolto = conflittoRisolto

        End Sub

    End Class

    Public Class SalvaVaccinazioneEseguitaCommand
        Inherits SalvaCommandBase

#Region " Public "

        Public Property VaccinazioneEseguita As Entities.VaccinazioneEseguita

        Public Property VaccinazioneEseguitaOriginale As Entities.VaccinazioneEseguita

        Public Property IsVaccinazioneEseguitaScaduta As Boolean

#End Region

#Region " Friend "

        'Friend Property CheckConflittoVisibilitaCentraleIfNeeded As Boolean

        Friend Property UpdateVaccinazioneEseguitaCentraleInConflittoIfNeeded As Boolean

        Friend Property VaccinazioneEseguitaCentrale As Entities.VaccinazioneEseguitaCentrale

#End Region

    End Class

    Private Class CanSaveVaccinazioneEseguitaCommand
        Inherits SalvaCommandBase

        Public Property VaccinazioneEseguita As VaccinazioneEseguita

        Public Property VaccinazioneEseguitaOriginale As VaccinazioneEseguita

        Public Property IsVaccinazioneEseguitaScaduta As Boolean

        ' Public Property CheckConflittoVisibilitaCentraleIfNeeded As Boolean

    End Class

#Region " Centrale "

    Public Class AcquisisciVaccinazioneEseguitaCentraleResult

        Public Property VaccinazioniEseguiteInserite As VaccinazioneEseguita()
        Public Property VaccinazioniEseguiteScaduteInserite As VaccinazioneEseguita()

        Public Property VaccinazioniEseguiteNonInserite As VaccinazioneEseguita()
        Public Property VaccinazioniEseguiteScaduteNonInserite As VaccinazioneEseguita()

        Public ReadOnly Property VaccinazioniEseguiteAcquisite As Boolean
            Get
                Return (Me.VaccinazioniEseguiteNonInserite.Count = 0)
            End Get
        End Property

        Public ReadOnly Property VaccinazioniEseguiteScaduteAcquisite As Boolean
            Get
                Return (Me.VaccinazioniEseguiteScaduteNonInserite.Count = 0)
            End Get
        End Property

    End Class

    Friend Class VaccinazioneEseguitaCentraleInfo

        Public Property VaccinazioneEseguita As VaccinazioneEseguita
        Public Property ReazioneAvversa As ReazioneAvversa
        Public Property VaccinazioneEseguitaCentrale As VaccinazioneEseguitaCentrale

    End Class

    Friend Class AggiornaVisibilitaVaccinazioniEseguiteResult

        Public Property VaccinazioniEseguiteAggiornate As VaccinazioneEseguita()

    End Class

#End Region

#End Region

#Region " Ricalcolo dosi "

    Public Class VaccinazioniCalcoloDosiResult

        Public Property Success As Boolean
        Public Property Vaccinazioni As List(Of VaccinazioniCalcoloDosi)
        Public Property VaccinazioniScartate As List(Of VaccinazioniCalcoloDosi)
        Public Property ControlList As ControlloEsecuzione

        Public Sub New(success As Boolean, vaccinazioniRicalcolate As List(Of VaccinazioniCalcoloDosi), vaccinazioniScartate As List(Of VaccinazioniCalcoloDosi), controlList As ControlloEsecuzione)
            Me.Success = success
            Me.Vaccinazioni = vaccinazioniRicalcolate
            Me.VaccinazioniScartate = vaccinazioniScartate
            Me.ControlList = controlList
        End Sub

    End Class

    <Serializable()>
    Public Class VaccinazioniCalcoloDosi
        Public Property Id As Long?
        Public Property CodicePaziente As Long
        Public Property CodiceVaccinazione As String
        Public Property DescrizioneVaccinazione As String
        Public Property DoseVaccinazione As Integer
        Public Property CodiceAssociazione As String
        Public Property DoseAssociazione As Integer
        Public Property DataEsecuzione As DateTime
        Public Property DataRegistrazione As DateTime
        Public Property DoseVaccinazioneOld As Integer
        Public Property DoseAssociazioneOld As Integer
        Public Property IsScaduta As Boolean
    End Class

    Private Class ElementoCalcoloDoseVaccinazione

        Public CodiceVaccinazione As String
        Public DoseVaccinazione As Integer

        Public Sub New(codice As String)
            Me.CodiceVaccinazione = codice
            Me.DoseVaccinazione = 0
        End Sub

    End Class

    Private Class ElementoCalcoloDoseAssociazione

        Public CodiceAssociazione As String
        Public CodiceVaccinazione As String
        Public DoseAssociazione As Integer

        Public Sub New(codiceAssociazione As String, codiceVaccinazione As String)
            Me.CodiceAssociazione = codiceAssociazione
            Me.CodiceVaccinazione = codiceVaccinazione
            Me.DoseAssociazione = 0
        End Sub

    End Class

    ''' <summary>
    ''' Calcolo delle dosi di vaccinazione e di associazione per le vaccinazioni registrate, e ricalcolo delle stesse anche per le eseguite 
    ''' in base alla data di effettuazione e alla dose. Scarta dal ricalcolo eventuali vaccinazioni già presenti in stessa data. 
    ''' </summary>
    ''' <param name="dtEseguite">Contiene sia le eseguite che le registrate. Le registrate hanno il campo ves_id nullo.</param>
    ''' <param name="controlList"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CalcoloDosi(dtEseguite As DataTable, controlList As ControlloEsecuzione, codicePaziente As Long) As VaccinazioniCalcoloDosiResult

        If dtEseguite Is Nothing OrElse dtEseguite.Rows.Count = 0 Then Return New VaccinazioniCalcoloDosiResult(True, Nothing, Nothing, controlList)

        ' Dataview contenente solo le vaccinazioni registrate non ancora su db
        Dim dvDaRegistrare As New DataView(dtEseguite)
        dvDaRegistrare.RowFilter = "ves_id is null"

        If dvDaRegistrare.Count = 0 Then Return New VaccinazioniCalcoloDosiResult(True, Nothing, Nothing, controlList)

        ' Controllo data effettuazione non futura e non precedente rispetto alla data di nascita
        Dim controlliDateEsecuzione As New ControlloEsecuzione()

        Dim dataNascitaPaziente As DateTime = Me.GenericProvider.Paziente.GetDataNascita(codicePaziente)

        For i As Integer = 0 To dvDaRegistrare.Count - 1

            Dim dose As Integer = Convert.ToInt32(dvDaRegistrare(i)("ves_n_richiamo"))
            Dim descrizione As String = dvDaRegistrare(i)("vac_descrizione").ToString()
            Dim dataEsecuzione As DateTime = Convert.ToDateTime(dvDaRegistrare(i)("ves_data_effettuazione"))

            Me.CheckDataEffettuazione(dose, descrizione, dataEsecuzione, controlliDateEsecuzione, dataNascitaPaziente)

        Next

        If Not controlliDateEsecuzione Is Nothing AndAlso controlliDateEsecuzione.Count > 0 Then

            controlList.AddRange(controlliDateEsecuzione)

            If controlliDateEsecuzione.HasError Then
                Return New VaccinazioniCalcoloDosiResult(False, Nothing, Nothing, controlList)
            End If

        End If

        ' Estrazione vaccinazioni e associazioni (distinte) che devono essere registrate
        ' Se una vaccinazione da registrare è in stessa data rispetto a una già registrata, non la considero
        Dim vaccinazioniDaRegistrare As New List(Of ElementoCalcoloDoseVaccinazione)()
        Dim codiciAssociazioni As New List(Of String)()

        ' Dataview contenente solo le eseguite già salvate
        Dim dvEseguite As New DataView(dtEseguite)
        dvEseguite.RowFilter = "ves_id is not null"

        Dim listVaccinazioniScartate As New List(Of VaccinazioniCalcoloDosi)()

        For i As Integer = 0 To dvDaRegistrare.Count - 1

            Dim codiceVaccinazione As String = dvDaRegistrare(i)("ves_vac_codice").ToString()
            Dim dataEsecuzione As DateTime = Convert.ToDateTime(dvDaRegistrare(i)("ves_data_effettuazione"))

            ' Ricerca vaccinazione e data tra le vaccinazioni eseguite già su database
            If ExistsVaccinazioneStessaData(codiceVaccinazione, dataEsecuzione, dvEseguite) Then

                listVaccinazioniScartate.Add(CreateVaccinazioneScartata(dvDaRegistrare(i), codicePaziente))

            Else

                If Not vaccinazioniDaRegistrare.Any(Function(p) p.CodiceVaccinazione = codiceVaccinazione) Then
                    vaccinazioniDaRegistrare.Add(New ElementoCalcoloDoseVaccinazione(codiceVaccinazione))
                End If

                Dim codiceAssociazione As String = dvDaRegistrare(i)("ves_ass_codice").ToString()

                If Not String.IsNullOrEmpty(codiceAssociazione) AndAlso Not codiciAssociazioni.Contains(codiceAssociazione) Then
                    codiciAssociazioni.Add(codiceAssociazione)
                End If

            End If

        Next

        ' Considero solo le righe del dtEseguite che hanno vaccinazione o associazione uguale a una di quelle elencate nelle liste di cui sopra
        Dim listVaccinazioniCalcoloDosi As New List(Of VaccinazioniCalcoloDosi)()

        Dim now As DateTime = DateTime.Now

        For Each row As DataRow In dtEseguite.Rows

            Dim codiceVaccinazioneCorrente As String = row("ves_vac_codice").ToString()

            Dim dataEsecuzioneCorrente As DateTime? = Nothing
            If Not row.IsNull("ves_data_effettuazione") Then
                dataEsecuzioneCorrente = Convert.ToDateTime(row("ves_data_effettuazione"))
            End If

            Dim scartata As Boolean = (
                dataEsecuzioneCorrente.HasValue AndAlso
                listVaccinazioniScartate.Any(Function(p) p.CodiceVaccinazione = codiceVaccinazioneCorrente And p.DataEsecuzione = dataEsecuzioneCorrente.Value)
            )

            ' Perchè la vaccinazione venga inclusa tra quelle per cui ricalcolare le dosi, devono essere verificate le condizioni:
            '   - non ha data esecuzione, cioè deve ancora essere eseguita.
            ' Oppure:
            '   - non deve essere stata scartata dai controlli precedenti (relativamente a vaccinazione e data di esecuzione) E
            '   - è inclusa tra quelle da registrare (come vaccinazione o perchè fa parte di un'associazione da registrare)

            If (Not dataEsecuzioneCorrente.HasValue) OrElse
               (Not scartata AndAlso
                    (vaccinazioniDaRegistrare.Any(Function(p) p.CodiceVaccinazione = codiceVaccinazioneCorrente) OrElse
                    (Not row.IsNull("ves_ass_codice") AndAlso codiciAssociazioni.Contains(row("ves_ass_codice"))))) Then

                listVaccinazioniCalcoloDosi.Add(CreateVaccinazioneCalcoloDosi(row, codicePaziente))

            End If

        Next

        ' Recupero vaccinazioni che compongono le associazioni
        Dim associazioniDaRegistrare As New List(Of ElementoCalcoloDoseAssociazione)()

        If listVaccinazioniCalcoloDosi.Any(Function(p) Not String.IsNullOrEmpty(p.CodiceAssociazione)) Then

            Dim listCodiciAssociazioniDaCercare As List(Of String) =
                listVaccinazioniCalcoloDosi.Select(Function(p) p.CodiceAssociazione).Distinct().ToList()

            Dim vaccAss As List(Of Entities.VaccinazioneAssociazione) =
                Me.GenericProvider.Associazioni.GetVaccinazioniAssociazioni(listCodiciAssociazioniDaCercare)

            If Not vaccAss.IsNullOrEmpty() Then

                For Each v As Entities.VaccinazioneAssociazione In vaccAss
                    associazioniDaRegistrare.Add(New ElementoCalcoloDoseAssociazione(v.CodiceAssociazione, v.CodiceVaccinazione))
                Next

            End If

        End If

        ' Ordinamento e ricalcolo dosi
        ' N.B. : l'ordinamento per il valore della dose (pre-calcolo) serve per non modificare le dosi a vaccinazioni già eseguite. 
        ' Se sono in stessa data, le dosi potrebbero venire scambiate e generare, al momento dell'update, un errore oracle di indice univoco violato.
        ' Questo è solo un caso (forse il più probabile) tra quelli possibili per cui il ricalcolo può generare errori oracle di quel tipo.
        listVaccinazioniCalcoloDosi = listVaccinazioniCalcoloDosi.
            OrderBy(Function(p) p.DataEsecuzione).
            ThenBy(Function(p) p.CodiceVaccinazione).
            ThenBy(Function(p) p.DoseVaccinazioneOld).ToList()

        ' Vaccinazioni NON SCADUTE: ricalcolo le dosi
        For Each vaccinazione As VaccinazioniCalcoloDosi In listVaccinazioniCalcoloDosi.Where(Function(vac) Not vac.IsScaduta)

            ' Dose di vaccinazione
            Dim itemVacc As ElementoCalcoloDoseVaccinazione = vaccinazioniDaRegistrare.First(Function(p) p.CodiceVaccinazione = vaccinazione.CodiceVaccinazione)
            itemVacc.DoseVaccinazione += 1

            vaccinazione.DoseVaccinazione = itemVacc.DoseVaccinazione

            ' Dose di associazione
            If Not String.IsNullOrEmpty(vaccinazione.CodiceAssociazione) Then

                Dim itemAss As ElementoCalcoloDoseAssociazione = associazioniDaRegistrare.First(Function(p) p.CodiceAssociazione = vaccinazione.CodiceAssociazione And p.CodiceVaccinazione = vaccinazione.CodiceVaccinazione)
                itemAss.DoseAssociazione += 1

                vaccinazione.DoseAssociazione = itemAss.DoseAssociazione

            End If

        Next

        ' Vaccinazioni SCADUTE: mantengo le stesse dosi
        For Each vaccinazione As VaccinazioniCalcoloDosi In listVaccinazioniCalcoloDosi.Where(Function(vac) vac.IsScaduta)

            vaccinazione.DoseVaccinazione = vaccinazione.DoseVaccinazioneOld
            vaccinazione.DoseAssociazione = vaccinazione.DoseAssociazioneOld

        Next

        ' Controlli post-calcolo: esistenza vaccinazioni scadute con stessa dose in stessa data o esistenza vaccinazioni programmate con stessa dose
        Dim controlliScaduteEProgrammate As New ControlloEsecuzione()

        For Each vaccinazioneRicalcolata As VaccinazioniCalcoloDosi In listVaccinazioniCalcoloDosi

            ' Per ogni vaccinazione non scaduta, controllo se esiste la stessa vaccinazione in stato "scaduta" (avente stessa dose e stessa data di effettuazione)
            If Not vaccinazioneRicalcolata.IsScaduta Then

                If ExistsStessaScaduta(vaccinazioneRicalcolata.CodiceVaccinazione, vaccinazioneRicalcolata.DoseVaccinazione, vaccinazioneRicalcolata.DataEsecuzione, dtEseguite) Then
                    controlliScaduteEProgrammate.Add(New ControlloEsecuzioneItem(vaccinazioneRicalcolata.CodiceVaccinazione, vaccinazioneRicalcolata.DoseVaccinazione, True, ControlloEsecuzioneItemType.ScadutaEsistente))
                End If

            End If

            ' Controllo l'esistenza, tra le programmate, di una stessa vaccinazione con stessa dose
            If Me.GenericProvider.VaccinazioneProg.ExistsVaccinazioneProgrammataByRichiamo(codicePaziente, vaccinazioneRicalcolata.CodiceVaccinazione, vaccinazioneRicalcolata.DoseVaccinazione) Then
                controlliScaduteEProgrammate.Add(New ControlloEsecuzioneItem(vaccinazioneRicalcolata.CodiceVaccinazione, vaccinazioneRicalcolata.DoseVaccinazione, True, ControlloEsecuzioneItemType.VaccinazioneProgrammataEsistente))
            End If

        Next

        If Not controlliScaduteEProgrammate.IsNullOrEmpty() Then

            controlList.AddRange(controlliScaduteEProgrammate)

            If controlliScaduteEProgrammate.HasError Then
                Return New VaccinazioniCalcoloDosiResult(False, Nothing, Nothing, controlList)
            End If

        End If

        Return New VaccinazioniCalcoloDosiResult(True, listVaccinazioniCalcoloDosi, listVaccinazioniScartate, controlList)

    End Function

    Private Function CreateVaccinazioneCalcoloDosi(row As DataRow, codicePaziente As Long) As VaccinazioniCalcoloDosi

        Dim vaccinazione As New VaccinazioniCalcoloDosi()

        vaccinazione.DoseVaccinazione = 0
        vaccinazione.DoseAssociazione = 0

        vaccinazione.DoseVaccinazioneOld = Convert.ToInt32(row("ves_n_richiamo"))

        If Not row.IsNull("ves_ass_n_dose") Then
            vaccinazione.DoseAssociazioneOld = Convert.ToInt32(row("ves_ass_n_dose"))
        End If

        vaccinazione.CodiceVaccinazione = row("ves_vac_codice").ToString()
        vaccinazione.DescrizioneVaccinazione = row("vac_descrizione").ToString()

        If row.IsNull("ves_ass_codice") Then
            vaccinazione.CodiceAssociazione = String.Empty
        Else
            vaccinazione.CodiceAssociazione = row("ves_ass_codice").ToString()
        End If

        If row.IsNull("ves_id") Then
            vaccinazione.Id = Nothing
        Else
            vaccinazione.Id = Convert.ToInt64(row("ves_id"))
        End If

        If row.IsNull("paz_codice") Then
            vaccinazione.CodicePaziente = codicePaziente
        Else
            vaccinazione.CodicePaziente = Convert.ToInt64(row("paz_codice"))
        End If

        If row.IsNull("ves_data_effettuazione") Then
            vaccinazione.DataEsecuzione = Now
        Else
            vaccinazione.DataEsecuzione = Convert.ToDateTime(row("ves_data_effettuazione"))
        End If

        If row.IsNull("ves_data_registrazione") Then
            vaccinazione.DataRegistrazione = Now
        Else
            vaccinazione.DataRegistrazione = Convert.ToDateTime(row("ves_data_registrazione"))
        End If

        If row.IsNull("scaduta") OrElse row("scaduta").ToString() = "N" Then
            vaccinazione.IsScaduta = False
        Else
            vaccinazione.IsScaduta = True
        End If

        Return vaccinazione

    End Function

    Private Function CreateVaccinazioneScartata(dvRow As DataRowView, codicePaziente As Long) As VaccinazioniCalcoloDosi

        Dim vaccinazioneScartata As New VaccinazioniCalcoloDosi()

        Dim doseVaccinazione As Integer = Convert.ToInt32(dvRow("ves_n_richiamo"))

        vaccinazioneScartata.CodicePaziente = codicePaziente
        vaccinazioneScartata.CodiceVaccinazione = dvRow("ves_vac_codice").ToString()
        vaccinazioneScartata.DescrizioneVaccinazione = dvRow("vac_descrizione").ToString()
        vaccinazioneScartata.DataEsecuzione = Convert.ToDateTime(dvRow("ves_data_effettuazione"))
        vaccinazioneScartata.DoseVaccinazione = doseVaccinazione
        vaccinazioneScartata.DoseVaccinazioneOld = doseVaccinazione

        If dvRow("ves_ass_codice") Is DBNull.Value Then
            vaccinazioneScartata.CodiceAssociazione = String.Empty
        Else
            vaccinazioneScartata.CodiceAssociazione = dvRow("ves_ass_codice").ToString()
        End If

        If dvRow("ves_id") Is DBNull.Value Then
            vaccinazioneScartata.Id = Nothing
        Else
            vaccinazioneScartata.Id = Convert.ToInt64(dvRow("ves_id"))
        End If

        If Not dvRow("ves_data_registrazione") Is DBNull.Value Then
            vaccinazioneScartata.DataRegistrazione = Convert.ToDateTime(dvRow("ves_data_registrazione"))
        End If

        If Not dvRow("ves_ass_n_dose") Is DBNull.Value Then
            Dim doseAssociazione As Integer = Convert.ToInt32(dvRow("ves_ass_n_dose"))
            vaccinazioneScartata.DoseAssociazione = doseAssociazione
            vaccinazioneScartata.DoseAssociazioneOld = doseAssociazione
        End If

        If dvRow("scaduta") Is Nothing OrElse dvRow("scaduta") Is DBNull.Value OrElse dvRow("scaduta").ToString() = "N" Then
            vaccinazioneScartata.IsScaduta = False
        Else
            vaccinazioneScartata.IsScaduta = True
        End If

        Return vaccinazioneScartata

    End Function





    ''' <summary>
    ''' Classe per memorizzare dosi e associazioni da modificare
    ''' </summary>
    Private Class VaccinazioneEseguitaModificaDosi

        Public Property Id As Long
        Public Property DoseVaccinazione As Integer
        Public Property DoseAssociazione As Integer

        Public Sub New(id As Long, doseVaccinazione As Integer, doseAssociazione As Integer)
            Me.Id = id
            Me.DoseVaccinazione = doseVaccinazione
            Me.DoseAssociazione = doseAssociazione
        End Sub

    End Class

    ''' <summary>
    ''' Allineamento numero dosi e associazioni delle vaccinazioni eseguite di un paziente
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    Public Function AllineamentoNumeroDosiEAssociazioni(codicePaziente As String) As BizGenericResult
        Dim result As New BizGenericResult
        Dim dt_vacEseguite As DataTable = Nothing
        Dim controlList As New ControlloEsecuzione()
        Dim listVaccinazioniScartate As New List(Of Biz.BizVaccinazioniEseguite.VaccinazioniCalcoloDosi)()

        Try
            dt_vacEseguite = GetVaccinazioniEseguite(codicePaziente, False)
            'Mi estraggo tutte le vacc e le associazioni del paziente
            Dim listaVaccini As New List(Of String)
            Dim listaAssoc As New List(Of String)
            For Each row As DataRow In dt_vacEseguite.Rows
                Dim codiceVaccinazione As String = row("ves_vac_codice").ToString()
                If Not listaVaccini.Contains(codiceVaccinazione) Then
                    listaVaccini.Add(codiceVaccinazione)
                End If
                Dim codiceAssociazione As String = String.Empty
                If Not row.IsNull("ves_ass_codice") Then
                    codiceAssociazione = row("ves_ass_codice").ToString()
                End If
                If Not listaAssoc.Contains(codiceAssociazione) Then
                    listaAssoc.Add(codiceAssociazione)
                End If
            Next
            ' Effettuo il ricalcolo dei progressivi delle vaccinazioni (n_richiamo)
            ' ordino le vaccinazioni per data di effettuazione e rifaccio il conteggio. 
            ' se il nuovo conteggio è diverso dal numero richiamo allora memorizzo ves_id e nuovo conteggio 
            Dim listaVacModifi As New List(Of VaccinazioneEseguitaModificaDosi)
            ' Per ogni vaccinazione estratta dalle eseguite
            For Each vacc As String In listaVaccini
                Dim expression As String = String.Format("ves_vac_codice = '{0}' ", vacc)
                Dim sort As String = "ves_data_effettuazione ASC"
                Dim countVac As Integer = 1
                ' filtro le vaccinazioni per la vaccinazione corrente e ordino per data di effettuazione
                For Each row As DataRow In dt_vacEseguite.Select(expression, sort)
                    ' se il conteggio è diverso dal richiamo allora memorizzo ves_id e nuovo valore
                    If countVac <> Convert.ToInt32(row("ves_n_richiamo")) Then
                        Dim lisVac As New VaccinazioneEseguitaModificaDosi(Convert.ToInt32(row("ves_id")), countVac, 0)
                        listaVacModifi.Add(lisVac)
                    End If
                    countVac = countVac + 1
                Next
            Next
            ' Effettuo il ricalcolo delle associazioni ordinando le vaccinazioni per data effettuazione della stessa associazione
            ' Il conteggio viene incrementato per ogni modifica di data effettuazione.
            ' Se il nuovo conteggio è diverso da ves_ass_n_dose, allora memorizzo ves_id e nuovo valore
            Dim listaAssModifi As New List(Of VaccinazioneEseguitaModificaDosi)
            ' Per ogni associazione estratta dalle associazioni
            For Each ass As String In listaAssoc
                Dim expression As String = String.Format("ves_ass_codice = '{0}' ", ass)
                Dim sort As String = "ves_data_effettuazione ASC"
                Dim count As Integer = 0
                Dim data As String = String.Empty
                ' Filtro le eseguite per per associazioni e ordino per data effettuazione
                For Each row As DataRow In dt_vacEseguite.Select(expression, sort)
                    ' se data cambia incremento conteggio
                    If String.IsNullOrWhiteSpace(data) OrElse row("ves_data_effettuazione") <> data Then
                        count = count + 1
                        data = row("ves_data_effettuazione")
                    End If
                    ' controllo il conteggio con dato memorizzatto se diverso memorizzo ves_id e nuovo conteggio
                    If count <> Convert.ToInt32(row("ves_ass_n_dose")) Then
                        Dim lisVac As New VaccinazioneEseguitaModificaDosi(Convert.ToInt32(row("ves_id")), 0, count)
                        listaAssModifi.Add(lisVac)
                    End If
                Next
            Next
            ' Aggiorno il valore delle dosi di ogni vaccinazione memorizzata
            For Each vaccDaModi As VaccinazioneEseguitaModificaDosi In listaVacModifi
                Dim upDate As Boolean
                upDate = UpdateNumeroDoseEseguita(vaccDaModi.Id, vaccDaModi.DoseVaccinazione, False)
                If Not upDate Then
                    upDate = UpdateNumeroDoseEseguita(vaccDaModi.Id, vaccDaModi.DoseVaccinazione, True)
                End If
            Next
            ' Aggiorno il valore delle associazioni di ogni vaccinazione memorizzata
            For Each associazioniDaModificare As VaccinazioneEseguitaModificaDosi In listaAssModifi
                Dim upDate As Boolean
                upDate = UpdateNumeroAssociazioneEseguita(associazioniDaModificare.Id, associazioniDaModificare.DoseAssociazione, False)
                If Not upDate Then
                    upDate = UpdateNumeroAssociazioneEseguita(associazioniDaModificare.Id, associazioniDaModificare.DoseAssociazione, True)
                End If
            Next
        Catch ex As Exception
            result.Success = False
            result.Message = ex.Message
        End Try

        Return result
    End Function

    Public Function UpdateNumeroDoseEseguita(vesId As Long, numeroDose As Integer, scaduta As Boolean) As Boolean
        Dim success As Boolean
        If scaduta Then
            success = GenericProvider.VaccinazioniEseguite.UpdateNumeroVacVacScadute(vesId, numeroDose)
        Else
            success = GenericProvider.VaccinazioniEseguite.UpdateNumeroVacVacEseguita(vesId, numeroDose)
        End If
        Return success
    End Function

    Public Function UpdateNumeroAssociazioneEseguita(vesId As Long, numeroDose As Integer, scaduta As Boolean) As Boolean
        Dim success As Boolean
        If scaduta Then
            success = GenericProvider.VaccinazioniEseguite.UpdateNumeroAssociazioneVacScadute(vesId, numeroDose)
        Else
            success = GenericProvider.VaccinazioniEseguite.UpdateNumeroAssociazioneVacEseguita(vesId, numeroDose)
        End If
        Return success
    End Function


#End Region

#Region " OnVac API "

    ''' <summary>
    ''' Restituisce le vaccinazioni eseguite dei pazienti specificati, comprese le mantoux.
    ''' </summary>
    ''' <param name="listCodiciPazienti">Elenco dei codici locali dei pazienti nella usl di lavoro</param>
    ''' <returns></returns>
    Public Function GetListVaccinazioniEseguitePazientiAPP(listCodiciPazienti As List(Of Long)) As List(Of Entities.VaccinazioneEseguitaAPP)

        Dim listEseguite As List(Of Entities.VaccinazioneEseguitaAPP) =
            Me.GenericProvider.VaccinazioniEseguite.GetListVaccinazioniEseguitePazientiAPP(listCodiciPazienti)

        Dim listMantouxEseguite As List(Of Entities.VaccinazioneEseguitaAPP) =
            GetMantouxEseguitePaziente(listCodiciPazienti)

        If Not listMantouxEseguite.IsNullOrEmpty() Then

            If listEseguite Is Nothing Then listEseguite = New List(Of VaccinazioneEseguitaAPP)()

            listEseguite.AddRange(listMantouxEseguite)

        End If

        If Not listEseguite.IsNullOrEmpty() Then

            For Each item As Entities.VaccinazioneEseguitaAPP In listEseguite
                item.AppIdAziendaLocale = Me.ContextInfos.IDApplicazione
            Next

            listEseguite = listEseguite.
                OrderBy(Function(p) p.CodicePaziente).
                ThenByDescending(Function(p) p.DataOraEffettuazione).
                ThenBy(Function(p) p.CodiceAssociazione).
                ThenByDescending(Function(p) p.DoseAssociazione).
                ThenBy(Function(p) p.Obbligatorieta).
                ThenBy(Function(p) p.OrdineVaccinazione).
                ToList()

        End If

        Return listEseguite

    End Function

    ''' <summary>
    ''' Restituisce le vaccinazioni eseguite del paziente nella data specificata
    ''' </summary>
    ''' <param name="codicePaziente">Codice locale del paziente nella usl di lavoro</param>
    ''' <param name="dataEffettuazione"></param>
    ''' <param name="codiceAssociazione"></param>
    ''' <returns></returns>
    Public Function GetListVaccinazioniEseguitePazienteDataAPP(codicePaziente As Long, dataEffettuazione As DateTime, codiceAssociazione As String) As List(Of Entities.VaccinazioneEseguitaAPP)

        Dim listEseguite As List(Of Entities.VaccinazioneEseguitaAPP) = Nothing

        If codiceAssociazione = Me.Settings.MANTOUX_CODICE_ASSOCIAZIONE Then

            Dim listMantouxEseguite As List(Of Entities.VaccinazioneEseguitaAPP) =
                GetMantouxEseguitePaziente(New List(Of Long) From {codicePaziente})

            If listMantouxEseguite.IsNullOrEmpty() Then
                listEseguite = New List(Of VaccinazioneEseguitaAPP)()
            Else
                listEseguite = listMantouxEseguite.Where(Function(p) p.DataEffettuazione.Date = dataEffettuazione.Date).ToList()
            End If

        Else

            listEseguite = Me.GenericProvider.VaccinazioniEseguite.GetListVaccinazioniEseguitePazienteDataAPP(codicePaziente, dataEffettuazione, codiceAssociazione)

        End If

        If Not listEseguite.IsNullOrEmpty() Then

            For Each item As Entities.VaccinazioneEseguitaAPP In listEseguite
                item.AppIdAziendaLocale = Me.ContextInfos.IDApplicazione
            Next

            listEseguite = listEseguite.
                OrderByDescending(Function(p) p.DoseAssociazione).
                ThenBy(Function(p) p.Obbligatorieta).
                ThenBy(Function(p) p.OrdineVaccinazione).
                ToList()

        End If

        Return listEseguite

    End Function

    Private Function GetMantouxEseguitePaziente(listCodiciPazienti As List(Of Long)) As List(Of Entities.VaccinazioneEseguitaAPP)

        Dim listMantouxEseguite As New List(Of Entities.VaccinazioneEseguitaAPP)()

        Dim mantoux As List(Of Entities.PazienteMantoux) =
            Me.GenericProvider.VaccinazioniEseguite.GetListMantoux(listCodiciPazienti)

        If Not mantoux.IsNullOrEmpty() Then

            Dim descrizioneVaccinazione As String =
                Me.GenericProvider.AnaVaccinazioni.GetCodiceDescrizioneVaccinazioni(New List(Of String) From {Me.Settings.MANTOUX_CODICE_VACCINAZIONE}).Single().Value

            Dim descrizioneAssociazione As String =
                Me.GenericProvider.Associazioni.GetDescrizioneAssociazione(Me.Settings.MANTOUX_CODICE_ASSOCIAZIONE)

            For Each item As Entities.PazienteMantoux In mantoux

                Dim mantouxEseguita As New Entities.VaccinazioneEseguitaAPP()
                mantouxEseguita.CodicePaziente = item.CodicePaziente
                mantouxEseguita.CodiceVaccinatore = item.CodiceVaccinatore
                mantouxEseguita.CognomePaziente = item.Cognome
                mantouxEseguita.NomePaziente = item.Nome
                mantouxEseguita.DataEffettuazione = item.DataEsecuzione
                mantouxEseguita.DataOraEffettuazione = item.DataEsecuzione
                mantouxEseguita.DataNascitaPaziente = item.DataNascita
                mantouxEseguita.DescrizioneVaccinatore = item.DescrizioneVaccinatore
                mantouxEseguita.CodiceVaccinazione = Me.Settings.MANTOUX_CODICE_VACCINAZIONE
                mantouxEseguita.DescrizioneVaccinazione = descrizioneVaccinazione
                mantouxEseguita.CodiceAssociazione = Me.Settings.MANTOUX_CODICE_ASSOCIAZIONE
                mantouxEseguita.DescrizioneAssociazione = descrizioneAssociazione

                listMantouxEseguite.Add(mantouxEseguita)

            Next

        End If

        Return listMantouxEseguite

    End Function

#Region " FSE "

    ''' <summary>
    ''' Restituisce le vaccinazioni eseguite di un dato paziente. Per l'FSE
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <returns></returns>
    Public Function GetListVaccinazioniEseguitePazienteFSE(codicePaziente As Long) As List(Of Entities.VaccinazioneFSE)

        Dim listEseguite As List(Of Entities.VaccinazioneFSE) = Me.GenericProvider.VaccinazioniEseguite.GetListVaccinazioniEseguitePazienteFSE(codicePaziente)

        If Not listEseguite.IsNullOrEmpty() Then

            listEseguite = listEseguite.
                OrderBy(Function(p) p.OrdineVaccinazione).
                ThenBy(Function(p) p.CodiceVaccinazione).
                ThenBy(Function(p) p.DataEffettuazione).
                ThenBy(Function(p) p.DoseVaccinazione).
                ToList()

        End If

        Return listEseguite

    End Function

#End Region

    ''' <summary>
    ''' Elimina eseguite e scadute in base agli id specificati
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="listaIdVaccinazione"></param>
    ''' <returns></returns>
    Public Function DeleteVaccinazioniById(codicePaziente As Long, listaIdVaccinazione As List(Of Long), ripristinoGiacenze As Boolean) As BizGenericResult

        ' Popolo un datatable con tutti i dati delle vaccinazioni specificate, metto le righe in stato Deleted e passo il dt alla funzione di salvataggio già esistente

        If codicePaziente <= 0 Then
            Return New BizGenericResult() With {.Success = False, .Message = "Eliminazione non effettuata: codice paziente non specificato"}
        End If

        If listaIdVaccinazione.IsNullOrEmpty() Then
            Return New BizGenericResult() With {.Success = False, .Message = "Eliminazione non effettuata: nessuna vaccinazione specificata"}
        End If

        Dim dt As DataTable = GetDtVaccinazioniEseguite(codicePaziente, listaIdVaccinazione)

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            Return New BizGenericResult() With {.Success = False, .Message = "Eliminazione non effettuata: nessun dato presente per le vaccinazioni specificate"}
        End If

        For Each row As DataRow In dt.Rows
            row.Delete()
        Next

        Dim listDatiLottiEliminati As New List(Of DatiLottoEliminato)()

        Try
            GenericProvider.BeginTransaction()

            ' SALVATAGGIO
            Dim command As New VaccinazioniEseguiteSalvaCommand() With {
                .CodicePaziente = codicePaziente,
                .DtEseguite = dt,
                .DatiVaccinazioniProgrammateDaEliminare = Nothing
            }

            Dim result As VaccinazioniEseguiteSalvaResult = SalvaNoTransactionScope(command, listDatiLottiEliminati, False, Nothing)

#Region " Ripristino Giacenze "

            If ripristinoGiacenze AndAlso Settings.GESMAG AndAlso result.Success AndAlso Not listDatiLottiEliminati.IsNullOrEmpty() Then

                ' Ripristino lotti in magazzino (se le vaccinazioni eliminate sono state eseguite da applicativo, non registrate nè provenienti da ACN)

                Dim listLotti As List(Of DatiLottoEliminato) = listDatiLottiEliminati.Where(Function(p) Not p.IsRegistrato).ToList()

                If Not listLotti.IsNullOrEmpty() Then
                    ' Caricamento lotti in magazzino, per ripristinare il numero di dosi in caso di eliminazione di una vaccinazione eseguita.
                    ' Viene caricata 1 dose per ogni terna lotto-data esecuzione-id associazione.

                    Dim dataRegistrazione As Date = Date.Now
                    Dim listTestateLog As New List(Of DataLogStructure.Testata)()

                    ' Inserimento di un movimento di carico per ripristinare ogni lotto + log
                    For Each datiLotto As DatiLottoEliminato In listLotti

                        ' Oggetto lock che verrà creato (uno per ogni lotto da ripristinare), e che verrà chiuso al termine di ogni salvataggio.
                        Dim lockLotto As Onit.Shared.Manager.Lock.Lock = Nothing

                        Try
                            ' Lock del lotto utilizzato
                            lockLotto = BizLotti.EnterLockLotto(datiLotto.CodiceLotto, ContextInfos.IDApplicazione, datiLotto.CnsCodiceMagazzino)

                            Using bizLotti As New BizLotti(GenericProvider, Settings, ContextInfos)

                                Dim movimentoLotto As New MovimentoLotto() With
                                {
                                    .CodiceLotto = datiLotto.CodiceLotto,
                                    .CodiceConsultorio = datiLotto.CnsCodiceMagazzino,
                                    .NumeroDosi = datiLotto.NumeroDosiDaRipristinare,
                                    .TipoMovimento = Constants.TipoMovimentoMagazzino.Carico,
                                    .IdUtente = ContextInfos.IDUtente,
                                    .DataRegistrazione = dataRegistrazione,
                                    .IdEsecuzioneAssociazione = datiLotto.IdAssociazione,
                                    .Note = BizLotti.ImpostaNoteMovimentoVaccinazione(
                                        codicePaziente.ToString(), datiLotto.DataEsecuzione, datiLotto.Vaccinazioni, Constants.TipoMovimentoMagazzino.Carico)
                                }

                                bizLotti.CaricaLottoVaccinazione(movimentoLotto, datiLotto.CnsCodiceEsecuzione, datiLotto.CnsCodiceMagazzino, False, listTestateLog)

                            End Using

                        Finally
                            BizLotti.ExitLockLotto(lockLotto)
                        End Try

                    Next

                    ' Scrittura log
                    For Each testata As DataLogStructure.Testata In listTestateLog
                        LogBox.WriteData(testata)
                    Next

                End If

            End If

#End Region

            GenericProvider.Commit()

        Catch ex As Exception

            GenericProvider.Rollback()

            Dim logMessage As New Text.StringBuilder()
            logMessage.AppendLine("Errore eliminazione vaccinazioni (DeleteVaccinazioniById)")
            logMessage.AppendFormat("Utente: {0}", ContextInfos.IDUtente.ToString()).AppendLine()
            logMessage.AppendFormat("VES_ID: {0}", String.Join(",", listaIdVaccinazione.ToArray()))

            Common.Utility.EventLogHelper.EventLogWrite(ex, logMessage.ToString(), EventLogEntryType.Information, ContextInfos.IDApplicazione)

            Dim msg As New Text.StringBuilder()
            msg.AppendFormat("Eliminazione non effettuata.").AppendLine()
            msg.AppendFormat("Errore: {0}", ex.Message).AppendLine()

            Return New BizGenericResult() With {.Success = False, .Message = msg.ToString()}

        End Try

        Return New BizGenericResult() With {.Success = True}

    End Function

    Public Class SalvaVaccinazioniEseguiteAPPCommand
        Public Property CodicePaziente As Long
        Public Property DataEffettuazione As Date
        Public Property CodiceAssociazione As String
        Public Property CodiceLotto As String
        Public Property CodiceSitoInoculazione As String
        Public Property CodiceViaSomministrazione As String
        Public Property CodiceCondizioneSanitaria As String
        Public Property CodiceCondizioneRischio As String
        Public Property IdRSA As String
        Public Property CodiceConsultorioMagazzino As String
        Public Property TipoPagamentoRSA As String
        Public Property IdUtente As Long
        Public Property CodiceUsl As String
        Public Property CodiceMedicoResponsabile As String
        Public Property CodiceVaccinatore As String
        Public Property GestioneGiacenze As Boolean
    End Class

    Public Function SalvaVaccinazioniEseguiteAPP(command As SalvaVaccinazioniEseguiteAPPCommand) As BizGenericResult

        If command.CodicePaziente <= 0 Then
            Return New BizGenericResult() With {.Success = False, .Message = "Salvataggio non effettuato: codice paziente mancante"}
        End If

        If String.IsNullOrWhiteSpace(command.CodiceAssociazione) Then
            Return New BizGenericResult() With {.Success = False, .Message = "Salvataggio non effettuato: codice associazione mancante"}
        End If

        ' Controllo sospensione paziente
        Using bizVisite As New BizVisite(GenericProvider, Settings, ContextInfos, Nothing)

            Dim sospensioneResult As BizGenericResult = bizVisite.CheckSospensioneVaccinazioniPaziente(command.CodicePaziente, command.DataEffettuazione)

            If Not sospensioneResult.Success Then
                Return New BizGenericResult() With {.Success = False, .Message = sospensioneResult.Message}
            End If

        End Using

        ' Calcolo dose associazione
        Dim doseAssociazione As Integer = GenericProvider.VaccinazioniEseguite.GetMaxDoseAssociazione(command.CodicePaziente, command.CodiceAssociazione) + 1

        ' Controllo campi obbligatori
        Dim msgCampiObbligatori As New Text.StringBuilder()

        If String.IsNullOrWhiteSpace(command.CodiceLotto) Then
            msgCampiObbligatori.Append("Codice Lotto, ")
        End If

        If Settings.CHECK_SITO_INOCULO AndAlso String.IsNullOrWhiteSpace(command.CodiceSitoInoculazione) Then
            msgCampiObbligatori.Append("Sito di Inoculazione, ")
        End If

        If Settings.CHECK_VIA_SOMMINISTRAZIONE AndAlso String.IsNullOrWhiteSpace(command.CodiceViaSomministrazione) Then
            msgCampiObbligatori.Append("Via di Somministrazione, ")
        End If

        If Settings.CONDIZIONE_SANITARIA_OBBLIGATORIA AndAlso String.IsNullOrWhiteSpace(command.CodiceCondizioneSanitaria) Then
            msgCampiObbligatori.Append("Condizione Sanitaria, ")
        End If

        If Settings.CONDIZIONE_RISCHIO_OBBLIGATORIA AndAlso String.IsNullOrWhiteSpace(command.CodiceCondizioneRischio) Then
            msgCampiObbligatori.Append("Condizione di Rischio, ")
        End If

        If msgCampiObbligatori.Length > 0 Then
            msgCampiObbligatori.RemoveLast(2)
            msgCampiObbligatori.Insert(0, "Salvataggio non effettuato. I seguenti campi obbligatori non sono stati valorizzati: ")
            Return New BizGenericResult() With {.Success = False, .Message = msgCampiObbligatori.ToString()}
        End If

        ' Controlli di coerenza dei dati
        ' Controllo data effettuazione > data di nascita
        Dim dataNascitaPaziente As Date = GenericProvider.Paziente.GetDataNascita(command.CodicePaziente)

        If dataNascitaPaziente > Date.MinValue AndAlso command.DataEffettuazione < dataNascitaPaziente Then
            Return New BizGenericResult() With {.Success = False, .Message = "Salvataggio non effettuato: la data di effettuazione è inferiore alla data di nascita del paziente"}
        End If

        ' Controllo giacenza lotto
        Dim msgGiacenzaLotto As String = String.Empty
        Dim msgMovimentoLotto As String = String.Empty

        If command.GestioneGiacenze AndAlso Settings.GESMAG Then

            Using bizLotti As New BizLotti(GenericProvider, Settings, ContextInfos, Nothing)

                Dim controlloLotti As BizLotti.BizLottiResult = bizLotti.ControlloDosiLottoDaEseguire(command.CodiceLotto, command.CodiceConsultorioMagazzino)

                If controlloLotti.Result = BizLotti.BizLottiResult.ResultType.GenericError Then
                    Return New BizGenericResult() With {.Success = False, .Message = controlloLotti.Message}
                ElseIf controlloLotti.Result <> BizLotti.BizLottiResult.ResultType.Success Then
                    msgGiacenzaLotto = controlloLotti.Message
                End If

            End Using

        End If

        ' Lista dei lock che vengono creati (uno per ogni lotto eseguito), che verranno chiusi al termine del salvataggio.
        Dim listLockLotti As New List(Of Onit.Shared.Manager.Lock.Lock)()

        Dim eseguiVaccinazioni As Boolean = True
        Dim msgVacNonEseguite As String = String.Empty

        Try
            GenericProvider.BeginTransaction()

            ' Mi faccio restituire un datatable vuoto ma con la struttura corretta
            Dim dtVaccinazioniEseguite As DataTable = GetDtVaccinazioniEseguite(command.CodicePaziente, New List(Of Long)() From {-1})

            ' Recupero i codici delle vaccinazioni relative all'associazione
            Dim listCodiciVaccinazioni As List(Of String) =
                GenericProvider.Associazioni.GetVaccinazioniAssociazioni(New List(Of String) From {command.CodiceAssociazione}).
                Select(Function(v) v.CodiceVaccinazione).
                ToList()

            ' Controllo data effettuazione minima, in caso di vaccinazione covid
            If listCodiciVaccinazioni.Any(Function(v) Settings.CODICI_VACCINAZIONI_COVID.Contains(v)) AndAlso
               Settings.DATA_INIZIO_SOMMINISTRAZIONE_COVID.HasValue AndAlso
               command.DataEffettuazione.Date < Settings.DATA_INIZIO_SOMMINISTRAZIONE_COVID.Value.Date Then

                eseguiVaccinazioni = False
                msgVacNonEseguite = String.Format("Non è possibile registrare vaccinazioni {0} con data di effettuazione inferiore al {1:dd/MM/yyyy} ",
                                                  String.Join(",", Settings.CODICI_VACCINAZIONI_COVID.ToArray()), command.DataEffettuazione.Date)

            End If

            If eseguiVaccinazioni Then

                Dim dataRegistrazione As Date = Date.Now

                For Each codiceVaccinazione As String In listCodiciVaccinazioni

                    InsertRowIntoDtEseguite(command, codiceVaccinazione, doseAssociazione, dataRegistrazione, dtVaccinazioniEseguite)

                    ' TODO [API - RSA]: DeleteInadempienza => FARE? (vedi VacProg r.4142)
                    GenericProvider.Paziente.DeleteInadempienza(command.CodicePaziente, codiceVaccinazione)

                Next

                ' Lock delle risorse per scarico da magazzino
                If command.GestioneGiacenze AndAlso Settings.GESMAG Then
                    listLockLotti.Add(BizLotti.EnterLockLotto(command.CodiceLotto, ContextInfos.IDApplicazione, command.CodiceConsultorioMagazzino))
                End If

                SalvaNoTransactionScope(command.CodicePaziente, dtVaccinazioniEseguite)

                Dim listTestateLog As New List(Of Testata)()

                If command.GestioneGiacenze AndAlso Settings.GESMAG Then

                    Dim _logOptions As BizLogOptions = LogOptions

                    If _logOptions Is Nothing Then
                        _logOptions = New BizLogOptions(DataLogStructure.TipiArgomento.VAC_PROGRAMMATE, True)
                    End If

                    Using bizLotti As New BizLotti(GenericProvider, Settings, ContextInfos, _logOptions)

                        Dim dv As New DataView(dtVaccinazioniEseguite)
                        dv.Sort = "ves_ass_prog"

                        Dim idEsecuzioneAssociazione As String = dv(0)("ves_ass_prog")

                        Dim movimentoLotto As New MovimentoLotto() With
                        {
                            .CodiceLotto = command.CodiceLotto,
                            .CodiceConsultorio = command.CodiceConsultorioMagazzino,
                            .NumeroDosi = 1,
                            .TipoMovimento = Constants.TipoMovimentoMagazzino.Scarico,
                            .IdUtente = command.IdUtente,
                            .DataRegistrazione = dataRegistrazione,
                            .IdEsecuzioneAssociazione = idEsecuzioneAssociazione,
                            .Note = BizLotti.ImpostaNoteMovimentoVaccinazione(
                                command.CodicePaziente.ToString(), command.DataEffettuazione,
                                String.Join(",", listCodiciVaccinazioni.ToArray()), Constants.TipoMovimentoMagazzino.Scarico)
                        }

                        ' N.B. : bisognerebbe finire di sistemare e utilizzare bizLotti.ScaricaLottoVaccinazioneNoTransactionScope ma hanno chiesto espressamente di non gestire le giacenze, per cui non faccio nulla

                        ' Inserimento del movimento su db + log
                        Dim insertMovimentoResult As BizLotti.BizLottiResult =
                        bizLotti.ScaricaLottoVaccinazione(movimentoLotto, command.CodiceConsultorioMagazzino, command.CodiceConsultorioMagazzino, False, listTestateLog)

                        If insertMovimentoResult.Result = BizLotti.BizLottiResult.ResultType.GenericError Then

                            msgMovimentoLotto = insertMovimentoResult.Message

                        ElseIf insertMovimentoResult.Result = BizLotti.BizLottiResult.ResultType.LottoDisattivatoScortaNullaWarning Then

                            msgMovimentoLotto = String.Format("Lotto {0} disattivato: numero dosi esaurito.", command.CodiceLotto)

                        End If

                    End Using

                End If

                ' TODO [API - RSA]: manca eliminazione programmazione
                ' bizVaccinazioneProg.EliminaProgrammazione (vedi VacProg r.4318)

                ' TODO [API - RSA]: manca integrazione FSE
                ' If Settings.FSE_GESTIONE (vedi VacProg r.4352)

                ' Scrittura Log
                If Not listTestateLog.IsNullOrEmpty() Then

                    For Each testata As Testata In listTestateLog
                        LogBox.WriteData(testata)
                    Next

                End If

            End If

            GenericProvider.Commit()

        Catch ex As Exception

            GenericProvider.Rollback()

            Dim logMessage As New Text.StringBuilder()
            logMessage.AppendLine("Errore inserimento vaccinazioni (SalvaVaccinazioniEseguiteAPP)")
            logMessage.AppendFormat("Utente: {0}", command.IdUtente.ToString()).AppendLine()
            logMessage.AppendFormat("Paziente: {0}", command.CodicePaziente.ToString()).AppendLine()
            logMessage.AppendFormat("Associazione: {0}", command.CodiceAssociazione.ToString()).AppendLine()

            Common.Utility.EventLogHelper.EventLogWrite(ex, logMessage.ToString(), EventLogEntryType.Information, ContextInfos.IDApplicazione)

            Dim msg As New Text.StringBuilder()
            msg.AppendFormat("Salvataggio non effettuato.").AppendLine()
            msg.AppendFormat("Errore: {0}", ex.Message).AppendLine()

            Return New BizGenericResult() With {.Success = False, .Message = msg.ToString()}

        Finally

            If command.GestioneGiacenze AndAlso Settings.GESMAG Then
                BizLotti.ExitLockLotti(listLockLotti)
            End If

        End Try

        If Not eseguiVaccinazioni Then

            Return New BizGenericResult() With {.Success = False, .Message = msgVacNonEseguite}

        End If

        Dim msgOk As New Text.StringBuilder("Salvataggio effettuato.")

        If Not String.IsNullOrWhiteSpace(msgGiacenzaLotto) Then
            msgOk.AppendLine(msgGiacenzaLotto).AppendLine()
        End If

        If Not String.IsNullOrWhiteSpace(msgMovimentoLotto) Then
            msgOk.AppendLine(msgMovimentoLotto).AppendLine()
        End If

        Return New BizGenericResult() With {.Success = True, .Message = msgOk.ToString()}

    End Function

    Private Sub InsertRowIntoDtEseguite(command As SalvaVaccinazioniEseguiteAPPCommand, codiceVaccinazione As String, doseAssociazione As Integer, dataRegistrazione As Date, ByRef dtVaccinazioniEseguite As DataTable)

        Dim newRow As DataRow = dtVaccinazioniEseguite.NewRow()

        newRow("scaduta") = "N"
        newRow("PAZ_CODICE") = command.CodicePaziente
        newRow("VES_VAC_CODICE") = codiceVaccinazione
        newRow("VES_N_RICHIAMO") = GenericProvider.VaccinazioniEseguite.GetMaxRichiamo(command.CodicePaziente, codiceVaccinazione) + 1
        newRow("VES_DATA_EFFETTUAZIONE") = command.DataEffettuazione.Date
        newRow("VES_DATAORA_EFFETTUAZIONE") = command.DataEffettuazione
        newRow("VES_ASS_CODICE") = command.CodiceAssociazione
        newRow("VES_ASS_N_DOSE") = doseAssociazione
        newRow("VES_DATA_REGISTRAZIONE") = dataRegistrazione
        newRow("VES_USL_INSERIMENTO") = command.CodiceUsl
        newRow("VES_UTE_ID") = command.IdUtente
        newRow("VES_LOT_CODICE") = command.CodiceLotto
        newRow("VES_SII_CODICE") = command.CodiceSitoInoculazione
        newRow("VES_VII_CODICE") = command.CodiceViaSomministrazione
        newRow("VES_OPE_IN_AMBULATORIO") = "N"
        newRow("VES_FLAG_FITTIZIA") = "N"
        newRow("VES_FLAG_VISIBILITA") = Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente

        Dim lottoAnagrafe As LottoAnagrafe = Nothing

        Using bizLotti As New BizLotti(GenericProvider, Settings, ContextInfos)
            lottoAnagrafe = bizLotti.GetLottoAnagrafe(command.CodiceLotto)
        End Using

        newRow("VES_NOC_CODICE") = lottoAnagrafe.CodiceNomeCommerciale
        newRow("VES_LOT_DATA_SCADENZA") = lottoAnagrafe.DataScadenza

        Using bizVac As New BizVaccinazioniAnagrafica(GenericProvider, Settings, ContextInfos)
            newRow("VES_ANTIGENE") = bizVac.GetAntigene(command.CodiceAssociazione, codiceVaccinazione)
        End Using

        newRow("VES_STATO") = Constants.StatoVaccinazioneEseguita.Registrata
        newRow("VES_LUOGO") = Constants.CodiceLuogoVaccinazione.InRegione
        newRow("VES_PROVENIENZA") = [Enum].GetName(GetType(Enumerators.ProvenienzaVaccinazioni), Enumerators.ProvenienzaVaccinazioni.RSA)

        newRow("VES_MAL_CODICE_COND_SANITARIA") = command.CodiceCondizioneSanitaria
        newRow("VES_RSC_CODICE") = command.CodiceCondizioneRischio

        newRow("VES_TPA_GUID_TIPI_PAGAMENTO") = New Guid(command.TipoPagamentoRSA).ToByteArray()

        Dim codiceConsultorioRSA As String = String.Empty
        Dim tipoErogatore As String = String.Empty
        Dim strutture As List(Of Struttura) = Nothing

        Using bizCns As New BizConsultori(GenericProvider, Settings, ContextInfos, LogOptions)

            codiceConsultorioRSA = bizCns.GetCodiceConsultorioRSA(command.IdRSA)

            If String.IsNullOrWhiteSpace(codiceConsultorioRSA) Then codiceConsultorioRSA = command.CodiceConsultorioMagazzino

            tipoErogatore = bizCns.GetTipoErogatoreConsultorio(codiceConsultorioRSA)

            strutture = bizCns.GetStrutture(command.IdRSA, tipoErogatore)

        End Using

        newRow("VES_CNS_CODICE") = codiceConsultorioRSA
        newRow("VES_CNS_REGISTRAZIONE") = codiceConsultorioRSA

        newRow("VES_TIPO_EROGATORE") = tipoErogatore

        If Not strutture.IsNullOrEmpty() Then
            newRow("VES_COMUNE_O_STATO") = strutture.First().CodiceComune
            newRow("VES_CODICE_STRUTTURA") = strutture.First().CodiceStruttura
            newRow("VES_USL_COD_SOMMINISTRAZIONE") = strutture.First().CodiceAsl
        End If

        newRow("VES_OPE_CODICE") = command.CodiceMedicoResponsabile
        newRow("VES_MED_VACCINANTE") = command.CodiceVaccinatore

        dtVaccinazioniEseguite.Rows.Add(newRow)

    End Sub

#End Region

End Class


