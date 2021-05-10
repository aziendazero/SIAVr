Imports System.Collections.Generic

Public Interface IStatiAnagraficiProvider

    ''' <summary>
    ''' Restituisce true se lo stato anagrafico specificato è attivo
    ''' </summary>
    ''' <param name="codiceStatoAnagrafico"></param>
    ''' <returns></returns>
    Function IsStatoAnagraficoAttivo(codiceStatoAnagrafico As String) As Boolean

    ''' <summary>
    ''' Restituisce tutti gli stati anagrafici presenti in tabella
    ''' </summary>
    ''' <remarks>In caso di errore scrive sul file di log</remarks></remarks>
    ''' <returns></returns>
    Function GetStatiAnagrafici() As DataTable

    ''' <summary>
    ''' Restituisce la stringa corrispondente al codice dello stato anagrafico di default.
    ''' </summary>
    ''' <returns></returns>
    Function GetStatoAnagraficoDefault() As String

    ''' <summary>
    ''' Restituisce l'enumerazione corrispondente allo stato anagrafico di default.
    ''' </summary>
    ''' <returns></returns>
    Function GetStatoAnagCodificatoDefault() As Enumerators.StatoAnagrafico

    ''' <summary>
    ''' Restituisce una lista di codici degli stati anagrafici per cui cancellare la programmazione vaccinale
    ''' </summary>
    ''' <returns></returns>
    Function GetStatiAnagraficiCancellazioneProgrammazione() As List(Of String)

    ''' <summary>
    ''' Restituisce gli stati anagrafici che hanno il flag chiamata ad "S"
    ''' </summary>
    ''' <returns></returns>
    Function GetStatiAnagraficiAttivi() As IEnumerable(Of Enumerators.StatoAnagrafico)

    Function GetErrorMsg() As String

    ''' <summary>
    '''Restituisce lo stato anagrafico a partire dalle categiorie cittadino dell'AURV tramite la tabella di mapping
    ''' </summary>
    ''' <returns></returns>
    Function GetStatoAnagraficoFromCategoriaCittadino(categoriaCittadino As String) As String
    Function GetStati(acronimo As String) As List(Of Stato)
End Interface
