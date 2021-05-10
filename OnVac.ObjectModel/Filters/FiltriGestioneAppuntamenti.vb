Imports System.Collections.Generic

Namespace Filters

    <Serializable()>
    Public Class FiltriGestioneAppuntamenti

        ' Appuntamenti
        Public ConsultoriSelezionati As String
        Public malattia As String
        Public chkObb As Boolean
        Public chkFac As Boolean
        Public chkRac As Boolean
        Public chkNonExtracomPrima As Boolean
        Public chkExtracom As Boolean
        Public chkCronici As Boolean
        Public chkSoloRitardatari As Boolean
        Public DaDataNascita As Date
        Public ADataNascita As Date
        Public chkDataSingola As Boolean
        Public FinoAData As Date
        Public fmMedCodice As String
        Public categ_rischio As String
        Public sesso As String
        Public StatiAnagrafici As List(Of String)
        Public cnsCodice As String
        Public codiceComune As String
        Public vaccinazioniTutteEscluse As Boolean

        Public dtFiltroAssociazioniSel As DataTable
        Public dtFiltroDosiSel As DataTable

        Public dtFiltroCicliSel As DataTable
        Public dtFiltroSeduteSel As DataTable

        ' Opzioni
        Public chkRicConvocazioni As Boolean
        Public chkRicCiclo As Boolean
        Public chkRicMedico As Boolean
        Public chkRicBilancio As Boolean
        Public chkOrdineAlfabeticoRicerca As Boolean

        Public txtNumPazientiAlGiorno As String
        Public txtNumNuoviPazientiAlGiorno As String
        Public txtDurata As String
        Public chkOrdineAlfabeticoPrenotazione As Boolean

        ' Prenotazione
        Public odpDataInizPrenotazioni As Date
        Public odpDataFinePrenotazioni As Date
        Public chkOrariPers As Boolean
        Public chkSovrapponiRit As Boolean
        Public chkFiltroPomeriggioObbligatorio As Boolean

        Public SetFiltriMaschera As Boolean

        Public Sub New()
            StatiAnagrafici = New List(Of String)
        End Sub

    End Class

End Namespace
