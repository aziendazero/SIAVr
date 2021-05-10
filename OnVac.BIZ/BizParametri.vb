Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.Collection

Public Class BizParametri
    Inherits BizClass

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)
        MyBase.New(genericprovider, settings, contextInfos, logOptions)
    End Sub

#End Region

#Region " Metodi di Select "

    ''' <summary>
    ''' Restituisce una collection di parametri, contenente tutti i parametri generici
    ''' Se non trova nessun parametro restituisce Nothing
    ''' </summary>
    Public Function GetGenericParameters() As ParametroCollection

        Dim dt As DataTable = Me.GenericProvider.Parametri.SelectGenericParameters()

        Return Me.GetParameterCollection(dt)

    End Function

    ''' <summary>
    ''' Restituisce una collection di parametri, contenente tutti i parametri specifici del consultorio.
    ''' Se non trova nessun parametro restituisce Nothing.
    ''' </summary>
    ''' <param name="codiceConsultorio">Codice del consultorio di cui recuperare i parametri</param>
    Public Function GetParameters(codiceConsultorio As String) As ParametroCollection

        Dim dt As DataTable = Me.GenericProvider.Parametri.SelectCnsParameters(codiceConsultorio)

        Return Me.GetParameterCollection(dt)

    End Function

    ''' <summary>
    ''' Restituisce una collection di parametri, contenente i valori dei parametri specificati per il consultorio.
    ''' Se non trova nessun parametro restituisce Nothing.
    ''' </summary>
    ''' <param name="codiceConsultorio">Codice del consultorio di cui recuperare i parametri</param>
    ''' <param name="elencoParametri">Lista di codici dei parametri da recuperare</param>
    Public Function GetParameters(codiceConsultorio As String, ParamArray elencoParametri() As String) As ParametroCollection

        Dim dt As DataTable = Me.GenericProvider.Parametri.SelectParametersValue(codiceConsultorio, elencoParametri)

        Return Me.GetParameterCollection(dt)

    End Function

    ''' <summary>
    ''' Restituisce un datatable con i parametri generici.
    ''' Se non trova nessun parametro restituisce Nothing.
    ''' </summary>
    Public Function GetDtGenericParameters() As DataTable

        Return Me.GenericProvider.Parametri.SelectGenericParameters()

    End Function

    ''' <summary>
    ''' Restituisce un datatable di parametri, contenente tutti i parametri specifici del consultorio.
    ''' Se non trova nessun parametro restituisce Nothing.
    ''' </summary>
    ''' <param name="codiceConsultorio">Codice del consultorio di cui recuperare i parametri</param>
    Public Function GetDtParameters(codiceConsultorio As String) As DataTable

        Return Me.GenericProvider.Parametri.SelectCnsParameters(codiceConsultorio)

    End Function

    ''' <summary>
    ''' Restituisce una collezione di parametri, creata in base al datatable specificato.
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetParameterCollection(dt As DataTable) As ParametroCollection

        If dt Is Nothing Then Return Nothing

        Dim parametro As Parametro

        Dim collectionParametri As New ParametroCollection()

        For i As Integer = 0 To dt.Rows.Count - 1

            parametro = New Parametro() With
                        {
                            .Codice = dt.Rows(i)("par_codice").ToString(),
                            .Consultorio = dt.Rows(i)("par_cns_codice").ToString(),
                            .Descrizione = dt.Rows(i)("par_descrizione").ToString(),
                            .Valore = dt.Rows(i)("par_valore").ToString(),
                            .Centrale = (dt.Rows(i)("par_centrale").ToString() = "S")
                        }

            collectionParametri.Add(parametro)

        Next

        Return collectionParametri

    End Function

#End Region

#Region " Insert/Update/Delete "

    ''' <summary>
    ''' Inserisce un parametro su db.
    ''' </summary>
    ''' <param name="parametro">Parametro da inserire</param>
    Public Sub InsertParameter(parametro As Parametro)

        Me.GenericProvider.Parametri.InsertParam(parametro)

    End Sub

    ''' <summary>
    ''' Modifica un parametro su db.
    ''' </summary>
    ''' <param name="parametro">Parametro da modificare</param>
    Public Sub UpdateParameter(parametro As Parametro)

        Me.GenericProvider.Parametri.UpdateParam(parametro)

    End Sub

    ''' <summary>
    ''' Cancellazione di un parametro da db.
    ''' </summary>
    ''' <param name="parametro">Parametro da eliminare</param>
    Public Sub DeleteParameter(parametro As Parametro)

        Me.GenericProvider.Parametri.DeleteParam(parametro)

    End Sub

#End Region

End Class
