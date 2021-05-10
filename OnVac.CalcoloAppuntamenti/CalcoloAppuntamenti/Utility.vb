Imports System.IO
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters.Binary

Public Class Utility

    Public Shared Function CloneObject(obj As Object) As Object

        ' Crea un memory stream e un formatter.
        Dim ms As New MemoryStream()
        Dim bf As New BinaryFormatter(Nothing, New StreamingContext(StreamingContextStates.Clone))

        ' Serializza l’oggetto nello stream.
        bf.Serialize(ms, obj)

        ms.Seek(0, SeekOrigin.Begin)

        ' Lo deserializza in un oggetto differente e rilascia la memoria.
        Dim res As Object = bf.Deserialize(ms)
        ms.Close()

        Return res

    End Function

End Class
