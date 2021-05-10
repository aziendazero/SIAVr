Namespace Entities


    Public Class DatiIntestazioneReport


        Private ins_reg_codice As String
        Public Property CodiceRegione() As String
            Get
                Return ins_reg_codice
            End Get
            Set(ByVal Value As String)
                ins_reg_codice = Value
            End Set
        End Property

        Private ins_usl_codice As String
        Public Property CodiceUsl() As String
            Get
                Return ins_usl_codice
            End Get
            Set(ByVal Value As String)
                ins_usl_codice = Value
            End Set
        End Property

        Private ins_usl_descrizione As String
        Public Property DescrizioneUsl() As String
            Get
                Return ins_usl_descrizione
            End Get
            Set(ByVal Value As String)
                ins_usl_descrizione = Value
            End Set
        End Property

        Private ins_usl_descrizione_report As String
        Public Property DescrizioneUslPerReport() As String
            Get
                Return ins_usl_descrizione_report
            End Get
            Set(ByVal Value As String)
                ins_usl_descrizione_report = Value
            End Set
        End Property

        Private ins_usl_indirizzo As String
        Public Property IndirizzoUsl() As String
            Get
                Return ins_usl_indirizzo
            End Get
            Set(ByVal Value As String)
                ins_usl_indirizzo = Value
            End Set
        End Property

        Private ins_usl_cap As String
        Public Property CapUsl() As String
            Get
                Return ins_usl_cap
            End Get
            Set(ByVal Value As String)
                ins_usl_cap = Value
            End Set
        End Property

        Private ins_usl_citta As String
        Public Property ComuneUsl() As String
            Get
                Return ins_usl_citta
            End Get
            Set(ByVal Value As String)
                ins_usl_citta = Value
            End Set
        End Property

        Private ins_usl_provincia As String
        Public Property ProvinciaUsl() As String
            Get
                Return ins_usl_provincia
            End Get
            Set(ByVal Value As String)
                ins_usl_provincia = Value
            End Set
        End Property

        Private ins_usl_regione As String
        Public Property RegioneUsl() As String
            Get
                Return ins_usl_regione
            End Get
            Set(ByVal Value As String)
                ins_usl_regione = Value
            End Set
        End Property

        Private ins_usl_telefono As String
        Public Property TelefonoUsl() As String
            Get
                Return ins_usl_telefono
            End Get
            Set(ByVal Value As String)
                ins_usl_telefono = Value
            End Set
        End Property

        Private ins_usl_scadenza As String
        Public Property ScadenzaUsl() As String
            Get
                Return ins_usl_scadenza
            End Get
            Set(ByVal Value As String)
                ins_usl_scadenza = Value
            End Set
        End Property

        Private ins_installazione As String
        Public Property Installazione() As String
            Get
                Return ins_installazione
            End Get
            Set(ByVal Value As String)
                ins_installazione = Value
            End Set
        End Property

        Private ins_usl_partita_iva As String
        Public Property PartitaIvaUsl() As String
            Get
                Return ins_usl_partita_iva
            End Get
            Set(ByVal Value As String)
                ins_usl_partita_iva = Value
            End Set
        End Property

        Private ins_usl_fax As String
        Public Property FaxUsl() As String
            Get
                Return ins_usl_fax
            End Get
            Set(ByVal Value As String)
                ins_usl_fax = Value
            End Set
        End Property

        Private ins_intestazione_report As String
        Public Property ReportHeader() As String
            Get
                Return ins_intestazione_report
            End Get
            Set(ByVal Value As String)
                ins_intestazione_report = Value
            End Set
        End Property

        Private ins_piede_report As String
        Public Property ReportFooter() As String
            Get
                Return ins_piede_report
            End Get
            Set(ByVal Value As String)
                ins_piede_report = Value
            End Set
        End Property

        Private ins_responsabile As String
        Public Property Responsabile() As String
            Get
                Return ins_responsabile
            End Get
            Set(ByVal Value As String)
                ins_responsabile = Value
            End Set
        End Property



        Public Sub New()

        End Sub


    End Class




End Namespace
