Imports System.Collections.Generic

Namespace Entities.ArchiviazioneDIRV

    <System.Xml.Serialization.XmlTypeAttribute()>
    <System.Xml.Serialization.XmlRootAttribute("Intestazione")>
    <System.SerializableAttribute()>
    Public Class Intestazione

        Public IdMessaggio As String
        Public TmstMessaggio As String
        Public Mittente As String
        Public Servizio As String
        Public Destinatario As String
        Public UserId As String
        Public Password As String
        Public RichiestaApplicativa As RichiestaApplicativa
        Public RispostaApplicativa As RispostaApplicativa

        Public Sub New()
            Me.RichiestaApplicativa = New RichiestaApplicativa()
            Me.RispostaApplicativa = New RispostaApplicativa()
        End Sub

    End Class

    Public Class RichiestaApplicativa

        Public Allegati As List(Of Allegato)

        Public Sub New()
            Me.Allegati = New List(Of Allegato)()
        End Sub

    End Class

    Public Class RispostaApplicativa

        Public Allegati As List(Of Allegato)
        Public Messaggio As String

        Public Sub New()
            Me.Allegati = New List(Of Allegato)()
        End Sub

    End Class

    Public Class Allegato

        Public Nome As String

        Public Sub New()
        End Sub

        Public Sub New(nome As String)
            Me.Nome = nome
        End Sub

    End Class

End Namespace