Namespace Entities

    Public Class BilancioProgrammato

#Region " Properties "

        Private _Bil_id As Integer = 0
        Public Property Bil_id() As Integer
            Get
                Return _Bil_id
            End Get
            Set(Value As Integer)
                _Bil_id = Value
            End Set
        End Property

        Private _Paz_Codice As Integer
        Public Property Paz_Codice() As Integer
            Get
                Return _Paz_Codice
            End Get
            Set(Value As Integer)
                _Paz_Codice = Value
            End Set
        End Property

        Private _Descrizione_Malattia As String
        Public Property Descrizione_Malattia() As String
            Get
                Return _Descrizione_Malattia
            End Get
            Set(Value As String)
                _Descrizione_Malattia = Value
            End Set
        End Property

        Public ReadOnly Property NBilNMal() As String
            Get
                Return _N_bilancio & "|" & _Mal_codice
            End Get
        End Property

        Public ReadOnly Property MalDescNBil() As String
            Get
                Return _Descrizione_Malattia & " | " & _N_bilancio
            End Get
        End Property

        Private _Mal_codice As String
        Public Property Mal_codice() As String
            Get
                Return _Mal_codice
            End Get
            Set(Value As String)
                _Mal_codice = Value
            End Set
        End Property

        Private _N_bilancio As Int16
        Public Property N_bilancio() As Int16
            Get
                Return _N_bilancio
            End Get
            Set(Value As Int16)
                _N_bilancio = Value
            End Set
        End Property

        Private _BIL_DESCRIZIONE As String
        Public Property BIL_DESCRIZIONE() As String
            Get
                Return _BIL_DESCRIZIONE
            End Get
            Set(value As String)
                _BIL_DESCRIZIONE = value
            End Set
        End Property

        Private _Data_CNV As Date
        Public Property Data_CNV() As Date
            Get
                Return _Data_CNV
            End Get
            Set(Value As Date)
                _Data_CNV = Value
            End Set
        End Property

        Private _Bil_stato As String
        Public Property Bil_stato() As String
            Get
                Return _Bil_stato
            End Get
            Set(Value As String)
                _Bil_stato = Value
            End Set
        End Property

        Private _New_Cnv As Boolean
        Public Property New_Cnv() As Boolean
            Get
                Return _New_Cnv
            End Get
            Set(Value As Boolean)
                _New_Cnv = Value
            End Set
        End Property

        Private _Eta_Minima As Integer
        Public Property Eta_Minima() As Integer
            Get
                Return _Eta_Minima
            End Get
            Set(Value As Integer)
                _Eta_Minima = Value
            End Set
        End Property

        Private _Eta_Massima As Integer
        Public Property Eta_Massima() As Integer
            Get
                Return _Eta_Massima
            End Get
            Set(Value As Integer)
                _Eta_Massima = Value
            End Set
        End Property

        Private _Intervallo As Integer
        Public Property Intervallo() As Integer
            Get
                Return _Intervallo
            End Get
            Set(Value As Integer)
                _Intervallo = Value
            End Set
        End Property

        Private _BIL_CRANIO As Boolean
        Public Property BIL_CRANIO() As Boolean
            Get
                Return _BIL_CRANIO
            End Get
            Set(value As Boolean)
                _BIL_CRANIO = value
            End Set
        End Property

        Private _BIL_PESO As Boolean
        Public Property BIL_PESO() As Boolean
            Get
                Return _BIL_PESO
            End Get
            Set(value As Boolean)
                _BIL_PESO = value
            End Set
        End Property

        Private _BIL_ALTEZZA As Boolean
        Public Property BIL_ALTEZZA() As Boolean
            Get
                Return _BIL_ALTEZZA
            End Get
            Set(value As Boolean)
                _BIL_ALTEZZA = value
            End Set
        End Property

        Private _BIL_OBBLIGATORIO As Boolean
        Public Property BIL_OBBLIGATORIO() As Boolean
            Get
                Return _BIL_OBBLIGATORIO
            End Get
            Set(value As Boolean)
                _BIL_OBBLIGATORIO = value
            End Set
        End Property

#End Region

#Region " Constructors "

        Sub New()
        End Sub

        Sub New(codiceMalattia As String, numeroBilancio As Int16, dataConvocazione As Date, statoBilancio As String, isNewConvocazione As Boolean, codicePaziente As Integer)
            Me._Mal_codice = codiceMalattia
            Me._N_bilancio = numeroBilancio
            Me._Data_CNV = dataConvocazione
            Me._Bil_stato = statoBilancio
            Me._New_Cnv = isNewConvocazione
            Me._Paz_Codice = codicePaziente
        End Sub

        Sub New(codiceMalattia As String, numeroBilancio As Int16)
            Me._Mal_codice = codiceMalattia
            Me._N_bilancio = numeroBilancio
        End Sub

#End Region

#Region " Overrides "

        Public Overrides Function ToString() As String

            Dim str As String =
                   "MAL CODICE: " & Me.Mal_codice & ", "
            str &= "N BILANCIO: " & Me.N_bilancio & ", "
            str &= "DATA CNV: " & Me.Data_CNV & ", "
            str &= "BIL STATO: " & Me.Bil_stato & ", "
            str &= "NEW CNV: " & Me.New_Cnv & ", "
            str &= "PAZ CODICE: " & Me.Paz_Codice & ", "
            str &= "ETA' MINIMA: " & Me.Eta_Minima & ", "
            str &= "ETA' MASSIMA: " & Me.Eta_Massima & ""

            Return str

        End Function

#End Region

    End Class

End Namespace
