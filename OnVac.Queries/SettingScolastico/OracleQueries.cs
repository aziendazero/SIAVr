using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Onit.OnAssistnet.OnVac.Queries.SettingScolastico
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
                return @"select SPS_DESCRIZIONE from T_TIPO_SETTING_P_SCOLASTICO where SPS_ID = :SPS_ID";
            }
        }
    }
}
