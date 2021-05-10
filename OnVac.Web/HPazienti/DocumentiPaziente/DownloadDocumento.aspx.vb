Imports Onit.Database.DataAccessManager


Partial Class DownloadDocumento
    Inherits OnVac.Common.PageBase

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(sender As System.Object, e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()

        ' N.B. : questa pagina NON deve avere nell'header della pagina la direttiva NO_CACHE!!!
        '        In alcuni browser (es. explorer 8) la presenza di questa direttiva fa sì che non venga visualizzato il pdf 
        Me.DoCache = True

    End Sub

#End Region

    Private Sub Page_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        '--
        Dim bufferSize As Integer = 1024
        Dim buffer(bufferSize - 1) As Byte
        '--
        Dim nome As String
        '--
        Dim dam As IDAM = OnVacUtility.OpenDam()
        '--
        Try
            '--
            dam.QB.NewQuery()
            dam.QB.AddTables("t_paz_documenti")
            dam.QB.AddSelectFields("pdo_nome")
            dam.QB.AddWhereCondition("pdo_id", Comparatori.Uguale, Me.Request.QueryString("Id"), DataTypes.Numero)
            '--
            nome = dam.ExecScalar()
            '--
            dam.QB.NewQuery()
            dam.QB.AddTables("t_paz_documenti")
            dam.QB.AddSelectFields("pdo_file")
            dam.QB.AddWhereCondition("pdo_id", Comparatori.Uguale, Me.Request.QueryString("Id"), DataTypes.Numero)
            '--
            Dim retval As Long = 1024
            '--
            Using reader As IDataReader = dam.BuildDataReader(CommandBehavior.SequentialAccess)
                '--
                Dim startIndex As Long = 0
                Dim i As Integer = 0
                '--
                While reader.Read()
                    '--
                    While retval = bufferSize
                        '--
                        If i > 0 Then ReDim Preserve buffer(bufferSize * (i + 1) - 1)
                        '--
                        retval = reader.GetBytes(0, startIndex, buffer, bufferSize * i, bufferSize)
                        '--
                        startIndex = startIndex + bufferSize
                        '--
                        i += 1
                        '--
                    End While
                    '--
                End While
                '--
            End Using
            '--
            ReDim Preserve buffer(buffer.Length - (bufferSize - retval) - 1)
            '--
        Finally
            OnVacUtility.CloseDam(dam)
        End Try
        '--
        Response.Clear()
        Response.ContentType = Me.GetMimeTypeFromFileName(nome)
        Response.AddHeader("Content-Disposition", String.Format("filename={0};", nome))
        Response.OutputStream.Write(buffer, 0, buffer.Length)
        Response.End()
        '--
    End Sub

    Private Function GetMimeTypeFromFileName(fileName As String) As String

        Dim extension As String = fileName.Substring(fileName.LastIndexOf("."))

        Dim registryKey As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey("MIME\\Database\\Content Type")

        For Each keyName As String In registryKey.GetSubKeyNames()
            If extension.Equals(registryKey.OpenSubKey(keyName).GetValue("Extension")) Then Return keyName
        Next

        Return Constants.MIMEContentType.APPLICATION_OCTET_STREAM

    End Function

End Class
