
<Serializable()>
Public Class PazienteCentrale

    Public Enum TipoVariazioneEnum
        Added
        Modified
        Deleted
    End Enum

    Public Property codice() As String
    Public Property Cognome() As String
    Public Property Nome() As String
    Public Property Tessera() As String
    Public Property TesseraInizioValidita() As Date
    Public Property TeamEmissione() As String
    Public Property codiceFiscale() As String
    Public Property DataNascita() As String
    Public Property ComCodiceNascita() As String
    Public Property Sesso() As String

    Public Property ResidenzaComCodice() As String
    Public Property ResidenzaIndirizzo() As String
    Public Property ResidenzaCap() As String
    Public Property ResidenzaUslCodice() As String
    Public Property ResidenzaLocalita() As String
    Public Property ResidenzaDataInizio() As String
    Public Property ResidenzaDataFine() As String

    Public Property DomicilioComCodice() As String
    Public Property DomicilioIndirizzo() As String
    Public Property DomicilioCap() As String
    Public Property DomicilioLocalita() As String
    Public Property DomicilioCircoscrizione() As String
    Public Property DomicilioDataInizio() As String
    Public Property DomicilioDataFine() As String

    Public Property CitCodice() As String
    Public Property CitCodiceIstat() As String
    Public Property Telefono1() As String
    Public Property Telefono2() As String
    Public Property Telefono3() As String
    Public Property Email() As String
    Public Property MedCodiceBase() As String
    Public Property DataDecesso() As String
    Public Property ComCodiceDecesso() As String

    Public Property DisCodice() As String

    Public Property EmigrazioneData() As String
    Public Property EmigrazioneComCodice() As String

    Public Property ImmigrazioneData() As String
    Public Property ImmigrazioneComCodice() As String

    Public Property StatoCivile() As String


    Public Property CodiceProfessione() As String
    Public Property CodiceDemografico() As String
    Public Property CEE() As String

    Public Property UslAssistenzaCodice() As String
    Public Property UslAssistenzaDataInizio() As String
    Public Property UslAssistenzaDataCessazione() As String
    Public Property UslAssistenzaMotivoCessazione() As String

    Public Property UslProvenienza() As String

    Public Property Note() As String
    Public Property NoteAcquisizione() As String
    Public Property DataInserimento() As String
    Public Property CfisValidato() As String
    Public Property CfisCertificatore() As String
    Public Property Tipo() As String
    Public Property AliasCodice() As String
    Public Property Local() As String
    Public Property AliasData() As String

    Public Property DataInizioContratto() As Date
    Public Property DataFineContratto() As Date
    Public Property CodiceRegione() As String
    Public Property PosizioneProfessionale() As String
    Public Property SituazioneProfessionale() As String
    Public Property DatoreLavoro() As String
    Public Property TipoAzienda() As String
    Public Property RamoAttivita() As String
    Public Property CodiceAzienda() As String
    Public Property ComCodiceAzienda() As String
    Public Property ComCodiceDemografico() As String
    Public Property DataVariazioneComunale() As DateTime
    Public Property CategoriaRischioCodice() As String
    Public Property NumeroVariazioni() As Integer
    Public Property TipoVariazione() As TipoVariazioneEnum


    'indica se almeno un campo (a parte il codice ) è valorizzato
    Public Function HasData() As Boolean

        Dim blnRet As Boolean

        blnRet = Cognome Is Nothing
        blnRet = blnRet And Nome Is Nothing
        blnRet = blnRet And Tessera Is Nothing
        blnRet = blnRet And codiceFiscale Is Nothing
        blnRet = blnRet And DataNascita Is Nothing
        blnRet = blnRet And ComCodiceNascita Is Nothing
        blnRet = blnRet And Sesso Is Nothing
        blnRet = blnRet And ResidenzaComCodice Is Nothing
        blnRet = blnRet And ResidenzaIndirizzo Is Nothing
        blnRet = blnRet And ResidenzaCap Is Nothing
        blnRet = blnRet And ResidenzaDataInizio Is Nothing
        blnRet = blnRet And ResidenzaDataFine Is Nothing
        blnRet = blnRet And ResidenzaUslCodice Is Nothing
        blnRet = blnRet And ResidenzaLocalita Is Nothing
        blnRet = blnRet And DomicilioComCodice Is Nothing
        blnRet = blnRet And DomicilioIndirizzo Is Nothing
        blnRet = blnRet And DomicilioCap Is Nothing
        blnRet = blnRet And DomicilioDataInizio Is Nothing
        blnRet = blnRet And DomicilioDataFine Is Nothing
        blnRet = blnRet And DomicilioLocalita Is Nothing
        blnRet = blnRet And DomicilioCircoscrizione Is Nothing
        blnRet = blnRet And CitCodice Is Nothing
        blnRet = blnRet And Telefono1 Is Nothing
        blnRet = blnRet And Telefono2 Is Nothing
        blnRet = blnRet And Telefono3 Is Nothing
        blnRet = blnRet And MedCodiceBase Is Nothing
        blnRet = blnRet And DataDecesso Is Nothing
        blnRet = blnRet And UslAssistenzaCodice Is Nothing
        blnRet = blnRet And UslAssistenzaDataInizio Is Nothing
        blnRet = blnRet And UslAssistenzaDataCessazione Is Nothing
        blnRet = blnRet And UslAssistenzaMotivoCessazione Is Nothing
        blnRet = blnRet And DisCodice Is Nothing
        blnRet = blnRet And EmigrazioneData Is Nothing
        blnRet = blnRet And EmigrazioneComCodice Is Nothing
        blnRet = blnRet And ImmigrazioneData Is Nothing
        blnRet = blnRet And ImmigrazioneComCodice Is Nothing
        blnRet = blnRet And StatoCivile Is Nothing
        blnRet = blnRet And ComCodiceDecesso Is Nothing
        blnRet = blnRet And CodiceProfessione Is Nothing
        blnRet = blnRet And CodiceDemografico Is Nothing
        blnRet = blnRet And CEE Is Nothing
        blnRet = blnRet And Note Is Nothing
        blnRet = blnRet And UslProvenienza Is Nothing
        blnRet = blnRet And DataInserimento Is Nothing
        blnRet = blnRet And CfisValidato Is Nothing
        blnRet = blnRet And CfisCertificatore Is Nothing
        blnRet = blnRet And Tipo Is Nothing
        blnRet = blnRet And AliasCodice Is Nothing
        blnRet = blnRet And Local Is Nothing
        blnRet = blnRet And AliasData Is Nothing
        blnRet = blnRet And dataInizioContratto = Nothing
        blnRet = blnRet And dataFineContratto = Nothing
        blnRet = blnRet And ComCodiceDemografico Is Nothing
        blnRet = blnRet And DataVariazioneComunale = Nothing
        blnRet = blnRet And CategoriaRischioCodice = Nothing

        Return Not blnRet

    End Function


End Class
