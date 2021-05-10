Imports System.Collections.Generic

Namespace Entities
    Public Class SalvaRicovero
        Public Property CodiceGruppo As String
        Public Property CodicePaziente As Long

        Public Property Episodi As IEnumerable(Of EpisodioRicovero)

        Public Class EpisodioRicovero
            Public Property Codice As Long?
            Public Property CodiceTipoEvento As Long
            Public Property DataIngresso As Date
            Public Property DataUscita As Date?
            Public Property CodiceStatoClinico As String
            Public Property CodiceStruttura As String
            Public Property CodiceUnitaOperativa As String
            Public Property CodiceArea As Long?
        End Class

    End Class

    Public Class RicoveroPaziente
        Public Property CodiceGruppo As String
        Public Property CodicePaziente As Long

        Public Property Episodi As List(Of EpisodioRicovero)

        Public ReadOnly Property Aperto As Boolean
            Get
                For Each ep As EpisodioRicovero In Episodi
                    If ep.Chiuso Then
                        Return False
                    End If
                Next
                Return True
            End Get
        End Property

        Public ReadOnly Property DataInizio As Date
            Get
                Return Me.Episodi.Select(Function(e)
                                             Return e.DataIngresso
                                         End Function).Min()
            End Get
        End Property

        Public ReadOnly Property DataFine As Date?
            Get
                If Me.Aperto Then
                    Return Nothing
                Else
                    Me.Episodi.Select(Function(e)
                                          Return e.DataIngresso
                                      End Function).Max()
                End If
            End Get
        End Property

        Public Class EpisodioRicovero

            <DbColumnName("RIP_ID")>
            Public Property Codice As Long

            <DbColumnName("RIP_ATR_CODICE")>
            Public Property CodiceTipoEvento As Integer
            <DbColumnName("ATR_DESCRIZIONE")>
            Public Property DescrizioneTipoEvento As String

            <DbColumnName("ATR_APERTO")>
            Public Property Aperto As Boolean
            <DbColumnName("ATR_CHIUSO")>
            Public Property Chiuso As Boolean

            <DbColumnName("RIP_DATA_INGRESSO")>
            Public Property DataIngresso As Date
            <DbColumnName("RIP_DATA_USCITA")>
            Public Property DataUscita As Date?

            <DbColumnName("RIP_STC")>
            Public Property CodiceStatoClinico As String
            <DbColumnName("RIP_STC_DESCRIZIONE")>
            Public Property DescrizioneStatoClinico As String

            <DbColumnName("RIP_HSP_ID")>
            Public Property CodiceStruttura As String
            <DbColumnName("HSP_DESCRIZIONE")>
            Public Property DescrizioneStruttura As String

            <DbColumnName("RIP_UOP")>
            Public Property CodiceUnitaOperativa As String
            <DbColumnName("RIP_UOP_DESCRIZIONE")>
            Public Property DescrizioneUnitaOperativa As String

            <DbColumnName("RIP_ARE_CODICE")>
            Public Property CodiceArea As Integer?
            <DbColumnName("ARE_DESCRIZIONE")>
            Public Property DescrizioneArea As String

            <DbColumnName("RIP_REPARTO_DESCRIZIONE")>
            Public Property DescrizioneRepartoPic As String
        End Class
    End Class


    Public Class TestataRicovero
        Public Property CodiceGruppoRicovero As String
        Public Property DataInizio As Date
        Public Property CodicePaziente As Long
        Public Property NomePaziente As String
        Public Property CognomePaziente As String
        Public Property DataDiNascita As Date?
        Public Property Sesso As String
        Public Property DataUltimoTamponePositivo As Date?
        Public Property DataUltimoTampone As Date?
        Public Property CodiceStatoClinico As String
        Public Property DescrizioneStatoClinico As String
        Public Property CodiceStruttura As String
        Public Property DescrizioneStruttura As String
        Public Property CodiceReparto As Integer?
        Public Property DescrizioneReparto As String
        Public Property Chiuso As Boolean
        Public Property CodiceStatoRicovero As Long?
        Public Property DescrizioneStatoRicovero As String
    End Class
End Namespace

Public Class FiltriRicoveri
    Public Property CodiceUsl As String
    Public Property CodiceStatoClinico As String
    Public Property CodiceStruttura As String
    Public Property CodiceReparto As Integer?
    Public Property SoloAperto As Boolean
    Public Property SoloChiusi As Boolean
    Public Property StatoRicovero As Long?

    Public Property CodicePaziente As Long?

    Public Property Skip As Integer?
    Public Property Take As Integer?
End Class