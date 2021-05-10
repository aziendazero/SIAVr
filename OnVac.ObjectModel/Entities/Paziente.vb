Imports System.Runtime.Serialization

Namespace Entities

    <Serializable()>
    <DataContract()>
    Public Class Paziente

#Region " Properties "

        <DataMember()>
        Public Property Paz_Codice As Integer

        <DataMember()>
        Public Property PAZ_CODICE_REGIONALE As String

        <DataMember()>
        Public Property CodiceAusiliario As String

        <DataMember()>
        Public Property PAZ_COGNOME As String

        <DataMember()>
        Public Property PAZ_NOME As String

        <DataMember()>
        Public Property Sesso As String

        <DataMember()>
        Public Property PAZ_CODICE_FISCALE As String

        <DataMember()>
        Public Property Tessera As String

        <DataMember()>
        Public Property Paz_Cns_Codice As String

        <DataMember()>
        Public Property Paz_Cns_Codice_Old As String

        <DataMember()>
        Public Property Paz_Cns_Data_Assegnazione As DateTime?

        <DataMember()>
        Public Property Paz_Cns_Terr_Codice As String

        <DataMember()>
        Public Property Data_Nascita As Date

        <DataMember()>
        Public Property ComuneNascita_Codice As String

        <DataMember()>
        Public Property Cittadinanza_Codice As String

        <DataMember()>
        Public Property Cittadinanza_Descrizione As String

        <DataMember()>
        Public Property Circoscrizione_Codice As String

        <DataMember()>
        Public Property Circoscrizione2_Codice As String

        <DataMember()>
        Public Property Distretto_Codice As String

        <DataMember()>
        Public Property StatoAnagrafico As Enumerators.StatoAnagrafico?

        <DataMember()>
        Public Property IndirizzoResidenza As String

        <DataMember()>
        Public Property ComuneResidenza_Codice As String

        <DataMember()>
        Public Property ComuneResidenza_Descrizione As String

        <DataMember()>
        Public Property ComuneResidenza_Provincia As String

        <DataMember()>
        Public Property ComuneResidenza_Cap As String

        <DataMember()>
        Public Property ComuneResidenza_DataInizio As DateTime

        <DataMember()>
        Public Property ComuneResidenza_DataFine As DateTime

        <DataMember()>
        Public Property IndirizzoDomicilio As String

        <DataMember()>
        Public Property ComuneDomicilio_Codice As String

        <DataMember()>
        Public Property ComuneDomicilio_Descrizione As String

        <DataMember()>
        Public Property ComuneDomicilio_Provincia As String

        <DataMember()>
        Public Property ComuneDomicilio_Cap As String

        <DataMember()>
        Public Property ComuneDomicilio_DataInizio As DateTime

        <DataMember()>
        Public Property ComuneDomicilio_DataFine As DateTime

        <DataMember()>
        Public Property ComuneEmigrazione_Codice As String

        <DataMember()>
        Public Property Telefono1 As String

        <DataMember()>
        Public Property Telefono2 As String

        <DataMember()>
        Public Property Telefono3 As String

        <DataMember()>
        Public Property Email As String

        <DataMember()>
        Public Property MedicoBase_Codice As String

        <DataMember()>
        Public Property MedicoBase_Descrizione As String

        <DataMember()>
        Public Property MedicoBase_DataDecorrenza As DateTime

        <DataMember()>
        Public Property MedicoBase_DataScadenza As DateTime

        <DataMember()>
        Public Property MedicoBase_DataRevoca As DateTime

        <DataMember()>
        Public Property UslResidenza_Codice As String

        <DataMember()>
        Public Property UslAssistenzaPrecedente_Codice As String

        <DataMember()>
        Public Property UslAssistenza_Codice As String

        <DataMember()>
        Public Property UslAssistenza_DataInizio As DateTime

        <DataMember()>
        Public Property UslAssistenza_DataCessazione As DateTime

        <DataMember()>
        Public Property DataEmigrazione As DateTime

        <DataMember()>
        Public Property ComuneProvenienza_Codice As String

        <DataMember()>
        Public Property DataImmigrazione As DateTime

        <DataMember()>
        Public Property DataDecesso As DateTime

        <DataMember()>
        Public Property Stato As Enumerators.StatiVaccinali?

        <DataMember()>
        Public Property DataAire As DateTime?

        <DataMember()>
        Public Property FlagCessato As String

        <DataMember()>
        Public Property FlagAire As String

        <DataMember()>
        Public Property FlagRegolarizzato As String

        <DataMember()>
        Public Property FlagLocale As String

        <DataMember()>
        Public Property FlagIrreperibilita As String

        <DataMember()>
        Public Property DataIrreperibilita As DateTime?

        <DataMember()>
        Public Property DataInserimento As DateTime

        <DataMember()>
        Public Property DataAggiornamentoDaAnagrafe As DateTime?

        <DataMember()>
        Public Property FlagAnonimo As String

        <DataMember()>
        Public Property CodiceAzienda As String

        <DataMember()>
        Public Property CodiceCategoria1 As String

        <DataMember()>
        Public Property CodiceCategoria2 As String

        <DataMember()>
        Public Property FlagCancellato As String

        <DataMember()>
        Public Property CodiceDemografico As String

        <DataMember()>
        Public Property FlagCompletare As String

        <DataMember()>
        Public Property DataAggiornamento As DateTime

        <DataMember()>
        Public Property DataAggiornamentoDaComune As DateTime

        <DataMember()>
        Public Property DataCancellazione As DateTime

        <DataMember()>
        Public Property FlagStampaMaggiorenni As String

        <DataMember()>
        Public Property Giorno As String

        <DataMember()>
        Public Property CodiceIndirizzoResidenza As Integer?

        <DataMember()>
        Public Property CodiceIndirizzoDomicilio As Integer?

        <DataMember()>
        Public Property Padre As String

        <DataMember()>
        Public Property Madre As String

        <DataMember()>
        Public Property FlagOccasionale As String

        <DataMember()>
        Public Property IdElaborazione As Integer?

        <DataMember()>
        Public Property FlagPosizioneVaccinale As String

        <DataMember()>
        Public Property RegAssistenza As String

        <DataMember()>
        Public Property FlagRichiestaCartella As String

        <DataMember()>
        Public Property FlagRichiestaCertificato As String

        <DataMember()>
        Public Property CodiceCategoriaRischio As String

        <DataMember()>
        Public Property StatoAcquisizioneDatiVaccinaliCentrale As Enumerators.StatoAcquisizioneDatiVaccinaliCentrale?

        <DataMember()>
        Public Property StatoAcquisizioneImmigrazione As String

        <DataMember()>
        Public Property StatoAnagraficoDettaglio As String

        <DataMember()>
        Public Property StatoNotificaEmigrazione As String

        <DataMember()>
        Public Property FlagStampaCertificatoEmigrazione As String

        <DataMember()>
        Public Property Tipo As String

        <DataMember()>
        Public Property TipoOccasionalita As String

        <DataMember()>
        Public Property LivelloCertificazione As Integer

        <DataMember()>
        Public Property ComuneNascita_Descrizione As String

        <DataMember()>
        Public Property CodiceEsterno As String

        <DataMember()>
        Public Property DataScadenzaSSN As DateTime?

        <DataMember()>
        Public Property Malattie As dsMalattie.MalattieDataTable

        <DataMember()>
        Public Property IdACN As String

        <DataMember()>
        Public Property CategoriaCittadino As String

        <DataMember()>
        Public Property MotivoCessazioneAssistenza As String

#End Region

#Region " .ctor "

        Sub New()
        End Sub

        Sub New(codicePaziente As Integer, codiceConsultorio As String)
            Me.Paz_Codice = codicePaziente
            Me.Paz_Cns_Codice = codiceConsultorio
        End Sub

        Sub New(codicePaziente As Integer)
            Me.Paz_Codice = codicePaziente
        End Sub

#End Region

#Region " Public "

        ''' <summary>
        ''' Restituisce un datatable con codice, cognome, nome, data di nascita e consultorio vaccinale del paziente
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function getDataTable() As DataTable

            Dim dt As New DataTable()

            dt.Columns.Add("PAZ_CODICE", Type.GetType("System.Int32"))
            dt.Columns.Add("PAZ_COGNOME", Type.GetType("System.String"))
            dt.Columns.Add("PAZ_NOME", Type.GetType("System.String"))
            dt.Columns.Add("PAZ_CNS_CODICE", Type.GetType("System.String"))
            dt.Columns.Add("PAZ_DATA_NASCITA", Type.GetType("System.DateTime"))

            Dim row As DataRow = dt.NewRow()
            row("PAZ_CODICE") = Me.Paz_Codice
            row("PAZ_COGNOME") = Me.PAZ_COGNOME
            row("PAZ_NOME") = Me.PAZ_NOME
            row("PAZ_CNS_CODICE") = Me.Paz_Cns_Codice
            row("PAZ_DATA_NASCITA") = Me.Data_Nascita

            dt.Rows.Add(row)

            Return dt

        End Function

        Public Function addRow2DT(dt As DataTable) As DataTable

            Dim row As DataRow = dt.NewRow()

            If Not dt.Columns.Item("PAZ_CODICE") Is Nothing Then
                row("PAZ_CODICE") = Me.Paz_Codice
            End If
            If Not dt.Columns.Item("PAZ_COGNOME") Is Nothing Then
                row("PAZ_COGNOME") = Me.PAZ_COGNOME
            End If
            If Not dt.Columns.Item("PAZ_NOME") Is Nothing Then
                row("PAZ_NOME") = Me.PAZ_NOME
            End If
            If Not dt.Columns.Item("PAZ_CNS_CODICE") Is Nothing Then
                row("PAZ_CNS_CODICE") = Me.Paz_Cns_Codice
            End If
            If Not dt.Columns.Item("PAZ_DATA_NASCITA") Is Nothing Then
                row("PAZ_DATA_NASCITA") = Me.Data_Nascita
            End If

            dt.Rows.Add(row)

            Return dt

        End Function

#End Region

#Region " Overrides "

        Public Overrides Function toString() As String

            Dim str As String

            str = "CODICE PAZIENTE: " & Paz_Codice & ", "
            str &= "COGNOME: " & PAZ_COGNOME & ", "
            str &= "NOME: " & PAZ_NOME & ", "
            str &= "CENTRO VACCINALE: " & Paz_Cns_Codice & ", "
            str &= "DATA DI NASCITA: " & Data_Nascita & ""

            Return str

        End Function

#End Region

    End Class

End Namespace
