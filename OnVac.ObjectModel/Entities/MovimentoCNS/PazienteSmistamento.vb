
Namespace Entities.MovimentoCNS

    <Serializable()>
    Public Class PazienteSmistamento

        Private _CodicePaziente As Integer
        Public Property CodicePaziente() As Integer
            Get
                Return _CodicePaziente
            End Get
            Set(ByVal value As Integer)
                _CodicePaziente = value
            End Set
        End Property

        Private _StatoAnagraficoPrecedente As Enumerators.StatoAnagrafico?
        Public Property StatoAnagraficoPrecedente() As Enumerators.StatoAnagrafico?
            Get
                Return _StatoAnagraficoPrecedente
            End Get
            Set(ByVal value As Enumerators.StatoAnagrafico?)
                _StatoAnagraficoPrecedente = value
            End Set
        End Property

        Private _StatoAnagraficoCorrente As Enumerators.StatoAnagrafico?
        Public Property StatoAnagraficoCorrente() As Enumerators.StatoAnagrafico?
            Get
                Return _StatoAnagraficoCorrente
            End Get
            Set(ByVal value As Enumerators.StatoAnagrafico?)
                _StatoAnagraficoCorrente = value
            End Set
        End Property

        Private _CodiceConsultorioPrecedente As String
        Public Property CodiceConsultorioPrecedente() As String
            Get
                Return _CodiceConsultorioPrecedente
            End Get
            Set(ByVal value As String)
                _CodiceConsultorioPrecedente = value
            End Set
        End Property

        Private _CodiceConsultorioCorrente As String
        Public Property CodiceConsultorioCorrente() As String
            Get
                Return _CodiceConsultorioCorrente
            End Get
            Set(ByVal value As String)
                _CodiceConsultorioCorrente = value
            End Set
        End Property

    End Class

End Namespace