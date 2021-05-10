Namespace Entities.Consenso

    <Serializable()>
    Public Class StatoConsensoPaziente

        Private _codicePaziente As Double
        Public Property CodicePaziente() As Double
            Get
                Return _codicePaziente
            End Get
            Set(value As Double)
                _codicePaziente = value
            End Set
        End Property

        Private _codStatoConsenso As String
        ''' <summary>
        ''' Codice dell'icona che indica lo stato dei consensi del paziente
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property CodiceStatoConsenso() As String
            Get
                Return _codStatoConsenso
            End Get
            Set(value As String)
                _codStatoConsenso = value
            End Set
        End Property

        Private _descrStatoConsenso As String
        ''' <summary>
        ''' Descrizione dell'icona che indica lo stato dei consensi del paziente
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DescrizioneStatoConsenso() As String
            Get
                Return _descrStatoConsenso
            End Get
            Set(value As String)
                _descrStatoConsenso = value
            End Set
        End Property

        Private _urlIconaStatoConsenso As String
        ''' <summary>
        ''' Url dell'icona che indica lo stato dei consensi del paziente
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property UrlIconaStatoConsenso() As String
            Get
                Return _urlIconaStatoConsenso
            End Get
            Set(value As String)
                _urlIconaStatoConsenso = value
            End Set
        End Property

        Private _ordinamentoStatoConsenso As String
        ''' <summary>
        ''' Ordine di importanza delle icone che indicano lo stato dei consensi del paziente
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property OrdinamentoStatoConsenso() As String
            Get
                Return _ordinamentoStatoConsenso
            End Get
            Set(value As String)
                _ordinamentoStatoConsenso = value
            End Set
        End Property

        Private _controllo As Enumerators.ControlloConsenso
        ''' <summary>
        ''' Flag relativo al controllo (bloccante, warning, non bloccante).
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Controllo() As Enumerators.ControlloConsenso
            Get
                Return _controllo
            End Get
            Set(value As Enumerators.ControlloConsenso)
                _controllo = value
            End Set
        End Property

        ''' <summary>
        ''' Indica, in base al livello, se deve essere bloccato l'accesso ai dati da maschere utilizzate esternamente
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property BloccoAccessiEsterni As Boolean

    End Class

End Namespace
