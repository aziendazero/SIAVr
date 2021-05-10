'@ -----------------------------------------------------------------------------
'@ Project	 : CalcoloAppuntamenti
'@ Class	 : CalcoloAppuntamenti.CnvPaziente
'@ 
'@ -----------------------------------------------------------------------------
'@ <summary>
'@ Rappresenta un'entità di convocazione. Un pazientwe a cui è associata una convocazion e e a cui si deve provvedere a trovare un appuntamento. La classe supporta filtri di preferenza, priorità, gestione dei ritardatari,
'@ pazienti con solo bilancio e associazione ad ambulatorio.
'@ </summary>
'@ <remarks>
'@ Nella classe che funge da collezione per le convocazioni non possono essere presenti due pazienti uguali con la stessa data di convocazione.
'@ </remarks>
'@ <history>
'@ 	[ssabbattini]	23/02/2006	Created
'@ </history>
'@ -----------------------------------------------------------------------------
Public Class CnvPaziente

    Public Event PrioritaChanged(ByVal paz As CnvPaziente, ByVal oldPri As Date, ByVal newPri As Date)

    '@ -----------------------------------------------------------------------------
    '@ <summary>
    '@ Crea un nuovo paziente con codice e data di convocazione. Le due proprietà sono ReadOnly.
    '@ </summary>
    '@ <param name="codice">Codice numerico del paziente.</param>
    '@ <param name="dataCnv">Data di convocazione.</param>
    '@ <history>
    '@ 	[ssabbattini]	23/02/2006	Created
    '@ </history>
    '@ -----------------------------------------------------------------------------
    Public Sub New(ByVal codice As String, ByVal dataCnv As Date, ByVal soloBilancio As Boolean, ByVal ritardatario As Boolean)
        _Codice = codice
        _DataConvocazione = dataCnv
        _SoloBilancio = soloBilancio
        _Ritardatario = ritardatario
        _IsAsssociated = False
        _EsitoProcedura = New EsitoProcedura
    End Sub

    Private _Codice As Integer
    Public ReadOnly Property Codice() As Integer
        Get
            Return _Codice
        End Get
    End Property

    Private _DataNascita As Date
    Public Property DataNascita() As Date
        Get
            Return _DataNascita
        End Get
        Set(ByVal value As Date)
            _DataNascita = value
        End Set
    End Property

    Private _Nome As String
    Public Property Nome() As String
        Get
            Return _Nome
        End Get
        Set(ByVal value As String)
            _Nome = value
        End Set
    End Property

    Private _Cognome As String
    Public Property Cognome() As String
        Get
            Return _Cognome
        End Get
        Set(ByVal value As String)
            _Cognome = value
        End Set
    End Property

    Private _DataConvocazione As Date
    Public Property DataConvocazione() As Date
        Get
            Return _DataConvocazione
        End Get
        Set(ByVal value As Date)
            _DataConvocazione = value
        End Set
    End Property

    Private _DurataAppuntamento As Integer
    Public Property DurataAppuntamento() As Integer
        Get
            Return _DurataAppuntamento
        End Get
        Set(ByVal value As Integer)
            If value < 0 Then value = 0
            _DurataAppuntamento = value
        End Set
    End Property

    Private _Priorita As Date
    '@ -----------------------------------------------------------------------------
    '@ <summary>
    '@ La priorità è intesa come data di convocazione + ___, è la chiave su cui verrà oiirdinata la collezione richiamando il metodo GetPazienti.
    '@ </summary>
    '@ <value></value>
    '@ <history>
    '@ 	[ssabbattini]	23/02/2006	Created
    '@ </history>
    '@ -----------------------------------------------------------------------------
    Public Property Priorita() As Date
        Get
            Return _Priorita
        End Get
        Set(ByVal value As Date)
            RaiseEvent PrioritaChanged(Me, _Priorita, value)
            _Priorita = value
        End Set
    End Property

    Private _Ritardatario As Boolean
    '@ -----------------------------------------------------------------------------
    '@ <summary>
    '@ Indica se il paziente è ritardatario oppure no. Nel caso sia ritardatario, verrà trattato in maniuera differente dall calcolo degli appuntamenti.
    '@ </summary>
    '@ <value></value>
    '@ <history>
    '@ 	[ssabbattini]	23/02/2006	Created
    '@ </history>
    '@ -----------------------------------------------------------------------------
    Public ReadOnly Property Ritardatario() As Boolean
        Get
            Return _Ritardatario
        End Get
    End Property

    Private _Info As New Hashtable
    Public ReadOnly Property Info() As Hashtable
        Get
            Return _Info
        End Get
    End Property

    Private _EsitoProcedura As New EsitoProcedura
    Public Property EsitoProcedura() As EsitoProcedura
        Get
            Return _EsitoProcedura
        End Get
        Set(ByVal value As EsitoProcedura)
            _EsitoProcedura = value
        End Set
    End Property

    Private _Preferenze As New Filtri.FiltroCollection
    Public Property Preferenze() As Filtri.FiltroCollection
        Get
            Return _Preferenze
        End Get
        Set(ByVal value As Filtri.FiltroCollection)
            _Preferenze = value
        End Set
    End Property

    Private _SoloBilancio As Boolean
    Public ReadOnly Property SoloBilancio() As Boolean
        Get
            Return _SoloBilancio
        End Get
    End Property

    Private _Ambulatorio As String
    '@ -----------------------------------------------------------------------------
    '@ <summary>
    '@ Indica l'ambulatorio di appartenenza del paziente. La proprietà è writable finchè il paziente non viene aggiunto ad una proprietà CnvPazienteCollection di un ambulatorio tramite la classe
    '@ Convocazioni.
    '@ </summary>
    '@ <value></value>
    '@ <history>
    '@ 	[ssabbattini]	23/02/2006	Created
    '@ </history>
    '@ -----------------------------------------------------------------------------
    Public Property Ambulatorio() As String
        Get
            Return _Ambulatorio
        End Get
        Set(ByVal value As String)
            If _IsAsssociated Then
                Throw New Exceptions.PazienteAssociatoException(_Ambulatorio)
            End If
            _Ambulatorio = value
        End Set
    End Property

    Private _IsAsssociated As Boolean
    '@ -----------------------------------------------------------------------------
    '@ <summary>
    '@ Indica se il paziente è stato associato a un ambulatorio. Questo non vuol dire che la sua proprietà Ambulatorio è valorizzata, ma che quando è stato aggiunto
    '@ alla collezione Pazienti della classe Consultorio, è stato smistato nella classe consultorio e inserito in una delle sue prioprietà CnvPazientiCollection. Quando la proprietà
    '@ è impostata a True (cioè finchè non viene rimosso dalla collezione Pazienti della classe Consultorio) non è possibile cambiare la sua proprietà Ambulatorio (causa eccezzione).
    '@ </summary>
    '@ <value></value>
    '@ <history>
    '@ 	[ssabbattini]	23/02/2006	Created
    '@ </history>
    '@ -----------------------------------------------------------------------------
    Friend Property IsAssociated() As Boolean
        Get
            Return _IsAsssociated
        End Get
        Set(ByVal value As Boolean)
            _IsAsssociated = value
        End Set
    End Property

    Public Overrides Function ToString() As String
        Return String.Format("{0} {1} {2} ({3}) {4}", _Codice, _Nome, _Cognome, _DataConvocazione.ToShortDateString, _Priorita.ToShortDateString)
    End Function
End Class
