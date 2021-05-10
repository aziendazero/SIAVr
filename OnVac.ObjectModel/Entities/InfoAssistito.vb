Imports System.Runtime.Serialization
Namespace Entities

    <Serializable()>
    <DataContract()>
    Public Class InfoAssistito
        <DataMember()>
        Public Property Codice As Integer
        Public Property Cognome As String
        Public Property Nome As String
        Public Property Sesso As String
        Public Property Codice_Fiscale As String
        Public Property Tessera As String
        Public Property DataDiNascita As DateTime
        Public Property ComuneDiNascita As String
        Public Property CittadinanzaCodice As String
        Public Property CittadinanzaDescrizione As String
        Public Property ComuneResidenza As Localita
        Public Property IndirizzoResidenza As String
        Public Property ComuneDomicilio As Localita
        Public Property IndirizzoDomicilio As String
        Public Property Telefono As String
        Public Property UslResidenzaCodice As String
        Public Property UslResidenzaDescrizione As String
        Public Property Stato As StatiVaccinaliEnum?
        Public Property DataInserimento As DateTime
        Public Property DataDecesso As Date?

        Public Sub New()
            ComuneResidenza = New Localita()
            ComuneDomicilio = New Localita()
        End Sub

    End Class

    Public Class Localita
        <DataMember()>
        Public Property Codice As String
        Public Property Descrizione As String
        Public Property Provincia As String
        Public Property Cap As String
        Public Property DataInizio As String
        Public Property DataFine As String
    End Class

    Public Class ContattiAssistito
        Public Property Telefono As String
        Public Property Telefono2 As String
        Public Property Telefono3 As String
        Public Property EMail As String
    End Class
    Public Enum StatiVaccinaliEnum
        InCorso = 3
        Terminato = 4
        InadempienteTotale = 9
    End Enum
    Public Enum StatiDocumentoEnum
        PresaInCarico = 1
        DaValutare = 2
        Confermato = 3
        Annullato = 4
    End Enum

    Public Class SetContattoResult
        Public Property Success As Boolean
        Public Property Message As String
    End Class
    Public Class ResultSetPost
        Public Property Success As Boolean
        Public Property Message As String
    End Class
    Public Class ResultTestSierologici
        Inherits ResultSetPost
        Public Property IdTest As Long
    End Class

    Public Class DTOSetContatto
        Inherits ContattiAssistito

        Public Property CodiceFiscale As String

    End Class
    Public Class DTOSetIndirizzoTemporaneo
        Public Property CodiceFiscale As String
        Public Property IndirizzoTemporaneo As String
        Public Property InizioIT As DateTime
        Public Property FineIT As DateTime
    End Class
    Public Class DtoDocumento
        Public Property ID As Integer
        Public Property DataArchiviazione As DateTime
        Public Property Descrizione As String
        Public Property Tipologia As String
        Public Property NomeDocumento As String
        Public Property StatoDocumentoDescrizione As String
        Public Property StatoDocumentoId As Integer
        Public Property Documento64 As String
    End Class

    Public Class DtoVaccino
        Public Property Codice As String
        Public Property Descrizione As String
        Public Property ImportoIndicativo As Double
        Public Property Obbligatorio As Boolean
    End Class

End Namespace


