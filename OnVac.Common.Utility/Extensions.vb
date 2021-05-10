Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.IO

Public Module Extensions

    ' ''' <summary>
    ' ''' Restituisce un nuovo oggetto, copia dell'oggetto di partenza.
    ' ''' </summary>
    ' ''' <typeparam name="T"></typeparam>
    ' ''' <param name="source"></param>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    '<System.Runtime.CompilerServices.Extension()> _
    'Public Function Clone(Of T)(ByVal source As T) As T
    '    If Not GetType(T).IsSerializable Then
    '        Throw New ArgumentException("The type must be serializable.", "source")
    '    End If
    '    ' Don't serialize a null object, simply return the default for that object
    '    If Object.ReferenceEquals(source, Nothing) Then
    '        Return Nothing
    '    End If
    '    Dim formatter As IFormatter = New BinaryFormatter
    '    Using ms As New System.IO.MemoryStream
    '        formatter.Serialize(ms, source)
    '        ms.Seek(0, SeekOrigin.Begin)
    '        Return DirectCast(formatter.Deserialize(ms), T)
    '    End Using
    'End Function


End Module

