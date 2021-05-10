Namespace Entities.Consenso

    <Serializable>
    Public Class ConsensoRegistrato

        Public Property Progressivo As Long?
        Public Property CodicePaziente As String
        Public Property DataNascitaPaziente As DateTime
        Public Property Addetto As String
        Public Property Applicativo As String
        Public Property Consenso As String
        Public Property ConsensoId As Integer
        Public Property DataEvento As DateTime
        Public Property DataRegistrazione As DateTime
        Public Property DataScadenza As DateTime
        Public Property Descrizione As String
        Public Property Ente As String
        Public Property EnteId As Integer
        Public Property FlagControllo As String
        Public Property Livello As String
        Public Property LivelloId As Integer
        Public Property StatoAnagraficoCodiceMnemonico As String
        Public Property TipoConsenso As String
        Public Property TipoEvento As String
        Public Property StatoIDIcona As String
        Public Property StatoUrlIcona As String
        Public Property StatoDescrizione As String
        Public Property Obsoleto As Boolean
        Public Property Centralizzato As Boolean
        Public Property UsaPerCalcoloStatoGlobale As Boolean
        Public Property BloccoAccessiEsterni As Boolean

        ''' <summary>
        ''' Campo personalizzato. Per il consenso APP contiene il telefono.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property CampoCustom1 As String

        ''' <summary>
        ''' Campo personalizzato. Per il consenso APP contiene il numero del documento.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property CampoCustom2 As String

        ''' <summary>
        ''' Campo personalizzato. Per il consenso APP contiene il tipo del documento.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property CampoCustom3 As String

        ''' <summary>
        ''' Ordinamento dello stato del consenso
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property StatoOrdinamento As Integer

        ''' <summary>
        ''' Ordinamento dell'anagrafica del consenso
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ConsensoOrdinamento As Integer

        ''' <summary>
        ''' Campo generico contenente sempre l'azienda centrale "050000"
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Azienda As String

        ''' <summary>
        ''' Campo contenente l'azienda locale di registrazione 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property CodiceAziendaRegistrazione As String

    End Class

End Namespace