Namespace Entities

    <Serializable()> _
    Public Class Associazione

#Region " Property "


        Private _Codice As String
        Public Property Codice() As String
            Get
                Return _Codice
            End Get
            Set(ByVal value As String)
                _Codice = value
            End Set
        End Property


        Private _Dose As String
        Public Property Dose() As String
            Get
                Return _Dose
            End Get
            Set(ByVal value As String)
                _Dose = value
            End Set
        End Property


        Private _DataEffettuazione As String
        Public Property DataEffettuazione() As String
            Get
                Return _DataEffettuazione
            End Get
            Set(ByVal value As String)
                _DataEffettuazione = value
            End Set
        End Property


        Private _scaduta As String
        Public Property Scaduta() As String
            Get
                Return _scaduta
            End Get
            Set(ByVal value As String)
                _scaduta = value
            End Set
        End Property


#End Region

    End Class

End Namespace
