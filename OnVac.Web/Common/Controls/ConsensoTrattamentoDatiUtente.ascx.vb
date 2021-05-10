Public Class ConsensoTrattamentoDatiUtente
    Inherits Common.UserControlPageBase

#Region " Events "

    Public Event ConsensoTrattamentoDatiUtenteAccettato()
    Public Event ConsensoTrattamentoDatiUtenteRifiutato()

#End Region

#Region " Types "

    Public Enum DestinazioneRedirect
        NessunaSelezione = 0
        DettaglioPaziente = 1
        ConvocazioniPaziente = 2
        VacEseguitePS = 3
        VacEseguiteCentrale = 4
    End Enum

#End Region

#Region " Properties "

    Public Property CodicePaziente As Long
        Get
            If ViewState("CodPaz") Is Nothing Then ViewState("CodPaz") = 0
            Return Convert.ToInt64(ViewState("CodPaz"))
        End Get
        Set(value As Long)
            ViewState("CodPaz") = value
        End Set
    End Property

    Public Property CodiceAusiliarioPaziente As String
        Get
            If ViewState("CodAux") Is Nothing Then ViewState("CodAux") = String.Empty
            Return ViewState("CodAux").ToString()
        End Get
        Set(value As String)
            ViewState("CodAux") = value
        End Set
    End Property

    Public Property CodiceConsultorio As String
        Get
            If ViewState("CodCns") Is Nothing Then ViewState("CodCns") = String.Empty
            Return ViewState("CodCns").ToString()
        End Get
        Set(value As String)
            ViewState("CodCns") = value
        End Set
    End Property

    Public Property Destinazione As DestinazioneRedirect
        Get
            If ViewState("Redir") Is Nothing Then ViewState("Redir") = DestinazioneRedirect.NessunaSelezione
            Return ViewState("Redir")
        End Get
        Set(value As DestinazioneRedirect)
            ViewState("Redir") = value
        End Set
    End Property

    Public Property CodiceAmbulatorio As Integer
        Get
            If ViewState("CodAmb") Is Nothing Then ViewState("CodAmb") = 0
            Return Convert.ToInt32(ViewState("CodAmb"))
        End Get
        Set(value As Integer)
            ViewState("CodAmb") = value
        End Set
    End Property

#End Region

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

    End Sub

    Private Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click

        InserimentoConsensoUtente(True)

        RaiseEvent ConsensoTrattamentoDatiUtenteAccettato()

    End Sub

    Private Sub btnAnnulla_Click(sender As Object, e As EventArgs) Handles btnAnnulla.Click

        InserimentoConsensoUtente(False)

        RaiseEvent ConsensoTrattamentoDatiUtenteRifiutato()

    End Sub

    Private Sub InserimentoConsensoUtente(valore As Boolean)

        If CodicePaziente = 0 AndAlso String.IsNullOrWhiteSpace(CodiceAusiliarioPaziente) Then
            Throw New Exception("Consenso Utente - Parametri non impostati: codice locale e ausiliario paziente")
        End If

        If String.IsNullOrWhiteSpace(CodiceConsultorio) Then
            Throw New Exception("Consenso Utente - Parametro non impostato: codice centro vaccinale")
        End If

        Using genericProvider As New DAL.DbGenericProvider(OnVacContext.Connection)

            Using bizUtenti As New Biz.BizUtenti(genericProvider, Settings, OnVacContext.CreateBizContextInfos(CodiceConsultorio))

                If CodicePaziente > 0 Then
                    bizUtenti.InserimentoConsensoByCodiceLocale(Convert.ToInt64(CodicePaziente), valore)
                Else
                    bizUtenti.InserimentoConsensoByCodiceAusiliario(CodiceAusiliarioPaziente, valore)
                End If

            End Using

        End Using

    End Sub

    Public Sub ClearParameters()
        CodicePaziente = 0
        CodiceConsultorio = String.Empty
        CodiceAmbulatorio = 0
    End Sub

End Class