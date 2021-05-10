Public Class Consultorio

#Region " Events "

    Public Event TrovatoAppuntamento(paz As CnvPaziente)
    Public Event NonTrovatoAppuntamento(paz As CnvPaziente)

#End Region

#Region " Properties "

    Private _DaData As Date
    Public Property DaData() As Date
        Get
            Return Me._DaData
        End Get
        Set(value As Date)
            Me._DaData = value
        End Set
    End Property

    Private _AData As Date
    Public Property AData() As Date
        Get
            Return Me._AData
        End Get
        Set(value As Date)
            Me._AData = value
        End Set
    End Property

#End Region

#Region " Private "

    Private WithEvents _Ambulatori As New AmbulatorioCollection
    Public ReadOnly Property Ambulatori() As AmbulatorioCollection
        Get
            Return Me._Ambulatori
        End Get
    End Property

    Private WithEvents _Pazienti As New CnvPazienteCollection
    Public ReadOnly Property Pazienti() As CnvPazienteCollection
        Get
            Return Me._Pazienti
        End Get
    End Property

#End Region

#Region " Public Methods "

    Public Sub FindAppuntamenti()
        For Each amb As Ambulatorio In _Ambulatori
            amb.FindAppuntamenti()
        Next
    End Sub

#End Region

#Region " Private Methods "

    '@ -----------------------------------------------------------------------------
    '@ <summary>
    '@ Rimuove il paziente dalla collezione a cui è associato in base alle sue proprietà di Overbooking o SoloBilancio.
    '@ </summary>
    '@ <param name="paziente"></param>
    '@ <param name="cancel"></param>
    '@ <history>
    '@ 	[ssabbattini]	23/02/2006	Created
    '@ </history>
    '@ -----------------------------------------------------------------------------
    Private Sub _Pazienti_RemovePaziente(paziente As CnvPaziente, ByRef cancel As Boolean) Handles _Pazienti.RemovePaziente

        Dim ambulatorio As Ambulatorio = Me._Ambulatori(paziente.Ambulatorio)

        If ambulatorio.InOverbooking Then
            ambulatorio.PazientiRitardatari.Remove(paziente)
        Else
            If paziente.SoloBilancio Then
                ambulatorio.PazientiSoloBilancio.Remove(paziente)
            Else
                ambulatorio.Pazienti.Remove(paziente)
            End If
        End If

        paziente.IsAssociated = False

    End Sub

    '@ -----------------------------------------------------------------------------
    '@ <summary>
    '@ Smista i pazienti nelle proprietà specifiche dell'ambulatorio (controllando le proprietà Overbooking e SoloBilancio). L'elenco completo dei pazienti è comunque disponibile in Pazienti in qualto
    '@ l'operazione non viene cancellata (a meno che non avvenga un'eccezzione).
    '@ </summary>
    '@ <param name="paziente">Nuovo paziente da aggiungere.</param>
    '@ <param name="cancel">Indica se annullare l'ìoperazione di inserimento.</param>
    '@ <exception cref="OnVac.Exceptions.NotExistAmbulatorioException">Scatenato quando si inserisce un paziente con una data e un codice già esistenti.</exception>
    '@ <history>
    '@ 	[ssabbattini]	23/02/2006	Created
    '@ </history>
    '@ -----------------------------------------------------------------------------
    Private Sub _Pazienti_AddPaziente(paziente As CnvPaziente, ByRef cancel As Boolean) Handles _Pazienti.AddPaziente

        If paziente.Ambulatorio Is Nothing OrElse Not Me._Ambulatori.Contains(paziente.Ambulatorio) Then
            Throw New Exceptions.NotExistAmbulatorioException(paziente.Ambulatorio)
        End If

        Dim ambulatorio As Ambulatorio = Me._Ambulatori(paziente.Ambulatorio)

        If ambulatorio.InOverbooking And paziente.Ritardatario Then
            AggiungiRitardatari(ambulatorio, paziente)
        Else
            If paziente.SoloBilancio Then
                AggiungiSoloBilancio(ambulatorio, paziente)
            Else
                ambulatorio.Pazienti.Add(paziente)
            End If
        End If

        paziente.IsAssociated = True

    End Sub

    Private Sub AggiungiSoloBilancio(ambulatorio As Ambulatorio, paziente As CnvPaziente)

        paziente.Preferenze.Clear()

        Dim dataCnv As Date = IIf(_DaData > paziente.DataConvocazione, _DaData, paziente.DataConvocazione)

        Dim fval As New Filtri.FiltroDataValidita()
        fval.DataCnv = dataCnv
        fval.DataValidita = _AData

        paziente.Preferenze.Add(fval)

        paziente.DurataAppuntamento = 0

        ambulatorio.PazientiSoloBilancio.Add(paziente)

    End Sub

    Private Sub AggiungiRitardatari(ambulatorio As Ambulatorio, paziente As CnvPaziente)

        paziente.DurataAppuntamento = 0

        ambulatorio.PazientiRitardatari.Add(paziente)

        Dim fRit As New Filtri.FiltroNumeroMassimoRitardatariGiornalieri(paziente.DataConvocazione, paziente.DataConvocazione, paziente.DataConvocazione)
        ambulatorio.FiltriVariabili.Add(fRit)

    End Sub

    Private Sub _Ambulatori_TrovatoAppuntamento(paz As CnvPaziente) Handles _Ambulatori.TrovatoAppuntamento
        RaiseEvent TrovatoAppuntamento(paz)
    End Sub

    Private Sub _Ambulatori_NonTrovatoAppuntamento(paz As CnvPaziente) Handles _Ambulatori.NonTrovatoAppuntamento
        RaiseEvent NonTrovatoAppuntamento(paz)
    End Sub

#End Region

End Class
