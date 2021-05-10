Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Log


Public Class BizConfigurazioneCertificato
    Inherits Biz.BizClass

#Region " Constructors "

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="genericprovider"></param>
    ''' <param name="settings"></param>
    ''' <param name="contextInfos"></param>
    ''' <param name="logOptions"></param>
    Public Sub New(ByRef genericprovider As DbGenericProvider, ByRef settings As Onit.OnAssistnet.OnVac.Settings.Settings, ByVal contextInfos As BizContextInfos, ByVal logOptions As BizLogOptions)

        MyBase.New(genericprovider, settings, contextInfos, logOptions)

    End Sub



#End Region

    ''' <summary>
    ''' Get delle configurazioni
    ''' </summary>
    ''' <param name="id"></param>
    ''' <returns></returns>
    Public Function GetConfigurazioni(id As Integer) As System.Collections.Generic.IEnumerable(Of Entities.ConfigurazioneCertificazione)

        Dim listConfigurazioni As List(Of Entities.ConfigurazioneCertificazione) = GenericProvider.ConfigurazioneCertificato.GetConfigurazioneCertificato(id)

        For Each conf As Entities.ConfigurazioneCertificazione In listConfigurazioni

            Dim list As List(Of Entities.ConfigurazioneCertificazioneVaccinazioni) = GenericProvider.ConfigurazioneCertificato.GetListVacciniConfigurazione(conf.Id)

            Dim codici As New Text.StringBuilder()
            Dim descrizioni As New Text.StringBuilder()

            Dim listaFiltri As New List(Of Entities.Filtro)()

            For Each vac As Entities.ConfigurazioneCertificazioneVaccinazioni In list

                codici.AppendFormat("{0}, ", vac.CodVacNDose)
                descrizioni.AppendFormat("{0}, ", vac.DescVacNDose)

                listaFiltri.Add(New Entities.Filtro() With {.Codice = vac.CodiceVaccino, .Valore = vac.NumeroDose})

            Next

            If codici.Length > 0 Then codici.RemoveLast(2)
            If descrizioni.Length > 0 Then descrizioni.RemoveLast(2)

            conf.listCodVacciniDosi = codici.ToString()
            conf.listDescVacciniDosi = descrizioni.ToString()
            conf.filtri = listaFiltri

        Next

        Return listConfigurazioni

    End Function

    ''' <summary>
    ''' Salvataggio configurazione certificato e relativi controlli di obbligatorieta
    ''' </summary>
    ''' <param name="configurazione"></param>
    ''' <param name="vacciniDosi"></param>
    ''' <returns></returns>
    Public Function SaveConfigurazione(configurazione As Entities.ConfigurazioneCertificazione, vacciniDosi As List(Of Entities.ConfigurazioneCertificazioneVaccinazioni)) As Biz.BizGenericResult

        Dim ownTransaction As Boolean = False
        Dim idConfig As Integer = 0
        Dim righeCancellate As Integer = 0
        Dim okInsertVaccini As Boolean = True

        If configurazione Is Nothing Then
            '--
            Return New BizGenericResult(False, "Salvataggio non effettuato: nessun dato da salvare.")
            '--
        End If
        ' controllo vaccini per obbligatorietà e valorizzazione delle dosi
        If vacciniDosi Is Nothing Or vacciniDosi.Count = 0 Then
            Return New BizGenericResult(False, "Salvataggio non effettuato: Vaccini devono essere valorizzati.")
        Else
            For Each vac As Entities.ConfigurazioneCertificazioneVaccinazioni In vacciniDosi
                If Not vac.NumeroDose.HasValue Then
                    Return New BizGenericResult(False, "Salvataggio non effettuato: la dose dei vaccini deve essere valorizzata.")
                End If
            Next
        End If
        ' controllo obbligo di eta d'inizio
        If Not configurazione.EtaAnnoDa.HasValue OrElse Not configurazione.EtaMeseDa.HasValue OrElse Not configurazione.EtaGiornoDa.HasValue Then
            Return New BizGenericResult(False, "Salvataggio non effettuato: campi eta inizio obbligatori.")
        End If

        ' controllo obbligo di eta fine 
        If Not configurazione.EtaAnnoA.HasValue OrElse Not configurazione.EtaMeseA.HasValue OrElse Not configurazione.EtaGiornoA.HasValue Then
            Return New BizGenericResult(False, "Salvataggio non effettuato: campi eta fine obbligatori.")
        End If
        ' controllo obbligo per data nscita da
        If Not configurazione.DataNascitaDa.HasValue Then
            Return New BizGenericResult(False, "Salvataggio non effettuato: campo data nascita da obbligatorio.")
        End If
        ' controllo obbligo per data nscita a
        If Not configurazione.DataNascitaA.HasValue Then
            Return New BizGenericResult(False, "Salvataggio non effettuato: campo data nascita a obbligatorio.")
        End If

        ' controllo obbligo sesso
        If configurazione.Sesso.IsNullOrEmpty Then
            Return New BizGenericResult(False, "Salvataggio non effettuato: sesso obbligatorio.")
        End If

        Try
            If Me.GenericProvider.Transaction Is Nothing Then
                Me.GenericProvider.BeginTransaction()
                ownTransaction = True
            End If
            If configurazione.IsNew Then

                idConfig = InsertConfigurazioneCertificati(configurazione)
                configurazione.Id = idConfig
                For Each vac As Entities.ConfigurazioneCertificazioneVaccinazioni In vacciniDosi
                    vac.IdConfigurazioneCertificato = idConfig
                    okInsertVaccini = InsertConfigurazioneCertificatiVaccini(vac)
                Next
                If idConfig > 0 Then configurazione.IsNew = False

            Else

                idConfig = UpdateConfigurazioneCertificati(configurazione)
                righeCancellate = DeleteConfigurazioneVaccini(configurazione.Id)
                '' aggiungere i controlli
                For Each vac As Entities.ConfigurazioneCertificazioneVaccinazioni In vacciniDosi
                    okInsertVaccini = InsertConfigurazioneCertificatiVaccini(vac)
                Next


                If idConfig = 0 Then configurazione.IsNew = True

            End If
            If ownTransaction Then
                Me.GenericProvider.Commit()
            End If
        Catch ex As Exception
            If ownTransaction Then
                Me.GenericProvider.Rollback()
            End If

            ex.InternalPreserveStackTrace()
            Throw
        End Try
        Return New BizGenericResult()
    End Function
    ''' <summary>
    ''' Cancella configurazione certificati
    ''' </summary>
    ''' <param name="idConfigurazione"></param>
    ''' <returns></returns>
    Public Function DeleteConfigurazioni(idConfigurazione As Integer) As BizGenericResult
        Dim ownTransaction As Boolean = False

        Try
            If Me.GenericProvider.Transaction Is Nothing Then
                Me.GenericProvider.BeginTransaction()
                ownTransaction = True
            End If
            GenericProvider.ConfigurazioneCertificato.DeleteConfigurazioneCertVaccini(idConfigurazione)
            GenericProvider.ConfigurazioneCertificato.DeleteConfigurazioneCertificato(idConfigurazione)
            If ownTransaction Then
                Me.GenericProvider.Commit()
            End If
        Catch ex As Exception
            If ownTransaction Then
                Me.GenericProvider.Rollback()
            End If

            ex.InternalPreserveStackTrace()
            Throw
        End Try
        Return New BizGenericResult()

    End Function
    ''' <summary>
    ''' Recupera scritta certificato per report
    ''' </summary>
    ''' <param name="idPaz"></param>
    ''' <returns></returns>
    Public Function GetScrittaCertificatoPaz(idPaz As String) As String
        Dim retScrittaCert As String = String.Empty
        retScrittaCert = GenericProvider.ConfigurazioneCertificato.GetScrittaCertificato(idPaz)
        Return retScrittaCert
    End Function
    ''' <summary>
    ''' Inserimento della configurazione certificati
    ''' </summary>
    ''' <param name="configurazione"></param>
    ''' <returns></returns>
    Private Function InsertConfigurazioneCertificati(configurazione As Entities.ConfigurazioneCertificazione) As Integer

        Dim idConfig As Integer = 0
        ' Insert configurazione
        idConfig = Me.GenericProvider.ConfigurazioneCertificato.InsertConfigurazioneCertificato(configurazione)
        Return idConfig

    End Function

    ''' <summary>
    ''' Update della configurazione certificati
    ''' </summary>
    ''' <param name="configurazione"></param>
    ''' <returns></returns>
    Private Function UpdateConfigurazioneCertificati(configurazione As Entities.ConfigurazioneCertificazione) As Integer

        Dim idConfigurazione As Integer = 0

        ' Update 
        Me.GenericProvider.ConfigurazioneCertificato.UpdateConfigurazioneCertificato(configurazione)

        Return idConfigurazione

    End Function
    ''' <summary>
    ''' Insert in configurazione vaccini 
    ''' </summary>
    ''' <param name="configurazioneVaccini"></param>
    ''' <returns></returns>
    Private Function InsertConfigurazioneCertificatiVaccini(configurazioneVaccini As Entities.ConfigurazioneCertificazioneVaccinazioni) As Boolean

        Dim ok As Boolean = 0

        ' Insert configurazione
        ok = Me.GenericProvider.ConfigurazioneCertificato.InsertConfigurazioneCertificatoVaccini(configurazioneVaccini)

        Return ok

    End Function
    ''' <summary>
    ''' Delete in configurazione vaccini 
    ''' </summary>
    ''' <param name="idconfigurazione"></param>
    ''' <returns></returns>
    Private Function DeleteConfigurazioneVaccini(idconfigurazione As Integer) As Integer

        Dim idConfigurazioni As Integer = 0

        idConfigurazioni = GenericProvider.ConfigurazioneCertificato.DeleteConfigurazioneCertVaccini(idconfigurazione)

        Return idConfigurazioni

    End Function
    ''' <summary>
    ''' Delete Configurazione
    ''' </summary>
    ''' <param name="idconfigurazione"></param>
    ''' <returns></returns>
    Private Function DeleteConfigurazione(idconfigurazione As Integer) As Integer

        Dim idConfigurazioni As Integer = 0

        idConfigurazioni = GenericProvider.ConfigurazioneCertificato.DeleteConfigurazioneCertificato(idconfigurazione)

        Return idConfigurazioni

    End Function
End Class

