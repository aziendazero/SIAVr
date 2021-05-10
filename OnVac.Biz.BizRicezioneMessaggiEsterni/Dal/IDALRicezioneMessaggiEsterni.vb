Namespace Dal


    Public Interface IDALRicezioneMessaggiEsterni
        Inherits IDisposable

        ReadOnly Property Provider() As String
        ReadOnly Property Connection() As IDbConnection
        ReadOnly Property Transaction() As IDbTransaction

        Sub BeginTransaction()

        Function GetCodicePazByCodiceAusiliario(ByVal paz_codice_ausiliario As String) As Integer
        Function GetPazienteByCodiceLocale(ByVal paz_codice As Integer) As Entities.Paziente
        Function GetPazientiByCodiceAusiliario(ByVal paz_codice_ausiliario As String) As Collection.PazienteCollection
        Function GetCodiceAslByCodiceComune(ByVal cod_comune As String) As String

        Function InsertPaziente(ByVal msg As Onit.OnAssistnet.Contracts.Messaggio, ByVal paz_regolarizzato As String, ByVal cod_cns_vacc As String, ByVal data_cns_vacc As Date, ByVal cod_cns_terr As String) As Integer
        Function UpdatePaziente(ByVal paz_codice As Integer, ByVal msg As Onit.OnAssistnet.Contracts.Messaggio, ByVal paz_regolarizzato As String, ByVal cod_cns_vacc As String, ByVal cod_cns_vacc_old As String, ByVal data_cns_vacc As Date, ByVal cod_cns_terr As String) As Integer


        Function InserimentoCicliPaziente(ByVal paz_codice As Integer, ByVal data_nascita As Date, ByVal sesso As String) As Integer
        Function InserimentoMovimentoPaziente(ByVal paz_codice As Integer, ByVal cns_codice_old As String, ByVal cns_codice_new As String) As Integer

        Function CountCicliIncompatibili(ByVal paz_codice As Integer, ByVal data_nascita As Date, ByVal sesso As String) As Integer

        Sub DeleteConvocazioni(ByVal paz_codice As Integer, ByVal del_cnv_con_appuntamento As Boolean)
        Sub UpdateCnsConvocazioniConApp(ByVal paz_codice As Integer, ByVal cod_cns As String)
        Sub UpdateCnsConvocazioniSenzaApp(ByVal paz_codice As Integer, ByVal cod_cns As String)

        Sub UpdateCodiceAusiliario(ByVal paz_codice As Integer, ByVal paz_codice_ausiliario As String)

    End Interface


End Namespace