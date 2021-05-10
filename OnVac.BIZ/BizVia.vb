Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL
Imports Onit.OnAssistnet.OnVac.Log


Public Class BizVia
    Inherits Biz.BizClass

#Region " Constructors "

    ''' <summary>
    ''' Costruttore classe BizVia
    ''' </summary>
    ''' <param name="genericprovider"></param>
    ''' <param name="settings"></param>
    ''' <param name="contextInfos"></param>
    ''' <param name="logOptions"></param>
    ''' <remarks></remarks>
    Public Sub New(ByRef genericprovider As DbGenericProvider, ByRef settings As Onit.OnAssistnet.OnVac.Settings.Settings, ByVal contextInfos As BizContextInfos, ByVal logOptions As BizLogOptions)

        MyBase.New(genericprovider, settings, contextInfos, LogOptions)

    End Sub



#End Region

    ''' <summary>
    ''' Restituisce true se ci sono altri record, escluso quello corrente, relativi al codice via e al comune specificati, ma con descrizione diversa.
    ''' </summary>
    ''' <param name="codiceVia"></param>
    ''' <param name="codiceComune"></param>
    ''' <param name="descrizioneVia"></param>
    ''' <param name="progressivo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ExistsDescrizioneDiversa(codiceVia As String, codiceComune As String, descrizioneVia As String, progressivo As Integer) As Boolean

        If progressivo <= 0 Then

            Me.GenericProvider.Via.ExistsDescrizioneDiversa(codiceVia, codiceComune, descrizioneVia)

        End If

        Return Me.GenericProvider.Via.ExistsDescrizioneDiversa(codiceVia, codiceComune, descrizioneVia, progressivo)

    End Function

    ''' <summary>
    ''' Restituisce true se ci sono altri record relativi al codice della via e al comune specificati, oltre a quello corrente, 
    ''' con flag default impostato a true.
    ''' </summary>
    ''' <param name="codiceVia"></param>
    ''' <param name="codiceComune"></param>
    ''' <param name="progressivo"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ExistsDefault(codiceVia As String, codiceComune As String, progressivo As Integer) As Boolean

        If progressivo <= 0 Then

            Return Me.GenericProvider.Via.ExistsDefault(codiceVia, codiceComune)

        End If

        Return Me.GenericProvider.Via.ExistsDefault(codiceVia, codiceComune, progressivo)

    End Function

    ''' <summary>
    ''' Salvataggio indirizzo paziente 
    ''' </summary>
    ''' <param name="indirizzoPaziente"></param>
    ''' <remarks></remarks>
    Public Sub SaveIndirizzoPaziente(indirizzoPaziente As Entities.IndirizzoPaziente)

        Using transactionScope As New System.Transactions.TransactionScope(Transactions.TransactionScopeOption.Required, Me.GetReadCommittedTransactionOptions())

            Dim codiceIndirizzo As Integer = 0

            If indirizzoPaziente.Manuale Then
                '--
                ' INDIRIZZO MANUALE
                ' t_paz_pazienti    => sbiancamento codice indirizzo e valorizzazione del campo descrittivo con il valore inserito dall'utente nel campo libero.
                ' t_paz_indirizzi   => cancellazione indirizzo codificato.
                '--
                codiceIndirizzo = Me.UpdateIndirizzoManuale(indirizzoPaziente)
            Else
                '--
                ' INDIRIZZO CODIFICATO
                ' t_paz_indirizzi   => inserimento indirizzo codificato con i dati inseriti dall'utente.
                ' t_paz_pazienti    => update del codice indirizzo (quello appena inserito) e del campo descrittivo con il valore creato in base ai dati inseriti dall'operatore.
                '--
                If indirizzoPaziente.IsNew Then

                    codiceIndirizzo = InsertIndirizzoCodificatoPaziente(indirizzoPaziente)
                    indirizzoPaziente.Codice = codiceIndirizzo

                    If codiceIndirizzo > 0 Then indirizzoPaziente.IsNew = False

                Else

                    codiceIndirizzo = UpdateIndirizzoCodificatoPaziente(indirizzoPaziente)
                    indirizzoPaziente.Codice = codiceIndirizzo

                    If codiceIndirizzo = 0 Then indirizzoPaziente.IsNew = True

                End If

            End If

            transactionScope.Complete()

        End Using

    End Sub

    ''' <summary>
    ''' Inserimento indirizzo codificato del paziente
    ''' </summary>
    ''' <param name="indirizzoPaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function InsertIndirizzoCodificatoPaziente(indirizzoPaziente As Entities.IndirizzoPaziente) As Integer

        Dim codiceIndirizzo As Integer = 0

        ' Recupero la descrizione della via
        Dim listDescrizioniVie As List(Of String) = Me.GenericProvider.Via.GetDescrizioniViaByCodice(indirizzoPaziente.Via.Codice)

        Dim descrizioneVia As String = String.Empty

        If listDescrizioniVie Is Nothing OrElse listDescrizioniVie.Count > 0 Then
            descrizioneVia = listDescrizioniVie.First()
        End If

        indirizzoPaziente.Via.Descrizione = descrizioneVia

        ' Insert indirizzo in T_PAZ_INDIRIZZI
        codiceIndirizzo = Me.GenericProvider.Via.InsertIndirizzoCodificato(indirizzoPaziente)

        ' Update dei campi codice e indirizzo in T_PAZ_PAZIENTI
        Dim descrizioneIndirizzo As String = Me.GetIndirizzoEsteso(indirizzoPaziente)

        If indirizzoPaziente.Tipo = Enumerators.TipoIndirizzo.Residenza Then

            Me.GenericProvider.Paziente.UpdateCodiceEIndirizzoResidenzaPaziente(indirizzoPaziente.Paziente, codiceIndirizzo, descrizioneIndirizzo)

        ElseIf indirizzoPaziente.Tipo = Enumerators.TipoIndirizzo.Domicilio Then

            Me.GenericProvider.Paziente.UpdateCodiceEIndirizzoDomicilioPaziente(indirizzoPaziente.Paziente, codiceIndirizzo, descrizioneIndirizzo)

        End If

        Return codiceIndirizzo

    End Function

    ''' <summary>
    ''' Update indirizzo codificato del paziente
    ''' </summary>
    ''' <param name="indirizzoPaziente"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdateIndirizzoCodificatoPaziente(indirizzoPaziente As Entities.IndirizzoPaziente) As Integer

        Dim codiceIndirizzo As Integer = 0

        ' Recupero la descrizione della via
        Dim listDescrizioniVie As List(Of String) = Me.GenericProvider.Via.GetDescrizioniViaByCodice(indirizzoPaziente.Via.Codice)

        Dim descrizioneVia As String = String.Empty

        If listDescrizioniVie Is Nothing OrElse listDescrizioniVie.Count > 0 Then
            descrizioneVia = listDescrizioniVie.First()
        End If

        indirizzoPaziente.Via.Descrizione = descrizioneVia

        ' Update t_paz_indirizzi
        Me.GenericProvider.Via.UpdateIndirizzoCodificato(indirizzoPaziente)

        ' Update del solo campo indirizzo in t_paz_pazienti
        Dim descrizioneIndirizzo As String = Me.GetIndirizzoEsteso(indirizzoPaziente)

        If indirizzoPaziente.Tipo = Enumerators.TipoIndirizzo.Residenza Then

            Me.GenericProvider.Paziente.UpdateIndirizzoResidenzaPaziente(indirizzoPaziente.Paziente, descrizioneIndirizzo)

        ElseIf indirizzoPaziente.Tipo = Enumerators.TipoIndirizzo.Domicilio Then

            Me.GenericProvider.Paziente.UpdateIndirizzoDomicilioPaziente(indirizzoPaziente.Paziente, descrizioneIndirizzo)

        End If

        Return codiceIndirizzo

    End Function

    Private Function GetIndirizzoEsteso(indirizzoPaziente As Entities.IndirizzoPaziente) As String

        Dim indirizzo As New System.Text.StringBuilder()

        indirizzo.AppendFormat("{0}, {1}", indirizzoPaziente.Via.Descrizione, indirizzoPaziente.NCivico)

        If Not String.IsNullOrEmpty(indirizzoPaziente.CivicoLettera) Then
            indirizzo.AppendFormat("/{0}", indirizzoPaziente.CivicoLettera)
        End If

        If Not String.IsNullOrEmpty(indirizzoPaziente.Interno) Then
            indirizzo.AppendFormat(" Int:{0}", indirizzoPaziente.Interno)
        End If

        If Not String.IsNullOrEmpty(indirizzoPaziente.Lotto) Then
            indirizzo.AppendFormat(" Lotto:{0}", indirizzoPaziente.Lotto)
        End If

        If Not String.IsNullOrEmpty(indirizzoPaziente.Palazzina) Then
            indirizzo.AppendFormat(" Palaz.:{0}", indirizzoPaziente.Palazzina)
        End If

        If Not String.IsNullOrEmpty(indirizzoPaziente.Scala) Then
            indirizzo.AppendFormat(" Scala:{0}", indirizzoPaziente.Scala)
        End If

        If Not String.IsNullOrEmpty(indirizzoPaziente.Piano) Then
            indirizzo.AppendFormat(" Piano:{0}", indirizzoPaziente.Piano)
        End If

        Return indirizzo.ToString()

    End Function

    Private Function UpdateIndirizzoManuale(indirizzoPaziente As Entities.IndirizzoPaziente) As Integer

        Dim codiceIndirizzo As Integer = 0

        If indirizzoPaziente.Tipo = Enumerators.TipoIndirizzo.Residenza Then

            ' Recupero il codice dell'indirizzo di residenza del paziente, prima di sbiancarlo
            codiceIndirizzo = Me.GenericProvider.Paziente.GetCodiceIndirizzoResidenzaPaziente(indirizzoPaziente.Paziente)

            ' Update residenza t_paz_pazienti per cancellare il codice indirizzo
            Me.GenericProvider.Paziente.UpdateCodiceEIndirizzoResidenzaPaziente(indirizzoPaziente.Paziente, Nothing, indirizzoPaziente.Libero)

        ElseIf indirizzoPaziente.Tipo = Enumerators.TipoIndirizzo.Domicilio Then

            ' Recupero il codice dell'indirizzo di domicilio del paziente, prima di sbiancarlo
            codiceIndirizzo = Me.GenericProvider.Paziente.GetCodiceIndirizzoDomicilioPaziente(indirizzoPaziente.Paziente)

            ' Update domicilio t_paz_pazienti per cancellare il codice indirizzo
            Me.GenericProvider.Paziente.UpdateCodiceEIndirizzoDomicilioPaziente(indirizzoPaziente.Paziente, Nothing, indirizzoPaziente.Libero)

        End If

        ' Cancellazione indirizzo codificato da t_paz_indirizzi
        If codiceIndirizzo > 0 Then Me.GenericProvider.Via.DeleteIndirizzoCodificato(codiceIndirizzo)

        Return codiceIndirizzo

    End Function

End Class
