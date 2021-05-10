Namespace Entities

    <Serializable()>
    Public Class ReazioneAvversa

        Public IdReazioneAvversa As Long?
        Public IdVaccinazioneEseguita As Long

        Public CodicePaziente As Long
        Public CodicePazientePrecedente As Long?

        Public CodiceVaccinazione As String
        Public NumeroRichiamo As Short
        Public DataEffettuazione As Date
        Public OraEffettuazione As String

        Public DataReazione As Date

        Public CodiceReazione As String
        Public DescrizioneReazione As String
        Public CodiceReazione1 As String
        Public DescrizioneReazione1 As String
        Public CodiceReazione2 As String
        Public DescrizioneReazione2 As String

        Public AltraReazione As String

        Public Terapie As String
        Public GravitaReazione As String
        Public Grave As String
        Public VisiteRicoveri As String
        Public Esito As String
        Public CodiceEsito As String
        Public DataEsito As Date

        Public StatoAnagrafico As Enumerators.StatoAnagrafico
        Public DataDecesso As Date
        Public MotivoDecesso As String

        Public LuogoDescrizione As String           ' non più gestito, mantenuto per compatibilità con le procedure esistenti (merge, centralizzazione, ...)
        Public AltroLuogoDescrizione As String      ' non più gestito, mantenuto per compatibilità con le procedure esistenti (merge, centralizzazione, ...)
        Public UsoConcomitante As String
        Public CondizioniConcomitanti As String
        Public Qualifica As String
        Public AltraQualifica As String
        Public CognomeSegnalatore As String
        Public NomeSegnalatore As String
        Public IndirizzoSegnalatore As String
        Public TelSegnalatore As String
        Public FaxSegnalatore As String
        Public MailSegnalatore As String
      
        Public DataCompilazione As Date
        Public IdUtenteCompilazione As Long?

        Public CodiceUslInserimento As String
        Public DescrizioneUslInserimento As String

        Public DataModifica As Date?
        Public IdUtenteModifica As Long?

        Public DataEliminazione As Date?
        Public IdUtenteEliminazione As Long?

        ' Farmaco sospetto

        Public DescrizioneNomeCommerciale As String
        Public CodiceLotto As String
        Public CodiceSitoInoculazione As String
        Public DescrizioneSitoInoculazione As String
        Public CodiceViaSomministrazione As String
        Public DescrizioneViaSomministrazione As String
        Public Sospeso As String
        Public Migliorata As String
        Public Ripreso As String
        Public Ricomparsa As String
        Public Indicazioni As String
        Public CodiceIndicazioni As String
        Public Dosaggio As String
        Public Richiamo As Integer
        Public DataScadenzaLotto As Date?            ' non più gestito, mantenuto per compatibilità con le procedure esistenti (merge, centralizzazione, ...)

        ' Farmaci concomitanti
        Public FarmacoConcomitante As String        ' Flag presenza farmaci concomitanti
        Public FarmacoDescrizione As String         ' non più gestito, mantenuto per compatibilità con le procedure esistenti (merge, centralizzazione, ...)

        ' Farmaco concomitante 1
        Public FarmacoConcomitante1_CodiceNomeCommerciale As String
        Public FarmacoConcomitante1_DescrizioneNomeCommerciale As String
        Public FarmacoConcomitante1_CodiceLotto As String
        Public FarmacoConcomitante1_DataScadenzaLotto As Date?
        Public FarmacoConcomitante1_DataOraEffettuazione As DateTime?
        Public FarmacoConcomitante1_Dose As Integer?
        Public FarmacoConcomitante1_CodiceSitoInoculazione As String
        Public FarmacoConcomitante1_DescrizioneSitoInoculazione As String
        Public FarmacoConcomitante1_CodiceViaSomministrazione As String
        Public FarmacoConcomitante1_DescrizioneViaSomministrazione As String
        Public FarmacoConcomitante1_Sospeso As String
        Public FarmacoConcomitante1_Migliorata As String
        Public FarmacoConcomitante1_Ripreso As String
        Public FarmacoConcomitante1_Ricomparsa As String
        Public FarmacoConcomitante1_Indicazioni As String
        Public FarmacoConcomitante1_CodiceIndicazioni As String
        Public FarmacoConcomitante1_Dosaggio As String
        Public FarmacoConcomitante1_Richiamo As Integer?

        ' Farmaco concomitante 2
        Public FarmacoConcomitante2_CodiceNomeCommerciale As String
        Public FarmacoConcomitante2_DescrizioneNomeCommerciale As String
        Public FarmacoConcomitante2_CodiceLotto As String
        Public FarmacoConcomitante2_DataScadenzaLotto As Date?
        Public FarmacoConcomitante2_DataOraEffettuazione As DateTime?
        Public FarmacoConcomitante2_Dose As Integer?
        Public FarmacoConcomitante2_CodiceSitoInoculazione As String
        Public FarmacoConcomitante2_DescrizioneSitoInoculazione As String
        Public FarmacoConcomitante2_CodiceViaSomministrazione As String
        Public FarmacoConcomitante2_DescrizioneViaSomministrazione As String
        Public FarmacoConcomitante2_Sospeso As String
        Public FarmacoConcomitante2_Migliorata As String
        Public FarmacoConcomitante2_Ripreso As String
        Public FarmacoConcomitante2_Ricomparsa As String
        Public FarmacoConcomitante2_Indicazioni As String
        Public FarmacoConcomitante2_CodiceIndicazioni As String
        Public FarmacoConcomitante2_Dosaggio As String
        Public FarmacoConcomitante2_Richiamo As Integer?

        Public AltreInformazioni As String
        Public AmbitoOsservazione As String
        Public AmbitoOsservazione_Studio_Titolo As String
        Public AmbitoOsservazione_Studio_Tipologia As String
        Public AmbitoOsservazione_Studio_Numero As String
        Public Peso As Double?
        Public Altezza As Integer?
        Public DataUltimaMestruazione As DateTime?
        Public Allattamento As String
        Public Gravidanza As String
        Public CausaReazioneOsservata As String
        Public FirmaSegnalatore As String
        Public IdScheda As String
		Public SegnalazioneId As String
		Public UtenteInvio As Integer?
		Public DataInvio As DateTime?
		Public FlagInviato As String


		' Non su db, solo per la visualizzazione
		Public SessoPaziente As String
        Public CodiceOrigineEtnica As String
        Public FlgInsert As Boolean
        Public FlgScaduta As Boolean
        Public VesAssCodice As String
        Public VesAssNDose As Integer

    End Class

End Namespace