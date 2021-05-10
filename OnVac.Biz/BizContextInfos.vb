Public Class BizContextInfos

    Public ReadOnly IDUtente As Long
    Public ReadOnly CodiceAzienda As String

    Public ReadOnly IDApplicazione As String

    Public ReadOnly CodiceCentroVaccinale As String

    Public ReadOnly CentraleInfos As BizCentraleInfos

    Public ReadOnly CodiceUsl As String

    Public Class BizCentraleInfos

        Public ReadOnly Addetto As String
        Public ReadOnly Ente As String

        Public Sub New(addetto As String, ente As String)
            Me.Addetto = addetto
            Me.Ente = ente
        End Sub

    End Class

    Public Sub New(idUtente As Long, codiceAzienda As String, idApplicazione As String, codiceCentroVaccinale As String, codiceUsl As String, centraleInfos As BizCentraleInfos)

        Me.IDUtente = idUtente
        Me.CodiceAzienda = codiceAzienda

        Me.IDApplicazione = idApplicazione

        Me.CodiceCentroVaccinale = codiceCentroVaccinale

        If centraleInfos Is Nothing Then

            Dim ente As String = String.Format("{0}|{1}", idApplicazione, codiceAzienda)
            Dim addetto As String

            If idUtente > 0 Then
                addetto = idUtente
            Else
                addetto = ente
            End If

            centraleInfos = New BizCentraleInfos(addetto, ente)

        End If

        Me.CentraleInfos = centraleInfos

        Me.CodiceUsl = codiceUsl

    End Sub

    Public Sub New(idUtente As Long, codiceAzienda As String, idApplicazione As String, codiceCentroVaccinale As String)

        Me.New(idUtente, codiceAzienda, idApplicazione, codiceCentroVaccinale, Nothing, Nothing)

    End Sub
    Public Sub New(idUtente As Long, codiceAzienda As String, idApplicazione As String)

        Me.New(idUtente, codiceAzienda, idApplicazione, String.Empty, String.Empty, Nothing)

    End Sub

End Class
