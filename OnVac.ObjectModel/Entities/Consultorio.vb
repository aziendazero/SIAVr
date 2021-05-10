Namespace Entities

    <Serializable()>
    Public Class Consultorio

        Public Property Codice As String
        Public Property Descrizione As String
        Public Property Ambulatorio As Ambulatorio

        Public Sub New()
            Me.Ambulatorio = New Ambulatorio()
        End Sub

    End Class

	<Serializable()>
	Public Class Ambulatorio
		Public Property Codice As Integer
		Public Property Descrizione As String
		Public Property DataApertura As DateTime?
		Public Property DataChiusura As DateTime?
	End Class

	<Serializable()>
	Public Class ConsultorioAperti

		Public Property Codice As String
		Public Property Descrizione As String
		Public Property DataChiusura As Date?
		Public ReadOnly Property DescrizioneChiuso As String
			Get
				If DataChiusura.HasValue AndAlso DataChiusura.Value < Today Then
					Return String.Format("chiuso")
				End If
				Return String.Empty
			End Get
		End Property


	End Class
	<Serializable()>
	Public Class ConsultorioUlss

		Public Property CodiceConsultorio As String
		Public Property DescrizioneConsultorio As String
		Public Property CodiceDistretto As String
		Public Property DescrizioneDistretto As String
		Public Property CodiceUlss As String
		Public Property DescrizioneUlss As String

		Public ReadOnly Property DesAmbitoDistretto As String
			Get
				Dim ambito As String = String.Empty
				If Not String.IsNullOrEmpty(CodiceUlss) Then
					ambito = String.Format("{0} - ", CodiceUlss)
				End If
				Return String.Format("{0}{1}", ambito, DescrizioneDistretto).TrimStart()
			End Get
		End Property


	End Class

End Namespace
