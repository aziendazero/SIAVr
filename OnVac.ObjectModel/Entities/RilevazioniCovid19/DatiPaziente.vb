Namespace Entities

    <Serializable()>
    Public Class DatiAnagraficiCommon
        Public Property Cognome As String
        Public Property Nome As String
        Public Property Cf As String
        Public Property DataNascita As DateTime?
        Public Property ComuneResidenza As String
    End Class

    <Serializable()>
    Public Class DatiPaziente
        Inherits DatiAnagraficiCommon
        Public Property Codice As Long
        Public Property Sesso As String
        Public Property ComuneNascita As String
        Public Property IndirizzoResidenza As String
        Public Property ComuneDomicilio As String
        Public Property IndirizzoDomicilio As String
        Public Property TelefonoUno As String
        Public Property TelefonoDue As String
        Public Property TelefonoTre As String
        Public Property Email As String
        Public Property NomeMedico As String
        Public Property CognomeMedico As String
        Public Property CfMedico As String
    End Class

    'Classe che rappresenta un paziente creatore di un contatto con un altro paziente
    <Serializable()>
    Public Class DatiPazienteCreatoreContatto
        Inherits DatiAnagraficiCommon
        Public Property DataInserimentoContatto As DateTime?
        Public Property CodicePaziente As Long
    End Class
End Namespace
