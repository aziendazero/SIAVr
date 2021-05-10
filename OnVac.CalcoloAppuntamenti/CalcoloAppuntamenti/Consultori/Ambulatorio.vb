Imports Onit.OnAssistnet.OnVac.CalcoloAppuntamenti.Filtri
Imports Onit.OnAssistnet.OnVac.CalcoloAppuntamenti.Timing

Public Class Ambulatorio

#Region " Events "

    Public Event TrovatoAppuntamento(paz As CnvPaziente)
    Public Event NonTrovatoAppuntamento(paz As CnvPaziente)
    Public Event OnPreRicercaAppuntamentoLiberoPaziente(cnvPaziente As CnvPaziente, ByRef eseguiRicerca As Boolean, ByRef motivoRifiutoAppuntamento As MotiviRifiuto.IMotivoRifiuto)

#End Region

#Region " Constructors "

    Sub New(codiceAmbulatorio As String)
        Me._Codice = codiceAmbulatorio
    End Sub

    Sub New(codiceAmbulatorio As String, inOverbooking As Boolean, consultorio As Consultorio)
        Me.New(codiceAmbulatorio)
        Me._ConsultorioParent = consultorio
        Me._InOverbooking = inOverbooking
    End Sub

#End Region

#Region " Properties "

    Private _InOverbooking As Boolean
    Public ReadOnly Property InOverbooking() As Boolean
        Get
            Return _InOverbooking
        End Get
    End Property

    Private _Giorni As New DayCollection
    Public ReadOnly Property Giorni() As DayCollection
        Get
            Return _Giorni
        End Get
    End Property

    Private _Codice As String
    Public ReadOnly Property Codice() As String
        Get
            Return _Codice
        End Get
    End Property

    Private _Descrizione As String
    Public Property Descrizione() As String
        Get
            Return _Descrizione
        End Get
        Set(Value As String)
            _Descrizione = Value
        End Set
    End Property

    Private _FiltriAmbulatorio As New FiltroCollection()
    Public ReadOnly Property FiltriAmbulatorio() As FiltroCollection
        Get
            Return _FiltriAmbulatorio
        End Get
    End Property

    Private _FiltriVariabili As New FiltroCollection()
    Public ReadOnly Property FiltriVariabili() As FiltroCollection
        Get
            Return _FiltriVariabili
        End Get
    End Property

    Private _Pazienti As New CnvPazienteCollection()
    Public ReadOnly Property Pazienti() As CnvPazienteCollection
        Get
            Return _Pazienti
        End Get
    End Property

    Private _PazientiSoloBilancio As New CnvPazienteCollection()
    Public ReadOnly Property PazientiSoloBilancio() As CnvPazienteCollection
        Get
            Return _PazientiSoloBilancio
        End Get
    End Property

    Private _PazientiRitardatari As New CnvPazienteCollection()
    Public ReadOnly Property PazientiRitardatari() As CnvPazienteCollection
        Get
            Return _PazientiRitardatari
        End Get
    End Property

    Private _ConsultorioParent As Consultorio
    Public ReadOnly Property ConsultorioParent() As Consultorio
        Get
            Return _ConsultorioParent
        End Get
    End Property

    Private _NumeroRitardatariGiornalieri As Integer
    Public Property NumeroRitardatariGiornalieri() As Integer
        Get
            Return _NumeroRitardatariGiornalieri
        End Get
        Set(value As Integer)
            _NumeroRitardatariGiornalieri = value
        End Set
    End Property

    Public Property DataAperturaAmbulatorio As DateTime?
    Public Property DataChiusuraAmbulatorio As DateTime?

#End Region

#Region " Public "

    Public Sub FindAppuntamenti()

        If _ConsultorioParent.DaData = Date.MinValue Or _ConsultorioParent.DaData = Date.MinValue Then
            Throw New Exceptions.DataIntervalNotSetException()
        End If

        _Pazienti.UsaOrdineAlfabetico = _ConsultorioParent.Pazienti.UsaOrdineAlfabetico
        _PazientiRitardatari.UsaOrdineAlfabetico = _ConsultorioParent.Pazienti.UsaOrdineAlfabetico
        _PazientiSoloBilancio.UsaOrdineAlfabetico = _ConsultorioParent.Pazienti.UsaOrdineAlfabetico

        Dim maxPriAmbulatorio As Integer = FindMaxPriority(Integer.MaxValue, _FiltriAmbulatorio)

        CreaGiorni()

        ' Creazione collection delle date disponibili
        _Giorni = _FiltriAmbulatorio.ApplyFilter(maxPriAmbulatorio, _Giorni)

        ' Ordinamento ascendente per data
        _Giorni.Sort()

        ' Cancellazione giorni precedenti la data di apertura dell'ambulatorio e successivi la data di chiusura
        ' N.B. : potrebbe essere fatto nell'ApplyFilter
        If _Giorni.Count > 0 AndAlso (DataAperturaAmbulatorio.HasValue OrElse DataChiusuraAmbulatorio.HasValue) Then

            For i As Integer = _Giorni.Count - 1 To 0 Step -1

                Dim giorno As Day = _Giorni(i)
                If (DataAperturaAmbulatorio.HasValue AndAlso giorno.Giorno.Date < DataAperturaAmbulatorio.Value.Date) OrElse
                   (DataChiusuraAmbulatorio.HasValue AndAlso giorno.Giorno.Date > DataChiusuraAmbulatorio.Value.Date) Then

                    _Giorni.RemoveAt(i)

                End If

            Next

        End If

        'For Each d As Timing.Day In _Giorni
        '    d.CheckIsFull(_FiltriVariabili)
        'Next

        'Salva una copia dei filtri variabili in quanto per la ricerca dei pazienti ritardatari i filtri Occupato, 
        ' numeromassimoapp e numeromassimonuoviapp non devono essere tenuti in considerazione
        Dim oldFiltriVariabili As FiltroCollection = _FiltriVariabili.Clone()
        Dim noRitardi As FiltroCollection = _FiltriVariabili.Clone()
        Dim copiaFiltriVariabili As FiltroCollection = _FiltriVariabili.Clone()

        For i As Integer = oldFiltriVariabili.Count - 1 To 0 Step -1
            Dim removeIndex As Integer = -1
            If TypeOf oldFiltriVariabili(i) Is FiltroOccupato Then
                removeIndex = i
            End If
            If TypeOf oldFiltriVariabili(i) Is FiltroNumeroMassimoAppuntamentiGiornalieri Then
                removeIndex = i
            End If
            If TypeOf oldFiltriVariabili(i) Is FiltroNumeroMassimoNuoviAppuntamentiGiornalieri Then
                removeIndex = i
            End If
            If removeIndex > -1 Then oldFiltriVariabili.RemoveAt(i)
        Next

        For i As Integer = copiaFiltriVariabili.Count - 1 To 0 Step -1
            Dim removeIndex As Integer = -1
            If TypeOf copiaFiltriVariabili(i) Is FiltroNumeroMassimoRitardatariGiornalieri Then
                removeIndex = i
            End If
            If removeIndex > -1 Then copiaFiltriVariabili.RemoveAt(i)
        Next

        _FiltriVariabili = copiaFiltriVariabili
        RicercaDataPazienti(_Pazienti, False)

        _FiltriVariabili = New FiltroCollection()
        RicercaDataPazienti(_PazientiSoloBilancio, False)

        _FiltriVariabili = oldFiltriVariabili
        RicercaDataPazienti(_PazientiRitardatari, True)

    End Sub

#End Region

#Region " Private and Protected "

    Private Sub RicercaDataPazienti(ByRef pazCollection As CnvPazienteCollection, inOverbookingProcedure As Boolean)

        Dim maxPriVariabili As Integer = FindMaxPriority(Integer.MaxValue, _FiltriVariabili)

        pazCollection.Sort()

        For Each paz As CnvPaziente In pazCollection

            Dim removePriority As Boolean = True
            Dim maxPriPaziente As Integer = Integer.MaxValue

            While removePriority

                maxPriPaziente = FindMaxPriority(maxPriPaziente, paz.Preferenze)
                removePriority = CanRemovePriority(maxPriPaziente, paz.Preferenze)

                Dim filterMax As Filtri.FiltroCollection = paz.Preferenze.GetFromPriority(maxPriPaziente)

                Dim giorniPaziente As DayCollection = Me._Giorni.Clone()

                giorniPaziente = paz.Preferenze.ApplyFilter(maxPriPaziente, giorniPaziente)
                giorniPaziente = Me._FiltriVariabili.ApplyFilter(maxPriVariabili, giorniPaziente)

                Dim t As TimeBlock = Nothing

                Dim eseguiRicerca As Boolean = True
                Dim motivoRifiutoEsecuzioneRicerca As MotiviRifiuto.IMotivoRifiuto = Nothing

                RaiseEvent OnPreRicercaAppuntamentoLiberoPaziente(paz, eseguiRicerca, motivoRifiutoEsecuzioneRicerca)

                If eseguiRicerca Then
                    t = giorniPaziente.GetAppuntamentoLibero(paz.DurataAppuntamento, Me._FiltriVariabili)
                End If

                If Not t Is Nothing AndAlso t.InOverrideDurata Then
                    paz.DurataAppuntamento = t.OverrideDurata
                End If

                If Not t Is Nothing Then

                    paz.EsitoProcedura.DataAppuntamento = t.ParentDay.Giorno.Add(t.Inizio.TimeOfDay)
                    paz.EsitoProcedura.Successo = True

                    'Aggiunge i filtri variabili
                    If inOverbookingProcedure Then

                        Dim dummy As New Filtri.FiltroNumeroMassimoRitardatariGiornalieri()

                        'imposta le durate dei pazienti in overbooking
                        Dim minutes As Integer = t.ParentDay.TimeBlocks.GetTotalMinutesFree(dummy.Priorita, dummy.Tipo)
                        Dim mediaMinuti As Integer = minutes / Me._FiltriVariabili.GetParam(Filtri.FiltroNumeroMassimoRitardatariGiornalieri.NumeroMassimoRitardatariGiornaliero)

                        If t.Inizio.AddMinutes(mediaMinuti) > t.Fine Then
                            paz.DurataAppuntamento = t.Fine.Subtract(t.Inizio).TotalMinutes
                        Else
                            paz.DurataAppuntamento = mediaMinuti
                        End If

                        Dim frit As New Filtri.FiltroNumeroMassimoRitardatariGiornalieri(t.ParentDay.Giorno, t.Inizio, t.Inizio.AddMinutes(paz.DurataAppuntamento))
                        Me._FiltriVariabili.Add(frit)

                    Else

                        Dim focc As New Filtri.FiltroOccupato(t.ParentDay.Giorno, t.Inizio, t.Inizio.AddMinutes(paz.DurataAppuntamento))
                        Me._FiltriVariabili.Add(focc)

                        If Not Me._FiltriVariabili.GetParam(Filtri.FiltroNumeroMassimoAppuntamentiGiornalieri.NumeroMassimoPrenotatiGiornaliero) Is Nothing Then
                            Dim fmax As New Filtri.FiltroNumeroMassimoAppuntamentiGiornalieri(t.ParentDay.Giorno, t.Inizio, t.Inizio.AddMinutes(paz.DurataAppuntamento))
                            Me._FiltriVariabili.Add(fmax)
                        End If

                        For i As Int16 = 0 To Me._FiltriVariabili.NumeroMassimoBlocchiPersonalizzatiGiornalieri
                            If Not Me._FiltriVariabili.GetParam(Filtri.FiltroNumeroMassimoAppuntamentiBlocco.GetNewParameterID(t.ParentDay.Giorno, i)) Is Nothing Then
                                'Dim durataOverride As ParameterBlocco = _FiltriVariabili.GetParam(Filtri.FiltroNumeroMassimoAppuntamentiBlocco.GetNewParameterID(t.ParentDay.Giorno, i))
                                'Dim fmaxb As New Filtri.FiltroNumeroMassimoAppuntamentiBlocco(t.ParentDay.Giorno, t.Inizio, t.Inizio.AddMinutes(durataOverride.Durata))
                                Dim fmaxb As New Filtri.FiltroNumeroMassimoAppuntamentiBlocco(t.ParentDay.Giorno, t.Inizio, t.Inizio.AddMinutes(paz.DurataAppuntamento))
                                Me._FiltriVariabili.Add(fmaxb)
                            End If
                        Next

                        If Not Me._FiltriVariabili.GetParam(Filtri.FiltroNumeroMassimoNuoviAppuntamentiGiornalieri.NumeroMassimoNuoviPrenotatiGiornaliero) Is Nothing Then
                            Dim fmaxn As New Filtri.FiltroNumeroMassimoNuoviAppuntamentiGiornalieri(t.ParentDay.Giorno, t.Inizio, t.Inizio.AddMinutes(paz.DurataAppuntamento))
                            Me._FiltriVariabili.Add(fmaxn)
                        End If

                    End If

                    RaiseEvent TrovatoAppuntamento(paz)

                    Exit While

                Else

                    ' => Appuntamento non trovato o ricerca non effettuata

                    paz.EsitoProcedura.Successo = False

                    If Not eseguiRicerca Then

                        If Not motivoRifiutoEsecuzioneRicerca Is Nothing Then
                            paz.EsitoProcedura.MotivoRifiuto.Add(motivoRifiutoEsecuzioneRicerca)
                        End If

                    ElseIf removePriority Then

                        If filterMax.Count > 0 Then
                            paz.EsitoProcedura.MotivoRifiuto.Add(filterMax(0).GetMotivoRifiuto)
                        End If

                    Else

                        Dim motivoValidita As Filtri.FiltroDataValidita = paz.Preferenze.GetFromPrioritySingle((New Filtri.FiltroDataValidita).Priorita)

                        If Not motivoValidita Is Nothing AndAlso motivoValidita.DataValidita < Me._ConsultorioParent.DaData Then
                            paz.EsitoProcedura.MotivoRifiuto.Add(motivoValidita.GetMotivoRifiuto)
                        Else
                            paz.EsitoProcedura.MotivoRifiuto.Add(New MotiviRifiuto.RifiutoNoAppuntamentoTrovato())
                        End If

                    End If

                    RaiseEvent NonTrovatoAppuntamento(paz)

                End If

            End While

        Next

    End Sub

    Function GetMotivoRifiuto(ByRef totFiltri As Filtri.FiltroCollection, filtri As Filtri.FiltroCollection, paz As CnvPaziente) As MotiviRifiuto.IMotivoRifiuto

        Return Nothing

    End Function

    '@ -----------------------------------------------------------------------------
    '@ <summary>
    '@ Restituisce la priorità massima del primo filtro non abbligatorio. Se non sono trovati filtri restituisce 0.
    '@ </summary>
    '@ <param name="filtri">Collezione di filtri da cui estrapolare il massimo.</param>
    '@ <returns>Integer. Massima priorità o 0.</returns>
    '@ <history>
    '@ 	[ssabbattini]	28/02/2006	Created
    '@ </history>
    '@ -----------------------------------------------------------------------------
    Private Function FindMaxPriority(oldPri As Integer, filtri As FiltroCollection) As Integer

        Dim maxPri As Integer

        For Each f As IFiltro In filtri
            If Not f.Obbligatorio AndAlso maxPri < f.Priorita AndAlso f.Priorita < oldPri Then
                maxPri = f.Priorita
            End If
        Next

        Return maxPri

    End Function

    Private Function FindMaxPriorityObbligatory(oldPri As Integer, filtri As FiltroCollection) As Integer

        Dim maxPri As Integer

        For Each f As IFiltro In filtri
            If f.Obbligatorio AndAlso maxPri < f.Priorita AndAlso f.Priorita < oldPri Then
                maxPri = f.Priorita
            End If
        Next

        Return maxPri

    End Function

    Private Function CanRemovePriority(maxPri As Integer, filtri As FiltroCollection) As Boolean

        Dim remove As Boolean

        For Each f As IFiltro In filtri
            If Not f.Obbligatorio AndAlso maxPri >= f.Priorita Then
                remove = True
            End If
        Next

        Return remove

    End Function

    Private Sub CreaGiorni()

        Dim numDays As Integer = Me._ConsultorioParent.AData.Subtract(Me._ConsultorioParent.DaData).TotalDays

        For i As Integer = 0 To numDays
            Dim d As New Day(Me._ConsultorioParent.DaData.AddDays(i))
            _Giorni.Add(d)
        Next

    End Sub

#End Region

End Class
