Namespace ExecLog.Entities

    ' oggetto per il logging dell'esecuzione della funzione BilanciDaProgrammare
    ' wwwroot/BizLogic/BizBilancioProgrammato.vb
    Public Class XLBilanciDaProgrammare

#Region "Variabili Membro"
        Private _BilNumero As Int16
        Private _MalCodice As String
        Private _CnvData As Date
        Private _Operatore As String
        Private _PazCodice As Integer
        Private _DataOperazione As Date
        Private _XL As Int64
#End Region

#Region "Costruttore"
        Sub New(ByVal BilNumero As Int16, ByVal MalCodice As String, ByVal CnvData As Date, ByVal Operatore As String, ByVal PazCodice As Integer, ByVal DataOperazione As Date, ByVal XL As Int64)

            _BilNumero = BilNumero
            _MalCodice = MalCodice
            _CnvData = CnvData
            _Operatore = Operatore
            _PazCodice = PazCodice
            _DataOperazione = DataOperazione
            _XL = XL

        End Sub
#End Region

#Region "Metodi Pubblici"

        Public Property BilNumero() As Int16
            Get
                Return _BilNumero
            End Get
            Set(ByVal Value As Int16)

                _BilNumero = Value

            End Set
        End Property

        Public Property MalCodice() As String
            Get
                Return _MalCodice
            End Get
            Set(ByVal Value As String)

                _MalCodice = Value

            End Set
        End Property

        Public Property CnvData() As Date
            Get
                Return _CnvData
            End Get
            Set(ByVal Value As Date)

                _CnvData = Value

            End Set
        End Property

        Public Property Operatore() As String
            Get
                Return _Operatore
            End Get
            Set(ByVal Value As String)

                _Operatore = Value

            End Set
        End Property

        Public Property PazCodice() As Integer
            Get
                Return _PazCodice
            End Get
            Set(ByVal Value As Integer)

                _PazCodice = Value

            End Set
        End Property

        Public Property DataOperazione() As Date
            Get
                Return _DataOperazione
            End Get
            Set(ByVal Value As Date)

                _DataOperazione = Value

            End Set
        End Property

        Public Property XL() As Int64
            Get
                Return _XL
            End Get
            Set(ByVal Value As Int64)

                _XL = Value

            End Set
        End Property

#End Region

    End Class
End Namespace
