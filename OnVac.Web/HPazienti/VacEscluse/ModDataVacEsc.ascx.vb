Imports System.Collections
Imports System.Collections.Generic

Imports Onit.Database.DataAccessManager
Imports Onit.Controls
Imports Onit.Web.UI.WebControls.Validators

Public Class ModDataVacEsc
    Inherits Common.UserControlFinestraModalePageBase

    Public Event Conferma(dataScadenza As Date)
    Public Event RiabilitaLayout()

    'scatena l'evento per riabilitare il layout
    Protected Sub OnRiabilitaLayout()
        RaiseEvent RiabilitaLayout()
    End Sub

    Protected Sub OnConferma(dataScadenzaModificata As Date)
        RaiseEvent Conferma(dataScadenzaModificata)
    End Sub

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(sender As System.Object, e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

    Private Sub ToolBar_ModDataVacEsc_ButtonClicked(sender As Object, e As Infragistics.WebUI.UltraWebToolbar.ButtonEvent) Handles ToolBar_ModDataVacEsc.ButtonClicked

        Select Case e.Button.Key

            ' N.B. : in realtà, questo non viene richiamato mai perchè la modale viene chiusa lato client
            Case "btn_Annulla"

                OnRiabilitaLayout()

            Case "btn_Salva"

                OnConferma(dpkDataScadenzaModificata.Data)

                dpkDataScadenzaModificata.Text = String.Empty

        End Select

    End Sub

End Class