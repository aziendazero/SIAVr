Imports System.Collections.Generic

Namespace Entities.Anamnesi

    '<System.Xml.Serialization.XmlTypeAttribute()>

    <System.SerializableAttribute()>
    <System.Xml.Serialization.XmlRoot("Anamnesi")>
    Public Class Anamnesi

        Public Property IdVisita As Long
        Public Property DataVisita As Date
        Public Property Peso As Double?
        Public Property PercentilePeso As String
        Public Property RegistraPeso As Boolean
        Public Property Altezza As Double?
        Public Property PercentileAltezza As String
        Public Property RegistraAltezza As Boolean
        Public Property Cranio As Double?
        Public Property PercentileCranio As String
        Public Property RegistraCranio As Boolean
        Public Property BilancioNumero As Long?
        Public Property BilancioDescrizione As String
        Public Property BilancioEtaMinima As String
        Public Property MalattiaCodice As String
        Public Property MalattiaDescrizione As String
        Public Property IdUtente As Long
        Public Property MedicoCodice As String
        Public Property MedicoDescrizione As String
        Public Property Firma As String

        Public Property PazienteCodice As Long
        Public Property PazienteCognome As String
        Public Property PazienteNome As String
        Public Property PazienteDataNascita As DateTime
        Public Property PazienteSesso As String
        Public Property PazienteIndirizzo As String

        <System.Xml.Serialization.XmlElement("Sezione")>
        Public Property Sezioni As List(Of Entities.Anamnesi.Sezione)

    End Class

    <System.SerializableAttribute()>
    Public Class Sezione

        Public Property SezioneCodice As String
        Public Property SezioneDescrizione As String
        Public Property SezioneNumero As String

        <System.Xml.Serialization.XmlElement("Osservazione")>
        Public Property Osservazioni As List(Of Entities.Anamnesi.Osservazione)

    End Class

    <System.SerializableAttribute()>
    Public Class Osservazione

        Public Property OsservazioneId As Int64
        Public Property OsservazioneCodice As String
        Public Property OsservazioneDescrizione As String
        Public Property OsservazioneNumero As Integer
        Public Property RispostaCodice As String
        Public Property RispostaDescrizione As String
        Public Property RispostaTesto As String

    End Class

End Namespace
