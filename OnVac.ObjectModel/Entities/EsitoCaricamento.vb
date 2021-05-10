Imports System.Collections.Generic

Namespace Entities
    <Serializable>
    Public Class FiltroEsitoControlloScuole

        Public Property CodiceCaricamento As Guid
        Public Property IdControllo As String
        Public Property StatoEsito As String

    End Class
    <Serializable>
    Public Class EsitoControlloScuole
        Public Property IdControllo As String
        Public Property CodiceCaricamento As Guid
        Public Property IdEsito As Integer
        Public Property PazCodice As Integer?
        Public Property Cognome As String
        Public Property Nome As String
        Public Property DataDiNascita As Date?
        Public Property Sesso As String
        Public Property CodiceFiscale As String
        Public Property ComuneScuola As String
        Public Property NomeScuola As String
        Public Property IndirizzoScuola As String
        Public Property CocId As Integer?
        Public Property EsitoControllo As String
        Public Property Idoneo As String
        Public Property Vacinato As String
        Public Property DataControllo As Date
        Public Property Errore As String
        Public Property Stato As String

        Public ReadOnly Property Nominativo As String
            Get
                Return String.Format("{0} {1}", Cognome, Nome)
            End Get
        End Property


    End Class


End Namespace
