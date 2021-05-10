<script language=javascript>
	//Vengono controllati i campi obbligatori
	<%
		Dim valid As Boolean = True
		dim strErr As String
		dim htDati As Hashtable=OnVacUtility.GetValoreCampiPaziente(Me.Settings, Me.IsGestioneCentrale, "PAZ_CNS_CODICE", "PAZ_NOME", "PAZ_COGNOME", "PAZ_DATA_NASCITA", "PAZ_COM_CODICE_RESIDENZA", "PAZ_STATO_ANAGRAFICO")
		
		If Not htDati Is Nothing Then
			Dim cnsCod As Object=htDati("PAZ_CNS_CODICE")
			Dim pazNome As Object=htDati("PAZ_NOME")
			Dim pazCognome As Object=htDati("PAZ_COGNOME")
			Dim pazDataNascita As Object=htDati("PAZ_DATA_NASCITA")
			Dim pazComune As Object=htDati("PAZ_COM_CODICE_RESIDENZA")
			Dim pazStatoAnagrafico As Object=htDati("PAZ_STATO_ANAGRAFICO")
		
			If cnsCod Is DBNull.Value Then
				valid=False
				strErr = "<LI>Il centro vaccinale di appartenza deve essere indicato obbligatoriamente!<BR>"
			End If
			If (pazNome Is DBNull.Value) Or (pazCognome Is DbNull.Value) Or (pazDataNascita Is DBNull.Value) Or (pazComune Is DBNull.Value) Or (pazStatoAnagrafico Is DBNull.Value) Then
				valid=False
				strErr &= "<LI>Uno o più dei campi obbligatori (in giallo) non sono impostati!"
			End If
		End If
	%>
</script>
