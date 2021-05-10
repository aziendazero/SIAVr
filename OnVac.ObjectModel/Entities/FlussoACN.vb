Imports System.Collections.Generic
Imports System.Runtime.Serialization

Namespace Entities

	<Serializable>
	<DataContract([Namespace]:="http://schemas.datacontract.org/2004/07/OnVac.IntegrazioneVaccinazioni")>
	Public Class FlussoACN
		<DataMember>
		Public Property CampagnaVaccinale As String 'valori: C - campagna vaccinale; NC - nessuna campagna vaccinale
		<DataMember>
		Public Property CodiceAssociazione As String ' codice associazione CVX 
		<DataMember>
		Public Property CodiceAIC As String 'codice AIC del nome commerciale farmaco
		<DataMember>
		Public Property DataEsecuzione As DateTime 'data esecuzione vaccinazione
		<DataMember>
		Public Property DescrizioneAttivita As String ' descrizione attivita, testo libero
		<DataMember>
		Public Property EsecutoreVaccinazione As String ' codice della persona che ha eseguito la somministrazione
		<DataMember>
		Public Property IdMessaggio As String ' Id Messaggio
		<DataMember>
		Public Property Note As String
		<DataMember>
		Public Property NumeroLotto As String
		<DataMember>
		Public Property expirationDateLotto As DateTime 'data scadenza Lotto
		<DataMember>
		Public Property CondizioneDiRischio As String ' Codice della condizione di rischio mappato nella tabella T_ANA_RISCHIO
		<DataMember>
		Public Property TipoPagamento As String ' Codice esterno del tipo di pagamento mappato nella tabella T_ANA_TIPI_PAGAMENTO
		<DataMember>
		Public Property SitoInoculazione As String 'sito inoculazione [HL7 - table 0163]
		<DataMember>
		Public Property TipoOperazione As String ' Tipo Operazione
		<DataMember>
		Public Property ViaSomministrazione As String 'via somministrazione [HL7 - table 0162]
		<DataMember>
		Public Property codicePaziente As String 'codice paziente
		<DataMember>
		Public Property idAcn As String ' Id Acn
		<DataMember>
		Public Property statoCarePlan As String ' stato del CarePlan che serve per sapere se si tratta di una cancellazione Obsoleta Ora serve Stato Immunization
		<DataMember>
		Public Property statoImmunization As String ' stato del vaccino che serve per sapere se si tratta di una cancellazione
		<DataMember>
		Public Property vaccNonEseguita As String ' vaccinazione eseguita
		<DataMember>
		Public Property ServiceRequest As String ' xml del messaggio ricevuto
		<DataMember>
		Public Property listaVaccini As List(Of Vaccino) ' lista codici esterni dei vaccini
		<DataMember>
		Public Property listaEsenzioni As List(Of Esenzione) ' lista codici esterni delle esenzioni
		<DataMember>
		Public Property listaCompletaVaccini As List(Of Vaccini) ' lista dei vaccini
		<DataMember>
		Public Property datiCarePlane As DatiCarePlane
	End Class
	<Serializable>
	<DataContract([Namespace]:="http://schemas.datacontract.org/2004/07/OnVac.IntegrazioneVaccinazioni")>
	Public Class Vaccino
		<DataMember>
		Public Property Codice As String
		<DataMember>
		Public Property Descrizione As String
		<DataMember>
		Public Property idAcn As String
	End Class
	<Serializable>
	<DataContract([Namespace]:="http://schemas.datacontract.org/2004/07/OnVac.IntegrazioneVaccinazioni")>
	Public Class Esenzione
		<DataMember>
		Public Property Codice As String
		<DataMember>
		Public Property Descrizione As String
		<DataMember>
		Public Property idAcn As String
	End Class
	<Serializable>
	<DataContract([Namespace]:="http://schemas.datacontract.org/2004/07/OnVac.IntegrazioneVaccinazioni")>
	Public Class Vaccini
		<DataMember>
		Public Property CampagnaVaccinale As String 'valori: C - campagna vaccinale; NC - nessuna campagna vaccinale	
		<DataMember>
		Public Property CodiceAssociazione As String ' codice associazione CVX 
		<DataMember>
		Public Property CodiceAIC As String 'codice AIC del nome commerciale farmaco
		<DataMember>
		Public Property DataEsecuzione As DateTime 'data esecuzione vaccinazione
		<DataMember>
		Public Property EsecutoreVaccinazione As String ' codice della persona che ha eseguito la somministrazione
		<DataMember>
		Public Property Note As String
		<DataMember>
		Public Property NumeroLotto As String
		<DataMember>
		Public Property expirationDateLotto As DateTime 'data scadenza Lotto
		<DataMember>
		Public Property SitoInoculazione As String 'sito inoculazione [HL7 - table 0163]		
		<DataMember>
		Public Property TipoOperazione As String ' Tipo Operazione
		<DataMember>
		Public Property ViaSomministrazione As String 'via somministrazione [HL7 - table 0162]
		<DataMember>
		Public Property codicePaziente As String 'codice paziente
		<DataMember>
		Public Property idAcn As String ' Id Acn
		<DataMember>
		Public Property statoImmunization As String ' stato del vaccino che serve per sapere se si tratta di una cancellazione
		<DataMember>
		Public Property vaccNonEseguita As String ' vaccinazione eseguita
	End Class
	<Serializable>
	<DataContract([Namespace]:="http://schemas.datacontract.org/2004/07/OnVac.IntegrazioneVaccinazioni")>
	Public Class DatiCarePlane
		<DataMember>
		Public Property CondizioneDiRischio As String ' Codice della condizione di rischio mappato nella tabella T_ANA_RISCHIO
		<DataMember>
		Public Property DescrizioneAttivita As String ' descrizione attivita, testo libero
		<DataMember>
		Public Property statoCarePlan As String ' stato del CarePlan che serve per sapere se si tratta di una cancellazione Obsoleta Ora serve Stato Immunization
		<DataMember>
		Public Property TipoPagamento As String ' Codice esterno del tipo di pagamento mappato nella tabella T_ANA_TIPI_PAGAMENTO
		<DataMember>
		Public Property Ulss As String  ' lettura dell'ulss dalla sezione author/reference/value = "Organization/39"
	End Class

End Namespace
