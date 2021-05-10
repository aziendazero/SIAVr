Imports System.Configuration
Imports System.Xml
Imports Onit.OnAssistnet.OnVac.Log.LogWriterProvider

#Region " ESEMPI DI UTILIZZO "

' Ottenere la sezione di configurazione dei Log:
'
'   Dim logConfifHandler As OnVac.Log.ConfigHandler.LogConfigCollection = ConfigurationSettings.GetConfig("Logs")
'
'
' Configurare il web.config per aggiungere dei LogConfig per la generazione di un log su DB e su File.
'
'	<configSections>
'	    <section name="Logs" type="Onit.OnAssistnet.OnVac.Log.ConfigHandler,Log"/>
'	</configSections>
'
'	<Logs>
'		<add name="LogDb" type="Onit.OnAssistnet.OnVac.Log.DbLogWriterProvider"/>
'		<add name="LogFile" type="Onit.OnAssistnet.OnVac.Log.FileLogWriterProvider">
'			<param propertyName="AbsoluteFileName" value="prova.txt"/>
'		</add>
'	</Logs>

#End Region

Namespace Config

    ''' <summary>
    ''' Classe per la parserizzazione del file di configurazione web.config per la generazione delle classi di Log. La classe registra i provider di Log necessari per quella applicazione o sottocartella dell'applicativo.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ConfigHandler
        Implements IConfigurationSectionHandler

#Region " Types "

        ''' <summary>
        ''' Identifica un LogConfig che contiene gli oggetti per l'esecuzione dei Log tramite le classi ILogWriterProvider
        ''' </summary>
        ''' <remarks></remarks>
        Public Class LogConfig
            Implements ICloneable

#Region " Properties "

            Private _Name As String

            ''' <summary>
            ''' Nome da utilizzare per fare riferimento a un oggetto ILogWriterProvider.
            ''' </summary>
            ''' <value>Nome del LogProvider</value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Name() As String
                Get
                    Return _Name
                End Get
                Set(value As String)
                    _Name = value
                End Set
            End Property

            Private _LogProvider As ILogWriterProvider

            ''' <summary>
            ''' Classe che implementa l'interfaccia ILogWriterProvider.
            ''' </summary>
            ''' <value>Classe del LogWriterProvider</value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property LogProvider() As ILogWriterProvider
                Get
                    Return _LogProvider
                End Get
                Set(Value As ILogWriterProvider)
                    _LogProvider = Value
                End Set
            End Property

#End Region

#Region " Public "

            Public Function Clone() As Object Implements System.ICloneable.Clone
                Return Me.MemberwiseClone
            End Function

#End Region

        End Class

        ''' <summary>
        ''' Collezione di LogConfig relativi a quella cartella di un'applicazione.
        ''' </summary>
        ''' <remarks></remarks>
        Public Class LogConfigCollection
            Inherits CollectionBase
            Implements ICloneable

#Region " Private "

            ''' <summary>
            ''' Hashtable parallela per la ricerca per nome degli oggetti.
            ''' </summary>
            ''' <remarks></remarks>
            Private ListHashTable As New Hashtable()

#End Region

#Region " Properties "

            Default Public Property Item(index As Integer) As LogConfig
                Get
                    Return CType(List(index), LogConfig)
                End Get
                Set(Value As LogConfig)
                    List(index) = Value
                End Set
            End Property

            ''' <summary>
            ''' Restituisce un oggetto LogConfig dal suo nome.
            ''' </summary>
            ''' <param name="name">Nome del LogConfig di cui ottenere l'oggetto</param>
            ''' <value>Il LogConfig da ottenere.</value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Default Public Property Item(name As String) As LogConfig
                Get
                    Return CType(ListHashTable(name), LogConfig)
                End Get
                Set(Value As LogConfig)
                    ListHashTable(name) = Value
                End Set
            End Property

            Private _EnableAll As Boolean
            Public Property EnableAll() As Boolean
                Get
                    Return _EnableAll
                End Get
                Set(Value As Boolean)
                    _EnableAll = Value
                End Set
            End Property

            Private _ThrowExceptions As Boolean
            Public Property ThrowExceptions() As Boolean
                Get
                    Return _ThrowExceptions
                End Get
                Set(Value As Boolean)
                    _ThrowExceptions = Value
                End Set
            End Property

#End Region

#Region " Public "

            Public Function Add(value As LogConfig) As Integer
                ListHashTable(value.Name) = value
                Return List.Add(value)
            End Function

            Public Function IndexOf(value As LogConfig) As Integer
                Return List.IndexOf(value)
            End Function

            Public Sub Remove(value As LogConfig)
                ListHashTable.Remove(value.Name)
                List.Remove(value)
            End Sub

            Public Sub Remove(value As String)
                List.Remove(ListHashTable(value))
                ListHashTable.Remove(value)
            End Sub

            Public Function Contains(value As LogConfig) As Boolean
                Return List.Contains(value)
            End Function

            Public Function Clone() As Object Implements System.ICloneable.Clone
                Return Me.MemberwiseClone
            End Function

#End Region

        End Class

#End Region

#Region " Public "

        ''' <summary>
        ''' Richiamato ad ogni load della pagina per la parserizzazione della sezione di configurazione relativa alla registrazione dei log.
        ''' </summary>
        ''' <param name="parent">Contiene l'oggetto padre relativo al file web.config di gerarchia superiore.</param>
        ''' <param name="configContext">Contiene il percorso del file web.config relativo alla chiamata corrente (solo ASP.NET).</param>
        ''' <param name="section">La struttura XML relativa alla sezione personalizzata da analizzare.</param>
        ''' <returns>L'oggetto che contiene il LogConfigCollection relativo alla sezione di configurazione dei Log</returns>
        ''' <remarks></remarks>
        Public Function Create(parent As Object, configContext As Object, section As System.Xml.XmlNode) As Object Implements System.Configuration.IConfigurationSectionHandler.Create

            Dim parentConfig As LogConfigCollection = CType(parent, LogConfigCollection)

            Dim config As LogConfigCollection

            If Not parentConfig Is Nothing Then
                config = CType(parentConfig.Clone, LogConfigCollection)
            Else
                config = New LogConfigCollection()
            End If

            Dim child As XmlNode

            Try
                config.EnableAll = Boolean.Parse(section.Attributes("enable").Value)
            Catch ex As Exception
                config.EnableAll = True
            End Try

            Try
                config.ThrowExceptions = Boolean.Parse(section.Attributes("throwexceptions").Value)
            Catch ex As Exception
                config.ThrowExceptions = False
            End Try

            For Each child In section.ChildNodes
                If child.NodeType <> XmlNodeType.Comment And child.NodeType <> XmlNodeType.Whitespace Then
                    Select Case child.Name
                        Case "add"
                            config.Add(ReadSection(child))
                        Case "remove"
                            config.Remove(child.Attributes("name").Value)
                    End Select
                End If
            Next

            Return config

        End Function

        ''' <summary>
        ''' Crea un singolo oggetto LogConfig in base ai parametri relativi al tag &lt;add&gt;. tramite gli attributi name as type viene generato il LogProvider specifico. name è un nome generico da utilizzare
        ''' durante la ricerca dei LogConfig nella collezione e type è il nome completo della classe specifica del LogWriteProvider. Il tag può contenere sottoelementi &lt;param&gt; che specificano le proprietà
        ''' specifiche dei provider da impostare. propertyName è il nome della proprietà e value il suo valore.
        ''' </summary>
        ''' <param name="section">La sezione relativa a un LogWriteProvider.</param>
        ''' <returns>Il LogConfig relativo a quella sezione di configurazione</returns>
        ''' <remarks>Internamente viene utilizzata la reflection per creare e per impostare le proprietà degli oggetti.</remarks>
        Private Function ReadSection(section As XmlNode) As LogConfig

            Dim childPar As XmlNode
            Dim newObject As New LogConfig()

            newObject.Name = section.Attributes.GetNamedItem("name").Value
            newObject.LogProvider = DirectCast(Activator.CreateInstance(Type.GetType(section.Attributes.GetNamedItem("type").Value)), ILogWriterProvider)
            newObject.LogProvider.Enabled = Boolean.Parse(section.Attributes.GetNamedItem("enable").Value)

            For Each childPar In section.ChildNodes
                If childPar.NodeType <> XmlNodeType.Comment And childPar.NodeType <> XmlNodeType.Whitespace Then
                    Dim valore As Object = childPar.Attributes.GetNamedItem("value").Value
                    CType(newObject.LogProvider, Object).GetType.GetProperty(childPar.Attributes.GetNamedItem("name").Value).SetValue(newObject.LogProvider, valore, Nothing)
                End If
            Next

            Return newObject

        End Function

#End Region

    End Class

End Namespace