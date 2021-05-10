Imports System.Data



Public Class DataSetHelper
    Implements IDisposable

    Private _ds As Data.DataSet


    Public Sub setDataSet(ByRef _DataSet As Data.DataSet)
        _ds = _DataSet
    End Sub


    Public Function getDataSet() As Data.DataSet
        Return _ds
    End Function


    Public Sub removeDataSet()
        _ds.Dispose()
    End Sub


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Conteggio di tutte le righe non cancellate
    ''' </summary>
    ''' <param name="SourceTable"></param>
    ''' <param name="Flag"></param>
    ''' <history>
    ''' 	[pmontevecchi]	11/10/2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function RealCount(ByVal SourceTable As DataTable, ByVal Flag As String) As Integer
        Dim i As Integer
        Dim counter As Integer = 0

        If (SourceTable Is Nothing) Then Return 0

        For i = 0 To SourceTable.Rows.Count - 1

            If (SourceTable.Rows(i).RowState <> DataRowState.Deleted) Then
                counter += 1
            End If

        Next

        Return counter
    End Function


    Private Function ColumnEqual(ByVal A As Object, ByVal B As Object) As Boolean
        ' Compares two values to see if they are equal. Also compares DBNULL.Value.
        ' Note: If your DataTable contains object fields, then you must extend this
        ' function to handle them in a meaningful way if you intend to group on them.

        If (A Is DBNull.Value And B Is DBNull.Value) Then 'both are DBNull.Value
            Return True
        End If

        If (A Is DBNull.Value Or B Is DBNull.Value) Then ' only one is DBNull.Value
            Return False
        End If

        Return (A.Equals(B))    'value type standard comparison
    End Function


    Public Function SelectDistinct(ByVal TableName As String, ByVal SourceTable As DataTable, ByVal FieldName As String) As DataTable
        Dim dt As New DataTable(TableName)
        Dim dt_Row As DataRow

        dt.Columns.Add(FieldName, SourceTable.Columns(FieldName).DataType)

        Dim LastValue As Object
        LastValue = DBNull.Value

        For Each dr As DataRow In SourceTable.Select("", FieldName)

            If (LastValue Is DBNull.Value Or Not (ColumnEqual(LastValue, dr(FieldName)))) Then

                dt_Row = dt.NewRow()

                dt_Row(FieldName) = dr(FieldName)
                dt.Rows.Add(dt_Row)

                LastValue = dr(FieldName)
                'dt.Rows.Add(LastValue)

            End If

        Next

        If (Not _ds Is DBNull.Value) Then
            _ds.Tables.Add(dt)

            Return dt
        End If

        Return Nothing
    End Function


    ''' <summary>
    ''' Pone a zero tutti i valori con DBNull presenti del datatable in ingresso
    ''' </summary>
    ''' <param name="dt"> DataTable in ingresso</param>
    ''' <remarks> Se il DataTable restituito è corretto, chiamare il metodo AcceptChanges. Nel caso di 
    ''' eccezione nel cast, viene conservato il valore DBNull  </remarks>
    Public Sub DBNull2Zero(ByRef dt As DataTable)
        Dim int As Integer = 0
        DBNull2Value(dt, int)
    End Sub


    ''' <summary>
    ''' Pone a stringa vuota tutti i valori con DBNull presenti del datatable in ingresso
    ''' </summary>
    ''' <param name="dt"> DataTable in ingresso</param>
    ''' <remarks> Se il DataTable restituito è corretto, chiamare il metodo AcceptChanges. Nel caso di 
    ''' eccezione nel cast, viene conservato il valore DBNull  </remarks>
    Public Sub DBNull2EmptyString(ByRef dt As DataTable)
        Dim str As String = ""
        DBNull2Value(dt, str)
    End Sub

    ''' <summary>
    ''' Pone a val tutti i valori con DBNull presenti del datatable passato per riferimento in ingresso
    ''' </summary>
    ''' <param name="dt"> DataTable in ingresso</param>
    ''' <param name="v"> Valore da porre al posto dei DBNull</param>
    ''' <remarks> Se il DataTable restituito è corretto, chiamare il metodo AcceptChanges. Nel caso di 
    ''' eccezione nel cast, viene conservato il valore DBNull  </remarks>
    Public Sub DBNull2Value(ByRef dt As DataTable, ByVal v As String)
        Dim i, j As Integer

        For j = 0 To dt.Columns.Count - 1
            For i = 0 To dt.Rows.Count - 1
                If dt.Rows(i)(j) Is System.DBNull.Value Then
                    Dim oldObj As Object = dt.Rows(i)(j)
                    Try
                        dt.Rows(i)(j) = CType(v, String)
                    Catch ex As Exception
                        dt.Rows(i)(j) = oldObj
                    End Try
                End If
            Next
        Next
    End Sub

    ''' <summary>
    ''' Pone a val tutti i valori con DBNull presenti del datatable passato per riferimento in ingresso
    ''' </summary>
    ''' <param name="dt"> DataTable in ingresso</param>
    ''' <param name="v"> Valore da porre al posto dei DBNull</param>
    ''' <remarks> Se il DataTable restituito è corretto, chiamare il metodo AcceptChanges. Nel caso di 
    ''' eccezione nel cast, viene conservato il valore DBNull  </remarks>
    Public Sub DBNull2Value(ByRef dt As DataTable, ByVal v As Integer)
        Dim i, j As Integer

        For j = 0 To dt.Columns.Count - 1
            For i = 0 To dt.Rows.Count - 1
                If dt.Rows(i)(j) Is System.DBNull.Value Then
                    Dim oldObj As Object = dt.Rows(i)(j)
                    Try
                        dt.Rows(i)(j) = CType(v, Integer)
                    Catch ex As Exception
                        dt.Rows(i)(j) = oldObj
                    End Try
                End If
            Next
        Next
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If (Not _ds Is Nothing) Then _ds.Dispose()
        GC.SuppressFinalize(Me)
    End Sub

End Class

