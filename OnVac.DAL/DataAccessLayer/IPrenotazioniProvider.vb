Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.Filters
Imports System.Collections.Generic

Public Interface IPrenotazioniProvider

    Function AppuntamentoSovrapposto(tmpData As String, durataAppuntamento As Integer, codiceAmbulatorio As Integer, orario As String) As Boolean

    Function OrarioCompresoInOrarioAperturaAmb(tmpData As String, durataAppuntamento As Integer, codiceAmbulatorio As Integer, orario As String) As Boolean

    Function CercaConvocazioniAppuntamentiPaziente(codicePaziente As Integer, codiceConsultorioCorrente As String, soloConsultorioCorrente As Boolean) As DataTable

    Function GetOrariAppuntamento(codiceAmbulatorio As Integer) As DataTable
    Function GetFiltriApertura(codiceAmbulatorio As Integer, fuoriOrario As Boolean, giorno As DayOfWeek) As DataTable
    Function GetFiltriApertura(codiceAmbulatorio As Integer, ByRef cnsControl As Boolean) As DataTable
    Function GetFestivita(codiceAmbulatorio As Integer) As DataTable
    Function GetPrenotati(codiceAmbulatorio As Integer, dayStart As Date, dayEnd As Date) As DataTable
    Function GetGiorniApertura(codiceAmbulatorio As Integer) As DataTable
    Function GetGiorniFestivi(codiceAmbulatorio As Integer) As DataTable

    Function GetAppuntamenti(codiceAmbulatorio As Integer) As DataTable

    Sub FillDtMattino(dtMattino As DataTable, codiceAmbulatorio As Integer, data As Date, ByRef oraAMPM As DateTime, maxNumSollDefault As Integer)
    Sub FillDtPomeriggio(dtPomeriggio As DataTable, codiceAmbulatorio As Integer, data As Date, ByRef oraMinPM As DateTime, maxNumSollDefault As Integer)

    Function ControllaOrariApertura(codiceAmbulatorio As Integer) As Boolean

    Function BuildDtFeste() As DataTable
    Function BuildDtIndisponibilita(codiceAmbulatorio As Integer) As DataTable

    Function CaricaOrariPersonalizzati(codiceAmbulatorio As Integer) As DataTable

    Function CercaConvocazioni(codiciConsultorio As List(Of String), durataSedutaBilancioDefault As Integer, maxNumSollDefault As Integer, validitaPerSoloBilancio As Integer, ByRef filtri As FiltriGestioneAppuntamenti, usaOrdineAlfabetico As Boolean) As DataTable

    Function UltimaDataCnvConsultorio(codiceConsultorio As String) As String

    Function SalvaOrariPersonalizzati(codiceAmbulatorio As Integer, ByRef dtOrariPersonalizzati As DataTable) As Boolean
    Sub SalvaUltimaDataCnvConsultorio(codiceConsultorio As String, data As Date)

    Function GetDataPrimoAppuntamento(codicePaziente As Long, dataConvocazione As DateTime) As DateTime?
    Function UpdateDataPrimoAppuntamento(codicePaziente As Long, dataConvocazione As DateTime, dataPrimoAppuntamento As DateTime) As Integer

    Function UpdateConvocazione_DatiAppuntamento(convocazioneAppuntamento As Entities.ConvocazioneAppuntamento) As Integer

    Function GetNextIdStoricoAppuntamenti() As Long
    Function InsertStoricoAppuntamenti(convocazioneAppuntamento As Entities.ConvocazioneAppuntamento) As Integer
    Function GetLastIdStoricoAppuntamenti(codicePaziente As Long, dataConvocazione As DateTime) As Long?
    Function GetLastStoricoAppuntamenti(codicePaziente As Long, dataConvocazione As DateTime) As Entities.ConvocazioneAppuntamento
    Function UpdateStoricoAppuntamenti_Eliminazione(idConvocazioneAppuntamento As Long, idUtenteEliminazione As Long, dataEliminazione As DateTime, motivoEliminazione As String, noteEliminazione As String, noteModificaAppuntamento As String) As Integer
    Function UpdateStoricoAppuntamenti_DatiInvio(idConvocazioneAppuntamento As Long, idUtenteInvio As Long, dataInvio As DateTime, noteAvvisi As String) As Integer

    Function GetDataAppuntamento(codicePaziente As Long, dataConvocazione As DateTime) As DateTime?
    Function GetMotiviEliminazioneAppuntamento() As List(Of KeyValuePair(Of String, String))
    Function CountStoricoAppuntamenti(codicePaziente As Long, dataConvocazione As DateTime?) As Integer
    Function GetStoricoAppuntamenti(codicePaziente As Long, dataConvocazione As DateTime?, orderBy As String, pagingOptions As Onit.OnAssistnet.Data.PagingOptions) As List(Of StoricoAppuntamento)

End Interface
