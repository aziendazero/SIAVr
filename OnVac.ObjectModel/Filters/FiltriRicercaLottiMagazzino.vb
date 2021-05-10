Namespace Filters

    <Serializable()>
    Public Class FiltriRicercaLottiMagazzino

#Region " Properties "

        Private _codiceLotto As String
        Public Property CodiceLotto() As String
            Get
                Return _codiceLotto
            End Get
            Set(value As String)
                _codiceLotto = value
            End Set
        End Property

        Private _descrizioneLotto As String
        Public Property DescrizioneLotto() As String
            Get
                Return _descrizioneLotto
            End Get
            Set(value As String)
                _descrizioneLotto = value
            End Set
        End Property

        Private _codiceNomeCommerciale As String
        Public Property CodiceNomeCommerciale() As String
            Get
                Return _codiceNomeCommerciale
            End Get
            Set(value As String)
                _codiceNomeCommerciale = value
            End Set
        End Property

        Private _descrizioneNomeCommerciale As String
        Public Property DescrizioneNomeCommerciale() As String
            Get
                Return _descrizioneNomeCommerciale
            End Get
            Set(value As String)
                _descrizioneNomeCommerciale = value
            End Set
        End Property

        Private _noLottiScaduti As Boolean
        Public Property NoLottiScaduti() As Boolean
            Get
                Return _noLottiScaduti
            End Get
            Set(value As Boolean)
                _noLottiScaduti = value
            End Set
        End Property

        Private _soloLottiSequestrati As Boolean
        Public Property SoloLottiSequestrati() As Boolean
            Get
                Return _soloLottiSequestrati
            End Get
            Set(value As Boolean)
                _soloLottiSequestrati = value
            End Set
        End Property

        Private _noLottiScortaNulla As Boolean
        Public Property NoLottiScortaNulla() As Boolean
            Get
                Return _noLottiScortaNulla
            End Get
            Set(value As Boolean)
                _noLottiScortaNulla = value
            End Set
        End Property
        Public Property CodiceDistretto As String

#End Region

#Region " Constructors "

        Public Sub New()

            Me.New(String.Empty, String.Empty, String.Empty, String.Empty, False, False, False, String.Empty)

        End Sub

        Public Sub New(noLottiScaduti As Boolean)

            Me.New(String.Empty, String.Empty, String.Empty, String.Empty, noLottiScaduti, False, False, String.Empty)

        End Sub

        Public Sub New(codiceLotto As String, descrizioneLotto As String, codiceNomeCommerciale As String, descrizioneNomeCommerciale As String, noLottiScaduti As Boolean, soloLottiSequestrati As Boolean, noLottiScortaNulla As Boolean, codDistretto As String)

            Me.CodiceLotto = codiceLotto
            Me.DescrizioneLotto = descrizioneLotto
            Me.CodiceNomeCommerciale = codiceNomeCommerciale
            Me.DescrizioneNomeCommerciale = descrizioneNomeCommerciale
            Me.NoLottiScaduti = noLottiScaduti
            Me.SoloLottiSequestrati = soloLottiSequestrati
            Me.NoLottiScortaNulla = noLottiScortaNulla
            CodiceDistretto = codDistretto

        End Sub

#End Region

    End Class

End Namespace