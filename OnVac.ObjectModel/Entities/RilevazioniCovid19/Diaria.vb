Imports System.Collections.Generic

Namespace Entities
    Public Class SituazioneDiaria
        Public codiceDiaria As Long
        Public note As String
        Public sintomi As List(Of SituazioneSintomo)

        Public Class SituazioneSintomo
            Public codiceSintomo As Integer
            Public codiceLink As Long
        End Class

        Public ReadOnly Property SintomiEsistenti As IEnumerable(Of Integer)
            Get
                Return sintomi.Select(Function(x)
                                          Return x.codiceSintomo
                                      End Function)
            End Get
        End Property

    End Class

    <Serializable()>
    Public Class Diaria
        Public Property CodiceDiaria As Long
        Public Property CodiceRilevazioneUsl As String
        Public Property DataRilevazione As DateTime?
        Public Property Asintomatico As Boolean
        Public Property Note As String
        Public Property UtenteInserimento As Long
        Public Property CodiceInserimentoUsl As String
        Public Property DataInserimento As DateTime
        Public Property UtenteModifica As Long?
        Public Property DataModifica As DateTime?
        Public Property RispostaTelefono As Boolean
        Public Property Sintomi As List(Of Sintomo)
        Public Property CodiceEpisodio As Long

        Public Sub New()
            Sintomi = New List(Of Sintomo)
        End Sub
    End Class

    <Serializable()>
    Public Class Sintomo
        Public Property idSintomo As Long
        Public Property CodiceSintomo As Integer
        Public Property Note As String
        Public Property UtenteInserimento As Long?
        Public Property CodiceInserimentoUsl As String
        Public Property DataInserimento As DateTime?
        Public Property Descrizione As String
        Public Property Ordine As Integer
    End Class
End Namespace
