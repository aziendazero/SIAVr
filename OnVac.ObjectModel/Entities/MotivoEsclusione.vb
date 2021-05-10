Imports System.Collections.Generic

Namespace Entities

    <Serializable()> _
    Public Class MotivoEsclusione


#Region " Property "

        Private _CodiceEsclusione As String
        Public Property CodiceEsclusione() As String
            Get
                Return _CodiceEsclusione
            End Get
            Set(ByVal value As String)
                _CodiceEsclusione = value
            End Set
        End Property

        Private _calcoloScadenza As Enumerators.MotiviEsclusioneCalcoloScadenza
        Public Property CalcoloScadenza() As Enumerators.MotiviEsclusioneCalcoloScadenza
            Get
                Return _calcoloScadenza
            End Get
            Set(ByVal value As Enumerators.MotiviEsclusioneCalcoloScadenza)
                _calcoloScadenza = value
            End Set
        End Property

        Private _Scadenze As List(Of Scadenza)
        Public Property Scadenze() As List(Of Scadenza)
            Get
                Return _Scadenze
            End Get
            Set(ByVal value As List(Of Scadenza))
                _Scadenze = value
            End Set
        End Property

#End Region

        Public Sub New()
            _Scadenze = New List(Of Scadenza)
        End Sub

#Region " Sub class "

        Public Class Scadenza

            Private _id As Integer
            Public Property ID() As Integer
                Get
                    Return _id
                End Get
                Set(ByVal value As Integer)
                    _id = value
                End Set
            End Property

            Private _Mesi As Integer
            Public Property Mesi() As Integer
                Get
                    Return _Mesi
                End Get
                Set(ByVal value As Integer)
                    _Mesi = value
                End Set
            End Property

            Private _Anni As Integer
            Public Property Anni() As Integer
                Get
                    Return _Anni
                End Get
                Set(ByVal value As Integer)
                    _Anni = value
                End Set
            End Property

            Private _CodiciVaccinazioni As List(Of String)
            Public Property CodiciVaccinazioni() As List(Of String)
                Get
                    Return _CodiciVaccinazioni
                End Get
                Set(ByVal value As List(Of String))
                    _CodiciVaccinazioni = value
                End Set
            End Property

        End Class

#End Region

    End Class

End Namespace
