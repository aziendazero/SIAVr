Imports System.Collections.ObjectModel
Imports System.Collections.Generic


Public Interface IMovimentiEsterniCNSProvider

    Function LoadPazientiInIngresso(pazientiInIngressoFilter As MovimentiCNSPazientiInIngressoFilter, orderBy As String) As DstMovimentiEsterni
    Function LoadPazientiInIngresso(pazientiInIngressoFilter As MovimentiCNSPazientiInIngressoFilter, orderBy As String, pagingOptions As MovimentiCNSPagingOptions?) As DstMovimentiEsterni
    Function CountPazientiInIngresso(pazientiInIngressoFilter As MovimentiCNSPazientiInIngressoFilter) As Int32

    Function LoadImmigrati(immigratiFilter As MovimentiCNSImmigratiFilter, pagingOptions As MovimentiCNSPagingOptions?) As DstMovimentiEsterni
    Function CountImmigrati(immigratiFilter As MovimentiCNSImmigratiFilter) As Int32

    Function LoadEmigrati(emigratiFilter As MovimentiCNSEmigratiFilter, pagingOptions As MovimentiCNSPagingOptions?, codiceAslCorrente As String) As DstMovimentiEsterni
    Function CountEmigrati(emigratiFilter As MovimentiCNSEmigratiFilter, codiceAslCorrente As String) As Int32

    'Function EsistePazienteByCodiceAusiliario(codiceAusiliarioPaziente As String) As Boolean
    'Function LoadPazienteByCodiceAusiliario(codiceAusiliarioPaziente As String) As Entities.MovimentoCNS.Paziente
    'Function LoadPazienteByCodice(codicePaziente As String) As Entities.MovimentoCNS.Paziente
    'Sub InserisciPaziente(paziente As Entities.MovimentoCNS.Paziente)
    'Sub ModificaPaziente(paziente As Entities.MovimentoCNS.Paziente)

    'Function LoadIDApplicazioneUslGestitaByCodiceComune(codiceComune As String) As String

    'Function LoadConsultorioInfoByCodiceComune(codiceComune As String) As Entities.MovimentoCNS.ConsultorioInfo
    'Function LoadConsultorioInfoSmistamento() As Entities.MovimentoCNS.ConsultorioInfo

    'Function LoadMalattiePaziente(codicePaziente As String) As Collection(Of Entities.MovimentoCNS.MalattiaPaziente)
    'Function CountMalattiePaziente(codicePaziente As String) As Int32
    'Sub EliminaMalattiePaziente(codicePaziente As String)
    'Sub InserisciMalattiePaziente(codicePaziente As String, malattiePaziente As Collection(Of Entities.MovimentoCNS.MalattiaPaziente))

    'Function LoadVisitePaziente(codicePaziente As String) As Collection(Of Entities.MovimentoCNS.VisitaPaziente)
    'Function CountVisitePaziente(codicePaziente As String) As Int32
    'Sub EliminaVisitePaziente(codicePaziente As String)
    'Sub InserisciVisitePaziente(codicePaziente As String, visitePaziente As Collection(Of Entities.MovimentoCNS.VisitaPaziente))

    'Function LoadVaccinazioniEsclusePaziente(codicePaziente As String) As Collection(Of Entities.MovimentoCNS.VaccinazioneEsclusaPaziente)
    'Function CountVaccinazioniEsclusePaziente(codicePaziente As String) As Int32
    'Sub EliminaVaccinazioniEsclusePaziente(codicePaziente As String)
    'Sub InserisciVaccinazioniEsclusePaziente(codicePaziente As String, vaccinazioniEsclusePaziente As Collection(Of Entities.MovimentoCNS.VaccinazioneEsclusaPaziente))

    'Function LoadRifiutiPaziente(codicePaziente As String) As Collection(Of Entities.MovimentoCNS.RifiutoPaziente)
    'Function CountRifiutiPaziente(codicePaziente As String) As Int32
    'Sub EliminaRifiutiPaziente(codicePaziente As String)
    'Sub InserisciRifiutiPaziente(codicePaziente As String, rifiutiPaziente As Collection(Of Entities.MovimentoCNS.RifiutoPaziente))

    'Function LoadInadempienzePaziente(codicePaziente As String) As Collection(Of Entities.MovimentoCNS.InadempienzaPaziente)
    'Function CountInadempienzePaziente(codicePaziente As String) As Int32
    'Sub EliminaInadempienzePaziente(codicePaziente As String)
    'Sub InserisciInadempienzePaziente(codicePaziente As String, inadempienzePaziente As Collection(Of Entities.MovimentoCNS.InadempienzaPaziente))

    'Function LoadCicliPaziente(codicePaziente As String) As Collection(Of Entities.MovimentoCNS.CicloPaziente)
    'Function CountCicliPaziente(codicePaziente As String) As Int32
    'Sub EliminaCicliPaziente(codicePaziente As String)
    'Sub InserisciCicliPaziente(codicePaziente As String, cicliPaziente As Collection(Of Entities.MovimentoCNS.CicloPaziente))

    'Function LoadVaccinazioniEseguitePaziente(codicePaziente As String) As Collection(Of Entities.MovimentoCNS.VaccinazioneEseguitaPaziente)
    'Sub InserisciVaccinazioniEseguitePaziente(codicePaziente As String, vaccinazioniEseguitePaziente As Collection(Of Entities.MovimentoCNS.VaccinazioneEseguitaPaziente))

    'Function LoadReazioniAvversePaziente(codicePaziente As String) As Collection(Of Entities.MovimentoCNS.ReazioneAvversaPaziente)
    'Sub InserisciReazioniAvversePaziente(codicePaziente As String, reazioniAvversaPaziente As Collection(Of Entities.MovimentoCNS.ReazioneAvversaPaziente))

    'Function LoadConvocazioniPaziente(codicePaziente As String) As Collection(Of Entities.MovimentoCNS.ConvocazionePaziente)
    'Function CountConvocazioniPaziente(codicePaziente As String) As Int32
    'Sub EliminaConvocazioniPaziente(codicePaziente As String)
    'Sub InserisciConvocazioniPaziente(codicePaziente As String, convocazioniPaziente As Collection(Of Entities.MovimentoCNS.ConvocazionePaziente))

    'Function LoadBilanciProgrammatiPaziente(codicePaziente As String) As Collection(Of Entities.MovimentoCNS.BilancioProgrammatoPaziente)
    'Function CountBilanciProgrammatiPaziente(codicePaziente As String) As Int32
    'Sub EliminaBilanciProgrammatiPaziente(codicePaziente As String)
    'Sub InserisciBilanciProgrammatiPaziente(codicePaziente As String, bilanciProgrammatiPaziente As Collection(Of Entities.MovimentoCNS.BilancioProgrammatoPaziente))

End Interface

<Serializable()>
Public Class MovimentiCNSPazientiInIngressoFilter
    Public CodiceConsultorio As String
    Public DataNascitaInizio As DateTime?
    Public DataNascitaFine As DateTime?
    Public DataImmigrazioneInizio As DateTime?
    Public DataImmigrazioneFine As DateTime?
    Public DataResidenzaInizio As DateTime?
    Public DataResidenzaFine As DateTime?
    Public DataDomicilioInizio As DateTime?
    Public DataDomicilioFine As DateTime?
    Public DataAssistenzaInizio As DateTime?
    Public DataAssistenzaFine As DateTime?
    Public Regolarizzato As Boolean?
    Public StatoAcquisizioneDatiVaccinaliCentrale As Integer?
    Public StatiAnagrafici As List(Of String)

    <Serializable()>
    Public Class ValoriFiltroStatoAcquisizione
        Public Const AcquisizioneNonEffettuata As Integer = -1
        Public Const NessunDatoDaAcquisire As Integer = 0
        Public Const AcquisizioneParziale As Integer = 1
        Public Const AcquisizioneTotale As Integer = 2
    End Class

End Class

<Serializable()>
Public Class MovimentiCNSImmigratiFilter
    Public CodiceAsl As String
    Public CodiceConsultorio As String
    Public DataNascitaInizio As DateTime?
    Public DataNascitaFine As DateTime?
    Public DataImmigrazioneInizio As DateTime?
    Public DataImmigrazioneFine As DateTime?
    Public Regolarizzato As Boolean?
    Public CertificatoRichiesto As Boolean?
    Public StatiAcquisizioneImmigrazione As Enumerators.MovimentiCNS.StatoAcquisizioneImmigrazione()
End Class

<Serializable()>
Public Class MovimentiCNSEmigratiFilter
    Public CodiceConsultorio As String
    Public DataNascitaInizio As DateTime?
    Public DataNascitaFine As DateTime?
    Public DataEmigrazioneInizio As DateTime?
    Public DataEmigrazioneFine As DateTime?
    Public CertificatoRichiesto As Boolean?
    Public StatiNotificaEmigrazione As Enumerators.MovimentiCNS.StatoNotificaEmigrazione()
End Class

<Serializable()>
Public Structure MovimentiCNSPagingOptions
    Public StartRecordIndex As Int32
    Public EndRecordIndex As Int32
End Structure
