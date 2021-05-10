Namespace MotiviRifiuto
    Public Class RifiutoNoAppuntamentoTrovato
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
                Return "Non sono stati trovati appuntamenti nel periodo di tempo specificato."
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
