Namespace Filters

    Public Class FiltriRicercaPaziente

        Public Codice As Integer
        Public CodiceAusiliario As String
        Public CodiceRegionale As String
        Public Nome As String
        Public Cognome As String
        Public DataNascita_Da As Date
        Public DataNascita_A As Date
        Public CodiceComuneNascita As String
        Public Sesso As String
        Public CodiceFiscale As String
        Public Tessera As String
        Public Consultorio As String
        Public Malattia As String
        Public CategoriaRischio As String
        Public StatoAnagrafico As String()


        Public Sub New()
            Codice = -1
            CodiceAusiliario = String.Empty
            CodiceRegionale = String.Empty
            Nome = String.Empty
            Cognome = String.Empty
            DataNascita_Da = Date.MinValue
            DataNascita_A = Date.MaxValue
            CodiceComuneNascita = String.Empty
            Sesso = String.Empty
            CodiceFiscale = String.Empty
            Tessera = String.Empty
            Consultorio = String.Empty
            Malattia = String.Empty
            CategoriaRischio = String.Empty
            StatoAnagrafico = New String() {}
        End Sub


        Public Function IsNullValue(ByVal filtro As Integer) As Boolean
            Return (filtro = -1)
        End Function


        Public Function IsNullValue(ByVal filtro As Date) As Boolean
            Return (filtro = Date.MaxValue Or filtro = Date.MinValue)
        End Function


        Public Function IsNullValue(ByVal filtro As String) As Boolean
            Return (filtro = String.Empty)
        End Function


        Public Function IsNullValue(ByVal filtro As String()) As Boolean
            Return (filtro.Length = 0)
        End Function

        ''' <summary>
        ''' Restituisce true se nessun filtro è valorizzato
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsEmpty() As Boolean

            If Not IsNullValue(Me.Codice) Then Return False
            If Not String.IsNullOrEmpty(Me.CodiceAusiliario) Then Return False
            If Not String.IsNullOrEmpty(Me.CodiceRegionale) Then Return False
            If Not String.IsNullOrEmpty(Me.Nome) Then Return False
            If Not String.IsNullOrEmpty(Me.Cognome) Then Return False
            If Not IsNullValue(Me.DataNascita_Da) Then Return False
            If Not IsNullValue(Me.DataNascita_A) Then Return False
            If Not String.IsNullOrEmpty(Me.CodiceComuneNascita) Then Return False
            If Not String.IsNullOrEmpty(Me.Sesso) Then Return False
            If Not String.IsNullOrEmpty(Me.CodiceFiscale) Then Return False
            If Not String.IsNullOrEmpty(Me.Tessera) Then Return False
            If Not String.IsNullOrEmpty(Me.Consultorio) Then Return False
            If Not String.IsNullOrEmpty(Me.Malattia) Then Return False
            If Not String.IsNullOrEmpty(Me.CategoriaRischio) Then Return False
            If Not IsNullValue(Me.StatoAnagrafico) Then Return False

            Return True
        End Function

    End Class

End Namespace
