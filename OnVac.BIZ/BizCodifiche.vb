Imports Onit.OnAssistnet.OnVac.DAL


Public Class BizCodifiche
    Inherits Biz.BizClass

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)
        MyBase.New(genericprovider, settings, contextInfos, logOptions)
    End Sub

#End Region

#Region " Public "

    ''' <summary>
    ''' Restituisce una collection di oggetti di tipo codifica, relativa al campo specificato.
    ''' </summary>
    ''' <param name="campo"></param>
    ''' <returns></returns>
    Public Function GetCodifiche(campo As String) As Collection.CodificheCollection
        Return GenericProvider.Codifiche.GetCodifiche(campo)
    End Function


    ''' <summary>
    ''' Restituisce la descrizione associata alla codifica, dati il campo e il codice specificati.
    ''' </summary>
    ''' <param name="campo"></param>
    ''' <param name="codice"></param>
    ''' <returns></returns>
    Public Function GetDescrizioneCodifica(campo As String, codice As String) As String

        Dim codifica As Entities.Codifica = GenericProvider.Codifiche.GetCodifica(campo, codice)

        If codifica Is Nothing Then Return String.Empty

        Return codifica.Descrizione

    End Function

    ''' <summary>
    ''' Restituisce l'oggetto Codifice in base al campo e al codice specificati.
    ''' </summary>
    ''' <param name="campo"></param>
    ''' <param name="codice"></param>
    ''' <returns></returns>
    Public Function GetCodifica(campo As String, codice As String) As Entities.Codifica

        Return GenericProvider.Codifiche.GetCodifica(campo, codice)

    End Function

#End Region

End Class
