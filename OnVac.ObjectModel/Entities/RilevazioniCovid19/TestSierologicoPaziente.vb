Namespace Entities
    Public Class TestSierologicoPaziente
        Public Property Tipologia As TipologiaTestSierologico

        Public Property CodiceCampione As String
        Public Property Cognome As String
        Public Property Nome As String
        Public Property CodiceFiscale As String
        Public Property DataDiNascita As Date
        Public Property LuogoDiNascita As String
        Public Property Sesso As String
        Public Property Residenza As String
        Public Property DataPrelievo As Date
        Public Property Centro As String
        Public Property UlssResidenza As String
        Public Property UlssRichiedente As String
        Public Property DataReferto As Date?
        Public Property IGG As String
        Public Property IGGUnita As String
        Public Property IGGDescrizione As String
        Public Property IGM As String
        Public Property IGMUnita As String
        Public Property IGMDescrizione As String

        Public Property CodiceSettingScolastico As Integer?
        Public Property DescrizioneSettingScolastico As String
        Public Property CodicePersonaleScolastico As Integer?
        Public Property DescrizionePersonaleScolastico As String
        Public Property Valido As Boolean
        Public Property Marca As String
        Public Property EsitoRapido As String
        Public Property Richiedente As String
        Public Property CodiceLaboratorio As String
        Public Property DescrizioneLaboratorio As String
    End Class

    Public Enum TipologiaTestSierologico
        sierologico = 0
        Rapido = 1
        RapidoAntigene = 2
    End Enum
End Namespace
