Namespace Filters

    Public Class FiltriRiconduzionePazienti

        ''' <summary>
        ''' Flag che indica se la ricerca dei pazienti da ricondurre deve includere il filtro sul codice ausiliario nullo.
        ''' </summary>
        ''' <remarks></remarks>
        Public FlagCodiceAusiliarioNullo As Boolean

        Public Cognome As String
        Public Nome As String
        Public CodiceFiscale As String
        Public Tessera As String
        Public DataNascita As DateTime?
        Public CodiceComuneNascita As String
        Public Sesso As String
        Public CodiceComuneResidenza As String
        Public CodiceComuneDomicilio As String
        Public CodiceCittadinanza As String

        Public Sub New()

            Me.FlagCodiceAusiliarioNullo = True

            Me.Cognome = Nothing
            Me.Nome = Nothing
            Me.CodiceFiscale = Nothing
            Me.Tessera = Nothing
            Me.DataNascita = Nothing
            Me.CodiceComuneNascita = Nothing
            Me.Sesso = Nothing
            Me.CodiceComuneResidenza = Nothing
            Me.CodiceComuneDomicilio = Nothing
            Me.CodiceCittadinanza = Nothing

        End Sub

    End Class

End Namespace
