Imports Onit.OnAssistnet.OnVac.DAL


Public Class BizInstallazioni
    Inherits Biz.BizClass

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos)

        MyBase.New(genericprovider, settings, contextInfos, Nothing)

    End Sub

#End Region

#Region " Public "

    ''' <summary>
    ''' Restituisce i dati di installazione relativi alla ulss corrente
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetInstallazioneCorrente() As Entities.Installazione

        Return GenericProvider.Installazioni.GetInstallazione(ContextInfos.CodiceUsl)

    End Function
    Public Function GetInstallazioneCorrenteBycodiceUsl(codiceUsl As String) As Entities.Installazione

        Return GenericProvider.Installazioni.GetInstallazione(codiceUsl)

    End Function

    ''' <summary>
    ''' Restituisce la descrizione della usl in base al codice
    ''' </summary>
    ''' <param name="codiceUsl"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDescrizioneUsl(codiceUsl As String) As String

        Return GenericProvider.Installazioni.GetDescrizioneUsl(codiceUsl)

    End Function

#End Region

End Class
