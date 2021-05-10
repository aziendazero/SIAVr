Namespace Entities

    Public Class Report
        Implements IDisposable

        ' nome del file .rpt
        Private _nome As String
        Public Property Nome() As String
            Get
                Return _nome
            End Get
            Set(ByVal Value As String)
                _nome = Value
            End Set
        End Property

        ' installazione in cui il file è usato (es. ReportRavenna)
        Private _installazione As String
        Public Property Installazione() As String
            Get
                Return _installazione
            End Get
            Set(ByVal Value As String)
                _installazione = Value
            End Set
        End Property

        ' cartella in cui è contenuto il report
        Private _cartella As String
        Public Property Cartella() As String
            Get
                Return _cartella
            End Get
            Set(ByVal Value As String)
                _cartella = Value
            End Set
        End Property

        ' nome del file .xsd utilizzato (null se non usato)
        Private _dataset As String
        Public Property DataSet() As String
            Get
                Return _dataset
            End Get
            Set(ByVal Value As String)
                _dataset = Value
            End Set
        End Property


        Sub New()
            _nome = String.Empty
            _installazione = String.Empty
            _cartella = String.Empty
            _dataset = String.Empty
        End Sub


        Sub New(ByVal reportName As String)
            _nome = reportName
            _installazione = String.Empty
            _cartella = String.Empty
            _dataset = String.Empty
        End Sub


#Region "IDisposable"

        ' Questo codice è aggiunto da Visual Basic per implementare in modo corretto il modello Disposable.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Non modificare questo codice. Inserire il codice di pulitura in Dispose(ByVal disposing As Boolean).
            GC.SuppressFinalize(Me)
        End Sub

#End Region


    End Class


End Namespace
