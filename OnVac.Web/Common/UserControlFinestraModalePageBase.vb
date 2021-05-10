Imports Onit.OnAssistnet.OnVac


Namespace Common

    ''' <summary>
    ''' Classe che estende la pagina base degli user control di OnVac, per la gestione degli user control contenuti in finestre modali.
    ''' In particolare, questa classe eredita la gestione dei parametri applicativi.
    ''' </summary>
    Public Class UserControlFinestraModalePageBase
        Inherits Common.UserControlPageBase


            Public Overridable Sub LoadModale()
                Return
            End Sub

            Public Property ModaleName() As String
                Get
                    Return ViewState("ModaleName")
                End Get
                Set(ByVal Value As String)
                    ViewState("ModaleName") = Value
                End Set
            End Property

    End Class

End Namespace

