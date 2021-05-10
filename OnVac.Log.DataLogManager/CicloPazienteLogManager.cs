using System.Collections.Generic;


namespace Onit.OnAssistnet.OnVac.Log.DataLogManager
{
    public static class CicloPazienteLogManager
    {
        public static DataLogStructure.Record GetInsertCicliPazienteLog(ICollection<string> codiceCicli)
        {
            DataLogStructure.Record recordLog = new DataLogStructure.Record();

            if (codiceCicli != null)
            {
                foreach (string codiceCiclo in codiceCicli)
                {
                    recordLog.Campi.Add(new DataLogStructure.Campo("PAC_CIC_CODICE", "", codiceCiclo));
                }
            }

            return recordLog;
        }
    }
}
