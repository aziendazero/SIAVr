Imports Onit.OnAssistnet.OnVac.DAL


Public Class BizElencoBilanci

    Private _GenericProvider As DbGenericProvider

    Public Sub New(ByRef genericprovider As DbGenericProvider)

        _GenericProvider = genericprovider

    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="codCns"></param>
    ''' <param name="strDataInizio"></param>
    ''' <param name="strDataFine"></param>
    ''' <param name="filtroPazAvvisati"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adesimone]	05/02/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function fillDtElencoBilanci(ByRef dt As DataTable, ByVal codCns As String, codAmb As Integer, ByVal strDataInizio As String, ByVal strDataFine As String, ByVal filtroPazAvvisati As OnVac.Enumerators.FiltroAvvisati, ByVal malattia As String, codiceUsl As String) As Boolean

        Return _GenericProvider.ElencoBilanci.fillDtElencoBilanci(dt, codCns, codAmb, strDataInizio, strDataFine, filtroPazAvvisati, malattia, codiceUsl)

    End Function


End Class
