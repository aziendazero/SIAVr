Imports System.Collections.Generic

Namespace Entities

    <Serializable>
    Public Class PazienteTrovato
        Public Property CodiceLocale As Integer?
        Public Property CodiceCentrale As String
        Public Property CodiceFiscale As String
        Public Property Cognome As String
        Public Property ComuneNascita As String
        Public Property ComuneResidenza As String
        Public Property DataNascita As Date?
        Public Property IndirizzoResidenza As String
        Public Property Nome As String
        Public Property Sesso As String
        Public Property StatoAnagrafico As String
        Public Property Tessera As String
        Public Property CodiceCentroVaccinale As String
        Public Property Cancellato As Boolean
        Public Property CodiceRegionale As String
        Public Property PazTipo As String
        Public Property Fonte As Enumerators.FonteAnagrafica
        Public Property ID As String
        Public Property VaccinazioniEseguite As Integer?
        Public Property Appuntamenti As Integer?
        Public Property VaccinazioniEscluse As Integer?
        Public Property CodiceUslAssistenza As String
        Public Property CodiceUslDomicilio As String
    End Class

    <Serializable>
    Public Class RicercaPazientiResult
        Public Property ListaPazienti As List(Of PazienteTrovato)
        Public Property MaxRecordRaggiunto As Boolean
        Public Property InterrogatoServizioQPv2 As Boolean
        Public Property PazienteQPv2 As Paziente
    End Class

End Namespace