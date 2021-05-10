Namespace Entities

    <Serializable()> _
    Public Class PazientiVaccinazioni

        ' Codice del paziente
        Private _cod_paz As String
        Public Property CodicePaziente() As String
            Get
                Return _cod_paz
            End Get
            Set(ByVal Value As String)
                _cod_paz = Value
            End Set
        End Property


        ' Hashtable per mettere in relazione i codici delle vaccinazioni con le relative descrizioni 
        Private _vac_descr As Hashtable
        Public Property VaccinazioniDescrizioni() As Hashtable
            Get
                Return _vac_descr
            End Get
            Set(ByVal Value As Hashtable)
                _vac_descr = Value
            End Set
        End Property


        ' Hashtable per mettere in relazione i codici delle vaccinazioni con le dosi associate
        Private _vac_dose As Hashtable
        Public Property VaccinazioniDosi() As Hashtable
            Get
                Return _vac_dose
            End Get
            Set(ByVal Value As Hashtable)
                _vac_dose = Value
            End Set
        End Property


        Public Sub New()
            _cod_paz = String.Empty
            _vac_descr = New Hashtable
            _vac_dose = New Hashtable
        End Sub


        Public Sub New(ByVal cod_paz As String)
            _cod_paz = cod_paz
            _vac_descr = New Hashtable
            _vac_dose = New Hashtable
        End Sub


    End Class


End Namespace
