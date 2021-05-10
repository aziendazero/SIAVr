Namespace Entities

    <Serializable()> _
    Public Class DatiOrdinamento

#Region " Properties "

        Public Campo As String

        Public Verso As Enumerators.VersoOrdinamento

#End Region

#Region " Constructors "

        Public Sub New()
        End Sub

        Public Sub New(campo As String, verso As String)

            Me.Campo = campo
            Me.Verso = CType([Enum].Parse(GetType(Enumerators.VersoOrdinamento), verso), Enumerators.VersoOrdinamento)

        End Sub

        Public Sub New(campo As String, verso As Enumerators.VersoOrdinamento)

            Me.Campo = campo
            Me.Verso = verso

        End Sub

        ' Inverte il verso dell'ordinamento specificato
        Public Sub InvertiOrdinamento()

            If Me.Verso = Enumerators.VersoOrdinamento.ASC Then
                Me.Verso = Enumerators.VersoOrdinamento.DESC
            Else
                Me.Verso = Enumerators.VersoOrdinamento.ASC
            End If

        End Sub

#End Region

    End Class

End Namespace
