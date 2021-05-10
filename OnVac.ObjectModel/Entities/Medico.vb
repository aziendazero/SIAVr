Namespace Entities


    Public Class Medico

        Private _codice As String
        Public Property Codice() As String
            Get
                Return _codice
            End Get
            Set(ByVal Value As String)
                _codice = Value
            End Set
        End Property

        Private _descrizione As String
        Public Property Descrizione() As String
            Get
                Return _descrizione
            End Get
            Set(ByVal Value As String)
                _descrizione = Value
            End Set
        End Property

        Private _cognome As String
        Public Property Cognome() As String
            Get
                Return _cognome
            End Get
            Set(ByVal Value As String)
                _cognome = Value
            End Set
        End Property

        Private _nome As String
        Public Property Nome() As String
            Get
                Return _nome
            End Get
            Set(ByVal Value As String)
                _nome = Value
            End Set
        End Property

        Private _codice_regionale As String
        Public Property CodiceRegionale() As String
            Get
                Return _codice_regionale
            End Get
            Set(ByVal Value As String)
                _codice_regionale = Value
            End Set
        End Property

        Private _codice_fiscale As String
        Public Property CodiceFiscale() As String
            Get
                Return _codice_fiscale
            End Get
            Set(ByVal Value As String)
                _codice_fiscale = Value
            End Set
        End Property

        Private _tipo As String
        Public Property Tipo() As String
            Get
                Return _tipo
            End Get
            Set(ByVal Value As String)
                _tipo = Value
            End Set
        End Property

        Private _descrizione_tipo As String
        Public Property DescrizioneTipo() As String
            Get
                Return _descrizione_tipo
            End Get
            Set(ByVal Value As String)
                _descrizione_tipo = Value
            End Set
        End Property

        Private _data_iscrizione As Date
        Public Property DataIscrizione() As Date
            Get
                Return _data_iscrizione
            End Get
            Set(ByVal Value As Date)
                _data_iscrizione = Value
            End Set
        End Property

        Private _data_scadenza As Date
        Public Property DataScadenza() As Date
            Get
                Return _data_scadenza
            End Get
            Set(ByVal Value As Date)
                _data_scadenza = Value
            End Set
        End Property


    End Class


End Namespace

