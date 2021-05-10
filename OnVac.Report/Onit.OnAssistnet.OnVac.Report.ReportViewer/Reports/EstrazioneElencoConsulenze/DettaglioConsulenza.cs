using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Onit.OnAssistnet.OnVac.Report.ReportViewer.Reports.EstrazioneElencoConsulenze
{
    [Serializable]
    public class DettaglioConsulenza
    {
        public string Cognome { get; set; }
        public string Nome { get; set; }
        public DateTime? DataNascita { get; set; }
        public string CodiceFiscale { get; set; }
        public string ComuneResisdenza { get; set; }
        public string CodCNS { get; set; }
        public string DescrCNS { get; set; }
        public string DescrTipoConsulenza { get; set; }
        public DateTime? DataConsulenza { get; set; }
        public Int32? DurataConsulenza { get; set; }
        public string OperatoreDescr { get; set; }
        public string Note { get; set; }
        
    }
}
