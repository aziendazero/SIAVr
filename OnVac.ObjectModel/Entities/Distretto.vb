Namespace Entities

    <Serializable>
    Public Class Distretto

        Public Property Codice As String
        Public Property Descrizione As String
        Public Property CodiceEsterno As String
        Public Property CodiceComune As String
        Public Property CodiceUlss As String
        Public ReadOnly Property DesUlssDistretto As String
            Get
                Dim ulss As String = String.Empty
                If Not String.IsNullOrEmpty(CodiceUlss) Then
                    ulss = String.Format(" - {0}", CodiceUlss)
                End If
                Return String.Format("{0}{1}", Descrizione, ulss).TrimStart()
            End Get
        End Property
    End Class
    Public Class DistrettoDDL

        Public Property Codice As String
        Public Property Descrizione As String
    End Class



End Namespace

