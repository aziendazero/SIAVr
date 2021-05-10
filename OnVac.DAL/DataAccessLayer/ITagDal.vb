Imports System.Collections.Generic

Public Interface ITagDal
    Function CercaTag(gruppo As String, filtro As String) As IEnumerable(Of Tag)
    Function GetTags(elementi As IEnumerable(Of Long)) As IEnumerable(Of Tag)
End Interface
