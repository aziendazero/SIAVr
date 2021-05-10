Imports System.Collections.Generic

Public Interface ICovid19Viaggiatori
    Function GetViaggiatori() As List(Of Viaggiatore)
    Function SalvaElaborazioneViaggiatore(statoElab As Integer, dataElab As Date, noteElaborazione As String, codiceFiscale As String, dataRientro As Date, data As Date) As Object
    Function GetViaggiatoriExtraEU() As List(Of Viaggiatore)
    Function SalvaElaborazioneViaggiatoreExtraEU(statoElab As Integer, dataElab As Date, noteElaborazione As String, codiceFiscale As String, dataRientro As Date, data As Date) As Object
End Interface
