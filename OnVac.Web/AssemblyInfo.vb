Imports System.Reflection
Imports System.Runtime.InteropServices

' Le informazioni generali relative a un assembly sono controllate dal seguente
' insieme di attributi. Per modificare le informazioni associate a un assembly 
' occorre quindi modificare i valori di questi attributi.

<Assembly: AssemblyTitle("SIAVr")>
<Assembly: AssemblyCompany("Onit Group srl")>
<Assembly: AssemblyProduct("SIAVr")>
<Assembly: AssemblyCopyright("Copyright © OnitGroup 2017")>
<Assembly: AssemblyTrademark("OnitGroup")>

' Compile a Debug or Release flag into the assembly.
#If DEBUG Then
<Assembly: AssemblyDescription("SIAVr - Gestione vaccinazioni [Debug Version]")>
#Else
<Assembly: AssemblyDescription("SIAVr - Gestione vaccinazioni [Release Version]")> 
#End If

'Se il progetto viene esposto a COM, il GUID che segue verrà utilizzato per creare l'ID della libreria dei tipi.
<Assembly: Guid("828CE151-CA67-49AC-9A75-84F55A0F1414")> 

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