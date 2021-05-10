using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Onit.OnAssistnet.OnVac.DIRV.ArchiviazioneSostitutiva.Headers
{    
    [Serializable]
    public class RichiestaApplicativa
    {
        public List<Allegato> Allegati { get; set; }

        public RichiestaApplicativa()
        {
            this.Allegati = new List<Allegato>();
        }
    }
}
