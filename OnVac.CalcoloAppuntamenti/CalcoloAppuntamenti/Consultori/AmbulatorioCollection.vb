Public Class AmbulatorioCollection
    Inherits CollectionBase

#Region " Events "

    Public Event TrovatoAppuntamento(paz As CnvPaziente)
    Public Event NonTrovatoAppuntamento(paz As CnvPaziente)

#End Region

#Region " Properties "

    Private _AmbulatorioHash As New Hashtable()
    Default Public ReadOnly Property Item(codice As String) As Ambulatorio
        Get
            Return DirectCast(_AmbulatorioHash(codice), Ambulatorio)
        End Get
    End Property

#End Region

#Region " Public "

    Public Function Add(value As Ambulatorio) As Integer

        AddHandler value.TrovatoAppuntamento, AddressOf TrovatoAppuntamentoHandler
        AddHandler value.NonTrovatoAppuntamento, AddressOf NonTrovatoAppuntamentoHandler

        _AmbulatorioHash.Add(value.Codice, value)

        Return List.Add(value)

    End Function

    Public Function Contains(value As String) As Boolean
        Return Me._AmbulatorioHash.ContainsKey(value)
    End Function

    Public Sub Remove(value As Ambulatorio)
        Me._AmbulatorioHash.Remove(value.Codice)
        List.Remove(value)
    End Sub

#End Region

#Region " Private "

    Private Sub TrovatoAppuntamentoHandler(paz As CnvPaziente)
        RaiseEvent TrovatoAppuntamento(paz)
    End Sub

    Private Sub NonTrovatoAppuntamentoHandler(paz As CnvPaziente)
        RaiseEvent NonTrovatoAppuntamento(paz)
    End Sub

#End Region

End Class