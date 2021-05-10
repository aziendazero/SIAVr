Imports Onit.OnAssistnet.OnVac.Entities

<Serializable()>
Public Class VariabiliOnVac

#Region " Public "

#Region " Consultorio "

    Private _CNS As Consultorio
    Public Property CNS() As Consultorio
        Get
            Return _CNS
        End Get
        Set(Value As Consultorio)
            _CNS = Value
        End Set
    End Property

    Private _CNSMagazzino As Consultorio
    Public Property CNSMagazzino() As Consultorio
        Get
            Return _CNSMagazzino
        End Get
        Set(Value As Consultorio)
            _CNSMagazzino = Value
        End Set
    End Property

    Private _CNSSovrascritto As Boolean = False
    Public Property CNSSovrascritto() As Boolean
        Get
            Return _CNSSovrascritto
        End Get
        Set(Value As Boolean)
            _CNSSovrascritto = Value
        End Set
    End Property

    ' Codice dell'ambulatorio selezionato nelle programmate
    Private _AMBConvocazione As Integer?
    Public Property AMBConvocazione() As Integer?
        Get
            Return _AMBConvocazione
        End Get
        Set(value As Integer?)
            _AMBConvocazione = value
        End Set
    End Property

#End Region

#Region " Paziente "

    Private _PazId As String
    Public Property PazId() As String
        Get
            If String.IsNullOrEmpty(_PazId) Then _PazId = "-1"
            Return _PazId
        End Get
        Set(Value As String)
            _PazId = Value
            If String.IsNullOrEmpty(_PazId) Then _PazId = "-1"
        End Set
    End Property

    'id del paziente precedentemente selezionato
    Private _PazIdPrecedente As String
    Public Property PazIdPrecedente() As String
        Get
            If String.IsNullOrEmpty(_PazIdPrecedente) Then _PazIdPrecedente = "-1"
            Return _PazIdPrecedente
        End Get
        Set(Value As String)
            _PazIdPrecedente = Value
            If String.IsNullOrEmpty(_PazIdPrecedente) Then _PazIdPrecedente = "-1"
        End Set
    End Property

#End Region

#Region " Medico "

    Private _MedicoResponsabile As VariabiliOnVac.Medico

    ''' <summary>
    ''' Medico Responsabile impostato nelle vaccinazioni programmate
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MedicoResponsabile() As VariabiliOnVac.Medico
        Get
            If _MedicoResponsabile Is Nothing Then _MedicoResponsabile = New VariabiliOnVac.Medico()
            Return _MedicoResponsabile
        End Get
        Set(Value As VariabiliOnVac.Medico)
            _MedicoResponsabile = Value
        End Set
    End Property

    Private _MedicoVaccinante As VariabiliOnVac.Medico

    ''' <summary>
    ''' Medico Vaccinatore impostato nelle vaccinazioni programmate
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MedicoVaccinante() As VariabiliOnVac.Medico
        Get
            If _MedicoVaccinante Is Nothing Then _MedicoVaccinante = New VariabiliOnVac.Medico()
            Return _MedicoVaccinante
        End Get
        Set(Value As VariabiliOnVac.Medico)
            _MedicoVaccinante = Value
        End Set
    End Property

    Private _MedicoInAmbulatorio As Boolean? = Nothing
    Public Property MedicoInAmbulatorio() As Boolean?
        Get
            Return _MedicoInAmbulatorio
        End Get
        Set(Value As Boolean?)
            _MedicoInAmbulatorio = Value
        End Set
    End Property

    Private _MedicoRilevatore As VariabiliOnVac.Medico

    ''' <summary>
    ''' Medico Rilevatore impostato nelle visite o nei bilanci 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MedicoRilevatore() As VariabiliOnVac.Medico
        Get
            If _MedicoRilevatore Is Nothing Then _MedicoRilevatore = New VariabiliOnVac.Medico()
            Return _MedicoRilevatore
        End Get
        Set(Value As VariabiliOnVac.Medico)
            _MedicoRilevatore = Value
        End Set
    End Property

#End Region

#Region " Data esecuzione "

    Public Property DataEsecuzione() As DateTime
        
#End Region

#End Region

#Region " Types "

    <Serializable()>
    Public Class Medico
        Public Codice As String
        Public Nome As String
    End Class

#End Region

End Class
