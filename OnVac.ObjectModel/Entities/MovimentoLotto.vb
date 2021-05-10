Namespace Entities

    <Serializable()> _
    Public Class MovimentoLotto

        Private _progressivo As String
        Public Property Progressivo() As String
            Get
                Return _progressivo
            End Get
            Set(ByVal value As String)
                _progressivo = value
            End Set
        End Property

        Private _codiceLotto As String
        Public Property CodiceLotto() As String
            Get
                Return _codiceLotto
            End Get
            Set(ByVal value As String)
                _codiceLotto = value
            End Set
        End Property

        Private _numeroDosi As Integer
        Public Property NumeroDosi() As Integer
            Get
                Return _numeroDosi
            End Get
            Set(ByVal value As Integer)
                _numeroDosi = value
            End Set
        End Property

        Private _tipoMovimento As String
        Public Property TipoMovimento() As String
            Get
                Return _tipoMovimento
            End Get
            Set(ByVal value As String)
                _tipoMovimento = value
            End Set
        End Property

        Private _idUtente As Integer
        Public Property IdUtente() As Integer
            Get
                Return _idUtente
            End Get
            Set(ByVal value As Integer)
                _idUtente = value
            End Set
        End Property

        Private _dataRegistrazione As DateTime
        Public Property DataRegistrazione() As DateTime
            Get
                Return _dataRegistrazione
            End Get
            Set(ByVal value As DateTime)
                _dataRegistrazione = value
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

        Private _codiceConsultorioTrasferimento As String
        Public Property CodiceConsultorioTrasferimento() As String
            Get
                Return _codiceConsultorioTrasferimento
            End Get
            Set(ByVal value As String)
                _codiceConsultorioTrasferimento = value
            End Set
        End Property

        Private _note As String
        Public Property Note() As String
            Get
                Return _note
            End Get
            Set(ByVal value As String)
                _note = value
            End Set
        End Property

        Private _idEsecuzioneAssociazione As String
        Public Property IdEsecuzioneAssociazione() As String
            Get
                Return _idEsecuzioneAssociazione
            End Get
            Set(ByVal value As String)
                _idEsecuzioneAssociazione = value
            End Set
        End Property

    End Class

End Namespace

