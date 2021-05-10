Namespace Entities

    <Serializable>
    Public Class AttivitaRegistrazioneFilter

        Public Property OrderBy As Entities.AttivitaRegistrazione.Ordinamento?
        Public Property Descending As Boolean?
        Public Property FiltroLibero As String
        Public Property filtroDataRegistrazioneInizio As String
        Public Property filtroDataRegistrazioneFine As String
        Public Property filtroDataEsecuzioneInizio As String
        Public Property filtroDataEsecuzioneFine As String
        Public Property filtroIdUtente As Long?
        Public Property filtroCodiceUtente As String

    End Class

End Namespace