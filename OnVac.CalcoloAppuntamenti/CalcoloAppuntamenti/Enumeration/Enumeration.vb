Namespace Sorting
    '@ -----------------------------------------------------------------------------
    '@ <summary>
    '@ Direzione di ordinamento.
    '@ </summary>
    '@ <remarks>
    '@ </remarks>
    '@ <history>
    '@ 	[ssabbattini]	14/02/2006	Created
    '@ </history>
    '@ -----------------------------------------------------------------------------
    Public Enum SortDirection
        '@ <summary>Ordine crescente</summary>
        Ascending = 1
        '@ <summary>Ordine Decrescente</summary>
        Descending = 2
    End Enum
End Namespace

Namespace Timing
    '@ -----------------------------------------------------------------------------
    '@ <summary>
    '@ Indica la tipolagia del marcher, la famiglia di filtri a cui fa riferimento.
    '@ </summary>
    '@ <history>
    '@ 	[ssabbattini]	27/02/2006	Created
    '@ </history>
    '@ -----------------------------------------------------------------------------
    Public Enum MarcherTipo
        '@<summary>Filtro relativo all'ambulatorio. Vengono eseguiti all'inizio della procedura, per tutti i pazienti.</summary>
        Ambulatorio
        '@<summary>Filtri relativi al paziente. Vengono riapplicati a d ogni nuovo paziente controllato.</summary>
        Paziente
        '@<summary>Filtro riapplicato ad ogni nuovo appuntamento trovato.</summary>
        Variabile
    End Enum
End Namespace

Namespace Filtri
    Public Enum Ricorsivita
        Settimanale
        Mensile
        Annuale
        Singola
    End Enum
End Namespace

Namespace Filtri
    Public Enum MattinaPomeriggio
        Mattina
        Pomeriggio
    End Enum
End Namespace