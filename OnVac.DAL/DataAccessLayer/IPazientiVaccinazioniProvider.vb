Imports System.Collections.Generic


Public Interface IPazientiVaccinazioniProvider

    Function LoadDescrizioneVaccinazioni(listCodiciVaccinazioni As List(Of String)) As Hashtable

End Interface
