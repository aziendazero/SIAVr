Imports System.Collections.Generic

Namespace Entities

    <Serializable()> _
    Public Class Malattia

#Region " Properties "

        Private _codiceMalattia As String
        Public Property CodiceMalattia() As String
            Get
                Return _codiceMalattia
            End Get
            Set(ByVal value As String)
                _codiceMalattia = value
            End Set
        End Property

        Private _descrizioneMalattia As String
        Public Property DescrizioneMalattia() As String
            Get
                Return _descrizioneMalattia
            End Get
            Set(ByVal value As String)
                _descrizioneMalattia = value
            End Set
        End Property

        Private _flagVisita As Boolean
        Public Property FlagVisita() As Boolean
            Get
                Return _flagVisita
            End Get
            Set(ByVal value As Boolean)
                _flagVisita = value
            End Set
        End Property

        Private _codiceEsenzione As String
        Public Property CodiceEsenzione() As String
            Get
                Return _codiceEsenzione
            End Get
            Set(ByVal value As String)
                _codiceEsenzione = value
            End Set
        End Property

        ' TODO [Entities.Malattia]: esiste un'anagrafica delle esenzioni? Servono altri campi?
        Private _descrizioneEsenzione As String
        Public Property DescrizioneEsenzione() As String
            Get
                Return _descrizioneEsenzione
            End Get
            Set(ByVal value As String)
                _descrizioneEsenzione = value
            End Set
        End Property

        Private _tipologia As List(Of TipologiaMalattia)
        Public Property Tipologia() As List(Of TipologiaMalattia)
            Get
                Return _tipologia
            End Get
            Set(ByVal value As List(Of TipologiaMalattia))
                _tipologia = value
            End Set
        End Property

        Public ReadOnly Property DescrizioneMalattiaCodiceEsenzione() As String
            Get
                Return String.Format("{0} - {1}", _descrizioneMalattia, _codiceEsenzione)
            End Get
        End Property
        Public ReadOnly Property CodiceMalattiaCodiceEsenzione() As String
            Get
                Return String.Format("{0}|{1}", _codiceMalattia, _codiceEsenzione)
            End Get
        End Property

#End Region

        Public Sub New()
            Me.Tipologia = New List(Of Malattia.TipologiaMalattia)
        End Sub

#Region " Sub class "

        Public Class TipologiaMalattia

            Private _codice As String
            Public Property Codice() As String
                Get
                    Return _codice
                End Get
                Set(ByVal value As String)
                    _codice = value
                End Set
            End Property

            Private _descrizione As String
            Public Property Descrizione() As String
                Get
                    Return _descrizione
                End Get
                Set(ByVal value As String)
                    _descrizione = value
                End Set
            End Property

            Private _flagDefault As Boolean
            Public Property FlagDefault() As Boolean
                Get
                    Return _flagDefault
                End Get
                Set(ByVal value As Boolean)
                    _flagDefault = value
                End Set
            End Property

        End Class

#End Region

    End Class

End Namespace
