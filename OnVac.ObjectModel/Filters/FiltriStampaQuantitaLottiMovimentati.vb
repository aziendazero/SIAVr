Namespace Filters

    <Serializable()>
    Public Class FiltriStampaQuantitaLottiMovimentati

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

        Private _dataInizioRegistrazione As DateTime
        Public Property DataInizioRegistrazione() As DateTime
            Get
                Return _dataInizioRegistrazione
            End Get
            Set(ByVal value As DateTime)
                _dataInizioRegistrazione = value
            End Set
        End Property

        Private _dataFineRegistrazione As DateTime
        Public Property DataFineRegistrazione() As DateTime
            Get
                Return _dataFineRegistrazione
            End Get
            Set(ByVal value As DateTime)
                _dataFineRegistrazione = value
            End Set
        End Property

        Private _idUtente As String
        Public Property IdUtente() As String
            Get
                Return _idUtente
            End Get
            Set(ByVal value As String)
                _idUtente = value
            End Set
        End Property

        Private _codiceUtente As String
        Public Property CodiceUtente() As String
            Get
                Return _codiceUtente
            End Get
            Set(ByVal value As String)
                _codiceUtente = value
            End Set
        End Property

        Private _descrizioneUtente As String
        Public Property DescrizioneUtente() As String
            Get
                Return _descrizioneUtente
            End Get
            Set(ByVal value As String)
                _descrizioneUtente = value
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

        Private _quantita As String
        Public Property Quantita() As String
            Get
                Return _quantita
            End Get
            Set(value As String)
                _quantita = value
            End Set
        End Property

        Private _operatoreConfrontoQuantita As String
        Public Property OperatoreConfrontoQuantita() As String
            Get
                Return _operatoreConfrontoQuantita
            End Get
            Set(value As String)
                _operatoreConfrontoQuantita = value
            End Set
        End Property

        Private _tipoMovimento As String
		Public Property TipoMovimento() As String
			Get
				Return _tipoMovimento
			End Get
			Set(value As String)
				_tipoMovimento = value
			End Set
		End Property

		Private _idUteConnesso As String
		Public Property IdUtenteConnesso() As String
			Get
				Return _idUteConnesso
			End Get
			Set(ByVal value As String)
				_idUteConnesso = value
			End Set
		End Property

	End Class

End Namespace
