Imports System.Collections.ObjectModel

Namespace Entities

    <Serializable()> _
    Public Class RitardoCicloConvocazione

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


        Private _DataAppuntamento1 As Nullable(Of DateTime)
        Public Property DataAppuntamento1() As Nullable(Of DateTime)
            Get
                Return _DataAppuntamento1
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                _DataAppuntamento1 = value
            End Set
        End Property

        Private _DataAppuntamento2 As Nullable(Of DateTime)
        Public Property DataAppuntamento2() As Nullable(Of DateTime)
            Get
                Return _DataAppuntamento2
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                _DataAppuntamento2 = value
            End Set
        End Property

        Private _DataAppuntamento3 As Nullable(Of DateTime)
        Public Property DataAppuntamento3() As Nullable(Of DateTime)
            Get
                Return _DataAppuntamento3
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                _DataAppuntamento3 = value
            End Set
        End Property

        Private _DataAppuntamento4 As Nullable(Of DateTime)
        Public Property DataAppuntamento4() As Nullable(Of DateTime)
            Get
                Return _DataAppuntamento4
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                _DataAppuntamento4 = value
            End Set
        End Property

        Private _DataInvio As Nullable(Of DateTime)
        Public Property DataInvio() As Nullable(Of DateTime)
            Get
                Return _DataInvio
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                _DataInvio = value
            End Set
        End Property

        Private _Note As String
        Public Property Note() As String
            Get
                Return _Note
            End Get
            Set(ByVal value As String)
                _Note = value
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

    End Class

End Namespace