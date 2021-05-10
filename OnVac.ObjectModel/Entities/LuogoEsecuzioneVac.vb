Namespace Entities

    <Serializable()>
    Public Class LuogoEsecuzioneVac
        Implements IDisposable


        Private _codiceLuogo As String
        Public ReadOnly Property CodiceLuogo() As String
            Get
                Return _codiceLuogo
            End Get
        End Property

        Private _descrizioneLuogo As String
        Public ReadOnly Property DescrizioneLuogo() As String
            Get
                Return _descrizioneLuogo
            End Get
        End Property


        Public Sub New(ByVal codice As String, ByVal descrizione As String)
            _codiceLuogo = codice
            _descrizioneLuogo = descrizione
        End Sub


        Public Sub Dispose() Implements System.IDisposable.Dispose
        End Sub

    End Class

End Namespace
