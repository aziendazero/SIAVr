Imports System.Collections.Generic

Imports Onit.OnAssistnet.OnVac
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.DataSet
Imports Onit.OnAssistnet.OnVac.Entities

Public Class BizAppuntamentiGiorno
    Inherits BizClass

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, bizContextInfos As BizContextInfos)

        MyBase.New(genericprovider, settings, bizContextInfos, Nothing)

    End Sub

#End Region

#Region " Public "

    ''' <summary>Restituisce il dataset di tipo AppuntamentiGiornoBilanci (definito in AppuntamentiGiornoBilanciDS), utilizzato dal report ReportComuni\AppuntamentiDelGiorno.rpt (nuova versione)</summary>
    ''' <param name="codiceConsultorio">Codice del consultorio di cui stampare gli appuntamenti</param>
    ''' <param name="strDataInizio">Data di inizio del periodo di stampa degli appuntamenti</param>
    ''' <param name="strDataFine">Data di fine del periodo di stampa degli appuntamenti</param>
    ''' <param name="filtroPazientiAvvisati">Valori: SoloAvvisati (filtra solo paz con data invio valorizzata) - SoloNonAvvisati (filtra solo paz senza data invio) - Tutti (non filtra)</param>
    ''' <remarks> Restituisce nothing in caso di errore. E' necessario chiamare il metodo dispose di questo oggetto se il DAM è interno </remarks>
    Public Function CreaDataSetAppuntamentiGiornoBilanci(codiceConsultorio As String, strDataInizio As String, strDataFine As String, filtroPazientiAvvisati As Enumerators.FiltroAvvisati) As AppuntamentiGiornoBilanci

        Return Me.GenericProvider.AppuntamentiGiorno.BuildDataSetAppuntamentiGiornoBilanci(
            codiceConsultorio, ContextInfos.CodiceUsl, strDataInizio, strDataFine, filtroPazientiAvvisati)

    End Function

    ''' <summary>Restituisce il dataset di tipo AppuntamentiGiornoBilanci (definito in AppuntamentiGiornoBilanciDS), utilizzato dal report ReportComuni\AppuntamentiDelGiorno.rpt (nuova versione)</summary>
    ''' <param name="codiceConsultorio">Codice del consultorio di cui stampare gli appuntamenti</param>
    ''' <param name="codiceAmbulatorio"></param>
    ''' <param name="strDataInizio">Data di inizio del periodo di stampa degli appuntamenti (in formato stringa dd/MM/yyyy)</param>
    ''' <param name="strDataFine">Data di fine del periodo di stampa degli appuntamenti (in formato stringa dd/MM/yyyy)</param>
    ''' <param name="filtroPazientiAvvisati">Valori: SoloAvvisati (filtra solo paz con data invio valorizzata) - SoloNonAvvisati (filtra solo paz senza data invio) - Tutti (non filtra)</param>
    ''' <param name="filtroAssociazioniDosi">Filtro associazioni o dosi che devono essere presenti nella convocazioni (Nothing per non filtrare)</param>
    ''' <remarks> Restituisce nothing in caso di errore. E' necessario chiamare il metodo dispose di questo oggetto se il DAM è interno </remarks>
    Public Function CreaDataSetAppuntamentiGiornoBilanci(codiceConsultorio As String, codiceAmbulatorio As Integer, strDataInizio As String, strDataFine As String, filtroPazientiAvvisati As Enumerators.FiltroAvvisati, filtroAssociazioniDosi As Entities.FiltroComposto) As AppuntamentiGiornoBilanci

        Return Me.GenericProvider.AppuntamentiGiorno.BuildDataSetAppuntamentiGiornoBilanci(
            codiceConsultorio, codiceAmbulatorio, ContextInfos.CodiceUsl, strDataInizio, strDataFine, filtroPazientiAvvisati, filtroAssociazioniDosi)

    End Function

    ''' <summary>Restituisce il datatable contenente gli appuntamenti del giorno</summary>
    ''' <param name="codCns">Codice del consultorio in cui cercare gli appuntamenti</param>
    ''' <param name="codAmb"></param>
    ''' <param name="strData">Data del giorno in cui cercare gli appuntamenti</param>
    ''' <param name="strCampiOrdinamento"></param>
    ''' <remarks> Restituisce nothing in caso di errore. E' necessario chiamare il metodo dispose di questo oggetto se il DAM è interno </remarks>
    Public Function CercaAppuntamentiGiorno(codCns As String, codAmb As Integer, strData As String, strCampiOrdinamento As String) As DataTable

        Return Me.GenericProvider.AppuntamentiGiorno.FindAppuntamentiGiorno(codCns, codAmb, ContextInfos.CodiceUsl, strData, strCampiOrdinamento)

    End Function

    ''' <summary>Restituisce il datatable contenente vaccinazioni e dosi del paziente per la convocazione specificata</summary>
    ''' <param name="codicePaziente">Codice del paziente</param>
    ''' <param name="dataConvocazione">Data della convocazione</param>
    ''' <remarks> Restituisce nothing in caso di errore. E' necessario chiamare il metodo dispose di questo oggetto se il DAM è interno </remarks>
    Public Function GetVaccDosiPaziente(codicePaziente As Integer, dataConvocazione As Date) As DataTable

        Return Me.GenericProvider.AppuntamentiGiorno.GetVaccDosiPaziente(codicePaziente, dataConvocazione)

    End Function

#End Region

#Region " OnVac API "

    ''' <summary>
    ''' Restituisce la lista di appuntamenti per i pazienti specificati
    ''' </summary>
    ''' <param name="listCodiciPazienti"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetListAppuntamentiPazienti(listCodiciPazienti As List(Of Long)) As List(Of Entities.Appuntamento)

        Dim list As List(Of Entities.Appuntamento) =
            Me.GenericProvider.AppuntamentiGiorno.GetListAppuntamentiPazienti(listCodiciPazienti)

        For Each appuntamento As Entities.Appuntamento In list
            appuntamento.AppIdAziendaLocale = Me.ContextInfos.IDApplicazione
        Next

        Return list

    End Function

    ''' <summary>
    ''' Restituisce la lista di appuntamenti per il paziente in data specificata
    ''' </summary>
    ''' <param name="codicePaziente"></param>
    ''' <param name="dataAppuntamento"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetListAppuntamentiPazienteData(codicePaziente As Long, dataAppuntamento As DateTime) As List(Of Entities.Appuntamento)

        Dim list As List(Of Entities.Appuntamento) =
            Me.GenericProvider.AppuntamentiGiorno.GetListAppuntamentiPazienteData(codicePaziente, dataAppuntamento)

        For Each appuntamento As Entities.Appuntamento In list
            appuntamento.AppIdAziendaLocale = Me.ContextInfos.IDApplicazione
        Next

        Return list

    End Function
    Public Function GetListAppuntamentiPazientiByIdApi(idAppuntamento As Long) As List(Of Appuntamento)

        Dim list As List(Of Appuntamento) = GenericProvider.AppuntamentiGiorno.GetListAppuntamentiPazientiByIdApi(idAppuntamento)

        For Each appuntamento As Appuntamento In list
            appuntamento.AppIdAziendaLocale = ContextInfos.IDApplicazione
        Next

        Return list

    End Function
    Public Function GetListAppuntamentiPazientiApi(listCodiciPazienti As List(Of Long)) As List(Of Appuntamento)

        Dim list As List(Of Appuntamento) = GenericProvider.AppuntamentiGiorno.GetListAppuntamentiPazientiApi(listCodiciPazienti)

        For Each appuntamento As Appuntamento In list
            appuntamento.AppIdAziendaLocale = ContextInfos.IDApplicazione
        Next

        Return list

    End Function
    Public Function GetLisAppuntamentiPaziente(CodiceFiscale As String) As List(Of Entities.DTOAppuntamento)

        Dim c As ICollection(Of String) = Me.GenericProvider.Paziente.GetCodicePazientiByCodiceFiscale(CodiceFiscale)
        Dim Appuntamenti As New List(Of Entities.DTOAppuntamento)
        Dim appuntamento As New Entities.DTOAppuntamento

        If c.IsNullOrEmpty() Then
            appuntamento.ResultMessage = "Non è stato trovato nessun appuntamento"
            Appuntamenti.Add(appuntamento)
        ElseIf c.Count > 1 Then
            appuntamento.ResultMessage = "é stato trovato un duplicato di Codice Fiscale"
            Appuntamenti.Add(appuntamento)
        Else
            Dim l As New List(Of Long)()
            l.Add(Convert.ToInt64(c.Single()))

            Dim list As List(Of Entities.Appuntamento) =
                 GenericProvider.AppuntamentiGiorno.GetListAppuntamentiPazienti(l)


            For Each item As Entities.Appuntamento In list
                appuntamento = New Entities.DTOAppuntamento()
                appuntamento.IdAppuntamento = item.IdConvocazione
                appuntamento.CodiceVaccino = item.CodiceVaccinazione
                appuntamento.Data = item.DataAppuntamento
                appuntamento.DescrizioneVaccino = item.DescrizioneVaccinazione
                appuntamento.Luogo = item.DescrizioneConsultorio

                Appuntamenti.Add(appuntamento)
            Next

        End If


        Return Appuntamenti
    End Function

    Public Class GetAvvisoAppuntamentoCommand

        Public Property IdAppuntamento As String

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

    Public Function GetAvvisoAppuntamento(command As GetAvvisoAppuntamentoCommand) As Byte()

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
                Dim strFiltro As String = String.Empty

                ' TODO [CNV_ID_CONVOCAZIONE]: se servirà un'API per stampare l'avviso, aggiungere il campo CNV_ID_CONVOCAZIONE nella V_AVVISI e filtrare direttamente la vista
                Dim appuntamento As Entities.DTOAppuntamentoStampa = GenericProvider.AppuntamentiGiorno.GetDatiAppuntamentoStampa(command.IdAppuntamento)

                strFiltro = "{V_AVVISI.PAZ_CODICE}=" & appuntamento.CodicePaziente.ToString() &
                             " AND year({V_AVVISI.CNV_DATA_APPUNTAMENTO}) = " & appuntamento.DataAppuntamento.Year &
                             " AND month({V_AVVISI.CNV_DATA_APPUNTAMENTO}) = " & appuntamento.DataAppuntamento.Month &
                             " AND day({V_AVVISI.CNV_DATA_APPUNTAMENTO}) = " & appuntamento.DataAppuntamento.Day &
                             " AND hour({V_AVVISI.CNV_DATA_APPUNTAMENTO}) = " & appuntamento.DataAppuntamento.Hour &
                             " AND minute({V_AVVISI.CNV_DATA_APPUNTAMENTO}) = " & appuntamento.DataAppuntamento.Minute &
                             " AND year({V_AVVISI.CNV_DATA}) = " & appuntamento.DataConvocazione.Year &
                             " AND month({V_AVVISI.CNV_DATA}) = " & appuntamento.DataConvocazione.Month &
                             " AND day({V_AVVISI.CNV_DATA}) = " & appuntamento.DataConvocazione.Day

                rptDocument.RecordSelectionFormula = strFiltro
                AddParameter(rptDocument, "NoteAvviso", String.Empty)
                AddParameter(rptDocument, "Installazione", appuntamento.CodiceUsl)

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




    ''' <summary>
    ''' Restituisce tutti gli appuntamenti nell'intervallo di date (di appuntamento) specificato.
    ''' Viene eseguito sempre in centrale per recuperare tutti gli appuntamenti di tutte le aziende.
    ''' </summary>
    ''' <param name="dataAppuntamentoDa"></param>
    ''' <param name="dataAppuntamentoA"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAppuntamentiNotifica(dataAppuntamentoDa As DateTime, dataAppuntamentoA As DateTime) As List(Of Entities.AppuntamentoNotifica)

        Return Me.GenericProviderCentrale.AppuntamentiGiorno.GetAppuntamentiNotifica(dataAppuntamentoDa, dataAppuntamentoA)

    End Function

    Public Function ExistsAppuntamentoAmbulatorio(dataAppuntamento As Date, codiceAmbulatorio As Integer) As Boolean

        Return GenericProvider.AppuntamentiGiorno.ExistsAppuntamentoAmbulatorio(dataAppuntamento, codiceAmbulatorio)

    End Function
    Public Function ExistsAppuntamentoPaziente(dataAppuntamento As Date, codicePaziente As Long) As Boolean

        Return GenericProvider.AppuntamentiGiorno.ExistsAppuntamentoPaziente(dataAppuntamento, codicePaziente)

    End Function

    Public Function GetPrenotazioneAnnullabileEsterni(codicePaziente As Integer, data As DateTime) As String
        Return GenericProvider.AppuntamentiGiorno.GetPrenotazioneAnnullabileEsterni(codicePaziente, data)
    End Function

    Public Function SetRevocaAppuntamento(idAppuntamento As String, Note As String) As ResultSetPost
        Dim revoca As New DTORevoca()
        Dim result As New ResultSetPost()
        result.Success = False
        result.Message = "Revoca non effettuata"
        revoca = GenericProvider.AppuntamentiGiorno.GetDatiPropostaVariazione(idAppuntamento)
        If revoca.CodicePaziente.IsNullOrEmpty Then
        ElseIf revoca.DataConvocazione = Date.MinValue Then
        ElseIf revoca.DurataAppuntamento = 0 Then
        ElseIf revoca.CodiceConsultorioAppuntamento.IsNullOrEmpty Then
        Else
            Dim command As New BizRicercaAppuntamenti.SalvaAppuntamentiCommand()
            Using res As New BizRicercaAppuntamenti(GenericProvider, Settings, ContextInfos, Nothing)

                command.CodicePaziente = revoca.CodicePaziente
                command.DataConvocazione = revoca.DataConvocazione
                command.DataAppuntamento = revoca.DataAppuntamento
                command.EliminaBilancio = False
                command.DataAppuntamento = Nothing
                command.NoteAppuntamento = Note
                command.IdMotivoEliminazioneAppuntamento = Constants.MotiviEliminazioneAppuntamento.EliminazioneAppuntamento
                command.NoteUtenteModificaAppuntamento = "ELIMINAZIONE APPUNTAMENTO"
                command.DurataAppuntamento = revoca.DurataAppuntamento
                command.CodiceConsultorioAppuntamento = revoca.CodiceConsultorioAppuntamento
                command.CodiceAmbulatorio = revoca.CodiceAmbulatorio
                command.IsSoloBilancio = False
                command.SuppressLog = True

                Dim salvaResult As BizRicercaAppuntamenti.SalvaAppuntamentiResult = res.SalvaAppuntamento(command)

                result.Success = salvaResult.Success

                If result.Success Then

                    result.Message = "Revoca effettuata con successo"

                    ' Se le vaccinazioni programmate della convocazione sono tutte annullabili, viene eliminata anche la convocazione. 
                    ' Il controllo sull'annullabilità è già stato fatto prima di eliminare l'appuntamento.
                    Dim count As Integer = 0

                    Using bizCnv As BizConvocazione = New BizConvocazione(GenericProvider, Settings, ContextInfos, LogOptions)

                        count = bizCnv.EliminaConvocazioni(
                            New BizConvocazione.EliminaConvocazioniCommand() With {
                                .CodicePaziente = revoca.CodicePaziente,
                                .DataConvocazione = revoca.DataConvocazione,
                                .DataEliminazione = Date.Now
                            }
                        )

                    End Using

                    If count > 0 Then
                        result.Message += " - Convocazione eliminata"
                    End If
                Else
                    result.Success = False
                    result.Message = "Revoca non effettuata" + If(String.IsNullOrWhiteSpace(salvaResult.Message), String.Empty, ": " + salvaResult.Message)
                End If

            End Using
        End If

        Return result

    End Function

#End Region

End Class