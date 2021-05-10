'------------------------------------------------------------------------------
' <generato automaticamente>
'     Codice generato da uno strumento.
'
'     Le modifiche a questo file possono causare un comportamento non corretto e verranno perse se
'     il codice viene rigenerato. 
' </generato automaticamente>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Partial Public Class OnVac_VieSomministrazione
    
    '''<summary>
    '''Controllo Form1.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Form1 As Global.System.Web.UI.HtmlControls.HtmlForm
    
    '''<summary>
    '''Controllo OnitLayout31.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents OnitLayout31 As Global.Onit.Controls.PagesLayout.OnitLayout3
    
    '''<summary>
    '''Controllo odpVieSomministrazioneMaster.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents odpVieSomministrazioneMaster As Global.Onit.Controls.OnitDataPanel.OnitDataPanel
    
    '''<summary>
    '''Controllo OnitTable1.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents OnitTable1 As Global.Onit.Controls.OnitTable
    
    '''<summary>
    '''Controllo sezRicerca.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents sezRicerca As Global.Onit.Controls.OnitSection
    
    '''<summary>
    '''Controllo cellaRicerca.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents cellaRicerca As Global.Onit.Controls.OnitCell
    
    '''<summary>
    '''Controllo ToolBar.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ToolBar As Global.Infragistics.WebUI.UltraWebToolbar.UltraWebToolbar
    
    '''<summary>
    '''Controllo tabFiltri.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents tabFiltri As Global.Onit.Controls.OnitDataPanel.wzFilter
    
    '''<summary>
    '''Controllo Label1.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label1 As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''Controllo WzFilterKeyBase.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents WzFilterKeyBase As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo sezRisultati.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents sezRisultati As Global.Onit.Controls.OnitSection
    
    '''<summary>
    '''Controllo cellaRisultati.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents cellaRisultati As Global.Onit.Controls.OnitCell
    
    '''<summary>
    '''Controllo dgrVieSomministrazione.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents dgrVieSomministrazione As Global.Onit.Controls.OnitDataPanel.wzDataGrid
    
    '''<summary>
    '''Controllo sezDettaglio.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents sezDettaglio As Global.Onit.Controls.OnitSection
    
    '''<summary>
    '''Controllo cellaDettaglio.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents cellaDettaglio As Global.Onit.Controls.OnitCell
    
    '''<summary>
    '''Controllo odpVieSomministrazioneDetail.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents odpVieSomministrazioneDetail As Global.Onit.Controls.OnitDataPanel.OnitDataPanel
    
    '''<summary>
    '''Controllo txtCodice.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents txtCodice As Global.Onit.Controls.OnitDataPanel.wzTextBox
    
    '''<summary>
    '''Controllo txtCodiceEsterno.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents txtCodiceEsterno As Global.Onit.Controls.OnitDataPanel.wzTextBox
    
    '''<summary>
    '''Controllo lblCodiceFSE.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents lblCodiceFSE As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''Controllo txtCodiceFse.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents txtCodiceFse As Global.Onit.Controls.OnitDataPanel.wzTextBox
    
    '''<summary>
    '''Controllo txtDescrizione.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents txtDescrizione As Global.Onit.Controls.OnitDataPanel.wzTextBox
    
    '''<summary>
    '''Controllo txtAvn.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents txtAvn As Global.Onit.Controls.OnitDataPanel.wzTextBox
End Class
