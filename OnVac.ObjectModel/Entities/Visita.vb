Namespace Entities

    <Serializable()>
    Public Class Visita

        Private _IdVisita As Integer
        Public Property IdVisita() As Integer
            Get
                Return _IdVisita
            End Get
            Set(value As Integer)
                _IdVisita = value
            End Set
        End Property

        Private _CodicePaziente As Integer
        Public Property CodicePaziente() As Integer
            Get
                Return _CodicePaziente
            End Get
            Set(value As Integer)
                _CodicePaziente = value
            End Set
        End Property

        Private _BilancioNumero As Long?
        Property BilancioNumero() As Long?
            Get
                Return _BilancioNumero
            End Get
            Set(Value As Long?)
                _BilancioNumero = Value
            End Set
        End Property

        Private _BilancioDescrizione As String
        Property BilancioDescrizione() As String
            Get
                Return _BilancioDescrizione
            End Get
            Set(Value As String)
                _BilancioDescrizione = Value
            End Set
        End Property

        Private _MalattiaCodice As String
        Property MalattiaCodice() As String
            Get
                Return _MalattiaCodice
            End Get
            Set(Value As String)
                _MalattiaCodice = Value
            End Set
        End Property

        Private _MalattiaDescrizione As String
        Property MalattiaDescrizione() As String
            Get
                Return _MalattiaDescrizione
            End Get
            Set(Value As String)
                _MalattiaDescrizione = Value
            End Set
        End Property

        Private _DataVisita As Date
        Property DataVisita() As Date
            Get
                Return _DataVisita
            End Get
            Set(Value As Date)
                _DataVisita = Value
            End Set
        End Property

        Private _DataRegistrazione As Date
        Property DataRegistrazione() As Date
            Get
                Return _DataRegistrazione
            End Get
            Set(Value As Date)
                _DataRegistrazione = Value
            End Set
        End Property

        Private _Peso As Double?
        Property Peso() As Double?
            Get
                Return _Peso
            End Get
            Set(Value As Double?)
                _Peso = Value
            End Set
        End Property

        Private _Altezza As Double?
        Property Altezza() As Double?
            Get
                Return _Altezza
            End Get
            Set(Value As Double?)
                _Altezza = Value
            End Set
        End Property

        Private _Cranio As Double?
        Property Cranio() As Double?
            Get
                Return _Cranio
            End Get
            Set(Value As Double?)
                _Cranio = Value
            End Set
        End Property

        Private _PercentilePeso As String
        Property PercentilePeso() As String
            Get
                Return _PercentilePeso
            End Get
            Set(Value As String)
                _PercentilePeso = Value
            End Set
        End Property

        Private _PercentileAltezza As String
        Property PercentileAltezza() As String
            Get
                Return _PercentileAltezza
            End Get
            Set(Value As String)
                _PercentileAltezza = Value
            End Set
        End Property

        Private _PercentileCranio As String
        Property PercentileCranio() As String
            Get
                Return _PercentileCranio
            End Get
            Set(Value As String)
                _PercentileCranio = Value
            End Set
        End Property

        Private _RegistraPeso As Boolean
        Property RegistraPeso() As Boolean
            Get
                Return _RegistraPeso
            End Get
            Set(Value As Boolean)
                _RegistraPeso = Value
            End Set
        End Property

        Private _RegistraAltezza As Boolean
        Property RegistraAltezza() As Boolean
            Get
                Return _RegistraAltezza
            End Get
            Set(Value As Boolean)
                _RegistraAltezza = Value
            End Set
        End Property

        Private _RegistraCranio As Boolean
        Property RegistraCranio() As Boolean
            Get
                Return _RegistraCranio
            End Get
            Set(Value As Boolean)
                _RegistraCranio = Value
            End Set
        End Property

        Private _MedicoCodice As String
        Property MedicoCodice() As String
            Get
                Return _MedicoCodice
            End Get
            Set(Value As String)
                _MedicoCodice = Value
            End Set
        End Property

        Private _MedicoDescrizione As String
        Property MedicoDescrizione() As String
            Get
                Return _MedicoDescrizione
            End Get
            Set(Value As String)
                _MedicoDescrizione = Value
            End Set
        End Property

        Private _EtaGiorniEsecuzione As Integer
        Property EtaGiorniEsecuzione() As Integer
            Get
                Return _EtaGiorniEsecuzione
            End Get
            Set(Value As Integer)
                _EtaGiorniEsecuzione = Value
            End Set
        End Property

        Private _Firma As String
        Property Firma() As String
            Get
                Return _Firma
            End Get
            Set(Value As String)
                _Firma = Value
            End Set
        End Property

        Private _MotivoSospensioneCodice As String
        Public Property MotivoSospensioneCodice() As String
            Get
                Return _MotivoSospensioneCodice
            End Get
            Set(value As String)
                _MotivoSospensioneCodice = value
            End Set
        End Property

        Private _MotivoSospensioneDescrizione As String
        Public Property MotivoSospensioneDescrizione() As String
            Get
                Return _MotivoSospensioneDescrizione
            End Get
            Set(value As String)
                _MotivoSospensioneDescrizione = value
            End Set
        End Property

        Private _DataFineSospensione As Date
        Property DataFineSospensione() As Date
            Get
                Return _DataFineSospensione
            End Get
            Set(Value As Date)
                _DataFineSospensione = Value
            End Set
        End Property

        Private _CodicePazienteAlias As Int32?
        Public Property CodicePazienteAlias() As Int32?
            Get
                Return _CodicePazienteAlias
            End Get
            Set(value As Int32?)
                _CodicePazienteAlias = value
            End Set
        End Property

        Private _CodiceConsultorio As String
        Public Property CodiceConsultorio() As String
            Get
                Return _CodiceConsultorio
            End Get
            Set(value As String)
                _CodiceConsultorio = value
            End Set
        End Property

        Private _FlagPatologia As String
        Public Property FlagPatologia() As String
            Get
                Return _FlagPatologia
            End Get
            Set(value As String)
                _FlagPatologia = value
            End Set
        End Property

        Private _Note As String
        Public Property Note() As String
            Get
                Return _Note
            End Get
            Set(value As String)
                _Note = value
            End Set
        End Property

        Private _IdUtente As Integer
        Public Property IdUtente() As Integer
            Get
                Return _IdUtente
            End Get
            Set(value As Integer)
                _IdUtente = value
            End Set
        End Property

        Private _CodiceUslInserimento As String
        Public Property CodiceUslInserimento() As String
            Get
                Return _CodiceUslInserimento
            End Get
            Set(value As String)
                _CodiceUslInserimento = value
            End Set
        End Property

        Private _Vaccinabile As String

        ''' <summary>
        ''' Può assumere uno dei valori presenti nella T_ANA_CODIFICHE.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Vaccinabile() As String
            Get
                Return _Vaccinabile
            End Get
            Set(value As String)
                _Vaccinabile = value
            End Set
        End Property

        Public Property FlagVisibilitaDatiVaccinaliCentrale() As String

        Public Property DataModifica() As DateTime?
        Public Property IdUtenteModifica() As Int64?

        Public Property NoteAcquisizioneDatiVaccinaliCentrale() As String

        Public Property DataEliminazione() As DateTime?
        Public Property IdUtenteEliminazione() As Int64?

        Public Property IdDocumento As Long?
        Public Property DataFirmaDigitale As DateTime?
        Public Property IdUtenteFirmaDigitale As Long?
        Public Property DataArchiviazione As DateTime?
        Public Property IdUtenteArchiviazione As Long?

        Public Property RilevatoreCodice As String
        Public Property RilevatoreDescrizione As String

        Public Property DataInizioViaggio As DateTime?
        Public Property DataFineViaggio As DateTime?
        Public Property PaeseViaggioCodice As String
        Public Property PaeseViaggioDescrizione As String

        Public Property VaccinazioniBilancio As String

        Public Property DescrizioneConsultorio As String
        Public Property FollowUpId As Integer?
        Public Property DataFollowUpPrevisto As Date?
        Public Property DataFollowUpEffettivo As Date?
        Public Property MalattiaFollowUp As String
        Public Property MalattiaCodFollowUp As String

    End Class

End Namespace
