Imports Onit.OnAssistnet.OnVac.Common.Utility
Imports Onit.OnAssistnet.OnVac.Enumerators


Namespace Biz

    ''' <summary>
    ''' Questa libreria è utilizzata da OnVacActor per impattare su OnVac i dati presenti nel messaggio.
    ''' Viene richiamto il metodo RicezioneMessaggio che contiene al suo interno le logiche di gestione dei dati.
    ''' Il costruttore crea il proprio dal di gestione dei dati, utilizzando la stringa di connessione al db di OnVac 
    ''' </summary>
    Public Class BizRicezioneMessaggiEsterni
        Implements IDisposable

#Region " Private "

        Private _warning As String
        Private _message As String

        Private _dal As Dal.IDALRicezioneMessaggiEsterni
        Private _genericprovider As Dal.DbGenericProvider
        Private _settings As Onit.OnAssistnet.OnVac.Settings.Settings

        Private ente_invio As String = String.Empty         ' "APC"
        Private ente_ricezione As String = String.Empty     ' "OnVac"

        Private AppId As String = String.Empty
        Private Azienda As String = String.Empty

#End Region

#Region " Costruttori "

        ''' <summary>
        ''' Costruttore. Cerca la stringa di connessione all'applicativo nella t_ana_applicativi a partire da AppId e Azienda. 
        ''' Utilizza come ente di invio ed ente di ricezione i valori specificati nei parametri.
        ''' </summary>
        Public Sub New(AppId As String,
                       Azienda As String,
                       enteInvio As String,
                       enteRicezione As String)

            Me.ente_invio = enteInvio
            Me.ente_ricezione = enteRicezione
            Me.AppId = AppId
            Me.Azienda = Azienda

            ' Creazione del dal interno
            Dim c As ConnectionInfo = Me.GetConnectionInfo()
            _dal = _getDALInstance(c.Provider, c.ConnectionString, Nothing, Nothing)

            ' Creazione del generic provider per l'utilizzo del dal di OnVac
            _genericprovider = New Dal.DbGenericProvider(c)

            ' Lettura parametri di OnVac
            ' Leggo i parametri comuni e quelli del consultorio fittizio "ALLINEA" che in realtà contiene i valori impostati
            ' nel caso di allineamento automatico dei pazienti.
            _settings = New Settings.Settings("ALLINEA", _genericprovider)

        End Sub

        Private Function GetConnectionInfo() As ConnectionInfo
            Dim app As Onit.Shared.Manager.Apps.App = Onit.Shared.Manager.Apps.App.getInstance(Me.AppId, Me.Azienda)
            Dim cInfo As Onit.Shared.NTier.Dal.DAAB.DbConnectionInfo = app.getConnectionInfo()
            Return New ConnectionInfo(cInfo.Provider.ToString(), cInfo.ConnectionString)
        End Function
        
        Private Function _getDALInstance(_provider As String, _connection_string As String, ByRef _conn As IDbConnection, ByRef _tx As IDbTransaction) As Dal.IDALRicezioneMessaggiEsterni

            Dim asm As System.Reflection.Assembly = System.Reflection.Assembly.GetExecutingAssembly()

            Dim strClass As String = String.Empty

            Select Case _provider.ToUpper()
                Case "ORACLE"
                    strClass = "Onit.OnAssistnet.OnVac.Dal.Oracle.OracleDalRicezioneMessaggiEsterni"
                Case "SQLSERVER"
                    strClass = "Onit.OnAssistnet.OnVac.Dal.Sql.SqlDALRicezioneMessaggiEsterni"
            End Select

            ' Richiama il costruttore del dal in base ai parametri passati
            Dim _dal As Dal.IDALRicezioneMessaggiEsterni

            If _conn Is Nothing Then
                _dal = DirectCast(asm.CreateInstance(strClass, True, Nothing, Nothing, New Object() {_provider, _connection_string}, Nothing, Nothing), Dal.IDALRicezioneMessaggiEsterni)
            Else
                _dal = DirectCast(asm.CreateInstance(strClass, True, Nothing, Nothing, New Object() {_provider, _conn, _tx}, Nothing, Nothing), Dal.IDALRicezioneMessaggiEsterni)
            End If

            Return _dal

        End Function

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Metodo di ricezione del messaggio. Effettua l'insert o l'update dei dati del paziente, 
        ''' oppure il merge dei dati di master e alias, e gestisce i dati vaccinali.
        ''' </summary>
        Public Function RicezioneMessaggio(msg As Onit.OnAssistnet.Contracts.Messaggio) As RicezioneMessaggiEsterniResponse

            ' TODO [AURV]: quando passa di qui? CONTROLLARE!!!


            ' Stringa contenente l'id del messaggio ricevuto, da stampare nel log
            Dim strIdMsg As String = String.Format(" Id_msg: {0} ", msg.ControlID)

            ' Non gestisco messaggi diversi da quelli di tipo ADT
            If msg.Type <> "ADT" Then
                _message = strIdMsg + " Tipo del messaggio non gestito."
                Return New RicezioneMessaggiEsterniResponse() With {.Success = False, .Message = _message}
            End If


            ' --- Messaggio di Merge --- '
            If (msg.TriggerEvent = "A40") Then

                ' --- Codice centrale del master (dal msg) --- '
                Dim codMasterCentrale As String = String.Empty
                codMasterCentrale = getCodiceCentrale(msg)

                If (codMasterCentrale = String.Empty) Then
                    _message = strIdMsg + "Il messaggio non contiene il codice del paziente master. Unificazione non effettuata."
                    Return New RicezioneMessaggiEsterniResponse() With {.Success = False, .Message = _message}
                End If

                ' --- Codice centrale dell'alias (dal msg) --- '
                Dim codAliasCentrale As String = String.Empty

                If msg.PazienteAlias Is Nothing OrElse msg.PazienteAlias.DatiAnagrafici Is Nothing _
                   OrElse msg.PazienteAlias.DatiAnagrafici.Codice Is Nothing OrElse msg.PazienteAlias.DatiAnagrafici.Codice = String.Empty Then
                    _message = strIdMsg + "Il messaggio non contiene il codice del paziente alias. Unificazione non effettuata."
                    Return New RicezioneMessaggiEsterniResponse() With {.Success = False, .Message = _message}
                End If

                codAliasCentrale = msg.PazienteAlias.DatiAnagrafici.Codice

                Dim msg_codici As String = String.Format(" Master: {0} Alias: {1}", codMasterCentrale, codAliasCentrale)

                ' --- Codice del paziente alias su db --- '
                ' Cerco prima l'alias perchè, se non è in anagrafe, non devo nemmeno inserire il master.
                Dim codAlias As Integer = -1

                Dim coll_alias As Collection.PazienteCollection
                coll_alias = _dal.GetPazientiByCodiceAusiliario(codAliasCentrale)

                ' Se non ho trovato l'alias, non eseguo il merge.
                If coll_alias.Count = 0 Then
                    _message = strIdMsg + "Paziente alias non presente in anagrafe. Unificazione non effettuata." + msg_codici
                    Return New RicezioneMessaggiEsterniResponse() With {.Success = False, .Message = _message}
                End If

                codAlias = coll_alias(0).Paz_Codice


                ' --- Codice del paziente master su db --- '
                Dim codMaster As Integer = -1

                ' Ricerca codice locale del master
                Dim codMasterLocale As Integer = -1
                codMasterLocale = getCodiceLocale(msg)    ' lettura dal messaggio

                ' Ricerca del paziente master tramite codice locale su db
                If (codMasterLocale <> -1) Then
                    Dim paz As Entities.Paziente = Nothing
                    paz = _dal.GetPazienteByCodiceLocale(codMasterLocale)
                    If Not paz Is Nothing Then
                        codMaster = paz.Paz_Codice
                    End If
                End If

                If (codMaster = -1) Then
                    ' Se non ho trovato il master tramite codice locale, ricerca tramite codice centrale
                    Dim coll_master As Collection.PazienteCollection
                    coll_master = _dal.GetPazientiByCodiceAusiliario(codMasterCentrale)

                    ' Se non ho trovato il master, lo inserisco.
                    If (coll_master.Count = 0) Then
                        ' INSERT
                        ' L'inserimento avviene sempre, senza considerare se è residente o domiciliato.
                        codMaster = InserimentoPaziente(msg, False)
                        If (codMaster = -1) Then
                            ' Concateno l'id del messaggio e i codici di master e alias al messaggio di errore 
                            ' impostato dalla funzione di inserimento paziente.
                            _message = strIdMsg + _message + " Paziente master non inserito. Unificazione non effettuata." + msg_codici
                            Return New RicezioneMessaggiEsterniResponse() With {.Success = False, .Message = _message}
                        End If
                    Else
                        ' Paziente master trovato
                        codMaster = coll_master(0).Paz_Codice
                    End If
                End If


                ' --- Unificazione pazienti --- '
                Dim result As BizResult

                ' TODO [Unificazione Ulss]: bisognerebbe passare il codice azienda, ma sta roba è ancora in uso???
                Dim bizContextInfo As New BizContextInfos(0, Azienda, AppId)

                Using bizPazienti As New BizPaziente(_genericprovider, _settings, bizContextInfo, Nothing)

                    result = bizPazienti.UnisciPazienti(codMaster, codAlias)

                End Using

                Dim resultMessage As String = "Accorpamento effettuato."

                If Not result.Success Then

                    resultMessage = "Accorpamento non effettuato. " + result.Messages.ToString()

                End If

                ' Valorizza il messaggio con il risultato, i codici dei pazienti master e alias e il motivo dell'eventuale errore.
                _message = String.Format("{0}{1}{2}", strIdMsg, resultMessage, msg_codici)

                Return New RicezioneMessaggiEsterniResponse() With {.Success = result.Success, .Message = _message}

            End If


            ' --- Messaggio di Insert/Update --- '
            If (msg.TriggerEvent = "A28" Or msg.TriggerEvent = "A31") Then
                ' In entrambi i casi (inserimento/modifica) devo prima determinare i due codici centrale e locale, effettuare la ricerca
                ' per determinare se il paziente è già presente in anagrafe locale ed effettuare di conseguenza una insert o una update.

                ' Lettura del codice centrale presente nel messaggio
                Dim cod_centrale As String = String.Empty
                cod_centrale = getCodiceCentrale(msg)

                ' Il codice centrale deve esserci.
                If cod_centrale = String.Empty Then
                    ' ??? LOG ??? '
                    _message = strIdMsg + "Il messaggio ricevuto non contiene il codice centrale del paziente."
                    Return New RicezioneMessaggiEsterniResponse() With {.Success = False, .Message = _message}
                End If

                Dim msg_cod_paz As String = " Paziente: " + cod_centrale

                ' Lettura del codice locale presente nel messaggio
                Dim cod_locale As Integer = -1
                cod_locale = getCodiceLocale(msg)

                ' Codice del paziente trovato (su db)
                Dim paz_codice As Integer = -1


                '' Ricerca del paziente tramite codice locale
                'If (cod_locale <> -1) Then
                '    Dim paz As Entities.Paziente = Nothing
                '    paz = _dal.GetPazienteByCodiceLocale(cod_locale)
                '    If Not paz Is Nothing Then
                '        ' Controllo che il codice ausiliario sia uguale al codice centrale
                '        If (paz.Paz_Codice <> cod_centrale) Then
                '            ' ??? LOG ??? '
                '            _message = "Il messaggio presenta un codice ausiliario e un codice locale non corrispondenti per il paziente trovato."
                '            Return False
                '        End If
                '        paz_codice = paz.Paz_Codice
                '    End If
                'End If

                'If (paz_codice = -1) Then
                '   ' Se non ho trovato il paziente tramite codice locale, ricerca tramite codice centrale


                ' --- RICERCA PER CODICE CENTRALE (ausiliario) --- '
                Dim coll_paz As Collection.PazienteCollection
                coll_paz = _dal.GetPazientiByCodiceAusiliario(cod_centrale)

                If (coll_paz.Count = 0) Then
                    ' Nessun paziente presente con il codice centrale specificato nel messaggio.

                    ' Controllo se nel messaggio è presente il codice locale
                    If (cod_locale = -1) Then
                        ' Se il codice locale non è presente nel messaggio, inserisco il paziente.

                        ' INSERT
                        ' L'inserimento avviene solo se il paziente è residente o domiciliato.
                        paz_codice = InserimentoPaziente(msg, True)
                        If (paz_codice = -1) Then
                            _message = strIdMsg + _message + msg_cod_paz
                            Return New RicezioneMessaggiEsterniResponse() With {.Success = False, .Message = _message}
                        Else
                            _message = strIdMsg + "Inserimento effettuato." + msg_cod_paz
                        End If

                    Else
                        ' Se il codice locale è presente nel messaggio, cerco su db per codice locale
                        Dim paz As Entities.Paziente = Nothing
                        paz = _dal.GetPazienteByCodiceLocale(cod_locale)

                        If (Not paz Is Nothing) Then
                            ' Se trovo un paziente corrispondente, posso fare l'update perchè sono sicuro che il codice centrale
                            ' non è associato ad un altro paziente (ho già fatto la ricerca in precedenza)

                            ' UPDATE
                            If Not ModificaPaziente(paz.Paz_Codice, msg) Then
                                _message = strIdMsg + _message + msg_cod_paz
                                Return New RicezioneMessaggiEsterniResponse() With {.Success = False, .Message = _message}
                            Else
                                _message = strIdMsg + "Variazione effettuata." + msg_cod_paz
                            End If
                        Else
                            ' Non ho trovato nessun paziente associato nè al codice centrale nè a quello locale. Lo inserisco.

                            ' INSERT
                            ' L'inserimento avviene solo se il paziente è residente o domiciliato all'interno della asl.
                            paz_codice = InserimentoPaziente(msg, True)
                            If (paz_codice = -1) Then
                                _message = strIdMsg + _message + msg_cod_paz
                                Return New RicezioneMessaggiEsterniResponse() With {.Success = False, .Message = _message}
                            Else
                                _message = strIdMsg + "Inserimento effettuato." + msg_cod_paz
                            End If

                        End If


                    End If

                ElseIf (coll_paz.Count = 1) Then
                    ' C'è un paziente con il codice ausiliario uguale al codice centrale. 
                    ' Controllo che anche il codice locale sia corretto (se c'è).
                    Dim paz As Entities.Paziente = coll_paz(0)
                    If (cod_locale <> -1 And cod_locale <> paz.Paz_Codice) Then
                        ' Il paziente trovato non ha il codice locale corrispondente.

                        ' Il messaggio verrà scartato. In più, controllo se esiste un paziente con il codice locale 
                        ' presente nel messaggio, per sbiancargli il codice centrale.
                        Dim msg_paz_canc As String = "Il messaggio presenta un codice ausiliario e un codice locale non corrispondenti per il paziente trovato. "
                        Dim pazAusiliarioDaCanc As Entities.Paziente = Nothing
                        pazAusiliarioDaCanc = _dal.GetPazienteByCodiceLocale(cod_locale)
                        If (Not pazAusiliarioDaCanc Is Nothing) Then
                            _dal.UpdateCodiceAusiliario(cod_locale, String.Empty)
                            msg_paz_canc += "Il codice ausiliario del paziente corrispondente al codice locale e' stato sbiancato."
                        Else
                            msg_paz_canc += "Non esiste nessun paziente corrispondente al codice locale riportato nel messaggio."
                        End If

                        ' ERRORE
                        ' ??? LOG ??? '
                        _message = strIdMsg + msg_paz_canc + msg_cod_paz
                        Return New RicezioneMessaggiEsterniResponse() With {.Success = False, .Message = _message}

                    Else
                        ' Il codice locale non è nel messaggio, oppure c'è ed è uguale a quello del paziente trovato.

                        ' UPDATE
                        If Not ModificaPaziente(paz.Paz_Codice, msg) Then
                            _message = strIdMsg + _message + msg_cod_paz
                            Return New RicezioneMessaggiEsterniResponse() With {.Success = False, .Message = _message}
                        Else
                            _message = strIdMsg + "Variazione effettuata." + msg_cod_paz
                        End If
                    End If

                Else    ' (coll_paz.Count > 1)
                    ' Ci sono più pazienti con codice ausiliario uguale al centrale.
                    ' Seleziono quello giusto in base al codice locale.
                    If (cod_locale = -1) Then
                        ' ERRORE
                        ' Non c'è il codice locale, non ho modo di identificare univocamente il paziente.
                        ' ??? LOG ??? '
                        _message = strIdMsg + "Ci sono più pazienti aventi stesso codice centrale. Codice locale mancante. Impossibile identificare univocamente il paziente." + msg_cod_paz
                        Return New RicezioneMessaggiEsterniResponse() With {.Success = False, .Message = _message}
                    Else
                        ' Cerco se uno dei pazienti trovati ha il codice uguale al codice locale.
                        Dim paz As Entities.Paziente
                        paz = coll_paz.FindByCodPaziente(cod_locale.ToString)
                        If (paz Is Nothing) Then
                            ' ERRORE
                            ' ??? LOG ??? '
                            _message = strIdMsg + "Ci sono più pazienti aventi stesso codice centrale, nessuno corrispondente al codice locale. Impossibile identificare univocamente il paziente." + msg_cod_paz
                            Return New RicezioneMessaggiEsterniResponse() With {.Success = False, .Message = _message}
                        Else
                            ' UPDATE
                            If Not ModificaPaziente(paz.Paz_Codice, msg) Then
                                _message = strIdMsg + _message + msg_cod_paz
                                Return New RicezioneMessaggiEsterniResponse() With {.Success = False, .Message = _message}
                            Else
                                _message = strIdMsg + "Variazione effettuata." + msg_cod_paz
                            End If
                        End If
                    End If

                End If
                ' ------------------------------------------------ '


                'Else
                '    ' Paziente trovato tramite codice locale

                '    ' UPDATE
                '    If Not ModificaPaziente(paz_codice, msg) Then
                '        Return False
                '    Else
                '        _message = "Variazione effettuata"
                '    End If

                'End If


                ' Concateno gli eventuali messaggi di warning all'indicazione che l'operazione è andata a buon fine.
                If (_warning <> String.Empty) Then
                    _message += " ATTENZIONE:" + _warning + msg_cod_paz
                End If
                Return New RicezioneMessaggiEsterniResponse() With {.Success = True, .Message = _message}

            End If

            ' Tutti gli altri casi non sono previsti
            _message = strIdMsg + " Tipo di messaggio non gestito."
            Return New RicezioneMessaggiEsterniResponse() With {.Success = False, .Message = _message}

        End Function

#End Region

#Region " Private Methods "

        ' Ricerca codice ausiliario (codice centrale) tra i codici presenti per il paziente. Se non lo trova restituisce la stringa vuota.
        Private Function getCodiceCentrale(msg As Onit.OnAssistnet.Contracts.Messaggio) As String

            Dim cod_ausiliario As String = String.Empty

            Dim codici() As Onit.OnAssistnet.Contracts.ID = msg.Pazienti(0).DatiAnagrafici.IDArray

            For i As Integer = 0 To codici.Length - 1

                If codici(i).TipoCodificatoID = Onit.OnAssistnet.Contracts.TipoCodificatoID.Aziendale And
                   codici(i).AutoritaID = ente_invio Then

                    cod_ausiliario = codici(i).IDValue
                    Exit For

                End If

            Next

            Return cod_ausiliario

        End Function

        ' Ricerca codice locale tra i codici presenti per il paziente. Se non lo trova restituisce la stringa vuota.
        ' Il tipo codificato è aziendale o locale, e l'autorità che assegna è OnVac stesso.
        Private Function getCodiceLocale(msg As Onit.OnAssistnet.Contracts.Messaggio) As Integer

            Dim strCodLocale As String = String.Empty

            Dim codici() As Onit.OnAssistnet.Contracts.ID = msg.Pazienti(0).DatiAnagrafici.IDArray

            For i As Integer = 0 To codici.Length - 1

                If (codici(i).TipoCodificatoID = Onit.OnAssistnet.Contracts.TipoCodificatoID.Aziendale Or
                    codici(i).TipoCodificatoID = Onit.OnAssistnet.Contracts.TipoCodificatoID.Locale) And
                   codici(i).AutoritaID = ente_ricezione Then

                    strCodLocale = codici(i).IDValue
                    Exit For

                End If

            Next

            Dim cod_locale As Integer = -1

            If strCodLocale <> String.Empty Then
                Try
                    cod_locale = System.Convert.ToInt32(strCodLocale)
                Catch ex As Exception
                    cod_locale = -1
                End Try
            End If

            Return cod_locale

        End Function

#Region " Inserimento "

        ' Inserimento paziente. Se il parametro checkStatiAnagInserimento è true, l'inserimento avviene solo se residente o domiciliato. 
        ' Restituisce il codice del paziente inserito, -1 se non ha inserito il paziente.
        Private Function InserimentoPaziente(ByVal msg As Onit.OnAssistnet.Contracts.Messaggio, ByVal checkInserimento As Boolean) As Integer
            Dim paz_cod As Integer = -1

            Dim dati_paz As Onit.OnAssistnet.Contracts.Paziente
            dati_paz = msg.Pazienti(0)

            ' Ricerca comuni residenza e domicilio presenti nel messaggio
            Dim com_res As String = String.Empty
            Dim com_dom As String = String.Empty

            For i As Int16 = 0 To dati_paz.DatiAnagrafici.Indirizzi.Length - 1
                If dati_paz.DatiAnagrafici.Indirizzi(i).TipoCodificato = Onit.OnAssistnet.Contracts.TipoCodificatoIndirizzo.Legal Then
                    ' Residenza
                    com_res = dati_paz.DatiAnagrafici.Indirizzi(i).ComuneCodiceXMPI
                ElseIf dati_paz.DatiAnagrafici.Indirizzi(i).TipoCodificato = Onit.OnAssistnet.Contracts.TipoCodificatoIndirizzo.Home Then
                    ' Domicilio
                    com_dom = dati_paz.DatiAnagrafici.Indirizzi(i).ComuneCodiceXMPI
                End If
            Next


            ' --- Controllo inserimento --- '
            ' In caso di merge, il parametro vale false (eseguo sempre l'inserimento, senza controllare residenza/domicilio).
            If (checkInserimento) Then
                ' Il paziente deve essere inserito solo se il comune di residenza o di domicilio appartiene all'asl di lavoro
                Dim asl_res As String = String.Empty
                Dim asl_dom As String = String.Empty

                ' Se residenza e domicilio vuoti, scarto il messaggio
                If (com_res = String.Empty And com_dom = String.Empty) Then
                    ' ??? LOG ??? '
                    _message = "Messaggio ricevuto ma inserimento non effettuato: non sono presenti ne' il comune di residenza ne' quello di domicilio. "
                    Return -1
                End If

                ' Controllo la asl del comune di residenza
                If (com_res <> String.Empty) Then
                    asl_res = _dal.GetCodiceAslByCodiceComune(com_res)
                End If

                ' Controllo la asl del comune di domicilio
                If (com_dom <> String.Empty) Then
                    ' Controllo la asl del comune di domicilio
                    asl_dom = _dal.GetCodiceAslByCodiceComune(com_dom)
                End If


                ' Controllo asl
                If (asl_res <> _settings.CODICE_ASL And asl_dom <> _settings.CODICE_ASL) Then
                    ' Se le asl associate al comune di residenza e di domicilio del paziente sono diverse da quella di lavoro,
                    ' significa che il paziente non è residente nè domiciliato all'interno dell'asl, quindi non verrà inserito.
                    ' ??? LOG ??? '
                    _message = "Messaggio ricevuto ma inserimento non effettuato: il paziente non e' residente ne' domiciliato all'interno della asl. "
                    Return -1
                End If

            End If


            ' --- Calcolo stato anagrafico --- '
            Dim stato_anag As Onit.OnAssistnet.OnVac.Enumerators.StatoAnagrafico

            ' In inserimento, se il parametro GES_AUTO_STATO_ANAGRAFICO è True, calcolo lo stato anagrafico in base ai dati del messaggio.
            ' Altrimenti, leggo lo stato anagrafico di default dalla tabella.
            If (_settings.GES_AUTO_STATO_ANAGRAFICO) Then
                stato_anag = CalcolaStatoAnagrafico(msg)
            Else
                stato_anag = _genericprovider.StatiAnagrafici.GetStatoAnagCodificatoDefault()
            End If

            ' Assegnazione dello stato anagrafico calcolato 
            msg.Pazienti(0).DatiAnagrafici.StatoAnagrafico = stato_anag


            ' --- Calcolo consultorio vaccinale e territoriale --- '
            Dim cod_cns_vacc As String = String.Empty
            Dim cod_cns_terr As String = String.Empty
            Dim data_cns_vacc As Date = Date.MinValue

            ' I consultori vaccinale e territoriale devono essere calcolati solo se il parametro relativo è True
            If (_settings.AUTOSETCNS_INSLOCALE) Then

                ' Cns vaccinale
                cod_cns_vacc = CalcolaCnsVaccinale(msg, com_res, com_dom)
                If (cod_cns_vacc <> String.Empty) Then
                    ' Data di assegnazione del consultorio vaccinale
                    data_cns_vacc = Date.Now.Date
                End If

                ' Cns territoriale
                cod_cns_terr = CalcolaCnsTerritoriale(msg, com_res)

            End If


            ' --- Impostazione del flag regolarizzato --- '
            Dim paz_regolarizzato As String
            paz_regolarizzato = CalcolaFlagRegolarizzato(msg, -1)


            Try
                _dal.BeginTransaction()

                ' --- Inserimento dati anagrafici del paziente --- '
                paz_cod = _dal.InsertPaziente(msg, paz_regolarizzato, cod_cns_vacc, data_cns_vacc, cod_cns_terr)

                If (paz_cod = -1) Then
                    Throw New Exception("Impossibile ottenere il codice locale del paziente. ")
                End If

                ' --- Caricamento automatico cicli --- '
                If (_settings.AUTO_CALC_CICLI) Then
                    _dal.InserimentoCicliPaziente(paz_cod, msg.Pazienti(0).DatiAnagrafici.DataNascita, msg.Pazienti(0).DatiAnagrafici.Sesso)
                End If

                ' --- Inserimento movimento --- '
                If (cod_cns_vacc <> String.Empty) Then
                    _dal.InserimentoMovimentoPaziente(paz_cod, String.Empty, cod_cns_vacc)
                End If

                _dal.Transaction.Commit()

            Catch ex As Exception
                _dal.Transaction.Rollback()
                ' ??? LOG ??? '
                _message = "Inserimento non effettuato. Errore durante l'inserimento dei dati del paziente. " + ex.ToString
                Return -1
            End Try

            Return paz_cod
        End Function

#End Region

#Region " Modifica "

        Private Function ModificaPaziente(cod_paz As Integer, msg As Onit.OnAssistnet.Contracts.Messaggio) As Boolean

            _warning = String.Empty

            ' --- Lettura dati del paziente da db (valori old) --- '
            Dim coll_paz As Collection.PazienteCollection
            coll_paz = _genericprovider.Paziente.GetPazienti(cod_paz)

            Dim paz_sesso_old As String = String.Empty
            Dim paz_data_nascita_old As Date = Date.MinValue
            Dim com_res_old As String = String.Empty
            Dim com_dom_old As String = String.Empty
            Dim cod_cns_vacc_old As String = String.Empty

            If (Not coll_paz Is Nothing) AndAlso coll_paz.Count > 0 Then
                ' Se ho trovato un paziente, leggo i suoi dati
                paz_sesso_old = coll_paz(0).Sesso
                paz_data_nascita_old = coll_paz(0).Data_Nascita
                com_res_old = coll_paz(0).ComuneResidenza_Codice
                com_dom_old = coll_paz(0).ComuneDomicilio_Codice
                cod_cns_vacc_old = coll_paz(0).Paz_Cns_Codice
            End If

            Dim com_emig_old As String = String.Empty
            Dim com_immig_old As String = String.Empty
            com_immig_old = _genericprovider.Paziente.GetCodiceComuneImmigrazione(cod_paz)
            com_emig_old = _genericprovider.Paziente.GetCodiceComuneEmigrazione(cod_paz)


            ' --- Dati del paziente presenti nel messaggio ricevuto (valori new) --- '
            Dim dati_paz As Onit.OnAssistnet.Contracts.Paziente
            dati_paz = msg.Pazienti(0)

            ' Ricerca comuni di residenza/domicilio/emigrazione/immigrazione presenti nel messaggio (valori new)
            Dim com_res_new As String = String.Empty
            Dim com_dom_new As String = String.Empty
            Dim com_emig_new As String = String.Empty
            Dim com_immig_new As String = String.Empty

            For i As Int16 = 0 To dati_paz.DatiAnagrafici.Indirizzi.Length - 1
                If dati_paz.DatiAnagrafici.Indirizzi(i).TipoCodificato = Onit.OnAssistnet.Contracts.TipoCodificatoIndirizzo.Current Then
                    ' Emigrazione
                    com_emig_new = dati_paz.DatiAnagrafici.Indirizzi(i).ComuneCodiceXMPI
                ElseIf dati_paz.DatiAnagrafici.Indirizzi(i).TipoCodificato = Onit.OnAssistnet.Contracts.TipoCodificatoIndirizzo.Foreign Then
                    ' Immigrazione
                    com_immig_new = dati_paz.DatiAnagrafici.Indirizzi(i).ComuneCodiceXMPI
                ElseIf dati_paz.DatiAnagrafici.Indirizzi(i).TipoCodificato = Onit.OnAssistnet.Contracts.TipoCodificatoIndirizzo.Legal Then
                    ' Residenza
                    com_res_new = dati_paz.DatiAnagrafici.Indirizzi(i).ComuneCodiceXMPI
                ElseIf dati_paz.DatiAnagrafici.Indirizzi(i).TipoCodificato = Onit.OnAssistnet.Contracts.TipoCodificatoIndirizzo.Home Then
                    ' Domicilio
                    com_dom_new = dati_paz.DatiAnagrafici.Indirizzi(i).ComuneCodiceXMPI
                End If
            Next


            ' --- Controllo coerenza cicli --- '
            ' Da effettuare in base al parametro e solo se sono variati il sesso e/o la data di nascita del paziente
            If (_settings.CHECK_CICLI) Then
                If (dati_paz.DatiAnagrafici.Sesso <> paz_sesso_old) Or (dati_paz.DatiAnagrafici.DataNascita <> paz_data_nascita_old) Then

                    ' Conteggio cicli incompatibili per il paziente (per sessoe/o data di nascita)
                    Dim cnt As Integer = 0
                    cnt = _dal.CountCicliIncompatibili(cod_paz, dati_paz.DatiAnagrafici.DataNascita, dati_paz.DatiAnagrafici.Sesso)

                    ' Se ci sono cicli incompatibili, devo loggare il messaggio e continuare.
                    If (cnt > 0) Then
                        ' ??? LOG ??? '
                        _warning = " In seguito alla modifica, il paziente presenta cicli vaccinali non coerenti per sesso o data di nascita."
                    Else
                        _warning = " Controllo cicli effettuato con successo."
                    End If

                End If
            End If


            ' --- Stato anagrafico --- '
            Dim stato_anag_new As StatoAnagrafico

            ' In modifica, se il parametro GES_AUTO_STATO_ANAGRAFICO è True, ricalcolo lo stato anagrafico in base ai dati ricevuti.
            If (_settings.GES_AUTO_STATO_ANAGRAFICO) Then
                stato_anag_new = CalcolaStatoAnagrafico(msg)

                ' Controllo la correttezza del nuovo stato anagrafico calcolato.
                If (stato_anag_new = StatoAnagrafico.IMMIGRATO) Then
                    ' Se il paz è IMMIGRATO, può darsi che sia stato regolarizzato ma in centrale non risulti.
                    If (com_immig_new <> com_immig_old) Or (com_emig_new <> com_emig_old) Or (com_dom_new <> com_dom_old) Then
                        ' Se uno dei campi che influisce sull'immigrazione è variato rispetto al precedente valore, 
                        ' assegno al paziente lo stato anagrafico IMMIGRATO.
                        msg.Pazienti(0).DatiAnagrafici.StatoAnagrafico = stato_anag_new
                    Else
                        ' Se i campi presi in considerazione non sono variati, lo stato anagrafico non varia.
                        ' Svuoto il campo stato anagrafico. La funzione di update, se lo trova vuoto, non lo modifica.
                        msg.Pazienti(0).DatiAnagrafici.StatoAnagrafico = String.Empty
                    End If

                ElseIf (stato_anag_new = StatoAnagrafico.RESIDENTE And com_res_new = com_res_old) Then
                    ' Se il paz è RESIDENTE ma non è cambiato il comune di residenza: non cambio lo stato anagrafico.
                    msg.Pazienti(0).DatiAnagrafici.StatoAnagrafico = String.Empty

                Else
                    ' In tutti gli altri casi, assegno lo stato anagrafico calcolato.
                    msg.Pazienti(0).DatiAnagrafici.StatoAnagrafico = stato_anag_new

                End If

            Else
                ' Svuoto il campo stato anagrafico. La funzione di update, se lo trova vuoto, non lo modifica.
                msg.Pazienti(0).DatiAnagrafici.StatoAnagrafico = String.Empty
            End If


            ' Circoscrizione presente nel messaggio (valore new)
            Dim cir_paz_new As String = dati_paz.DatiSanitari.Distretto

            ' Lettura circoscrizione del paziente da db (valore old)
            Dim cir_paz_old As String = String.Empty
            cir_paz_old = _genericprovider.Paziente.GetCircoscrizioneResidenza(cod_paz)


            ' --- Calcolo consultorio vaccinale e territoriale --- '
            Dim cod_cns_vacc_new As String = String.Empty
            Dim cod_cns_terr_new As String = String.Empty
            Dim data_cns_vacc_new As Date = Date.MinValue

            ' I consultori vaccinale e territoriale devono essere calcolati solo se il parametro relativo è True
            ' e se il comune di residenza e/o di domicilio è cambiato
            If (_settings.AUTOSETCNS_INSLOCALE) Then

                ' Calcolo il consultorio vaccinale e territoriale solo se sono variati
                ' il comune di residenza o quello di domicilio o la circoscrizione (che mi viene passata nel campo distretto)
                If (com_res_new <> com_res_old) Or (com_dom_new <> com_dom_old) Or (cir_paz_new <> cir_paz_old) Then
                    ' Cns vaccinale (calcolato in base ai dati ricevuti)
                    cod_cns_vacc_new = CalcolaCnsVaccinale(msg, com_res_new, com_dom_new)

                    ' Cns territoriale (calcolato in base ai dati ricevuti)
                    cod_cns_terr_new = CalcolaCnsTerritoriale(msg, com_res_new)

                    ' Calcolo del cns vaccinale in base ai dati del paziente presenti in locale
                    Dim cod_cns_vacc_locale As String = String.Empty

                    ' Se il cns vaccinale rispetto ai dati attuali in locale corrisponde a quello calcolato rispetto ai dati centrali 
                    ' ma e' diverso rispetto a quello attuale, lascio il vecchio cns perchè, evidentemente, 
                    ' e' stato richiesto espressamente dal paziente e non lo devo modificare in automatico.
                    If cod_cns_vacc_new = cod_cns_vacc_locale And coll_paz(0).Paz_Cns_Codice <> cod_cns_vacc_locale Then
                        cod_cns_vacc_new = coll_paz(0).Paz_Cns_Codice
                    End If

                    ' Se il consultorio associato al paziente (già presente su db) è uguale a quello calcolato con i nuovi dati, 
                    ' non modifico nulla. Altrimenti valorizzo il cns old e la data di assegnazione.
                    If coll_paz(0).Paz_Cns_Codice = cod_cns_vacc_new Then
                        cod_cns_vacc_new = String.Empty
                        cod_cns_vacc_old = String.Empty
                        data_cns_vacc_new = Date.MinValue
                    Else
                        cod_cns_vacc_old = coll_paz(0).Paz_Cns_Codice
                        data_cns_vacc_new = Date.Now.Date
                    End If

                End If

            End If

            ' Se non ho determinato il nuovo consultorio vaccinale, deve rimanere inalterato anche il vecchio, quindi lo sbianco.
            If cod_cns_vacc_new = String.Empty Then
                cod_cns_vacc_old = String.Empty
            End If


            ' --- Calcolo flag regolarizzato --- '
            Dim paz_regolarizzato As String = String.Empty

            ' Lettura dello stato anagrafico attuale da db
            Dim strStatoAnagOld As String = _genericprovider.Paziente.GetCodiceStatoAnag(cod_paz)
            Dim stato_anag_old As Enumerators.StatoAnagrafico
            Try
                stato_anag_old = [Enum].Parse(GetType(Enumerators.StatoAnagrafico), strStatoAnagOld)
            Catch ex As Exception
                _message = "Errore di decodifica stato anagrafico del paziente. Codice Stato Anagrafico: " + strStatoAnagOld
                Return False
            End Try


            ' Calcolo il flag regolarizzato solo in questo caso. Altrimenti lo lascio vuoto, così non viene aggiornato.
            If stato_anag_new = StatoAnagrafico.IMMIGRATO And stato_anag_new <> stato_anag_old Then
                paz_regolarizzato = CalcolaFlagRegolarizzato(msg, cod_paz)
            End If


            ' --- Update del paziente in base al codice --- '
            Try
                _dal.BeginTransaction()

                ' Update dei dati anagrafici
                _dal.UpdatePaziente(cod_paz, msg, paz_regolarizzato, cod_cns_vacc_new, cod_cns_vacc_old, data_cns_vacc_new, cod_cns_terr_new)


                ' Controllo se lo stato anagrafico è presente tra gli stati elencati nel parametro
                Dim _canc As Boolean = False
                If _settings.STATIANAG_CANCCNV.Contains(stato_anag_new) Then _canc = True

                If _canc Then
                    ' Se lo stato anagrafico del paziente è uno di quelli elencati, devono essere cancellate 
                    ' le convocazioni senza appuntamento. Quelle con appuntamento vanno cancellate solo se
                    ' il parametro STATIANAG_CANCAPP è impostato a True.
                    _dal.DeleteConvocazioni(cod_paz, _settings.STATIANAG_CANCAPP)

                Else

                    ' Se il consultorio è variato, creo un movimento e aggiorno il cns nelle convocazioni
                    If (cod_cns_vacc_new <> String.Empty) Then

                        ' Inserimento di un movimento per il paziente
                        _dal.InserimentoMovimentoPaziente(cod_paz, cod_cns_vacc_old, cod_cns_vacc_new)

                        ' Se il consultorio è variato, lo aggiorno anche nelle cnv del paziente, se il parametro relativo è true. 
                        If (_settings.UPDCNV_UPDCNS) Then

                            ' Aggiornamento delle convocazioni senza appuntamento.
                            ' Viene impostato solo il nuovo consultorio (e sbiancato l'ambulatorio). 
                            _dal.UpdateCnsConvocazioniSenzaApp(cod_paz, cod_cns_vacc_new)

                            ' Se il parametro UPDCNV_DELAPP è True, l'aggiornamento del cns comporta anche la cancellazione
                            ' della data di appuntamento (e dei dati relativi).
                            If (_settings.UPDCNV_DELAPP) Then
                                ' Aggiornamento di tutte le convocazioni, cancellando eventuali appuntamenti.
                                ' Vengono cancellati l'ambulatorio, il tipo di appuntamento e la data. 
                                ' Se c'è una data di appuntamento, viene impostata come data di primo appuntamento (se vuota).
                                _dal.UpdateCnsConvocazioniConApp(cod_paz, cod_cns_vacc_new)
                            End If

                        Else
                            ' Se non è stata effettuata la variazione del cns nelle convocazioni, devo loggare il messaggio e continuare.
                            ' ??? LOG ??? '
                            _warning += "Il consultorio e' variato ma non e' stato modificato nelle convocazioni del paziente."
                        End If

                    End If

                End If

                _dal.Transaction.Commit()

            Catch ex As Exception
                _dal.Transaction.Rollback()
                ' ??? LOG ??? '
                _message = "Variazione non effettuata. Errore durante la modifica dei dati del paziente. " + ex.ToString
                Return False
            End Try

            Return True

        End Function

#End Region

        ' Restituisce lo stato anagrafico del paziente calcolato in base ai dati contenuti nel messaggio.
        ' Questa funzione è la "traduzione" in vb.net dello script pl-sql:
        ' \\ondc\Dati\Pubblici\Progetti\On.Assisnet\On.Vac\INSTALLAZIONI\VENETO\Integrazione Anagrafe\SCRIPT\GET_STATO_ANAGRAFICO.fnc
        Private Function CalcolaStatoAnagrafico(msg As Onit.OnAssistnet.Contracts.Messaggio) As StatoAnagrafico

            Dim i As Integer

            Dim dati_paz As Onit.OnAssistnet.Contracts.Paziente = msg.Pazienti(0)

            ' --- Deceduto --- '
            If (dati_paz.DatiAnagrafici.DataDecessoSpecified) OrElse (dati_paz.DatiAnagrafici.DataDecesso > Date.MinValue) Then

                Return StatoAnagrafico.DECEDUTO

            End If

            ' Ricerca date immigrazione/emigrazione e comuni immigrazione/emigrazione/residenza/domicilio
            Dim data_emig As Date = Date.MinValue
            Dim data_immig As Date = Date.MinValue
            Dim com_emig As String = String.Empty
            Dim com_immig As String = String.Empty
            Dim com_res As String = String.Empty
            Dim com_dom As String = String.Empty

            Dim _indir As Onit.OnAssistnet.Contracts.Indirizzo

            For i = 0 To dati_paz.DatiAnagrafici.Indirizzi.Length - 1

                _indir = dati_paz.DatiAnagrafici.Indirizzi(i)

                If _indir.TipoCodificato = Onit.OnAssistnet.Contracts.TipoCodificatoIndirizzo.Current Then
                    ' Emigrazione
                    com_emig = _indir.ComuneCodiceXMPI
                    data_emig = _indir.DataInizio
                ElseIf _indir.TipoCodificato = Onit.OnAssistnet.Contracts.TipoCodificatoIndirizzo.Foreign Then
                    ' Immigrazione
                    com_immig = _indir.ComuneCodiceXMPI
                    If (dati_paz.DatiAnagrafici.DataImmigrazioneSpecified) Then
                        data_immig = dati_paz.DatiAnagrafici.DataImmigrazione
                    Else
                        data_immig = _indir.DataInizio
                    End If
                ElseIf _indir.TipoCodificato = Onit.OnAssistnet.Contracts.TipoCodificatoIndirizzo.Legal Then
                    ' Residenza
                    com_res = _indir.ComuneCodiceXMPI
                ElseIf _indir.TipoCodificato = Onit.OnAssistnet.Contracts.TipoCodificatoIndirizzo.Home Then
                    ' Domicilio
                    com_dom = _indir.ComuneCodiceXMPI
                End If
            Next

            ' --- Irreperibile --- '
            ' Il paziente è IRREPERIBILE se:
            ' 1- risulta emigrato in un comune sconosciuto e
            ' 2- le date di emigrazione e immigrazione non sono valorizzate oppure è presente una data di emigrazione successiva all'immigrazione
            If (com_emig = _settings.COMUNE_SCONOSCIUTO) And _
            ((data_emig = Date.MinValue And data_immig = Date.MinValue) Or (data_emig > data_immig)) Then

                Return StatoAnagrafico.IRREPERIBILE

            End If

            ' --- Controllo Emigrazione/Immigrazione --- '

            ' Ricerca asl di emigrazione
            Dim asl_emig As String = String.Empty
            If (com_emig <> String.Empty) Then
                asl_emig = _dal.GetCodiceAslByCodiceComune(com_emig)
            End If

            ' Ricerca asl di immigrazione
            Dim asl_immig As String = String.Empty
            If (com_immig <> String.Empty) Then
                asl_immig = _dal.GetCodiceAslByCodiceComune(com_immig)
            End If

            ' Controllo la presenza di entrambe le date di immigrazione/emigrazione:
            ' se mancano entrambe, ma c'è solo uno tra i due comuni (o immmigrazione o emigrazione), assegno lo stato anagrafico relativo.
            If (data_emig = Date.MinValue And data_immig = Date.MinValue) Then

                If (com_emig <> String.Empty And com_immig = String.Empty) Then
                    ' Controllo emigrazione:
                    ' assegno lo stato emigrato solo se la asl di emigrazione è vuota o diversa da quella di lavoro
                    If (asl_emig = String.Empty Or (asl_emig <> String.Empty And asl_emig <> _settings.CODICE_ASL)) Then

                        Return StatoAnagrafico.EMIGRATO

                    End If

                ElseIf (com_immig <> String.Empty And com_emig = String.Empty) Then
                    ' Controllo immigrazione
                    ' assegno lo stato immigrato solo se la asl di immigrazione è vuota o diversa da quella di lavoro
                    If (asl_immig = String.Empty Or (asl_immig <> String.Empty And asl_immig <> _settings.CODICE_ASL)) Then

                        Return StatoAnagrafico.IMMIGRATO

                    End If
                End If

            Else

                ' Almeno una delle due date è vuota.

                ' --- Emigrato --- '
                ' Il paziente è EMIGRATO se:
                ' 1- la data di emigrazione è più recente rispetto a quella di immigrazione, e
                ' 2- la asl di emigrazione è nulla oppure diversa dalla asl di lavoro (comune di emigrazione fuori asl)
                If (data_emig > data_immig) And (asl_emig = String.Empty Or (asl_emig <> String.Empty And asl_emig <> _settings.CODICE_ASL)) Then

                    Return StatoAnagrafico.EMIGRATO

                End If

                ' --- Immigrato --- '
                ' Il paziente è IMMIGRATO se:
                ' 1- la data di immigrazione è più recente rispetto a quella di emigrazione, e
                ' 2- la asl di immigrazione è nulla oppure diversa dalla asl di lavoro (comune di immigrazione fuori asl)
                If (data_immig > data_emig) And (asl_immig = String.Empty Or (asl_immig <> String.Empty And asl_immig <> _settings.CODICE_ASL)) Then

                    Return StatoAnagrafico.IMMIGRATO

                End If

            End If

            ' Ricerca asl di residenza
            Dim asl_com_res As String = String.Empty
            If com_res <> String.Empty Then
                asl_com_res = _dal.GetCodiceAslByCodiceComune(com_res)
            End If

            ' Ricerca asl di domicilio
            Dim asl_com_dom As String = String.Empty
            If com_dom <> String.Empty Then
                asl_com_dom = _dal.GetCodiceAslByCodiceComune(com_dom)
            End If

            ' Asl di assistenza
            Dim asl_assistenza As String = dati_paz.DatiSanitari.USLCodiceAssistenza

            ' --- Residente domiciliato fuori usl --- '
            ' Il paziente è RESIDENTE DOMICILIATO FUORI USL se:
            ' 1- la asl relativa al comune di residenza è valorizzata ed è uguale alla asl di lavoro, e
            ' 2- la asl di assistenza è valorizzata e diversa dalla asl di lavoro, oppure 
            '    la asl di assistenza è nulla, la asl associata al comune di domicilio è valorizzata ed è diversa dalla asl di lavoro
            If (asl_com_res <> String.Empty And asl_com_res = _settings.CODICE_ASL) And
               ((asl_assistenza <> String.Empty And asl_assistenza <> _settings.CODICE_ASL) Or
                (asl_assistenza = String.Empty And asl_com_dom <> String.Empty And asl_com_dom <> _settings.CODICE_ASL)) Then

                Return StatoAnagrafico.RESIDENTE_DOMICILIATO_FUORI_USL

            End If

            ' --- Residente --- '
            ' Il paziente è residente se:
            ' 1- la asl relativa al comune di residenza è valorizzata ed è uguale alla asl di lavoro, e
            ' 2- la asl relativa al comune di domicilio è nulla, oppure è valorizzata ed è uguale alla asl di lavoro, oppure
            '    la asl di assistenza è nulla, oppure è valorizzata ed è uguale alla asl corrente
            If (asl_com_res <> String.Empty And asl_com_res = _settings.CODICE_ASL) And
               ((asl_com_dom = String.Empty Or (asl_com_dom <> String.Empty And asl_com_dom = _settings.CODICE_ASL) Or
                (asl_assistenza = String.Empty Or (asl_assistenza <> String.Empty And asl_assistenza = _settings.CODICE_ASL)))) Then

                Return StatoAnagrafico.RESIDENTE

            End If

            '--- Domiciliato --- '
            ' Il paziente è DOMICILIATO se:
            ' 1- la asl relativa al comune di residenza è nulla, oppure è valorizzata e diversa dalla asl di lavoro, e
            ' 2- la asl relativa al domicilio è valorizzata e uguale alla asl di lavoro, oppure
            '    la asl di assistenza è valorizzata ed uguale alla asl di lavoro
            If (asl_com_res = String.Empty Or (asl_com_res <> String.Empty And asl_com_res <> _settings.CODICE_ASL)) And
               ((asl_com_dom <> String.Empty And asl_com_dom = _settings.CODICE_ASL) Or
                (asl_assistenza <> String.Empty And asl_assistenza = _settings.CODICE_ASL)) Then

                Return StatoAnagrafico.DOMICILIATO

            End If

            ' --- Non residente non domiciliato --- '
            ' Se tutti i controlli precedenti non sono andati a buon fine, il paziente non è nè residente nè domiciliato.
            Return StatoAnagrafico.NON_RESIDENTE_NON_DOMICILIATO

        End Function

        ' Calcolo del consultorio vaccinale. Il consultorio viene calcolato in questo ordine:
        ' 1- cerco il consultorio in base al pediatra vaccinatore
        ' 2- se non lo trovo, lo cerco in base alla circoscrizione
        ' 3- se non lo trovo, lo cerco in base al comune di residenza
        ' 4- se non lo trovo, lo cerco in base al comune di domicilio
        ' 5- se non ho trovato niente, assegno il consultorio di smistamento, se c'è. Altrimenti viene restituita la stringa vuota
        ' In tutti i casi, viene filtrata anche l'età del paziente e la validità del consultorio.
        Private Function CalcolaCnsVaccinale(msg As Onit.OnAssistnet.Contracts.Messaggio, ByVal com_res As String, ByVal com_dom As String) As String

            Dim cns_vacc As String = String.Empty
            Dim eta_paz As Integer = 0

            Dim dati_paz As Onit.OnAssistnet.Contracts.Paziente
            dati_paz = msg.Pazienti(0)

            ' Calcolo età paziente
            eta_paz = Date.Now.Subtract(dati_paz.DatiAnagrafici.DataNascita).TotalDays \ 365

            ' --- CNS by pediatra vaccinatore --- '
            ' Se il paziente non ha nessun medico associato, è inutile cercare il consultorio in base al pediatra
            If (dati_paz.DatiAnagrafici.CodiceMedicoBase <> String.Empty) Then
                cns_vacc = _genericprovider.Consultori.GetCnsByPediatra(dati_paz.DatiAnagrafici.CodiceMedicoBase, eta_paz)
                If cns_vacc <> String.Empty Then Return cns_vacc
            End If

            ' --- CNS by circoscrizione --- '
            If (dati_paz.DatiSanitari.Distretto <> String.Empty) Then
                cns_vacc = _genericprovider.Consultori.GetCnsByCircoscrizione(dati_paz.DatiSanitari.Distretto, eta_paz)
                If (cns_vacc <> String.Empty) Then Return cns_vacc
            End If

            ' --- CNS by comune di residenza --- '
            If (com_res <> String.Empty) Then
                cns_vacc = _genericprovider.Consultori.GetCnsByComune(com_res, eta_paz)
                If (cns_vacc <> String.Empty) Then Return cns_vacc
            End If

            ' --- CNS by comune di domicilio --- '
            If (com_dom <> String.Empty) Then
                cns_vacc = _genericprovider.Consultori.GetCnsByComune(com_dom, eta_paz)
                If (cns_vacc <> String.Empty) Then Return cns_vacc
            End If

            ' --- CNS di smistamento --- '
            cns_vacc = _genericprovider.Consultori.GetCnsSmistamento(eta_paz)

            Return cns_vacc

        End Function

        ' Calcolo del consultorio territoriale. Il consultorio viene calcolato in questo ordine:
        ' 1- cerco il consultorio in base alla circoscrizione
        ' 2- se non lo trovo, lo cerco in base al comune di residenza
        ' In tutti gli altri casi viene restituita la stringa vuota.
        ' Vengono sempre filtrate anche l'età del paziente e la validità del consultorio.
        Private Function CalcolaCnsTerritoriale(ByVal msg As Onit.OnAssistnet.Contracts.Messaggio, ByVal com_res As String) As String

            Dim cns_terr As String = String.Empty
            Dim eta_paz As Integer = 0

            Dim dati_paz As Onit.OnAssistnet.Contracts.Paziente
            dati_paz = msg.Pazienti(0)

            ' Calcolo età paziente
            eta_paz = Date.Now.Subtract(dati_paz.DatiAnagrafici.DataNascita).TotalDays \ 365

            ' --- CNS by circoscrizione --- '
            If (dati_paz.DatiSanitari.Distretto <> String.Empty) Then
                cns_terr = _genericprovider.Consultori.GetCnsByCircoscrizione(dati_paz.DatiSanitari.Distretto, eta_paz)
                If (cns_terr <> String.Empty) Then Return cns_terr
            End If

            ' --- CNS by comune di residenza --- '
            If (com_res <> String.Empty) Then
                cns_terr = _genericprovider.Consultori.GetCnsByComune(com_res, eta_paz)
                If (cns_terr <> String.Empty) Then Return cns_terr
            End If

            Return String.Empty

        End Function

        ' Restituisce il valore che andrà impostato nel campo paz_regolarizzato (S/N).
        ' Se restituisce la stringa vuota, significa che il valore non deve essere modificato.
        Private Function CalcolaFlagRegolarizzato(ByVal msg As Onit.OnAssistnet.Contracts.Messaggio, ByVal cod_paziente As Integer) As String

            Dim paz_regolarizzato As String

            Dim dati_paz As Onit.OnAssistnet.Contracts.Paziente
            dati_paz = msg.Pazienti(0)

            If (cod_paziente = -1) Then

                ' In caso di inserimento

                ' Calcolo giorni di età del paziente
                Dim gg_eta_paz As Integer = 0
                gg_eta_paz = Date.Now.Subtract(dati_paz.DatiAnagrafici.DataNascita).TotalDays

                If (gg_eta_paz > _settings.NUM_GIORNI_REGOLARIZZAZIONE) Then
                    ' Se il paziente ha un'età superiore al parametro, controllo se è residente non immigrato.

                    ' Comune immigrazione del paziente
                    Dim com_immig As String = String.Empty
                    For i As Int16 = 0 To dati_paz.DatiAnagrafici.Indirizzi.Length - 1
                        If dati_paz.DatiAnagrafici.Indirizzi(i).TipoCodificato = Onit.OnAssistnet.Contracts.TipoCodificatoIndirizzo.Foreign Then
                            ' Immigrazione
                            com_immig = dati_paz.DatiAnagrafici.Indirizzi(i).ComuneCodiceXMPI
                            Exit For
                        End If
                    Next

                    Dim stato_anag_residente As Integer = Enumerators.StatoAnagrafico.RESIDENTE
                    If (dati_paz.DatiAnagrafici.StatoAnagrafico = stato_anag_residente.ToString) And _
                       (com_immig = String.Empty) Then
                        paz_regolarizzato = "S"
                    Else
                        paz_regolarizzato = "N"
                    End If

                Else
                    ' Se il paziente ha un'età inferiore al parametro, è regolarizzato di default.
                    paz_regolarizzato = "S"
                End If

            Else

                ' In caso di modifica

                ' Ricerca data e comune immigrazione (nuovi dati, ricevuti nel messaggio)
                Dim data_immig_new As Date = Date.MinValue
                Dim com_immig_new As String = String.Empty

                For i As Int16 = 0 To dati_paz.DatiAnagrafici.Indirizzi.Length - 1
                    If dati_paz.DatiAnagrafici.Indirizzi(i).TipoCodificato = Onit.OnAssistnet.Contracts.TipoCodificatoIndirizzo.Foreign Then
                        ' Immigrazione
                        com_immig_new = dati_paz.DatiAnagrafici.Indirizzi(i).ComuneCodiceXMPI
                        If (dati_paz.DatiAnagrafici.DataImmigrazioneSpecified) Then
                            data_immig_new = dati_paz.DatiAnagrafici.DataImmigrazione
                        Else
                            data_immig_new = dati_paz.DatiAnagrafici.Indirizzi(i).DataInizio
                        End If
                        Exit For
                    End If
                Next

                ' Ricerca data e comune di immigrazione (vecchi dati, su db)
                Dim data_immig_old As Date = Date.MinValue
                Dim com_immig_old As String = String.Empty

                data_immig_old = _genericprovider.Paziente.GetDataImmigrazione(cod_paziente)
                com_immig_old = _genericprovider.Paziente.GetCodiceComuneImmigrazione(cod_paziente)


                If (com_immig_old <> String.Empty And com_immig_new <> String.Empty And com_immig_old <> com_immig_new) Or _
                   (data_immig_old <> Date.MinValue And data_immig_new <> Date.MinValue And data_immig_new > data_immig_old) Then

                    paz_regolarizzato = "N"
                Else
                    ' Il valore va lasciato invariato
                    paz_regolarizzato = String.Empty
                End If

            End If

            Return paz_regolarizzato

        End Function

#End Region

#Region " IDisposable "

        Public Sub Dispose() Implements System.IDisposable.Dispose
            If Not _dal Is Nothing Then
                _dal.Dispose()
            End If
            If Not _genericprovider Is Nothing Then
                _genericprovider.Dispose()
            End If
        End Sub

#End Region

    End Class

End Namespace