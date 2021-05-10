Imports System.Collections.Generic
Namespace Entities

    ''' <summary>
    ''' Entity relativa alle Usl Gestite.
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class ConfigurazioneCertificazione

        Public Property Id As Integer
        Public Property DataNascitaDa As Date?
        Public Property DataNascitaA As Date?
        Public Property EtaAnnoDa As Integer?
        Public Property EtaMeseDa As Integer?
        Public Property EtaGiornoDa As Integer?
        Public Property EtaAnnoA As Integer?
        Public Property EtaMeseA As Integer?
        Public Property EtaGiornoA As Integer?
        Public Property Sesso As String
        Public Property DataControllo As Date?
        Public Property CodiceMotiviImmunita As String
        Public Property CodiceMotiviEsonero As String
        Public Property TestoPositivo As String
        Public Property TestoNegativo As String
        Public Property TestoParziale As String
        Public Property FlgCheckAppuntamenti As String
        Public Property TipoCheckAppuntamenti As String
        Public ReadOnly Property PeriodoDa As String
            Get
                Return String.Format("{0} anni {1} mesi {2} giorni", EtaAnnoDa.ToString(), EtaMeseDa.ToString(), EtaGiornoDa.ToString())
            End Get
        End Property
        Public ReadOnly Property PeriodoA As String
            Get
                Return String.Format("{0} anni {1} mesi {2} giorni", EtaAnnoA.ToString(), EtaMeseA.ToString(), EtaGiornoA.ToString())
            End Get
        End Property

        Public ReadOnly Property SessoCod As String
            Get
                Dim sesCod As String = ""
                If Sesso = "M" Then
                    sesCod = "MASCHIO"
                End If
                If Sesso = "F" Then
                    sesCod = "FEMMINA"
                End If
                If Sesso = "E" Then
                    sesCod = "ENTRAMBI"
                End If
                Return sesCod
            End Get
        End Property
        Public Property listCodVacciniDosi As String
        Public Property listDescVacciniDosi As String
        Public Property filtri As List(Of Filtro)
        Public Property IsNew As Boolean
    End Class
    <Serializable>
    Public Class Filtro
        Public Property Codice As String
        Public Property Descrizione As String
        Public Property Valore As String
    End Class
    ''' <summary>
    ''' Entity relativa alla link vaccini configurazioni
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class ConfigurazioneCertificazioneVaccinazioni

        Public Property IdConfigurazioneCertificato As Integer
        Public Property CodiceVaccino As String
        Public Property NumeroDose As Integer?
        Public Property DescVaccini As String
        Public ReadOnly Property CodVacNDose As String
            Get

                Return String.Format("{0}({1})", CodiceVaccino, NumeroDose.ToString())
            End Get
        End Property
        Public ReadOnly Property DescVacNDose As String
            Get
                Return String.Format("{0}({1})", DescVaccini, NumeroDose.ToString())
            End Get
        End Property

    End Class

End Namespace

