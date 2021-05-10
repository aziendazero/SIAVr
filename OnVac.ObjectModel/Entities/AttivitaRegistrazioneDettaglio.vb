Imports System.Collections.Generic

Namespace Entities

    <Serializable>
    Public Class AttivitaRegistrazioneDettaglio

        Public Property AttivitaRegistrazioneCorrente As AttivitaRegistrazione
        Public Property RispostePossibili As List(Of Entities.AttivitaRegistrazioneRisposta)
        Public Property Valori As List(Of Entities.AttivitaRegistrazioneValore)

        Public Sub New()

            RispostePossibili = New List(Of AttivitaRegistrazioneRisposta)()
            Valori = New List(Of AttivitaRegistrazioneValore)()

        End Sub

    End Class

End Namespace
