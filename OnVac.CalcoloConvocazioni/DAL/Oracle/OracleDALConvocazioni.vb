Imports System.Data
Imports System.Collections.Generic

Imports Onit.OnAssistnet.OnVac.Queries
Imports Onit.OnAssistnet.OnVac.DAL


Namespace DAL.Oracle

    Friend Class OracleDALConvocazioni
        Implements DAL.IDALConvocazioni

#Region " Properties "

        Private IsConnectionExternal As Boolean

        Private _provider As String
        Public ReadOnly Property Provider() As String Implements IDALConvocazioni.Provider
            Get
                Return _provider
            End Get
        End Property

        Private _conn As IDbConnection
        Public ReadOnly Property Connection() As System.Data.IDbConnection Implements IDALConvocazioni.Connection
            Get
                Return _conn
            End Get
        End Property

        Private _tx As IDbTransaction
        Public ReadOnly Property Transaction() As System.Data.IDbTransaction Implements IDALConvocazioni.Transaction
            Get
                Return _tx
            End Get
        End Property

#End Region

#Region " Disposing "

        Public Sub Dispose() Implements System.IDisposable.Dispose

            If Not Me.IsConnectionExternal Then
                If Not _tx Is Nothing Then _tx.Dispose()
                If Not _conn Is Nothing Then _conn.Close()
            End If

        End Sub

#End Region

#Region " Costruttori "

        Public Sub New(provider As String, connectionString As String)

            _provider = provider
            _conn = Me.GetConnection(connectionString)

            Me.IsConnectionExternal = False

        End Sub

        Public Sub New(provider As String, ByRef conn As IDbConnection, ByRef tx As IDbTransaction)

            _provider = provider
            _conn = conn
            _tx = tx

            Me.IsConnectionExternal = True

        End Sub

#End Region

#Region " Private "

        Private Function GetConnection(connectionString As String) As OracleClient.OracleConnection

            ' Se non ho la stringa di connessione, sollevo un'eccezione
            If connectionString = String.Empty Then Throw New Exception("DAL: Impossibile creare la connessione")

            ' --- Creazione della connessione --- '
            Return New OracleClient.OracleConnection(connectionString)

        End Function

        ' Apre la connessione, se è chiusa, e restituisce true se l'ha aperta, false altrimenti
        Private Function ConditionalOpenConnection() As Boolean

            If Me.Connection.State = ConnectionState.Closed Then
                Me.Connection.Open()
                Return True
            End If

            Return False

        End Function

        Protected Friend Sub ConditionalCloseConnection(ownConnection As Boolean)

            If ownConnection AndAlso Not Me.Connection Is Nothing AndAlso Me.Connection.State = ConnectionState.Open Then

                Me.Connection.Close()

            End If

        End Sub

#End Region

#Region " Protected "

        ' Restituisce dbnull se la stringa passata per parametro è vuota. Altrimenti restituisce la stringa stessa.
        ' Utilizzato per il passaggio dei parametri all'oracleCommand senza dover effettuare tutte le volte il controllo esplicito di ogni parametro
        Protected Friend Shared Function GetStringParam(stringValue As String, toUppercase As Boolean) As Object

            If stringValue = String.Empty Then Return DBNull.Value
            If toUppercase Then Return stringValue.ToUpper()

            Return stringValue

        End Function

        ' Restituisce dbnull se la data passata per parametro è minvalue. Altrimenti restituisce la data stessa.
        ' Utilizzato per il passaggio dei parametri all'oracleCommand senza dover effettuare tutte le volte il controllo esplicito di ogni parametro
        Protected Friend Shared Function GetDateParam(dateValue As DateTime) As Object

            If dateValue = Date.MinValue Then Return DBNull.Value
            Return dateValue

        End Function

        ' Restituisce dbnull se il numero passato per parametro è -1. Altrimenti restituisce il numero stesso.
        ' Utilizzato per il passaggio dei parametri all'oracleCommand senza dover effettuare tutte le volte il controllo esplicito di ogni parametro
        Protected Friend Shared Function GetIntParam(intValue As Integer) As Object

            If intValue = -1 Then Return DBNull.Value
            Return intValue

        End Function

#End Region

#Region " IDALConvocazioni "

        Public Sub BeginTransaction() Implements IDALConvocazioni.BeginTransaction

            Me.ConditionalOpenConnection()
            Me._tx = Me.Connection.BeginTransaction()

        End Sub

#Region " Paziente "

        ''' <summary>
        ''' Restituisce un oggetto DatiPazienteClass contenente i dati del paziente con codice specificato
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        Public Function GetDatiPaziente(codicePaziente As Integer) As ObjectModel.DatiPazienteClass Implements IDALConvocazioni.GetDatiPaziente

            Dim datiPaziente As ObjectModel.DatiPazienteClass = Nothing

            Dim ownConnection As Boolean = False

            Try
                ownConnection = Me.ConditionalOpenConnection()

                Using cmd As New OracleClient.OracleCommand(Queries.Convocazioni.OracleQueries.selDatiPaziente, Me.Connection)

                    If Not _tx Is Nothing Then cmd.Transaction = _tx

                    cmd.Parameters.AddWithValue("cod", codicePaziente)

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim pos_cod As Int16 = idr.GetOrdinal("paz_codice")
                            Dim pos_cognome As Int16 = idr.GetOrdinal("paz_cognome")
                            Dim pos_nome As Int16 = idr.GetOrdinal("paz_nome")
                            Dim pos_nascita As Int16 = idr.GetOrdinal("paz_data_nascita")
                            Dim pos_cns As Int16 = idr.GetOrdinal("paz_cns_codice")
                            Dim pos_stato_vac As Int16 = idr.GetOrdinal("paz_stato")

                            If idr.Read() Then
                                datiPaziente = New ObjectModel.DatiPazienteClass()
                                datiPaziente.Codice = idr.GetDecimal(pos_cod)
                                datiPaziente.Cognome = idr.GetStringOrDefault(pos_cognome)
                                datiPaziente.Nome = idr.GetStringOrDefault(pos_nome)
                                datiPaziente.DataNascita = idr.GetDateTimeOrDefault(pos_nascita)
                                datiPaziente.CodiceConsultorio = idr.GetStringOrDefault(pos_cns)
                                datiPaziente.StatoVaccinale = idr.GetStringOrDefault(pos_stato_vac)
                            End If

                        End If

                    End Using

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return datiPaziente

        End Function

#End Region

#Region " Vaccinazioni Sostitute "

        ''' <summary>
        ''' Restituisce il codice della vaccinazione sostituta associato alla vaccinazione specificata
        ''' </summary>
        Public Function GetVaccinazioneSostituta(codiceVaccinazione As String) As String Implements IDALConvocazioni.GetVaccinazioneSostituta

            Dim codiceVaccinazioneSostituta As String = String.Empty

            Dim ownConnection As Boolean = False

            Try
                ownConnection = Me.ConditionalOpenConnection()

                Using cmd As New OracleClient.OracleCommand(Queries.Convocazioni.OracleQueries.selVacSostByVacCod, Me.Connection)

                    If Not _tx Is Nothing Then cmd.Transaction = _tx

                    cmd.Parameters.AddWithValue("cod_vac", GetStringParam(codiceVaccinazione, False))

                    Dim obj As Object = cmd.ExecuteScalar()
                    If Not obj Is Nothing Then
                        codiceVaccinazioneSostituta = obj.ToString()
                    End If

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return codiceVaccinazioneSostituta

        End Function

        ''' <summary>
        ''' Controlla se la vaccinazione ha un'inadempienza o un'esclusione legate al paziente specificato
        ''' </summary>
        ''' <param name="codiceVaccinazione"></param>
        ''' <param name="codicePaziente"></param>
        Function ControllaInadempienzaEsclusioneVacPaz(codiceVaccinazione As String, codicePaziente As Integer) As Boolean Implements IDALConvocazioni.ControllaInadempienzaEsclusioneVacPaz

            Dim value As Boolean = False

            Dim ownConnection As Boolean = False

            Try
                ownConnection = Me.ConditionalOpenConnection()

                Using cmd As New OracleClient.OracleCommand(Queries.Convocazioni.OracleQueries.chkInadEsclVacByPaz, Me.Connection)

                    If Not _tx Is Nothing Then cmd.Transaction = _tx

                    cmd.Parameters.AddWithValue("cod_vac", GetStringParam(codiceVaccinazione, False))
                    cmd.Parameters.AddWithValue("cod_paz", codicePaziente)

                    Dim obj As Object = cmd.ExecuteScalar()

                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        Try
                            ' Se la query restituisce il valore 1, significa che ci sono inadempienze e/o esclusioni per la vacc
                            value = (Convert.ToInt32(obj) = 1)
                        Catch ex As Exception
                            value = False
                        End Try
                    End If

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return value

        End Function

        ''' <summary>
        ''' Restituisce il massimo valore del richiamo del paziente per la vaccinazione specificata
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="codiceVaccinazione"></param>
        Public Function GetMaxRichiamo(codicePaziente As Integer, codiceVaccinazione As String) As Integer Implements IDALConvocazioni.GetMaxRichiamo

            Dim maxRichiamo As Integer = 0

            Dim ownConnection As Boolean = False

            Try
                ownConnection = Me.ConditionalOpenConnection()

                Using cmd As New OracleClient.OracleCommand(OnVac.Queries.Convocazioni.OracleQueries.selMaxRichiamo, Me.Connection)

                    If Not _tx Is Nothing Then cmd.Transaction = _tx

                    cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                    cmd.Parameters.AddWithValue("cod_vac", GetStringParam(codiceVaccinazione, False))

                    Dim obj As Object = cmd.ExecuteScalar()
                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        maxRichiamo = Convert.ToInt32(obj)
                    End If

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return maxRichiamo

        End Function

        ''' <summary>
        ''' Restituisce i codici delle vaccinazioni aventi come sostituta la vaccinazione specificata
        ''' </summary>
        ''' <param name="codiceVaccinazioneSostituta"></param>
        Public Function GetVaccinazioniBySostituta(codiceVaccinazioneSostituta As String) As ArrayList Implements IDALConvocazioni.GetVaccinazioniBySostituta

            Dim listVaccinazioni As New ArrayList()

            Dim ownConnection As Boolean = False

            Try
                ownConnection = Me.ConditionalOpenConnection()

                Using cmd As New OracleClient.OracleCommand(Queries.Convocazioni.OracleQueries.selVacBySost, Me.Connection)

                    If Not _tx Is Nothing Then cmd.Transaction = _tx

                    cmd.Parameters.AddWithValue("cod_sost", GetStringParam(codiceVaccinazioneSostituta, False))

                    Using idr As IDataReader = cmd.ExecuteReader()

                        If Not idr Is Nothing Then

                            Dim pos_vac As Integer = idr.GetOrdinal("vac_codice")

                            While idr.Read()
                                listVaccinazioni.Add(idr(pos_vac).ToString())
                            End While

                        End If

                    End Using

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return listVaccinazioni

        End Function

#End Region

#Region " Intervalli Sedute "

        ''' <summary>
        ''' Restituisce un oggetto DatiIntervalliSeduteClass contenente 
        ''' la data dell'ultima vaccinazione effettuata e l'intervallo in giorni alla successiva
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="codiceCiclo"></param>
        ''' <param name="numeroSeduta"></param>
        ''' <remarks>La query utilizzata ordina le effettuate per data discendente, così è sufficiente leggere il primo valore restituito</remarks>
        Public Function GetIntervalloUltimaEffettuata(codicePaziente As Integer, codiceCiclo As String, numeroSeduta As Integer) As ObjectModel.DatiIntervalliSeduteClass Implements IDALConvocazioni.GetIntervalloUltimaEffettuata

            Dim datiIntervalliSedute As ObjectModel.DatiIntervalliSeduteClass = Nothing

            Dim ownConnection As Boolean = False

            Try
                ownConnection = Me.ConditionalOpenConnection()

                Using cmd As New OracleClient.OracleCommand(Queries.Convocazioni.OracleQueries.selIntervalliSedute, Me.Connection)

                    If Not _tx Is Nothing Then cmd.Transaction = _tx

                    cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                    cmd.Parameters.AddWithValue("n_sed", numeroSeduta)
                    cmd.Parameters.AddWithValue("cod_ciclo", GetStringParam(codiceCiclo, False))

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim pos_data As Int16 = idr.GetOrdinal("ves_data_effettuazione")
                            Dim pos_intervallo As Int16 = idr.GetOrdinal("tsd_intervallo_prossima")

                            If idr.Read() Then
                                datiIntervalliSedute = New ObjectModel.DatiIntervalliSeduteClass()
                                datiIntervalliSedute.DataEffettuazione = idr.GetDateTimeOrDefault(pos_data)
                                datiIntervalliSedute.IntervalloProssima = idr.GetDecimal(pos_intervallo)
                            End If

                        End If
                    End Using

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return datiIntervalliSedute

        End Function

#End Region

#Region " Ricerca pazienti da convocare "

        Public Function GetCodiciPazientiDaConvocare(codiceConsultorio As String, statiAnagrafici As String(),
                                                     dataNascitaDa As Date, dataNascitaA As Date,
                                                     tutteCnv As Boolean, sesso As String,
                                                     codiceMalattia As String, codiceCategoriaRischio As String) As ArrayList Implements IDALConvocazioni.GetCodiciPazientiDaConvocare

            Dim listCodiciPazienti As New ArrayList()

            Dim ownConnection As Boolean = False

            Try
                ownConnection = Me.ConditionalOpenConnection()

                Using cmd As New OracleClient.OracleCommand()

                    cmd.Connection = Me.Connection

                    If Not _tx Is Nothing Then cmd.Transaction = _tx

                    ' --- Filtro consultorio --- '
                    cmd.Parameters.AddWithValue("cod_cns", codiceConsultorio)

                    ' --- Filtro pazienti non inadempienti totali --- '
                    ' Passo il parametro alla query in base all'enumerazione degli stati vaccinali e lo passo come stringa.
                    Dim inadempienteTotale As Integer = Enumerators.StatiVaccinali.InadempienteTotale
                    cmd.Parameters.AddWithValue("inadempiente_tot", inadempienteTotale.ToString())

                    ' --- Filtro stati anagrafici selezionati --- '
                    Dim stbStatiAnag As New System.Text.StringBuilder()

                    If Not statiAnagrafici Is Nothing AndAlso statiAnagrafici.Length > 0 Then
                        Dim nome_param As String
                        stbStatiAnag.Append(" and paz_stato_anagrafico in (")
                        For i As Integer = 0 To statiAnagrafici.Length - 1
                            nome_param = String.Format("p{0}", i.ToString())
                            stbStatiAnag.AppendFormat(":{0},", nome_param)
                            cmd.Parameters.AddWithValue(nome_param, statiAnagrafici(i))
                        Next
                        stbStatiAnag.Remove(stbStatiAnag.Length - 1, 1)
                        stbStatiAnag.AppendFormat(") {0}", vbNewLine)
                    End If

                    ' --- Filtro per sottoquery convocazioni --- '
                    Dim stbSubQueryCnv As New System.Text.StringBuilder()

                    If Not tutteCnv Then
                        stbSubQueryCnv.AppendFormat("and not exists ({0}", vbNewLine)
                        stbSubQueryCnv.AppendFormat("select 1 {0}", vbNewLine)
                        stbSubQueryCnv.AppendFormat("from t_cnv_convocazioni join t_vac_programmate {0}", vbNewLine)
                        stbSubQueryCnv.AppendFormat("on cnv_paz_codice = vpr_paz_codice {0}", vbNewLine)
                        stbSubQueryCnv.AppendFormat("and cnv_data = vpr_cnv_data {0}", vbNewLine)
                        stbSubQueryCnv.AppendFormat("where cnv_paz_codice = paz_codice) {0}", vbNewLine)
                    End If

                    Dim stbFiltri As New System.Text.StringBuilder()

                    ' --- Filtro data di nascita --- '
                    If dataNascitaDa > Date.MinValue And dataNascitaA > Date.MinValue Then
                        stbFiltri.AppendFormat("and paz_data_nascita >= :da_data_nascita {0}", vbNewLine)
                        stbFiltri.AppendFormat("and paz_data_nascita <= :a_data_nascita {0}", vbNewLine)
                        cmd.Parameters.AddWithValue("da_data_nascita", dataNascitaDa)
                        cmd.Parameters.AddWithValue("a_data_nascita", dataNascitaA)
                    End If

                    ' --- Filtro sesso --- '
                    If sesso <> String.Empty Then
                        stbFiltri.AppendFormat("and paz_sesso = :sesso {0}", vbNewLine)
                        cmd.Parameters.AddWithValue("sesso", sesso)
                    End If

                    ' --- Filtro malattia --- '
                    If codiceMalattia <> String.Empty Then
                        stbFiltri.AppendFormat("and pma_mal_codice = :cod_malattia {0}", vbNewLine)
                        cmd.Parameters.AddWithValue("cod_malattia", codiceMalattia)
                    End If

                    ' --- Filtro categoria rischio --- '
                    If codiceCategoriaRischio <> String.Empty Then
                        stbFiltri.AppendFormat("and paz_rsc_codice = :cod_rischio {0}", vbNewLine)
                        cmd.Parameters.AddWithValue("cod_rischio", codiceCategoriaRischio)
                    End If

                    ' --- Query finale --- '
                    cmd.CommandText = String.Format(Queries.Convocazioni.OracleQueries.selCodiciPazientiDaConvocare,
                                                    stbStatiAnag.ToString(), stbSubQueryCnv.ToString(), stbFiltri.ToString())

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim pos_cod As Int16 = idr.GetOrdinal("paz_codice")

                            While idr.Read()
                                listCodiciPazienti.Add(idr.GetDecimal(pos_cod))
                            End While

                        End If
                    End Using

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return listCodiciPazienti

        End Function

#End Region

#Region " Convocazioni "

        ''' <summary>
        ''' Restituisce un'arraylist contenente le date di convocazione del paziente.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        Public Function GetDateConvocazioniPaziente(codicePaziente As Integer) As ArrayList Implements IDALConvocazioni.GetDateConvocazioniPaziente

            Return Me.GetDateConvocazione(codicePaziente, False)

        End Function

        ''' <summary>Restituisce un'arraylist contenente le date di convocazione per le obbligatorie del paziente.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        Public Function GetDateConvocazioneVaccinazioniObbligatorie(codicePaziente As Integer) As ArrayList Implements IDALConvocazioni.GetDateConvocazioneVaccinazioniObbligatorie

            Return Me.GetDateConvocazione(codicePaziente, True)

        End Function

        Private Function GetDateConvocazione(codicePaziente As Integer, soloVaccinazioniObligatorie As Boolean) As ArrayList

            Dim listDateConvocazione As New ArrayList()

            Dim strQuery As String = String.Empty

            If soloVaccinazioniObligatorie Then
                strQuery = Onit.OnAssistnet.OnVac.Queries.Convocazioni.OracleQueries.selDateCnvVacObbl
            Else
                strQuery = Onit.OnAssistnet.OnVac.Queries.Convocazioni.OracleQueries.selDateCnvPaz
            End If

            Dim ownConnection As Boolean = False

            Try
                ownConnection = Me.ConditionalOpenConnection()

                Using cmd As New OracleClient.OracleCommand(strQuery, Me.Connection)

                    If Not _tx Is Nothing Then cmd.Transaction = _tx

                    cmd.Parameters.AddWithValue("cod_paz", codicePaziente)

                    Using idr As IDataReader = cmd.ExecuteReader()
                        If Not idr Is Nothing Then

                            Dim pos_data As Int16 = idr.GetOrdinal("vpr_cnv_data")

                            While idr.Read()
                                listDateConvocazione.Add(idr.GetDateTime(pos_data))    ' data cnv sempre valorizzata
                            End While

                        End If
                    End Using

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return listDateConvocazione

        End Function

        ''' <summary>
        ''' Restituisce una struttura contenente tutti i dati per il calcolo delle convocazioni da programmare al paziente.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        Public Function GetConvocazioniDaProgrammare(codicePaziente As Integer) As System.Data.DataTable Implements IDALConvocazioni.GetConvocazioniDaProgrammare

            Return GetConvocazioniPaziente(codicePaziente, True)

        End Function

        ''' <summary>
        ''' Restituisce una struttura contenente tutti i dati delle convocazioni programmate al paziente.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        Public Function GetConvocazioniProgrammate(codicePaziente As Integer) As DataTable Implements IDALConvocazioni.GetConvocazioniProgrammate

            Return GetConvocazioniPaziente(codicePaziente, False)

        End Function

        Private Function GetConvocazioniPaziente(codicePaziente As Integer, daProgrammare As Boolean) As DataTable

            Dim dt As New DataTable()

            Dim strQuery As String = String.Empty

            If daProgrammare Then
                ' Query con i dati delle convocazioni da programmare (comprese le vaccinazioni)
                strQuery = Onit.OnAssistnet.OnVac.Queries.Convocazioni.OracleQueries.selCnvDaProgrammare
            Else
                ' Query con i dati delle convocazioni programmate (comprese le vaccinazioni)
                strQuery = Onit.OnAssistnet.OnVac.Queries.Convocazioni.OracleQueries.selCnvProgrammate
            End If

            Dim ownConnection As Boolean = False

            Try
                ownConnection = Me.ConditionalOpenConnection()

                Using cmd As New OracleClient.OracleCommand(strQuery, Me.Connection)

                    If Not _tx Is Nothing Then cmd.Transaction = _tx

                    cmd.Parameters.AddWithValue("cod_paz", codicePaziente)

                    Using adp As New OracleClient.OracleDataAdapter(cmd)
                        adp.Fill(dt)
                    End Using

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return dt

        End Function

        ''' <summary>
        ''' Modifica di una convocazione. Restituisce true se ha eseguito la modifica, false altrimenti.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataConvocazione"></param>
        ''' <param name="durata"></param>
        Public Function UpdateConvocazione(codicePaziente As Integer, dataConvocazione As Date, durata As Short) As Boolean Implements IDALConvocazioni.UpdateConvocazione

            Dim upd As Boolean = False

            Dim ownConnection As Boolean = False

            Try
                ownConnection = Me.ConditionalOpenConnection()

                Using cmd As New OracleClient.OracleCommand()

                    cmd.Connection = Me.Connection
                    If Not _tx Is Nothing Then cmd.Transaction = _tx

                    ' Creazione stringa per campi da modificare
                    Dim query As New System.Text.StringBuilder()

                    If durata <> -1 Then
                        query.Append("cnv_durata_appuntamento = :dur,")
                        cmd.Parameters.AddWithValue("dur", durata)
                    End If

                    If query.Length > 0 Then

                        ' Tolgo la virgola finale
                        query.RemoveLast(1)

                        ' Assegno la query al command
                        cmd.CommandText = String.Format(Queries.Convocazioni.OracleQueries.updConvocazione, query.ToString())

                        cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                        cmd.Parameters.AddWithValue("data_cnv", dataConvocazione)

                        Dim num As Int16 = cmd.ExecuteNonQuery()
                        upd = (num > 0)

                    End If

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return upd

        End Function

#End Region

#Region " Unione Convocazioni "

        ''' <summary>
        ''' Inserimento della convocazione specificata. Restituisce true se ha eseguito l'inserimento, false altrimenti.
        ''' </summary>
        ''' <param name="convocazione"></param>
        Public Function InsertConvocazione(convocazione As Entities.Convocazione) As Boolean Implements IDALConvocazioni.InsertConvocazione

            Dim count As Integer = 0

            Dim ownConnection As Boolean = False

            Try
                ownConnection = Me.ConditionalOpenConnection()

                Using cmd As New OracleClient.OracleCommand(Queries.Convocazioni.OracleQueries.insConvocazioneSpostata, Me.Connection)

                    If Not _tx Is Nothing Then cmd.Transaction = _tx

                    cmd.Parameters.AddWithValue("cod_paz", convocazione.Paz_codice)
                    cmd.Parameters.AddWithValue("cnv_data", convocazione.Data_CNV)
                    cmd.Parameters.AddWithValue("eta_pom", GetStringParam(convocazione.EtaPomeriggio, False))
                    cmd.Parameters.AddWithValue("rinvio", GetStringParam(convocazione.Rinvio, False))
                    cmd.Parameters.AddWithValue("data_app", GetDateParam(convocazione.DataAppuntamento))
                    cmd.Parameters.AddWithValue("tipo_app", GetStringParam(convocazione.TipoAppuntamento, False))
                    cmd.Parameters.AddWithValue("durata", convocazione.Durata_Appuntamento)
                    cmd.Parameters.AddWithValue("data_invio", GetDateParam(convocazione.DataInvio))
                    cmd.Parameters.AddWithValue("cod_cns", GetStringParam(convocazione.Cns_Codice, False))
                    cmd.Parameters.AddWithValue("id_ute", GetIntParam(convocazione.IdUtente))
                    cmd.Parameters.AddWithValue("data_primo_app", GetDateParam(convocazione.DataPrimoAppuntamento))
                    cmd.Parameters.AddWithValue("cod_amb", GetIntParam(convocazione.CodiceAmbulatorio))
                    cmd.Parameters.AddWithValue("campagna", GetStringParam(convocazione.CampagnaVaccinale, True))

                    If convocazione.DataInserimento.HasValue Then
                        cmd.Parameters.AddWithValue("data_ins", convocazione.DataInserimento.Value)
                    Else
                        cmd.Parameters.AddWithValue("data_ins", DBNull.Value)
                    End If

                    If convocazione.IdUtenteInserimento.HasValue Then
                        cmd.Parameters.AddWithValue("ute_ins", convocazione.IdUtenteInserimento.Value)
                    Else
                        cmd.Parameters.AddWithValue("ute_ins", DBNull.Value)
                    End If

                    count = cmd.ExecuteNonQuery()

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return (count > 0)

        End Function

        ''' <summary>
        ''' Modifica della convocazione specificata. Restituisce true se ha eseguito la modifica, false altrimenti.
        ''' </summary>
        ''' <param name="convocazione"></param>
        Public Function UpdateConvocazione(convocazione As Entities.Convocazione) As Boolean Implements IDALConvocazioni.UpdateConvocazione

            Dim count As Int16

            Dim ownConnection As Boolean = False

            Try
                ownConnection = Me.ConditionalOpenConnection()

                Using cmd As New OracleClient.OracleCommand(Queries.Convocazioni.OracleQueries.updConvocazioneSpostata, Me.Connection)

                    If Not _tx Is Nothing Then cmd.Transaction = _tx

                    cmd.Parameters.AddWithValue("eta_pom", GetStringParam(convocazione.EtaPomeriggio, False))
                    cmd.Parameters.AddWithValue("rinvio", GetStringParam(convocazione.Rinvio, False))
                    cmd.Parameters.AddWithValue("data_app", GetDateParam(convocazione.DataAppuntamento))
                    cmd.Parameters.AddWithValue("tipo_app", GetStringParam(convocazione.TipoAppuntamento, False))
                    cmd.Parameters.AddWithValue("durata", convocazione.Durata_Appuntamento)
                    cmd.Parameters.AddWithValue("data_invio", GetDateParam(convocazione.DataInvio))
                    cmd.Parameters.AddWithValue("cod_cns", GetStringParam(convocazione.Cns_Codice, False))
                    cmd.Parameters.AddWithValue("id_ute", GetIntParam(convocazione.IdUtente))
                    cmd.Parameters.AddWithValue("data_primo_app", GetDateParam(convocazione.DataPrimoAppuntamento))
                    cmd.Parameters.AddWithValue("cod_amb", GetIntParam(convocazione.CodiceAmbulatorio))
                    cmd.Parameters.AddWithValue("campagna", GetStringParam(convocazione.CampagnaVaccinale, True))
                    cmd.Parameters.AddWithValue("cod_paz", convocazione.Paz_codice)
                    cmd.Parameters.AddWithValue("data_cnv", convocazione.Data_CNV)

                    count = cmd.ExecuteNonQuery()

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return (count > 0)

        End Function

#End Region

#Region " Azzeramento Campi Convocazione "

        ''' <summary>
        ''' Imposta al valore specificato la data di appuntamento relativa alla convocazione.
        ''' Se il flag azzera_data_invio è true, imposta anche la data di invio. 
        ''' Restituisce il numero di update eseguiti (0 o 1).
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataConvocazione"></param>
        ''' <param name="valoreData"></param>
        ''' <param name="azzeraDataInvio"></param>
        ''' <returns></returns>
        Public Function UpdateDateAppuntamentoInvio(codicePaziente As Integer, dataConvocazione As Date, valoreData As DateTime, azzeraDataInvio As Boolean) As Integer Implements IDALConvocazioni.UpdateDateAppuntamentoInvio

            Dim count As Integer = 0

            Dim strQuery As String = String.Empty

            Dim ownConnection As Boolean = False

            Try
                ownConnection = Me.ConditionalOpenConnection()

                Using cmd As New OracleClient.OracleCommand()

                    cmd.Parameters.AddWithValue("data_app", GetDateParam(valoreData))

                    If azzeraDataInvio Then
                        strQuery = Queries.Convocazioni.OracleQueries.updDataAppuntamentoInvioCnv
                        cmd.Parameters.AddWithValue("data_invio", GetDateParam(valoreData))
                    Else
                        strQuery = Queries.Convocazioni.OracleQueries.updDataAppuntamentoCnv
                    End If

                    cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                    cmd.Parameters.AddWithValue("data_cnv", dataConvocazione)

                    cmd.CommandText = strQuery

                    cmd.Connection = Me.Connection
                    If Not _tx Is Nothing Then cmd.Transaction = _tx

                    count = cmd.ExecuteNonQuery()

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return count

        End Function

#End Region

#Region " Vaccinazioni Programmate "

        ''' <summary>
        ''' Esegue l'inserimento di una vaccinazione programmata per il paziente, se non è già presente.
        ''' Restituisce il numero di record inseriti.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataConvocazione"></param>
        ''' <param name="codiceVaccinazione"></param>
        ''' <param name="codiceCiclo"></param>
        ''' <param name="numeroSeduta"></param>
        ''' <param name="numeroRichiamo"></param>
        ''' <param name="codiceAssociazione"></param>
        ''' <param name="dataInserimento"></param>
        ''' <param name="idUtenteInserimento"></param>
        Public Function InsertVaccinazioneProgrammata(codicePaziente As Integer, dataConvocazione As Date, codiceVaccinazione As String,
                                                      codiceCiclo As String, numeroSeduta As Integer, numeroRichiamo As Integer, codiceAssociazione As String,
                                                      dataInserimento As DateTime, idUtenteInserimento As Long) As Integer Implements IDALConvocazioni.InsertVaccinazioneProgrammata
            Dim num As Integer = 0

            Dim ownConnection As Boolean = False

            Try
                ownConnection = Me.ConditionalOpenConnection()

                Using cmd As New OracleClient.OracleCommand(Queries.Convocazioni.OracleQueries.insVacProg, Me.Connection)

                    If Not _tx Is Nothing Then cmd.Transaction = _tx

                    cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                    cmd.Parameters.AddWithValue("data_cnv", dataConvocazione)
                    cmd.Parameters.AddWithValue("cod_vac", GetStringParam(codiceVaccinazione, False))
                    cmd.Parameters.AddWithValue("n_richiamo", numeroRichiamo)
                    cmd.Parameters.AddWithValue("cod_cic", GetStringParam(codiceCiclo, False))
                    cmd.Parameters.AddWithValue("n_seduta", GetIntParam(numeroSeduta))
                    cmd.Parameters.AddWithValue("cod_ass", GetStringParam(codiceAssociazione, False))
                    cmd.Parameters.AddWithValue("data_ins", dataInserimento)
                    cmd.Parameters.AddWithValue("ute_ins", idUtenteInserimento)

                    num = cmd.ExecuteNonQuery()

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return num

        End Function

        ''' <summary>
        ''' Restituisce il numero di vaccinazioni programmate del paziente.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        Public Function CountVaccinazioniProgrammatePaz(codicePaziente As Integer) As Integer Implements IDALConvocazioni.CountVaccinazioniProgrammatePaz

            Dim count As Integer = 0

            Dim ownConnection As Boolean = False

            Try
                ownConnection = Me.ConditionalOpenConnection()

                Using cmd As New OracleClient.OracleCommand(Queries.Convocazioni.OracleQueries.countVacProg, Me.Connection)

                    If Not _tx Is Nothing Then cmd.Transaction = _tx

                    cmd.Parameters.AddWithValue("cod_paz", codicePaziente)

                    Dim obj As Object = cmd.ExecuteScalar()
                    If Not obj Is Nothing AndAlso Not obj Is DBNull.Value Then
                        Try
                            count = Convert.ToInt32(obj)
                        Catch
                            count = 0
                        End Try
                    End If

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return count

        End Function

#End Region

#Region " Cicli "

        ''' <summary>
        ''' Inserimento del ciclo specificato per il paziente, se il ciclo non è già presente.
        ''' Restituisce il numero di record inseriti.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        ''' <param name="dataConvocazione"></param>
        ''' <param name="codiceCiclo"></param>
        ''' <param name="numeroSeduta"></param>
        ''' <param name="dataInserimento"></param>
        ''' <param name="idUtenteInserimento"></param>
        Public Function InsertCicloPaziente(codicePaziente As Integer, dataConvocazione As Date, codiceCiclo As String, numeroSeduta As Integer, dataInserimento As DateTime, idUtenteInserimento As Long) As Integer Implements IDALConvocazioni.InsertCicloPaziente

            Dim num As Integer = 0

            Dim ownConnection As Boolean = False

            Try
                ownConnection = Me.ConditionalOpenConnection()

                Using cmd As New OracleClient.OracleCommand(Queries.Convocazioni.OracleQueries.insCicloPaz, Me.Connection)

                    If Not _tx Is Nothing Then cmd.Transaction = _tx

                    cmd.Parameters.AddWithValue("cod_paz", codicePaziente)
                    cmd.Parameters.AddWithValue("data_cnv", dataConvocazione)
                    cmd.Parameters.AddWithValue("cod_cic", GetStringParam(codiceCiclo, False))
                    cmd.Parameters.AddWithValue("n_seduta", numeroSeduta)
                    cmd.Parameters.AddWithValue("data_ins", dataInserimento)
                    cmd.Parameters.AddWithValue("ute_ins", idUtenteInserimento)

                    num = cmd.ExecuteNonQuery()

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return num

        End Function

#End Region

#Region " Date Sospensione "

        ''' <summary>
        ''' Restituisce la massima data di sospensione per il paziente, oppure Date.Minvalue se il paziente non ha sospensioni.
        ''' </summary>
        ''' <param name="codicePaziente"></param>
        Public Function GetMaxDataFineSospensionePaziente(codicePaziente As Integer) As Date Implements IDALConvocazioni.GetMaxDataFineSospensionePaziente

            Dim dataSospensione As Date = Date.Now

            Dim ownConnection As Boolean = False

            Try
                ownConnection = Me.ConditionalOpenConnection()

                Using cmd As New OracleClient.OracleCommand(Queries.Convocazioni.OracleQueries.selMaxFineSospPaz, Me.Connection)

                    If Not _tx Is Nothing Then cmd.Transaction = _tx

                    cmd.Parameters.AddWithValue("cod_paz", codicePaziente)

                    Dim obj As Object = cmd.ExecuteScalar()
                    If obj Is Nothing OrElse obj Is DBNull.Value Then
                        dataSospensione = Date.MinValue
                    Else
                        dataSospensione = CDate(obj)
                    End If

                End Using

            Finally
                Me.ConditionalCloseConnection(ownConnection)
            End Try

            Return dataSospensione

        End Function

#End Region

#End Region

    End Class

End Namespace
