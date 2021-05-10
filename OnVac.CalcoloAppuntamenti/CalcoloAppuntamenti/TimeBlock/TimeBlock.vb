Namespace Timing
    <Serializable()> _
    Public Class TimeBlock
        Public Event InizioChanged(ByVal timeBlock As TimeBlock, ByVal oldInizio As Date, ByVal newInizio As Date, ByRef cancel As Boolean)

        Sub New(ByVal inizio As Date, ByVal fine As Date)
            _Inizio = Date.MinValue.Add(inizio.TimeOfDay)
            _Fine = Date.MinValue.Add(fine.TimeOfDay)

            If _Inizio > _Fine Then Throw New Exceptions.TimeBlockInvalidException("La data iniziale di un TimeBlock deve essere minore della data di fine!")
        End Sub

        Sub New(ByVal inizio As Date, ByVal fine As Date, ByVal marcherAmulatorio As Integer, ByVal marcherPaziente As Integer, ByVal marcherVariabili As Integer)
            Me.New(inizio, fine)
            _MarchersAmbulatorio = marcherAmulatorio
            _MarchersPaziente = marcherPaziente
            _MarchersVariabili = marcherVariabili
        End Sub

        Sub New(ByVal inizio As Date, ByVal fine As Date, ByVal marcher As Integer, ByVal tipo As MarcherTipo)
            Me.New(inizio, fine)
            Select Case tipo
                Case MarcherTipo.Ambulatorio
                    _MarchersAmbulatorio = marcher
                Case MarcherTipo.Paziente
                    _MarchersPaziente = marcher
                Case MarcherTipo.Variabile
                    _MarchersVariabili = marcher
            End Select
        End Sub

        Sub New(ByVal inizio As Date, ByVal fine As Date, ByVal marcherAmulatorio As Integer, ByVal marcherPaziente As Integer, ByVal marcherVariabili As Integer, ByVal marcher As Integer, ByVal tipo As MarcherTipo)
            Me.New(inizio, fine, marcherAmulatorio, marcherPaziente, marcherVariabili)
            SetMarcher(marcher, tipo)
        End Sub

        Friend _InOverrideDurata As Boolean
        Public ReadOnly Property InOverrideDurata() As Boolean
            Get
                Return _InOverrideDurata
            End Get
        End Property

        Friend _OverrideDurata As Integer
        Public ReadOnly Property OverrideDurata() As Integer
            Get
                Return _OverrideDurata
            End Get
        End Property

        Friend _ParentDay As Day
        Public ReadOnly Property ParentDay() As Day
            Get
                Return _ParentDay
            End Get
        End Property

        Private _Inizio As Date
        '@ -----------------------------------------------------------------------------
        '@ <summary>
        '@ Indica la data di inizio del marcher. Solo la componente TimeOfDay verrà considerata.
        '@ </summary>
        '@ <value></value>
        '@ <history>
        '@ 	[ssabbattini]	27/02/2006	Created
        '@ </history>
        '@ -----------------------------------------------------------------------------
        Public Property Inizio() As Date
            Get
                Return _Inizio
            End Get
            Set(ByVal value As Date)
                value = Date.MinValue.Add(value.TimeOfDay)

                If value > _Fine Then Throw New Exceptions.TimeBlockInvalidException("La data iniziale di un TimeBlock deve essere minore della data di fine!")

                Dim cancel As Boolean
                RaiseEvent InizioChanged(Me, _Inizio, value, cancel)

                If Not cancel Then _Inizio = value
            End Set
        End Property

        Private _Fine As Date
        '@ -----------------------------------------------------------------------------
        '@ <summary>
        '@ Indica la fine del periodo. Solo la componente TimeOfDay verrà considerata.
        '@ </summary>
        '@ <value></value>
        '@ <history>
        '@ 	[ssabbattini]	27/02/2006	Created
        '@ </history>
        '@ -----------------------------------------------------------------------------
        Public Property Fine() As Date
            Get
                Return _Fine
            End Get
            Set(ByVal value As Date)
                value = Date.MinValue.Add(value.TimeOfDay)

                If value < _Inizio Then Throw New Exceptions.TimeBlockInvalidException("La data finale di un TimeBlock deve essere maggiore della data di inizio!")
                _Fine = value
            End Set
        End Property

        Private _MarchersAmbulatorio As Integer
        '@ -----------------------------------------------------------------------------
        '@ <summary>
        '@ Il valore numerico dei marcher di ambulatorio applicati al TimeBlock. Utilizzare le funzione SetMarcher, UnsetMarcher e HaveMarcher per elaborarli.
        '@ </summary>
        '@ <value></value>
        '@ <history>
        '@ 	[ssabbattini]	27/02/2006	Created
        '@ </history>
        '@ -----------------------------------------------------------------------------
        Public ReadOnly Property MarchersAmbulatorio() As Integer
            Get
                Return _MarchersAmbulatorio
            End Get
        End Property

        Private _MarchersPaziente As Integer
        '@ -----------------------------------------------------------------------------
        '@ <summary>
        '@ Il valore numerico dei marcher di paziente applicati al TimeBlock. Utilizzare le funzione SetMarcher, UnsetMarcher e HaveMarcher per elaborarli.
        '@ </summary>
        '@ <value></value>
        '@ <history>
        '@ 	[ssabbattini]	27/02/2006	Created
        '@ </history>
        '@ -----------------------------------------------------------------------------
        Public ReadOnly Property MarchersPaziente() As Integer
            Get
                Return _MarchersPaziente
            End Get
        End Property

        Private _MarchersVariabili As Integer
        '@ -----------------------------------------------------------------------------
        '@ <summary>
        '@ Il valore numerico dei marcher variabili applicati al TimeBlock. Utilizzare le funzione SetMarcher, UnsetMarcher e HaveMarcher per elaborarli.
        '@ </summary>
        '@ <value></value>
        '@ <history>
        '@ 	[ssabbattini]	27/02/2006	Created
        '@ </history>
        '@ -----------------------------------------------------------------------------
        Public ReadOnly Property MarchersVariabili() As Integer
            Get
                Return _MarchersVariabili
            End Get
        End Property

        '@ -----------------------------------------------------------------------------
        '@ <summary>
        '@ Imposta un marcher ad una variabile. Il valore marcher deve essere una esponente di due per impostare correttamente il biut n-esimo.
        '@ </summary>
        '@ <param name="marcher"></param>
        '@ <param name="tipo"></param>
        '@ <returns></returns>
        '@ <remarks>
        '@ </remarks>
        '@ <history>
        '@ 	[ssabbattini]	27/02/2006	Created
        '@ </history>
        '@ -----------------------------------------------------------------------------
        Public Function HaveMarcher(ByVal marcher As Integer, ByVal tipo As MarcherTipo) As Boolean
            If Not IsLogaritm2(marcher) Then
                Throw New Exceptions.NotValidMarcherException
            End If

            Select Case tipo
                Case MarcherTipo.Ambulatorio
                    Return ((_MarchersAmbulatorio And marcher) = marcher)
                Case MarcherTipo.Paziente
                    Return ((_MarchersPaziente And marcher) = marcher)
                Case MarcherTipo.Variabile
                    Return ((_MarchersVariabili And marcher) = marcher)
            End Select
        End Function

        Public Function HaveMarcher() As Boolean
            Return _MarchersAmbulatorio > 0 OrElse _MarchersPaziente > 0 OrElse _MarchersVariabili > 0
        End Function

        Public Sub SetMarcher(ByVal marcher As Integer, ByVal tipo As MarcherTipo)
            If Not IsLogaritm2(marcher) Then
                Throw New Exceptions.NotValidMarcherException(Math.Log(marcher, 2))
            End If

            Select Case tipo
                Case MarcherTipo.Ambulatorio
                    _MarchersAmbulatorio = _MarchersAmbulatorio Or marcher
                Case MarcherTipo.Paziente
                    _MarchersPaziente = _MarchersPaziente Or marcher
                Case MarcherTipo.Variabile
                    _MarchersVariabili = _MarchersVariabili Or marcher
            End Select
        End Sub

        Public Sub UnsetMarcher(ByVal marcher As Integer, ByVal tipo As MarcherTipo)
            If Not IsLogaritm2(marcher) Then
                Throw New Exceptions.NotValidMarcherException
            End If

            Select Case tipo
                Case MarcherTipo.Ambulatorio
                    _MarchersAmbulatorio -= _MarchersAmbulatorio And marcher
                Case MarcherTipo.Paziente
                    _MarchersPaziente -= _MarchersPaziente And marcher
                Case MarcherTipo.Variabile
                    _MarchersVariabili -= _MarchersVariabili And marcher
            End Select
        End Sub

        '@ -----------------------------------------------------------------------------
        '@ <summary>
        '@ Controlla se il marcher è lungo un determinato numero di minuti e che non sia occupato da marcher.
        '@ </summary>
        '@ <param name="minuti">Numero di minuti con cui si vuole testare la lunghezza del marcher.</param>
        '@ <returns>True se il marcher è libero e lungo abbastanza, False altrimenti.</returns>
        '@ <history>
        '@ 	[ssabbattini]	27/02/2006	Created
        '@ </history>
        '@ -----------------------------------------------------------------------------
        Public Function IsFree(ByVal minuti As Integer) As Boolean
            If _MarchersAmbulatorio = 0 AndAlso _MarchersPaziente = 0 AndAlso _MarchersVariabili = 0 Then
                Return _Inizio.AddMinutes(minuti) <= _Fine
            Else
                Return False
            End If
        End Function

        '@ -----------------------------------------------------------------------------
        '@ <summary>
        '@ Valuta se il numero passato è un esposnete di due.
        '@ </summary>
        '@ <param name="val">Valore intero da controllare.</param>
        '@ <returns>True se val è uin esponente di due, False altrimenti.</returns>
        '@ <history>
        '@ 	[ssabbattini]	27/02/2006	Created
        '@ </history>
        '@ -----------------------------------------------------------------------------
        Private Function IsLogaritm2(ByVal val As Integer) As Boolean
            If val > 0 Then
                Dim exp As Double
                exp = Math.Log(val, 2)

                Dim ret As Boolean
                ret = (Math.Round(exp, 3) = Math.Round(exp, 0))

                Return ret
            Else
                Return True
            End If
        End Function
    End Class
End Namespace