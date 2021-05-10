Namespace Entities

    <Serializable()> _
    Public Class IndirizzoPaziente

#Region " Properties "

        Public Property Codice() As Integer
        Public Property Tipo() As Enumerators.TipoIndirizzo
        Public Property Libero() As String
        Public Property Paziente() As Integer
        Public Property IsNew() As Boolean
        Public Property IsModified() As Boolean
        Public Property Manuale() As Boolean
        Public Property Via() As Entities.Via
        Public Property NCivico() As String
        Public Property CivicoLettera() As String
        Public Property Interno() As String
        Public Property Lotto() As String
        Public Property Palazzina() As String
        Public Property Scala() As String
        Public Property Piano() As String

#End Region

        Public Sub New(ByVal codice As Integer, ByVal codicePaziente As Integer)

            Me.New()

            If codice <> -1 Then
                Me.IsNew = False
                Me.Codice = codice
            End If

            Me.Paziente = codicePaziente

        End Sub

        Public Sub New()

            Me.Via = New Via()

            Me.IsNew = True

            Me.Libero = String.Empty
            Me.NCivico = String.Empty
            Me.Interno = String.Empty
            Me.Lotto = String.Empty
            Me.Palazzina = String.Empty
            Me.Scala = String.Empty
            Me.Piano = String.Empty
            Me.CivicoLettera = String.Empty

        End Sub

    End Class

End Namespace