Public Interface IMovimentiInterniCNSProvider

    Function InserimentoMovimentoPaziente(codicePaziente As Integer, codiceConsultorioOld As String, codiceConsultorioNew As String, dataAssegnazione As DateTime, flagInvioCartella As Boolean, flagPresaVisione As Boolean, flagAutoAdulti As Boolean) As Integer

    Function UpdatePresaVisione(progressivoMovimento As Integer, presaVisione As String) As Integer
    Function UpdateInvioCartella(progressivoMovimento As Integer, invioCartella As Boolean) As Integer
    Function GetDtSmistamenti(smistamentiFilter As AppoggiatiRASMIFiltriRicerca, pagingOptions As MovimentiInterniCNSPagingOptions?, codUslCorrente As String, codCnsDefault As String) As DataTable
    Function CountSmistamenti(smistamentiFilter As AppoggiatiRASMIFiltriRicerca, codUslCorrente As String, codCnsDefault As String) As Integer
End Interface


<Serializable()>
Public Structure MovimentiInterniCNSPagingOptions
    Public StartRecordIndex As Int32
    Public EndRecordIndex As Int32
End Structure


<Serializable()>
Public Class AppoggiatiRASMIFiltriRicerca
    Public DataNascitaDa As DateTime?
    Public DataNascitaA As DateTime?
    Public DataCnsDa As DateTime?
    Public DataCnsA As DateTime?
End Class
