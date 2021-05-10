using Onit.OnAssistnet.OnVac.Entities;

namespace Onit.OnAssistnet.OnVac.Log.DataLogManager
{
    public static class MovimentoCNSLogManager
    {
        public static DataLogStructure.Record GetInsertMovimentoCNSLog(Paziente pazienteModificato, Paziente pazienteOriginale)
        {
            DataLogStructure.Record recordLog = new DataLogStructure.Record();

            string codiceCNSOriginale = null;
            string codiceCNSCorrente = null;

            if (!string.IsNullOrEmpty(pazienteModificato.Paz_Cns_Codice))
            {
                codiceCNSCorrente = pazienteModificato.Paz_Cns_Codice;
            }

            if (pazienteOriginale != null)
            {
                if (!string.IsNullOrEmpty(pazienteOriginale.Paz_Cns_Codice))
                {
                    codiceCNSOriginale = pazienteOriginale.Paz_Cns_Codice;
                }

                recordLog.Campi.Add(new DataLogStructure.Campo("PAZ_CNS_CODICE", codiceCNSOriginale, codiceCNSCorrente));
            }

            return recordLog;
        }
    }
}
