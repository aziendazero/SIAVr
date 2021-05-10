Namespace Entities

    <Serializable()> _
    Public Class DatiLottoEliminato

        Public CodiceLotto As String
        Public DataEsecuzione As DateTime
        Public IdAssociazione As String
        Public Vaccinazioni As String
        Public NumeroDosiDaRipristinare As Integer
        Public CnsCodiceEsecuzione As String
        Public CnsCodiceMagazzino As String
        ''' <summary>
        ''' Indica se il lotto è stato registrato senza effettiva esecuzione
        ''' </summary>
        ''' <remarks></remarks>
        Public IsRegistrato As Boolean

        Public Sub New()
            Me.CodiceLotto = String.Empty
            Me.DataEsecuzione = DateTime.MinValue
            Me.IdAssociazione = String.Empty
            Me.Vaccinazioni = String.Empty
            Me.NumeroDosiDaRipristinare = 0
            Me.CnsCodiceEsecuzione = String.Empty
            Me.CnsCodiceMagazzino = String.Empty
            Me.IsRegistrato = False
        End Sub

        Public Sub New(codiceLotto As String, dataEsecuzione As DateTime, numeroDosiDaRipristinare As Integer, idAssociazione As String, vaccinazioni As String, cnsCodiceEsecuzione As String, cnsCodiceMagazzino As String, isRegistrato As Boolean)
            Me.CodiceLotto = codiceLotto
            Me.DataEsecuzione = dataEsecuzione
            Me.IdAssociazione = idAssociazione
            Me.Vaccinazioni = vaccinazioni
            Me.NumeroDosiDaRipristinare = numeroDosiDaRipristinare
            Me.CnsCodiceEsecuzione = cnsCodiceEsecuzione
            Me.CnsCodiceMagazzino = cnsCodiceMagazzino
            Me.IsRegistrato = isRegistrato
        End Sub

    End Class

End Namespace