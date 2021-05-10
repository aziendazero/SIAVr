Imports System.Collections.Generic

Namespace Entities

    <Serializable()>
    Public Class ConsultorioInfoAPP
        Public Property CodiceConsultorio As String
        Public Property DescrizioneConsultorio As String
        Public Property CodiceComune As String
        Public Property DescrizioneComune As String
        Public Property Indirizzo As String
        Public Property Telefono As String
        Public Property Note As String
        Public Property CodiceAmbulatorio As Integer
        Public Property DescrizioneAmbulatorio As String
        Public Property CountOrariGiornalieri As Integer
        Public Property OrariAmbulatorio As IEnumerable(Of OrarioInfoAPP)
        Public Property AppIdAziendaLocale As String
    End Class

    <Serializable()>
    Public Class OrarioInfoAPP

        ''' <summary>
        ''' Da 0 (domenica) a 6 (sabato)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Giorno() As Integer

        Public Property OraInizioAM As DateTime?
        Public Property OraFineAM As DateTime?
        Public Property OraInizioPM As DateTime?
        Public Property OraFinePM As DateTime?

        Public Property CodiceAmbulatorio As Integer

    End Class

End Namespace