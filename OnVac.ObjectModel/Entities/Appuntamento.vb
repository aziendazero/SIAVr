Imports System.Collections.Generic

Namespace Entities

    <Serializable()>
    Public Class Appuntamento
        Public Property CodiceLocalePaziente As Long
        Public Property AppIdAziendaLocale As String
        Public Property DataConvocazione As DateTime
        Public Property DataAppuntamento As DateTime
        Public Property CodiceConsultorio As String
        Public Property DescrizioneConsultorio As String
        Public Property IndirizzoConsultorio As String
        Public Property ComuneConsultorio As String
        Public Property TelefonoConsultorio As String
        Public Property NoteConsultorio As String
        Public Property CodiceAmbulatorio As Integer
        Public Property DescrizioneAmbulatorio As String
        Public Property CodiceVaccinazione As String
        Public Property DescrizioneVaccinazione As String
        Public Property DoseVaccinazione As Integer
        Public Property CognomePaziente As String
        Public Property NomePaziente As String
        Public Property DataNascitaPaziente As DateTime
        Public Property IdConvocazione As String
        Public Property CodiceAssociazione As String
        Public Property DescrizioneAssociazione As String
        Public Property ImportoIndicativo As Double
        Public Property IdAppuntamento As String
        Public Property AnnullabileEsterni As Boolean
        Public Property AgendaEsposta As String
        Public Property DurataAppuntamento As Integer
        Public Property UslInserimento As String
        Public Property CodiceUslassistenza As String
    End Class

    Public Class DTOAppuntamento
        Public Property ResultMessage As String
        Public Property Data As DateTime
        Public Property IdAppuntamento As String
        Public Property Luogo As String
        Public Property CodiceVaccino As String
        Public Property DescrizioneVaccino As String

    End Class

    Public Class DTOAppuntamentoStampa

        Public Property DataAppuntamento As DateTime
        Public Property DataConvocazione As DateTime
        Public Property IdConvocazione As String
        Public Property CodicePaziente As Long
        Public Property CodiceUsl As String
    End Class

    Public Class DTORevoca
        Public Property CodicePaziente As String
        Public Property DataConvocazione As DateTime
        Public Property DataAppuntamento As DateTime
        Public Property DurataAppuntamento As Double
        Public Property CodiceConsultorioAppuntamento As String
        Public Property CodiceAmbulatorio As Integer
    End Class
End Namespace