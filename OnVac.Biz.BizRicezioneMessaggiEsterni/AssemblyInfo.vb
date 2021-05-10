Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices

' Le informazioni generali relative a un assembly sono controllate dal seguente
' insieme di attributi. Per modificare le informazioni associate a un assembly 
' occorre quindi modificare i valori di questi attributi.

' Rivedere i valori degli attributi dell'assembly.

<Assembly: AssemblyTitle("On.Vac - BizRicezioneMessaggiEsterni")> 
<Assembly: AssemblyCompany("Onit Group srl")> 
<Assembly: AssemblyProduct("On.Vac - BizRicezioneMessaggiEsterni")> 
<Assembly: AssemblyCopyright("Onit")> 
<Assembly: AssemblyTrademark("Onit")> 

' Compile a Debug or Release flag into the assembly.
#If DEBUG Then
<Assembly: AssemblyDescription("Libreria di ricezione messaggi esterni per OnVac [Debug Version]")> 
#Else
<Assembly: AssemblyDescription("Libreria di ricezione messaggi esterni per OnVac [Release Version]")> 
#End If

'Se il progetto viene esposto a COM, il GUID che segue verrà utilizzato per creare l'ID della libreria dei tipi.
<Assembly: Guid("6C2BF929-11DA-4E48-9D8F-067134A533D2")> 

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