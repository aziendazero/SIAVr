
Namespace Entities

    <Serializable()> _
    Public Class Via

        Private _Progressivo As Int32
        Public Property Progressivo() As Int32
            Get
                Return _Progressivo
            End Get
            Set(ByVal Value As Int32)
                _Progressivo = Value
            End Set
        End Property

        Private _Codice As String
        Public Property Codice() As String
            Get
                Return _Codice
            End Get
            Set(ByVal Value As String)
                _Codice = Value
            End Set
        End Property

        Private _Descrizione As String
        Public Property Descrizione() As String
            Get
                Return _Descrizione
            End Get
            Set(ByVal Value As String)
                _Descrizione = Value
            End Set
        End Property

        Private _CivicoDa As String
        Public Property CivicoDa() As String
            Get
                Return _CivicoDa
            End Get
            Set(ByVal Value As String)
                _CivicoDa = Value
            End Set
        End Property

        Private _CivicoA As String
        Public Property CivicoA() As String
            Get
                Return _CivicoA
            End Get
            Set(ByVal Value As String)
                _CivicoA = Value
            End Set
        End Property

        Private _TipoNumeroCivico As String
        Public Property TipoNumeroCivico() As String
            Get
                Return _TipoNumeroCivico
            End Get
            Set(ByVal Value As String)
                _TipoNumeroCivico = Value
            End Set
        End Property

        Private _Cap As String
        Public Property Cap() As String
            Get
                Return _Cap
            End Get
            Set(ByVal Value As String)
                _Cap = Value
            End Set
        End Property

        Private _CodiceCircoscrizione As String
        Public Property CodiceCircoscrizione() As String
            Get
                Return _CodiceCircoscrizione
            End Get
            Set(ByVal Value As String)
                _CodiceCircoscrizione = Value
            End Set
        End Property

        Private _CodiceDistetto As String
        Public Property CodiceDistetto() As String
            Get
                Return _CodiceDistetto
            End Get
            Set(ByVal Value As String)
                _CodiceDistetto = Value
            End Set
        End Property

        Private _CodiceComune As String
        Public Property CodiceComune() As String
            Get
                Return _CodiceComune
            End Get
            Set(ByVal Value As String)
                _CodiceComune = Value
            End Set
        End Property

        Private _Default As Boolean
        Public Property [Default]() As Boolean
            Get
                Return _Default
            End Get
            Set(ByVal Value As Boolean)
                _Default = Value
            End Set
        End Property

    End Class

End Namespace

