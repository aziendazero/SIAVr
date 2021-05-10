Imports System.Collections.ObjectModel
Imports System.Collections.Generic

Namespace Common.Solleciti

#Region " Enums "

    Public Enum TipoStatoPazientiDaSollecitare
        Sollecito
        SollecitoRaccomandato
        TerminePerentorio
    End Enum

    Public Enum TipoStatoPazientiVaccinazioniNonObbligatorie
        PosticipoGiorni
        SollecitoStandard
        PosticipoSeduta
        EsclusioneStandard
    End Enum

#End Region

#Region " Classes "

    <Serializable()>
    Public Class Vaccinazione

        Public Property Codice() As String
        Public Property Obbligatoria() As Boolean

    End Class

    <Serializable()>
    Public Class PazienteCommon

        Public Property PazienteCodice As String
        Public Property Nome As String
        Public Property Cognome As String
        Public Property DataNascita As Date
        Public Property CodiceStatoAnagrafico As String
        Public Property DescStatoAnagrafico As String
        Public Property DataConvocazione As Date
        Public Property DataAppuntamento As DateTime
        Public Property Vaccinazioni As Collection(Of Vaccinazione)
        Public Property DescrizioneStato As String

        Public Sub New()
            Me.Vaccinazioni = New Collection(Of Vaccinazione)()
        End Sub

    End Class

    <Serializable()>
    Public Class PazienteCicloCommon
        Inherits PazienteCommon

        Public Property Ciclo As String
        Public Property Seduta As Short
        Public Property NumSollecitoSeduta As Short

    End Class

    <Serializable()>
    Public Class PazienteCicloObbligatorioNoTP
        Inherits PazienteCicloCommon

        Public Property MaxSolleciti As Short
        Public Property DataInvio As Date
        Public Property IsMaxSollecitiSeduta As Boolean
        Public Property TipoStato As TipoStatoPazientiDaSollecitare

    End Class

    <Serializable()>
    Public Class PazienteCicloObbligatorioInTP
        Inherits PazienteCicloCommon

        Public Property MaxSolleciti As Short
        Public Property DataInvio As Date
        Public Property CodiceStatoAnagrafico As String
        Public Property DescStatoAnagrafico As String

    End Class

    <Serializable()>
    Public Class PazienteCicloRaccomandatoTerminato
        Inherits PazienteCicloCommon

        Public Property MaxSolleciti As Short

    End Class

    <Serializable()>
    Public Class PazienteCicloNonObbligatorio
        Inherits PazienteCicloCommon

        Public Property MaxSolleciti As Integer
        Public Property DataInvio As Date
        Public Property DurataAppuntamento As Integer
        Public Property Bilancio As Integer
        Public Property Malattia As String
        Public Property Consultorio As String
        Public Property GiorniPosticipo As Integer
        Public Property TipoStato As TipoStatoPazientiVaccinazioniNonObbligatorie

    End Class

    <Serializable()>
    Public Class PazienteNoCiclo
        Inherits PazienteCommon

    End Class

    <Serializable()>
    Public Class PazienteDisallineato
        Inherits PazienteCommon

    End Class

#End Region

End Namespace
