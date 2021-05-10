Public Class PazientiScambiatiException
    Inherits Exception

    Public PazienteOriginale As Entities.Paziente
    Public PazienteModificato As Entities.Paziente
    Public MessaggioPersonalizzato As String

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(msg As String)

        MyBase.New(msg)

        MessaggioPersonalizzato = msg

    End Sub

    Public Sub New(pazienteOriginale As Entities.Paziente, pazienteModificato As Entities.Paziente)

        MyBase.New()

        Me.PazienteOriginale = pazienteOriginale
        Me.PazienteModificato = pazienteModificato

    End Sub

    Public Sub New(pazienteOriginale As Entities.Paziente, pazienteModificato As Entities.Paziente, msg As String)

        MyBase.New()

        Me.PazienteOriginale = pazienteOriginale
        Me.PazienteModificato = pazienteModificato
        Me.MessaggioPersonalizzato = msg

    End Sub

End Class
