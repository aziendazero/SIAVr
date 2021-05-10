Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DataSet


Public Interface IAppuntamentiGiornoProvider

    Function BuildDataSetAppuntamentiGiornoBilanci(codiceConsultorio As String, codiceUsl As String, strDataInizio As String, strDataFine As String, filtroAvvisati As Enumerators.FiltroAvvisati) As AppuntamentiGiornoBilanci
    Function BuildDataSetAppuntamentiGiornoBilanci(codiceConsultorio As String, codiceAmbulatorio As Integer, codiceUsl As String, strDataInizio As String, strDataFine As String, filtroAvvisati As Enumerators.FiltroAvvisati, filtroAssociazioniDosi As Entities.FiltroComposto) As AppuntamentiGiornoBilanci

    Function FindAppuntamentiGiorno(codiceConsultorio As String, codiceAmbulatorio As Integer, codiceUsl As String, strData As String, strCampiOrdinamento As String) As DataTable

    Function GetVaccDosiPaziente(codicePaziente As Integer, dataConvocazione As Date) As DataTable

    Function ControllaSoloBilancio(codicePaziente As Integer, dataConvocazione As Date) As Boolean
    Function ControllaAvviso(codicePaziente As Integer, dataConvocazione As Date) As Boolean
    Function GetListPazientiAvvisi(command As Entities.PazientiAvvisiCommand) As List(Of Entities.PazienteAvviso)
    Function GetListPazientiAvvisiPostel(command As Entities.PazientiAvvisiCommand) As List(Of Entities.PazienteAvviso)

    Function GetListAppuntamentiPazienti(codicePazienti As List(Of Long)) As List(Of Entities.Appuntamento)
    Function GetListAppuntamentiPazienteData(codicePaziente As Long, dataAppuntamento As DateTime) As List(Of Entities.Appuntamento)
    Function GetAppuntamentiNotifica(dataAppuntamentoDa As DateTime, dataAppuntamentoA As DateTime) As List(Of Entities.AppuntamentoNotifica)
    Function GetDatiAppuntamentoStampa(idConvocazione As String) As DTOAppuntamentoStampa
    Function GetListAppuntamentiPazientiApi(listCodiciPazienti As List(Of Long)) As List(Of Appuntamento)
    Function GetListAppuntamentiPazientiByIdApi(IdAppuntamento As Long) As List(Of Appuntamento)
    Function ExistsAppuntamentoAmbulatorio(dataAppuntamento As Date, codiceAmbulatorio As Integer) As Boolean
    Function ExistsAppuntamentoPaziente(dataAppuntamento As Date, codicePaziente As Long) As Boolean
    Function GetPrenotazioneAnnullabileEsterni(codicePaziente As Integer, dataConv As Date) As String
    Function InserisciLockAppuntamento(CodiceAmb As Long, DataAppuntamento As Date) As Integer
    Function GetLockAppuntamento(DataAppuntamento As Date, CodiceAmbulatorio As Long, Nparametro As Integer) As LockAppuntamento
End Interface
