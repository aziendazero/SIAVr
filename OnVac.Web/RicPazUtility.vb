'contiene delle funzioni di utilizzo comune da parte della 
'classe visibili solo all'interno di questa dll
Imports System.Reflection


Friend Class RicPazUtility

    'validazione lato server del cognome e del nome 
    Friend Shared Function CognomeNomeValido(ByVal strValue As String) As Boolean
        Dim re As New System.Text.RegularExpressions.Regex("[^a-zA-Z\-' ]")
        Return Not re.IsMatch(strValue)
    End Function

    'restituisce un nome e cognome conforme alle regole di validazione
    Friend Shared Function validaCognomeNome(ByVal strValue As String) As String
        Dim strOut As String
        Dim re As New System.Text.RegularExpressions.Regex("[^a-zA-Z\-' ]")
        strOut = re.Replace(strValue.Trim(), "")
        Return strOut
    End Function


    'fulltrim ed equispaziatura
    Friend Shared Function fullTrim(ByVal strIn As String) As String
        Dim strOut As String
        If strIn Is Nothing Then
            Return Nothing
        Else
            Dim re As New System.Text.RegularExpressions.Regex("\s+")
            strOut = re.Replace(strIn.Trim(), " ")
        End If
        Return strOut
    End Function

    'imposta i controlli dataPiker per validare il range delle date
    ' yearsAgo indica di quanti anni la data di partenza precede quella di oggi
    ' la data massima ovviamente sarà quella attuale
    Friend Shared Sub impostaRangeDate(ByVal ctl As System.Web.UI.Control, ByVal yearsAgo As Integer)
        If ctl Is Nothing Then
            Return
        End If
        Dim strMsg As String = "La data non deve superare quella odierna"
        setProperty(ctl, "DataA", Date.Now.ToString("dd/MM/yyyy"))

        If yearsAgo >= 0 Then
            Dim dateAgo As Date
            dateAgo = Date.Now.AddYears(yearsAgo * -1)
            setProperty(ctl, "DataDa", dateAgo.ToString("dd/MM/yyyy"))
            strMsg += " e deve essere successiva al " + dateAgo.ToString("dd/MM/yyyy")
        End If
        setProperty(ctl, "MessaggioErr", strMsg)
        setProperty(ctl, "ControlloTemporale", True)
    End Sub

    'imposta un valore in una proprietà
    Private Shared Sub setProperty(ByRef TargetObj As Object, ByVal propName As String, ByVal objVal As Object)
        If TargetObj Is Nothing Then
            Return
        End If
        Dim t As Type = TargetObj.GetType
        t.InvokeMember(propName, BindingFlags.SetProperty _
                    , Nothing, TargetObj, New [Object]() {objVal})

    End Sub

End Class
