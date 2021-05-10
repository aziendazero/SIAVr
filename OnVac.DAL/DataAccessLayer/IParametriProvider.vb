Imports System.Collections.Generic

Public Interface IParametriProvider

    Sub InsertParam(param As OnVac.Entities.Parametro)

    Sub UpdateParam(param As OnVac.Entities.Parametro)

    Sub DeleteParam(param As OnVac.Entities.Parametro)

    Function SelectCnsParameters(consultorio As String) As DataTable

    Function SelectParametersValue(consultorio As String, ParamArray cod_param_list() As String) As DataTable

    Function SelectParametersValue(ParamArray cod_param_list() As String) As DataTable

    Function SelectGenericParameters() As DataTable

    Function GetParametriSistema() As List(Of KeyValuePair(Of String, Object))

    Function GetParametriCns(codiceConsultorio As String) As List(Of KeyValuePair(Of String, Object))
    Function GetParametriCns(codiceConsultorio As String, listaCodiciParametri As List(Of String)) As List(Of KeyValuePair(Of String, Object))

End Interface
