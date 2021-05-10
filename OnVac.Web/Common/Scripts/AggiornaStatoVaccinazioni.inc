<script type="text/javascript" language="javascript">
function AggiornaLeftTitles()
{
	// Inadempienze
	<% Dim vacInadempienze as Integer=OnVacUtility.CalcolaVacInadempienze(Me.Settings, Me.IsGestioneCentrale) %>
	SetTextLeftMenu('Inadempienze', 'Inadempienze<%=IIf(vacInadempienze>0," (" & vacInadempienze & ")","")%>',<%=IIf(vacInadempienze>0,"1","2")%>,0);

	// Reazioni Avverse
	<% Dim vacAvverse as Integer=OnVacUtility.CalcolaReazioniAvverse(Me.Settings, Me.IsGestioneCentrale) %>
	SetTextLeftMenu('Reazioni Avverse', 'Reazioni Avverse<%=IIf(vacAvverse>0," (" & vacAvverse & ")","")%>',<%=IIf(vacAvverse>0,"1","2")%>,0);

	// Vaccinazioni Escluse
	<% Dim vacEscluse as Integer=OnVacUtility.CalcolaVacEscluse(Me.Settings, Me.IsGestioneCentrale) %>
	SetTextLeftMenu('Escluse', 'Escluse<%=IIf(vacEscluse>0," (" & vacEscluse & ")","")%>',<%=IIf(vacEscluse>0,"1","2")%>,0);

	// Vaccinazioni Eseguite
	<% Dim vacEseguite as Integer=OnVacUtility.CalcolaVacEseguite(Me.Settings, Me.IsGestioneCentrale) %>
	SetTextLeftMenu('Eseguite', 'Eseguite<%=IIf(vacEseguite>0," (" & vacEseguite & ")","")%>',<%=IIf(vacEseguite>0,"1","2")%>,0);

	// Vaccinazioni Programmate
	<% Dim vacProgrammate as Integer=OnVacUtility.CalcolaVacProgrammate(Me.Settings, Me.IsGestioneCentrale) %>
	SetTextLeftMenu('Programmate', 'Programmate<%=IIf(vacProgrammate>0," (" & vacProgrammate & ")","")%>',<%=IIf(vacProgrammate>0,"1","2")%>,0);
	
	// Visite
	<% Dim numVisite as Integer=OnVacUtility.CalcolaVisite(Me.Settings, Me.IsGestioneCentrale) %>
	SetTextLeftMenu('Visite', 'Visite<%=IIf(numVisite>0," (" & numVisite & ")","")%>',<%=IIf(numVisite>0,"1","2")%>,0);
	
	// Appuntamenti
	<% Dim convocazioniApp as String=OnVacUtility.CalcolaConvocazioniAppuntamenti(Me.Settings, Me.IsGestioneCentrale, Request.QueryString) %>
	SetTextLeftMenu('Ricerca Appuntamenti', 'Ricerca Appuntamenti<%=IIf(convocazioniApp<>"","<br>" & convocazioniApp,"")%>',<%=IIf(convocazioniApp<>"","1","2")%>,0);
	
	// Gestione Pazienti: ritardi e mantoux
	<% Dim info as String = OnVacUtility.GetInfoGestionePazientiLeftMenu(Me.Settings, Me.IsGestioneCentrale)  %>
	SetTextLeftMenu('Gestione Pazienti', 'Gestione Pazienti<%=info%>',<%=IIf(Not String.IsNullOrWhiteSpace(info),"1","2")%>,0);

	// Note
	<% Dim numNotePaz as String = OnVacUtility.CalcolaNumeroNotePaziente(Me.Settings, Me.IsGestioneCentrale)  %>
	SetTextLeftMenu('Note', 'Note<%=IIf(numNotePaz>0," (" & numNotePaz & ")","")%>',<%=IIf(numNotePaz>0,"1","2")%>,0);
	
	// Bilanci
	<% Dim bilanciPaz as String=OnVacUtility.CalcolaNumeroBilanci(Me.Settings, Me.IsGestioneCentrale) %>
	SetTextLeftMenu('Visione Anamnesi', 'Visione Anamnesi<%=IIf(bilanciPaz > 0,"<br>" & "(" & bilanciPaz & ")","")%>',<%=IIf(bilanciPaz>0,"1","2")%>,0);
	
	// Documenti
	<% Dim documentiPaz as String=OnVacUtility.CalcolaNumeroDocumenti(Me.Settings, Me.IsGestioneCentrale) %>
	SetTextLeftMenu('Documenti', 'Documenti<%=IIf(documentiPaz > 0,"<br>" & "(" & documentiPaz & ")","")%>',<%=IIf(documentiPaz>0,"1","2")%>,0);
	
	// Rifiuti
	<% Dim rifiutiPaz as String=OnVacUtility.CalcolaNumeroRifiuti(Me.Settings, Me.IsGestioneCentrale) %>
	SetTextLeftMenu('Rifiuti', 'Rifiuti<%=IIf(rifiutiPaz > 0,"<br>" & "(" & rifiutiPaz & ")","")%>',<%=IIf(rifiutiPaz>0,"1","2")%>,0);
  
	// Consulenze
	<% Dim consulenzePaz as String=OnVacUtility.CalcolaNumeroConsulenze(Me.Settings, Me.IsGestioneCentrale) %>
	SetTextLeftMenu('Consulenze', 'Consulenze<%=IIf(consulenzePaz > 0,"<br>" & "(" & consulenzePaz & ")","")%>',<%=IIf(consulenzePaz>0,"1","2")%>,0);

	// Episodi
	<% Dim episodiPaz as Integer = OnVacUtility.CalcolaNumeroEpisodiCovid(Settings) %>
	SetTextLeftMenu('Episodi', 'Episodi<%= If(episodiPaz > 0, "<br/>(" + episodiPaz.ToString() + ")", String.Empty) %>', <%= If(episodiPaz > 0, "1", "2") %>, 0);

	// Tamponi
	<% Dim tamponiPaz as Integer = OnVacUtility.CalcolaNumeroTamponiCovid(Settings) %>
	SetTextLeftMenu('Tamponi', 'Tamponi<%= If(tamponiPaz > 0, "<br/>(" + tamponiPaz.ToString() + ")", String.Empty) %>', <%= If(tamponiPaz > 0, "1", "2") %>, 0);
  
	// Test Sierologici
	<% Dim testSierologiciPaz as Integer = OnVacUtility.CalcolaNumeroTestSierologiciCovid(Settings) %>
	SetTextLeftMenu('Test Sierologici', 'Test Sierologici<%= If(testSierologiciPaz > 0, "<br/>(" + testSierologiciPaz.ToString() + ")", String.Empty) %>', <%= If(testSierologiciPaz > 0, "1", "2") %>, 0);

	// Ricoveri
	<% Dim ricoveriPaz as Integer = OnVacUtility.CalcolaNumeroRicoveriCovid(Settings) %>
	SetTextLeftMenu('Ricoveri', 'Ricoveri<%= If(ricoveriPaz > 0, "<br/>(" + ricoveriPaz.ToString() + ")", String.Empty) %>', <%= If(ricoveriPaz > 0, "1", "2") %>, 0);
}

// Controllo maschere centrali
if (!<%= Me.IsGestioneCentrale.ToString().ToLower() %>)
{
  window.setTimeout('AggiornaLeftTitles();', 1000);
}

</script>
