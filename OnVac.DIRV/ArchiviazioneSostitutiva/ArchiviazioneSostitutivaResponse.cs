using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Onit.Shared.Soap.SWA;
using Onit.OnAssistnet.OnVac.DIRV.ArchiviazioneSostitutiva.MetadatiRisposta;


namespace Onit.OnAssistnet.OnVac.DIRV.ArchiviazioneSostitutiva
{
    public class ArchiviazioneSostitutivaResponse
    {
        public SWAResponse SwaResponse { get; set; }
        public risposta_archiviazione_conservazione MetadatiRisposta { get; set; }
    }

    public static class CodiceErroreRispostaServizio
    {
        public const string NESSUN_ERRORE = "00";
        public const string ERRORE_INVIO = "-99";
        public const string ECCEZIONE = "-98";
    }
}
