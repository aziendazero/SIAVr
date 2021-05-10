
Namespace Collection


    <Serializable()> _
    Public Class PazienteCollection
        Inherits CollectionBase

        ' Hash di appoggio per recuperare il paziente in base alla chiave senza dover effettuare una ricerca completa
        Private _hash_paz As New Hashtable


        Default Public ReadOnly Property Item(ByVal indice As Integer) As Paziente
            Get
                Return CType(List.Item(indice), Paziente)
            End Get
        End Property


        Public Function Add(ByVal value As Paziente) As Integer
            Dim pos As Integer = List.Add(value)

            ' Inserisco nell'hashtable la posizione in cui è stato aggiunto l'elemento
            _hash_paz.Add(value.Paz_Codice.ToString, pos)

            Return pos
        End Function


        Public Sub Remove(ByVal value As Paziente)
            _hash_paz.Remove(value.Paz_Codice.ToString)
            List.Remove(value)
        End Sub


        Public Function FindByCodPaziente(ByVal cod_paziente As String) As Paziente
            Dim obj As Object = _hash_paz(cod_paziente)
            If obj Is Nothing Then Return Nothing

            Dim pos As Integer
            Try
                pos = CInt(obj)
            Catch ex As Exception
                Return Nothing
            End Try

            Return CType(List(pos), Paziente)

        End Function


        Public Function getDataTable() As DataTable
            Dim _dt As DataTable
            Dim i As Integer = 0

            If Count > 0 Then
                _dt = CType(List.Item(0), Paziente).getDataTable

                For i = 1 To Count - 1
                    _dt = (CType(List.Item(i), Paziente)).addRow2DT(_dt)
                Next
            Else
                _dt = New DataTable
            End If

            Return _dt

        End Function


    End Class


End Namespace
