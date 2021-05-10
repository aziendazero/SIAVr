Namespace MotiviRifiuto

    Public Class RifiutoAmbulatorioNonInConsultorio
        Implements IMotivoRifiuto

        Private _Commento As String
        Public Property Commento() As String Implements IMotivoRifiuto.Commento
            Get
                Return _Commento
            End Get
            Set(Value As String)
                _Commento = Value
            End Set
        End Property

        Public ReadOnly Property MessaggioStandard() As String Implements IMotivoRifiuto.MessaggioStandard
            Get
                Return "L'ambulatorio specificato non fa parte del consultorio."
            End Get
        End Property

        Public Overrides Function ToString() As String Implements IMotivoRifiuto.ToString
            If Not String.IsNullOrEmpty(_Commento) Then
                Return MessaggioStandard & Environment.NewLine & _Commento
            Else
                Return MessaggioStandard
            End If
        End Function

    End Class

End Namespace