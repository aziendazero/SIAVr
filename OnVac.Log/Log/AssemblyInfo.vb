Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices

' Le informazioni generali relative a un assembly sono controllate dal seguente
' insieme di attributi. Per modificare le informazioni associate a un assembly 
' occorre quindi modificare i valori di questi attributi.

' Rivedere i valori degli attributi dell'assembly.

<Assembly: AssemblyTitle("OnVac.Log")> 
<Assembly: AssemblyCompany("OnitGroup Srl")> 
<Assembly: AssemblyProduct("OnVac.Log")> 
<Assembly: AssemblyCopyright("OnitGroup Srl")> 
<Assembly: AssemblyTrademark("")> 

#If DEBUG Then
<Assembly: AssemblyDescription("Libreria per il log di OnVac - Debug Version")> 
#Else
<Assembly: AssemblyDescription("Libreria per il log di OnVac - Release Version")> 
#End If

'Se il progetto viene esposto a COM, il GUID che segue verr� utilizzato per creare l'ID della libreria dei tipi.
<Assembly: Guid("20010739-B74B-49DF-B6D9-96A85B5B08A0")> 

' Le informazioni sulla versione di un assembly sono costituite dai seguenti valori:
'
'      Numero di versione principale
'      Numero di versione secondario 
'      Numero build
'      Revisione
'
' � possibile specificare tutti i valori oppure impostare valori predefiniti per i numeri relativi alla revisione e alla build
' utilizzando il carattere "*" come mostrato di seguito:

<Assembly: AssemblyVersion("1.0.1")> 
