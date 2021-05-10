using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Onit.OnAssistnet.OnVac.DIRV.ArchiviazioneSostitutiva.Headers
{
    /// <summary>
    /// La classe Intestazione rappresenta l'XML_MESSAGE da inviare al servizio
    /// </summary>
    [Serializable]
    public class Intestazione
    {
        /// <summary>
        /// Identificativo univoco del messaggio. 
        /// Non obbligatorio, se non impostato lo valorizza il servizio di archiviazione
        /// </summary>
        public string IdMessaggio { get; set; }
        
        /// <summary>
        /// Timestamp del messaggio. 
        /// Non obbligatorio, se non impostato lo valorizza il servizio di archiviazione
        /// </summary>
        public string TmstMessaggio { get; set; }
        
        /// <summary>
        /// Richiedente del servizio. Obbligatorio.
        /// </summary>
        public string Mittente { get; set; }
        
        /// <summary>
        /// Nome del servizio. Non è obbligatorio per i servizi specifici (come il servizio di archiviazione)
        /// </summary>
        public string Servizio { get; set; }
        
        /// <summary>
        /// Destinatario del servizio. Non obbligatorio
        /// </summary>
        public string Destinatario { get; set; }
        
        /// <summary>
        /// Id del richiedente, per i servizi che richiedono autenticazione tramite password.
        /// </summary>
        public string UserId { get; set; }
        
        /// <summary>
        /// Password del richiedente, per i servizi che richiedono autenticazione tramite password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Oggetto contenente allegati di richiesta.
        /// </summary>
        public RichiestaApplicativa RichiestaApplicativa { get; set; }
        
        /// <summary>
        /// Oggetto contenente allegati di risposta.
        /// </summary>
        public RispostaApplicativa RispostaApplicativa { get; set; }

        public Intestazione()
        {
            this.RichiestaApplicativa = new RichiestaApplicativa();
        }
    }
}
