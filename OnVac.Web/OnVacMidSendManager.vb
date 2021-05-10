Imports System.Collections.Generic
Imports Onit.OnAssistnet.MID
Imports Onit.OnAssistnet.MID.Models
Imports Onit.OnAssistnet.OnVac.MID.Factories

Public Class OnVacMidSendManager

#Region " Inserisci Paziente "

    Shared Sub InserisciPaziente(paziente As Entities.Paziente)
        InserisciPaziente(paziente, OperazionePaziente.Iscrizione)
    End Sub

    Shared Sub InserisciPaziente(paziente As Entities.Paziente, operazionePaziente As OperazionePaziente)

        Dim allineaPazienteSendModel As IAllineaPazienteSendModel(Of Entities.Paziente, DataTable) =
            AllineaPazienteModelFactory.Instance().CreateInserisciPazienteSendModel(OnVacContext.AppId, OnVacContext.Azienda, operazionePaziente)

        If Not allineaPazienteSendModel Is Nothing Then

            Dim allineaPazienteInfo As New AllineaPazienteInfo()
            allineaPazienteInfo.Operazione = operazionePaziente
            allineaPazienteInfo.UserID = OnVacContext.UserId
            allineaPazienteInfo.EventDate = DateTime.Now

            allineaPazienteSendModel.InserisciPaziente(paziente, allineaPazienteInfo)

        End If

    End Sub

#End Region

#Region " Modifica Paziente "

    Shared Sub ModificaPaziente(paziente As Entities.Paziente, pazientePrecedente As Entities.Paziente)
        ModificaPaziente(paziente, pazientePrecedente, Nothing, OperazionePaziente.Variazione)
    End Sub

    Shared Sub ModificaPaziente(paziente As Entities.Paziente, vaccinazioniEseguite As DataTable)
        ModificaPaziente(paziente, Nothing, vaccinazioniEseguite, OperazionePaziente.VariazioneVaccinale)
    End Sub

    Shared Sub ModificaPaziente(paziente As Entities.Paziente, pazientePrecedente As Entities.Paziente, vaccinazioniEseguite As DataTable, operazionePaziente As OperazionePaziente)

        Dim allineaPazienteSendModel As IAllineaPazienteSendModel(Of Entities.Paziente, DataTable) =
            AllineaPazienteModelFactory.Instance.CreateModificaPazienteSendModel(OnVacContext.AppId, OnVacContext.Azienda, operazionePaziente)

        If Not allineaPazienteSendModel Is Nothing Then

            Dim allineaPazienteInfo As New AllineaPazienteInfo()
            allineaPazienteInfo.Operazione = operazionePaziente
            allineaPazienteInfo.UserID = OnVacContext.UserId
            allineaPazienteInfo.EventDate = DateTime.Now

            allineaPazienteSendModel.ModificaPaziente(paziente, pazientePrecedente, vaccinazioniEseguite, allineaPazienteInfo)

        End If

    End Sub

#End Region

#Region " Unisci Pazienti "

    Shared Sub UnisciPazienti(pazienteMaster As Entities.Paziente, pazienteMasterPrecedente As Entities.Paziente, pazientiAlias As IEnumerable(Of Entities.Paziente))
        UnisciPazienti(pazienteMaster, pazienteMasterPrecedente, pazientiAlias, OperazionePaziente.Unificazione)
    End Sub

    Shared Sub UnisciPazienti(pazienteMaster As Entities.Paziente, pazienteMasterPrecedente As Entities.Paziente, pazientiAlias As IEnumerable(Of Entities.Paziente), operazionePaziente As OperazionePaziente)

        Dim allineaPazienteSendModel As IAllineaPazienteSendModel(Of Entities.Paziente, DataTable) =
            AllineaPazienteModelFactory.Instance.CreateUnisciPazientiSendModel(OnVacContext.AppId, OnVacContext.Azienda, operazionePaziente)

        If Not allineaPazienteSendModel Is Nothing Then

            Dim allineaPazienteInfo As New AllineaPazienteInfo()
            allineaPazienteInfo.Operazione = operazionePaziente
            allineaPazienteInfo.UserID = OnVacContext.UserId
            allineaPazienteInfo.EventDate = DateTime.Now

            allineaPazienteSendModel.UnisciPazienti(pazienteMaster, pazienteMasterPrecedente, pazientiAlias, allineaPazienteInfo)

        End If

    End Sub

#End Region

#Region " Disunisci Pazienti "

    Shared Sub DisunisciPazienti(pazienteMaster As Entities.Paziente, pazienteMasterPrecedente As Entities.Paziente, pazienteAlias As Entities.Paziente, pazienteAliasPrecedente As Entities.Paziente, settings As Settings.Settings)
        DisunisciPazienti(pazienteMaster, pazienteMasterPrecedente, pazienteAlias, pazienteAliasPrecedente, OperazionePaziente.AnnullamentoUnificazione, settings)
    End Sub

    Shared Sub DisunisciPazienti(pazienteMaster As Entities.Paziente, pazienteMasterPrecedente As Entities.Paziente, pazienteAlias As Entities.Paziente, pazienteAliasPrecedente As Entities.Paziente, ByVal operazionePaziente As OperazionePaziente, ByVal settings As Settings.Settings)

        Dim allineaPazienteSendModel As IAllineaPazienteSendModel(Of Entities.Paziente, DataTable) =
            AllineaPazienteModelFactory.Instance.CreateUnisciPazientiSendModel(OnVacContext.AppId, OnVacContext.Azienda, operazionePaziente)

        If Not allineaPazienteSendModel Is Nothing Then

            Dim allineaPazienteInfo As New AllineaPazienteInfo()
            allineaPazienteInfo.Operazione = operazionePaziente
            allineaPazienteInfo.UserID = OnVacContext.UserId
            allineaPazienteInfo.EventDate = DateTime.Now

            allineaPazienteSendModel.DisunisciPazienti(pazienteMaster, pazienteMasterPrecedente, pazienteAlias, pazienteAliasPrecedente, allineaPazienteInfo)

        End If

    End Sub

#End Region

End Class
