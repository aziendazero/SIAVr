Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.Log.DataLogStructure

' entity di interfaccia Vb/Biz
<Serializable>
Public Class TipoErogatoreWebCommand

    Public Property Id As Integer
    Public Property Codice As String
    Public Property Descrizione As String
    Public Property CodiceAvn As String
    Public Property Ordine As String
    Public Property Obsoleto As Boolean

    Public Property CodiceMaxLength As Integer
    Public Property DescrizioneMaxLength As Integer
    Public Property CodiceAvnMaxLength As Integer
    Public Property OrdineMaxLength As Integer

End Class

Public Class BizErogatori
    Inherits BizClass

#Region " Constructors "

    Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)

        Me.New(genericprovider, settings, contextInfos, Nothing, logOptions)

    End Sub

    Public Sub New(genericprovider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos, bizUslGestitaAllineaSettingsProvider As BizUslGestitaAllineaSettingsProvider, logOptions As BizLogOptions)

        MyBase.New(genericprovider, settings, bizUslGestitaAllineaSettingsProvider, contextInfos, logOptions)

    End Sub

#End Region

#Region " Public "

    Public Function GetTipiErogatori() As List(Of TipoErogatoreVacc)

        Return GenericProvider.TipiErogatori.GetTipiErogatori()

    End Function

    Public Function GetDettaglioTipoErogatore(id As Integer) As TipoErogatoreVacc

        Return GenericProvider.TipiErogatori.GetDettaglioTipoErogatore(id).FirstOrDefault()

    End Function
    Public Function GetDettaglioTipoErogatoreFromCodice(codice As String) As TipoErogatoreVacc

        Return GenericProvider.TipiErogatori.GetDettaglioTipoErogatoreFromCodice(codice).FirstOrDefault()

    End Function

    Public Sub DeleteTipoErogatore(id As Integer)

        GenericProvider.TipiErogatori.DeleteTipoErogatore(id)

    End Sub

    Public Function GetIntFromString(value As String) As Integer?

        Dim conv As Integer? = Nothing

        If Not String.IsNullOrWhiteSpace(value) Then
            conv = Integer.Parse(value)
        End If

        Return conv

    End Function

    Public Function GetStringFromBoolean(value As Boolean) As String

        Dim conv As String

        If value Then
            conv = "S"
        Else
            conv = "N"
        End If

        Return conv

    End Function

    Public Function InsertTipoErogatore(aspCommand As TipoErogatoreWebCommand) As BizGenericResult

        Dim result As New BizGenericResult
        result = ControlloCampiTipoErogatore(aspCommand)

        If result.Success Then

            Dim bizCommand As New TipoErogatoreVaccCommand With {
             .Codice = aspCommand.Codice,
             .Descrizione = aspCommand.Descrizione,
             .CodiceAvn = aspCommand.CodiceAvn,
             .Ordine = GetIntFromString(aspCommand.Ordine),
             .Obsoleto = GetStringFromBoolean(aspCommand.Obsoleto),
             .CodiceMaxLength = aspCommand.CodiceMaxLength,
             .DescrizioneMaxLength = aspCommand.DescrizioneMaxLength,
             .CodiceAvnMaxLength = aspCommand.CodiceAvnMaxLength,
             .OrdineMaxLength = aspCommand.OrdineMaxLength
            }

            GenericProvider.TipiErogatori.InsertTipoErogatore(bizCommand)

        End If

        Return result

    End Function

    Public Function UpdateTipoErogatore(aspCommand As TipoErogatoreWebCommand) As BizGenericResult

        Dim result As New BizGenericResult
        result = ControlloCampiTipoErogatore(aspCommand)

        If result.Success Then

            Dim bizCommand As New TipoErogatoreVaccCommand With {
            .Id = aspCommand.Id, .Codice = aspCommand.Codice,
            .Descrizione = aspCommand.Descrizione, .CodiceAvn = aspCommand.CodiceAvn,
            .Ordine = GetIntFromString(aspCommand.Ordine),
            .Obsoleto = GetStringFromBoolean(aspCommand.Obsoleto),
            .CodiceMaxLength = aspCommand.CodiceMaxLength,
            .DescrizioneMaxLength = aspCommand.DescrizioneMaxLength,
            .CodiceAvnMaxLength = aspCommand.CodiceAvnMaxLength,
            .OrdineMaxLength = aspCommand.OrdineMaxLength
            }

            GenericProvider.TipiErogatori.UpdateTipoErogatore(bizCommand)

        End If

        Return result

    End Function

    Public Function ControlloCampiTipoErogatore(aspCommand As TipoErogatoreWebCommand) As BizGenericResult

        aspCommand.Codice = aspCommand.Codice.Trim().ToUpper()
        aspCommand.Descrizione = aspCommand.Descrizione.Trim().ToUpper()
        aspCommand.CodiceAvn = aspCommand.CodiceAvn.Trim().ToUpper()
        aspCommand.Ordine = aspCommand.Ordine.Trim().ToUpper()

        Dim result As New BizGenericResult()

        result.Success = True
        result.Message = String.Empty

        ' controllo lunghezza campi inserimento
        If aspCommand.Codice.Length > aspCommand.CodiceMaxLength Then
            result.Success = False
            result.Message += "il campo ""Codice"" è troppo lungo, "
        End If

        If String.IsNullOrWhiteSpace(aspCommand.Codice) Then
            result.Success = False
            result.Message += "il campo ""Codice"" è vuoto, "
        End If

        If aspCommand.Descrizione.Length > aspCommand.DescrizioneMaxLength Then
            result.Success = False
            result.Message += "il campo ""Descrizione"" è troppo lungo, "
        End If

        If String.IsNullOrWhiteSpace(aspCommand.Descrizione) Then
            result.Success = False
            result.Message += "il campo ""Descrizione"" è vuoto, "
        End If

        If aspCommand.CodiceAvn.Length > aspCommand.CodiceAvnMaxLength Then
            result.Success = False
            result.Message += "il campo ""Codice AVN"" è troppo lungo, "
        End If

        Dim tmpOrdine As Integer
        If Not String.IsNullOrWhiteSpace(aspCommand.Ordine) AndAlso Not Integer.TryParse(aspCommand.Ordine, tmpOrdine) Then
            result.Success = False
            result.Message += "il campo ""Ordine"" deve contenere solo numeri, "
        End If

        If aspCommand.Ordine.ToString().Length > aspCommand.OrdineMaxLength Then
            result.Success = False
            result.Message += "il campo ""Ordine"" è troppo lungo, "
        End If

        If Not result.Success Then
            result.Message = result.Message.Remove(result.Message.Length - 2, 2)
            result.Message = String.Concat("Attenzione, impossibile completare l'operazione: ", result.Message, "!")
        End If

        Return result

    End Function

    Public Function GetTipiErogatoriFromLuogoEsecuzione(codiceLuogoEsecuzione As String) As List(Of TipoErogatoreVacc)
        Return GenericProvider.TipiErogatori.GetTipiErogatoriFromLuogoEsecuzione(codiceLuogoEsecuzione)
    End Function

    Public Function InsertLinkTipiErogatoreLuogo(idTipiErogatore As List(Of Integer), codiceLuogo As String) As Integer
        If Not (idTipiErogatore Is Nothing OrElse idTipiErogatore.Count = 0 OrElse String.IsNullOrWhiteSpace(codiceLuogo)) Then
            Return GenericProvider.TipiErogatori.InsertLinkTipiErogatoreLuogo(idTipiErogatore, codiceLuogo)
        End If
    End Function

    Function DeleteTipoErogatoreFromLuogo(codiceLuogo As String) As Integer
        Return GenericProvider.TipiErogatori.DeleteTipoErogatoreFromLuogo(codiceLuogo)
    End Function

#End Region

End Class