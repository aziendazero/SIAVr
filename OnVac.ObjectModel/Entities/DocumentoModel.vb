Imports System.Runtime.Serialization
Namespace Entities
    <Serializable()>
    <DataContract()>
    Public Class DocumentoModel
        <DataMember()>
        Public Property ID As Integer
        Public Property DataArchiviazione As DateTime
        Public Property Descrizione As String
        Public Property CodicePaziente As String
        Public Property Tipologia As String
        Public Property NomeDocumento As String
        Public Property StatoDocumentoDescrizione As String
        Public Property StatoDocumentoId As Integer
        Public Property Documento64 As String
        Public Property SoggettoEmittente As String
    End Class
End Namespace