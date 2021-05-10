Namespace Entities

    Public Class Convocazione

        Private _paz_codice As Integer
        Public Property Paz_codice() As Integer
            Get
                Return _paz_codice
            End Get
            Set(ByVal Value As Integer)
                _paz_codice = Value
            End Set
        End Property

        Private _paz_codice_old As Integer
        Public Property Paz_codice_old() As Integer
            Get
                Return _paz_codice_old
            End Get
            Set(ByVal Value As Integer)
                _paz_codice_old = Value
            End Set
        End Property

        Private _data_cnv As Date
        Public Property Data_CNV() As Date
            Get
                Return _data_cnv
            End Get
            Set(ByVal Value As Date)
                _data_cnv = Value
            End Set
        End Property

        Private _eta_pomeriggio As String
        Public Property EtaPomeriggio() As String
            Get
                Return _eta_pomeriggio
            End Get
            Set(ByVal Value As String)
                _eta_pomeriggio = Value
            End Set
        End Property

        Private _rinvio As String
        Public Property Rinvio() As String
            Get
                Return _rinvio
            End Get
            Set(ByVal Value As String)
                _rinvio = Value
            End Set
        End Property

        Private _data_appuntamento As Date
        Public Property DataAppuntamento() As Date
            Get
                Return _data_appuntamento
            End Get
            Set(ByVal Value As Date)
                _data_appuntamento = Value
            End Set
        End Property

        Private _tipo_appuntamento As String
        Public Property TipoAppuntamento() As String
            Get
                Return _tipo_appuntamento
            End Get
            Set(ByVal Value As String)
                _tipo_appuntamento = Value
            End Set
        End Property

        Private _durata_appuntamento As Integer
        Public Property Durata_Appuntamento() As Integer
            Get
                Return _durata_appuntamento
            End Get
            Set(ByVal Value As Integer)
                _durata_appuntamento = Value
            End Set
        End Property

        Private _data_invio As Date
        Public Property DataInvio() As Date
            Get
                Return _data_invio
            End Get
            Set(ByVal Value As Date)
                _data_invio = Value
            End Set
        End Property

        Private _cns_codice As String
        Public Property Cns_Codice() As String
            Get
                Return _cns_codice
            End Get
            Set(ByVal Value As String)
                _cns_codice = Value
            End Set
        End Property

        Private _ute_id As Integer
        Public Property IdUtente() As Integer
            Get
                Return _ute_id
            End Get
            Set(ByVal Value As Integer)
                _ute_id = Value
            End Set
        End Property

        Private _data_primo_app As Date
        Public Property DataPrimoAppuntamento() As Date
            Get
                Return _data_primo_app
            End Get
            Set(ByVal Value As Date)
                _data_primo_app = Value
            End Set
        End Property

        Public Property CodiceAmbulatorio As Integer

        Public Property CampagnaVaccinale As String

        Public Property DataInserimento As DateTime?

        Public Property IdUtenteInserimento As Long?

        Sub New()
        End Sub

        Sub New(data_cnv As Date, paz_codice As Integer, cns_codice As String, durata_appuntamento As Integer)
            _data_cnv = data_cnv
            _paz_codice = paz_codice
            _cns_codice = cns_codice
            _durata_appuntamento = durata_appuntamento
        End Sub

    End Class

End Namespace
