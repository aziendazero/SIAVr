Imports System.Collections.Generic

Public Interface IAssociazioniProvider

    Function GetVaccinazioniAssociazione(codiceAssociazione As String) As ArrayList
    Function GetDtVaccinazioniAssociazioni(listCodiciAssociazioni As List(Of String)) As DataTable
    Function GetDescrizioneAssociazione(codiceAssociazione As String) As String

    Function LoadAssociazioniDaInserire(codicePaziente As Integer, dataConvocazione As Date, gestioneTipiConsultori As Boolean,
                                        codiceConsultorio As String, codiciVaccinazioniProgrammateConvocazione As String(),
                                        getSitoInoculazioneDefault As Boolean, getViaSomministrazioneDefault As Boolean) As DataTable

    Function GetInfoSomministrazioneDefaultAssociazione(codiceAssociazione As String) As Entities.InfoSomministrazione

    Function GetAssociazioneInfo(codiceAssociazione As String) As Entities.AssociazioneInfo
    Function InsertAssociazioneInfo(info As Entities.AssociazioneInfo) As Integer
    Function UpdateAssociazioneInfo(info As Entities.AssociazioneInfo) As Integer
    Function DeleteAssociazioneInfo(idInfo As Integer) As Integer

    Function GetListAssociazioniAPP() As List(Of Entities.AssociazioneAPP)
    Function GetListAssociazioniAPP(codiceVaccinazione As String) As List(Of Entities.AssociazioneAPP)
    Function GetListaVacAss(codiceAssociazione As List(Of String)) As List(Of Entities.StatVacciniAssociati)
    Function GetAssociazioneByCodiceACN(codiceEsterno As String) As AssociazioneAnag
    Function GetVaccinazioniAssociazioni(codiciAssociazioni As List(Of String)) As List(Of VaccinazioneAssociazione)
    Function GetVaccByCodiceACN(codVaccinazioneACN As String, codAssociazione As String) As VaccinazioneAssociazione
	Function GetAssociazioneByCodiceACNAic(codiceACN As String, codiceAic As String) As AssociazioneAnag
    Function GetListCodiciAssociazioni() As List(Of String)
End Interface