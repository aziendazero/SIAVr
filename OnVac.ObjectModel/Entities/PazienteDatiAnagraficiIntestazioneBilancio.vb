
Namespace Entities

    <Serializable()>
    Public Class PazienteDatiAnagraficiIntestazioneBilancio

        Public Property CodicePaziente As String
        Public Property Cognome As String
        Public Property Nome As String
        Public Property Sesso As String
        Public Property CodiceFiscale As String
        Public Property TesseraSanitaria As String

        Public Property DataNascita As DateTime?
        Public Property CodiceComuneNascita As String
        Public Property DescrizioneComuneNascita As String
        Public Property ProvinciaNascita As String
        Public Property CapNascita As String

        Public Property IndirizzoResidenza As String
        Public Property CodiceComuneResidenza As String
        Public Property DescrizioneComuneResidenza As String
        Public Property ProvinciaResidenza As String
        Public Property CapResidenza As String

        Public Property IndirizzoDomicilio As String
        Public Property CodiceComuneDomicilio As String
        Public Property DescrizioneComuneDomicilio As String
        Public Property ProvinciaDomicilio As String
        Public Property CapDomicilio As String

        Public Property CodiceMedico As String
        Public Property CognomeMedico As String
        Public Property NomeMedico As String
        Public Property CodiceFiscaleMedico As String
        Public Property CodiceRegionaleMedico As String

        Public Property CodiceCittadinanza As String
        Public Property StatoCittadinanza As String

        Public Property Padre As String
        Public Property Madre As String

    End Class

End Namespace