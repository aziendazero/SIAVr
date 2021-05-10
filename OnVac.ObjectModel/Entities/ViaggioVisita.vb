Imports System.Collections.Generic

Namespace Entities

    <Serializable()>
    Public Class ViaggioVisita

        Public Property Id() As Int64
        Public Property IdVisita() As Int64
        Public Property DataInizioViaggio() As DateTime
        Public Property DataFineViaggio() As DateTime
        Public Property CodicePaese() As String
        Public Property DescPaese As String
        Public Property Operazione As OperazioneViaggio

    End Class
    Public Enum OperazioneViaggio
        Update
        Insert
        Delete
    End Enum

End Namespace
