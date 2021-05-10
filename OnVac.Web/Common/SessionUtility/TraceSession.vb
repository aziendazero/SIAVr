Imports Onit.OnAssistnet.OnVac.Log.DataLogStructure

Public Class TraceSession

    Private Const ARGOMENTOLOG As String = "SESSIONCLEANER"

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Log delle variabili in Session.
    ''' </summary>
    ''' <param name="s"></param>
    ''' -----------------------------------------------------------------------------
    Public Shared Function TraceSession(ByVal s As System.Web.SessionState.HttpSessionState) As DataTable

        Dim dtSession As New DataTable
        Dim drow As DataRow
        Dim keys As IEnumerator
        Dim key As String
        Dim totSize As Integer = 0

        Dim t As Testata
        Dim r As Record

        Try
            If LogBox.Argomenti.Contains(ARGOMENTOLOG) Then

                keys = s.Keys.GetEnumerator

                dtSession.Columns.Add("Chiave")
                dtSession.Columns.Add("Tipo")
                dtSession.Columns.Add("Dimensione")
                dtSession.Columns.Add("Valore")
                dtSession.Columns.Add("Aggiunta")

                t = New Testata(ARGOMENTOLOG, Operazione.Generico, False)
                t.Intestazione = False

                While (keys.MoveNext)

                    key = keys.Current.ToString

                    If Not s(key) Is Nothing Then

                        drow = dtSession.NewRow
                        drow("Chiave") = key
                        drow("Tipo") = String.Format("{0}({1})", s(key).GetType.ToString, s(key).GetType.IsSerializable.ToString.Chars(0))
                        Dim lc As New LengthCalculator(s(key))
                        totSize += lc.Value
                        drow("Dimensione") = lc.ToString
                        drow("Valore") = s(key).ToString
                        drow("Aggiunta") = "PageName"
                        dtSession.Rows.Add(drow)

                        r = New Record

                        Dim valOld As String = String.Format("Size: {0}{1}{2}Type: {3}", drow("Dimensione"), vbTab, vbTab, drow("Tipo"))
                        Dim valNew As String = String.Format("Value: {0}", drow("Valore"))

                        r.Campi.Add(New Campo(key, valOld, valNew))
                        t.Records.Add(r)

                    End If

                End While

                r = New Record
                r.Campi.Add(New Campo("Dimensione totale", totSize))
                t.Records.Add(r)
                LogBox.WriteData(t)
            End If

        Catch ex As Exception
            ' Se il SessionCleaner va in errore, non deve bloccare l'applicativo
        End Try

        Return dtSession

    End Function

    Public Class LengthCalculator

        Private _length As Integer

        Public Function Value() As Integer
            Return _length
        End Function

        Public Overrides Function ToString() As String
            If _length > 0 Then
                Return _length.ToString
            Else
                Return "Oggetto non serializzabile"
            End If
        End Function

        Private Function Object2ByteLength(ByVal obj As Object) As Integer

            Dim result As Integer = 0
            Dim ms As New System.IO.MemoryStream
            Dim bf As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter

            If Not obj Is Nothing Then
                If obj.GetType.IsSerializable Then
                    Try
                        bf.Serialize(ms, obj)
                        result = ms.ToArray().Length.ToString
                    Catch ex As Exception

                    End Try
                Else
                    Dim objProps() As System.Reflection.PropertyInfo = obj.GetType.GetProperties()
                    For Each p As System.Reflection.PropertyInfo In objProps
                        Try
                            Dim oarr() As Object = Nothing
                            Dim pobj As Object = p.GetValue(obj, oarr)
                            result += Object2ByteLength(pobj)
                        Catch ex As Exception

                        End Try
                    Next
                    Dim objFields() As System.Reflection.FieldInfo = obj.GetType.GetFields()
                    For Each f As System.Reflection.FieldInfo In objFields
                        result += Object2ByteLength(f.GetValue(obj))
                    Next
                End If
            End If
            Return result
        End Function

        Public Sub New(ByVal obj As Object)
            _length = Object2ByteLength(obj)
        End Sub

    End Class


End Class
