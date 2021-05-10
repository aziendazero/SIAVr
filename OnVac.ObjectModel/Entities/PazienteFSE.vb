Imports System.Collections.Generic

Namespace Entities

    <Serializable>
    Public Class PazienteFSE

        Public Property CodicePaziente As Integer
        Public Property CodiceRegionale As String
        Public Property CodiceAusiliario As String
        Public Property Cognome As String
        Public Property Nome As String
        Public Property Sesso As String
        Public Property CodiceFiscale As String
        Public Property TesseraSanitaria As String
        Public Property CodiceCnsCorrente As String
        Public Property CodiceUslCorrente As String
        Public Property DescrizioneUslCorrente As String

        Public Property DataNascita As DateTime?
        Public Property CodiceComuneNascita As String
        Public Property DescrizioneComuneNascita As String
        Public Property ProvinciaNascita As String
        Public Property CodiceIstatComuneNascita As String
        Public Property CodiceStatoNascita As String

        Public Property IndirizzoResidenza As String
        Public Property CodiceComuneResidenza As String
        Public Property DescrizioneComuneResidenza As String
        Public Property ProvinciaResidenza As String
        Public Property CapResidenza As String
        Public Property CodiceIstatComuneResidenza As String
        Public Property CodiceStatoResidenza As String

        Public Property IndirizzoDomicilio As String
        Public Property CodiceComuneDomicilio As String
        Public Property DescrizioneComuneDomicilio As String
        Public Property ProvinciaDomicilio As String
        Public Property CapDomicilio As String
        Public Property CodiceIstatComuneDomicilio As String
        Public Property CodiceStatoDomicilio As String

        Public Property CodiceCittadinanza As String
        Public Property StatoCittadinanza As String
        Public Property CodiceIsoCittadinanza As String

        Public Property Padre As String
        Public Property Madre As String
        Public Property IdAcn As Integer?

        Public Property StatoAnagrafico As Enumerators.StatoAnagrafico?
        Public Property CategoriaCittadino As String
        Public Property Telefono1 As String
        Public Property Telefono2 As String
        Public Property Telefono3 As String
        Public Property UslAssistenza_Codice As String
        Public Property UslResidenza_Codice As String
        Public Property TipoPaziente As Enumerators.TipoPaziente?

    End Class



    <Serializable>
    Public Class StrutturaFSE

        Public Property CodiceStruttura As String
        Public Property Descrizione As String
        Public Property Oid As String
        Public Property TipoStruttura As Enumerators.TipoStruttura?

    End Class


End Namespace
