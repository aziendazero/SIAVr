Imports System.Collections.Generic

Namespace Entities

    <Serializable>
    Public Class LuoghiEsecuzioneVaccinazioni

        Public Property Codice As String
        Public Property Descrizione As String
        Public Property Tipo As String
        Public Property Ordine As Integer?
        Public Property Obsoleto As String
        Public Property FlagEstraiAvn As String
        Public Property IdCampiObbligatori As List(Of String)
        Public Property IsDefaultConsultorio As Boolean

    End Class

    <Serializable>
    Public Class LuoghiEsecuzioneVaccCommand

        Public Property Codice As String
        Public Property Descrizione As String
        Public Property Tipo As String
        Public Property Ordine As Integer?
        Public Property Obsoleto As String
        Public Property FlagEstraiAvn As String

        Public Property CodiceMaxLength As Integer
        Public Property DescrizioneMaxLength As Integer
        Public Property OrdineMaxLength As Integer

    End Class

    <Serializable>
    Public Class CampoObbligatorio

        Public Property CodCampo As String
        Public Property DescrizCampo As String

        Public Sub New(codice As String, descriz As String)
            CodCampo = codice
            DescrizCampo = descriz
        End Sub

    End Class

    <Serializable>
    Public Class CampoObbligLuogoVacc

        Public Property CodLuogo As String
        Public Property CodCampo As String
        Public Property DataInizioObblig As DateTime?
        Public Property IdUteInserimento As Integer
        Public Property DataInserimento As DateTime
    End Class

    <Serializable>
    Public Class TipoErogatore

        Public Property CodCampo As String
        Public Property DescrizCampo As String

        Public Sub New(codice As String, descriz As String)
            CodCampo = codice
            DescrizCampo = descriz
        End Sub

    End Class

End Namespace
