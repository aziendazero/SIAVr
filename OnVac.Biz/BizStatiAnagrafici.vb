Imports System.Collections.Generic
Imports Onit.OnAssistnet.OnVac.DAL


Public Class BizStatiAnagrafici
    Inherits BizClass

#Region " Properties "

    Private _message As String

    Public Property Message() As String
        Get
            Return _message
        End Get
        Private Set(value As String)
            _message = value
        End Set
    End Property

#End Region

#Region " Constructors "

    Public Sub New(genericProvider As DbGenericProvider, settings As Settings.Settings, contextInfos As BizContextInfos)

        MyBase.New(genericProvider, settings, contextInfos, Nothing)

    End Sub

#End Region

#Region " Public Methods "

    ''' <summary>
    ''' Restituisce un dataset con gli stati anagrafici presenti nella t_ana_stati_anagrafici
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>In caso di errore scrive sul log</remarks>
    Public Function LeggiStatiAnagrafici() As DataTable

        Dim dt As DataTable = Nothing

        Try
            dt = Me.GenericProvider.StatiAnagrafici.GetStatiAnagrafici()

        Catch ex As Exception

            ' Scrittura sul log effettuata dal motodo della DbStatiAnagraficiProvider

            ' Errore da restiture all'utente
            hasError = True
            Me.Message = Me.GenericProvider.StatiAnagrafici.GetErrorMsg()

            dt = Nothing

        End Try

        Return dt

    End Function

    ''' <summary>
    ''' Restituisce il codice dello stato anagrafico di default
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetStatoAnagraficoDefault() As String

        Return Me.GenericProvider.StatiAnagrafici.GetStatoAnagraficoDefault()

    End Function

    ''' <summary>
    ''' Restituisce una stringa contenente i codici degli stati anagrafici per cui verrà cancellata la programmazione
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetStatiAnagraficiCancellazioneProgrammazione() As String

        Dim listCodici As List(Of String) =
            Me.GenericProvider.StatiAnagrafici.GetStatiAnagraficiCancellazioneProgrammazione()

        If listCodici Is Nothing OrElse listCodici.Count = 0 Then Return String.Empty

        Return String.Join("|", listCodici.ToArray())

    End Function

    ''' <summary>
    ''' Restituisce true se lo stato anagrafico è tra quelli per cui verrà cancellata la programmazione
    ''' </summary>
    ''' <param name="codiceStatoAnagrafico"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsStatoAnagraficoCancellazioneProgrammazione(codiceStatoAnagrafico As String) As Boolean

        Dim listCodici As List(Of String) =
            Me.GenericProvider.StatiAnagrafici.GetStatiAnagraficiCancellazioneProgrammazione()

        If listCodici Is Nothing OrElse listCodici.Count = 0 Then Return False

        Return listCodici.Contains(codiceStatoAnagrafico)

    End Function
    ''' <summary>
    ''' È il metodo che restituisce la lista di stati relativi all'oggetto della tabella legato all'acronimo. 
    ''' Es: Un record della tabella viaggiatori può essere in stato "elaborato" oppure "da elaborare" in questo caso, per visualizzare tutti gli stati del viaggiatore basta inserire l'acronimo della tabella che in questo caso è "VGC"
    ''' </summary>
    ''' <param name="acronimo">VGC,DOC,REA</param>
    ''' <returns></returns>
    Public Function GetStati(acronimo As String) As List(Of Stato)
        Return GenericProvider.StatiAnagrafici.GetStati(acronimo)
    End Function

#End Region

End Class
