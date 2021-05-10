Imports Onit.OnAssistnet.OnVac.DAL
Imports System.Collections.Generic


Public Class BizProgressivi
    Inherits BizClass

#Region " Costruttori "

    Public Sub New(contextInfos As BizContextInfos, logOptions As BizLogOptions)
        MyBase.New(contextInfos, logOptions)
    End Sub

    Public Sub New(dbGenericProvider As DbGenericProvider, settings As Onit.OnAssistnet.OnVac.Settings.Settings, contextInfos As BizContextInfos, logOptions As BizLogOptions)
        MyBase.New(dbGenericProvider, settings, contextInfos, logOptions)
    End Sub

#End Region

#Region " Public "

    ''' <summary>
    ''' Calcolo del codice progressivo. Il formato è il seguente: [prefisso][anno][progressivo].
    ''' Il progressivo è letto dalla tabella t_ana_progressivi in base al codice e all'anno corrente.
    ''' Se non lo trova, lo crea. Sempre nella stessa tabella sono specificati il prefisso da utilizzare e la lunghezza del codice.
    ''' Vengono concatenati al progressivo tanti "0" quanti ne servono per raggiungere la lunghezza prevista.
    ''' Se il parametro usa_anno_corrente è false, il progressivo viene creato senza l'indicazione dell'anno.
    ''' </summary>
    ''' <param name="codiceProgressivo"></param>
    ''' <param name="usaAnnoCorrente"></param>
    ''' <returns></returns>
    ''' <remarks>Il calcolo dei progressivi utilizza una transazione interna</remarks>
    Public Function CalcolaProgressivo(codiceProgressivo As String, usaAnnoCorrente As Boolean) As String

        Dim prefisso As String = String.Empty
        Dim valoreProgressivo As String = String.Empty

        Dim lunghezza As Integer = 0

        Dim anno As Integer = 0
        If usaAnnoCorrente Then anno = Date.Now.Year

        Using transactionScope As New System.Transactions.TransactionScope(Transactions.TransactionScopeOption.Suppress)

            Try
                Me.GenericProvider.BeginTransaction(IsolationLevel.ReadCommitted)

                ' Blocca il progressivo con una "finta" update
                Me.GenericProvider.Progressivi.LockProgressivo(codiceProgressivo, anno)

                ' Lettura progressivo
                Dim progressivo As Entities.Progressivi = Me.GenericProvider.Progressivi.SelectProgressivo(codiceProgressivo, anno)

                If Not progressivo Is Nothing Then

                    ' Progressivo trovato: incremento

                    valoreProgressivo = progressivo.Progressivo.ToString()
                    anno = progressivo.Anno
                    prefisso = progressivo.Prefisso
                    lunghezza = progressivo.Lunghezza

                    ' Incremento
                    Me.GenericProvider.Progressivi.UpdateProgressivo(valoreProgressivo + 1, codiceProgressivo, anno)

                Else

                    ' Progressivo non trovato

                    Dim progressivoDaInserire As Entities.Progressivi = Nothing

                    ' Se si usa un progressivo annuo, si deve inserire una nuova riga con l'anno corrente.
                    ' Per non perdere i dati dell'anno precedente (prefisso, lunghezza e max) bisogna cercare
                    ' il progressivo vecchio più recente per copiare i suoi dati. 
                    ' Se non c'è, si inserisce un progressivo da zero.
                    If usaAnnoCorrente Then

                        ' Lettura progressivo precedente con stesso codice, per non perdere lunghezza, prefisso e max.
                        Dim listProgressiviStessoCodice As List(Of Entities.Progressivi) = Me.GenericProvider.Progressivi.LoadProgressivi(codiceProgressivo)

                        If Not listProgressiviStessoCodice Is Nothing Then

                            progressivo = listProgressiviStessoCodice.FirstOrDefault()

                        End If

                    End If

                    If progressivo Is Nothing Then

                        ' Nessun progressivo trovato: 
                        ' inserimento da zero

                        progressivoDaInserire = New Entities.Progressivi()

                        progressivoDaInserire.Progressivo = 1
                        progressivoDaInserire.Codice = codiceProgressivo
                        progressivoDaInserire.Anno = anno
                        progressivoDaInserire.Prefisso = String.Empty
                        progressivoDaInserire.Lunghezza = -1
                        progressivoDaInserire.Max = -1
                        progressivoDaInserire.CodiceAzienda = Me.ContextInfos.CodiceAzienda

                        ' Valori da usare per il calcolo del progressivo da restituire all'utente
                        valoreProgressivo = "1"
                        prefisso = String.Empty
                        lunghezza = 0

                    Else

                        ' Ho trovato un progressivo precedente: 
                        ' inserimento di un nuovo progressivo copiando i dati che non devo perdere

                        progressivoDaInserire = New Entities.Progressivi()

                        progressivoDaInserire.Progressivo = 1
                        progressivoDaInserire.Codice = codiceProgressivo
                        progressivoDaInserire.Anno = anno
                        progressivoDaInserire.Prefisso = progressivo.Prefisso
                        progressivoDaInserire.Lunghezza = progressivo.Lunghezza
                        progressivoDaInserire.Max = progressivo.Max
                        progressivoDaInserire.CodiceAzienda = progressivo.CodiceAzienda

                        ' Valori da usare per il calcolo del progressivo da restituire all'utente
                        valoreProgressivo = "1"
                        prefisso = progressivo.Prefisso
                        lunghezza = progressivo.Lunghezza

                    End If

                    ' Inserimento del nuovo progressivo
                    progressivoDaInserire.Progressivo += 1

                    Me.GenericProvider.Progressivi.InsertProgressivo(progressivoDaInserire)

                End If

                Me.GenericProvider.Commit()

            Catch ex As Exception

                Me.GenericProvider.Rollback()

                ex.InternalPreserveStackTrace()
                Throw

            End Try

            transactionScope.Complete()

        End Using

        ' Calcolo del progressivo da restituire 
        Dim progressivoCorrente As String = String.Empty

        Dim strAnno As String = String.Empty

        If usaAnnoCorrente Then strAnno = anno.ToString()

        If lunghezza > 0 Then

            ' Se il codice è di una lunghezza prestabilita, metto nel codice anche gli zeri che servono per riempirlo.

            Dim lunghezzaProgressivo As Integer = lunghezza - prefisso.Length - strAnno.Length

            progressivoCorrente = String.Format("{0}{1}{2}", prefisso, strAnno, valoreProgressivo.PadLeft(lunghezzaProgressivo, "0"))

        Else
            ' Se il codice non ha una lunghezza fissa, concateno i tre campi e basta.

            progressivoCorrente = String.Format("{0}{1}{2}", prefisso, strAnno, valoreProgressivo)

        End If

        Return progressivoCorrente

    End Function

#End Region

End Class
