Imports Onit.OnAssistnet.OnVac.CalcoloAppuntamenti.Timing
Imports Onit.OnAssistnet.OnVac.CalcoloAppuntamenti.MotiviRifiuto

Namespace Filtri
    Public Interface IFiltro

        ReadOnly Property Priorita() As Integer
        ReadOnly Property Obbligatorio() As Boolean
        ReadOnly Property Tipo() As MarcherTipo
        ReadOnly Property GetMotivoRifiuto() As IMotivoRifiuto
        Property FiltriParent() As Filtri.FiltroCollection

        '@ -----------------------------------------------------------------------------
        '@ <summary>
        '@ Applica i marcatori relativi a quella classe di filtro.
        '@ </summary>
        '@ <param name="giorni">Collezione di giorni a cui applicare i marcatori.</param>
        '@ <param name="pars">Pars relativi a tutta la famiglia di filtri. Ogni filtro analizza i suoi parametri che sono descritti all'interno della classe di filtro.</param>
        '@ <returns></returns>
        '@ <history>
        '@ 	[ssabbattini]	02/03/2006	Created
        '@ </history>
        '@ -----------------------------------------------------------------------------
        Function ApplicaMarcher(ByVal giorni As DayCollection, ByVal pars As Hashtable) As DayCollection
        Function ApplicaMarcher(ByVal giorno As Day, ByVal pars As Hashtable) As Day
    End Interface
End Namespace
