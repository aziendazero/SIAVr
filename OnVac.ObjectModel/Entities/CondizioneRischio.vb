Namespace Entities

    <Serializable>
    Public Class CondizioneRischio
        Public Property CodiceCategoriaRischio As String
        Public Property DescrizioneCategoriaRischio As String
        Public Property CodiceVaccinazione As String
    End Class

    Public Class CondRischio_Vac

        Public Property CodiceCondRischio As String
        Public Property DescrCondRischio As String
        Public Property CodVaccinazione As String
        Public Property DefaultCondRischio As String
        Public ReadOnly Property DescrReadCondRischio As String
            Get
                Dim val As New System.Text.StringBuilder()
                val.Append(String.Format("{0}", CodiceCondRischio & " [" & DescrCondRischio & "]"))
                If DefaultCondRischio = "S" Then
                    val.Append(" (d)")
                End If
                DescrReadCondRischio = val.ToString()
            End Get
        End Property

    End Class

    Public Class CondizioneRischioCodiceDescrizione
        Public Property Codice As String
        Public Property Descrizione As String
    End Class

End Namespace
