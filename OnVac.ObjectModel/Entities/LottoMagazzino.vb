Namespace Entities

    <Serializable()> _
    Public Class LottoMagazzino

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

        Private _dataPreparazione As DateTime
        Public Property DataPreparazione() As DateTime
            Get
                Return _dataPreparazione
            End Get
            Set(ByVal value As DateTime)
                _dataPreparazione = value
            End Set
        End Property

        Private _dataScadenza As DateTime
        Public Property DataScadenza() As DateTime
            Get
                Return _dataScadenza
            End Get
            Set(ByVal value As DateTime)
                _dataScadenza = value
            End Set
        End Property

        Private _ditta As String
        Public Property Ditta() As String
            Get
                Return _ditta
            End Get
            Set(ByVal value As String)
                _ditta = value
            End Set
        End Property

        Private _dosiScatola As Integer
        Public Property DosiScatola() As Integer
            Get
                Return _dosiScatola
            End Get
            Set(ByVal value As Integer)
                _dosiScatola = value
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

        Private _obsoleto As Boolean
        Public Property Obsoleto() As Boolean
            Get
                Return _obsoleto
            End Get
            Set(ByVal value As Boolean)
                _obsoleto = value
            End Set
        End Property

        Private _codiceNomeCommerciale As String
        Public Property CodiceNomeCommerciale() As String
            Get
                Return _codiceNomeCommerciale
            End Get
            Set(ByVal value As String)
                _codiceNomeCommerciale = value
            End Set
        End Property

        Private _descrizioneNomeCommerciale As String
        Public Property DescrizioneNomeCommerciale() As String
            Get
                Return _descrizioneNomeCommerciale
            End Get
            Set(ByVal value As String)
                _descrizioneNomeCommerciale = value
            End Set
        End Property

        Private _sesso As String
        Public Property Sesso() As String
            Get
                Return _sesso
            End Get
            Set(ByVal value As String)
                _sesso = value
            End Set
        End Property

        ' N.B. : dall'anagrafica del nome commerciale
        Private _etaInizio As Integer?
        Public Property EtaInizio() As Integer?
            Get
                Return _etaInizio
            End Get
            Set(ByVal value As Integer?)
                _etaInizio = value
            End Set
        End Property

        ' N.B. : dall'anagrafica del nome commerciale
        Private _etaFine As Integer?
        Public Property EtaFine() As Integer?
            Get
                Return _etaFine
            End Get
            Set(ByVal value As Integer?)
                _etaFine = value
            End Set
        End Property

        Private _codiceAssociazione As String
        Public Property CodiceAssociazione() As String
            Get
                Return _codiceAssociazione
            End Get
            Set(ByVal value As String)
                _codiceAssociazione = value
            End Set
        End Property

        Private _codiceFornitore As String
        Public Property CodiceFornitore() As String
            Get
                Return _codiceFornitore
            End Get
            Set(ByVal value As String)
                _codiceFornitore = value
            End Set
        End Property

        Private _descrizioneFornitore As String
        Public Property DescrizioneFornitore() As String
            Get
                Return _descrizioneFornitore
            End Get
            Set(ByVal value As String)
                _descrizioneFornitore = value
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

        Private _dataMovimento As DateTime
        Public Property DataMovimento() As DateTime
            Get
                Return _dataMovimento
            End Get
            Set(ByVal value As DateTime)
                _dataMovimento = value
            End Set
        End Property

        Private _idUtenteModificaFlagAttivo As Integer?
        Public Property IdUtenteModificaFlagAttivo() As Integer?
            Get
                Return _idUtenteModificaFlagAttivo
            End Get
            Set(ByVal value As Integer?)
                _idUtenteModificaFlagAttivo = value
            End Set
        End Property

        Private _dataModificaFlagAttivo As DateTime?
        Public Property DataModificaFlagAttivo() As DateTime?
            Get
                Return _dataModificaFlagAttivo
            End Get
            Set(ByVal value As DateTime?)
                _dataModificaFlagAttivo = value
            End Set
        End Property

        Private _idUtenteModificaFlagObsoleto As Integer?
        Public Property IdUtenteModificaFlagObsoleto() As Integer?
            Get
                Return _idUtenteModificaFlagObsoleto
            End Get
            Set(ByVal value As Integer?)
                _idUtenteModificaFlagObsoleto = value
            End Set
        End Property

        Private _dataModificaFlagObsoleto As DateTime?
        Public Property DataModificaFlagObsoleto() As DateTime?
            Get
                Return _dataModificaFlagObsoleto
            End Get
            Set(ByVal value As DateTime?)
                _dataModificaFlagObsoleto = value
            End Set
        End Property

        Private _etaMinimaAttivazione As Integer?
        Public Property EtaMinimaAttivazione() As Integer?
            Get
                Return _etaMinimaAttivazione
            End Get
            Set(value As Integer?)
                _etaMinimaAttivazione = value
            End Set
        End Property

        Private _etaMassimaAttivazione As Integer?
        Public Property EtaMassimaAttivazione() As Integer?
            Get
                Return _etaMassimaAttivazione
            End Get
            Set(value As Integer?)
                _etaMassimaAttivazione = value
            End Set
        End Property

        ' N.B. : serve per gestire lo user control InsDatiLotto
        Private _quantitaIniziale As Integer
        Public Property QuantitaIniziale() As Integer
            Get
                Return _quantitaIniziale
            End Get
            Set(value As Integer)
                _quantitaIniziale = value
            End Set
        End Property

        ' N.B. : serve per gestire lo user control InsDatiLotto
        Private _unitaMisura As Enumerators.UnitaMisuraLotto
        Public Property UnitaMisura() As Enumerators.UnitaMisuraLotto
            Get
                Return _unitaMisura
            End Get
            Set(value As Enumerators.UnitaMisuraLotto)
                _unitaMisura = value
            End Set
        End Property

    End Class

End Namespace