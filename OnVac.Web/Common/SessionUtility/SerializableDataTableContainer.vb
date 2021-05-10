Imports System.Runtime.Serialization
Imports System.Data

' La classe è stata fatta per essere utilizzata dal codice preesistente senza apportare modifiche "radicali", 
' al fine di poter configurare la session "out of proc": questo non era possibile in quanto alcune maschere
' filtrano/sortano il defaultview del datatable e la serializzazione non includerebbe tali informazioni.
<Serializable()> _
Public Class SerializableDataTableContainer
    Implements ISerializable

    Public Data As DataTable

    Public Sub New(ByVal data As DataTable)
        Me.Data = Data
    End Sub

    Public Sub New()

    End Sub

    Protected Sub New(ByVal si As SerializationInfo, ByVal ctx As StreamingContext)
        Me.Data = si.GetValue("Data", GetType(DataTable))
        If Not Me.Data Is Nothing Then
            Me.Data.DefaultView.RowFilter = si.GetString("RowFilter")
            Me.Data.DefaultView.RowStateFilter = si.GetValue("RowStateFilter", GetType(DataViewRowState))
            Me.Data.DefaultView.Sort = si.GetString("Sort")
        End If
    End Sub


    Public Sub GetObjectData(ByVal info As SerializationInfo, ByVal context As StreamingContext) Implements System.Runtime.Serialization.ISerializable.GetObjectData
        info.AddValue("Data", Me.Data, GetType(DataTable))
        If Not Me.Data Is Nothing Then
            info.AddValue("RowFilter", Me.Data.DefaultView.RowFilter)
            info.AddValue("RowStateFilter", Me.Data.DefaultView.RowStateFilter, GetType(DataViewRowState))
            info.AddValue("Sort", Me.Data.DefaultView.Sort)
        End If
    End Sub


End Class
