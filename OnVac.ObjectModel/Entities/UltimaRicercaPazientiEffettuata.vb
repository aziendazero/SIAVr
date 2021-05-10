Namespace Entities

    ''' <summary>
    ''' Dati dell'ultima ricerca paziente effettuata
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class UltimaRicercaPazientiEffettuata

        Public Property CodicePaziente As String
        Public Property CodiceAusiliarioPaziente As String
        Public Property Cognome As String
        Public Property Nome As String
        Public Property Sesso As String
        Public Property DataNascita As DateTime?
        Public Property AnnoNascita As String
        Public Property CodiceComuneNascita As String
        Public Property DescrizioneComuneNascita As String
        Public Property CodiceFiscale As String
        Public Property TesseraSanitaria As String
        Public Property CodiceComuneResidenza As String
        Public Property DescrizioneComuneResidenza As String
        Public Property CodiceConsultorio As String
        Public Property DescrizioneConsultorio As String
        Public Property SoloLocale As Boolean

        Public Sub New()
        End Sub

        Public Function IsEmpty() As Boolean

            If Not String.IsNullOrEmpty(CodicePaziente) Then Return False
            If Not String.IsNullOrEmpty(Cognome) Then Return False
            If Not String.IsNullOrEmpty(Nome) Then Return False
            If Not String.IsNullOrEmpty(Sesso) Then Return False
            If DataNascita.HasValue Then Return False
            If Not String.IsNullOrEmpty(AnnoNascita) Then Return False
            If Not String.IsNullOrEmpty(CodiceComuneNascita) Then Return False
            If Not String.IsNullOrEmpty(DescrizioneComuneNascita) Then Return False
            If Not String.IsNullOrEmpty(CodiceFiscale) Then Return False
            If Not String.IsNullOrEmpty(TesseraSanitaria) Then Return False
            If Not String.IsNullOrEmpty(CodiceConsultorio) Then Return False
            If Not String.IsNullOrEmpty(DescrizioneConsultorio) Then Return False

            Return True

        End Function

    End Class

End Namespace