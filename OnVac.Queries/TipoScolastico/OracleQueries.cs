using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Onit.OnAssistnet.OnVac.Queries.TipoScolastico
{
    public class OracleQueries
    {
        /// <summary>
        /// Seleziona la descrizione della usl in base al codice
        /// </summary>
        public static string selDescrizione
        {
            get
            {
                return @"select TPS_DESCRIZIONE from T_TIPO_PERSONALE_SCOLASTICO where TPS_ID = :TPS_ID";
            }
        }
    }
}
