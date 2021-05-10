Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities

Public Class BizLuoghiEsecuzioneVaccinazioni
    Inherits BizClass

#Region " Constructors "

    Public Sub New(genericProvider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos)

        MyBase.New(genericProvider, settings, contextInfos, Nothing)

    End Sub

#End Region

    Public Class CampiObbligatori
        Public Const CondizioneSanitaria As String = "CONDSAN"
        Public Const CategoriaRischio As String = "CATRIS"
        Public Const CentroVaccinale As String = "CENVAC"
        Public Const NomeCommerciale As String = "NOMECOMM"
        Public Const CodiceLotto As String = "CODLOTTO"
        Public Const DataScadenzaLotto As String = "DATASCADLOTTO"
        Public Const ViaSomministrazione As String = "VIASOMMIN"
        Public Const SitoInoculazione As String = "SITOINOC"
        Public Const MedicoResponsabile As String = "MEDRESP"
        Public Const Vaccinatore As String = "VACCIN"
        Public Const CodiceStruttura As String = "CODSTRU"
        Public Const Luogo As String = "LUOGO"
        Public Const Asl As String = "ASL"
        Public Const TipoErogatore As String = "TIPOEROG"
    End Class

    Public Function GetLuoghiEsecuzioneVaccinazioni(escludiObsoleti As Boolean) As List(Of Entities.LuoghiEsecuzioneVaccinazioni)
        Dim result As New List(Of LuoghiEsecuzioneVaccinazioni)
        result = GenericProvider.LuoghiEsecuzioneVaccinazioni.GetLuoghiEsecuzioneVaccinazioni()
        If Not result Is Nothing AndAlso escludiObsoleti Then
            result = result.Where(Function(f) f.Obsoleto = "N").ToList()
        End If

        Return result

    End Function

    Public Function GetLuoghiEsecuzioneVaccinazioni() As List(Of Entities.LuoghiEsecuzioneVaccinazioni)

        Return GenericProvider.LuoghiEsecuzioneVaccinazioni.GetLuoghiEsecuzioneVaccinazioni()

    End Function

    ''' <summary>
    ''' Restituisce il luogo dove viene eseguita la vaccianzione in base all'id specificato
    ''' </summary>
    ''' <param name="id"></param>
    ''' <returns></returns>
    Public Function GetLuogoEsecuzioneVaccinazione(id As String) As Entities.LuoghiEsecuzioneVaccinazioni

        Return GenericProvider.LuoghiEsecuzioneVaccinazioni.GetLuogoEsecuzioneVaccinazione(id)

    End Function

    Public Function GetCampiObbligatori() As List(Of Entities.CampoObbligatorio)

        Return New List(Of Entities.CampoObbligatorio) From {New Entities.CampoObbligatorio(CampiObbligatori.CondizioneSanitaria, "CONDIZIONE SANITARIA"),
                                                              New Entities.CampoObbligatorio(CampiObbligatori.CategoriaRischio, "CATEGORIA RISCHIO"),
                                                              New Entities.CampoObbligatorio(CampiObbligatori.CentroVaccinale, "CENTRO VACCINALE"),
                                                              New Entities.CampoObbligatorio(CampiObbligatori.NomeCommerciale, "NOME COMMERCIALE"),
                                                              New Entities.CampoObbligatorio(CampiObbligatori.CodiceLotto, "CODICE LOTTO"),
                                                              New Entities.CampoObbligatorio(CampiObbligatori.DataScadenzaLotto, "DATA SCADENZA LOTTO"),
                                                              New Entities.CampoObbligatorio(CampiObbligatori.ViaSomministrazione, "VIA SOMMINISTRAZIONE"),
                                                              New Entities.CampoObbligatorio(CampiObbligatori.SitoInoculazione, "SITO INOCULAZIONE"),
                                                              New Entities.CampoObbligatorio(CampiObbligatori.MedicoResponsabile, "MEDICO RESPONSABILE"),
                                                              New Entities.CampoObbligatorio(CampiObbligatori.Vaccinatore, "VACCINATORE"),
                                                              New Entities.CampoObbligatorio(CampiObbligatori.CodiceStruttura, "CODICE STRUTTURA"),
                                                              New Entities.CampoObbligatorio(CampiObbligatori.Luogo, "LUOGO"),
                                                              New Entities.CampoObbligatorio(CampiObbligatori.Asl, "ASL"),
                                                              New Entities.CampoObbligatorio(CampiObbligatori.TipoErogatore, "TIPO EROGATORE")
        }

    End Function

    Public Function InsertLuoghiEsecuzioneVaccinazioni(command As Entities.LuoghiEsecuzioneVaccCommand) As BizGenericResult
        Dim result As New BizGenericResult()

        'Controlli
        If String.IsNullOrWhiteSpace(command.Codice) Then
            result.Success = False
            result.Message = "Il codice è obbligatorio"
            Return result
        End If

        If String.IsNullOrWhiteSpace(command.Descrizione) Then
            result.Success = False
            result.Message = "La descrizione è obbligatoria"
            Return result
        End If

        command.Codice = command.Codice.Trim().ToUpper()
        command.Descrizione = command.Descrizione.Trim().ToUpper()
        command.Tipo = command.Tipo.Trim().ToUpper()

        result.Success = True
        result.Message = String.Empty

        'Esistenza
        Dim itemExists As Entities.LuoghiEsecuzioneVaccinazioni = GenericProvider.LuoghiEsecuzioneVaccinazioni.GetLuogoEsecuzioneVaccinazione(command.Codice)
        If itemExists IsNot Nothing Then
            result.Success = False
            result.Message = "Codice già esistente"
            Return result
        End If

        ' controllo lunghezza campi inserimento
        If command.Codice.Length > command.CodiceMaxLength Then
            result.Success = False
            result.Message = "Impossibile completare l'operazione. Il campo codice è troppo lungo!"

        ElseIf command.Descrizione.Length > command.DescrizioneMaxLength Then
            result.Success = False
            result.Message = "Impossibile completare l'operazione. Il campo descrizione è troppo lungo!"

        ElseIf command.Ordine.ToString().Length > command.OrdineMaxLength Then
            result.Success = False
            result.Message = "Impossibile completare l'operazione. Il campo ordine è troppo lungo!"

        ElseIf String.IsNullOrWhiteSpace(command.Codice) And String.IsNullOrWhiteSpace(command.Descrizione) Then
            result.Success = False
            result.Message = "Impossibile completare l'operazione. I campi codice e descrizione sono vuoti!"

        ElseIf String.IsNullOrWhiteSpace(command.Descrizione) Then
            result.Success = False
            result.Message = "Impossibile completare l'operazione. Il campo descrizione è vuoto!"

        ElseIf String.IsNullOrWhiteSpace(command.Codice) Then
            result.Success = False
            result.Message = "Impossibile completare l'operazione. Il campo codice è vuoto!"

        ElseIf itemExists Is Nothing And Not String.IsNullOrWhiteSpace(command.Descrizione) Then
            GenericProvider.LuoghiEsecuzioneVaccinazioni.InsertLuoghiEsecuzioneVaccinazioni(command)

        End If

        Return result

    End Function

    Public Function UpdateLuoghiEsecuzioneVaccinazioni(command As Entities.LuoghiEsecuzioneVaccCommand) As BizGenericResult

        Dim result As New BizGenericResult()
        'Controlli
        If String.IsNullOrWhiteSpace(command.Codice) Then
            result.Success = False
            result.Message = "Il codice è obbligatorio"
            Return result
        End If
        If String.IsNullOrWhiteSpace(command.Descrizione) Then
            result.Success = False
            result.Message = "La descrizione è obbligatoria"
            Return result
        End If
        GenericProvider.LuoghiEsecuzioneVaccinazioni.UpdateLuoghiEsecuzioneVaccinazioni(command)
        result.Success = True
        result.Message = String.Empty
        Return result

    End Function

    Public Function DeleteLuoghiEsecuzioneVaccinazioni(Codice As String) As BizGenericResult

        Dim result As New BizGenericResult()

        GenericProvider.LuoghiEsecuzioneVaccinazioni.DeleteLuoghiEsecuzioneVaccinazioni(Codice)

        result.Success = True
        result.Message = String.Empty

        Return result

    End Function

#Region " Campi Obbligatori "

    Public Function IsCampoInLuogoCampiObbligatori(codLuogo As String, codCampo As String) As Boolean
        Return GenericProvider.LuoghiEsecuzioneVaccinazioni.IsCampoInLuogoCampiObbligatori(codLuogo, codCampo)
    End Function

    Public Function GetDettagliCampoObbligatorioLuogo(codLuogo As String, codCampo As String) As CampoObbligLuogoVacc
        Return GenericProvider.LuoghiEsecuzioneVaccinazioni.GetDettagliCampoObbligatorioLuogo(codLuogo, codCampo)
    End Function

    Public Function GetCampiObbligatoriByLuogo(codLuogo As String, dataValidita As DateTime?) As List(Of CampoObbligLuogoVacc)
        Dim result As List(Of CampoObbligLuogoVacc) = GenericProvider.LuoghiEsecuzioneVaccinazioni.GetCampiObbligatoriByLuogo(codLuogo)
        If dataValidita.HasValue Then
            result = result.Where(Function(f) f.DataInizioObblig < dataValidita OrElse Not f.DataInizioObblig.HasValue).ToList()
        End If
        Return result
    End Function

    Public Function GetCampiObbligatori(filterDataValidita As Boolean) As List(Of CampoObbligLuogoVacc)
        If filterDataValidita Then
            Return GenericProvider.LuoghiEsecuzioneVaccinazioni.GetCampiObbligatori().Where(Function(f) f.DataInizioObblig < Date.Now OrElse Not f.DataInizioObblig.HasValue).ToList()
        Else
            Return GenericProvider.LuoghiEsecuzioneVaccinazioni.GetCampiObbligatori()
        End If

    End Function


    Public Sub InsertCampiObbligatoriLuogo(campiObbligatori As List(Of CampoObbligLuogoVacc))

        For Each campo As CampoObbligLuogoVacc In campiObbligatori
            campo.IdUteInserimento = ContextInfos.IDUtente
            campo.DataInserimento = DateTime.Now
            GenericProvider.LuoghiEsecuzioneVaccinazioni.InsertCampoObbligatorioLuogo(campo)
        Next

    End Sub

    Public Function DeleteCampiObbligatoriLuogo(codLuogo As String) As BizGenericResult

        Dim result As New BizGenericResult()
        GenericProvider.LuoghiEsecuzioneVaccinazioni.DeleteCampiObbligatoriLuogo(codLuogo)

        result.Success = True
        result.Message = String.Empty

        Return result

    End Function

#End Region

End Class
