using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onit.OnAssistnet.OnVac.DIRV.ArchiviazioneSostitutiva.Headers;

namespace Onit.OnAssistnet.OnVac.DIRV.ArchiviazioneSostitutiva
{
    public class ArchiviazioneSostitutivaMessage
    {
        public Intestazione Intestazione { get; set; }
        public archiviazione_conservazione Metadati { get; set; }
        public List<DocumentoFirmato> Documenti { get; set; }
        
        public ArchiviazioneSostitutivaMessage()
        {
            this.Intestazione = new Intestazione();
            this.Metadati = new archiviazione_conservazione();
            this.Documenti = new List<DocumentoFirmato>();
        }
    }
}
