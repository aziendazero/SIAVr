Imports System.Collections.Generic

Public Interface IStampaElencoEsclusione

    Function GetDataSetElencoEsclusione(filtriElencoEsclusione As IStampaElencoEsclusione.FiltriElencoEsclusione, codiceUsl As String) As DataSet.DSElencoEsclusione
    Function GetDataSetElencoEsclusioneVaccinazioni(filtriElencoEsclusione As IStampaElencoEsclusione.FiltriElencoEsclusione) As DataSet.DSElencoEsclusione

    Class FiltriElencoEsclusione
        Public DataNascitaInizio As DateTime
        Public DataNascitaFine As DateTime
		Public CodiceConsultorio As List(Of String)
		Public DescrizioneConsultorio As String
        Public CodiceComuneResidenza As String
        Public DescrizioneComuneResidenza As String
        Public CodiceCircoscrizione As String
        Public DescrizioneCircoscrizione As String
        Public CodiceDistretto As String
        Public DescrizioneDistretto As String
        Public SoloVaccinazioniObbligatorie As Boolean
        Public CodiceMotivoEsclusione As String
        Public DescrizioneMotivoEsclusione As String
        Public CodiceVaccinazione As String
        Public DescrizioneVaccinazione As String
        Public CodiciStatiAnagrafici As String
        Public DescrizioniStatiAnagrafici As String
    End Class

End Interface
