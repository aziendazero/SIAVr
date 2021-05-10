Imports System.Collections.Generic

Namespace Entities
    Public Class InformazioneContattoEsportazione
        Public Property Classe As String
        Public Property CodiceElaborazione As Long?
        Public Property CodicePaziente As Long
        Public Property Nome As String
        Public Property Cognome As String
        Public Property Sesso As String
        Public Property DataNascita As Date
        Public Property ComuneResidenza As String
        Public Property CodiceStato As Integer?
        Public Property DescrizioneStato As String
        Public Property Tipologia As String
        Public Property Ultimo As InformazioniTestScolastici
        Public Property Iniziale As InformazioniTestScolastici
        Public Property Finale As InformazioniTestScolastici
    End Class

    Public Class InformazioniTestScolastici
        Public Property CodicePaziente As Long
        Public Property Data As Date
        Public Property RisultatoTampone As String
        Public Property IsTampone As Boolean
        Public Property Esito As String
        Public Property IGG As String
        Public Property IGM As String
        Public Property Motodologia As String
    End Class
End Namespace
