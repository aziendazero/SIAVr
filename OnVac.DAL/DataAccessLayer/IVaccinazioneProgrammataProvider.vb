Imports System.Collections.Generic


Public Interface IVaccinazioneProgrammataProvider

    Function CountFromKey(codicePaziente As Integer, dataConvocazione As Date) As Integer
    Function CountProgrammatePaziente(codicePaziente As Integer) As Integer
    Function CountProgrammazioneDaEliminare(codicePaziente As Integer, dataConvocazione As Date?, eliminaAppuntamenti As Boolean, eliminaSollecitiBilancio As Boolean, eliminaBilanci As Boolean) As Integer

    Function GetVacProgNotEseguiteAndNotEscluse(codicePaziente As Integer, dataConvocazione As Date) As DataTable
    Function GetVacProgNotEseguiteAndNotEscluseListCodiceDose(codicePaziente As Long, dataConvocazione As Date) As List(Of KeyValuePair(Of String, Integer))
    Function GetVacProgNotEseguiteAndNotEscluseListBilancioVaccinazioni(codicePaziente As Long, dataConvocazione As Date) As List(Of BilancioVaccinazione)
    Function GetProgrammazioneDaEliminare(codicePaziente As Integer, dataConvocazione As Date?, eliminaAppuntamenti As Boolean, eliminaSollecitiBilancio As Boolean, eliminaBilanci As Boolean, codiciCicli As List(Of String)) As List(Of ProgrammazioneDaEliminare)

    Function EliminaVaccinazioneProgrammata(codicePaziente As Integer, codiceVaccinazione As String, dataConvocazione As Nullable(Of DateTime)) As Integer
    Function EliminaVaccinazioneProgrammataByRichiamo(codicePaziente As Integer, codiceVaccinazione As String, numeroRichiamo As Int32) As Integer

    Function GetCodiceVacProgrammatePazienteByData(codicePaziente As Integer, dataConvocazione As DateTime) As ArrayList

    Function GetDataByVaccinazione(codicePaziente As Integer, codiceVaccinazione As String) As Object
    Function GetVacProgrammatePazienteByData(codicePaziente As Integer, dataConvocazione As DateTime) As DataTable
    Function GetVaccinazioneProgrammata(codicePaziente As Integer, codiceVaccinazione As String, dataConvocazione As DateTime?) As List(Of VaccinazioneProgrammata)
    Function GetVaccinazioniProgrammatePaziente(codicePaziente As Integer) As IEnumerable(Of VaccinazioneProgrammata)

    Function GetVaccinazioniProgrammateByCnv(codicePaziente As Long, dataConvocazione As DateTime) As List(Of VaccinazioneProgrammata)

    Function ExistsVaccinazioneProgrammataByRichiamo(codicePaziente As Integer, codiceVaccinazione As String, numeroRichiamo As Int16) As Boolean
    Function ExistsVaccinazioneProgrammataByConvocazione(codicePaziente As Integer, codiceVaccinazione As String, dataConvocazione As DateTime) As Boolean

    Function InsertVaccinazioneProgrammata(codicePaziente As Integer, dataConvocazione As DateTime, codiceVaccinazione As String, codiceAssociazione As String, numeroRichiamo As Integer, dataInserimento As DateTime, idUtenteInserimento As Long) As Integer
    Sub InsertVaccinazioneProgrammata(vaccinazioneProgrammata As VaccinazioneProgrammata)

    Function UpdateVaccinazioneProgrammata(vaccinazioneProgrammata As VaccinazioneProgrammata) As Boolean

    Function UpdateDataCnv(codicePaziente As Integer, oldDataConvocazione As Date, newDataConvocazione As Date) As Integer

    Function GetVaccinazioniProgrammateAssociate(dataInizioApp As DateTime, dataFineApp As DateTime, dataInizioNascita As DateTime?, dataFineNascita As DateTime?, codiceMedico As String, codiceStatoAnagrafico As String, codiceVaccini As String, numeroSeduta As Integer?, codiceConsultorio As List(Of String)) As List(Of StatVaccinazioneProgrammateAssociate)

End Interface
