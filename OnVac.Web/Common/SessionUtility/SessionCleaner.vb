Imports Onit.OnAssistnet.OnVac.Log.DataLogStructure

Public Class SessionCleaner


    Private Const ARGOMENTOLOG As String = "SESSIONCLEANER"
    Private _paginaConViewStateInSession As Boolean

    Private _settings As Onit.OnAssistnet.OnVac.Settings.Settings


    Public Sub Start()

        ' --- Pulizia della Session --- '
        If _settings.SESSIONCLEANER Then
            Dim i As Integer
            Dim key As String
            Dim keysToRemove As New ArrayList

            Try
                Dim keys As IEnumerator = HttpContext.Current.Session.Keys.GetEnumerator
                While (keys.MoveNext)

                    key = keys.Current.ToString
                    Dim sessObj As Object = HttpContext.Current.Session(key)
                    If Not sessObj Is Nothing AndAlso sessObj.GetType.ToString = "Onit.Controls.OnitDataPanel.GeneralStoreManager" Then
                        keysToRemove.Add(key)
                    End If

                End While

                For i = 0 To keysToRemove.Count - 1
                    key = keysToRemove(i).ToString
                    If key <> "GsmngodpRicercaPaziente" Then
                        HttpContext.Current.Session.Remove(key)
                    End If
                Next

                If Not _paginaConViewStateInSession Then
                    If Not HttpContext.Current.Session("ViewState") Is Nothing Then
                        HttpContext.Current.Session.Remove("ViewState")
                    End If
                End If

                Dim arr() As String = {"OnVac_dt_lotti",
"OnVac_tb_par",
"OnVac_staIndisp",
"OnVac_dtaNAppuntamenti",
"OnVac_dtaCicliEliminati",
"OnVac_dtaMantoux",
"OnVac_dtaCicli",
"OnVac_dtaCampiLiberi",
"OnVac_dtaCampiLiberi2",
"OnVac_OrariPersonalizzati",
"OnVac_OrariPersonalizzati_Last",
"OnVac_GestioneApp_dsLog",
"OnVac_GestioneApp_dtsRicerca",
"OnVac_dtaRicerca",
"OnVac_dt_vacEseguite",
"OnVac_dtaMattino",
"pazientiScadutiCollection",
"pazientiDaSollecitatiCollection",
"OnVac_dt_vacEff",
"OnVac_p_dtaOrariValidi",
"OnVac_dt_LottiVac",
"OnVac_p_dtaFestivita",
"OnVac_dt_associazioni",
"OnVac_dt_vacProg",
"dtJobsOnVac",
"DatiSanitariPazienti_dtaMalattie",
"DatiSanitariPazienti_dtaCicli",
"DatiSanitariPazienti_dtaMantoux",
"DatiSanitariPazienti_dtaCicliEliminati",
"DatiSanitariPazientiLogTestata_tCicliAggiuntiAutomatico",
"DatiSanitariPazientiLogTestata_tCicliAggiunti",
"DatiSanitariPazientiLogTestata_tCicliEliminati",
"DatiSanitariPazientiLogTestata_tMalattieAggiunte",
"DatiSanitariPazientiLogTestata_tMalattieEliminate",
"DatiSanitariPazientiLogTestata_tMantouxAggiunte",
"DatiSanitariPazientiLogTestata_tMantouxEliminate",
"DatiSanitariPazientiLogTestata_tMantouxModificate",
"Solleciti"}

                For i = 0 To arr.Length - 1
                    If Not HttpContext.Current.Session(arr(i)) Is Nothing Then
                        HttpContext.Current.Session.Remove(arr(i))
                    End If
                Next
                
            Catch ex As Exception
                ' Se il SessionCleaner va in errore, non deve bloccare l'applicativo
            End Try

        End If

        ' --- Scrittura del log --- '
        TraceSession.TraceSession(HttpContext.Current.Session)
            
    End Sub


    Public Sub New(ByRef settings As Settings.Settings)
        _paginaConViewStateInSession = False
        _settings = settings
    End Sub


    Public Sub New(ByRef settings As Settings.Settings, ByVal paginaConViewStateInSession As Boolean)
        _paginaConViewStateInSession = paginaConViewStateInSession
        _settings = settings
    End Sub


End Class
