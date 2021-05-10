Imports Onit.OnAssistnet.OnVac.DAL


Public Class BizPostazioni
    Inherits BizClass

    Public Sub New(ByRef genericprovider As DbGenericProvider, ByRef settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfo As BizContextInfos)

        MyBase.New(genericprovider, settings, contextInfo, Nothing)

    End Sub

    ''' <summary>
    ''' Restituisce True se il numero di postazioni presenti nella t_ana_postazioni non supera il massimo possibile, 
    ''' impostato nel parametro MAXPOSTAZIONI. Restituisce False altrimenti.
    ''' Se il parametro non è gestito o non è criptato nel formato corretto, solleva un'eccezione tipizzata.
    ''' </summary>
    Public Function CheckAutorizzazioni() As Boolean

        ' --- Lettura parametro Max Postazioni --- '
        ' Lettura del parametro contenente il numero massimo di postazioni che si possono collegare all'applicativo
        ' Il parametro è criptato, viene decriptato dalla libreria dei settings nel momento in cui viene letto.
        ' Se non riesce a decriptarlo, perchè c'è un valore sbagliato, solleva un'eccezione. In questo caso non si accede al programma.
        Dim maxPostazioni As Integer
        Try
            maxPostazioni = Me.Settings.MAXPOSTAZIONI
        Catch ex As UnhandledSettingException
            ' Parametro presente su db ma non gestito dalla libreria
            ex.InternalPreserveStackTrace()
            Throw
        Catch ex As CryptedSettingException
            ' Parametro criptato contenente un valore errato 
            Throw New Exception(String.Format("Impossibile accedere all'applicativo.{0}" + ex.Message, "<br>"), ex)
        Catch ex As Exception
            ' Altra eccezione non gestita
            ex.InternalPreserveStackTrace()
            Throw
        End Try

        ' --- Calcolo numero totale postazioni presenti --- '
        Dim numPostazioni As Integer = Me.GenericProvider.Postazioni.CountPostazioni()

        ' --- Controllo autorizzazioni insufficienti --- '
        If (maxPostazioni < 0) OrElse (maxPostazioni > 0 AndAlso numPostazioni > maxPostazioni) Then
            ' Autorizzazioni insufficienti
            Return False
        End If

        Return True

    End Function

End Class
