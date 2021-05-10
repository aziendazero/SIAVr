Imports System.Collections.Generic

Namespace Entities
	<Serializable>
	Public Class ElaborazioneACN
		Public Property Id As Integer?
		Public Property IdVaccinoAcn As String
		Public Property DataInizio As DateTime?
		Public Property DataFine As DateTime?
		Public Property CfInviante As String
		Public Property CfMedico As String
		Public Property NomeMedico As String
		Public Property CognomeMedico As String
		Public Property CodiceRegionaleMedico As String
		Public Property TipoMedico As String
		Public Property CodiceUlss As String
		Public Property TipoAttivita As String
		Public Property DataOraAttivita As DateTime?
		Public Property CfAttivita As String
		Public Property CodicePrincipioAttivo As String 'Associazione farmaco
		Public Property CodiceAic As String ' Codice aic del farmaco
		Public Property CodiceSitoInoculazione As String
		Public Property CodiceViaSomministrazione As String
		Public Property Lotto As String
		Public Property DataScadenzaLotto As Date?
		Public Property CodiceCategoriaRischio As String
		Public Property CodiceModalitaPagamento As String
		Public Property CodCampagnaVaccinale As String
		Public Property CodiceDiagnosi As String
		Public Property CfPaziente As String
		Public Property CodieMpiPaziente As String
		Public Property CognomePaziente As String
		Public Property NomePaziente As String
		Public Property IdPrcAcquisizione As Integer?
		Public Property DataAcquisizione As DateTime?
		Public Property CodicePazienteAcquisizione As Integer?
		Public Property StatoAcquisizione As Enumerators.StatoAcquisizioneElaborazioneVaccinazioneAcn?
		Public Property MessaggioAcquisizione As String
	End Class

	<Serializable>
	Public Class FiltroElaborazioneACN
		Public Property Id As Long?
		Public Property DataAcquisizione As Date?
		Public Property DataAcquisizioneA As Date?
		Public Property IdProcessoAcquisizione As Integer?
		Public Property AppId As String
		Public Property StatoAcquisizione() As Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente?

	End Class
	<Serializable>
	Public Class DatoDecodificato

		Public Property IdACN As String
		Public Property PazienteVac As Paziente
		Public Property DataEffettuazione As DateTime
		Public Property IsCampagnaVaccinale As String
		Public Property CodiceAssociazione As String
		Public Property ListaVaccinazioni As List(Of VaccinazioneAssociazione)
		Public Property CodiceSitoInoculazione As String
		Public Property CodiceViaSomministrazione As String
		Public Property ListaMalattie As List(Of Malattia)
		Public Property CodEsenzione As String
		Public Property CodiceNomeCommerciale As String
		Public Property NrLotto As String
		Public Property DataScadezaLotto As DateTime
		Public Property CodiceVaccinatore As String
		Public Property CodiceMedico As String
		Public Property PresenzaMedico As Boolean?
		Public Property LuogoEsecuzione As String
		Public Property CondizioneDiRischio As String
		Public Property MalCondizioneSanitaria As String
		Public Property TipoPagamento As Guid
		Public Property MalCodiceMalattia As String
		Public Property CodiceEsenzione As String
		Public Property Ulss As String
		Public Property Note As String
		Public Property TipoAssociazione As String
		Public Property EsitoControlli As Boolean
		Public Property MessaggioControlli As System.Text.StringBuilder
		Public Property listCodiciAss As List(Of String)
	End Class

End Namespace