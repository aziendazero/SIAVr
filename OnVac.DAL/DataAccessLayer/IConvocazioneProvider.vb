Imports System.Collections.Generic

Public Interface IConvocazioneProvider

    Function GetFromKey(codicePaziente As Integer, dataConvocazione As Date) As DataTable

    Function InsertConvocazione(convocazione As Entities.Convocazione) As Boolean
    Function UpdateConvocazione(convocazione As Entities.Convocazione) As Boolean

    Function GetCnvFromInterval(codicePaziente As Integer, firstDate As Date, secondDate As Date) As DataTable
    Function GetDataInvio(codicePaziente As Integer, dataConvocazione As Date) As Date
    Function GetMaxSollecitoVaccinazioni(codicePaziente As Integer, dataConvocazione As Date) As Int32
    Function GetDurataAppuntamento(codicePaziente As Integer, dataConvocazione As Date) As Int32
    Function TerminePerentorio(codicePaziente As Int32, dataConvocazione As Date?, parametroNumSol As Integer) As Boolean

    Function Exists(codicePaziente As Integer) As Boolean
    Function Exists(codicePaziente As Integer, dataConvocazione As Date) As Boolean
    Function ExistsConvocazioneAltroConsultorio(codicePaziente As Integer, dataConvocazione As Date, codiceConsultorioCorrente As String) As Boolean
    Function ExistsStessaVaccinazioneInConvocazioni(codicePaziente As Integer, dataConvocazione1 As Date, dataConvocazione2 As Date) As Boolean

    Function CountConvocazioniConAppuntamento(codicePaziente As Integer, codiceConsultorioCorrente As String, soloConsultorioCorrente As Boolean) As Integer
    Function CountConvocazioniSenzaAppuntamento(codicePaziente As Integer, codiceConsultorioCorrente As String, soloConsultorioCorrente As Boolean) As Integer
    Function CountConvocazioniPrecedentiFineSospensione(codicePaziente As Integer, dataFineSospensione As DateTime) As Integer

    Sub Copy(codicePaziente As Integer, dataConvocazioneOld As Date, dataConvocazioneNew As Date, empyDataAppuntamento As Boolean, emptyDataInvio As Boolean)

    Function UpdateDataInvio(codicePaziente As Integer, dataConvocazione As Date, dataInvio As Date, noteAvvisi As String) As Boolean
    Function UpdateDurataAppuntamento(codicePaziente As Integer, dataConvocazione As Date, durataAppuntamento As Integer) As Boolean
    Sub UpdateDataInvioSollecitoVaccinazioni(codicePaziente As Int32, dataConvocazione As Date, dataStampa As Date)
    Function UpdateDatiConvocazioneSoloBilancio(codicePaziente As Integer, dataConvocazione As Date, durataAppuntamento As Integer, campagna As String) As Boolean

    Function DeleteEmpty(codicePaziente As Integer, dataConvocazione As DateTime?) As Boolean
    Function Delete(codicePaziente As Integer, cancellaBilanciAssociati As Boolean) As Boolean
    Function Delete(codicePaziente As Integer, dataConvocazione As DateTime?, cancellaBilanciAssociati As Boolean) As Boolean
    Function DeleteSollecitiBilancioConvocazione(codicePaziente As Integer, dataConvocazione As DateTime?) As Integer
    Function DeleteBilancioConvocazione(codicePaziente As Integer, dataConvocazione As DateTime?) As Integer
    Function DeleteConvocazione(codicePaziente As Integer, dataConvocazione As DateTime?) As Integer

    Sub UpdateCnvCampagna(codicePaziente As Integer, dataConvocazione As Date, inCampagna As Boolean)
    Function IsCnvCampagna(codicePaziente As Integer, dataConvocazione As Date) As Boolean

    Function EliminaCicliSenzaVaccinazioniProgrammate(codicePaziente As Integer, dataConvocazione As DateTime?) As Integer

    Function GetDtConvocazioniPaziente(codicePaziente As Integer) As DataTable
    Function GetConvocazionePaziente(codicePaziente As String, dataConvocazione As DateTime) As Entities.Convocazione
    Function GetConvocazioniPaziente(codicePaziente As String, appuntamenti As Boolean?) As ICollection(Of Entities.Convocazione)
    Function GetConvocazioniPaziente(codicePaziente As String) As ICollection(Of Entities.Convocazione)
    Function GetConvocazioniVaccinazioniPaziente(codicePaziente As Integer) As List(Of Entities.ConvocazioneVaccinazione)

    Function GetDateConvocazioniPaziente(codicePaziente As Int64, maxDataConvocazione As DateTime?) As IEnumerable(Of DateTime)

    Function GetCalendarioVaccinaleDS(codicePaziente As Integer) As DSCalendarioVaccinale

    Function GetCicliConvocazioniPaziente(codicePaziente As Integer) As IEnumerable(Of Entities.CicloConvocazione)
    Function GetRitardiCicliConvocazioniPaziente(codicePaziente As Integer) As IEnumerable(Of Entities.RitardoCicloConvocazione)

    Sub InsertCicloConvocazione(cicloConvocazione As Entities.CicloConvocazione)
    Sub InsertRitardoCicloConvocazione(ritardoCicloConvocazione As Entities.RitardoCicloConvocazione)

    Function InsertCicliCnvUnita(codicePaziente As Integer, oldDataConvocazione As Date, newDataConvocazione As Date) As Integer
    Function UpdateRitardiCnvUnita(codicePaziente As Integer, oldDataConvocazione As Date, newDataConvocazione As Date) As Integer
    Function DeleteCicliCnvUnita(codicePaziente As Integer, oldDataConvocazione As Date) As Integer

    Function EliminaCicliConvocazione(codicePaziente As Integer, dataConvocazione As DateTime) As Integer
    Function GetIdConvocazione(pazCodice As Long, dataConv As Date) As String
End Interface
