Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities

Public Class BizCovid19TestSierologici
    Inherits BizClass
#Region " Constructors "
    Public Sub New(genericProvider As DbGenericProvider, settings As Settings.Settings, contextInfo As BizContextInfos)

        MyBase.New(genericProvider, settings, contextInfo, Nothing)

    End Sub
#End Region

    Public Function GetTestSierologiciPaziente(codicePaziente As Long) As List(Of TestSierologicoPaziente)
        Return GenericProvider.Covid19TestSierologici.GetTestSierologiciPaziente(codicePaziente)
    End Function

    <Obsolete("Da non usare più, usare il metodo con il codice del paziente", True)>
    Public Function GetTestSierologiciByCodiceFiscale(codiceFiscale As String) As List(Of TestRapidoScaricoScreening)
        Return GenericProvider.Covid19TestSierologici.GetTestSierologiciByCodiceFiscale(codiceFiscale)
    End Function

    Public Function InsertTest(test As TestRapidoScaricoScreening) As ResultTestSierologici
        Return GenericProvider.Covid19TestSierologici.InsertTest(test)
    End Function

    <Obsolete("Usare il metodo InsertTest per inserire in tutte e due le tabelle", True)>
    Public Function InsertTestSierologico(test As TestRapidoScaricoScreening) As ResultTestSierologici
        Return GenericProvider.Covid19TestSierologici.InsertTestSierologico(test)
    End Function

    <Obsolete("Usare il metodo InsertTest per inserire in tutte e due le tabelle", True)>
    Public Function InsertScaricoScreening(test As TestRapidoScaricoScreening) As ResultSetPost
        Return GenericProvider.Covid19TestSierologici.InsertScaricoScreening(test)
    End Function
End Class
