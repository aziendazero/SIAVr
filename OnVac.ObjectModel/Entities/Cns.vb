Namespace Entities

    <Serializable()>
    Public Class Cns

#Region "Public properties"

        Public Property Codice() As String
        Public Property Descrizione() As String
        Public Property Tipo() As String
        Public Property Comune() As String
        Public Property Indirizzo() As String
        Public Property Cap() As String
        Public Property Telefono() As String
        Public Property Stampa1() As String
        Public Property Stampa2() As String
        Public Property Email() As String

#End Region

#Region " Constructors "

        Sub New(codice As String, descrizione As String, tipo As String, comune As String, indirizzo As String, cap As String, telefono As String, stampa1 As String, stampa2 As String, email As String)

            Me.Codice = codice
            Me.Descrizione = descrizione
            Me.Tipo = tipo
            Me.Comune = comune
            Me.Indirizzo = indirizzo
            Me.Cap = cap
            Me.Telefono = telefono
            Me.Stampa1 = stampa1
            Me.Stampa2 = stampa2
            Me.Email = email

        End Sub

#End Region

    End Class

End Namespace