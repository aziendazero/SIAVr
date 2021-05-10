Namespace Entities

    <Serializable()> _
    Public Class ConvocazioneVaccinazione

        Private _codicePaziente As Int32
        Public Property CodicePaziente() As Int32
            Get
                Return _codicePaziente
            End Get
            Set(ByVal value As Int32)
                _codicePaziente = value
            End Set
        End Property

        Private _dataConvocazione As DateTime
        Public Property DataConvocazione() As DateTime
            Get
                Return _dataConvocazione
            End Get
            Set(ByVal value As DateTime)
                _dataConvocazione = value
            End Set
        End Property

        Private _codiceConsultorio As String
        Public Property CodiceConsultorio() As String
            Get
                Return _codiceConsultorio
            End Get
            Set(ByVal value As String)
                _codiceConsultorio = value
            End Set
        End Property

        Private _descrizioneConsultorio As String
        Public Property DescrizioneConsultorio() As String
            Get
                Return _descrizioneConsultorio
            End Get
            Set(ByVal value As String)
                _descrizioneConsultorio = value
            End Set
        End Property

        Private _codiceAmbulatorio As String
        Public Property CodiceAmbulatorio() As String
            Get
                Return _codiceAmbulatorio
            End Get
            Set(ByVal value As String)
                _codiceAmbulatorio = value
            End Set
        End Property

        Private _descrizioneAmbulatorio As String
        Public Property DescrizioneAmbulatorio() As String
            Get
                Return _descrizioneAmbulatorio
            End Get
            Set(ByVal value As String)
                _descrizioneAmbulatorio = value
            End Set
        End Property

        Private _codicePazienteAlias As Nullable(Of Int32)
        Public Property CodicePazienteAlias() As Nullable(Of Int32)
            Get
                Return _codicePazienteAlias
            End Get
            Set(ByVal value As Nullable(Of Int32))
                _codicePazienteAlias = value
            End Set
        End Property

        Private _dataAppuntamento As Nullable(Of DateTime)
        Public Property DataAppuntamento() As Nullable(Of DateTime)
            Get
                Return _dataAppuntamento
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                _dataAppuntamento = value
            End Set
        End Property

        Private _codiceVaccinazione As String
        Public Property CodiceVaccinazione() As String
            Get
                Return _codiceVaccinazione
            End Get
            Set(ByVal value As String)
                _codiceVaccinazione = value
            End Set
        End Property

        Private _descrizioneVaccinazione As String
        Public Property DescrizioneVaccinazione() As String
            Get
                Return _descrizioneVaccinazione
            End Get
            Set(ByVal value As String)
                _descrizioneVaccinazione = value
            End Set
        End Property

        Private _numeroRichiamo As Int32
        Public Property NumeroRichiamo() As Int32
            Get
                Return _numeroRichiamo
            End Get
            Set(ByVal value As Int32)
                _numeroRichiamo = value
            End Set
        End Property

    End Class

End Namespace