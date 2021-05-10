using System;
using System.Collections.Generic;
using System.Linq;

using Onit.OnAssistnet.OnVac.Entities;
using Onit.OnAssistnet.OnVac.Log.DataLogStructure;


namespace Onit.OnAssistnet.OnVac.Log.DataLogManager
{
    public static class ConvocazioneLogManager
    {
        public static IEnumerable<DataLogStructure.Record> GetUpdateConvocazioniLog(Paziente pazienteModificato, ICollection<Convocazione> convocazioniModificate, ICollection<Convocazione> convocazioniOriginali)
        {
            List<DataLogStructure.Record> recordLogList = new List<DataLogStructure.Record>();

            if (convocazioniModificate != null)
            {
                   foreach (Convocazione convocazioneModificata in convocazioniModificate) 
                   {
                       DataLogStructure.Record recordLog = new DataLogStructure.Record();

                        DateTime dataConvocazioneModificata = convocazioneModificata.Data_CNV;
                        Convocazione convocazioneOriginale = convocazioniOriginali.First(c => c.Data_CNV == dataConvocazioneModificata);

                        if (convocazioneOriginale.Cns_Codice != convocazioneModificata.Cns_Codice)
                        {
                            recordLog.Campi.Add(new DataLogStructure.Campo("CNV_CNS_CODICE", convocazioneOriginale.Cns_Codice, convocazioneModificata.Cns_Codice));
                        }

                        if (convocazioneOriginale.DataAppuntamento != convocazioneModificata.DataAppuntamento)
                        {
                            recordLog.Campi.Add(new DataLogStructure.Campo("CNV_DATA_APPUNTAMENTO", (convocazioneOriginale.DataAppuntamento == DateTime.MinValue ? string.Empty : convocazioneOriginale.DataAppuntamento.ToString()), (convocazioneModificata.DataAppuntamento == DateTime.MinValue ? string.Empty : convocazioneModificata.DataAppuntamento.ToString())));
                        }

                        if (convocazioneOriginale.TipoAppuntamento != convocazioneModificata.TipoAppuntamento)
                        {
                            recordLog.Campi.Add(new DataLogStructure.Campo("CNV_TIPO_APPUNTAMENTO", convocazioneOriginale.TipoAppuntamento, convocazioneModificata.TipoAppuntamento));
                        }

                        if (convocazioneOriginale.Durata_Appuntamento != convocazioneModificata.Durata_Appuntamento)
                        {
                            recordLog.Campi.Add(new DataLogStructure.Campo("CNV_DURATA_APPUNTAMENTO", (convocazioneOriginale.Durata_Appuntamento == int.MinValue ? string.Empty : convocazioneOriginale.Durata_Appuntamento.ToString()), (convocazioneModificata.Durata_Appuntamento == int.MinValue ? string.Empty : convocazioneModificata.Durata_Appuntamento.ToString())));
                        }

                        if (convocazioneOriginale.DataPrimoAppuntamento != convocazioneModificata.DataPrimoAppuntamento)
                        {
                            recordLog.Campi.Add(new DataLogStructure.Campo("CNV_PRIMO_APPUNTAMENTO", (convocazioneOriginale.DataPrimoAppuntamento == DateTime.MinValue ? string.Empty : convocazioneOriginale.DataPrimoAppuntamento.ToString()), (convocazioneModificata.DataPrimoAppuntamento == DateTime.MinValue ? string.Empty : convocazioneModificata.DataPrimoAppuntamento.ToString())));
                        }

                        recordLogList.Add(recordLog);
                }
            }

            return recordLogList;
        }

        public static IEnumerable<DataLogStructure.Record> GetDeleteConvocazioniLog(Paziente pazienteModificato, ICollection<Convocazione> convocazioniEliminate)
        {
            List<DataLogStructure.Record> recordLogList = new List<DataLogStructure.Record>();

            if (convocazioniEliminate != null) 
            {
                // todo: MID => LOG => T_BIL_PROGRAMMATI / T_VAC_PROGRAMMATE / T_PAZ_RITARDI / T_CNV_CICLI ?!?!

                foreach (Convocazione convocazioneEliminata in convocazioniEliminate)
                {
                    DataLogStructure.Record recordLog = new DataLogStructure.Record();
                    recordLog.Campi.Add(new Campo("CNV_PAZ_CODICE", convocazioneEliminata.Paz_codice.ToString()));
                    recordLog.Campi.Add(new Campo("CNV_PAZ_CODICE_OLD", (convocazioneEliminata.Paz_codice_old == int.MinValue ? string.Empty : convocazioneEliminata.Paz_codice_old.ToString())));
                    recordLog.Campi.Add(new Campo("CNV_DATA", convocazioneEliminata.Data_CNV.ToString()));
                    recordLog.Campi.Add(new Campo("CNV_CNS_CODICE", convocazioneEliminata.Cns_Codice));
                    recordLog.Campi.Add(new Campo("CNV_DATA_INVIO", (convocazioneEliminata.DataInvio == DateTime.MinValue ? string.Empty : convocazioneEliminata.DataInvio.ToString())));
                    recordLog.Campi.Add(new Campo("CNV_DATA_APPUNTAMENTO", (convocazioneEliminata.DataAppuntamento == DateTime.MinValue ? string.Empty : convocazioneEliminata.DataAppuntamento.ToString())));
                    recordLog.Campi.Add(new Campo("CNV_DURATA_APPUNTAMENTO", (convocazioneEliminata.Durata_Appuntamento == int.MinValue ? string.Empty : convocazioneEliminata.Durata_Appuntamento.ToString()) ));
                    recordLog.Campi.Add(new Campo("CNV_TIPO_APPUNTAMENTO", convocazioneEliminata.TipoAppuntamento));
                    recordLog.Campi.Add(new Campo("CNV_PRIMO_APPUNTAMENTO", (convocazioneEliminata.DataPrimoAppuntamento == DateTime.MinValue ? string.Empty : convocazioneEliminata.DataPrimoAppuntamento.ToString())));
                    recordLog.Campi.Add(new Campo("CNV_AMB_CODICE", (convocazioneEliminata.CodiceAmbulatorio == int.MinValue ? string.Empty : convocazioneEliminata.CodiceAmbulatorio.ToString())));
                    recordLog.Campi.Add(new Campo("CNV_ETA_POMERIGGIO", convocazioneEliminata.EtaPomeriggio));
                    recordLog.Campi.Add(new Campo("CNV_RINVIO", convocazioneEliminata.Rinvio));
                    recordLog.Campi.Add(new Campo("CNV_UTE_ID", (convocazioneEliminata.IdUtente == int.MinValue ? string.Empty : convocazioneEliminata.IdUtente.ToString())));
                    recordLog.Campi.Add(new Campo("CNV_CAMPAGNA", convocazioneEliminata.CampagnaVaccinale));
                    
                    recordLogList.Add(recordLog);
                }
            }

            return recordLogList.ToArray();
        }
    }
}
