Imports System.Collections.Generic

Public Interface IStampaConteggioSedute

	Function GetDataSetConteggioSedute(filtriConteggio As IStampaConteggioSedute.FiltriConteggioSedute, codiceUsl As String) As DSConteggioSedute


	Class FiltriConteggioSedute
		Public DataEffettuazioneInizio As DateTime
		Public DataEffettuazioneFine As DateTime
		Public CodiceConsultorio As List(Of String)
		Public DescrizioneConsultori As String
		Public FlagVacciniEseguiti As Boolean
		Public FlagVacciniRegistrati As Boolean
		Public FlagTipoCentroAdulti As Boolean
		Public FlagTipoCentroPediatrico As Boolean
		Public FlagTipoCentroVaccinatore As Boolean
		Public CodiceDistretto As String
		Public DescrizioneDistretto As String
	End Class

End Interface
