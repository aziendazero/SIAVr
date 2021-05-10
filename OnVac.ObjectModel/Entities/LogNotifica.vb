Imports System.Collections.Generic

Namespace Entities

	<Serializable>
	Public Class LogNotificaRicevuta
		Public Property IdNotifica As Long
		Public Property IdMessaggio As String
		Public Property CodiceCentralePaziente As String
		Public Property CodiceCentralePazienteAlias As String
		Public Property DataRicezione As DateTime
		Public Property EntityPazienteSerializzata As String
		Public Property DataInvioRisposta As DateTime?
		Public Property NumeroRicezioni As Integer?
		Public Property Operazione As Enumerators.OperazioneLogNotifica?
		Public Property RisultatoElaborazione As String
		Public Property MessaggioElaborazione As String
		Public Property Esenzioni As String
		Public Property ServiceRequest As String
		Public Property ServiceResponse As String
		Public Property RepositoryUniqueId As String
		Public Property DocumentUniqueId As String
		Public Property EntitySerializzata As String
	End Class
	<Serializable>
	Public Class FiltriLogNotifiche

		Public IdMessaggio As String
		Public DataRicezioneDa As DateTime?
		Public DataRicezioneA As DateTime?
		Public Operazioni As List(Of Enumerators.OperazioneLogNotifica)

		Public DataNascitaDa As DateTime?
		Public DataNascitaA As DateTime?

		Public Risultato As String
		Public CodiceMPI As String
		Public CF As String
		Public CodiceLocale As String

	End Class
	<Serializable>
	Public Class LogNotificaRicevutaACN
		Inherits LogNotificaRicevuta

		Public Property CodiceLocalePaziente As Long
		Public Property Cognome As String
		Public Property Nome As String
		Public Property DataNascita As DateTime
		Public Property CodiceFiscale As String
		Public Property CodiceRegionale As String

	End Class
	<Serializable>
	Public Class LogNotificaInviata
		Public Property IdNotifica As Long
		Public Property CodiceCentralePaziente As String
		Public Property CodiceLocalePaziente As Long
		Public Property Operazione As Enumerators.OperazioneLogNotifica?
		Public Property EntityPazienteSerializzata As String
		Public Property EntityPazientePrecedenteSerializzata As String
		Public Property DataRicezioneRisposta As DateTime?
		Public Property MessaggioRisposta As String
		Public Property RisultatoRisposta As String
		Public Property ServiceRequest As String
		Public Property ServiceResponse As String
		Public Property DataInserimentoNotifica As DateTime
		Public Property IdUtenteInserimentoNotifica As Long
		Public Property DataInvio As DateTime?
		Public Property Stato As Enumerators.StatoLogNotificaInviata
		Public Property IdMessaggio As String
		Public Property IdPolo As Long?
		Public Property CdaFse As String
		Public Property NewDocUniqId As String
		Public Property ReposUniqIdFse As String
		Public Property SourceIpFse As String
        Public Property DestiUserIdFse As String
        Public Property NumeroInvii As Integer?
        Public Property CodiceUsl As String
        Public Property Operatore As String
    End Class

End Namespace
