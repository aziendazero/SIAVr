Imports Onit.OnAssistnet.OnVac.CalcoloAppuntamenti.MotiviRifiuto

Public Class EsitoProcedura
    Sub New()
        _MotivoRifiuto = New MotiviRifiuto.MotivoRifiutoCollection
    End Sub

    Private _Successo As Boolean
    '@ -----------------------------------------------------------------------------
    '@ <summary>
    '@ Indica se la procedura di ricerca del giorno di appuntamento ha avuto successo.
    '@ </summary>
    '@ <value></value>
    '@ <history>
    '@ 	[ssabbattini]	27/02/2006	Created
    '@ </history>
    '@ -----------------------------------------------------------------------------
    Public Property Successo() As Boolean
        Get
            Return _Successo
        End Get
        Set(ByVal value As Boolean)
            _Successo = value
        End Set
    End Property

    Private _DataAppuntamento As Date
    '@ -----------------------------------------------------------------------------
    '@ <summary>
    '@ La data di appuntamento che è stata trovata.
    '@ </summary>
    '@ <value></value>
    '@ <remarks>La proprietà contiene un valore valido solo se Successo è ugualle a True.</remarks>
    '@ <history>
    '@ 	[ssabbattini]	27/02/2006	Created
    '@ </history>
    '@ -----------------------------------------------------------------------------
    Public Property DataAppuntamento() As Date
        Get
            Return _DataAppuntamento
        End Get
        Set(ByVal value As Date)
            _DataAppuntamento = value
        End Set
    End Property

    Private _MotivoRifiuto As MotivoRifiutoCollection
    '@ -----------------------------------------------------------------------------
    '@ <summary>
    '@ Indica il motivo di rifiuto se la procedura non ha trovato una data di convocazione o ha scartato dei filtri.
    '@ </summary>
    '@ <value></value>
    '@ <history>
    '@ 	[ssabbattini]	27/02/2006	Created
    '@ </history>
    '@ -----------------------------------------------------------------------------
    Public Property MotivoRifiuto() As MotivoRifiutoCollection
        Get
            Return _MotivoRifiuto
        End Get
        Set(ByVal value As MotivoRifiutoCollection)
            _MotivoRifiuto = value
        End Set
    End Property
End Class
