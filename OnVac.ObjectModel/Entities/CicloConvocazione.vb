Imports System.Collections.ObjectModel

Namespace Entities

    <Serializable()>
    Public Class CicloConvocazione

        Private _DataConvocazione As DateTime
        Public Property DataConvocazione() As DateTime
            Get
                Return _DataConvocazione
            End Get
            Set(ByVal value As DateTime)
                _DataConvocazione = value
            End Set
        End Property

        Private _CodicePaziente As Int64
        Public Property CodicePaziente() As Int64
            Get
                Return _CodicePaziente
            End Get
            Set(ByVal value As Int64)
                _CodicePaziente = value
            End Set
        End Property

        Private _CodiceCiclo As String
        Public Property CodiceCiclo() As String
            Get
                Return _CodiceCiclo
            End Get
            Set(ByVal value As String)
                _CodiceCiclo = value
            End Set
        End Property

        Private _NumeroSeduta As Int16
        Public Property NumeroSeduta() As Int16
            Get
                Return _NumeroSeduta
            End Get
            Set(ByVal value As Int16)
                _NumeroSeduta = value
            End Set
        End Property

        Private _CodicePazientePrecedente As Nullable(Of Int64)
        Public Property CodicePazientePrecedente() As Nullable(Of Int64)
            Get
                Return _CodicePazientePrecedente
            End Get
            Set(ByVal value As Nullable(Of Int64))
                _CodicePazientePrecedente = value
            End Set
        End Property

        Private _FlagGiorniPosticipo As String
        Public Property FlagGiorniPosticipo() As String
            Get
                Return _FlagGiorniPosticipo
            End Get
            Set(ByVal value As String)
                _FlagGiorniPosticipo = value
            End Set
        End Property

        Private _FlagPosticipoSeduta As String
        Public Property FlagPosticipoSeduta() As String
            Get
                Return _FlagPosticipoSeduta
            End Get
            Set(ByVal value As String)
                _FlagPosticipoSeduta = value
            End Set
        End Property

        Private _NumeroSollecito As Nullable(Of Int16)
        Public Property NumeroSollecito() As Nullable(Of Int16)
            Get
                Return _NumeroSollecito
            End Get
            Set(ByVal value As Nullable(Of Int16))
                _NumeroSollecito = value
            End Set
        End Property

        Private _DataInvioSollecito As Nullable(Of Date)
        Public Property DataInvioSollecito() As Nullable(Of Date)
            Get
                Return _DataInvioSollecito
            End Get
            Set(ByVal value As Nullable(Of Date))
                _DataInvioSollecito = value
            End Set
        End Property

        Private _Ritardo As RitardoCicloConvocazione
        Public Property Ritardo() As RitardoCicloConvocazione
            Get
                Return _Ritardo
            End Get
            Set(ByVal value As RitardoCicloConvocazione)
                _Ritardo = value
            End Set
        End Property

        Public Property DataInserimento As DateTime?

        Public Property IdUtenteInserimento As Int64?

    End Class

End Namespace