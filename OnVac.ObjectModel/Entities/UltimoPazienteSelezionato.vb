Namespace Entities

    ''' <summary>
    ''' Dati dell'ultimo paziente selezionato 
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class UltimoPazienteSelezionato

        Public Property CodicePazienteCentrale As String
        Public Property CodicePazienteLocale As String

        Public Sub New()
            Me.CodicePazienteCentrale = String.Empty
            Me.CodicePazienteLocale = String.Empty
        End Sub

        Public Sub New(codiceCentrale As String, codiceLocale As String)
            Me.CodicePazienteCentrale = codiceCentrale
            Me.CodicePazienteLocale = codiceLocale
        End Sub

    End Class

End Namespace
