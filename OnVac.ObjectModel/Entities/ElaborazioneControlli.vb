Imports System.Collections.Generic

Namespace Entities

    <Serializable>
    Public Class FiltroElaborazioneControlli

        Public Property IdUtente As Integer?
        Public Property DaData As Date?
        Public Property AData As Date?
        Public Property IdProcesso As Integer?
        Public Property AppId As String

    End Class
    Public Class FiltroSingolaElaborazioneControlli

        Public Property CodiceProcesso As Guid
        Public Property AppId As String

    End Class
    <Serializable>
    Public Class ElaborazioneControlli
        Public Property IdUtenteCaricamento As Integer
        Public Property CodiceUtenteCaricamento As String
        Public Property IdCaricamento As Integer
        Public Property DataCaricamento As Date
        Public Property CodiceCaricamento As Guid
        Public Property AppId As String
        Public Property Argomento As String
        Public Property EnteCaricamento As Guid
        Public Property NomeFileOrigine As String
        Public Property NomeFileElaborazione As String
        Public Property DataInizioCaricamento As Date
        Public Property DataFineCaricamento As Date
        Public Property RigheElaborate As Integer
        Public Property RigheScartate As Integer
        Public Property StatoCaricamento As String
        Public Property IdUtenteControllo As Integer?
        Public Property CodiceUtenteControllo As String
        Public Property IdControllo As Integer?
        Public Property DataInizioControllo As Date?
        Public Property DataFineControllo As Date?
        Public Property StatoControllo As String
        Public Property TotaleRecord As Integer?
        Public Property TotaleControllati As Integer?
        Public Property TotaleErrore As Integer?
        Public Property TotaleVaccinati As Integer?
        Public Property TotaleNonVaccinati As Integer?
        Public Property Copertura As Decimal?
        Public ReadOnly Property RigheElabScart As String
            Get
                Return String.Format("{0}/{1}", RigheElaborate.ToString(), RigheScartate.ToString())
            End Get
        End Property
        Public ReadOnly Property RigheElabDiff As Integer
            Get
                Return RigheElaborate - RigheScartate
            End Get
        End Property

    End Class
    <Serializable>
    Public Class InsertControlliTesta
        Public Property IdCaricamento As Integer
        Public Property CodiceCaricamento As Guid
        Public Property IdUtenteControllo As Integer
        Public Property StatoControllo As String
        Public Property TotaleRecord As Integer
    End Class


End Namespace
