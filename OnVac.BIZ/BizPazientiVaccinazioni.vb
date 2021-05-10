Imports System.Collections.Generic

Imports Onit.OnAssistnet.OnVac.DAL


Public Class BizPazientiVaccinazioni
    Inherits BizClass

#Region " Constructors "

    Public Sub New(ByVal genericprovider As DbGenericProvider, ByVal settings As Onit.OnAssistnet.OnVac.Settings.Settings, ByVal contextInfos As BizContextInfos, ByVal logOptions As BizLogOptions)

        MyBase.New(genericprovider, settings, contextInfos, logOptions)

    End Sub

#End Region

#Region " Public "

    ''' <summary>
    ''' Restituisce il dataset da passare al report AvvisoConvocati.rpt, in base
    ''' all'hashtable contenente i pazienti e le vaccinazioni associate.
    ''' </summary>
    ''' <param name="hVacPazSelezionati"></param>
    ''' <param name="cnsCodice"></param>
    Public Function CreateDtsAvvisoConvocatiSelezionati(hVacPazSelezionati As Hashtable, cnsCodice As String) As dtsAvvisoConvocati

        ' --- Filtro pazienti selezionati --- '
        Dim filtro_pazienti As New System.Text.StringBuilder
        Dim lPazSel As New ArrayList
        Dim _key, _val As String
        Dim el() As String

        Dim idict As IDictionaryEnumerator = hVacPazSelezionati.GetEnumerator()

        While (idict.MoveNext())

            ' Formato elemento: "codice_paziente*data_convocazione"
            el = Split(idict.Key.ToString, "*")

            ' Codice del paziente
            _key = el(0)

            ' Vaccinazioni
            _val = idict.Value.ToString

            If (Not lPazSel.Contains(_key)) Then

                ' Filtro i pazienti in modo da eliminare eventuali doppioni
                lPazSel.Add(_key)

                ' Stringa con i codici dei pazienti (campo numerico)
                filtro_pazienti.AppendFormat("{0},", _key)

            End If

        End While

        If (filtro_pazienti.Length = 0) Then
            ERRORMESSAGE = "Nessun paziente selezionato."
            Return Nothing
        End If
        filtro_pazienti.Remove(filtro_pazienti.Length - 1, 1)

        ' --- Caricamento pazienti selezionati --- '
        Dim coll_paz As Collection.PazienteCollection
        Try
            Using bizPaziente As New Biz.BizPaziente(Me.GenericProvider, Me.Settings, Me.ContextInfos, Me.LogOptions)
                coll_paz = bizPaziente.CercaPazienti(filtro_pazienti.ToString)
            End Using
        Catch ex As Exception
            ' In caso di errore visualizzo un msg e non continuo l'elaborazione
            ERRORMESSAGE = "Errore durante il caricamento dei pazienti selezionati:" + vbNewLine + ex.Message.Replace(vbNewLine, "")
            Return Nothing
        End Try

        ' --- Caricamento dati consultorio corrente --- '
        Dim dati_cns As Entities.Cns = Nothing
        Try
            dati_cns = GenericProvider.Consultori.GetConsultorio(cnsCodice)
        Catch ex As Exception
            ERRORMESSAGE = "Errore durante il caricamento dei dati del consultorio corrente:" + vbNewLine + ex.Message
            Return Nothing
        End Try

        ' --- Caricamento dati vaccinazioni --- '
        Dim coll_paz_vac As Collection.PazientiVaccinazioniCollection = Nothing
        Try
            coll_paz_vac = createCollection(hVacPazSelezionati)
        Catch ex As Exception
            ERRORMESSAGE = "Errore durante il caricamento dei dati delle vaccinazioni dei pazienti." + vbNewLine + ex.Message
            Return Nothing
        End Try

        If (coll_paz_vac.Count = 0) Then
            ERRORMESSAGE = "Attenzione: nessun dato da stampare. Il report Elenco Convocati Selezionati non verrà creato."
            Return Nothing
        End If

        ' --- Creazione dataset da passare al report --- '
        Dim dtsPazVac As dtsAvvisoConvocati = Nothing
        Try
            dtsPazVac = CreateDataset(coll_paz, coll_paz_vac, dati_cns)
            If (dtsPazVac Is Nothing OrElse dtsPazVac.Convocati.Rows.Count = 0) Then
                ERRORMESSAGE = "Attenzione: nessun dato da stampare. Il report Elenco Convocati Selezionati non verrà creato"
                Return Nothing
            End If
        Catch ex As Exception
            ERRORMESSAGE = "Errore durante il caricamento dei dati da passare al report Elenco Convocati Selezionati." + vbNewLine + ex.Message
            Return Nothing
        End Try

        Return dtsPazVac

    End Function

#End Region

#Region " Private "

    ''' <summary>
    ''' Dato un hashtable ("codice_paziente*data_cnv", stringa vaccinazioni-dosi) restituisce 
    ''' una collection con oggetti di tipo PazientiVaccinazioni contenenti, per ogni 
    ''' paziente, un hashtable con codice e descrizione delle vaccinazioni relative
    ''' al paziente e uno con codice e dose associata. Ci sarà un oggetto di questo tipo per 
    ''' ogni convocazione di ogni paziente nell'hashtable.
    ''' </summary>
    ''' <param name="hPazientiVaccinazioni"></param>
    Private Function createCollection(hPazientiVaccinazioni As Hashtable) As Collection.PazientiVaccinazioniCollection

        Dim collPazVac As New Collection.PazientiVaccinazioniCollection
        Dim p As Entities.PazientiVaccinazioni

        ' hPazientiVaccinazioni contiene una stringa codice paziente concatenato con data convocazione (separati da *) 
        ' come chiave e una stringa con codice di ogni vaccinazione e dose associata (tra parentesi) come valore.
        ' Ad es. "DIF(5), HA(2), MAN(4)". Suddivido questa stringa in 2 array, uno con le vaccinazioni e uno con le dosi.
        ' Poi inserisco tutte le vac distinte di tutti i paz in un array per effettuare un'unica ricerca per ottenere le descrizioni.
        ' Queste operazioni le faccio non solo per ogni paziente ma anche per ogni convocazione distinta (modifica 02.12.2008)
        Dim idict As IDictionaryEnumerator = hPazientiVaccinazioni.GetEnumerator()

        Dim vacDosi As String()
        Dim element As String()

        Dim codiceVaccinazione As String

        ' Inserisco tutti i codici delle vaccinazioni (distinte) in una lista da passare alla query di select delle descrizioni
        Dim listVaccinazioniDaCaricare As New List(Of String)()

        Dim _key As String
        Dim el() As String

        ' Ciclo per ogni elemento dell'hashtable passata per parametro. 
        ' Ogni elemento rappresenta un paziente con i dati delle sue vaccinazioni per la convocazione specificata. 
        While idict.MoveNext()

            If idict.Value.ToString() <> String.Empty Then

                ' split del codice dalla data di convocazione
                el = Split(idict.Key.ToString(), "*")
                _key = el(0).ToString()

                ' Nuovo oggetto PazientiVaccinazioni (1 per ogni coppia paziente + convocazione)
                p = New Entities.PazientiVaccinazioni(_key)

                ' Separazione dei codici delle vaccinazioni e delle dosi associate
                vacDosi = idict.Value.ToString().Split(",")

                ' Ciclo per ogni vaccinazione del paziente
                For i As Integer = 0 To vacDosi.Length - 1

                    ' Elemento singolo contenente la vaccinazione e la dose associata
                    element = vacDosi(i).Split("(")

                    codiceVaccinazione = element(0).ToString.Trim

                    ' Aggiunta della vaccinazione e della dose nell'hashtable VaccinazioniDosi
                    p.VaccinazioniDosi.Add(codiceVaccinazione, element(1).ToString.Trim.Replace(")", ""))

                    ' Aggiunta della vaccinazione tra quelle da cercare, se non c'è già.
                    If Not listVaccinazioniDaCaricare.Contains(codiceVaccinazione) Then listVaccinazioniDaCaricare.Add(codiceVaccinazione)

                Next

                ' Aggiungo i dati del paziente alla collection che devo restituire
                collPazVac.Add(p)

            End If

        End While

        ' Caricamento descrizioni vaccinazioni
        If listVaccinazioniDaCaricare.Count > 0 Then

            Dim hashDatiVaccinazioni As Hashtable =
                Me.GenericProvider.PazientiVaccinazioni.LoadDescrizioneVaccinazioni(listVaccinazioniDaCaricare)

            ' Creazione hashtable VaccinazioneDescrizioni per ogni paziente
            Dim vacDict As IDictionaryEnumerator

            Dim paz_vac As Entities.PazientiVaccinazioni

            For idx As Integer = 0 To collPazVac.Count - 1

                paz_vac = collPazVac(idx)

                vacDict = paz_vac.VaccinazioniDosi.GetEnumerator()

                While (vacDict.MoveNext())
                    ' Aggiunta della vaccinazione nell'hashtable VaccinazioniDescrizioni, con la descrizione trovata
                    paz_vac.VaccinazioniDescrizioni.Add(vacDict.Key.ToString(), hashDatiVaccinazioni(vacDict.Key.ToString()))
                End While

            Next

        End If

        Return collPazVac

    End Function

    ''' <summary>
    ''' Restituisce il dataset da passare al report AvvisoConvocati.rpt.
    ''' Vanno specificate: la collection con i dati anagrafici dei pazienti, 
    ''' la collection con i dati delle vaccinazioni(codice, descrizione e dosi)
    ''' e la classe contenente i dati del consultorio.
    ''' </summary>
    ''' <param name="collPazienti"></param>
    ''' <param name="collPazVaccinazioni"></param>
    ''' <param name="datiConsultorio"></param>
    Private Function CreateDataset(collPazienti As Collection.PazienteCollection, collPazVaccinazioni As Collection.PazientiVaccinazioniCollection, datiConsultorio As Entities.Cns) As dtsAvvisoConvocati

        Dim dtsPazVac As New dtsAvvisoConvocati()

        Dim row_paz_vac As dtsAvvisoConvocati.ConvocatiRow
        Dim paz As Entities.Paziente
        Dim idict As IDictionaryEnumerator

        ' --- Costruzione dataset --- '

        ' Ciclo sui pazienti
        Dim paz_vac As Entities.PazientiVaccinazioni

        For idx As Integer = 0 To collPazVaccinazioni.Count - 1

            paz_vac = collPazVaccinazioni(idx)

            ' Cerco il paziente nella collection, in base al codice
            paz = collPazienti.FindByCodPaziente(paz_vac.CodicePaziente)

            If Not paz Is Nothing Then

                ' Ciclo sulle vaccinazioni
                idict = paz_vac.VaccinazioniDescrizioni.GetEnumerator()

                While idict.MoveNext()

                    ' Aggiungo una riga per ogni vaccinazione di ogni paziente
                    row_paz_vac = dtsPazVac.Convocati.NewRow()

                    ' --- Dati Anagrafici Paziente --- '
                    row_paz_vac.paz_codice = paz_vac.CodicePaziente
                    row_paz_vac.paz_cognome = paz.PAZ_COGNOME
                    row_paz_vac.paz_nome = paz.PAZ_NOME
                    row_paz_vac.paz_data_nascita = paz.Data_Nascita
                    row_paz_vac.paz_sesso = paz.Sesso
                    row_paz_vac.med_codice = paz.MedicoBase_Codice
                    row_paz_vac.med_descrizione = paz.MedicoBase_Descrizione
                    row_paz_vac.paz_indirizzo_residenza = paz.IndirizzoResidenza
                    row_paz_vac.res_com_codice = paz.ComuneResidenza_Codice
                    row_paz_vac.res_com_descrizione = paz.ComuneResidenza_Descrizione
                    row_paz_vac.res_com_provincia = paz.ComuneResidenza_Provincia
                    row_paz_vac.res_com_cap = paz.ComuneResidenza_Cap
                    row_paz_vac.paz_indirizzo_domicilio = paz.IndirizzoDomicilio
                    row_paz_vac.dom_com_codice = paz.ComuneDomicilio_Codice
                    row_paz_vac.dom_com_descrizione = paz.ComuneDomicilio_Descrizione
                    row_paz_vac.dom_com_provincia = paz.ComuneDomicilio_Provincia
                    row_paz_vac.dom_com_cap = paz.ComuneDomicilio_Cap

                    ' --- Dati Consultorio --- '
                    row_paz_vac.cns_codice = datiConsultorio.codice
                    row_paz_vac.cns_com_descrizione = datiConsultorio.comune
                    row_paz_vac.cns_descrizione = datiConsultorio.descrizione
                    row_paz_vac.cns_indirizzo = datiConsultorio.indirizzo
                    row_paz_vac.cns_n_telefono = datiConsultorio.Telefono
                    row_paz_vac.cns_stampa1 = datiConsultorio.Stampa1
                    row_paz_vac.cns_stampa2 = datiConsultorio.Stampa2
 
                    ' --- Dati Vaccinazione --- '
                    row_paz_vac.vac_codice = idict.Key.ToString
                    row_paz_vac.vac_descrizione = idict.Value.ToString
                    row_paz_vac.vac_dose = paz_vac.VaccinazioniDosi(idict.Key.ToString)

                    ' Aggiunta al dataset
                    dtsPazVac.Convocati.AddConvocatiRow(row_paz_vac)

                End While

            End If

        Next

        Return dtsPazVac

    End Function

#End Region

End Class
