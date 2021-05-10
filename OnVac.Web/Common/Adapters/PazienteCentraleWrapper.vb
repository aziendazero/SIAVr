Imports System.Configuration
Imports System.Collections.Generic
Imports System.Collections.ObjectModel

Imports Onit.Database.DataAccessManager
Imports Onit.Controls.OnitDataPanel
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Entities
Imports Onit.OnAssistnet.OnVac.Filters
Imports Onit.OnAssistnet.OnVac.Collection
Imports Onit.OnAssistnet.OnVac.Log
Imports Onit.OnAssistnet.OnVac.Log.DataLogManager
Imports Onit.OnAssistnet.OnVac.Common.Utility.Extensions


''' -----------------------------------------------------------------------------
''' Project	 : Paziente_HL7
''' Class	 : QueryPaziente
''' 
''' -----------------------------------------------------------------------------
''' <summary> Questa classe racchiude tutte le procedure per la ricerca e la
''' modifica dei pazienti sul database </summary>
''' <remarks>
''' </remarks>
''' <history>
''' 	[adesimone]	10/06/2008	Created
''' </history>
''' -----------------------------------------------------------------------------
Public Class PazienteCentraleWrapper

    Private _valorized As Integer = 0

    ''' <summary>
    ''' Salvataggio in anagrafe centrale tramite query o servizio xmpi
    ''' </summary>
    ''' <param name="drRow"></param>
    ''' <param name="dtEncoder"></param>
    ''' <param name="maintable"></param>
    ''' <param name="connectionName"></param>
    ''' <param name="strEnte"></param>
    ''' <param name="strAddetto"></param>
    ''' <param name="strErrMessage"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SalvaAnagrafica(drRow As System.Data.DataRow, dtEncoder As Onit.Controls.OnitDataPanel.FieldsEncoder,
                                    maintable As Onit.Controls.OnitDataPanel.MainTableInfo, connectionName As String,
                                    strEnte As String, strAddetto As String, ByRef strErrMessage As String) As OnitDataPanelError

        Dim odpError As New OnitDataPanelError(True, "OnitServicePaziente.SaveRow()", "Salvataggio eseguito con successo.")

        'riempimento della struttura paziente
        Dim paz As New PazienteCentrale()
        paz.codice = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_CODICE")

        _valorized = 0

        ' residenza
        paz.ResidenzaIndirizzo = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_INDIRIZZO_RESIDENZA")
        paz.ResidenzaComCodice = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_COM_CODICE_RESIDENZA")
        paz.ResidenzaCap = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_CAP_RESIDENZA")
        paz.ResidenzaLocalita = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_LOCALITA_RESIDENZA")
        paz.ResidenzaDataInizio = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_DATA_INIZIO_RESIDENZA")
        paz.ResidenzaDataFine = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_DATA_FINE_RESIDENZA")
        'domicilio
        paz.DomicilioIndirizzo = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_INDIRIZZO_DOMICILIO")
        paz.DomicilioComCodice = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_COM_CODICE_DOMICILIO")
        paz.DomicilioCap = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_CAP_DOMICILIO")
        paz.DomicilioLocalita = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_LOCALITA_DOMICILIO")
        paz.DomicilioDataInizio = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_DATA_INIZIO_DOMICILIO")
        paz.DomicilioDataFine = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_DATA_FINE_DOMICILIO")
        'emigrazione
        paz.EmigrazioneData = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_DATA_EMIGRAZIONE")
        paz.EmigrazioneComCodice = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_COM_CODICE_EMIGRAZIONE")
        'immigrazione
        paz.ImmigrazioneComCodice = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_COM_CODICE_IMMIGRAZIONE")
        paz.ImmigrazioneData = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_DATA_IMMIGRAZIONE")

        paz.ResidenzaUslCodice = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_USL_CODICE_RESIDENZA")
        paz.UslProvenienza = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_USL_PROVENIENZA")
        paz.UslAssistenzaCodice = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_USL_CODICE_ASSISTENZA")
        paz.UslAssistenzaDataInizio = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_DATA_INIZIO_ASS")
        paz.UslAssistenzaDataCessazione = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_DATA_CESSAZIONE_ASS")
        paz.UslAssistenzaMotivoCessazione = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_MOTIVO_CESSAZIONE_ASS")

        paz.Nome = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_NOME")
        paz.Sesso = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_SESSO")
        paz.Tessera = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_TESSERA")
        paz.CEE = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_CEE")
        paz.CitCodice = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_CIT_CODICE")
        paz.CodiceDemografico = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_CODICE_DEMOGRAFICO")
        paz.codiceFiscale = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_CODICE_FISCALE")
        paz.CodiceProfessione = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_CODICE_PROFESSIONE")
        paz.Cognome = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_COGNOME")
        paz.DataDecesso = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_DATA_DECESSO")
        paz.ComCodiceDecesso = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_COM_CODICE_DECESSO")
        paz.DataNascita = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_DATA_NASCITA")
        paz.ComCodiceNascita = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_COM_CODICE_NASCITA")
        paz.DisCodice = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_DIS_CODICE")
        paz.MedCodiceBase = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_MED_CODICE_BASE")
        paz.StatoCivile = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_STATO_CIVILE")
        paz.Telefono1 = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_TELEFONO_1")
        paz.Telefono2 = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_TELEFONO_2")
        paz.Telefono3 = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_TELEFONO_3")
        paz.Note = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_NOTE")
        paz.CategoriaRischioCodice = GetValueFromRow(drRow, dtEncoder, connectionName, "PAZ_RSC_CODICE")

        ' Riporto il numero variazioni
        paz.NumeroVariazioni = _valorized

        Using Biz As New Biz.BizPazienteCentrale(OnVacContext.CreateBizContextInfos(), Nothing)

            Dim result As Biz.BizPazienteCentrale.SalvaAnagraficaResult = Nothing

            Select Case drRow.RowState
                Case DataRowState.Added

                    paz.TipoVariazione = PazienteCentrale.TipoVariazioneEnum.Added
                    result = Biz.SalvaAnagrafica(paz, strEnte, strAddetto)

                    'aggiornamento del codice paziente
                    Dim strColCode As String
                    strColCode = dtEncoder.getCode(maintable.Connection, "t_paz_pazienti_centrale", "paz_codice")
                    If strColCode <> "" Then
                        drRow(strColCode) = result.PazCodice  'id paziente calcolato
                    End If

                Case DataRowState.Modified

                    paz.TipoVariazione = PazienteCentrale.TipoVariazioneEnum.Modified
                    result = Biz.SalvaAnagrafica(paz, strEnte, strAddetto)

                Case DataRowState.Deleted

                    result = Biz.SalvaAnagrafica(paz, strEnte, strAddetto)

                Case Else

            End Select

            If result.HasError Then
                If String.IsNullOrEmpty(strErrMessage) Then strErrMessage &= " "
                strErrMessage &= result.Message
                odpError.generateError(strErrMessage)
            Else
                odpError.comment = result.Message
            End If

        End Using

        'output dell'errore sul parametro
        Return odpError

    End Function

    Private Function GetValueFromRow(dr As DataRow, enc As FieldsEncoder, connectionName As String, fieldName As String) As Object

        Dim out, original As Object

        If dr.RowState <> DataRowState.Deleted Then

            out = enc.getValOf(dr, connectionName, fieldName, "t_paz_pazienti_centrale", True)

            If dr.RowState = DataRowState.Added Then

                If IsDBNull(out) Then
                    out = Nothing
                Else
                    out = out.ToString()
                    _valorized += 1
                End If

            Else ' -- stato modified

                original = enc.getValOf(dr, connectionName, fieldName, "t_paz_pazienti_centrale", False)

                'nothing se il valore è nullo o non modificato fatta eccezione per la chiave
                Dim blnModificato As Boolean = (Not out.Equals(original) Or String.Compare(fieldName, "paz_codice", True) = 0)

                If blnModificato Then
                    out = out.ToString()
                    _valorized += 1
                Else
                    out = Nothing
                End If

            End If

        Else
            out = Nothing
        End If

        Return out

    End Function

End Class
