Imports System.Data


Namespace Dal.Oracle


    Public Class OracleDALRicezioneMessaggiEsterni
        Implements IDALRicezioneMessaggiEsterni



#Region " --- Property --- "

        Private _external_conn As Boolean

        Private _provider As String
        Public ReadOnly Property Provider() As String Implements IDALRicezioneMessaggiEsterni.Provider
            Get
                Return _provider
            End Get
        End Property


        Private _conn As IDbConnection
        Public ReadOnly Property Connection() As System.Data.IDbConnection Implements IDALRicezioneMessaggiEsterni.Connection
            Get
                Return _conn
            End Get
        End Property


        Private _tx As IDbTransaction
        Public ReadOnly Property Transaction() As System.Data.IDbTransaction Implements IDALRicezioneMessaggiEsterni.Transaction
            Get
                Return _tx
            End Get
        End Property

#End Region


#Region " --- Disposing --- "

        Public Sub Dispose() Implements System.IDisposable.Dispose
            If Not _tx Is Nothing Then _tx.Dispose()
            If Not _conn Is Nothing AndAlso Not _conn.State = ConnectionState.Closed Then _conn.Close()
        End Sub

#End Region


#Region "  --- Costruttori --- "

        Public Sub New(ByVal provider As String, ByVal connection_string As String)
            _provider = provider
            _conn = _getConnection(connection_string)
            _external_conn = False
        End Sub


        Public Sub New(ByVal provider As String, ByRef conn As IDbConnection, ByRef tx As IDbTransaction)
            _provider = provider
            _conn = conn
            _tx = tx
            _external_conn = True
        End Sub

#End Region


#Region " --- Membri privati --- "

        Private Function _getConnection(ByVal connection_string As String) As OracleClient.OracleConnection

            ' Se non ho la stringa di connessione, sollevo un'eccezione
            If connection_string = String.Empty Then Throw New Exception("DALRicezioneMessaggiEsterni: Impossibile creare la connessione")

            ' --- Creazione della connessione --- '
            Dim _oraConn As OracleClient.OracleConnection
            _oraConn = New OracleClient.OracleConnection(connection_string)

            Return _oraConn
        End Function


        ' Apre la connessione, se è chiusa
        Private Sub _conditionalOpenConnection()
            If _conn.State = ConnectionState.Closed Then _conn.Open()
        End Sub


        ' Restituisce dbnull se la stringa passata per parametro è vuota. Altrimenti restituisce la stringa stessa.
        ' Utilizzato per il passaggio dei parametri all'oracleCommand senza dover effettuare tutte le volte il controllo esplicito di ogni parametro
        Protected Friend Shared Function _getStringParam(ByVal string_value As String, ByVal to_uppercase As Boolean) As Object
            If string_value = String.Empty Then Return DBNull.Value
            If to_uppercase Then Return string_value.ToUpper()
            Return string_value
        End Function


        ' Restituisce dbnull se la data passata per parametro è minvalue. Altrimenti restituisce la data stessa.
        ' Utilizzato per il passaggio dei parametri all'oracleCommand senza dover effettuare tutte le volte il controllo esplicito di ogni parametro
        Protected Friend Shared Function _getDateParam(ByVal date_value As DateTime) As Object
            If date_value = Date.MinValue Then Return DBNull.Value
            Return date_value
        End Function


        ' Restituisce dbnull se il numero passato per parametro è -1. Altrimenti restituisce il numero stesso.
        ' Utilizzato per il passaggio dei parametri all'oracleCommand senza dover effettuare tutte le volte il controllo esplicito di ogni parametro
        Protected Friend Shared Function _getIntParam(ByVal int_value As Integer) As Object
            If int_value = -1 Then Return DBNull.Value
            Return int_value
        End Function

#End Region



        Public Sub BeginTransaction() Implements IDALRicezioneMessaggiEsterni.BeginTransaction
            _conditionalOpenConnection()
            _tx = _conn.BeginTransaction
        End Sub


#Region " Ricerca pazienti "

        Public Function GetPazienteByCodiceLocale(ByVal paz_codice As Integer) As Entities.Paziente Implements IDALRicezioneMessaggiEsterni.GetPazienteByCodiceLocale
            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim dr As IDataReader
            Dim p As Entities.Paziente = Nothing

            Dim strQuery As String = "select paz_codice, paz_cognome, paz_nome, paz_data_nascita from t_paz_pazienti where paz_codice = :cod_paz"

            _conditionalOpenConnection()

            Try
                cmd = New OracleClient.OracleCommand(strQuery, _conn)
                If Not _tx Is Nothing Then cmd.Transaction = _tx
                cmd.Parameters.AddWithValue("cod_paz", paz_codice)

                dr = cmd.ExecuteReader
                If Not dr Is Nothing Then
                    Dim pos_paz_cod, pos_paz_cognome, pos_paz_nome, pos_paz_data_nascita As Integer
                    pos_paz_cod = dr.GetOrdinal("paz_codice")
                    pos_paz_cognome = dr.GetOrdinal("paz_cognome")
                    pos_paz_nome = dr.GetOrdinal("paz_nome")
                    pos_paz_data_nascita = dr.GetOrdinal("paz_data_nascita")

                    If dr.Read Then
                        p = New Entities.Paziente(dr.GetDecimal(pos_paz_cod))

                        p.PAZ_COGNOME = dr(pos_paz_cognome).ToString
                        p.PAZ_NOME = dr(pos_paz_nome).ToString
                        If dr.IsDBNull(pos_paz_data_nascita) Then
                            p.Data_Nascita = Date.MinValue
                        Else
                            p.Data_Nascita = dr.GetDateTime(pos_paz_data_nascita)
                        End If
                    End If

                End If

            Finally
                If Not cmd Is Nothing Then cmd.Dispose()
            End Try

            Return p
        End Function


        Public Function GetPazientiByCodiceAusiliario(ByVal paz_codice_ausiliario As String) As Collection.PazienteCollection Implements IDALRicezioneMessaggiEsterni.GetPazientiByCodiceAusiliario
            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim dr As IDataReader

            Dim coll_paz As New Collection.PazienteCollection
            Dim p As Entities.Paziente

            Dim strQuery As String = "select paz_codice, paz_cognome, paz_nome, paz_data_nascita from t_paz_pazienti where paz_codice_ausiliario = :cod_ausiliario"

            _conditionalOpenConnection()

            Try
                cmd = New OracleClient.OracleCommand(strQuery, _conn)
                If Not _tx Is Nothing Then cmd.Transaction = _tx
                cmd.Parameters.AddWithValue("cod_ausiliario", _getStringParam(paz_codice_ausiliario, False))

                dr = cmd.ExecuteReader
                If Not dr Is Nothing Then
                    Dim pos_paz_cod, pos_paz_cognome, pos_paz_nome, pos_paz_data_nascita As Integer
                    pos_paz_cod = dr.GetOrdinal("paz_codice")
                    pos_paz_cognome = dr.GetOrdinal("paz_cognome")
                    pos_paz_nome = dr.GetOrdinal("paz_nome")
                    pos_paz_data_nascita = dr.GetOrdinal("paz_data_nascita")

                    While dr.Read
                        p = New Entities.Paziente(dr.GetDecimal(pos_paz_cod))

                        p.PAZ_COGNOME = dr(pos_paz_cognome).ToString
                        p.PAZ_NOME = dr(pos_paz_nome).ToString
                        If dr.IsDBNull(pos_paz_data_nascita) Then
                            p.Data_Nascita = Date.MinValue
                        Else
                            p.Data_Nascita = dr.GetDateTime(pos_paz_data_nascita)
                        End If

                        coll_paz.Add(p)
                    End While

                End If

            Finally
                If Not cmd Is Nothing Then cmd.Dispose()
            End Try

            Return coll_paz
        End Function


        Public Function GetCodicePazByCodiceAusiliario(ByVal paz_codice_ausiliario As String) As Integer Implements IDALRicezioneMessaggiEsterni.GetCodicePazByCodiceAusiliario
            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim cod As Integer = -1

            Dim strQuery As String = "select paz_codice from t_paz_pazienti where paz_codice_ausiliario = :cod_ausiliario"

            ' Se la connessione è chiusa la apre. Se era già aperta, non fa niente.
            _conditionalOpenConnection()

            Try
                cmd = New OracleClient.OracleCommand(strQuery, _conn)
                If Not _tx Is Nothing Then cmd.Transaction = _tx
                cmd.Parameters.AddWithValue("cod_ausiliario", _getStringParam(paz_codice_ausiliario, False))

                Dim obj As Object = cmd.ExecuteScalar()
                If (obj Is Nothing OrElse obj Is DBNull.Value) Then
                    cod = -1
                Else
                    cod = Convert.ToInt32(obj)
                End If

            Finally
                If Not cmd Is Nothing Then cmd.Dispose()
            End Try

            Return cod
        End Function

#End Region


        ''' <summary>
        ''' Restituisce il codice dell'usl in base al comune specificato.
        ''' </summary>
        Public Function GetCodiceAslByCodiceComune(ByVal cod_comune As String) As String Implements IDALRicezioneMessaggiEsterni.GetCodiceAslByCodiceComune
            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim _cod_usl As String = String.Empty

            Dim strQuery As String = "select lcu_usl_codice from t_ana_link_comuni_usl where lcu_com_codice = :cod_comune"

            _conditionalOpenConnection()

            Try
                cmd = New OracleClient.OracleCommand(strQuery, _conn)
                If Not _tx Is Nothing Then cmd.Transaction = _tx

                cmd.Parameters.AddWithValue("cod_comune", _getStringParam(cod_comune, False))

                Dim obj As Object = cmd.ExecuteScalar

                If obj Is Nothing OrElse obj Is DBNull.Value Then
                    _cod_usl = String.Empty
                Else
                    _cod_usl = obj.ToString
                End If

            Finally
                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return _cod_usl
        End Function



#Region " --- Insert --- "

        ' Inserimento paziente e restituzione codice del paziente inserito
        Public Function InsertPaziente(ByVal msg As Onit.OnAssistnet.Contracts.Messaggio, ByVal paz_regolarizzato As String, ByVal cod_cns_vacc As String, ByVal data_cns_vacc As Date, ByVal cod_cns_terr As String) As Integer Implements IDALRicezioneMessaggiEsterni.InsertPaziente
            Dim cod_paz As Integer = -1

            Dim cmd As OracleClient.OracleCommand = Nothing

            _conditionalOpenConnection()

            Try
                cmd = New OracleClient.OracleCommand
                cmd.Connection = _conn
                If Not _tx Is Nothing Then cmd.Transaction = _tx

                ' Calcolo del codice del paziente che verrà inserito
                cmd.CommandText = "select seq_pazienti.nextval from dual"
                cod_paz = cmd.ExecuteScalar()

                ' Costruzione query di insert
                Dim strQuery As String = "insert into t_paz_pazienti ({0}) values ({1})"
                cmd.CommandText = String.Empty
                cmd.Parameters.Clear()

                Dim stbCampi As New System.Text.StringBuilder
                Dim stbValori As New System.Text.StringBuilder

                Dim dati_paz As Onit.OnAssistnet.Contracts.Paziente = msg.Pazienti(0)

                ' Aggiunta istruzione nella query e parametro nel command
                AddIntegerInsertCondition("paz_codice", cod_paz, stbCampi, stbValori, cmd)
                AddStringInsertCondition("paz_cit_codice", dati_paz.DatiAnagrafici.CittadinanzaCodice, stbCampi, stbValori, cmd)
                AddStringInsertCondition("paz_cognome", dati_paz.DatiAnagrafici.Cognome, stbCampi, stbValori, cmd)
                AddStringInsertCondition("paz_nome", dati_paz.DatiAnagrafici.Nome, stbCampi, stbValori, cmd)
                AddStringInsertCondition("paz_sesso", dati_paz.DatiAnagrafici.Sesso, stbCampi, stbValori, cmd)
                AddStringInsertCondition("paz_telefono_1", dati_paz.DatiAnagrafici.Telefono1, stbCampi, stbValori, cmd)
                AddStringInsertCondition("paz_telefono_2", dati_paz.DatiAnagrafici.Telefono2, stbCampi, stbValori, cmd)

                ' Non gestito
                'AddStringInsertCondition("paz_codice_regionale", dati_paz.DatiAnagrafici.CodiceRegione, stbCampi, stbValori, cmd)

                AddDateInsertCondition("paz_data_nascita", dati_paz.DatiAnagrafici.DataNascita, stbCampi, stbValori, cmd)
                If (dati_paz.DatiAnagrafici.DataDecessoSpecified) Then
                    AddDateInsertCondition("paz_data_decesso", dati_paz.DatiAnagrafici.DataDecesso, stbCampi, stbValori, cmd)
                End If
                If (dati_paz.DatiAnagrafici.DataImmigrazioneSpecified) Then
                    AddDateInsertCondition("paz_data_immigrazione", dati_paz.DatiAnagrafici.DataImmigrazione, stbCampi, stbValori, cmd)
                End If
                AddDateInsertCondition("paz_data_inserimento", Date.Now.Date, stbCampi, stbValori, cmd)

                AddStringInsertCondition("paz_codice_ausiliario", dati_paz.DatiAnagrafici.Codice, stbCampi, stbValori, cmd)

                Dim _id As Onit.OnAssistnet.Contracts.ID

                Dim i As Int16
                For i = 0 To dati_paz.DatiAnagrafici.IDArray.Length - 1
                    _id = dati_paz.DatiAnagrafici.IDArray(i)

                    Select Case (_id.TipoCodificatoID)
                        Case Onit.OnAssistnet.Contracts.TipoCodificatoID.Fiscale
                            ' Codice fiscale
                            AddStringInsertCondition("paz_codice_fiscale", _id.IDValue, stbCampi, stbValori, cmd)
                        Case Onit.OnAssistnet.Contracts.TipoCodificatoID.Sanitario
                            ' Tessera sanitaria
                            AddStringInsertCondition("paz_tessera", _id.IDValue, stbCampi, stbValori, cmd)
                    End Select
                    ' Per quanto riguarda il codice ausiliario, è di tipo Aziendale (Case Onit.OnAssistnet.Contracts.TipoCodificatoID.Aziendale),
                    ' ma bisogna controllare anche che l'assigning authority sia APC.
                    ' Questo controllo è già stato fatto e il valore è già stato impostato nel campo Paziente.DatiAnagrafici.Codice
                Next

                Dim _indir As Onit.OnAssistnet.Contracts.Indirizzo
                For i = 0 To dati_paz.DatiAnagrafici.Indirizzi.Length - 1
                    _indir = dati_paz.DatiAnagrafici.Indirizzi(i)

                    Select Case (_indir.TipoCodificato)
                        Case Onit.OnAssistnet.Contracts.TipoCodificatoIndirizzo.Nee
                            ' Nascita
                            AddStringInsertCondition("paz_com_codice_nascita", _indir.ComuneCodiceXMPI, stbCampi, stbValori, cmd)

                        Case Onit.OnAssistnet.Contracts.TipoCodificatoIndirizzo.Legal
                            ' Residenza
                            AddStringInsertCondition("paz_com_codice_residenza", _indir.ComuneCodiceXMPI, stbCampi, stbValori, cmd)
                            AddStringInsertCondition("paz_indirizzo_residenza", _indir.ViaECivico, stbCampi, stbValori, cmd)
                            AddStringInsertCondition("paz_cap_residenza", _indir.Cap, stbCampi, stbValori, cmd)
                            ' ??? CONTROLLARE DATA
                            'If (_indir.DataFineSpecified) Then
                            '    AddDateInsertCondition("paz_data_emigrazione", _indir.DataFine, stbCampi, stbValori, cmd)
                            'End If

                        Case Onit.OnAssistnet.Contracts.TipoCodificatoIndirizzo.Home
                            ' Domicilio
                            AddStringInsertCondition("paz_com_codice_domicilio", _indir.ComuneCodiceXMPI, stbCampi, stbValori, cmd)
                            AddStringInsertCondition("paz_indirizzo_domicilio", _indir.ViaECivico, stbCampi, stbValori, cmd)
                            AddStringInsertCondition("paz_cap_domicilio", _indir.Cap, stbCampi, stbValori, cmd)
                            ' ??? CONTROLLARE DATA
                            'If (_indir.DataFineSpecified) Then
                            '    AddDateInsertCondition("paz_data_emigrazione", _indir.DataFine, stbCampi, stbValori, cmd)
                            'End If

                        Case Onit.OnAssistnet.Contracts.TipoCodificatoIndirizzo.Current
                            ' Emigrazione
                            AddStringInsertCondition("paz_com_comune_emigrazione", _indir.ComuneCodiceXMPI, stbCampi, stbValori, cmd)
                            ' ??? CONTROLLARE DATA
                            If (_indir.DataInizioSpecified) Then
                                AddDateInsertCondition("paz_data_emigrazione", _indir.DataInizio, stbCampi, stbValori, cmd)
                            End If

                        Case Onit.OnAssistnet.Contracts.TipoCodificatoIndirizzo.Foreign
                            ' Immigrazione
                            AddStringInsertCondition("paz_com_comune_provenienza", _indir.ComuneCodiceXMPI, stbCampi, stbValori, cmd)
                            ' ??? CONTROLLARE DATA
                            If (_indir.DataInizioSpecified) Then
                                If (Not dati_paz.DatiAnagrafici.DataImmigrazioneSpecified) Then
                                    AddDateInsertCondition("paz_data_immigrazione", _indir.DataInizio, stbCampi, stbValori, cmd)
                                End If
                            End If

                    End Select

                Next

                AddStringInsertCondition("paz_usl_codice_residenza", dati_paz.DatiSanitari.USLCodiceResidenza, stbCampi, stbValori, cmd)
                AddStringInsertCondition("paz_usl_codice_assistenza", dati_paz.DatiSanitari.USLCodiceAssistenza, stbCampi, stbValori, cmd)
                AddStringInsertCondition("paz_cir_codice", dati_paz.DatiSanitari.Distretto, stbCampi, stbValori, cmd)

                AddStringInsertCondition("paz_med_codice_base", dati_paz.DatiAnagrafici.CodiceMedicoBase, stbCampi, stbValori, cmd)
                If (dati_paz.DatiSanitari.DataSceltaMedicoSpecified) Then
                    AddDateInsertCondition("paz_data_decorrenza_med", dati_paz.DatiSanitari.DataSceltaMedico, stbCampi, stbValori, cmd)
                End If

                If (dati_paz.DatiSanitari.DataScadenzaMedicoSpecified) Then
                    AddDateInsertCondition("paz_data_scadenza_med", dati_paz.DatiSanitari.DataScadenzaMedico, stbCampi, stbValori, cmd)
                End If


                ' Dati calcolati
                AddStringInsertCondition("paz_stato_anagrafico", dati_paz.DatiAnagrafici.StatoAnagrafico, stbCampi, stbValori, cmd)
                AddStringInsertCondition("paz_regolarizzato", paz_regolarizzato, stbCampi, stbValori, cmd)
                AddStringInsertCondition("paz_cns_codice", cod_cns_vacc, stbCampi, stbValori, cmd)
                AddDateInsertCondition("paz_cns_data_assegnazione", data_cns_vacc, stbCampi, stbValori, cmd)
                AddStringInsertCondition("paz_cns_terr_codice", cod_cns_terr, stbCampi, stbValori, cmd)


                If stbCampi.Length > 0 Then
                    ' Cancellazione ultima virgola (e spazio)
                    stbCampi.Remove(stbCampi.Length - 2, 2)
                    stbValori.Remove(stbValori.Length - 2, 2)

                    ' Query
                    cmd.CommandText = String.Format(strQuery, stbCampi.ToString, stbValori.ToString)
                    cmd.ExecuteNonQuery()
                End If

            Finally
                If Not cmd Is Nothing Then cmd.Dispose()
            End Try


            Return cod_paz
        End Function


        Private Sub AddStringInsertCondition(ByVal campo As String, ByVal valore As String, ByRef stbCampi As System.Text.StringBuilder, ByRef stbValori As System.Text.StringBuilder, ByRef cmd As OracleClient.OracleCommand)
            stbCampi.AppendFormat("{0}, ", campo)
            stbValori.AppendFormat(":{0}, ", campo)

            If (valore Is Nothing) Then valore = String.Empty
            cmd.Parameters.AddWithValue(campo, _getStringParam(valore, False))
        End Sub


        Private Sub AddDateInsertCondition(ByVal campo As String, ByVal valore As Date, ByRef stbCampi As System.Text.StringBuilder, ByRef stbValori As System.Text.StringBuilder, ByRef cmd As OracleClient.OracleCommand)
            stbCampi.AppendFormat("{0}, ", campo)
            stbValori.AppendFormat(":{0}, ", campo)

            If (valore = Nothing) Then valore = Date.MinValue
            cmd.Parameters.AddWithValue(campo, _getDateParam(valore))
        End Sub


        Private Sub AddIntegerInsertCondition(ByVal campo As String, ByVal valore As Integer, ByRef stbCampi As System.Text.StringBuilder, ByRef stbValori As System.Text.StringBuilder, ByRef cmd As OracleClient.OracleCommand)
            stbCampi.AppendFormat("{0}, ", campo)
            stbValori.AppendFormat(":{0}, ", campo)

            If (valore = Nothing) Then valore = -1
            cmd.Parameters.AddWithValue(campo, _getIntParam(valore))
        End Sub

#End Region


#Region " --- Update --- "


        Public Sub UpdateCodiceAusiliario(ByVal paz_codice As Integer, ByVal paz_codice_ausiliario As String) Implements IDALRicezioneMessaggiEsterni.UpdateCodiceAusiliario
            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim strQuery As String = "update t_paz_pazienti set paz_codice_ausiliario = :paz_codice_ausiliario where paz_codice = :paz_codice"

            _conditionalOpenConnection()

            Try
                cmd = New OracleClient.OracleCommand(strQuery, _conn)
                If Not _tx Is Nothing Then cmd.Transaction = _tx

                cmd.Parameters.AddWithValue("paz_codice_ausiliario", _getStringParam(paz_codice_ausiliario, False))
                cmd.Parameters.AddWithValue("paz_codice", paz_codice)

                cmd.ExecuteNonQuery()

            Finally
                If Not cmd Is Nothing Then cmd.Dispose()
            End Try

        End Sub


        Public Function UpdatePaziente(ByVal paz_codice As Integer, ByVal msg As Onit.OnAssistnet.Contracts.Messaggio, ByVal paz_regolarizzato As String, ByVal cod_cns_vacc As String, ByVal cod_cns_vacc_old As String, ByVal data_cns_vacc As Date, ByVal cod_cns_terr As String) As Integer Implements IDALRicezioneMessaggiEsterni.UpdatePaziente
            Dim num As Integer = 0

            Dim cmd As OracleClient.OracleCommand = Nothing

            Dim strQuery As String = "update t_paz_pazienti set {0} where paz_codice = :paz_codice"

            _conditionalOpenConnection()

            Try
                cmd = New OracleClient.OracleCommand(strQuery, _conn)
                If Not _tx Is Nothing Then cmd.Transaction = _tx

                ' Costruzione query di update
                Dim stbQuery As New System.Text.StringBuilder

                Dim dati_paz As Onit.OnAssistnet.Contracts.Paziente = msg.Pazienti(0)

                ' Aggiunta istruzione nella query e parametro nel command
                AddStringUpdateCondition("paz_codice_ausiliario", dati_paz.DatiAnagrafici.Codice, stbQuery, cmd)
                AddStringUpdateCondition("paz_cit_codice", dati_paz.DatiAnagrafici.CittadinanzaCodice, stbQuery, cmd)
                AddStringUpdateCondition("paz_cognome", dati_paz.DatiAnagrafici.Cognome, stbQuery, cmd)
                AddStringUpdateCondition("paz_nome", dati_paz.DatiAnagrafici.Nome, stbQuery, cmd)
                AddStringUpdateCondition("paz_sesso", dati_paz.DatiAnagrafici.Sesso, stbQuery, cmd)
                AddStringUpdateCondition("paz_telefono_1", dati_paz.DatiAnagrafici.Telefono1, stbQuery, cmd)
                AddStringUpdateCondition("paz_telefono_2", dati_paz.DatiAnagrafici.Telefono2, stbQuery, cmd)

                ' Non gestito
                'AddStringUpdateCondition("paz_codice_regionale", dati_paz.DatiAnagrafici.CodiceRegione, stbQuery, cmd)

                AddDateUpdateCondition("paz_data_nascita", dati_paz.DatiAnagrafici.DataNascita, stbQuery, cmd)
                AddDateUpdateCondition("paz_data_decesso", dati_paz.DatiAnagrafici.DataDecesso, stbQuery, cmd)
                AddDateUpdateCondition("paz_data_agg_da_anag", Date.Now.Date, stbQuery, cmd)

                ' Update data immigrazione se è specificata direttamente. 
                ' Metto il controllo, altrimenti si potrebbe duplicare la colonna se è specificato anche l'indirizzo di tipo Foreign.
                If (dati_paz.DatiAnagrafici.DataImmigrazioneSpecified) Then
                    AddDateUpdateCondition("paz_data_immigrazione", dati_paz.DatiAnagrafici.DataImmigrazione, stbQuery, cmd)
                End If


                Dim i As Int16

                Dim _id As Onit.OnAssistnet.Contracts.ID
                For i = 0 To dati_paz.DatiAnagrafici.IDArray.Length - 1
                    _id = dati_paz.DatiAnagrafici.IDArray(i)

                    Select Case (_id.TipoCodificatoID)
                        Case Onit.OnAssistnet.Contracts.TipoCodificatoID.Fiscale
                            ' Codice fiscale
                            AddStringUpdateCondition("paz_codice_fiscale", _id.IDValue, stbQuery, cmd)
                        Case Onit.OnAssistnet.Contracts.TipoCodificatoID.Sanitario
                            ' Tessera sanitaria
                            AddStringUpdateCondition("paz_tessera", _id.IDValue, stbQuery, cmd)
                    End Select
                    ' Per quanto riguarda il codice ausiliario, è di tipo Aziendale (Case Onit.OnAssistnet.Contracts.TipoCodificatoID.Aziendale),
                    ' ma bisogna controllare anche che l'assigning authority sia APC.
                    ' Questo controllo è già stato fatto e il valore è già stato impostato nel campo Paziente.DatiAnagrafici.Codice
                Next

                Dim _indir As Onit.OnAssistnet.Contracts.Indirizzo
                For i = 0 To dati_paz.DatiAnagrafici.Indirizzi.Length - 1
                    _indir = dati_paz.DatiAnagrafici.Indirizzi(i)

                    Select Case (_indir.TipoCodificato)
                        Case Onit.OnAssistnet.Contracts.TipoCodificatoIndirizzo.Nee
                            ' Nascita
                            AddStringUpdateCondition("paz_com_codice_nascita", _indir.ComuneCodiceXMPI, stbQuery, cmd)

                        Case Onit.OnAssistnet.Contracts.TipoCodificatoIndirizzo.Legal
                            ' Residenza
                            AddStringUpdateCondition("paz_com_codice_residenza", _indir.ComuneCodiceXMPI, stbQuery, cmd)
                            AddStringUpdateCondition("paz_indirizzo_residenza", _indir.ViaECivico, stbQuery, cmd)
                            AddStringUpdateCondition("paz_cap_residenza", _indir.Cap, stbQuery, cmd)
                            ' ??? CONTROLLARE DATA
                            'If (_indir.DataFineSpecified) Then
                            '    AddDateUpdateCondition("paz_data_emigrazione", _indir.DataFine, stbQuery, cmd)
                            'End If

                        Case Onit.OnAssistnet.Contracts.TipoCodificatoIndirizzo.Home
                            ' Domicilio
                            AddStringUpdateCondition("paz_com_codice_domicilio", _indir.ComuneCodiceXMPI, stbQuery, cmd)
                            AddStringUpdateCondition("paz_indirizzo_domicilio", _indir.ViaECivico, stbQuery, cmd)
                            AddStringUpdateCondition("paz_cap_domicilio", _indir.Cap, stbQuery, cmd)
                            ' ??? CONTROLLARE DATA
                            'If (_indir.DataFineSpecified) Then
                            '    AddDateUpdateCondition("paz_data_emigrazione", _indir.DataFine, stbQuery, cmd)
                            'End If

                        Case Onit.OnAssistnet.Contracts.TipoCodificatoIndirizzo.Current
                            ' Emigrazione
                            AddStringUpdateCondition("paz_com_comune_emigrazione", _indir.ComuneCodiceXMPI, stbQuery, cmd)
                            ' ??? CONTROLLARE DATA
                            If (_indir.DataInizioSpecified) Then
                                AddDateUpdateCondition("paz_data_emigrazione", _indir.DataInizio, stbQuery, cmd)
                            End If

                        Case Onit.OnAssistnet.Contracts.TipoCodificatoIndirizzo.Foreign
                            ' Immigrazione
                            AddStringUpdateCondition("paz_com_comune_provenienza", _indir.ComuneCodiceXMPI, stbQuery, cmd)
                            ' ??? CONTROLLARE DATA
                            If (_indir.DataInizioSpecified) Then
                                If (Not dati_paz.DatiAnagrafici.DataImmigrazioneSpecified) Then
                                    ' Update data immigrazione in base ai dati di immigrazione, se la data non è specificata direttamente.
                                    ' Metto il controllo, altrimenti si potrebbe duplicare la colonna se è specificata anche la data di immigrazione.
                                    AddDateUpdateCondition("paz_data_immigrazione", _indir.DataInizio, stbQuery, cmd)
                                End If
                            End If

                    End Select

                Next

                AddStringUpdateCondition("paz_usl_codice_residenza", dati_paz.DatiSanitari.USLCodiceResidenza, stbQuery, cmd)
                AddStringUpdateCondition("paz_usl_codice_assistenza", dati_paz.DatiSanitari.USLCodiceAssistenza, stbQuery, cmd)
                AddStringUpdateCondition("paz_cir_codice", dati_paz.DatiSanitari.Distretto, stbQuery, cmd)

                AddStringUpdateCondition("paz_med_codice_base", dati_paz.DatiAnagrafici.CodiceMedicoBase, stbQuery, cmd)
                AddDateUpdateCondition("paz_data_decorrenza_med", dati_paz.DatiSanitari.DataSceltaMedico, stbQuery, cmd)
                AddDateUpdateCondition("paz_data_scadenza_med", dati_paz.DatiSanitari.DataScadenzaMedico, stbQuery, cmd)


                ' Dati calcolati (da modificare solo se valorizzati)

                If (dati_paz.DatiAnagrafici.StatoAnagrafico <> String.Empty) Then
                    AddStringUpdateCondition("paz_stato_anagrafico", dati_paz.DatiAnagrafici.StatoAnagrafico, stbQuery, cmd)
                End If

                If (paz_regolarizzato <> String.Empty) Then
                    AddStringUpdateCondition("paz_regolarizzato", paz_regolarizzato, stbQuery, cmd)
                End If

                If (cod_cns_vacc <> String.Empty) Then
                    AddStringUpdateCondition("paz_cns_codice", cod_cns_vacc, stbQuery, cmd)
                End If

                If (cod_cns_vacc_old <> String.Empty) Then
                    AddStringUpdateCondition("paz_cns_codice_old", cod_cns_vacc_old, stbQuery, cmd)
                End If

                If (data_cns_vacc <> Date.MinValue) Then
                    AddDateUpdateCondition("paz_cns_data_assegnazione", data_cns_vacc, stbQuery, cmd)
                End If

                If (cod_cns_terr <> String.Empty) Then
                    AddStringUpdateCondition("paz_cns_terr_codice", cod_cns_terr, stbQuery, cmd)
                End If


                If stbQuery.Length > 0 Then
                    ' cancellazione ultima virgola (e spazio)
                    stbQuery.Remove(stbQuery.Length - 2, 2)

                    ' Filtro
                    cmd.Parameters.AddWithValue("paz_codice", paz_codice)

                    ' Query 
                    cmd.CommandText = String.Format(strQuery, stbQuery.ToString)
                    num = cmd.ExecuteNonQuery()

                End If

            Finally
                If Not cmd Is Nothing Then cmd.Dispose()
            End Try

            Return num
        End Function


        Private Sub AddStringUpdateCondition(ByVal campo As String, ByVal valore As String, ByRef stbQuery As System.Text.StringBuilder, ByRef cmd As OracleClient.OracleCommand)
            If (valore Is Nothing) Then valore = String.Empty

            stbQuery.AppendFormat("{0} = :{0}, ", campo)

            cmd.Parameters.AddWithValue(campo, _getStringParam(valore, False))
        End Sub


        Private Sub AddDateUpdateCondition(ByVal campo As String, ByVal valore As Date, ByRef stbQuery As System.Text.StringBuilder, ByRef cmd As OracleClient.OracleCommand)
            stbQuery.AppendFormat("{0} = :{0}, ", campo)

            If (valore = Nothing) Then valore = Date.MinValue
            cmd.Parameters.AddWithValue(campo, _getDateParam(valore))
        End Sub


#End Region



        Private Function InserimentoCicliPaziente(ByVal paz_codice As Integer, ByVal data_nascita As Date, ByVal sesso As String) As Integer Implements IDALRicezioneMessaggiEsterni.InserimentoCicliPaziente
            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim num As Integer

            Dim strQuery As String = "insert into t_paz_cicli select :paz_codice, cic_codice, null from t_ana_cicli "
            strQuery += "where cic_standard = 'T' "
            strQuery += "and cic_data_introduzione <= :data_nascita "
            strQuery += "and (cic_data_fine > :data_nascita or cic_data_fine is null) "
            strQuery += "and (cic_sesso = :sesso or cic_sesso = 'E')"

            _conditionalOpenConnection()

            Try
                cmd = New OracleClient.OracleCommand(strQuery, _conn)
                If Not _tx Is Nothing Then cmd.Transaction = _tx

                cmd.Parameters.AddWithValue("paz_codice", paz_codice)
                cmd.Parameters.AddWithValue("data_nascita", data_nascita)
                cmd.Parameters.AddWithValue("sesso", _getStringParam(sesso, False))

                num = cmd.ExecuteNonQuery

            Finally
                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return num
        End Function


        Public Function InserimentoMovimentoPaziente(ByVal paz_codice As Integer, ByVal cns_codice_old As String, ByVal cns_codice_new As String) As Integer Implements IDALRicezioneMessaggiEsterni.InserimentoMovimentoPaziente
            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim num As Integer

            Dim strQuery As String = "insert into t_cns_movimenti "
            strQuery += "(cnm_paz_codice, cnm_cns_codice_old, cnm_cns_codice_new, cnm_data, cnm_invio_cartella, cnm_presa_visione, cnm_auto_adulti) "
            strQuery += "values "
            strQuery += "(:paz_codice, :cns_codice_old, :cns_codice_new, :data_mov, null, 'N', 'N') "


            _conditionalOpenConnection()

            Try
                cmd = New OracleClient.OracleCommand(strQuery, _conn)
                If Not _tx Is Nothing Then cmd.Transaction = _tx

                cmd.Parameters.AddWithValue("paz_codice", paz_codice)
                cmd.Parameters.AddWithValue("cns_codice_old", _getStringParam(cns_codice_old, False))
                cmd.Parameters.AddWithValue("cns_codice_new", _getStringParam(cns_codice_new, False))
                cmd.Parameters.AddWithValue("data_mov", Date.Now.Date)

                num = cmd.ExecuteNonQuery

            Finally
                If Not cmd Is Nothing Then cmd.Dispose()
            End Try

            Return num

        End Function


        Public Function CountCicliIncompatibili(ByVal paz_codice As Integer, ByVal data_nascita As Date, ByVal sesso As String) As Integer Implements IDALRicezioneMessaggiEsterni.CountCicliIncompatibili
            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim cnt As Integer = 0

            Dim strQuery As String = "select count(*) from t_ana_cicli, t_paz_cicli "
            strQuery += "where cic_codice = pac_cic_codice and pac_paz_codice = :paz_codice "
            strQuery += "and (cic_data_introduzione > :data_nascita "
            strQuery += "or (cic_data_fine < :data_nascita and cic_data_fine is not null) "
            If sesso <> String.Empty Then
                strQuery += "or (cic_sesso <> :sesso and cic_sesso <> 'E') "
            End If
            strQuery += ")"


            _conditionalOpenConnection()

            Try
                cmd = New OracleClient.OracleCommand(strQuery, _conn)
                If Not _tx Is Nothing Then cmd.Transaction = _tx

                cmd.Parameters.AddWithValue("paz_codice", paz_codice)
                cmd.Parameters.AddWithValue("data_nascita", data_nascita)
                If sesso <> String.Empty Then
                    cmd.Parameters.AddWithValue("sesso", sesso)
                End If

                Dim obj As Object = cmd.ExecuteScalar
                If obj Is Nothing OrElse obj Is DBNull.Value Then
                    cnt = 0
                Else
                    cnt = System.Convert.ToInt32(obj)
                End If

            Finally
                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

            Return cnt
        End Function


#Region " --- Delete Cnv --- "

        Public Sub DeleteConvocazioni(ByVal paz_codice As Integer, ByVal del_cnv_con_appuntamento As Boolean) Implements IDALRicezioneMessaggiEsterni.DeleteConvocazioni
            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim strQuery As String

            _conditionalOpenConnection()

            Try
                cmd = New OracleClient.OracleCommand
                cmd.Connection = _conn
                If Not _tx Is Nothing Then cmd.Transaction = _tx

                ' --- Cancellazione della t_bil_programmati --- '
                strQuery = "delete from t_bil_programmati "
                strQuery += "where bip_paz_codice = :paz_codice "
                strQuery += "and bip_stato = 'UX'"

                ExecDeleteQuery(cmd, strQuery, paz_codice)


                ' --- Cancellazione della t_vac_programmate --- '
                strQuery = "delete from t_vac_programmate where vpr_paz_codice = :paz_codice"

                ExecDeleteQuery(cmd, strQuery, paz_codice)


                '  --- Cancellazione della t_paz_ritardi --- '
                strQuery = "delete from t_paz_ritardi where pri_paz_codice = :paz_codice"

                ExecDeleteQuery(cmd, strQuery, paz_codice)


                ' --- Cancellazione t_cnv_cicli --- '
                strQuery = "delete from t_cnv_cicli where cnc_cnv_paz_codice = :paz_codice"

                ExecDeleteQuery(cmd, strQuery, paz_codice)

                ' --- Cancellazione t_cnv_convocazioni (solo cnv senza appuntamento) --- '
                strQuery = "delete from t_cnv_convocazioni "
                strQuery += "where cnv_paz_codice = :paz_codice "
                strQuery += "and cnv_data_appuntamento is null"

                ExecDeleteQuery(cmd, strQuery, paz_codice)


                If del_cnv_con_appuntamento Then

                    ' --- Cancellazione t_cnv_convocazioni (cnv con appuntamento) --- '
                    strQuery = "delete from t_cnv_convocazioni "
                    strQuery += "where cnv_paz_codice = :paz_codice "
                    strQuery += "and cnv_data_appuntamento is not null"

                    ExecDeleteQuery(cmd, strQuery, paz_codice)

                End If


            Finally
                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

        End Sub


        Private Sub ExecDeleteQuery(ByRef cmd As OracleClient.OracleCommand, ByVal strQuery As String, ByVal paz_codice As Integer)
            cmd.CommandText = strQuery
            cmd.Parameters.AddWithValue("paz_codice", paz_codice)

            cmd.ExecuteNonQuery()

            cmd.CommandText = String.Empty
            cmd.Parameters.Clear()
        End Sub

#End Region


#Region " --- Update Cnv --- "

        ''' <summary>
        ''' Aggiornamento di tutte le convocazioni, cancellando eventuali appuntamenti.
        ''' Vengono cancellati l'ambulatorio, il tipo di appuntamento e la data. 
        ''' Se c'è una data di appuntamento, viene impostata come data di primo appuntamento (se vuota).
        ''' </summary>
        Public Sub UpdateCnsConvocazioniConApp(ByVal paz_codice As Integer, ByVal cod_cns As String) Implements IDALRicezioneMessaggiEsterni.UpdateCnsConvocazioniConApp
            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim strQuery As String = String.Empty

            _conditionalOpenConnection()

            Try
                cmd = New OracleClient.OracleCommand
                cmd.Connection = _conn
                If Not _tx Is Nothing Then cmd.Transaction = _tx

                ' --- Aggiornamento cnv con appuntamento ma senza data primo appuntamento --- '
                strQuery = "update t_cnv_convocazioni "
                strQuery += "set cnv_cns_codice = :cod_cns, cnv_amb_codice = null, "
                strQuery += "cnv_tipo_appuntamento = null, cnv_data_appuntamento = null, "
                strQuery += "cnv_primo_appuntamento = cnv_data_appuntamento "
                strQuery += "where cnv_paz_codice = :paz_codice "
                strQuery += "and cnv_data_appuntamento is not null "
                strQuery += "and cnv_primo_appuntamento is null"
                cmd.CommandText = strQuery

                cmd.Parameters.AddWithValue("cod_cns", cod_cns)
                cmd.Parameters.AddWithValue("paz_codice", paz_codice)

                cmd.ExecuteNonQuery()

                cmd.CommandText = String.Empty
                cmd.Parameters.Clear()


                ' --- Aggiornamento cnv con appuntamento e con data primo appuntamento --- '
                strQuery = "update t_cnv_convocazioni "
                strQuery += "set cnv_cns_codice = :cod_cns, cnv_amb_codice = null, "
                strQuery += "cnv_data_appuntamento = null, cnv_tipo_appuntamento = null "
                strQuery += "where cnv_paz_codice = :paz_codice "
                strQuery += "and cnv_data_appuntamento is not null "
                strQuery += "and cnv_primo_appuntamento is not null"
                cmd.CommandText = strQuery

                cmd.Parameters.AddWithValue("cod_cns", cod_cns)
                cmd.Parameters.AddWithValue("paz_codice", paz_codice)

                cmd.ExecuteNonQuery()

            Finally
                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

        End Sub


        ''' <summary>
        ''' Aggiornamento delle sole convocazioni senza appuntamento.
        ''' Viene impostato solo il nuovo consultorio. 
        ''' </summary>
        Public Sub UpdateCnsConvocazioniSenzaApp(ByVal paz_codice As Integer, ByVal cod_cns As String) Implements IDALRicezioneMessaggiEsterni.UpdateCnsConvocazioniSenzaApp
            Dim cmd As OracleClient.OracleCommand = Nothing
            Dim strQuery As String = "update t_cnv_convocazioni "
            strQuery += "set cnv_cns_codice = :cod_cns, cnv_amb_codice = null "
            strQuery += "where cnv_paz_codice = :paz_codice "
            strQuery += "and cnv_data_appuntamento is null"

            _conditionalOpenConnection()

            Try
                cmd = New OracleClient.OracleCommand(strQuery, _conn)
                If Not _tx Is Nothing Then cmd.Transaction = _tx

                cmd.Parameters.AddWithValue("cod_cns", cod_cns)
                cmd.Parameters.AddWithValue("paz_codice", paz_codice)

                cmd.ExecuteNonQuery()

            Finally
                If Not cmd Is Nothing Then cmd.Dispose()

            End Try

        End Sub


#End Region


    End Class


End Namespace

