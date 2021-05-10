Namespace Entities
    Public Class RicercaEpisodi

        Public Sub New()
            Me.Skip = 0
            Me.Take = 200
        End Sub

        Public CodiceUslInCarico As String
        Public CodiceUslResidenza As String
        Public StatoRilevazione As Integer?
        Public Tipologia As String
        Public DataFineIsolamentoPresunto As DateTime?
        Public DataUltimaDiaria As DateTime?
        Public SoloSintomatici As Boolean
        'se valorizzato mostra gli episodi dei contatti definiti dal paziente indice e gli episodi del paziente indice stesso.
        Public CodicePazienteIndice As Long?
        'se valorizzato a true, recupera gli episodi che hanno almeno un tampone con 'PET_FLG_DA_VISIONARE' valorizzato as S.
        Public Property TamponiDaVisionare As Boolean
        Public Property DataRefertoTampone As Date?
        'se valorizzata a true, recupera gli episodi che hanno l'ultima diaria senza risposta telefonica (PED_RISPOSTA_TELEFONO = N)
        Public Property MancataRisposta As Boolean
        'se valorizzato filtra gli episodi per i quali l'ultima diaria è stata inserita dall'app. Viene inserito l'id dell'utente associato alla prenoWeb.
        Public Property UltimaDiariaFromApp As Long?
        Public Property StatoAttivazioneApp As AttivazioneApp
        'filtro per il comune di isolamento.
        Public Property CodiceComuneIsolamento As String

        Public Property Nome As String
        Public Property Cognome As String
        Public Property DataNascita As DateTime?

        Public Skip As Integer?
        Public Take As Integer?
        Public Property Tags As Long()
        Public Property AutoPositivi As Boolean
        Public Property EscludiPazientiRSA As Boolean
        Public Property CodiceVariante As Long?

        'filtro gli episodi in base ai pazienti allo stato del consenso dell'informativa covid dei pazienti.
        'Attive => PAZ_CONFERMA_INFORMATIVA_COVID = 'S', NonAttive => PAZ_CONFERMA_INFORMATIVA_COVID = 'N', Tutte => 'Nessun filtro applicato'
        Public Enum AttivazioneApp
            Attive
            NonAttive
            Tutte
        End Enum
    End Class
End Namespace
