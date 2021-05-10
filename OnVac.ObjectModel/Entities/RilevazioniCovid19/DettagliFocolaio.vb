Imports System.Collections.Generic

Namespace Entities
    Public Class DettagliFocolaio
        Public Property CodiceFocolaio As Integer
        Public Property Descrizione As String
        Public Property Scolastico As Boolean
        Public Property Plesso As String
        Public Property Istituto As String
        Public Property GiorniChiusura As Integer?
        Public Property InformazioniEpisodi As IEnumerable(Of InfoEpisodioFocolaio)


        Public Class InfoEpisodioFocolaio
            Public Property CodiceEpisodio As Long
            Public Property NomePaziente As String
            Public Property CognomePaziente As String
            Public Property DataDiNascita As Date?
            Public Property CodiceFiscale As String
        End Class
    End Class

    Public Class CreaFocolaio
        Public Property Descrizione As String
        Public Property Scolastico As Boolean
        Public Property Plesso As String
        Public Property Istituto As String
        Public Property GiorniChiusura As Integer?
        Public Property Episodi As IEnumerable(Of Long)
    End Class
End Namespace
