Namespace Entities
    <Serializable()>
    Public Class Tampone
        Public Property CodiceTampone As Long
        Public Property DataTampone As DateTime?
        Public Property CodiceEsito As String
        Public Property DescrizioneEsito As String
        Public Property Note As String
        Public Property CodiceUsl As String
        Public Property UtenteInserimento As Long
        Public Property CodiceInserimentoUsl As String
        Public Property DataInserimento As DateTime?
        Public Property CodiceCampione As String
        Public Property CodiceLab As String
        Public Property DaVisionare As Boolean
        Public Property DataRichiesta As DateTime?
        Public Property DataReferto As DateTime?
        Public Property CodiceLaboratorio As Long?
        Public Property DescrizioneLaboratorio As String

        Public Property CodiceTipologia As Long?
        Public Property MnemonicoTipologia As String
        Public Property DescrizioneTipologia As String
    End Class

    Public Class TamponeDiFrontiera
        Public Property Id As Integer
        Public Property CodiceFiscale As String
        Public Property DataDiNascita As Date
        Public Property Domicilio As String
        Public Property DomicilioCivico As String
        Public Property DomicilioCap As String
        Public Property DomicilioComune As String
        Public Property DomicilioProvincia As String
        Public Property Sesso As String
        Public Property CodiceMicrobiologico As String
        Public Property NomeMicrobiologico As String
        Public Property MPI As Double?
        Public Property Nome As String
        Public Property Cognome As String
        Public Property Note As String
        Public Property NumreQ As String
        Public Property LuogoDiNascita As String
        Public Property IndirizzoResidenza As String
        Public Property ResidenzaCivico As String
        Public Property ResidenzaCap As String
        Public Property ResidenzaCom As String
        Public Property ResidenzaProv As String
        Public Property Richiedente As String
        Public Property Risultato As String
        Public Property SamplingDate As Date
        Public Property SamplingDateResponse As Date?
        Public Property Ulss As String
        Public Property DTDDecesso As Date?
        Public Property DescCittadinanza As String
        Public Property Comres As String
        Public Property ComDom As String
        Public Property DoctorMpi As String
        Public Property DottoreNome As String
        Public Property DottoreCognome As String
        Public Property CfMedico As String
        Public Property Guarito As String
        Public Property DataGuarigione As Date
        Public Property Stato_ID As Integer
        Public Property Acquisito As Long?
        Public Property LogErrore As String
        Public Property PazCodice As Long
    End Class

    Public Class TamponeCertificatoNeg
        Public Property CodiceTampone As Long
        Public Property DataTampone As DateTime?
        Public Property CodiceEsito As String
    End Class

    Public Class TamponeDatiRidotti

        Public Property CodiceEpisodio As Long?
        ''' <summary>
        ''' Codice laboratorio per Episodi, ID per la tamponi_covid
        ''' </summary>
        ''' <returns></returns>
        Public Property CodiceCampione As String
        Public Property CodiceLaboratorio As String
        Public Property DescrizioneLaboratorio As String
        Public Property CodicePaziente As Long
        Public Property Esito As String
        Public Property DataTampone As DateTime?
        Public Property DataReferto As DateTime?
        Public Property CodiceUlssInserimento As String
        Public Property CodiceUlssRaccolta As String
        Public Property DescrizioneUlssInserimento As String
    End Class

End Namespace