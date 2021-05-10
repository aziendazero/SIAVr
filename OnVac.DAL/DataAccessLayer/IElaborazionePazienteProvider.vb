Imports System.Collections.Generic

Public Interface IElaborazionePazienteProvider

    Function CountElaborazioniVaccinazionePaziente(filter As ElaborazioneVaccinazionePazienteFilter) As Long

    Function GetElaborazioniVaccinazionePaziente(filter As ElaborazioneVaccinazionePazienteFilter, pagingOptions As Onit.OnAssistnet.Data.PagingOptions) As IEnumerable(Of Entities.ElaborazioneVaccinazionePaziente)

    Function GetElaborazioniVaccinazionePazienteByIDProcesso(idProcesso As Int64) As IEnumerable(Of Entities.ElaborazioneVaccinazionePaziente)

    Sub UpdateElaborazioneVaccinazionePaziente(elaborazioneVaccinazionePaziente As Entities.ElaborazioneVaccinazionePaziente)

    Function UpdateElaborazioniVaccinazionePaziente(idProcessoElaborazione As Int64, statoElaborazione As Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente, statoElaborazioneUpdated As Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente) As Int64

    Class ElaborazioneVaccinazionePazienteFilter

        Private _Id As Long?
        Public Property Id() As Long?
            Get
                Return _Id
            End Get
            Set(ByVal value As Long?)
                _Id = value
            End Set
        End Property

        Private _DataAcquisizioneDa As DateTime?
        Public Property DataAcquisizioneDa() As DateTime?
            Get
                Return _DataAcquisizioneDa
            End Get
            Set(ByVal value As DateTime?)
                _DataAcquisizioneDa = value
            End Set
        End Property

        Private _DataAcquisizioneA As DateTime?
        Public Property DataAcquisizioneA() As DateTime?
            Get
                Return _DataAcquisizioneA
            End Get
            Set(ByVal value As DateTime?)
                _DataAcquisizioneA = value
            End Set
        End Property

        Private _StatoAcquisizione As Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente?
        Public Property StatoAcquisizione() As Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente?
            Get
                Return _StatoAcquisizione
            End Get
            Set(ByVal value As Enumerators.StatoAcquisizioneElaborazioneVaccinazionePaziente?)
                _StatoAcquisizione = value
            End Set
        End Property

        Private _IdProcessoAcquisizione As Long?
        Public Property IdProcessoAcquisizione() As Long?
            Get
                Return _IdProcessoAcquisizione
            End Get
            Set(ByVal value As Long?)
                _IdProcessoAcquisizione = value
            End Set
        End Property

    End Class

End Interface
