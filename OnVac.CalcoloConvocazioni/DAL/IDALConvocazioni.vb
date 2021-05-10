Namespace DAL

    Friend Interface IDALConvocazioni
        Inherits IDisposable

        ' Properties
        ReadOnly Property Provider() As String
        ReadOnly Property Connection() As IDbConnection
        ReadOnly Property Transaction() As IDbTransaction

        ' Transazione
        Sub BeginTransaction()

        ' Paziente
        Function GetDatiPaziente(codicePaziente As Integer) As ObjectModel.DatiPazienteClass

        ' Vaccinazioni Sostitute
        Function GetVaccinazioneSostituta(codiceVaccinazione As String) As String
        Function ControllaInadempienzaEsclusioneVacPaz(codiceVaccinazione As String, codicePaziente As Integer) As Boolean
        Function GetMaxRichiamo(codicePaziente As Integer, codiceVaccinazione As String) As Integer
        Function GetVaccinazioniBySostituta(codiceVaccinazioneSostituta As String) As ArrayList

        ' Intervalli Sedute
        Function GetIntervalloUltimaEffettuata(codicePaziente As Integer, codiceCiclo As String, numeroSeduta As Integer) As ObjectModel.DatiIntervalliSeduteClass

        ' Ricerca dei pazienti da convocare
        Function GetCodiciPazientiDaConvocare(codiceConsultorio As String, statiAnagrafici As String(), dataNascitaDa As Date, dataNascitaA As Date, tutteCnv As Boolean, sesso As String, codiceMalattia As String, codiceCategoriaRischio As String) As ArrayList

        ' Convocazioni
        Function GetDateConvocazioniPaziente(codicePaziente As Integer) As ArrayList
        Function GetDateConvocazioneVaccinazioniObbligatorie(codicePaziente As Integer) As ArrayList
        Function GetConvocazioniDaProgrammare(codicePaziente As Integer) As DataTable
        Function GetConvocazioniProgrammate(codicePaziente As Integer) As DataTable

        Function UpdateConvocazione(codicePaziente As Integer, dataConvocazione As Date, durata As Int16) As Boolean

        ' Unione Convocazioni
        Function InsertConvocazione(convocazione As Entities.Convocazione) As Boolean
        Function UpdateConvocazione(convocazione As Entities.Convocazione) As Boolean

        ' Modifica Campi Convocazione
        Function UpdateDateAppuntamentoInvio(codicePaziente As Integer, dataConvocazione As Date, valoreData As DateTime, azzeraDataInvio As Boolean) As Integer

        ' Vaccinazioni Programmate
        Function InsertVaccinazioneProgrammata(codicePaziente As Integer, dataConvocazione As Date, codiceVaccinazione As String, codiceCiclo As String, numeroSeduta As Integer, numeroRichiamo As Integer, codiceAssociazione As String, dataInserimento As DateTime, idUtenteInserimento As Long) As Integer
        Function CountVaccinazioniProgrammatePaz(codicePaziente As Integer) As Integer

        ' Cicli
        Function InsertCicloPaziente(codicePaziente As Integer, dataConvocazione As Date, codiceCiclo As String, numeroSeduta As Integer, dataInserimento As DateTime, idUtenteInserimento As Long) As Integer

        ' Data Sospensione
        Function GetMaxDataFineSospensionePaziente(codicePaziente As Integer) As Date

    End Interface

End Namespace

