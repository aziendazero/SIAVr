Imports System.Collections.Generic

Namespace Entities

    <Serializable>
    Public Class PazienteElaborazione

        Public Property Progressivo As Long
        Public Property CodiceProcedura As Integer
        Public Property IdJob As Long
        Public Property CodicePaziente As String
        Public Property CodicePazienteAlias As String
        Public Property DataRichiesta As DateTime?
        Public Property IdUtenteRichiesta As Long?
        Public Property Parametri As String
        Public Property DataConvocazione As DateTime?

        Public Property DataEsecuzione As DateTime?
        Public Property Stato As String
        Public Property CodiceErrore As Integer?
        Public Property DescrizioneErrore As String

    End Class

End Namespace