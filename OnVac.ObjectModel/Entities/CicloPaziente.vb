Namespace Entities

    <Serializable()> _
    Public Class CicloPaziente

        Private _codicePaziente As Int32
        Public Property CodicePaziente() As Int32
            Get
                Return _codicePaziente
            End Get
            Set(ByVal value As Int32)
                _codicePaziente = value
            End Set
        End Property

        Private _codicePazienteAlias As Nullable(Of Int32)
        Public Property CodicePazienteAlias() As Nullable(Of Int32)
            Get
                Return _codicePazienteAlias
            End Get
            Set(ByVal value As Nullable(Of Int32))
                _codicePazienteAlias = value
            End Set
        End Property

        Private _codiceCiclo As String
        Public Property CodiceCiclo() As String
            Get
                Return _codiceCiclo
            End Get
            Set(ByVal value As String)
                _codiceCiclo = value
            End Set
        End Property

        Private _descrizioneCiclo As String
        Public Property DescrizioneCiclo() As String
            Get
                Return _descrizioneCiclo
            End Get
            Set(ByVal value As String)
                _descrizioneCiclo = value
            End Set
        End Property

        Private _standard As String
        Public Property Standard() As String
            Get
                Return _standard
            End Get
            Set(ByVal value As String)
                _standard = value
            End Set
        End Property

    End Class


End Namespace