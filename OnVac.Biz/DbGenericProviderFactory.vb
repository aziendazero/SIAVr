Imports System.Collections.Generic

Imports Onit.OnAssistnet.OnVac.DAL

Public Class DbGenericProviderFactory
    Implements IDisposable

    Private ReadOnly dbGenericProviderDictionary As New Dictionary(Of DbGenericProviderFactoryKey, DbGenericProvider)(New DbGenericProviderFactoryKeyComparer())

    Public Function GetDbGenericProvider(idApplicazione As String, codiceAzienda As String) As DbGenericProvider

        Dim dbGenericProvider As DbGenericProvider = Nothing
        Dim key As New DbGenericProviderFactoryKey() With {.IdApplicazione = idApplicazione, .CodiceAzienda = codiceAzienda}

        Dim loadGenericProviderFromManager As Boolean = True

        If dbGenericProviderDictionary.ContainsKey(key) Then

            ' Prendo il genericProvider dal dictionary, senza caricare i dati dal manager
            dbGenericProvider = dbGenericProviderDictionary(key)
            loadGenericProviderFromManager = False

            ' Se è stata fatta la dispose del genericProvider, risulta ancora nel dictionary ma la connectionstring è vuota. Quindi bisogna crearlo di nuovo.  
            If dbGenericProvider Is Nothing OrElse String.IsNullOrEmpty(dbGenericProvider.Connection.ConnectionString) Then

                dbGenericProviderDictionary.Remove(key)
                loadGenericProviderFromManager = True

            End If

        End If

        If loadGenericProviderFromManager Then

            ' Caricamento dati del genericProvider dal manager
            Using transactionScope As New Transactions.TransactionScope(Transactions.TransactionScopeOption.Suppress)

                Dim app As Onit.Shared.Manager.Apps.App = Onit.Shared.Manager.Apps.App.getInstance(idApplicazione, codiceAzienda)
                dbGenericProvider = New DbGenericProvider(New ConnectionInfo(app.DbmsProvider, app.getConnectionInfo().ConnectionString))

                transactionScope.Complete()

            End Using

            key.IsExternalGenericProvider = False
            dbGenericProviderDictionary.Add(key, dbGenericProvider)

        End If

#If DEBUG Then

        If Not dbGenericProvider Is Nothing AndAlso String.IsNullOrEmpty(dbGenericProvider.Connection.ConnectionString) Then
            Throw New Exception("dbGenericProvider non contiene la connection string.")
        End If

#End If

        Return dbGenericProvider

    End Function

    Public Function AddDbGenericProvider(idApplicazione As String, codiceAzienda As String, dbGenericProvider As DbGenericProvider) As Boolean

        Dim key As New DbGenericProviderFactoryKey() With {.IdApplicazione = idApplicazione, .CodiceAzienda = codiceAzienda}
        key.IsExternalGenericProvider = True
        dbGenericProviderDictionary.Add(key, dbGenericProvider)

    End Function

    Private Class DbGenericProviderFactoryKey

        Public Property IdApplicazione() As String
        Public Property CodiceAzienda() As String
        Public Property IsExternalGenericProvider() As Boolean

        Public ReadOnly Property Value() As String
            Get
                Return String.Format("{0};{1}", IdApplicazione, CodiceAzienda)
            End Get
        End Property

    End Class

    Private Class DbGenericProviderFactoryKeyComparer
        Implements IEqualityComparer(Of DbGenericProviderFactoryKey)

        Public Function Equals(key1 As DbGenericProviderFactoryKey, key2 As DbGenericProviderFactoryKey) As Boolean Implements IEqualityComparer(Of DbGenericProviderFactoryKey).Equals
            If (key1.IdApplicazione = key2.IdApplicazione AndAlso key1.CodiceAzienda = key2.CodiceAzienda) Then
                Return True
            End If
            Return False
        End Function

        Public Function GetHashCode(key As DbGenericProviderFactoryKey) As Integer Implements IEqualityComparer(Of DbGenericProviderFactoryKey).GetHashCode
            key.GetHashCode()
        End Function

    End Class

#Region "IDisposable"

    Private disposedValue As Boolean = False

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then

                For Each dbGenericProviderKeyValuePair As KeyValuePair(Of DbGenericProviderFactoryKey, DbGenericProvider) In dbGenericProviderDictionary
                    If Not dbGenericProviderKeyValuePair.Key.IsExternalGenericProvider Then
                        If Not dbGenericProviderKeyValuePair.Value Is Nothing Then dbGenericProviderKeyValuePair.Value.Dispose()
                    End If
                Next

            End If
            'liberare le risorse non gestite condivise
        End If
        disposedValue = True
    End Sub

    ' Questo codice è aggiunto da Visual Basic per implementare in modo corretto il modello Disposable.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Non modificare questo codice. Inserire il codice di pulitura in Dispose(ByVal disposing As Boolean).
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    Protected Overrides Sub Finalize()
        ' Simply call Dispose(False).
        Dispose(False)
    End Sub

#End Region

End Class
