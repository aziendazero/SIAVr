Namespace Entities

	<Serializable()>
	Public Class Usl

		Public Property Codice As String
		Public Property Descrizione As String
		Public Property IDApplicazione As String
		Public Property FlagConsensoDatiVaccinaliCentralizzati As Boolean
		Public Property FlagAbilitazioneDatiVaccinaliCentralizzati As Boolean
		Public Property IsApplicazioneUnificata As Boolean

	End Class
	<Serializable()>
	Public Class UslDistretto
		Public Property Codice As String
		Public Property Descrizione As String
		Public ReadOnly Property DesCodice As String
			Get
				Dim cod As String = String.Empty
				If Not String.IsNullOrEmpty(Codice) Then
					cod = String.Format(" - {0}", Codice)
				End If

				Return String.Format("{0} {1}", Descrizione, cod)
			End Get
		End Property
	End Class

End Namespace
