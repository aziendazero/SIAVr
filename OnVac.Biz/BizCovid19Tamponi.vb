Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities
Imports Microsoft.Ajax.Utilities

Public Class BizCovid19Tamponi
    Inherits BizClass

#Region " Constructors "
    Public Sub New(genericProvider As DbGenericProvider, settings As Settings.Settings, contextInfo As BizContextInfos)

        MyBase.New(genericProvider, settings, contextInfo, Nothing)

    End Sub
#End Region

    ''' <summary>
    ''' Restituisce i tamponi fatti da un paziente
    ''' </summary>
    ''' <param name="Id">IdPaziente, stringa</param>
    ''' <returns></returns>
    Public Function GetTamponiById(Id As Long) As List(Of TamponeDiFrontiera)
        Return GenericProvider.Covid19Tamponi.GetTamponiById(Id)
    End Function
    ''' <summary>
    ''' Restituisce tutti tamponi in stato da elaborare
    ''' </summary>
    ''' <returns></returns>
    Public Function GetTamponiFrontiera() As List(Of TamponeDiFrontiera)
        Return GenericProvider.Covid19Tamponi.GetTamponiFrontiera()
    End Function
    Public Function GetTamponiFrontieraPerPazientiNonIdentificati() As List(Of TamponeDiFrontiera)
        Return GenericProvider.Covid19Tamponi.GetTamponiFrontieraPerPazientiNonIdentificati()
    End Function

    ''' <summary>
    ''' Aggiorna il tampone portando lo stato ad 12 e segnando il tipo di errore
    ''' </summary>
    ''' <param name="errore"></param>
    ''' <param name="IdTampone"></param>
    ''' <param name="pazCodice"></param>
    ''' <param name="dataElaborazione"></param>
    ''' <returns></returns>
    Public Function UpdateError(errore As String, IdTampone As Integer, pazCodice As Long, dataElaborazione As DateTime)
        Return GenericProvider.Covid19Tamponi.UpdateError(errore, IdTampone, pazCodice, dataElaborazione)
    End Function
    Public Function UpdateErrorNoPaz(errore As String, IdTampone As Integer, dataElaborazione As DateTime)
        Return GenericProvider.Covid19Tamponi.UpdateErrorNoPaz(errore, IdTampone, dataElaborazione)
    End Function
    ''' <summary>
    ''' Aggiorna lo stato di elaborazione del tampone dato l'id del tampone
    ''' </summary>
    ''' <param name="idTampone"></param>
    ''' <param name="statoElab"></param>
    ''' <param name="dataElaborazione"></param>
    ''' <returns></returns>
    Public Function UpdateStatoElab(idTampone As Integer, statoElab As Integer, pazCodice As Long, dataElaborazione As DateTime)
        GenericProvider.Covid19Tamponi.UpdateStatoElab(idTampone, statoElab, pazCodice, dataElaborazione)
    End Function
    ''' <summary>
    ''' Aggiorna lo stato di elaborazione del tampone aggiungendo anche un messaggio e aggiornando la data di guarigione, si utilizza solo se il paziente deve risultare guarito
    ''' </summary>
    ''' <param name="idTampone"></param>
    ''' <param name="statoElab"></param>
    ''' <param name="data">data di guarigione</param>
    ''' <param name="messaggio">messaggio di annotazione</param>
    ''' <param name="dataElaborazione"></param>
    ''' <returns></returns>
    Public Function UpdateStatoElab(idTampone As Integer, statoElab As Integer, data As DateTime, messaggio As String, pazCodice As Long, dataElaborazione As DateTime)
        GenericProvider.Covid19Tamponi.UpdateStatoElab(idTampone, statoElab, data, messaggio, pazCodice, dataElaborazione)
    End Function
    ''' <summary>
    ''' Aggiorna lo stato di elaborazione del tampone dato l'id del tampone, va a modificare lo stato elaborazione il messaggio e la data di elaborazione
    ''' </summary>
    ''' <param name="idTampone"></param>
    ''' <param name="statoElab"></param>
    ''' <param name="messaggio"></param>
    ''' <param name="dataElaborazione"></param>
    ''' <returns></returns>
    Public Function UpdateStatoElabSDG(idTampone As Integer, statoElab As Integer, messaggio As String, pazCodice As Long, dataElaborazione As DateTime)
        GenericProvider.Covid19Tamponi.UpdateStatoElabSDG(idTampone, statoElab, messaggio, pazCodice, dataElaborazione)
    End Function

    Public Function UpdateStatoElab(idTampone As Integer, statoElab As Integer, messaggio As String, pazCodice As Long, dataElaborazione As DateTime)
        GenericProvider.Covid19Tamponi.UpdateStatoElab2(idTampone, statoElab, messaggio, pazCodice, dataElaborazione)
    End Function
    Public Function GetAllTamponiById(Id As Long) As List(Of TamponeDatiRidotti)
        Dim elenco As List(Of TamponeDatiRidotti) = GenericProvider.Covid19Tamponi.GetAllTamponiById(Id)

        Dim campioneNullo As IEnumerable(Of TamponeDatiRidotti) = elenco.Where(Function(el)
                                                                                   Return String.IsNullOrWhiteSpace(el.CodiceCampione)
                                                                               End Function)

        Dim campione As IEnumerable(Of TamponeDatiRidotti) = elenco.Where(Function(el)
                                                                              Return Not String.IsNullOrWhiteSpace(el.CodiceCampione)
                                                                          End Function).DistinctBy(Function(el)
                                                                                                       Return el.CodiceCampione
                                                                                                   End Function)

        Dim ritorno As New List(Of TamponeDatiRidotti)
        ritorno.AddRange(campioneNullo)
        ritorno.AddRange(campione)
        Return ritorno
    End Function

    Public Function ContaTamponiPaziente(id As Long) As Integer
        Return GenericProvider.Covid19Tamponi.ContaTamponiPaziente(id)
    End Function

    Public Function GetTamponiOrfani(ricerca As RicercaTamponiOrfani) As List(Of TamponeOrfano)
        Dim s As Integer
        If Not ricerca.Skip.HasValue Then
            ricerca.Skip = 0
        End If
        Return GenericProvider.Covid19Tamponi.GetTamponiOrfani(ricerca)
    End Function

    Public Sub UpdateTamponiOrfaniCovid(idTampone As Long, idPaziente As Long, note As String, ulss As String)

        Dim paziente As PazienteDatiAnagrafici = GenericProvider.Paziente.GetDatiAnagraficiPaziente(idPaziente)

        GenericProvider.Covid19Tamponi.UpdateTamponeOrfano(idTampone, paziente.CodiceFiscale, idPaziente, DateTime.Now, note, ulss)

    End Sub

    Public Sub UpdateUlssTamponeOrfano(idTampone As Long, ulss As String)
        GenericProvider.Covid19Tamponi.UpdateUlssTamponeOrfanp(idTampone, ulss)
    End Sub

    Public Function ElencoEsitiOrfani(q As String) As IEnumerable(Of String)
        Return GenericProvider.Covid19Tamponi.ElencoEsitiOrfani(q)
    End Function

#Region " Mapping AURA - ONVAC "

    ' TODO [QPv2]: questa entity andrebbe spostata nell'ObjectModel assieme alle altre Entity comuni

    <Serializable>
    Public Class PazienteAura

        Public Property mpi As String
        Public Property fiscalCode As String
        Public Property stp As String
        Public Property stpDateBegin As Date?
        Public Property stpDateEnd As Date?
        Public Property cs As String
        Public Property csRegion As String
        Public Property csDateBegin As Date?
        Public Property csDateEnd As Date?
        Public Property eni As String
        Public Property eniDateBegin As Date?
        Public Property eniDateEnd As Date?
        Public Property teamPers As String
        Public Property teamInst As String
        Public Property teamDateEnd As Date?
        Public Property teamIdent As String
        Public Property teamCode As String
        Public Property nameFam As String
        Public Property nameGiv As String
        Public Property birthTime As Date
        Public Property genderCode As String
        Public Property birthplaceCode As String
        Public Property countryOfBirth As String
        Public Property addrStr As String
        Public Property addrBnr As String
        Public Property addrCode As String
        Public Property addrZip As String
        Public Property countryOfAddr As String
        Public Property domAddrStr As String
        Public Property domAddrBnr As String
        Public Property domAddrCode As String
        Public Property domAddrZip As String
        Public Property countryOfDom As String
        Public Property telecomH As String
        Public Property telecomHp As String
        Public Property telecomBad As String
        Public Property telecomMc As String
        Public Property telecomMail As String
        Public Property telecomTmp As String
        Public Property telecomEc As String
        Public Property telecomPg As String
        Public Property deathDate As Date?
        Public Property reliability As String
        Public Property mmgRegionalCode As String
        Public Property mmgNameFam As String
        Public Property mmgNameGiv As String
        Public Property mmgDateBegin As Date?
        Public Property mmgDateEnd As Date?
        Public Property istatUlssAss As String
        Public Property citizenship As String
        Public Property category As String

    End Class

    Public Function InserimentoPazienteCentraleLocale(pazienteAura As PazienteAura) As BizGenericResult

        Dim result As New BizGenericResult()

        ' TODO [COV - mapping paz AURV-VAC]: log inserimento paz?

        Dim pazienteVac As Paziente = MappingPazienteAuraToPazienteVac(pazienteAura)

        Try
            Using pazienteBiz As BizPaziente = BizFactory.Instance.CreateBizPaziente(GenericProvider, Settings, ContextInfos, Nothing)

                Dim insertResult As BizResult = pazienteBiz.InserisciPaziente(
                    New BizPaziente.InserisciPazienteCommand() With {
                        .Paziente = pazienteVac,
                        .FromVAC = False,
                        .ForzaInserimento = True
                    })

                result.Success = insertResult.Success

                If insertResult.Messages IsNot Nothing AndAlso insertResult.Messages.Count > 0 Then

                    result.Message = insertResult.Messages.GetDescriptions().Aggregate(Function(p, g) p & ", " & g)

                End If

            End Using

        Catch ex As Exception
            result.Success = False
            result.Message = ex.Message
            Common.Utility.EventLogHelper.EventLogWrite(ex, "BizCovid19Tamponi.InserimentoPazienteCentraleLocale - " + ContextInfos.IDApplicazione, EventLogEntryType.Information, "OnVac")
        End Try

        Return result

    End Function

    ''' <summary>
    ''' Data un'entità con i dati provenienti da AURA, restituisce l'entità Paziente di OnVac, con i dati rimappati.
    ''' </summary>
    ''' <param name="pazienteAura"></param>
    ''' <returns></returns>
    Public Function MappingPazienteAuraToPazienteVac(pazienteAura As PazienteAura) As Paziente

        Dim pazienteVac As New Paziente()

        pazienteVac.DataInserimento = DateTime.Now
        pazienteVac.CodiceAzienda = ContextInfos.CodiceAzienda

        pazienteVac.Stato = Enumerators.StatiVaccinali.InCorso
        pazienteVac.FlagLocale = False

        ' N.B. : pazienteVac.CodiceAusiliario  => calcolato nel momento in cui il paziente viene inserito in centrale

        pazienteVac.PAZ_CODICE_REGIONALE = pazienteAura.mpi

        pazienteVac.PAZ_COGNOME = pazienteAura.nameFam
        pazienteVac.PAZ_NOME = pazienteAura.nameGiv
        pazienteVac.Sesso = pazienteAura.genderCode
        pazienteVac.Cittadinanza_Codice = If(pazienteAura.citizenship, Settings.CITTADINANZA_SCONOSCIUTA)
        pazienteVac.CategoriaCittadino = pazienteAura.category
        pazienteVac.Data_Nascita = pazienteAura.birthTime

        If pazienteAura.deathDate.HasValue Then
            pazienteVac.DataDecesso = pazienteAura.deathDate
        End If

        ' N.B. : qui viene ripetuta la stessa logica del mapping del mid (il codice ricevuto è il codice ISTAT)
        If String.IsNullOrWhiteSpace(pazienteAura.citizenship) OrElse pazienteAura.citizenship <> Settings.CITTADINANZA_DEFAULT Then

            ' Pazienti stranieri o senza cittadinanza specificata, il calcolo avviene in base alla data di oggi
            pazienteVac.ComuneNascita_Codice = GenericProvider.Comuni.GetCodiceComuneByCodiceIstat(pazienteAura.birthplaceCode, Date.Today)

        Else

            ' Pazienti italiani: il calcolo avviene in base alla data di nascita
            If String.IsNullOrWhiteSpace(pazienteAura.birthplaceCode) Then
                pazienteVac.ComuneNascita_Codice = String.Empty
            Else
                pazienteVac.ComuneNascita_Codice = GenericProvider.Comuni.GetCodiceComuneByCodiceIstat(pazienteAura.birthplaceCode, pazienteAura.birthTime)
            End If

        End If

        ' Residenza
        If String.IsNullOrWhiteSpace(pazienteAura.addrCode) Then
            pazienteVac.ComuneResidenza_Codice = String.Empty
        Else
            pazienteVac.ComuneResidenza_Codice = GenericProvider.Comuni.GetCodiceComuneByCodiceIstat(pazienteAura.addrCode, Date.Today)
        End If

        If Not String.IsNullOrWhiteSpace(pazienteAura.addrStr) Then
            pazienteVac.IndirizzoResidenza = String.Format("{0}, {1}", pazienteAura.addrStr, pazienteAura.addrBnr)
        End If

        pazienteVac.ComuneResidenza_Cap = pazienteAura.addrZip

        ' Domicilio
        If String.IsNullOrWhiteSpace(pazienteAura.domAddrCode) Then
            pazienteVac.ComuneDomicilio_Codice = String.Empty
        Else
            pazienteVac.ComuneDomicilio_Codice = GenericProvider.Comuni.GetCodiceComuneByCodiceIstat(pazienteAura.domAddrCode, Date.Today)
        End If

        If Not String.IsNullOrWhiteSpace(pazienteAura.domAddrStr) Then
            pazienteVac.IndirizzoDomicilio = String.Format("{0}, {1}", pazienteAura.domAddrStr, pazienteAura.domAddrBnr)
        End If

        pazienteVac.ComuneDomicilio_Cap = pazienteAura.domAddrZip

        ' Codice fiscale
        If Not String.IsNullOrWhiteSpace(pazienteAura.fiscalCode) Then

            pazienteVac.PAZ_CODICE_FISCALE = pazienteAura.fiscalCode

        ElseIf Not String.IsNullOrWhiteSpace(pazienteAura.stp) Then

            pazienteVac.PAZ_CODICE_FISCALE = pazienteAura.stp

        ElseIf Not String.IsNullOrWhiteSpace(pazienteAura.eni) Then

            pazienteVac.PAZ_CODICE_FISCALE = pazienteAura.eni

        End If

        ' Tessera sanitaria
        pazienteVac.Tessera = pazienteAura.cs

        ' Telefoni e Mail
        pazienteVac.Telefono1 = pazienteAura.telecomHp
        pazienteVac.Telefono2 = pazienteAura.telecomMc
        pazienteVac.Email = pazienteAura.telecomMail

        ' Usl
        pazienteVac.UslAssistenza_Codice = pazienteAura.istatUlssAss
        ' N.B. : pazienteVac.UslResidenza_Codice => nelle NHApi lo leggiamo direttamente dal messaggio, qui non c'è quindi non lo valorizzo

        ' Medico di base (se non presente deve essere inserito in anagrafica)
        If Not String.IsNullOrWhiteSpace(pazienteAura.mmgRegionalCode) Then

            If Not GenericProvider.Medico.ExistsMedico(pazienteAura.mmgRegionalCode) Then

                GenericProvider.Medico.InsertMedico(
                    New Medico() With {
                        .Codice = pazienteAura.mmgRegionalCode,
                        .CodiceRegionale = pazienteAura.mmgRegionalCode,
                        .Nome = pazienteAura.mmgNameGiv,
                        .Cognome = pazienteAura.mmgNameFam,
                        .Descrizione = String.Format("{0}*{1}", pazienteAura.mmgNameFam, pazienteAura.mmgNameGiv)
                    })

            End If

            pazienteVac.MedicoBase_Codice = pazienteAura.mmgRegionalCode
            If pazienteAura.mmgDateBegin.HasValue Then
                pazienteVac.MedicoBase_DataDecorrenza = pazienteAura.mmgDateBegin.Value
            End If
            If pazienteAura.mmgDateEnd.HasValue Then
                pazienteVac.MedicoBase_DataScadenza = pazienteAura.mmgDateEnd.Value
            End If

        End If

        Return pazienteVac

    End Function

#End Region

End Class
