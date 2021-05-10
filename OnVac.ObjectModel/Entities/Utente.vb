Namespace Entities

''' <summary>
''' Classe contenente tutti i dati previsti per l'utente, sia quelli letti da db che quelli presenti nella InfoUtente.
''' </summary>
Public Class Utente

    ' Campi della struttura portale.shared.InfoUtente
    Private _amministratore As Boolean
    Private _computerName As String
    Private _fax As String
    Private _isValidMachine As Boolean
    Private _machineGroupID As String
    Private _machineGroupName As String
    Private _machineID As String
    Private _remoteIP As String
    Private _telefono As String

    ' Campi della tabella t_ana_utenti
    Private _obsoleto As String
    Private _firma As String

    ' Campi comuni a entrambe
    Private _id As Integer
    Private _codice As String
    Private _descrizione As String
    Private _email As String
    Private _cognome As String
    Private _nome As String


    Public Property Id() As Integer
        Get
            Return _id
        End Get
        Set(ByVal Value As Integer)
            _id = Value
        End Set
    End Property

    Public Property Codice() As String
        Get
            Return _codice
        End Get
        Set(ByVal Value As String)
            _codice = Value
        End Set
    End Property

    Public Property Descrizione() As String
        Get
            Return _descrizione
        End Get
        Set(ByVal Value As String)
            _descrizione = Value
        End Set
    End Property

    Public Property Obsoleto() As String
        Get
            Return _obsoleto
        End Get
        Set(ByVal Value As String)
            _obsoleto = Value
        End Set
    End Property

    Public Property Email() As String
        Get
            Return _email
        End Get
        Set(ByVal Value As String)
            _email = Value
        End Set
    End Property

    Public Property Cognome() As String
        Get
            Return _cognome
        End Get
        Set(ByVal Value As String)
            _cognome = Value
        End Set
    End Property

    Public Property Nome() As String
        Get
            Return _nome
        End Get
        Set(ByVal Value As String)
            _nome = Value
        End Set
    End Property

    Public Property Firma() As String
        Get
            Return _firma
        End Get
        Set(ByVal Value As String)
            _firma = Value
        End Set
    End Property

    Public Property Amministratore() As Boolean
        Get
            Return _amministratore
        End Get
        Set(ByVal Value As Boolean)
            _amministratore = Value
        End Set
    End Property

    Public Property ComputerName() As String
        Get
            Return _computerName
        End Get
        Set(ByVal Value As String)
            _computerName = Value
        End Set
    End Property

    Public Property Fax() As String
        Get
            Return _fax
        End Get
        Set(ByVal Value As String)
            _fax = Value
        End Set
    End Property

    Public Property IsValidMachine() As Boolean
        Get
            Return _isValidMachine
        End Get
        Set(ByVal Value As Boolean)
            _isValidMachine = Value
        End Set
    End Property

    Public Property MachineGroupID() As String
        Get
            Return _machineGroupID
        End Get
        Set(ByVal Value As String)
            _machineGroupID = Value
        End Set
    End Property

    Public Property MachineGroupName() As String
        Get
            Return _machineGroupName
        End Get
        Set(ByVal Value As String)
            _machineGroupName = Value
        End Set
    End Property

    Public Property MachineID() As String
        Get
            Return _machineID
        End Get
        Set(ByVal Value As String)
            _machineID = Value
        End Set
    End Property

    Public Property RemoteIP() As String
        Get
            Return _remoteIP
        End Get
        Set(ByVal Value As String)
            _remoteIP = Value
        End Set
    End Property

    Public Property Telefono() As String
        Get
            Return _telefono
        End Get
        Set(ByVal Value As String)
            _telefono = Value
        End Set
    End Property


End Class


End Namespace