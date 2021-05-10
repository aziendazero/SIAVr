Imports System.Collections.Generic
Imports System.Collections.ObjectModel

Namespace Entities

    <Serializable()>
    Public Class StatVaccinazioneProgrammateAssociate

        Public Property DataConvocazione As DateTime
        Public Property CodicePaziente As Int64
        Public Property CodiceConsultorio As String
        Public Property ListaVacProgrammate As List(Of String)
        Public Property ListaAssProgrammate As List(Of String)
    End Class

    <Serializable()>
    Public Class StatVacciniAssociati
        Public Property CodiceAssociazione As String
        Public Property DescrizioneAssociazione As String
        Public Property DefaultAssociazione As String
        Public Property Obsoleta As String
        Public Property ListaVaccini As List(Of String)
        Public ReadOnly Property CountVac As Int64
            Get
                Return ListaVaccini.Count
            End Get
        End Property
    End Class
    <Serializable()>
    Public Class StatVacciniAssociatiFinale
        Public Property CodiceAssociazione As String
        Public Property DescrizioneAssociazione As String
        Public Property CodiceConsultorio As String
        Public Property NumeroAssociazioniConsultorio As Int64
    End Class

    <Serializable()>
    Public Class StatVacciniAssociatiControllo
        Public Property CodiceAssociazione As String
        Public Property DescrizioneAssociazione As String
        Public Property DefaultAssociazione As String
        Public Property CountVac As Int64
    End Class

End Namespace