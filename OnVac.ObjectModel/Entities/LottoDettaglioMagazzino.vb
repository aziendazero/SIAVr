Namespace Entities

    <Serializable()> _
    Public Class LottoDettaglioMagazzino

        Private _codiceLotto As String
        Public Property CodiceLotto() As String
            Get
                Return _codiceLotto
            End Get
            Set(ByVal value As String)
                _codiceLotto = value
            End Set
        End Property

        Private _descrizioneLotto As String
        Public Property DescrizioneLotto() As String
            Get
                Return _descrizioneLotto
            End Get
            Set(ByVal value As String)
                _descrizioneLotto = value
            End Set
        End Property

        Private _codiceConsultorio As String
        Public Property CodiceConsultorio() As String
            Get
                Return _codiceConsultorio
            End Get
            Set(ByVal value As String)
                _codiceConsultorio = value
            End Set
        End Property

        Private _descrizioneConsultorio As String
        Public Property DescrizioneConsultorio() As String
            Get
                Return _descrizioneConsultorio
            End Get
            Set(ByVal value As String)
                _descrizioneConsultorio = value
            End Set
        End Property

        Private _dosiRimaste As Integer
        Public Property DosiRimaste() As Integer
            Get
                Return _dosiRimaste
            End Get
            Set(ByVal value As Integer)
                _dosiRimaste = value
            End Set
        End Property

        Private _quantitaMinima As Integer
        Public Property QuantitaMinima() As Integer
            Get
                Return _quantitaMinima
            End Get
            Set(ByVal value As Integer)
                _quantitaMinima = value
            End Set
        End Property

        Private _attivo As Boolean
        Public Property Attivo() As Boolean
            Get
                Return _attivo
            End Get
            Set(ByVal value As Boolean)
                _attivo = value
            End Set
        End Property

    End Class

End Namespace
