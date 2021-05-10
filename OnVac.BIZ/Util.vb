Imports System.Collections.Generic

Public Module BizUtil

    <System.Runtime.CompilerServices.Extension()>
    Public Function DistinctBy(Of T, TComparer)(ByRef elenco As IEnumerable(Of T), comparer As Func(Of T, TComparer)) As IEnumerable(Of T) 
        Dim ritorno As New List(Of T)

        Dim comp As IEnumerable(Of TComparer) = ritorno.Select(Function(el)
                                                                   Return comparer(el)
                                                               End Function)

        For Each el As T In elenco
            Dim a As TComparer = comparer(el)

            If Not comp.Contains(a) Then
                ritorno.Add(el)
            End If

        Next

        Return ritorno
    End Function
End Module
