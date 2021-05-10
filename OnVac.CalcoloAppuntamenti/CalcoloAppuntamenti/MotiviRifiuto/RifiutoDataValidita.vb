Namespace MotiviRifiuto
    Public Class RifiutoDataValidita
        Implements IMotivoRifiuto

        Private _Commento As String
        Public Property Commento() As String Implements IMotivoRifiuto.Commento
            Get
                Return _Commento
            End Get
            Set(ByVal Value As String)
                _Commento = Value
            End Set
        End Property

        Public ReadOnly Property MessaggioStandard() As String Implements IMotivoRifiuto.MessaggioStandard
            Get
                Return "La ricerca appuntamento non è riuscita a trovare un appuntamento per via del filtro di data validità."
            End Get
        End Property

        Public Overrides Function ToString() As String Implements IMotivoRifiuto.ToString
            If _Commento <> "" Then
                Return MessaggioStandard & vbCrLf & _Commento
            Else
                Return MessaggioStandard
            End If
        End Function
    End Class
End Namespace
