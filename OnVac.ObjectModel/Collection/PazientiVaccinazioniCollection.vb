Namespace Collection


    <Serializable()> _
    Public Class PazientiVaccinazioniCollection
        Inherits CollectionBase

        Default Public ReadOnly Property Item(ByVal index As Integer) As Entities.PazientiVaccinazioni
            Get
                Return CType(List.Item(index), PazientiVaccinazioni)
            End Get
        End Property

        Public Function Add(ByVal value As Entities.PazientiVaccinazioni) As Integer
            Return List.Add(value)
        End Function

        Public Sub Remove(ByVal value As Entities.PazientiVaccinazioni)
            List.Remove(value)
        End Sub

    End Class


End Namespace