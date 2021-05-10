Namespace Entities

	<Serializable()>
	Public Class ConsultorioOperatore

		Public Property CodiceOperatore As String
		Public Property CodiceConsultorio As String
		Public Property DescrizioneConsultorio As String
		Public Property ConsultorioDefault As Boolean
		Public Property Abilitato As Boolean
		Public Property CodiceDistretto As String
		Public Property DescrizioneDistretto As String
		Public Property CodiceUsl As String
		Public Property DescrizioneUsl As String
		Public ReadOnly Property DescrCentro As String
			Get
				Return String.Format("{0} - {1}", DescrizioneConsultorio, CodiceConsultorio)
			End Get
		End Property
		Public ReadOnly Property DesAmbitoDistretto As String
			Get
				Dim usl As String = String.Empty
				If Not String.IsNullOrEmpty(CodiceUsl) Then
					usl = String.Format(" - {0}", CodiceUsl)
				End If
				Return String.Format("{0}{1}", DescrizioneDistretto, usl).TrimStart()
			End Get
		End Property
	End Class

End Namespace
