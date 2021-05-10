Namespace Entities

    ''' <summary>
    ''' Classe con le informazioni sul bilancio
    ''' </summary>
    Public Class BilancioInfo


        Private idValue As Integer
        ''' <summary>
        ''' Id del bilancio
        ''' </summary>
        Public Property Id() As Integer
            Get
                Return idValue
            End Get
            Set(ByVal value As Integer)
                idValue = value
            End Set
        End Property


        Private pazCodiceValue As Integer
        ''' <summary>
        ''' Codice del paziente
        ''' </summary>
        Public Property CodicePaziente() As Integer
            Get
                Return pazCodiceValue
            End Get
            Set(ByVal value As Integer)
                pazCodiceValue = value
            End Set
        End Property


        Private codMalattiaValue As String
        ''' <summary>
        ''' Codice della malattia associata al bilancio.
        ''' </summary>
        Public Property CodiceMalattia() As String
            Get
                Return codMalattiaValue
            End Get
            Set(ByVal value As String)
                codMalattiaValue = value
            End Set
        End Property


        Private descrMalattiaValue As String
        ''' <summary>
        ''' Descrizione della malattia associata al bilancio.
        ''' </summary>
        Public Property DescrizioneMalattia() As String
            Get
                Return descrMalattiaValue
            End Get
            Set(ByVal value As String)
                descrMalattiaValue = value
            End Set
        End Property


        Private flagVisitaValue As Boolean
        ''' <summary>
        ''' Flag che indica se calcolare il bilancio in base alla data di visita.
        ''' Discriminante tra bilanci di salute e bilanci di malattia.
        ''' </summary>
        Public Property FlagVisita() As Boolean
            Get
                Return flagVisitaValue
            End Get
            Set(ByVal value As Boolean)
                flagVisitaValue = value
            End Set
        End Property


        Private numBilancioValue As Integer
        ''' <summary>
        ''' Numero del bilancio
        ''' </summary>
        Public Property NumeroBilancio() As Integer
            Get
                Return numBilancioValue
            End Get
            Set(ByVal value As Integer)
                numBilancioValue = value
            End Set
        End Property


        Private statoValue As String
        ''' <summary>
        ''' Stato del bilancio (UX, US, EX)
        ''' </summary>
        Public Property Stato() As String
            Get
                Return statoValue
            End Get
            Set(ByVal value As String)
                statoValue = value
            End Set
        End Property


        Private cnvDataValue As Date
        ''' <summary>
        ''' Data della convocazione in cui consegnare il bilancio
        ''' </summary>
        Public Property DataConvocazione() As Date
            Get
                Return cnvDataValue
            End Get
            Set(ByVal value As Date)
                cnvDataValue = value
            End Set
        End Property


        Private cnvDataAppValue As Date
        ''' <summary>
        ''' Data dell'appuntamento associato alla convocazione
        ''' </summary>
        Public Property DataAppuntamento() As Date
            Get
                Return cnvDataAppValue
            End Get
            Set(ByVal value As Date)
                cnvDataAppValue = value
            End Set
        End Property


        Private cnvDataInvioValue As Date
        ''' <summary>
        ''' Data di invio dell'avviso relativo alla convocazione
        ''' </summary>
        Public Property DataInvio() As Date
            Get
                Return cnvDataInvioValue
            End Get
            Set(ByVal value As Date)
                cnvDataInvioValue = value
            End Set
        End Property


        ''' <summary>
        ''' Costruttore
        ''' </summary>
        Public Sub New()
        End Sub


    End Class


End Namespace
