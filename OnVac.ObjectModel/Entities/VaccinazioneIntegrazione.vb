Imports System.Collections.Generic

Namespace Entities

    <Serializable()>
    Public Class VaccinazioneIntegrazioneDB

        Public Property IdVaccinazione As Long
        Public Property CodicePaziente As Long
        Public Property DataEffettuazione As Date
        Public Property DataOraEffettuazione As Date
        Public Property Luogo As String
        Public Property CodiceConsultorio As String
        Public Property CodiceMedico As String
        Public Property CodiceVaccinatore As String
        Public Property AssociazioneCod As String
        Public Property NrDoseAssociazione As Integer
        Public Property AssociazioneDescr As String
        Public Property CodiceVaccinazione As String
        Public Property DescrVaccinazione As String
        Public Property NrDoseVaccinazione As Integer
		Public Property CodiceLotto As String
		Public Property DataScadenzaLotto As Date
		Public Property CodiceNomeCommerciale As String
        Public Property CodiceSomministrazione As String
        Public Property CodiceInoculazione As String
        Public Property Scaduta As Boolean
        Public Property Stato As String
        Public Property IdACN As String
        Public Property IsMedicoInAmbulatorio As Boolean?
        Public Property Note As String
        Public Property CodiceMalattia As String
        Public Property CodiceEsenzione As String
        Public Property InCampagna As String
		Public Property Provenienza As Enumerators.ProvenienzaVaccinazioni?
		Public Property CodiceCondizioneRischio As String
		Public Property CodiceMalCondizioneSanitaria As String
		Public Property TipoPagamento As Guid
		Public Property Ulss As String
		Public Property TipoAssociazione As String

	End Class

End Namespace



