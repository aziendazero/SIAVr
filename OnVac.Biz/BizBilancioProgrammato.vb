Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.Collection
Imports Onit.OnAssistnet.OnVac.Common.Utility


Public Class BizBilancioProgrammato
    Inherits BizClass

#Region " Constants "

    Public Const LIMITE_MIN_PERCENTILE As Integer = 3
    Public Const LIMITE_MAX_PERCENTILE As Integer = 90

#End Region

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)

        MyBase.New(genericprovider, settings, contextInfos, logOptions)

    End Sub

#End Region

#Region " Class "

    Public Class DatiBilancioPazienteResult

        Public Sezioni As DataTable
        Public CondizioniBilancio As DataTable
        Public RispostePossibili As DataTable
        Public Domande As DataTable
        Public Risposte As DataTable

    End Class

#End Region

#Region " IEqualityComparers "

    Private Class UslInserimentoVisiteComparer
        Implements IEqualityComparer(Of Entities.VisitaCentrale)

        Public Function Equals1(x As Entities.VisitaCentrale, y As Entities.VisitaCentrale) As Boolean Implements IEqualityComparer(Of Entities.VisitaCentrale).Equals

            If String.IsNullOrEmpty(x.CodiceUslVisita) AndAlso String.IsNullOrEmpty(y.CodiceUslVisita) Then
                Return False
            End If

            Return (x.CodiceUslVisita = y.CodiceUslVisita)

        End Function

        Public Function GetHashCode1(obj As Entities.VisitaCentrale) As Integer Implements IEqualityComparer(Of Entities.VisitaCentrale).GetHashCode

            Return obj.CodiceUslVisita.GetHashCode()

        End Function

    End Class

#End Region

#Region " Public Methods "

    ''' <summary>
    ''' Se ci sono bilanci non eseguiti fuori età massima per il paziente, imposta lo stato a UNSOLVED.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    Public Sub ControlloBilanciNonEseguitiScaduti(codicePaziente As Integer)

        Dim listInfoBilanci As List(Of IBilancioProgrammatoProvider.InfoBilancio) =
            Me.GenericProvider.BilancioProgrammato.GetInfoBilanciNonEseguitiScaduti(codicePaziente)

        If Not listInfoBilanci Is Nothing AndAlso listInfoBilanci.Count > 0 Then

            Dim now As DateTime = DateTime.Now

            For Each infoBilancio As IBilancioProgrammatoProvider.InfoBilancio In listInfoBilanci

                ' Update del bilancio: lo stato passa da "non eseguito" a "scaduto" (UX => US)
                infoBilancio.Bilancio.Bil_stato = Constants.StatiBilancio.UNSOLVED
                infoBilancio.Bilancio.New_Cnv = False

                Me.GenericProvider.BilancioProgrammato.Update(infoBilancio.Bilancio)

                ' Eliminazione convocazione se solo bilancio
                If infoBilancio.IsConvocazioneSoloBilancio Then

                    Using bizConvocazione As New BizConvocazione(Me.GenericProvider, Me.Settings, Me.ContextInfos, Nothing)

                        Dim command As New BizConvocazione.EliminaConvocazioniSollecitiBilanciCommand()
                        command.CodicePaziente = infoBilancio.Bilancio.Paz_Codice
                        command.DataConvocazione = infoBilancio.Bilancio.Data_CNV
                        command.CancellaBilanciAssociati = False
                        command.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.EliminazioneConvocazione
                        command.DataEliminazione = now
                        command.NoteEliminazione = "Eliminata convocazione solo bilancio"
                        command.WriteLog = False

                        bizConvocazione.EliminaConvocazioniSollecitiBilanci(command)

                    End Using

                End If
            Next
        End If

    End Sub

    ''' <summary>
    ''' Restituisce il valore del percentile relativo al tipo specificato
    ''' </summary>
    ''' <param name="tipoPercentile"></param>
    ''' <param name="valorePercentile">formato Stringa</param>
    ''' <param name="dataNascitaPaziente"></param>
    ''' <param name="sessoPaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CalcolaPercentile(tipoPercentile As Enumerators.TipoPercentile, valorePercentile As String, dataNascitaPaziente As DateTime, sessoPaziente As String) As String

        valorePercentile = valorePercentile.Trim().Replace(".", ",")

        If String.IsNullOrEmpty(valorePercentile) Then Return String.Empty

        ' TODO: controllo valore non numerico?

        Return Me.CalcolaPercentile(tipoPercentile, Convert.ToDouble(valorePercentile), dataNascitaPaziente, sessoPaziente)

    End Function

    ''' <summary>
    ''' Restituisce il valore del percentile relativo al tipo specificato
    ''' </summary>
    ''' <param name="tipoPercentile"></param>
    ''' <param name="valorePercentile">formato Double</param>
    ''' <param name="dataNascitaPaziente"></param>
    ''' <param name="sessoPaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CalcolaPercentile(tipoPercentile As Enumerators.TipoPercentile, valorePercentile As Double?, dataNascitaPaziente As DateTime, sessoPaziente As String) As String

        If Not valorePercentile.HasValue OrElse valorePercentile.Value = 0 Then Return String.Empty

        Dim anniPaziente As Integer =
            DateTime.Now.Year - dataNascitaPaziente.Year + (1 / ((1 / (DateTime.Now.Month - dataNascitaPaziente.Month)) * 12))

        Dim listPercentili As List(Of Entities.Percentile) =
            Me.GenericProvider.BilancioProgrammato.GetListPercentili(anniPaziente, sessoPaziente, tipoPercentile, valorePercentile)

        If listPercentili Is Nothing OrElse listPercentili.Count = 0 Then
            Return ">" + LIMITE_MAX_PERCENTILE.ToString()
        End If

        If listPercentili(0).Percentile = LIMITE_MIN_PERCENTILE And valorePercentile < listPercentili(0).Percentile Then
            Return "<" + LIMITE_MIN_PERCENTILE.ToString()
        End If

        Return listPercentili(0).Percentile.ToString()

    End Function

#Region " Programmazione Bilanci "

    ''' <summary>
    ''' Metodo per la programmazione automatica dei Bilanci di Salute 
    ''' </summary>
    ''' <param name="dtaNewConv">DataTable contenente convocazioni per vaccinazioni generate 
    ''' dalla procedura chiamante (OnVacConvocazioni.vb), ma non ancora inserite nel DB</param>
    ''' <param name="codicePaziente">Codice del paziente che si sta processando</param>
    ''' <param name="etaPaziente">Età del paziente espressa in giorni</param>
    ''' <param name="dataNascita">Data di nascita del paziente</param>
    ''' <returns>Un oggetto di tipo BilancioCollection contenente tutti i bilanci (sia quelli scaduti che il prossimo da eseguire) 
    ''' da inserire nella tabella T_BIL_PROGRAMMATI</returns>
    ''' <remarks>
    ''' Il metodo implementa l'algoritmo per la programmazione automatica dei Bilanci di Salute. 
    ''' Ciascuna malattia avente dei bilanci viene considerata come un flusso di lavori (WORKFLOW). 
    ''' 
    ''' L'algoritmo effettua le seguenti operazioni per ciascun flusso (ciascuna malattia):
    ''' 
    ''' - raccoglie i lavori che non hanno più senso di essere eseguiti (perchè scaduti) e che non sono stati
    '''   ancora inseriti nella tabella T_BIL_PROGRAMMATI;
    ''' - cerca inoltre il primo lavoro che possa essere programmato; 
    ''' - recupera una data di convocazione utile per il lavoro da programmare;
    '''   * cerca in un primo momento tra le convocazioni presenti nel datatable dtaNewConv;
    '''   * se la ricerca precedente fallisce, effettua una query nella tabella T_CNV_CONVOCAZINI
    '''     per verificare l'esistenza di una convocazione per vaccinazione obbligatoria che soddisfi i requisiti;
    '''   * se la ricerca precedente fallisce, programma la creazione di una convocazione 
    '''     nella data estremo inferiore di validità del lavoro.
    ''' - restituisce una collezione di bilanci che contengono i dati necessari alla popolazione
    '''   della tabella T_BIL_PROGRAMMATI ed eventualmente alla generazione di Convocazioni per SOLO BILANCIO. 
    '''</remarks>
    ''' <history>
    ''' 	[pmontevecchi]	17/10/2007	Created
    ''' </history>
    Public Function BilanciDaProgrammare(dtaNewConv As DataTable, codicePaziente As Integer, etaPaziente As Integer, dataNascita As Date) As BilancioProgrammatoCollection
        '--
        Dim NewConvS As Integer = dtaNewConv.Rows.Count                 ' numero di convocazioni da programmare
        Dim DtsHelper As New Common.Utility.DataSetHelper()
        Dim ds As New System.Data.DataSet("DSDistinct")
        Dim dtmal As DataTable
        '--
        Dim BiltoPrg As DataTable
        Dim data_cnv As Date                                            ' data di convocazione calcolata per il bilancio da prog
        Dim i, j As Integer
        Dim bilancioCollection As New BilancioProgrammatoCollection()
        Dim convocazioneCollection As New ConvocazioneCollection()
        '--
        Dim datapartenza As Date                                        ' data di partenza per il calcolo dell'intervallo di cnv per il bil da prog
        Dim data_cnv_min, data_cnv_max As Date                          ' intervallo di date di cnv per il bil da prog
        Dim intervallo As Integer
        Dim new_cnv As Boolean                                          ' flag che indica se creare una cnv per il bil da prog, o se c'è già
        Dim data_cnv_vacc() As DataRow
        '--
        DtsHelper.setDataSet(ds)

        '  --- Calcolo i bilanci da programmare --- '
        BiltoPrg = Me.GenericProvider.BilancioProgrammato.GetBiltoProgr(codicePaziente)

        ' Se non ci sono bilanci da programmare, termino l'algoritmo
        If (BiltoPrg.Rows.Count <= 0) Then Return bilancioCollection

        ' --- Creo un dataset che contiene le diverse malattie associate ai bilanci da programmare --- '
        dtmal = DtsHelper.SelectDistinct("Malattie", BiltoPrg, "bil_mal_codice")
        If dtmal Is Nothing Then Return bilancioCollection

        Dim cod_malattia As String = String.Empty

        ' --- Ciclo sulle malattie (distinte) che appartengono ai bilanci del paziente --- '
        For i = 0 To dtmal.Rows.Count - 1
            '--
            data_cnv = Date.MinValue
            '--

            ' Codice della malattia corrente
            cod_malattia = dtmal.Rows(i)("bil_mal_codice").ToString()

            ' Filtro il datatable dei bilanci per avere solo i bilanci relativi alla malattia corrente
            Dim dvSelected_rows As New DataView(BiltoPrg)
            dvSelected_rows.Sort = "bil_eta_massima, bil_eta_minima, bil_numero ASC"
            dvSelected_rows.RowFilter = String.Format("bil_mal_codice = '{0}'", cod_malattia)

            ' --- Ciclo sui bilanci relativi alla malattia corrente --- '
            For j = 0 To dvSelected_rows.Count - 1
                '--
                datapartenza = dataNascita  ' Data di partenza per il calcolo delle date di cnv.
                data_cnv = Date.MinValue    ' Data di una cnv già esistente a cui associare il bilancio corrente.
                new_cnv = False             ' Flag di segnalazione se, alla programmazione del bilancio, deve essere creata anche la cnv
                '--

                ' Per continuare con la programmazione di questo bilancio, devono essere verificate due condizioni:
                '   1 - Il bilancio deve essere obbligatorio oppure ci devono essere delle convocazioni da programmare.
                '       Se il bilancio non è obbligatorio e non ci sono delle convocazioni da programmare, passa al bilancio successivo.
                '   2 - L'età del paziente non deve superare quella massima impostata per eseguire il bilancio.
                '       Se il paziente è fuori età massima, passa al bilancio successivo.
                If (Not (dvSelected_rows(j)("bil_obbligatorio") = "N" AndAlso NewConvS = 0)) _
                    AndAlso (dvSelected_rows(j)("bil_eta_massima") >= etaPaziente) Then

                    ' Il campo MAL_FLAG_VISITA è il discriminante tra bilanci di salute (il cui calcolo parte dalla data di nascita 
                    ' e dipende dall'età) e bilanci di malattia (che devono essere consegnati a scadenze predefinite, partendo dalla prima
                    ' diagnosi o dalla prima visita).
                    Dim flagVisita As Boolean = (dvSelected_rows(j)("mal_flag_visita").ToString() = "S")

                    '--
                    ' Determino l'intervallo di partenza per il calcolo della data di cnv del bilancio
                    '--
                    ' Controlla se il paziente ha già effettuato visite per la malattia corrente oppure no.
                    If (Not (Me.GenericProvider.BilancioProgrammato.VerifyVisits(cod_malattia, codicePaziente))) Then
                        '--
                        ' IL PAZIENTE NON HA MAI CONSEGNATO UN BILANCIO IN PRECEDENZA
                        '--
                        ' Determino la data di partenza per il calcolo cnv: se flag visita vale N si parte dalla data di nascita.
                        ' Altrimenti, se vale S, la data di partenza deve essere la data dell'ultima visita o dalla diagnosi, 
                        ' in base al flag nuova diagnosi.
                        ' Se ci sono anche bilanci in stato unsolved (per la stessa malattia), devo controllare la data del più recente.
                        ' Se è più recente anche rispetto alla data di partenza calcolata, la data dell'ultimo bilancio unsolved
                        ' diventa la data di partenza.
                        '--
                        Dim isLastDateUS As Boolean = False
                        If (flagVisita) Then
                            '--
                            If (Not dvSelected_rows(j)("pma_nuova_diagnosi") Is DBNull.Value) Then
                                '--
                                If (dvSelected_rows(j)("pma_nuova_diagnosi") = "S") Then
                                    '--
                                    ' PARAMETRO: PMA_NUOVA_DIAGNOSI
                                    datapartenza = dvSelected_rows(j)("pma_data_diagnosi")
                                Else
                                    '--
                                    ' PARAMETRO: PMA_DATA_ULTIMA_VISITA
                                    datapartenza = dvSelected_rows(j)("pma_data_ultima_visita")
                                    '--
                                End If
                                '--
                            End If

                            ' Ricerca data di cnv più recente di un bilancio US per il paziente e la malattia corrente
                            ' Se data ultimo US è più recente della datapartenza, la prendo come data di partenza.
                            Dim dataCnvUS As Date = Me.GenericProvider.BilancioProgrammato.GetLastDateBilUnsolved(codicePaziente, cod_malattia)
                            If (dataCnvUS > datapartenza) Then
                                datapartenza = dataCnvUS
                                isLastDateUS = True
                            End If

                        End If
                        '--
                        ' Se l'algoritmo prende come data di partenza quella dell'US, per il calcolo dell'intervallo 
                        ' utilizza il campo bil_tempo_cnv_prec. Altrimenti è usato il campo bil_intervallo.
                        intervallo = Me.GenericProvider.BilancioProgrammato.RetrieveIntervalByDate(cod_malattia, codicePaziente, dvSelected_rows(j)("bil_numero"), datapartenza, isLastDateUS)
                        '--
                    Else
                        '--
                        ' IL PAZIENTE HA GIA' EFFETTUATO VISITE. 
                        '--
                        ' L'algoritmo deve tener conto della data di ultima visita effettuata, ma anche delle date di convocazione 
                        ' di eventuali bilanci unsolved. Il calcolo dell'intervallo di giorni che deve passare prima della cnv 
                        ' avviene in due modi:
                        '   1 - in base alla data della visita + recente.
                        '   2 - in base alla data + recente della cnv di un bilancio US
                        ' Seleziono il maggiore dei due intervalli.
                        '--
                        intervallo = GenericProvider.BilancioProgrammato.RetrieveIntervalByVisit(cod_malattia, codicePaziente, dvSelected_rows(j)("bil_numero"))
                        Dim intervalloFromUS As Integer = GenericProvider.BilancioProgrammato.RetrieveIntervalByBilUnsolved(cod_malattia, codicePaziente, dvSelected_rows(j)("bil_numero"))

                        If (intervalloFromUS > intervallo) Then intervallo = intervalloFromUS
                        '--
                    End If
                    ' --
                    ' Calcolo le date di inizio e fine dell'intervallo possibile di convocazione per il bilancio
                    ' --
                    If (etaPaziente <= dvSelected_rows(j)("bil_eta_minima")) Then
                        '--
                        data_cnv_min = Date.Now.AddDays((dvSelected_rows(j)("bil_eta_minima") * Settings.AGGGIORNI) - etaPaziente)
                        If (intervallo > 0) Then data_cnv_min = data_cnv_min.AddDays(intervallo)
                        data_cnv_max = data_cnv_min.AddDays((dvSelected_rows(j)("bil_eta_massima") * Settings.AGGGIORNI) - (dvSelected_rows(j)("bil_eta_minima") * Settings.AGGGIORNI))
                        '--
                    Else
                        data_cnv_min = Date.Now
                        If (intervallo > 0) Then data_cnv_min = data_cnv_min.AddDays(intervallo)
                        data_cnv_max = Date.Now.AddDays((dvSelected_rows(j)("bil_eta_massima") * Settings.AGGGIORNI) - etaPaziente)
                        '--
                    End If
                    '--
                    ' Se flag_visita vale "S", è necessario che tra la data di cnv minima e quella massima 
                    ' intercorrano i giorni indicati nel parametro SCARTO_MASSIMO. 
                    If (flagVisita) Then
                        '--
                        If (Date.op_LessThan(data_cnv_min.AddDays(Settings.SCARTO_MASSIMO), data_cnv_max)) Then
                            '--
                            data_cnv_max = data_cnv_min.AddDays(Settings.SCARTO_MASSIMO)
                            '--
                        End If
                        '--
                    End If
                    '--
                    ' Cerco di reperire all'interno del datatable dtaNewConv (contenente le cnv da programmare) 
                    ' una data di convocazione che possa essere utile per la consegna del Bilancio che si sta processando,
                    ' cercando di dare precedenze alle date di convocazione per Vaccinazioni Obbligatorie.
                    '--
                    If (NewConvS > 0) Then
                        '--
                        data_cnv_vacc = dtaNewConv.Select("CONVOCAZIONE >= '" & data_cnv_min & "' AND CONVOCAZIONE <= '" & data_cnv_max & "'", "VAC_OBBLIGATORIA,CONVOCAZIONE ASC")
                        '--
                        If (Not data_cnv_vacc Is Nothing AndAlso data_cnv_vacc.Length > 0) Then
                            '--
                            ' VIENE ASSEGNATA LA DATA DI UNA CONVOCAZIONE APPENA CREATA
                            data_cnv = data_cnv_vacc(0)("CONVOCAZIONE")
                            '--
                        End If
                        '--
                    End If
                    '--
                    ' Se non è stata trovata nessuna data cnv all'interno del DataTable
                    ' cerco su db, tra le cnv già programmate, una data compatibile con l'intervallo generato 
                    '--
                    If (data_cnv = Date.MinValue) Then
                        '--
                        Using convocazioneprovider As New Biz.BizConvocazione(Me.GenericProvider, Me.Settings, Me.ContextInfos, Me.LogOptions)
                            data_cnv = convocazioneprovider.CercaConvocazione(codicePaziente, data_cnv_min, data_cnv_max)
                        End Using
                        '--
                    End If
                    '--
                    ' MGR 12/11/2008
                    ' Se non è stata trovata nessuna data di cnv, cerco nella collection dei bil che verranno programmati.
                    ' Altrimenti, assegno l'estremo inferiore dell'intervallo di convocazione calcolato in precedenza.
                    '-- 
                    If (data_cnv = Date.MinValue) Then
                        If (bilancioCollection.Count > 0) Then
                            data_cnv = CercaDataCnvBilanci(codicePaziente, data_cnv_min, data_cnv_max, bilancioCollection)
                            new_cnv = (data_cnv = Date.MinValue)
                        Else
                            new_cnv = True
                        End If

                        If (data_cnv = Date.MinValue) Then data_cnv = data_cnv_min
                    End If
                    '-- fine MGR

                    '--  
                    ' Creazione del bilancio in base ai dati calcolati e aggiunta alla collection dei bilanci che verranno programmati
                    ' Se è stata trovata una data di cnv già esistente a cui associare il bil da programmare, 
                    ' il flag new_cnv è rimasto false (assegnato all'inizio del ciclo). 
                    ' Altrimenti, dovrà essere creata una nuova cnv, quindi il flag vale true.
                    '--
                    Dim bilancio As New BilancioProgrammato(cod_malattia, dvSelected_rows(j)("bil_numero"), data_cnv, Constants.StatiBilancio.UNEXECUTED, new_cnv, codicePaziente)
                    bilancioCollection.Add(bilancio)
                    '--

                    Exit For    ' j

                End If
                '--
            Next j      ' --- Fine ciclo sui bilanci relativi alla malattia corrente --- '
            '--
        Next i      ' --- Fine ciclo sulle malattie --- '
        '--

        Return bilancioCollection
        '--
    End Function

    ''' <summary>
    ''' Estrae i bilanci che possono essere associati 'MANUALMENTE' ad una convocazione
    ''' </summary>
    ''' <param name="codicePaziente">Codice identificativo del paziente</param>
    ''' <param name="codiceMalattia"></param>
    ''' <history>
    ''' 	[pmontevecchi]	17/10/2007	Created
    ''' </history>
    Public Function BilanciDaProgrammareManuale(codicePaziente As Integer, codiceMalattia As String) As Collection.BilancioProgrammatoCollection

        Dim bilanciDaProgrammareMan As New Collection.BilancioProgrammatoCollection

        Using idr As IDataReader = Me.GenericProvider.BilancioProgrammato.ManGetBiltoProgr(codicePaziente, codiceMalattia)

            If (Not idr Is Nothing) Then

                Dim bilancio As BilancioProgrammato = Nothing

                Dim bil_numero As Integer = idr.GetOrdinal("bil_numero")
                Dim bil_mal_codice As Integer = idr.GetOrdinal("bil_mal_codice")
                Dim mal_descrizione As Integer = idr.GetOrdinal("mal_descrizione")

                While (idr.Read())

                    bilancio = New BilancioProgrammato(idr.GetString(bil_mal_codice), idr.GetDecimal(bil_numero))

                    bilancio.Descrizione_Malattia = idr(mal_descrizione)
                    bilanciDaProgrammareMan.Add(bilancio)

                End While

            End If

        End Using

        Return bilanciDaProgrammareMan

    End Function

#End Region

#Region " Split Condizioni "

    Public Shared Function SplittaCondizioni(condizioni As String) As String()

        Return condizioni.Split(":")

    End Function

    Public Shared Sub SplittaCondizione(condizione As String, ByRef codiceRisposta As String, ByRef codiceOsservazione As String, ByRef disabilitata As Boolean, ByRef collegata As Boolean)

        Dim condizioneValues As String() = condizione.Split("|")

        codiceRisposta = condizioneValues(0)
        codiceOsservazione = condizioneValues(1)
        disabilitata = condizioneValues(2) = "S"
        collegata = condizioneValues(3) = "S"

    End Sub

#End Region

#Region " Metodi di Update/Insert "

    ''' <summary>
    ''' Aggiorna il bilancio indicato.
    ''' </summary>
    ''' <param name="bilancioDaAggiornare"></param>
    Public Function AggiornaBilancio(bilancioDaAggiornare As BilancioProgrammato) As Boolean

        If bilancioDaAggiornare.Bil_id <> 0 Then
            Return Me.GenericProvider.BilancioProgrammato.Update(bilancioDaAggiornare)
        End If

        Return False

    End Function

    ''' <summary>
    ''' Salvataggio su db dei bilanci associati al paziente specificato
    ''' </summary>
    ''' <param name="bilanciDaProgrammare"></param>
    ''' <param name="pazCodice"></param>
    Public Sub ProgrammaBilanci(bilanciDaProgrammare As BilancioProgrammatoCollection, pazCodice As Integer)

        If (Not bilanciDaProgrammare Is Nothing AndAlso bilanciDaProgrammare.Count > 0) Then

            For i As Integer = 0 To bilanciDaProgrammare.Count - 1
                CreaNuovoBilancio(bilanciDaProgrammare(i))
            Next

        End If

    End Sub

    ''' <summary>
    ''' Salvataggio del bilancio specificato su db
    ''' </summary>
    ''' <param name="bilancioDaProgrammare"></param>
    Public Sub ProgrammaBilancio(bilancioDaProgrammare As BilancioProgrammato)

        CreaNuovoBilancio(bilancioDaProgrammare)

    End Sub

    Public Sub ProgrammaBilancioByVisita(visita As Entities.Visita)

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions())

            Dim now As DateTime = DateTime.Now

            Using convocazionebiz As New Biz.BizConvocazione(Me.GenericProvider, Me.Settings, Me.ContextInfos, Nothing)

                ' --- Ricerca bilancio corrente --- '
                Dim bilancioProgrammatoCollection As Collection.BilancioProgrammatoCollection

                Try
                    bilancioProgrammatoCollection = Me.CercaBilancio(visita.CodicePaziente, visita.BilancioNumero.Value, visita.MalattiaCodice)

                Catch ex As Exception

                    Dim stbMsg As New System.Text.StringBuilder()
                    stbMsg.Append("Errore in ricerca bilancio. ")
                    stbMsg.AppendFormat("Numero bilancio: {0} Malattia: {1} Paziente: {2}{3}", visita.BilancioNumero.Value, visita.MalattiaCodice, visita.CodicePaziente, vbNewLine)

                    Throw New Exception(stbMsg.ToString(), ex)

                End Try

                If Not bilancioProgrammatoCollection.IsNullOrEmpty() Then

                    Dim j As Int16

                    ' --- Update del bilancio corrente --- '
                    ' Il bilancio corrente era già stato programmato, adesso è stato consegnato per cui viene impostato a UX.
                    ' In più, se nella cnv relativa al bilancio corrente non ci sono vaccinazioni o bilanci in stato UX, cancella la cnv.
                    Try
                        For j = 0 To bilancioProgrammatoCollection.Count - 1

                            bilancioProgrammatoCollection(j).Bil_stato = Constants.StatiBilancio.EXECUTED

                            ' Se il bilancio è stato aggiornato, cancella la cnv, se non ci sono vac prog o altri bil UX.
                            If Me.AggiornaBilancio(bilancioProgrammatoCollection(j)) Then

                                Dim command As New Biz.BizConvocazione.EliminaConvocazioneEmptyCommand()
                                command.CodicePaziente = visita.CodicePaziente
                                command.DataConvocazione = bilancioProgrammatoCollection(j).Data_CNV
                                command.DataEliminazione = now
                                command.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.Esecuzione
                                command.NoteEliminazione = "Eliminazione convocazione per escuzione bilancio"
                                command.WriteLog = True

                                convocazionebiz.EliminaConvocazioneEmpty(command)

                            End If

                        Next

                    Catch ex As Exception

                        Dim stbMsg As New System.Text.StringBuilder()
                        stbMsg.Append("Errore in update bilanci e vaccinazioni. ")
                        stbMsg.AppendFormat("Numero bilancio: {0} Malattia: {1} Paziente: {2}{3}", bilancioProgrammatoCollection(j).N_bilancio.ToString(), bilancioProgrammatoCollection(j).Mal_codice, bilancioProgrammatoCollection(j).Paz_Codice.ToString(), Environment.NewLine)

                        Throw New Exception(stbMsg.ToString(), ex)

                    End Try

                Else

                    ' --- Programmazione del bilancio corrente --- '
                    ' Il bilancio corrente non era già programmato per cui lo crea (in stato EX).
                    Dim bilancio As New Entities.BilancioProgrammato(visita.MalattiaCodice, visita.BilancioNumero.Value, Date.MinValue, Constants.StatiBilancio.EXECUTED, False, visita.CodicePaziente)

                    Try
                        ' Inserisce il bilancio nella t_bil_programmati
                        Me.ProgrammaBilancio(bilancio)

                    Catch ex As Exception

                        Dim stbMsg As New System.Text.StringBuilder()

                        stbMsg.Append("Errore in Programmazione Bilancio. ")
                        stbMsg.AppendFormat("Numero bilancio: {0} Malattia: {1} Paziente: {2}{3}", visita.BilancioNumero.Value.ToString(), visita.MalattiaCodice, visita.CodicePaziente, Environment.NewLine)

                        Throw New Exception(stbMsg.ToString(), ex)

                    End Try

                End If

                ' --- Cancellazione bilancio successivo --- '
                Dim bilInfo As Entities.BilancioInfo = Nothing

                Try
                    ' Cerca tra i programmati l'ultimo bilancio relativo alla stessa malattia, in stato UX.
                    bilInfo = Me.GetLastBilUX(visita.CodicePaziente, visita.MalattiaCodice)

                    ' Se il bilancio esiste, è successivo, è un bilancio per malattia (flag_visita = "S") 
                    ' e le date di appuntamento e invio non sono valorizzate, lo cancella.
                    ' La cancellazione è stata aggiunta per far sì che, al calcolo delle cnv, 
                    ' venga riprogrammato anche questo bilancio, in base alla data di ultima visita.
                    If bilInfo IsNot Nothing AndAlso
                       bilInfo.NumeroBilancio > visita.BilancioNumero.Value AndAlso
                       bilInfo.FlagVisita AndAlso
                       bilInfo.DataAppuntamento = Date.MinValue AndAlso
                       bilInfo.DataInvio = Date.MinValue Then

                        ' Cancellazione bilancio successivo
                        If Me.CancellaBilancio(bilInfo.CodicePaziente, bilInfo.CodiceMalattia, bilInfo.NumeroBilancio) Then

                            ' Cancellazione convocazione relativa, se non ci sono vaccinazioni nè altri bilanci.
                            Dim command As New Biz.BizConvocazione.EliminaConvocazioneEmptyCommand()
                            command.CodicePaziente = bilInfo.CodicePaziente
                            command.DataConvocazione = bilInfo.DataConvocazione
                            command.DataEliminazione = now
                            command.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.EliminazioneConvocazione
                            command.NoteEliminazione = "Eliminazione convocazione per cancellazione bilancio"
                            command.WriteLog = True

                            convocazionebiz.EliminaConvocazioneEmpty(command)

                        End If
                    End If

                Catch ex As Exception

                    Dim msg As New System.Text.StringBuilder()

                    msg.AppendFormat("Maschera GestioneBilancio - Errore in consegna bilancio{0}", Environment.NewLine)
                    msg.AppendFormat("Paziente: {0}{1}", visita.CodicePaziente, Environment.NewLine)
                    msg.AppendFormat("Malattia: {0}{1}", visita.MalattiaCodice, Environment.NewLine)
                    msg.AppendFormat("Numero bilancio consegnato: {0}{1}", visita.BilancioNumero.Value.ToString(), Environment.NewLine)

                    If (bilInfo Is Nothing) Then
                        msg.AppendFormat("Errore in ricerca del bilancio successivo.{0}", Environment.NewLine)
                    Else
                        msg.AppendFormat("Errore in cancellazione del bilancio successivo.", Environment.NewLine)
                        msg.AppendFormat("Numero bilancio successivo: {0}{1}", bilInfo.NumeroBilancio.ToString(), Environment.NewLine)
                    End If

                    Common.Utility.EventLogHelper.EventLogWrite(ex, msg.ToString(), Me.ContextInfos.IDApplicazione)

                End Try

            End Using

            transactionScope.Complete()

        End Using

    End Sub

    ''' <summary>
    ''' Restituisce il bilancio in stato unexecuted con numero più alto per il paziente e la malattia specificati.
    ''' Se non lo trova restituisce Nothing.
    ''' </summary>
    ''' <param name="pazCodice"></param>
    ''' <param name="malCodice"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetLastBilUX(pazCodice As Integer, malCodice As String) As Entities.BilancioInfo

        Return Me.GenericProvider.BilancioProgrammato.GetLastBil(pazCodice, malCodice, Constants.StatiBilancio.UNEXECUTED)

    End Function

#End Region

#Region " Metodi di Delete "

    Public Function CancellaBilanciByVisita(visita As Entities.Visita) As Boolean

        Dim convocazioneDeleted As Boolean

        Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions())

            Dim now As DateTime = DateTime.Now

            Dim bilanciProgrammati As Collection.BilancioProgrammatoCollection =
                Me.CercaBilancio(visita.CodicePaziente, visita.BilancioNumero.Value, visita.MalattiaCodice)

            If (Not bilanciProgrammati Is Nothing AndAlso bilanciProgrammati.Count > 0) Then

                Using convocazioneBiz As New Biz.BizConvocazione(Me.GenericProvider, Me.Settings, Me.ContextInfos, Nothing)

                    For j As Int16 = 0 To bilanciProgrammati.Count - 1

                        ' elimino il bilancio e cancello la vaccinazione dove necessario
                        If Me.CancellaBilancio(bilanciProgrammati(j)) Then

                            Dim command As New Biz.BizConvocazione.EliminaConvocazioneEmptyCommand()
                            command.CodicePaziente = visita.CodicePaziente
                            command.DataConvocazione = bilanciProgrammati(j).Data_CNV
                            command.DataEliminazione = now
                            command.IdMotivoEliminazione = Constants.MotiviEliminazioneAppuntamento.EliminazioneConvocazione
                            command.NoteEliminazione = "Eliminazione convocazione per cancellazione bilancio da modifica visita"
                            command.WriteLog = True

                            Dim result As Biz.BizConvocazione.EliminaConvocazioneEmptyResult = convocazioneBiz.EliminaConvocazioneEmpty(command)
                            If result.Success Then
                                convocazioneDeleted = True
                            End If

                        End If
                    Next

                End Using

            End If

            transactionScope.Complete()

        End Using

        Return convocazioneDeleted

    End Function
    ''' <summary>
    ''' Cancellazione del bilancio specificato. Restituisce true se ha cancellato il record, false altrimenti.
    ''' </summary>
    ''' <param name="pazCodice"></param>
    ''' <param name="malCodice"></param>
    ''' <param name="numBilancio"></param>
    Public Function CancellaBilancio(pazCodice As Integer, malCodice As String, numBilancio As Integer) As Boolean

        Return Me.GenericProvider.BilancioProgrammato.DeleteRecord(pazCodice, numBilancio, malCodice)

    End Function

    ' E' necessario dircordarsi di implementare un meccanismo di cancellazione a cascata anche dei solleciti per i bilanci <-----
    ''' <summary>
    ''' Cancellazione del bilancio specificato. Restituisce true se ha cancellato il record, false altrimenti.
    ''' </summary>
    ''' <param name="bilancioDaCancellare"></param>
    ''' <returns></returns>
    ''' <remarks>Controlla che l'id del bilancio sia un numero diverso da zero</remarks>
    Public Function CancellaBilancio(bilancioDaCancellare As BilancioProgrammato) As Boolean

        If IsNumeric(bilancioDaCancellare.Bil_id) AndAlso bilancioDaCancellare.Bil_id <> 0 Then

            Return Me.GenericProvider.BilancioProgrammato.DeleteRecord(bilancioDaCancellare.Paz_Codice, bilancioDaCancellare.N_bilancio, bilancioDaCancellare.Mal_codice)

        End If

        Return False

    End Function

    ''' <summary>
    ''' Cancellazione di tutti i bilanci del paziente relativi alla malattia specificata. 
    ''' Vengono cancellati anche eventuali solleciti. Restituisce true se ha cancellato almeno un record, false altrimenti.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="codiceMalattia"></param>
    Public Function CancellaBilanciDiMalattia(codicePaziente As Integer, codiceMalattia As String) As Boolean

        If IsNumeric(codicePaziente) And Not codiceMalattia Is Nothing Then

            ' Cancellazione solleciti
            Me.GenericProvider.SollecitiBilanci.DeleteRecord(codicePaziente, codiceMalattia)

            ' Cancellazione bilanci
            Return Me.GenericProvider.BilancioProgrammato.DeleteRecord(codicePaziente, codiceMalattia)

        End If

        Return False

    End Function

#End Region

#Region " Metodi di Selezione "

    ''' <summary>
    ''' Restituisce una collection di bilanci in base ai dati specificati
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="numeroBilancio"></param>
    ''' <param name="codiceMalattia"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CercaBilancio(codicePaziente As Integer, numeroBilancio As Integer, codiceMalattia As String) As BilancioProgrammatoCollection

        Dim dtBilancio As DataTable = Me.GenericProvider.BilancioProgrammato.GetFromKey(codicePaziente, numeroBilancio, codiceMalattia)

        If dtBilancio Is Nothing Then Return Nothing

        Dim bilanci As New BilancioProgrammatoCollection()
        Dim bilancioTrovato As BilancioProgrammato = Nothing

        For i As Integer = 0 To dtBilancio.Rows.Count - 1

            Dim dataCnv As New Date(1901, 1, 1)

            If Not dtBilancio.Rows(i)("bip_cnv_data") Is System.DBNull.Value Then
                dataCnv = dtBilancio.Rows(i)("bip_cnv_data")
            End If

            bilancioTrovato = New BilancioProgrammato(dtBilancio.Rows(i)("bip_mal_codice"),
                                                      dtBilancio.Rows(i)("bip_bil_numero"),
                                                      dataCnv,
                                                      dtBilancio.Rows(i)("bip_stato"),
                                                      False,
                                                      dtBilancio.Rows(i)("bip_paz_codice"))

            bilancioTrovato.Bil_id = dtBilancio.Rows(i)("id")

            bilanci.Add(bilancioTrovato)

        Next

        Return bilanci

    End Function

    ''' <summary>
    ''' Restituisce una collection di bilanci in base al paziente e alla data specificati
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataConvocazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CercaBilanci(codicePaziente As Integer, dataConvocazione As Date) As BilancioProgrammatoCollection

        Return Me.CercaBilanci(codicePaziente, dataConvocazione, Nothing)

    End Function

    ''' <summary>
    ''' Restituisce una collection di bilanci in base al paziente e alla data specificati
    ''' Se è valorizzato il parametro statoBilancio, restituisce solo i bilanci avento lo stato specificato.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataConvocazione"></param>
    ''' <param name="statoBilancio"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CercaBilanci(codicePaziente As Integer, dataConvocazione As Date, statoBilancio As String) As BilancioProgrammatoCollection

        Dim dtBilancio As DataTable = Me.GenericProvider.BilancioProgrammato.GetFromKey(codicePaziente, dataConvocazione)

        If dtBilancio Is Nothing Then Return Nothing

        Dim bilanci As New BilancioProgrammatoCollection()

        For i As Integer = 0 To dtBilancio.Rows.Count - 1

            If Not String.IsNullOrEmpty(statoBilancio) AndAlso dtBilancio.Rows(i)("bip_stato").ToString() <> statoBilancio Then Continue For

            Dim bilancioTrovato As New BilancioProgrammato(dtBilancio.Rows(i)("bip_mal_codice"),
                                                           dtBilancio.Rows(i)("bip_bil_numero"),
                                                           dtBilancio.Rows(i)("bip_cnv_data"),
                                                           dtBilancio.Rows(i)("bip_stato"),
                                                           False,
                                                           dtBilancio.Rows(i)("bip_paz_codice"))

            bilancioTrovato.Bil_id = dtBilancio.Rows(i)("id")

            bilanci.Add(bilancioTrovato)

        Next

        Return bilanci

    End Function

    ''' <summary>
    ''' Restituisce un datatable di bilanci in base al paziente e alla data specificati
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataConvocazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CercaBilanciDt(codicePaziente As Integer, dataConvocazione As Date) As DataTable

        Return Me.GenericProvider.BilancioProgrammato.GetFromKey(codicePaziente, dataConvocazione)

    End Function

    ''' <summary>
    ''' Restituisce una stringa contenente il numero del bilancio e la descrizione della malattia, per il paziente e la convocazione specificati.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataConvocazione"></param>
    ''' <param name="stato"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDescrizioneBilancioMalattia(codicePaziente As Integer, dataConvocazione As Date, stato As String) As String

        Dim descrizione As New System.Text.StringBuilder()

        Dim bilanciMalattieList As List(Of KeyValuePair(Of Integer, String)) =
            Me.GenericProvider.BilancioProgrammato.GetBilanciMalattiePaziente(codicePaziente, dataConvocazione, stato)

        Dim enumerator As List(Of KeyValuePair(Of Integer, String)).Enumerator = bilanciMalattieList.GetEnumerator()

        While enumerator.MoveNext()

            If String.IsNullOrEmpty(enumerator.Current.Value) Then
                descrizione.AppendFormat("{0} - Malattia non associata", enumerator.Current.Key)
            Else
                descrizione.AppendFormat("{0} - Malattia: {1}", enumerator.Current.Key, enumerator.Current.Value)
            End If

            descrizione.Append("<br/>")

        End While

        If descrizione.Length > 0 Then
            descrizione.Remove(descrizione.Length - 5, 5)
        Else
            descrizione.Append("Nessun Bilancio")
        End If

        Return descrizione.ToString()

    End Function

    Public Function VaccinazioneAssociata(codicePaziente As Integer, dataConvocazione As Date) As Boolean

        Return Me.GenericProvider.BilancioProgrammato.VaccinazioneAssociata(codicePaziente, dataConvocazione)

    End Function

    Public Function VerificaDataCNV(codicePaziente As Integer, numeroBilancio As Integer, codiceMalattia As String, dataConvocazione As Date) As Boolean

        Dim paziente As Entities.Paziente
        Dim bilancio As Entities.BilancioProgrammato

        Using pazienteBiz As New Biz.BizPaziente(Me.GenericProvider, Me.Settings, Me.ContextInfos, Me.LogOptions)
            paziente = pazienteBiz.CercaPaziente(codicePaziente)
        End Using

        If paziente.Paz_Codice <> 0 Then

            bilancio = Me.GenericProvider.AnaBilancio.GetFromKey(numeroBilancio, codiceMalattia)

            If (Not bilancio Is Nothing) Then

                If (bilancio.Eta_Minima >= DateDiff(DateInterval.DayOfYear, paziente.Data_Nascita, dataConvocazione, , ) _
                    And bilancio.Eta_Massima <= DateDiff(DateInterval.DayOfYear, paziente.Data_Nascita, dataConvocazione, , )) Then

                    Return True

                End If

            End If

        End If

        Return False

    End Function

    ''' <summary>
    ''' Restituisce il numero di bilanci del paziente specificato
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="isGestioneCentrale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CountBilanciPaziente(codicePaziente As String, isGestioneCentrale As Boolean) As Integer

        If isGestioneCentrale Then Return 0

        Return Me.GenericProvider.BilancioProgrammato.CountBilanciPaziente(codicePaziente)

    End Function

    Public Function GetBilanciPaziente(codicePaziente As Integer) As List(Of BilancioProgrammato)

        Dim lst As List(Of BilancioProgrammato) = Me.GenericProvider.AnaBilancio.GetBilanciPaziente(codicePaziente, Me.Settings.VACPROG_TIPOLOGIA_MALATTIA, Me.Settings.VACPROG_BIL_CONSEGNATO_A)
        Dim dataNascita As DateTime = Me.GenericProvider.Paziente.GetDataNascita(codicePaziente)
        Dim eta As Integer = PazienteHelper.CalcoloEta(dataNascita).GiorniTotali

        lst = lst.Where(Function(p) p.Eta_Minima <= eta AndAlso p.Eta_Massima >= eta).ToList()
        lst = lst.GroupBy(Function(p) p.Mal_codice).Select(Function(p) p.OrderBy(Function(o) o.N_bilancio).First()).ToList()

        For i As Integer = lst.Count - 1 To 0 Step -1
            If Not Me.GenericProvider.BilancioProgrammato.VerifyInterval(lst(i).Mal_codice, codicePaziente, Date.MinValue, lst(i).N_bilancio) Then
                lst.Remove(lst(i))
            End If
        Next

        Return lst

    End Function

    ''' <summary>
    ''' Restituisce un datatable contenente tutti i bilanci associati alla malattia "NESSUNA"
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDtBilanciNessunaMalattia() As DataTable

        Return Me.GenericProvider.BilancioProgrammato.GetDtBilanci(Me.Settings.CODNOMAL)

    End Function

#Region " Caricamento visite locale/centrale "

    ''' <summary>
    ''' Restituisce la visita con id specificato. 
    ''' Se il parametro isGestioneCentrale vale true, restituisce la visita con i dati della usl corrente, 
    ''' recuperando l'id dal centrale in base ai parametri id e usl (relativi alla usl di inserimento e non alla usl corrente)
    ''' </summary>
    ''' <param name="idVisita"></param>
    ''' <param name="codiceUslInserimento"></param>
    ''' <param name="isGestioneCentrale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetVisitaBilancio(idVisita As Integer, codiceUslInserimento As String, isGestioneCentrale As Boolean) As Visita

        ' [Unificazione Ulss]: isGestioneCentrale => NON PIU' PREVISTA
        'If isGestioneCentrale Then

        '    ' Recupero dati visita direttamente dalla usl di inserimento
        '    Using genericProviderUsl As DAL.DbGenericProvider = Me.GetDBGenericProviderByCodiceUslGestita(codiceUslInserimento)

        '        Return genericProviderUsl.BilancioProgrammato.GetVisitaBilancio(idVisita)

        '    End Using

        'End If

        Return GenericProvider.BilancioProgrammato.GetVisitaBilancio(idVisita)

    End Function

    ''' <summary>
    ''' Restituisce un datatable con i dati delle visite e relativi bilanci.
    ''' Se isGestioneCentrale vale true, vengono recuperate dal centrale le visite con visibilità "V" e dalla usl di inserimento di ogni visita vengono letti i dati relativi.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="isGestioneCentrale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDtVisiteBilanci(codicePaziente As String, isGestioneCentrale As Boolean) As DataTable

        ' [Unificazione Ulss]: isGestioneCentrale => NON PIU' PREVISTA
        'If isGestioneCentrale Then

        '    Return Me.GetVisiteCentralizzate(codicePaziente)

        'End If

        Return Me.GenericProvider.BilancioProgrammato.GetDtVisiteBilanci(codicePaziente)

    End Function

    'Private Function GetVisiteCentralizzate(codicePazienteCentrale As String) As DataTable

    '    ' Elenco visite del paziente presenti in centrale con visibilità concessa
    '    Dim listVisiteCentrale As IEnumerable(Of Entities.VisitaCentrale) =
    '        Me.GenericProviderCentrale.VisitaCentrale.GetVisitaCentraleEnumerable(codicePazienteCentrale, Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente)

    '    ' Se nessun risultato: deve continuare per creare la struttura del datatable (che verrà restituito vuoto, ma con le colonne impostate)
    '    If listVisiteCentrale Is Nothing Then
    '        listVisiteCentrale = (New List(Of Entities.VisitaCentrale)()).AsEnumerable()
    '    End If

    '    Dim dtVisiteCentralizzate As New DataTable()

    '    ' Elenco usl inserimento delle visite
    '    Dim codiciUslInserimentoVisite As List(Of String) =
    '        listVisiteCentrale.Distinct(New UslInserimentoVisiteComparer()).Select(Function(v) v.CodiceUslVisita).ToList()

    '    ' Per ogni usl in cui sono presenti visite del paziente, recupero i dati necessari in base a paz_codice locale e vis_id
    '    For Each codiceUslInserimentoVisita As String In codiciUslInserimentoVisite

    '        Dim codiceUsl As String = codiceUslInserimentoVisita

    '        ' Elenco id delle visite nella usl corrente
    '        Dim listVisiteUsl As IEnumerable(Of Entities.VisitaCentrale) =
    '            listVisiteCentrale.Where(Function(v) v.CodiceUslVisita = codiceUsl)

    '        If Not listVisiteUsl Is Nothing AndAlso listVisiteUsl.Count > 0 Then

    '            ' Query sulla singola usl per reperimento dati visite del paziente (con paz_codice e i vis_id presenti nella lista)
    '            Dim dtVisite As DataTable = Me.GetDtVisiteByUslInserimentoVisitaAndId(codiceUsl, listVisiteUsl.Select(Function(v) v.IdVisita).ToList())

    '            ' Aggiunta risultati al datatable delle vaccinazioni da restituire
    '            If Not dtVisite Is Nothing Then

    '                If dtVisiteCentralizzate.Columns.Count = 0 Then

    '                    dtVisiteCentralizzate = dtVisite.Copy()

    '                Else

    '                    For Each row As DataRow In dtVisite.Rows

    '                        Dim newRow As DataRow = dtVisiteCentralizzate.NewRow()
    '                        newRow.ItemArray = row.ItemArray
    '                        dtVisiteCentralizzate.Rows.Add(newRow)

    '                    Next

    '                End If

    '            End If

    '        End If

    '    Next

    '    ' Se non è stata creata la struttura delle colonne, eseguo una query "fittizia" per averla comunque.
    '    If dtVisiteCentralizzate.Columns.Count = 0 Then
    '        Dim listId As New List(Of Long)()
    '        listId.Add(0)
    '        dtVisiteCentralizzate = Me.GenericProvider.BilancioProgrammato.GetDtVisiteBilanciById(listId)
    '    End If

    '    Return dtVisiteCentralizzate

    'End Function

    'Private Function GetDtVisiteByUslInserimentoVisitaAndId(codiceUsl As String, listIdVisiteUsl As List(Of Long)) As DataTable

    '    If listIdVisiteUsl Is Nothing Then Return Nothing

    '    Using genericProviderUsl As DbGenericProvider = Me.GetDBGenericProviderByCodiceUslGestita(codiceUsl)

    '        Return genericProviderUsl.BilancioProgrammato.GetDtVisiteBilanciById(listIdVisiteUsl)

    '    End Using

    'End Function

#End Region

#Region " Caricamento dati bilancio locale/centrale "

    ''' <summary>
    ''' Restituisce un oggetto contenente i datatable con i dati di Sezioni, Condizioni, Risposte Possibili, Domande e Risposte per il bilancio relativo alla visita specificata.
    ''' </summary>
    ''' <param name="idVisita"></param>
    ''' <param name="codiceUslInserimento"></param>
    ''' <param name="codicePaziente"></param>
    ''' <param name="numeroBilancio"></param>
    ''' <param name="codiceMalattia"></param>
    ''' <param name="sessoPaziente"></param>
    ''' <param name="dataVisita"></param>
    ''' <param name="isGestioneCentrale"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDatiBilancioPaziente(idVisita As Integer, codiceUslInserimento As String, codicePaziente As String, numeroBilancio As Integer, codiceMalattia As String, sessoPaziente As String, dataVisita As DateTime, isGestioneCentrale As Boolean, dataRegistrazione As Date) As DatiBilancioPazienteResult

        Dim result As New DatiBilancioPazienteResult()

        ' [Unificazione Ulss]: isGestioneCentrale => NON PIU' PREVISTA
        'If isGestioneCentrale Then
        '    result.Sezioni = Me.GenericProviderCentrale.AnaBilancio.GetSezioni(numeroBilancio, codiceMalattia)
        '    result.CondizioniBilancio = Me.GenericProviderCentrale.AnaBilancio.GetCondizioni(numeroBilancio, codiceMalattia)
        '    result.RispostePossibili = Me.GenericProviderCentrale.AnaBilancio.GetRispostePossibili(numeroBilancio, codiceMalattia)
        '    result.Domande = Me.GenericProviderCentrale.AnaBilancio.GetDomande(numeroBilancio, codiceMalattia, sessoPaziente)
        '    result.Risposte = Me.GetRisposteBilancioCentralizzate(idVisita, codiceUslInserimento)
        'Else

        result.Sezioni = GenericProvider.AnaBilancio.GetSezioni(numeroBilancio, codiceMalattia)
        result.CondizioniBilancio = GenericProvider.AnaBilancio.GetCondizioni(numeroBilancio, codiceMalattia)
        result.RispostePossibili = GenericProvider.AnaBilancio.GetRispostePossibili(numeroBilancio, codiceMalattia)
        result.Domande = GenericProvider.AnaBilancio.GetDomande(numeroBilancio, codiceMalattia, sessoPaziente, dataRegistrazione)
        result.Risposte = GenericProvider.BilancioProgrammato.GetRisposteBilancio(numeroBilancio, codiceMalattia, codicePaziente, dataVisita)

        'End If

        Return result

    End Function

    ''' <summary>
    ''' Restituisce un oggetto contenente i datatable con i dati di Sezioni, Condizioni, Risposte Possibili e Domande per il bilancio relativo alla visita specificata.
    ''' Se il tipo di compilazione vale "RispostaPrecedente", valorizza il datatable Risposte con le risposte relative al bilancio precedente, altrimenti lo restituisce Nothing.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="numeroBilancio"></param>
    ''' <param name="codiceMalattia"></param>
    ''' <param name="sessoPaziente"></param>
    ''' <param name="tipoCompilazioneBilancio"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDatiBilancioPaziente(codicePaziente As String, numeroBilancio As Integer, codiceMalattia As String, sessoPaziente As String, tipoCompilazioneBilancio As Enumerators.TipoCompilazioneBilancio, dataRegistrazione As Date) As DatiBilancioPazienteResult

        Dim result As New DatiBilancioPazienteResult()

        result.Sezioni = Me.GenericProvider.AnaBilancio.GetSezioni(numeroBilancio, codiceMalattia)
        result.CondizioniBilancio = Me.GenericProvider.AnaBilancio.GetCondizioni(numeroBilancio, codiceMalattia)
        result.RispostePossibili = Me.GenericProvider.AnaBilancio.GetRispostePossibili(numeroBilancio, codiceMalattia)
        result.Domande = Me.GenericProvider.AnaBilancio.GetDomande(numeroBilancio, codiceMalattia, sessoPaziente, dataRegistrazione)

        result.Risposte = Nothing

        If tipoCompilazioneBilancio = Enumerators.TipoCompilazioneBilancio.RispostaPrecedente Then

            If numeroBilancio > 1 Then

                Dim dataVisitaPrecedente As DateTime = Me.GenericProvider.BilancioProgrammato.GetUltimaDataVisitaBilancio(numeroBilancio - 1, codiceMalattia, codicePaziente)

                If dataVisitaPrecedente <> DateTime.MinValue Then

                    result.Risposte = Me.GenericProvider.BilancioProgrammato.GetRisposteBilancio(numeroBilancio - 1, codiceMalattia, codicePaziente, dataVisitaPrecedente)

                End If

            End If

        End If

        Return result

    End Function

    'Private Function GetRisposteBilancioCentralizzate(idVisita As Integer, codiceUslInserimento As String) As DataTable

    '    Dim dtRisposteCentralizzate As New DataTable()

    '    ' Recupero dati della visita dalla usl di appartenenza
    '    Using genericProviderUsl As DbGenericProvider = Me.GetDBGenericProviderByCodiceUslGestita(codiceUslInserimento)

    '        dtRisposteCentralizzate = genericProviderUsl.BilancioProgrammato.GetRisposteBilancio(idVisita)

    '    End Using

    '    If dtRisposteCentralizzate.Columns.Count = 0 Then

    '        Return Me.GenericProviderCentrale.BilancioProgrammato.GetRisposteBilancio(0)

    '    End If

    '    Return dtRisposteCentralizzate

    'End Function

#End Region

#End Region

#End Region

#Region " Private "

    ''' <summary>
    ''' Restituisce la più recente tra le date di convocazione dei bilanci da programmare, 
    ''' nell'intervallo di date specificato.
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataCnvMin"></param>
    ''' <param name="dataCnvMax"></param>
    ''' <param name="bilanciDaProgrammare"></param>
    Private Function CercaDataCnvBilanci(codicePaziente As Integer, dataCnvMin As Date, dataCnvMax As Date, bilanciDaProgrammare As BilancioProgrammatoCollection) As Date

        Dim listDateCnv As New List(Of Date)()

        If (bilanciDaProgrammare Is Nothing OrElse bilanciDaProgrammare.Count = 0) Then Return Date.MinValue

        listDateCnv = (From bil As BilancioProgrammato In bilanciDaProgrammare
                       Where bil.Paz_Codice = codicePaziente _
                       And bil.Data_CNV.Date >= dataCnvMin.Date _
                       And bil.Data_CNV.Date <= dataCnvMax.Date _
                       Select bil.Data_CNV).ToList()

        If listDateCnv.Count = 0 Then Return Date.MinValue

        Return listDateCnv.Min()

    End Function

    Private Sub CreaNuovoBilancio(bilancioDaProgrammare As BilancioProgrammato)

        Me.GenericProvider.BilancioProgrammato.NewRecord(bilancioDaProgrammare)

    End Sub

#End Region

End Class
