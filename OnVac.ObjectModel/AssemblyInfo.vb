Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices

' Le informazioni generali relative a un assembly sono controllate dal seguente
' insieme di attributi. Per modificare le informazioni associate a un assembly 
' occorre quindi modificare i valori di questi attributi.

' Rivedere i valori degli attributi dell'assembly.

<Assembly: AssemblyTitle("OnVac.ObjectModel")> 
<Assembly: AssemblyCompany("OnitGroup Srl")> 
<Assembly: AssemblyProduct("OnVac.ObjectModel")> 
<Assembly: AssemblyCopyright("OnitGroup")> 
<Assembly: AssemblyTrademark("OnitGroup")> 

' Compile a Debug or Release flag into the assembly.
#If DEBUG Then
<Assembly: AssemblyDescription("Debug Version")>
#Else
<Assembly: AssemblyDescription("Release Version")> 
#End If

'Se il progetto viene esposto a COM, il GUID che segue verrà utilizzato per creare l'ID della libreria dei tipi.
<Assembly: Guid("A2DCFA72-6E2B-4A6B-BCEB-D21A6BF6B58F")> 

' Le informazioni sulla versione di un assembly sono costituite dai seguenti valori:
'
'      Numero di versione principale
'      Numero di versione secondario 
'      Numero build
'      Revisione
'
' È possibile specificare tutti i valori oppure impostare valori predefiniti per i numeri relativi alla revisione e alla build
' utilizzando il carattere "*" come mostrato di seguito:

<Assembly: AssemblyVersion(Onit.OnAssistnet.OnVac.Constants.CommonConstants.Version)> 

