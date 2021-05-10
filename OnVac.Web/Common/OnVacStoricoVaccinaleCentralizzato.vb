Imports System.Collections.Generic


Namespace Common

    Public Class OnVacStoricoVaccinaleCentralizzato

#Region " Constants "

        Public Const AcquisisciDatiVaccinaliCentraliConfirmKey As String = "ADVC"

#Region " Messaggi "

        ''' <summary>
        ''' Messaggio per avvertire l'utente dell'aggiornamento dell'Anagrafe Vaccinale Centralizzata
        ''' </summary>
        Public Const MessaggioAggiornamentoAnagrafeCentrale As String = "in caso di modifica, si procedera' ad aggiornare l'Anagrafe Vaccinale Centralizzata."

#End Region

#Region " Alert Javascript "

        ''' <summary>
        ''' Alert javascript con messaggio per avvertire l'utente che deve essere recuperato lo Storico Vaccinale
        ''' </summary>
        Public Const AlertMessageRecuperoStoricoVaccinale As String = "alert('ATTENZIONE: e\' presente uno storico in Anagrafe Vaccinale Regionale.\nPer poter effettuare operazioni occorre recuperare i dati.');"

        ''' <summary>
        ''' Alert javascript con messaggio per avvertire l'utente dell'aggiornamento dell'Anagrafe Vaccinale Centralizzata
        ''' </summary>
        Public Const AlertMessageAggiornamentoAnagrafeCentrale As String = "alert('ATTENZIONE: in caso di modifica, si procedera\' ad aggiornare l\'Anagrafe Vaccinale Centralizzata.');"

        ''' <summary>
        ''' Alert javascript con messaggio in caso di tentativo di modifica 
        ''' di una vaccinazione eseguita da una usl diversa da quella corrente.
        ''' </summary>
        Public Const AlertMessageModificaEseguitaUslInserimentoNoUslCorrente As String = "alert('La vaccinazione selezionata non è stata eseguita dall\'azienda corrente: impossibile effettuare la modifica.');"

        ''' <summary>
        ''' Alert javascript con messaggio in caso di tentativo di cancellazione 
        ''' di una vaccinazione eseguita da una usl diversa da quella corrente.
        ''' </summary>
        Public Const AlertMessageCancellazioneEseguitaUslInserimentoNoUslCorrente As String = "alert('La vaccinazione selezionata non è stata eseguita dall\'azienda corrente: impossibile effettuare l\'eliminazione.');"

        ''' <summary>
        ''' Alert javascript con messaggio in caso di tentativo di modifica di una vaccinazione eseguita 
        ''' con reazione avversa registrata da una usl diversa da quella corrente.
        ''' </summary>
        Public Const AlertMessageModificaEseguitaUslReazioneNoUslCorrente As String = "alert('Impossibile modificare la vaccinazione selezionata: è presente una reazione avversa registrata da un\'azienda diversa da quella corrente.');"

        ''' <summary>
        ''' Alert javascript con messaggio in caso di tentativo di modifica di una vaccinazione eseguita 
        ''' con reazione avversa registrata da una usl diversa da quella corrente.
        ''' </summary>
        Public Const AlertMessageCancellazioneEseguitaUslReazioneNoUslCorrente As String = "alert('La reazione avversa associata alla vaccinazione selezionata non è stata registrata dall\'azienda corrente: impossibile effettuare l\'eliminazione.');"

        ''' <summary>
        ''' Alert javascript con messaggio in caso di tentativo di inserimento o modifica 
        ''' di una reazione avversa registrata da una usl diversa da quella corrente.
        ''' </summary>
        Public Const AlertMessageInserimentoModificaReazioneNoUslCorrente As String = "alert('Impossibile inserire o modificare la reazione avversa: è già presente una reazione avversa registrata da un\'azienda diversa da quella corrente!');"

        ''' <summary>
        ''' Alert javascript con messaggio in caso di tentativo di modifica 
        ''' di una reazione avversa registrata da una usl diversa da quella corrente.
        ''' </summary>
        Public Const AlertMessageModificaReazioneNoUslCorrente As String = "alert('La reazione avversa selezionata è stata inserita da un\'azienda diversa da quella corrente: impossibile effettuare la modifica.');"

        ''' <summary>
        ''' Alert javascript con messaggio in caso di tentativo di eliminazione 
        ''' di una reazione avversa registrata da una usl diversa da quella corrente.
        ''' </summary>
        Public Const AlertMessageCancellazioneReazioneNoUslCorrente As String = "alert('La reazione avversa selezionata è stata inserita da un\'azienda diversa da quella corrente: impossibile effettuare l\'eliminazione.');"

        ''' <summary>
        ''' Alert javascript con messaggio in caso di tentativo di modifica/eliminazione
        ''' di una vaccinazione esclusa registrata da una usl diversa da quella corrente.
        ''' </summary>
        Public Const AlertMessageEsclusaNoUslCorrente As String = "alert('La vaccinazione selezionata è stata esclusa da un\'azienda diversa da quella corrente: impossibile effettuare la modifica.');"

        ''' <summary>
        ''' Alert javascript con messaggio in caso di tentativo di modifica
        ''' di una visita registrata da una usl diversa da quella corrente.
        ''' </summary>
        Public Const AlertMessageModificaVisitaNoUslCorrente As String = "alert('La visita selezionata e\' stata inserita da un\'azienda diversa da quella corrente: impossibile effettuare la modifica.');"

        ''' <summary>
        ''' Alert javascript con messaggio in caso di tentativo di eliminazione
        ''' di una visita registrata da una usl diversa da quella corrente.
        ''' </summary>
        Public Const AlertMessageCancellazioneVisitaNoUslCorrente As String = "alert('La visita selezionata e\' stata inserita da un\'azienda diversa da quella corrente: impossibile effettuare l\'eliminazione.');"

#End Region

#End Region

#Region " Constructors "

        Private Settings As OnVac.Settings.Settings

        Public Sub New(settings As OnVac.Settings.Settings)

            Me.Settings = settings

        End Sub

#End Region

#Region " Public "

#Region " Controllo usl inserimento "

        '''' <summary>
        '''' Restituisce true se la vaccinazione è stata eseguita dalla usl corrente.
        '''' </summary>
        '''' <param name="selectedRow"></param>
        '''' <returns></returns>
        '''' <remarks></remarks>
        Public Function CheckEseguitaStessaUsl(selectedRow As DataRow) As Boolean

            Return CheckUslInserimentoCorrente(selectedRow, "ves_usl_inserimento")

        End Function

        '''' <summary>
        '''' Restituisce true se la vaccinazione è stata esclusa dalla usl corrente.
        '''' </summary>
        '''' <param name="selectedRow"></param>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Public Function CheckEsclusaStessaUsl(selectedRow As DataRow) As Boolean

        '    Return CheckUslInserimentoCorrente(selectedRow, "vex_usl_inserimento")

        'End Function

        ''' <summary>
        ''' Restituisce true se la visita è stata inserita dalla usl corrente.
        ''' </summary>
        ''' <param name="selectedRow"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CheckVisitaStessaUsl(selectedRow As DataRow) As Boolean

            Return CheckUslInserimentoCorrente(selectedRow, "vis_usl_inserimento")

        End Function

        '''' <summary>
        '''' Restituisce true se la visita è stata inserita dalla usl corrente.
        '''' </summary>
        '''' <param name="visita"></param>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Public Function CheckVisitaStessaUsl(visita As Entities.Visita) As Boolean

        '    If Not visita Is Nothing Then

        '        Return CheckUslInserimentoCorrente(visita.CodiceUslInserimento)

        '    End If

        '    Return True

        'End Function

        Private Function CheckUslInserimentoCorrente(row As DataRow, nomeCampo As String) As Boolean

            If Not row Is Nothing AndAlso Not row(nomeCampo) Is DBNull.Value Then

                Return CheckUslInserimentoCorrente(row(nomeCampo).ToString())

            End If

            Return True

        End Function

        '''' <summary>
        '''' Restituisce true se la reazione avversa associata all'eseguita non è presente oppure c'è ed è stata registrata dalla usl corrente
        '''' </summary>
        '''' <param name="rowAssociazioneSelezionata"></param>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Public Function CheckReazioneAvversaStessaUsl(rowAssociazioneSelezionata As DataRow) As Boolean

        '    If Not rowAssociazioneSelezionata Is Nothing AndAlso Not rowAssociazioneSelezionata("vra_data_reazione") Is DBNull.Value Then

        '        Return CheckUslInserimentoCorrente(rowAssociazioneSelezionata("vra_usl_inserimento").ToString())

        '    End If

        '    Return True

        'End Function

        '''' <summary>
        '''' Restituisce true se la reazione avversa associata all'eseguita non è presente oppure c'è ed è stata registrata dalla usl corrente
        '''' </summary>
        '''' <param name="vaccinazioneEseguita"></param>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Public Function CheckReazioneAvversaStessaUsl(vaccinazioneEseguita As Entities.VaccinazioneEseguita) As Boolean

        '    If Not vaccinazioneEseguita Is Nothing AndAlso vaccinazioneEseguita.ReazioneAvversa.DataReazione > DateTime.MinValue Then

        '        Return CheckUslInserimentoCorrente(vaccinazioneEseguita.ReazioneAvversa.CodiceUslInserimento)

        '    End If

        '    Return True

        'End Function

        ''' <summary>
        ''' Controlla se la usl specificata corrisponde alla usl corrente
        ''' </summary>
        ''' <param name="codiceUslInserimento"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CheckUslInserimentoCorrente(codiceUslInserimento As String) As Boolean

            If Not String.IsNullOrEmpty(codiceUslInserimento) AndAlso OnVacContext.CodiceUslCorrente <> codiceUslInserimento Then

                Return False

            End If

            Return True

        End Function

#End Region

#Region " Flag visibilità "

        Public Shared Function IsVisibilitaCentraleDatiVaccinaliPaziente(codicePaziente As Integer, ByRef settings As Settings.Settings) As Boolean

            Dim isVisibilitaCentrale As Boolean = False

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizPaziente As New Biz.BizPaziente(genericProvider, settings, OnVacContext.CreateBizContextInfos(), Nothing)
                    isVisibilitaCentrale = bizPaziente.IsVisibilitaCentraleDatiVaccinaliPaziente(codicePaziente)
                End Using
            End Using

            Return isVisibilitaCentrale

        End Function

        Public Shared Function GetValoreVisibilitaDatiVaccinali(dataGridItem As DataGridItem, idCheckFlagVisibilita As String) As String

            Dim chkFlagVisibilita As CheckBox = DirectCast(dataGridItem.FindControl(idCheckFlagVisibilita), CheckBox)

            Return GetValoreVisibilitaDatiVaccinali(chkFlagVisibilita)

        End Function

        ''' <summary>
        ''' Valorizzazione flag visibilità dati vaccinali in base al check del singolo evento ("V" o "N")
        ''' </summary>
        ''' <param name="chkFlagVisibilita"></param>
        ''' <returns></returns>
        Public Shared Function GetValoreVisibilitaDatiVaccinali(chkFlagVisibilita As CheckBox) As String

            If chkFlagVisibilita.Checked Then Return Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente

            Return Constants.ValoriVisibilitaDatiVaccinali.NegatoDaPaziente

        End Function

        ''' <summary>
        ''' Restituisce il valore del flag di visibilità dei dati vaccinali in base allo stato del consenso alla COMUNICAZIONE del paziente,
        ''' recuperandolo in base al codice locale del paziente stesso.
        ''' </summary>
        ''' <param name="settings"></param>
        ''' <param name="codicePaziente"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetValoreVisibilitaDatiVaccinaliDefault(settings As Settings.Settings, codicePaziente As String) As String

            Return GetValoreVisibilitaDatiVaccinaliDefault(settings, codicePaziente, True)

        End Function

        ''' <summary>
        ''' Restituisce il valore del flag di visibilità dei dati vaccinali.
        ''' Se il parametro controllaSoloConsensoComunicazione vale true, prende in considerazione solo lo stato del consenso alla COMUNICAZIONE del paziente,
        ''' recuperandolo in base al codice locale del paziente stesso.
        ''' Altrimenti, controlla lo stato di tutti i consensi (escludendo eventualmente quelli marcati come da non utilizzare per il calcolo dello stato globale).
        ''' </summary>
        ''' <param name="settings"></param>
        ''' <param name="codicePaziente"></param>
        ''' <param name="controllaSoloConsensoComunicazione"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetValoreVisibilitaDatiVaccinaliDefault(settings As Settings.Settings, codicePaziente As String, controllaSoloConsensoComunicazione As Boolean) As String

            Dim valoreVisibilitaDatiVaccinaliDefault As String = String.Empty

            ' Il controllo del flag del consenso per la usl corrente è effettuato dal biz
            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Using bizPaziente As New Biz.BizPaziente(genericProvider, settings, OnVacContext.CreateBizContextInfos(), Nothing)

                    valoreVisibilitaDatiVaccinaliDefault = bizPaziente.GetValoreVisibilitaDatiVaccinaliPaziente(codicePaziente, controllaSoloConsensoComunicazione)

                End Using
            End Using

            Return valoreVisibilitaDatiVaccinaliDefault

        End Function

#End Region

#Region " DataBinder.Eval "

        Private Shared Function GetImgConcesso(page As Page) As String
            Return page.ResolveClientUrl("~/Images/consensoPositivo.png")
        End Function

        Private Shared Function GetImgNegato(page As Page) As String
            Return page.ResolveClientUrl("~/Images/consensoNegativo.png")
        End Function

        Public Shared Function GetImageUrlFlagVisibilita(dataItem As Object, fieldName As String, page As Page) As String

            If Not dataItem Is Nothing AndAlso Not dataItem Is DBNull.Value Then

                Dim flagVisibilita As Object = DataBinder.Eval(dataItem, fieldName)

                If Not flagVisibilita Is Nothing AndAlso
                   Not flagVisibilita Is DBNull.Value Then

                    Return GetImageUrlFlagVisibilita(flagVisibilita.ToString(), page)

                End If

            End If

            Return OnVacStoricoVaccinaleCentralizzato.GetImgNegato(page)

        End Function

        Public Shared Function GetImageUrlFlagVisibilita(flagVisibilita As String, page As Page) As String

            If Not String.IsNullOrEmpty(flagVisibilita) AndAlso
               flagVisibilita = Onit.OnAssistnet.OnVac.Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente Then

                Return OnVacStoricoVaccinaleCentralizzato.GetImgConcesso(page)

            End If

            Return OnVacStoricoVaccinaleCentralizzato.GetImgNegato(page)

        End Function

        Public Shared Function GetToolTipFlagVisibilita(dataItem As Object, fieldName As String) As String

            If Not dataItem Is Nothing AndAlso Not dataItem Is DBNull.Value Then

                Dim flagVisibilita As Object = DataBinder.Eval(dataItem, fieldName)

                If Not flagVisibilita Is Nothing AndAlso
                   Not flagVisibilita Is DBNull.Value Then

                    Return OnVacStoricoVaccinaleCentralizzato.GetToolTipFlagVisibilita(flagVisibilita.ToString())

                End If

            End If

            Return OnVacStoricoVaccinaleCentralizzato.GetToolTipFlagVisibilita(String.Empty)

        End Function

        Public Shared Function GetToolTipFlagVisibilita(flagVisibilita As String) As String

            Dim toolTipMessage As String = "Negato"

            If Not String.IsNullOrEmpty(flagVisibilita) AndAlso
               flagVisibilita = Onit.OnAssistnet.OnVac.Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente Then

                toolTipMessage = "Concesso"

            End If

            Return String.Format("Consenso alla comunicazione dei dati vaccinali da parte del paziente: {0}", toolTipMessage)

        End Function

#End Region

#Region " Acquisizione "

        Public Shared Function GetStatoAcquisizioneDatiVaccinaliCentralePaziente(codicePaziente As Long) As Enumerators.StatoAcquisizioneDatiVaccinaliCentrale?

            Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)
                Return genericProvider.Paziente.GetStatoAcquisizioneDatiVaccinaliCentrale(codicePaziente)
            End Using

        End Function

        Public Class AcquisisciStoricoCommand
            Public CodicePaziente As Long
            Public RichiediConfermaSovrascrittura As Boolean
            Public Settings As Settings.Settings
            Public OnitLayout3 As Onit.Controls.PagesLayout.OnitLayout3
            Public BizLogOptions As Biz.BizLogOptions
            Public Note As String
        End Class

        Public Shared Function AcquisisciDatiVaccinaliCentraliPaziente(command As AcquisisciStoricoCommand) As Biz.BizPaziente.AcquisisciDatiVaccinaliCentraliResult

            Dim acquisisciDatiVaccinaliCentraliResult As Biz.BizPaziente.AcquisisciDatiVaccinaliCentraliResult

            Using transactionScope As New System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, OnVacUtility.GetReadCommittedTransactionOptions())

                Using bizPaziente As New Biz.BizPaziente(command.Settings, Nothing, OnVacContext.CreateBizContextInfos(), command.BizLogOptions)

                    Dim note As String = command.Note
                    If String.IsNullOrWhiteSpace(note) Then note = "Recupero Storico Vaccinale"

                    Dim acquisisciDatiVaccinaliCentraliCommand As New Biz.BizPaziente.AcquisisciDatiVaccinaliCentraliCommand()
                    acquisisciDatiVaccinaliCentraliCommand.CodicePaziente = command.CodicePaziente
                    acquisisciDatiVaccinaliCentraliCommand.CodicePazienteCentrale = bizPaziente.GenericProvider.Paziente.GetCodiceAusiliario(command.CodicePaziente)
                    acquisisciDatiVaccinaliCentraliCommand.CodiceConsultorioPaziente = bizPaziente.GenericProvider.Paziente.GetCodiceConsultorio(command.CodicePaziente)
                    acquisisciDatiVaccinaliCentraliCommand.RichiediConfermaSovrascrittura = command.RichiediConfermaSovrascrittura
                    acquisisciDatiVaccinaliCentraliCommand.Note = note

                    acquisisciDatiVaccinaliCentraliResult = bizPaziente.AcquisisciDatiVaccinaliCentrali(acquisisciDatiVaccinaliCentraliCommand)

                End Using

                transactionScope.Complete()

            End Using

            Dim recuperoStoricoVaccinaleStringBuilder As New System.Text.StringBuilder()

            If Not acquisisciDatiVaccinaliCentraliResult.StatoAcquisizioneDatiVaccinaliCentrale.HasValue Then

                recuperoStoricoVaccinaleStringBuilder.Append("ATTENZIONE\n\nDurante l\'acquisizione sono stati riscontrati i seguenti problemi:\n\n")

                OnVacStoricoVaccinaleCentralizzato.AddAcquisizionePartMessage(recuperoStoricoVaccinaleStringBuilder, "VACCINAZIONI ESCLUSE", acquisisciDatiVaccinaliCentraliResult.VaccinazioniEsclusePresenti)
                OnVacStoricoVaccinaleCentralizzato.AddAcquisizionePartMessage(recuperoStoricoVaccinaleStringBuilder, "PROGRAMMAZIONE", acquisisciDatiVaccinaliCentraliResult.ProgrammazionePresente)

                recuperoStoricoVaccinaleStringBuilder.Append("\nProcedere con l\'acquisizione sovrascrivendo i dati in locale ?")

                command.OnitLayout3.ShowMsgBox(New Onit.Controls.PagesLayout.OnitLayout3.OnitLayoutMsgBox(recuperoStoricoVaccinaleStringBuilder.ToString(),
                                                                                                  OnVacStoricoVaccinaleCentralizzato.AcquisisciDatiVaccinaliCentraliConfirmKey,
                                                                                                  True, True))

            Else

                If acquisisciDatiVaccinaliCentraliResult.StatoAcquisizioneDatiVaccinaliCentrale = Enumerators.StatoAcquisizioneDatiVaccinaliCentrale.Parziale Then

                    recuperoStoricoVaccinaleStringBuilder.AppendFormat("Il recupero dello storico vaccinale e\' stato completato con errori:\n\n{0}", acquisisciDatiVaccinaliCentraliResult.BuildMessage("\n"))

                Else

                    recuperoStoricoVaccinaleStringBuilder.Append("Il recupero dello storico vaccinale e\' stato completato con successo.")

                End If

                command.OnitLayout3.InsertRoutineJS("alert('" + recuperoStoricoVaccinaleStringBuilder.ToString() + "');")

            End If

            Return acquisisciDatiVaccinaliCentraliResult

        End Function

#End Region

#End Region

#Region " Private "

        Private Shared Sub AddAcquisizionePartMessage(ByVal message As System.Text.StringBuilder, ByVal titolo As String, ByVal presenti As Boolean)

            message.AppendFormat("{0}: ", titolo.ToUpper())

            If Not presenti Then
                message.Append("ok")
            Else
                message.Append("già presenti")
            End If

            message.Append("\n")

        End Sub

#End Region

    End Class

End Namespace
