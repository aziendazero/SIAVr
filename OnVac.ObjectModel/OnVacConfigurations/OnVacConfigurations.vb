Imports System.Configuration

Namespace OnVacConfigSectionHandler

    Public Class OnVacConfigHandler
        Inherits ConfigurationSection

        Public Sub New()
        End Sub

        <ConfigurationProperty("ULSSUnificata")>
        Public Property ULSSUnificataSection() As ULSSUnificata
            Get
                Return DirectCast(Me("ULSSUnificata"), ULSSUnificata)
            End Get
            Set(value As ULSSUnificata)
                Me("ULSSUnificata") = value
            End Set
        End Property

    End Class

    Public Class ULSSUnificata
        Inherits ConfigurationElement

        Public Sub New()
        End Sub

        Public Sub New(codice As String, descrizione As String, idApplicazione As String, flagAbilitazione As Boolean, flagConsenso As Boolean)
            Me.Codice = codice
            Me.Descrizione = descrizione
            Me.IdApplicazione = idApplicazione
            Me.FlagAbilitazione = flagAbilitazione
            Me.FlagConsenso = flagConsenso
        End Sub

        <ConfigurationProperty("codice", IsRequired:=True)>
        Public Property Codice As String
            Get
                Return Me("codice").ToString()
            End Get
            Set(value As String)
                Me("codice") = value
            End Set
        End Property

        <ConfigurationProperty("descrizione")>
        Public Property Descrizione As String
            Get
                Return Convert.ToString(Me("descrizione"))
            End Get
            Set(value As String)
                Me("descrizione") = value
            End Set
        End Property

        <ConfigurationProperty("idApplicazione")>
        Public Property IdApplicazione As String
            Get
                Return Convert.ToString(Me("idApplicazione"))
            End Get
            Set(value As String)
                Me("idApplicazione") = value
            End Set
        End Property

        <ConfigurationProperty("flagAbilitazione")>
        Public Property FlagAbilitazione As Boolean
            Get
                Return Convert.ToBoolean(Me("flagAbilitazione"))
            End Get
            Set(value As Boolean)
                Me("flagAbilitazione") = value
            End Set
        End Property

        <ConfigurationProperty("flagConsenso")>
        Public Property FlagConsenso As Boolean
            Get
                Return Convert.ToBoolean(Me("flagConsenso"))
            End Get
            Set(value As Boolean)
                Me("flagConsenso") = value
            End Set
        End Property

    End Class

End Namespace